using System.Threading.Tasks;
using System.Web.Http;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Common.EventInfrastructure;

namespace Gyldendal.Api.CoreData.Controllers
{
    /// <summary>
    /// Handles events from multiple sources.
    /// </summary>
    public class EventController : ApiController
    {
        private readonly IEventProcessor _eventProcessor;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventProcessor"></param>
        public EventController(IEventProcessor eventProcessor)
        {
            _eventProcessor = eventProcessor;
        }

        /// <summary>
        /// Handles data update event coming from 3rd party source
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v1/Event/ProcessEvent")]
        public async Task<IHttpActionResult> ProcessEvent(EventInfo eventInfo)
        {
            await _eventProcessor.ProcessAsync(eventInfo);

            return Ok();
        }
    }
}
