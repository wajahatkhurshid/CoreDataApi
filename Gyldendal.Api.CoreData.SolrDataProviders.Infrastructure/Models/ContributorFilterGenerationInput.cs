using System.Collections.Generic;
using Gyldendal.Api.CommonContracts;

namespace Gyldendal.Api.CoreData.SolrDataProviders.Infrastructure.Models
{
    public class ContributorFilterGenerationInput : FilterGenerationInput
    {
        public IEnumerable<string> ContributorIds { get; set; }

        public string SearchString { get; set; }

        public ContributorFilterGenerationInput(IEnumerable<string> contributorIds = null, string searchString = null,
            IEnumerable<WebShop> webShops = null) : base(webShops)
        {
            ContributorIds = contributorIds;
            SearchString = searchString;
        }
    }
}