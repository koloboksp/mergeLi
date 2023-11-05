using System;
using System.IO;
using System.IO.Compression;

namespace Atom
{
    public static partial class Conversion
    {
        //-----------------------------------------------------------------------------------------
        /*
        public static bool? ToNullableBool(byte[] buffer)
        {
            if (buffer == null)
                return null;
            return ToBool(buffer);
        }
        */
        //-----------------------------------------------------------------------------------------
        public static sbyte ToInt8(byte[] buffer)
        {
            return (sbyte) buffer[0];
        }
        //-----------------------------------------------------------------------------------------
        public static byte ToUInt8(byte[] buffer)
        {
            return buffer[0];
        }
        //-----------------------------------------------------------------------------------------
        public static byte ToUInt8(bool value)
        {
            return ToUInt8(value ? 1 : 0);
        }
        //-----------------------------------------------------------------------------------------
        public static byte ToUInt8(char value)
        {
            return (byte)value;
        }
        //-----------------------------------------------------------------------------------------
        public static byte ToUInt8(int value)
        {
            return (byte)value;
        }
        //-----------------------------------------------------------------------------------------
        public static byte ToUInt8(double value)
        {
            return (byte)value;
        }
        //-----------------------------------------------------------------------------------------
        public static byte ToUInt8(uint value)
        {
            return (byte)value;
        }
        //-----------------------------------------------------------------------------------------
        public static short ToInt16(byte[] buffer)
        {
            return BitConverter.ToInt16(buffer, 0);
        }
        //-----------------------------------------------------------------------------------------
        public static ushort ToUInt16(byte[] buffer)
        {
            return BitConverter.ToUInt16(buffer, 0);
        }
        //-----------------------------------------------------------------------------------------
        public static ushort? ToNullableUInt16(byte[] buffer)
        {
            if (buffer == null)
                return null;
            return ToUInt16(buffer);
        }
        //-----------------------------------------------------------------------------------------
        public static long ToInt64(byte[] buffer)
        {
            return BitConverter.ToInt64(buffer, 0);
        }
        //-----------------------------------------------------------------------------------------
        public static ulong ToUInt64(byte[] buffer)
        {
            return BitConverter.ToUInt64(buffer, 0);
        }
        //-----------------------------------------------------------------------------------------
        public static uint ToUInt32(byte[] buffer)
        {
            return BitConverter.ToUInt32(buffer, 0);
        }
        //-----------------------------------------------------------------------------------------
        public static Decimal ToDecimal(byte[] buffer)
        {
            using (var stream = new MemoryStream(buffer))
            {
                using (var reader = new BinaryReader(stream))
                {
                    return reader.ReadDecimal();
                }
            }
        }
        //-----------------------------------------------------------------------------------------
        public static double ToDouble(byte[] buffer)
        {
            return BitConverter.ToDouble(buffer, 0);
        }
        //-----------------------------------------------------------------------------------------
        public static byte[] ToByteArray(byte[] buffer)
        {
            return buffer;
        }
        //-----------------------------------------------------------------------------------------
        public static void ByteArrayToFile(string path, byte[] data)
        {
            try
            {
                File.WriteAllBytes(path, data);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error writing to file: {path}.", ex);
            }
        }
        //-----------------------------------------------------------------------------------------
        public static byte[] ByteArrayToZipByteArray(byte[] value)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var zipStream = new BufferedStream(new GZipStream(memoryStream, CompressionMode.Compress)))
                {
                    zipStream.Write(value, 0, value.Length);
                }
                return memoryStream.ToArray();
            }
        }
        //-----------------------------------------------------------------------------------------
    }
}
