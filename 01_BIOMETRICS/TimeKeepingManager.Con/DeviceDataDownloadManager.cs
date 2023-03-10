using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CommonLibrary;
using System.Configuration;
using Payroll.DAL;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Data.SqlClient;

namespace TimeKeepingManager.Con
{
    public class DeviceDataDownloadManager : DeviceDataObjectHelper
    {
        /// <summary>
        /// Download Engine for RFID/Biometric Device 
        /// Database and Device Mangement
        /// Developer : Nilo Luansing (nlluansig@n-pax.com)
        /// Date : Jan. 09,2012 - Present
        /// </summary>
        

        /// <summary>
        /// ZKSofware Finger Print RFID Reader objects
        /// </summary>
        
        #region Constructor
        private DALHelper _DALServer;
        TimeKeepingManager.Con.Logger _log = new TimeKeepingManager.Con.Logger();
        //public zkemkeeper.CZKEMClass axCZKEM1 = new zkemkeeper.CZKEMClass(); // use declaration for GUI
        public zkemkeeper.CZKEM BioDataHelper = new zkemkeeper.CZKEM(); // use declaration for console
        //public zkemkeeper.CZKEMClass BioDataHelper = new zkemkeeper.CZKEMClass(); // use declaration for GUI
        private bool bIsConnected = false;//the boolean value identifies whether the device is connected
        private int iMachineNumber = 1;//the serial number of the device.After connecting the device ,this value will be changed.
        private String _profile = string.Empty;
        private bool _showMSG = false;
        private String _ip = "0";
        public DeviceDataDownloadManager(String Profile)
        {
            //_DALServer = new DALHelper(DALHelper.ConnectionSource.Server);
            _DALServer = new DALHelper(Profile,1);
            _profile = Profile;
        }
        public DeviceDataDownloadManager(String Profile, bool ShowMSG)
        {
            //_DALServer = new DALHelper(DALHelper.ConnectionSource.Server);
            _DALServer = new DALHelper(Profile, 1);
            _profile = Profile;
            _showMSG = ShowMSG;
        }
        public DeviceDataDownloadManager(bool DTR)
        {
            _DALServer = new DALHelper(GetConStrDTR(), 1);
            _profile = GetConStrDTR();
        }
        public DeviceDataDownloadManager(bool DTR, bool ShowMSG)
        {
            _DALServer = new DALHelper(GetConStrDTR(), 1);
            _profile = GetConStrDTR();
            _showMSG = true;
        }
        public DeviceDataDownloadManager(String Profile, bool ShowMSG,String IP)
        {
            //_DALServer = new DALHelper(DALHelper.ConnectionSource.Server);
            _DALServer = new DALHelper(Profile, 1);
            _profile = Profile;
            _showMSG = ShowMSG;
            _ip = IP.Substring(IP.Length - 2, 2);
        }

        #endregion

        #region Database Common Connection
        /// <summary>
        /// Opens the remote DAL connection
        /// </summary>
        /// <returns></returns>
        public bool ConnectToServer()
        {
            bool bRetVal = false;
            try
            {
                if (_DALServer == null)
                    _DALServer = new DALHelper(_profile, 1);

                if (_DALServer.ConnectionState == ConnectionState.Closed)
                {
                    _DALServer = new DALHelper(true);
                    _DALServer.OpenDB();
                }
                if (_DALServer.ConnectionState == ConnectionState.Open)
                    bRetVal = true;
            }
            catch
            {
                CollectGarbage();
            }

            return bRetVal;
        }

        public bool DisconnectToServer()
        {
            bool bRetVal = false;
            try
            {
                if (_DALServer.ConnectionState == ConnectionState.Open)
                {
                    _DALServer.CloseDB();
                    bRetVal = true;
                }
            }
            catch
            {
                CollectGarbage();
            }

            return bRetVal;
        }

        public String GetDTRDBName()
        {
            String DTR = string.Empty;
            try
            {
                DTR = Encrypt.decryptText(ConfigurationManager.AppSettings["DTRDBName"].ToString());
            }
            catch (Exception e)
            {
                ExeptionCatcher(e, "GetDTRDBName");
            }
            return DTR;
        }

        public String GetConStrDTR()
        {
            string[] param = new string[4];
            string _connection = string.Empty;
            try
            {
                param[0] = Encrypt.decryptText(ConfigurationManager.AppSettings["DataSource"].ToString());
                param[1] = Encrypt.decryptText(ConfigurationManager.AppSettings["DTRDBName"].ToString());
                param[2] = Encrypt.decryptText(ConfigurationManager.AppSettings["UserID"].ToString());
                param[3] = Encrypt.decryptText(ConfigurationManager.AppSettings["Password"].ToString());
                _connection = string.Format(ConfigurationManager.ConnectionStrings["dtrConnectionString"].ConnectionString, param);
            }
            catch (Exception e)
            {
                ExeptionCatcher(e, "GetConStrDTR");
            }
            return _connection;
        }

        private void CollectGarbage()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        #endregion

        #region Methods
        //Downdload employee data from device to database
        public void DownloadDeviceFingerPrinttoDatabase(DeviceEmployeeObjects _zkObject, String T_TableName)
        {
            try
            {
                _DALServer.BeginTransaction();
                if (InsertRawFingerPrintDatatoDatabase(_zkObject, _DALServer, T_TableName))
                {
                    _DALServer.CommitTransaction();
                }
                else
                    _DALServer.RollBackTransaction();
            }
            catch (Exception e)
            {
                ExeptionCatcher(e, "DownloadEmployee");
            }
        }

        public int DownloadDeviceFingerPrintListtoDatabase(List<DeviceEmployeeObjects> paramZKObjList, String T_TableName)
        {
            int Affected = 0;
            int i = 0;
            try
            {
                if (ConnectToServer())
                {
                    foreach (DeviceEmployeeObjects zkObj in paramZKObjList)
                    {
                        _DALServer.BeginTransaction();
                        if (InsertRawFingerPrintDatatoDatabase(zkObj, _DALServer, T_TableName))
                        {
                            _DALServer.CommitTransaction();
                            Affected = ++Affected;
                        }
                        else
                            _DALServer.RollBackTransaction();
                        ++i;
                        DeviceGlobals.Progress = (Convert.ToDouble(i) / Convert.ToDouble(paramZKObjList.Count)) * 100.0;
                    }
                    DisconnectToServer();
                }
                else
                {
                    Affected = ErrCode.DBDisconnected; //Unable to connect to database
                }
            }
            catch (Exception e)
            {
                ExeptionCatcher(e, "DownloadEmployee");
            }
            return Affected;
        }

        //Download Logs from Device to database
        public int DownloadDeviceLogstoDatabase(String DeviceIP,
                                                Int32 iMachineNumber,
                                                DateTime StartDate,
                                                DateTime EndDate, 
                                                String T_TableName)
        {
            //Author : Nilo
            //Note that the function GETDeviceRawLogs get ALL the logs from device
            //then insert to dtr if not existing without checking date range

            try
            {
                StartDate = Convert.ToDateTime(StartDate.ToShortDateString());
                EndDate = Convert.ToDateTime(EndDate.ToShortDateString()).AddDays(1);
                DateTime LogDate = StartDate;
                int days = EndDate.Subtract(StartDate).Days;
                int LogsUploaded = 0;
                int TotalUpload = 0;
                
                while (LogDate <= EndDate)
                {
                    string DeviceDesc = "";
                    LogsUploaded = DownloadDeviceLogsListtoDatabase(DeviceIP, 
                                                                    GetDeviceRawLogs(iMachineNumber, LogDate, ref DeviceDesc), 
                                                                    LogDate, 
                                                                    T_TableName, 
                                                                    DeviceDesc);
                    TotalUpload = TotalUpload + LogsUploaded;
                    LogDate = LogDate.AddDays(1);
                }

                //Nilo Added : 20131022
                //This is a very critical function
                //Archiving and verifying after getting device logs.

                //Archiving the device log to text file.
                //Clear device logs.
                //Upload archive text file to DTR.
                //Rename .dat archive file to .prc processed file.

                if (TotalUpload > 0)
                {
                    DownloadArchiveClearDeviceLogs();
                }
            }
            catch (Exception e)
            {
                ExeptionCatcher(e, "DonwloadDeviceLogs");
            }
            return DeviceGlobals.DeviceLogCtr;
        }

        public int DownloadDeviceLogsListtoDatabase(String DeviceIP, List<DeviceTimeLogObjects> paramZkLogs, DateTime LogDate, String T_TableName, string DeviceDesc)
        {
            int Affected = 0;
            try
            {
                if (ConnectToServer())
                {
                    //Nilo added 2013 : Get mapping table before inserting data
                    if (DeviceGlobals.dtEmployeeIDMapping == null)
                    {
                        GetEmployeeIDMappingTable();
                    }
                    else if (DeviceGlobals.dtEmployeeIDMapping.Rows.Count <= 0)
                    {
                        GetEmployeeIDMappingTable();
                    }

                    double MaxProgress = 0;
                    int i = 0;
                    try { MaxProgress = Convert.ToDouble(paramZkLogs.Count); }
                    catch { }

                    foreach (DeviceTimeLogObjects rawLog in paramZkLogs)
                    {
                        #region For Progress Display 

                        double progress = 0;
                        try
                        {
                            if (MaxProgress > 0)
                                progress = ((Convert.ToDouble(i) + 1) / Convert.ToDouble(MaxProgress)) * 100;
                        }
                        catch { }
                        DeviceGlobals.Progress = progress;
                        ++i;

                        #endregion

                        bool NoExeption = true;

                        if (InsertRawLogstoDatabase(rawLog, 
                                                    LogDate, 
                                                    T_TableName, 
                                                    ref NoExeption,
                                                    DeviceIP,
                                                    DeviceDesc))
                        {
                            Affected = ++Affected;
                        }
                    }
                    DisconnectToServer();
                }
                else
                {
                    Affected = ErrCode.DBDisconnected; //Unable to connect to database
                }
            }
            catch (Exception e)
            {
                ExeptionCatcher(e, "MainDownloadDeviceLogstoDatabse");
            }
            return Affected;
        }

