using log4net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            var counters = new Collection<ICounter>();
            var sourceNodes = root.SelectNodes("./Source");

            foreach (XmlNode sourceNode in sourceNodes)
            {
                var sourceName = sourceNode.Attributes["name"].Value;
                var startPort = Convert.ToInt32(sourceNode.Attributes["startport"].Value);
                var endPort = Convert.ToInt32(sourceNode.Attributes["endport"].Value);

                // Validate port range.
                if (startPort > endPort || startPort <= 0 || startPort > 65535 || endPort <= 0 || endPort > 65535)
                {
                    throw new ArgumentException("Invalid port range");
                }

                // Retrieve a collection of all available clients within the specified port range, then new up counters using those.
                // This way, multiple counters can share a single client & connection.
                var mbeanClientPool = MBeanClientFactory.CreateClients(host.Name, startPort, endPort);
                foreach (var mbeanClient in mbeanClientPool)
                {
                    var counterNodesForSource = sourceNode.SelectNodes(".//Counter");
                    foreach (XmlNode counterNode in counterNodesForSource)
                    {
                        var counterName = counterNode.Attributes["name"].Value;

                        CounterLifecycleType configuredCounterLifecycleType = GetConfiguredLifecycleType(counterNode);
                        if (configuredCounterLifecycleType != lifeCycleTypeToLoad)
                        {
                            continue;
                        }

                        var categoryName = counterNode.ParentNode.Attributes["name"].Value;
                        var path = counterNode.ParentNode.Attributes["path"].Value;

                        string unitOfMeasurement = null;
                        if (counterNode.Attributes.GetNamedItem("unit") != null)
                        {
                            unitOfMeasurement = counterNode.Attributes["unit"].Value;
                        }

                        var instanceName = sourceName;

                        // Massage the instance name to match what Perfmon does (i.e. a machine with two dataserver processes will have
                        // instance names "dataserver" & "dataserver#1".
                        var processIndex = mbeanClient.Connector.ConnectionInfo.Port - startPort;
                        if (processIndex > 0)
                        {
                            instanceName += "#" + processIndex;
                        }

                        // Create the counter.
                        var counterType = counterNode.ParentNode.ParentNode.Name.ToLower();
                        try
                        {
                            var builder = new MBeanBuilder();
                            var counter = builder.CreateCounter(host)
                                .UsingClient(mbeanClient)
                                .WithSourceName(sourceName)
                                .WithPath(path)
                                .WithCategoryName(categoryName)
                                .WithCounterName(counterName)
                                .WithInstanceName(instanceName)
                                .WithUnit(unitOfMeasurement)
                                .Build(counterType);
                            counters.Add(counter);
                        }
                        catch (Exception ex)
                        {
                            Log.Debug(String.Format("Failed to register MBean counter {0}\\{1}\\{2}\\{3}\\{4}: {5}",
                                host.Name, sourceName, categoryName, counterName, instanceName, ex.Message));
                        }
                    }
                }
            }

            return counters;
        }

        private static CounterLifecycleType GetConfiguredLifecycleType(XmlNode counterNode)
        {
            if (counterNode.Attributes != null && counterNode.Attributes.GetNamedItem("ephemeral") != null)
            {
                bool isEphemeral;
                Boolean.TryParse(counterNode.Attributes["ephemeral"].Value.ToLowerInvariant(), out isEphemeral);
                if (isEphemeral)
                {
                    return CounterLifecycleType.Ephemeral;
                }
            }

            return CounterLifecycleType.Persistent;
        }
    }
}