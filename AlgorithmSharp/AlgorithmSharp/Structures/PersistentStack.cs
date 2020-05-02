// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;
using System.Collections;
using System.Collections.Generic;

namespace AlgorithmSharp.Structures
{
    public class PersistentStack<T> : IEnumerable<T>, IReadOnlyCollection<T>, ICollection, IEnumerable
    {
        private T value;
        private PersistentStack<T> popTo;

        private PersistentStack()
        {
            value = default;
            popTo = null;
            Count = 0;
        }

        /// <summary>
        /// Version of the <see cref="PersistentStack{T}"/> that is empty. If you pop all the elements out of any stack the result will be equal to this field value;
        /// </summary>
        public static readonly PersistentStack<T> EmptyStack = new PersistentStack<T>();

        public int Count { get; private set; }

        public bool IsSynchronized => true;

        public object SyncRoot => throw new NotSupportedException($"This collection is thread-safe and do not require {nameof(SyncRoot)}");

        #region Factories
        /// <summary>
        /// Returns <see cref="EmptyStack"/>
        /// </summary>
        /// <returns><see cref="EmptyStack"/></returns>
        public static PersistentStack<T> Create() => EmptyStack;

        /// <summary>
        /// Creates a version of the <see cref="PersistentStack{T}"/> that contains elements copied from the specified collection.
        /// </summary>
        /// <param name="collection">The collection to copy elements from.</param>
        /// <returns>The version of <see cref="PersistentStack{T}"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <c>null</c></exception>
        public static PersistentStack<T> Create(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            var curr = Create();
            foreach (var item in collection)
                curr.Push(item, out curr);
            return curr;
        }
        #endregion

        /// <summary>
        /// Copies the <see cref="PersistentStack{T}"/> to an existing one-dimensional <see cref="Array"/>, starting at the specified array index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied from <see cref="PersistentStack{T}"/>. The <see cref="Array"/> must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        public void CopyTo(Array array, int index)
        {
            var curr = this;
            while (curr != EmptyStack)
            {
                array.SetValue(curr.value, index);
                index++;
                curr = curr.popTo;
            }
        }

        /// <summary>
        /// Returns an enumerator for the <see cref="PersistentStack{T}"/>.
        /// </summary>
        /// <returns>An enumerator for the <see cref="PersistentStack{T}"/></returns>
        public IEnumerator<T> GetEnumerator()
        {
            var curr = this;
            while (curr != EmptyStack)
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
        /// <param name="item">The object to locate in the <see cref="PersistentStack{T}"/>. The value can be <c>null</c> for reference types.</param>
        /// <returns><c>true</c> if <paramref name="item"/> is found in the <see cref="PersistentStack{T}"/>; otherwise, <c>false</c>.</returns>
        public bool Contains(T item)
        {
            var comp = EqualityComparer<T>.Default;
            var curr = this;
            while (curr != EmptyStack)
            {
                if ((curr.value == null && item == null) || comp.Equals(curr.value, item))
                    return true;
                curr = curr.popTo;
            }
            return false;
        }

        /// <summary>
        /// Returns the object at the top of the <see cref="PersistentStack{T}"/> without removing it.
        /// </summary>
        /// <returns>The object at the top of the <see cref="PersistentStack{T}"/>.</returns>
        /// <exception cref="InvalidOperationException">The <see cref="PersistentStack{T}"/> is empty.</exception>
        public T Peek() => this == EmptyStack ? throw new InvalidOperationException($"Stack is empty.") : value;

        /// <summary>
        /// Removes and returns the object at the top of the <see cref="PersistentStack{T}"/>.
        /// </summary>
        /// <param name="newVersion">The modified version of <see cref="PersistentStack{T}"/></param>
        /// <returns>The object removed from the top of the <see cref="PersistentStack{T}"/>.</returns>
        /// <exception cref="InvalidOperationException">The <see cref="PersistentStack{T}"/> is empty.</exception>
        public T Pop(out PersistentStack<T> newVersion)
        {
            if (this == EmptyStack)
                throw new InvalidOperationException($"Stack is empty.");
            newVersion = popTo;
            return value;
        }

        /// <summary>
        /// Inserts an object at the top of the <see cref="PersistentStack{T}"/>.
        /// </summary>
        /// <param name="item">The object to push onto the <see cref="PersistentStack{T}"/>. The value can be <c>null</c> for reference types.</param>
        /// <param name="newVersion">The modified version of <see cref="PersistentStack{T}"/></param>
        public void Push(T item, out PersistentStack<T> newVersion)
        {
            newVersion = new PersistentStack<T>()
            {
                Count = Count + 1,
                popTo = this,
                value = item
            };
        }

        /// <summary>
        /// Copies the <see cref="PersistentStack{T}"/> to a new array.
        /// </summary>
        /// <returns>A new array containing copies of the elements of the <see cref="PersistentStack{T}"/>.</returns>
        public T[] ToArray()
        {
            var result = new T[Count];
            CopyTo(result, 0);
            return result;
        }

        /// <summary>
        /// Returns a value that indicates whether there is an object at the top of the <see cref="PersistentStack{T}"/>, and if one is present, copies it to the <paramref name="result"/> parameter. The object is not removed from the <see cref="PersistentStack{T}"/>.
        /// </summary>
        /// <param name="result">If present, the object at the top of the <see cref="PersistentStack{T}"/>; otherwise, the default value of <typeparamref name="T"/>.</param>
        /// <returns><c>true</c> if there is an object at the top of the <see cref="PersistentStack{T}"/>; <c>false</c> if the <see cref="PersistentStack{T}"/> is empty.</returns>
        public bool TryPeek(out T result)
        {
            if (this == EmptyStack)
            {
                result = default;
                return false;
            }
            else
            {
                result = value;
                return true;
            }
        }

        /// <summary>
        /// Returns a value that indicates whether there is an object at the top of the <see cref="PersistentStack{T}"/>, and if one is present, copies it to the <paramref name="result"/> parameter, and removes it from the <see cref="PersistentStack{T}"/>.
        /// </summary>
        /// <param name="result">If present, the object at the top of the <see cref="PersistentStack{T}"/>; otherwise, the default value of <typeparamref name="T"/>.</param>
        /// <param name="newVersion">The modified version of <see cref="PersistentStack{T}"/></param>
        /// <returns><c>true</c> if there is an object at the top of the <see cref="PersistentStack{T}"/>; <c>false</c> if the <see cref="PersistentStack{T}"/> is empty.</returns>
        public bool TryPop(out T result, out PersistentStack<T> newVersion)
        {
            if (this == EmptyStack)
            {
                result = default;
                newVersion = this;
                return false;
            }
            else
            {
                result = value;
                newVersion = popTo;
                return true;
            }
        }
    }
}
