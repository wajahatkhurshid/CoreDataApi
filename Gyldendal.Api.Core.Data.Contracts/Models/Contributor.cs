using System.Xml.Serialization;
using Gyldendal.Api.CoreData.Contracts.Enumerations;

namespace Gyldendal.Api.CoreData.Contracts.Models
{
    /// <summary>
    /// Contributor of the products
    /// </summary>
    //[XmlRoot( ElementName = "Contributor", Namespace = "www.gyldendal.dk")]

    [XmlType("Contributor")]
    public class Contributor : BaseContributor
    {
        /// <summary>
        /// Gets or sets the photo.
        /// </summary>
        /// <value>The photo.</value>
        [XmlElement("Photo")]
        public string Photo { get; set; }
    }
}