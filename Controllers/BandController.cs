using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Mvc;
using MusicApi.Models;
using MusicApi.Models.Request;
using PointW.WebApi.ResourceModel;

namespace MusicApi.Controllers
{
    /// <summary>Handles MusicAPI requests for Bands</summary>
    /// <returns></returns>
    [System.Web.Http.RoutePrefix("api/v1/bands")]
    public class BandsController : ApiController
    {
        static readonly IBandRepository Repository = new BandRepository();

        /// <summary>Returns all Bands in the database.</summary>
        /// <returns>IEnumerable list of Bands, or empty collection if none found.</returns>
        [System.Web.Http.AcceptVerbs("GET", "HEAD")]
        [System.Web.Http.Route("", Name = "GetAllBands")]
        [ResponseType(typeof(List<Band>))]
        public HttpResponseMessage GetAllBands()
        {
            var bands = Repository.GetAll();
            foreach (var band in bands)
            {
                AddRelations(band);
            }
            return Request.CreateResponse(HttpStatusCode.OK, bands);
        }

        /// <summary>Returns a specific Band given a Band ID.</summary>
        /// <returns>A 200 OK with a single Band object, or a 404 Not Found if Id is not found.</returns>
        /// <param name="id">The integer Band ID to return.</param>
        [System.Web.Http.AcceptVerbs("GET", "HEAD")]
        [System.Web.Http.Route("{id:int}", Name = "GetBandById")]
        [ResponseType(typeof(Band))]
        public HttpResponseMessage GetBand(int id)
        {
            var band = Repository.Get(id);
            if (band != null)
            {
                AddRelations(band);
                return Request.CreateResponse(HttpStatusCode.OK, band);
            }
            var responseMessage = string.Format("No Band with an Id of {0} could be found.", id);
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, responseMessage);
        }

        private void AddRelations(Band band)
        {
            band.Relations.Add("self", new Link { Href = Url.Link("GetBandById", new { band.Id }) });
            band.Relations.Add("genre", new Link { Href = Url.Link("GetBandsByGenre", new { band.Genre }) });
            band.Relations.Add("index", new Link() { Href = Url.Link("GetAllBands", null) });
            foreach (var album in band.Albums)
            {
                album.Relations.Add("self", new Link { Href = Url.Link("GetAlbumById", new { album.Id }) });
                album.Relations.Add("genre", new Link { Href = Url.Link("GetAlbumsByGenre", new { album.Genre }) });
                album.Relations.Add("band", new Link { Href = Url.Link("GetBandById", new { id = album.BandId }) });
            }
        }

        /// <summary>Returns all Bands in the database of a given genre.</summary>
        /// <returns>IEnumerable list of Bands meeting the search criteria, empty collection if none found.</returns>
        /// <param name="genre">The text value of the genre to search for.</param>
        [System.Web.Http.AcceptVerbs("GET", "HEAD")]
        [System.Web.Http.Route("{genre}", Name = "GetBandsByGenre")]
        [ResponseType(typeof(List<Band>))]
        public HttpResponseMessage GetBandsByGenre(string genre)
        {
            var bands = Repository.GetAll().Where(
                p => string.Equals(p.Genre, genre, StringComparison.OrdinalIgnoreCase));
            foreach (var band in bands)
            {
                AddRelations(band);
            }
            return Request.CreateResponse(HttpStatusCode.OK, bands);
        }

        /// <summary>POSTs a Band to the database.</summary>
        /// <returns>A 201 HTTP Response.</returns>
        /// <param name="band">The Band object to POST.</param>
        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.Route("", Name = "UpsertBand")]
        [ResponseType(typeof(Band))]
        public HttpResponseMessage PostBand(BandRequest band)
        {
            var name = band.Name;
            var bandResponse = Repository.Update(band);
            if (bandResponse != null)
            {
                AddRelations(bandResponse);
                return Request.CreateResponse(HttpStatusCode.Created, bandResponse);
            }
            var responseMessage = string.Format("Error inserting or updating Band '{0}'", name);
            return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, responseMessage);
        }


        /// <summary>DELETEs a Band from the database.</summary>
        /// <returns>200 OK on success, 404 Not Found if Band does not exist, 500 Internal Server Error on failure.</returns>
        /// <param name="id">The integer Band ID to delete.</param>
        [System.Web.Http.AcceptVerbs("DELETE")]
        [System.Web.Http.Route("{id}", Name = "DeleteBand")]
        public HttpResponseMessage DeleteBand(int id)
        {
            var item = Repository.Get(id);
            if (item == null)
            {
                var responseMessage = string.Format("No Band with an Id of {0} could be found.", id);
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, responseMessage);
            }

            Repository.Remove(id);
            return Request.CreateResponse(HttpStatusCode.OK, string.Empty);
        }
    }
}