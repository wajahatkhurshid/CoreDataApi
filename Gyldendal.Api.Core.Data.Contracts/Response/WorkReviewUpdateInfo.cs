using System;

namespace Gyldendal.Api.CoreData.Contracts.Response
{
    public class WorkReviewUpdateInfo
    {
        public int WorkReviewId { get; set; }

        public WorkReviewUpdateType UpdateType { get; set; }
        
        public DateTime UpdateTime { get; set; }
    }

    public enum WorkReviewUpdateType
    {
        Updated = 0,
        Deleted = 1
    }
}
