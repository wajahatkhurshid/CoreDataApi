using System;
using System.Collections.Generic;
using System.Linq;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Response;
using Gyldendal.Api.CoreData.ResultsPostProcessing.Infrastructure;
using Gyldendal.Api.CoreData.ResultsPostProcessing.Processors.GPlus;

namespace Gyldendal.Api.CoreData.Tests.PostProcessing.Processors.GPlus
{
    public partial class WorksResultProcessorTest
    {
        private const string NyheadLabel = "Nyhed";

        private const string KommerSnartLabel = "Kommer snart";

        private readonly IWorkResultsProcessor _workResultsProcessor;

        private SearchResponse<Work> _works;

        public WorksResultProcessorTest()
        {
            _workResultsProcessor = new WorkResultsProcessor();
        }

        private void PopulateWorks(string id, DateTime? publishDate, bool physicalProduct, bool isStereoImprint = false, List<string> labels = null)
        {
            if (labels == null)
            {
                labels = new List<string>();
            }

            _works = new SearchResponse<Work>
            {
                SearchResults = new Result<Work>
                {
                    Results = new List<Work>
                    {
                        new Work
                        {
                            WebShop = WebShop.ClubBogklub,
                            Products = new List<Product>
                            {
                                new Product
                                {
                                    Id = id,
                                    PublishDate = publishDate,
                                    IsPhysical = physicalProduct,
                                    Labels = labels,
                                    Imprint = isStereoImprint ? "Gyldendal Stereo" : null
                                }
                            }
                        }
                    }
                }
            };
        }

        private Product GetProductById(string productId)
        {
            var product = _works?.SearchResults?.Results?.SelectMany(result => result.Products)
                .Where(prod => prod.Id.Equals(productId)).Select(prod => prod).SingleOrDefault();

            return product;
        }
    }
}