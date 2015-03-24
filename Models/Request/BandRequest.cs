using System.Collections.Generic;

namespace MusicApi.Models.Request
{
    public class BandRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SortName { get; set; }
        public string Genre { get; set; }
        public List<AlbumRequest> Albums { get; set; }
    }
}