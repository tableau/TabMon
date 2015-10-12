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
    }
}