using javax.management.openmbean;
using System;
using TabMon.Helpers;

namespace TabMon.Counters.MBean
{
    /// <summary>
    /// Represents a Java language MBean counter.
    /// </summary>
    internal class JavaHealthCounter : AbstractMBeanCounter
    {
        private const string JavaHealthJmxDomain = "java.lang";
        private const string JavaHealthCounterType = "JVM Health";

        public JavaHealthCounter(IMBeanClient mbeanClient, Host host, string sourceName, string path, string categoryName, string counterName, string instanceName, string unit)
            : base(mbeanClient: mbeanClient, counterType: JavaHealthCounterType, jmxDomain: JavaHealthJmxDomain, host: host, source: sourceName, filter: path, category: categoryName, counter: counterName, instance: instanceName, unit: unit) { }

        #region Protected Methods

        protected override object GetAttributeValue(string attribute)
        {
            // Find MBean object.
            var objectNames = MBeanClient.QueryObjects(JmxDomain, Path);

            // Validated MBean object was found.
            if (objectNames.Count < 1)
            {
                throw new ArgumentException("Unable to query MBean.");
            }

            object result;
            var obj = objectNames[0];

            // The Java Health counters may be nested as CompositeData objects, so we need to be prepared to handle this.
            if (attribute.Contains(@"\"))
            {
                var pathSegments = attribute.Split('\\');
                var parent = pathSegments[0];
                var child = pathSegments[1];
                var compositeData = MBeanClient.GetAttributeValue(obj, parent) as CompositeData;
                result = compositeData.get(child);
            }
            else
            {
                result = MBeanClient.GetAttributeValue(obj, attribute);
            }

            // Wonky parsing to convert result from Java lang object into C# float.
            return float.Parse(result.ToString());
        }

        #endregion
    }
}