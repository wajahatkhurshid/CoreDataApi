using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Gql.Common;
using Gyldendal.Api.CoreData.GqlToSolrConnector.Faceting;
using Gyldendal.Api.CoreData.GqlToSolrConnector.Factories;
using Gyldendal.Api.CoreData.GqlToSolrConnector.Infrastructure;
using Gyldendal.Api.CoreData.GqlToSolrConnector.Model;
using Gyldendal.Api.CoreData.GqlToSolrConnector.SearchRepositories.Common;
using Gyldendal.Api.CoreData.GqlToSolrConnector.SolrSearch;
using Gyldendal.Api.CoreData.SolrContracts.Product;
using Irony.Parsing.LINQ_Generator;
using NewRelic.Api.Agent;
using SolrNet;
using SolrNet.Commands.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using Product = Gyldendal.Api.CoreData.SolrContracts.Product.Product;
using SchemaField = Gyldendal.Api.CoreData.SolrContracts.Product.ProductSchemaField;

namespace Gyldendal.Api.CoreData.GqlToSolrConnector
{
    /// <summary>
    /// Use parsed information to build and execute solr query.
    /// </summary>
    public class Connector<T> where T : Product
    {
        private const string FullMatch = "100%";

        private readonly ISearch<T> _solrSearch;

        private readonly DataScope _dataScope;

        private readonly object _lock = new object();

        private readonly IGeneralSearchQueryBuilderFactory _generalSearchQueryBuilderFactory;

        private readonly ISolrQueryEngineFactory _solrQueryEngineFactory;

        private GroupingParameters _workGroupingParams;

        private List<RankedMediaType> _mediaTypePriorityList;

        private SearchRequest _request;

        private bool _isOnePerWorkWithParams;

        private bool _isOnePerWork;

        private FacetParameters _facets;

        private ICollection<ISolrQuery> _filters;

        private GroupingParameters _groupParams;

        private List<OrderByParam> _orderByParams;

        /// <summary>
        /// initialize member variables
        /// </summary>
        /// <param name="gqlOpToSolrMapping"></param>
        /// <param name="solrSearch"></param>
        /// <param name="mediaTypeNames"></param>
        /// <param name="dataScope"></param>
        public Connector(ISearch<T> solrSearch, string[] mediaTypeNames,
            Dictionary<GqlOperation, string> gqlOpToSolrMapping, DataScope dataScope)
        {
            _solrSearch = solrSearch;
            _dataScope = dataScope;
            _solrQueryEngineFactory = new SolrQueryEngineFactory(gqlOpToSolrMapping);
            _generalSearchQueryBuilderFactory = new GeneralSearchQueryBuilderFactory();
            SetMediaTypesPriorityListWithDefaultRank(mediaTypeNames);
        }

        /// <summary>
        /// Builds solr query and executes to get results
        /// </summary>
        /// <typeparam name="T">type of index</typeparam>
        /// <returns>results returned by solr</returns>
        [Trace]
        public SearchResult<T> Execute(SearchRequest request)
        {
            _request = request ?? throw new ArgumentException("request cannot be null");

            lock (_lock)
            {
                return DoExecute();
            }
        }

        [Trace]
        private SearchResult<T> DoExecute()
        {
            if (!(_request.GqlExpressionInfo.Result.IsValidated))
            {
                return null;
            }

            BuildFilters();
            BuildFacets();
            SetGroupingParams();
            SetOrderByParams();

            return PerformSolrSearch();
        }

