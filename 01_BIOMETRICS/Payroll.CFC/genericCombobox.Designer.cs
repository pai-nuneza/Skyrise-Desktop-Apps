namespace Payroll.CFC
{
    partial class genericCombobox
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtReadOnly = new System.Windows.Forms.TextBox();
            this.Combobox = new Payroll.CFC.genericDBCombobox();
            this.SuspendLayout();
            // 
            // txtReadOnly
            // 
            this.txtReadOnly.Location = new System.Drawing.Point(0, 0);
            this.txtReadOnly.Name = "txtReadOnly";
            this.txtReadOnly.ReadOnly = true;
            this.txtReadOnly.Size = new System.Drawing.Size(117, 20);
            this.txtReadOnly.TabIndex = 1;
            // 
            // Combobox
            // 
            this.Combobox.DBType = CommonLibrary.CommonEnum.GenericCBType.CURRENCY;
            this.Combobox.FormattingEnabled = true;
            this.Combobox.Location = new System.Drawing.Point(0, 0);
            this.Combobox.Name = "Combobox";
            this.Combobox.Size = new System.Drawing.Size(117, 21);
            this.Combobox.TabIndex = 0;
            this.Combobox.ValueRequired = false;
            // 
            // genericCombobox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtReadOnly);
            this.Controls.Add(this.Combobox);
            this.Name = "genericCombobox";
            this.Size = new System.Drawing.Size(118, 21);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public genericDBCombobox Combobox;
        
        private System.Windows.Forms.TextBox txtReadOnly;
    }
}
