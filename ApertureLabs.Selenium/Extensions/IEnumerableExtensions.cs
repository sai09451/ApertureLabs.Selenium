using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
