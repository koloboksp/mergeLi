using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Atom
{
    public static partial class Conversion
    {
        //-----------------------------------------------------------------------------------------
        public static string ToString(bool value)
        {
            return value.ToString();
        }
        //-----------------------------------------------------------------------------------------
        public static string ToString(bool[] value)
        {
            var s = "";
            for (var i = 0; i != value.Length; i++)
                s += value[i] + " ";
            return s;
        }
        //-----------------------------------------------------------------------------------------
        public static string ToString(double value)
        {
            return value.ToString(CultureInfo.InvariantCulture).Replace(",", ".");
        }
        //-----------------------------------------------------------------------------------------
        public static string ToString(Int64 value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
        //-----------------------------------------------------------------------------------------
        public static string ToString(UInt64 value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
        //-----------------------------------------------------------------------------------------
        public static string ToString(Int32 value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
        //-----------------------------------------------------------------------------------------
        public static string ToString(UInt32 value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
        //-----------------------------------------------------------------------------------------
        public static string ToString(Int16 value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
        //-----------------------------------------------------------------------------------------
        public static string ToString(UInt16 value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
        //-----------------------------------------------------------------------------------------
        public static string ToString(byte value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
        //-----------------------------------------------------------------------------------------
        public static string ToString(sbyte value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
        //-----------------------------------------------------------------------------------------
        public static string ToString(float value)
        {
            return value.ToString(CultureInfo.InvariantCulture).Replace(",", ".");
        }
        //-----------------------------------------------------------------------------------------
        public static string ToString(float? value)
        {
            if (!value.HasValue)
                return "null";
            return value.Value.ToString(CultureInfo.InvariantCulture).Replace(",", ".");
        }
        //-----------------------------------------------------------------------------------------
        public static string ToString(decimal value)
        {
            return value.ToString(CultureInfo.InvariantCulture).Replace(",", ".");
        }
        //-----------------------------------------------------------------------------------------
        /*
        public static string ToString (ValueType value)
        {
            return value.ToString();
        }
        */
        //-----------------------------------------------------------------------------------------
        public static string ToString(byte[] value)
        {
            var str = new char[value.Length / 2];
            Memory.Memcpy(str, 0, value, 0, value.Length);
            return new string(str);
        }
        //-----------------------------------------------------------------------------------------
        public static char ToChar(byte[] buffer)
        {
			return BitConverter.ToChar(buffer, 0); 
        }
        //-----------------------------------------------------------------------------------------
        //каждый символ будет переведён в формат 00-ff
        public static string ByteArrayToHexString(byte[] value)
        {
            var result = new StringBuilder(value.Length * 2);
            const string hexAlphabet = "0123456789abcdef";

            foreach (var i in value)
            {
                result.Append(hexAlphabet[i >> 4]);
                result.Append(hexAlphabet[i & 0xF]);
            }

            return result.ToString();
        }
        //-----------------------------------------------------------------------------------------
        public static string ByteArrayToDecString(byte[] value, char separator = ' ')
        {
            var result = new StringBuilder(value.Length * 3);
            foreach (var i in value)
            {
                result.Append(i);
                result.Append(separator);
            }
            result.Remove(result.Length - 1, 1);
            return result.ToString();
        }
        //-----------------------------------------------------------------------------------------
        public static string FileToString(string path)
        {
            try
            {
                return File.ReadAllText(path);
            }
            catch (Exception ex)
            {
                throw new Exception("error opening file: " + path, ex);
            }
        }
        //-----------------------------------------------------------------------------------------
        public static string ToString(float value, int digits)
        {
            var format = "{0:0." + new string('#', digits) + "}";
            var result = string.Format(format, value);
            result = result.ToString(CultureInfo.InvariantCulture).Replace(",", ".");
            return result;
        }
        //-----------------------------------------------------------------------------------------
    }
}
