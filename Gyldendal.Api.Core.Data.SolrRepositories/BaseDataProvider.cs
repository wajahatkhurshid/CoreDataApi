using SolrNet;
using System.Collections.Generic;
using Gyldendal.Api.CoreData.SolrDataProviders.Infrastructure;
using Gyldendal.Api.CoreData.SolrDataProviders.Infrastructure.FilterGenerator;
using Gyldendal.Api.CoreData.SolrDataProviders.Infrastructure.Models;

namespace Gyldendal.Api.CoreData.SolrDataProviders
{
    public class BaseDataProvider<T> where T : FilterGenerationInput
    {
        private readonly IFilterInfoToSolrQueryBuilder _solrQueryBuilder;

        private readonly IFilterGenerator<T> _filterGenerator;

        protected readonly string SolrServerUrl;

        protected readonly string CoreName;

        public BaseDataProvider(IFilterInfoToSolrQueryBuilder solrQueryBuilder, IFilterGenerator<T> filterGenerator, string solrServerUrl, string coreName)
        {
            _solrQueryBuilder = solrQueryBuilder;
            _filterGenerator = filterGenerator;
            SolrServerUrl = solrServerUrl;
            CoreName = coreName;
        }

        protected ICollection<ISolrQuery> GenerateSolrQuery(T input)
        {
            return _solrQueryBuilder.Build(_filterGenerator.Generate(input));
        }
    }
}