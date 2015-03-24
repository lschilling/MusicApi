using System.Collections.Generic;
using PointW.WebApi.ResourceModel;

namespace MusicApi.Models
{
    /// <summary>
    /// Class for encapsulating Band information for responses
    /// </summary>
    public class Band : Resource
    {
        /// <summary>Unique identifer for the Band.</summary>
        public int Id { get; set; }

        /// <summary>Name for band.</summary>
        public string Name { get; set; }

        /// <summary>Sorting name, may be different from Name. Eliminates "The" at the beginning of many Band names.</summary>
        public string SortName { get; set; }

        /// <summary>The name of the primary genre for the Band. Multiple genres cannot be represented in this version.</summary>
        public string Genre { get; set; }
        
        /// <summary>An IEnumerable list of Album objects.</summary>
        public List<Album> Albums { get; set; }
    }
}