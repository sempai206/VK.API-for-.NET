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
        public static void FillTableLessons(List<LessonInfo> parsedLessons)
        {
            FillTeachers(parsedLessons);
            FillNamesOfLessons(parsedLessons);
            Authorize.connection.Open();
            new SqlCommand("UPDATE tbl_Lessons SET isActive = 0").ExecuteNonQuery();
            Authorize.connection.Close();
            foreach (var lesson in parsedLessons)
            {
                if (lesson.DateStart == null || lesson.DateEnd == null)
                    continue;
                var insertCommand = "INSERT INTO tbl_Lessons " +
                    "(DateFrom, DateTo, Number, TypeOfLesson, Classroom, DayOfWeek, TeacherId, LessonNameId) " +
                    "VALUES " +
                    $"(CONVERT(date, '{lesson.DateStart.Value.Date.ToString("dd.MM.yyyy")}', 104), CONVERT(date, '{lesson.DateEnd.Value.Date.ToString("dd.MM.yyyy")}', 104), {lesson.Number}, '{lesson.Type}', '{lesson.Classroom}', '{GetIdByTitle(lesson.Day, "ref_DaysOfWeek")}', {GetIdByTitle(lesson.Teacher.Trim(), "tbl_Teachers")}, {GetIdByTitle(lesson.Lesson.Trim(), "ref_NamesOfLessons")})";
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
        private static void FillTeachers(List<LessonInfo> parsedLessons)
        {
            Authorize.connection.Open();
            var teachersList = new List<string>();
            var reader = new SqlCommand("SELECT * FROM tbl_Teachers", Authorize.connection).ExecuteReader();
            while (reader.Read())
            {
                teachersList.Add(reader["Title"].ToString());
            }
            Authorize.connection.Close();
            var uniqueTeachers = new List<string>();
            foreach (var lesson in parsedLessons)
                if (!teachersList.Contains(lesson.Teacher))
                    uniqueTeachers.Add(lesson.Teacher);
            foreach (var teacher in uniqueTeachers)
            {
                var insertCommand = "INSERT INTO tbl_Teachers " +
                    "(Title) " +
                    "VALUES " +
                    $"('{teacher}')";
                Authorize.connection.Open();
                new SqlCommand(insertCommand, Authorize.connection).ExecuteNonQuery();
                Authorize.connection.Close();
            }
        }
        private static void FillNamesOfLessons(List<LessonInfo> parsedLessons)
        {
            Authorize.connection.Open();
            var namesOfLessonsList = new List<string>();
            var reader = new SqlCommand("SELECT * FROM ref_NamesOfLessons", Authorize.connection).ExecuteReader();
            while (reader.Read())
            {
                namesOfLessonsList.Add(reader["Title"].ToString());
            }
            Authorize.connection.Close();
            var uniqueTeachers = new List<string>();
            foreach (var lesson in parsedLessons)
                if (!namesOfLessonsList.Contains(lesson.Teacher))
                    uniqueTeachers.Add(lesson.Teacher);
            foreach (var teacher in uniqueTeachers)
            {
                var insertCommand = "INSERT INTO tbl_Teachers " +
                    "(Title) " +
                    "VALUES " +
                    $"('{teacher}')";
                Authorize.connection.Open();
                new SqlCommand(insertCommand, Authorize.connection).ExecuteNonQuery();
                Authorize.connection.Close();
            }
        }
    }
}
