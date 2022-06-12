using System;
using System.Collections.Generic;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Requests;
using Gyldendal.Api.CoreData.Contracts.Response;

namespace Gyldendal.Api.CoreData.ApiClient
{
    public partial class CoreDataServiceClient
    {
        private const string ContributorController = "v1/Contributor";

        private const string ContributorControllerV2 = "v2/Contributor";

        /// <summary>
        /// Gets details of a contributor against given id
        /// </summary>
        /// <param name="webshop">Webshop</param>
        /// <param name="id">Id</param>
        /// <returns>Contributor Details</returns>
        public ContributorDetails GetContributor(WebShop webshop, string id)
        {
            var queryString = $"{ContributorController}/GetContributor/{webshop}/{id}";

            return HttpClient.GetAsync<ContributorDetails>(queryString);
        }

        /// <summary>
        /// Gets details of a contributor against given id
        /// </summary>
        /// <param name="webshop">Webshop</param>
        /// <param name="id">Id</param>
        /// <returns>Contributor Details</returns>
        public ContributorDetailsV2 GetContributorV2(WebShop webshop, string id)
        {
            var queryString = $"{ContributorControllerV2}/GetContributor/{webshop}/{id}";

            return HttpClient.GetAsync<ContributorDetailsV2>(queryString);
        }

        /// <summary>
        /// Gets details of a contributor against given id
        /// </summary>
        /// <param name="webshop">Webshop</param>
        /// <param name="searchRequest">Contributor search request</param>
        /// <returns>Contributor Details</returns>
        public SearchContributorResponse<ContributorDetails> GetContributorsByType(WebShop webshop, SearchContributorRequest searchRequest)
        {
            var queryString = $"{ContributorController}/GetContributorsByType/{webshop}";

            return HttpClient.PostAsync<SearchContributorResponse<ContributorDetails>, SearchContributorRequest>(queryString, searchRequest);
        }

        /// <summary>
        /// Gets details of a contributor against given id
        /// </summary>
        /// <param name="webshop">Webshop</param>
        /// <param name="searchRequest">Contributor search request</param>
        /// <returns>Contributor Details</returns>
        public SearchContributorResponse<ContributorDetailsV2> GetContributorsByTypeV2(WebShop webshop, SearchContributorRequest searchRequest)
        {
            var queryString = $"{ContributorControllerV2}/GetContributorsByType/{webshop}";

            return HttpClient.PostAsync<SearchContributorResponse<ContributorDetailsV2>, SearchContributorRequest>(queryString, searchRequest);
        }

        /// <summary>
        /// Returns the number of contributors updated after the given Ticks value,for the given WebShop.
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="updatedAfter"></param>
        /// <returns></returns>
        public int GetUpdatedContributorsCount(DataScope dataScope, DateTime updatedAfter)
        {
            var queryString = $"{ContributorController}/GetUpdatedContributorsCount/{dataScope}/{updatedAfter.Ticks}";
            return HttpClient.GetAsync<int>(queryString);
        }

        /// <summary>
        /// Returns the asked page of ContributorupdatedInfo objects, for each contributor,
        /// that was updated after the given DateTime, in KoncernDataWebShops database.
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="updatedAfterTicks"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<ContributorUpdateInfo> GetContributorsUpdateInfo(DataScope dataScope, DateTime updatedAfterTicks, int pageIndex, int pageSize)
        {
            var queryString = $"{ContributorController}/GetContributorsUpdateInfo/{dataScope}/{updatedAfterTicks.Ticks}/{pageIndex}/{pageSize}";
            return HttpClient.GetAsync<IEnumerable<ContributorUpdateInfo>>(queryString);
        }

        /// <summary>
        /// Gets details of a contributor against given id
        /// </summary>
        /// <param name="dataScope">dataScope</param>
        /// <param name="id">Id</param>
        /// <returns>Contributor Details</returns>
        public ContributorDetails GetContributorDetail(DataScope dataScope, string id)
        {
            var queryString = $"{ContributorController}/GetContributorDetail/{dataScope}/{id}";
            return HttpClient.GetAsync<ContributorDetails>(queryString);
        }

