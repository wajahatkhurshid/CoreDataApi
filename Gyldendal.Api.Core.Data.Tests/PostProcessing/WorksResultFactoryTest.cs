using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.ResultsPostProcessing;
using Gyldendal.Api.CoreData.ResultsPostProcessing.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GPlusWorkResultsProcessor = Gyldendal.Api.CoreData.ResultsPostProcessing.Processors.GPlus.WorkResultsProcessor;
using TradeGdkWorkResultsProcessor = Gyldendal.Api.CoreData.ResultsPostProcessing.Processors.TradeGdk.WorkResultsProcessor;

namespace Gyldendal.Api.CoreData.Tests.PostProcessing
{
    [TestClass]
    public class WorksResultFactoryTest
    {
        private readonly IWorksResultFactory _worksResultFactory;

        public WorksResultFactoryTest()
        {
            _worksResultFactory = new WorksResultFactory();
        }

        [TestMethod]
        public void GetProcessors_BogklubWebShop_ShouldReturnSingleWorksResultProcessor()
        {
            // Act
            var results = _worksResultFactory.GetProcessors(WebShop.ClubBogklub);

            // Assert
            Assert.AreEqual(1, results.Count, "Wrong no. of processors returned.");
        }

        [TestMethod]
        public void GetProcessors_GyldendalDkWebShop_ShouldReturnSingleWorksResultProcessor()
        {
            // Act
            var results = _worksResultFactory.GetProcessors(WebShop.GyldendalDk);

            // Assert
            Assert.AreEqual(2, results.Count, "Wrong no. of processors returned.");
        }

        [TestMethod]
        public void GetProcessors_BogklubWebShop_ShouldReturnProcessorOfTypeWorksResultProcessor()
        {
            // Act
            var results = _worksResultFactory.GetProcessors(WebShop.ClubBogklub);

            // Assert
            Assert.AreEqual(typeof(GPlusWorkResultsProcessor), results[0].GetType(), "Wrong processor returned.");
        }

        [TestMethod]
        public void GetProcessors_GyldendalDkWebShop_ShouldReturnProcessorOfTypeWorksResultProcessor()
        {
            // Act
            var results = _worksResultFactory.GetProcessors(WebShop.GyldendalDk);

            // Assert
            Assert.AreEqual(typeof(GPlusWorkResultsProcessor), results[0].GetType(), "Wrong processor returned.");
            Assert.AreEqual(typeof(TradeGdkWorkResultsProcessor), results[1].GetType(), "Wrong processor returned.");
        }

        [TestMethod]
        public void GetProcessors_ShopOtherThanGyldendalPlusAndGyldendalDk_ShouldNotReturnProcessor()
        {
            // Act
            var results = _worksResultFactory.GetProcessors(WebShop.MunksGaard);

            // Assert
            Assert.AreEqual(0, results.Count, "There should not be any processor against webshop other than gyldendal plus webshops.");
        }
    }
}