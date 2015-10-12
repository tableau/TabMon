namespace TabMon.Helpers
{
    /// <summary>
    /// Helper class that simply encapsulates metadata about a host.
    /// </summary>
    public sealed class Host
    {
        public string Name { get; private set; }
        public string Cluster { get; private set; }

        public Host(string name, string cluster)
        {
            Name = name;
            Cluster = cluster;
        }

        public override string ToString()
        {
            return Cluster + "\\" + Name;
        }
    }
}
