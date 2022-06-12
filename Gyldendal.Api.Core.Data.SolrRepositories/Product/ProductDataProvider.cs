using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Business.ExtensionMethods;
using Gyldendal.Api.CoreData.Common;
using Gyldendal.Api.CoreData.Common.DataProviderInfrastructure;
using Gyldendal.Api.CoreData.Common.Request;
using Gyldendal.Api.CoreData.Common.Utils;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Requests;
using Gyldendal.Api.CoreData.Contracts.Response;
using Gyldendal.Api.CoreData.Gql.Common;
using Gyldendal.Api.CoreData.GqlToSolrConnector;
using Gyldendal.Api.CoreData.GqlToSolrConnector.CriteriaExtraction;
using Gyldendal.Api.CoreData.GqlToSolrConnector.Model;
using Gyldendal.Api.CoreData.GqlToSolrConnector.SolrSearch;
using Gyldendal.Api.CoreData.SolrContracts.Product;
using Gyldendal.Api.CoreData.SolrDataProviders.Infrastructure;
using Gyldendal.Api.CoreData.SolrDataProviders.Infrastructure.FilterGenerator;
using Gyldendal.Api.CoreData.SolrDataProviders.Infrastructure.Models;
using Gyldendal.Api.CoreData.SolrDataProviders.Mappings;
using Gyldendal.Api.CoreData.SolrDataProviders.Utils;
using Gyldendal.Common.WebUtils.Exceptions;
using NewRelic.Api.Agent;
using SolrNet;
using FilterInfo = Gyldendal.Api.CoreData.GqlToSolrConnector.Model.FilterInfo;
using SolrQueryEngineExpressionTree = Gyldendal.Api.CoreData.GqlToSolrConnector.QueryParsingV3.SolrQueryEngine;
using SolrQueryEngineSimple = Gyldendal.Api.CoreData.GqlToSolrConnector.QueryParsingV2.SolrQueryEngine;

namespace Gyldendal.Api.CoreData.SolrDataProviders.Product
{
    public partial class ProductDataProvider : BaseDataProvider<ProductFilterGenerationInput>, IProductDataProvider
    {
        private static readonly string WorkIdSolrFieldName = ProductSchemaField.WorkId.GetFieldName();

        private readonly ISearch<SolrContracts.Product.Product> _solrSearch;

        private readonly string[] _mediaTypeNames;

        private readonly Dictionary<GqlOperation, string> _gqlOpToSolrFieldMapping;

        private readonly ResultProcessor _resultProcessor;

        public ProductDataProvider(string coreName, string solrServerUrl,
            Dictionary<GqlOperation, string> gqlOpToSolrFieldMapping,
            string[] mediaTypeNames,
            ResultProcessor resultProcessor,
            IFilterGenerator<ProductFilterGenerationInput> filterGenerator,
            IFilterInfoToSolrQueryBuilder solrQueryBuilder,
            ISearch<SolrContracts.Product.Product> solrSearch)
            : base(solrQueryBuilder, filterGenerator, solrServerUrl,
                coreName)
        {
            _gqlOpToSolrFieldMapping = gqlOpToSolrFieldMapping;
            _mediaTypeNames = mediaTypeNames;
            _resultProcessor = resultProcessor;
            _solrSearch = solrSearch;
        }

        public ProductSearchResponse Search(ProductFixSearchRequest request)
        {
            var filters = GenerateSolrQuery(new ProductFilterGenerationInput
            {
                WebShops = request.Webshop,
                SearchString = request.SearchString,
                ProductSearchType = request.ProductSearchType
            });

            var result = _solrSearch.Search(SolrQuery.All, request.Paging.PageIndex * request.Paging.PageSize,
                request.Paging.PageSize, Utils.Utils.GetOrderByParams(request), null, null, filters);

            return new ProductSearchResponse
            {
                CurrentPage = request.Paging.PageIndex,
                PageSize = request.Paging.PageSize,
                Results = GetProductsFromResult(result),
                TotalResults = result.TotalResults
            };
        }

        public IEnumerable<Contracts.Models.Product> GetProductsByIsbns(IEnumerable<string> isbns, bool skipInvalidSaleConfigProds)
        {
            var isbnsList = isbns.ToList();

            //var solrSearch = GetSolrDataReader();
            var filters = GenerateSolrQuery(new ProductFilterGenerationInput
            {
                Isbns = isbnsList,
                SkipInvalidSaleConfigProds = skipInvalidSaleConfigProds
            });

            var result = _solrSearch.Search(SolrQuery.All, 0, 0, null, null, null, filters);

            return GetProductsFromResult(result);
        }

