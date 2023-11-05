using System.Runtime.InteropServices;

namespace Atom
{
    public static partial class Memory
    {
        //-----------------------------------------------------------------------------------------
        [StructLayout(LayoutKind.Explicit)]
        private struct UIntUnion
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
            public uint Value;
        }
        //-----------------------------------------------------------------------------------------
        public static int GetSizeOfPackedUInt()
        {
            return sizeof(uint);
        }
        //-----------------------------------------------------------------------------------------
        public static void Packing(byte[] buffer, int offset, uint value)
        {
            var i = new UIntUnion { Value = value };

            buffer[offset + 0] = i.Byte0;
            buffer[offset + 1] = i.Byte1;
            buffer[offset + 2] = i.Byte2;
            buffer[offset + 3] = i.Byte3;
        }
        //-----------------------------------------------------------------------------------------
        public static void Packing(byte[] buffer, ref int offset, uint value)
        {
            var i = new UIntUnion { Value = value };

            buffer[offset + 0] = i.Byte0;
            buffer[offset + 1] = i.Byte1;
            buffer[offset + 2] = i.Byte2;
            buffer[offset + 3] = i.Byte3;
            offset += GetSizeOfPackedUInt();
        }
        //-----------------------------------------------------------------------------------------
        public static uint UnpackingUInt(byte[] buffer)
        {
            return UnpackingUInt(buffer, 0);
        }
        //-----------------------------------------------------------------------------------------
        public static uint UnpackingUInt(byte[] buffer, int offset)
        {
            var i = new UIntUnion
            {
                Byte0 = buffer[offset + 0],
                Byte1 = buffer[offset + 1],
                Byte2 = buffer[offset + 2],
                Byte3 = buffer[offset + 3]
            };
            return i.Value;
        }
        //-----------------------------------------------------------------------------------------
        public static uint UnpackingUInt(byte[] buffer, ref int offset)
        {
            var i = new UIntUnion
            {
                Byte0 = buffer[offset + 0],
                Byte1 = buffer[offset + 1],
                Byte2 = buffer[offset + 2],
                Byte3 = buffer[offset + 3]
            };
            offset += GetSizeOfPackedUInt();
            return i.Value;
        }
        //-----------------------------------------------------------------------------------------
    }
}
