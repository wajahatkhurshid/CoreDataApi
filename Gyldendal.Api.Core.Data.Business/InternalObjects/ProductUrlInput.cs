using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Common.ConfigurationManager;

namespace Gyldendal.Api.CoreData.Business.InternalObjects
{
    /// <summary>
    /// Holds Input Product url creation method
    /// </summary>
    internal class ProductUrlInput
    {
        internal WebShop webShop;
        internal string productId;
        internal string mediaType;
        internal bool isPhysical;
        internal string url;
        internal bool hasAttachments;
        internal IConfigurationManager configManager;
    }
}
