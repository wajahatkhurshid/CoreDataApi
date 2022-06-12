// ReSharper disable UnusedAutoPropertyAccessor.Global

using Gyldendal.Api.CommonContracts;

namespace Gyldendal.Api.CoreData.Contracts.Models
{
    public class Subject
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// webshop of subject
        /// </summary>
        public WebShop WebShop { get; set; }

        /// <summary>
        /// Area of the subject
        /// </summary>
        public int? AreaId { get; set; }
    }
}
