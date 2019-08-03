using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using AlgorithmSharp.Utils;

namespace AlgorithmSharp.Structures
{
    public class Treap<TKey, TValue, TData> : IDictionary<TKey, TValue>
        where TKey : IComparable<TKey>
    {
        private readonly Func<TValue, TData> dataBuildingOperation;
        private readonly AssociativeOperation<TData> dataCombiningOperation;
        private TreapNode<TKey, TValue, TData> root;


        public Treap(AssociativeOperation<TData> dataCombiningOperation = null,
            Func<TValue, TData> dataBuildingOperation = null)
        {
            this.dataCombiningOperation = dataCombiningOperation;
            this.dataBuildingOperation = dataBuildingOperation;
            root = null;
        }

        public Treap(TKey element, TValue value, AssociativeOperation<TData> dataCombiningOperation = null,
            Func<TValue, TData> dataBuildingOperation = null)
        {
            this.dataCombiningOperation = dataCombiningOperation;
            this.dataBuildingOperation = dataBuildingOperation;
            root = new TreapNode<TKey, TValue, TData>(element, value, dataCombiningOperation, dataBuildingOperation);
        }

        public Treap(IEnumerable<KeyValuePair<TKey, TValue>> elements,
            AssociativeOperation<TData> dataCombiningOperation = null, Func<TValue, TData> dataBuildingOperation = null)
            : this(elements.ToList(), dataCombiningOperation, dataBuildingOperation)
        {
        }

        public Treap(IList<KeyValuePair<TKey, TValue>> elements,
            AssociativeOperation<TData> dataCombiningOperation = null, Func<TValue, TData> dataBuildingOperation = null)
        {
            this.dataCombiningOperation = dataCombiningOperation;
            this.dataBuildingOperation = dataBuildingOperation;
            root = BuildFromlist(elements, 0, elements.Count - 1);
        }

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

        private TreapNode<TKey, TValue, TData> BuildFromlist(IList<KeyValuePair<TKey, TValue>> elements, int left,
            int right)
        {
            if (left == right)
                return new TreapNode<TKey, TValue, TData>(elements[left].Key, elements[left].Value,
                    dataCombiningOperation, dataBuildingOperation);
            var mid = (left + right) / 2;
            var node = new TreapNode<TKey, TValue, TData>(elements[mid].Key, elements[mid].Value,
                dataCombiningOperation, dataBuildingOperation)
            {
                Left = BuildFromlist(elements, left, mid - 1),
                Right = BuildFromlist(elements, mid + 1, right)
            };
            SiftDown(node);
            return node;
        }

        private IEnumerator<TreapNode<TKey, TValue, TData>> GetNodesEnumerator()
        {
            var nodes = new Stack<TreapNode<TKey, TValue, TData>>();
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
        public static Treap<TKey, TValue, TData> Merge(Treap<TKey, TValue, TData> leftTreap,
            Treap<TKey, TValue, TData> rightTreap) =>
            new Treap<TKey, TValue, TData> {root = Merge(leftTreap.root, rightTreap.root)};

        private static TreapNode<TKey, TValue, TData> Merge(TreapNode<TKey, TValue, TData> left,
            TreapNode<TKey, TValue, TData> right)
        {
            if (left == null) return right;
            if (right == null) return left;
            if (left.Priority > right.Priority)
            {
                left.Right = Merge(left.Right, right);
                return left;
            }

            right.Left = Merge(left, right.Left);
            return right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (Treap<TKey, TValue, TData>, Treap<TKey, TValue, TData>) Split(TKey element)
        {
            var result = Split(root, element);
            return (new Treap<TKey, TValue, TData> {root = result.Item1},
                new Treap<TKey, TValue, TData> {root = result.Item2});
        }

        private static (TreapNode<TKey, TValue, TData>, TreapNode<TKey, TValue, TData>) Split(
            TreapNode<TKey, TValue, TData> treapNode,
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
                return (treapNode, rightSplit.Item2);
            }

            var leftTree = treapNode.Left;
            treapNode.Left = null;
            var leftSplit = Split(leftTree, element);
            treapNode.Left = leftSplit.Item2;
            return (leftSplit.Item1, treapNode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (Treap<TKey, TValue, TData>, Treap<TKey, TValue, TData>) SplitWithEqual(TKey element)
        {
            var result = SplitWithEqual(root, element);
            return (new Treap<TKey, TValue, TData> {root = result.Item1},
                new Treap<TKey, TValue, TData> {root = result.Item2});
        }

        private static (TreapNode<TKey, TValue, TData>, TreapNode<TKey, TValue, TData>) SplitWithEqual(
            TreapNode<TKey, TValue, TData> treapNode, TKey element)
        {
            if (treapNode == null)
                return (null, null);
            if (treapNode.Key.CompareTo(element) <= 0)
            {
                var rightTree = treapNode.Right;
                treapNode.Right = null;
                var rightSplit = Split(rightTree, element);
                treapNode.Right = rightSplit.Item1;
                return (treapNode, rightSplit.Item2);
            }

            var leftTree = treapNode.Left;
            treapNode.Left = null;
            var leftSplit = Split(leftTree, element);
            treapNode.Left = leftSplit.Item2;
            return (leftSplit.Item1, treapNode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Insert(TKey element, TValue value) =>
            root = Insert(root,
                new TreapNode<TKey, TValue, TData>(element, value, dataCombiningOperation, dataBuildingOperation));

        private static TreapNode<TKey, TValue, TData> Insert(TreapNode<TKey, TValue, TData> treapNode,
            TreapNode<TKey, TValue, TData> inserting)
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
                return treapNode;
            }

            var splitResult = Split(treapNode, inserting.Key);
            inserting.Left = splitResult.Item1;
            inserting.Right = splitResult.Item2;
            return inserting;
        }

        public (TValue, TData) Find(TKey key)
        {
            var node = Find(root, key);
            return (node.Value, node.NodeData);
        }

        private static TreapNode<TKey, TValue, TData> Find(TreapNode<TKey, TValue, TData> treapNode, TKey key)
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

        private static TreapNode<TKey, TValue, TData> Erase(TreapNode<TKey, TValue, TData> treapNode, TKey element)
        {
            if (treapNode == null) return null;
            if (treapNode.Key.CompareTo(element) == 0)
                return Merge(treapNode.Left, treapNode.Right);
            if (treapNode.Key.CompareTo(element) < 0)
                treapNode.Right = Erase(treapNode.Right, element);
            else
                treapNode.Left = Erase(treapNode.Left, element);
            return treapNode;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Treap<TKey, TValue, TData> Union(Treap<TKey, TValue, TData> treap1,
            Treap<TKey, TValue, TData> treap2) =>
            new Treap<TKey, TValue, TData> {root = Union(treap1.root, treap2.root)};

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Treap<TKey, TValue, TData> Union(IEnumerable<Treap<TKey, TValue, TData>> treaps) =>
            new Treap<TKey, TValue, TData> {root = Union(treaps.Select(x => x.root))};

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Treap<TKey, TValue, TData> Union(params Treap<TKey, TValue, TData>[] treaps) =>
            Union(treaps.AsEnumerable());

        private static TreapNode<TKey, TValue, TData> Union(TreapNode<TKey, TValue, TData> left,
            TreapNode<TKey, TValue, TData> right)
        {
            if (left == null) return right;
            if (right == null) return left;
            if (left.Priority > right.Priority)
            {
                var splitResult = Split(right, left.Key);
                left.Left = Merge(left.Left, splitResult.Item1);
                left.Right = Merge(left.Right, splitResult.Item2);
                return left;
            }
            else
            {
                var splitResult = Split(left, right.Key);
                right.Left = Merge(right.Left, splitResult.Item1);
                right.Right = Merge(right.Right, splitResult.Item2);
                return right;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static TreapNode<TKey, TValue, TData> Union(IEnumerable<TreapNode<TKey, TValue, TData>> treapNodes) =>
            treapNodes.Aggregate(Union);

        private static void SiftDown(TreapNode<TKey, TValue, TData> treapNode)
        {
            while (true)
            {
                if (treapNode == null) return;
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
        }


        private class TreapNode<TNodeKey, TNodeValue, TNodeData>
        {
            // ReSharper disable once StaticMemberInGenericType
            private static readonly Random rng = new Random();
            private readonly Func<TNodeValue, TNodeData> dataBuildingOperation;
            private readonly AssociativeOperation<TNodeData> dataCombiningOperation;
            public readonly TNodeKey Key;
            public int Count;

            private TreapNode<TNodeKey, TNodeValue, TNodeData> left;
            public int Priority;
            private TreapNode<TNodeKey, TNodeValue, TNodeData> right;
            private TNodeValue value;

            public TreapNode(TNodeKey key, TNodeValue value, AssociativeOperation<TNodeData> dataCombiningOperation,
                Func<TNodeValue, TNodeData> dataBuildingOperation)
            {
                Key = key;
                Value = value;
                this.dataCombiningOperation = dataCombiningOperation;
                this.dataBuildingOperation = dataBuildingOperation;
                Count = 1;
                Priority = rng.Next();
                NodeData = dataBuildingOperation == null ? default(TNodeData) : dataBuildingOperation.Invoke(Value);
            }

            public TNodeValue Value
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => value;
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                set
                {
                    this.value = value;
                    NodeData = dataCombiningOperation(left.NodeData,
                        dataCombiningOperation(dataBuildingOperation(this.value), right.NodeData));
                }
            }

            public TreapNode<TNodeKey, TNodeValue, TNodeData> Left
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => left;
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                set
                {
                    left = value;
                    Count = left.Count + right.Count + 1;
                    NodeData = dataCombiningOperation(left.NodeData,
                        dataCombiningOperation(dataBuildingOperation(Value), right.NodeData));
                }
            }

            public TreapNode<TNodeKey, TNodeValue, TNodeData> Right
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => right;
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                set
                {
                    right = value;
                    Count = left.Count + right.Count + 1;
                    NodeData = dataCombiningOperation(left.NodeData,
                        dataCombiningOperation(dataBuildingOperation(Value), right.NodeData));
                }
            }

            public TNodeData NodeData { get; private set; }
        }
    }
}