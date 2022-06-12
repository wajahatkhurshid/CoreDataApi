using Gyldendal.Api.CoreData.Gql.Common;
using Gyldendal.Api.CoreData.GqlToSolrConnector.Model;
using Irony.Parsing.LINQ_Generator;

namespace Gyldendal.Api.CoreData.GqlToSolrConnector.CriteriaExtraction
{
    public interface ITokenToCriteriaConverter
    {
        SearchCriteria ConvertToSearchCriteria(string token);

        SearchCriteria ConvertToSearchCriteria(GqlExpression token);
    }
}