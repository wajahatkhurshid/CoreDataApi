using System.Collections.Generic;
using System.Text.RegularExpressions;
using NewRelic.Api.Agent;

namespace Gyldendal.Api.CoreData.GqlToSolrConnector.Model
{
    public class FilterInfo
    {
        public string SolrFieldName { get; set; }

        public IEnumerable<string> FilterValues { get; set; }

        /// <summary>
        /// If true, then the filters will be excluded from facet query
        /// </summary>
        public bool ExcludeFromFacets { get; set; }

        /// <summary>
        /// To be used within Filter query for multiple filter values
        /// </summary>
        public string QueryOperator { get; set; } = "AND";

        public IEnumerable<FilterInfo> NestedFilters { get; set; }

        /// <summary>
        /// Quoted false will mean that SolrNet will not escape special characters.
        /// </summary>
        public bool Quoted { get; set; } = true;

        /// <summary>
        /// Provides funtionality to manually quote the value if Quoted = false is set for SolrNet
        /// </summary>
        /// <param name="value">The string to qoute</param>
        /// <returns>Quoted string value. In case of Null empty string will be returned.</returns>
        [Trace]
        public static string QuoteString(string value)
        {
            var r = Regex.Replace(value ?? string.Empty, "(\\s|\\+|\\-|\\&\\&|\\|\\||\\!|\\{|\\}|\\[|\\]|\\^|\\(|\\)|\\\"|\\~|\\:|\\;|\\\\)", "\\$1");

            //if (r.IndexOf(' ') != -1)
            //    r = $"\"{r}\"";

            return r;
        }
    }
}