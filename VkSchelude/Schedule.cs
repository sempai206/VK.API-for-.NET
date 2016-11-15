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

namespace VkSchelude
{
    class Schedule
    {
        //static List<string> authData = System.IO.File.ReadAllLines("authData.txt").ToList();
        //static VkApi vkScheludePost = new VkApi();
        //static int groupId = int.Parse(authData[3]);
        //[DllImport("user32.dll")]
        //private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        //[DllImport("kernel32.dll", ExactSpelling = true)]
        //private static extern IntPtr GetConsoleWindow();
        public static void Start()
        {
            //buildSchedule(DateTime.Now.AddDays(1).Date.ToString()); //test
            //SendOnWall(Authorize.AuthUser(), buildSchedule(DateTime.Now.AddDays(1).Date.ToString()));
            while (true)
            {
                if (DateTime.Now.Hour == 20)
                {
                    SendOnWall(Authorize.vkUser, buildSchedule(DateTime.Now.AddDays(1).Date.ToString()));
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
        public static void SendOnWall(VkApi vk, string message)
        {
            long groupId = -128223029;
            var postIdSchedule = vk.Wall.Post(new WallPostParams
            {
                OwnerId = groupId,
                FriendsOnly = false,
                FromGroup = true,
                Message = message
            });
            if (postIdSchedule != 0)
                Console.WriteLine(String.Format("{0} - Расписание на {1} успешно размещено", DateTime.Now, DateTime.Now.AddDays(1).Date));
            else
                Console.WriteLine(String.Format("{0} - Ошибка!Расписание на {1} не было размещено", DateTime.Now, DateTime.Now.AddDays(1).Date));

            //if (vkScheludePost.Wall.Pin(postIdSchedule) == true)
            //    Console.WriteLine(String.Format("{0} - Расписание на {1} успешно закреплено", DateTime.Now, DateTime.Now.AddDays(1).Date));
            //else
            //    Console.WriteLine(String.Format("{0} - Ошибка!Расписание на {1} не было закреплено", DateTime.Now, DateTime.Now.AddDays(1).Date));
        }
        public static void SendInMessages(VkApi vk, string message, long? userId)
        {
            var postId = vk.Messages.Send(new MessagesSendParams
            {
                UserId = userId,
                Message = message,
            });
            if (postId != 0)
                Console.WriteLine(String.Format("{0} - Расписание пользователю успешно отправлено", DateTime.Now));
            else
                Console.WriteLine(String.Format("{0} - Ошибка!Расписание пользователю не было отправлено", DateTime.Now));
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

