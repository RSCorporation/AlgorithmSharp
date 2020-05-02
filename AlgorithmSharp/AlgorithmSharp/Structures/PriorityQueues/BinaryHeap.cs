using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace AlgorithmSharp.Structures.PriorityQueues
{
    /// <summary>
    /// Implements <see cref="IPriorityQueue{TKey, TValue}"/> in the simpliest way. The best performer for small sizes and worst as the size grows.
    /// </summary>
    /// <typeparam name="TKey">The key (priority) of the <see cref="BinaryHeap{TKey, TValue}"/></typeparam>
    /// <typeparam name="TValue">The value of <see cref="BinaryHeap{TKey, TValue}"/></typeparam>
    public class BinaryHeap<TKey, TValue> : IPriorityQueue<TKey, TValue> where TKey : IComparable<TKey>
    {
        private KeyValuePair<TKey, TValue>[] heap;
        public int Count { get; private set; }

        private void SiftDown(int i)
        {
            while (2 * i + 1 < Count)
            {
                var left = (i << 1) + 1;
                var right = (i << 1) + 2;
                var j = left;
                if (right < Count && heap[right].Key.CompareTo(heap[left].Key) < 0)
                    j = right;
                if (heap[i].Key.CompareTo(heap[j].Key) <= 0)
                    break;
                (heap[i], heap[j]) = (heap[j], heap[i]);
                i = j;
            }
        }
        private void SiftUp(int i)
        {
            while(heap[i].Key.CompareTo(heap[(i - 1) >> 1].Key) < 0)
            {
                (heap[i], heap[(i - 1) >> 1]) = (heap[(i - 1) >> 1], heap[i]);
                i = (i - 1) >> 1;
            }
        }

        public bool IsSynchronized => false;

        public object SyncRoot { get; } = new object();

        public bool IsReadOnly => false;

        public void Add(KeyValuePair<TKey, TValue> item) => Insert(item.Key, item.Value);

        public void Clear() => Count = 0;

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

        public TValue Peek()
        {
            if (Count == 0)
                throw new InvalidOperationException("Binary heap is empty");
            return heap[0].Value;
        }

        public TValue Peek(out TKey priority)
        {
            if (Count == 0)
                throw new InvalidOperationException("Binary heap is empty");
            priority = heap[0].Key;
            return heap[0].Value;
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
