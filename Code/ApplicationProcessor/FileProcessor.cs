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

namespace ApplicationProcessor
{
    class FileProcessor
    {
        public DataSources DataSource { get; set; }
        public Configuration Config { get; set; }

        public DataTable FillDataTable()
        {
            DataTable dataTable = new DataTable();
            switch (DataSource)
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
                    break;
                default:
                    break;
            }


            return dataTable;
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

        public void setOwningCustomer (DataTable sourceTable, LogWriter log, FieldMapper fieldMap)
        {
            List<int> rowsToRemove = new List<int>();

            //foreach (DataRow row in sourceTable.Rows)
            for (int i = 0; i < sourceTable.Rows.Count; i++)
            {
                DataRow row = sourceTable.Rows[i];
                
                DataRow[] primaryRows = sourceTable.Select("loanNumber = '" + row[fieldMap.loanNumberFieldName].ToString() + "' AND borrowerType = ''");
                if (primaryRows.Count() != 1)
                {
                    rowsToRemove.Add(i);
                    log.LogMessage(string.Format("Wrong number of primary relationships for account {0} - primary relationships: {1}",
                        row[fieldMap.loanNumberFieldName].ToString(), primaryRows.Count()));
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
    }



    public enum DataSources
    { 
        delimited,
        SQL,
        XML
    }
}
