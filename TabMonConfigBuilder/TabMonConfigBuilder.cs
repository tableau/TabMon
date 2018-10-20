using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TabMonConfigBuilder.Helpers;

namespace TabMonConfigBuilder
{
    public sealed class TabMonConfigBuilder
    {
        private readonly CommandLineOptions commandLineOptions;
        private readonly string currentWorkingDirectory;

        public TabMonConfigBuilder(CommandLineOptions commandLineOptions, string currentWorkingDirectory)
        {
            this.commandLineOptions = commandLineOptions;
            this.currentWorkingDirectory = currentWorkingDirectory;
        }

        #region Public Methods

        /// <summary>
        /// Sets up and issues the LogsharkRequest to the LogsharkController.
        /// </summary>
        public void Execute()
        {
            try
            {
                Console.WriteLine("Parsing topology file for process entries..");
                var hosts = ParseTopologyAndUpdateHosts(commandLineOptions.Target);
                Console.WriteLine(string.Format("Writing config section and instructions to {0}..", commandLineOptions.Output));
                WriteToFile(commandLineOptions.Output, hosts);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            Console.WriteLine("Done writing config section..");
        }

        #endregion Public Methods

        #region Private Methods

        private static Dictionary<string, Host> ParseTopologyAndUpdateHosts(string target)
        {
            string line;
            int index = 0;
            Regex spaceRegex = new Regex(@"\s+");
            var hosts = new Dictionary<string, Host>();

            Console.WriteLine("Building config section..");
            var input = new StreamReader(target);
            while ((line = input.ReadLine()) != null)
            {
                // Check if index is not 0 to skip the first line.
                if (index != 0)
                {
                    var splitLine = spaceRegex.Split(line);
                    var process = splitLine[1].Split(':')[0];
                    var portType = splitLine[1].Split(':')[1];
                    if (portType == "jmx" && StringHelpers.ProcessLookupDict.ContainsKey(process))
                    {
                        UpdateHosts(hosts, splitLine[0], process, Int32.Parse(splitLine[2]), Int32.Parse(splitLine[3]));
                    }
                }
                index++;
            }
            input.Close();

            return hosts;
        }

        private static void UpdateHosts(Dictionary<string, Host> hosts, string nodeName, string processName, int processNum, int portNum)
        {
            if (!hosts.ContainsKey(nodeName))
            {
                hosts.Add(nodeName, new Host(nodeName, StringHelpers.ProcessLookupDict[processName], processName, processNum, portNum));
            }
            else
            {
                hosts[nodeName].Add(StringHelpers.ProcessLookupDict[processName], processName, processNum, portNum);
            }
        }

        private static string CreateConfigString(Dictionary<string, Host> hosts)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string line in StringHelpers.InstructionSet)
            {
                sb.AppendLine(line);
            }

            foreach (Host host in hosts.Values)
            {
                sb.AppendLine(host.CreateString());
            }

            return sb.ToString();
        }

        private static void WriteToFile(string outputTarget, Dictionary<string, Host> hosts)
        {
            File.WriteAllText(outputTarget, CreateConfigString(hosts));
        }

        #endregion Private Methods
    }
}
