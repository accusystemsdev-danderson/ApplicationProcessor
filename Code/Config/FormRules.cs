using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.IO;
using System.Xml;

namespace Config
{
    public partial class FormRules : Form
    {
        public DataTable dataTable = new DataTable();
        public XElement xmlConfig = XElement.Load("config.xml");
        public List<string> fieldNames = new List<string>();
        public decimal nextSequence = 0;

        public FormRules()
        {
            InitializeComponent();
        }

        private void FormRules_Load(object sender, EventArgs e)
        {
            initializeFields();

        }

        private void initializeFields()
        {
            string rulesFile = xmlConfig.Element("RulesFile").Value;
            if (!File.Exists(rulesFile))
            {
                MessageBox.Show(string.Format("The requested file does not exist: {0}", rulesFile));
                Close();
            }
            else
            {
                XElement xml = XElement.Load(rulesFile);

                dataTable.Columns.Add("Sequence");
                dataTable.Columns["Sequence"].DataType = typeof(System.Decimal);
                dataTable.Columns.Add("Field");
                dataTable.Columns.Add("Operator");
                dataTable.Columns.Add("Value");
                dataTable.Columns.Add("Action");
                dataTable.Columns.Add("Parameter1");
                dataTable.Columns.Add("Parameter2");
                dataTable.Columns.Add("Parameter3");

                foreach (XElement element in xml.Elements("Rule"))
                {
                    DataRow newRow = dataTable.NewRow();
                    newRow["Sequence"] = decimal.Parse(element.Attribute("Sequence").Value);
                    newRow["Field"] = element.Attribute("Field").Value;
                    newRow["Operator"] = element.Attribute("Operator").Value;
                    newRow["Value"] = element.Attribute("Value").Value;
                    newRow["Action"] = element.Attribute("Action").Value;
                    newRow["Parameter1"] = element.Attribute("Parameter1").Value;
                    newRow["Parameter2"] = element.Attribute("Parameter2").Value;
                    newRow["Parameter3"] = element.Attribute("Parameter3").Value;

                    dataTable.Rows.Add(newRow);
                }

                dataGridViewRules.DataSource = dataTable;
                dataGridViewRules.Sort( dataGridViewRules.Columns[0], ListSortDirection.Ascending);
            }
            nextSequence = dataTable.Rows.Count + 1;

            numericUpDownSequence.Value = nextSequence;

            getFieldNamesFromInputFile(out fieldNames);

            comboBoxField.Items.AddRange(fieldNames.ToArray());

            List<string> operators = new List<string>() { "=", "!=", "<", "<=", ">", ">=", "Has Value", "Has no Value", "Always" };
            comboBoxOperator.Items.AddRange(operators.ToArray());
            
            List<string> Actions = new List<string>() { "Set Value to:", "Set Value to field:", "Set Other Field to:", "Skip Record", 
                "Combine fields with space", "Combine fields no space", "Append field with text", "Prepend field with text",
                "Replace text", "Lookup Code from DB", "Convert to short date", "Next Collateral Number", "Pad Collateral Addenda"};
            comboBoxAction.Items.AddRange(Actions.ToArray());
        
        }

        private void getFieldNamesFromInputFile(out List<string> fieldNames)
        {
            fieldNames = new List<string>();

            string inputFileName = xmlConfig.Element("FieldMapFile").Value;

            XElement xmlFields = XElement.Load(inputFileName);

            foreach (XElement field in xmlFields.Elements())
            {
                string fieldName = field.Name.ToString();
                string mappedField = field.Value.ToString();
                if (mappedField != "")
                {
                    fieldNames.Add(string.Format("{0} ({1})", fieldName, mappedField));
                }
                else
                {
                    fieldNames.Add(fieldName);
                }
            }

        }

