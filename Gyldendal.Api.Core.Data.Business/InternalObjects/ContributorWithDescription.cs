using Gyldendal.Api.CoreData.Contracts.Response;

namespace Gyldendal.Api.CoreData.Business.InternalObjects
{
    public class ContributorWithDescription: ContributorUpdateInfo
    {
        public string Description { get; set; }
    }
}
