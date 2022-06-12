using System;
using System.Linq;

namespace Gyldendal.Api.CoreData.SolrContracts.ClubPeriod
{
    /// <summary>
    /// Utility methods supporting the contract classes.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Gets the Solr schmea field name, against the provided ClubPeriodSchemaField enumeration member.
        /// </summary>
        /// <param name="schemaField"></param>
        /// <returns></returns>
        public static string GetFieldName(this ClubPeriodSchemaField schemaField)
        {
            var memberInfo = typeof(ClubPeriodSchemaField).GetMember(schemaField.ToString()).FirstOrDefault();

            if (memberInfo == null)
            {
                throw new ArgumentException("Invalid value passed for the argument of enumeration type ContributorSchemaField.");
            }

            var attribute = (ClubPeriodSchemaFieldMetaAttribute)memberInfo.GetCustomAttributes(typeof(ClubPeriodSchemaFieldMetaAttribute), false).FirstOrDefault();

            if (attribute == null)
            {
                throw new ArgumentException("The passd ContributorSchemaField enumeration memeber as no ContributorSchemaFieldMetaAttribute defined on it.");
            }

            return attribute.SolrFieldName;
        }
    }
}