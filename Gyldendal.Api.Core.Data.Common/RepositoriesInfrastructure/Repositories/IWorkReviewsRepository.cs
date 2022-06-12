using System;
using System.Collections.Generic;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Response;

namespace Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Repositories
{
    public interface IWorkReviewsRepository : ICoreDataRepository
    {
        /// <summary>
        /// Gets count of updated work reviews
        /// </summary>
        /// <param name="updatedAfter"></param>
        /// <returns>Count of updated work reviews</returns>
        int GetUpdatedWorkReviewsCount(DateTime updatedAfter);

        /// <summary>
        /// Get update info of the work review
        /// </summary>
        /// <param name="updatedAfterDateTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns>List of WorkReviewUpdateInfo</returns>
        IEnumerable<WorkReviewUpdateInfo> GetWorkReviewsUpdateInfo(DateTime updatedAfterDateTime, int pageIndex, int pageSize);

        /// <summary>
        /// Gets details of a WorkReview
        /// </summary>
        /// <param name="id">Identifier of a WorkReview</param>
        /// <returns>Details of a WorkReview</returns>
        WorkReview GetWorkReview(int id);

        /// <summary>
        /// Gets details of a WorkReview
        /// </summary>
        /// <param name="isbn">Identifier of a WorkReview</param>
        /// <returns>Details of a WorkReview</returns>
        List<WorkReview> GetWorkReviews(string isbn);
    }
}