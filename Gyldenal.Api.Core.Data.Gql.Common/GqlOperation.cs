namespace Gyldendal.Api.CoreData.Gql.Common
{
    /// <summary>
    /// Enumeration for all available GQL methods
    /// </summary>
    public enum GqlOperation
    {
        [GqlOperationMeta("generalsearch")]
        GeneralSearch,

        [GqlOperationMeta("worksearch")]
        WorkSearch,

        [GqlOperationMeta("relatedproducts")]
        RelatedProducts,

        [GqlOperationMeta("thema")]
        Thema,

        [GqlOperationMeta("author")]
        Author,

        [GqlOperationMeta("title")]
        Title,

        [GqlOperationMeta("isbn")]
        Isbn,

        [GqlOperationMeta("publisher")]
        Publisher,

        [GqlOperationMeta("in_series")]
        InSeries,

        [GqlOperationMeta("publication_date")]
        PublicationDate,

        [GqlOperationMeta("media")]
        MediaType,

        [GqlOperationMeta("materialtype")]
        MaterialType,

        [GqlOperationMeta("area")]
        Area,

        [GqlOperationMeta("subarea")]
        SubArea,

        [GqlOperationMeta("subject")]
        Subject,

        [GqlOperationMeta("level")]
        Level,

        [GqlOperationMeta("contributorid")]
        ContributorId,

        [GqlOperationMeta("productid")]
        ProductId,

        [GqlOperationMeta("work")]
        Work,

        [GqlOperationMeta("maincat")]
        MainCategory,

        [GqlOperationMeta("subcat")]
        SubCategory,

        [GqlOperationMeta("label")]
        Label,

        [GqlOperationMeta("imprint")]
        Imprint,

        [GqlOperationMeta("isPhysical")]
        IsPhysical,

        [GqlOperationMeta("pricerange")]
        PriceRange,

        [GqlOperationMeta("webshop")]
        Webshop,


        [GqlOperationMeta("hasTrialAccess")]
        HasTrialAccess,
    }
}