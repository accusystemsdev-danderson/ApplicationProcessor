//-----------------------------------------------------------------------------
// <copyright file="Program.cs" company="AccuSystems LLC">
//     Copyright (c) AccuSystems.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------


namespace UnitTests

{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Data;
    using System.IO;
    using System.Xml.Linq;
    using System.Xml;
    
    [TestClass]
    public class RuleTests
    {
        [TestMethod]
        public void RuleTestAlwaysSetValueTo()
        {
            ApplicationProcessor.RuleDefinition rule = new ApplicationProcessor.RuleDefinition();
            rule.Field = "loanNumber";
            rule.Operator = "Always";
            rule.Value = "";
            rule.Action = "Set Value to:";
            rule.Parameter1 = "12345";
            rule.Parameter2 = "";
            rule.Parameter3 = "";

            ApplicationProcessor.SourceRecord record = new ApplicationProcessor.SourceRecord();

            record.LoanNumber = "11111";

            Helpers.SetupRuleXML(rule);
            
            ApplicationProcessor.Configuration.RulesFile = "testRules.xml";
            ApplicationProcessor.RuleProcessor.ClearRulesList();
            ApplicationProcessor.RuleProcessor.LoadRulesFromFile();
            ApplicationProcessor.RuleProcessor.ProcessRules(record);

            Assert.AreEqual("12345", record.LoanNumber);
        }

        [TestMethod]
        public void RuleTestEqualSetValueTo()
        {
            ApplicationProcessor.RuleDefinition rule = new ApplicationProcessor.RuleDefinition();
            rule.Field = "loanNumber";
            rule.Operator = "=";
            rule.Value = "11111";
            rule.Action = "Set Value to:";
            rule.Parameter1 = "12345";
            rule.Parameter2 = "";
            rule.Parameter3 = "";

            ApplicationProcessor.SourceRecord record = new ApplicationProcessor.SourceRecord();
            ApplicationProcessor.SourceRecord record2 = new ApplicationProcessor.SourceRecord();
            
            record.LoanNumber = "11111";
            record2.LoanNumber = "22222";
            Helpers.SetupRuleXML(rule);

            ApplicationProcessor.Configuration.RulesFile = "testRules.xml";
            ApplicationProcessor.RuleProcessor.ClearRulesList();
            ApplicationProcessor.RuleProcessor.LoadRulesFromFile();
            ApplicationProcessor.RuleProcessor.ProcessRules(record);
            ApplicationProcessor.RuleProcessor.ProcessRules(record2);

            Assert.AreEqual("12345", record.LoanNumber);
            Assert.AreNotEqual("12345", record2.LoanNumber);
        }

        [TestMethod]
        public void RuleTestNotEqualSetValueTo()
        {
            ApplicationProcessor.RuleDefinition rule = new ApplicationProcessor.RuleDefinition();
            rule.Field = "loanNumber";
            rule.Operator = "!=";
            rule.Value = "11111";
            rule.Action = "Set Value to:";
            rule.Parameter1 = "12345";
            rule.Parameter2 = "";
            rule.Parameter3 = "";

            ApplicationProcessor.SourceRecord record = new ApplicationProcessor.SourceRecord();
            ApplicationProcessor.SourceRecord record2 = new ApplicationProcessor.SourceRecord();

            record.LoanNumber = "11112";
            record2.LoanNumber = "11111";

            Helpers.SetupRuleXML(rule);

            ApplicationProcessor.Configuration.RulesFile = "testRules.xml";
            ApplicationProcessor.RuleProcessor.ClearRulesList();
            ApplicationProcessor.RuleProcessor.LoadRulesFromFile();
            ApplicationProcessor.RuleProcessor.ProcessRules(record);
            ApplicationProcessor.RuleProcessor.ProcessRules(record2);

            Assert.AreEqual("12345", record.LoanNumber);
            Assert.AreNotEqual("12345", record2.LoanNumber);
        }

