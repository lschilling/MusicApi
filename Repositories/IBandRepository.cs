using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicApi.Models.Request;

namespace MusicApi.Models
{
    public interface IBandRepository
    {
        IEnumerable<Band> GetAll();
        Band Get(int id);
        void Remove(int id);
        Band Update(BandRequest item);
    }
}
