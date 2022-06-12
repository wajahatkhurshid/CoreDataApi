using System;
using System.Collections.Generic;
using System.Linq;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Business.Porter.Models;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.DataAccess.KoncernData;

namespace Gyldendal.Api.CoreData.Business.Porter.Services.DataProviders.Product
{
    public class GuProductServiceDataProvider : BaseProductServiceDataProvider
    {
        public GuProductServiceDataProvider(koncerndata_webshops_Entities kdEntities) : base(kdEntities)
        {
        }

        protected override void PopulateAdditionalPropertiesFor_GetProductByIsbn(Contracts.Models.Product product)
        {
            product.IsBuyable = IsProductBuyable(isPublished: false, product);
            product.FreeMaterials = GetFreeMaterials(product);
        }

        private List<ProductFreeMaterial> GetFreeMaterials(Contracts.Models.Product product)
        {
            var attachments = GetWebShopAttachments(product);
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
                    attachments = KdEntities.DEA_KDWS_GUattachments
                        .Where(a => a.vare.Equals(product.Isbn13) && a.is_secured == false)
                        .Select(x => new Attachment { SampleUrl = x.sampleURL, Beskrivelse = x.beskrivelse }).ToList();
                    break;

                case WebShop.HansReitzel:
                    attachments = KdEntities.DEA_KDWS_HRattachments
                        .Where(a => a.vare.Equals(product.Isbn13) && a.is_secured == false)
                        .Select(x => new Attachment { SampleUrl = x.sampleURL, Beskrivelse = x.beskrivelse }).ToList();
                    break;

                case WebShop.MunksGaard:
                    attachments = KdEntities.DEA_KDWS_MUNKattachments
                        .Where(a => a.vare.Equals(product.Isbn13) && a.is_secured == false)
                        .Select(x => new Attachment { SampleUrl = x.sampleURL, Beskrivelse = x.beskrivelse }).ToList();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return attachments;
        }
    }
}
