using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using TabMon.CounterConfig;
using TabMon.Counters;
using TabMon.Helpers;

namespace TabMon.Sampler
{
    /// <summary>
    /// Concrete class that handles sampling all counters in a collection and mapping the results to the desired schema.
    /// </summary>
    internal sealed class CounterSampler
    {
        private readonly ICollection<ICounter> persistentCounters;
        private readonly ICollection<Host> hostsToSample;
        private readonly string tableName;
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public CounterSampler(ICollection<Host> hostsToSample, string tableName)
        {
            this.hostsToSample = hostsToSample;
            this.tableName = tableName;
            persistentCounters = CounterConfigLoader.Load(hostsToSample, CounterLifecycleType.Persistent);
            Log.InfoFormat("Successfully loaded {0} persistent {1} from configuration file.", persistentCounters.Count, "counter".Pluralize(persistentCounters.Count));
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

            // Load any "ephemeral" counters which may have been instantiated between polling cycles.
            var ephemeralCounters = CounterConfigLoader.Load(hostsToSample, CounterLifecycleType.Ephemeral);
            Log.DebugFormat("Successfully loaded {0} ephemeral {1}.", ephemeralCounters.Count, "counter".Pluralize(ephemeralCounters.Count));
            var allCounters = persistentCounters.Concat(ephemeralCounters).ToList();

            // Sample all persistent counters.
            DataTable dataTable = SampleCounters(allCounters, pollTimestamp);

            var numFailed = allCounters.Count - dataTable.Rows.Count;
            Log.InfoFormat("Finished polling {0} {1}. [{2} {3}]", allCounters.Count, "counter".Pluralize(allCounters.Count), numFailed, "failure".Pluralize(numFailed));

            return dataTable;
        }

        #endregion Public Methods

        #region Private Methods

        private DataTable SampleCounters(ICollection<ICounter> counters, DateTime pollTimestamp)
        {
            // Create a new empty table to store results of this sampling.
            var dataTable = GenerateSchema(counters);

            foreach (var counter in counters)
            {
                // Retrieve sample for this counter.
                var counterSample = counter.Sample();

                if (counterSample != null)
                {
                    // Map sample result to schema and insert it into result table.
                    var row = MapToSchema(counterSample, dataTable, pollTimestamp);
                    dataTable.Rows.Add(row);
                }
            }

            return dataTable;
        }

        /// <summary>
        /// Generates a dynamic schema that can support all known counters.
        /// </summary>
        /// <param name="counters">The counters that the data table will need to be able to accomodate.</param>
        /// <returns>DataTable that can accomodate both metadata and sample results of all input counters.</returns>
        private DataTable GenerateSchema(ICollection<ICounter> counters)
        {
            var generatedSchema = new DataTable(tableName);

            generatedSchema.Columns.Add(BuildColumnMetadata("timestamp", "System.DateTime", false));
            generatedSchema.Columns.Add(BuildColumnMetadata("cluster", "System.String", true, 32));
            generatedSchema.Columns.Add(BuildColumnMetadata("machine", "System.String", false, 63));
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

            Log.DebugFormat("Dynamically built result schema '{0}'. [{1} {2}]",
                            tableName, generatedSchema.Columns.Count, "column".Pluralize(generatedSchema.Columns.Count));
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

            var column = new DataColumn(columnName.ToSnakeCase(), type) { AllowDBNull = isNullable };
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
        /// <param name="pollTimestamp">The timestamp that the poll occurred at.</param>
        /// <returns>DataRow with all fields from the sample correctly mapped.</returns>
        private static DataRow MapToSchema(ICounterSample sample, DataTable tableSchema, DateTime pollTimestamp)
        {
            var row = tableSchema.NewRow();

            var counter = sample.Counter;
            row["timestamp"] = pollTimestamp;
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