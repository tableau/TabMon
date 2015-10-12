using System;
using System.Collections.Generic;
using DataTableWriter.Writers;
using TabMon.Helpers;

namespace TabMon
{
    /// <summary>
    /// Encapsulates runtime options for TabMonAgent.
    /// </summary>
    public class TabMonOptions
    {
        [CLSCompliant(false)]
        public IDataTableWriter Writer { get; set; }
        public ICollection<Host> Hosts { get; set; }
        public int PollInterval { get; set; }
        public string TableName { get; set; }
        private static TabMonOptions instance;
        private const int MinPollInterval = 1; // In seconds.

        #region Singleton Constructor/Accessor

        private TabMonOptions()
        {
            Hosts = new List<Host>();
        }

        public static TabMonOptions Instance
        {
            get { return instance ?? (instance = new TabMonOptions()); }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Indicates whether the current options configuration is valid or not.
        /// </summary>
        /// <returns>True if current options are all valid.</returns>
        public bool Valid()
        {
            return Hosts.Count > 0 && Writer != null && PollInterval >= MinPollInterval && TableName != null;
        }

        public override string ToString()
        {
            var writerName = "null";
            if (Writer != null)
            {
                writerName = Writer.Name;
            }

            return String.Format("[{0}='{1}', Writer='{2}', PollInterval='{3}', TableName='{4}']",
                                   "Host".Pluralize(Hosts.Count), String.Join(",", Hosts), writerName, PollInterval, TableName);
        }

        #endregion
    }
}