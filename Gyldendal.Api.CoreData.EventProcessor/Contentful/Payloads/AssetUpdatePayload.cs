using Gyldendal.Api.CoreData.EventProcessor.Common;

namespace Gyldendal.Api.CoreData.EventProcessor.Contentful.Payloads
{
    public class AssetUpdatePayload : BasePayload
    {
        public string AssetId { get; set; }
    }
}
