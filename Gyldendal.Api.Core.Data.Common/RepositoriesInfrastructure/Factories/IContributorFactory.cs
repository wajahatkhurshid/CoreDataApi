using System;
using System.Collections.Generic;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Requests;
using Gyldendal.Api.CoreData.Contracts.Response;

namespace Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Factories
{
    public interface IContributorFactory
    {
        /// <summary>
        /// Gets details of a contributor
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="id">Identifier of a contributor</param>
        /// <returns>Details of a contributor</returns>
        T GetContributor<T>(DataScope dataScope, string id) where T : BaseContributorDetails;

        ///// <summary>
        ///// Gets details of a contributor
        ///// </summary>
        ///// <param name="dataScope"></param>
        ///// <param name="id">Identifier of a contributor</param>
        ///// <returns>Details of a contributor</returns>
        //ContributorDetailsV2 GetContributorV2(DataScope dataScope, string id);

        /// <summary>
        /// Gets all contributors for given type and webshop
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="searchRequest">Contributor search request</param>
        /// <returns>List of contributors</returns>
        SearchContributorResponse<T> GetContributors<T>(DataScope dataScope, SearchContributorRequest searchRequest) where T : BaseContributorDetails;

        int GetUpdatedContributorsCount(DataScope dataScope, DateTime updateAfterDateTime);

        IEnumerable<ContributorUpdateInfo> GetContributorsUpdateInfo(DataScope dataScope, DateTime updatedAfterDateTime, int pageIndex, int pageSize);

        ContributorUpdateInfo GetContributorUpdateInfo(DataScope dataScope, string contributorId);
    }
}
