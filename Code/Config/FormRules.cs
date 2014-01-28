//-----------------------------------------------------------------------------
// <copyright file="FormRules.cs" company="AccuSystems LLC">
//     Copyright (c) AccuSystems LLC  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using System.IO;
using System.Xml;

namespace Config
{
    using System.Globalization;

    public partial class FormRules : Form
    {
        private const string ConfigFileName = "config.xml";
        public DataTable RulesTable = new DataTable();
        public XElement XmlConfig = XElement.Load(ConfigFileName);
        public List<string> FieldNames = new List<string>();
        public decimal NextSequence = 0;

        public FormRules()
        {
            InitializeComponent();
        }

        private void FormRules_Load(object sender, EventArgs e)
        {
            if (XmlConfig == null)
            {
                MessageBox.Show(string.Format("Unable to find the file {0}", 
                    ConfigFileName));
                Close();
            }
            InitializeFields();

        }

        private void InitializeFields()
        {
            XElement rulesFileElement = XmlConfig.Element("RulesFile");

            string rulesFile = "";
            if (rulesFileElement != null)
            {
                rulesFile = rulesFileElement.Value;
            }
            else
            {
                MessageBox.Show(@"Unable to read rules file name from config file");
                Close();
            }


            if (!File.Exists(rulesFile))
            {
                MessageBox.Show(string.Format("The requested file does not exist: {0}", rulesFile));
                Close();
            }
            else
            {
                XElement xml = XElement.Load(rulesFile);

                this.RulesTable.Columns.Add("Sequence");
                this.RulesTable.Columns["Sequence"].DataType = typeof(Decimal);
                this.RulesTable.Columns.Add("Field");
                this.RulesTable.Columns.Add("Operator");
                this.RulesTable.Columns.Add("Value");
                this.RulesTable.Columns.Add("Action");
                this.RulesTable.Columns.Add("Parameter1");
                this.RulesTable.Columns.Add("Parameter2");
                this.RulesTable.Columns.Add("Parameter3");

                foreach (XElement element in xml.Elements("Rule"))
                {
                    DataRow newRow = this.RulesTable.NewRow();
                    newRow["Sequence"] = decimal.Parse(element.Attribute("Sequence").Value);
                    newRow["Field"] = element.Attribute("Field").Value;
                    newRow["Operator"] = element.Attribute("Operator").Value;
                    newRow["Value"] = element.Attribute("Value").Value;
                    newRow["Action"] = element.Attribute("Action").Value;
                    newRow["Parameter1"] = element.Attribute("Parameter1").Value;
                    newRow["Parameter2"] = element.Attribute("Parameter2").Value;
                    newRow["Parameter3"] = element.Attribute("Parameter3").Value;

                    this.RulesTable.Rows.Add(newRow);
                }

                dataGridViewRules.DataSource = this.RulesTable;
                dataGridViewRules.Sort( dataGridViewRules.Columns[0], ListSortDirection.Ascending);
            }
            this.NextSequence = this.RulesTable.Rows.Count + 1;

            numericUpDownSequence.Value = this.NextSequence;

            GetFieldNamesFromInputFile(out this.FieldNames);

            comboBoxField.Items.AddRange(FieldNames.ToArray());

            var operators = new List<string>
                            {
                                "=",
                                "!=",
                                "<",
                                "<=",
                                ">",
                                ">=",
                                "Has Value",
                                "Has no Value",
                                "Contains Text",
                                "Contained in Text",
                                "Always"
                            };
            comboBoxOperator.Items.AddRange(operators.ToArray());

            var actions = new List<string>
                          {
                              "Set Value to:",
                              "Set Value to field:",
                              "Set Other Field to:",
                              "Skip Record",
                              "Combine fields with space",
                              "Combine fields no space",
                              "Append field with text",
                              "Prepend field with text",
                              "Replace text",
                              "Lookup Code from DB",
                              "Convert to short date",
                              "Next Collateral Number",
                              "Pad Collateral Addenda"
                          };

            comboBoxAction.Items.AddRange(actions.ToArray());
        
        }

        private void GetFieldNamesFromInputFile(out List<string> fieldNames)
        {
            fieldNames = new List<string>();

            XElement xFieldMapFile = this.XmlConfig.Element("FieldMapFile");

            if (xFieldMapFile == null)
            {
                return;
            }

            string inputFileName = xFieldMapFile.Value;

            XElement xmlFields = XElement.Load(inputFileName);

            foreach (XElement field in xmlFields.Elements())
            {
                string fieldName = field.Name.ToString();
                string mappedField = field.Value;
                fieldNames.Add(mappedField != ""
                                   ? string.Format("{0} ({1})",
                                                   fieldName,
                                                   mappedField)
                                   : fieldName);
            }

        }

