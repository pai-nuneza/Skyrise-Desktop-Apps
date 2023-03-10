using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Drawing;
using CommonLibrary;
using System.Configuration;
using Payroll.DAL;
using System.Data;
using System.Windows.Forms;

namespace TimeKeepingManager.Con
{
    public class DeviceDataUploadManager : DeviceDataObjectHelper
    {
        /// <summary>
        /// Upload Engine for RFID/Biometric Device 
        /// Database and Device Mangement
        /// </summary>
        
        /// <summary>
        /// ZKSofware Finger Print RFID Reader objects
        /// </summary>
        
        #region constructor
        private DALHelper _DALServer;
        //public zkemkeeper.CZKEMClass axCZKEM1 = new zkemkeeper.CZKEMClass(); // use declaration for GUI
        public zkemkeeper.CZKEM BioDataHelper = new zkemkeeper.CZKEM(); // use declaration for console
        //public zkemkeeper.CZKEMClass BioDataHelper = new zkemkeeper.CZKEMClass(); // use declaration for GUI
        private bool bIsConnected = false;//the boolean value identifies whether the device is connected
        private int iMachineNumber = 1;//the serial number of the device.After connecting the device ,this value will be changed.
        private string _profile = string.Empty;
        private bool _showmsg = false;
        DeviceDataDownloadManager _DownloadEngine = new DeviceDataDownloadManager("");

        public DeviceDataUploadManager(String Profile)
        {
            _DALServer = new DALHelper(Profile,1);
            _profile = Profile;
            //Added for face data
        }
        public DeviceDataUploadManager(String Profile, bool ShowMsg)
        {
            //_DALServer = new DALHelper(DALHelper.ConnectionSource.Server);
            _DALServer = new DALHelper(Profile, 1);
            _profile = Profile;
            _showmsg = ShowMsg;
        }
        #endregion

        #region Methods

        public int UploadDatabaseFingerPrinttoDevice(int iMachineNumber, string T_TableName)
        {
            int Affected = -1;
            try
            {
                if (DeleteDeviceData(iMachineNumber, 5)) //5 delete Data and FP Template
                {
                    DataTable EmployeePicture = new DataTable();
                    Affected = UploadDatabaseFingerPrinttoDeviceBatch(GetDatabasePerTableFingerPrintData(T_TableName, ref EmployeePicture), iMachineNumber, EmployeePicture, T_TableName);
                    EmployeePicture.Dispose();
                }
                else
                {
                    Affected = ErrCode.DeviceErrDel; // unable to delete
                }
            }
            catch (Exception e) { _DownloadEngine.ExeptionCatcher(e, "UploadEmployee"); }
            return Affected;
        }

        #endregion

