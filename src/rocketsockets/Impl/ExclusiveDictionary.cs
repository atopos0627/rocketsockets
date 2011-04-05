using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace rocketsockets
{
    public class ExclusiveDictionary<TKey, TValue>
    {
        public ReaderWriterLockSlim SlimLock { get; set; }
        public ConcurrentDictionary<TKey, TValue> Dictionary { get; set; }

        public int MostWaiting { get; set; } // for diagnostic purposes

        public int Count
        {
            get { return Dictionary.Count; }
        }

        public IEnumerable<TValue> Values
        {
            get { return Dictionary.Values; }
        }

        public TValue this[ TKey key ]
        {
            get { return GetOrDefault( key ); }
            set
            {
                try
                {
                    Dictionary[key] = value;
                }
                catch ( Exception e )
                {
                    Console.WriteLine( e );
                }
            }
        }

        public TValue GetOrDefault( TKey key )
        {
            var value = default(TValue);
            Dictionary.TryGetValue( key, out value );
            return value;
        }

        public TValue ReadOrWrite( TKey key, Func<TValue> valueProvider )
        {
            TValue value = default(TValue);
            if ( !Dictionary.TryGetValue( key, out value ) )
            {
                try
                {
                    SlimLock.EnterWriteLock();
                    UpdateWaiting();
                    if ( !Dictionary.TryGetValue( key, out value ) )
                        value = Dictionary.GetOrAdd( key, valueProvider() );
                }
                finally
                {
                    SlimLock.ExitWriteLock();
                }
            }
            return value;
        }

        public void Remove( TKey key )
        {
            try
            {
                SlimLock.EnterWriteLock();
                TValue value;
                Dictionary.TryRemove( key, out value );
            }
            catch ( Exception ex )
            {
                SlimLock.ExitWriteLock();
            }
        }

        public void UpdateWaiting()
        {
            var waiting = SlimLock.WaitingWriteCount;
            MostWaiting = MostWaiting > waiting
                              ? MostWaiting
                              : waiting;
        }

        public ExclusiveDictionary() 
        {
            SlimLock = new ReaderWriterLockSlim();
            Dictionary = new ConcurrentDictionary<TKey, TValue>();
        }
    }
}