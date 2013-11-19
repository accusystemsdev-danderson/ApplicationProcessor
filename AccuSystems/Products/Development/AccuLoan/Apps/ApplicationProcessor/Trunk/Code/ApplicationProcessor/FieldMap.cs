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
    using System.Threading.Tasks;
    using System.Xml.Linq;

    public static class FieldMap
    {
        public static string XmlMappingFile { get; set; }

        public static string CustomerNumberFieldName { get; set; }
        public static string TaxIdFieldName { get; set; }
        public static string CustomerNameFieldName { get; set; }
        public static string BusinessNameFieldName { get; set; }
        public static string CustomerFirstNameFieldName { get; set; }
        public static string CustomerMiddleNameFieldName { get; set; }
        public static string CustomerLastNameFieldName { get; set; }
        public static string CustomerTypeCodeFieldName { get; set; }
        public static string BankCodeFieldName { get; set; }
        public static string EmployeeFieldName { get; set; }
        public static string CustomerBranchFieldName { get; set; }
        public static string CustomerOfficerCodeFieldName { get; set; }
        public static string Address1FieldName { get; set; }
        public static string Address2FieldName { get; set; }
        public static string CityFieldName { get; set; }
        public static string StateFieldName { get; set; }
        public static string ZipCodeFieldName { get; set; }
        public static string HomePhoneFieldName { get; set; }
        public static string WorkPhoneFieldName { get; set; }
        public static string MobilePhoneFieldName { get; set; }
        public static string FaxFieldName { get; set; }
        public static string EmailFieldName { get; set; }
        public static string ClassificationCodeFieldName { get; set; }
        public static string CustomerStatusFieldName { get; set; }
        public static string LoanNumberFieldName { get; set; }
        public static string CollateralLoanNumberFieldName { get; set; }
        public static string CollateralAddendaFieldName { get; set; }
        public static string ParentLoanNumberFieldName { get; set; }
        public static string AccountClassFieldName { get; set; }
        public static string LoanOfficerCodeFieldName { get; set; }
        public static string LoanTypeCodeFieldName { get; set; }
        public static string CollateralLoanTypeCodeFieldName { get; set; }
        public static string LoanStatusCodeFieldName { get; set; }
        public static string LoanClosedFieldName { get; set; }
        public static string LoanAmountFieldName { get; set; }
        public static string LoanOriginationDateFieldName { get; set; }
        public static string LoanDescriptionFieldName { get; set; }
        public static string CollateralDescriptionFieldName { get; set; }
        public static string BorrowerTypeFieldName { get; set; }
        public static string OwningCustomerNumberFieldName { get; set; }
        public static string LoanBranchFieldName { get; set; }
        public static string CoreClassCodeFieldName { get; set; }
        public static string CoreCollCodeFieldName { get; set; }
        public static string CoreCollateralCodeFieldName { get; set; }
        public static string CorePurposeCodeFieldName { get; set; }
        public static string CoreTypeCodeFieldName { get; set; }
        public static string CommitmentAmountFieldName { get; set; }
        public static string CoreNaicsCodeFieldName { get; set; }
        public static string LoanMaturityDateFieldName { get; set; }
        public static string LoanClassificationCodeFieldName { get; set; }
        public static string ApplicationDateFieldName { get; set; }
        public static string CreditAnalysisStatusFieldName { get; set; }
        public static string RequestedAmountFieldName { get; set; }
        public static string PrimaryCollateralValueFieldName { get; set; }
        public static string FICOFieldName { get; set; }
        public static string ValuationDateFieldName { get; set; }
        public static string InterestRateFieldName { get; set; }
        public static string ProbabilityFieldName { get; set; }
        public static string EstimatedCloseDateFieldName { get; set; }
        public static string AssignedLenderFieldName { get; set; }
        public static string AssignedLenderTypeFieldName { get; set; }
        public static string AssignedAnalystFieldName { get; set; }
        public static string AssignedAnalystTypeFieldName { get; set; }
        public static string AssignedLoanProcessorFieldName { get; set; }
        public static string AssignedLoanProcessorTypeFieldName { get; set; }
        public static string ApplicationLockedFieldName { get; set; }
        public static string ApprovalStatusFieldName { get; set; }
        public static string OriginatingUserFieldName { get; set; }
        public static string AssignedApproverFieldName { get; set; }
        public static string AssignedApproverTypeFieldName { get; set; }

        public static string PostProcessingFieldFieldName { get; set; } //used for building a list of processed accounts to use in post processing query
        public static string Generic1FieldName { get; set; } // generic fields can be used in rule processing but are not exported to xml
        public static string Generic2FieldName { get; set; }
        public static string Generic3FieldName { get; set; }
        public static string Generic4FieldName { get; set; }
        

        public static string CustomerNumberMappedField { get; set; }
        public static string TaxIdMappedField { get; set; }
        public static string CustomerNameMappedField { get; set; }
        public static string BusinessNameMappedField { get; set; }
        public static string CustomerFirstNameMappedField { get; set; }
        public static string CustomerMiddleNameMappedField { get; set; }
        public static string CustomerLastNameMappedField { get; set; }
        public static string CustomerTypeCodeMappedField { get; set; }
        public static string BankCodeMappedField { get; set; }
        public static string EmployeeMappedField { get; set; }
        public static string CustomerBranchMappedField { get; set; }
        public static string CustomerOfficerCodeMappedField { get; set; }
        public static string Address1MappedField { get; set; }
        public static string Address2MappedField { get; set; }
        public static string CityMappedField { get; set; }
        public static string StateMappedField { get; set; }
        public static string ZipCodeMappedField { get; set; }
        public static string HomePhoneMappedField { get; set; }
        public static string WorkPhoneMappedField { get; set; }
        public static string MobilePhoneMappedField { get; set; }
        public static string FaxMappedField { get; set; }
        public static string EmailMappedField { get; set; }
        public static string ClassificationCodeMappedField { get; set; }
        public static string CustomerStatusMappedField { get; set; }
        public static string LoanNumberMappedField { get; set; }
        public static string CollateralLoanNumberMappedField { get; set; }
        public static string CollateralAddendaMappedField { get; set; }
        public static string ParentLoanNumberMappedField { get; set; }
        public static string AccountClassMappedField { get; set; }
        public static string LoanOfficerCodeMappedField { get; set; }
        public static string LoanTypeCodeMappedField { get; set; }
        public static string CollateralLoanTypeCodeMappedField { get; set; }
        public static string LoanStatusCodeMappedField { get; set; }
        public static string LoanClosedMappedField { get; set; }
        public static string LoanAmountMappedField { get; set; }
        public static string LoanOriginationDateMappedField { get; set; }
        public static string LoanDescriptionMappedField { get; set; }
        public static string CollateralDescriptionMappedField { get; set; }
        public static string BorrowerTypeMappedField { get; set; }
        public static string OwningCustomerNumberMappedField { get; set; }
        public static string LoanBranchMappedField { get; set; }
        public static string CoreClassCodeMappedField { get; set; }
        public static string CoreCollCodeMappedField { get; set; }
        public static string CoreCollateralCodeMappedField { get; set; }
        public static string CorePurposeCodeMappedField { get; set; }
        public static string CoreTypeCodeMappedField { get; set; }
        public static string CommitmentAmountMappedField { get; set; }
        public static string CoreNaicsCodeMappedField { get; set; }
        public static string LoanMaturityDateMappedField { get; set; }
        public static string LoanClassificationCodeMappedField { get; set; }
        public static string ApplicationDateMappedField { get; set; }
        public static string CreditAnalysisStatusMappedField { get; set; }
        public static string RequestedAmountMappedField { get; set; }
        public static string PrimaryCollateralValueMappedField { get; set; }
        public static string FICOMappedField { get; set; }
        public static string ValuationDateMappedField { get; set; }
        public static string InterestRateMappedField { get; set; }
        public static string ProbabilityMappedField { get; set; }
        public static string EstimatedCloseDateMappedField { get; set; }
        public static string AssignedLenderMappedField { get; set; }
        public static string AssignedLenderTypeMappedField { get; set; }
        public static string AssignedAnalystMappedField { get; set; }
        public static string AssignedAnalystTypeMappedField { get; set; }
        public static string AssignedLoanProcessorMappedField { get; set; }
        public static string AssignedLoanProcessorTypeMappedField { get; set; }
        public static string ApplicationLockedMappedField { get; set; }
        public static string ApprovalStatusMappedField { get; set; }
        public static string OriginatingUserMappedField { get; set; }
        public static string AssignedApproverMappedField { get; set; }
        public static string AssignedApproverTypeMappedField { get; set; }

        public static string PostProcessingFieldMappedField { get; set; }
        public static string Generic1MappedField { get; set; }
        public static string Generic2MappedField { get; set; }
        public static string Generic3MappedField { get; set; }
        public static string Generic4MappedField { get; set; }
        
        /// <summary>
        /// Reads field name mappings from XmlMappingFile
        /// </summary>
        public static void ReadFieldMappings()
        {
            CustomerNumberFieldName = "customerNumber";
            TaxIdFieldName = "taxId";
            CustomerNameFieldName = "customerName";
            BusinessNameFieldName = "businessName";
            CustomerFirstNameFieldName = "customerFirstName";
            CustomerMiddleNameFieldName = "customerMiddleName";
            CustomerLastNameFieldName = "customerLastName";
            CustomerTypeCodeFieldName = "customerTypeCode";
            BankCodeFieldName = "bankCode";
            EmployeeFieldName = "employee";
            CustomerBranchFieldName = "customerBranch";
            CustomerOfficerCodeFieldName = "customerOfficerCode";
            Address1FieldName = "address1";
            Address2FieldName = "address2";
            CityFieldName = "city";
            StateFieldName = "state";
            ZipCodeFieldName = "zipCode";
            HomePhoneFieldName = "homePhone";
            WorkPhoneFieldName = "workPhone";
            MobilePhoneFieldName = "mobilePhone";
            FaxFieldName = "fax";
            EmailFieldName = "email";
            ClassificationCodeFieldName = "classificationCode";
            CustomerStatusFieldName = "customerStatus";
            LoanNumberFieldName = "loanNumber";
            CollateralLoanNumberFieldName = "collateralLoanNumber";
            CollateralAddendaFieldName = "collateralAddenda";
            ParentLoanNumberFieldName = "parentLoanNumber";
            AccountClassFieldName = "accountClass";
            LoanOfficerCodeFieldName = "loanOfficerCode";
            LoanTypeCodeFieldName = "loanTypeCode";
            CollateralLoanTypeCodeFieldName = "collateralLoanTypeCode";
            LoanStatusCodeFieldName = "loanStatusCode";
            LoanClosedFieldName = "loanClosed";
            LoanAmountFieldName = "loanAmount";
            LoanOriginationDateFieldName = "loanOriginationDate";
            LoanDescriptionFieldName = "loanDescription";
            CollateralDescriptionFieldName = "collateralDescription";
            BorrowerTypeFieldName = "borrowerType";
            OwningCustomerNumberFieldName = "owningCustomerNumber";
            LoanBranchFieldName = "loanBranch";
            CoreClassCodeFieldName = "coreClassCode";
            CoreCollCodeFieldName = "coreCollCode";
            CoreCollateralCodeFieldName = "coreCollateralCode";
            CorePurposeCodeFieldName = "corePurposeCode";
            CoreTypeCodeFieldName = "coreTypeCode";
            CommitmentAmountFieldName = "commitmentAmount";
            CoreNaicsCodeFieldName = "coreNaicsCode";
            LoanMaturityDateFieldName = "loanMaturityDate";
            LoanClassificationCodeFieldName = "loanClassificationCode";
            ApplicationDateFieldName = "applicationDate";
            CreditAnalysisStatusFieldName = "creditAnalysisStatus";
            RequestedAmountFieldName = "requestedAmount";
            PrimaryCollateralValueFieldName = "primaryCollateralValue";
            FICOFieldName = "FICO";
            ValuationDateFieldName = "valuationDate";
            InterestRateFieldName = "interestRate";
            ProbabilityFieldName = "probability";
            EstimatedCloseDateFieldName = "estimatedCloseDate";
            AssignedLenderFieldName = "assignedLender";
            AssignedLenderTypeFieldName = "assignedLenderType";
            AssignedAnalystFieldName = "assignedAnalyst";
            AssignedAnalystTypeFieldName = "assignedAnalystType";
            AssignedLoanProcessorFieldName = "assignedLoanProcessor";
            AssignedLoanProcessorTypeFieldName = "assignedLoanProcessorType";
            ApplicationLockedFieldName = "applicationLocked";
            ApprovalStatusFieldName = "approvalStatus";
            OriginatingUserFieldName = "originatingUser";
            AssignedApproverFieldName = "assignedApprover";
            AssignedApproverTypeFieldName = "assignedApproverType";

            PostProcessingFieldFieldName = "postProcessingField";
            Generic1FieldName = "generic1";
            Generic2FieldName = "generic2";
            Generic3FieldName = "generic3";
            Generic4FieldName = "generic4";
            
            XElement xmlMap = XElement.Load(XmlMappingFile);

            CustomerNumberMappedField = Utils.ReadXMLElementValue(xmlMap, CustomerNumberFieldName, "");
            TaxIdMappedField = Utils.ReadXMLElementValue(xmlMap, TaxIdFieldName, "");
            CustomerNameMappedField = Utils.ReadXMLElementValue(xmlMap, CustomerNameFieldName, "");
            BusinessNameMappedField = Utils.ReadXMLElementValue(xmlMap, BusinessNameFieldName, "");
            CustomerFirstNameMappedField = Utils.ReadXMLElementValue(xmlMap, CustomerFirstNameFieldName, "");
            CustomerMiddleNameMappedField = Utils.ReadXMLElementValue(xmlMap, CustomerMiddleNameFieldName, "");
            CustomerLastNameMappedField = Utils.ReadXMLElementValue(xmlMap, CustomerLastNameFieldName, "");
            CustomerTypeCodeMappedField = Utils.ReadXMLElementValue(xmlMap, CustomerTypeCodeFieldName, "");
            BankCodeMappedField = Utils.ReadXMLElementValue(xmlMap, BankCodeFieldName, "");
            EmployeeMappedField = Utils.ReadXMLElementValue(xmlMap, EmployeeFieldName, "");
            CustomerBranchMappedField = Utils.ReadXMLElementValue(xmlMap, CustomerBranchFieldName, "");
            CustomerOfficerCodeMappedField = Utils.ReadXMLElementValue(xmlMap, CustomerOfficerCodeFieldName, "");
            Address1MappedField = Utils.ReadXMLElementValue(xmlMap, Address1FieldName, "");
            Address2MappedField = Utils.ReadXMLElementValue(xmlMap, Address2FieldName, "");
            CityMappedField = Utils.ReadXMLElementValue(xmlMap, CityFieldName, "");
            StateMappedField = Utils.ReadXMLElementValue(xmlMap, StateFieldName, "");
            ZipCodeMappedField = Utils.ReadXMLElementValue(xmlMap, ZipCodeFieldName, "");
            HomePhoneMappedField = Utils.ReadXMLElementValue(xmlMap, HomePhoneFieldName, "");
            WorkPhoneMappedField = Utils.ReadXMLElementValue(xmlMap, WorkPhoneFieldName, "");
            MobilePhoneMappedField = Utils.ReadXMLElementValue(xmlMap, MobilePhoneFieldName, "");
            FaxMappedField = Utils.ReadXMLElementValue(xmlMap, FaxFieldName, "");
            EmailMappedField = Utils.ReadXMLElementValue(xmlMap, EmailFieldName, "");
            ClassificationCodeMappedField = Utils.ReadXMLElementValue(xmlMap, ClassificationCodeFieldName, "");
            CustomerStatusMappedField = Utils.ReadXMLElementValue(xmlMap, CustomerStatusFieldName, "");
            LoanNumberMappedField = Utils.ReadXMLElementValue(xmlMap, LoanNumberFieldName, "");
            CollateralLoanNumberMappedField = Utils.ReadXMLElementValue(xmlMap, CollateralLoanNumberFieldName, "");
            CollateralAddendaMappedField = Utils.ReadXMLElementValue(xmlMap, CollateralAddendaFieldName, "");
            ParentLoanNumberMappedField = Utils.ReadXMLElementValue(xmlMap, ParentLoanNumberFieldName, "");
            AccountClassMappedField = Utils.ReadXMLElementValue(xmlMap, AccountClassFieldName, "");
            LoanOfficerCodeMappedField = Utils.ReadXMLElementValue(xmlMap, LoanOfficerCodeFieldName, "");
            LoanTypeCodeMappedField = Utils.ReadXMLElementValue(xmlMap, LoanTypeCodeFieldName, "");
            CollateralLoanTypeCodeMappedField = Utils.ReadXMLElementValue(xmlMap, CollateralLoanTypeCodeFieldName, "");
            LoanStatusCodeMappedField = Utils.ReadXMLElementValue(xmlMap, LoanStatusCodeFieldName, "");
            LoanClosedMappedField = Utils.ReadXMLElementValue(xmlMap, LoanClosedFieldName, "");
            LoanAmountMappedField = Utils.ReadXMLElementValue(xmlMap, LoanAmountFieldName, "");
            LoanOriginationDateMappedField = Utils.ReadXMLElementValue(xmlMap, LoanOriginationDateFieldName, "");
            LoanDescriptionMappedField = Utils.ReadXMLElementValue(xmlMap, LoanDescriptionFieldName, "");
            CollateralDescriptionMappedField = Utils.ReadXMLElementValue(xmlMap, CollateralDescriptionFieldName, "");
            BorrowerTypeMappedField = Utils.ReadXMLElementValue(xmlMap, BorrowerTypeFieldName, "");
            OwningCustomerNumberMappedField = Utils.ReadXMLElementValue(xmlMap, OwningCustomerNumberFieldName, "");
            LoanBranchMappedField = Utils.ReadXMLElementValue(xmlMap, LoanBranchFieldName, "");
            CoreClassCodeMappedField = Utils.ReadXMLElementValue(xmlMap, CoreClassCodeFieldName, "");
            CoreCollCodeMappedField = Utils.ReadXMLElementValue(xmlMap, CoreCollCodeFieldName, "");
            CoreCollateralCodeMappedField = Utils.ReadXMLElementValue(xmlMap, CoreCollateralCodeFieldName, "");
            CorePurposeCodeMappedField = Utils.ReadXMLElementValue(xmlMap, CorePurposeCodeFieldName, "");
            CoreTypeCodeMappedField = Utils.ReadXMLElementValue(xmlMap, CoreTypeCodeFieldName, "");
            CommitmentAmountMappedField = Utils.ReadXMLElementValue(xmlMap, CommitmentAmountFieldName, "");
            CoreNaicsCodeMappedField = Utils.ReadXMLElementValue(xmlMap, CoreNaicsCodeFieldName, "");
            LoanMaturityDateMappedField = Utils.ReadXMLElementValue(xmlMap, LoanMaturityDateFieldName, "");
            LoanClassificationCodeMappedField = Utils.ReadXMLElementValue(xmlMap, LoanClassificationCodeFieldName, "");
            ApplicationDateMappedField = Utils.ReadXMLElementValue(xmlMap, ApplicationDateFieldName, "");
            CreditAnalysisStatusMappedField = Utils.ReadXMLElementValue(xmlMap, CreditAnalysisStatusFieldName, "");
            RequestedAmountMappedField = Utils.ReadXMLElementValue(xmlMap, RequestedAmountFieldName, "");
            PrimaryCollateralValueMappedField = Utils.ReadXMLElementValue(xmlMap, PrimaryCollateralValueFieldName, "");
            FICOMappedField = Utils.ReadXMLElementValue(xmlMap, FICOFieldName, "");
            ValuationDateMappedField = Utils.ReadXMLElementValue(xmlMap, ValuationDateFieldName, "");
            InterestRateMappedField = Utils.ReadXMLElementValue(xmlMap, InterestRateFieldName, "");
            ProbabilityMappedField = Utils.ReadXMLElementValue(xmlMap, ProbabilityFieldName, "");
            EstimatedCloseDateMappedField = Utils.ReadXMLElementValue(xmlMap, EstimatedCloseDateFieldName, "");
            AssignedLenderMappedField = Utils.ReadXMLElementValue(xmlMap, AssignedLenderFieldName, "");
            AssignedLenderTypeMappedField = Utils.ReadXMLElementValue(xmlMap, AssignedLenderTypeFieldName, "");
            AssignedAnalystMappedField = Utils.ReadXMLElementValue(xmlMap, AssignedAnalystFieldName, "");
            AssignedAnalystTypeMappedField = Utils.ReadXMLElementValue(xmlMap, AssignedAnalystTypeFieldName, "");
            AssignedLoanProcessorMappedField = Utils.ReadXMLElementValue(xmlMap, AssignedLoanProcessorFieldName, "");
            AssignedLoanProcessorTypeMappedField = Utils.ReadXMLElementValue(xmlMap, AssignedLoanProcessorTypeFieldName, "");
            ApplicationLockedMappedField = Utils.ReadXMLElementValue(xmlMap, ApplicationLockedFieldName, "");
            ApprovalStatusMappedField = Utils.ReadXMLElementValue(xmlMap, ApprovalStatusFieldName, "");
            OriginatingUserMappedField = Utils.ReadXMLElementValue(xmlMap, OriginatingUserFieldName, "");
            AssignedApproverMappedField = Utils.ReadXMLElementValue(xmlMap, AssignedApproverFieldName, "");
            AssignedApproverTypeMappedField = Utils.ReadXMLElementValue(xmlMap, AssignedApproverTypeFieldName, "");

            PostProcessingFieldMappedField = Utils.ReadXMLElementValue(xmlMap, PostProcessingFieldFieldName, LoanNumberMappedField);
            Generic1MappedField = Utils.ReadXMLElementValue(xmlMap, Generic1FieldName, "");
            Generic2MappedField = Utils.ReadXMLElementValue(xmlMap, Generic2FieldName, "");
            Generic3MappedField = Utils.ReadXMLElementValue(xmlMap, Generic3FieldName, "");
            Generic4MappedField = Utils.ReadXMLElementValue(xmlMap, Generic4FieldName, "");
            
        }
    
    }
}
