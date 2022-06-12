using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Common.Request;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Requests;
using Gyldendal.Api.CoreData.Gql.Common;
using Gyldendal.Api.CoreData.GqlToSolrConnector;
using Gyldendal.Api.CoreData.GqlToSolrConnector.Model;
using Gyldendal.Api.CoreData.GqlToSolrConnector.SolrSearch;
using Gyldendal.Api.CoreData.GqlValidator;
using Gyldendal.Api.CoreData.SolrContracts.Contributor;
using Gyldendal.Api.CoreData.SolrContracts.Product;
using Gyldendal.Api.CoreData.SolrDataProviders.Mappings;
using NewRelic.Api.Agent;
using SolrNet;
using System.Collections.Generic;
using System.Linq;
using FilterInfo = Gyldendal.Api.CoreData.GqlToSolrConnector.Model.FilterInfo;

namespace Gyldendal.Api.CoreData.SolrDataProviders.Utils
{
    public static class Utils
    {
        private static readonly string WorkIdSolrFieldName = ProductSchemaField.WorkId.GetFieldName();

        private static readonly string WebsiteIdSolrFieldName = ProductSchemaField.WebsiteId.GetFieldName();

        private static readonly string IsSaleConfigAvailableSolrFieldName = ProductSchemaField.IsSaleConfigAvailable.GetFieldName();

        [Trace]
        public static List<Contracts.Models.Work> PreserveOrder(List<Contracts.Models.Work> works, IList<int> originalOrder)
        {
            return originalOrder.Select(work => works.FirstOrDefault(w => w.Id == work)).ToList();
        }

        public static List<Contracts.Models.Product> PreserveProductOrder(List<Contracts.Models.Product> products, IList<int> originalOrder)
        {
            return originalOrder.Select(workId => products.FirstOrDefault(p => p.WorkId == workId)).ToList();
        }

        public static List<OrderByParam> GetOrderByParams(ProductFixSearchRequest request)
        {
            var orderByParams = new List<OrderByParam>();
            request.OrderBy.GetOrderByParams(request.SortBy, ref orderByParams);
            return orderByParams;
        }

        [Trace]
        public static IEnumerable<FilterInfo> GetSolrConnectorFilters(WorkProductSearchRequest request, bool skipInvalidSaleConfigProds)
        {
            return GetSolrConnectorFilters(request.WebShops, request.Filters, skipInvalidSaleConfigProds);
        }

        [Trace]
        public static IEnumerable<FilterInfo> GetSolrConnectorFilters(WebShop[] webShops, Dictionary<WorkProductSearchFilter,
            List<string>> filters, bool skipInvalidSaleConfigProds)
        {
            var retVal = new List<FilterInfo>();

            if (filters?.Any() ?? false)
            {
                retVal = filters.Select(x => GetSolrConnectorFilterInfo(FilterFieldMapping.Map[x.Key], x.Value, true)).ToList();
            }

            if (webShops?.Any() ?? false)
            {
                var validWebShops = webShops.ToArray();
                retVal.Add(GetWebShopFilterInfo(validWebShops));
            }

            if (skipInvalidSaleConfigProds)
            { return retVal; }

            retVal.Add(GetIsSaleConfigAvailableDefaultFilter());
            return retVal;
        }

        [Trace]
        public static FilterInfo GetSolrConnectorFilterInfo(string solrFieldName, IEnumerable<string> filterValues, bool excludeFromFacets, string @operator = "AND")
        {
            return new FilterInfo
            {
                SolrFieldName = solrFieldName,
                FilterValues = filterValues,
                ExcludeFromFacets = excludeFromFacets,
                QueryOperator = @operator
            };
        }

        [Trace]
        public static FilterInfo GetWebShopFilterInfo(WebShop[] webShops, bool excludeFromFacets = false)
        {
            return GetSolrConnectorFilterInfo(WebsiteIdSolrFieldName, webShops.Select(x => $"{x:D}").ToArray(), excludeFromFacets, "OR");
        }

        [Trace]
        public static FilterInfo GetIsSaleConfigAvailableDefaultFilter()
        {
            return GetSolrConnectorFilterInfo(IsSaleConfigAvailableSolrFieldName, new[] { "true" }, false);
        }

        [Trace]
        public static Connector<SolrContracts.Product.Product> GetSolrConnector(string coreName, string solrUrl, string[] mediaTypeNames,
            Dictionary<GqlOperation, string> gqlOpToSolrFieldMapping, DataScope dataScope)
        {
            var solrSearch = new GenericSolrSearch<SolrContracts.Product.Product>(coreName, solrUrl);

            var solrConnector = new Connector<SolrContracts.Product.Product>(
                solrSearch,
                mediaTypeNames,
                gqlOpToSolrFieldMapping,
                dataScope
            );
            return solrConnector;
        }

        [Trace]
        public static List<Contracts.Models.Work> GroupProductsToWorks(SearchResult<SolrContracts.Product.Product> prodResults, out int totalGroups)
        {
            var workIdGrouping = prodResults.ItemsFound.Grouping[WorkIdSolrFieldName];
            totalGroups = workIdGrouping.Ngroups.GetValueOrDefault(0);
            return workIdGrouping.Groups.Select(x => x.ToCoreDataWork()).ToList();
        }

        [Trace]
        public static SyntaxInfo ValidateGql(string gql)
        {
            if (string.IsNullOrWhiteSpace(gql))
            {
                return new SyntaxInfo
                {
                    Result = new ValidationResult
                    {
                        IsValidated = true,
                        Message = string.Empty
                    },
                    MainExpression = string.Empty,
                    PostProcessingExpression = string.Empty,
                    PostProcessingTokensExist = false,
                    PostProcessTokens = null
                };
            }

            return new ExpressionValidator().Validate(gql);
        }

        [Trace]
        public static SolrQueryByField ToSolrQuery(this ContributorSchemaField schemaField, string value, bool isWildCard = false)
        {
            var fieldVal = value;
            if (isWildCard)
            {
                fieldVal = $"*{fieldVal}*";
            }
            else
            {
                fieldVal = '"' + fieldVal + '"';
            }

            return new SolrQueryByField(schemaField.GetFieldName(), fieldVal) { Quoted = false };
        }

        public static int GetFieldBoost(this ContributorSchemaField field)
        {
            int boost;

            switch (field)
            {
                case ContributorSchemaField.ExactMatch:
                    boost = 100;
                    break;

                case ContributorSchemaField.SubstringField:
                    boost = 50;
                    break;

                default:
                    boost = 1;
                    break;
            }

            return boost;
        }
    }
}