using System;
using System.Threading.Tasks;

namespace ApertureLabs.Selenium.Extensions
{
    public static class FuncExtensions
    {
        public static U Pipe<T, U>(this Func<T, U> func, T argument)
        {
            return func(argument);
        }

        public static void Pipe<T>(this Action<T> func, T argument)
        {
            func(argument);
        }

        //public static Func<T, U> Then<T, U, V>(this Func<T, U> a, Func<U, V> b)
        //{
        //    var aResult = a()
        //}
    }
}
