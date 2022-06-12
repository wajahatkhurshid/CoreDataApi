using System;
using SolrNet;
using SolrNet.Attributes;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable InconsistentNaming
// ReSharper disable CheckNamespace
// ReSharper disable IdentifierTypo

namespace Gyldendal.Api.CoreData.SolrContracts.Product
{
	public class Product
	{
		[SolrUniqueKey("id")]
		public string Id { get; set; }
	 
		[SolrField("description")]
		public string Description { get; set; }
	 
		[SolrField("duration")]
		public int Duration { get; set; }
	 
		[SolrField("edition")]
		public int Edition { get; set; }
	 
		[SolrField("excuseCode")]
		public string ExcuseCode { get; set; }
	 
		[SolrField("inStock")]
		public bool InStock { get; set; }
	 
		[SolrField("isNextPrintRunPlanned")]
		public bool IsNextPrintRunPlanned { get; set; }
	 
		[SolrField("isSaleConfigAvailable")]
		public bool IsSaleConfigAvailable { get; set; }
	 
		[SolrField("inspectionCopyAllowed")]
		public bool InspectionCopyAllowed { get; set; }
	 
		[SolrField("pages")]
		public int Pages { get; set; }
	 
		[SolrField("productId")]
		public string ProductId { get; set; }
	 
		[SolrField("discountPercentage")]
		public float DiscountPercentage { get; set; }
	 
		[SolrField("hasOtherDiscount")]
		public bool HasOtherDiscount { get; set; }
	 
		[SolrField("sampleUrl")]
		public string SampleUrl { get; set; }
	 
		[SolrField("seoText")]
		public string SeoText { get; set; }
	
		/// <summary>
        /// Index only field, not for retrieving purposes.
        /// </summary> 
		[SolrField("seriesIds")]
		public int[] SeriesIds { get; set; }
	 
		[SolrField("productSource")]
		public int ProductSource { get; set; }
	 
		[SolrField("seriesNames")]
		public string[] SeriesNames { get; set; }
	 
		[SolrField("subtitle")]
		public string Subtitle { get; set; }
	 
		[SolrField("systemNames")]
		public string[] SystemNames { get; set; }
	 
		[SolrField("title")]
		public string Title { get; set; }
	 
		[SolrField("workId")]
		public int WorkId { get; set; }
	 
		[SolrField("workText")]
		public string WorkText { get; set; }
	 
		[SolrField("workTitle")]
		public string WorkTitle { get; set; }
	 
		[SolrField("hasVideos")]
		public bool HasVideos { get; set; }
	 
		[SolrField("hasImages")]
		public bool HasImages { get; set; }
	 
		[SolrField("originalCoverImageUrl")]
		public string OriginalCoverImageUrl { get; set; }
	 
		[SolrField("isPhysical")]
		public bool IsPhysical { get; set; }
	 
		[SolrField("contributorIds")]
		public string[] ContributorIds { get; set; }
	 
		[SolrField("membershipPaths")]
		public string[] MembershipPaths { get; set; }
	 
		[SolrField("lastUpdated")]
		public DateTime LastUpdated { get; set; }
	 
		[SolrField("bundleProductTitles")]
		public string[] BundleProductTitles { get; set; }
	 
		[SolrField("bundleProductIsbns")]
		public string[] BundleProductIsbns { get; set; }
	 
		[SolrField("productType")]
		public int ProductType { get; set; }
	 
		[SolrField("reviewText")]
		public string ReviewText { get; set; }
	 
		[SolrField("areas")]
		public string[] Areas { get; set; }
	 
		[SolrField("authorNames")]
		public string[] AuthorNames { get; set; }
	 
		[SolrField("isbn13")]
		public string Isbn13 { get; set; }
	 
		[SolrField("levels")]
		public string[] Levels { get; set; }
	 
		[SolrField("materialTypeName")]
		public string MaterialTypeName { get; set; }
	 
		[SolrField("mediaTypeName")]
		public string MediaTypeName { get; set; }
	 
