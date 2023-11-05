using System.Collections;
using System.Collections.Generic;

namespace Atom.Containers
{
    public class LnkListNode<T> : IEnumerable<T>
    {
        public LnkListNode<T> Next { get; internal set; }
        public LnkListNode<T> Prev { get; internal set; }
        public T Value { get; internal set; }

        public override string ToString()
        {
            return Value.ToString();
        }

        public IEnumerator<T> GetEnumerator()
        {
            var currentNode = this;
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

        public static LnkListNode<T> operator ++(LnkListNode<T> node)
        {
            return node.Next;
        }
    }
}
