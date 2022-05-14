using MC_authNET.Network.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace MC_authNET.Network.Packet
{
    public abstract class Packet
    {
        protected int PacketId = -1;
    

        public abstract void Send(MinecraftStream stream);

        //public bool compressionEnabled = false;
        //public int compression_threshold;
    }
}
