using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;

namespace Atom
{
    public static partial class Conversion
    {
        //-----------------------------------------------------------------------------------------
        public static int ToInt(string value)
        {
            /*
            // strip the leading 0x
            if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                value = value.Substring(2);
                if (!uint.TryParse(value, NumberStyles.HexNumber, null, out var ui))
                    throw new Exception("cannot convert: " + value + " to type: int");

                return ToInt(ui);
            }
            */
            if (!int.TryParse(value, out var i))
                throw new Exception("Cannot convert: " + value + " to type: int.");
            return i;
        }
        //-----------------------------------------------------------------------------------------
        public static uint ToUInt32(string value)
        {
            uint ui;
            // strip the leading 0x
            if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                value = value.Substring(2);
                if (!uint.TryParse(value, NumberStyles.HexNumber, null, out ui))
                    throw new Exception("Cannot convert: " + value + " to type: uint.");
            }
            if (!uint.TryParse(value, out ui))
                throw new Exception("Cannot convert: " + value + " to type: uint.");

            return ui;
        }
        //-----------------------------------------------------------------------------------------
        public static short ToInt16(string value)
        {
            // strip the leading 0x
            if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                value = value.Substring(2);
                if (!UInt16.TryParse(value, NumberStyles.HexNumber, null, out var ui))
                    throw new Exception("конвертация в Int16 не возможна");

                return ToInt16(ui);
            }
            if (!Int16.TryParse(value, out short i))
                throw new Exception("конвертация в Int16 не возможна");
            return i;
        }
        //-----------------------------------------------------------------------------------------
        public static ushort ToUInt16(string value)
        {
            ushort ui;
            // strip the leading 0x
            if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                value = value.Substring(2);
                if (!ushort.TryParse(value, NumberStyles.HexNumber, null, out ui))
                    throw new Exception("конвертация в UInt16 не возможна");
            }
            if (!ushort.TryParse(value, out ui))
                throw new Exception("конвертация в UInt16 не возможна");

            return ui;
        }
        //-----------------------------------------------------------------------------------------
        public static sbyte ToInt8(string value)
        {
            // strip the leading 0x
            if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                value = value.Substring(2);
                if (!byte.TryParse(value, NumberStyles.HexNumber, null, out byte ui))
                    throw new Exception("конвертация в Int8 не возможна");

                return ToInt8(ui);
            }
            if (!sbyte.TryParse(value, out sbyte i))
                throw new Exception("конвертация в Int8 не возможна");
            return i;
        }
        //-----------------------------------------------------------------------------------------
        public static byte ToUInt8(string value)
        {
            byte ui;
            // strip the leading 0x
            if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                value = value.Substring(2);
                if (!byte.TryParse(value, NumberStyles.HexNumber, null, out ui))
                    throw new Exception("конвертация в UInt8 не возможна");
            }
            if (!byte.TryParse(value, out ui))
                throw new Exception("конвертация в UInt8 не возможна");

            return ui;
        }
        //-----------------------------------------------------------------------------------------
        public static Int64 ToInt64(string value)
        {
            // strip the leading 0x
            if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                value = value.Substring(2);
                if (!UInt64.TryParse(value, NumberStyles.HexNumber, null, out var ui))
                    throw new Exception("конвертация в Int64 не возможна");

                return ToInt64(ui);
            }
            if (!Int64.TryParse(value, out long i))
                throw new Exception("конвертация в Int64 не возможна");
            return i;
        }
        //-----------------------------------------------------------------------------------------
        public static UInt64 ToUInt64(string value)
        {
            ulong ui;
            // strip the leading 0x
            if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                value = value.Substring(2);
                if (!ulong.TryParse(value, NumberStyles.HexNumber, null, out ui))
                    throw new Exception("конвертация в UInt64 не возможна");
            }
            if (!ulong.TryParse(value, out ui))
                throw new Exception("конвертация в UInt64 не возможна");

            return ui;
        }
        //-----------------------------------------------------------------------------------------
        public static bool ToBool(string value)
        {
            return value switch
            {
                "0" => false,
                "1" => true,
                _ => bool.Parse(value)
            };
        }
        //-----------------------------------------------------------------------------------------
        public static double ToDouble(string value)
        {
            var decimalSep = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
            value = value.Replace(".", decimalSep);
            value = value.Replace(",", decimalSep);

            if (!double.TryParse(value, out var d))
                throw new Exception("конвертация в double не возможна");
            return d;
        }
        //-----------------------------------------------------------------------------------------
        public static float ToFloat(string value)
        {
            var decimalSep = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
            value = value.Replace(".", decimalSep);
            value = value.Replace(",", decimalSep);

            if (!float.TryParse(value, out var f))
                throw new Exception("Cannot convert: " + value + " to type: float.");
            return f;
        }
        //-----------------------------------------------------------------------------------------
        public static decimal ToDecimal(string value)
        {
            var decimalSep = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
            value = value.Replace(".", decimalSep);
            value = value.Replace(",", decimalSep);

            if (!decimal.TryParse(value, out var d))
                throw new Exception("конвертация в decimal не возможна");
            return d;
        }
        //-----------------------------------------------------------------------------------------
        public static string ToString(string value)
        {
            return value;
        }
        //-----------------------------------------------------------------------------------------
        /*
        public static byte ToVarType(string value)
        {
            return VarType.ToVarType(value);
        }
        */
        //-----------------------------------------------------------------------------------------
        public static string ToHexString(string value)
        {
            return ByteArrayToHexString(ToByteArray(value));
        }
        //-----------------------------------------------------------------------------------------
        public static void StringToFile(string path, string data)
        {
            try
            {
                File.WriteAllText(path, data);
            }
            catch (Exception ex)
            {
                throw new Exception("error writing to file: " + path, ex);
            }
        }
        //-----------------------------------------------------------------------------------------
        public static IPAddress ToIpAddress(string ip)
        {
            if (!IsIpAddress(ip))
                throw new Exception("конвертация в IPAddress не возможна");

            return IPAddress.Parse(ip);
        }
        //-----------------------------------------------------------------------------------------
        public static GuidEx ToMd5(string value)
        {
            using (var md5 = MD5.Create())
                return new GuidEx(md5.ComputeHash(ToByteArray(value)));
        }
        //-----------------------------------------------------------------------------------------
        public static GuidEx ToMd5(byte[] value, int offset, int count)
        {
            using (var md5 = MD5.Create())
                return new GuidEx(md5.ComputeHash(value, offset, count));
        }
        //-----------------------------------------------------------------------------------------
        public static GuidEx ToMd5(byte[] value, ref int offset, int count)
        {
            var uid = ToMd5(value, offset, count);
            offset += count;
            return uid;
        }
        //-----------------------------------------------------------------------------------------
        public static bool FastCheckInt(string sources, int begin, int end)
        {
            if (end - begin == 0)
                return false;

            var ch = sources[begin];

            if (ch == '-' || ch == '+')
            {
                begin++;

                if (end - begin == 0)
                    return false;
            }

            for (var i = begin; i != end; i++)
            {
                ch = sources[i];
                if (ch < '0' || ch > '9')
                    return false;
            }
            return true;
        }
        //-----------------------------------------------------------------------------------------
        public static bool FastCheckFloat(string sources, int begin, int end)
        {
            if (end - begin == 0)
                return false;

            var ch = sources[begin];

            if (ch == '-' || ch == '+')
            {
                begin++;

                if (end - begin == 0)
                    return false;
            }

            var point = -1;

            for (var i = begin; i != end; i++)
            {
                ch = sources[i];

                if ('0' <= ch && ch <= '9') 
                    continue;

                if (ch != '.') 
                    return false;

                if (point >= 0)
                    return false;

                point = i;
            }

            if (point == -1)
                return true;
            if (point - begin == 0)
                return false;
            if (end - 1 - point == 0)
                return false;

            return true;
        }
        //-----------------------------------------------------------------------------------------
        private static bool FastCheckTrue(string sources, int begin)
        {
            if (sources[begin + 0] != 'T' && sources[begin + 0] != 't')
                return false;

            if (sources[begin + 1] != 'r')
                return false;

            if (sources[begin + 2] != 'u')
                return false;

            if (sources[begin + 3] != 'e')
                return false;

            return true;
        }
        //-----------------------------------------------------------------------------------------
        private static bool FastCheckFalse(string sources, int begin)
        {
            if (sources[begin + 0] != 'F' && sources[begin + 0] != 'f')
                return false;

            if (sources[begin + 1] != 'a')
                return false;

            if (sources[begin + 2] != 'l')
                return false;

            if (sources[begin + 3] != 's')
                return false;

            if (sources[begin + 4] != 'e')
                return false;

            return true;
        }
        //-----------------------------------------------------------------------------------------
        public static bool FastCheckBool(string sources, int begin, int end)
        {
            if (end - begin == 1)
            {
                var ch = sources[begin];

                if (ch == '0' || ch == '1')
                    return true;
            }

            if (end - begin == 4)
                return FastCheckTrue(sources, begin);

            if (end - begin == 5)
                return FastCheckFalse(sources, begin);

            return false;
        }
        //-----------------------------------------------------------------------------------------
        public static bool FastCheckName(string sources, int begin, int end)
        {
            for (var i = begin; i != end; i++)
            {
                var ch = sources[i];
                if (!('A' <= ch && ch <= 'Z') && !('a' <= ch && ch <= 'z') && ch != '_')
                    return false;
            }
            return true;
        }
        //-----------------------------------------------------------------------------------------
        public static string NameValidation(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new Exception("name cannot be null or empty");

            if (!FastCheckName(name, 0, name.Length))
                throw new Exception("invalid name: " + name + ", may contain only a-Z characters");

            return name;
        }
        //-----------------------------------------------------------------------------------------
        internal static string NameValidationIgnoringNullOrWhiteSpace(string name)
        {
            if (name == null)
                return null;

            if (string.IsNullOrWhiteSpace(name))
                return null;

            if (!FastCheckName(name, 0, name.Length))
                throw new Exception("invalid name: " + name + ", may contain only a-Z characters");

            return name;
        }
        //-----------------------------------------------------------------------------------------
    }
}
