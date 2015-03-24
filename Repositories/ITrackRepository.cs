using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicApi.Models
{
    interface ITrackRepository
    {
        IEnumerable<Track> GetAll();
        Track Get(int id);
        Track Add(Track item, bool localOnly);
        void Remove(int id);
        bool Update(Track item);
    }
}
