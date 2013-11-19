//-----------------------------------------------------------------------------
// <copyright file="Program.cs" company="AccuSystems LLC">
//     Copyright (c) AccuSystems.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace ApplicationProcessor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    /// <summary>
    /// Provides information about configuration settings
    /// </summary>
    public static class Configuration
    {
        public static string LogFolder { get; set; }
        public static string DaysToKeepLogs { get; set; }
        public static string PathToDBXML { get; set; }
        public static string FieldMapFile { get; set; }
        public static string OutputFile { get; set; }
        public static string RulesFile { get; set; }
        public static string TestSourceModeYN { get; set; }
        public static string SourceDelimitedSQLXML { get; set; }
        public static string SourceSQLQueryFile { get; set; }
        public static string SourceSQLConnectionString { get; set; }
        public static string PostProcessingQueryYN { get; set; }
        public static string PostProcessingSQLQueryFile { get; set; }
        public static string SourceFile { get; set; }
        public static string CollateralsYN { get; set; }
        public static string ProcessMTEs { get; set; }
        public static string dbConnectionString { get; set; }
        public static string ProcessExistingAccounts { get; set; }
        public static string AccountsProcessedLogFile { get; set; }
        public static string ImporterPath { get; set; }
        public static string UseLegacyImportYN { get; set; }

        public string UseLegacyImportYN { get; set; }

        
        /// <summary>
        /// Reads in configuration settings from the configuration file
        /// </summary>
        /// <param name="configFileName">The File Name of the configuration file</param>
        public static void ReadConfiguration(string configFileName)
        {
            XElement xmlConfig = XElement.Load(configFileName);
            
            LogFolder = Utils.ReadXMLElementValue(xmlConfig, "LogFolder", "Logs\\");
            DaysToKeepLogs = Utils.ReadXMLElementValue(xmlConfig, "DaysToKeepLogs", "14");
            PathToDBXML = Utils.ReadXMLElementValue(xmlConfig, "PathToDBXML", "..\\..\\config\\db.xml");
            FieldMapFile = Utils.ReadXMLElementValue(xmlConfig, "FieldMapFile", "FieldMappings.xml");
            OutputFile = Utils.ReadXMLElementValue(xmlConfig, "OutputFile", "..\\AccuAccountImporter\\acculoan.xml");
            RulesFile = Utils.ReadXMLElementValue(xmlConfig, "RulesFile", "rules.xml");
            TestSourceModeYN = Utils.ReadXMLElementValue(xmlConfig, "TestSourceModeYN", "N");
            SourceDelimitedSQLXML = Utils.ReadXMLElementValue(xmlConfig, "SourceDelimitedSQLXML", "");
            SourceSQLQueryFile = Utils.ReadXMLElementValue(xmlConfig, "SourceSQLQueryFile", "");
            SourceSQLConnectionString = Utils.ReadXMLElementValue(xmlConfig, "SourceSQLConnectionString", "");
            PostProcessingQueryYN = Utils.ReadXMLElementValue(xmlConfig, "PostProcessingQueryYN", "");
            PostProcessingSQLQueryFile = Utils.ReadXMLElementValue(xmlConfig, "PostProcessingSQLQueryFile", "");
            SourceFile = Utils.ReadXMLElementValue(xmlConfig, "SourceFile", "");
            CollateralsYN = Utils.ReadXMLElementValue(xmlConfig, "CollateralsYN", "N");
            ProcessMTEs = Utils.ReadXMLElementValue(xmlConfig, "ProcessMTEs", "N");
            ProcessExistingAccounts = Utils.ReadXMLElementValue(xmlConfig, "ProcessExistingAccounts", "N");
            AccountsProcessedLogFile = Utils.ReadXMLElementValue(xmlConfig, "AccountsProcessedLogFile", "Accounts.txt");
            ImporterPath = Utils.ReadXMLElementValue(xmlConfig, "ImporterPath", "..\\accuaccountimporter");
            UseLegacyImportYN = Utils.ReadXMLElementValue(xmlConfig, "LegacyImportYN", "N");
        }

        /// <summary>
        /// Builds a database connection string from the db.xml file using the PathToDBXML configration
        /// </summary>
        /// <returns>True if no exceptions are thrown</returns>
        public static bool SetupDBConnectionString()
        {
            bool success = false;
            try
            {
                StringBuilder connectionString = new StringBuilder();
                XElement dbXML = XElement.Load(PathToDBXML);
                foreach (XElement source in dbXML.Elements("DBSource"))
                {
                    if (source.Element("application").Value.ToUpper() == "ACCULOAN")
                    {
                        connectionString.Append("data source=" + source.Element("server").Value + ";");
                        connectionString.Append("initial catalog=" + source.Element("database").Value + ";");
                        connectionString.Append("user id=" + source.Element("user_ID").Value + ";");
                        connectionString.Append("password=" + source.Element("password").Value + ";");
                        connectionString.Append("MultipleActiveResultSets=True");
                    }
                }
                dbConnectionString = connectionString.ToString();
                if (dbConnectionString != "")
                    success = true;
            }
            catch (Exception)
            {
            }
            return success;
        }
    }
}
