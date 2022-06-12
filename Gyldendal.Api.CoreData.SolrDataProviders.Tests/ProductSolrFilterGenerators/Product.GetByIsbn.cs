using System;
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
        public void GetProductsByIsbnsSaleConfigTrue()
        {
            var isbnsList = new[] { "123", "456" };
            var searchCriteria = new ProductFilterGenerationInput
            {
                Isbns = isbnsList,
                SkipInvalidSaleConfigProds = true
            };
            var filters = _filterGenerator.Generate(searchCriteria).ToArray();
            var i = 0;
            Assert.AreEqual(filters.Length, 2);
            FieldAssert(filters[i++], ProductSchemaField.IsSaleConfigAvailable.GetFieldName(), false, false, data: "true");
            FieldAssert(filters[i++], ProductSchemaField.Isbn13.GetFieldName(), false, data: isbnsList);

            //var solrQueries = SolrQueryBuilder.Build(filters).ToList();

            //var query1 = solrQueries[0] as SolrNet.SolrQueryByField;
            //AssertSolrQueryByField(query1, ProductSchemaField.IsSaleConfigAvailable.GetFieldName(), "true");

            //var query2 = solrQueries[1] as SolrNet.SolrMultipleCriteriaQuery;
            //AssertSolrMultipleCriteriaQuery(query2, ProductSchemaField.Isbn13.GetFieldName(), isbnsList);
        }

        [TestMethod]
        public void GetProductsByIsbnsSaleConfigFalse()
        {
            var isbnsList = new[] { "123", "456" };
            var searchCriteria = new ProductFilterGenerationInput
            {
                Isbns = isbnsList,
                SkipInvalidSaleConfigProds = false
            };
            var filters = _filterGenerator.Generate(searchCriteria).ToArray();
            var i = 0;
            Assert.AreEqual(filters.Length, 1);
            FieldAssert(filters[i++], ProductSchemaField.Isbn13.GetFieldName(), false, data: isbnsList);

            //var solrQueries = SolrQueryBuilder.Build(filters).ToList();

            //var query2 = solrQueries[0] as SolrNet.SolrMultipleCriteriaQuery;
            //AssertSolrMultipleCriteriaQuery(query2, ProductSchemaField.Isbn13.GetFieldName(), isbnsList);
        }
    }
}