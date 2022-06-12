using System.Net.Http;
using Gyldendal.Api.CoreData.Business.Factories;
using Gyldendal.Api.CoreData.Business.Repositories.Common;
using Gyldendal.Api.CoreData.Business.Repositories.Munks;
using Gyldendal.Api.CoreData.Business.Util;
using Gyldendal.Api.CoreData.Common.ConfigurationManager;
using Gyldendal.Api.CoreData.Common.DataProviderInfrastructure;
using Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Factories;
using Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Repositories;
using Gyldendal.Api.CoreData.DataAccess.KoncernData;
using Gyldendal.Api.CoreData.DataAccess.ShopAdmin;
using Gyldendal.Api.CoreData.ShopAdmin.Implementation;
using Gyldendal.Api.CoreData.SolrDataProviders.Contributor;
using Gyldendal.Api.CoreData.SolrDataProviders.Infrastructure;
using Gyldendal.Api.CoreData.SolrDataProviders.Mappings;
using Gyldendal.Api.CoreData.SolrDataProviders.Product;
using Gyldendal.Api.CoreData.SolrDataProviders.Utils;
using Gyldendal.Api.CoreData.SolrDataProviders.Work;
using Gyldendal.Api.CoreData.SolrDataProviders.WorkReview;
using Gyldendal.PulsenServices.ApiClient;
using SimpleInjector;
using System.Web.Configuration;
using Gyldendal.Api.CoreData.Business.Porter.Interfaces;
using Gyldendal.Api.CoreData.Business.Porter.Services;
using Gyldendal.Api.CoreData.Business.Repositories.Global;
using Gyldendal.Api.CoreData.Common.EventInfrastructure;
using Gyldendal.Api.CoreData.Common.Logging;
using Gyldendal.Api.CoreData.ContentfulProxy.Implementation;
using Gyldendal.Api.CoreData.ContentfulProxy.Infrastructure;
using Gyldendal.Api.CoreData.GqlToSolrConnector.SolrSearch;
using Gyldendal.Api.CoreData.ResultsPostProcessing;
using Gyldendal.Api.CoreData.ResultsPostProcessing.Infrastructure;
using Gyldendal.Api.CoreData.ResultsPostProcessing.Processors.GPlus;
using Gyldendal.Api.CoreData.SolrContracts.Contributor;
using Gyldendal.Api.CoreData.SolrContracts.Product;
using Gyldendal.Api.CoreData.SolrContracts.WorkReview;
using PorterApiClient = Gyldendal.Api.CoreData.Services.PorterApiClient;
using LoggingManager.Entities;
using MMSImageService;
using ContributorRepository = Gyldendal.Api.CoreData.Business.Repositories.Munks.ContributorRepository;
using MasterDataRepository = Gyldendal.Api.CoreData.Business.Repositories.Munks.MasterDataRepository;
using ProductRepository = Gyldendal.Api.CoreData.Business.Repositories.Munks.ProductRepository;
using WorkRepository = Gyldendal.Api.CoreData.Business.Repositories.Munks.WorkRepository;
using Gyldendal.Api.CoreData.Business.Provider;

namespace Gyldendal.Api.CoreData.Bootstrapping
{
    /// <summary>
    /// Ioc container
    /// </summary>
    public static class Ioc
    {
        /// <summary>
        /// container of simple injector
        /// </summary>
        public static Container Container { get; set; }

        /// <summary>
        /// Setup all interfaces
        /// </summary>
        public static void SetupContainer(Container container)
        {
            Container = container;

            RegisterUtilities();

            RegisterDatabaseEntities();

            RegisterClients();

            RegisterContentfulProxy();

            RegisterRepositories();

            RegisterFactories();

            RegisterSolrFilterGenerators();

            RegisterSolrSearch();

            RegisterDataProviders();

            RegisterPostProcessing();

            RegisterEventProcessing();

            RegisterPressSite();

            RegisterGga();

            RegisterPorterDependencies();
        }

        private static void RegisterUtilities()
        {
            Container.Register<ILogger, Logger>(Lifestyle.Singleton);
            Container.Register<LoggingManager.ILogger, LoggingManager.Logger>(Lifestyle.Singleton);
            Container.Register<ILoggingManagerConfig, LoggingManagerConfig>(Lifestyle.Singleton);
            Container.Register<IConfigurationManager, ConfigurationManager>(Lifestyle.Singleton);
            Container.Register<ICoverImageUtil, CoverImageUtil>(Lifestyle.Singleton);
            Container.Register<IErrorCodeUtil, ErrorCodeUtil>();
        }

        private static void RegisterDatabaseEntities()
        {
            Container.Register<koncerndata_webshops_Entities>(Lifestyle.Scoped);
            Container.Register<ShopAdminEntities>(Lifestyle.Scoped);
            Container.Register<IKoncernDataUtils, KoncernDataUtils>();
            Container.Register<IPublizonProducts, ShopAdmin.Infrastructure.PublizonProducts>();
        }

