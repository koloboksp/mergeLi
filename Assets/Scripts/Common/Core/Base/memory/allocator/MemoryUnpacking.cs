namespace Atom
{
    //*********************************************************************************************
    public class MemoryUnpacking : MemoryStorage, IUnpacking
    {
        internal MemoryUnpacking(StorageBasePool pool, int level) : base(pool, level) { }
        public MemoryUnpacking(byte[] data, int offset = 0) : base(data) { mOffset = offset; }
        //-----------------------------------------------------------------------------------------
        public void Reset() { mOffset = 0;}
        //-----------------------------------------------------------------------------------------
        //bool
        public bool UnpackingBool(int offsetBuffer) { return Memory.UnpackingBool(Buffer, offsetBuffer); }
        public bool UnpackingBool() { return Memory.UnpackingBool(Buffer, ref mOffset); }
        //-----------------------------------------------------------------------------------------
        //int
        public int UnpackingInt(int offsetBuffer) { return Memory.UnpackingInt(Buffer, offsetBuffer); }
        public int UnpackingInt() { return Memory.UnpackingInt(Buffer, ref mOffset); }
        //-----------------------------------------------------------------------------------------
        //float
        public float UnpackingFloat(int offsetBuffer) { return Memory.UnpackingFloat(Buffer, offsetBuffer); }
        public float UnpackingFloat() { return Memory.UnpackingFloat(Buffer, ref mOffset); }
        //-----------------------------------------------------------------------------------------
        //byte
        public byte UnpackingByte(int offsetBuffer) { return Memory.UnpackingByte(Buffer, offsetBuffer); }
        public byte UnpackingByte() { return Memory.UnpackingByte(Buffer, ref mOffset); }
        //-----------------------------------------------------------------------------------------
        //double
        public double UnpackingDouble(int offsetBuffer) { return Memory.UnpackingDouble(Buffer, offsetBuffer); }
        public double UnpackingDouble() { return Memory.UnpackingDouble(Buffer, ref mOffset); }
        //-----------------------------------------------------------------------------------------
        //short
        public short UnpackingShort(int offsetBuffer) { return Memory.UnpackingShort(Buffer, offsetBuffer); }
        public short UnpackingShort() { return Memory.UnpackingShort(Buffer, ref mOffset); }
        //-----------------------------------------------------------------------------------------
        //param
        public Param UnpackingParam(int offsetBuffer) { return Memory.UnpackingParam(Buffer, offsetBuffer); }
        public Param UnpackingParam() { return Memory.UnpackingParam(Buffer, ref mOffset); }
        //-----------------------------------------------------------------------------------------
        //array
        public byte[] UnpackingArray(int offsetBuffer) { return Memory.UnpackingArray(Buffer, ref offsetBuffer); }
        public byte[] UnpackingArray() { return Memory.UnpackingArray(Buffer, ref mOffset); }
        //-----------------------------------------------------------------------------------------
        //string
        public string UnpackingString(int offset) { return Memory.UnpackingString(Buffer, ref offset); }
        public string UnpackingString() { return Memory.UnpackingString(Buffer, ref mOffset); }
        //-----------------------------------------------------------------------------------------
        //GuidEx
        public GuidEx UnpackingGuidEx(int offset) { return Memory.UnpackingGuidEx(Buffer, ref offset); }
        public GuidEx UnpackingGuidEx() { return Memory.UnpackingGuidEx(Buffer, ref mOffset); }
    }
    //*********************************************************************************************
}
