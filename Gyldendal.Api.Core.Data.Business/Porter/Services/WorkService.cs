using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Gyldendal.Api.CoreData.Business.Porter.Interfaces;
using Gyldendal.Api.CoreData.Business.Porter.Mapping;
using Gyldendal.Api.CoreData.Business.Util;
using Gyldendal.Api.CoreData.Common.ConfigurationManager;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.DataAccess.KoncernData;
using CoreDataResponses = Gyldendal.Api.CoreData.Contracts.Response;
using Gyldendal.Api.CoreData.Services.PorterApiClient;
using Gyldendal.Api.ShopServices.ApiClient;
using Gyldendal.PulsenServices.Api.Contracts.Common;
using Gyldendal.PulsenServices.ApiClient;
using PorterApi = Gyldendal.Api.CoreData.Services.PorterApiClient;
using Product = Gyldendal.Api.CoreData.Contracts.Models.Product;
using ProductType = Gyldendal.PulsenServices.Api.Contracts.Common.ProductType;
using Work = Gyldendal.Api.CoreData.Contracts.Models.Work;

namespace Gyldendal.Api.CoreData.Business.Porter.Services
{
    public class WorkService : IWorkService
    {
        private readonly IPorterClient _porterClient;
        private readonly ShopServices.ApiClient.Client _shopServicesApiClient;
        private readonly ICoverImageUtil _imageUtil;
        private readonly IConfigurationManager _configurationManager;
        private readonly PulsenServiceApiClient _pulsenServiceApiClient;
        private readonly IServiceDataProviderFactory _serviceDataProviderFactory;

        public WorkService(IPorterClient porterClient, Client shopServicesApiClient, ICoverImageUtil imageUtil, IConfigurationManager configurationManager, PulsenServiceApiClient pulsenServiceApiClient, IServiceDataProviderFactory serviceDataProviderFactory)
        {
            _porterClient = porterClient;
            _shopServicesApiClient = shopServicesApiClient;
            _imageUtil = imageUtil;
            _configurationManager = configurationManager;
            _pulsenServiceApiClient = pulsenServiceApiClient;
            _serviceDataProviderFactory = serviceDataProviderFactory;
        }

        public async Task<CoreDataResponses.GetScopeWorksByProductIdResponse> GetScopeWorksByProductIdAsync(DataScope dataScope, string isbn)
        {
            var response = new CoreDataResponses.GetScopeWorksByProductIdResponse { Works = new List<Work>() };

            var pulsenProdDetailTask = Task.Factory.StartNew(() => _pulsenServiceApiClient.Product.GetProductDetails(isbn));
            pulsenProdDetailTask.Wait();

            var pulsenProdDetail = pulsenProdDetailTask.Result;

            if (pulsenProdDetail.Product == null)
            {
                response.Message = $"No supplementary data found in pulsen services: {pulsenProdDetail.ProductNotFoundReason:G}";
                response.Works = null;
                response.ProductNotFoundReason = GetProductNotFoundReason(pulsenProdDetail.ProductNotFoundReason);

                return response;
            }

            if (pulsenProdDetail.Product.ProductType == ProductType.BundleProduct && string.IsNullOrWhiteSpace(pulsenProdDetail.Product.Description))
            {
                pulsenProdDetail.Product.Description = GetBundleProductDescription(isbn);
            }
            var workResponse = await _porterClient.WorkApiV1WorkGetAsync(new GetWorkQuery { Isbn = isbn, WebShop = WebShop.GyldendalPlus });

            var cdWork = workResponse.Work.ToCoreDataWork(_imageUtil, _configurationManager, _shopServicesApiClient, CommonContracts.WebShop.ClubBogklub);

            var works = pulsenProdDetail.Product.ClubIds.Where(club => club != Clubs.GyldendalDk).Select(clubId => Repositories.Common.ModelsMapping.GetClubWorkForTrade(cdWork, pulsenProdDetail.Product, clubId)).ToList();
            response.Works = works;

            return response;
        }

        public async Task<CoreDataResponses.GetProductDetailsResponse> GetWorkByProductIdAsync(CommonContracts.WebShop webShop, Contracts.Enumerations.ProductType productType, string id)
        {
            switch (productType)
            {
                case Contracts.Enumerations.ProductType.SingleProduct:
                    return await GetWorkForSingleProductAsync(webShop, id);
                case Contracts.Enumerations.ProductType.Bundle:
                    return await GetWorkForBundleProductsAsync(webShop, id);
                default:
                    throw new InvalidDataException($"Product Type: {productType} is not correct");
            }
        }

