using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MusicApi.Models;

namespace MusicApi.Controllers
{
    public class TracksController : ApiController
    {
        static readonly ITrackRepository repository = new TrackRepository();

        public IEnumerable<Track> GetAllTracks()
        {
            return repository.GetAll();
        }

        public Track GetTrack(int id)
        {
            Track item = repository.Get(id);
            if (item == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return item;
        }

        public HttpResponseMessage PostTrack(Track item)
        {
            item = repository.Add(item, false);
            var response = Request.CreateResponse<Track>(HttpStatusCode.Created, item);

            string uri = Url.Link("DefaultApi", new { id = item.Id });
            response.Headers.Location = new Uri(uri);
            return response;
        }

        public void PutTrack(int id, Track track)
        {
            track.Id = id;
            if (!repository.Update(track))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }

        public void DeleteTrack(int id)
        {
            Track item = repository.Get(id);
            if (item == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            repository.Remove(id);
        }
    }
}
