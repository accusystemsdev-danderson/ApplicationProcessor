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
        public void WriteProcessedData(DataTable dataToWrite, string processedDataFile)
        {
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

        public void WriteXMLData(DataTable dataToWrite, string XMLFile, string processCollaterals, FieldMapper fieldMap)
        {            
            XmlWriterSettings xmlRules = new XmlWriterSettings();
            xmlRules.Indent = true;
            xmlRules.NewLineOnAttributes = true;
            xmlRules.OmitXmlDeclaration = true;
            xmlRules.Encoding = Encoding.Default;

            DataTable uniqueCustomers = dataToWrite.DefaultView.ToTable(true, fieldMap.customerNumberFieldName, fieldMap.customerNameFieldName,
                fieldMap.taxIdFieldName, fieldMap.customerBranchFieldName, fieldMap.customerOfficerCodeFieldName);

            using (XmlWriter XMLOut = XmlWriter.Create(XMLFile, xmlRules))
            {
                XMLOut.WriteStartDocument();
                XMLOut.WriteStartElement("AccuSystems");

                foreach (DataRow customer in uniqueCustomers.Rows)
                {
                    XMLOut.WriteStartElement("customer");

                    XMLOut.WriteElementString(fieldMap.customerNumberFieldName, customer[fieldMap.customerNumberFieldName].ToString());
                    XMLOut.WriteElementString(fieldMap.customerNameFieldName, customer[fieldMap.customerNameFieldName].ToString());
                    XMLOut.WriteElementString(fieldMap.taxIdFieldName, customer[fieldMap.taxIdFieldName].ToString());
                    XMLOut.WriteElementString(fieldMap.customerBranchFieldName, customer[fieldMap.customerBranchFieldName].ToString());
                    XMLOut.WriteElementString(fieldMap.customerOfficerCodeFieldName, customer[fieldMap.customerOfficerCodeFieldName].ToString());

                    DataRow[] accountRows = dataToWrite.Select(fieldMap.customerNumberFieldName + " = '" + customer[fieldMap.customerNumberFieldName].ToString() + "'");
                    
                    foreach (DataRow accountRow in accountRows)
                    {
                        XMLOut.WriteStartElement("loan");
                        XMLOut.WriteElementString(fieldMap.loanNumberFieldName, accountRow[fieldMap.loanNumberFieldName].ToString());
                        XMLOut.WriteElementString(fieldMap.customerNumberFieldName, accountRow[fieldMap.customerNumberFieldName].ToString());
                        XMLOut.WriteElementString(fieldMap.accountClassFieldName, accountRow[fieldMap.accountClassFieldName].ToString());
                        XMLOut.WriteElementString(fieldMap.loanTypeCodeFieldName, accountRow[fieldMap.loanTypeCodeFieldName].ToString());
                        XMLOut.WriteElementString(fieldMap.loanStatusCodeFieldName, accountRow[fieldMap.loanStatusCodeFieldName].ToString());
                        XMLOut.WriteElementString(fieldMap.loanAmountFieldName, accountRow[fieldMap.loanAmountFieldName].ToString());
                        XMLOut.WriteElementString(fieldMap.loanOriginationDateFieldName, accountRow[fieldMap.loanOriginationDateFieldName].ToString());
                        XMLOut.WriteElementString(fieldMap.loanDescriptionFieldName, accountRow[fieldMap.loanDescriptionFieldName].ToString());
                        XMLOut.WriteElementString(fieldMap.borrowerTypeFieldName, accountRow[fieldMap.borrowerTypeFieldName].ToString());
                        XMLOut.WriteElementString(fieldMap.owningCustomerNumberFieldName, accountRow[fieldMap.owningCustomerNumberFieldName].ToString());

                            XMLOut.WriteStartElement("application");
                            XMLOut.WriteElementString(fieldMap.applicationDateFieldName, accountRow[fieldMap.applicationDateFieldName].ToString());
                            XMLOut.WriteElementString(fieldMap.creditAnalysisStatusFieldName, accountRow[fieldMap.creditAnalysisStatusFieldName].ToString());
                            XMLOut.WriteElementString(fieldMap.requestedAmountFieldName, accountRow[fieldMap.requestedAmountFieldName].ToString());
                            XMLOut.WriteElementString(fieldMap.primaryCollateralValueFieldName, accountRow[fieldMap.primaryCollateralValueFieldName].ToString());
                            XMLOut.WriteElementString(fieldMap.FICOFieldName, accountRow[fieldMap.FICOFieldName].ToString());
                            XMLOut.WriteElementString(fieldMap.valuationDateFieldName, accountRow[fieldMap.valuationDateFieldName].ToString());
                            XMLOut.WriteElementString(fieldMap.interestRateFieldName, accountRow[fieldMap.interestRateFieldName].ToString());
                            XMLOut.WriteElementString(fieldMap.probabilityFieldName, accountRow[fieldMap.probabilityFieldName].ToString());
                            XMLOut.WriteElementString(fieldMap.estimatedCloseDateFieldName, accountRow[fieldMap.estimatedCloseDateFieldName].ToString());
                            XMLOut.WriteStartElement(fieldMap.assignedLenderFieldName);
                            XMLOut.WriteAttributeString("type", accountRow[fieldMap.assignedLenderTypeFieldName].ToString());
                            XMLOut.WriteValue(accountRow[fieldMap.assignedLenderFieldName].ToString());
                            XMLOut.WriteEndElement();
                            XMLOut.WriteStartElement(fieldMap.assignedAnalystFieldName);
                            XMLOut.WriteAttributeString("type", accountRow[fieldMap.assignedAnalystTypeFieldName].ToString());
                            XMLOut.WriteValue(accountRow[fieldMap.assignedAnalystFieldName].ToString());
                            XMLOut.WriteEndElement();
                            XMLOut.WriteStartElement(fieldMap.assignedLoanProcessorFieldName);
                            XMLOut.WriteAttributeString("type", accountRow[fieldMap.assignedLoanProcessorTypeFieldName].ToString());
                            XMLOut.WriteValue(accountRow[fieldMap.assignedLoanProcessorFieldName].ToString());
                            XMLOut.WriteEndElement();
                            XMLOut.WriteElementString(fieldMap.applicationLockedFieldName, accountRow[fieldMap.applicationLockedFieldName].ToString());

                                XMLOut.WriteStartElement("approval");
                                XMLOut.WriteElementString(fieldMap.approvalStatusFieldName, accountRow[fieldMap.approvalStatusFieldName].ToString());
                                XMLOut.WriteElementString(fieldMap.originatingUserFieldName, accountRow[fieldMap.originatingUserFieldName].ToString());
                                XMLOut.WriteElementString(fieldMap.assignedApproverFieldName, accountRow[fieldMap.assignedApproverFieldName].ToString());
                                XMLOut.WriteStartElement(fieldMap.assignedApproverFieldName);
                                XMLOut.WriteAttributeString("type", accountRow[fieldMap.assignedApproverTypeFieldName].ToString());
                                XMLOut.WriteValue(accountRow[fieldMap.assignedApproverFieldName].ToString());
                                XMLOut.WriteEndElement();

                                XMLOut.WriteEndElement();

                            XMLOut.WriteEndElement();
    
                        XMLOut.WriteEndElement();

                        if (processCollaterals == "Y")
                        {
                            XMLOut.WriteStartElement("loan");
                            XMLOut.WriteElementString(fieldMap.loanNumberFieldName, accountRow[fieldMap.collateralLoanNumberFieldName].ToString());
                            XMLOut.WriteElementString(fieldMap.customerNumberFieldName, accountRow[fieldMap.customerNumberFieldName].ToString());
                            XMLOut.WriteElementString(fieldMap.accountClassFieldName, accountRow[fieldMap.accountClassFieldName].ToString());
                            XMLOut.WriteElementString(fieldMap.loanTypeCodeFieldName, accountRow[fieldMap.collateralLoanTypeCodeFieldName].ToString());
                            XMLOut.WriteElementString(fieldMap.parentLoanNumberFieldName, accountRow[fieldMap.parentLoanNumberFieldName].ToString());
                            XMLOut.WriteElementString(fieldMap.loanStatusCodeFieldName, accountRow[fieldMap.loanStatusCodeFieldName].ToString());
                            XMLOut.WriteElementString(fieldMap.loanAmountFieldName, accountRow[fieldMap.loanAmountFieldName].ToString());
                            XMLOut.WriteElementString(fieldMap.loanOriginationDateFieldName, accountRow[fieldMap.loanOriginationDateFieldName].ToString());
                            XMLOut.WriteElementString(fieldMap.loanDescriptionFieldName, accountRow[fieldMap.collateralDescriptionFieldName].ToString());
                            XMLOut.WriteEndElement();
                        }
                        
                    }

                    XMLOut.WriteEndElement();
                }
                
            }

        }

    }
}
