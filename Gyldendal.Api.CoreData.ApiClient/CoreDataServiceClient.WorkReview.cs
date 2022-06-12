using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Response;

namespace Gyldendal.Api.CoreData.ApiClient
{
    public partial class CoreDataServiceClient
    {
        private const string WorkReviewsController = "v1/WorkReviews";

        /// <summary>
        /// Gets List of WorkReviews against given shop id
        /// </summary>
        /// <param name="dataScope">dataScope</param>
        /// <returns>List of WorkReview</returns>
        public List<WorkReview> GetWorkReviewsByShopFromSolr(DataScope dataScope)
        {
            var queryString = $"{WorkReviewsController}/GetWorkReviewsByShopFromSolr/{dataScope}";
            return HttpClient.GetAsync<List<WorkReview>>(queryString);
        }

        /// <summary>
        /// Gets work reviews for given work id
        /// </summary>
        /// <param name="workId"></param>
        /// <param name="webShop"></param>
        /// <returns>WorkReview</returns>
        public List<WorkReview> GetWorkReviewsByWorkIdFromSolr(int workId, WebShop webShop = WebShop.None)
        {
            var queryString = $"{WorkReviewsController}/GetWorkReviewsByWorkIdFromSolr?workId={workId}&webShop={webShop}";
            return HttpClient.GetAsync<List<WorkReview>>(queryString);
        }

        /// <summary>
        /// Returns the number of WorkReviews updated after the given DateTime value, for the given WebShop.
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="updateAfterDateTime"></param>
        /// <returns></returns>
        public int GetUpdatedWorkReviewsCount(DataScope dataScope, DateTime updateAfterDateTime)
        {
            return HttpClient.GetAsync<int>($"{WorkReviewsController}/GetUpdatedWorkReviewsCount/{dataScope}/{updateAfterDateTime.Ticks}");
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
            return HttpClient.GetAsync<IEnumerable<WorkReviewUpdateInfo>>(
                $"{WorkReviewsController}/GetWorkReviewsUpdateInfo/{dataScope}/{updatedAfterDateTime.Ticks}/{pageIndex}/{pageSize}");
        }

        /// <summary>
        /// Gets details of a WorkReview
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="id">Identifier of a WorkReview</param>
        /// <returns>Details of a WorkReview</returns>
        public WorkReview GetWorkReview(DataScope dataScope, int id)
        {
            return HttpClient.GetAsync<WorkReview>($"{WorkReviewsController}/GetWorkReview/{dataScope}/{id}");
        }

        /// <summary>
        /// Determines if the WorkReviews data will be available in Koncerndata for the next x minutes.
        /// </summary>
        /// <param name="webShop"></param>
        /// <param name="xMinutes">The number of minutes by which the WorkReviews data will be available or not.</param>
        /// <returns></returns>
        public bool IsWorkReviewsDataAvailableForXMinutes(WebShop webShop, short xMinutes)
        {
            return HttpClient.GetAsync<bool>(
                $"{WorkReviewsController}/IsWorkReviewsDataAvailableForXMinutes/{webShop}/{xMinutes}");
        }

        /// <summary>
        /// Get reviews from pulsen services against Gyldendal plus products
        /// </summary>
        /// <param name="isbn"></param>
        /// <param name="dataScope"></param>
        /// <returns></returns>
        public List<WorkReview> GetWorkReviews(string isbn, DataScope dataScope)
        {
            return HttpClient.GetAsync<List<WorkReview>>(
                $"{WorkReviewsController}/GetWorkReviews/{dataScope}/{isbn}");
        }
    }
}