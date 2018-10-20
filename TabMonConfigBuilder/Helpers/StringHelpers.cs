using System.Collections.Generic;

namespace TabMonConfigBuilder.Helpers
{
    /// <summary>
    /// Static string helper class.
    /// </summary>
    class StringHelpers
    {
        public static readonly IList<string> InstructionSet = new List<string>()
                    {
                        "Usage Instructions!",
                        "STEP 1: Verify that the host and process count are correct. If there",
                        "        are any discrepancies, you may need to manually add or remove",
                        "        hosts and processes.",
                        "STEP 2: For each host, replace the worker in \"computerName\" and",
                        "        \"address\" entry to reflect the correct host information.",
                        "STEP 3: Copy the information below the line into the \"Cluster section\"",
                        "        of the TabMon.Config.",
                        "_______________________________________________________________________",
                        ""
                    };

        public static readonly IDictionary<string, string> ProcessLookupDict = new Dictionary<string, string>
                                                                                        {
                                                                                            {"vizqlserver","vizql"},
                                                                                            {"vizportal","vizportal"},
                                                                                            {"dataserver","dataserver"},
                                                                                            {"backgrounder","background job monitor"}
                                                                                        };
    }
}
