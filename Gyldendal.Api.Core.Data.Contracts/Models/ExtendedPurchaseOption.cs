using Gyldendal.Api.CoreData.Contracts.Enumerations;

namespace Gyldendal.Api.CoreData.Contracts.Models
{
    public class ExtendedPurchaseOption
    {
        public PurchaseOptionType Type { get; set; }

        public string Description { get; set; }

        public PurchaseOptionProperties PurchaseOptionProperties { get; set; }
    }
}