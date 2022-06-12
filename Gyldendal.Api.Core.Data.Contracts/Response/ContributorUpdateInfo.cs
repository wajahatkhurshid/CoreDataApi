using System;

namespace Gyldendal.Api.CoreData.Contracts.Response
{
    public class ContributorUpdateInfo
    {
        public string ContributorId { get; set; }

        public ContributorUpdateType UpdateType { get; set; }

        
        public DateTime UpdateTime { get; set; }
    }

    public enum ContributorUpdateType
    {
        Updated = 0,
        Deleted = 1
    }
}
