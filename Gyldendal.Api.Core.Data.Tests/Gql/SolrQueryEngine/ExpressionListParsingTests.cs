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
using SolrQueryEngineSimple = Gyldendal.Api.CoreData.GqlToSolrConnector.QueryParsingV2.SolrQueryEngine;

namespace Gyldendal.Api.CoreData.Tests.Gql.SolrQueryEngine
{
    [TestClass]
    public class ExpressionListParsingTests
    {
        private ISolrQueryEngine GetSolrQueryEngine() => new SolrQueryEngineSimple(_tokenToCriteriaConverter);

        private ValidationResult _result;

        private GqlTokenToCriteriaConverter _tokenToCriteriaConverter;

        private Dictionary<string, string> _defaultCriteria;

        private int GetBoostValue() => _result.GqlExpressions?.Count(x => x.Type == GqlType.Operator) ?? -1;

        private void VerifyGeneratedSolrQueryFromGql(string gql, string finalSolrQuery, bool boosted)
        {
            var syntaxChecker = new SyntaxChecker();

            var result = syntaxChecker.Parse(gql);
            Assert.IsTrue(result);
            _result.GqlExpressions = syntaxChecker.GqlExpression;
            var solrQueryEngine = GetSolrQueryEngine();
            var solrQuery = solrQueryEngine.ParseExpression(_result, _defaultCriteria, boosted, GetBoostValue());
            var serializeQuery = solrQuery.Serialize();

            Assert.AreEqual(finalSolrQuery, serializeQuery);
        }

        [TestInitialize]
        public void Init()
        {
            _tokenToCriteriaConverter = new GqlTokenToCriteriaConverter(GqlToSolrFieldMapping.GetMappings());
            _result = new ValidationResult();
            _defaultCriteria = new Dictionary<string, string>();
        }

        [DataTestMethod]
        [DataRow("ISBN(9788714118167)", "isbn13:(9788714118167)", false)]
        [DataRow("area(abc)", "areas:(\"abc\")", false)]
        public void SingleFunctionGqlNoBoost_GenerateExpectedSolrQuery(string gql, string finalSolrQuery, bool boosted)
        {
            VerifyGeneratedSolrQueryFromGql(gql, finalSolrQuery, boosted);
        }

        [DataTestMethod]
        [DataRow("ISBN(9788714118167)", "(isbn13:(9788714118167))^0", true)]
        [DataRow("area(abc)", "(areas:(\"abc\"))^0", true)]
        public void SingleFunctionGqlWithBoost_GenerateExpectedSolrQuery(string gql, string finalSolrQuery, bool boosted)
        {
            VerifyGeneratedSolrQueryFromGql(gql, finalSolrQuery, boosted);
        }

        [DataTestMethod]
        [DataRow("ISBN(9788714118167) OR Area(abc)", "((isbn13:(9788714118167))^1 OR (areas:(\"abc\"))^0)", true)]
        [DataRow("ISBN(9788714118167) OR Imprint(abc)", "((isbn13:(9788714118167))^1 OR (imprint:(abc))^0)", true)]
        public void MultipleFunctionGqlWithBoost_GenerateExpectedSolrQuery(string gql, string finalSolrQuery, bool boosted)
        {
            VerifyGeneratedSolrQueryFromGql(gql, finalSolrQuery, boosted);
        }

        [DataTestMethod]
        [DataRow("ISBN(9788714118167) OR Area(abc) AND Imprint(Stereo)", "(((isbn13:(9788714118167))^1 OR (areas:(\"abc\"))^0) AND imprint:(Stereo))", true)]
        [DataRow("ISBN(9788714118167) OR Area(abc) AND Imprint(Stereo) OR SubArea(Kashmir)", "((((isbn13:(9788714118167))^2 OR (areas:(\"abc\"))^1) AND (imprint:(Stereo))^0) OR subAreas:(\"Kashmir\"))", true)]
        public void MultipleFunctionGqlWithBoost_GenerateExpectedSolrQueryNoOperatorPrecedence(string gql, string finalSolrQuery, bool boosted)
        {
            VerifyGeneratedSolrQueryFromGql(gql, finalSolrQuery, boosted);
        }

        [DataTestMethod]
        [DataRow("Area(abc) AND (isPhysical(true) OR Imprint(Stereo Imprint))", "(((areas:(\"abc\"))^1 AND (isPhysical:(true))^0) OR imprint:(Stereo\\ Imprint))", true)]
        [DataRow("Area(abc) AND (isPhysical(true) OR Imprint(Stereo Imprint)) OR ISBN(9788702048018)", "((((areas:(\"abc\"))^2 AND (isPhysical:(true))^1) OR (imprint:(Stereo\\ Imprint))^0) OR isbn13:(9788702048018))", true)]
        public void ComplexFunctionGqlWithBoost_GenerateExpectedSolrQueryNoOperatorPrecedence(string gql, string finalSolrQuery, bool boosted)
        {
            VerifyGeneratedSolrQueryFromGql(gql, finalSolrQuery, boosted);
        }
    }
}