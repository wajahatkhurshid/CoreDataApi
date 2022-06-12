using System.Threading.Tasks;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Business.Util;
using Gyldendal.Api.CoreData.Common;
using Gyldendal.Api.CoreData.Common.EventInfrastructure;
using Gyldendal.Api.CoreData.Common.Logging;
using Gyldendal.Api.CoreData.ContentfulProxy.Infrastructure;
using Gyldendal.Api.CoreData.EventProcessor.Contentful.Payloads;
using Gyldendal.Common.WebUtils.Exceptions;
using ErrorCodes = Gyldendal.PulsenServices.Api.Contracts.Common.ErrorCodes;

namespace Gyldendal.Api.CoreData.EventProcessor.Contentful.EventHandlers
{
    public class AssetUpdateEventHandler : BaseContentfulEventHandler<AssetUpdatePayload>, IEventHandler
    {
        private readonly IContentfulManager _contentfulManager;
        private readonly ILogger _logger;
        
        public AssetUpdateEventHandler(IContentfulManager contentfulManager, IKoncernDataUtils koncernDataUtils, ILogger logger)
            : base(koncernDataUtils, logger)
        {
            _contentfulManager = contentfulManager;
            _logger = logger;
        }

        public async Task HandleAsync(EventInfo eventInfo)
        {
            LogEventInfo(eventInfo);

            EventPayload = Newtonsoft.Json.JsonConvert.DeserializeObject<AssetUpdatePayload>(eventInfo.EventPayload);

            ValidatePayload();

            var contributorId = await GetContributorIdByAssetIdAsync(EventPayload.AssetId);

            if (string.IsNullOrWhiteSpace(contributorId))
            {
                _logger.Warning($"EventProcessor: Contributor id received as null. Skipping registration of author data update event. Possible cause is publishing of an image which is not associated to any author in Contentful.", isGdprSafe: true);
                return;
            }
            
            await RegisterContributorChangeFromThirdPartyAsync(EventPayload.DataScope, eventInfo.Source, contributorId);
        }

        private void ValidatePayload()
        {
            if (EventPayload == null || string.IsNullOrWhiteSpace(EventPayload.AssetId))
                throw new ValidationException((ulong)ErrorCodes.InvalidModelJson,
                    $"EventProcessor: Null values received in {nameof(AssetUpdateEventHandler)}",
                    Extensions.CoreDataSystemName, null);
        }


        private async Task<string> GetContributorIdByAssetIdAsync(string assetId)
        {
            var author = await _contentfulManager.GetAuthorByAssetIdAsync(assetId);

            return author?.Id;
        }
    }
}
