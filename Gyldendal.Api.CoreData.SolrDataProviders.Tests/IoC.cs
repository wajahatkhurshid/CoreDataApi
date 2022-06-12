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
using Gyldendal.Api.CoreData.GqlToSolrConnector.SolrSearch;
using Gyldendal.Api.CoreData.ShopAdmin.Implementation;
using Gyldendal.Api.CoreData.SolrDataProviders.Contributor;
using Gyldendal.Api.CoreData.SolrDataProviders.Infrastructure;
using Gyldendal.Api.CoreData.SolrDataProviders.Mappings;
using Gyldendal.Api.CoreData.SolrDataProviders.Product;
using Gyldendal.Api.CoreData.SolrDataProviders.Utils;
using Gyldendal.Api.CoreData.SolrDataProviders.Work;
using Gyldendal.Api.CoreData.SolrDataProviders.WorkReview;
using LoggingManager;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;
using ContributorRepository = Gyldendal.Api.CoreData.Business.Repositories.Munks.ContributorRepository;
using MasterDataRepository = Gyldendal.Api.CoreData.Business.Repositories.Munks.MasterDataRepository;
using ProductRepository = Gyldendal.Api.CoreData.Business.Repositories.Munks.ProductRepository;
using WorkRepository = Gyldendal.Api.CoreData.Business.Repositories.Munks.WorkRepository;

namespace Gyldendal.Api.CoreData.SolrDataProviders.Tests
{
    /// <summary>
    /// Ioc container
    /// </summary>
    public static class Ioc
    {
        /// <summary>
        /// container of simple injector
        /// </summary>
        public static Container Container { get; }

        static Ioc()
        {
            Container = new Container();
        }

