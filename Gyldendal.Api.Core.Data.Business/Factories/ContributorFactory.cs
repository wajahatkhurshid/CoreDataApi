using System;
using System.Collections.Generic;
using Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Factories;
using Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Repositories;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Requests;
using Gyldendal.Api.CoreData.Contracts.Response;

namespace Gyldendal.Api.CoreData.Business.Factories
{
    public class ContributorFactory : CoreDataFactory<IContributorRepository>, IContributorFactory
    {
        public ContributorFactory(IEnumerable<IContributorRepository> repositories) : base(repositories)
        {

        }

        /// <summary>
        /// Gets details of a contributor
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="id">Identifier of a contributor</param>
        /// <returns>Details of a contributor</returns>
        public T GetContributor<T>(DataScope dataScope, string id) where T : BaseContributorDetails
        {
            return this[dataScope].GetContributor<T>(id);
        }

        /// <summary>
        /// Gets all contributors for given type and webshop
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="searchRequest">Contributor search request</param>
        /// <returns>Search contributor response object</returns>
        public SearchContributorResponse<T> GetContributors<T>(DataScope dataScope, SearchContributorRequest searchRequest) where T : BaseContributorDetails
        {
            return this[dataScope].GetContributors<T>(searchRequest);
        }

        public int GetUpdatedContributorsCount(DataScope dataScope, DateTime updateAfterDateTime)
        {
            return this[dataScope].GetUpdatedContributorsCount(updateAfterDateTime);
        }

        /// <summary>
        /// Returns the asked page of ContributorupdatedInfo objects, for each contributor,
        /// that was updated after the given DateTime, in KoncernDataWebShops database.
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="updatedAfterDateTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<ContributorUpdateInfo> GetContributorsUpdateInfo(DataScope dataScope, DateTime updatedAfterDateTime, int pageIndex, int pageSize)
        {
            return this[dataScope].GetContributorsUpdateInfo(updatedAfterDateTime, pageIndex, pageSize);
        }

        public ContributorUpdateInfo GetContributorUpdateInfo(DataScope datScope, string contributorId)
        {
            return this[datScope].GetContributorUpdateInfo(contributorId);
        }
    }
}
