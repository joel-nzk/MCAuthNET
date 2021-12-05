using MC_authNET.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_authNET.Protocol.Network
{
    static class ClientStatus
    {
        public static ProtocolVersion ProtocolVersion = ProtocolVersion.v_1_17_1;
        public static ConnectionState NextState = ConnectionState.Status;
    }
}
