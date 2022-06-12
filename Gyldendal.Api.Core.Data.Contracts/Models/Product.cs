// ReSharper disable UnusedAutoPropertyAccessor.Global

using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models.Bundle;
using Gyldendal.Api.CoreData.Contracts.Models.Club;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Gyldendal.Api.CoreData.Contracts.Models
{
    /// <summary>
    /// Class Product.
    /// </summary>
    public class Product
    {
        public string Id { get; set; }

        public string Isbn13 { get; set; }

        public string PhysicalIsbn { get; set; }

        public string Title { get; set; }

        public string Subtitle { get; set; }

        public string Description { get; set; }

        public decimal? DiscountPercentage { get; set; }

        public decimal? DiscountedPrice { get; set; }

        /// <summary>
        /// Gets or sets the has other discount
        /// </summary>
        /// <value>value is based on the campaigns in which product has been added.</value>
        public bool HasOtherDiscount { get; set; }

        public ShopServices.Contracts.SalesConfiguration.SalesConfiguration SalesConfiguration { get; set; }

        public PricingInfo PricingInfo { get; set; }

        [XmlArray("Contributors")]
        public List<Contributor> Contributors { get; set; }

        public int DurationInMinutes { get; set; }

        public MediaType MediaType { get; set; }

        public MaterialType MaterialType { get; set; }

        public bool IsPublished { get; set; }

        public DateTime? PublishDate { get; set; }

        public DateTime? CurrentPrintRunPublishDate { get; set; }

        public int? Edition { get; set; }

        public string Publisher { get; set; }

        public int PublisherId { get; set; }

        public bool HasImages { get; set; }

        public bool HasVideos { get; set; }

        public int? Pages { get; set; }

        public List<ProductImage> CoverImages { get; set; }

        public string OriginalCoverImageUrl { get; set; }

        public List<Series> Series { get; set; }

        public bool InStock { get; set; }

        public bool IsNextPrintPlanned { get; set; }
        
        public DateTime? UpdateDueOn { get; set; }

        public bool IsBuyable { get; set; }

        public string SeoText { get; set; }

        public int? WorkId { get; set; }

        public List<ElectronicDistributor> Distributors { get; set; }

        public string SampleUrl { get; set; }

        public List<ProductUrl> ProductUrls { get; set; }

        public string ExcuseCode { get; set; }

        public List<ProductReview> Reviews { get; set; }

        public bool IsPhysical { get; set; }

        public DateTime LastUpdated { get; set; }

        public WebShop WebShop { get; set; }

        public List<string> MembershipPaths { get; set; }

        public ProductType ProductType { get; set; }

        public List<BundleProduct> BundleProducts { get; set; }

        public bool IsSupplementaryMaterial { get; set; }

        public bool InspectionCopyAllowed { get; set; }

        public List<ProductFreeMaterial> FreeMaterials { get; set; }

        public ProductSource ProductSource { get; set; }

        public int MaterialTypeRank { get; set; }

        public int MediaTypeRank { get; set; }

        public double GrossWeight { get; set; }

        public double NetWeight { get; set; }

        public int Height { get; set; }

        public int Width { get; set; }

        public int ThicknessDepth { get; set; }

        /// <summary>
        /// Period information for "Book of the Month" in a club (For Gyldendal+)<br/>
        /// No longer in Use.
        /// </summary>
        [Obsolete]
        public List<ClubPeriod> ClubPeriods { get; set; }

        public List<Genre> Genres { get; set; }

        public List<Category> Categories { get; set; }

        public List<SubCategory> SubCategories { get; set; }

        /// <summary>
        /// Product labels for Bogklub project.
        /// </summary>
        public List<string> Labels { get; set; }

        /// <summary>
        /// Default price of a product. Initially only used for B2C products.
        /// </summary>
        public decimal? DefaultPrice { get; set; }

        /// <summary>
        /// Contains product extra data related to webshop in serialized form.
        /// </summary>
        public string ExtraData { get; set; }

        /// <summary>
        /// Product imprint information. Initially only used for B2C products.
        /// </summary>
        public string Imprint { get; set; }

        /// <summary>
        /// Details for extended purchase options for a product like teacherSample or inspection copy etc.
        /// </summary>
        public List<ExtendedPurchaseOption> ExtendedPurchaseOptions { get; set; }
      
    }
}