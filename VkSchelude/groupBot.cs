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
using VkSchelude.Utils;

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
                            if (Regex.IsMatch(inputMessageText, patternStart + "[" + String.Join("|",listHelper.Select(i => i["CommandName"])) + "]{1}"))
                            {
                                List<string> partsOfMessage = inputMessageText.Split(' ').ToList();
                                for(int i = 1; i < partsOfMessage.Count; i++)
                                {
                                    //if (Regex.IsMatch(partsOfMessage, ""))
                                    {

                                    }
                                }
                            }
                            #endregion
                            #region !помощь
                            //if (Regex.IsMatch(dialog.Body, patternStart))
                            //{
                            //    foreach (var itemHelper in listHelper)
                            //    {
                            //        outString += $"!{itemHelper.Key} - {itemHelper.Value}\n";
                            //    }
                            //}
                            #endregion
                            if (!String.IsNullOrEmpty(outString))
                                Send.SendInMessages(Authorize.vkGroup, outString, dialog.UserId);
                        }
                        //if (dialog.Body.Contains("&"))
                        //{
                        //    if (dialog.Body.Contains("&расписание"))
                        //    {
                        //        var dates = dialog.Body.Replace("&расписание", "").Trim();
                        //        var parsedDates = dates.Split(',').ToList();
                        //        foreach (var date in parsedDates)
                        //        {
                        //            var message = Schedule.buildSchedule(date);
                        //            if (message != String.Empty)
                        //                Send.SendInMessages(Authorize.vkGroup, message, dialog.UserId);
                        //        }
                        //    }
                        //}
                    }
                }
                Thread.Sleep(10000);
            }
        }
    }
}
