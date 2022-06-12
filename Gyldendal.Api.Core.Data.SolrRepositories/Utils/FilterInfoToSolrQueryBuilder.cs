using System.Collections.Generic;
using System.Linq;
using Gyldendal.Api.CoreData.GqlToSolrConnector.Model;
using Gyldendal.Api.CoreData.SolrDataProviders.Infrastructure;
using SolrNet;

namespace Gyldendal.Api.CoreData.SolrDataProviders.Utils
{
    // TODO: 1. Manage class level state management.
    // 2. Throw argument null exception in case IEnumerable<FilterInfo> is null
    // 3. Breakdown private method

    public class FilterInfoToSolrQueryBuilder : IFilterInfoToSolrQueryBuilder
    {
        public ICollection<ISolrQuery> Build(IEnumerable<FilterInfo> filters)
        {
            var filterInfos = filters.ToList();
            if (!(filterInfos.Any()))
            {
                return null;
            }
            var solrQueryFilters = new List<ISolrQuery>();

            filterInfos.ToList().ForEach(x => AddFilterQuery(solrQueryFilters, x));

            return solrQueryFilters;
        }

        private static void AddFilterQuery(ICollection<ISolrQuery> solrQueryFilters, FilterInfo filterInfo)
        {
            if (string.IsNullOrWhiteSpace(filterInfo?.SolrFieldName) ||
                (filterInfo.FilterValues == null || !filterInfo.FilterValues.Any()))
            {
                return;
            }

            var solrQueries =
                filterInfo.FilterValues.Select(
                    filterValue =>
                        new SolrQueryByField(filterInfo.SolrFieldName, filterValue) { Quoted = filterInfo.Quoted });
            if (filterInfo.NestedFilters != null)
            {
                solrQueries = solrQueries.Union(filterInfo.NestedFilters.SelectMany(x => x.FilterValues.Select(
                    filterValue =>
                        new SolrQueryByField(x.SolrFieldName, filterValue) { Quoted = filterInfo.Quoted })));
            }

            var solrQueryByFields = solrQueries as SolrQueryByField[] ?? solrQueries.ToArray();

            if (solrQueryByFields.Count() > 1)
            {
                solrQueryFilters.Add(!filterInfo.ExcludeFromFacets
                    ? new SolrMultipleCriteriaQuery(solrQueryByFields, "OR")
                    : new SolrMultipleCriteriaQuery(solrQueryByFields, "AND"));
            }
            else
            {
                solrQueryFilters.Add(solrQueryByFields.First());
            }
        }
    }
}