        [TestMethod]
        public void RuleTestLessThanSetValueTo()
        {
            ApplicationProcessor.RuleDefinition rule = new ApplicationProcessor.RuleDefinition();
            rule.Field = "loanNumber";
            rule.Operator = "<";
            rule.Value = "11111";
            rule.Action = "Set Value to:";
            rule.Parameter1 = "12345";
            rule.Parameter2 = "";
            rule.Parameter3 = "";

            ApplicationProcessor.SourceRecord record = new ApplicationProcessor.SourceRecord();
            ApplicationProcessor.SourceRecord record2 = new ApplicationProcessor.SourceRecord();

            record.LoanNumber = "11110";
            record2.LoanNumber = "11111";

            Helpers.SetupRuleXML(rule);

            ApplicationProcessor.Configuration.RulesFile = "testRules.xml";
            ApplicationProcessor.RuleProcessor.ClearRulesList();
            ApplicationProcessor.RuleProcessor.LoadRulesFromFile();
            ApplicationProcessor.RuleProcessor.ProcessRules(record);
            ApplicationProcessor.RuleProcessor.ProcessRules(record2);

            Assert.AreEqual("12345", record.LoanNumber);
            Assert.AreNotEqual("12345", record2.LoanNumber);
        }

        [TestMethod]
        public void RuleTestLessThanEqualToSetValueTo()
        {
            ApplicationProcessor.RuleDefinition rule = new ApplicationProcessor.RuleDefinition();
            rule.Field = "loanNumber";
            rule.Operator = "<=";
            rule.Value = "11111";
            rule.Action = "Set Value to:";
            rule.Parameter1 = "12345";
            rule.Parameter2 = "";
            rule.Parameter3 = "";

            ApplicationProcessor.SourceRecord record = new ApplicationProcessor.SourceRecord();
            ApplicationProcessor.SourceRecord record2 = new ApplicationProcessor.SourceRecord();
            ApplicationProcessor.SourceRecord record3 = new ApplicationProcessor.SourceRecord();

            record.LoanNumber = "11110";
            record2.LoanNumber = "11111";
            record3.LoanNumber = "11112";

            Helpers.SetupRuleXML(rule);

            ApplicationProcessor.Configuration.RulesFile = "testRules.xml";
            ApplicationProcessor.RuleProcessor.ClearRulesList();
            ApplicationProcessor.RuleProcessor.LoadRulesFromFile();
            ApplicationProcessor.RuleProcessor.ProcessRules(record);
            ApplicationProcessor.RuleProcessor.ProcessRules(record2);
            ApplicationProcessor.RuleProcessor.ProcessRules(record3);

            Assert.AreEqual("12345", record.LoanNumber);
            Assert.AreEqual("12345", record2.LoanNumber);
            Assert.AreNotEqual("12345", record3.LoanNumber);
        }

        [TestMethod]
        public void RuleTestGreaterThanSetValueTo()
        {
            ApplicationProcessor.RuleDefinition rule = new ApplicationProcessor.RuleDefinition();
            rule.Field = "loanNumber";
            rule.Operator = ">";
            rule.Value = "11111";
            rule.Action = "Set Value to:";
            rule.Parameter1 = "12345";
            rule.Parameter2 = "";
            rule.Parameter3 = "";

            ApplicationProcessor.SourceRecord record = new ApplicationProcessor.SourceRecord();
            ApplicationProcessor.SourceRecord record2 = new ApplicationProcessor.SourceRecord();

            record.LoanNumber = "11112";
            record2.LoanNumber = "11111";

            Helpers.SetupRuleXML(rule);

            ApplicationProcessor.Configuration.RulesFile = "testRules.xml";
            ApplicationProcessor.RuleProcessor.ClearRulesList();
            ApplicationProcessor.RuleProcessor.LoadRulesFromFile();
            ApplicationProcessor.RuleProcessor.ProcessRules(record);
            ApplicationProcessor.RuleProcessor.ProcessRules(record2);

            Assert.AreEqual("12345", record.LoanNumber);
            Assert.AreNotEqual("12345", record2.LoanNumber);
        }

