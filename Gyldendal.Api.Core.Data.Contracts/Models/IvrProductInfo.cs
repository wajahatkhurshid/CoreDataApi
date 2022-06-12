using System.Collections.Generic;
using Gyldendal.Api.CommonContracts;

namespace Gyldendal.Api.CoreData.Contracts.Models
{
    public class IvrProductInfo : BaseProductDataProfile
    {
        /// <summary>
        /// Product Id (Article Id in context of B2C Products).
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// Isbn13.
        /// </summary>
        public string Isbn13 { get; set; }

        /// <summary>
        /// List of WebShops a product exists in.
        /// </summary>
        public List<WebShop> WebShops { get; set; }
    }
}
