using System.Linq;
using Gyldendal.Api.CoreData.SolrContracts.WorkReview;
using Gyldendal.Api.CoreData.SolrDataProviders.Infrastructure.FilterGenerator;
using Gyldendal.Api.CoreData.SolrDataProviders.Infrastructure.Models;

namespace Gyldendal.Api.CoreData.SolrDataProviders.WorkReview
{
    /// <summary>
    /// Helper class to Create Filter Info based upon WorkReview filters
    /// </summary>
    public class WorkReviewSolrFilterGenerator : BaseFilterGenerator<WorkReviewFilterGenerationInput>, IFilterGenerator<WorkReviewFilterGenerationInput>
    {
        protected override void DoGenerate()
        {
            GenerateWebShopFilters();

            GenerateWorkIdsFilter();
        }

        private void GenerateWorkIdsFilter()
        {
            if (Input.WorkIds == null || !Input.WorkIds.Any()) return;

            Filters.Add(GetSolrConnectorFilterInfo(WorkReviewSchemaField.WorkId.GetFieldName(), Input.WorkIds, false));
        }

        private void GenerateWebShopFilters()
        {
            if (Input.WebShops == null || !Input.WebShops.Any()) return;
            var webShopFilters = Input.WebShops.Select(x =>
                GetSolrConnectorFilterInfo(WorkReviewSchemaField.WebsiteId.GetFieldName(),
                    new[] { x.ToString("D") }, false, false));

            Filters.AddRange(webShopFilters);
        }
    }
}