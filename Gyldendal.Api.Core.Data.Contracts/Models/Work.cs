// ReSharper disable UnusedAutoPropertyAccessor.Global

using System.Collections.Generic;
using Gyldendal.Api.CommonContracts;

namespace Gyldendal.Api.CoreData.Contracts.Models
{
    
    
    public class Work
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
        public string Title { get; set; }

        /// <summary>
        ///  Gets or sets the description for the Work.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The shop to which the work is associated.
        /// </summary>
        public WebShop WebShop { get; set; }
        
        /// <summary>
        /// Gets or sets the products contained in the work.
        /// </summary>
        public List<Product> Products { get; set; }
        
        /// <summary>
        /// Gets or sets the Themacodes of the work
        /// </summary>
        public List<ThemaCode> ThemaCodes { get; set; }

        /// <summary>
        /// A collection of subejects associated with the product.
        /// </summary>
        public List<Subject> Subjects { get; set; }

        /// <summary>
        /// A collection of Areas associated with the product.
        /// </summary>
        public List<Area> Areas { get; set; }

        /// <summary>
        /// A collection of SubAreas associated with the product.
        /// </summary>
        public List<SubArea> SubAreas { get; set; }

        /// <summary>
        /// A collection of Levels associated with the product.
        /// </summary>
        public List<Level> Levels { get; set; }
    }
}
