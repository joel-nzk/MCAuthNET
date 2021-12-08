using MC_authNET.Network.Packet;
using MC_authNET.Network.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_authNET.Protocol.Network.Packets.Play
{
    class JoinGamePacket : Packet
    {


        public JoinGamePacket(MinecraftStream stream)
        {
            PacketId = 0x26;
            this.stream = stream;
        }


        public override void Send()
        {

        }

        public void Read()
        {
            int packet_length = stream.ReadVarInt();
            stream.ReadBytes(packet_length);
            stream.Buffer.Clear();
        }
    }
}
