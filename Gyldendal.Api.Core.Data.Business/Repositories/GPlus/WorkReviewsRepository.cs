using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Common;
using Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Repositories;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Response;
using Gyldendal.Api.CoreData.DataAccess.KoncernData;
using Gyldendal.Common.WebUtils.Exceptions;
using Gyldendal.PulsenServices.ApiClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gyldendal.Api.CoreData.Business.Repositories.GPlus
{
    public class WorkReviewsRepository : IWorkReviewsRepository
    {
        public DataScope DataScope { get; }

        private readonly PulsenServiceApiClient _pulsenServiceApiClient;

        private readonly koncerndata_webshops_Entities _kdEntities;

        public WorkReviewsRepository(PulsenServiceApiClient pulsenServiceApiClient, koncerndata_webshops_Entities kdEntities)
        {
            _pulsenServiceApiClient = pulsenServiceApiClient;
            _kdEntities = kdEntities;
            DataScope = DataScope.GyldendalPlus;
        }

        /// <summary>
        /// Gets count of updated work reviews
        /// </summary>
        /// <param name="updatedAfter"></param>
        /// <returns>Count of updated work reviews</returns>
        public int GetUpdatedWorkReviewsCount(DateTime updatedAfter)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets details of a WorkReview
        /// </summary>
        /// <param name="id">Identifier of a WorkReview</param>
        /// <returns>Details of a WorkReview</returns>
        public WorkReview GetWorkReview(int id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// return workId from koncernData base on isbn
        /// </summary>
        /// <param name="isbn"></param>
        /// <returns></returns>
        private int GetProductByIsbn(string isbn)
        {
            var workId = _kdEntities.DEA_KDWS_GPlusproduct
                .FirstOrDefault(p => p.vare_id.Equals(isbn))?.work_id;

            if (workId == null)
                throw new ValidationException((ulong)ErrorCodes.NullValue, "workId not found for this product in KD.", Extensions.CoreDataSystemName, null);

            return (int)workId;
        }

        public List<WorkReview> GetWorkReviews(string isbn)
        {
            try
            {
                var getReviews = Task.Run(() => _pulsenServiceApiClient.Product.GetProductReviews(isbn));

                var getWorkId = Task.Run(() => GetProductByIsbn(isbn));

                Task.WaitAll(getReviews, getWorkId);

                var workId = getWorkId.Result;
                var productReviews = getReviews.Result;

                return productReviews.Select(x => x.ToCoreDataWorkReview(workId, WebShop.ClubBogklub)).ToList();
            }
            catch (AggregateException e)
            {
                throw e.InnerException ?? e;
            }
        }
    }
}