		[SolrField("publishDate")]
		public DateTime PublishDate { get; set; }
	 
		[SolrField("currentPrintRunPublishDate")]
		public DateTime CurrentPrintRunPublishDate { get; set; }
	 
		[SolrField("physicalIsbn")]
		public string PhysicalIsbn { get; set; }
	 
		[SolrField("publisherId")]
		public int PublisherId { get; set; }
	 
		[SolrField("publisher")]
		public string Publisher { get; set; }
	 
		[SolrField("subAreas")]
		public string[] SubAreas { get; set; }
	
		/// <summary>
        /// Index only field, not for retrieving purposes.
        /// </summary> 
		[SolrField("subjects")]
		public string[] Subjects { get; set; }
	 
		[SolrField("subjectWithAreaAndSubarea")]
		public string[] SubjectWithAreaAndSubarea { get; set; }
	
		/// <summary>
        /// Index only field, not for retrieving purposes.
        /// </summary> 
		[SolrField("themaCodes")]
		public string[] ThemaCodes { get; set; }
	 
		[SolrField("websiteId")]
		public int WebsiteId { get; set; }
	 
		[SolrField("mediaTypeRank")]
		public int MediaTypeRank { get; set; }
	 
		[SolrField("materialTypeRank")]
		public int MaterialTypeRank { get; set; }
	 
		[SolrField("grossWeight")]
		public double GrossWeight { get; set; }
	 
		[SolrField("netWeight")]
		public double NetWeight { get; set; }
	 
		[SolrField("height")]
		public int Height { get; set; }
	 
		[SolrField("width")]
		public int Width { get; set; }
	 
		[SolrField("thicknessDepth")]
		public int ThicknessDepth { get; set; }
	 
		[SolrField("isSupplementaryMaterial")]
		public bool IsSupplementaryMaterial { get; set; }
	 
		[SolrField("serializedAreasInfo")]
		public string SerializedAreasInfo { get; set; }
	 
		[SolrField("serializedContributorsInfo")]
		public string SerializedContributorsInfo { get; set; }
	 
		[SolrField("serializedCoverImagesInfo")]
		public string SerializedCoverImagesInfo { get; set; }
	 
		[SolrField("serializedDistributorsInfo")]
		public string SerializedDistributorsInfo { get; set; }
	 
		[SolrField("serializedLevelsInfo")]
		public string SerializedLevelsInfo { get; set; }
	 
		[SolrField("serializedReviews")]
		public string SerializedReviews { get; set; }
	 
		[SolrField("serializedSalesConfigs")]
		public string SerailizedSalesConfigs { get; set; }
	 
		[SolrField("serializedPricingInfo")]
		public string SerializedPricingInfo { get; set; }
	 
		[SolrField("serializedSeriesInfo")]
		public string SerializedSerisInfo { get; set; }
	 
		[SolrField("serializedSubAreasInfo")]
		public string SerializedSubAreasInfo { get; set; }
	 
		[SolrField("serializedSubjectsInfo")]
		public string SerializedSubjectsInfo { get; set; }
	 
		[SolrField("serializedThemaCodes")]
		public string SerializedThemaCodes { get; set; }
	 
		[SolrField("serializedBundledProducts")]
		public string SerializedBundledProducts { get; set; }
	 
		[SolrField("serializedProductUrlInfo")]
		public string serializedProductUrlInfo { get; set; }
	 
		[SolrField("serializedExtendedPurchaseOption")]
		public string serializedExtendedPurchaseOption { get; set; }
	 
		[SolrField("serializedProductFreeMaterial")]
		public string serializedProductFreeMaterial { get; set; }
	 
		[SolrField("phoneticsearch")]
		public string[] PhoneticSearch { get; set; }
	 
		[SolrField("exactmatchfield")]
		public string[] ExactMatch { get; set; }
	 
		[SolrField("substringfield")]
		public string[] SubstringField { get; set; }
	 
		[SolrField("themaexactmatchfield")]
		public string[] ThemaExactmatch { get; set; }
	 
