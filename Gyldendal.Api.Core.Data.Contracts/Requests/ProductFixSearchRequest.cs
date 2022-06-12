using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using System.Collections.Generic;

namespace Gyldendal.Api.CoreData.Contracts.Requests
{
    /// <summary>
    /// Product Search Request Information container
    /// </summary>
    public class ProductFixSearchRequest
    {
        /// <summary>
        /// The web shop whose product to be searched, None means for all
        /// </summary>
        public List<WebShop> Webshop { get; set; }

        /// <summary>
        /// The search string which has to be searched against product title
        /// </summary>
        public string SearchString { get; set; }

        /// <summary>
        /// The type of product which has to be searched
        /// </summary>
        public ProductSearchType ProductSearchType { get; set; }

        /// <summary>
        /// The Paging information
        /// </summary>
        public PagingInfo Paging { get; set; }

        /// <summary>
        /// Order results by
        /// </summary>
        public OrderBy OrderBy { get; set; }

        /// <summary>
        /// The sorting order of the search result
        /// </summary>
        public SortBy SortBy { get; set; } = SortBy.Desc;
    }
}