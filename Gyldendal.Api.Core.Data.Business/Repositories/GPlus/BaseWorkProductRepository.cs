using Gyldendal.Api.CoreData.Business.Repositories.Common;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.DataAccess.KoncernData;
using System.Collections.Generic;
using System.Linq;

namespace Gyldendal.Api.CoreData.Business.Repositories.GPlus
{
    public class BaseWorkProductRepository : BaseRepository
    {
        /// <summary>
        /// Constructor of Bogklub Product
        /// </summary>
        /// <param name="kdEntities"></param>
        protected BaseWorkProductRepository(koncerndata_webshops_Entities kdEntities) :
            base(DataScope.GyldendalPlus, kdEntities)
        {
        }

        /// <summary>
        /// Fetches list of free materials from under lying repository
        /// </summary>
        /// <param name="productId">Product id whose free material has to be returned</param>
        /// <returns>Free material information of the product from under lying system</returns>
        /// <remarks>
        /// </remarks>
        protected List<ProductFreeMaterial> GetFreeMaterials(string productId)
        {
            var freeMaterials = new List<ProductFreeMaterial>();

            var attachments = KdEntities.DEA_KDWS_GPlusattachments.Where(a => a.vare.Equals(productId) && a.is_secured == false); //  && a.kd_slettet == 0, as only not deleted attachments are in this table
            foreach (var attachment in attachments)
            {
                if (string.IsNullOrEmpty(attachment.sampleURL)) continue;

                var index = attachment.sampleURL.LastIndexOf('/');
                if (index >= 0)
                {
                    freeMaterials.Add(
                        new ProductFreeMaterial
                        {
                            FileName = attachment.sampleURL.Substring(index + 1),
                            Description = attachment.beskrivelse
                        }
                    );
                }
            }

            return freeMaterials;
        }
    }
}