        private SearchResult<T> PerformSolrSearch()
        {
            //GQL parsing to solr query
            var queryEngine = _solrQueryEngineFactory.GetInstance(_request.UseGqlExpressionTree);

            var baseQuery = queryEngine.ParseExpression(_request.GqlExpressionInfo.Result, _request.DefaultSearchCriteria,
                ApplyBoost(), GetBoostValue(_request.GqlExpressionInfo.Result.GqlExpressions));

            SearchResult<T> results;
            IEnumerable<KeyValuePair<string, string>> extraParams = null;

            if (_request.GqlExpressionInfo.PostProcessingTokensExist)
            {
                results = QueryExecutionWithPostProcessing(baseQuery, _orderByParams, _filters, _facets, _groupParams);
                results = ProcessOnePerWork(results);
                return results;
            }

            if (queryEngine.IsWorkSearch || queryEngine.IsGeneralSearch)
            {
                var exactMatchQuery = queryEngine.IsWorkSearch
                    ? ConstructWorkSearchQuery(queryEngine)
                    : ConstructGeneralSearchQuery(queryEngine);

                results = _solrSearch.Search(exactMatchQuery, _request.Start, _request.Rows, _orderByParams,
                    _groupParams,
                    _facets, _filters);

                if (results.TotalResults > 0)
                    return results;
                extraParams = BuildExtraParams(FullMatch);
            }

            results = _solrSearch.Search(baseQuery, _request.Start, _request.Rows, _orderByParams, _groupParams,
                _facets,
                _filters, extraParams);

            return results;
        }

        private void BuildFilters()
        {
            _filters = new Filtering.FilterQueryBuilder().Build(_request.Filters);
        }

        private void BuildFacets()
        {
            _facets = _facets.Build(_request.FacetFields).Build(_request.PriceRangeFacetParams);
        }

        private void SetGroupingParams()
        {
            if (string.IsNullOrWhiteSpace(_request.GroupByField))
                _groupParams = null;
            else 
                _groupParams = new GroupingParameters
                {
                    Fields = new[] { _request.GroupByField },
                    Format = GroupingFormat.Grouped,
                    Limit = int.MaxValue,
                    Ngroups = true,
                    Facet = false
                };
        }

        private static AbstractSolrQuery ConstructWorkSearchQuery(ISolrQueryEngine queryEngine)
        {
            var token = queryEngine.Tokens.First(t => t.GqlOperation == GqlOperation.WorkSearch);

            return new WorkSearchSolrQueryBuilder().Build(token.Value);
        }

        private AbstractSolrQuery ConstructGeneralSearchQuery(ISolrQueryEngine queryEngine)
        {
            var token = queryEngine.Tokens.First(t => t.GqlOperation == GqlOperation.GeneralSearch);

            var solrQueryBuilder = _generalSearchQueryBuilderFactory.GetInstance(_dataScope);
            return solrQueryBuilder.Build(token.Value);
        }

        private int GetBoostValue(List<GqlExpression> gqlExpressions) => gqlExpressions?.Count(x => x.Type == GqlType.Operator) ?? -1;

        /// <summary>
        /// Boost is applied for all GQL expressions except for select few which apply search on multiple solr fields.
        /// </summary>
        /// <returns></returns>
        private bool ApplyBoost()
        {
            return !(_request.GqlExpressionInfo.Result.GqlExpressions == null ||
                     _request.GqlExpressionInfo.Result.GqlExpressions
                         .Any(x =>
                             x.Name == GqlOperation.GeneralSearch.ToTokenString() ||
                             x.Name == GqlOperation.WorkSearch.ToTokenString()));
        }

