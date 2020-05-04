using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace AlgorithmSharp.Structures.PriorityQueues
{
    public class BinomialHeap<TKey, TValue> : IPriorityQueue<TKey, TValue> where TKey : IComparable<TKey>
    {
        public class Node
        {
            internal Node parent;
            internal Node sibling;
            internal Node child;
            internal int degree;
        }

        private Node head;

        public int Count { get; private set; }

        public bool IsSynchronized => false;

        public object SyncRoot { get; } = new object();

        public bool IsReadOnly => false;

        public void Add(KeyValuePair<TKey, TValue> item) => Insert(item.Key, item.Value);

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public TValue Extract()
        {
            throw new NotImplementedException();
        }

        public TValue Extract(out TKey priority)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void Insert(TKey key, TValue value)
        {
            throw new NotImplementedException();
        }

        public static BinomialHeap<TKey, TValue> Meld(BinomialHeap<TKey, TValue> h1, BinomialHeap<TKey, TValue> h2)
        {
            if (h1 == null)
                return h2;
            if (h2 == null)
                return h1;
            var result = new BinomialHeap<TKey, TValue>();
            var curH = result.head;
            var curH1 = h1.head;
            var curH2 = h2.head;
            while (curH1 != null && curH2 != null)
            {
                if (curH1.degree < curH2.degree)
                {
                    if (curH != null)
                        curH.sibling = curH1;
                    else
                        result.head = curH1;
                    curH = curH1;
                    curH1 = curH1.sibling;
                }
                else
                {
                    if (curH != null)
                        curH.sibling = curH2;
                    else
                        result.head = curH2;
                    curH = curH2;
                    curH2 = curH2.sibling;
                }
            }
            if (curH1 == null)
            {
                if (curH != null)
                    curH.sibling = curH2;
                else
                    result.head = curH2;
            }
            else
            {
                if (curH != null)
                    curH.sibling = curH1;
                else
                    result.head = curH1;
            }
            curH = result.head;
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

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
