using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Gql.Common;
using Gyldendal.Api.CoreData.GqlToSolrConnector.Model;
using Gyldendal.Api.CoreData.GqlToSolrConnector.SolrSearch;
using Gyldendal.Api.CoreData.GqlValidator;
using Gyldendal.Api.CoreData.SolrContracts.Product;
using Irony.Parsing.LINQ_Generator;
using SolrNet;
using SolrNet.Commands.Parameters;
using SolrNet.Impl.FieldSerializers;
using SolrNet.Impl.QuerySerializers;
using System;
using System.Collections.Generic;
using Product = Gyldendal.Api.CoreData.Contracts.Models.Product;

namespace Gyldendal.Api.CoreData.GqlToSolrConnector
{
    /// <summary>
    /// Common Utility methods for the GQLToSolrConnector Layer
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Extension method that adds group parameters to be used in solr query
        /// </summary>
        /// <param name="groupParameters">instance object</param>
        /// <param name="field">field to be used for group by</param>
        /// <param name="sortField">field to be used for sorting within group</param>
        /// <param name="sortDirection">sorting order</param>
        public static void AddGroupParameters(this GroupingParameters groupParameters, string field, string sortField = "", Order sortDirection = Order.ASC)
        {
            if (groupParameters.Fields == null)
                groupParameters.Fields = new List<string>();
            groupParameters.Fields.Add(field);
            if (sortField.Length != 0)
            {
                var sortOrder = new SortOrder(sortField, sortDirection);
                if (groupParameters.OrderBy == null)
                    groupParameters.OrderBy = new List<SortOrder>();
                groupParameters.OrderBy.Add(sortOrder);
            }
        }

        /// <summary>
        /// convert parsed unit to method
        /// </summary>
        /// <param name="firstUnit">parsed method information</param>
        /// <returns>convert parsed unit to method</returns>
        public static List<GqlExpression> ToFirstExpression(this ParsedUnits firstUnit)
        {
            var expression = new List<GqlExpression>();
            foreach (var param in firstUnit.Parameters)
            {
                if (expression.Count == 0)
                    expression.Add(new GqlExpression(GqlType.Function, "ISBN", new[] { param }));
                else
                {
                    expression.Add(new GqlExpression(GqlType.Operator, "+", null));
                    expression.Add(new GqlExpression(GqlType.Function, "ISBN", new[] { param }));
                }
            }

            return expression;
        }

        /// <summary>
        /// convert parsed unit to method
        /// </summary>
        /// <param name="firstUnit">parsed method information</param>
        /// <param name="gqlExpressions"></param>
        /// <returns>convert parsed unit to method</returns>
        public static Node ToFirstExpressionTree(this ParsedUnits firstUnit, out List<GqlExpression> gqlExpressions)
        {
            gqlExpressions = new List<GqlExpression>();
            foreach (var param in firstUnit.Parameters)
            {
                if (gqlExpressions.Count == 0)
                    gqlExpressions.Add(new GqlExpression(GqlType.Function, "ISBN", new[] { param }));
                else
                {
                    gqlExpressions.Add(new GqlExpression(GqlType.Operator, "+", null));
                    gqlExpressions.Add(new GqlExpression(GqlType.Function, "ISBN", new[] { param }));
                }
            }

            return new SyntaxChecker().GenerateGqlExpressionTree(gqlExpressions);
        }

