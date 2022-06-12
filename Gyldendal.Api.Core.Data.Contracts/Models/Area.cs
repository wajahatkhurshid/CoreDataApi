using Gyldendal.Api.CommonContracts;

namespace Gyldendal.Api.CoreData.Contracts.Models
{
    /// <summary>
    /// Grundskole, Gymnasium etc.
    /// </summary>
    public class Area
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Name { get; set; }

        /// <summary>
        /// webshop of the Area
        /// </summary>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public WebShop WebShop { get; set; }
    }
}
