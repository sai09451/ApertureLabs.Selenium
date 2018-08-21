using System;

namespace ApertureLabs.Selenium.Extensions
{
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
    }
}