        public bool DownloadArchiveClearDeviceLogs()
        {
            bool ret = true;
            try
            {
                if (GetDeviceLogsStatistic() > 75) //Device logs is greater than 75% of available storage.
                {
                    //Start Archiving and device log clean up.
                    bool archivedResult = false;
                    List<DeviceTimeLogObjects> DeviceLogs = new List<DeviceTimeLogObjects>();
                    DeviceLogs = GetDeviceRawLogs(iMachineNumber, Convert.ToDateTime("01/01/2013"));
                    int ArchivedLog = DeviceRawLogstoAttDataArchived(Application.StartupPath + @"\AttlogArchive\", DeviceLogs, ref archivedResult);
                    DeviceLogs = null; //Disposing.
                    string[] ArchiveFiles = Directory.GetFiles(Application.StartupPath + @"\AttlogArchive\", "*.dat");

                    if (archivedResult                              // Archiving process was successful.
                        && ArchivedLog > 0                          // There are logs that has been archived.
                        && ArchivedLog == GetCountTimeLogs()        // No new logs from the device that has not included in archived file.
                        && ArchiveFiles.Length > 0)                 // Make sure there is an existing archived file
                    {
                        //Clrear Logs
                        BioDataHelper.ClearGLog(iMachineNumber);
                        //Disconnect at this point
                        DisconnectDevice();

                        //Start verifying archive text file here
                        foreach (string AttlogArchiveFile in ArchiveFiles)
                        {
                            bool uploadResult = false;
                            USBTextFileUploading(AttlogArchiveFile,
                                                    Convert.ToDateTime("01/01/2013"),
                                                    DateTime.MaxValue,
                                                   (DeviceGlobals.DirectDTR ? "T_EmpDTR" : "T_EmpDTRDevice"),
                                                    false,
                                                    ref uploadResult);

                            if (uploadResult)
                            {
                                //Rename as processed file.
                                File.Move(AttlogArchiveFile, AttlogArchiveFile.Substring(0, (AttlogArchiveFile.Length - 4)) + ".prc");
                            }
                        }
                    }
                    else
                    { 
                        //Disconnect at this point
                        DisconnectDevice();
                    }
                }

                ret = true;

            }
            catch
            {
                ret = false;
            }

            return ret;
        }

        #endregion

        #region Device Data Handling

        public List<DeviceEmployeeObjects> GetDeviceRawUserFingerPrintData(int iMachineNumber)
        {
            // Wans : Note for devices check the version of firmware and most importantly the 
            //        reading algorithims used [9.0 and 10.0] this program runs on 10.0 for 
            //        TFT device but better use 9.0 it is more tested.
            //        B&W device only works on 9.0 version
            // Date : 4/20/2013
            List<DeviceEmployeeObjects> ZkDeviceDataList = new List<DeviceEmployeeObjects>();
            try
            {
                #region Private variables

                string EnrolledIDNumber = "";
                int BWEnrollIDNumber = 0;
                string EnrolledName = "";
                string EnrolledPassword = "";
                int Privilege = 0;
                bool Enabled = false;
                int FingerIndex;
                string FingerPrintTemplate = "";
                int TemplateLength = 0;
                int Flag = 0;
                string RFIDCardNumber = "";
                //Get Face Data
                int FaceDataIndex = 50;//the only possible parameter value
                string FaceDataTemplate = "";
                int FaceDataLength = 0;
                bool HasFingerTemplate = false;
                bool HasRFID = false;
                bool HasFaceData = false;

                #endregion

                BioDataHelper.EnableDevice(iMachineNumber, false);
                BioDataHelper.ReadAllUserID(iMachineNumber);//read all the user information to the memory
                BioDataHelper.ReadAllTemplate(iMachineNumber);//read all the users' fingerprint templates to the memory

                while (DeviceVersions.IsBlackAndWhiteType ? BioDataHelper.GetAllUserInfo(iMachineNumber, 
                                                                                    ref BWEnrollIDNumber, 
                                                                                    ref EnrolledName, 
                                                                                    ref EnrolledPassword, 
                                                                                    ref Privilege, 
                                                                                    ref Enabled)              //(Black&White) Getting all users' information from the memory
                                                        : BioDataHelper.SSR_GetAllUserInfo(iMachineNumber, 
                                                                                    out EnrolledIDNumber, 
                                                                                    out EnrolledName, 
                                                                                    out EnrolledPassword, 
                                                                                    out Privilege, 
                                                                                    out Enabled))             //(TFT Colored/ Veriface) Getting all users' information from the memory
                {
                    ErrCode.DeviceNoData = false;
                    EnrolledIDNumber = DeviceVersions.IsBlackAndWhiteType ? BWEnrollIDNumber.ToString() : EnrolledIDNumber;
                    for (FingerIndex = 0; FingerIndex < 10; FingerIndex++)
                    {
                        DeviceEmployeeObjects zkObj = new DeviceEmployeeObjects();
                        zkObj = InitializeEmployeeObjects(zkObj);

                        //Download user's 9.0 or 10.0 arithmetic fingerprint templates(in strings)
                        //Only TFT screen devices with firmware version Ver 6.60 version later support function "GetUserTmpExStr" and "GetUserTmpEx".
                        //While you are using 9.0 fingerprint arithmetic and your device's firmware version is under ver6.60,you should use the functions "SSR_GetUserTmp" or 
                        //"SSR_GetUserTmpStr" instead of "GetUserTmpExStr" or "GetUserTmpEx" in order to download the fingerprint templates.

                        //version 6.60 onward
                        //if (axCZKEM1.GetUserTmpExStr(iMachineNumber, sdwEnrollNumber, idwFingerIndex, out iFlag, out sTmpData, out iTmpLength))//get the corresponding templates string and length from the memory
                        //{
                        //roll backing version 05/30/2012 previous version 6.60

                        if (EnrolledIDNumber == "1105")
                        {
                            string x = "testbreak";
                        }

                        if (DeviceVersions.IsBlackAndWhiteType ? BioDataHelper.GetUserTmpExStr(iMachineNumber,
                                                                                                EnrolledIDNumber,
                                                                                                FingerIndex,
                                                                                                out Flag,
                                                                                                out FingerPrintTemplate,
                                                                                                out TemplateLength)              //(Black&White) Getting per users' finger print template data
                                                                    : BioDataHelper.SSR_GetUserTmpStr(iMachineNumber,
                                                                                                EnrolledIDNumber,
                                                                                                FingerIndex,
                                                                                                out FingerPrintTemplate,
                                                                                                out TemplateLength))             //(TFT Colored) Getting per users' finger print template data
                        {
                            Flag = 1;
                            //set zkobject from memory of the device
                            zkObj.Trf_FingerIndex = FingerIndex;
                            zkObj.Trf_Template = FingerPrintTemplate;
                            HasFingerTemplate = true;
                        }
                        
                        try
                        {
                            if (FingerIndex == 0 && BioDataHelper.GetStrCardNumber(out RFIDCardNumber))//get the card number from the memory
                            {
                                zkObj.Trf_RFID = RFIDCardNumber;
                                HasRFID = true;
                            }
                        }
                        catch
                        { }

                        try
                        {
                            if (DeviceVersions.IsVeriface)
                            {   
                                if (FingerIndex == 0 && BioDataHelper.GetUserFaceStr(iMachineNumber, 
                                                                                     EnrolledIDNumber, 
                                                                                     FaceDataIndex, 
                                                                                     ref FaceDataTemplate, 
                                                                                     ref FaceDataLength))//get the face templates from the memory
                                {
                                    zkObj.Trf_FaceData = FaceDataTemplate;
                                    HasFaceData = true;
                                }
                            }
                        }
                        catch
                        { }

                        //set zkobject from memory of the device
                        if (HasFingerTemplate || HasRFID || HasFaceData)
                        {
                            zkObj.Trf_EmployeeID = EnrolledIDNumber;
                            zkObj.Trf_Privilege = Privilege;
                            zkObj.Trf_Password = EnrolledPassword;
                            zkObj.Trf_Enabled = Enabled;
                            zkObj.Trf_Flag = Flag;
                            zkObj.ludatetime = DateTime.Now;

                            ZkDeviceDataList.Add(zkObj);
                            HasFingerTemplate = false;
                            HasRFID = false;
                            HasFaceData = false;
                        }
                    }
                }

                BioDataHelper.EnableDevice(iMachineNumber, true);

                DeviceGlobals.DeviceDataCtr = ZkDeviceDataList.Count;
                _log.WriteLog(ErrCode.PathLog, ErrCode.ErrLog, "GetDeviceRawUserFingerPrintData", "Successful", true);
            }
            catch (Exception e)
            {
                BioDataHelper.EnableDevice(iMachineNumber, true);

                if (_showMSG)
                    MessageBox.Show("GetDeviceRawUserFingerPrintData : " + e.StackTrace, MSG.Err);
                else
                    _log.WriteLog(ErrCode.PathLog, ErrCode.ErrLog, "GetDeviceRawUserFingerPrintData", e.StackTrace, true);
            }
            return ZkDeviceDataList;
        }

        public List<DeviceTimeLogObjects> GetDeviceRawLogs(int iMachineNumber, DateTime Logdate)
        {
            string DeviceDesc = "";
            return GetDeviceRawLogs(iMachineNumber, Logdate, ref DeviceDesc);
        }

        public List<DeviceTimeLogObjects> GetDeviceRawLogs(int iMachineNumber, DateTime Logdate, ref string DeviceDesc)
        {
            iMachineNumber = BioDataHelper.MachineNumber;
            Logdate = Convert.ToDateTime(Logdate.ToShortDateString());
            string DeviceIpAddress = "";
            BioDataHelper.GetDeviceIP(iMachineNumber, ref DeviceIpAddress);
            List<DeviceTimeLogObjects> RawLogList = new List<DeviceTimeLogObjects>();
            try
            {
                #region Private variables
                string EnrolledIDNumber = "";
                int VerifyMode = 0;
                int InOutMode = 0;
                int YearValue = 0;
                int MonthValue = 0;
                int DayValue = 0;
                int HourValue = 0;
                int MinuteValue = 0;
                int SecondValue = 0;
                int WorkCode = 0;
                int ErrorCode = 0;
                int ParamValue = 0;
                int Reserved = 0;
                bool logExist = _log.AttLogExist(Application.StartupPath, Convert.ToDateTime(Logdate).ToString("yyyyMMdd_raw"));
                #endregion

                //axCZKEM1.EnableDevice(iMachineNumber, false);//disable the device

                if (BioDataHelper.GetDeviceStatus(iMachineNumber, 6, ref ParamValue)) //Here we use the function "GetDeviceStatus" to get the record's count.The parameter "Status" is 6.
                {
                    if (ParamValue <= 0)
                    {
                        ErrCode.DeviceNologs = true;
                        return RawLogList;
                    }
                }
                if (BioDataHelper.ReadGeneralLogData(iMachineNumber))//read all the attendance records to the memory
                {
                    try
                    {
                        int idNumber = 0;
                        int idwTMachineNumber = 0;

                        while ((DeviceVersions.IsBlackAndWhiteType) ? BioDataHelper.GetGeneralExtLogData(iMachineNumber,
                                    ref idNumber,
                                    ref VerifyMode,
                                    ref InOutMode,
                                    ref YearValue,
                                    ref MonthValue,
                                    ref DayValue,
                                    ref HourValue,
                                    ref MinuteValue,
                                    ref SecondValue,
                                    ref WorkCode,
                                    ref Reserved)
                            : BioDataHelper.SSR_GetGeneralLogData(iMachineNumber,
                                    out EnrolledIDNumber,
                                    out VerifyMode,
                                    out InOutMode,
                                    out YearValue,
                                    out MonthValue,
                                    out DayValue,
                                    out HourValue,
                                    out MinuteValue,
                                    out SecondValue,
                                    ref WorkCode))           //TFT/Veriface : Get device time logs
                        {
                            EnrolledIDNumber = (DeviceVersions.IsBlackAndWhiteType) ? idNumber.ToString() : EnrolledIDNumber;

                            DeviceTimeLogObjects RawLog = new DeviceTimeLogObjects();
                            RawLog = InitializeTimeLogObjects(RawLog);

                            RawLog.Tel_IDNo = EnrolledIDNumber;
                            RawLog.Tel_LogDate = string.Format("{0:00}/{1:00}/{2}", MonthValue, DayValue, YearValue);
                            RawLog.Tel_LogTime = string.Format("{0:00}{1:00}", HourValue, MinuteValue);
                            RawLog.Tel_LogType = InOutMode.ToString().Trim() == "0" ? "I" : "O";
                            RawLog.Dtr_RawDateTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}"
                                                                  , YearValue
                                                                  , MonthValue
                                                                  , DayValue
                                                                  , HourValue
                                                                  , MinuteValue
                                                                  , SecondValue);
                            RawLog.Dtr_VerifyMode = GetVerficationMode(DeviceVersions.IsVeriface ?
                                                                        DeviceVersions.DeviceTypes.Veriface :
                                                                        DeviceVersions.IsTFT ? DeviceVersions.DeviceTypes.TFT : DeviceVersions.DeviceTypes.BlackandWhite
                                                                      , VerifyMode);

                            RawLog.Dtr_Standard = GetLogStandardStatus(DeviceIpAddress, RawLog);
                            DeviceDesc = DeviceVersions.GetDeviceVersionDesc(DeviceVersions.DeviceType);

                            //Optimize from all device logs, get only logdate(month -1)
                            DateTime FromDateForUploading = Logdate.AddMonths(-1);
                            try
                            {
                                if (FromDateForUploading <= Convert.ToDateTime(RawLog.Tel_LogDate))
                                {
                                    RawLogList.Add(RawLog);
                                }
                            }
                            catch
                            {
                                RawLogList.Add(RawLog);
                            }

                            //Writing raw logs to text file
                            if (Logdate <= Convert.ToDateTime(RawLog.Tel_LogDate) && DeviceGlobals.TimeSched.Length > 2)
                            {
                                if (!logExist && DateTime.Now.ToString("HH") == DeviceGlobals.TimeSched.Substring(0, 2))
                                    _log.WriteAttLog(Application.StartupPath, Convert.ToDateTime(RawLog.Tel_LogDate).ToString("yyyyMMdd_raw"), string.Format("{0},{1},{2},{3}",
                                                                            RawLog.Tel_IDNo, RawLog.Tel_LogDate, RawLog.Tel_LogTime, RawLog.Tel_LogType), true);
                            }
                        }
                    }
                    catch (Exception x)
                    {

                    }
                }
                else
                {
                    BioDataHelper.GetLastError(ref ErrorCode);

                    if (ErrorCode != 0)
                    {
                        ErrCode.DeviceErrCon = true;
                    }
                    else
                    {
                        ErrCode.DeviceNologs = true;
                    }
                }
                _log.WriteLog(ErrCode.PathLog, ErrCode.ErrLog, string.Format("GetDeviceRawLogs : {0:MM/dd/yyyy}", Logdate), "Successful", true);
            }
            catch (Exception e)
            {
                if (_showMSG)
                    MessageBox.Show("GetDeviceRawLogs : " + e.StackTrace, MSG.Err);
                else
                    _log.WriteLog(ErrCode.PathLog, ErrCode.ErrLog, string.Format("GetDeviceRawLogs : {0:MM/dd/yyyy}", Logdate), e.StackTrace, true);
            }

            //axCZKEM1.EnableDevice(iMachineNumber, true);//enable the device

            return RawLogList;
        }
        //Device connections
        public bool ConnecttoDevice(String IP, String Port)
        {
            bIsConnected = BioDataHelper.Connect_Net(IP, Convert.ToInt32(Port));

            try
            {
                //try Rs232 Connection
                if (!bIsConnected)
                {
                    DataTable dt = new DataTable();
                    dt = GetDevicePropertyfromDatabaseList(IP, true);
                    if (dt == null)
                        bIsConnected = false;
                    else if (dt.Rows.Count == 0)
                        bIsConnected = false;
                    else
                        bIsConnected = ConnecttoDeviceRS232(GetDevicePropertyfromDatabaseList(IP, true).Rows[0]["DEVICERSCOM"].ToString(), 1, GetDevicePropertyfromDatabaseList(IP, true).Rows[0]["DEVICERATE"].ToString());
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
                if (_showMSG)
                    MessageBox.Show("ConnecttoDevice : " + e.StackTrace, MSG.Err);
                else
                    _log.WriteLog(ErrCode.PathLog, ErrCode.ErrLog, "ConnecttoDevice", e.StackTrace, true);
            }
            return DeviceGlobals.isDeviceCon;
        }
        
        public bool ConnecttoDeviceRS232(string RS232COM, int iMachineNumber, string RsBaudRate)
        {
            RS232COM = RS232COM.ToUpper().Replace("COM", "");
            bool bIsConnected = BioDataHelper.Connect_Com(Convert.ToInt16(RS232COM), iMachineNumber, Convert.ToInt32(RsBaudRate));
            try
            {
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
                if (_showMSG)
                    MessageBox.Show("ConnecttoDeviceRS : " + e.StackTrace, MSG.Err);
                else
                    _log.WriteLog(ErrCode.PathLog, ErrCode.ErrLog, "ConnecttoDeviceRS", e.StackTrace, true);
            }
            return DeviceGlobals.isDeviceCon;
        }
        
        public bool DisconnectDevice()
        {
            try
            {
                if (DeviceGlobals.isDeviceCon)
                    BioDataHelper.Disconnect();
                DeviceGlobals.isDeviceCon = false;
            }
            catch (Exception e)
            {
                if (_showMSG)
                    MessageBox.Show("DisconnectDevice : " + e.StackTrace, MSG.Err);
                else
                    _log.WriteLog(ErrCode.PathLog, ErrCode.ErrLog, "DisconnectDevice", e.StackTrace, true);
            }
            return DeviceGlobals.isDeviceCon;
        }
        
        public bool SetDeviceDateTime(String IP, String Port)
        {
            bool _return = false;
            try
            {
                #region Set Device Date Time
                DateTime SetDeviceDateTime = GetServerDateTime();

                if (ConnecttoDevice(IP, Port)) // connect to device
                {
                    #region Private Variables
                    SetDeviceDateTime = SetDeviceDateTime.AddSeconds(5);
                    int idwYear = Convert.ToInt32(SetDeviceDateTime.ToString("yyyy"));
                    int idwMonth = Convert.ToInt32(SetDeviceDateTime.ToString("MM"));
                    int idwDay = Convert.ToInt32(SetDeviceDateTime.ToString("dd"));
                    int idwHour = Convert.ToInt32(SetDeviceDateTime.ToString("HH"));
                    int idwMinute = Convert.ToInt32(SetDeviceDateTime.ToString("mm"));
                    int idwSecond = Convert.ToInt32(SetDeviceDateTime.ToString("ss"));
                    #endregion
                    if (BioDataHelper.SetDeviceTime2(iMachineNumber, idwYear, idwMonth, idwDay, idwHour, idwMinute, idwSecond))
                    {
                        BioDataHelper.RefreshData(iMachineNumber);//the data in the device should be refreshed
                        _log.WriteLog(LogGlobal.AppPath, LogGlobal.RFFP, string.Format("SetDeviceDateTime : {0}", IP), "Successful", true);
                        _return = true;
                    }
                    else
                    {
                        _log.WriteLog(LogGlobal.AppPath, LogGlobal.RFFP, string.Format("FailSetDeviceDateTime : {0}", IP), "Failed", true);
                        _return = false;
                    }
                }
                else
                {
                    _return = false;
                }
                #endregion
            }
            catch (Exception e)
            {
                ExeptionCatcher(e, "SetDeviceDateTime");
                _return = false;
            }

            DisconnectDevice();

            return _return;
        }

        public DataSet GetDeviceIPfromDatabaseList()
        {
            DataSet ds = new DataSet();
            try
            {
                if (ConnectToServer())
                {
                    String sql = @" SELECT [Mtd_DeviceIP] AS DEVICEIP
                                          ,[Mtd_DeviceName] AS DEVICENAME
                                     FROM [M_TerminalDevice2]
                                     WHERE [Mtd_RecordStatus] = 'A'
                                  ORDER BY [Mtd_DeviceName]";
                    try
                    {
                        ds = _DALServer.ExecuteDataSet(sql);

                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            ds.Tables[0].Rows[i]["DEVICEIP"] = Encrypt.decryptText(ds.Tables[0].Rows[i]["DEVICEIP"].ToString());
                        }
                    }
                    catch (Exception e) { ExeptionCatcher(e, "GetDeviceIPfromDatabaseList : Rollback"); }
                    finally
                    {
                        DisconnectToServer();
                    }
                }
            }
            catch (Exception e)
            {
                if (_showMSG)
                    MessageBox.Show("GetDeviceIPList : " + e.StackTrace, MSG.Err);
                else
                    _log.WriteLog(ErrCode.PathLog, ErrCode.ErrLog, "GetDeviceIPList", e.StackTrace, true);
            }
            return ds;
        }

