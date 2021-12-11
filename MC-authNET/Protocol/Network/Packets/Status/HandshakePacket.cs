using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MC_authNET.Network.Packet;
using MC_authNET.Network.Util;
using MC_authNET.Protocol.Network;

namespace MC_authNET.Network.Packet
{
    class HandshakePacket : Packet
    {

        public HandshakePacket()
        {
            PacketId = 0x00;
        }


        public override void Send(MinecraftStream stream)
        {
            stream.WriteVarInt((int)stream.ProtocolVersion);
            stream.WriteString(stream.sv_adress);
            stream.WriteShort(stream.sv_port);
            stream.WriteVarInt((int)stream.NextState);

            stream.SendPacket(PacketId);
        }

      
    }
}
