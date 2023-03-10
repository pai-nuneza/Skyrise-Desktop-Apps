using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Xml;
using System.Windows.Forms;
using CommonPostingLibrary;
using Posting.DAL;
using Posting.BLogic;
using UploadDownloadSchedulerConsole;
using System.Threading;
using System.Configuration;
namespace UploadDownloadUtility
{
    public partial class frmLogPosting : Form
    {
        private ConfigXML ConfigXML = new ConfigXML();
        private AppConfigXML AppConfigXML = new AppConfigXML();
        private BackgroundWorker bgwPosting;
        private Boolean IsProcessRunning = false;
        private bool IsStopWorker = false;
        private Thread thread;
        private bool AllowRefreshGrid = false;
        private string SelectedProfile = "";
        private bool Posted = false;
        private string StartDate = DateTime.Now.ToShortDateString();
        private string EndDate = DateTime.Now.ToShortDateString();
        DataTable AffectedRows = new DataTable();
        private delegate void Function();
        private string configPath = Application.StartupPath + @"\UploadDownloadUtility.exe.config";
        private string runConsole = "UploadDownloadSchedulerConsole.exe";
        private string[] encryptData = new string[11] { "ReportServerUsername"
                                                    , "ReportServerPassword"
                                                    , "DataSource"
                                                    , "CentralDBName"
                                                    , "DBNameDTR"
                                                    , "UserID"
                                                    , "Password"
                                                    , "PROXServer"
                                                    , "PROXDB"
                                                    , "PROXUserId"
                                                    , "PROXPassword"};
        string Identifier = string.Empty;
        string Filter1 = "Flag = True";
        string Filter2 = string.Empty;
        DataTable dsGrid = new DataTable();
        //delegate void SetConsoleParams(string service, string start, string end);
        //delegate string[] GetConsoleParams();
        public frmLogPosting()
        {
            InitializeComponent();
            //this.cBoxServiceCode.DataSource = GetListOfService();
            //this.bgwPosting = new BackgroundWorker 
            //{
            //    WorkerReportsProgress = true,
            //    WorkerSupportsCancellation = true
            //};
            /*
            this.bgwPosting.WorkerReportsProgress = true;
            this.bgwPosting.WorkerSupportsCancellation = true;
            this.bgwPosting.DoWork  += bgwPosting_DoWork;
            this.bgwPosting.ProgressChanged += bgwPosting_ProgressChanged;
            this.bgwPosting.RunWorkerCompleted += bgwPosting_RunWorkerCompleted;
             */

            this.bWokerPosting.WorkerReportsProgress = true;
            this.bWokerPosting.WorkerSupportsCancellation = true;
            this.bWokerPosting.DoWork += bgwPosting_DoWork;
            //this.bWokerPosting.ProgressChanged += bgwPosting_ProgressChanged;
            this.bWokerPosting.RunWorkerCompleted += bgwPosting_RunWorkerCompleted;
            this.btnModifyGrid.Enabled = false;
            this.btnSaveGrid.Enabled = false;
            this.btnCancelRun.Enabled = false;
            this.cBoxServiceCode.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void LoadConfigFile()
        {
            //getServicesCodes(); 
        }

        private void getServicesCodes()
        {
            DALHelper dal = new DALHelper();
            ParameterInfo[] paramInfo = new ParameterInfo[1];
            DataSet dataset = null;
            String sqlQuery = "";
            try
            {
                paramInfo[0] = new ParameterInfo("@Database", "HRCNONCONFI", SqlDbType.VarChar, 20);

                #region SchedulerServiceTimeSetting Query
                sqlQuery = @"SELECT  [Mss_ServiceCode]
                                    ,[Mss_IsMultipleRun] 
                               FROM M_ServiceSchedule
                              WHERE Mss_RecordStatus = 'A' ";
                #endregion

                dal.OpenDB();
                dal.BeginTransaction();
                dataset = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);

            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex.Message);
                if (dal != null)
                {
                    dal.RollBackTransaction();
                    dal.CloseDB();
                }
            }
            finally
            {
                dal.CommitTransaction();
                dal.CloseDB();
                dal.Dispose();
            }
        }

        private List<String> GetListOfService()
        {
            DataSet dataSet = new DataSet();
            DALHelper dal = new DALHelper("", true);
            String sqlQuery = "";
            List<string> serviceCodes = new List<string>();
            DataTable dTable = new DataTable();

            try
            {
                #region  SchedulerServiceTimeSetting Query
                sqlQuery = @"SELECT  [Mst_ServiceCode]
                                        ,[Mst_IsMultipleTrigger] 
                                   FROM M_ServiceTiming
                                  WHERE Mst_RecordStatus = 'A' ";
                #endregion

                dal.OpenDB();
                dataSet = dal.ExecuteDataSet(sqlQuery, CommandType.Text);
                if (dataSet != null)
                {
                    dTable = dataSet.Tables[0];
                }
                #region Add all Service code
                if (dTable != null)
                {
                    foreach (DataRow dr in dTable.Rows)
                    {
                        serviceCodes.Add(dr["Mst_ServiceCode"].ToString());
                    }
                }
                #endregion

            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception Message: " + ex.Message);
            }
            finally
            {
                dal.CloseDB();
                dal.Dispose();
            }

            return serviceCodes;
        }

