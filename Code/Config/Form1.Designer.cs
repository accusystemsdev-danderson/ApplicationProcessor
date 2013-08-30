namespace Config
{
    partial class FormMain
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
            this.buttonSettings = new System.Windows.Forms.Button();
            this.buttonFieldMap = new System.Windows.Forms.Button();
            this.buttonFinished = new System.Windows.Forms.Button();
            this.buttonRules = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonSettings
            // 
            this.buttonSettings.Location = new System.Drawing.Point(38, 29);
            this.buttonSettings.Name = "buttonSettings";
            this.buttonSettings.Size = new System.Drawing.Size(75, 23);
            this.buttonSettings.TabIndex = 0;
            this.buttonSettings.Text = "Settings";
            this.buttonSettings.UseVisualStyleBackColor = true;
            this.buttonSettings.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonFieldMap
            // 
            this.buttonFieldMap.Location = new System.Drawing.Point(139, 29);
            this.buttonFieldMap.Name = "buttonFieldMap";
            this.buttonFieldMap.Size = new System.Drawing.Size(75, 23);
            this.buttonFieldMap.TabIndex = 1;
            this.buttonFieldMap.Text = "Field Map";
            this.buttonFieldMap.UseVisualStyleBackColor = true;
            this.buttonFieldMap.Click += new System.EventHandler(this.buttonFieldMap_Click);
            // 
            // buttonFinished
            // 
            this.buttonFinished.Location = new System.Drawing.Point(139, 85);
            this.buttonFinished.Name = "buttonFinished";
            this.buttonFinished.Size = new System.Drawing.Size(75, 23);
            this.buttonFinished.TabIndex = 2;
            this.buttonFinished.Text = "Finished";
            this.buttonFinished.UseVisualStyleBackColor = true;
            this.buttonFinished.Click += new System.EventHandler(this.buttonFinished_Click);
            // 
            // buttonRules
            // 
            this.buttonRules.Location = new System.Drawing.Point(38, 85);
            this.buttonRules.Name = "buttonRules";
            this.buttonRules.Size = new System.Drawing.Size(75, 23);
            this.buttonRules.TabIndex = 3;
            this.buttonRules.Text = "Rules";
            this.buttonRules.UseVisualStyleBackColor = true;
            this.buttonRules.Click += new System.EventHandler(this.buttonRules_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(259, 143);
            this.Controls.Add(this.buttonRules);
            this.Controls.Add(this.buttonFinished);
            this.Controls.Add(this.buttonFieldMap);
            this.Controls.Add(this.buttonSettings);
            this.Name = "FormMain";
            this.Text = "Config";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonSettings;
        private System.Windows.Forms.Button buttonFieldMap;
        private System.Windows.Forms.Button buttonFinished;
        private System.Windows.Forms.Button buttonRules;
    }
}

