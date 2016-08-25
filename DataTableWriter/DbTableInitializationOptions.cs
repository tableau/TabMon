using System.Collections.Generic;

namespace DataTableWriter
{
    /// <summary>
    /// Represents a configuration of database table initialization behaviors.
    /// </summary>
    public struct DbTableInitializationOptions
    {
        // Create the table dynamically.
        public bool CreateTableDynamically;

        // Update the database table to contain all of the columns present in the in-memory schema.
        public bool UpdateDbTableToMatchSchema;

        // Update the in-memory schema to match the database table by copying it.
        public bool UpdateSchemaToMatchDbTable;

        // Update the indexes in the database.
        public bool UpdateIndexes;

        // Collection of indexes to generate. The bool is whether or not this is a clustered index.
        public IDictionary<string, bool> IndexesToGenerate;

        // Determine whether to delete data older than a specific threshold.
        public bool PurgeData;

        // Threshold that determines when data should be purged.
        public int PurgeDataThreshold;
    }
}