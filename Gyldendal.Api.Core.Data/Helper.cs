using System.Collections.Generic;
using Gyldendal.Api.CoreData.Gql.Common;
using Gyldendal.Api.CoreData.SolrContracts;

namespace Gyldendal.Api.CoreData
{
    /// <summary>
    /// Helper class to provide stattic methods required in this layer
    /// </summary>
    public class Helper
    {
        /// <summary>
        /// GQL to Solr file mapping
        /// </summary>
        /// <returns></returns>
        internal static Dictionary<GqlOperation, string> GetGqlToSolrFieldMapping()
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
                { GqlOperation.Work, ProductSchemaField.WorkId.GetFieldName() }

            };

            return retVal;
        }
    }
}