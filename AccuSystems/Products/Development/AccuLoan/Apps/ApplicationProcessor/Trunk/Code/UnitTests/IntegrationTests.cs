//-----------------------------------------------------------------------------
// <copyright file="Program.cs" company="AccuSystems LLC">
//     Copyright (c) AccuSystems.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    /// <summary>
    /// Summary description for IntegrationTests
    /// </summary>
    [TestClass]
    public class IntegrationTests
    {
        [TestMethod]
        public void RuleTestAlwaysLookupCodeFromDB()
        {
            ApplicationProcessor.RuleDefinition rule = new ApplicationProcessor.RuleDefinition();
            rule.Field = "loanNumber";
            rule.Operator = "Always";
            rule.Value = "";
            rule.Action = "Lookup Code from DB";
            rule.Parameter1 = "user";
            rule.Parameter2 = "userLogin";
            rule.Parameter3 = "userId";

            ApplicationProcessor.SourceRecord record = new ApplicationProcessor.SourceRecord();

            record.LoanNumber = "scanner";

            TestUtils.SetupRuleXML(rule);

            ApplicationProcessor.Configuration.RulesFile = "testRules.xml";
            ApplicationProcessor.RuleProcessor.ClearRulesList();
            ApplicationProcessor.RuleProcessor.LoadRulesFromFile();
            ApplicationProcessor.RuleProcessor.ProcessRules(record);

            Assert.AreEqual("311E430E-9F17-4AC1-B236-7FEC254E318F", record.LoanNumber.ToUpper());
        }
    }
}
