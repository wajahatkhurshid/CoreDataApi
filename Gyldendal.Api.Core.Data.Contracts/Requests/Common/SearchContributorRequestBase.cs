using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Gyldendal.Api.CoreData.Contracts.Enumerations;

namespace Gyldendal.Api.CoreData.Contracts.Requests.Common
{
    public class SearchContributorRequestBase
    {
        /// <summary>
        /// Search String in Contributor Name
        /// </summary>
        public string SearchString { get; set; }

        /// <summary>
        /// Page Index
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// Page Size
        /// </summary>
        [Required]
        public int PageSize { get; set; }

        /// <summary>
        /// Contributor Type
        /// </summary>
        public List<ContributorType> ContributorType { get; set; }
    }
}
