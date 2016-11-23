using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNet;
using VkNet.Enums.Filters;
using VkSchelude.Utils;

namespace VkSchelude
{
    class Authorize
    {
        public static VkApi vkUser { get; set; }
        public static VkApi vkGroup { get; set; }
        public static long groupId { get; set; }
        private static bool vkUserReq = true;
        private static bool vkGroupReq = true;
        public static SqlConnection connection { get; set; }
        public static void Auth()
        {
            var authData = getAuthData();
            var vk = new VkApi();
            if (!authData.ContainsKey("appId"))
            {
                Log.Logging("В файле authData.txt не найден ID приложения(appId)");
                vkUserReq = false;
            }
            if (!authData.ContainsKey("login"))
            {
                Log.Logging("В файле authData.txt не найден логин пользователя(login)");
                vkUserReq = false;
            }
            if (!authData.ContainsKey("password"))
            {
                Log.Logging("В файле authData.txt не найден пароль пользователя(password)");
                vkUserReq = false;
            }
            if (vkUserReq)
            {
                vk.Authorize(new ApiAuthParams
                {
                    ApplicationId = ulong.Parse(authData["appId"]),
                    Login = authData["login"],
                    Password = authData["password"],
                    Settings = Settings.Wall
                });
                vkUser = vk;
            }
            if (!authData.ContainsKey("groupAccessToken"))
            {
                Log.Logging("В файле authData.txt не найден токен доступа группы(groupAccessToken)");
                vkGroupReq = false;
            }
            if (vkGroupReq)
            {
                vk = new VkApi();
                vk.Authorize(authData["groupAccessToken"]);
                vkGroup = vk;
            }
            if (!authData.ContainsKey("groupId"))
            {
                Log.Logging("В файле authData.txt не найден ID группы(groupId)");
                groupId = 0;
            }
            else
                groupId = long.Parse(authData["groupId"]);
            if (!authData.ContainsKey("connectionString"))
                Log.Logging("Подключение к БД невозможно, так как в файле authData.txt не найдена строка подключения(connectionString)");
            else
            {
                connection = new SqlConnection(authData["connectionString"]);
                Authorize.connection.Open();
            }
                
        }
        private static Dictionary<string, string> getAuthData()
        {
            if (!File.Exists("authData.txt"))
            {
                Log.Logging("Не найден файл authData.txt");
                Console.ReadKey();
                Environment.Exit(0);
            } 
            List<string> authData = File.ReadAllLines("authData.txt").ToList();
            Dictionary<string, string> authDictionary = new Dictionary<string, string>();
            foreach (var item in authData)
            {
                var parsedItem = item.Split(':');
                authDictionary.Add(parsedItem[0].Trim(), parsedItem[1].Trim());
            }
            return authDictionary;
        }
    }
}
