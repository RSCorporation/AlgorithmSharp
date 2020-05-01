using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace AlgorithmSharp.Structures
{
    public class PersistentQueue<T> : IEnumerable<T>, IReadOnlyCollection<T>, ICollection, IEnumerable
    {
        private PersistentStack<T> Ln;
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
