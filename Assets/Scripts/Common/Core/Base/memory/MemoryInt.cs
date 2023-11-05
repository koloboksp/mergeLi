using System.Runtime.InteropServices;

namespace Atom
{
    public static partial class Memory
    {
        //-----------------------------------------------------------------------------------------
        [StructLayout(LayoutKind.Explicit)]
        private struct IntUnion
        {
            [FieldOffset(0)]
            public byte Byte0;
            [FieldOffset(1)]
            public byte Byte1;
            [FieldOffset(2)]
            public byte Byte2;
            [FieldOffset(3)]
            public byte Byte3;

            [FieldOffset(0)]
            public int Value;
        }
        //-----------------------------------------------------------------------------------------
        public static int GetSizeOfPackedInt()
        {
            return sizeof(int);
        }
        //-----------------------------------------------------------------------------------------
        public static void Packing(byte[] buffer, int offset, int value)
        {
            var i = new IntUnion { Value = value };

            buffer[offset + 0] = i.Byte0;
            buffer[offset + 1] = i.Byte1;
            buffer[offset + 2] = i.Byte2;
            buffer[offset + 3] = i.Byte3;
        }
        //-----------------------------------------------------------------------------------------
        public static void Packing(byte[] buffer, ref int offset, int value)
        {
            var i = new IntUnion { Value = value };

            buffer[offset + 0] = i.Byte0;
            buffer[offset + 1] = i.Byte1;
            buffer[offset + 2] = i.Byte2;
            buffer[offset + 3] = i.Byte3;
            offset += GetSizeOfPackedInt();
        }
        //-----------------------------------------------------------------------------------------
        public static int UnpackingInt(byte[] buffer)
        {
            return UnpackingInt(buffer, 0);
        }
        //-----------------------------------------------------------------------------------------
        public static int UnpackingInt(byte[] buffer, int offset)
        {
            var i = new IntUnion
            {
                Byte0 = buffer[offset + 0],
                Byte1 = buffer[offset + 1],
                Byte2 = buffer[offset + 2],
                Byte3 = buffer[offset + 3]
            };
            return i.Value;
        }
        //-----------------------------------------------------------------------------------------
        public static int UnpackingInt(byte[] buffer, ref int offset)
        {
            var i = new IntUnion
            {
                Byte0 = buffer[offset + 0],
                Byte1 = buffer[offset + 1],
                Byte2 = buffer[offset + 2],
                Byte3 = buffer[offset + 3]
            };
            offset += GetSizeOfPackedInt();
            return i.Value;
        }
        //-----------------------------------------------------------------------------------------
    }
}
