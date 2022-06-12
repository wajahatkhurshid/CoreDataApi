using Gyldendal.Api.CoreData.GqlToSolrConnector.Infrastructure;
using SolrNet;
using System.Collections.Generic;
using Gyldendal.Api.CoreData.GqlToSolrConnector.Model;
using Field = Gyldendal.Api.CoreData.SolrContracts.Product.ProductSchemaField;

namespace Gyldendal.Api.CoreData.GqlToSolrConnector.SearchRepositories.Common
{
    public class WorkSearchSolrQueryBuilder : BaseSolrQueryBuilder, ISolrQueryBuilder
    {
        public WorkSearchSolrQueryBuilder()
        {
            SearchMatrix = new List<SolrFieldQueryParams>
        {
            new SolrFieldQueryParams(Field.SubstringField,  isQuotedString: true , isWildCard: true , boostFactor: 50),
            new SolrFieldQueryParams(Field.ExactMatch,      isQuotedString: false, isWildCard: false, boostFactor: 10),
            new SolrFieldQueryParams(Field.ThemaExactmatch, isQuotedString: false, isWildCard: false, boostFactor: 10),
            new SolrFieldQueryParams(Field.ThemaSubstring,  isQuotedString: true , isWildCard: true , boostFactor: 5),
        };
        }

        public override AbstractSolrQuery Build(string gqlQueryValue)
        {
            gqlQueryValue = gqlQueryValue.RemoveWildCard();
            return base.Build(gqlQueryValue);
        }
    }
}