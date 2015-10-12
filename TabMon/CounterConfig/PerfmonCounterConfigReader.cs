using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml;
using TabMon.Counters;
using TabMon.Counters.Perfmon;
using TabMon.Helpers;

namespace TabMon.CounterConfig
{
    /// <summary>
    /// Handles parsing & loading the Perfmon counters defined within a Counters.config file.
    /// </summary>
    internal sealed class PerfmonCounterConfigReader : ICounterConfigReader
    {
        /// <summary>
        /// Parses & loads a set of Perfmon counters for the given host using the XML tree.
        /// </summary>
        /// <param name="root">The root Perfmon counters config node.</param>
        /// <param name="host">The host to load counters for.</param>
        /// <returns>Collection of Perfmon counters specified in the root node's config.</returns>
        public ICollection<ICounter> LoadCounters(XmlNode root, Host host)
        {
            var counters = new Collection<ICounter>();
            var perfmonCounterNodes = root.SelectNodes("./*/Counter");

            foreach (XmlNode counterNode in perfmonCounterNodes)
            {
                // Set what we know.
                var counterName = counterNode.Attributes["name"].Value;
                var categoryName = counterNode.ParentNode.Attributes["name"].Value;
                string unitOfMeasurement = null;
                if (counterNode.Attributes.GetNamedItem("unit") != null)
                {
                    unitOfMeasurement = counterNode.Attributes["unit"].Value;
                }

                // If any instance names are called out, shove them into a list of filters.
                IList<string> instanceNameFilters = new List<string>();
                if (counterNode.HasChildNodes)
                {
                    var instanceNodes = counterNode.SelectNodes("./Instance");
                    
                    foreach (var instanceNode in instanceNodes.Cast<XmlNode>().Where(instanceNode => instanceNode.Attributes.GetNamedItem("name") != null))
                    {
                        instanceNameFilters.Add(instanceNode.Attributes["name"].Value);
                    }
                }

                // Load an instance of this perfmon counter for each matching instance name that exists.
                // If no instance name is specified, just load them all.
                var counterInstances = PerfmonCounterLoader.LoadInstancesForCounter(host, categoryName, counterName, unitOfMeasurement, instanceNameFilters);
                foreach (var counterInstance in counterInstances)
                {
                    counters.Add(counterInstance);
                }
            }

            return counters;
        }
    }
}