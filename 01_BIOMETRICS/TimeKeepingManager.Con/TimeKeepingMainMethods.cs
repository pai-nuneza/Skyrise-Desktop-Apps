using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data;

namespace TimeKeepingManager.Con
{
    public class TimeKeepingMainMethods
    {
        private string _profile = string.Empty;
        TimeKeepingManager.Con.Logger _log = new TimeKeepingManager.Con.Logger();
        DeviceDataDownloadManager _downloadengine;
        DeviceDataUploadManager _uploadengine;

        public TimeKeepingMainMethods(String Profile)
        {
            _profile = Profile;
            _downloadengine = new DeviceDataDownloadManager(_profile);
            _uploadengine = new DeviceDataUploadManager(_profile);
        }


        public int iMachineNumber = 1;//the serial number of the device.After connecting the device ,this value will be changed.

        public String GETDEVICEEMPLOYEEDATA(String IP, String Port, String T_TableName, bool ShowMSG)
        {
            String Err = MSG.DefaultErr;
            _downloadengine = new DeviceDataDownloadManager(_profile, ShowMSG);
            try
            {
                #region Get Device Data
                if (_downloadengine.ConnecttoDevice(IP, Port)) // connect to server
                {
                    int DownloadCount = _downloadengine.DownloadDeviceFingerPrintListtoDatabase(_downloadengine.GetDeviceRawUserFingerPrintData(iMachineNumber), T_TableName);
                    _downloadengine.DisconnectDevice();
                    if (DownloadCount > 0)
                    {
                        _log.WriteLog(LogGlobal.AppPath, LogGlobal.RFFP, "DOWNLOADDATAFROMDEVICE", "Successful", true);
                        if (ShowMSG)
                            return String.Format(MSG.DeviceDataDownloadSuccess, DownloadCount);
                    }
                    else if (ErrCode.DeviceNoData)
                    {
                        if (ShowMSG)
                            return MSG.DeviceNoData;
                    }
                    else if (DownloadCount == ErrCode.DBDisconnected)
                    {
                        if (ShowMSG)
                            return MSG.DBDisconnected;
                    }
                    else if (DeviceGlobals.DeviceDataCtr == _uploadengine.GetDatabasePerTableFingerPrintDataCount(T_TableName))
                    {
                        if (ShowMSG)
                            return MSG.DefaultSuccess;
                    }
                }
                else
                {
                    if (ShowMSG)
                        return MSG.DeviceDisconnected;
                }
                #endregion
            }
            catch (Exception e)
            {
                _downloadengine.DisconnectDevice();
                _downloadengine.ExeptionCatcher(e, "DOWNLOADDATAFROMDEVICE");
                if (ShowMSG)
                    return "DOWNLOADDATAFROMDEVICE : " + e.ToString();
            }
            return Err;
        }

        public String UPLOADEMPLOYEEDATATODEVICE(String IP, String Port, String T_TableName, bool ShowMSG)
        {
            string Err = MSG.DefaultErr;
            _downloadengine = new DeviceDataDownloadManager(_profile, ShowMSG);
            try
            {
                _uploadengine = new DeviceDataUploadManager(_profile, true);

                try
                {
                    if (_downloadengine.ConnectToServer())
                    {
                        if (DeviceGlobals.dtEmployeeIDMapping == null)
                        {
                            _downloadengine.GetEmployeeIDMappingTable();
                        }
                        else if (DeviceGlobals.dtEmployeeIDMapping.Rows.Count <= 0)
                        {
                            _downloadengine.GetEmployeeIDMappingTable();
                        }
                        _downloadengine.DisconnectToServer();
                    }
                }
                catch 
                {
 
                }
                #region Upload Employee Templates to device
                if (_uploadengine.ConnecttoDevice(IP, Port)) // connect to device
                {
                    int UploadCount = _uploadengine.UploadDatabaseFingerPrinttoDevice(iMachineNumber, T_TableName);
                    _uploadengine.DisconnectDevice(); //disconnect device
                    if (UploadCount > 0) //Successfull
                    {
                        _downloadengine.UpdateDatabaseDeviceProperties(IP, T_TableName);
                        _log.WriteLog(LogGlobal.AppPath, LogGlobal.RFFP, "UPLOADDATATODEVICE", "Successful", true);
                        if (ShowMSG)
                            return string.Format(MSG.DeviceDataUploadSuccess, UploadCount);
                    }
                    else if (UploadCount == ErrCode.DeviceErrDel) //Error Delete all
                    {
                        if (ShowMSG)
                            return MSG.DeviceCannotRefresh;
                    }
                }
                else
                {
                    if (ShowMSG)
                        return MSG.DeviceDisconnected;
                }
                #endregion
            }
            catch (Exception e)
            {
                _uploadengine.DisconnectDevice(); //disconnect device
                _downloadengine.ExeptionCatcher(e, "UPLOADDATATODEVICE");
                return "UPLOADDATATODEVICE" + e.ToString();
            }
            return Err;
        }

