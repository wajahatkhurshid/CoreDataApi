using System;

namespace Gyldendal.Api.CoreData.Contracts.Models.Club
{
    public class ClubPeriod
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Date until user can cancel book of month.
        /// </summary>
        public DateTime CancellationDeadline { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public Club Club { get; set; }

        public ClubBranch ClubBranch { get; set; }

        public Product Product { get; set; }
    }
}