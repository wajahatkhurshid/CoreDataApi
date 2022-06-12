using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Requests;
using Gyldendal.Api.CoreData.Contracts.Response;
using System;
using System.Collections.Generic;

namespace Gyldendal.Api.CoreData.ApiClient
{
    public partial class CoreDataServiceClient
    {
        private const string ProductControllerV1 = "v1/Product";

        private const string ProductControllerV2 = "v2/Product";

        /// <summary>
        /// Get Product Details
        /// </summary>
        /// <param name="webShop"></param>
        /// <param name="isbn"></param>
        /// <param name="productType"></param>
        /// <returns></returns>
        public Product GetProductByIsbn(WebShop webShop, string isbn, ProductType productType)
        {
            var apiUrl = $"v1/Product/GetProductDetails/{webShop}/{isbn}/{productType}";
            return HttpClient.GetAsync<Product>(apiUrl);
        }

        /// <summary>
        /// Gets Product count by data scope based on data profile
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="productDataProfile"></param>
        public int GetProductCountByDataScope(DataScope dataScope, ProductDataProfile productDataProfile)
        {
            var apiUrl = $"v1/Product/GetProductCountByDataScope/{dataScope}/{productDataProfile}";
            return HttpClient.GetAsync<int>(apiUrl);
        }

        /// <summary>
        /// Gets Product details by data scope for given data profile
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public List<T> GetProductsByDataScope<T>(GetProductsByDataScopeRequest request) where T : BaseProductDataProfile
        {
            var apiUrl = $"v1/Product/GetProductsByDataScope/";
            return HttpClient.PostAsync<List<T>, GetProductsByDataScopeRequest>(apiUrl, request);
        }

        /// <summary>
        /// Returns the number of products updated after the given time value,for the given WebShop.
        /// </summary>
        /// <param name="webShop"></param>
        /// <param name="updatedAfter"></param>
        /// <returns></returns>
        public int GetUpdatedProductsCount(WebShop webShop, DateTime updatedAfter)
        {
            var queryString = $"{ProductControllerV1}/GetUpdatedProductsCount/{webShop}/{updatedAfter.Ticks}";

            return HttpClient.GetAsync<int>(queryString);
        }

        /// <summary>
        /// Returns the number of products updated after the given time value,for the given dataScope.
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="updatedAfter"></param>
        /// <returns></returns>
        public int GetUpdatedProductsCount(DataScope dataScope, DateTime updatedAfter)
        {
            var queryString = $"{ProductControllerV1}/GetUpdatedProductsCountByDataScope/{dataScope}/{updatedAfter.Ticks}";

            return HttpClient.GetAsync<int>(queryString);
        }

        /// <summary>
        /// Returns the asked page of ProductupdatedInfo objects, for each product, related to the given WebShop, that was updated after the given DateTime, in KoncernDataWebShops database.
        /// </summary>
        /// <param name="webShop"></param>
        /// <param name="updatedAfter"></param>
        /// <param name="pageIndex">Minimum value 1.</param>
        /// <param name="pageSize">Minimum value 1.</param>
        /// <returns></returns>
        public IEnumerable<ProductUpdateInfo> GetProductsUpdateInfo(WebShop webShop, DateTime updatedAfter, int pageIndex, int pageSize)
        {
            var queryString = $"{ProductControllerV1}/GetProductsUpdateInfo/{webShop}/{updatedAfter.Ticks}/{pageIndex}/{pageSize}";

            return HttpClient.GetAsync<IEnumerable<ProductUpdateInfo>>(queryString);
        }

        /// <summary>
        /// Returns the asked page of ProductupdatedInfo objects, for each product, related to the given dataScope, that was updated after the given DateTime, in KoncernDataWebShops database.
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="updatedAfter"></param>
        /// <param name="pageIndex">Minimum value 1.</param>
        /// <param name="pageSize">Minimum value 1.</param>
        /// <returns></returns>
        public IEnumerable<ProductUpdateInfo> GetProductsUpdateInfo(DataScope dataScope, DateTime updatedAfter, int pageIndex, int pageSize)
        {
            var queryString = $"{ProductControllerV1}/GetProductsUpdateInfoByDataScope/{dataScope}/{updatedAfter.Ticks}/{pageIndex}/{pageSize}";

            return HttpClient.GetAsync<IEnumerable<ProductUpdateInfo>>(queryString);
        }

        /// <summary>
        /// Returns a list of Product objects, against the passed in Isbns.
        /// </summary>
        /// <param name="isbns"></param>
        /// <param name="skipInvalidSaleConfigProds"></param>
        /// <returns></returns>
        public IEnumerable<Product> GetAllProductsByIsbn(IEnumerable<string> isbns, bool skipInvalidSaleConfigProds = true)
        {
            var apiUrl = $"{ProductControllerV1}/GetAllProductsByIsbns/{skipInvalidSaleConfigProds}?";
            return HttpClient.PostAsync<IEnumerable<Product>, IEnumerable<string>>(apiUrl, isbns);
        }

