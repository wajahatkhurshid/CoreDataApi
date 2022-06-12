using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Requests.Common;

namespace Gyldendal.Api.CoreData.Contracts.Requests
{
    /// <summary>
    /// Search Contributor Request Model
    /// </summary>
    public class SearchContributorRequest : SearchContributorRequestBase
    {
        /// <summary>
        /// Data Scope Id
        /// </summary>
        public DataScope DataScope { get; set; }
    }
}