﻿using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Ion.Specialized.Utilities.Extensions
{
    public static class ConcurrentQueueExtensions
    {
        /// <summary>
        /// Drain to a list
        /// </summary>
        public static List<T> Dequeue<T>(this ConcurrentQueue<T> queue)
        {
            var list = new List<T>();

            while (queue.Count > 0)
            {
                T element;

                if (queue.TryDequeue(out element))
                    list.Add(element);
            }

            return list;
        }
    }
}
