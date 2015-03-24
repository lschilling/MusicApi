using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using MusicApi.Models;
using MusicApi.Models.Request;
using PointW.WebApi.ResourceModel;

namespace MusicApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [RoutePrefix("api/v1/albums")]
    public class AlbumsController : ApiController
    {
        static readonly IAlbumRepository Repository = new AlbumRepository();

        /// <summary>Returns all albums in the database, sorted by Band.</summary>
        /// <returns>An IEnumerable list of Album objects, with link relations.</returns>
        [AcceptVerbs("GET","HEAD")]
        [Route("", Name = "GetAllAlbums")]
        [ResponseType(typeof(List<Album>))]
        public HttpResponseMessage GetAllAlbums()
        {
            var albums = Repository.GetAll();
            foreach (var album in albums)
            {
                AddRelations(album);
            }
            return Request.CreateResponse(HttpStatusCode.OK, albums);
        }

        private void AddRelations(Album album)
        {
            album.Relations.Add("self", new Link { Href = Url.Link("GetAlbumById", new { album.Id }) });
            album.Relations.Add("genre", new Link { Href = Url.Link("GetAlbumsByGenre", new { album.Genre }) });
            album.Relations.Add("band", new Link { Href = Url.Link("GetBandById", new { id = album.BandId }) });
        }

        /// <summary>Returns a specific Album given an Album Id.</summary>
        /// <param name="id">The unique Id of the Album to return.</param>
        /// <returns>An Album object, with link relations.</returns>
        [AcceptVerbs("GET", "HEAD")]
        [Route("{id:int}", Name = "GetAlbumById")]
        [ResponseType(typeof(Album))]
        public HttpResponseMessage GetAlbum(int id)
        {
            var album = Repository.Get(id);
            if (album != null)
            {
                AddRelations(album);
                return Request.CreateResponse(HttpStatusCode.OK, album);
            }
            var responseMessage = string.Format("No Album with an Id of {0} could be found.", id);
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, responseMessage);
        }

        /// <summary>Returns all Albums in the database of a given genre.</summary>
        /// <param name="genre">The text value of the genre to search for.</param>
        /// <returns>IEnumerable list of Albums meeting the search criteria, empty collection if none found.</returns>
        [AcceptVerbs("GET", "HEAD")]
        [Route("{genre}", Name = "GetAlbumsByGenre")]
        [ResponseType(typeof(List<Album>))]
        public HttpResponseMessage GetAlbumsByGenre(string genre)
        {
            var albums = Repository.GetAll().Where(
                p => string.Equals(p.Genre, genre, StringComparison.OrdinalIgnoreCase));
            foreach (var album in albums)
            {
                AddRelations(album);
            }
            return Request.CreateResponse(HttpStatusCode.OK, albums);
        }

        /// <summary>POSTs an Album to the database.</summary>
        /// <param name="album">The Album object to POST.</param>
        /// <returns>A 201 HTTP Response.</returns>
        [AcceptVerbs("POST")]
        [Route("", Name = "UpsertAlbum")]
        [ResponseType(typeof(Album))]
        public HttpResponseMessage PostAlbum(AlbumRequest album)
        {
            var name = album.Name;
            var albumResponse = Repository.Update(album);
            if (albumResponse != null)
            {
                AddRelations(albumResponse);
                return Request.CreateResponse(HttpStatusCode.OK, albumResponse);
            }
            var responseMessage = string.Format("Error inserting or updating Album '{0}'", name);
            return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, responseMessage);
        }

        /// <summary>DELETEs an Album from the database.</summary>
        /// <returns>200 OK on success, 404 Not Found if Album does not exist, 500 Internal Server Error on failure.</returns>
        /// <param name="id">The integer Album ID to delete.</param>
        [AcceptVerbs("DELETE")]
        [Route("{id:int}", Name = "DeleteAlbum")]
        public HttpResponseMessage DeleteAlbum(int id)
        {
            var item = Repository.Get(id);
            if (item == null)
            {
                var responseMessage = string.Format("No Album with an Id of {0} could be found.", id);
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, responseMessage);
            }

            Repository.Remove(id);
            return Request.CreateResponse(HttpStatusCode.OK, string.Empty);
        }
    }
}
