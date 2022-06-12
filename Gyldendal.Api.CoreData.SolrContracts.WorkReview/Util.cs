using System;
using System.Linq;

namespace Gyldendal.Api.CoreData.SolrContracts.WorkReview
{
    public static class Util
    {
        /// <summary>
        /// Gets the Solr schmea field name, against the provided WorkReviewSchemaField enumeration member.
        /// </summary>
        /// <param name="schemaField"></param>
        /// <returns></returns>
        public static string GetFieldName(this WorkReviewSchemaField schemaField)
        {
            var memberInfo = typeof(WorkReviewSchemaField).GetMember(schemaField.ToString()).FirstOrDefault();

            if (memberInfo == null)
            {
                throw new ArgumentException("Invalid value passed for the argument of enumeration type WorkReviewSchemaField.");
            }

            var attribute = (WorkReviewSchemaFieldMetaAttribute)memberInfo.GetCustomAttributes(typeof(WorkReviewSchemaFieldMetaAttribute), false).FirstOrDefault();

            if (attribute == null)
            {
                throw new ArgumentException("The passd WorkReviewSchemaField enumeration memeber as no WorkReviewSchemaFieldMetaAttribute defined on it.");
            }

            return attribute.SolrFieldName;
        }
    }
}
