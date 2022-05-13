using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Ionic.Zlib;


namespace MC_authNET.Utils.Compression
{
    static class Zlib
    {
 
        public static byte[] Decompress(byte[] raw_data, int uncompressedSize)
        {
            ZlibStream stream = new ZlibStream(new MemoryStream(raw_data, false), CompressionMode.Decompress);
            byte[] packetData_decompressed = new byte[uncompressedSize];
            stream.Read(packetData_decompressed, 0, uncompressedSize);
            stream.Close();
            return packetData_decompressed;
        }

        public static byte[] Compress(byte[] raw_data)
        {
            byte[] compresssed_data;
            using (MemoryStream memstream = new MemoryStream())
            {
                using (ZlibStream stream = new ZlibStream(memstream, CompressionMode.Compress))
                {
                    stream.Write(raw_data, 0, raw_data.Length);
                }
                compresssed_data = memstream.ToArray();
            }
            return compresssed_data;
        }
    }

}
