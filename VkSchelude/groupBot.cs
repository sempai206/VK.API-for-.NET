using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VkNet;
using VkNet.Model.RequestParams;
using VkSchelude.Utils;

namespace VkSchelude
{
    class groupBot
    {
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
                        if (dialog.Body.Contains("&"))
                        {
                            if (dialog.Body.Contains("&расписание"))
                            {
                                var dates = dialog.Body.Replace("&расписание", "").Trim();
                                var parsedDates = dates.Split(',').ToList();
                                foreach (var date in parsedDates)
                                {
                                    var message = Schedule.buildSchedule(date);
                                    if (message != String.Empty)
                                        Send.SendInMessages(Authorize.vkGroup, message, dialog.UserId);
                                }
                            }
                        }
                    }
                }
                Thread.Sleep(10000);
            }
        }
    }
}
