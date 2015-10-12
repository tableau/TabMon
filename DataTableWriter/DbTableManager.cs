using DataTableWriter.Adapters;
using log4net;
using System;
using System.Data;
using System.Data.Common;
using System.Reflection;

namespace DataTableWriter
{
    /// <summary>
    /// Primary point of access to the functionalities of this library.  Provides ability to create dynamic tables on a remote server as well as update existing table schemas to match.
    /// </summary>
    internal static class DbTableManager
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region Public Methods

        /// <summary>
        /// Creates a table on a remote database server.
        /// </summary>
        /// <param name="adapter">Open adapter to a database.</param>
        /// <param name="schema">The schema of the table to create.</param>
        /// <returns>True if table is successfully created.</returns>
        public static bool CreateTable(IDbAdapter adapter, DataTable schema)
        {
            try
            {
                if (!adapter.ExistsTable(schema.TableName))
                {
                    adapter.CreateTable(schema);
                }
                else
                {
                    Log.Debug(String.Format("Database table '{0}' already exists; skipping creation.", schema.TableName));
                }
                return true;
            }
            catch (DbException ex)
            {
                Log.Error(String.Format("Failed to create database table '{0}': {1}", schema.TableName, ex.Message));
                return false;
            }
        }

        /// <summary>
        /// Updates an existing database table to 'match' a schema by adding any missing columns.
        /// </summary>
        /// <param name="adapter">Open adapter to a database.</param>
        /// <param name="schema">The schema to use to update the database.</param>
        /// <returns>True if table is successfully updated to match schema.</returns>
        public static bool UpdateTableToMatchSchema(IDbAdapter adapter, DataTable schema)
        {
            // Check if we need to update the db schema and/or this result schema.
            var dbTable = adapter.GetSchema(schema.TableName);
            if (!schema.SchemaEquals(dbTable))
            {
                try
                {
                    // Insert any missing columns into the database.
                    adapter.AddColumnsToTableToMatchSchema(dbTable.TableName, schema);
                    return true;
                }
                catch (DbException ex)
                {
                    Log.Error("Failed to initialize database writer: " + ex.Message);
                    return false;
                }
            }

            Log.Debug("Database table already matches schema; nothing to update.");
            return true;
        }

        /// <summary>
        /// Updates a schema to 'match' an existing database table by copying it.
        /// </summary>
        /// <param name="adapter">Open adapter to a database.</param>
        /// <param name="schema">The schema to update.</param>
        /// <returns></returns>
        public static bool UpdateSchemaToMatchTable(IDbAdapter adapter, DataTable schema)
        {
            // Copy the DB table's schema back over the top of our in-memory schema to ensure parity.
            try
            {
                // Check to see if overwriting the schema will yield inconsistencies.
                var existingTableSchema = adapter.GetSchema(schema.TableName);
                foreach (DataColumn column in schema.Columns)
                {
                    if (!existingTableSchema.Columns.Contains(column.ColumnName))
                    {
                        Log.Error(String.Format("Cannot update local schema to match database table; column '{0}' exists in the schema, but not the database table.", column.ColumnName));
                        return false;
                    }
                    if (existingTableSchema.Columns[column.ColumnName].DataType != column.DataType)
                    {
                        Log.Error(String.Format("Cannot update local schema to match database table; data types are inconsistent. [Schema='{0}', DbTable='{1}']", column.DataType, existingTableSchema.Columns[column.ColumnName].DataType));
                        return false;
                    }
                    if (existingTableSchema.Columns[column.ColumnName].AllowDBNull != column.AllowDBNull)
                    {
                        Log.Error(String.Format("Cannot update local schema to match database table; inconsistencies in 'AllowDBNull' parameter. [Schema='{0}', DbTable='{1}']", column.AllowDBNull, existingTableSchema.Columns[column.ColumnName].AllowDBNull));
                        return false;
                    }
                }

                // Copy the remote database's schema over the top of our in-memory schema.
                schema = existingTableSchema.Copy();
                Log.Debug(String.Format("Updated internal schema to match database table '{0}'.", schema.TableName));
                return true;
            }
            catch (DbException ex)
            {
                Log.Error(String.Format("Error updating schema '{0}' from database: {1}", schema.TableName, ex.Message));
                return false;
            }
        }

        /// <summary>
        /// Uses an in-memory schema to initialize a database table according to a set of initialization options.
        /// </summary>
        /// <param name="adapter">Open adapter to a database.</param>
        /// <param name="schema">The schema to use to initialize.</param>
        /// <param name="tableInitializationOptions">A set of options to determine initialization behavior.</param>
        public static void InitializeTable(IDbAdapter adapter, DataTable schema, DbTableInitializationOptions tableInitializationOptions)
        {
            if (tableInitializationOptions.CreateTableDynamically)
            {
                CreateTable(adapter, schema);
            }
            if (tableInitializationOptions.UpdateDbTableToMatchSchema)
            {
                UpdateTableToMatchSchema(adapter, schema);
            }
            if (tableInitializationOptions.UpdateSchemaToMatchDbTable)
            {
                UpdateSchemaToMatchTable(adapter, schema);
            }
        }

        #endregion
    }
}