        /// <summary>
        /// Search products in solr against isbns in provided webshops
        /// </summary>
        /// <param name="webShops"></param>
        /// <param name="isbns"></param>
        /// <param name="skipInvalidSaleConfigProds"></param>
        /// <returns></returns>
        public IEnumerable<Contracts.Models.Product> GetProductsByIsbns(WebShop[] webShops,
            IEnumerable<string> isbns, bool skipInvalidSaleConfigProds)
        {
            var isbnsList = isbns.ToList();
            var webshopList = webShops.ToList();

            var filters = GenerateSolrQuery(new ProductFilterGenerationInput
            {
                WebShops = webshopList,
                Isbns = isbnsList,
                SkipInvalidSaleConfigProds = skipInvalidSaleConfigProds
            });

            var result = _solrSearch.Search(SolrQuery.All, 0, 0, null, null, null, filters);

            return GetProductsFromResult(result);
        }

        public IEnumerable<Contracts.Models.Product> GetProductsByIsbns(WebShop webShop, IEnumerable<string> isbns, bool skipInvalidSaleConfigProds)
        {
            var isbnsList = isbns.ToList();

            var filters = GenerateSolrQuery(new ProductFilterGenerationInput
            {
                WebShops = new List<WebShop> { webShop },
                Isbns = isbnsList,
                SkipInvalidSaleConfigProds = skipInvalidSaleConfigProds
            });

            var result = _solrSearch.Search(SolrQuery.All, 0, 0, null, null, null, filters);

            return GetProductsFromResult(result);
        }

        [Trace]
        public SearchResult<SolrContracts.Product.Product> GetProductsFromSolr(WorkProductSearchRequest request,
            SearchControlParams controlParams)
        {
            var dataScope = request.CallingWebShop.ToDataScope();
            var solrConnector = Utils.Utils.GetSolrConnector(CoreName, SolrServerUrl, _mediaTypeNames, _gqlOpToSolrFieldMapping, dataScope);

            var preprocessResponse = PreProcessRequest(request, solrConnector, controlParams);
            if (preprocessResponse == null)
            {
                return null;
            }

            var exprValidationResult = Utils.Utils.ValidateGql(preprocessResponse.Gql);
            if (!exprValidationResult.Result.IsValidated)
            {
                return null;
            }

            SetRequestDefaultValues(preprocessResponse);

            var solrSearchRequest = GetConnectorRequest(preprocessResponse, exprValidationResult, controlParams);

            var solrProds = solrConnector.Execute(solrSearchRequest);
            return solrProds;
        }

        /// <summary>
        /// Search products without any default groupBy i.e. Work       (Source: Solr)
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Facets, Paging, Products(without merging in work)</returns>
        /// <exception cref="ValidationException"></exception>
        public SearchResponse<Contracts.Models.Product> GetProducts(WorkProductSearchRequest request)
        {
            var searchControlOptions = new SearchControlParams
            {
              GroupByWork = false,
              SkipInvalidSaleConfigProd = true,
            };
            var solrProds = GetProductsFromSolr(request, searchControlOptions);
          
            return new SearchResponse<Contracts.Models.Product>
            {
                SearchResults = new Result<Contracts.Models.Product>
                {
                    CurrentPage = request.Paging.PageIndex,
                    PageSize = request.Paging.PageSize,
                    TotalResults = solrProds.TotalResults,
                    Results = GetProductsFromResult(solrProds)
                },
                Facets = solrProds.Facets.ToDictionary(item => FacetFieldMapping.Map[item.Key], item => item.Value.ToList())
            };
        }

        public SearchResponse<Contracts.Models.Product> Get(WorkProductSearchRequest request)
        {
            var searchControlParams = new SearchControlParams
            {
                GroupByWork = true,
                SkipInvalidSaleConfigProd = false,
            };
            var solrProds = GetProductsFromSolr(request, searchControlParams);
            var distinctWorks = solrProds.ItemsFound.Select(p => p.WorkId).Distinct();
            var workIds = distinctWorks as IList<int> ?? distinctWorks.ToList();
            var workProducts = solrProds;
            return new SearchResponse<Contracts.Models.Product>
            {
                SearchResults = new Result<Contracts.Models.Product>
                {
                    CurrentPage = request.Paging.PageIndex,
                    PageSize = request.Paging.PageSize,
                    TotalResults = solrProds.TotalResults,
                    Results = Utils.Utils.PreserveProductOrder(GetProductsFromResult(workProducts), workIds)
                },
                Facets = solrProds.Facets.ToDictionary(item => FacetFieldMapping.Map[item.Key], item => item.Value.ToList())
            };
        }

