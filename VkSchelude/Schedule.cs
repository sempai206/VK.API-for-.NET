using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model.RequestParams;
using VkSchelude.Types;
using VkSchelude.Utils;

namespace VkSchelude
{
    class Schedule
    {
        public static void Start()
        {
            while (true)
            {
                if (DateTime.Now.Hour == 20 && CheckWallForSchedule(DateTime.Now.AddDays(1).Date.ToString("dd.MM.yyyy")))
                {
                    Send.SendOnWall(Authorize.vkUser, buildSchedule(DateTime.Now.AddDays(1).Date.ToString("dd.MM.yyyy")));
                }
                Thread.Sleep(600000);
            }
        }
        public static string buildSchedule(string date)
        {
            List<LessonInfo> schedule = getSchedule(date);
            if (schedule.Count == 0)
                return String.Empty;
            var message = $"Расписание на {date}:\n";
            foreach (var item in schedule)
            {
                message += $"{item.Number} пара: {item.Lesson} - {item.Type}, преподаватель {item.Teacher}, ауд. {item.Classroom}\n";
            }
            return message;
        }
        private static List<LessonInfo> getSchedule(string date)
        {
            var selectSchedule = "SELECT Lessons.Number, Lessons.TypeOfLesson, Lessons.Classroom, NamesOfLessons.Title AS Lesson, Teachers.Title AS Teacher " +
                                 "FROM Lessons INNER JOIN " +
                                    "Teachers ON Lessons.TeacherId = Teachers.Id INNER JOIN " +
                                    "NamesOfLessons ON Lessons.LessonNameId = NamesOfLessons.Id " +
                                 $"WHERE DateFrom <= CONVERT(date, '{date}', 104) AND DateTo >= CONVERT(date, '{date}', 104) AND Lessons.DayOfWeek = {(int)DateTime.Parse(date).DayOfWeek} " + 
                                 "ORDER BY Number";
            Authorize.connection.Open();
            var reader = new SqlCommand(selectSchedule, Authorize.connection).ExecuteReader();
            List<LessonInfo> result = new List<LessonInfo>();
            while (reader.Read())
            {
                var lesson = new LessonInfo();
                lesson.Lesson = reader["Lesson"].ToString();
                lesson.Type = reader["TypeOfLesson"].ToString();
                lesson.Number = int.Parse(reader["Number"].ToString());
                lesson.Teacher = reader["Teacher"].ToString();
                lesson.Classroom = reader["Classroom"].ToString();
                result.Add(lesson);
            }
            reader.Close();
            Authorize.connection.Close();
            return result;
        }
        public static bool CheckWallForSchedule(string date)
        {
            var wallPosts = Authorize.vkUser.Wall.Get(new WallGetParams
            {
                OwnerId = Authorize.groupId,
                Count = 20,
                Filter = VkNet.Enums.SafetyEnums.WallFilter.Owner,
            });
            foreach (var post in wallPosts.WallPosts)
            {
                if (post.Text.Contains($"Расписание на {date}"))
                {
                    return false;
                }
            }
            return true;
        }
    }
}

