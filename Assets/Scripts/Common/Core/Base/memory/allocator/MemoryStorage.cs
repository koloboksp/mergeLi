namespace Atom
{
    //*********************************************************************************************
    public abstract class MemoryStorage : Storage
    {
        public readonly byte[] Buffer;

        protected MemoryStorage(StorageBasePool pool, int level) : base (pool, GetSize(level), level)
        {
            Buffer = new byte[Size];
        }

        protected MemoryStorage(int level) : base(null, GetSize(level), level)
        {
            Buffer = new byte[Size];
        }

        protected MemoryStorage(byte[] data) : base(null, data.Length, GetLevel(data.Length))
        {
            Buffer = data;
        }
    }
    //*********************************************************************************************
}
