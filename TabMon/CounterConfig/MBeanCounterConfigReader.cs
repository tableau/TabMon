using log4net;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using TabMon.Counters;
using TabMon.Counters.MBean;
using TabMon.Helpers;

namespace TabMon.CounterConfig
{
    /// <summary>
    /// Handles parsing & loading the MBean counters defined within a Counters.config file.
    /// </summary>
    internal sealed class MBeanCounterConfigReader : ICounterConfigReader
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Parses & loads a set of MBean counters for the given host using the XML tree.
        /// </summary>
        /// <param name="root">The root MBean counters config node.</param>
        /// <param name="host">The host to load counters for.</param>
        /// <param name="lifeCycleTypeToLoad">A filter that indicates whether ephemeral or persistent counters should be loaded.</param>
        /// <returns>Collection of MBean counters specified in the root node's config.</returns>
        public ICollection<ICounter> LoadCounters(XmlNode root, Host host, CounterLifecycleType lifeCycleTypeToLoad)
        {
            var counters = new List<ICounter>();

            // We currently don't support ephemeral counters for MBeans, so just short circuit out in this case.
            if (lifeCycleTypeToLoad == CounterLifecycleType.Ephemeral)
            {
                return counters;
            }

            var sourceNodes = root.SelectNodes("./Source");
            if (sourceNodes != null)
            {
                foreach (XmlNode sourceNode in sourceNodes)
                {
                    ICollection<IMBeanClient> mbeanClientPool = null;
                    if (!host.SpecifyPorts)
                    {
                        var startPort = Convert.ToInt32(sourceNode.Attributes["startport"].Value);
                        var endPort = Convert.ToInt32(sourceNode.Attributes["endport"].Value);

                        // Retrieve a collection of all available clients within the specified port range, then new up counters using those.
                        // This way, multiple counters can share a single client & connection.
                        mbeanClientPool = MBeanClientFactory.CreateClients(host.Address, startPort, endPort);
                    }
                    else
                    {
                        var processName = sourceNode.Attributes["name"].Value;
                        if (host.Processes.ContainsKey(processName))
                        {
                            var ports = host.Processes[processName];

                            // Retrieve a collection of all available clients within the specified port range, then new up counters using those.
                            // This way, multiple counters can share a single client & connection.
                            mbeanClientPool = MBeanClientFactory.CreateClients(host.Address, ports);
                        }
                        else
                        {
                            Log.DebugFormat("Process \"{0}\" is not contained in host section. Continuing..", processName);
                        }
                    }

                    if (mbeanClientPool != null)
                    {
                        foreach (var mbeanClient in mbeanClientPool)
                        {
                            var countersForSource = BuildCountersForSourceNode(sourceNode, host, mbeanClient);
                            counters.AddRange(countersForSource);
                        }
                    }
                }
            }

            return counters;
        }

        /// <summary>
        /// Builds a collection of ICounter instances from a source XML node.
        /// </summary>
        private List<ICounter> BuildCountersForSourceNode(XmlNode sourceNode, Host host, IMBeanClient mbeanClient)
        {
            string sourceName = sourceNode.Attributes["name"].Value;
            var counterNodesForSource = sourceNode.SelectNodes(".//Counter");
            var counters = new List<ICounter>();

            if (counterNodesForSource != null)
            {
                foreach (XmlNode counterNode in counterNodesForSource)
                {
                    var counter = BuildCounterFromCounterNode(counterNode, mbeanClient, host, sourceName);
                    if (counter != null)
                    {
                        counters.Add(counter);
                    }
                }
            }

            return counters;
        }

        /// <summary>
        /// Builds an ICounter instance from a counter XML node and a set of properties.
        /// </summary>
        private static ICounter BuildCounterFromCounterNode(XmlNode counterNode, IMBeanClient mbeanClient, Host host, string sourceName)
        {
            var counterName = counterNode.Attributes["name"].Value;
            var categoryName = counterNode.ParentNode.Attributes["name"].Value;
            var path = counterNode.ParentNode.Attributes["path"].Value;

            string unitOfMeasurement = null;
            if (counterNode.Attributes.GetNamedItem("unit") != null)
            {
                unitOfMeasurement = counterNode.Attributes["unit"].Value;
            }

            var instanceName = BuildInstanceName(sourceName, mbeanClient.InstanceNumber);

            var rootCounterTypeNode = counterNode.ParentNode.ParentNode;
            var counterType = rootCounterTypeNode.Name.ToLower();

            string subDomain = null;
            if (rootCounterTypeNode.Attributes != null && rootCounterTypeNode.Attributes["subdomain"] != null)
            {
                subDomain = rootCounterTypeNode.Attributes["subdomain"].Value;
            }

            // Create the counter.
            try
            {
                var builder = new MBeanBuilder();
                return builder.CreateCounter(host)
                              .UsingClient(mbeanClient)
                              .WithSourceName(sourceName)
                              .WithSubDomain(subDomain)
                              .WithPath(path)
                              .WithCategoryName(categoryName)
                              .WithCounterName(counterName)
                              .WithInstanceName(instanceName)
                              .WithUnit(unitOfMeasurement)
                              .Build(counterType);
            }
            catch (Exception ex)
            {
                Log.DebugFormat(@"Failed to register MBean counter {0}\{1}\{2}\{3}\{4}: {5}",
                                host.Address, sourceName, categoryName, counterName, instanceName, ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Builds the instance name to match what Perfmon does (i.e. a machine with two dataserver processes will have instance names "dataserver" & "dataserver#1".
        /// </summary>
        private static string BuildInstanceName(string sourceName, int instanceNumber)
        {
            if (instanceNumber == 0)
            {
                return sourceName;
            }

            return String.Format("{0}#{1}", sourceName, instanceNumber);
        }
    }
}