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
        private string patternStart = @"^!";
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
                        if (Regex.IsMatch(dialog.Body, @"^!"))
                        {
                            if (Authorize.connection.State != System.Data.ConnectionState.Open)
                                Authorize.connection.Open();
                            string selectRights = DBHelper.GetInternalSQLRequest(1);
                            int currentRightsId = int.Parse(DBHelper.GetSingleObject(selectRights, new Dictionary<string, object> { { "@UserId", dialog.UserId } }).ToString());
                            selectRights = DBHelper.GetInternalSQLRequest(5);
                            var aa = DBHelper.GetDictionary(selectRights, new Dictionary<string, object> { { "@CurrentRightsId", currentRightsId } });
                            string outString = String.Empty;
                            foreach(var item in aa)
                            {
                                outString += $"!{item.Key} - {item.Value}\n";
                            }
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
