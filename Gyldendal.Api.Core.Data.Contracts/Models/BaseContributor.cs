using System.Xml.Serialization;
using Gyldendal.Api.CoreData.Contracts.Enumerations;

namespace Gyldendal.Api.CoreData.Contracts.Models
{
    public class BaseContributor
    {
        /// <summary>
        ///Gets or Sets of Id of the Contributor
        /// </summary>
        [XmlElement("Id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or Sets of First name of the contributor
        /// </summary>
        [XmlElement("FirstName")]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or Sets of Last name of the contributor
        /// </summary>
        [XmlElement("LastName")]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the biography of Author.
        /// </summary>
        /// <value>The biography.</value>
        [XmlElement("Biography")]
        public string Biography { get; set; }

        /// <summary>
        /// Gets or sets the bibliography.
        /// </summary>
        /// <value>The bibliography.</value>
        [XmlElement("Bibliography")]
        public string Bibliography { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        [XmlElement("Url")]
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the author number.
        /// </summary>
        /// <value>The author number.</value>
        [XmlElement("AuthorNumber")]
        public string AuthorNumber { get; set; }

        [XmlElement("SearchName")]
        public string SearchName { get; set; }

        /// <summary>
        /// Gets or Sets Contributor Type
        /// </summary>
        [XmlElement("ContributorType")]
        public ContributorType ContibutorType { get; set; }
    }
}
