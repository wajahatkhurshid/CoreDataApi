using Gyldendal.Api.CoreData.Gql.Common;
using Gyldendal.Api.CoreData.GqlValidator;
using Irony.Parsing.LINQ_Generator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Gyldendal.Api.CoreData.Tests.Gql.ExpressionValidatorTests
{
    [TestClass]
    public class ExpressionValidatorTests
    {
        [DataTestMethod]
        [DataRow("Area(Islamabad) and oneperwork() and media(Bog) and not media(e-bog) and not media(lydbog) and not materialtype(paperback)")]
        [DataRow("Area(Islamabad) and First(9788123456789) and media(Bog) and not media(e-bog) and not media(lydbog) and not materialtype(paperback)")]
        public void ExpressionValidator_Validate_PostProcessingTokenGql(string gql)
        {
            var validator = new ExpressionValidator();
            var output = validator.Validate(gql);
            Assert.IsTrue(output.Result.IsValidated);
            Assert.IsTrue(output.PostProcessTokens.Count > 0);
        }
    }
}