        public String UPLOADEMPLOYEEDATATODEVICE(String IP, String Port, String T_TableName, String DeviceID, bool ShowMSG)
        {
            string Err = MSG.DefaultErr;
            _downloadengine = new DeviceDataDownloadManager(_profile, ShowMSG);
            try
            {
                _uploadengine = new DeviceDataUploadManager(_profile, true);

                try
                {
                    if (_downloadengine.ConnectToServer())
                    {
                        if (DeviceGlobals.dtEmployeeIDMapping == null)
                        {
                            _downloadengine.GetEmployeeIDMappingTable();
                        }
                        else if (DeviceGlobals.dtEmployeeIDMapping.Rows.Count <= 0)
                        {
                            _downloadengine.GetEmployeeIDMappingTable();
                        }
                        _downloadengine.DisconnectToServer();
                    }
                }
                catch
                {

                }
                #region Upload Employee Templates to device
                if (_uploadengine.ConnecttoDevice(IP, Port)) // connect to device
                {
                    System.Data.DataTable dtResult = _uploadengine.GetDatabasePerTableWithDeviceID(T_TableName, DeviceID);
                    TimeKeepingManager.Con.DeviceDataObjectHelper.DeviceEmployeeObjects zkObj = new TimeKeepingManager.Con.DeviceDataObjectHelper.DeviceEmployeeObjects();

                    int UploadCount = 0;
                    try
                    {
                        for (int i = 0; i < dtResult.Rows.Count; i++)
                        {
                            //Initialize
                            zkObj.Trf_EmployeeID = dtResult.Rows[i]["Trf_EmployeeID"].ToString();
                            zkObj.Trf_RFID = dtResult.Rows[i]["Trf_RFID"].ToString();
                            zkObj.Trf_FingerIndex = Convert.ToInt32(dtResult.Rows[i]["Trf_FingerIndex"].ToString());
                            zkObj.Trf_Template = dtResult.Rows[i]["Trf_Template"].ToString();
                            zkObj.Trf_FaceData = dtResult.Rows[i]["Trf_FaceData"].ToString();
                            zkObj.Trf_Privilege = Convert.ToInt32(dtResult.Rows[i]["Trf_Privilege"].ToString());
                            zkObj.Trf_Password = dtResult.Rows[i]["Trf_Password"].ToString();
                            zkObj.Trf_Enabled = (dtResult.Rows[i]["Trf_Enabled"].ToString() == "True")? true : false;
                            zkObj.Trf_Flag = (dtResult.Rows[i]["Trf_Flag"].ToString() == "True") ? 1 : 0;
                            zkObj.ludatetime = DateTime.Now;

                            _uploadengine.UploadDatabaseFingerPrinttoDeviceBatchIndividual(zkObj, iMachineNumber);
                            UploadCount++;
                        }
                    }
                    catch
                    {
                        UploadCount = 0;
                    }

                    _uploadengine.DisconnectDevice(); //disconnect device
                    if (UploadCount > 0) //Successful
                    {
                        //_downloadengine.UpdateDatabaseDeviceProperties(IP, T_TableName);
                        _log.WriteLog(LogGlobal.AppPath, LogGlobal.RFFP, "UPLOADSELECTEDEMPLOYEETODEVICE", "Successful", true);
                        if (ShowMSG)
                            return string.Format(MSG.DeviceIndividualDataUploadSuccess, UploadCount);
                    }
                    else if (UploadCount == ErrCode.DeviceErrDel) //Error Delete all
                    {
                        if (ShowMSG)
                            return MSG.DeviceCannotRefresh;
                    }
                }
                else
                {
                    if (ShowMSG)
                        return MSG.DeviceDisconnected;
                }
                #endregion
            }
            catch (Exception e)
            {
                _uploadengine.DisconnectDevice(); //disconnect device
                _downloadengine.ExeptionCatcher(e, "UPLOADSELECTEDEMPLOYEETODEVICE");
                return "UPLOADSELECTEDEMPLOYEETODEVICE" + e.ToString();
            }
            return Err;
        }

