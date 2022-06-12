using System.Collections.Generic;
using System.Linq;
using Gyldendal.Api.CoreData.Gql.Common;
using Gyldendal.Api.CoreData.GqlToSolrConnector;
using Gyldendal.Api.CoreData.GqlToSolrConnector.CriteriaExtraction;
using Gyldendal.Api.CoreData.GqlToSolrConnector.Model;
using Gyldendal.Api.CoreData.GqlValidator;
using Gyldendal.Api.CoreData.SolrDataProviders.Mappings;
using Irony.Parsing.LINQ_Generator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolrQueryEngineExpressionTree = Gyldendal.Api.CoreData.GqlToSolrConnector.QueryParsingV3.SolrQueryEngine;

namespace Gyldendal.Api.CoreData.Tests.Gql.SolrQueryEngine
{
    [TestClass]
    public class ExpressionTreeParsingTests
    {
        private ISolrQueryEngine GetSolrQueryEngine() => new SolrQueryEngineExpressionTree(_tokenToCriteriaConverter);

        private ValidationResult _result;

        private GqlTokenToCriteriaConverter _tokenToCriteriaConverter;

        private Dictionary<string, string> _defaultCriteria;

        private int GetBoostValue() => _result.GqlExpressions?.Count(x => x.Type == GqlType.Operator) ?? -1;

        private void VerifyGeneratedSolrQueryFromGql(string gql, string finalSolrQuery, bool boosted, int treeDepth)
        {
            var syntaxChecker = new SyntaxChecker();

            var result = syntaxChecker.Parse(gql);
            Assert.IsTrue(result);

            var depth = MaxDepth(syntaxChecker.GqlExpressionTree);

            Assert.AreEqual(depth, treeDepth);

            _result.GqlExpressions = syntaxChecker.GqlExpression;
            _result.GqlExpressionTree = syntaxChecker.GqlExpressionTree;
            var solrQueryEngine = GetSolrQueryEngine();
            var solrQuery = solrQueryEngine.ParseExpression(_result, _defaultCriteria, boosted, GetBoostValue());
            var serializeQuery = solrQuery.Serialize();

            Assert.AreEqual(finalSolrQuery, serializeQuery);
        }

        private static int MaxDepth(Node root)
        {
            if (root == null)
                return 0;

            var leftDepth = MaxDepth(root.Left);
            var rightDepth = MaxDepth(root.Right);

            return leftDepth > rightDepth ? leftDepth + 1 : rightDepth + 1;
        }

        [TestInitialize]
        public void Init()
        {
            _tokenToCriteriaConverter = new GqlTokenToCriteriaConverter(GqlToSolrFieldMapping.GetMappings());
            _result = new ValidationResult();
            _defaultCriteria = new Dictionary<string, string>();
        }

        [DataTestMethod]
        [DataRow("ISBN(9788714118167)", "isbn13:(9788714118167)", false, 1)]
        [DataRow("area(abc)", "areas:(\"abc\")", false, 1)]
        public void SingleFunctionGqlNoBoost_GenerateExpectedSolrQuery(string gql, string finalSolrQuery, bool boosted, int treeDepth)
        {
            VerifyGeneratedSolrQueryFromGql(gql, finalSolrQuery, boosted, treeDepth);
        }

        [DataTestMethod]
        [DataRow("ISBN(9788714118167)", "(isbn13:(9788714118167))^0", true, 1)]
        [DataRow("area(abc)", "(areas:(\"abc\"))^0", true, 1)]
        public void SingleFunctionGqlWithBoost_GenerateExpectedSolrQuery(string gql, string finalSolrQuery, bool boosted, int treeDepth)
        {
            VerifyGeneratedSolrQueryFromGql(gql, finalSolrQuery, boosted, treeDepth);
        }

        [DataTestMethod]
        [DataRow("ISBN(9788714118167) OR Area(abc)", "((isbn13:(9788714118167))^1 OR (areas:(\"abc\"))^0)", true, 2)]
        [DataRow("ISBN(9788714118167) OR Imprint(abc)", "((isbn13:(9788714118167))^1 OR (imprint:(abc))^0)", true, 2)]
        public void MultipleFunctionGqlWithBoost_GenerateExpectedSolrQuery(string gql, string finalSolrQuery, bool boosted, int treeDepth)
        {
            VerifyGeneratedSolrQueryFromGql(gql, finalSolrQuery, boosted, treeDepth);
        }

        [DataTestMethod]
        [DataRow("ISBN(9788714118167) OR Area(abc) AND Imprint(Stereo)", "((isbn13:(9788714118167))^2 OR ((areas:(\"abc\"))^1 AND (imprint:(Stereo))^0))", true, 3)]
        [DataRow("ISBN(9788714118167) OR Area(abc) AND Imprint(Stereo) OR SubArea(Kashmir)", "(((isbn13:(9788714118167))^3 OR ((areas:(\"abc\"))^2 AND (imprint:(Stereo))^1)) OR (subAreas:(\"Kashmir\"))^0)", true, 4)]
        public void MultipleFunctionGqlWithBoost_GenerateExpectedSolrQueryWIthOperatorPrecedence(string gql, string finalSolrQuery, bool boosted, int treeDepth)
        {
            VerifyGeneratedSolrQueryFromGql(gql, finalSolrQuery, boosted, treeDepth);
        }

        [DataTestMethod]
        [DataRow("Area(abc) AND (isPhysical(true) OR Imprint(Stereo Imprint))", "((areas:(\"abc\"))^2 AND ((isPhysical:(true))^1 OR (imprint:(Stereo\\ Imprint))^0))", true, 3)]
        [DataRow("Area(abc) AND (isPhysical(true) OR Imprint(Stereo Imprint)) OR ISBN(9788702048018)", "(((areas:(\"abc\"))^3 AND ((isPhysical:(true))^2 OR (imprint:(Stereo\\ Imprint))^1)) OR (isbn13:(9788702048018))^0)", true, 4)]
        public void ComplexFunctionGqlWithBoost_GenerateExpectedSolrQueryWithOperatorPrecedence(string gql, string finalSolrQuery, bool boosted, int treeDepth)
        {
            VerifyGeneratedSolrQueryFromGql(gql, finalSolrQuery, boosted, treeDepth);
        }
    }
}