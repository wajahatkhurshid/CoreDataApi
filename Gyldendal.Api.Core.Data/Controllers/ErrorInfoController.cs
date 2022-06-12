using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using Gyldendal.Api.CoreData.Business.Util;
using Gyldendal.Api.CoreData.Common.Logging;
using Gyldendal.Api.CoreData.Filters;
using Gyldendal.Common.WebUtils.Models;

namespace Gyldendal.Api.CoreData.Controllers
{
    /// <summary>
    /// Returning Error Information
    /// </summary>
    [IsGdprSafe(true)]
    public class ErrorInfoController : ApiController
    {
        private readonly ILogger _logger;
        private readonly IErrorCodeUtil _errorCodeUtil;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="errorCodeUtil"></param>
        public ErrorInfoController(ILogger logger, IErrorCodeUtil errorCodeUtil)
        {
            _logger = logger;
            _errorCodeUtil = errorCodeUtil;
        }

        /// <summary>
        /// Returns all Error Codes of CoreData and it's underlying systems
        /// </summary>
        /// <returns></returns>
        [Route("api/v1/ErrorInfo")]
        [ResponseType(typeof (IEnumerable<ErrorDetail>))]
        [HttpGet]
        public IHttpActionResult GetAllErrorCodes()
        {
            return Ok(_errorCodeUtil.GetAllErrorCodes());
        }

        /// <summary>
        /// Get error details for the specified error code of CoreData
        /// </summary>
        /// <returns>Error code details</returns>
        [Route("api/v1/ErrorInfo/{errorCode}")]
        [ResponseType(typeof(ErrorDetail))]
        [HttpGet]
        public IHttpActionResult GetErrorCodeDetails(ulong errorCode)
        {
            return Ok(_errorCodeUtil.GetErrorDetail(errorCode));
        }
    }
}
