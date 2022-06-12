using Gyldendal.Api.CoreData.Filters;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;

namespace Gyldendal.Api.CoreData.DependencyResolver
{
    /// <summary>
    /// Ioc container
    /// </summary>
    public static class Ioc
    {
        /// <summary>
        /// container of simple injector
        /// </summary>
        public static Container Container { get; }

        static Ioc()
        {
            Container = new Container();
        }

        /// <summary>
        /// Setup all interfaces
        /// </summary>
        public static void SetupContainer()
        {
            Container.Options.DefaultScopedLifestyle = new WebApiRequestLifestyle();

            Bootstrapping.Ioc.SetupContainer(Container);

            Container.Register<IExceptionFilter, ExceptionFilter>();

            Container.Verify();
        }
    }
}