        [Trace]
        public SearchResponse<Contracts.Models.Work> GetProductsAsWork(WorkProductSearchRequest request)
        {
            var searchControlParams = new SearchControlParams
            {
                GroupByWork = true,
                SkipInvalidSaleConfigProd = false,
            };
            var solrProds = GetProductsFromSolr(request, searchControlParams);
            var distinctWorks = solrProds.ItemsFound.Select(p => p.WorkId).Distinct();
            var workIds = distinctWorks as IList<int> ?? distinctWorks.ToList();
            var workProducts = solrProds;
            var works = new List<Contracts.Models.Work>();
            if (solrProds.ItemsFound.Count > 0)
            {
                works = Utils.Utils.GroupProductsToWorks(workProducts, out _);

                //Post processing products
                works.ForEach(x => x.Products.ForEach(y => _resultProcessor.ProcessProduct(y)));
            }

            return MapToSearchResponse(solrProds, works, workIds, request);
        }

        [Trace]
        public SearchResponse<Contracts.Models.Work> MapToSearchResponse(SearchResult<SolrContracts.Product.Product> solrProds, List<Contracts.Models.Work> works, IList<int> workIds, WorkProductSearchRequest request)
        {
            var priceRangeFacet = new List<KeyValuePair<PriceRange, int>>();
            if (solrProds.Facets.Any(a => a.Key.Equals(ProductSchemaField.DefaultPrice.GetFieldName(), StringComparison.InvariantCultureIgnoreCase)))
            {
                priceRangeFacet = solrProds.Facets
                    .Where(a => a.Key.Equals(ProductSchemaField.DefaultPrice.GetFieldName(), StringComparison.InvariantCultureIgnoreCase))
                    .Select(a => a.Value).ToList().FirstOrDefault().ToPriceRangeFacet();
            }

            return new SearchResponse<Contracts.Models.Work>
            {
                SearchResults = new Result<Contracts.Models.Work>
                {
                    CurrentPage = request.Paging.PageIndex,
                    PageSize = request.Paging.PageSize,
                    TotalResults = solrProds.TotalResults,
                    Results = Utils.Utils.PreserveOrder(works, workIds)
                },
                Facets = solrProds.Facets.Where(a =>
                        !a.Key.Equals(ProductSchemaField.DefaultPrice.GetFieldName(),
                            StringComparison.InvariantCultureIgnoreCase))
                    .ToDictionary(item => FacetFieldMapping.Map[item.Key], item => item.Value.ToList()),

                PriceRangeFacet = priceRangeFacet
            };
        }

        public List<WebShop> GetWebsiteIdsByContributorId(string contributorId, DataScope dataScope)
        {
            var filters = GenerateSolrQuery(new ProductFilterGenerationInput
            {
                ContributorId = contributorId,
                WebShops = dataScope.ToWebShops()
            });

            var field = ProductSchemaField.WebsiteId.GetFieldName();

            var result = _solrSearch.SearchSolrQueryResults(SolrQuery.All, 0, 0, null, null, null, filters, null, new[] { field });
            return result.Select(itemFound => (WebShop)itemFound.WebsiteId).Distinct().ToList();
        }

        [Trace]
        private static SearchRequest GetConnectorRequest(WorkProductSearchRequest request, SyntaxInfo exprValidationResult,
            SearchControlParams controlParams)
        {
            var startRow = ((request.Paging.PageIndex) * request.Paging.PageSize);

            var searchRequest = new SearchRequest
            {
                GqlExpressionInfo = exprValidationResult,
                OrderBy = request.OrderBy,
                SortBy = request.SortBy,
                Start = startRow,
                Rows = request.Paging.PageSize,
                GroupByField = controlParams.GroupByWork ? WorkIdSolrFieldName : null,
                Filters = Utils.Utils.GetSolrConnectorFilters(request, controlParams.SkipInvalidSaleConfigProd).ToList(),
                FacetFields = FacetFieldMapping.GetFacetFieldNames(request.FacetTypes),
                PriceRangeFacetParams = request.PriceRangeFacetParams,
                UseGqlExpressionTree = request.UseGqlExpressionTree
            };

            var priceFilter = GetPriceRangeFilters(request.PriceRangeFilters);

            if (!priceFilter.Any()) return searchRequest;

            if (searchRequest.Filters == null)
                searchRequest.Filters = priceFilter;
            else
                searchRequest.Filters.AddRange(priceFilter);

            return searchRequest;
        }

        [Trace]
        private static List<FilterInfo> GetPriceRangeFilters(List<PriceRange> priceRangeFilters)
        {
            var priceFilters = new List<FilterInfo>();

            if (priceRangeFilters == null) return priceFilters;
            // For single filter with 0  as both ranges, ignore filter. If multiple with invalid ranges, throw exception.
            if (!ValidatePriceRangeFilters(priceRangeFilters)) return priceFilters;

            var filter = new FilterInfo
            {
                SolrFieldName = "defaultPrice",
                Quoted = false,
                ExcludeFromFacets = true
            };

            var filterValues = priceRangeFilters.Select(GetPriceFilter).ToList();

            filter.FilterValues = filterValues;

            priceFilters.Add(filter);

            return priceFilters;
        }

