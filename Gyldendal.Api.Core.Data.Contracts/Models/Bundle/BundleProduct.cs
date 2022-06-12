using System;
using Gyldendal.Api.ShopServices.Contracts.SalesConfiguration;
using System.Collections.Generic;
using Gyldendal.Api.CoreData.Contracts.Enumerations;

namespace Gyldendal.Api.CoreData.Contracts.Models.Bundle
{
    /// <summary>
    /// Product of bundle
    /// </summary>
    public class BundleProduct
    {
        /// <summary>
        /// ISBN of Product
        /// </summary>
        public string Isbn { get; set; }

        /// <summary>
        /// Title of Product
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Discount percentage on product
        /// </summary>
        public decimal DiscountPercentage { get; set; }

        /// <summary>
        /// A collection of SalesConfigurations associated with the product.
        /// </summary>
        public SalesConfiguration SalesConfiguration { get; set; }

        /// <summary>
        /// Price and discounted Price of bundle's Product for its accessFroms
        /// and billing period
        /// </summary>
        public List<BundleProductPrice> ProductPrices { get; set; }

        /// <summary>
        /// Nature of Product
        /// </summary>
        public bool IsPhysical { get; set; }

        /// <summary>
        /// Product's stock status
        /// </summary>
        public bool InStock { get; set; }

        /// <summary>
        /// Product's stock status
        /// </summary>
        public bool IsBuyable { get; set; }

        /// <summary>
        /// Either product is published or not
        /// </summary>
        public bool IsPublished { get; set; }

        /// <summary>
        /// Gets or sets the published date for the current print run of this product.
        /// </summary>
        public DateTime? CurrentPrintRunPublishDate { get; set; }

        /// <summary>
        /// Gets or sets the publish date for this product.
        /// </summary>
        public DateTime? PublishDate { get; set; }

        /// <summary>
        /// Gets or sets if the product's next print run is planned.
        /// </summary>
        public bool IsNextPrintRunPlanned { get; set; }

        /// <summary>
        /// Gets or sets the type of the media.
        /// </summary>
        /// <value>The type of the media.</value>
        public MediaType MediaType { get; set; }

        /// <summary>
        /// Gets or sets the type of the material.
        /// </summary>
        /// <value>The type of the material.</value>
        public MaterialType MaterialType { get; set; }

        public ProductSource ProductSource { get; set; }

    }
}