using Gyldendal.Api.CoreData.GqlToSolrConnector.CriteriaExtraction;
using Gyldendal.Api.CoreData.GqlToSolrConnector.Model;
using Gyldendal.Api.CoreData.GqlToSolrConnector.QueryParsing;
using Irony.Parsing.LINQ_Generator;
using NewRelic.Api.Agent;
using SolrNet;
using System.Collections.Generic;
using Gyldendal.Api.CoreData.Gql.Common;

namespace Gyldendal.Api.CoreData.GqlToSolrConnector.QueryParsingV3
{
    public class SolrQueryEngine : BaseQueryEngine, ISolrQueryEngine
    {
        private bool _queryBoost;

        private int _boostValue = -1;

        public SolrQueryEngine(ITokenToCriteriaConverter tokenToCriteriaConverter)
            : base(tokenToCriteriaConverter)
        {
        }

        [Trace]
        public AbstractSolrQuery ParseExpression(ValidationResult result, Dictionary<string, string> defaultCriteria, bool queryBoost = false, int boostValue = -1)
        {
            AbstractSolrQuery solrQuery = null;

            _boostValue = boostValue;
            _queryBoost = queryBoost;
            foreach (var criteria in defaultCriteria)
            {
                solrQuery = solrQuery && new SolrQueryByField(criteria.Key, criteria.Value) { Quoted = false };
            }

            DoParseExpression(ref solrQuery, result.GqlExpressionTree);

            return solrQuery;
        }

        [Trace]
        private void DoParseExpression(ref AbstractSolrQuery baseQuery, Node gqlExpressionsTree)
        {
            if (gqlExpressionsTree == null)
            {
                baseQuery = SolrQuery.All;
                return;
            }

            if (baseQuery == null)
                baseQuery = Evaluate(gqlExpressionsTree);
            else
                baseQuery &= Evaluate(gqlExpressionsTree);
        }

        private AbstractSolrQuery Evaluate(Node node)
        {
            if (node.Value.Type == GqlType.Function)
                return AddQuery(null, new ParseToken
                {
                    Token = node.Value.Name,
                    GqlExpression = node.Value,
                    OpCode = '.',
                    //Boost for the next Gql expression is less than the last one.
                    BoostValue = _queryBoost ? _boostValue-- : -1
                });
            else
            {
                var a = Evaluate(node.Left);
                var b = Evaluate(node.Right);
                switch (node.Value.Name)
                {
                    case "+":
                    case " ":
                        return a | b;

                    case "-":
                        return a - b;

                    case "&":
                        return a & b;
                }
                return new SolrMultipleCriteriaQuery(new[] { a, b }, node.Value.Name);
            }
        }
    }
}