using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using MC_authNET.Network;
using MC_authNET.Minecraft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MC_authNET.Utils.ASCII;
using MC_authNET.Utils.Extensions;
using System.Configuration;
using System.Collections.Specialized;

namespace MC_authNET
{
    class Program
    {
        public static string email = "myemail@gmail.com";
        public static string username = "TheBestPlayer";
        public static string password = "pa$$w0rd";


        public static string sv_adress = "149.202.45.8";
        public static ushort sv_port = 25565;

        public static string[] names = 
        {
            "the_player1212",
            "mc_user1",
            "fake_botUser",
            "TheUserPlayerMC",
            "aypierre",
            "boblennon",
            "cocacola12",
            "my player" ,
            "fanta12124",
            "apple_pie13",
            "the_bot72",
            "habbo-bot-mc",
            "fortdhiver",
            "userman4456",
            "linux12",
            "windows2008",
            "macintosh1255",
            "ubuntu-22",
            "kalilinux22",
            "bakugan1233"
        };

       

        static void Main(string[] args)
        {
            Start();
            MinecraftUser user = new MinecraftUser(email, username, password, AccountType.Offline);
            MinecraftClient client = new MinecraftClient(sv_adress, sv_port);
            //client.LoginToServer(user);

            client.ServerListPing();
        }



        private static void Start()
        {
            Console.Title = ConfigurationManager.AppSettings.Get("appName");
            ConsoleMore.WriteTitle(ASCII.AppTitle, ConsoleColor.Blue);
            ConsoleMore.WriteTitle($"Version {ConfigurationManager.AppSettings.Get("version")}", ConsoleColor.Red);
            ConsoleMore.WriteTitle("https://github.com/Joel-NK2503/MCAuthNET", ConsoleColor.Red);
        }

    }
}
