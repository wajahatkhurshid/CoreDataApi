using System.Linq;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.GqlToSolrConnector.Faceting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolrNet;
using SolrNet.Commands.Parameters;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

// ReSharper disable ExpressionIsAlwaysNull

namespace Gyldendal.Api.CoreData.SolrDataProviders.Tests.Facets
{
    [TestClass]
    public class PriceFacetBuilderTests
    {
        [TestMethod]
        public void Facets_0To500With100Gap_MustHave5Facets()
        {
            FacetParameters facets = null;
            var facetsParams = new PriceRangeFacetParams { Gap = 100m, Max = 500m, Min = 0m };
            var facetParams = facets.Build(facetsParams);
            AreEqual(facetParams.Queries.Count, 5);
        }

        [TestMethod]
        public void Facets_MustHave1AsMinCount()
        {
            FacetParameters facets = null;
            var facetsParams = new PriceRangeFacetParams { Gap = 100m, Max = 500m, Min = 0m };
            var facetParams = facets.Build(facetsParams);
            AreEqual(facetParams.MinCount, 1);
        }

        [DataTestMethod]
        [DataRow(500)]
        [DataRow(1000)]
        [DataRow(5000)]
        public void Facets_0ToInputWith100Gap_LastFacetMustHaveShortMaxValueAsUpperLimit(int max)
        {
            const decimal gap = 100m;
            var lastFacetStartRangeValue = max - gap;
            const decimal lastFacetEndRangeValue = short.MaxValue - 0.01m;

            var facetParameters = new PriceRangeFacetParams { Gap = 100m, Max = max, Min = 0m };

            var facets = new FacetParameters();
            var facetParams = facets.Build(facetParameters);

            var lastFacet = facetParams.Queries.Last();

            VerifyFacet(lastFacet, lastFacetStartRangeValue, lastFacetEndRangeValue);
        }

        [DataTestMethod]
        [DataRow(0, 500, 100)]
        [DataRow(0, 1000, 100)]
        [DataRow(100, 5000, 100)]
        public void VerifyAllFacetQueries_0To500With100Gap(long min, long max, long gap)
        {
            var start = min;

            var facetParameters = new PriceRangeFacetParams { Gap = gap, Max = max, Min = min };

            var facets = new FacetParameters();
            var facetParams = facets.Build(facetParameters);

            for (var index = 0; index < facetParams.Queries.ToList().Count - 1; index++)
            {
                var facetParamsQuery = facetParams.Queries.ToList()[index];
                VerifyFacet(facetParamsQuery, start, (start += gap) - 0.01m);
            }
        }

        private static void VerifyFacet(ISolrFacetQuery facet, decimal start, decimal end)
        {
            if (!(facet is SolrFacetQuery solrFacetQuery))
            {
                Fail("Query Type should be SolrFacetQuery.");
                return;
            }

            if (!(solrFacetQuery.Query is LocalParams.LocalParamsQuery localParamsQuery))
            {
                Fail("Query Type should be LocalParams.LocalParamsQuery.");
                return;
            }

            if (!(localParamsQuery.Query is SolrQueryByRange<Money> queryByRange))
            {
                Fail("Query Type should be SolrQueryByRange<Money>.");
                return;
            }

            AreEqual(queryByRange.From.Value, start);
            AreEqual(queryByRange.To.Value, end);
        }
    }
}