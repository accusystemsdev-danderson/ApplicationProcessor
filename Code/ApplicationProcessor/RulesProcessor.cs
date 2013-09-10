using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Xml.Linq;
using System.Reflection;
using System.Data.SqlClient;
using AccuAccount.Data;


namespace ApplicationProcessor
{
    class RulesProcessor
    {
        public DataTable TableToProcess { get; set; }
        public LogWriter logFile { get; set; }
        public FieldMapper FieldMap { get; set; }
        public Configuration Config { get; set; }

        private DataContext db;
        
        public bool ProcessRules()
        {
                db = new DataContext(Config.dbConnectionString);

            DataTable rulesToProcess = new DataTable();
            if (!loadDataTableFromRulesFile(out rulesToProcess))
            {
                logFile.LogMessage("Unable to read rules file.");
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
                    if (ruleMatches(row, rule))
                    {
                        try
                        {
                            processAction(row, rule, TableToProcess, out removeRecordFromRule);
                        }
                        catch (Exception e)
                        {
                            logFile.LogMessage(e.ToString());
                            logFile.LogMessage();
                            logFile.LogMessage("Unable to process rules at datarow " + (i + 1).ToString());
                            return false;
                        }
                    }


                    if (removeRecordFromRule)
                    {
                        rowsToRemove.Add(i);
                    }

                }
                //--------Check that all required fields have values
                if (!verifyRequiredFields(row))
                {
                    if (!rowsToRemove.Contains(i))
                        rowsToRemove.Add(i);
                }
                //--------According to config setting - check to see if the account number already exists in the acculoan database.  If it exists, don't process the record.
                if (Config.ProcessExistingAccounts.ToUpper() != "Y")
                { 
                    if (accountExistsInDatabase(row[FieldMap.loanNumberFieldName].ToString()))
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

        private bool  loadDataTableFromRulesFile(out DataTable dataTable)
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
                logFile.LogMessage(e.ToString());
                logFile.LogMessage();
                logFile.LogMessage("Unable to read rules file");
                return false;
            }

            return true;

        }

        private bool ruleMatches(DataRow row, DataRow rule)
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

        private void processAction(DataRow row, DataRow rule, DataTable dataTable, out bool removeRecord)
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
                    logFile.LogMessage(string.Format("Skipping record - Customer Number: {0} - Account Number: {1}.  {2} {3}",
                            row[FieldMap.customerNumberFieldName], row[FieldMap.loanNumberFieldName], field, action));
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
                    row[field] = lookupFromDB(row[field].ToString(), parameter1, parameter2, parameter3);
                    break;
                case "Convert to short date":
                    row[field] = DateTime.Parse(row[field].ToString()).ToShortDateString().ToString();
                    break;
                case "Next Collateral Number":
                    int nextCollateral = 1;
                    Guid loanId;
                    bool loanExists = getLoanId(row[FieldMap.loanNumberFieldName].ToString(), 
                        row[FieldMap.customerNumberFieldName].ToString(), 
                        row[FieldMap.loanTypeCodeFieldName].ToString(), 
                        row[FieldMap.accountClassFieldName].ToString(),
                        out loanId);
                    if (loanExists)
                        nextCollateral = getNextCollateral(loanId);
                    int collateralFromTable = getHighCollateralInTable(dataTable, row[FieldMap.loanNumberFieldName].ToString());
                    if (collateralFromTable >= nextCollateral)
                        nextCollateral = collateralFromTable + 1;
                    row[field] = nextCollateral.ToString();                        
                    break;
                case "Pad Collateral Addenda":
                    string originalValue = row[field].ToString();
                    int collateralPaddingSize = getCollateralPaddingSize();
                    if (collateralPaddingSize > originalValue.Length)
                    {
                        row[field] = int.Parse(originalValue).ToString("D" + collateralPaddingSize.ToString());
                    }
                    break;
                default:
                    break;
                
            }

        }

        private bool verifyRequiredFields(DataRow row)
        {
            bool fieldsVerified = false;
            StringBuilder logMessage = new StringBuilder();
            
            logMessage.Append("Missing Fields: ");
            if (row[FieldMap.customerNumberFieldName].ToString() == "") logMessage.Append(FieldMap.customerNumberFieldName + " ");
            if (row[FieldMap.customerNameFieldName].ToString() == "") logMessage.Append(FieldMap.customerNameFieldName + " ");
            if (row[FieldMap.customerBranchFieldName].ToString() == "") logMessage.Append(FieldMap.customerBranchFieldName + " ");
            if (row[FieldMap.loanBranchFieldName].ToString() == "") logMessage.Append(FieldMap.loanBranchFieldName + " ");
            if (row[FieldMap.loanNumberFieldName].ToString() == "") logMessage.Append(FieldMap.loanNumberFieldName + " ");
            if (row[FieldMap.accountClassFieldName].ToString() == "") logMessage.Append(FieldMap.accountClassFieldName + " ");
            // if (row[FieldMap.borrowerTypeFieldName].ToString() == "") logMessage.Append(FieldMap.borrowerTypeFieldName + " ");
            // borrowertype cannot be checked for a value because a primary borrower type is defined as blank.
            if (row[FieldMap.loanTypeCodeFieldName].ToString() == "") logMessage.Append(FieldMap.loanTypeCodeFieldName + " ");
            if (row[FieldMap.originatingUserFieldName].ToString() == "") logMessage.Append(FieldMap.originatingUserFieldName + " ");

            if (logMessage.ToString() == "Missing Fields: ")
            {
                fieldsVerified = true;
            }
            else
            {
                if (row[FieldMap.loanNumberFieldName].ToString() != "")
                    logMessage.Append("for account number " + row[FieldMap.loanNumberFieldName].ToString() + " ");
                logFile.LogMessage("Skipping Record: " + logMessage);
            }
            
            return fieldsVerified;
        }

        private string lookupFromDB(string lookupValue, string lookupTable, string lookupField, string returnField)
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

        private bool getLoanId(string loanNumber, string customerNumber, string loanTypeCode, string accountClass, out Guid loanId)
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

        private int getNextCollateral(Guid loanId)
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

        private int getHighCollateralInTable(DataTable dataTable, string loanNumber)
        {
            int highCollateral = 0;
            DataRow[] collateralRows = dataTable.Select(FieldMap.loanNumberFieldName + " = '" + loanNumber + 
                "' AND " + FieldMap.borrowerTypeFieldName + " = ''", FieldMap.collateralAddendaFieldName + " Desc" );
            if (collateralRows.Count() > 0)
            {
                string topCollateral = collateralRows[0][FieldMap.collateralAddendaFieldName].ToString();
                if (topCollateral == "") topCollateral = "0";
                highCollateral = int.Parse(topCollateral);
            }

            return highCollateral;
        }

        private int getCollateralPaddingSize()
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

        private bool accountExistsInDatabase(string accountNumber)
        {
            int accounts = (from l in db.Loans
                            where l.LoanNumber == accountNumber
                            select l).Count();


            if (accounts > 0)
            {
                logFile.LogMessage("accountNumber " + accountNumber + " already exists");
                return true;
            }
            else
                return false;
        }
    }
}
