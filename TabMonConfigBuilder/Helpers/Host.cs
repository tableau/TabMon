using System.Collections.Generic;
using System.Text;

namespace TabMonConfigBuilder.Helpers
{
    /// <summary>
    /// Object defining a host.
    /// </summary>
    class Host
    {
        private IDictionary<string, ProcessType> ProcessTypes { get; set; }
        private string HostName { get; set; }
        private string HostString = "      <Host computerName=\"{0}\" address=\"{0}\" specifyPorts=\"true\">\r\n{1}      </Host>";

        public Host(string host, string processName, string process, int processNum, int portNum)
        {
            HostName = host;
            ProcessTypes = new Dictionary<string, ProcessType>();
            ProcessTypes.Add(processName, new ProcessType(processName, processNum, portNum));
        }

        public void Add(string processName, string process, int processNum, int portNum)
        {
            if (!ProcessTypes.ContainsKey(processName))
            {
                ProcessTypes.Add(processName, new ProcessType(processName, processNum, portNum));
            }
            else
            {
                ProcessTypes[processName].Add(processNum, portNum);
            }
        }

        public string CreateString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (ProcessType processType in ProcessTypes.Values)
            {
                sb.AppendLine(processType.CreateString());
            }

            return string.Format(HostString, HostName, sb.ToString());
        }
    }
}
