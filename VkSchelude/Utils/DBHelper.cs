using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkSchelude.Utils
{
    public static class DBHelper
    {
        public static string GetInternalSQLRequest(int Id)
        {
            if (Authorize.connection.State != System.Data.ConnectionState.Open)
                Authorize.connection.Open();
            return new SqlCommand($"SELECT SqlCommand FROM sys_SqlCommands WHERE Id = {Id}", Authorize.connection).ExecuteScalar().ToString();
        }
        public static object GetSingleObject(string Request, Dictionary <string,object> Parameters = null)
        {
            if (Authorize.connection.State != System.Data.ConnectionState.Open)
                Authorize.connection.Open();
            if (Parameters == null)
                Parameters = new Dictionary<string, object>();
            SqlCommand cmd = new SqlCommand(Request, Authorize.connection);
            foreach(var item in Parameters)
            {
                cmd.Parameters.AddWithValue(item.Key, item.Value);
            }
            return cmd.ExecuteScalar().ToString();
        }
        public static Dictionary<object, object> GetDictionary(string Request, Dictionary<string, object> Parameters = null)
        {
            if (Authorize.connection.State != System.Data.ConnectionState.Open)
                Authorize.connection.Open();
            if (Parameters == null)
                Parameters = new Dictionary<string, object>();
            SqlCommand cmd = new SqlCommand(Request, Authorize.connection);
            foreach (var item in Parameters)
            {
                cmd.Parameters.AddWithValue(item.Key, item.Value);
            }
            Dictionary<object, object> result = new Dictionary<object, object>();
            var reader = cmd.ExecuteReader();
            while(reader.Read())
            {
                result.Add(reader.GetValue(0), reader.GetValue(1));
            }
            return result;
        }
    }
}
