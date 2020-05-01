// This is an open source non-commercial project.Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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

        public PersistentQueue(PersistentStack<T> l, PersistentStack<T> lc, PersistentStack<T> r, PersistentStack<T> rc, PersistentStack<T> s, bool reCopy, int toCopy, bool copied, int count)
        {
            L = l;
            Lc = lc;
            R = r;
            Rc = rc;
            S = s;
            this.reCopy = reCopy;
            this.toCopy = toCopy;
            this.copied = copied;
            Count = count;
        }
        public PersistentQueue() : this(PersistentStack<T>.Create(), PersistentStack<T>.Create(), PersistentStack<T>.Create(), PersistentStack<T>.Create(), PersistentStack<T>.Create(), false, 0, true, 0) { }

        private void CheckReCopy()
        {
            if (L.Count > R.Count)
            {
                reCopy = true;
                toCopy = R.Count;
                copied = false;
                CheckNormal();
            }
            else
                reCopy = false;
        }
        private void CheckNormal()
        {
            int todo = 3;
            PersistentStack<T> Rn = R, Sn = S;
            bool cc = copied;
            while(!cc && todo > 0 && Rn.Count > 0)
            {
                var x = Rn.Pop(out Rn);
                Sn.Push(x, out Sn);
                todo--;
            }
            PersistentStack<T> Ln = L;
            while(todo > 0 && Ln.Count > 0)
            {
                cc = true;
                var x = Ln.Pop(out Ln);
                Rn.Push(x, out Rn);
                todo--;
            }
            var c = toCopy;
            while (todo > 0 && Sn.Count > 0)
            {
                var x = Sn.Pop(out Sn);
                if (c > 0)
                {
                    Rn.Push(x, out Rn);
                    c--;
                }
                todo--;
            }
            var Lcn = Lc;
            if (S.Count == 0)
            {
                (Ln, Lcn) = (Lcn, Ln);
            }
            L = Ln;
            Lc = Lcn;
            R = Rn;
            S = Sn;
            reCopy = (S.Count != 0);
            toCopy = c;
            copied = cc;
        }

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

        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="PersistentQueue{T}"/>.
        /// </summary>
        /// <param name="newVersion">The modified version of <see cref="PersistentQueue{T}"/></param>
        /// <returns>The object that is removed from the beginning of the <see cref="PersistentQueue{T}"/>.</returns>
        /// <exception cref="InvalidOperationException">The <see cref="PersistentQueue{T}"/> is empty.</exception>
        public T Dequeue(out PersistentQueue<T> newVersion)
        {
            if (Count == 0)
                throw new InvalidOperationException("Queue is empty");
            if (!reCopy)
            {
                PersistentStack<T> rn;
                var x = R.Pop(out rn);
                newVersion = new PersistentQueue<T>(L, Lc, rn, Rc, S, reCopy, toCopy, copied, Count - 1);
                newVersion.CheckReCopy();
                return x;
            }
            else
            {
                PersistentStack<T> rcn;
                var x = Rc.Pop(out rcn);
                int c = toCopy;
                var rn = R;
                if (toCopy > 0)
                    c--;
                else
                    x = rn.Pop(out rn);
                newVersion = new PersistentQueue<T>(L, Lc, rn, rcn, S, reCopy, c, copied, Count - 1);
                newVersion.CheckNormal();
                return x;
            }
        }

        /// <summary>
        /// Adds an object to the end of the <see cref="PersistentQueue{T}"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="PersistentQueue{T}"/>. The value can be <c>null</c> for reference types.</param>
        /// <param name="newVersion">The modified version of <see cref="PersistentQueue{T}"/></param>
        public void Enqueue(T item, out PersistentQueue<T> newVersion)
        {
            if (!reCopy)
            {
                PersistentStack<T> ln;
                L.Push(item, out ln);
                newVersion = new PersistentQueue<T>(ln, Lc, R, Rc, S, reCopy, toCopy, copied, Count + 1);
                newVersion.CheckReCopy();
                return;
            }
            else
            {
                PersistentStack<T> lcn;
                Lc.Push(item, out lcn);
                newVersion = new PersistentQueue<T>(L, lcn, R, Rc, S, reCopy, toCopy, copied, Count + 1);
                newVersion.CheckNormal();
                return;
            }
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

        /// <summary>
        /// Returns the object at the beginning of the <see cref="PersistentQueue{T}"/> without removing it.
        /// </summary>
        /// <returns>The object at the beginning of the <see cref="PersistentQueue{T}"/>.</returns>
        /// <exception cref="InvalidOperationException">The <see cref="PersistentQueue{T}"/> is empty.</exception>
        public T Peek()
        {
            if (Count == 0)
                throw new InvalidOperationException("Queue is empty");
            if (!reCopy)
                return R.Peek();
            else if (toCopy > 0)
                return Rc.Peek();
            else
                return R.Peek();
        }
    }
}
