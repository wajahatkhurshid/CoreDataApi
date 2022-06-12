using System.Collections.Generic;
using System.Linq;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Business.ExtensionMethods;
using Gyldendal.Api.CoreData.Common.DataProviderInfrastructure;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.GqlToSolrConnector.SolrSearch;
using Gyldendal.Api.CoreData.SolrDataProviders.Infrastructure;
using Gyldendal.Api.CoreData.SolrDataProviders.Infrastructure.FilterGenerator;
using Gyldendal.Api.CoreData.SolrDataProviders.Infrastructure.Models;
using Gyldendal.Api.CoreData.SolrDataProviders.Mappings;
using SolrNet;

namespace Gyldendal.Api.CoreData.SolrDataProviders.WorkReview
{
    public class WorkReviewDataProvider : BaseDataProvider<WorkReviewFilterGenerationInput>, IWorkReviewDataProvider
    {
        private readonly ISearch<SolrContracts.WorkReview.WorkReview> _solrSearch;

        public WorkReviewDataProvider(string coreName, string solrServerUrl,
            IFilterGenerator<WorkReviewFilterGenerationInput> filterGenerator
            , IFilterInfoToSolrQueryBuilder solrQueryBuilder,
            ISearch<SolrContracts.WorkReview.WorkReview> solrSearch) : base(solrQueryBuilder, filterGenerator, solrServerUrl,
            coreName)
        {
            _solrSearch = solrSearch;
        }

        public List<CoreData.Contracts.Models.WorkReview> GetWorkReviewsByShop(DataScope dataScope)
        {
            var filters = GenerateSolrQuery(
                new WorkReviewFilterGenerationInput(webShops: new[] { dataScope.ToWebShop() }));

            var result = _solrSearch.Search(SolrQuery.All, 0, 0, null, null, null, filters);

            return result.ItemsFound.Select(x => x.ToCoreDataWorkReview()).ToList();
        }

        public List<Contracts.Models.WorkReview> GetWorkReviewsByWorkId(int workId, WebShop webShop)
        {
            var filterGenerationInput = new WorkReviewFilterGenerationInput { WorkIds = new[] { workId.ToString() } };
            if (webShop != WebShop.None)
                filterGenerationInput.WebShops = new List<WebShop> { webShop };
            var filters = GenerateSolrQuery(filterGenerationInput);

            var result = _solrSearch.Search(SolrQuery.All, 0, 0, null, null, null, filters);

            return result.ItemsFound.Select(x => x.ToCoreDataWorkReview()).ToList();
        }
    }
}