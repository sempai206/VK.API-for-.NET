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
                        throw new Exception(ex.Message); // 
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
        #region Заполнения таблиц
        /// <summary>
        /// Заполняет таблицы в БД
        /// </summary>
        /// <param name="parsedLessons">Лист с распарсенным расписанием</param>
        public static void FillTables(List<LessonInfo> parsedLessons)
        {
            FillTeachers(parsedLessons);
            FillNamesOfLessons(parsedLessons);
            new SqlCommand(DBHelper.GetInternalSQLRequest(9), Authorize.connection).ExecuteNonQuery();
            foreach (var lesson in parsedLessons)
            {
                if (lesson.DateStart == null || lesson.DateEnd == null)
                    continue;
                var request = DBHelper.GetInternalSQLRequest(14);
                request = request.Replace("@DateStart", $"{lesson.DateStart.Value.Date}");
                request = request.Replace("@DateEnd", $"{lesson.DateEnd.Value.Date}");
                request = request.Replace("@Number", $"{lesson.Number}");
                request = request.Replace("@TypeOfLesson", $"{lesson.Type}");
                request = request.Replace("@Classroom", $"{lesson.Classroom}");
                request = request.Replace("@DayOfWeek", $"{GetIdByTitle(lesson.Day.Trim(), "ref_DaysOfWeek").ToString()}");
                request = request.Replace("@TeacherId", $"{GetIdByTitle(lesson.Teacher.Trim(), "tbl_Teachers")}");
                request = request.Replace("@LessonNameId", $"{GetIdByTitle(lesson.Lesson.Trim(), "ref_NamesOfLessons")}");
                new SqlCommand(request, Authorize.connection).ExecuteNonQuery();
            }
            Log.Logging("Расписание успешно занесено в БД");
        }
        /// <summary>
        /// Получает ID из таблицы по заданному наименованию
        /// </summary>
        /// <param name="title">Наименование</param>
        /// <param name="table">Таблица</param>
        /// <returns></returns>
        private static int GetIdByTitle(string title, string table)
        {
            var request = DBHelper.GetInternalSQLRequest(10);
            request = request.Replace("@Table", table);
            request = request.Replace("@Title", title);
            return (int)DBHelper.GetObject(request)["Id"];
        }
        private static void FillTeachers(List<LessonInfo> parsedLessons)
        {
            if (Authorize.connection.State != System.Data.ConnectionState.Open)
                Authorize.connection.Open();
            var teachersList = DBHelper.GetListObject(DBHelper.GetInternalSQLRequest(8).Replace("@Table", "tbl_Teachers"));
            var uniqueTeachers = new List<string>();
            foreach (var lesson in parsedLessons)
                if (!uniqueTeachers.Contains(lesson.Teacher))
                    uniqueTeachers.Add(lesson.Teacher);
            foreach (var teacher in uniqueTeachers)
            {
                if (!teachersList.Any(i => i["Title"].ToString() == teacher))
                {
                    var request = DBHelper.GetInternalSQLRequest(12);
                    request = request.Replace("@Table", "tbl_Teachers");
                    request = request.Replace("@Value", teacher);
                    new SqlCommand(request, Authorize.connection).ExecuteNonQuery();
                }
            }
        }
        private static void FillNamesOfLessons(List<LessonInfo> parsedLessons)
        {
            var namesOfLessonsList = DBHelper.GetListObject(DBHelper.GetInternalSQLRequest(8).Replace("@Table", "ref_NamesOfLessons"));
            var uniqueNamesOfLessons = new List<string>();
            foreach (var lesson in parsedLessons)
                if (!uniqueNamesOfLessons.Contains(lesson.Lesson))
                    uniqueNamesOfLessons.Add(lesson.Lesson);
            foreach (var lesson in uniqueNamesOfLessons)
            {
                if (!namesOfLessonsList.Any(i => i["Title"].ToString() == lesson))
                {
                    var request = DBHelper.GetInternalSQLRequest(12);
                    request = request.Replace("@Table", "ref_NamesOfLessons");
                    request = request.Replace("@Value", lesson);
                    new SqlCommand(request, Authorize.connection).ExecuteNonQuery();
                }
            }
        }
        #endregion
    }
}
