using System.Collections.Generic;
using Gyldendal.Api.CommonContracts;

namespace Gyldendal.Api.CoreData.SolrDataProviders.Infrastructure.Models
{
    public abstract class FilterGenerationInput
    {
        public IEnumerable<WebShop> WebShops { get; set; }

        protected FilterGenerationInput(IEnumerable<WebShop> webShops)
        {
            WebShops = webShops;
        }

        protected FilterGenerationInput()
        {
        }
    }
}