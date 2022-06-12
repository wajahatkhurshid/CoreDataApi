using System.Collections.Generic;
using Gyldendal.Api.CoreData.GqlToSolrConnector.Model;
using SolrNet;

namespace Gyldendal.Api.CoreData.GqlToSolrConnector.Filtering
{
    public interface IFiltering
    {
        ICollection<ISolrQuery> Build(IEnumerable<FilterInfo> filters);
    }
}
