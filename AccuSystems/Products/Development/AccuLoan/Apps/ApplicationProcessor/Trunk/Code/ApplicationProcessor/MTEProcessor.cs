//-----------------------------------------------------------------------------
// <copyright file="MTEProcessor.cs" company="AccuSystems LLC">
//     Copyright (c) AccuSystems.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace ApplicationProcessor
{
    using AccuAccount.Data;
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    

    /// <summary>
    /// Processes MTE information for customers in a DataTable
    /// </summary>
    public static class MteProcessor
    {
        private static readonly DataContext DB = new DataContext();

        /// <summary>
        /// Updates CustomerName or CustomerNumber if MTE records indicate an update should occur
        /// </summary>
        /// <returns>True upon successful processing</returns>
        public static bool ProcessMte(SourceRecord record)
        {
            try
            {
                if (!DB.Database.Exists())
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

            bool duplicateTaxIDs =
                DB.Customers.Any(c => c.TaxId.Equals(record.TaxId, StringComparison.OrdinalIgnoreCase) &&
                                        !c.CustomerName.Equals(record.CustomerName, StringComparison.OrdinalIgnoreCase));

            bool existingMtes = DB.MTELists.Any(p => p.TaxId.Equals(record.TaxId, StringComparison.OrdinalIgnoreCase));

            if (duplicateTaxIDs || existingMtes)
            {
                if (!UpdateRecordFromMteList(record))
                {
                    //todo: default processing for unresolved mtes.

                    record.IgnoreRecord = true;
                    LogWriter.LogMessage(string.Format("Ignoring record - MTE needs to be resolved.  Application Number: {0} TaxId: {1} Customer Name: {2}",
                                                       record.LoanNumber,
                                                       record.TaxId,
                                                       record.CustomerName));
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Reads MTE records from MTE_List and updates record with correct CustomerName and CustomerNumber
        /// </summary>
        /// <param name="record">The source record to process</param>
        /// <returns>True if the record was updated from an mte record.  False if the mte is unresolved</returns>
        private static bool UpdateRecordFromMteList(SourceRecord record)
        {
            var mtes = DB.MTELists.Where(m => m.TaxId.Equals(record.TaxId, StringComparison.OrdinalIgnoreCase));

            //if count = 0 then this taxID has not been an MTE before.  The current customer record in the customer table, as well
            //as the record being processed must be added to the table.
            if (!mtes.Any())
            {
                AddPrimaryRecordToMteList(record.TaxId);
                AddNewMteListRecord(record.CustomerName, record.CustomerNumber, record.TaxId, string.Empty);
                return false;
            }

            var matchedMteRecord = mtes.SingleOrDefault(m => m.CustomerName.Equals(record.CustomerName, StringComparison.OrdinalIgnoreCase));

            if (matchedMteRecord == null)
            {
                AddNewMteListRecord(record.CustomerName, record.CustomerNumber, record.TaxId, string.Empty);
                return false;
            }

            if (matchedMteRecord.Appendage == "*")
            {
                return true;
            }

            if (matchedMteRecord.Appendage == "-")
            {
                //appendages of '-' indicate an additional name for a single customer, replace the current name with the primary MTE record
                var primaryMte =
                    DB.MTELists.SingleOrDefault(m => m.TaxId == record.TaxId &&
                                                    m.Appendage == "*");

                if (primaryMte != null)
                {
                    record.CustomerName = primaryMte.CustomerName;
                    return true;
                }
                
                return false;
            }

            if (Regex.IsMatch(matchedMteRecord.Appendage, "^[0-9|a-z|A-Z]"))
            {
                //appendages of any other single character indicate an additional customer, append the character to the customer number
                record.CustomerNumber = string.Format("{0}-{1}",
                                                      record.CustomerNumber,
                                                      matchedMteRecord.Appendage);

                return true;
            }

            if (Regex.IsMatch(matchedMteRecord.Appendage, "-[0-9|a-z|A-Z]"))
            {
                //appendages with '-' followed by another single character indicate an additional name for a secondary customer, replace the current name and customer number
                var primaryMte =
                    DB.MTELists.SingleOrDefault(m => m.TaxId == record.TaxId &&
                                                     m.Appendage == matchedMteRecord.Appendage.Replace("-", ""));

                if (primaryMte != null)
                {
                    record.CustomerName   = primaryMte.CustomerName;
                    record.CustomerNumber = string.Format("{0}-{1}", record.CustomerNumber, primaryMte.Appendage);
                    return true;
                }
                return false;
            }

            if (matchedMteRecord.Appendage == "~")
            {
                //appendages of '~' are a second name line.  Replace the current name with the primary record, and assign the current name as name line 2
                var primaryMte =
                    DB.MTELists.SingleOrDefault(m => m.TaxId == record.TaxId &&
                                                     m.Appendage == "*");

                if (primaryMte != null)
                {
                    record.BusinessName = record.CustomerName;
                    record.CustomerName = primaryMte.CustomerName;
                    return true;
                }
                return false;
            }

            if (Regex.IsMatch(matchedMteRecord.Appendage, "~[0-9a-zA-Z]"))
            {
                //apendages of '~' followed by another single character are a second name line for an additional customer, replace the current name with the primary record and assign
                //the current name as name line 2
                var primaryMte =
                    DB.MTELists.SingleOrDefault(m => m.TaxId == record.TaxId &&
                                                     m.Appendage == matchedMteRecord.Appendage.Replace("~", ""));

                if (primaryMte != null)
                {
                    record.BusinessName = record.CustomerName;
                    record.CustomerName = primaryMte.CustomerName;
                    record.CustomerNumber = string.Format("{0}-{1}", record.CustomerNumber, primaryMte.Appendage);
                    return true;
                }
                return false;
            }

            return false;
        }

        /// <summary>
        /// Creates an MTE record for the primary holder of a taxId
        /// </summary>
        /// <param name="taxID">The taxId to lookup</param>
        private static void AddPrimaryRecordToMteList(string taxID)
        {
            var primaryRecord = DB.Customers.First(c => c.TaxId == taxID);

            AddNewMteListRecord(primaryRecord.CustomerName, primaryRecord.CustomerNumber, primaryRecord.TaxId, string.Empty);
        }

        /// <summary>
        /// creates an unresolved MTE record for a customer
        /// </summary>
        /// <param name="customerName">The Customer Name for the MTE</param>
        /// <param name="customerNumber">The Customer Number for the MTE</param>
        /// <param name="taxID">The TaxId for the MTE</param>
        /// <param name="appendage">The appendage for the MTE record</param>
        private static void AddNewMteListRecord(string customerName, string customerNumber, string taxID, string appendage)
        {

            var newMteRecord = new MTEList
                               {
                                   MTEId = Guid.NewGuid(),
                                   CustomerName = customerName,
                                   CustomerNumber = customerNumber,
                                   TaxId = taxID,
                                   Appendage = appendage
                               };

            DB.MTELists.Add(newMteRecord);

            try
            {
                DB.SaveChanges();
            }
            catch (Exception e)
            {
                LogWriter.LogMessage(e.ToString());
                LogWriter.LogMessage();
                LogWriter.LogMessage("Unable to save to MTE table");
            }
        }
    }
}
