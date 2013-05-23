using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sandstorm.ParticleSystem.structs
{
    /// <summary>
    /// Represents a pool of objects with a size limit.
    /// </summary>
    /// <typeparam name="T">The type of object in the pool.</typeparam>
    public sealed class ObjectPool<T> : IDisposable where T : new()
    {
        private readonly int _size;
        private readonly object _locker;
        private readonly Queue<T> _queue;

        /// <summary>
        /// Initializes a new instance of the ObjectPool class.
        /// </summary>
        /// <param name="size">The size of the object pool.</param>
        public ObjectPool(int pSize)
        {
            if (pSize <= 0)
            {
                const string message = "The size of the pool must be greater than zero.";
                throw new ArgumentOutOfRangeException("size", pSize, message);
            }

            this._size = pSize;
            _locker = new object();
            _queue = new Queue<T>();
        }


        /// <summary>
        /// Retrieves an item from the pool. 
        /// </summary>
        /// <returns>The item retrieved from the pool.</returns>
        public T Get()
        {
            lock (_locker)
            {
                if (_queue.Count > 0)
                {
                    return _queue.Dequeue();
                }
                else
                    return new T();
            }
        }

        /// <summary>
        /// Places an item in the pool.
        /// </summary>
        /// <param name="item">The item to place to the pool.</param>
        public void Put(T pItem)
        {
            lock (_locker)
            {
                if (_queue.Count < _size)
                {
                    _queue.Enqueue(pItem);
                }
            }
        }

        /// <summary>
        /// Disposes of items in the pool that implement IDisposable.
        /// </summary>
        public void Dispose()
        {
            lock (_locker)
            {
                while (_queue.Count > 0)
                {
                    using (_queue.Dequeue() as IDisposable)
                    {

                    }
                }
            }
        }
    }
}
