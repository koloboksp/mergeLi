using System;
using System.Collections.Generic;
//*********************************************************************************************
namespace Atom
{
    public class Allocator<T> : IDisposable
    {
        public delegate T OnCreate();
        public delegate void OnDestroy(T data);

        public Allocator(OnCreate onCreate, OnDestroy onDestroy)
        {
            mData = new Stack<T>();
            mOnCreate = onCreate;
            mOnDestroy = onDestroy;
        }

        public T Create()
        {
            Busy++;

            if (mData.Count == 0)
                return mOnCreate();

            Free--;
            return mData.Pop();
        }

        public void Destroy(T data)
        {
            if (data == null)
                return;

            mOnDestroy(data);
            mData.Push(data);

            Free++;
            Busy--;           
        }

        public void Destroy(IList<T> data)
        {
            for (var i = 0; i != data.Count; i++)
                Destroy(data[i]);

            data.Clear();
        }

        public void Dispose()
        {
            mData.Clear();
            Free = 0;
            Busy = 0;  
        }

        public int Free { get; private set; }
        public int Busy { get; private set; }

        private readonly Stack<T> mData;

        private readonly OnCreate mOnCreate;
        private readonly OnDestroy mOnDestroy;

        public override string ToString() => "Free: " + Conversion.ToString(Free) + " Busy: " + Conversion.ToString(Busy);
    }

    public class ThreadAllocator<T> : IDisposable
    {
        public delegate T OnCreate();
        public delegate void OnDestroy(T data);

        public ThreadAllocator(OnCreate onCreate, OnDestroy onDestroy)
        {
            mData = new Stack<T>();
            mOnCreate = onCreate;
            mOnDestroy = onDestroy;
        }

        public T Create()
        {
            lock (this)
            {
                Busy++;

                if (mData.Count == 0)
                    return mOnCreate();

                Free--;
                return mData.Pop();
            }
        }

        public void Destroy(T data)
        {
            lock (this)
            {
                if (data == null)
                    return;

                mOnDestroy(data);
                mData.Push(data);

                Free++;
                Busy--;
            }
        }

        public void Destroy(IList<T> data)
        {
            lock (this)
            {
                for (var i = 0; i != data.Count; i++)
                    Destroy(data[i]);

                data.Clear();
            }
        }

        public void Dispose()
        {
            lock (this)
            {
                mData.Clear();
                Free = 0;
                Busy = 0;
            }
        }

        public int Free { get; private set; }
        public int Busy { get; private set; }

        private readonly Stack<T> mData;

        private readonly OnCreate mOnCreate;
        private readonly OnDestroy mOnDestroy;

        public override string ToString() => "Free: " + Conversion.ToString(Free) + " Busy: " + Conversion.ToString(Busy);
    }
}
