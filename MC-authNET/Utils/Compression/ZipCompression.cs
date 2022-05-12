using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using ICSharpCode.SharpZipLib.Zip;

namespace MC_authNET.Utils.Compression
{
    static class ZipCompression
    {
 
        public static byte[] Decompress(byte[] data)
        {
            MemoryStream compressed = new MemoryStream(data);
            MemoryStream decompressed = new MemoryStream();
            InflaterInputStream inputStream = new InflaterInputStream(compressed);
            inputStream.CopyTo(decompressed);
            return decompressed.ToArray();
        }

        public static byte[] Compress(byte[] data)
        {
            MemoryStream compressed = new MemoryStream();
            DeflaterOutputStream outputStream = new DeflaterOutputStream(compressed);
            outputStream.Write(data, 0, data.Length);
            outputStream.Close();
            return compressed.ToArray();
        }
    }

}
