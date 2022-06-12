using Gyldendal.Api.CoreData.Contracts.Enumerations;

namespace Gyldendal.Api.CoreData.GqlToSolrConnector.Infrastructure
{
    public interface IGeneralSearchQueryBuilderFactory
    {
        ISolrQueryBuilder GetInstance(DataScope dataScope);
    }
}