		[SolrField("themasubstringfield")]
		public string[] ThemaSubstring { get; set; }
	
		/// <summary>
        /// Index only field, not for retrieving purposes.
        /// </summary> 
		[SolrField("genres")]
		public string[] Genres { get; set; }
	
		/// <summary>
        /// Index only field, not for retrieving purposes.
        /// </summary> 
		[SolrField("categories")]
		public string[] Categories { get; set; }
	
		/// <summary>
        /// Index only field, not for retrieving purposes.
        /// </summary> 
		[SolrField("subcategories")]
		public string[] Subcategories { get; set; }
	 
		[SolrField("serializedGenres")]
		public string SerializedGenres { get; set; }
	 
		[SolrField("serializedCategories")]
		public string SerializedCategories { get; set; }
	 
		[SolrField("serializedSubcategories")]
		public string SerializedSubcategories { get; set; }
	 
		[SolrField("labels")]
		public string[] Labels { get; set; }
	 
		[SolrField("defaultPrice")]
		public Money DefaultPrice { get; set; }
	 
		[SolrField("discountedPrice")]
		public Money DiscountedPrice { get; set; }
	 
		[SolrField("authorIdWithName")]
		public string[] AuthorIdWithName { get; set; }
	 
		[SolrField("extraData")]
		public string ExtraData { get; set; }
	 
		[SolrField("imprint")]
		public string Imprint { get; set; }
	 
		[SolrField("materialWithMediaTypeName")]
		public string MaterialWithMediaTypeName { get; set; }
	 
		[SolrField("updateDueOn")]
		public DateTime? UpdateDueOn { get; set; }
	 
		[SolrField("hasTrialAccess")]
		public bool HasTrialAccess { get; set; }
	}

	public enum ProductSchemaField
	{
		[ProductSchemaFieldMeta("id")]
		Id,

		[ProductSchemaFieldMeta("description")]
		Description,

		[ProductSchemaFieldMeta("duration")]
		Duration,

		[ProductSchemaFieldMeta("edition")]
		Edition,

		[ProductSchemaFieldMeta("excuseCode")]
		ExcuseCode,

		[ProductSchemaFieldMeta("inStock")]
		InStock,

		[ProductSchemaFieldMeta("isNextPrintRunPlanned")]
		IsNextPrintRunPlanned,

		[ProductSchemaFieldMeta("isSaleConfigAvailable")]
		IsSaleConfigAvailable,

		[ProductSchemaFieldMeta("inspectionCopyAllowed")]
		InspectionCopyAllowed,

		[ProductSchemaFieldMeta("pages")]
		Pages,

		[ProductSchemaFieldMeta("productId")]
		ProductId,

		[ProductSchemaFieldMeta("discountPercentage")]
		DiscountPercentage,

		[ProductSchemaFieldMeta("hasOtherDiscount")]
		HasOtherDiscount,

		[ProductSchemaFieldMeta("sampleUrl")]
		SampleUrl,

		[ProductSchemaFieldMeta("seoText")]
		SeoText,

		[ProductSchemaFieldMeta("seriesIds")]
		SeriesIds,

		[ProductSchemaFieldMeta("productSource")]
		ProductSource,

		[ProductSchemaFieldMeta("seriesNames")]
		SeriesNames,

		[ProductSchemaFieldMeta("seriesNamesDa")]
		SeriesNamesDa,

		[ProductSchemaFieldMeta("subtitle")]
		Subtitle,

		[ProductSchemaFieldMeta("systemNamesDa")]
		SystemNamesDa,

		[ProductSchemaFieldMeta("systemNames")]
		SystemNames,

		[ProductSchemaFieldMeta("title")]
		Title,

		[ProductSchemaFieldMeta("titleDa")]
		TitleDa,

		[ProductSchemaFieldMeta("title_contains")]
		TitleContains,

		[ProductSchemaFieldMeta("titleSorting")]
		TitleSorting,

