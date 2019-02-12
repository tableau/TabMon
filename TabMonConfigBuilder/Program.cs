using CommandLine;
using System;

namespace TabMonConfigBuilder
{
    internal class Program
    {
        // <summary>
        /// The entry point for the application.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        /// <returns>0 if program execution was successful; 1 otherwise.</returns>
        private static int Main(string[] args)
        {
            // Store CWD in case the executing assembly is being run from the system PATH.
            string currentWorkingDirectory = Environment.CurrentDirectory;

            CommandLineOptions options = new CommandLineOptions();
            if (!Parser.Default.ParseArgumentsStrict(args, options))
            {
                // Parsing failed, exit with failure code.
                Console.WriteLine("Exiting..");
                return 1;
            }

            try
            {
                var configBuilder = new TabMonConfigBuilder(options, currentWorkingDirectory);
                configBuilder.Execute();
                Console.WriteLine("Exiting..");
                return 0;
            }
            catch (Exception Ex)
            {
                // Write exception to console, exit.
                Console.WriteLine(Ex);
                Console.WriteLine("Exiting..");
                return 1;
            }
        }
    }
}
