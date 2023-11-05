namespace Atom
{
    public static partial class Memory
    {
        //-----------------------------------------------------------------------------------------
        public static int GetSizeOfPackedParam()
        {
            return 8;
        }
        //-----------------------------------------------------------------------------------------
        public static void Packing(byte[] buffer, int offset, Param value)
        {
            Param.Packing(buffer, offset, value);
        }
        //-----------------------------------------------------------------------------------------
        public static void Packing(byte[] buffer, ref int offset, Param value)
        {
            Param.Packing(buffer, ref offset, value);
        }
        //-----------------------------------------------------------------------------------------
        public static Param UnpackingParam(byte[] buffer)
        {
            return Param.UnpackingParam(buffer, 0);
        }
        //-----------------------------------------------------------------------------------------
        public static Param UnpackingParam(byte[] buffer, int offset)
        {
            return Param.UnpackingParam(buffer, 0);
        }
        //-----------------------------------------------------------------------------------------
        public static bool UnpackingParam(byte[] buffer, ref int offset)
        {
            return Param.UnpackingParam(buffer, ref offset);
        }
        //-----------------------------------------------------------------------------------------
    }
}

