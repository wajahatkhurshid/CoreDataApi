// ReSharper disable UnusedAutoPropertyAccessor.Global

using Gyldendal.Api.CommonContracts;

namespace Gyldendal.Api.CoreData.Contracts.Models
{
    
    
    //The Grade level for Primary (Grundskole) school 
    //The level for Secondary school (Gymnasium)
    public class Level
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public int? LevelNumber { get; set; }

        /// <summary>
        /// webshop of level
        /// </summary>
        public WebShop WebShop { get; set; }

        /// <summary>
        /// Area of the level
        /// </summary>
        public int? AreaId { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Name { get; set; }
    }
}
