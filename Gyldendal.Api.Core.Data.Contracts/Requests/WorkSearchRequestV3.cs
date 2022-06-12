using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using System.Collections.Generic;
using Gyldendal.Api.CoreData.Contracts.Models;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Gyldendal.Api.CoreData.Contracts.Requests
{
    public class WorkSearchRequestV3
    {
        public WebShop CallingWebShop { get; set; }

        /// <summary>
        /// The web shops whose products to be searched
        /// </summary>
        public WebShop[] PrimaryWebShops { get; set; }

        /// <summary>
        /// The web shops whose products will be included in works searched using PrimaryWebShops
        /// </summary>
        public WebShop[] SecondaryWebShops { get; set; }

        /// <summary>
        /// GQL to search products in Solr
        /// </summary>
        public string Gql { get; set; } = "";

        /// <summary>
        /// Solr Filters
        /// </summary>
        public Dictionary<FilterType, List<string>> Filters { get; set; }

        public List<PriceRange> PriceRangeFilters { get; set; }

        /// <summary>
        /// Solr Facets for query
        /// </summary>
        public List<FacetType> FacetTypes { get; set; }

        /// <summary>
        /// Input settings for PriceRange, in case faceting required on Price Range.
        /// </summary>
        public PriceRangeFacetParams PriceRangeFacetParams { get; set; }

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