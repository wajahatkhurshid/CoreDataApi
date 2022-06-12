using System;
using System.Xml.Serialization;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Gyldendal.Api.CoreData.Contracts.Models
{
    
    
    [XmlType("Review")]
    public class ProductReview
    {
        [XmlElement("ProductId")]
        public string ProductId { get; set; }

        [XmlElement("ReviewBy")]
        public string ReviewBy { get; set; }

        [XmlElement("ReviewDate")]
        public DateTime ReviewDate { get; set; }

        [XmlElement("ReviewText")]
        public string ReviewText { get; set; }

        [XmlElement("ReviewSource")]
        public string ReviewSource { get; set; }
    }
}
