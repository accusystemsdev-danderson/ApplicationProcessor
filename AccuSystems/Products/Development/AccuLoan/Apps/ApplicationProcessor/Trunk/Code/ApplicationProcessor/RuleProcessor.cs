//-----------------------------------------------------------------------------
// <copyright file="RuleProcessor.cs" company="AccuSystems LLC">
//     Copyright (c) AccuSystems.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace ApplicationProcessor
{
    using AccuAccount.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Xml.Linq;

    /// <summary>
    /// Applies data rules to source data
    /// </summary>
    public static class RuleProcessor
    {
        private static DataContext db = new DataContext();
        private static List<RuleDefinition> Rules = new List<RuleDefinition>();
        private static Dictionary<string, int> highCollateral = new Dictionary<string,int>();

        /// <summary>
        /// Applies data rules to source data in TableToProcess
        /// </summary>
        /// <returns>True on successfull processing</returns>
        public static bool ProcessRules(SourceRecord record)
        {
            
            foreach (RuleDefinition rule in Rules)
            {
                if (RuleMatches(record, rule))
                {
                    try
                    {
                        ProcessAction(record, rule);
                    }
                    catch (Exception e)
                    {
                        LogWriter.LogMessage(e.ToString());
                        LogWriter.LogMessage();
                        LogWriter.LogMessage("Unable to process rules for account" + record.LoanNumber);
                        return false;
                    }
                    
                }

            }
            SetHighCollateral(record);
            return true;
        }

        /// <summary>
        /// Loads rules from XML file
        /// </summary>
        /// <returns>True on successfull processing</returns>
        public static bool LoadRulesFromFile()
        {
            XElement xml;
            try
            {
                xml = XElement.Load(Configuration.RulesFile);
            }
            catch (Exception e)
            {
                LogWriter.LogMessage(e.ToString());
                LogWriter.LogMessage();
                LogWriter.LogMessage("Unable to read rules file");
                return false;
            }

            foreach (XElement element in xml.Elements("Rule"))
            {
                RuleDefinition rule = new RuleDefinition();

                rule.Field = element.Attribute("Field").Value;
                rule.Operator = element.Attribute("Operator").Value;
                rule.Value = element.Attribute("Value").Value;
                rule.Action = element.Attribute("Action").Value;
                rule.Parameter1 = element.Attribute("Parameter1").Value;
                rule.Parameter2 = element.Attribute("Parameter2").Value;
                rule.Parameter3 = element.Attribute("Parameter3").Value;

                Rules.Add(rule);
            }
             
            return true;

        }

        /// <summary>
        /// Checks to see if a rule is applicable to a give DataRow
        /// </summary>
        /// <param name="sourceRecord">The DataRow being processed</param>
        /// <param name="rule">The rule to check</param>
        /// <returns>True if the rule applies to the current record</returns>
        private static bool RuleMatches(SourceRecord sourceRecord, RuleDefinition rule)
        {
            bool match = false;
            var fieldToCheck = sourceRecord.GetType().GetProperty(rule.Field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).GetValue(sourceRecord, null);
            string valueOfFieldToCheck = "";
            if (fieldToCheck != null)
            {
                valueOfFieldToCheck = fieldToCheck.ToString();
            }
            
            switch (rule.Operator)
            {
                case "=":
                    if (valueOfFieldToCheck.Equals(rule.Value,StringComparison.OrdinalIgnoreCase))
                        match = true;
                    break;
                case "!=":
                    if (!valueOfFieldToCheck.Equals(rule.Value, StringComparison.OrdinalIgnoreCase))
                        match = true;
                    break;
                case "<":
                    if (decimal.Parse(valueOfFieldToCheck) < decimal.Parse(rule.Value))
                        match = true;
                    break;
                case "<=":
                    if (decimal.Parse(valueOfFieldToCheck) <= decimal.Parse(rule.Value))
                        match = true;
                    break;
                case ">":
                    if (decimal.Parse(valueOfFieldToCheck) > decimal.Parse(rule.Value))
                        match = true;
                    break;
                case ">=":
                    if (decimal.Parse(valueOfFieldToCheck) >= decimal.Parse(rule.Value))
                        match = true;
                    break;
                case "Has Value":
                    if (valueOfFieldToCheck != null && valueOfFieldToCheck.Trim() != "")
                        match = true;
                    break;
                case "Has no Value":
                    if (valueOfFieldToCheck == null || valueOfFieldToCheck.Trim() == "")
                        match = true;
                    break;
                case "Contains Text":
                    if (valueOfFieldToCheck.IndexOf(rule.Value, StringComparison.OrdinalIgnoreCase) >= 0)
                        match = true;
                    break;
                case "Contained in Text":
                    if (rule.Value.IndexOf(valueOfFieldToCheck, StringComparison.OrdinalIgnoreCase) >= 0)
                        match = true;
                    break;
                case "Always":
                    match = true;
                    break;
            }

            return match;
        }

        /// <summary>
        /// Applies the rule logic to the current record
        /// </summary>
        /// <param name="record">The <see cref="SourceRecord"/> to apply the rule to</param>
        /// <param name="rule">The rule to apply</param>
        private static void ProcessAction(SourceRecord record, RuleDefinition rule)
        {
            PropertyInfo fieldProperty = record.GetType().GetProperty(rule.Field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            PropertyInfo parameter1Property = record.GetType().GetProperty(rule.Parameter1, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            PropertyInfo parameter2Property = record.GetType().GetProperty(rule.Parameter2, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            PropertyInfo parameter3Property = record.GetType().GetProperty(rule.Parameter3, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            string fieldValue = fieldProperty != null ? fieldProperty.GetValue(record, null).ToString() : "";
            string parameter1FieldValue = parameter1Property != null ? parameter1Property.GetValue(record, null).ToString() : "";
            string parameter2FieldValue = parameter2Property != null ? parameter2Property.GetValue(record, null).ToString() : "";
            string parameter3FieldValue = parameter3Property != null ? parameter3Property.GetValue(record, null).ToString() : "";
            
            switch (rule.Action)
            {
                case "Set Value to:":
                    fieldProperty.SetValue(record, rule.Parameter1, null);
                    break;
                case "Set Value to field:":
                    fieldProperty.SetValue(record, parameter1FieldValue, null);
                    break;
                case "Set Other Field to:":
                    parameter1Property.SetValue(record, rule.Parameter2, null);
                    break;
                case "Skip Record":
                    LogWriter.LogMessage(string.Format("Skipping record - Customer Number: {0} - Account Number: {1}.  {2} {3}",
                            record.CustomerNumber, record.LoanNumber, rule.Field, rule.Action));
                    record.IgnoreRecord = true;
                    break;
                case "Combine fields with space":
                    fieldProperty.SetValue(record, parameter1FieldValue + " " + parameter2FieldValue, null);
                    break;
                case "Combine fields no space":
                    fieldProperty.SetValue(record, parameter1FieldValue + parameter2FieldValue, null);
                    break;
                case "Append field with text":
                    fieldProperty.SetValue(record, fieldValue + rule.Parameter1, null); 
                    break;
                case "Prepend field with text":
                    fieldProperty.SetValue(record, rule.Parameter1 + fieldValue, null); 
                    break;
                case "Replace text":
                    fieldProperty.SetValue(record, fieldValue.Replace(rule.Parameter1, rule.Parameter2), null);
                    break;
                case "Lookup Code from DB":
                    fieldProperty.SetValue(record, LookupFromDB(fieldValue, rule.Parameter1, rule.Parameter2, rule.Parameter3), null);
                    break;
                case "Convert to short date":
                    fieldProperty.SetValue(record, DateTime.Parse(fieldValue).ToShortDateString(), null);
                    break;
                case "Next Collateral Number":
                    int nextCollateral = 1;
                    Guid loanId;
                    bool loanExists = GetLoanId(record.LoanNumber,
                        record.CustomerNumber, 
                        record.LoanTypeCode, 
                        record.AccountClass,
                        out loanId);
                    if (loanExists)
                        nextCollateral = GetNextCollateral(loanId);
                    int collateralFromDictionary = GetHighCollateralFromDictionary(record.LoanNumber);
                    if (collateralFromDictionary >= nextCollateral)
                        nextCollateral = collateralFromDictionary + 1;
                    fieldProperty.SetValue(record, nextCollateral.ToString(), null);
                    break;
                case "Pad Collateral Addenda":
                    string originalValue = fieldValue;
                    int collateralPaddingSize = GetCollateralPaddingSize();
                    if (collateralPaddingSize > originalValue.Length)
                    {
                        fieldProperty.SetValue(record, int.Parse(originalValue).ToString("D" + collateralPaddingSize), null);
                    }
                    break;
               
            }

        }

        /// <summary>
        /// Validates required fields for the current record
        /// </summary>
        /// <param name="record">The <see cref="SourceRecord"/> to be processed</param>
        /// <returns>True if all fields are successfully validated</returns>
        public static void VerifyRequiredFields(SourceRecord record)
        {
            StringBuilder logMessage = new StringBuilder();
            
            logMessage.Append("Missing Fields: ");
            if (record.CustomerNumber == "") logMessage.Append(FieldMap.CustomerNumberFieldName + " ");
            if (record.CustomerName == "") logMessage.Append(FieldMap.CustomerNameFieldName + " ");
            if (record.CustomerBranch == "") logMessage.Append(FieldMap.CustomerBranchFieldName + " ");
            if (record.CustomerOfficerCode == "") logMessage.Append(FieldMap.CustomerOfficerCodeFieldName + " ");
            if (record.LoanBranch == "") logMessage.Append(FieldMap.LoanBranchFieldName + " ");
            if (record.LoanNumber == "") logMessage.Append(FieldMap.LoanNumberFieldName + " ");
            if (record.AccountClass == "") logMessage.Append(FieldMap.AccountClassFieldName + " ");
            // if (record.borrowerType == "") logMessage.Append(FieldMap.borrowerTypeFieldName + " ");
            // borrowertype cannot be checked for a value because a primary borrower type is defined as blank.
            if (record.LoanTypeCode == "") logMessage.Append(FieldMap.LoanTypeCodeFieldName + " ");
            if (record.OriginatingUser == "") logMessage.Append(FieldMap.OriginatingUserFieldName + " ");

            if (logMessage.ToString() != "Missing Fields: ")
            {
                if (record.LoanNumber != "")
                    logMessage.Append("for account number " + record.LoanNumber + " ");
                LogWriter.LogMessage("Skipping Record: " + logMessage);
                record.IgnoreRecord = true;
            }

        }

        /// <summary>
        /// Retrieves a given field from the AccuAccount database.
        /// </summary>
        /// <param name="lookupValue">The value to lookup</param>
        /// <param name="lookupTable">The database search table</param>
        /// <param name="lookupField">The database search field</param>
        /// <param name="selectField">The database field to retrieve</param>
        /// <returns>The database value in the selectField field, or string.Empty if number of results != 1</returns>
        private static string LookupFromDB(string lookupValue, string lookupTable, string lookupField, string selectField)
        {
            string sqlQuery = "Select " + selectField + " from [" + lookupTable + "] where " + lookupField + " = '" + lookupValue + "'";
            
            using (DataContext db = new DataContext())
            {
                if (selectField.EndsWith("ID", StringComparison.OrdinalIgnoreCase))
                {
                    var results = db.Database.SqlQuery<Guid>(sqlQuery).ToList();

                    if (results.Count() == 1)
                    {
                        return results.First().ToString();
                    }
                }
                else
                {
                    var results = db.Database.SqlQuery<string>(sqlQuery).ToList();

                    if (results.Count() == 1)
                    {
                        return results.First();
                    }
                }

            }

            return string.Empty;
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
        private static bool GetLoanId(string loanNumber, string customerNumber, string loanTypeCode, string accountClass, out Guid loanId)
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
        /// Sets the dictionary entry for the high collateral addenda for a loan number.
        /// </summary>
        /// <param name="record">The record to get the high collateral for.</param>
        private static void SetHighCollateral(SourceRecord record)
        {
            int collateralFromRecord = 0;
            bool recordHasCollateralAddenda = int.TryParse(record.CollateralAddenda, out collateralFromRecord);
            int collateralFromDictionary = 0;
            if (highCollateral.TryGetValue(record.LoanNumber, out collateralFromDictionary))
            {
                if (collateralFromDictionary < collateralFromRecord)
                {
                    highCollateral[record.LoanNumber] = collateralFromRecord;
                }
            }
            else
            {
                if (recordHasCollateralAddenda)
                {
                    highCollateral.Add(record.LoanNumber, collateralFromRecord);
                }
            }
        }

        /// <summary>
        /// Gets the next collateral number for a loan
        /// </summary>
        /// <param name="loanId">The LoanId to lookup</param>
        /// <returns>The next collateral number for the loan</returns>
        private static int GetNextCollateral(Guid loanId)
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
        /// Gets the highest collateral number for a loan from dictionary
        /// </summary>
        /// <param name="loanNumber">The LoanNumber to lookup</param>
        /// <returns>The highest collateral number in the database for the LoanNumber</returns>
        private static int GetHighCollateralFromDictionary(string loanNumber)
        {
            int collateralFromDictionary = 0;
            highCollateral.TryGetValue(loanNumber, out collateralFromDictionary);

            return collateralFromDictionary;
        }

        /// <summary>
        /// Gets the CollateralPaddingSize setting from the AccuAccount database
        /// </summary>
        /// <returns>The CollateralPaddingSize</returns>
        private static int GetCollateralPaddingSize()
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
        /// <param name="record">The <see cref="SourceRecord"/> to check the account number for</param>
        /// <returns>True if the account number exists in the database</returns>
        public static void CheckIfAccountExistsInDatabase(SourceRecord record)
        {
            int accounts = (from l in db.Loans
                            where l.LoanNumber == record.LoanNumber
                            select l).Count();


            if (accounts > 0)
            {
                LogWriter.LogMessage("accountNumber " + record.LoanNumber + " already exists");
                record.IgnoreRecord = true;
            }
        }

        /// <summary>
        /// Clears the list of rules
        /// </summary>
        public static void ClearRulesList()
        {
            Rules.Clear();
        }
    }
}
