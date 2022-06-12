using Gyldendal.Api.CommonContracts;

namespace Gyldendal.Api.CoreData.ApiClient
{
    public partial class CoreDataServiceClient
    {
        private const string EventControllerV1 = "v1/Event";

        public void ProcessEvent(EventInfo eventInfo)
        {
            var apiUrl = $"{EventControllerV1}/ProcessEvent";

            HttpClient.PostAsync(apiUrl, eventInfo);
        }
    }
}
