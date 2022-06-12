using Gyldendal.Api.CoreData.Gql.Common;
using Gyldendal.Api.CoreData.GqlValidator;
using Irony.Parsing.LINQ_Generator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Gyldendal.Api.CoreData.Tests.Gql.SyntaxCheckerTest
{
    [TestClass]
    public class ExpressionListParsingTests
    {
        [DataTestMethod]
        [DataRow("ISBN(9788714118167)", "isbn(9788714118167)", 0, 1)]
        [DataRow("Area(abc)", "area(abc)", 0, 1)]
        public void SingleFunctionGql_VerifyParsedGql(string gql, string parsedGql, int numberOfOperators, int numberOfOperands)
        {
            VerifyParsedGql(gql, parsedGql, numberOfOperators, numberOfOperands);
        }

        [DataTestMethod]
        [DataRow("ISBN(9788714118167) OR Area(abc)", "isbn(9788714118167)+area(abc)", 1, 2)]
        [DataRow("ISBN(9788714118167) OR Imprint(abc)", "isbn(9788714118167)+imprint(abc)", 1, 2)]
        public void MultipleFunctionGql_VerifyParsedGqlNoOperatorPrecedence(string gql, string parsedGql, int numberOfOperators, int numberOfOperands)
        {
            VerifyParsedGql(gql, parsedGql, numberOfOperators, numberOfOperands);
        }

        [DataTestMethod]
        [DataRow("publication_date(1, 30) AND (isPhysical(true) OR Imprint(Stereo Imprint))", "publication_date(1,30)&isphysical(true)+imprint(Stereo Imprint)", 2, 3)]
        [DataRow("ISBN(9788714118167) OR Imprint(abc)", "isbn(9788714118167)+imprint(abc)", 1, 2)]
        [DataRow("Area(Islamabad) and oneperwork() and media(Bog) and not media(e-bog) and not media(lydbog) and not materialtype(paperback)", "area(Islamabad)&oneperwork()&media(Bog)-media(e-bog)-media(lydbog)-materialtype(paperback)", 5, 6)]
        public void ComplexFunctionGql_VerifyParsedGqlNoOperatorPrecedence(string gql, string parsedGql, int numberOfOperators, int numberOfOperands)
        {
            VerifyParsedGql(gql, parsedGql, numberOfOperators, numberOfOperands);
        }

        private static void VerifyParsedGql(string gql, string parsedGql, int numberOfOperators, int numberOfOperands)
        {
            var syntaxChecker = new SyntaxChecker();
            var result = syntaxChecker.Parse(gql);
            Assert.IsTrue(result);

            var operands = syntaxChecker.GqlExpression.Where(x => x.Type == GqlType.Function).ToList();
            var operators = syntaxChecker.GqlExpression.Where(x => x.Type == GqlType.Operator).ToList();

            Assert.AreEqual(numberOfOperands, operands.Count);
            Assert.AreEqual(numberOfOperators, operators.Count);

            var actualParsedGql = syntaxChecker.GqlExpression.ToGqlString();
            Assert.AreEqual(parsedGql, actualParsedGql);
        }
    }
}