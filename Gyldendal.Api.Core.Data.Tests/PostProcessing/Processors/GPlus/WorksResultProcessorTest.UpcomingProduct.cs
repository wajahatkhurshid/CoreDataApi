using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gyldendal.Api.CoreData.Tests.PostProcessing.Processors.GPlus
{
    public partial class WorksResultProcessorTest
    {
        //+---+------------------------------------------------------------------------------+----------+---------+---------------+-------------------+----------------+
        //| # | Test                                                                         | Physical | Imprint |   Publishing  | Existing HY Label | Expected Label |
        //+---+------------------------------------------------------------------------------+----------+---------+---------------+-------------------+----------------+
        //| 1 | Process_PhysicalProductPublishingAfter15Days_ShouldHaveKommerSnart           |    Yes   |    -    | After 15 Days |         -         |  Kommer snart  |
        //+---+------------------------------------------------------------------------------+----------+---------+---------------+-------------------+----------------+
        //| 2 | Process_PhysicalProductPublishedToday_NoKommerSnart                          |    Yes   |    -    |     Today     |         -         |        -       |
        //+---+------------------------------------------------------------------------------+----------+---------+---------------+-------------------+----------------+
        //| 3 | Process_PhysicalProductPublishingAfter30Days_ShouldHaveKommerSnart           |    Yes   |    -    | After 30 Days |         -         |  Kommer snart  |
        //+---+------------------------------------------------------------------------------+----------+---------+---------------+-------------------+----------------+
        //| 4 | Process_PhysicalProductPublishingAfter31Days_NoKommerSnart                   |    Yes   |    -    | After 31 Days |         -         |        -       |
        //+---+------------------------------------------------------------------------------+----------+---------+---------------+-------------------+----------------+
        //| 5 | Process_PhysicalProductPublishedYesterday_NoKommerSnart                      |    Yes   |    -    |   Yesterday   |         -         |        -       |
        //+---+------------------------------------------------------------------------------+----------+---------+---------------+-------------------+----------------+
        //| 6 | Process_NonPhysicalProductPublishingAfter15Days_NoKommerSnart                |    No    |    -    | After 15 Days |         -         |        -       |
        //+---+------------------------------------------------------------------------------+----------+---------+---------------+-------------------+----------------+
        //| 7 | Process_NonPhysicalImprintProductPublishingAfter15Days_ShouldHaveKommerSnart |    No    | Stereo  | After 15 Days |         -         |  Kommer snart  |
        //+---+------------------------------------------------------------------------------+----------+---------+---------------+-------------------+----------------+
        //| 8 | Process_PhysicalProductAlreadyKommerSnart_NoDuplicateKommerSnarts            |    Yes   |    -    | After 15 Days |    Kommer snart   |        -       |
        //+---+------------------------------------------------------------------------------+----------+---------+---------------+-------------------+----------------+

        [TestMethod]
        public void Process_PhysicalProductPublishingAfter15Days_ShouldHaveKommerSnart()
        {
            // Arrange
            PopulateWorks("00001", DateTime.Now.AddDays(15), physicalProduct: true);

            // Act
            _workResultsProcessor.Process(_works);
            var product = GetProductById("00001");

            // Assert
            Assert.IsTrue(product.Labels.Contains(KommerSnartLabel), "Kommer snart label should be added for product publishing after 15 days.");
        }

        [TestMethod]
        public void Process_PhysicalProductPublishedToday_NoKommerSnart()
        {
            // Arrange
            PopulateWorks("00001", DateTime.Now, physicalProduct: true);

            // Act
            _workResultsProcessor.Process(_works);
            var product = GetProductById("00001");

            // Assert
            Assert.IsFalse(product.Labels.Contains(KommerSnartLabel), "Product published today should not have label Kommer snart.");
        }

        [TestMethod]
        public void Process_PhysicalProductPublishingAfter30Days_ShouldHaveKommerSnart()
        {
            // Arrange
            PopulateWorks("00001", DateTime.Now.AddDays(30), physicalProduct: true);

            // Act
            _workResultsProcessor.Process(_works);
            var product = GetProductById("00001");

            // Assert
            Assert.IsTrue(product.Labels.Contains(KommerSnartLabel), "Product publishing after 30 days should have label Kommer snart.");
        }

        [TestMethod]
        public void Process_PhysicalProductPublishingAfter31Days_NoKommerSnart()
        {
            // Arrange
            PopulateWorks("00001", DateTime.Now.AddDays(31), physicalProduct: true);

            // Act
            _workResultsProcessor.Process(_works);
            var product = GetProductById("00001");

            // Assert
            Assert.IsFalse(product.Labels.Contains(KommerSnartLabel), "Product publishing after 31 days should not have Kommer snart label.");
        }

        [TestMethod]
        public void Process_PhysicalProductPublishedYesterday_NoKommerSnart()
        {
            // Arrange
            PopulateWorks("00001", DateTime.Now.AddDays(-1), physicalProduct: true);

            // Act
            _workResultsProcessor.Process(_works);
            var product = GetProductById("00001");

            // Assert
            Assert.IsFalse(product.Labels.Contains(KommerSnartLabel), "Product published yesterday should not have Kommer snart label.");
        }

        [TestMethod]
        public void Process_NonPhysicalProductPublishingAfter15Days_NoKommerSnart()
        {
            // Arrange
            PopulateWorks("00001", DateTime.Now.AddDays(15), physicalProduct: false);

            // Act
            _workResultsProcessor.Process(_works);
            var product = GetProductById("00001");

            // Assert
            Assert.IsFalse(product.Labels.Contains(KommerSnartLabel), "Non physical product publishing after 15 days should not have Kommer snart label.");
        }

        [TestMethod]
        public void Process_NonPhysicalImprintProductPublishingAfter15Days_ShouldHaveKommerSnart()
        {
            // Arrange
            PopulateWorks("00001", DateTime.Now.AddDays(15), physicalProduct: false, isStereoImprint: true);

            // Act
            _workResultsProcessor.Process(_works);
            var product = GetProductById("00001");

            // Assert
            Assert.IsTrue(product.Labels.Contains(KommerSnartLabel), "Non physical product having Gyldendal Stereo publishing after 15 days should have Kommer snart label.");
        }

        [TestMethod]
        public void Process_PhysicalProductAlreadyKommerSnart_NoDuplicateKommerSnarts()
        {
            // Arrange
            PopulateWorks("00001", DateTime.Now.AddDays(15), physicalProduct: true, isStereoImprint: false, new List<string> { "Kommer snart" });

            // Act
            _workResultsProcessor.Process(_works);
            var product = GetProductById("00001");

            // Assert
            Assert.AreEqual(1, product.Labels.Count, "Already set Kommer snart product should not contain duplicate labels.");
        }
    }
}