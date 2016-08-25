using DataTableWriter.Connection;
using System;
using System.Collections.Generic;
using System.Data;

namespace DataTableWriter.Drivers
{
    /// <summary>
    /// Describes an interface for a database "driver".  This is basically a mapping between a discrete action that the DbAdapter needs and a particular SQL dialect.
    /// </summary>
    public interface IDbDriver
    {
        string Name { get; }

        IDbConnection BuildConnection(IDbConnectionInfo connectionInfo);
        Type MapToSystemType(string dbType);
        string MapToDbType(string systemType, bool allowDbNull);
        string GetIdentityColumnSpecification();
        string GetStandardColumnSpecification(string columnName, string columnType);
        string BuildQueryCreateTable(string tableName, ICollection<string> columns);
        string BuildQuerySelectTable(string tableName);
        string BuildQueryAddColumnToTable(string tableName, DataColumn column);
        string BuildQueryColumnNamesAndTypes(string tableName, bool excludeIdentityColumn = true);
        string BuildQueryInsertRow(string tableName, ICollection<string> columnList, IDataParameterCollection parameterList);
        string BuildQueryIndex(string tableName, string columnName, string indexName);
        string BuildQueryClusterIndex(string tableName, string indexName);
        string BuildQueryGetIndexes(string tableName);
        string BuildQueryDropIndex(string indexName);
        string BuildQueryDeleteRows(string tableName, int interval);
    }
}