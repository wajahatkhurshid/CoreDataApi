using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Business.ExtensionMethods;
using Gyldendal.Api.CoreData.Business.Repositories.Common;
using Gyldendal.Api.CoreData.Business.Util;
using Gyldendal.Api.CoreData.Common.ConfigurationManager;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Response;
using Gyldendal.Api.CoreData.DataAccess.KoncernData;
using Gyldendal.Api.CoreData.ShopAdmin.Implementation;
using Gyldendal.PulsenServices.Api.Contracts.Common;
using Gyldendal.PulsenServices.ApiClient;
using PulsenProductNotFoundReason = Gyldendal.PulsenServices.Api.Contracts.Product.ProductNotFoundReason;
using PorterApi = Gyldendal.Api.CoreData.Services.PorterApiClient;
using Gyldendal.Api.CoreData.Business.Porter.Mapping;

namespace Gyldendal.Api.CoreData.Business.Porter.Services.DataProviders.Work
{
    public class GdkWorkServiceDataProvider : BaseWorkServiceDataProvider
    {
        private readonly IPublizonProducts _publizonProducts;
        private readonly PulsenServiceApiClient _pulsenServiceApiClient;

        public GdkWorkServiceDataProvider(koncerndata_webshops_Entities kdEntities, IPublizonProducts publizonProducts, PulsenServiceApiClient pulsenServiceApiClient, ShopServices.ApiClient.Client shopServicesApiClient, ICoverImageUtil imageUtil, IConfigurationManager configManager) : 
            base(kdEntities, shopServicesApiClient, imageUtil, configManager)
        {
            _publizonProducts = publizonProducts;
            _pulsenServiceApiClient = pulsenServiceApiClient;
        }

        protected override GetProductDetailsResponse GetWorkForSingleProduct(Contracts.Models.Work work, string isbn)
        {
            var clonedWork = work;
            var website = clonedWork.WebShop.KdWebshopName() ?? "";

            work.Products.ForEach(
                product =>
                {
                    product.HasOtherDiscount = HasOtherDiscounts(product.Isbn13, product.MediaType.Name, website);
                    if (product.HasOtherDiscount)
                        product.DiscountPercentage = GetProductDiscount(product.Isbn13, product.MediaType.Name, website);
                    product.FreeMaterials = GetFreeMaterials(product.Isbn13, product.MediaType);
                    product.IsBuyable = IsProductBuyable(product);
                });

            var pulsenProdDetailTask = Task.Factory.StartNew(() => _pulsenServiceApiClient.Product.GetProductDetails(isbn));
            pulsenProdDetailTask.Wait();

            var pulsenProdDetail = pulsenProdDetailTask.Result;

            if (pulsenProdDetail.Product == null || !pulsenProdDetail.Product.ClubIds.Contains(Clubs.GyldendalDk))
            {
                return new GetProductDetailsResponse
                {
                    Message = pulsenProdDetail.Message,
                    ProductNotFoundReason = GetProductNotFoundReason(pulsenProdDetail.ProductNotFoundReason)
                };
            }

            if (pulsenProdDetail.Product.ProductType == PulsenServices.Api.Contracts.Common.ProductType.BundleProduct && string.IsNullOrWhiteSpace(pulsenProdDetail.Product.Description))
            {
                pulsenProdDetail.Product.Description = GetBundleProductDescription(isbn);
            }

            work = ModelsMapping.GetClubWorkForTrade(work, pulsenProdDetail.Product, Clubs.GyldendalDk);

            return new GetProductDetailsResponse
            {
                Message = "",
                ProductNotFoundReason = ProductNotFoundReason.None,
                ProductWork = work
            };
        }

        protected override Contracts.Models.Work GetWorkForBundleProducts(string bundleId,
            List<PorterApi.Product> porterProducts, WebShop webshop)
        {
            return null;
        }

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

        private ProductNotFoundReason GetProductNotFoundReason(PulsenServices.Api.Contracts.Product.ProductNotFoundReason productNotFoundReason)
        {
            var reason = productNotFoundReason switch
            {
                PulsenProductNotFoundReason.ProductOutOfStock => ProductNotFoundReason.ProductOutOfStock,
                PulsenProductNotFoundReason.DataImportInProgress => ProductNotFoundReason.DataImportInProgress,
                PulsenProductNotFoundReason.ProductIsDeleted => ProductNotFoundReason.ProductIsDeleted,
                _ => ProductNotFoundReason.SupplementaryDataNotFound
            };

            return reason;
        }

        private string GetBundleProductDescription(string isbn)
        {
            return _KdEntities.DEA_KDWS_GDKProduct.FirstOrDefault(a => a.ISBN13.Equals(isbn))?.langbeskrivelse;
        }
    }
}
