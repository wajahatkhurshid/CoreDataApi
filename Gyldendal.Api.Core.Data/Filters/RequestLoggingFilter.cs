using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http.Controllers;
using Gyldendal.Api.CoreData.Common.Logging;
using Gyldendal.Api.CoreData.DependencyResolver;
using Newtonsoft.Json;
using ActionFilterAttribute = System.Web.Http.Filters.ActionFilterAttribute;

namespace Gyldendal.Api.CoreData.Filters
{
    /// <summary>
    /// 
    /// </summary>
    public class RequestLoggingFilter : ActionFilterAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            var request = actionContext.Request;
            var requestType = $"{request.Method}";
            var requestUri = request.RequestUri;
            var requestMessage = ExtractActionArguments(actionContext.ActionArguments);
       

            var controllerGdprSafeAttribute = actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<IsGdprSafeAttribute>().FirstOrDefault();
            var actionGdprSafeAttribute = actionContext.ActionDescriptor.GetCustomAttributes<IsGdprSafeAttribute>().FirstOrDefault();
            var gdprSafeAttribute = actionGdprSafeAttribute ?? controllerGdprSafeAttribute;
            var isGdprSafe = gdprSafeAttribute != null && gdprSafeAttribute.IsGdprSafe;
            var requestId = Guid.NewGuid().ToString();

            await LogRequest(requestType, $"{requestUri}", requestMessage, requestId, requestUri.AbsolutePath, isGdprSafe);
            if (!isGdprSafe)
            {
                // In case isGdprSafe = false, we are creating an extra entry with request message as empty, requestUri without query string parameter and isGdprSafe = true, to trace the calling method of the incoming request.
                await LogRequest(requestType, requestUri.AbsolutePath, string.Empty, requestId, absoluteUri: requestUri.AbsolutePath, isGdprSafe: true);
            }
        }

        private static Task LogRequest(string requestType, string requestUri, string message, string requestId, string absoluteUri, bool isGdprSafe)
        {
            var logger = Ioc.Container.GetInstance<ILogger>();
            var messageToLog = $"RequestId={requestId}{Environment.NewLine}RequestInfo={requestType} {requestUri}" +
                               $"{(string.IsNullOrEmpty(message) ? "" : $"{Environment.NewLine}Message={Environment.NewLine}{message}")}";

            HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
            {
                logger.SetCurrentThreadContext(absoluteUri.Trim('/'));
                logger.Info(messageToLog, isGdprSafe: isGdprSafe);
            });
            return Task.CompletedTask;
        }

        private string ExtractActionArguments(Dictionary<string, object> actionArguments)
        {
            var actionArgumentsList = new List<string>();
            foreach (var actionArgument in actionArguments)
            {
                var actionArgumentVal = JsonConvert.SerializeObject(actionArgument.Value);
                actionArgumentsList.Add($"{actionArgument.Key}:{actionArgumentVal}");
            }
            return string.Join(" - ", actionArgumentsList);
        }

    }
}