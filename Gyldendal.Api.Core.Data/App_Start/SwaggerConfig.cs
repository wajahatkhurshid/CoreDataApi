using Gyldendal.Api.CoreData;
using Swashbuckle.Application;
using System.Globalization;
using System.Web.Http;
using Gyldendal.Api.CoreData.App_Start;
using WebActivatorEx;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace Gyldendal.Api.CoreData
{
    /// <summary>
    /// Swagger configuration
    /// </summary>
    public class SwaggerConfig
    {
        /// <summary>
        /// Registers this instance.
        /// </summary>
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;
            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                {
                    // Use "SingleApiVersion" to describe a single version API. Swagger 2.0 includes an "Info" object to
                    // hold additional metadata for an API. Version and title are required but you can also provide
                    // additional fields by chaining methods off SingleApiVersion.
                    //
                    //c.SingleApiVersion("v1", "Gyldendal API Core Data");
                    c.DescribeAllEnumsAsStrings();
                    c.MultipleApiVersions((apiDesc, version) =>
                    {
                        var path = apiDesc.RelativePath.Split('/');

                        var pathVersion = path[1];
                        if ((!string.IsNullOrWhiteSpace(pathVersion)) &&
                            (pathVersion.Length != 2 ||
                             (pathVersion.Length == 2 && pathVersion[0] != 'v' && !char.IsDigit(pathVersion[1]))))
                        {
                            pathVersion = "v";
                        }

                        return
                            CultureInfo.InvariantCulture.CompareInfo.IndexOf(pathVersion, version,
                                CompareOptions.IgnoreCase) >= 0;
                    }, vc =>
                    {
                        vc.Version("v1", $"{thisAssembly.GetName().Name} {thisAssembly.GetName().Version}");
                        vc.Version("v2", $"{thisAssembly.GetName().Name} {thisAssembly.GetName().Version}"); //add this line when v2 is released
                        vc.Version("v3", $"{thisAssembly.GetName().Name} {thisAssembly.GetName().Version}"); //add this line when v3 is released
                    });

                    // Set this flag to omit descriptions for any actions decorated with the Obsolete attribute
                    c.UseFullTypeNameInSchemaIds();
                    c.IgnoreObsoleteActions();

                    // If you annotate Controllers and API Types with
                    // Xml comments (http://msdn.microsoft.com/en-us/library/b2s063f7(v=vs.110).aspx), you can incorporate
                    // those comments into the generated docs and UI. You can enable this by providing the path to one or
                    // more Xml comment files.
                    //
                    c.IncludeXmlComments(GetXmlCommentsPath());
                    c.OperationFilter<ApplyCustomOperationFilter>();
                }).EnableSwaggerUi(c =>
                {
                    c.DocumentTitle("Core Data Api");
                    c.EnableDiscoveryUrlSelector();
                    c.DisableValidator();
                });
        }

        private static string GetXmlCommentsPath()
        {
            return $@"{System.AppDomain.CurrentDomain.BaseDirectory}\bin\Gyldendal.Api.CoreData.XML";
        }
    }
}