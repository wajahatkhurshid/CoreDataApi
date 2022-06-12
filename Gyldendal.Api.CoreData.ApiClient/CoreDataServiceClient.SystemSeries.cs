using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Requests;
using Gyldendal.Api.CoreData.Contracts.Response;

namespace Gyldendal.Api.CoreData.ApiClient
{
    public partial class CoreDataServiceClient
    {
        private const string SystemSeriesController = "v1/SystemSeries";

        /// <summary>
        /// Calls Core Data Service to get Series according to website
        /// </summary>
        /// <param name="webSite"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public GetSeriesResponse GetSeries(WebShop webSite, GetSeriesRequest request)
        {
            var queryString = $"{SystemSeriesController}/Series/{webSite}";
            return HttpClient.PostAsync<GetSeriesResponse, GetSeriesRequest>(queryString, request);
        }

        /// <summary>
        /// Returns the Serie object against the given id and WebShop.
        /// </summary>
        /// <param name="webSite"></param>
        /// <param name="serieId"></param>
        /// <returns></returns>
        public Series GetSerieById(WebShop webSite, int serieId)
        {
            return HttpClient.GetAsync<Series>($"{SystemSeriesController}/SerieById/{webSite}/{serieId}");
        }
    }
}
