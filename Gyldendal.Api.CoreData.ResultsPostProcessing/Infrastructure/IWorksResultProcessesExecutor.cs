using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Contracts.Response;

namespace Gyldendal.Api.CoreData.ResultsPostProcessing.Infrastructure
{
    public interface IWorksResultProcessesExecutor
    {
        void Execute(SearchResponse<Contracts.Models.Work> works, WebShop webShop);
    }
}
