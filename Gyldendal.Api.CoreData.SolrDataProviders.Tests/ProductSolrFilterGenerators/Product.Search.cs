using Gyldendal.Api.CoreData.Contracts.Enumerations;
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
        public void ProductSearchNone()
        {
            var searchCriteria = new ProductFilterGenerationInput
            {
                WebShops = _webShops,
                SearchString = _searchString,
                ProductSearchType = ProductSearchType.None
            };
            var filters = _filterGenerator.Generate(searchCriteria).ToArray();

            var i = 0;
            Assert.AreEqual(filters.Length, 2);
            FieldAssert(filters[i++], ProductSchemaField.WebsiteId.GetFieldName(), false, data: _webShops);
            FieldAssert(filters[i++], ProductSchemaField.TitleContains.GetFieldName(), false, false, data: $"*{_searchString}*");
        }

        [TestMethod]
        public void ProductSearchBundle()
        {
            var searchCriteria = new ProductFilterGenerationInput
            {
                WebShops = _webShops,
                SearchString = _searchString,
                ProductSearchType = ProductSearchType.Bundle
            };
            var filters = _filterGenerator.Generate(searchCriteria).ToArray();

            var i = 0;
            Assert.AreEqual(filters.Length, 3);
            FieldAssert(filters[i++], ProductSchemaField.WebsiteId.GetFieldName(), false, data: _webShops);
            FieldAssert(filters[i++], ProductSchemaField.ProductType.GetFieldName(), false, data: ProductType.Bundle);
            FieldAssert(filters[i++], ProductSchemaField.TitleContains.GetFieldName(), false, false, data: $"*{_searchString}*");
        }

        [TestMethod]
        public void ProductSearchDigital()
        {
            var searchCriteria = new ProductFilterGenerationInput
            {
                WebShops = _webShops,
                SearchString = _searchString,
                ProductSearchType = ProductSearchType.Digital
            };
            var filters = _filterGenerator.Generate(searchCriteria).ToArray();
            var i = 0;
            Assert.AreEqual(filters.Length, 4);
            FieldAssert(filters[i++], ProductSchemaField.WebsiteId.GetFieldName(), false, data: _webShops);
            FieldAssert(filters[i++], ProductSchemaField.ProductType.GetFieldName(), false, data: ProductType.SingleProduct);
            FieldAssert(filters[i++], ProductSchemaField.IsPhysical.GetFieldName(), false, data: false);
            FieldAssert(filters[i++], ProductSchemaField.TitleContains.GetFieldName(), false, false, data: $"*{_searchString}*");
        }

        [TestMethod]
        public void ProductSearchPhysical()
        {
            var searchCriteria = new ProductFilterGenerationInput
            {
                WebShops = _webShops,
                SearchString = _searchString,
                ProductSearchType = ProductSearchType.Physical
            };
            var filters = _filterGenerator.Generate(searchCriteria).ToArray();
            var i = 0;
            Assert.AreEqual(filters.Length, 4);
            FieldAssert(filters[i++], ProductSchemaField.WebsiteId.GetFieldName(), false, data: _webShops);
            FieldAssert(filters[i++], ProductSchemaField.ProductType.GetFieldName(), false, data: ProductType.SingleProduct);
            FieldAssert(filters[i++], ProductSchemaField.IsPhysical.GetFieldName(), false, data: true);
            FieldAssert(filters[i++], ProductSchemaField.TitleContains.GetFieldName(), false, false, data: $"*{_searchString}*");
        }
    }
}