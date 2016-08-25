using log4net;
using System;
using System.Collections.Generic;
using System.Reflection;

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
                var connectionInfo = new JmxConnectionInfo(hostname, currentPort);
                var connector = new JmxConnectorProxy(connectionInfo);

                try
                {
                    connector.OpenConnection();
                    IMBeanClient client = new MBeanClient(connector);
                    validClients.Add(client);
                    Log.Debug(String.Format("Created JMX client for {0}", connectionInfo));
                }
                catch (Exception)
                {
                    Log.Debug(String.Format("Encountered closed JMX port ({0}), stopping scan.", currentPort));
                    break;
                }
            }

            return validClients;
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