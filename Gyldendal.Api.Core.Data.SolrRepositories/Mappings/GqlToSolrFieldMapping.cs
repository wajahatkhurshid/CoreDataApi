using System.Collections.Generic;
using Gyldendal.Api.CoreData.Gql.Common;
using Gyldendal.Api.CoreData.SolrContracts.Product;

namespace Gyldendal.Api.CoreData.SolrDataProviders.Mappings
{
    public class GqlToSolrFieldMapping
    {
        /// <summary>
        /// Returns the mappings dictionary from GqlOperation to respective Solr field name.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<GqlOperation, string> GetMappings()
        {
            var retVal = new Dictionary<GqlOperation, string>
            {
                { GqlOperation.Thema, ProductSchemaField.ThemaCodes.GetFieldName() },
                { GqlOperation.Author, ProductSchemaField.AuthorNames.GetFieldName() },
                { GqlOperation.Title, ProductSchemaField.Title.GetFieldName() },
                { GqlOperation.Isbn, ProductSchemaField.Isbn13.GetFieldName() },
                { GqlOperation.Publisher, ProductSchemaField.Publisher.GetFieldName() },
                { GqlOperation.InSeries, ProductSchemaField.SeriesIds.GetFieldName() },
                { GqlOperation.PublicationDate, ProductSchemaField.PublishDate.GetFieldName() },
                { GqlOperation.MediaType, ProductSchemaField.MediaTypeName.GetFieldName() },
                { GqlOperation.MaterialType, ProductSchemaField.MaterialTypeName.GetFieldName() },
                { GqlOperation.Area,  ProductSchemaField.Areas.GetFieldName() },
                { GqlOperation.SubArea,  ProductSchemaField.SubAreas.GetFieldName() },
                { GqlOperation.Subject, ProductSchemaField.Subjects.GetFieldName() },
                { GqlOperation.Level, ProductSchemaField.Levels.GetFieldName() },
                { GqlOperation.ContributorId, ProductSchemaField.ContributorIds.GetFieldName() },
                { GqlOperation.ProductId, ProductSchemaField.ProductId.GetFieldName() },
                { GqlOperation.Work, ProductSchemaField.WorkId.GetFieldName() },
                { GqlOperation.MainCategory, ProductSchemaField.CategoriesDa.GetFieldName() },
                { GqlOperation.SubCategory, ProductSchemaField.SubcategoriesDa.GetFieldName() },
                { GqlOperation.Label, ProductSchemaField.Labels.GetFieldName() },
                { GqlOperation.Imprint, ProductSchemaField.Imprint.GetFieldName() },
                { GqlOperation.IsPhysical, ProductSchemaField.IsPhysical.GetFieldName() },
                { GqlOperation.PriceRange, ProductSchemaField.DefaultPrice.GetFieldName() },
                { GqlOperation.Webshop, ProductSchemaField.WebsiteId.GetFieldName() },
                { GqlOperation.HasTrialAccess, ProductSchemaField.HasTrialAccess.GetFieldName() },
            };

            return retVal;
        }
    }
}