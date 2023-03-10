namespace CommonLibrary
{
    partial class frmGenericPrompt
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmGenericPrompt));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnNo = new System.Windows.Forms.Button();
            this.btnYes = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.txtSuggestion = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.pnlMessage = new System.Windows.Forms.Panel();
            this.lblDetails = new System.Windows.Forms.LinkLabel();
            this.flpErrorCode = new System.Windows.Forms.FlowLayoutPanel();
            this.lblMessageCode = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtMessage = new System.Windows.Forms.RichTextBox();
            this.pbIcon = new System.Windows.Forms.PictureBox();
            this.grpDetails = new System.Windows.Forms.Panel();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.flowLayoutPanel1.SuspendLayout();
            this.pnlMessage.SuspendLayout();
            this.flpErrorCode.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).BeginInit();
            this.grpDetails.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.White;
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.ForeColor = System.Drawing.Color.Black;
            this.btnCancel.Location = new System.Drawing.Point(386, 3);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 3, 6, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 25);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            this.btnCancel.MouseEnter += new System.EventHandler(this.btn_MouseEnter);
            this.btnCancel.MouseLeave += new System.EventHandler(this.btn_MouseLeave);
            // 
            // btnNo
            // 
            this.btnNo.BackColor = System.Drawing.Color.White;
            this.btnNo.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnNo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNo.ForeColor = System.Drawing.Color.Black;
            this.btnNo.Location = new System.Drawing.Point(287, 3);
            this.btnNo.Margin = new System.Windows.Forms.Padding(3, 3, 6, 3);
            this.btnNo.Name = "btnNo";
            this.btnNo.Size = new System.Drawing.Size(90, 25);
            this.btnNo.TabIndex = 1;
            this.btnNo.Text = "No";
            this.btnNo.UseVisualStyleBackColor = false;
            this.btnNo.Click += new System.EventHandler(this.btnNo_Click);
            this.btnNo.MouseEnter += new System.EventHandler(this.btn_MouseEnter);
            this.btnNo.MouseLeave += new System.EventHandler(this.btn_MouseLeave);
            // 
            // btnYes
            // 
            this.btnYes.BackColor = System.Drawing.Color.White;
            this.btnYes.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnYes.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnYes.ForeColor = System.Drawing.Color.Black;
            this.btnYes.Location = new System.Drawing.Point(188, 3);
            this.btnYes.Margin = new System.Windows.Forms.Padding(3, 3, 6, 3);
            this.btnYes.Name = "btnYes";
            this.btnYes.Size = new System.Drawing.Size(90, 25);
            this.btnYes.TabIndex = 0;
            this.btnYes.Text = "Yes";
            this.btnYes.UseVisualStyleBackColor = false;
            this.btnYes.Click += new System.EventHandler(this.btnYes_Click);
            this.btnYes.MouseEnter += new System.EventHandler(this.btn_MouseEnter);
            this.btnYes.MouseLeave += new System.EventHandler(this.btn_MouseLeave);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.Controls.Add(this.btnCancel);
            this.flowLayoutPanel1.Controls.Add(this.btnNo);
            this.flowLayoutPanel1.Controls.Add(this.btnYes);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(21, 8);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(482, 34);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // txtSuggestion
            // 
            this.txtSuggestion.BackColor = System.Drawing.Color.White;
            this.txtSuggestion.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtSuggestion.Font = new System.Drawing.Font("Arial Unicode MS", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSuggestion.Location = new System.Drawing.Point(112, 11);
            this.txtSuggestion.Multiline = true;
            this.txtSuggestion.Name = "txtSuggestion";
            this.txtSuggestion.ReadOnly = true;
            this.txtSuggestion.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtSuggestion.Size = new System.Drawing.Size(368, 80);
            this.txtSuggestion.TabIndex = 4;
            this.txtSuggestion.TabStop = false;
            this.txtSuggestion.Text = resources.GetString("txtSuggestion.Text");
            this.txtSuggestion.TextChanged += new System.EventHandler(this.txtSuggestion_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(96, 14);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(10, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = ":";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial Unicode MS", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(13, 11);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 18);
            this.label3.TabIndex = 0;
            this.label3.Text = "Suggestion";
            // 
            // pnlMessage
            // 
            this.pnlMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlMessage.BackColor = System.Drawing.Color.White;
            this.pnlMessage.Controls.Add(this.lblDetails);
            this.pnlMessage.Controls.Add(this.flpErrorCode);
            this.pnlMessage.Controls.Add(this.txtMessage);
            this.pnlMessage.Controls.Add(this.pbIcon);
            this.pnlMessage.Location = new System.Drawing.Point(2, 2);
            this.pnlMessage.Name = "pnlMessage";
            this.pnlMessage.Size = new System.Drawing.Size(508, 126);
            this.pnlMessage.TabIndex = 4;
            // 
            // lblDetails
            // 
            this.lblDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblDetails.AutoSize = true;
            this.lblDetails.Location = new System.Drawing.Point(13, 97);
            this.lblDetails.Name = "lblDetails";
            this.lblDetails.Size = new System.Drawing.Size(81, 13);
            this.lblDetails.TabIndex = 3;
            this.lblDetails.TabStop = true;
            this.lblDetails.Text = "More Details >>";
            this.lblDetails.Visible = false;
            this.lblDetails.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // flpErrorCode
            // 
            this.flpErrorCode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpErrorCode.Controls.Add(this.lblMessageCode);
            this.flpErrorCode.Controls.Add(this.label2);
            this.flpErrorCode.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flpErrorCode.Location = new System.Drawing.Point(193, 3);
            this.flpErrorCode.Name = "flpErrorCode";
            this.flpErrorCode.Size = new System.Drawing.Size(306, 18);
            this.flpErrorCode.TabIndex = 5;
            this.flpErrorCode.Visible = false;
            // 
            // lblMessageCode
            // 
            this.lblMessageCode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMessageCode.AutoSize = true;
            this.lblMessageCode.Font = new System.Drawing.Font("Arial Unicode MS", 9.75F);
            this.lblMessageCode.Location = new System.Drawing.Point(267, 0);
            this.lblMessageCode.Name = "lblMessageCode";
            this.lblMessageCode.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblMessageCode.Size = new System.Drawing.Size(36, 18);
            this.lblMessageCode.TabIndex = 2;
            this.lblMessageCode.Text = "1521";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial Unicode MS", 9.75F);
            this.label2.Location = new System.Drawing.Point(183, 0);
            this.label2.Name = "label2";
            this.label2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label2.Size = new System.Drawing.Size(78, 18);
            this.label2.TabIndex = 2;
            this.label2.Text = "Error Code :";
            // 
            // txtMessage
            // 
            this.txtMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMessage.BackColor = System.Drawing.Color.White;
            this.txtMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtMessage.Font = new System.Drawing.Font("Arial Unicode MS", 9.75F);
            this.txtMessage.Location = new System.Drawing.Point(112, 23);
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.ReadOnly = true;
            this.txtMessage.Size = new System.Drawing.Size(368, 100);
            this.txtMessage.TabIndex = 4;
            this.txtMessage.TabStop = false;
            this.txtMessage.Text = resources.GetString("txtMessage.Text");
            this.txtMessage.TextChanged += new System.EventHandler(this.txt_TextChanged);
            // 
            // pbIcon
            // 
            this.pbIcon.ErrorImage = global::CommonLibrary.Properties.Resources.Error;
            this.pbIcon.Location = new System.Drawing.Point(25, 31);
            this.pbIcon.Name = "pbIcon";
            this.pbIcon.Size = new System.Drawing.Size(60, 60);
            this.pbIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbIcon.TabIndex = 0;
            this.pbIcon.TabStop = false;
            // 
            // grpDetails
            // 
            this.grpDetails.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpDetails.BackColor = System.Drawing.Color.White;
            this.grpDetails.Controls.Add(this.txtSuggestion);
            this.grpDetails.Controls.Add(this.label6);
            this.grpDetails.Controls.Add(this.label3);
            this.grpDetails.Location = new System.Drawing.Point(2, 129);
            this.grpDetails.Name = "grpDetails";
            this.grpDetails.Size = new System.Drawing.Size(508, 100);
            this.grpDetails.TabIndex = 5;
            // 
            // pnlButtons
            // 
            this.pnlButtons.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlButtons.BackColor = System.Drawing.Color.Gainsboro;
            this.pnlButtons.Controls.Add(this.flowLayoutPanel1);
            this.pnlButtons.Location = new System.Drawing.Point(0, 229);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(511, 44);
            this.pnlButtons.TabIndex = 6;
            // 
            // frmGenericPrompt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(511, 273);
            this.Controls.Add(this.pnlButtons);
            this.Controls.Add(this.pnlMessage);
            this.Controls.Add(this.grpDetails);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmGenericPrompt";
            this.Text = "Minerva Payroll";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.frmGenericPrompt_Load);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.pnlMessage.ResumeLayout(false);
            this.pnlMessage.PerformLayout();
            this.flpErrorCode.ResumeLayout(false);
            this.flpErrorCode.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).EndInit();
            this.grpDetails.ResumeLayout(false);
            this.grpDetails.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnNo;
        private System.Windows.Forms.Button btnYes;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtSuggestion;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel pnlMessage;
        private System.Windows.Forms.FlowLayoutPanel flpErrorCode;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox txtMessage;
        private System.Windows.Forms.PictureBox pbIcon;
        private System.Windows.Forms.LinkLabel lblDetails;
        private System.Windows.Forms.Panel grpDetails;
        private System.Windows.Forms.Label lblMessageCode;
        private System.Windows.Forms.Panel pnlButtons;
    }
}