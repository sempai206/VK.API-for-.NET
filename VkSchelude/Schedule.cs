﻿using System;
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
                if (DateTime.Now.Hour == 20)
                {
                    Send.SendOnWall(Authorize.vkUser, buildSchedule(DateTime.Now.AddDays(1).Date.ToString("dd.MM.yyyy")));
                }
                Thread.Sleep(600000);
            }
        }
        public static string buildSchedule(string date)
        {
            if (DateTime.Parse(date).DayOfWeek == DayOfWeek.Sunday)
                return String.Empty;
            var message = String.Empty;
            List<LessonInfo> schedule = new List<LessonInfo>();
            schedule.Add(new LessonInfo()
            {
                Lesson = "Теория трансляции",
                Number = 2,
                Classroom = "УК.1",
                Teacher = "Сивков"
            });
            schedule.Add(new LessonInfo()
            {
                Lesson = "Теория трансляции",
                Number = 3,
                Classroom = "УК.1",
                Teacher = "Сивков"
            });
            schedule.Add(new LessonInfo()
            {
                Lesson = "Беспроводные технологии",
                Number = 4,
                Classroom = "УК.3",
                Teacher = "Денисов"
            });
            //List<LessonInfo> schedule = getSchedule(date); //нужно отсортировать по последовательности пар
            message += String.Format("Расписание на {0}:\n", date);
            foreach (var item in schedule)
            {
                message += String.Format("{0} пара: {1}, преподаватель {2}, ауд. {3}\n", item.Number, item.Lesson, item.Teacher, item.Classroom);
            }
            return message;
        }
        private static List<LessonInfo> getSchedule(string date)
        {
            var connectionString = ""; //строка подключения
            var selectSchedule = ""; //здесь будет строка запроса расписания из базы
            var connection = new SqlConnection(connectionString);
            var cmdSchedule = new SqlCommand(selectSchedule, connection);
            connection.Open();
            var reader = cmdSchedule.ExecuteReader();
            List<LessonInfo> result = new List<LessonInfo>();
            while (reader.Read())
            {
                var lesson = new LessonInfo();
                //здесь будет заполнение объекта класса Schedule
                result.Add(lesson);
            }
            reader.Close();
            connection.Close();
            return result;
        }
    }
}

