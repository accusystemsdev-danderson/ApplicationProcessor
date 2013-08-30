using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Xml.Linq;
using System.Reflection;
using System.Data.SqlClient;


namespace ApplicationProcessor
{
    class RulesProcessor
    {
        public DataTable TableToProcess { get; set; }
        public string RulesFile { get; set; }
        public LogWriter logFile { get; set; }
        public FieldMapper FieldMap { get; set; }
        public Configuration Config { get; set; }

        public void ProcessRules()
        {
            DataTable rulesToProcess = loadDataTableFromRulesFile();
            List<int> rowsToRemove = new List<int>();

            //foreach (DataRow row in TableToProcess.Rows)
            for (int i=0; i < TableToProcess.Rows.Count; i++)
            {
                DataRow row = TableToProcess.Rows[i];
                foreach (DataRow rule in rulesToProcess.Rows)
                {
                    bool removeRecordFromRule = false;
                    if (ruleMatches(row, rule))
                    {
                        processAction(row, rule, out removeRecordFromRule);
                    }


                    if (removeRecordFromRule)
                    {
                        rowsToRemove.Add(i);
                    }

                }

                bool removeRecordFromRequiredFields = false;
                verifyRequiredFields(row, out removeRecordFromRequiredFields);

                if (removeRecordFromRequiredFields)
                {
                    if (!rowsToRemove.Contains(i))
                        rowsToRemove.Add(i);
                }

            }
            for (int i = rowsToRemove.Count - 1; i >= 0; i--)
            {
                TableToProcess.Rows[rowsToRemove[i]].Delete();
            }
            TableToProcess.AcceptChanges();
        
        }

        private DataTable loadDataTableFromRulesFile()
        {
            XElement xml = XElement.Load(RulesFile);

            DataTable dataTable = new DataTable();

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

            return dataTable;


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
                    if (fieldToCheck == value)
                        match = true;
                    break;
                case "!=":
                    if (fieldToCheck != value)
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

        private void processAction(DataRow row, DataRow rule, out bool removeRecord)
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
                case "Skip Record":
                    logFile.LogMessage("Skipping Record");
                    //logFile.LogMessage(string.Format("Skipping record - Customer Number: {0} - Account Number: {1}.  {2} {3}",
                    //        row["CustomerNumber"], row["ApplicationNumber"], field, action));
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
                default:
                    break;
                
            }

        }

        private void verifyRequiredFields(DataRow row, out bool removeRecord)
        {
            removeRecord = false;
            StringBuilder logMessage = new StringBuilder();
            
            logMessage.Append("Missing Fields: ");
            if (row[FieldMap.customerNumberFieldName].ToString() == "") logMessage.Append(FieldMap.customerNumberFieldName + " ");
            if (row[FieldMap.customerNameFieldName].ToString() == "") logMessage.Append(FieldMap.customerNameFieldName + " ");
            if (row[FieldMap.customerBranchFieldName].ToString() == "") logMessage.Append(FieldMap.customerBranchFieldName + " ");
            if (row[FieldMap.loanNumberFieldName].ToString() == "") logMessage.Append(FieldMap.loanNumberFieldName + " ");
            if (row[FieldMap.accountClassFieldName].ToString() == "") logMessage.Append(FieldMap.accountClassFieldName + " ");
            // if (row[FieldMap.borrowerTypeFieldName].ToString() == "") logMessage.Append(FieldMap.borrowerTypeFieldName + " ");
            // borrowertype cannot be checked for a value because a primary borrower type is defined as blank.
            if (row[FieldMap.loanTypeCodeFieldName].ToString() == "") logMessage.Append(FieldMap.loanTypeCodeFieldName + " ");
            if (row[FieldMap.originatingUserFieldName].ToString() == "") logMessage.Append(FieldMap.originatingUserFieldName + " ");

            if (logMessage.ToString() != "Missing Fields: ")
            {
                removeRecord = true;
                if (row[FieldMap.loanNumberFieldName].ToString() != "")
                    logMessage.Append("for account number " + row[FieldMap.loanNumberFieldName].ToString() + " ");
                logFile.LogMessage("Skipping Record: " + logMessage);
            }

        }

        private string lookupFromDB(string lookupValue, string lookupTable, string lookupField, string returnField)
        {
            string dbConnectionString = setupDBConnectionString();
            string sqlQuery = "Select " + returnField + " from [" + lookupTable + "] where " + lookupField + " = '" + lookupValue + "'";
            SqlConnection connection = new SqlConnection(dbConnectionString);
            connection.Open();
            SqlDataAdapter dadapter = new SqlDataAdapter();
            dadapter.SelectCommand = new SqlCommand(sqlQuery, connection);
            DataTable dataTable = new DataTable();
            dadapter.Fill(dataTable);
            connection.Close();
            
            string result = dataTable.Rows[0][returnField].ToString();
            return result.ToString();
        }

        private string setupDBConnectionString()
        {
            StringBuilder connectionString = new StringBuilder();

            XElement xml = XElement.Load(Config.PathToDBXML);
            foreach (XElement setting in xml.Elements("DBSource"))
            {
                if (setting.Element("application").Value.ToUpper() == "ACCULOAN")
                {
                    connectionString.Append("data source=" + setting.Element("server").Value + ";");
                    connectionString.Append("initial catalog=" + setting.Element("database").Value + ";");
                    connectionString.Append("user id=" + setting.Element("user_ID").Value + ";");
                    connectionString.Append("password=" + setting.Element("password").Value + ";");
                }

            }
            return connectionString.ToString();

        }
    }
}
