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

        public string customerNameFieldName { get; set; }
        public string customerNumberFieldName { get; set; }
        public string taxIdFieldName { get; set; }
        public string customerBranchFieldName { get; set; }
        public string customerOfficerCodeFieldName { get; set; }
        public string loanNumberFieldName { get; set; }
        public string collateralLoanNumberFieldName { get; set; }
        public string collateralAddendaFieldName { get; set; }
        public string parentLoanNumberFieldName { get; set; }
        public string accountClassFieldName { get; set; }
        public string loanTypeCodeFieldName { get; set; }
        public string collateralLoanTypeCodeFieldName { get; set; }
        public string loanStatusCodeFieldName { get; set; }
        public string loanAmountFieldName { get; set; }
        public string loanOriginationDateFieldName { get; set; }
        public string loanDescriptionFieldName { get; set; }
        public string collateralDescriptionFieldName { get; set; }
        public string borrowerTypeFieldName { get; set; }
        public string owningCustomerNumberFieldName { get; set; }
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
        public string generic1FieldName { get; set; }
        public string generic2FieldName { get; set; }
        public string generic3FieldName { get; set; }
        public string generic4FieldName { get; set; }
        

        public string customerNameMappedField { get; set; }
        public string customerNumberMappedField { get; set; }
        public string taxIdMappedField { get; set; }
        public string customerBranchMappedField { get; set; }
        public string customerOfficerCodeMappedField { get; set; }
        public string loanNumberMappedField { get; set; }
        public string collateralLoanNumberMappedField { get; set; }
        public string collateralAddendaMappedField { get; set; }
        public string parentLoanNumberMappedField { get; set; }
        public string accountClassMappedField { get; set; }
        public string loanTypeCodeMappedField { get; set; }
        public string collateralLoanTypeCodeMappedField { get; set; }
        public string loanStatusCodeMappedField { get; set; }
        public string loanAmountMappedField { get; set; }
        public string loanOriginationDateMappedField { get; set; }
        public string loanDescriptionMappedField { get; set; }
        public string collateralDescriptionMappedField { get; set; }
        public string borrowerTypeMappedField { get; set; }
        public string owningCustomerNumberMappedField { get; set; }
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
        public string generic1MappedField { get; set; }
        public string generic2MappedField { get; set; }
        public string generic3MappedField { get; set; }
        public string generic4MappedField { get; set; }
        

        public void ReadFieldMappings()
        {
            customerNameFieldName = "customerName";
            customerNumberFieldName = "customerNumber";
            taxIdFieldName = "taxId";
            customerBranchFieldName = "customerBranch";
            customerOfficerCodeFieldName = "customerOfficerCode";
            loanNumberFieldName = "loanNumber";
            collateralLoanNumberFieldName = "collateralLoanNumber";
            collateralAddendaFieldName = "collateralAddenda";
            parentLoanNumberFieldName = "parentLoanNumber";
            accountClassFieldName = "accountClass";
            loanTypeCodeFieldName = "loanTypeCode";
            collateralLoanTypeCodeFieldName = "collateralLoanTypeCode";
            loanStatusCodeFieldName = "loanStatusCode";
            loanAmountFieldName = "loanAmount";
            loanOriginationDateFieldName = "loanOriginationDate";
            loanDescriptionFieldName = "loanDescription";
            collateralDescriptionFieldName = "collateralDescription";
            borrowerTypeFieldName = "borrowerType";
            owningCustomerNumberFieldName = "owningCustomerNumber";
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
            generic1FieldName = "generic1";
            generic2FieldName = "generic2";
            generic3FieldName = "generic3";
            generic4FieldName = "generic4";
            

            XElement xmlMap = XElement.Load(xmlMappingFile);

            customerNameMappedField = Utils.ReadXMLElementValue(xmlMap, customerNameFieldName, "");
            customerNumberMappedField = Utils.ReadXMLElementValue(xmlMap, customerNumberFieldName, "");
            taxIdMappedField = Utils.ReadXMLElementValue(xmlMap, taxIdFieldName, "");
            customerBranchMappedField = Utils.ReadXMLElementValue(xmlMap, customerBranchFieldName, "");
            customerOfficerCodeMappedField = Utils.ReadXMLElementValue(xmlMap, customerOfficerCodeFieldName, "");
            loanNumberMappedField = Utils.ReadXMLElementValue(xmlMap, loanNumberFieldName, "");
            collateralLoanNumberMappedField = Utils.ReadXMLElementValue(xmlMap, collateralLoanNumberFieldName, "");
            collateralAddendaMappedField = Utils.ReadXMLElementValue(xmlMap, collateralAddendaFieldName, "");
            parentLoanNumberMappedField = Utils.ReadXMLElementValue(xmlMap, parentLoanNumberFieldName, "");
            accountClassMappedField = Utils.ReadXMLElementValue(xmlMap, accountClassFieldName, "");
            loanTypeCodeMappedField = Utils.ReadXMLElementValue(xmlMap, loanTypeCodeFieldName, "");
            collateralLoanTypeCodeMappedField = Utils.ReadXMLElementValue(xmlMap, collateralLoanTypeCodeFieldName, "");
            loanStatusCodeMappedField = Utils.ReadXMLElementValue(xmlMap, loanStatusCodeFieldName, "");
            loanAmountMappedField = Utils.ReadXMLElementValue(xmlMap, loanAmountFieldName, "");
            loanOriginationDateMappedField = Utils.ReadXMLElementValue(xmlMap, loanOriginationDateFieldName, "");
            loanDescriptionMappedField = Utils.ReadXMLElementValue(xmlMap, loanDescriptionFieldName, "");
            collateralDescriptionMappedField = Utils.ReadXMLElementValue(xmlMap, collateralDescriptionFieldName, "");
            borrowerTypeMappedField = Utils.ReadXMLElementValue(xmlMap, borrowerTypeFieldName, "");
            owningCustomerNumberMappedField = Utils.ReadXMLElementValue(xmlMap, owningCustomerNumberFieldName, "");
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
            generic1MappedField = Utils.ReadXMLElementValue(xmlMap, generic1FieldName, "");
            generic2MappedField = Utils.ReadXMLElementValue(xmlMap, generic2FieldName, "");
            generic3MappedField = Utils.ReadXMLElementValue(xmlMap, generic3FieldName, "");
            generic4MappedField = Utils.ReadXMLElementValue(xmlMap, generic4FieldName, "");
            
        }
    
    }
}
