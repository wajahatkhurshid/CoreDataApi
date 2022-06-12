using Gyldendal.Api.CoreData.Gql.Common;

namespace Gyldendal.Api.CoreData.GqlToSolrConnector.Model
{
    public class SearchCriteria
    {
        public string FieldName { get; set; }

        public string LowerLimit { get; set; }

        public string UpperLimit { get; set; }

        public string Value { get; set; }

        public GqlOperation GqlOperation { get; set; }
    }
}