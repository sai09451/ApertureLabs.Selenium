using System;
using System.Collections.Generic;

namespace ApertureLabs.Selenium.Extensions
{
    /// <summary>
    /// Contains extensions for classes implementing IDisposable.
    /// </summary>
    public static class IDisposableExtensions
    {
        /// <summary>
        /// Calls <code>IDisposable.Dispose</code> on each disposable after
        /// executing the function.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="disposables"></param>
        /// <param name="function"></param>
        /// <returns></returns>
        public static U Using<T, U>(this IEnumerable<T> disposables,
            Func<IEnumerable<T>, U> function) where T : IDisposable
        {
            var result = function(disposables);

            foreach (var disposable in disposables)
                disposable.Dispose();

            return result;
        }

        /// <summary>
        /// Calls <code>IDisposable.Dispose</code> on each disposable after
        /// executing the function.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="disposables"></param>
        /// <param name="action"></param>
        public static void Using<T>(this IEnumerable<T> disposables,
            Action<IEnumerable<T>> action)
            where T : IDisposable
        {
            action(disposables);

            foreach (var disposable in disposables)
                disposable.Dispose();
        }
    }
}
