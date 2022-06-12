using System.Collections.Generic;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Gql.Common;
using Gyldendal.Api.CoreData.GqlToSolrConnector.Model;

namespace Gyldendal.Api.CoreData.GqlToSolrConnector
{
    /// <summary>
    /// SearchRequest object for GqlToSolr layer. Core.Data.Contracts object must be mapped to this
    /// </summary>
    public class SearchRequest
    {
        public SearchRequest()
        {
            Filters = new List<FilterInfo>();
            DefaultSearchCriteria = new Dictionary<string, string>();
        }

        public SyntaxInfo GqlExpressionInfo { get; set; }

        public OrderBy OrderBy { get; set; }

        public SortBy SortBy { get; set; }

        public int Start { get; set; }

        public int Rows { get; set; }

        public Dictionary<string, string> DefaultSearchCriteria { get; private set; }

        public string GroupByField { get; set; }

        public List<FilterInfo> Filters { get; set; }

        /// <summary>
        /// Input settings for PriceRange, in case faceting required on Price Range.
        /// </summary>
        public PriceRangeFacetParams PriceRangeFacetParams { get; set; }

        public string[] FacetFields { get; set; }

        /// <summary>
        /// If true, then use gql expression tree for gql to solr query translation
        /// </summary>
        public bool UseGqlExpressionTree { get; set; } = false;
    }
}