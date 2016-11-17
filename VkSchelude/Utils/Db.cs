using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkSchelude.Types;

namespace VkSchelude.Utils
{
    class Db
    {
        private static void ClearTable(string table)
        {
            Authorize.connection.Open();
            new SqlCommand($"truncate table {table}", Authorize.connection).ExecuteNonQuery();
            Authorize.connection.Close();
        }
        public static void FillTableLessons(List<LessonInfo> parsedLessons)
        {
            ClearTable("Lessons");

            foreach (var lesson in parsedLessons)
            {
                if (lesson.DateStart == null || lesson.DateEnd == null)
                    continue;
                var insertCommand = "INSERT INTO Lessons " +
                    "(DateFrom, DateTo, Number, TypeOfLesson, Classroom, DayOfWeek, TeacherId, LessonNameId) " +
                    "VALUES " +
                    $"(CONVERT(date, '{lesson.DateStart.Value.Date.ToString("dd.MM.yyyy")}', 104), CONVERT(date, '{lesson.DateEnd.Value.Date.ToString("dd.MM.yyyy")}', 104), {lesson.Number}, '{lesson.Type}', '{lesson.Classroom}', '{GetIdByTitle(lesson.Day, "DaysOfWeek")}', {GetIdByTitle(lesson.Teacher.Trim(), "Teachers")}, {GetIdByTitle(lesson.Lesson.Trim(), "NamesOfLessons")})";
                Authorize.connection.Open();
                new SqlCommand(insertCommand, Authorize.connection).ExecuteNonQuery();
                Authorize.connection.Close();
            }
            Log.Logging("Расписание успешно занесено в БД");
        }
        private static int GetIdByTitle(string title, string table)
        {
            Authorize.connection.Open();
            var id = new SqlCommand($"SELECT Id FROM {table} WHERE Title = '{title}'", Authorize.connection).ExecuteScalar();
            Authorize.connection.Close();
            return (int)id;
        }
    }
}