        private SearchResult<T> QueryExecutionWithPostProcessing(AbstractSolrQuery baseQuery, List<OrderByParam> orderByParams, ICollection<ISolrQuery> filters, FacetParameters facets, GroupingParameters groupParams)
        {
            ExtractPagingSortingCriteria();

            ExtractOnePerWorkCriteria();

            var firstToken = _request.GqlExpressionInfo.PostProcessTokens.FirstOrDefault(p => p.Type == TokenType.First);
            if (firstToken == null)
            {
                return _solrSearch.Search(baseQuery, _request.Start, _isOnePerWorkWithParams ? int.MaxValue : _request.Rows, orderByParams, groupParams, facets, filters);
            }

            // If First method is found in the expression, First method is a special case.
            // It is evaluated in two steps, in first step it check whether the given isbn's are present in the
            // expression or not and if ISBNs are present then it evaluates the remaining expression.
            var expressionTree = firstToken.ToFirstExpressionTree(out var gqlExpressions);
            var validResult = new ValidationResult
            {
                GqlExpressionTree = expressionTree,
                GqlExpressions = gqlExpressions
            };
            var parser = _solrQueryEngineFactory.GetInstance(_request.UseGqlExpressionTree);
            var queryWithIsbns = baseQuery & parser.ParseExpression(validResult, _request.DefaultSearchCriteria, true);

            var resultWithIsbn = _solrSearch.Search(queryWithIsbns, _request.Start, _request.Rows, orderByParams, groupParams, facets, filters);
            if (resultWithIsbn.TotalResults != 0)
            {
                return resultWithIsbn;
            }

            var queryWithoutIsbns = baseQuery - parser.ParseExpression(validResult, _request.DefaultSearchCriteria);

            var recordCount = _request.Rows - resultWithIsbn.TotalResults > 0
                ? _request.Rows - resultWithIsbn.TotalResults
                : 0;

            var resultWithoutIsbn = _solrSearch.Search(queryWithoutIsbns, _request.Start, recordCount, orderByParams, groupParams, facets, filters);
            resultWithIsbn.TotalResults += resultWithoutIsbn.TotalResults;
            resultWithIsbn.ItemsFound.AddRange(resultWithoutIsbn.ItemsFound);

            return resultWithIsbn;
        }

        private void SetupMediaTypesPrioritizedList(ParsedUnits token)
        {
            var mediaTypesListDefaultRank = _mediaTypePriorityList;

            _mediaTypePriorityList = new List<RankedMediaType>();
            var priority = 1;
            _isOnePerWorkWithParams = true;
            _mediaTypePriorityList.AddRange(token.Parameters.Select(mediaType => new RankedMediaType
            {
                Name = mediaType,
                Rank = priority++
            }));

            var mediaTypesNotInParams = mediaTypesListDefaultRank.Where(x =>
                !_mediaTypePriorityList.Any(
                    y => string.Equals(y.Name, x.Name, StringComparison.CurrentCultureIgnoreCase))
                );

            foreach (var mediaType in mediaTypesNotInParams.ToArray())
            {
                _mediaTypePriorityList.Add(new RankedMediaType
                {
                    Name = mediaType.Name,
                    Rank = priority++
                });
            }
        }

        private void ExtractPagingSortingCriteria()
        {
            var orderByToken =
                _request.GqlExpressionInfo.PostProcessTokens.FirstOrDefault(x => x.Type == TokenType.OrderBy);
            if (orderByToken != null)
            {
                if (Enum.TryParse(orderByToken.Parameters[0], true, out OrderBy orderBy))
                {
                    _request.OrderBy = orderBy;
                }

                if (Enum.TryParse(orderByToken.Parameters[1], true, out SortBy sortBy))
                {
                    _request.OrderBy = orderBy;
                }
                _request.SortBy = sortBy;
            }

            var topRowsToken = _request.GqlExpressionInfo.PostProcessTokens.FirstOrDefault(x => x.Type == TokenType.Top);
            if (topRowsToken != null)
            {
                _request.Rows = int.Parse(topRowsToken.Parameters[0]);
            }
        }

        private void ExtractOnePerWorkCriteria()
        {
            _isOnePerWorkWithParams = false;
            _isOnePerWork = false;

            var onePerWorkToken =
                _request.GqlExpressionInfo.PostProcessTokens.FirstOrDefault(x => x.Type == TokenType.OnePerWork);
            if (onePerWorkToken == null)
            {
                return;
            }

            _isOnePerWork = true;
            if (onePerWorkToken.Parameters.Count > 0 && onePerWorkToken.Parameters[0].Length > 0)
            {
                SetupMediaTypesPrioritizedList(onePerWorkToken);
            }
            if (!_isOnePerWorkWithParams)
            {
                _workGroupingParams = new GroupingParameters() { Format = GroupingFormat.Grouped, Limit = 1 };
                _workGroupingParams.AddGroupParameters(SchemaField.WorkId.GetFieldName(), "rank");
            }
        }

