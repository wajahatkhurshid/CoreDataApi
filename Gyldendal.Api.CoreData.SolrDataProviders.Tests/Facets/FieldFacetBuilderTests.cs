using System.Linq;
using Gyldendal.Api.CoreData.GqlToSolrConnector.Faceting;
using Gyldendal.Api.CoreData.SolrContracts.Product;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolrNet;
using SolrNet.Commands.Parameters;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Gyldendal.Api.CoreData.SolrDataProviders.Tests.Facets
{
    [TestClass]
    public class FieldFacetBuilderTests
    {
        private static readonly LocalParams ExcludeFacetFromQuery = new LocalParams { { "ex", GqlToSolrConnector.Constants.FilterExclTagName } };

        [TestMethod]
        public void VerifyFieldFacets()
        {
            var facetFields = new[]
            {
                ProductSchemaField.Areas.GetFieldName(),
                ProductSchemaField.AuthorNames.GetFieldName()
            };
            var facets = new FacetParameters();
            var facetQuery = facets.Build(facetFields).Queries.ToList();
            for (var i = 0; i < facetQuery.Count; i++)
            {
                AreEqual((facetQuery[i] as SolrFacetFieldQuery)?.Field,
                    new SolrFacetFieldQuery(ExcludeFacetFromQuery + facetFields[i]).Field);
            }
        }

        [TestMethod]
        public void Facets_MustHave1AsMinCount()
        {
            var facetFields = new[]
            {
                ProductSchemaField.Areas.GetFieldName(),
                ProductSchemaField.AuthorNames.GetFieldName()
            };
            var facets = new FacetParameters();
            var facetParams = facets.Build(facetFields);
            AreEqual(facetParams.MinCount, 1);
        }
    }
}