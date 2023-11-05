using System.Runtime.InteropServices;

namespace Atom
{
    public static partial class Memory
    {
        //-----------------------------------------------------------------------------------------
        [StructLayout(LayoutKind.Explicit)]
        private struct DoubleUnion
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
            public double Value;
        }
        //-----------------------------------------------------------------------------------------
        public static int GetSizeOfPackedDouble()
        {
            return sizeof(double);
        }
        //-----------------------------------------------------------------------------------------
        public static void Packing(byte[] buffer, int offset, double value)
        {
            var i = new DoubleUnion { Value = value };

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
        public static void Packing(byte[] buffer, ref int offset, double value)
        {
            var i = new DoubleUnion { Value = value };

            buffer[offset + 0] = i.Byte0;
            buffer[offset + 1] = i.Byte1;
            buffer[offset + 2] = i.Byte2;
            buffer[offset + 3] = i.Byte3;
            buffer[offset + 4] = i.Byte4;
            buffer[offset + 5] = i.Byte5;
            buffer[offset + 6] = i.Byte6;
            buffer[offset + 7] = i.Byte7;
            offset += GetSizeOfPackedDouble();
        }
        //-----------------------------------------------------------------------------------------
        
        public static double UnpackingDouble(byte[] buffer)
        {
            return UnpackingDouble(buffer, 0);
        }
        //-----------------------------------------------------------------------------------------
        public static double UnpackingDouble(byte[] buffer, int offset)
        {
            var i = new DoubleUnion
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
        public static double UnpackingDouble(byte[] buffer, ref int offset)
        {
            var i = new DoubleUnion
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
            offset += GetSizeOfPackedDouble();
            return i.Value;
        }
        //-----------------------------------------------------------------------------------------
    }
}