using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Requests;
using Gyldendal.Api.CoreData.Contracts.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gyldendal.Api.CoreData.ApiClient
{
    public interface ICoreDataServiceClient
    {
        /// <summary>
        /// Gets Product details by data scope for given data profile
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        List<T> GetProductsByDataScope<T>(GetProductsByDataScopeRequest request) where T : BaseProductDataProfile;

        /// <summary>
        /// Gets Product count by data scope based on data profile
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="productDataProfile"></param>
        int GetProductCountByDataScope(DataScope dataScope, ProductDataProfile productDataProfile);

        /// <summary>
        /// Calls Core Data Service to get Area by Website
        /// </summary>
        /// <param name="webSite"></param>
        /// <returns></returns>
        List<Area> GetAreas(WebShop webSite);

        /// <summary>
        /// Returns the Sub Areas against the given web shop and the subject id.
        /// </summary>
        /// <param name="webSite"></param>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        List<SubArea> GetSubAreas(WebShop webSite, int subjectId);

        /// <summary>
        /// Calls Core Data Service to get Levels according to website and according to area
        /// </summary>
        /// <param name="webSite"></param>
        /// <param name="areaId"></param>
        /// <returns></returns>
        List<Level> GetLevels(WebShop webSite, int areaId);

        /// <summary>
        /// Calls Core Data Service to get MediaTypes according to website
        /// </summary>
        /// <param name="webSite"></param>
        /// <returns></returns>
        List<MediaType> GetMediaTypes(WebShop webSite);

        /// <summary>
        /// Calls Core Data Service to get MaterialTypes according to website
        /// </summary>
        /// <param name="webSite"></param>
        /// <returns></returns>
        List<MaterialType> GetMaterialTypes(WebShop webSite);

        /// <summary>
        /// Calls Core Data Service to get Subject according to website and according to area
        /// </summary>
        /// <param name="webSite"></param>
        /// <param name="areaId"></param>
        /// <returns></returns>
        List<Subject> GetSubjects(WebShop webSite, int areaId);

        /// <summary>
        /// Get Produt Details
        /// </summary>
        /// <param name="webShop"></param>
        /// <param name="isbn"></param>
        /// <param name="productType"></param>
        /// <returns></returns>
        Product GetProductByIsbn(WebShop webShop, string isbn, ProductType productType);

        /// <summary>
        /// Returns the number of products updated after the given time value,for the given WebShop.
        /// </summary>
        /// <param name="webShop"></param>
        /// <param name="updatedAfter"></param>
        /// <returns></returns>
        int GetUpdatedProductsCount(WebShop webShop, DateTime updatedAfter);

        /// <summary>
        /// Returns the number of products updated after the given time value,for the given WebShop.
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="updatedAfter"></param>
        /// <returns></returns>
        int GetUpdatedProductsCount(DataScope dataScope, DateTime updatedAfter);

        /// <summary>
        /// Returns the asked page of ProductupdatedInfo objects, for each product, related to the given WebShop, that was updated after the given DateTime, in KoncernDataWebShops database.
        /// </summary>
        /// <param name="webShop"></param>
        /// <param name="updatedAfter"></param>
        /// <param name="pageIndex">Minimum value 1.</param>
        /// <param name="pageSize">Minimum value 1.</param>
        /// <returns></returns>
        IEnumerable<ProductUpdateInfo> GetProductsUpdateInfo(WebShop webShop, DateTime updatedAfter, int pageIndex, int pageSize);

        /// <summary>
        /// Returns the asked page of ProductupdatedInfo objects, for each product, related to the given dataScope, that was updated after the given DateTime, in KoncernDataWebShops database.
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="updatedAfter"></param>
        /// <param name="pageIndex">Minimum value 1.</param>
        /// <param name="pageSize">Minimum value 1.</param>
        /// <returns></returns>
        IEnumerable<ProductUpdateInfo> GetProductsUpdateInfo(DataScope dataScope, DateTime updatedAfter, int pageIndex, int pageSize);

        /// <summary>
        /// Returns alist of Product objects, against the passed in Isbns and the WebShop parameter.
        /// </summary>
        /// <param name="webShop"></param>
        /// <param name="isbns"></param>
        /// <param name="skipInvalidSaleConfigProds"></param>
        /// <returns></returns>
        IEnumerable<Product> GetProductsByIsbn(WebShop webShop, IEnumerable<string> isbns, bool skipInvalidSaleConfigProds = true);

        /// <summary>
        /// Returns a list of Product objects, against the passed in Isbns and the WebShops parameter.
        /// </summary>
        /// <returns></returns>
        IEnumerable<Product> GetProductsByIsbn(GetProductRequestV2 request);

        /// <summary>
        /// Returns a list of Product objects, against the passed in Isbns.
        /// </summary>
        /// <param name="isbns"></param>
        /// <param name="skipInvalidSaleConfigProds"></param>
        /// <returns></returns>
        IEnumerable<Product> GetAllProductsByIsbn(IEnumerable<string> isbns, bool skipInvalidSaleConfigProds = true);

        /// <summary>
        /// Returns a list of Product based on GQL (Source: Solr).
        /// </summary>
        /// <param name="request"></param>
        /// <param name="skipInvalidSaleConfigProds"></param>
        /// <returns></returns>
        Result<Product> GetProductsByGql(GetProductsByGqlRequest request, bool skipInvalidSaleConfigProds = true);

        /// <summary>
        /// Returns alist of Product objects from Koncerndata, against the passed in Isbns and the WebShop parameter.
        /// </summary>
        /// <param name="isbns"></param>
        /// <param name="webShop"></param>
        /// <param name="getImageUrl"></param>
        /// <returns></returns>
        IEnumerable<ProductBasicData> GetLicensedProductsByIsbn(IEnumerable<string> isbns, WebShop webShop, bool getImageUrl = true);

        /// <summary>
        /// Search products without any default groupBy i.e. Work       (Source: Solr)
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Facets, Paging, Products(without merging in work)</returns>
        SearchResponse<Product> AbsoluteSearch(ProductSearchRequest request);

        /// <summary>
        /// Search products with data scope...
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        SearchResponse<Product> AbsoluteSearchV2(ProductSearchRequestEx request);

        /// <summary>
        /// Return the List of bundle Ids which have specific Isbn
        /// </summary>
        /// <param name="webShop"></param>
        /// <param name="isbn"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Result<string> GetBundleIdsByIsbn(WebShop webShop, string isbn, int pageIndex, int pageSize);

        /// <summary>
        /// Getting work from Core Data Services
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        SearchResponse<Work> GetWorksSearchResponse(WorkSearchRequest request);

        /// <summary>
        /// Gets a Work object for the given WebShop and Workid parameters.
        /// </summary>
        /// <param name="webShop"></param>
        /// <param name="workId"></param>
        /// <returns></returns>
        Work GetWorkById(WebShop webShop, int workId);

        /// <summary>
        /// Gets a Work object for the given WebShops and Workid parameters.
        /// </summary>
        /// <param name="webShops"></param>
        /// <param name="workId"></param>
        /// <returns></returns>
        Work GetWorkById(WebShop[] webShops, int workId);

        /// <summary>
        /// Returns a Work object containg the single Product for the given productId, and the related Work data in Work attributes.
        /// </summary>
        /// <param name="websiteId"></param>
        /// <param name="productType"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        GetProductDetailsResponse GetProductAsWork(int websiteId, ProductType productType, string id);

        /// <summary>
        /// Returns list of work objects against scope and ISBN
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="isbn"></param>
        /// <returns></returns>
        GetScopeWorksByProductIdResponse GetScopeWorksByProductId(DataScope dataScope, string isbn);

        /// <summary>
        /// Calls Core Data Service to get Series according to website
        /// </summary>
        /// <param name="webSite"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        GetSeriesResponse GetSeries(WebShop webSite, GetSeriesRequest request);

        /// <summary>
        /// Returns the Serie object against the given id and WebShop.
        /// </summary>
        /// <param name="webSite"></param>
        /// <param name="serieId"></param>
        /// <returns></returns>
        Series GetSerieById(WebShop webSite, int serieId);

        /// <summary>
        /// Determines if the Web Shop's data will be available in Koncerndata for the next x minutes.
        /// </summary>
        /// <param name="webShop"></param>
        /// <param name="xMinutes">The number of minutes by which the Web Shop data will be available or not.</param>
        /// <returns></returns>
        bool IsShopDataAvailableForXMinutes(WebShop webShop, short xMinutes);

        /// <summary>
        /// Gets details of a contributor against given id
        /// </summary>
        /// <param name="webshop">Webshop</param>
        /// <param name="id">Id</param>
        /// <returns>Contributor Details</returns>
        ContributorDetails GetContributor(WebShop webshop, string id);

        /// <summary>
        /// Gets details of a contributor against given id
        /// </summary>
        /// <param name="webshop">Webshop</param>
        /// <param name="id">Id</param>
        /// <returns>Contributor Details</returns>
        ContributorDetailsV2 GetContributorV2(WebShop webshop, string id);

        /// <summary>
        /// Gets details of a contributor against given id
        /// </summary>
        /// <param name="webshop">Webshop</param>
        /// <param name="searchRequest">Contributor search request</param>
        /// <returns>Contributor Details</returns>
        SearchContributorResponse<ContributorDetails> GetContributorsByType(WebShop webshop, SearchContributorRequest searchRequest);

        /// <summary>
        /// Gets details of a contributor against given id
        /// </summary>
        /// <param name="webshop">Webshop</param>
        /// <param name="searchRequest">Contributor search request</param>
        /// <returns>Contributor Details</returns>
        SearchContributorResponse<ContributorDetailsV2> GetContributorsByTypeV2(WebShop webshop, SearchContributorRequest searchRequest);

        /// <summary>
        /// Returns the number of contributors updated after the given Ticks value,for the given WebShop.
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="updatedAfter"></param>
        /// <returns></returns>
        int GetUpdatedContributorsCount(DataScope dataScope, DateTime updatedAfter);

        /// <summary>
        /// Returns the asked page of ContributorupdatedInfo objects, for each contributor,
        /// that was updated after the given DateTime, in KoncernDataWebShops database.
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="updatedAfterTicks"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IEnumerable<ContributorUpdateInfo> GetContributorsUpdateInfo(DataScope dataScope,
            DateTime updatedAfterTicks, int pageIndex, int pageSize);

        /// <summary>
        /// Gets details of a contributor against given id
        /// </summary>
        /// <param name="dataScope">dataScope</param>
        /// <param name="id">Id</param>
        /// <returns>Contributor Details</returns>
        ContributorDetails GetContributorDetail(DataScope dataScope, string id);

        /// <summary>
        /// Gets details of a contributor against given id
        /// </summary>
        /// <param name="dataScope">dataScope</param>
        /// <param name="id">Id</param>
        /// <returns>Contributor Details</returns>
        ContributorDetailsV2 GetContributorDetailV2(DataScope dataScope, string id);

        /// <summary>
        /// Determines if the contributor data will be available in Koncerndata for the next x minutes.
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="xMinutes">The number of minutes by which the contributor data will be available or not.</param>
        /// <returns></returns>
        bool IsContributorDataAvailableForXMinutes(DataScope dataScope, short xMinutes);

        /// <summary>
        /// Gets List contributor against given contributor ids
        /// </summary>
        /// <param name="dataScope">dataScope</param>
        /// <param name="ids">Id</param>
        /// <returns>Contributor Details</returns>
        List<ContributorDetails> GetContributorsByIdsFromSolr(DataScope dataScope, IList<string> ids);

        /// <summary>
        /// Gets List contributor against given contributor ids
        /// </summary>
        /// <param name="dataScope">dataScope</param>
        /// <param name="ids">Id</param>
        /// <returns>Contributor Details</returns>
        List<ContributorDetailsV2> GetContributorsByIdsFromSolrV2(DataScope dataScope, IList<string> ids);

        /// <summary>
        /// Gets List contributor against given contributor ids
        /// </summary>
        /// <param name="webShop">dataScope</param>
        /// <param name="ids">Id</param>
        /// <returns>Contributor Details</returns>
        List<ContributorDetails> GetContributorsByIds(WebShop webShop, IList<string> ids);

        /// <summary>
        /// Gets List contributor against given contributor ids
        /// </summary>
        /// <param name="webShop">dataScope</param>
        /// <param name="ids">Id</param>
        /// <returns>Contributor Details</returns>
        List<ContributorDetailsV2> GetContributorsByIdsV2(WebShop webShop, IList<string> ids);

        /// <summary>
        /// Gets List of contributor against given shop id
        /// </summary>
        /// <param name="dataScope">dataScope</param>
        /// <returns>Contributor Details</returns>
        List<ContributorDetails> GetContributorsByShopFromSolr(DataScope dataScope);

        /// <summary>
        /// Gets List of contributor against given shop id
        /// </summary>
        /// <param name="dataScope">dataScope</param>
        /// <returns>Contributor Details</returns>
        List<ContributorDetailsV2> GetContributorsByShopFromSolrV2(DataScope dataScope);

        /// <summary>
        /// Searches Contributors against the provided query string.
        /// </summary>
        /// <param name="request">request</param>
        /// <returns>Contributor Details</returns>
        SearchContributorResponse<ContributorDetails> Search(SearchContributorRequest request);

        /// <summary>
        /// Searches Contributors against the provided query string.
        /// </summary>
        /// <param name="request">request</param>
        /// <returns>Contributor Details</returns>
        SearchContributorResponse<ContributorDetailsV2> SearchV2(SearchContributorRequest request);

        /// <summary>
        /// Searches Contributors against the provided request having multiple webshops.
        /// </summary>
        /// <param name="request">request</param>
        /// <returns>Contributor Details</returns>
        SearchContributorResponse<ContributorDetails> Search(SearchContributorByWebShopsRequest request);

        /// <summary>
        /// Searches Contributors against the provided request having multiple webshops.
        /// </summary>
        /// <param name="request">request</param>
        /// <returns>Contributor Details</returns>
        SearchContributorResponse<ContributorDetailsV2> SearchV2(SearchContributorByWebShopsRequest request);

        /// <summary>
        /// Searches Contributors against the provided query string.
        /// </summary>
        /// <param name="request">request</param>
        /// <returns>Contributor Details</returns>
        SearchContributorResponse<ContributorDetails> SearchBySearchName(SearchContributorRequest request);

        /// <summary>
        /// Searches Contributors against the provided query string.
        /// </summary>
        /// <param name="request">request</param>
        /// <returns>Contributor Details</returns>
        SearchContributorResponse<ContributorDetailsV2> SearchBySearchNameV2(SearchContributorRequest request);

        /// <summary>
        /// Returns the latest update info for synchronization purposes, for the given contributor id and data scope. (Source: KD)
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="contributorId"></param>
        /// <returns></returns>
        ContributorUpdateInfo GetContributorUpdateInfo(DataScope dataScope, string contributorId);

        /// <summary>
        /// Returns a list of Products objects based on provided GQL.
        /// </summary>
        /// <param name="request">Product Search Request information, containing web shop, product type, search string and pagging information</param>
        /// <returns>Product Search Response containing current page information and result records of the page</returns>
        SearchResponse<Work> SearchProducts(ProductSearchRequest request);

        /// <summary>
        /// Returns a list of Products objects based on provided GQL.
        /// </summary>
        /// <param name="request">Product Search Request information, containing web shop, product type, search string and paging information</param>
        /// <returns>Product Search Response containing current page information and result records of the page</returns>
        SearchResponse<Work> SearchProductsV2(ProductSearchRequestV2 request);

        /// <summary>
        /// Returns a list of Products objects based on the fixed search request criteria.
        /// </summary>
        /// <param name="request">Product Search Request information, containing web shop, product type, search string and pagging information</param>
        /// <returns>Product Search Response containing current page information and result records of the page</returns>
        ProductSearchResponse SearchProductsByFixedCriteria(ProductSearchRequest request);

        /// <summary>
        /// Gets all the products for which some campaign exists
        /// </summary>
        /// <param name="webshop"></param>
        /// <param name="updatedAfter"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns>List of string containing product ids</returns>
        IEnumerable<string> GetCampaignProducts(WebShop webshop, DateTime updatedAfter, int pageIndex, int pageSize);

        /// <summary>
        /// Gets count of the products for which campaigns exist
        /// </summary>
        /// <param name="webshop">Webshop</param>
        /// <param name="updatedAfter">Start date of campaign</param>
        /// <returns>Count of campaign affected products</returns>
        int GetCampaignProductsCount(WebShop webshop, DateTime updatedAfter);

        /// <summary>
        /// Checks whether a product has any active campaign
        /// </summary>
        /// <param name="webShop"></param>
        /// <param name="productId">Product ID / ISBN</param>
        /// <returns>Returns whether a product has one or more active campaigns</returns>
        bool HasActiveCampaign(WebShop webShop, string productId);

        /// <summary>
        /// Get list of WebShops against isbn
        /// </summary>
        /// <param name="isbns"></param>
        /// <returns></returns>
        Dictionary<string, List<WebShop>> GetProductWebshops(List<string> isbns);

        int GetBundleCampaignsCount(WebShop webshop, DateTime updatedAfter);

        IEnumerable<string> GetUpdatedBundleCampaignsInfo(WebShop webshop, DateTime updatedAfter, int pageIndex,
            int pageSize);

        List<WebShop> GetDataScopeWebShops(DataScope dataScope);

        DataScope GetDataScopeByWebShop(WebShop webShop);

        string GetSupplementaryData(string isbn);

        ProductAccessControlType GetProductAccessType(string id);

        /// <summary>
        /// Processes events from multiple sources.
        /// </summary>
        /// <param name="eventInfo"></param>
        void ProcessEvent(EventInfo eventInfo);

        /// <summary>
        /// Returns a list of countries.
        /// </summary>
        /// <returns></returns>
        List<Country> GetCountriesList();

        /// <summary>
        /// Returns country by code
        /// </summary>
        /// <returns></returns>
        Country GetCountryByCode(string code);

        /// <summary>
        /// Returns country by Name
        /// </summary>
        /// <returns></returns>
        Country GetCountrybyName(string name);

        #region ErrorInfoController

        /// <summary>
        /// Returns all Error Codes of CoreData and it's underlying systems
        /// </summary>
        /// <returns></returns>
        IEnumerable<Common.WebUtils.Models.ErrorDetail> GetAllErrorCodes();

        /// <summary>
        /// Get error details for the specified error code of CoreData
        /// </summary>
        /// <returns>Error code details</returns>
        Common.WebUtils.Models.ErrorDetail GetErrorCodeDetails(ulong errorCode);

        #endregion ErrorInfoController

        #region WorkReviews

        /// <summary>
        /// Returns the number of WorkReviews updated after the given DateTime value, for the given WebShop.
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="updateAfterDateTime"></param>
        /// <returns></returns>

        int GetUpdatedWorkReviewsCount(DataScope dataScope, DateTime updateAfterDateTime);

        /// <summary>
        /// Get update info of the work review
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="updatedAfterDateTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns>List of WorkReviewUpdateInfo</returns>
        IEnumerable<WorkReviewUpdateInfo> GetWorkReviewsUpdateInfo(DataScope dataScope, DateTime updatedAfterDateTime,
            int pageIndex, int pageSize);

        /// <summary>
        /// Gets details of a WorkReview
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="id">Identifier of a WorkReview</param>
        /// <returns>Details of a WorkReview</returns>
        WorkReview GetWorkReview(DataScope dataScope, int id);

        /// <summary>
        /// Determines if the WorkReviews data will be available in Koncerndata for the next x minutes.
        /// </summary>
        /// <param name="webShop"></param>
        /// <param name="xMinutes">The number of minutes by which the WorkReviews data will be available or not.</param>
        /// <returns>Boolean indicating if data is available or not</returns>
        bool IsWorkReviewsDataAvailableForXMinutes(WebShop webShop, short xMinutes);

        /// <summary>
        /// Gets List of WorkReviews against given shop id
        /// </summary>
        /// <param name="dataScope">dataScope</param>
        /// <returns>WorkReviews Details</returns>
        List<WorkReview> GetWorkReviewsByShopFromSolr(DataScope dataScope);

        /// <summary>
        /// Gets work reviews for given work id
        /// </summary>
        /// <param name="workId"></param>
        /// <param name="webShop"></param>
        /// <returns>WorkReview</returns>
        List<WorkReview> GetWorkReviewsByWorkIdFromSolr(int workId, WebShop webShop = WebShop.None);

        /// <summary>
        /// Get reviews from pulsen services against Gyldendal plus products
        /// </summary>
        /// <param name="isbn"></param>
        /// <param name="dataScope"></param>
        /// <returns></returns>
        List<WorkReview> GetWorkReviews(string isbn, DataScope dataScope);

        #endregion WorkReviews

    }
}