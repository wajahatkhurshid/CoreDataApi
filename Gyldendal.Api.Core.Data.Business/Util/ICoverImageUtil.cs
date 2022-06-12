using Gyldendal.Api.CoreData.Business.InternalObjects;
using Gyldendal.Api.CoreData.Contracts.Enumerations;

namespace Gyldendal.Api.CoreData.Business.Util
{
    public interface ICoverImageUtil
    {
        VariantImage GetProductImagesVariant(string isbn, DataScope webShop = DataScope.Global, string organizationName = null);

        string GetOriginalImageUrl(string isbn);
    }
}