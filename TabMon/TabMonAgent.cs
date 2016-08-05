using DataTableWriter.Writers;
using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Threading;
using TabMon.Config;
using TabMon.CounterConfig;
using TabMon.Counters;
using TabMon.Sampler;

[assembly: CLSCompliant(true)]

namespace TabMon
{
    /// <summary>
    /// A timer-based performance monitoring agent.  Loads a set of counters from a config file and polls them periodically, passing the results to a writer object.
    /// </summary>
    public class TabMonAgent : IDisposable
    {
        private Timer timer;
        private CounterSampler sampler;
        private readonly TabMonOptions options;
        private bool disposed;
        private const int WriteLockAcquisitionTimeout = 10; // In seconds.
        private static readonly object WriteLock = new object();
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static readonly string Log4NetConfigKey = "log4net-config-file";

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public TabMonAgent(bool loadOptionsFromConfig = true)
        {
            // Initialize log4net settings.
            var assemblyLocation = Assembly.GetExecutingAssembly().Location;
            Directory.SetCurrentDirectory(Path.GetDirectoryName(assemblyLocation));
            XmlConfigurator.Configure(new FileInfo(ConfigurationManager.AppSettings[Log4NetConfigKey]));

            // Load TabMonOptions.  In certain use cases we may not want to load options from the config, but provide them another way (such as via a UI).
            options = TabMonOptions.Instance;
            if (loadOptionsFromConfig)
            {
                TabMonConfigReader.LoadOptions();
            }
        }

        ~TabMonAgent()
        {
            Dispose(false);
        }

        #region Public Methods

        /// <summary>
        /// Starts up the agent.
        /// </summary>
        public void Start()
        {
            Log.Info("Initializing TabMon..");

            // Assert that runtime options are valid.
            if (!TabMonOptions.Instance.Valid())
            {
                Log.Fatal("Invalid TabMon options specified!\nAborting..");
                return;
            }

            // Spin up counter sampler.
            try
            {
                sampler = new CounterSampler(options.Hosts, options.TableName);
            }
            catch(Exception ex)
            {
                Log.ErrorFormat("Failed to initialize counter sampler using counters from configuration file: {0}\nAborting..", ex.Message);
                return;
            }

            // Kick off the polling timer.
            Log.Info("TabMon initialized!  Starting performance counter polling..");
            timer = new Timer(callback: OnTimer, state: null, dueTime: 0, period: options.PollInterval * 1000);
        }

        /// <summary>
        /// Stops the agent by disabling the timer.  Uses a write lock to prevent data from being corrupted mid-write.
        /// </summary>
        public void Stop()
        {
            Log.Info("Shutting down TabMon..");
            // Wait for write lock to finish before exiting to avoid corrupting data, up to a certain threshold.
            if (!Monitor.TryEnter(WriteLock, WriteLockAcquisitionTimeout * 1000))
            {
                Log.Error("Could not acquire write lock; forcing exit..");
            }
            else
            {
                Log.Debug("Acquired write lock gracefully..");
            }

            if (timer != null)
            {
                timer.Dispose();
            }
            Log.Info("TabMon stopped.");
        }

        /// <summary>
        /// Indicates whether the agent is currently running (is initialized & has an active timer).
        /// </summary>
        /// <returns>Bool indicating whether the agent is currently running.</returns>
        public bool IsRunning()
        {
            return sampler != null && timer != null;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Polls the sampler's counters and writes the results to the writer object.
        /// </summary>
        private void Poll()
        {
            var sampleResults = sampler.SampleAll();
            var compressedTable = CounterSamplerResultHelper.CompressTable(sampleResults);
            options.Writer.Write(compressedTable);   
        }

        /// <summary>
        /// Checks to see if data is purgeable then purges based off of threshold.
        /// </summary>
        private void PurgeExpiredData()
        {
            if (options.Writer is IPurgeableDatasource)
            {
                var purgeableDatasource = options.Writer as IPurgeableDatasource;
                try
                {
                    purgeableDatasource.PurgeExpiredData(options.TableName);
                }
                catch { }
            }
        }

        /// <summary>
        /// Handles all tasks involved in the pulling cycle.
        /// </summary>
        /// <param name="stateInfo"></param>
        private void OnTimer(object stateInfo)
        {
            lock (WriteLock)
            {
                Poll();
                PurgeExpiredData();
            }
        }


        #endregion Private Methods

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
                if (options.Writer != null)
                {
                    options.Writer.Dispose();
                }
            }
            disposed = true;
        }

        #endregion IDisposable Methods
    }
}