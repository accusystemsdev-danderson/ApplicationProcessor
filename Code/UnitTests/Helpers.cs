//-----------------------------------------------------------------------------
// <copyright file="Program.cs" company="AccuSystems LLC">
//     Copyright (c) AccuSystems.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------


namespace UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using System.Xml;

    public static class Helpers
    {
        public static void SetupRuleXML(ApplicationProcessor.RuleDefinition rule)
        {
            XElement ruleXml = new XElement("ApplicationRules",
                new XElement("Rule",
                new XAttribute("Field", rule.Field),
                new XAttribute("Operator", rule.Operator),
                new XAttribute("Value", rule.Value),
                new XAttribute("Action", rule.Action),
                new XAttribute("Parameter1", rule.Parameter1),
                new XAttribute("Parameter2", rule.Parameter2),
                new XAttribute("Parameter3", rule.Parameter3)));

            using (XmlWriter writer = XmlWriter.Create("testRules.xml"))
            {
                XDocument doc = new XDocument();
                doc.Add(ruleXml);
                doc.WriteTo(writer);
                writer.Flush();
            }
        }

        public static void SetupLogging()
        {
            ApplicationProcessor.LogWriter.LogFilePath = "logs\\";
            ApplicationProcessor.LogWriter.LogFileName = "UnitTestLog.txt";

            ApplicationProcessor.LogWriter.OpenLog();
        }
    }
}
