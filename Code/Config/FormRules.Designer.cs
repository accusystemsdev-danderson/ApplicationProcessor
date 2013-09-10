namespace Config
{
    partial class FormRules
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dataGridViewRules = new System.Windows.Forms.DataGridView();
            this.labelField = new System.Windows.Forms.Label();
            this.labelOperator = new System.Windows.Forms.Label();
            this.labelAction = new System.Windows.Forms.Label();
            this.labelParameter1 = new System.Windows.Forms.Label();
            this.comboBoxField = new System.Windows.Forms.ComboBox();
            this.comboBoxOperator = new System.Windows.Forms.ComboBox();
            this.comboBoxAction = new System.Windows.Forms.ComboBox();
            this.comboBoxParameter1 = new System.Windows.Forms.ComboBox();
            this.comboBoxParameter2 = new System.Windows.Forms.ComboBox();
            this.labelParameter2 = new System.Windows.Forms.Label();
            this.labelValue = new System.Windows.Forms.Label();
            this.comboBoxValue = new System.Windows.Forms.ComboBox();
            this.buttonAddRule = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelSequence = new System.Windows.Forms.Label();
            this.numericUpDownSequence = new System.Windows.Forms.NumericUpDown();
            this.buttonEditRule = new System.Windows.Forms.Button();
            this.labelParameter3 = new System.Windows.Forms.Label();
            this.comboBoxParameter3 = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRules)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSequence)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewRules
            // 
            this.dataGridViewRules.AllowUserToAddRows = false;
            this.dataGridViewRules.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewRules.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewRules.Location = new System.Drawing.Point(21, 12);
            this.dataGridViewRules.Name = "dataGridViewRules";
            this.dataGridViewRules.ReadOnly = true;
            this.dataGridViewRules.Size = new System.Drawing.Size(867, 206);
            this.dataGridViewRules.TabIndex = 0;
            this.dataGridViewRules.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.dataGridViewRules_UserDeletedRow);
            // 
            // labelField
            // 
            this.labelField.AutoSize = true;
            this.labelField.Location = new System.Drawing.Point(99, 231);
            this.labelField.Name = "labelField";
            this.labelField.Size = new System.Drawing.Size(29, 13);
            this.labelField.TabIndex = 1;
            this.labelField.Text = "Field";
            // 
            // labelOperator
            // 
            this.labelOperator.AutoSize = true;
            this.labelOperator.Location = new System.Drawing.Point(255, 231);
            this.labelOperator.Name = "labelOperator";
            this.labelOperator.Size = new System.Drawing.Size(48, 13);
            this.labelOperator.TabIndex = 2;
            this.labelOperator.Text = "Operator";
            // 
            // labelAction
            // 
            this.labelAction.AutoSize = true;
            this.labelAction.Location = new System.Drawing.Point(18, 287);
            this.labelAction.Name = "labelAction";
            this.labelAction.Size = new System.Drawing.Size(37, 13);
            this.labelAction.TabIndex = 3;
            this.labelAction.Text = "Action";
            // 
            // labelParameter1
            // 
            this.labelParameter1.AutoSize = true;
            this.labelParameter1.Location = new System.Drawing.Point(18, 338);
            this.labelParameter1.Name = "labelParameter1";
            this.labelParameter1.Size = new System.Drawing.Size(64, 13);
            this.labelParameter1.TabIndex = 4;
            this.labelParameter1.Text = "Parameter 1";
            // 
            // comboBoxField
            // 
            this.comboBoxField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxField.FormattingEnabled = true;
            this.comboBoxField.Location = new System.Drawing.Point(102, 247);
            this.comboBoxField.Name = "comboBoxField";
            this.comboBoxField.Size = new System.Drawing.Size(150, 21);
            this.comboBoxField.TabIndex = 5;
            // 
            // comboBoxOperator
            // 
            this.comboBoxOperator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxOperator.FormattingEnabled = true;
            this.comboBoxOperator.Location = new System.Drawing.Point(258, 247);
            this.comboBoxOperator.Name = "comboBoxOperator";
            this.comboBoxOperator.Size = new System.Drawing.Size(97, 21);
            this.comboBoxOperator.TabIndex = 6;
            this.comboBoxOperator.SelectedIndexChanged += new System.EventHandler(this.comboBoxOperator_SelectedIndexChanged);
            // 
            // comboBoxAction
            // 
            this.comboBoxAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxAction.FormattingEnabled = true;
            this.comboBoxAction.Location = new System.Drawing.Point(21, 303);
            this.comboBoxAction.Name = "comboBoxAction";
            this.comboBoxAction.Size = new System.Drawing.Size(121, 21);
            this.comboBoxAction.TabIndex = 7;
            this.comboBoxAction.SelectedIndexChanged += new System.EventHandler(this.comboBoxAction_SelectedIndexChanged);
            // 
            // comboBoxParameter1
            // 
            this.comboBoxParameter1.Enabled = false;
            this.comboBoxParameter1.FormattingEnabled = true;
            this.comboBoxParameter1.Location = new System.Drawing.Point(21, 354);
            this.comboBoxParameter1.Name = "comboBoxParameter1";
            this.comboBoxParameter1.Size = new System.Drawing.Size(121, 21);
            this.comboBoxParameter1.TabIndex = 8;
            // 
            // comboBoxParameter2
            // 
            this.comboBoxParameter2.Enabled = false;
            this.comboBoxParameter2.FormattingEnabled = true;
            this.comboBoxParameter2.Location = new System.Drawing.Point(148, 354);
            this.comboBoxParameter2.Name = "comboBoxParameter2";
            this.comboBoxParameter2.Size = new System.Drawing.Size(121, 21);
            this.comboBoxParameter2.TabIndex = 9;
            // 
            // labelParameter2
            // 
            this.labelParameter2.AutoSize = true;
            this.labelParameter2.Location = new System.Drawing.Point(145, 338);
            this.labelParameter2.Name = "labelParameter2";
            this.labelParameter2.Size = new System.Drawing.Size(61, 13);
            this.labelParameter2.TabIndex = 10;
            this.labelParameter2.Text = "Parameter2";
            // 
            // labelValue
            // 
            this.labelValue.AutoSize = true;
            this.labelValue.Location = new System.Drawing.Point(361, 231);
            this.labelValue.Name = "labelValue";
            this.labelValue.Size = new System.Drawing.Size(34, 13);
            this.labelValue.TabIndex = 11;
            this.labelValue.Text = "Value";
            // 
            // comboBoxValue
            // 
            this.comboBoxValue.Enabled = false;
            this.comboBoxValue.FormattingEnabled = true;
            this.comboBoxValue.Location = new System.Drawing.Point(361, 247);
            this.comboBoxValue.Name = "comboBoxValue";
            this.comboBoxValue.Size = new System.Drawing.Size(121, 21);
            this.comboBoxValue.TabIndex = 12;
            // 
            // buttonAddRule
            // 
            this.buttonAddRule.Location = new System.Drawing.Point(21, 383);
            this.buttonAddRule.Name = "buttonAddRule";
            this.buttonAddRule.Size = new System.Drawing.Size(75, 23);
            this.buttonAddRule.TabIndex = 13;
            this.buttonAddRule.Text = "AddRule";
            this.buttonAddRule.UseVisualStyleBackColor = true;
            this.buttonAddRule.Click += new System.EventHandler(this.buttonAddRule_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(731, 383);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 14;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(812, 383);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 15;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // labelSequence
            // 
            this.labelSequence.AutoSize = true;
            this.labelSequence.Location = new System.Drawing.Point(21, 232);
            this.labelSequence.Name = "labelSequence";
            this.labelSequence.Size = new System.Drawing.Size(56, 13);
            this.labelSequence.TabIndex = 17;
            this.labelSequence.Text = "Sequence";
            // 
            // numericUpDownSequence
            // 
            this.numericUpDownSequence.Location = new System.Drawing.Point(24, 248);
            this.numericUpDownSequence.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.numericUpDownSequence.Name = "numericUpDownSequence";
            this.numericUpDownSequence.Size = new System.Drawing.Size(72, 20);
            this.numericUpDownSequence.TabIndex = 18;
            // 
            // buttonEditRule
            // 
            this.buttonEditRule.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonEditRule.Location = new System.Drawing.Point(894, 26);
            this.buttonEditRule.Name = "buttonEditRule";
            this.buttonEditRule.Size = new System.Drawing.Size(75, 23);
            this.buttonEditRule.TabIndex = 19;
            this.buttonEditRule.Text = "Edit Rule";
            this.buttonEditRule.UseVisualStyleBackColor = true;
            this.buttonEditRule.Click += new System.EventHandler(this.buttonEditRule_Click);
            // 
            // labelParameter3
            // 
            this.labelParameter3.AutoSize = true;
            this.labelParameter3.Location = new System.Drawing.Point(272, 338);
            this.labelParameter3.Name = "labelParameter3";
            this.labelParameter3.Size = new System.Drawing.Size(61, 13);
            this.labelParameter3.TabIndex = 21;
            this.labelParameter3.Text = "Parameter3";
            // 
            // comboBoxParameter3
            // 
            this.comboBoxParameter3.Enabled = false;
            this.comboBoxParameter3.FormattingEnabled = true;
            this.comboBoxParameter3.Location = new System.Drawing.Point(275, 354);
            this.comboBoxParameter3.Name = "comboBoxParameter3";
            this.comboBoxParameter3.Size = new System.Drawing.Size(121, 21);
            this.comboBoxParameter3.TabIndex = 20;
            // 
            // FormRules
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(972, 418);
            this.Controls.Add(this.labelParameter3);
            this.Controls.Add(this.comboBoxParameter3);
            this.Controls.Add(this.buttonEditRule);
            this.Controls.Add(this.numericUpDownSequence);
            this.Controls.Add(this.labelSequence);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.buttonAddRule);
            this.Controls.Add(this.comboBoxValue);
            this.Controls.Add(this.labelValue);
            this.Controls.Add(this.labelParameter2);
            this.Controls.Add(this.comboBoxParameter2);
            this.Controls.Add(this.comboBoxParameter1);
            this.Controls.Add(this.comboBoxAction);
            this.Controls.Add(this.comboBoxOperator);
            this.Controls.Add(this.comboBoxField);
            this.Controls.Add(this.labelParameter1);
            this.Controls.Add(this.labelAction);
            this.Controls.Add(this.labelOperator);
            this.Controls.Add(this.labelField);
            this.Controls.Add(this.dataGridViewRules);
            this.Name = "FormRules";
            this.Text = "Rules";
            this.Load += new System.EventHandler(this.FormRules_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRules)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSequence)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewRules;
        private System.Windows.Forms.Label labelField;
        private System.Windows.Forms.Label labelOperator;
        private System.Windows.Forms.Label labelAction;
        private System.Windows.Forms.Label labelParameter1;
        private System.Windows.Forms.ComboBox comboBoxField;
        private System.Windows.Forms.ComboBox comboBoxOperator;
        private System.Windows.Forms.ComboBox comboBoxAction;
        private System.Windows.Forms.ComboBox comboBoxParameter1;
        private System.Windows.Forms.ComboBox comboBoxParameter2;
        private System.Windows.Forms.Label labelParameter2;
        private System.Windows.Forms.Label labelValue;
        private System.Windows.Forms.ComboBox comboBoxValue;
        private System.Windows.Forms.Button buttonAddRule;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label labelSequence;
        private System.Windows.Forms.NumericUpDown numericUpDownSequence;
        private System.Windows.Forms.Button buttonEditRule;
        private System.Windows.Forms.Label labelParameter3;
        private System.Windows.Forms.ComboBox comboBoxParameter3;
    }
}