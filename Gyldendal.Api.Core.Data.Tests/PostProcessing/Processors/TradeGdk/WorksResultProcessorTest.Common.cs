using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Response;
using Gyldendal.Api.CoreData.ResultsPostProcessing.Infrastructure;
using Gyldendal.Api.CoreData.ResultsPostProcessing.Processors.TradeGdk;
using System.Collections.Generic;

namespace Gyldendal.Api.CoreData.Tests.PostProcessing.Processors.TradeGdk
{
    public partial class WorksResultProcessorTest
    {
        private readonly IWorkResultsProcessor _workResultsProcessor;

        private SearchResponse<Work> _works;

        public WorksResultProcessorTest()
        {
            _workResultsProcessor = new WorkResultsProcessor();
        }

        private void DuplicateProductPopulateWorks()
        {
            _works = new SearchResponse<Work>
            {
                SearchResults = new Result<Work>
                {
                    Results = new List<Work>
                    {
                        new Work
                        {
                            WebShop = WebShop.TradeGyldendalDk,
                            Id = 123,
                            Products = new List<Product>
                            {
                                new Product
                                {
                                    Id = "9788700375789_2",
                                    Isbn13 = "9788700375789",
                                    WebShop = WebShop.Gu
                                },
                                new Product
                                {
                                    Id = "9788700375789_26",
                                    Isbn13 = "9788700375789",
                                    WebShop = WebShop.TradeGyldendalDk
                                }
                            }
                        }
                    }
                }
            };
        }

        private void NoDuplicateProductPopulateWorks()
        {
            _works = new SearchResponse<Work>
            {
                SearchResults = new Result<Work>
                {
                    Results = new List<Work>
                    {
                        new Work
                        {
                            WebShop = WebShop.TradeGyldendalDk,
                            Id = 123,
                            Products = new List<Product>
                            {
                                new Product
                                {
                                    Id = "9788700375789_2",
                                    Isbn13 = "9788700375789",
                                    WebShop = WebShop.Gu
                                },
                                new Product
                                {
                                    Id = "9788700375788_26",
                                    Isbn13 = "9788700375788",
                                    WebShop = WebShop.TradeGyldendalDk
                                }
                            }
                        }
                    }
                }
            };
        }

        private List<Work> GetWorks()
        {
            return _works.SearchResults.Results;
        }
    }
}