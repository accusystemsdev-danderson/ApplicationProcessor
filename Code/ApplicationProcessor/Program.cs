using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using System.Data.OleDb;
using System.Reflection;



namespace ApplicationProcessor
{
    class Program
    {

        static void Main(string[] args)
        {
                    
            DateTime StartTime = DateTime.Now;
            string LogFileName = string.Format("ApplicationProcessorLog_{0}.log", StartTime.ToString().Replace("/","_").Replace(":","_"));
            string ConfigFileName = "config.xml";
            

            //---------Read configuration

            Configuration Config = new Configuration();
            Config.ReadConfiguration(ConfigFileName);
            

            //----------Setup Logging

            LogWriter logFile = new LogWriter() 
            { 
                LogFilePath = Config.LogFolder, 
                LogFileName = LogFileName, 
            };


            if (!logFile.OpenLog())
            {
                Console.WriteLine("Unable to setup log file at: " + Config.LogFolder + LogFileName);
                Console.WriteLine("Press Enter to Exit");
                Console.ReadLine();
                return;
            };

            logFile.LogMessage(string.Format("{0} version {1} started", Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version));

            logFile.LogMessage("Removing Previous Log Files");
            logFile.RemovePreviousLogFiles(int.Parse(Config.DaysToKeepLogs));

            //---------Get DB connection string from db.xml

            if (!Config.SetupDBConnectionString())
            {
                logFile.LogMessage("Unable to get database configuration from db.xml file at: " + Config.PathToDBXML);
                return;
            }
            
            logFile.LogAllProperties(Config);


            
            //---------Setup field name mapping

            FieldMapper fieldMap = new FieldMapper() 
            { 
                xmlMappingFile = Config.FieldMapFile 
            };
            fieldMap.ReadFieldMappings();
            logFile.LogAllProperties(fieldMap);
            



            //----------Get Source Data

            logFile.LogMessage("Retrieving Source Data");

            FileProcessor fileProcessor = new FileProcessor()
            {
                Config = Config,
                logFile = logFile
            };
            DataTable sourceData = new DataTable();
            if (!fileProcessor.FillDataTable(out sourceData))
            {
                return;
            };
            


            //----------Load sourceData by Mapped Fields

            logFile.LogMessage("Mapping Source Data to MappedTable");

            DataTable mappedData = fileProcessor.LoadDataTableFromMappedFields(sourceData, fieldMap);

          

            //----------Process Rules

            logFile.LogMessage("Applying Rules");

            RulesProcessor ruleProcessor = new RulesProcessor()
            {
                TableToProcess = mappedData,
                logFile = logFile,
                FieldMap = fieldMap,
                Config = Config
            };
            if (!ruleProcessor.ProcessRules())
            {
                return;
            }



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
                if (!MTE.processMTEs())
                {
                    return;
                }
            }
            else
                logFile.LogMessage("Skipping MTE processing");



            //---------Set Owning Customer

            logFile.LogMessage("Setting owningCustomer Number");

            fileProcessor.setOwningCustomer(mappedData, logFile, fieldMap);
            


            //----------Write ProcessedData File

            logFile.LogMessage("Writing XML File for Importer");
            ProcessedDataWriter processedDataWriter = new ProcessedDataWriter() 
            { 
                Config = Config, 
                FieldMap = fieldMap, 
                LogFile = logFile 
            };

            processedDataWriter.WriteXMLData(mappedData);



            //----------Finish up, Close Connections
            
            DateTime finishedTime = DateTime.Now;
            TimeSpan elapsedTime = finishedTime - StartTime;

            logFile.LogMessage();
            logFile.LogMessage("Finished at " + finishedTime.ToShortTimeString() + " Elapsed Time: " + elapsedTime.ToString());
            logFile.CloseLog();
            Console.ReadLine();

        }
    }
}
