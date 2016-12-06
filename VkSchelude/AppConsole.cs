using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VkSchelude.Utils;

namespace VkSchelude
{
    class AppConsole
    {
        public static void Start()
        {
            bool _working = true;
            while (_working)
            {
                var command = Console.ReadLine();
                if (command.Equals("help"))
                {
                    Log.Logging("Запрос справки..");
                    getConsoleHelper();
                }
                else if (command.Equals("auth"))
                {
                    Authorize.Auth();
                }
                else if (command.Equals("autoschedule start"))
                {
                    if (Authorize.vkUserReq)
                    {
                        if (Program.tomorrowScheludeThread.ThreadState == ThreadState.Unstarted || Program.tomorrowScheludeThread.ThreadState == ThreadState.Stopped)
                        {
                            Program.tomorrowScheludeThread.Start();
                            Log.Logging("Модуль авторасписания запущен");
                        }
                        else if (Program.tomorrowScheludeThread.ThreadState == ThreadState.Running)
                            Log.Logging("Модуль авторасписания уже запущен");
                    }
                    else
                    {
                        Log.Logging("Невозможно подключить модуль авторасписания, т.к. отсутствует авторизация пользователя");
                    }
                }
                else if (command.Equals("autoschedule stop"))
                {
                    if (Program.tomorrowScheludeThread.ThreadState == ThreadState.Running || Program.tomorrowScheludeThread.ThreadState == ThreadState.WaitSleepJoin)
                    {
                        Program.tomorrowScheludeThread.Abort();
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
                        if (Program.vkBotThread.ThreadState == ThreadState.Unstarted || Program.vkBotThread.ThreadState == ThreadState.Stopped)
                        {
                            Program.vkBotThread.Start();
                            Log.Logging("Модуль бота запущен");
                        }
                        else if (Program.vkBotThread.ThreadState == ThreadState.Running)
                            Log.Logging("Модуль бота уже запущен");
                    }
                    else if (!Authorize.vkUserReq || !Authorize.vkGroupReq)
                    {
                        Log.Logging("Невозможно подключить модуль бота, т.к. отсутствует авторизация пользователя или группы");
                    }
                }
                else if (command.Equals("bot stop"))
                {
                    if (Program.vkBotThread.ThreadState == ThreadState.Running || Program.vkBotThread.ThreadState == ThreadState.WaitSleepJoin)
                    {
                        Program.vkBotThread.Abort();
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
                                            {"@Year", DateTime.Now.AddDays(1).Year },
                                            {"@Month", DateTime.Now.AddDays(1).Month },
                                            {"@Day", DateTime.Now.AddDays(1).Day },
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
                                        {"@Year", DateTime.Now.Year },
                                        {"@Month", DateTime.Now.Month },
                                        {"@Day", DateTime.Now.Day },
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
                    if (Program.vkBotThread.ThreadState == ThreadState.Running)
                    {
                        Program.vkBotThread.Suspend(); //Устаревший метод. Найти нормальный способ.
                        //new Document(plusConsole).Parse(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + path);
                        Log.Logging("Занесение парсенного расписания в БД..");                         
                        DBHelper.FillTables(new Document(plusConsole).Parse(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + path));
                        Program.vkBotThread.Resume();
                    }
                    else
                        DBHelper.FillTables(new Document(plusConsole).Parse(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + path));
                }
                else if (command.Equals("exit"))
                {
                    Program.tomorrowScheludeThread.Abort();
                    Program.vkBotThread.Abort();
                    _working = false;
                }
            }
        }
        private static void getConsoleHelper()
        {
            var commandsList = DBHelper.GetListObject(DBHelper.GetInternalSQLRequest(8).Replace("@Table", "sys_ConsoleHelper"));
            Console.WriteLine("Команды для консоли:");
            foreach (var command in commandsList)
            {
                Console.WriteLine($"{command["CommandTitle"]} - {command["Description"]}");
            }
        }
    }
}
