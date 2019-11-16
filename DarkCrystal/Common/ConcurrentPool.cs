
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Concurrent;

namespace DarkCrystal
{
    public class ConcurrentPool<T>
    {
        private System.Func<T> CreateFunction;
        private ConcurrentQueue<T> FreeObjects = new ConcurrentQueue<T>();
        
        public ConcurrentPool(System.Func<T> createFunction = null)
        {
            CreateFunction = createFunction != null ? createFunction : () => default(T);
        }

        public T Retain()
        {
            if (FreeObjects.TryDequeue(out T @object))
            {
                return @object;
            }
            return CreateFunction();
        }

        public void Release(T @object)
        {
            FreeObjects.Enqueue(@object);
        }
    }
}