        private async Task<CoreDataResponses.GetProductDetailsResponse> GetWorkForSingleProductAsync(CommonContracts.WebShop webShop, string isbn)
        {
            var response = await _porterClient.WorkApiV1WorkGetbyproductAsync(
                new PorterApi.GetWorkByProductRequest
                {
                    Isbn = isbn,
                    ProductType = PorterApi.ProductType.SingleProduct,
                    WebShop = webShop.ToPorterWebShop()
                });

            if (response.Work == null)
            {
                return new CoreDataResponses.GetProductDetailsResponse
                {
                    Message = "Product not found in Porter.",
                    ProductNotFoundReason = ProductNotFoundReason.None //TODO: need to modify core data contracts to have specific reason
                };
            }
            
            var work = response.Work.ToCoreDataWork(_imageUtil, _configurationManager, _shopServicesApiClient, webShop);

            var serviceDataProvider = _serviceDataProviderFactory.GetWorkServiceDataProvider(webShop);

            return serviceDataProvider.ProvideDataFor_GetWorkByProduct(work, isbn);
        }

        private async Task<CoreDataResponses.GetProductDetailsResponse> GetWorkForBundleProductsAsync(CommonContracts.WebShop webShop, string bundleId)
        {
            var serviceDataProvider = _serviceDataProviderFactory.GetWorkServiceDataProvider(webShop);

            var isbnsList = serviceDataProvider.GetBundleProductIsbnsFromKd(bundleId).ToList();

            if (isbnsList.Count > 0)
            {
                var response = await _porterClient.ProductApiV1ProductProductsearchAsync(
                    new PorterApi.ProductSearchQuery()
                    {
                        Isbns = isbnsList,
                        AuthorName = "",
                        MediaType = "",
                        PageIndex = 0,
                        PageSize = isbnsList.Count(),
                        PropertiesToInclude = "",
                        Title = "",
                        WebShop = webShop.ToPorterWebShop()
                    });

                if (response.Results.Count == 0)
                {
                    return new CoreDataResponses.GetProductDetailsResponse
                    {
                        Message = "Product not found in Porter.",
                        ProductNotFoundReason = ProductNotFoundReason.None //TODO: need to modify core data contracts to have specific reason
                    };
                }

                var work = serviceDataProvider.ProvideDataFor_GetWorkForBundleProduct(bundleId, response.Results.ToList(), webShop);

                if (work.Id > 0)
                    work.Levels = await GetWorkLevelsAsync(work.Areas, webShop);

                return new CoreDataResponses.GetProductDetailsResponse
                {
                    Message = "",
                    ProductNotFoundReason = ProductNotFoundReason.None,
                    ProductWork = work
                };
            }
            return new CoreDataResponses.GetProductDetailsResponse
            {
                Message = "Isbns not found in kd.",
                ProductNotFoundReason = ProductNotFoundReason.None  //TODO: need to modify core data contracts to have specific reason
            };
        }

        private string GetBundleProductDescription(string isbn)
        {
            return "Bundles not implemented yet"; //TODO: Implement this once GPM implements bundles for GPlus
        }

        private ProductNotFoundReason GetProductNotFoundReason(PulsenServices.Api.Contracts.Product.ProductNotFoundReason productNotFoundReason)
        {
            var reason = productNotFoundReason switch
            {
                PulsenServices.Api.Contracts.Product.ProductNotFoundReason.ProductOutOfStock => ProductNotFoundReason.ProductOutOfStock,
                PulsenServices.Api.Contracts.Product.ProductNotFoundReason.DataImportInProgress => ProductNotFoundReason.DataImportInProgress,
                PulsenServices.Api.Contracts.Product.ProductNotFoundReason.ProductIsDeleted => ProductNotFoundReason.ProductIsDeleted,
                _ => ProductNotFoundReason.SupplementaryDataNotFound
            };

            return reason;
        }

        private async Task<List<Contracts.Models.Level>> GetWorkLevelsAsync(List<Contracts.Models.Area> areas, CommonContracts.WebShop webShop)
        {
            List<Contracts.Models.Level> levelsList = new List<Contracts.Models.Level>();
            foreach (var area in areas)
            {
                var areaResponse = await _porterClient.MasterdataApiV1MasterdataAreasAsync(webShop.ToPorterWebShop());
                var porterArea = areaResponse.Areas.Where(a => a.Name == area.Name).FirstOrDefault();

                if (porterArea != null)
                {
                    var levelResponse = await _porterClient.MasterdataApiV1MasterdataLevelsAsync(new GetLevelsRequest()
                    {
                        WebShop = webShop.ToPorterWebShop(),
                        AreaId = porterArea.Id
                    });

                    if (levelResponse != null)
                    {
                        foreach (var level in levelResponse.Levels)
                            levelsList.Add(new Contracts.Models.Level()
                            {
                                AreaId = 0,
                                LevelNumber = level.LevelNumber,
                                Name = level.Name,
                                WebShop = webShop
                            });
                    }
                }
            }
            return levelsList;
        }
    }
}
