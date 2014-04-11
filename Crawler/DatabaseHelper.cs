using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Crawler
{
    class DatabaseHelper
    {
        private readonly MySqlConnectionStringBuilder _connString;

        public DatabaseHelper()
        {
            _connString = new MySqlConnectionStringBuilder
            {
                Server = "ForagerAdmin.db.10586941.hostedresource.com",
                Password = "Te@mQu4tro",
                UserID = "ForagerAdmin",
                Database = "ForagerAdmin"
            };
        }

        private static string GetMd5Hash(DateTime hashTime)
        {
            var hash = MD5.Create();
            var data = hash.ComputeHash(Encoding.UTF8.GetBytes(hashTime.ToString()));
            var sBuilder = new StringBuilder();
            for (var i = 0; i < data.Length; i++)
                sBuilder.Append(data[i].ToString("x2"));
            return sBuilder.ToString();
        }
         
        public bool CheckRunning(string startTimeGuid)
        {
            var tf = false;
            var conn = new MySqlConnection();
            var cmd = new MySqlCommand();
            conn.ConnectionString = _connString.ToString();
            try
            {
                conn.Open();
                cmd.Connection = conn;
                cmd.CommandText = "SELECT running FROM `ForagerAdmin`.`Scans` " +
                                  "WHERE scan_guid = '" + startTimeGuid + "'";
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                    tf = (int)reader[0] == 1;
                return tf;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error " + ex.Number + " has occurred: " + ex.Message, "Error");
                return false;
            }
        }
        public void BeginScan(DateTime startTime)
        {
            var conn = new MySqlConnection();
            var cmd = new MySqlCommand();
            conn.ConnectionString = _connString.ToString();
            try
            {
                var startTimeGuid = GetMd5Hash(startTime);
                conn.Open();
                cmd.Connection = conn;

                cmd.CommandText = "INSERT INTO `ForagerAdmin`.`Scans` " +
                                  "VALUES(NULL, @UserID, @site, @start_time, @end_time, @total_pages, @running, @scan_guid)";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@UserID", "ForagerAdmin");
                cmd.Parameters.AddWithValue("@site", "http://www.spsu.edu/");
                cmd.Parameters.AddWithValue("@start_time", startTime);
                cmd.Parameters.AddWithValue("@end_time", null);
                cmd.Parameters.AddWithValue("@total_pages", 0);
                cmd.Parameters.AddWithValue("@running", 1);
                cmd.Parameters.AddWithValue("@scan_guid", startTimeGuid);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error " + ex.Number + " has occurred: " + ex.Message, "Error");
            }
        }
        public int GetId(string _startTimeGuid)
        {
            var conn = new MySqlConnection();
            var cmd = new MySqlCommand();
            conn.ConnectionString = _connString.ToString();
            try
            {
                conn.Open();
                cmd.Connection = conn;
                cmd.CommandText = "SELECT scan_id, start_time FROM `ForagerAdmin`.`Scans` " +
                                  "WHERE scan_guid = '" + _startTimeGuid + "'";
                var reader = cmd.ExecuteReader();
                return reader.Read() ? (int)reader[0] : -1;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error " + ex.Number + " has occurred: " + ex.Message, "Error");
                return -1;
            }
        }
    }
}
