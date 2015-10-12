using TabMon.Helpers;

namespace TabMon.Counters.Perfmon
{
    /// <summary>
    /// Builder class to clean up instantiation of AbstractMBeanCounter-derived classes.
    /// </summary>
    public class PerfmonCounterBuilder
    {
        private Host Host { get; set; }
        private string Category { get; set; }
        private string Counter { get; set; }
        private string Instance { get; set; }
        private string Unit { get; set; }

        public PerfmonCounterBuilder CreateCounter(Host host)
        {
            Host = host;
            return this;
        }

        public PerfmonCounterBuilder WithCategoryName(string category)
        {
            Category = category;
            return this;
        }

        public PerfmonCounterBuilder WithCounterName(string counterName)
        {
            Counter = counterName;
            return this;
        }

        public PerfmonCounterBuilder WithInstanceName(string instance)
        {
            Instance = instance;
            return this;
        }

        public PerfmonCounterBuilder WithUnit(string unit)
        {
            Unit = unit;
            return this;
        }

        public static implicit operator PerfmonCounter(PerfmonCounterBuilder builder)
        {
            return new PerfmonCounter(builder.Host, builder.Category, builder.Counter, builder.Instance, builder.Unit);
        }
    }
}
