using System;
using System.ComponentModel.DataAnnotations;

namespace MusicApi.Models.Request
{
    public class TrackRequest
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int AlbumId { get; set; }
        
        public string AlbumName { get; set; }
        
        public int BandId { get; set; }
        
        public string BandName { get; set; }

        public int Order { get; set; }

        public TimeSpan Duration
        {
            get { return TimeSpan.FromTicks(Length); }
            set { Length = value.Ticks; }
        }
        
        public long Length { get; set; }

        public int BitRate { get; set; }

        public int Year { get; set; }
        
        public string Description { get; set; }
        
        public string Comment { get; set; }

        public string Genre { get; set; }
    }
}