		[ProductSchemaFieldMeta("title_substring")]
		TitleSubstring,

		[ProductSchemaFieldMeta("workId")]
		WorkId,

		[ProductSchemaFieldMeta("workText")]
		WorkText,

		[ProductSchemaFieldMeta("workTitle")]
		WorkTitle,

		[ProductSchemaFieldMeta("hasVideos")]
		HasVideos,

		[ProductSchemaFieldMeta("hasImages")]
		HasImages,

		[ProductSchemaFieldMeta("originalCoverImageUrl")]
		OriginalCoverImageUrl,

		[ProductSchemaFieldMeta("isPhysical")]
		IsPhysical,

		[ProductSchemaFieldMeta("contributorIds")]
		ContributorIds,

		[ProductSchemaFieldMeta("membershipPaths")]
		MembershipPaths,

		[ProductSchemaFieldMeta("lastUpdated")]
		LastUpdated,

		[ProductSchemaFieldMeta("bundleProductTitles")]
		BundleProductTitles,

		[ProductSchemaFieldMeta("bundleProductIsbns")]
		BundleProductIsbns,

		[ProductSchemaFieldMeta("productType")]
		ProductType,

		[ProductSchemaFieldMeta("reviewText")]
		ReviewText,

		[ProductSchemaFieldMeta("areas")]
		Areas,

		[ProductSchemaFieldMeta("areasDa")]
		AreasDa,

		[ProductSchemaFieldMeta("authorNames")]
		AuthorNames,

		[ProductSchemaFieldMeta("authorNamesDa")]
		AuthorNamesDa,

		[ProductSchemaFieldMeta("isbn13")]
		Isbn13,

		[ProductSchemaFieldMeta("isbn13String")]
		isbn13String,

		[ProductSchemaFieldMeta("levels")]
		Levels,

		[ProductSchemaFieldMeta("levelsDa")]
		LevelsDa,

		[ProductSchemaFieldMeta("materialTypeName")]
		MaterialTypeName,

		[ProductSchemaFieldMeta("mediaTypeName")]
		MediaTypeName,

		[ProductSchemaFieldMeta("searchMedia")]
		SearchMedia,

		[ProductSchemaFieldMeta("publishDate")]
		PublishDate,

		[ProductSchemaFieldMeta("currentPrintRunPublishDate")]
		CurrentPrintRunPublishDate,

		[ProductSchemaFieldMeta("physicalIsbn")]
		PhysicalIsbn,

		[ProductSchemaFieldMeta("publisherId")]
		PublisherId,

		[ProductSchemaFieldMeta("publisher")]
		Publisher,

		[ProductSchemaFieldMeta("publisherDa")]
		PublisherDa,

		[ProductSchemaFieldMeta("subAreas")]
		SubAreas,

		[ProductSchemaFieldMeta("subAreasDa")]
		SubAreasDa,

		[ProductSchemaFieldMeta("subjects")]
		Subjects,

		[ProductSchemaFieldMeta("subjectsDa")]
		SubjectsDa,

		[ProductSchemaFieldMeta("subjectWithAreaAndSubarea")]
		SubjectWithAreaAndSubarea,

		[ProductSchemaFieldMeta("themaCodes")]
		ThemaCodes,

		[ProductSchemaFieldMeta("websiteId")]
		WebsiteId,

		[ProductSchemaFieldMeta("mediaTypeRank")]
		MediaTypeRank,

		[ProductSchemaFieldMeta("materialTypeRank")]
		MaterialTypeRank,

		[ProductSchemaFieldMeta("grossWeight")]
		GrossWeight,

		[ProductSchemaFieldMeta("netWeight")]
		NetWeight,

		[ProductSchemaFieldMeta("height")]
		Height,

		[ProductSchemaFieldMeta("width")]
		Width,

		[ProductSchemaFieldMeta("thicknessDepth")]
		ThicknessDepth,

		[ProductSchemaFieldMeta("isSupplementaryMaterial")]
		IsSupplementaryMaterial,

