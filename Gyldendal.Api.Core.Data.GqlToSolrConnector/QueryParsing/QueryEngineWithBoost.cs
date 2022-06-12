using System;
using System.Collections.Generic;
using System.Linq;
using Gyldendal.Api.CoreData.GqlToSolrConnector.CriteriaExtraction;
using Gyldendal.Api.CoreData.GqlToSolrConnector.Model;
using SolrNet;

namespace Gyldendal.Api.CoreData.GqlToSolrConnector.QueryParsing
{
    /// <summary>
    /// Converts isbn expresion to solr query and adds field boosting factor
    /// </summary>
    internal class QueryEngineWithBoost : BaseQueryEngine, IQueryParser
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="tokenToCriteriaConverter">instance of parser implementation through
        /// dependency injection</param>
        public QueryEngineWithBoost(ITokenToCriteriaConverter tokenToCriteriaConverter) : base(tokenToCriteriaConverter)
        {
        }

        /// <summary>
        /// Implementation of the interface, it is the core method
        /// which parses expression and converts it to solr query.
        /// </summary>
        /// <param name="expression">isbn expression</param>
        /// <param name="searchCriteria"></param>
        /// <returns>solr query</returns>
        public ISolrQuery ParseExpression(string expression, Dictionary<string, string> searchCriteria)
        {
            try
            {
                var boostCount = expression.Split('+').Length - 1;
                var opCode = ' ';
                var prevOpCode = ' ';
                AbstractSolrQuery searchQuery = null;
                var delimiters = new[] {'+', '-', '&'};
                var tokenStack = new Stack<ParseToken>();
                while (expression.Length > 0)
                {
                    var index = FindConsecutiveCloseBraces(expression);
                    var token = expression.Substring(0, index + 1);
                    expression = expression.Remove(0, index + 1);
                    var opIndex = expression.IndexOfAny(delimiters);
                    if (opIndex >= 0)
                    {
                        opCode = expression[0];
                        expression = expression.Remove(0, 1);
                    }
                    tokenStack.Push(new ParseToken
                    {
                        Token = token,
                        OpCode = prevOpCode,
                        BoostValue = boostCount--
                    });
                    prevOpCode = opCode;
                }

                while (tokenStack.Count() != 0)
                {
                    searchQuery = AddQuery(searchQuery, tokenStack.Pop());
                }

                return searchQuery;
            }
            catch (Exception)
            {
                // ignored
            }
            return null;
        }

        private int FindConsecutiveCloseBraces(string expression)
        {
            var index = expression.IndexOf(')') + 1;
            while (index < expression.Length)
            {
                if (expression[index] != ')')
                    break;
                index++;
            }
            return index - 1;
        }
    }
}