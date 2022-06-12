using Gyldendal.Api.CoreData.GqlToSolrConnector.Model;

namespace Gyldendal.Api.CoreData.GqlToSolrConnector.Infrastructure
{
    public interface ISolrQueryEngineFactory
    {
        ISolrQueryEngine GetInstance(bool useGqlExpressionTree);
    }
}