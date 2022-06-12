using Gyldendal.Api.CoreData.GqlToSolrConnector.Infrastructure;
using Gyldendal.Api.CoreData.GqlToSolrConnector.Model;
using System;
using System.Collections.Generic;
using Gyldendal.Api.CoreData.Gql.Common;
using Gyldendal.Api.CoreData.GqlToSolrConnector.CriteriaExtraction;
using Gyldendal.Api.CoreData.GqlToSolrConnector.QueryParsingV3;

namespace Gyldendal.Api.CoreData.GqlToSolrConnector.Factories
{
    internal class SolrQueryEngineFactory : ISolrQueryEngineFactory
    {
        private readonly ITokenToCriteriaConverter _tokenToCriteriaConverter;

        public SolrQueryEngineFactory(Dictionary<GqlOperation, string> gqlOpToSolrMapping)
        {
            _tokenToCriteriaConverter = new GqlTokenToCriteriaConverter(gqlOpToSolrMapping);
        }

        public ISolrQueryEngine GetInstance(bool useGqlExpressionTree)
        {
            if (useGqlExpressionTree)
            {
                var queryParser = new SolrQueryEngine(_tokenToCriteriaConverter);
                return queryParser;
            }
            else
            {
                var queryParser = new QueryParsingV2.SolrQueryEngine(_tokenToCriteriaConverter);
                return queryParser;
            }
        }
    }
}