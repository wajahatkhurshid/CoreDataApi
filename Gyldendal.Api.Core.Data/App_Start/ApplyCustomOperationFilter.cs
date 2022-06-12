using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Description;
using Swashbuckle.Swagger;

namespace Gyldendal.Api.CoreData.App_Start
{
    public class ApplyCustomOperationFilter : IOperationFilter
    {
        public void Apply(
            Operation operation,
            SchemaRegistry schemaRegistry,
            ApiDescription apiDescription)
        {
            if (operation.parameters != null && apiDescription.RelativePath != null)
            {
                var splitOperation = operation.operationId.Split('_');
                operation.operationId = $"{apiDescription.HttpMethod}_{apiDescription.RelativePath.Split('/')[1]}_{splitOperation.Last()}_{Convert.ToBase64String(Guid.NewGuid().ToByteArray())}";

            }
        }
    }
}