		[ProductSchemaFieldMeta("serializedAreasInfo")]
		SerializedAreasInfo,

		[ProductSchemaFieldMeta("serializedContributorsInfo")]
		SerializedContributorsInfo,

		[ProductSchemaFieldMeta("serializedCoverImagesInfo")]
		SerializedCoverImagesInfo,

		[ProductSchemaFieldMeta("serializedDistributorsInfo")]
		SerializedDistributorsInfo,

		[ProductSchemaFieldMeta("serializedLevelsInfo")]
		SerializedLevelsInfo,

		[ProductSchemaFieldMeta("serializedReviews")]
		SerializedReviews,

		[ProductSchemaFieldMeta("serializedSalesConfigs")]
		SerailizedSalesConfigs,

		[ProductSchemaFieldMeta("serializedPricingInfo")]
		SerializedPricingInfo,

		[ProductSchemaFieldMeta("serializedSeriesInfo")]
		SerializedSerisInfo,

		[ProductSchemaFieldMeta("serializedSubAreasInfo")]
		SerializedSubAreasInfo,

		[ProductSchemaFieldMeta("serializedSubjectsInfo")]
		SerializedSubjectsInfo,

		[ProductSchemaFieldMeta("serializedThemaCodes")]
		SerializedThemaCodes,

		[ProductSchemaFieldMeta("serializedBundledProducts")]
		SerializedBundledProducts,

		[ProductSchemaFieldMeta("serializedProductUrlInfo")]
		serializedProductUrlInfo,

		[ProductSchemaFieldMeta("serializedExtendedPurchaseOption")]
		serializedExtendedPurchaseOption,

		[ProductSchemaFieldMeta("serializedProductFreeMaterial")]
		serializedProductFreeMaterial,

		[ProductSchemaFieldMeta("phoneticsearch")]
		PhoneticSearch,

		[ProductSchemaFieldMeta("exactmatchfield")]
		ExactMatch,

		[ProductSchemaFieldMeta("substringfield")]
		SubstringField,

		[ProductSchemaFieldMeta("themaexactmatchfield")]
		ThemaExactmatch,

		[ProductSchemaFieldMeta("themasubstringfield")]
		ThemaSubstring,

		[ProductSchemaFieldMeta("titleReplaced")]
		TitleReplaced,

		[ProductSchemaFieldMeta("subtitleReplaced")]
		SubtitleReplaced,

		[ProductSchemaFieldMeta("seriesNamesReplaced")]
		SeriesNamesReplaced,

		[ProductSchemaFieldMeta("genres")]
		Genres,

		[ProductSchemaFieldMeta("categories")]
		Categories,

		[ProductSchemaFieldMeta("categoriesDa")]
		CategoriesDa,

		[ProductSchemaFieldMeta("subcategories")]
		Subcategories,

		[ProductSchemaFieldMeta("subcategoriesDa")]
		SubcategoriesDa,

		[ProductSchemaFieldMeta("serializedGenres")]
		SerializedGenres,

		[ProductSchemaFieldMeta("serializedCategories")]
		SerializedCategories,

		[ProductSchemaFieldMeta("serializedSubcategories")]
		SerializedSubcategories,

		[ProductSchemaFieldMeta("labels")]
		Labels,

		[ProductSchemaFieldMeta("defaultPrice")]
		DefaultPrice,

		[ProductSchemaFieldMeta("discountedPrice")]
		DiscountedPrice,

		[ProductSchemaFieldMeta("authorIdWithName")]
		AuthorIdWithName,

		[ProductSchemaFieldMeta("extraData")]
		ExtraData,

		[ProductSchemaFieldMeta("imprint")]
		Imprint,

		[ProductSchemaFieldMeta("materialWithMediaTypeName")]
		MaterialWithMediaTypeName,

		[ProductSchemaFieldMeta("updateDueOn")]
		UpdateDueOn,

		[ProductSchemaFieldMeta("hasTrialAccess")]
		HasTrialAccess,


	}
}
