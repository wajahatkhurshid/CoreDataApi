using Gyldendal.Api.CoreData.GqlToSolrConnector.Infrastructure;
using System.Collections.Generic;
using Gyldendal.Api.CoreData.GqlToSolrConnector.Model;
using Field = Gyldendal.Api.CoreData.SolrContracts.Product.ProductSchemaField;

namespace Gyldendal.Api.CoreData.GqlToSolrConnector.SearchRepositories.Common
{
    public class GeneralSearchSolrQueryBuilder : BaseSolrQueryBuilder, ISolrQueryBuilder
    {
        public GeneralSearchSolrQueryBuilder()
        {
            SearchMatrix = new List<SolrFieldQueryParams>
        {
            new SolrFieldQueryParams(Field.TitleReplaced,       isQuotedString: true , isWildCard: false, boostFactor: 1000),
            new SolrFieldQueryParams(Field.TitleReplaced,       isQuotedString: true , isWildCard: true , boostFactor: 500),
            new SolrFieldQueryParams(Field.Isbn13,              isQuotedString: false, isWildCard: false, boostFactor: 500),
            new SolrFieldQueryParams(Field.SubtitleReplaced,    isQuotedString: false, isWildCard: false, boostFactor: 450),
            new SolrFieldQueryParams(Field.AuthorNames,         isQuotedString: true , isWildCard: false, boostFactor: 400),
            new SolrFieldQueryParams(Field.AuthorNames,         isQuotedString: true , isWildCard: true , boostFactor: 200),
            new SolrFieldQueryParams(Field.SeriesNamesReplaced, isQuotedString: false, isWildCard: false, boostFactor: 150),
        };
        }
    }
}