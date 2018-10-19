using System.Collections.Generic;
using System.Text;

namespace TabMonConfigBuilder.Helpers
{
    /// <summary>
    /// Object defining processes.
    /// </summary>
    class Process
    {
        private string ProcessName { get; set; }
        private IList<Port> Ports { get; set; }
        private string ProcessString = "        <Process processName=\"{0}\">\r\n{1}        </Process>";

        public Process(string processName, int processNum, int portNum)
        {
            ProcessName = processName;
            Ports = new List<Port>();
            Ports.Add(new Port(portNum, processNum));
        }

        public void Add(int processNum, int portNum)
        {
            Ports.Add(new Port(portNum, processNum));
        }

        public string CreateString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Port port in Ports)
            {
                sb.AppendLine(port.CreateString());
            }

            return string.Format(ProcessString, ProcessName, sb.ToString());
        }
    }
}