        #region Device Data Handling
        //Adding Employee Data by batch
        public int UploadDatabaseFingerPrinttoDeviceBatch(DataTable dt, int iMachineNumber, DataTable EmployeePicture, String TableName)
        {
            #region Private Variables
            int Affected = 0;
            int ErrorCode = 0;
            string EnrolledIDNumber = "";
            string EnrolledName = "";
            int FingerIndex = 0;
            string FingerPrintTemplate = "";
            Int32 Privilege = 0;
            string Password = "";
            bool Enabled = false;
            int Flag = 1;
            string RFIDCardNumber = "";
            //Veriface
            string FaceDataTemplate = "";
            Int32 FaceDataLength = 0;
            Int32 FaceIndex = 50;
            int iUpdateFlag = 1;
            #endregion

            try
            {
                BioDataHelper.EnableDevice(iMachineNumber, false);
                
                if (DeviceVersions.IsVeriface ? true : BioDataHelper.BeginBatchUpdate(iMachineNumber, iUpdateFlag))//create memory space for batching data
                {
                    string sLastEnrollNumber = "";//the former enrollnumber you have upload(define original value as 0)
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        EnrolledIDNumber = dt.Rows[i]["EmployeeID"].ToString();
                        _DownloadEngine.GetReferenceID(EnrolledIDNumber, out EnrolledName);
                        FingerIndex = Convert.ToInt16(dt.Rows[i]["FingerIndex"]);
                        FingerPrintTemplate = dt.Rows[i]["Template"].ToString();
                        Privilege = Convert.ToInt16(dt.Rows[i]["Privilege"]);
                        Password = dt.Rows[i]["Password"].ToString();
                        Enabled = Convert.ToBoolean(dt.Rows[i]["Enabled"]);
                        Flag = Convert.ToInt16(dt.Rows[i]["Flag"]);
                        RFIDCardNumber = dt.Rows[i]["RFID"].ToString();
                        FaceDataTemplate = dt.Rows[i]["FaceData"].ToString();
                        FaceDataLength = FaceDataTemplate.Length;
                        //if(sdwEnrollNumber=="10141")
                        //    iFlag=iFlag;

                        if (BioDataHelper.SetStrCardNumber(RFIDCardNumber))//Before you using function SetUserInfo,set the card number to make sure you can upload it to the device
                        {
                            bool success = true;
                        }

                        if (EnrolledIDNumber != sLastEnrollNumber)//identify whether the user information(except fingerprint templates) has been uploaded
                        {
                            //Upload the 9.0 or 10.0 fingerprint arithmetic templates to the device(in strings) in batches.
                            //Only TFT screen devices with firmware version Ver 6.60 version later support function "SetUserTmpExStr" and "SetUserTmpEx".
                            //While you are using 9.0 fingerprint arithmetic and your device's firmware version is under ver6.60,you should use the functions "SSR_SetUserTmp" or 
                            //"SSR_SetUserTmpStr" instead of "SetUserTmpExStr" or "SetUserTmpEx" in order to upload the fingerprint templates.

                            if (DeviceVersions.IsBlackAndWhiteType ? BioDataHelper.SetUserInfo(iMachineNumber, Convert.ToInt32(EnrolledIDNumber), EnrolledName, Password, Privilege, Enabled) 
                                                                   : BioDataHelper.SSR_SetUserInfo(iMachineNumber, EnrolledIDNumber, EnrolledName, Password, Privilege, Enabled))//upload user information to the memory
                            {
                                Affected = ++Affected;
                                //version of 6.60 onward build
                                //Trying New Firmware Version
                                try
                                {
                                    try
                                    {
                                        //Load HRC Picture to device
                                        if (!DeviceVersions.IsBlackAndWhiteType)
                                        {
                                            BinaryImageAnalyzer AnalyzeImage = new BinaryImageAnalyzer(TableName);
                                            if (EnrolledIDNumber == "1105")
                                            {
                                                string TestBreak = "";
                                            }
                                            string ImgFile = AnalyzeImage.BinaryToImageFile(GetPicture(ref EmployeePicture, EnrolledIDNumber));
                                            if (!string.IsNullOrEmpty(ImgFile))
                                            {
                                                if (BioDataHelper.SendFile(iMachineNumber, ImgFile))
                                                {
                                                    bool FileisLoaded = true;
                                                }
                                            }
                                        }
                                    }
                                    catch
                                    { }

                                    if (DeviceVersions.IsVeriface)
                                    {
                                        //Upload face templates information to the device
                                        if (!string.IsNullOrEmpty(FaceDataTemplate))
                                        {
                                            BioDataHelper.SetUserFaceStr(iMachineNumber, EnrolledIDNumber, FaceIndex, FaceDataTemplate, FaceDataLength);
                                        }
                                    }

                                    if (!string.IsNullOrEmpty(FingerPrintTemplate))
                                    {
                                        //Upload Finger print templates to the device
                                        if (BioDataHelper.SetUserTmpExStr(iMachineNumber, EnrolledIDNumber, FingerIndex, Flag, FingerPrintTemplate))
                                        {
                                            string sucess = "";
                                        }
                                        else
                                        {
                                            try
                                            {
                                                //Try Old version uploading of finger print
                                                if (BioDataHelper.SetUserTmpStr(iMachineNumber, Convert.ToInt32(EnrolledIDNumber), FingerIndex, FingerPrintTemplate))
                                                {
                                                    string success = "";
                                                }
                                            }
                                            catch
                                            { }
                                        }
                                    }
                                    
                                }
                                catch (Exception e) { _DownloadEngine.ExeptionCatcher(e, string.Format("UploadBatchToDevice : ID : {0}", EnrolledIDNumber)); }

                                Flag = 1;

                                #region Commented Code
                                //Function Roll back Version 05/30/2012 - previous version of 6.60 build
                                //Trying Old Firmware Version below 6.60
                                /*try 
                                {
                                    axCZKEM1.SetUserTmpStr(iMachineNumber, Convert.ToInt32(sdwEnrollNumber), idwFingerIndex, sTmpData); // No iFlag
                                }
                                catch (Exception e)
                                {
                                    _DownloadEngine.ExeptionCatcher(e, string.Format("UploadBatchToDevice : ID : {0}", sdwEnrollNumber));
                                }*/
                                #endregion

                            }
                            else
                            {
                                BioDataHelper.GetLastError(ref ErrorCode);
                                if (_showmsg)
                                    MessageBox.Show("Operation failed,ErrorCode=" + ErrorCode.ToString(), "Error");
                                continue;
                            }
                        }
                        else//the current fingerprint and the former one belongs the same user,that is ,one user has more than one template
                        {
                            Affected = ++Affected;
                            //version of 6.60 onward build
                            //Trying New Firmware Version
                            try
                            {
                                if (!string.IsNullOrEmpty(FingerPrintTemplate))
                                {
                                    //Upload Finger print templates to the device
                                    if (BioDataHelper.SetUserTmpExStr(iMachineNumber, EnrolledIDNumber, FingerIndex, Flag, FingerPrintTemplate))
                                    {
                                        string sucess = "";
                                    }
                                    else
                                    {
                                        try
                                        {
                                            //Try Old version uploading of finger print
                                            if (BioDataHelper.SetUserTmpStr(iMachineNumber, Convert.ToInt32(EnrolledIDNumber), FingerIndex, FingerPrintTemplate))
                                            { 
                                                string sucess = ""; 
                                            }
                                        }
                                        catch
                                        { }
                                    }
                                }

                            }
                            catch (Exception e) { _DownloadEngine.ExeptionCatcher(e, string.Format("UploadBatchToDevice : ID : {0}", EnrolledIDNumber)); }

                            Flag = 1;

                            #region Commented Code
                            //Function Roll back Version 05/30/2012 - previous version of 6.60 build
                            //Trying Old Firmware Version below 6.60
                            /*try 
                            {
                                axCZKEM1.SetUserTmpStr(iMachineNumber, Convert.ToInt32(sdwEnrollNumber), idwFingerIndex, sTmpData); // No iFlag
                            }
                            catch (Exception e)
                            {
                                _DownloadEngine.ExeptionCatcher(e, string.Format("UploadBatchToDevice : ID : {0}", sdwEnrollNumber));
                            } */
                            #endregion

                            ErrCode.DeviceDupTmpl = ++ErrCode.DeviceDupTmpl;
                        //    //if (_showmsg)
                        //    //    MessageBox.Show(string.Format(MSG.DeviceDuplicateTmpl, sdwEnrollNumber, idwFingerIndex), MSG.Failed);
                        }

                        sLastEnrollNumber = EnrolledIDNumber;//change the value of iLastEnrollNumber dynamicly

                        DeviceGlobals.Progress = ((Convert.ToDouble(i) + 1) / Convert.ToDouble(dt.Rows.Count)) * 100;
                    }
                }

                if(!DeviceVersions.IsVeriface) BioDataHelper.BatchUpdate(iMachineNumber);//upload all the information in the memory

            }
            catch (Exception e)
            {
                _DownloadEngine.ExeptionCatcher(e, "UploadDatabaseFingerPrinttoDeviceBatch");
            }

