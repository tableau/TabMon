using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TabMon.Sampler
{
    /// <summary>
    /// Compares dictionaries comprised of datatable metadata.
    /// </summary>
    internal class CounterSampleDictionaryComparer : IEqualityComparer<IDictionary<string,string>>
    {
        private static readonly IList<string> staticColumns = new[] { "Cluster", "Machine", "Source", "Category", "Instance", "Unit" };

        public bool Equals(IDictionary<string,string> x, IDictionary<string,string> y)
        {
            foreach (var staticColumn in staticColumns)
            {
                if (x[staticColumn] != y[staticColumn])
                {
                    return false;
                }
            }
            return true;
        }

        public int GetHashCode(IDictionary<string, string> dict)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var staticColumn in staticColumns)
            {
                sb.Append(dict[staticColumn]);
            }
            return sb.ToString().GetHashCode();
        }
    }

    /// <summary>
    /// Compresses a datatable based off the uniqueness of the metadata.
    /// </summary>
    public static class CounterSamplerResultHelper
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Compresses a data table into a semi-sparse data structure by evaluating the uniqueness of the metadata associated with each row.
        /// </summary>
        /// <param name="table">The original data table.</param>
        /// <returns>The compressed data table.</returns>
        public static DataTable CompressTable(DataTable table)
        {
            var distinctRowTypes = from DataRow dataRow in table.Rows
                                  orderby dataRow["Instance"].ToString()
                                  select new Dictionary<string, string>()
                                  { { "Cluster", dataRow["Cluster"].ToString() },
                                      { "Machine", dataRow["Machine"].ToString() },
                                      { "Source", dataRow["Source"].ToString() },
                                      { "Category", dataRow["Category"].ToString() },
                                      { "Instance", dataRow["Instance"].ToString() },
                                      { "Unit", dataRow["Unit"].ToString() }
                                  };
            var distinctRows = distinctRowTypes.Distinct(new CounterSampleDictionaryComparer()).ToList();

            var compressedTable = table.Clone();

            foreach (var distinctRow in distinctRows)
            {
                // Builds the select statment based off of the values in the distinctRow dictionary
                var selectString = BuildSelectString(distinctRow);

                // Queries the data table for all rows that match the distinct row metadata
                DataRow[] foundRows = table.Select(selectString);

                DataRow newRow = null;
                foreach (var singleRow in foundRows)
                {
                    if (newRow == null)
                    {
                        newRow = singleRow;
                    }
                    else
                    {
                        foreach (DataColumn column in newRow.Table.Columns)
                        {
                            var columnValue = newRow[column.ColumnName].ToString();
                            var comparedValue = singleRow[column.ColumnName].ToString();
                            if (String.IsNullOrWhiteSpace(columnValue) && !String.IsNullOrWhiteSpace(comparedValue))
                            {
                                newRow[column.ToString()] = singleRow[column.ToString()];
                            }
                        }
                    }
                }
                compressedTable.ImportRow(newRow);
            }

            Log.DebugFormat("Compressed {0} rows into {1} unique rows..", table.Rows.Count, compressedTable.Rows.Count);
            return compressedTable;
        }

        /// <summary>
        /// Builds the select statment that is used to query the original data table. 
        /// </summary>
        /// <param name="metaData">A dictionary that consists of the metadata column name and the value associated with it.</param>
        /// <returns>The select statement string.</returns>
        private static string BuildSelectString(IDictionary<string, string> metaData)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var entry in metaData)
            {
                if (sb.ToString() != "")
                {
                    sb.Append(" And ");
                }

                if (entry.Value != "")
                {
                    sb.AppendFormat("{0} = '{1}'", entry.Key, entry.Value);
                }
                else if (entry.Value == "")
                {
                    sb.AppendFormat("{0} Is Null", entry.Key);
                }
            }

            return sb.ToString();
        }
    }
}
