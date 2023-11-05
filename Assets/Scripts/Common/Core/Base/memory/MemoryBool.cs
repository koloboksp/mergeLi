using System.Runtime.InteropServices;

namespace Atom
{
    public static partial class Memory
    {
        //-----------------------------------------------------------------------------------------
        [StructLayout(LayoutKind.Explicit)]
        private struct BoolUnion
        {
            [FieldOffset(0)]
            public byte Byte0;

            [FieldOffset(0)]
            public bool Value;
        }
        //-----------------------------------------------------------------------------------------
        public static int GetSizeOfPackedBool()
        {
            return sizeof(bool);
        }
        //-----------------------------------------------------------------------------------------
        public static void Packing(byte[] buffer, int offset, bool value)
        {
            var i = new BoolUnion { Value = value };

            buffer[offset + 0] = i.Byte0;
        }
        //-----------------------------------------------------------------------------------------
        public static void Packing(byte[] buffer, ref int offset, bool value)
        {
            var i = new BoolUnion { Value = value };

            buffer[offset + 0] = i.Byte0;

            offset += GetSizeOfPackedBool();
        }
        //-----------------------------------------------------------------------------------------
        public static bool UnpackingBool(byte[] buffer)
        {
            return UnpackingBool(buffer, 0);
        }
        //-----------------------------------------------------------------------------------------
        public static bool UnpackingBool(byte[] buffer, int offset)
        {
            var i = new BoolUnion
            {
                Byte0 = buffer[offset + 0]
            };
            return i.Value;
        }
        //-----------------------------------------------------------------------------------------
        public static bool UnpackingBool(byte[] buffer, ref int offset)
        {
            var i = new BoolUnion
            {
                Byte0 = buffer[offset + 0]
            };
            offset += GetSizeOfPackedBool();
            return i.Value;
        }
        //-----------------------------------------------------------------------------------------

    }
}

