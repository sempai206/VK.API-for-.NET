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

namespace VkSchelude
{
    class Program
    {
        static void Main(string[] args)
        {
            Authorize.setAuthorize();
            //Thread tomorrowScheludeThread = new Thread(Schedule.Start);
            //tomorrowScheludeThread.Start();
            Thread vkBotThread = new Thread(groupBot.Start);
            vkBotThread.Start();
            while (true)
            {
                var command = Console.ReadLine();
                if (command.Equals("schedule"))
                {
                    //Schedule.buildSchedule(DateTime.Now.AddDays(1).Date.ToString());
                    Send.SendOnWall(Authorize.vkUser, Schedule.buildSchedule(DateTime.Now.AddDays(1).Date.ToString("dd.MM.yyyy")));
                }
                else if (command.Equals("schedule today"))
                    Send.SendOnWall(Authorize.vkUser, Schedule.buildSchedule(DateTime.Now.Date.ToString("dd.MM.yyyy")));
                else if (command.Equals("parse"))
                    new Document().Parse(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\rasp.xls");
            }
        }
    }
}
