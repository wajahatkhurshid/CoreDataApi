using System.Collections.Generic;
using SolrNet;

namespace Gyldendal.Api.CoreData.GqlToSolrConnector.QueryParsing
{
    internal interface IQueryParser
    {
        ISolrQuery ParseExpression(string expression, Dictionary<string, string> searchCriteria);
    }
}