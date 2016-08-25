using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using System.Text;

namespace TabMon
{
    /// <summary>
    /// Extension methods for the System.String class.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Pluralizes a string.
        /// </summary>
        /// <param name="str">The string to pluralize.</param>
        /// <param name="count">The quantity used to pluralize.</param>
        /// <returns>Pluralized form of string.</returns>
        public static string Pluralize(this string str, int count)
        {
            if (count == 1)
            {
                return str;
            }
            return PluralizationService
                .CreateService(new CultureInfo("en-US"))
                .Pluralize(str);
        }

        /// <summary>
        /// Converts a string to the snake case (lowercase with underscores instead of spaces).
        /// </summary>
        /// <param name="str">A string that is Camel Case or Pluralized.</param>
        /// <returns>The string in the snake case.</returns>
        public static string ToSnakeCase(this string str)
        {
            str = str.Trim().Replace(" ", "_");

            for (var i = 1; i < str.Length; i++)
            {
                if (char.IsLower(str[i - 1]) && char.IsUpper(str[i]))
                {
                    str = str.Insert(i, "_");
                }
            }
            return str.ToLower();
        }

        /// <summary>
        /// Joins multiple strings together with a delimiter.  If a string segment is null, it will not be appended.
        /// </summary>
        public static string JoinIfNotNull(this string baseString, string separator, params string[] stringsToJoin)
        {
            if (stringsToJoin == null || stringsToJoin.Length == 0)
            {
                return baseString;
            }

            var result = new StringBuilder(baseString);
            foreach (var joinStr in stringsToJoin.Where(joinStr => joinStr != null))
            {
                result.Append(separator + joinStr);
            }

            return result.ToString();
        }
    }
}