using System.Collections.Generic;

namespace DataTableWriter.Helpers
{
    /// <summary>
    /// Helper class that stores the results of a database index query.
    /// </summary>
    public class IndexInfo
    {
        public string IndexName { get; set; }

        public IList<string> IndexedColumns { get; set; }

        public bool IsClustered { get; set; }

        public IndexInfo()
        {
            IndexedColumns = new List<string>();
        }
    }
}
