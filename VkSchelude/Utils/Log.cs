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
            Console.WriteLine(message);
            //if (!File.Exists("Log.txt"))
            //{
            //    File.Create("Log.txt");
            //}
            File.AppendAllText("Log.txt", message + "\n");
        }
    }
}
