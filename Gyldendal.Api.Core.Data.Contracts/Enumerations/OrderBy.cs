namespace Gyldendal.Api.CoreData.Contracts.Enumerations
{
    /// <summary>
    /// Enumerations to specify the field to order by. Case sensitive. When adding a new field ensure exact match with solr field.
    /// </summary>
    public enum OrderBy
    {
        None=0,
        // ReSharper disable once InconsistentNaming
        isbn13,
        // ReSharper disable once InconsistentNaming
        title,
        // ReSharper disable once InconsistentNaming
        productId,
        // ReSharper disable once InconsistentNaming
        workId,
        // ReSharper disable once InconsistentNaming
        publishDate,
        // ReSharper disable once InconsistentNaming
        mediaTypeRank,
        // ReSharper disable once InconsistentNaming
        defaultPrice
    }
}
