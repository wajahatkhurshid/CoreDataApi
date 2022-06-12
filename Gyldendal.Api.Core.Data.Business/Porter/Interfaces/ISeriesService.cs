using System.Threading.Tasks;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Contracts.Models;
using CoreDataRequest = Gyldendal.Api.CoreData.Contracts.Requests;
using CoreDataResponse = Gyldendal.Api.CoreData.Contracts.Response;

namespace Gyldendal.Api.CoreData.Business.Porter.Interfaces
{
    public interface ISeriesService 
    {
        Task<Series> GetSeriesByIdAsync(WebShop webShop, int subjectId);

        Task<CoreDataResponse.GetSeriesResponse> GetSeriesAsync(WebShop webShop, CoreDataRequest.GetSeriesRequest request);
    }
}
