using System.Collections.Generic;
using Irony.Parsing.LINQ_Generator;

namespace Gyldendal.Api.CoreData.Gql.Common
{
    /// <summary>
    /// Parsing result from expression
    /// </summary>
    public class ParsedUnits
    {
        /// <summary>
        /// type of token i.e. First, OnePerWork etc
        /// </summary>
        public TokenType Type { get; set; }

        /// <summary>
        /// method parameters
        /// </summary>
        public List<string> Parameters { get; set; }

        public GqlExpression GqlExpression { get; set; }

        /// <summary>
        /// intialize variables to default values
        /// </summary>
        public ParsedUnits()
        {
            Type = TokenType.None;
            Parameters = new List<string>();
        }
    }
}