// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;

namespace AlgorithmSharp.Structures.PriorityQueues
{
    public class BinomialHeap<TKey, TValue> : IPriorityQueue<TKey, TValue> where TKey : IComparable<TKey>
    {
        public class Node
        {
            public TKey Key { get; private set; }
            public TValue Value;
            internal Node parent;
            internal Node sibling;
            internal Node child;
            internal int degree;
            internal IEnumerable<Node> Enumerate()
            {
                yield return this;
                if (child != null)
                    foreach (var node in child.Enumerate())
                        yield return node;
                if (sibling != null)
                    foreach (var node in sibling.Enumerate())
                        yield return node;
            }
        }

        private Node head;

        public int Count { get; private set; }

        public bool IsSynchronized => false;

        public object SyncRoot { get; } = new object();

        public bool IsReadOnly => false;

        public void Add(KeyValuePair<TKey, TValue> item) => Insert(item.Key, item.Value);

        public void Clear()
        {
            head = null;
            Count = 0;
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            foreach(var node in this)
            {
                if (node.Equals(item))
                    return true;
            }
            return false;
        }

        public void CopyTo(Array array, int index)
        {
            foreach(var node in this)
            {
                array.SetValue(node, index++);
            }
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            foreach (var node in this)
            {
                array[arrayIndex++] = node;
            }
        }

        public TValue Extract() => Extract(out _);

        public TValue Extract(out TKey priority)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var node in head.Enumerate())
                yield return new KeyValuePair<TKey, TValue>(node.Key, node.Value);
        }

        void IPriorityQueue<TKey, TValue>.Insert(TKey key, TValue value) => Insert(key, value);
        public Node Insert(TKey key, TValue value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a heap that contains elements from both <paramref name="h1"/> and <paramref name="h2"/>.
        /// After this operation both arguments are invalidated.
        /// </summary>
        /// <param name="h1">First heap</param>
        /// <param name="h2">Second heap</param>
        /// <returns>Heap that contains elements from both <paramref name="h1"/> and <paramref name="h2"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="h1"/> or <paramref name="h2"/> is <c>null</c></exception>
        public static BinomialHeap<TKey, TValue> Meld(BinomialHeap<TKey, TValue> h1, BinomialHeap<TKey, TValue> h2)
        {
            Contract.Requires<ArgumentNullException>(h1 != null);
            Contract.Requires<ArgumentNullException>(h2 != null);
            return new BinomialHeap<TKey, TValue>()
            {
                head = Meld(h1.head, h2.head),
                Count = h1.Count + h2.Count
            };
        }

        private static Node Meld(Node h1, Node h2)
        {
            if (h1 == null)
                return h2;
            if (h2 == null)
                return h1;
            var result = new Node();
            var curH = result;
            var curH1 = h1;
            var curH2 = h2;
            while (curH1 != null && curH2 != null)
            {
                if (curH1.degree < curH2.degree)
                {
                    if (curH != null)
                        curH.sibling = curH1;
                    else
                        result = curH1;
                    curH = curH1;
                    curH1 = curH1.sibling;
                }
                else
                {
                    if (curH != null)
                        curH.sibling = curH2;
                    else
                        result = curH2;
                    curH = curH2;
                    curH2 = curH2.sibling;
                }
            }
            if (curH1 == null)
            {
                if (curH != null)
                    curH.sibling = curH2;
                else
                    result = curH2;
            }
            else
            {
                if (curH != null)
                    curH.sibling = curH1;
                else
                    result = curH1;
            }
            curH = result;
            while (curH.sibling != null)
            {
                if (curH.degree == curH.sibling.degree)
                {
                    curH.parent = curH.sibling;
                    var tmp = curH.sibling;
                    curH.sibling = curH.sibling.child;
                    tmp.child = curH;
                    curH = tmp;
                    continue;
                }
                curH = curH.sibling;
            }
            return result;
        }

        public TValue Peek()
        {
            throw new NotImplementedException();
        }

        public TValue Peek(out TKey priority)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
