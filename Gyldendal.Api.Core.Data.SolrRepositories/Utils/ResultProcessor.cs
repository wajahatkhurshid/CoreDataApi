using System;
using System.Collections.Generic;
using System.Linq;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Business.ExtensionMethods;
using Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Factories;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models.Bundle;
using NewRelic.Api.Agent;

namespace Gyldendal.Api.CoreData.SolrDataProviders.Utils
{
    public class ResultProcessor
    {
        private readonly IProductFactory _productFactory;

        public ResultProcessor(IProductFactory productFactory)
        {
            _productFactory = productFactory;
        }

        [Trace]
        /// <summary>
        /// Post data retrival processing.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public void ProcessProduct(Contracts.Models.Product product)
        {
            SetProductBuyableStatus(product);
        }

        [Trace]
        /// <summary>
        /// Set product buyable status
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        private void SetProductBuyableStatus(Contracts.Models.Product product)
        {
            switch (product.ProductType)
            {
                case ProductType.SingleProduct:
                    product.IsBuyable = _productFactory.IsProductBuyable(product.WebShop.ToDataScope(), product);
                    break;

                case ProductType.Bundle:
                    if (product.BundleProducts == null)
                    { throw new ArgumentNullException(nameof(product.BundleProducts)); }

                    SetBundleProductProperties(product.BundleProducts, product.WebShop);
                    product.IsBuyable = product.BundleProducts.All(x => x.IsBuyable);
                    break;

                default:
                    return;
            }
        }

        [Trace]
        /// <summary>
        ///  Setting onfly calculating properties for bundle products.
        /// </summary>
        /// <param name="bundleProducts"></param>
        /// <param name="webShop"></param>
        private void SetBundleProductProperties(List<BundleProduct> bundleProducts, WebShop webShop)
        {
            foreach (var bundleProduct in bundleProducts)
            {
                bundleProduct.IsPublished = bundleProduct.CurrentPrintRunPublishDate <= DateTime.Now;
                bundleProduct.IsBuyable = _productFactory.IsProductBuyable(webShop.ToDataScope(), bundleProduct);
            }
        }
    }
}