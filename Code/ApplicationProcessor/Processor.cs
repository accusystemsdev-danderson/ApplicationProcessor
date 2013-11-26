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
                        string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=.\\;Extended Properties=\"Text;HDR=YES;FMT=Delimited\";";
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
            //--------Used for testing
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
                    record.Append(row[column].ToString() + "\t");
                }

                record.Remove(record.Length - 1, 1);
                record.Append(Environment.NewLine);

            }

            //-------Write to file

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
                using (XmlWriter XMLOut = XmlWriter.Create(Configuration.OutputFile, xmlRules))
                {
                    XMLOut.WriteStartDocument();
                    XMLOut.WriteStartElement("AccuSystems");

                    foreach (string customerNumber in uniqueCustomers)
                    {
                        SourceRecord customer = recordsToWrite.First(cust => cust.CustomerNumber == customerNumber);
                        XMLOut.WriteStartElement("customer");

                        XMLOut.WriteElementString(FieldMap.CustomerNumberFieldName, customer.CustomerNumber);
                        XMLOut.WriteElementString(FieldMap.TaxIdFieldName, customer.TaxId);
                        XMLOut.WriteElementString(FieldMap.CustomerNameFieldName, customer.CustomerName);
                        XMLOut.WriteElementString(FieldMap.BusinessNameFieldName, customer.BusinessName);
                        XMLOut.WriteElementString(FieldMap.CustomerFirstNameFieldName, customer.CustomerFirstName);
                        XMLOut.WriteElementString(FieldMap.CustomerMiddleNameFieldName, customer.CustomerMiddleName);
                        XMLOut.WriteElementString(FieldMap.CustomerLastNameFieldName, customer.CustomerLastName);
                        XMLOut.WriteElementString(FieldMap.CustomerTypeCodeFieldName, customer.CustomerTypeCode);
                        XMLOut.WriteElementString(FieldMap.BankCodeFieldName, customer.BankCode);
                        XMLOut.WriteElementString(FieldMap.EmployeeFieldName, customer.Employee);
                        XMLOut.WriteElementString(FieldMap.CustomerBranchFieldName, customer.CustomerBranch);
                        XMLOut.WriteElementString(FieldMap.CustomerOfficerCodeFieldName, customer.CustomerOfficerCode);
                        XMLOut.WriteElementString(FieldMap.Address1FieldName, customer.Address1);
                        XMLOut.WriteElementString(FieldMap.Address2FieldName, customer.Address2);
                        XMLOut.WriteElementString(FieldMap.CityFieldName, customer.City);
                        XMLOut.WriteElementString(FieldMap.StateFieldName, customer.State);
                        XMLOut.WriteElementString(FieldMap.ZipCodeFieldName, customer.ZipCode);
                        XMLOut.WriteElementString(FieldMap.HomePhoneFieldName, customer.HomePhone);
                        XMLOut.WriteElementString(FieldMap.WorkPhoneFieldName, customer.WorkPhone);
                        XMLOut.WriteElementString(FieldMap.MobilePhoneFieldName, customer.MobilePhone);
                        XMLOut.WriteElementString(FieldMap.FaxFieldName, customer.Fax);
                        XMLOut.WriteElementString(FieldMap.EmailFieldName, customer.Email);
                        XMLOut.WriteElementString(FieldMap.ClassificationCodeFieldName, customer.ClassificationCode);
                        XMLOut.WriteElementString(FieldMap.CustomerStatusFieldName, customer.CustomerStatus);

                        var uniqueAccounts = (from a in recordsToWrite
                                              where a.CustomerNumber == customerNumber
                                              select a.LoanNumber).Distinct();

                        foreach (string accountNumber in uniqueAccounts)
                        {
                            SourceRecord accountRow = recordsToWrite.Where(acct => acct.CustomerNumber == customerNumber && acct.LoanNumber == accountNumber).First();
                            
                            accountsProcessed.Append(accountRow.PostProcessingField + "', '");

                            XMLOut.WriteStartElement("loan");
                            XMLOut.WriteElementString(FieldMap.LoanNumberFieldName, accountRow.LoanNumber);
                            XMLOut.WriteElementString(FieldMap.CustomerNumberFieldName, accountRow.CustomerNumber);
                            XMLOut.WriteElementString(FieldMap.AccountClassFieldName, accountRow.AccountClass);
                            XMLOut.WriteElementString(FieldMap.LoanStatusCodeFieldName, accountRow.LoanStatusCode);
                            XMLOut.WriteElementString(FieldMap.LoanOfficerCodeFieldName, accountRow.LoanOfficerCode);
                            XMLOut.WriteElementString(FieldMap.LoanTypeCodeFieldName, accountRow.LoanTypeCode);
                            XMLOut.WriteElementString(FieldMap.LoanClosedFieldName, accountRow.LoanClosed);
                            XMLOut.WriteElementString(FieldMap.LoanAmountFieldName, accountRow.LoanAmount);
                            XMLOut.WriteElementString(FieldMap.LoanOriginationDateFieldName, accountRow.LoanOriginationDate);
                            XMLOut.WriteElementString(FieldMap.LoanDescriptionFieldName, accountRow.LoanDescription);
                            XMLOut.WriteElementString(FieldMap.BorrowerTypeFieldName, accountRow.BorrowerType);
                            XMLOut.WriteElementString(FieldMap.OwningCustomerNumberFieldName, accountRow.OwningCustomerNumber);
                            XMLOut.WriteElementString(FieldMap.LoanBranchFieldName, accountRow.LoanBranch);
                            XMLOut.WriteElementString(FieldMap.CoreClassCodeFieldName, accountRow.CoreClassCode);
                            XMLOut.WriteElementString(FieldMap.CoreCollCodeFieldName, accountRow.CoreCollCode);
                            XMLOut.WriteElementString(FieldMap.CoreCollateralCodeFieldName, accountRow.CoreCollateralCode);
                            XMLOut.WriteElementString(FieldMap.CorePurposeCodeFieldName, accountRow.CorePurposeCode);
                            XMLOut.WriteElementString(FieldMap.CoreTypeCodeFieldName, accountRow.CoreTypeCode);
                            XMLOut.WriteElementString(FieldMap.CommitmentAmountFieldName, accountRow.CommitmentAmount);
                            XMLOut.WriteElementString(FieldMap.CoreNaicsCodeFieldName, accountRow.CoreNaicsCode);
                            XMLOut.WriteElementString(FieldMap.LoanMaturityDateFieldName, accountRow.LoanMaturityDate);
                            XMLOut.WriteElementString(FieldMap.LoanClassificationCodeFieldName, accountRow.LoanClassificationCode);

                            {
                                XMLOut.WriteStartElement("application");
                                XMLOut.WriteElementString("applicationNumber", accountRow.LoanNumber);
                                XMLOut.WriteElementString(FieldMap.ApplicationDateFieldName, accountRow.ApplicationDate);
                                XMLOut.WriteElementString(FieldMap.CreditAnalysisStatusFieldName, accountRow.CreditAnalysisStatus);
                                XMLOut.WriteElementString(FieldMap.RequestedAmountFieldName, accountRow.RequestedAmount);
                                XMLOut.WriteElementString(FieldMap.PrimaryCollateralValueFieldName, accountRow.PrimaryCollateralValue);
                                XMLOut.WriteElementString(FieldMap.FICOFieldName, accountRow.FICO);
                                XMLOut.WriteElementString(FieldMap.ValuationDateFieldName, accountRow.ValuationDate);
                                XMLOut.WriteElementString(FieldMap.InterestRateFieldName, accountRow.InterestRate);
                                XMLOut.WriteElementString(FieldMap.ProbabilityFieldName, accountRow.Probability);
                                XMLOut.WriteElementString(FieldMap.EstimatedCloseDateFieldName, accountRow.EstimatedCloseDate);
                                XMLOut.WriteStartElement(FieldMap.AssignedLenderFieldName);
                                XMLOut.WriteAttributeString("type", accountRow.AssignedLenderType);
                                XMLOut.WriteValue(accountRow.AssignedLender);
                                XMLOut.WriteEndElement();
                                XMLOut.WriteStartElement(FieldMap.AssignedAnalystFieldName);
                                XMLOut.WriteAttributeString("type", accountRow.AssignedAnalystType);
                                XMLOut.WriteValue(accountRow.AssignedAnalyst);
                                XMLOut.WriteEndElement();
                                XMLOut.WriteStartElement(FieldMap.AssignedLoanProcessorFieldName);
                                XMLOut.WriteAttributeString("type", accountRow.AssignedLoanProcessorType);
                                XMLOut.WriteValue(accountRow.AssignedLoanProcessor);
                                XMLOut.WriteEndElement();
                                XMLOut.WriteElementString(FieldMap.ApplicationLockedFieldName, accountRow.ApplicationLocked);

                                {
                                    XMLOut.WriteStartElement("approval");
                                    XMLOut.WriteElementString(FieldMap.ApprovalStatusFieldName, accountRow.ApprovalStatus);
                                    XMLOut.WriteElementString(FieldMap.OriginatingUserFieldName, accountRow.OriginatingUser);
                                    XMLOut.WriteStartElement(FieldMap.AssignedApproverFieldName);
                                    XMLOut.WriteAttributeString("type", accountRow.AssignedApproverType);
                                    XMLOut.WriteValue(accountRow.AssignedApprover);
                                    XMLOut.WriteEndElement();

                                    XMLOut.WriteEndElement();
                                }

                                XMLOut.WriteEndElement();
                            }

                            XMLOut.WriteEndElement();

                            if (Configuration.CollateralsYN == "Y")
                            {
                                var collateralRows = recordsToWrite.Where(acct => acct.CustomerNumber == customerNumber &&
                                    acct.LoanNumber == accountNumber && acct.BorrowerType == "");
                                
                                foreach (SourceRecord collateralRow in collateralRows)
                                {
                                    XMLOut.WriteStartElement("loan");
                                    XMLOut.WriteElementString(FieldMap.LoanNumberFieldName, collateralRow.CollateralLoanNumber);
                                    XMLOut.WriteElementString(FieldMap.CustomerNumberFieldName, collateralRow.CustomerNumber);
                                    XMLOut.WriteElementString(FieldMap.AccountClassFieldName, collateralRow.AccountClass);
                                    XMLOut.WriteElementString(FieldMap.LoanStatusCodeFieldName, accountRow.LoanStatusCode);
                                    XMLOut.WriteElementString(FieldMap.LoanOfficerCodeFieldName, accountRow.LoanOfficerCode);
                                    XMLOut.WriteElementString(FieldMap.LoanTypeCodeFieldName, collateralRow.CollateralLoanTypeCode);
                                    XMLOut.WriteElementString(FieldMap.LoanClosedFieldName, accountRow.LoanClosed);
                                    XMLOut.WriteElementString(FieldMap.LoanAmountFieldName, accountRow.LoanAmount);
                                    XMLOut.WriteElementString(FieldMap.LoanOriginationDateFieldName, accountRow.LoanOriginationDate);
                                    XMLOut.WriteElementString(FieldMap.BorrowerTypeFieldName, accountRow.BorrowerType);
                                    XMLOut.WriteElementString(FieldMap.ParentLoanNumberFieldName, collateralRow.ParentLoanNumber);
                                    XMLOut.WriteElementString(FieldMap.LoanDescriptionFieldName, collateralRow.CollateralDescription);
                                    XMLOut.WriteElementString(FieldMap.LoanBranchFieldName, accountRow.LoanBranch);
                                    XMLOut.WriteElementString(FieldMap.CoreClassCodeFieldName, accountRow.CoreClassCode);
                                    XMLOut.WriteElementString(FieldMap.CoreCollCodeFieldName, accountRow.CoreCollCode);
                                    XMLOut.WriteElementString(FieldMap.CoreCollateralCodeFieldName, accountRow.CoreCollateralCode);
                                    XMLOut.WriteElementString(FieldMap.CorePurposeCodeFieldName, accountRow.CorePurposeCode);
                                    XMLOut.WriteElementString(FieldMap.CoreTypeCodeFieldName, accountRow.CoreTypeCode);
                                    XMLOut.WriteElementString(FieldMap.CommitmentAmountFieldName, accountRow.CommitmentAmount);
                                    XMLOut.WriteElementString(FieldMap.CoreNaicsCodeFieldName, accountRow.CoreNaicsCode);
                                    XMLOut.WriteElementString(FieldMap.LoanMaturityDateFieldName, accountRow.LoanMaturityDate);
                                    XMLOut.WriteElementString(FieldMap.LoanClassificationCodeFieldName, accountRow.LoanClassificationCode);
                                    XMLOut.WriteEndElement();
                                }
                            }

                        }

                        XMLOut.WriteEndElement();
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
                var primaryRows = from r in sourceRecords
                                  where r.LoanNumber == record.LoanNumber &&
                                    r.BorrowerType == ""
                                  select r;
                                  
                var distinctPrimaries = (from p in primaryRows
                                        select p.CustomerNumber).Distinct().ToList().Count();
                if (distinctPrimaries != 1)
                {
                    record.IgnoreRecord = true;
                    LogWriter.LogMessage(string.Format("Wrong number of primary relationships for account {0} - primary relationships: {1}",
                        record.LoanNumber, distinctPrimaries.ToString()));
                }
                else if (record.BorrowerType != "")
                {
                    record.OwningCustomerNumber = primaryRows.First().CustomerNumber;
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
