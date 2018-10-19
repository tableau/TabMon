using log4net;
using System;
using System.Reflection;
using JMXConnector = javax.management.remote.JMXConnector;
using JMXConnectorFactory = javax.management.remote.JMXConnectorFactory;
using MBeanServerConnection = javax.management.MBeanServerConnection;

namespace TabMon.Counters.MBean
{
    /// <summary>
    /// This class serves as a proxy for the Java JMXConnector class.  It adds in reconnection logic to handle broken-pipe scenarios.
    /// </summary>
    public sealed class JmxConnectorProxy
    {
        private const int ConnectionRetryThreshold = 60; // How often we can retry opening this connection, in seconds.
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private bool disposed;

        public JmxConnectionInfo ConnectionInfo { get; private set; }
        private JMXConnector Connector { get; set; }
        private DateTime LastConnectionAttempt { get; set; }

        public JmxConnectorProxy(JmxConnectionInfo connectionInfo)
        {
            ConnectionInfo = connectionInfo;
            Connector = JMXConnectorFactory.newJMXConnector(ConnectionInfo.GetJmxServiceUrl(), null);
            LastConnectionAttempt = DateTime.MinValue;
        }

        ~JmxConnectorProxy()
        {
            Dispose(false);
        }

        #region Public Methods

        /// <summary>
        /// Retrieve an MBeanServerConnection object for this connector.  If this connector is currently not connected, will attempt to establish a new connection.
        /// </summary>
        /// <returns>MBeanServerConnection object, or null if no connection can be established.</returns>
        [CLSCompliant(false)]
        public MBeanServerConnection GetConnection()
        {
            try
            {
                return Connector.getMBeanServerConnection();
            }
            catch (java.io.IOException)
            {
                Log.Debug(String.Format("Connection to JMX server at {0} is closed", ConnectionInfo));

                // Reconnect only if we have surpassed time threshold since last reconnection attempt, or if we have never attempted to connect at all.
                var secondsSinceLastConnectionAttempt = (DateTime.Now - LastConnectionAttempt).TotalSeconds;

                // Give up if we're trying to reconnect before reaching the retry threshold.
                if (secondsSinceLastConnectionAttempt < ConnectionRetryThreshold)
                {
                    return null;
                }

                Log.Debug(String.Format("Attempting to reconnect to JMX server at {0}..", ConnectionInfo));
                try
                {
                    OpenConnection();
                    return Connector.getMBeanServerConnection();
                }
                catch (Exception innerEx)
                {
                    // Failed to reconnect.. log and give up.
                    Log.Error(String.Format("Could not establish connection to JMX server at {0}: {1}", ConnectionInfo, innerEx.Message));
                }
                return null;
            }
        }

        /// <summary>
        /// Opens a new connection to a remote JMX server.  Rethrows any encountered exceptions.
        /// </summary>
        public void OpenConnection()
        {
            LastConnectionAttempt = DateTime.Now;

            Connector = JMXConnectorFactory.newJMXConnector(ConnectionInfo.GetJmxServiceUrl(), environment: null);
            Connector.connect();
            Log.Debug(String.Format("Opened connection to JMX server at {0}.", ConnectionInfo));
        }

        /// <summary>
        /// Attempts to gracefully close a connection to a remote JMX server.
        /// </summary>
        public void CloseConnection()
        {
            try
            {
                Connector.close();
            }
            catch (java.io.IOException ex)
            {
                Log.Warn(String.Format("Failed to cleanly close JMX connection to {0}: {1}", ConnectionInfo, ex.Message));
            }
        }

        #endregion Public Methods

        #region IDisposable Methods

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing && Connector != null)
            {
                CloseConnection();
                Connector.Dispose();
            }
            disposed = true;
        }

        #endregion IDisposable Methods
    }
}