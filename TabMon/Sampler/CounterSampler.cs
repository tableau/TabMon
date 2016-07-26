using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using TabMon.Counters;

namespace TabMon.Sampler
{
    /// <summary>
    /// Concrete class that handles sampling all counters in a collection and mapping the results to the desired schema.
    /// </summary>
    internal sealed class CounterSampler
    {
        private readonly ICollection<ICounter> counters;
        private readonly DataTable schema;
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public CounterSampler(ICollection<ICounter> counterCollection, string tableName)
        {
            counters = counterCollection;
            schema = GenerateSchema(tableName);
        }

        #region Public Methods

        /// <summary>
        /// Polls all known counters and maps the results to the dynamic data model.
        /// </summary>
        /// <returns>DataTable of all samples mapped to dynamic data model.</returns>
        public DataTable SampleAll()
        {
            Log.Info("Polling..");
            var pollTimestamp = DateTime.Now;

            // Create a new empty table to store results of this sampling, using our existing column schema.
            var dataTable = schema.Clone();

            // Sample all counters.
            foreach (var counter in counters)
            {
                // Retrieve sample for this counter.
                var counterSample = counter.Sample();

                // Bail out if we were unable to sample.
                if (counterSample == null) continue;

                // Map sample result to schema and insert it into result table.
                var row = MapToSchema(counterSample, dataTable);
                row["timestamp"] = pollTimestamp;
                dataTable.Rows.Add(row);
            }

            var numFailed = counters.Count - dataTable.Rows.Count;
            Log.Info(String.Format("Finished polling {0} {1}. [{2} {3}]", counters.Count, "counter".Pluralize(counters.Count), numFailed, "failure".Pluralize(numFailed)));

            return dataTable;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Generates a dynamic schema that can support all known counters.
        /// </summary>
        /// <param name="tableName">The name of the resulting data table.</param>
        /// <returns>DataTable that can accomodate both metadata and sample results of all counters managed by this sampler.</returns>
        private DataTable GenerateSchema(string tableName)
        {
            var generatedSchema = new DataTable(tableName);

            generatedSchema.Columns.Add(BuildColumnMetadata("timestamp", "System.DateTime", false));
            generatedSchema.Columns.Add(BuildColumnMetadata("cluster", "System.String", true, 32));
            generatedSchema.Columns.Add(BuildColumnMetadata("machine", "System.String", false, 16));
            generatedSchema.Columns.Add(BuildColumnMetadata("counter_type", "System.String", false, 32));
            generatedSchema.Columns.Add(BuildColumnMetadata("source", "System.String", false, 32));
            generatedSchema.Columns.Add(BuildColumnMetadata("category", "System.String", false, 64));
            foreach (var counter in counters)
            {
                if (!generatedSchema.Columns.Contains(counter.Counter.ToSnakeCase()))
                {
                    generatedSchema.Columns.Add(BuildColumnMetadata(counter.Counter, "System.Double", true));
                }
            }
            generatedSchema.Columns.Add(BuildColumnMetadata("instance", "System.String", true, 128));
            generatedSchema.Columns.Add(BuildColumnMetadata("unit", "System.String", true, 32));

            Log.Debug(String.Format("Dynamically built result schema '{0}'. [{1} {2}]",
                      tableName, generatedSchema.Columns.Count, "column".Pluralize(generatedSchema.Columns.Count)));
            return generatedSchema;
        }

        /// <summary>
        /// Builds a DataColumn for the given parameters.
        /// </summary>
        /// <param name="columnName">The name of the resulting column.</param>
        /// <param name="columnType">The type of data the column will contain.</param>
        /// <param name="isNullable">Whether or not this column supports nulls.</param>
        /// <param name="maxLength">The max length of this column.</param>
        /// <returns>DataColumn object with schema matching the given parameters.</returns>
        private static DataColumn BuildColumnMetadata(string columnName, string columnType, bool isNullable, int maxLength = -1)
        {
            var type = Type.GetType(columnType);
            if (type == null)
            {
                return null;
            }

            var column = new DataColumn(columnName.ToSnakeCase(), type)
                {
                    AllowDBNull = isNullable
                };
            if (maxLength >= 1)
            {
                column.MaxLength = maxLength;
            }

            return column;
        }

        /// <summary>
        /// Maps a counter's metadata and sampled value to the table schema.
        /// </summary>
        /// <param name="sample">The counter sample to map.</param>
        /// <param name="tableSchema">The DataTable to map the counter to.</param>
        /// <returns>DataRow with all fields from the sample correctly mapped.</returns>
        private static DataRow MapToSchema(ICounterSample sample, DataTable tableSchema)
        {
            var row = tableSchema.NewRow();

            var counter = sample.Counter;
            row["cluster"] = counter.Host.Cluster;
            row["machine"] = counter.Host.Name.ToLower();
            row["counter_type"] = counter.CounterType;
            row["source"] = counter.Source;
            row["category"] = counter.Category;
            row[counter.Counter.ToSnakeCase()] = sample.SampleValue;
            row["instance"] = counter.Instance;
            row["unit"] = counter.Unit;

            return row;
        }

        #endregion Private Methods
    }
}