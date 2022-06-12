using System.Collections.Generic;
using System.Linq;
using Gyldendal.Api.CoreData.Gql.Common;
using Irony.Parsing.LINQ_Generator;

namespace Gyldendal.Api.CoreData.GqlValidator
{
    public static class Util
    {
        /// <summary>
        /// Maps string to token type
        /// </summary>
        /// <param name="keyword">given string</param>
        /// <returns>mapped token type</returns>
        public static TokenType ParseToken(string keyword)
        {
            TokenType type;
            switch (keyword.ToLower())
            {
                case "first":
                    type = TokenType.First;
                    break;

                case "oneperwork":
                    type = TokenType.OnePerWork;
                    break;

                case "orderby":
                    type = TokenType.OrderBy;
                    break;

                case "top":
                    type = TokenType.Top;
                    break;

                default:
                    type = TokenType.None;
                    break;
            }
            return type;
        }

        public static string ToGqlString(this List<GqlExpression> gqlExpressions)
        {
            return string.Join("", gqlExpressions.Select(x => x.ToString()));
        }

        ///// <summary>
        ///// Extension method that adds group parameters to be used in solr query
        ///// </summary>
        ///// <param name="groupParameters">instance object</param>
        ///// <param name="field">field to be used for group by</param>
        ///// <param name="sortField">field to be used for sorting within group</param>
        ///// <param name="sortDirection">sorting order</param>
        //public static void AddGroupParameters(this GroupingParameters groupParameters, string field,
        //    string sortField="", Order sortDirection=Order.ASC)
        //{
        //    if (groupParameters.Fields == null)
        //        groupParameters.Fields=new List<string>();
        //    groupParameters.Fields.Add(field);
        //    if (sortField.Length != 0)
        //    {
        //        var sortOrder = new SortOrder(sortField, sortDirection);
        //        if (groupParameters.OrderBy == null)
        //            groupParameters.OrderBy = new List<SortOrder>();
        //        groupParameters.OrderBy.Add(sortOrder);
        //    }
        //}

        ///// <summary>
        ///// splits isbn expression on '+' delimiter and
        ///// returns the count which is used to assign
        ///// rank values to different expressions.
        ///// </summary>
        ///// <param name="expression">isbn expression</param>
        ///// <returns>boost count of the query</returns>
        //public static int GetFieldBoostCount(string expression)
        //{
        //    return expression.Split('+').Length;
        //}

        /// <summary>
        /// convert parsed information into method
        /// </summary>
        /// <param name="unit"></param>
        /// <returns>method</returns>
        public static string ToMethod(this ParsedUnits unit)
        {
            return unit.Type.ToString("G") + "(" + string.Join(",", unit.Parameters) + ")";
        }
    }
}