using System;
using System.Collections.Generic;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Requests;
using Gyldendal.Api.CoreData.Contracts.Response;

namespace Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Repositories
{
    public interface IContributorRepository : ICoreDataRepository
    {
        /// <summary>
        /// Gets details of a contributor
        /// </summary>
        /// <param name="id">Identifier of a contributor</param>
        /// <returns>Details of a contributor</returns>
        T GetContributor<T>(string id) where T : BaseContributorDetails;

        ///// <summary>
        ///// Gets details of a contributor
        ///// </summary>
        ///// <param name="id">Identifier of a contributor</param>
        ///// <returns>Details of a contributor</returns>
        //ContributorDetailsV2 GetContributorV2(string id);

        /// <summary>
        /// Gets all contributors for given type and webshop
        /// </summary>
        /// <param name="searchRequest">Contributor search request</param>
        /// <returns>List of contributors</returns>
        SearchContributorResponse<T> GetContributors<T>(SearchContributorRequest searchRequest) where T : BaseContributorDetails;

        int GetUpdatedContributorsCount(DateTime updateAfterDateTime);

        IEnumerable<ContributorUpdateInfo> GetContributorsUpdateInfo(DateTime updatedAfterDateTime, int pageIndex, int pageSize);

        ContributorUpdateInfo GetContributorUpdateInfo(string contributorId);
    }
}
