using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace AlgorithmSharp.Structures.PriorityQueues
{
    /// <summary>
    /// Represents a priority queue abstract data type
    /// </summary>
    /// <typeparam name="TKey">The key (priority) of the <see cref="IPriorityQueue{TKey, TValue}"/></typeparam>
    /// <typeparam name="TValue">The value of <see cref="IPriorityQueue{TKey, TValue}"/></typeparam>
    public interface IPriorityQueue<TKey, TValue> : ICollection, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable, IEnumerable<KeyValuePair<TKey, TValue>> where TKey : IComparable<TKey>
    {
        /// <summary>
        /// Inserts a new pair into the priority queue.
        /// </summary>
        /// <param name="key">The priority of the pair</param>
        /// <param name="value">The value of the pair. This parameter can be <c>null</c> for reference types</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <c>null</c></exception>
        void Insert(TKey key, TValue value);

        /// <summary>
        /// Gets the value from the pair with minimal priority inside the <see cref="IPriorityQueue{TKey, TValue}"/>.
        /// </summary>
        /// <returns>The value from the minimal priority pair</returns>
        /// <exception cref="InvalidOperationException"><see cref="IPriorityQueue{TKey, TValue}"/> is empty.</exception>
        TValue Peek();

        /// <summary>
        /// Gets the value from the pair with minimal priority inside the <see cref="IPriorityQueue{TKey, TValue}"/>. 
        /// </summary>
        /// <param name="priority">The minimal priority inside the <see cref="IPriorityQueue{TKey, TValue}"/></param>
        /// <returns>The value from the minimal priority pair</returns>
        /// <exception cref="InvalidOperationException"><see cref="IPriorityQueue{TKey, TValue}"/> is empty.</exception>
        TValue Peek(out TKey priority);

        /// <summary>
        /// Gets the value from the pair with minimal priority inside the <see cref="IPriorityQueue{TKey, TValue}"/> and removes this pair.
        /// </summary>
        /// <returns>The value from the minimal priority pair</returns>
        /// <exception cref="InvalidOperationException"><see cref="IPriorityQueue{TKey, TValue}"/> is empty.</exception>
        TValue Extract();

        /// <summary>
        /// Gets the value from the pair with minimal priority inside the <see cref="IPriorityQueue{TKey, TValue}"/> and removes this pair.
        /// </summary>
        /// <param name="priority">The minimal priority inside the <see cref="IPriorityQueue{TKey, TValue}"/></param>
        /// <returns>The value from the minimal priority pair</returns>
        /// <exception cref="InvalidOperationException"><see cref="IPriorityQueue{TKey, TValue}"/> is empty.</exception>
        TValue Extract(out TKey priority);
    }
}
