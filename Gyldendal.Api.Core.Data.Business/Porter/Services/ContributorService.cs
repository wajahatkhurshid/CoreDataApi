using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gyldendal.Api.CoreData.Business.Porter.Interfaces;
using Gyldendal.Api.CoreData.Business.Porter.Mapping;
using Gyldendal.Api.CoreData.Common.Exceptions;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Response;
using LoggingManager;
using PorterApi = Gyldendal.Api.CoreData.Services.PorterApiClient;
using WebShop = Gyldendal.Api.CommonContracts.WebShop;

namespace Gyldendal.Api.CoreData.Business.Porter.Services
{
    public class ContributorService : IContributorService
    {
        PorterApi.IPorterClient _porterClient;

        private readonly IServiceDataProviderFactory _serviceDataProviderFactory;

        private readonly ILogger _logger;

        public ContributorService(PorterApi.IPorterClient porterClient, IServiceDataProviderFactory serviceDataProviderFactory, ILogger logger)
        {
            _porterClient = porterClient;
            _serviceDataProviderFactory = serviceDataProviderFactory;
            _logger = logger;
        }

        public async Task<ContributorDetails> GetContributorByIdAsync(WebShop webShop, string id)
        {
            var response = await _porterClient.ApiV1ContributorGetAsync(new PorterApi.GetContributorQuery()
            {
                Id = id,
                WebShop = webShop.ToPorterWebShop()
            });

            if (response == null)
                throw new NotFoundException($"Contributor for Id: {id} not found.");

            return response.ToCoreDataContributors(webShop);
        }

        public async Task<ContributorUpdateInfo> GetContributorUpdateInfo(string contributorId, WebShop webShop)
        {
            var contributor = await _porterClient.ApiV1ContributorGetcontributorupdateinfoAsync(webShop.ToPorterWebShop(), contributorId);

            if (contributor == null)
                throw new NotFoundException($"Contributor for Id: {contributorId} not found.");

            return contributor.ToCoreDataContributors();

        }

        public async Task<IEnumerable<ContributorUpdateInfo>> GetContributorsUpdateInfo(long updatedAfterTicks, int pageIndex, int pageSize, WebShop webShop)
        {
            var contributors = await _porterClient.ApiV1ContributorGetcontributorsupdateinfoAsync(webShop.ToPorterWebShop(), updatedAfterTicks, pageIndex, pageSize);

            if (contributors == null)
                throw new NotFoundException($"Contributors not found.");

            return contributors.Select(x => x.ToCoreDataContributors()).ToList();

        }
        public async Task<int> GetUpdatedContributorsCountAsync(long updatedAfterTicks, WebShop webShop)
        {
            var contributorsCount =
                  await _porterClient.ApiV1ContributorGetupdatedcontributorscountAsync(webShop.ToPorterWebShop(), updatedAfterTicks);

            return contributorsCount;
        }

        public async Task<ContributorDetailsV2> GetContributorDetailAsync(WebShop webShop, string id)
        {
            var contributorDetail = await GetContributorByIdAsync(webShop, id);
            var serviceDataProvider = _serviceDataProviderFactory.GetContributorServiceDataProvider(webShop);
            var images =
                serviceDataProvider.GetContributorImages(contributorDetail.Id, contributorDetail.Url);
            return contributorDetail.ToContributorDetailsV2(images);
        }

        public async Task<bool> IsContributorDataAvailable()
        {
            return await _porterClient.ApiV1ContributorIscontributordataavailableAsync();
        }
    }
}
