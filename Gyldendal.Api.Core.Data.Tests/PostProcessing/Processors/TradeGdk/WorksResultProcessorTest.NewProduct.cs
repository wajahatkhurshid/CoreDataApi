using Gyldendal.Api.CommonContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Gyldendal.Api.CoreData.Tests.PostProcessing.Processors.TradeGdk
{
    [TestClass]
    public partial class WorksResultProcessorTest
    {
        [TestMethod]
        public void Process_DuplicateGuProductWorks_DuplicateProductShouldBeRemoved()
        {
            // Arrange
            DuplicateProductPopulateWorks();
            // Act
            _workResultsProcessor.Process(_works);
            var works = GetWorks();
            // Assert
            Assert.IsTrue(works.All(c => c.Products.All(v => v.WebShop == WebShop.TradeGyldendalDk)));
        }

        [TestMethod]
        public void Process_NoDuplicateGuProductWorks_ResultShouldRemainUntouched()
        {
            // Arrange
            NoDuplicateProductPopulateWorks();
            // Act
            _workResultsProcessor.Process(_works);
            var works = GetWorks();
            // Assert
            Assert.IsTrue(!works.All(c => c.Products.All(v => v.WebShop == WebShop.TradeGyldendalDk)));
        }
    }
}