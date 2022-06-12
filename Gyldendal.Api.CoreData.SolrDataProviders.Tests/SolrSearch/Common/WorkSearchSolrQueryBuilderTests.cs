using Gyldendal.Api.CoreData.GqlToSolrConnector;
using Gyldendal.Api.CoreData.GqlToSolrConnector.Model;
using Gyldendal.Api.CoreData.GqlToSolrConnector.SearchRepositories.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

// ReSharper disable StringLiteralTypo

namespace Gyldendal.Api.CoreData.SolrDataProviders.Tests.SolrSearch.Common
{
    [TestClass]
    public class WorkSearchSolrQueryBuilderTests
    {
        [DataTestMethod]
        [DataRow("harry potter")]
        [DataRow("harry")]
        [DataRow("J.K. Rowling")]
        public void WorkSearch_VerifySolrQuery(string searchQueryValue)
        {
            var quoteString = FilterInfo.QuoteString(searchQueryValue);
            var queryBuilder = new WorkSearchSolrQueryBuilder();
            var result = queryBuilder.Build(searchQueryValue);
            var query = result.Serialize();
            AreEqual(
                query,
                $"((substringfield:(*{quoteString}*))^50 OR (exactmatchfield:(\"{searchQueryValue}\"))^10 OR " +
                $"(themaexactmatchfield:(\"{searchQueryValue}\"))^10 OR (themasubstringfield:(*{quoteString}*))^5)"
            );
        }
    }
}