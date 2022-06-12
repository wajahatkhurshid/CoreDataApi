using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Business.Porter.Interfaces.DataProviders;

namespace Gyldendal.Api.CoreData.Business.Porter.Interfaces
{
    public interface IServiceDataProviderFactory
    {
        IProductServiceDataProvider GetProductServiceDataProvider(WebShop webShop);

        IWorkServiceDataProvider GetWorkServiceDataProvider(WebShop webShop);

        IContributorServiceDataProvider GetContributorServiceDataProvider(WebShop webShop);
    }
}
