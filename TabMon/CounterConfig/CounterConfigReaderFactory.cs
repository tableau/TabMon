using log4net;
using System;
using System.Reflection;

namespace TabMon.CounterConfig
{
    /// <summary>
    /// Handles creation of instances of ICounterConfigReader.
    /// </summary>
    internal static class CounterConfigReaderFactory
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Given the name of a config reader, news up an instance of the appropriate type.
        /// </summary>
        /// <param name="configReaderType">The name of the config reader to instantiate.</param>
        /// <returns>ICounterConfigReader object of the appropriate type,.  Null if no match is found.</returns>
        static public ICounterConfigReader CreateConfigReader(string configReaderType)
        {
            ICounterConfigReader reader = null;

            switch (configReaderType.ToLower())
            {
                case "perfmon":
                    reader = new PerfmonCounterConfigReader();
                    break;

                case "mbean":
                    reader = new MBeanCounterConfigReader();
                    break;

                default:
                    Log.Error(String.Format("Invalid counter type '{0}' encountered in configuration.", configReaderType));
                    break;
            }

            return reader;
        }
    }
}