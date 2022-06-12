using System.Threading.Tasks;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Business.Util;
using Gyldendal.Api.CoreData.Common;
using Gyldendal.Api.CoreData.Common.EventInfrastructure;
using Gyldendal.Api.CoreData.Common.Logging;
using Gyldendal.Api.CoreData.EventProcessor.Contentful.Payloads;
using Gyldendal.Common.WebUtils.Exceptions;
using ErrorCodes = Gyldendal.PulsenServices.Api.Contracts.Common.ErrorCodes;

namespace Gyldendal.Api.CoreData.EventProcessor.Contentful.EventHandlers
{
    public class AuthorUpdateEventHandler : BaseContentfulEventHandler<AuthorUpdatePayload>, IEventHandler
    {
        public AuthorUpdateEventHandler(IKoncernDataUtils koncernDataUtils, ILogger logger) : base(koncernDataUtils, logger)
        {
        }

        public async Task HandleAsync(EventInfo eventInfo)
        {
            LogEventInfo(eventInfo);

            EventPayload = Newtonsoft.Json.JsonConvert.DeserializeObject<AuthorUpdatePayload>(eventInfo.EventPayload);

            ValidatePayload();

            await RegisterContributorChangeFromThirdPartyAsync(EventPayload.DataScope, eventInfo.Source, EventPayload.ContributorId);
        }

        private void ValidatePayload()
        {
            if (EventPayload == null || string.IsNullOrWhiteSpace(EventPayload.ContributorId))
                throw new ValidationException((ulong)ErrorCodes.InvalidModelJson,
                    $"EventProcessor: Null values received in {nameof(AuthorUpdateEventHandler)}",
                    Extensions.CoreDataSystemName, null);
        }
    }
}
