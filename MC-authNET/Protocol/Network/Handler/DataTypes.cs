using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC_authNET.Protocol.Network.Handler
{
    [Obsolete("Not used any more", true)]

    class DataTypes
    {


        #region Writing
        public void WriteVarInt(List<byte> Buffer, int value)
        {
            while (true)
            {
                if ((value & ~0x7F) == 0)
                {
                    Buffer.Add((byte)value);
                    return;
                }

                Buffer.Add((byte)((value & 0x7F) | 0x80));
                value >>= 7;
            }
        }

        public void WriteShort(List<byte> Buffer, ushort value)
        {
            Buffer.AddRange(BitConverter.GetBytes(value));
        }

        public void WriteString(List<byte> Buffer, string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            WriteVarInt(Buffer,bytes.Length);
            Buffer.AddRange(bytes);

        }

        public void WriteVarLong(List<byte> Buffer, long value)
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



        public void WriteByte(Stream stream,int b)
        {
            stream.WriteByte((byte)b);
        }

        #endregion

        #region Reading
        public string ReadString(Stream stream,int length)
        {
            byte[] data = new byte[length];

            stream.Read(data);

            return Encoding.UTF8.GetString(data);
        }

        public string ReadUUID(Stream stream, int length)
        {

            byte[] data = new byte[length];

            stream.Read(data);

            ulong b1 = BitConverter.ToUInt64(data, 0);
            ulong b2 = BitConverter.ToUInt64(data, 8);


            byte[] bytes = new byte[0];
            bytes = bytes.Concat(BitConverter.GetBytes(b1)).Concat(BitConverter.GetBytes(b2)).ToArray();

            string uuid = "";
            foreach (byte b in bytes)
                uuid += b.ToString("x2");

            return uuid.Substring(0, 8) + "-" + uuid.Substring(8, 4) + "-" + uuid.Substring(12, 4) + "-" + uuid.Substring(16, 4) + "-" + uuid.Substring(20, 12);
        }

        public byte[] ReadBytes(Stream stream, int length)
        {
            byte[] data = new byte[length];
            stream.Read(data);
            return data;
        }
        public int ReadVarInt(Stream stream)
        {
            int value = 0;
            int length = 0;
            int currentByte;

            while (true)
            {
                currentByte = stream.ReadByte();
                value |= (currentByte & 0x7F) << (length++ * 7);
                if (length > 5) throw new IOException("VarInt too big");
                if ((currentByte & 0x80) != 0x80) break;
            }
            return value;
        }



        public long ReadVarLong(Stream stream)
        {
            long value = 0;
            int length = 0;
            int currentByte;

            while (true)
            {
                currentByte = stream.ReadByte();
                value |= (currentByte & 0x7F) << (length++ * 7);
                if (length > 10) throw new IOException("VarLong is too big");
                if ((currentByte & 0x80) != 0x80) break;
            }
            return value;
        }


        #endregion




        #region Others

        /**
        * 
        * @author Ammar Askar 
        * https://gist.github.com/ammaraskar/7b4a3f73bee9dc4136539644a0f27e63
        *
        */

       /* public static string MinecraftShaDigest(string server_id, byte[] shared_secret, byte[] public_key)
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
        }*/

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

    }
}
