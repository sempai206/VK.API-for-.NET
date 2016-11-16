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
            long groupId = -128223029;
            var postIdSchedule = vk.Wall.Post(new WallPostParams
            {
                OwnerId = groupId,
                FriendsOnly = false,
                FromGroup = true,
                Message = message
            });
            if (postIdSchedule != 0)
                Log.Logging(String.Format("{0} - Расписание на {1} успешно размещено", DateTime.Now, DateTime.Now.AddDays(1).ToString("dd.MM.yyyy")));
            else
                Log.Logging(String.Format("{0} - Ошибка!Расписание на {1} не было размещено", DateTime.Now, DateTime.Now.AddDays(1).ToString("dd.MM.yyyy")));
        }
        public static void SendInMessages(VkApi vk, string message, long? userId)
        {
            if (message == String.Empty)
                return;
            var userInfo = vk.Users.Get((long)userId);
            var postId = vk.Messages.Send(new MessagesSendParams
            {
                UserId = userId,
                Message = message,
            });
            if (postId != 0)
                Log.Logging(String.Format("{0} - Сообщение пользователю {1} успешно отправлено", DateTime.Now, userInfo.FirstName + " " + userInfo.LastName));
            else
                Log.Logging(String.Format("{0} - Ошибка!Сообщение пользователю {1} не было отправлено", DateTime.Now, userInfo.FirstName + " " + userInfo.LastName));
        }
    }
}
