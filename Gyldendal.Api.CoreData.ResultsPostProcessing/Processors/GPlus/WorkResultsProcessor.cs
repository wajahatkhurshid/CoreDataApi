using System;
using System.Collections.Generic;
using System.Linq;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Response;
using Gyldendal.Api.CoreData.ResultsPostProcessing.Infrastructure;

namespace Gyldendal.Api.CoreData.ResultsPostProcessing.Processors.GPlus
{
    public class WorkResultsProcessor : IWorkResultsProcessor
    {
        private const string NewLabel = "Nyhed";

        private const string UpcomingLabel = "Kommer snart";

        private readonly DateTime _today = DateTime.Now.Date;

        private readonly DateTime _newProductThreshold = DateTime.Now.AddDays(-30).Date;

        private readonly DateTime _upcomingProductThreshold = DateTime.Now.AddDays(30).Date;

        private SearchResponse<Work> _works;

        public void Process(SearchResponse<Work> works)
        {
            _works = works;

            DoProcess();
        }

        private void DoProcess()
        {
            foreach (var product in _works.SearchResults.Results.SelectMany(result => result.Products))
            {
                ProcessNewAndUpcomingLabel(product);
            }
        }

        private void ProcessNewAndUpcomingLabel(Product product)
        {
            if (product.PublishDate == null) return;

            // Product published before earlier than last 30 days or after next 30 days is not eligible for new/upcoming label
            if (product.PublishDate.Value.Date < _newProductThreshold || product.PublishDate.Value.Date > _upcomingProductThreshold)
            {
                return;
            }

            // A product is physical, or imprint with Gyldendal Stereo, or MediaType is EAN-vare, is eligible for the new or upcoming label.
            if (ProductIsPhysicalOrHasStereoImprint(product) || ProductMediaTypeIsEan(product))
            {
                ApplyLabel(product);
            }
        }

        private static bool ProductMediaTypeIsEan(Product product)
        {
            return product.MediaType?.Name?.Equals("EAN-vare") ?? false;
        }

        private static bool ProductIsPhysicalOrHasStereoImprint(Product product)
        {
            return product.IsPhysical || (product.Imprint?.Equals("Gyldendal Stereo") ?? false);
        }

        private void ApplyLabel(Product product)
        {
            string label;

            // ReSharper disable once PossibleInvalidOperationException
            // Checking whether the product is published in past 30 days which makes it qualify for New product
            if (product.PublishDate.Value.Date >= _newProductThreshold && product.PublishDate.Value.Date <= _today)
            {
                label = NewLabel;
            }
            else // Product is upcoming as it is going to be published in next 30 days
            {
                label = UpcomingLabel;
            }

            if (product.Labels == null)
            {
                product.Labels = new List<string> { label };
            }
            else if (!product.Labels.Contains(label)) // Checking if already have a label which is manually added directly in HY.
            {
                product.Labels.Add(label);
            }
        }
    }
}