        /// <summary>
        /// Returns a list of Product based on GQL (Source: Solr).
        /// </summary>
        /// <param name="request"></param>
        /// <param name="skipInvalidSaleConfigProds"></param>
        /// <returns></returns>
        public Result<Product> GetProductsByGql(GetProductsByGqlRequest request, bool skipInvalidSaleConfigProds = true)
        {
            var apiUrl = $"{ProductControllerV1}/GetProductsByGql/{skipInvalidSaleConfigProds}?";
            return HttpClient.PostAsync<Result<Product>, GetProductsByGqlRequest>(apiUrl, request);
        }

        /// <summary>
        /// Returns a list of Product objects, against the passed in Isbns and the WebShop parameter.
        /// </summary>
        /// <param name="webShop"></param>
        /// <param name="isbns"></param>
        /// <param name="skipInvalidSaleConfigProds"></param>
        /// <returns></returns>
        public IEnumerable<Product> GetProductsByIsbn(WebShop webShop, IEnumerable<string> isbns, bool skipInvalidSaleConfigProds = true)
        {
            var apiUrl = $"{ProductControllerV1}/GetProductsByIsbns/{skipInvalidSaleConfigProds}?webShop={webShop}";
            return HttpClient.PostAsync<IEnumerable<Product>, IEnumerable<string>>(apiUrl, isbns);
        }

        /// <summary>
        /// Returns a list of Product objects, against the passed in Isbns and the WebShops parameter.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Product> GetProductsByIsbn(GetProductRequestV2 request)
        {
            var apiUrl = $"{ProductControllerV2}/GetProductsByIsbns/";
            return HttpClient.PostAsync<IEnumerable<Product>, GetProductRequestV2>(apiUrl, request);
        }

        /// <summary>
        /// Returns alist of Product objects from Koncerndata, against the passed in Isbns and the WebShop parameter.
        /// </summary>
        /// <param name="isbns"></param>
        /// <param name="webShop"></param>
        /// <param name="getImageUrl"></param>
        /// <returns></returns>
        public IEnumerable<ProductBasicData> GetLicensedProductsByIsbn(IEnumerable<string> isbns, WebShop webShop, bool getImageUrl = true)
        {
            var apiUrl = $"{ProductControllerV1}/GetLicensedProductsByIsbn?webshop={webShop}&getImageUrl={getImageUrl}";
            return HttpClient.PostAsync<IEnumerable<ProductBasicData>, IEnumerable<string>>(apiUrl, isbns);
        }

        /// <summary>
        /// Search products without any default groupBy i.e. Work       (Source: Solr)
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Facets, Paging, Products(without merging in work)</returns>
        public SearchResponse<Product> AbsoluteSearch(ProductSearchRequest request)
        {
            var apiUrl = $"{ProductControllerV1}/AbsoluteSearch";
            return HttpClient.PostAsync<SearchResponse<Product>, ProductSearchRequest>(apiUrl, request);
        }

        /// <summary>
        /// Search products with data scope...
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public SearchResponse<Product> AbsoluteSearchV2(ProductSearchRequestEx request)
        {
            var apiUrl = $"{ProductControllerV2}/AbsoluteSearch";
            return HttpClient.PostAsync<SearchResponse<Product>, ProductSearchRequestEx>(apiUrl, request);
        }

        /// <summary>
        /// Return the List of bundle Ids which have specific Isbn
        /// </summary>
        /// <param name="webShop"></param>
        /// <param name="isbn"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Result<string> GetBundleIdsByIsbn(WebShop webShop, string isbn, int pageIndex, int pageSize)
        {
            var queryString = $"{ProductControllerV1}/GetBundleIdsByIsbn/{webShop}/{isbn}/{pageIndex}/{pageSize}";

            return HttpClient.GetAsync<Result<string>>(queryString);
        }

        /// <summary>
        /// Determines if the Web Shop's data will be available in Koncerndata for the next x minutes.
        /// </summary>
        /// <param name="webShop"></param>
        /// <param name="xMinutes">The number of minutes by which the Web Shop data will be available or not.</param>
        /// <returns></returns>
        public bool IsShopDataAvailableForXMinutes(WebShop webShop, short xMinutes)
        {
            var queryString = $"{ProductControllerV1}/IsShopDataAvailableForXMinutes/{webShop}/{xMinutes}";
            return HttpClient.GetAsync<bool>(queryString);
        }

