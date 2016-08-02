using DataTableWriter;
using log4net;
using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using DataTableWriter.Connection;
using DataTableWriter.Drivers;
using DataTableWriter.Writers;
using TabMon.Helpers;
using System.Collections.Generic;

namespace TabMon.Config
{
    /// <summary>
    /// Parses the main application config to initialize an instance of TabMonOptions.
    /// </summary>
    public static class TabMonConfigReader
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region Public Methods

        /// <summary>
        /// Load TabMon.config and parse it into the TabMonOptions singleton, initializing writers if necessary.
        /// </summary>
        public static void LoadOptions()
        {
            var options = TabMonOptions.Instance;

            // Read TabMon.config
            Log.Info("Loading TabMon user configuration..");
            Configuration configFile;
            try
            {
                configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            }
            catch (ConfigurationErrorsException ex)
            {
                Log.Fatal(String.Format("Could not open configuration file: {0}", ex.Message));
                throw;
            }

            // Parse config.
            try
            {
                var config = (TabMonConfig)configFile.Sections["TabMonConfig"];

                // Load PollInterval.
                options.PollInterval = config.PollInterval.Value;

                // Load OutputMode.
                var outputMode = config.OutputMode.Value;
                if (outputMode.Equals("DB", StringComparison.InvariantCultureIgnoreCase))
                {
                    options.Writer = LoadDbWriterFromConfig(config);
                    options.TableName = config.Database.Table.Name;
                }
                else if (outputMode.Equals("CSV", StringComparison.InvariantCultureIgnoreCase))
                {
                    options.Writer = LoadCsvWriter();
                    options.TableName = "countersamples";
                }
                else
                {
                    Log.Fatal("Invalid output mode specified in configuration!");
                }

                // Load Cluster/Host configuration.
                var clusters = config.Clusters;
                foreach (Cluster cluster in clusters)
                {
                    var clusterName = cluster.Name;
                    foreach (Host host in cluster)
                    {
                        var resolvedHostname = HostnameHelper.Resolve(host.Name);
                        options.Hosts.Add(new Helpers.Host(resolvedHostname, clusterName));
                    }
                }
            }
            catch (ConfigurationErrorsException ex)
            {
                Log.Fatal(String.Format("Error loading TabMon.config: {0})", ex.Message));
                throw;
            }

            // Validate runtime options.
            if (!options.Valid())
            {
                Log.Fatal("Invalid options in configuration: " + options);
            }
            else
            {
                Log.Info("Successfully loaded TabMon config options! " + options);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes a new Database Writer object using a TabMonConfig.
        /// </summary>
        /// <param name="config">User's TabMon.config file containing the required Db Writer parameters.</param>
        /// <returns>Initialized DataTableDbWriter object.</returns>
        private static DataTableDbWriter LoadDbWriterFromConfig(TabMonConfig config)
        {
            DbDriverType dbDriverType;

            Log.Debug("Loading database configuration..");
            var databaseConfig = config.Database;

            var validDriverType = Enum.TryParse(databaseConfig.Type, true, out dbDriverType);
            if (!validDriverType)
            {
                throw new ConfigurationErrorsException("Invalid database driver type specified!");
            }

            IDbConnectionInfo dbConnInfo = new DbConnectionInfo()
            {
                Server = databaseConfig.Server.Host,
                Port = databaseConfig.Server.Port,
                Username = databaseConfig.User.Login,
                Password = databaseConfig.User.Password,
                DatabaseName = databaseConfig.Name
            };

            var indexes = new Dictionary<string, bool>();
            foreach (Index index in config.Database.Indexes)
            {
                indexes.Add(index.Column, index.Clustered);
            }

            if (!dbConnInfo.Valid())
            {
                throw new ConfigurationErrorsException("Missing required database connection information!");
            }

            var tableInitializationOptions = new DbTableInitializationOptions()
            {
                CreateTableDynamically = true,
                UpdateDbTableToMatchSchema = true,
                UpdateSchemaToMatchDbTable = true,
                UpdateIndexes = databaseConfig.Indexes.Generate,
                IndexesToGenerate = indexes,
                PurgeData = databaseConfig.PurgeOldData.Enabled,
                PurgeDataThreshold = databaseConfig.PurgeOldData.ThresholdDays
            };

            Log.Info("Connecting to results database..");
            try
            {
                return new DataTableDbWriter(dbDriverType, dbConnInfo, tableInitializationOptions);
            }
            catch (Exception ex)
            {
                Log.Fatal("Could not initialize writer: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Initializes a new CSV Writer object.
        /// </summary>
        /// <returns>Initialized DataTableCSVWriter object.</returns>
        private static DataTableCsvWriter LoadCsvWriter()
        {
            Log.Info("Initializing CSV writer..");
            // Set up output directory & filename.
            var resultOutputDirectory = Directory.GetCurrentDirectory() + @"\Results";
            var csvFileName = String.Format("TabMonResult_{0}.csv", DateTime.Now.ToString("yyyyMMdd_HHmmss"));
            var csvFilePath = resultOutputDirectory + @"\" + csvFileName;

            try
            {
                Directory.CreateDirectory(resultOutputDirectory);
                return new DataTableCsvWriter(csvFilePath);
            }
            catch (Exception ex)
            {
                Log.Fatal(String.Format("Could not open file stream to {0}: {1}", csvFileName, ex.Message));
                return null;
            }
        }

        #endregion
    }
}