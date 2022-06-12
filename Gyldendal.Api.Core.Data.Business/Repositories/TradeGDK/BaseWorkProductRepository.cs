using System;
using System.Collections.Generic;
using System.Linq;
using Gyldendal.Api.CoreData.Business.Repositories.Common;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Models.Bundle;
using Gyldendal.Api.CoreData.DataAccess.KoncernData;
using Gyldendal.Api.CoreData.ShopAdmin.Implementation;
using Product = Gyldendal.Api.CoreData.Contracts.Models.Product;
using PulsenProduct = Gyldendal.PulsenServices.Api.Contracts.Product.Product;
using PulsenProductNotFoundReason = Gyldendal.PulsenServices.Api.Contracts.Product.ProductNotFoundReason;

namespace Gyldendal.Api.CoreData.Business.Repositories.TradeGDK
{
    public class BaseWorkProductRepository : BaseRepository
    {
        private readonly IPublizonProducts _publizonProducts;

        public BaseWorkProductRepository(koncerndata_webshops_Entities kdEntities, IPublizonProducts publizonProducts) :
            base(DataScope.TradeGyldendalDk, kdEntities)
        {
            _publizonProducts = publizonProducts;
        }

        public bool CheckCampaign(string isbn, string mediaType)
        {
            var currentDate = DateTime.Now;
            var firstOrDefault = KdEntities.Campaign.FirstOrDefault(a =>
                (a.CampaignItem.Any(x => x.VareId.Equals(isbn) && (a.CampaignType == 1 || a.CampaignType == 4)) ||
                 a.MediaType == mediaType) && a.StartDate <= currentDate &&
                (a.EndDate == null || a.EndDate >= currentDate) && a.IsActive);
            return firstOrDefault != null;
        }

        /// <summary>
        /// Fetches list of free materials from under lying repository
        /// </summary>
        /// <param name="productId">Product id whose free material has to be returned</param>
        /// <param name="productMediaType"></param>
        /// <returns>Free material information of the product from under lying system</returns>
        /// <remarks>
        /// Bug Id 10888 proposed solution point 3
        /// Fix Free Material Population of Product Methods
        /// </remarks>
        protected List<ProductFreeMaterial> GetFreeMaterials(string productId, MediaType productMediaType)
        {
            if (!productMediaType.Name.ToLower().Contains("e-bog") && !productMediaType.Name.ToLower().Equals("lydfiler"))
                return new List<ProductFreeMaterial>();
            var freeMaterials = new List<ProductFreeMaterial>();
            var freeMatirial = new ProductFreeMaterial();

            var attachments = _publizonProducts.GetPublizonProductDetails(productId);
            foreach (var attachment in attachments)
            {
                if (string.IsNullOrEmpty(attachment.SampleURL)) continue;

                freeMatirial.FileName = attachment.SampleURL;
                freeMatirial.Description = attachment.LastUpdated.ToString("O");
                freeMatirial.PublizonIdentifier = attachment.PublizonIdentifier;
                freeMaterials.Add(freeMatirial);
            }
            return freeMaterials;
        }

        /// <summary>
        /// Evaluates whether product is buyable or not
        /// </summary>
        /// <returns>True if product is buable otherwise false</returns>
        /// <remarks>Bug Id 14107 fixed</remarks>
        public bool IsProductBuyable(Product product)
        {
            return product.IsPhysical ?
                product.IsPublished && (product.InStock || product.IsNextPrintPlanned) :
                product.IsPublished && (product.SalesConfiguration != null
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
            return bundleProduct.IsPhysical ?
                bundleProduct.IsPublished && (bundleProduct.InStock || bundleProduct.IsNextPrintRunPlanned) :
                bundleProduct.IsPublished && (bundleProduct.SalesConfiguration != null
                                              && bundleProduct.SalesConfiguration.AccessForms.Count > 0
                                              && bundleProduct.SalesConfiguration.SalesForms.Count > 0);
        }
    }
}