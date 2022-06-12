using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Gyldendal.Api.CoreData.Gql.Common
{
    public class GqlGrammarTraslator
    {
        private readonly Dictionary<string, string> _internalToGqlKeywordMap =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                {"thema", ConfigurationManager.AppSettings["thema"]},
                {"author", ConfigurationManager.AppSettings["author"]},
                {"publisher", ConfigurationManager.AppSettings["publisher"]},
                {"in_series", ConfigurationManager.AppSettings["in_series"]},
                {"publication_date", ConfigurationManager.AppSettings["publication_date"]},
                {"media", ConfigurationManager.AppSettings["media"]},
                {"materialtype", ConfigurationManager.AppSettings["materialtype"]},
                {"category", ConfigurationManager.AppSettings["category"]},
                {"oneperwork", ConfigurationManager.AppSettings["oneperwork"]},
                {"first", ConfigurationManager.AppSettings["first"]},
            };

        /// <summary>
        /// Replaces the input gql expression to translated gql grammar
        /// </summary>
        /// <param name="expression"></param>
        public GqlGrammarTraslator(string expression)
        {
            var inputExpression = expression;

            foreach (var keyword in GqlKeywords.Keywords)
            {
                if (!_internalToGqlKeywordMap.ContainsKey(keyword)) continue;

                var translatedKeyword = _internalToGqlKeywordMap[keyword];

                if (!string.IsNullOrEmpty(translatedKeyword))
                {
                    inputExpression = Regex.Replace(inputExpression, translatedKeyword + '\\' + GqlKeywords.BraceStart, keyword + GqlKeywords.BraceStart, RegexOptions.IgnoreCase);
                }
            }

            TranslatedExpression = inputExpression;
        }

        public string TranslatedExpression { get; private set; }
    }
}