        /// <summary>
        /// Returns a list of Products objects based on the search request criteria.
        /// </summary>
        /// <param name="request">Product Search Request information, containing web shop, product type, search string and pagging information</param>
        /// <returns>Product Search Response containing current page information and result records of the page</returns>
        public SearchResponse<Work> SearchProducts(ProductSearchRequest request)
        {
            var addressSuffix = $"{ProductControllerV1}/Search";

            return HttpClient.PostAsync<SearchResponse<Work>, ProductSearchRequest>(addressSuffix, request);
        }

        /// <summary>
        /// Returns a list of Products objects based on the search request criteria.
        /// </summary>
        /// <param name="request">Product Search Request information, containing web shop, product type, search string and pagging information</param>
        /// <returns>Product Search Response containing current page information and result records of the page</returns>
        public SearchResponse<Work> SearchProductsV2(ProductSearchRequestV2 request)
        {
            var addressSuffix = $"{ProductControllerV2}/Search";

            return HttpClient.PostAsync<SearchResponse<Work>, ProductSearchRequestV2>(addressSuffix, request);
        }

        public ProductSearchResponse SearchProductsByFixedCriteria(ProductSearchRequest request)
        {
            var addressSuffix = $"{ProductControllerV1}/SearchByFixedCriteria";

            return HttpClient.PostAsync<ProductSearchResponse, ProductSearchRequest>(addressSuffix, request);
        }

        /// <summary>
        /// Gets count of the products for which campaigns exist
        /// </summary>
        /// <param name="webshop">Webshop</param>
        /// <param name="updatedAfter">Start date of campaign</param>
        /// <returns>Count of campaign affected products</returns>
        public int GetCampaignProductsCount(WebShop webshop, DateTime updatedAfter)
        {
            var requestString = $"{ProductControllerV1}/GetCampaignProdsCount/{webshop}/{updatedAfter.Ticks}";

            return HttpClient.GetAsync<int>(requestString);
        }

        /// <summary>
        /// Gets all the products for which some campaign exists
        /// </summary>
        /// <param name="webshop"></param>
        /// <param name="updatedAfter"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns>List of string containing product ids</returns>
        public IEnumerable<string> GetCampaignProducts(WebShop webshop, DateTime updatedAfter, int pageIndex, int pageSize)
        {
            var requestString = $"{ProductControllerV1}/GetCampaignProds/{webshop}/{updatedAfter.Ticks}/{pageIndex}/{pageSize}";

            return HttpClient.GetAsync<IEnumerable<string>>(requestString);
        }

        /// <summary>
        /// Checks whether a product has any active campaign
        /// </summary>
        /// <param name="webShop"></param>
        /// <param name="productId">Product ID / ISBN</param>
        /// <returns>Returns whether a product has one or more active campaigns</returns>
        public bool HasActiveCampaign(WebShop webShop, string productId)
        {
            var requestString = $"{ProductControllerV1}/HasActiveCampaign/{webShop}/{productId}";

            return HttpClient.GetAsync<bool>(requestString);
        }

        public string GetSupplementaryData(string isbn)
        {
            var queryString = $"{ProductControllerV1}/GetSupplementaryData/{isbn}";
            return HttpClient.GetAsync<string>(queryString);
        }

        public ProductAccessControlType GetProductAccessType(string id)
        {
            var queryString = $"{ProductControllerV1}/GetProductAccessType/{id}";
            return HttpClient.GetAsync<ProductAccessControlType>(queryString);
        }

        public Dictionary<string, List<WebShop>> GetProductWebshops(List<string> isbns)
        {
            var addressSuffix = $"{ProductControllerV1}/GetProductWebshops";

            return HttpClient.PostAsync<Dictionary<string, List<WebShop>>, List<string>>(addressSuffix, isbns);
        }
        /// <summary>
        /// Gets modified Bundle campaigns
        /// </summary>
        /// <param name="webshop"></param>
        /// <param name="updatedAfter"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns>List of string containing product ids</returns>
        public IEnumerable<string> GetUpdatedBundleCampaignsInfo(WebShop webshop, DateTime updatedAfter, int pageIndex, int pageSize)
        {
            var requestString = $"{ProductControllerV1}/GetUpdatedBundleCampaignsInfo/{webshop}/{updatedAfter.Ticks}/{pageIndex}/{pageSize}";

            return HttpClient.GetAsync<IEnumerable<string>>(requestString);
        }

        /// <summary>
        /// Gets count of the modified bundle campaigns
        /// </summary>
        /// <param name="webshop">Webshop</param>
        /// <param name="updatedAfter">Start date of campaign</param>
        /// <returns>Count of campaign affected products</returns>
        public int GetBundleCampaignsCount(WebShop webshop, DateTime updatedAfter)
        {
            var requestString = $"{ProductControllerV1}/GetBundleCampaignsCount/{webshop}/{updatedAfter.Ticks}";

            return HttpClient.GetAsync<int>(requestString);
        }
    }
}