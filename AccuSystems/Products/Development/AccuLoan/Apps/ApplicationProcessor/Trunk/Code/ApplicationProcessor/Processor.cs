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
    using System.Data.OleDb;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;

    /// <summary>
    /// Functions for processing source data
    /// </summary>
    class Processor
    {
        private StringBuilder accountsProcessed;

        public Configuration Config { get; set; }
        public LogWriter LogFile { get; set; }
        public FieldMapper FieldMap { get; set; }

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

                switch (Config.SourceDelimitedSQLXML.ToUpper())
                {
                    case "DELIMITED":
                        string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=.\\;Extended Properties=\"Text;HDR=YES;FMT=Delimited\";";
                        OleDbConnection fileConn = new OleDbConnection(connectionString);
                        OleDbDataAdapter fileAdapter = new OleDbDataAdapter("select * from " + Config.SourceFile, fileConn);
                        fileAdapter.Fill(dataTable);
                        break;

                    case "SQL":
                        string dbConnectionString = Config.SourceSQLConnectionString;
                        string sqlQuery = File.ReadAllText(Config.SourceSQLQueryFile);
                        SqlConnection connection = new SqlConnection(dbConnectionString);
                        connection.Open();
                        SqlDataAdapter dadapter = new SqlDataAdapter();
                        dadapter.SelectCommand = new SqlCommand(sqlQuery, connection);
                        dadapter.Fill(dataTable);
                        connection.Close();
                        break;
                    case "XML":
                        LogFile.LogMessage("XML Data Source not implemented");
                        break;
                    default:
                        break;
                }
                success = true;
            }
            catch (Exception e)
            {
                LogFile.LogMessage(e.ToString());
                LogFile.LogMessage();
                LogFile.LogMessage("Unable to retrieve source data");
            }

            return success;
        }

        /// <summary>
        /// Mappes source data into a new datatable based on field mapping
        /// </summary>
        /// <param name="sourceTable">The DataTable containing the source data</param>
        /// <returns>DataTable containing mapped information</returns>
        public DataTable LoadDataTableFromMappedFields(DataTable sourceTable)
        {
            DataTable newTable = new DataTable();
            foreach (PropertyInfo prop in FieldMap.GetType().GetProperties())
            {
                if (prop.Name.Contains("MappedField"))
                    newTable.Columns.Add(prop.Name.Replace("MappedField", ""));
            }

            foreach (DataRow sourceRow in sourceTable.Rows)
            {
                DataRow newRow = newTable.NewRow();

                foreach (PropertyInfo prop in FieldMap.GetType().GetProperties())
                {
                    if (prop.Name.Contains("MappedField"))
                    {
                        string sourceField = prop.GetValue(FieldMap, null).ToString();
                        string destField = prop.Name.Replace("MappedField", "");

                        if (sourceField != "")
                        {
                            newRow[destField] = sourceRow[sourceField];
                        }
                    }
                }
                newTable.Rows.Add(newRow);
            }
            return newTable;
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
        /// <param name="dataToWrite">The DataTable to process</param>
        /// <returns>True upon successful completion</returns>
        public bool WriteXMLData(DataTable dataToWrite)
        {
            bool success = false;

            XmlWriterSettings xmlRules = new XmlWriterSettings();
            xmlRules.Indent = true;
            xmlRules.NewLineOnAttributes = true;
            xmlRules.OmitXmlDeclaration = true;
            xmlRules.Encoding = Encoding.Default;

            //List of using postProcessingField for use in post processing query.  Start with single quote for first record
            accountsProcessed = new StringBuilder();
            accountsProcessed.Append("'");

            DataRow[] rowArray = dataToWrite.Select();
            var uniqueCustomers = (from c in rowArray
                                   select c[FieldMap.CustomerNumberFieldName]).Distinct();
            if (uniqueCustomers.Count() == 0)
            {
                LogFile.LogMessage("No records to write");
                return false;
            }
            try
            {
                using (XmlWriter XMLOut = XmlWriter.Create(Config.OutputFile, xmlRules))
                {
                    XMLOut.WriteStartDocument();
                    XMLOut.WriteStartElement("AccuSystems");

                    foreach (string customerNumber in uniqueCustomers)
                    {
                        DataRow customer = dataToWrite.Select(FieldMap.CustomerNumberFieldName + " = '" + customerNumber + "'").First();
                        XMLOut.WriteStartElement("customer");

                        XMLOut.WriteElementString(FieldMap.CustomerNumberFieldName, customer[FieldMap.CustomerNumberFieldName].ToString());
                        XMLOut.WriteElementString(FieldMap.TaxIdFieldName, customer[FieldMap.TaxIdFieldName].ToString());
                        XMLOut.WriteElementString(FieldMap.CustomerNameFieldName, customer[FieldMap.CustomerNameFieldName].ToString());
                        XMLOut.WriteElementString(FieldMap.BusinessNameFieldName, customer[FieldMap.BusinessNameFieldName].ToString());
                        XMLOut.WriteElementString(FieldMap.CustomerFirstNameFieldName, customer[FieldMap.CustomerFirstNameFieldName].ToString());
                        XMLOut.WriteElementString(FieldMap.CustomerMiddleNameFieldName, customer[FieldMap.CustomerMiddleNameFieldName].ToString());
                        XMLOut.WriteElementString(FieldMap.CustomerLastNameFieldName, customer[FieldMap.CustomerLastNameFieldName].ToString());
                        XMLOut.WriteElementString(FieldMap.CustomerTypeCodeFieldName, customer[FieldMap.CustomerTypeCodeFieldName].ToString());
                        XMLOut.WriteElementString(FieldMap.BankCodeFieldName, customer[FieldMap.BankCodeFieldName].ToString());
                        XMLOut.WriteElementString(FieldMap.EmployeeFieldName, customer[FieldMap.EmployeeFieldName].ToString());
                        XMLOut.WriteElementString(FieldMap.CustomerBranchFieldName, customer[FieldMap.CustomerBranchFieldName].ToString());
                        XMLOut.WriteElementString(FieldMap.CustomerOfficerCodeFieldName, customer[FieldMap.CustomerOfficerCodeFieldName].ToString());
                        XMLOut.WriteElementString(FieldMap.Address1FieldName, customer[FieldMap.Address1FieldName].ToString());
                        XMLOut.WriteElementString(FieldMap.Address2FieldName, customer[FieldMap.Address2FieldName].ToString());
                        XMLOut.WriteElementString(FieldMap.CityFieldName, customer[FieldMap.CityFieldName].ToString());
                        XMLOut.WriteElementString(FieldMap.StateFieldName, customer[FieldMap.StateFieldName].ToString());
                        XMLOut.WriteElementString(FieldMap.ZipCodeFieldName, customer[FieldMap.ZipCodeFieldName].ToString());
                        XMLOut.WriteElementString(FieldMap.HomePhoneFieldName, customer[FieldMap.HomePhoneFieldName].ToString());
                        XMLOut.WriteElementString(FieldMap.WorkPhoneFieldName, customer[FieldMap.WorkPhoneFieldName].ToString());
                        XMLOut.WriteElementString(FieldMap.MobilePhoneFieldName, customer[FieldMap.MobilePhoneFieldName].ToString());
                        XMLOut.WriteElementString(FieldMap.FaxFieldName, customer[FieldMap.FaxFieldName].ToString());
                        XMLOut.WriteElementString(FieldMap.EmailFieldName, customer[FieldMap.EmailFieldName].ToString());
                        XMLOut.WriteElementString(FieldMap.ClassificationCodeFieldName, customer[FieldMap.ClassificationCodeFieldName].ToString());
                        XMLOut.WriteElementString(FieldMap.CustomerStatusFieldName, customer[FieldMap.CustomerStatusFieldName].ToString());

                        var uniqueAccounts = (from a in rowArray
                                              where a[FieldMap.CustomerNumberFieldName].ToString() == customerNumber
                                              select a[FieldMap.LoanNumberFieldName]).Distinct();

                        foreach (string accountNumber in uniqueAccounts)
                        {
                            DataRow accountRow = dataToWrite.Select(FieldMap.CustomerNumberFieldName + " = '" + customerNumber +
                                "' AND " + FieldMap.LoanNumberFieldName + " = '" + accountNumber + "'").First();

                            //build list using postProcessingField for use in post processing query
                            accountsProcessed.Append(accountRow[FieldMap.PostProcessingFieldFieldName].ToString() + "', '");

                            XMLOut.WriteStartElement("loan");
                            XMLOut.WriteElementString(FieldMap.LoanNumberFieldName, accountRow[FieldMap.LoanNumberFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.CustomerNumberFieldName, accountRow[FieldMap.CustomerNumberFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.AccountClassFieldName, accountRow[FieldMap.AccountClassFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.LoanStatusCodeFieldName, accountRow[FieldMap.LoanStatusCodeFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.LoanOfficerCodeFieldName, accountRow[FieldMap.LoanOfficerCodeFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.LoanTypeCodeFieldName, accountRow[FieldMap.LoanTypeCodeFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.LoanClosedFieldName, accountRow[FieldMap.LoanClosedFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.LoanAmountFieldName, accountRow[FieldMap.LoanAmountFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.LoanOriginationDateFieldName, accountRow[FieldMap.LoanOriginationDateFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.LoanDescriptionFieldName, accountRow[FieldMap.LoanDescriptionFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.BorrowerTypeFieldName, accountRow[FieldMap.BorrowerTypeFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.OwningCustomerNumberFieldName, accountRow[FieldMap.OwningCustomerNumberFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.LoanBranchFieldName, accountRow[FieldMap.LoanBranchFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.CoreClassCodeFieldName, accountRow[FieldMap.CoreClassCodeFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.CoreCollCodeFieldName, accountRow[FieldMap.CoreCollCodeFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.CoreCollateralCodeFieldName, accountRow[FieldMap.CoreCollateralCodeFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.CorePurposeCodeFieldName, accountRow[FieldMap.CorePurposeCodeFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.CoreTypeCodeFieldName, accountRow[FieldMap.CoreTypeCodeFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.CommitmentAmountFieldName, accountRow[FieldMap.CommitmentAmountFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.CoreNaicsCodeFieldName, accountRow[FieldMap.CoreNaicsCodeFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.LoanMaturityDateFieldName, accountRow[FieldMap.LoanMaturityDateFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.LoanClassificationCodeFieldName, accountRow[FieldMap.LoanClassificationCodeFieldName].ToString());

                            {
                                XMLOut.WriteStartElement("application");
                                XMLOut.WriteElementString("applicationNumber", accountRow[FieldMap.LoanNumberFieldName].ToString());
                                XMLOut.WriteElementString(FieldMap.ApplicationDateFieldName, accountRow[FieldMap.ApplicationDateFieldName].ToString());
                                XMLOut.WriteElementString(FieldMap.CreditAnalysisStatusFieldName, accountRow[FieldMap.CreditAnalysisStatusFieldName].ToString());
                                XMLOut.WriteElementString(FieldMap.RequestedAmountFieldName, accountRow[FieldMap.RequestedAmountFieldName].ToString());
                                XMLOut.WriteElementString(FieldMap.PrimaryCollateralValueFieldName, accountRow[FieldMap.PrimaryCollateralValueFieldName].ToString());
                                XMLOut.WriteElementString(FieldMap.FICOFieldName, accountRow[FieldMap.FICOFieldName].ToString());
                                XMLOut.WriteElementString(FieldMap.ValuationDateFieldName, accountRow[FieldMap.ValuationDateFieldName].ToString());
                                XMLOut.WriteElementString(FieldMap.InterestRateFieldName, accountRow[FieldMap.InterestRateFieldName].ToString());
                                XMLOut.WriteElementString(FieldMap.ProbabilityFieldName, accountRow[FieldMap.ProbabilityFieldName].ToString());
                                XMLOut.WriteElementString(FieldMap.EstimatedCloseDateFieldName, accountRow[FieldMap.EstimatedCloseDateFieldName].ToString());
                                XMLOut.WriteStartElement(FieldMap.AssignedLenderFieldName);
                                XMLOut.WriteAttributeString("type", accountRow[FieldMap.AssignedLenderTypeFieldName].ToString());
                                XMLOut.WriteValue(accountRow[FieldMap.AssignedLenderFieldName].ToString());
                                XMLOut.WriteEndElement();
                                XMLOut.WriteStartElement(FieldMap.AssignedAnalystFieldName);
                                XMLOut.WriteAttributeString("type", accountRow[FieldMap.AssignedAnalystTypeFieldName].ToString());
                                XMLOut.WriteValue(accountRow[FieldMap.AssignedAnalystFieldName].ToString());
                                XMLOut.WriteEndElement();
                                XMLOut.WriteStartElement(FieldMap.AssignedLoanProcessorFieldName);
                                XMLOut.WriteAttributeString("type", accountRow[FieldMap.AssignedLoanProcessorTypeFieldName].ToString());
                                XMLOut.WriteValue(accountRow[FieldMap.AssignedLoanProcessorFieldName].ToString());
                                XMLOut.WriteEndElement();
                                XMLOut.WriteElementString(FieldMap.ApplicationLockedFieldName, accountRow[FieldMap.ApplicationLockedFieldName].ToString());

                                {
                                    XMLOut.WriteStartElement("approval");
                                    XMLOut.WriteElementString(FieldMap.ApprovalStatusFieldName, accountRow[FieldMap.ApprovalStatusFieldName].ToString());
                                    XMLOut.WriteElementString(FieldMap.OriginatingUserFieldName, accountRow[FieldMap.OriginatingUserFieldName].ToString());
                                    XMLOut.WriteStartElement(FieldMap.AssignedApproverFieldName);
                                    XMLOut.WriteAttributeString("type", accountRow[FieldMap.AssignedApproverTypeFieldName].ToString());
                                    XMLOut.WriteValue(accountRow[FieldMap.AssignedApproverFieldName].ToString());
                                    XMLOut.WriteEndElement();

                                    XMLOut.WriteEndElement();
                                }

                                XMLOut.WriteEndElement();
                            }

                            XMLOut.WriteEndElement();

                            if (Config.CollateralsYN == "Y")
                            {
                                DataRow[] collateralRows = dataToWrite.Select(FieldMap.CustomerNumberFieldName + " = '" + customerNumber +
                                "' AND " + FieldMap.LoanNumberFieldName + " = '" + accountNumber +
                                "' AND " + FieldMap.BorrowerTypeFieldName + " = ''");
                                foreach (DataRow collateralRow in collateralRows)
                                {
                                    XMLOut.WriteStartElement("loan");
                                    XMLOut.WriteElementString(FieldMap.LoanNumberFieldName, collateralRow[FieldMap.CollateralLoanNumberFieldName].ToString());
                                    XMLOut.WriteElementString(FieldMap.CustomerNumberFieldName, collateralRow[FieldMap.CustomerNumberFieldName].ToString());
                                    XMLOut.WriteElementString(FieldMap.AccountClassFieldName, collateralRow[FieldMap.AccountClassFieldName].ToString());
                                    XMLOut.WriteElementString(FieldMap.LoanStatusCodeFieldName, accountRow[FieldMap.LoanStatusCodeFieldName].ToString());
                                    XMLOut.WriteElementString(FieldMap.LoanOfficerCodeFieldName, accountRow[FieldMap.LoanOfficerCodeFieldName].ToString());
                                    XMLOut.WriteElementString(FieldMap.LoanTypeCodeFieldName, collateralRow[FieldMap.CollateralLoanTypeCodeFieldName].ToString());
                                    XMLOut.WriteElementString(FieldMap.LoanClosedFieldName, accountRow[FieldMap.LoanClosedFieldName].ToString());
                                    XMLOut.WriteElementString(FieldMap.LoanAmountFieldName, accountRow[FieldMap.LoanAmountFieldName].ToString());
                                    XMLOut.WriteElementString(FieldMap.LoanOriginationDateFieldName, accountRow[FieldMap.LoanOriginationDateFieldName].ToString());
                                    XMLOut.WriteElementString(FieldMap.BorrowerTypeFieldName, accountRow[FieldMap.BorrowerTypeFieldName].ToString());
                                    XMLOut.WriteElementString(FieldMap.ParentLoanNumberFieldName, collateralRow[FieldMap.ParentLoanNumberFieldName].ToString());
                                    XMLOut.WriteElementString(FieldMap.LoanDescriptionFieldName, collateralRow[FieldMap.CollateralDescriptionFieldName].ToString());
                                    XMLOut.WriteElementString(FieldMap.LoanBranchFieldName, accountRow[FieldMap.LoanBranchFieldName].ToString());
                                    XMLOut.WriteElementString(FieldMap.CoreClassCodeFieldName, accountRow[FieldMap.CoreClassCodeFieldName].ToString());
                                    XMLOut.WriteElementString(FieldMap.CoreCollCodeFieldName, accountRow[FieldMap.CoreCollCodeFieldName].ToString());
                                    XMLOut.WriteElementString(FieldMap.CoreCollateralCodeFieldName, accountRow[FieldMap.CoreCollateralCodeFieldName].ToString());
                                    XMLOut.WriteElementString(FieldMap.CorePurposeCodeFieldName, accountRow[FieldMap.CorePurposeCodeFieldName].ToString());
                                    XMLOut.WriteElementString(FieldMap.CoreTypeCodeFieldName, accountRow[FieldMap.CoreTypeCodeFieldName].ToString());
                                    XMLOut.WriteElementString(FieldMap.CommitmentAmountFieldName, accountRow[FieldMap.CommitmentAmountFieldName].ToString());
                                    XMLOut.WriteElementString(FieldMap.CoreNaicsCodeFieldName, accountRow[FieldMap.CoreNaicsCodeFieldName].ToString());
                                    XMLOut.WriteElementString(FieldMap.LoanMaturityDateFieldName, accountRow[FieldMap.LoanMaturityDateFieldName].ToString());
                                    XMLOut.WriteElementString(FieldMap.LoanClassificationCodeFieldName, accountRow[FieldMap.LoanClassificationCodeFieldName].ToString());
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
                LogFile.LogMessage(e.ToString());
                LogFile.LogMessage();
                LogFile.LogMessage("Unable to write XML file");
                return success;
            }

            success = true;
            return success;
        }

        /// <summary>
        /// Sets the OwningCustomer field for collateral records
        /// </summary>
        /// <param name="sourceTable">The DataTable to process</param>
        public void SetOwningCustomer (DataTable sourceTable)
        {
            List<int> rowsToRemove = new List<int>();

            for (int i = 0; i < sourceTable.Rows.Count; i++)
            {
                DataRow row = sourceTable.Rows[i];

                DataRow[] primaryRows = sourceTable.Select("loanNumber = '" + row[FieldMap.LoanNumberFieldName].ToString() + "' AND borrowerType = ''");
                var distinctPrimaries = (from p in primaryRows
                                        select p[FieldMap.CustomerNumberFieldName]).Distinct().ToList().Count();
                if (distinctPrimaries != 1)
                {
                    rowsToRemove.Add(i);
                    LogFile.LogMessage(string.Format("Wrong number of primary relationships for account {0} - primary relationships: {1}",
                        row[FieldMap.LoanNumberFieldName].ToString(), distinctPrimaries.ToString()));
                }
                else if (row[FieldMap.BorrowerTypeFieldName].ToString() != "")
                {
                    DataRow primaryRow = primaryRows[0];
                    row[FieldMap.OwningCustomerNumberFieldName] = primaryRow[FieldMap.CustomerNumberFieldName].ToString();
                }

            }
            foreach (int i in rowsToRemove)
            {
                sourceTable.Rows[i].Delete();
            }
            sourceTable.AcceptChanges();
        }

        /// <summary>
        /// Writes a log file of all account numbers written to the XML file
        /// </summary>
        public void WriteAccountsProcessedLogFile()
        {
            string[] fileNameParts = Config.AccountsProcessedLogFile.Split('.');
            string path = string.Format("{0}{1}_{2}.{3}", Config.LogFolder, fileNameParts[0], DateTime.Now.ToString().Replace("/", "_").Replace(":", "_"), fileNameParts[1]);
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
                string sqlQuery = File.ReadAllText(Config.PostProcessingSQLQueryFile).Replace("%%A", accountsProcessed.ToString());
                int recordsAffected = Utils.ExecuteSQLQuery(Config.SourceSQLConnectionString, sqlQuery);
                LogFile.LogMessage(recordsAffected + " Records Affected.");
            }
            catch (Exception e)
            {
                LogFile.LogMessage(e.ToString());
                LogFile.LogMessage();
                LogFile.LogMessage("Unable to execute post processing query");

            }
        }
    }
}
