using log4net;
using System;
using System.Net;
using System.Reflection;

namespace TabMon.Helpers
{
    /// <summary>
    /// Helper class to handle resolving hostnames.
    /// </summary>
    public static class HostnameHelper
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region Public Methods

        /// <summary>
        /// Given a hostname or IP address, attempts to resolve it to a simple machine name.
        /// </summary>
        /// <param name="unresolvedHostName">A hostname or IP address which we may be unsure of the validity of.</param>
        /// <returns>A valid hostname without any trailing domain information, or else null if we failed to resolve.</returns>
        public static string Resolve(string unresolvedHostName)
        {
            try
            {
                var hostEntry = Dns.GetHostEntry(unresolvedHostName);
                var resolvedHostName = hostEntry.HostName;
                // Strip off domain, if applicable.
                if (resolvedHostName.Split('.').Length >= 2)
                {
                    resolvedHostName = resolvedHostName.Substring(0, resolvedHostName.IndexOf('.'));
                }
                Log.Debug(String.Format("Successfully resolved '{0}' to '{1}'.", unresolvedHostName, resolvedHostName));
                return resolvedHostName;
            }
            catch (Exception ex)
            {
                Log.Error(String.Format("Could not resolve hostname for '{0}': {1}", unresolvedHostName, ex.Message));
                return null;
            }
        }

        #endregion
    }
}