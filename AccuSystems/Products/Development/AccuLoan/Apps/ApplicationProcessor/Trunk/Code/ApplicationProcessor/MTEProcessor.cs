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
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;

    /// <summary>
    /// Processes MTE information for customers in a DataTable
    /// </summary>
    class MTEProcessor
    {
        private DataContext db { get; set; }

        public Configuration Config { get; set; }
        public DataTable TableToProcess { get; set; }
        public LogWriter LogFile { get; set; }
        public FieldMapper FieldMap { get; set; } 
        
        /// <summary>
        /// Updates CustomerName or CustomerNumber if MTE records indicate an update should occur
        /// </summary>
        /// <returns>True upon successful processing</returns>
        public bool ProcessMTEs()
        {
            db = new DataContext();
            bool success = false;
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
                string customerNumber = row[FieldMap.CustomerNumberFieldName].ToString();
                string customerName = row[FieldMap.CustomerNameFieldName].ToString();
                string customerTaxID = row[FieldMap.TaxIdFieldName].ToString();

                //Check to see if any customers exist that share the same taxID, but under a different name
                int DupTaxIDs = (from M in db.Customers
                                 where M.TaxId == customerTaxID && M.CustomerName.ToUpper() != customerName.ToUpper()
                                 select M).Count();

                if (DupTaxIDs > 0)
                {
                    //get current entries in MTE_List
                    var MTEs = from M in db.MTELists
                               where M.TaxId == customerTaxID
                               select M;
                    
                    //if count = 0 then this taxID has not been an MTE before.  The current customer record in the customer table, as well
                    //as the record being processed must be added to the table.
                    if (MTEs.Count() == 0)
                    {
                        AddPrimaryRecordToMTEList(customerTaxID);
                        AddCurrentRecordToMTEList(customerName, customerNumber, customerTaxID);

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
                                        string primaryName = (from N in db.MTELists
                                                              where N.CustomerNumber == customerNumber && N.Appendage == "*"
                                                              select N.CustomerName).FirstOrDefault().ToString();
                                        row[FieldMap.CustomerNameFieldName] = primaryName;
                                    }
                                    
                                    //appendages of any other single character indicate an additional customer, append the character to the customer number
                                    else if (Regex.IsMatch(mteRecord.Appendage, "[0-9a-zA-Z]"))
                                    {
                                        row[FieldMap.CustomerNumberFieldName] = customerNumber + "-" + mteRecord.Appendage;
                                    }

                                    //appendages with '-' followed by another single character indicate an additional name for a secondary customer, replace the current name and customer number
                                    else if (Regex.IsMatch(mteRecord.Appendage, "-[0-9a-zA-Z"))
                                    {
                                        string primaryName = (from N in db.MTELists
                                                              where N.CustomerNumber == customerNumber && N.Appendage == mteRecord.Appendage.Replace("-", "")
                                                              select N.CustomerName).FirstOrDefault().ToString();
                                        row[FieldMap.CustomerNameFieldName] = primaryName;
                                        row[FieldMap.CustomerNumberFieldName] = customerNumber + "-" + mteRecord.Appendage;
                                    }

                                }
                            }
                        }

                        if (!currentRecordExits)
                            AddCurrentRecordToMTEList(customerName, customerNumber, customerTaxID);
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

        /// <summary>
        /// Creates an MTE record for the primary holder of a taxId
        /// </summary>
        /// <param name="taxID">The taxId to lookup</param>
        private void AddPrimaryRecordToMTEList(string taxID)
        {
            var primaryRecord = (from c in db.Customers
                                where c.TaxId == taxID
                                 select c).FirstOrDefault();
                AddCurrentRecordToMTEList(primaryRecord.CustomerName, primaryRecord.CustomerNumber, primaryRecord.TaxId);
        }
        
        /// <summary>
        /// creates an unresolved MTE record for a customer
        /// </summary>
        /// <param name="customerName">The Customer Name for the MTE</param>
        /// <param name="customerNumber">The Customer Number for the MTE</param>
        /// <param name="taxID">The TaxId for the MTE</param>
        private void AddCurrentRecordToMTEList(string customerName, string customerNumber, string taxID)
        {
            {
                MTEList newMTERecord = new MTEList();
                newMTERecord.MTEId = Guid.NewGuid();
                newMTERecord.CustomerName = customerName;
                newMTERecord.CustomerNumber = customerNumber;
                newMTERecord.TaxId = taxID;
                db.MTELists.Add(newMTERecord);
            }
        }

    }
}
