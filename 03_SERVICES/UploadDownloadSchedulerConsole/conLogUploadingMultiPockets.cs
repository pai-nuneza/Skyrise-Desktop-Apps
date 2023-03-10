using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;
using CommonPostingLibrary;
using Posting.DAL;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace MultiPackets
{
    public class LogUploadingMultiPockets
    {
        #region Member Variables
        DateTime _processDate = DateTime.Today;
        NLLogger.Logger _log = new NLLogger.Logger();
        #endregion

        #region Public Methods

        public void UploadMutiPackets(string EmploeeID)
        {
            MultiPocketVar.ProgressProcess = string.Format("Start posting time logs on {0}", _processDate.ToShortDateString());
            _log.WriteLog(Application.StartupPath, "Posting", "Posting Service Started", "", true);

            LogUploadingManager uploadManager = new LogUploadingManager(CommonProcedures.GetAppSettingConfigString("DBNameDTR", string.Empty));
            uploadManager.CentralProfile = CommonProcedures.GetAppSettingConfigString("CentralDBName", string.Empty);
            List<string> ledgerDBNames = uploadManager.GetListOfDBProfiles(CommonProcedures.GetAppSettingConfigBool("UseDBProfiles", false));

            MultiPacketUploding LogUploading_V2_func = new MultiPacketUploding(CommonProcedures.GetAppSettingConfigString("DBNameDTR", string.Empty));
            MultiPocketVar.SetLoguploadingGlobal();

            foreach (string PayrollDBName in ledgerDBNames)
            {
                try
                {
                    string[] strArr = PayrollDBName.Split(new char[] { '|' });
                    string ledgerDBName = strArr[0];
                    string ledgerCompanyCode = strArr[1];

                    MultiPocketVar.POCKETTIME = Convert.ToDouble((new Posting.BLogic.CommonBL()).GetParameterValueFromPayroll("POCKETTIME", ledgerCompanyCode, ledgerDBName));
                    MultiPocketVar.POCKETGAP = Convert.ToDouble((new Posting.BLogic.CommonBL()).GetParameterValueFromPayroll("POCKETGAP", ledgerCompanyCode, ledgerDBName));

                    Console.WriteLine(String.Format("Date : {0} {1} {2}\n", _processDate.ToShortDateString(), " Start Posting Ledger : ", ledgerDBName));

                    #region Multi Pocket Posting

                    //for Multiple packet processing
                    try
                    {
                        String HHText = DateTime.Now.ToString("HH");
                        List<Pocket> multiPocketList = new List<Pocket>();
                        if (MultiPocketVar._ServiceCode.Trim().ToUpper() == "MULTIPOCKETS")
                        {
                            if (Convert.ToInt16(HHText) >= 23) //hardcoded for multipocket processing for previous day to run within the day
                            {
                                multiPocketList = LogUploading_V2_func.PocketPairingWOLogType(LogUploading_V2_func.dtRegShift(_processDate, ledgerDBName, ledgerCompanyCode, uploadManager.CentralProfile, EmploeeID));
                                uploadManager.CheckExtIfFull(multiPocketList, ledgerDBName, true, _processDate);
                                multiPocketList = LogUploading_V2_func.PocketPairingWOLogType(LogUploading_V2_func.dtGrveShift(_processDate, ledgerDBName, ledgerCompanyCode, uploadManager.CentralProfile));
                                uploadManager.CheckExtIfFull(multiPocketList, ledgerDBName, false, _processDate);
                                _log.WriteLog(Application.StartupPath, "Posting", "MULTIPOCKET Posting Done: Via Schedule", "", true);

                            }
                            else
                            {
                                multiPocketList = LogUploading_V2_func.PocketPairingWOLogType(LogUploading_V2_func.dtRegShift(_processDate.AddDays(-1), ledgerDBName, ledgerCompanyCode, uploadManager.CentralProfile, EmploeeID));
                                uploadManager.CheckExtIfFull(multiPocketList, ledgerDBName, true, _processDate);
                                multiPocketList = LogUploading_V2_func.PocketPairingWOLogType(LogUploading_V2_func.dtGrveShift(_processDate.AddDays(-1), ledgerDBName, ledgerCompanyCode, uploadManager.CentralProfile));
                                uploadManager.CheckExtIfFull(multiPocketList, ledgerDBName, false, _processDate);
                                _log.WriteLog(Application.StartupPath, "Posting", "MULTIPOCKET Posting previous day Done: Via Schedule", "", true);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        _log.WriteLog(Application.StartupPath, "Posting", "MULTIPOCKET processing", e.ToString(), true);
                    }

                    #endregion

                    #region Set Global Variables

                    try
                    {
                        MultiPocketVar.T_Ledger = "T_EmpTimeRegister";
                        MultiPocketVar.T_Overtime = "T_EmpOvertime";
                        uploadManager.LedgerDBName = ledgerDBName;
                        uploadManager.LedgerDBCompanyCode = ledgerCompanyCode;
                        uploadManager.IsFILO = uploadManager.IsParameterActive("FILO");
                        uploadManager.FourPockets = CommonProcedures.GetAppSettingConfigBool("FourPockets", true);
                        uploadManager.Plus24Hours = CommonProcedures.GetAppSettingConfigBool("Plus24Hours", false);
                        MultiPocketVar.LedgerDBName = ledgerDBName;
                        MultiPocketVar.LedgerDBCompanyCode = ledgerCompanyCode;
                        MultiPocketVar.setFILO = uploadManager.IsFILO;
                        MultiPocketVar.ValidTimeGap = Convert.ToDouble((new Posting.BLogic.CommonBL()).GetParameterValueFromPayroll("TIMEGAP", ledgerCompanyCode, ledgerDBName));
                    }
                    catch (Exception e)
                    {
                        _log.WriteLog(Application.StartupPath, "Posting", "UploadTheLogs : Checking Parameters", e.ToString(), true);
                    }

                    #endregion

                    #region Typical Log Posting

                    #region Ledger data retrieval

                    List<EmployeeLedger> ledgerList = uploadManager.GetEmployeesLedger(_processDate);

                    if (ledgerList.Count < 1)
                    {
                        _log.WriteLog(Application.StartupPath, "Posting", "Posting Service On LedgerHist Started", "", true);

                        //getting ledgerhist
                        MultiPocketVar.T_Ledger = "T_EmpTimeRegisterHst";
                        MultiPocketVar.T_Overtime = "T_EmpOvertimeHst";
                        ledgerList = uploadManager.GetEmployeesLedger(_processDate);
                        if (ledgerList.Count < 1)
                            continue;
                    }

                    #endregion

                    List<EmployeeDTR> dtrList = uploadManager.GetEmployeesDTR(_processDate);

                    if (dtrList.Count < 1)
                    {
                        try
                        {
                            //Start 2nd run ot after midnight capturing
                            OTAfterMidNightFlexShift(uploadManager.LedgerDBName, uploadManager.LedgerDBCompanyCode); 
                            //end
                            NewLogPostingFunctions(uploadManager.LedgerDBName);
                        }
                        catch { }

                        continue;
                    }


                    ledgerList = uploadManager.PostDTRToLedger(dtrList, ledgerList);
                    MultiPocketVar.EmployeeStraightWorkList = uploadManager.GetEmployeeStraightWork(ledgerDBName, _processDate.AddDays(-1), _processDate);
                    MultiPocketVar.GenericCount = ledgerList.Count;
                    MultiPocketVar.Count = 0;
                    ledgerList.ForEach(uploadManager.SaveLedger);
                    MultiPocketVar.Progress = 0;

                    try
                    {
                        //Start 2nt run ot after m idnight capturing
                        OTAfterMidNightFlexShift(uploadManager.LedgerDBName, uploadManager.LedgerDBCompanyCode);
                        NewLogPostingFunctions(uploadManager.LedgerDBName);
                    }
                    catch { }

                    #endregion

                    #region Update Post Flag

                    //updating postflag for denso is separated
                    if (!MultiPocketVar.isDENZOPOSTING || !MultiPocketVar.setFILO)
                    {
                        try
                        {
                            Console.WriteLine(String.Format("\nUpdating Post Flag\n"));
                        }
                        catch { }
                        try
                        {
                            uploadManager.UpdateDTRPostFlag(uploadManager.PostedDTR);
                            //uploadManager.PostedDTR.ForEach(uploadManager.UpdateDTRPostFlag);
                        }
                        catch (Exception Err)
                        {
                            _log.WriteLog(Application.StartupPath, "Posting", "Error updating postflag", Err.Message, true);
                        }
                    }

                    #endregion
                }
                catch (Exception e)
                {
                    _log.WriteLog(Application.StartupPath, "Posting", "UploadTheLogs : LedgerDBNameLoop " + PayrollDBName, e.ToString(), true);
                }

                _log.WriteLog(Application.StartupPath, "Posting", "Posting Service Committed", "", true);

                MultiPocketVar.ProgressProcess = string.Empty;
            }
        }

        public void OTAfterMidNightFlexShift(String ledgerDBName, String ledgerCompanyCode)
        {
            MultiPacketUploding LogUploadingFunc = new MultiPacketUploding(CommonProcedures.GetAppSettingConfigString("DBNameDTR", string.Empty));

            try
            {
                try
                {
                    MultiPocketVar.LedgerDBName = ledgerDBName;
                    MultiPocketVar.LedgerDBCompanyCode = ledgerCompanyCode;
                    MultiPocketVar.ValidTimeGap = Convert.ToDouble((new Posting.BLogic.CommonBL()).GetParameterValueFromPayroll("TIMEGAP", ledgerCompanyCode, ledgerDBName)); //LogUploadingFunc.GetTimeGap(ledgerDBName);
                    MultiPocketVar.POCKETTIME = Convert.ToDouble((new Posting.BLogic.CommonBL()).GetParameterValueFromPayroll("POCKETTIME", ledgerCompanyCode, ledgerDBName)); //LogUploadingFunc.GetTimeGap(ledgerDBName);
                    MultiPocketVar.POCKETGAP = Convert.ToDouble((new Posting.BLogic.CommonBL()).GetParameterValueFromPayroll("POCKETGAP", ledgerCompanyCode, ledgerDBName)); //LogUploadingFunc.GetTimeGap(ledgerDBName);
                }
                catch (Exception e)
                {
                    _log.WriteLog(Application.StartupPath, "Posting", "UploadTheLogs : Checking Parameters", e.ToString(), true);
                }
                if (MultiPocketVar.isOTAfterMidnight)
                    LogUploadingFunc.OTAfterMindightCapturing(_processDate, ledgerDBName);

                //Start ShiftCode Manipulation
                LogUploadingFunc.FlexShift(_processDate, ledgerDBName, ledgerCompanyCode);
                LogUploadingFunc.ManipulateShiftCode(_processDate, ledgerDBName, ledgerCompanyCode);
                LogUploadingFunc.UpdateEmployeeAdvanceOTApp(_processDate, ledgerDBName);

            }
            catch (Exception e)
            {
                _log.WriteLog(Application.StartupPath, "Posting", "UploadTheLogs : LedgerDBNameLoop2ndRun", e.ToString(), true);
            }
    
        }

        #endregion

        public void UploadMutiPockets(DateTime processDate, DateTime UntilDate, string EmploeeID)
        {
            #region Version Updating
            //try
            //{
            //string AppVersion = System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();
            //Posting.BLogic.CommonBL commonBL = new Posting.BLogic.CommonBL();
            //commonBL.UpdateVersion("HRCLOGPOSTING", AppVersion, "LOGUPLDSRVC");
            //}
            //catch (Exception err)
            //{
            //_log.WriteLog(Application.StartupPath, "Posting", "Posting Application Version", err.Message, true);
            //}
            #endregion

            _log.WriteLog(Application.StartupPath, "Posting", "Posting Service Started", "", true);

            
            _processDate = Convert.ToDateTime(processDate.ToShortDateString());
            String HHText = DateTime.Now.ToString("HH");
            bool OverloadRecoveryPost = false;

            while (_processDate <= Convert.ToDateTime(UntilDate.ToShortDateString()))
            {
                UploadMutiPackets(EmploeeID);
                #region Recoveries on 1 am posting overload and posting via text file and text file to dtr
                if (Convert.ToInt16(HHText) <= 01 && MultiPocketVar.isServicePost)
                {
                    OverloadRecoveryPost = true;
                }
                #endregion
                _processDate = _processDate.AddDays(1);
            }

            //Get ledger date min date - 1
            _processDate = LedgerMinDate();
            #region Overload Reposting for HOGP3

            if (OverloadRecoveryPost && MultiPocketVar.isServicePost)
            {
                _log.WriteLog(Application.StartupPath, "Posting", "Repost Overloading Started", DateTime.UtcNow.ToLongDateString(), true);
                //FOR HOGP REPOSTING ON PREVIOUS DAYS WITH LEDGER ALL POCKETS HAS DATA

                while (_processDate < Convert.ToDateTime(UntilDate.ToShortDateString()))
                {
                    //no overwritting upon retrieval
                    MultiPocketVar.isOverwrite = false;
                    //Over Load Posting
                    MultiPocketVar.isOverLoadingRetrieval = true;
                    UploadMutiPackets(EmploeeID);
                    _processDate = _processDate.AddDays(1);
                }
                _log.WriteLog(Application.StartupPath, "Posting", "Repost Overloading Committed", DateTime.UtcNow.ToLongDateString(), true);
            }

            #endregion
        }

        private void NewLogPostingFunctions(String ledgerDBName)
        {
            Posting.BLogic.LogPostingBL LogPostingFunctions = new Posting.BLogic.LogPostingBL(CommonProcedures.GetAppSettingConfigString("DBNameDTR", string.Empty));
            using (DALHelper dal = new DALHelper(true))
            {
                try
                {
                    dal.OpenDB();

                    #region Log trail recovery function

                    //LogPostingFunctions.RepostLogTrail(ledgerDBName, _processDate, dal, "AND (Ttr_ActIn_1 <> '9999')"); //Repost all edited from log trail

                    LogPostingFunctions.LogpostingCounterMeasureFunction(ledgerDBName, dal);  //Start updating ledger based on accumulated dataset

                    #endregion
                }
                catch (Exception err)
                {
                    _log.WriteLog(Application.StartupPath, "Posting", "NewLogPostingFunctions\n" + err.Message, DateTime.UtcNow.ToLongDateString(), true);
                }
                finally
                {
                    dal.CloseDB();
                    dal.Dispose();
                }
            }
        }

        public DateTime LedgerMinDate()
        {
            DateTime RetDate = DateTime.UtcNow;
            try
            {
                string query = @"SELECT MIN(DATEADD(DD,+1,Ttr_Date)) LedgerMinDate FROM .dbo.T_EmpTimeRegister";
                DALHelper dal = new DALHelper("");
                dal.OpenDB();
                RetDate = Convert.ToDateTime(dal.ExecuteDataSet(query).Tables[0].Rows[0]["LedgerMinDate"]);
                dal.CloseDB();

            }
            catch (Exception ex) { }
            return RetDate;
        }


        public void UploadMutiPockets(string[] args)
        {
            if (args.Length == 2)
            {
                try
                {
                    _log.WriteLog(Application.StartupPath, "Posting", "On date posting", "", true);
                    MultiPocketVar.isServicePost = true; //will excecute text file and previous day posting
                    //UploadTheLogs(Convert.ToDateTime("11/18/2011"), Convert.ToDateTime("11/18/2011"));
                    UploadMutiPockets(Convert.ToDateTime(args[1]), Convert.ToDateTime(args[1]), "");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error in Log Uploading: " + e.Message);
                }
            }
            else if (args.Length == 3)
            {
                try
                {
                    _log.WriteLog(Application.StartupPath, "Posting", "Range date posting", "", true);
                    UploadMutiPockets(Convert.ToDateTime(args[1]), Convert.ToDateTime(args[2]), "");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error in Log Uploading: " + e.Message);
                }
            }
            else if (args.Length == 4)
            {
                try
                {
                    if (Convert.ToByte(args[3]) == 0)
                        MultiPocketVar.isOverwrite = false;
                    _log.WriteLog(Application.StartupPath, "Posting", "Range date no overwrite posting", "", true);
                    UploadMutiPockets(Convert.ToDateTime(args[1]), Convert.ToDateTime(args[2]), "");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error in Log Uploading: " + e.Message);
                }
            }
            else if (args.Length == 5)
            {
                try
                {
                    if (Convert.ToByte(args[3]) == 0)
                        MultiPocketVar.isOverwrite = false;
                    _log.WriteLog(Application.StartupPath, "Posting", "Range date no overwrite posting", "", true);
                    UploadMutiPockets(Convert.ToDateTime(args[1]), Convert.ToDateTime(args[2]), args[4]);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error in Log Uploading: " + e.Message);
                }
            }
            else
            {
                MultiPocketVar.isServicePost = true; //will excecute text file and previous day posting
                //Input date here for debugging specificdate
                //UploadTheLogs(Convert.ToDateTime("11/18/2011"), Convert.ToDateTime("11/18/2011"));
                _log.WriteLog(Application.StartupPath, "Posting", "Service on date posting", "", true);
                UploadMutiPockets(DateTime.Now, DateTime.Now, "");
            }
        }

    }

    #region Additional Log Handling Functions

    public class MultiPacketUploding
    {
        DALHelper dtrDB;
        string _dtrDBName = string.Empty;
        NLLogger.Logger _log = new NLLogger.Logger();

        //Note: value of pocket gap is static revise to get the value from database
        public double pocketGAP = 15.0; //pocket time gap--static value 15 mins
        public MultiPacketUploding(string dtrDBName)
        {
            dtrDB = new DALHelper(dtrDBName, false);
            _dtrDBName = dtrDBName;
        }

        #region Uploading V2 Objects

        public struct TimeEntryPrev
        {
            public Int16 TimeIn;
            public Int16 TimeOut;
        }
        public struct FIRSTLASTINOUT
        {
            public String IN;
            public String Out;
        }
        public class EmployeeLedgerPrev
        {
            public string EmployeeID;
            public DateTime ProcessDate;
            public string PayPeriod;
            public string DayCode;
            public string ShiftCode;
            public bool IsHoliday;
            public bool RestDay;
            public TimeEntryPrev ShiftTime = new TimeEntryPrev();
            public TimeEntryPrev ShiftBreak = new TimeEntryPrev();
            public TimeEntryPrev LogTime1 = new TimeEntryPrev();
            public TimeEntryPrev LogTime2 = new TimeEntryPrev();
        }
        public class EmployeeInAfterOutDTR
        {
            public string EmployeeID;
            public DateTime Logdate;
            public Int16 Logtime;
            public Char Logtype;
        }
        public class EmployeeCapturedOut
        {
            public string EmployeeID;
            public DateTime Logdate;
            public Int16 Logtime;
            public Char Logtype;
        }

        #endregion

        #region Uploading V2 Methods Log Validations

        public string FormatCapturedOutOccurence(DateTime CapturedOUTOccurence)
        {
            string CapturedOUT;
            CapturedOUT = (Convert.ToInt16(CapturedOUTOccurence.ToString("HH"))).ToString("00") + (Convert.ToInt16(CapturedOUTOccurence.ToString("mm"))).ToString("00");
            return CapturedOUT;
        }
        public bool _isTimeGapValid(DateTime Log2, DateTime Log1)
        {
            bool valid = true;
            try
            {
                TimeSpan Span = Log2.Subtract(Log1);
                Double CapturedGap = Convert.ToDouble(Span.TotalMinutes);
                if (MultiPocketVar.ValidTimeGap > CapturedGap
                   && Log1 != DateTime.MinValue)
                    valid = false;
            }
            catch (Exception e)
            {
                _log.WriteLog(Application.StartupPath, "Posting", "_isTimeGapValid : General", e.ToString(), true);
            }
            return valid;
        }
        public bool _isTimeGapValidOutOccurence(DateTime Log2, DateTime Log1)
        {
            bool valid = true;
            try
            {
                TimeSpan Span = Log2.Subtract(Log1);
                Double CapturedGap = Convert.ToDouble(Span.TotalMinutes);
                if (MultiPocketVar.ValidTimeGap > CapturedGap
                   && Log2 != DateTime.MinValue)
                    valid = false;
            }
            catch (Exception e)
            {
                _log.WriteLog(Application.StartupPath, "Posting", "CapturedGap : General", e.ToString(), true);
            }
            return valid;
        }
        //Check for consecutive graveyard shift
        public bool _isConsecutiveGY(DateTime dtrShiftTimeIN, DateTime LedgerShiftTimeOUT)
        {
            bool Consecutive = false;
            try
            {
                if (dtrShiftTimeIN.AddDays(1).ToShortDateString() != LedgerShiftTimeOUT.ToShortDateString())
                    Consecutive = true;
            }
            catch (Exception e)
            {
                _log.WriteLog(Application.StartupPath, "Posting", "CapturedGap : General", e.ToString(), true);
            }
            return Consecutive;
        }
        public EmployeeLedger ErrLogRegShiftCleaner(EmployeeLedger LedgerCleaner, DateTime ReferenceDate, DateTime RegShiftOUT)
        {
            Int16 IN1 = Convert.ToInt16(LedgerCleaner.LogTime1.TimeIn.ToString("HHmm"));
            Int16 OUT1 = Convert.ToInt16(LedgerCleaner.LogTime1.TimeOut.ToString("HHmm"));
            Int16 IN2 = Convert.ToInt16(LedgerCleaner.LogTime2.TimeIn.ToString("HHmm"));
            Int16 OUT2 = Convert.ToInt16(LedgerCleaner.LogTime2.TimeOut.ToString("HHmm"));
            try
            {
                //Same In Out Cleaner / Time Gap In Out Cleaner

                if ((IN1 >= OUT1
                   || LedgerCleaner.LogTime1.TimeIn > LedgerCleaner.LogTime1.TimeOut.AddMinutes(-MultiPocketVar.ValidTimeGap)
                   || ReferenceDate.ToShortDateString() != LedgerCleaner.LogTime1.TimeOut.ToShortDateString())
                   && LedgerCleaner.LogTime1.TimeIn != DateTime.MinValue
                   && LedgerCleaner.LogTime1.TimeOut != DateTime.MinValue)
                {
                    LedgerCleaner.LogTime1.TimeOut = DateTime.MinValue;
                }

                if ((IN2 >= OUT2
                   || LedgerCleaner.LogTime2.TimeIn > LedgerCleaner.LogTime2.TimeOut.AddMinutes(-MultiPocketVar.ValidTimeGap)
                   || ReferenceDate.ToShortDateString() != LedgerCleaner.LogTime2.TimeOut.ToShortDateString())
                   && LedgerCleaner.LogTime2.TimeIn != DateTime.MinValue
                   && LedgerCleaner.LogTime2.TimeOut != DateTime.MinValue)
                {
                    if (!MultiPocketVar.isFirstINLastOut
                        && LedgerCleaner.LogTime1.TimeIn.Hour > 12
                        && LedgerCleaner.LogTime2.TimeOut.Hour < 12
                        && LedgerCleaner.LogTime1.TimeOut == DateTime.MinValue
                        && LedgerCleaner.LogTime2.TimeIn == DateTime.MinValue)
                    {
                        //Do not remove Out 2 since it is reposted with intellegent inverse post
                    }
                    else
                    {
                        LedgerCleaner.LogTime2.TimeOut = DateTime.MinValue;
                    }
                }

                if ((IN1 >= OUT2
                   || LedgerCleaner.LogTime1.TimeIn > LedgerCleaner.LogTime2.TimeOut.AddMinutes(-MultiPocketVar.ValidTimeGap)
                   || ReferenceDate.ToShortDateString() != LedgerCleaner.LogTime2.TimeOut.ToShortDateString())
                   && LedgerCleaner.LogTime1.TimeIn != DateTime.MinValue
                   && LedgerCleaner.LogTime2.TimeOut != DateTime.MinValue)
                {
                    if (!MultiPocketVar.isFirstINLastOut
                        && LedgerCleaner.LogTime1.TimeIn.Hour > 12
                        && LedgerCleaner.LogTime2.TimeOut.Hour < 12
                        && LedgerCleaner.LogTime1.TimeOut == DateTime.MinValue
                        && LedgerCleaner.LogTime2.TimeIn == DateTime.MinValue)
                    { 
                        //Do not remove Out 2 since it is reposted with intellegent inverse post
                    }
                    else
                    {
                        LedgerCleaner.LogTime2.TimeOut = DateTime.MinValue;
                    }
                }

                if (ReferenceDate.ToShortDateString() != LedgerCleaner.LogTime2.TimeOut.ToShortDateString()
                   && LedgerCleaner.LogTime2.TimeOut != DateTime.MinValue)
                {
                    if (!MultiPocketVar.isFirstINLastOut
                        && LedgerCleaner.LogTime1.TimeIn.Hour > 12
                        && LedgerCleaner.LogTime2.TimeOut.Hour < 12
                        && LedgerCleaner.LogTime1.TimeOut == DateTime.MinValue
                        && LedgerCleaner.LogTime2.TimeIn == DateTime.MinValue)
                    {
                        //Do not remove Out 2 since it is reposted with intellegent inverse post
                    }
                    else
                    {
                        LedgerCleaner.LogTime2.TimeOut = DateTime.MinValue;
                    }
                }

            }
            catch (Exception e)
            {
                //_log.WriteLog(Application.StartupPath, "Posting", "ErrLogRegShiftCleaner : General", e.ToString(), true);
            }

            return LedgerCleaner;
        }
        public String PriorityIN()
        {
            FIRSTLASTINOUT Priority;
            if (MultiPocketVar.setFILO)
                Priority.IN = "ASC";
            else
                Priority.IN = "DESC";
            return Priority.IN;
        }
        public String PriorityOUT()
        {
            FIRSTLASTINOUT Priority;
            if (MultiPocketVar.setFILO)
                Priority.Out = "DESC";
            else
                Priority.Out = "ASC";
            return Priority.Out;
        }

        #endregion

        #region Methods used externally controlling logs
        public void setNightShiftHandlerLIFOINb4OUT(DataSet ds, String LedgerDBName)
        {
            DataTable dt = ds.Tables[0];
            DataSet dsLedgerExist = new DataSet();
            ParameterInfo[] paramInfo = new ParameterInfo[4];
            int affectedrow = 0;
            int LedgerSelect = 0;
            String _isLedger;
            String sql;
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    try
                    {
                        _isLedger = "T_EmpTimeRegister";
                        paramInfo[0] = new ParameterInfo("@ID", dt.Rows[i]["ID"].ToString(), SqlDbType.VarChar, 15);
                        paramInfo[1] = new ParameterInfo("@PRCDATE", dt.Rows[i]["PRCDATE"].ToString(), SqlDbType.DateTime);
                        paramInfo[2] = new ParameterInfo("@DTRIN1", dt.Rows[i]["DTRIN1"].ToString(), SqlDbType.VarChar, 4);
                        paramInfo[3] = new ParameterInfo("@DTROUT2", dt.Rows[i]["DTROUT2"].ToString(), SqlDbType.VarChar, 4);

                        sql = string.Format("SELECT COUNT(Ttr_IDNo) AS COUNT FROM {0}..T_EmpTimeRegister WHERE Ttr_IDNo=@ID AND Ttr_Date=@PRCDATE", LedgerDBName);

                        //dtrDB.BeginTransaction();
                        dsLedgerExist = dtrDB.ExecuteDataSet(sql, CommandType.Text, paramInfo);
                        LedgerSelect = Convert.ToInt16(dsLedgerExist.Tables[0].Rows[0]["COUNT"].ToString());
                        if (LedgerSelect <= 0)
                            _isLedger = "T_EmpTimeRegisterHst";
                        sql = string.Format(@"UPDATE {0}..{1}
                                                 SET Ttr_ActIn_1=@DTRIN1
                                                 ,Ttr_ActOut_2=@DTROUT2
                                                 ,Usr_Login='LOGUPLDSRVC'
                                                 ,Ludatetime=GETDATE()
                                                 WHERE Ttr_Date=@PRCDATE AND Ttr_IDNo=@ID",
                                                    LedgerDBName, _isLedger);
                        affectedrow = dtrDB.ExecuteNonQuery(sql, CommandType.Text, paramInfo);
                        //dtrDB.CommitTransaction();
                    }
                    catch
                    {
                        //dtrDB.RollBackTransaction();

                    }
                }
            }

        }
        public void OTAfterMindightCapturing(DateTime processdate, String LedgerDBName)
        {
            DataSet ds = new DataSet();
            try
            {
                String sqlOTAfterMid;

                if (!MultiPocketVar.isOTAfterMidnight)
                {
                    return;
                }
                DataSet dsOTAfterMidNight = new DataSet();
                ParameterInfo[] paramInfo = new ParameterInfo[3];
                paramInfo[0] = new ParameterInfo("@PREVDATE", processdate.AddDays(-1).ToShortDateString(), SqlDbType.DateTime);
                paramInfo[1] = new ParameterInfo("@CURRENTDATE", processdate.ToShortDateString(), SqlDbType.DateTime);
                paramInfo[2] = new ParameterInfo("@GAP", MultiPocketVar.ValidTimeGap, SqlDbType.Float);

                #region query
                sqlOTAfterMid = string.Format(@"SELECT 
	                                             OUTNXTD.Dtr_LogDate
	                                            ,OUTNXTD.Dtr_LogTime
	                                            ,CONVERT(DATETIME,Dtr_LogDate
			                                            +' '+
			                                            LEFT((SELECT TOP(1)INAFTER.Dtr_LogTIME FROM {1}..{2} INAFTER WHERE INAFTER.Dtr_LogType='I' 
			                                            AND Dtr_EmployeeID=Ttr_IDNo AND Dtr_LogDate=@CURRENTDATE ORDER BY Dtr_LogTime),2)
			                                            +':'+ 
			                                            RIGHT((SELECT TOP(1)INAFTER.Dtr_LogTIME FROM {1}..{2} INAFTER WHERE INAFTER.Dtr_LogType='I' 
			                                            AND Dtr_EmployeeID=Ttr_IDNo AND Dtr_LogDate=@CURRENTDATE ORDER BY Dtr_LogTime),2)
			                                            ) Dtr_DateTime
	                                            ,Msh_ShiftIn1 AS SHIFT
	                                            ,Ttr_IDNo AS EMP_ID
	                                            ,CONVERT(VARCHAR(20),Ttr_Date,101) AS PROCESS_DATE
	                                            ,Ttr_ActIn_1 AS IN1
	                                            ,Ttr_ActIn_2 AS OUT1
	                                            ,Ttr_ActOut_1 AS IN2
	                                            ,Ttr_ActOut_2 AS OUT2
                                                FROM 
		                                            (SELECT Ttr_IDNo
			                                            ,Ttr_Date
			                                            ,Ttr_ActIn_1
			                                            ,Ttr_ActIn_2
			                                            ,Ttr_ActOut_1
			                                            ,Ttr_ActOut_2
			                                            ,Ttr_ShiftCode
		                                            FROM {0}..T_EmployeeLogLedger
		                                            WHERE Ttr_Date BETWEEN @PREVDATE AND @CURRENTDATE
		                                            UNION 
		                                            select Ttr_IDNo
			                                            ,Ttr_Date
			                                            ,Ttr_ActIn_1
			                                            ,Ttr_ActIn_2
			                                            ,Ttr_ActOut_1
			                                            ,Ttr_ActOut_2
			                                            ,Ttr_ShiftCode
		                                            FROM {0}..T_EmployeeLogLedgerHist
		                                            WHERE Ttr_Date BETWEEN @PREVDATE AND @CURRENTDATE
		                                            ) LEDGERLIST
                                                    JOIN
                                                        {0}..T_ShiftCodeMaster SHIFT
                                                        ON Ttr_ShiftCode=Scm_ShiftCode
                                                    LEFT JOIN
                                                    {1}..{2} AS OUTNXTD
                                                    ON Dtr_EmployeeID=Ttr_IDNo
                                                    AND Dtr_LogDate=@CURRENTDATE
                                                    AND Dtr_LogType='O' 
                                                    --ORDER BY DTR_LOGTIME
                                                    AND CONVERT(INT,DATEDIFF(MI,CONVERT(DATETIME,Dtr_LogDate +' '+ LEFT(Dtr_LogTime,2)+':'+RIGHT(Dtr_LogTime,2)),
									                                            CASE WHEN 
										                                            CONVERT(INT,(SELECT TOP(1)INAFTER.Dtr_LogTIME FROM {1}..{2} INAFTER WHERE INAFTER.Dtr_LogType='I' 
										                                            AND Dtr_EmployeeID=Ttr_IDNo AND Dtr_LogDate=@CURRENTDATE ORDER BY Dtr_LogTime)) 
										                                            IS NULL THEN 
										                                            CONVERT(DATETIME,CONVERT(VARCHAR,@CURRENTDATE,101) +' '+ LEFT(SHIFT.Msh_ShiftIn1,2)+':'+RIGHT(SHIFT.Msh_ShiftIn1,2)) 
									                                            ELSE 
										                                            CONVERT(DATETIME,Dtr_LogDate
												                                            +' '+
												                                            LEFT((SELECT TOP(1)INAFTER.Dtr_LogTIME FROM {1}..{2} INAFTER WHERE INAFTER.Dtr_LogType='I' 
												                                            AND Dtr_EmployeeID=Ttr_IDNo AND Dtr_LogDate=@CURRENTDATE ORDER BY Dtr_LogTime),2)
												                                            +':'+ 
												                                            RIGHT((SELECT TOP(1)INAFTER.Dtr_LogTIME FROM {1}..{2} INAFTER WHERE INAFTER.Dtr_LogType='I' 
												                                            AND Dtr_EmployeeID=Ttr_IDNo AND Dtr_LogDate=@CURRENTDATE ORDER BY Dtr_LogTime),2)
												                                            )
									
									                                            END)
					                                            )> @GAP
                                                WHERE 
                                                (CONVERT(BIGINT,Ttr_ActIn_2+Ttr_ActIn_1)!=0)
                                                    AND 
                                                    (CONVERT(INT,Ttr_ActOut_2)=0 OR CONVERT(INT,Ttr_ActOut_2)<CONVERT(INT,Msh_ShiftIn1))
                                                AND
                                                Ttr_Date=@PREVDATE
                                                AND 
                                                CONVERT(INT,Dtr_LogTime)<CONVERT(INT,Msh_ShiftIn1)
                                                AND CONVERT(INT,Msh_ShiftIn1)<CONVERT(INT,Msh_ShiftOut2)
                                                AND Ttr_ActIn_1!='9999' --added for mutipocker"
                                                , LedgerDBName, _dtrDBName, MultiPocketVar.T_DTR);
                #endregion


                try
                {
                    dtrDB.OpenDB();
                    //dtrDB.BeginTransaction();
                    ds = dtrDB.ExecuteDataSet(sqlOTAfterMid, CommandType.Text, paramInfo);
                    //dtrDB.CommitTransaction();
                    //start updating of ledger
                    OTafternextDayUpdater(ds, LedgerDBName);
                    //plus 24 all OT
                    OTAfternextDayPlus24(processdate, LedgerDBName);
                }
                catch (Exception e)
                {
                    //dtrDB.RollBackTransaction();
                    _log.WriteLog(Application.StartupPath, "Posting", "OTAfterMindightCapturing : RollBack", e.ToString(), true);
                }
                finally
                {
                    dtrDB.CloseDB();
                }
            }
            catch (Exception e)
            {
                _log.WriteLog(Application.StartupPath, "Posting", "OTAfterMindightCapturing : General", e.ToString(), true);
            }
        }
        public void OTafternextDayUpdater(DataSet ds, String LedgerDBName)
        {
            DataTable dt = ds.Tables[0];
            DataSet dsLedgerExist = new DataSet();
            ParameterInfo[] paramInfo = new ParameterInfo[3];
            int affectedrow = 0;
            int LedgerSelect = 0;
            String _isLedger;
            String sql;
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    try
                    {
                        _isLedger = "T_EmployeeLogLedger";
                        paramInfo[0] = new ParameterInfo("@EMP_ID", dt.Rows[i]["EMP_ID"].ToString(), SqlDbType.VarChar, 15);
                        paramInfo[1] = new ParameterInfo("@PROCESS_DATE", dt.Rows[i]["PROCESS_DATE"].ToString(), SqlDbType.DateTime);
                        paramInfo[2] = new ParameterInfo("@DTR_LOGTIME", dt.Rows[i]["Dtr_LogTime"].ToString(), SqlDbType.VarChar, 4);

                        sql = string.Format("Select Count(Ttr_IDNo) AS COUNT FROM {0}..T_EMPLOYEELOGLEDGER WHERE Ttr_IDNo=@EMP_ID AND Ttr_Date=@PROCESS_DATE", LedgerDBName);

                        //dtrDB.BeginTransaction();
                        dsLedgerExist = dtrDB.ExecuteDataSet(sql, CommandType.Text, paramInfo);
                        LedgerSelect = Convert.ToInt16(dsLedgerExist.Tables[0].Rows[0]["COUNT"].ToString());
                        if (LedgerSelect <= 0)
                            _isLedger = "T_EmployeeLogLedgerHist";
                        sql = string.Format(@"Update {0}..{1}
                                                 SET Ttr_ActOut_2=CONVERT(INT,@DTR_LOGTIME)+2400
                                                 ,USR_LOGIN='LOGUPLDONXTD'
                                                 ,LUDATETIME=GETDATE()
                                                 WHERE Ttr_Date=@PROCESS_DATE AND Ttr_IDNo=@EMP_ID",
                                                    LedgerDBName, _isLedger);
                        affectedrow = dtrDB.ExecuteNonQuery(sql, CommandType.Text, paramInfo);
                        //dtrDB.CommitTransaction();
                    }
                    catch
                    {
                        //dtrDB.RollBackTransaction();
                    }
                }
            }

        }
        public void OTAfternextDayPlus24(DateTime processdate, String LedgerDBName)
        {
            //if (!MultiPocketVar.isReposting)
            //    return;
            int affectedrow = 0;
            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@ProcessDate", processdate.ToShortDateString(), SqlDbType.DateTime);
            string sql = string.Format(@"UPDATE {0}..{1}
                                        SET Ttr_ActOut_2=CONVERT(INT,Ttr_ActOut_2)+2400
	                                        ,Usr_Login='LOGUPLDONXTD'
                                        FROM 
                                        {0}..{1} LEDGER
                                        JOIN
                                        {0}..T_ShiftCodeMaster
                                        ON Ttr_ShiftCode=Scm_ShiftCode 
                                        WHERE CONVERT(INT,Ttr_ActOut_2)<CONVERT(INT,Msh_ShiftIn1)
                                        AND CONVERT(INT,Ttr_ActOut_2)!=0
                                        AND CONVERT(INT,Msh_ShiftIn1)<CONVERT(INT,Msh_ShiftOut2)
                                        AND Ttr_Date=@ProcessDate", LedgerDBName, MultiPocketVar.T_Ledger);
            try
            {
                dtrDB.OpenDB();
                //dtrDB.BeginTransaction();
                affectedrow = dtrDB.ExecuteNonQuery(sql, CommandType.Text, paramInfo);
                //dtrDB.CommitTransaction();
            }
            catch (Exception e)
            {
                //dtrDB.RollBackTransaction();
                dtrDB.CloseDB();
                _log.WriteLog(Application.StartupPath, "Posting", "OTAfternextDayPlus24 : General", e.ToString(), true);
            }
            finally
            {
                dtrDB.CloseDB();
            }
        }
        public bool TextFileDiffDTRUploading(String[] LogsData, DateTime processDate)
        {
            bool bRetVal = false;
            int nAffectedRows = 0;

            #region parameters

            ParameterInfo[] paramInsertDTR = new ParameterInfo[9];

            LogsData[2] = String.Format("{0:0000}", Convert.ToInt16(LogsData[2].Trim()));

            paramInsertDTR[0] = new ParameterInfo("@Dtr_EmployeeID", LogsData[0].Trim(), SqlDbType.VarChar, 15);
            paramInsertDTR[1] = new ParameterInfo("@Dtr_LogDate", Convert.ToDateTime(LogsData[1].Trim()).ToString("MM/dd/yyyy"), SqlDbType.VarChar, 10);
            paramInsertDTR[2] = new ParameterInfo("@Dtr_LogTime", LogsData[2].Trim(), SqlDbType.Char, 4);//LogsData[2].Trim(), SqlDbType.Char, 4);
            paramInsertDTR[3] = new ParameterInfo("@Dtr_LogType", LogsData[3].Trim(), SqlDbType.Char, 1);
            paramInsertDTR[4] = new ParameterInfo("@Dtr_StationNo", "TX", SqlDbType.Char, 2);
            paramInsertDTR[5] = new ParameterInfo("@Dtr_PostFlag", 0, SqlDbType.Bit);
            paramInsertDTR[6] = new ParameterInfo("@Dtr_UploadedFlag", 1, SqlDbType.Bit);
            paramInsertDTR[7] = new ParameterInfo("@Usr_Login", "LOGUPLDFTP", SqlDbType.VarChar, 15);
            paramInsertDTR[8] = new ParameterInfo("@LudateTime", processDate.ToLongDateString(), SqlDbType.DateTime);

            #endregion

            try
            {
                //dtrDB.BeginTransaction();
                nAffectedRows = dtrDB.ExecuteNonQuery("spLogReadingInsertToServerDTR_Diff", CommandType.StoredProcedure, paramInsertDTR);
                //dtrDB.CommitTransaction();
                bRetVal = true;

            }
            catch (Exception ex)
            {
                bRetVal = false;
                //dtrDB.RollBackTransaction();
            }

            return bRetVal;
        }
        public void T_DTRDifferentialCreator()
        {
            ParameterInfo[] paramInsertDTR = new ParameterInfo[1];
            paramInsertDTR[0] = new ParameterInfo("@NOTHING", "", SqlDbType.VarChar, 15);
            String sqlGetDiffSP = string.Format(@"USE [{0}]
                                                SELECT COUNT(NAME) AS DIFFSTORPROC FROM SYS.procedures
                                                WHERE name='spLogReadingInsertToServerDTR_Diff'", _dtrDBName);
            String sqlGetT_DTRDiff = string.Format(@"USE [{0}] SELECT COUNT(NAME) AS DTRDIFF FROM SYS.tables WHERE NAME='T_DTRDifferential' ", _dtrDBName);
            #region Create T_DTRDIfferential ...
            String sql = string.Format(@"USE [{0}]
                                        SET ANSI_NULLS ON
                                        SET QUOTED_IDENTIFIER ON
                                        SET ANSI_PADDING ON
                                        CREATE TABLE [dbo].[T_DTRDifferential](
	                                        [Dtr_EmployeeID] [varchar](15) NOT NULL,
	                                        [Dtr_LogDate] [varchar](10) NOT NULL,
	                                        [Dtr_LogTime] [char](4) NOT NULL,
	                                        [Dtr_LogType] [char](1) NOT NULL,
	                                        [Dtr_StationNo] [char](2) NOT NULL,
	                                        [Dtr_PostFlag] [bit] NOT NULL,
	                                        [Usr_Login] [varchar](15) NOT NULL,
	                                        [LudateTime] [datetime] NOT NULL,
	                                        [Dtr_UploadedFlag] [bit] NULL,
                                         CONSTRAINT [PK_T_DTRDifferential] PRIMARY KEY CLUSTERED 
                                        (
	                                        [Dtr_EmployeeID] ASC,
	                                        [Dtr_LogDate] ASC,
	                                        [Dtr_LogTime] ASC,
	                                        [Dtr_LogType] ASC
                                        )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
                                        ) ON [PRIMARY]
                                        SET ANSI_PADDING OFF", _dtrDBName);
            #endregion
            #region Create Differential SP ...
            String sqlCreateSP = string.Format(@"CREATE PROCEDURE [dbo].[spLogReadingInsertToServerDTR_Diff]            
                                                (            
                                                  @Dtr_EmployeeID VARCHAR(15)            
                                                 ,@Dtr_LogDate VARCHAR(10)            
                                                 ,@Dtr_LogTime CHAR(4)            
                                                 ,@Dtr_LogType CHAR(1)            
                                                 ,@Dtr_StationNo CHAR(2)            
                                                 ,@Dtr_PostFlag BIT            
                                                 ,@Dtr_UploadedFlag BIT            
                                                 ,@Usr_Login VARCHAR(15)            
                                                 ,@Ludatetime DATETIME            
                                                )            
                                                AS            
                                                IF NOT EXISTS             
                                                  (SELECT 1            
                                                     FROM T_DTRDifferential             
                                                    WHERE Dtr_EmployeeID = @Dtr_EmployeeID            
                                                      AND Dtr_LogDate = @Dtr_LogDate            
                                                      AND Dtr_LogTime = @Dtr_LogTime            
                                                      AND Dtr_LogType = @Dtr_LogType)            
                                                  BEGIN            
                                                   INSERT INTO T_DTRDifferential            
                                                                       (Dtr_EmployeeID            
                                                                       ,Dtr_LogDate            
                                                                       ,Dtr_LogTime            
                                                                       ,Dtr_LogType            
                                                                       ,Dtr_StationNo            
                                                                       ,Dtr_PostFlag            
                                                                       ,Dtr_UploadedFlag            
                                                                       ,Usr_Login            
                                                                       ,LudateTime            
                                                                       )            
                                                                VALUES (@Dtr_EmployeeID            
                                                                       ,@Dtr_LogDate            
                                                                       ,@Dtr_LogTime            
                                                                       ,@Dtr_LogType            
                                                                       ,@Dtr_StationNo            
                                                                       ,@Dtr_PostFlag            
                                                                       ,@Dtr_UploadedFlag            
                                                                       ,@Usr_Login            
                                                                       ,@LudateTime            
                                                                       )                                                                                         
                                                    IF (ISNULL((SELECT Lmt_LastSwipe             
                                                         FROM T_LogMaster            
                                                        WHERE Lmt_EmployeeID = @Dtr_EmployeeID), '1900-01-01')            
                                                     <= CAST(@Dtr_LogDate + ' ' + LEFT(@Dtr_LogTime, 2) + ':' + RIGHT(@Dtr_LogTime, 2) AS DATETIME))            
                                                    BEGIN            
                                                     UPDATE T_LogMaster            
                                                        SET Lmt_LastSwipe = CAST(@Dtr_LogDate + ' ' + LEFT(@Dtr_LogTime, 2) + ':' + RIGHT(@Dtr_LogTime, 2) AS DATETIME)            
                                                           ,Lmt_LastLogType = @Dtr_LogType            
                                                      WHERE Lmt_EmployeeID = @Dtr_EmployeeID            
                                                    END            
                                                  END", _dtrDBName);
            #endregion
            try
            {
                dtrDB.OpenDB();

                //Create Table
                //dtrDB.BeginTransaction();
                DataSet ds_existing = dtrDB.ExecuteDataSet(sqlGetT_DTRDiff);
                //dtrDB.CommitTransaction();
                DataTable T_EXISTING = ds_existing.Tables[0];
                int affected = Convert.ToInt16(T_EXISTING.Rows[0]["DTRDIFF"].ToString());
                if (affected == 0)
                    try
                    {
                        //dtrDB.BeginTransaction();
                        dtrDB.ExecuteNonQuery(sql, CommandType.Text, paramInsertDTR);
                        //dtrDB.CommitTransaction();
                    }
                    catch (Exception e)
                    {
                        //dtrDB.RollBackTransaction();
                        _log.WriteLog(Application.StartupPath, "Posting", "T_DTRDifferentialCreator : Create Diff Table", e.ToString(), true);
                    }



                //Create SP
                //dtrDB.BeginTransaction();
                ds_existing = dtrDB.ExecuteDataSet(sqlGetDiffSP);
                //dtrDB.CommitTransaction();
                T_EXISTING = ds_existing.Tables[0];
                affected = Convert.ToInt16(T_EXISTING.Rows[0]["DIFFSTORPROC"].ToString());
                dtrDB.CloseDB();
                DALHelper dal = new DALHelper();
                using (SqlConnection connection = dal.getConnectionDTR())
                {
                    SqlCommand command = new SqlCommand(sqlCreateSP, connection);
                    command.CommandType = CommandType.Text;
                    connection.Open();

                    if (affected == 0)
                        try
                        {
                            command.ExecuteScalar();
                        }
                        catch (Exception e)
                        {
                            _log.WriteLog(Application.StartupPath, "Posting", "T_DTRDifferentialCreator : Create Diff STored Proc", e.ToString(), true);
                        }
                    connection.Close();
                }

            }
            catch (Exception e)
            {

                //dtrDB.RollBackTransaction();
                dtrDB.CloseDB();
                _log.WriteLog(Application.StartupPath, "Posting", "T_DTRDifferentialCreator : General", e.ToString(), true);
            }

        }
        public DataSet MINMAXPosting(DateTime Processdate, String LedgerDBName, String LedgerTable)
        {
            DataSet ds = new DataSet();
            try
            {
                {
                    ParameterInfo[] paramInfo = new ParameterInfo[1];
                    paramInfo[0] = new ParameterInfo("@ProcessDate", Processdate.ToShortDateString(), SqlDbType.DateTime);

                    #region MinMax Query
                    string sql = string.Format(@"SELECT 
	                                                DISTINCT DTR_EMPLOYEEID AS EMPLOYEEID 
	                                                --IN 1
	                                                ,(SELECT MIN(DTR_LOGTIME) FROM {0}..T_DTR
	                                                    WHERE CONVERT(DATETIME,DTR_LOGDATE,101)=@Processdate
	                                                    AND DTR_LOGTYPE='I'
	                                                    AND DTR_EMPLOYEEID=DTR.DTR_EMPLOYEEID
	                                                    AND CONVERT(INT,DTR_LOGTIME)<CONVERT(INT,SCM.Scm_ShiftBreakStart)
	                                                    AND DTR_POSTFLAG=0) AS IN1
	                                                --OUT1
	                                                ,(SELECT MAX(DTR_LOGTIME) FROM {0}..T_DTR
	                                                    WHERE CONVERT(DATETIME,DTR_LOGDATE,101)=@Processdate
	                                                    AND DTR_LOGTYPE='O'
	                                                    AND DTR_EMPLOYEEID=DTR.DTR_EMPLOYEEID
	                                                    AND CONVERT(INT,DTR_LOGTIME)<=CONVERT(INT,SCM.Scm_ShiftBreakStart)
	                                                    AND DTR_POSTFLAG=0) AS OUT1
	                                                --IN2
	                                                ,(SELECT MIN(DTR_LOGTIME) FROM {0}..T_DTR
	                                                    WHERE CONVERT(DATETIME,DTR_LOGDATE,101)=@Processdate
	                                                    AND DTR_LOGTYPE='I'
	                                                    AND DTR_EMPLOYEEID=DTR.DTR_EMPLOYEEID
	                                                    AND CONVERT(INT,DTR_LOGTIME)>=CONVERT(INT,SCM.Scm_ShiftBreakStart)
	                                                    AND DTR_POSTFLAG=0) AS IN2
	                                                --OUT2
	                                                ,(SELECT MAX(DTR_LOGTIME) FROM {0}..T_DTR
	                                                    WHERE CONVERT(DATETIME,DTR_LOGDATE,101)=@Processdate
	                                                    AND DTR_LOGTYPE='O'
	                                                    AND DTR_EMPLOYEEID=DTR.DTR_EMPLOYEEID
	                                                    AND CONVERT(INT,DTR_LOGTIME)>CONVERT(INT,SCM.Scm_ShiftBreakStart)
	                                                    AND DTR_POSTFLAG=0) AS OUT2
	                                                    --,Ttr_IDNo AS EMPLOYEEID
	                                                    ,CONVERT(VARCHAR(20),Ttr_Date,101) AS PROCESSDATE
	                                                    ,Ttr_ShiftCode AS SHIFT
	                                                    ,Msh_ShiftIn1 AS SHFTIN
	                                                    ,Scm_ShiftBreakStart AS BRKIN
	                                                    ,Scm_ShiftBreakEnd AS BRKOUT
	                                                    ,Msh_ShiftOut2 AS SHFTOUT
	                                                    ,[Ttr_ActIn_1]AS LEDGERIN1
                                                        ,[Ttr_ActOut_1] AS LEDGEROUT1
                                                        ,[Ttr_ActIn_2] AS LEDGERIN2
                                                        ,[Ttr_ActOut_2] AS LEDGEROUT2
                                                FROM {0}..T_DTR DTR
	                                                LEFT JOIN
	                                                {1}..{2}
                                                ON Ttr_IDNo=DTR_EMPLOYEEID
		                                                LEFT JOIN {1}..T_SHIFTCODEMASTER SCM
		                                                ON Ttr_ShiftCode=SCM_SHIFTCODE
                                                WHERE CONVERT(DATETIME,DTR_LOGDATE,101)=@Processdate
                                                AND Ttr_Date=@Processdate", _dtrDBName, LedgerDBName, LedgerTable);
                    #endregion

                    try
                    {
                        dtrDB.OpenDB();
                        //dtrDB.BeginTransaction();
                        ds = dtrDB.ExecuteDataSet(sql, CommandType.Text, paramInfo);
                        //dtrDB.CommitTransaction();
                    }
                    catch (Exception e)
                    {
                        _log.WriteLog(Application.StartupPath, "Posting", "MINMAXPosting : RollBack", e.ToString(), true);
                        //dtrDB.RollBackTransaction();
                    }
                    finally
                    {
                        dtrDB.CloseDB();
                    }
                }
            }
            catch (Exception e)
            {
                _log.WriteLog(Application.StartupPath, "Posting", "MINMAXPosting : General", e.ToString(), true);
            }
            return ds;
        }
        private string checkifnull(string LOG)
        {
            if (LOG == "")
                LOG = "0000";
            return LOG;
        }
        private string FILO(String DTR, String LEDGER, bool IO)
        {
            if (IO)
            {
                if ((Convert.ToInt16(DTR) < Convert.ToInt16(LEDGER) && DTR != "0000") || LEDGER == "0000")
                    return DTR;
                else
                    return LEDGER;
            }
            else
            {
                if ((Convert.ToInt16(DTR) > Convert.ToInt16(LEDGER) && DTR != "0000") || LEDGER == "0000")
                    return DTR;
                else
                    return LEDGER;
            }
        }

        #region NOT IN USE
        public DataSet getUnpostedAfterPosting(DateTime Processdate, String LedgerDBName)
        {
            DataSet dsUnposted = new DataSet();
            try
            {
                ParameterInfo[] paramInfo = new ParameterInfo[1];
                paramInfo[0] = new ParameterInfo("@ProcessDate", Processdate.ToShortDateString(), SqlDbType.DateTime);
                String sql = string.Format(@"SELECT 
	                                          -- IN 1
	                                          (SELECT TOP 1 DTR_LOGTIME FROM {0}..{3} 
			                                        WHERE 
			                                        Dtr_EmployeeID=LEDGER.Ttr_IDNo 
			                                        AND CONVERT(DATETIME,Dtr_LogDate)=LEDGER.Ttr_Date 
				                                        AND CONVERT(INT,Dtr_LogTime) 
				                                        < CONVERT(INT,SHIFT.Scm_ShiftBreakStart)
			                                        AND Dtr_LogType='I' ORDER BY Dtr_LogTime {4}) AS DTR_IN1
	                                          --OUT 1
	                                          ,(SELECT TOP 1 DTR_LOGTIME FROM {0}..{3} 
			                                        WHERE 
			                                        Dtr_EmployeeID=LEDGER.Ttr_IDNo 
			                                        AND CONVERT(DATETIME,Dtr_LogDate)=LEDGER.Ttr_Date 
				                                        AND CONVERT(INT,Dtr_LogTime) 
				                                        <= CONVERT(INT,SHIFT.Scm_ShiftBreakEnd)
			                                        AND Dtr_LogType='O'
			                                        ORDER BY Dtr_LogTime {5})  AS DTR_OUT1
	                                          --IN 2
		                                        ,(SELECT TOP 1 DTR_LOGTIME FROM {0}..{3} 
			                                        WHERE 
			                                        Dtr_EmployeeID=LEDGER.Ttr_IDNo 
			                                        AND CONVERT(DATETIME,Dtr_LogDate)=LEDGER.Ttr_Date 
				                                        AND CONVERT(INT,Dtr_LogTime) 
				                                        >= CONVERT(INT,SHIFT.Scm_ShiftBreakStart)
			                                        AND Dtr_LogType='I' ORDER BY Dtr_LogTime {4})  AS DTR_IN2
	                                          --OUT 2
	                                          ,(SELECT TOP 1 DTR_LOGTIME FROM {0}..{3} 
			                                        WHERE 
			                                        Dtr_EmployeeID=LEDGER.Ttr_IDNo 
			                                        AND CONVERT(DATETIME,Dtr_LogDate)=LEDGER.Ttr_Date 
				                                        AND CONVERT(INT,Dtr_LogTime) 
				                                        >= CONVERT(INT,SHIFT.Scm_ShiftBreakEnd)
			                                        AND Dtr_LogType='O'
			                                        ORDER BY Dtr_LogTime {5}) AS  DTR_OUT2
	                                          ,[Ttr_IDNo] AS EMPLOYEE_ID
                                              ,[Ttr_Date] AS PRCDATE
                                              ,[Ttr_ActIn_1] AS LEDGER_IN1
                                              ,[Ttr_ActOut_1]AS LEDGER_OUT1
                                              ,[Ttr_ActIn_2] AS LEDGER_IN2
                                              ,[Ttr_ActOut_2] AS lEDGER_OUT2
                                          FROM {1}..[{2}] LEDGER
	                                        LEFT JOIN 
	                                        {1}..T_ShiftCodeMaster SHIFT
	                                        ON Scm_ShiftCode=Ttr_ShiftCode
                                        WHERE Ttr_Date = @ProcessDate
                                        AND
                                        CONVERT(INT,Msh_ShiftIn1)<CONVERT(INT,Msh_ShiftOut2)
                                        AND 
                                        (CONVERT(BIGINT,Ttr_ActIn_1+Ttr_ActIn_2+Ttr_ActOut_1+Ttr_ActOut_2)=0
                                        OR
	                                        (CONVERT(BIGINT,Ttr_ActIn_1+Ttr_ActIn_2+Ttr_ActOut_1)=0
	                                        AND
	                                        CONVERT(INT,Ttr_ActOut_2)>2400)
                                        )
                                        ORDER BY Ttr_Date,Ttr_IDNo",
                                           _dtrDBName, LedgerDBName, MultiPocketVar.T_Ledger, MultiPocketVar.T_DTR, PriorityIN(), PriorityOUT());
                try
                {
                    dtrDB.OpenDB();
                    //dtrDB.BeginTransaction();
                    dsUnposted = dtrDB.ExecuteDataSet(sql, CommandType.Text, paramInfo);
                    //dtrDB.CommitTransaction();
                }
                catch (Exception e)
                {
                    //dtrDB.RollBackTransaction();
                    _log.WriteLog(Application.StartupPath, "Posting", "getUnpostedAfterPosting : Rollback", e.ToString(), true);
                    dtrDB.CloseDB();
                }
                finally
                {
                    dtrDB.CloseDB();
                }

            }
            catch (Exception e)
            {
                _log.WriteLog(Application.StartupPath, "Posting", "getUnpostedAfterPosting : General", e.ToString(), true);
            }
            return dsUnposted;
        }
        public void UploadReadDiffTextFileLogs(String FileFolder, DateTime processDate)
        {
            try
            {
                try
                {
                    //Create Differential Table and Stored Procedure
                    T_DTRDifferentialCreator();

                    //DELETING DIFFERENTIAL DTR DATE -1
                    int Deleted = 0;
                    dtrDB.OpenDB();
                    //dtrDB.BeginTransaction();
                    ParameterInfo[] paramInfo = new ParameterInfo[1];
                    paramInfo[0] = new ParameterInfo("@PREVDATE", processDate.AddDays(-2).ToShortDateString(), SqlDbType.DateTime);
                    String delsql = string.Format("DELETE {0}..T_DTRDifferential WHERE CONVERT(DATETIME,DTR_LOGDATE,101)<=@PREVDATE", _dtrDBName);
                    Deleted = dtrDB.ExecuteNonQuery(delsql, CommandType.Text, paramInfo);
                    //dtrDB.CommitTransaction();
                    dtrDB.CloseDB();

                }
                catch (Exception ex)
                {
                    //dtrDB.RollBackTransaction();
                    dtrDB.CloseDB();
                    _log.WriteLog(Application.StartupPath, "Posting", "UplaodReadDiffTextFileLogs : Delete Diff", "", true);
                }

                _log.WriteLog(Application.StartupPath, "Posting", "Reposting Previous Day TextFile Started", "", true);

                String LogsLine = "";
                String[] LogsData;
                System.IO.StreamReader objReader;
                string[] FTPFiles = Directory.GetFiles(FileFolder);

                foreach (string _FTPFiles in FTPFiles)
                {
                    try
                    {
                        dtrDB.OpenDB();
                        objReader = new System.IO.StreamReader(_FTPFiles);

                        String FTPTxtDate = _FTPFiles.Replace(FileFolder, "");
                        FTPTxtDate = FTPTxtDate.Replace(@"\", "");
                        //parse the .txt filename by yearmonthdate (text file must be yyyymmdd--any-other-character.text
                        FTPTxtDate = (String.Format("{0}-{1}-{2}", FTPTxtDate.Substring(4, 2)
                                                                 , FTPTxtDate.Substring(6, 2)
                                                                 , FTPTxtDate.Substring(0, 4)));
                        if (Convert.ToDateTime(FTPTxtDate).ToShortDateString() == processDate.ToShortDateString()
                            || Convert.ToDateTime(FTPTxtDate).ToShortDateString() == processDate.AddDays(-1).ToShortDateString())
                        {

                            while (objReader.Peek() != -1)
                            {

                                LogsLine = objReader.ReadLine();
                                LogsData = LogsLine.Split(',');
                                if (LogsLine.Trim() != String.Empty)
                                {
                                    try
                                    {
                                        TextFileDiffDTRUploading(LogsData, processDate);
                                    }
                                    catch { }
                                }

                            }
                            objReader.Close();
                            dtrDB.CloseDB();
                        }
                    }
                    catch (Exception e)
                    {
                        dtrDB.CloseDB();
                        _log.WriteLog(Application.StartupPath, "Posting", "UplaodReadDiffTextFileLogs : GetFileLoop", e.ToString(), true);
                    }
                }
            }
            catch (Exception e)
            {
                _log.WriteLog(Application.StartupPath, "Posting", "UplaodReadDiffTextFileLogs : General", e.ToString(), true);
            }
        }
        public void setPostFlagToZero(DateTime Processdate) //hard coded for hoya
        {
            try
            {
                String HHText = DateTime.Now.ToString("HH"); //hardcoded befor or on 22 am
                if ((Convert.ToInt16(HHText) == 22)
                    && MultiPocketVar.isServicePost
                    && !File.Exists(Application.StartupPath + @"\\Logged\~0.log"))
                {
                    int Affected = 0;
                    ParameterInfo[] paramInfo = new ParameterInfo[1];
                    paramInfo[0] = new ParameterInfo("@ProcessDate", Processdate.ToShortDateString(), SqlDbType.DateTime);
                    String sql = string.Format(@"UPDATE {0}..T_DTR
                                                      SET Dtr_PostFlag='0'
	                                                    WHERE CONVERT(Datetime,Dtr_LogDate,101) = @ProcessDate", _dtrDBName);

                    try
                    {
                        dtrDB.OpenDB();
                        //dtrDB.BeginTransaction();
                        Affected = dtrDB.ExecuteNonQuery(sql, CommandType.Text, paramInfo);
                        //dtrDB.CommitTransaction();
                        _log.WriteLog(Application.StartupPath, "Posting", "setPostFlagToZero : Committed", "", true);
                    }
                    catch (Exception e)
                    {
                        _log.WriteLog(Application.StartupPath, "Posting", "setPostFlagToZero : RollBack", e.ToString(), true);
                        //dtrDB.RollBackTransaction();
                    }
                    finally
                    {
                        dtrDB.CloseDB();
                    }
                }
                HHText = DateTime.Now.ToString("HH"); //hardcoded befor or on 08 am
                if ((Convert.ToInt16(HHText) == 22)
                    && MultiPocketVar.isServicePost)
                    _log.WriteLog(Application.StartupPath, "0", "Set post to zero", "", true);

                if ((Convert.ToInt16(HHText) != 22)
                    && MultiPocketVar.isServicePost)
                    if (File.Exists(Application.StartupPath + @"\\Logged\~0.log"))
                        File.Delete(Application.StartupPath + @"\\Logged\~0.log");

            }
            catch (Exception e)
            {
                _log.WriteLog(Application.StartupPath, "Posting", "setPostFlagToZero : General", e.ToString(), true);
            }

        }
        public void RecoverGraveyardOut(DateTime Processdate, String LedgerDBName)
        {

            try
            {
                {
                    if (MultiPocketVar.setFILO)
                        return;
                    int Affected = 0;
                    ParameterInfo[] paramInfo = new ParameterInfo[1];
                    paramInfo[0] = new ParameterInfo("@ProcessDate", Processdate.ToShortDateString(), SqlDbType.DateTime);
                    #region query
                    String sql = string.Format(@"UPDATE {1}..T_EmployeeLogLedger 
                                                SET Ttr_ActOut_2=
	                                                (CASE WHEN(SELECT TOP 1 DTR_LOGTIME FROM {0}..T_DTR 
			                                                WHERE 
			                                                Dtr_EmployeeID=LEDGER.Ttr_IDNo 
			                                                AND CONVERT(DATETIME,Dtr_LogDate)=DATEADD(DD,1,LEDGER.Ttr_Date)
				                                                AND CONVERT(INT,Dtr_LogTime) 
				                                                >=(CONVERT(INT,SHIFT.Scm_ShiftBreakEnd)-2400)
			                                                AND Dtr_LogType='O'
			                                                ORDER BY Dtr_LogTime ASC) IS NULL THEN '0000'
			                                                ELSE
			                                                (SELECT TOP 1 DTR_LOGTIME FROM {0}..T_DTR 
			                                                WHERE 
			                                                Dtr_EmployeeID=LEDGER.Ttr_IDNo 
			                                                AND CONVERT(DATETIME,Dtr_LogDate)=DATEADD(DD,1,LEDGER.Ttr_Date)
				                                                AND CONVERT(INT,Dtr_LogTime) 
				                                                >= (CONVERT(INT,SHIFT.Scm_ShiftBreakEnd)-2400)
			                                                AND Dtr_LogType='O'
			                                                ORDER BY Dtr_LogTime ASC) END)
                                                ,Usr_Login='LOGUPLDSRVCS'
                                                ,Ludatetime = GETDATE()
                                                FROM {1}..[T_EmployeeLogLedger] LEDGER
	                                                LEFT JOIN 
	                                                {1}..T_ShiftCodeMaster SHIFT
	                                                ON Scm_ShiftCode=Ttr_ShiftCode
                                                WHERE Ttr_Date = @ProcessDate
                                                AND
                                                CONVERT(INT,Msh_ShiftIn1)>CONVERT(INT,Msh_ShiftOut2)
                                                AND Ttr_ActIn_1!='0000'
                                                AND Ttr_ActOut_2='0000'
                                                AND LEDGER.Usr_Login!='LOGUPLDSRVCS'
                                                AND 
	                                                (
	                                                CONVERT(INT,CASE WHEN(SELECT TOP 1 DTR_LOGTIME FROM {0}..T_DTR 
		                                                WHERE 
		                                                Dtr_EmployeeID=Ttr_IDNo 
		                                                AND CONVERT(DATETIME,Dtr_LogDate)=DATEADD(DD,1,Ttr_Date)
			                                                AND CONVERT(INT,Dtr_LogTime) 
			                                                >=(CONVERT(INT,SHIFT.Scm_ShiftBreakEnd)-2400)
		                                                AND Dtr_LogType='O'
		                                                ORDER BY Dtr_LogTime ASC) IS NULL THEN '0000'
		                                                ELSE
		                                                (SELECT TOP 1 
			                                                DTR_LOGTIME FROM {0}..T_DTR 
		                                                WHERE 
		                                                Dtr_EmployeeID=Ttr_IDNo 
		                                                AND CONVERT(DATETIME,Dtr_LogDate)=DATEADD(DD,1,Ttr_Date)
			                                                AND CONVERT(INT,Dtr_LogTime) 
			                                                >= (CONVERT(INT,SHIFT.Scm_ShiftBreakEnd)-2400)
		                                                AND Dtr_LogType='O'
		                                                ORDER BY Dtr_LogTime ASC) END)
	                                                ) <=1200", _dtrDBName, LedgerDBName);
                    #endregion

                    try
                    {
                        dtrDB.OpenDB();
                        //dtrDB.BeginTransaction();
                        Affected = dtrDB.ExecuteNonQuery(sql, CommandType.Text, paramInfo);
                        //dtrDB.CommitTransaction();
                        if (Affected > 0)
                            _log.WriteLog(Application.StartupPath, "Posting", "RecoverGraveyardOut : Committed", String.Format("Affected[{0}]", Affected.ToString()), true);
                    }
                    catch (Exception e)
                    {
                        _log.WriteLog(Application.StartupPath, "Posting", "RecoverGraveyardOut : RollBack", e.ToString(), true);
                        //dtrDB.RollBackTransaction();
                    }
                    finally
                    {
                        dtrDB.CloseDB();
                    }
                }
            }
            catch (Exception e)
            {
                _log.WriteLog(Application.StartupPath, "Posting", "RecoverGraveyardOut : General", e.ToString(), true);
            }
        }
        public void RecoverGraveUnpostLIFO(DateTime Processdate, String LedgerDBName)
        {
            if (MultiPocketVar.setFILO)
                return;
            try
            {
                {
                    int Affected = 0;
                    ParameterInfo[] paramInfo = new ParameterInfo[1];
                    paramInfo[0] = new ParameterInfo("@ProcessDate", Processdate.ToShortDateString(), SqlDbType.DateTime);
                    #region query
                    String sql = string.Format(@"UPDATE {1}..[T_EmployeeLogLedger]
	                                                --SETTING IN1
	                                                  SET
	                                                  Ttr_ActIn_1 =
		                                                (CASE WHEN(SELECT TOP 1 DTR_LOGTIME FROM {0}..T_DTR 
			                                                WHERE 
			                                                DTR_EmployeeID=LEDGER.Ttr_IDNo 
			                                                AND CONVERT(DATETIME,DTR_LogDate)=LEDGER.Ttr_Date 
				                                                AND CONVERT(INT,DTR_LogTime) 
				                                                < CONVERT(INT,SHIFT.Msh_ShiftIn1)
			                                                AND DTR_LogType='I' ORDER BY DTR_LogTime DESC) IS NULL THEN '0000'
			                                                ELSE 
			                                                (SELECT TOP 1 DTR_LOGTIME FROM {0}..T_DTR 
			                                                WHERE 
			                                                DTR_EmployeeID=LEDGER.Ttr_IDNo 
			                                                AND CONVERT(DATETIME,DTR_LogDate)=LEDGER.Ttr_Date 
				                                                AND CONVERT(INT,DTR_LogTime) 
				                                                < CONVERT(INT,SHIFT.Msh_ShiftIn1)
			                                                AND DTR_LogType='I' ORDER BY DTR_LogTime DESC)END)
	 
	                                                 --SETTING OUT2
	                                                  ,Ttr_ActOut_2 =
		                                                (CASE WHEN(SELECT TOP 1 DTR_LOGTIME FROM {0}..T_DTR 
			                                                WHERE 
			                                                DTR_EmployeeID=LEDGER.Ttr_IDNo 
			                                                AND CONVERT(DATETIME,DTR_LogDate)=DATEADD(DD,1,LEDGER.Ttr_Date)
				                                                AND CONVERT(INT,DTR_LogTime) 
				                                                >=(CONVERT(INT,SHIFT.Scm_ShiftBreakEnd)-2400)
			                                                AND DTR_LogType='O'
			                                                ORDER BY DTR_LogTime ASC) IS NULL THEN '0000'
			                                                ELSE
			                                                (SELECT TOP 1 DTR_LOGTIME FROM {0}..T_DTR 
			                                                WHERE 
			                                                DTR_EmployeeID=LEDGER.Ttr_IDNo 
			                                                AND CONVERT(DATETIME,DTR_LogDate)=DATEADD(DD,1,LEDGER.Ttr_Date)
				                                                AND CONVERT(INT,DTR_LogTime) 
				                                                >= (CONVERT(INT,SHIFT.Scm_ShiftBreakEnd)-2400)
			                                                AND DTR_LogType='O'
			                                                ORDER BY DTR_LogTime ASC)END)
	                                                  ,[Usr_Login]='LOGUPLDSRVC'
                                                      ,[Ludatetime]=GETDATE()
                                                  FROM {1}..[T_EmployeeLogLedger] LEDGER
	                                                LEFT JOIN 
	                                                {1}..T_ShiftCodeMaster SHIFT
	                                                ON Scm_ShiftCode=Ttr_ShiftCode

                                                WHERE Ttr_Date BETWEEN @ProcessDate AND @ProcessDate
                                                AND
                                                CONVERT(INT,Msh_ShiftIn1)>CONVERT(INT,Msh_ShiftOut2)
                                                --AND CONVERT(INT,Ttr_ActIn_1)<CONVERT(INT,Msh_ShiftOut2)
                                                AND CONVERT(INT,Ttr_ActOut_2)='0000'
                                                AND CONVERT(INT,Ttr_ActIn_1)='0000'

                                                --CHECKING OF IN 1
                                                AND
                                                (
		                                                ((SELECT TOP 1 DTR_LOGTIME FROM {0}..T_DTR 
				                                                WHERE 
				                                                DTR_EmployeeID=LEDGER.Ttr_IDNo 
				                                                AND CONVERT(DATETIME,DTR_LogDate)=LEDGER.Ttr_Date 
					                                                AND CONVERT(INT,DTR_LogTime) 
					                                                < CONVERT(INT,SHIFT.Msh_ShiftIn1)
				                                                AND DTR_LogType='I' ORDER BY DTR_LogTime DESC))!='0000'
		                                                OR
		
		                                                ((SELECT TOP 1 DTR_LOGTIME FROM {0}..T_DTR 
			                                                WHERE 
			                                                DTR_EmployeeID=LEDGER.Ttr_IDNo 
			                                                AND CONVERT(DATETIME,DTR_LogDate)=DATEADD(DD,1,LEDGER.Ttr_Date)
				                                                AND CONVERT(INT,DTR_LogTime) 
				                                                >= (CONVERT(INT,SHIFT.Scm_ShiftBreakEnd)-2400)
			                                                AND DTR_LogType='O'
			                                                ORDER BY DTR_LogTime ASC))!='0000'
                                                )
                                                --CHECKING OF OUT2

                                                --IN1 is PM
                                                AND
		                                                (CONVERT(INT,((SELECT TOP 1 DTR_LOGTIME FROM {0}..T_DTR 
			                                                WHERE 
			                                                DTR_EmployeeID=LEDGER.Ttr_IDNo 
			                                                AND CONVERT(DATETIME,DTR_LogDate)=DATEADD(DD,1,LEDGER.Ttr_Date)
				                                                AND CONVERT(INT,DTR_LogTime) 
				                                                >= (CONVERT(INT,SHIFT.Scm_ShiftBreakEnd)-2400)
			                                                AND DTR_LogType='O'
			                                                ORDER BY DTR_LogTime ASC))))<'1000'", _dtrDBName, LedgerDBName);
                    #endregion

                    try
                    {
                        dtrDB.OpenDB();
                        //dtrDB.BeginTransaction();
                        Affected = dtrDB.ExecuteNonQuery(sql, CommandType.Text, paramInfo);
                        //dtrDB.CommitTransaction();
                        if (Affected > 0)
                            _log.WriteLog(Application.StartupPath, "Posting", "RecoverGraveUnpostLIFO : Committed", String.Format("Affected[{0}]", Affected.ToString()), true);
                    }
                    catch (Exception e)
                    {
                        _log.WriteLog(Application.StartupPath, "Posting", "RecoverGraveUnpostLIFO : RollBack", e.ToString(), true);
                        //dtrDB.RollBackTransaction();
                    }
                    finally
                    {
                        dtrDB.CloseDB();
                    }
                }
            }
            catch (Exception e)
            {
                _log.WriteLog(Application.StartupPath, "Posting", "RecoverGraveUnpostLIFO : General", e.ToString(), true);
            }
        }
        public void startMaxPosting(DateTime processdate, String LedgerDBName, String LedgerTabe)
        {
            if (!MultiPocketVar.isDENZOPOSTING || !MultiPocketVar.setFILO)
                return;
            try
            {
                DataSet ds = MINMAXPosting(processdate, LedgerDBName, LedgerTabe);
                DataTable dt = ds.Tables[0];
                ds.Dispose();
                DataRow dr;
                int affected = 0;

                ParameterInfo[] paramInfo = new ParameterInfo[10];

                if (dt.Rows.Count > 0)
                {
                    dtrDB.OpenDB();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dt.Rows[i]["IN1"] = checkifnull(dt.Rows[i]["IN1"].ToString());
                        dt.Rows[i]["OUT1"] = checkifnull(dt.Rows[i]["OUT1"].ToString());
                        dt.Rows[i]["IN2"] = checkifnull(dt.Rows[i]["IN2"].ToString());
                        dt.Rows[i]["OUT2"] = checkifnull(dt.Rows[i]["OUT2"].ToString());
                        dt.Rows[i]["LEDGERIN1"] = FILO(dt.Rows[i]["IN1"].ToString(), dt.Rows[i]["LEDGERIN1"].ToString(), true);
                        dt.Rows[i]["LEDGEROUT1"] = FILO(dt.Rows[i]["OUT1"].ToString(), dt.Rows[i]["LEDGEROUT1"].ToString(), false);
                        dt.Rows[i]["LEDGERIN2"] = FILO(dt.Rows[i]["IN2"].ToString(), dt.Rows[i]["LEDGERIN2"].ToString(), true);
                        dt.Rows[i]["LEDGEROUT2"] = FILO(dt.Rows[i]["OUT2"].ToString(), dt.Rows[i]["LEDGEROUT2"].ToString(), false);

                        paramInfo[0] = new ParameterInfo("@EMPLOYEEID", dt.Rows[i]["EmployeeID"], SqlDbType.VarChar, 15);
                        paramInfo[1] = new ParameterInfo("@ProcessDate", processdate.ToShortDateString(), SqlDbType.DateTime);
                        paramInfo[2] = new ParameterInfo("@IN1", dt.Rows[i]["IN1"], SqlDbType.Char, 4);
                        paramInfo[3] = new ParameterInfo("@OUT1", dt.Rows[i]["OUT1"], SqlDbType.Char, 4);
                        paramInfo[4] = new ParameterInfo("@IN2", dt.Rows[i]["IN2"], SqlDbType.Char, 4);
                        paramInfo[5] = new ParameterInfo("@OUT2", dt.Rows[i]["OUT2"], SqlDbType.Char, 4);
                        paramInfo[6] = new ParameterInfo("@LEDGERIN1", dt.Rows[i]["LEDGERIN1"], SqlDbType.Char, 4);
                        paramInfo[7] = new ParameterInfo("@LEDGEROUT1", dt.Rows[i]["LEDGEROUT1"], SqlDbType.Char, 4);
                        paramInfo[8] = new ParameterInfo("@LEDGERIN2", dt.Rows[i]["LEDGERIN2"], SqlDbType.Char, 4);
                        paramInfo[9] = new ParameterInfo("@LEDGEROUT2", dt.Rows[i]["LEDGEROUT2"], SqlDbType.Char, 4);

                        //Remove Mid logs if  there is IN1 and OUT2
                        if (dt.Rows[i]["LEDGEROUT2"].ToString().Trim() != "0000" && dt.Rows[i]["LEDGERIN1"].ToString().Trim() != "0000")
                        {
                            paramInfo[7] = new ParameterInfo("@LEDGEROUT1", "0000", SqlDbType.Char, 4);
                            paramInfo[8] = new ParameterInfo("@LEDGERIN2", "0000", SqlDbType.Char, 4);
                        }


                        String sql = String.Format(@"UPDATE {0}..{1}
                                        SET Ttr_ActIn_1=@LEDGERIN1
                                            ,Ttr_ActOut_1=@LEDGEROUT1
                                            ,Ttr_ActIn_2=@LEDGERIN2
                                            ,Ttr_ActOut_2=@LEDGEROUT2
                                            ,USR_LOGIN='LOGUPLDSRVC'
                                            ,LUDATETIME=GETDATE()
                                        WHERE Ttr_Date=@ProcessDate
                                        AND Ttr_IDNo=@EMPLOYEEID", LedgerDBName, LedgerTabe);
                        //updating post flag
                        string sqlDTR = string.Format(@"UPDATE {0}..T_DTR
                                                       SET DTR_POSTFLAG=1
                                                       ,USR_LOGIN='LOGUPLDSRVC'
                                                       --,LUDATETIME=GETDATE()
                                                        WHERE CONVERT(DATETIME,DTR_LOGDATE,101)=@ProcessDate
                                                        AND 
                                                        (  (DTR_LOGTYPE='I' AND DTR_LOGTIME=@IN1)
                                                        OR (DTR_LOGTYPE='O' AND DTR_LOGTIME=@OUT1)
                                                        OR (DTR_LOGTYPE='I' AND DTR_LOGTIME=@IN2)
                                                        OR (DTR_LOGTYPE='O' AND DTR_LOGTIME=@OUT2)
                                                        )
                                                        AND DTR_EMPLOYEEID=@EMPLOYEEID", _dtrDBName);
                        try
                        {

                            _log.WriteLog(Application.StartupPath, "Posting", "Min/Max Posting : Started", "", true);
                            //dtrDB.BeginTransaction();
                            affected = dtrDB.ExecuteNonQuery(sql, CommandType.Text, paramInfo);
                            //dtrDB.CommitTransaction();

                            //dtrDB.BeginTransaction();
                            affected = dtrDB.ExecuteNonQuery(sqlDTR, CommandType.Text, paramInfo);
                            //dtrDB.CommitTransaction();
                            _log.WriteLog(Application.StartupPath, "Posting", "Min/Max Posting : Committed", "", true);

                        }
                        catch (Exception e)
                        {
                            //dtrDB.RollBackTransaction();
                            _log.WriteLog(Application.StartupPath, "Posting", "startMaxPosting : RollBack", e.ToString(), true);
                        }
                    }

                    dtrDB.CloseDB();
                }
            }
            catch (Exception e)
            {
                _log.WriteLog(Application.StartupPath, "Posting", "startMaxPosting : General", e.ToString(), true);
            }
        }
        public void getNightShiftHandlerLIFOINb4OUT(DateTime ProcessDate, String LedgerDBName, Double TimeGap)
        {

            if (MultiPocketVar.setFILO)
                return;
            DataSet ds = new DataSet();
            ParameterInfo[] paramInfo = new ParameterInfo[3];
            paramInfo[0] = new ParameterInfo("@START", ProcessDate.ToShortDateString(), SqlDbType.DateTime);
            paramInfo[1] = new ParameterInfo("@END", ProcessDate.ToShortDateString(), SqlDbType.DateTime);
            paramInfo[2] = new ParameterInfo("@GAP", TimeGap, SqlDbType.Float);
            #region query
            String sql = string.Format(@"SELECT 
	                                      CASE WHEN(SELECT TOP 1 DTR_LOGTIME FROM {1}..T_DTR 
			                                    WHERE 
			                                    Dtr_EmployeeID=Ttr_IDNo 
			                                    AND CONVERT(DATETIME,Dtr_LogDate)=Ttr_Date 
				                                    AND CONVERT(INT,Dtr_LogTime) 
				                                    < CONVERT(INT,SHIFT.Msh_ShiftIn1)
			                                    AND Dtr_LogType='I' ORDER BY Dtr_LogTime DESC) IS NULL THEN '--'
			                                    ELSE 
			                                    (SELECT TOP 1 DTR_LOGTIME FROM {1}..T_DTR 
			                                    WHERE 
			                                    Dtr_EmployeeID=Ttr_IDNo 
			                                    AND CONVERT(DATETIME,Dtr_LogDate)=Ttr_Date 
				                                    AND CONVERT(INT,Dtr_LogTime) 
				                                    < CONVERT(INT,SHIFT.Msh_ShiftIn1)
			                                    AND Dtr_LogType='I' ORDER BY Dtr_LogTime DESC) END AS DTRIN1
	                                      ,CASE WHEN (SELECT TOP 1 DTR_LOGTIME FROM {1}..T_DTR 
			                                    WHERE 
			                                    Dtr_EmployeeID=Ttr_IDNo 
			                                    AND CONVERT(DATETIME,Dtr_LogDate)=Ttr_Date
				                                    AND CONVERT(INT,Dtr_LogTime) 
				                                    <= CONVERT(INT,SHIFT.Scm_ShiftBreakStart)
			                                    AND Dtr_LogType='O'
			                                    AND CONVERT(DATETIME,Dtr_LogDate)=Ttr_Date
				                                    AND CONVERT(INT,Dtr_LogTime) 
				                                    > CONVERT(INT,SHIFT.Msh_ShiftIn1)
			                                    ORDER BY Dtr_LogTime ASC)IS NULL THEN '--' 
			                                    ELSE 
			                                    (SELECT TOP 1 DTR_LOGTIME FROM {1}..T_DTR 
			                                    WHERE 
			                                    Dtr_EmployeeID=Ttr_IDNo 
			                                    AND CONVERT(DATETIME,Dtr_LogDate)=Ttr_Date
				                                    AND CONVERT(INT,Dtr_LogTime) 
				                                    <= CONVERT(INT,SHIFT.Scm_ShiftBreakStart)
			                                    AND Dtr_LogType='O'
			                                    AND CONVERT(DATETIME,Dtr_LogDate)=Ttr_Date
				                                    AND CONVERT(INT,Dtr_LogTime) 
				                                    > CONVERT(INT,SHIFT.Msh_ShiftIn1)
			                                    ORDER BY Dtr_LogTime ASC) END AS DTROUT1
	                                      ,CASE WHEN (SELECT TOP 1 DTR_LOGTIME FROM {1}..T_DTR 
			                                    WHERE 
			                                    Dtr_EmployeeID=Ttr_IDNo 
			                                    AND CONVERT(DATETIME,Dtr_LogDate)=Ttr_Date 
				                                    AND CONVERT(INT,Dtr_LogTime) 
				                                    >= CONVERT(INT,SHIFT.Scm_ShiftBreakEnd)
			                                    AND Dtr_LogType='I' ORDER BY Dtr_LogTime DESC) IS NULL THEN '--'
			                                    ELSE
			                                    (SELECT TOP 1 DTR_LOGTIME FROM {1}..T_DTR 
			                                    WHERE 
			                                    Dtr_EmployeeID=Ttr_IDNo 
			                                    AND CONVERT(DATETIME,Dtr_LogDate)=Ttr_Date 
				                                    AND CONVERT(INT,Dtr_LogTime) 
				                                    >= CONVERT(INT,SHIFT.Scm_ShiftBreakEnd)
			                                    AND Dtr_LogType='I' ORDER BY Dtr_LogTime DESC) END AS DTRIN2
	                                      ,CASE WHEN(SELECT TOP 1 DTR_LOGTIME FROM {1}..T_DTR 
			                                    WHERE 
			                                    Dtr_EmployeeID=Ttr_IDNo 
			                                    AND CONVERT(DATETIME,Dtr_LogDate)=DATEADD(DD,1,Ttr_Date)
				                                    AND CONVERT(INT,Dtr_LogTime) 
				                                    >=(CONVERT(INT,SHIFT.Scm_ShiftBreakEnd)-2400)
			                                    AND Dtr_LogType='O'
			                                    ORDER BY Dtr_LogTime ASC) IS NULL THEN '--'
			                                    ELSE
			                                    (SELECT TOP 1 DTR_LOGTIME FROM {1}..T_DTR 
			                                    WHERE 
			                                    Dtr_EmployeeID=Ttr_IDNo 
			                                    AND CONVERT(DATETIME,Dtr_LogDate)=DATEADD(DD,1,Ttr_Date)
				                                    AND CONVERT(INT,Dtr_LogTime) 
				                                    >= (CONVERT(INT,SHIFT.Scm_ShiftBreakEnd)-2400)
			                                    AND Dtr_LogType='O'
			                                    ORDER BY Dtr_LogTime ASC) END AS DTROUT2
	                                      ,[Ttr_IDNo] AS ID
                                          ,[Ttr_Date] AS PRCDATE
                                          ,[Ttr_ActIn_1] AS IN1
                                          ,[Ttr_ActOut_1] AS OUT1
                                          ,[Ttr_ActIn_2] AS IN2
                                          ,[Ttr_ActOut_2] AS OUT2
                                          ,[Ttr_ShiftCode] AS SHIFTCOD
                                          ,Msh_ShiftIn1 AS SHFTIN
                                          ,Scm_ShiftBreakStart AS BRKIN
                                          ,Scm_ShiftBreakEnd AS BRKOUT
                                          ,Msh_ShiftOut2 AS SHFTOUT
	                                      FROM
	                                      (SELECT 
		                                       [Ttr_IDNo]
		                                      ,[Ttr_Date]
		                                      ,[Ttr_ActIn_1]
		                                      ,[Ttr_ActOut_1]
		                                      ,[Ttr_ActIn_2] 
		                                      ,[Ttr_ActOut_2] 
		                                      ,[Ttr_ShiftCode]
                                           FROM {0}..T_EmployeeLogLedger
                                           WHERE Ttr_Date BETWEEN @START AND @END
                                           UNION
	                                       SELECT 
		                                       [Ttr_IDNo] 
		                                      ,[Ttr_Date]
		                                      ,[Ttr_ActIn_1] 
		                                      ,[Ttr_ActOut_1]
		                                      ,[Ttr_ActIn_2] 
		                                      ,[Ttr_ActOut_2]
		                                      ,[Ttr_ShiftCode]
                                           FROM {0}..T_EmployeeLogLedgerHist
                                           WHERE Ttr_Date BETWEEN @START AND @END) LEDGERLIST
	                                    LEFT JOIN 
	                                    {0}..T_ShiftCodeMaster SHIFT
	                                    ON Scm_ShiftCode=Ttr_ShiftCode
                                    WHERE
                                    CONVERT(INT,Msh_ShiftIn1)>CONVERT(INT,Msh_ShiftOut2)
                                    AND Ttr_ActOut_1='0000'
                                    AND Ttr_ActOut_2='0000'
                                    AND Ttr_ActIn_1!='0000'
                                    AND 
	                                    CONVERT(INT,REPLACE(LEFT(CONVERT(varchar(25),DATEADD(MINUTE,(120),LEFT(Ttr_ActIn_1,2)+':'+RIGHT(Ttr_ActIn_1,2)),24),5),':',''))
	                                    <
	                                    CONVERT(INT,Msh_ShiftIn1)
                                    AND
		                                    -- DTR OUT >= IN1 - GAP
		                                    (
		                                    CONVERT(INT,CASE WHEN(SELECT TOP 1 DTR_LOGTIME FROM {1}..T_DTR 
		                                    WHERE 
		                                    Dtr_EmployeeID=Ttr_IDNo 
		                                    AND CONVERT(DATETIME,Dtr_LogDate)=DATEADD(DD,1,Ttr_Date)
			                                    AND CONVERT(INT,Dtr_LogTime) 
			                                    >=(CONVERT(INT,SHIFT.Scm_ShiftBreakEnd)-2400)
		                                    AND Dtr_LogType='O'
		                                    ORDER BY Dtr_LogTime ASC) IS NULL THEN '0000'
		                                    ELSE
		                                    (SELECT TOP 1 
			                                    DTR_LOGTIME FROM {1}..T_DTR 
		                                    WHERE 
		                                    Dtr_EmployeeID=Ttr_IDNo 
		                                    AND CONVERT(DATETIME,Dtr_LogDate)=DATEADD(DD,1,Ttr_Date)
			                                    AND CONVERT(INT,Dtr_LogTime) 
			                                    >= (CONVERT(INT,SHIFT.Scm_ShiftBreakEnd)-2400)
		                                    AND Dtr_LogType='O'
		                                    ORDER BY Dtr_LogTime ASC) END)
		                                    )>=CONVERT(INT,REPLACE(LEFT(CONVERT(varchar(25),DATEADD(MINUTE,(@GAP*-1),LEFT(Ttr_ActIn_1,2)+':'+RIGHT(Ttr_ActIn_1,2)),24),5),':',''))
                                    AND
		                                    -- OUT1 != 0
		                                    CASE WHEN(SELECT TOP 1 DTR_LOGTIME FROM {1}..T_DTR 
			                                    WHERE 
			                                    Dtr_EmployeeID=Ttr_IDNo 
			                                    AND CONVERT(DATETIME,Dtr_LogDate)=Ttr_Date 
				                                    AND CONVERT(INT,Dtr_LogTime) 
				                                    < CONVERT(INT,SHIFT.Msh_ShiftIn1)
			                                    AND Dtr_LogType='I' ORDER BY Dtr_LogTime DESC) IS NULL THEN '0000'
			                                    ELSE 
			                                    (SELECT TOP 1 DTR_LOGTIME FROM {1}..T_DTR 
			                                    WHERE 
			                                    Dtr_EmployeeID=Ttr_IDNo 
			                                    AND CONVERT(DATETIME,Dtr_LogDate)=Ttr_Date 
				                                    AND CONVERT(INT,Dtr_LogTime) 
				                                    < CONVERT(INT,SHIFT.Msh_ShiftIn1)
			                                    AND Dtr_LogType='I' ORDER BY Dtr_LogTime DESC)END!='0000'
                                    AND
		                                    --IN1 is not in the morning
	                                        (CONVERT(INT,CASE WHEN(SELECT TOP 1 DTR_LOGTIME FROM {1}..T_DTR 
			                                    WHERE 
			                                    Dtr_EmployeeID=Ttr_IDNo 
			                                    AND CONVERT(DATETIME,Dtr_LogDate)=Ttr_Date 
				                                    AND CONVERT(INT,Dtr_LogTime) 
				                                    < CONVERT(INT,SHIFT.Msh_ShiftIn1)
			                                    AND Dtr_LogType='I' ORDER BY Dtr_LogTime DESC) IS NULL THEN '0000'
			                                    ELSE 
			                                    (SELECT TOP 1 
                                                    -- hardcoded 3 hours befor shift time in
				                                    REPLACE(LEFT(CONVERT(varchar(25),DATEADD(MINUTE,(180),LEFT(DTR_LOGTIME,2)+':'+RIGHT(DTR_LOGTIME,2)),24),5),':','')
		                                        FROM {1}..T_DTR 
			                                    WHERE 
			                                    Dtr_EmployeeID=Ttr_IDNo 
			                                    AND CONVERT(DATETIME,Dtr_LogDate)=Ttr_Date 
				                                    AND CONVERT(INT,Dtr_LogTime) 
				                                    < CONVERT(INT,SHIFT.Msh_ShiftIn1)
			                                    AND Dtr_LogType='I' ORDER BY Dtr_LogTime DESC) END)
			                                    )>=CONVERT(INT,Msh_ShiftIn1)
                                    ORDER BY Ttr_Date,Ttr_IDNo", LedgerDBName, _dtrDBName);
            #endregion
            try
            {
                dtrDB.OpenDB();
                //dtrDB.BeginTransaction();
                ds = dtrDB.ExecuteDataSet(sql, CommandType.Text, paramInfo);
                //dtrDB.CommitTransaction();
                setNightShiftHandlerLIFOINb4OUT(ds, LedgerDBName);
            }
            catch (Exception e)
            {
                //dtrDB.RollBackTransaction();
                _log.WriteLog(Application.StartupPath, "Posting", "NightShiftHandlerLIFOINb4OUT : RollBack", e.ToString(), true);
            }
            finally
            {
                dtrDB.CloseDB();
            }
        }
        public void UnpostRecovery(DateTime ProcessDate, String LedgerDBName)
        {
            if (MultiPocketVar._ServiceCode.Trim().ToUpper() == MultiPocketVar.MPCode)
                return;//to prevent this function from executing when running Multipockets.
            try
            {
                int Affected = 0;
                ParameterInfo[] paramInfo = new ParameterInfo[1];
                paramInfo[0] = new ParameterInfo("@ProcessDate", ProcessDate.ToShortDateString(), SqlDbType.DateTime);
                String sqlINRecovery = string.Format(@"UPDATE {1}..T_DTR
                                                      SET Dtr_PostFlag='0'
                                                      FROM [{0}].[dbo].[{2}]
	                                                    JOIN
	                                                    [{0}].[dbo].T_ShiftCodeMaster
	                                                    ON Ttr_ShiftCode=Scm_ShiftCode
		                                                    LEFT JOIN 
		                                                    {1}..T_DTR
		                                                    ON CONVERT(DATETIME,Dtr_LogDate,101)=Ttr_Date
		                                                    AND Dtr_EmployeeID=Ttr_IDNo
		                                                    AND Dtr_LogType='I'
	                                                    WHERE Ttr_Date = @ProcessDate
	                                                    AND Ttr_ActIn_1='0000' AND CONVERT(INT,Dtr_LogTime)<1200 AND Dtr_LogTime!='NULL'
	                                                    AND CONVERT(INT,Msh_ShiftIn1)<CONVERT(INT,Msh_ShiftOut2)", LedgerDBName, _dtrDBName, MultiPocketVar.T_Ledger);

                try
                {
                    dtrDB.OpenDB();
                    //dtrDB.BeginTransaction();
                    Affected = dtrDB.ExecuteNonQuery(sqlINRecovery, CommandType.Text, paramInfo);
                    //dtrDB.CommitTransaction();
                }
                catch (Exception e)
                {
                    _log.WriteLog(Application.StartupPath, "Posting", "UnpostRecovery : RollBack", e.ToString(), true);
                    //dtrDB.RollBackTransaction();
                }
                finally
                {
                    dtrDB.CloseDB();
                }
            }
            catch (Exception e)
            {
                _log.WriteLog(Application.StartupPath, "Posting", "UnpostRecovery : General", e.ToString(), true);
            }
        }
        #endregion

        #endregion

        #region Manipulate the shift code

        public void ManipulateShiftCode(DateTime ProcessDate, String LedgerDBName, String LedgerCompanyCode)
        {
            try
            {
                if (!CommonProcedures.GetAppSettingConfigBool("AutoChangeShift", true))
                {
                    return;
                }
                int affected = 0;
                if (CommonProcedures.GetAppSettingConfigBool("DefaultShift", true))
                {
                    ParameterInfo[] paramInfo = new ParameterInfo[1];
                    paramInfo[0] = new ParameterInfo("@ProcessDate", ProcessDate.ToShortDateString(), SqlDbType.DateTime);

                    //rest day and holiday : shift handling
                    string sqlHolRest = string.Format(@"UPDATE [{0}].[dbo].[{1}] 
		                                    SET Ttr_ShiftCode = 
			                                CASE 
			                                WHEN Ttr_RestDayFlag=1 OR Ttr_HolidayFlag=1 THEN 
				                                (SELECT Msh_8HourShiftCode FROM [{0}].[dbo].M_Shift 
					                                WHERE 
					                                Msh_ShiftCode = Ttr_ShiftCode
					                                AND Msh_Schedule = Ttr_ScheduleType
                                                    AND Msh_CompanyCode = '{2}')
			                                ELSE Ttr_ShiftCode
			                                END --restday or holiday
                                            WHERE Ttr_Date=@ProcessDate", LedgerDBName, MultiPocketVar.T_Ledger, LedgerCompanyCode);

                    try
                    {
                        dtrDB.OpenDB();
                        //dtrDB.BeginTransaction();
                        affected = dtrDB.ExecuteNonQuery(sqlHolRest, CommandType.Text, paramInfo);
                        //dtrDB.CommitTransaction();
                    }
                    catch (Exception e)
                    {
                        //dtrDB.RollBackTransaction();
                        _log.WriteLog(Application.StartupPath, "Posting", "ManipulateShiftCode : RollBack", e.ToString(), true);

                    }
                    finally
                    {
                        dtrDB.CloseDB();
                    }

                    affected = 0;
                    //wednesday and leave : default shift handling WEDNESDAY CPH HARDCODED
                    #region CPH Query
                    string sqlWedLeave = "";
                    if (MultiPocketVar.isCHPAUTOSHIFT)
                        sqlWedLeave = string.Format(@"UPDATE [{0}].[dbo].[{1}] 
		                                        SET Ttr_ShiftCode =
			                                    CASE
			                                    WHEN 
			                                    (CONVERT(FLOAT,Ell_EncodedNoPayLeaveHr)+CONVERT(FLOAT,Ell_EncodedNoPayLeaveHr)>0 AND Ell_DayCode='REG') 
			                                    OR 
			                                    (datepart(WEEKDAY,Ttr_Date)=4 and Ttr_HolidayFlag!=1 and Ttr_RestDayFlag!=1)
			                                    THEN
				                                    (SELECT [Scm_ShiftCode] FROM [{0}].[dbo].T_ShiftCodeMaster 
					                                    WHERE 
					                                    Scm_DefaultShift = 'TRUE'
					                                    and scm_scheduletype = Ell_ScheduleType)
			                                    ELSE Ttr_ShiftCode
			                                    END -- wednesday or leave
                                                WHERE Ttr_Date=@ProcessDate", LedgerDBName, MultiPocketVar.T_Ledger);
                    #endregion
                    #region DENZO Query
                    //Denzo default shift for with advance type OT
                    if (MultiPocketVar.isDENZOPOSTING)
                        sqlWedLeave = string.Format(@"UPDATE [{0}].[dbo].[{1}] 
		                                        SET Ttr_ShiftCode =
			                                    CASE
			                                    WHEN 
			                                    (CONVERT(FLOAT,Ell_EncodedNoPayLeaveHr)+CONVERT(FLOAT,Ell_EncodedNoPayLeaveHr)>0 AND Ell_DayCode='REG') 
			                                    OR
                                                Ttr_IDNo IN
	                                                    (SELECT DISTINCT Eot_EmployeeId FROM {0}..T_EmployeeOvertime
	                                                    WHERE Eot_OvertimeDate=@Processdate
	                                                    AND Eot_OvertimeType='A')
                                                AND
			                                    (Ttr_HolidayFlag!=1 and Ttr_RestDayFlag!=1)
			                                    THEN
				                                    (SELECT [Scm_ShiftCode] FROM [{0}].[dbo].T_ShiftCodeMaster 
					                                    WHERE 
					                                    Scm_DefaultShift = 'TRUE'
					                                    and scm_scheduletype = Ell_ScheduleType)
			                                    ELSE Ttr_ShiftCode
			                                    END -- Advance OT Type or leave
                                                WHERE Ttr_Date=@ProcessDate", LedgerDBName, MultiPocketVar.T_Ledger);

                    #endregion

                    try
                    {
                        dtrDB.OpenDB();
                        //dtrDB.BeginTransaction();
                        affected = dtrDB.ExecuteNonQuery(sqlWedLeave, CommandType.Text, paramInfo);
                        //dtrDB.CommitTransaction();
                    }
                    catch (Exception e)
                    {
                        _log.WriteLog(Application.StartupPath, "Posting", "ManipulateShiftCode : RollBack", e.ToString(), true);
                        //dtrDB.RollBackTransaction();
                    }
                    finally
                    {
                        dtrDB.CloseDB();
                    }
                }
            }
            catch (Exception e)
            {
                _log.WriteLog(Application.StartupPath, "Posting", "ManipulateShiftCode : General", e.ToString(), true);
            }
        }

        public void FlexShift(DateTime ProcessDate, String LedgerDBName, String LedgerCompanyCode)
        {
            try
            {
                if (!CommonProcedures.GetAppSettingConfigBool("AutoChangeShift", true))
                {
                    return;
                }
                int affected = 0;
                ParameterInfo[] paramInfo = new ParameterInfo[1];
                paramInfo[0] = new ParameterInfo("@Processdate", ProcessDate.ToShortDateString(), SqlDbType.DateTime);
                //flex shift handling
                String sqlFlexShift = "";
                String sqlShiftMV = "";
                #region CPH Query
                if (MultiPocketVar.isCHPAUTOSHIFT)
                    sqlFlexShift = String.Format(@"UPDATE {0}..{1}
		                                            SET Ttr_ShiftCode = CASE WHEN 
	                                                                    (SELECT TOP 1 Msh_ShiftCode
	                                                                    FROM {0}..M_Shift
	                                                                    WHERE CONVERT(INT,Msh_ShiftIn1)>=CONVERT(INT,Ttr_ActIn_1)
	                                                                    AND Msh_ShiftCode NOT LIKE 'R00%'
	                                                                    AND Ttr_ScheduleType=Msh_Schedule
                                                                        AND Msh_CompanyCode = '{3}'
	                                                                    AND Msh_RecordStatus = 'A') 
	                                                                    IS NULL THEN 
					                                                                    (SELECT TOP 1 Msh_ShiftCode from {0}..M_Shift 
                                                                                        WHERE Msh_Schedule='N' 
                                                                                        AND Msh_CompanyCode = '{3}'
					                                                                    AND Msh_ShiftCode not like 'R00%' 
					                                                                    AND Msh_RecordStatus = 'A'
					                                                                    ORDER BY Msh_ShiftIn1 DESC)
	                                                                    ELSE
	                                                                    (SELECT TOP 1 Msh_ShiftCode
	                                                                    FROM {0}..M_Shift
	                                                                    WHERE CONVERT(INT,Msh_ShiftIn1)>=CONVERT(INT,Ttr_ActIn_1)
	                                                                    AND Ttr_ScheduleType=Msh_Schedule
                                                                        AND Msh_CompanyCode = '{3}'
	                                                                    AND Msh_RecordStatus = 'A') END
		                                            FROM {0}..{1} 
                                                        LEFT JOIN {0}..M_Employee
                                                        ON Ttr_IDNo=Mem_IDNo
		                                            WHERE CONVERT(INT,Ttr_ActIn_1)>0
		                                            AND Ttr_HolidayFlag!=1 AND Ttr_RestDayFlag!=1 AND DATEPART(WEEKDAY,Ttr_Date)!=4
                                                    AND (CONVERT(FLOAT,Ttr_WFNoPayLVHr)+CONVERT(FLOAT,Ttr_WFNoPayLVHr))<=0
                                                    AND Ttr_Date = @Processdate
                                                    {2} -- Position Exclusion(DRVR)", LedgerDBName, MultiPocketVar.T_Ledger, GetPositionExcluded(), LedgerCompanyCode);
                #endregion
                #region Denzo Query
                if (MultiPocketVar.isDENZOPOSTING)
                {
                    #region Query Flex Shift
                    sqlFlexShift = String.Format(@"UPDATE {0}..{1}
		                                            SET Ttr_ShiftCode = case when 
	                                                                    (SELECT TOP 1 [Scm_ShiftCode]
	                                                                    FROM {0}..[T_ShiftCodeMaster] 
	                                                                    WHERE CONVERT(INT,Msh_ShiftIn1)>=CONVERT(INT,Ttr_ActIn_1)
	                                                                    AND Scm_ShiftCode NOT LIKE 'R00%'
	                                                                    AND Ell_ScheduleType=Scm_ScheduleType
	                                                                    AND Scm_Status = 'A') 
	                                                                    IS null  then 
					                                                                    (select top 1 Scm_ShiftCode from {0}..T_ShiftCodeMaster where Scm_ScheduleType='D' 
					                                                                    and Scm_ShiftCode not like 'R00%' 
					                                                                    and Scm_Status = 'A'
					                                                                    order  by Msh_ShiftIn1 desc)
	                                                                    else
	                                                                    (SELECT TOP 1 [Scm_ShiftCode]
	                                                                    FROM {0}..[T_ShiftCodeMaster] 
	                                                                    WHERE CONVERT(INT,Msh_ShiftIn1)>=CONVERT(INT,Ttr_ActIn_1)
	                                                                    AND Ell_ScheduleType=Scm_ScheduleType
	                                                                    AND Scm_Status = 'A')END
		                                            FROM {0}..{1} 
                                                        LEFT JOIN {0}..T_EmployeeMaster
                                                        ON Ttr_IDNo=Emt_EmployeeID
		                                            WHERE CONVERT(INT,Ttr_ActIn_1)>0
		                                            AND Ttr_HolidayFlag!=1 AND Ttr_RestDayFlag!=1
                                                    AND (CONVERT(FLOAT,Ell_EncodedNoPayLeaveHr)+CONVERT(FLOAT,Ell_EncodedNoPayLeaveHr))<=0
                                                    AND Ttr_Date = @Processdate
                                                    AND Ttr_IDNo NOT IN
	                                                    (SELECT DISTINCT Eot_EmployeeId FROM {0}..T_EmployeeOvertime
	                                                    WHERE Eot_OvertimeDate=@Processdate
	                                                    AND Eot_OvertimeType='A')
                                                    {2} -- Emp Status Exclusion(ProB)", LedgerDBName, MultiPocketVar.T_Ledger, GetEmploymentExcluded());
                    #endregion
                    #region Query Approved Shift Movement
                    sqlShiftMV = String.Format(@"UPDATE {0}..{1} set Ttr_ShiftCode=mve_to
                                                               FROM {0}..{1}
                                                        LEFT JOIN 
	                                                        {0}..T_MOVEMENT
	                                                        ON [Mve_EmployeeId] = Ttr_IDNo
	                                                        AND convert(varchar,[Mve_ApprovedDate],101) = @Processdate
	                                                        LEFT JOIN {0}..T_EmployeeMaster --set hrc
		                                                        ON Ttr_IDNo=Emt_EmployeeID
			                                                        WHERE
			                                                        --CONVERT(INT,Ttr_ActIn_1)>0
			                                                        --AND
			                                                        Ttr_HolidayFlag!=1 AND Ttr_RestDayFlag!=1
			                                                        AND (CONVERT(FLOAT,Ell_EncodedNoPayLeaveHr)+CONVERT(FLOAT,Ell_EncodedNoPayLeaveHr))<=0
			                                                        AND Ttr_Date = convert(varchar,[Mve_EffectivityDate],101)
			                                                        --AND Ttr_IDNo NOT IN
				                                                    --    (SELECT DISTINCT Eot_EmployeeId FROM {0}..T_EmployeeOvertime --set HRC
				                                                    --    WHERE convert(varchar,Eot_OvertimeDate,101)=convert(varchar,[Mve_EffectivityDate],101)
				                                                    --    AND Eot_OvertimeType='A') --prioritize shift movement
			                                                        {2} -- Emp Status Exclusion(ProB)
			                                                        AND [Mve_Status] = 9
			                                                        AND Ttr_ShiftCode!=MVE_TO", LedgerDBName, MultiPocketVar.T_Ledger, GetEmploymentExcluded());
                    #endregion

                }
                #endregion
                try
                {
                    dtrDB.OpenDB();
                    //dtrDB.BeginTransaction();
                    affected = dtrDB.ExecuteNonQuery(sqlFlexShift, CommandType.Text, paramInfo);
                    //dtrDB.CommitTransaction();

                    //dtrDB.BeginTransaction();
                    DataSet test = dtrDB.ExecuteDataSet(sqlShiftMV, CommandType.Text, paramInfo);
                    affected = dtrDB.ExecuteNonQuery(sqlShiftMV, CommandType.Text, paramInfo);
                    //dtrDB.CommitTransaction();
                }
                catch (Exception e)
                {
                    //dtrDB.RollBackTransaction();
                    _log.WriteLog(Application.StartupPath, "Posting", "FlexShift : RollBack", e.ToString(), true);
                }
                finally
                {
                    dtrDB.CloseDB();
                }
            }
            catch (Exception e)
            {
                _log.WriteLog(Application.StartupPath, "Posting", "FlexShift : General", e.ToString(), true);
            }

        }

        public string GetEmploymentExcluded()
        {
            string sql = "";
            int count = ConfigurationManager.AppSettings["EmploymentStatusExclusion"].Trim().Split(',').Length;
            string[] Status = new string[count];
            Status = ConfigurationManager.AppSettings["EmploymentStatusExclusion"].Trim().Split(',');
            for (int i = 0; i < count; i++)
            {
                Status[i] = string.Format(" AND Emt_EmploymentStatus!= '{0}' ", Status[i]);
                sql = sql + Status[i];
            }
            return sql;
        }

        public string GetPositionExcluded()
        {
            string sql = "";
            int count = ConfigurationManager.AppSettings["PositionExclusion"].Trim().Split(',').Length;
            string[] Positions = new string[count];
            Positions = ConfigurationManager.AppSettings["PositionExclusion"].Trim().Split(',');
            for (int i = 0; i < count; i++)
            {
                Positions[i] = string.Format(" AND Emt_PositionCode!= '{0}' ", Positions[i]);
                sql = sql + Positions[i];
            }
            return sql;
        }
        #endregion

        #region Handling Advance Over Time Application
        public DataSet OTAdvanceApplication(DateTime processdate, String LedgerDBName)
        {

            DataSet dsOT = new DataSet();

            if (MultiPocketVar.isDENZOPOSTING)
                return dsOT;

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@ProcessDate", processdate.ToShortDateString());

            String sqlOTApp = String.Format(@"select Ttr_IDNo AS EmployeeID
                                             , Ttr_Date AS OTDate
                                             , Ttr_ShiftCode AS ShiftCode
                                             , eot_starttime AS StartTime
                                             , eot_endtime AS EndTime
                                             ,{0}.dbo.addMinutes(eot_starttime, (Convert(int, RIGHT(Eot_StartTime,2)) - Convert(int, RIGHT(Msh_ShiftOut2,2))) * -1) AS [NewStartTime]
                                             ,{0}.dbo.addMinutes(eot_endtime
                                                 , (Convert(int, RIGHT(Eot_StartTime,2)) - Convert(int, RIGHT(Msh_ShiftOut2,2))) * -1
                                                 ) AS [NewEndTime]
                                                 , Eot_OvertimeHour
                                            from {0}..{1}
                                            inner join {0}..{2} on Ttr_IDNo  = Eot_EmployeeId and Ttr_Date = Eot_OvertimeDate
                                            inner join {0}..T_ShiftCodeMaster on Scm_ShiftCode = Ttr_ShiftCode
                                            where Msh_ShiftOut2 <> Eot_StartTime
                                            and LEFT(Msh_ShiftOut2,2) = LEFT(Eot_StartTime,2)
                                            and (Ttr_ActIn_1 <> '0000' or Ttr_ActIn_2 <> '0000')
                                            and Ttr_Date =@ProcessDate
                                            and Ell_DayCode = 'REG'", LedgerDBName, MultiPocketVar.T_Overtime, MultiPocketVar.T_Ledger);

            try
            {
                dtrDB.OpenDB();
                //dtrDB.BeginTransaction();
                dsOT = dtrDB.ExecuteDataSet(sqlOTApp, CommandType.Text, paramInfo);
                //dtrDB.CommitTransaction();
            }
            catch (Exception e)
            {
                //dtrDB.RollBackTransaction();
                _log.WriteLog(Application.StartupPath, "Posting", "OTAdvanceApplication : RollBack", e.ToString(), true);
            }
            finally
            {
                dtrDB.CloseDB();
            }
            return dsOT;
        }

        public void UpdateEmployeeAdvanceOTApp(DateTime processdate, String LedgerDBName)
        {
            try
            {
                if (!CommonProcedures.GetAppSettingConfigBool("AutoChangeShift", true) || MultiPocketVar.isDENZOPOSTING)
                {
                    return;
                }
                DataSet dsOT = OTAdvanceApplication(processdate, LedgerDBName);
                DataTable dtOT = dsOT.Tables[0];
                DataRow drOT;
                int affected = 0;

                ParameterInfo[] paramInfo = new ParameterInfo[7];

                if (dtOT.Rows.Count > 0)
                {
                    dtrDB.OpenDB();

                    for (int i = 0; i < dtOT.Rows.Count; i++)
                    {
                        paramInfo[0] = new ParameterInfo("@EmployeeID", dtOT.Rows[i]["EmployeeID"], SqlDbType.VarChar, 15);
                        paramInfo[1] = new ParameterInfo("@OTDate", dtOT.Rows[i]["OTDate"], SqlDbType.DateTime);
                        paramInfo[2] = new ParameterInfo("@StartTime", dtOT.Rows[i]["StartTime"], SqlDbType.Char, 4);
                        paramInfo[3] = new ParameterInfo("@EndTime", dtOT.Rows[i]["EndTime"], SqlDbType.Char, 4);
                        paramInfo[4] = new ParameterInfo("@NewStartTime", dtOT.Rows[i]["NewStartTime"], SqlDbType.Char, 4);
                        paramInfo[5] = new ParameterInfo("@NewEndTime", dtOT.Rows[i]["NewEndTime"], SqlDbType.Char, 4);
                        paramInfo[6] = new ParameterInfo("@Eot_OvertimeHour", dtOT.Rows[i]["Eot_OvertimeHour"], SqlDbType.Decimal);

                        String sql = String.Format(@"UPDATE {0}..{1}
                                        SET Tot_StartTime=@NewStartTime
                                           ,Tot_EndTime=@NewEndTime
                                           ,Usr_Login='LOGUPLDSRVC'
                                        WHERE Tot_IDNo=@EmployeeID
                                        AND CONVERT(VARCHAR,Tot_OvertimeDate,101)=@OTDate
                                        AND CONVERT(INT,Tot_StartTime)=@StartTime
                                        AND CONVERT(INT,Tot_EndTime)=@EndTime", LedgerDBName, MultiPocketVar.T_Overtime);
                        try
                        {
                            //dtrDB.BeginTransaction();
                            affected = dtrDB.ExecuteNonQuery(sql, CommandType.Text, paramInfo);
                            //dtrDB.CommitTransaction();
                            _log.WriteLog(Application.StartupPath, "AdvanceOTApplicationORIG.Log", "",
                               String.Format("{0},{1},{2},{3},{4},{5},{6}"
                                                , dtOT.Rows[i]["EmployeeID"].ToString()
                                                , dtOT.Rows[i]["OTDate"].ToString()
                                                , dtOT.Rows[i]["StartTime"].ToString()
                                                , dtOT.Rows[i]["EndTime"].ToString()
                                                , dtOT.Rows[i]["NewStartTime"].ToString()
                                                , dtOT.Rows[i]["NewEndTime"].ToString()
                                                , dtOT.Rows[i]["Eot_OvertimeHour"].ToString())
                               , true);
                        }
                        catch (Exception e)
                        {
                            //dtrDB.RollBackTransaction();
                            _log.WriteLog(Application.StartupPath, "Posting", "UpdateEmployeeAdvanceOTApp : RollBack", e.ToString(), true);
                        }
                    }

                    dtrDB.CloseDB();
                }
            }
            catch (Exception e)
            {
                _log.WriteLog(Application.StartupPath, "Posting", "UpdateEmployeeAdvanceOTApp : General", e.ToString(), true);
            }
        }
        #endregion

        #region Multi packet Handling

        #region Pocket Paring Process

        public DataTable dtRegShift(DateTime ProcessDate, String LedgerDBName, String LedgerCompanyCode, string CentralProfile, string EmploeeID)
        {
            string condition = "";
            if (EmploeeID != "")
                condition = string.Format("AND Tel_IDNo = '{0}'", EmploeeID);

            //get list of dtr order by ID , Date , Type
            string query = string.Format(@"SELECT Tel_IDNo
                                             ,(CONVERT(CHAR(10),Tel_LogDate,101) + ' ' + Substring(Tel_LogTime, 1, 2) + ':' + Substring(Tel_LogTime, 3, 2)) AS Tel_Log
                                             ,Tel_LogType
                                             ,Ttr_PayCycle AS PayPeriod
                                             ,DTR.Usr_Login
                                             ,Ttr_Date AS 'PROCESSDATE'
	                                         ,Ttr_ShiftCode AS SHIFT
                                             ,SHFT.Msh_ShiftIn1
                                             ,SHFT.Msh_ShiftOut2
                                         FROM [{0}].dbo.T_EmpDTR DTR
                                         RIGHT JOIN [{1}].[dbo].T_EmpTimeRegister 
                                            ON Tel_IDNo = Ttr_IDNo
                                            AND CONVERT(DATE,Ttr_Date) = @ProcessDate
                                         LEFT JOIN [{3}].dbo.M_Shift SHFT ON SHFT.Msh_ShiftCode = Ttr_ShiftCode
                                            AND SHFT.Msh_CompanyCode = '{2}'
                                         WHERE Tel_LogDate BETWEEN @ProcessDate and @ProcessDate
                                         AND CONVERT(INT,SHFT.Msh_ShiftIn1) < CONVERT(INT,SHFT.Msh_ShiftOut2)
                                         {4} 
                                         ORDER BY Tel_IDNo ASC
                                         ,CONVERT(DATETIME,CONVERT(CHAR(10),Tel_LogDate,101) + ' ' + SUBSTRING(Tel_LogTime, 1, 2) + ':' + SUBSTRING(Tel_LogTime, 3, 2)) ASC
                                        ", _dtrDBName, LedgerDBName, LedgerCompanyCode, CentralProfile, condition);

            if (MultiPocketVar.isOTAfterMidnight)
            {
                query = string.Format(@"SELECT Tel_IDNo
                                             ,(CONVERT(CHAR(10),Tel_LogDate,101) + ' ' + Substring(Tel_LogTime, 1, 2) + ':' + Substring(Tel_LogTime, 3, 2)) AS Tel_Log
                                             ,Tel_LogType
                                             ,Ttr_PayCycle AS PayPeriod
                                             ,DTR.Usr_Login
                                             ,Ttr_Date AS 'PROCESSDATE'
	                                         ,Ttr_ShiftCode AS SHIFT
                                             ,SHFT.Msh_ShiftIn1
                                             ,SHFT.Msh_ShiftOut2
                                         FROM [{0}].[dbo].T_EmpDTR DTR
                                         RIGHT JOIN [{1}].[dbo].T_EmpTimeRegister ON Tel_IDNo = Ttr_IDNo
                                            AND CONVERT(DATE,Ttr_Date) = @ProcessDate
                                         LEFT JOIN [{3}].[dbo].M_Shift SHFT ON SHFT.Msh_ShiftCode = Ttr_ShiftCode
                                            AND SHFT.Msh_CompanyCode = '{2}'
                                         WHERE Tel_LogDate BETWEEN @ProcessDate and DateAdd(DD,1,@ProcessDate)
                                         AND CONVERT(INT,SHFT.Msh_ShiftIn1) < CONVERT(INT,SHFT.Msh_ShiftOut2)
                                         AND CONVERT(DATETIME,CONVERT(CHAR(10),Tel_LogDate,101) + ' ' + Substring(Tel_LogTime, 1, 2) + ':' + Substring(Tel_LogTime, 3, 2)) < DateAdd(HH,30,@ProcessDate)
                                         {4} 
                                         ORDER BY Tel_IDNo ASC
                                         ,CONVERT(DATETIME,CONVERT(CHAR(10),Tel_LogDate,101) + ' ' + Substring(Tel_LogTime, 1, 2) + ':' + Substring(Tel_LogTime, 3, 2)) ASC
                                        ", _dtrDBName, LedgerDBName, LedgerCompanyCode, CentralProfile, condition);
            }
            DataTable dt = new DataTable();
            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@ProcessDate", ProcessDate.ToShortDateString(), SqlDbType.Date);


            try
            {
                dtrDB.OpenDB();
                //dtrDB.BeginTransaction();
                dt = dtrDB.ExecuteDataSet(query, CommandType.Text, paramInfo).Tables[0];
                //dtrDB.CommitTransaction();
            }
            catch (Exception e)
            {
                //dtrDB.RollBackTransaction();
                _log.WriteLog(Application.StartupPath, "MultipocketPosting for regular shift", " : RollBack", e.ToString(), true);

            }
            finally
            {
                dtrDB.CloseDB();
            }
            return dt;
        }

        public DataTable dtGrveShift(DateTime ProcessDate, String LedgerDBName, String LedgerCompanyCode, string CentralProfile)
        {
            double gap = -15.0;//pocket gap
            string query = string.Format(@"SELECT 
                                                Ttr_Date AS 'PROCESSDATE'
                                                ,Ttr_ShiftCode AS 'SHIFT'
                                                ,Tel_IDNo
                                                ,Ttr_PayCycle AS PayPeriod
                                                ,_DTR.Usr_Login AS Usr_Login
                                                ,(CONVERT(CHAR(10),Tel_LogDate,101) + ' ' + Substring(Tel_LogTime, 1, 2) + ':' + Substring(Tel_LogTime, 3, 2)) AS 'Tel_Log'
                                                ,Tel_LogType
                                                ,(SELECT MAX((CONVERT(CHAR(10),Tel_LogDate,101) + ' ' + Substring(Tel_LogTime, 1, 2) + ':' + Substring(Tel_LogTime, 3, 2))) FROM {0}..T_EmpDTR
                                                     WHERE Tel_LogType='O' 
                                                       AND Tel_IDNo=_DTR.Tel_IDNo 
                                                       AND CONVERT(DATETIME,Tel_LogDate) = @START) AS 'PREV MAX OUT'
                                                ,(SELECT MAX((CONVERT(CHAR(10),Tel_LogDate,101) + ' ' + Substring(Tel_LogTime, 1, 2) + ':' + Substring(Tel_LogTime, 3, 2))) FROM {0}..T_EmpDTR 
                                                     WHERE Tel_LogType='I' 
                                                       AND Tel_IDNo=_DTR.Tel_IDNo 
                                                       AND CONVERT(DATETIME,Tel_LogDate) = @END) AS 'CUR MAX IN'
           
                                                ,(SELECT MAX((CONVERT(CHAR(10),Tel_LogDate,101) + ' ' + Substring(Tel_LogTime, 1, 2) + ':' + Substring(Tel_LogTime, 3, 2))) FROM {0}..T_EmpDTR
                                                     WHERE Tel_LogType='O' 
                                                       AND Tel_IDNo=_DTR.Tel_IDNo 
                                                       AND Tel_LogTime<Msh_ShiftIn1
                                                       AND CONVERT(DATETIME,Tel_LogDate) = @END) AS 'LAST OUT'

                                                ,DATEADD(MINUTE, @GAP,CONVERT(DATETIME,(SELECT CONVERT(VARCHAR,@END,101) + ' ' + Substring(Msh_ShiftIn1, 1, 2) + ':' + Substring(Msh_ShiftIn1, 3, 2) 
                                                    FROM {3}..M_Shift 
		                                            WHERE Msh_CompanyCode = '{2}'
                                                    AND LEDGER.Ttr_ShiftCode = Msh_ShiftCode))) AS 'SHIFT IN'
	
                                           FROM {1}..T_EmpTimeRegister LEDGER
                                           RIGHT JOIN {0}..T_EmpDTR _DTR
                                                ON CONVERT(DATE,LEDGER.Ttr_Date) = _DTR.Tel_LogDate
                                                AND LEDGER.Ttr_IDNo = _DTR.Tel_IDNo
                                           JOIN {3}..M_Shift ON Ttr_ShiftCode = Msh_ShiftCode
                                                AND Msh_ShiftIn1 > Msh_ShiftOut2
                                                AND Msh_CompanyCode = '{2}'
                                            WHERE Ttr_Date BETWEEN @START AND @END", _dtrDBName, LedgerDBName, LedgerCompanyCode, CentralProfile);


            DataTable dt = new DataTable();
            ParameterInfo[] paramInfo = new ParameterInfo[3];
            paramInfo[0] = new ParameterInfo("@END", ProcessDate.ToShortDateString(), SqlDbType.DateTime);
            paramInfo[1] = new ParameterInfo("@START", ProcessDate.AddDays(-1).ToShortDateString(), SqlDbType.DateTime);
            paramInfo[2] = new ParameterInfo("@GAP", gap, SqlDbType.Float);


            try
            {
                dtrDB.OpenDB();
                //dtrDB.BeginTransaction();
                dt = dtrDB.ExecuteDataSet(query, CommandType.Text, paramInfo).Tables[0];
                //dtrDB.CommitTransaction();

                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    DateTime Pmax = (string.IsNullOrEmpty(dt.Rows[i]["PREV MAX OUT"].ToString())) ? DateTime.MinValue : (Convert.ToDateTime(dt.Rows[i]["PREV MAX OUT"]));
                    DateTime CMin = (string.IsNullOrEmpty(dt.Rows[i]["CUR MAX IN"].ToString())) ? DateTime.MinValue : (Convert.ToDateTime(dt.Rows[i]["CUR MAX IN"]));
                    DateTime Log = (string.IsNullOrEmpty(dt.Rows[i]["Dtr_Log"].ToString())) ? DateTime.MinValue : (Convert.ToDateTime(dt.Rows[i]["Tel_Log"]));
                    dt.Rows[i]["PREV MAX OUT"] = Pmax;
                    dt.Rows[i]["CUR MAX IN"] = CMin;
                    dt.Rows[i]["Tel_Log"] = Log;

                    if (Log == Convert.ToDateTime(dt.Rows[i]["CUR MAX IN"]))
                    {
                        if ((i + 1) == dt.Rows.Count || (dt.Rows[i + 1]["Tel_IDNo"].ToString() != dt.Rows[i]["Tel_IDNo"].ToString()))
                            dt.Rows[i].Delete();

                    }
                    else if (Log <= Pmax || Log > Convert.ToDateTime(dt.Rows[i]["SHIFT IN"]))
                    {
                        dt.Rows[i].Delete();
                    }
                    else
                        dt.Rows[i]["PROCESSDATE"] = ProcessDate.AddDays(-1);
                }
            }
            catch (Exception e)
            {
                //dtrDB.RollBackTransaction();
                _log.WriteLog(Application.StartupPath, "MultipocketPosting for graveyard shift", " : RollBack", e.ToString(), true);

            }
            finally
            {
                dtrDB.CloseDB();
            }



            return dt;
        }
        //*********************************************
        //
        //  Multipacket Pairing Function for reg shift
        //  04/12/2012
        //
        //*********************************************
        public List<Pocket> PocketPairing(DataTable dt)
        {
            List<Pocket> listMultiPocket = new List<Pocket>();

            Pocket _pocket = new Pocket();
            //start looping per logs assigning to what packet
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i].RowState == DataRowState.Deleted)
                    continue;

                _pocket.EmpID = dt.Rows[i]["Tel_IDNo"].ToString().Trim();
                _pocket.ProcessDate = Convert.ToDateTime(dt.Rows[i]["PROCESSDATE"]);
                _pocket.PayPeriod = dt.Rows[i]["PayPeriod"].ToString().Trim();
                _pocket.usr_login = dt.Rows[i]["Usr_Login"].ToString().Trim();
                _pocket.ShiftCode = dt.Rows[i]["SHIFT"].ToString().Trim();

                DateTime EmpLog = Convert.ToDateTime(dt.Rows[i]["Tel_Log"]);

                //if first occurence of dtr
                if (i == 0)
                {
                    if (dt.Rows[i]["Tel_LogType"].ToString().Trim().Equals("I"))
                    {
                        _pocket.IN01 = EmpLog;
                    }
                    else
                    {
                        _pocket.OUT01 = EmpLog;
                    }
                }
                else if (i != 0)
                {

                    if (dt.Rows[i]["Tel_LogType"].ToString().Trim().Equals("I"))
                    {
                        if (i != dt.Rows.Count - 1)
                        {
                            #region process IN Packet
                            if (dt.Rows[i + 1].RowState == DataRowState.Deleted)
                                continue;
                            DateTime EmpLogwithGAP = Convert.ToDateTime(dt.Rows[i + 1]["Tel_Log"]);
                            
                            //bool check = DateTime.Compare(EmpLogwithGAP, EmpLog.AddMinutes(pocketGAP)) >= 0;
                            if ((dt.Rows[i + 1]["Tel_LogType"].ToString().Trim().Equals('I') && DateTime.Compare(EmpLogwithGAP, EmpLog.AddMinutes(pocketGAP)) >= 0)) //|| (dt.Rows[i + 1]["Dtr_LogType"].ToString().Trim().Equals("O") && DateTime.Compare(EmpLogwithGAP, EmpLog.AddMinutes(pocketGAP)) >= 0))
                                continue;
                            else
                            {
                                //check sequentially if IN is empty and 
                                if (_pocket.IN01.Equals(DateTime.MinValue) && (_pocket.OUT01.Equals(DateTime.MinValue) && DateTime.Compare(EmpLogwithGAP, EmpLog.AddMinutes(pocketGAP)) >= 0)) // && dt.Rows[i + 1]["Dtr_LogType"].ToString().Trim().Equals("O"))
                                {
                                    _pocket.IN01 = EmpLog;
                                }
                                else if (_pocket.IN02.Equals(DateTime.MinValue) && (_pocket.OUT02.Equals(DateTime.MinValue) && DateTime.Compare(EmpLogwithGAP, EmpLog.AddMinutes(pocketGAP)) >= 0))// && dt.Rows[i + 1]["Dtr_LogType"].ToString().Trim().Equals("O"))
                                {
                                    _pocket.IN02 = EmpLog;
                                }
                                else if (_pocket.IN03.Equals(DateTime.MinValue) && (_pocket.OUT03.Equals(DateTime.MinValue) && DateTime.Compare(EmpLogwithGAP, EmpLog.AddMinutes(pocketGAP)) >= 0))// && dt.Rows[i + 1]["Dtr_LogType"].ToString().Trim().Equals("O"))
                                {
                                    _pocket.IN03 = EmpLog;
                                }
                                else if (_pocket.IN04.Equals(DateTime.MinValue) && (_pocket.OUT04.Equals(DateTime.MinValue) && DateTime.Compare(EmpLogwithGAP, EmpLog.AddMinutes(pocketGAP)) >= 0))// && dt.Rows[i + 1]["Dtr_LogType"].ToString().Trim().Equals("O"))
                                {
                                    _pocket.IN04 = EmpLog;
                                }
                                else if (_pocket.IN05.Equals(DateTime.MinValue) && (_pocket.OUT05.Equals(DateTime.MinValue) && DateTime.Compare(EmpLogwithGAP, EmpLog.AddMinutes(pocketGAP)) >= 0))// && dt.Rows[i + 1]["Dtr_LogType"].ToString().Trim().Equals("O"))
                                {
                                    _pocket.IN05 = EmpLog;
                                }
                                else if (_pocket.IN06.Equals(DateTime.MinValue) && (_pocket.OUT06.Equals(DateTime.MinValue) && DateTime.Compare(EmpLogwithGAP, EmpLog.AddMinutes(pocketGAP)) >= 0))// && dt.Rows[i + 1]["Dtr_LogType"].ToString().Trim().Equals("O"))
                                {
                                    _pocket.IN06 = EmpLog;
                                }
                                else if (_pocket.IN07.Equals(DateTime.MinValue) && (_pocket.OUT07.Equals(DateTime.MinValue) && DateTime.Compare(EmpLogwithGAP, EmpLog.AddMinutes(pocketGAP)) >= 0))// && dt.Rows[i + 1]["Dtr_LogType"].ToString().Trim().Equals("O"))
                                {
                                    _pocket.IN07 = EmpLog;
                                }
                                else if (_pocket.IN08.Equals(DateTime.MinValue) && (_pocket.OUT08.Equals(DateTime.MinValue) && DateTime.Compare(EmpLogwithGAP, EmpLog.AddMinutes(pocketGAP)) >= 0))// && dt.Rows[i + 1]["Dtr_LogType"].ToString().Trim().Equals("O"))
                                {
                                    _pocket.IN08 = EmpLog;
                                }
                                else if (_pocket.IN09.Equals(DateTime.MinValue) && (_pocket.OUT09.Equals(DateTime.MinValue) && DateTime.Compare(EmpLogwithGAP, EmpLog.AddMinutes(pocketGAP)) >= 0))// && dt.Rows[i + 1]["Dtr_LogType"].ToString().Trim().Equals("O"))
                                {
                                    _pocket.IN09 = EmpLog;
                                }
                                else if (_pocket.IN10.Equals(DateTime.MinValue) && (_pocket.OUT09 != DateTime.MinValue))
                                {
                                    _pocket.IN10 = EmpLog;
                                }
                            }
                            #endregion
                        }
                    }
                    else
                    {
                        #region Process OUT Packets
                        if (_pocket.OUT01 == DateTime.MinValue && (_pocket.IN01 != DateTime.MinValue && DateTime.Compare(EmpLog, _pocket.IN01.AddMinutes(pocketGAP)) > 0) && dt.Rows[i - 1]["Tel_LogType"].ToString().Trim().Equals("I"))
                        {
                            _pocket.OUT01 = EmpLog;
                        }
                        else if (_pocket.OUT02.Equals(DateTime.MinValue) && (_pocket.IN02 != DateTime.MinValue && DateTime.Compare(EmpLog, _pocket.IN02.AddMinutes(pocketGAP)) > 0) && dt.Rows[i - 1]["Tel_LogType"].ToString().Trim().Equals("I"))
                        {
                            _pocket.OUT02 = EmpLog;
                        }
                        else if (_pocket.OUT03.Equals(DateTime.MinValue) && (_pocket.IN03 != DateTime.MinValue && DateTime.Compare(EmpLog, _pocket.IN03.AddMinutes(pocketGAP)) > 0) && dt.Rows[i - 1]["Tel_LogType"].ToString().Trim().Equals("I"))
                        {
                            _pocket.OUT03 = EmpLog;
                        }
                        else if (_pocket.OUT04.Equals(DateTime.MinValue) && (_pocket.IN04 != DateTime.MinValue && DateTime.Compare(EmpLog, _pocket.IN04.AddMinutes(pocketGAP)) > 0) && dt.Rows[i - 1]["Tel_LogType"].ToString().Trim().Equals("I"))
                        {
                            _pocket.OUT04 = EmpLog;
                        }
                        else if (_pocket.OUT05.Equals(DateTime.MinValue) && (_pocket.IN05 != DateTime.MinValue && DateTime.Compare(EmpLog, _pocket.IN05.AddMinutes(pocketGAP)) > 0) && dt.Rows[i - 1]["Tel_LogType"].ToString().Trim().Equals("I"))
                        {
                            _pocket.OUT05 = EmpLog;
                        }
                        else if (_pocket.OUT06.Equals(DateTime.MinValue) && (_pocket.IN06 != DateTime.MinValue && DateTime.Compare(EmpLog, _pocket.IN06.AddMinutes(pocketGAP)) > 0) && dt.Rows[i - 1]["Tel_LogType"].ToString().Trim().Equals("I"))
                        {
                            _pocket.OUT06 = EmpLog;
                        }
                        else if (_pocket.OUT07.Equals(DateTime.MinValue) && (_pocket.IN07 != DateTime.MinValue && DateTime.Compare(EmpLog, _pocket.IN07.AddMinutes(pocketGAP)) > 0) && dt.Rows[i - 1]["Tel_LogType"].ToString().Trim().Equals("I"))
                        {
                            _pocket.OUT07 = EmpLog;
                        }
                        else if (_pocket.OUT08.Equals(DateTime.MinValue) && (_pocket.IN08 != DateTime.MinValue && DateTime.Compare(EmpLog, _pocket.IN08.AddMinutes(pocketGAP)) > 0) && dt.Rows[i - 1]["Tel_LogType"].ToString().Trim().Equals("I"))
                        {
                            _pocket.OUT08 = EmpLog;
                        }
                        else if (_pocket.OUT09.Equals(DateTime.MinValue) && (_pocket.IN09 != DateTime.MinValue && DateTime.Compare(EmpLog, _pocket.IN09.AddMinutes(pocketGAP)) > 0) && dt.Rows[i - 1]["Tel_LogType"].ToString().Trim().Equals("I"))
                        {
                            _pocket.OUT09 = EmpLog;
                        }
                        else if (_pocket.OUT10.Equals(DateTime.MinValue) && (_pocket.IN09 != DateTime.MinValue))
                        {
                            _pocket.OUT10 = EmpLog;
                        }

                        #endregion
                    }
                }

                if (i + 1 < dt.Rows.Count)
                {

                    if (dt.Rows[i + 1].RowState == DataRowState.Deleted)
                    {
                        listMultiPocket.Add(_pocket);
                        _pocket = new Pocket();
                        continue;
                    }

                    else if (_pocket.EmpID != dt.Rows[i + 1]["Tel_IDNo"].ToString().Trim())
                    {
                        listMultiPocket.Add(_pocket);
                        _pocket = new Pocket();
                    }//Add row of packet if list is the last one

                }
                else if (i == dt.Rows.Count - 1)
                {
                    listMultiPocket.Add(_pocket);
                    _pocket = new Pocket();
                }

            }
            return listMultiPocket;
        }

        public List<Pocket> PocketPairingWOLogType(DataTable dt)
        {
            List<Pocket> listMultiPocket = new List<Pocket>();

            Pocket _pocket = new Pocket();
            //start looping per logs assigning to what packet
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i].RowState == DataRowState.Deleted)
                    continue;

                _pocket.EmpID = dt.Rows[i]["Tel_IDNo"].ToString().Trim();
                _pocket.ProcessDate = Convert.ToDateTime(dt.Rows[i]["PROCESSDATE"]);
                _pocket.PayPeriod = dt.Rows[i]["PayPeriod"].ToString().Trim();
                _pocket.usr_login = dt.Rows[i]["Usr_Login"].ToString().Trim();
                _pocket.ShiftCode = dt.Rows[i]["SHIFT"].ToString().Trim();

                DateTime EmpLog = Convert.ToDateTime(dt.Rows[i]["Tel_Log"]);

                /*
                DateTime.Compare(d1,d2):
                < 0 = d1 is earlier than d2.
                  0 = d1 is the same as d2.
                > 0 = d1 is later than d2.
                */

                //if first occurence of dtr
                if (i == 0)
                    _pocket.IN01 = EmpLog;
                else if (i != 0)
                {
                    if (_pocket.OUT01 == DateTime.MinValue && (_pocket.IN01 != DateTime.MinValue && DateTime.Compare(EmpLog, _pocket.IN01.AddMinutes(MultiPocketVar.POCKETTIME)) > 0))
                    {
                        _pocket.OUT01 = EmpLog;
                    }
                    else if (_pocket.IN02 == DateTime.MinValue && (_pocket.OUT01 != DateTime.MinValue && DateTime.Compare(EmpLog, _pocket.OUT01.AddMinutes(MultiPocketVar.POCKETGAP)) > 0))
                    {
                        _pocket.IN02 = EmpLog;
                    }
                    else if (_pocket.OUT02 == DateTime.MinValue && (_pocket.IN02 != DateTime.MinValue && DateTime.Compare(EmpLog, _pocket.IN02.AddMinutes(MultiPocketVar.POCKETTIME)) > 0))
                    {
                        _pocket.OUT02 = EmpLog;
                    }
                    else if (_pocket.IN03 == DateTime.MinValue && (_pocket.OUT02 != DateTime.MinValue && DateTime.Compare(EmpLog, _pocket.OUT02.AddMinutes(MultiPocketVar.POCKETGAP)) > 0))
                    {
                        _pocket.IN03 = EmpLog;
                    }
                    else if (_pocket.OUT03 == DateTime.MinValue && (_pocket.IN03 != DateTime.MinValue && DateTime.Compare(EmpLog, _pocket.IN03.AddMinutes(MultiPocketVar.POCKETTIME)) > 0))
                    {
                        _pocket.OUT03 = EmpLog;
                    }
                    else if (_pocket.IN04 == DateTime.MinValue && (_pocket.OUT03 != DateTime.MinValue && DateTime.Compare(EmpLog, _pocket.OUT03.AddMinutes(MultiPocketVar.POCKETGAP)) > 0))
                    {
                        _pocket.IN04 = EmpLog;
                    }
                    else if (_pocket.OUT04 == DateTime.MinValue && (_pocket.IN04 != DateTime.MinValue && DateTime.Compare(EmpLog, _pocket.IN04.AddMinutes(MultiPocketVar.POCKETTIME)) > 0))
                    {
                        _pocket.OUT04 = EmpLog;
                    }
                    else if (_pocket.IN05 == DateTime.MinValue && (_pocket.OUT04 != DateTime.MinValue && DateTime.Compare(EmpLog, _pocket.OUT04.AddMinutes(MultiPocketVar.POCKETGAP)) > 0))
                    {
                        _pocket.IN05 = EmpLog;
                    }
                    else if (_pocket.OUT05 == DateTime.MinValue && (_pocket.IN05 != DateTime.MinValue && DateTime.Compare(EmpLog, _pocket.IN05.AddMinutes(MultiPocketVar.POCKETTIME)) > 0))
                    {
                        _pocket.OUT05 = EmpLog;
                    }
                    else if (_pocket.IN06 == DateTime.MinValue && (_pocket.OUT05 != DateTime.MinValue && DateTime.Compare(EmpLog, _pocket.OUT05.AddMinutes(MultiPocketVar.POCKETGAP)) > 0))
                    {
                        _pocket.IN06 = EmpLog;
                    }
                    else if (_pocket.OUT06 == DateTime.MinValue && (_pocket.IN06 != DateTime.MinValue && DateTime.Compare(EmpLog, _pocket.IN06.AddMinutes(MultiPocketVar.POCKETTIME)) > 0))
                    {
                        _pocket.OUT06 = EmpLog;
                    }
                    else if (_pocket.IN07 == DateTime.MinValue && (_pocket.OUT06 != DateTime.MinValue && DateTime.Compare(EmpLog, _pocket.OUT06.AddMinutes(MultiPocketVar.POCKETGAP)) > 0))
                    {
                        _pocket.IN07 = EmpLog;
                    }
                    else if (_pocket.OUT07 == DateTime.MinValue && (_pocket.IN07 != DateTime.MinValue && DateTime.Compare(EmpLog, _pocket.IN07.AddMinutes(MultiPocketVar.ValidTimeGap)) > 0))
                    {
                        _pocket.OUT07 = EmpLog;
                    }
                    else if (_pocket.IN08 == DateTime.MinValue && (_pocket.OUT07 != DateTime.MinValue && DateTime.Compare(EmpLog, _pocket.OUT07.AddMinutes(MultiPocketVar.POCKETGAP)) > 0))
                    {
                        _pocket.IN08 = EmpLog;
                    }
                    else if (_pocket.OUT08 == DateTime.MinValue && (_pocket.IN08 != DateTime.MinValue && DateTime.Compare(EmpLog, _pocket.IN08.AddMinutes(MultiPocketVar.POCKETTIME)) > 0))
                    {
                        _pocket.OUT08 = EmpLog;
                    }
                    else if (_pocket.IN09 == DateTime.MinValue && (_pocket.OUT08 != DateTime.MinValue && DateTime.Compare(EmpLog, _pocket.OUT08.AddMinutes(MultiPocketVar.POCKETGAP)) > 0))
                    {
                        _pocket.IN09 = EmpLog;
                    }
                    else if (_pocket.OUT09 == DateTime.MinValue && (_pocket.IN09 != DateTime.MinValue && DateTime.Compare(EmpLog, _pocket.IN09.AddMinutes(MultiPocketVar.ValidTimeGap)) > 0))
                    {
                        _pocket.OUT09 = EmpLog;
                    }
                    else if (_pocket.IN10 == DateTime.MinValue && (_pocket.OUT09 != DateTime.MinValue && DateTime.Compare(EmpLog, _pocket.OUT09.AddMinutes(MultiPocketVar.POCKETGAP)) > 0))
                    {
                        _pocket.IN10 = EmpLog;
                    }
                    else if (_pocket.OUT10 == DateTime.MinValue && (_pocket.IN10 != DateTime.MinValue && DateTime.Compare(EmpLog, _pocket.IN10.AddMinutes(MultiPocketVar.POCKETTIME)) > 0))
                    {
                        _pocket.OUT10 = EmpLog;
                    }
                    else if (_pocket.IN11 == DateTime.MinValue && (_pocket.OUT10 != DateTime.MinValue && DateTime.Compare(EmpLog, _pocket.OUT10.AddMinutes(MultiPocketVar.POCKETGAP)) > 0))
                    {
                        _pocket.IN11 = EmpLog;
                    }
                    else if (_pocket.OUT11 == DateTime.MinValue && (_pocket.IN11 != DateTime.MinValue && DateTime.Compare(EmpLog, _pocket.IN11.AddMinutes(MultiPocketVar.POCKETTIME)) > 0))
                    {
                        _pocket.OUT11 = EmpLog;
                    }
                    else if (_pocket.IN12 == DateTime.MinValue && (_pocket.OUT11 != DateTime.MinValue && DateTime.Compare(EmpLog, _pocket.OUT11.AddMinutes(MultiPocketVar.POCKETGAP)) > 0))
                    {
                        _pocket.IN12 = EmpLog;
                    }
                    else if (_pocket.OUT12 == DateTime.MinValue && (_pocket.IN12 != DateTime.MinValue && DateTime.Compare(EmpLog, _pocket.IN12.AddMinutes(MultiPocketVar.POCKETTIME)) > 0))
                    {
                        _pocket.OUT12 = EmpLog;
                    }
                }

                if (i + 1 < dt.Rows.Count)
                {

                    if (dt.Rows[i + 1].RowState == DataRowState.Deleted)
                    {
                        listMultiPocket.Add(_pocket);
                        _pocket = new Pocket();
                        continue;
                    }

                    else if (_pocket.EmpID != dt.Rows[i + 1]["Tel_IDNo"].ToString().Trim())
                    {
                        listMultiPocket.Add(_pocket);
                        _pocket = new Pocket();
                    }//Add row of packet if list is the last one

                }
                else if (i == dt.Rows.Count - 1)
                {
                    listMultiPocket.Add(_pocket);
                    _pocket = new Pocket();
                }

            }
            return listMultiPocket;
        }

        #endregion

        #endregion

    }

    #endregion

    #region Mutliple Pocket Variables

    public class MultiPocketVar
    {
        public static bool RegularShiftSched = false;
        public static bool IsOUTBeforeIN = true;
        public static string EmloyeeIDPrevDate = String.Empty;
        public static string EmployeeIDCurrentTOProcess = String.Empty;
        public static DateTime LogDateTimePrevDate;
        public static string ShiftCodePrevDate = String.Empty;
        public static string CapturedOutOccurence = String.Empty;
        public static bool IsFourPackets = true;
        public static double ValidTimeGap = 0;
        public static string LedgerDBName = String.Empty;
        public static string LedgerDBCompanyCode = String.Empty;
        public static bool FromTextFilePosting = false;
        public static bool isLedgerHist = false;
        public static bool setFILO = true;
        public static string T_Ledger = "T_EmpTimeRegister";
        public static string T_Overtime = "T_EmpOvertime";
        public static string T_DTR = "T_DTR";  // values can also be "T_DTR Differential"
        public static string NXTDUSER = "LOGUPLDONXTD";
        public static bool isFirstINLastOut = true;
        public static bool isServicePost = false;
        public static bool isOverwrite = true;
        public static bool isDENZOPOSTING = false;
        public static bool isCHPAUTOSHIFT = false;
        public static bool isOTAfterMidnight = false;
        public static string _ServiceCode;
        public static string MPCode = "MULTIPOCKETS";
        public static DataTable EmployeeStraightWorkList = new DataTable();
        public static double Progress = 0;
        public static bool isOverLoadingRetrieval = false;
        public static string ProgressProcess = string.Empty;
        public static int GenericCount = 0;
        public static int Count = 0;
        public static string Used_T_DTR = "T_EmpDTR";
        public static double POCKETTIME = 0;
        public static double POCKETGAP = 0;
        public static void SetLoguploadingGlobal()
        {
            try { isOTAfterMidnight = CommonProcedures.GetAppSettingConfigBool("OTAfterMidnight", false); }
            catch { }
            try { isDENZOPOSTING = CommonProcedures.GetAppSettingConfigBool("DENZOAutoShift", false); }
            catch { }
            try { isCHPAUTOSHIFT = CommonProcedures.GetAppSettingConfigBool("CPHAutoShift", false); }
            catch { }
        }

    }

    #endregion

    #region LogUploadingManager Class

    public class LogUploadingManager
    {
        #region Member Variables

        DALHelper dtrDB;
        string _dtrDBName = string.Empty;

        #endregion

        #region Public Properties
        LedgerExtforMultiplePocket extension = new LedgerExtforMultiplePocket();
        LedgerExtforMultiplePocket TempExtension = new LedgerExtforMultiplePocket();
        int affected = 0;
        public string LedgerDBName = string.Empty;
        public string LedgerDBCompanyCode = string.Empty;
        public string CentralProfile = string.Empty;
        public bool IsFILO = true;
        public bool FourPockets = true;
        public bool Plus24Hours = false;
        public List<EmployeeDTR> PostedDTR = new List<EmployeeDTR>();

        MultiPacketUploding _logUploading_v2 = new MultiPacketUploding("");
        NLLogger.Logger _log = new NLLogger.Logger();
        #endregion

        public LogUploadingManager(string dtrDBName)
        {
            dtrDB = new DALHelper(dtrDBName, false);
            _dtrDBName = dtrDBName;
        }

        #region Private Methods
        private List<EmployeeDTR> ConvertTextToDTR(string textFilePath)
        {
            List<EmployeeDTR> dtr = new List<EmployeeDTR>();

            if (File.Exists(textFilePath))
            {
                string line = string.Empty;
                string[] values;

                using (StreamReader file = new StreamReader(textFilePath))
                {
                    while ((line = file.ReadLine()) != null)
                    {
                        values = line.Split(',');
                        dtr.Add(new EmployeeDTR());
                        dtr[dtr.Count - 1].EmployeeID = values[0];
                        dtr[dtr.Count - 1].LogDateTime = Convert.ToDateTime(values[1] + " " + this.FormatTime(values[2]));
                        dtr[dtr.Count - 1].LogType = values[3] == "I" ? LogTypes.IN : LogTypes.OUT;
                        dtr[dtr.Count - 1] = GetDTRShift(dtr[dtr.Count - 1]);
                    }

                    file.Close();
                }
            }

            return dtr;
        }

        private void SaveDtr(EmployeeDTR dtr)
        {
            string sql = string.Format(@"IF NOT EXISTS (SELECT 1 FROM {0}..T_EmpDTR WHERE Tel_IDNo = @EmployeeID AND Tel_LogDate = @LogDate AND Tel_LogTime = @LogTime AND Tel_LogType = @LogType)
                                            BEGIN
                                                INSERT INTO {0}..T_EmpDTR
                                                VALUES(@EmployeeID, @LogDate, @LogTime, @LogType, '00', false, 'SERVICE', GETDATE(), false)
                                            END", _dtrDBName);

            ParameterInfo[] paramInfo = new ParameterInfo[4];

            paramInfo[0] = new ParameterInfo("@EmployeeID", dtr.EmployeeID);
            paramInfo[1] = new ParameterInfo("@LogDate", dtr.LogDateTime.ToShortDateString(), SqlDbType.Date);
            paramInfo[2] = new ParameterInfo("@LogTime", FormatLedgerTime(dtr.LogDateTime, dtr.LogDateTime));
            paramInfo[3] = new ParameterInfo("@LogType", dtr.LogType.ToString().Substring(0, 1));

            try
            {
                dtrDB.OpenDB();
                //dtrDB.BeginTransaction();
                dtrDB.ExecuteNonQuery(sql, System.Data.CommandType.Text, paramInfo);
                //dtrDB.CommitTransaction();
            }
            catch
            {
                //dtrDB.RollBackTransaction();
            }
            finally
            {
                dtrDB.CloseDB();
            }
        }

        private EmployeeDTR GetDTRShift(EmployeeDTR dtr)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[3];
            string sql = string.Format(@"SELECT Ledger.Ttr_DayCode AS DayCode
	                                        , Ledger.Ttr_ShiftCode AS ShiftCode
	                                        , Ledger.Ttr_HolidayFlag AS Holiday
	                                        , Ledger.Ttr_RestDayFlag AS RestDay
	                                        , Shift.Msh_Schedule AS ScheduleType
	                                        , Shift.Msh_ShiftIn1 AS ShiftTimeIn
	                                        , Shift.Msh_ShiftOut1 AS ShiftBreakStart
	                                        , Shift.Msh_ShiftIn2 AS ShiftBreakEnd
	                                        , Shift.Msh_ShiftOut2 AS ShiftTimeOut
                                        FROM {0}..T_EmpTimeRegister AS Ledger
                                        INNER JOIN M_Shift AS Shift ON Ledger.Ttr_ShiftCode = Shift.Msh_ShiftCode
                                            AND Shift.Msh_CompanyCode = @CompanyCode
                                        WHERE Ledger.Ttr_IDNo = @IDNo
                                        AND Ledger..Ttr_Date = @LogDate", LedgerDBName);

            paramInfo[0] = new ParameterInfo("@IDNo", dtr.EmployeeID);
            paramInfo[1] = new ParameterInfo("@LogDate", Convert.ToDateTime(dtr.LogDateTime.ToShortDateString()));
            paramInfo[2] = new ParameterInfo("@CompanyCode", "");

            dtrDB.OpenDB();
            SqlDataReader dataReader = dtrDB.ExecuteReader(sql, CommandType.Text, paramInfo, CommandBehavior.Default);

            while (dataReader.Read())
            {
                dtr.DayCode = dataReader["DayCode"].ToString();
                dtr.ShiftCode = dataReader["ShiftCode"].ToString();
                dtr.IsHoliday = Convert.ToBoolean(dataReader["Holiday"]);
                dtr.IsRestDay = Convert.ToBoolean(dataReader["RestDay"]);
                dtr.ShiftTime.TimeIn = Convert.ToDateTime(dataReader["LogDate"].ToString() + " " + FormatTime(dataReader["ShiftTimeIn"].ToString()));
                dtr.ShiftBreak.TimeIn = Convert.ToDateTime(dataReader["LogDate"].ToString() + " " + FormatTime(dataReader["ShiftBreakStart"].ToString()));
                dtr.ShiftBreak.TimeOut = Convert.ToDateTime(dataReader["LogDate"].ToString() + " " + FormatTime(dataReader["ShiftBreakEnd"].ToString()));
                dtr.ShiftTime.TimeOut = Convert.ToDateTime(dataReader["LogDate"].ToString() + " " + FormatTime(dataReader["ShiftTimeOut"].ToString()));

                if (dtr.ShiftTime.TimeIn > dtr.ShiftBreak.TimeIn)
                {
                    dtr.ShiftBreak.TimeIn = dtr.ShiftBreak.TimeIn.AddDays(1);
                }

                if (dtr.ShiftTime.TimeIn > dtr.ShiftBreak.TimeOut)
                {
                    dtr.ShiftBreak.TimeOut = dtr.ShiftBreak.TimeOut.AddDays(1);
                }

                if (dtr.ShiftTime.TimeIn > dtr.ShiftTime.TimeOut)
                {
                    dtr.ShiftTime.TimeOut = dtr.ShiftTime.TimeOut.AddDays(1);
                }
            }

            return dtr;
        }

        private bool IsBetweenTimeRange(DateTime dateTimeToBeEvaluated, DateTime dateStart, DateTime dateEnd)
        {
            return (dateTimeToBeEvaluated >= dateStart && dateTimeToBeEvaluated <= dateEnd);
        }

        private bool IsTimeDefault(string logTime)
        {
            return (string.IsNullOrEmpty(logTime) || logTime == "0000");
        }

        private string FormatTime(string value)
        {
            int valueInt = Convert.ToInt32(value);

            if (valueInt >= 2400)
            {
                valueInt -= 2400;
                value = valueInt.ToString("0000");
            }
            else
            {
                value = string.Format("{0:0000}", valueInt);
            }

            return value.Substring(0, 2) + ":" + value.Substring(2, 2);
        }

        private string FormatLedgerTime(DateTime processDate, DateTime logDateTime)
        {
            return (string.Compare(processDate.ToShortDateString(), logDateTime.ToShortDateString()) == 0 ||
                logDateTime == DateTime.MinValue ? logDateTime.ToString("HHmm") : (Convert.ToInt32(logDateTime.ToString("HHmm")) + (Plus24Hours ? 2400 : 0)).ToString("0000"));
        }

        private string FormatLedgerTimeExt(DateTime processDate, DateTime logDateTime)
        {
            return (string.Compare(processDate.ToShortDateString(), logDateTime.ToShortDateString()) == 0 ||
                logDateTime == DateTime.MinValue ? logDateTime.ToString("HHmm") : (Convert.ToInt32(logDateTime.ToString("HHmm")) + ((Convert.ToDateTime(processDate.ToShortDateString()) < Convert.ToDateTime(logDateTime.ToShortDateString()) && MultiPocketVar.isOTAfterMidnight) ? 2400 : 0)).ToString("0000"));
        }

        private DateTime FormatActualLog(DateTime logDate, string logTime)
        {
            if (IsTimeDefault(logTime))
            {
                return DateTime.MinValue;
            }

            return ((Convert.ToInt32(logTime) > 2359) ?
                Convert.ToDateTime(logDate.AddDays(1).ToShortDateString() + " " + FormatTime((Convert.ToInt32(logTime) - 2400).ToString("0000"))) :
                Convert.ToDateTime(logDate.ToShortDateString() + " " + FormatTime(logTime)));
        }

        private LogTypes GetEquivalentLogType(string logType)
        {
            return (string.Compare(logType, "I", true) == 0 ? LogTypes.IN : LogTypes.OUT);
        }

        private EmployeeLedger SetUpLedger(EmployeeLedger ledger, EmployeeDTR dtr)
        {
            //Save2pocketstoELLext(ledger);
            return (IsFILO ? FirstInLastOut(ledger, dtr) : LastInFirstOut(ledger, dtr));
        }

        private EmployeeLedger FirstInLastOut(EmployeeLedger ledger, EmployeeDTR dtr)
        {
            #region Graveyard Checking

            bool isGraveYard = false;

            //FILO
            if (ledger.ShiftTime.TimeIn.ToShortDateString() != ledger.ShiftTime.TimeOut.ToShortDateString())
            {
                isGraveYard = true;
            }
            else if (MultiPocketVar.isDENZOPOSTING && MultiPocketVar.isFirstINLastOut)
            {
                //Force Exit, Denso uses min max.
                return ledger;
            }

            #endregion

            if (dtr.LogType == LogTypes.OUT)
            {
                #region Log Out

                //FOR TIME OUT 1
                if (dtr.LogDateTime <= ledger.ShiftBreak.TimeOut //Log out is less than break out
                 && dtr.LogDateTime > ledger.ShiftTime.TimeIn)   //Log out is greater than shift in
                {
                    if (!MultiPocketVar.isOverwrite && ledger.LogTime1.TimeOut != DateTime.MinValue)
                        return ledger;

                    //Time Gap Checking Before Posting
                    if (_logUploading_v2._isTimeGapValid(dtr.LogDateTime, ledger.LogTime1.TimeIn))
                    {
                        ledger.LogTime1.TimeOut = (ledger.LogTime1.TimeOut == DateTime.MinValue
                                                || (ledger.LogTime1.TimeOut < dtr.LogDateTime)
                            //Do not post Out 1 reposted with inverse shift logic for graveyard (HOYA)
                            //Do not post too early log out 1 if no Log In 1, This might be wrong shift only.
                                                 ? ((!MultiPocketVar.isFirstINLastOut && isGraveYard && ledger.LogTime1.TimeIn.Hour < 12 && ledger.LogTime2.TimeOut.Hour > 12) || ledger.LogTime1.TimeIn == DateTime.MinValue && !_logUploading_v2._isTimeGapValid(dtr.LogDateTime, ledger.ShiftTime.TimeIn)) ? ledger.LogTime1.TimeOut : dtr.LogDateTime
                                                 : ledger.LogTime1.TimeOut);
                    }
                }
                else if (dtr.LogDateTime > ledger.ShiftBreak.TimeIn) //Log out is greater then break In
                {
                    //FOR TIME OUT 2
                    if (!MultiPocketVar.isOverwrite && ledger.LogTime2.TimeOut != DateTime.MinValue)
                        return ledger;

                    //Remove IN2 instead of OUT2
                    if ((ledger.LogTime2.TimeIn > dtr.LogDateTime.AddMinutes(-MultiPocketVar.ValidTimeGap)
                       && ledger.ShiftTime.TimeOut.ToShortDateString() == dtr.LogDateTime.ToShortDateString()
                       && (ledger.LogTime2.TimeIn > ledger.ShiftTime.TimeOut.AddMinutes(-MultiPocketVar.ValidTimeGap)
                           || ledger.LogTime1.TimeOut == DateTime.MinValue))
                       && ledger.LogTime2.TimeIn != DateTime.MinValue
                       && !isGraveYard)
                        ledger.LogTime2.TimeIn = DateTime.MinValue;

                    //Time Gap Checking Before Posting
                    if (_logUploading_v2._isTimeGapValid(dtr.LogDateTime, ledger.LogTime2.TimeIn)
                     && _logUploading_v2._isTimeGapValid(dtr.LogDateTime, ledger.LogTime1.TimeIn))
                    {
                        ledger.LogTime2.TimeOut = ((ledger.LogTime2.TimeOut == DateTime.MinValue
                                                  || ledger.LogTime2.TimeOut < dtr.LogDateTime))
                                                  && ((!isGraveYard && dtr.LogDateTime.ToShortDateString() == ledger.ShiftTime.TimeIn.ToShortDateString()) || (isGraveYard && dtr.LogDateTime.Hour < 9))    //Allow only until 8:59 for grave yard out.
                                                  ? dtr.LogDateTime : ledger.LogTime2.TimeOut;
                    }
                }

                #endregion
            }
            else if (dtr.LogType == LogTypes.IN)
            {
                #region Log In

                if (dtr.LogDateTime < ledger.ShiftBreak.TimeIn && dtr.LogDateTime < ledger.ShiftTime.TimeOut)
                {
                    if (!MultiPocketVar.isOverwrite && ledger.LogTime1.TimeIn != DateTime.MinValue)
                    {
                        return ledger;
                    }
                    else
                    {

                        ledger.LogTime1.TimeIn = ((ledger.LogTime1.TimeIn == DateTime.MinValue
                                                 || ledger.LogTime1.TimeIn > dtr.LogDateTime)  //Ledger log in is greater than dtr log in
                                                 && ledger.ShiftTime.TimeIn.ToShortDateString() == dtr.LogDateTime.ToShortDateString() //Ledger data is same with dtr date
                                                 || (isGraveYard
                                                     && ledger.LogTime1.TimeIn < dtr.LogDateTime
                                                     && ledger.LogTime1.TimeIn.Hour < 12
                                                     && dtr.LogDateTime.Hour > 12) //Nilo added : For graveyard with multiple Time In on that date overwrite if the current IN is too early
                                                 ? ((isGraveYard && dtr.LogDateTime.Hour < 9) ? ledger.LogTime1.TimeIn : dtr.LogDateTime) : ledger.LogTime1.TimeIn); //Ledger is empty && Dtr Log Time Hour is not too early [9am]
                    }
                }
                else if (dtr.LogDateTime >= ledger.ShiftBreak.TimeIn && dtr.LogDateTime < ledger.ShiftTime.TimeOut)
                {
                    if (!MultiPocketVar.isOverwrite && ledger.LogTime2.TimeIn != DateTime.MinValue)
                        return ledger;

                    ledger.LogTime2.TimeIn = (ledger.LogTime2.TimeIn == DateTime.MinValue || ledger.LogTime2.TimeIn > dtr.LogDateTime ? dtr.LogDateTime : ledger.LogTime2.TimeIn);
                }

                #endregion
            }

            #region Log Leadger Cleaner
            if (!isGraveYard)
                ledger = _logUploading_v2.ErrLogRegShiftCleaner(ledger, ledger.ShiftBreak.TimeOut, ledger.ShiftTime.TimeOut);
            #endregion

            return ledger;
        }

        private EmployeeLedger LastInFirstOut(EmployeeLedger ledger, EmployeeDTR dtr)
        {
            #region Graveyard Checking

            bool isGraveYard = false;

            //LIFO
            if (ledger.ShiftTime.TimeIn.ToShortDateString() != ledger.ShiftTime.TimeOut.ToShortDateString())
            {
                isGraveYard = true;
            }
            else if (MultiPocketVar.isDENZOPOSTING && MultiPocketVar.isFirstINLastOut)
            {
                //Force Exit, Denso uses min max.
                return ledger;
            }

            #endregion

            if (dtr.LogType == LogTypes.OUT)
            {
                #region Log Out

                if (dtr.LogDateTime <= ledger.ShiftBreak.TimeOut //Log out is less than break out
                 && dtr.LogDateTime > ledger.ShiftTime.TimeIn)   //Log out is greater than shift in
                {
                    //FOR TIME OUT 1

                    if (!MultiPocketVar.isOverwrite && ledger.LogTime1.TimeOut != DateTime.MinValue)
                        return ledger;

                    //Time Gap Checking Before Posting
                    if (_logUploading_v2._isTimeGapValid(dtr.LogDateTime, ledger.LogTime1.TimeIn))
                    {
                        ledger.LogTime1.TimeOut = (ledger.LogTime1.TimeOut == DateTime.MinValue
                                                || (ledger.LogTime1.TimeOut > dtr.LogDateTime)
                            //(Do not post Out 1 reposted with inverse shift logic for graveyard (HOYA)) 
                            //Do not post too early log out 1 if no Log In 1, This might be wrong shift only.
                                                 ? ((!MultiPocketVar.isFirstINLastOut && isGraveYard && ledger.LogTime1.TimeIn.Hour < 12 && ledger.LogTime2.TimeOut.Hour > 12) || ledger.LogTime1.TimeIn == DateTime.MinValue && !_logUploading_v2._isTimeGapValid(dtr.LogDateTime, ledger.ShiftTime.TimeIn)) ? ledger.LogTime1.TimeOut : dtr.LogDateTime
                                                 : ledger.LogTime1.TimeOut);
                    }

                    #region Unpost Captured Out First Occurence
                    ledger.LogTime1.TimeOut = (!isGraveYard && (ledger.LogTime1.TimeOut < ledger.ShiftTime.TimeIn)
                                              ? ledger.LogTime1.TimeOut = DateTime.MinValue : ledger.LogTime1.TimeOut);
                    #endregion
                }
                else if (dtr.LogDateTime > ledger.ShiftBreak.TimeIn) //Log out is greater then break In
                {
                    //FOR TIME OUT 2

                    if (!MultiPocketVar.isOverwrite && ledger.LogTime2.TimeOut != DateTime.MinValue)
                        return ledger;

                    //Remove IN2 instead of OUT2
                    if ((ledger.LogTime2.TimeIn > dtr.LogDateTime.AddMinutes(-MultiPocketVar.ValidTimeGap)
                      && ledger.ShiftBreak.TimeOut.ToShortDateString() == dtr.LogDateTime.ToShortDateString()
                      && (ledger.LogTime2.TimeIn > ledger.ShiftTime.TimeOut.AddMinutes(-MultiPocketVar.ValidTimeGap)
                          || ledger.LogTime1.TimeOut == DateTime.MinValue))
                      && ledger.LogTime2.TimeIn != DateTime.MinValue
                      && !isGraveYard)
                    {
                        ledger.LogTime2.TimeIn = DateTime.MinValue;
                    }

                    //Time Gap Checking Before Posting
                    if (_logUploading_v2._isTimeGapValid(dtr.LogDateTime, ledger.LogTime2.TimeIn)
                     && _logUploading_v2._isTimeGapValid(dtr.LogDateTime, ledger.LogTime1.TimeIn))
                    {
                        ledger.LogTime2.TimeOut = ((ledger.LogTime2.TimeOut == DateTime.MinValue
                                                  || ledger.LogTime2.TimeOut > dtr.LogDateTime))
                                                  && ((!isGraveYard && dtr.LogDateTime.ToShortDateString() == ledger.ShiftTime.TimeIn.ToShortDateString()) || (isGraveYard && dtr.LogDateTime.Hour < 9))    //Allow only until 8:59 for grave yard out.
                                                   ? dtr.LogDateTime : ledger.LogTime2.TimeOut;
                    }
                }

                #endregion
            }
            else if (dtr.LogType == LogTypes.IN)
            {
                #region Log In

                if (dtr.LogDateTime < ledger.ShiftBreak.TimeIn && dtr.LogDateTime < ledger.ShiftTime.TimeOut)
                {
                    //FOR IN 1

                    if (!MultiPocketVar.isOverwrite && ledger.LogTime1.TimeIn != DateTime.MinValue)
                        return ledger;

                    ledger.LogTime1.TimeIn = (ledger.LogTime1.TimeIn == DateTime.MinValue  //Ledger is empty
                                                || (ledger.LogTime1.TimeIn < dtr.LogDateTime) //Ledger log in is less than dtr log in
                                                && ledger.ShiftTime.TimeIn.ToShortDateString() == dtr.LogDateTime.ToShortDateString() //Ledger data is same with dtr date
                                                || (isGraveYard
                                                    && ledger.LogTime1.TimeIn < dtr.LogDateTime
                                                    && ledger.LogTime1.TimeIn.Hour < 12
                                                    && dtr.LogDateTime.Hour > 12) //Nilo added : For graveyard with multiple Time In on that date overwrite if the current IN is too early
                                                ? ((isGraveYard && dtr.LogDateTime.Hour < 9) ? ledger.LogTime1.TimeIn : dtr.LogDateTime) : ledger.LogTime1.TimeIn); //Ledger is empty && Dtr Log Time Hour is not too early [9am]
                }
                else if (dtr.LogDateTime >= ledger.ShiftBreak.TimeIn && dtr.LogDateTime < ledger.ShiftTime.TimeOut)
                {
                    //FOR IN 2

                    if (!MultiPocketVar.isOverwrite && ledger.LogTime2.TimeIn != DateTime.MinValue)
                        return ledger;
                    ledger.LogTime2.TimeIn = (ledger.LogTime2.TimeIn == DateTime.MinValue || (ledger.LogTime1.TimeIn < dtr.LogDateTime) ? dtr.LogDateTime : ledger.LogTime2.TimeIn);
                }

                #endregion
            }

            #region Log Leadger Cleaner
            if (!isGraveYard)
                ledger = _logUploading_v2.ErrLogRegShiftCleaner(ledger, ledger.ShiftBreak.TimeOut, ledger.ShiftTime.TimeOut);
            #endregion

            return ledger;
        }

        private EmployeeLedger PostInLedgerExtension(EmployeeLedger ledger, EmployeeDTR dtr)
        {
            int extIndex = ledger.LedgerExtension.Count - 1;

            if (dtr.LogType == LogTypes.OUT && extIndex > -1)
            {
                ledger.LedgerExtension[extIndex].LogTime.TimeOut = dtr.LogDateTime;
                ledger.LedgerExtension[extIndex].RecordType = (ledger.LedgerExtension[extIndex].RecordType == RecordType.RETRIEVED ? RecordType.UPDATED : RecordType.NEW);
            }
            else
            {
                if (extIndex < 0 || ledger.LedgerExtension[extIndex].LogTime.TimeOut != DateTime.MinValue)
                {
                    LedgerExtension extension = new LedgerExtension();

                    if (dtr.LogType == LogTypes.IN)
                    {
                        extension.LogTime.TimeIn = dtr.LogDateTime;
                    }
                    else if (dtr.LogType == LogTypes.OUT)
                    {
                        extension.LogTime.TimeOut = dtr.LogDateTime;
                    }

                    extension.SequenceNumber = extIndex + 2;
                    extension.EmployeeID = dtr.EmployeeID;
                    extension.PayPeriod = ledger.PayPeriod;
                    extension.ProcessDate = ledger.ProcessDate;
                    extension.RecordType = RecordType.NEW;

                    ledger.LedgerExtension.Add(extension);
                }
                else
                {
                    ledger.LedgerExtension[extIndex].LogTime.TimeIn = dtr.LogDateTime;
                    //TODO: Posting of RecordType
                    ledger.LedgerExtension[extIndex].RecordType = (ledger.LedgerExtension[extIndex].RecordType == RecordType.RETRIEVED ? RecordType.UPDATED : RecordType.NEW);
                }
            }

            return ledger;
        }

        private void StorePostedDTR(EmployeeDTR dtr)
        {
            if (!PostedDTR.Contains(dtr))
            {
                PostedDTR.Add(dtr);
            }
        }

        private void AddToDBProfiles(string dbName, ref List<string> dbProfiles)
        {
            if (!string.IsNullOrEmpty(dbName) && !dbProfiles.Contains(dbName))
            {
                dbProfiles.Add(dbName);
            }
        }

        private class pockets<T>
        {
            private T[] arr = new T[20];
            public T this[int i]
            {
                get { return arr[i]; }
                set { arr[i] = value; }
            }
        }


        #endregion

        #region DTR Retrieval

        public List<EmployeeDTR> GetEmployeesDTR()
        {
            return GetEmployeesDTR(string.Empty);
        }

        public List<EmployeeDTR> GetEmployeesDTR(string employeeID)
        {
            return GetEmployeesDTR(employeeID, DateTime.MinValue);
        }

        public List<EmployeeDTR> GetEmployeesDTR(DateTime logDate)
        {
            return GetEmployeesDTR(string.Empty, logDate);
        }

        public List<EmployeeDTR> GetEmployeesDTR(string employeeID, DateTime logDate)
        {
            return GetEmployeesDTR(employeeID, string.Empty, logDate);
        }

        public List<EmployeeDTR> GetEmployeesDTR(DateTime logDateStart, DateTime logDateEnd)
        {
            return GetEmployeesDTR(string.Empty, string.Empty, logDateStart, logDateEnd);
        }

        public List<EmployeeDTR> GetEmployeesDTR(string employeeID, string shiftCode, DateTime logDate)
        {
            return GetEmployeesDTR(employeeID, shiftCode, logDate, DateTime.MinValue);
        }

        public List<EmployeeDTR> GetEmployeesDTR(string employeeID, string shiftCode, DateTime logDateStart, DateTime logDateEnd)
        {
            List<EmployeeDTR> dtrList = new List<EmployeeDTR>();
            ParameterInfo[] paramInfo = new ParameterInfo[4];

            if (string.IsNullOrEmpty(employeeID))
            {
                paramInfo[0] = new ParameterInfo("@EmployeeID", DBNull.Value);
            }
            else
            {
                paramInfo[0] = new ParameterInfo("@EmployeeID", employeeID);
            }

            if (logDateStart == DateTime.MinValue)
            {
                paramInfo[1] = new ParameterInfo("@LogDateStart", DBNull.Value);
                paramInfo[2] = new ParameterInfo("@LogDateEnd", DBNull.Value);
            }
            else if (logDateEnd == DateTime.MinValue)
            {
                logDateEnd = logDateStart;
                logDateStart = logDateStart.AddDays(-1);

                paramInfo[1] = new ParameterInfo("@LogDateStart", logDateStart);
                paramInfo[2] = new ParameterInfo("@LogDateEnd", logDateEnd);
            }

            if (string.IsNullOrEmpty(shiftCode))
            {
                paramInfo[3] = new ParameterInfo("@ShiftCode", DBNull.Value);
            }
            else
            {
                paramInfo[3] = new ParameterInfo("@ShiftCode", shiftCode);
            }
            string sql = "";

            if (MultiPocketVar.FromTextFilePosting)
            {
                #region text posting
                sql = string.Format(@"SELECT dtr.Tel_IDNo AS EmployeeID
	                                    , dtr.Tel_LogDate AS LogDate
	                                    , dtr.Tel_LogTime AS LogTime
	                                    , dtr.Dtr_LogType AS LogType
                                        , dtr.Tel_IsPosted AS PostFlag
	                                    , Ledger.Ttr_DayCode AS DayCode
	                                    , Ledger.Ttr_ShiftCode AS ShiftCode
	                                    , Ledger.Ttr_HolidayFlag AS Holiday
	                                    , Ledger.Ttr_RestDayFlag AS RestDay
	                                    , Shift.Msh_Schedule AS ScheduleType
	                                    , Shift.Msh_ShiftIn1 AS ShiftTimeIn
	                                    , Shift.Msh_ShiftOut1 AS ShiftBreakStart
	                                    , Shift.Msh_ShiftIn2 AS ShiftBreakEnd
	                                    , Shift.Msh_ShiftOut2 AS ShiftTimeOut
                                    FROM {1}..T_DTRDifferential AS dtr
                                    INNER JOIN {0}..{2} AS Ledger ON dtr.Tel_IDNo = Ledger.Ttr_IDNo
                                        AND (dtr.Tel_IDNo = @EmployeeID OR @EmployeeID IS NULL)
                                        AND CAST(dtr.Tel_LogDate AS DateTime) = Ledger.Ttr_Date
                                        AND ((@LogDateStart IS NULL AND @LogDateEnd IS NULL) OR (Ledger.Ttr_Date BETWEEN @LogDateStart AND @LogDateEnd))
                                    INNER JOIN {4}..M_Shift AS Shift ON Ledger.Ttr_ShiftCode = Shift.Msh_ShiftCode
                                        AND (Shift.Msh_ShiftCode = @ShiftCode OR @ShiftCode IS NULL)
                                        AND Shift.Msh_CompanyCode = '{3}'
                                    WHERE dtr.Tel_IsPosted != 1
                                    ORDER BY dtr.Tel_IDNo, CAST(dtr.Tel_LogDate + ' ' + Substring(dtr.Tel_LogTime, 1, 2) + ':' + Substring(dtr.Tel_LogTime, 3, 2) AS DateTime), dtr.LudateTime", LedgerDBName, _dtrDBName, MultiPocketVar.T_Ledger, LedgerDBCompanyCode, CentralProfile);
                #endregion
            }
            else
            {
                #region db posting
                sql = string.Format(@"SELECT dtr.Tel_IDNo AS EmployeeID
	                                    , dtr.Tel_LogDate AS LogDate
	                                    , dtr.Tel_LogTime AS LogTime
	                                    , dtr.Dtr_LogType AS LogType
                                        , dtr.Tel_IsPosted AS PostFlag
	                                    , Ledger.Ttr_DayCode AS DayCode
	                                    , Ledger.Ttr_ShiftCode AS ShiftCode
	                                    , Ledger.Ttr_HolidayFlag AS Holiday
	                                    , Ledger.Ttr_RestDayFlag AS RestDay
	                                    , Shift.Msh_Schedule AS ScheduleType
	                                    , Shift.Msh_ShiftIn1 AS ShiftTimeIn
	                                    , Shift.Msh_ShiftOut1 AS ShiftBreakStart
	                                    , Shift.Msh_ShiftIn2 AS ShiftBreakEnd
	                                    , Shift.Msh_ShiftOut2 AS ShiftTimeOut
                                    FROM {1}..T_EmpDTR AS dtr
                                    INNER JOIN {0}..{2} AS Ledger ON dtr.Tel_IDNo = Ledger.Ttr_IDNo
                                        AND (dtr.Tel_IDNo = @EmployeeID OR @EmployeeID IS NULL)
                                        AND dtr.Tel_LogDate = CONVERT(DATE,Ledger.Ttr_Date)
                                        AND ((@LogDateStart IS NULL AND @LogDateEnd IS NULL) OR (Ledger.Ttr_Date BETWEEN @LogDateStart AND @LogDateEnd))
                                    INNER JOIN {4}..M_Shift AS Shift ON Ledger.Ttr_ShiftCode = Shift.Msh_ShiftCode
                                        AND (Shift.Msh_ShiftCode = @ShiftCode OR @ShiftCode IS NULL)
                                        AND Shift.Msh_CompanyCode = '{3}'
                                    WHERE dtr.Tel_IsPosted != 1
                                    ORDER BY dtr.Tel_IDNo, CAST(CONVERT(CHAR(10),dtr.Tel_LogDate) + ' ' + Substring(dtr.Tel_LogTime, 1, 2) + ':' + Substring(dtr.Tel_LogTime, 3, 2) AS DateTime), dtr.LudateTime"
                                    , LedgerDBName
                                    , _dtrDBName
                                    , MultiPocketVar.T_Ledger
                                    , LedgerDBCompanyCode
                                    , CentralProfile);
                #endregion
            }
            dtrDB.OpenDB();

            SqlDataReader dataReader = dtrDB.ExecuteReader(sql, System.Data.CommandType.Text, paramInfo, System.Data.CommandBehavior.Default);

            while (dataReader.Read())
            {
                EmployeeDTR dtr = new EmployeeDTR();
                string LogDate = dataReader["LogDate"].ToString();
                dtr.EmployeeID = dataReader["EmployeeID"].ToString().Trim();
                dtr.LogDateTime = Convert.ToDateTime(LogDate + " " + FormatTime(dataReader["LogTime"].ToString()));
                dtr.LogType = GetEquivalentLogType(dataReader["LogType"].ToString());
                dtr.IsPosted = Convert.ToBoolean(dataReader["PostFlag"]);
                dtr.DayCode = dataReader["DayCode"].ToString();
                dtr.ShiftCode = dataReader["ShiftCode"].ToString();
                dtr.IsHoliday = Convert.ToBoolean(dataReader["Holiday"]);
                dtr.IsRestDay = Convert.ToBoolean(dataReader["RestDay"]);
                dtr.ShiftTime.TimeIn = Convert.ToDateTime(LogDate + " " + FormatTime(dataReader["ShiftTimeIn"].ToString()));
                dtr.ShiftBreak.TimeIn = Convert.ToDateTime(LogDate + " " + FormatTime(dataReader["ShiftBreakStart"].ToString()));
                dtr.ShiftBreak.TimeOut = Convert.ToDateTime(LogDate + " " + FormatTime(dataReader["ShiftBreakEnd"].ToString()));
                dtr.ShiftTime.TimeOut = Convert.ToDateTime(LogDate + " " + FormatTime(dataReader["ShiftTimeOut"].ToString()));

                if (dtr.ShiftTime.TimeIn > dtr.ShiftBreak.TimeIn)
                {
                    dtr.ShiftBreak.TimeIn = dtr.ShiftBreak.TimeIn.AddDays(1);
                }

                if (dtr.ShiftTime.TimeIn > dtr.ShiftBreak.TimeOut)
                {
                    dtr.ShiftBreak.TimeOut = dtr.ShiftBreak.TimeOut.AddDays(1);
                }

                if (dtr.ShiftTime.TimeIn > dtr.ShiftTime.TimeOut)
                {
                    dtr.ShiftTime.TimeOut = dtr.ShiftTime.TimeOut.AddDays(1);
                }

                dtrList.Add(dtr);
            }

            dtrDB.CloseDB();

            return dtrList;
        }

        #endregion

        #region Ledger Retrieval

        public List<EmployeeLedger> GetEmployeesLedger()
        {
            return GetEmployeesLedger(string.Empty);
        }

        public List<EmployeeLedger> GetEmployeesLedger(string employeeID)
        {
            return GetEmployeesLedger(employeeID, DateTime.MinValue);
        }

        public List<EmployeeLedger> GetEmployeesLedger(DateTime logDate)
        {
            return GetEmployeesLedger(logDate, DateTime.MinValue);
        }

        public List<EmployeeLedger> GetEmployeesLedger(string employeeID, DateTime logDate)
        {
            return GetEmployeesLedger(employeeID, string.Empty, logDate);
        }

        public List<EmployeeLedger> GetEmployeesLedger(DateTime logDateStart, DateTime logDateEnd)
        {
            return GetEmployeesLedger(string.Empty, string.Empty, logDateStart, logDateEnd);
        }

        public List<EmployeeLedger> GetEmployeesLedger(string employeeID, string shiftCode, DateTime logDate)
        {
            return GetEmployeesLedger(employeeID, shiftCode, logDate, DateTime.MinValue);
        }

        public List<EmployeeLedger> GetEmployeesLedger(string employeeID, string shiftCode, DateTime logDateStart, DateTime logDateEnd)
        {
            List<EmployeeLedger> ledgerList = new List<EmployeeLedger>();
            ParameterInfo[] paramInfo = new ParameterInfo[4];

            if (string.IsNullOrEmpty(employeeID))
            {
                paramInfo[0] = new ParameterInfo("@EmployeeID", DBNull.Value);
            }
            else
            {
                paramInfo[0] = new ParameterInfo("@EmployeeID", employeeID);
            }

            if (logDateStart == DateTime.MinValue)
            {
                paramInfo[1] = new ParameterInfo("@LogDateStart", DBNull.Value);
                paramInfo[2] = new ParameterInfo("@LogDateEnd", DBNull.Value);
            }
            else if (logDateEnd == DateTime.MinValue)
            {
                logDateEnd = logDateStart;
                logDateStart = logDateStart.AddDays(-1);

                paramInfo[1] = new ParameterInfo("@LogDateStart", logDateStart);
                paramInfo[2] = new ParameterInfo("@LogDateEnd", logDateEnd);
            }

            if (string.IsNullOrEmpty(shiftCode))
            {
                paramInfo[3] = new ParameterInfo("@ShiftCode", DBNull.Value);
            }
            else
            {
                paramInfo[3] = new ParameterInfo("@ShiftCode", shiftCode);
            }

            string sql = string.Format(@"BEGIN
 
                                            SELECT    ledger.Ttr_IDNo
                                                    , ledger.Ttr_Date
                                                    , ledger.Ttr_PayCycle
                                                    , ledger.Ttr_DayCode
                                                    , ledger.Ttr_ShiftCode
                                                    , ledger.Ttr_ActIn_1
                                                    , ledger.Ttr_ActOut_1
                                                    , ledger.Ttr_ActIn_2
                                                    , ledger.Ttr_ActOut_2
	                                                , Shift.Msh_Schedule AS ScheduleType
	                                                , Shift.Msh_ShiftIn1 AS ShiftTimeIn
	                                                , Shift.Msh_ShiftOut1 AS ShiftBreakStart
	                                                , Shift.Msh_ShiftIn2 AS ShiftBreakEnd
	                                                , Shift.Msh_ShiftOut2 AS ShiftTimeOut
                                            FROM {0}..{1} AS ledger
                                            INNER JOIN {2}..M_Shift AS shift ON ledger.Ttr_ShiftCode = shift.Msh_ShiftCode
                                                AND shift.Msh_CompanyCode = '{3}'
                                            WHERE (ledger.Ttr_IDNo = @EmployeeID OR @EmployeeID IS NULL)
                                            AND (ledger.Ttr_ShiftCode = @ShiftCode OR @ShiftCode IS NULL)
                                            AND ledger.Ttr_ActIn_1 != '9999'
                                            AND ((@LogDateStart IS NULL AND @LogDateEnd IS NULL) OR (ledger.Ttr_Date BETWEEN @LogDateStart AND @LogDateEnd))
                                
                                        END", LedgerDBName, MultiPocketVar.T_Ledger, CentralProfile, LedgerDBCompanyCode);

            dtrDB.OpenDB();
            SqlDataReader dataReader = dtrDB.ExecuteReader(sql, System.Data.CommandType.Text, paramInfo, System.Data.CommandBehavior.Default);
            //DataSet ds = dtrDB.ExecuteDataSet(sql, CommandType.Text, paramInfo);

            while (dataReader.Read())
            {
                EmployeeLedger ledger = new EmployeeLedger();
                ledger.EmployeeID = dataReader["Ttr_IDNo"].ToString().Trim();
                ledger.ProcessDate = Convert.ToDateTime(dataReader["Ttr_Date"].ToString());
                string LedgerProcessShortDate = ledger.ProcessDate.ToShortDateString();
                ledger.PayPeriod = dataReader["Ttr_PayCycle"].ToString();
                ledger.DayCode = dataReader["Ttr_DayCode"].ToString();
                ledger.ShiftCode = dataReader["Ttr_ShiftCode"].ToString();
                ledger.LogTime1.TimeIn = FormatActualLog(ledger.ProcessDate, dataReader["Ttr_ActIn_1"].ToString());
                ledger.LogTime1.TimeOut = FormatActualLog(ledger.ProcessDate, dataReader["Ttr_ActOut_1"].ToString());
                ledger.LogTime2.TimeIn = FormatActualLog(ledger.ProcessDate, dataReader["Ttr_ActIn_2"].ToString());
                ledger.LogTime2.TimeOut = FormatActualLog(ledger.ProcessDate, dataReader["Ttr_ActOut_2"].ToString());
                ledger.ShiftTime.TimeIn = Convert.ToDateTime(LedgerProcessShortDate + " " + FormatTime(dataReader["ShiftTimeIn"].ToString()));
                ledger.ShiftBreak.TimeIn = Convert.ToDateTime(LedgerProcessShortDate + " " + FormatTime(dataReader["ShiftBreakStart"].ToString()));
                ledger.ShiftBreak.TimeOut = Convert.ToDateTime(LedgerProcessShortDate + " " + FormatTime(dataReader["ShiftBreakEnd"].ToString()));
                ledger.ShiftTime.TimeOut = Convert.ToDateTime(LedgerProcessShortDate + " " + FormatTime(dataReader["ShiftTimeOut"].ToString()));

                if (ledger.ShiftTime.TimeIn > ledger.ShiftBreak.TimeIn)
                {
                    ledger.ShiftBreak.TimeIn = ledger.ShiftBreak.TimeIn.AddDays(1);
                }

                if (ledger.ShiftTime.TimeIn > ledger.ShiftBreak.TimeOut)
                {
                    ledger.ShiftBreak.TimeOut = ledger.ShiftBreak.TimeOut.AddDays(1);
                }

                if (ledger.ShiftTime.TimeIn > ledger.ShiftTime.TimeOut)
                {
                    ledger.ShiftTime.TimeOut = ledger.ShiftTime.TimeOut.AddDays(1);
                }

                ledgerList.Add(ledger);
            }

            dataReader.Close();

            dtrDB.CloseDB();

            return ledgerList;
        }

        #endregion

        #region Ledger Extension Retrieval

        public List<LedgerExtension> GetLedgerExtensions()
        {
            return GetLedgerExtensions(string.Empty);
        }

        public List<LedgerExtension> GetLedgerExtensions(string employeeID)
        {
            return GetLedgerExtensions(employeeID, DateTime.MinValue);
        }

        public List<LedgerExtension> GetLedgerExtensions(string employeeID, DateTime processDate)
        {
            List<LedgerExtension> extensionList = new List<LedgerExtension>();
            ParameterInfo[] paramInfo = new ParameterInfo[2];

            if (string.IsNullOrEmpty(employeeID))
            {
                paramInfo[0] = new ParameterInfo("@EmployeeID", DBNull.Value);
            }
            else
            {
                paramInfo[0] = new ParameterInfo("@EmployeeID", employeeID);
            }

            if (processDate == DateTime.MinValue)
            {
                paramInfo[1] = new ParameterInfo("@ProcessDate", DBNull.Value);
            }
            else
            {
                paramInfo[1] = new ParameterInfo("@ProcessDate", processDate);
            }

            string sql = string.Format(@"SELECT *
                                        FROM {0}..T_EmpTimeRegisterMisc
                                        WHERE (Ttm_IDNo = @EmployeeID OR @EmployeeID IS NULL)
                                        AND (Ttm_Date = @ProcessDate OR @ProcessDate IS NULL)", LedgerDBName);

            SqlDataReader ledgerDataReader = dtrDB.ExecuteReader(sql, System.Data.CommandType.Text, paramInfo, System.Data.CommandBehavior.Default);

            while (ledgerDataReader.Read())
            {
                LedgerExtension extension = new LedgerExtension();

                extension.EmployeeID = ledgerDataReader["Ttm_IDNo"].ToString().Trim();
                extension.ProcessDate = Convert.ToDateTime(ledgerDataReader["Ttm_Date"]);
                extension.PayPeriod = ledgerDataReader["Ttm_PayCycle"].ToString();
                extension.LogTime.TimeIn = Convert.ToDateTime(extension.ProcessDate.ToShortDateString() + " " + FormatTime(ledgerDataReader["Ell_ActualTimeIn"].ToString()));
                extension.LogTime.TimeOut = Convert.ToDateTime(extension.ProcessDate.ToShortDateString() + " " + FormatTime(ledgerDataReader["Ell_ActualTimeOut"].ToString()));
                extension.SequenceNumber = Convert.ToInt32(ledgerDataReader["Ell_SeqNo"]);
                extension.RecordType = RecordType.RETRIEVED;

                extensionList.Add(extension);
            }

            ledgerDataReader.Close();

            return extensionList;
        }

        #endregion

        #region Public Methods

        public void PostTextFileToLedger(string textFile)
        {
            List<EmployeeLedger> ledgers = new List<EmployeeLedger>();
            List<EmployeeDTR> employeesDtr = ConvertTextToDTR(textFile);

            foreach (EmployeeDTR dtr in employeesDtr)
            {
                ledgers = GetEmployeesLedger(dtr.EmployeeID, Convert.ToDateTime(dtr.LogDateTime.ToShortDateString()));
                ledgers = PostDTRToLedger(employeesDtr, ledgers);
                ledgers.ForEach(SaveLedger);
            }
        }

        public void PostTextFileToDTR(string textFile)
        {
            List<EmployeeDTR> employeesDtr = ConvertTextToDTR(textFile);
            employeesDtr.ForEach(SaveDtr);
        }

        public bool IsParameterActive(string parameter)
        {
            bool isActive = false;

            try
            {
                dtrDB.OpenDB();

                string sql = string.Format(@"SELECT 1 FROM {0}..M_PolicyDtl 
                                            WHERE Mpd_SubCode = @ParameterClass 
                                            AND Mpd_CompanyCode = '{1}'
                                            AND Mpd_ParamValue = 1", LedgerDBName, LedgerDBCompanyCode);
                ParameterInfo[] paramInfo = new ParameterInfo[1];
                paramInfo[0] = new ParameterInfo("@ParameterClass", parameter);

                SqlDataReader dataReader = dtrDB.ExecuteReader(sql, System.Data.CommandType.Text, paramInfo, CommandBehavior.Default);

                while (dataReader.Read())
                {
                    isActive = true;
                }

                dataReader.Close();
            }
            finally
            {
                dtrDB.CloseDB();
            }

            return isActive;
        }

        public List<string> GetListOfDBProfiles(bool useDBProfiles)
        {
            List<string> dbProfiles = new List<string>();
            string dbName = string.Empty;

            if (!useDBProfiles)
            {
                AddToDBProfiles(CommonProcedures.GetAppSettingConfigString("CentralDBName", string.Empty), ref dbProfiles);
                //AddToDBProfiles(CommonProcedures.GetAppSettingConfigString("DBNameConfi", string.Empty), ref dbProfiles);
            }
            else
            {
                string sql = string.Format(@"SELECT Mpf_DatabaseName + '|' + Mpf_CompanyCode FROM {0}..M_Profile 
                                                WHERE Mpf_ProfileType IN ('P','S') 
                                                AND Mpf_RecordStatus <> 'C'
                                                AND Mpf_ProfileCategory = '{1}'"
                                                , CentralProfile
                                                , ConfigurationManager.AppSettings["ProfileCategory"].ToString());

                try
                {
                    dtrDB.OpenDB();

                    SqlDataReader dataReader = dtrDB.ExecuteReader(sql);

                    while (dataReader.Read())
                    {
                        AddToDBProfiles(dataReader[0].ToString(), ref dbProfiles);
                    }

                    dataReader.Close();
                }
                finally
                {
                    dtrDB.CloseDB();
                }
            }

            return dbProfiles;
        }

        public void UpdateDTRPostFlag(List<EmployeeDTR> EmployeeDTRList)
        {
            try
            {
                dtrDB.OpenDB();
                String BatchQueryUpdate = "";
                int UpdateCtr = 0;
                foreach (EmployeeDTR dtr in EmployeeDTRList)
                {
                    BatchQueryUpdate += string.Format(@"UPDATE {0}.dbo.{1} SET Tel_IsPosted = '{2}' WHERE Tel_IDNo = '{3}' AND  Tel_LogDate = '{4}'  AND Tel_LogTime = '{5}' AND Tel_LogType = '{6}'"
                                                       , _dtrDBName
                                                       , MultiPocketVar.Used_T_DTR
                                                       , dtr.IsPosted
                                                       , dtr.EmployeeID
                                                       , dtr.LogDateTime.ToString("MM/dd/yyyy")
                                                       , string.Format("{0:00}{1:00}", dtr.LogDateTime.Hour, dtr.LogDateTime.Minute)
                                                       , dtr.LogType.ToString().Substring(0, 1))
                                                       + "\n";
                    ++UpdateCtr;
                    if (UpdateCtr > 25)
                    {
                        dtrDB.ExecuteNonQuery(BatchQueryUpdate);
                        BatchQueryUpdate = "";
                        UpdateCtr = 0;
                    }

                    try
                    {
                        if (MultiPocketVar.GenericCount > 0)
                        {
                            MultiPocketVar.Progress = ((Convert.ToDouble(MultiPocketVar.Count)) / Convert.ToDouble(MultiPocketVar.GenericCount)) * 100;
                            ++MultiPocketVar.Count;
                        }
                    }
                    catch { }
                }

                if (!string.IsNullOrEmpty(BatchQueryUpdate))
                {
                    dtrDB.ExecuteNonQuery(BatchQueryUpdate);
                }
                dtrDB.CloseDB();
            }
            catch { }
        }

        public void UpdateDTRPostFlagDifferential(EmployeeDTR dtr)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[4];

            paramInfo[0] = new ParameterInfo("@IsPosted", dtr.IsPosted);
            paramInfo[1] = new ParameterInfo("@EmployeeID", dtr.EmployeeID);
            paramInfo[2] = new ParameterInfo("@LogDateTime", dtr.LogDateTime);
            paramInfo[3] = new ParameterInfo("@LogType", dtr.LogType.ToString().Substring(0, 1));

            string sql = string.Format(@"UPDATE {0}..T_DTRDifferential
                        SET Dtr_PostFlag = @IsPosted
                        WHERE Dtr_EmployeeID = @EmployeeID
                        AND CAST(Dtr_LogDate + ' ' + Substring(Dtr_LogTime, 1, 2) + ':' + Substring(Dtr_LogTime, 3, 2) AS DateTime) = @LogDateTime
                        AND Dtr_LogType = @LogType", _dtrDBName);

            try
            {
                dtrDB.OpenDB();
                //dtrDB.BeginTransaction();
                dtrDB.ExecuteNonQuery(sql, System.Data.CommandType.Text, paramInfo);
                //dtrDB.CommitTransaction();
            }
            catch
            {
                //dtrDB.RollBackTransaction();
            }
            finally
            {
                dtrDB.CloseDB();
            }
        }

        public void SaveLedger(EmployeeLedger ledger)
        {
            try
            {
                if (MultiPocketVar.GenericCount > 0)
                {
                    ++MultiPocketVar.Count;
                    MultiPocketVar.Progress = ((Convert.ToDouble(MultiPocketVar.Count)) / Convert.ToDouble(MultiPocketVar.GenericCount)) * 100;
                    //Globals.ProgressProcess = string.Format("Start posting time logs on {0}", dtr.LogDateTime);
                }
            }
            catch { }
            //Nilo Added 20130827 : Additional Object in ledger list
            //This indicates that the index is for posting or not.
            //To optimize and minimize number of rows for updating.
            if (ledger.IsForPosting)
            {
                string Query = @"UPDATE {0}.dbo.{1}
                            SET   Ttr_ActIn_1 = @TimeIn1 
                                , Ttr_ActOut_1 = @TimeOut1
                                , Ttr_ActIn_2 = @TimeIn2
                                , Ttr_ActOut_2 = @TimeOut2
                            WHERE Ttr_IDNo = @EmployeeID
                            AND   Ttr_Date = @ProcessDate";

                #region Straight Work Application Checking

                bool StraightWorkConsecutiveApplciation = false;
                /***** *****/
                foreach (DataRow dr in MultiPocketVar.EmployeeStraightWorkList.Rows)
                {
                    String SwtEmployeeID = dr["EmployeeID"].ToString();
                    DateTime SwtStartDate = Convert.ToDateTime(dr["StartDate"]);
                    DateTime SwtEndDate = Convert.ToDateTime(dr["EndDate"]);
                    bool End = false;
                    if ((ledger.EmployeeID == SwtEmployeeID) && (ledger.ProcessDate == SwtStartDate || ledger.ProcessDate == SwtEndDate))
                    {
                        //Checking if employee has file 2 consecutive staight work dates.
                        foreach (DataRow _dr in MultiPocketVar.EmployeeStraightWorkList.Rows)
                        {
                            string _EmployeeID = _dr["EmployeeID"].ToString();
                            DateTime _StartDate = Convert.ToDateTime(_dr["StartDate"]);
                            DateTime _EndDate = Convert.ToDateTime(_dr["EndDate"]);

                            if (_EmployeeID == SwtEmployeeID && _StartDate == SwtEndDate && ledger.ProcessDate == SwtEndDate)
                            {
                                StraightWorkConsecutiveApplciation = true;
                                End = true;
                                break;
                            }
                        }
                    }
                    if (End) break;
                }

                if (StraightWorkConsecutiveApplciation)
                {
                    //Do nothing
                }
                else
                {
                    /***** Nilo added 20130624 : straight work checking *****/
                    foreach (DataRow dr in MultiPocketVar.EmployeeStraightWorkList.Rows)
                    {
                        if (ledger.EmployeeID == dr["EmployeeID"].ToString())
                        {
                            DateTime StartDate = Convert.ToDateTime(dr["StartDate"]);
                            DateTime EndDate = Convert.ToDateTime(dr["EndDate"]);
                            if (DateTime.Compare(ledger.ProcessDate, StartDate) > 0 && DateTime.Compare(ledger.ProcessDate, EndDate) < 0)
                            {
                                return; //Process date is between approved file straightwork
                            }
                            if (Convert.ToDateTime(dr["StartDate"]).ToString("MM/dd/yyyy") == ledger.ProcessDate.ToString("MM/dd/yyyy"))
                            {
                                Query = @"UPDATE {0}.dbo.{1}
                                      SET   Ttr_ActIn_1 = @TimeIn1
                                      WHERE Ttr_IDNo = @EmployeeID
                                      AND   Ttr_Date = @ProcessDate"; ; //Update only the log In
                                break;
                            }
                            if (Convert.ToDateTime(dr["EndDate"]).ToString("MM/dd/yyyy") == ledger.ProcessDate.ToString("MM/dd/yyyy"))
                            {
                                Query = @"UPDATE {0}.dbo.{1}
                                      SET   Ttr_ActOut_2 = @TimeOut2
                                      WHERE Ttr_IDNo = @EmployeeID
                                      AND Ttr_Date = @ProcessDate"; //Update only the log Out
                                break;
                            }
                        }
                    }
                }

                #endregion

                ParameterInfo[] paramInfo = new ParameterInfo[6];

                paramInfo[0] = new ParameterInfo("@TimeIn1", FormatLedgerTime(ledger.ProcessDate, ledger.LogTime1.TimeIn));
                paramInfo[1] = new ParameterInfo("@TimeOut1", FormatLedgerTime(ledger.ProcessDate, ledger.LogTime1.TimeOut));
                paramInfo[2] = new ParameterInfo("@TimeIn2", FormatLedgerTime(ledger.ProcessDate, ledger.LogTime2.TimeIn));
                paramInfo[3] = new ParameterInfo("@TimeOut2", FormatLedgerTime(ledger.ProcessDate, ledger.LogTime2.TimeOut));
                paramInfo[4] = new ParameterInfo("@EmployeeID", ledger.EmployeeID);
                paramInfo[5] = new ParameterInfo("@ProcessDate", ledger.ProcessDate);

                string sql = string.Format(Query, LedgerDBName, MultiPocketVar.T_Ledger);

                try
                {
                    try
                    {
                        Console.Clear();
                        Console.WriteLine(String.Format("\nLedger : {4} \nDate : {1}\nEmployee ID  [{0}]\nPosting {2} of {3}", ledger.EmployeeID, ledger.ProcessDate, MultiPocketVar.Count, MultiPocketVar.GenericCount - 1, MultiPocketVar.LedgerDBName));
                    }
                    catch { }

                    dtrDB.OpenDB();
                    //dtrDB.BeginTransaction();
                    dtrDB.ExecuteNonQuery(sql, System.Data.CommandType.Text, paramInfo);

                    /*****Nilo Commented 20130625 : Code not used *****/
                    //if (!FourPockets)
                    //{
                    //    ledger.LedgerExtension.ForEach(SaveLedgerExtension);
                    //}

                    //dtrDB.CommitTransaction();
                    int dtrIndex = 0;

                    foreach (EmployeeDTR dtr in PostedDTR)
                    {
                        if (dtr.EmployeeID == ledger.EmployeeID &&
                            dtr.DTRLedgerID == ledger.ProcessDate)
                        {
                            PostedDTR[dtrIndex].IsPosted = true;
                        }

                        dtrIndex++;
                    }
                }
                catch
                {
                    //dtrDB.RollBackTransaction();
                }
                finally
                {
                    dtrDB.CloseDB();
                }
            }
        }

        public void SaveLedgerExtension(LedgerExtension extension)
        {
            if (extension.RecordType == RecordType.RETRIEVED)
            {
                return;
            }

            ParameterInfo[] param = new ParameterInfo[7];

            param[0] = new ParameterInfo("@EmployeeID", extension.EmployeeID);
            param[1] = new ParameterInfo("@ProcessDate", extension.ProcessDate);
            param[2] = new ParameterInfo("@PayPeriod", extension.PayPeriod);
            param[3] = new ParameterInfo("@SeqNo", extension.SequenceNumber.ToString("00"));
            param[4] = new ParameterInfo("@TimeIn", FormatLedgerTime(extension.ProcessDate, extension.LogTime.TimeIn));
            param[5] = new ParameterInfo("@TimeOut", FormatLedgerTime(extension.ProcessDate, extension.LogTime.TimeOut));
            param[6] = new ParameterInfo("@Login", extension.UpdatedBy);

            string sql = string.Empty;

            if (extension.RecordType == RecordType.NEW)
            {
                sql = string.Format(@"INSERT INTO {0}..T_EmployeeLogLedgerExt(Ttr_IDNo, Ttr_Date, Ell_PayPeriod, Ell_SeqNo, Ell_ActualTimeIn, Ell_ActualTimeOut, Usr_Login, Ludatetime)
                    VALUES(@EmployeeID, @ProcessDate, @PayPeriod, @SeqNo, @TimeIn, @TimeOut, @Login, GETDATE())", LedgerDBName);
            }
            else if (extension.RecordType == RecordType.UPDATED)
            {
                sql = string.Format(@"UPDATE {0}..T_EmployeeLogLedgerExt
                                    SET Ell_ActualTimeIn = @TimeIn
                                        , Ell_ActualTimeOut = @TimeOut
                                        , Usr_Login = @Login
                                        , Ludatetime = GETDATE()
                                    WHERE Ttr_IDNo = @EmployeeID
                                    AND Ttr_Date = @ProcessDate
                                    AND Ell_PayPeriod = @PayPeriod
                                    AND Ell_SeqNo = @SeqNo", LedgerDBName);
            }
            else
            {
                return;
            }

            dtrDB.ExecuteNonQuery(sql, System.Data.CommandType.Text, param);
        }

        //for MultiPocket processing
        public void SetFirstInto9999(Pocket ledger, string LedgerDBname)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[3];

            paramInfo[0] = new ParameterInfo("@EmployeeID", ledger.EmpID);
            paramInfo[1] = new ParameterInfo("@ProcessDate", ledger.ProcessDate.ToShortDateString(), SqlDbType.DateTime);
            paramInfo[2] = new ParameterInfo("@ShiftCode", ledger.ShiftCode);

            string sql2 = string.Format(@"UPDATE {0}..T_EmpTimeRegister
                                    SET Ttr_ActIn_1 = '9999'
                                    WHERE Ttr_IDNo = @EmployeeID
                                    AND Ttr_ShiftCode=@ShiftCode
                                    AND CONVERT(DATETIME,Ttr_Date) = @ProcessDate", LedgerDBname);
            try
            {
                dtrDB.OpenDB();
                //dtrDB.BeginTransaction();
                affected = dtrDB.ExecuteNonQuery(sql2, CommandType.Text, paramInfo);
                //dtrDB.CommitTransaction();
            }
            catch (Exception ex)
            {
                //dtrDB.RollBackTransaction();
            }
            finally { dtrDB.CloseDB(); }
        }

        public void saveDTRtoELLext(Pocket ledgerExt, string ledgerDBname)
        {
            ParameterInfo[] paraInfo = new ParameterInfo[29];

            #region pairing condition
            if ((ledgerExt.IN01 != DateTime.MinValue) && (ledgerExt.OUT01 == DateTime.MinValue) || (ledgerExt.OUT01 != DateTime.MinValue && ledgerExt.IN01 == DateTime.MinValue))
            {
                ledgerExt.IN01 = DateTime.MinValue;
                ledgerExt.OUT01 = DateTime.MinValue;
            }
            if ((ledgerExt.IN02 != DateTime.MinValue) && (ledgerExt.OUT02 == DateTime.MinValue) || (ledgerExt.OUT02 != DateTime.MinValue && ledgerExt.IN02 == DateTime.MinValue))
            {
                ledgerExt.IN02 = DateTime.MinValue;
                ledgerExt.OUT02 = DateTime.MinValue;
            }
            if ((ledgerExt.IN03 != DateTime.MinValue) && (ledgerExt.OUT03 == DateTime.MinValue) || (ledgerExt.OUT03 != DateTime.MinValue && ledgerExt.IN03 == DateTime.MinValue))
            {
                ledgerExt.IN03 = DateTime.MinValue;
                ledgerExt.OUT03 = DateTime.MinValue;
            }
            if ((ledgerExt.IN04 != DateTime.MinValue) && (ledgerExt.OUT04 == DateTime.MinValue) || (ledgerExt.OUT04 != DateTime.MinValue && ledgerExt.IN04 == DateTime.MinValue))
            {
                ledgerExt.IN04 = DateTime.MinValue;
                ledgerExt.OUT04 = DateTime.MinValue;
            }
            if ((ledgerExt.IN05 != DateTime.MinValue) && (ledgerExt.OUT05 == DateTime.MinValue) || (ledgerExt.OUT05 != DateTime.MinValue && ledgerExt.IN05 == DateTime.MinValue))
            {
                ledgerExt.IN05 = DateTime.MinValue;
                ledgerExt.OUT05 = DateTime.MinValue;
            }
            if ((ledgerExt.IN06 != DateTime.MinValue) && (ledgerExt.OUT06 == DateTime.MinValue) || (ledgerExt.OUT06 != DateTime.MinValue && ledgerExt.IN06 == DateTime.MinValue))
            {
                ledgerExt.IN06 = DateTime.MinValue;
                ledgerExt.OUT06 = DateTime.MinValue;
            }
            if ((ledgerExt.IN07 != DateTime.MinValue) && (ledgerExt.OUT07 == DateTime.MinValue) || (ledgerExt.OUT07 != DateTime.MinValue && ledgerExt.IN07 == DateTime.MinValue))
            {
                ledgerExt.IN07 = DateTime.MinValue;
                ledgerExt.OUT07 = DateTime.MinValue;
            }
            if ((ledgerExt.IN08 != DateTime.MinValue) && (ledgerExt.OUT08 == DateTime.MinValue) || (ledgerExt.OUT08 != DateTime.MinValue && ledgerExt.IN08 == DateTime.MinValue))
            {
                ledgerExt.IN08 = DateTime.MinValue;
                ledgerExt.OUT08 = DateTime.MinValue;
            }

            if ((ledgerExt.IN09 != DateTime.MinValue) && (ledgerExt.OUT09 == DateTime.MinValue) || (ledgerExt.OUT09 != DateTime.MinValue && ledgerExt.IN09 == DateTime.MinValue))
            {
                ledgerExt.IN09 = DateTime.MinValue;
                ledgerExt.OUT09 = DateTime.MinValue;
            }

            if ((ledgerExt.IN10 != DateTime.MinValue) && (ledgerExt.OUT10 == DateTime.MinValue) || (ledgerExt.OUT10 != DateTime.MinValue && ledgerExt.IN10 == DateTime.MinValue))
            {
                ledgerExt.IN10 = DateTime.MinValue;
                ledgerExt.OUT10 = DateTime.MinValue;
            }

            if ((ledgerExt.IN11 != DateTime.MinValue) && (ledgerExt.OUT11 == DateTime.MinValue) || (ledgerExt.OUT11 != DateTime.MinValue && ledgerExt.IN11 == DateTime.MinValue))
            {
                ledgerExt.IN11 = DateTime.MinValue;
                ledgerExt.OUT11 = DateTime.MinValue;
            }

            if ((ledgerExt.IN12 != DateTime.MinValue) && (ledgerExt.OUT12 == DateTime.MinValue) || (ledgerExt.OUT12 != DateTime.MinValue && ledgerExt.IN12 == DateTime.MinValue))
            {
                ledgerExt.IN12 = DateTime.MinValue;
                ledgerExt.OUT12 = DateTime.MinValue;
            }
            #endregion

            #region paraInfo
            paraInfo[0] = new ParameterInfo("@TimeIn1", FormatLedgerTimeExt(ledgerExt.ProcessDate, ledgerExt.IN01));
            paraInfo[1] = new ParameterInfo("@TimeOut1", FormatLedgerTimeExt(ledgerExt.ProcessDate, ledgerExt.OUT01));
            paraInfo[2] = new ParameterInfo("@TimeIn2", FormatLedgerTimeExt(ledgerExt.ProcessDate, ledgerExt.IN02));
            paraInfo[3] = new ParameterInfo("@TimeOut2", FormatLedgerTimeExt(ledgerExt.ProcessDate, ledgerExt.OUT02));
            paraInfo[4] = new ParameterInfo("@TimeIn3", FormatLedgerTimeExt(ledgerExt.ProcessDate, ledgerExt.IN03));
            paraInfo[5] = new ParameterInfo("@TimeOut3", FormatLedgerTimeExt(ledgerExt.ProcessDate, ledgerExt.OUT03));
            paraInfo[6] = new ParameterInfo("@TimeIn4", FormatLedgerTimeExt(ledgerExt.ProcessDate, ledgerExt.IN04));
            paraInfo[7] = new ParameterInfo("@TimeOut4", FormatLedgerTimeExt(ledgerExt.ProcessDate, ledgerExt.OUT04));
            paraInfo[8] = new ParameterInfo("@TimeIn5", FormatLedgerTimeExt(ledgerExt.ProcessDate, ledgerExt.IN05));
            paraInfo[9] = new ParameterInfo("@TimeOut5", FormatLedgerTimeExt(ledgerExt.ProcessDate, ledgerExt.OUT05));
            paraInfo[10] = new ParameterInfo("@TimeIn6", FormatLedgerTimeExt(ledgerExt.ProcessDate, ledgerExt.IN06));
            paraInfo[11] = new ParameterInfo("@TimeOut6", FormatLedgerTimeExt(ledgerExt.ProcessDate, ledgerExt.OUT06));
            paraInfo[12] = new ParameterInfo("@TimeIn7", FormatLedgerTimeExt(ledgerExt.ProcessDate, ledgerExt.IN07));
            paraInfo[13] = new ParameterInfo("@TimeOut7", FormatLedgerTimeExt(ledgerExt.ProcessDate, ledgerExt.OUT07));
            paraInfo[14] = new ParameterInfo("@TimeIn8", FormatLedgerTimeExt(ledgerExt.ProcessDate, ledgerExt.IN08));
            paraInfo[15] = new ParameterInfo("@TimeOut8", FormatLedgerTimeExt(ledgerExt.ProcessDate, ledgerExt.OUT08));
            paraInfo[16] = new ParameterInfo("@TimeIn9", FormatLedgerTimeExt(ledgerExt.ProcessDate, ledgerExt.IN09));
            paraInfo[17] = new ParameterInfo("@TimeOut9", FormatLedgerTimeExt(ledgerExt.ProcessDate, ledgerExt.OUT09));
            paraInfo[18] = new ParameterInfo("@TimeIn10", FormatLedgerTimeExt(ledgerExt.ProcessDate, ledgerExt.IN10));
            paraInfo[19] = new ParameterInfo("@TimeOut10", FormatLedgerTimeExt(ledgerExt.ProcessDate, ledgerExt.OUT10));
            paraInfo[20] = new ParameterInfo("@TimeIn11", FormatLedgerTimeExt(ledgerExt.ProcessDate, ledgerExt.IN11));
            paraInfo[21] = new ParameterInfo("@TimeOut11", FormatLedgerTimeExt(ledgerExt.ProcessDate, ledgerExt.OUT11));
            paraInfo[22] = new ParameterInfo("@TimeIn12", FormatLedgerTimeExt(ledgerExt.ProcessDate, ledgerExt.IN12));
            paraInfo[23] = new ParameterInfo("@TimeOut12", FormatLedgerTimeExt(ledgerExt.ProcessDate, ledgerExt.OUT12));
            paraInfo[24] = new ParameterInfo("@EmployeeID", ledgerExt.EmpID);
            paraInfo[25] = new ParameterInfo("@ProcessDate", ledgerExt.ProcessDate.ToShortDateString(), SqlDbType.Date);
            paraInfo[26] = new ParameterInfo("@Payperiod", ledgerExt.PayPeriod);
            paraInfo[27] = new ParameterInfo("@Usr_Login", ledgerExt.usr_login);
            paraInfo[28] = new ParameterInfo("@ShiftCode", ledgerExt.ShiftCode);
            #endregion

            #region updateQuery
            string sqlUpdate = string.Format(@"UPDATE {0}..T_EmpTimeRegisterMisc
                                                SET Ttm_ActIn_01 = @TimeIn1
                                                    , Ttm_ActOut_01 = @TimeOut1
                                                    , Ttm_ActIn_02 = @TimeIn2
                                                    , Ttm_ActOut_02 = @TimeOut2
                                                    , Ttm_ActIn_03 = @TimeIn3
                                                    , Ttm_ActOut_03 = @TimeOut3
                                                    , Ttm_ActIn_04 = @TimeIn4
                                                    , Ttm_ActOut_04 = @TimeOut4
                                                    , Ttm_ActIn_05 = @TimeIn5
                                                    , Ttm_ActOut_05 = @TimeOut5
                                                    , Ttm_ActIn_06 = @TimeIn6
                                                    , Ttm_ActOut_06 = @TimeOut6
                                                    , Ttm_ActIn_07 = @TimeIn7
                                                    , Ttm_ActOut_07 = @TimeOut7
                                                    , Ttm_ActIn_08 = @TimeIn8
                                                    , Ttm_ActOut_08 = @TimeOut8
                                                    , Ttm_ActIn_09 = @TimeIn9
                                                    , Ttm_ActOut_09 = @TimeOut9
                                                    , Ttm_ActIn_10 = @TimeIn10
                                                    , Ttm_ActOut_10 = @TimeOut10
                                                    , Ttm_ActIn_11 = @TimeIn11
                                                    , Ttm_ActOut_11 = @TimeOut11
                                                    , Ttm_ActIn_12 = @TimeIn12
                                                    , Ttm_ActOut_12 = @TimeOut12
                                                    , Ludatetime = GETDATE()
                                                WHERE Ttm_IDNo = @EmployeeID
                                                AND CONVERT(DATETIME,Ttm_Date) = @ProcessDate", ledgerDBname);
            #endregion

            #region InsertQuery
            string sqlInsert = string.Format(@"INSERT {0}..T_EmpTimeRegisterMisc
		                                    (Ttm_IDNo
                                            ,Ttm_Date
		                                    ,Ttm_PayCycle
                                            ,Ttm_ActIn_01
                                            ,Ttm_ActOut_01
                                            ,Ttm_ActIn_02
                                            ,Ttm_ActOut_02
                                            ,Ttm_ActIn_03
                                            ,Ttm_ActOut_03
                                            ,Ttm_ActIn_04
                                            ,Ttm_ActOut_04
                                            ,Ttm_ActIn_05
                                            ,Ttm_ActOut_05
                                            ,Ttm_ActIn_06
                                            ,Ttm_ActOut_06
                                            ,Ttm_ActIn_07
                                            ,Ttm_ActOut_07
                                            ,Ttm_ActIn_08
                                            ,Ttm_ActOut_08
                                            ,Ttm_ActIn_09
                                            ,Ttm_ActOut_09
                                            ,Ttm_ActIn_10
                                            ,Ttm_ActOut_10
                                            ,Ttm_ActIn_11
                                            ,Ttm_ActOut_11
                                            ,Ttm_ActIn_12
                                            ,Ttm_ActOut_12
                                            ,Usr_Login
                                            ,Ludatetime)
		                                  values(@EmployeeID,@ProcessDate,@Payperiod
				                                 ,@TimeIn1,@TimeOut1 --PAIR 1
                                                 ,@TimeIn2,@TimeOut2
                                                 ,@TimeIn3,@TimeOut3
                                                 ,@TimeIn4,@TimeOut4
                                                 ,@TimeIn5,@TimeOut5
				                                 ,@TimeIn6,@TimeOut6
                                                 ,@TimeIn7,@TimeOut7
                                                 ,@TimeIn8,@TimeOut8
                                                 ,@TimeIn9,@TimeOut9
                                                 ,@TimeIn10,@TimeOut10
                                                 ,@TimeIn11,@TimeOut11
                                                 ,@TimeIn12,@TimeOut12
				                                 ,@Usr_Login,GETDATE())", ledgerDBname);
            #endregion

            #region Updating postflag
            string sqlflag = string.Format(@"UPDATE {0}..T_EmpDTR
                                             SET Tel_IsPosted=1
                                             ,USR_LOGIN='LOGUPLDSRVC'
                                             WHERE Tel_LogDate=@ProcessDate
                                                AND Tel_IsPosted!=1
                                                AND 
                                                 (  
                                                        Tel_LogTime=@TimeIn1
                                                        OR (Tel_LogTime=@TimeOut1)
                                                        OR (Tel_LogTime=@TimeIn2)
                                                        OR (Tel_LogTime=@TimeOut2)
                                                        OR (Tel_LogTime=@TimeIn3)
                                                        OR (Tel_LogTime=@TimeOut3)
                                                        OR (Tel_LogTime=@TimeIn4)
                                                        OR (Tel_LogTime=@TimeOut4)
                                                        OR (Tel_LogTime=@TimeIn5)
                                                        OR (Tel_LogTime=@TimeOut5)
                                                        OR (Tel_LogTime=@TimeIn6)
                                                        OR (Tel_LogTime=@TimeOut6)
                                                        OR (Tel_LogTime=@TimeIn7)
                                                        OR (Tel_LogTime=@TimeOut7)
                                                        OR (Tel_LogTime=@TimeIn8)
                                                        OR (Tel_LogTime=@TimeOut8)
                                                        OR (Tel_LogTime=@TimeIn9)
                                                        OR (Tel_LogTime=@TimeOut9)
                                                        OR (Tel_LogTime=@TimeIn10)
                                                        OR (Tel_LogTime=@TimeOut10)
                                                        OR (Tel_LogTime=@TimeIn11)
                                                        OR (Tel_LogTime=@TimeOut11)
                                                        OR (Tel_LogTime=@TimeIn12)
                                                        OR (Tel_LogTime=@TimeOut12)
                                                )
                                                AND Tel_IDNo=@EmployeeID", _dtrDBName);
            #endregion

            string selectsql = string.Format(@"SELECT COUNT(Ttm_IDNo) AS COUNT 
                                               FROM {0}..T_EmpTimeRegisterMisc
                                               WHERE Ttm_IDNo= @EmployeeID
                                                    AND CONVERT(DATE,Ttm_Date)=@ProcessDate", ledgerDBname);
            try
            {
                int affected;
                dtrDB.OpenDB();
                //dtrDB.BeginTransaction();
                if (Convert.ToInt16(dtrDB.ExecuteDataSet(selectsql, CommandType.Text, paraInfo).Tables[0].Rows[0]["COUNT"].ToString()) > 0)
                    affected = dtrDB.ExecuteNonQuery(sqlUpdate, System.Data.CommandType.Text, paraInfo);
                else
                    affected = dtrDB.ExecuteNonQuery(sqlInsert, System.Data.CommandType.Text, paraInfo);

                //dtrDB.CommitTransaction();

                //dtrDB.BeginTransaction();
                affected = dtrDB.ExecuteNonQuery(sqlflag, System.Data.CommandType.Text, paraInfo);
                //dtrDB.CommitTransaction();
            }
            catch (Exception e)
            {
                _log.WriteLog(Application.StartupPath, "Save to EmpTimeRegisterMisc", " : RollBack", e.ToString(), true);
                //dtrDB.RollBackTransaction();
            }
            finally { dtrDB.CloseDB(); }

        }

        public void CheckExtIfFull(List<Pocket> MultiList, string ledgerDBname, bool _RegShift, DateTime _processDate)
        {
            foreach (Pocket _pocket in MultiList)
            {
                //if (
                //    (
                //        (_pocket.OUT02 != DateTime.MinValue) &&
                //            ((_pocket.IN03 != DateTime.MinValue) && (_pocket.OUT03 != DateTime.MinValue))
                //     )
                //    ||
                //    (Convert.ToDateTime(_pocket.OUT02.ToShortDateString()) == Convert.ToDateTime(_processDate.ToShortDateString())) //Since Process Date was subtracted by 1 upon calling
                //   )
                //{
                    saveDTRtoELLext(_pocket, ledgerDBname);
                    //SetFirstInto9999(_pocket, ledgerDBname);
                //}
            }
        }

        public DataTable GetEmployeeStraightWork(string ledgerDBname, DateTime StartDate, DateTime EndDate)
        {
            DataTable dt = new DataTable();
            try
            {
                ParameterInfo[] param = new ParameterInfo[2];
                param[0] = new ParameterInfo("@StartDate", StartDate.ToShortDateString(), SqlDbType.DateTime);
                param[1] = new ParameterInfo("@EndDate", EndDate.ToShortDateString(), SqlDbType.DateTime);
                string Query = @" SELECT Swt_EmployeeId [EmployeeID] 
                                       , Swt_FromDate [StartDate]
                                       , Swt_ToDate [EndDate]
                                       , Swt_Status [Status]
                                    FROM {0}.dbo.T_EmployeeStraightWork
                                   WHERE ((@StartDate BETWEEN Swt_FromDate AND Swt_ToDate) OR (@EndDate BETWEEN Swt_FromDate AND Swt_ToDate))
                                     AND Swt_Status = '9'
                                ORDER BY Swt_FromDate";
                dtrDB.OpenDB();
                DataSet ds = dtrDB.ExecuteDataSet(string.Format(Query,ledgerDBname), CommandType.Text, param);
                if(ds != null)
                {
                    foreach (DataTable _dt in ds.Tables)
                    {
                        dt = _dt;
                    }
                }

                dtrDB.CloseDB();
            }
            catch { }
            return dt;
        }


        public List<EmployeeLedger> PostDTRToLedger(List<EmployeeDTR> employeesDTR, List<EmployeeLedger> employeeLedgers)
        {
            int ledgerIndex = -1;
            int dtrIndex = 0;
            int x = 0;
            foreach (EmployeeDTR dtr in employeesDTR)
            {
                //Test break 
                if (dtr.EmployeeID == "200857")
                {
                    string Xx = "90133";
                }

                try
                {
                    if (employeesDTR.Count > 0)
                    {
                        MultiPocketVar.Progress = ((Convert.ToDouble(x)) / Convert.ToDouble(employeesDTR.Count)) * 100;
                        ++x;
                    }
                }
                catch { }

                if (dtr.IsPosted)
                {
                    dtrIndex++;
                    continue;
                }

                //Look for the appropriate ledger on where the log should be posted
                ledgerIndex = employeeLedgers.FindIndex(delegate(EmployeeLedger ledger)
                {
                    if (dtr.EmployeeID == ledger.EmployeeID &&
                    (dtr.ShiftTime.TimeIn.ToShortDateString() != dtr.ShiftTime.TimeOut.ToShortDateString()) &&
                    dtr.LogType == LogTypes.OUT &&
                    dtr.LogDateTime < dtr.ShiftTime.TimeIn)
                    {
                        return (dtr.LogDateTime.AddDays(-1).ToShortDateString() == ledger.ProcessDate.ToShortDateString());
                    }

                    return (dtr.EmployeeID == ledger.EmployeeID) &&
                    (dtr.LogDateTime.ToShortDateString() == ledger.ProcessDate.ToShortDateString());
                });

                if (ledgerIndex < 0)
                {
                    dtrIndex++;
                    continue;
                }

                //Recheck on where to post the log coz sometimes the shift code are not correctly indicated
                if (ledgerIndex > 0)
                {
                    if (dtr.LogType == LogTypes.IN && dtrIndex > 0 && dtrIndex < employeesDTR.Count - 1)
                    {
                        //Test break 
                        if (dtr.EmployeeID == "0484")
                        {
                            string Xx = "";
                        }

                        if (employeesDTR[dtrIndex + 1].EmployeeID == dtr.EmployeeID &&
                            employeesDTR[dtrIndex + 1].LogDateTime < dtr.ShiftTime.TimeIn &&
                            employeesDTR[dtrIndex - 1].LogType != dtr.LogType
                            //This is to indicate that In of employee earlier of [shift in] with [regular shift] will not be included.
                            && employeesDTR[dtrIndex + 1].LogDateTime > Convert.ToDateTime(employeesDTR[dtrIndex + 1].LogDateTime.ToShortDateString() + " 03:00 PM")
                            //This to disclude afternoon logs in with grave shift.
                            && !(dtr.LogDateTime > Convert.ToDateTime(dtr.LogDateTime.ToShortDateString() + " 12:01 PM") && dtr.ShiftTime.TimeIn.ToShortDateString() != dtr.ShiftTime.TimeOut.ToShortDateString())
                            )
                        {
                            ledgerIndex--;
                        }


                        //Commented : Confused on the usage of this block and some logs are being skipped.

                        //if (dtrIndex - 1 >= 0 &&
                        //dtr.LogType == employeesDTR[dtrIndex - 1].LogType &&
                        //dtr.EmployeeID == employeesDTR[dtrIndex - 1].EmployeeID)
                        //{
                        //    dtr.IsPosted = true;
                        //    StorePostedDTR(dtr);
                        //    dtrIndex++;
                        //    continue;
                        //}

                        //End Comment
                    }
                    else if (dtr.LogType == LogTypes.OUT)
                    {
                        if (dtr.LogDateTime < dtr.ShiftBreak.TimeIn &&
                        employeeLedgers[ledgerIndex - 1].EmployeeID == dtr.EmployeeID &&
                        (employeeLedgers[ledgerIndex].EmployeeID == dtr.EmployeeID &&
                        (employeeLedgers[ledgerIndex].LogTime1.TimeIn == DateTime.MinValue &&
                        employeeLedgers[ledgerIndex].LogTime2.TimeIn == DateTime.MinValue)))
                        {
                            ledgerIndex--;
                        }
                    }
                }

                if (ledgerIndex < 0)
                {
                    dtrIndex++;
                    continue;
                }

                if (employeeLedgers[ledgerIndex].EmployeeID == dtr.EmployeeID)
                {
                    employeeLedgers[ledgerIndex] = SetUpLedger(employeeLedgers[ledgerIndex], dtr);
                    dtr.DTRLedgerID = employeeLedgers[ledgerIndex].ProcessDate;
                    StorePostedDTR(dtr);
                    //Additional Object in ledger list
                    //This indicates that the index is for posting or not.
                    //To optimize and minimize number of rows for updating.
                    employeeLedgers[ledgerIndex].IsForPosting = true;
                }

                dtrIndex++;
            }

            MultiPocketVar.Progress = 100;
            return employeeLedgers;
        }

        public List<EmployeeLedger> SaveToLedgerWithUpdatedShifts(List<EmployeeLedger> employeeLedgers)
        {
            #region queries
            string sqlScheduleN = string.Format(@"
                    select * from {0}..T_ShiftCodeMaster
                    where Scm_ScheduleType = 'N'
                    and Msh_8HourShiftCode <> ''
                    order by Msh_ShiftIn1 asc
                ", LedgerDBName);

            string sqlScheduleG = string.Format(@"
                    select * from {0}..T_ShiftCodeMaster
                    where Scm_ScheduleType = 'G'
                    and Msh_8HourShiftCode <> ''
                    order by Msh_ShiftIn1 asc
                ", LedgerDBName);
            string sqlScheduleA = string.Format(@"
                    select * from {0}..T_ShiftCodeMaster
                    where Scm_ScheduleType = 'A'
                    and Msh_8HourShiftCode <> ''
                    order by Msh_ShiftIn1 asc
                ", LedgerDBName);

            string sqlEquivN = string.Format(@"
                    select * from {0}..T_ShiftCodeMaster
                    where Scm_ScheduleType = 'N'
                    and Msh_8HourShiftCode = ''
                ", LedgerDBName);

            string sqlEquivG = string.Format(@"
                    select * from {0}..T_ShiftCodeMaster
                    where Scm_ScheduleType = 'G'
                    and Msh_8HourShiftCode = ''
                ", LedgerDBName);

            string sqlEquivA = string.Format(@"
                    select * from {0}..T_ShiftCodeMaster
                    where Scm_ScheduleType = 'A'
                    and Msh_8HourShiftCode = ''
                ", LedgerDBName);

            #endregion

            DataSet dsShiftCodeN;
            DataSet dsShiftCodeG;
            DataSet dsShiftCodeA;
            DataSet dsShiftCodeNEquiv;
            DataSet dsShiftCodeGEquiv;
            DataSet dsShiftCodeAEquiv;
            using (DALHelper ledgerDB = new DALHelper(LedgerDBName, false))
            {
                try
                {
                    ledgerDB.OpenDB();
                    dsShiftCodeN = ledgerDB.ExecuteDataSet(sqlScheduleN, CommandType.Text);
                    dsShiftCodeG = ledgerDB.ExecuteDataSet(sqlScheduleG, CommandType.Text);
                    dsShiftCodeA = ledgerDB.ExecuteDataSet(sqlScheduleA, CommandType.Text);
                    dsShiftCodeNEquiv = ledgerDB.ExecuteDataSet(sqlEquivN, CommandType.Text);
                    dsShiftCodeGEquiv = ledgerDB.ExecuteDataSet(sqlEquivG, CommandType.Text);
                    dsShiftCodeAEquiv = ledgerDB.ExecuteDataSet(sqlEquivA, CommandType.Text);
                }
                catch
                {
                    ledgerDB.CloseDB();
                    return employeeLedgers;
                }
                finally
                {
                    ledgerDB.CloseDB();
                }
            }
            //Main Update (SHIFTCODE and OVERTIME APPLICATION)
            for (int idx = 0; idx < employeeLedgers.Count; idx++)
            {
                try
                {
                    string Endtime = string.Empty;

                    #region shiftcode update
                    int time = 0;
                    string shiftCode = employeeLedgers[idx].ShiftCode;
                    time = ConvertTimeToMinutes(Plus24HundredHours(employeeLedgers[idx].ProcessDate, employeeLedgers[idx].LogTime1.TimeIn));


                    #region Normal shift
                    if (employeeLedgers[idx].isNormalShift == 1 && !employeeLedgers[idx].IsHoliday && !employeeLedgers[idx].IsRestday)
                    {
                        for (int i = 0; i < dsShiftCodeN.Tables[0].Rows.Count; i++)
                        {
                            if (i == dsShiftCodeN.Tables[0].Rows.Count - 1)
                            {
                                if (time >= ConvertTimeToMinutes(dsShiftCodeN.Tables[0].Rows[i]["Msh_ShiftIn1"].ToString())
                                    && time <= ConvertTimeToMinutes(dsShiftCodeN.Tables[0].Rows[i]["Scm_ShiftBreakStart"].ToString()))
                                {
                                    shiftCode = dsShiftCodeN.Tables[0].Rows[i]["Scm_ShiftCode"].ToString().Trim(); //N005
                                    Endtime = dsShiftCodeN.Tables[0].Rows[i]["Msh_ShiftOut2"].ToString().Trim();
                                    break;
                                }
                                else if (time > ConvertTimeToMinutes(dsShiftCodeN.Tables[0].Rows[i]["Scm_ShiftBreakStart"].ToString()))
                                {
                                    shiftCode = dsShiftCodeN.Tables[0].Rows[0]["Scm_ShiftCode"].ToString().Trim(); //N001
                                    Endtime = dsShiftCodeN.Tables[0].Rows[0]["Msh_ShiftOut2"].ToString().Trim();
                                    break;
                                }
                            }
                            else
                            {
                                if (time > ConvertTimeToMinutes(dsShiftCodeN.Tables[0].Rows[i]["Msh_ShiftIn1"].ToString())
                                   && time <= ConvertTimeToMinutes(dsShiftCodeN.Tables[0].Rows[i + 1]["Msh_ShiftIn1"].ToString()))
                                {
                                    shiftCode = dsShiftCodeN.Tables[0].Rows[i + 1]["Scm_ShiftCode"].ToString().Trim();
                                    Endtime = dsShiftCodeN.Tables[0].Rows[i + 1]["Msh_ShiftOut2"].ToString().Trim();
                                }
                                if (i == 0 && time <= ConvertTimeToMinutes(dsShiftCodeN.Tables[0].Rows[i]["Msh_ShiftIn1"].ToString()))
                                {
                                    shiftCode = dsShiftCodeN.Tables[0].Rows[i]["Scm_ShiftCode"].ToString().Trim();
                                    Endtime = dsShiftCodeN.Tables[0].Rows[i]["Msh_ShiftOut2"].ToString().Trim();
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!employeeLedgers[idx].IsHoliday && !employeeLedgers[idx].IsRestday)
                        {
                            shiftCode = dsShiftCodeN.Tables[0].Rows[0]["Scm_ShiftCode"].ToString().Trim();
                            Endtime = dsShiftCodeN.Tables[0].Rows[0]["Msh_ShiftOut2"].ToString().Trim();
                        }
                        else if (dsShiftCodeNEquiv.Tables[0].Rows.Count > 0)
                        {
                            shiftCode = dsShiftCodeNEquiv.Tables[0].Rows[0]["Scm_ShiftCode"].ToString().Trim();
                            Endtime = dsShiftCodeNEquiv.Tables[0].Rows[0]["Msh_ShiftOut2"].ToString().Trim();
                        }
                    }
                    #endregion
                    #region Afternoon shift
                    if (employeeLedgers[idx].isNormalShift == 1 && !employeeLedgers[idx].IsHoliday && !employeeLedgers[idx].IsRestday)
                    {
                        for (int i = 0; i < dsShiftCodeA.Tables[0].Rows.Count - 1; i++)
                        {
                            if (time > ConvertTimeToMinutes(dsShiftCodeA.Tables[0].Rows[i]["Msh_ShiftIn1"].ToString())
                                && time <= ConvertTimeToMinutes(dsShiftCodeA.Tables[0].Rows[i + 1]["Msh_ShiftIn1"].ToString()))
                            {
                                shiftCode = dsShiftCodeA.Tables[0].Rows[i]["Scm_ShiftCode"].ToString().Trim();
                                Endtime = dsShiftCodeA.Tables[0].Rows[i]["Msh_ShiftOut2"].ToString().Trim();
                                break;
                            }
                        }
                    }
                    #endregion
                    #region Graveyard shift
                    if (employeeLedgers[idx].isNormalShift == 1 && !employeeLedgers[idx].IsHoliday && !employeeLedgers[idx].IsRestday)
                    {
                        for (int i = 0; i < dsShiftCodeG.Tables[0].Rows.Count - 1; i++)
                        {
                            if (time > ConvertTimeToMinutes(dsShiftCodeG.Tables[0].Rows[i]["Msh_ShiftIn1"].ToString())
                                && time <= ConvertTimeToMinutes(dsShiftCodeG.Tables[0].Rows[i + 1]["Msh_ShiftIn1"].ToString()))
                            {
                                shiftCode = dsShiftCodeG.Tables[0].Rows[i]["Scm_ShiftCode"].ToString().Trim();
                                Endtime = dsShiftCodeG.Tables[0].Rows[i]["Msh_ShiftOut2"].ToString().Trim();
                                break;
                            }
                        }
                    }
                    #endregion

                    employeeLedgers[idx].ShiftCode = shiftCode;

                    #endregion

                    #region Overtime Application

                    #region query

                    string sqlOvertime = string.Format(
                                @"
                        select 
                            * 
                        from T_EmployeeOvertime
                        where Eot_EmployeeId = '{0}'
                        and Eot_OvertimeDate = '{1}'
                        and Eot_Status not in ('0','2','4','6','8')
                        and Eot_OvertimeType = 'P'
                        order by Eot_StartTime
                            ",
                                employeeLedgers[idx].EmployeeID, employeeLedgers[idx].ProcessDate.ToString("MM/dd/yyyy"));

                    #endregion
                    using (DALHelper ledgerDB = new DALHelper(LedgerDBName, false))
                    {
                        try
                        {
                            ledgerDB.OpenDB();
                            DataSet ds = ledgerDB.ExecuteDataSet(sqlOvertime, CommandType.Text);
                            int ActualtimeOut = ConvertTimeToMinutes(Endtime);
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                #region update process
                                //ledgerDB.BeginTransaction();
                                int OvertimeStart = ConvertTimeToMinutes(ds.Tables[0].Rows[i]["Eot_StartTime"].ToString());
                                if (OvertimeStart < ActualtimeOut)
                                {
                                    //string i1 = ConvertMinutesToHours(ActualtimeOut);
                                    //int i2 = ConvertTimeToMinutes(ds.Tables[0].Rows[i]["Eot_EndTime"].ToString());
                                    //string i3 = ConvertMinutesToHours( i2 + (ActualtimeOut - OvertimeStart) );
                                    string sqlUpdateOvertime = string.Format(
                                                    @"Update T_EmployeeOvertime
                                            set Eot_StartTime = '{0}'
                                            , Eot_EndTime = '{1}'
                                            , Usr_Login = 'LOGUPLDSRVC'
                                            , ludatetime = GETDATE()
                                            where Eot_EmployeeID = '{2}'
                                            And Eot_OvertimeDate = '{3}'
                                            And Eot_ControlNo = '{4}'
                                            "
                                                    , ConvertMinutesToHours(ActualtimeOut)
                                                    , ConvertMinutesToHours(
                                                                ConvertTimeToMinutes(ds.Tables[0].Rows[i]["Eot_EndTime"].ToString())
                                                                + (ActualtimeOut - OvertimeStart))
                                                    , employeeLedgers[idx].EmployeeID
                                                    , ds.Tables[0].Rows[i]["Eot_OvertimeDate"].ToString()
                                                    , ds.Tables[0].Rows[i]["Eot_ControlNo"].ToString());
                                    ledgerDB.ExecuteNonQuery(sqlUpdateOvertime, CommandType.Text);
                                }
                                //ledgerDB.CommitTransaction();
                                #endregion

                                ActualtimeOut = ConvertTimeToMinutes(ds.Tables[0].Rows[i]["Eot_EndTime"].ToString())
                                                                + (ActualtimeOut - OvertimeStart);
                            }
                        }
                        catch
                        {
                            //ledgerDB.RollBackTransaction();
                        }
                        finally
                        {
                            ledgerDB.CloseDB();
                        }
                    }
                    #endregion
                }
                catch
                {

                }
                finally
                {

                }


            }
            return employeeLedgers;
        }

        private int ConvertTimeToMinutes(string time)
        {
            int ret = (
                (Convert.ToInt32(time.Substring(0, 2)) * 60)
                +
                (Convert.ToInt32(time.Substring(2, 2)))
                );
            return ret;
        }

        private string ConvertMinutesToHours(int time)
        {
            int hour = time / 60;
            int min = time % 60;
            string ret = (
                hour.ToString("00")
                +
                min.ToString("00")
                );
            return ret;
        }

        private string Plus24HundredHours(DateTime processDate, DateTime logDateTime)
        {
            return (string.Compare(processDate.ToShortDateString(), logDateTime.ToShortDateString()) == 0 ||
                logDateTime == DateTime.MinValue ? logDateTime.ToString("HHmm") : (Convert.ToInt32(logDateTime.ToString("HHmm")) + (Plus24Hours ? 2400 : 0)).ToString("0000"));
        }

        #endregion

    }

    #endregion

    #region LogUploading Objects

    public enum LogTypes
    {
        IN,
        OUT
    }

    public enum LedgerLogTypes
    {
        UNDEFINED,
        IN1,
        IN2,
        OUT1,
        OUT2,
        EXTENSIONIN,
        EXTENSIONOUT
    }

    public enum ScheduleTypes
    {
        DAY,
        GRAVEYARD,
        SWING
    }

    public enum RecordType
    {
        RETRIEVED,
        NEW,
        UPDATED
    }

    public struct TimeEntry
    {
        public DateTime TimeIn;
        public DateTime TimeOut;
    }

    public class EmployeeDTR
    {
        public string EmployeeID;
        public DateTime LogDateTime;
        public LogTypes LogType;
        public string DayCode;
        public string ShiftCode;
        public bool IsHoliday;
        public bool IsRestDay;
        public bool IsPosted;
        public TimeEntry ShiftTime = new TimeEntry();
        public TimeEntry ShiftBreak = new TimeEntry();

        public DateTime DTRLedgerID = DateTime.MinValue; //Serves as the ID of the Ledger if DTR has been posted or not
    }

    //Added by:Robeth John Bacalan<03/29/2012>
    //Ledger for Multiple pockets.
    public class LedgerExtforMultiplePocket
    {
        public string EmployeeID;
        public DateTime ProcessDate;
        public string PayPeriod;
        public DateTime LuDateTime;
        public TimeEntry pocket03 = new TimeEntry();
        public TimeEntry pocket04 = new TimeEntry();
        public TimeEntry pocket05 = new TimeEntry();
        public TimeEntry pocket06 = new TimeEntry();
        public TimeEntry pocket07 = new TimeEntry();
        public TimeEntry pocket08 = new TimeEntry();
        public TimeEntry pocket09 = new TimeEntry();
        public TimeEntry pocket10 = new TimeEntry();
    }
    //Pocket Objects
    public class Pocket
    {
        #region packet objects
        public String EmpID;
        public String usr_login;
        public String ShiftCode;
        public DateTime ProcessDate;
        public string PayPeriod;
        public DateTime IN01;
        public DateTime OUT01;
        public DateTime IN02;
        public DateTime OUT02;
        public DateTime IN03;
        public DateTime OUT03;
        public DateTime IN04;
        public DateTime OUT04;
        public DateTime IN05;
        public DateTime OUT05;
        public DateTime IN06;
        public DateTime OUT06;
        public DateTime IN07;
        public DateTime OUT07;
        public DateTime IN08;
        public DateTime OUT08;
        public DateTime IN09;
        public DateTime OUT09;
        public DateTime IN10;
        public DateTime OUT10;
        public DateTime IN11;
        public DateTime OUT11;
        public DateTime IN12;
        public DateTime OUT12;
        public static Char REGSHIFT = '<';
        public static Char GRAVESHIFT = '>';
        #endregion
    }

    //end

    public class EmployeeLedger
    {
        public string EmployeeID;
        public DateTime ProcessDate;
        public string PayPeriod;
        public string DayCode;
        public string ShiftCode;
        public bool IsHoliday;
        public bool IsRestday;
        public TimeEntry ShiftTime = new TimeEntry();
        public TimeEntry ShiftBreak = new TimeEntry();
        public TimeEntry LogTime1 = new TimeEntry();
        public TimeEntry LogTime2 = new TimeEntry();
        public LedgerLogTypes LastLogType = LedgerLogTypes.UNDEFINED;
        public int isNormalShift;
        public DateTime StraightWorkStartDate;
        public DateTime StraightWorkEndDate;
        public List<LedgerExtension> LedgerExtension = new List<LedgerExtension>();
        public bool IsForPosting = false;
    }

    public class LedgerExtension
    {
        public string EmployeeID;
        public DateTime ProcessDate;
        public string PayPeriod;
        public int SequenceNumber;
        public TimeEntry LogTime = new TimeEntry();
        public string UpdatedBy = "SERVICE";
        public RecordType RecordType;
    }

#endregion

}
