using DataTableWriter.Connection;
using DataTableWriter.Drivers;
using DataTableWriter.Helpers;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Runtime.Serialization;

namespace DataTableWriter.Adapters
{
    /// <summary>
    /// A simple database adapter to manage connections as well as exposing table manipulation functionality that other classes can leverage.
    /// </summary>
    internal class DbAdapter : IDbAdapter
    {
        public IDbDriver Driver { get; protected set; }
        public IDbConnectionInfo ConnectionInfo { get; protected set; }
        public IDbConnection Connection { get; protected set; }
        private bool disposed;
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public DbAdapter(DbDriverType driverType, IDbConnectionInfo connectionInfo)
        {
            Driver = DbDriverFactory.GetInstance(driverType);
            ConnectionInfo = connectionInfo;
            Connection = Driver.BuildConnection(ConnectionInfo);
            OpenConnection();
        }

        ~DbAdapter()
        {
            Dispose(false);
        }

        #region Public Methods

        /// <summary>
        /// Open the database connection.
        /// </summary>
        public void OpenConnection()
        {
            Connection.Open();

            if (!IsConnectionOpen())
            {
                throw new DbConnectionException("Could not open database connection using connection info " + ConnectionInfo);
            }
        }

        /// <summary>
        /// Close the database connection.
        /// </summary>
        public void CloseConnection()
        {
            Connection.Close();
        }

        /// <summary>
        /// Returns true if database connection is in an open state.
        /// </summary>
        /// <returns></returns>
        public bool IsConnectionOpen()
        {
            return Connection.State == ConnectionState.Open;
        }

        /// <summary>
        /// Creates a table on the remote database server corresponding to a given schema.
        /// </summary>
        /// <param name="schema">The schema defining the table to be created.</param>
        /// <param name="generateIdentity">If true, automatically generates an identity column.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public void CreateTable(DataTable schema, bool generateIdentity = true)
        {
            var columns = new List<string>();
            if (generateIdentity)
            {
                columns.Add(Driver.GetIdentityColumnSpecification());
            }
            for (var i = 0; i < schema.Columns.Count; i++)
            {
                var dbType = Driver.MapToDbType(schema.Columns[i].DataType.ToString(), schema.Columns[i].AllowDBNull);
                columns.Add(Driver.GetStandardColumnSpecification(schema.Columns[i].ColumnName, dbType));
            }

            using (var command = Connection.CreateCommand())
            {
                command.Connection = Connection;
                command.CommandText = Driver.BuildQueryCreateTable(schema.TableName, columns);

                try
                {
                    Log.Debug(String.Format("Dynamically creating database table '{0}'..", schema.TableName));
                    command.ExecuteNonQuery();
                }
                catch (DbException ex)
                {
                    Log.Error(String.Format("Could not create database table '{0}': {1}", schema.TableName, ex.Message));
                    throw;
                }
            }
        }

        /// <summary>
        /// Indicates whether a table with a given name already exists on the remote database.
        /// </summary>
        /// <param name="tableName">The name of the table to check.</param>
        /// <returns>True if a table with the given name already exists.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public bool ExistsTable(string tableName)
        {
            using (var command = Connection.CreateCommand())
            {
                command.Connection = Connection;
                command.CommandText = Driver.BuildQuerySelectTable(tableName);

                try
                {
                    var result = command.ExecuteScalar();
                    return result != null;
                }
                catch (DbException ex)
                {
                    Log.Error(String.Format("Could not query database for table '{0}': {1}", tableName, ex.Message));
                    throw;
                }
            }
        }

        /// <summary>
        /// Adds columns to an existing database table to include all columns present in a given schema.
        /// </summary>
        /// <param name="tableName">The name of the table to add columns to.</param>
        /// <param name="schema">The schema to use as a point of reference.</param>
        public void AddColumnsToTableToMatchSchema(string tableName, DataTable schema)
        {
            var existingDbTable = GetSchema(tableName);
            var numColumnsAdded = 0;

            // Insert any missing columns into the database.
            foreach (DataColumn column in schema.Columns)
            {
                // If a column with this name already exists, break out.
                if (existingDbTable.Columns.Contains(column.ColumnName))
                {
                    continue;
                }

                if (!column.AllowDBNull)
                {
                    throw new ArgumentException("Cannot add non-nullable columns to an existing database table!", "column");
                }

                var newColumn = new DataColumn(column.ColumnName)
                {
                    DataType = column.DataType,
                    DefaultValue = column.DefaultValue,
                    AllowDBNull = column.AllowDBNull
                };
                AddColumn(schema.TableName, newColumn);
                numColumnsAdded++;
            }

            // Log results.
            if (numColumnsAdded > 0)
            {
                Log.Debug(String.Format("Modified database table '{0}' to match schema. [{1} {2} added]", tableName, numColumnsAdded, "column".Pluralize(numColumnsAdded)));
            }
            else
            {
                Log.Debug(String.Format("All columns in schema are present in database table '{0}'.", tableName));
            }
        }

