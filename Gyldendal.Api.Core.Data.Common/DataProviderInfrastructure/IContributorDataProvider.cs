using System.Collections.Generic;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Requests;
using Gyldendal.Api.CoreData.Contracts.Response;

namespace Gyldendal.Api.CoreData.Common.DataProviderInfrastructure
{
    public interface IContributorDataProvider
    {
        List<ContributorDetails> GetContributorsByIds(IEnumerable<string> contributorIds, DataScope dataScope);

        List<ContributorDetailsV2> GetContributorsByIdsV2(IEnumerable<string> contributorIds, DataScope dataScope);

        List<ContributorDetails> GetContributorsByIds(IEnumerable<string> contributorIds, WebShop webShop);

        List<ContributorDetailsV2> GetContributorsByIdsV2(IEnumerable<string> contributorIds, WebShop webShop);

        List<ContributorDetails> GetContributorsByShop(DataScope dataScope);

        List<ContributorDetailsV2> GetContributorsByShopV2(DataScope dataScope);

        /// <summary>
        /// Searched Contributors based on search request.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        SearchContributorResponse<T> Search<T>(SearchContributorRequest request) where T : BaseContributorDetails;

        SearchContributorResponse<T> Search<T>(SearchContributorByWebShopsRequest request) where T : BaseContributorDetails;

        SearchContributorResponse<T> SearchBySearchName<T>(SearchContributorRequest request) where T : BaseContributorDetails;
    }
}