        [TestMethod]
        public void RuleTestGreaterThanEqualToSetValueTo()
        {
            ApplicationProcessor.RuleDefinition rule = new ApplicationProcessor.RuleDefinition();
            rule.Field = "loanNumber";
            rule.Operator = ">=";
            rule.Value = "11111";
            rule.Action = "Set Value to:";
            rule.Parameter1 = "12345";
            rule.Parameter2 = "";
            rule.Parameter3 = "";

            ApplicationProcessor.SourceRecord record = new ApplicationProcessor.SourceRecord();
            ApplicationProcessor.SourceRecord record2 = new ApplicationProcessor.SourceRecord();
            ApplicationProcessor.SourceRecord record3 = new ApplicationProcessor.SourceRecord();

            record.LoanNumber = "11110";
            record2.LoanNumber = "11111";
            record3.LoanNumber = "11112";

            Helpers.SetupRuleXML(rule);

            ApplicationProcessor.Configuration.RulesFile = "testRules.xml";
            ApplicationProcessor.RuleProcessor.ClearRulesList();
            ApplicationProcessor.RuleProcessor.LoadRulesFromFile();
            ApplicationProcessor.RuleProcessor.ProcessRules(record);
            ApplicationProcessor.RuleProcessor.ProcessRules(record2);
            ApplicationProcessor.RuleProcessor.ProcessRules(record3);

            Assert.AreNotEqual("12345", record.LoanNumber);
            Assert.AreEqual("12345", record2.LoanNumber);
            Assert.AreEqual("12345", record3.LoanNumber);
        }

        [TestMethod]
        public void RuleTestHasValueSetValueTo()
        {
            ApplicationProcessor.RuleDefinition rule = new ApplicationProcessor.RuleDefinition();
            rule.Field = "loanNumber";
            rule.Operator = "Has Value";
            rule.Value = "";
            rule.Action = "Set Value to:";
            rule.Parameter1 = "12345";
            rule.Parameter2 = "";
            rule.Parameter3 = "";

            ApplicationProcessor.SourceRecord record = new ApplicationProcessor.SourceRecord();
            ApplicationProcessor.SourceRecord record2 = new ApplicationProcessor.SourceRecord();

            record.LoanNumber = "11111";
            record2.LoanNumber = "";

            Helpers.SetupRuleXML(rule);

            ApplicationProcessor.Configuration.RulesFile = "testRules.xml";
            ApplicationProcessor.RuleProcessor.ClearRulesList();
            ApplicationProcessor.RuleProcessor.LoadRulesFromFile();
            ApplicationProcessor.RuleProcessor.ProcessRules(record);
            ApplicationProcessor.RuleProcessor.ProcessRules(record2);

            Assert.AreEqual("12345", record.LoanNumber);
            Assert.AreNotEqual("12345", record2.LoanNumber);
        }

        [TestMethod]
        public void RuleTestHasNoValueSetValueTo()
        {
            ApplicationProcessor.RuleDefinition rule = new ApplicationProcessor.RuleDefinition();
            rule.Field = "loanNumber";
            rule.Operator = "Has no Value";
            rule.Value = "";
            rule.Action = "Set Value to:";
            rule.Parameter1 = "12345";
            rule.Parameter2 = "";
            rule.Parameter3 = "";

            ApplicationProcessor.SourceRecord record = new ApplicationProcessor.SourceRecord();
            ApplicationProcessor.SourceRecord record2 = new ApplicationProcessor.SourceRecord();

            record.LoanNumber = "11111";
            record2.LoanNumber = "";

            Helpers.SetupRuleXML(rule);

            ApplicationProcessor.Configuration.RulesFile = "testRules.xml";
            ApplicationProcessor.RuleProcessor.ClearRulesList();
            ApplicationProcessor.RuleProcessor.LoadRulesFromFile();
            ApplicationProcessor.RuleProcessor.ProcessRules(record);
            ApplicationProcessor.RuleProcessor.ProcessRules(record2);

            Assert.AreNotEqual("12345", record.LoanNumber);
            Assert.AreEqual("12345", record2.LoanNumber);
        }

        [TestMethod]
        public void RuleTestAlwaysSetValueToField()
        {
            ApplicationProcessor.RuleDefinition rule = new ApplicationProcessor.RuleDefinition();
            rule.Field = "loanNumber";
            rule.Operator = "Always";
            rule.Value = "";
            rule.Action = "Set Value to field:";
            rule.Parameter1 = "loanDescription";
            rule.Parameter2 = "";
            rule.Parameter3 = "";

            ApplicationProcessor.SourceRecord record = new ApplicationProcessor.SourceRecord();

            record.LoanNumber = "11111";
            record.LoanDescription = "Test";
            

            Helpers.SetupRuleXML(rule);

            ApplicationProcessor.Configuration.RulesFile = "testRules.xml";
            ApplicationProcessor.RuleProcessor.ClearRulesList();
            ApplicationProcessor.RuleProcessor.LoadRulesFromFile();
            ApplicationProcessor.RuleProcessor.ProcessRules(record);
            
            Assert.AreEqual("Test", record.LoanNumber);
            
        }

