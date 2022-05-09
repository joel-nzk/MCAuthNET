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


        public JoinGamePacket()
        {
            PacketId = 0x26;
        }


        public override void Send(MinecraftStream stream)
        {

        }

        public void Read(MinecraftStream stream)
        {
            int packet_length = stream.ReadVarIntRAW();
            stream.ReadBytesRAW(packet_length);
            stream.Buffer.Clear();
        }
    }
}
