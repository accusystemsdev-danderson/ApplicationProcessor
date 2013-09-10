using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Xml;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace ApplicationProcessor
{
    class MTEProcessor
    {
        public Configuration Config { get; set; }
        public DataTable TableToProcess { get; set; }
        public LogWriter LogFile { get; set; }
        public FieldMapper FieldMap { get; set; } 
        private AcculoanDBEntities db { get; set; }

        public bool processMTEs()
        {
            bool success = false;
            db = new AcculoanDBEntities(Config.dbConnectionString);
            try
            {
                if (!db.Database.Exists())
                {
                    LogFile.LogMessage("Unable to connect to database");
                    return success;
                }
            }
            catch (Exception e)
            {
                LogFile.LogMessage(e.ToString());
                LogFile.LogMessage();
                LogFile.LogMessage("Unable to connect to database");
                return success;
            }
            foreach (DataRow row in TableToProcess.Rows)
            {
                string customerNumber = row[FieldMap.customerNumberFieldName].ToString();
                string customerName = row[FieldMap.customerNameFieldName].ToString();
                string customerTaxID = row[FieldMap.taxIdFieldName].ToString();

                //Check to see if any customers exist that share the same taxID, but under a different name
                int DupTaxIDs = (from M in db.customers
                                 where M.TaxId == customerTaxID && M.customerName.ToUpper() != customerName.ToUpper()
                                 select M).Count();

                if (DupTaxIDs > 0)
                {
                    //get current entries in MTE_List
                    var MTEs = from M in db.MTE_List
                               where M.TaxID == customerTaxID
                               select M;
                    
                    //if count = 0 then this taxID has not been an MTE before.  The current customer record in the customer table, as well
                    //as the record being processed must be added to the table.
                    if (MTEs.Count() == 0)
                    {
                        addPrimaryRecordToMTEList(customerTaxID);
                        addCurrentRecordToMTEList(customerName, customerNumber, customerTaxID);

                    }

                    else
                    {
                        bool currentRecordExits = false;

                        foreach (var mteRecord in MTEs)
                        {
                            //look through the MTE records to see if an entry exists for the current customer name.
                            //if an MTE record already exists for the current customer name, assign the appropriate appendage.
                            //if no MTE record exists, create one.
                            
                            if (mteRecord.CustomerName.ToUpper() == customerName.ToUpper())
                            {
                                currentRecordExits = true;

                                //appendages of '*' indate the primary name, no changes made.  Null appendages have not been resolved.
                                if (mteRecord.Appendage != "*" && mteRecord.Appendage != null)
                                {
                                    //appendages of '-' indicate an additional name for a single customer, replace the current name with the primary MTE record
                                    if (mteRecord.Appendage == "-")
                                    {
                                        string primaryName = (from N in db.MTE_List
                                                              where N.CustomerNumber == customerNumber && N.Appendage == "*"
                                                              select N.CustomerName).FirstOrDefault().ToString();
                                        row[FieldMap.customerNameFieldName] = primaryName;
                                    }
                                    
                                    //appendages of any other single character indicate an additional customer, append the character to the customer number
                                    else if (Regex.IsMatch(mteRecord.Appendage, "[0-9a-zA-Z]"))
                                    {
                                        row[FieldMap.customerNumberFieldName] = customerNumber + "-" + mteRecord.Appendage;
                                    }

                                    //appendages with '-' followed by another single character indicate an additional name for a secondary customer, replace the current name and customer number
                                    else if (Regex.IsMatch(mteRecord.Appendage, "-[0-9a-zA-Z"))
                                    {
                                        string primaryName = (from N in db.MTE_List
                                                              where N.CustomerNumber == customerNumber && N.Appendage == mteRecord.Appendage.Replace("-", "")
                                                              select N.CustomerName).FirstOrDefault().ToString();
                                        row[FieldMap.customerNameFieldName] = primaryName;
                                        row[FieldMap.customerNumberFieldName] = customerNumber + "-" + mteRecord.Appendage;
                                    }

                                }
                            }
                        }

                        if (!currentRecordExits)
                            addCurrentRecordToMTEList(customerName, customerNumber, customerTaxID);
                    }
                }
            }
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                LogFile.LogMessage(e.ToString());
                LogFile.LogMessage();
                LogFile.LogMessage("Unable to save to MTE table");
                return success;
            }

            success = true;
            return success;
        }

        private void addPrimaryRecordToMTEList(string taxID)
        {
            var primaryRecord = (from c in db.customers
                                where c.TaxId == taxID
                                 select c).FirstOrDefault();
                addCurrentRecordToMTEList(primaryRecord.customerName, primaryRecord.customerNumber, primaryRecord.TaxId);
        }
        
        private void addCurrentRecordToMTEList(string customerName, string customerNumber, string taxID)
        {
            {
                MTE_List newMTERecord = new MTE_List();
                newMTERecord.MTEID = Guid.NewGuid();
                newMTERecord.CustomerName = customerName;
                newMTERecord.CustomerNumber = customerNumber;
                newMTERecord.TaxID = taxID;
                db.MTE_List.Add(newMTERecord);
            }
        }

    }
}
