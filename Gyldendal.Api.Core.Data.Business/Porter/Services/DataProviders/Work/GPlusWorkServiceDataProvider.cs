using System.Collections.Generic;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Business.Util;
using Gyldendal.Api.CoreData.Common.ConfigurationManager;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Response;
using Gyldendal.Api.CoreData.DataAccess.KoncernData;
using PorterApi = Gyldendal.Api.CoreData.Services.PorterApiClient;
using Gyldendal.Api.CoreData.Business.Porter.Mapping;

namespace Gyldendal.Api.CoreData.Business.Porter.Services.DataProviders.Work
{
    public class GPlusWorkServiceDataProvider : BaseWorkServiceDataProvider
    {
        public GPlusWorkServiceDataProvider(koncerndata_webshops_Entities kdEntities, ShopServices.ApiClient.Client shopServicesApiClient, ICoverImageUtil imageUtil, IConfigurationManager configManager) : 
            base(kdEntities, shopServicesApiClient, imageUtil, configManager)
        {
        }
        protected override GetProductDetailsResponse GetWorkForSingleProduct(Contracts.Models.Work work, string isbn)
        {
            return new GetProductDetailsResponse
            {
                Message = "",
                ProductNotFoundReason = ProductNotFoundReason.None,
                ProductWork = work
            };
        }

        protected override Contracts.Models.Work GetWorkForBundleProducts(string bundleId,
            List<PorterApi.Product> porterProducts, WebShop webshop)
        {
            return null;
        }
    }
}
