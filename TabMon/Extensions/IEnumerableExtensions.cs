using System.Collections.Generic;

namespace TabMon
{
    /// <summary>
    /// Extension methods for the System.Collection.Generic.IEnumerable class.
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Wraps object instance into an IEnumerable consisting of a single item.
        /// </summary>
        /// <typeparam name="T">Type of the object.</typeparam>
        /// <param name="item">The instanced to be wrapped.</param>
        /// <returns>IEnumerable consisting of a single item.</returns>
        public static IEnumerable<T> Yield<T>(this T item)
        {
            yield return item;
        }
    }
}