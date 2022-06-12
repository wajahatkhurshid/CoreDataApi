using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Business.ExtensionMethods;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.DataAccess.KoncernData;
using System;
using System.Linq;

namespace Gyldendal.Api.CoreData.Business.Repositories.Common
{
    public class BaseRepository
    {
        public WebShop WebShop => DataScope.ToWebShop();

        public DataScope DataScope { get; }

        protected readonly koncerndata_webshops_Entities KdEntities;

        protected BaseRepository(DataScope dataScope, koncerndata_webshops_Entities kdEntities)
        {
            DataScope = dataScope;
            KdEntities = kdEntities;
        }

        /// <summary>
        /// True if product has free supplementary material
        /// </summary>
        /// <param name="isbn"></param>
        /// <returns></returns>
        /// <remarks>
        /// Bug Id 10888 proposed solution point 3
        /// Fix Free Material Population of Product Methods
        /// </remarks>
        protected bool HasFreeSupplementaryMaterial(string isbn)
        {
            return KdEntities.supplerende_materialer.Any(s =>
                s.vare.Equals(isbn) && s.is_secured == false && s.kd_slettet == 0);
        }

        /// <summary>
        /// Base method to get all active campaigns source
        /// </summary>
        /// <returns>IQueryable Campaigns</returns>
        protected IQueryable<Campaign> ActiveCampaigns()
        {
            var currentDate = DateTime.Now;

            return KdEntities.Campaign.Where(x => x.IsActive
                                                  && x.StartDate <= currentDate
                                                  && (x.EndDate == null || x.EndDate >= currentDate));
        }

        protected IQueryable<Campaign> GetModifiedCampaigns(DateTime updatedAfter)
        {
            var currentDate = DateTime.Now;

            var campaigns = KdEntities.Campaign.Where(x => (x.CreatedAt >= updatedAfter && x.CreatedAt <= currentDate)
                                                           || (x.ModifiedAt >= updatedAfter && x.ModifiedAt <= currentDate)
                                                           || (x.StartDate >= updatedAfter && x.StartDate <= currentDate)
                                                           || (x.EndDate >= updatedAfter && x.EndDate <= currentDate));

            return campaigns;
        }

        /// <summary>
        /// Checks whether product has other discount then 1 global discount
        /// </summary>
        /// <param name="isbn"></param>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        protected bool HasOtherDiscounts(string isbn, string mediaType)
        {
            var currentDate = DateTime.Now;
            var website = WebShop.KdWebshopName() ?? "";

            var globalDiscountCampaignId = GlobalDiscountQuery(isbn, mediaType, website, currentDate).
                Select(y => y.Id).FirstOrDefault();

            return DiscountQuery(isbn, mediaType, website, currentDate)
                .Any(x => x.Id != globalDiscountCampaignId);
        }

        /// <summary>
        /// Gets dicount percentage applicable on product based upon maximun discount percentage as per global campaigns
        /// </summary>
        /// <param name="isbn13">ISBN13</param>
        /// <param name="mediaType">MediaType</param>
        /// <returns>Discount Percentage as decimal</returns>
        protected decimal? GetProductDiscount(string isbn13, string mediaType)
        {
            var currentDate = DateTime.Now;
            var website = WebShop.KdWebshopName() ?? "";

            return GlobalDiscountQuery(isbn13, mediaType, website, currentDate).FirstOrDefault()
                                              ?.DiscountPercentage ?? 0m;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="isbn13"></param>
        /// <param name="mediaType"></param>
        /// <param name="website"></param>
        /// <param name="valiDateTime"></param>
        /// <returns></returns>
        private IQueryable<Campaign> GlobalDiscountQuery(string isbn13, string mediaType, string website, DateTime valiDateTime)
        {
            return DiscountQuery(isbn13, mediaType, website, valiDateTime).Where(a =>
                (a.CouponCode == null || a.CouponCode == "")
                &&
                a.CampaignMembership.Any() == false
                &&
                a.MaxQuantityAllowed == null
            );
        }

        private IQueryable<Campaign> DiscountQuery(string isbn13, string mediaType, string website, DateTime valiDateTime)
        {
            return KdEntities.Campaign.OrderByDescending(a => a.DiscountPercentage).Where(

                a => (
                    (
                        (a.CampaignItem.Any(x => x.VareId.Equals(isbn13)) &&
                         (a.CampaignType == 1 || a.CampaignType == 4))
                        ||
                        (a.MediaType == mediaType && (a.CampaignType == 3))
                    )
                    &&
                    a.StartDate <= valiDateTime
                    &&
                    (a.EndDate == null || a.EndDate >= valiDateTime)
                    &&
                    a.IsActive
                    &&
                    (website == "" || a.ShopName == website)
                )
            );
        }

        protected void ValidatePagination(int pageIndex, int pageSize)
        {
            if (pageIndex < 0)
            {
                throw new ArgumentException($"Value for {nameof(pageIndex)} should be greater than or equal to 0.");
            }

            if (pageSize < 1)
            {
                throw new ArgumentException($"Value for {nameof(pageSize)} should be greater than 0.");
            }
        }

        /// <summary>
        /// Creates and returns a standard NotImplementedException instance.
        /// </summary>
        /// <returns></returns>
        protected static NotImplementedException GetNotImplementedException()
        {
            return new NotImplementedException("The call is not implemented for Web Shop: None");
        }
    }
}