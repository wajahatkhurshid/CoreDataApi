using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Business.ExtensionMethods;
using Gyldendal.Api.CoreData.Business.Util;
using Gyldendal.Api.CoreData.Common;
using Gyldendal.Api.CoreData.Common.DataProviderInfrastructure;
using Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Factories;
using Gyldendal.Api.CoreData.Common.Utils;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Requests;
using Gyldendal.Api.CoreData.Contracts.Response;
using Gyldendal.Api.CoreData.Filters;
using Gyldendal.Api.CoreData.GqlValidator;
using Gyldendal.Api.CoreData.ResultsPostProcessing.Infrastructure;
using Gyldendal.Common.WebUtils.Exceptions;
using NewRelic.Api.Agent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Gyldendal.Api.CoreData.Business.Porter.Interfaces;
using Gyldendal.Api.CoreData.Common.ConfigurationManager;

// ReSharper disable InconsistentNaming

namespace Gyldendal.Api.CoreData.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Returns Product Related Data
    /// </summary>
    [IsGdprSafe(true)]
    public class ProductController : ApiController
    {
        private readonly IProductFactory _productFactory;

        private readonly IKoncernDataUtils _koncernDataUtils;

        private readonly IProductDataProvider _productDataProvider;

        private readonly IWorksResultProcessesExecutor _worksResultProcessesExecutor;

        private readonly IProductService _productService;

        /// <summary>
        /// Used to switch CoreData API between running against Porter(GPM) if true and standard KD if false
        /// </summary>
        private readonly bool _isShadowMode;
        private readonly bool _isUseRequestWebshop;

        /// <param name="productFactory"></param>
        /// <param name="koncernDataUtils"></param>
        /// <param name="productDataProvider"></param>
        /// <param name="worksResultProcessesExecutor"></param>
        /// <param name="productService"></param>
        /// <param name="configurationManager"></param>
        public ProductController(IProductFactory productFactory, IKoncernDataUtils koncernDataUtils,
            IProductDataProvider productDataProvider, IWorksResultProcessesExecutor worksResultProcessesExecutor,
            IProductService productService, IConfigurationManager configurationManager)
        {
            _productFactory = productFactory;
            _koncernDataUtils = koncernDataUtils;
            _productDataProvider = productDataProvider;
            _worksResultProcessesExecutor = worksResultProcessesExecutor;
            _productService = productService;
            _isShadowMode = configurationManager.IsShadowMode;
            _isUseRequestWebshop = configurationManager.UseRequestWebshop;
        }

        /// <summary>
        /// Get Product Details by it's id/isbn (Source: KD)
        /// </summary>
        /// <param name="webShop"></param>
        /// <param name="id"></param>
        /// <param name="productType"></param>
        /// <returns>Product Details</returns>
        [HttpGet]
        [Route("api/v1/Product/GetProductDetails/{webShop}/{id}/{productType}")]
        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> GetProductDetails(WebShop webShop, string id, ProductType productType)
        {
            if (!_isShadowMode)
            {
                var result = _productFactory.GetProductByIsbn(webShop.ToDataScope(), id, productType);
                return Ok(result);
            }
            else
            {
                var result = await _productService.GetProductByIsbnAsync(webShop, id, productType);
                return Ok(result);
            }
        }

        /// <summary>
        /// Gets Product details by data scope for given data profile
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v1/Product/GetProductsByDataScope/")]
        public IHttpActionResult GetProductsByDataScope(GetProductsByDataScopeRequest request)
        {
            return Ok(_productDataProvider.GetProductsByDataScope(request));
        }

        /// <summary>
        /// Gets Product count by data scope based on data profile
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="productDataProfile"></param>
        [HttpGet]
        [ResponseType(typeof(int))]
        [Route("api/v1/Product/GetProductCountByDataScope/{dataScope}/{productDataProfile}")]
        public IHttpActionResult GetProductCountByDataScope(DataScope dataScope, ProductDataProfile productDataProfile)
        {
            return Ok(_productDataProvider.GetProductCountByDataScope(dataScope, productDataProfile));
        }

        /// <summary>
        /// Get Product Details by it's id/isbn (Source: KD)
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Product Details</returns>
        [HttpGet]
        [ResponseType(typeof(ProductAccessControlType))]
        [Route("api/v1/Product/GetProductAccessType/{id}")]
        public async Task<IHttpActionResult> GetProductAccessType(string id)
        {
            if (!_isShadowMode)
            {
                var result = _koncernDataUtils.GetProductAccessTypeByIsbn(id);
                return Ok(result);
            }
            else
            {
                var result = await _productService.GetProductAccessTypeByIsbnAsync(id);
                return Ok(result);
            }
        }

        /// <summary>
        /// Get List of Product if sales configuration exists (Source: Solr)
        /// </summary>
        /// <param name="webShop"></param>
        /// <param name="isbns"></param>
        /// <param name="skipInvalidSaleConfigProds"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v1/Product/GetProductsByIsbns/{skipInvalidSaleConfigProds?}")]
        [ResponseType(typeof(IEnumerable<Product>))]
        public IHttpActionResult GetProductsBysIsbns(WebShop webShop, List<string> isbns,
            bool skipInvalidSaleConfigProds = true)
        {
            return Ok(_productDataProvider.GetProductsByIsbns(webShop, isbns, skipInvalidSaleConfigProds));
        }

        /// <summary>
        /// Get request isbns from the provided webshops (Source: Solr)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v2/Product/GetProductsByIsbns/")]
        [ResponseType(typeof(IEnumerable<Product>))]
        public IHttpActionResult GetProductsBysIsbns(GetProductRequestV2 request)
        {
            if (!(request?.Isbns.Any() ?? false))
            {
                throw new ValidationException((ulong)ErrorCodes.MissingIsbns, ErrorCodes.MissingIsbns.GetDescription(),
                    Extensions.CoreDataSystemName, null);
            }

            return Ok(_productDataProvider.GetProductsByIsbns(request.WebShops, request.Isbns.ToList(),
                request.SkipInvalidSaleConfigProds));
        }

        /// <summary>
        /// Get List of Product if sales configuration exists (Source: Solr)
        /// </summary>
        /// <param name="isbns"></param>
        /// <param name="skipInvalidSaleConfigProds"></param>
        /// <returns>List of products for given isbns</returns>
        [HttpPost]
        [ResponseType(typeof(IEnumerable<Product>))]
        [Route("api/v1/Product/GetAllProductsByIsbns/{skipInvalidSaleConfigProds?}")]
        public IHttpActionResult GetProductsBysIsbns(List<string> isbns, bool skipInvalidSaleConfigProds = true)
        {
            return Ok(_productDataProvider.GetProductsByIsbns(isbns, skipInvalidSaleConfigProds));
        }

        /// <summary>
        /// Get List of Product based on GQL (Source: Solr)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="skipInvalidSaleConfigProds"></param>
        /// <returns>List of products for given isbns</returns>
        [HttpPost]
        [ResponseType(typeof(Result<Product>))]
        [Route("api/v1/Product/GetProductsByGql/{skipInvalidSaleConfigProds?}")]
        public IHttpActionResult GetProductsByGql(GetProductsByGqlRequest request,
            bool skipInvalidSaleConfigProds = true)
        {
            if (!(string.IsNullOrWhiteSpace(request.Gql)) &&
                !(new ExpressionValidator().Validate(request.Gql)).Result.IsValidated)
            {
                throw new ValidationException((ulong)ErrorCodes.InvalidGql, ErrorCodes.InvalidGql.GetDescription(),
                    Extensions.CoreDataSystemName, null);
            }

            var searchRequest = request.ToWorkProductSearchRequest();
            var response = _productDataProvider.GetProducts(searchRequest);

            return Ok(response?.SearchResults);
        }

        /// <summary>
        /// Performs Product search in Solr
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="ValidationException"></exception>
        [NullValueFilter]
        [HttpPost]
        [Route("api/v1/Product/Search")]
        [Trace]
        public IHttpActionResult SearchProducts(ProductSearchRequest request)
        {
            if (!(string.IsNullOrWhiteSpace(request.Gql)) &&
                !(new ExpressionValidator().Validate(request.Gql)).Result.IsValidated)
            {
                throw new ValidationException((ulong)ErrorCodes.InvalidGql, ErrorCodes.InvalidGql.GetDescription(),
                    Extensions.CoreDataSystemName, null);
            }

            var searchRequest = request.ToWorkProductSearchRequest();
            var response = _productDataProvider.GetProductsAsWork(searchRequest);

            _worksResultProcessesExecutor.Execute(response, request.Webshop);

            return Ok(response);
        }

        /// <summary>
        /// Performs Product search (Source: Solr)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="ValidationException"></exception>
        [NullValueFilter]
        [HttpPost]
        [Route("api/v2/Product/Search")]
        [Trace]
        public IHttpActionResult SearchProducts(ProductSearchRequestV2 request)
        {
            if (!(string.IsNullOrWhiteSpace(request.Gql)) &&
                !(new ExpressionValidator().Validate(request.Gql)).Result.IsValidated)
            {
                throw new ValidationException((ulong)ErrorCodes.InvalidGql, ErrorCodes.InvalidGql.GetDescription(),
                    Extensions.CoreDataSystemName, null);
            }

            var searchRequest = request.ToWorkProductSearchRequest();
            var response = _productDataProvider.GetProductsAsWork(searchRequest);

            _worksResultProcessesExecutor.Execute(response, request.Webshop);

            return Ok(response);
        }

        /// <summary>
        /// Search products without any default groupBy i.e. Work       (Source: Solr)
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Facets, Paging, Products(without merging in work)</returns>
        /// <exception cref="ValidationException"></exception>
        [NullValueFilter]
        [HttpPost]
        [Route("api/v1/Product/AbsoluteSearch")]
        [ResponseType(typeof(SearchResponse<Product>))]
        [Trace]
        public IHttpActionResult AbsoluteSearch(ProductSearchRequest request)
        {
            if (!(string.IsNullOrWhiteSpace(request.Gql)) &&
                !(new ExpressionValidator().Validate(request.Gql)).Result.IsValidated)
            {
                throw new ValidationException((ulong)ErrorCodes.InvalidGql, ErrorCodes.InvalidGql.GetDescription(),
                    Extensions.CoreDataSystemName, null);
            }

            var searchRequest = request.ToWorkProductSearchRequest();
            if (!_isUseRequestWebshop)
            {
                searchRequest.WebShops = (WebShop[])Enum.GetValues(typeof(WebShop));
            }

            var response = _productDataProvider.GetProducts(searchRequest);

            return Ok(response);
        }

        /// <summary>
        /// Search products without any default groupBy i.e. Work (Source: Solr)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [NullValueFilter]
        [HttpPost]
        [Route("api/v2/Product/AbsoluteSearch")]
        [ResponseType(typeof(SearchResponse<Product>))]
        [Trace]
        public IHttpActionResult AbsoluteSearchV2(ProductSearchRequestEx request)
        {
            if (!(string.IsNullOrWhiteSpace(request.Gql)) &&
                !(new ExpressionValidator().Validate(request.Gql)).Result.IsValidated)
            {
                throw new ValidationException((ulong)ErrorCodes.InvalidGql, ErrorCodes.InvalidGql.GetDescription(),
                    Extensions.CoreDataSystemName, null);
            }

            var searchRequest = request.ToWorkProductSearchRequest();
            searchRequest.WebShops = request.DataScope.ToWebShops()?.ToArray();

            var response = _productDataProvider.GetProducts(searchRequest);

            return Ok(response);
        }

        /// <summary>
        /// Get List of Products (Source: KD)
        /// </summary>
        /// <param name="webshop"></param>
        /// <param name="isbns">ISBN list</param>
        /// <param name="getImageUrl"></param>
        /// <returns></returns>
        [NullValueFilter]
        [HttpPost]
        [Route("api/v1/Product/GetLicensedProductsByIsbn/")]
        [ResponseType(typeof(SearchResponse<ProductBasicData>))]
        public async Task<IHttpActionResult> GetLicensedProductsByIsbn(WebShop webshop, IList<string> isbns,
            bool getImageUrl = true)
        {
            if (!_isShadowMode)
                return Ok(_koncernDataUtils.GetLicensedProductsByIsbn(webshop, isbns, getImageUrl));
            else
                return Ok(await _productService.GetLicensedProductsByIsbnAsync(webshop, isbns, getImageUrl));
        }

        /// <summary>
        /// Returns a list of Products objects based on the search request criteria (Source: Solr)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [NullValueFilter]
        [ResponseType(typeof(ProductSearchResponse))]
        [HttpPost]
        [Route("api/v1/Product/SearchByFixedCriteria")]
        public IHttpActionResult SearchProductsByFixedCriteria(ProductFixSearchRequest request)
        {
            return Ok(_productDataProvider.Search(request));
        }

        /// <summary>
        /// Returns the number of products updated after the given Ticks value,for the given WebShop.
        /// updatedAfterTicks Time has to be send in query string see this link
        /// http://stackoverflow.com/questions/26659406/trouble-passing-datetime-parameter-to-web-service-in-get
        /// (Source: KD)
        /// </summary>
        /// <param name="webShop">Web Shop</param>
        /// <param name="updatedAfterTicks"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/Product/GetUpdatedProductsCount/{webShop}/{updatedAfterTicks}")]
        public async Task<IHttpActionResult> GetUpdatedProductsCount(WebShop webShop, long updatedAfterTicks)
        {
            var updateAfterDateTime = new DateTime(updatedAfterTicks);
            if (!_isShadowMode)
            {
                var result = _productFactory.GetUpdatedProductsCount(webShop.ToDataScope(), updateAfterDateTime);
                return Ok(result);
            }
            else
            {
                var result = await _productService.GetUpdatedProductsCountAsync(webShop, updatedAfterTicks);
                return Ok(result);
            }
        }

        /// <summary>
        /// Returns the number of products updated after the given Ticks value,for the given scope.
        /// updatedAfterTicks Time has to be send in query string see this link
        /// http://stackoverflow.com/questions/26659406/trouble-passing-datetime-parameter-to-web-service-in-get
        /// (Source: KD)
        /// </summary>
        /// <param name="dataScope">Web Shop</param>
        /// <param name="updatedAfterTicks"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/Product/GetUpdatedProductsCountByDataScope/{dataScope}/{updatedAfterTicks}")]
        public async Task<IHttpActionResult> GetUpdatedProductsCount(DataScope dataScope, long updatedAfterTicks)
        {
            var updateAfterDateTime = new DateTime(updatedAfterTicks);
            if (!_isShadowMode)
            {
                var result = _productFactory.GetUpdatedProductsCount(dataScope, updateAfterDateTime);
                return Ok(result);
            }
            else
            {
                var result =
                    await _productService.GetUpdatedProductsCountAsync(dataScope.ToFirstWebShop(), updatedAfterTicks);
                return Ok(result);
            }
        }

        /// <summary>
        /// Returns the asked page of ProductupdatedInfo objects, for each product, related to the given WebShop,
        /// that was updated after the given DateTime, in KD.
        /// </summary>
        /// <param name="webShop"></param>
        /// <param name="updatedAfterTicks"></param>
        /// <param name="pageIndex">Minimum value 1.</param>
        /// <param name="pageSize">Minimum value 1.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/Product/GetProductsUpdateInfo/{webShop}/{updatedAfterTicks}/{pageIndex}/{pageSize}")]
        public async Task<IHttpActionResult> GetProductsUpdateInfo(WebShop webShop, long updatedAfterTicks,
            int pageIndex, int pageSize)
        {
            var updatedAfterDateTime = new DateTime(updatedAfterTicks);
            if (!_isShadowMode)
                return Ok(_productFactory.GetProductsUpdateInfo(webShop.ToDataScope(), updatedAfterDateTime, pageIndex,
                    pageSize));
            else
                return Ok(await _productService.GetUpdatedProductsInfoAsync(webShop, updatedAfterTicks, pageIndex,
                    pageSize));
        }


        /// <summary>
        /// Returns the asked page of ProductupdatedInfo objects, for each product, related to the given DataScope,
        /// that was updated after the given DateTime, in KD.
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="updatedAfterTicks"></param>
        /// <param name="pageIndex">Minimum value 1.</param>
        /// <param name="pageSize">Minimum value 1.</param>
        /// <returns></returns>
        [HttpGet]
        [Route(
            "api/v1/Product/GetProductsUpdateInfoByDataScope/{dataScope}/{updatedAfterTicks}/{pageIndex}/{pageSize}")]
        public async Task<IHttpActionResult> GetProductsUpdateInfo(DataScope dataScope, long updatedAfterTicks,
            int pageIndex, int pageSize)
        {
            var updatedAfterDateTime = new DateTime(updatedAfterTicks);
            if (!_isShadowMode)
                return Ok(_productFactory.GetProductsUpdateInfo(dataScope, updatedAfterDateTime, pageIndex, pageSize));
            else
                return Ok(await _productService.GetUpdatedProductsInfoAsync(dataScope.ToFirstWebShop(), updatedAfterTicks,
                    pageIndex, pageSize));
        }

        /// <summary>
        /// Gets all the products for which some campaign exists (Source: KD)
        /// </summary>
        /// <param name="webShop"></param>
        /// <param name="updatedAfterTicks"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns>List of string containing product ids</returns>
        [HttpGet]
        [Route("api/v1/Product/GetCampaignProds/{webShop}/{updatedAfterTicks}/{pageIndex}/{pageSize}/")]
        public IHttpActionResult GetCampaignAffectedProducts(WebShop webShop, long updatedAfterTicks, int pageIndex,
            int pageSize)
        {
            var updatedAfterDateTime = new DateTime(updatedAfterTicks);

            if (!_isShadowMode)
                return Ok(_productFactory.GetCampaignProducts(webShop.ToDataScope(), updatedAfterDateTime, pageIndex, pageSize));
            else
                return Ok(_productService.GetCampaignProducts(webShop, updatedAfterDateTime, pageIndex, pageSize));
        }

        /// <summary>
        /// Gets count of the products for which campaigns exist (Source: KD)
        /// </summary>
        /// <param name="webShop">Webshop</param>
        /// <param name="updatedAfterTicks">Start date of campaign</param>
        /// <returns>Count of campaign affected products</returns>
        [HttpGet]
        [Route("api/v1/Product/GetCampaignProdsCount/{webShop}/{updatedAfterTicks}")]
        public IHttpActionResult GetCampaignAffectedProductsCount(WebShop webShop, long updatedAfterTicks)
        {
            var updatedAfterDateTime = new DateTime(updatedAfterTicks);

            if (!_isShadowMode)
                return Ok(_productFactory.GetCampaignProductsCount(webShop.ToDataScope(), updatedAfterDateTime));
            else
                return Ok(_productService.GetCampaignProductsCount(webShop, updatedAfterDateTime));
        }

        /// <summary>
        /// Gets modified Bundle campaigns (Source: KD)
        /// </summary>
        /// <param name="webShop"></param>
        /// <param name="updatedAfterTicks"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns>List of string containing product ids</returns>
        [HttpGet]
        [Route("api/v1/Product/GetUpdatedBundleCampaignsInfo/{webShop}/{updatedAfterTicks}/{pageIndex}/{pageSize}/")]
        public IHttpActionResult GetUpdatedBundleCampaignsInfo(WebShop webShop, long updatedAfterTicks, int pageIndex,
            int pageSize)
        {
            var updatedAfterDateTime = new DateTime(updatedAfterTicks);

            if (!_isShadowMode)
                throw new NotImplementedException("Endpoint not in use");
            else
                return Ok(_productService.GetUpdatedBundleCampaignsInfo(webShop, updatedAfterDateTime, pageIndex, pageSize));
        }

        /// <summary>
        /// Gets count of the modified bundle campaigns (Source: KD)
        /// </summary>
        /// <param name="webShop">Webshop</param>
        /// <param name="updatedAfterTicks">Start date of campaign</param>
        /// <returns>Count of campaign affected products</returns>
        [HttpGet]
        [Route("api/v1/Product/GetBundleCampaignsCount/{webShop}/{updatedAfterTicks}")]
        public IHttpActionResult GetBundleCampaignsCount(WebShop webShop, long updatedAfterTicks)
        {
            var updatedAfterDateTime = new DateTime(updatedAfterTicks);

            if (!_isShadowMode)
                throw new NotImplementedException("Endpoint not in use");
            else
                return Ok(_productService.GetBundleCampaignsCount(webShop, updatedAfterDateTime));
        }

        /// <summary>
        /// Checks whether a product has any active campaign (Source: KD)
        /// </summary>
        /// <param name="webShop"></param>
        /// <param name="productId">Product ID / ISBN</param>
        /// <returns>Returns whether a product has one or more active campaigns</returns>
        [HttpGet]
        [Route("api/v1/Product/HasActiveCampaign/{webShop}/{productId}")]
        public IHttpActionResult HasActiveCampaign(WebShop webShop, string productId)
        {
            if (!_isShadowMode)
            {
                return Ok(_productFactory.HasActiveCampaign(webShop.ToDataScope(), productId));
            }
            else
            {
                throw new NotImplementedException("Endpoint is unused");
            }
        }

        /// <summary>
        /// Determines if the Web Shop's data will be available in Koncerndata for the next x minutes (Source: KD)
        /// </summary>
        /// <param name="webShop"></param>
        /// <param name="xMinutes">The number of minutes by which the Web Shop data will be available or not.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/Product/IsShopDataAvailableForXMinutes/{webShop}/{xMinutes}")]
        public async Task<IHttpActionResult> IsShopDataAvailableForXMinutes(WebShop webShop, short xMinutes)
        {
            if (!_isShadowMode)
                return Ok(_koncernDataUtils.IsShopDataAvailableForXMinutes(webShop, xMinutes));
            else
                return Ok(await _productService.IsProductDataAvailable());
        }

        /// <summary>
        /// Return the List of bundle Ids which have specific Isbn (Source: KD)
        /// </summary>
        /// <param name="webShop"></param>
        /// <param name="isbn"></param>
        /// <param name="pageIndex">Minimum value 1.</param>
        /// <param name="pageSize">Minimum value 1.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/Product/GetBundleIdsByIsbn/{webShop}/{isbn}/{pageIndex}/{pageSize}")]
        public IHttpActionResult GetBundleIdsByIsbn(WebShop webShop, string isbn, int pageIndex, int pageSize)
        {
            if (!_isShadowMode)
            {
                return Ok(_productFactory.GetBundleIdsByIsbn(webShop.ToDataScope(), isbn, pageIndex, pageSize));
            }
            else
            {
                throw new NotImplementedException("Endpoint is unused");
            }
        }

        /// <summary>
        /// Location (URL) of first secured material of the product (Source: KD)
        /// </summary>
        /// <param name="isbn"></param>
        /// <returns>
        /// Location (URL) of first secured material of given isbn or null if not found
        /// </returns>
        [HttpGet]
        [Route("api/v1/Product/GetSupplementaryData/{isbn}")]
        public IHttpActionResult GetSupplementaryData(string isbn)
        {
            if (!_isShadowMode)
                return Ok(_koncernDataUtils.GetSupplementaryData(isbn));
            else
                return Ok(_productService.GetSupplementaryDataAsync(isbn));
        }

        /// <summary>
        /// Get list of WebShops against isbn
        /// </summary>
        /// <param name="isbns"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v1/Product/GetProductWebshops/")]
        public async Task<IHttpActionResult> GetProductWebshops(List<string> isbns)
        {

            if (!_isShadowMode)
                return Ok(_productFactory.GetProductWebshops(DataScope.Global, isbns));
            else
                return Ok(await _productService.GetProductWebshopsAsync(isbns));
        }
    }
}