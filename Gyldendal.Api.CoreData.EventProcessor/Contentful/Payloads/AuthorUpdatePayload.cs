using Gyldendal.Api.CoreData.EventProcessor.Common;

namespace Gyldendal.Api.CoreData.EventProcessor.Contentful.Payloads
{
    public class AuthorUpdatePayload : BasePayload
    {
        public string ContributorId { get; set; }
    }
}
