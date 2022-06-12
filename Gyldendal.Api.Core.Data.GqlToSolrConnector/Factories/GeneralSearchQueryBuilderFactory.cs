using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.GqlToSolrConnector.Infrastructure;

namespace Gyldendal.Api.CoreData.GqlToSolrConnector.Factories
{
    public class GeneralSearchQueryBuilderFactory : IGeneralSearchQueryBuilderFactory
    {
        public ISolrQueryBuilder GetInstance(DataScope dataScope)
        {
            switch (dataScope)
            {
                case DataScope.GyldendalPlus:
                    return new SearchRepositories.GPlus.GeneralSearchSolrQueryBuilder();

                default:
                    return new SearchRepositories.Common.GeneralSearchSolrQueryBuilder();
            }
        }
    }
}