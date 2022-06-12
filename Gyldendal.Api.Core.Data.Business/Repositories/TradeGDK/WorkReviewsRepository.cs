using System;
using System.Collections.Generic;
using System.Linq;
using Gyldendal.Api.CoreData.Business.Repositories.Common;
using Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Repositories;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Response;
using Gyldendal.Api.CoreData.DataAccess.KoncernData;

namespace Gyldendal.Api.CoreData.Business.Repositories.TradeGDK
{
    public class WorkReviewsRepository : BaseRepository, IWorkReviewsRepository
    {
        public WorkReviewsRepository(koncerndata_webshops_Entities kdEntities) : base(DataScope.TradeGyldendalDk, kdEntities)
        {
        }

        /// <summary>
        /// Gets count of updated work reviews
        /// </summary>
        /// <param name="updatedAfter"></param>
        /// <returns>Count of updated work reviews</returns>
        public int GetUpdatedWorkReviewsCount(DateTime updatedAfter)
        {
            return KdEntities.ConsolidatedWorkReviewLogView.Count(x => x.CreatedDate > updatedAfter);
        }

        /// <summary>
        /// Get update info of the work review
        /// </summary>
        /// <param name="updatedAfterDateTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns>List of WorkReviewUpdateInfo</returns>
        public IEnumerable<WorkReviewUpdateInfo> GetWorkReviewsUpdateInfo(DateTime updatedAfterDateTime, int pageIndex, int pageSize)
        {
            if (pageIndex < 0)
            {
                throw new ArgumentException($"Value for {nameof(pageIndex)} should be greater than or equal to 0.");
            }

            if (pageSize < 1)
            {
                throw new ArgumentException($"Value for {nameof(pageSize)} should be greater than 0.");
            }

            var skip = pageIndex * pageSize;

            return KdEntities.ConsolidatedWorkReviewLogView.Where(x => x.CreatedDate > updatedAfterDateTime)
                .OrderBy(x => x.WorkReviewId)
                .Skip(skip)
                .Take(pageSize)
                .ToArray()
                .Select(x => new WorkReviewUpdateInfo
                {
                    WorkReviewId = x.WorkReviewId,
                    UpdateTime = x.CreatedDate,
                    UpdateType = x.Action.GetUpdateType<WorkReviewUpdateType>(),
                })
                .ToArray();
        }

        /// <summary>
        /// Gets details of a WorkReview
        /// </summary>
        /// <param name="id">Identifier of a WorkReview</param>
        /// <returns>Details of a WorkReview</returns>
        public WorkReview GetWorkReview(int id)
        {
            var kdWorkReview = KdEntities.ConsolidatedWorkReviewView.FirstOrDefault(x => x.WorkReviewId == id);

            return kdWorkReview?.ToCoreDataWorkReview();
        }

        public List<WorkReview> GetWorkReviews(string isbn)
        {
            throw new NotImplementedException();
        }
    }
}