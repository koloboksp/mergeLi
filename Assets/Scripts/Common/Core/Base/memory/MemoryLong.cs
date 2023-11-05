using System.Runtime.InteropServices;

namespace Atom
{
    public static partial class Memory
    {
        [StructLayout(LayoutKind.Explicit)]
        private struct LongUnion
        {
            [FieldOffset(0)]
            public byte Byte0;
            [FieldOffset(1)]
            public byte Byte1;
            [FieldOffset(2)]
            public byte Byte2;
            [FieldOffset(3)]
            public byte Byte3;
            [FieldOffset(4)]
            public byte Byte4;
            [FieldOffset(5)]
            public byte Byte5;
            [FieldOffset(6)]
            public byte Byte6;
            [FieldOffset(7)]
            public byte Byte7;

            [FieldOffset(0)]
            public int Int1;

            [FieldOffset(4)]
            public int Int2;

            [FieldOffset(0)]
            public long Value;  
        }
        //-----------------------------------------------------------------------------------------
        public static int GetSizeOfPackedLong()
        {
            return sizeof(long);
        }
        //-----------------------------------------------------------------------------------------
        public static void Packing(byte[] buffer, int offset, long value)
        {
            var i = new LongUnion { Value = value };

            buffer[offset + 0] = i.Byte0;

            buffer[offset + 1] = i.Byte1;
            buffer[offset + 2] = i.Byte2;
            buffer[offset + 3] = i.Byte3;
            buffer[offset + 4] = i.Byte4;
            buffer[offset + 5] = i.Byte5;
            buffer[offset + 6] = i.Byte6;
            buffer[offset + 7] = i.Byte7;
        }
        //-----------------------------------------------------------------------------------------
        public static void Packing(byte[] buffer, ref int offset, long value)
        {
            var i = new LongUnion { Value = value };

            buffer[offset + 0] = i.Byte0;
            buffer[offset + 1] = i.Byte1;
            buffer[offset + 2] = i.Byte2;
            buffer[offset + 3] = i.Byte3;
            buffer[offset + 4] = i.Byte4;
            buffer[offset + 5] = i.Byte5;
            buffer[offset + 6] = i.Byte6;
            buffer[offset + 7] = i.Byte7;
            offset += GetSizeOfPackedLong();
        }
        //-----------------------------------------------------------------------------------------
        public static long UnpackingLong(byte[] buffer)
        {
            return UnpackingLong(buffer, 0);
        }
        //-----------------------------------------------------------------------------------------
        public static long UnpackingLong(byte[] buffer, long offset)
        {
            var i = new LongUnion
            {
                Byte0 = buffer[offset + 0],
                Byte1 = buffer[offset + 1],
                Byte2 = buffer[offset + 2],
                Byte3 = buffer[offset + 3],
                Byte4 = buffer[offset + 4],
                Byte5 = buffer[offset + 5],
                Byte6 = buffer[offset + 6],
                Byte7 = buffer[offset + 7]
            };
            return i.Value;
        }
        //-----------------------------------------------------------------------------------------
        public static long UnpackingLong(byte[] buffer, ref int offset)
        {
            var i = new LongUnion
            {
                Byte0 = buffer[offset + 0],
                Byte1 = buffer[offset + 1],
                Byte2 = buffer[offset + 2],
                Byte3 = buffer[offset + 3],
                Byte4 = buffer[offset + 4],
                Byte5 = buffer[offset + 5],
                Byte6 = buffer[offset + 6],
                Byte7 = buffer[offset + 7]
            };
            offset += GetSizeOfPackedLong();
            return i.Value;
        }
        //-----------------------------------------------------------------------------------------
        public static long Packing(int value1, int value2)
        {
            var i = new LongUnion
            {
                Int1 = value1,
                Int2 = value2
            };
            return i.Value;
        }
        //-----------------------------------------------------------------------------------------
    }
}
