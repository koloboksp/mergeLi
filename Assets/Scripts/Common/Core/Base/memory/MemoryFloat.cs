using System.Runtime.InteropServices;

namespace Atom
{
    public static partial class Memory
    {
        //-----------------------------------------------------------------------------------------
        [StructLayout(LayoutKind.Explicit)]
        private struct FloatUnion
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
            public float Value;
        }
        //-----------------------------------------------------------------------------------------
        public static int GetSizeOfPackedFloat()
        {
            return sizeof(float);
        }
        //-----------------------------------------------------------------------------------------
        public static void Packing(byte[] buffer, int offset, float value)
        {
            var i = new FloatUnion { Value = value };

            buffer[offset + 0] = i.Byte0;
            buffer[offset + 1] = i.Byte1;
            buffer[offset + 2] = i.Byte2;
            buffer[offset + 3] = i.Byte3;
        }
        //-----------------------------------------------------------------------------------------
        public static void Packing(byte[] buffer, ref int offset, float value)
        {
            var i = new FloatUnion { Value = value };

            buffer[offset + 0] = i.Byte0;
            buffer[offset + 1] = i.Byte1;
            buffer[offset + 2] = i.Byte2;
            buffer[offset + 3] = i.Byte3;
            offset += GetSizeOfPackedFloat();
        }
        //-----------------------------------------------------------------------------------------
        public static float UnpackingFloat(byte[] buffer)
        {
            return UnpackingFloat(buffer, 0);
        }
        //-----------------------------------------------------------------------------------------
        public static float UnpackingFloat(byte[] buffer, int offset)
        {
            var i = new FloatUnion
            {
                Byte0 = buffer[offset + 0],
                Byte1 = buffer[offset + 1],
                Byte2 = buffer[offset + 2],
                Byte3 = buffer[offset + 3]
            };
            return i.Value;
        }
        //-----------------------------------------------------------------------------------------
        public static float UnpackingFloat(byte[] buffer, ref int offset)
        {
            var i = new FloatUnion
            {
                Byte0 = buffer[offset + 0],
                Byte1 = buffer[offset + 1],
                Byte2 = buffer[offset + 2],
                Byte3 = buffer[offset + 3]
            };
            offset += GetSizeOfPackedFloat();
            return i.Value;
        }
        //-----------------------------------------------------------------------------------------
    }
}