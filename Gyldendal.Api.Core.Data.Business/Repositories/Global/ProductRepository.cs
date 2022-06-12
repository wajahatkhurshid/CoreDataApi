using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Business.ExtensionMethods;
using Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Repositories;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Models.Bundle;
using Gyldendal.Api.CoreData.Contracts.Response;
using Gyldendal.Api.CoreData.DataAccess.KoncernData;
using NewRelic.Api.Agent;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gyldendal.Api.CoreData.Business.Repositories.Global
{
    public class ProductRepository : BaseWorkProductRepository, IProductRepository
    {
        /// <summary>
        /// Creates a new instance of ProductRepository.
        /// </summary>
        /// <param name="kdEntities"></param>
        public ProductRepository(koncerndata_webshops_Entities kdEntities) :
            base(kdEntities)
        {
        }

        /// <summary>
        /// Get GU Product details By isbn
        /// </summary>
        /// <param name="isbn"></param>
        /// <returns></returns>
        public Product GetProductByIsbn(string isbn)
        {
            throw GetNotImplementedException();
        }

        /// <summary>
        /// Get GU Bundle details By isbn
        /// </summary>
        /// <param name="isbn"></param>
        /// <returns></returns>
        public Product GetBundleByIsbn(string isbn)
        {
            throw GetNotImplementedException();
        }

        /// <summary>
        /// Returns the number of products updated after the given DateTime value.
        /// </summary>
        /// <param name="updatedAfter"></param>
        /// <returns></returns>
        public int GetUpdatedProductsCount(DateTime updatedAfter)
        {
            throw GetNotImplementedException();
        }

        /// <summary>
        /// Returns the asked page of ProductupdatedInfo objects, for each product, that was updated after the given DateTime, in KoncernDataWebShops database.
        /// </summary>
        /// <param name="updatedAfter"></param>
        /// <param name="pageIndex">Minimum value 0.</param>
        /// <param name="pageSize">Minimum value 1.</param>
        /// <exception cref="ArgumentException">If pageNo is less than zero.</exception>
        /// <exception cref="ArgumentException">If pageSize is less than one.</exception>
        /// <returns></returns>
        public IEnumerable<ProductUpdateInfo> GetProductsUpdateInfo(DateTime updatedAfter, int pageIndex, int pageSize)
        {
            throw GetNotImplementedException();
        }

        /// <summary>
        /// Gets count of the products for which campaigns exist
        /// </summary>
        /// <param name="updatedAfter">Start date of campaign</param>
        /// <returns>Count of campaign affected products</returns>
        public int GetCampaignProductsCount(DateTime updatedAfter)
        {
            throw GetNotImplementedException();
        }

        /// <summary>
        /// Checks whether a product has any active campaign
        /// </summary>
        /// <param name="productId">Product ID / ISBN</param>
        /// <returns>Returns whether a product has one or more active campaigns</returns>
        public bool HasActiveCampaign(string productId)
        {
            throw GetNotImplementedException();
        }

        /// <summary>
        /// Gets product ids of the products for which campaigns exist
        /// </summary>
        /// <param name="updatedAfter">Start date of campaign</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns>List of string containing product ids</returns>
        public IEnumerable<string> GetCampaignProducts(DateTime updatedAfter, int pageIndex, int pageSize)
        {
            throw GetNotImplementedException();
        }

        /// <summary>
        /// Returns the bundle's Id List which have this Isbn
        /// </summary>
        /// <param name="isbn"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Result<string> GetBundleIdsByIsbn(string isbn, int pageIndex, int pageSize)
        {
            throw GetNotImplementedException();
        }

        public bool IsProductBuyable(Product product)
        {
            throw GetNotImplementedException();
        }

        [Trace]
        public bool IsProductBuyable(BundleProduct bundleProduct)
        {
            throw GetNotImplementedException();
        }

        /// <summary>
        /// Get list of WebShops against isbn
        /// </summary>
        /// <param name="isbns"></param>
        /// <returns></returns>
        public Dictionary<string, List<WebShop>> GetProductWebshops(List<string> isbns)
        {
            if (!isbns?.Any() ?? true)
            { return null; }

            var dbResults = KdEntities.DEA_KDWS_GUproduct.Where(x => isbns.Contains(x.ISBN13))
                .Select(z => new { z.ISBN13, z.Website })
                .Union(KdEntities.DEA_KDWS_HRproduct.Where(x => isbns.Contains(x.ISBN13))
                    .Select(z => new { z.ISBN13, z.Website })
                .Union(KdEntities.DEA_KDWS_MUNKproduct.Where(x => isbns.Contains(x.ISBN13))
                    .Select(z => new { z.ISBN13, z.Website }))).ToList();

            return dbResults.GroupBy(x => x.ISBN13)
                .Select(x => new { ISBN = x.Key, Webshops = x.Select(y => y.Website) })
                .ToDictionary(z => z.ISBN, zz => zz.Webshops.Select(x => x.WebshopEnum()).ToList());
        }
    }
}