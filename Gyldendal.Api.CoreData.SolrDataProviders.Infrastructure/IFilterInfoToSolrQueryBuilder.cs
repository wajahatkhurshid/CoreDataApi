using System.Collections.Generic;
using Gyldendal.Api.CoreData.GqlToSolrConnector.Model;
using SolrNet;

namespace Gyldendal.Api.CoreData.SolrDataProviders.Infrastructure
{
    public interface IFilterInfoToSolrQueryBuilder
    {
        ICollection<ISolrQuery> Build(IEnumerable<FilterInfo> filters);
    }
}