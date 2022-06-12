using Gyldendal.Api.CoreData.GqlToSolrConnector.Model;
using NewRelic.Api.Agent;
using SolrNet;
using System.Collections.Generic;
using System.Linq;

namespace Gyldendal.Api.CoreData.GqlToSolrConnector.Filtering
{
    public class FilterQueryBuilder : IFiltering
    {
        /// <summary>
        /// Create filter object for Solr
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        [Trace]
        public ICollection<ISolrQuery> Build(IEnumerable<FilterInfo> filters)
        {
            var filterInfos = filters as FilterInfo[] ?? filters.ToArray();
            if (!filterInfos.Any())
            {
                return null;
            }

            var solrQueryFilters = new List<ISolrQuery>();

            foreach (var filter in filterInfos)
            {
                if (filter.FilterValues.Any())
                    solrQueryFilters.Add(AddFilterQuery(filter));
            }

            return solrQueryFilters;
        }

        /// <summary>
        /// Create SolrQuery and add filter and tag
        /// </summary>
        /// <param name="filter"></param>
        [Trace]
        private static ISolrQuery AddFilterQuery(FilterInfo filter)
        {
            var solrQueries = filter.FilterValues
                .Select(filterValue => new SolrQueryByField(filter.SolrFieldName, filterValue) { Quoted = filter.Quoted }).ToList();

            if (filter.ExcludeFromFacets)
            {
                return (new LocalParams { { "tag", Constants.FilterExclTagName } } + new SolrMultipleCriteriaQuery(solrQueries, "OR"));
            }

            return new SolrMultipleCriteriaQuery(solrQueries, filter.QueryOperator);
        }
    }
}