        /// <summary>
        /// Gets details of a contributor against given id
        /// </summary>
        /// <param name="dataScope">dataScope</param>
        /// <param name="id">Id</param>
        /// <returns>Contributor Details</returns>
        public ContributorDetailsV2 GetContributorDetailV2(DataScope dataScope, string id)
        {
            var queryString = $"{ContributorControllerV2}/GetContributorDetail/{dataScope}/{id}";
            return HttpClient.GetAsync<ContributorDetailsV2>(queryString);
        }

        /// <summary>
        /// Determines if the contributor data will be available in Koncerndata for the next x minutes.
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="xMinutes">The number of minutes by which the contributor data will be available or not.</param>
        /// <returns></returns>
        public bool IsContributorDataAvailableForXMinutes(DataScope dataScope, short xMinutes)
        {
            var queryString = $"{ContributorController}/IsContributorDataAvailableForXMinutes/{dataScope}/{xMinutes}";
            return HttpClient.GetAsync<bool>(queryString);
        }

        /// <summary>
        /// Gets List contributor against given contributor ids
        /// </summary>
        /// <param name="dataScope">dataScope</param>
        /// <param name="ids">Id</param>
        /// <returns>Contributor Details</returns>
        public List<ContributorDetails> GetContributorsByIdsFromSolr(DataScope dataScope, IList<string> ids)
        {
            var queryString = $"{ContributorController}/GetContributorsByIdsFromSolr/{dataScope}";
            return HttpClient.PostAsync<List<ContributorDetails>, IList<string>>(queryString, ids);
        }

        /// <summary>
        /// Gets List contributor against given contributor ids
        /// </summary>
        /// <param name="dataScope">dataScope</param>
        /// <param name="ids">Id</param>
        /// <returns>Contributor Details</returns>
        public List<ContributorDetailsV2> GetContributorsByIdsFromSolrV2(DataScope dataScope, IList<string> ids)
        {
            var queryString = $"{ContributorControllerV2}/GetContributorsByIdsFromSolr/{dataScope}";
            return HttpClient.PostAsync<List<ContributorDetailsV2>, IList<string>>(queryString, ids);
        }

        /// <summary>
        /// Gets List contributor against given contributor ids
        /// </summary>
        /// <param name="webShop">dataScope</param>
        /// <param name="ids">Id</param>
        /// <returns>Contributor Details</returns>
        public List<ContributorDetails> GetContributorsByIds(WebShop webShop, IList<string> ids)
        {
            var queryString = $"{ContributorController}/GetContributorsByIds/{webShop}";
            return HttpClient.PostAsync<List<ContributorDetails>, IList<string>>(queryString, ids);
        }

        /// <summary>
        /// Gets List contributor against given contributor ids
        /// </summary>
        /// <param name="webShop">dataScope</param>
        /// <param name="ids">Id</param>
        /// <returns>Contributor Details</returns>
        public List<ContributorDetailsV2> GetContributorsByIdsV2(WebShop webShop, IList<string> ids)
        {
            var queryString = $"{ContributorControllerV2}/GetContributorsByIds/{webShop}";
            return HttpClient.PostAsync<List<ContributorDetailsV2>, IList<string>>(queryString, ids);
        }

        /// <summary>
        /// Gets List of contributor against given shop id
        /// </summary>
        /// <param name="dataScope">dataScope</param>
        /// <returns>Contributor Details</returns>
        public List<ContributorDetails> GetContributorsByShopFromSolr(DataScope dataScope)
        {
            var queryString = $"{ContributorController}/GetContributorsByShopFromSolr/{dataScope}";
            return HttpClient.GetAsync<List<ContributorDetails>>(queryString);
        }