        private void btnRunConsole_Click(object sender, EventArgs e)
        {
            //String serviceCode = "";
            //String startDate = "";
            //String endDate = "";
            //Action DoCrossThreadUIWork = () =>
            //{
            //    serviceCode = this.cBoxServiceCode.SelectedValue.ToString();
            //    startDate = this.dPickerStart.Value.ToShortDateString();
            //    endDate = this.dPickerEnd.Value.ToShortDateString();

            //};
            //this.BeginInvoke(DoCrossThreadUIWork);

            // MessageBox.Show("Service Code: " + serviceCode + " " + startDate + " " + endDate);
            //this.bgwPosting.RunWorkerAsync();
            if (cBoxServiceCode.DataSource != null)
            {

                this.bWokerPosting.RunWorkerAsync();
                this.btnCancelRun.Enabled = true;
                this.btnRunConsole.Enabled = false;
                this.pBarStatus.Value = 0;
                this.labelPercent.Text = this.pBarStatus.Value + "%";
            }
            else
            {
                MessageBox.Show("Please check/test network connection");
            }
        }

        //private void SetConsoleParams(string service, string start, string end) 
        //{
        //}

        //private string[] GetConsoleParams() 
        //{
        //    string[] setParams = new string[3];
        //    try 
        //    {
        //        setParams[0] = this.cBoxServiceCode.Text.ToString();
        //        setParams[1] = this.dPickerStart.Value.ToShortDateString();
        //        setParams[2] = this.dPickerEnd.Value.ToShortDateString();
        //    }catch(Exception ex)
        //    {

        //    }

        //    return setParams;
        //}
        private void bgwPosting_DoWork(object sender, DoWorkEventArgs e)
        {
            String serviceCode = "";
            String startDate = "";
            String endDate = "";
            bool enableCalc = false;
            string[] setParams;

            if (this.cBoxServiceCode.InvokeRequired && this.dPickerStart.InvokeRequired && this.dPickerEnd.InvokeRequired)
            {

                this.cBoxServiceCode.Invoke(new MethodInvoker(delegate { serviceCode = this.cBoxServiceCode.SelectedValue.ToString(); }));
                this.dPickerStart.Invoke(new MethodInvoker(delegate { startDate = this.dPickerStart.Value.ToShortDateString(); }));
                this.dPickerEnd.Invoke(new MethodInvoker(delegate { endDate = this.dPickerEnd.Value.ToShortDateString(); }));
                this.chBoxLabHours.Invoke(new MethodInvoker(delegate { enableCalc = this.chBoxLabHours.Checked; }));
                this.btnRunConsole.Invoke(new MethodInvoker(delegate { this.btnRunConsole.Enabled = false; }));
            }


            try
            {


                if (serviceCode == "LOGUPLOADING" || serviceCode == "MULTIPOCKETS" || serviceCode == "GENERICEMAIL")
                {
                    setParams = new string[3];
                    setParams[0] = serviceCode;
                    setParams[1] = startDate;
                    setParams[2] = endDate;
                }
                else if (serviceCode == "LABORHOURS" || serviceCode == "GENLABORHOURS")
                {

                    if (enableCalc == true)
                    {
                        setParams = new string[2];
                        setParams[0] = serviceCode;
                        setParams[1] = "1";
                    }
                    else
                    {
                        setParams = new string[1];
                        setParams[0] = serviceCode;
                    }
                }
                else
                {
                    setParams = new string[1];
                    setParams[0] = serviceCode;
                }

                BeginInvoke(new Function(delegate()
                {
                    tmrPosting.Enabled = true;
                    tmrPosting.Start();
                    pBarStatus.Maximum = 100;
                    pBarStatus.Visible = true;
                    pBarStatus.Value = 0;
                    pBarStatus.Step = 1;
                }));

                SchedulerConsole.Main(setParams);


                if (IsStopWorker)
                {
                    e.Cancel = true;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception message: " + ex.Message);
            }
        }

        private void bgwPosting_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.pBarStatus.Value = Convert.ToInt16(Globals.Progress);
            this.labelPercent.Text = pBarStatus.Value + "%";
        }

