using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;

namespace Gyldendal.Api.CoreData.Contracts.Response
{
    public class GetProductDetailsResponse
    {
        public Work ProductWork { get; set; }

        public string Message { get; set; }

        public ProductNotFoundReason ProductNotFoundReason { get; set; }
    }
}