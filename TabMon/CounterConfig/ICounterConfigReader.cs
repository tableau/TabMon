using System.Collections.Generic;
using System.Xml;
using TabMon.Counters;
using TabMon.Helpers;

namespace TabMon.CounterConfig
{
    /// <summary>
    /// Basic interface for an ICounterConfigReader.  Should be able to load counters for a given host just given a root node in an XML tree.
    /// </summary>
    internal interface ICounterConfigReader
    {
        ICollection<ICounter> LoadCounters(XmlNode root, Host host);
    }
}