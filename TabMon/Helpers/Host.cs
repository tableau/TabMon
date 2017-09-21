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

        public Host(string address, string computerName, string cluster)
        {
            Address = address;
            ComputerName = computerName;
            Cluster = cluster;
        }

        public override string ToString()
        {
            return Cluster + "\\" + Address + "\\" + ComputerName;
        }
    }
}
