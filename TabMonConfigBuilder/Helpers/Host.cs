using System.Collections.Generic;
using System.Text;

namespace TabMonConfigBuilder.Helpers
{
    /// <summary>
    /// Object defining a host.
    /// </summary>
    class Host
    {
        private IDictionary<string, Process> Processes { get; set; }
        private string HostName { get; set; }
        private string HostString = "      <Host computerName=\"{0}\" address=\"{0}\" specifyPorts=\"true\">\r\n{1}      </Host>";

        public Host(string host, string processName, string process, int processNum, int portNum)
        {
            HostName = host;
            Processes = new Dictionary<string, Process>();
            Processes.Add(processName, new Process(processName, processNum, portNum));
        }

        public void Add(string processName, string process, int processNum, int portNum)
        {
            if (!Processes.ContainsKey(processName))
            {
                Processes.Add(processName, new Process(processName, processNum, portNum));
            }
            else
            {
                Processes[processName].Add(processNum, portNum);
            }
        }

        public string CreateString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (Process process in Processes.Values)
            {
                sb.AppendLine(process.CreateString());
            }

            return string.Format(HostString, HostName, sb.ToString());
        }
    }
}
