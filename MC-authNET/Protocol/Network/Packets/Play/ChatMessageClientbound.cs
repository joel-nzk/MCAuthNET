using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MC_authNET.Network.Packet;
using MC_authNET.Network.Util;

namespace MC_authNET.Protocol.Network.Packets.Play
{
    class ChatMessageClientbound : Packet
    {

        public string jsonData;
        public byte position;
        public string uuid;

        public ChatMessageClientbound()
        {
            PacketId = 0x0F;
        }

        public override void Send(MinecraftStream stream)
        {
            throw new NotImplementedException();
        }

        public void Read(MinecraftStream stream)
        {
            //jsonData = stream.ReadNextString(packetData.data);
            //position = stream.ReadByte(packetData.data);
            //uuid = stream.ReadUUID(packetData.data);
        }
    }
}
