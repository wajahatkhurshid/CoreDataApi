using Gyldendal.Api.CoreData.GqlToSolrConnector;
using Gyldendal.Api.CoreData.GqlToSolrConnector.Model;
using Gyldendal.Api.CoreData.GqlToSolrConnector.SearchRepositories.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Gyldendal.Api.CoreData.SolrDataProviders.Tests.SolrSearch.Common
{
    [TestClass]
    public class GeneralSearchSolrQueryBuilderTests
    {
        [DataTestMethod]
        [DataRow("harry potter")]
        [DataRow("harry")]
        [DataRow("J.K. Rowling")]
        public void GeneralSearch_VerifySolrQuery(string searchQueryValue)
        {
            var quoteString = FilterInfo.QuoteString(searchQueryValue);
            var queryBuilder = new GeneralSearchSolrQueryBuilder();
            var result = queryBuilder.Build(searchQueryValue);
            var query = result.Serialize();
            AreEqual(
                query,
                $"((titleReplaced:(\"{quoteString}\"))^1000 OR (titleReplaced:(*{quoteString}*))^500 OR " +
                $"(isbn13:(\"{searchQueryValue}\"))^500 OR (subtitleReplaced:(\"{searchQueryValue}\"))^450 OR " +
                $"(authorNames:(\"{quoteString}\"))^400 OR (authorNames:(*{quoteString}*))^200 OR " +
                $"(seriesNamesReplaced:(\"{searchQueryValue}\"))^150)"
            );
        }
    }
}