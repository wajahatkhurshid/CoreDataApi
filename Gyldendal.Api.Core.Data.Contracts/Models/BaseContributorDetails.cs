using System.Collections.Generic;
using System.Xml.Serialization;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Contracts.Enumerations;

namespace Gyldendal.Api.CoreData.Contracts.Models
{
    public class BaseContributorDetails
    {
        /// <summary>
        ///Gets or Sets of Id of the Contributor
        /// </summary>
        [XmlElement("Id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or Sets of full name of the contributor
        /// </summary>
        [XmlElement("ContributorName")]
        public string ContributorName { get; set; }

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

        /// <summary>
        /// Gets or Sets Contributor Type
        /// </summary>
        [XmlElement("ContributorType")]
        public List<ContributorType> ContibutorType { get; set; }

        [XmlElement("SearchName")]
        public string SearchName { get; set; }

        public string ContributorFirstName { get; set; }

        public string ContributorLastName { get; set; }

        public List<WebShop> WebShopsId { get; set; }
    }
}
