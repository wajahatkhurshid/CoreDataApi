using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gyldendal.Api.CoreData.Tests.PostProcessing.Processors.GPlus
{
    [TestClass]
    public partial class WorksResultProcessorTest
    {
        //+----+----------------------------------------------------------------------+----------+---------+-------------+-------------------+----------------+
        //| #  | Test                                                                 | Physical | Imprint | Published   | Existing HY Label | Expected Label |
        //+----+----------------------------------------------------------------------+----------+---------+-------------+-------------------+----------------+
        //| 1  | Process_PhysicalProductPublished15DaysAgo_ShouldHaveNyhead           |    Yes   |    -    | 15 Days Ago |         -         |      Nyhed     |
        //+----+----------------------------------------------------------------------+----------+---------+-------------+-------------------+----------------+
        //| 2  | Process_PhysicalProductPublishedToday_ShouldHaveNyhead               |    Yes   |    -    |    Today    |         -         |      Nyhed     |
        //+----+----------------------------------------------------------------------+----------+---------+-------------+-------------------+----------------+
        //| 3  | Process_PhysicalProductPublished30DaysAgo_ShouldHaveNyhead           |    Yes   |    -    | 30 Days Ago |         -         |      Nyhed     |
        //+----+----------------------------------------------------------------------+----------+---------+-------------+-------------------+----------------+
        //| 4  | Process_PhysicalProductPublished31DaysAgo_NoNyhead                   |    Yes   |    -    | 31 Days Ago |         -         |        -       |
        //+----+----------------------------------------------------------------------+----------+---------+-------------+-------------------+----------------+
        //| 5  | Process_PhysicalProductWillPublishTomorrow_NoNyhead                  |    Yes   |    -    |  Tomorrow   |         -         |        -       |
        //+----+----------------------------------------------------------------------+----------+---------+-------------+-------------------+----------------+
        //| 6  | Process_NonPhysicalProductPublished15DaysAgo_NoNyhead                |    No    |    -    | 15 Days Ago |         -         |        -       |
        //+----+----------------------------------------------------------------------+----------+---------+-------------+-------------------+----------------+
        //| 7  | Process_NonPhysicalImprintProductPublished15DaysAgo_ShouldHaveNyhead |    No    | Stereo  | 15 Days Ago |         -         |      Nyhed     |
        //+----+----------------------------------------------------------------------+----------+---------+-------------+-------------------+----------------+
        //| 8  | Process_PhysicalProductAlreadyNyhed_NoDuplicateNyheads               |    Yes   |    -    | 15 Days Ago |       Nyhed       |        -       |
        //+----+----------------------------------------------------------------------+----------+---------+-------------+-------------------+----------------+
        //| 9  | Process_NonPhysicalProductAlreadyNyhed_NoDuplicateNyheads            |    No    |    -    | 15 Days Ago |       Nyhed       |        -       |
        //+----+----------------------------------------------------------------------+----------+---------+-------------+-------------------+----------------+
        //| 10 | Process_NonPhysicalImprintProductAlreadyNyhed_NoDuplicateNyheads     |    No    | Stereo  | 15 Days Ago |       Nyhed       |        -       |
        //+----+----------------------------------------------------------------------+----------+---------+-------------+-------------------+----------------+

        [TestMethod]
        public void Process_PhysicalProductPublished15DaysAgo_ShouldHaveNyhead()
        {
            // Arrange
            PopulateWorks("00001", DateTime.Now.AddDays(-15), physicalProduct: true);

            // Act
            _workResultsProcessor.Process(_works);
            var product = GetProductById("00001");

            // Assert
            Assert.IsTrue(product.Labels.Contains(NyheadLabel), "Product published 15 days ago should have label Nyhed.");
        }

        [TestMethod]
        public void Process_PhysicalProductPublishedToday_ShouldHaveNyhead()
        {
            // Arrange
            PopulateWorks("00001", DateTime.Now, physicalProduct: true);

            // Act
            _workResultsProcessor.Process(_works);
            var product = GetProductById("00001");

            // Assert
            Assert.IsTrue(product.Labels.Contains(NyheadLabel), "Product published today should have label Nyhed.");
        }

        [TestMethod]
        public void Process_PhysicalProductPublished30DaysAgo_ShouldHaveNyhead()
        {
            // Arrange
            PopulateWorks("00001", DateTime.Now.AddDays(-30), physicalProduct: true);

            // Act
            _workResultsProcessor.Process(_works);
            var product = GetProductById("00001");

            // Assert
            Assert.IsTrue(product.Labels.Contains(NyheadLabel), "Product published 30 days ago should have label Nyhed.");
        }

        [TestMethod]
        public void Process_PhysicalProductPublished31DaysAgo_NoNyhead()
        {
            // Arrange
            PopulateWorks("00001", DateTime.Now.AddDays(-31), physicalProduct: true);

            // Act
            _workResultsProcessor.Process(_works);
            var product = GetProductById("00001");

            // Assert
            Assert.IsFalse(product.Labels.Contains(NyheadLabel), "Product published 31 days ago should not have Nyhed label.");
        }

        [TestMethod]
        public void Process_PhysicalProductWillPublishTomorrow_NoNyhead()
        {
            // Arrange
            PopulateWorks("00001", DateTime.Now.AddDays(1), physicalProduct: true);

            // Act
            _workResultsProcessor.Process(_works);
            var product = GetProductById("00001");

            // Assert
            Assert.IsFalse(product.Labels.Contains(NyheadLabel), "Product publishing tomorrow should not have Nyhed label.");
        }

        [TestMethod]
        public void Process_NonPhysicalProductPublished15DaysAgo_NoNyhead()
        {
            // Arrange
            PopulateWorks("00001", DateTime.Now.AddDays(-15), physicalProduct: false);

            // Act
            _workResultsProcessor.Process(_works);
            var product = GetProductById("00001");

            // Assert
            Assert.IsFalse(product.Labels.Contains(NyheadLabel), "Non physical product published 15 days ago should not have Nyhed label.");
        }

        [TestMethod]
        public void Process_NonPhysicalImprintProductPublished15DaysAgo_ShouldHaveNyhead()
        {
            // Arrange
            PopulateWorks("00001", DateTime.Now.AddDays(-15), physicalProduct: false, isStereoImprint: true);

            // Act
            _workResultsProcessor.Process(_works);
            var product = GetProductById("00001");

            // Assert
            Assert.IsTrue(product.Labels.Contains(NyheadLabel), "Non physical product having Gyldendal Stereo published 15 days ago should have Nyhed label.");
        }

        [TestMethod]
        public void Process_PhysicalProductAlreadyNyhed_NoDuplicateNyheads()
        {
            // Arrange
            PopulateWorks("00001", DateTime.Now.AddDays(-15), physicalProduct: true, isStereoImprint: false, new List<string> { "Nyhed" });

            // Act
            _workResultsProcessor.Process(_works);
            var product = GetProductById("00001");

            // Assert
            Assert.AreEqual(1, product.Labels.Count, "Already set Nyhed product should not contain duplicate labels.");
        }

        [TestMethod]
        public void Process_NonPhysicalProductAlreadyNyhed_NoDuplicateNyheads()
        {
            // Arrange
            PopulateWorks("00001", DateTime.Now.AddDays(-15), physicalProduct: false, isStereoImprint: false, new List<string> { "Nyhed" });

            // Act
            _workResultsProcessor.Process(_works);
            var product = GetProductById("00001");

            // Assert
            Assert.AreEqual(1, product.Labels.Count, "Non physical Nyhed product should not have duplicate Nyhed labels.");
        }

        [TestMethod]
        public void Process_NonPhysicalImprintProductAlreadyNyhed_NoDuplicateNyheads()
        {
            // Arrange
            PopulateWorks("00001", DateTime.Now.AddDays(-15), physicalProduct: false, isStereoImprint: true, new List<string> { "Nyhed" });

            // Act
            _workResultsProcessor.Process(_works);
            var product = GetProductById("00001");

            // Assert
            Assert.AreEqual(1, product.Labels.Count, "Non physical Nyhed product having Gyldendal Stereo should not have duplicate Nyhed labels.");
        }
    }
}