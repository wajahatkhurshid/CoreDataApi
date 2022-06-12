using System.Xml.Serialization;

namespace Gyldendal.Api.CoreData.Contracts.Models
{
    /// <summary>
    /// Contributor of the products
    /// </summary>
    //[XmlRoot( ElementName = "Contributor", Namespace = "www.gyldendal.dk")]

    [XmlType("ContributorDetails")]
    public class ContributorDetails : BaseContributorDetails
    {
        /// <summary>
        /// Gets or sets the photo.
        /// </summary>
        /// <value>The photo.</value>
        [XmlElement("Photo")]
        public string Photo { get; set; }
    }
}