using System;
using System.Collections.Generic;
using System.Linq;

namespace ApertureLabs.Selenium.Extensions
{
    public delegate ref TResult TempSetRefDelegate<TSource, TResult>(TSource source);

    public static class DisposableExtensions
    {
        /// <summary>
        /// Wraps a disposable in a using statement return the actions result.
        /// This will call <code>Dispose()</code> even if an error occurs.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="disposable"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static T Use<T>(this IDisposable disposable,
            Func<IDisposable, T> action)
        {
            using (disposable)
            {
                return action(disposable);
            };
        }

        /// <summary>
        /// Wraps a disposable in a using statement. This will call
        /// <code>Dispose()</code> even if an error occurs.
        /// </summary>
        /// <param name="disposable"></param>
        /// <param name="action"></param>
        public static void Use(this IDisposable disposable,
            Action<IDisposable> action)
        {
            using (disposable)
            {
                action(disposable);
            };
        }

        public static void TemporarilySet<TSource, U>(
            this TSource @object,
            Func<TSource, U> getter,
            Action<TSource, U> setter,
            U newVal,
            Action<TSource> then)
        {
            // Store old val and update object with newVal.
            var oldVal = getter(@object);
            setter(@object, newVal);

            // Call 'then' after property contains the new values.
            then(@object);

            // Unset.
            setter(@object, oldVal);
        }
    }
}
