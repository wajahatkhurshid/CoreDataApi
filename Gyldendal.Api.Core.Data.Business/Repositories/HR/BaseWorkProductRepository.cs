using Gyldendal.Api.CoreData.Business.Repositories.Common;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Models.Bundle;
using Gyldendal.Api.CoreData.DataAccess.KoncernData;
using System.Collections.Generic;
using System.Linq;

namespace Gyldendal.Api.CoreData.Business.Repositories.HR
{
    public class BaseWorkProductRepository : BaseRepository
    {
        /// <summary>
        /// Creates a new instance of BaseWorkProductRepository.
        /// </summary>
        /// <param name="kdEntities"></param>
        protected BaseWorkProductRepository(koncerndata_webshops_Entities kdEntities) :
            base(DataScope.HansReitzelShop, kdEntities)
        {
        }

        /// <summary>
        /// Fetches list of free materials from under lying repository
        /// </summary>
        /// <param name="productId">Product id whose free material has to be returned</param>
        /// <returns>Free material information of the product from under lying system</returns>
        /// <remarks>
        /// Bug Id 10888 proposed solution point 3
        /// Fix Free Material Population of Product Methods
        /// </remarks>
        protected List<ProductFreeMaterial> GetFreeMaterials(string productId)
        {
            var freeMaterials = new List<ProductFreeMaterial>();

            var attachments =
                KdEntities.DEA_KDWS_HRattachments.Where(a =>
                    a.vare.Equals(productId) &&
                    a.is_secured == false); //  && a.kd_slettet == 0, as only not deleted attachments are in this table
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

        /// <summary>
        /// Evaluates whether product is buyable or not
        /// </summary>
        /// <returns>True if product is buable otherwise false</returns>
        /// <remarks>Bug Id 14107 fixed</remarks>
        /// <remarks>Bug Id 16399 fixed</remarks>
        public bool IsProductBuyable(Product product)
        {
            return product.IsPhysical ? (product.InStock || product.IsNextPrintPlanned) :
                (product.SalesConfiguration != null
                 && product.SalesConfiguration.AccessForms.Count > 0
                 && product.SalesConfiguration.SalesForms.Count > 0);
        }

        public bool IsBundleBuyable(Product bundle)
        {
            bundle.BundleProducts.ForEach(x => x.IsBuyable = IsProductBuyable(x));
            return bundle.BundleProducts.All(x => x.IsBuyable);
        }

        public bool IsProductBuyable(BundleProduct bundleProduct)
        {
            return bundleProduct.IsPhysical ? (bundleProduct.InStock || bundleProduct.IsNextPrintRunPlanned) :
                (bundleProduct.SalesConfiguration != null
                 && bundleProduct.SalesConfiguration.AccessForms.Count > 0
                 && bundleProduct.SalesConfiguration.SalesForms.Count > 0);
        }
    }
}