using DataTableWriter.Connection;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DataTableWriter.Drivers
{
    /// <summary>
    /// Represents a mapping between Postgres-dialect SQL and the assorted actions the DbAdapter needs in order to accomplish its tasks.
    /// </summary>
    internal class PostgresDriver : IDbDriver
    {
        // Map of Postgres types -> System types.
        protected readonly IReadOnlyDictionary<string, string> postgresToSystemTypeMap
            = new Dictionary<string, string>()
            {
                { "bigint", "System.Int64" },
                { "bigserial", "System.Int64" },
                { "boolean", "System.Boolean" },
                { "character", "System.String" },
                { "double", "System.Double" },
                { "integer", "System.Int32" },
                { "numeric", "System.Decimal" },
                { "serial", "System.Int32" },
                { "smallint", "System.Int16" },
                { "smallserial", "System.Int16" },
                { "text", "System.String" },
                { "timestamp", "System.DateTime" }
            };

        // Map of System types -> Postgres types.
        protected readonly IReadOnlyDictionary<string, string> systemToPostgresTypeMap
            = new Dictionary<string, string>()
            {
                { "System.Boolean", "boolean" },
                { "System.Byte", "smallint" },
                { "System.Char", "character(1)" },
                { "System.DateTime", "timestamp" },
                { "System.Decimal", "numeric" },
                { "System.Double", "double precision" },
                { "System.Int16", "smallint" },
                { "System.Int32", "integer" },
                { "System.Int64", "bigint" },
                { "System.Single", "float8" },
                { "System.String", "text" },
            };

        public string Name
        {
            get
            {
                return "Postgres Driver";
            }
        }

        #region Public Methods

        /// <summary>
        /// Builds a Postgres connection object for the given remote Postgres server.
        /// </summary>
        /// <param name="connectionInfo">The database connection information.</param>
        /// <returns>Connection object for the remote Postgres server.</returns>
        public IDbConnection BuildConnection(IDbConnectionInfo connectionInfo)
        {
            if (connectionInfo.Port == null)
            {
                throw new ArgumentException("Port cannot be null!");
            }

            var connectionString =
                new NpgsqlConnectionStringBuilder()
                {
                    Host = connectionInfo.Server,
                    Port = connectionInfo.Port.Value,
                    UserName = connectionInfo.Username,
                    Password = connectionInfo.Password,
                    Database = connectionInfo.DatabaseName
                };

            return new NpgsqlConnection(connectionString);
        }

        /// <summary>
        /// Maps the name of a Postgres type to a C# System type.
        /// </summary>
        /// <param name="pgType">The name of the Postgres type.</param>
        /// <returns>System type that correlates to the given Postgres type.</returns>
        public Type MapToSystemType(string pgType)
        {
            // We only take the first word of the postgres type into consideration, in order to simplify our mapping.
            var pgFirstTermOfType = pgType.Split(' ')[0].ToLower();
            if (!postgresToSystemTypeMap.ContainsKey(pgFirstTermOfType))
            {
                return Type.GetType("System.String");
            }

            return Type.GetType(postgresToSystemTypeMap[pgFirstTermOfType]);
        }

        /// <summary>
        /// Maps the name of a C# System type to a Postgres type.
        /// </summary>
        /// <param name="systemType">The name of the System type.</param>
        /// <param name="allowDbNull">Flag indicating whether the input type is nullable.</param>
        /// <returns>Postgres type that correlates to the given System type.</returns>
        public string MapToDbType(string systemType, bool allowDbNull)
        {
            string pgType;
            if (!systemToPostgresTypeMap.ContainsKey(systemType))
            {
                pgType = "text";
            }
            else
            {
                pgType = systemToPostgresTypeMap[systemType];
            }

            if (!allowDbNull)
            {
                pgType = String.Format("{0} NOT NULL", pgType);
            }

            return pgType;
        }

        /// <summary>
        /// Generates the Postgres dialect expression of a default value.
        /// </summary>
        /// <param name="defaultValue">The default value to assign.</param>
        /// <returns>Postgres expression of a default value statement.</returns>
        public string GetDefaultValueClause(object defaultValue)
        {
            return String.Format("DEFAULT '{0}'", defaultValue);
        }

        /// <summary>
        /// Generates the Postgres dialect expression of an identity column specification.
        /// </summary>
        /// <returns>Postgres expression of an identity column specification.</returns>
        public string GetIdentityColumnSpecification()
        {
            return "id serial PRIMARY KEY";
        }

        /// <summary>
        /// Generates the Postgres dialect expression of a standard column specification.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        /// <param name="columnType">The data type of the column.</param>
        /// <returns>Postgres expression of a column with the given name & type.</returns>
        public string GetStandardColumnSpecification(string columnName, string columnType)
        {
            return String.Format("\"{0}\" {1}", columnName, columnType);
        }

        /// <summary>
        /// Builds a table creation statement in Postgres DDL.
        /// </summary>
        /// <param name="tableName">The name of the table to create.</param>
        /// <param name="columns">The names of the columns to include in the table.</param>
        /// <returns>Postgres DDL for creation of a table with the given name & columns.</returns>
        public string BuildQueryCreateTable(string tableName, ICollection<string> columns)
        {
            return String.Format(@"CREATE TABLE ""{0}""({1});", tableName, String.Join(",\n", columns));
        }

        /// <summary>
        /// Builds a Postgres table selection statement.
        /// </summary>
        /// <param name="tableName">The name of the table to select.</param>
        /// <returns>Postgres select statement for the given table.</returns>
        public string BuildQuerySelectTable(string tableName)
        {
            return String.Format("SELECT * FROM pg_tables WHERE tablename='{0}'", tableName);
        }

        /// <summary>
        /// Builds a Postgres alter statement to add a column to a table.
        /// </summary>
        /// <param name="tableName">The name of the table to add the column to.</param>
        /// <param name="column">The column to add.</param>
        /// <returns>Postgres alter statement to add the column.</returns>
        public string BuildQueryAddColumnToTable(string tableName, DataColumn column)
        {
            var defaultClause = "";
            if (!column.AllowDBNull)
            {
                defaultClause = GetDefaultValueClause(column.DefaultValue);
            }

            return String.Format(@"ALTER TABLE ""{0}"" ADD COLUMN {1} {2};", tableName, GetStandardColumnSpecification(column.ColumnName, MapToDbType(column.DataType.ToString(), column.AllowDBNull)), defaultClause);
        }

        /// <summary>
        /// Builds a Postgres query to retrieve the names and types of all columns in a table.
        /// </summary>
        /// <param name="tableName">The name of the table to retrieve information about.</param>
        /// <param name="excludeIdentityColumn">Flag indicating whether the statement should exclude the ID column.</param>
        /// <returns>Postgres query that will retrieve the names and types of all columns in the designated table.</returns>
        public string BuildQueryColumnNamesAndTypes(string tableName, bool excludeIdentityColumn = true)
        {
            var excludeIdentityColumnClause = excludeIdentityColumn ? "and a.attname != 'id'" : "";
            return String.Format(@"SELECT a.attname AS name, format_type(a.atttypid, a.atttypmod) AS type, NOT(a.attnotnull) AS nullable
                                   FROM pg_attribute a JOIN pg_class b ON (a.attrelid = b.relfilenode)
                                   WHERE b.relname = '{0}' AND a.attstattarget = -1 {1};", tableName, excludeIdentityColumnClause);
        }

        /// <summary>
        /// Builds a Postgres query to insert a row of data into a table.
        /// </summary>
        /// <param name="tableName">The name of the table to insert data into.</param>
        /// <param name="columnList">The list of columns that make up the row.</param>
        /// <param name="parameterList">A collection of parameters to insert.</param>
        /// <returns>Postgres insert statement to insert the given data into the designated table.</returns>
        public string BuildQueryInsertRow(string tableName, ICollection<string> columnList, IDataParameterCollection parameterList)
        {
            var columns = String.Join(",", columnList);
            ICollection<string> paramNameList = new List<string>();
            foreach (IDbDataParameter parameter in parameterList)
            {
                paramNameList.Add(parameter.ParameterName);
            }
            var paramNames = String.Join(",", paramNameList);
            return String.Format(@"INSERT INTO ""{0}"" ({1}) VALUES ({2});", tableName, columns, paramNames);
        }

        /// <summary>
        /// Builds a query to create an index.
        /// </summary>
        /// <param name="tableName">The name of the table to create the index on.</param>
        /// <param name="columnName">The name of the column that will be indexed.</param>
        /// <returns>Postgres statement to create an index.</returns>
        public string BuildQueryIndex(string tableName, string columnName, string indexName)
        {
            return String.Format(@"CREATE INDEX ""{0}"" ON ""{1}"" (""{2}"");", indexName, tableName, columnName);
        }

        /// <summary>
        /// Builds a query to Cluster an index.
        /// </summary>
        /// <param name="tableName">The name of the table to alter.</param>
        /// <param name="indexName">The name of the index to cluster on.</param>
        /// <returns>Postgres statement to cluster on a specific index.</returns>
        public string BuildQueryClusterIndex(string tableName, string indexName)
        {
            return String.Format(@"ALTER TABLE ""{0}"" CLUSTER ON ""{1}"";", tableName, indexName);
        }

        /// <summary>
        /// Builds a query to find all of the indexes on a given table.
        /// </summary>
        /// <param name="tablename">The name of the table that the indexes are associated with.</param>
        /// <returns>Postgres statement that returns all indexes for a given table.</returns>
        public string BuildQueryGetIndexes(string tableName)
        {
            return String.Format(@"SELECT c.relname as ""index"", array_to_string(array_agg(a.attname), ', ') as ""column_name"", i.indisclustered as ""isclustered"" 
                                    FROM pg_catalog.pg_class c JOIN pg_catalog.pg_namespace n ON n.oid = c.relnamespace JOIN pg_catalog.pg_index i ON i.indexrelid = c.oid 
                                    JOIN pg_catalog.pg_class t ON i.indrelid = t.oid JOIN pg_catalog.pg_attribute a ON a.attrelid = t.oid WHERE c.relkind = 'i' 
                                    and n.nspname not in ('pg_catalog', 'pg_toast') and pg_catalog.pg_table_is_visible(c.oid) and t.relname = '{0}' 
                                    and c.relname not like '%pkey' and a.attnum = ANY(i.indkey) GROUP BY c.relname, i.indisclustered;", tableName);
        }

        /// <summary>
        /// Builds a query to drop an index.
        /// </summary>
        /// <param name="indexName">The name of the index to drop.</param>
        /// <returns>Postgres statement to drop a given index.</returns>
        public string BuildQueryDropIndex(string indexName)
        {
            return String.Format(@"DROP INDEX ""{0}"";", indexName);
        }

        /// <summary>
        /// Builds a query to delete rows older than X amount of days.
        /// </summary>
        /// <param name="tableName">The name of the table to drop rows from.</param>
        /// <param name="interval">The interval for days to drop.</param>
        /// <returns>Postgres statement to drop rows older than X amount of days.</returns>
        public string BuildQueryDeleteRows(string tableName, int interval)
        {
            return String.Format(@"DELETE FROM {0} WHERE timestamp < NOW() - INTERVAL '{1} {2}';", tableName, interval.ToString(), "day".Pluralize(interval));
        }

        #endregion
    }
}