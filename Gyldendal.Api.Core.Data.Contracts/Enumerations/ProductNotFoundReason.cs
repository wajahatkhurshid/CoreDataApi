namespace Gyldendal.Api.CoreData.Contracts.Enumerations
{
    public enum ProductNotFoundReason
    {
        None = 0,

        SupplementaryDataNotFound = 1,

        NoProductFoundInKd = 2,

        DataImportInProgress = 3,

        ProductOutOfStock = 4,
        
        ProductIsDeleted = 5,

        ErrorWhileGettingProductData = 6
    }
}