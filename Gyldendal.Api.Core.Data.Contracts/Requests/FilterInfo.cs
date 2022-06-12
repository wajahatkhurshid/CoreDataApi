using System.Collections.Generic;
// ReSharper disable UnusedAutoPropertyAccessor.Global

// ReSharper disable CollectionNeverUpdated.Global

namespace Gyldendal.Api.CoreData.Contracts.Requests
{
    public class FilterInfo
    {
        public FilterInfo()
        {
            SystemNameFilter = new List<string>();
            SeriesNameFilter = new List<string>();
            LevelNameFilter = new List<string>();
            MaterialTypeNameFilter = new List<string>();
            MediaTypeNameFilter = new List<string>();
            SubjectsFilter = new List<string>();
            AreasFilter = new List<string>();
            SubAreasFilter = new List<string>();
        }

        public List<string> SystemNameFilter { get; set; }

        public List<string> LevelNameFilter { get; set; }

        public List<string> SeriesNameFilter { get; set; }

        public List<string> MaterialTypeNameFilter { get; set; }

        public List<string> MediaTypeNameFilter { get; set; }

        public List<string> SubjectsFilter { get; set; }

        public List<string> AreasFilter { get; set; }

        public List<string> SubAreasFilter { get; set; }
    }
}
