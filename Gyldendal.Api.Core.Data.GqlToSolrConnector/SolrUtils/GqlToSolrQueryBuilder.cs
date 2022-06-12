using Gyldendal.Api.CoreData.Common;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Gql.Common;
using Gyldendal.Api.CoreData.GqlToSolrConnector.Factories;
using Gyldendal.Api.CoreData.GqlToSolrConnector.Infrastructure;
using Gyldendal.Api.CoreData.GqlToSolrConnector.Model;
using Gyldendal.Api.CoreData.GqlToSolrConnector.SearchRepositories.Common;
using Gyldendal.Api.CoreData.GqlValidator;
using Gyldendal.Common.WebUtils.Exceptions;
using Irony.Parsing.LINQ_Generator;
using SolrNet;
using System.Collections.Generic;
using System.Linq;

namespace Gyldendal.Api.CoreData.GqlToSolrConnector.SolrUtils
{
    public class GqlToSolrQueryBuilder
    {
        private IGeneralSearchQueryBuilderFactory _generalSearchQueryBuilderFactory;

        private ISolrQueryEngineFactory _solrQueryEngineFactory;

        private DataScope _dataScope;

        private string _gqlExpression;

        private AbstractSolrQuery _solrQuery;

        private bool _applyBoosting;

        private bool _useExpressionTree;

        public string Build(string expression, bool applyBoosting, bool useExpressionTree,
            Dictionary<GqlOperation, string> gqlOpToSolrMapping, DataScope dataScope)
        {
            _gqlExpression = expression;
            _applyBoosting = applyBoosting;
            _useExpressionTree = useExpressionTree;
            _dataScope = dataScope;

            _generalSearchQueryBuilderFactory = new GeneralSearchQueryBuilderFactory();
            _solrQueryEngineFactory = new SolrQueryEngineFactory(gqlOpToSolrMapping);

            DoBuild();
            return _solrQuery.Serialize();
        }

        private void DoBuild()
        {
            var queryEngine = _solrQueryEngineFactory.GetInstance(_useExpressionTree);
            var result = new ValidationResult();
            var syntaxChecker = new SyntaxChecker();

            if (!(syntaxChecker.Parse(_gqlExpression)))
                throw new ValidationException((ulong)ErrorCodes.InvalidGql, ErrorCodes.InvalidGql.GetDescription(), Extensions.CoreDataSystemName, null);

            result.GqlExpressions = syntaxChecker.GqlExpression;
            result.GqlExpressionTree = syntaxChecker.GqlExpressionTree;

            var solrQuery = queryEngine.ParseExpression(result, new Dictionary<string, string>(), _applyBoosting,
                GetBoostValue(syntaxChecker.GqlExpression));
            if (queryEngine.IsWorkSearch || queryEngine.IsGeneralSearch)
            {
                solrQuery = queryEngine.IsWorkSearch
                    ? ConstructWorkSearchQuery(queryEngine)
                    : ConstructGeneralSearchQuery(queryEngine);
            }

            _solrQuery = solrQuery;
        }

        private static AbstractSolrQuery ConstructWorkSearchQuery(ISolrQueryEngine queryEngine)
        {
            var token = queryEngine.Tokens.First(t => t.GqlOperation == GqlOperation.WorkSearch);

            return new WorkSearchSolrQueryBuilder().Build(token.Value);
        }

        private AbstractSolrQuery ConstructGeneralSearchQuery(ISolrQueryEngine queryEngine)
        {
            var token = queryEngine.Tokens.First(t => t.GqlOperation == GqlOperation.GeneralSearch);

            var generalSearchQueryGenerator = _generalSearchQueryBuilderFactory.GetInstance(_dataScope);
            return generalSearchQueryGenerator.Build(token.Value);
        }

        private static int GetBoostValue(IEnumerable<GqlExpression> gqlExpressions) => gqlExpressions?.Count(x => x.Type == GqlType.Operator) ?? -1;
    }
}