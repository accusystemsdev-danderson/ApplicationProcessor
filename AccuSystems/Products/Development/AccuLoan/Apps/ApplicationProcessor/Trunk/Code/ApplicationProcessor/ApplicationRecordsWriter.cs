//-----------------------------------------------------------------------------
// <copyright file="ApplicationRecordsWriter.cs" company="AccuSystems LLC">
//     Copyright (c) AccuSystems.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace ApplicationProcessor
{
    using AccuAccount.Data;
    using System;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Validation;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;

    /// <summary>
    /// Inserts an initial application, approval, and history record for applications in an importer formated XML file.  Does not update applications.
    /// </summary>
    class ApplicationRecordsWriter
    {

        /// <summary>
        /// Inserts initial application information into the AccuAccount database from importer XML file
        /// </summary>
        /// <param name="xmlFile">Standard importer xml file to process</param>
        public void InsertLoanApplicationRecords(string xmlFile)
        {
            XDocument document = XDocument.Load(xmlFile);

            foreach (XElement loanXml in document.Descendants("loan"))
            {
                if (loanXml.Element("borrowerType").Value == "")
                {
                    foreach (XElement applicationXml in loanXml.Descendants("application"))
                    {
                        LoanApplication loanApplication = GetApplicationRecord(applicationXml);
                        XElement approvalXml = applicationXml.Descendants("approval").SingleOrDefault();
                        Approval applicationApproval = GetApplicationApproval(approvalXml);
                        loanApplication.ApprovalId = applicationApproval.ApprovalId;

                        if (ValidateApplicationFields(loanApplication, applicationApproval))
                        {
                            LoanApplicationHistory applicationHistory = new LoanApplicationHistory()
                            {
                                LoanApplicationHistoryId = Guid.NewGuid(),
                                LoanApplicationId = loanApplication.LoanApplicationId,
                                ChangedByUserId = applicationApproval.OriginatingUserId,
                                DateModified = DateTime.Now,
                                ActionTaken = "Loan Application created."
                            };
                            InsertApplicationIntoDatabase(loanApplication, applicationApproval, applicationHistory);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Processes an application XML block and creates an application object
        /// </summary>
        /// <param name="applicationXml">The XML block for an application</param>
        /// <returns>The processed application object</returns>
        private LoanApplication GetApplicationRecord(XElement applicationXml)
        {
            LoanApplication application = new LoanApplication();

            application.LoanApplicationId = Guid.NewGuid();
            application.ApplicationNumber = applicationXml.Element("applicationNumber").Value;
            application.LoanId = GetLoanId(application.ApplicationNumber);

            DateTime applicationDate;
            bool parsed = DateTime.TryParse(applicationXml.Element("applicationDate").Value,
                out applicationDate);
            if (parsed)
            {
                application.ApplicationDate = applicationDate;
            }

            application.CreditAnalysisStatusId = GetCreditAnalysisStatusId(applicationXml.Element("creditAnalysisStatus").Value);

            decimal requestedAmount;
            parsed = decimal.TryParse(applicationXml.Element("requestedAmount").Value,
                                           out requestedAmount);
            if (parsed)
            {
                application.RequestedAmount = requestedAmount;
            }

            decimal primaryCollateralValue;
            parsed = decimal.TryParse(applicationXml.Element("primaryCollateralValue").Value,
                out primaryCollateralValue);
            if (parsed)
            {
                application.PrimaryCollateralValue = primaryCollateralValue;
            }

            application.FICO = applicationXml.Element("FICO").Value;

            DateTime valuationDate;
            parsed = DateTime.TryParse(applicationXml.Element("valuationDate").Value,
                out valuationDate);
            if (parsed)
            {
                application.ValuationDate = valuationDate;
            }

            application.InterestRate = applicationXml.Element("interestRate").Value;

            decimal probability;
            parsed = decimal.TryParse(applicationXml.Element("probability").Value,
                out probability);
            if (parsed)
            {
                application.Probability = probability;
            }

            DateTime estimatedCloseDate;
            parsed = DateTime.TryParse(applicationXml.Element("estimatedCloseDate").Value,
                out estimatedCloseDate);
            if (parsed)
            {
                application.EstimatedCloseDate = estimatedCloseDate;
            }

            application.AssignedLenderType = applicationXml.Element("assignedLender").Attribute("type").Value;
            if (application.AssignedLenderType != "")
            {
                application.AssignedLenderId = GetUserOrGroupId(applicationXml.Element("assignedLender").Value,
                    application.AssignedLenderType);
            }

            application.AssignedAnalystType = applicationXml.Element("assignedAnalyst").Attribute("type").Value;
            if (application.AssignedAnalystType != "")
            {
                application.AssignedLenderId = GetUserOrGroupId(applicationXml.Element("assignedAnalyst").Value,
                    application.AssignedAnalystType);
            }

            application.AssignedLoanProcessorType = applicationXml.Element("assignedLoanProcessor").Attribute("type").Value;
            if (application.AssignedLoanProcessorType != "")
            {
                application.AssignedLoanProcessorId = GetUserOrGroupId(applicationXml.Element("assignedLoanProcessor").Value,
                    application.AssignedLoanProcessorType);
            }

            return application;
        }

        /// <summary>
        /// Processes an approval XML block and creates an approval object
        /// </summary>
        /// <param name="approvalXml">The XML block for an approval</param>
        /// <returns>The processed approval object</returns>
        private Approval GetApplicationApproval(XElement approvalXml)
        {
            Approval approval = new Approval();
            approval.ApprovalId = Guid.NewGuid();
            approval.ApprovalStatusId = GetApprovalStatusID(approvalXml.Element("approvalStatus").Value);

            Guid? originatingUserID = GetUserOrGroupId(approvalXml.Element("originatingUser").Value,
                "user");
            if (originatingUserID != null)
            {
                approval.OriginatingUserId = (Guid)originatingUserID;
            }

            approval.AssignedApproverType = approvalXml.Element("assignedApprover").Attribute("type").Value;
            if (approval.AssignedApproverType != "")
            {
                approval.AssignedApproverId = GetUserOrGroupId(approvalXml.Element("assignedApprover").Value,
                    approval.AssignedApproverType);
            }

            return approval;
        }

        /// <summary>
        /// Validates required fields for an application, and approval objects
        /// </summary>
        /// <param name="application">The application to validate</param>
        /// <param name="approval">The approval to validate</param>
        /// <returns>True if all required fields are validated, False if any fields are missing</returns>
        private bool ValidateApplicationFields(LoanApplication application, Approval approval)
        {
            StringBuilder missingFields = new StringBuilder();
            if (application.LoanApplicationId == Guid.Empty) 
            {
                missingFields.Append(" LoanApplicationID");
            }

            if (application.LoanId == Guid.Empty)
            {
                missingFields.Append(" LoanId");
            }

            if (application.ApplicationNumber == "")
            {
                missingFields.Append(" ApplicationNumber");
            }

            if (application.ApprovalId == Guid.Empty || application.ApprovalId == null)
            { 
                missingFields.Append(" ApprovalId");
            }

            if (approval.ApprovalStatusId == Guid.Empty)
            {
                missingFields.Append(" ApprovalStatusId");
            }

            if (approval.OriginatingUserId == Guid.Empty)
            {
                missingFields.Append(" OriginatingUserId");
            }

            if (missingFields.ToString() == "")
            {
                return true;
            }
            else
            {
                LogWriter.LogMessage("Missing application information for fields: " + missingFields.ToString());
                return false;
            }
        }

        /// <summary>
        /// Inserts a LoanApplication, Approval, and LoanApplicationHistory object into the AccuAccount database
        /// </summary>
        /// <param name="application">The LoanApplication to insert</param>
        /// <param name="approval">The Approval to insert</param>
        /// <param name="applicationHistory">The ApplicationHistory to insert</param>
        private void InsertApplicationIntoDatabase(LoanApplication application, Approval approval, LoanApplicationHistory applicationHistory)
        {
            using (DataContext db = new DataContext())
            {
                int existingApplications = (from a in db.LoanApplications
                                            where a.ApplicationNumber.Equals(application.ApplicationNumber, StringComparison.OrdinalIgnoreCase) &&
                                                a.LoanId == application.LoanId
                                            select a).Count();

                if (existingApplications > 0)
                {
                    LogWriter.LogMessage("Application already exists: " + application.ApplicationNumber);
                    return;
                }

                db.LoanApplications.Add(application);
                db.Approvals.Add(approval);
                db.LoanApplicationHistories.Add(applicationHistory);

                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var error in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in error.ValidationErrors)
                        {
                            LogWriter.LogMessage(string.Format("Error inserting application. Message: {0}, Column: {1}",
                                validationError.ErrorMessage,
                                validationError.PropertyName));
                        }
                    }
                }
                catch (DbUpdateException ex)
                {
                    LogWriter.LogMessage(string.Format("Error updating database. {0}", ex.InnerException.Message));
                    
                }
            }
        }
        
        /// <summary>
        /// Looks up the CreditAnalysisStatusID based on the CreditAnalysisStatusCode
        /// </summary>
        /// <param name="creditAnalysisStatusCode">The CreditAnalysisStatusCode to lookup</param>
        /// <returns>The corresponding CreditAnalysisStatusID</returns>
        private Guid? GetCreditAnalysisStatusId(string creditAnalysisStatusCode)
        {
            using (DataContext db = new DataContext())
            {
                var creditAnalysisStatus = db.CreditAnalysisStatuses
                    .SingleOrDefault(p => p.CreditAnalysisStatusCode.Equals(creditAnalysisStatusCode,
                        StringComparison.OrdinalIgnoreCase));

                if (creditAnalysisStatus != null)
                {
                    return creditAnalysisStatus.CreditAnalysisStatusId;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Looks up the ApprovalStatusID based on the ApprovalStatusCode
        /// </summary>
        /// <param name="approvalStatusCode">The ApprovalStatusCode to lookup</param>
        /// <returns>The corresponding ApprovalStatusID</returns>
        private Guid GetApprovalStatusID(string approvalStatusCode)
        {
            using (DataContext db = new DataContext())
            {
                var approvalStatus = (from s in db.ApprovalStatuses
                                      where s.ApprovalStatusCode.Equals(approvalStatusCode, StringComparison.OrdinalIgnoreCase)
                                      select s).SingleOrDefault();

                if (approvalStatus != null)
                {
                    return approvalStatus.ApprovalStatusId;
                }
                else
                {
                    var defaultApprovalStatus = (from s in db.ApprovalStatuses
                                                 where s.IsDefault
                                                 select s).SingleOrDefault();
                    if (defaultApprovalStatus != null)
                    {
                        return defaultApprovalStatus.ApprovalStatusId;
                    }
                    else
                    {
                        return Guid.Empty;
                    }
                }
            }
        }

        /// <summary>
        /// Looks up the UserId or GroupId based on the User/Group Name and type
        /// </summary>
        /// <param name="assignedName">The User or Group name to lookup</param>
        /// <param name="assignedType">The user/group type.  Valid options are "user" and "group"</param>
        /// <returns>The corresponding UserId or GroupId</returns>
        private Guid? GetUserOrGroupId(string assignedName, string assignedType)
        {
            using (DataContext db = new DataContext())
            {
                switch (assignedType.ToLower())
                {
                    case "user":
                        var user = (from u in db.Users
                                    where u.UserLogin.Equals(assignedName, StringComparison.OrdinalIgnoreCase)
                                    select u).SingleOrDefault();
                        if (user != null)
                        {
                            return user.UserId;
                        }
                        break;
                    case "group":
                        var group = (from g in db.UserGroups
                                     where g.UserGroupCode.Equals(assignedName, StringComparison.OrdinalIgnoreCase)
                                     select g).SingleOrDefault();
                        if (group != null)
                        {
                            return group.UserGroupId;
                        }
                        break;
                }
                return null;
            }
        }

        /// <summary>
        /// Looks up a LoanId based on the LoanNumber
        /// </summary>
        /// <param name="loanNumber">The LoanNumber to lookup</param>
        /// <returns>The corresonding LoanId</returns>
        private Guid GetLoanId(string loanNumber)
        {
            using (DataContext db = new DataContext())
            {
                var loan = (from l in db.Loans
                            where l.LoanNumber.Equals(loanNumber, StringComparison.OrdinalIgnoreCase)
                            select l).SingleOrDefault();
                if (loan != null)
                {
                    return loan.LoanId;
                }
                else
                {
                    return Guid.Empty;
                }
            }
        }

    }
}