        /// <summary>
        /// Gets List of contributor against given shop id
        /// </summary>
        /// <param name="dataScope">dataScope</param>
        /// <returns>Contributor Details</returns>
        public List<ContributorDetailsV2> GetContributorsByShopFromSolrV2(DataScope dataScope)
        {
            var queryString = $"{ContributorControllerV2}/GetContributorsByShopFromSolr/{dataScope}";
            return HttpClient.GetAsync<List<ContributorDetailsV2>>(queryString);
        }

        /// <summary>
        /// Searches Contributors against the provided query string.
        /// </summary>
        /// <param name="request">request</param>
        /// <returns>Contributor Details</returns>
        public SearchContributorResponse<ContributorDetails> Search(SearchContributorRequest request)
        {
            var queryString = $"{ContributorController}/Search";
            return HttpClient.PostAsync<SearchContributorResponse<ContributorDetails>, SearchContributorRequest>(queryString, request);
        }

        /// <summary>
        /// Searches Contributors against the provided query string.
        /// </summary>
        /// <param name="request">request</param>
        /// <returns>Contributor Details</returns>
        public SearchContributorResponse<ContributorDetailsV2> SearchV2(SearchContributorRequest request)
        {
            var queryString = $"{ContributorControllerV2}/Search";
            return HttpClient.PostAsync<SearchContributorResponse<ContributorDetailsV2>, SearchContributorRequest>(queryString, request);
        }

        /// <summary>
        /// Searches Contributors against the provided request having multiple webshops.
        /// </summary>
        /// <param name="request">request</param>
        /// <returns>Contributor Details</returns>
        public SearchContributorResponse<ContributorDetails> Search(SearchContributorByWebShopsRequest request)
        {
            var queryString = $"{ContributorController}/SearchByWebShops";
            return HttpClient.PostAsync<SearchContributorResponse<ContributorDetails>, SearchContributorByWebShopsRequest>(queryString, request);
        }

        /// <summary>
        /// Searches Contributors against the provided request having multiple webshops.
        /// </summary>
        /// <param name="request">request</param>
        /// <returns>Contributor Details</returns>
        public SearchContributorResponse<ContributorDetailsV2> SearchV2(SearchContributorByWebShopsRequest request)
        {
            var queryString = $"{ContributorControllerV2}/SearchByWebShops";
            return HttpClient.PostAsync<SearchContributorResponse<ContributorDetailsV2>, SearchContributorByWebShopsRequest>(queryString, request);
        }

        /// <summary>
        /// Searches Contributors against the provided query string.
        /// </summary>
        /// <param name="request">request</param>
        /// <returns>Contributor Details</returns>
        public SearchContributorResponse<ContributorDetails> SearchBySearchName(SearchContributorRequest request)
        {
            var queryString = $"{ContributorController}/SearchBySearchName";
            return HttpClient.PostAsync<SearchContributorResponse<ContributorDetails>, SearchContributorRequest>(queryString, request);
        }

        /// <summary>
        /// Searches Contributors against the provided query string.
        /// </summary>
        /// <param name="request">request</param>
        /// <returns>Contributor Details</returns>
        public SearchContributorResponse<ContributorDetailsV2> SearchBySearchNameV2(SearchContributorRequest request)
        {
            var queryString = $"{ContributorControllerV2}/SearchBySearchName";
            return HttpClient.PostAsync<SearchContributorResponse<ContributorDetailsV2>, SearchContributorRequest>(queryString, request);
        }

        /// <summary>
        /// Returns the latest update info for synchronization purposes, for the given contributor id and data scope. (Source: KD)
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="contributorId"></param>
        /// <returns></returns>
        public ContributorUpdateInfo GetContributorUpdateInfo(DataScope dataScope, string contributorId)
        {
            var queryString = $"{ContributorController}/GetContributorUpdateInfo/{dataScope}/{contributorId}";
            return HttpClient.GetAsync<ContributorUpdateInfo>(queryString);
        }
    }
}