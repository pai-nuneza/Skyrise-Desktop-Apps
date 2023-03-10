namespace LogPostingService
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel1 = new System.Windows.Forms.Panel();
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
            this.bWokerPosting = new System.ComponentModel.BackgroundWorker();
            this.tmrPosting = new System.Windows.Forms.Timer(this.components);
            this.chBoxLabHours = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.tabRunControl.SuspendLayout();
            this.tabMain.SuspendLayout();
            this.grpBxAppConfig.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridKeys)).BeginInit();
            this.gBoxApp.SuspendLayout();
            this.tabConsole.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.tabRunControl);
            this.panel1.Location = new System.Drawing.Point(1, -1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(733, 458);
            this.panel1.TabIndex = 0;
            // 
            // tabRunControl
            // 
            this.tabRunControl.Controls.Add(this.tabMain);
            this.tabRunControl.Controls.Add(this.tabConsole);
            this.tabRunControl.Location = new System.Drawing.Point(11, 13);
            this.tabRunControl.Name = "tabRunControl";
            this.tabRunControl.SelectedIndex = 0;
            this.tabRunControl.Size = new System.Drawing.Size(711, 433);
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
            this.tabMain.Size = new System.Drawing.Size(703, 407);
            this.tabMain.TabIndex = 2;
            this.tabMain.Text = "Main";
            this.tabMain.UseVisualStyleBackColor = true;
            // 
            // lblMessageTest
            // 
            this.lblMessageTest.AutoSize = true;
            this.lblMessageTest.Location = new System.Drawing.Point(159, 204);
            this.lblMessageTest.Name = "lblMessageTest";
            this.lblMessageTest.Size = new System.Drawing.Size(0, 13);
            this.lblMessageTest.TabIndex = 16;
            // 
            // btnLoadConfigFile
            // 
            this.btnLoadConfigFile.Location = new System.Drawing.Point(278, 379);
            this.btnLoadConfigFile.Name = "btnLoadConfigFile";
            this.btnLoadConfigFile.Size = new System.Drawing.Size(119, 23);
            this.btnLoadConfigFile.TabIndex = 15;
            this.btnLoadConfigFile.Text = "Load Config File";
            this.btnLoadConfigFile.UseVisualStyleBackColor = true;
            this.btnLoadConfigFile.Click += new System.EventHandler(this.btnLoadConfigFile_Click);
            // 
            // btnModifyGrid
            // 
            this.btnModifyGrid.Location = new System.Drawing.Point(526, 379);
            this.btnModifyGrid.Name = "btnModifyGrid";
            this.btnModifyGrid.Size = new System.Drawing.Size(75, 23);
            this.btnModifyGrid.TabIndex = 0;
            this.btnModifyGrid.Text = "Edit";
            this.btnModifyGrid.UseVisualStyleBackColor = true;
            this.btnModifyGrid.Click += new System.EventHandler(this.btnModifyGrid_Click);
            // 
            // btnSaveGrid
            // 
            this.btnSaveGrid.Location = new System.Drawing.Point(607, 379);
            this.btnSaveGrid.Name = "btnSaveGrid";
            this.btnSaveGrid.Size = new System.Drawing.Size(75, 23);
            this.btnSaveGrid.TabIndex = 1;
            this.btnSaveGrid.Text = "Save";
            this.btnSaveGrid.UseVisualStyleBackColor = true;
            this.btnSaveGrid.Click += new System.EventHandler(this.btnSaveGrid_Click);
            // 
            // grpBxAppConfig
            // 
            this.grpBxAppConfig.Controls.Add(this.dataGridKeys);
            this.grpBxAppConfig.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpBxAppConfig.Location = new System.Drawing.Point(17, 199);
            this.grpBxAppConfig.Name = "grpBxAppConfig";
            this.grpBxAppConfig.Size = new System.Drawing.Size(668, 176);
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
            this.dataGridKeys.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridKeys.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridKey,
            this.dataGridValue});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridKeys.DefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridKeys.Location = new System.Drawing.Point(19, 22);
            this.dataGridKeys.Name = "dataGridKeys";
            this.dataGridKeys.ReadOnly = true;
            this.dataGridKeys.RowHeadersVisible = false;
            this.dataGridKeys.Size = new System.Drawing.Size(634, 148);
            this.dataGridKeys.TabIndex = 2;
            // 
            // dataGridKey
            // 
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.LightGray;
            this.dataGridKey.DefaultCellStyle = dataGridViewCellStyle3;
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
            this.btnTestCon.Enabled = false;
            this.btnTestCon.Location = new System.Drawing.Point(403, 379);
            this.btnTestCon.Name = "btnTestCon";
            this.btnTestCon.Size = new System.Drawing.Size(117, 23);
            this.btnTestCon.TabIndex = 12;
            this.btnTestCon.Text = "Test Connection";
            this.btnTestCon.UseVisualStyleBackColor = true;
            this.btnTestCon.Click += new System.EventHandler(this.btnTestCon_Click);
            // 
            // gBoxApp
            // 
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
            this.gBoxApp.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gBoxApp.Location = new System.Drawing.Point(16, 5);
            this.gBoxApp.Name = "gBoxApp";
            this.gBoxApp.Size = new System.Drawing.Size(668, 188);
            this.gBoxApp.TabIndex = 3;
            this.gBoxApp.TabStop = false;
            this.gBoxApp.Text = "Application Config";
            // 
            // chBoxAutoChangeShift
            // 
            this.chBoxAutoChangeShift.AutoSize = true;
            this.chBoxAutoChangeShift.Location = new System.Drawing.Point(371, 87);
            this.chBoxAutoChangeShift.Name = "chBoxAutoChangeShift";
            this.chBoxAutoChangeShift.Size = new System.Drawing.Size(133, 21);
            this.chBoxAutoChangeShift.TabIndex = 21;
            this.chBoxAutoChangeShift.Text = "AutoChangeShift";
            this.chBoxAutoChangeShift.UseVisualStyleBackColor = true;
            // 
            // chBoxDefaultShift
            // 
            this.chBoxDefaultShift.AutoSize = true;
            this.chBoxDefaultShift.Location = new System.Drawing.Point(372, 60);
            this.chBoxDefaultShift.Name = "chBoxDefaultShift";
            this.chBoxDefaultShift.Size = new System.Drawing.Size(100, 21);
            this.chBoxDefaultShift.TabIndex = 20;
            this.chBoxDefaultShift.Text = "DefaultShift";
            this.chBoxDefaultShift.UseVisualStyleBackColor = true;
            // 
            // chBoxOTAfterMidnight
            // 
            this.chBoxOTAfterMidnight.AutoSize = true;
            this.chBoxOTAfterMidnight.Location = new System.Drawing.Point(372, 30);
            this.chBoxOTAfterMidnight.Name = "chBoxOTAfterMidnight";
            this.chBoxOTAfterMidnight.Size = new System.Drawing.Size(130, 21);
            this.chBoxOTAfterMidnight.TabIndex = 19;
            this.chBoxOTAfterMidnight.Text = "OTAfterMidnight";
            this.chBoxOTAfterMidnight.UseVisualStyleBackColor = true;
            // 
            // txtSQLUsername
            // 
            this.txtSQLUsername.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSQLUsername.Location = new System.Drawing.Point(481, 119);
            this.txtSQLUsername.Name = "txtSQLUsername";
            this.txtSQLUsername.Size = new System.Drawing.Size(172, 23);
            this.txtSQLUsername.TabIndex = 15;
            // 
            // txtSQLPassword
            // 
            this.txtSQLPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSQLPassword.Location = new System.Drawing.Point(480, 146);
            this.txtSQLPassword.Name = "txtSQLPassword";
            this.txtSQLPassword.PasswordChar = '*';
            this.txtSQLPassword.Size = new System.Drawing.Size(173, 23);
            this.txtSQLPassword.TabIndex = 13;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(369, 149);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(105, 17);
            this.label11.TabIndex = 11;
            this.label11.Text = "SQL Password:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(368, 119);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(109, 17);
            this.label10.TabIndex = 10;
            this.label10.Text = "SQL Username:";
            // 
            // txtProfile
            // 
            this.txtProfile.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtProfile.Location = new System.Drawing.Point(146, 57);
            this.txtProfile.Name = "txtProfile";
            this.txtProfile.Size = new System.Drawing.Size(203, 23);
            this.txtProfile.TabIndex = 9;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(16, 60);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(108, 17);
            this.label9.TabIndex = 8;
            this.label9.Text = "ProfileDBName:";
            // 
            // txtDtr
            // 
            this.txtDtr.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDtr.Location = new System.Drawing.Point(146, 150);
            this.txtDtr.Name = "txtDtr";
            this.txtDtr.Size = new System.Drawing.Size(203, 23);
            this.txtDtr.TabIndex = 7;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(17, 153);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(87, 17);
            this.label8.TabIndex = 6;
            this.label8.Text = "DBNameDtr:";
            // 
            // txtConfi
            // 
            this.txtConfi.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtConfi.Location = new System.Drawing.Point(146, 119);
            this.txtConfi.Name = "txtConfi";
            this.txtConfi.Size = new System.Drawing.Size(203, 23);
            this.txtConfi.TabIndex = 5;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(16, 122);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(100, 17);
            this.label7.TabIndex = 4;
            this.label7.Text = "DBNameConfi:";
            // 
            // txtNonConfi
            // 
            this.txtNonConfi.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNonConfi.Location = new System.Drawing.Point(146, 88);
            this.txtNonConfi.Name = "txtNonConfi";
            this.txtNonConfi.Size = new System.Drawing.Size(203, 23);
            this.txtNonConfi.TabIndex = 3;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(17, 91);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(126, 17);
            this.label6.TabIndex = 2;
            this.label6.Text = "DBNameNonConfi:";
            // 
            // txtDataSource
            // 
            this.txtDataSource.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDataSource.Location = new System.Drawing.Point(146, 26);
            this.txtDataSource.Name = "txtDataSource";
            this.txtDataSource.Size = new System.Drawing.Size(203, 23);
            this.txtDataSource.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(17, 30);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(91, 17);
            this.label5.TabIndex = 0;
            this.label5.Text = "DataSource :";
            // 
            // tabConsole
            // 
            this.tabConsole.Controls.Add(this.groupBox1);
            this.tabConsole.Location = new System.Drawing.Point(4, 22);
            this.tabConsole.Name = "tabConsole";
            this.tabConsole.Padding = new System.Windows.Forms.Padding(3);
            this.tabConsole.Size = new System.Drawing.Size(703, 407);
            this.tabConsole.TabIndex = 0;
            this.tabConsole.Text = "Services";
            this.tabConsole.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chBoxLabHours);
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
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(17, 17);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(668, 362);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Run Service";
            // 
            // labelPercent
            // 
            this.labelPercent.AutoSize = true;
            this.labelPercent.Location = new System.Drawing.Point(64, 109);
            this.labelPercent.Name = "labelPercent";
            this.labelPercent.Size = new System.Drawing.Size(25, 15);
            this.labelPercent.TabIndex = 10;
            this.labelPercent.Text = "0%";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(21, 109);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 15);
            this.label4.TabIndex = 9;
            this.label4.Text = "Status :";
            // 
            // pBarStatus
            // 
            this.pBarStatus.Location = new System.Drawing.Point(21, 131);
            this.pBarStatus.Name = "pBarStatus";
            this.pBarStatus.Size = new System.Drawing.Size(622, 23);
            this.pBarStatus.TabIndex = 8;
            // 
            // lblEndDate
            // 
            this.lblEndDate.AutoSize = true;
            this.lblEndDate.Location = new System.Drawing.Point(440, 40);
            this.lblEndDate.Name = "lblEndDate";
            this.lblEndDate.Size = new System.Drawing.Size(58, 15);
            this.lblEndDate.TabIndex = 7;
            this.lblEndDate.Text = "End Date";
            // 
            // lblStartDate
            // 
            this.lblStartDate.AutoSize = true;
            this.lblStartDate.Location = new System.Drawing.Point(220, 40);
            this.lblStartDate.Name = "lblStartDate";
            this.lblStartDate.Size = new System.Drawing.Size(61, 15);
            this.lblStartDate.TabIndex = 6;
            this.lblStartDate.Text = "Start Date";
            // 
            // dPickerEnd
            // 
            this.dPickerEnd.Location = new System.Drawing.Point(443, 69);
            this.dPickerEnd.Name = "dPickerEnd";
            this.dPickerEnd.Size = new System.Drawing.Size(200, 20);
            this.dPickerEnd.TabIndex = 5;
            // 
            // cBoxServiceCode
            // 
            this.cBoxServiceCode.FormattingEnabled = true;
            this.cBoxServiceCode.Location = new System.Drawing.Point(21, 68);
            this.cBoxServiceCode.Name = "cBoxServiceCode";
            this.cBoxServiceCode.Size = new System.Drawing.Size(181, 21);
            this.cBoxServiceCode.TabIndex = 4;
            this.cBoxServiceCode.SelectedIndexChanged += new System.EventHandler(this.cBoxServiceCode_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "Service Code";
            // 
            // dPickerStart
            // 
            this.dPickerStart.Location = new System.Drawing.Point(223, 69);
            this.dPickerStart.Name = "dPickerStart";
            this.dPickerStart.Size = new System.Drawing.Size(200, 20);
            this.dPickerStart.TabIndex = 2;
            // 
            // btnCancelRun
            // 
            this.btnCancelRun.Location = new System.Drawing.Point(348, 186);
            this.btnCancelRun.Name = "btnCancelRun";
            this.btnCancelRun.Size = new System.Drawing.Size(75, 23);
            this.btnCancelRun.TabIndex = 1;
            this.btnCancelRun.Text = "Stop";
            this.btnCancelRun.UseVisualStyleBackColor = true;
            this.btnCancelRun.Click += new System.EventHandler(this.btnCancelRun_Click);
            // 
            // btnRunConsole
            // 
            this.btnRunConsole.Location = new System.Drawing.Point(253, 186);
            this.btnRunConsole.Name = "btnRunConsole";
            this.btnRunConsole.Size = new System.Drawing.Size(75, 23);
            this.btnRunConsole.TabIndex = 0;
            this.btnRunConsole.Text = "Start";
            this.btnRunConsole.UseVisualStyleBackColor = true;
            this.btnRunConsole.Click += new System.EventHandler(this.btnRunConsole_Click);
            // 
            // tmrPosting
            // 
            this.tmrPosting.Tick += new System.EventHandler(this.tmrPosting_Tick);
            // 
            // chBoxLabHours
            // 
            this.chBoxLabHours.AutoSize = true;
            this.chBoxLabHours.Location = new System.Drawing.Point(223, 69);
            this.chBoxLabHours.Name = "chBoxLabHours";
            this.chBoxLabHours.Size = new System.Drawing.Size(181, 19);
            this.chBoxLabHours.TabIndex = 11;
            this.chBoxLabHours.Text = "Skip Process Flag Validation";
            this.chBoxLabHours.UseVisualStyleBackColor = true;
            this.chBoxLabHours.Visible = false;
            // 
            // frmLogPosting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(735, 457);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "frmLogPosting";
            this.Text = "LogPostingService";
            this.panel1.ResumeLayout(false);
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
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
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
        
    }
}

