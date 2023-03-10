namespace UploadDownloadUtility
{
    partial class frmLogPosting
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tabRunControl = new System.Windows.Forms.TabControl();
            this.tabMain = new System.Windows.Forms.TabPage();
            this.lblMessageTest = new System.Windows.Forms.Label();
            this.btnLoadConfigFile = new System.Windows.Forms.Button();
            this.btnModifyGrid = new System.Windows.Forms.Button();
            this.btnSaveGrid = new System.Windows.Forms.Button();
            this.grpBxAppConfig = new System.Windows.Forms.GroupBox();
            this.dataGridKeys = new System.Windows.Forms.DataGridView();
            this.dataGridKey = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnTestCon = new System.Windows.Forms.Button();
            this.gBoxApp = new System.Windows.Forms.GroupBox();
            this.chBoxAutoChangeShift = new System.Windows.Forms.CheckBox();
            this.chBoxDefaultShift = new System.Windows.Forms.CheckBox();
            this.chBoxOTAfterMidnight = new System.Windows.Forms.CheckBox();
            this.txtSQLUsername = new System.Windows.Forms.TextBox();
            this.txtSQLPassword = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txtProfile = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtDtr = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtConfi = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtNonConfi = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtDataSource = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tabConsole = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelPercent = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.pBarStatus = new System.Windows.Forms.ProgressBar();
            this.lblEndDate = new System.Windows.Forms.Label();
            this.lblStartDate = new System.Windows.Forms.Label();
            this.dPickerEnd = new System.Windows.Forms.DateTimePicker();
            this.cBoxServiceCode = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dPickerStart = new System.Windows.Forms.DateTimePicker();
            this.btnCancelRun = new System.Windows.Forms.Button();
            this.btnRunConsole = new System.Windows.Forms.Button();
            this.chBoxLabHours = new System.Windows.Forms.CheckBox();
            this.tabPostFlag = new System.Windows.Forms.TabPage();
            this.grpResullt = new System.Windows.Forms.GroupBox();
            this.cbCheckAll = new System.Windows.Forms.CheckBox();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.dgvResult = new System.Windows.Forms.DataGridView();
            this.Select = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.EmployeeId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LastName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FirstName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Flag = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lblMessage = new System.Windows.Forms.Label();
            this.cbPosted = new System.Windows.Forms.CheckBox();
            this.cmbProfile = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.label14 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.label18 = new System.Windows.Forms.Label();
            this.lblRowCount = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.btnUpdatePostFlag = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.bWokerPosting = new System.ComponentModel.BackgroundWorker();
            this.tmrPosting = new System.Windows.Forms.Timer(this.components);
            this.bWorkerGetDTR = new System.ComponentModel.BackgroundWorker();
            this.tabRunControl.SuspendLayout();
            this.tabMain.SuspendLayout();
            this.grpBxAppConfig.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridKeys)).BeginInit();
            this.gBoxApp.SuspendLayout();
            this.tabConsole.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPostFlag.SuspendLayout();
            this.grpResullt.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResult)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabRunControl
            // 
            this.tabRunControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabRunControl.Controls.Add(this.tabMain);
            this.tabRunControl.Controls.Add(this.tabConsole);
            this.tabRunControl.Controls.Add(this.tabPostFlag);
            this.tabRunControl.Location = new System.Drawing.Point(0, 3);
            this.tabRunControl.Name = "tabRunControl";
            this.tabRunControl.SelectedIndex = 0;
            this.tabRunControl.Size = new System.Drawing.Size(728, 451);
            this.tabRunControl.TabIndex = 0;
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.lblMessageTest);
            this.tabMain.Controls.Add(this.btnLoadConfigFile);
            this.tabMain.Controls.Add(this.btnModifyGrid);
            this.tabMain.Controls.Add(this.btnSaveGrid);
            this.tabMain.Controls.Add(this.grpBxAppConfig);
            this.tabMain.Controls.Add(this.btnTestCon);
            this.tabMain.Controls.Add(this.gBoxApp);
            this.tabMain.Location = new System.Drawing.Point(4, 22);
            this.tabMain.Name = "tabMain";
            this.tabMain.Size = new System.Drawing.Size(720, 425);
            this.tabMain.TabIndex = 2;
            this.tabMain.Text = "Connection Settings";
            this.tabMain.UseVisualStyleBackColor = true;
            // 
            // lblMessageTest
            // 
            this.lblMessageTest.AutoSize = true;
            this.lblMessageTest.Location = new System.Drawing.Point(165, 209);
            this.lblMessageTest.Name = "lblMessageTest";
            this.lblMessageTest.Size = new System.Drawing.Size(0, 13);
            this.lblMessageTest.TabIndex = 16;
            // 
            // btnLoadConfigFile
            // 
            this.btnLoadConfigFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoadConfigFile.Location = new System.Drawing.Point(229, 397);
            this.btnLoadConfigFile.Name = "btnLoadConfigFile";
            this.btnLoadConfigFile.Size = new System.Drawing.Size(117, 23);
            this.btnLoadConfigFile.TabIndex = 15;
            this.btnLoadConfigFile.Text = "Load Config File";
            this.btnLoadConfigFile.UseVisualStyleBackColor = true;
            this.btnLoadConfigFile.Click += new System.EventHandler(this.btnLoadConfigFile_Click);
            // 
            // btnModifyGrid
            // 
            this.btnModifyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnModifyGrid.Location = new System.Drawing.Point(352, 397);
            this.btnModifyGrid.Name = "btnModifyGrid";
            this.btnModifyGrid.Size = new System.Drawing.Size(117, 23);
            this.btnModifyGrid.TabIndex = 0;
            this.btnModifyGrid.Text = "Edit";
            this.btnModifyGrid.UseVisualStyleBackColor = true;
            this.btnModifyGrid.Click += new System.EventHandler(this.btnModifyGrid_Click);
            // 
            // btnSaveGrid
            // 
            this.btnSaveGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveGrid.Location = new System.Drawing.Point(475, 397);
            this.btnSaveGrid.Name = "btnSaveGrid";
            this.btnSaveGrid.Size = new System.Drawing.Size(117, 23);
            this.btnSaveGrid.TabIndex = 1;
            this.btnSaveGrid.Text = "Save";
            this.btnSaveGrid.UseVisualStyleBackColor = true;
            this.btnSaveGrid.Click += new System.EventHandler(this.btnSaveGrid_Click);
            // 
            // grpBxAppConfig
            // 
            this.grpBxAppConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpBxAppConfig.Controls.Add(this.dataGridKeys);
            this.grpBxAppConfig.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpBxAppConfig.Location = new System.Drawing.Point(3, 192);
            this.grpBxAppConfig.Name = "grpBxAppConfig";
            this.grpBxAppConfig.Size = new System.Drawing.Size(712, 199);
            this.grpBxAppConfig.TabIndex = 14;
            this.grpBxAppConfig.TabStop = false;
            this.grpBxAppConfig.Text = "App Configuration";
            // 
            // dataGridKeys
            // 
            this.dataGridKeys.AllowUserToAddRows = false;
            this.dataGridKeys.AllowUserToDeleteRows = false;
            this.dataGridKeys.AllowUserToResizeColumns = false;
            this.dataGridKeys.AllowUserToResizeRows = false;
            this.dataGridKeys.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridKeys.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridKeys.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridKey,
            this.dataGridValue});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridKeys.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridKeys.Location = new System.Drawing.Point(6, 22);
            this.dataGridKeys.Name = "dataGridKeys";
            this.dataGridKeys.ReadOnly = true;
            this.dataGridKeys.RowHeadersVisible = false;
            this.dataGridKeys.Size = new System.Drawing.Size(700, 171);
            this.dataGridKeys.TabIndex = 2;
            // 
            // dataGridKey
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.LightGray;
            this.dataGridKey.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridKey.HeaderText = "Key";
            this.dataGridKey.Name = "dataGridKey";
            this.dataGridKey.ReadOnly = true;
            this.dataGridKey.Width = 200;
            // 
            // dataGridValue
            // 
            this.dataGridValue.HeaderText = "Value";
            this.dataGridValue.Name = "dataGridValue";
            this.dataGridValue.ReadOnly = true;
            this.dataGridValue.Width = 420;
            // 
            // btnTestCon
            // 
            this.btnTestCon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTestCon.Enabled = false;
            this.btnTestCon.Location = new System.Drawing.Point(598, 397);
            this.btnTestCon.Name = "btnTestCon";
            this.btnTestCon.Size = new System.Drawing.Size(117, 23);
            this.btnTestCon.TabIndex = 12;
            this.btnTestCon.Text = "Test Connection";
            this.btnTestCon.UseVisualStyleBackColor = true;
            this.btnTestCon.Click += new System.EventHandler(this.btnTestCon_Click);
            // 
            // gBoxApp
            // 
            this.gBoxApp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gBoxApp.Controls.Add(this.chBoxAutoChangeShift);
            this.gBoxApp.Controls.Add(this.chBoxDefaultShift);
            this.gBoxApp.Controls.Add(this.chBoxOTAfterMidnight);
            this.gBoxApp.Controls.Add(this.txtSQLUsername);
            this.gBoxApp.Controls.Add(this.txtSQLPassword);
            this.gBoxApp.Controls.Add(this.label11);
            this.gBoxApp.Controls.Add(this.label10);
            this.gBoxApp.Controls.Add(this.txtProfile);
            this.gBoxApp.Controls.Add(this.label9);
            this.gBoxApp.Controls.Add(this.txtDtr);
            this.gBoxApp.Controls.Add(this.label8);
            this.gBoxApp.Controls.Add(this.txtConfi);
            this.gBoxApp.Controls.Add(this.label7);
            this.gBoxApp.Controls.Add(this.txtNonConfi);
            this.gBoxApp.Controls.Add(this.label6);
            this.gBoxApp.Controls.Add(this.txtDataSource);
            this.gBoxApp.Controls.Add(this.label5);
            this.gBoxApp.Enabled = false;
            this.gBoxApp.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gBoxApp.Location = new System.Drawing.Point(3, 3);
            this.gBoxApp.Name = "gBoxApp";
            this.gBoxApp.Size = new System.Drawing.Size(712, 183);
            this.gBoxApp.TabIndex = 3;
            this.gBoxApp.TabStop = false;
            this.gBoxApp.Text = "Application Config";
            // 
            // chBoxAutoChangeShift
            // 
            this.chBoxAutoChangeShift.AutoSize = true;
            this.chBoxAutoChangeShift.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chBoxAutoChangeShift.Location = new System.Drawing.Point(373, 87);
            this.chBoxAutoChangeShift.Name = "chBoxAutoChangeShift";
            this.chBoxAutoChangeShift.Size = new System.Drawing.Size(117, 19);
            this.chBoxAutoChangeShift.TabIndex = 21;
            this.chBoxAutoChangeShift.Text = "AutoChangeShift";
            this.chBoxAutoChangeShift.UseVisualStyleBackColor = true;
            // 
            // chBoxDefaultShift
            // 
            this.chBoxDefaultShift.AutoSize = true;
            this.chBoxDefaultShift.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chBoxDefaultShift.Location = new System.Drawing.Point(373, 59);
            this.chBoxDefaultShift.Name = "chBoxDefaultShift";
            this.chBoxDefaultShift.Size = new System.Drawing.Size(89, 19);
            this.chBoxDefaultShift.TabIndex = 20;
            this.chBoxDefaultShift.Text = "DefaultShift";
            this.chBoxDefaultShift.UseVisualStyleBackColor = true;
            // 
            // chBoxOTAfterMidnight
            // 
            this.chBoxOTAfterMidnight.AutoSize = true;
            this.chBoxOTAfterMidnight.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chBoxOTAfterMidnight.Location = new System.Drawing.Point(373, 30);
            this.chBoxOTAfterMidnight.Name = "chBoxOTAfterMidnight";
            this.chBoxOTAfterMidnight.Size = new System.Drawing.Size(114, 19);
            this.chBoxOTAfterMidnight.TabIndex = 19;
            this.chBoxOTAfterMidnight.Text = "OTAfterMidnight";
            this.chBoxOTAfterMidnight.UseVisualStyleBackColor = true;
            // 
            // txtSQLUsername
            // 
            this.txtSQLUsername.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSQLUsername.Location = new System.Drawing.Point(481, 119);
            this.txtSQLUsername.Name = "txtSQLUsername";
            this.txtSQLUsername.Size = new System.Drawing.Size(211, 21);
            this.txtSQLUsername.TabIndex = 15;
            // 
            // txtSQLPassword
            // 
            this.txtSQLPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSQLPassword.Location = new System.Drawing.Point(480, 146);
            this.txtSQLPassword.Name = "txtSQLPassword";
            this.txtSQLPassword.PasswordChar = '*';
            this.txtSQLPassword.Size = new System.Drawing.Size(212, 21);
            this.txtSQLPassword.TabIndex = 13;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(373, 149);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(91, 15);
            this.label11.TabIndex = 11;
            this.label11.Text = "SQL Password:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(373, 119);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(95, 15);
            this.label10.TabIndex = 10;
            this.label10.Text = "SQL Username:";
            // 
            // txtProfile
            // 
            this.txtProfile.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtProfile.Location = new System.Drawing.Point(135, 57);
            this.txtProfile.Name = "txtProfile";
            this.txtProfile.Size = new System.Drawing.Size(203, 21);
            this.txtProfile.TabIndex = 9;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(16, 60);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(96, 15);
            this.label9.TabIndex = 8;
            this.label9.Text = "CentralDBName:";
            // 
            // txtDtr
            // 
            this.txtDtr.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDtr.Location = new System.Drawing.Point(135, 150);
            this.txtDtr.Name = "txtDtr";
            this.txtDtr.Size = new System.Drawing.Size(203, 21);
            this.txtDtr.TabIndex = 7;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(16, 152);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(77, 15);
            this.label8.TabIndex = 6;
            this.label8.Text = "DBNameDtr:";
            // 
            // txtConfi
            // 
            this.txtConfi.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtConfi.Location = new System.Drawing.Point(135, 119);
            this.txtConfi.Name = "txtConfi";
            this.txtConfi.Size = new System.Drawing.Size(203, 21);
            this.txtConfi.TabIndex = 5;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(16, 121);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(89, 15);
            this.label7.TabIndex = 4;
            this.label7.Text = "DBNameConfi:";
            // 
            // txtNonConfi
            // 
            this.txtNonConfi.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNonConfi.Location = new System.Drawing.Point(135, 88);
            this.txtNonConfi.Name = "txtNonConfi";
            this.txtNonConfi.Size = new System.Drawing.Size(203, 21);
            this.txtNonConfi.TabIndex = 3;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(16, 91);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(112, 15);
            this.label6.TabIndex = 2;
            this.label6.Text = "DBNameNonConfi:";
            // 
            // txtDataSource
            // 
            this.txtDataSource.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDataSource.Location = new System.Drawing.Point(135, 26);
            this.txtDataSource.Name = "txtDataSource";
            this.txtDataSource.Size = new System.Drawing.Size(203, 21);
            this.txtDataSource.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(16, 30);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(78, 15);
            this.label5.TabIndex = 0;
            this.label5.Text = "DataSource :";
            // 
            // tabConsole
            // 
            this.tabConsole.Controls.Add(this.groupBox1);
            this.tabConsole.Location = new System.Drawing.Point(4, 22);
            this.tabConsole.Name = "tabConsole";
            this.tabConsole.Padding = new System.Windows.Forms.Padding(3);
            this.tabConsole.Size = new System.Drawing.Size(720, 425);
            this.tabConsole.TabIndex = 0;
            this.tabConsole.Text = "Service Codes Utility";
            this.tabConsole.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.labelPercent);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.pBarStatus);
            this.groupBox1.Controls.Add(this.lblEndDate);
            this.groupBox1.Controls.Add(this.lblStartDate);
            this.groupBox1.Controls.Add(this.dPickerEnd);
            this.groupBox1.Controls.Add(this.cBoxServiceCode);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.dPickerStart);
            this.groupBox1.Controls.Add(this.btnCancelRun);
            this.groupBox1.Controls.Add(this.btnRunConsole);
            this.groupBox1.Controls.Add(this.chBoxLabHours);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(707, 413);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Run Service";
            // 
            // labelPercent
            // 
            this.labelPercent.AutoSize = true;
            this.labelPercent.Location = new System.Drawing.Point(606, 117);
            this.labelPercent.Name = "labelPercent";
            this.labelPercent.Size = new System.Drawing.Size(25, 15);
            this.labelPercent.TabIndex = 10;
            this.labelPercent.Text = "0%";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(561, 117);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 15);
            this.label4.TabIndex = 9;
            this.label4.Text = "Status :";
            // 
            // pBarStatus
            // 
            this.pBarStatus.Location = new System.Drawing.Point(9, 87);
            this.pBarStatus.Name = "pBarStatus";
            this.pBarStatus.Size = new System.Drawing.Size(622, 23);
            this.pBarStatus.TabIndex = 8;
            // 
            // lblEndDate
            // 
            this.lblEndDate.AutoSize = true;
            this.lblEndDate.Location = new System.Drawing.Point(212, 62);
            this.lblEndDate.Name = "lblEndDate";
            this.lblEndDate.Size = new System.Drawing.Size(11, 15);
            this.lblEndDate.TabIndex = 7;
            this.lblEndDate.Text = "-";
            // 
            // lblStartDate
            // 
            this.lblStartDate.AutoSize = true;
            this.lblStartDate.Location = new System.Drawing.Point(6, 60);
            this.lblStartDate.Name = "lblStartDate";
            this.lblStartDate.Size = new System.Drawing.Size(61, 15);
            this.lblStartDate.TabIndex = 6;
            this.lblStartDate.Text = "Start Date";
            // 
            // dPickerEnd
            // 
            this.dPickerEnd.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dPickerEnd.Location = new System.Drawing.Point(229, 59);
            this.dPickerEnd.Name = "dPickerEnd";
            this.dPickerEnd.Size = new System.Drawing.Size(115, 20);
            this.dPickerEnd.TabIndex = 5;
            // 
            // cBoxServiceCode
            // 
            this.cBoxServiceCode.FormattingEnabled = true;
            this.cBoxServiceCode.Location = new System.Drawing.Point(91, 30);
            this.cBoxServiceCode.Name = "cBoxServiceCode";
            this.cBoxServiceCode.Size = new System.Drawing.Size(253, 21);
            this.cBoxServiceCode.TabIndex = 4;
            this.cBoxServiceCode.SelectedIndexChanged += new System.EventHandler(this.cBoxServiceCode_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "Service Code";
            // 
            // dPickerStart
            // 
            this.dPickerStart.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dPickerStart.Location = new System.Drawing.Point(91, 59);
            this.dPickerStart.Name = "dPickerStart";
            this.dPickerStart.Size = new System.Drawing.Size(115, 20);
            this.dPickerStart.TabIndex = 2;
            // 
            // btnCancelRun
            // 
            this.btnCancelRun.Location = new System.Drawing.Point(132, 116);
            this.btnCancelRun.Name = "btnCancelRun";
            this.btnCancelRun.Size = new System.Drawing.Size(117, 23);
            this.btnCancelRun.TabIndex = 1;
            this.btnCancelRun.Text = "Stop";
            this.btnCancelRun.UseVisualStyleBackColor = true;
            this.btnCancelRun.Click += new System.EventHandler(this.btnCancelRun_Click);
            // 
            // btnRunConsole
            // 
            this.btnRunConsole.Location = new System.Drawing.Point(9, 116);
            this.btnRunConsole.Name = "btnRunConsole";
            this.btnRunConsole.Size = new System.Drawing.Size(117, 23);
            this.btnRunConsole.TabIndex = 0;
            this.btnRunConsole.Text = "Start";
            this.btnRunConsole.UseVisualStyleBackColor = true;
            this.btnRunConsole.Click += new System.EventHandler(this.btnRunConsole_Click);
            // 
            // chBoxLabHours
            // 
            this.chBoxLabHours.AutoSize = true;
            this.chBoxLabHours.Location = new System.Drawing.Point(91, 60);
            this.chBoxLabHours.Name = "chBoxLabHours";
            this.chBoxLabHours.Size = new System.Drawing.Size(181, 19);
            this.chBoxLabHours.TabIndex = 11;
            this.chBoxLabHours.Text = "Skip Process Flag Validation";
            this.chBoxLabHours.UseVisualStyleBackColor = true;
            this.chBoxLabHours.Visible = false;
            // 
            // tabPostFlag
            // 
            this.tabPostFlag.Controls.Add(this.grpResullt);
            this.tabPostFlag.Controls.Add(this.groupBox3);
            this.tabPostFlag.Controls.Add(this.label18);
            this.tabPostFlag.Controls.Add(this.lblRowCount);
            this.tabPostFlag.Controls.Add(this.label19);
            this.tabPostFlag.Controls.Add(this.btnUpdatePostFlag);
            this.tabPostFlag.Controls.Add(this.progressBar1);
            this.tabPostFlag.Location = new System.Drawing.Point(4, 22);
            this.tabPostFlag.Name = "tabPostFlag";
            this.tabPostFlag.Padding = new System.Windows.Forms.Padding(3);
            this.tabPostFlag.Size = new System.Drawing.Size(720, 425);
            this.tabPostFlag.TabIndex = 3;
            this.tabPostFlag.Text = "DTR Post Flag Utility";
            this.tabPostFlag.UseVisualStyleBackColor = true;
            // 
            // grpResullt
            // 
            this.grpResullt.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpResullt.Controls.Add(this.cbCheckAll);
            this.grpResullt.Controls.Add(this.txtSearch);
            this.grpResullt.Controls.Add(this.dgvResult);
            this.grpResullt.Controls.Add(this.label13);
            this.grpResullt.Controls.Add(this.label12);
            this.grpResullt.Location = new System.Drawing.Point(6, 89);
            this.grpResullt.Name = "grpResullt";
            this.grpResullt.Size = new System.Drawing.Size(707, 301);
            this.grpResullt.TabIndex = 22;
            this.grpResullt.TabStop = false;
            this.grpResullt.Text = "Result";
            // 
            // cbCheckAll
            // 
            this.cbCheckAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbCheckAll.AutoSize = true;
            this.cbCheckAll.Location = new System.Drawing.Point(630, 16);
            this.cbCheckAll.Name = "cbCheckAll";
            this.cbCheckAll.Size = new System.Drawing.Size(71, 17);
            this.cbCheckAll.TabIndex = 9;
            this.cbCheckAll.Text = "Check All";
            this.cbCheckAll.UseVisualStyleBackColor = true;
            this.cbCheckAll.Click += new System.EventHandler(this.cbCheckAll_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(68, 13);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(175, 20);
            this.txtSearch.TabIndex = 8;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // dgvResult
            // 
            this.dgvResult.AllowUserToAddRows = false;
            this.dgvResult.AllowUserToDeleteRows = false;
            this.dgvResult.AllowUserToOrderColumns = true;
            this.dgvResult.AllowUserToResizeColumns = false;
            this.dgvResult.AllowUserToResizeRows = false;
            this.dgvResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvResult.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvResult.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Select,
            this.EmployeeId,
            this.LastName,
            this.FirstName,
            this.Date,
            this.Type,
            this.Time,
            this.Flag});
            this.dgvResult.Location = new System.Drawing.Point(9, 39);
            this.dgvResult.MultiSelect = false;
            this.dgvResult.Name = "dgvResult";
            this.dgvResult.RowHeadersWidth = 15;
            this.dgvResult.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvResult.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvResult.Size = new System.Drawing.Size(692, 256);
            this.dgvResult.TabIndex = 0;
            this.dgvResult.CurrentCellDirtyStateChanged += new System.EventHandler(this.dgvResult_CurrentCellDirtyStateChanged);
            // 
            // Select
            // 
            this.Select.DataPropertyName = "Select";
            this.Select.FillWeight = 50F;
            this.Select.HeaderText = "Select";
            this.Select.Name = "Select";
            this.Select.Width = 50;
            // 
            // EmployeeId
            // 
            this.EmployeeId.DataPropertyName = "EmployeeId";
            this.EmployeeId.HeaderText = "EmployeeId";
            this.EmployeeId.Name = "EmployeeId";
            this.EmployeeId.ReadOnly = true;
            // 
            // LastName
            // 
            this.LastName.DataPropertyName = "LastName";
            this.LastName.FillWeight = 150F;
            this.LastName.HeaderText = "LastName";
            this.LastName.Name = "LastName";
            this.LastName.ReadOnly = true;
            this.LastName.Width = 150;
            // 
            // FirstName
            // 
            this.FirstName.DataPropertyName = "FirstName";
            this.FirstName.FillWeight = 150F;
            this.FirstName.HeaderText = "FirstName";
            this.FirstName.Name = "FirstName";
            this.FirstName.ReadOnly = true;
            this.FirstName.Width = 150;
            // 
            // Date
            // 
            this.Date.DataPropertyName = "Date";
            this.Date.FillWeight = 150F;
            this.Date.HeaderText = "Date";
            this.Date.Name = "Date";
            this.Date.ReadOnly = true;
            this.Date.Width = 150;
            // 
            // Type
            // 
            this.Type.DataPropertyName = "Type";
            this.Type.FillWeight = 45F;
            this.Type.HeaderText = "Type";
            this.Type.Name = "Type";
            this.Type.ReadOnly = true;
            this.Type.Width = 45;
            // 
            // Time
            // 
            this.Time.DataPropertyName = "Time";
            this.Time.FillWeight = 45F;
            this.Time.HeaderText = "Time";
            this.Time.Name = "Time";
            this.Time.ReadOnly = true;
            this.Time.Width = 45;
            // 
            // Flag
            // 
            this.Flag.DataPropertyName = "Flag";
            this.Flag.FillWeight = 50F;
            this.Flag.HeaderText = "Posted";
            this.Flag.Name = "Flag";
            this.Flag.ReadOnly = true;
            this.Flag.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Flag.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Flag.Width = 50;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F);
            this.label13.Location = new System.Drawing.Point(52, 15);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(10, 15);
            this.label13.TabIndex = 7;
            this.label13.Text = ":";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F);
            this.label12.Location = new System.Drawing.Point(6, 15);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(46, 15);
            this.label12.TabIndex = 7;
            this.label12.Text = "Search";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.lblMessage);
            this.groupBox3.Controls.Add(this.cbPosted);
            this.groupBox3.Controls.Add(this.cmbProfile);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.button1);
            this.groupBox3.Controls.Add(this.dateTimePicker2);
            this.groupBox3.Controls.Add(this.label14);
            this.groupBox3.Controls.Add(this.label17);
            this.groupBox3.Controls.Add(this.label15);
            this.groupBox3.Controls.Add(this.dateTimePicker1);
            this.groupBox3.Location = new System.Drawing.Point(6, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(708, 80);
            this.groupBox3.TabIndex = 21;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Filters";
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F);
            this.lblMessage.ForeColor = System.Drawing.Color.DarkGoldenrod;
            this.lblMessage.Location = new System.Drawing.Point(390, 22);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(270, 15);
            this.lblMessage.TabIndex = 12;
            this.lblMessage.Text = "Please wait. Retrieving DTR records from server.";
            this.lblMessage.Visible = false;
            // 
            // cbPosted
            // 
            this.cbPosted.AutoSize = true;
            this.cbPosted.Location = new System.Drawing.Point(393, 49);
            this.cbPosted.Name = "cbPosted";
            this.cbPosted.Size = new System.Drawing.Size(59, 17);
            this.cbPosted.TabIndex = 9;
            this.cbPosted.Text = "Posted";
            this.cbPosted.UseVisualStyleBackColor = true;
            this.cbPosted.Click += new System.EventHandler(this.cbPosted_Click);
            // 
            // cmbProfile
            // 
            this.cmbProfile.FormattingEnabled = true;
            this.cmbProfile.Location = new System.Drawing.Point(104, 19);
            this.cmbProfile.Name = "cmbProfile";
            this.cmbProfile.Size = new System.Drawing.Size(280, 21);
            this.cmbProfile.TabIndex = 11;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F);
            this.label3.Location = new System.Drawing.Point(88, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(10, 15);
            this.label3.TabIndex = 7;
            this.label3.Text = ":";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F);
            this.label2.Location = new System.Drawing.Point(12, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 15);
            this.label2.TabIndex = 7;
            this.label2.Text = "Profile";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(458, 44);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(117, 23);
            this.button1.TabIndex = 10;
            this.button1.Text = "Generate";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnGenerate);
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.Checked = false;
            this.dateTimePicker2.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker2.Location = new System.Drawing.Point(104, 46);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(130, 20);
            this.dateTimePicker2.TabIndex = 8;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F);
            this.label14.Location = new System.Drawing.Point(12, 48);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(73, 15);
            this.label14.TabIndex = 7;
            this.label14.Text = "Date Range";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F);
            this.label17.Location = new System.Drawing.Point(239, 48);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(11, 15);
            this.label17.TabIndex = 7;
            this.label17.Text = "-";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F);
            this.label15.Location = new System.Drawing.Point(88, 48);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(10, 15);
            this.label15.TabIndex = 7;
            this.label15.Text = ":";
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker1.Location = new System.Drawing.Point(254, 46);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(130, 20);
            this.dateTimePicker1.TabIndex = 8;
            // 
            // label18
            // 
            this.label18.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F);
            this.label18.Location = new System.Drawing.Point(567, 401);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(25, 15);
            this.label18.TabIndex = 20;
            this.label18.Text = "0%";
            // 
            // lblRowCount
            // 
            this.lblRowCount.AutoSize = true;
            this.lblRowCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblRowCount.Location = new System.Drawing.Point(30, 345);
            this.lblRowCount.Name = "lblRowCount";
            this.lblRowCount.Size = new System.Drawing.Size(0, 15);
            this.lblRowCount.TabIndex = 19;
            // 
            // label19
            // 
            this.label19.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F);
            this.label19.Location = new System.Drawing.Point(524, 401);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(47, 15);
            this.label19.TabIndex = 18;
            this.label19.Text = "Status :";
            // 
            // btnUpdatePostFlag
            // 
            this.btnUpdatePostFlag.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUpdatePostFlag.Location = new System.Drawing.Point(596, 396);
            this.btnUpdatePostFlag.Name = "btnUpdatePostFlag";
            this.btnUpdatePostFlag.Size = new System.Drawing.Size(117, 23);
            this.btnUpdatePostFlag.TabIndex = 17;
            this.btnUpdatePostFlag.Text = "Update Post Flag";
            this.btnUpdatePostFlag.UseVisualStyleBackColor = true;
            this.btnUpdatePostFlag.Click += new System.EventHandler(this.btn_UpdatePostFlag);
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.progressBar1.Location = new System.Drawing.Point(5, 396);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(585, 23);
            this.progressBar1.TabIndex = 16;
            // 
            // bWokerPosting
            // 
            this.bWokerPosting.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwPosting_DoWork);
            this.bWokerPosting.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgwPosting_ProgressChanged);
            this.bWokerPosting.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwPosting_RunWorkerCompleted);
            // 
            // tmrPosting
            // 
            this.tmrPosting.Tick += new System.EventHandler(this.tmrPosting_Tick);
            // 
            // bWorkerGetDTR
            // 
            this.bWorkerGetDTR.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bWorkerGetDTR_DoWork);
            this.bWorkerGetDTR.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bWorkerGetDTR_RunWorkerCompleted);
            // 
            // frmLogPosting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(726, 453);
            this.Controls.Add(this.tabRunControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "frmLogPosting";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Log Posting Utility";
            this.Load += new System.EventHandler(this.frmLogPosting_Load);
            this.tabRunControl.ResumeLayout(false);
            this.tabMain.ResumeLayout(false);
            this.tabMain.PerformLayout();
            this.grpBxAppConfig.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridKeys)).EndInit();
            this.gBoxApp.ResumeLayout(false);
            this.gBoxApp.PerformLayout();
            this.tabConsole.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPostFlag.ResumeLayout(false);
            this.tabPostFlag.PerformLayout();
            this.grpResullt.ResumeLayout(false);
            this.grpResullt.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResult)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabRunControl;
        private System.Windows.Forms.TabPage tabConsole;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblEndDate;
        private System.Windows.Forms.Label lblStartDate;
        private System.Windows.Forms.DateTimePicker dPickerEnd;
        private System.Windows.Forms.ComboBox cBoxServiceCode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dPickerStart;
        private System.Windows.Forms.Button btnCancelRun;
        private System.Windows.Forms.Button btnRunConsole;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ProgressBar pBarStatus;
        private System.ComponentModel.BackgroundWorker bWokerPosting;
        private System.Windows.Forms.Label labelPercent;
        private System.Windows.Forms.TabPage tabMain;
        private System.Windows.Forms.GroupBox gBoxApp;
        private System.Windows.Forms.Button btnTestCon;
        private System.Windows.Forms.TextBox txtConfi;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtNonConfi;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtDataSource;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtDtr;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtSQLPassword;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtProfile;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtSQLUsername;
        private System.Windows.Forms.Button btnLoadConfigFile;
        private System.Windows.Forms.Button btnModifyGrid;
        private System.Windows.Forms.Button btnSaveGrid;
        private System.Windows.Forms.GroupBox grpBxAppConfig;
        private System.Windows.Forms.DataGridView dataGridKeys;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridKey;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridValue;
        private System.Windows.Forms.CheckBox chBoxAutoChangeShift;
        private System.Windows.Forms.CheckBox chBoxDefaultShift;
        private System.Windows.Forms.CheckBox chBoxOTAfterMidnight;
        private System.Windows.Forms.Label lblMessageTest;
        private System.Windows.Forms.Timer tmrPosting;
        private System.Windows.Forms.CheckBox chBoxLabHours;
        private System.Windows.Forms.TabPage tabPostFlag;
        private System.Windows.Forms.GroupBox grpResullt;
        private System.Windows.Forms.CheckBox cbCheckAll;
        private System.Windows.Forms.CheckBox cbPosted;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.DataGridView dgvResult;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox cmbProfile;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DateTimePicker dateTimePicker2;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label lblRowCount;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Button btnUpdatePostFlag;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker bWorkerGetDTR;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Select;
        private System.Windows.Forms.DataGridViewTextBoxColumn EmployeeId;
        private System.Windows.Forms.DataGridViewTextBoxColumn LastName;
        private System.Windows.Forms.DataGridViewTextBoxColumn FirstName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Date;
        private System.Windows.Forms.DataGridViewTextBoxColumn Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn Time;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Flag;
        
    }
}

