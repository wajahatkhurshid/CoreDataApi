using Gyldendal.Api.CoreData.Common;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.SolrContracts.Product;
using Gyldendal.Common.WebUtils.Exceptions;
using NewRelic.Api.Agent;
using SolrNet;
using SolrNet.Commands.Parameters;
using static Gyldendal.Api.CoreData.GqlToSolrConnector.Faceting.BaseFacetBuilder;

namespace Gyldendal.Api.CoreData.GqlToSolrConnector.Faceting
{
    public static class PriceRangeFacetBuilder
    {
        [Trace]
        public static FacetParameters Build(this FacetParameters facetParameters, PriceRangeFacetParams priceFacet)
        {
            if (IsInvalidFacetParams(priceFacet)) return facetParameters;

            ValidatePriceRanges(priceFacet);

            facetParameters = facetParameters ?? new FacetParameters { MinCount = 1 };

            var facets = new FacetParameters { MinCount = 1 };

            decimal @from;

            for (@from = priceFacet.Min; @from < priceFacet.Max - priceFacet.Gap; @from += priceFacet.Gap)
            {
                var to = priceFacet.Gap + @from - 0.01m;
                facets.Queries.Add(new SolrFacetQuery(ExcludeFacetFromQuery + GeneratePriceFacet(@from, to)));
            }

            facets.Queries.Add(new SolrFacetQuery(ExcludeFacetFromQuery + GeneratePriceFacet(@from, short.MaxValue - 0.01m)));
            return ConcatFacets(facetParameters, facets);
        }

        private static SolrQueryByRange<Money> GeneratePriceFacet(decimal @from, decimal to)
        {
            return new SolrQueryByRange<Money>(
                ProductSchemaField.DefaultPrice.GetFieldName(),
                new Money(from, "DKK"),
                new Money(to, "DKK")
            );
        }

        private static bool IsInvalidFacetParams(PriceRangeFacetParams priceRangeFacetParams)
        {
            return priceRangeFacetParams == null
                   || (priceRangeFacetParams.Min == 0
                       && priceRangeFacetParams.Max == 0
                       && priceRangeFacetParams.Gap == 0);
        }

        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        private static void ValidatePriceRanges(PriceRangeFacetParams inputParams)
        {
            if (inputParams.Min < 0
                || inputParams.Max < inputParams.Min
                || inputParams.Gap < 0
                || inputParams.Gap > inputParams.Max)
            {
                throw new ValidationException((ulong)ErrorCodes.InvalidPriceFacet,
                    ErrorCodes.InvalidPriceFacet.GetDescription(), Extensions.CoreDataSystemName, null);
            }
        }
    }
}