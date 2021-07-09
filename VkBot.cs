using System;
using System.Linq;
using System.Text;
using System.Threading;
using VkNet.Model.RequestParams;
using VkNet.Utils;
using System.Net;
using Newtonsoft.Json.Linq;
using VkNet.Enums.SafetyEnums;
using System.Collections.Generic;
using Newtonsoft.Json;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;
using System.IO;

namespace ConsoleApp2
{
    class VkBot
    { 
        static string[] Commands = { "!help", "!gstart", "!gend", "!ghelp", "!gnext" };
        bool game_on = false;
        private VkNet.Model.Attachments.Photo game_photo;
        private int count_t = 0;
        private int count_f = 0;


        private static VkNet.Model.Attachments.Photo GetRandomPhoto()
        {
            Random rnd = new Random();

            var photos = AuthObjects.GetVkUser().Photo.Get(new PhotoGetParams
            {
                AlbumId = PhotoAlbumType.Id(268211254),
                OwnerId = -180356749
            });

            var photo = photos.ElementAt(rnd.Next(0, photos.Count()));

            return photo;
        }

        private void SendGroupMessage(string msg, long user_id)
        {
            Random rnd = new Random();


            AuthObjects.GetVkGroup().Messages.Send(new MessagesSendParams
            {
                RandomId = rnd.Next(),
                UserId = user_id,
                Message = msg
            });
        }

        private void SendGroupPhMessage(long user_id, VkNet.Model.Attachments.Photo photo)
        {
            Random rnd = new Random();


            AuthObjects.GetVkGroup().Messages.Send(new MessagesSendParams
            {
                RandomId = rnd.Next(),
                UserId = user_id,
                Attachments = new List<VkNet.Model.Attachments.MediaAttachment>
                {
                    photo
                }
            });
        }

        private string ConvertWinUtf(string photo_text)
        {
            byte[] phArr = Encoding.GetEncoding(1251).GetBytes(photo_text);

            string ph_text = Encoding.UTF8.GetString(phArr);

            return ph_text;
        }