        /// <summary>
        /// Used to process oneperwork method with parameters. The output of solr is
        /// processed according to priority media types set by the user as parameters.
        /// </summary>
        /// <param name="matchingProducts">results returned by solr</param>
        /// <returns>processed products</returns>
        private SearchResult<T> ProcessOnePerWork(SearchResult<T> matchingProducts)
        {
            if (_isOnePerWorkWithParams)
            {
                var output = FilterListForOnePerWork(matchingProducts.ItemsFound);
                if (_request.Rows > 0)
                {
                    matchingProducts.ItemsFound.Clear();
                    matchingProducts.ItemsFound.AddRange(SortResultsForOnePerWork(output, _request.OrderBy,
                        _request.SortBy));
                    matchingProducts.TotalResults = matchingProducts.ItemsFound.Count;
                }
                else
                {
                    matchingProducts.ItemsFound.Clear();
                    matchingProducts.TotalResults = output.Count;
                }
            }
            else if (_isOnePerWork)
            {
                var output = SortResultsForOnePerWork(matchingProducts.ItemsFound, _request.OrderBy, _request.SortBy);
                matchingProducts.ItemsFound.Clear();
                matchingProducts.ItemsFound.AddRange(SortResultsForOnePerWork(output, _request.OrderBy, _request.SortBy));
                matchingProducts.TotalResults = matchingProducts.ItemsFound.Count;
            }

            return matchingProducts;
        }

        /// <summary>
        /// Sorts products on the basis of orderby and sortby parameters
        /// </summary>
        /// <param name="matchingProducts">products found in solr</param>
        /// <param name="orderBy">orderby field name</param>
        /// <param name="sortBy">sorting order i.e. asc or desc</param>
        /// <returns></returns>
        private static IEnumerable<T> SortResultsForOnePerWork(IEnumerable<T> matchingProducts, OrderBy orderBy,
            SortBy sortBy)
        {
            var isAcscending = sortBy.ToString() == "asc";

            if (orderBy.ToString().Equals(SchemaField.Title.GetFieldName()))
            {
                return isAcscending
                    ? matchingProducts.OrderBy(p => p.Title).ToList()
                    : matchingProducts.OrderByDescending(p => p.Title).ToList();
            }

            if (orderBy.ToString().Equals(SchemaField.Isbn13.GetFieldName()))
            {
                return isAcscending
                    ? matchingProducts.OrderBy(p => p.ProductId).ToList()
                    : matchingProducts.OrderByDescending(p => p.ProductId).ToList();
            }
            //return default ordering
            return isAcscending
                ? matchingProducts.OrderBy(p => p.PublishDate).ToList()
                : matchingProducts.OrderByDescending(p => p.PublishDate).ToList();
        }

        /// <summary>
        /// This method implements a priority based selection algorithm.
        /// Each product belongs to a work which has a workId. A work can have multiple products in it.
        /// OnePerWork method of ISBN expression returns one product per work according to predefined priority
        /// of media types i.e. E-bog has highest priority, Lydbog 2 etc
        /// If a work contains two products of type E-bog and Lydbog then OnePerWork should return product with media type
        /// E-bog as it has high priority than Lydbog. In case of OnePerWork with parameters (mediatypes), the user overrides
        /// the default priority of media types.
        /// for e.g. OnePerWork(Bog), in this expression Bog would be assigned the highest priority instead of E-bog
        /// The order would be Bog,E-bog,Lydbog. Now if a work has products with both E-bog and Bog media types
        /// This expression will return product of Bog media type.
        /// </summary>
        /// <param name="matchingProducts">List of products</param>
        /// <returns>list of processed products</returns>
        private List<T> FilterListForOnePerWork(IEnumerable<T> matchingProducts)
        {
            var results = new Dictionary<int, T>();
            foreach (var product in matchingProducts)
            {
                if (results.ContainsKey(product.WorkId))
                {
                    var value = results[product.WorkId];
                    var currentItem =
                        _mediaTypePriorityList.FirstOrDefault(
                            p => string.Equals(p.Name, product.MediaTypeName, StringComparison.CurrentCultureIgnoreCase));
                    var oldItem =
                        _mediaTypePriorityList.FirstOrDefault(
                            p => string.Equals(p.Name, value.MediaTypeName, StringComparison.CurrentCultureIgnoreCase));
                    if (currentItem == null || oldItem == null) continue;

                    if (currentItem.Rank >= oldItem.Rank) continue;

                    results.Remove(product.WorkId);
                    results[product.WorkId] = product;
                }
                else
                {
                    results[product.WorkId] = product;
                }
            }
            return results.Values.ToList();
        }

