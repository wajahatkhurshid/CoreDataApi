using System.Collections.Generic;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;

namespace Gyldendal.Api.CoreData.Common.DataProviderInfrastructure
{
    public interface IWorkReviewDataProvider
    {
        /// <summary>
        /// Gets WorkReview for given webshop
        /// </summary>
        /// <param name="dataScope">Webshop</param>
        /// <returns>List of WorkReview</returns>
        List<WorkReview> GetWorkReviewsByShop(DataScope dataScope);

        /// <summary>
        /// Gets work reviews for given work id
        /// </summary>
        /// <param name="workId"></param>
        /// <param name="webShop"></param>
        /// <returns>WorkReview</returns>
        List<WorkReview> GetWorkReviewsByWorkId(int workId, WebShop webShop);
    }
}