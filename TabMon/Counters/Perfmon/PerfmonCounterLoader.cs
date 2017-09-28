using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using TabMon.CounterConfig;
using TabMon.Helpers;

namespace TabMon.Counters.Perfmon
{
    /// <summary>
    /// A factory-like class that loads PerfmonCounters matching the given set of search criteria.  This enables the user to provide a single "filter" that can potentially yield multiple concrete counters.
    /// </summary>
    internal static class PerfmonCounterLoader
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Loads all Perfmon counters on the target host matching the given parameters.
        /// </summary>
        /// <param name="host">The host to check counters on.</param>
        /// <param name="lifecycleType">Indicates whether the counters should be loaded as persistent or ephemeral counters.</param>
        /// <param name="categoryName">The counter category.</param>
        /// <param name="counterName">The name of the counter.</param>
        /// <param name="unitOfMeasurement">The unit of measurement that this counter reports in.  This is a piece of metadata we add in.</param>
        /// <param name="instanceFilters">A collection of search terms to use to filter out instances that do not match.</param>
        /// <returns>List of all matching Perfmon counters.</returns>
        public static IList<PerfmonCounter> LoadInstancesForCounter(Host host, CounterLifecycleType lifecycleType, string categoryName, string counterName, string unitOfMeasurement, ISet<string> instanceFilters)
        {
            IList<PerfmonCounter> counters = new List<PerfmonCounter>();

            // If the requested category does not exist, log it and bail out.
            if (!ExistsCategory(categoryName, host.ComputerName))
            {
                Log.WarnFormat("PerfMon counter category '{0}' on host '{1}' does not exist.",
                                categoryName, host.ComputerName);
                return counters;
            }

            // If the requested counter does not exist, log it and bail out.
            if (!ExistsCounter(counterName, categoryName, host.ComputerName))
            {
                Log.DebugFormat("PerfMon counter '{0}' in category '{1}' on host '{2}' does not exist.",
                                counterName, categoryName, host.ComputerName);
                return counters;
            }

            var category = new PerformanceCounterCategory(categoryName, host.ComputerName);

            // Perfmon has both "single-instance" and "multi-instance" counter types -- we need to handle both appropriately.
            switch (category.CategoryType)
            {
                case PerformanceCounterCategoryType.SingleInstance:
                    counters.Add(new PerfmonCounter(host, lifecycleType, categoryName, counterName, instance: null, unit: unitOfMeasurement));
                    break;
                case PerformanceCounterCategoryType.MultiInstance:
                    foreach (var instanceName in category.GetInstanceNames())
                    {
                        if (IsInstanceRequested(instanceName, instanceFilters))
                        {
                            counters.Add(new PerfmonCounter(host, lifecycleType, categoryName, counterName, instanceName, unitOfMeasurement));
                        }
                    }
                    break;
                default:
                    Log.ErrorFormat("Unable to determine category type of PerfMon counter '{0}' in category '{1}' on host '{2}' is unknown; skipping loading it.", counterName, categoryName, host.ComputerName);
                    break;
            }

            return counters;
        }

        /// <summary>
        /// Helper method that determines whether a given instance name matches a list of filter strings of instances that should be loaded.
        /// We treat the absence of filter strings as a wildcard match.
        /// </summary>
        /// <param name="instanceName">The PerfMon counter instance name.</param>
        /// <param name="instanceFilters">A collection of filter strings. If instance name contains one of these, we consider it to be requested.</param>
        /// <returns>True if instanceName contains one of the instance filter strings, or if no filters are specified</returns>
        private static bool IsInstanceRequested(string instanceName, ICollection<string> instanceFilters)
        {
            if (instanceFilters == null || instanceFilters.Count == 0)
            {
                return true;
            }

            return instanceFilters.Any(instanceName.Contains);
        }

        /// <summary>
        /// Indicates whether a performance counter category exists on a target machine.
        /// </summary>
        private static bool ExistsCategory(string categoryName, string machineName)
        {
            try
            {
                return PerformanceCounterCategory.Exists(categoryName, machineName);
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.ErrorFormat("Error checking for existence of Perfmon counter category '{0}' on host '{1}': {2}",
                                categoryName, machineName, ex.Message);
                return false;
            }
            catch (Win32Exception ex)
            {
                Log.ErrorFormat("Could not communicate with Perfmon on target host '{0}': {1}", machineName, ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Indicates whether a performance counter exists on a target machine.
        /// </summary>
        private static bool ExistsCounter(string counterName, string categoryName, string machineName)
        {
            try
            {
                return PerformanceCounterCategory.CounterExists(counterName, categoryName, machineName);
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.ErrorFormat("Error checking for existence of Perfmon counter '{0}' in category '{1}' on host '{2}': {3}",
                                counterName, categoryName, machineName, ex.Message);
                return false;
            }
            catch (Win32Exception ex)
            {
                Log.ErrorFormat("Could not communicate with Perfmon on target host '{0}': {1}", machineName, ex.Message);
                return false;
            }
        }
    }
}