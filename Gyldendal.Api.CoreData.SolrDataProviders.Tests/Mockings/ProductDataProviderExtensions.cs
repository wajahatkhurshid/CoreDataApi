using System.Collections.Generic;
using System.Configuration;
using Gyldendal.Api.CoreData.GqlToSolrConnector.Model;
using Gyldendal.Api.CoreData.GqlToSolrConnector.SolrSearch;
using Gyldendal.Api.CoreData.SolrDataProviders.Mappings;
using Gyldendal.Api.CoreData.SolrDataProviders.Product;
using Gyldendal.Api.CoreData.SolrDataProviders.Utils;
using Moq;
using SolrNet;
using SolrNet.Commands.Parameters;

namespace Gyldendal.Api.CoreData.SolrDataProviders.Tests.Mockings
{
    public static class ProductDataProviderExtensions
    {
        public static Mock<ISearch<SolrContracts.Product.Product>> GetMockedSolrSearch(this SolrQueryResults<SolrContracts.Product.Product> solrProductsQueryResult)
        {
            var solrSearch = new Mock<ISearch<SolrContracts.Product.Product>>();

            solrSearch.Setup(a => a.SearchSolrQueryResults(It.IsAny<ISolrQuery>(), It.IsAny<int>(), It.IsAny<int>(),
                    It.IsAny<List<OrderByParam>>(), It.IsAny<GroupingParameters>(), It.IsAny<FacetParameters>(),
                    It.IsAny<ICollection<ISolrQuery>>(), It.IsAny<IEnumerable<KeyValuePair<string, string>>>(),
                    It.IsAny<string[]>()))
                .Returns(solrProductsQueryResult);

            var productSearchResult = solrProductsQueryResult.GetSearchResult();
            solrSearch.Setup(a => a.Search(It.IsAny<ISolrQuery>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<List<OrderByParam>>(),
                    It.IsAny<GroupingParameters>(),
                    It.IsAny<FacetParameters>(),
                    It.IsAny<ICollection<ISolrQuery>>(),
                    It.IsAny<IEnumerable<KeyValuePair<string, string>>>()))
                .Returns(productSearchResult);

            return solrSearch;
        }

        public static Mock<ProductDataProvider> GetMockedProductDataProvider(this Mock<ISearch<SolrContracts.Product.Product>> mockedSolrSearchOfProds)
        {
            return new Mock<ProductDataProvider>
            (
                "products",
                ConfigurationManager.AppSettings["SolrUrl"],
                GqlToSolrFieldMapping.GetMappings(),
                ConfigurationManager.AppSettings["MediaTypeValues"].Split(','),
                new ResultProcessor(null),
                new ProductSolrFilterGenerator(),
                new Utils.FilterInfoToSolrQueryBuilder(),
                mockedSolrSearchOfProds.Object
            );
        }

        private static SearchResult<SolrContracts.Product.Product> GetSearchResult(this SolrQueryResults<SolrContracts.Product.Product> solrProductsQueryResult)
        {
            return new SearchResult<SolrContracts.Product.Product>(solrProductsQueryResult);
        }

    }
}
