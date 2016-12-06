using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model.RequestParams;
using VkSchelude.Types;
using VkSchelude.Utils;

namespace VkSchelude
{
    class AutoSchedule
    {
        public static void Start()
        {
            while (true)
            {
                if (DateTime.Now.Hour == 20 && CheckWallForSchedule(DateTime.Now.AddDays(1).Date.ToString("dd.MM.yyyy")))
                {
                    var message = Helper.GetAnswerString(2, DBHelper.GetListObject(DBHelper.GetInternalSQLRequest(7), new Dictionary<string, object> {
                                    {"@Year", DateTime.Now.AddDays(1).Year },
                                    {"@Month", DateTime.Now.AddDays(1).Month },
                                    {"@Day", DateTime.Now.AddDays(1).Day },
                                    { "@DayOfWeek", (int)DateTime.Now.AddDays(1).DayOfWeek}
                                    }),
                                    DateTime.Now.AddDays(1).Date);
                    if (!message.Equals($"На указанный день ({DateTime.Now.AddDays(1).Date}) занятий не найдено"))
                        Send.SendOnWall(Authorize.vkUser, message);
                }
                Thread.Sleep(600000);
            }
        }
        private static bool CheckWallForSchedule(string date)
        {
            var wallPosts = Authorize.vkUser.Wall.Get(new WallGetParams
            {
                OwnerId = Authorize.groupId,
                Count = 20,
                Filter = VkNet.Enums.SafetyEnums.WallFilter.Owner,
            });
            foreach (var post in wallPosts.WallPosts)
            {
                if (post.Text.Contains($"Расписание на {date}") || post.Text.Contains($"На указанный день ({date}) занятий не найдено"))
                {
                    return false;
                }
            }
            return true;
        }
    }
}

