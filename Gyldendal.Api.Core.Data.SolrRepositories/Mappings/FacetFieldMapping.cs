using System.Collections.Generic;
using System.Linq;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.SolrContracts.Product;
using NewRelic.Api.Agent;

namespace Gyldendal.Api.CoreData.SolrDataProviders.Mappings
{
    public class FacetFieldMapping
    {
        private static readonly Dictionary<string, FacetType> FacetTypeToFieldMapping = new Dictionary<string, FacetType>
        {
            { ProductSchemaField.SystemNames.GetFieldName(), FacetType.SystemName },
            { ProductSchemaField.SeriesNames.GetFieldName(), FacetType.SeriesName },
            { ProductSchemaField.MaterialTypeName.GetFieldName(), FacetType.MaterialTypeName },
            { ProductSchemaField.MediaTypeName.GetFieldName(), FacetType.MediaTypeName },
            { ProductSchemaField.Levels.GetFieldName(), FacetType.LevelName },
            { ProductSchemaField.SubjectWithAreaAndSubarea.GetFieldName(), FacetType.SubjectWithAreaAndSubarea },
            { ProductSchemaField.Subjects.GetFieldName(), FacetType.Subjects },
            { ProductSchemaField.Areas.GetFieldName(), FacetType.Areas },
            { ProductSchemaField.SubAreas.GetFieldName(), FacetType.SubAreas },
            { ProductSchemaField.Genres.GetFieldName(), FacetType.Genre },
            { ProductSchemaField.Categories.GetFieldName(), FacetType.Category},
            { ProductSchemaField.Subcategories.GetFieldName(), FacetType.SubCategory },
            { ProductSchemaField.AuthorNames.GetFieldName(), FacetType.AuthorName },
            { ProductSchemaField.AuthorIdWithName.GetFieldName(), FacetType.AuthorIdWithName },
            { ProductSchemaField.MaterialWithMediaTypeName.GetFieldName(), FacetType.MaterialWithMediaTypeName },
            { ProductSchemaField.HasTrialAccess.GetFieldName(), FacetType.HasTrialAccess }
        };

        private static FacetFieldMapping _instance;

        public static FacetFieldMapping Map => _instance ?? (_instance = new FacetFieldMapping());

        private FacetFieldMapping()
        {
        }

        [Trace]
        public static string[] GetFacetFieldNames(List<FacetType> facetTypes)
        {
            return FacetTypeToFieldMapping.Where(x => facetTypes.Contains(x.Value)).Select(x => x.Key).ToArray();
        }

        public FacetType this[string fieldName] => FacetTypeToFieldMapping[fieldName];

        public string[] FacetFieldNames => FacetTypeToFieldMapping.Select(x => x.Key).ToArray();
    }
}