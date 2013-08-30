using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.IO;

namespace Config
{
    public partial class FormSettings : Form
    {
        public string xmlFileToProcess { get; set; }

        public FormSettings()
        {
            InitializeComponent();
        }

        private void FormSettings_Load(object sender, EventArgs e)
        {
            
            if (!File.Exists(xmlFileToProcess))
            {
                MessageBox.Show(string.Format("The requested file does not exist: {0}", xmlFileToProcess));
                Close();
            }
            else
            {
                XElement xml = XElement.Load(xmlFileToProcess);

                DataSet dataSet = new DataSet();
                dataSet.Tables.Add("Settings");
                dataSet.Tables["Settings"].Columns.Add("Name");
                dataSet.Tables["Settings"].Columns.Add("Value");

                foreach (XElement element in xml.Elements())
                {
                    DataRow newRow = dataSet.Tables["Settings"].NewRow();
                    newRow["Name"] = element.Name;
                    newRow["Value"] = element.Value;
                    dataSet.Tables["Settings"].Rows.Add(newRow);
                }

                dataGridViewConfig.DataSource = dataSet.Tables["Settings"];
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Save Current Configuration?", "Save?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                //Directory.Delete("config.xml");
                XElement xml = XElement.Load(xmlFileToProcess);
                string startElement = xml.Name.ToString();
                
                XmlWriterSettings xmlSettings = new XmlWriterSettings();
                xmlSettings.Indent = true;
                xmlSettings.NewLineOnAttributes = true;
                xmlSettings.OmitXmlDeclaration = true;
                xmlSettings.Encoding = Encoding.Default;

                using (XmlWriter newConfig = XmlWriter.Create(xmlFileToProcess, xmlSettings))
                {
                    newConfig.WriteStartDocument();
                    newConfig.WriteStartElement(startElement);
                            
                    for (int rows = 0; rows < dataGridViewConfig.Rows.Count - 1; rows++)
                    {
                        for (int col = 0; col < dataGridViewConfig.Rows[rows].Cells.Count; col+=2)
                        {
                            newConfig.WriteStartElement(dataGridViewConfig.Rows[rows].Cells[col].Value.ToString());
                            newConfig.WriteValue(dataGridViewConfig.Rows[rows].Cells[col+1].Value.ToString());
                            newConfig.WriteEndElement();
                                    
                        }
                    }
                    newConfig.WriteEndElement();
                    newConfig.WriteEndDocument();
                }
                Close();
            }
        }

    }
}
