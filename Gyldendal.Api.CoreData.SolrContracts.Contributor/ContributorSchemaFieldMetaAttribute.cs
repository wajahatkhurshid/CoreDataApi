using System;

namespace Gyldendal.Api.CoreData.SolrContracts.Contributor
{
    /// <inheritdoc />
    /// <summary>
    /// Supposed to be applied on the ContributorSchemaField enumeration members, provides fields to store metadata bout a Contributor field.
    /// </summary>
    public class ContributorSchemaFieldMetaAttribute : Attribute
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        // ReSharper disable once MemberCanBePrivate.Global
        /// <summary>
        /// The equivalent ContributorSchemaField enumeration member, for the Contributor field the attribute is applied on.
        /// </summary>
        public string SolrFieldName { get; private set; }

        /// <summary>
        /// Creates a new instance of ContributorSchemaFieldMetaAttribute.
        /// </summary>
        /// <param name="solrFieldName"></param>
        public ContributorSchemaFieldMetaAttribute(string solrFieldName)
        {
            SolrFieldName = solrFieldName;
        }
    }
}