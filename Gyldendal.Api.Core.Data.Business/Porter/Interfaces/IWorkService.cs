using System.Threading.Tasks;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using CoreDataResponses = Gyldendal.Api.CoreData.Contracts.Response;
using Gyldendal.Api.CoreData.Services.PorterApiClient;
using ProductType = Gyldendal.Api.CoreData.Contracts.Enumerations.ProductType;

namespace Gyldendal.Api.CoreData.Business.Porter.Interfaces
{
    public interface IWorkService
    {
        Task<CoreDataResponses.GetScopeWorksByProductIdResponse> GetScopeWorksByProductIdAsync(DataScope dataScope, string isbn);

        Task<CoreDataResponses.GetProductDetailsResponse> GetWorkByProductIdAsync(CommonContracts.WebShop webShop, ProductType productType, string isbn);
    }
}
