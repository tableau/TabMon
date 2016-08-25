using TabMon.CounterConfig;
using TabMon.Helpers;
using TabMon.Sampler;

namespace TabMon.Counters
{
    /// <summary>
    /// Represents a generic performance counter interface.
    /// </summary>
    public interface ICounter
    {
        Host Host { get; }
        CounterLifecycleType LifecycleType { get; }
        string CounterType { get; }
        string Source { get; }
        string Category { get; }
        string Counter { get; }
        string Instance { get; }
        string Unit { get; }

        ICounterSample Sample();

        string ToString();
    }
}