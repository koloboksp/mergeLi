using System;
using System.Collections.Generic;

namespace Core.Utils
{
    public class ReuseList<T> where T : class
    {
        readonly Func<T> _getInstance;
        readonly List<int> _free = new List<int>();
        readonly List<int> _used = new List<int>();
        readonly List<T> _elements = new List<T>();

        public int Count => _used.Count;

        public ReuseList(Func<T> getInstance)
        {
            _getInstance = getInstance;
        }

        public T Get(out int index)
        {
            if (_free.Count == 0)
            {
                index = _used.Count;
                _used.Add(_used.Count);

                var newElement = _getInstance();
                _elements.Add(newElement);
                return newElement;
            }

            var freeElementIndex = _free[_free.Count - 1];
            _free.RemoveAt(_free.Count - 1);
            index = freeElementIndex;
            _used.Add(freeElementIndex);
            return _elements[freeElementIndex];
        }

        public void Return(int index)
        {
            var indexOfIndex = _used.IndexOf(index);
            _free.Add(_used[indexOfIndex]);
            _used.RemoveAt(indexOfIndex);
        }

        public T this[int index]
        {
            get { return _elements[_used[index]]; }
        }

        public void Clear()
        {
            for (int i = 0; i < _used.Count; i++)
                _free.Add(_used[i]);

            _used.Clear();
        }
    }
}