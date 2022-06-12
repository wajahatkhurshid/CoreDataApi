// ReSharper disable UnusedAutoPropertyAccessor.Global

using Gyldendal.Api.CoreData.Contracts.Enumerations;
using System;

namespace Gyldendal.Api.CoreData.Contracts.Models
{
    public class ProductPrice
    {
        public DateTime? ValidTo { get; set; }

        public DateTime? ValidFrom { get; set; }

        public decimal PriceWithVat { get; set; }

        public decimal PriceWithoutVat { get; set; }

        public decimal? DiscountPercentage { get; set; }

        public decimal? DiscountedPriceWithVat { get; set; }

        public decimal? DiscountedPriceWithoutVat { get; set; }

        public decimal VatPercentage { get; set; }

        public ProductPriceType PriceType { get; set; }
    }
}