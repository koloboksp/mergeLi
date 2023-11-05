using System.Runtime.InteropServices;

namespace Atom
{
    public static partial class Memory
    {
        [StructLayout(LayoutKind.Explicit)]
        private struct CharUnion
        {
            [FieldOffset(0)]
            public byte Byte0;
            [FieldOffset(1)]
            public byte Byte1;

            [FieldOffset(0)]
            public char Value;
        }
        //-----------------------------------------------------------------------------------------
        public static int GetSizeOfPackedString(string s)
        {
            return sizeof(int) + s.Length * 2;
        }
        //-----------------------------------------------------------------------------------------
        //unicode
        public static void Packing(byte[] buffer, int offset, string value)
        {
            Packing(buffer, ref offset, value);
        }
        //-----------------------------------------------------------------------------------------
        //unicode
        public static void Packing(byte[] buffer, ref int offset, string value)
        {
            Packing(buffer, ref offset, value.Length);

            for (var i = 0; i != value.Length; ++i)
            {
                var c = new CharUnion{ Value = value[i] };

                buffer[offset] = c.Byte0;
                offset++;
                buffer[offset] = c.Byte1;
                offset++;
            }
        }
        //-----------------------------------------------------------------------------------------
        public static string UnpackingString(byte[] buffer, int offset)
        {
            return UnpackingString(buffer, ref offset);
        }
        //-----------------------------------------------------------------------------------------
        //unicode
        public static string UnpackingString(byte[] buffer, ref int offset)
        {
            var len = UnpackingInt(buffer, ref offset);

            if (len == 0)
                return "";

            var array = new char[len];

            for (var i = 0; i != len; ++i)
            {
                var b0 = buffer[offset];
                offset++;
                var b1 = buffer[offset];
                offset++;

                var c = new CharUnion
                {
                    Byte0 = b0,
                    Byte1 = b1
                };

                array[i] = c.Value;
            }

            return new string(array);
        }
        //-----------------------------------------------------------------------------------------
    }
}
