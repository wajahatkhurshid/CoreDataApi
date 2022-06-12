using NewRelic.Api.Agent;
using SolrNet;
using SolrNet.Commands.Parameters;
using System.Linq;
using static Gyldendal.Api.CoreData.GqlToSolrConnector.Faceting.BaseFacetBuilder;

namespace Gyldendal.Api.CoreData.GqlToSolrConnector.Faceting
{
    public static class FieldFacetBuilder
    {
        [Trace]
        public static FacetParameters Build(this FacetParameters inputFacet, string[] facetFieldNames)
        {
            if (facetFieldNames == null || !(facetFieldNames.Any()))
            {
                return inputFacet;
            }

            inputFacet = inputFacet ?? new FacetParameters { MinCount = 1 };

            var facets = new FacetParameters
            {
                Queries = facetFieldNames.Select(solrField =>
                    (ISolrFacetQuery)
                    new SolrFacetFieldQuery(ExcludeFacetFromQuery + solrField)
                ).ToList(),
                MinCount = 1
            };

            return ConcatFacets(inputFacet, facets);
        }
    }
}