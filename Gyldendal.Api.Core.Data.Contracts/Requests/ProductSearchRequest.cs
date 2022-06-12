using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Contracts.Enumerations;

namespace Gyldendal.Api.CoreData.Contracts.Requests
{
    public class ProductSearchRequest
    {
        /// <summary>
        /// The web shop whose product to be searched, None means for all
        /// </summary>
        public WebShop Webshop { get; set; }

        /// <summary>
        /// GQL to search products in Solr
        /// </summary>
        public string Gql { get; set; } = "";

        /// <summary>
        /// Solr Filters
        /// </summary>
        public FilterInfo Filters { get; set; }

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
