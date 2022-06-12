using Gyldendal.Api.CoreData.Common.Request;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.GqlToSolrConnector.Model;
using Gyldendal.Api.CoreData.SolrContracts.Product;
using Gyldendal.Api.CoreData.SolrDataProviders.Infrastructure.FilterGenerator;
using Gyldendal.Api.CoreData.SolrDataProviders.Infrastructure.Models;
using Gyldendal.Api.CoreData.SolrDataProviders.Mappings;
using System.Collections.Generic;
using System.Linq;

namespace Gyldendal.Api.CoreData.SolrDataProviders.Product
{
    /// <summary>
    /// Helper class to Create Filter Info based upon product filters
    /// </summary>
    public class ProductSolrFilterGenerator : BaseFilterGenerator<ProductFilterGenerationInput>, IFilterGenerator<ProductFilterGenerationInput>
    {
        protected override void DoGenerate()
        {
            GenerateWebShopFilters();

            GenerateSkipInvalidSaleConfigsFilter();

            GenerateIsbnsFilter();

            GenerateProductSearchTypeFilters();

            GenerateSearchStringFilter();

            GenerateContributorIdFilter();
        }

        private void GenerateContributorIdFilter()
        {
            if (!string.IsNullOrWhiteSpace(Input.ContributorId))
            {
                Filters.Add(GetSolrConnectorFilterInfo(ProductSchemaField.ContributorIds.GetFieldName(), new List<string> { Input.ContributorId }, false, false));
            }
        }

        private void GenerateSearchStringFilter()
        {
            if (!string.IsNullOrWhiteSpace(Input.SearchString))
            {
                Filters.Add(GetSolrConnectorFilterInfo(FilterFieldMapping.Map[WorkProductSearchFilter.Title], new List<string> { $"*{FilterInfo.QuoteString(Input.SearchString)}*" }, false, false));
            }
        }

        private void GenerateIsbnsFilter()
        {
            if (Input.Isbns == null || !Input.Isbns.Any()) return;

            Filters.Add(GetSolrConnectorFilterInfo(FilterFieldMapping.Map[WorkProductSearchFilter.Isbn], Input.Isbns, false));
        }

        private void GenerateSkipInvalidSaleConfigsFilter()
        {
            if (!Input.SkipInvalidSaleConfigProds) return;

            Filters.Add(GetSolrConnectorFilterInfo(ProductSchemaField.IsSaleConfigAvailable.GetFieldName(), new List<string> { "true" }, false, false));
        }

        private void GenerateProductSearchTypeFilters()
        {
            switch (Input.ProductSearchType)
            {
                case ProductSearchType.None:
                    return;

                case ProductSearchType.Bundle:
                    Filters.Add(GetSolrConnectorFilterInfo(FilterFieldMapping.Map[WorkProductSearchFilter.ProductType], new[] { ((int)ProductType.Bundle).ToString() }, false));
                    break;

                default:
                    Filters.Add(GetSolrConnectorFilterInfo(FilterFieldMapping.Map[WorkProductSearchFilter.ProductType], new[] { ((int)ProductType.SingleProduct).ToString() }, false));

                    Filters.Add(GetSolrConnectorFilterInfo(FilterFieldMapping.Map[WorkProductSearchFilter.IsPhysical], new[] { (Input.ProductSearchType == ProductSearchType.Physical).ToString() }, false));
                    break;
            }
        }

        private void GenerateWebShopFilters()
        {
            if (Input.WebShops == null || !Input.WebShops.Any()) return;

            Filters.Add(GetSolrConnectorFilterInfo(FilterFieldMapping.Map[WorkProductSearchFilter.WebShop], Input.WebShops.Select(x => ((int)x).ToString()), false));
        }
    }
}