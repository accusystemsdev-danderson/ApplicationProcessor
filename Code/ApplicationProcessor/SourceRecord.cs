//-----------------------------------------------------------------------------
// <copyright file="Program.cs" company="AccuSystems LLC">
//     Copyright (c) AccuSystems.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------


namespace ApplicationProcessor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Provides information about a source data record
    /// </summary>
    public class SourceRecord
    {
        public string CustomerNumber { get; set; }
        public string TaxId { get; set; }
        public string CustomerName { get; set; }
        public string BusinessName { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerMiddleName { get; set; }
        public string CustomerLastName { get; set; }
        public string CustomerTypeCode { get; set; }
        public string BankCode { get; set; }
        public string Employee { get; set; }
        public string CustomerBranch { get; set; }
        public string CustomerOfficerCode { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string HomePhone { get; set; }
        public string WorkPhone { get; set; }
        public string MobilePhone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string ClassificationCode { get; set; }
        public string CustomerStatus { get; set; }
        public string LoanNumber { get; set; }
        public string CollateralLoanNumber { get; set; }
        public string CollateralAddenda { get; set; }
        public string ParentLoanNumber { get; set; }
        public string AccountClass { get; set; }
        public string LoanOfficerCode { get; set; }
        public string LoanTypeCode { get; set; }
        public string CollateralLoanTypeCode { get; set; }
        public string LoanStatusCode { get; set; }
        public string LoanClosed { get; set; }
        public string LoanAmount { get; set; }
        public string LoanOriginationDate { get; set; }
        public string LoanDescription { get; set; }
        public string CollateralDescription { get; set; }
        public string BorrowerType { get; set; }
        public string OwningCustomerNumber { get; set; }
        public string LoanBranch { get; set; }
        public string CoreClassCode { get; set; }
        public string CoreCollCode { get; set; }
        public string CoreCollateralCode { get; set; }
        public string CorePurposeCode { get; set; }
        public string CoreTypeCode { get; set; }
        public string CommitmentAmount { get; set; }
        public string CoreNaicsCode { get; set; }
        public string LoanMaturityDate { get; set; }
        public string LoanClassificationCode { get; set; }
        public string ApplicationDate { get; set; }
        public string CreditAnalysisStatus { get; set; }
        public string RequestedAmount { get; set; }
        public string PrimaryCollateralValue { get; set; }
        public string FICO { get; set; }
        public string ValuationDate { get; set; }
        public string InterestRate { get; set; }
        public string Probability { get; set; }
        public string EstimatedCloseDate { get; set; }
        public string AssignedLender { get; set; }
        public string AssignedLenderType { get; set; }
        public string AssignedAnalyst { get; set; }
        public string AssignedAnalystType { get; set; }
        public string AssignedLoanProcessor { get; set; }
        public string AssignedLoanProcessorType { get; set; }
        public string ApplicationLocked { get; set; }
        public string ApprovalStatus { get; set; }
        public string OriginatingUser { get; set; }
        public string AssignedApprover { get; set; }
        public string AssignedApproverType { get; set; }

        public string PostProcessingField { get; set; }
        public string Generic1 { get; set; }
        public string Generic2 { get; set; }
        public string Generic3 { get; set; }
        public string Generic4 { get; set; }

        public bool IgnoreRecord { get; set; }

    }
}
