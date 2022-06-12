using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Response;
using Gyldendal.Api.CoreData.ResultsPostProcessing.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace Gyldendal.Api.CoreData.ResultsPostProcessing.Processors.TradeGdk
{
    public class WorkResultsProcessor : IWorkResultsProcessor
    {
        private List<Work> _works;

        public void Process(SearchResponse<Work> works)
        {
            _works = works.SearchResults.Results;
            DoProcess();
        }

        private void DoProcess()
        {
            if (AllWorksHaveTradeProducts())
                return;
            ProcessWorks();
        }

        private void ProcessWorks()
        {
            foreach (var work in _works.Where(x => x.Products.Any(c => !(IsGdkWebShop(c.WebShop)))
                                                   && x.Products.Any(c => IsGdkWebShop(c.WebShop))))
            {
                RemoveDuplicateNonTradeProducts(work);
            }
        }

        private static void RemoveDuplicateNonTradeProducts(Work work)
        {
            var groupByIsbnAndCount = work.Products.GroupBy(c => c.Isbn13)
                .Select(v => new { Count = v.Count(), Isbn13 = v.Key }).ToList();

            if (!(groupByIsbnAndCount.Any(c => c.Count > 1))) return;

            foreach (var duplicate in groupByIsbnAndCount.Where(c => c.Count > 1))
            {
                var productsToRemove = work.Products.Where(c => c.Isbn13 == duplicate.Isbn13).ToList();
                if (productsToRemove.Any(c => IsGdkWebShop(c.WebShop)))
                    productsToRemove.Where(c => !(IsGdkWebShop(c.WebShop))).ToList().ForEach(p => work.Products.Remove(p));
            }
        }

        private static bool IsGdkWebShop(WebShop webShop)
        {
            return webShop == WebShop.GyldendalDk || webShop == WebShop.TradeGyldendalDk;
        }

        private bool AllWorksHaveTradeProducts()
        {
            return _works.SelectMany(x => x.Products).All(x => IsGdkWebShop(x.WebShop));
        }
    }
}