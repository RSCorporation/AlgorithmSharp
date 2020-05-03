using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            while (heap[i].Key.CompareTo(heap[(i - 1) >> 1].Key) < 0)
            {
                (heap[i], heap[(i - 1) >> 1]) = (heap[(i - 1) >> 1], heap[i]);
                i = (i - 1) >> 1;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryHeap{TKey, TValue}"/> class that is empty and has the default initial capacity.
        /// </summary>
        public BinaryHeap() : this(4) { }

        /// <summary>
        /// Initializes new <see cref="BinaryHeap{TKey, TValue}"/> that contains elements copied from the specified collection.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new heap.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <c>null</c></exception>
        public BinaryHeap(IEnumerable<KeyValuePair<TKey, TValue>> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            heap = collection.ToArray();
            Count = heap.Length;
            for (var i = Count << 1; i >= 0; i--)
                SiftDown(i);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryHeap{TKey, TValue}"/> class that is empty and has the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The number of elements that the new list can initially store.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than 0</exception>
        public BinaryHeap(int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity can't be negative");
            heap = new KeyValuePair<TKey, TValue>[capacity];
            Count = 0;
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

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            IncreaseCapacity();
            heap[Count] = item;
            Count++;
            SiftUp(Count - 1);
        }

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

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => heap.Take(Count).GetEnumerator();

        public void Insert(TKey key, TValue value) => Add(new KeyValuePair<TKey, TValue>(key, value));

        public TValue Peek() => Peek(out _);

        public TValue Peek(out TKey priority)
        {
            if (Count == 0)
                throw new InvalidOperationException("Binary heap is empty");
            priority = heap[0].Key;
            return heap[0].Value;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item) => throw new NotSupportedException();

        /// <summary>
        /// Copies the elements of the <see cref="BinaryHeap{TKey, TValue}"/>> to a new array.
        /// </summary>
        /// <returns>An array containing copies of the elements of the <see cref="BinaryHeap{TKey, TValue}"/>.</returns>
        public KeyValuePair<TKey, TValue>[] ToArray()
        {
            var result = new KeyValuePair<TKey, TValue>[Count];
            CopyTo(result, 0);
            return result;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
