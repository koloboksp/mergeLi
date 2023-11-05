using System.Runtime.InteropServices;

namespace Atom
{
    public static partial class Memory
    {
        //-----------------------------------------------------------------------------------------
        [StructLayout(LayoutKind.Explicit)]
        private struct ShortUnion
        {
            [FieldOffset(0)]
            public byte Byte0;
            [FieldOffset(1)]
            public byte Byte1;

            [FieldOffset(0)]
            public short Value;
        }
        //-----------------------------------------------------------------------------------------
        public static int GetSizeOfPackedShort()
        {
            return sizeof(short);
        }
        //-----------------------------------------------------------------------------------------
        public static void Packing(byte[] buffer, int offset, short value)
        {
            var i = new ShortUnion { Value = value };

            buffer[offset + 0] = i.Byte0;
            buffer[offset + 1] = i.Byte1;
        }
        //-----------------------------------------------------------------------------------------
        public static void Packing(byte[] buffer, ref int offset, short value)
        {
            var i = new ShortUnion { Value = value };

            buffer[offset + 0] = i.Byte0;
            buffer[offset + 1] = i.Byte1;
            offset += GetSizeOfPackedShort();
        }
        //-----------------------------------------------------------------------------------------
        public static short UnpackingShort(byte[] buffer)
        {
            return UnpackingShort(buffer, 0);
        }
        //-----------------------------------------------------------------------------------------
        public static short UnpackingShort(byte[] buffer, int offset)
        {
            var i = new ShortUnion
            {
                Byte0 = buffer[offset + 0],
                Byte1 = buffer[offset + 1]
            };
            return i.Value;
        }
        //-----------------------------------------------------------------------------------------
        public static short UnpackingShort(byte[] buffer, ref int offset)
        {
            var i = new ShortUnion
            {
                Byte0 = buffer[offset + 0],
                Byte1 = buffer[offset + 1]
            };
            offset += GetSizeOfPackedShort();
            return i.Value;
        }
        //-----------------------------------------------------------------------------------------
    }
}
