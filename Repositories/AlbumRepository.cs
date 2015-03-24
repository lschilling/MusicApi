using System.Collections.Generic;
using System.Linq;
using Dapper;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using MusicApi.Models.Request;

namespace MusicApi.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class AlbumRepository : IAlbumRepository
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["MusicAPIConnection"].ToString();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Album> GetAll()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var multi = connection.QueryMultiple("lrs7225.GetAlbums", commandType: CommandType.StoredProcedure))
                {
                    var albums = multi.Read<Album>().ToList();
                    var tracks = multi.Read<Track>().ToList();

                    foreach (var album in albums)
                    {
                        album.Tracks = tracks.FindAll(x => x.AlbumId == album.Id);
                    }
                    return albums;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Album Get(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var multi = connection.QueryMultiple("lrs7225.GetAlbumById", new { AlbumId = id }, commandType: CommandType.StoredProcedure))
                {
                    var album = multi.Read<Album>().ToList().FirstOrDefault();
                    if (album == null) return null;
                    var tracks = multi.Read<Track>().ToList();

                    album.Tracks = tracks;
                    return album;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public void Remove(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var param = new DynamicParameters();
                param.Add("@Id", id, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                connection.Execute("lrs7225.DeleteAlbum", param, commandType: CommandType.StoredProcedure);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="album"></param>
        /// <returns></returns>
        public Album Update(AlbumRequest album)
        {
            if (album == null)
                return null;
            var albumResponse = UpsertAlbumInDb(album);
            return albumResponse;
        }

        private Album UpsertAlbumInDb(AlbumRequest album)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var param = new DynamicParameters();
                param.Add("@Id", album.Id, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                param.Add("@Name", album.Name, dbType: DbType.String);
                param.Add("@BandId", album.BandId, dbType: DbType.Int32);
                param.Add("@BandName", album.BandName, dbType: DbType.String);
                param.Add("@Genre", album.Genre, dbType: DbType.String);
                param.Add("@AlbumArtUrl", album.AlbumArtUrl, dbType: DbType.String);
                //param.Add("@ReleaseDate", album.ReleaseDate, DbType.DateTime);
                connection.Execute("lrs7225.UpsertAlbum", param, commandType: CommandType.StoredProcedure);
                album.Id = param.Get<int>("@Id");

                foreach (var track in album.Tracks)
                {
                    track.AlbumId = album.Id;
                    UpsertTrackInDb(track, connection);
                }
            }
            return Get(album.Id);
        }

        private void UpsertTrackInDb(Track track, SqlConnection connection)
        {
            var param = new DynamicParameters();
            param.Add("@Id", track.Id, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
            param.Add("@AlbumId", track.AlbumId, dbType: DbType.Int32);
            param.Add("@Name", track.Name, dbType: DbType.String);
            param.Add("@Length", track.Length, dbType: DbType.String);
            param.Add("@Order", track.Order, dbType: DbType.Int32);
            param.Add("@mp3BitRate", track.BitRate, dbType: DbType.Int32);
            param.Add("@Description", track.Description, dbType: DbType.String);
            param.Add("@Comment", track.Comment, dbType: DbType.String);
            param.Add("@Genre", track.Genre, dbType: DbType.String);
            param.Add("@Year", track.Year, dbType: DbType.Int32);
            connection.Execute("lrs7225.UpsertTrack", param, commandType: CommandType.StoredProcedure);
            track.Id = param.Get<int>("@Id");
        }
    }
}