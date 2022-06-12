using System;
using System.Collections.Generic;
using System.Linq;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Business.ExtensionMethods;
using Gyldendal.Api.CoreData.Common;
using Gyldendal.Api.CoreData.Common.DataProviderInfrastructure;
using Gyldendal.Api.CoreData.Common.Request;
using Gyldendal.Api.CoreData.Common.Utils;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Response;
using Gyldendal.Api.CoreData.Gql.Common;
using Gyldendal.Api.CoreData.GqlToSolrConnector;
using Gyldendal.Api.CoreData.GqlToSolrConnector.Model;
using Gyldendal.Api.CoreData.GqlToSolrConnector.SolrSearch;
using Gyldendal.Api.CoreData.SolrContracts.Product;
using Gyldendal.Api.CoreData.SolrDataProviders.Infrastructure;
using Gyldendal.Api.CoreData.SolrDataProviders.Infrastructure.FilterGenerator;
using Gyldendal.Api.CoreData.SolrDataProviders.Infrastructure.Models;
using Gyldendal.Api.CoreData.SolrDataProviders.Mappings;
using Gyldendal.Api.CoreData.SolrDataProviders.Product;
using Gyldendal.Api.CoreData.SolrDataProviders.Utils;

namespace Gyldendal.Api.CoreData.SolrDataProviders.Work
{
    using Product = SolrContracts.Product.Product;
    using Work = Contracts.Models.Work;

    public class WorkDataProvider : IWorkDataProvider
    {
        private static readonly string WorkIdSolrFieldName = ProductSchemaField.WorkId.GetFieldName();

        private readonly string _solrServerUrl;

        private readonly string _coreName;

        private readonly ISearch<Product> _solrSearch;

        private readonly string[] _mediaTypeNames;

        private readonly Dictionary<GqlOperation, string> _gqlOpToSolrFieldMapping;

        private readonly ResultProcessor _resultProcessor;

        private readonly IFilterGenerator<ProductFilterGenerationInput> _filterGenerator;

        private readonly IFilterInfoToSolrQueryBuilder _solrQueryBuilder;

        public WorkDataProvider(string solrServerUrl, string coreName,
            Dictionary<GqlOperation, string> gqlOpToSolrFieldMapping, string[] mediaTypeNames,
            ResultProcessor resultProcessor,
            IFilterGenerator<ProductFilterGenerationInput> filterGenerator,
            IFilterInfoToSolrQueryBuilder solrQueryBuilder,
            ISearch<Product> solrSearch)
        {
            _solrServerUrl = solrServerUrl;
            _coreName = coreName;
            _gqlOpToSolrFieldMapping = gqlOpToSolrFieldMapping;
            _mediaTypeNames = mediaTypeNames;
            _resultProcessor = resultProcessor;
            _filterGenerator = filterGenerator;
            _solrQueryBuilder = solrQueryBuilder;
            _solrSearch = solrSearch;
        }

        public SearchResponse<Work> Get(WorkProductSearchRequest request)
        {
            var solrConnector =
                Utils.Utils.GetSolrConnector(_coreName, _solrServerUrl, _mediaTypeNames, _gqlOpToSolrFieldMapping, request.CallingWebShop.ToDataScope());

            var productSearchRequest = request.Clone();

            //TODO: Remove productDataProvider dependency
            var productDataProvider = new ProductDataProvider(_coreName, _solrServerUrl,
                _gqlOpToSolrFieldMapping, _mediaTypeNames, _resultProcessor, _filterGenerator, _solrQueryBuilder,
                _solrSearch);

            var searchControlParams = new SearchControlParams
            {
                GroupByWork = true,
                SkipInvalidSaleConfigProd = false,
            };
            var solrProds = productDataProvider.GetProductsFromSolr(productSearchRequest, searchControlParams);
            var distinctWorkIds = solrProds.ItemsFound.Select(p => p.WorkId).Distinct().ToList();
            SearchResult<Product> workProducts = solrProds;
            if (distinctWorkIds.Any())
            {
                workProducts = GetProductsByWorkIds(distinctWorkIds.ToList(), request, solrConnector);
            }

            var works = new List<Work>();
            if (solrProds.ItemsFound.Count > 0)
            {
                works = Utils.Utils.GroupProductsToWorks(workProducts, out _);

                //Post processing products
                works.ForEach(work => work.Products.ForEach(prod => _resultProcessor.ProcessProduct(prod)));
            }

            var priceRangeFacet = new List<KeyValuePair<PriceRange, int>>();
            if (solrProds.Facets.Any(a => a.Key.Equals(ProductSchemaField.DefaultPrice.GetFieldName(), StringComparison.InvariantCultureIgnoreCase)))
            {
                priceRangeFacet = solrProds.Facets
                    .Where(a => a.Key.Equals(ProductSchemaField.DefaultPrice.GetFieldName(), StringComparison.InvariantCultureIgnoreCase))
                    .Select(a => a.Value).ToList().FirstOrDefault().ToPriceRangeFacet();
            }

            return new SearchResponse<Work>
            {
                SearchResults = new Result<Work>
                {
                    CurrentPage = request.Paging.PageIndex,
                    PageSize = request.Paging.PageSize,
                    TotalResults = solrProds.TotalResults,
                    Results = Utils.Utils.PreserveOrder(works, distinctWorkIds)
                },
                Facets = solrProds.Facets.Where(a =>
                        !a.Key.Equals(ProductSchemaField.DefaultPrice.GetFieldName(),
                            StringComparison.InvariantCultureIgnoreCase))
                    .ToDictionary(item => FacetFieldMapping.Map[item.Key], item => item.Value.ToList()),

                PriceRangeFacet = priceRangeFacet
            };
        }

