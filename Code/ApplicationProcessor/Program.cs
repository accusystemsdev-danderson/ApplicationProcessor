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

        static void Main(string[] args)
        {
                    
            DateTime StartTime = DateTime.Now;
            string LogFileName = string.Format("ApplicationProcessorLog_{0}.log", StartTime.ToString().Replace("/","_").Replace(":","_"));
            string ConfigFileName = "config.xml";

            #region initialize

            Configuration.ReadConfiguration(ConfigFileName);

            LogWriter.LogFilePath = Configuration.LogFolder;
            LogWriter.LogFileName = LogFileName;
            
            if (!LogWriter.OpenLog())
            {
                Console.WriteLine("Unable to setup log file at: " + Configuration.LogFolder + LogFileName);
                Console.WriteLine("Press Enter to Exit");
                Console.ReadLine();
                return;
            };

            LogWriter.LogMessage(string.Format("{0} version {1} started", Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version));

            LogWriter.LogMessage("Removing Previous Log Files");
            LogWriter.RemovePreviousLogFiles(int.Parse(Configuration.DaysToKeepLogs));
            
            LogWriter.LogConfigProperties();

            #endregion

            #region getSourceData

            FieldMap.XmlMappingFile = Configuration.FieldMapFile;

            FieldMap.ReadFieldMappings();

            LogWriter.LogMessage("Retrieving Source Data");

            Processor appProcessor = new Processor();

            DataTable sourceData = new DataTable();
            if (!appProcessor.FillDataTable(out sourceData))
            {
                return;
            };
            #endregion

            #region writeTestData

            if (Configuration.TestSourceModeYN.ToUpper() == "Y")
            {
                LogWriter.LogMessage("Writing Source Data to TestSourceData.txt");
                appProcessor.WriteDataTableToFile(sourceData, "TestSourceData.txt");
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("Source data written to \"TestSourceData.txt\".  Press any key to exit..");
                Console.WriteLine();
                Console.ReadLine();
                return;
            }
            #endregion

            #region processData
            
            LogWriter.LogMessage("Mapping Source Data to MappedTable");

            List<SourceRecord> sourceRecords = new List<SourceRecord>();
            foreach (DataRow sourceRow in sourceData.Rows)
            {
                sourceRecords.Add(appProcessor.ReadSourceRecordFromDataRow(sourceRow));
            }

            LogWriter.LogMessage("Applying Rules");


            if (!RuleProcessor.LoadRulesFromFile())
            {
                LogWriter.LogMessage("Unable to read rules file.");
                return;
            }
            else
            {
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
                        MTEProcessor.ProcessMTEs(record);
                    }
                }                
            }

            LogWriter.LogMessage("Setting owningCustomer Number");

            appProcessor.SetOwningCustomer(sourceRecords);
            #endregion

            #region writeData
            
            LogWriter.LogMessage("Writing XML File for Importer");

            if (!appProcessor.WriteXMLData(sourceRecords))
            {
                LogWriter.LogMessage("Unable to write XML File");
                return;
            }

            appProcessor.WriteAccountsProcessedLogFile();
            
            #endregion

            #region LegacyImport

            if (Configuration.UseLegacyImportYN == "Y")
            {
                LogWriter.LogMessage("Starting Importer");

                Process importer = new Process();
                importer.StartInfo.FileName = Path.Combine(Configuration.ImporterPath, "accuaccount.importer.exe");
                importer.StartInfo.WorkingDirectory = Configuration.ImporterPath;
                importer.Start();
                importer.WaitForExit();
                LogWriter.LogMessage("Importer Complete");

                LogWriter.LogMessage("Writing Application Records To Database");
                ApplicationRecordsWriter appWriter = new ApplicationRecordsWriter();
                
                appWriter.InsertLoanApplicationRecords(Configuration.OutputFile);
            }
            #endregion

            #region finalProcesses

            if (Configuration.PostProcessingQueryYN == "Y")
            {
                LogWriter.LogMessage("Running Post Processing Query");
                appProcessor.RunPostProcessingQuery();
            }
            
            DateTime finishedTime = DateTime.Now;
            TimeSpan elapsedTime = finishedTime - StartTime;

            LogWriter.LogMessage();
            LogWriter.LogMessage("Finished at " + finishedTime.ToShortTimeString() + " Elapsed Time: " + elapsedTime.ToString());
            LogWriter.CloseLog();
            #endregion
        }


    }
}
