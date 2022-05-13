using MC_authNET.Network.Packet;
using MC_authNET.Network.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            PacketId = 0x0F;
        }

        public override void Send(MinecraftStream stream)
        {        
            stream.WriteLong(KeepAliveID);
            stream.SendPacket(PacketId);
        }

        public void Read(MinecraftStream stream,PacketData packet)
        {
            KeepAliveID = stream.ReadLong(packet.data);

        }

 
    }
}