        private static string GetPriceFilter(PriceRange priceFilter)
        {
            var from = priceFilter.From.ToString(CultureInfo.InvariantCulture);
            var to = priceFilter.To.ToString(CultureInfo.InvariantCulture);
            return $"[{from},DKK TO {to},DKK]";
        }

        [Trace]
        private static bool ValidatePriceRangeFilters(List<PriceRange> priceRangeFilters)
        {
            if (priceRangeFilters.Count == 1)   // If single filter provided with range limits as 0, return status and let caller handle it.
                return IsValidPriceRange(priceRangeFilters.FirstOrDefault());

            // In case multiple price range filters are provided with range limits as 0, throw exception as the input is invalid.
            if (priceRangeFilters.All(IsValidPriceRange)) return true;

            throw new ValidationException((ulong)ErrorCodes.InvalidPriceFilter,
                ErrorCodes.InvalidPriceFilter.GetDescription(), Extensions.CoreDataSystemName, null);
        }

        [Trace]
        private static bool IsValidPriceRange(PriceRange priceRange)
        {
            return priceRange.From < priceRange.To && !(priceRange.From < 0 && priceRange.To < 0);
        }

        [Trace]
        private WorkProductSearchRequest PreProcessRequest(WorkProductSearchRequest searchRequest, 
            Connector<SolrContracts.Product.Product> solrConnector, SearchControlParams controlParams)
        {
            var validatedGql = Utils.Utils.ValidateGql(searchRequest.Gql);
            if (!validatedGql.Result.IsValidated)
            {
                return null;
            }

            var searchRequestV2 = searchRequest.Clone();

            var tokenCriteria = new GqlTokenToCriteriaConverter(_gqlOpToSolrFieldMapping);
            var solrSearchRequest = GetConnectorRequest(searchRequest, validatedGql, controlParams);

            List<SearchCriteria> tokens;
            if (searchRequest.UseGqlExpressionTree)
            {
                var queryParser = new SolrQueryEngineExpressionTree(tokenCriteria);
                queryParser.ParseExpression(solrSearchRequest.GqlExpressionInfo.Result, solrSearchRequest.DefaultSearchCriteria);
                tokens = queryParser.Tokens;
            }
            else
            {
                var queryParser = new SolrQueryEngineSimple(tokenCriteria);
                queryParser.ParseExpression(solrSearchRequest.GqlExpressionInfo.Result, solrSearchRequest.DefaultSearchCriteria);
                tokens = queryParser.Tokens;
            }
            var relatedProductToken = tokens.FirstOrDefault(t => t.GqlOperation == GqlOperation.RelatedProducts);
            if (relatedProductToken == null)
            {
                return searchRequestV2;
            }

            var sbWebShop = new StringBuilder();
            foreach (var webShop in searchRequest.WebShops)
            {
                sbWebShop.Append(string.Join("_", relatedProductToken.Value, (int)webShop));
                sbWebShop.Append(" OR ");
            }

            var fieldValue = sbWebShop.ToString().Substring(0, sbWebShop.ToString().LastIndexOf(" OR ", StringComparison.Ordinal));
            //var fieldValue = string.Join("_", relatedProductToken.Value, (int)searchRequest.Webshop);
            var results = solrConnector.Execute(new SolrQueryByField(relatedProductToken.FieldName, fieldValue));
            if (results.TotalResults == 0)
            {
                return null;
            }

            var product = results.ItemsFound.FirstOrDefault();
            if (product == null)
            {
                return searchRequestV2;
            }

            if (product.Areas != null)
            {
                searchRequestV2.Filters.AddFilter(WorkProductSearchFilter.Areas, product.Areas);
            }

            if (product.SubAreas != null)
            {
                searchRequestV2.Filters.AddFilter(WorkProductSearchFilter.Areas, product.SubAreas);
            }

            searchRequestV2.Gql += $" and not {GqlOperation.Isbn.ToTokenString()}({relatedProductToken.Value})";
            return searchRequestV2;
        }

        /// <summary>
        /// Searches products based upon search Request provided.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Trace]
        private static void SetRequestDefaultValues(WorkProductSearchRequest request)
        {
            if (request.Paging.PageIndex < 0)
            {
                request.Paging.PageIndex = 0;
            }
            if (request.Paging.PageSize < 1)
            {
                request.Paging.PageSize = 50;
            }
        }

        private List<Contracts.Models.Product> GetProductsFromResult(SearchResult<SolrContracts.Product.Product> result)
        {
            var products = new List<Contracts.Models.Product>();
            foreach (var solrProduct in result.ItemsFound)
            {
                var product = solrProduct.ToCoreDataProduct();
                _resultProcessor.ProcessProduct(product);

                products.Add(product);
            }

            return products;
        }
    }
}