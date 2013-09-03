using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using System.Reflection;
using System.Xml;

namespace ApplicationProcessor
{
    class ProcessedDataWriter
    {
        public LogWriter LogFile { get; set; }
        public Configuration Config { get; set; }
        public FieldMapper FieldMap { get; set; }

        public void WriteProcessedData(DataTable dataToWrite, string processedDataFile)
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

            DataTable uniqueCustomers = dataToWrite.DefaultView.ToTable(true, FieldMap.customerNumberFieldName, FieldMap.customerNameFieldName,
                FieldMap.taxIdFieldName, FieldMap.customerBranchFieldName, FieldMap.customerOfficerCodeFieldName);

            try
            {
                using (XmlWriter XMLOut = XmlWriter.Create(Config.OutputFile, xmlRules))
                {
                    XMLOut.WriteStartDocument();
                    XMLOut.WriteStartElement("AccuSystems");

                    foreach (DataRow customer in uniqueCustomers.Rows)
                    {
                        XMLOut.WriteStartElement("customer");

                        XMLOut.WriteElementString(FieldMap.customerNumberFieldName, customer[FieldMap.customerNumberFieldName].ToString());
                        XMLOut.WriteElementString(FieldMap.customerNameFieldName, customer[FieldMap.customerNameFieldName].ToString());
                        XMLOut.WriteElementString(FieldMap.taxIdFieldName, customer[FieldMap.taxIdFieldName].ToString());
                        XMLOut.WriteElementString(FieldMap.customerBranchFieldName, customer[FieldMap.customerBranchFieldName].ToString());
                        XMLOut.WriteElementString(FieldMap.customerOfficerCodeFieldName, customer[FieldMap.customerOfficerCodeFieldName].ToString());

                        DataRow[] accountRows = dataToWrite.Select(FieldMap.customerNumberFieldName + " = '" + customer[FieldMap.customerNumberFieldName].ToString() + "'");

                        foreach (DataRow accountRow in accountRows)
                        {
                            XMLOut.WriteStartElement("loan");
                            XMLOut.WriteElementString(FieldMap.loanNumberFieldName, accountRow[FieldMap.loanNumberFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.customerNumberFieldName, accountRow[FieldMap.customerNumberFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.accountClassFieldName, accountRow[FieldMap.accountClassFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.loanTypeCodeFieldName, accountRow[FieldMap.loanTypeCodeFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.loanStatusCodeFieldName, accountRow[FieldMap.loanStatusCodeFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.loanAmountFieldName, accountRow[FieldMap.loanAmountFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.loanOriginationDateFieldName, accountRow[FieldMap.loanOriginationDateFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.loanDescriptionFieldName, accountRow[FieldMap.loanDescriptionFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.borrowerTypeFieldName, accountRow[FieldMap.borrowerTypeFieldName].ToString());
                            XMLOut.WriteElementString(FieldMap.owningCustomerNumberFieldName, accountRow[FieldMap.owningCustomerNumberFieldName].ToString());

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
                                XMLOut.WriteStartElement("loan");
                                XMLOut.WriteElementString(FieldMap.loanNumberFieldName, accountRow[FieldMap.collateralLoanNumberFieldName].ToString());
                                XMLOut.WriteElementString(FieldMap.customerNumberFieldName, accountRow[FieldMap.customerNumberFieldName].ToString());
                                XMLOut.WriteElementString(FieldMap.accountClassFieldName, accountRow[FieldMap.accountClassFieldName].ToString());
                                XMLOut.WriteElementString(FieldMap.loanTypeCodeFieldName, accountRow[FieldMap.collateralLoanTypeCodeFieldName].ToString());
                                XMLOut.WriteElementString(FieldMap.parentLoanNumberFieldName, accountRow[FieldMap.parentLoanNumberFieldName].ToString());
                                XMLOut.WriteElementString(FieldMap.loanStatusCodeFieldName, accountRow[FieldMap.loanStatusCodeFieldName].ToString());
                                XMLOut.WriteElementString(FieldMap.loanAmountFieldName, accountRow[FieldMap.loanAmountFieldName].ToString());
                                XMLOut.WriteElementString(FieldMap.loanOriginationDateFieldName, accountRow[FieldMap.loanOriginationDateFieldName].ToString());
                                XMLOut.WriteElementString(FieldMap.loanDescriptionFieldName, accountRow[FieldMap.collateralDescriptionFieldName].ToString());
                                XMLOut.WriteEndElement();
                            }

                        }

                        XMLOut.WriteEndElement();
                    }

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

    }
}
