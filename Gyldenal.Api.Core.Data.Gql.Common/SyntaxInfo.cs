using System.Collections.Generic;
using Irony.Parsing.LINQ_Generator;

namespace Gyldendal.Api.CoreData.Gql.Common
{
    /// <summary>
    /// Contains information about syntax validation of expression
    /// </summary>
    public class SyntaxInfo
    {
        /// <summary>
        /// True if expression is pre-validated
        /// else false
        /// </summary>
        public bool PostProcessingTokensExist { get; set; }

        /// <summary>
        /// method which satisfies pre-validation condition
        /// </summary>
        public string PostProcessingExpression { get; set; }

        public List<GqlExpression> PostProcessingExpressionGql { get; set; }

        /// <summary>
        /// List of pre validation tokens
        /// </summary>
        public List<ParsedUnits> PostProcessTokens { get; set; }

        /// <summary>
        /// expression with pre-validation method
        /// </summary>
        public List<GqlExpression> MainExpressionGql { get; set; }

        /// <summary>
        /// expression with pre-validation method
        /// </summary>
        public string MainExpression { get; set; }

        /// <summary>
        /// contains output of validation
        /// </summary>
        public ValidationResult Result { get; set; }

        /// <summary>
        /// constructor it initializes internal class objects
        /// </summary>
        public SyntaxInfo()
        {
            Result = new ValidationResult();
            PostProcessingTokensExist = false;
            Result.IsValidated = false;
            PostProcessTokens = new List<ParsedUnits>();
        }
    }
}