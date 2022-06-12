using Gyldendal.Api.CoreData.Common;
using Gyldendal.Api.CoreData.Gql.Common;
using Gyldendal.Api.CoreData.GqlToSolrConnector.CriteriaExtraction;
using Gyldendal.Api.CoreData.GqlToSolrConnector.Model;
using Gyldendal.Api.CoreData.SolrContracts.Product;
using Gyldendal.Common.WebUtils.Exceptions;
using NewRelic.Api.Agent;
using SolrNet;
using System;
using System.Collections.Generic;
using System.Globalization;
using Utils = Gyldendal.Utilities.CommonUtils.Utils;

namespace Gyldendal.Api.CoreData.GqlToSolrConnector.QueryParsing
{
    /// <summary>
    /// Convert Irony Parsed tokens into Solr Query token and add to BaseQuery
    /// </summary>
    public class BaseQueryEngine
    {
        private readonly ITokenToCriteriaConverter _tokenToCriteriaConverter;

        public List<SearchCriteria> Tokens { get; }

        public bool IsGeneralSearch { get; private set; }

        public bool IsWorkSearch { get; private set; }

        public bool IsRelatedProducts { get; private set; }

        protected BaseQueryEngine(ITokenToCriteriaConverter tokenToCriteriaConvereter)
        {
            _tokenToCriteriaConverter = tokenToCriteriaConvereter;
            Tokens = new List<SearchCriteria>();
            IsGeneralSearch = false;
            IsWorkSearch = false;
        }

        /// <summary>
        /// Transform expression token into solr query object
        /// </summary>
        /// <param name="searchQuery">Solr query object</param>
        /// <param name="parseToken">
        /// instance of parsetoken which contains information about solr query
        /// </param>
        /// <returns>Solr query object</returns>
        [Trace]
        protected AbstractSolrQuery AddQuery(AbstractSolrQuery searchQuery, ParseToken parseToken)
        {
            var tokenInfo = _tokenToCriteriaConverter.ConvertToSearchCriteria(parseToken.GqlExpression);
            Tokens.Add(tokenInfo);
            AbstractSolrQuery solrQuery;
            switch (tokenInfo.GqlOperation)
            {
                case GqlOperation.GeneralSearch:
                    IsGeneralSearch = true;
                    tokenInfo.Value = Utils.DecodeGqlRequest(tokenInfo.Value.ReplaceSlashAndHypen());
                    solrQuery = new SolrQuery(tokenInfo.Value);
                    break;

                case GqlOperation.WorkSearch:
                    IsWorkSearch = true;
                    solrQuery = new SolrQuery(Utils.DecodeGqlRequestUnicode(tokenInfo.Value));
                    break;

                case GqlOperation.ContributorId:
                    solrQuery = new SolrQueryByField(tokenInfo.FieldName, tokenInfo.Value.RemoveWildCard()) { Quoted = false };
                    break;

                case GqlOperation.RelatedProducts:
                    IsRelatedProducts = true;
                    solrQuery = SolrQuery.All;
                    break;

                case GqlOperation.PublicationDate:
                    var currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    var startDate = currentDate;
                    var endDate = currentDate;

                    var startOffset = Convert.ToInt32(tokenInfo.LowerLimit);
                    var endOffset = Convert.ToInt32(tokenInfo.UpperLimit);

                    if (startOffset > endOffset)
                    {
                        throw new ValidationException((ulong)ErrorCodes.InvalidGql, ErrorCodes.InvalidGql.GetDescription(), Extensions.CoreDataSystemName, null);
                    }
                    startDate = startDate.AddDays(startOffset);
                    endDate = endDate.AddDays(endOffset);

                    solrQuery = new SolrQueryByRange<DateTime>(tokenInfo.FieldName, startDate, endDate);
                    break;

                case GqlOperation.Label:
                case GqlOperation.IsPhysical:
                case GqlOperation.HasTrialAccess:
                    solrQuery = new SolrQueryByField(tokenInfo.FieldName, tokenInfo.Value.RemoveWildCard()) { Quoted = false };
                    break;

                case GqlOperation.PriceRange:
                    solrQuery = GetSolrPriceRangeQuery(tokenInfo);
                    break;

                default:
                    solrQuery = GetQueryWithFormattedValue(tokenInfo);
                    break;
            }
            if (parseToken.BoostValue > -1)
            {
                solrQuery = solrQuery.Boost(parseToken.BoostValue);
            }

            if (searchQuery == null)
            {
                searchQuery = solrQuery;
            }
            else
            {
                switch (parseToken.OpCode)
                {
                    case '+':
                    case ' ':
                        searchQuery |= solrQuery;
                        break;

                    case '-':
                        searchQuery -= solrQuery;
                        break;

                    case '&':
                        searchQuery &= solrQuery;
                        break;

                    default:
                        searchQuery = solrQuery;
                        break;
                }
            }

            return searchQuery;
        }

        private SolrQueryByField GetQueryWithFormattedValue(SearchCriteria tokenInfo)
        {
            if (tokenInfo.FieldName.ToLower() == GqlOperation.Publisher.ToString().ToLower())
            {
                return ProductSchemaField.Publisher.ToSolrQuery(FilterInfo.QuoteString(tokenInfo.Value), true);
                //new SolrQueryByField(tokenInfo.FieldName,FilterInfo.QuoteString(tokenInfo.Value)) { Quoted = false };
            }
            return new SolrQueryByField(tokenInfo.FieldName, tokenInfo.Value) { Quoted = false };
        }

        private SolrQueryByRange<string> GetSolrPriceRangeQuery(SearchCriteria tokenInfo)
        {
            var danishCultureInfo = CultureInfo.GetCultureInfo("da-DK");

            var startLimit = Convert.ToDecimal(tokenInfo.LowerLimit, danishCultureInfo);
            var endLimit = Convert.ToDecimal(tokenInfo.UpperLimit, danishCultureInfo);

            if (startLimit > endLimit)
            {
                throw new ValidationException((ulong)ErrorCodes.InvalidGql, ErrorCodes.InvalidGql.GetDescription(), Extensions.CoreDataSystemName, null);
            }

            var engStartLimit = startLimit.ToString(CultureInfo.GetCultureInfo("en-US"));
            var engEndLimit = endLimit.ToString(CultureInfo.GetCultureInfo("en-US"));

            return new SolrQueryByRange<string>(tokenInfo.FieldName, engStartLimit + ",DKK", engEndLimit + ",DKK");
        }
    }
}