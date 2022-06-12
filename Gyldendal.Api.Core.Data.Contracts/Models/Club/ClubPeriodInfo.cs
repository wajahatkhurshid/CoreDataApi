using System;

namespace Gyldendal.Api.CoreData.Contracts.Models.Club
{
    public class ClubPeriodInfo
    {
        public string ClubId { get; set; }

        public ClubPeriodUpdateType UpdateType { get; set; }

        public DateTime UpdateTime { get; set; }

    }
}
