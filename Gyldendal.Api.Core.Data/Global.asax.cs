using System;
using Gyldendal.Api.CoreData.DependencyResolver;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SimpleInjector.Integration.WebApi;
using System.Configuration;
using System.Web.Http;
using Gyldendal.Api.CoreData.Common.Logging;
using Gyldendal.Api.CoreData.Filters;
// ReSharper disable RedundantToStringCall

namespace Gyldendal.Api.CoreData
{
    /// <summary>
    ///
    /// </summary>
    public class WebApiApplication : System.Web.HttpApplication
    {
        /// <summary>
        ///
        /// </summary>
        protected void Application_Start()
        {
            var config = GlobalConfiguration.Configuration;

            bool enableRequestLogging;
            Boolean.TryParse(ConfigurationManager.AppSettings["EnableIncomingRequestLogging"], out enableRequestLogging);
            if(enableRequestLogging)
                GlobalConfiguration.Configuration.Filters.Add(new RequestLoggingFilter());
            
            Ioc.SetupContainer();
            GlobalConfiguration.Configuration.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(Ioc.Container);

            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            var settings = config.Formatters.JsonFormatter.SerializerSettings;
            var dateConverter = new IsoDateTimeConverter
            {
                DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fff'Z'"
            };
            settings.Converters.Add(dateConverter);
        }

        /// <summary>
        ///
        /// </summary>
        protected void Application_End()
        {
            var logger = Ioc.Container.GetInstance<ILogger>();
            logger.ShutdownLogger();
        }
    }
}