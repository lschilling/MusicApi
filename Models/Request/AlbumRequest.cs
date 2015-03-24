using System;
using System.Collections.Generic;

namespace MusicApi.Models.Request
{
    public class AlbumRequest
    {
        public int Id { get; set; }
        public int BandId { get; set; }
        public string BandName { get; set; }
        public string Name { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Genre { get; set; }
        public string AlbumArtUrl { get; set; }
        public List<Track> Tracks { get; set; }
    }
}