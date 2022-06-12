using Gyldendal.Api.CoreData.GqlToSolrConnector.Model;
using SolrNet;
using System.Collections.Generic;
using System.Linq;

namespace Gyldendal.Api.CoreData.GqlToSolrConnector.SearchRepositories.Common
{
    public abstract class BaseSolrQueryBuilder
    {
        protected List<SolrFieldQueryParams> SearchMatrix;

        public virtual AbstractSolrQuery Build(string gqlQueryValue)
        {
            return new SolrMultipleCriteriaQuery(SearchMatrix.Select(x => x.GetBoostedQuery(gqlQueryValue)).ToArray(), "OR");
        }
    }
}