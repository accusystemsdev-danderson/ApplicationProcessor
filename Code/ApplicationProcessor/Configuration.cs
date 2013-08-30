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
        public string SourceDelimitedSQLXML { get; set; }
        public string SourceSQLQueryFile { get; set; }
        public string SourceSQLConnectionString { get; set; }
        public string SourceFile { get; set; }
        public string CollateralsYN { get; set; }
        public string ProcessMTEs { get; set; }


        public void ReadConfiguration(string configFileName)
        {
            XElement xmlConfig = XElement.Load(configFileName);
            
            LogFolder = xmlConfig.Element("LogFolder").Value;
            DaysToKeepLogs = xmlConfig.Element("DaysToKeepLogs").Value;
            PathToDBXML = xmlConfig.Element("PathToDBXML").Value;
            FieldMapFile = xmlConfig.Element("FieldMapFile").Value;
            OutputFile = xmlConfig.Element("OutputFile").Value;
            RulesFile = xmlConfig.Element("RulesFile").Value;
            SourceDelimitedSQLXML = xmlConfig.Element("SourceDelimitedSQLXML").Value;
            SourceSQLQueryFile = xmlConfig.Element("SourceSQLQueryFile").Value;
            SourceSQLConnectionString = xmlConfig.Element("SourceSQLConnectionString").Value;
            SourceFile = xmlConfig.Element("SourceFile").Value;
            CollateralsYN = xmlConfig.Element("CollateralsYN").Value;
            ProcessMTEs = xmlConfig.Element("ProcessMTEs").Value;
        }

    }
}
