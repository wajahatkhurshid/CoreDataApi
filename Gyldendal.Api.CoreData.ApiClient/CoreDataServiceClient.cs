using System.Collections.Generic;
using Gyldendal.Common.WebUtils.HttpClient;

namespace Gyldendal.Api.CoreData.ApiClient
{
    /// <summary>
    /// CoreDataService agent
    /// </summary>
    public partial class CoreDataServiceClient : ICoreDataServiceClient
    {
        private static string _baseUrl;
        private HttpClientUtility _httpClientUtility;
        private HttpClientUtility HttpClient => _httpClientUtility ?? (_httpClientUtility = new HttpClientUtility(_baseUrl, Header));

        private static Dictionary<string, string> Header { get; set; }

        /// <summary>
        /// Creates a new instance of CoreDataServiceClient using the passed in base Api Url, and the request headers.
        /// </summary>
        public CoreDataServiceClient(string baseUrl, Dictionary<string, string> headers = null)
        {
            _baseUrl = baseUrl;
            Header = headers;
        }
    }
}
