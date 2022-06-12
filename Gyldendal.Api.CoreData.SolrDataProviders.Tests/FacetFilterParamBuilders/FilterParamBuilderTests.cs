using System.Collections.Generic;
using System.Linq;
using Gyldendal.Api.CoreData.GqlToSolrConnector.Filtering;
using Gyldendal.Api.CoreData.GqlToSolrConnector.Model;
using Gyldendal.Api.CoreData.SolrContracts.Product;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolrNet;

namespace Gyldendal.Api.CoreData.SolrDataProviders.Tests.FacetFilterParamBuilders
{
    [TestClass]
    public class FilterParamBuilderTests
    {
        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public void FilterQueryBuilder_EmptyFilters_Success()
        {
            var filterBuilder = new FilterQueryBuilder();

            var solrQuery = filterBuilder.Build(new List<FilterInfo>());

            Assert.IsNull(solrQuery, "solrQuery is expected to be null.");
        }

        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public void FilterQueryBuilder_EmptyFilterValues_Success()
        {
            var filters = new List<FilterInfo>
            {
                new FilterInfo
                {
                    FilterValues = new List<string>(),
                }
            };

            var filterBuilder = new FilterQueryBuilder();

            var solrQuery = filterBuilder.Build(filters);

            Assert.IsTrue(!solrQuery.Any(),"solrQuery is expected to be empty.");
        }

        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public void FilterQueryBuilder_MultipleCriteriaInclFacetTag_Success()
        {
            var filters = new List<FilterInfo>
            {
                new FilterInfo
                {
                    SolrFieldName = ProductSchemaField.DefaultPrice.GetFieldName(),
                    Quoted = false,
                    ExcludeFromFacets = false,
                    FilterValues = new List<string>
                    {
                        "0 TO 100",
                        "100 TO 200"
                    }
                }
            };

            var filterBuilder = new FilterQueryBuilder();

            var solrQuery = filterBuilder.Build(filters).ToList();

            Assert.IsTrue(solrQuery != null && solrQuery.Count == filters.Select(a => a.FilterValues).Count(),
                "solrQuery query count must be equal to no. of filter values passed as input."
            );
        }

        [TestMethod]
        [TestCategory(Constants.UnitTest)]
        public void FilterQueryBuilder_MultipleCriteriaExclFacetTag_Success()
        {
            var filters = new List<FilterInfo>
            {
                new FilterInfo
                {
                    SolrFieldName = ProductSchemaField.DefaultPrice.GetFieldName(),
                    Quoted = false,
                    ExcludeFromFacets = true,
                    FilterValues = new List<string>
                    {
                        "0 TO 100",
                        "100 TO 200"
                    }
                }
            };

            var filterBuilder = new FilterQueryBuilder();

            var solrQuery = filterBuilder.Build(filters).Select(a => (LocalParams.LocalParamsQuery) a).ToList();

            Assert.IsTrue(solrQuery != null
                          && solrQuery.Any(a => a.Local.Keys.Contains("tag"))
                          && solrQuery.Count == filters.Select(a => a.FilterValues).Count(),
                "solrQuery is expected to contain local param 'tag' and query count to be equal to no. of filter values passed as input.");
        }
    }
}
