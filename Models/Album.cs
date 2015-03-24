using System;
using System.Collections.Generic;
using PointW.WebApi.ResourceModel;

namespace MusicApi.Models
{
    /// <summary>
    /// Class for encapsulating Album information for responses
    /// </summary>
    public class Album : Resource
    {
        /// <summary>Unique identifer for the Album.</summary>
        public int Id { get; set; }

        /// <summary>Unique Band Id for the Album.</summary>
        public int BandId { get; set; }

        /// <summary>Band Name for the Album.</summary>
        public string BandName { get; set; }

        /// <summary>Name for the Album.</summary>
        public string Name { get; set; }

        /// <summary>Date of release for the Album.</summary>
        public DateTime ReleaseDate { get; set; }

        /// <summary>Genre for the Album. Multiple genres cannot be represented in this version.</summary>
        public string Genre { get; set; }

        /// <summary>Genre Id for the Album. Multiple genres cannot be represented in this version.</summary>
        public int GenreId { get; set; }

        /// <summary>URL for any album art that exists.</summary>
        public string AlbumArtUrl { get; set; }

        /// <summary>An IEnumerable list of Track objects.</summary>
        public List<Track> Tracks { get; set; }
    }
}