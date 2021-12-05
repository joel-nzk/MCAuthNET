using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_authNET.Network
{
    class ResponsePacket
    {
        public int PacketLength;
        public int packetID;
        public int JsonLength;
        public string JsonContent;

        public ResponsePacket(int PacketLength, int packetID, int JsonLength, string JsonContent)
        {
            this.PacketLength = PacketLength;
            this.packetID = packetID;
            this.JsonLength = JsonLength;
            this.JsonContent = JsonContent;
        }

        
    }
}
