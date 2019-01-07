﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace ApertureLabs.Selenium.Extensions
{
    /// <summary>
    /// Contains extensions for ICollection.
    /// </summary>
    public static class IEnumerableExtensions
    {
        private static Random Random = new Random();

        /// <summary>
        /// Selects a random element from a list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static T SelectRandom<T>(this IEnumerable<T> collection)
        {
            var rnd = Random.Next(collection.Count());
            return collection.ElementAt(rnd);
        }

        /// <summary>
        /// Retrieves the index of an item in a collection. Returns -1 if the
        /// item isn't in the collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static int IndexOf<T>(this IEnumerable<T> collection, T item)
        {
            var match = collection
                .Select((Item, Index) => new { Item, Index })
                .FirstOrDefault(q => q.Item.Equals(item));

            return match?.Index ?? -1;
        }
    }
}
