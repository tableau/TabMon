using System;
using System.Collections.Generic;
using MBeanInfo = javax.management.MBeanInfo;
using MBeanOperationInfo = javax.management.MBeanOperationInfo;
using ObjectName = javax.management.ObjectName;

namespace TabMon.Counters.MBean
{
    /// <summary>
    /// Represents an MBean server client.  References to instances of this class should be shared amongst multiple counters for efficiency.
    /// </summary>
    internal class MBeanClient : IMBeanClient
    {
        public JmxConnectorProxy Connector { get; protected set; }
        private bool disposed;

        public MBeanClient(JmxConnectorProxy connector)
        {
            Connector = connector;
        }

        ~MBeanClient()
        {
            Dispose(false);
        }

        #region Public Methods

        /// <summary>
        /// Query remote objects to find matches to the given domain & filter values in accordance with the domain:filter pattern.
        /// </summary>
        /// <param name="domain">The domain to match.</param>
        /// <param name="filter">The filter to match.</param>
        /// <returns>List of object names matching the domain:filter search parameters.</returns>
        public IList<ObjectName> QueryObjects(string domain, string filter)
        {
            IList<ObjectName> objects = new List<ObjectName>();
            var name = new ObjectName(domain + ":" + filter);
            var serverConnection = Connector.GetConnection();

            // Return empty list if we were unable to connect.
            if (serverConnection == null) return objects;

            var objectNames = serverConnection.queryNames(name, null);

            // We're working with a Java set here, so we need to explicitly use an iterator to loop over the data structure.
            var it = objectNames.iterator();
            while (it.hasNext())
            {
                var obj = (ObjectName)it.next();
                objects.Add(obj);
            }

            return objects;
        }

        /// <summary>
        /// Retrieve the value of some attribute on a remote object.
        /// </summary>
        /// <param name="objectName">The object name to query.</param>
        /// <param name="attributeName">The attribute name to sample.</param>
        /// <returns>Generic object containing the sampled value of the attribute.</returns>
        public object GetAttributeValue(ObjectName objectName, string attributeName)
        {
            var serverConnection = Connector.GetConnection();
            return serverConnection.getAttribute(objectName, attributeName);
        }

        /// <summary>
        /// Retrieve MBeanInfo object for a given object name.
        /// </summary>
        /// <param name="objectName">The object name to query.</param>
        /// <returns>MBeanInfo for the given object.</returns>
        public MBeanInfo GetMBeanInfo(ObjectName objectName)
        {
            var serverConnection = Connector.GetConnection();
            return serverConnection.getMBeanInfo(objectName);
        }

        /// <summary>
        /// Retrieve a collection of operations that a given object name exposes.
        /// </summary>
        /// <param name="objectName">The object name to query.</param>
        /// <returns>A collection of operations that the given object exposes.</returns>
        public ICollection<MBeanOperationInfo> GetOperations(ObjectName objectName)
        {
            var operationCollection = GetMBeanInfo(objectName).getOperations();
            return new List<MBeanOperationInfo>(operationCollection);
        }

        /// <summary>
        /// Invokes an operation on a remote object.
        /// </summary>
        /// <param name="objectName">The object name containing the method to execute.</param>
        /// <param name="methodname">The name of the method to execute.</param>
        /// <returns></returns>
        public object InvokeMethod(ObjectName objectName, string methodname)
        {
            var serverConnection = Connector.GetConnection();
            return serverConnection.invoke(objectName, methodname, null, null);
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
                Connector.Dispose();
            }
            disposed = true;
        }

        #endregion IDisposable Methods
    }
}