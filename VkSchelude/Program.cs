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
        static void Main(string[] args)
        {
            Thread tomorrowScheludeThread = new Thread(Schedule.Start);
            tomorrowScheludeThread.Start();
            while (true)
            {
                var command = Console.ReadLine();
                //if (command.Equals("schedule"))
                //    Schedule.postSchedule(DateTime.Now.AddDays(1).Date.ToString());
                //else if (command.Equals("schedule today"))
                //    Schedule.postSchedule(DateTime.Now.Date.ToString());
                else if (command.Equals("parse"))
                    new Document().Parse(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\rasp.xls");
            }
        }
    }
}
