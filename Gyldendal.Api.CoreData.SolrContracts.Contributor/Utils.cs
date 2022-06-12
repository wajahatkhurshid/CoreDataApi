using System;
using System.Linq;

namespace Gyldendal.Api.CoreData.SolrContracts.Contributor
{
    /// <summary>
    /// Utility methods supporting the contract classes.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Gets the Solr schmea field name, against the provided ContributorSchemaField enumeration member.
        /// </summary>
        /// <param name="schemaField"></param>
        /// <returns></returns>
        public static string GetFieldName(this ContributorSchemaField schemaField)
        {
            var memberInfo = typeof(ContributorSchemaField).GetMember(schemaField.ToString()).FirstOrDefault();

            if (memberInfo == null)
            {
                throw new ArgumentException("Invalid value passed for the argument of enumeration type ContributorSchemaField.");
            }

            var attribute = (ContributorSchemaFieldMetaAttribute)memberInfo.GetCustomAttributes(typeof(ContributorSchemaFieldMetaAttribute), false).FirstOrDefault();

            if (attribute == null)
            {
                throw new ArgumentException("The passd ContributorSchemaField enumeration memeber as no ContributorSchemaFieldMetaAttribute defined on it.");
            }

            return attribute.SolrFieldName;
        }
    }
}