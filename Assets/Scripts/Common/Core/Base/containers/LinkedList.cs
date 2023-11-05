using System;
using System.Collections;
using System.Collections.Generic;
//*************************************************************************************************
namespace Atom.Containers
{
    public class LnkList<T> : IDisposable, IEnumerable<T>
    {
        public LnkList(Allocator<LnkListNode<T>> allocator)
        {
            mAllocator = allocator;
            Count = 0;
            mHead = null;
            mTail = null;
        }

        public static LnkList<T> Empty => new LnkList<T>(null);

        public LnkListNode<T> AddLast(T data)
        {
            var node = mAllocator.Create();
            node.Value = data;

            if (mTail == null)
            {
                mHead = node;
                mTail = node;
            }
            else
            {
                mTail.Next = node;
                node.Prev = mTail;
                mTail = node;
            }

            Count++;
            return node;
        }

        /*
        public LnkListNode<T> GetNode()
        {
            var node = mAllocator.GetNode();

            if (node == null)
                return null;

            if (mTail == null)
            {
                mHead = node;
                mTail = node;
            }
            else
            {
                mTail.Next = node;
                node.Prev = mTail;
                mTail = node;
            }

            Count++;
            return node;
        }
        */


        public void Remove(ref LnkListNode<T> node)
        {
            if (node == null)
                return;

            if (mHead == node && mTail == node)
            {
                mHead = null;
                mTail = null;
            }
            else if (mHead == node)
            {
                mHead = node.Next;
                mHead.Prev = null;
            }
            else if (mTail == node)
            {
                mTail = node.Prev;
                mTail.Next = null;
            }
            else
            {
                node.Prev.Next = node.Next;
                node.Next.Prev = node.Prev;
            }

            mAllocator.Destroy(node);
            node = null;
            Count--;
        }

        public LnkListNode<T> Insert(LnkListNode<T> prev, T data)
        {
            var node = mAllocator.Create();
            node.Value = data;

            node.Next = prev.Next;
            prev.Next = node;

            node.Prev = prev;

            if (mTail == prev)
                mTail = node;

            Count++;
            return node;
        }

        public override string ToString()
        {
            return Conversion.ToString(Count);
        }

        public LnkListNode<T> GetHead()
        {
            return mHead;
        }

        public LnkListNode<T> GetTail()
        {
            return mTail;
        }

        public void Dispose()
        {
            Clear();
            mAllocator = null;
        }

        public void Clear()
        {
            var node = GetHead();

            while (node != null)
            {
                var current = node;
                node = node.Next;
                mAllocator.Destroy(current);
            }

            mHead = null;
            mTail = null;
            Count = 0;
        }

        public delegate bool DelegateFind(LnkListNode<T> node);

        public bool Find(DelegateFind find)
        {
            var node = GetHead();

            while (node != null)
            {
                if (find(node))
                    return true;

                node = node.Next;
            }

            return false;
        }

        public int GetNodeNumber(LnkListNode<T> findNode)
        {
            var i = 0;

            var node = GetHead();

            while (node != null)
            {
                if (node == findNode)
                    return i;
                node = node.Next;
                i++;
            }
            return -1;
        }

        /*
        public delegate bool DelegateSort(LnkListNode<T> nodeLeft, LnkListNode<T> nodeRight);

        private static void Swap(LnkListNode<T> nodeLeft, LnkListNode<T> nodeRight)
        {
            var tmp = nodeLeft.Value;
            nodeLeft.Value = nodeRight.Value;
            nodeRight.Value = tmp;
        }

        public void Sort(DelegateSort sort)
        {
            if (Count == 0)
                return;

            var repeat = true;
            while (repeat)
            {
                repeat = false;
                for (var node = GetHead(); node != null; node = node.Next)
                    if (node.Next != null)
                        if (!sort(node, node.Next))
                        {
                            repeat = true;
                            Swap(node, node.Next);
                        }
            }
        }
        */

        public bool Contains(T data)
        {
            var node = GetHead();

            while (node != null)
            {
                if (Equals(node.Value, data))
                    return true;

                node = node.Next;
            }

            return false;
        }

        public bool Remove(T data)
        {
            var node = GetHead();

            while (node != null)
            {
                if (Equals(node.Value, data))
                {
                    Remove(ref node);
                    return true;
                }

                node = node.Next;
            }
            return false;
        }

        public int Count { get; private set; }

        public bool IsEmpty => Count == 0;

        public IEnumerator<T> GetEnumerator()
        {
            var currentNode = mHead;
            while (currentNode != null)
            {
                yield return currentNode.Value;
                currentNode = currentNode.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private LnkListNode<T> mHead;
        private LnkListNode<T> mTail;
        private Allocator<LnkListNode<T>> mAllocator;
    }
}
//*************************************************************************************************