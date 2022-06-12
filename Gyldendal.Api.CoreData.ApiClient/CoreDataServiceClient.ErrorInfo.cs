using System.Collections.Generic;
using Gyldendal.Common.WebUtils.Models;

namespace Gyldendal.Api.CoreData.ApiClient
{
    public partial class CoreDataServiceClient
    {
        //ErrorController prefix
        private const string ErrorInfoController = "v1/ErrorInfo";

        /// <summary>
        /// Returns all Error Codes of CoreData and it's underlying systems
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ErrorDetail> GetAllErrorCodes()
        {
            var apiUrl = ErrorInfoController;
            return HttpClient.GetAsync<IEnumerable<ErrorDetail>>(apiUrl);
        }

        /// <summary>
        /// Get error details for the specified error code of CoreData
        /// </summary>
        /// <returns>Error code details</returns>
        public ErrorDetail GetErrorCodeDetails(ulong errorCode)
        {
            var apiUrl = ErrorInfoController + $"/{errorCode}";
            return HttpClient.GetAsync<ErrorDetail>(apiUrl);
        }
    }
}
