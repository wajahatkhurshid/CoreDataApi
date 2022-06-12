using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gyldendal.Api.CoreData.Business.Porter.Interfaces;
using Gyldendal.Api.CoreData.Business.Porter.Mapping;
using Gyldendal.Api.CoreData.Business.Util;
using Gyldendal.Api.CoreData.Common.ConfigurationManager;
using Gyldendal.Api.CoreData.Common.Exceptions;
using Gyldendal.Api.CoreData.Common.Utils;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Response;
using CoreDataModels = Gyldendal.Api.CoreData.Contracts.Models;
using PorterApi = Gyldendal.Api.CoreData.Services.PorterApiClient;
using ProductType = Gyldendal.Api.CoreData.Contracts.Enumerations.ProductType;
using WebShop = Gyldendal.Api.CommonContracts.WebShop;

namespace Gyldendal.Api.CoreData.Business.Porter.Services
{
    public class ProductService : IProductService
    {
        private readonly ShopServices.ApiClient.Client _shopServicesApiClient;
        private readonly ICoverImageUtil _imageUtil;
        private readonly IConfigurationManager _configManager;
        private readonly PorterApi.IPorterClient _porterClient;
        private readonly IServiceDataProviderFactory _serviceDataProviderFactory;

        public ProductService(PorterApi.IPorterClient porterClient, ShopServices.ApiClient.Client shopServicesApiClient, ICoverImageUtil imageUtil, IConfigurationManager configManager, IServiceDataProviderFactory serviceDataProviderFactory)
        {
            _porterClient = porterClient;
            _shopServicesApiClient = shopServicesApiClient;
            _imageUtil = imageUtil;
            _configManager = configManager;
            _serviceDataProviderFactory = serviceDataProviderFactory;
        }

        public async Task<CoreDataModels.Product> GetProductByIsbnAsync(WebShop webShop, string isbn, ProductType productType)
        {
            var response = await _porterClient.ProductApiV1ProductDetailsAsync(
                new PorterApi.GetProductDetailsRequest
                {
                    WebShop = webShop.ToPorterWebShop(),
                    Isbn = isbn,
                    ProductType = productType.ToPorterProductType()
                });

            if (response.Product == null)
            {
                throw new NotFoundException($"Product for Isbn: {isbn} not found.");
            }

            var product = response.Product.ToCoreDataProduct(_shopServicesApiClient, _imageUtil, _configManager, webShop);

            var serviceDataProvider = _serviceDataProviderFactory.GetProductServiceDataProvider(webShop);

            serviceDataProvider.ProvideDataFor_GetProductByIsbn(product);

            return product;
        }

        public async Task<ProductAccessControlType> GetProductAccessTypeByIsbnAsync(string isbn)
        {
            var response = await _porterClient.ProductApiV1ProductAccesstypeAsync(
                new PorterApi.GetProductAccessTypeRequest() { Isbn = isbn });

            if (response == null)
                throw new NotFoundException($"Product for Isbn: {isbn} not found.");

            switch (response.AccessControl)
            {
                case "Ekey":
                    return ProductAccessControlType.Ekey;
                default:
                    return ProductAccessControlType.Unic;
            }

        }

        public async Task<string> GetSupplementaryDataAsync(string isbn)
        {
            var response = await _porterClient.ProductApiV1ProductAttachmentsAsync(
                new PorterApi.GetProductAttachmentsRequest() { Isbn = isbn });

            if (response == null)
                throw new NotFoundException($"Supplementary Data for Isbn: {isbn} not found.");

            var supplementaryData = response.Attachments.FirstOrDefault();

            return supplementaryData?.SampleUrl;
        }

        public async Task<List<CoreDataModels.ProductBasicData>> GetLicensedProductsByIsbnAsync(WebShop webshop, IList<string> isbns, bool withImageUrl)
        {
            var response = await _porterClient.ProductApiV1ProductBasicdetailsAsync(
                new PorterApi.GetProductsBasicDetailRequest()
                {
                    WebShop = webshop.ToPorterWebShop(),
                    Isbns = isbns,
                    WithImageUrl = withImageUrl
                });
            return response.ProductBasicDetails.Select(p => new CoreDataModels.ProductBasicData
            {
                Isbn = p.Isbn,
                Isbn10 = p.Isbn,
                Title = p.Title,
                Subtitle = p.Subtitle,
                Description = p.Description,
                MediaType = p.MediaType,
                ImageUrl = withImageUrl ? _imageUtil.GetOriginalImageUrl(p.Isbn) : null,
                DigitalProductLink = PorterProductModelsMapping.GetDigitalProductUrl(webshop, p.Isbn, p.MediaType, p.WebsiteAddress, _configManager)
            }).ToList();
        }

