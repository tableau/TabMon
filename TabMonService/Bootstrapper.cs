using System;
using Topshelf;
using TabMon;

namespace TabMonService
{
    /// <summary>
    /// Serves as a thin bootstrapper for the TabMonAgent class and adapts underlying Stop/Start methods to the service context.
    /// </summary>
    public class TabMonServiceBootstrapper : ServiceControl, IDisposable
    {
        private TabMonAgent agent;
        private bool disposed;
        public static readonly string Log4NetConfigKey = TabMonAgent.Log4NetConfigKey;

        ~TabMonServiceBootstrapper()
        {
            Dispose(false);
        }

        #region Public Methods

        /// <summary>
        /// Creates an instance of the TabMonAgent and starts it.
        /// </summary>
        /// <param name="hostControl">Service HostControl object</param>
        /// <returns>Indicator that service succesfully started</returns>
        public bool Start(HostControl hostControl)
        {
            // Request additional time from the service host due to how much initialization has to take place.
            hostControl.RequestAdditionalTime(TimeSpan.FromSeconds(10));

            // Initialize and start service.
            try
            {
                agent = new TabMonAgent();
            }
            catch (Exception)
            {
                return false;
            }
            agent.Start();
            return agent.IsRunning();
        }

        /// <summary>
        /// Stops the TabMonAgent service.
        /// </summary>
        /// <param name="hostControl">Service HostControl object</param>
        /// <returns>Indicator that service succesfully stopped</returns>
        public bool Stop(HostControl hostControl)
        {
            if (agent != null)
            {
                agent.Stop();
                agent.Dispose();
            }
            return true;
        }

        #endregion

        #region IDisposable Methods

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                agent.Dispose();
            }
            disposed = true;
        }

        #endregion
    }
}