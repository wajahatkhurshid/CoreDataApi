using System;
using System.Collections.Generic;
using System.Linq;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Business.ExtensionMethods;
using Gyldendal.Api.CoreData.Common.DataProviderInfrastructure;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Requests;
using Gyldendal.Api.CoreData.Contracts.Requests.Common;
using Gyldendal.Api.CoreData.Contracts.Response;
using Gyldendal.Api.CoreData.GqlToSolrConnector.Model;
using Gyldendal.Api.CoreData.GqlToSolrConnector.SolrSearch;
using Gyldendal.Api.CoreData.SolrContracts.Contributor;
using Gyldendal.Api.CoreData.SolrDataProviders.Infrastructure;
using Gyldendal.Api.CoreData.SolrDataProviders.Infrastructure.FilterGenerator;
using Gyldendal.Api.CoreData.SolrDataProviders.Infrastructure.Models;
using Gyldendal.Api.CoreData.SolrDataProviders.Mappings;
using Gyldendal.Api.CoreData.SolrDataProviders.Utils;
using SolrNet;
using FilterInfo = Gyldendal.Api.CoreData.GqlToSolrConnector.Model.FilterInfo;

namespace Gyldendal.Api.CoreData.SolrDataProviders.Contributor
{
    public class ContributorDataProvider :
        BaseDataProvider<ContributorFilterGenerationInput>,
        IContributorDataProvider
    {
        private readonly ISearch<SolrContracts.Contributor.Contributor> _solrSearch;

        public ContributorDataProvider(string coreName, string solrServerUrl,
            IFilterGenerator<ContributorFilterGenerationInput> filterGenerator,
            IFilterInfoToSolrQueryBuilder solrQueryBuilder,
            ISearch<SolrContracts.Contributor.Contributor> solrSearch)
            : base(solrQueryBuilder, filterGenerator, solrServerUrl, coreName)
        {
            _solrSearch = solrSearch;
        }

        public List<ContributorDetails> GetContributorsByIds(IEnumerable<string> contributorIds, DataScope dataScope)
        {
            var idsList = contributorIds.ToList();
            var webShops = dataScope.ToWebShops();

            return GetContributors(webShops, idsList);
        }

        public List<ContributorDetailsV2> GetContributorsByIdsV2(IEnumerable<string> contributorIds, DataScope dataScope)
        {
            var idsList = contributorIds.ToList();
            var webShops = dataScope.ToWebShops();

            return GetContributorsV2(webShops, idsList);
        }

        public List<ContributorDetails> GetContributorsByIds(IEnumerable<string> contributorIds, WebShop webShop)
        {
            var idsList = contributorIds.ToList();
            var webShops = new[] { webShop };

            return GetContributors(webShops, idsList);
        }

        public List<ContributorDetailsV2> GetContributorsByIdsV2(IEnumerable<string> contributorIds, WebShop webShop)
        {
            var idsList = contributorIds.ToList();
            var webShops = new[] { webShop };

            return GetContributorsV2(webShops, idsList);
        }

        private List<ContributorDetails> GetContributors(IEnumerable<WebShop> webShops, IEnumerable<string> idsList)
        {
            var result = GetContributorSearchResults(webShops, idsList);

            return result.ItemsFound.Select(x => x.ToCoreDataContributor()).ToList();
        }

        private List<ContributorDetailsV2> GetContributorsV2(IEnumerable<WebShop> webShops, IEnumerable<string> idsList)
        {
            var result = GetContributorSearchResults(webShops, idsList);

            return result.ItemsFound.Select(x => x.ToCoreDataContributorV2()).ToList();
        }

        public List<ContributorDetails> GetContributorsByShop(DataScope dataScope)
        {
            var result = GetContributorSearchResults(dataScope);

            return result.ItemsFound.Select(x => x.ToCoreDataContributor()).ToList();
        }

        public List<ContributorDetailsV2> GetContributorsByShopV2(DataScope dataScope)
        {
            var result = GetContributorSearchResults(dataScope);

            return result.ItemsFound.Select(x => x.ToCoreDataContributorV2()).ToList();
        }

        /// <summary>
        /// Searches Contributors based upon search Request having DataScope provided.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public SearchContributorResponse<T> Search<T>(SearchContributorRequest request) where T : BaseContributorDetails
        {
            var solrQuery = AbstractSolrQuery(request.SearchString, request.DataScope.ToWebShops(), out var contributorSolrFilter);
            return DoContributorSearch<T>(request, contributorSolrFilter, solrQuery);
        }

        /// <summary>
        /// Searches Contributors based upon search Request having WebShops array provided.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public SearchContributorResponse<T> Search<T>(SearchContributorByWebShopsRequest request) where T : BaseContributorDetails
        {
            var solrQuery = AbstractSolrQuery(request.SearchString, request.WebShops, out var contributorSolrFilter);
            return DoContributorSearch<T>(request, contributorSolrFilter, solrQuery);
        }

        private AbstractSolrQuery AbstractSolrQuery(string searchString, IEnumerable<WebShop> webShops, out ContributorFilterGenerationInput contributorSolrFilter)
        {
            var solrQuery = GetSearchQuery(searchString);
            contributorSolrFilter = new ContributorFilterGenerationInput(webShops: webShops);
            return solrQuery;
        }

        /// <summary>
        /// Searches Contributors based upon search Request provided.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public SearchContributorResponse<T> SearchBySearchName<T>(SearchContributorRequest request) where T : BaseContributorDetails
        {
            var contributorSolrFilter = new ContributorFilterGenerationInput(webShops: request.DataScope.ToWebShops(), searchString: request.SearchString);
            return DoContributorSearch<T>(request, contributorSolrFilter);
        }

        private SearchContributorResponse<T> DoContributorSearch<T>(SearchContributorRequestBase request
            , ContributorFilterGenerationInput contributorSolrFilterGenerator, ISolrQuery solrQuery = null) where T : BaseContributorDetails
        {
            solrQuery = solrQuery ?? SolrQuery.All;

            var filters = GenerateSolrQuery(contributorSolrFilterGenerator);

            var result = _solrSearch.Search(solrQuery, request.PageIndex * request.PageSize,
                request.PageSize, null, null, null, filters);

            var response = new SearchContributorResponse<T>
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                TotalRecords = result.TotalResults
            };
            
            if (typeof(T) == typeof(ContributorDetails))
            {
                response.Contributors = (IEnumerable<T>) result.ItemsFound.Select(x => x.ToCoreDataContributor()).ToList();
            }
            else if (typeof(T) == typeof(ContributorDetailsV2))
            {
                response.Contributors = (IEnumerable<T>) result.ItemsFound.Select(x => x.ToCoreDataContributorV2()).ToList();
            }
            else
            {
                throw new ArgumentException($"Invalid argument for generic type: {nameof(T)}");
            }

            return response;
        }

