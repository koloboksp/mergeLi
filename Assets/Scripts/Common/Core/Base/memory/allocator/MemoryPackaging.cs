namespace Atom
{
    //*********************************************************************************************
    public class MemoryPacking : MemoryStorage, IPacking
    {
        internal MemoryPacking(StorageBasePool pool, int level) : base(pool, level) { }
        public MemoryPacking(byte[] data, int offset = 0) : base(data) { mOffset = offset; }
        //-----------------------------------------------------------------------------------------
        //bool
        public IPacking Packing(int offsetBuffer, bool value) { Memory.Packing(Buffer, offsetBuffer, value); return this; }
        public IPacking Packing(bool value) { Memory.Packing(Buffer, ref mOffset, value); return this; }
        //-----------------------------------------------------------------------------------------
        //int
        public IPacking Packing(int offsetBuffer, int value) { Memory.Packing(Buffer, offsetBuffer, value); return this; }
        public IPacking Packing(int value) { Memory.Packing(Buffer, ref mOffset, value); return this; }
        //-----------------------------------------------------------------------------------------
        //float
        public IPacking Packing(int offsetBuffer, float value) { Memory.Packing(Buffer, offsetBuffer, value); return this; }
        public IPacking Packing(float value) { Memory.Packing(Buffer, ref mOffset, value); return this; }
        //-----------------------------------------------------------------------------------------
        //byte
        public IPacking Packing(int offsetBuffer, byte value) { Memory.Packing(Buffer, offsetBuffer, value); return this; }
        public IPacking Packing(byte value) { Memory.Packing(Buffer, ref mOffset, value); return this; }
        //-----------------------------------------------------------------------------------------
        //double
        public IPacking Packing(int offsetBuffer, double value) { Memory.Packing(Buffer, offsetBuffer, value); return this; }
        public IPacking Packing(double value) { Memory.Packing(Buffer, ref mOffset, value); return this; }
        //-----------------------------------------------------------------------------------------
        //short
        public IPacking Packing(int offsetBuffer, short value) { Memory.Packing(Buffer, offsetBuffer, value); return this; }
        public IPacking Packing(short value) { Memory.Packing(Buffer, ref mOffset, value); return this; }
        //----------------------------------------------------------------------------------------- 
        //param
        public IPacking Packing(int offsetBuffer, Param value) { Memory.Packing(Buffer, offsetBuffer, value); return this; }
        public IPacking Packing(Param value) { Memory.Packing(Buffer, ref mOffset, value); return this; }
        //-----------------------------------------------------------------------------------------
        //array
        public IPacking Packing(byte[] array, int offsetArray, int size) { Memory.Packing(Buffer, ref mOffset, array, offsetArray, size); return this; }
        public IPacking Packing(byte[] array) { Memory.Packing(Buffer, ref mOffset, array, 0, array.Length); return this; }
        //-----------------------------------------------------------------------------------------
        //string
        public IPacking Packing(int offsetBuffer, string value) { Memory.Packing(Buffer, offsetBuffer, value); return this; }
        public IPacking Packing(string value) { Memory.Packing(Buffer, ref mOffset, value); return this; }
        //-----------------------------------------------------------------------------------------
        //GuidEx
        public IPacking Packing(int offsetBuffer, GuidEx value) { Memory.Packing(Buffer, offsetBuffer, value); return this; }
        public IPacking Packing(GuidEx value) { Memory.Packing(Buffer, ref mOffset, value); return this; }
        //-----------------------------------------------------------------------------------------
        //unpacking
        public IPacking Packing(MemoryUnpacking value) { Memory.Memcpy(Buffer, ref mOffset, value.Buffer, 0, value.Buffer.Length); return this; }
    }
    //*********************************************************************************************
}
