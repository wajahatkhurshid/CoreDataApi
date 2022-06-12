using Gyldendal.Api.CoreData.Gql.Common;
using Irony.Parsing.LINQ_Generator;

namespace Gyldendal.Api.CoreData.GqlToSolrConnector.Model
{
    public class ParseToken
    {
        /// <summary>
        /// token of isbn query
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// operational code i.e. and,or, and not
        /// </summary>
        public char OpCode { get; set; }

        /// <summary>
        /// solr field boosting value
        /// </summary>
        public double BoostValue { get; set; }

        public GqlExpression GqlExpression { get; set; }
    }
}