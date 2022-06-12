using System.Linq;

namespace Gyldendal.Api.CoreData.Gql.Common
{
    public static class GqlKeywords
    {
        /// <summary>
        /// The internal gql operators
        /// </summary>
        public static string[] Operators => new[]
        {
            "OR",
            "AND",
            "AND NOT"
        };

        /// <summary>
        /// The internal gql grammar keywords
        /// </summary>
        public static string[] PostProcessingKeywords => new[]
        {
            "first",
            OnePerWork,
            "orderby",
            "top"
        };

        /// <summary>
        /// The internal gql grammar keywords
        /// </summary>
        public static string[] Keywords => new[]
            {
                "thema",
                "author",
                InSeries,
                "isbn",
                "publication_date",
                "publisher",
                "media",
                Category,
                "materialtype"
            }
            .Union(PostProcessingKeywords)
            .ToArray();

        public static char BraceStart => '(';

        public static char BraceEnd => ')';

        public static char ArgumentSplitChar => ',';

        public static char TokenSeparator => ' ';

        public static string Category => "category";

        public static string InSeries => "in_series";

        public static string OnePerWork => "oneperwork";
    }
}