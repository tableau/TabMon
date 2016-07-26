using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
        /// <param name="categoryName">The counter category.</param>
        /// <param name="counterName">The name of the counter.</param>
        /// <param name="unitOfMeasurement">The unit of measurement that this counter reports in.  This is a piece of metadata we add in.</param>
        /// <param name="instanceFilters">A collection of search terms to use to filter out instances that do not match.</param>
        /// <returns>List of all matching Perfmon counters.</returns>
        public static IList<PerfmonCounter> LoadInstancesForCounter(Host host, string categoryName, string counterName, string unitOfMeasurement, IList<string> instanceFilters)
        {
            IList<PerfmonCounter> counters = new List<PerfmonCounter>();

            // Check if counter exists.
            var counterExists = false;
            try
            {
                counterExists = PerformanceCounterCategory.CounterExists(counterName, categoryName, host.Name);
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error(String.Format("Error checking for existence of Perfmon counter '{0}' in category '{1}' on host '{2}': {3}",
                                        counterName, categoryName, host.Name, ex.Message));
            }
            catch (Win32Exception ex)
            {
                Log.Error(String.Format("Could not communicate with Perfmon on target host '{0}': {1}", host.Name, ex.Message));
            }

            // If the requested counter does not exist, log it and bail out.
            if (!counterExists)
            {
                Log.Debug(String.Format("Counter '{0}' in category '{1}' on host '{2}' does not exist.",
                        counterName, categoryName, host.Name));
                return counters;
            }

            var category = new PerformanceCounterCategory(categoryName, host.Name);

            // Perfmon has both "single-instance" and "multi-instance" counter types -- we need to handle both appropriately.
            if (category.CategoryType == PerformanceCounterCategoryType.SingleInstance)
            {
                // Just create it and add it to the list.
                var counter = new PerfmonCounter(host, categoryName, counterName, null, unitOfMeasurement);
                counters.Add(counter);
            }
            else
            {
                var instanceNames = new List<string>(category.GetInstanceNames());

                // If we didn't specify any instance names to filter by, we just grab everything.
                if (instanceFilters.Count == 0) 
                {
                    foreach (var instanceName in instanceNames)
                    {
                        var builder = new PerfmonCounterBuilder();
                        var counter = builder.CreateCounter(host)
                                        .WithCategoryName(categoryName)
                                        .WithCounterName(counterName)
                                        .WithInstanceName(instanceName)
                                        .WithUnit(unitOfMeasurement);
                        counters.Add(counter);
                    }
                }
                // If we did specify instance names to filter by, we loop over instance names, instantiating any counters that match our filters.
                else
                {
                    foreach (var instanceFilter in instanceFilters)
                    {
                        var matchingInstances = instanceNames.Where(instanceName => instanceName.Contains(instanceFilter));

                        foreach (var instanceName in matchingInstances)
                        {
                            var builder = new PerfmonCounterBuilder();
                            var counter = builder.CreateCounter(host)
                                            .WithCategoryName(categoryName)
                                            .WithCounterName(counterName)
                                            .WithInstanceName(instanceName)
                                            .WithUnit(unitOfMeasurement);

                            counters.Add(counter);
                        }
                    }
                }
            }

            return counters;
        }
    }
}