        private void comboBoxOperator_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxOperator.Text == "=" ||
                comboBoxOperator.Text == "!=" ||
                comboBoxOperator.Text == "<" ||
                comboBoxOperator.Text == "<=" ||
                comboBoxOperator.Text == ">" ||
                comboBoxOperator.Text == ">=")
            {
                comboBoxValue.Enabled = true;
            }
            else
            {
                comboBoxValue.Enabled = false;
            }

        }

        private void buttonAddRule_Click(object sender, EventArgs e)
        {
            DataRow newRow = dataTable.NewRow();

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

            if (numericUpDownSequence.Value < nextSequence)
            {
                foreach (DataRow row in dataTable.Rows)
                { 
                    decimal rowSequence = decimal.Parse(row["Sequence"].ToString());
                    if (rowSequence >= addedSequence)
                    {
                        decimal newSequence = rowSequence + 1;
                        row["Sequence"] = newSequence;
                    }
                }
            
            }

 
            dataTable.Rows.Add(newRow);
            dataGridViewRules.Sort(dataGridViewRules.Columns[0], ListSortDirection.Ascending);
            sequenceRules(); 

            comboBoxField.SelectedItem = null;
            comboBoxOperator.SelectedItem = null;
            comboBoxValue.Text = "";
            comboBoxAction.SelectedItem = null;
            comboBoxParameter1.Text = "";
            comboBoxParameter2.Text = "";
            comboBoxParameter3.Text = "";

        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Save Current Configuration?", "Save?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                dataGridViewRules.Sort(dataGridViewRules.Columns[0], ListSortDirection.Ascending);

                string rulesFile = xmlConfig.Element("RulesFile").Value;
                XElement xml = XElement.Load(rulesFile);
                string startElement = xml.Name.ToString();

                XmlWriterSettings xmlRules = new XmlWriterSettings();
                xmlRules.Indent = true;
                xmlRules.NewLineOnAttributes = true;
                xmlRules.OmitXmlDeclaration = true;
                xmlRules.Encoding = Encoding.Default;

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
                                    value = value.Remove(value.IndexOf(" ("));
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

        private void comboBoxAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBoxAction.Text)
            {
                case "Set Value to:":
                    comboBoxParameter1.Enabled = true;
                    comboBoxParameter2.Enabled = false;
                    labelParameter1.Text = "Assigned Value";
                    break;
                case "Set Value to field:":
                    comboBoxParameter1.Enabled = true;
                    labelParameter1.Text = "Assigned field";
                    comboBoxParameter1.Items.AddRange(fieldNames.ToArray());
                    break;
                case "Set Other Field to:":
                    comboBoxParameter1.Enabled = true;
                    comboBoxParameter2.Enabled = true;
                    labelParameter1.Text = "Field to Set";
                    labelParameter2.Text = "Value";
                    comboBoxParameter1.Items.AddRange(fieldNames.ToArray());
                    break;
                case "Combine fields with space":
                    comboBoxParameter1.Enabled = true;
                    comboBoxParameter2.Enabled = true;
                    labelParameter1.Text = "Combined field 1";
                    labelParameter2.Text = "Combined field 2";
                    comboBoxParameter1.Items.AddRange(fieldNames.ToArray());
                    comboBoxParameter2.Items.AddRange(fieldNames.ToArray());
                    break;

                case "Combine fields no space":
                    comboBoxParameter1.Enabled = true;
                    comboBoxParameter2.Enabled = true;
                    labelParameter1.Text = "Combined field 1";
                    labelParameter2.Text = "Combined field 2";
                    comboBoxParameter1.Items.AddRange(fieldNames.ToArray());
                    comboBoxParameter2.Items.AddRange(fieldNames.ToArray());
                    break;
                case "Append field with text":
                    comboBoxParameter1.Enabled = true;
                    labelParameter1.Text = "Text to append";
                    break;
                case "Prepend field with text":
                    comboBoxParameter1.Enabled = true;
                    labelParameter1.Text = "Text to prepend";
                    break;
                case "Lookup Code from DB":
                    comboBoxParameter1.Enabled = true;
                    comboBoxParameter2.Enabled = true;
                    comboBoxParameter3.Enabled = true;
                    labelParameter1.Text = "Lookup Table";
                    labelParameter2.Text = "Lookup Field";
                    labelParameter3.Text = "Result Field";
                    break;
                case "Replace text":
                    comboBoxParameter1.Enabled = true;
                    comboBoxParameter2.Enabled = true;
                    labelParameter1.Text = "Search Text";
                    labelParameter2.Text = "Replacement Text";
                    break;
                default:           
                    comboBoxParameter1.Enabled = false;
                    comboBoxParameter2.Enabled = false;
                    comboBoxParameter3.Enabled = false;
                    labelParameter1.Text = "Parameter 1";
                    labelParameter2.Text = "Parameter 2";
                    labelParameter3.Text = "Parameter 3";
                    break;
            }


        }

        private void dataGridViewRules_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            //sequence rules after deletion
            sequenceRules();

        }

        private void buttonEditRule_Click(object sender, EventArgs e)
        
        {
            numericUpDownSequence.Value = decimal.Parse(dataGridViewRules.CurrentRow.Cells["Sequence"].Value.ToString());
            comboBoxField.SelectedIndex = findIndexOfText(comboBoxField, dataGridViewRules.CurrentRow.Cells["Field"].Value.ToString());
            comboBoxOperator.SelectedIndex = findIndexOfText(comboBoxOperator, dataGridViewRules.CurrentRow.Cells["Operator"].Value.ToString());
            if (comboBoxValue.Items.Count > 0)
                comboBoxValue.SelectedIndex = findIndexOfText(comboBoxValue, dataGridViewRules.CurrentRow.Cells["Value"].Value.ToString());
            else
                comboBoxValue.Text = dataGridViewRules.CurrentRow.Cells["Value"].Value.ToString();
            comboBoxAction.SelectedIndex = findIndexOfText(comboBoxAction, dataGridViewRules.CurrentRow.Cells["Action"].Value.ToString());
            if (comboBoxParameter1.Items.Count > 0)
                comboBoxParameter1.SelectedIndex = findIndexOfText(comboBoxParameter1, dataGridViewRules.CurrentRow.Cells["Parameter1"].Value.ToString());
            else
                comboBoxParameter1.Text = dataGridViewRules.CurrentRow.Cells["Parameter1"].Value.ToString();
            if (comboBoxParameter2.Items.Count > 0)
                comboBoxParameter2.SelectedIndex = findIndexOfText(comboBoxParameter2, dataGridViewRules.CurrentRow.Cells["Parameter2"].Value.ToString());
            else
                comboBoxParameter2.Text = dataGridViewRules.CurrentRow.Cells["Parameter2"].Value.ToString();
            if (comboBoxParameter3.Items.Count > 0)
                comboBoxParameter3.SelectedIndex = findIndexOfText(comboBoxParameter3, dataGridViewRules.CurrentRow.Cells["Parameter3"].Value.ToString());
            else
                comboBoxParameter3.Text = dataGridViewRules.CurrentRow.Cells["Parameter3"].Value.ToString();

            dataGridViewRules.Rows.Remove(dataGridViewRules.CurrentRow);

        }

        private int findIndexOfText(ComboBox cb, string text)
        {
            int foundIndex = -1;
            
            for (int i = 0; i < cb.Items.Count; i++)
			{
                string textFromList = cb.Items[i].ToString();
                int startIndex = textFromList.IndexOf(" (");
                if (startIndex > 0)
                    textFromList = textFromList.Remove(textFromList.IndexOf(" ("));
			    if (text == textFromList)
                    foundIndex = i;
			}

            return foundIndex;
        }

        private void sequenceRules()
        {

            for (int i = 0; i < dataGridViewRules.Rows.Count; i++)
            {
                dataGridViewRules.Rows[i].Cells["Sequence"].Value = double.Parse(i.ToString()) + 1;
            }

            nextSequence = dataTable.Rows.Count + 1;
            numericUpDownSequence.Value = nextSequence;
        }

    }
}

