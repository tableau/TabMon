using System.Collections.Generic;
using System.Text;

namespace TabMonConfigBuilder.Helpers
{
    /// <summary>
    /// Object defining processes.
    /// </summary>
    class ProcessType
    {
        private string ProcessName { get; set; }
        private IList<Process> Processes { get; set; }
        private string ProcessString = "        <ProcessType processName=\"{0}\">\r\n{1}        </ProcessType>";

        public ProcessType(string processName, int processNum, int portNum)
        {
            ProcessName = processName;
            Processes = new List<Process>();
            Processes.Add(new Process(portNum, processNum));
        }

        public void Add(int processNum, int portNum)
        {
            Processes.Add(new Process(portNum, processNum));
        }

        public string CreateString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Process process in Processes)
            {
                sb.AppendLine(process.CreateString());
            }

            return string.Format(ProcessString, ProcessName, sb.ToString());
        }
    }
}
