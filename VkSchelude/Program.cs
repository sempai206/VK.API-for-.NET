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
using VkSchelude.Utils;
using System.Text.RegularExpressions;

namespace VkSchelude
{
    class Program
    {
        public static System.Threading.Timer time = new System.Threading.Timer(new TimerCallback(AutoSchedule.Start), new Object(), Convert.ToInt32(Helper.test()), 86400 * 1000);
        //public static Thread tomorrowScheludeThread = new Thread(AutoSchedule.Start);
        public static Thread vkBotThread = new Thread(groupBot.Start);
        static void Main(string[] args)
        {
            Authorize.Auth(); //Авторизация по входе
            #region Запуск модулей
            if (Authorize.vkUserReq && !args.Contains("-noautoschedule"))
            {
                //System.Threading.Timer time = new System.Threading.Timer(new TimerCallback(AutoSchedule.Start), new Object(), Convert.ToInt32(Helper.test()), 86400 * 1000)
                //tomorrowScheludeThread.Start();
                Log.Logging("Модуль авторасписания запущен");
            }
            else
            {
                time.Dispose();
                Log.Logging("Модуль авторасписания отключен");
            }
            if (!Authorize.vkUserReq || !Authorize.vkGroupReq || args.Contains("-nobot"))
            {
                Log.Logging("Модуль бота отключен");
            }
            else if (Authorize.vkUserReq && Authorize.vkGroupReq)
            {
                vkBotThread.Start();
                Log.Logging("Модуль бота запущен");
            }
            if (!Authorize.DBReq)
                Console.WriteLine("Команда help недоступна, т.к. отсутствует подключение к БД");
            else
                Console.WriteLine("Используйте команду help для получения справки по командам консоли");
            #endregion
            AppConsole.Start();
        }
    }
}
