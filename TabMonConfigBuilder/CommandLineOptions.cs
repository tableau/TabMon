using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace TabMonConfigBuilder
{
    /// <summary>
    /// Command-line options for config builder.
    /// </summary>
    public class CommandLineOptions
    {
        private string sanitizedTarget;
        private string sanitizedOutput;

        [ParserState]
        public IParserState LastParserState { get; set; }

        [ValueOption(0)]
        public string Target
        {
            get
            {
                return sanitizedTarget;
            }
            set
            {
                sanitizedTarget = value.TrimEnd('"', Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).TrimStart('"');
            }
        }

        [ValueOption(1)]
        public string Output
        {
            get
            {
                return sanitizedOutput;
            }
            set
            {
                sanitizedOutput = value.TrimEnd('"', Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).TrimStart('"');
            }
        }

        [HelpOption]
        public string GetUsage()
        {
            var help = GetHeader();

            help.AddPreOptionsLine(Environment.NewLine + "Usage:");
            help.AddPreOptionsLine(@"  tabmonconfigbuilder [TARGET] [OUTPUT]");
            help.AddPreOptionsLine(@" ");
            help.AddPreOptionsLine(@"  Builds a TabMon config host section from the command output");
            help.AddPreOptionsLine(@"  of 'tsm topology list-ports'.");
            help.AddPreOptionsLine(@"_____________________________________________________________");
            help.AddPreOptionsLine(@" ");
            help.AddPreOptionsLine(Environment.NewLine + "Usage Examples:");
            help.AddPreOptionsLine(@"  tabmonconfigbuilder C:\Tableau\topology.txt C:\output.txt");
            help.AddPreOptionsLine(@" ");
            help.AddPreOptionsLine(@"  Runs the config builder using tsm topology output.");
            help.AddPreOptionsLine(@"  Outputs the config file chunk with instructions");
            help.AddPreOptionsLine(@"  to C:\output.txt.");
            help.AddPreOptionsLine(@" ");

            // Display helpful information about any parsing errors.
            if (LastParserState != null && LastParserState.Errors.Any())
            {
                var errors = help.RenderParsingErrorsText(this, indent: 2);

                if (!string.IsNullOrEmpty(errors))
                {
                    help.AddPostOptionsLine("ERROR(S):");
                    help.AddPostOptionsLine(errors);
                }
            }

            return help;
        }

        public HelpText GetHeader()
        {
            AssemblyName assembly = Assembly.GetExecutingAssembly().GetName();
            string company = ((AssemblyCompanyAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyCompanyAttribute), false)).Company;

            return new HelpText
            {
                Heading = new HeadingInfo(assembly.Name, assembly.Version.ToString()),
                Copyright = new CopyrightInfo(company, DateTime.UtcNow.Year),
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = true
            };
        }
    }
}
