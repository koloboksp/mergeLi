using System;
using System.Collections.Generic;
using System.Reflection;

namespace Atom
{
    //*********************************************************************************************
    public abstract class StorageBasePool
    {
        private readonly List<Stack<Storage>> mList = new();

        protected StorageBasePool()
        {
            lock (mList)
            {
                for (var i = 0; i != 32; i++)
                    mList.Add(new Stack<Storage>());
            }
        }
        //-----------------------------------------------------------------------------------------
        protected abstract Storage GetStorage(int level);
        //-----------------------------------------------------------------------------------------
        protected Storage Create(int size)
        {
            var level = Storage.GetLevel(size);

            lock (mList)
            {
                if (mList[level].Count == 0)
                    mList[level].Push(GetStorage(level));

                var mb = mList[level].Pop();
                mb.Resize(size);

                return mb;
            }
        }
        //-----------------------------------------------------------------------------------------
        internal void Destroy(Storage storage)
        {
            if (storage == null)
                return;

            lock (mList)
            {
                mList[storage.Level].Push(storage);
            }
        }
        //-----------------------------------------------------------------------------------------
    }
    //*********************************************************************************************
    public class StoragePool<T> : StorageBasePool where T : Storage
    {
        protected override Storage GetStorage(int level) 
            => (Storage)Activator.CreateInstance(typeof(T), BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
                , null, new object[]{this, level}, null, null);

        public T CreateStorage(int size = 127)
        {
            return (T)Create(size);    
        }
    }
    //*********************************************************************************************
}
