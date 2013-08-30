using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using System.Data.OleDb;



namespace ApplicationProcessor
{
    class Program
    {
        
        public static string AppTitle = "AccuAccount - Application Processor: Decision Pro";
        public static DateTime StartTime = DateTime.Now;
        public static string LogFileName = string.Format("ApplicationProcessorLog_{0}.log", StartTime.ToString().Replace("/","_").Replace(":","_"));
        public static string ConfigFileName = "config.xml";

        static void Main(string[] args)
        {
            //---------Read configuration

            Configuration Config = new Configuration();
            Config.ReadConfiguration(ConfigFileName);
            


            //----------Setup Logging

            LogWriter logFile = new LogWriter(Config.LogFolder, LogFileName, int.Parse(Config.DaysToKeepLogs));
            logFile.LogMessage(string.Format("{0} started", AppTitle));
            logFile.LogAllProperties(Config);


            
            //---------Setup field name mapping

            FieldMapper fieldMap = new FieldMapper();
            fieldMap.ReadFieldMappings(Config.FieldMapFile);
            logFile.LogAllProperties(fieldMap);
            



            //----------Get Source Data

            logFile.LogMessage("Retrieving Source Data");

            FileProcessor fileProcessor = new FileProcessor()
            {
                DataSource = (DataSources)Enum.Parse(typeof(DataSources), Config.SourceDelimitedSQLXML, true),
                Config = Config
            };
            DataTable sourceData = fileProcessor.FillDataTable();
            


            //----------Load sourceData by Mapped Fields

            logFile.LogMessage("Mapping Source Data to MappedTable");

            DataTable mappedData = fileProcessor.LoadDataTableFromMappedFields(sourceData, fieldMap);

          

            //----------Process Rules

            logFile.LogMessage("Applying Rules");

            RulesProcessor ruleProcessor = new RulesProcessor()
            {
                TableToProcess = mappedData,
                logFile = logFile,
                RulesFile = Config.RulesFile,
                FieldMap = fieldMap,
                Config = Config
            };
            ruleProcessor.ProcessRules();



            //----------Check MTEs

            if (Config.ProcessMTEs == "Y")
            {
                logFile.LogMessage("Processing MTEs");
                MTEProcessor MTE = new MTEProcessor()
                {
                    Config = Config,
                    LogFile = logFile,
                    TableToProcess = mappedData,
                    FieldMap = fieldMap
                };
                MTE.processMTEs();
            }
            else
                logFile.LogMessage("Skipping MTE processing");



            //---------Set Owning Customer

            logFile.LogMessage("Setting owningCustomer Number");

            fileProcessor.setOwningCustomer(mappedData, logFile, fieldMap);
            


            //----------Write ProcessedData File

            logFile.LogMessage("Writing XML File for Importer");
            ProcessedDataWriter processedDataWriter = new ProcessedDataWriter();
            //processedDataWriter.WriteProcessedData(mappedData, Config.OutputFile);
            processedDataWriter.WriteXMLData(mappedData, Config.OutputFile, Config.CollateralsYN, fieldMap);



            //----------Close Connections
            
            logFile.CloseLog();
            Console.ReadLine();

        }
    }
}
