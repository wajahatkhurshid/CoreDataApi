using System.Threading.Tasks;
using Gyldendal.Api.CoreData.Business.Porter.Interfaces;
using Gyldendal.Api.CoreData.Business.Porter.Mapping;
using CoreDataModels = Gyldendal.Api.CoreData.Contracts.Models;
using PorterApi = Gyldendal.Api.CoreData.Services.PorterApiClient;
using CoreDataResponse = Gyldendal.Api.CoreData.Contracts.Response;
using GetSeriesRequest = Gyldendal.Api.CoreData.Contracts.Requests.GetSeriesRequest;
using WebShop = Gyldendal.Api.CommonContracts.WebShop;

namespace Gyldendal.Api.CoreData.Business.Porter.Services
{
    public class SeriesService : ISeriesService
    {
        PorterApi.IPorterClient _porterClient;
        public SeriesService(PorterApi.IPorterClient porterClient)
        {
            _porterClient = porterClient;
        }
        public async Task<CoreDataModels.Series> GetSeriesByIdAsync(WebShop webShop, int seriesId)
        {
            var response = await _porterClient.SystemseriesApiV1SystemseriesSeriesbyidAsync(new PorterApi.GetSeriesRequest{
                WebShop = webShop.ToPorterWebShop(), 
                SeriesId = seriesId
                });

            var series = response.Series.ToCoreDataSeries();

            return series;
        }
        public async Task<CoreDataResponse.GetSeriesResponse> GetSeriesAsync(WebShop webShop, GetSeriesRequest request)
        {
            var response = await _porterClient.SystemseriesApiV1SystemseriesSeriesAsync(new PorterApi.GetSeriesPaginatedRequest
            {
                WebShop = webShop.ToPorterWebShop(), 
                SeriesType = request.RequestType.ToPorterSeriesType(),
                Subject = request.Subject,
                Area = request.Area,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                SortBy = request.SortBy.ToPorterSortBy(),
                OrderBy = request.OrderBy.ToPorterSeriesOrderBy()
            });

            var seriesList = response.Series.ToCoreDataSeriesList();

            var paginatedSeriesResponse = new CoreDataResponse.GetSeriesResponse
            {
                Count = response.Count,
                PageIndex = response.PageIndex,
                PageSize = response.PageSize,
                Series = seriesList
            };

            return paginatedSeriesResponse;
        }
    }
}
