using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Factories;
using Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Repositories;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Models.Bundle;
using Gyldendal.Api.CoreData.Contracts.Response;
using System;
using System.Collections.Generic;
using System.IO;
using NewRelic.Api.Agent;

namespace Gyldendal.Api.CoreData.Business.Factories
{
    public class ProductFactory : CoreDataFactory<IProductRepository>, IProductFactory
    {
        public ProductFactory(IEnumerable<IProductRepository> repositories)
            : base(repositories)
        {
        }

        /// <summary>
        /// Get Product Details by Isbn
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="isbn"></param>
        /// <param name="productType"></param>
        /// <returns></returns>
        public Product GetProductByIsbn(DataScope dataScope, string isbn, ProductType productType)
        {
            switch (productType)
            {
                case ProductType.SingleProduct:
                    return this[dataScope].GetProductByIsbn(isbn);

                case ProductType.Bundle:
                    return this[dataScope].GetBundleByIsbn(isbn);

                default:
                    try
                    {
                        return this[dataScope].GetProductByIsbn(isbn);
                    }
                    catch
                    {
                        // ignored
                    }
                    try
                    {
                        return this[dataScope].GetBundleByIsbn(isbn);
                    }
                    catch
                    {
                        // ignored
                    }

                    throw new InvalidDataException($"Product Type: {productType} is not correct");
            }
        }

        /// <summary>
        /// Returns the number of products updated after the given DateTime value, for the given WebShop.
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="updatedAfter"></param>
        /// <returns></returns>
        public int GetUpdatedProductsCount(DataScope dataScope, DateTime updatedAfter)
        {
            return this[dataScope].GetUpdatedProductsCount(updatedAfter);
        }

        /// <summary>
        /// Returns the asked page of ProductupdatedInfo objects, for each product, related to the given WebShop, that was updated after the given DateTime, in KoncernDataWebShops database.
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="updatedAfter"></param>
        /// <param name="pageIndex">Minimum value 0.</param>
        /// <param name="pageSize">Minimum value 1.</param>
        /// <exception cref="ArgumentException">If pageNo is less than zero.</exception>
        /// <exception cref="ArgumentException">If pageSize is less than one.</exception>
        /// <returns></returns>
        public IEnumerable<ProductUpdateInfo> GetProductsUpdateInfo(DataScope dataScope, DateTime updatedAfter,
            int pageIndex, int pageSize)
        {
            return this[dataScope].GetProductsUpdateInfo(updatedAfter, pageIndex, pageSize);
        }

        /// <summary>
        /// Gets all the products for which some campaign exists
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="updatedAfterTicks"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns>List of string containing product ids</returns>
        public IEnumerable<string> GetCampaignProducts(DataScope dataScope, DateTime updatedAfterTicks, int pageIndex,
            int pageSize)
        {
            return this[dataScope].GetCampaignProducts(updatedAfterTicks, pageIndex, pageSize);
        }

        /// <summary>
        /// Gets count of the products for which campaigns exist
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="updatedAfterTicks">Start date of campaign</param>
        /// <returns>Count of campaign affected products</returns>
        public int GetCampaignProductsCount(DataScope dataScope, DateTime updatedAfterTicks)
        {
            return this[dataScope].GetCampaignProductsCount(updatedAfterTicks);
        }

        public bool HasActiveCampaign(DataScope dataScope, string productId)
        {
            return this[dataScope].HasActiveCampaign(productId);
        }

        [Trace]
        public bool IsProductBuyable(DataScope dataScope, Product product)
        {
            return this[dataScope].IsProductBuyable(product);
        }

        [Trace]
        public bool IsProductBuyable(DataScope dataScope, BundleProduct bundleProduct)
        {
            return this[dataScope].IsProductBuyable(bundleProduct);
        }

        /// <summary>
        /// Returns the bundle's Id List which have this Isbn
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="isbn"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Result<string> GetBundleIdsByIsbn(DataScope dataScope, string isbn, int pageIndex, int pageSize)
        {
            return this[dataScope].GetBundleIdsByIsbn(isbn, pageIndex, pageSize);
        }

        /// <summary>
        /// Get list of WebShops against isbn
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="isbns"></param>
        /// <returns></returns>
        public Dictionary<string, List<WebShop>> GetProductWebshops(DataScope dataScope, List<string> isbns)
        {
            return this[dataScope].GetProductWebshops(isbns);
        }
    }
}