        public void MessageProcessing(JToken item, WebClient webClient)
        {
            Random rnd = new Random();


            long user_id = Convert.ToInt32(item["object"]["user_id"]);

            string msg = item["object"]["body"].ToString().ToLower();
            Console.WriteLine($"{msg} ");


            //информация о собеседнике
            var user_data = AuthObjects.GetVkUser().Users.Get(new long[] { user_id }).FirstOrDefault();


           



            #region Обработка сообщений


            if (msg.IndexOf("привет") > -1 || msg.IndexOf("здравствуйте") > -1)
            {
                SendGroupMessage($"Приветствуем Вас в нашей группе!\nНапишите \"{Commands[0]}\" для получения списка команд", user_id);
            }
            else
            if (msg.IndexOf(Commands[0]) > -1)
            {
                string msg_to_send = "Список команд:\n";
                for (int j = 0; j < Commands.Length; j++)
                {
                    msg_to_send += "\"" + Commands[j] + "\"\n";
                }

                SendGroupMessage(msg_to_send, user_id);
            }
            else
            if (msg.IndexOf(Commands[1]) > -1)
            {
                string msg_to_send;
                if (game_on == false)
                {
                    msg_to_send = "Бот присылает Вам фотографию астрономического объекта, а Вы должны отгадать его название.\nОбозначения каталогов (NGC, M и т.д.) набираются латиницей и отделяются от номера объекта пробелом." +
                     $"\nПример назаний: M 45, Плеяды, М 42, М 33, Туманность Ориона\n\nДелайте приписку \"Ответ:\", когда хотите написать ответ.\nНапишите \"{Commands[4]}\" для получения новой фотографии\nНапишите \"{Commands[3]}\", если нужна подсказка.\nНапишите \"{Commands[2]}\", чтобы закончить играть.\n\nИгра начинается!";
                    SendGroupMessage(msg_to_send, user_id);
                    game_photo = GetRandomPhoto();
                    SendGroupPhMessage(user_id, game_photo);
                    game_on = true;
                }
                else
                {
                    msg_to_send = "Игра уже начата";
                    SendGroupMessage(msg_to_send, user_id);
                }

            }
            else
            if (msg.IndexOf(Commands[3]) > -1)
            {
                string msg_to_send;
                if (game_on == false)
                    msg_to_send = $"Данная команда выводит подсказку для игры.\nИгра не начата, наберите \"{Commands[1]}\", чтобы играть";
                else
                    msg_to_send = "Подсказка: Вы можете открыть фотографию и подсмотреть название объекта в описании, приятной игры!";
                SendGroupMessage(msg_to_send, user_id);
            }
            else
            if (msg.IndexOf(Commands[4]) > -1)
            {
                string msg_to_send;
                if (game_on == false)
                {
                    msg_to_send = $"Данная команда внутреигровая.\nИгра не начата, наберите \"{Commands[1]}\", чтобы играть";
                    SendGroupMessage(msg_to_send, user_id);
                }
                else
                {
                    game_photo = GetRandomPhoto();
                    SendGroupPhMessage(user_id, game_photo);
                }
            }
            else
            if (msg.IndexOf(Commands[2]) > -1)
            {
                string msg_to_send;
                if (game_on == false)
                    msg_to_send = $"Данная команда заканчивает игру.\nИгра не начата, наберите \"{Commands[1]}\", чтобы играть";
                else
                {
                    int percentage = (int)(count_t / (double)(count_t + count_f) * 100) ;
                    msg_to_send = $"Игра закончена!\nПроцент правильных ответов: {percentage}%";

                }

                SendGroupMessage(msg_to_send, user_id);
                game_on = false;
            }         
            else
            if (game_on == true)
            {
                if (msg.IndexOf("ответ:") > -1)
                {
                    string ans = msg.Replace("ответ:", "");

                    if (ans[0] == ' ') ans = ans.Remove(0, 1);

                    string photo_text = ConvertWinUtf(game_photo.Text).ToLower();

                    if (photo_text.IndexOf(ans) > -1)
                    {
                        SendGroupMessage("Правильно!\nСледующее фото:", user_id);
                        count_t++;
                        game_photo = GetRandomPhoto();
                        SendGroupPhMessage(user_id, game_photo);
                    }
                    else
                    {
                        SendGroupMessage("Неправильно!\nПопробуйте ещё", user_id);
                        count_f++;
                    }
                        
                }
                else
                {
                    SendGroupMessage("Неизвестная команда!\nИспользуйте приписку \"Ответ:\", чтобы дать ответ", user_id);
                }
            }
            else
            {
                SendGroupMessage($"Неизвестная команда!\nНаберите \"{Commands[0]}\", чтобы увидеть список команд", user_id);
            }


            #endregion







        }

        public void Run()
        {

            var webClient = new WebClient() { Encoding = Encoding.UTF8 };


            var param = new VkParameters() { };
            param.Add<string>("group_id", AuthObjects.group_id.ToString());


            dynamic responseLongPoll = JObject.Parse(AuthObjects.GetVkUser().Call("groups.getLongPollServer", param).RawJson);

            string json = String.Empty;

            string url = string.Empty;

            while (true)
            {
                url = string.Format("{0}?act=a_check&key={1}&wait=5&mode=2&ts={2}",
                    responseLongPoll.response.server.ToString(),
                    responseLongPoll.response.key.ToString(),
                    json != String.Empty ? JObject.Parse(json)["ts"].ToString() : responseLongPoll.response.ts.ToString()
                    );

                json = webClient.DownloadString(url);

                var jsonMsg = json.IndexOf(":[]}") > -1 ? "" : $"{json} \n";

                var col = JObject.Parse(json)["updates"].ToList();

                //message new
                foreach (var item in col)
                {
                    if (item["type"].ToString() == "message_new")
                    {
                        MessageProcessing(item, webClient);
                    }


                    if (item["type"].ToString() == "group_join")
                    {
                        string user_id = item["object"]["user_id"].ToString();
                        Console.WriteLine("{0} {1} <{2}> подписался!\n", item["object"]["first_name"].ToString(), item["object"]["last_name"].ToString(), user_id);
                    }

                    Thread.Sleep(200);
                }



            }
        }
    }
}

