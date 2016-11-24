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
        private static bool _working = true;
        static void Main(string[] args)
        {
            Authorize.Auth();
            Thread tomorrowScheludeThread = new Thread(Schedule.Start);
            if (Authorize.vkUserReq)
            {
                tomorrowScheludeThread.Start();
                Log.Logging("Модуль авторасписания запущен");
            }
            else
            {
                Log.Logging("Модуль авторасписания отключен");
            }
            Thread vkBotThread = new Thread(groupBot.Start);
            if (!Authorize.vkUserReq || !Authorize.vkGroupReq)
            {
                Log.Logging("Модуль бота отключен");
            }
            else if (Authorize.vkUserReq && Authorize.vkGroupReq)
            {
                vkBotThread.Start();
                Log.Logging("Модуль бота запущен");
            }
            while (_working)
            {
                var command = Console.ReadLine();
                if (command.Equals("auth"))
                {
                    Authorize.Auth();
                }
                else if (command.Equals("autoschedule start"))
                {
                    if (Authorize.vkUserReq)
                    {
                        if (tomorrowScheludeThread.ThreadState == ThreadState.Unstarted || tomorrowScheludeThread.ThreadState == ThreadState.Stopped)
                        {
                            tomorrowScheludeThread.Start();
                            Log.Logging("Модуль авторасписания запущен");
                        }
                        else if (tomorrowScheludeThread.ThreadState == ThreadState.Running)
                            Log.Logging("Модуль авторасписания уже запущен");
                    }
                    else
                    {
                        Log.Logging("Невозможно подключить модуль авторасписания, т.к. отсутствует авторизация пользователя");
                    }
                }
                else if (command.Equals("autoschedule stop"))
                {
                    if (tomorrowScheludeThread.ThreadState == ThreadState.Running || tomorrowScheludeThread.ThreadState == ThreadState.WaitSleepJoin)
                    {
                        tomorrowScheludeThread.Abort();
                        Log.Logging("Модуль авторасписания остановлен");
                    }
                    else
                    {
                        Log.Logging("Модуль авторасписания уже остановлен");
                    }
                }
                else if (command.Equals("bot start"))
                {
                    if (Authorize.vkUserReq && Authorize.vkGroupReq)
                    {
                        if (vkBotThread.ThreadState == ThreadState.Unstarted || vkBotThread.ThreadState == ThreadState.Stopped)
                        {
                            vkBotThread.Start();
                            Log.Logging("Модуль бота запущен");
                        }
                        else if (vkBotThread.ThreadState == ThreadState.Running)
                            Log.Logging("Модуль бота уже запущен");
                    }
                    else if (!Authorize.vkUserReq || !Authorize.vkGroupReq)
                    {
                        Log.Logging("Невозможно подключить модуль бота, т.к. отсутствует авторизация пользователя или группы");
                    }
                }
                else if (command.Equals("bot stop"))
                {
                    if (vkBotThread.ThreadState == ThreadState.Running || vkBotThread.ThreadState == ThreadState.WaitSleepJoin)
                    {
                        vkBotThread.Abort();
                        Log.Logging("Модуль бота остановлен");
                    }
                    else
                        Log.Logging("Модуль бота уже остановлен");
                }
                else if (command.Equals("schedule"))
                {
                    if (!Authorize.vkUserReq)
                        Log.Logging("Невозможно разместить расписание, т.к. отсутствует авторизация пользователя");
                    else
                        Send.SendOnWall(Authorize.vkUser,
                                        Helper.GetAnswerString(2, DBHelper.GetListObject(DBHelper.GetInternalSQLRequest(7), new Dictionary<string, object> {
                                            { "@Date", DateTime.Now.AddDays(1).Date },
                                            { "@DayOfWeek", (int)DateTime.Now.AddDays(1).DayOfWeek}
                                            }),
                                        DateTime.Now.AddDays(1).Date));
                }
                else if (command.Equals("schedule today"))
                {
                    if (!Authorize.vkUserReq)
                        Log.Logging("Невозможно разместить расписание, т.к. отсутствует авторизация пользователя");
                    else
                        Send.SendOnWall(Authorize.vkUser,
                                    Helper.GetAnswerString(2, DBHelper.GetListObject(DBHelper.GetInternalSQLRequest(7), new Dictionary<string, object> {
                                        { "@Date", DateTime.Now.Date },
                                        { "@DayOfWeek", (int)DateTime.Now.DayOfWeek}
                                        }),
                                    DateTime.Now.Date));
                }
                else if (Regex.IsMatch(command, "^parse"))
                {
                    bool plusConsole = false;
                    string path = "\\rasp.xls";
                    if (command.Contains("+console"))
                        plusConsole = true;
                    if (command.Contains("/filename"))
                    {
                        string fname = command.Substring(command.IndexOf("/filename") + "/filename".Length).Trim();
                        fname = fname.Substring(0, fname.IndexOf(' ') != -1 ? fname.IndexOf(' ') : fname.Length);
                        if (!Regex.IsMatch(fname, "[*\\|:<>?/\" ]"))
                        {
                            path = "\\" + fname;
                        }
                        else
                        {
                            Console.WriteLine("Ошибка: значение поля /filename определяется относительно папки рабочего стола текущего пользователя");
                        }
                    }
                    vkBotThread.Suspend();
                    //new Document(plusConsole).Parse(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + path);
                    Db.FillTableLessons(new Document(plusConsole).Parse(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + path));
                    vkBotThread.Resume();
                }
                else if (command.Equals("exit"))
                {
                    tomorrowScheludeThread.Abort();
                    vkBotThread.Abort();
                    _working = false;
                }
            }
        }
    }
}
