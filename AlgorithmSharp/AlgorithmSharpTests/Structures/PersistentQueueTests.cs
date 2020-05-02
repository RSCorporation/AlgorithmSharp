using AlgorithmSharp.Structures;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace AlgorithmSharpTests.Structures
{
    [TestFixture]
    class PersistentQueueTests
    {
        [Test]
        public void BasicOperations()
        {
            var queue = PersistentQueue<int>.Create();
            Assert.AreEqual(queue.Count, 0);
            queue.Enqueue(1, out queue);
            queue.ToArray();
            queue.Enqueue(2, out queue);
            queue.ToArray();
            queue.Enqueue(3, out queue);
            queue.ToArray();
            queue.Enqueue(4, out queue);
            queue.ToArray();
            queue.Enqueue(5, out queue);
            Assert.AreEqual(new int[] { 1, 2, 3, 4, 5 }, queue.ToArray());
            Assert.AreEqual(true, queue.Contains(2), "Contains 2");
            Assert.AreEqual(true, queue.Contains(4), "Contains 4");
            Assert.AreEqual(false, queue.Contains(6), "Contains 6");
            Assert.AreEqual(1, queue.Peek());
            Assert.AreEqual(5, queue.Count);
            Assert.AreEqual(true, queue.TryPeek(out _));
            Assert.AreEqual(true, queue.TryDequeue(out int res, out queue));
            Assert.AreEqual(1, res);
            Assert.AreEqual(2, queue.Peek());
            Assert.AreEqual(4, queue.Count);
            queue.Dequeue(out queue);
            queue.Dequeue(out queue);
            queue.Dequeue(out queue);
            Assert.AreEqual(5, queue.Dequeue(out queue));
            Assert.AreEqual(0, queue.Count);
            Assert.Throws(typeof(InvalidOperationException), delegate { queue.Dequeue(out queue); });
            Assert.Throws(typeof(InvalidOperationException), delegate { queue.Peek(); });
            Assert.AreEqual(false, queue.TryPeek(out _));
            Assert.AreEqual(false, queue.TryDequeue(out _, out var queuec));
            Assert.AreSame(queue, queuec);
        }
        [Test]
        public void Branchicg()
        {
            var queue = PersistentQueue<int>.Create(new int[] { 1, 2, 3 });
            var queue2 = queue;
            Assert.AreEqual(new int[] { 1, 2, 3 }, queue);
            queue.Enqueue(4, out queue);
            queue2.Enqueue(5, out queue2);
            Assert.AreEqual(true, queue.Contains(4));
            Assert.AreEqual(true, queue2.Contains(5));
            Assert.AreEqual(false, queue.Contains(5));
            Assert.AreEqual(false, queue2.Contains(4));
            queue2.Dequeue(out queue2);
            queue2.Dequeue(out queue2);
            queue2.Dequeue(out queue2);
            queue2.Dequeue(out queue2);
            Assert.AreEqual(0, queue2.Count);
            Assert.AreEqual(new int[] { 1, 2, 3, 4 }, queue);
        }
    }
}
