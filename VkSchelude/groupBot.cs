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
                            switch (dialog.Body)
                            {
                                case "&Расписание на завтра":
                                    Send.SendInMessages(Authorize.vkGroup, Schedule.buildSchedule(DateTime.Now.AddDays(1).ToString("dd.MM.yyyy")), dialog.UserId);
                                    break;

                                case "&test":
                                    Send.SendInMessages(Authorize.vkGroup, Schedule.buildSchedule(DateTime.Now.ToString("dd.MM.yyyy")), dialog.UserId);
                                    break;
                            }
                        }
                    }
                }
                Thread.Sleep(1000);
            }
        }
    }
}
