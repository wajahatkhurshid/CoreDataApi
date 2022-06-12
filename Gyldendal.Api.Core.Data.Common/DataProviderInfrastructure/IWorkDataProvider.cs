using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Common.Request;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Response;

namespace Gyldendal.Api.CoreData.Common.DataProviderInfrastructure
{
    public interface IWorkDataProvider : IDataProvider<SearchResponse<Work>, WorkProductSearchRequest>
    {
        Work GetWorkById(WebShop[] webShops, int workId);
    }
}