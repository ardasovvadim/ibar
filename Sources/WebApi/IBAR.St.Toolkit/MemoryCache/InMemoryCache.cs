using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IBAR.St.Toolkit.MemoryCache
{
    public class InMemoryCache<T> where T : class
    {
        private readonly object _syncRoot = new object();
        private readonly Dictionary<string, T> _memoryCache = new Dictionary<string, T>();

        public void Add(string id, T obj)
        {
            lock (_syncRoot)
            {
                if (!_memoryCache.ContainsKey(id))
                {
                    _memoryCache.Add(id, obj);
                }
            }
        }

        public bool Contains(string key)
        {
            lock (_syncRoot)
            {
                return _memoryCache.ContainsKey(key);
            }
        }

        public T GetById(string id)
        {
            lock (_syncRoot)
            {
                return _memoryCache[id];
            }
        }

        public T GetFirst()
        {
            lock (_syncRoot)
            {
                return _memoryCache.Values.First();
            }
        }

    }
}
