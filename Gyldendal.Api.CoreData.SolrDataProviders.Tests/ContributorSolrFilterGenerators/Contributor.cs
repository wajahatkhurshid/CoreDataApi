using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.SolrContracts.Contributor;
using Gyldendal.Api.CoreData.SolrDataProviders.Contributor;
using Gyldendal.Api.CoreData.SolrDataProviders.Infrastructure.FilterGenerator;
using Gyldendal.Api.CoreData.SolrDataProviders.Infrastructure.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Gyldendal.Api.CoreData.GqlToSolrConnector.Model;

// ReSharper disable RedundantAssignment

namespace Gyldendal.Api.CoreData.SolrDataProviders.Tests.ContributorSolrFilterGenerators
{
    [TestClass]
    public class Contributor : BaseFilterTest
    {
        private readonly IFilterGenerator<ContributorFilterGenerationInput> _filterGenerator = new ContributorSolrFilterGenerator();

        private readonly WebShop[] _webShops = { WebShop.GyldendalDk };

        [TestMethod]
        public void GetContributorsByIds()
        {
            var i = 0;
            var contributorIds = new[] { "123456" };
            var searchCriteria = new ContributorFilterGenerationInput(webShops: _webShops, contributorIds: contributorIds);
            var filters = _filterGenerator.Generate(searchCriteria).ToArray();

            Assert.AreEqual(filters.Length, 2);

            FieldAssert(filters[i++], ContributorSchemaField.WebsiteId.GetFieldName(), false, false, data: _webShops);
            FieldAssert(filters[i++], ContributorSchemaField.ContributorId.GetFieldName(), false, data: contributorIds);
        }

        [TestMethod]
        public void GetContributorsByShop()
        {
            var i = 0;
            var searchCriteria = new ContributorFilterGenerationInput(webShops: _webShops);
            var filters = _filterGenerator.Generate(searchCriteria).ToArray();

            Assert.AreEqual(filters.Length, 1);

            FieldAssert(filters[i++], ContributorSchemaField.WebsiteId.GetFieldName(), false, false, data: _webShops);
        }

        [TestMethod]
        public void SearchBySearchName()
        {
            var i = 0;
            var searchString = ("contributor-name");
            var searchCriteria = new ContributorFilterGenerationInput(webShops: _webShops, searchString: searchString);
            var filters = _filterGenerator.Generate(searchCriteria).ToArray();

            Assert.AreEqual(filters.Length, 2);

            FieldAssert(filters[i++], ContributorSchemaField.WebsiteId.GetFieldName(), false, false, data: _webShops);
            FieldAssert(filters[i], ContributorSchemaField.ContributorName.GetFieldName(), false, false, data:
                FilterInfo.QuoteString($"*{searchString}*"));

            Assert.IsTrue(filters[i].NestedFilters != null && filters[i].NestedFilters.Any());
            FieldAssert(filters[i].NestedFilters.First(), ContributorSchemaField.SearchName.GetFieldName(), false, false, data:
                FilterInfo.QuoteString($"{searchString}"));
        }
    }
}