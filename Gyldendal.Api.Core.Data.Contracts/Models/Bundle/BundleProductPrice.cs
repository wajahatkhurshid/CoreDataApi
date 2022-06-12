using Gyldendal.AccessServices.Contracts.Enumerations;
using Gyldendal.Api.ShopServices.Contracts.SalesConfiguration;

namespace Gyldendal.Api.CoreData.Contracts.Models.Bundle
{
    public class BundleProductPrice
    {
        /// <summary>
        /// AccessForm Code of Product.
        /// </summary>
        public EnumAccessForm AccessFormCode { get; set; }

        /// <summary>
        /// Display Name for Period
        /// </summary>
        public string PeriodDisplayName { get; set; }

        /// <summary>
        /// Billing Period Type Code.
        /// </summary>
        public Enums.EnumPeriodType PeriodTypeCode { get; set; }

        /// <summary>
        /// Billing Period reference unit type code.
        /// </summary>
        public EnumPeriodUnitType PeriodUnitTypeCode { get; set; }

        /// <summary>
        /// Billing Period reference unit value.
        /// </summary>
        public int PeriodUnitValue { get; set; }

        /// <summary>
        /// Original price of Bundle product.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Original price of Bundle product with vat.
        /// </summary>
        public decimal PriceWithVat { get; set; }

        /// <summary>
        /// Discounted price of Bundle product.
        /// </summary>
        public decimal DiscountedPrice { get; set; }

        /// <summary>
        /// Discounted price of Bundle product with vat.
        /// </summary>
        public decimal DiscountedPriceWithVat { get; set; }

        /// <summary>
        /// Discount percentage on Bundle.
        /// </summary>
        public decimal DiscountPercentage { get; set; }
    }
}