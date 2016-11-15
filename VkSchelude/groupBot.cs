using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VkNet;
using VkNet.Model.RequestParams;

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
                                //case "&Расписание на завтра":
                                //    vkBot.Messages.Send(new MessagesSendParams
                                //    {
                                //        UserId = dialog.UserId,
                                //    });
                                //    break;
                                case "&test":
                                    Schedule.SendInMessages(Authorize.vkGroup, Schedule.buildSchedule(DateTime.Now.ToString()), dialog.UserId);
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