        public DataTable GetDevicePropertyfromDatabaseList()
        {
            DataSet ds = new DataSet();
            DataTable dt =new DataTable();
            try
            {
                if (ConnectToServer())
                {
                    String sql = @"SELECT [Mtd_DeviceIP] AS DEVICEIP
                                  ,[Mtd_DevicePort] AS DEVICEPORT
                                  ,[Mtd_RS232Com] AS DEVICERSCOM
                                  ,[Mtd_RS232BaudRate] AS DEVICERATE
                                  ,[Mtd_LastLoadedTable] AS LASTLOADED
                                  ,[Mtd_TableName] AS TABLENAME
                                  ,[Mtd_RecordStatus] AS STATUS
                                  ,[Mtd_VersionNo] AS VERSION
                              FROM [M_TerminalDevice2]
                              WHERE Mtd_RecordStatus='A'";
                    try
                    {
                        ds = _DALServer.ExecuteDataSet(sql);
                        //Decrypt IP - Rendell Uy (9/13/2016)
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            ds.Tables[0].Rows[i]["DEVICEIP"] = Encrypt.decryptText(ds.Tables[0].Rows[i]["DEVICEIP"].ToString());
                        }

                        dt = ds.Tables[0];
                    }
                    catch (Exception e) { ExeptionCatcher(e, "GetDevicePropertyfromDatabaseList : Rollback"); }
                    finally
                    {
                        DisconnectToServer();
                    }
                }
            }
            catch (Exception e)
            {
                ExeptionCatcher(e, "GetDeviceProperty");
            }
            return dt;
        }

