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
    /// Repository for performing CRUD operations on Bands in the SQL Server database.
    /// </summary>
    public class BandRepository : IBandRepository
    {
        //private List<Band> bands = new List<Band>();
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["MusicAPIConnection"].ToString();

        /// <summary>This method returns all Band records.</summary>
        /// <returns>A list of band objects.</returns>
        public IEnumerable<Band> GetAll()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var multi = connection.QueryMultiple("lrs7225.GetBands", commandType: CommandType.StoredProcedure))
                {
                    var bands = multi.Read<Band>().ToList();
                    var albums = multi.Read<Album>().ToList();
                    var tracks = multi.Read<Track>().ToList();

                    foreach (var band in bands)
                    {
                        band.Albums = albums.FindAll(x => x.BandId == band.Id);
                        foreach (var album in band.Albums)
                        {
                            album.Tracks = tracks.FindAll(x => x.AlbumId == album.Id);
                        }
                    }
                    return bands;
                }
            }
        }

        /// <summary>This method returns a Band record, with Album list, for a given Band Id.</summary>
        /// <returns>A single band object, or a null Band object if none found by the given id.</returns>
        public Band Get(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var multi = connection.QueryMultiple("lrs7225.GetBandById", new { Id = id }, commandType: CommandType.StoredProcedure))
                {
                    var band = multi.Read<Band>().ToList().FirstOrDefault();
                    if (band == null)
                        return null;
                    var albums = multi.Read<Album>().ToList();
                    var tracks = multi.Read<Track>().ToList();

                    band.Albums = albums;
                    foreach (var album in band.Albums)
                    {
                        album.Tracks = tracks.FindAll(x => x.AlbumId == album.Id);
                    }
                    return band;
                }
            }
        }

        /// <summary>Removes the band object from the repo</summary>
        /// <param name="id">ID of band object to remove</param>
        public void Remove(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var param = new DynamicParameters();
                param.Add("@Id", id, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                connection.Execute("lrs7225.DeleteBand", param, commandType: CommandType.StoredProcedure);
            }
        }

        /// <summary>Updates band object</summary>
        /// <param name="band">Band object to update.</param>
        /// <returns>Updated/inserted band object.</returns>
        public Band Update(BandRequest band)
        {
            if (band == null)
                return null;
            var bandResponse = UpsertBandInDb(band);
            return bandResponse;
        }

        private Band UpsertBandInDb(BandRequest band)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var param = new DynamicParameters();
                param.Add("@Id", band.Id, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                param.Add("@Name", band.Name, dbType: DbType.String);
                param.Add("@Genre", band.Genre, dbType: DbType.String);
                param.Add("@SortName", band.SortName, dbType: DbType.String);
                connection.Execute("lrs7225.UpsertBand", param, commandType: CommandType.StoredProcedure);
                band.Id = param.Get<int>("@Id");
            }

            var albumRepository = new AlbumRepository();
            foreach (var album in band.Albums)
            {
                album.BandId = band.Id;
                albumRepository.Update(album);
            }

            var bandModified = Get(band.Id);
            return bandModified;
        }

    }
}