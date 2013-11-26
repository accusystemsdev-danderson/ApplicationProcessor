//-----------------------------------------------------------------------------
// <copyright file="Configuration.cs" company="AccuSystems LLC">
//     Copyright (c) AccuSystems.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace ApplicationProcessor
{
    using System.Xml.Linq;

    /// <summary>
    /// Provides information about configuration settings
    /// </summary>
    public static class Configuration
    {
        public static string LogFolder { get; set; }
        public static string DaysToKeepLogs { get; set; }
        public static string FieldMapFile { get; set; }
        public static string OutputFile { get; set; }
        public static string RulesFile { get; set; }
        public static string TestSourceModeYN { get; set; }
        public static string SourceDelimitedSqlXml { get; set; }
        public static string SourceSqlQueryFile { get; set; }
        public static string SourceSqlConnectionString { get; set; }
        public static string PostProcessingQueryYN { get; set; }
        public static string PostProcessingSqlQueryFile { get; set; }
        public static string SourceFile { get; set; }
        public static string CollateralsYN { get; set; }
        public static string ProcessMTEs { get; set; }
        public static string DbConnectionString { get; set; }
        public static string ProcessExistingAccounts { get; set; }
        public static string AccountsProcessedLogFile { get; set; }
        public static string ImporterPath { get; set; }
        public static string UseLegacyImportYN { get; set; }

        /// <summary>
        /// Reads in configuration settings from the configuration file
        /// </summary>
        /// <param name="configFileName">The File Name of the configuration file</param>
        public static void ReadConfiguration(string configFileName)
        {
            XElement xmlConfig = XElement.Load(configFileName);
            
            LogFolder = Utils.ReadXMLElementValue(xmlConfig, "LogFolder", "Logs\\");
            DaysToKeepLogs = Utils.ReadXMLElementValue(xmlConfig, "DaysToKeepLogs", "14");
            FieldMapFile = Utils.ReadXMLElementValue(xmlConfig, "FieldMapFile", "FieldMappings.xml");
            OutputFile = Utils.ReadXMLElementValue(xmlConfig, "OutputFile", "..\\AccuAccountImporter\\acculoan.xml");
            RulesFile = Utils.ReadXMLElementValue(xmlConfig, "RulesFile", "rules.xml");
            TestSourceModeYN = Utils.ReadXMLElementValue(xmlConfig, "TestSourceModeYN", "N");
            SourceDelimitedSqlXml = Utils.ReadXMLElementValue(xmlConfig, "SourceDelimitedSqlXml", "");
            SourceSqlQueryFile = Utils.ReadXMLElementValue(xmlConfig, "SourceSqlQueryFile", "");
            SourceSqlConnectionString = Utils.ReadXMLElementValue(xmlConfig, "SourceSqlConnectionString", "");
            PostProcessingQueryYN = Utils.ReadXMLElementValue(xmlConfig, "PostProcessingQueryYN", "");
            PostProcessingSqlQueryFile = Utils.ReadXMLElementValue(xmlConfig, "PostProcessingSqlQueryFile", "");
            SourceFile = Utils.ReadXMLElementValue(xmlConfig, "SourceFile", "");
            CollateralsYN = Utils.ReadXMLElementValue(xmlConfig, "CollateralsYN", "N");
            ProcessMTEs = Utils.ReadXMLElementValue(xmlConfig, "ProcessMTEs", "N");
            ProcessExistingAccounts = Utils.ReadXMLElementValue(xmlConfig, "ProcessExistingAccounts", "N");
            AccountsProcessedLogFile = Utils.ReadXMLElementValue(xmlConfig, "AccountsProcessedLogFile", "Accounts.txt");
            ImporterPath = Utils.ReadXMLElementValue(xmlConfig, "ImporterPath", "..\\accuaccountimporter");
            UseLegacyImportYN = Utils.ReadXMLElementValue(xmlConfig, "UseLegacyImportYN", "N");
        }

    }
}
