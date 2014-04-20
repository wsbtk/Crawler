﻿using System;
using System.Collections.Generic;
using System.Threading;

namespace Crawler
{
    //class ConcurrentHash
    //{
        public class ConcurrentHashSet<T> : IDisposable
        {
            private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
            private readonly HashSet<T> _hashSet = new HashSet<T>();

            #region Implementation of ICollection<T> ...ish
            public bool Add(T item)
            {
                try
                {
                    _lock.EnterWriteLock();
                    return _hashSet.Add(item);
                }
                finally
                {
                    if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
                }
            }

            public void Clear()
            {
                try
                {
                    _lock.EnterWriteLock();
                    _hashSet.Clear();
                }
                finally
                {
                    if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
                }
            }

            public bool Contains(T item)
            {
                try
                {
                    _lock.EnterReadLock();
                    return _hashSet.Contains(item);
                }
                finally
                {
                    if (_lock.IsReadLockHeld) _lock.ExitReadLock();
                }
            }

            public bool Remove(T item)
            {
                try
                {
                    _lock.EnterWriteLock();
                    return _hashSet.Remove(item);
                }
                finally
                {
                    if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
                }
            }

            public int Count
            {
                get
                {
                    try
                    {
                        _lock.EnterReadLock();
                        return _hashSet.Count;
                    }
                    finally
                    {
                        if (_lock.IsReadLockHeld) _lock.ExitReadLock();
                    }
                }
            }
            #endregion

            #region Dispose
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
            protected virtual void Dispose(bool disposing)
            {
                if (disposing)
                    if (_lock != null)
                        _lock.Dispose();
            }
            ~ConcurrentHashSet()
            {
                Dispose(false);
            }
            #endregion
        }
    //}
}
