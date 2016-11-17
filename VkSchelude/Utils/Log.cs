using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkSchelude.Utils
{
    class Log
    {
        public static void Logging(string message)
        {
            Console.WriteLine($"{DateTime.Now} - {message}");
            File.AppendAllText("Log.txt", $"{DateTime.Now} - {message}{Environment.NewLine}");
        }
    }
}
