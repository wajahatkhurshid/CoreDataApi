using System;
using System.Collections.Generic;
using System.Linq;
using Gyldendal.Api.CoreData.SolrContracts.Product;
using NewRelic.Api.Agent;
using SolrNet;

namespace Gyldendal.Api.CoreData.GqlToSolrConnector.Model
{
    /// <summary>
    /// Processes result returned by solr
    /// </summary>
    /// <typeparam name="T">solr index</typeparam>
    public class SearchResult<T>
    {
        /// <summary>
        /// Constructor for SearchResult, initialization if required
        /// </summary>
        /// <param name="solrResults"></param>
        public SearchResult(SolrQueryResults<T> solrResults)
        {
            if (solrResults != null)
            {
                Initialize(solrResults);
            }
        }

        /// <summary>
        /// converts solr query result to SearchResult class.
        /// Also extracts spell checking information.
        /// </summary>
        /// <param name="solrResults"></param>
        [Trace]
        private void Initialize(SolrQueryResults<T> solrResults)
        {
            TotalResults = solrResults.NumFound;

            if (solrResults.Grouping != null && solrResults.Grouping.Count > 0)
            {
                solrResults.Clear();
                TotalResults = 0;
                foreach (var group in solrResults.Grouping)
                {
                    foreach (var products in @group.Value.Groups.Select(item => item.Documents.ToList()))
                    {
                        solrResults.AddRange(products);
                    }
                    TotalResults += group.Value.Ngroups.GetValueOrDefault(0);
                }
            }
            ItemsFound = solrResults;

            Facets = solrResults.FacetFields.ToDictionary(facet => facet.Key, facet => facet.Value);

            if (solrResults.FacetQueries.Any(a=>a.Key.Contains(ProductSchemaField.DefaultPrice.GetFieldName())))
            {
                ICollection<KeyValuePair<string, int>> facetQueries = solrResults.FacetQueries
                    .Select(query => new KeyValuePair<string, int>(query.Key, query.Value)).ToList();

                Facets.Add(ProductSchemaField.DefaultPrice.GetFieldName(), facetQueries);
            }

            if (solrResults.SpellChecking == null || solrResults.SpellChecking.Count == 0)
            {
                return;
            }

            DidYouMean = new List<string>();
            var suggestions = solrResults.SpellChecking.Select(s => s.Suggestions).FirstOrDefault();
            if (suggestions != null)
            {
                DidYouMean = suggestions.Where(s => !string.IsNullOrEmpty(s)).ToArray();
            }
        }

        /// <summary>
        /// facets and their values
        /// </summary>
        public Dictionary<string, ICollection<KeyValuePair<string, int>>> Facets { get; private set; }

        /// <summary>
        /// items found by solr
        /// </summary>
        public SolrQueryResults<T> ItemsFound { get; set; }

        /// <summary>
        /// total results found
        /// </summary>
        public int TotalResults { get; set; }

        /// <summary>
        /// count of gyldendal product in main result
        /// </summary>
        public int GyldendalProductCount { get; set; }

        /// <summary>
        /// count of non-gyldendal products
        /// </summary>
        public int ExternalProductCount { get; set; }

        /// <summary>
        /// contains information about spell checking
        /// </summary>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public ICollection<string> DidYouMean { get; set; }

        public IDictionary<string, GroupedResults<T>> GroupedResults;
    }
}