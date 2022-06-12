using System.Collections.Generic;
using System.Linq;
using Gyldendal.Api.CoreData.GqlToSolrConnector.Model;
using CommonServiceLocator;
using NewRelic.Api.Agent;
using SolrNet;
using SolrNet.Commands.Parameters;

// ReSharper disable UnusedAutoPropertyAccessor.Global

// ReSharper disable once UnusedAutoPropertyAccessor.Global

namespace Gyldendal.Api.CoreData.GqlToSolrConnector.SolrSearch
{
    //Call Solr with formed query and return results
    public class GenericSolrSearch<T> : ISearch<T>
    {
        private readonly ISolrOperations<T> _solrInstance;

        private const int MaxResults = 50000;

        public string SolrCoreName { get; private set; }

        public string SolrServerUrl { get; private set; }

        public GenericSolrSearch(string coreName, string solrServerUrl)
        {
            SolrCoreName = coreName;
            SolrServerUrl = solrServerUrl;

            SolrConnectionManager.InitConnection<T>(solrServerUrl, coreName);
            _solrInstance = ServiceLocator.Current.GetInstance<ISolrOperations<T>>();
        }

        #region ISearch<T> Implementation

        /// <summary>
        /// This method queries solr server and returns result. query object is passed
        /// in as input.
        /// </summary>
        /// <param name="searchQuery">solr query object</param>
        /// <param name="start">number of records to skip</param>
        /// <param name="rows">number of records to fetch</param>
        /// <param name="orderByParams"></param>
        /// <param name="groupParameters">grouping information</param>
        /// <param name="facetParameters"></param>
        /// <param name="filterParameters"></param>
        /// <param name="extraParams"></param>
        /// <returns>products returned by solr</returns>
        [Trace]
        public SearchResult<T> Search(ISolrQuery searchQuery, int start, int rows, List<OrderByParam> orderByParams = null, GroupingParameters groupParameters = null,
            FacetParameters facetParameters = null, ICollection<ISolrQuery> filterParameters = null,
            IEnumerable<KeyValuePair<string, string>> extraParams = null)
        {
            var queryResults = ExecuteSolrQuery(searchQuery, start, rows, orderByParams, groupParameters,
                facetParameters, filterParameters, extraParams, null);

            var result = new SearchResult<T>(queryResults);
            return result;
        }

        /// <summary>
        /// This method queries solr server and returns result. query object is passed
        /// in as input.
        /// </summary>
        /// <param name="searchQuery">solr query object</param>
        /// <param name="start">number of records to skip</param>
        /// <param name="rows">number of records to fetch</param>
        /// <param name="orderByParams"></param>
        /// <param name="groupParameters">grouping information</param>
        /// <param name="facetParameters"></param>
        /// <param name="filterParameters"></param>
        /// <param name="extraParams"></param>
        /// <param name="fields"></param>
        /// <returns>products returned by solr</returns>
        [Trace]
        public SolrQueryResults<T> SearchSolrQueryResults(ISolrQuery searchQuery, int start, int rows, List<OrderByParam> orderByParams = null, GroupingParameters groupParameters = null,
            FacetParameters facetParameters = null, ICollection<ISolrQuery> filterParameters = null,
            IEnumerable<KeyValuePair<string, string>> extraParams = null, string[] fields = null)
        {
            var queryResults = ExecuteSolrQuery(searchQuery, start, rows, orderByParams, groupParameters, facetParameters, filterParameters, extraParams, fields);

            return queryResults;
        }

        private SolrQueryResults<T> ExecuteSolrQuery(ISolrQuery searchQuery, int start, int rows, List<OrderByParam> orderByParams,
            GroupingParameters groupParameters, FacetParameters facetParameters, ICollection<ISolrQuery> filterParameters,
            IEnumerable<KeyValuePair<string, string>> extraParams, string[] fields)
        {
            ICollection<SortOrder> orderCollection = null;
            if (orderByParams != null && orderByParams.Any())
            {
                orderCollection = orderByParams.Select(x => x.ToSortOrder()).ToList();
            }

            var numberOfRows = rows;
            if (groupParameters != null || numberOfRows == 0)
            {
                numberOfRows = rows == 0 ? MaxResults : rows;
            }

            if (extraParams != null)
            {
                searchQuery = new LocalParams() {{"type", "dismax"}} + searchQuery;
            }

            var queryOptions = new QueryOptions
            {
                Rows = numberOfRows,
                StartOrCursor = new StartOrCursor.Start(start),
                OrderBy = orderCollection,
                Grouping = groupParameters,
                Facet = facetParameters,
                FilterQueries = filterParameters,
                ExtraParams = extraParams
            };

            if (fields != null && fields.Length > 0)
                queryOptions.Fields = fields;

            var queryResults = _solrInstance.Query(searchQuery, queryOptions);
            return queryResults;
        }

        #endregion ISearch<T> Implementation
    }
}