        private void ComboBoxOperator_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxOperator.Text == @"=" ||
                comboBoxOperator.Text == @"!=" ||
                comboBoxOperator.Text == @"<" ||
                comboBoxOperator.Text == @"<=" ||
                comboBoxOperator.Text == @">" ||
                comboBoxOperator.Text == @">=" ||
                comboBoxOperator.Text == @"Contains Text" ||
                comboBoxOperator.Text == @"Contained in Text")
            {
                comboBoxValue.Enabled = true;
            }
            else
            {
                comboBoxValue.Enabled = false;
            }

        }

        private void ButtonAddRule_Click(object sender, EventArgs e)
        {
            DataRow newRow = this.RulesTable.NewRow();

            newRow["Sequence"] = numericUpDownSequence.Value;
            newRow["Field"] = comboBoxField.Text;
            newRow["Operator"] = comboBoxOperator.Text;
            newRow["Value"] = comboBoxValue.Text;
            newRow["Action"] = comboBoxAction.Text;
            newRow["Parameter1"] = comboBoxParameter1.Text;
            newRow["Parameter2"] = comboBoxParameter2.Text;
            newRow["Parameter3"] = comboBoxParameter3.Text;

            
            //reorder existing sequences if necessary
            decimal addedSequence = numericUpDownSequence.Value;

            if (numericUpDownSequence.Value < this.NextSequence)
            {
                foreach (DataRow row in this.RulesTable.Rows)
                { 
                    decimal rowSequence = decimal.Parse(row["Sequence"].ToString());
                    if (rowSequence >= addedSequence)
                    {
                        decimal newSequence = rowSequence + 1;
                        row["Sequence"] = newSequence;
                    }
                }
            
            }

 
            this.RulesTable.Rows.Add(newRow);
            dataGridViewRules.Sort(dataGridViewRules.Columns[0], ListSortDirection.Ascending);
            SequenceRules(); 

            comboBoxField.SelectedItem = null;
            comboBoxOperator.SelectedItem = null;
            comboBoxValue.Text = "";
            comboBoxAction.SelectedItem = null;
            comboBoxParameter1.Text = "";
            comboBoxParameter2.Text = "";
            comboBoxParameter3.Text = "";

        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(@"Save Current Configuration?", @"Save?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                dataGridViewRules.Sort(dataGridViewRules.Columns[0], ListSortDirection.Ascending);

                string rulesFile;
                XElement xRulesFile = XmlConfig.Element("RulesFile");
                
                if (xRulesFile != null)
                {
                    rulesFile = xRulesFile.Value;
                }
                else
                {
                    var result =
                        MessageBox.Show(
                                        @"Unable to find the rules file name in the configuration.  Use the default (rules.xml)",
                                        @"Rules file name",
                                        MessageBoxButtons.OKCancel);
                    if (result == DialogResult.OK)
                    {
                        rulesFile = "rules.xml";
                    }
                    else
                    {
                        return;
                    }
                }
                
                XElement xml = XElement.Load(rulesFile);
                string startElement = xml.Name.ToString();

                var xmlRules = new XmlWriterSettings
                               {
                                   Indent = true,
                                   NewLineOnAttributes = true,
                                   OmitXmlDeclaration = true,
                                   Encoding = Encoding.Default
                               };

                using (XmlWriter newConfig = XmlWriter.Create(rulesFile, xmlRules))
                {
                    newConfig.WriteStartDocument();
                    newConfig.WriteStartElement(startElement);

                    for (int rows = 0; rows < dataGridViewRules.Rows.Count; rows++)
                    {
                        newConfig.WriteStartElement("Rule");

                        for (int col = 0; col < dataGridViewRules.Rows[rows].Cells.Count; col ++)
                        {
                            string field = dataGridViewRules.Columns[col].Name;
                            string value = dataGridViewRules.Rows[rows].Cells[col].Value.ToString();
                            if (field == "Field" || field == "Parameter1" || field == "Parameter2" || field == "Parameter3")
                            {
                                if (value.Contains(" ("))
                                    value = value.Remove(value.IndexOf(" (", StringComparison.Ordinal));
                            }
                            newConfig.WriteAttributeString(field, value);
                        }

                        newConfig.WriteEndElement();

                    }
                    newConfig.WriteEndElement();
                    newConfig.WriteEndDocument();
                }
                Close();
            }
        }

        private void ComboBoxAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBoxAction.Text)
            {
                case "Set Value to:":
                    comboBoxParameter1.Enabled = true;
                    comboBoxParameter2.Enabled = false;
                    labelParameter1.Text = @"Assigned Value";
                    break;
                case "Set Value to field:":
                    comboBoxParameter1.Enabled = true;
                    labelParameter1.Text = @"Assigned field";
                    comboBoxParameter1.Items.AddRange(this.FieldNames.ToArray());
                    break;
                case "Set Other Field to:":
                    comboBoxParameter1.Enabled = true;
                    comboBoxParameter2.Enabled = true;
                    labelParameter1.Text = @"Field to Set";
                    labelParameter2.Text = @"Value";
                    comboBoxParameter1.Items.AddRange(this.FieldNames.ToArray());
                    break;
                case "Combine fields with space":
                    comboBoxParameter1.Enabled = true;
                    comboBoxParameter2.Enabled = true;
                    labelParameter1.Text = @"Combined field 1";
                    labelParameter2.Text = @"Combined field 2";
                    comboBoxParameter1.Items.AddRange(this.FieldNames.ToArray());
                    comboBoxParameter2.Items.AddRange(this.FieldNames.ToArray());
                    break;

                case "Combine fields no space":
                    comboBoxParameter1.Enabled = true;
                    comboBoxParameter2.Enabled = true;
                    labelParameter1.Text = @"Combined field 1";
                    labelParameter2.Text = @"Combined field 2";
                    comboBoxParameter1.Items.AddRange(this.FieldNames.ToArray());
                    comboBoxParameter2.Items.AddRange(this.FieldNames.ToArray());
                    break;
                case "Append field with text":
                    comboBoxParameter1.Enabled = true;
                    labelParameter1.Text = @"Text to append";
                    break;
                case "Prepend field with text":
                    comboBoxParameter1.Enabled = true;
                    labelParameter1.Text = @"Text to prepend";
                    break;
                case "Lookup Code from DB":
                    comboBoxParameter1.Enabled = true;
                    comboBoxParameter2.Enabled = true;
                    comboBoxParameter3.Enabled = true;
                    labelParameter1.Text = @"Lookup Table";
                    labelParameter2.Text = @"Lookup Field";
                    labelParameter3.Text = @"Result Field";
                    break;
                case "Replace text":
                    comboBoxParameter1.Enabled = true;
                    comboBoxParameter2.Enabled = true;
                    labelParameter1.Text = @"Search Text";
                    labelParameter2.Text = @"Replacement Text";
                    break;
                default:           
                    comboBoxParameter1.Enabled = false;
                    comboBoxParameter2.Enabled = false;
                    comboBoxParameter3.Enabled = false;
                    labelParameter1.Text = @"Parameter 1";
                    labelParameter2.Text = @"Parameter 2";
                    labelParameter3.Text = @"Parameter 3";
                    break;
            }


        }

        private void DataGridViewRules_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            //sequence rules after deletion
            SequenceRules();

        }

        private void ButtonEditRule_Click(object sender, EventArgs e)

        {
            var currentRow = dataGridViewRules.CurrentRow;
            if (currentRow == null)
            {
                MessageBox.Show(@"There is no rule currently selected", @"Select rule", MessageBoxButtons.OK);
                return;
            }
            numericUpDownSequence.Value = decimal.Parse(currentRow.Cells["Sequence"].Value.ToString());

            comboBoxField.SelectedIndex = FindIndexOfText(comboBoxField, currentRow.Cells["Field"].Value.ToString());
            comboBoxOperator.SelectedIndex = FindIndexOfText(comboBoxOperator, currentRow.Cells["Operator"].Value.ToString());
            if (comboBoxValue.Items.Count > 0)
                comboBoxValue.SelectedIndex = FindIndexOfText(comboBoxValue, currentRow.Cells["Value"].Value.ToString());
            else
                comboBoxValue.Text = currentRow.Cells["Value"].Value.ToString();
            comboBoxAction.SelectedIndex = FindIndexOfText(comboBoxAction, currentRow.Cells["Action"].Value.ToString());
            if (comboBoxParameter1.Items.Count > 0)
                comboBoxParameter1.SelectedIndex = FindIndexOfText(comboBoxParameter1, currentRow.Cells["Parameter1"].Value.ToString());
            else
                comboBoxParameter1.Text = currentRow.Cells["Parameter1"].Value.ToString();
            if (comboBoxParameter2.Items.Count > 0)
                comboBoxParameter2.SelectedIndex = FindIndexOfText(comboBoxParameter2, currentRow.Cells["Parameter2"].Value.ToString());
            else
                comboBoxParameter2.Text = currentRow.Cells["Parameter2"].Value.ToString();
            if (comboBoxParameter3.Items.Count > 0)
                comboBoxParameter3.SelectedIndex = FindIndexOfText(comboBoxParameter3, currentRow.Cells["Parameter3"].Value.ToString());
            else
                comboBoxParameter3.Text = currentRow.Cells["Parameter3"].Value.ToString();

            dataGridViewRules.Rows.Remove(currentRow);

        }

        private int FindIndexOfText(ComboBox cb, string text)
        {
            int foundIndex = -1;
            
            for (int i = 0; i < cb.Items.Count; i++)
			{
                string textFromList = cb.Items[i].ToString();
                int startIndex = textFromList.IndexOf(" (", StringComparison.Ordinal);
                if (startIndex > 0)
                    textFromList = textFromList.Remove(textFromList.IndexOf(" (", StringComparison.Ordinal));
			    if (text == textFromList)
                    foundIndex = i;
			}

            return foundIndex;
        }

        private void SequenceRules()
        {

            for (int i = 0; i < dataGridViewRules.Rows.Count; i++)
            {
                dataGridViewRules.Rows[i].Cells["Sequence"].Value = double.Parse(i.ToString(CultureInfo.InvariantCulture)) + 1;
            }

            this.NextSequence = this.RulesTable.Rows.Count + 1;
            numericUpDownSequence.Value = this.NextSequence;
        }

    }
}

