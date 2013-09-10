using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using System.Reflection;
using System.IO;
using System.Data.SqlClient;
using System.Xml;

namespace ApplicationProcessor
{
    class Processor
    {
        public Configuration Config { get; set; }
        public LogWriter LogFile { get; set; }
        public FieldMapper FieldMap { get; set; }

        private StringBuilder accountsProcessed;

        public bool FillDataTable(out DataTable dataTable)
        {
            dataTable = new DataTable();
            bool success = false;

            try
            {
                DataSources dataSource = (DataSources)Enum.Parse(typeof(DataSources), Config.SourceDelimitedSQLXML, true);
                switch (dataSource)
                {
                    case DataSources.delimited:
                        string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=.\\;Extended Properties=\"Text;HDR=YES;FMT=Delimited\";";
                        OleDbConnection fileConn = new OleDbConnection(connectionString);
                        OleDbDataAdapter fileAdapter = new OleDbDataAdapter("select * from " + Config.SourceFile, fileConn);
                        fileAdapter.Fill(dataTable);
                        break;

                    case DataSources.SQL:
                        string dbConnectionString = Config.SourceSQLConnectionString;
                        string sqlQuery = File.ReadAllText(Config.SourceSQLQueryFile);
                        SqlConnection connection = new SqlConnection(dbConnectionString);
                        connection.Open();
                        SqlDataAdapter dadapter = new SqlDataAdapter();
                        dadapter.SelectCommand = new SqlCommand(sqlQuery, connection);
                        dadapter.Fill(dataTable);
                        connection.Close();
                        break;
                    case DataSources.XML:
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

        public DataTable LoadDataTableFromMappedFields(DataTable sourceTable, FieldMapper fieldMap)
        {
            DataTable newTable = new DataTable();
            foreach (PropertyInfo prop in fieldMap.GetType().GetProperties())
            {
                if (prop.Name.Contains("MappedField"))
                    newTable.Columns.Add(prop.Name.Replace("MappedField", ""));
            }

            foreach (DataRow sourceRow in sourceTable.Rows)
            {
                DataRow newRow = newTable.NewRow();

                foreach (PropertyInfo prop in fieldMap.GetType().GetProperties())
                {
                    if (prop.Name.Contains("MappedField"))
                    {
                        string sourceField = prop.GetValue(fieldMap, null).ToString();
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
                                   select c[FieldMap.customerNumberFieldName]).Distinct();
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
                        DataRow customer = dataToWrite.Select(FieldMap.customerNumberFieldName + " = '" + customerNumber + "'").First();
                        XMLOut.WriteStartElement("customer");

                        XMLOut.WriteElementString(FieldMap.customerNumberFieldName, customer[FieldMap.customerNumberFieldName].ToString());
                        XMLOut.WriteElementString(FieldMap.taxIdFieldName, customer[FieldMap.taxIdFieldName].ToString());
                        XMLOut.WriteElementString(FieldMap.customerNameFieldName, customer[FieldMap.customerNameFieldName].ToString());
                        XMLOut.WriteElementString(FieldMap.businessNameFieldName, customer[FieldMap.businessNameFieldName].ToString());
                        XMLOut.WriteElementString(FieldMap.customerFirstNameFieldName, customer[FieldMap.customerFirstNameFieldName].ToString());
                        XMLOut.WriteElementString(FieldMap.customerMiddleNameFieldName, customer[FieldMap.customerMiddleNameFieldName].ToString());
                        XMLOut.WriteElementString(FieldMap.customerLastNameFieldName, customer[FieldMap.customerLastNameFieldName].ToString());
                        XMLOut.WriteElementString(FieldMap.customerTypeCodeFieldName, customer[FieldMap.customerTypeCodeFieldName].ToString());
                        XMLOut.WriteElementString(FieldMap.bankCodeFieldName, customer[FieldMap.bankCodeFieldName].ToString());
                        XMLOut.WriteElementString(FieldMap.employeeFieldName, customer[FieldMap.employeeFieldName].ToString());
                        XMLOut.WriteElementString(FieldMap.customerBranchFieldName, customer[FieldMap.customerBranchFieldName].ToString());
                        XMLOut.WriteElementString(FieldMap.customerOfficerCodeFieldName, customer[FieldMap.customerOfficerCodeFieldName].ToString());
                        XMLOut.WriteElementString(FieldMap.address1FieldName, customer[FieldMap.address1FieldName].ToString());
                        XMLOut.WriteElementString(FieldMap.address2FieldName, customer[FieldMap.address2FieldName].ToString());
                        XMLOut.WriteElementString(FieldMap.cityFieldName, customer[FieldMap.cityFieldName].ToString());
                        XMLOut.WriteElementString(FieldMap.stateFieldName, customer[FieldMap.stateFieldName].ToString());
                        XMLOut.WriteElementString(FieldMap.zipCodeFieldName, customer[FieldMap.zipCodeFieldName].ToString());
                        XMLOut.WriteElementString(FieldMap.homePhoneFieldName, customer[FieldMap.homePhoneFieldName].ToString());
                        XMLOut.WriteElementString(FieldMap.workPhoneFieldName, customer[FieldMap.workPhoneFieldName].ToString());
                        XMLOut.WriteElementString(FieldMap.mobilePhoneFieldName, customer[FieldMap.mobilePhoneFieldName].ToString());
                        XMLOut.WriteElementString(FieldMap.faxFieldName, customer[FieldMap.faxFieldName].ToString());
                        XMLOut.WriteElementString(FieldMap.emailFieldName, customer[FieldMap.emailFieldName].ToString());
                        XMLOut.WriteElementString(FieldMap.classificationCodeFieldName, customer[FieldMap.classificationCodeFieldName].ToString());
                        XMLOut.WriteElementString(FieldMap.customerStatusFieldName, customer[FieldMap.customerStatusFieldName].ToString());

                        var uniqueAccounts = (from a in rowArray
                                              where a[FieldMap.customerNumberFieldName].ToString() == customerNumber
                                              select a[FieldMap.loanNumberFieldName]).Distinct();

                        foreach (string accountNumber in uniqueAccounts)
                        {
                            DataRow accountRow = dataToWrite.Select(FieldMap.customerNumberFieldName + " = '" + customerNumber +
                                "' AND " + FieldMap.loanNumberFieldName + " = '" + accountNumber + "'").First();

                            //build list using postProcessingField for use in post processing query
                            accountsProcessed.Append(accountRow[FieldMap.postProcessingFieldFieldName].ToString() + "', '");

                            XMLOut.WriteStartElement("loan");
                            XMLOut.WriteElementString(FieldMap.loanNumberFieldName, accountRow[FieldMap.loanNumberFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.customerNumberFieldName, accountRow[FieldMap.customerNumberFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.accountClassFieldName, accountRow[FieldMap.accountClassFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.loanStatusCodeFieldName, accountRow[FieldMap.loanStatusCodeFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.loanOfficerCodeFieldName, accountRow[FieldMap.loanOfficerCodeFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.loanTypeCodeFieldName, accountRow[FieldMap.loanTypeCodeFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.loanClosedFieldName, accountRow[FieldMap.loanClosedFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.loanAmountFieldName, accountRow[FieldMap.loanAmountFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.loanOriginationDateFieldName, accountRow[FieldMap.loanOriginationDateFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.loanDescriptionFieldName, accountRow[FieldMap.loanDescriptionFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.borrowerTypeFieldName, accountRow[FieldMap.borrowerTypeFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.owningCustomerNumberFieldName, accountRow[FieldMap.owningCustomerNumberFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.loanBranchFieldName, accountRow[FieldMap.loanBranchFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.coreClassCodeFieldName, accountRow[FieldMap.coreClassCodeFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.coreCollCodeFieldName, accountRow[FieldMap.coreCollCodeFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.coreCollateralCodeFieldName, accountRow[FieldMap.coreCollateralCodeFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.corePurposeCodeFieldName, accountRow[FieldMap.corePurposeCodeFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.coreTypeCodeFieldName, accountRow[FieldMap.coreTypeCodeFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.commitmentAmountFieldName, accountRow[FieldMap.commitmentAmountFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.coreNaicsCodeFieldName, accountRow[FieldMap.coreNaicsCodeFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.loanMaturityDateFieldName, accountRow[FieldMap.loanMaturityDateFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.loanClassificationCodeFieldName, accountRow[FieldMap.loanClassificationCodeFieldName].ToString());

                            {
                                XMLOut.WriteStartElement("application");
                                XMLOut.WriteElementString(FieldMap.applicationDateFieldName, accountRow[FieldMap.applicationDateFieldName].ToString());
                                XMLOut.WriteElementString(FieldMap.creditAnalysisStatusFieldName, accountRow[FieldMap.creditAnalysisStatusFieldName].ToString());
                                XMLOut.WriteElementString(FieldMap.requestedAmountFieldName, accountRow[FieldMap.requestedAmountFieldName].ToString());
                                XMLOut.WriteElementString(FieldMap.primaryCollateralValueFieldName, accountRow[FieldMap.primaryCollateralValueFieldName].ToString());
                                XMLOut.WriteElementString(FieldMap.FICOFieldName, accountRow[FieldMap.FICOFieldName].ToString());
                                XMLOut.WriteElementString(FieldMap.valuationDateFieldName, accountRow[FieldMap.valuationDateFieldName].ToString());
                                XMLOut.WriteElementString(FieldMap.interestRateFieldName, accountRow[FieldMap.interestRateFieldName].ToString());
                                XMLOut.WriteElementString(FieldMap.probabilityFieldName, accountRow[FieldMap.probabilityFieldName].ToString());
                                XMLOut.WriteElementString(FieldMap.estimatedCloseDateFieldName, accountRow[FieldMap.estimatedCloseDateFieldName].ToString());
                                XMLOut.WriteStartElement(FieldMap.assignedLenderFieldName);
                                XMLOut.WriteAttributeString("type", accountRow[FieldMap.assignedLenderTypeFieldName].ToString());
                                XMLOut.WriteValue(accountRow[FieldMap.assignedLenderFieldName].ToString());
                                XMLOut.WriteEndElement();
                                XMLOut.WriteStartElement(FieldMap.assignedAnalystFieldName);
                                XMLOut.WriteAttributeString("type", accountRow[FieldMap.assignedAnalystTypeFieldName].ToString());
                                XMLOut.WriteValue(accountRow[FieldMap.assignedAnalystFieldName].ToString());
                                XMLOut.WriteEndElement();
                                XMLOut.WriteStartElement(FieldMap.assignedLoanProcessorFieldName);
                                XMLOut.WriteAttributeString("type", accountRow[FieldMap.assignedLoanProcessorTypeFieldName].ToString());
                                XMLOut.WriteValue(accountRow[FieldMap.assignedLoanProcessorFieldName].ToString());
                                XMLOut.WriteEndElement();
                                XMLOut.WriteElementString(FieldMap.applicationLockedFieldName, accountRow[FieldMap.applicationLockedFieldName].ToString());

                                {
                                    XMLOut.WriteStartElement("approval");
                                    XMLOut.WriteElementString(FieldMap.approvalStatusFieldName, accountRow[FieldMap.approvalStatusFieldName].ToString());
                                    XMLOut.WriteElementString(FieldMap.originatingUserFieldName, accountRow[FieldMap.originatingUserFieldName].ToString());
                                    XMLOut.WriteElementString(FieldMap.assignedApproverFieldName, accountRow[FieldMap.assignedApproverFieldName].ToString());
                                    XMLOut.WriteStartElement(FieldMap.assignedApproverFieldName);
                                    XMLOut.WriteAttributeString("type", accountRow[FieldMap.assignedApproverTypeFieldName].ToString());
                                    XMLOut.WriteValue(accountRow[FieldMap.assignedApproverFieldName].ToString());
                                    XMLOut.WriteEndElement();

                                    XMLOut.WriteEndElement();
                                }

                                XMLOut.WriteEndElement();
                            }

                            XMLOut.WriteEndElement();

                            if (Config.CollateralsYN == "Y")
                            {
                                DataRow[] collateralRows = dataToWrite.Select(FieldMap.customerNumberFieldName + " = '" + customerNumber +
                                "' AND " + FieldMap.loanNumberFieldName + " = '" + accountNumber +
                                "' AND " + FieldMap.borrowerTypeFieldName + " = ''");
                                foreach (DataRow collateralRow in collateralRows)
                                {
                                    XMLOut.WriteStartElement("loan");
                                    XMLOut.WriteElementString(FieldMap.loanNumberFieldName, collateralRow[FieldMap.collateralLoanNumberFieldName].ToString());
                                    XMLOut.WriteElementString(FieldMap.customerNumberFieldName, collateralRow[FieldMap.customerNumberFieldName].ToString());
                                    XMLOut.WriteElementString(FieldMap.accountClassFieldName, collateralRow[FieldMap.accountClassFieldName].ToString());
                                    XMLOut.WriteElementString(FieldMap.loanStatusCodeFieldName, accountRow[FieldMap.loanStatusCodeFieldName].ToString());
                                    XMLOut.WriteElementString(FieldMap.loanOfficerCodeFieldName, accountRow[FieldMap.loanOfficerCodeFieldName].ToString());
                                    XMLOut.WriteElementString(FieldMap.loanTypeCodeFieldName, collateralRow[FieldMap.collateralLoanTypeCodeFieldName].ToString());
                                    XMLOut.WriteElementString(FieldMap.loanClosedFieldName, accountRow[FieldMap.loanClosedFieldName].ToString());
                                    XMLOut.WriteElementString(FieldMap.loanAmountFieldName, accountRow[FieldMap.loanAmountFieldName].ToString());
                                    XMLOut.WriteElementString(FieldMap.loanOriginationDateFieldName, accountRow[FieldMap.loanOriginationDateFieldName].ToString());
                                    XMLOut.WriteElementString(FieldMap.borrowerTypeFieldName, accountRow[FieldMap.borrowerTypeFieldName].ToString());
                                    XMLOut.WriteElementString(FieldMap.parentLoanNumberFieldName, collateralRow[FieldMap.parentLoanNumberFieldName].ToString());
                                    XMLOut.WriteElementString(FieldMap.loanDescriptionFieldName, collateralRow[FieldMap.collateralDescriptionFieldName].ToString());
                                    XMLOut.WriteElementString(FieldMap.loanBranchFieldName, accountRow[FieldMap.loanBranchFieldName].ToString());
                                    XMLOut.WriteElementString(FieldMap.coreClassCodeFieldName, accountRow[FieldMap.coreClassCodeFieldName].ToString());
                                    XMLOut.WriteElementString(FieldMap.coreCollCodeFieldName, accountRow[FieldMap.coreCollCodeFieldName].ToString());
                                    XMLOut.WriteElementString(FieldMap.coreCollateralCodeFieldName, accountRow[FieldMap.coreCollateralCodeFieldName].ToString());
                                    XMLOut.WriteElementString(FieldMap.corePurposeCodeFieldName, accountRow[FieldMap.corePurposeCodeFieldName].ToString());
                                    XMLOut.WriteElementString(FieldMap.coreTypeCodeFieldName, accountRow[FieldMap.coreTypeCodeFieldName].ToString());
                                    XMLOut.WriteElementString(FieldMap.commitmentAmountFieldName, accountRow[FieldMap.commitmentAmountFieldName].ToString());
                                    XMLOut.WriteElementString(FieldMap.coreNaicsCodeFieldName, accountRow[FieldMap.coreNaicsCodeFieldName].ToString());
                                    XMLOut.WriteElementString(FieldMap.loanMaturityDateFieldName, accountRow[FieldMap.loanMaturityDateFieldName].ToString());
                                    XMLOut.WriteElementString(FieldMap.loanClassificationCodeFieldName, accountRow[FieldMap.loanClassificationCodeFieldName].ToString());
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

        public void setOwningCustomer (DataTable sourceTable, FieldMapper fieldMap)
        {
            List<int> rowsToRemove = new List<int>();

            for (int i = 0; i < sourceTable.Rows.Count; i++)
            {
                DataRow row = sourceTable.Rows[i];

                DataRow[] primaryRows = sourceTable.Select("loanNumber = '" + row[fieldMap.loanNumberFieldName].ToString() + "' AND borrowerType = ''");
                var distinctPrimaries = (from p in primaryRows
                                        select p[fieldMap.customerNumberFieldName]).Distinct().ToList().Count();
                if (distinctPrimaries != 1)
                {
                    rowsToRemove.Add(i);
                    LogFile.LogMessage(string.Format("Wrong number of primary relationships for account {0} - primary relationships: {1}",
                        row[fieldMap.loanNumberFieldName].ToString(), distinctPrimaries.ToString()));
                }
                else if (row[fieldMap.borrowerTypeFieldName].ToString() != "")
                {
                    DataRow primaryRow = primaryRows[0];
                    row[fieldMap.owningCustomerNumberFieldName] = primaryRow[fieldMap.customerNumberFieldName].ToString();
                }

            }
            foreach (int i in rowsToRemove)
            {
                sourceTable.Rows[i].Delete();
            }
            sourceTable.AcceptChanges();
        }

        public void WriteAccountsProcessedLogFile()
        {
            string[] fileNameParts = Config.AccountsProcessedLogFile.Split('.');
            string path = string.Format("{0}{1}_{2}.{3}", Config.LogFolder, fileNameParts[0], DateTime.Now.ToString().Replace("/", "_").Replace(":", "_"), fileNameParts[1]);
            string textToWrite = accountsProcessed.ToString().Replace(", ", "\r\n");
            File.WriteAllText(path, textToWrite);
        }

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


    public enum DataSources
    { 
        delimited,
        SQL,
        XML
    }
}
