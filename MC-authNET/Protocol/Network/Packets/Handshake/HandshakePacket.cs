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

        public HandshakePacket(MinecraftStream stream)
        {
            PacketId = 0x00;
            this.stream = stream;
        }


        public override void Send()
        {
            stream.WriteVarInt((int)ClientStatus.ProtocolVersion);
            stream.WriteString(stream.sv_adress);
            stream.WriteShort(stream.sv_port);
            stream.WriteVarInt((int)ClientStatus.NextState);

            stream.SendPacket(PacketId);
        }

      
    }
}
