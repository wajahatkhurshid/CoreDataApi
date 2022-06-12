using System;
using System.Collections.Generic;
using System.Linq;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Business.ExtensionMethods;
using Gyldendal.Api.CoreData.Business.Porter.Models;
using Gyldendal.Api.CoreData.Business.Util;
using Gyldendal.Api.CoreData.Common.ConfigurationManager;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.DataAccess.KoncernData;
using Gyldendal.Api.CoreData.Contracts.Response;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using PorterApi = Gyldendal.Api.CoreData.Services.PorterApiClient;
using Gyldendal.Api.CoreData.Business.Porter.Mapping;

namespace Gyldendal.Api.CoreData.Business.Porter.Services.DataProviders.Work
{
    public class GuWorkServiceDataProvider : BaseWorkServiceDataProvider
    {
        public GuWorkServiceDataProvider(koncerndata_webshops_Entities kdEntities, ShopServices.ApiClient.Client shopServicesApiClient, ICoverImageUtil imageUtil, IConfigurationManager configManager) 
            : base(kdEntities, shopServicesApiClient, imageUtil, configManager)
        {

        }

        protected override GetProductDetailsResponse GetWorkForSingleProduct(Contracts.Models.Work work, string isbn)
        {
            var website = work.WebShop.KdWebshopName() ?? "";

            work.Products.ForEach(product =>
            {
                product.HasOtherDiscount = HasOtherDiscounts(product.Isbn13, product.MediaType.Name, website);
                product.DiscountPercentage = GetProductDiscount(product.Isbn13, product.MediaType.Name, website);
                product.FreeMaterials = GetFreeMaterials(product);
                product.IsBuyable = IsProductBuyable(product);
            });

            return new GetProductDetailsResponse
            {
                Message = "",
                ProductNotFoundReason = ProductNotFoundReason.None,
                ProductWork = work
            };
        }

        protected override Contracts.Models.Work GetWorkForBundleProducts(string bundleId, List<PorterApi.Product> porterProducts, WebShop webshop)
        {
            var bundleProduct = _KdEntities.varer.Where(b => b.id.Equals(bundleId)).FirstOrDefault();

            if (bundleProduct == null)
                return new Contracts.Models.Work();

            var siteCatogories = _KdEntities.portersitekategorier.Where(c => c.Varer_id == bundleId).ToArray();

            var work = bundleProduct.ToCoreDataWork(porterProducts, siteCatogories, _shopServicesApiClient, _imageUtil, _configManager, webshop);

            work.Products[0].LastUpdated = GetCampaignLastModifiedAt(bundleId).Value;
            work.Products[0].MembershipPaths = GetMembershipPaths(bundleId);

            return work;
        }

        private List<ProductFreeMaterial> GetFreeMaterials(Contracts.Models.Product product)
        {
           // var attachments = GetWebShopAttachments(product);
           var attachments = new List<Attachment>();
           var freeMaterials = new List<ProductFreeMaterial>();

            foreach (var attachment in attachments)
            {
                if (string.IsNullOrEmpty(attachment.SampleUrl))
                {
                    continue;
                }

                var index = attachment.SampleUrl.LastIndexOf('/');
                if (index < 0)
                {
                    continue;
                }

                var freeMaterial = GetProductFreeMaterial(attachment.SampleUrl.Substring(index + 1), attachment.Beskrivelse, publizonIdentifier: null);
                freeMaterials.Add(freeMaterial);
            }

            return freeMaterials;
        }

        private IEnumerable<Attachment> GetWebShopAttachments(Contracts.Models.Product product)
        {
            List<Attachment> attachments;

            switch (product.WebShop)
            {
                case WebShop.Gu:
                    attachments = _KdEntities.DEA_KDWS_GUattachments
                        .Where(a => a.vare.Equals(product.Isbn13) && a.is_secured == false)
                        .Select(x => new Attachment { SampleUrl = x.sampleURL, Beskrivelse = x.beskrivelse }).ToList();
                    break;

                case WebShop.HansReitzel:
                    attachments = _KdEntities.DEA_KDWS_HRattachments
                        .Where(a => a.vare.Equals(product.Isbn13) && a.is_secured == false)
                        .Select(x => new Attachment { SampleUrl = x.sampleURL, Beskrivelse = x.beskrivelse }).ToList();
                    break;

                case WebShop.MunksGaard:
                    attachments = _KdEntities.DEA_KDWS_MUNKattachments
                        .Where(a => a.vare.Equals(product.Isbn13) && a.is_secured == false)
                        .Select(x => new Attachment { SampleUrl = x.sampleURL, Beskrivelse = x.beskrivelse }).ToList();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return attachments;
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
        private DateTime? GetCampaignLastModifiedAt(string bundleId)
        {
            return (from campaign in _KdEntities.Campaign
                join campaignItem in _KdEntities.CampaignItem on campaign.Id equals campaignItem.CampaignId
                where campaignItem.VareId == bundleId
                select campaign.ModifiedAt).FirstOrDefault();
        }
        private List<string> GetMembershipPaths(string bundleId)
        {
            return (from campaignMembership in _KdEntities.CampaignMembership
                join campaignItem in _KdEntities.CampaignItem on campaignMembership.CampaignId equals campaignItem.CampaignId
                where campaignItem.VareId == bundleId
                select campaignMembership.MembershipPath).ToList();
        }
    }
}
