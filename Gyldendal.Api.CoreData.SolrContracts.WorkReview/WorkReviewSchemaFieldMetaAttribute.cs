using System;

namespace Gyldendal.Api.CoreData.SolrContracts.WorkReview
{
    /// <inheritdoc />
    /// <summary>
    /// Supposed to be applied on the WorkReviewSchemaField enumeration members, provides fields to store metadata bout a WorkReview field.
    /// </summary>
    public class WorkReviewSchemaFieldMetaAttribute : Attribute
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        // ReSharper disable once MemberCanBePrivate.Global
        /// <summary>
        /// The equivalent WorkReviewSchemaField enumeration member, for the WorkReview field the attribute is applied on.
        /// </summary>
        public string SolrFieldName { get; private set; }

        /// <summary>
        /// Creates a new instance of WorkReviewSchemaFieldMetaAttribute.
        /// </summary>
        /// <param name="solrFieldName"></param>
        public WorkReviewSchemaFieldMetaAttribute(string solrFieldName)
        {
            SolrFieldName = solrFieldName;
        }
    }
}
