using System;

namespace Gyldendal.Api.CoreData.Gql.Common
{
    public class GqlOperationMetaAttribute : Attribute
    {
        public string GqlTokenString { get; private set; }

        public GqlOperationMetaAttribute(string gqlTokenString)
        {
            GqlTokenString = gqlTokenString;
        }
    }
}
