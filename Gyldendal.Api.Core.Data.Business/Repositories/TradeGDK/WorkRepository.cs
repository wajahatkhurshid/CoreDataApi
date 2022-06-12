using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Gyldendal.Api.CoreData.Business.Util;
using Gyldendal.Api.CoreData.Common.ConfigurationManager;
using Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Repositories;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Response;
using Gyldendal.Api.CoreData.DataAccess.KoncernData;
using Gyldendal.Api.CoreData.ShopAdmin.Implementation;
using Gyldendal.Api.ShopServices.ApiClient;
using Gyldendal.PulsenServices.Api.Contracts.Common;
using Gyldendal.PulsenServices.ApiClient;
using Constants = Gyldendal.Api.CoreData.Business.Repositories.Common.Constants;
using ProductType = Gyldendal.Api.CoreData.Contracts.Enumerations.ProductType;
using PulsenProduct = Gyldendal.PulsenServices.Api.Contracts.Product.Product;
using PulsenProductNotFoundReason = Gyldendal.PulsenServices.Api.Contracts.Product.ProductNotFoundReason;

namespace Gyldendal.Api.CoreData.Business.Repositories.TradeGDK
{
    public class WorkRepository : BaseWorkProductRepository, IWorkRepository
    {
        private readonly Client _shopServicesApiClient;

        private readonly ICoverImageUtil _imageUtil;

        private readonly IConfigurationManager _configurationManager;

        private readonly PulsenServiceApiClient _pulsenServiceApiClient;

        public WorkRepository(koncerndata_webshops_Entities kdEntities, Client shopServicesApiClient, ICoverImageUtil imageUtil, IConfigurationManager configurationManager, IPublizonProducts publizonProducts, PulsenServiceApiClient pulsenServiceApiClient) :
            base(kdEntities, publizonProducts)
        {
            _shopServicesApiClient = shopServicesApiClient;
            _imageUtil = imageUtil;
            _configurationManager = configurationManager;
            _pulsenServiceApiClient = pulsenServiceApiClient;
        }

        public List<string> GetDeletedWorks(DateTime? fromDate)
        {
            if (!fromDate.HasValue)
            {
                fromDate = DateTime.MinValue;
            }

            return KdEntities.DEA_KDWS_GDKWorkLog
                .Where(w => w.Action.Equals(Constants.DeleteAction) && w.CreatedDate >= fromDate)
                .Select(w => w.work_id.ToString())
                .ToList();
        }

        public GetProductDetailsResponse GetWorkByProductId(string id, ProductType productType)
        {
            switch (productType)
            {
                case ProductType.SingleProduct:
                    return GetProductByIsbn(id);

                case ProductType.Bundle:
                    return GetBundleById(id);

                default:
                    throw new InvalidDataException($"Product Type: {productType} is not correct");
            }
        }

        public GetScopeWorksByProductIdResponse GetScopeWorks(string isbn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// return Work (Product) from kencernData base on isbn
        /// </summary>
        /// <param name="isbn"></param>
        /// <returns></returns>
        private GetProductDetailsResponse GetProductByIsbn(string isbn)
        {
            try
            {
                var kdProd = KdEntities.DEA_KDWS_GDKProduct
                    .Include("DEA_KDWS_GDKproductseries.DEA_KDWS_GDKseries")
                    .Include("DEA_KDWS_GDKproductcontributors.DEA_KDWS_GDKContributors")
                    .Include("DEA_KDWS_GDKwork")
                    .FirstOrDefault(p => p.vare_id.Equals(isbn));

                if (kdProd?.DEA_KDWS_GDKWork == null)
                {
                    return new GetProductDetailsResponse
                    {
                        Message = "Product not found in KD.",
                        ProductNotFoundReason = ProductNotFoundReason.NoProductFoundInKd
                    };
                }
                var prodCategories = GetProductCategories(isbn);

                var work = kdProd.ToCoreDataWork(prodCategories.ToArray(), _shopServicesApiClient, _imageUtil, _configurationManager, HasFreeSupplementaryMaterial(isbn));

                work.Products.ForEach(
                    product =>
                    {
                        product.HasOtherDiscount = HasOtherDiscounts(product.Isbn13, product.MediaType.Name);
                        if (product.HasOtherDiscount)
                            product.DiscountPercentage = GetProductDiscount(product.Isbn13, product.MediaType.Name);
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

                if (string.IsNullOrWhiteSpace(pulsenProdDetail.Product.Description))
                    pulsenProdDetail.Product.Description = GetProductDescriptionFromRap(isbn);

                work = Common.ModelsMapping.GetClubWorkForTrade(work, pulsenProdDetail.Product, Clubs.GyldendalDk);

                return new GetProductDetailsResponse
                {
                    Message = "",
                    ProductNotFoundReason = ProductNotFoundReason.None,
                    ProductWork = work
                };
            }
            catch (Exception ex)
            {
                return new GetProductDetailsResponse
                {
                    ProductNotFoundReason = ProductNotFoundReason.ErrorWhileGettingProductData,
                    Message = $"Unexpected error occurred while retrieving product from KD. {ex}"
                };
            }
        }

        private string GetProductDescriptionFromRap(string isbn) => 
            KdEntities.DEA_KDWS_GDKProduct.FirstOrDefault(a => a.ISBN13.Equals(isbn))?.langbeskrivelse;
        
        /// <summary>
        /// Gets the Category objects agains the provided ISBN.
        /// </summary>
        /// <param name="isbn"></param>
        /// <returns></returns>
        private IEnumerable<DEA_KDWS_GDKcategory> GetProductCategories(string isbn)
        {
            return
                (from prodCatView in KdEntities.DEA_KDWS_GDK_ProductCategoriesView
                 join category in KdEntities.DEA_KDWS_GDKcategory on prodCatView.kategori equals category.id
                 where prodCatView.Vare_id == isbn
                 select category).ToArray();
        }

        /// <summary>
        /// return Work(Bundle) from kencernData base on id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private GetProductDetailsResponse GetBundleById(string id)
        {
            var kdBundleCategories = (from kdbundle in KdEntities.DEA_KDWS_GDKBundle
                                      where kdbundle.bundle_id.Equals(id)
                                      let kdCategories = (from kdcategory in KdEntities.DEA_KDWS_GDKcategory
                                                          join kdbundleCategory in KdEntities.DEA_KDWS_GDK_BundleCategoriesView on kdcategory.id equals kdbundleCategory.kategori
                                                          where kdbundleCategory.Bundle_id.Equals(id)
                                                          select kdcategory)
                                      select new { Bundle = kdbundle, Categories = kdCategories }).FirstOrDefault();

            if (kdBundleCategories?.Bundle == null)
            {
                return new GetProductDetailsResponse
                {
                    Message = "Product not found in KD.",
                    ProductNotFoundReason = ProductNotFoundReason.NoProductFoundInKd
                };
            }

            var work = kdBundleCategories.Bundle.ToCoreDataWork(kdBundleCategories.Categories.ToArray(), _configurationManager);
            work.Products.ForEach(bundleProducts => bundleProducts.IsBuyable = IsBundleBuyable(bundleProducts));

            return new GetProductDetailsResponse
            {
                Message = "",
                ProductNotFoundReason = ProductNotFoundReason.None,
                ProductWork = work
            };
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
    }
}