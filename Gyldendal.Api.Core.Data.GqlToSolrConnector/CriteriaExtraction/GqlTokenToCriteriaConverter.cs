using System.Collections.Generic;
using System.Text.RegularExpressions;
using Gyldendal.Api.CoreData.Gql.Common;
using Gyldendal.Api.CoreData.GqlToSolrConnector.Model;
using Gyldendal.Api.CoreData.SolrContracts.Product;
using Irony.Parsing.LINQ_Generator;
using NewRelic.Api.Agent;

namespace Gyldendal.Api.CoreData.GqlToSolrConnector.CriteriaExtraction
{
    /// <summary>
    /// Implements ITokenToCriteriaConverter interface to extract and return the search criteria from the Gql expression token.
    /// </summary>
    public class GqlTokenToCriteriaConverter : ITokenToCriteriaConverter
    {
        private readonly Dictionary<GqlOperation, string> _gqlOpToSolrFieldMapping;

        public GqlTokenToCriteriaConverter(Dictionary<GqlOperation, string> gqlOpToSolrFieldMapping)
        {
            _gqlOpToSolrFieldMapping = gqlOpToSolrFieldMapping;
        }

        /// <summary>
        /// Extracts the search criteria from the provided Gql token.r query.
        /// </summary>
        /// <param name="token">function in isbn expression</param>
        /// <returns>parse</returns>
        public SearchCriteria ConvertToSearchCriteria(string token)
        {
            var tokenCriteria = new SearchCriteria();
            var startIndex = token.IndexOf('(');
            var endIndex = token.LastIndexOf(')');
            var value = token.Substring(startIndex + 1, endIndex - startIndex - 1);
            var parameters = value.Split(',');
            token = token.Remove(startIndex + 1, value.Length);
            token = token.Remove(token.IndexOf('('), 1);
            token = token.Remove(token.IndexOf(')'), 1);
            if (parameters.Length > 1)
            {
                tokenCriteria.LowerLimit = parameters[0];
                tokenCriteria.UpperLimit = parameters[1];
            }
            else
            {
                tokenCriteria.Value = value + "*";
            }

            tokenCriteria.GqlOperation = Gql.Common.Util.ConvertToGqlOperation(token.ToLower());

            if (tokenCriteria.GqlOperation != GqlOperation.GeneralSearch && tokenCriteria.GqlOperation != GqlOperation.WorkSearch && tokenCriteria.GqlOperation != GqlOperation.RelatedProducts)
            {
                tokenCriteria.FieldName = _gqlOpToSolrFieldMapping[tokenCriteria.GqlOperation];
            }

            //if (tokenCriteria.GqlOperation == GqlOperation.Author)
            //{
            //    tokenCriteria.Value = '"' + tokenCriteria.Value + '"';
            //}
            switch (tokenCriteria.GqlOperation)
            {
                case GqlOperation.Level:
                case GqlOperation.Title:
                case GqlOperation.Author:
                case GqlOperation.Subject:
                case GqlOperation.SubArea:
                case GqlOperation.Area:
                    tokenCriteria.Value = '"' + value + '"';
                    break;

                case GqlOperation.InSeries:
                case GqlOperation.GeneralSearch:
                    tokenCriteria.Value = value;
                    break;

                case GqlOperation.Work:
                    tokenCriteria.Value = FilterInfo.QuoteString(value);
                    break;

                case GqlOperation.RelatedProducts:
                    tokenCriteria.Value = value;
                    tokenCriteria.FieldName = ProductSchemaField.Id.GetFieldName();
                    break;
            }

            return tokenCriteria;
        }

        /// <summary>
        /// Extracts the search criteria from the provided Gql token.r query.
        /// </summary>
        /// <param name="gqlExpression">function in isbn expression</param>
        /// <returns>parse</returns>
        [Trace]
        public SearchCriteria ConvertToSearchCriteria(GqlExpression gqlExpression)
        {
            var value = string.Join(",", gqlExpression.Parameters);
            var tokenCriteria = new SearchCriteria();
            SetCriteriaValue(tokenCriteria, gqlExpression, value);

            tokenCriteria.GqlOperation = Gql.Common.Util.ConvertToGqlOperation(gqlExpression.Name);

            if (tokenCriteria.GqlOperation != GqlOperation.GeneralSearch && tokenCriteria.GqlOperation != GqlOperation.WorkSearch && tokenCriteria.GqlOperation != GqlOperation.RelatedProducts)
            {
                tokenCriteria.FieldName = _gqlOpToSolrFieldMapping[tokenCriteria.GqlOperation];
            }

            switch (tokenCriteria.GqlOperation)
            {
                case GqlOperation.Isbn:
                case GqlOperation.Webshop:
                    // No wild card search in case of Isbn13 and webshop field.
                    tokenCriteria.Value = value.Trim('*');
                    break;

                case GqlOperation.Level:
                case GqlOperation.Title:
                case GqlOperation.Author:
                case GqlOperation.Subject:
                case GqlOperation.SubArea:
                case GqlOperation.Area:
                    tokenCriteria.Value = '"' + value + '"';
                    break;

                case GqlOperation.MainCategory:
                case GqlOperation.SubCategory:
                case GqlOperation.Imprint:
                    tokenCriteria.Value = FilterInfo.QuoteString(value);
                    break;

                case GqlOperation.InSeries:
                case GqlOperation.GeneralSearch:
                    tokenCriteria.Value = value;
                    break;

                case GqlOperation.Work:
                    tokenCriteria.Value = FilterInfo.QuoteString(value);
                    break;

                case GqlOperation.RelatedProducts:
                    tokenCriteria.Value = value;
                    tokenCriteria.FieldName = ProductSchemaField.Id.GetFieldName();
                    break;
            }

            return tokenCriteria;
        }

        private void SetCriteriaValue(SearchCriteria searchCriteria, GqlExpression gqlExpression, string value)
        {
            var priceRangeRegex = new Regex(@"\s*\d+(,\d+)?\s*;\s*\d+(,\d+)?\s*");
            
            if (gqlExpression.Parameters.Length > 1)
            {
                searchCriteria.LowerLimit = gqlExpression.Parameters[0];
                searchCriteria.UpperLimit = gqlExpression.Parameters[1];
            }
            else if (priceRangeRegex.IsMatch(gqlExpression.Parameters[0]))
            {
                var priceRangeSplit = gqlExpression.Parameters[0].Split(';');
                searchCriteria.LowerLimit = priceRangeSplit[0];
                searchCriteria.UpperLimit = priceRangeSplit[1];
            }
            else
            {
                searchCriteria.Value = value + "*";
            }
        }
    }
}