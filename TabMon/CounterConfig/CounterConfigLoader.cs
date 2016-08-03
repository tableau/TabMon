using log4net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using TabMon.Counters;
using TabMon.Helpers;

namespace TabMon.CounterConfig
{
    /// <summary>
    /// Validates & parses the Counters.config file into a collection of ICounter objects.
    /// </summary>
    internal static class CounterConfigLoader
    {
        private const string PathToCountersConfig = @"Config\Counters.config";
        private const string PathToSchema = @"Resources\CountersConfig.xsd";
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static XmlDocument loadedConfigDocument; 

        /// <summary>
        /// Loads the Counters.config file, validates it against the XSD schema, and news up the appropriate CounterConfigReader object for each root counter type node.
        /// </summary>
        /// <param name="hosts">The target hosts to load counters for.</param>
        /// <param name="counterLifecycleType">The type of counter to load; e.g. ephemeral or persistent.</param>
        /// <returns>Collection of all valid counters in Counters.config across the set of given hosts.</returns>
        public static ICollection<ICounter> Load(IEnumerable<Host> hosts, CounterLifecycleType counterLifecycleType)
        {
            Log.DebugFormat(@"Loading {0} performance counters from {1}..", counterLifecycleType.ToString().ToLowerInvariant(), Path.Combine(Directory.GetCurrentDirectory(), PathToCountersConfig));

            var counters = new Collection<ICounter>();

            if (loadedConfigDocument == null)
            {
                loadedConfigDocument = LoadConfig();
            }

            // Set the root element & begin loading counters.
            var documentRoot = loadedConfigDocument.DocumentElement.SelectSingleNode("/Counters");
            var counterRootNodes = documentRoot.SelectNodes("child::*");
            foreach (XmlNode counterRootNode in counterRootNodes)
            {
                var counterType = counterRootNode.Name;
                Log.DebugFormat("Loading {0} counters..", counterType);
                var configReader = CounterConfigReaderFactory.CreateConfigReader(counterType);

                foreach (var host in hosts)
                {
                    var countersInNode = configReader.LoadCounters(counterRootNode, host, counterLifecycleType);
                    Log.DebugFormat("Loaded {0} {1} {2} {3} on {4}.",
                                    countersInNode.Count, counterLifecycleType.ToString().ToLowerInvariant(), counterType, "counter".Pluralize(countersInNode.Count), host.Name);
                    counters.AddRange(countersInNode);
                }
            }

            Log.DebugFormat("Successfully loaded {0} {1} from configuration file.", counters.Count, "counter".Pluralize(counters.Count));
            return counters;
        }

        private static XmlDocument LoadConfig()
        {
            Log.DebugFormat(@"Loading performance counters from {0}..", Path.Combine(Directory.GetCurrentDirectory(), PathToCountersConfig));

            var settings = new XmlReaderSettings
            {
                ValidationType = ValidationType.Schema
            };

            var doc = new XmlDocument();
            try
            {
                settings.Schemas.Add("", PathToSchema);
                var reader = XmlReader.Create(PathToCountersConfig, settings);
                doc.Load(reader);
            }
            catch (FileNotFoundException ex)
            {
                throw new ConfigurationErrorsException(String.Format("Could not find file '{0}'.", ex.Message));
            }
            catch (XmlException ex)
            {
                throw new ConfigurationErrorsException(String.Format("Malformed XML in' {0}': {1}", PathToCountersConfig,
                    ex.Message));
            }
            catch (XmlSchemaValidationException ex)
            {
                throw new ConfigurationErrorsException(String.Format("Failed to validate '{0}': {1} (Line {2})",
                    PathToCountersConfig, ex.Message, ex.LineNumber));
            }
            Log.Debug(String.Format("Successfully validated '{0}' against '{1}'.", PathToCountersConfig, PathToSchema));
            return doc;
        }
    }
}