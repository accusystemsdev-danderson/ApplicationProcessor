using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace ApplicationProcessor
{
    class Configuration
    {
        public string LogFolder { get; set; }
        public string DaysToKeepLogs { get; set; }
        public string PathToDBXML { get; set; }
        public string FieldMapFile { get; set; }
        public string OutputFile { get; set; }
        public string RulesFile { get; set; }
        public string TestSourceModeYN { get; set; }
        public string SourceDelimitedSQLXML { get; set; }
        public string SourceSQLQueryFile { get; set; }
        public string SourceSQLConnectionString { get; set; }
        public string PostProcessingQueryYN { get; set; }
        public string PostProcessingSQLQueryFile { get; set; }
        public string SourceFile { get; set; }
        public string CollateralsYN { get; set; }
        public string ProcessMTEs { get; set; }
        public string dbConnectionString { get; set; }
        public string ProcessExistingAccounts { get; set; }
        public string AccountsProcessedLogFile { get; set; }
        
        public void ReadConfiguration(string configFileName)
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
        }

        public bool SetupDBConnectionString()
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
                        connectionString.Append("metadata=res://*/AccuLoanDBModel.csdl|res://*/AccuLoanDBModel.ssdl|res://*/AccuLoanDBModel.msl;provider=System.Data.SqlClient;provider connection string='");
                        connectionString.Append("data source=" + source.Element("server").Value + ";");
                        connectionString.Append("initial catalog=" + source.Element("database").Value + ";");
                        connectionString.Append("user id=" + source.Element("user_ID").Value + ";");
                        connectionString.Append("password=" + source.Element("password").Value + ";");
                        connectionString.Append("MultipleActiveResultSets=True;App=EntityFramework'");
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
