using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Business.Repositories.Common;
using Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Repositories;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Models.Bundle;
using Gyldendal.Api.CoreData.Contracts.Response;
using Gyldendal.Api.CoreData.DataAccess.KoncernData;
using NewRelic.Api.Agent;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gyldendal.Api.CoreData.Business.Repositories.WsNone
{
    public class ProductRepository : BaseWorkProductRepository, IProductRepository
    {
        /// <summary>
        /// Creates a new instance of ProductRepository.
        /// </summary>
        /// <param name="kdEntities"></param>
        public ProductRepository(koncerndata_webshops_Entities kdEntities) :
            base(kdEntities)
        {
        }

        /// <summary>
        /// Get GU Product details By isbn
        /// </summary>
        /// <param name="isbn"></param>
        /// <returns></returns>
        public Product GetProductByIsbn(string isbn)
        {
            throw GetNotImplementedException();
        }

        /// <summary>
        /// Get GU Bundle details By isbn
        /// </summary>
        /// <param name="isbn"></param>
        /// <returns></returns>
        public Product GetBundleByIsbn(string isbn)
        {
            throw GetNotImplementedException();
        }

        /// <summary>
        /// Returns the number of products updated after the given DateTime value.
        /// </summary>
        /// <param name="updatedAfter"></param>
        /// <returns></returns>
        public int GetUpdatedProductsCount(DateTime updatedAfter)
        {
            return KdEntities.DEA_KDWS_Ekey_ConsolidatedProdLogView.Count(x => x.LastAction_TimeStamp > updatedAfter);
        }

        /// <summary>
        /// Returns the asked page of ProductupdatedInfo objects, for each product, that was updated after the given DateTime, in KoncernDataWebShops database.
        /// </summary>
        /// <param name="updatedAfter"></param>
        /// <param name="pageIndex">Minimum value 0.</param>
        /// <param name="pageSize">Minimum value 1.</param>
        /// <exception cref="ArgumentException">If pageNo is less than zero.</exception>
        /// <exception cref="ArgumentException">If pageSize is less than one.</exception>
        /// <returns></returns>
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

            return
            KdEntities.DEA_KDWS_Ekey_ConsolidatedProdLogView.Where(x => x.LastAction_TimeStamp > updatedAfter)
                .OrderBy(x => x.Product_Id)
                .Skip(skip)
                .Take(pageSize)
                .ToArray()
                .Select(x => new ProductUpdateInfo
                {
                    ProductId = x.Product_Id.ToString(),
                    UpdateTime = x.LastAction_TimeStamp,
                    UpdateType = x.LastAction.ToProductUpdateType(),
                    ProductType = Contracts.Enumerations.ProductType.SingleProduct
                })
                .ToArray();
        }

        /// <summary>
        /// Gets count of the products for which campaigns exist
        /// </summary>
        /// <param name="updatedAfter">Start date of campaign</param>
        /// <returns>Count of campaign affected products</returns>
        public int GetCampaignProductsCount(DateTime updatedAfter)
        {
            var campaigns = GetCampaignProducts(updatedAfter);

            return campaigns.Count();
        }

        /// <summary>
        /// Base query for getting campaign affected products
        /// </summary>
        /// <param name="updatedAfter">Start date of campaign</param>
        /// <returns>IQueryable string (Product Ids)</returns>
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

        private IQueryable<string> OtherCampaignProds(IQueryable<Campaign> campaigns)
        {
            return campaigns.Join(KdEntities.DEA_KDWS_Ekeyproduct, cmp => cmp.MediaType, prod => prod.medietype,
                (cmp, prod) => new { cmp, prod })
                .Where(x => x.cmp.CampaignType == 3)
                .Select(x => x.prod.vare_id);
        }

        private IQueryable<string> GroupCampaignProds(IQueryable<Campaign> campaigns)
        {
            return campaigns.Join(KdEntities.CampaignItem, c => c.Id, ci => ci.CampaignId,
                (c, ci) => new { c, ci })
                .Join(KdEntities.DEA_KDWS_Ekeyproduct, cmp => cmp.ci.VareId, prod => prod.vare_id,
                    (cmp, prod) => new { cmp, prod })
                .Where(x => (x.cmp.c.CampaignType == 4))
                .Select(x => x.prod.vare_id);
        }

        /// <summary>
        /// Checks whether a product has any active campaign
        /// </summary>
        /// <param name="productId">Product ID / ISBN</param>
        /// <returns>Returns whether a product has one or more active campaigns</returns>
        public bool HasActiveCampaign(string productId)
        {
            throw GetNotImplementedException();
        }

        /// <summary>
        /// Gets product ids of the products for which campaigns exist
        /// </summary>
        /// <param name="updatedAfter">Start date of campaign</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns>List of string containing product ids</returns>
        public IEnumerable<string> GetCampaignProducts(DateTime updatedAfter, int pageIndex, int pageSize)
        {
            var campaigns = GetCampaignProducts(updatedAfter);

            return campaigns.OrderBy(x => x)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToArray()
                .Select(x => x);
        }

        /// <summary>
        /// Returns the bundle's Id List which have this Isbn
        /// </summary>
        /// <param name="isbn"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Result<string> GetBundleIdsByIsbn(string isbn, int pageIndex, int pageSize)
        {
            throw GetNotImplementedException();
        }

        public bool IsProductBuyable(Product product)
        {
            return false;
        }

        [Trace]
        public bool IsProductBuyable(BundleProduct bundleProduct)
        {
            return false;
        }

        public Dictionary<string, List<WebShop>> GetProductWebshops(List<string> isbns)
        {
            throw new NotImplementedException();
        }
    }
}