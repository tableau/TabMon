using TabMon.Counters;

namespace TabMon.Sampler
{
    /// <summary>
    /// Represents a generic sampled performance counter of some kind.
    /// </summary>
    public class CounterSample : ICounterSample
    {
        public ICounter Counter { get; protected set; }
        public object SampleValue { get; protected set; }

        public CounterSample(ICounter counter, object sampleValue)
        {
            Counter = counter;
            SampleValue = sampleValue;
        }
    }
}