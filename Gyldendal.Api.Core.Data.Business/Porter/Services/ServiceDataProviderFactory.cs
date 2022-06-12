using System;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Business.Porter.Interfaces;
using Gyldendal.Api.CoreData.Business.Porter.Interfaces.DataProviders;
using Gyldendal.Api.CoreData.Business.Porter.Services.DataProviders.Contributor;
using Gyldendal.Api.CoreData.Business.Porter.Services.DataProviders.Product;
using Gyldendal.Api.CoreData.Business.Util;
using Gyldendal.Api.CoreData.Common.ConfigurationManager;
using Gyldendal.Api.CoreData.ContentfulProxy.Infrastructure;
using Gyldendal.Api.CoreData.DataAccess.KoncernData;
using Gyldendal.Api.CoreData.ShopAdmin.Implementation;
using Gyldendal.PulsenServices.ApiClient;
using LoggingManager;

namespace Gyldendal.Api.CoreData.Business.Porter.Services
{
    public class ServiceDataProviderFactory : IServiceDataProviderFactory
    {
        private readonly koncerndata_webshops_Entities _kdEntities;
        private readonly IPublizonProducts _publizonProducts;
        private readonly IContentfulManager _contentfulManager;
        private readonly ILogger _logger;
        private IServiceDataProviderFactory _serviceDataProviderFactoryImplementation;
        private readonly PulsenServiceApiClient _pulsenServiceApiClient;
        private readonly ShopServices.ApiClient.Client _shopServicesApiClient;
        private readonly ICoverImageUtil _imageUtil;
        private readonly IConfigurationManager _configManager;

        public ServiceDataProviderFactory(koncerndata_webshops_Entities kdEntities, IPublizonProducts publizonProducts, IContentfulManager contentfulManager, PulsenServiceApiClient pulsenServiceApiClient, ShopServices.ApiClient.Client shopServicesApiClient, ICoverImageUtil imageUtil, IConfigurationManager configManager, ILogger logger)
        {
            _kdEntities = kdEntities;
            _publizonProducts = publizonProducts;
            _contentfulManager = contentfulManager;
            _pulsenServiceApiClient = pulsenServiceApiClient;
            _shopServicesApiClient = shopServicesApiClient;
            _imageUtil = imageUtil;
            _configManager = configManager;
            _logger = logger;
        }

        public IContributorServiceDataProvider GetContributorServiceDataProvider(WebShop webShop)
        {
            switch (webShop)
            {
                case WebShop.TradeGyldendalDk:
                    return new GdkContributorServiceDataProvider(_contentfulManager, _logger);

                case WebShop.Gu:
                case WebShop.HansReitzel:
                case WebShop.MunksGaard:
                    return new GuContributorServiceDataProvider();

                case WebShop.ClubBogklub:
                case WebShop.ClubBoerne:
                case WebShop.ClubBoerne3To5:
                case WebShop.ClubBoerne5To10:
                case WebShop.ClubFlamingo:
                case WebShop.ClubHistorie:
                case WebShop.ClubKrimi:
                case WebShop.ClubPaedagogisk:
                case WebShop.ClubPsykeSjael:
                case WebShop.ClubSamleren:
                case WebShop.ClubSundtLiv:
                    return new GplusContributorServiceDataProvider(_contentfulManager, _logger);


                default:
                    throw new ArgumentOutOfRangeException(nameof(webShop), webShop, null);
            }
        }

        public IProductServiceDataProvider GetProductServiceDataProvider(WebShop webShop)
        {
            switch (webShop)
            {
                case WebShop.TradeGyldendalDk:
                    return new GdkProductServiceDataProvider(_kdEntities, _publizonProducts);

                case WebShop.Gu:
                case WebShop.HansReitzel:
                case WebShop.MunksGaard:
                    return new GuProductServiceDataProvider(_kdEntities);

                case WebShop.ClubBogklub:
                case WebShop.ClubBoerne:
                case WebShop.ClubBoerne3To5:
                case WebShop.ClubBoerne5To10:
                case WebShop.ClubFlamingo:
                case WebShop.ClubHistorie:
                case WebShop.ClubKrimi:
                case WebShop.ClubPaedagogisk:
                case WebShop.ClubPsykeSjael:
                case WebShop.ClubSamleren:
                case WebShop.ClubSundtLiv:
                    return new GPlusProductServiceDataProvider(_kdEntities);

                default:
                    throw new ArgumentOutOfRangeException(nameof(webShop), webShop, null);
            }
        }

        public IWorkServiceDataProvider GetWorkServiceDataProvider(WebShop webShop)
        {
            switch (webShop)
            {
                case WebShop.TradeGyldendalDk:
                    return new DataProviders.Work.GdkWorkServiceDataProvider(_kdEntities, _publizonProducts, _pulsenServiceApiClient, _shopServicesApiClient, _imageUtil, _configManager);

                case WebShop.Gu:
                case WebShop.HansReitzel:
                case WebShop.MunksGaard:
                case WebShop.None:
                    return new DataProviders.Work.GuWorkServiceDataProvider(_kdEntities, _shopServicesApiClient, _imageUtil, _configManager);

                case WebShop.ClubBogklub:
                case WebShop.ClubBoerne:
                case WebShop.ClubBoerne3To5:
                case WebShop.ClubBoerne5To10:
                case WebShop.ClubFlamingo:
                case WebShop.ClubHistorie:
                case WebShop.ClubKrimi:
                case WebShop.ClubPaedagogisk:
                case WebShop.ClubPsykeSjael:
                case WebShop.ClubSamleren:
                case WebShop.ClubSundtLiv:
                    return new DataProviders.Work.GPlusWorkServiceDataProvider(_kdEntities, _shopServicesApiClient, _imageUtil, _configManager);

                default:
                    throw new ArgumentOutOfRangeException(nameof(webShop), webShop, null);
            }
        }
    }
}
