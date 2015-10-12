using System.Collections.Generic;
using System.Data;

namespace DataTableWriter.Helpers
{
    /// <summary>
    /// Custom comparison helper used to compare two DataColumn objects.
    /// </summary>
    internal class DataColumnEqualityComparer : IEqualityComparer<DataColumn>
    {
        private DataColumnEqualityComparer() { }

        public static DataColumnEqualityComparer instance = new DataColumnEqualityComparer();

        /// <summary>
        /// Indicates whether two DataColumns are equal.
        /// </summary>
        /// <param name="first">The first column to compare.</param>
        /// <param name="second">The second column to compare.</param>
        /// <returns>True if the two DataColumns have the same name and data type.</returns>
        public bool Equals(DataColumn first, DataColumn second)
        {
            if (first.ColumnName != second.ColumnName)
            {
                return false;
            }
            return first.DataType == second.DataType;
        }

        /// <summary>
        /// Retrieve the hash code for DataColumn object.
        /// </summary>
        /// <param name="obj">The DataColumn to retrieve a hash code for.</param>
        /// <returns>The hash code of the given DataColumn.</returns>
        public int GetHashCode(DataColumn obj)
        {
            var hash = 17;
            hash = 31 * hash + obj.ColumnName.GetHashCode();
            hash = 31 * hash + obj.DataType.GetHashCode();

            return hash;
        }
    }
}