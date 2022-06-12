using System.Collections.Generic;
using System.Linq;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.DataAccess.KoncernData;

namespace Gyldendal.Api.CoreData.Business.Porter.Services.DataProviders.Product
{
    public class GPlusProductServiceDataProvider : BaseProductServiceDataProvider
    {
        public GPlusProductServiceDataProvider(koncerndata_webshops_Entities kdEntities) : base(kdEntities)
        {
        }

        protected override void PopulateAdditionalPropertiesFor_GetProductByIsbn(Contracts.Models.Product product)
        {
            product.FreeMaterials = GetFreeMaterials(product);
        }

        private List<ProductFreeMaterial> GetFreeMaterials(Contracts.Models.Product product)
        {
            var freeMaterials = new List<ProductFreeMaterial>();

            var attachments = KdEntities.DEA_KDWS_GPlusattachments.Where(a => a.vare.Equals(product.Isbn13) && a.is_secured == false); //  && a.kd_slettet == 0, as only not deleted attachments are in this table
            foreach (var attachment in attachments)
            {
                if (string.IsNullOrEmpty(attachment.sampleURL)) continue;

                var index = attachment.sampleURL.LastIndexOf('/');
                if (index >= 0)
                {
                    var freeMaterial = GetProductFreeMaterial(attachment.sampleURL.Substring(index + 1), attachment.beskrivelse, publizonIdentifier: null);
                    freeMaterials.Add(freeMaterial);
                }
            }
            
            return freeMaterials;
        }
    }
}
