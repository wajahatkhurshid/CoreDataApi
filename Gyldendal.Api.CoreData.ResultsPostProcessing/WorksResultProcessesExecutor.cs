using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Response;
using Gyldendal.Api.CoreData.ResultsPostProcessing.Infrastructure;

namespace Gyldendal.Api.CoreData.ResultsPostProcessing
{
    public class WorksResultProcessesExecutor : IWorksResultProcessesExecutor
    {
        private readonly IWorksResultFactory _worksResultFactory;

        public WorksResultProcessesExecutor(IWorksResultFactory worksResultFactory)
        {
            _worksResultFactory = worksResultFactory;
        }

        public void Execute(SearchResponse<Work> works, WebShop webShop)
        {
            var processors = _worksResultFactory.GetProcessors(webShop);

            foreach (var processor in processors)
            {
                processor.Process(works);
            }
        }
    }
}
