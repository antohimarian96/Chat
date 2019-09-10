using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Chat;
using Xunit;

namespace Tests
{
    public class ThreadSyncFacts
    {
        private const int Iterations = 100000;
        readonly static object lockObject = new object();
        List<string> firstList = new List<string>();
        SafeList<string> secondList = new SafeList<string>();

        [Fact]
        public void LockFacts()
        {
            var threads = new List<Thread>()
            {
                new Thread(AddElement){Name = "1"},
                new Thread(AddElement){Name = "2"},
                new Thread(AddElement){Name = "3"},
                new Thread(AddElement){Name = "4"}
            };
            threads.ForEach(t => t.Start());
            threads.ForEach(t => t.Join());
            Assert.Equal(Iterations * 4, firstList.Count);
        }

        private void AddElement()
        {
            var name = Thread.CurrentThread.Name;
            for (int i = 0; i < Iterations; i++)
            {
                lock(lockObject)
                firstList.Add(name);
            }
        }

        [Fact]
        public void SynchronizedListFact()
        {
            var list = new SafeList<string>();
            void AddElements()
            {
                Enumerable.Range(0, Iterations)
                    .ToList()
                    .ForEach(i => list.Add(i.ToString()));
            }
            var threads = new List<Thread>()
            {
                new Thread(AddElements){Name = "1"},
                new Thread(AddElements){Name = "2"},
                new Thread(AddElements){Name = "3"},
                new Thread(AddElements){Name = "4"}
            };
            threads.ForEach(t => t.Start());
            threads.ForEach(t => t.Join());
            Assert.Equal(Iterations * 4, list.Count());
        }
    }
}