        /// <summary>
        /// Setup all interfaces
        /// </summary>
        public static void SetupContainer()
        {
            Container.Options.DefaultScopedLifestyle = new WebApiRequestLifestyle();

            Container.Register<ILogger, Logger>(Lifestyle.Singleton);
            Container.Register<IConfigurationManager, ConfigurationManager>(Lifestyle.Singleton);
            Container.Register<ICoverImageUtil, CoverImageUtil>(Lifestyle.Singleton);

            Container.Register<koncerndata_webshops_Entities>(Lifestyle.Scoped);
            Container.Register<ShopAdminEntities>(Lifestyle.Scoped);

            Container.Register<IErrorCodeUtil, ErrorCodeUtil>();
            Container.Register<IPublizonProducts, ShopAdmin.Infrastructure.PublizonProducts>();

            Container.RegisterCollection<IMasterDataRepository>(new[] {
                typeof(MasterDataRepository),
                typeof(Business.Repositories.GU.MasterDataRepository),
                //typeof(Business.Repositories.RosinanteCo.MasterDataRepository),
                typeof(Business.Repositories.HR.MasterDataRepository),
                typeof(Business.Repositories.GPlus.MasterDataRepository)
            });

            Container.RegisterCollection<IProductRepository>(new[] {
                typeof(ProductRepository),
                typeof(Business.Repositories.GU.ProductRepository),
                typeof(Business.Repositories.HR.ProductRepository),
                //typeof(Business.Repositories.RosinanteCo.ProductRepository),
                typeof(Business.Repositories.WsNone.ProductRepository),
                typeof(Business.Repositories.GPlus.ProductRepository)
            });

            Container.RegisterCollection<IWorkRepository>(new[] {
                typeof(WorkRepository),
                typeof(Business.Repositories.GU.WorkRepository),
                typeof(Business.Repositories.HR.WorkRepository),
                //typeof(Business.Repositories.RosinanteCo.WorkRepository),
                typeof(Business.Repositories.WsNone.WorkRepository),
                typeof(Business.Repositories.GPlus.WorkRepository)
            });
            
            Container.RegisterCollection<ISystemSeriesRepository>(new[] {
                typeof(SystemSeriesRepository),
                typeof(Business.Repositories.GU.SystemSeriesRepository),
                typeof(Business.Repositories.HR.SystemSeriesRepository),
                typeof(Business.Repositories.GPlus.SystemSeriesRepository)
            });

            Container.RegisterCollection<IContributorRepository>(new[] {
                typeof(ContributorRepository),
                typeof(Business.Repositories.GU.ContributorRepository),
                typeof(Business.Repositories.HR.ContributorRepository),
                typeof(Business.Repositories.Global.ContributorRepository),
                //typeof(Business.Repositories.RosinanteCo.ContributorRepository),
                typeof(Business.Repositories.GPlus.ContributorRepository)
            });

            Container.Register<ICommonLookupsRepository, CommonLookupsRepository>();

            Container.Register<IMasterDataFactory, MasterDataFactory>();
            Container.Register<IProductFactory, ProductFactory>();
            Container.Register<ResultProcessor>();
            Container.Register<IWorkFactory, WorkFactory>();
            Container.Register<IWorkReviewsFactory, WorkReviewsFactory>();
            Container.Register<ISystemSeriesFactory, SystemSeriesFactory>();
            Container.Register<IContributorFactory, ContributorFactory>();
            Container.Register<IKoncernDataUtils, KoncernDataUtils>();

            Container.Register<WorkReviewSolrFilterGenerator, WorkReviewSolrFilterGenerator>();
            Container.Register<ProductSolrFilterGenerator, ProductSolrFilterGenerator>();
            Container.Register<ContributorSolrFilterGenerator, ContributorSolrFilterGenerator>();
            Container.Register<IFilterInfoToSolrQueryBuilder, Utils.FilterInfoToSolrQueryBuilder>();


            var solrCore = string.Empty;
            var solrUrl = "http://dummyurl:8983/solr";

            Container.Register<ISearch<SolrContracts.Product.Product>>(() =>
                new GenericSolrSearch<SolrContracts.Product.Product>(solrCore, solrUrl));

            Container
                .Register<ISearch<SolrContracts.Contributor.Contributor>>(() =>
                    new GenericSolrSearch<SolrContracts.Contributor.Contributor>(solrCore, solrUrl));

            Container
                .Register<ISearch<SolrContracts.WorkReview.WorkReview>>(() =>
                    new GenericSolrSearch<SolrContracts.WorkReview.WorkReview>(solrCore, solrUrl));

            Container.Register<IProductDataProvider>(
                () =>
                    new ProductDataProvider(solrCore, solrUrl,
                        GqlToSolrFieldMapping.GetMappings(),
                        System.Configuration.ConfigurationManager.AppSettings["MediaTypeValues"].Split(','),
                        Container.GetInstance<ResultProcessor>(),
                        Container.GetInstance<ProductSolrFilterGenerator>(),
                        Container.GetInstance<IFilterInfoToSolrQueryBuilder>(),
                        Container.GetInstance<ISearch<SolrContracts.Product.Product>>()
                    ));

            Container.Register<IWorkDataProvider>(
                () =>
                    new WorkDataProvider(solrUrl, solrCore,
                        GqlToSolrFieldMapping.GetMappings(),
                        System.Configuration.ConfigurationManager.AppSettings["MediaTypeValues"].Split(','),
                        Container.GetInstance<ResultProcessor>(),
                        Container.GetInstance<ProductSolrFilterGenerator>(),
                        Container.GetInstance<IFilterInfoToSolrQueryBuilder>(),
                        Container.GetInstance<ISearch<SolrContracts.Product.Product>>()
                        ));

            Container.Register<IContributorDataProvider>(
                () =>
                    new ContributorDataProvider(solrCore, solrUrl,
                        Container.GetInstance<ContributorSolrFilterGenerator>(),
                        Container.GetInstance<IFilterInfoToSolrQueryBuilder>(),
                        Container.GetInstance<ISearch<SolrContracts.Contributor.Contributor>>()
                        ));
            Container.Register<IWorkReviewDataProvider>(
                () =>
                    new WorkReviewDataProvider(solrCore, solrUrl,
                        Container.GetInstance<WorkReviewSolrFilterGenerator>(),
                        Container.GetInstance<IFilterInfoToSolrQueryBuilder>(),
                        Container.GetInstance<ISearch<SolrContracts.WorkReview.WorkReview>>()
                        ));

            Container.Verify();
        }
    }
}