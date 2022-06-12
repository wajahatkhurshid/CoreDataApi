using System.Threading.Tasks;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Common;
using Gyldendal.Api.CoreData.Common.EventInfrastructure;
using Gyldendal.Common.WebUtils.Exceptions;
using ErrorCodes = Gyldendal.PulsenServices.Api.Contracts.Common.ErrorCodes;

namespace Gyldendal.Api.CoreData.EventProcessor
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IEventHandlerFactory _eventHandlerFactory;

        public EventProcessor(IEventHandlerFactory eventHandlerFactory)
        {
            _eventHandlerFactory = eventHandlerFactory;
        }

        public async Task ProcessAsync(EventInfo eventInfo)
        {
            var eventHandler = _eventHandlerFactory.CreateEventHandler(eventInfo);

            if (eventHandler == null)
                throw new ValidationException((ulong) ErrorCodes.InvalidValue,
                    $"EventProcessor: Invalid value ({eventInfo.EventName}) received for event name. Event not supported.",
                    Extensions.CoreDataSystemName, null);


            await eventHandler.HandleAsync(eventInfo);
        }
    }
}
