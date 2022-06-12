using System.Collections.Generic;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Contracts.Response;
using PorterApi = Gyldendal.Api.CoreData.Services.PorterApiClient;

namespace Gyldendal.Api.CoreData.Business.Porter.Interfaces.DataProviders
{
    public interface IWorkServiceDataProvider
    {
        GetProductDetailsResponse ProvideDataFor_GetWorkByProduct(Contracts.Models.Work work, string isbn);

        Contracts.Models.Work ProvideDataFor_GetWorkForBundleProduct(string bundleId, List<PorterApi.Product> bundleProducts, WebShop webShop);
        IEnumerable<string> GetBundleProductIsbnsFromKd(string pakkeId);
    }
}
