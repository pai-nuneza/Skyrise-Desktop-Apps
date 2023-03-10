using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Xml;
using System.Text;
using System.Windows.Forms;
using CommonLibrary;
using Inventory.DAL;
using Inventory.BLogic;
using System.Diagnostics;
using LogPostingService;
using UploadDownloadSchedulerConsole;
using System.Threading;
using System.Windows;
using System.Configuration;
namespace LogPostingService
{
    public partial class frmLogPosting : Form
    {
        private ConfigXML ConfigXML = new ConfigXML();
        private LogPostingService.AppConfigXML AppConfigXML = new AppConfigXML();
        private BackgroundWorker bgwPosting;
        private Boolean IsProcessRunning = false;
        private bool IsStopWorker = false;
        private Thread thread;
        private delegate void Function();
        private string configPath = @"LogPostingService.exe.config";
        private string runConsole = "UploadDownloadSchedulerConsole.exe";
        private string[] encryptData = new string[20] { "ReportServerUsername"
                                                    , "ReportServerPassword"
                                                    , "DataSource"
                                                    , "DBNameNonConfi"
                                                    , "DBNameConfi"
                                                    , "DBNameDTR"
                                                    , "UserID"
                                                    , "Password"
                                                    , "ProfileDBName"
                                                    , "PROXServer"
                                                    , "PROXDB"
                                                    , "PROXUserId"
                                                    , "PROXPassword"
                                                    , "AmanoServer1"
                                                    , "AmanoDB1"
                                                    , "AmanoUserId1"
                                                    , "AmanoPassword1"
                                                    , "AmanoServer2"
                                                    , "AmanoUserId2"
                                                    , "AmanoPassword2"};
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
                sqlQuery = @"SELECT  [Sst_ServiceCode]
                                    ,[Sst_MultipleTrigger] 
                               FROM T_SchedulerServiceTimeSetting
                              WHERE Sst_MultipleTrigger = '1' ";
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
            DALHelper dal = new DALHelper(this.txtDataSource.Text.ToString(), this.txtProfile.Text.ToString());
            String sqlQuery = "";
            List<string> serviceCodes = new List<string>();
            DataTable dTable = new DataTable();
            
            try
            {
                #region  SchedulerServiceTimeSetting Query
                    sqlQuery = @"SELECT  [Sst_ServiceCode]
                                        ,[Sst_MultipleTrigger] 
                                   FROM T_SchedulerServiceTimeSetting
                                  WHERE Sst_MultipleTrigger = '1' ";
                #endregion

                dal.OpenDB();
                dataSet = dal.ExecuteDataSet(sqlQuery, CommandType.Text);
                if(dataSet != null)
                {
                    dTable = dataSet.Tables[0];
                }
                #region Add all Service code
                    if(dTable != null)
                    {
                        foreach (DataRow dr in dTable.Rows)
                        {
                            serviceCodes.Add(dr["Sst_ServiceCode"].ToString());
                        }
                    }
                #endregion

            }
            catch(Exception ex)
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
            if(cBoxServiceCode.DataSource != null)
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

            if(this.cBoxServiceCode.InvokeRequired && this.dPickerStart.InvokeRequired && this.dPickerEnd.InvokeRequired)
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
            catch(Exception ex)
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
            MessageBox.Show("Service Code Process Completed");
            this.pBarStatus.Value = Convert.ToInt16(Globals.Progress);
            this.pBarStatus.Value = 100;
            this.btnRunConsole.Enabled = true;
            this.btnCancelRun.Enabled = false;
            this.labelPercent.Text = "100 %";
            this.IsStopWorker = false;
            
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
            catch(Exception ex)
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

                while(reader.Read())
                {
                    if(reader.Name == "add")
                    {
                        if (reader.GetAttribute("key") != null && reader.GetAttribute("value") != null)
                        {
                            //appConfig = CreateAppConfig(reader.GetAttribute("key"), reader.GetAttribute("value"));
                            //InsertAppConfig(appConfig);
                            keyCheck = reader.GetAttribute("key");
                            switch (keyCheck)
                            {
                                case "DataSource":
                                    foreach(string data in encryptData)
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
                                case "ProfileDBName":
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
            }catch(Exception ex)
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
            if(dataGridKeys.IsCurrentCellDirty)
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
                if(!IsProcessRunning)
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
            catch(Exception ex)
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
                if(this.txtDataSource.Text.ToString() != string.Empty && this.txtProfile.Text.ToString() != string.Empty)
                {
                    dal = new DALHelper(this.txtDataSource.Text.ToString(), this.txtProfile.Text.ToString());
                }
                
                paramInfo[0] = new ParameterInfo("@Database", this.txtProfile.Text.ToString(), SqlDbType.VarChar, 20);

                #region SchedulerServiceTimeSetting Query
                sqlQuery = @"SELECT  [Sst_ServiceCode]
                                    ,[Sst_MultipleTrigger] 
                               FROM T_SchedulerServiceTimeSetting
                              WHERE Sst_MultipleTrigger = '1' ";
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
      
    }
}
