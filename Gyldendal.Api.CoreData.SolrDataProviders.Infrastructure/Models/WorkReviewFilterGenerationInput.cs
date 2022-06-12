using System.Collections.Generic;
using Gyldendal.Api.CommonContracts;

namespace Gyldendal.Api.CoreData.SolrDataProviders.Infrastructure.Models
{
    public class WorkReviewFilterGenerationInput : FilterGenerationInput
    {
        public IEnumerable<string> WorkIds { get; set; }

        public WorkReviewFilterGenerationInput()
        {
        }

        public WorkReviewFilterGenerationInput(IEnumerable<string> workIds = null, IEnumerable<WebShop> webShops = null) : base(webShops)
        {
            WorkIds = workIds;
        }
    }
}