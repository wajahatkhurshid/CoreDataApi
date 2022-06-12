using System.Collections.Generic;
using System.Linq;
using Gyldendal.Api.CoreData.GqlToSolrConnector.Model;
using Gyldendal.Api.CoreData.SolrContracts.Contributor;
using Gyldendal.Api.CoreData.SolrDataProviders.Infrastructure.FilterGenerator;
using Gyldendal.Api.CoreData.SolrDataProviders.Infrastructure.Models;

namespace Gyldendal.Api.CoreData.SolrDataProviders.Contributor
{
    /// <summary>
    /// Helper class to Create Filter Info based upon Contributor filters
    /// </summary>
    public class ContributorSolrFilterGenerator : BaseFilterGenerator<ContributorFilterGenerationInput>, IFilterGenerator<ContributorFilterGenerationInput>
    {
        protected override void DoGenerate()
        {
            GenerateWebShopFilters();

            GenerateContributorIdsFilter();

            GenerateSearchStringFilters();
        }

        private void GenerateContributorIdsFilter()
        {
            if (Input.ContributorIds == null || !Input.ContributorIds.Any()) return;

            Filters.Add(GetSolrConnectorFilterInfo(ContributorSchemaField.ContributorId.GetFieldName(), Input.ContributorIds, false));
        }

        private void GenerateSearchStringFilters()
        {
            if (string.IsNullOrWhiteSpace(Input.SearchString)) return;
            Filters.Add(GetSolrConnectorFilterInfo(ContributorSchemaField.ContributorName.GetFieldName(),
                new List<string> { $"*{FilterInfo.QuoteString(Input.SearchString)}*" }, false, false, new[]
                {
                    GetSolrConnectorFilterInfo(ContributorSchemaField.SearchName.GetFieldName(),
                        new List<string> {$"{FilterInfo.QuoteString(Input.SearchString)}"}, false, false)
                }));
        }

        private void GenerateWebShopFilters()
        {
            if (Input.WebShops == null || !Input.WebShops.Any()) return;

            var filter = GetSolrConnectorFilterInfo(
                ContributorSchemaField.WebsiteId.GetFieldName(),
                Input.WebShops.Select(x => x.ToString("D")).ToArray(),
                false,
                false
            );

            Filters.Add(filter);
        }
    }
}