using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.SolrContracts.WorkReview;
using Gyldendal.Api.CoreData.SolrDataProviders.Infrastructure.FilterGenerator;
using Gyldendal.Api.CoreData.SolrDataProviders.Infrastructure.Models;
using Gyldendal.Api.CoreData.SolrDataProviders.WorkReview;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

// ReSharper disable RedundantAssignment

namespace Gyldendal.Api.CoreData.SolrDataProviders.Tests.WorkReviewSolrFilterGenerators
{
    [TestClass]
    public class WorkReview : BaseFilterTest
    {
        private readonly IFilterGenerator<WorkReviewFilterGenerationInput> _filterGenerator = new WorkReviewSolrFilterGenerator();

        private readonly WebShop[] _webShops = { WebShop.GyldendalDk };

        [TestMethod]
        public void GetWorkReviewsByShop()
        {
            var searchCriteria = new WorkReviewFilterGenerationInput(webShops: _webShops);
            var filters = _filterGenerator.Generate(searchCriteria).ToArray();
            var i = 0;
            Assert.AreEqual(filters.Length, 1);
            FieldAssert(filters[i++], WorkReviewSchemaField.WebsiteId.GetFieldName(), false, false, data: _webShops);
        }

        [TestMethod]
        public void GetWorkReviewsByWorkId()
        {
            var workIds = new[] { "123456" };
            var searchCriteria = new WorkReviewFilterGenerationInput(workIds);
            var filters = _filterGenerator.Generate(searchCriteria).ToArray();
            var i = 0;
            Assert.AreEqual(filters.Length, 1);
            FieldAssert(filters[i++], WorkReviewSchemaField.WorkId.GetFieldName(), false, true, data: workIds);
        }
    }
}