using System.Threading.Tasks;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Common.EventInfrastructure;
using Gyldendal.Common.WebUtils.Exceptions;
using Gyldendal.PulsenServices.Api.Contracts.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;

namespace Gyldendal.Api.CoreData.Tests.EventProcessing.Processor
{
    [TestClass]
    public class EventProcessorTest
    {
        private Mock<IEventHandlerFactory> _eventHandlerFactory;

        // Use TestInitialize to run code before running each test 
        [TestInitialize]
        public void TestInitialize()
        {
            _eventHandlerFactory = new Mock<IEventHandlerFactory>();

            _eventHandlerFactory.Setup(a => a.CreateEventHandler(It.IsAny<EventInfo>())).Returns(() => null);
        }

        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public async Task Process_UnSupportedEvent_ThrowsException()
        {
            var eventProcessor = new CoreData.EventProcessor.EventProcessor(_eventHandlerFactory.Object);

            var eventInfo = new EventInfo
            {
                Source = "Contentful",
                EventName = "contentful_assetXYZ",
                EventPayload = JsonConvert.SerializeObject(null)
            };

            try
            {
                await eventProcessor.ProcessAsync(eventInfo);
                Assert.Fail("ValidationException was expected against invalid event provided in the request but was not received.");
            }
            catch (ValidationException e)
            {
                Assert.AreEqual(e.ErrorCode, (ulong) ErrorCodes.InvalidValue);
            }
        }
    }
}
