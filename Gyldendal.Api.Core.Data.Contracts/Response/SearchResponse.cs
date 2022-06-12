using System.Collections.Generic;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Gyldendal.Api.CoreData.Contracts.Response
{
    public class SearchResponse<T>
    {
        /// <summary>
        /// For every facet specified in the solr query, you receive a list count of products for each media type. For e.g. a facet mediaType
        /// returns values { e-bog : 121, i-bog : 100 } etc 
        /// </summary>
        public Dictionary<FacetType, List<KeyValuePair<string, int>>> Facets { get; set; }

        // If price range provided in input, price facets will be returned for given price range.
        public List<KeyValuePair<PriceRange, int>> PriceRangeFacet { get; set; }

        public Result<T> SearchResults { get; set; }
    }
}
