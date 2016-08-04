using CsvHelper;
using log4net;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DataTableWriter.Writers
{
    /// <summary>
    /// Contains functionality for writing DataTable objects to CSV file.
    /// </summary>
    public class DataTableCsvWriter : IDataTableWriter
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private bool disposed;
        protected bool headerHasBeenWritten;
        protected StreamWriter streamWriter;
        protected CsvWriter csvWriter;

        public virtual string Name
        {
            get { return "CSV File Writer"; }
        }

        public DataTableCsvWriter(string filePath)
        {
            streamWriter = File.CreateText(filePath);
            csvWriter = new CsvWriter(streamWriter);
        }

        #region Public Methods

        /// <summary>
        /// Writes the data table to CSV.  Also ensures that the header has been written.
        /// </summary>
        /// <param name="table">The data table to write.</param>
        public void Write(DataTable table)
        {
            if (!headerHasBeenWritten)
            {
                Log.Debug("Writing header to CSV..");
                WriteHeader(table);
                headerHasBeenWritten = true;
            }

            Log.Debug(String.Format("Writing {0} {1} to CSV..", table.Rows.Count, "record".Pluralize(table.Rows.Count)));
            var compressedTable = compressTable(table);
            foreach (DataRow row in compressedTable.Rows)
            {
                WriteRow(row);
            }
            Log.Debug(String.Format("Finished writing {0}!", "record".Pluralize(table.Rows.Count)));
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Writes header information to CSV.
        /// </summary>
        /// <param name="table">The table to use as a point of reference for header information.</param>
        protected void WriteHeader(DataTable table)
        {
            foreach (DataColumn column in table.Columns)
            {
                csvWriter.WriteField(column.ColumnName);
            }
            csvWriter.NextRecord();
        }

        /// <summary>
        /// Writes a single row of data to CSV.
        /// </summary>
        /// <param name="row">The data row to write.</param>
        protected void WriteRow(DataRow row)
        {
            try
            {
                for (var i = 0; i < row.Table.Columns.Count; i++)
                {
                    if (row[i] != null)
                    {
                        csvWriter.WriteField(row[i]);
                    }
                    else
                    {
                        csvWriter.WriteField("");
                    }
                }
                csvWriter.NextRecord();
            }
            catch (CsvWriterException ex)
            {
                Log.Error("Error writing record to CSV: " + ex.Message);
            }
        }

        protected static string BuildSelect(string category1, string category2, string category3, string category4, string category5, string category6)
        {
            StringBuilder sb = new StringBuilder();
            if (category1 != "")
            {
                sb.AppendFormat("Cluster = '{0}'", category1);
            }
            else
            {
                sb.Append("Cluster Is Null");
            }

            if (category2 != "")
            {
                sb.AppendFormat(" And Machine = '{0}'", category2);
            }
            else
            {
                sb.Append(" And Machine Is Null");
            }

            if (category3 != "")
            {
                sb.AppendFormat(" And Source = '{0}'", category3);
            }
            else
            {
                sb.Append(" And Source Is Null");
            }

            if (category4 != "")
            {
                sb.AppendFormat(" And Category = '{0}'", category4);
            }
            else
            {
                sb.Append(" And Category Is Null");
            }

            if (category5 != "")
            {
                sb.AppendFormat(" And Instance = '{0}'", category5);
            }
            else
            {
                sb.Append(" And Instance Is Null");
            }

            if (category6 != "")
            {
                sb.AppendFormat(" And Unit = '{0}'", category6);
            }
            else
            {
                sb.Append(" And Unit Is Null");
            }

            return sb.ToString();
        }

        protected static DataTable compressTable(DataTable table)
        {
            Log.Info("Printing original table");
            foreach (DataRow row in table.Rows)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var thing in row.ItemArray)
                {
                    sb.Append(thing.ToString() + "|");
                }
                Log.Info(sb);
            }

            var distinctValues = (from DataRow dr in table.Rows
                                  orderby dr["Instance"].ToString()
                                  select new
                                  {
                                      temp1 = dr["Cluster"].ToString(),
                                      temp2 = dr["Machine"].ToString(),
                                      temp3 = dr["Source"].ToString(),
                                      temp4 = dr["Category"].ToString(),
                                      temp5 = dr["Instance"].ToString(),
                                      temp6 = dr["Unit"].ToString()
                                  }).Distinct();

            Log.Info("Printing unique values");
            foreach (var a in distinctValues)
            {
                Log.Info(a.ToString());
            }
             
            var newTable = table.Clone();

            foreach (var item in distinctValues)
            {
                DataRow[] foundRows;
                var selectString = BuildSelect(item.temp1, item.temp2, item.temp3, item.temp4, item.temp5, item.temp6);
                foundRows = table.Select(selectString);

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
                newTable.ImportRow(newRow);
            }

            Log.Info("Printing compressed table");
            foreach (DataRow row in newTable.Rows)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var thing in row.ItemArray)
                {
                    sb.Append(thing.ToString() + "|");
                }
                Log.Info(sb);
            }
            return newTable;
        }

        #endregion Protected Methods

        #region IDisposable Methods

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                csvWriter.Dispose();
                streamWriter.Dispose();
            }
            disposed = true;
        }

        #endregion IDisposable Methods
    }
}