namespace Atom
{
    public static partial class Memory
    {
        //-----------------------------------------------------------------------------------------
        public static int GetSizeOfPackedGuidEx()
        {
            return 16;
        }
        //-----------------------------------------------------------------------------------------
        public static void Packing(byte[] buffer, int offset, GuidEx value)
        {
            var array = value.ToByteArray();

            for (var i = 0; i != array.Length; i++)
                buffer[offset + i] = array[i];
        }
        //-----------------------------------------------------------------------------------------
        public static void Packing(byte[] buffer, ref int offset, GuidEx value)
        {
            var array = value.ToByteArray();

            for (var i = 0; i != array.Length; i++)
                buffer[offset + i] = array[i];

            offset += GetSizeOfPackedGuidEx();
        }
        //-----------------------------------------------------------------------------------------
        public static GuidEx UnpackingGuidEx(byte[] buffer)
        {
            return new GuidEx(buffer);
        }
        //-----------------------------------------------------------------------------------------
        public static GuidEx UnpackingGuidEx(byte[] buffer, int offset)
        {
            return new GuidEx(buffer, offset);
        }
        //-----------------------------------------------------------------------------------------
        public static GuidEx UnpackingGuidEx(byte[] buffer, ref int offset)
        {
            var result = new GuidEx(buffer, offset);
            offset += GetSizeOfPackedGuidEx();
            return result;
        }
        //-----------------------------------------------------------------------------------------
    }
}
