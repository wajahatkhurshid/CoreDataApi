// ReSharper disable UnusedAutoPropertyAccessor.Global

using System.Collections.Generic;

namespace Gyldendal.Api.CoreData.Contracts.Models
{
    /// <summary>
    /// Class Pricing Information.
    /// </summary>
    public class PricingInfo
    {
        public ProductPrice DefaultPrice { get; set; }

        public DiscountOfferInfo DiscountOfferInfo { get; set; }

        public List<ProductPrice> AvailablePrices { get; set; }
    }
}