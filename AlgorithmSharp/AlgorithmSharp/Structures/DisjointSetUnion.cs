using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AlgorithmSharp.Structures
{
    /// <summary>
    ///     Represents a disjoint set union data structure.
    /// </summary>
    /// <typeparam name="T">Type of elements in sets</typeparam>
    public class DisjointSetUnion<T>
    {
        private readonly Dictionary<T, DsuNode> nodes;

        /// <summary>
        ///     Creates new disjoint set union with no sets
        /// </summary>
        public DisjointSetUnion()
        {
            nodes = new Dictionary<T, DsuNode>();
            NumberOfSets = 0;
        }

        /// <summary>
        ///     Creates disjoint set union with elements being a 1-item set
        /// </summary>
        /// <param name="elements">Sets</param>
        public DisjointSetUnion(IEnumerable<T> elements)
        {
            nodes = elements.ToDictionary(x => x, x => new DsuNode());
            NumberOfSets = nodes.Count;
        }

        public int NumberOfSets { get; private set; }

        /// <summary>
        ///     Adds new 1-item set to disjoint set union
        /// </summary>
        /// <param name="element"></param>
        public void AddElement(T element)
        {
            nodes.Add(element, new DsuNode());
            NumberOfSets++;
        }

        /// <summary>
        ///     Merges sets, containing element 1 and element 2
        /// </summary>
        /// <param name="element1">Element from set 1</param>
        /// <param name="element2">Element from set 2</param>
        public void MergeSets(T element1, T element2)
        {
            if (!CompareSets(element1, element2))
                NumberOfSets--;
            nodes[element2].GetRoot().Parent = nodes[element1].GetRoot();
        }

        /// <summary>
        ///     Checks if <paramref name="element1" /> and <paramref name="element2" /> are in the same set
        /// </summary>
        /// <param name="element1">Element from set 1</param>
        /// <param name="element2">Element from set 2</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CompareSets(T element1, T element2) => nodes[element1].GetRoot() == nodes[element2].GetRoot();

        /// <summary>
        ///     Merges to disjoint set unions (no sets will be merged)
        /// </summary>
        /// <param name="other">Disjoint set union to add</param>
        public void AddDisjointSetUnion(DisjointSetUnion<T> other)
        {
            foreach (var i in other.nodes)
                nodes.Add(i.Key, i.Value);
            NumberOfSets += other.NumberOfSets;
        }

        private class DsuNode
        {
            public DsuNode Parent;

            [DebuggerStepThrough]
            public DsuNode GetRoot()
            {
                return Parent == null || Parent == this ? this : Parent = Parent.GetRoot();
            }
        }
    }
}