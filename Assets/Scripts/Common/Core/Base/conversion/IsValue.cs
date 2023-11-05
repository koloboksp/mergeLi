using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Atom
{
    public static partial class Conversion
    {
        //-----------------------------------------------------------------------------------------
        public static bool IsMacAddress(string mac)
        {
            if (string.IsNullOrWhiteSpace(mac))
                return false;
            //0a:1b:3c:4d:5e:6f
            //0a-1b-3c-4d-5e-6f
            //0a1b.3c4d.5e6f
            return Regex.IsMatch(mac, "^([0-9a-fA-F]{2}(-|:)){5}([0-9a-fA-F]{2})$|([0-9a-fA-F]{4}.){2}([0-9a-fA-F]{4})$");
        }
        //-----------------------------------------------------------------------------------------
        public static bool IsIpAddress(string ip)
        {
            if (string.IsNullOrWhiteSpace(ip))
                return false;
            //000.000.000.000
            //192.168.0.1
            return Regex.IsMatch(ip, "^([0-9]{1,3}\\.){3}([0-9]{1,3})$");
        }
        //-----------------------------------------------------------------------------------------
        public static bool IsInt8(string value)
        {
            // strip the leading 0x
            if (!value.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) 
                return sbyte.TryParse(value, out _);

            value = value.Substring(2);
            return byte.TryParse(value, NumberStyles.HexNumber, null, out _);
        }
        //-----------------------------------------------------------------------------------------
        public static bool IsInt16(string value)
        {
            // strip the leading 0x
            if (!value.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) 
                return short.TryParse(value, out _);

            value = value.Substring(2);
            return ushort.TryParse(value, NumberStyles.HexNumber, null, out _);
        }
        //-----------------------------------------------------------------------------------------
        public static bool IsInt(string value)
        {
            // strip the leading 0x
            if (!value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                return int.TryParse(value, out _);

            value = value.Substring(2);
            return uint.TryParse(value, NumberStyles.HexNumber, null, out _);
        }
        //-----------------------------------------------------------------------------------------
        public static bool IsInt64(string value)
        {
            // strip the leading 0x
            if (!value.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) 
                return long.TryParse(value, out _);

            value = value.Substring(2);
            return ulong.TryParse(value, NumberStyles.HexNumber, null, out _);
        }
        //-----------------------------------------------------------------------------------------
        public static bool IsUInt8(string value)
        {
            // strip the leading 0x
            if (!value.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) 
                return byte.TryParse(value, out _);

            value = value.Substring(2);
            return byte.TryParse(value, NumberStyles.HexNumber, null, out _);
        }
        //-----------------------------------------------------------------------------------------
        public static bool IsUInt16(string value)
        {
            // strip the leading 0x
            if (!value.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) 
                return ushort.TryParse(value, out _);

            value = value.Substring(2);
            return ushort.TryParse(value, NumberStyles.HexNumber, null, out _);
        }
        //-----------------------------------------------------------------------------------------
        public static bool IsUInt32(string value)
        {
            // strip the leading 0x
            if (!value.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) 
                return uint.TryParse(value, out _);

            value = value.Substring(2);
            return uint.TryParse(value, NumberStyles.HexNumber, null, out _);
        }
        //-----------------------------------------------------------------------------------------
        public static bool IsUInt64(string value)
        {
            // strip the leading 0x
            if (!value.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) 
                return ulong.TryParse(value, out _);

            value = value.Substring(2);
            return ulong.TryParse(value, NumberStyles.HexNumber, null, out _);
        }
        //-----------------------------------------------------------------------------------------
        public static bool IsDouble(string value)
        {
            var decimalSep = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
            value = value.Replace(".", decimalSep);
            value = value.Replace(",", decimalSep);

            return double.TryParse(value, out _);
        }
        //-----------------------------------------------------------------------------------------
        public static bool IsFloat(string value)
        {
            var decimalSep = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
            value = value.Replace(".", decimalSep);
            value = value.Replace(",", decimalSep);

            return float.TryParse(value, out _);
        }
        //-----------------------------------------------------------------------------------------
        public static bool IsDecimal(string value)
        {
            var decimalSep = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
            value = value.Replace(".", decimalSep);
            value = value.Replace(",", decimalSep);

            return decimal.TryParse(value, out _);
        }
        //-----------------------------------------------------------------------------------------
        public static bool IsString(string value)
        {
            return true;
        }
        //-----------------------------------------------------------------------------------------
        public static bool IsBool(string value)
        {
            if (value == "0" || value == "1")
                return true;

            return bool.TryParse(value, out _);
        }
        //-----------------------------------------------------------------------------------------
        public static bool IsByteArray(string value, bool hex = false)
        {
            if (!hex)
                return true;

            if (value.Length % 2 != 0)
                return false;

            for (var i = 0; i != value.Length; i++)
            {
                var ch = value[i];
                if (!('0' <= ch && ch <= '9' || 'A' <= ch && ch <= 'F' || 'a' <= ch && ch <= 'e'))
                    return false;
            }

            return true;
        }
        //-----------------------------------------------------------------------------------------
    }
}
