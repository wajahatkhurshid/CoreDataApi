using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Business.Util;
using Gyldendal.Api.CoreData.Common.Logging;
using Gyldendal.Api.CoreData.ContentfulProxy.Infrastructure;
using Gyldendal.Api.CoreData.EventProcessor;
using Gyldendal.Api.CoreData.EventProcessor.Contentful.EventHandlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Gyldendal.Api.CoreData.Tests.EventProcessing.Factory
{
    [TestClass]
    public class EventHandlerFactoryTests
    {
        private Mock<IContentfulManager> _contentfulManager;
        private Mock<IKoncernDataUtils> _koncerndataUtil;
        private Mock<ILogger> _logger;


        // Use TestInitialize to run code before running each test 
        [TestInitialize]
        public void MyTestInitialize()
        {
            _contentfulManager = new Mock<IContentfulManager>();
            _koncerndataUtil = new Mock<IKoncernDataUtils>();
            _logger = new Mock<ILogger>();
        }

        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public void CreateEventHandler_AssetUpdateHandler_EventHandlerCreated()
        {
            var eventFactory =
                new EventHandlerFactory(_contentfulManager.Object, _koncerndataUtil.Object, _logger.Object);

            var eventInfo = new EventInfo
            {
                EventName = "contentful_asset",
                EventPayload = "",
                Source = ""
            };

            var handler = eventFactory.CreateEventHandler(eventInfo);

            Assert.IsTrue((handler is AssetUpdateEventHandler),
                $"Update handler of type {typeof(AssetUpdateEventHandler)} was expected, but the type returned is {handler.GetType()}");
        }

        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public void CreateEventHandler_AuthorUpdateHandler_EventHandlerCreated()
        {
            var eventFactory =
                new EventHandlerFactory(_contentfulManager.Object, _koncerndataUtil.Object, _logger.Object);

            var eventInfo = new EventInfo
            {
                EventName = "contentful_author",
                EventPayload = "",
                Source = ""
            };

            var handler = eventFactory.CreateEventHandler(eventInfo);

            Assert.IsTrue((handler is AuthorUpdateEventHandler),
                $"Update handler of type {typeof(AuthorUpdateEventHandler)} was expected, but the type returned is {handler.GetType()}");
        }
    }
}
