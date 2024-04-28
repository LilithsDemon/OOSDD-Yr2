using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Compression
{
    class GZIP
    {
        public byte[] Compress(byte[] buffer)
        {
           using (var memoryStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(memoryStream, CompressionLevel.Optimal))
                {
                    gzipStream.Write(buffer, 0, buffer.Length);
                }
                return memoryStream.ToArray();
            }
        }

        public byte[] Compress(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);

            return Compress(buffer);
        }

        public byte[] Decompress(byte[] compressedData)
        {
            using var memoryStream = new MemoryStream(compressedData);

            using var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress);

            using var resultStream = new MemoryStream();

            gZipStream.CopyTo(resultStream);

            return resultStream.ToArray();
        }

        public string Decompress(string compressedData)
        {
            byte[] buffer = Decompress(Encoding.UTF8.GetBytes(compressedData));

            return Encoding.UTF8.GetString(buffer);
        }
    }
}