using System.Collections.Generic;
using Newtonsoft.Json;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;
using System.IO;

namespace ConsoleApp2
{


    class Program
    {
        static void Main()
        {
            AuthObjects auth = new();
            VkBot bot = new VkBot();



            bot.Run();
        }
    }
}

