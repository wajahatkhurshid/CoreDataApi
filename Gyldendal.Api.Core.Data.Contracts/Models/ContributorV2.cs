using System.Collections.Generic;
using System.Xml.Serialization;

namespace Gyldendal.Api.CoreData.Contracts.Models
{
    /// <summary>
    /// Contributor of the products
    /// </summary>
    //[XmlRoot( ElementName = "Contributor", Namespace = "www.gyldendal.dk")]

    [XmlType("Contributor")]
    public class ContributorV2 : BaseContributor
    {
        /// <summary>
        /// Gets or sets the photos.
        /// </summary>
        /// <value>The photos.</value>
        [XmlElement("Photos")]
        public List<ProfileImage> Photos { get; set; }
    }
}
