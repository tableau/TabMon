using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;

namespace DataTableWriter
{
    /// <summary>
    /// Extensions for the String class.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Generates the correctly-pluralized form of a string.
        /// </summary>
        /// <param name="value">The string to pluralize.</param>
        /// <param name="count">The quantity used to determine pluralized form.</param>
        /// <returns>Pluralized form of this string using the given quantity.</returns>
        public static string Pluralize(this string value, int count)
        {
            if (count == 1)
            {
                return value;
            }
            return PluralizationService
                .CreateService(new CultureInfo("en-US"))
                .Pluralize(value);
        }
    }
}