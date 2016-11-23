using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkSchelude.Types;

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
        /// <summary>
        /// Возвращает ОДИН элемент из базы данных по запросу
        /// </summary>
        /// <param name="Request">Запрос к базе данных</param>
        /// <param name="Parameters">Аргументы к запросу</param>
        /// <returns></returns>
        public static DCustom GetObject(string Request, Dictionary<string, object> Parameters = null)
        {
            if (Authorize.connection.State != System.Data.ConnectionState.Open)
                Authorize.connection.Open();
            if (Parameters == null)
                Parameters = new Dictionary<string, object>();
            if (!Request.Contains(" TOP 1 "))
                Request = Request.Replace("SELECT ", "SELECT TOP 1 ");
            SqlCommand cmd = new SqlCommand(Request, Authorize.connection);
            foreach (var item in Parameters)
            {
                cmd.Parameters.AddWithValue(item.Key, item.Value);
            }
            DCustom result = new DCustom();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                    try
                    {
                        result[reader.GetName(i)] = reader.GetValue(i);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(); // 
                    }
                //result.Add(reader.GetValue(0));
            }
            reader.Close();
            return result;
        }
        public static List<DCustom> GetListObject(string Request, Dictionary<string, object> Parameters = null)
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
            List<DCustom> result = new List<DCustom>();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                DCustom newItem = new DCustom();
                for (int i = 0; i < reader.FieldCount; i++)
                        newItem[reader.GetName(i)] = reader.GetValue(i);
                result.Add(newItem);
            }
            reader.Close();
            return result;
        }
        public static List<object> GetList(string Request, Dictionary<string, object> Parameters = null)
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
            List<object> result = new List<object>();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(reader.GetValue(0));
            }
            reader.Close();
            return result;
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
            reader.Close();
            return result;
        }
    }
}