        private void bgwPosting_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // MessageBox.Show("Finished Running Console");
            this.pBarStatus.Value = Convert.ToInt16(Globals.Progress);
            this.pBarStatus.Value = 100;
            this.btnRunConsole.Enabled = true;
            this.btnCancelRun.Enabled = false;
            this.labelPercent.Text = "100 %";
            this.IsStopWorker = false;
            MessageBox.Show("Service Code Process Completed");
            this.pBarStatus.Value = 0;
            this.labelPercent.Text = "0 %";
        }

        private void RefreshMainGrid()
        {
            ClearGrid(dataGridKeys);
            foreach (AppConfigFile AppConfigFile in AppConfigXML.AppConfigList)
            {
                DataGridViewRow dvRowProfile = new DataGridViewRow();
                dvRowProfile.Cells.Add(new DataGridViewTextBoxCell());
                dvRowProfile.Cells.Add(new DataGridViewTextBoxCell());
                dvRowProfile.Cells[0].Value = AppConfigFile.XmlKey;
                dvRowProfile.Cells[1].Value = AppConfigFile.XmlValue;
                dataGridKeys.Rows.Add(dvRowProfile);
            }
        }

        private void btnLoadConfigFile_Click(object sender, EventArgs e)
        {
            //string configPath = @"C:\Users\Win7\Documents\Visual Studio 2010\Projects\LogPostingService\LogPostingService\App.config";

            try
            {
                if (AppConfigXML.LoadAppConfigXMLFile(configPath))
                {
                    RefreshMainGrid();
                    this.dataGridKeys.Enabled = true;
                    this.dataGridKeys.ReadOnly = true;
                    this.btnLoadConfigFile.Enabled = false;
                    this.btnModifyGrid.Enabled = true;
                    this.setFieldValueFromXml(configPath);
                    // EnableCentralGrid(false);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message + "\tApp.config");
            }
        }

        private void setFieldValueFromXml(string xmlConfigPath)
        {
            string keyCheck = "";

            try
            {
                XmlReader reader = XmlReader.Create(xmlConfigPath);
                //this.txtDataSource.Text = "WIN7\\MSSQL2012";
                //this.txtProfile.Text = "PAYROLLGENIE_PROFILE";
                this.btnTestCon.Enabled = true;

                while (reader.Read())
                {
                    if (reader.Name == "add")
                    {
                        if (reader.GetAttribute("key") != null && reader.GetAttribute("value") != null)
                        {
                            //appConfig = CreateAppConfig(reader.GetAttribute("key"), reader.GetAttribute("value"));
                            //InsertAppConfig(appConfig);
                            keyCheck = reader.GetAttribute("key");
                            switch (keyCheck)
                            {
                                case "DataSource":
                                    foreach (string data in encryptData)
                                    {
                                        if (keyCheck == data)
                                        {
                                            this.txtDataSource.Text = Encrypt.decryptText(reader.GetAttribute("value"));
                                            break;
                                        }
                                        else
                                        {
                                            this.txtDataSource.Text = reader.GetAttribute("value");
                                        }


                                    }
                                    break;
                                case "CentralDBName":
                                    foreach (string data in encryptData)
                                    {
                                        if (keyCheck == data)
                                        {
                                            this.txtProfile.Text = Encrypt.decryptText(reader.GetAttribute("value"));
                                            break;
                                        }
                                        else
                                        {
                                            this.txtProfile.Text = reader.GetAttribute("value");

                                        }

                                    }
                                    break;
                                case "DBNameConfi":
                                    foreach (string data in encryptData)
                                    {
                                        if (keyCheck == data)
                                        {
                                            this.txtConfi.Text = Encrypt.decryptText(reader.GetAttribute("value"));
                                            break;
                                        }
                                        else
                                        {
                                            this.txtConfi.Text = reader.GetAttribute("value");

                                        }

                                    }
                                    break;
                                case "DBNameNonConfi":

                                    foreach (string data in encryptData)
                                    {
                                        if (keyCheck == data)
                                        {
                                            this.txtNonConfi.Text = Encrypt.decryptText(reader.GetAttribute("value"));
                                            break;
                                        }
                                        else
                                        {
                                            this.txtNonConfi.Text = reader.GetAttribute("value");

                                        }

                                    }

                                    break;
                                case "DBNameDTR":

                                    foreach (string data in encryptData)
                                    {
                                        if (keyCheck == data)
                                        {
                                            this.txtDtr.Text = Encrypt.decryptText(reader.GetAttribute("value"));
                                            break;
                                        }
                                        else
                                        {
                                            this.txtDtr.Text = reader.GetAttribute("value");

                                        }

                                    }

                                    break;
                                case "OTAfterMidnight":
                                    if (reader.GetAttribute("value").Trim() == "TRUE" || reader.GetAttribute("value").Trim() == "true")
                                    {
                                        this.chBoxOTAfterMidnight.Checked = true;
                                    }
                                    break;
                                case "DefaultShift":
                                    if (reader.GetAttribute("value").Trim() == "TRUE" || reader.GetAttribute("value").Trim() == "true")
                                    {
                                        this.chBoxDefaultShift.Checked = true;
                                    }
                                    break;
                                case "AutoChangeShift":
                                    if (reader.GetAttribute("value").Trim() == "TRUE" || reader.GetAttribute("value").Trim() == "true")
                                    {
                                        //this.chBoxDefaultShift.Checked = true;
                                        this.chBoxAutoChangeShift.Checked = true;
                                    }
                                    break;
                                case "UserID":

                                    foreach (string data in encryptData)
                                    {
                                        if (keyCheck == data)
                                        {
                                            this.txtSQLUsername.Text = Encrypt.decryptText(reader.GetAttribute("value"));
                                            break;
                                        }
                                        else
                                        {
                                            this.txtSQLUsername.Text = reader.GetAttribute("value");

                                        }

                                    }

                                    break;
                                case "Password":

                                    foreach (string data in encryptData)
                                    {
                                        if (keyCheck == data)
                                        {
                                            this.txtSQLPassword.Text = Encrypt.decryptText(reader.GetAttribute("value"));
                                            break;
                                        }
                                        else
                                        {
                                            this.txtSQLPassword.Text = reader.GetAttribute("value");

                                        }

                                    }

                                    break;
                            }

                        }
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void ColorGrid(DataGridView dgv, Color color)
        {
            DataGridViewCellStyle cellstyle = new DataGridViewCellStyle();
            cellstyle.BackColor = color;

            string columnName = "dataGridValue";

            dgv.Columns[columnName].DefaultCellStyle = cellstyle;
        }

        private void btnSaveGrid_Click(object sender, EventArgs e)
        {
            if (dataGridKeys.IsCurrentCellDirty)
            {
                dataGridKeys.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }

            DataGridViewRowCollection rows = dataGridKeys.Rows;
            AppConfigXML.SaveXMLFile(rows);
            SaveApp();
            EnableCentralControls(false);

        }

        private void btnModifyGrid_Click(object sender, EventArgs e)
        {


            if (btnModifyGrid.Text == "Edit")
            {
                EnableCentralControls(true);

            }
            else
            {
                EnableCentralControls(false);
                AppConfigXML.LoadAppConfigXMLFile(configPath);
                RefreshMainGrid();
            }
        }

        private void ClearGrid(DataGridView dataGridView)
        {
            dataGridView.Rows.Clear();
        }

        private void EnableCentralControls(bool state)
        {
            string columnName = "dataGridValue";
            btnSaveGrid.Enabled = state;
            this.lblMessageTest.Text = "";
            this.dataGridKeys.ReadOnly = !state;
            if (btnModifyGrid.Text != "Cancel")
            {
                btnLoadConfigFile.Enabled = !state;
                gBoxApp.Enabled = state;
            }

            if (state)
            {
                ColorGrid(dataGridKeys, Color.White);
                btnModifyGrid.Text = "Cancel";
            }
            else
            {
                ColorGrid(dataGridKeys, Color.LightGray);
                btnModifyGrid.Text = "Edit";
                this.gBoxApp.Enabled = state;
            }
        }

        private void btnCancelRun_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsProcessRunning)
                {

                    this.btnRunConsole.Enabled = false;
                    this.btnCancelRun.Enabled = false;
                    this.tmrPosting.Stop();
                    this.pBarStatus.Value = 0;
                    this.labelPercent.Text = 0 + "%";
                    this.IsStopWorker = true;
                    if (this.bgwPosting != null)
                    {
                        this.bgwPosting.CancelAsync();
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:" + ex.Message);
            }
        }

        private void SaveApp()
        {
            DataGridViewRowCollection rows = dataGridKeys.Rows;
            AppConfigXML.SaveXMLFile(rows, this.txtDataSource.Text.ToString(), this.txtProfile.Text.ToString(), this.txtNonConfi.Text.ToString(),
                                      this.txtConfi.Text.ToString(), this.txtDtr.Text.ToString(), this.chBoxOTAfterMidnight.Checked,
                                      this.chBoxDefaultShift.Checked, this.chBoxAutoChangeShift.Checked,
                                      this.txtSQLUsername.Text.ToString(), this.txtSQLPassword.Text.ToString());

            AppConfigXML.LoadAppConfigXMLFile(configPath);
            RefreshMainGrid();
        }

        private void btnTestCon_Click(object sender, EventArgs e)
        {

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            DataSet dataset = null;
            String sqlQuery = "";
            this.btnTestCon.Enabled = false;

            try
            {
                DALHelper dal = null;
                if (this.txtDataSource.Text.ToString() != string.Empty && this.txtProfile.Text.ToString() != string.Empty)
                {
                    dal = new DALHelper(this.txtDataSource.Text.ToString());
                }

                paramInfo[0] = new ParameterInfo("@Database", this.txtProfile.Text.ToString(), SqlDbType.VarChar, 20);

                #region SchedulerServiceTimeSetting Query
                sqlQuery = @"SELECT  [Mst_ServiceCode]
                                    ,[Mst_IsMultipleTrigger] 
                               FROM M_ServiceTiming
                              WHERE Mst_IsMultipleTrigger = '1' ";
                #endregion

                dal.OpenDB();
                dal.BeginTransaction();
                dataset = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);
                if (dataset != null)
                {
                    //this.lblMessageTest.Text = "Successfully Connected";
                    //this.lblMessageTest.ForeColor = Color.Green;
                    this.cBoxServiceCode.DataSource = GetListOfService();
                    MessageBox.Show("Successfully Connected");
                    this.btnTestCon.Enabled = true;
                }
                else
                {
                    //this.lblMessageTest.Text = "Error Connecting, Please check config";
                    //this.lblMessageTest.ForeColor = Color.Red;
                    MessageBox.Show("Error connecting to database");
                    this.btnTestCon.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error connecting to database");
                //this.lblMessageTest.Text = "Error Connecting, Please check config";
                //this.lblMessageTest.ForeColor = Color.Red;
                this.btnTestCon.Enabled = true;
                //if (dal != null)
                //{
                //    dal.RollBackTransaction();
                //    dal.CloseDB();
                //}
            }
            finally
            {
                //dal.CommitTransaction();
                //dal.CloseDB();
                //dal.Dispose();
            }
        }

        private void tmrPosting_Tick(object sender, EventArgs e)
        {

            if (!IsProcessRunning)
            {
                if (Globals.Progress > 0)
                {
                    //pBarStatus.Visible = true;
                    //lblProcess.Text = Globals.ProgressProcess;
                    pBarStatus.Value = Convert.ToInt16(Globals.Progress);
                    labelPercent.Text = "" + pBarStatus.Value + "%";
                }
            }
            else
            {
                return;
            }
        }

        private void cBoxServiceCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedItem = this.cBoxServiceCode.SelectedItem.ToString();

            if (selectedItem == "LOGUPLOADING" || selectedItem == "MULTIPOCKETS" || selectedItem == "GENERICEMAIL")
            {
                this.dPickerStart.Enabled = true;
                this.dPickerEnd.Enabled = true;
                this.chBoxLabHours.Visible = false;
                this.dPickerStart.Visible = true;
                this.dPickerEnd.Visible = true;
                this.lblStartDate.Visible = true;
                this.lblEndDate.Visible = true;

            }
            else if (selectedItem == "LABORHOURS" || selectedItem == "GENLABORHOURS")
            {
                this.dPickerStart.Visible = false;
                this.dPickerEnd.Visible = false;
                this.chBoxLabHours.Visible = true;
                this.lblStartDate.Visible = false;
                this.lblEndDate.Visible = false;
            }
            else
            {
                this.dPickerStart.Enabled = false;
                this.dPickerEnd.Enabled = false;
                this.chBoxLabHours.Visible = false;
                this.dPickerStart.Visible = false;
                this.dPickerEnd.Visible = false;
                this.lblStartDate.Visible = false;
                this.lblEndDate.Visible = false;
            }
        }

        private void frmLogPosting_Load(object sender, EventArgs e)
        {
            DataTable dbname = GetProfileDetails();
            for (int i = 0; i < dbname.Rows.Count; i++)
            {
                cmbProfile.Items.Add(dbname.Rows[i][0].ToString());
            }
            label18.Visible = false;
            label19.Visible = false;
            progressBar1.Visible = false;
            btnUpdatePostFlag.Enabled = false;
            txtSearch.Enabled = false;
            cbCheckAll.Enabled = false;
            //cbPosted.Enabled = false;
        }

        public DataTable GetProfileDetails()
        {
            DataTable DBNames = new DataTable();
            DataSet dsTemp = new DataSet();
            string sql = @"SELECT Mpf_DatabaseNo
                                , Mpf_ProfileName
                                , Mpf_ServerName
                                , Mpf_DatabaseName
                                , Mpf_UserID
                                , Mpf_Password
                             FROM M_Profile
                            WHERE Mpf_RecordStatus = 'A'
                            and Mpf_ProfileType in ('P','S')";
            using (DALHelper dal = new DALHelper(Encrypt.decryptText(ConfigurationManager.AppSettings["CentralDBName"].ToString()), false))
            {
                try
                {
                    dal.OpenDB();
                    dsTemp = dal.ExecuteDataSet(sql, CommandType.Text);
                }
                catch
                {

                }
                finally
                {
                    dal.CloseDB();
                }
            }

            //Test connections
            if (dsTemp.Tables.Count > 0 && dsTemp.Tables[0].Rows.Count > 0)
            {
                DBNames.Columns.Add("DBName");
                bool connectionOK = true;
                for (int i = 0; i < dsTemp.Tables[0].Rows.Count; i++)
                {
                    connectionOK = true;
                    using (DALHelper dalTest = new DALHelper(dsTemp.Tables[0].Rows[i]["Mpf_DatabaseName"].ToString(), false))
                    {
                        try
                        {
                            dalTest.OpenDB();
                            dalTest.CloseDB();
                        }
                        catch (Exception)
                        {
                            connectionOK = false;
                        }
                    }

                    if (connectionOK)
                    {
                        DBNames.Rows.Add(DBNames.NewRow());
                        DBNames.Rows[DBNames.Rows.Count - 1]["DBName"] = dsTemp.Tables[0].Rows[i]["Mpf_DatabaseName"].ToString();
                    }
                }
            }

            return DBNames;
        }

        private void btnGenerate(object sender, EventArgs e)
        {
            //dgvResult.AutoGenerateColumns = false;
            //dgvResult.DataSource = Generate(cmbProfile.Text, dateTimePicker2.Text, dateTimePicker1.Text);

            if (bWorkerGetDTR.IsBusy)
            {
                MessageBox.Show("Retrieving record in-progress.");
            }
            else
            {
                MessageBox.Show("Retrieving record may take for a while please wait.");
                if (cmbProfile.Text != string.Empty && dateTimePicker1.Text != string.Empty && dateTimePicker2.Text != string.Empty)
                {
                    AffectedRows = new DataTable();
                    Posted = cbPosted.Checked;
                    grpResullt.Enabled = false;
                    btnUpdatePostFlag.Enabled = false;
                    this.dgvResult.DataSource = null;
                    dgvResult.Columns.Clear();
                    dgvResult.Refresh();
                    //dgvResult.AutoGenerateColumns = false;
                    AllowRefreshGrid = true;
                    SelectedProfile = cmbProfile.Text;
                    StartDate = dateTimePicker2.Text;
                    EndDate = dateTimePicker1.Text;
                }
                else
                {
                    AllowRefreshGrid = false;
                    MessageBox.Show("Profile & Date Range must not be empty!!!");
                }

                bWorkerGetDTR.RunWorkerAsync();
            }
        }

        private void PopulateDGV()
        {
            if (AllowRefreshGrid)
            {
                BeginInvoke(new Function(delegate()
                {
                    lblMessage.Visible = true;
                }));
               
                dsGrid = Generate(SelectedProfile, StartDate, EndDate);
            }
        }

        private DataTable Generate(string Profile, string startdate, string enddate)
        {
            DataTable Result = new DataTable();
            DataSet dsResult = new DataSet();

           string sql = string.Format(@"
                                select cast('False' as bit) as 'Select', 
                                       Tel_IDNo EmployeeId, 
                                       Mem_LastName LastName,
                                       Mem_FirstName FirstName, 
                                       Tel_LogDate Date,
                                       Tel_LogType Type,
                                       Tel_LogTime Time, 
                                       Tel_IsPosted Flag
                                  FROM {0}..T_EmpDTR
                                 INNER JOIN {1}..M_Employee 
                                    ON Tel_IDNo = Mem_IDNo
                                   AND convert(datetime, Tel_LogDate) between '{2}' and '{3}' 
                                   AND Mem_WorkStatus = 'AC'
                                   AND Tel_IsPosted = '{4}'"
                , Encrypt.decryptText(ConfigurationManager.AppSettings["DBNameDTR"].ToString()),Profile,startdate,enddate, (Posted ? "1" : "0"));

            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    dsResult = dal.ExecuteDataSet(sql, CommandType.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            
            return  Result = dsResult.Tables[0];
        }

        private void dgvResult_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {

            ////Temp comment
            //if (e.ColumnIndex != 0) e.Cancel = true;
            //RowCount();
        }

        private void btn_UpdatePostFlag(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            dt = AffectedRows;

            if (dt == null || dt.Rows.Count <= 0)
            {
                MessageBox.Show("Please select record to update.");
                return;
            }
            else if (dt.Select("Select = True").Length == 0)
            {
                MessageBox.Show("Please select record to update.");
                return;
            }

            bool Successful = PostFlagUpdating(dt);

            if (Successful)
            {
                MessageBox.Show("Updating Successful. Reloading data.");
                {
                    btnGenerate(sender, e);
                }
            }
        }

        private DataTable GetSelectedRows()
        {
            //Temp comment

            DataTable dt = new DataTable();
            if ((AffectedRows != new DataTable()) && AffectedRows != null  && AffectedRows.Rows.Count > 0)
            {
                //Do nothing means use the affected row table
                dt = AffectedRows;
            }
            else
            {
                foreach (DataGridViewColumn column in dgvResult.Columns)
                {
                    DataColumn dc = new DataColumn(column.Name.ToString());
                    dt.Columns.Add(dc);
                }
            }

            for (int i = 0; i < dgvResult.Rows.Count; i++)
            {
                DataGridViewRow row = dgvResult.Rows[i];
                if (Convert.ToBoolean(row.Cells["Select"].Value) == true)
                    //Commented this take too long.
                    //&& 
                    //dt.Select(string.Format("EmployeeId = '{0}' AND Date = '{1}' AND Time = '{2}' AND Type = '{3}' AND Select = True"
                    //                        , row.Cells["EmployeeId"].Value
                    //                        , row.Cells["Date"].Value
                    //                        , row.Cells["Time"].Value
                    //                        , row.Cells["Type"].Value)).Length == 0)
                {
                    DataRow dr = dt.NewRow();
                    for (int j = 0; j < dgvResult.Columns.Count; j++)
                    {
                        dr[j] = (row.Cells[j].Value == null) ? "" : row.Cells[j].Value.ToString();
                    }
                    dt.Rows.Add(dr);
                }
            }

            //Wans 09252014 Added if selected already has been unchecked.
            foreach (DataRow dr in AffectedRows.Rows)
            { 
                string EmpID =  dgvResult.SelectedRows[0].Cells["EmployeeId"].Value.ToString();
                string Date = dgvResult.SelectedRows[0].Cells["Date"].Value.ToString();
                string Time = dgvResult.SelectedRows[0].Cells["Time"].Value.ToString();
                string Type = dgvResult.SelectedRows[0].Cells["Type"].Value.ToString();
                string Select = dgvResult.SelectedRows[0].Cells["Select"].Value.ToString();
                if (dr["EmployeeId"].ToString() == EmpID && 
                    dr["Date"].ToString() == Date &&
                    dr["Time"].ToString() == Time &&
                    dr["Type"].ToString() == Type &&
                    dr["Select"].ToString() == "True" &&
                    Select == "False")
                { 
                    //Item has beed unchecked
                    dr["Select"] = "False";
                }
            
            }
            return dt;
        }

        private void RowCount()
        {
            AffectedRows = GetSelectedRows();
            lblRowCount.Text = AffectedRows.Rows.Count.ToString() + " of " + dgvResult.Rows.Count.ToString() + " for Processing";

            if (AffectedRows.Rows.Count > 0)
            {
                btnUpdatePostFlag.Enabled = true;
            }
            else
            {
                btnUpdatePostFlag.Enabled = false;
            }
        }

        private bool PostFlagUpdating(DataTable forUpdate)
        {
            string sql = "";
            int rows = 0;
            int Updating = 0;
            string DTR = Encrypt.decryptText(ConfigurationManager.AppSettings["DBNameDTR"].ToString());

            foreach (DataRow item in forUpdate.Select("Select = True"))
            {
                 sql += string.Format(@"
update {0}..T_EmpDTR set Tel_IsPosted = 0 where Tel_IDNo = '{1}' and Tel_LogDate = '{2}' and Tel_LogType = '{3}'  and Tel_LogTime = '{4}'
"
                                            , DTR
                                            , item["EmployeeId"].ToString()
                                            , item["Date"].ToString()
                                            , item["Type"].ToString()
                                            , item["Time"].ToString());

                 ++rows;

                 if (rows > 30) //Batch per 30 records
                 {
                     using (DALHelper dal = new DALHelper())
                     {
                         try
                         {
                             dal.OpenDB();
                             Updating = dal.ExecuteNonQuery(sql, CommandType.Text);
                         }
                         catch (Exception ex)
                         {
                             MessageBox.Show(ex.ToString());
                             return false;
                         }
                         finally
                         {
                             dal.CloseDB();
                         }
                     }

                     sql = "";

                     rows = 0;
                 }
            }

            //Wans added 20140925 Final post flag update by bathc
            if (!string.IsNullOrEmpty(sql))
            {
                using (DALHelper dal = new DALHelper())
                {
                    try
                    {
                        dal.OpenDB();
                        Updating = dal.ExecuteNonQuery(sql, CommandType.Text);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                        return false;
                    }
                    finally
                    {
                        dal.CloseDB();
                    }
                }
            }

            return true;
        }
                
        private void cbPosted_Click(object sender, EventArgs e)
        {
            //PostedFilter();
            //RowCount();
        }

        private void cbPostedLoad()
        {
            //PostedFilter();

            //Temp comment
            //RowCount();
        }

        private void PostedFilter()
        {
            //Temp comment
            //if (cbPosted.Checked == true)
            //{
            //    Filter1 = "Flag = True";
            //}
            //else
            //{
            //    Filter1 = "Flag in (True,False)";
            //}

            //dgvFiltering(Filter1, Filter2);
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {

            //Temp comment
            if (string.IsNullOrEmpty(txtSearch.Text))
            {
                this.dgvResult.DataSource = null;
                dgvResult.Columns.Clear();
                dgvResult.Refresh();
                dgvResult.DataSource = dsGrid;
            }
            else
            {
                Filter1 = string.Format("Flag = {0}", cbPosted.Checked ? "True" : "False");
                Filter2 = " AND (EmployeeId like '%" + txtSearch.Text + "%' OR LastName like '%" + txtSearch.Text + "%' OR FirstName like '%" + txtSearch.Text + "%' OR Date like '%" + txtSearch.Text + "%' OR Type like '%" + txtSearch.Text + "%' OR Time like '%" + txtSearch.Text + "%')";
                dgvFiltering(Filter1, Filter2);
                //RowCount(); //Wans 09252014 No need to recheck select on filtering.
            }
        }

        private void dgvFiltering(string filter1, string filter2)
        {
            string filter = filter1;
            if (filter2 != string.Empty)
                filter += filter2;
            DataView dv = dsGrid.DefaultView;
            try
            {
                dv.RowFilter = filter;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            dgvResult.DataSource = dv;
            dgvResult.Refresh();
        }

        private void cbCheckAll_Click(object sender, EventArgs e)
        {

            //Temp comment
            this.Cursor = Cursors.WaitCursor;
            CheckAllRowCheckboxes(cbCheckAll.Checked);
            this.Cursor = Cursors.Default;
            if (cbCheckAll.Checked)
            {
                RowCount();
            }
            else
            { 
                //Wans Added 20140925 Only unchecking method from datagrid to affectedtable
                if (AffectedRows != new DataTable() && AffectedRows != null && AffectedRows.Rows.Count > 0)
                {
                    for (int i = 0; i < dgvResult.Rows.Count; i++)
                    {
                        DataGridViewRow row = dgvResult.Rows[i];
                        if (Convert.ToBoolean(row.Cells["Select"].Value) == false)
                            //Commented Take too long.
                            //&&
                            //AffectedRows.Select(string.Format("EmployeeId = '{0}' AND Date = '{1}' AND Time = '{2}' AND Type = '{3}' AND Select = True"
                            //                        , row.Cells["EmployeeId"].Value
                            //                        , row.Cells["Date"].Value
                            //                        , row.Cells["Time"].Value
                            //                        , row.Cells["Type"].Value)).Length > 0)
                        {
                            
                            DataRow[] drr =  AffectedRows.Select(string.Format("EmployeeId = '{0}' AND Date = '{1}' AND Time = '{2}' AND Type = '{3}' AND Select = True"
                                                    , row.Cells["EmployeeId"].Value
                                                    , row.Cells["Date"].Value
                                                    , row.Cells["Time"].Value
                                                    , row.Cells["Type"].Value));

                            foreach (DataRow dr in drr)
                            {
                                dr.Delete();
                            }

                            AffectedRows.AcceptChanges();
                        }
                    }
                }
            }
        }

        private void CheckAllRowCheckboxes(bool bChecked)
        {
            //Temp comment
            foreach (DataGridViewRow row in dgvResult.Rows)
            {
                if (bChecked == true)
                    row.Cells[0].Value = "True";
                else
                    row.Cells[0].Value = "False";
            }
        }

        private void dgvResult_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            dgvResult.EndEdit();
            RowCount();
        }

        private void bWorkerGetDTR_DoWork(object sender, DoWorkEventArgs e)
        {
            PopulateDGV();
        }

        private void bWorkerGetDTR_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            BeginInvoke(new Function(delegate()
                {
                    lblMessage.Visible = false;
                    if (dsGrid.Rows.Count != 0)
                    {
                        dgvResult.DataSource = dsGrid;
                        dgvResult.ReadOnly = false;
                        txtSearch.Enabled = true;
                        cbCheckAll.Enabled = true;
                        //cbPosted.Enabled = true;
                        //this.cbPosted.Checked = true;
                        //this.cbCheckAll.Checked = false;
                        this.btnUpdatePostFlag.Enabled = false;
                        cbPostedLoad();
                        MessageBox.Show("Retrieving record is complete.");
                        grpResullt.Enabled = true;
                        btnUpdatePostFlag.Enabled = true;
                    }
                    else
                    {
                        MessageBox.Show("No Records Found!");
                    }
                }));
        }
    }
}
