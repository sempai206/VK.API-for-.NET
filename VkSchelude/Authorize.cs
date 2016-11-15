using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNet;
using VkNet.Enums.Filters;

namespace VkSchelude
{
    class Authorize
    {
        public static VkApi vkUser { get; set; }
        public static VkApi vkGroup { get; set; }
        public static void setAuthorize()
        {
            AuthUser();
            AuthGroup();
        }
        public static void AuthUser()
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
            vkUser = vk;
        }
        public static void AuthGroup()
        {
            var vk = new VkApi();
            string groupAccessToken = System.IO.File.ReadAllLines("authData.txt")[3];
            vk.Authorize(groupAccessToken);
            vkGroup = vk;
        }
    }
}
