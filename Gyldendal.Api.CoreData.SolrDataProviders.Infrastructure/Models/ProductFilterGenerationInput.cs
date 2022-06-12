using System.Collections.Generic;
using Gyldendal.Api.CoreData.Contracts.Enumerations;

namespace Gyldendal.Api.CoreData.SolrDataProviders.Infrastructure.Models
{
    public class ProductFilterGenerationInput : FilterGenerationInput
    {
        public IEnumerable<string> Isbns { get; set; }

        public string SearchString { get; set; }

        public ProductSearchType ProductSearchType { get; set; }

        public bool SkipInvalidSaleConfigProds { get; set; }

        public string ContributorId { get; set; }
    }
}