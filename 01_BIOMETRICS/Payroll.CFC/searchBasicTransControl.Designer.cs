namespace Payroll.CFC
{
    partial class searchBasicTransControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(searchBasicTransControl));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.gridLookup = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnPopUpSearch = new System.Windows.Forms.Button();
            this.btnSearchClear = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtLookupCode = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookup)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.gridLookup);
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(385, 579);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Search Control";
            // 
            // gridLookup
            // 
            this.gridLookup.AllowUserToAddRows = false;
            this.gridLookup.AllowUserToDeleteRows = false;
            this.gridLookup.AllowUserToResizeRows = false;
            this.gridLookup.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridLookup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridLookup.Location = new System.Drawing.Point(3, 52);
            this.gridLookup.MultiSelect = false;
            this.gridLookup.Name = "gridLookup";
            this.gridLookup.ReadOnly = true;
            this.gridLookup.RowHeadersVisible = false;
            this.gridLookup.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridLookup.Size = new System.Drawing.Size(379, 524);
            this.gridLookup.TabIndex = 8;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnPopUpSearch);
            this.panel1.Controls.Add(this.btnSearchClear);
            this.panel1.Controls.Add(this.btnSearch);
            this.panel1.Controls.Add(this.txtLookupCode);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(3, 16);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(379, 36);
            this.panel1.TabIndex = 7;
            // 
            // btnPopUpSearch
            // 
            this.btnPopUpSearch.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnPopUpSearch.Location = new System.Drawing.Point(262, 3);
            this.btnPopUpSearch.Name = "btnPopUpSearch";
            this.btnPopUpSearch.Size = new System.Drawing.Size(54, 27);
            this.btnPopUpSearch.TabIndex = 10;
            this.btnPopUpSearch.Text = "&Search";
            this.btnPopUpSearch.UseVisualStyleBackColor = true;
            this.btnPopUpSearch.Visible = false;
            // 
            // btnSearchClear
            // 
            this.btnSearchClear.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSearchClear.Location = new System.Drawing.Point(322, 3);
            this.btnSearchClear.Name = "btnSearchClear";
            this.btnSearchClear.Size = new System.Drawing.Size(54, 27);
            this.btnSearchClear.TabIndex = 9;
            this.btnSearchClear.Text = "&Reset";
            this.btnSearchClear.UseVisualStyleBackColor = true;
            this.btnSearchClear.Visible = false;
            this.btnSearchClear.Click += new System.EventHandler(this.btnSearchClear_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSearch.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSearch.BackgroundImage")));
            this.btnSearch.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnSearch.Location = new System.Drawing.Point(228, 3);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(28, 27);
            this.btnSearch.TabIndex = 8;
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtLookupCode
            // 
            this.txtLookupCode.Location = new System.Drawing.Point(30, 7);
            this.txtLookupCode.Name = "txtLookupCode";
            this.txtLookupCode.Size = new System.Drawing.Size(192, 20);
            this.txtLookupCode.TabIndex = 7;
            this.txtLookupCode.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtLookupCode_KeyUp_1);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Find:";
            // 
            // searchBasicTransControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "searchBasicTransControl";
            this.Size = new System.Drawing.Size(385, 579);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridLookup)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        protected System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel1;
        protected System.Windows.Forms.Button btnPopUpSearch;
        protected System.Windows.Forms.Button btnSearchClear;
        protected System.Windows.Forms.Button btnSearch;
        protected System.Windows.Forms.TextBox txtLookupCode;
        protected System.Windows.Forms.Label label1;
        protected System.Windows.Forms.DataGridView gridLookup;

    }
}
