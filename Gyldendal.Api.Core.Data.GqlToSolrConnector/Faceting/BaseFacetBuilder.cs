using SolrNet;
using SolrNet.Commands.Parameters;
using System.Linq;

namespace Gyldendal.Api.CoreData.GqlToSolrConnector.Faceting
{
    public static class BaseFacetBuilder
    {
        public static readonly LocalParams ExcludeFacetFromQuery = new LocalParams { { "ex", Constants.FilterExclTagName } };

        public static FacetParameters ConcatFacets(params FacetParameters[] inputFacets)
        {
            var facets = new FacetParameters { MinCount = 1 };
            foreach (var facetParameters in inputFacets)
            {
                if (facetParameters == null || !(facetParameters.Queries.Any())) continue;

                facetParameters.Queries.ToList().ForEach(query => facets.Queries.Add(query));
            }

            return facets;
        }
    }
}