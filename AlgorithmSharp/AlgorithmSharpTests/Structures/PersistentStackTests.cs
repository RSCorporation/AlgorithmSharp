using AlgorithmSharp.Structures;
using NUnit.Framework;
using System;

namespace AlgorithmSharpTests.Structures
{
    [TestFixture]
    class PersistentStackTests
    {
        [Test]
        public void InstantiationAndBasicOperations()
        {
            var stack = PersistentStack<int>.Create();
            Assert.AreEqual(0, stack.Count);
            stack.Push(1, out stack);
            stack.Push(2, out stack);
            stack.Push(3, out stack);
            Assert.AreEqual(true, stack.Contains(2));
            Assert.AreEqual(true, stack.Contains(1));
            Assert.AreEqual(false, stack.Contains(4));
            Assert.AreEqual(3, stack.Peek());
            Assert.AreEqual(3, stack.Count);
            Assert.AreEqual(true, stack.TryPeek(out _));
            Assert.AreEqual(true, stack.TryPop(out int res, out stack));
            Assert.AreEqual(3, res);
            Assert.AreEqual(2, stack.Peek());
            Assert.AreEqual(2, stack.Count);
            stack.Pop(out stack);
            stack.Pop(out stack);
            Assert.AreEqual(PersistentStack<int>.EmptyStack, stack);
            Assert.Throws(typeof(InvalidOperationException), delegate { stack.Pop(out stack); });
            Assert.Throws(typeof(InvalidOperationException), delegate { stack.Peek(); });
            Assert.AreEqual(PersistentStack<int>.EmptyStack, stack);
            Assert.AreEqual(false, stack.TryPeek(out _));
        }
        [Test]
        public void Branching()
        {
            var stack = PersistentStack<int>.Create();
            stack.Push(1, out stack);
            var stackCopy = stack;
            stack.Push(3, out stackCopy);
            stack.Push(2, out stack);
            Assert.AreEqual(true, stackCopy.Contains(3));
            Assert.AreEqual(false, stack.Contains(3));
            Assert.AreEqual(false, stackCopy.Contains(2));
            Assert.AreEqual(true, stack.Contains(2));
            Assert.AreEqual(2, stack.Peek());
            stack.Pop(out stack);
            stack.Pop(out stack);
            Assert.AreEqual(PersistentStack<int>.EmptyStack, stack);
            Assert.AreEqual(3, stackCopy.Pop(out stackCopy));
            Assert.AreEqual(1, stackCopy.Pop(out stackCopy));
        }

        [Test]
        public void Enumerables()
        {
            var arr = new int[] { 1, 2, 3 };
            var stack = PersistentStack<int>.Create(arr);
            Assert.AreEqual(3, stack.Peek());
            Assert.AreEqual(new int[] { 3, 2, 1 }, stack.ToArray());
            int nxt = 3;
            foreach (var item in stack)
                Assert.AreEqual(nxt--, item);
        }
    }
}
