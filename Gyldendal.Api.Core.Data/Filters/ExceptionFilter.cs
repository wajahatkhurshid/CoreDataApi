using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using Gyldendal.Api.CoreData.Common.Exceptions;
using Gyldendal.Api.CoreData.Common.Logging;
using Gyldendal.Common.WebUtils.Exceptions;
using Gyldendal.Common.WebUtils.Models;
// ReSharper disable ExplicitCallerInfoArgument

namespace Gyldendal.Api.CoreData.Filters
{
    /// <summary>
    /// Class ExceptionFilter.
    /// </summary>
    public class ExceptionFilter : ExceptionFilterAttribute, IExceptionFilter
    {
        private readonly ILogger _logger;
        /// <summary>
        /// Constructor of work Controller
        /// </summary>
        /// <param name="logger"></param>
        public ExceptionFilter(ILogger logger)
        {
            _logger = logger;
        }
        /// <summary>
        /// Raises the exception event.
        /// </summary>
        /// <param name="actionExecutedContext">The context for the action.</param>
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            var exception = actionExecutedContext.Exception;

            _logger.Error(
                "Error encountered. " +
                _logger.GetPropertyNameAndValue(() => actionExecutedContext.Request.RequestUri) +
                ", HTTPContent: " +
                GetBodyFromRequest(actionExecutedContext),
                exception, isGdprSafe: true);

            if (exception is NotFoundException)
            {
                var notFoundException = (NotFoundException)exception;
                actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.NotFound, notFoundException.Message);
            }
            else if (exception is ValidationException)
            {
                var modelValidationException = (ValidationException)exception;
                actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(
                    HttpStatusCode.BadRequest,
                    new ValidationErrorDetail
                    {
                        Code = modelValidationException.ErrorCode,
                        Description = modelValidationException.Description,
                        OriginatingSystem = modelValidationException.OriginatingSystem,
                        ObjectValidationDetail = modelValidationException.ObjectValidations
                    });
            }
            else if (exception is ProcessException)
            {
                var processException = (ProcessException)exception;
                actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(
                    HttpStatusCode.BadRequest,
                    new ErrorDetail
                    {
                        Code = processException.ErrorCode,
                        Description = processException.Description,
                        OriginatingSystem = processException.OriginatingSystem
                    });
            }
            else
            {
                actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.InternalServerError, exception.Message);
            }
        }

        /// <summary>
        /// Method to Read all request content from context. In the process it sets stream position to zero to read contents.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string GetBodyFromRequest(HttpActionExecutedContext context)
        {
            string data;
            using (var stream = context.Request.Content.ReadAsStreamAsync().Result)
            {
                if (stream.CanSeek)
                {
                    stream.Position = 0; // As stream is read, it is required to set its position to 0
                }
                data = context.Request.Content.ReadAsStringAsync().Result;
            }
            return data;
        }

    }
}