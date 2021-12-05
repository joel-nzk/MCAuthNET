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

        public KeepAlivePacket(MinecraftStream stream)
        {
            PacketId = 0x21;
            this.stream = stream;
        }

        public override void Send()
        {        
            stream.WriteVarLong(KeepAliveID);
            stream.SendPacket(PacketId);
        }
    }
}
