using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Common.Request;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Requests;
using Gyldendal.Api.CoreData.Contracts.Response;
using System.Collections.Generic;
using Gyldendal.Api.CoreData.Contracts.Enumerations;

namespace Gyldendal.Api.CoreData.Common.DataProviderInfrastructure
{
    public interface IProductDataProvider : IDataProvider<SearchResponse<Product>, WorkProductSearchRequest>
    {
        IEnumerable<Product> GetProductsByIsbns(WebShop webShop, IEnumerable<string> isbns, bool skipInvalidSaleConfigProds);

        IEnumerable<BaseProductDataProfile> GetProductsByDataScope(GetProductsByDataScopeRequest request);

        IEnumerable<Product> GetProductsByIsbns(WebShop[] webShops, IEnumerable<string> isbns, bool skipInvalidSaleConfigProds);

        int GetProductCountByDataScope(DataScope dataScope, ProductDataProfile productDataProfile);

        IEnumerable<Product> GetProductsByIsbns(IEnumerable<string> isbns, bool skipInvalidSaleConfigProds);

        /// <summary>
        /// Searches products based upon search Request provided.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        ProductSearchResponse Search(ProductFixSearchRequest request);

        SearchResponse<Work> GetProductsAsWork(WorkProductSearchRequest request);

        SearchResponse<Product> GetProducts(WorkProductSearchRequest request);

        List<WebShop> GetWebsiteIdsByContributorId(string contributorId, DataScope dataScope);
    }
}