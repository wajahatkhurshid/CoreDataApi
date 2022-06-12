using Gyldendal.Api.CoreData.Contracts.Enumerations;

namespace Gyldendal.Api.CoreData.Contracts.Requests
{
    public class GetProductsByDataScopeRequest
    {
        public DataScope DataScope { get; set; }

        public ProductDataProfile ProductDataProfile { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }
}
