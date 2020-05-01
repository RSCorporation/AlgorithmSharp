using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace AlgorithmSharp.Structures
{
    public class PersistentQueue<T> : IEnumerable<T>, IReadOnlyCollection<T>, ICollection, IEnumerable
    {
        private PersistentStack<T> L;
        private PersistentStack<T> Lc;
        private PersistentStack<T> R;
        private PersistentStack<T> Rc;
        private PersistentStack<T> S;
        private bool reCopy;
        private int toCopy;
        private bool copied;

        public int Count { get; private set; }

        public bool IsSynchronized => true;

        public object SyncRoot => throw new NotSupportedException($"This collection is thread-safe and do not require {nameof(SyncRoot)}");

        /// <summary>
        /// Determines whether an element is in the <see cref="PersistentQueue{T}"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="PersistentQueue{T}"/>. The value can be <c>null</c> for reference types.</param>
        /// <returns><c>true</c> if item is found in the <see cref="PersistentQueue{T}"/>; otherwise, <c>false</c>.</returns>
        public bool Contains(T item)
        {
            var comp = EqualityComparer<T>.Default;
            foreach (var curr in this)
                if (comp.Equals(curr, item))
                    return true;
            return false;
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
