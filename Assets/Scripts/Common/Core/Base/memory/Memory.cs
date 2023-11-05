using System;


namespace Atom
{
    public static partial class Memory
    {
        //-----------------------------------------------------------------------------------------
        public static bool Memcmp(byte[] value1, byte[] value2)
        {
            if (value1 == null && value2 == null)
                return true;
            if (value1 == null || value2 == null)
                return false;
            if (value1.Length != value2.Length)
                return false;

            for (var i = 0; i != value1.Length; ++i)
                if (value1[i] != value2[i])
                    return false;

            return true;
        }
        //-----------------------------------------------------------------------------------------
        public static bool Memcmp(byte[] value1, int offsetValue1, byte[] value2, int offsetValue2, int size)
        {
            if (value1 == null && value2 == null)
                return true;

            if (value1 == null || value2 == null)
                return false;

            for (var i = 0; i != size; ++i)
                if (value1[i + offsetValue1] != value2[i + offsetValue2])
                    return false;
  
            return true;
        }
        //-----------------------------------------------------------------------------------------
        public static bool Memcmp(byte[] value1, ref int offsetValue1, byte[] value2, ref int offsetValue2, int size)
        {
            if (value1 == null && value2 == null)
                return true;

            if (value1 == null || value2 == null)
                return false;

            for (var i = 0; i != size; ++i)
                if (value1[i + offsetValue1] != value2[i + offsetValue2])
                    return false;
     
            offsetValue1 += size;
            offsetValue2 += size;
            return true;
        }
        //-----------------------------------------------------------------------------------------
        public static bool Memcmp(bool[] bufferDec, int offsetDst, bool[] bufferScr, int offsetSrc, int count)
        {
            if (bufferDec == null && bufferScr == null)
                return true;

            if (bufferDec == null || bufferScr == null)
                return false;

            for (var i = 0; i != count; ++i)
                if (bufferDec[i + offsetDst] != bufferScr[i + offsetSrc])
                    return false;

            return true;
        }
        //-----------------------------------------------------------------------------------------
        public static int Memcpy(Array bufferDst, int offsetDst, Array bufferScr, int offsetSrc, int size)
        {
            Buffer.BlockCopy(bufferScr, offsetSrc, bufferDst, offsetDst, size);
            return size;
        }
        //-----------------------------------------------------------------------------------------
        public static void Memcpy(Array bufferDst, ref int offsetDst, Array bufferScr, int offsetSrc, int size)
        {
            Buffer.BlockCopy(bufferScr, offsetSrc, bufferDst, offsetDst, size);
            offsetDst += size;
        }
        //-----------------------------------------------------------------------------------------
        public static void Memcpy(Array bufferDst, int offsetDst, Array bufferScr, ref int offsetSrc, int size)
        {
            Buffer.BlockCopy(bufferScr, offsetSrc, bufferDst, offsetDst, size);
            offsetSrc += size;
        }
        //-----------------------------------------------------------------------------------------
        public static void Memcpy(Array bufferDst, ref int offsetDst, Array bufferScr, ref int offsetSrc, int size)
        {
            Buffer.BlockCopy(bufferScr, offsetSrc, bufferDst, offsetDst, size);
            offsetDst += size;
            offsetSrc += size;
        }
        //-----------------------------------------------------------------------------------------
        public static void Clear(byte[] bufferDst)
        {
            for (var i = 0; i != bufferDst.Length; ++i)
                bufferDst[i] = 0;
        }
        //-----------------------------------------------------------------------------------------
    }

}
