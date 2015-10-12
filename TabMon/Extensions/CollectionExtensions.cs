using System.Collections.Generic;

namespace TabMon
{
    /// <summary>
    /// Extension methods for the System.Collections.Generic.ICollection class.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Adds all items from an IEnumerable into an existing collection.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="collection">The collection to add the items to.</param>
        /// <param name="enumerable">The enumerable containing the items to add.</param>
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> enumerable)
        {
            foreach (var item in enumerable)
            {
                collection.Add(item);
            }
        }
    }
}