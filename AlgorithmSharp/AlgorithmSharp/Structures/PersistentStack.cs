// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace AlgorithmSharp.Structures
{
    class PersistentStack<T> : IEnumerable<T>, IReadOnlyCollection<T>, ICollection, IEnumerable
    {
        private T value;
        private PersistentStack<T> popTo;
        public int Count { get; private set; }

        public bool IsSynchronized => true;

        public object SyncRoot => throw new NotSupportedException($"This collection is thread-safe and do not require {nameof(SyncRoot)}");

        public void CopyTo(Array array, int index)
        {
            var curr = this;
            while (curr != null)
            {
                array.SetValue(curr.value, index);
                index++;
                curr = curr.popTo;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            var curr = this;
            while (curr != null)
            {
                yield return curr.value;
                curr = curr.popTo;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Determines whether an element is in the <see cref="PersistentStack{T}"/>
        /// </summary>
        /// <param name="item">The object to locate in the see cref="PersistentStack{T}"/>. The value can be null for reference types.</param>
        /// <returns><c>true</c> if <paramref name="item"/> is found in the <see cref="PersistentStack{T}"/>; otherwise, <c>false</c>.</returns>
        public bool Contains(T item)
        {
            var comp = EqualityComparer<T>.Default;
            var curr = this;
            while (curr != null)
            {
                if ((curr == null && item == null) || comp.Equals(curr.value, item))
                    return true;
                curr = curr.popTo;
            }
            return false;
        }
    }
}
