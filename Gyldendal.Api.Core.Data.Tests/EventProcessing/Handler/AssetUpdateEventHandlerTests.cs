using System;
using System.Threading.Tasks;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Business.Util;
using Gyldendal.Api.CoreData.Common.Logging;
using Gyldendal.Api.CoreData.ContentfulProxy.Infrastructure;
using Gyldendal.Api.CoreData.ContentfulProxy.Model;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.EventProcessor.Contentful.EventHandlers;
using Gyldendal.Api.CoreData.EventProcessor.Contentful.Payloads;
using Gyldendal.Common.WebUtils.Exceptions;
using Gyldendal.PulsenServices.Api.Contracts.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
// ReSharper disable ExplicitCallerInfoArgument

namespace Gyldendal.Api.CoreData.Tests.EventProcessing.Handler
{
    [TestClass]
    public class AssetUpdateEventHandlerTests
    {
        private Mock<IContentfulManager> _contentfulManager;
        private Mock<IKoncernDataUtils> _koncerndataUtil;
        private Mock<ILogger> _logger;

        // Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext) { }

        // Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup]
        public static void MyClassCleanup() { }

        // Use TestInitialize to run code before running each test 
        [TestInitialize]
        public void MyTestInitialize()
        {
            _contentfulManager = new Mock<IContentfulManager>();
            _koncerndataUtil = new Mock<IKoncernDataUtils>();
            _logger = new Mock<ILogger>();


            _contentfulManager.Setup(a => a.GetAuthorByAssetIdAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(GetAuthor("F1234")));

            _koncerndataUtil.Setup(a =>
                a.RegisterContributorChangeFromThirdPartyAsync(It.IsAny<DataScope>(), It.IsAny<string>(),
                    It.IsAny<string>()));

            _logger.Setup(c => c.Info(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool>()));

            _logger.Setup(a => a.Error(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()));
        }

        // Use TestCleanup to run code after each test has run
        [TestCleanup]
        public void MyTestCleanup() { }



        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public async Task Handle_WithNullPayloadInput_ThrowsException()
        {
            var assetHandler =
                new AssetUpdateEventHandler(_contentfulManager.Object, _koncerndataUtil.Object, _logger.Object);

            var eventInfo = new EventInfo
            {
                Source = "Contentful",
                EventName = "contentful_asset",
                EventPayload = JsonConvert.SerializeObject(null)
            };

            try
            {
                await assetHandler.HandleAsync(eventInfo);
                Assert.Fail("ValidationException was expected against null event payload value provided in the request, but exception was not received.");
            }
            catch (ValidationException e)
            {
                Assert.AreEqual(e.ErrorCode, (ulong)ErrorCodes.InvalidModelJson, "Invalid model json error code was expected, but not received.");
            }
        }

        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public async Task Handle_WithInvalidAssetId_ThrowsException(string inputAssetId)
        {
            var assetHandler =
                new AssetUpdateEventHandler(_contentfulManager.Object, _koncerndataUtil.Object, _logger.Object);

            var payload = new AssetUpdatePayload
            {
                DataScope = DataScope.GyldendalDkShop,
                AssetId = inputAssetId
            };
            var eventInfo = new EventInfo
            {
                Source = "Contentful",
                EventName = "contentful_asset",
                EventPayload = JsonConvert.SerializeObject(payload)
            };

            try
            {
                await assetHandler.HandleAsync(eventInfo);
                Assert.Fail("ValidationException was expected against invalid asset id provided in the request, but was not received.");
            }
            catch (ValidationException e)
            {
                Assert.AreEqual(e.ErrorCode, (ulong)ErrorCodes.InvalidModelJson, "Invalid model json error code was expected, but not received.");
            }
        }

        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public async Task Handle_AllValidInputs_EventRegistered()
        {
            var assetHandler =
                new AssetUpdateEventHandler(_contentfulManager.Object, _koncerndataUtil.Object, _logger.Object);

            var payload = new AssetUpdatePayload
            {
                DataScope = DataScope.GyldendalDkShop,
                AssetId = "F1234"
            };
            var eventInfo = new EventInfo
            {
                Source = "Contentful",
                EventName = "contentful_asset",
                EventPayload = JsonConvert.SerializeObject(payload)
            };

            await assetHandler.HandleAsync(eventInfo);

            _koncerndataUtil.Verify(a =>
                a.RegisterContributorChangeFromThirdPartyAsync(It.IsAny<DataScope>(), It.IsAny<string>(),
                    It.IsAny<string>()), "Event couldn't not be registered for some reason.");
        }

        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public async Task Handle_InvalidContributorId_LogsWarning()
        {
            var source = "Contentful";

            _contentfulManager.Setup(a => a.GetAuthorByAssetIdAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(GetAuthor("")));

            _logger.Setup(a => a.Warning(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool>()));

            var assetHandler =
                new AssetUpdateEventHandler(_contentfulManager.Object, _koncerndataUtil.Object, _logger.Object);

            var payload = new AssetUpdatePayload
            {
                DataScope = DataScope.GyldendalDkShop,
                AssetId = "F1234"
            };
            var eventInfo = new EventInfo
            {
                Source = source,
                EventName = "contentful_asset",
                EventPayload = JsonConvert.SerializeObject(payload)
            };

            try
            {
                await assetHandler.HandleAsync(eventInfo);

                _logger.Verify(a => a.Warning(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool>()), "Logger failed to log warning.");
            }
            catch (Exception)
            {
                Assert.Fail("Logger was expected to log a warning, but instead, an exception was thrown by the code.");
            }
        }

        private Author GetAuthor(string authorId)
        {
            if (authorId == null) return null;

            return new Author
            {
                Id = authorId
            };
        }
    }
}
