using Gyldendal.Api.CommonContracts;

namespace Gyldendal.Api.CoreData.Contracts.Requests
{
    public class GetProductRequestV2
    {
        /// <summary>
        /// The web shops whose products to be searched
        /// </summary>
        public WebShop[] WebShops { get; set; }

        /// <summary>
        /// Isbns of the products to be searched
        /// </summary>
        public string[] Isbns { get; set; }

        /// <summary>
        /// Flag to skip products with invalid saleConfiguration
        /// </summary>
        public bool SkipInvalidSaleConfigProds { get; set; }
    }
}