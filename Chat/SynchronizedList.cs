using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Chat
{
    public class SafeList<T> : IEnumerable<T>
    {
        private List<T> list;
        private readonly object lockObject = new object();

        public SafeList()
        {
            list = new List<T>();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Clone().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Clone().GetEnumerator();
        }

        public List<T> Clone()
        {
            lock (lockObject)
            {
                return new List<T>(list);
            }
        }

        public void Add(T item)
        {
            lock (lockObject)
            {
                list.Add(item);
            }
        }

        public void Remove(T item)
        {
            lock (lockObject)
            {
                list.Remove(item);
            }
        }
    }
}