        /// <summary>
        /// Checks status of solr server whether its running or not
        /// </summary>
        /// <param name="core"></param>
        /// <param name="solrServerUrl"></param>
        /// <returns>returns server status object</returns>
        // ReSharper disable once UnusedMember.Global
        public static ServerStatus CheckSolrServerStatus(string core, string solrServerUrl)
        {
            var status = new ServerStatus
            {
                IsCoreExists = false,
                IsServerRunning = false,
                ErrorMessage = "",
                IsError = false
            };
            try
            {
                var solrOperations = new GenericSolrSearch<Product>(core, solrServerUrl);
                solrOperations.Search(new SolrHasValueQuery("id"), 0, 10);
                status.IsCoreExists = true;
                status.IsServerRunning = true;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("404"))
                {
                    status.IsServerRunning = true;
                }
                else if (ex.Message.ToLower().Contains("unable to connect"))
                {
                    status.ErrorMessage = ex.Message;
                }
                else
                {
                    status.IsCoreExists = true;
                    status.IsServerRunning = true;
                    status.IsError = true;
                    status.ErrorMessage = ex.Message;
                }
            }
            return status;
        }

        public static SolrQueryByField ToSolrQuery(this ProductSchemaField schemaField, string value, bool isWildCard = false)
        {
            var fieldVal = value;
            if (isWildCard)
            {
                fieldVal = $"*{fieldVal}*";
            }
            else
                fieldVal = '"' + fieldVal + '"';
            return new SolrQueryByField(schemaField.GetFieldName(), fieldVal) { Quoted = false };
        }

        public static AbstractSolrQuery ToBoostedFieldQuery(this ProductSchemaField schemaField, string value, double boostFactor, bool isWildCard = false)
        {
            var query = schemaField.ToSolrQuery(value, isWildCard);
            return query.Boost(boostFactor);
        }

        public static int GetFieldExcatMatchBoost(this ProductSchemaField field)
        {
            const int maxExactBoostValue = 1000;
            int boost;

            switch (field)
            {
                case ProductSchemaField.TitleSubstring:
                case ProductSchemaField.TitleReplaced:
                case ProductSchemaField.ExactMatch:
                    boost = maxExactBoostValue;
                    break;

                case ProductSchemaField.Isbn13:
                case ProductSchemaField.TitleContains:
                    boost = maxExactBoostValue - 500;
                    break;

                case ProductSchemaField.Subtitle:
                case ProductSchemaField.SubtitleReplaced:
                    boost = maxExactBoostValue - 550;
                    break;

                case ProductSchemaField.AuthorNamesDa:
                    boost = maxExactBoostValue - 600;
                    break;

                case ProductSchemaField.AuthorNames:
                    boost = maxExactBoostValue - 800;
                    break;

                case ProductSchemaField.SystemNamesDa:
                case ProductSchemaField.SeriesNamesReplaced:
                    boost = maxExactBoostValue - 850;
                    break;

                default:
                    boost = 1;
                    break;
            }

            return boost;
        }

        public static int GetFieldBoost(this ProductSchemaField field)
        {
            int boost;

            switch (field)
            {
                case ProductSchemaField.Isbn13:
                case ProductSchemaField.ProductId:
                case ProductSchemaField.ThemaExactmatch:
                    boost = 10;
                    break;

                case ProductSchemaField.Description:
                    boost = 1;
                    break;

                case ProductSchemaField.TitleDa:
                case ProductSchemaField.SubstringField:
                    boost = 50;
                    break;

                case ProductSchemaField.Subtitle:
                case ProductSchemaField.SubtitleReplaced:
                    boost = 20;
                    break;

                case ProductSchemaField.SearchMedia:
                    boost = 2;
                    break;

                case ProductSchemaField.AuthorNamesDa:
                case ProductSchemaField.SystemNamesDa:
                case ProductSchemaField.SeriesNamesReplaced:
                case ProductSchemaField.AreasDa:
                case ProductSchemaField.SubAreasDa:
                case ProductSchemaField.LevelsDa:
                    boost = 3;
                    break;

                case ProductSchemaField.TitleContains:
                    boost = 100;
                    break;

                case ProductSchemaField.TitleSubstring:
                case ProductSchemaField.TitleReplaced:
                case ProductSchemaField.ExactMatch:
                    boost = 1000;
                    break;

                case ProductSchemaField.ThemaSubstring:
                    boost = 5;
                    break;

                default:
                    boost = 1;
                    break;
            }

            return boost;
        }

        public static SortOrder ToSortOrder(this OrderByParam orderByParam)
        {
            Enum.TryParse(orderByParam.Direction.ToString(), true, out Order direction);
            return new SortOrder(orderByParam.FieldName, direction);
        }

        public static string ConvertToString(this Enum eff)
        {
            return Enum.GetName(eff.GetType(), eff);
        }

        public static List<OrderByParam> GetOrderByParams(this SearchRequest request)
        {
            var orderByParams = new List<OrderByParam>();
            request.OrderBy.GetOrderByParams(request.SortBy, ref orderByParams);
            return orderByParams;
        }

        public static OrderByParam AddMediaTypeRankSorting()
        {
            return new OrderByParam
            {
                FieldName = $"{OrderBy.mediaTypeRank:G}",
                Direction = SortBy.Desc
            };
        }

        public static OrderByParam AddBoostingScoreSorting()
        {
            return new OrderByParam
            {
                FieldName = "score",
                Direction = SortBy.Desc
            };
        }

        public static void GetOrderByParams(this OrderBy orderBy, SortBy sortBy, ref List<OrderByParam> orderByParams)
        {
            orderByParams = orderByParams ?? new List<OrderByParam>();
            if (orderBy != OrderBy.None)
            {
                orderByParams.Add(new OrderByParam
                {
                    FieldName = orderBy == OrderBy.title
                        ? ProductSchemaField.TitleSorting.GetFieldName()
                        : $"{orderBy:G}",
                    Direction = sortBy
                });
            }
        }

        public static string RemoveWildCard(this string source)
        {
            return source.IndexOf('*') > -1 ? source.Remove(source.IndexOf('*')) : source;
        }

        public static string ReplaceSlashAndHypen(this string source)
        {
            source = source.Replace('/', ' ');
            source = source.Replace('-', ' ');
            return source.Trim();
        }

        /// <summary>
        /// Serialize solr query to string.
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public static string Serialize(this ISolrQuery q)
        {
            var serializer = new DefaultQuerySerializer(new DefaultFieldSerializer());
            return serializer.Serialize(q);
        }
    }
}