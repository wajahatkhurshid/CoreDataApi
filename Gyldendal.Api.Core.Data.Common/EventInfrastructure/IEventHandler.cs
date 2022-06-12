using System.Threading.Tasks;
using Gyldendal.Api.CommonContracts;

namespace Gyldendal.Api.CoreData.Common.EventInfrastructure
{
    public interface IEventHandler
    {
        Task HandleAsync(EventInfo eventInfo);
    }
}
