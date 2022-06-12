using System.Collections.Generic;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Business.ExtensionMethods;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.ResultsPostProcessing.Infrastructure;
using GPlusWorkResultsProcessor = Gyldendal.Api.CoreData.ResultsPostProcessing.Processors.GPlus.WorkResultsProcessor;
using TradeGdkWorkResultsProcessor = Gyldendal.Api.CoreData.ResultsPostProcessing.Processors.TradeGdk.WorkResultsProcessor;

namespace Gyldendal.Api.CoreData.ResultsPostProcessing
{
    public class WorksResultFactory : IWorksResultFactory
    {
        public List<IWorkResultsProcessor> GetProcessors(WebShop webShop)
        {
            var processors = new List<IWorkResultsProcessor>();

            var gyldendalPlusWebShops = DataScope.GyldendalPlus.ToWebShops();

            if (gyldendalPlusWebShops.Contains(webShop) || webShop == WebShop.GyldendalDk || webShop == WebShop.TradeGyldendalDk)
            {
                processors.Add(new GPlusWorkResultsProcessor());
            }

            if (webShop == WebShop.GyldendalDk || webShop == WebShop.TradeGyldendalDk)
            {
                processors.Add(new TradeGdkWorkResultsProcessor());
            }

            return processors;
        }
    }
}