        public DataTable GetDevicePropertyfromDatabaseList(String IP, bool withDeviceID)
        {
            string[] deviceIP;
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            DataRow[] arrResult;
            try
            {
                if (ConnectToServer())
                {
                    if (withDeviceID)
                    {
                        String sql = @"SELECT [Mtd_DeviceIP] AS DEVICEIP
                                            , [Mtd_DeviceIP] [OrgDeviceIP]
                                    FROM [M_TerminalDevice2]
                                    WHERE Mtd_RecordStatus='A' ";                        
                        try
                        {
                            ds = _DALServer.ExecuteDataSet(sql);
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                deviceIP = Encrypt.decryptText(ds.Tables[0].Rows[i]["DEVICEIP"].ToString()).Split('_');
                                ds.Tables[0].Rows[i]["DEVICEIP"] = deviceIP[0];                                
                            }
                            arrResult = ds.Tables[0].Select("DEVICEIP = '" + IP + "'");
                            if (arrResult.Length > 0)
                                IP = arrResult[0].ItemArray[1].ToString();

                            String qSelect = String.Format(@"SELECT [Mtd_DeviceIP] AS DEVICEIP
                                  ,[Mtd_DevicePort] AS DEVICEPORT
                                  ,[Mtd_RS232Com] AS DEVICERSCOM
                                  ,[Mtd_RS232BaudRate] AS DEVICERATE
                                  ,[Mtd_LastLoadedTable] AS LASTLOADED
                                  ,[Mtd_TableName] AS TABLENAME
                                  ,[Mtd_RecordStatus] AS STATUS
                                  ,[Mtd_VersionNo] AS VERSION
                                  , CASE Mtd_DefaultLogType 
                                                        WHEN 'I'
                                                            THEN 'IN'
                                                        WHEN 'O'
                                                            THEN 'OUT'
                                                        ELSE
                                                            'MULTI' 
                                                        END AS DEVICELOGTYPE
                              FROM [M_TerminalDevice2]
                              WHERE Mtd_RecordStatus='A' 
                              AND Mtd_DeviceIP='{0}'", IP);
                            ds = _DALServer.ExecuteDataSet(qSelect);
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                deviceIP = Encrypt.decryptText(ds.Tables[0].Rows[i]["DEVICEIP"].ToString()).Split('_');
                                ds.Tables[0].Rows[i]["DEVICEIP"] = deviceIP[0];
                            }
                            dt = ds.Tables[0];
                        }
                        catch (Exception e) { ExeptionCatcher(e, "GetDevicePropertyfromDatabaseList : Rollback"); }
                        finally
                        {
                            DisconnectToServer();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ExeptionCatcher(e, "GetDeviceProperty");
            }
            return dt;
        }

        public DataTable GetDevicePropertyfromDatabaseList(String IP)
        {
            string[] deviceIP;
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            try
            {
                if (ConnectToServer())
                {
                    IP = Encrypt.encryptText(IP.Trim());
                    String sql = String.Format(@"SELECT [Mtd_DeviceIP] AS DEVICEIP
                                  ,[Mtd_DevicePort] AS DEVICEPORT
                                  ,[Mtd_RS232Com] AS DEVICERSCOM
                                  ,[Mtd_RS232BaudRate] AS DEVICERATE
                                  ,[Mtd_LastLoadedTable] AS LASTLOADED
                                  ,[Mtd_TableName] AS TABLENAME
                                  ,[Mtd_RecordStatus] AS STATUS
                                  ,[Mtd_VersionNo] AS VERSION
                                  , CASE Mtd_DefaultLogType 
                                                        WHEN 'I'
                                                            THEN 'IN'
                                                        WHEN 'O'
                                                            THEN 'OUT'
                                                        ELSE
                                                            'MULTI' 
                                                        END AS DEVICELOGTYPE
                              FROM [M_TerminalDevice2]
                              WHERE Mtd_RecordStatus='A' AND Mtd_DeviceIP='{0}'", IP);
                    try
                    {
                        ds = _DALServer.ExecuteDataSet(sql);
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            deviceIP = Encrypt.decryptText(ds.Tables[0].Rows[i]["DEVICEIP"].ToString()).Split('_');
                            ds.Tables[0].Rows[i]["DEVICEIP"] = deviceIP[0];
                        }
                        dt = ds.Tables[0];
                    }
                    catch (Exception e) { ExeptionCatcher(e, "GetDevicePropertyfromDatabaseList : Rollback"); }
                    finally
                    {
                        DisconnectToServer();
                    }
                }
            }
            catch (Exception e)
            {
                ExeptionCatcher(e, "GetDeviceProperty");
            }
            return dt;
        }

        public DataSet GetStandardVerificationTableSchedule()
        {
            DataSet ds = new DataSet();
            try
            {
                if (ConnectToServer())
                {
                    try
                    {
                        ds = _DALServer.ExecuteDataSet(Payroll.BLogic.CommonDeviceQueries.SelectStandardVerificationTypeShedule());
                    }
                    catch (Exception e) { ExeptionCatcher(e, "GetStandardVerificationTableSchedule : Rollback"); }
                    finally
                    {
                        DisconnectToServer();
                    }
                }
            }
            catch (Exception e)
            {
                if (_showMSG)
                    MessageBox.Show("GetStandardVerificationTableSchedule : " + e.StackTrace, MSG.Err);
                else
                    _log.WriteLog(ErrCode.PathLog, ErrCode.ErrLog, "GetStandardVerificationTableSchedule", e.StackTrace, true);
            }
            return ds;
        }

        public DeviceDataObjectHelper.DeviceDetails GetDeviceDatails()
        {
            DeviceDataObjectHelper.DeviceDetails DeviceDataDetail = new DeviceDataObjectHelper.DeviceDetails();
            BioDataHelper.GetDeviceIP(iMachineNumber, ref DeviceDataDetail.DeviceIpAddress);
            BioDataHelper.GetDeviceStatus(iMachineNumber, 1, ref DeviceDataDetail.Administrators);
            BioDataHelper.GetDeviceStatus(iMachineNumber, 2, ref DeviceDataDetail.RegisteredUsers);
            BioDataHelper.GetDeviceStatus(iMachineNumber, 3, ref DeviceDataDetail.RegisteredFP);
            BioDataHelper.GetDeviceStatus(iMachineNumber, 21, ref DeviceDataDetail.RegisteredFaces);
            BioDataHelper.GetDeviceStatus(iMachineNumber, 6, ref DeviceDataDetail.AttendanceRecord);
            BioDataHelper.GetDeviceStatus(iMachineNumber, 8, ref DeviceDataDetail.UserCapacity);
            BioDataHelper.GetDeviceStatus(iMachineNumber, 7, ref DeviceDataDetail.FPCapacity);
            BioDataHelper.GetDeviceStatus(iMachineNumber, 22, ref DeviceDataDetail.FaceCapacity);
            BioDataHelper.GetDeviceStatus(iMachineNumber, 9, ref DeviceDataDetail.AttendanceCapacity);
            BioDataHelper.GetSerialNumber(iMachineNumber, out DeviceDataDetail.SerialNumber);
            BioDataHelper.GetCardFun(iMachineNumber,ref DeviceDataDetail.isCardSupported);
            BioDataHelper.GetSysOption(iMachineNumber, "~ZKFPVersion", out DeviceDataDetail.Algorithm);
            return DeviceDataDetail;
        }

        public Double GetDeviceLogsStatistic()
        {
            int TotalTimeLogs = 0;
            int TimeLogsCapacity = 0;
            BioDataHelper.GetDeviceStatus(iMachineNumber, 6, ref TotalTimeLogs);
            BioDataHelper.GetDeviceStatus(iMachineNumber, 9, ref TimeLogsCapacity);
            try
            {
                return TotalTimeLogs / TimeLogsCapacity;
            }
            catch
            {
                return 0;
            }
        }

        public int GetCountTimeLogs()
        {
            int TotalTimeLogs = 0;
            BioDataHelper.GetDeviceStatus(iMachineNumber, 6, ref TotalTimeLogs);
            try
            {
                return TotalTimeLogs;
            }
            catch
            {
                return 0;
            }    
        }

        public bool ClearDeviceAdministrators()
        {
            return BioDataHelper.ClearAdministrators(iMachineNumber);
        }

        private DeviceDataObjectHelper.Standard GetLogStandardStatus(String DeviceIP, DeviceTimeLogObjects dtr)
        {
            try
            {
                if (dtr.Dtr_VerifyMode == VerifyMode.Unkown)
                {
                    return Standard.Yes;
                }
                else if (DeviceGlobals.dtStandardVerificaitonSchedule == null || DeviceGlobals.dtStandardVerificaitonSchedule.Rows.Count == 0)
                {
                    //No Standard has been set.
                    return Standard.Yes;
                }
                else
                {
                    foreach (DataRow dr in DeviceGlobals.dtStandardVerificaitonSchedule.Rows)
                    {
                        //Check if log is with in the Year/Month/Day/Time Schedule
                        DateTime dtrLogDate = Convert.ToDateTime(dtr.Tel_LogDate);
                        int YearFrom = Convert.ToInt32(dr["From Year"]);
                        int YearTo = Convert.ToInt32(dr["To Year"]);
                        int MonthFrom = Convert.ToInt32(dr["From Month"]);
                        int MonthTo = Convert.ToInt32(dr["To Month"]);
                        int TimeFrom = Convert.ToInt32(dr["From Time"].ToString().Replace(":", ""));
                        int TimeTo = Convert.ToInt32(dr["To Time"].ToString().Replace(":", ""));
                        int dtrLogTime = Convert.ToInt32(dtr.Tel_LogTime);
                        bool IsDayActive = (bool)dr[dtrLogDate.ToString("ddd").ToUpper()];
                        bool IsVerificationActive = (bool)dr["Verify-" + Convert.ToInt32(dtr.Dtr_VerifyMode).ToString()];

                        if ((dr["IP"].ToString().Trim() == DeviceIP) &&
                            (YearFrom <= dtrLogDate.Year) && (YearTo >= dtrLogDate.Year) &&
                            (MonthFrom <= dtrLogDate.Month) && (MonthTo >= dtrLogDate.Month) &&
                            (TimeFrom <= dtrLogTime) && (TimeTo >= dtrLogTime) &&
                            IsDayActive && //Day
                            !IsVerificationActive) //Substandard Log 
                        {
                            return Standard.No; //Log is not standard for DTR.
                        }
                    }
                }

                return Standard.Yes;
            }
            catch
            {
                return Standard.Yes;
            }

        }

        #endregion

        #region Device Real Time Events
        private TextBox TxtReceiver = null;
        public void DeviceRealTimeEvents(bool isDeviceConneted, TextBox txtReceiver)
        {
            try
            {
                if (isDeviceConneted)
                {
                    if (BioDataHelper.RegEvent(iMachineNumber, 65535))//Here you can register the realtime events that you want to be triggered(the parameters 65535 means registering all)
                    {
                        //Attaching Device Real Time Events
                        TxtReceiver = txtReceiver;
                        BioDataHelper.OnHIDNum += new zkemkeeper._IZKEMEvents_OnHIDNumEventHandler(OnCard_Read);
                    }
                }
                else
                {
                    //Detaching Device Real Time Events
                    BioDataHelper.OnHIDNum -= new zkemkeeper._IZKEMEvents_OnHIDNumEventHandler(OnCard_Read);
                }
            }
            catch{ }
        }

        private void OnCard_Read(int CardNumber)
        {
            string CardIDNumber = CardNumber.ToString();

            // Lito added method invoker for allowing to set value of txtbox through cross-thread. 10/31/2014
            if (TxtReceiver.InvokeRequired)
            {
                //TxtReceiver.Text = CardIDNumber;
                TxtReceiver.Invoke(new MethodInvoker(delegate { TxtReceiver.Text = CardIDNumber; }));
            }

            if (TxtReceiver.TabIndex != 200)
            {
                TxtReceiver.TabIndex = 200;
            }
            else
            {
                TxtReceiver.TabIndex = 400;
            }
        }
        #endregion

        #region Time Keeping Manager Business Logic

        public bool InsertRawFingerPrintDatatoDatabase(DeviceEmployeeObjects _zkobject, DALHelper dalServer, String T_TableName)
        {
            int nAffectedRows = 0;
            try
            {
                ParameterInfo[] param = new ParameterInfo[11];

                param[0] = new ParameterInfo("@EmployeeID", _zkobject.Trf_EmployeeID, SqlDbType.VarChar, 15);
                param[1] = new ParameterInfo("@RFID", _zkobject.Trf_RFID, SqlDbType.VarChar, 50);
                param[2] = new ParameterInfo("@FingerIndex", _zkobject.Trf_FingerIndex, SqlDbType.TinyInt);
                //param[2] = new ParameterInfo("@template", "xx", SqlDbType.VarChar, 15);
                param[3] = new ParameterInfo("@template", _zkobject.Trf_Template, SqlDbType.VarChar, _zkobject.Trf_Template.Length);
                param[4] = new ParameterInfo("@Privilege", _zkobject.Trf_Privilege, SqlDbType.Char, 10);
                param[5] = new ParameterInfo("@Password", _zkobject.Trf_Password, SqlDbType.VarChar, 15);
                param[6] = new ParameterInfo("@Enabled", _zkobject.Trf_Enabled, SqlDbType.Bit);
                param[7] = new ParameterInfo("@Flag", _zkobject.Trf_Flag, SqlDbType.TinyInt);
                param[8] = new ParameterInfo("@Ludatetime", _zkobject.ludatetime, SqlDbType.DateTime);
                param[9] = new ParameterInfo("@UsrLogin", _zkobject.Usr_Login, SqlDbType.VarChar, 15);
                param[10] = new ParameterInfo("@FaceData", _zkobject.Trf_FaceData, SqlDbType.VarChar, _zkobject.Trf_FaceData.Length);

                string SQLInsertEmployeeFingerprint = string.Format(Payroll.BLogic.CommonDeviceQueries.CreateEmployeeDeviceData(T_TableName));

                nAffectedRows = dalServer.ExecuteNonQuery(SQLInsertEmployeeFingerprint, CommandType.Text, param);
            }
            catch(Exception e)
            {
                nAffectedRows = -1;
            }
            return (nAffectedRows > 0 ? true : false);
        }

        public bool InsertRawLogstoDatabase(DeviceTimeLogObjects RawLogs, DateTime processDate, String T_TableName)
        {
            bool result = false;
            return InsertRawLogstoDatabase(RawLogs, processDate, T_TableName, ref result);
        }

        public bool InsertRawLogstoDatabase(DeviceTimeLogObjects RawLogs, DateTime processDate, String T_TableName, ref bool NoExeption)
        {
            return InsertRawLogstoDatabase(RawLogs, processDate, T_TableName, ref NoExeption, "", "");
        }

        public bool InsertRawLogstoDatabase(DeviceTimeLogObjects RawLogs, 
                                            DateTime processDate,
                                            String T_TableName, 
                                            ref bool NoExeption, 
                                            String DeviceIP, 
                                            String StationDescription)
        {
            //Nilo Additional Note:
            //Make sure that GetEmployeeIDMappingTable is called before executing this function.
            //This is to properly set correct reference ID from Mapping Employee ID
            //When GetReferenceID function is called.
            
            bool bRetVal = false;
            int nAffectedRows = 0;
            ParameterInfo[] paramInsertDTR = new ParameterInfo[1];
            string query = "";

            DeviceDataObjectHelper deviceHelper = new DeviceDataObjectHelper();
            if (deviceHelper.IsStandard(RawLogs.Dtr_Standard))
            {
                #region parameters

                paramInsertDTR = new ParameterInfo[9];

                paramInsertDTR[0] = new ParameterInfo("@Tel_IDNo", GetReferenceID(RawLogs.Tel_IDNo.Trim()), SqlDbType.VarChar, 15);
                paramInsertDTR[1] = new ParameterInfo("@Tel_LogDate", RawLogs.Tel_LogDate, SqlDbType.VarChar, 10);
                paramInsertDTR[2] = new ParameterInfo("@Tel_LogTime", RawLogs.Tel_LogTime, SqlDbType.Char, 4);
                paramInsertDTR[3] = new ParameterInfo("@Tel_LogType", RawLogs.Tel_LogType, SqlDbType.Char, 1);
                paramInsertDTR[4] = new ParameterInfo("@Tel_StationNo", string.IsNullOrEmpty(_ip) ? "TK" : _ip, SqlDbType.Char, 2);
                paramInsertDTR[5] = new ParameterInfo("@Tel_IsPosted", 0, SqlDbType.Bit);
                paramInsertDTR[6] = new ParameterInfo("@Tel_IsUploaded", RawLogs.Tel_IsUploaded, SqlDbType.Bit);
                paramInsertDTR[7] = new ParameterInfo("@Usr_Login", RawLogs.Usr_login, SqlDbType.VarChar, 15);
                try
                {
                    paramInsertDTR[8] = new ParameterInfo("@LudateTime", Convert.ToDateTime(RawLogs.Dtr_RawDateTime), SqlDbType.DateTime);
                }
                catch
                {
                    paramInsertDTR[8] = new ParameterInfo("@LudateTime", RawLogs.ludatetime, SqlDbType.DateTime);
                }

                #endregion
                #region query
                query = string.Format(@"IF(ISDATE(@Tel_LogDate) = 1)
                                        BEGIN
                                            INSERT INTO {0} 
                                                (Tel_IDNo, Tel_LogDate, Tel_LogTime, Tel_LogType, Tel_StationNo, Tel_IsPosted, Tel_IsUploaded, Usr_Login, LudateTime)            
                                            VALUES (@Tel_IDNo, @Tel_LogDate, @Tel_LogTime, @Tel_LogType, @Tel_StationNo, @Tel_IsPosted, @Tel_IsUploaded, @Usr_Login, @LudateTime)
                                        END"
                                        , T_TableName);
                #endregion
            }
            else
            {
                #region Parameters

                paramInsertDTR = new ParameterInfo[12];

                paramInsertDTR[0] = new ParameterInfo("@EmployeeID", GetReferenceID(RawLogs.Tel_IDNo.Trim()), SqlDbType.VarChar, 15);
                paramInsertDTR[1] = new ParameterInfo("@LogDate", RawLogs.Tel_LogDate, SqlDbType.VarChar, 10);
                paramInsertDTR[2] = new ParameterInfo("@LogTime", RawLogs.Tel_LogTime, SqlDbType.Char, 4);
                paramInsertDTR[3] = new ParameterInfo("@LogType", RawLogs.Tel_LogType, SqlDbType.Char, 1);
                paramInsertDTR[4] = new ParameterInfo("@LogDateTime", RawLogs.Dtr_RawDateTime, SqlDbType.DateTime);
                paramInsertDTR[5] = new ParameterInfo("@SubStandardID", RawLogs.Tel_IDNo, SqlDbType.Char, 15);
                paramInsertDTR[6] = new ParameterInfo("@VerificationType", deviceHelper.GetVerificationDesciption(RawLogs.Dtr_VerifyMode), SqlDbType.Char, 15);
                paramInsertDTR[7] = new ParameterInfo("@StationType", StationDescription, SqlDbType.Char, 25);
                paramInsertDTR[8] = new ParameterInfo("@StationNo", DeviceIP, SqlDbType.Char, 25);
                paramInsertDTR[9] = new ParameterInfo("@UploadedFlag", RawLogs.Tel_IsUploaded, SqlDbType.Bit);
                paramInsertDTR[10] = new ParameterInfo("@Usr_Login", RawLogs.Usr_login, SqlDbType.VarChar, 15);

                try
                {
                    paramInsertDTR[11] = new ParameterInfo("@LudateTime", Convert.ToDateTime(RawLogs.Dtr_RawDateTime).ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss"), SqlDbType.DateTime);
                }
                catch
                {
                    paramInsertDTR[11] = new ParameterInfo("@LudateTime", RawLogs.ludatetime.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss"), SqlDbType.DateTime);
                }

                #endregion
                #region Query
                query = @"    INSERT INTO [dbo].[T_EmpSubstandardDTR]
                                               ([Tsd_IDNo], 
                                                [Tsd_LogDate], 
                                                [Tsd_LogTime], 
                                                [Tsd_LogType], 
                                                [Tsd_LogDateTime], 
                                                [Tsd_SubStandardID], 
                                                [Tsd_VerificationType], 
                                                [Tsd_StationType], 
                                                [Tsd_StationNo], 
                                                [Tsd_UploadedDate],
                                                [Usr_Login], 
                                                [LudateTime])
                                        VALUES
                                               (@EmployeeID, 
                                                @LogDate, 
                                                @LogTime, 
                                                @LogType, 
                                                @LogDateTime, 
                                                @SubStandardID, 
                                                @VerificationType,
                                                @StationType,
                                                @StationNo, 
                                                @UploadedFlag, 
                                                @Usr_Login, 
                                                @LudateTime)";
                #endregion
            }

            try
            {
                nAffectedRows = _DALServer.ExecuteNonQuery(query, CommandType.Text, paramInsertDTR);
                if (nAffectedRows > 0)
                {
                    DeviceGlobals.DeviceLogCtr = DeviceGlobals.DeviceLogCtr + nAffectedRows;
                    bRetVal = true;
                }

            }
            catch(SqlException sqlex)
            {
                if (sqlex.Number == 2627)
                {
                    //Primariky error
                }
                else
                {
                    NoExeption = false;
                }
                bRetVal = false;
            }
            catch (Exception ex)
            {
                bRetVal = false;
                NoExeption = false;
            }

            return bRetVal;
        }

        public void UpdateDatabaseDeviceProperties(String DeviceIP,String TableName)
        {
            int affected = 0;
            DataTable dtResult = new DataTable();
            DataRow[] arrResult;
            string[] tmpDeviceIP;
            //DeviceIP = Encrypt.encryptText(DeviceIP); //Encrypt IP

            String qSelect = @"SELECT Mtd_DeviceIP [TmpDeviceIp]
                               , Mtd_DeviceIP [OrigDeviceId]
                               FROM M_TerminalDevice2
                               WHERE Mtd_RecordStatus = 'A'";

            dtResult = _DALServer.ExecuteDataSet(qSelect).Tables[0];
            for (int i = 0; i < dtResult.Rows.Count; i++)
            {
                tmpDeviceIP = Encrypt.decryptText(dtResult.Rows[i]["TmpDeviceIp"].ToString()).Split('_');
                dtResult.Rows[i]["TmpDeviceIp"] = tmpDeviceIP[0];
            }
            arrResult = dtResult.Select("TmpDeviceIp = '" + DeviceIP + "'");
            if (arrResult.Length > 0)
                DeviceIP = arrResult[0].ItemArray[1].ToString();
            ParameterInfo[] param = new ParameterInfo[4];
            param[0] = new ParameterInfo("@DeviceIP",DeviceIP,SqlDbType.VarChar,50);
            param[1] = new ParameterInfo("@TableName",TableName,SqlDbType.VarChar,100);
            param[2] = new ParameterInfo("@LastLoaded",FormatTableName(TableName),SqlDbType.VarChar,100);
            param[3] = new ParameterInfo("@UsrLogin", DeviceGlobals._UsrLogin, SqlDbType.VarChar, 100);
            String sql = string.Format(@"UPDATE [M_TerminalDevice2]
                                         SET [Mtd_LastLoadedTable] = @LastLoaded
                                              ,[Mtd_TableName] = @TableName
                                              ,[Usr_Login] = @UsrLogin
                                              ,[Ludatetime] = GETDATE()
                                         WHERE Mtd_DeviceIP=@DeviceIP and Mtd_RecordStatus='A'");
            try
            {
                if (ConnectToServer())
                {
                    _DALServer.BeginTransaction();
                    affected = _DALServer.ExecuteNonQuery(sql, CommandType.Text, param);
                    _DALServer.CommitTransaction();
                }
            }
            catch(Exception e)
            {
                _DALServer.RollBackTransaction();
                ExeptionCatcher(e, "UpdateDeviceProperties : Rollback");
            }
            finally 
            {
                DisconnectToServer();
            }
        }

        public bool SynchronizeDatabaseFingerprintData(String T_TableName1, String T_TableName2)
        {
            int affected = 0;
            String querySync1 = string.Format(@"INSERT INTO [{0}]
                                                SELECT * FROM [{1}]
                                                WHERE CONVERT(VARCHAR,Trf_EmployeeID) + '_' + CONVERT(VARCHAR,Trf_FingerIndex)
                                                NOT IN
                                                (SELECT CONVERT(VARCHAR,Trf_EmployeeID) + '_' + CONVERT(VARCHAR,Trf_FingerIndex) 
                                                FROM [{0}])", T_TableName1, T_TableName2);
            String DeleteTable2 = string.Format(@"DELETE FROM [{0}]", T_TableName2);
            String querySync2 = string.Format(@"INSERT INTO [{1}]
                                                SELECT * FROM [{0}]", T_TableName1, T_TableName2);
            try
            {
                if (ConnectToServer())
                {
                    _DALServer.BeginTransaction();
                    affected = _DALServer.ExecuteNonQuery(querySync1, CommandType.Text);
                    affected = _DALServer.ExecuteNonQuery(DeleteTable2, CommandType.Text);
                    affected = _DALServer.ExecuteNonQuery(querySync2, CommandType.Text);
                    _DALServer.CommitTransaction();
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                _DALServer.RollBackTransaction();
                ExeptionCatcher(e, "SynchronizeDatabaseFingerprintData : Rollback");
                return false;
            }
            finally
            {
                DisconnectToServer();
            }
            return true;
        }

        #endregion

        #region Database logs uploading

        public int UploadRawLogstoDatabaseMinMax(DateTime StartDate, DateTime EndDate, String T_Raw)
        {
            int total = 0;
            StartDate = Convert.ToDateTime(StartDate.ToShortDateString());
            EndDate = Convert.ToDateTime(EndDate.ToShortDateString());
            DateTime LogDate = StartDate;
            while (LogDate <= EndDate)
            {
                List<DeviceTimeLogObjects> UploadedDTRList = new List<DeviceTimeLogObjects>();
                DataSet dsRaw = new DataSet();
                ParameterInfo[] param = new ParameterInfo[3];
                int affected = 0;
                param[0] = new ParameterInfo("@StartDate", LogDate.ToShortDateString(), SqlDbType.DateTime);
                param[1] = new ParameterInfo("@EndDate", LogDate.ToShortDateString(), SqlDbType.DateTime);
                param[2] = new ParameterInfo("@Flag", 0, SqlDbType.Bit);
                #region Query
                string sqlRaw = String.Format(@"SELECT DISTINCT Tel_IDNo AS [EMPLOYEE_ID]
                                --LOGDATE
                                ,(SELECT MIN(Tel_LogDate) FROM {0}
                                    WHERE CONVERT(DATETIME,Tel_LogDate,101) BETWEEN @StartDate AND @EndDate
                                    AND Tel_IDNo=DTR.Tel_IDNo
                                    AND Tel_IsUploaded=@FLAG) AS [LOG_DATE]
                                --IN 
                                ,(SELECT MIN(Tel_LogTime) FROM {0}
	                                WHERE CONVERT(DATETIME,Tel_LogDate,101) BETWEEN @StartDate AND @EndDate
	                                AND Tel_IDNo=DTR.Tel_IDNo
	                                AND Tel_IsUploaded=@FLAG) AS [LOG_IN]
                                --OUT
                                ,(SELECT MAX(Tel_LogTime) FROM {0} DEVICE
                                    WHERE CONVERT(DATETIME,Tel_LogDate,101) BETWEEN @StartDate AND @EndDate
                                    AND Tel_IDNo=DTR.Tel_IDNo
                                    AND DEVICE.Tel_LogTime!=(SELECT MIN(Tel_LogTime) FROM {0}
							                                WHERE CONVERT(DATETIME,Tel_LogDate,101) BETWEEN @StartDate AND @EndDate
							                                AND Tel_IDNo=DEVICE.Tel_IDNo
							                                AND Tel_IsUploaded=@FLAG)
	                                AND Tel_IsUploaded=@FLAG) AS [LOG_OUT]
                                ,(SELECT TOP 1 Tel_StationNo FROM {0}
	                                  WHERE CONVERT(DATETIME,Tel_LogDate,101) BETWEEN @StartDate AND @EndDate
	                                  AND Tel_IDNo=DTR.Tel_IDNo
	                                  AND Tel_IsUploaded=@FLAG) AS [STATION]
                                FROM {0} DTR WHERE CONVERT(DATETIME,Tel_LogDate,101) BETWEEN @StartDate AND @EndDate", T_Raw);
                #endregion
                try
                {
                    if (ConnectToServer())
                    {
                        _DALServer.OpenDB();
                        _DALServer.BeginTransaction();
                        dsRaw = _DALServer.ExecuteDataSet(sqlRaw, CommandType.Text, param);
                        _DALServer.CommitTransaction();

                        DataTable dtRaw = dsRaw.Tables[0];

                        for (int i = 0; i < dtRaw.Rows.Count; i++)
                        {
                            #region parameters

                            ParameterInfo[] paramInsertDTR = new ParameterInfo[9];
                            if (dtRaw.Rows[i]["LOG_DATE"].ToString().Trim() == "")
                                continue;
                            DateTime Ludatetime = DateTime.Now.AddMilliseconds(i);
                            paramInsertDTR[0] = new ParameterInfo("@Tel_IDNo", dtRaw.Rows[i]["EMPLOYEE_ID"].ToString(), SqlDbType.VarChar, 15);
                            paramInsertDTR[1] = new ParameterInfo("@Tel_LogDate", dtRaw.Rows[i]["LOG_DATE"].ToString(), SqlDbType.VarChar, 10);
                            paramInsertDTR[2] = new ParameterInfo("@Tel_LogTime", dtRaw.Rows[i]["LOG_IN"].ToString(), SqlDbType.Char, 4);//LogsData[2].Trim(), SqlDbType.Char, 4);
                            paramInsertDTR[3] = new ParameterInfo("@Tel_LogType", "I", SqlDbType.Char, 1);
                            paramInsertDTR[4] = new ParameterInfo("@Tel_StationNo", dtRaw.Rows[i]["STATION"].ToString(), SqlDbType.Char, 2);
                            paramInsertDTR[5] = new ParameterInfo("@Tel_IsPosted", 0, SqlDbType.Bit);
                            paramInsertDTR[6] = new ParameterInfo("@Tel_IsUploaded", 0, SqlDbType.Bit);
                            paramInsertDTR[7] = new ParameterInfo("@Usr_Login", "LOGUPLDSRVC", SqlDbType.VarChar, 15);
                            paramInsertDTR[8] = new ParameterInfo("@LudateTime",Ludatetime, SqlDbType.DateTime);

                            #endregion
                            //insert IN
                            try
                            {
                                _DALServer.BeginTransaction();
                                affected = _DALServer.ExecuteNonQuery("spLogReadingInsertToServerDTRMinMax", CommandType.StoredProcedure, paramInsertDTR);
                                _DALServer.CommitTransaction();
                            }
                            catch (Exception e){ExeptionCatcher(e, "UploadMinMax Insert IN: Rollback");}
                            if (affected > 0)
                            {
                                total = total + affected;
                                DeviceTimeLogObjects DTR = new DeviceTimeLogObjects();
                                InitializeTimeLogObjects(DTR);
                                DTR.Tel_IDNo = dtRaw.Rows[i]["EMPLOYEE_ID"].ToString();
                                DTR.Tel_LogDate = dtRaw.Rows[i]["LOG_DATE"].ToString();
                                DTR.Tel_LogTime = dtRaw.Rows[i]["LOG_IN"].ToString();
                                UploadedDTRList.Add(DTR);
                            }

                            affected = 0;
                            if (dtRaw.Rows[i]["LOG_OUT"].ToString().Trim() != "")
                            {
                                paramInsertDTR[2] = new ParameterInfo("@Tel_LogTime", dtRaw.Rows[i]["LOG_OUT"].ToString(), SqlDbType.Char, 4);//LogsData[2].Trim(), SqlDbType.Char, 4);
                                paramInsertDTR[3] = new ParameterInfo("@Tel_LogType", "O", SqlDbType.Char, 1);
                                //insert OUT
                                try
                                {
                                    _DALServer.BeginTransaction();
                                    affected = _DALServer.ExecuteNonQuery("spLogReadingInsertToServerDTRMinMax", CommandType.StoredProcedure, paramInsertDTR);
                                    _DALServer.CommitTransaction();
                                }catch (Exception e){ExeptionCatcher(e, "UploadMinMax Insert OUT: Rollback");}
                                if (affected > 0)
                                {
                                    total = total + affected;
                                    DeviceTimeLogObjects DTR = new DeviceTimeLogObjects();
                                    InitializeTimeLogObjects(DTR);
                                    DTR.Tel_IDNo = dtRaw.Rows[i]["EMPLOYEE_ID"].ToString();
                                    DTR.Tel_LogDate = dtRaw.Rows[i]["LOG_DATE"].ToString();
                                    DTR.Tel_LogTime = dtRaw.Rows[i]["LOG_OUT"].ToString();
                                    UploadedDTRList.Add(DTR);
                                }
                            }
                        }
                    }

                }
                catch (Exception e)
                {
                    ExeptionCatcher(e, "UploadRawLogstoDatabaseMinMax : Rollback");
                }
                finally
                {
                    DisconnectToServer();
                    _DALServer.CloseDB();
                    dsRaw.Dispose();
                }
                if (total > 0)
                {
                    int Updated = 0;
                    Updated = UpdateLogsUploadFlag(UploadedDTRList, T_Raw);
                }
                LogDate = LogDate.AddDays(1);
            }
            if (_showMSG)
            {
                if (total > 0)
                    MessageBox.Show(String.Format(MSG.MinMaxUploadSuccess, total), MSG.Success);
                else
                    MessageBox.Show(String.Format(MSG.AttLogsNoRetrieved, total), MSG.Failed);
            }
            return total;
        }

        public int UploadRawLogstoDatabaseSequential(DateTime StartDate, DateTime EndDate, String T_Raw, bool Sequencial)
        {
            int total = 0;
            string PrevEmpID = string.Empty;
            string CurrEmpID = string.Empty;    
            string NextEmpID = string.Empty;
            string PrevLogDate = string.Empty;
            string CurrLogDate = string.Empty;
            string NextLogDate = string.Empty;
            string CurrLogTime = "0000";
            string PrevLogTime = "0000";
            string LastLogtype = "I";
            StartDate = Convert.ToDateTime(StartDate.ToShortDateString());
            EndDate = Convert.ToDateTime(EndDate.ToShortDateString());
            DateTime LogDate = StartDate;
            while (LogDate <= EndDate)
            {
                List<DeviceTimeLogObjects> UploadedDTRList = new List<DeviceTimeLogObjects>();
                DataSet dsRaw = new DataSet();
                ParameterInfo[] param = new ParameterInfo[3];
                int affected = 0;
                param[0] = new ParameterInfo("@StartDate", LogDate.ToShortDateString(), SqlDbType.DateTime);
                param[1] = new ParameterInfo("@EndDate", LogDate.ToShortDateString(), SqlDbType.DateTime);
                param[2] = new ParameterInfo("@Flag", 0, SqlDbType.Bit);
                #region Query
                string sqlRaw = String.Format(@"SELECT [Tel_IDNo] as EMPLOYEE_ID
                                                  ,[Tel_LogDate] as LOG_DATE
                                                  ,[Tel_LogTime] as LOG_TIME
                                                  ,[Tel_LogType] as LOG_TYPE
                                                  ,[Tel_StationNo] as STATION
                                                  ,[Tel_IsPosted]
                                                  ,[Usr_Login]
                                                  ,[LudateTime]
                                                  ,[Tel_IsUploaded]
                                                FROM {0} DTR WHERE CONVERT(DATETIME,Tel_LogDate,101) BETWEEN @StartDate AND @StartDate
                                                ORDER BY Tel_IDNo, CONVERT(DATETIME,Tel_LogDate),Tel_LogTime", T_Raw);
                #endregion
                try
                {
                    if (ConnectToServer())
                    {
                        _DALServer.OpenDB();
                        try
                        { 
                            //Added : Forcefully remove logs that date time are incompatible.
                            _DALServer.ExecuteNonQuery(String.Format(@"DELETE {0} WHERE ISDATE(Tel_LogDate) = 0", T_Raw));
                        }
                        catch
                        { }
                        _DALServer.BeginTransaction();
                        dsRaw = _DALServer.ExecuteDataSet(sqlRaw, CommandType.Text, param);
                        _DALServer.CommitTransaction();

                        DataTable dtRaw = dsRaw.Tables[0];

                        for (int i = 0; i < dtRaw.Rows.Count; i++)
                        {
                            #region parameters
                            bool SameLogGrp = true;
                            ParameterInfo[] paramInsertDTR = new ParameterInfo[9];
                            DateTime Ludatetime = DateTime.Now.AddMilliseconds(i);
                            paramInsertDTR[0] = new ParameterInfo("@Tel_IDNo", dtRaw.Rows[i]["EMPLOYEE_ID"].ToString(), SqlDbType.VarChar, 15);
                            paramInsertDTR[1] = new ParameterInfo("@Tel_LogDate", dtRaw.Rows[i]["LOG_DATE"].ToString(), SqlDbType.VarChar, 10);
                            paramInsertDTR[2] = new ParameterInfo("@Tel_LogTime", dtRaw.Rows[i]["LOG_TIME"].ToString(), SqlDbType.Char, 4);//LogsData[2].Trim(), SqlDbType.Char, 4);
                            paramInsertDTR[4] = new ParameterInfo("@Tel_StationNo", dtRaw.Rows[i]["STATION"].ToString(), SqlDbType.Char, 2);
                            paramInsertDTR[5] = new ParameterInfo("@Tel_IsPosted", 0, SqlDbType.Bit);
                            paramInsertDTR[6] = new ParameterInfo("@Tel_IsUploaded", 0, SqlDbType.Bit);
                            paramInsertDTR[7] = new ParameterInfo("@Usr_Login", DeviceGlobals._UsrLogin, SqlDbType.VarChar, 15);
                            paramInsertDTR[8] = new ParameterInfo("@LudateTime", Ludatetime, SqlDbType.DateTime);

                            #endregion
                            //insert IN
                            //paramInsertDTR[3] = new ParameterInfo("@Tel_LogType", "I", SqlDbType.Char, 1);  
                            try
                            {
                                if(i>0)
                                {
                                    PrevEmpID = dtRaw.Rows[i-1]["EMPLOYEE_ID"].ToString().Trim();
                                    CurrEmpID = dtRaw.Rows[i]["EMPLOYEE_ID"].ToString().Trim();
                                   
                                    PrevLogDate = dtRaw.Rows[i-1]["LOG_DATE"].ToString().Trim();
                                    CurrLogDate = dtRaw.Rows[i]["LOG_DATE"].ToString().Trim();
                                    
                                    PrevLogTime = dtRaw.Rows[i-1]["LOG_TIME"].ToString().Trim();
                                    CurrLogTime = dtRaw.Rows[i]["LOG_TIME"].ToString().Trim();
                                }

                                //Validate log as IN/OUT
                                if (i == 0 || PrevEmpID != CurrEmpID || PrevLogDate != CurrLogDate)
                                {
                                    paramInsertDTR[3] = new ParameterInfo("@Tel_LogType", "I", SqlDbType.Char, 1);  
                                }
                                else if(_isTimeGapValid(ToDateFormat(CurrLogDate, CurrLogTime), ToDateFormat(PrevLogDate, PrevLogTime))
                                    && PrevEmpID == CurrEmpID && PrevLogDate == CurrLogDate)
                                {
                                    if (LastLogtype == "I")
                                    {
                                        paramInsertDTR[3] = new ParameterInfo("@Tel_LogType", "O", SqlDbType.Char, 1);
                                        LastLogtype = "O";
                                    }
                                    else
                                    {
                                        paramInsertDTR[3] = new ParameterInfo("@Tel_LogType", "I", SqlDbType.Char, 1);
                                        LastLogtype = "I";
                                    }
                                }
                                else if (!_isTimeGapValid(ToDateFormat(CurrLogDate, CurrLogTime), ToDateFormat(PrevLogDate, PrevLogTime))
                                    && PrevEmpID == CurrEmpID && PrevLogDate == CurrLogDate)
                                {
                                    paramInsertDTR[3] = new ParameterInfo("@Tel_LogType", LastLogtype, SqlDbType.Char, 1);
                                }                               
                                
                                //Validate log as Last Log : OUT
                                if(i < dtRaw.Rows.Count-1 && i!=0)
                                {
                                    NextEmpID = dtRaw.Rows[i+1]["EMPLOYEE_ID"].ToString().Trim();
                                    NextLogDate = dtRaw.Rows[i+1]["LOG_DATE"].ToString().Trim();

                                    if((CurrEmpID!=NextEmpID || CurrLogDate!=NextLogDate) && (PrevEmpID==CurrEmpID && PrevLogDate==CurrLogDate))
                                        paramInsertDTR[3] = new ParameterInfo("@Tel_LogType", "O", SqlDbType.Char, 1);  
                                }
                                if (i == dtRaw.Rows.Count - 1)
                                {
                                    if (PrevEmpID == CurrEmpID && PrevLogDate == CurrLogDate)
                                        paramInsertDTR[3] = new ParameterInfo("@Tel_LogType", "O", SqlDbType.Char, 1);  
                                }
                               
                                _DALServer.BeginTransaction();
                                affected = _DALServer.ExecuteNonQuery("spLogReadingInsertToServerDTR_2", CommandType.StoredProcedure, paramInsertDTR);
                                _DALServer.CommitTransaction();

                            }
                            catch (Exception e) { ExeptionCatcher(e, "UploadMinMax Sequencial: Rollback"); }
                            if (affected > 0)
                            {
                                total = total + affected;
                                DeviceTimeLogObjects DTR = new DeviceTimeLogObjects();
                                InitializeTimeLogObjects(DTR);
                                DTR.Tel_IDNo = dtRaw.Rows[i]["EMPLOYEE_ID"].ToString();
                                DTR.Tel_LogDate = dtRaw.Rows[i]["LOG_DATE"].ToString();
                                DTR.Tel_LogTime = dtRaw.Rows[i]["LOG_TIME"].ToString();
                                UploadedDTRList.Add(DTR);
                            }

                            DeviceGlobals.Progress = ((Convert.ToDouble(i) + 1) / Convert.ToDouble(dtRaw.Rows.Count)) * 100;
                        }
                    }

                }
                catch (Exception e)
                {
                    ExeptionCatcher(e, "UploadRawLogstoDatabaseSequential: General");
                }
                finally
                {
                    DisconnectToServer();
                    _DALServer.CloseDB();
                    dsRaw.Dispose();
                }
                if (total > 0)
                {
                    int Updated = 0;
                    Updated = UpdateLogsUploadFlag(UploadedDTRList, T_Raw);
                }
                LogDate = LogDate.AddDays(1);
            }
            if (_showMSG)
            {
                if (total > 0)
                    MessageBox.Show(String.Format(MSG.MinMaxUploadSuccess, total), MSG.Success);
                else
                    MessageBox.Show(String.Format(MSG.AttLogsNoRetrieved, total), MSG.Failed);
            }
            return total;
        }

        public int UpdateLogsUploadFlag(List<DeviceTimeLogObjects> DTRList, String T_TableName)
        {
            int affected = 0;
            int Updated = 0;
            if (ConnectToServer())
            {
                string sql;
                foreach (DeviceTimeLogObjects DTR in DTRList)
                {
                    ParameterInfo[] param = new ParameterInfo[4];
                    param[0] = new ParameterInfo("@EmployeeID", DTR.Tel_IDNo, SqlDbType.VarChar, 15);
                    param[1] = new ParameterInfo("@LogDate", DTR.Tel_LogDate, SqlDbType.VarChar, 10);
                    param[2] = new ParameterInfo("@LogTime", DTR.Tel_LogTime, SqlDbType.Char, 4);
                    param[3] = new ParameterInfo("@UsrLogin", DTR.Usr_login, SqlDbType.VarChar, 15);
                    sql = string.Format(@"UPDATE {0} SET Tel_IsUploaded='1',USR_LOGIN=@UsrLogin,LudateTime=GETDATE()
                                            WHERE Tel_IDNo=@EmployeeID
                                            AND Tel_LogDate=@LogDate
                                            AND Tel_LogTime=@LogTime", T_TableName);
                    try
                    {
                        _DALServer.BeginTransaction();
                        affected = _DALServer.ExecuteNonQuery(sql, CommandType.Text, param);
                        _DALServer.CommitTransaction();
                        if (affected>0)
                            Updated = Updated + affected;
                    }
                    catch (Exception e)
                    {
                        _DALServer.RollBackTransaction();
                        ExeptionCatcher(e, "UpdateLogsUploadFlag : Rollback");
                    }

                }
                DisconnectToServer();
            }
            return Updated;
        }

        public int USBTextFileUploading(String _File, DateTime _StartDate, DateTime _EndDate, String T_TableName, bool ShowMsg, ref bool Success)
        {
            bool CurrentShowMsgStatus = _showMSG;
            _showMSG = false;
            int ret = USBTextFileUploading(_File, _StartDate, _EndDate, T_TableName, ref Success);
            _showMSG = CurrentShowMsgStatus;
            return ret;
        }

        public int USBTextFileUploading(String _File, DateTime _StartDate, DateTime _EndDate, String T_TableName)
        { 
            bool Result = false;
            return USBTextFileUploading(_File, _StartDate, _EndDate, T_TableName, ref Result);
        }

        public int USBTextFileUploading(String _File, DateTime _StartDate, DateTime _EndDate, String T_TableName, ref bool Success)
        {
            int affected = 0;
            DeviceGlobals.DeviceLogCtr = 0;
            try
            {
                String LogsLine = "";
                String[] LogsData;
                List<DeviceTimeLogObjects> TimeLogs = new List<DeviceTimeLogObjects>();
                double Max = 0;

                _StartDate = Convert.ToDateTime(_StartDate.ToString("MM/dd/yyyy"));
                _EndDate = Convert.ToDateTime(_EndDate.ToString("MM/dd/yyyy"));
                
                if (ConnectToServer())
                {
                    if (DeviceGlobals.dtEmployeeIDMapping == null)
                    {
                        GetEmployeeIDMappingTable();
                    }
                    else if (DeviceGlobals.dtEmployeeIDMapping.Rows.Count <= 0)
                    {
                        GetEmployeeIDMappingTable();
                    }

                    using (StreamReader objReader = new StreamReader(_File))
                    {
                        while (objReader.Peek() != -1)
                        {
                            Max = ++Max;
                            LogsLine = objReader.ReadLine();
                            if (LogsLine.Trim() != String.Empty)
                            {
                                LogsData = LogsLine.Split('\t');
                                DeviceTimeLogObjects zklogs = new DeviceTimeLogObjects();
                                InitializeTimeLogObjects(zklogs);
                                zklogs.Tel_IDNo = LogsData[0].Trim();
                                zklogs.Tel_LogDate = Convert.ToDateTime(LogsData[1].Trim()).ToString("MM/dd/yyyy");
                                zklogs.Tel_LogTime = Convert.ToDateTime(LogsData[1].Trim()).ToString("HHmm");
                                zklogs.Tel_LogType = LogsData[3].Trim() == "0" ? "I" : "O";
                                zklogs.Dtr_RawDateTime = LogsData[1].Trim();
                                TimeLogs.Add(zklogs);
                            }

                        }
                        objReader.Close();
                        objReader.Dispose();
                    }

                    int i = 0;
                    bool InsertProcessResult = true;

                    foreach (DeviceTimeLogObjects TimeLog in TimeLogs)
                    {
                        if (Convert.ToDateTime(TimeLog.Tel_LogDate) >= _StartDate && Convert.ToDateTime(TimeLog.Tel_LogDate) <= _EndDate)
                            {
                                //Saving text log to db
                                _ip = "TK";
                                Success = true;
                                InsertRawLogstoDatabase(TimeLog, Convert.ToDateTime(TimeLog.Tel_LogDate), T_TableName, ref Success);
                                if (!Success)
                                { 
                                    //Verified that there is an exeption on inserting dtr data/ Not all data is inserted.
                                    InsertProcessResult = Success;
                                }
                                ErrCode.NoAttLogs = false;
                            }
                            
                            DeviceGlobals.Progress = ((Convert.ToDouble(i) + 1) / Convert.ToDouble(Max)) * 100;
                            ++i;
                    }

                    Success = InsertProcessResult;
                    TimeLogs = null; //Dispose
                    DisconnectToServer();
                    affected = DeviceGlobals.DeviceLogCtr;
                    //This point no exeption is encountered
                    
                    if (_showMSG)
                    {
                        if (!ErrCode.NoAttLogs)
                            MessageBox.Show(String.Format(MSG.AttLogsUploadSuccess, affected), MSG.Success);
                        else if (ErrCode.NoAttLogs)
                            MessageBox.Show(MSG.AttLogsNoRetrieved, MSG.Failed);
                        else
                            MessageBox.Show(MSG.AttLogsAlreadyUploaded, MSG.Success);
                    }
                }
                else
                {
                    if (_showMSG)
                        MessageBox.Show(MSG.DBDisconnected, MSG.Failed);
                    affected = ErrCode.DBDisconnected;
                }
            }
            catch (Exception e)
            {
                Success = false;
                ExeptionCatcher(e, "USBTextFileUploading");
            }

            return affected;
        }

        public int DeviceRawLogstoAttDataArchived(String Path, List<DeviceTimeLogObjects> DTRList, ref bool Success)
        {
            //Archiving Device Logs to Text file preparation for device logs clean up.

            int affected = 0;

            try
            {
                string FileName = "Attlog_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".dat";
                Logger log = new Logger();
                foreach (DeviceTimeLogObjects dtr in DTRList)
                {
                    string Logdata = dtr.Tel_IDNo + "\t"
                                    + dtr.Dtr_RawDateTime + "\t"
                                    + "X" + "\t"
                                    + ((dtr.Tel_LogType == "I") ? "0" : "1") + "\t"
                                    + "X" + "\t"
                                    + "X" + "\t"
                                    + (int)dtr.Dtr_VerifyMode;

                    if (log.WriteText(Path, FileName, Logdata, true))
                    {
                        ++affected;
                    }
                }

                if (DTRList.Count == affected)
                {
                    Success = true;
                }
            }
            catch
            {
                Success = false;
            }

            return affected;
        }

        public bool _isTimeGapValid(DateTime Log2, DateTime Log1)
        {
            bool valid = false;
            try
            {
                TimeSpan Span = Log2.Subtract(Log1);
                Double CapturedGap = Convert.ToDouble(Span.TotalMinutes);
                if (DeviceGlobals.TimeGap <= CapturedGap)
                    valid = true;
            }
            catch (Exception e)
            {
                _log.WriteLog(Application.StartupPath, "Posting", "_isTimeGapValid : General", e.ToString(), true);
            }
            return valid;
        }

        public DateTime ToDateFormat(string LogDate, string LogTime)
        { 
            //string x = string.Format("{0} {1}:{2}:00 {3}",LogDate,LogTime.Substring(0,2),LogTime.Substring(2,2),Convert.ToInt16(LogTime)<1200?"AM":"PM");
            DateTime RetDate = Convert.ToDateTime(string.Format("{0} {1}:{2}:00 {3}",LogDate,LogTime.Substring(0,2),LogTime.Substring(2,2),Convert.ToInt16(LogTime)<1200?"AM":"PM"));
            return RetDate;
        }

        public void GetEmployeeIDMappingTable()
        {
            try
            {
                DeviceGlobals.dtEmployeeIDMapping = _DALServer.ExecuteDataSet(Payroll.BLogic.CommonDeviceQueries.GenerateDeviceIDMapping(), CommandType.Text).Tables[0];
            }
            catch(Exception e)
            {
                ExeptionCatcher(e, "GetEmployeeIDMappingTable");   
            }
        }

        private String GetReferenceID(string DeviceID)
        {
            string EmpName = "";
            return GetReferenceID(DeviceID, out EmpName);
        }

        public String GetReferenceID(string DeviceID, out string EmployeeName)
        {
            EmployeeName = "";

            try
            {
                foreach (DataRow dr in DeviceGlobals.dtEmployeeIDMapping.Rows)
                {
                    bool Isfound = false;

                    //Search base on mapping.
                    if (dr["EmployeeID"].ToString().Trim().Equals(DeviceID))
                    {
                        if (!string.IsNullOrEmpty(dr["MappingID"].ToString().Trim()))
                        {
                            DeviceID = dr["MappingID"].ToString().Trim();
                            try
                            {
                                //Get Name

                                int len = dr["EmployeeName"].ToString().Length;
                                EmployeeName = dr["EmployeeName"].ToString().Substring(0, len < 20 ? len : 19);
                            }
                            catch { }
                            Isfound = true;
                        }
                    }
                    else
                    {
                        try
                        {
                            //Search base on integer value of employeeid.
                            int LogMasterID = Convert.ToInt32(dr["EmployeeID"].ToString().Trim());
                            int intDeviceID = Convert.ToInt32(DeviceID);

                            if (LogMasterID == intDeviceID)
                            {
                                DeviceID = string.IsNullOrEmpty(dr["MappingID"].ToString().Trim()) ? dr["EmployeeID"].ToString().Trim() : dr["MappingID"].ToString().Trim();
                                try
                                {
                                    //Get Name

                                    int len = dr["EmployeeName"].ToString().Length;
                                    EmployeeName = dr["EmployeeName"].ToString().Substring(0, len < 20 ? len : 19);
                                }
                                catch { }
                                Isfound = true;
                            }
                        }
                        catch { }
                    }

                    if (Isfound)
                    {
                        break;
                    }
                }
            }
            catch(Exception e)
            {
                ExeptionCatcher(e, "GetReferenceID " + DeviceID);
            }

            if (string.IsNullOrEmpty(EmployeeName))
            {
                EmployeeName = DeviceID;
            }

            return DeviceID;
        }

        #endregion

        #region Common Methods

        public String FormatTableName(String TableName)
        {
            try
            {
                Regex exp = new Regex(@" (?<=[A-Z])(?=[A-Z][a-z]) | 
                                     (?<=[^A-Z])(?=[A-Z]) | 
                                     (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);
                TableName = TableName.Replace("T_EmpRFFP_", "");
                TableName = exp.Replace(TableName, " ");
            }
            catch (Exception e) { ExeptionCatcher(e, "FormatTableName"); }
            return TableName;
        }

        public String FormatTableName(String TableName, bool bInsert)
        {
            try
            {
                TableName = TableName.Replace(" ", "");
                TableName = TableName.Replace("\t", "");
                if (bInsert)
                    TableName = string.Format("T_EmpRFFP_{0}", TableName);
                else
                    TableName = string.Format("T_EmpRF{0}", TableName);
            }
            catch (Exception e) { ExeptionCatcher(e, "FormatTableName DB"); }
            return TableName;
        }

        public void ExeptionCatcher(Exception e, String FuncName)
        {
            FuncName = string.Format("{0} : ", FuncName);
            if (_showMSG)
                MessageBox.Show(FuncName + e.StackTrace, MSG.Err);
            else
                _log.WriteLog(ErrCode.PathLog, ErrCode.ErrLog, FuncName, e.StackTrace, true);
        }

        private DateTime GetServerDateTime()
        {
            string ServerDateTime = string.Empty;
            try
            {
                if (ConnectToServer())
                {
                    String sql = @"SELECT CONVERT(CHAR,GETDATE(),120) [DATETIME]";
                    try
                    {
                        ServerDateTime = _DALServer.ExecuteScalar(sql).ToString().Trim();
                    }
                    catch (Exception e) { ExeptionCatcher(e, "SetDeviceDateTime : Rollback"); }
                    finally
                    {
                        DisconnectToServer();
                    }
                }
            }
            catch (Exception e)
            {
                _log.WriteLog(ErrCode.PathLog, ErrCode.ErrLog, "SetDeviceDateTime", e.StackTrace, true);
            }
            return string.IsNullOrEmpty(ServerDateTime) ? DateTime.UtcNow : Convert.ToDateTime(ServerDateTime);
        }

        #endregion

    }

}
