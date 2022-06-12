// ReSharper disable UnusedAutoPropertyAccessor.Global

using System;
using System.Collections.Generic;
using Gyldendal.Api.CommonContracts;

namespace Gyldendal.Api.CoreData.Contracts.Models
{
    public class Series
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }

        public Series ParentSeries { get; set; }

        public int? ParentSerieId { get; set; }

        public List<Series> ChildSeries { get; set; }

        /// <summary>
        /// A collection of Areas associated with the system.
        /// </summary>
        public List<Area> Areas { get; set; }

        /// <summary>
        /// A collection of SubAreas associated with the system.
        /// </summary>
        public List<SubArea> SubAreas { get; set; }

        /// <summary>
        /// A collection of Levels associated with the system.
        /// </summary>
        public List<Level> Levels { get; set; }

        /// <summary>
        /// A collection of Subject associated with the system.
        /// </summary>
        public List<Subject> Subjects { get; set; }

        /// <summary>
        /// webshop of the system
        /// </summary>
        public WebShop WebShop { get; set; }

        /// <summary>
        /// Url for the image of this System, can be empty.
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the DateTime value for when the last time this System got updated.
        /// </summary>
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// Flag indicating whether this Series is a System Series or not.
        /// </summary>
        public bool IsSystemSeries { get; set; }
    }
}
