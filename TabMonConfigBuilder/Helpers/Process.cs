namespace TabMonConfigBuilder.Helpers
{
    /// <summary>
    /// Object defining ports.
    /// </summary>
    class Process
    {
        private int PortNumber { get; set; }
        private int ProcessNumber { get; set; }
        private string PortString = "          <Process portNumber=\"{0}\" processNumber=\"{1}\"/>";

        public Process(int portNumber, int processNumber)
        {
            PortNumber = portNumber;
            ProcessNumber = processNumber;
        }

        public string CreateString()
        {
            return string.Format(PortString, PortNumber, ProcessNumber);
        }
    }
}
