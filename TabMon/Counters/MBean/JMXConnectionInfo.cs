using System;
using JMXServiceURL = javax.management.remote.JMXServiceURL;

namespace TabMon.Counters.MBean
{
    /// <summary>
    /// Helper object to nicely encapsulate JMX server connection information and provide utility for retrieving properly-formatted service URLs.
    /// </summary>
    public class JmxConnectionInfo
    {
        public string Hostname { get; protected set; }
        public int Port { get; protected set; }

        public JmxConnectionInfo(string hostname, int port)
        {
            Hostname = hostname;
            Port = port;
        }

        #region Public Methods

        /// <summary>
        /// Retrieve the properly-formatted service URL string for a remote JMX server using the connection information maintained within this object.
        /// </summary>
        /// <returns>The formatted service URL of the remote JMX server.</returns>
        [CLSCompliant(false)]
        public JMXServiceURL GetJmxServiceUrl()
        {
            return new JMXServiceURL(String.Format("service:jmx:rmi:///jndi/rmi://{0}:{1}/jmxrmi", Hostname, Port));
        }

        public override string ToString()
        {
            return String.Format("{0}:{1}", Hostname, Port);
        }

        #endregion Public Methods
    }
}