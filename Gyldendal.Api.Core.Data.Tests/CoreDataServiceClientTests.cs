using System;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Requests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gyldendal.Api.CoreData.ApiClient;
using Gyldendal.Api.CoreData.ContentfulProxy.Implementation;

namespace Gyldendal.Api.CoreData.Tests
{
    [TestClass]
    public partial class CoreDataServiceClientTests
    {
        private readonly string _coreDataServiceUrl = "https://dev-coredata.gyldendal.dk/api/";

        [TestMethod]
        public async Task GetAuthorByAssetIdTest()
        {
            var client = new ContentfulApiClient("gn9xa1f2zc94", "Development",
                "iufA4b2Lzw2-FgigGY3fwxQHKPfHGaWQHYxxKbYRSUM", "jSqwVw_JLYpch-oONKi14bZG6A_vkA56Lpig0x55Hrs");

            var entry = await client.GetAuthorByAssetIdAsync("4d9J56DVQBMnQzx4GVs2Jl");

            Assert.IsNotNull(entry);
        }

        //private string _coreDataServiceUrl = ""http://localhost:6653/api/";
        [TestMethod]
        [TestCategory(Constants.DisabledTest)]
        public void GetLicensedProductsByIsbnTest()
        {
            var client = new CoreDataServiceClient(_coreDataServiceUrl);

            // ReSharper disable once UnusedVariable
            var data = client.GetLicensedProductsByIsbn(new List<string> { "9788762505797", "9788762501706", "9788741256788" },
                WebShop.Gu);
        }

        [TestMethod]
        [TestCategory(Constants.DisabledTest)]
        public void GetAllProductsByIsbnTest()
        {
            var client = new CoreDataServiceClient(_coreDataServiceUrl);

            // http://localhost:6653/api/v1/Product/GetLicensedProductsByIsbn?webshop=2

            var data = client.GetAllProductsByIsbn(new List<string>
                {"9788762505797", "9788762501706", "9788741256788"});

            Assert.IsNotNull(data);
        }

        [TestMethod]
        [TestCategory(Constants.DisabledTest)]
        public void WorkSearchV2()
        {
            var client = new CoreDataServiceClient(_coreDataServiceUrl);

            var data = client.GetWorksSearchResponseV2(new WorkSearchRequestV2
            {
                Webshop = WebShop.GyldendalDk,
                Gql = "WorkSearch(harry)",
                Filters = new Dictionary<FilterType, List<string>>
                {
                    {
                        FilterType.MaterialTypeName,
                        new List<string>
                        {
                            "andet"
                        }
                    }
                },
                FacetTypes = (new List<int>
                {
                    1,2,3,4,5,6,7,8,9
                }).Select(x => (FacetType)x).ToList(),
                Paging = new PagingInfo
                {
                    PageIndex = 0,
                    PageSize = 50
                },
                OrderBy = OrderBy.None,
                SortBy = SortBy.Asc
            });

            Assert.IsNotNull(data);
        }

        [TestMethod]
        [TestCategory(Constants.DisabledTest)]
        public void ProductSearchV2()
        {
            var client = new CoreDataServiceClient(_coreDataServiceUrl);

            var data = client.SearchProductsV2(new ProductSearchRequestV2
            {
                Webshop = WebShop.ClubBogklub,
                Gql = "WorkSearch(p)",
                Filters = new Dictionary<FilterType, List<string>>
                {
                    {
                        FilterType.Genre,
                        new List<string>
                        {
                            "Historiske romaner"
                        }
                    }
                },
                FacetTypes = (new List<int>
                {
                    1,2,3,4,5,6,7,8,9
                }).Select(x => (FacetType)x).ToList(),
                Paging = new PagingInfo
                {
                    PageIndex = 0,
                    PageSize = 50
                },
                OrderBy = OrderBy.None,
                SortBy = SortBy.Asc
            });

            Assert.IsNotNull(data);
        }
    }
}