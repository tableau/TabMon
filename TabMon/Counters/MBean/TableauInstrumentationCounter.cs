using javax.management;
using System;
using System.Collections.Generic;
using System.Linq;
using TabMon.Helpers;

namespace TabMon.Counters.MBean
{
    /// <summary>
    /// Represents a Tableau MBean counter defined by the instrumentation framework introduced in v9.3.
    /// </summary>
    internal class TableauInstrumentationCounter : AbstractMBeanCounter
    {
        private const string TableauInstrumentationJmxDomain = "com.tableausoftware.instrumentation";
        private const string TableauInstrumentationCounterType = "Tableau Server Instrumentation";

        public TableauInstrumentationCounter(IMBeanClient mbeanClient, Host host, string sourceName, string subDomain, string path, string categoryName, string counterName, string instanceName, string unit)
            : base(mbeanClient: mbeanClient,
                   counterType: TableauInstrumentationCounterType,
                   jmxDomain: TableauInstrumentationJmxDomain.JoinIfNotNull(".", subDomain),
                   host: host,
                   source: sourceName,
                   filter: path,
                   category: categoryName,
                   counter: counterName,
                   instance: instanceName,
                   unit: unit) { }

        #region Protected Methods

        protected override object GetAttributeValue(string attribute)
        {
            // Find MBean object.
            ICollection<ObjectName> objectNames = MBeanClient.QueryObjects(JmxDomain, Path);

            // Validated MBean object was found.
            if (objectNames.Count == 0)
            {
                throw new ArgumentException("Unable to query MBean.");
            }

            ObjectName obj = objectNames.First();
            object result = MBeanClient.GetAttributeValue(obj, attribute);

            // Wonky parsing to convert result from Java lang object into C# float.
            return float.Parse(result.ToString());
        }

        #endregion
    }
}
