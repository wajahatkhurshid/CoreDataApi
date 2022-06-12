using System;
using System.Collections.Generic;
using System.Linq;
using Gyldendal.Api.CoreData.Business.ExtensionMethods;
using Gyldendal.Api.CoreData.Business.Porter.Interfaces.DataProviders;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.DataAccess.KoncernData;

namespace Gyldendal.Api.CoreData.Business.Porter.Services.DataProviders.Product
{
    /// <summary>
    /// Provides common functionality of different shop specific ProductService data providers
    /// </summary>
    public abstract class BaseProductServiceDataProvider : IProductServiceDataProvider
    {
        protected readonly koncerndata_webshops_Entities KdEntities;

        protected BaseProductServiceDataProvider(koncerndata_webshops_Entities kdEntities)
        {
            KdEntities = kdEntities;
        }

        protected abstract void PopulateAdditionalPropertiesFor_GetProductByIsbn(Contracts.Models.Product product);

        public Contracts.Models.Product ProvideDataFor_GetProductByIsbn(Contracts.Models.Product product)
        {
            var clonedProduct = product;
            var website = clonedProduct.WebShop.KdWebshopName() ?? "";

            var campaignQueryable = GlobalDiscountQuery(clonedProduct.Isbn13, clonedProduct.MediaType.Name, website);
            var campaignId = campaignQueryable.Select(y => y.Id).FirstOrDefault();

            clonedProduct.HasOtherDiscount = DiscountQuery(clonedProduct.Isbn13, clonedProduct.MediaType.Name, website).Any(x => x.Id != campaignId);
            clonedProduct.DiscountPercentage = campaignQueryable.FirstOrDefault()?.DiscountPercentage ?? 0m;

            PopulateAdditionalPropertiesFor_GetProductByIsbn(clonedProduct);

            return clonedProduct;
        }

        public IEnumerable<string> ProvideDataFor_GetModifiedSingleProductCampaignsIsbn(DateTime updatedAfterTicks, string shopName)
        {
            var campaigns = GetModifiedCampaigns(updatedAfterTicks, shopName);
            var campaignProducts = SingleCampaignProductIds(campaigns);
            
            return campaignProducts.Distinct();
        }

        public IEnumerable<string> ProvideDataFor_GetModifiedBundleCampaignsPakkeId(DateTime updatedAfterTicks, string shopName)
        {
            var campaigns = GetModifiedCampaigns(updatedAfterTicks, shopName);
            var bundleCampaigns = BundleCampaignPakkeIds(campaigns);
            
            return bundleCampaigns.Distinct();
        }

        public IEnumerable<string> ProvideDataFor_GetModifiedBundleCampaignsUpdateInfo(DateTime updatedAfterTicks, string shopName, int pageIndex, int pageSize)
        {
            var campaigns = GetModifiedCampaigns(updatedAfterTicks, shopName);
            var result = BundleCampaignUpdateInfo(campaigns, pageIndex, pageSize);
            return result.Select(x => x.ProductId).ToList();
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

            return KdEntities.Campaign.OrderByDescending(a => a.DiscountPercentage)
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

        protected bool IsProductBuyable(bool isPublished, Contracts.Models.Product product)
        {
            var isBuyable = product.IsPhysical ? (product.InStock || product.IsNextPrintPlanned) :
                (product.SalesConfiguration != null
                 && product.SalesConfiguration.AccessForms.Count > 0
                 && product.SalesConfiguration.SalesForms.Count > 0);

            // Evaluates whether product is buyable or not. Product which are not published, can also be buyable in case of GU i.e Back Orders
            if (isPublished)
            {
                return isBuyable && product.IsPublished;
            }

            return isBuyable;
        }

        protected ProductFreeMaterial GetProductFreeMaterial(string fileName, string description, string publizonIdentifier)
        {
            return new ProductFreeMaterial
            {
                FileName = fileName,
                Description = description,
                PublizonIdentifier = publizonIdentifier
            };
        }

        protected IQueryable<Campaign> GetModifiedCampaigns(DateTime updatedAfter, string shopName)
        {
            var currentDate = DateTime.Now;
            var campaigns = KdEntities.Campaign.Where(x => ((x.CreatedAt >= updatedAfter && x.CreatedAt <= currentDate)
                                                           || (x.ModifiedAt >= updatedAfter && x.ModifiedAt <= currentDate)
                                                           || (x.StartDate >= updatedAfter && x.StartDate <= currentDate)
                                                           || (x.EndDate >= updatedAfter && x.EndDate <= currentDate))
                                                           && (x.ShopName == shopName));
            return campaigns;
        }

        protected IQueryable<string> SingleCampaignProductIds(IQueryable<Campaign> campaigns)
        {
            return campaigns.Join(KdEntities.CampaignItem, c => c.Id, ci => ci.CampaignId,
                    (c, ci) => new { c, ci })
                .Where(x => (x.c.CampaignType == 4))
                .Select(x => x.ci.VareId);
        }

        protected IQueryable<string> BundleCampaignPakkeIds(IQueryable<Campaign> campaigns)
        {
            return campaigns.Join(KdEntities.CampaignItem, c => c.Id, ci => ci.CampaignId,
                    (c, ci) => new { c, ci })
                .Where(x => (x.c.CampaignType == 2))
                .Select(x => x.ci.VareId);
        }

        protected IEnumerable<Contracts.Response.ProductUpdateInfo> BundleCampaignUpdateInfo(IQueryable<Campaign> campaigns, int pageIndex, int pageSize)
        {
            return campaigns.Join(KdEntities.CampaignItem, c => c.Id, ci => ci.CampaignId,
                    (c, ci) => new { c, ci })
                .Where(x => (x.c.CampaignType == 2))
                .OrderBy(x => x.c.Id)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToArray()
                .Select(x => new Contracts.Response.ProductUpdateInfo(){
                    ProductId = x.ci.VareId,
                    ProductType = Contracts.Enumerations.ProductType.Bundle, 
                    UpdateTime = x.c.ModifiedAt.HasValue ? x.c.ModifiedAt.Value : DateTime.MinValue,
                    UpdateType = (!x.c.IsActive || (x.c.IsActive && (x.c.EndDate.HasValue && x.c.EndDate.Value <= DateTime.Now))) ? Contracts.Response.ProductUpdateType.Deleted : Contracts.Response.ProductUpdateType.Updated
                    });
        }
    }
}