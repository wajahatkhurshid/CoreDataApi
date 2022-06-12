using System.Collections.Generic;
using System.Linq;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Business.ExtensionMethods;
using Gyldendal.Api.CoreData.Common;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Requests;
using Gyldendal.Api.CoreData.SolrContracts.Product;
using Gyldendal.Api.CoreData.SolrDataProviders.Infrastructure.Models;
using Gyldendal.Common.WebUtils.Exceptions;
using SolrNet;
using SolrNet.Commands.Parameters;

namespace Gyldendal.Api.CoreData.SolrDataProviders.Product
{
    public partial class ProductDataProvider
    {
        public int GetProductCountByDataScope(DataScope dataScope, ProductDataProfile productDataProfile)
        {
            var filters = GenerateSolrQuery(new ProductFilterGenerationInput
            {
                WebShops = dataScope.ToWebShops(),
            });

            var result = _solrSearch.Search(SolrQuery.All, 0, 1, null,
                GetGroupField(productDataProfile), null, filters);
            // In case of getting count, we get SearchResult<Product> which has field for total count.

            return result.TotalResults;
        }
        
        public IEnumerable<BaseProductDataProfile> GetProductsByDataScope(GetProductsByDataScopeRequest request)
        {
            var fields = GetSelectFields(request.ProductDataProfile);
            
            var filters = GenerateSolrQuery(new ProductFilterGenerationInput
            {
                WebShops = request.DataScope.ToWebShops(),
            });

            var groupField = GetGroupField(request.ProductDataProfile);

            var startRow = request.PageIndex * request.PageSize;

            var result = _solrSearch.SearchSolrQueryResults(SolrQuery.All,
                startRow,
                request.PageSize, null,
                groupField, null,
                filters, null, fields);
            // In case of getting product data, we get SolrQueryResult<Product> where we only have group(s) to access data from.

            return ExtractProdResults(result, request.ProductDataProfile);

        }

        private List<BaseProductDataProfile> ExtractProdResults(SolrQueryResults<SolrContracts.Product.Product> result, ProductDataProfile productDataProfile)
        {
            switch (productDataProfile)
            {
                case ProductDataProfile.IVR:
                    return GetIvrProductInfos(result);

                default:
                    throw new ValidationException((ulong) ErrorCodes.InvalidProductDataProfile,
                        ErrorCodes.InvalidProductDataProfile.GetDescription(), Extensions.CoreDataSystemName, null);
            }
        }

        private static List<BaseProductDataProfile> GetIvrProductInfos(
            SolrQueryResults<SolrContracts.Product.Product> solrResults)
        {
            if (solrResults.Grouping == null || solrResults.Grouping.Count <= 0) return null;

            var serializedIvrProductsList = new List<BaseProductDataProfile>();
            solrResults.Clear();

            var isbnGroups = solrResults.Grouping.First().Value.Groups;

            foreach (var solrProducts in isbnGroups.Select(item => item.Documents.ToList()))
            {
                var ivrProductInfo = GetIvrProductInfo(solrProducts);

                serializedIvrProductsList.Add(ivrProductInfo);
            }
            
            return serializedIvrProductsList;

        }

        private static BaseProductDataProfile GetIvrProductInfo(List<SolrContracts.Product.Product> solrProducts)
        {
            var prodInfo = solrProducts.FirstOrDefault();
            var responseProd = new IvrProductInfo
            {
                Isbn13 = prodInfo?.Isbn13,
                ProductId = prodInfo?.ProductId,
                WebShops = solrProducts.Select(a => (WebShop)a.WebsiteId).Distinct().ToList()
            };
            return responseProd;
        }

        private GroupingParameters GetGroupField(ProductDataProfile prodDataProfile)
        {
            if (prodDataProfile == ProductDataProfile.IVR)
            {
                return new GroupingParameters
                {
                    Fields = new[] { ProductSchemaField.isbn13String.GetFieldName() },
                    Format = GroupingFormat.Grouped,
                    Limit = int.MaxValue,
                    Ngroups = true,
                    Facet = false
                };
            }

            return null;
        }

        private string[] GetSelectFields(ProductDataProfile productDataProfile)
        {
            if (productDataProfile == ProductDataProfile.IVR)
            {
                return new[]
                {
                    ProductSchemaField.ProductId.GetFieldName(),
                    ProductSchemaField.Isbn13.GetFieldName(),
                    ProductSchemaField.WebsiteId.GetFieldName(),
                };
            }

            return Extensions.GetValues<ProductSchemaField>().Select(a => a.GetFieldName()).ToArray();
        }

    }
}
