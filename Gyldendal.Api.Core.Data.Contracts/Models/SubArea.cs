// ReSharper disable UnusedAutoPropertyAccessor.Global

using Gyldendal.Api.CommonContracts;

namespace Gyldendal.Api.CoreData.Contracts.Models
{
    /// <summary>
    /// "Opslagsbøger", "Frilæsning" 
    /// </summary>
    public class SubArea
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Name { get; set; }

        /// <summary>
        /// WebShot of the SubArea
        /// </summary>
        public WebShop WebShop { get; set; }

        /// <summary>
        /// The SubjectId this SubArea belongs to.
        /// </summary>
        public int SubjectId { get; set; }
    }
}
