using System.Collections.Generic;

namespace TabMon.Helpers
{
    /// <summary>
    /// Helper class that simply encapsulates metadata about a process.
    /// </summary>
    public sealed class Process
    {
        public string ProcessName { get; private set; }
        public int PortNumber { get; private set; }
        public int ProcessNumber { get; private set; }

        public Process(string processName, int portNumber, int processNumber)
        {
            ProcessName = processName;
            PortNumber = portNumber;
            ProcessNumber = processNumber;
        }
    }
}