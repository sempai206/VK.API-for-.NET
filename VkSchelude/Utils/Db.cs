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
            //FillTeachers(parsedLessons);
            //FillNamesOfLessons(parsedLessons);
            new SqlCommand(DBHelper.GetInternalSQLRequest(9), Authorize.connection).ExecuteNonQuery();
            foreach (var lesson in parsedLessons)
            {
                if (lesson.DateStart == null || lesson.DateEnd == null)
                    continue;
                var insertCommand = DBHelper.GetInternalSQLRequest(14);
                insertCommand.Replace("@DateStart", $"{lesson.DateStart.Value.Date}");
                insertCommand.Replace("@DateEnd", $"{lesson.DateEnd.Value.Date}");
                insertCommand.Replace("@Number", $"{lesson.Number}");
                insertCommand.Replace("@TypeOfLesson", $"{lesson.Type}");
                insertCommand.Replace("@Classroom", $"{lesson.Classroom}");
                insertCommand.Replace("@DayOfWeek", $"{GetIdByTitle(lesson.Day.Trim(), "ref_DaysOfWeek").ToString()}");
                insertCommand.Replace("@TeacherId", $"{GetIdByTitle(lesson.Teacher.Trim(), "tbl_Teachers")}");
                insertCommand.Replace("@LessonNameId", $"{GetIdByTitle(lesson.Lesson.Trim(), "ref_NamesOfLessons")}");
                new SqlCommand(insertCommand, Authorize.connection).ExecuteNonQuery();
            }
            Log.Logging("Расписание успешно занесено в БД");
        }
        private static int GetIdByTitle(string title, string table)
        {
            //var request = DBHelper.GetInternalSQLRequest(10);
            //request.Replace("@Table", table);
            //request.Replace("@Title", title);
            //return (int)DBHelper.GetObject(request)["Id"];
            return (int)DBHelper.GetObject(DBHelper.GetInternalSQLRequest(10), new Dictionary<string, object>
            {
                { "@Table", table },
                { "@Title", title }
            })["Id"];
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
                    var insertCommand = DBHelper.GetInternalSQLRequest(12);
                    insertCommand.Replace("@Table", "tbl_Teachers");
                    insertCommand.Replace("@Value", teacher);
                    new SqlCommand(insertCommand, Authorize.connection).ExecuteNonQuery();
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
                    var insertCommand = DBHelper.GetInternalSQLRequest(12);
                    insertCommand.Replace("@Table", "ref_NamesOfLessons");
                    insertCommand.Replace("@Value", lesson);
                    new SqlCommand(insertCommand, Authorize.connection).ExecuteNonQuery();
                }
            }
        }
    }
}