using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Response;

namespace Gyldendal.Api.CoreData.ResultsPostProcessing.Infrastructure
{
    public interface IWorkResultsProcessor
    {
        void Process(SearchResponse<Work> works);
    }
}