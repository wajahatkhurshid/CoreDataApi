using Gyldendal.Api.CoreData.Business.Util;
using Gyldendal.Api.CoreData.Common.ConfigurationManager;
using Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Repositories;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Response;
using Gyldendal.Api.CoreData.DataAccess.KoncernData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Gyldendal.Api.CoreData.Business.Repositories.WsNone
{
    public class WorkRepository : BaseWorkProductRepository, IWorkRepository
    {
        private readonly ICoverImageUtil _imageUtil;

        private readonly IConfigurationManager _configurationManager;

        /// <summary>
        /// Creates a new instance of WorkRepository.
        /// </summary>
        /// <param name="kdEntities"></param>
        /// <param name="imageUtil"></param>
        public WorkRepository(koncerndata_webshops_Entities kdEntities, ICoverImageUtil imageUtil, IConfigurationManager configurationManager) :
            base(kdEntities)
        {
            _imageUtil = imageUtil;
            _configurationManager = configurationManager;
        }

        /// <summary>
        /// Return List of Deleted Work Ids of Gu shop from KD
        /// </summary>
        /// <returns></returns>
        public List<string> GetDeletedWorks(DateTime? fromDate)
        {
            throw GetNotImplementedException();
        }

        /// <summary>
        /// Return Work Object with Product Details
        /// </summary>
        /// <param name="id"></param>
        /// <param name="productType"></param>
        /// <returns>Returns Null if Work is not found</returns>
        public GetProductDetailsResponse GetWorkByProductId(string id, ProductType productType)
        {
            switch (productType)
            {
                case ProductType.SingleProduct:
                    return GetProductByIsbn(id);

                case ProductType.Bundle:
                    throw new InvalidOperationException("GetWorkByProductId does not supports bundle products.");

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
            var kdProd = KdEntities.DEA_KDWS_Ekeyproduct.FirstOrDefault(x => x.vare_id.Equals(isbn));

            if (kdProd == null)
            {
                return new GetProductDetailsResponse
                {
                    ProductNotFoundReason = ProductNotFoundReason.NoProductFoundInKd,
                    Message = "Product not found in KD."
                };
            }
            var work = kdProd?.ToCoreDataWork(_imageUtil, _configurationManager);

            return new GetProductDetailsResponse
            {
                Message = "",
                ProductNotFoundReason = ProductNotFoundReason.None,
                ProductWork = work
            };
        }

        /// <summary>
        /// return Work(Bundle) from kencernData base on id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // ReSharper disable once UnusedParameter.Local
        // ReSharper disable once UnusedMember.Local
        private Work GetBundleById(string id)
        {
            throw GetNotImplementedException();
        }
    }
}