// ReSharper disable UnusedAutoPropertyAccessor.Global

using System;

namespace Gyldendal.Api.CoreData.Contracts.Models
{
    public class DiscountOfferInfo
    {
        public string OfferMiniText { get; set; }

        public string OfferLongText { get; set; }

        public string[] IsbnsPartOfOffer { get; set; }

        public DateTime OfferStartDate { get; set; }

        public DateTime OfferEndDate { get; set; }
    }
}