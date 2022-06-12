using System.Collections.Generic;
using System.Linq;
using Gyldendal.Api.CoreData.Common.Request;
using Gyldendal.Api.CoreData.SolrContracts.Product;

namespace Gyldendal.Api.CoreData.SolrDataProviders.Mappings
{
    public class FilterFieldMapping
    {
        private static readonly Dictionary<string, WorkProductSearchFilter> FilterTypeToFieldMapping
            = new Dictionary<string, WorkProductSearchFilter>
            {
                {ProductSchemaField.SystemNames.GetFieldName(), WorkProductSearchFilter.SystemName},
                {ProductSchemaField.SeriesNames.GetFieldName(), WorkProductSearchFilter.SeriesName},
                {ProductSchemaField.MaterialTypeName.GetFieldName(), WorkProductSearchFilter.MaterialTypeName},
                {ProductSchemaField.MediaTypeName.GetFieldName(), WorkProductSearchFilter.MediaTypeName},
                {ProductSchemaField.Levels.GetFieldName(), WorkProductSearchFilter.LevelName},
                {ProductSchemaField.Subjects.GetFieldName(), WorkProductSearchFilter.Subjects},
                {ProductSchemaField.Areas.GetFieldName(), WorkProductSearchFilter.Areas},
                {ProductSchemaField.SubAreas.GetFieldName(), WorkProductSearchFilter.SubAreas},
                {ProductSchemaField.Isbn13.GetFieldName(), WorkProductSearchFilter.Isbn},
                {ProductSchemaField.WebsiteId.GetFieldName(), WorkProductSearchFilter.WebShop},
                {ProductSchemaField.TitleContains.GetFieldName(), WorkProductSearchFilter.Title},
                {ProductSchemaField.IsPhysical.GetFieldName(), WorkProductSearchFilter.IsPhysical},
                {ProductSchemaField.ProductType.GetFieldName(), WorkProductSearchFilter.ProductType},
                {ProductSchemaField.Genres.GetFieldName(), WorkProductSearchFilter.Genre},
                {ProductSchemaField.Categories.GetFieldName(), WorkProductSearchFilter.Category},
                {ProductSchemaField.Subcategories.GetFieldName(), WorkProductSearchFilter.SubCategory},
                {ProductSchemaField.AuthorNames.GetFieldName(), WorkProductSearchFilter.AuthorName},
                {ProductSchemaField.ContributorIds.GetFieldName(), WorkProductSearchFilter.AuthorId}
            };

        private static FilterFieldMapping _instance;

        public static FilterFieldMapping Map => _instance ?? (_instance = new FilterFieldMapping());

        private FilterFieldMapping()
        {
        }

        public WorkProductSearchFilter this[string fieldName] => FilterTypeToFieldMapping[fieldName];

        public string this[WorkProductSearchFilter filterType] => FilterTypeToFieldMapping.FirstOrDefault(x => x.Value == filterType).Key;

        public string[] FilterFieldNames => FilterTypeToFieldMapping.Select(x => x.Key).ToArray();
    }
}