        private static void RegisterClients()
        {
            Container.Register(() =>
                new ShopServices.ApiClient.Client(Container.GetInstance<IConfigurationManager>().ShopServicesUrl));
            Container.Register(() =>
                new PulsenServiceApiClient(Container.GetInstance<IConfigurationManager>().PulsenServicesUrl));
            Container.Register(()
                => new MMSImageServiceSoapClient(MMSImageServiceSoapClient.EndpointConfiguration.MMSImageServiceSoap,
                    Container.GetInstance<IConfigurationManager>().MmmsWcfServiceUrl), Lifestyle.Singleton);
            Container.Register<IContentfulApiClient>(() =>
                new ContentfulApiClient(
                    Container.GetInstance<IConfigurationManager>().Contentful_SpaceId,
                    Container.GetInstance<IConfigurationManager>().Contentful_Environment,
            Container.GetInstance<IConfigurationManager>().Contentful_PreviewApiKey,
            Container.GetInstance<IConfigurationManager>().Contentful_DeliveryApiKey
                    ));
        }

        private static void RegisterContentfulProxy()
        {
            Container.Register<IContentfulManager, ContentfulManager>();
        }

        private static void RegisterRepositories()
        {
            Container.RegisterCollection<IMasterDataRepository>(new[]
            {
                typeof(MasterDataRepository),
                typeof(Business.Repositories.GU.MasterDataRepository),
                typeof(Business.Repositories.HR.MasterDataRepository),
                typeof(Business.Repositories.GPlus.MasterDataRepository),
                typeof(Business.Repositories.TradeGDK.MasterDataRepository)
            });

            Container.RegisterCollection<IProductRepository>(new[]
            {
                typeof(ProductRepository),
                typeof(Business.Repositories.GU.ProductRepository),
                typeof(Business.Repositories.HR.ProductRepository),
                typeof(Business.Repositories.WsNone.ProductRepository),
                typeof(Business.Repositories.GPlus.ProductRepository),
                typeof(Business.Repositories.Global.ProductRepository),
                typeof(Business.Repositories.StudyBox.ProductRepository),
                typeof(Business.Repositories.TradeGDK.ProductRepository)
            });

            Container.RegisterCollection<IWorkRepository>(new[]
            {
                typeof(WorkRepository),
                typeof(Business.Repositories.GU.WorkRepository),
                typeof(Business.Repositories.HR.WorkRepository),
                typeof(Business.Repositories.WsNone.WorkRepository),
                typeof(Business.Repositories.GPlus.WorkRepository),
                typeof(Business.Repositories.TradeGDK.WorkRepository)
            });

            Container.RegisterCollection<IWorkReviewsRepository>(new[]
            {
                typeof(Business.Repositories.GPlus.WorkReviewsRepository),
                typeof(Business.Repositories.TradeGDK.WorkReviewsRepository)
            });

            Container.RegisterCollection<ISystemSeriesRepository>(new[]
            {
                typeof(SystemSeriesRepository),
                typeof(Business.Repositories.GU.SystemSeriesRepository),
                typeof(Business.Repositories.HR.SystemSeriesRepository),
                typeof(Business.Repositories.GPlus.SystemSeriesRepository)
            });

            Container.RegisterCollection<IContributorRepository>(new[]
            {
                typeof(ContributorRepository),
                typeof(Business.Repositories.GU.ContributorRepository),
                typeof(Business.Repositories.HR.ContributorRepository),
                typeof(Business.Repositories.Global.ContributorRepository),
                typeof(Business.Repositories.GPlus.ContributorRepository),
                typeof(Business.Repositories.TradeGDK.ContributorRepository)
            });

            Container.Register<ICommonLookupsRepository, CommonLookupsRepository>();
            Container.Register<ICoreDataAgentRepository, CoreDataAgentRepository>();


            Container.Register<PorterApiClient.IPorterClient>(() => new PorterApiClient.PorterClient(WebConfigurationManager.AppSettings["PorterApiBaseUrl"], new HttpClient()));
        }

        private static void RegisterFactories()
        {
            Container.Register<IMasterDataFactory, MasterDataFactory>();
            Container.Register<IProductFactory, ProductFactory>();
            Container.Register<ResultProcessor>();
            Container.Register<IWorkFactory, WorkFactory>();
            Container.Register<IWorkReviewsFactory, WorkReviewsFactory>();
            Container.Register<ISystemSeriesFactory, SystemSeriesFactory>();
            Container.Register<IContributorFactory, ContributorFactory>();
        }

