using System.Threading.Tasks;
using Gyldendal.Api.CommonContracts;

namespace Gyldendal.Api.CoreData.Common.EventInfrastructure
{
    public interface IEventProcessor
    {
        Task ProcessAsync(EventInfo eventInfo);
    }
}
