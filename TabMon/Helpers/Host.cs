using System.Collections.Generic;

namespace TabMon.Helpers
{
    /// <summary>
    /// Helper class that simply encapsulates metadata about a host.
    /// </summary>
    public sealed class Host
    {
        public string Address { get; private set; }
        public string ComputerName { get; private set; }
        public string Cluster { get; private set; }
        public bool SpecifyPorts { get; private set; }
        public IDictionary<string, List<Process>> Processes {get; private set; }

        public Host(string address, string computerName, string cluster, bool specifyPorts, Dictionary<string, List<Process>> processes)
        {
            Address = address;
            ComputerName = computerName;
            Cluster = cluster;
            SpecifyPorts = specifyPorts;
            Processes = processes;
        }

        public Host(string address, string computerName, string cluster, bool specifyPorts)
        {
            Address = address;
            ComputerName = computerName;
            Cluster = cluster;
            SpecifyPorts = specifyPorts;
        }

        public override string ToString()
        {
            return Cluster + "\\" + Address + "\\" + ComputerName;
        }
    }
}
