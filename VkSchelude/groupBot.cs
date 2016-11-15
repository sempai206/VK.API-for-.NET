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
        static VkApi vkBot = new VkApi();
        public void Start()
        {
            string[] readText = System.IO.File.ReadAllLines("authData.txt");
            string groupAccessToken = readText[readText.Length];
            vkBot.Authorize(groupAccessToken);
            while (true)
            {
                var dialogs = vkBot.Messages.GetDialogs(new MessagesDialogsGetParams
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
                                    vkBot.Messages.Send(new MessagesSendParams
                                    {
                                        UserId = dialog.UserId,
                                    });
                                    break;
                            }
                        }
                    }
                }
                Thread.Sleep(60000);
            }
        }
    }
}