        private static void RegisterSolrFilterGenerators()
        {
            Container.Register<WorkReviewSolrFilterGenerator, WorkReviewSolrFilterGenerator>();
            Container.Register<ProductSolrFilterGenerator, ProductSolrFilterGenerator>();
            Container.Register<ContributorSolrFilterGenerator, ContributorSolrFilterGenerator>();
            Container.Register<IFilterInfoToSolrQueryBuilder, FilterInfoToSolrQueryBuilder>();
        }

        private static void RegisterSolrSearch()
        {
            Container.Register<ISearch<Product>>(() =>
                new GenericSolrSearch<Product>(WebConfigurationManager.AppSettings["ProductSolrCore"],
                    WebConfigurationManager.AppSettings["SolrUrl"]
                ));

            Container
                .Register<ISearch<Contributor>>(() =>
                    new GenericSolrSearch<Contributor>(WebConfigurationManager.AppSettings["ContributorSolrCore"],
                        WebConfigurationManager.AppSettings["SolrUrl"]));

            Container
                .Register<ISearch<WorkReview>>(() =>
                    new GenericSolrSearch<WorkReview>(WebConfigurationManager.AppSettings["WorkReviewSolrCore"],
                        WebConfigurationManager.AppSettings["SolrUrl"]));
        }

        private static void RegisterDataProviders()
        {
            Container.Register<IProductDataProvider>(
                () =>
                    new ProductDataProvider(WebConfigurationManager.AppSettings["ProductSolrCore"],
                        WebConfigurationManager.AppSettings["SolrUrl"],
                        GqlToSolrFieldMapping.GetMappings(),
                        WebConfigurationManager.AppSettings["MediaTypeValues"].Split(','),
                        Container.GetInstance<ResultProcessor>(),
                        Container.GetInstance<ProductSolrFilterGenerator>(),
                        Container.GetInstance<IFilterInfoToSolrQueryBuilder>(),
                        Container.GetInstance<ISearch<Product>>()
                    ));

            Container.Register<IWorkDataProvider>(
                () =>
                    new WorkDataProvider(WebConfigurationManager.AppSettings["SolrUrl"],
                        WebConfigurationManager.AppSettings["ProductSolrCore"],
                        GqlToSolrFieldMapping.GetMappings(),
                        WebConfigurationManager.AppSettings["MediaTypeValues"].Split(','),
                        Container.GetInstance<ResultProcessor>(),
                        Container.GetInstance<ProductSolrFilterGenerator>(),
                        Container.GetInstance<IFilterInfoToSolrQueryBuilder>(),
                        Container.GetInstance<ISearch<Product>>()
                    ));

            Container.Register<IContributorDataProvider>(
                () =>
                    new ContributorDataProvider(WebConfigurationManager.AppSettings["ContributorSolrCore"],
                        WebConfigurationManager.AppSettings["SolrUrl"],
                        Container.GetInstance<ContributorSolrFilterGenerator>(),
                        Container.GetInstance<IFilterInfoToSolrQueryBuilder>(),
                        Container.GetInstance<ISearch<Contributor>>()
                    ));
            Container.Register<IWorkReviewDataProvider>(
                () =>
                    new WorkReviewDataProvider(WebConfigurationManager.AppSettings["WorkReviewSolrCore"],
                        WebConfigurationManager.AppSettings["SolrUrl"],
                        Container.GetInstance<WorkReviewSolrFilterGenerator>(),
                        Container.GetInstance<IFilterInfoToSolrQueryBuilder>(),
                        Container.GetInstance<ISearch<WorkReview>>()
                    ));
        }

        private static void RegisterPostProcessing()
        {
            Container.Register<IWorksResultProcessesExecutor, WorksResultProcessesExecutor>();
            Container.Register<IWorksResultFactory, WorksResultFactory>();
            Container.Register<IWorkResultsProcessor, WorkResultsProcessor>();
            Container.RegisterCollection<IWorkResultsProcessor>(new[]
            {
                typeof(WorkResultsProcessor)
            });
        }

        private static void RegisterEventProcessing()
        {
            Container.Register<IEventProcessor, EventProcessor.EventProcessor>();
            Container.Register<IEventHandlerFactory, EventProcessor.EventHandlerFactory>();
        }

        private static void RegisterPressSite()
        {
            Container.Register<IPressSiteProvider, PressSiteProvider>();
        }

        private static void RegisterGga()
        {
            Container.Register<IGGAProvider, GGAProvider>();
        }

        private static void RegisterPorterDependencies()
        {
            Container.Register<IMasterDataService, MasterDataService>();
            Container.Register<IWorkService, WorkService>();
            Container.Register<IProductService, ProductService>();
            Container.Register<ISeriesService, SeriesService>();
            Container.Register<IContributorService, ContributorService>();
            Container.Register<IServiceDataProviderFactory, ServiceDataProviderFactory>();
            Container.Register<IWorkReviewService, WorkReviewService>();
            Container.Register<IGgaService, GgaService>();
            Container.Register<IPressSiteService, PressSiteService>();
        }
    }
}