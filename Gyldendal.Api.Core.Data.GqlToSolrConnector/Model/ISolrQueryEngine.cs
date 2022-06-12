using Gyldendal.Api.CoreData.Gql.Common;
using SolrNet;
using System.Collections.Generic;

namespace Gyldendal.Api.CoreData.GqlToSolrConnector.Model
{
    public interface ISolrQueryEngine
    {
        AbstractSolrQuery ParseExpression(ValidationResult result, Dictionary<string, string> defaultCriteria, bool queryBoost = false, int boostValue = -1);

        List<SearchCriteria> Tokens { get; }

        bool IsGeneralSearch { get; }

        bool IsWorkSearch { get; }

        bool IsRelatedProducts { get; }
    }
}