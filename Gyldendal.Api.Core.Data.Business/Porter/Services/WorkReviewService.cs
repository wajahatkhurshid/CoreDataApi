using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gyldendal.Api.CoreData.Business.Porter.Interfaces;
using Gyldendal.Api.CoreData.Business.Porter.Mapping;
using Gyldendal.Api.CoreData.Common.Exceptions;
using Gyldendal.Api.CoreData.Contracts.Response;
using Gyldendal.Api.CoreData.Services.PorterApiClient;
using LoggingManager;

namespace Gyldendal.Api.CoreData.Business.Porter.Services
{
    public class WorkReviewService : IWorkReviewService
    {
        private readonly IPorterClient _porterClient;

        private readonly ILogger _logger;

        public WorkReviewService(IPorterClient porterClient, ILogger logger)
        {
            _porterClient = porterClient;
            _logger = logger;
        }
        
        public async Task<Contracts.Models.WorkReview> GetWorkReviewByIdAsync(CommonContracts.WebShop webShop, string id)
        {
            var response = await _porterClient.ApiV1WorkreviewGetAsync(new GetWorkReviewQuery()
            {
                Id = id
            });

            if (response == null)
            {
                _logger.Info($"WorkReview for Id: {id} not found.");
                throw new NotFoundException($"WorkReview for Id: {id} not found.");
            }

            return response.WorkReview.ToCoreDataWorkReview();
        }

        public async Task<List<Contracts.Models.WorkReview>> GetWorkReviewsByIsbn(CommonContracts.WebShop webShop, string isbn)
        {
            var reviews = await _porterClient.ApiV1WorkreviewsGetworkreviewsAsync(new GetWorkReviewsByProductQuery
            {
                WebShop = webShop.ToPorterWebShop(),
                Isbn = isbn
            });

            // Based on porter's implementation, it won't return null
            if (reviews.Count == 0)
            {
                _logger.Info($"WorkReview for Isbn: {isbn} and WebShop: {webShop} not found.");
            }

            return reviews.Select(r=> r.ToCoreDataWorkReview()).ToList();
        }

        public async Task<int> GetUpdatedWorkReviewsCountAsync(long updatedAfterTicks)
        {
            return await _porterClient.ApiV1WorkreviewGetupdatedworkreviewscountAsync(updatedAfterTicks);
        }

        public async Task<List<WorkReviewUpdateInfo>> GetWorkReviewsUpdateInfoAsync(long updatedAfterTicks, int pageIndex, int pageSize)
        {
            var response =
                await _porterClient.ApiV1WorkreviewGetworkreviewsupdateinfoAsync(updatedAfterTicks, pageIndex, pageSize);

            if (!response.Any())
            {
                _logger.Info($"WorkReview UpdateInfo not found.");
                throw new NotFoundException($"WorkReview UpdateInfo not found.");
            }

            return response.ToCoreDataWorkReviewUpdateInfo();
        }

        public async Task<bool> IsWorkReviewDataAvailable()
        {
            return await _porterClient.ApiV1WorkreviewIsworkreviewdataavailableAsync();
        }
    }
}
