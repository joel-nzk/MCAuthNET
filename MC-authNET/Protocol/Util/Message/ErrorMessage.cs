using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_authNET.Protocol.Util
{
    class ErrorMessage
    {
        public string message;
        public string context;
        public ConsoleColor Color;

        public ErrorMessage(string message, ConsoleColor color,string context)
        {
            this.message = message;
            this.Color = color;
            this.context = context;
        }
    }
}
