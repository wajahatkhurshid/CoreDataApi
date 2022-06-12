using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.GqlToSolrConnector.Model;
using Gyldendal.Api.CoreData.SolrDataProviders.Infrastructure.FilterGenerator;
using Gyldendal.Api.CoreData.SolrDataProviders.Infrastructure.Models;
using Gyldendal.Api.CoreData.SolrDataProviders.Product;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// ReSharper disable RedundantAssignment

namespace Gyldendal.Api.CoreData.SolrDataProviders.Tests.ProductSolrFilterGenerators
{
    [TestClass]
    public partial class Product : BaseFilterTest
    {
        private readonly string _searchString = FilterInfo.QuoteString("p");

        private readonly IFilterGenerator<ProductFilterGenerationInput> _filterGenerator = new ProductSolrFilterGenerator();

        private readonly WebShop[] _webShops = { WebShop.ClubBogklub, WebShop.ClubBoerne };
    }
}