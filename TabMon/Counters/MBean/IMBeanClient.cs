using System;
using System.Collections.Generic;
using MBeanInfo = javax.management.MBeanInfo;
using MBeanOperationInfo = javax.management.MBeanOperationInfo;
using ObjectName = javax.management.ObjectName;

namespace TabMon.Counters.MBean
{
    /// <summary>
    /// An interface describing a local client to a remote MBean server.
    /// </summary>
    [CLSCompliant(false)]
    public interface IMBeanClient : IDisposable
    {
        JmxConnectorProxy Connector { get; }

        IList<ObjectName> QueryObjects(string domain, string filter);
        object GetAttributeValue(ObjectName objectName, string attributeName);
        MBeanInfo GetMBeanInfo(ObjectName objectName);
        ICollection<MBeanOperationInfo> GetOperations(ObjectName objectName);
        object InvokeMethod(ObjectName objectName, string methodname);
    }
}