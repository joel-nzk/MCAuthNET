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

       

        static void Main(string[] args)
        {
            Console.Title = "Minecraft Console Client";
            Console.ForegroundColor = ConsoleColor.Gray;

           
            MinecraftUser user = new MinecraftUser(email,username, password, AccountType.Offline);
            MinecraftClient client = new MinecraftClient(sv_adress, sv_port);

            client.LoginToServer(user);

            //JToken jsonData = JToken.Parse(client.ServerListPing().JsonContent);
            //Console.WriteLine(jsonData);

                   

        }

    }
}
