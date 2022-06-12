using Gyldendal.Api.CoreData.SolrDataProviders.Contributor;
using Gyldendal.Api.CoreData.SolrDataProviders.Infrastructure.FilterGenerator;
using Gyldendal.Api.CoreData.SolrDataProviders.Infrastructure.Models;
using Gyldendal.Api.CoreData.SolrDataProviders.Product;

namespace Gyldendal.Api.CoreData.SolrDataProviders.Tests.FilterInfoToSolrQueryBuilder
{
    public partial class FilterInfo : BaseFilterTest
    {
        private readonly IFilterGenerator<ProductFilterGenerationInput> _productFilterGenerator = new ProductSolrFilterGenerator();

        private readonly IFilterGenerator<ContributorFilterGenerationInput> _contributorfilterGenerator = new ContributorSolrFilterGenerator();
    }
}
