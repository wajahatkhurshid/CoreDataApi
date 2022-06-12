using System.Net.Http.Formatting;
using System.Web.Http;
using Gyldendal.Api.CoreData.Filters;
using Newtonsoft.Json;

namespace Gyldendal.Api.CoreData
{
    /// <summary>
    /// Web Api Configuration
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="config"></param>
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            GlobalConfiguration.Configuration.Formatters.Clear();
            GlobalConfiguration.Configuration.Formatters.Add(new JsonMediaTypeFormatter());
            config.Formatters.JsonFormatter.SerializerSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore };

            var exceptionFilter = (ExceptionFilter)config.DependencyResolver.GetService(typeof(ExceptionFilter));
            config.Filters.Add(exceptionFilter);
        }
    }
}