using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using AlgorithmSharp.Utils;

namespace AlgorithmSharp.Structures
{
    public class Treap<TKey, TValue> : IDictionary<TKey, TValue>
        where TKey : IComparable<TKey>
    {
        private TreapNode<TKey, TValue> root;

        public Treap() => root = null;

        public Treap(TKey element, TValue value) => root = new TreapNode<TKey, TValue>(element, value);

        public Treap(IEnumerable<KeyValuePair<TKey, TValue>> elements) : this(elements.ToList())
        {
        }

        public Treap(IList<KeyValuePair<TKey, TValue>> elements) =>
            root = BuildFromlist(elements, 0, elements.Count - 1);

        public IEnumerable<TKey> Keys => new Enumerable<TKey>(GetKeysEnumerator());

        public IEnumerable<TValue> Values => new Enumerable<TValue>(GetValuesEnumerator());
        public int Count => root.Count;

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            var nodes = GetNodesEnumerator();
            while (nodes.MoveNext())
                // ReSharper disable once PossibleNullReferenceException
                yield return new KeyValuePair<TKey, TValue>(nodes.Current.Key, nodes.Current.Value);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) =>
            Insert(item.Key, item.Value);

        void ICollection<KeyValuePair<TKey, TValue>>.Clear() => root = null;

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            var node = Find(root, item.Key);
            return node != null && EqualityComparer<TValue>.Default.Equals(node.Value, item.Value);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            foreach (var entry in this)
                array[arrayIndex++] = entry;
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            if (!((ICollection<KeyValuePair<TKey, TValue>>) this).Contains(item)) return false;
            Erase(item.Key);
            return true;
        }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys.ToList();

        ICollection<TValue> IDictionary<TKey, TValue>.Values => Values.ToList();

        TValue IDictionary<TKey, TValue>.this[TKey key]
        {
            get => Find(root, key).Value;
            set => Find(root, key).Value = value;
        }

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value) => Insert(key, value);

        bool IDictionary<TKey, TValue>.ContainsKey(TKey key) => Find(root, key) != null;

        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            var ret = Find(root, key) != null;
            if (ret)
                Erase(key);
            return ret;
        }

        bool IDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)
        {
            var node = Find(root, key);
            if (node == null)
            {
                value = default(TValue);
                return false;
            }

            value = node.Value;
            return true;
        }

        private static TreapNode<TKey, TValue> BuildFromlist(IList<KeyValuePair<TKey, TValue>> elements, int left,
            int right)
        {
            if (left == right)
                return new TreapNode<TKey, TValue>(elements[left].Key, elements[left].Value);
            var mid = (left + right) / 2;
            var node = new TreapNode<TKey, TValue>(elements[mid].Key, elements[mid].Value)
            {
                Left = BuildFromlist(elements, left, mid - 1),
                Right = BuildFromlist(elements, mid + 1, right)
            };
            SiftDown(node);
            return node;
        }

        private IEnumerator<TreapNode<TKey, TValue>> GetNodesEnumerator()
        {
            var nodes = new Stack<TreapNode<TKey, TValue>>();
            var curr = root;
            while (true)
            {
                if (curr == null)
                    if (nodes.Any())
                    {
                        yield return nodes.Peek();
                        curr = nodes.Pop().Right;
                    }
                    else
                    {
                        break;
                    }

                nodes.Push(curr);
                curr = curr.Left;
            }
        }

        private IEnumerator<TKey> GetKeysEnumerator()
        {
            var nodes = GetNodesEnumerator();
            while (nodes.MoveNext())
                // ReSharper disable once PossibleNullReferenceException
                yield return nodes.Current.Key;
        }

        private IEnumerator<TValue> GetValuesEnumerator()
        {
            var nodes = GetNodesEnumerator();
            while (nodes.MoveNext())
                // ReSharper disable once PossibleNullReferenceException
                yield return nodes.Current.Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Treap<TKey, TValue> Merge(Treap<TKey, TValue> leftTreap, Treap<TKey, TValue> rightTreap) =>
            new Treap<TKey, TValue> {root = Merge(leftTreap.root, rightTreap.root)};

        private static TreapNode<TKey, TValue> Merge(TreapNode<TKey, TValue> left, TreapNode<TKey, TValue> right)
        {
            if (left == null) return right;
            if (right == null) return left;
            if (left.Priority > right.Priority)
            {
                left.Right = Merge(left.Right, right);
                left.UpadteCount();
                return left;
            }

            right.Left = Merge(left, right.Left);
            right.UpadteCount();
            return right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (Treap<TKey, TValue>, Treap<TKey, TValue>) Split(TKey element)
        {
            var result = Split(root, element);
            return (new Treap<TKey, TValue> {root = result.Item1}, new Treap<TKey, TValue> {root = result.Item2});
        }

        private static (TreapNode<TKey, TValue>, TreapNode<TKey, TValue>) Split(TreapNode<TKey, TValue> treapNode,
            TKey element)
        {
            if (treapNode == null)
                return (null, null);
            if (treapNode.Key.CompareTo(element) < 0)
            {
                var rightTree = treapNode.Right;
                treapNode.Right = null;
                var rightSplit = Split(rightTree, element);
                treapNode.Right = rightSplit.Item1;
                treapNode.UpadteCount();
                return (treapNode, rightSplit.Item2);
            }

            var leftTree = treapNode.Left;
            treapNode.Left = null;
            var leftSplit = Split(leftTree, element);
            treapNode.Left = leftSplit.Item2;
            treapNode.UpadteCount();
            return (leftSplit.Item1, treapNode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (Treap<TKey, TValue>, Treap<TKey, TValue>) SplitWithEqual(TKey element)
        {
            var result = SplitWithEqual(root, element);
            return (new Treap<TKey, TValue> {root = result.Item1}, new Treap<TKey, TValue> {root = result.Item2});
        }

        private static (TreapNode<TKey, TValue>, TreapNode<TKey, TValue>) SplitWithEqual(
            TreapNode<TKey, TValue> treapNode, TKey element)
        {
            if (treapNode == null)
                return (null, null);
            if (treapNode.Key.CompareTo(element) <= 0)
            {
                var rightTree = treapNode.Right;
                treapNode.Right = null;
                var rightSplit = Split(rightTree, element);
                treapNode.Right = rightSplit.Item1;
                treapNode.UpadteCount();
                return (treapNode, rightSplit.Item2);
            }

            var leftTree = treapNode.Left;
            treapNode.Left = null;
            var leftSplit = Split(leftTree, element);
            treapNode.Left = leftSplit.Item2;
            treapNode.UpadteCount();
            return (leftSplit.Item1, treapNode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Insert(TKey element, TValue value) =>
            root = Insert(root, new TreapNode<TKey, TValue>(element, value));

        private static TreapNode<TKey, TValue> Insert(TreapNode<TKey, TValue> treapNode,
            TreapNode<TKey, TValue> inserting)
        {
            if (treapNode == null) return inserting;
            if (inserting.Priority < treapNode.Priority)
            {
                if (treapNode.Key.CompareTo(inserting.Key) == 0)
                    throw new ArgumentException("The key already exists in treap", nameof(inserting));
                if (treapNode.Key.CompareTo(inserting.Key) < 0)
                    treapNode.Right = Insert(treapNode.Right, inserting);
                else
                    treapNode.Left = Insert(treapNode.Left, inserting);
                treapNode.UpadteCount();
                return treapNode;
            }

            var splitResult = Split(treapNode, inserting.Key);
            inserting.Left = splitResult.Item1;
            inserting.Right = splitResult.Item2;
            inserting.UpadteCount();
            return inserting;
        }

        private static TreapNode<TKey, TValue> Find(TreapNode<TKey, TValue> treapNode, TKey key)
        {
            while (true)
            {
                if (treapNode == null) return null;
                if (treapNode.Key.CompareTo(key) == 0) return treapNode;
                treapNode = treapNode.Key.CompareTo(key) < 0 ? treapNode.Right : treapNode.Left;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Erase(TKey element) =>
            root = Erase(root, element);

        private static TreapNode<TKey, TValue> Erase(TreapNode<TKey, TValue> treapNode, TKey element)
        {
            if (treapNode == null) return null;
            if (treapNode.Key.CompareTo(element) == 0)
                return Merge(treapNode.Left, treapNode.Right);
            if (treapNode.Key.CompareTo(element) < 0)
                treapNode.Right = Erase(treapNode.Right, element);
            else
                treapNode.Left = Erase(treapNode.Left, element);
            treapNode.UpadteCount();
            return treapNode;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Treap<TKey, TValue> Union(Treap<TKey, TValue> treap1, Treap<TKey, TValue> treap2) =>
            new Treap<TKey, TValue> {root = Union(treap1.root, treap2.root)};

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Treap<TKey, TValue> Union(IEnumerable<Treap<TKey, TValue>> treaps) =>
            new Treap<TKey, TValue> {root = Union(treaps.Select(x => x.root))};

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Treap<TKey, TValue> Union(params Treap<TKey, TValue>[] treaps) => Union(treaps.AsEnumerable());

        private static TreapNode<TKey, TValue> Union(TreapNode<TKey, TValue> left, TreapNode<TKey, TValue> right)
        {
            if (left == null) return right;
            if (right == null) return left;
            if (left.Priority > right.Priority)
            {
                var splitResult = Split(right, left.Key);
                left.Left = Merge(left.Left, splitResult.Item1);
                left.Right = Merge(left.Right, splitResult.Item2);
                left.UpadteCount();
                return left;
            }
            else
            {
                var splitResult = Split(left, right.Key);
                right.Left = Merge(right.Left, splitResult.Item1);
                right.Right = Merge(right.Right, splitResult.Item2);
                right.UpadteCount();
                return right;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static TreapNode<TKey, TValue> Union(IEnumerable<TreapNode<TKey, TValue>> treapNodes) =>
            treapNodes.Aggregate(Union);

        private static TreapNode<TKey, TValue> SiftDown(TreapNode<TKey, TValue> treapNode)
        {
            while (true)
            {
                if (treapNode == null) return null;
                var maxPriority = treapNode;
                if (treapNode.Left != null && treapNode.Left.Priority > maxPriority.Priority)
                    maxPriority = treapNode.Left;
                if (treapNode.Right != null && treapNode.Right.Priority > maxPriority.Priority)
                    maxPriority = treapNode.Right;
                if (maxPriority != treapNode)
                {
                    var p = treapNode.Priority;
                    treapNode.Priority = maxPriority.Priority;
                    maxPriority.Priority = p;
                    treapNode = maxPriority;
                    continue;
                }

                break;
            }

            return treapNode;
        }

        private class TreapNode<TNodeKey, TNodeValue>
        {
            // ReSharper disable once StaticMemberInGenericType
            private static readonly Random rng = new Random();
            public readonly TNodeKey Key;
            public int Count;
            public TreapNode<TNodeKey, TNodeValue> Left, Right;
            public int Priority;

            public TreapNode(TNodeKey key, TNodeValue value)
            {
                Key = key;
                Value = value;
                Count = 1;
                Priority = rng.Next();
            }

            public TNodeValue Value { get; set; }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void UpadteCount()
            {
                Count = Left?.Count ?? 0 + Right?.Count ?? 0;
            }
        }
    }
}