using Gyldendal.Api.CoreData.GqlToSolrConnector.CriteriaExtraction;
using Gyldendal.Api.CoreData.GqlToSolrConnector.Model;
using Gyldendal.Api.CoreData.GqlToSolrConnector.QueryParsing;
using Irony.Parsing.LINQ_Generator;
using NewRelic.Api.Agent;
using SolrNet;
using System.Collections.Generic;
using System.Linq;
using Gyldendal.Api.CoreData.Gql.Common;

namespace Gyldendal.Api.CoreData.GqlToSolrConnector.QueryParsingV2
{
    public class SolrQueryEngine : BaseQueryEngine, ISolrQueryEngine
    {
        public SolrQueryEngine(ITokenToCriteriaConverter tokenToCriteriaConverter)
            : base(tokenToCriteriaConverter)
        {
        }

        [Trace]
        public AbstractSolrQuery ParseExpression(ValidationResult result, Dictionary<string, string> defaultCriteria, bool queryBoost = false, int boostValue = -1)
        {
            AbstractSolrQuery solrQuery = null;

            foreach (var criteria in defaultCriteria)
            {
                solrQuery = solrQuery && new SolrQueryByField(criteria.Key, criteria.Value) { Quoted = false };
            }

            DoParseExpression(ref solrQuery, result.GqlExpressions, queryBoost);

            return solrQuery;
        }

        [Trace]
        private void DoParseExpression(ref AbstractSolrQuery baseQuery,
            IReadOnlyCollection<GqlExpression> gqlExpressions, bool queryBoost)
        {
            if (gqlExpressions == null || !gqlExpressions.Any())
            {
                baseQuery = SolrQuery.All;
                return;
            }

            var boostValue = -1;
            if (queryBoost)
            {
                boostValue = gqlExpressions.Count(x => x.Name == "+");
            }
            var prevOpCode = ' ';
            foreach (var gqlExpression in gqlExpressions)
            {
                if (gqlExpression.Type == GqlType.Operator)
                {
                    prevOpCode = gqlExpression.Name[0];
                    continue;
                }
                baseQuery = AddQuery(baseQuery, new ParseToken
                {
                    Token = gqlExpression.Name,
                    GqlExpression = gqlExpression,
                    OpCode = prevOpCode,
                    //Boost for the next Gql expression is less than the last one.
                    BoostValue = queryBoost ? boostValue-- : boostValue
                });
            }
        }
    }
}