using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model.RequestParams;
using System.Windows.Forms;

namespace VkSchelude
{
    class Program
    {
        static VkApi vk = new VkApi();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();
        static void Main(string[] args)
        {
            Thread commands = new Thread(readCommand);
            commands.Start();
            vk.Authorize(accessToken);
            //vk.Authorize(new ApiAuthParams
            //{
            //    ApplicationId = appID,
            //    Login = email,
            //    Password = pass,
            //    Settings = scope
            //});
            postSchedule();
            //while (true)
            //{
            //    if (DateTime.Now.Hour == 20)
            //    {
            //        postSchedule();
            //    }
            //    Thread.Sleep(1800000);
            //}
            Console.ReadKey();
        }
        public static void postSchedule()
        {
            if (DateTime.Now.AddDays(1).DayOfWeek == DayOfWeek.Sunday)
                return;
            var message = String.Empty;
            List<Period> schedule = new List<Period>();
            schedule.Add(new Period()
            {
                periodName = "Теория трансляции",
                periodNum = 2,
                periodPlace = "УК.1",
                teacherName = "Сивков"
            });
            schedule.Add(new Period()
            {
                periodName = "Теория трансляции",
                periodNum = 3,
                periodPlace = "УК.1",
                teacherName = "Сивков"
            });
            schedule.Add(new Period()
            {
                periodName = "Беспроводные технологии",
                periodNum = 4,
                periodPlace = "УК.3",
                teacherName = "Денисов"
            });
            //List<Schedule> schedule = getTomorrowSchedule(); //нужно отсортировать по последовательности пар
            message += String.Format("Расписание на {0}:\n", DateTime.Now.AddDays(1).Date);
            foreach (var item in schedule)
            {
                message += String.Format("{0} пара: {1}, преподаватель {2}, ауд. {3}\n", item.periodNum, item.periodName, item.teacherName, item.periodPlace);
            }
            var postIdSchedule = vk.Wall.Post(new WallPostParams
            {
                FriendsOnly = false,
                FromGroup = true,
                Message = message
            });
            if (postIdSchedule != 0)
                Console.WriteLine(String.Format("{0} - Расписание на {1} успешно размещено", DateTime.Now, DateTime.Now.AddDays(1).Date));
            else
                Console.WriteLine(String.Format("{0} - Ошибка!Расписание на {1} не было размещено", DateTime.Now, DateTime.Now.AddDays(1).Date));

            if (vk.Wall.Pin(postIdSchedule) == true)
                Console.WriteLine(String.Format("{0} - Расписание на {1} успешно закреплено", DateTime.Now, DateTime.Now.AddDays(1).Date));
            else
                Console.WriteLine(String.Format("{0} - Ошибка!Расписание на {1} не было закреплено", DateTime.Now, DateTime.Now.AddDays(1).Date));
        }
        private static List<Period> getTomorrowSchedule()
        {
            var connectionString = ""; //строка подключения
            var selectSchedule = ""; //здесь будет строка запроса расписания из базы
            var connection = new SqlConnection(connectionString);
            var cmdSchedule = new SqlCommand(selectSchedule, connection);
            connection.Open();
            var reader = cmdSchedule.ExecuteReader();
            List<Period> result = new List<Period>();
            while (reader.Read())
            {
                var period = new Period();
                //здесь будет заполнение объекта класса Schedule
                result.Add(period);
            }
            reader.Close();
            connection.Close();
            return result;
        }
        static void readCommand()
        {
            while (true)
            {
                var command = Console.ReadLine();
                if (command.Equals("schedule"))
                    postSchedule();
                else if (command.Equals("min"))
                {
                    ShowWindow(GetConsoleWindow(), 0); // Скрыть.
                    var icon = new NotifyIcon();
                    icon.Icon = new Icon("icon.ico");
                    icon.Visible = true;
                    icon.MouseClick += new MouseEventHandler(notifyIcon_DoubleClick);
                }
            }
        }
        private static void notifyIcon_DoubleClick(object Sender, EventArgs e)
        {
            ShowWindow(GetConsoleWindow(), 1);
            //icon.Visible = false;
        }
    }
    class Period
    {
        public int periodNum { get; set; }
        public string periodName { get; set; }
        public string teacherName { get; set; }
        public string periodPlace { get; set; }

    }
}
