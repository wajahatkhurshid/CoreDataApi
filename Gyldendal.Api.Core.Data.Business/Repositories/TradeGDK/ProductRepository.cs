using System;
using System.Collections.Generic;
using System.Linq;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Business.Repositories.Common;
using Gyldendal.Api.CoreData.Business.Util;
using Gyldendal.Api.CoreData.Common.ConfigurationManager;
using Gyldendal.Api.CoreData.Common.Exceptions;
using Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Repositories;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Response;
using Gyldendal.Api.CoreData.DataAccess.KoncernData;
using Gyldendal.Api.CoreData.ShopAdmin.Implementation;
using Gyldendal.Api.ShopServices.ApiClient;

namespace Gyldendal.Api.CoreData.Business.Repositories.TradeGDK
{
    public class ProductRepository : BaseWorkProductRepository, IProductRepository
    {
        private readonly Client _shopServicesApiClient;

        private readonly ICoverImageUtil _imageUtil;

        private readonly IConfigurationManager _configurationManager;

        public ProductRepository(koncerndata_webshops_Entities kdEntities, Client shopServicesApiClient, ICoverImageUtil imageUtil, IConfigurationManager configurationManager, IPublizonProducts publizonProducts) : base(kdEntities, publizonProducts)
        {
            _shopServicesApiClient = shopServicesApiClient;
            _imageUtil = imageUtil;
            _configurationManager = configurationManager;
        }

        public Product GetProductByIsbn(string isbn)
        {
            var gdkProducts = KdEntities.DEA_KDWS_GDKProduct
                .Include("DEA_KDWS_GDKproductseries.DEA_KDWS_GDKseries")
                .Include("DEA_KDWS_GDKproductcontributors.DEA_KDWS_GDKContributors")
                .Include("DEA_KDWS_GDKProductThemacode.DEA_KDWS_GDKThemacode")
                .FirstOrDefault(v => v.vare_id.Equals(isbn));

            if (gdkProducts == null)
            {
                throw new NotFoundException($"Product for Isbn: {isbn} not found.");
            }

            var product = gdkProducts.ToCoreDataProduct(_shopServicesApiClient, _imageUtil, _configurationManager, HasFreeSupplementaryMaterial(isbn));
            
            product.HasOtherDiscount = CheckCampaign(isbn, product.MediaType.Name);
            product.HasOtherDiscount = HasOtherDiscounts(product.Isbn13, product.MediaType.Name);
            
            product.IsBuyable = IsProductBuyable(product);

            product.FreeMaterials = GetFreeMaterials(product.Isbn13, product.MediaType);
            
            product.DiscountPercentage = GetProductDiscount(product.Isbn13, product.MediaType.Name);

            return product;
        }

        public Product GetBundleByIsbn(string isbn)
        {
            var kdBundleCategories = (from kdbundle in KdEntities.DEA_KDWS_GDKBundle
                                      where kdbundle.bundle_id.Equals(isbn)
                                      let kdCategories = (from kdcategory in KdEntities.DEA_KDWS_GDKcategory
                                                          join kdbundleCategory in KdEntities.DEA_KDWS_GDK_BundleCategoriesView on kdcategory.id equals kdbundleCategory.kategori
                                                          where kdbundleCategory.Bundle_id.Equals(isbn)
                                                          select kdcategory)
                                      select new { Bundle = kdbundle, Categories = kdCategories }).FirstOrDefault();

            var product = kdBundleCategories?.Bundle?.ToCoreDataProduct(_configurationManager);

            if (product != null)
            {
                product.IsBuyable = IsBundleBuyable(product);
            }

            return product;
        }

        public int GetUpdatedProductsCount(DateTime updatedAfter)
        {
            return KdEntities.DEA_KDWS_GDK_ConsolidatedProdLogView.Count(x => x.LastAction_TimeStamp > updatedAfter);
        }

