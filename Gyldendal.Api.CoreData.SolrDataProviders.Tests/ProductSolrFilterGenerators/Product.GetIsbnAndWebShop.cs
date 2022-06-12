using Gyldendal.Api.CoreData.SolrContracts.Product;
using Gyldendal.Api.CoreData.SolrDataProviders.Infrastructure.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

// ReSharper disable RedundantAssignment

namespace Gyldendal.Api.CoreData.SolrDataProviders.Tests.ProductSolrFilterGenerators
{
    public partial class Product
    {
        [TestMethod]
        public void GetProductsByIsbnsWebShopSaleConfigTrue()
        {
            var isbnsList = new[] { "123", "456" };
            var searchCriteria = new ProductFilterGenerationInput
            {
                Isbns = isbnsList,
                WebShops = _webShops,
                SkipInvalidSaleConfigProds = true
            };
            var filters = _filterGenerator.Generate(searchCriteria).ToArray();
            var i = 0;
            Assert.AreEqual(filters.Length, 3);
            FieldAssert(filters[i++], ProductSchemaField.WebsiteId.GetFieldName(), false, data: _webShops);
            FieldAssert(filters[i++], ProductSchemaField.IsSaleConfigAvailable.GetFieldName(), false, false, data: "true");
            FieldAssert(filters[i++], ProductSchemaField.Isbn13.GetFieldName(), false, data: isbnsList);

            //var solrQueries = SolrQueryBuilder.Build(filters).ToList();

            //var query0 = solrQueries[0] as SolrNet.SolrMultipleCriteriaQuery;
            //AssertSolrMultipleCriteriaQuery(query0, ProductSchemaField.WebsiteId.GetFieldName(), _webShops);

            //var query1 = solrQueries[1] as SolrNet.SolrQueryByField;
            //AssertSolrQueryByField(query1, ProductSchemaField.IsSaleConfigAvailable.GetFieldName(), "true");

            //var query2 = solrQueries[2] as SolrNet.SolrMultipleCriteriaQuery;
            //AssertSolrMultipleCriteriaQuery(query2, ProductSchemaField.Isbn13.GetFieldName(), isbnsList);
        }

        [TestMethod]
        public void GetProductsByIsbnsWebShopSaleConfigFalse()
        {
            var isbnsList = new[] { "123", "456" };
            var searchCriteria = new ProductFilterGenerationInput
            {
                Isbns = isbnsList,
                WebShops = _webShops,
                SkipInvalidSaleConfigProds = false
            };
            var filters = _filterGenerator.Generate(searchCriteria).ToArray();
            var i = 0;
            Assert.AreEqual(filters.Length, 2);
            FieldAssert(filters[i++], ProductSchemaField.WebsiteId.GetFieldName(), false, data: _webShops);
            FieldAssert(filters[i++], ProductSchemaField.Isbn13.GetFieldName(), false, data: isbnsList);

            //var solrQueries = SolrQueryBuilder.Build(filters).ToList();

            //var query0 = solrQueries[0] as SolrNet.SolrMultipleCriteriaQuery;
            //AssertSolrMultipleCriteriaQuery(query0, ProductSchemaField.WebsiteId.GetFieldName(), _webShops);

            //var query2 = solrQueries[1] as SolrNet.SolrMultipleCriteriaQuery;
            //AssertSolrMultipleCriteriaQuery(query2, ProductSchemaField.Isbn13.GetFieldName(), isbnsList);
        }
    }
}