using System;
using System.Linq;
using NewRelic.Api.Agent;

namespace Gyldendal.Api.CoreData.Gql.Common
{
    /// <summary>
    /// Utility class to support the class in the domain of common Gql functionality and infrastructure.
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Returns the quivalent GqlOperation enumeration member for the passed in token string.
        /// </summary>
        /// <param name="tokenString"></param>
        /// <returns></returns>
        [Trace]
        public static GqlOperation ConvertToGqlOperation(string tokenString)
        {
            foreach (var value in Enum.GetValues(typeof(GqlOperation)))
            {
                if (((GqlOperation)value).ToTokenString().Equals(tokenString, StringComparison.InvariantCultureIgnoreCase))
                {
                    return (GqlOperation)value;
                }
            }
            string[] postProcessingTokens =
            {
                "first", "oneperwork", "orderby", "top"
            };
            if (postProcessingTokens.Contains(tokenString))
            {
            }
            throw new ArgumentException(
                $"The value for the argument tokenString does not matches with any GqlOperation enumeration member. token: {tokenString}");
        }

        /// <summary>
        /// Returns the token string associated with the passed in GqlOperation enumeration member.
        /// </summary>
        /// <param name="gqlOperation"></param>
        /// <returns></returns>
        [Trace]
        public static string ToTokenString(this GqlOperation gqlOperation)
        {
            var memberInfo = typeof(GqlOperation).GetMember(gqlOperation.ToString()).FirstOrDefault();

            if (memberInfo == null)
            {
                throw new ArgumentException("Invalid value passed for the argument of enumeration type GqlOperation.");
            }

            var attribute = (GqlOperationMetaAttribute)memberInfo.GetCustomAttributes(typeof(GqlOperationMetaAttribute), false).FirstOrDefault();

            if (attribute == null)
            {
                throw new ArgumentException("The passd GqlOperation enumeration memeber as no GqlOperationMetaAttribute defined on it.");
            }

            return attribute.GqlTokenString;
        }
    }
}