        public IEnumerable<ProductUpdateInfo> GetProductsUpdateInfo(DateTime updatedAfter, int pageIndex, int pageSize)
        {
            if (pageIndex < 0)
            {
                throw new ArgumentException($"Value for {nameof(pageIndex)} should be greater than or equal to 0.");
            }

            if (pageSize < 1)
            {
                throw new ArgumentException($"Value for {nameof(pageSize)} should be greater than 0.");
            }

            var skip = pageIndex * pageSize;

            return KdEntities.DEA_KDWS_GDK_ConsolidatedProdLogView.Where(x => x.LastAction_TimeStamp > updatedAfter)
                .OrderBy(x => x.Product_Id)
                .Skip(skip)
                .Take(pageSize)
                .ToArray()
                .Select(x => new ProductUpdateInfo
                {
                    ProductId = x.Product_Id,
                    UpdateTime = x.LastAction_TimeStamp,
                    UpdateType = x.LastAction.ToProductUpdateType(),
                    ProductType = x.Product_Type.ToProductType()
                })
                .ToArray();
        }

        public Result<string> GetBundleIdsByIsbn(string isbn, int pageIndex, int pageSize)
        {
            if (pageIndex < 0)
            {
                throw new ArgumentException($"Value for {nameof(pageIndex)} should be greater than or equal to 0.");
            }

            if (pageSize < 1)
            {
                throw new ArgumentException($"Value for {nameof(pageSize)} should be greater than 0.");
            }

            var skip = pageIndex * pageSize;

            var bundleIds = KdEntities.DEA_KDWS_GDKBundle_Products
                .Where(x => x.product.Equals(isbn))
                .Select(y => y.bundle).Distinct();

            return new Result<string>
            {
                CurrentPage = pageIndex,
                PageSize = pageSize,
                TotalResults = bundleIds.Count(),
                Results = bundleIds
                .OrderBy(z => z)
                .Skip(skip)
                .Take(pageSize)
                .ToList(),
            };
        }

        public IEnumerable<string> GetCampaignProducts(DateTime updatedAfter, int pageIndex, int pageSize)
        {
            var campaigns = GetCampaignProducts(updatedAfter);

            return campaigns.OrderBy(x => x)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToArray()
                .Select(x => x);
        }

        public int GetCampaignProductsCount(DateTime updatedAfter)
        {
            var campaigns = GetCampaignProducts(updatedAfter);

            return campaigns.Count();
        }

        public bool HasActiveCampaign(string productId)
        {
            var campaigns = ActiveCampaigns();

            var campaignProds =
                GroupCampaignProds(campaigns)
                    .Union(OtherCampaignProds(campaigns))
                    .Distinct();

            return campaignProds.Any(x => x.Equals(productId));
        }

        public Dictionary<string, List<WebShop>> GetProductWebshops(List<string> isbns)
        {
            throw new NotImplementedException();
        }

        private IQueryable<string> OtherCampaignProds(IQueryable<Campaign> campaigns)
        {
            return campaigns.Join(KdEntities.DEA_KDWS_GDKProduct, cmp => cmp.MediaType, prod => prod.medietype,
                (cmp, prod) => new { cmp, prod })
                .Where(x => x.cmp.CampaignType == 3)
                .Select(x => x.prod.vare_id);
        }

        private IQueryable<string> GroupCampaignProds(IQueryable<Campaign> campaigns)
        {
            return campaigns.Join(KdEntities.CampaignItem, c => c.Id, ci => ci.CampaignId,
                (c, ci) => new { c, ci })
                .Join(KdEntities.DEA_KDWS_GDKProduct, cmp => cmp.ci.VareId, prod => prod.vare_id,
                    (cmp, prod) => new { cmp, prod })
                .Where(x => (x.cmp.c.CampaignType == 1 || x.cmp.c.CampaignType == 4))
                .Select(x => x.prod.vare_id);
        }

        /// <summary>
        /// Base query for getting campaign affected products
        /// </summary>
        /// <param name="updatedAfter">Start date of campaign</param>
        /// <returns>IQueryable string</returns>
        private IQueryable<string> GetCampaignProducts(DateTime updatedAfter)
        {
            //var campaigns = ActiveCampaigns(updatedAfter);
            var campaigns = GetModifiedCampaigns(updatedAfter);

            // Get Single/Group campaigns
            var campaignProds = GroupCampaignProds(campaigns);

            // Get mediatype campaigns
            var otherCampaignProds = OtherCampaignProds(campaigns);

            return campaignProds.Union(otherCampaignProds).Distinct();
        }
    }
}