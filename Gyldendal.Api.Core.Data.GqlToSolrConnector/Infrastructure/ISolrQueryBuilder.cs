using SolrNet;

namespace Gyldendal.Api.CoreData.GqlToSolrConnector.Infrastructure
{
    public interface ISolrQueryBuilder
    {
        AbstractSolrQuery Build(string gqlQueryValue);
    }
}