using System.Collections.Generic;
using System.Threading.Tasks;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Response;

namespace Gyldendal.Api.CoreData.Business.Porter.Interfaces
{
    public interface IWorkReviewService
    {
        Task<WorkReview> GetWorkReviewByIdAsync(WebShop webShop, string id);

        Task<List<WorkReview>> GetWorkReviewsByIsbn(WebShop webShop, string isbn);
        
        Task<int> GetUpdatedWorkReviewsCountAsync(long updatedAfterTicks);
        
        Task<List<WorkReviewUpdateInfo>> GetWorkReviewsUpdateInfoAsync(long updatedAfterTicks, int pageIndex, int pageSize);

        Task<bool> IsWorkReviewDataAvailable();
    }
}
