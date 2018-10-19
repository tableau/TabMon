namespace TabMonConfigBuilder.Helpers
{
    /// <summary>
    /// Object defining ports.
    /// </summary>
    class Port
    {
        private int PortNumber { get; set; }
        private int ProcessNumber { get; set; }
        private string PortString = "          <Port portNumber=\"{0}\" processNumber=\"{1}\"/>";

        public Port(int portNumber, int processNumber)
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
