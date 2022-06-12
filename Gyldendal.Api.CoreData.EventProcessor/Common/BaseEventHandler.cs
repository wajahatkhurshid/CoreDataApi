using Gyldendal.Api.CommonContracts;
using ILogger = Gyldendal.Api.CoreData.Common.Logging.ILogger;

namespace Gyldendal.Api.CoreData.EventProcessor.Common
{
    public class BaseEventHandler<T>
    {
        private readonly ILogger _logger;

        protected T EventPayload { get; set; }

        protected BaseEventHandler(ILogger logger)
        {
            _logger = logger;
        }

        protected void LogEventInfo(EventInfo eventInfo)
        {
            _logger.Info(
                $"Event Received: {eventInfo.EventName} \n Event Source: {eventInfo.Source} \n EventPayload: {eventInfo.EventPayload}", isGdprSafe:true);
        }
    }
}
