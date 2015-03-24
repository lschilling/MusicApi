using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicApi.Models.Request;

namespace MusicApi.Models
{
    interface IAlbumRepository
    {
        IEnumerable<Album> GetAll();
        Album Get(int id);
        void Remove(int id);
        Album Update(AlbumRequest item);
    }
}