            BioDataHelper.RefreshData(iMachineNumber);//the data in the device should be refreshed

            BioDataHelper.EnableDevice(iMachineNumber, true);

            return Affected;
        }
        
        //Adding Individual Employee Data to device 
        public void UploadDatabaseFingerPrinttoDeviceBatchIndividual(DeviceEmployeeObjects ZkObj, int iMachineNumber)
        {
            try
            {
                int idwErrorCode = 0;
                string sName = "";
                _DownloadEngine.GetReferenceID(ZkObj.Trf_EmployeeID, out sName);
                
                BioDataHelper.EnableDevice(iMachineNumber, false);

                if (BioDataHelper.SSR_SetUserInfo(iMachineNumber, ZkObj.Trf_EmployeeID, sName, ZkObj.Trf_Password, ZkObj.Trf_Privilege, ZkObj.Trf_Enabled))//upload user information to the device
                {
                    BioDataHelper.SetUserTmpExStr(iMachineNumber, ZkObj.Trf_EmployeeID, ZkObj.Trf_FingerIndex, ZkObj.Trf_Flag, ZkObj.Trf_Template);//upload templates information to the device
                }
                else
                {
                    BioDataHelper.GetLastError(ref idwErrorCode);
                  
                    BioDataHelper.EnableDevice(iMachineNumber, true);

                    return;
                }
            }
            catch (Exception e)
            {
                _DownloadEngine.ExeptionCatcher(e, "UploadDatabaseFingerPrinttoDeviceBatchIndividual");
            }

            BioDataHelper.RefreshData(iMachineNumber);//the data in the device should be refreshed
            
            BioDataHelper.EnableDevice(iMachineNumber, true);
        }