        public void GETDEVICELOGS(String IP, String Port, String T_TableName, DateTime StartDate, DateTime EndDate, String TimeSched, bool ShowMSG)
        {
            _downloadengine = new DeviceDataDownloadManager(_profile, ShowMSG, IP);

            try
            { 
                //Get Standard Verification Table
                DeviceGlobals.dtStandardVerificaitonSchedule = _downloadengine.GetStandardVerificationTableSchedule().Tables[0];
            }
            catch
            { }

            try
            {
                #region Download Device Logs
                //ZkGlobals.TimeSched = dtGetRawLogs.Value.ToString("HH:mm");
                DeviceGlobals.TimeSched = TimeSched; //textfile writting
                if (_downloadengine.ConnecttoDevice(IP, Port)) // connect to device
                {
                    Console.WriteLine("Date : {0} -- Uploading device : {1}", StartDate.ToString(), IP);
                    int DownloadCount = _downloadengine.DownloadDeviceLogstoDatabase(IP, iMachineNumber, StartDate, EndDate, T_TableName);
                    _downloadengine.DisconnectDevice(); //disconnect device

                    if (DownloadCount > 0) //Successfull
                    {
                        Console.WriteLine("\tDevice : {0}  Uploaded Logs : {1}", IP, DownloadCount);
                        _log.WriteLog(LogGlobal.AppPath, LogGlobal.RFFP, string.Format("Get Logs : {0}:{1}", IP, Port), "Successful", true);
                        if (ShowMSG)
                            MessageBox.Show(String.Format(MSG.DeviceLogsUploadSuccess, DownloadCount));
                    }
                    else if (DownloadCount == 0) //Already Downloaded
                    {
                        _log.WriteLog(LogGlobal.AppPath, LogGlobal.RFFP, string.Format("Get Logs : {0}:{1}", IP, Port), "Successful", true);
                        if (ShowMSG)
                            MessageBox.Show(MSG.DeviceLogAlreadyDownloaded);
                    }
                    else if (ErrCode.DeviceNologs) //No Logs
                    {
                        _log.WriteLog(LogGlobal.AppPath, LogGlobal.RFFP, string.Format("Get Logs : {0}:{1}", IP, Port), "Successful", true);
                        if (ShowMSG)
                            MessageBox.Show(MSG.DeviceNoLogs);
                    }
                    else if (ErrCode.DeviceErrCon) //Device Err Connection
                    {
                        if (ShowMSG)
                            MessageBox.Show(MSG.DeviceErrConnetion);
                    }
                    else if (DownloadCount == ErrCode.DBDisconnected) //DB Disconnected
                    {
                        if (ShowMSG)
                            MessageBox.Show(MSG.DBDisconnected, MSG.Failed);
                    }
                }
                else
                {
                    if (ShowMSG)
                        MessageBox.Show(MSG.DeviceDisconnected);
                    Console.WriteLine(String.Format("Unable to connect to device : {0}", IP));
                }
                DeviceGlobals.DeviceLogCtr = 0;
                #endregion
            }
            catch (Exception e)
            {
                _downloadengine.DisconnectDevice(); //disconnect device
                _downloadengine.ExeptionCatcher(e, "GETDEVICELOGS");
            }
        }

        public void GETLOGSEQUENTIALMETHOD(String T_TableName, DateTime StartDate, DateTime EndDate, bool ShowMSG)
        {
            _downloadengine = new DeviceDataDownloadManager(_profile, ShowMSG);
            try
            {
                #region Get logs sequentially : note that this is not effective in graveyard.
                Console.WriteLine("Date : {0}  Sequential Log Uploading ", StartDate.ToString());
                int DownloadCount = _downloadengine.UploadRawLogstoDatabaseSequential(StartDate, EndDate, T_TableName, true);
                if (DownloadCount >= 1) //Successfull
                {
                    Console.WriteLine("\tUploaded Logs : {0}", DownloadCount);
                    _log.WriteLog(LogGlobal.AppPath, LogGlobal.RFFP, "Get Sequential", "Successful", true);
                    if (ShowMSG)
                        MessageBox.Show(String.Format(MSG.DeviceLogsUploadSuccess, DownloadCount));
                }
                #endregion
            }
            catch (Exception e)
            {
                _downloadengine.ExeptionCatcher(e, "GETMINMAXLOGS");
            }
        }

        public bool SYNCDEVICETIME(String IP, String Port)
        {
            return _downloadengine.SetDeviceDateTime(IP, Port);
        }

    }
}
