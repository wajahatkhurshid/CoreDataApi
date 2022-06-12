using System.Collections.Generic;
using Irony.Parsing.LINQ_Generator;

namespace Gyldendal.Api.CoreData.Gql.Common
{
    /// <summary>
    /// Result of validation from Irony Parser
    /// </summary>
    public class ValidationResult
    {
        public bool IsValidated { get; set; }

        public string Message { get; set; }

        public List<GqlExpression> GqlExpressions { get; set; }

        public List<GqlExpression> GqlExpressionWithParentheses { get; set; }

        public Node GqlExpressionTree { get; set; }
    }
}