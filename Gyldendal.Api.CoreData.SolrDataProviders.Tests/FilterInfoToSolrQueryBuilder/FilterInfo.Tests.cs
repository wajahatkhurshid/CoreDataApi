using System.Collections.Generic;
using System.Linq;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.SolrContracts.Product;
using Gyldendal.Api.CoreData.SolrContracts.Contributor;
using Gyldendal.Api.CoreData.SolrDataProviders.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gyldendal.Api.CoreData.SolrDataProviders.Infrastructure.Models;
using SolrNet;

namespace Gyldendal.Api.CoreData.SolrDataProviders.Tests.FilterInfoToSolrQueryBuilder
{
    [TestClass]
    public partial class FilterInfo
    {
        private readonly IFilterInfoToSolrQueryBuilder _filterInfoQueryBuilder;

        public FilterInfo()
        {
            _filterInfoQueryBuilder = new Utils.FilterInfoToSolrQueryBuilder();
        }

        /// <summary>
        /// Empty list of FilterInfo passed.
        /// </summary>
        [TestMethod]
        public void FilterInfo_QueryBuilder_NullFilterInfo()
        {
            // Arrange
            // ReSharper disable once CollectionNeverUpdated.Local
            var filterInfo = new List<GqlToSolrConnector.Model.FilterInfo>();

            // Act
            var queries = _filterInfoQueryBuilder.Build(filterInfo);

            // Assert
            Assert.IsNull(queries, "SolrQuery collection is expected to be null");
        }

        /// <summary>
        /// FilterInfo list passed with FilterInfo object but FilterValues property is null.
        /// </summary>
        [TestMethod]
        public void FilterInfo_QueryBuilder_NullFilterValues()
        {
            // Arrange
            var filterInfo = new List<GqlToSolrConnector.Model.FilterInfo>
            {
                new GqlToSolrConnector.Model.FilterInfo
                {
                    FilterValues = null
                }
            };

            // Act
            var queries = _filterInfoQueryBuilder.Build(filterInfo);

            // Assert
            Assert.IsTrue(queries != null && !queries.Any(), "SolrQuery collection is expected to be null");
        }

        /// <summary>
        /// Valid FilterInfo passed with SingleCriteriaQuery for SkipInvalidSalesConfigurations.
        /// </summary>
        [TestMethod]
        public void FilterInfo_QueryBuilder_ValidFilter_SingleQueryCriteria()
        {
            // Arrange
            var searchCriteria = new ProductFilterGenerationInput
            {
                SkipInvalidSaleConfigProds = true
            };
            var filters = _productFilterGenerator.Generate(searchCriteria).ToArray();

            // Act
            var queries = _filterInfoQueryBuilder.Build(filters).ToList();

            // Assert
            var query1 = queries[0] as SolrQueryByField;
            AssertSolrQueryByField(query1, ProductSchemaField.IsSaleConfigAvailable.GetFieldName(), "true");
        }

        /// <summary>
        /// Valid FilterInfo passed with MultipleCriteriaQuery for 2 Isbns.
        /// </summary>
        [TestMethod]
        public void FilterInfo_QueryBuilder_ValidFilter_MultipleQueryCriteria_Isbns()
        {
            // Arrange
            var isbnsList = new[] { "123", "456" };
            var searchCriteria = new ProductFilterGenerationInput
            {
                Isbns = isbnsList
            };
            var filters = _productFilterGenerator.Generate(searchCriteria).ToArray();

            // Act
            var queries = _filterInfoQueryBuilder.Build(filters).ToList();

            // Assert
            var query2 = queries[0] as SolrMultipleCriteriaQuery;
            AssertSolrMultipleCriteriaQuery(query2, ProductSchemaField.Isbn13.GetFieldName(), isbnsList);
        }

        /// <summary>
        /// Valid FilterInfo passed with MultipleCriteriaQuery for GU & GDK webshops.
        /// </summary>
        [TestMethod]
        public void FilterInfo_QueryBuilder_ValidFilter_MultipleQueryCriteria_Webshop()
        {
            // Arrange
            var webshops = new[] { WebShop.Gu, WebShop.GyldendalDk };
            var searchCriteria = new ProductFilterGenerationInput
            {
                WebShops = webshops
            };
            var filters = _productFilterGenerator.Generate(searchCriteria).ToArray();

            // Act
            var queries = _filterInfoQueryBuilder.Build(filters).ToList();

            // Assert
            var query2 = queries[0] as SolrMultipleCriteriaQuery;
            AssertSolrMultipleCriteriaQuery(query2, ProductSchemaField.WebsiteId.GetFieldName(), webshops);
        }

        /// <summary>
        /// Valid FilterInfo passed with nested filters.
        /// </summary>
        [TestMethod]
        public void FilterInfo_QueryBuilder_ValidFilter_NestedFilters()
        {
            // Arrange
            var searchStr = "John";
            var searchCriteria = new ContributorFilterGenerationInput
            {
                SearchString = searchStr
            };

            var filters = _contributorfilterGenerator.Generate(searchCriteria).ToArray();

            // Act
            var queries = _filterInfoQueryBuilder.Build(filters).ToList();

            // Assert
            var query = (queries[0] as SolrMultipleCriteriaQuery)?.Queries.ToList()[1] as SolrQueryByField;
            // Ignoring first query as its delibrately added by ContributorSolrFilterGenerator as ContributorName
            AssertSolrQueryByField(query, ContributorSchemaField.SearchName.GetFieldName(), searchStr);
        }
    }
}