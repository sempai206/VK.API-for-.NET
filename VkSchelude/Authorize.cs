using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNet;
using VkNet.Enums.Filters;

namespace VkSchedule
{
    class Authorize
    {
        public static VkApi AuthUser()
        {
            List<string> authData = System.IO.File.ReadAllLines("authData.txt").ToList();
            var vk = new VkApi();
            vk.Authorize(new ApiAuthParams
            {
                ApplicationId = ulong.Parse(authData[0]),
                Login = authData[1],
                Password = authData[2],
                Settings = Settings.All
            });
            return vk;
        }
        public static VkApi AuthGroup()
        {
            var vk = new VkApi();
            string[] readText = System.IO.File.ReadAllLines("authData.txt");
            string groupAccessToken = readText[readText.Length];
            vk.Authorize(groupAccessToken);
            return vk;
        }
    }
}
