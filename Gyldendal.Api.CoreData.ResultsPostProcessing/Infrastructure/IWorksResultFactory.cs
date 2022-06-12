using System.Collections.Generic;
using Gyldendal.Api.CommonContracts;

namespace Gyldendal.Api.CoreData.ResultsPostProcessing.Infrastructure
{
    public interface IWorksResultFactory
    {
        List<IWorkResultsProcessor> GetProcessors(WebShop webShop);
    }
}