using System;
using System.Net.Http;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gyldendal.Api.Core.Data.Tests.WorksController
{
    [TestClass]
    public class SearchWorks
    {
        //[TestMethod]
        //public void SearchWorksIntegrationTest()
        //{
        //    // Arrange
        //    controller.Request = new HttpRequestMessage();
        //    controller.Configuration = new HttpConfiguration();

        //    // Act
        //    var response = controller.SearchWorks(new SearchRequest());

        //    // Assert
        //    //Product product;
        //    //Assert.IsTrue(response.TryGetContentValue<Product>(out product));
        //    //Assert.AreEqual(10, product.Id);
        //}

        [TestMethod]
        public void SearchWorksWithFiltering()
        {
        }

        [TestMethod]
        public void SearchWorksWithFaceting()
        {
        }

        [TestMethod]
        public void SearchWorksWithOnePerWork()
        {
        }
    }
}