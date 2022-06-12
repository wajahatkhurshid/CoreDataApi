using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Business.Util;
using Gyldendal.Api.CoreData.Common.ConfigurationManager;
using Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Repositories;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Response;
using Gyldendal.Api.CoreData.DataAccess.KoncernData;
using Gyldendal.PulsenServices.Api.Contracts.Common;
using Gyldendal.PulsenServices.ApiClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PulsenProduct = Gyldendal.PulsenServices.Api.Contracts.Product.Product;
using PulsenProductNotFoundReason = Gyldendal.PulsenServices.Api.Contracts.Product.ProductNotFoundReason;
using ProductNotFoundReason = Gyldendal.Api.CoreData.Contracts.Enumerations.ProductNotFoundReason;

namespace Gyldendal.Api.CoreData.Business.Repositories.GPlus
{
    public class WorkRepository : BaseWorkProductRepository, IWorkRepository
    {
        private readonly ShopServices.ApiClient.Client _shopServicesApiClient;

        private readonly ICoverImageUtil _imageUtil;

        private readonly IConfigurationManager _configurationManager;

        private readonly PulsenServiceApiClient _pulsenServiceApiClient;

        /// <summary>
        /// Constructor of GPlus Product
        /// </summary>
        /// <param name="kdEntities"></param>
        /// <param name="shopServicesApiClient"></param>
        /// <param name="imageUtil"></param>
        /// <param name="configurationManager"></param>
        /// <param name="pulsenServiceApiClient"></param>
        public WorkRepository(koncerndata_webshops_Entities kdEntities, ShopServices.ApiClient.Client shopServicesApiClient, ICoverImageUtil imageUtil, IConfigurationManager configurationManager, PulsenServiceApiClient pulsenServiceApiClient) :
            base(kdEntities)
        {
            _shopServicesApiClient = shopServicesApiClient;
            _imageUtil = imageUtil;
            _configurationManager = configurationManager;
            _pulsenServiceApiClient = pulsenServiceApiClient;
        }

        /// <summary>
        /// Return List of Deleted Work Ids of GPlus shop from KD
        /// </summary>
        /// <returns></returns>
        public List<string> GetDeletedWorks(DateTime? fromDate)
        {
            if (!fromDate.HasValue)
            {
                fromDate = DateTime.MinValue;
            }

            return KdEntities.DEA_KDWS_GPlusWorkLog
                .Where(w => w.Action.Equals(Common.Constants.DeleteAction) && w.CreatedDate >= fromDate)
                .Select(w => w.work_id.ToString())
                .ToList();
        }

        /// <summary>
        /// Return Work Object with Product Details
        /// </summary>
        /// <param name="id"></param>
        /// <param name="productType"></param>
        /// <returns>Returns Null if Work is not found</returns>
        public GetProductDetailsResponse GetWorkByProductId(string id, Contracts.Enumerations.ProductType productType)
        {
            switch (productType)
            {
                case Contracts.Enumerations.ProductType.SingleProduct:
                    return GetProductByIsbn(id);

                case Contracts.Enumerations.ProductType.Bundle:
                    throw new InvalidOperationException("GD-Plus doesn't have support for Bundle type product.");

                default:
                    throw new InvalidDataException($"Product Type: {productType} is not correct");
            }
        }

        /// <summary>
        /// Gets the Category objects agains the provided ISBN.
        /// </summary>
        /// <param name="isbn"></param>
        /// <returns></returns>
        private IEnumerable<DEA_KDWS_GPluscategory> GetProductCategories(string isbn)
        {
            return
                (from prodCatView in KdEntities.DEA_KDWS_GPlus_ProductCategoriesView
                 join category in KdEntities.DEA_KDWS_GPluscategory on prodCatView.kategori equals category.id
                 where prodCatView.Vare_id == isbn
                 select category).ToArray();
        }