        //Deleting Individual Employee Data from device
        public void DeleteDeviceIndividualUserData( int iMachineNumber, string sUserID)
        {
            try
            {
                int idwErrorCode = 0;
                int iBackupNumber = 0;

                if (BioDataHelper.SSR_DeleteEnrollData(iMachineNumber, sUserID, iBackupNumber))
                {
                    BioDataHelper.RefreshData(iMachineNumber);//the data in the device should be refreshed
                }
                else
                {
                    BioDataHelper.GetLastError(ref idwErrorCode);
                }
            }
            catch (Exception e)
            {
                _DownloadEngine.ExeptionCatcher(e, "DeleteDeviceIndividualUserData");
            }
        }
        
        //Deleting Individual Employee Data and Finger Index from device
        public void DeleteDeviceIndividualFingerPrintData(int iMachineNumber, string sUserID, int iFingerIndex)
        {
            try
            {
                int idwErrorCode = 0;
                if (BioDataHelper.SSR_DelUserTmpExt(iMachineNumber, sUserID, iFingerIndex))
                {
                    BioDataHelper.RefreshData(iMachineNumber);//the data in the device should be refreshed
                }
                else
                {
                    BioDataHelper.GetLastError(ref idwErrorCode);
                }
            }
            catch (Exception e)
            {
                _DownloadEngine.ExeptionCatcher(e, "DeleteDeviceIndividualFingerPrintData");
            }
        }

        //Clear all the fingerprint templates in the device(While the parameter DataFlag  of the Function "ClearData" is 2 )
        //Delete all the user information in the device,while the related fingerprint templates will be deleted either. 
        //(While the parameter DataFlag  of the Function "ClearData" is 5 )

        //iDataFlag Value
        //1. Attendance record 
        //2. Fingerprint template data 
        //3. None 
        //4. Operation record 
        //5. User information
        public bool DeleteDeviceData(int iMachineNumber,int iDataFlag)
        {
            bool success = false;
            try
            {
                int idwErrorCode = 0;
                if (BioDataHelper.ClearData(iMachineNumber, iDataFlag))
                {
                    BioDataHelper.RefreshData(iMachineNumber);//the data in the device should be refreshed
                    success = true;
                }
                else
                {
                    BioDataHelper.GetLastError(ref idwErrorCode);
                }
            }
            catch (Exception e)
            {
                _DownloadEngine.ExeptionCatcher(e, "DeleteDeviceData");
            }
            return success;
        }

        //Device Connections
        public bool ConnecttoDevice(String IP, String Port)
        {
            try
            {
                TimeKeepingManager.Con.Logger _log = new TimeKeepingManager.Con.Logger();
                BioDataHelper.PullMode = 1;
                bIsConnected = BioDataHelper.Connect_Net(IP, Convert.ToInt32(Port));
                //try Rs232 Connection
                if (!bIsConnected)
                {
                    DeviceDataDownloadManager _downloadengine = new DeviceDataDownloadManager(_profile);
                    bIsConnected = ConnecttoDeviceRS232(
                             _downloadengine.GetDevicePropertyfromDatabaseList(IP).Rows[0]["DEVICERSCOM"].ToString(), 1,
                             _downloadengine.GetDevicePropertyfromDatabaseList(IP).Rows[0]["DEVICERATE"].ToString());
                }

                if (bIsConnected == true)
                {

                    iMachineNumber = 1;//In fact,when you are using the tcp/ip communication,this parameter will be ignored,that is any integer will all right.Here we use 1.
                    BioDataHelper.RegEvent(iMachineNumber, 65535);//Here you can register the realtime events that you want to be triggered(the parameters 65535 means registering all)
                    _log.WriteLog(Application.StartupPath, LogGlobal.RFFP, string.Format("ConnecttoDevice : {0}:{1}", IP, Port), "Connected", true);
                }
                else
                {
                    bIsConnected = false;
                    _log.WriteLog(Application.StartupPath, LogGlobal.RFFP, string.Format("FailConnecttoDevice : {0}:{1}", IP, Port), "Disconnected", true);
                }

                DeviceGlobals.isDeviceCon = bIsConnected;
            }
            catch (Exception e)
            {
                _DownloadEngine.ExeptionCatcher(e, "ConnecttoDevice");
            }
            return DeviceGlobals.isDeviceCon;
        }

