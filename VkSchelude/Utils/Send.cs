using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNet;
using VkNet.Model.RequestParams;

namespace VkSchelude.Utils
{
    class Send
    {
        public static void SendOnWall(VkApi vk, string message)
        {
            if (message == String.Empty)
                return;
            var postIdSchedule = vk.Wall.Post(new WallPostParams
            {
                OwnerId = Authorize.groupId,
                FriendsOnly = false,
                FromGroup = true,
                Message = message
            });
            if (postIdSchedule != 0)
                Log.Logging($"Расписание на {DateTime.Now.AddDays(1).ToString("dd.MM.yyyy")} успешно размещено");
            else
                Log.Logging($"Ошибка!Расписание на {DateTime.Now.AddDays(1).ToString("dd.MM.yyyy")} не было размещено");
        }
        public static void SendInMessages(VkApi vk, string message, long? userId)
        {
            if (message == String.Empty)
                return;
            var userInfo = Authorize.vkUser.Users.Get((long)userId);
            long postId = 0;
            try
            {
                postId = vk.Messages.Send(new MessagesSendParams
                {
                    UserId = userId,
                    Message = message,
                });
            }
            catch
            {
                postId = vk.Messages.Send(new MessagesSendParams
                {
                    UserId = userId,
                    Message = "ОШИБКА: Превышен интервал запроса. Попробуйте позже.",
                });
            }
            if (postId != 0)
                Log.Logging($"Сообщение пользователю {userInfo.FirstName + " " + userInfo.LastName} успешно отправлено");
            else
                Log.Logging($"Ошибка!Сообщение пользователю {userInfo.FirstName + " " + userInfo.LastName} не было отправлено");
        }
    }
}