        /// <summary>
        /// Retrieves the schema for a remote database table.
        /// </summary>
        /// <param name="tableName">The name of the table to retrieve the schema for.</param>
        /// <returns>Schema for the given table.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public DataTable GetSchema(string tableName)
        {
            var table = new DataTable(tableName);
            using (var command = Connection.CreateCommand())
            {
                command.Connection = Connection;
                command.CommandText = Driver.BuildQueryColumnNamesAndTypes(tableName);
                IDataReader reader = null;
                try
                {
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var column = new DataColumn
                        {
                            ColumnName = reader["name"].ToString(),
                            DataType = Driver.MapToSystemType(reader["type"].ToString()),
                            AllowDBNull = Boolean.Parse(reader["nullable"].ToString())
                        };
                        table.Columns.Add(column);
                    }
                }
                catch (DbException ex)
                {
                    Log.Error(String.Format("Could not query database table '{0}': {1}", tableName, ex.Message));
                    throw;
                }
                finally
                {
                    if (reader != null)
                    {
                        reader.Close();
                    }
                }
            }

            return table;
        }

        /// <summary>
        /// Adds a column to a remote database table.
        /// </summary>
        /// <param name="dbTableName">The name of the table to add a column to.</param>
        /// <param name="column">The column to be added.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public void AddColumn(string dbTableName, DataColumn column)
        {
            using (var command = Connection.CreateCommand())
            {
                command.Connection = Connection;
                command.CommandText = Driver.BuildQueryAddColumnToTable(dbTableName, column);
                try
                {
                    Log.Debug(String.Format("Adding column '{0}' to table '{1}'..", column.ColumnName, dbTableName));
                    command.ExecuteNonQuery();
                }
                catch (DbException ex)
                {
                    Log.Error(String.Format("Could not add column '{0}' to table '{1}': {2}", column.ColumnName, dbTableName, ex.Message));
                    throw;
                }
            }
        }

        /// <summary>
        /// Indicates whether a column with a given name already exists in a database table.
        /// </summary>
        /// <param name="dbTableName">The table to check for existence of the column.</param>
        /// <param name="column">The column to check for a match.</param>
        /// <returns></returns>
        public bool ExistsColumn(string dbTableName, DataColumn column)
        {
            return GetSchema(dbTableName).Columns.Contains(column.ColumnName);
        }

        /// <summary>
        /// Inserts a row of data into a remote database.
        /// </summary>
        /// <param name="dbTableName">The name of the table to add the row to.</param>
        /// <param name="row">The row of data to add to the table.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public void InsertRow(string dbTableName, DataRow row)
        {
            using (var command = Connection.CreateCommand())
            {
                command.Connection = Connection;

                // Dynamically calculate list of column names and parameters based on table schema.
                ICollection<string> columnList = new List<string>();
                for (var i = 0; i < row.Table.Columns.Count; i++)
                {
                    columnList.Add(String.Format("\"{0}\"", row.Table.Columns[i].ColumnName));
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "@param" + i;
                    parameter.Value = row[i];
                    command.Parameters.Add(parameter);
                }

                // Construct insert statement.
                command.CommandText = Driver.BuildQueryInsertRow(dbTableName, columnList, command.Parameters);

                // Run it!
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (DbException ex)
                {
                    Log.Error(String.Format("Could not insert row into database table '{0}': {1}", dbTableName, ex.Message));
                    throw;
                }
            }
        }

        /// <summary>
        /// Creates an index on a given column of a selected table.
        /// </summary>
        /// <param name="tableName">The name of the table to create the index on.</param>
        /// <param name="columnName">The name of the column that will be indexed.</param>
        /// <param name="isClusteredIndex">Flag to indicate whether the index is clustered or not.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public void CreateIndexOnTable(string tableName, string columnName, string indexName)
        {
            using (var command = Connection.CreateCommand())
            {
                command.Connection = Connection;
                command.CommandText = Driver.BuildQueryIndex(tableName, columnName, indexName);
                try
                {
                    Log.Debug(String.Format("Creating index '{0}' on column '{1}'..", indexName, columnName));
                    command.ExecuteNonQuery();
                }
                catch (DbException ex)
                {
                    Log.Error(String.Format("Could not create index '{0}' on column '{1}': {2}", indexName, columnName, ex.Message));
                }
            }
        }

