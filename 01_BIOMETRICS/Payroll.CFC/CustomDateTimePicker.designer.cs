namespace Payroll.CFC
{
    partial class CustomDateTimePicker
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
            this.components = new System.ComponentModel.Container();
            this.dtp = new System.Windows.Forms.DateTimePicker();
            this.txtDateTime = new System.Windows.Forms.TextBox();
            this.Tip = new System.Windows.Forms.ToolTip(this.components);
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // dtp
            // 
            this.dtp.Checked = false;
            this.dtp.CustomFormat = "  ";
            this.dtp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dtp.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtp.Location = new System.Drawing.Point(0, 0);
            this.dtp.Name = "dtp";
            this.dtp.Size = new System.Drawing.Size(100, 20);
            this.dtp.TabIndex = 2;
            this.dtp.TabStop = false;
            this.dtp.LocationChanged += new System.EventHandler(this.dtp_LocationChanged);
            this.dtp.ValueChanged += new System.EventHandler(this.dtp_ValueChanged);
            this.dtp.Resize += new System.EventHandler(this.dtp_Resize);
            // 
            // txtDateTime
            // 
            this.txtDateTime.BackColor = System.Drawing.SystemColors.Window;
            this.txtDateTime.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.txtDateTime.Location = new System.Drawing.Point(2, 0);
            this.txtDateTime.Margin = new System.Windows.Forms.Padding(1);
            this.txtDateTime.Name = "txtDateTime";
            this.txtDateTime.Size = new System.Drawing.Size(70, 20);
            this.txtDateTime.TabIndex = 0;
            this.txtDateTime.WordWrap = false;
            this.txtDateTime.TextChanged += new System.EventHandler(this.txtDateTime_TextChanged);
            this.txtDateTime.Leave += new System.EventHandler(this.txtDateTime_Leave);
            this.txtDateTime.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtDateTime_KeyPress);
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // CustomDateTimePicker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtDateTime);
            this.Controls.Add(this.dtp);
            this.Name = "CustomDateTimePicker";
            this.Size = new System.Drawing.Size(100, 20);
            this.Load += new System.EventHandler(this.ctrlDateTimePicker_Load);
            this.Enter += new System.EventHandler(this.CustomDateTimePicker_Enter);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dtp;
        private System.Windows.Forms.ToolTip Tip;
        private System.Windows.Forms.ErrorProvider errorProvider;
        internal System.Windows.Forms.TextBox txtDateTime;
    }
}
