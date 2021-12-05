using MC_authNET.Network.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_authNET.Network.Packet.Status
{
    class RequestPacket : Packet
    {
        public RequestPacket(MinecraftStream stream)
        {
            PacketId = 0x00;
            this.stream = stream;
        }


        public override void Send()
        {
            stream.WriteByte(0x01);
            stream.WriteByte(PacketId);
        }
    }
}
