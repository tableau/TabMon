using DataTableWriter.Adapters;
using DataTableWriter.Connection;
using DataTableWriter.Drivers;
using log4net;
using System;
using System.Data;
using System.Data.Common;
using System.Reflection;

namespace DataTableWriter.Writers
{
    /// <summary>
    /// Contains functionality for writing DataTable objects to a database.
    /// </summary>
    public class DataTableDbWriter : IDataTableWriter, IPurgeableDatasource
    {
        protected IDbAdapter Adapter { get; set; }
        protected DbTableInitializationOptions tableInitializationOptions;
        protected bool isInitialized;
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private bool disposed;

        public DataTableDbWriter(DbDriverType driverType, IDbConnectionInfo connectionInfo, DbTableInitializationOptions tableInitializationOptions = default(DbTableInitializationOptions))
        {
            Adapter = new DbAdapter(driverType, connectionInfo);
            this.tableInitializationOptions = tableInitializationOptions;
        }

        ~DataTableDbWriter()
        {
            Dispose(false);
        }

        public string Name
        {
            get { return String.Format("Database Writer ({0})", Adapter.Driver.Name); }
        }

        public void Write(DataTable table)
        {
            Log.Debug(String.Format("Writing {0} {1} to database..", table.Rows.Count, "record".Pluralize(table.Rows.Count)));

            // Reopen connection, if it has closed for some reason.
            if (!Adapter.IsConnectionOpen())
            {
                Adapter.OpenConnection();
            }

            // Manage dynamic table creation/management, if requested.
            if (!isInitialized)
            {
                DbTableManager.InitializeTable(Adapter, table, tableInitializationOptions);
                isInitialized = true;
            }

            // Write all rows in table.
            int numRecordsWritten = 0;
            foreach (DataRow row in table.Rows)
            {
                try
                {
                    Adapter.InsertRow(table.TableName, row);
                    numRecordsWritten++;
                }
                catch (DbException) { }
            }
            Log.Debug(String.Format("Finished writing {0} {1}!", numRecordsWritten, "record".Pluralize(numRecordsWritten)));
        }

        public void PurgeExpiredData(string table)
        {
            if (tableInitializationOptions.PurgeData == false)
            {
                return;
            }
            if (!Adapter.IsConnectionOpen())
            {
                Adapter.OpenConnection();
            }

            try
            {
                DbTableManager.PurgeExpiredData(Adapter, table, tableInitializationOptions.PurgeDataThreshold);
            }
            catch (DbException ex)
            {
                Log.ErrorFormat("Unable to delete rows: {0}", ex.Message);
                throw;
            }
        }

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
                Adapter.Dispose();
            }
            disposed = true;
        }
    }
}