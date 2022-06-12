using System;
using System.Collections.Generic;
using System.Linq;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Business.Porter.Interfaces.DataProviders;
using Gyldendal.Api.CoreData.DataAccess.KoncernData;
using Gyldendal.Api.CoreData.Contracts.Response;
using PorterApi = Gyldendal.Api.CoreData.Services.PorterApiClient;
using Gyldendal.Api.CoreData.Business.Porter.Mapping;
using Gyldendal.Api.CoreData.Business.Util;
using Gyldendal.Api.CoreData.Common.ConfigurationManager;


namespace Gyldendal.Api.CoreData.Business.Porter.Services.DataProviders.Work
{
    /// <summary>
    /// Provides common functionality of different shop specific WorkService data providers
    /// </summary>
    public abstract class BaseWorkServiceDataProvider : IWorkServiceDataProvider
    {
        protected readonly koncerndata_webshops_Entities _KdEntities;
        public readonly ShopServices.ApiClient.Client _shopServicesApiClient;
        public readonly ICoverImageUtil _imageUtil;
        public readonly IConfigurationManager _configManager;

        protected BaseWorkServiceDataProvider(koncerndata_webshops_Entities kdEntities, ShopServices.ApiClient.Client shopServicesApiClient, ICoverImageUtil imageUtil, IConfigurationManager configManager)
        {
            _KdEntities = kdEntities;
            _shopServicesApiClient = shopServicesApiClient;
            _imageUtil = imageUtil;
            _configManager = configManager;
        }

        protected abstract GetProductDetailsResponse GetWorkForSingleProduct(Contracts.Models.Work work, string isbn);

        protected abstract Contracts.Models.Work GetWorkForBundleProducts(string bundleId, List<PorterApi.Product> porterProducts, WebShop webshop);

        public GetProductDetailsResponse ProvideDataFor_GetWorkByProduct(Contracts.Models.Work work, string isbn)
        {
            return GetWorkForSingleProduct(work, isbn);
        }

        public Contracts.Models.Work ProvideDataFor_GetWorkForBundleProduct(string bundleId, List<PorterApi.Product> porterProducts, WebShop webshop)
        {
            return GetWorkForBundleProducts(bundleId, porterProducts, webshop);
        }

        public IEnumerable<string> GetBundleProductIsbnsFromKd(string pakkeId)
        {
            return (from bundleProducts in _KdEntities.pakker
                where bundleProducts.pakke == pakkeId
                select bundleProducts.vare).ToArray();
        }

        protected IQueryable<Campaign> GlobalDiscountQuery(string isbn13, string mediaType, string website)
        {
            return DiscountQuery(isbn13, mediaType, website)
                .Where(a => a.CampaignMembership.Any() == false
                            && a.MaxQuantityAllowed == null
                            && (a.CouponCode == null || a.CouponCode == ""));
        }
        protected IQueryable<Campaign> DiscountQuery(string isbn13, string mediaType, string website)
        {
            var currentDate = DateTime.Now;

            return _KdEntities.Campaign.OrderByDescending(a => a.DiscountPercentage)
                .Where(a => (
                        (
                            (a.CampaignItem.Any(x => x.VareId.Equals(isbn13)) && (a.CampaignType == 1 || a.CampaignType == 4))
                            ||
                            (a.MediaType == mediaType && (a.CampaignType == 3))
                        )
                        && a.StartDate <= currentDate
                        && (a.EndDate == null || a.EndDate >= currentDate)
                        && a.IsActive
                        && (website == "" || a.ShopName == website)
                    )
                );
        }
        public bool IsProductBuyable(Contracts.Models.Product product)
        {
            return product.IsPhysical ? (product.InStock || product.IsNextPrintPlanned) :
                (product.SalesConfiguration != null
                 && product.SalesConfiguration.AccessForms.Count > 0
                 && product.SalesConfiguration.SalesForms.Count > 0);
        }
        protected bool HasOtherDiscounts(string isbn, string mediaType, string website)
        {

            var globalDiscountCampaignId = GlobalDiscountQuery(isbn, mediaType, website).
                Select(y => y.Id).FirstOrDefault();

            return DiscountQuery(isbn, mediaType, website)
                .Any(x => x.Id != globalDiscountCampaignId);
        }
        protected decimal? GetProductDiscount(string isbn13, string mediaType, string website)
        {
            return GlobalDiscountQuery(isbn13, mediaType, website).FirstOrDefault()
                ?.DiscountPercentage ?? 0m;
        }
    }
}