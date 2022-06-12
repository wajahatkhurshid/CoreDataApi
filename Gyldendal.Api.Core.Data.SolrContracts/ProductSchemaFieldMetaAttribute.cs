using System;

namespace Gyldendal.Api.CoreData.SolrContracts.Product
{
    /// <summary>
    /// Supposed to be applied on the ProductSchemaField enumeration members, provides fields to store metadata bout a Product field.
    /// </summary>
    public class ProductSchemaFieldMetaAttribute : Attribute
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        // ReSharper disable once MemberCanBePrivate.Global
        /// <summary>
        /// The equivalent ProductSchemaField enumeration member, for the Product field the attribute is applied on.
        /// </summary>
        public string SolrFieldName { get; private set; }

        /// <summary>
        /// Creates a new instance of ProductSchemaFieldMetaAttribute.
        /// </summary>
        /// <param name="solrFieldName"></param>
        public ProductSchemaFieldMetaAttribute(string solrFieldName)
        {
            SolrFieldName = solrFieldName;
        }
    }
}