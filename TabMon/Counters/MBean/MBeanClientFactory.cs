using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TabMon.Helpers;

namespace TabMon.Counters.MBean
{
    /// <summary>
    /// Factory class that produces instances of MBeanClient objects for a given hostname and port range.
    /// </summary>
    public static class MBeanClientFactory
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Creates instances of MBeanClient objects for all open JMX ports on a host within a given port range.
        /// This is accomplished by scanning the range starting from the bottom and stopping when a closed port is encountered, to maintain parity with how Tableau exposes JMX ports.
        /// </summary>
        /// <param name="hostname">The hostname of the remote host to generate clients for.</param>
        /// <param name="startPort">The start of the port range to scan.</param>
        /// <param name="endPort">The end of the port range to scan.</param>
        /// <returns>Collection of MBeanClients for open ports within the given range.</returns>
        public static ICollection<IMBeanClient> CreateClients(string hostname, int startPort, int endPort)
        {
            // Validate port range.
            if (!IsValidPortRange(startPort, endPort))
            {
                throw new ArgumentException("Invalid port range");
            }

            Log.Debug(String.Format("Scanning JMX ports {0}-{1} on {2}..", startPort, endPort, hostname));
            ICollection<IMBeanClient> validClients = new List<IMBeanClient>();

            for (var currentPort = startPort; currentPort <= endPort; currentPort++)
            {
                try
                {
                    validClients.Add(CreateMBeanClient(hostname, currentPort, currentPort-startPort));
                    Log.Debug(String.Format("Created JMX client for {0}:{1}", hostname, currentPort));
                }
                catch (Exception)
                {
                    Log.Debug(String.Format("Encountered closed JMX port ({0}), stopping scan.", currentPort));
                    break;
                }
            }

            return validClients;
        }

        /// <summary>
        /// Creates instances of MBeanClient objects for all open JMX ports on a host within a given port range.
        /// This is accomplished by scanning the range starting from the bottom and stopping when a closed port is encountered, to maintain parity with how Tableau exposes JMX ports.
        /// </summary>
        /// <param name="hostname">The hostname of the remote host to generate clients for.</param>
        /// <param name="startPort">The start of the port range to scan.</param>
        /// <param name="endPort">The end of the port range to scan.</param>
        /// <returns>Collection of MBeanClients for open ports within the given range.</returns>
        public static ICollection<IMBeanClient> CreateClients(string hostname, List<Process> ports)
        {
            Log.Debug(String.Format("Scanning JMX port(s) \"{0}\" on {1}..", string.Join(", ", ports.Select(process => process.PortNumber).ToList()), hostname));
            ICollection<IMBeanClient> validClients = new List<IMBeanClient>();

            foreach (Process port in ports)
            {
                // Validate ports.
                if (!IsValidPort(port.PortNumber))
                {
                    throw new ArgumentException("Invalid port range.");
                }

                try
                {
                    validClients.Add(CreateMBeanClient(hostname, port.PortNumber, port.ProcessNumber));
                    Log.Debug(String.Format("Created JMX client for {0}:{1}.", hostname, port.PortNumber));
                }
                catch (Exception)
                {
                    Log.Debug(String.Format("Encountered closed JMX port ({0}), stopping scan.", port.PortNumber));
                }
            }

            return validClients;
        }

        private static IMBeanClient CreateMBeanClient(string hostname, int port, int instanceNumber)
        {
            var connectionInfo = new JmxConnectionInfo(hostname, port);
            var connector = new JmxConnectorProxy(connectionInfo);

            connector.OpenConnection();
            return new MBeanClient(connector, instanceNumber);
        }

        private static bool IsValidPortRange(int startPort, int endPort)
        {
            return IsValidPort(startPort) && IsValidPort(endPort) && startPort <= endPort;
        }

        private static bool IsValidPort(int port)
        {
            return port > 0 && port <= 65535;
        }
    }
}