using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Gyldendal.Api.CoreData.Common;
using Gyldendal.Common.WebUtils.Attributes;
using Gyldendal.Common.WebUtils.Exceptions;
using NewRelic.Api.Agent;

namespace Gyldendal.Api.CoreData.Filters
{
    /// <summary>
    /// if action input value is null, throw exception
    /// in case of optional parameters null can be pass 
    /// </summary>
    public class NullValueFilter : ActionFilterAttribute, INullValueFilter
    {
        /// <inheritdoc />
        /// Generate error if action arguments are null
        /// ignore nullable parameters
        [Trace]
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var optionalParameters = actionContext.ActionDescriptor.GetCustomAttributes<OptionalParameterAttribute>().FirstOrDefault();
            var parameters = actionContext.ActionDescriptor.GetParameters();
            foreach (var param in parameters)
            {
                // ignore null value validation if its optional (works for primitive types)
                if (param.IsOptional)
                    continue;

                // ignore null value validation if it is define OptionalParameter attribute (works for non-primitive types)
                if (optionalParameters != null && optionalParameters.OptionalParameters.Contains(param.ParameterName))
                    continue;

                object value = null;

                if (actionContext.ActionArguments.ContainsKey(param.ParameterName))
                    value = actionContext.ActionArguments[param.ParameterName];

                if (value == null)
                    throw new ProcessException((ulong)ErrorCodes.NullValue, ErrorCodes.NullValue.GetDescription(), Extensions.CoreDataSystemName);
            }
        }
    }
}