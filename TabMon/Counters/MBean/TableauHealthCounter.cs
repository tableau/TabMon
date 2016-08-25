using System;
using javax.management.openmbean;
using TabMon.CounterConfig;
using TabMon.Helpers;

namespace TabMon.Counters.MBean
{
    /// <summary>
    /// Represents a Tableau MBean counter.  These counters expose their information as a composite data object obtained via a 'getPerformanceMetrics' method.
    /// </summary>
    internal class TableauHealthCounter : AbstractMBeanCounter
    {
        private const string TableauHealthJmxDomain = "tableau.health.jmx";
        private const string TableauHealthCounterType = "Tableau Server Health";

        public TableauHealthCounter(IMBeanClient mbeanClient, Host host, string sourceName, string path, string categoryName, string counterName, string instanceName, string unit)
            : base(mbeanClient: mbeanClient, lifecycleType: CounterLifecycleType.Persistent, counterType: TableauHealthCounterType, jmxDomain: TableauHealthJmxDomain, host: host, source: sourceName, filter: path, category: categoryName, counter: counterName, instance: instanceName, unit: unit) { }

        #region Protected Methods

        protected override object GetAttributeValue(string attribute)
        {
            // Find MBean object.
            var objectNames = MBeanClient.QueryObjects(JmxDomain, Path);

            // Validated MBean object was found.
            if (objectNames.Count == 0)
            {
                throw new ArgumentException("Unable to query MBean.");
            }

            // Grab associated attributes.
            var attributes = MBeanClient.InvokeMethod(objectNames[0], "getPerformanceMetrics") as CompositeData;

            // Look up the attribute we care about & do some wonky parsing to convert it from Java CompositeData into C# float.
            var result = attributes.get(attribute).ToString();
            return float.Parse(result);
        }

        #endregion
    }
}