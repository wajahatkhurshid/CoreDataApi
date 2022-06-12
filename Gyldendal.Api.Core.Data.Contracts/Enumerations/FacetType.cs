namespace Gyldendal.Api.CoreData.Contracts.Enumerations
{
    /// <summary>
    /// Enumerations used for Faceting. Case Sensitive. When adding a new field make sure applied facet has the same name.
    /// </summary>
    public enum FacetType
    {
        SystemName = 1,

        LevelName = 2,

        SeriesName = 3,

        MaterialTypeName = 4,

        MediaTypeName = 5,

        Subjects = 6,

        Areas = 7,

        SubjectWithAreaAndSubarea = 8,

        SubAreas = 9,

        Genre = 10,

        Category = 11,

        SubCategory = 12,

        AuthorName = 13,

        AuthorIdWithName = 14,

        MaterialWithMediaTypeName = 15,

        HasTrialAccess = 16
    }
}