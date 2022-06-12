using System.Collections.Generic;
using System.Linq;
using Gyldendal.Api.CoreData.Common;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Requests;
using Gyldendal.Common.WebUtils.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gyldendal.Api.CoreData.SolrDataProviders.Tests.Mockings;
using SolrNet;
using SolrProduct = Gyldendal.Api.CoreData.SolrContracts.Product.Product;

namespace Gyldendal.Api.CoreData.SolrDataProviders.Tests.ProductDataProfile
{
    [TestClass]
    public class ProductDataProfile
    {
        private static SolrQueryResults<SolrProduct> GetMockedSolrQueryResult()
        {
            var groupedResults = new GroupedResults<SolrProduct>
            {
                Groups = new List<Group<SolrProduct>>
                {
                    new Group<SolrProduct>
                    {
                        Documents = new List<SolrProduct>
                        {
                            new SolrProduct {Isbn13 = "9788705095050", WebsiteId = 11, ProductId = "40068"},
                            new SolrProduct {Isbn13 = "9788705095050", WebsiteId = 12, ProductId = "40068"},
                            new SolrProduct {Isbn13 = "9788705095050", WebsiteId = 13, ProductId = "40068"},
                            new SolrProduct {Isbn13 = "9788705095050", WebsiteId = 14, ProductId = "40068"},
                        },
                        GroupValue = "40068",
                        NumFound = 4
                    },
                    new Group<SolrProduct>
                    {
                        Documents = new List<SolrProduct>
                        {
                            new SolrProduct {Isbn13 = "9788705095051", WebsiteId = 11, ProductId = "40069"},
                            new SolrProduct {Isbn13 = "9788705095051", WebsiteId = 12, ProductId = "40069"},
                            new SolrProduct {Isbn13 = "9788705095051", WebsiteId = 14, ProductId = "40069"},
                            new SolrProduct {Isbn13 = "9788705095051", WebsiteId = 19, ProductId = "40069"},
                        },
                        GroupValue = "40069",
                        NumFound = 4
                    },
                    new Group<SolrProduct>
                    {
                        Documents = new List<SolrProduct>
                        {
                            new SolrProduct {Isbn13 = "9788705095052", WebsiteId = 13, ProductId = "40070"},
                            new SolrProduct {Isbn13 = "9788705095052", WebsiteId = 14, ProductId = "40070"},
                            new SolrProduct {Isbn13 = "9788705095052", WebsiteId = 15, ProductId = "40070"},
                        },
                        GroupValue = "40070",
                        NumFound = 3
                    },
                    new Group<SolrProduct>
                    {
                        Documents = new List<SolrProduct>
                        {

                            new SolrProduct {Isbn13 = "9788705095053", WebsiteId = 20, ProductId = "40071"},
                            new SolrProduct {Isbn13 = "9788705095053", WebsiteId = 21, ProductId = "40071"},
                        },
                        GroupValue = "40071",
                        NumFound = 2
                    },
                },
                Matches = 13,
                Ngroups = 4
            };
            return new SolrQueryResults<SolrProduct>
            {
                Grouping = new Dictionary<string, GroupedResults<SolrProduct>>
                {
                    {"isbnGroup", groupedResults}
                }
            };
        }

        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public void ProductDataProvider_DataProfile_GetIvrProducts_Success()
        {
            // Arrange
            var productPool = GetMockedSolrQueryResult();

            var productDataProvider = productPool.GetMockedSolrSearch().GetMockedProductDataProvider();

            var request = new GetProductsByDataScopeRequest
            {
                ProductDataProfile = Contracts.Enumerations.ProductDataProfile.IVR,
                DataScope = DataScope.GyldendalPlus,
                PageSize = 10,
                PageIndex = 0
            };

            // Act
            var products = productDataProvider.Object.GetProductsByDataScope(request);


            // Assert
            Assert.IsTrue(products.Count(product => product is IvrProductInfo) == productPool.Grouping.Values.First().Ngroups, "Invalid product data received.");
        }


        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public void ProductDataProvider_DataProfile_GetIvrProducts_WebshopsCount_Success()
        {
            // Arrange
            var productPool = GetMockedSolrQueryResult();

            var productDataProvider = productPool.GetMockedSolrSearch().GetMockedProductDataProvider();

            var request = new GetProductsByDataScopeRequest
            {
                ProductDataProfile = Contracts.Enumerations.ProductDataProfile.IVR,
                DataScope = DataScope.GyldendalPlus,
                PageSize = 10,
                PageIndex = 0
            };

            // Act
            var products = productDataProvider.Object.GetProductsByDataScope(request).Select(a => (IvrProductInfo)a)
                .ToList();


            // Assert
            Assert.IsTrue(
                products.FirstOrDefault(product => product.Isbn13 == "9788705095051")?.WebShops.Count == 4,
                "Invalid product data received.");
        }


        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public void ProductDataProvider_DataProfile_InvalidProfileProducts_Failure()
        {
            // Arrange
            var productPool = GetMockedSolrQueryResult();

            var productDataProvider = productPool.GetMockedSolrSearch().GetMockedProductDataProvider();

            var request = new GetProductsByDataScopeRequest
            {
                ProductDataProfile = (Contracts.Enumerations.ProductDataProfile)100,
                DataScope = DataScope.GyldendalPlus,
                PageSize = 10,
                PageIndex = 0
            };

            try
            {
                // Act
                productDataProvider.Object.GetProductsByDataScope(request);

                Assert.IsTrue(false, "Exception must be thrown for this test to pass.");
            }
            catch (ValidationException e)
            {
                // Assert
                Assert.IsTrue(e.ErrorCode == (ulong)ErrorCodes.InvalidProductDataProfile,
                    "This is a valid data profile. The test passes for invalid profile.");
            }
        }

        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public void ProductDataProvider_DataProfile_GetIvrProductsCount_Success()
        {
            // Arrange
            var productPool = GetMockedSolrQueryResult();

            var productDataProvider = productPool.GetMockedSolrSearch().GetMockedProductDataProvider();


            // Act
            var count = productDataProvider.Object.GetProductCountByDataScope(DataScope.GyldendalPlus,
                Contracts.Enumerations.ProductDataProfile.IVR);


            // Assert
            Assert.IsTrue(count == productPool.GroupBy(a => a.Isbn13).Count(),
                "count of GetProductCountByDataScope must be equal to the set of distinct ISBNs we have in source.");
        }
    }
}