        /// <summary>
        /// return Work (Product) from kencernData base on isbn
        /// </summary>
        /// <param name="isbn"></param>
        /// <returns></returns>
        private GetProductDetailsResponse GetProductByIsbn(string isbn)
        {
            var kdProd = KdEntities.DEA_KDWS_GPlusproduct
                      .Include("DEA_KDWS_GPlusproductseries.DEA_KDWS_GPlusseries")
                      .Include("DEA_KDWS_GPlusproductcontributors.DEA_KDWS_GPlusContributors")
                      .Include("DEA_KDWS_GPlusProductCategories.DEA_KDWS_GPluscategory")
                      .Include("DEA_KDWS_GPlusProductLevels.DEA_KDWS_GPluslevel")
                      .Include("DEA_KDWS_GPluswork")
                      .FirstOrDefault(p => p.vare_id.Equals(isbn));

            if (kdProd?.DEA_KDWS_GPluswork == null)
            {
                return new GetProductDetailsResponse
                {
                    Message = "Product not found in KD.",
                    ProductNotFoundReason = ProductNotFoundReason.NoProductFoundInKd,
                };
            }

            var prodCategories = GetProductCategories(isbn);

            var work = kdProd.ToCoreDataWork(prodCategories.ToArray(), _shopServicesApiClient, _imageUtil, _configurationManager, WebShop.ClubBogklub, HasFreeSupplementaryMaterial(isbn));

            return new GetProductDetailsResponse
            {
                Message = "",
                ProductNotFoundReason = ProductNotFoundReason.None,
                ProductWork = work
            };
        }

        public GetScopeWorksByProductIdResponse GetScopeWorks(string isbn)
        {
            var retVal = new GetScopeWorksByProductIdResponse { Works = new List<Work>() };

            var kdWorkTask = Task.Factory.StartNew(() => GetProductByIsbn(isbn));
            var pulsenProdDetailTask = Task.Factory.StartNew(() => _pulsenServiceApiClient.Product.GetProductDetails(isbn));

            try
            {
                kdWorkTask.Wait();

                var kdWork = kdWorkTask.Result;
                if (kdWork.ProductWork == null)
                {
                    retVal.Message = "Product Not Found in KD";
                    retVal.Works = null;
                    retVal.ProductNotFoundReason = ProductNotFoundReason.NoProductFoundInKd;

                    return retVal;
                }

                pulsenProdDetailTask.Wait();

                var pulsenProdDetail = pulsenProdDetailTask.Result;

                if (pulsenProdDetail.Product == null)
                {
                    retVal.Message = $"No supplementary data found in pulsen services: {pulsenProdDetail.ProductNotFoundReason:G}";
                    retVal.Works = null;
                    retVal.ProductNotFoundReason = GetProductNotFoundReason(pulsenProdDetail.ProductNotFoundReason);

                    return retVal;
                }

                if (string.IsNullOrWhiteSpace(pulsenProdDetail.Product.Description))
                    pulsenProdDetail.Product.Description = GetProductDescriptionFromRap(isbn);
                
                // Excluding sales channel 039 because its works will be imported through GDK importer written in CoreDataAgent
                var kdWorks = pulsenProdDetail.Product.ClubIds.Where(club => club != Clubs.GyldendalDk).Select(clubId => Common.ModelsMapping.GetClubWorkForTrade(kdWork.ProductWork, pulsenProdDetail.Product, clubId)).ToList();
                retVal.Works = kdWorks;

                return retVal;
            }
            catch (AggregateException e)
            {
                throw e.InnerException ?? e;
            }
        }

        private static ProductNotFoundReason GetProductNotFoundReason(PulsenServices.Api.Contracts.Product.ProductNotFoundReason productNotFoundReason)
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

        private string GetProductDescriptionFromRap(string isbn) => 
            KdEntities.DEA_KDWS_GPlusproduct.FirstOrDefault(a => a.ISBN13.Equals(isbn))?.langbeskrivelse;
    }
}