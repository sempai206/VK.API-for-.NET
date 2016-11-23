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
        private static string patternStart = @"^!";
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
                            string inputMessageText = dialog.Body;
                            if (Authorize.connection.State != System.Data.ConnectionState.Open)
                                Authorize.connection.Open();
                            string selectRights = DBHelper.GetInternalSQLRequest(1);
                            int currentRightsId = int.Parse(DBHelper.GetSingleObject(selectRights, new Dictionary<string, object> { { "@UserId", dialog.UserId } }).ToString());
                            string getHelperQuery = DBHelper.GetInternalSQLRequest(5);
                            string outString = String.Empty;
                            var listHelper = DBHelper.GetListObject(getHelperQuery, new Dictionary<string, object> { { "@CurrentRightsId", currentRightsId } });
                            if (inputMessageText.Contains('|'))
                                inputMessageText = inputMessageText.Substring(0, inputMessageText.IndexOf('|'));
                            inputMessageText = inputMessageText.Trim();
                            if (Regex.IsMatch(inputMessageText, patternStart + "(?>" + String.Join("|", listHelper.Select(i => i["Title"])) + "){1}"))
                            {
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
                                if (partsOfMessage.Count == 1)
                                {
                                    if (!(bool)itemMainCommand["NeedOverload"])
                                    {
                                        if (itemMainCommand["Title"].ToString().Equals("расписание"))
                                        {
                                            Responce = DBHelper.GetListObject(itemMainCommand["SelectCommand"].ToString() + " " + itemMainCommand["WhereCommand"].ToString(),
                                                new Dictionary<string, object>
                                                {
                                                {"@Date", DateTime.Now.AddDays(1) },
                                                {"@DayOfWeek", (int)DateTime.Now.AddDays(1).DayOfWeek }
                                                });
                                            outString = GetAnswerString(2, Responce, DateTime.Now.AddDays(1)); // 2 потому что id расписания
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception();
                                    }
                                }
                                else if (partsOfMessage.Count == 2)
                                {
                                    if (itemMainCommand["Title"].ToString().Equals("расписание"))
                                    {
                                        DateTime tryparse = new DateTime();
                                        if (DateTime.TryParse(partsOfMessage[1], out tryparse))
                                        {
                                            Responce = DBHelper.GetListObject(itemMainCommand["SelectCommand"].ToString() + " " + itemMainCommand["WhereCommand"].ToString(),
                                                new Dictionary<string, object>
                                                {
                                                {"@Date", DateTime.Parse(partsOfMessage[1]) },
                                                {"@DayOfWeek", DateTime.Parse(partsOfMessage[1]).DayOfWeek }
                                                });
                                            outString = GetAnswerString(2, Responce, DateTime.Parse(partsOfMessage[1])); // 2 потому что id расписания
                                        }
                                        else
                                            outString = "Входная строка имела неверный формат";
                                    }
                                }
                                else
                                    for (int i = 1; i < partsOfMessage.Count; i++)
                                    {
                                        //if (Regex.IsMatch(partsOfMessage, ""))
                                        {

                                        }
                                    }
                            }
                            #endregion
                            // тут обработать - записывать в outstring то, что высчиталось выше
                            if (String.IsNullOrEmpty(outString))
                                outString = "Запрос не дал результатов";
                            Send.SendInMessages(Authorize.vkGroup, outString, dialog.UserId);
                        }
                    }
                }
                Thread.Sleep(10000);
            }
        }
        private static string GetAnswerString(int modelId, List<DCustom> inputData, DateTime? lessonsDate = null)
        {
            string answerString = "Запрос не дал результатов";
            switch (modelId)
            {
                case 2: // расписание
                    answerString = String.Empty;
                    if (inputData.Count == 0)
                        answerString = $"На указанный день ({((DateTime)lessonsDate).ToShortDateString()}) занятий не найдено";
                    else
                        answerString = $"Расписание на {((DateTime)lessonsDate).ToShortDateString()} ({inputData.First()["DayOfWeek"]}):\n";
                    foreach(var item in inputData)
                    {
                        answerString += $"{item["Number"]} - {item["NameOfLesson"]} ({item["TypeOfLesson"]}) - {item["TeacherName"]} ({item["Classroom"]})\n";
                    }
                    break;
            }
            return answerString;
        }
    }
}
