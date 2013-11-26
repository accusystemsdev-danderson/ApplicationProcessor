using System;
using System.Windows.Forms;

namespace Config
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void buttonSettings_Click(object sender, EventArgs e)
        {
            FormSettings formSettings = new FormSettings();
            formSettings.xmlFileToProcess = "config.xml";
            formSettings.Show();
        }

        private void buttonFinished_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void buttonFieldMap_Click(object sender, EventArgs e)
        {
            FormSettings formMappings = new FormSettings();
            formMappings.xmlFileToProcess = "fieldmap.xml";
            formMappings.Show();
        }

        private void buttonRules_Click(object sender, EventArgs e)
        {
            FormRules formRules = new FormRules();
            formRules.Show();
        }

    }
}
