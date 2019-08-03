using System;
using System.Collections;
using System.Collections.Generic;

namespace AlgorithmSharp.Utils
{
    public class Enumerable<T> : IEnumerable<T>
    {
        private readonly IEnumerator<T> enumerator;

        public Enumerable(IEnumerator<T> enumerator) =>
            this.enumerator = enumerator ?? throw new ArgumentNullException(nameof(enumerator));

        public IEnumerator<T> GetEnumerator() => enumerator;

        IEnumerator IEnumerable.GetEnumerator() => enumerator;
    }
}