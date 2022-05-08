using MC_authNET.Network.Packet;
using MC_authNET.Network.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_authNET.Protocol.Network.Packets.Play
{
    class KeepAlivePacket : Packet
    {
        public long KeepAliveID;

        public KeepAlivePacket()
        {
            PacketId = 0x21;
        }

        public override void Send(MinecraftStream stream)
        {        
            stream.WriteLong(KeepAliveID);
            stream.SendPacket(0x0F);
        }
    }
}
