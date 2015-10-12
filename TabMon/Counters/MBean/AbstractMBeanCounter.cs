using log4net;
using System;
using System.Reflection;
using TabMon.Helpers;
using TabMon.Sampler;

namespace TabMon.Counters.MBean
{
    /// <summary>
    /// Base class that other MBeanCounter classes should inherit from.
    /// </summary>
    [CLSCompliant(false)]
    public abstract class AbstractMBeanCounter : ICounter
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public Host Host { get; private set; }
        public string CounterType { get; private set; }
        public string Source { get; private set; }
        public string Category { get; private set; }
        public string Counter { get; private set; }
        public string Instance { get; private set; }
        public string Unit { get; private set; }
        protected IMBeanClient MBeanClient { get; set; }
        protected string JmxDomain { get; set; }
        protected string Path { get; set; }

        protected AbstractMBeanCounter(IMBeanClient mbeanClient, string counterType, string jmxDomain, Host host, string source, string filter,
                                       string category, string counter, string instance, string unit)
        {
            Host = host;
            CounterType = counterType;
            Source = source;
            Category = category;
            Counter = counter;
            Instance = instance;
            Unit = unit;
            MBeanClient = mbeanClient;
            JmxDomain = jmxDomain;
            Path = filter;
        }

        #region Public Methods

        /// <summary>
        /// Sample this counter.
        /// </summary>
        /// <returns>ICounterSample containing the sampled attribute value of this counter.</returns>
        public ICounterSample Sample()
        {
            try
            {
                var value = GetAttributeValue(Counter);
                return new CounterSample(this, value);
            }
            catch (Exception ex)
            {
                Log.Debug(String.Format(@"Error sampling counter {0}: {1}", this, ex.Message));
                return null;
            }
        }

        public override string ToString()
        {
            return String.Format(@"{0}\{1}\{2}:{3}\{4}\{5}", Host, Source, JmxDomain, Path, Counter, Instance);
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Retrieve the value of the attribute with the given name for this counter.
        /// </summary>
        /// <param name="attribute">Name of the attribute to sample.</param>
        /// <returns>A generic object containing the sampled value for the given attribute.</returns>
        protected abstract object GetAttributeValue(string attribute);

        #endregion Protected Methods
    }
}