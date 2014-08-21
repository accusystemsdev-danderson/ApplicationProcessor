//-----------------------------------------------------------------------------
// <copyright file="Program.cs" company="AccuSystems LLC">
//     Copyright (c) AccuSystems.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace ApplicationProcessor
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;

    class Program
    {
        static void Main()
        {
                    
            DateTime startTime = DateTime.Now;
            string logFileName = string.Format("ApplicationProcessorLog_{0}.log",
                                               startTime.ToString("MM_dd_yyyy H_mm_ss"));

            const string configFileName = "config.xml";

            Configuration.ReadConfiguration(configFileName);
            InitializeLogWriter(logFileName);
            LogWriter.LogConfigProperties();


            FieldMap.XmlMappingFile = Configuration.FieldMapFile;
            FieldMap.ReadFieldMappings();

            LogWriter.LogMessage("Retrieving Source Data");

            var appProcessor = new Processor();

            var sourceData = new DataTable();
            if (!appProcessor.FillDataTable(out sourceData))
            {
                return;
            }
            
            if (Configuration.TestSourceModeYN.ToUpper() == "Y")
            {
                WriteSourceData(appProcessor, sourceData);
                return;
            }

            List<SourceRecord> sourceRecords;
            if (!ProcessSourceData(sourceData,
                                   appProcessor,
                                   out sourceRecords))
            {
                return;
            }

            LogWriter.LogMessage("Writing XML File for Importer");

            if (!appProcessor.WriteXMLData(sourceRecords))
            {
                LogWriter.LogMessage("Unable to write XML File");
                return;
            }

            appProcessor.WriteAccountsProcessedLogFile();

            if (Configuration.UseLegacyImportYN == "Y")
            {
                RunImporter();
            }
            
            if (Configuration.PostProcessingQueryYN == "Y")
            {
                LogWriter.LogMessage("Running Post Processing Query");
                appProcessor.RunPostProcessingQuery();
            }
            
            DateTime finishedTime = DateTime.Now;
            TimeSpan elapsedTime = finishedTime - startTime;

            LogWriter.LogMessage();
            LogWriter.LogMessage(string.Format("Finished at {0} Elapsed Time: {1}",
                                               finishedTime.ToShortTimeString(),
                                               elapsedTime));
            LogWriter.CloseLog();
        }

        /// <summary>
        /// Runs AccuAccount Import to create customer and loan records for the applications
        /// </summary>
        private static void RunImporter()
        {
            LogWriter.LogMessage("Starting Importer");

            var importer = new Process
                           {
                               StartInfo =
                               {
                                   FileName            = Path.Combine(Configuration.ImporterPath, "accuaccount.importer.exe"),
                                   WorkingDirectory    = Configuration.ImporterPath,
                                   Arguments           = string.Format("/IN:{0}",
                                                                       Path.GetFileName(Configuration.OutputFile))
                               }
                           };

            importer.Start();
            importer.WaitForExit();
            LogWriter.LogMessage("Importer Complete");

            LogWriter.LogMessage("Writing Application Records To Database");
            var applicationRecordsWriter = new ApplicationRecordsWriter();

            applicationRecordsWriter.InsertLoanApplicationRecords(Configuration.OutputFile);
        }

        /// <summary>
        /// Creates a list of <see cref="SourceRecord"/> from a data table, and applies processing rules.
        /// </summary>
        /// <param name="sourceData">The source <see cref="DataTable"/></param>
        /// <param name="appProcessor">The <see cref="Processor"/> instance for rule processing</param>
        /// <param name="sourceRecords">The list of <see cref="SourceRecord"/> to return</param>
        /// <returns>A list of processed <see cref="SourceRecord"/></returns>
        private static bool ProcessSourceData(DataTable sourceData, Processor appProcessor, out List<SourceRecord> sourceRecords)
        {
            LogWriter.LogMessage("Mapping Source Data to MappedTable");

            sourceRecords = new List<SourceRecord>();
            foreach (DataRow sourceRow in sourceData.Rows)
            {
                sourceRecords.Add(appProcessor.ReadSourceRecordFromDataRow(sourceRow));
            }

            LogWriter.LogMessage("Applying Rules");

            if (!RuleProcessor.LoadRulesFromFile())
            {
                LogWriter.LogMessage("Unable to read rules file.");
                return false;
            }
            
            foreach (SourceRecord record in sourceRecords)
            {
                RuleProcessor.ProcessRules(record);
                if (!record.IgnoreRecord)
                {
                    RuleProcessor.VerifyRequiredFields(record);
                }
                if (Configuration.ProcessExistingAccounts.ToUpper() != "Y" && !record.IgnoreRecord)
                {
                    RuleProcessor.CheckIfAccountExistsInDatabase(record);
                }
                if (Configuration.ProcessMTEs == "Y" && !record.IgnoreRecord)
                {
                    MteProcessor.ProcessMte(record);
                }
            }

            LogWriter.LogMessage("Setting owningCustomer Number");

            appProcessor.SetOwningCustomer(sourceRecords);
            return true;
        }

        /// <summary>
        /// Writes the data in a <see cref="DataTable"/> to file
        /// </summary>
        /// <param name="appProcessor"></param>
        /// <param name="sourceData"></param>
        /// <devdoc>
        /// This method is used to verify that data is properly read from the source.  
        /// This is helpfull when the source data is read from SQL
        /// </devdoc>
        private static void WriteSourceData(Processor appProcessor, DataTable sourceData)
        {
            LogWriter.LogMessage("Writing Source Data to TestSourceData.txt");
            appProcessor.WriteDataTableToFile(sourceData, "TestSourceData.txt");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Source data written to \"TestSourceData.txt\".  Press any key to exit..");
            Console.WriteLine();
            Console.ReadLine();
        }

        /// <summary>
        /// Initializes the <see cref="LogWriter"/>
        /// </summary>
        /// <param name="logFileName">The name of the log file to write</param>
        private static void InitializeLogWriter(string logFileName)
        {
            LogWriter.LogFilePath = Configuration.LogFolder;
            LogWriter.LogFileName = logFileName;

            if (!LogWriter.OpenLog())
            {
                Console.WriteLine("Unable to setup log file at: {0}",
                                  Path.Combine(Configuration.LogFolder, logFileName));
                Console.WriteLine("Press Enter to Exit");
                Console.ReadLine();
            }
            
            LogWriter.LogMessage(string.Format("{0} version {1} started",
                                               Assembly.GetExecutingAssembly().GetName().Name,
                                               Assembly.GetExecutingAssembly().GetName().Version));

            LogWriter.LogMessage("Removing Previous Log Files");
            LogWriter.RemovePreviousLogFiles(int.Parse(Configuration.DaysToKeepLogs));
        }
    }
}
