using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Numerics;
using System.Net.Sockets;
using MC_authNET.Protocol.Network;

namespace MC_authNET.Network.Util
{
    public class MinecraftStream
    {
        private NetworkStream networkStream;
        public List<byte> Buffer;

        public ProtocolVersion ProtocolVersion = ProtocolVersion.v_1_17_1;
        public ConnectionState NextState = ConnectionState.Status;


        public string sv_adress;
        public ushort sv_port;



        public MinecraftStream()
        {
            Buffer = new List<byte>();
        }



   
        public void InitializeStream(TcpClient client)
        {
            networkStream = client.GetStream();
        }

        public void SendPacket(int id)
        {
            byte[] buffer = Buffer.ToArray();
            Buffer.Clear();

            WriteVarInt(id);
            var packet_id = Buffer.ToArray();
            Buffer.Clear();

            WriteVarInt(buffer.Length + packet_id.Length);
            var bufferLenght = Buffer.ToArray();


            networkStream.Write(bufferLenght, 0, bufferLenght.Length);
            networkStream.Write(packet_id, 0, packet_id.Length);
            networkStream.Write(buffer, 0, buffer.Length);


            Buffer.Clear();
            networkStream.Flush();
        }



      

        #region Writing
        public void WriteVarInt(int value, List<byte> _buffer = null)
        {
            List<byte> local_buffer = _buffer == null ? Buffer : _buffer;

  
            while (true)
            {
                if ((value & ~0x7F) == 0)
                {
                    local_buffer.Add((byte)value);
                    return;
                }

                local_buffer.Add((byte)((value & 0x7F) | 0x80));
                value >>= 7;
            }
        }

        public  void WriteShort(ushort value)
        {
            Buffer.AddRange(BitConverter.GetBytes(value));
        }

        public void WriteString(string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            WriteVarInt( bytes.Length);
            Buffer.AddRange(bytes);

        }

        public void WriteVarLong(long value)
        {
            while (true)
            {
                if ((value & ~0x7F) == 0)
                {
                    Buffer.Add((byte)value);
                    return;
                }

                Buffer.Add((byte)((value & 0x7F) | 0x80));
                // Note: >>> means that the sign bit is shifted with the rest of the number rather than being left alone
                value >>= 7;
            }
        }

  

        public void WriteByte(int b)
        {
            networkStream.WriteByte((byte)b);
        }

        public void WriteLong(long value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Buffer.AddRange(bytes);
        }

        #endregion

        #region Reading
        public string ReadString(int length)
        {
            byte[] data = new byte[length];

            networkStream.Read(data);

            return Encoding.UTF8.GetString(data);
        }

        public string ReadUUID(int length)
        {

            byte[] data = new byte[length];

            networkStream.Read(data);

            ulong b1 = BitConverter.ToUInt64(data, 0);
            ulong b2 = BitConverter.ToUInt64(data, 8);


            byte[] bytes = new byte[0];
            bytes = bytes.Concat(BitConverter.GetBytes(b1)).Concat(BitConverter.GetBytes(b2)).ToArray();

            string uuid = "";
            foreach (byte b in bytes)
                uuid += b.ToString("x2");

            return uuid.Substring(0, 8) + "-" + uuid.Substring(8, 4) + "-" + uuid.Substring(12, 4) + "-" + uuid.Substring(16, 4) + "-" + uuid.Substring(20, 12);
        }

        public byte[] ReadBytes(int length)
        {
            byte[] data = new byte[length];
            networkStream.Read(data);
            return data;
        }
        public int ReadVarInt()
        {
            int value = 0;
            int length = 0;
            int currentByte;

            while (true)
            {
                currentByte = networkStream.ReadByte();               
                value |= (currentByte & 0x7F) << (length++ * 7);
                if (length > 5) throw new IOException("VarInt too big");
                if ((currentByte & 0x80) != 0x80) break;
            }
            return value;
        }

