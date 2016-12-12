using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VkNet;
using VkNet.Model.RequestParams;
using VkSchelude.Utils;
using System.Text.RegularExpressions;
using VkSchelude.Types;

namespace VkSchelude
{
    class groupBot
    {
        private static string patternStart = @"((^! {0,1})|(^. {0,1})){1}";
        public static void Start()
        {
            while (true)
            {
                var dialogs = Authorize.vkGroup.Messages.GetDialogs(new MessagesDialogsGetParams
                {
                    Count = 200,
                    Unread = true
                });
                if (dialogs.TotalCount != 0)
                {
                    foreach (var dialog in dialogs.Messages)
                    {
                        if (Regex.IsMatch(dialog.Body, patternStart))
                        {
                            #region--Подготовка к работе
                            string inputMessageText = dialog.Body.ToLower();
                            if (Authorize.connection.State != System.Data.ConnectionState.Open)
                                Authorize.connection.Open();
                            string selectRights = DBHelper.GetInternalSQLRequest(1);
                            int currentRightsId = int.Parse(DBHelper.GetSingleObject(selectRights, new Dictionary<string, object> { { "@UserId", dialog.UserId } }).ToString());
                            string getHelperQuery = DBHelper.GetInternalSQLRequest(5);
                            string outString = String.Empty;
                            var listHelper = DBHelper.GetListObject(getHelperQuery, new Dictionary<string, object> { { "@CurrentRightsId", currentRightsId } });
                            if (inputMessageText.Contains('|'))
                                inputMessageText = inputMessageText.Substring(0, inputMessageText.IndexOf('|'));
                            inputMessageText = inputMessageText.Replace("! ", "!").Replace(". ", ".").Trim();
                            //inputMessageText = inputMessageText.Trim();
                            if (Regex.IsMatch(inputMessageText, patternStart + "(?>" + String.Join("|", listHelper.Select(i => i["Title"])) + "){1}"))
                            {
                                DateTime tryparse = new DateTime();
                                var itemMainCommand = DBHelper.GetObject("SELECT * FROM tbl_Helper WHERE Title LIKE '" +
                                    (Regex.Match(inputMessageText, patternStart + "(?>" + String.Join("|", listHelper.Select(i => i["Title"])) + "){1}")).Value.Substring(1)
                                    + "'");
                                List<DCustom> Responce = new List<Types.DCustom>();
                                List<string> partsOfMessage = inputMessageText.Split(' ').ToList();
                                var getOverloads = DBHelper.GetInternalSQLRequest(6);
                                var listOverloads = DBHelper.GetListObject(getOverloads, new Dictionary<string, object> {
                                    { "@CurrentRightsId", currentRightsId },
                                    { "@CommandTitle", (Regex.Match(inputMessageText, patternStart + "(?>" + String.Join("|",listHelper.Select(i => i["Title"])) + "){1}")).Captures[0].Value.Substring(1) }
                                });
                                #region---Команда без аргументов
                                if (partsOfMessage.Count == 1)
                                {
                                    if (!(bool)itemMainCommand["NeedOverload"])
                                    {
                                        if (itemMainCommand["Title"].ToString().Equals("расписание"))
                                        {
                                            tryparse = DateTime.Now.AddDays(1);
                                            itemMainCommand.Id = 2;
                                            var a = itemMainCommand["SelectCommand"].ToString() + " " + itemMainCommand["WhereCommand"].ToString();
                                            Responce = DBHelper.GetListObject(itemMainCommand["SelectCommand"].ToString() + " " + itemMainCommand["WhereCommand"].ToString(),
                                                new Dictionary<string, object>
                                                {
                                                //{"@Date", DateTime.Now.AddDays(1) },
                                                {"@Year", DateTime.Now.AddDays(1).Year },
                                                {"@Month", DateTime.Now.AddDays(1).Month },
                                                {"@Day", DateTime.Now.AddDays(1).Day },
                                                {"@DayOfWeek", (int)DateTime.Now.AddDays(1).DayOfWeek }
                                                });
                                            //outString = GetAnswerString(2, Responce, tryparse); // 2 потому что id расписания
                                        }
                                        if (itemMainCommand["Title"].ToString().Equals("преподы"))
                                        {
                                            itemMainCommand.Id = 6;
                                            Responce = DBHelper.GetListObject(itemMainCommand["SelectCommand"].ToString() + " WHERE IsReal = 1 ORDER BY FullName");
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception();
                                    }
                                }
                                #endregion
                                #region---Команда с аргументом, без перегрузок
                                else if (partsOfMessage.Count == 2)
                                {
                                    if (itemMainCommand["Title"].ToString().Equals("расписание"))
                                    {
                                        int? difference = null;
                                        itemMainCommand.Id = 2;
                                        if (DateTime.TryParse(partsOfMessage[1], out tryparse))
                                        {
                                            Responce = DBHelper.GetListObject(itemMainCommand["SelectCommand"].ToString() + " " + itemMainCommand["WhereCommand"].ToString(),
                                                new Dictionary<string, object>
                                                {
                                                //{"@Date", DateTime.Parse(partsOfMessage[1]) },
                                                {"@Year", tryparse.Year },
                                                {"@Month", tryparse.Month },
                                                {"@Day", tryparse.Day },
                                                {"@DayOfWeek", DateTime.Parse(partsOfMessage[1]).DayOfWeek }
                                                });
                                            //outString = GetAnswerString(itemMainCommand.Id, Responce, tryparse); // 2 потому что id расписания
                                        }
                                        else if (Regex.IsMatch(partsOfMessage[1], "[а-яА-ЯёЁ]+"))
                                        {

                                            string argumentText = partsOfMessage[1].ToLower();
                                            if (argumentText.Equals("сегодня"))
                                            {
                                                tryparse = DateTime.Now;
                                                Responce = DBHelper.GetListObject(itemMainCommand["SelectCommand"].ToString() + " " + itemMainCommand["WhereCommand"].ToString(),
                                                new Dictionary<string, object>
                                                {{"@Year", DateTime.Now.Year },
                                                {"@Month", DateTime.Now.Month },
                                                {"@Day", DateTime.Now.Day },
                                                {"@DayOfWeek", DateTime.Now.DayOfWeek }
                                                });
                                            }
                                            else if (argumentText.Equals("завтра"))
                                            {
                                                difference = 1;
                                            }
                                            else if (argumentText.Equals("послезавтра"))
                                            {
                                                difference = 2;
                                            }
                                            else
                                            {
                                                var listDays = DBHelper.GetDictionary("SELECT Id, Title FROM ref_DaysOfWeek");
                                                if (listDays.Any(i => i.Value.ToString().Equals(argumentText)))
                                                {
                                                    int dateNumber = (int)listDays.Single(i => i.Value.ToString().Equals(argumentText)).Key;
                                                    difference = ((int)DateTime.Now.DayOfWeek) > dateNumber ? ((int)DateTime.Now.DayOfWeek) - dateNumber : dateNumber - ((int)DateTime.Now.DayOfWeek);
                                                    if (difference == 0)
                                                        difference = 7;
                                                }

                                            }
                                        }
                                        if (difference != null)
                                        {
                                            tryparse = DateTime.Now.AddDays((int)difference);
                                            Responce = DBHelper.GetListObject(itemMainCommand["SelectCommand"].ToString() + " " + itemMainCommand["WhereCommand"].ToString(),
                                        new Dictionary<string, object>
                                        {{"@Year", DateTime.Now.AddDays((int)difference).Year },
                                                {"@Month", DateTime.Now.AddDays((int)difference).Month },
                                                {"@Day", DateTime.Now.AddDays((int)difference).Day },
                                                {"@DayOfWeek", DateTime.Now.AddDays((int)difference).DayOfWeek }
                                        });
                                        }
                                        else
                                                    if (Responce.Count == 0)
                                            outString = "Входная строка имела неверный формат";
                                        else
                                            outString = "Входная строка имела неверный формат";
                                    }
                                    if (itemMainCommand["Title"].ToString().Equals("преподы"))
                                    {
                                        itemMainCommand.Id = 6;
                                        Responce = DBHelper.GetListObject(itemMainCommand["SelectCommand"].ToString() + " " + itemMainCommand["WhereCommand"].ToString(),
                                            new Dictionary<string, object> { { "@FName", partsOfMessage[1] + "%" } });
                                    }
                                }
                                #endregion
                                #region---Команда с перегрузками, без аргументов
                                else
                                    for (int i = 1; i < partsOfMessage.Count; i++)
                                    {
                                        //if (Regex.IsMatch(partsOfMessage, ""))
                                        {

                                        }
                                    }
                                #endregion
                                outString = Helper.GetAnswerString(itemMainCommand.Id, Responce, tryparse); // 2 потому что id расписания
                            }
                            #endregion

                            if (String.IsNullOrEmpty(outString))
                                outString = "Запрос не дал результатов";
                            Send.SendInMessages(Authorize.vkGroup, outString, dialog.UserId);
                        }
                    }
                }
                Thread.Sleep(5000);
            }
        }
    }
}
