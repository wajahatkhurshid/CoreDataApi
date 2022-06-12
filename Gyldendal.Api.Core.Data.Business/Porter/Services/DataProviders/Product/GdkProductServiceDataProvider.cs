using System.Collections.Generic;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.DataAccess.KoncernData;
using Gyldendal.Api.CoreData.ShopAdmin.Implementation;

namespace Gyldendal.Api.CoreData.Business.Porter.Services.DataProviders.Product
{
    public class GdkProductServiceDataProvider : BaseProductServiceDataProvider
    {
        private readonly IPublizonProducts _publizonProducts;

        public GdkProductServiceDataProvider(koncerndata_webshops_Entities kdEntities, IPublizonProducts publizonProducts) : base(kdEntities)
        {
            _publizonProducts = publizonProducts;
        }

        protected override void PopulateAdditionalPropertiesFor_GetProductByIsbn(Contracts.Models.Product product)
        {
            product.IsBuyable = IsProductBuyable(isPublished: true, product);
            product.FreeMaterials = GetFreeMaterials(product);
        }

        private List<ProductFreeMaterial> GetFreeMaterials(Contracts.Models.Product product)
        {
            if (!product.MediaType.Name.ToLower().Contains("e-bog") && !product.MediaType.Name.ToLower().Equals("lydfiler"))
            {
                return new List<ProductFreeMaterial>();
            }

            var freeMaterials = new List<ProductFreeMaterial>();

            var attachments = _publizonProducts.GetPublizonProductDetails(product.Isbn13);
            foreach (var attachment in attachments)
            {
                if (string.IsNullOrEmpty(attachment.SampleURL)) continue;

                var freeMaterial = GetProductFreeMaterial(attachment.SampleURL, attachment.LastUpdated.ToString("O"), attachment.PublizonIdentifier);
                freeMaterials.Add(freeMaterial);
            }

            return freeMaterials;
        }
    }
}
