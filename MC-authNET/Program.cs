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
using MC_authNET.Protocol.Util;

namespace MC_authNET
{
    class Program
    {
        public static string email = "myemail@gmail.com";
        public static string username = "console-player1";
        public static string password = "pa$$w0rd";


        public static string sv_adress = "localhost";
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
            Console.Title = "Minecraft Console Client";
            Console.ForegroundColor = ConsoleColor.Gray;
            Random rnd = new Random();
            int index = 0;

            for (int i = 0; i < 90; i++)
            {
                if (index+1 >= names.Length)
                    index = 0;
                else
                    index++;


                MinecraftUser user = new MinecraftUser(email, names[index] +rnd.Next(0,100), password, AccountType.Offline);
                MinecraftClient client = new MinecraftClient(sv_adress, sv_port);
                client.LoginToServer(user);
            }
           


            //JToken jsonData = JToken.Parse(client.ServerListPing().JsonContent);
            //Console.WriteLine(jsonData);

                   

        }

    }
}
