using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Diagnostics;
using MC_authNET.Minecraft;
using MC_authNET.Network.Packet.Status;
using MC_authNET.Network.Packet;
using MC_authNET.Network.Util;
using MC_authNET.Protocol.Network.Packets.Login;
using MC_authNET.Protocol.Network;
using MC_authNET.Protocol.Util;
using MC_authNET.Protocol.Network.Packets.Play;

namespace MC_authNET.Utils.Extensions
{
    public static class ConsoleMore
    {
        private const ConsoleColor originalColor = ConsoleColor.Gray;

        public static void WriteTitle(string title, ConsoleColor newColor = originalColor)
        {
            using (StringReader reader = new StringReader(title))
            {
                string line = string.Empty;
                do
                {
                    line = reader.ReadLine();
                    if (line != null)
                    {
                        Console.SetCursorPosition((Console.WindowWidth - line.Length) / 2, Console.CursorTop);
                        WriteLine(line,newColor);
                    }
                } while (line != null);
            }

            Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop+1);
        }

        public static void WriteDebug(string text, ConsoleColor mainColor = ConsoleColor.DarkCyan, ConsoleColor secondaryColor = ConsoleColor.DarkBlue)
        {
            Write($"[{DateTime.Now:HH:mm:ss}] [DEBUG] ", mainColor);
            WriteLine(text, secondaryColor);
        }

        public static void WriteInfo(string text, ConsoleColor mainColor = ConsoleColor.DarkMagenta, ConsoleColor secondaryColor = ConsoleColor.Magenta)
        {
            Write($"[{DateTime.Now:HH:mm:ss}] [INFO] ", mainColor);
            WriteLine(text, secondaryColor);
        }


        public static void Write(string title, ConsoleColor newColor = originalColor)
        {
            Console.ForegroundColor = newColor;
            Console.Write(title);
            Console.ForegroundColor = originalColor;
        }

        public static void WriteLine(string title, ConsoleColor newColor = originalColor)
        {
            
            Console.ForegroundColor = newColor;
            Console.WriteLine(title);
            Console.ForegroundColor = originalColor;
        }
    }
}
