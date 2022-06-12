using System;
using System.Collections.Generic;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Response;

namespace Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Factories
{
    public interface IWorkReviewsFactory
    {
        /// <summary>
        /// Returns the number of contributors updated after the given DateTime value, for the given WebShop.
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="updateAfterDateTime"></param>
        /// <returns></returns>
        int GetUpdatedWorkReviewsCount(DataScope dataScope, DateTime updateAfterDateTime);

        /// <summary>
        /// Get update info of the work review
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="updatedAfterDateTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns>List of WorkReviewUpdateInfo</returns>
        IEnumerable<WorkReviewUpdateInfo> GetWorkReviewsUpdateInfo(DataScope dataScope, DateTime updatedAfterDateTime, int pageIndex, int pageSize);

        /// <summary>
        /// Gets details of a WorkReview
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="id">Identifier of a WorkReview</param>
        /// <returns>Details of a WorkReview</returns>
        WorkReview GetWorkReview(DataScope dataScope, int id);

        /// <summary>
        /// Gets details of a WorkReview
        /// </summary>
        /// <param name="isbn"></param>
        /// <param name="dataScope">Identifier of a WorkReview</param>
        /// <returns>Details of a WorkReview</returns>
        List<WorkReview> GetWorkReviews(string isbn, DataScope dataScope);
    }
}