        public bool ConnecttoDeviceRS232(string RS232COM, int iMachineNumber, string RsBaudRate)
        {
            try
            {
                RS232COM = RS232COM.Replace("COM", "").Trim();
                bool bIsConnected = BioDataHelper.Connect_Com(Convert.ToInt16(RS232COM), iMachineNumber, Convert.ToInt32(RsBaudRate));
                if (bIsConnected == true)
                {
                    BioDataHelper.RegEvent(iMachineNumber, 65535);//Here you can register the realtime events that you want to be triggered(the parameters 65535 means registering all)
                }
                else
                {
                    bIsConnected = false;
                }

                DeviceGlobals.isDeviceCon = bIsConnected;
            }
            catch (Exception e)
            {
                _DownloadEngine.ExeptionCatcher(e, "ConnecttoDeviceRS");
            }
            return DeviceGlobals.isDeviceCon;
        }

        public bool DisconnectDevice()
        {
            if (DeviceGlobals.isDeviceCon)
                BioDataHelper.Disconnect();
            DeviceGlobals.isDeviceCon = false;
            return DeviceGlobals.isDeviceCon;
        }

        private DataRow GetPicture(ref DataTable dtPicture, string EmployeeID)
        {
            DataRow dr = null;
            foreach (DataRow drPic in dtPicture.Rows)
            {
                if (drPic["EmployeeID"].ToString().Trim() == EmployeeID)
                {
                    dr = drPic;
                    break;
                }
            }
            return dr;
        }
        #endregion

        #region UploadEngineBL

        public DataTable GetDatabasePerTableFingerPrintData(String T_TableName, ref DataTable EmployeePicture)
        {
            DataSet ds = new DataSet();
            DataTable dt;
            string query = String.Format(Payroll.BLogic.CommonDeviceQueries.SelectAllEmployeeData(T_TableName));
            string queryPicture = String.Format(Payroll.BLogic.CommonDeviceQueries.SelectEmployeePicture(T_TableName));
            try
            {
                _DALServer.OpenDB();
                _DALServer.BeginTransaction();
                ds = _DALServer.ExecuteDataSet(query, CommandType.Text);
                _DALServer.CommitTransaction();
                try
                {
                    _DALServer.BeginTransaction();
                    EmployeePicture = _DALServer.ExecuteDataSet(queryPicture, CommandType.Text).Tables[0];
                    _DALServer.CommitTransaction();
                }
                catch
                { }
            }
            catch (Exception e)
            { 
                _DALServer.RollBackTransaction();
                _DownloadEngine.ExeptionCatcher(e, "GetDatabasePerTableFingerPrintData");
            }
            finally
            { 
                _DALServer.CloseDB(); 
            }
            dt = ds.Tables[0];
            ds.Dispose();
            return dt;
        }

        public DataTable GetDatabasePerTableWithDeviceID(String T_TableName, String DeviceID)
        {
            DataSet ds = new DataSet();
            DataTable dt;
            string query = String.Format(Payroll.BLogic.CommonDeviceQueries.SelectEmployeeFingerPrintData(T_TableName, DeviceID, "SA"));
            //string queryPicture = String.Format(Payroll.BLogic.CommonDeviceQueries.SelectEmployeePicture(T_TableName));
            try
            {
                _DALServer.OpenDB();
                _DALServer.BeginTransaction();
                ds = _DALServer.ExecuteDataSet(query, CommandType.Text);
                _DALServer.CommitTransaction();
                //try
                //{
                //    _DALServer.BeginTransaction();
                //    EmployeePicture = _DALServer.ExecuteDataSet(queryPicture, CommandType.Text).Tables[0];
                //    _DALServer.CommitTransaction();
                //}
                //catch
                //{ }
            }
            catch (Exception e)
            {
                _DALServer.RollBackTransaction();
                _DownloadEngine.ExeptionCatcher(e, "GetDatabasePerTableFingerPrintData");
            }
            finally
            {
                _DALServer.CloseDB();
            }
            dt = ds.Tables[0];
            ds.Dispose();
            return dt;
        }

        public int GetDatabasePerTableFingerPrintDataCount(String T_TableName)
        {
            int Count = 0;
            DataSet ds = new DataSet();
            DataTable dt;
            string query = String.Format(@"SELECT Count([Trf_EmployeeID]) AS COUNT FROM [{0}]", T_TableName);
            try
            {
                _DALServer.OpenDB();
                _DALServer.BeginTransaction();
                ds = _DALServer.ExecuteDataSet(query);
                dt = ds.Tables[0];
                Count = Convert.ToInt16(dt.Rows[0]["COUNT"]);
                _DALServer.CommitTransaction();
            }
            catch (Exception e)
            {
                _DALServer.RollBackTransaction();
                _DownloadEngine.ExeptionCatcher(e, "GetDatabasePerTableFingerPrintDataCount");
            }
            finally
            {
                _DALServer.CloseDB();
            }
            return Count;
        }
        
        #endregion
    }
}
