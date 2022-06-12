using System.ComponentModel;

namespace Gyldendal.Api.CoreData.Common
{
    /// <summary>
    /// Error codes of CoreData , must start from 700__
    /// </summary>
    public enum ErrorCodes : ulong
    {
        [Description("The error code provided is not valid")]
        InvalidErrorCode = 1,
        [Description("Invalid WebSite")]
        InvalidWebSite = 70001,
        [Description("The provided Gql value is invalid.")]
        InvalidGql = 70002,
        [Description("The parameter value is Null")]
        NullValue = 70005,
        [Description("No implementation found")]
        NotImplemented = 70006,
        [Description("Invalid price range filter provided.")]
        InvalidPriceFilter = 70007,
        [Description("Invalid price range facet params.")]
        InvalidPriceFacet = 70008,
        [Description("Invalid product data profile.")]
        InvalidProductDataProfile = 70009,
        [Description("Isbns are required.")]
        MissingIsbns = 70010,

    }
}
