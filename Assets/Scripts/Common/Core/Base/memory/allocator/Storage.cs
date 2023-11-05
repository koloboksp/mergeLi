using System;

namespace Atom
{
    public abstract class Storage : IDisposable
    {
        protected int mOffset;
        public int Size { get; private set; }
        internal int Level;
        internal StorageBasePool Pool { get; }
        private int mRef;

        public int Offset => mOffset;

        //-----------------------------------------------------------------------------------------
        protected Storage(StorageBasePool pool, int size, int level)
        {
            Pool = pool;
            Level = level;
            Size = size;
        }
        //-----------------------------------------------------------------------------------------
        internal void Resize(int size)
        {
            lock (this)
            {
                mOffset = 0;
                mRef = 1;
                Size = size;
            }
        }
        //-----------------------------------------------------------------------------------------
        public Storage Clone()
        {
            lock (this)
            {
                if (mRef == 0)
                    return null;
                mRef++;
                return this;
            }
        }
        //-----------------------------------------------------------------------------------------
        public void Dispose()
        {
            lock (this)
            {
                if (mRef == 0)
                    return;

                mRef--;
                if(mRef == 0)
                    Pool?.Destroy(this);
            }
        }
        //-----------------------------------------------------------------------------------------
        internal static int GetLevel(int size)
        {
            var pow = 0;
            while (size > 0)
            {
                size >>= 1;
                pow++;
            }

            pow -= 7;

            if (pow < 0)
                pow = 0;

            return pow;
        }
        //-----------------------------------------------------------------------------------------
        internal static int GetSize(int level)
        {
            level += 7;
            var size = 1;
            while (level > 0)
            {
                size *= 2;
                level--;
            }
            return size;
        }
    }
    //*********************************************************************************************
}