        public int ReadVarInt2(Queue<byte> data)
        {
            int value = 0;
            int length = 0;
            int currentByte = 0;

            while (true)
            {
                currentByte = ReadNextByte(data);
                value |= (currentByte & 0x7F) << (length++ * 7);
                if (length > 5) throw new IOException("VarInt too big");
                if ((currentByte & 0x80) != 0x80) break;
            }
            return value;
        }

        public byte ReadNextByte(Queue<byte> cache)
        {
            byte result = cache.Dequeue();
            return result;
        }

        public long ReadLong2(byte[] data)
        {
            byte[] long_data = new byte[8];
            Queue<byte> queue_data = new Queue<byte>(data); 
            
            for (int i = 0; i < 8; i++)
            {
                long_data[i] = ReadNextByte(queue_data);
            }

           
            return BitConverter.ToInt64(long_data);
        }




        public long ReadVarLong()
        {
            long value = 0;
            int length = 0;
            int currentByte;

            while (true)
            {
                currentByte = networkStream.ReadByte();
                value |= (currentByte & 0x7F) << (length++ * 7);
                if (length > 10) throw new IOException("VarLong is too big");
                if ((currentByte & 0x80) != 0x80) break;
            }
            return value;
 
        }

        public long ReadLong()
        {       
            byte[] data = new byte[8];
            networkStream.Read(data);
            return BitConverter.ToInt64(data);
        }

       

        #endregion




        #region Others

        /**
        * 
        * @author Ammar Askar 
        * https://gist.github.com/ammaraskar/7b4a3f73bee9dc4136539644a0f27e63
        *
        */

        public static string MinecraftShaDigest(string server_id, byte[] shared_secret, byte[] public_key)
        {
            var sha1 = SHA1.Create();

            sha1.ComputeHash(Encoding.ASCII.GetBytes(server_id));
            sha1.ComputeHash(shared_secret);
            sha1.ComputeHash(public_key);

            byte[] hash = sha1.Hash;


            // Reverse the bytes since BigInteger uses little endian
            Array.Reverse(hash);

            BigInteger b = new BigInteger(hash);
            // very annoyingly, BigInteger in C# tries to be smart and puts in
            // a leading 0 when formatting as a hex number to allow roundtripping 
            // of negative numbers, thus we have to trim it off.
            if (b < 0)
            {
                // toss in a negative sign if the interpreted number is negative
                return "-" + (-b).ToString("x").TrimStart('0');
            }
            else
            {
                return b.ToString("x").TrimStart('0');
            }
        }

        public static string MinecraftShaDigest(byte[] input)
        {
            var hash = new SHA1Managed().ComputeHash(input);
            // Reverse the bytes since BigInteger uses little endian
            Array.Reverse(hash);


            // Reverse the bytes since BigInteger uses little endian
            Array.Reverse(hash);

            BigInteger b = new BigInteger(hash);
            // very annoyingly, BigInteger in C# tries to be smart and puts in
            // a leading 0 when formatting as a hex number to allow roundtripping 
            // of negative numbers, thus we have to trim it off.
            if (b < 0)
            {
                // toss in a negative sign if the interpreted number is negative
                return "-" + (-b).ToString("x").TrimStart('0');
            }
            else
            {
                return b.ToString("x").TrimStart('0');
            }
        }

        public static string ByteArrayToString(byte[] bytes)
        {
            StringWriter sw = new StringWriter();

            foreach (var value in bytes)
            {
                sw.Write(value);
            }

            return sw.ToString();
        }

        public static byte[] RandomByteArray(int length = 1)
        {
            Random rnd = new Random();

            length = length < 1 ? 1 : length;

            byte[] rnd_byte_array = new byte[length];

            rnd.NextBytes(rnd_byte_array);

            return rnd_byte_array;
        }

        #endregion

        public void Dispose()
        {
            networkStream.Close();
            Buffer.Clear();
        }

        public NetworkStream Get()
        {
            return networkStream;
        }

        public void Flush()
        {
            networkStream.Flush();
        }





    }
}