        private void SetMediaTypesPriorityListWithDefaultRank(string[] mediaTypeNames)
        {
            var index = 1;
            _mediaTypePriorityList = new List<RankedMediaType>();
            foreach (var mediaTypeName in mediaTypeNames)
            {
                _mediaTypePriorityList.Add(new RankedMediaType
                {
                    Name = mediaTypeName,
                    Rank = index++
                });
            }
        }

        private static IEnumerable<KeyValuePair<string, string>> BuildExtraParams(string matchingCriteria = "")
        {
            string ConstructExtraParam(SchemaField field, SchemaField? fieldBoost = null)
            {
                fieldBoost = fieldBoost ?? field;
                return $"{field.GetFieldName()}^{fieldBoost.Value.GetFieldBoost()}";
            }

            var extraParams = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("qf", ConstructExtraParam(SchemaField.Isbn13)),
                new KeyValuePair<string, string>("qf", ConstructExtraParam(SchemaField.Description)),
                new KeyValuePair<string, string>("qf", ConstructExtraParam(SchemaField.TitleDa)),
                new KeyValuePair<string, string>("qf", ConstructExtraParam(SchemaField.AuthorNamesDa)),
                new KeyValuePair<string, string>("qf", ConstructExtraParam(SchemaField.SystemNamesDa)),
                new KeyValuePair<string, string>("qf", ConstructExtraParam(SchemaField.SearchMedia)),
                new KeyValuePair<string, string>("qf", ConstructExtraParam(SchemaField.LevelsDa)),
                new KeyValuePair<string, string>("qf", ConstructExtraParam(SchemaField.AreasDa)),
                new KeyValuePair<string, string>("qf", ConstructExtraParam(SchemaField.SubAreasDa)),
                new KeyValuePair<string, string>("qf", ConstructExtraParam(SchemaField.SeriesNamesDa)),
                new KeyValuePair<string, string>("qf", ConstructExtraParam(SchemaField.TitleSubstring, SchemaField.TitleDa)),
                new KeyValuePair<string, string>("qf", ConstructExtraParam(SchemaField.Subtitle)),
                new KeyValuePair<string, string>("qf", ConstructExtraParam(SchemaField.Labels)),

                new KeyValuePair<string, string>("pf", ConstructExtraParam(SchemaField.TitleSubstring))
            };
            if (!string.IsNullOrEmpty(matchingCriteria))
                extraParams.Add(new KeyValuePair<string, string>("mm", matchingCriteria));
            return extraParams;
        }

        private void SetOrderByParams()
        {
            _orderByParams = _request.OrderBy == OrderBy.None ?
                AddMediaTypeSortingWithBoosting() :
                _request.GetOrderByParams();
        }

        /// <summary>
        /// This method will only work for GU, HR and Munks
        /// and it will add media type sorting over the boosting score.
        /// </summary>
        /// <returns></returns>
        private List<OrderByParam> AddMediaTypeSortingWithBoosting()
        {
            var orderByParams = new List<OrderByParam>();
            var uddShops = new[]
            {
                DataScope.GuShop,
                DataScope.HansReitzelShop,
                DataScope.MunksGaardShop
            };

            if (uddShops.Any(x => x == _dataScope))
            {
                orderByParams.Add(Util.AddBoostingScoreSorting());
                orderByParams.Add(Util.AddMediaTypeRankSorting());
            }

            return orderByParams;
        }

        public SearchResult<T> Execute(ISolrQuery solrQuery, int start = 0, int rows = 10, OrderBy orderBy = OrderBy.workId, SortBy sortBy = SortBy.Asc)
        {
            var paramsList = new List<OrderByParam>();
            orderBy.GetOrderByParams(sortBy, ref paramsList);
            return solrQuery == null ? null : _solrSearch.Search(solrQuery, start, rows, paramsList);
        }
    }
}