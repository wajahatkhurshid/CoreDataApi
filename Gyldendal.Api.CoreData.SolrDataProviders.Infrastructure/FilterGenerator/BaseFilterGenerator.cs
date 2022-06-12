using System.Collections.Generic;
using Gyldendal.Api.CoreData.GqlToSolrConnector.Model;
using Gyldendal.Api.CoreData.SolrDataProviders.Infrastructure.Models;

namespace Gyldendal.Api.CoreData.SolrDataProviders.Infrastructure.FilterGenerator
{
    public abstract class BaseFilterGenerator<T> where T : FilterGenerationInput
    {
        protected T Input;

        protected List<FilterInfo> Filters = new List<FilterInfo>();

        public IEnumerable<FilterInfo> Generate(T input)
        {
            Input = input;

            DoGenerate();

            return Filters;
        }

        protected abstract void DoGenerate();

        protected FilterInfo GetSolrConnectorFilterInfo(string solrFieldName, IEnumerable<string> filterValues,
            bool excludeFromFacets, bool quoted = true, IEnumerable<FilterInfo> nestedFilters = null)
        {
            return new FilterInfo
            {
                SolrFieldName = solrFieldName,
                FilterValues = filterValues,
                ExcludeFromFacets = excludeFromFacets,
                Quoted = quoted,
                NestedFilters = nestedFilters
            };
        }
    }
}