using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;

namespace Atom
{
    public static partial class Conversion
    {
        //-----------------------------------------------------------------------------------------
        public static byte[] ToByteArray(bool value)
        {
            return BitConverter.GetBytes(value);
        }
        //-----------------------------------------------------------------------------------------
        public static byte[] ToByteArray(bool? value)
        {
            return value == null ? null : BitConverter.GetBytes(value.Value);
        }
        //-----------------------------------------------------------------------------------------
        public static byte[] ToByteArray(byte value)
        {
            return new[] { value };
        }
        //-----------------------------------------------------------------------------------------
        public static byte[] ToByteArray(sbyte value)
        {
            return new[] { (byte)value };
        }
        //-----------------------------------------------------------------------------------------
        public static byte[] ToByteArray(Int32 value)
        {
            return BitConverter.GetBytes(value);
        }
        //-----------------------------------------------------------------------------------------
        public static byte[] ToByteArray(Int64 value)
        {
            return BitConverter.GetBytes(value);
        }
        //-----------------------------------------------------------------------------------------
        public static byte[] ToByteArray(UInt64 value)
        {
            return BitConverter.GetBytes(value);
        }
        //-----------------------------------------------------------------------------------------
        public static byte[] ToByteArray(UInt32 value)
        {
            return BitConverter.GetBytes(value);
        }
        //-----------------------------------------------------------------------------------------
        public static byte[] ToByteArray(Int16 value)
        {
            return BitConverter.GetBytes(value);
        }
        //-----------------------------------------------------------------------------------------
        public static byte[] ToByteArray(UInt16 value)
        {
            return BitConverter.GetBytes(value);
        }
        //-----------------------------------------------------------------------------------------
        public static byte[] ToByteArray(char value)
        {
            return BitConverter.GetBytes(value);
        }
        //-----------------------------------------------------------------------------------------
        public static byte[] ToByteArray(Decimal value)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write(value);
                    return stream.ToArray();
                }
            }
        }
        //-----------------------------------------------------------------------------------------
        public static byte[] ToByteArray(string value)
        {
			var result = new byte[value.Length*2];
            Memory.Memcpy(result, 0, value.ToCharArray(), 0, value.Length * 2);
			return result;
        }
        //-----------------------------------------------------------------------------------------
        //символы должны быть в формате 00-FF
        public static byte[] HexStringToByteArray(string value)
        {
            var array = new byte[value.Length / 2];
            var hexValue = new[]
                                   {
                                       0x00, 0x01, 0x02, 0x03, 0x04, 0x05,
                                       0x06, 0x07, 0x08, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                       0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F
                                   };

            for (var i = 0; i < array.Length; i++)
            {
                array[i] = (byte)(hexValue[char.ToUpper(value[i * 2 + 0]) - '0'] << 4 |
                                   hexValue[char.ToUpper(value[i * 2 + 1]) - '0']);
            }
            return array;    
        }
        //-----------------------------------------------------------------------------------------
        public static byte[] ToByteArray(double value)
        {
            return BitConverter.GetBytes(value);
        }
        //-----------------------------------------------------------------------------------------
        public static byte[] ToByteArray(float value)
        {
            return BitConverter.GetBytes(value);
        }
        //-----------------------------------------------------------------------------------------
        public static byte[] FileToByteArray(string path)
        {
            try
            {
                return File.ReadAllBytes(path);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while reading file: {path}.", ex);
            }
        }
        //-----------------------------------------------------------------------------------------
        public static byte[] FileToMd5(string path)
        {
            using (var md5 = MD5.Create())
                using (var stream = File.OpenRead(path))
                    return md5.ComputeHash(stream);
        }
        //-----------------------------------------------------------------------------------------
        public static byte[] ZipByteArrayToByteArray(byte[] value)
        {
            // the trick is to read the last 4 bytes to get the length
            // gzip appends this to the array when compressing
            var length = Memory.UnpackingInt(value, value.Length - sizeof (int));
            var buffer = new byte[length];
            using (var memoryStream = new MemoryStream(value))
            {
                using (var zipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    zipStream.Read(buffer, 0, length);
                }
            }
            return buffer;
        }
        //-----------------------------------------------------------------------------------------
    }
}
