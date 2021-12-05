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
        public static string username = "player_name";
        public static string password = "pa$$w0rd";
        public static AccountStatus status = AccountStatus.Offline;

        public static string sv_adress = "localhost";
        public static ushort sv_port = 25565;

       

        static void Main(string[] args)
        {
            Console.Title = "Minecraft Console Client";
            Console.ForegroundColor = ConsoleColor.Gray;

           
            MinecraftUser mc_user = new MinecraftUser(email,username, password, status);
            Client client = new Client(sv_adress, sv_port);

            client.LoginToServer(mc_user);
            //JToken jsonData = JToken.Parse(client.LoginToServer(mc_user).JsonContent);

           
            //JToken jsonData = JToken.Parse(client.ServerListPing().JsonContent);         
            //Console.WriteLine(jsonData);
        }

    }
}