        public async Task<int> GetUpdatedProductsCountAsync(WebShop webshop, long updateAfterTicks)
        {
            var response = await _porterClient.ProductApiV1ProductGetupdatecountAsync(
                new PorterApi.GetProductsUpdateCountRequest()
                {
                    WebShop = webshop.ToPorterWebShop(),
                    UpdatedAfterTicks = updateAfterTicks
                });

            return response;
        }

        public async Task<List<ProductUpdateInfo>> GetUpdatedProductsInfoAsync(WebShop webshop, long updateAfterTicks, int pageIndex, int pageSize)
        {
            var response = await _porterClient.ProductApiV1ProductGetupdateinfoAsync(
                new PorterApi.GetProductsUpdateInfoRequest()
                {
                    WebShop = webshop.ToPorterWebShop(),
                    UpdatedAfterTicks = updateAfterTicks,
                    PageIndex = pageIndex,
                    PageSize = pageSize
                });

            if (response == null)
                throw new NotFoundException($"No Updated Product found.");

            return response.ProductUpdateInfos.Select(p => new ProductUpdateInfo
            {
                ProductId = p.Id,
                ProductType = p.ProductType.ToCoreDataProductType(),
                UpdateTime = p.UpdateTime,
                UpdateType = p.IsDeleted ? ProductUpdateType.Deleted : ProductUpdateType.Updated,
            }).ToList();
        }

        public IEnumerable<string> GetCampaignProducts(WebShop webShop, DateTime updatedAfterTicks, int pageIndex, int pageSize)
        {
            var serviceDataProvider = _serviceDataProviderFactory.GetProductServiceDataProvider(webShop);
            var campaignProducts = serviceDataProvider.ProvideDataFor_GetModifiedSingleProductCampaignsIsbn(updatedAfterTicks, webShop.ToString());
            return campaignProducts.OrderBy(x => x)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToArray()
                .Select(x => x);
        }

        public int GetCampaignProductsCount(WebShop webShop, DateTime updatedAfterTicks)
        {
            var serviceDataProvider = _serviceDataProviderFactory.GetProductServiceDataProvider(webShop);
            var campaignProducts = serviceDataProvider.ProvideDataFor_GetModifiedSingleProductCampaignsIsbn(updatedAfterTicks, webShop.ToString());
            return campaignProducts.Count();
        }

        public IEnumerable<string> GetUpdatedBundleCampaignsInfo(WebShop webShop, DateTime updatedAfterTicks, int pageIndex, int pageSize)
        {
            if (pageIndex < 0)
                throw new ArgumentException($"Value for {nameof(pageIndex)} should be greater than or equal to 0.");

            if (pageSize < 1)
                throw new ArgumentException($"Value for {nameof(pageSize)} should be greater than 0.");

            var serviceDataProvider = _serviceDataProviderFactory.GetProductServiceDataProvider(webShop);
            return serviceDataProvider.ProvideDataFor_GetModifiedBundleCampaignsUpdateInfo(updatedAfterTicks, webShop.ToString(), pageIndex, pageSize);
        }

        public int GetBundleCampaignsCount(WebShop webShop, DateTime updatedAfterTicks)
        {
            var serviceDataProvider = _serviceDataProviderFactory.GetProductServiceDataProvider(webShop);
            var campaignProducts = serviceDataProvider.ProvideDataFor_GetModifiedBundleCampaignsPakkeId(updatedAfterTicks, webShop.ToString());
            return campaignProducts.Count();
        }

        public async Task<bool> IsProductDataAvailable()
        {
            return await _porterClient.ProductApiV1ProductIsproductdataavailableAsync();
        }
        public async Task<Dictionary<string, List<WebShop>>> GetProductWebshopsAsync(List<string> isbns)
        {
            if (!isbns?.Any() ?? true)
            { return null; }
            
            var response = await _porterClient.ProductApiV1ProductProductsearchAsync(
                new PorterApi.ProductSearchQuery()
                {
                    Isbns = isbns,
                    AuthorName = "",
                    MediaType = "",
                    PageIndex = 0,
                    PageSize = isbns.Count,
                    PropertiesToInclude = "WebShops,Isbn",
                    Title = "",
                    WebShop = PorterApi.WebShop.All
                });
            var result = response.Results.Select(x => new 
            {
                Isbn = x.Isbn,
                Shops = x.WebShops

            }).Where(y=>y.Shops.Contains(PorterApi.WebShop.GU) || y.Shops.Contains(PorterApi.WebShop.MunksGaard)
                                                               || y.Shops.Contains(PorterApi.WebShop.HansReitzel));
            return result.GroupBy(x => x.Isbn)
                .Select(x => new { ISBN = x.Key, Webshops = x.SelectMany(y => y.Shops) })
                .ToDictionary(z => z.ISBN, zz => zz.Webshops.Select(x => x.ToCoreDataWebShop()).ToList());
        }
    }
}