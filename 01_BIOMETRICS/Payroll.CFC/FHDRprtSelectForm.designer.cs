namespace Payroll.CFC
{
    partial class FHDRprtSelectForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FHDRprtSelectForm));
            this.dgvLeft = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dgvRight = new System.Windows.Forms.DataGridView();
            this.gbSettings = new System.Windows.Forms.GroupBox();
            this.rbNoSort = new System.Windows.Forms.RadioButton();
            this.label5 = new System.Windows.Forms.Label();
            this.txtGroupby = new System.Windows.Forms.TextBox();
            this.rbDescending = new System.Windows.Forms.RadioButton();
            this.rbAscending = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.btnApplyLeftAll = new System.Windows.Forms.Button();
            this.btnApplyLeft = new System.Windows.Forms.Button();
            this.btnApplyAll = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnApplyRow = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnPrint = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRight)).BeginInit();
            this.gbSettings.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvLeft
            // 
            this.dgvLeft.AllowDrop = true;
            this.dgvLeft.AllowUserToAddRows = false;
            this.dgvLeft.AllowUserToDeleteRows = false;
            this.dgvLeft.Anchor = System.Windows.Forms.AnchorStyles.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvLeft.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvLeft.ColumnHeadersHeight = 19;
            this.dgvLeft.Location = new System.Drawing.Point(6, 12);
            this.dgvLeft.MultiSelect = false;
            this.dgvLeft.Name = "dgvLeft";
            this.dgvLeft.ReadOnly = true;
            this.dgvLeft.RowHeadersWidth = 17;
            this.dgvLeft.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLeft.Size = new System.Drawing.Size(256, 308);
            this.dgvLeft.TabIndex = 0;
            this.dgvLeft.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dgvLeft_MouseDown);
            this.dgvLeft.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dgvLeft_DataBindingComplete);
            this.dgvLeft.DragEnter += new System.Windows.Forms.DragEventHandler(this.dgvLeft_DragEnter);
            this.dgvLeft.DragDrop += new System.Windows.Forms.DragEventHandler(this.dgvLeft_DragDrop);
            this.dgvLeft.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgvLeft_MouseUp);
            this.dgvLeft.SelectionChanged += new System.EventHandler(this.dgvLeft_SelectionChanged);
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 328);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 13);
            this.label2.TabIndex = 60;
            this.label2.Text = "{0 Rows}";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(325, 328);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 59;
            this.label1.Text = "{0 Rows}";
            // 
            // dgvRight
            // 
            this.dgvRight.AllowDrop = true;
            this.dgvRight.AllowUserToAddRows = false;
            this.dgvRight.AllowUserToDeleteRows = false;
            this.dgvRight.Anchor = System.Windows.Forms.AnchorStyles.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvRight.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvRight.ColumnHeadersHeight = 19;
            this.dgvRight.Location = new System.Drawing.Point(328, 9);
            this.dgvRight.MultiSelect = false;
            this.dgvRight.Name = "dgvRight";
            this.dgvRight.ReadOnly = true;
            this.dgvRight.RowHeadersWidth = 17;
            this.dgvRight.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvRight.Size = new System.Drawing.Size(256, 308);
            this.dgvRight.TabIndex = 3;
            this.dgvRight.Tag = "";
            this.dgvRight.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dgvRight_MouseDown);
            this.dgvRight.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dgvRight_DataBindingComplete);
            this.dgvRight.DragEnter += new System.Windows.Forms.DragEventHandler(this.dgvRight_DragEnter);
            this.dgvRight.DragDrop += new System.Windows.Forms.DragEventHandler(this.dgvRight_DragDrop);
            this.dgvRight.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgvRight_MouseUp);
            this.dgvRight.SelectionChanged += new System.EventHandler(this.dgvRight_SelectionChanged);
            // 
            // gbSettings
            // 
            this.gbSettings.Controls.Add(this.rbNoSort);
            this.gbSettings.Controls.Add(this.label5);
            this.gbSettings.Controls.Add(this.txtGroupby);
            this.gbSettings.Controls.Add(this.rbDescending);
            this.gbSettings.Controls.Add(this.rbAscending);
            this.gbSettings.Controls.Add(this.label4);
            this.gbSettings.Location = new System.Drawing.Point(639, 6);
            this.gbSettings.Name = "gbSettings";
            this.gbSettings.Size = new System.Drawing.Size(175, 279);
            this.gbSettings.TabIndex = 4;
            this.gbSettings.TabStop = false;
            this.gbSettings.Text = "Settings";
            // 
            // rbNoSort
            // 
            this.rbNoSort.AutoSize = true;
            this.rbNoSort.Location = new System.Drawing.Point(33, 142);
            this.rbNoSort.Name = "rbNoSort";
            this.rbNoSort.Size = new System.Drawing.Size(61, 17);
            this.rbNoSort.TabIndex = 10;
            this.rbNoSort.TabStop = true;
            this.rbNoSort.Text = "No Sort";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.label5.Location = new System.Drawing.Point(10, 258);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(156, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Note: This settings are optional.";
            // 
            // txtGroupby
            // 
            this.txtGroupby.AllowDrop = true;
            this.txtGroupby.Location = new System.Drawing.Point(24, 44);
            this.txtGroupby.MaxLength = 150;
            this.txtGroupby.Multiline = true;
            this.txtGroupby.Name = "txtGroupby";
            this.txtGroupby.Size = new System.Drawing.Size(137, 46);
            this.txtGroupby.TabIndex = 5;
            this.txtGroupby.Tag = "";
            // 
            // rbDescending
            // 
            this.rbDescending.AutoSize = true;
            this.rbDescending.Location = new System.Drawing.Point(33, 119);
            this.rbDescending.Name = "rbDescending";
            this.rbDescending.Size = new System.Drawing.Size(82, 17);
            this.rbDescending.TabIndex = 7;
            this.rbDescending.TabStop = true;
            this.rbDescending.Text = "Descending";
            this.rbDescending.UseVisualStyleBackColor = true;
            this.rbDescending.CheckedChanged += new System.EventHandler(this.rbDescending_CheckedChanged);
            // 
            // rbAscending
            // 
            this.rbAscending.AutoSize = true;
            this.rbAscending.Location = new System.Drawing.Point(33, 96);
            this.rbAscending.Name = "rbAscending";
            this.rbAscending.Size = new System.Drawing.Size(75, 17);
            this.rbAscending.TabIndex = 6;
            this.rbAscending.TabStop = true;
            this.rbAscending.Text = "Ascending";
            this.rbAscending.UseVisualStyleBackColor = true;
            this.rbAscending.CheckedChanged += new System.EventHandler(this.rbAscending_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(10, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Sort by :";
            // 
            // btnApplyLeftAll
            // 
            this.btnApplyLeftAll.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnApplyLeftAll.BackgroundImage = global::Inventory.CFC.Properties.Resources._2leftarrow_32;
            this.btnApplyLeftAll.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnApplyLeftAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnApplyLeftAll.Location = new System.Drawing.Point(268, 203);
            this.btnApplyLeftAll.Name = "btnApplyLeftAll";
            this.btnApplyLeftAll.Size = new System.Drawing.Size(54, 27);
            this.btnApplyLeftAll.TabIndex = 62;
            this.btnApplyLeftAll.UseVisualStyleBackColor = true;
            this.btnApplyLeftAll.Click += new System.EventHandler(this.btnApplyLeftAll_Click);
            // 
            // btnApplyLeft
            // 
            this.btnApplyLeft.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnApplyLeft.BackgroundImage = global::Inventory.CFC.Properties.Resources._1leftarrow_32;
            this.btnApplyLeft.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnApplyLeft.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnApplyLeft.Location = new System.Drawing.Point(268, 172);
            this.btnApplyLeft.Name = "btnApplyLeft";
            this.btnApplyLeft.Size = new System.Drawing.Size(54, 27);
            this.btnApplyLeft.TabIndex = 61;
            this.btnApplyLeft.UseVisualStyleBackColor = true;
            this.btnApplyLeft.Click += new System.EventHandler(this.btnApplyLeft_Click);
            // 
            // btnApplyAll
            // 
            this.btnApplyAll.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnApplyAll.BackgroundImage = global::Inventory.CFC.Properties.Resources._2rightarrow_32;
            this.btnApplyAll.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnApplyAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnApplyAll.Location = new System.Drawing.Point(268, 87);
            this.btnApplyAll.Name = "btnApplyAll";
            this.btnApplyAll.Size = new System.Drawing.Size(54, 27);
            this.btnApplyAll.TabIndex = 1;
            this.btnApplyAll.UseVisualStyleBackColor = true;
            this.btnApplyAll.Click += new System.EventHandler(this.btnApplyAll_Click);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnOk.Image = global::Inventory.CFC.Properties.Resources.page_preview_32;
            this.btnOk.Location = new System.Drawing.Point(639, 304);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(89, 45);
            this.btnOk.TabIndex = 8;
            this.btnOk.Text = "Preview";
            this.btnOk.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnApplyRow
            // 
            this.btnApplyRow.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnApplyRow.BackColor = System.Drawing.Color.Transparent;
            this.btnApplyRow.BackgroundImage = global::Inventory.CFC.Properties.Resources._1rightarrow_32;
            this.btnApplyRow.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnApplyRow.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnApplyRow.Location = new System.Drawing.Point(268, 118);
            this.btnApplyRow.Name = "btnApplyRow";
            this.btnApplyRow.Size = new System.Drawing.Size(54, 27);
            this.btnApplyRow.TabIndex = 2;
            this.btnApplyRow.UseVisualStyleBackColor = false;
            this.btnApplyRow.Click += new System.EventHandler(this.btnApplyRow_Click);
            // 
            // btnUp
            // 
            this.btnUp.BackgroundImage = global::Inventory.CFC.Properties.Resources._1uparrow1_32;
            this.btnUp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnUp.Location = new System.Drawing.Point(590, 132);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(31, 33);
            this.btnUp.TabIndex = 63;
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnDown
            // 
            this.btnDown.BackgroundImage = global::Inventory.CFC.Properties.Resources._1downarrow1_32;
            this.btnDown.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnDown.Location = new System.Drawing.Point(590, 168);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(31, 33);
            this.btnDown.TabIndex = 64;
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dgvLeft);
            this.groupBox1.Controls.Add(this.btnDown);
            this.groupBox1.Controls.Add(this.btnApplyAll);
            this.groupBox1.Controls.Add(this.btnUp);
            this.groupBox1.Controls.Add(this.btnApplyRow);
            this.groupBox1.Controls.Add(this.btnApplyLeftAll);
            this.groupBox1.Controls.Add(this.btnApplyLeft);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.dgvRight);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(5, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(629, 346);
            this.groupBox1.TabIndex = 65;
            this.groupBox1.TabStop = false;
            // 
            // btnPrint
            // 
            this.btnPrint.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnPrint.Enabled = false;
            this.btnPrint.Image = global::Inventory.CFC.Properties.Resources.Printers_and_Faxes_32;
            this.btnPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPrint.Location = new System.Drawing.Point(732, 304);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(82, 45);
            this.btnPrint.TabIndex = 66;
            this.btnPrint.Text = "Print";
            this.btnPrint.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // FHDRprtSelectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(820, 355);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gbSettings);
            this.Controls.Add(this.btnOk);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FHDRprtSelectForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FHDRprtSelectForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FHDRprtSelectForm_FormClosing);
            this.Load += new System.EventHandler(this.FHDRprtSelectForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRight)).EndInit();
            this.gbSettings.ResumeLayout(false);
            this.gbSettings.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvLeft;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dgvRight;
        private System.Windows.Forms.Button btnApplyRow;
        private System.Windows.Forms.Button btnApplyAll;
        public System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.GroupBox gbSettings;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtGroupby;
        private System.Windows.Forms.RadioButton rbDescending;
        private System.Windows.Forms.RadioButton rbAscending;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RadioButton rbNoSort;
        private System.Windows.Forms.Button btnApplyLeft;
        private System.Windows.Forms.Button btnApplyLeftAll;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.Button btnPrint;
    }
}