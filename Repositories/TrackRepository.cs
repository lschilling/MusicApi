using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;


namespace MusicApi.Models
{
    public class TrackRepository : ITrackRepository
    {
        private List<Track> tracks = new List<Track>();
        private string connectionString = ConfigurationManager.ConnectionStrings["MusicAPIConnection"].ToString();

        public TrackRepository()
        {
            RefreshLocalCache();
        }

        private void RefreshLocalCache()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                tracks.Clear();
                foreach (Track t in connection.Query<Track>("lrs7225.GetTracks"))
                {
                    Add(t, true);
                }
            }
        }

        public IEnumerable<Track> GetAll()
        {
            return tracks;
        }

        public Track Get(int id)
        {
            var track = tracks.Find(p => p.Id == id);
            return track;
        }

        public Track Add(Track item, bool localOnly)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            if (!localOnly)
            {
                item.Id = 0;
                item = UpsertTrackInDB(item);
                RefreshLocalCache();
            }
            else
                tracks.Add(item);
            return item;
        }

        public void Remove(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var param = new DynamicParameters();
                param.Add("@Id", id, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                connection.Execute("lrs7225.DeleteTrack", param, commandType: CommandType.StoredProcedure);
            }
            tracks.RemoveAll(p => p.Id == id);
        }

        public bool Update(Track item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            int index = tracks.FindIndex(p => p.Id == item.Id);
            if (index == -1)
            {
                return false;
            }
            item = UpsertTrackInDB(item);
            RefreshLocalCache();
            return true;
        }

        private Track UpsertTrackInDB(Track item)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var param = new DynamicParameters();
                param.Add("@Id", item.Id, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                param.Add("@AlbumId", item.AlbumId, dbType: DbType.Int32);
                param.Add("@Name", item.Name, dbType: DbType.String);
                param.Add("@Length", item.Length, dbType: DbType.String);
                param.Add("@Order", item.Order, dbType: DbType.Int32);
                connection.Execute("lrs7225.UpsertTrack", param, commandType: CommandType.StoredProcedure);
                item.Id = param.Get<int>("@Id");
            }

            return item;
        }
    }
}