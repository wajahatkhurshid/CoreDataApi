using Gyldendal.Api.CommonContracts;

namespace Gyldendal.Api.CoreData.Common.EventInfrastructure
{
    public interface IEventHandlerFactory
    { 
        IEventHandler CreateEventHandler(EventInfo eventInfo);
    }
}