        [TestMethod]
        public void RuleTestAlwaysSetOtherFieldTo()
        {
            ApplicationProcessor.RuleDefinition rule = new ApplicationProcessor.RuleDefinition();
            rule.Field = "loanNumber";
            rule.Operator = "Always";
            rule.Value = "";
            rule.Action = "Set Other Field to:";
            rule.Parameter1 = "loanDescription";
            rule.Parameter2 = "Test";
            rule.Parameter3 = "";

            ApplicationProcessor.SourceRecord record = new ApplicationProcessor.SourceRecord();

            record.LoanNumber = "11111";
            record.LoanDescription = "Description";
            
            Helpers.SetupRuleXML(rule);

            ApplicationProcessor.Configuration.RulesFile = "testRules.xml";
            ApplicationProcessor.RuleProcessor.ClearRulesList();
            ApplicationProcessor.RuleProcessor.LoadRulesFromFile();
            ApplicationProcessor.RuleProcessor.ProcessRules(record);

            Assert.AreEqual("Test", record.LoanDescription);

        }

        [TestMethod]
        public void RuleTestAlwaysSkipRecord()
        {
            ApplicationProcessor.RuleDefinition rule = new ApplicationProcessor.RuleDefinition();
            rule.Field = "loanNumber";
            rule.Operator = "Always";
            rule.Value = "";
            rule.Action = "Skip Record";
            rule.Parameter1 = "";
            rule.Parameter2 = "";
            rule.Parameter3 = "";

            ApplicationProcessor.SourceRecord record = new ApplicationProcessor.SourceRecord();

            record.LoanNumber = "11111";
            
            Helpers.SetupRuleXML(rule);
            Helpers.SetupLogging();

            ApplicationProcessor.Configuration.RulesFile = "testRules.xml";
            ApplicationProcessor.RuleProcessor.ClearRulesList();
            ApplicationProcessor.RuleProcessor.LoadRulesFromFile();
            ApplicationProcessor.RuleProcessor.ProcessRules(record);

            Assert.IsTrue(record.IgnoreRecord);

        }

        [TestMethod]
        public void RuleTestAlwaysCombineFieldsWithSpace()
        {
            ApplicationProcessor.RuleDefinition rule = new ApplicationProcessor.RuleDefinition();
            rule.Field = "loanNumber";
            rule.Operator = "Always";
            rule.Value = "";
            rule.Action = "Combine fields with space";
            rule.Parameter1 = "loanNumber";
            rule.Parameter2 = "loanDescription";
            rule.Parameter3 = "";

            ApplicationProcessor.SourceRecord record = new ApplicationProcessor.SourceRecord();

            record.LoanNumber = "11111";
            record.LoanDescription = "Test";

            Helpers.SetupRuleXML(rule);

            ApplicationProcessor.Configuration.RulesFile = "testRules.xml";
            ApplicationProcessor.RuleProcessor.ClearRulesList();
            ApplicationProcessor.RuleProcessor.LoadRulesFromFile();
            ApplicationProcessor.RuleProcessor.ProcessRules(record);

            Assert.AreEqual("11111 Test", record.LoanNumber);
        }

        [TestMethod]
        public void RuleTestAlwaysCombineFieldsNoSpace()
        {
            ApplicationProcessor.RuleDefinition rule = new ApplicationProcessor.RuleDefinition();
            rule.Field = "loanNumber";
            rule.Operator = "Always";
            rule.Value = "";
            rule.Action = "Combine fields no space";
            rule.Parameter1 = "loanNumber";
            rule.Parameter2 = "loanDescription";
            rule.Parameter3 = "";

            ApplicationProcessor.SourceRecord record = new ApplicationProcessor.SourceRecord();

            record.LoanNumber = "11111";
            record.LoanDescription = "Test";

            Helpers.SetupRuleXML(rule);

            ApplicationProcessor.Configuration.RulesFile = "testRules.xml";
            ApplicationProcessor.RuleProcessor.ClearRulesList();
            ApplicationProcessor.RuleProcessor.LoadRulesFromFile();
            ApplicationProcessor.RuleProcessor.ProcessRules(record);

            Assert.AreEqual("11111Test", record.LoanNumber);
        }

