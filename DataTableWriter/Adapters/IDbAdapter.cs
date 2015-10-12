using DataTableWriter.Connection;
using DataTableWriter.Drivers;
using System;
using System.Data;

namespace DataTableWriter.Adapters
{
    /// <summary>
    /// An interface describing a database adapter.
    /// </summary>
    public interface IDbAdapter : IDisposable
    {
        IDbDriver Driver { get; }
        IDbConnection Connection { get; }
        IDbConnectionInfo ConnectionInfo { get; }

        void OpenConnection();
        bool IsConnectionOpen();
        void CreateTable(DataTable schema, bool generateIdentity = true);
        bool ExistsTable(string tableName);
        DataTable GetSchema(string tableName);
        void AddColumn(string tableName, DataColumn column);
        void AddColumnsToTableToMatchSchema(string tableName, DataTable schema);
        bool ExistsColumn(string tableName, DataColumn column);
        void InsertRow(string tableName, DataRow row);
    }
}