using System.Web.Http.Filters;

namespace Gyldendal.Api.CoreData.Filters
{
    /// <summary>
    /// 
    /// </summary>
    public interface IExceptionFilter
    {
        /// <summary>
        /// On Exception, This method will be executed
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        void OnException(HttpActionExecutedContext actionExecutedContext);
    }
}