        /// <summary>
        /// Clusters a table on a given index.
        /// </summary>
        /// <param name="tableName">The name of the table that the command will be run on.</param>
        /// <param name="indexName">The name of the index to cluster on.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public void ClusterIndex(string tableName, string indexName)
        {
            using (var command = Connection.CreateCommand())
            {
                command.Connection = Connection;
                command.CommandText = Driver.BuildQueryClusterIndex(tableName, indexName);
                try
                {
                    Log.Debug(String.Format(@"Clustering table '{0}' on index '{1}'.", tableName, indexName));
                    command.ExecuteNonQuery();
                }
                catch (DbException ex)
                {
                    Log.Error(String.Format("Could not cluster on index '{0}' for table '{1}': {2}", indexName, tableName, ex.Message));
                }
            }
        }

        /// <summary>
        /// Gets all of the indexes on a given table.
        /// </summary>
        /// <param name="tableName">The name of the table to get the indexes from.</param>
        /// <returns>A collection of all of the indexes.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public IList<IndexInfo> GetIndexes(string tableName)
        {
            var indexList = new List<IndexInfo>();
            using (var command = Connection.CreateCommand())
            {
                command.Connection = Connection;
                command.CommandText = Driver.BuildQueryGetIndexes(tableName);
                IDataReader reader = null;
                try
                {
                    Log.Debug(String.Format("Gathering indexes for table '{0}'..", tableName));
                    
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var indexInfo = new IndexInfo();
                        indexInfo.IndexName = reader["index"].ToString();
                        indexInfo.IsClustered = Boolean.Parse(reader["isclustered"].ToString());
                        var columnNames = reader["column_name"].ToString().Split(',');
                        foreach (var columnName in columnNames)
                        {
                            indexInfo.IndexedColumns.Add(columnName);
                        }
                        indexList.Add(indexInfo);
                    }
                }
                catch (DbException ex)
                {
                    Log.Error(String.Format("Could not view indexes for table '{0}': {1}", tableName, ex.Message));
                }
                finally
                {
                    if (reader != null)
                    {
                        reader.Close();
                    }
                }
            }
            return indexList;
        }

        /// <summary>
        /// Drops a given index.
        /// </summary>
        /// <param name="indexName">The name of the index to drop.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public void DropIndex(string indexName)
        {
            using (var command = Connection.CreateCommand())
            {
                command.Connection = Connection;
                command.CommandText = Driver.BuildQueryDropIndex(indexName);
                try
                {
                    Log.Debug(String.Format("Dropping index '{0}'..", indexName));
                    command.ExecuteNonQuery();
                }
                catch (DbException ex)
                {
                    Log.Error(String.Format("Unable to drop index '{0}': {1}", indexName, ex));
                    throw;
                }
            }
        }

        /// <summary>
        /// Deletes rows from a table.
        /// </summary>
        /// <param name="tableName">The name of the table to drop rows from.</param>
        /// <param name="threshold">The interval for days to drop.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public void DeleteRowsOlderThan(string tableName, int threshold)
        {
            using (var command = Connection.CreateCommand())
            {
                command.Connection = Connection;
                command.CommandText = Driver.BuildQueryDeleteRows(tableName, threshold);
                try
                {
                    Log.Debug(String.Format("Dropping rows from {0} older than {1} {2}..", tableName, threshold.ToString(), "day".Pluralize(threshold)));
                }
                catch (DbException ex)
                {
                    Log.Error(String.Format("Unable to drop rows from table {0}: {1}", tableName, ex));
                    throw;
                }
            }
        }

        #endregion Public Methods

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
                Connection.Dispose();
            }
            disposed = true;
        }

        #endregion IDisposable Methods
    }

    #region Custom Exceptions

    /// <summary>
    /// Custom exception used to encapsulate the various database connection error conditions we may encounter.
    /// </summary>
    [Serializable]
    public class DbConnectionException : DbException
    {
        public DbConnectionException(string message)
            : base(message) { }

        protected DbConnectionException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }

    #endregion Custom Exceptions
}