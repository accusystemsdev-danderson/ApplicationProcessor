//-----------------------------------------------------------------------------
// <copyright file="Processor.cs" company="AccuSystems LLC">
//     Copyright (c) AccuSystems.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace ApplicationProcessor
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.OleDb;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Xml;
    
    /// <summary>
    /// Functions for processing source data
    /// </summary>
    class Processor
    {
        private StringBuilder accountsProcessed;

        /// <summary>
        /// Retrieves source data and loads records into a DataTable
        /// </summary>
        /// <param name="dataTable">The DataTable to hold the source data</param>
        /// <returns>True upon successful completion</returns>
        public bool FillDataTable(out DataTable dataTable)
        {
            dataTable = new DataTable();
            bool success = false;

            try
            {

                switch (Configuration.SourceDelimitedSqlXml.ToUpper())
                {
                    case "DELIMITED":
                        const string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=.\\;Extended Properties=\"Text;HDR=YES;FMT=Delimited\";";
                        OleDbConnection fileConn = new OleDbConnection(connectionString);
                        OleDbDataAdapter fileAdapter = new OleDbDataAdapter("select * from " + Configuration.SourceFile, fileConn);
                        fileAdapter.Fill(dataTable);
                        break;

                    case "SQL":
                        string dbConnectionString = Configuration.SourceSqlConnectionString;
                        string sqlQuery = File.ReadAllText(Configuration.SourceSqlQueryFile);
                        SqlConnection connection = new SqlConnection(dbConnectionString);
                        connection.Open();
                        SqlDataAdapter dadapter = new SqlDataAdapter();
                        dadapter.SelectCommand = new SqlCommand(sqlQuery, connection);
                        dadapter.Fill(dataTable);
                        connection.Close();
                        break;
                    case "XML":
                        LogWriter.LogMessage("XML Data Source not implemented");
                        break;
                }
                success = true;
            }
            catch (Exception e)
            {
                LogWriter.LogMessage(e.ToString());
                LogWriter.LogMessage();
                LogWriter.LogMessage(string.Format("Unable to retrieve source data: {0}",
                                                   e.Message));
            }

            return success;
        }

        /// <summary>
        /// Maps a source data row into a SourceRecord object
        /// </summary>
        /// <param name="sourceRow">The source row to map</param>
        /// <returns>A SourceRecord object containing data from the sourceRow</returns>
        public SourceRecord ReadSourceRecordFromDataRow(DataRow sourceRow)
        { 
            SourceRecord newRecord = new SourceRecord();
            foreach (PropertyInfo prop in newRecord.GetType().GetProperties())
            {
                string propertyName = prop.Name;
                if (prop.Name != "IgnoreRecord")
                {
                    var mappedField = typeof(FieldMap).GetProperty(propertyName + "MappedField").GetValue(null, null);
                    string mappedFieldValue = mappedField.ToString();
                    if (mappedFieldValue != "")
                    {
                        prop.SetValue(newRecord, sourceRow[mappedFieldValue].ToString(), null);
                    }
                    else
                    {
                        prop.SetValue(newRecord, string.Empty, null);
                    }
                }
            }
            return newRecord;
        }

        /// <summary>
        /// Writes the contents of a DataTable to a CSV file
        /// </summary>
        /// <param name="dataToWrite">The DataTable to write</param>
        /// <param name="processedDataFile">The file name of the CSV file to write</param>
        public void WriteDataTableToFile(DataTable dataToWrite, string processedDataFile)
        {
            if (dataToWrite.Rows.Count == 0)
            {
                LogWriter.LogMessage("No data to write");
                return;
            }

            StringBuilder record = new StringBuilder();

            //Write Header Row

            foreach (DataColumn column in dataToWrite.Columns)
            {
                record.Append(column.ColumnName + "\t");
            }

            record.Remove(record.Length - 1, 1);
            record.Append(Environment.NewLine);

            //Write Data Rows

            foreach (DataRow row in dataToWrite.Rows)
            {
                foreach (DataColumn column in dataToWrite.Columns)
                {
                    record.Append(row[column] + "\t");
                }

                record.Remove(record.Length - 1, 1);
                record.Append(Environment.NewLine);

            }

            File.WriteAllText(processedDataFile, record.ToString());
        }

        /// <summary>
        /// Writes the contents of a DataTable to a standard imorter XML file
        /// </summary>
        /// <param name="sourceRecords">The list of <see cref="SourceRecord"/>to process</param>
        /// <returns>True upon successful completion</returns>
        public bool WriteXMLData(List<SourceRecord> sourceRecords)
        {
            XmlWriterSettings xmlRules = new XmlWriterSettings();
            xmlRules.Indent = true;
            xmlRules.NewLineOnAttributes = true;
            xmlRules.OmitXmlDeclaration = true;
            xmlRules.Encoding = Encoding.Default;

            //List of postProcessingField for use in post processing query.  Start with single quote for first record
            accountsProcessed = new StringBuilder();
            accountsProcessed.Append("'");

            var recordsToWrite = (from r in sourceRecords
                                 where r.IgnoreRecord == false
                                 select r).ToList();

            var uniqueCustomers = (from c in recordsToWrite
                                   select c.CustomerNumber).Distinct().ToList();

            if (!uniqueCustomers.Any())
            {
                LogWriter.LogMessage("No records to write");
                return false;
            }
            try
            {
                using (XmlWriter xmlOut = XmlWriter.Create(Configuration.OutputFile, xmlRules))
                {
                    xmlOut.WriteStartDocument();
                    xmlOut.WriteStartElement("AccuSystems");

                    foreach (string customerNumber in uniqueCustomers)
                    {
                        SourceRecord customer = recordsToWrite.First(cust => cust.CustomerNumber == customerNumber);
                        xmlOut.WriteStartElement("customer");

                        xmlOut.WriteElementString(FieldMap.CustomerNumberFieldName, customer.CustomerNumber);
                        xmlOut.WriteElementString(FieldMap.TaxIdFieldName, customer.TaxId);
                        xmlOut.WriteElementString(FieldMap.CustomerNameFieldName, customer.CustomerName);
                        xmlOut.WriteElementString(FieldMap.BusinessNameFieldName, customer.BusinessName);
                        xmlOut.WriteElementString(FieldMap.CustomerFirstNameFieldName, customer.CustomerFirstName);
                        xmlOut.WriteElementString(FieldMap.CustomerMiddleNameFieldName, customer.CustomerMiddleName);
                        xmlOut.WriteElementString(FieldMap.CustomerLastNameFieldName, customer.CustomerLastName);
                        xmlOut.WriteElementString(FieldMap.CustomerTypeCodeFieldName, customer.CustomerTypeCode);
                        xmlOut.WriteElementString(FieldMap.BankCodeFieldName, customer.BankCode);
                        xmlOut.WriteElementString(FieldMap.EmployeeFieldName, customer.Employee);
                        xmlOut.WriteElementString(FieldMap.CustomerBranchFieldName, customer.CustomerBranch);
                        xmlOut.WriteElementString(FieldMap.CustomerOfficerCodeFieldName, customer.CustomerOfficerCode);
                        xmlOut.WriteElementString(FieldMap.Address1FieldName, customer.Address1);
                        xmlOut.WriteElementString(FieldMap.Address2FieldName, customer.Address2);
                        xmlOut.WriteElementString(FieldMap.CityFieldName, customer.City);
                        xmlOut.WriteElementString(FieldMap.StateFieldName, customer.State);
                        xmlOut.WriteElementString(FieldMap.ZipCodeFieldName, customer.ZipCode);
                        xmlOut.WriteElementString(FieldMap.HomePhoneFieldName, customer.HomePhone);
                        xmlOut.WriteElementString(FieldMap.WorkPhoneFieldName, customer.WorkPhone);
                        xmlOut.WriteElementString(FieldMap.MobilePhoneFieldName, customer.MobilePhone);
                        xmlOut.WriteElementString(FieldMap.FaxFieldName, customer.Fax);
                        xmlOut.WriteElementString(FieldMap.EmailFieldName, customer.Email);
                        xmlOut.WriteElementString(FieldMap.ClassificationCodeFieldName, customer.ClassificationCode);
                        xmlOut.WriteElementString(FieldMap.CustomerStatusFieldName, customer.CustomerStatus);

                        var uniqueAccounts = (from a in recordsToWrite
                                              where a.CustomerNumber == customerNumber
                                              select a.LoanNumber).Distinct();

                        foreach (string accountNumber in uniqueAccounts)
                        {
                            SourceRecord accountRow = recordsToWrite.Where(acct => acct.CustomerNumber == customerNumber && acct.LoanNumber == accountNumber).First();
                            
                            accountsProcessed.Append(accountRow.PostProcessingField + "', '");

                            xmlOut.WriteStartElement("loan");
                            xmlOut.WriteElementString(FieldMap.LoanNumberFieldName, accountRow.LoanNumber);
                            xmlOut.WriteElementString(FieldMap.CustomerNumberFieldName, accountRow.CustomerNumber);
                            xmlOut.WriteElementString(FieldMap.AccountClassFieldName, accountRow.AccountClass);
                            xmlOut.WriteElementString(FieldMap.LoanStatusCodeFieldName, accountRow.LoanStatusCode);
                            xmlOut.WriteElementString(FieldMap.LoanOfficerCodeFieldName, accountRow.LoanOfficerCode);
                            xmlOut.WriteElementString(FieldMap.LoanTypeCodeFieldName, accountRow.LoanTypeCode);
                            xmlOut.WriteElementString(FieldMap.LoanClosedFieldName, accountRow.LoanClosed);
                            xmlOut.WriteElementString(FieldMap.LoanAmountFieldName, accountRow.LoanAmount);
                            xmlOut.WriteElementString(FieldMap.LoanOriginationDateFieldName, accountRow.LoanOriginationDate);
                            xmlOut.WriteElementString(FieldMap.LoanDescriptionFieldName, accountRow.LoanDescription);
                            xmlOut.WriteElementString(FieldMap.BorrowerTypeFieldName, accountRow.BorrowerType);
                            xmlOut.WriteElementString(FieldMap.OwningCustomerNumberFieldName, accountRow.OwningCustomerNumber);
                            xmlOut.WriteElementString(FieldMap.LoanBranchFieldName, accountRow.LoanBranch);
                            xmlOut.WriteElementString(FieldMap.CoreClassCodeFieldName, accountRow.CoreClassCode);
                            xmlOut.WriteElementString(FieldMap.CoreCollCodeFieldName, accountRow.CoreCollCode);
                            xmlOut.WriteElementString(FieldMap.CoreCollateralCodeFieldName, accountRow.CoreCollateralCode);
                            xmlOut.WriteElementString(FieldMap.CorePurposeCodeFieldName, accountRow.CorePurposeCode);
                            xmlOut.WriteElementString(FieldMap.CoreTypeCodeFieldName, accountRow.CoreTypeCode);
                            xmlOut.WriteElementString(FieldMap.CommitmentAmountFieldName, accountRow.CommitmentAmount);
                            xmlOut.WriteElementString(FieldMap.CoreNaicsCodeFieldName, accountRow.CoreNaicsCode);
                            xmlOut.WriteElementString(FieldMap.LoanMaturityDateFieldName, accountRow.LoanMaturityDate);
                            xmlOut.WriteElementString(FieldMap.LoanClassificationCodeFieldName, accountRow.LoanClassificationCode);

                            {
                                xmlOut.WriteStartElement("application");
                                xmlOut.WriteElementString("applicationNumber", accountRow.LoanNumber);
                                xmlOut.WriteElementString(FieldMap.ApplicationDateFieldName, accountRow.ApplicationDate);
                                xmlOut.WriteElementString(FieldMap.CreditAnalysisStatusFieldName, accountRow.CreditAnalysisStatus);
                                xmlOut.WriteElementString(FieldMap.RequestedAmountFieldName, accountRow.RequestedAmount);
                                xmlOut.WriteElementString(FieldMap.PrimaryCollateralValueFieldName, accountRow.PrimaryCollateralValue);
                                xmlOut.WriteElementString(FieldMap.FICOFieldName, accountRow.FICO);
                                xmlOut.WriteElementString(FieldMap.ValuationDateFieldName, accountRow.ValuationDate);
                                xmlOut.WriteElementString(FieldMap.InterestRateFieldName, accountRow.InterestRate);
                                xmlOut.WriteElementString(FieldMap.ProbabilityFieldName, accountRow.Probability);
                                xmlOut.WriteElementString(FieldMap.EstimatedCloseDateFieldName, accountRow.EstimatedCloseDate);
                                xmlOut.WriteStartElement(FieldMap.AssignedLenderFieldName);
                                xmlOut.WriteAttributeString("type", accountRow.AssignedLenderType);
                                xmlOut.WriteValue(accountRow.AssignedLender);
                                xmlOut.WriteEndElement();
                                xmlOut.WriteStartElement(FieldMap.AssignedAnalystFieldName);
                                xmlOut.WriteAttributeString("type", accountRow.AssignedAnalystType);
                                xmlOut.WriteValue(accountRow.AssignedAnalyst);
                                xmlOut.WriteEndElement();
                                xmlOut.WriteStartElement(FieldMap.AssignedLoanProcessorFieldName);
                                xmlOut.WriteAttributeString("type", accountRow.AssignedLoanProcessorType);
                                xmlOut.WriteValue(accountRow.AssignedLoanProcessor);
                                xmlOut.WriteEndElement();
                                xmlOut.WriteElementString(FieldMap.ApplicationLockedFieldName, accountRow.ApplicationLocked);

                                {
                                    xmlOut.WriteStartElement("approval");
                                    xmlOut.WriteElementString(FieldMap.ApprovalStatusFieldName, accountRow.ApprovalStatus);
                                    xmlOut.WriteElementString(FieldMap.OriginatingUserFieldName, accountRow.OriginatingUser);
                                    xmlOut.WriteStartElement(FieldMap.AssignedApproverFieldName);
                                    xmlOut.WriteAttributeString("type", accountRow.AssignedApproverType);
                                    xmlOut.WriteValue(accountRow.AssignedApprover);
                                    xmlOut.WriteEndElement();

                                    xmlOut.WriteEndElement();
                                }

                                xmlOut.WriteEndElement();
                            }

                            xmlOut.WriteEndElement();

                            if (Configuration.CollateralsYN == "Y")
                            {
                                var collateralRows = recordsToWrite.Where(acct => acct.CustomerNumber == customerNumber &&
                                    acct.LoanNumber == accountNumber && acct.BorrowerType == "");
                                
                                foreach (SourceRecord collateralRow in collateralRows)
                                {
                                    xmlOut.WriteStartElement("loan");
                                    xmlOut.WriteElementString(FieldMap.LoanNumberFieldName, collateralRow.CollateralLoanNumber);
                                    xmlOut.WriteElementString(FieldMap.CustomerNumberFieldName, collateralRow.CustomerNumber);
                                    xmlOut.WriteElementString(FieldMap.AccountClassFieldName, collateralRow.AccountClass);
                                    xmlOut.WriteElementString(FieldMap.LoanStatusCodeFieldName, accountRow.LoanStatusCode);
                                    xmlOut.WriteElementString(FieldMap.LoanOfficerCodeFieldName, accountRow.LoanOfficerCode);
                                    xmlOut.WriteElementString(FieldMap.LoanTypeCodeFieldName, collateralRow.CollateralLoanTypeCode);
                                    xmlOut.WriteElementString(FieldMap.LoanClosedFieldName, accountRow.LoanClosed);
                                    xmlOut.WriteElementString(FieldMap.LoanAmountFieldName, accountRow.LoanAmount);
                                    xmlOut.WriteElementString(FieldMap.LoanOriginationDateFieldName, accountRow.LoanOriginationDate);
                                    xmlOut.WriteElementString(FieldMap.BorrowerTypeFieldName, accountRow.BorrowerType);
                                    xmlOut.WriteElementString(FieldMap.ParentLoanNumberFieldName, collateralRow.ParentLoanNumber);
                                    xmlOut.WriteElementString(FieldMap.LoanDescriptionFieldName, collateralRow.CollateralDescription);
                                    xmlOut.WriteElementString(FieldMap.LoanBranchFieldName, accountRow.LoanBranch);
                                    xmlOut.WriteElementString(FieldMap.CoreClassCodeFieldName, accountRow.CoreClassCode);
                                    xmlOut.WriteElementString(FieldMap.CoreCollCodeFieldName, accountRow.CoreCollCode);
                                    xmlOut.WriteElementString(FieldMap.CoreCollateralCodeFieldName, accountRow.CoreCollateralCode);
                                    xmlOut.WriteElementString(FieldMap.CorePurposeCodeFieldName, accountRow.CorePurposeCode);
                                    xmlOut.WriteElementString(FieldMap.CoreTypeCodeFieldName, accountRow.CoreTypeCode);
                                    xmlOut.WriteElementString(FieldMap.CommitmentAmountFieldName, accountRow.CommitmentAmount);
                                    xmlOut.WriteElementString(FieldMap.CoreNaicsCodeFieldName, accountRow.CoreNaicsCode);
                                    xmlOut.WriteElementString(FieldMap.LoanMaturityDateFieldName, accountRow.LoanMaturityDate);
                                    xmlOut.WriteElementString(FieldMap.LoanClassificationCodeFieldName, accountRow.LoanClassificationCode);
                                    xmlOut.WriteEndElement();
                                }
                            }

                        }

                        xmlOut.WriteEndElement();
                    }

                    //remove final comma, space, and single quote from list
                    accountsProcessed.Remove(accountsProcessed.Length - 3, 3);
                }
            }
            catch (Exception e)
            {
                LogWriter.LogMessage(e.ToString());
                LogWriter.LogMessage();
                LogWriter.LogMessage("Unable to write XML file");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Sets the OwningCustomer field for collateral records
        /// </summary>
        /// <param name="sourceRecords">The list of <see cref="SourceRecord"/> to process</param>
        public void SetOwningCustomer (List<SourceRecord> sourceRecords)
        {
            foreach(SourceRecord record in sourceRecords)
            {
                var primaryRows = (from r in sourceRecords
                                   where r.LoanNumber == record.LoanNumber &&
                                         r.BorrowerType == ""
                                   select r).ToArray();
                                  
                var distinctPrimaries = (from p in primaryRows
                                        select p.CustomerNumber).Distinct().Count();
                if (distinctPrimaries != 1)
                {
                    record.IgnoreRecord = true;
                    LogWriter.LogMessage(string.Format("Wrong number of primary relationships for account {0} - primary relationships: {1}",
                        record.LoanNumber, distinctPrimaries));
                }
                else if (record.BorrowerType != "")
                {
                    record.OwningCustomerNumber = primaryRows.First().CustomerNumber;
                    
                    if (primaryRows.Any(p => p.IgnoreRecord))
                    {
                        record.IgnoreRecord = true;
                        LogWriter.LogMessage(string.Format("Excluded relationship for account: {0} customer number: {1} - Primary customer record for the account already excluded.",
                                                           record.LoanNumber, 
                                                           record.CustomerNumber));
                    }
                }
            }
        }

        /// <summary>
        /// Writes a log file of all account numbers written to the XML file
        /// </summary>
        public void WriteAccountsProcessedLogFile()
        {
            string[] fileNameParts = Configuration.AccountsProcessedLogFile.Split('.');
            string path = string.Format("{0}{1}_{2}.{3}", Configuration.LogFolder, fileNameParts[0], DateTime.Now.ToString().Replace("/", "_").Replace(":", "_"), fileNameParts[1]);
            string textToWrite = accountsProcessed.ToString().Replace(", ", "\r\n");
            File.WriteAllText(path, textToWrite);
        }

        /// <summary>
        /// Runs a post processing SQL query using the SourceSQLConnectionString, used for clearing flags on source database records
        /// </summary>
        public void RunPostProcessingQuery()
        {
            try
            {
                string sqlQuery = File.ReadAllText(Configuration.PostProcessingSqlQueryFile).Replace("%%A", accountsProcessed.ToString());
                int recordsAffected = Utils.ExecuteSQLQuery(Configuration.SourceSqlConnectionString, sqlQuery);
                LogWriter.LogMessage(recordsAffected + " Records Affected.");
            }
            catch (Exception e)
            {
                LogWriter.LogMessage(e.ToString());
                LogWriter.LogMessage();
                LogWriter.LogMessage("Unable to execute post processing query");

            }
        }
    }
}
