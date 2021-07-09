using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Model.RequestParams;
using VkNet.Utils;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using VkNet.Enums.SafetyEnums;

namespace ConsoleApp2
{
    class AuthObjects
    {
        private static VkApi vk_user;
        private static VkApi vk_group;

        public static string token = @"1bfab2ddcf4cc8374a9b7128b50c25f5539a30e9aa46f3dba97b800c47efeeb71b795f9816b4e3f0418ec";
        //public static string urlBotMsg = $"https://api.vk.com/method/messages.send?v=5.131&access_token={token}&user_id=";
        public static ulong group_id = 180356749;

        public AuthObjects()
        {
            vk_user = new();
            vk_group = new();

            ulong ID = 7883197;
            string login = File.ReadAllText("_login.txt");
            string passw = File.ReadAllText("_password.txt");

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            vk_user.Authorize(new ApiAuthParams
            {

                ApplicationId = ID,
                Login = login,
                Password = passw,
                Settings = Settings.All,

            });

            Thread.Sleep(200);

            vk_group.Authorize(new ApiAuthParams
            {

                AccessToken = token

            });


        }

        public static ref VkApi GetVkUser()
        {
            return ref vk_user;
        }

        public static ref VkApi GetVkGroup()
        {
            return ref vk_group;
        }

    }
}