        [TestMethod]
        public void RuleTestAlwaysAppendFieldWithText()
        {
            ApplicationProcessor.RuleDefinition rule = new ApplicationProcessor.RuleDefinition();
            rule.Field = "loanNumber";
            rule.Operator = "Always";
            rule.Value = "";
            rule.Action = "Append field with text";
            rule.Parameter1 = "Test";
            rule.Parameter2 = "";
            rule.Parameter3 = "";

            ApplicationProcessor.SourceRecord record = new ApplicationProcessor.SourceRecord();

            record.LoanNumber = "11111";
            
            Helpers.SetupRuleXML(rule);

            ApplicationProcessor.Configuration.RulesFile = "testRules.xml";
            ApplicationProcessor.RuleProcessor.ClearRulesList();
            ApplicationProcessor.RuleProcessor.LoadRulesFromFile();
            ApplicationProcessor.RuleProcessor.ProcessRules(record);

            Assert.AreEqual("11111Test", record.LoanNumber);
        }

        [TestMethod]
        public void RuleTestAlwaysPrependFieldWithText()
        {
            ApplicationProcessor.RuleDefinition rule = new ApplicationProcessor.RuleDefinition();
            rule.Field = "loanNumber";
            rule.Operator = "Always";
            rule.Value = "";
            rule.Action = "Prepend field with text";
            rule.Parameter1 = "Test";
            rule.Parameter2 = "";
            rule.Parameter3 = "";

            ApplicationProcessor.SourceRecord record = new ApplicationProcessor.SourceRecord();

            record.LoanNumber = "11111";

            Helpers.SetupRuleXML(rule);

            ApplicationProcessor.Configuration.RulesFile = "testRules.xml";
            ApplicationProcessor.RuleProcessor.ClearRulesList();
            ApplicationProcessor.RuleProcessor.LoadRulesFromFile();
            ApplicationProcessor.RuleProcessor.ProcessRules(record);

            Assert.AreEqual("Test11111", record.LoanNumber);
        }

        [TestMethod]
        public void RuleTestAlwaysReplaceText()
        {
            ApplicationProcessor.RuleDefinition rule = new ApplicationProcessor.RuleDefinition();
            rule.Field = "loanNumber";
            rule.Operator = "Always";
            rule.Value = "";
            rule.Action = "Replace text";
            rule.Parameter1 = "_";
            rule.Parameter2 = " ";
            rule.Parameter3 = "";

            ApplicationProcessor.SourceRecord record = new ApplicationProcessor.SourceRecord();

            record.LoanNumber = "11_111";

            Helpers.SetupRuleXML(rule);

            ApplicationProcessor.Configuration.RulesFile = "testRules.xml";
            ApplicationProcessor.RuleProcessor.ClearRulesList();
            ApplicationProcessor.RuleProcessor.LoadRulesFromFile();
            ApplicationProcessor.RuleProcessor.ProcessRules(record);

            Assert.AreEqual("11 111", record.LoanNumber);
        }

        [TestMethod]
        public void RuleTestAlwaysLookupCodeFromDB()
        {
            ApplicationProcessor.RuleDefinition rule = new ApplicationProcessor.RuleDefinition();
            rule.Field = "loanNumber";
            rule.Operator = "Always";
            rule.Value = "";
            rule.Action = "Replace text";
            rule.Parameter1 = "_";
            rule.Parameter2 = " ";
            rule.Parameter3 = "";

            ApplicationProcessor.SourceRecord record = new ApplicationProcessor.SourceRecord();

            record.LoanNumber = "11_111";

            Helpers.SetupRuleXML(rule);

            ApplicationProcessor.Configuration.RulesFile = "testRules.xml";
            ApplicationProcessor.RuleProcessor.ClearRulesList();
            ApplicationProcessor.RuleProcessor.LoadRulesFromFile();
            ApplicationProcessor.RuleProcessor.ProcessRules(record);

            Assert.AreEqual("11 111", record.LoanNumber);
        }
    }
}
