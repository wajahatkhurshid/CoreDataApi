using System.Net.Http;
using Contentful.Core;
using Contentful.Core.Configuration;

namespace Gyldendal.Api.CoreData.ContentfulProxy
{
    public abstract class BaseApiClient
    {
        private readonly string _spaceId;

        private readonly string _environment;

        private readonly string _previewApiKey;

        private readonly string _deliveryApiKey;

        private ContentfulClient _httpClientUtility;

        protected ContentfulClient ContentfulClient =>
            _httpClientUtility ?? (_httpClientUtility = CreateInstance(_spaceId, _environment, _previewApiKey, _deliveryApiKey));

        protected BaseApiClient(string spaceId, string environment, string previewApiKey, string deliveryApiKey)
        {
            _spaceId = spaceId;
            _environment = environment;
            _previewApiKey = previewApiKey;
            _deliveryApiKey = deliveryApiKey;
        }

        private static ContentfulClient CreateInstance(string spaceId, string environment, string previewApiKey, string deliveryApiKey)
        {
            var httpClient = new HttpClient();
            var options = new ContentfulOptions
            {
                SpaceId = spaceId,
                Environment = environment,
                PreviewApiKey = previewApiKey,
                DeliveryApiKey = deliveryApiKey
            };

            var contentfulClient = new ContentfulClient(httpClient, options);

            return contentfulClient;
        }

        //private T ReadResponse<T>(HttpResponseMessage response)
        //{
        //    if (response.IsSuccessStatusCode)
        //    {
        //        return response.Content.ReadAsAsync<T>().Result;
        //    }

        //    throw this.ReadResponseError(response);
        //}

        //private Exception ReadResponseError(HttpResponseMessage response)
        //{
        //    if (response.StatusCode == HttpStatusCode.Unauthorized)
        //    {
        //        return new UnauthorizedAccessException();
        //    }

        //    return new Exception(response.Content.ReadAsStringAsync().Result);
        //}

        //protected async Task<T> GetAsync<T>(string address) where T : class
        //{
        //    var result = await HttpClient.GetAsync(address);
        //    var response = ReadResponse<T>(result);
        //    return response;
        //}
    }
}
