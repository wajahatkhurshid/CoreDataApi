using System.Collections.Generic;
using Gyldendal.Api.CoreData.Contracts.Models;

namespace Gyldendal.Api.CoreData.Contracts.Response
{
    /// <summary>
    /// Search Contributor Response Model
    /// </summary>
    public class SearchContributorResponse<T> where T : BaseContributorDetails
    {
        /// <summary>
        /// List of contributors
        /// </summary>
        public IEnumerable<T> Contributors { get; set; }

        /// <summary>
        /// 0 based page index
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// Page size for required no. of records
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Total records fetched against the request
        /// </summary>
        public int TotalRecords { get; set; }
    }
}
