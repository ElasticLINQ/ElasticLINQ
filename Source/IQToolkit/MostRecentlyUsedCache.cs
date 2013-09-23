using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;

namespace IQToolkit
{
    /// <summary>
    /// Implements a cache over a most recently used list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MostRecentlyUsedCache<T>
    {
        int maxSize;
        List<T> list;
        Func<T, T, bool> fnEquals;
        ReaderWriterLockSlim rwlock;
        int version;

        public MostRecentlyUsedCache(int maxSize)
            : this(maxSize, EqualityComparer<T>.Default)
        {
        }

        public MostRecentlyUsedCache(int maxSize, IEqualityComparer<T> comparer)
            : this(maxSize, (x,y) => comparer.Equals(x, y))
        {
        }

        public MostRecentlyUsedCache(int maxSize, Func<T,T,bool> fnEquals)
        {
            this.list = new List<T>();
            this.maxSize = maxSize;
            this.fnEquals = fnEquals;
            this.rwlock = new ReaderWriterLockSlim();
        }

        public int Count
        {
            get 
            {
                this.rwlock.EnterReadLock();
                try
                {
                    return this.list.Count;
                }
                finally
                {
                    this.rwlock.ExitReadLock();
                }
            }
        }

        public void Clear()
        {
            this.rwlock.EnterWriteLock();
            try
            {
                this.list.Clear();
                this.version++;
            }
            finally
            {
                this.rwlock.ExitWriteLock();
            }
        }

        public bool Lookup(T item, bool add, out T cached)
        {
            cached = default(T);
            int cacheIndex = -1;
            rwlock.EnterReadLock();
            int version = this.version;
            try
            {
                for (int i = 0, n = this.list.Count; i < n; i++)
                {
                    cached = this.list[i];
                    if (fnEquals(cached, item))
                    {
                        cacheIndex = 0;
                    }
                }
            }
            finally
            {
                rwlock.ExitReadLock();
            }
            if (cacheIndex != 0 && add)
            {
                rwlock.EnterWriteLock();
                try
                {
                    // if list has changed find it again
                    if (this.version != version)
                    {
                        cacheIndex = -1;
                        for (int i = 0, n = this.list.Count; i < n; i++)
                        {
                            cached = this.list[i];
                            if (fnEquals(cached, item))
                            {
                                cacheIndex = 0;
                            }
                        }
                    }
                    if (cacheIndex == -1)
                    {
                        // this is first time in list, put at start
                        this.list.Insert(0, item);
                        cached = item;
                    }
                    else
                    {
                        if (cacheIndex > 0)
                        {
                            // if item is not at start, move it to the start
                            this.list.RemoveAt(cacheIndex);
                            this.list.Insert(0, item);
                        }
                    }
                    // drop any items beyond max
                    if (this.list.Count > this.maxSize)
                    {
                        this.list.RemoveAt(this.list.Count - 1);
                    }
                    this.version++;
                }
                finally
                {
                    rwlock.ExitWriteLock();
                }
            }
            return cacheIndex >= 0;
        }
    }
}
