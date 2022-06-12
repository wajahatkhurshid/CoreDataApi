using System;
using System.Collections.Generic;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Business.ExtensionMethods;
using Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Factories;
using Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Repositories;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Response;

namespace Gyldendal.Api.CoreData.Business.Factories
{
    public class WorkReviewsFactory : CoreDataFactory<IWorkReviewsRepository>, IWorkReviewsFactory
    {
        public WorkReviewsFactory(IEnumerable<IWorkReviewsRepository> repositories) : base(repositories)
        {
        }

        /// <summary>
        /// Returns the number of WorkReviews updated after the given DateTime value, for the given WebShop.
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="updateAfterDateTime"></param>
        /// <returns></returns>
        public int GetUpdatedWorkReviewsCount(DataScope dataScope, DateTime updateAfterDateTime)
        {
            return this[dataScope].GetUpdatedWorkReviewsCount(updateAfterDateTime);
        }

        /// <summary>
        /// Get update info of the work review
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="updatedAfterDateTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns>List of WorkReviewUpdateInfo</returns>
        public IEnumerable<WorkReviewUpdateInfo> GetWorkReviewsUpdateInfo(DataScope dataScope, DateTime updatedAfterDateTime, int pageIndex, int pageSize)
        {
            return this[dataScope].GetWorkReviewsUpdateInfo(updatedAfterDateTime, pageIndex, pageSize);
        }

        /// <summary>
        /// Gets details of a WorkReview
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="id">Identifier of a WorkReview</param>
        /// <returns>Details of a WorkReview</returns>
        public WorkReview GetWorkReview(DataScope dataScope, int id)
        {
            return this[dataScope].GetWorkReview(id);
        }

        public List<WorkReview> GetWorkReviews(string isbn, DataScope dataScope)
        {
            return this[dataScope].GetWorkReviews(isbn);
        }
    }
}