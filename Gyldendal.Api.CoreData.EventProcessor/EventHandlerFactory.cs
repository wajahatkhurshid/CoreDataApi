using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Business.Util;
using Gyldendal.Api.CoreData.Common.EventInfrastructure;
using Gyldendal.Api.CoreData.Common.Logging;
using Gyldendal.Api.CoreData.ContentfulProxy.Infrastructure;
using Gyldendal.Api.CoreData.EventProcessor.Contentful.EventHandlers;

namespace Gyldendal.Api.CoreData.EventProcessor
{
    public class EventHandlerFactory : IEventHandlerFactory
    {
        private readonly IContentfulManager _contentfulManager;
        private readonly IKoncernDataUtils _koncernDataUtils;
        private readonly ILogger _logger;

        public EventHandlerFactory(IContentfulManager contentfulManager, IKoncernDataUtils koncernDataUtils, ILogger logger)
        {
            _contentfulManager = contentfulManager;
            _koncernDataUtils = koncernDataUtils;
            _logger = logger;
        }

        public IEventHandler CreateEventHandler(EventInfo eventInfo)
        {
            switch (eventInfo.EventName.ToLower())
            {
                case "contentful_asset":
                    return new AssetUpdateEventHandler(_contentfulManager, _koncernDataUtils, _logger);

                case "contentful_author":
                    return new AuthorUpdateEventHandler(_koncernDataUtils, _logger);

                default:
                    return null;
            }
        }
    }
}