        private AbstractSolrQuery GetSearchQuery(string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString)) return SolrQuery.All;

            return new SolrMultipleCriteriaQuery(new[]
            {
                ContributorSchemaField.ExactMatch.ToSolrQuery(searchString)
                    .Boost(ContributorSchemaField.ExactMatch.GetFieldBoost()),

                ContributorSchemaField.SubstringField.ToSolrQuery(FilterInfo.QuoteString(searchString), true)
                    .Boost(ContributorSchemaField.SubstringField.GetFieldBoost()),
            }, "OR");
        }

        private SearchResult<SolrContracts.Contributor.Contributor> GetContributorSearchResults(DataScope dataScope)
        {
            var filters =
                GenerateSolrQuery(new ContributorFilterGenerationInput(webShops: new[] { dataScope.ToWebShop() }));

            var result = _solrSearch.Search(SolrQuery.All, 0, 0, null, null, null, filters);
            return result;
        }

        private SearchResult<SolrContracts.Contributor.Contributor> GetContributorSearchResults(IEnumerable<WebShop> webShops, IEnumerable<string> idsList)
        {
            var filters = GenerateSolrQuery(new ContributorFilterGenerationInput(webShops: webShops, contributorIds: idsList));

            var result = _solrSearch.Search(SolrQuery.All, 0, 0, null, null, null, filters);
            return result;
        }
    }
}