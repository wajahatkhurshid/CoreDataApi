using System.Collections.Generic;
using Gyldendal.Api.CoreData.GqlToSolrConnector.CriteriaExtraction;
using Gyldendal.Api.CoreData.GqlToSolrConnector.Model;
using SolrNet;

namespace Gyldendal.Api.CoreData.GqlToSolrConnector.QueryParsing
{
    /// <summary>
    /// Convert Irony parsed tokens into Solr Query without adding a boost value
    /// </summary>
    public class QueryEngineNoBoost : BaseQueryEngine, IQueryParser
    {
        public QueryEngineNoBoost(ITokenToCriteriaConverter parser) : base(parser)
        {
        }

        /// <summary>
        /// This method converts isbn epxression to solr query.
        /// It parses isbn expression and separates methods and operators called tokens.
        /// The tokens are further processed by IParser object and converted to solr query object.
        /// </summary>
        /// <param name="expression">isbn expression</param>
        /// <param name="defaultCriteria"></param>
        /// <returns>Solr query object</returns>
        public ISolrQuery ParseExpression(string expression, Dictionary<string, string> defaultCriteria)
        {
            AbstractSolrQuery solrQuery = null;

            foreach (var criteria in defaultCriteria)
            {
                solrQuery = solrQuery && new SolrQueryByField(criteria.Key, criteria.Value) { Quoted = false };
            }

            DoParseExpression(ref solrQuery, expression);

            return solrQuery;
        }

        private void DoParseExpression(ref AbstractSolrQuery baseQuery, string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
            {
                baseQuery = SolrQuery.All;
                return;
            }

            var opCode = ' ';
            var prevOpCode = ' ';
            var delimiters = new[] { '+', '-', '&' };
            while (expression.Length > 0)
            {
                var index = expression.IndexOf(')');
                var token = expression.Substring(0, index + 1);
                expression = expression.Remove(0, index + 1);
                var opIndex = expression.IndexOfAny(delimiters);
                if (opIndex >= 0)
                {
                    opCode = expression[0];
                    expression = expression.Remove(0, 1);
                }
                baseQuery = AddQuery(baseQuery, new ParseToken
                {
                    Token = token,
                    OpCode = prevOpCode,
                    BoostValue = -1
                });
                prevOpCode = opCode;
            }
        }
    }
}