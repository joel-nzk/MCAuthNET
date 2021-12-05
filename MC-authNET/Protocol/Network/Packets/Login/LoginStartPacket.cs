using MC_authNET.Network.Packet;
using MC_authNET.Network.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_authNET.Protocol.Network.Packets.Login
{
    class LoginStartPacket : Packet
    {
        public string username;

        public LoginStartPacket(MinecraftStream stream)
        {
            PacketId = 0x00;
            this.stream = stream;
        }


        public override void Send()
        {
            stream.WriteString(username);
            stream.SendPacket(PacketId);
        }
    }
}
