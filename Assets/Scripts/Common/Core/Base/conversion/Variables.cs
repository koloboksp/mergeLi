using System;
using System.Collections;

namespace Atom
{
    public static partial class Conversion
    {
        //-----------------------------------------------------------------------------------------
        public static bool ToBool(int value)
        {
            return value != 0;
        }
        //-----------------------------------------------------------------------------------------
        public static UInt32 ToUInt32(double value)
        {
            return (UInt32) value;
        }
        //-----------------------------------------------------------------------------------------
        public static UInt32 ToUInt32(Int32 value)
        {
            return (UInt32)value;
        }
        //-----------------------------------------------------------------------------------------
        public static UInt64 ToUInt64(Int64 value)
        {
            return (UInt64)value;
        }
        //-----------------------------------------------------------------------------------------
        public static long ToLong(int value)
        {
            return value;
        }
        //-----------------------------------------------------------------------------------------
        public static Int64 ToInt64(Int64 value)
        {
            return value;
        }
        //-----------------------------------------------------------------------------------------
        public static Int64 ToInt64(double value)
        {
            return (Int64)value;
        }
        //-----------------------------------------------------------------------------------------
        public static UInt64 ToUInt64(double value)
        {
            return (UInt64)value;
        }
        //-----------------------------------------------------------------------------------------
        public static double ToDouble(Int32 value)
        {
            return value;
        }
        //-----------------------------------------------------------------------------------------
        public static double ToDouble(byte value)
        {
            return value;
        }
        //-----------------------------------------------------------------------------------------
        public static double ToDouble(UInt64 value)
        {
            return value;
        }
        //-----------------------------------------------------------------------------------------
        public static double ToDouble(double value)
        {
            return value;
        }
        //-----------------------------------------------------------------------------------------
        public static double ToDouble(float value)
        {
            return value;
        }
        //-----------------------------------------------------------------------------------------
        public static Int64 ToInt64(UInt64 value)
        {
            return (Int64)value;
        }
        //-----------------------------------------------------------------------------------------
        public static Int32 ToInt(UInt64 value)
        {
            return (Int32)value;
        }
        //-----------------------------------------------------------------------------------------
        public static Int32 ToInt(double value)
        {
            return (Int32)value;
        }
        //-----------------------------------------------------------------------------------------
        public static int ToInt(float value)
        {
            return (int)value;
        }
        //-----------------------------------------------------------------------------------------
        public static Int32 ToInt(Int64 value)
        {
            return (Int32)value;
        }
        //-----------------------------------------------------------------------------------------
        public static Int32 ToInt(int value)
        {
            return (Int32)value;
        }
        //-----------------------------------------------------------------------------------------
        public static Int32 ToInt(bool value)
        {
            return value == false ? 1 : 0;
        }
        //-----------------------------------------------------------------------------------------
        public static Int16 ToInt16(UInt16 value)
        {
            return (Int16)value;
        }
        //-----------------------------------------------------------------------------------------
        public static sbyte ToInt8(byte value)
        {
            return (sbyte)value;
        }
        //-----------------------------------------------------------------------------------------
        public static float ToFloat(double value)
        {
            return (float)value;
        }
        //-----------------------------------------------------------------------------------------
        public static float ToFloat(float value)
        {
            return (float)value;
        }
        //-----------------------------------------------------------------------------------------
        public static float ToFloat(int value)
        {
            return (float)value;
        }
        //-----------------------------------------------------------------------------------------
        public static byte ToByte(int value)
        {
            return (byte)value;
        }
        //-----------------------------------------------------------------------------------------
        public static byte ToByte(bool value)
        {
            return value ? (byte) 1 :(byte) 0;
        }
        //-----------------------------------------------------------------------------------------
        public static UInt16 ToUInt16(Int32 value)
        {
            return (UInt16) value;
        }
        //-----------------------------------------------------------------------------------------
        public static UInt16 ToUInt16(double value)
        {
            return (UInt16)value;
        }
        //-----------------------------------------------------------------------------------------
        public static UInt16 ToUInt16(UInt32 value)
        {
            return (UInt16)value;
        }
        //-----------------------------------------------------------------------------------------
        public static bool[] ToBoolArray(byte[] data)
        {
            var ret = new bool[data.Length * 8];
            var array = new BitArray(data);
            array.CopyTo(ret, 0);
            return ret;
        }
        //-----------------------------------------------------------------------------------------
        public static Int32[] ToInt32Array(Int16[] data)
        {
            var ret = new Int32[data.Length];
            for (var i = 0; i != ret.Length; i++)
                ret[i] = data[i];
            return ret;
        }
        //-----------------------------------------------------------------------------------------
        public static Int32[] ToInt32Array(UInt16[] data)
        {
            var ret = new Int32[data.Length];
            for (var i = 0; i != ret.Length; i++)
                ret[i] = data[i];
            return ret;
        }
        //-----------------------------------------------------------------------------------------
        public static ushort[] ToUInt16Array(int[] data)
        {
            var convert = new ushort[data.Length];

            for (var i = 0; i != data.Length; i++)
                convert[i] = ToUInt16(data[i]);

            return convert;
        }
        //-----------------------------------------------------------------------------------------
        public static long ToKByte()
        {
            return 1024;
        }
        //-----------------------------------------------------------------------------------------
        public static long ToMByte()
        {
            return 1024 * 1024;
        }
        //-----------------------------------------------------------------------------------------
        public static long ToGByte()
        {
            return 1024 * 1024 * 1024;
        }
        //-----------------------------------------------------------------------------------------
        public static string BytesToString(long data)
        {
            var b = data;
            var k = data / ToKByte();
            var m = data / ToMByte();
            var g = data / ToGByte();

            if (g != 0)
                return g + "." + (m - g * ToKByte()) + " Gb";
            if (m != 0)
                return m + "." + (k - m * ToKByte()) + " Mb";
            if (k != 0)
                return k + "." + (b - k * ToKByte()) + " Kb";

            return b + " b";
        }
        //-----------------------------------------------------------------------------------------
    }
}
