using System.Collections.Generic;
using Gyldendal.Api.CoreData.GqlToSolrConnector.Model;
using SolrNet;
using SolrNet.Commands.Parameters;

namespace Gyldendal.Api.CoreData.GqlToSolrConnector.SolrSearch
{
    /// <summary>
    /// Interface used to perform search operation on solr
    /// </summary>
    /// <typeparam name="T">Index or core</typeparam>
    public interface ISearch<T>
    {
        /// <summary>
        /// interface method provides search access to solr
        /// </summary>
        /// <param name="searchQuery">solr query object</param>
        /// <param name="start">number of records to skip</param>
        /// <param name="rows">number of records to return</param>
        /// <param name="orderByParams">field to be used for order by and sort direction (ASC or DESC)</param>
        /// <param name="groupingParameters">adds grouping information to the query</param>
        /// <param name="facetParameters"></param>
        /// <param name="filterParameters"></param>
        /// <param name="extraParams"></param>
        /// <returns>products returned by query</returns>
        SearchResult<T> Search(ISolrQuery searchQuery, int start, int rows, List<OrderByParam> orderByParams = null,
            GroupingParameters groupingParameters = null, FacetParameters facetParameters = null,
            ICollection<ISolrQuery> filterParameters = null,
            IEnumerable<KeyValuePair<string, string>> extraParams = null);

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
        SolrQueryResults<T> SearchSolrQueryResults(ISolrQuery searchQuery, int start, int rows, List<OrderByParam> orderByParams = null,
            GroupingParameters groupParameters = null,
            FacetParameters facetParameters = null, ICollection<ISolrQuery> filterParameters = null,
            IEnumerable<KeyValuePair<string, string>> extraParams = null, string[] fields = null);
    }
}
