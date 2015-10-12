using System;
using System.Data;

namespace DataTableWriter.Writers
{
    /// <summary>
    /// Interface describing an object capable of writing out DataTable objects.
    /// </summary>
    public interface IDataTableWriter : IDisposable
    {
        string Name { get; }

        void Write(DataTable table);
    }
}