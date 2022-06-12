using System.Collections.Generic;
using System.Threading.Tasks;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Response;

namespace Gyldendal.Api.CoreData.Business.Porter.Interfaces
{
    public interface IContributorService
    {
        Task<ContributorDetails> GetContributorByIdAsync(WebShop webShop, string id);
        Task<int> GetUpdatedContributorsCountAsync(long updatedAfterTicks, WebShop webShop);
        Task<ContributorUpdateInfo> GetContributorUpdateInfo(string contributorId, WebShop webShop);

        Task<IEnumerable<ContributorUpdateInfo>>
            GetContributorsUpdateInfo(long updatedAfterTicks, int pageIndex, int pageSize, WebShop webShop);

        Task<ContributorDetailsV2> GetContributorDetailAsync(WebShop webShop, string id);

        Task<bool> IsContributorDataAvailable();
    }
}
