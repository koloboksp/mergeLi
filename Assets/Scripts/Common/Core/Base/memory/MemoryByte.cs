namespace Atom
{
    public static partial class Memory
    {
        public static int GetSizeOfPackedByte()
        {
            return sizeof(byte);
        }
        //-----------------------------------------------------------------------------------------
        public static void Packing(byte[] buffer, int offset, byte value)
        {
            buffer[offset] = value;
        }
        //-----------------------------------------------------------------------------------------
        public static void Packing(byte[] buffer, ref int offset, byte value)
        {
            buffer[offset] = value;
            offset += GetSizeOfPackedByte();
        }
        //-----------------------------------------------------------------------------------------
        public static byte UnpackingByte(byte[] buffer)
        {
            return UnpackingByte(buffer, 0);
        }
        //-----------------------------------------------------------------------------------------
        public static byte UnpackingByte(byte[] buffer, int offset)
        {
            return buffer[offset];
        }
        //-----------------------------------------------------------------------------------------
        public static byte UnpackingByte(byte[] buffer, ref int offset)
        {
            var value = buffer[offset];
            offset += GetSizeOfPackedByte();
            return value;
        }
        //-----------------------------------------------------------------------------------------
    }
}