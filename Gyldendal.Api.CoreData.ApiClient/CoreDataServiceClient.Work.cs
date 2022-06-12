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
        private const string WorkSearchController = "v1/WorkSearch";

        private const string WorkSearchControllerV2 = "v2/WorkSearch";

        private const string WorkSearchControllerV3 = "v3/WorkSearch";

        private const string WorkController = "v1/Work";

        /// <summary>
        /// Getting work from Core Data Services
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public SearchResponse<Work> GetWorksSearchResponse(Contracts.Requests.WorkSearchRequest request)
        {
            return HttpClient.PostAsync<SearchResponse<Work>, Contracts.Requests.WorkSearchRequest>($"{WorkSearchController}/Search", request);
        }

        /// <summary>
        /// Getting work from Core Data Services
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public SearchResponse<Work> GetWorksSearchResponseV2(Contracts.Requests.WorkSearchRequestV2 request)
        {
            return HttpClient.PostAsync<SearchResponse<Work>, Contracts.Requests.WorkSearchRequestV2>($"{WorkSearchControllerV2}/Search", request);
        }

        /// <summary>
        /// Getting work from Core Data Services
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public SearchResponse<Work> GetWorksSearchResponseV3(Contracts.Requests.WorkSearchRequestV3 request)
        {
            return HttpClient.PostAsync<SearchResponse<Work>, Contracts.Requests.WorkSearchRequestV3>($"{WorkSearchControllerV3}/Search", request);
        }

        /// <summary>
        /// Gets a Work object for the given WebShop and Workid parameters.
        /// </summary>
        /// <param name="webShop"></param>
        /// <param name="workId"></param>
        /// <returns></returns>
        public Work GetWorkById(WebShop webShop, int workId)
        {
            return HttpClient.GetAsync<Work>($"{WorkController}/GetWorkById/{webShop}/{workId}");
        }

        /// <summary>
        /// Gets a Work object for the given WebShops and Workid parameters.
        /// </summary>
        /// <param name="webShops"></param>
        /// <param name="workId"></param>
        /// <returns></returns>
        public Work GetWorkById(WebShop[] webShops, int workId)
        {
            return HttpClient.PostAsync<Work, WebShop[]>($"{WorkController}/GetWorkById/{workId}", webShops);
        }

        /// <summary>
        /// Returns a Work object containing the single Product for the given productId, and the related Work data in Work attributes.
        /// </summary>
        /// <param name="websiteId"></param>
        /// <param name="productType"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public GetProductDetailsResponse GetProductAsWork(int websiteId, ProductType productType, string id)
        {
            return HttpClient.GetAsync<GetProductDetailsResponse>($"{WorkController}/GetProductDetails/{websiteId}/{productType}/{id}");
        }

        /// <summary>
        /// Returns list of work objects against scope and ISBN
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="isbn"></param>
        /// <returns></returns>
        public GetScopeWorksByProductIdResponse GetScopeWorksByProductId(DataScope dataScope, string isbn)
        {
            return HttpClient.GetAsync<GetScopeWorksByProductIdResponse>($"{WorkController}/GetScopeWorksByProductId/{dataScope}/{isbn}");
        }
    }
}