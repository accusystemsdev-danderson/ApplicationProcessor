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
    public static class MTEProcessor
    {
        private static DataContext db = new DataContext();

        /// <summary>
        /// Updates CustomerName or CustomerNumber if MTE records indicate an update should occur
        /// </summary>
        /// <returns>True upon successful processing</returns>
        public static bool ProcessMTEs(SourceRecord record)
        {
            try
            {
                if (!db.Database.Exists())
                {
                    LogWriter.LogMessage("Unable to connect to database");
                    return false;
                }
            }
            catch (Exception e)
            {
                LogWriter.LogMessage(e.ToString());
                LogWriter.LogMessage();
                LogWriter.LogMessage("Unable to connect to database");
                return false;
            }
            
            int DupTaxIDs = (from M in db.Customers
                             where M.TaxId.Equals(record.TaxId, StringComparison.OrdinalIgnoreCase) && 
                                !M.CustomerName.Equals(record.CustomerName, StringComparison.OrdinalIgnoreCase)
                             select M).Count();

            if (DupTaxIDs > 0)
            {
                UpdateRecordFromMteList(record);
            }
            return true;
        }
        
        /// <summary>
        /// Reads MTE records from MTE_List and updates record with correct CustomerName and CustomerNumber
        /// </summary>
        /// <param name="record">The source record to process</param>
        private static void UpdateRecordFromMteList(SourceRecord record)
        {
            var MTEs = from M in db.MTELists
                       where M.TaxId.Equals(record.TaxId, StringComparison.OrdinalIgnoreCase)
                       select M;
                    
            //if count = 0 then this taxID has not been an MTE before.  The current customer record in the customer table, as well
            //as the record being processed must be added to the table.
            if (MTEs.Count() == 0)
            {
                AddPrimaryRecordToMTEList(record.TaxId);
                AddCurrentRecordToMTEList(record.CustomerName, record.CustomerNumber, record.TaxId);
            }

            else
            {
                bool currentRecordExits = false;

                foreach (var mteRecord in MTEs)
                {

                    if (mteRecord.CustomerName.Equals(record.CustomerName, StringComparison.OrdinalIgnoreCase))
                    {
                        currentRecordExits = true;

                        if (mteRecord.Appendage != "*" && mteRecord.Appendage != null)
                        {
                            //appendages of '-' indicate an additional name for a single customer, replace the current name with the primary MTE record
                            if (mteRecord.Appendage == "-")
                            {
                                string primaryName = (from N in db.MTELists
                                                      where N.CustomerNumber == record.CustomerNumber && N.Appendage == "*"
                                                      select N.CustomerName).FirstOrDefault().ToString();
                                record.CustomerName = primaryName;
                            }

                            //appendages of any other single character indicate an additional customer, append the character to the customer number
                            else if (Regex.IsMatch(mteRecord.Appendage, "[0-9a-zA-Z]"))
                            {
                                record.CustomerNumber = record.CustomerNumber + "-" + mteRecord.Appendage;
                            }

                            //appendages with '-' followed by another single character indicate an additional name for a secondary customer, replace the current name and customer number
                            else if (Regex.IsMatch(mteRecord.Appendage, "-[0-9a-zA-Z]"))
                            {
                                string primaryName = (from N in db.MTELists
                                                      where N.CustomerNumber == record.CustomerNumber && N.Appendage == mteRecord.Appendage.Replace("-", "")
                                                      select N.CustomerName).FirstOrDefault().ToString();
                                record.CustomerName = primaryName;
                                record.CustomerNumber = record.CustomerNumber + "-" + mteRecord.Appendage;
                            }
                        }
                    }
                }

                if (!currentRecordExits)
                {
                    AddCurrentRecordToMTEList(record.CustomerName, record.CustomerNumber, record.TaxId);
                }
            }

                
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                LogWriter.LogMessage(e.ToString());
                LogWriter.LogMessage();
                LogWriter.LogMessage("Unable to save to MTE table");

            }

        }

        /// <summary>
        /// Creates an MTE record for the primary holder of a taxId
        /// </summary>
        /// <param name="taxID">The taxId to lookup</param>
        private static void AddPrimaryRecordToMTEList(string taxID)
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
        private static void AddCurrentRecordToMTEList(string customerName, string customerNumber, string taxID)
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
