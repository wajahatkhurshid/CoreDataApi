using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Contracts.Enumerations;

namespace Gyldendal.Api.CoreData.GqlToSolrConnector.Model
{
    public class OrderByParam
    {
        public SortBy Direction { get; set; }

        public string FieldName { get; set; }
    }
}