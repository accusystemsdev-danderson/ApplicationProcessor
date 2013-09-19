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

    class FieldMapper
    {
        public string XmlMappingFile { get; set; }

        public string CustomerNumberFieldName { get; set; }
        public string TaxIdFieldName { get; set; }
        public string CustomerNameFieldName { get; set; }
        public string BusinessNameFieldName { get; set; }
        public string CustomerFirstNameFieldName { get; set; }
        public string CustomerMiddleNameFieldName { get; set; }
        public string CustomerLastNameFieldName { get; set; }
        public string CustomerTypeCodeFieldName { get; set; }
        public string BankCodeFieldName { get; set; }
        public string EmployeeFieldName { get; set; }
        public string CustomerBranchFieldName { get; set; }
        public string CustomerOfficerCodeFieldName { get; set; }
        public string Address1FieldName { get; set; }
        public string Address2FieldName { get; set; }
        public string CityFieldName { get; set; }
        public string StateFieldName { get; set; }
        public string ZipCodeFieldName { get; set; }
        public string HomePhoneFieldName { get; set; }
        public string WorkPhoneFieldName { get; set; }
        public string MobilePhoneFieldName { get; set; }
        public string FaxFieldName { get; set; }
        public string EmailFieldName { get; set; }
        public string ClassificationCodeFieldName { get; set; }
        public string CustomerStatusFieldName { get; set; }
        public string LoanNumberFieldName { get; set; }
        public string CollateralLoanNumberFieldName { get; set; }
        public string CollateralAddendaFieldName { get; set; }
        public string ParentLoanNumberFieldName { get; set; }
        public string AccountClassFieldName { get; set; }
        public string LoanOfficerCodeFieldName { get; set; }
        public string LoanTypeCodeFieldName { get; set; }
        public string CollateralLoanTypeCodeFieldName { get; set; }
        public string LoanStatusCodeFieldName { get; set; }
        public string LoanClosedFieldName { get; set; }
        public string LoanAmountFieldName { get; set; }
        public string LoanOriginationDateFieldName { get; set; }
        public string LoanDescriptionFieldName { get; set; }
        public string CollateralDescriptionFieldName { get; set; }
        public string BorrowerTypeFieldName { get; set; }
        public string OwningCustomerNumberFieldName { get; set; }
        public string LoanBranchFieldName { get; set; }
        public string CoreClassCodeFieldName { get; set; }
        public string CoreCollCodeFieldName { get; set; }
        public string CoreCollateralCodeFieldName { get; set; }
        public string CorePurposeCodeFieldName { get; set; }
        public string CoreTypeCodeFieldName { get; set; }
        public string CommitmentAmountFieldName { get; set; }
        public string CoreNaicsCodeFieldName { get; set; }
        public string LoanMaturityDateFieldName { get; set; }
        public string LoanClassificationCodeFieldName { get; set; }
        public string ApplicationDateFieldName { get; set; }
        public string CreditAnalysisStatusFieldName { get; set; }
        public string RequestedAmountFieldName { get; set; }
        public string PrimaryCollateralValueFieldName { get; set; }
        public string FICOFieldName { get; set; }
        public string ValuationDateFieldName { get; set; }
        public string InterestRateFieldName { get; set; }
        public string ProbabilityFieldName { get; set; }
        public string EstimatedCloseDateFieldName { get; set; }
        public string AssignedLenderFieldName { get; set; }
        public string AssignedLenderTypeFieldName { get; set; }
        public string AssignedAnalystFieldName { get; set; }
        public string AssignedAnalystTypeFieldName { get; set; }
        public string AssignedLoanProcessorFieldName { get; set; }
        public string AssignedLoanProcessorTypeFieldName { get; set; }
        public string ApplicationLockedFieldName { get; set; }
        public string ApprovalStatusFieldName { get; set; }
        public string OriginatingUserFieldName { get; set; }
        public string AssignedApproverFieldName { get; set; }
        public string AssignedApproverTypeFieldName { get; set; }

        public string PostProcessingFieldFieldName { get; set; } //used for building a list of processed accounts to use in post processing query
        public string Generic1FieldName { get; set; } // generic fields can be used in rule processing but are not exported to xml
        public string Generic2FieldName { get; set; }
        public string Generic3FieldName { get; set; }
        public string Generic4FieldName { get; set; }
        

        public string CustomerNumberMappedField { get; set; }
        public string TaxIdMappedField { get; set; }
        public string CustomerNameMappedField { get; set; }
        public string BusinessNameMappedField { get; set; }
        public string CustomerFirstNameMappedField { get; set; }
        public string CustomerMiddleNameMappedField { get; set; }
        public string CustomerLastNameMappedField { get; set; }
        public string CustomerTypeCodeMappedField { get; set; }
        public string BankCodeMappedField { get; set; }
        public string EmployeeMappedField { get; set; }
        public string CustomerBranchMappedField { get; set; }
        public string CustomerOfficerCodeMappedField { get; set; }
        public string Address1MappedField { get; set; }
        public string Address2MappedField { get; set; }
        public string CityMappedField { get; set; }
        public string StateMappedField { get; set; }
        public string ZipCodeMappedField { get; set; }
        public string HomePhoneMappedField { get; set; }
        public string WorkPhoneMappedField { get; set; }
        public string MobilePhoneMappedField { get; set; }
        public string FaxMappedField { get; set; }
        public string EmailMappedField { get; set; }
        public string ClassificationCodeMappedField { get; set; }
        public string CustomerStatusMappedField { get; set; }
        public string LoanNumberMappedField { get; set; }
        public string CollateralLoanNumberMappedField { get; set; }
        public string CollateralAddendaMappedField { get; set; }
        public string ParentLoanNumberMappedField { get; set; }
        public string AccountClassMappedField { get; set; }
        public string LoanOfficerCodeMappedField { get; set; }
        public string loanTypeCodeMappedField { get; set; }
        public string CollateralLoanTypeCodeMappedField { get; set; }
        public string LoanStatusCodeMappedField { get; set; }
        public string LoanClosedMappedField { get; set; }
        public string LoanAmountMappedField { get; set; }
        public string LoanOriginationDateMappedField { get; set; }
        public string LoanDescriptionMappedField { get; set; }
        public string CollateralDescriptionMappedField { get; set; }
        public string BorrowerTypeMappedField { get; set; }
        public string OwningCustomerNumberMappedField { get; set; }
        public string LoanBranchMappedField { get; set; }
        public string CoreClassCodeMappedField { get; set; }
        public string CoreCollCodeMappedField { get; set; }
        public string CoreCollateralCodeMappedField { get; set; }
        public string CorePurposeCodeMappedField { get; set; }
        public string CoreTypeCodeMappedField { get; set; }
        public string CommitmentAmountMappedField { get; set; }
        public string CoreNaicsCodeMappedField { get; set; }
        public string LoanMaturityDateMappedField { get; set; }
        public string LoanClassificationCodeMappedField { get; set; }
        public string ApplicationDateMappedField { get; set; }
        public string CreditAnalysisStatusMappedField { get; set; }
        public string RequestedAmountMappedField { get; set; }
        public string PrimaryCollateralValueMappedField { get; set; }
        public string FICOMappedField { get; set; }
        public string ValuationDateMappedField { get; set; }
        public string InterestRateMappedField { get; set; }
        public string ProbabilityMappedField { get; set; }
        public string EstimatedCloseDateMappedField { get; set; }
        public string AssignedLenderMappedField { get; set; }
        public string AssignedLenderTypeMappedField { get; set; }
        public string AssignedAnalystMappedField { get; set; }
        public string AssignedAnalystTypeMappedField { get; set; }
        public string AssignedLoanProcessorMappedField { get; set; }
        public string AssignedLoanProcessorTypeMappedField { get; set; }
        public string ApplicationLockedMappedField { get; set; }
        public string ApprovalStatusMappedField { get; set; }
        public string OriginatingUserMappedField { get; set; }
        public string AssignedApproverMappedField { get; set; }
        public string AssignedApproverTypeMappedField { get; set; }

        public string PostProcessingFieldMappedField { get; set; }
        public string Generic1MappedField { get; set; }
        public string Generic2MappedField { get; set; }
        public string Generic3MappedField { get; set; }
        public string Generic4MappedField { get; set; }
        
        /// <summary>
        /// Reads field name mappings from XmlMappingFile
        /// </summary>
        public void ReadFieldMappings()
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
            loanTypeCodeMappedField = Utils.ReadXMLElementValue(xmlMap, LoanTypeCodeFieldName, "");
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