        public Work GetWorkById(WebShop[] webShops, int workId)
        {
            var solrConnector =
                Utils.Utils.GetSolrConnector(_coreName, _solrServerUrl, _mediaTypeNames, _gqlOpToSolrFieldMapping,
                     webShops.First().ToDataScope());

            var solrSearchRequest = GetConnectorRequestForWorkId(webShops, workId);

            var solrProds = solrConnector.Execute(solrSearchRequest);

            if (solrProds.ItemsFound.Count == 0)
            {
                return null;
            }

            var work = Utils.Utils.GroupProductsToWorks(solrProds, out _).FirstOrDefault();

            //Post processing products
            work?.Products.ForEach(prod => _resultProcessor.ProcessProduct(prod));

            return work;
        }

        private static SearchRequest GetConnectorRequest(WorkProductSearchRequest request, SyntaxInfo exprValidationResult)
        {
            var startRow = ((request.Paging.PageIndex) * request.Paging.PageSize);

            return new SearchRequest
            {
                GqlExpressionInfo = exprValidationResult,
                OrderBy = request.OrderBy,
                SortBy = request.SortBy,
                Start = startRow,
                Rows = request.Paging.PageSize,
                GroupByField = WorkIdSolrFieldName,
                Filters = Utils.Utils.GetSolrConnectorFilters(request, false).ToList(),
                FacetFields = FacetFieldMapping.GetFacetFieldNames(request.FacetTypes),
                UseGqlExpressionTree = request.UseGqlExpressionTree
            };
        }

        private static SearchRequest GetConnectorRequestForWorkId(WebShop[] webShops, int workId)
        {
            return new SearchRequest
            {
                GqlExpressionInfo = GetEmptyGqlSyntaxInfo(),
                OrderBy = OrderBy.title,
                SortBy = SortBy.Asc,
                Start = 0,
                Rows = 1,
                GroupByField = WorkIdSolrFieldName,
                Filters = new[]
                {
                    Utils.Utils.GetSolrConnectorFilterInfo(WorkIdSolrFieldName, new[] {workId.ToString()}, false),
                    Utils.Utils.GetWebShopFilterInfo(webShops),
                    Utils.Utils.GetIsSaleConfigAvailableDefaultFilter()
                }.ToList(),
                FacetFields = null
            };
        }

        private static SyntaxInfo GetEmptyGqlSyntaxInfo()
        {
            return new SyntaxInfo
            {
                MainExpression = string.Empty,
                PostProcessingTokensExist = false,
                Result = new ValidationResult
                {
                    IsValidated = true,
                    Message = string.Empty
                }
            };
        }

        private static SearchResult<Product> GetProductsByWorkIds(IEnumerable<int> workIds, WorkProductSearchRequest request, Connector<Product> solrConnector)
        {
            var workSearchRequest = request.Clone();

            if (workSearchRequest.SecondaryWebShops?.Any() ?? false)
                workSearchRequest.WebShops = workSearchRequest.WebShops.Union(workSearchRequest.SecondaryWebShops).ToArray();

            workSearchRequest.Gql = string.Join(" or ", workIds.Select(x => $"{GqlOperation.Work.GetDescription()}({x})"));
            var validatedGql = Utils.Utils.ValidateGql(workSearchRequest.Gql);
            if (!validatedGql.Result.IsValidated)
            {
                return null;
            }

            //because we have already searched using PageIndex, we need to set it 0 otherwise we will not get any results
            workSearchRequest.Paging.PageIndex = 0;
            var solrSearchRequest = GetConnectorRequest(workSearchRequest, validatedGql);
            return solrConnector.Execute(solrSearchRequest);
        }
    }
}