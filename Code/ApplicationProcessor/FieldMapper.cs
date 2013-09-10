using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ApplicationProcessor
{
    class FieldMapper
    {
        public string xmlMappingFile { get; set; }

        public string customerNumberFieldName { get; set; }
        public string taxIdFieldName { get; set; }
        public string customerNameFieldName { get; set; }
        public string businessNameFieldName { get; set; }
        public string customerFirstNameFieldName { get; set; }
        public string customerMiddleNameFieldName { get; set; }
        public string customerLastNameFieldName { get; set; }
        public string customerTypeCodeFieldName { get; set; }
        public string bankCodeFieldName { get; set; }
        public string employeeFieldName { get; set; }
        public string customerBranchFieldName { get; set; }
        public string customerOfficerCodeFieldName { get; set; }
        public string address1FieldName { get; set; }
        public string address2FieldName { get; set; }
        public string cityFieldName { get; set; }
        public string stateFieldName { get; set; }
        public string zipCodeFieldName { get; set; }
        public string homePhoneFieldName { get; set; }
        public string workPhoneFieldName { get; set; }
        public string mobilePhoneFieldName { get; set; }
        public string faxFieldName { get; set; }
        public string emailFieldName { get; set; }
        public string classificationCodeFieldName { get; set; }
        public string customerStatusFieldName { get; set; }
        public string loanNumberFieldName { get; set; }
        public string collateralLoanNumberFieldName { get; set; }
        public string collateralAddendaFieldName { get; set; }
        public string parentLoanNumberFieldName { get; set; }
        public string accountClassFieldName { get; set; }
        public string loanOfficerCodeFieldName { get; set; }
        public string loanTypeCodeFieldName { get; set; }
        public string collateralLoanTypeCodeFieldName { get; set; }
        public string loanStatusCodeFieldName { get; set; }
        public string loanClosedFieldName { get; set; }
        public string loanAmountFieldName { get; set; }
        public string loanOriginationDateFieldName { get; set; }
        public string loanDescriptionFieldName { get; set; }
        public string collateralDescriptionFieldName { get; set; }
        public string borrowerTypeFieldName { get; set; }
        public string owningCustomerNumberFieldName { get; set; }
        public string loanBranchFieldName { get; set; }
        public string coreClassCodeFieldName { get; set; }
        public string coreCollCodeFieldName { get; set; }
        public string coreCollateralCodeFieldName { get; set; }
        public string corePurposeCodeFieldName { get; set; }
        public string coreTypeCodeFieldName { get; set; }
        public string commitmentAmountFieldName { get; set; }
        public string coreNaicsCodeFieldName { get; set; }
        public string loanMaturityDateFieldName { get; set; }
        public string loanClassificationCodeFieldName { get; set; }
        public string applicationDateFieldName { get; set; }
        public string creditAnalysisStatusFieldName { get; set; }
        public string requestedAmountFieldName { get; set; }
        public string primaryCollateralValueFieldName { get; set; }
        public string FICOFieldName { get; set; }
        public string valuationDateFieldName { get; set; }
        public string interestRateFieldName { get; set; }
        public string probabilityFieldName { get; set; }
        public string estimatedCloseDateFieldName { get; set; }
        public string assignedLenderFieldName { get; set; }
        public string assignedLenderTypeFieldName { get; set; }
        public string assignedAnalystFieldName { get; set; }
        public string assignedAnalystTypeFieldName { get; set; }
        public string assignedLoanProcessorFieldName { get; set; }
        public string assignedLoanProcessorTypeFieldName { get; set; }
        public string applicationLockedFieldName { get; set; }
        public string approvalStatusFieldName { get; set; }
        public string originatingUserFieldName { get; set; }
        public string assignedApproverFieldName { get; set; }
        public string assignedApproverTypeFieldName { get; set; }

        public string postProcessingFieldFieldName { get; set; } //used for building a list of processed accounts to use in post processing query
        public string generic1FieldName { get; set; } // generic fields can be used in rule processing but are not exported to xml
        public string generic2FieldName { get; set; }
        public string generic3FieldName { get; set; }
        public string generic4FieldName { get; set; }
        

        public string customerNumberMappedField { get; set; }
        public string taxIdMappedField { get; set; }
        public string customerNameMappedField { get; set; }
        public string businessNameMappedField { get; set; }
        public string customerFirstNameMappedField { get; set; }
        public string customerMiddleNameMappedField { get; set; }
        public string customerLastNameMappedField { get; set; }
        public string customerTypeCodeMappedField { get; set; }
        public string bankCodeMappedField { get; set; }
        public string employeeMappedField { get; set; }
        public string customerBranchMappedField { get; set; }
        public string customerOfficerCodeMappedField { get; set; }
        public string address1MappedField { get; set; }
        public string address2MappedField { get; set; }
        public string cityMappedField { get; set; }
        public string stateMappedField { get; set; }
        public string zipCodeMappedField { get; set; }
        public string homePhoneMappedField { get; set; }
        public string workPhoneMappedField { get; set; }
        public string mobilePhoneMappedField { get; set; }
        public string faxMappedField { get; set; }
        public string emailMappedField { get; set; }
        public string classificationCodeMappedField { get; set; }
        public string customerStatusMappedField { get; set; }
        public string loanNumberMappedField { get; set; }
        public string collateralLoanNumberMappedField { get; set; }
        public string collateralAddendaMappedField { get; set; }
        public string parentLoanNumberMappedField { get; set; }
        public string accountClassMappedField { get; set; }
        public string loanOfficerCodeMappedField { get; set; }
        public string loanTypeCodeMappedField { get; set; }
        public string collateralLoanTypeCodeMappedField { get; set; }
        public string loanStatusCodeMappedField { get; set; }
        public string loanClosedMappedField { get; set; }
        public string loanAmountMappedField { get; set; }
        public string loanOriginationDateMappedField { get; set; }
        public string loanDescriptionMappedField { get; set; }
        public string collateralDescriptionMappedField { get; set; }
        public string borrowerTypeMappedField { get; set; }
        public string owningCustomerNumberMappedField { get; set; }
        public string loanBranchMappedField { get; set; }
        public string coreClassCodeMappedField { get; set; }
        public string coreCollCodeMappedField { get; set; }
        public string coreCollateralCodeMappedField { get; set; }
        public string corePurposeCodeMappedField { get; set; }
        public string coreTypeCodeMappedField { get; set; }
        public string commitmentAmountMappedField { get; set; }
        public string coreNaicsCodeMappedField { get; set; }
        public string loanMaturityDateMappedField { get; set; }
        public string loanClassificationCodeMappedField { get; set; }
        public string applicationDateMappedField { get; set; }
        public string creditAnalysisStatusMappedField { get; set; }
        public string requestedAmountMappedField { get; set; }
        public string primaryCollateralValueMappedField { get; set; }
        public string FICOMappedField { get; set; }
        public string valuationDateMappedField { get; set; }
        public string interestRateMappedField { get; set; }
        public string probabilityMappedField { get; set; }
        public string estimatedCloseDateMappedField { get; set; }
        public string assignedLenderMappedField { get; set; }
        public string assignedLenderTypeMappedField { get; set; }
        public string assignedAnalystMappedField { get; set; }
        public string assignedAnalystTypeMappedField { get; set; }
        public string assignedLoanProcessorMappedField { get; set; }
        public string assignedLoanProcessorTypeMappedField { get; set; }
        public string applicationLockedMappedField { get; set; }
        public string approvalStatusMappedField { get; set; }
        public string originatingUserMappedField { get; set; }
        public string assignedApproverMappedField { get; set; }
        public string assignedApproverTypeMappedField { get; set; }

        public string postProcessingFieldMappedField { get; set; }
        public string generic1MappedField { get; set; }
        public string generic2MappedField { get; set; }
        public string generic3MappedField { get; set; }
        public string generic4MappedField { get; set; }
        

        public void ReadFieldMappings()
        {
            customerNumberFieldName = "customerNumber";
            taxIdFieldName = "taxId";
            customerNameFieldName = "customerName";
            businessNameFieldName = "businessName";
            customerFirstNameFieldName = "customerFirstName";
            customerMiddleNameFieldName = "customerMiddleName";
            customerLastNameFieldName = "customerLastName";
            customerTypeCodeFieldName = "customerTypeCode";
            bankCodeFieldName = "bankCode";
            employeeFieldName = "employee";
            customerBranchFieldName = "customerBranch";
            customerOfficerCodeFieldName = "customerOfficerCode";
            address1FieldName = "address1";
            address2FieldName = "address2";
            cityFieldName = "city";
            stateFieldName = "state";
            zipCodeFieldName = "zipCode";
            homePhoneFieldName = "homePhone";
            workPhoneFieldName = "workPhone";
            mobilePhoneFieldName = "mobilePhone";
            faxFieldName = "fax";
            emailFieldName = "email";
            classificationCodeFieldName = "classificationCode";
            customerStatusFieldName = "customerStatus";
            loanNumberFieldName = "loanNumber";
            collateralLoanNumberFieldName = "collateralLoanNumber";
            collateralAddendaFieldName = "collateralAddenda";
            parentLoanNumberFieldName = "parentLoanNumber";
            accountClassFieldName = "accountClass";
            loanOfficerCodeFieldName = "loanOfficerCode";
            loanTypeCodeFieldName = "loanTypeCode";
            collateralLoanTypeCodeFieldName = "collateralLoanTypeCode";
            loanStatusCodeFieldName = "loanStatusCode";
            loanClosedFieldName = "loanClosed";
            loanAmountFieldName = "loanAmount";
            loanOriginationDateFieldName = "loanOriginationDate";
            loanDescriptionFieldName = "loanDescription";
            collateralDescriptionFieldName = "collateralDescription";
            borrowerTypeFieldName = "borrowerType";
            owningCustomerNumberFieldName = "owningCustomerNumber";
            loanBranchFieldName = "loanBranch";
            coreClassCodeFieldName = "coreClassCode";
            coreCollCodeFieldName = "coreCollCode";
            coreCollateralCodeFieldName = "coreCollateralCode";
            corePurposeCodeFieldName = "corePurposeCode";
            coreTypeCodeFieldName = "coreTypeCode";
            commitmentAmountFieldName = "commitmentAmount";
            coreNaicsCodeFieldName = "coreNaicsCode";
            loanMaturityDateFieldName = "loanMaturityDate";
            loanClassificationCodeFieldName = "loanClassificationCode";
            applicationDateFieldName = "applicationDate";
            creditAnalysisStatusFieldName = "creditAnalysisStatus";
            requestedAmountFieldName = "requestedAmount";
            primaryCollateralValueFieldName = "primaryCollateralValue";
            FICOFieldName = "FICO";
            valuationDateFieldName = "valuationDate";
            interestRateFieldName = "interestRate";
            probabilityFieldName = "probability";
            estimatedCloseDateFieldName = "estimatedCloseDate";
            assignedLenderFieldName = "assignedLender";
            assignedLenderTypeFieldName = "assignedLenderType";
            assignedAnalystFieldName = "assignedAnalyst";
            assignedAnalystTypeFieldName = "assignedAnalystType";
            assignedLoanProcessorFieldName = "assignedLoanProcessor";
            assignedLoanProcessorTypeFieldName = "assignedLoanProcessorType";
            applicationLockedFieldName = "applicationLocked";
            approvalStatusFieldName = "approvalStatus";
            originatingUserFieldName = "originatingUser";
            assignedApproverFieldName = "assignedApprover";
            assignedApproverTypeFieldName = "assignedApproverType";

            postProcessingFieldFieldName = "postProcessingField";
            generic1FieldName = "generic1";
            generic2FieldName = "generic2";
            generic3FieldName = "generic3";
            generic4FieldName = "generic4";
            
            XElement xmlMap = XElement.Load(xmlMappingFile);

            customerNumberMappedField = Utils.ReadXMLElementValue(xmlMap, customerNumberFieldName, "");
            taxIdMappedField = Utils.ReadXMLElementValue(xmlMap, taxIdFieldName, "");
            customerNameMappedField = Utils.ReadXMLElementValue(xmlMap, customerNameFieldName, "");
            businessNameMappedField = Utils.ReadXMLElementValue(xmlMap, businessNameFieldName, "");
            customerFirstNameMappedField = Utils.ReadXMLElementValue(xmlMap, customerFirstNameFieldName, "");
            customerMiddleNameMappedField = Utils.ReadXMLElementValue(xmlMap, customerMiddleNameFieldName, "");
            customerLastNameMappedField = Utils.ReadXMLElementValue(xmlMap, customerLastNameFieldName, "");
            customerTypeCodeMappedField = Utils.ReadXMLElementValue(xmlMap, customerTypeCodeFieldName, "");
            bankCodeMappedField = Utils.ReadXMLElementValue(xmlMap, bankCodeFieldName, "");
            employeeMappedField = Utils.ReadXMLElementValue(xmlMap, employeeFieldName, "");
            customerBranchMappedField = Utils.ReadXMLElementValue(xmlMap, customerBranchFieldName, "");
            customerOfficerCodeMappedField = Utils.ReadXMLElementValue(xmlMap, customerOfficerCodeFieldName, "");
            address1MappedField = Utils.ReadXMLElementValue(xmlMap, address1FieldName, "");
            address2MappedField = Utils.ReadXMLElementValue(xmlMap, address2FieldName, "");
            cityMappedField = Utils.ReadXMLElementValue(xmlMap, cityFieldName, "");
            stateMappedField = Utils.ReadXMLElementValue(xmlMap, stateFieldName, "");
            zipCodeMappedField = Utils.ReadXMLElementValue(xmlMap, zipCodeFieldName, "");
            homePhoneMappedField = Utils.ReadXMLElementValue(xmlMap, homePhoneFieldName, "");
            workPhoneMappedField = Utils.ReadXMLElementValue(xmlMap, workPhoneFieldName, "");
            mobilePhoneMappedField = Utils.ReadXMLElementValue(xmlMap, mobilePhoneFieldName, "");
            faxMappedField = Utils.ReadXMLElementValue(xmlMap, faxFieldName, "");
            emailMappedField = Utils.ReadXMLElementValue(xmlMap, emailFieldName, "");
            classificationCodeMappedField = Utils.ReadXMLElementValue(xmlMap, classificationCodeFieldName, "");
            customerStatusMappedField = Utils.ReadXMLElementValue(xmlMap, customerStatusFieldName, "");
            loanNumberMappedField = Utils.ReadXMLElementValue(xmlMap, loanNumberFieldName, "");
            collateralLoanNumberMappedField = Utils.ReadXMLElementValue(xmlMap, collateralLoanNumberFieldName, "");
            collateralAddendaMappedField = Utils.ReadXMLElementValue(xmlMap, collateralAddendaFieldName, "");
            parentLoanNumberMappedField = Utils.ReadXMLElementValue(xmlMap, parentLoanNumberFieldName, "");
            accountClassMappedField = Utils.ReadXMLElementValue(xmlMap, accountClassFieldName, "");
            loanOfficerCodeMappedField = Utils.ReadXMLElementValue(xmlMap, loanOfficerCodeFieldName, "");
            loanTypeCodeMappedField = Utils.ReadXMLElementValue(xmlMap, loanTypeCodeFieldName, "");
            collateralLoanTypeCodeMappedField = Utils.ReadXMLElementValue(xmlMap, collateralLoanTypeCodeFieldName, "");
            loanStatusCodeMappedField = Utils.ReadXMLElementValue(xmlMap, loanStatusCodeFieldName, "");
            loanClosedMappedField = Utils.ReadXMLElementValue(xmlMap, loanClosedFieldName, "");
            loanAmountMappedField = Utils.ReadXMLElementValue(xmlMap, loanAmountFieldName, "");
            loanOriginationDateMappedField = Utils.ReadXMLElementValue(xmlMap, loanOriginationDateFieldName, "");
            loanDescriptionMappedField = Utils.ReadXMLElementValue(xmlMap, loanDescriptionFieldName, "");
            collateralDescriptionMappedField = Utils.ReadXMLElementValue(xmlMap, collateralDescriptionFieldName, "");
            borrowerTypeMappedField = Utils.ReadXMLElementValue(xmlMap, borrowerTypeFieldName, "");
            owningCustomerNumberMappedField = Utils.ReadXMLElementValue(xmlMap, owningCustomerNumberFieldName, "");
            loanBranchMappedField = Utils.ReadXMLElementValue(xmlMap, loanBranchFieldName, "");
            coreClassCodeMappedField = Utils.ReadXMLElementValue(xmlMap, coreClassCodeFieldName, "");
            coreCollCodeMappedField = Utils.ReadXMLElementValue(xmlMap, coreCollCodeFieldName, "");
            coreCollateralCodeMappedField = Utils.ReadXMLElementValue(xmlMap, coreCollateralCodeFieldName, "");
            corePurposeCodeMappedField = Utils.ReadXMLElementValue(xmlMap, corePurposeCodeFieldName, "");
            coreTypeCodeMappedField = Utils.ReadXMLElementValue(xmlMap, coreTypeCodeFieldName, "");
            commitmentAmountMappedField = Utils.ReadXMLElementValue(xmlMap, commitmentAmountFieldName, "");
            coreNaicsCodeMappedField = Utils.ReadXMLElementValue(xmlMap, coreNaicsCodeFieldName, "");
            loanMaturityDateMappedField = Utils.ReadXMLElementValue(xmlMap, loanMaturityDateFieldName, "");
            loanClassificationCodeMappedField = Utils.ReadXMLElementValue(xmlMap, loanClassificationCodeFieldName, "");
            applicationDateMappedField = Utils.ReadXMLElementValue(xmlMap, applicationDateFieldName, "");
            creditAnalysisStatusMappedField = Utils.ReadXMLElementValue(xmlMap, creditAnalysisStatusFieldName, "");
            requestedAmountMappedField = Utils.ReadXMLElementValue(xmlMap, requestedAmountFieldName, "");
            primaryCollateralValueMappedField = Utils.ReadXMLElementValue(xmlMap, primaryCollateralValueFieldName, "");
            FICOMappedField = Utils.ReadXMLElementValue(xmlMap, FICOFieldName, "");
            valuationDateMappedField = Utils.ReadXMLElementValue(xmlMap, valuationDateFieldName, "");
            interestRateMappedField = Utils.ReadXMLElementValue(xmlMap, interestRateFieldName, "");
            probabilityMappedField = Utils.ReadXMLElementValue(xmlMap, probabilityFieldName, "");
            estimatedCloseDateMappedField = Utils.ReadXMLElementValue(xmlMap, estimatedCloseDateFieldName, "");
            assignedLenderMappedField = Utils.ReadXMLElementValue(xmlMap, assignedLenderFieldName, "");
            assignedLenderTypeMappedField = Utils.ReadXMLElementValue(xmlMap, assignedLenderTypeFieldName, "");
            assignedAnalystMappedField = Utils.ReadXMLElementValue(xmlMap, assignedAnalystFieldName, "");
            assignedAnalystTypeMappedField = Utils.ReadXMLElementValue(xmlMap, assignedAnalystTypeFieldName, "");
            assignedLoanProcessorMappedField = Utils.ReadXMLElementValue(xmlMap, assignedLoanProcessorFieldName, "");
            assignedLoanProcessorTypeMappedField = Utils.ReadXMLElementValue(xmlMap, assignedLoanProcessorTypeFieldName, "");
            applicationLockedMappedField = Utils.ReadXMLElementValue(xmlMap, applicationLockedFieldName, "");
            approvalStatusMappedField = Utils.ReadXMLElementValue(xmlMap, approvalStatusFieldName, "");
            originatingUserMappedField = Utils.ReadXMLElementValue(xmlMap, originatingUserFieldName, "");
            assignedApproverMappedField = Utils.ReadXMLElementValue(xmlMap, assignedApproverFieldName, "");
            assignedApproverTypeMappedField = Utils.ReadXMLElementValue(xmlMap, assignedApproverTypeFieldName, "");

            postProcessingFieldMappedField = Utils.ReadXMLElementValue(xmlMap, postProcessingFieldFieldName, loanNumberMappedField);
            generic1MappedField = Utils.ReadXMLElementValue(xmlMap, generic1FieldName, "");
            generic2MappedField = Utils.ReadXMLElementValue(xmlMap, generic2FieldName, "");
            generic3MappedField = Utils.ReadXMLElementValue(xmlMap, generic3FieldName, "");
            generic4MappedField = Utils.ReadXMLElementValue(xmlMap, generic4FieldName, "");
            
        }
    
    }
}
