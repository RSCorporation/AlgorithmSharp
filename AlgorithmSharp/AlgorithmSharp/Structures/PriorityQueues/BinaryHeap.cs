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

        private void IncreaseCapacity()
        {
            if (heap.Length == Count)
            {
                var newHeap = new KeyValuePair<TKey, TValue>[Count << 1];
                Array.Copy(heap, 0, newHeap, 0, Count);
                heap = newHeap;
            }
        }

        public bool IsSynchronized => false;

        public object SyncRoot { get; } = new object();

        public bool IsReadOnly => false;

        public void Add(KeyValuePair<TKey, TValue> item) => Insert(item.Key, item.Value);

        public void Clear() => Count = 0;

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            var comp = EqualityComparer<KeyValuePair<TKey, TValue>>.Default;
            foreach (var curr in this)
                if (comp.Equals(curr, item))
                    return true;
            return false;

        }

        public void CopyTo(Array array, int index)
        {
            foreach (var i in this)
                array.SetValue(i, index++);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            foreach (var i in this)
                array[arrayIndex++] = i;
        }

        public TValue Extract() => Extract(out _);

        public TValue Extract(out TKey priority)
        {
            if (Count == 0)
                throw new InvalidOperationException("Binary heap is empty");
            var result = heap[0];
            heap[0] = heap[Count - 1];
            Count--;
            SiftDown(0);
            priority = result.Key;
            return result.Value;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void Insert(TKey key, TValue value)
        {
            IncreaseCapacity();
            heap[Count] = new KeyValuePair<TKey, TValue>(key, value);
            Count++;
            SiftUp(Count - 1);
        }

        public TValue Peek() => Peek(out _);

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
