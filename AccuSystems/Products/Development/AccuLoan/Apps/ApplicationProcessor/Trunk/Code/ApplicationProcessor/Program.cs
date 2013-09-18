﻿using System;
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
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Validation;
    using System.Diagnostics;
    using System.Xml.Linq;
    using AccuAccount.Data;

    class Program
    {

        static void Main(string[] args)
        {
                    
            DateTime StartTime = DateTime.Now;
            string LogFileName = string.Format("ApplicationProcessorLog_{0}.log", StartTime.ToString().Replace("/","_").Replace(":","_"));
            string ConfigFileName = "config.xml";

            #region initialize

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
            #endregion

            #region getSourceData
            //---------Setup field name mapping

            FieldMapper fieldMap = new FieldMapper() 
            { 
                xmlMappingFile = Config.FieldMapFile 
            };
            fieldMap.ReadFieldMappings();
                        

            //----------Get Source Data

            logFile.LogMessage("Retrieving Source Data");

            Processor appProcessor = new Processor()
            {
                Config = Config,
                LogFile = logFile,
                FieldMap = fieldMap
            };
            DataTable sourceData = new DataTable();
            if (!appProcessor.FillDataTable(out sourceData))
            {
                return;
            };
            #endregion

            #region writeTestData
            //----------Testing Source Data Retreival.  Write data to file and exit.

            if (Config.TestSourceModeYN.ToUpper() == "Y")
            {
                logFile.LogMessage("Writing Source Data to TestSourceData.txt");
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
            //----------Load sourceData by Mapped Fields

            logFile.LogMessage("Mapping Source Data to MappedTable");

            DataTable mappedData = appProcessor.LoadDataTableFromMappedFields(sourceData, fieldMap);

          
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

            appProcessor.setOwningCustomer(mappedData, fieldMap);
            #endregion

            #region writeData
            //----------Write ProcessedData File

            logFile.LogMessage("Writing XML File for Importer");

            if (!appProcessor.WriteXMLData(mappedData))
            {
                logFile.LogMessage("Unable to write XML File");
                return;
            }

            appProcessor.WriteAccountsProcessedLogFile();
            #endregion

            #region finalProcesses

            if (Config.PostProcessingQueryYN == "Y")
            {
                logFile.LogMessage("Running Post Processing Query");
                appProcessor.RunPostProcessingQuery();
            }
            
            DateTime finishedTime = DateTime.Now;
            TimeSpan elapsedTime = finishedTime - StartTime;

            logFile.LogMessage();
            logFile.LogMessage("Finished at " + finishedTime.ToShortTimeString() + " Elapsed Time: " + elapsedTime.ToString());
            logFile.CloseLog();
            #endregion
        }

        private static void InsertLoanApplicationRecords()
        {
            XDocument document = XDocument.Load("");

            foreach (XElement applicationXml in document.Descendants("application"))
            {
                InsertLoanApplicationRecord(applicationXml);
            }
        }

        private static void InsertLoanApplicationRecord(XElement applicationXml)
        {
            LoanApplication application = new LoanApplication
            {
                ApplicationDate = DateTime.Parse(applicationXml.Element("applicationDate").Value),
                CreditAnalysisStatusId = GetCreditAnalysisStatusId(applicationXml.Element("creditAnalysisStatus").Value),
            };

            decimal requestedAmount;
            bool parsed = decimal.TryParse(applicationXml.Element("requestedAmount").Value,
                                           out requestedAmount);

            if (parsed)
            {
                application.RequestedAmount = requestedAmount;
            }

            decimal primaryCollateralValue;
            parsed = decimal.TryParse(applicationXml.Element("primaryCollateralValue").Value,
                out primaryCollateralValue);

            if (parsed)
            {
                application.PrimaryCollateralValue = primaryCollateralValue;
            }

            using (DataContext db = new DataContext())
            {
                db.LoanApplications.Add(application);

                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var error in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in error.ValidationErrors)
                        {
                            Debug.Print("Message: {0}, Column: {1}",
                                validationError.ErrorMessage,
                                validationError.PropertyName);
                        }
                    }
                }
                catch (DbUpdateException ex)
                {
                    
                }
            }
        }

        private static Guid? GetCreditAnalysisStatusId(string creditAnalysisStatusCode)
        {
            using (DataContext db = new DataContext())
            {
                var creditAnalysisStatus = db.CreditAnalysisStatuses
                    .SingleOrDefault(p => p.CreditAnalysisStatusCode.Equals(creditAnalysisStatusCode, 
                        StringComparison.OrdinalIgnoreCase));

                if (creditAnalysisStatus != null)
                {
                    return creditAnalysisStatus.CreditAnalysisStatusId;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
