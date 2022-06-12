using System.Collections.Generic;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;

namespace Gyldendal.Api.CoreData.Contracts.Response
{
    public class GetScopeWorksByProductIdResponse
    {
        public GetScopeWorksByProductIdResponse()
        {
            Works = new List<Work>();
        }

        public string Message { get; set; }

        public List<Work> Works { get; set; }

        public ProductNotFoundReason ProductNotFoundReason { get; set; }
    }
}