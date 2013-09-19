//-----------------------------------------------------------------------------
// <copyright file="Program.cs" company="AccuSystems LLC">
//     Copyright (c) AccuSystems.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace ApplicationProcessor
{
    using AccuAccount.Data;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    /// <summary>
    /// Applies data rules to source data
    /// </summary>
    class RulesProcessor
    {
        private DataContext db;

        public DataTable TableToProcess { get; set; }
        public LogWriter LogFile { get; set; }
        public FieldMapper FieldMap { get; set; }
        public Configuration Config { get; set; }

        /// <summary>
        /// Applies data rules to source data in TableToProcess
        /// </summary>
        /// <returns>True on successfull processing</returns>
        public bool ProcessRules()
        {
            db = new DataContext();

            DataTable rulesToProcess = new DataTable();
            if (!LoadDataTableFromRulesFile(out rulesToProcess))
            {
                LogFile.LogMessage("Unable to read rules file.");
                return false;
            }
            List<int> rowsToRemove = new List<int>();

            for (int i=0; i < TableToProcess.Rows.Count; i++)
            {
                //--------- check each row of data against all rules
                DataRow row = TableToProcess.Rows[i];
                foreach (DataRow rule in rulesToProcess.Rows)
                {
                    bool removeRecordFromRule = false;
                    if (RuleMatches(row, rule))
                    {
                        try
                        {
                            ProcessAction(row, rule, TableToProcess, out removeRecordFromRule);
                        }
                        catch (Exception e)
                        {
                            LogFile.LogMessage(e.ToString());
                            LogFile.LogMessage();
                            LogFile.LogMessage("Unable to process rules at datarow " + (i + 1).ToString());
                            return false;
                        }
                    }


                    if (removeRecordFromRule)
                    {
                        rowsToRemove.Add(i);
                    }

                }
                //--------Check that all required fields have values
                if (!VerifyRequiredFields(row))
                {
                    if (!rowsToRemove.Contains(i))
                        rowsToRemove.Add(i);
                }
                //--------According to config setting - check to see if the account number already exists in the acculoan database.  If it exists, don't process the record.
                if (Config.ProcessExistingAccounts.ToUpper() != "Y")
                { 
                    if (AccountExistsInDatabase(row[FieldMap.LoanNumberFieldName].ToString()))
                        if (!rowsToRemove.Contains(i))
                            rowsToRemove.Add(i);
                }

            }
            //----------Delete records marked for removal
            for (int i = rowsToRemove.Count - 1; i >= 0; i--)
            {
                TableToProcess.Rows[rowsToRemove[i]].Delete();
            }
            TableToProcess.AcceptChanges();
            
            return true;
        }

        /// <summary>
        /// Loads rules from XML file into DataTable
        /// </summary>
        /// <param name="dataTable">Rules DataTable</param>
        /// <returns>True on successfull processing</returns>
        private bool  LoadDataTableFromRulesFile(out DataTable dataTable)
        {
            dataTable = new DataTable();

            try
            {
                XElement xml = XElement.Load(Config.RulesFile);

                dataTable.Columns.Add("Field");
                dataTable.Columns.Add("Operator");
                dataTable.Columns.Add("Value");
                dataTable.Columns.Add("Action");
                dataTable.Columns.Add("Parameter1");
                dataTable.Columns.Add("Parameter2");
                dataTable.Columns.Add("Parameter3");

                foreach (XElement element in xml.Elements("Rule"))
                {
                    DataRow newRow = dataTable.NewRow();
                    newRow["Field"] = element.Attribute("Field").Value;
                    newRow["Operator"] = element.Attribute("Operator").Value;
                    newRow["Value"] = element.Attribute("Value").Value;
                    newRow["Action"] = element.Attribute("Action").Value;
                    newRow["Parameter1"] = element.Attribute("Parameter1").Value;
                    newRow["Parameter2"] = element.Attribute("Parameter2").Value;
                    newRow["Parameter3"] = element.Attribute("Parameter3").Value;

                    dataTable.Rows.Add(newRow);
                }
                
            }
            catch (Exception e)
            {
                LogFile.LogMessage(e.ToString());
                LogFile.LogMessage();
                LogFile.LogMessage("Unable to read rules file");
                return false;
            }

            return true;

        }

        /// <summary>
        /// Checks to see if a rule is applicable to a give DataRow
        /// </summary>
        /// <param name="row">The DataRow being processed</param>
        /// <param name="rule">The rule to check</param>
        /// <returns>True if the rule applies to the current record</returns>
        private bool RuleMatches(DataRow row, DataRow rule)
        {
            bool match = false;
            string field = rule["field"].ToString();
            string fieldToCheck = row[field].ToString();
            string oper = rule["operator"].ToString();
            string value = rule["value"].ToString();

            switch (oper)
            {
                case "=":
                    if (fieldToCheck.ToUpper() == value.ToUpper())
                        match = true;
                    break;
                case "!=":
                    if (fieldToCheck.ToUpper() != value.ToUpper())
                        match = true;
                    break;
                case "<":
                    if (decimal.Parse(fieldToCheck) < decimal.Parse(value))
                        match = true;
                    break;
                case "<=":
                    if (decimal.Parse(fieldToCheck) <= decimal.Parse(value))
                        match = true;
                    break;
                case ">":
                    if (decimal.Parse(fieldToCheck) > decimal.Parse(field))
                        match = true;
                    break;
                case ">=":
                    if (decimal.Parse(fieldToCheck) >= decimal.Parse(field))
                        match = true;
                    break;
                case "Has Value":
                    if (fieldToCheck != null && fieldToCheck.Trim() != "")
                        match = true;
                    break;
                case "Has no Value":
                    if (fieldToCheck == null || fieldToCheck.Trim() == "")
                        match = true;
                    break;
                case "Always":
                    match = true;
                    break;
                default:
                    break;
            }

            return match;
        }

        /// <summary>
        /// Applies the rule logic to the current record
        /// </summary>
        /// <param name="row">The DataRow being processed</param>
        /// <param name="rule">The rule to apply</param>
        /// <param name="dataTable">The Source dataTable</param>
        /// <param name="removeRecord">Flag to indicate that the current record should not be processed</param>
        private void ProcessAction(DataRow row, DataRow rule, DataTable dataTable, out bool removeRecord)
        {
            removeRecord = false;
            string field = rule["Field"].ToString();
            string action = rule["Action"].ToString();
            string value = rule["value"].ToString();
            string parameter1 = rule["Parameter1"].ToString();
            string parameter2 = rule["Parameter2"].ToString();
            string parameter3 = rule["Parameter3"].ToString();

            switch (action)
            {
                case "Set Value to:":
                    row[field] = parameter1;
                    break;
                case "Set Value to field:":
                    row[field] = row[parameter1].ToString();
                    break;
                case "Set Other Field to:":
                    row[parameter1] = parameter2;
                    break;
                case "Skip Record":
                    LogFile.LogMessage(string.Format("Skipping record - Customer Number: {0} - Account Number: {1}.  {2} {3}",
                            row[FieldMap.CustomerNumberFieldName], row[FieldMap.LoanNumberFieldName], field, action));
                    removeRecord = true;
                    break;
                case "Combine fields with space":
                    row[field] = row[parameter1].ToString() + " " + row[parameter2].ToString();
                    break;
                case "Combine fields no space":
                    row[field] = row[parameter1].ToString() + row[parameter2].ToString();
                    break;
                case "Append field with text":
                    row[field] = row[field].ToString() + parameter1;
                    break;
                case "Prepend field with text":
                    row[field] = parameter1 + row[field].ToString();
                    break;
                case "Replace text":
                    row[field] = row[field].ToString().Replace(parameter1, parameter2);
                    break;
                case "Lookup Code from DB":
                    row[field] = LookupFromDB(row[field].ToString(), parameter1, parameter2, parameter3);
                    break;
                case "Convert to short date":
                    row[field] = DateTime.Parse(row[field].ToString()).ToShortDateString().ToString();
                    break;
                case "Next Collateral Number":
                    int nextCollateral = 1;
                    Guid loanId;
                    bool loanExists = GetLoanId(row[FieldMap.LoanNumberFieldName].ToString(), 
                        row[FieldMap.CustomerNumberFieldName].ToString(), 
                        row[FieldMap.LoanTypeCodeFieldName].ToString(), 
                        row[FieldMap.AccountClassFieldName].ToString(),
                        out loanId);
                    if (loanExists)
                        nextCollateral = GetNextCollateral(loanId);
                    int collateralFromTable = GetHighCollateralInTable(dataTable, row[FieldMap.LoanNumberFieldName].ToString());
                    if (collateralFromTable >= nextCollateral)
                        nextCollateral = collateralFromTable + 1;
                    row[field] = nextCollateral.ToString();                        
                    break;
                case "Pad Collateral Addenda":
                    string originalValue = row[field].ToString();
                    int collateralPaddingSize = GetCollateralPaddingSize();
                    if (collateralPaddingSize > originalValue.Length)
                    {
                        row[field] = int.Parse(originalValue).ToString("D" + collateralPaddingSize.ToString());
                    }
                    break;
                default:
                    break;
                
            }

        }

        /// <summary>
        /// Validates required fields for the current record
        /// </summary>
        /// <param name="row">The record to be processed</param>
        /// <returns>True if all fields are successfully validated</returns>
        private bool VerifyRequiredFields(DataRow row)
        {
            bool fieldsVerified = false;
            StringBuilder logMessage = new StringBuilder();
            
            logMessage.Append("Missing Fields: ");
            if (row[FieldMap.CustomerNumberFieldName].ToString() == "") logMessage.Append(FieldMap.CustomerNumberFieldName + " ");
            if (row[FieldMap.CustomerNameFieldName].ToString() == "") logMessage.Append(FieldMap.CustomerNameFieldName + " ");
            if (row[FieldMap.CustomerBranchFieldName].ToString() == "") logMessage.Append(FieldMap.CustomerBranchFieldName + " ");
            if (row[FieldMap.LoanBranchFieldName].ToString() == "") logMessage.Append(FieldMap.LoanBranchFieldName + " ");
            if (row[FieldMap.LoanNumberFieldName].ToString() == "") logMessage.Append(FieldMap.LoanNumberFieldName + " ");
            if (row[FieldMap.AccountClassFieldName].ToString() == "") logMessage.Append(FieldMap.AccountClassFieldName + " ");
            // if (row[FieldMap.borrowerTypeFieldName].ToString() == "") logMessage.Append(FieldMap.borrowerTypeFieldName + " ");
            // borrowertype cannot be checked for a value because a primary borrower type is defined as blank.
            if (row[FieldMap.LoanTypeCodeFieldName].ToString() == "") logMessage.Append(FieldMap.LoanTypeCodeFieldName + " ");
            if (row[FieldMap.OriginatingUserFieldName].ToString() == "") logMessage.Append(FieldMap.OriginatingUserFieldName + " ");

            if (logMessage.ToString() == "Missing Fields: ")
            {
                fieldsVerified = true;
            }
            else
            {
                if (row[FieldMap.LoanNumberFieldName].ToString() != "")
                    logMessage.Append("for account number " + row[FieldMap.LoanNumberFieldName].ToString() + " ");
                LogFile.LogMessage("Skipping Record: " + logMessage);
            }
            
            return fieldsVerified;
        }

        /// <summary>
        /// Retrieves a given field from the AccuAccount database.
        /// </summary>
        /// <param name="lookupValue">The value to lookup</param>
        /// <param name="lookupTable">The database search table</param>
        /// <param name="lookupField">The database search field</param>
        /// <param name="returnField">The database field to retrieve</param>
        /// <returns>The database value in the returnField field</returns>
        private string LookupFromDB(string lookupValue, string lookupTable, string lookupField, string returnField)
        {
            string sqlQuery = "Select " + returnField + " from [" + lookupTable + "] where " + lookupField + " = '" + lookupValue + "'";
            SqlConnection connection = new SqlConnection(Config.dbConnectionString);
            connection.Open();
            SqlDataAdapter dadapter = new SqlDataAdapter();
            dadapter.SelectCommand = new SqlCommand(sqlQuery, connection);
            DataTable dataTable = new DataTable();
            dadapter.Fill(dataTable);
            connection.Close();
            
            string result = dataTable.Rows[0][returnField].ToString();
            return result.ToString();
        }

        /// <summary>
        /// Looks up the loan id from the AccuAccount database
        /// </summary>
        /// <param name="loanNumber">The LoanNumber to lookup</param>
        /// <param name="customerNumber">The Customer Number for the loan</param>
        /// <param name="loanTypeCode">The LoanTypeCode for the loan</param>
        /// <param name="accountClass">The AccountClass for the loan</param>
        /// <param name="loanId">The corresponding loanid</param>
        /// <returns>True on successfull completion</returns>
        private bool GetLoanId(string loanNumber, string customerNumber, string loanTypeCode, string accountClass, out Guid loanId)
        {
            loanId = new Guid();
            bool found = false;

            //get duplication mode
            string dupMode = (from p in db.AccuSystemsProperties
                           where p.PropertyKey == "accuimporter.LoanDuplicationMode"
                           select p.PropertyValue).FirstOrDefault().ToString();
            switch (dupMode)
            {
                case "None":
                    var loans0 = from l in db.Loans
                                 where l.LoanNumber == loanNumber
                                 select l;
                    if (loans0.Count() == 1)
                    {
                        loanId = loans0.FirstOrDefault().LoanId;
                        found = true;
                    }
                    break;
                case "CustomerAndLoan":
                    var loans1 = from l in db.Loans
                             where l.LoanNumber == loanNumber
                             from c in db.Customers
                             where c.CustomerNumber == customerNumber
                             select l;
                    if (loans1.Count() == 1)
                    {
                        loanId = loans1.FirstOrDefault().LoanId;
                        found = true;
                    }
                    break;

                case "CustomerAndLoanAndAccountClass":
                    var loans2 = from l in db.Loans
                             where l.LoanNumber == loanNumber
                             from c in db.Customers
                             where c.CustomerNumber == customerNumber
                             from ac in db.AccountClasses
                             where ac.AccountClassName == accountClass
                             select l;
                    if (loans2.Count() == 1)
                    {
                        loanId = loans2.FirstOrDefault().LoanId;
                        found = true;
                    }
                    break;

                case "CustomerAndLoanAndLoanTypeAndAccountClass":
                    var loans3 = from l in db.Loans
                                 where l.LoanNumber == loanNumber
                                 from c in db.Customers
                                 where c.CustomerNumber == customerNumber
                                 from lt in db.LoanTypes
                                 where lt.LoanTypeCode == loanTypeCode
                                 from ac in db.AccountClasses
                                 where ac.AccountClassName == accountClass
                                 select l;
                    if (loans3.Count() == 1)
                    {
                        loanId = loans3.FirstOrDefault().LoanId;
                        found = true;
                    }
                    break;

                default:
                    break;
            }
            return found;

        }

        /// <summary>
        /// Gets the next collateral number for a loan
        /// </summary>
        /// <param name="loanId">The LoanId to lookup</param>
        /// <returns>The next collateral number for the loan</returns>
        private int GetNextCollateral(Guid loanId)
        {
            int nextCollateral = 1;

            var collaterals = from c in db.Collaterals
                              where c.ParentLoanId == loanId
                              orderby c.CollateralSequence descending
                              select c;
            if (collaterals.Count() > 0)
                nextCollateral = (int)collaterals.FirstOrDefault().CollateralSequence + 1;
            return nextCollateral;
        }

        /// <summary>
        /// Gets the highest collateral number for a loan in the DataTable
        /// </summary>
        /// <param name="dataTable">The source DataTable</param>
        /// <param name="loanNumber">The LoanNumber to lookup</param>
        /// <returns>The highest collateral number in the database for the LoanNumber</returns>
        private int GetHighCollateralInTable(DataTable dataTable, string loanNumber)
        {
            int highCollateral = 0;
            DataRow[] collateralRows = dataTable.Select(FieldMap.LoanNumberFieldName + " = '" + loanNumber + 
                "' AND " + FieldMap.BorrowerTypeFieldName + " = ''", FieldMap.CollateralAddendaFieldName + " Desc" );
            if (collateralRows.Count() > 0)
            {
                string topCollateral = collateralRows[0][FieldMap.CollateralAddendaFieldName].ToString();
                if (topCollateral == "") topCollateral = "0";
                highCollateral = int.Parse(topCollateral);
            }

            return highCollateral;
        }

        /// <summary>
        /// Gets the CollateralPaddingSize setting from the AccuAccount database
        /// </summary>
        /// <returns>The CollateralPaddingSize</returns>
        private int GetCollateralPaddingSize()
        {
            int paddingSize = 0;
            string paddingSizeString = (from p in db.AccuSystemsProperties
                               where p.PropertyKey == "accuaccount.collateralPadSize"
                               select p.PropertyValue).FirstOrDefault();
            try
            {
                paddingSize = int.Parse(paddingSizeString);
            }
            catch
            {
            }
            return paddingSize;

        }

        /// <summary>
        /// Check to see if an AccountNumber exists in the AccuAccount database
        /// </summary>
        /// <param name="accountNumber">The Account Number to check</param>
        /// <returns>True if the account number exists in the database</returns>
        private bool AccountExistsInDatabase(string accountNumber)
        {
            int accounts = (from l in db.Loans
                            where l.LoanNumber == accountNumber
                            select l).Count();


            if (accounts > 0)
            {
                LogFile.LogMessage("accountNumber " + accountNumber + " already exists");
                return true;
            }
            else
                return false;
        }
    }
}
