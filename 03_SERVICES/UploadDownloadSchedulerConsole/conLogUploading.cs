using System;
using System.Collections.Generic;
using System.Data;
using CommonPostingLibrary;
using Posting.BLogic;
using Posting.DAL;
using System.Windows.Forms;
using System.Configuration;
using System.Reflection;
using System.Linq;

namespace UploadDownloadSchedulerConsole
{
    public class conLogUploading : BaseConsole
    {
        #region Member Variables
        DateTime _processDate = DateTime.Today;
        NLLogger.Logger _log = new NLLogger.Logger();
        private LogPostingBL LogPostingFunctions = new LogPostingBL(Globals.DTRDBName);
        private DALHelper dtrDB = new DALHelper(true);
        private CommonBL commonBL = new CommonBL();

        DataTable dtEmployeeLogLedger = null;
        DataTable dtDTR = null;
        DataTable dtLogTrail = null;
        DataTable dtTimeCor = null;
        DataTable dtDtrOverride = null;
        
        #endregion

        #region Public Methods

        public void UploadTheLogs(string[] args)
        {
            if (args.Length == 2)
            {
                try
                {
                    _log.WriteLog(Application.StartupPath, "Posting", string.Format("On date posting = {0}", args[1]), "", true);
                    Globals.isServicePost = true; //will execute text file and previous day posting

                    if (Globals.isAutoRepostLogsViaLogControl == false)
                        NewLogPosting(Convert.ToDateTime(args[1]).AddDays(-1), Convert.ToDateTime(args[1]));
                    else
                        NewLogPosting(Convert.ToDateTime(args[1]), Convert.ToDateTime(args[1]));
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
                    _log.WriteLog(Application.StartupPath, "Posting", string.Format("Range date posting = {0} to {1}", args[1], args[2]), "", true);

                    if (Globals.isAutoRepostLogsViaLogControl == false)
                        NewLogPosting(Convert.ToDateTime(args[1]).AddDays(-1), Convert.ToDateTime(args[2]));
                    else
                        NewLogPosting(Convert.ToDateTime(args[1]), Convert.ToDateTime(args[2]));
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
                    if (Convert.ToByte(args[3]) == 1)
                    {
                        Globals.isOverwrite = true;
                        _log.WriteLog(Application.StartupPath, "Posting", string.Format("Range date posting = {0} to {1}, With Overwrite", args[1], args[2]), "", true);
                    }
                    else
                    {
                        Globals.isOverwrite = false;
                        _log.WriteLog(Application.StartupPath, "Posting", string.Format("Range date posting = {0} to {1}", args[1], args[2]), "", true);
                    }

                    if (Globals.isAutoRepostLogsViaLogControl == false)
                        NewLogPosting(Convert.ToDateTime(args[1]).AddDays(-1), Convert.ToDateTime(args[2]));
                    else
                        NewLogPosting(Convert.ToDateTime(args[1]), Convert.ToDateTime(args[2]));
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error in Log Uploading: " + e.Message);
                }
            }
            else if (args.Length == 5) //LOGUPLOADING 07/13/2019 07/13/2019 00640 1
            {
                try
                {
                    if (Convert.ToByte(args[4]) == 1)
                    {
                        Globals.isOverwrite = true;
                        _log.WriteLog(Application.StartupPath, "Posting", string.Format("Range date posting = {0} to {1}, Employee ID = {2}, With Overwrite", args[1], args[2], args[3]), "", true);
                    }
                    else
                    {
                        Globals.isOverwrite = false;
                        _log.WriteLog(Application.StartupPath, "Posting", string.Format("Range date posting = {0} to {1}, Employee ID = {2}", args[1], args[2], args[3]), "", true);
                    }

                    if (Globals.isAutoRepostLogsViaLogControl == false)
                        NewLogPosting(Convert.ToDateTime(args[1]).AddDays(-1), Convert.ToDateTime(args[2]), args[3]);
                    else
                        NewLogPosting(Convert.ToDateTime(args[1]), Convert.ToDateTime(args[2]), args[3]);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error in Log Uploading: " + e.Message);
                }
            }
            else
            {
                Globals.isServicePost = true; //will execute text file and previous day posting
                _log.WriteLog(Application.StartupPath, "Posting", "Service on date posting", "", true);

                if (Globals.isAutoRepostLogsViaLogControl == false)
                    NewLogPosting(DateTime.Now.AddDays(-1), DateTime.Now);
                else
                    NewLogPosting(DateTime.Now, DateTime.Now);

            }
        }

        public DateTime LedgerMinDate()
        { 
            return LedgerMinDate("");
        }

        public DateTime LedgerMinDate(string LedgerDBName)
        {
            DateTime RetDate = DateTime.UtcNow;
            try
            {
                if (string.IsNullOrEmpty(LedgerDBName))
                {
                    //NonConfi Ledger
                    string query = @"SELECT min(DateAdd(DD,+1,Ttr_Date)) LedgerMinDate from .dbo.T_EmpTimeRegister";
                    DALHelper dal = new DALHelper("");
                    dal.OpenDB();
                    RetDate = Convert.ToDateTime(dal.ExecuteDataSet(query).Tables[0].Rows[0]["LedgerMinDate"]);
                    dal.CloseDB();
                }
                else
                {
                    //Specific Ledger
                    string query = @"SELECT min(DateAdd(DD,+1,Ttr_Date)) LedgerMinDate from .dbo.T_EmpTimeRegister";
                    DALHelper dal = new DALHelper(LedgerDBName, false);
                    dal.OpenDB();
                    RetDate = Convert.ToDateTime(dal.ExecuteDataSet(query).Tables[0].Rows[0]["LedgerMinDate"]);
                    dal.CloseDB();
                }
                
            }
            catch (Exception ex) { }
            return RetDate;
        }

        public class clsEmpDTR
        {
            public int Seq { get; set; }
            public string IDNo { get; set; }
            public DateTime LogDate { get; set; }
            public string LogTime { get; set; }
            public int LogTimeMinsOrig { get; set; }
            public int LogTimeMins { get; set; }
            public string LogType { get; set; }
            public bool PostFlag { get; set; }
            public int PocketNo { get; set; }
            public string LogRemark { get; set; }
            public string Remarks { get; set; }
            public bool Edited { get; set; }
        }
        public enum enumEmpDTR { IN, OUT };

        List<clsEmpDTR> lstEmpDTR = new List<clsEmpDTR>();
        #endregion

        #region Private Methods

        private void ShiftingManipuationFuntions(String ledgerDBName, String ledgerCompanyCode, DALHelper dal)
        {
            try
            {
                try
                {
                    Globals.LedgerDBName = ledgerDBName;
                }
                catch (Exception e)
                {
                    _log.WriteLog(Application.StartupPath, "Posting", "UploadTheLogs : Checking Parameters", e.ToString(), true);
                }

                //LogPostingFunctions.OvertimeAfterMidnightCapturing(_processDate, ledgerDBName, dal); remove here : this must be done lastly

                #region Setting shift code manipulation dataset

                //LogPostingFunctions.FlexShiftSetDataSet(_processDate, ledgerDBName, dal); //Accumulate flex shifting 
                LogPostingFunctions.ManipulateShiftCodeSetDataSet(_processDate, ledgerDBName, dal); //Custom Policy per client shift manipulation
                LogPostingFunctions.ShiftCodeManipulationFunction(ledgerDBName, dal);  //Start shift code manipulation base on accumated dataset
                #endregion

                LogPostingFunctions.UpdateEmployeeAdvanceOTApp(_processDate, ledgerDBName, ledgerCompanyCode, dal);
            }
            catch (Exception e)
            {
                _log.WriteLog(Application.StartupPath, "Posting", "UploadTheLogs : LedgerDBNameLoop2ndRun", e.ToString(), true);
            }  
        }

        private void NewLogPostingFunctions(String ledgerDBName, String ledgerCompanyCode)
        {
            try
            {
                dtrDB.OpenDB();
                
                #region Recovery batch dataset : Inverse shift posting

                //This functionality is specified for HOYA having LIFO (Last IN / First Out) Policy.
                LogPostingFunctions.IntelligentInversePostingPreviousDateSetDataSet(_processDate, ledgerDBName, dtrDB);      //Force Posting of graveyard logs having Regular shift - MUST BE LAST TO BE DONE [Intended for HOGP]
                
                LogPostingFunctions.LogpostingCounterMeasureFunction(ledgerDBName, dtrDB);  //Start updating ledger based on accumulated dataset

                #endregion

                #region Shift Manipulation and Overtime

                ShiftingManipuationFuntions(ledgerDBName, ledgerCompanyCode, dtrDB); // Overtime after midnight and Autoshift/Flexshit Manipulations

                LogPostingFunctions.OvertimeAfterMidnightCapturing(_processDate, ledgerDBName, ledgerCompanyCode, dtrDB);  //This is separate funtion to be done lastly

                #endregion

                #region Log trail recovery function

                //LogPostingFunctions.RepostLogTrail(ledgerDBName, _processDate, dtrDB); //Repost all edited from log trail
                
                LogPostingFunctions.LogpostingCounterMeasureFunction(ledgerDBName, dtrDB);  //Start updating ledger based on accumulated dataset

                #endregion

            }
            catch (Exception err)
            {
                _log.WriteLog(Application.StartupPath, "Posting", "NewLogPostingFunctions\n" + err.Message, DateTime.UtcNow.ToLongDateString(), true);
            }
            finally
            {
                dtrDB.CloseDB();
                dtrDB.Dispose();
            }
        }

        #endregion

        #region New Log Posting Function
        public void NewLogPosting(DateTime processDate, DateTime UntilDate)
        {
            NewLogPosting(processDate, UntilDate, "");
        }

        public void NewLogPosting(DateTime processDate, DateTime UntilDate, string EmployeeID)
        {
            List<string> ledgerDBNames = LogPostingFunctions.GetListOfDBProfiles();
            string ErrorMessages = "";
            int PostingDtr = 0;
            DateTime DateRun = DateTime.Now;

            try
            {
                dtrDB.OpenDB();
                dtrDB.BeginTransaction();
                DataTable dtPayPeriod;

                Console.WriteLine("\nLOG POSTING STARTED.");
                _log.WriteLog(Application.StartupPath, "Posting", "Posting Service Started", "", true);
                foreach (string PayrollDBName in ledgerDBNames)
                {
                    string[] strArr = PayrollDBName.Split(new char[] { '|' });
                    string ledgerDBName = strArr[0];
                    string ledgerCompanyCode = strArr[1];
                    Console.WriteLine(string.Format("\n\nPosting logs in {0} database...", ledgerDBName));

                    #region Version updating
                    try
                    {
                        string AppVersion = Assembly.GetEntryAssembly().GetName().Version.ToString();
                        commonBL.UpdateVersion("LOGPOSTING", AppVersion, "LOGUPLDSRVC", ledgerCompanyCode);
                    }
                    catch (Exception err)
                    {
                        _log.WriteLog(Application.StartupPath, "Posting", "Posting Application Version", err.Message, true);
                    }
                    #endregion

                    #region Set Parameters
                    Globals.TIMEGAP         = Convert.ToDouble(commonBL.GetParameterValueFromPayroll("TIMEGAP", ledgerCompanyCode, ledgerDBName));
                    Globals.EXTENDIN1       = Convert.ToInt32(commonBL.GetParameterValueFromPayroll("EXTENDIN1", ledgerCompanyCode, ledgerDBName));
                    Globals.POCKETTIME      = Convert.ToInt32(commonBL.GetParameterValueFromPayroll("POCKETTIME", ledgerCompanyCode, ledgerDBName));
                    Globals.POCKETGAP       = Convert.ToInt32(commonBL.GetParameterValueFromPayroll("POCKETGAP", ledgerCompanyCode, ledgerDBName));
                    Globals.TKADJCYCLE      = Convert.ToInt32(commonBL.GetParameterValueFromPayroll("TKADJCYCLE", ledgerCompanyCode, ledgerDBName));
                    Globals.POCKETSIZE      = Convert.ToInt32(commonBL.GetParameterValueFromPayroll("POCKETSIZE", ledgerCompanyCode, ledgerDBName));
                    Globals.LATEMAX2        = Convert.ToInt32(commonBL.GetParameterValueFromPayroll("LATEMAX2", ledgerCompanyCode, ledgerDBName));
                    Globals.LOGPOSTINGTYPE  = commonBL.GetParameterValueFromPayroll("LOGPOSTINGTYPE", ledgerCompanyCode, ledgerDBName);
                    //Globals.TIMEPOLICY      = commonBL.GetParameterValueFromPayroll("TIMEPOLICY", ledgerCompanyCode, ledgerDBName);
                    #endregion

                    #region Repost Logs via Log Control table - Basic Four Only
                    if (Globals.isAutoRepostLogsViaLogControl == true)
                    {
                        Console.Write("\nChecking for unposted logs...");
                        CreateLogControlRecordIfNotExists(ledgerDBName, dtrDB);

                        dtPayPeriod = GetMinimumPasPeriods(ledgerDBName, ledgerCompanyCode, dtrDB);
                        if (dtPayPeriod.Rows.Count > 0)
                        {
                            foreach (DataRow drPP in dtPayPeriod.Rows)
                            {
                                _processDate = Convert.ToDateTime(drPP["Tps_StartCycle"]);
                                while (_processDate < processDate && _processDate <= Convert.ToDateTime(drPP["Tps_EndCycle"]))
                                {
                                    Console.WriteLine("");
                                    if (Globals.bLogTypePostingEnable)
                                        PostingDtr += PostLogs(ledgerDBName, ledgerCompanyCode, _processDate, true, EmployeeID, dtrDB);
                                    else
                                        PostingDtr += PostLogsWOLogType(ledgerDBName, ledgerCompanyCode, _processDate, true, EmployeeID, dtrDB);

                                    _processDate = _processDate.AddDays(1);
                                }
                            }
                        }

                        dtPayPeriod = GetCurrentPayPeriod(ledgerDBName, dtrDB);
                        if (dtPayPeriod.Rows.Count > 0)
                        {
                            _processDate = Convert.ToDateTime(dtPayPeriod.Rows[0]["Tps_StartCycle"]);
                            while (_processDate < processDate && _processDate <= Convert.ToDateTime(dtPayPeriod.Rows[0]["Tps_EndCycle"]))
                            {
                                Console.WriteLine("");
                                if (Globals.bLogTypePostingEnable)
                                    PostingDtr += PostLogs(ledgerDBName, ledgerCompanyCode, _processDate, true, EmployeeID, dtrDB);
                                else
                                    PostingDtr += PostLogsWOLogType(ledgerDBName, ledgerCompanyCode, _processDate, true, EmployeeID, dtrDB);

                                _processDate = _processDate.AddDays(1);
                            }
                        }

                        dtPayPeriod = GetNextPayPeriod(ledgerDBName, dtrDB);
                        if (dtPayPeriod.Rows.Count > 0)
                        {
                            _processDate = Convert.ToDateTime(dtPayPeriod.Rows[0]["Tps_StartCycle"]);
                            while (_processDate < processDate && _processDate <= Convert.ToDateTime(dtPayPeriod.Rows[0]["Tps_EndCycle"]))
                            {
                                Console.WriteLine("");
                                if (Globals.bLogTypePostingEnable)
                                    PostingDtr += PostLogs(ledgerDBName, ledgerCompanyCode, _processDate, true, EmployeeID, dtrDB);
                                else
                                    PostingDtr += PostLogsWOLogType(ledgerDBName, ledgerCompanyCode, _processDate, true, EmployeeID, dtrDB);

                                _processDate = _processDate.AddDays(1);
                            }
                        }
                    }
                    #endregion

                    #region Text File and Log Posting
                    Console.Write(string.Format("\n\nNow posting logs from {0} to {1}...", processDate.ToShortDateString(), UntilDate.ToShortDateString()));
                    _processDate = Convert.ToDateTime(processDate.ToShortDateString());
                    while (_processDate <= Convert.ToDateTime(UntilDate.ToShortDateString()))
                    {
                        #region Text file Conversion
                        if (Globals.isTextFilePosting)
                        {
                            try
                            {
                                //Manual Uploaded Logs
                                LogPostingFunctions.DtrTextFileHandler(ConfigurationManager.AppSettings["TextFileLogsDirectory"], _processDate, dtrDB);
                                //FTP Uploaded logs
                                LogPostingFunctions.DtrTextFileHandler(ConfigurationManager.AppSettings["FTPTextFileFolder"], _processDate, dtrDB);
                            }
                            catch (Exception err)
                            {
                                ErrorMessages = ErrorMessages + err.Message + " " + Environment.NewLine;
                                _log.WriteLog(Application.StartupPath, "Posting", err.Message, "", true);
                            }
                        }
                        #endregion

                        Console.WriteLine("");

                        if (Globals.POCKETSIZE > 2)
                        {
                            #region Multiple Pockets
                            Globals.isOverwrite = true;
                            PostingDtr += PostLogsBasic4PlusExtension(ledgerDBName, ledgerCompanyCode, _processDate, true, EmployeeID, dtrDB);
                            #endregion
                        }
                        else
                        {
                            if (Globals.bLogTypePostingEnable)
                                PostingDtr += PostLogs(ledgerDBName, ledgerCompanyCode, _processDate, false, EmployeeID, dtrDB);
                            else
                                PostingDtr += PostLogsWOLogType(ledgerDBName, ledgerCompanyCode, _processDate, false, EmployeeID, dtrDB);
                        }
                        _processDate = _processDate.AddDays(1);
                    }
                    #endregion
                }

                #region Sending Email Notification
                Console.Write("\n\nSending Email Notification...");
                string Greeting = "Good Day!";
                if (DateTime.Now.Hour < 12)
                {
                    Greeting = "Good Morning";
                }
                else if (DateTime.Now.Hour == 12)
                {
                    Greeting = "Good Noon";
                }
                else if (DateTime.Now.Hour < 17)
                {
                    Greeting = "Good Afternoon";
                }
                else
                {
                    Greeting = "Good Evening";
                }

                #region Email Message
                string notifyMessage = string.Format(@"
{0}!



This is a system-generated message to inform you that the log posting process is finished.


Processing Statistics:
Start Date and Time: {1} Finish Date and Time: {2}

No. of DTR records processed: {3}

{4}



Truly Yours,

SSAS Support",
                Greeting,
                DateRun.ToString(),
                DateTime.Now.ToString(),
                PostingDtr,
                (string.IsNullOrEmpty(ErrorMessages) ? "" : string.Format("Warning : " + ErrorMessages)));
                #endregion

                conEWSSNotification wf = new conEWSSNotification();
                conEWSSNotification.SendEmail(CommonProcedures.GetAppSettingConfigString("FROM", "", false),
                                                    CommonProcedures.GetAppSettingConfigString("DTRemail", "", false),
                                                    "Log Posting Notification " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToString("hh:mm tt"),
                                                    notifyMessage);
                #endregion

                Console.Write("\n\nSaving All Changes...");
                dtrDB.CommitTransaction();

                _log.WriteLog(Application.StartupPath, "Posting", "Posting Service Committed", "", true);
                Console.WriteLine("\n\nLOG POSTING TRANSACTION COMMITTED.. HAVE A NICE DAY!");
            }
            catch (Exception ex)
            {
                dtrDB.RollBackTransaction();
                Console.WriteLine(string.Format("\nLOG POSTING ENCOUNTERED ERRORS.. {0}", ex.Message));
            }
            finally
            {
                dtrDB.CloseDB();
            }

        }

        public int PostLogs(string strDBName, string strCompanyCode, DateTime dtProcessDate, bool bPostPartial, string strSelectedEmployeeID, DALHelper dal)
        {
            #region Variables
            DataRow[] drArrLogLedger, drArrDTR, drArrLogTrail, drArrTimeMod, drDTROverride;
            string strEmployeeID, strScheduleType;
            int iActualTimeIn1Min, iActualTimeOut1Min, iActualTimeIn2Min, iActualTimeOut2Min;
            int iTrailTimeIn1Min, iTrailTimeOut1Min, iTrailTimeIn2Min, iTrailTimeOut2Min;
            int iShiftTimeIn1Min, iShiftTimeOut1Min, iShiftTimeIn2Min, iShiftTimeOut2Min;
            DateTime dtDTRDate;
            int iLogTime, iLogTimeOrig, iLogTimeTemp, iActualLogOutTemp;
            int iLatestLogTimeIN, iLatestLogTimeOUT;
            string strLogType, strLatestValidLogType, strLastLogType;
            bool bPostFlag, bEdited, bTimeGapValid, bFoundFirstIN, bNoLogIN, bIsInHistTable = false;
            const int GRAVEYARD24 = 1440;
            #endregion

            Console.WriteLine("\nStart: {0:M/dd/yyyy HH:mm:ss} ", DateTime.Now);
            Console.WriteLine("{0} Getting Log Ledger records...", dtProcessDate.ToShortDateString());
            dtEmployeeLogLedger = GetLogLedgerRecordsPerDate(strDBName, strCompanyCode, dtProcessDate, bPostPartial, strSelectedEmployeeID, dal);
            Console.WriteLine("{0} Getting DTR records...", dtProcessDate.ToShortDateString());
            dtDTR = GetDTRRecordsPerDate(dtProcessDate, strSelectedEmployeeID, dal);
            Console.WriteLine("{0} Getting Log Trail records...", dtProcessDate.ToShortDateString());
            dtLogTrail = GetLogTrailRecordsPerDate(strDBName, dtProcessDate, strSelectedEmployeeID, dal);
            Console.WriteLine("{0} Getting DTR Override records...", dtProcessDate.ToShortDateString());
            dtDtrOverride = GetDtrOverrideRecordsPerDate(strDBName, dtProcessDate, strSelectedEmployeeID, dal);
            Console.WriteLine("{0} Getting Time Correction records...", dtProcessDate.ToShortDateString());
            dtTimeCor = GetTimeCorrectionRecordsPerDate(strDBName, dtProcessDate, strSelectedEmployeeID, dal);

            for (int i = 0; i < dtEmployeeLogLedger.Rows.Count; i++)
            {
                Console.Write("\r{0} Posting logs {1} of {2}...", dtProcessDate.ToShortDateString(), i + 1, dtEmployeeLogLedger.Rows.Count);

                #region Initialize Variables
                bEdited = false;
                bFoundFirstIN = false;
                bNoLogIN = false;
                iLatestLogTimeIN = -1;
                iLatestLogTimeOUT = -1;
                strLatestValidLogType = "";
                strLastLogType = "";
                strEmployeeID = dtEmployeeLogLedger.Rows[i]["Ttr_IDNo"].ToString();
                strScheduleType = dtEmployeeLogLedger.Rows[i]["ScheduleType"].ToString();

                iActualTimeIn1Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttr_ActIn_1"].ToString());
                iActualTimeOut1Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttr_ActOut_1"].ToString());
                iActualTimeIn2Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttr_ActIn_2"].ToString());
                iActualTimeOut2Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttr_ActOut_2"].ToString());

                iShiftTimeIn1Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["ShiftTimeIn"].ToString());
                iShiftTimeOut1Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["ShiftBreakStart"].ToString());
                iShiftTimeIn2Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["ShiftBreakEnd"].ToString());
                iShiftTimeOut2Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["ShiftTimeOut"].ToString());

                if (dtEmployeeLogLedger.Rows[i]["TableName"].ToString() == "T_EmpTimeRegisterHst")
                    bIsInHistTable = true;

                //Convert to Graveyard
                if (iShiftTimeIn1Min > iShiftTimeOut1Min)
                    iShiftTimeOut1Min = iShiftTimeOut1Min + GRAVEYARD24;
                if (iShiftTimeIn1Min > iShiftTimeIn2Min)
                    iShiftTimeIn2Min = iShiftTimeIn2Min + GRAVEYARD24;
                if (iShiftTimeIn1Min > iShiftTimeOut2Min)
                    iShiftTimeOut2Min = iShiftTimeOut2Min + GRAVEYARD24;

                drArrDTR = dtDTR.Select(string.Format("EmployeeID = '{0}'", strEmployeeID));
                #endregion

                #region Loop through DTR Logs (Regular FILO Posting)
                for (int x = 0; x < drArrDTR.Length; x++)
                {
                    #region Initialize Variables
                    dtDTRDate = Convert.ToDateTime(drArrDTR[x]["LogDate"].ToString());
                    iLogTime = GetMinsFromHourStr(drArrDTR[x]["LogTime"].ToString());
                    iLogTimeOrig = iLogTime;
                    strLogType = drArrDTR[x]["LogType"].ToString();
                    bPostFlag = Convert.ToBoolean(drArrDTR[x]["PostFlag"].ToString());
                    #endregion

                    if (strLogType == "I") //IN
                    {
                        #region IN
                        if (iLogTime == 0)
                            iLogTimeOrig = iLogTime = GRAVEYARD24 - 1;

                        if (iLogTime != 0 && dtProcessDate.AddDays(1) == dtDTRDate) //IN on next day
                            iLogTime = iLogTime + GRAVEYARD24;

                        if (iLogTime != 0 && iLogTime <= iShiftTimeOut1Min) //IN1
                        {
                            if (iActualTimeIn1Min == 0 || (dtProcessDate == dtDTRDate && iLogTime < iActualTimeIn1Min))
                            {
                                iActualTimeIn1Min = iLogTimeOrig;
                                drArrDTR[x]["PostFlag"] = "True";
                                drArrDTR[x]["Edited"] = 1;
                                bEdited = true;
                            }
                            if (iActualTimeIn1Min > 0)
                            {
                                iLatestLogTimeIN = iActualTimeIn1Min;
                                strLatestValidLogType = "I";
                                bFoundFirstIN = true;
                            }
                        }
                        else if (iLogTime != 0 && iLogTime > iShiftTimeOut1Min && iLogTime < iShiftTimeOut2Min - 60) //IN2
                        {
                            if ((iActualTimeIn2Min == 0 || (dtProcessDate == dtDTRDate && iLogTime < iActualTimeIn2Min))
                                && (iActualTimeIn1Min == 0 || (dtProcessDate == dtDTRDate && iLogTime > iActualTimeIn1Min)))
                            {
                                iActualTimeIn2Min = iLogTimeOrig;
                                drArrDTR[x]["PostFlag"] = "True";
                                drArrDTR[x]["Edited"] = 1;
                                bEdited = true;
                            }
                            if (iActualTimeIn2Min > 0)
                            {
                                iLatestLogTimeIN = iActualTimeIn2Min;
                                strLatestValidLogType = "I";
                                bFoundFirstIN = true;
                            }
                        }
                        #endregion
                    }
                    else if (strLogType == "O") //OUT
                    {
                        #region OUT
                        if (iLogTime == 0)
                            iLogTimeOrig = iLogTime = 1;

                        if (iLogTime != 0 && dtProcessDate.AddDays(1) == dtDTRDate) //OUT on next day
                            iLogTime = iLogTime + GRAVEYARD24;

                        #region Check for TIMEGAP Validity
                        bTimeGapValid = true;
                        for (int y = x + 1; y < drArrDTR.Length && bTimeGapValid == true; y++)
                        {
                            if (drArrDTR[y]["LogType"].ToString() == "I") //Get the Next IN and validate against TIMEGAP
                            {
                                iLogTimeTemp = GetMinsFromHourStr(drArrDTR[y]["LogTime"].ToString());
                                if (iLogTimeTemp != 0 && dtProcessDate.AddDays(1) == Convert.ToDateTime(drArrDTR[y]["LogDate"].ToString()))
                                    iLogTimeTemp = iLogTimeTemp + GRAVEYARD24;
                                if (iLogTimeTemp - iLogTime < Globals.TIMEGAP)
                                    bTimeGapValid = false;
                            }
                        }
                        if (strLatestValidLogType == "I")
                        {
                            if (iLogTime - iLatestLogTimeIN < Globals.TIMEGAP)
                                bTimeGapValid = false;
                        }
                        #endregion

                        #region Check if Previous IN belongs to next day
                        if (strLatestValidLogType == "")
                            bFoundFirstIN = false; //initialize
                        for (int y = x - 1; y >= 0 && bFoundFirstIN == true; y--)
                        {
                            if (drArrDTR[y]["LogType"].ToString() == "I") //Get the Previous IN
                            {
                                iLogTimeTemp = GetMinsFromHourStr(drArrDTR[y]["LogTime"].ToString());
                                if (iLogTimeTemp != 0 && dtProcessDate.AddDays(1) == Convert.ToDateTime(drArrDTR[y]["LogDate"].ToString()))
                                    bFoundFirstIN = false;
                            }
                        }
                        //Check Previous Day DTR if OUT is found; In this case, the employee forgot to login on the current date
                        bNoLogIN = false; //initialize
                        if ((strLatestValidLogType == "" || strLastLogType == "O")
                            && dtProcessDate == dtDTRDate
                            && iLogTime >= iShiftTimeOut1Min)
                        {
                            bNoLogIN = true;
                        }
                        #endregion

                        if (iActualTimeIn1Min == 0 || iActualTimeOut2Min == 0 || iActualTimeOut2Min > iActualTimeIn1Min)
                            iActualLogOutTemp = iActualTimeOut2Min;
                        else
                            iActualLogOutTemp = iActualTimeOut2Min + GRAVEYARD24;

                        if (iLogTime != 0 && iLogTime <= iShiftTimeIn2Min && iLogTime > iShiftTimeIn1Min) //OUT1
                        {
                            if ((iActualTimeOut1Min == 0 || (dtProcessDate == dtDTRDate && iLogTime > iActualTimeOut1Min))
                                && (iActualTimeOut2Min == 0 || (dtProcessDate == dtDTRDate && iLogTime < iActualLogOutTemp))
                                && bTimeGapValid == true
                                && (bFoundFirstIN == true || bNoLogIN == true))
                            {
                                iActualTimeOut1Min = iLogTimeOrig;
                                drArrDTR[x]["PostFlag"] = "True";
                                drArrDTR[x]["Edited"] = 1;
                                bEdited = true;
                            }
                            if (iActualTimeOut1Min > 0)
                            {
                                iLatestLogTimeOUT = iActualTimeOut1Min;
                                strLatestValidLogType = "O";
                            }
                        }
                        else if (iLogTime != 0 && iLogTime > iShiftTimeIn2Min) //OUT2 
                        {
                            if (((iActualTimeOut2Min == 0 && iLogTime <= iShiftTimeOut1Min + GRAVEYARD24) //Do not post if greater than Shift Break of Next Day
                                || (strScheduleType != "G" && dtProcessDate == dtDTRDate && iLogTime > iActualLogOutTemp)
                                || (strScheduleType == "G" && iLogTime > iActualLogOutTemp))
                                && bTimeGapValid == true
                                && (bFoundFirstIN == true || bNoLogIN == true))
                            {
                                if (dtProcessDate == dtDTRDate)
                                    iActualTimeOut2Min = iLogTimeOrig;
                                else
                                    iActualTimeOut2Min = iLogTime; //+2400 if Next Day
                                drArrDTR[x]["PostFlag"] = "True";
                                drArrDTR[x]["Edited"] = 1;
                                bEdited = true;
                            }
                            if (iActualTimeOut2Min > 0)
                            {
                                iLatestLogTimeOUT = iActualTimeOut2Min;
                                strLatestValidLogType = "O";
                            }
                        }

                        //Unset Post Flag for Previous OUT records
                        for (int y = x - 1; y >= 0; y--)
                        {
                            if (drArrDTR[y]["LogType"].ToString() == "O")
                            {
                                iLogTimeTemp = GetMinsFromHourStr(drArrDTR[y]["LogTime"].ToString());
                                if (iLogTimeTemp == 0)
                                    iLogTimeTemp = 1;
                                if (iLogTimeTemp != 0 && dtProcessDate.AddDays(1) == Convert.ToDateTime(drArrDTR[y]["LogDate"])) //OUT on next day
                                    iLogTimeTemp = iLogTimeTemp + GRAVEYARD24;

                                if (iLogTimeTemp != iActualTimeOut1Min && iLogTimeTemp != iActualTimeOut2Min)
                                    drArrDTR[y]["Edited"] = 0;
                            }
                        }
                        #endregion
                    }

                    strLastLogType = strLogType;
                }
                #endregion

                #region Loop through DTR Logs (Soft Posting)
                if (Globals.isSoftPostingEnable == true)
                {
                    //Perform Soft Posting only on unpaired logs or no logs at all, or when logs are not aligned with shift
                    if ((((iActualTimeIn1Min != 0 && iActualTimeOut1Min != 0 && iActualTimeIn2Min != 0 && iActualTimeOut2Min != 0) //With IN1,OUT1,IN2,OUT2
                        || (iActualTimeIn1Min != 0 && iActualTimeOut1Min != 0) //With IN1,OUT1
                        || (iActualTimeIn2Min != 0 && iActualTimeOut2Min != 0) //With IN2,OUT2
                        || (iActualTimeIn1Min != 0 && iActualTimeOut2Min != 0)) //With IN1,OUT2
                            == false)
                        ||
                            ((strScheduleType == "G" && iActualTimeIn1Min < GRAVEYARD24 && iActualTimeOut2Min > 0 && iActualTimeOut2Min < GRAVEYARD24) //Graveyard shift but logs are not graveyard
                            || (strScheduleType != "G" && iActualTimeOut2Min > 0 && iActualTimeOut2Min < iActualTimeIn1Min)) //Day/Normal/Swing shift but logs are graveyard
                        )
                    {
                        //Initialize Variables
                        bFoundFirstIN = false;
                        iLatestLogTimeIN = -1;
                        iLatestLogTimeOUT = -1;
                        strLatestValidLogType = "";
                        strLastLogType = "";

                        for (int x = 0; x < drArrDTR.Length; x++)
                        {
                            #region Initialize Variables
                            dtDTRDate = Convert.ToDateTime(drArrDTR[x]["LogDate"].ToString());
                            iLogTime = GetMinsFromHourStr(drArrDTR[x]["LogTime"].ToString());
                            iLogTimeOrig = iLogTime;
                            strLogType = drArrDTR[x]["LogType"].ToString();
                            bPostFlag = Convert.ToBoolean(drArrDTR[x]["PostFlag"].ToString());
                            #endregion

                            //Soft Posting only posts logs in IN1 and OUT2
                            if (strLogType == "I") //IN
                            {
                                #region IN
                                if (iLogTime == 0)
                                    iLogTimeOrig = iLogTime = GRAVEYARD24 - 1;

                                if (iLogTime != 0
                                    && iLatestLogTimeIN == -1
                                    && dtProcessDate == dtDTRDate) //IN1
                                {
                                    iLatestLogTimeIN = iLogTimeOrig;
                                    drArrDTR[x]["PostFlag"] = "True";
                                    drArrDTR[x]["Edited"] = 1;
                                    bEdited = true;
                                    strLatestValidLogType = "I";
                                    bFoundFirstIN = true;
                                }
                                #endregion
                            }
                            else if (strLogType == "O") //OUT
                            {
                                #region OUT
                                if (iLogTime == 0)
                                    iLogTimeOrig = iLogTime = 1;

                                if (iLogTime != 0 && dtProcessDate.AddDays(1) == dtDTRDate) //OUT on next day
                                    iLogTime = iLogTime + GRAVEYARD24;

                                #region Check for TIMEGAP Validity
                                bTimeGapValid = true;
                                for (int y = x + 1; y < drArrDTR.Length && bTimeGapValid == true; y++)
                                {
                                    if (drArrDTR[y]["LogType"].ToString() == "I") //Get the Next IN and validate against TIMEGAP
                                    {
                                        iLogTimeTemp = GetMinsFromHourStr(drArrDTR[y]["LogTime"].ToString());
                                        if (iLogTimeTemp != 0 && dtProcessDate.AddDays(1) == Convert.ToDateTime(drArrDTR[y]["LogDate"].ToString()))
                                            iLogTimeTemp = iLogTimeTemp + GRAVEYARD24;
                                        if (iLogTimeTemp - iLogTime < Globals.TIMEGAP)
                                            bTimeGapValid = false;
                                    }
                                }
                                if (strLatestValidLogType == "I")
                                {
                                    if (iLogTime - iLatestLogTimeIN < Globals.TIMEGAP)
                                        bTimeGapValid = false;
                                }
                                #endregion

                                #region Check if Previous IN belongs to next day
                                if (strLatestValidLogType == "")
                                    bFoundFirstIN = false; //initialize
                                for (int y = x - 1; y >= 0 && bFoundFirstIN == true; y--)
                                {
                                    iLogTimeTemp = GetMinsFromHourStr(drArrDTR[y]["LogTime"].ToString());
                                    if (iLogTimeTemp != 0 && dtProcessDate.AddDays(1) == Convert.ToDateTime(drArrDTR[y]["LogDate"].ToString()))
                                        iLogTimeTemp = iLogTimeTemp + GRAVEYARD24;

                                    if (drArrDTR[y]["LogType"].ToString() == "I") //Get the Previous IN
                                    {
                                        if (iLogTimeTemp != 0 && dtProcessDate.AddDays(1) == Convert.ToDateTime(drArrDTR[y]["LogDate"].ToString()))
                                            bFoundFirstIN = false;
                                    }
                                    else if (drArrDTR[y]["LogType"].ToString() == "O") //Get the Previous OUT to check if it is the last OUT
                                    {
                                        if (strLatestValidLogType == "O" && iLogTimeTemp == iLatestLogTimeOUT && iLogTime - iLatestLogTimeOUT > 240) //Next OUT is greater than 4 hours
                                            bFoundFirstIN = false;
                                    }
                                }
                                #endregion

                                if (iLatestLogTimeIN == -1 || iLatestLogTimeOUT == -1 || iLatestLogTimeOUT > iLatestLogTimeIN)
                                    iActualLogOutTemp = iLatestLogTimeOUT;
                                else
                                    iActualLogOutTemp = iLatestLogTimeOUT + GRAVEYARD24;

                                if ((iLogTime != 0 && iLogTime <= iShiftTimeOut1Min + GRAVEYARD24) //Do not post if greater than Shift Break of Next Day
                                    && (iLatestLogTimeOUT == -1 || iLogTime >= iActualLogOutTemp)
                                    && bTimeGapValid == true
                                    && bFoundFirstIN == true) //OUT2
                                {
                                    if (dtProcessDate == dtDTRDate)
                                        iLatestLogTimeOUT = iLogTimeOrig;
                                    else
                                        iLatestLogTimeOUT = iLogTime; //+2400 if Next Day
                                    drArrDTR[x]["PostFlag"] = "True";
                                    drArrDTR[x]["Edited"] = 1;
                                    bEdited = true;
                                    strLatestValidLogType = "O";
                                }
                                #endregion
                            }

                            strLastLogType = strLogType;
                        }

                        #region Save Values Only if Paired
                        if (iLatestLogTimeIN != -1 && iLatestLogTimeOUT != -1)
                        {
                            iActualTimeIn1Min = iLatestLogTimeIN;
                            if (iLatestLogTimeOUT <= iShiftTimeIn2Min) //OUT1
                            {
                                iActualTimeOut1Min = iLatestLogTimeOUT;
                                iActualTimeOut2Min = 0;
                            }
                            else //OUT2
                            {
                                iActualTimeOut2Min = iLatestLogTimeOUT;
                                iActualTimeOut1Min = 0;
                            }
                            bEdited = true;
                        }
                        #endregion
                    }
                }
                #endregion

                #region DTR Override Reposting
                drDTROverride = dtDtrOverride.Select(string.Format("Tdo_IDNo = '{0}'", strEmployeeID));
                if (drDTROverride.Length > 0)
                {
                    //Get First Sequence Record Only
                    iTrailTimeIn1Min = GetMinsFromHourStr(drDTROverride[0]["Tdo_Time"].ToString());

                    #region Post according to DTR Override Type
                    switch (drDTROverride[0]["Tdo_Type"].ToString())
                    {
                        case "I1": //I1-IN1
                            if (iTrailTimeIn1Min != 0)
                            {
                                iActualTimeIn1Min = iTrailTimeIn1Min;
                                bEdited = true;
                            }
                            break;
                        case "I2": //I2-IN2
                            if (iTrailTimeIn1Min != 0)
                            {
                                iActualTimeIn2Min = iTrailTimeIn1Min;
                                bEdited = true;
                            }
                            break;
                        case "O1": //O1-OUT1
                            if (iTrailTimeIn1Min != 0)
                            {
                                iActualTimeOut1Min = iTrailTimeIn1Min;
                                bEdited = true;
                            }
                            break;
                        case "O2": //O2-OUT2
                            if (iTrailTimeIn1Min != 0)
                            {
                                iActualTimeOut2Min = iTrailTimeIn1Min;
                                bEdited = true;
                            }
                            break;
                    }
                    #endregion

                }
                #endregion

                #region Time Correction Reposting
                drArrTimeMod = dtTimeCor.Select(string.Format("Ttm_IDNo = '{0}'", strEmployeeID));
                if (drArrTimeMod.Length > 0)
                {
                    //Get First Sequence Record Only
                    iTrailTimeIn1Min = GetMinsFromHourStr(drArrTimeMod[0]["Ttm_TimeIn1"].ToString());
                    iTrailTimeOut1Min = GetMinsFromHourStr(drArrTimeMod[0]["Ttm_TimeOut1"].ToString());
                    iTrailTimeIn2Min = GetMinsFromHourStr(drArrTimeMod[0]["Ttm_TimeIn2"].ToString());
                    iTrailTimeOut2Min = GetMinsFromHourStr(drArrTimeMod[0]["Ttm_TimeOut2"].ToString());

                    #region Post according to Time Modification Type
                    switch (drArrTimeMod[0]["Ttm_TimeCorType"].ToString())
                    {
                        case "IN": //No In - (Either IN1 OR IN2)
                            if (iTrailTimeIn1Min != 0)
                            {
                                iActualTimeIn1Min = iTrailTimeIn1Min;
                                bEdited = true;
                            }
                            if (iTrailTimeIn2Min != 0)
                            {
                                iActualTimeIn2Min = iTrailTimeIn2Min;
                                bEdited = true;
                            }
                            break;
                        case "IN1": //No In 1
                            if (iTrailTimeIn1Min != 0)
                            {
                                iActualTimeIn1Min = iTrailTimeIn1Min;
                                bEdited = true;
                            }
                            break;
                        case "I12": //No In 1 & In 2
                            if (iTrailTimeIn1Min != 0)
                            {
                                iActualTimeIn1Min = iTrailTimeIn1Min;
                                bEdited = true;
                            }
                            if (iTrailTimeIn2Min != 0)
                            {
                                iActualTimeIn2Min = iTrailTimeIn2Min;
                                bEdited = true;
                            }
                            break;
                        case "I1O2": //No In 1 & Out 2
                            if (iTrailTimeIn1Min != 0)
                            {
                                iActualTimeIn1Min = iTrailTimeIn1Min;
                                bEdited = true;
                            }
                            if (iTrailTimeOut2Min != 0)
                            {
                                iActualTimeOut2Min = iTrailTimeOut2Min;
                                bEdited = true;
                            }
                            break;
                        case "IN2": //No In 2 
                            if (iTrailTimeIn2Min != 0)
                            {
                                iActualTimeIn2Min = iTrailTimeIn2Min;
                                bEdited = true;
                            }
                            break;
                        case "OUT": //No Out - (Either OUT1 OR OUT2)
                            if (iTrailTimeOut1Min != 0)
                            {
                                iActualTimeOut1Min = iTrailTimeOut1Min;
                                bEdited = true;
                            }
                            if (iTrailTimeOut2Min != 0)
                            {
                                iActualTimeOut2Min = iTrailTimeOut2Min;
                                bEdited = true;
                            }
                            break;
                        case "O1": //No Out 1
                            if (iTrailTimeOut1Min != 0)
                            {
                                iActualTimeOut1Min = iTrailTimeOut1Min;
                                bEdited = true;
                            }
                            break;
                        case "O1O2": //No Out 1 & Out 2
                            if (iTrailTimeOut1Min != 0)
                            {
                                iActualTimeOut1Min = iTrailTimeOut1Min;
                                bEdited = true;
                            }
                            if (iTrailTimeOut2Min != 0)
                            {
                                iActualTimeOut2Min = iTrailTimeOut2Min;
                                bEdited = true;
                            }
                            break;
                        case "O2": //No Out 2
                            if (iTrailTimeOut2Min != 0)
                            {
                                iActualTimeOut2Min = iTrailTimeOut2Min;
                                bEdited = true;
                            }
                            break;
                        case "INOU": //No In and Out
                            if (iTrailTimeIn1Min != 0)
                            {
                                iActualTimeIn1Min = iTrailTimeIn1Min;
                                bEdited = true;
                            }
                            if (iTrailTimeIn2Min != 0)
                            {
                                iActualTimeIn2Min = iTrailTimeIn2Min;
                                bEdited = true;
                            }
                            if (iTrailTimeOut1Min != 0)
                            {
                                iActualTimeOut1Min = iTrailTimeOut1Min;
                                bEdited = true;
                            }
                            if (iTrailTimeOut2Min != 0)
                            {
                                iActualTimeOut2Min = iTrailTimeOut2Min;
                                bEdited = true;
                            }
                            break;
                    }
                    #endregion
                }
                #endregion

                #region Save Values
                if (bEdited == true)
                {
                    dtEmployeeLogLedger.Rows[i]["Ttr_ActIn_1"] = GetHourStrFromMins(iActualTimeIn1Min);
                    dtEmployeeLogLedger.Rows[i]["Ttr_ActOut_1"] = GetHourStrFromMins(iActualTimeOut1Min);
                    dtEmployeeLogLedger.Rows[i]["Ttr_ActIn_2"] = GetHourStrFromMins(iActualTimeIn2Min);
                    dtEmployeeLogLedger.Rows[i]["Ttr_ActOut_2"] = GetHourStrFromMins(iActualTimeOut2Min);
                    dtEmployeeLogLedger.Rows[i]["Edited"] = 1;
                }
                #endregion
            }

            #region Save Log Ledger Records
            Console.Write("\n{0} Saving log ledger...", dtProcessDate.ToShortDateString());
            drArrLogLedger = dtEmployeeLogLedger.Select("Edited = 1");
            string strUpdateQuery = "";
            int iUpdateCtr = 0;

            string strLogLedgerTable = "T_EmpTimeRegister";
            if (bIsInHistTable == true)
                strLogLedgerTable = "T_EmpTimeRegisterHst";

            string strTrailPrevious = "";
            string strUpdateTemplate = @"UPDATE {0}..{7} SET Ttr_ActIn_1 = '{3}', Ttr_ActOut_1 = '{4}', Ttr_ActIn_2 = '{5}', Ttr_ActOut_2 = '{6}', Usr_Login = 'LOGUPLOADING', Ludatetime = GETDATE() WHERE Ttr_IDNo = '{1}' AND Ttr_Date = '{2}' ";
            for (int x = 0; x < drArrLogLedger.Length; x++)
            {
                if (bIsInHistTable == true)
                {
                    #region Create Trail Records
                    #region Query
                    strTrailPrevious = string.Format(@" 
        DECLARE @AffectedPayPeriod VARCHAR(7) = (Select TOP(1) Tps_PayCycle 
                                                From {0}..T_PaySchedule 
                                                Where Tps_CycleIndicator = 'P'
                                                    And '{2}' BETWEEN Tps_StartCycle AND Tps_EndCycle
                                                ORDER BY Tps_StartCycle DESC)
        DECLARE @AdjustPayPeriod VARCHAR(7) = (Select Tps_PayCycle 
        									   From {0}..T_PaySchedule 
        									   Where Tps_CycleIndicator='C' 
        										and Tps_RecordStatus = 'A')

        --INSERT INTO LOG LEDGER TRAIL
        INSERT INTO {0}..T_EmpTimeRegisterTrl
        (
        	Ttr_IDNo
        	, Ttr_Date
        	, Ttr_AdjPayCycle
        	, Ttr_PayCycle
        	, Ttr_DayCode
        	, Ttr_ShiftCode
        	, Ttr_HolidayFlag
        	, Ttr_RestDayFlag
        	, Ttr_ActIn_1
        	, Ttr_ActOut_1
        	, Ttr_ActIn_2
        	, Ttr_ActOut_2
        	, Ttr_ConvIn_1Min
        	, Ttr_ConvOut_1Min
        	, Ttr_ConvIn_2Min
        	, Ttr_ConvOut_2Min
        	, Ttr_CompIn_1Min
        	, Ttr_CompOut_1Min
        	, Ttr_CompIn_2Min
        	, Ttr_CompOut_2Min
        	, Ttr_CompAdvOTMin
        	, Ttr_ShiftIn_1Min
        	, Ttr_ShiftOut_1Min
        	, Ttr_ShiftIn_2Min
        	, Ttr_ShiftOut_2Min
        	, Ttr_ShiftMin
        	, Ttr_ScheduleType
        	, Ttr_WFPayLVCode
        	, Ttr_WFPayLVHr
        	, Ttr_PayLVMin
        	, Ttr_ExcLVMin
        	, Ttr_WFNoPayLVCode
        	, Ttr_NoPayLVMin
        	, Ttr_NoPayLVMin
        	, Ttr_WFOTAdvHr
        	, Ttr_WFOTPostHr
        	, Ttr_OTMin
        	, Ttr_CompOTMin
        	, Ttr_OffsetOTMin
        	, Ttr_CompLT1Min
        	, Ttr_LTPostFlag
        	, Ttr_InitialABSMin
        	, Ttr_CompABSMin
        	, Ttr_CompREGMin
        	, Ttr_CompWorkMin
        	, Ttr_CompNDMin
        	, Ttr_CompNDOTMin
        	, Ttr_PrvDayWorkMin
        	, Ttr_PrvDayHolRef
        	, Ttr_GraveyardPostFlag
        	, Ttr_GrvPostBy
        	, Ttr_GrvPostDate
        	, Ttr_AssumedFlag
        	, Ttr_AssumedBy
        	, Ttr_AssumedDate
        	, Ttr_PaidLEGHour
        	, Ttr_ForceLeaveBy
        	, Ttr_ForceLeaveDate
        	, Ttr_ForOffsetMin
        	, Ttr_ExcOffset
        	, Ttr_EarnedSatOff
        	, Ttr_RESTLEGHOLDay
        	, Ttr_WorkDay
        	, Ttr_MealDay
        	, Ttr_EXPHour
        	, Ttr_ABSHour
        	, Ttr_REGHour
        	, Ttr_OTHour
        	, Ttr_NDHour
        	, Ttr_NDOTHour
        	, Ttr_LVHour
        	, Usr_Login
        	, Ludatetime
        	, Ttr_NextDayHour
        	, Ttr_TBAmt01
        	, Ttr_TBAmt02
        	, Ttr_TBAmt03
        	, Ttr_TBAmt04
        	, Ttr_TBAmt05
        	, Ttr_TBAmt06
        	, Ttr_TBAmt07
        	, Ttr_TBAmt08
        	, Ttr_TBAmt09
        	, Ttr_TBAmt10
        	, Ttr_TBAmt11
        	, Ttr_TBAmt12
        	, Ttr_WorkLocationCode
        	, Ttr_FlextimeFlag
        	, Ttr_TagFlextime
        	, Ttr_WFTimeMod
        	, Ttr_CalendarType
        	, Ttr_CalendarGroup
        	, Ttr_AssumedPost
        	, Ttr_InitialOffsetMin
        	, Ttr_AppliedOffsetMin
        	, Ttr_CompOffsetMin
        	, Ttr_CompLT2Min
        	, Ttr_CompUT1Min
        	, Ttr_CompUT2Min
        )
        Select A.Ttr_IDNo
        	,A.Ttr_Date
        	,@AdjustPayPeriod
        	,A.Ttr_PayCycle
        	,A.Ttr_DayCode
        	,A.Ttr_ShiftCode
        	,A.Ttr_HolidayFlag
        	,A.Ttr_RestDayFlag
        	,A.Ttr_ActIn_1
        	,A.Ttr_ActOut_1
        	,A.Ttr_ActIn_2
        	,A.Ttr_ActOut_2
        	,A.Ttr_ConvIn_1Min
        	,A.Ttr_ConvOut_1Min
        	,A.Ttr_ConvIn_2Min
        	,A.Ttr_ConvOut_2Min
        	,A.Ttr_CompIn_1Min
        	,A.Ttr_CompOut_1Min
        	,A.Ttr_CompIn_2Min
        	,A.Ttr_CompOut_2Min
        	,A.Ttr_CompAdvOTMin
        	,A.Ttr_ShiftIn_1Min
        	,A.Ttr_ShiftOut_1Min
        	,A.Ttr_ShiftIn_2Min
        	,A.Ttr_ShiftOut_2Min
        	,A.Ttr_ShiftMin
        	,A.Ttr_ScheduleType
        	,A.Ttr_WFPayLVCode
        	,A.Ttr_WFPayLVHr
        	,A.Ttr_PayLVMin
        	,A.Ttr_ExcLVMin
        	,A.Ttr_WFNoPayLVCode
        	,A.Ttr_NoPayLVMin
        	,A.Ttr_NoPayLVMin
        	,A.Ttr_WFOTAdvHr
        	,A.Ttr_WFOTPostHr
        	,A.Ttr_OTMin
        	,A.Ttr_CompOTMin
        	,A.Ttr_OffsetOTMin
        	,A.Ttr_CompLT1Min
        	,A.Ttr_LTPostFlag
        	,A.Ttr_InitialABSMin
        	,A.Ttr_CompABSMin
        	,A.Ttr_CompREGMin
        	,A.Ttr_CompWorkMin
        	,A.Ttr_CompNDMin
        	,A.Ttr_CompNDOTMin
        	,A.Ttr_PrvDayWorkMin
        	,A.Ttr_PrvDayHolRef
        	,A.Ttr_GraveyardPostFlag
        	,A.Ttr_GrvPostBy
        	,A.Ttr_GrvPostDate
        	,A.Ttr_AssumedFlag
        	,A.Ttr_AssumedBy
        	,A.Ttr_AssumedDate
        	,A.Ttr_PaidLEGHour
        	,A.Ttr_ForceLeaveBy
        	,A.Ttr_ForceLeaveDate
        	,A.Ttr_ForOffsetMin
        	,A.Ttr_ExcOffset
        	,A.Ttr_EarnedSatOff
        	,A.Ttr_RESTLEGHOLDay
        	,A.Ttr_WorkDay
        	,A.Ttr_MealDay
        	,A.Ttr_EXPHour
        	,A.Ttr_ABSHour
        	,A.Ttr_REGHour
        	,A.Ttr_OTHour
        	,A.Ttr_NDHour
        	,A.Ttr_NDOTHour
        	,A.Ttr_LVHour
        	,A.Usr_Login
        	,A.Ludatetime
        	,A.Ttr_NextDayHour
        	,A.Ttr_TBAmt01
        	,A.Ttr_TBAmt02
        	,A.Ttr_TBAmt03
        	,A.Ttr_TBAmt04
        	,A.Ttr_TBAmt05
        	,A.Ttr_TBAmt06
        	,A.Ttr_TBAmt07
        	,A.Ttr_TBAmt08
        	,A.Ttr_TBAmt09
        	,A.Ttr_TBAmt10
        	,A.Ttr_TBAmt11
        	,A.Ttr_TBAmt12       
        	,A.Ttr_WorkLocationCode
        	,A.Ttr_FlextimeFlag
        	,A.Ttr_TagFlextime
        	,A.Ttr_WFTimeMod
        	,A.Ttr_CalendarType
        	,A.Ttr_CalendarGroup
        	,A.Ttr_AssumedPost
        	,A.Ttr_InitialOffsetMin
        	,A.Ttr_AppliedOffsetMin
        	,A.Ttr_CompOffsetMin
        	,A.Ttr_CompLT2Min
        	,A.Ttr_CompUT1Min
        	,A.Ttr_CompUT2Min
        From {0}..T_EmpTimeRegisterHst A
        LEFT JOIN {0}..T_EmpTimeRegisterTrl B
        ON A.Ttr_IDNo = B.Ttr_IDNo
        	AND A.Ttr_Date = B.Ttr_Date
            AND A.Ttr_PayCycle = B.Ttr_PayCycle
        	AND B.Ttr_AdjPayCycle = @AdjustPayPeriod
        WHERE B.Ttr_IDNo IS NULL
            AND A.Ttr_PayCycle = @AffectedPayPeriod
            AND A.Ttr_IDNo = '{1}'

        --INSERT INTO PAYROLL TRANSACTION TRAIL
        INSERT INTO {0}..T_EmpPayTranHdrTrl
        (
        	   Tph_IDNo
              ,Tph_AdjPayCycle
              ,Tph_PayCycle
              ,Tph_ABSHr
              ,Tph_REGHr
              ,Tph_REGOTHr
              ,Tph_REGNDHr
              ,Tph_REGNDOTHr
              ,Tph_RESTHr
              ,Tph_RESTOTHr
              ,Tph_RESTNDHr
              ,Tph_RESTNDOTHr
              ,Tph_LEGHOLHr
              ,Tph_LEGHOLOTHr
              ,Tph_LEGHOLNDHr
              ,Tph_LEGHOLNDOTHr
              ,Tph_SPLHOLHr
              ,Tph_SPLHOLOTHr
              ,Tph_SPLHOLNDHr
              ,Tph_SPLHOLNDOTHr
              ,Tph_PSDHr
              ,Tph_PSDOTHr
              ,Tph_PSDNDHr
              ,Tph_PSDNDOTHr
              ,Tph_COMPHOLHr
              ,Tph_COMPHOLOTHr
              ,Tph_COMPHOLNDHr
              ,Tph_COMPHOLNDOTHr
              ,Tph_RESTLEGHOLHr
              ,Tph_RESTLEGHOLOTHr
              ,Tph_RESTLEGHOLNDHr
              ,Tph_RESTLEGHOLNDOTHr
              ,Tph_RESTSPLHOLHr
              ,Tph_RESTSPLHOLOTHr
              ,Tph_RESTSPLHOLNDHr
              ,Tph_RESTSPLHOLNDOTHr
              ,Tph_RESTCOMPHOLHr
              ,Tph_RESTCOMPHOLOTHr
              ,Tph_RESTCOMPHOLNDHr
              ,Tph_COMPHOLNDOTHr
              ,Tph_RESTPSDHr
              ,Tph_RESTPSDOTHr
              ,Tph_RESTPSDNDHr
              ,Tph_RESTPSDNDOTHr
              ,Tph_TotalAdjAmt
              ,Tph_TaxableAdjAmt
              ,Tph_NontaxableAdjAmt
              ,Tph_TaxableIncomeAmt
              ,Tph_NontaxableIncomeAmt
              ,Tph_RESTLEGHOLDay
              ,Tph_WorkDay
              ,Tph_PayrollType
              ,Tph_LTHr
              ,Tph_UTHr
              ,Tph_WDABSHr
              ,Tph_UnpaidLVHr
              ,Tph_ABSLEGHOLHr
              ,Tph_ABSSPLHOLHr
              ,Tph_ABSCOMPHOLHr
              ,Tph_ABSPSDHr
              ,Tph_ABSOTHHOLHr
              ,Tph_PaidLVHr
              ,Tph_PaidLEGHOLHr
              ,Tph_PaidSPLHOLHr
              ,Tph_PaidCOMPHOLHr
              ,Tph_PaidOTHHOLHr
              ,Tph_OTAdjAmt
              ,Tph_RetainUserEntry
              ,Usr_Login
              ,Ludatetime
        )
        Select A.Tph_IDNo
              ,@AdjustPayPeriod
              ,A.Tph_PayCycle
              ,A.Tph_ABSHr
              ,A.Tph_REGHr
              ,A.Tph_REGOTHr
              ,A.Tph_REGNDHr
              ,A.Tph_REGNDOTHr
              ,A.Tph_RESTHr
              ,A.Tph_RESTOTHr
              ,A.Tph_RESTNDHr
              ,A.Tph_RESTNDOTHr
              ,A.Tph_LEGHOLHr
              ,A.Tph_LEGHOLOTHr
              ,A.Tph_LEGHOLNDHr
              ,A.Tph_LEGHOLNDOTHr
              ,A.Tph_SPLHOLHr
              ,A.Tph_SPLHOLOTHr
              ,A.Tph_SPLHOLNDHr
              ,A.Tph_SPLHOLNDOTHr
              ,A.Tph_PSDHr
              ,A.Tph_PSDOTHr
              ,A.Tph_PSDNDHr
              ,A.Tph_PSDNDOTHr
              ,A.Tph_COMPHOLHr
              ,A.Tph_COMPHOLOTHr
              ,A.Tph_COMPHOLNDHr
              ,A.Tph_COMPHOLNDOTHr
              ,A.Tph_RESTLEGHOLHr
              ,A.Tph_RESTLEGHOLOTHr
              ,A.Tph_RESTLEGHOLNDHr
              ,A.Tph_RESTLEGHOLNDOTHr
              ,A.Tph_RESTSPLHOLHr
              ,A.Tph_RESTSPLHOLOTHr
              ,A.Tph_RESTSPLHOLNDHr
              ,A.Tph_RESTSPLHOLNDOTHr
              ,A.Tph_RESTCOMPHOLHr
              ,A.Tph_RESTCOMPHOLOTHr
              ,A.Tph_RESTCOMPHOLNDHr
              ,A.Tph_COMPHOLNDOTHr
              ,A.Tph_RESTPSDHr
              ,A.Tph_RESTPSDOTHr
              ,A.Tph_RESTPSDNDHr
              ,A.Tph_RESTPSDNDOTHr
              ,A.Tph_TotalAdjAmt
              ,A.Tph_TaxableAdjAmt
              ,A.Tph_NontaxableAdjAmt
              ,A.Tph_TaxableIncomeAmt
              ,A.Tph_NontaxableIncomeAmt
              ,A.Tph_RESTLEGHOLDay
              ,A.Tph_WorkDay
              ,A.Tph_PayrollType
              ,A.Tph_LTHr
              ,A.Tph_UTHr
              ,A.Tph_WDABSHr
              ,A.Tph_UnpaidLVHr
              ,A.Tph_ABSLEGHOLHr
              ,A.Tph_ABSSPLHOLHr
              ,A.Tph_ABSCOMPHOLHr
              ,A.Tph_ABSPSDHr
              ,A.Tph_ABSOTHHOLHr
              ,A.Tph_PaidLVHr
              ,A.Tph_PaidLEGHOLHr
              ,A.Tph_PaidSPLHOLHr
              ,A.Tph_PaidCOMPHOLHr
              ,A.Tph_PaidOTHHOLHr
              ,A.Tph_OTAdjAmt
              ,A.Tph_RetainUserEntry
              ,A.Usr_Login
              ,A.Ludatetime
        From {0}..T_EmpPayTranHdrHst A
        LEFT JOIN {0}..T_EmpPayTranHdrTrl B
        ON A.Tph_IDNo = B.Tph_IDNo
        	AND A.Tph_PayCycle = B.Tph_PayCycle
            AND B.Tph_AdjPayCycle = @AdjustPayPeriod
        WHERE B.Tph_IDNo IS NULL
            AND A.Tph_PayCycle = @AffectedPayPeriod
            AND A.Tph_IDNo = '{1}'

        --INSERT INTO PAYROLL TRANSACTION TRAIL DETAIL
        INSERT INTO {0}..T_EmpPayTranDtlTrl
        (
        	   Tpd_IDNo
              ,Tpd_AdjPayCycle
              ,Tpd_PayCycle
              ,Tpd_Date
              ,Tpd_SalaryRate
              ,Tpd_HourRate
              ,Tpd_SalaryType
              ,Tpd_PayrollType
              ,Tpd_ABSHr
              ,Tpd_REGHr
              ,Tpd_REGOTHr
              ,Tpd_REGNDHr
              ,Tpd_REGNDOTHr
              ,Tpd_RESTHr
              ,Tpd_RESTOTHr
              ,Tpd_RESTNDHr
              ,Tpd_RESTNDOTHr
              ,Tpd_LEGHOLHr
              ,Tpd_LEGHOLOTHr
              ,Tpd_LEGHOLNDHr
              ,Tpd_LEGHOLNDOTHr
              ,Tpd_SPLHOLHr
              ,Tpd_SPLHOLOTHr
              ,Tpd_SPLHOLNDHr
              ,Tpd_SPLHOLNDOTHr
              ,Tpd_PSDHr
              ,Tpd_PSDOTHr
              ,Tpd_PSDNDHr
              ,Tpd_PSDNDOTHr
              ,Tpd_COMPHOLHr
              ,Tpd_COMPHOLOTHr
              ,Tpd_COMPHOLNDHr
              ,Tpd_COMPHOLNDOTHr
              ,Tpd_RESTLEGHOLHr
              ,Tpd_RESTLEGHOLOTHr
              ,Tpd_RESTLEGHOLNDHr
              ,Tpd_RESTLEGHOLNDOTHr
              ,Tpd_RESTSPLHOLHr
              ,Tpd_RESTSPLHOLOTHr
              ,Tpd_RESTSPLHOLNDHr
              ,Tpd_RESTSPLHOLNDOTHr
              ,Tpd_RESTCOMPHOLHr
              ,Tpd_RESTCOMPHOLOTHr
              ,Tpd_RESTCOMPHOLNDHr
              ,Tpd_COMPHOLNDOTHr
              ,Tpd_RESTPSDHr
              ,Tpd_RESTPSDOTHr
              ,Tpd_RESTPSDNDHr
              ,Tpd_RESTPSDNDOTHr
              ,Tpd_RESTLEGHOLDay
              ,Tpd_WorkDay
              ,Tpd_LTHr
              ,Tpd_UTHr
              ,Tpd_WDABSHr
              ,Tpd_UnpaidLVHr
              ,Tpd_ABSLEGHOLHr
              ,Tpd_ABSSPLHOLHr
              ,Tpd_ABSCOMPHOLHr
              ,Tpd_ABSPSDHr
              ,Tpd_ABSOTHHOLHr
              ,Tpd_PaidLVHr
              ,Tpd_PaidLEGHOLHr
              ,Tpd_PaidSPLHOLHr
              ,Tpd_PaidCOMPHOLHr
              ,Tpd_PaidOTHHOLHr
              ,Tpd_OTAdjAmt
              ,Usr_Login
              ,Ludatetime
        )
        Select A.Tpd_IDNo
              ,@AdjustPayPeriod
              ,A.Tpd_PayCycle
              ,A.Tpd_Date
              ,A.Tpd_SalaryRate
              ,A.Tpd_HourRate
              ,A.Tpd_SalaryType
              ,A.Tpd_PayrollType
              ,A.Tpd_ABSHr
              ,A.Tpd_REGHr
              ,A.Tpd_REGOTHr
              ,A.Tpd_REGNDHr
              ,A.Tpd_REGNDOTHr
              ,A.Tpd_RESTHr
              ,A.Tpd_RESTOTHr
              ,A.Tpd_RESTNDHr
              ,A.Tpd_RESTNDOTHr
              ,A.Tpd_LEGHOLHr
              ,A.Tpd_LEGHOLOTHr
              ,A.Tpd_LEGHOLNDHr
              ,A.Tpd_LEGHOLNDOTHr
              ,A.Tpd_SPLHOLHr
              ,A.Tpd_SPLHOLOTHr
              ,A.Tpd_SPLHOLNDHr
              ,A.Tpd_SPLHOLNDOTHr
              ,A.Tpd_PSDHr
              ,A.Tpd_PSDOTHr
              ,A.Tpd_PSDNDHr
              ,A.Tpd_PSDNDOTHr
              ,A.Tpd_COMPHOLHr
              ,A.Tpd_COMPHOLOTHr
              ,A.Tpd_COMPHOLNDHr
              ,A.Tpd_COMPHOLNDOTHr
              ,A.Tpd_RESTLEGHOLHr
              ,A.Tpd_RESTLEGHOLOTHr
              ,A.Tpd_RESTLEGHOLNDHr
              ,A.Tpd_RESTLEGHOLNDOTHr
              ,A.Tpd_RESTSPLHOLHr
              ,A.Tpd_RESTSPLHOLOTHr
              ,A.Tpd_RESTSPLHOLNDHr
              ,A.Tpd_RESTSPLHOLNDOTHr
              ,A.Tpd_RESTCOMPHOLHr
              ,A.Tpd_RESTCOMPHOLOTHr
              ,A.Tpd_RESTCOMPHOLNDHr
              ,A.Tpd_COMPHOLNDOTHr
              ,A.Tpd_RESTPSDHr
              ,A.Tpd_RESTPSDOTHr
              ,A.Tpd_RESTPSDNDHr
              ,A.Tpd_RESTPSDNDOTHr
              ,A.Tpd_RESTLEGHOLDay
              ,A.Tpd_WorkDay
              ,A.Tpd_LTHr
              ,A.Tpd_UTHr
              ,A.Tpd_WDABSHr
              ,A.Tpd_UnpaidLVHr
              ,A.Tpd_ABSLEGHOLHr
              ,A.Tpd_ABSSPLHOLHr
              ,A.Tpd_ABSCOMPHOLHr
              ,A.Tpd_ABSPSDHr
              ,A.Tpd_ABSOTHHOLHr
              ,A.Tpd_PaidLVHr
              ,A.Tpd_PaidLEGHOLHr
              ,A.Tpd_PaidSPLHOLHr
              ,A.Tpd_PaidCOMPHOLHr
              ,A.Tpd_PaidOTHHOLHr
              ,A.Tpd_OTAdjAmt
              ,A.Usr_Login
              ,A.Ludatetime
        From {0}..T_EmpPayTranDtlHst A
        LEFT JOIN {0}..T_EmpPayTranDtlTrl B
        ON A.Tpd_IDNo = B.Tpd_IDNo
        	AND A.Tpd_PayCycle = B.Tpd_PayCycle
        	AND A.Tpd_Date = B.Tpd_Date
            AND B.Tpd_AdjPayCycle = @AdjustPayPeriod
        WHERE B.Tpd_IDNo IS NULL
            AND A.Tpd_PayCycle = @AffectedPayPeriod
            AND A.Tpd_IDNo = '{1}'

        --INSERT INTO PAYROLL TRANSACTION TRAIL EXT
        INSERT INTO {0}..T_EmpPayTranHdrMiscTrl
        (
        	   Tph_IDNo
              ,Tph_AdjPayCycle
              ,Tph_PayCycle
              ,Tph_Misc1Hr
              ,Tph_Misc1OTHr
              ,Tph_Misc1NDHr
              ,Tph_Misc1NDOTHr
              ,Tph_Misc2Hr
              ,Tph_Misc2OTHr
              ,Tph_Misc2NDHr
              ,Tph_Misc2NDOTHr
              ,Tph_Misc3Hr
              ,Tph_Misc3OTHr
              ,Tph_Misc3NDHr
              ,Tph_Misc3NDOTHr
              ,Tph_Misc4Hr
              ,Tph_Misc4OTHr
              ,Tph_Misc4NDHr
              ,Tph_Misc4NDOTHr
              ,Tph_Misc5Hr
              ,Tph_Misc5OTHr
              ,Tph_Misc5NDHr
              ,Tph_Misc5NDOTHr
              ,Tph_Misc6Hr
              ,Tph_Misc6OTHr
              ,Tph_Misc6NDHr
              ,Tph_Misc6NDOTHr
              ,Usr_Login
              ,Ludatetime
        )
        Select A.Tph_IDNo
              ,@AdjustPayPeriod
              ,A.Tph_PayCycle
              ,A.Tph_Misc1Hr
              ,A.Tph_Misc1OTHr
              ,A.Tph_Misc1NDHr
              ,A.Tph_Misc1NDOTHr
              ,A.Tph_Misc2Hr
              ,A.Tph_Misc2OTHr
              ,A.Tph_Misc2NDHr
              ,A.Tph_Misc2NDOTHr
              ,A.Tph_Misc3Hr
              ,A.Tph_Misc3OTHr
              ,A.Tph_Misc3NDHr
              ,A.Tph_Misc3NDOTHr
              ,A.Tph_Misc4Hr
              ,A.Tph_Misc4OTHr
              ,A.Tph_Misc4NDHr
              ,A.Tph_Misc4NDOTHr
              ,A.Tph_Misc5Hr
              ,A.Tph_Misc5OTHr
              ,A.Tph_Misc5NDHr
              ,A.Tph_Misc5NDOTHr
              ,A.Tph_Misc6Hr
              ,A.Tph_Misc6OTHr
              ,A.Tph_Misc6NDHr
              ,A.Tph_Misc6NDOTHr
              ,A.Usr_Login
              ,A.Ludatetime
        From {0}..T_EmpPayTranHdrMiscHst A
        LEFT JOIN {0}..T_EmpPayTranHdrMiscTrl B
        ON A.Tph_IDNo = B.Tph_IDNo
        	AND A.Tph_PayCycle = B.Tph_PayCycle
            AND B.Tph_AdjPayCycle = @AdjustPayPeriod
        WHERE B.Tph_IDNo IS NULL
            AND A.Tph_PayCycle = @AffectedPayPeriod
            AND A.Tph_IDNo = '{1}'

        --INSERT INTO PAYROLL TRANSACTION TRAIL EXT DETAIL
        INSERT INTO {0}..T_EmpPayTranDtlMiscTrl
        (
        	   Tpd_IDNo
              ,Tpd_AdjPayCycle
              ,Tpd_PayCycle
              ,Tpd_Date
              ,Tpd_SalaryRate
              ,Tpd_HourRate
              ,Tpd_SalaryType
              ,Tpd_PayrollType
              ,Tpd_Misc1Hr
              ,Tpd_Misc1OTHr
              ,Tpd_Misc1NDHr
              ,Tpd_Misc1NDOTHr
              ,Tpd_Misc2Hr
              ,Tpd_Misc2OTHr
              ,Tpd_Misc2NDHr
              ,Tpd_Misc2NDOTHr
              ,Tpd_Misc3Hr
              ,Tpd_Misc3OTHr
              ,Tpd_Misc3NDHr
              ,Tpd_Misc3NDOTHr
              ,Tpd_Misc4Hr
              ,Tpd_Misc4OTHr
              ,Tpd_Misc4NDHr
              ,Tpd_Misc4NDOTHr
              ,Tpd_Misc5Hr
              ,Tpd_Misc5OTHr
              ,Tpd_Misc5NDHr
              ,Tpd_Misc5NDOTHr
              ,Tpd_Misc6Hr
              ,Tpd_Misc6OTHr
              ,Tpd_Misc6NDHr
              ,Tpd_Misc6NDOTHr
              ,Usr_Login
              ,Ludatetime
        )
        Select A.Tpd_IDNo
              ,@AdjustPayPeriod
              ,A.Tpd_PayCycle
              ,A.Tpd_Date
              ,A.Tpd_SalaryRate
              ,A.Tpd_HourRate
              ,A.Tpd_SalaryType
              ,A.Tpd_PayrollType
              ,A.Tpd_Misc1Hr
              ,A.Tpd_Misc1OTHr
              ,A.Tpd_Misc1NDHr
              ,A.Tpd_Misc1NDOTHr
              ,A.Tpd_Misc2Hr
              ,A.Tpd_Misc2OTHr
              ,A.Tpd_Misc2NDHr
              ,A.Tpd_Misc2NDOTHr
              ,A.Tpd_Misc3Hr
              ,A.Tpd_Misc3OTHr
              ,A.Tpd_Misc3NDHr
              ,A.Tpd_Misc3NDOTHr
              ,A.Tpd_Misc4Hr
              ,A.Tpd_Misc4OTHr
              ,A.Tpd_Misc4NDHr
              ,A.Tpd_Misc4NDOTHr
              ,A.Tpd_Misc5Hr
              ,A.Tpd_Misc5OTHr
              ,A.Tpd_Misc5NDHr
              ,A.Tpd_Misc5NDOTHr
              ,A.Tpd_Misc6Hr
              ,A.Tpd_Misc6OTHr
              ,A.Tpd_Misc6NDHr
              ,A.Tpd_Misc6NDOTHr
              ,A.Usr_Login
              ,A.Ludatetime
        From {0}..T_EmpPayTranHdrMiscHstDetail A
        LEFT JOIN {0}..T_EmpPayTranDtlMiscTrl B
        ON A.Tpd_IDNo = B.Tpd_IDNo
        	AND A.Tpd_PayCycle = B.Tpd_PayCycle
        	AND A.Tpd_Date = B.Tpd_Date
            AND B.Tpd_AdjPayCycle = @AdjustPayPeriod
        WHERE B.Tpd_IDNo IS NULL
            AND A.Tpd_PayCycle = @AffectedPayPeriod
            AND A.Tpd_IDNo = '{1}' ", strDBName, drArrLogLedger[x]["Ttr_IDNo"], dtProcessDate.ToShortDateString());
                    #endregion
                    dal.ExecuteNonQuery(strTrailPrevious);
                    #endregion
                }

                strUpdateQuery += string.Format(strUpdateTemplate
                                                , strDBName
                                                , drArrLogLedger[x]["Ttr_IDNo"]
                                                , drArrLogLedger[x]["Ttr_Date"]
                                                , drArrLogLedger[x]["Ttr_ActIn_1"]
                                                , drArrLogLedger[x]["Ttr_ActOut_1"]
                                                , drArrLogLedger[x]["Ttr_ActIn_2"]
                                                , drArrLogLedger[x]["Ttr_ActOut_2"]
                                                , strLogLedgerTable);
                iUpdateCtr++;
                if (iUpdateCtr == 150) //150 log ledger records per batch update
                {
                    dal.ExecuteNonQuery(strUpdateQuery);
                    strUpdateQuery = "";
                    iUpdateCtr = 0;
                }
            }

            if (strUpdateQuery != "")
                dal.ExecuteNonQuery(strUpdateQuery);

            Console.Write(" {0} Successfully Saved", drArrLogLedger.Length);
            #endregion

            #region Save DTR Records
            Console.Write("\n{0} Saving DTR...", dtProcessDate.ToShortDateString());
            drArrDTR = dtDTR.Select("Edited = 1 OR (PostFlag = 'True' AND Edited = 0)");
            strUpdateTemplate = @"UPDATE {0}..T_EmpDTR SET Tel_IsPosted = {4} WHERE Tel_IDNo = '{1}' AND Tel_LogDate = '{2}' AND Tel_LogTime = '{3}' AND Tel_LogType = '{5}' ";
            strUpdateQuery = "";
            iUpdateCtr = 0;
            for (int x = 0; x < drArrDTR.Length; x++)
            {
                strUpdateQuery += string.Format(strUpdateTemplate
                                                , Globals.DTRDBName
                                                , drArrDTR[x]["EmployeeID"]
                                                , drArrDTR[x]["LogDate"]
                                                , drArrDTR[x]["LogTime"]
                                                , drArrDTR[x]["Edited"]
                                                , drArrDTR[x]["LogType"]);
                iUpdateCtr++;
                if (iUpdateCtr == 150) //150 DTR records per batch update
                {
                    dal.ExecuteNonQuery(strUpdateQuery);
                    strUpdateQuery = "";
                    iUpdateCtr = 0;
                }
            }

            if (strUpdateQuery != "")
                dal.ExecuteNonQuery(strUpdateQuery);

            Console.Write(" {0} Successfully Saved", drArrDTR.Length);
            #endregion

            Console.WriteLine("\n{0} Updating Log Control record...", dtProcessDate.ToShortDateString());
            UpdateLogControlTable(strDBName, dtProcessDate, strLogLedgerTable, dal);

            Console.WriteLine("End: {0:M/dd/yyyy HH:mm:ss} ", DateTime.Now);
            return drArrDTR.Length;
        }

        public int PostLogsWOLogType(string strDBName, string strCompanyCode, DateTime dtProcessDate, bool bPostPartial, string strSelectedEmployeeID, DALHelper dal)
        {
            #region Variables
            DataRow[] drArrLogLedger, drArrDTR, drArrLogTrail, drArrTimeMod, drDTROverride;
            string strEmployeeID, strScheduleType;
            int iActualTimeIn1Min, iActualTimeOut1Min, iActualTimeIn2Min, iActualTimeOut2Min;
            int iTrailTimeIn1Min, iTrailTimeOut1Min, iTrailTimeIn2Min, iTrailTimeOut2Min;
            int iShiftTimeIn1Min, iShiftTimeOut1Min, iShiftTimeIn2Min, iShiftTimeOut2Min;
            DateTime dtDTRDate;
            int iLogTime, iLogTimeOrig, iActualLogOutTemp;
            bool bPostFlag, bEdited, bIsInHistTable = false;
            string SkipServiceFlag = "";
            const int GRAVEYARD24 = 1440;
            int bExtensionBeforeTimeIn;
            #endregion

            Console.WriteLine("\nStart: {0:M/dd/yyyy HH:mm:ss} ", DateTime.Now);
            Console.WriteLine("{0} Getting Log Ledger records...", dtProcessDate.ToShortDateString());
            dtEmployeeLogLedger = GetLogLedgerRecordsPerDate(strDBName, strCompanyCode, dtProcessDate, bPostPartial, strSelectedEmployeeID, dal);
            Console.WriteLine("{0} Getting DTR records...", dtProcessDate.ToShortDateString());
            dtDTR = GetDTRRecordsPerDate(dtProcessDate, strSelectedEmployeeID, dal);
            Console.WriteLine("{0} Getting DTR Override records...", dtProcessDate.ToShortDateString());
            dtDtrOverride = GetDtrOverrideRecordsPerDate(strDBName, dtProcessDate, strSelectedEmployeeID, dal);
            Console.WriteLine("{0} Getting Time Correction records...", dtProcessDate.ToShortDateString());
            dtTimeCor = GetTimeCorrectionRecordsPerDate(strDBName, dtProcessDate, strSelectedEmployeeID, dal);

            for (int i = 0; i < dtEmployeeLogLedger.Rows.Count; i++)
            {
                Console.Write("\r{0} Posting logs {1} of {2}...", dtProcessDate.ToShortDateString(), i + 1, dtEmployeeLogLedger.Rows.Count);

                #region Initialize Variables
                    bEdited = false;

                    strEmployeeID = dtEmployeeLogLedger.Rows[i]["Ttr_IDNo"].ToString();
                    strScheduleType = dtEmployeeLogLedger.Rows[i]["ScheduleType"].ToString();
                    SkipServiceFlag = dtEmployeeLogLedger.Rows[i]["SkipService"].ToString();

                    iActualTimeIn1Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttr_ActIn_1"].ToString());
                    iActualTimeOut1Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttr_ActOut_1"].ToString());
                    iActualTimeIn2Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttr_ActIn_2"].ToString());
                    iActualTimeOut2Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttr_ActOut_2"].ToString());

                    iShiftTimeIn1Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["ShiftTimeIn"].ToString());
                    iShiftTimeOut1Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["ShiftBreakStart"].ToString());
                    iShiftTimeIn2Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["ShiftBreakEnd"].ToString());
                    iShiftTimeOut2Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["ShiftTimeOut"].ToString());

                    if (dtEmployeeLogLedger.Rows[i]["TableName"].ToString() == "T_EmpTimeRegisterHst")
                        bIsInHistTable = true;

                    //Convert to Graveyard
                    if (iShiftTimeIn1Min > iShiftTimeOut1Min)
                        iShiftTimeOut1Min = iShiftTimeOut1Min + GRAVEYARD24;
                    if (iShiftTimeIn1Min > iShiftTimeIn2Min)
                        iShiftTimeIn2Min = iShiftTimeIn2Min + GRAVEYARD24;
                    if (iShiftTimeIn1Min > iShiftTimeOut2Min)
                        iShiftTimeOut2Min = iShiftTimeOut2Min + GRAVEYARD24;

                    drArrDTR = dtDTR.Select(string.Format("EmployeeID = '{0}'", strEmployeeID));
                    #endregion

                if (SkipServiceFlag.Equals("N"))
                {
                    #region Loop through DTR Logs (Regular FILO Posting)
                    for (int x = 0; x < drArrDTR.Length; x++)
                    {
                        #region Initialize Variables
                        dtDTRDate = Convert.ToDateTime(drArrDTR[x]["LogDate"].ToString());
                        iLogTime = GetMinsFromHourStr(drArrDTR[x]["LogTime"].ToString());
                        iLogTimeOrig = iLogTime;
                        bPostFlag = Convert.ToBoolean(drArrDTR[x]["PostFlag"].ToString());
                        #endregion

                        #region Without Mark INOUT

                        #region EARLY IN OR IS EQUAL TO SHIFT IN1
                        if (dtProcessDate == dtDTRDate && iLogTime != 0 &&
                            (iLogTime >= (iShiftTimeIn1Min - Globals.EXTENDIN1) && iLogTime <= iShiftTimeIn1Min)
                            && iActualTimeIn1Min == 0) //IN1 If TIME <= IN 1 : Post in IN 1 
                        {
                            iActualTimeIn1Min = iLogTimeOrig;
                            drArrDTR[x]["PostFlag"] = "True";
                            drArrDTR[x]["Edited"] = 1;
                            bEdited = true;
                        }
                        #endregion
                        #region LATE | LUNCHBREAK | UNDERTIME (IN BETWEEN)
                        else if (dtProcessDate == dtDTRDate && iLogTime != 0
                            && (iLogTime > iShiftTimeIn1Min && iLogTime < iShiftTimeOut2Min))
                        {
                            if (iActualTimeIn1Min == 0 &&
                                (iLogTime > iShiftTimeIn1Min && iLogTime < iShiftTimeOut1Min))  //LATE IN1
                            {   //TIME > IN1 AND < OUT1 : Post in IN 1
                                iActualTimeIn1Min = iLogTimeOrig;
                                drArrDTR[x]["PostFlag"] = "True";
                                drArrDTR[x]["Edited"] = 1;
                                bEdited = true;
                            }
                            else if (iActualTimeIn1Min > 0 && iActualTimeOut1Min == 0 &&
                                (iLogTime > iShiftTimeIn1Min && iLogTime < iShiftTimeOut1Min)
                                && (iLogTime - iActualTimeIn1Min) >= Globals.POCKETTIME) //EARLY OUT || EARLY LUNCHBREAK
                            {   //IF TIME > IN1 AND < EARLY OUT
                                iActualTimeOut1Min = iLogTimeOrig;
                                drArrDTR[x]["PostFlag"] = "True";
                                drArrDTR[x]["Edited"] = 1;
                                bEdited = true;
                            }
                            //else if (iActualTimeOut1Min == 0 &&
                            //      (iLogTime >= (iShiftTimeOut1Min - 60) && iLogTime < iShiftTimeOut1Min)) //EARLY LUNCHBREAK
                            //{  //IF TIME is In Between(OUT 1 - 1Hour) and OUT1 : POST OUT 1   
                            //    if (iActualTimeIn1Min == 0)
                            //        iActualTimeIn1Min = iLogTimeOrig;
                            //    else
                            //        iActualTimeOut1Min = iLogTimeOrig;
                            //    drArrDTR[x]["PostFlag"] = "True";
                            //    drArrDTR[x]["Edited"] = 1;
                            //    bEdited = true;
                            //}
                            else if (iLogTime >= iShiftTimeOut1Min && iLogTime < iShiftTimeIn2Min) //LUNCHBREAK
                            {  //IF TIME > =  OUT1  and < IN 2 : POST OUT1
                                if (iActualTimeOut1Min == 0
                                    && (iLogTime - iActualTimeIn1Min) >= Globals.POCKETTIME
                                    && iLogTime != iActualTimeIn2Min)
                                {
                                    iActualTimeOut1Min = iLogTimeOrig;
                                    drArrDTR[x]["PostFlag"] = "True";
                                    drArrDTR[x]["Edited"] = 1;
                                    bEdited = true;
                                }
                                else if (iActualTimeOut1Min > 0 && iActualTimeIn2Min == 0
                                    && Globals.POCKETTIME > 0 && (iLogTime - iActualTimeOut1Min) >= Globals.POCKETTIME)
                                { //IF TIME > = OUT1  and < IN2 and OUT1 is not Empty: POST IN2
                                    iActualTimeIn2Min = iLogTimeOrig;
                                    drArrDTR[x]["PostFlag"] = "True";
                                    drArrDTR[x]["Edited"] = 1;
                                    bEdited = true;
                                }
                                //else if (iActualTimeOut1Min > 0 && iActualTimeIn2Min == 0 
                                //    && Globals.POCKETTIME == 0 && iLogTime != iActualTimeOut1Min)
                                //{ //IF TIME > = OUT1  and < IN2 and OUT1 is not Empty: POST IN2
                                //    iActualTimeIn2Min = iLogTimeOrig;
                                //    drArrDTR[x]["PostFlag"] = "True";
                                //    drArrDTR[x]["Edited"] = 1;
                                //    bEdited = true;
                                //}
                            }
                            else if (iActualTimeIn2Min == 0 &&
                                (iLogTime >= iShiftTimeIn2Min && iLogTime < iShiftTimeOut2Min)
                                && Globals.POCKETTIME > 0 && (iLogTime - iActualTimeOut1Min) >= Globals.POCKETTIME)
                            {  //LATE IN2
                                iActualTimeIn2Min = iLogTimeOrig;
                                drArrDTR[x]["PostFlag"] = "True";
                                drArrDTR[x]["Edited"] = 1;
                                bEdited = true;
                            }

                            //else if ((iLogTime > (iShiftTimeIn2Min + (iShiftTimeOut2Min - iShiftTimeIn2Min) / 2)
                            //    && iLogTime < iShiftTimeOut2Min))  //UNDERTIME
                            else if (iActualTimeIn2Min > 0 && iActualTimeOut2Min == 0
                                    && iLogTime < iShiftTimeOut2Min
                                    && (iLogTime - iActualTimeIn2Min) >= Globals.POCKETTIME) //UNDERTIME
                            {   // IF TIME > IN 2 and < OUT 2 : Post in OUT 2 
                                iActualTimeOut2Min = iLogTimeOrig;
                                drArrDTR[x]["PostFlag"] = "True";
                                drArrDTR[x]["Edited"] = 1;
                                bEdited = true;
                            }
                        }
                        #endregion
                        #region AFTER SHIFT
                        else if ((iLogTime >= iShiftTimeOut2Min ||
                            (dtProcessDate.AddDays(1) == dtDTRDate && iLogTime <
                            (iShiftTimeIn1Min - Globals.EXTENDIN1))))
                        { //OUT2   If TIME >= OUT 2 : Post in Out 2
                          //if (iLogTime == 0)
                          //    iLogTimeOrig = iLogTime = GRAVEYARD24 - 1;

                            if (iLogTime == 0)
                                iLogTimeOrig = iLogTime = 1;

                            if (iLogTime != 0 && dtProcessDate.AddDays(1) == dtDTRDate) //INOUT on next day
                                iLogTime = iLogTime + GRAVEYARD24;

                            if (iActualTimeIn1Min == 0 || iActualTimeOut2Min == 0 || iActualTimeOut2Min > iActualTimeIn1Min)
                                iActualLogOutTemp = iActualTimeOut2Min;
                            else
                                iActualLogOutTemp = iActualTimeOut2Min + GRAVEYARD24;

                            if (((iActualTimeOut2Min == 0 && iLogTime <= iShiftTimeOut1Min + GRAVEYARD24) //Do not post if greater than Shift Break of Next Day
                                || (strScheduleType != "G" && dtProcessDate == dtDTRDate && iLogTime > iActualLogOutTemp)
                                || (strScheduleType == "G" && iLogTime > iActualLogOutTemp)))
                            {
                                if (dtProcessDate == dtDTRDate)
                                    iActualTimeOut2Min = iLogTimeOrig;
                                else
                                    iActualTimeOut2Min = iLogTime; //+2400 if Next Day

                                drArrDTR[x]["PostFlag"] = "True";
                                drArrDTR[x]["Edited"] = 1;
                                bEdited = true;
                            }
                        }
                        #endregion

                        #endregion

                        #region Secondary Pass
                        if (dtProcessDate == dtDTRDate
                            && iActualTimeOut1Min > 0 && iActualTimeOut1Min < iShiftTimeOut1Min
                            && iActualTimeIn1Min == 0 && iActualTimeOut1Min < iLogTime)
                        {//1.IF OUT1 != 0 AND IN1 == 0 AND OUT1 HAS A NEW ENTRY : COMPARE IF OUT1 LESS THAN NEW ENTRY 
                         //THEN MOVE OUT1 INTO IN1 AND UPDATE OUT1 WITH THE NEW ENTRY
                            iActualTimeIn1Min = iActualTimeOut1Min;
                            iActualTimeOut1Min = iLogTimeOrig;
                            drArrDTR[x]["PostFlag"] = "True";
                            drArrDTR[x]["Edited"] = 1;
                            bEdited = true;
                        }

                        if (dtProcessDate == dtDTRDate
                            && iActualTimeOut2Min > 0 && iActualTimeIn1Min == 0
                            && iActualTimeOut1Min > 0
                            && iActualTimeOut1Min < iShiftTimeOut1Min)
                        {//2.IF OUT2 != 0 AND IN1 == 0 AND OUT1 != 0 THEN COPY OUT1 INTO IN1 AND DO NOT ERASE OUT1
                            iActualTimeIn1Min = iActualTimeOut1Min;
                            drArrDTR[x]["PostFlag"] = "True";
                            drArrDTR[x]["Edited"] = 1;
                            bEdited = true;
                        }

                        if (dtProcessDate == dtDTRDate
                            && iActualTimeIn1Min == 0 && iActualTimeOut1Min > 0
                            && iActualTimeIn2Min == 0 && iActualTimeOut2Min > 0)
                        { //3.HALF DAY: MOVE OUT1 INTO IN2
                            iActualTimeIn2Min = iActualTimeOut1Min;
                            iActualTimeOut1Min = 0;
                            drArrDTR[x]["PostFlag"] = "True";
                            drArrDTR[x]["Edited"] = 1;
                            bEdited = true;
                        }

                        if (dtProcessDate < DateTime.Now
                            && iActualTimeIn2Min != 0 && iActualTimeOut2Min == 0
                            && x == (drArrDTR.Length - 1))
                        {//Previous day checking to correct unposted undertime
                            DataRow[] tempdrArrDTR = dtDTR.Select(string.Format("EmployeeID = '{0}' AND LogDate = '{1}'", strEmployeeID, dtProcessDate));
                            for (int y = 0; y < tempdrArrDTR.Length; y++)
                            {
                                DateTime tempDTRDate = Convert.ToDateTime(tempdrArrDTR[y]["LogDate"].ToString());
                                int tempLogTime = GetMinsFromHourStr(tempdrArrDTR[y]["LogTime"].ToString());

                                if (tempLogTime != 0 && tempLogTime != iActualTimeIn2Min
                                    && tempLogTime > iShiftTimeIn2Min
                                    && dtProcessDate == tempDTRDate)
                                {
                                    iActualTimeOut2Min = tempLogTime;
                                    drArrDTR[x]["PostFlag"] = "True";
                                    drArrDTR[x]["Edited"] = 1;
                                    bEdited = true;
                                }
                            }
                        }
                        #endregion

                        #region Clean-up
                        if (bEdited)
                        {
                            if (iActualTimeIn1Min != 0 && iActualTimeOut1Min != 0 && iActualTimeIn1Min == iActualTimeOut1Min)
                            {
                                if (iActualTimeOut1Min >= iShiftTimeOut1Min)
                                    iActualTimeIn1Min = 0;//Remove IN1
                                else
                                    iActualTimeOut1Min = 0;
                            }
                            else if (iActualTimeOut1Min != 0 && iActualTimeIn2Min != 0 && iActualTimeOut1Min == iActualTimeIn2Min)
                            {
                                if (iActualTimeIn2Min >= iShiftTimeIn2Min)
                                    iActualTimeOut1Min = 0; //Remove OUT1
                                else
                                    iActualTimeIn2Min = 0;
                            }
                            else if (iActualTimeIn2Min != 0 && iActualTimeOut2Min != 0 && iActualTimeIn2Min == iActualTimeOut2Min)
                            {
                                if (iActualTimeOut2Min > (iShiftTimeIn2Min + (iShiftTimeOut2Min - iShiftTimeIn2Min) / 2)
                                       && iActualTimeOut2Min < iShiftTimeOut2Min)
                                    iActualTimeIn2Min = 0; //Remove IN2
                            }
                        }
                        #endregion
                    }
                    #endregion
                }

                #region //Loop through DTR Logs (Soft Posting)
                //if (Globals.isSoftPostingEnable == true)
                //{
                //    //Perform Soft Posting only on unpaired logs or no logs at all, or when logs are not aligned with shift
                //    if ((((iActualTimeIn1Min != 0 && iActualTimeOut1Min != 0 && iActualTimeIn2Min != 0 && iActualTimeOut2Min != 0) //With IN1,OUT1,IN2,OUT2
                //        || (iActualTimeIn1Min != 0 && iActualTimeOut1Min != 0) //With IN1,OUT1
                //        || (iActualTimeIn2Min != 0 && iActualTimeOut2Min != 0) //With IN2,OUT2
                //        || (iActualTimeIn1Min != 0 && iActualTimeOut2Min != 0)) //With IN1,OUT2
                //            == false)
                //        ||
                //            ((strScheduleType == "G" && iActualTimeIn1Min < GRAVEYARD24 && iActualTimeOut2Min > 0 && iActualTimeOut2Min < GRAVEYARD24) //Graveyard shift but logs are not graveyard
                //            || (strScheduleType != "G" && iActualTimeOut2Min > 0 && iActualTimeOut2Min < iActualTimeIn1Min)) //Day/Normal/Swing shift but logs are graveyard
                //        )
                //    {
                //        //Initialize Variables
                //        bool bFoundFirstIN = false;
                //        int iLatestLogTimeIN = -1;
                //        int iLatestLogTimeOUT = -1;
                //        bool bTimeGapValid = false;
                //        int iLogTimeTemp = -1;
                //        //strLatestValidLogType = "";
                //        //strLastLogType = "";

                //        for (int x = 0; x < drArrDTR.Length; x++)
                //        {
                //            #region Initialize Variables
                //            dtDTRDate = Convert.ToDateTime(drArrDTR[x]["LogDate"].ToString());
                //            iLogTime = GetMinsFromHourStr(drArrDTR[x]["LogTime"].ToString());
                //            iLogTimeOrig = iLogTime;
                //            //strLogType = drArrDTR[x]["LogType"].ToString();
                //            bPostFlag = Convert.ToBoolean(drArrDTR[x]["PostFlag"].ToString());
                //            #endregion

                //            //Soft Posting only posts logs in IN1 and OUT2

                //            if (iLogTime == 0)
                //                iLogTimeOrig = iLogTime = GRAVEYARD24 - 1;

                //            if (iLogTime != 0
                //                && iLatestLogTimeIN == -1
                //                && dtProcessDate == dtDTRDate) //IN1
                //            {
                //                iLatestLogTimeIN = iLogTimeOrig;
                //                drArrDTR[x]["PostFlag"] = "True";
                //                drArrDTR[x]["Edited"] = 1;
                //                bEdited = true;
                //                //strLatestValidLogType = "I";
                //                bFoundFirstIN = true;
                //            }

                //            else ///if (strLogType == "O") //OUT
                //            {
                //                #region OUT
                //                if (iLogTime == 0)
                //                    iLogTimeOrig = iLogTime = 1;

                //                if (iLogTime != 0 && dtProcessDate.AddDays(1) == dtDTRDate) //OUT on next day
                //                    iLogTime = iLogTime + GRAVEYARD24;

                //                #region Check for TIMEGAP Validity
                //                bTimeGapValid = true;
                //                for (int y = x + 1; y < drArrDTR.Length && bTimeGapValid == true; y++)
                //                {
                //                    if (drArrDTR[y]["LogType"].ToString() == "I") //Get the Next IN and validate against TIMEGAP
                //                    {
                //                        iLogTimeTemp = GetMinsFromHourStr(drArrDTR[y]["LogTime"].ToString());
                //                        if (iLogTimeTemp != 0 && dtProcessDate.AddDays(1) == Convert.ToDateTime(drArrDTR[y]["LogDate"].ToString()))
                //                            iLogTimeTemp = iLogTimeTemp + GRAVEYARD24;
                //                        if (iLogTimeTemp - iLogTime < Globals.ValidTimeGap)
                //                            bTimeGapValid = false;
                //                    }
                //                }
                //                //if (strLatestValidLogType == "I")
                //                //{
                //                //    if (iLogTime - iLatestLogTimeIN < Globals.ValidTimeGap)
                //                //        bTimeGapValid = false;
                //                //}
                //                #endregion

                //                #region Check if Previous IN belongs to next day
                //                //if (strLatestValidLogType == "")
                //                //    bFoundFirstIN = false; //initialize
                //                for (int y = x - 1; y >= 0 && bFoundFirstIN == true; y--)
                //                {
                //                    iLogTimeTemp = GetMinsFromHourStr(drArrDTR[y]["LogTime"].ToString());
                //                    if (iLogTimeTemp != 0 && dtProcessDate.AddDays(1) == Convert.ToDateTime(drArrDTR[y]["LogDate"].ToString()))
                //                        iLogTimeTemp = iLogTimeTemp + GRAVEYARD24;

                //                    //if (drArrDTR[y]["LogType"].ToString() == "I") //Get the Previous IN
                //                    //{
                //                    //    if (iLogTimeTemp != 0 && dtProcessDate.AddDays(1) == Convert.ToDateTime(drArrDTR[y]["LogDate"].ToString()))
                //                    //        bFoundFirstIN = false;
                //                    //}
                //                    //else if (drArrDTR[y]["LogType"].ToString() == "O") //Get the Previous OUT to check if it is the last OUT
                //                    //{
                //                    //    if (strLatestValidLogType == "O" && iLogTimeTemp == iLatestLogTimeOUT && iLogTime - iLatestLogTimeOUT > 240) //Next OUT is greater than 4 hours
                //                    //        bFoundFirstIN = false;
                //                    //}
                //                }
                //                #endregion

                //                if (iLatestLogTimeIN == -1 || iLatestLogTimeOUT == -1 || iLatestLogTimeOUT > iLatestLogTimeIN)
                //                    iActualLogOutTemp = iLatestLogTimeOUT;
                //                else
                //                    iActualLogOutTemp = iLatestLogTimeOUT + GRAVEYARD24;

                //                if ((iLogTime != 0 && iLogTime <= iShiftTimeOut1Min + GRAVEYARD24) //Do not post if greater than Shift Break of Next Day
                //                    && (iLatestLogTimeOUT == -1 || iLogTime >= iActualLogOutTemp)
                //                    && bTimeGapValid == true
                //                    && bFoundFirstIN == true) //OUT2
                //                {
                //                    if (dtProcessDate == dtDTRDate)
                //                        iLatestLogTimeOUT = iLogTimeOrig;
                //                    else
                //                        iLatestLogTimeOUT = iLogTime; //+2400 if Next Day
                //                    drArrDTR[x]["PostFlag"] = "True";
                //                    drArrDTR[x]["Edited"] = 1;
                //                    bEdited = true;
                //                    //strLatestValidLogType = "O";
                //                }
                //                #endregion
                //            }

                //            //strLastLogType = strLogType;
                //        }

                //        #region Save Values Only if Paired
                //        if (iLatestLogTimeIN != -1 && iLatestLogTimeOUT != -1)
                //        {
                //            iActualTimeIn1Min = iLatestLogTimeIN;
                //            if (iLatestLogTimeOUT <= iShiftTimeIn2Min) //OUT1
                //            {
                //                iActualTimeOut1Min = iLatestLogTimeOUT;
                //                iActualTimeOut2Min = 0;
                //            }
                //            else //OUT2
                //            {
                //                iActualTimeOut2Min = iLatestLogTimeOUT;
                //                iActualTimeOut1Min = 0;
                //            }
                //            bEdited = true;
                //        }
                //        #endregion
                //    }
                //}
                #endregion

                #region //Log Trail Reposting
                //if (Globals.isRepostLedgerTrail == true)
                //{
                //    drArrLogTrail = dtLogTrail.Select(string.Format("Ttl_IDNo = '{0}'", strEmployeeID));
                //    if (drArrLogTrail.Length > 0)
                //    {
                //        //Get Last Sequence Record Only
                //        iTrailTimeIn1Min = GetMinsFromHourStr(drArrLogTrail[0]["Ttl_ActIn_1"].ToString());
                //        iTrailTimeOut1Min = GetMinsFromHourStr(drArrLogTrail[0]["Ttl_ActOut_1"].ToString());
                //        iTrailTimeIn2Min = GetMinsFromHourStr(drArrLogTrail[0]["Ttl_ActIn_2"].ToString());
                //        iTrailTimeOut2Min = GetMinsFromHourStr(drArrLogTrail[0]["Ttl_ActOut_2"].ToString());

                //        //Post if Logs are Paired
                //        if (((iTrailTimeIn1Min != 0 && iTrailTimeOut1Min != 0 && iTrailTimeIn2Min != 0 && iTrailTimeOut2Min != 0) //With IN1,OUT1,IN2,OUT2
                //            || (iTrailTimeIn1Min != 0 && iTrailTimeOut1Min != 0 && iTrailTimeIn2Min == 0 && iTrailTimeOut2Min == 0) //With IN1,OUT1
                //            || (iTrailTimeIn1Min == 0 && iTrailTimeOut1Min == 0 && iTrailTimeIn2Min != 0 && iTrailTimeOut2Min != 0) //With IN2,OUT2
                //            || (iTrailTimeIn1Min != 0 && iTrailTimeOut1Min == 0 && iTrailTimeIn2Min == 0 && iTrailTimeOut2Min != 0)) //With IN1,OUT2
                //                == true)
                //        {
                //            iActualTimeIn1Min = iTrailTimeIn1Min;
                //            iActualTimeOut1Min = iTrailTimeOut1Min;
                //            iActualTimeIn2Min = iTrailTimeIn2Min;
                //            iActualTimeOut2Min = iTrailTimeOut2Min;
                //            bEdited = true;
                //        }
                //    }
                //}
                #endregion

                #region DTR Override Reposting
                drDTROverride = dtDtrOverride.Select(string.Format("Tdo_IDNo = '{0}'", strEmployeeID));
                if (drDTROverride.Length > 0)
                {
                    //Get First Sequence Record Only
                    iTrailTimeIn1Min = GetMinsFromHourStr(drDTROverride[0]["Tdo_Time"].ToString());

                    #region Post according to DTR Override Type
                    switch (drDTROverride[0]["Tdo_Type"].ToString())
                    {
                        case "I1": //I1-IN1
                            if (iTrailTimeIn1Min != 0)
                            {
                                iActualTimeIn1Min = iTrailTimeIn1Min;
                                bEdited = true;
                            }
                            break;
                        case "I2": //I2-IN2
                            if (iTrailTimeIn1Min != 0)
                            {
                                iActualTimeIn2Min = iTrailTimeIn1Min;
                                bEdited = true;
                            }
                            break;
                        case "O1": //O1-OUT1
                            if (iTrailTimeIn1Min != 0)
                            {
                                iActualTimeOut1Min = iTrailTimeIn1Min;
                                bEdited = true;
                            }
                            break;
                        case "O2": //O2-OUT2
                            if (iTrailTimeIn1Min != 0)
                            {
                                iActualTimeOut2Min = iTrailTimeIn1Min;
                                bEdited = true;
                            }
                            break;
                    }
                    #endregion

                }
                #endregion

                #region Time Correction Reposting
                drArrTimeMod = dtTimeCor.Select(string.Format("Ttm_IDNo = '{0}'", strEmployeeID));
                if (drArrTimeMod.Length > 0)
                {
                    //Get First Sequence Record Only
                    iTrailTimeIn1Min = GetMinsFromHourStr(drArrTimeMod[0]["Ttm_TimeIn1"].ToString());
                    iTrailTimeOut1Min = GetMinsFromHourStr(drArrTimeMod[0]["Ttm_TimeOut1"].ToString());
                    iTrailTimeIn2Min = GetMinsFromHourStr(drArrTimeMod[0]["Ttm_TimeIn2"].ToString());
                    iTrailTimeOut2Min = GetMinsFromHourStr(drArrTimeMod[0]["Ttm_TimeOut2"].ToString());

                    #region Post according to Time Modification Type
                    switch (drArrTimeMod[0]["Ttm_TimeCorType"].ToString())
                    {
                        case "IN": //No In - (Either IN1 OR IN2)
                            if (iTrailTimeIn1Min != 0)
                            {
                                iActualTimeIn1Min = iTrailTimeIn1Min;
                                bEdited = true;
                            }
                            if (iTrailTimeIn2Min != 0)
                            {
                                iActualTimeIn2Min = iTrailTimeIn2Min;
                                bEdited = true;
                            }
                            break;
                        case "IN1": //No In 1
                            if (iTrailTimeIn1Min != 0)
                            {
                                iActualTimeIn1Min = iTrailTimeIn1Min;
                                bEdited = true;
                            }
                            break;
                        case "I12": //No In 1 & In 2
                            if (iTrailTimeIn1Min != 0)
                            {
                                iActualTimeIn1Min = iTrailTimeIn1Min;
                                bEdited = true;
                            }
                            if (iTrailTimeIn2Min != 0)
                            {
                                iActualTimeIn2Min = iTrailTimeIn2Min;
                                bEdited = true;
                            }
                            break;
                        case "I1O2": //No In 1 & Out 2
                            if (iTrailTimeIn1Min != 0)
                            {
                                iActualTimeIn1Min = iTrailTimeIn1Min;
                                bEdited = true;
                            }
                            if (iTrailTimeOut2Min != 0)
                            {
                                iActualTimeOut2Min = iTrailTimeOut2Min;
                                bEdited = true;
                            }
                            break;
                        case "IN2": //No In 2 
                            if (iTrailTimeIn2Min != 0)
                            {
                                iActualTimeIn2Min = iTrailTimeIn2Min;
                                bEdited = true;
                            }
                            break;
                        case "OUT": //No Out - (Either OUT1 OR OUT2)
                            if (iTrailTimeOut1Min != 0)
                            {
                                iActualTimeOut1Min = iTrailTimeOut1Min;
                                bEdited = true;
                            }
                            if (iTrailTimeOut2Min != 0)
                            {
                                iActualTimeOut2Min = iTrailTimeOut2Min;
                                bEdited = true;
                            }
                            break;
                        case "O1": //No Out 1
                            if (iTrailTimeOut1Min != 0)
                            {
                                iActualTimeOut1Min = iTrailTimeOut1Min;
                                bEdited = true;
                            }
                            break;
                        case "O1O2": //No Out 1 & Out 2
                            if (iTrailTimeOut1Min != 0)
                            {
                                iActualTimeOut1Min = iTrailTimeOut1Min;
                                bEdited = true;
                            }
                            if (iTrailTimeOut2Min != 0)
                            {
                                iActualTimeOut2Min = iTrailTimeOut2Min;
                                bEdited = true;
                            }
                            break;
                        case "O2": //No Out 2
                            if (iTrailTimeOut2Min != 0)
                            {
                                iActualTimeOut2Min = iTrailTimeOut2Min;
                                bEdited = true;
                            }
                            break;
                        case "INOU": //No In and Out
                            if (iTrailTimeIn1Min != 0)
                            {
                                iActualTimeIn1Min = iTrailTimeIn1Min;
                                bEdited = true;
                            }
                            if (iTrailTimeIn2Min != 0)
                            {
                                iActualTimeIn2Min = iTrailTimeIn2Min;
                                bEdited = true;
                            }
                            if (iTrailTimeOut1Min != 0)
                            {
                                iActualTimeOut1Min = iTrailTimeOut1Min;
                                bEdited = true;
                            }
                            if (iTrailTimeOut2Min != 0)
                            {
                                iActualTimeOut2Min = iTrailTimeOut2Min;
                                bEdited = true;
                            }
                            break;
                    }
                    #endregion
                }
                #endregion

                #region Save Values
                if (bEdited == true)
                {
                    dtEmployeeLogLedger.Rows[i]["Ttr_ActIn_1"] = GetHourStrFromMins(iActualTimeIn1Min);
                    dtEmployeeLogLedger.Rows[i]["Ttr_ActOut_1"] = GetHourStrFromMins(iActualTimeOut1Min);
                    dtEmployeeLogLedger.Rows[i]["Ttr_ActIn_2"] = GetHourStrFromMins(iActualTimeIn2Min);
                    dtEmployeeLogLedger.Rows[i]["Ttr_ActOut_2"] = GetHourStrFromMins(iActualTimeOut2Min);
                    dtEmployeeLogLedger.Rows[i]["Edited"] = 1;
                }
                #endregion
            }

            #region Save Log Ledger Records
            Console.Write("\n{0} Saving log ledger...", dtProcessDate.ToShortDateString());
            drArrLogLedger = dtEmployeeLogLedger.Select("Edited = 1");
            string strUpdateQuery = "";
            int iUpdateCtr = 0;

            string strLogLedgerTable = "T_EmpTimeRegister";
            if (bIsInHistTable == true)
                strLogLedgerTable = "T_EmpTimeRegisterHst";

            string strTrailPrevious = "";
            string strUpdateTemplate = @"UPDATE {0}..{7} SET Ttr_ActIn_1 = '{3}', Ttr_ActOut_1 = '{4}', Ttr_ActIn_2 = '{5}', Ttr_ActOut_2 = '{6}', Usr_Login = 'LOGUPLOADING', Ludatetime = GETDATE() WHERE Ttr_IDNo = '{1}' AND Ttr_Date = '{2}' ";
            for (int x = 0; x < drArrLogLedger.Length; x++)
            {
                if (bIsInHistTable == true)
                {
                    #region Create Trail Records
                    #region Query
                    strTrailPrevious = string.Format(@" 
DECLARE @AffectedPayPeriod VARCHAR(7) = (Select TOP(1) Tps_PayCycle 
                                        From {0}..T_PaySchedule 
                                        Where Tps_CycleIndicator = 'P'
                                            And '{2}' BETWEEN Tps_StartCycle AND Tps_EndCycle
                                        ORDER BY Tps_StartCycle DESC)
DECLARE @AdjustPayPeriod VARCHAR(7) = (Select Tps_PayCycle 
									   From {0}..T_PaySchedule 
									   Where Tps_CycleIndicator='C' 
										and Tps_RecordStatus = 'A')

--INSERT INTO LOG LEDGER TRAIL
INSERT INTO {0}..T_EmpTimeRegisterTrl
(
	    Ttr_IDNo
        ,Ttr_Date
        ,Ttr_AdjPayCycle
        ,Ttr_PayCycle
        ,Ttr_DayCode
        ,Ttr_ShiftCode
        ,Ttr_HolidayFlag
        ,Ttr_RestDayFlag
        ,Ttr_ActIn_1
        ,Ttr_ActOut_1
        ,Ttr_ActIn_2
        ,Ttr_ActOut_2
        ,Ttr_ConvIn_1Min
        ,Ttr_ConvOut_1Min
        ,Ttr_ConvIn_2Min
        ,Ttr_ConvOut_2Min
        ,Ttr_CompIn_1Min
        ,Ttr_CompOut_1Min
        ,Ttr_CompIn_2Min
        ,Ttr_CompOut_2Min
        ,Ttr_CompAdvOTMin
        ,Ttr_ShiftIn_1Min
        ,Ttr_ShiftOut_1Min
        ,Ttr_ShiftIn_2Min
        ,Ttr_ShiftOut_2Min
        ,Ttr_ShiftMin
        ,Ttr_ScheduleType
        ,Ttr_WFPayLVCode
        ,Ttr_WFPayLVHr
        ,Ttr_PayLVMin
        ,Ttr_ExcLVMin
        ,Ttr_WFNoPayLVCode
        ,Ttr_WFNoPayLVHr
        ,Ttr_NoPayLVMin
        ,Ttr_WFOTAdvHr
        ,Ttr_WFOTPostHr
        ,Ttr_OTMin
        ,Ttr_CompOTMin
        ,Ttr_OffsetOTMin
        ,Ttr_CompLT1Min
        ,Ttr_LTPostFlag
        ,Ttr_InitialABSMin
        ,Ttr_CompABSMin
        ,Ttr_CompREGMin
        ,Ttr_CompWorkMin
        ,Ttr_CompNDMin
        ,Ttr_CompNDOTMin
        ,Ttr_PrvDayWorkMin
        ,Ttr_PrvDayHolRef
        ,Ttr_SkipServiceFlag
        ,Ttr_SkipServiceBy
        ,Ttr_SkipServiceDate
        ,Ttr_AssumedFlag
        ,Ttr_AssumedBy
        ,Ttr_AssumedDate
        ,Ttr_PaidHOLHour
        ,Ttr_EmploymentStatusCode
        ,Ttr_ForceLeaveDate
        ,Ttr_ForOffsetMin
        ,Ttr_ExcOffset
        ,Ttr_EarnedSatOff
        ,Ttr_RESTLEGHOLDay
        ,Ttr_WorkDay
        ,Ttr_MealDay
        ,Ttr_EXPHour
        ,Ttr_ABSHour
        ,Ttr_REGHour
        ,Ttr_OTHour
        ,Ttr_NDHour
        ,Ttr_NDOTHour
        ,Ttr_LVHour
        ,Ttr_NextDayHour
        ,Ttr_TBAmt01
        ,Ttr_TBAmt02
        ,Ttr_TBAmt03
        ,Ttr_TBAmt04
        ,Ttr_TBAmt05
        ,Ttr_TBAmt06
        ,Ttr_TBAmt07
        ,Ttr_TBAmt08
        ,Ttr_TBAmt09
        ,Ttr_TBAmt10
        ,Ttr_TBAmt11
        ,Ttr_TBAmt12
        ,Ttr_WorkLocationCode
        ,Ttr_FlextimeFlag
        ,Ttr_TagFlextime
        ,Ttr_WFTimeMod
        ,Ttr_PayrollType
        ,Ttr_CalendarGroup
        ,Ttr_AssumedPost
        ,Ttr_InitialOffsetMin
        ,Ttr_AppliedOffsetMin
        ,Ttr_CompOffsetMin
        ,Ttr_CompLT2Min
        ,Ttr_CompUT1Min
        ,Ttr_CompUT2Min
        ,Ttr_Grade
        ,Ttr_PayrollGroup
        ,Ttr_PremiumGrpCode
        ,Usr_Login
        ,Ludatetime
)
Select A.Ttr_IDNo
	,A.Ttr_Date
    ,@AdjustPayPeriod
    ,A.Ttr_PayCycle
    ,A.Ttr_DayCode
    ,A.Ttr_ShiftCode
    ,A.Ttr_HolidayFlag
    ,A.Ttr_RestDayFlag
    ,A.Ttr_ActIn_1
    ,A.Ttr_ActOut_1
    ,A.Ttr_ActIn_2
    ,A.Ttr_ActOut_2
    ,A.Ttr_ConvIn_1Min
    ,A.Ttr_ConvOut_1Min
    ,A.Ttr_ConvIn_2Min
    ,A.Ttr_ConvOut_2Min
    ,A.Ttr_CompIn_1Min
    ,A.Ttr_CompOut_1Min
    ,A.Ttr_CompIn_2Min
    ,A.Ttr_CompOut_2Min
    ,A.Ttr_CompAdvOTMin
    ,A.Ttr_ShiftIn_1Min
    ,A.Ttr_ShiftOut_1Min
    ,A.Ttr_ShiftIn_2Min
    ,A.Ttr_ShiftOut_2Min
    ,A.Ttr_ShiftMin
    ,A.Ttr_ScheduleType
    ,A.Ttr_WFPayLVCode
    ,A.Ttr_WFPayLVHr
    ,A.Ttr_PayLVMin
    ,A.Ttr_ExcLVMin
    ,A.Ttr_WFNoPayLVCode
    ,A.Ttr_WFNoPayLVHr
    ,A.Ttr_NoPayLVMin
    ,A.Ttr_WFOTAdvHr
    ,A.Ttr_WFOTPostHr
    ,A.Ttr_OTMin
    ,A.Ttr_CompOTMin
    ,A.Ttr_OffsetOTMin
    ,A.Ttr_CompLT1Min
    ,A.Ttr_LTPostFlag
    ,A.Ttr_InitialABSMin
    ,A.Ttr_CompABSMin
    ,A.Ttr_CompREGMin
    ,A.Ttr_CompWorkMin
    ,A.Ttr_CompNDMin
    ,A.Ttr_CompNDOTMin
    ,A.Ttr_PrvDayWorkMin
    ,A.Ttr_PrvDayHolRef
    ,A.Ttr_SkipServiceFlag
    ,A.Ttr_SkipServiceBy
    ,A.Ttr_SkipServiceDate
    ,A.Ttr_AssumedFlag
    ,A.Ttr_AssumedBy
    ,A.Ttr_AssumedDate
    ,A.Ttr_PaidHOLHour
    ,A.Ttr_EmploymentStatusCode
    ,A.Ttr_ForceLeaveDate
    ,A.Ttr_ForOffsetMin
    ,A.Ttr_ExcOffset
    ,A.Ttr_EarnedSatOff
    ,A.Ttr_RESTLEGHOLDay
    ,A.Ttr_WorkDay
    ,A.Ttr_MealDay
    ,A.Ttr_EXPHour
    ,A.Ttr_ABSHour
    ,A.Ttr_REGHour
    ,A.Ttr_OTHour
    ,A.Ttr_NDHour
    ,A.Ttr_NDOTHour
    ,A.Ttr_LVHour
    ,A.Ttr_NextDayHour
    ,A.Ttr_TBAmt01
    ,A.Ttr_TBAmt02
    ,A.Ttr_TBAmt03
    ,A.Ttr_TBAmt04
    ,A.Ttr_TBAmt05
    ,A.Ttr_TBAmt06
    ,A.Ttr_TBAmt07
    ,A.Ttr_TBAmt08
    ,A.Ttr_TBAmt09
    ,A.Ttr_TBAmt10
    ,A.Ttr_TBAmt11
    ,A.Ttr_TBAmt12
    ,A.Ttr_WorkLocationCode
    ,A.Ttr_FlextimeFlag
    ,A.Ttr_TagFlextime
    ,A.Ttr_WFTimeMod
    ,A.Ttr_PayrollType
    ,A.Ttr_CalendarGroup
    ,A.Ttr_AssumedPost
    ,A.Ttr_InitialOffsetMin
    ,A.Ttr_AppliedOffsetMin
    ,A.Ttr_CompOffsetMin
    ,A.Ttr_CompLT2Min
    ,A.Ttr_CompUT1Min
    ,A.Ttr_CompUT2Min
    ,A.Ttr_Grade
    ,A.Ttr_PayrollGroup
    ,A.Ttr_PremiumGrpCode
    ,A.Usr_Login
    ,A.Ludatetime
From {0}..T_EmpTimeRegisterHst A
LEFT JOIN {0}..T_EmpTimeRegisterTrl B
ON A.Ttr_IDNo = B.Ttr_IDNo
	AND A.Ttr_Date = B.Ttr_Date
    AND A.Ttr_PayCycle = B.Ttr_PayCycle
	AND B.Ttr_AdjPayCycle = @AdjustPayPeriod
WHERE B.Ttr_IDNo IS NULL
    AND A.Ttr_PayCycle = @AffectedPayPeriod
    AND A.Ttr_IDNo = '{1}'

--INSERT INTO PAYROLL TRANSACTION TRAIL
INSERT INTO {0}..T_EmpPayTranHdrTrl
(
	  Tph_IDNo
        ,Tph_AdjPayCycle
        ,Tph_PayCycle
        ,Tph_LTHr
        ,Tph_UTHr
        ,Tph_UPLVHr
        ,Tph_ABSLEGHOLHr
        ,Tph_ABSSPLHOLHr
        ,Tph_ABSCOMPHOLHr
        ,Tph_ABSPSDHr
        ,Tph_ABSOTHHOLHr
        ,Tph_WDABSHr
        ,Tph_ABSHr
        ,Tph_REGHr
        ,Tph_PDLVHr
        ,Tph_PDLEGHOLHr
        ,Tph_PDSPLHOLHr
        ,Tph_PDCOMPHOLHr
        ,Tph_PDPSDHr
        ,Tph_PDOTHHOLHr
        ,Tph_PDRESTLEGHOLHr
        ,Tph_REGOTHr
        ,Tph_REGNDHr
        ,Tph_REGNDOTHr
        ,Tph_RESTHr
        ,Tph_RESTOTHr
        ,Tph_RESTNDHr
        ,Tph_RESTNDOTHr
        ,Tph_LEGHOLHr
        ,Tph_LEGHOLOTHr
        ,Tph_LEGHOLNDHr
        ,Tph_LEGHOLNDOTHr
        ,Tph_SPLHOLHr
        ,Tph_SPLHOLOTHr
        ,Tph_SPLHOLNDHr
        ,Tph_SPLHOLNDOTHr
        ,Tph_PSDHr
        ,Tph_PSDOTHr
        ,Tph_PSDNDHr
        ,Tph_PSDNDOTHr
        ,Tph_COMPHOLHr
        ,Tph_COMPHOLOTHr
        ,Tph_COMPHOLNDHr
        ,Tph_COMPHOLNDOTHr
        ,Tph_RESTLEGHOLHr
        ,Tph_RESTLEGHOLOTHr
        ,Tph_RESTLEGHOLNDHr
        ,Tph_RESTLEGHOLNDOTHr
        ,Tph_RESTSPLHOLHr
        ,Tph_RESTSPLHOLOTHr
        ,Tph_RESTSPLHOLNDHr
        ,Tph_RESTSPLHOLNDOTHr
        ,Tph_RESTCOMPHOLHr
        ,Tph_RESTCOMPHOLOTHr
        ,Tph_RESTCOMPHOLNDHr
        ,Tph_RESTCOMPHOLNDOTHr
        ,Tph_RESTPSDHr
        ,Tph_RESTPSDOTHr
        ,Tph_RESTPSDNDHr
        ,Tph_RESTPSDNDOTHr
        ,Tph_SRGAdjHr
        ,Tph_SRGAdjAmt
        ,Tph_SOTAdjHr
        ,Tph_SOTAdjAmt
        ,Tph_SHOLAdjHr
        ,Tph_SHOLAdjAmt
        ,Tph_SNDAdjHr
        ,Tph_SNDAdjAmt
        ,Tph_SLVAdjHr
        ,Tph_SLVAdjAmt
        ,Tph_MRGAdjHr
        ,Tph_MRGAdjAmt
        ,Tph_MOTAdjHr
        ,Tph_MOTAdjAmt
        ,Tph_MHOLAdjHr
        ,Tph_MHOLAdjAmt
        ,Tph_MNDAdjHr
        ,Tph_MNDAdjAmt
        ,Tph_TotalAdjAmt
        ,Tph_TaxableIncomeAmt
        ,Tph_NontaxableIncomeAmt
        ,Tph_WorkDay
        ,Tph_PayrollType
        ,Tph_RetainUserEntry
        ,Usr_Login
        ,Ludatetime
)
Select A.Tph_IDNo
        ,@AdjustPayPeriod
        ,A.Tph_PayCycle
        ,A.Tph_LTHr
        ,A.Tph_UTHr
        ,A.Tph_UPLVHr
        ,A.Tph_ABSLEGHOLHr
        ,A.Tph_ABSSPLHOLHr
        ,A.Tph_ABSCOMPHOLHr
        ,A.Tph_ABSPSDHr
        ,A.Tph_ABSOTHHOLHr
        ,A.Tph_WDABSHr
        ,A.Tph_ABSHr
        ,A.Tph_REGHr
        ,A.Tph_PDLVHr
        ,A.Tph_PDLEGHOLHr
        ,A.Tph_PDSPLHOLHr
        ,A.Tph_PDCOMPHOLHr
        ,A.Tph_PDPSDHr
        ,A.Tph_PDOTHHOLHr
        ,A.Tph_PDRESTLEGHOLHr
        ,A.Tph_REGOTHr
        ,A.Tph_REGNDHr
        ,A.Tph_REGNDOTHr
        ,A.Tph_RESTHr
        ,A.Tph_RESTOTHr
        ,A.Tph_RESTNDHr
        ,A.Tph_RESTNDOTHr
        ,A.Tph_LEGHOLHr
        ,A.Tph_LEGHOLOTHr
        ,A.Tph_LEGHOLNDHr
        ,A.Tph_LEGHOLNDOTHr
        ,A.Tph_SPLHOLHr
        ,A.Tph_SPLHOLOTHr
        ,A.Tph_SPLHOLNDHr
        ,A.Tph_SPLHOLNDOTHr
        ,A.Tph_PSDHr
        ,A.Tph_PSDOTHr
        ,A.Tph_PSDNDHr
        ,A.Tph_PSDNDOTHr
        ,A.Tph_COMPHOLHr
        ,A.Tph_COMPHOLOTHr
        ,A.Tph_COMPHOLNDHr
        ,A.Tph_COMPHOLNDOTHr
        ,A.Tph_RESTLEGHOLHr
        ,A.Tph_RESTLEGHOLOTHr
        ,A.Tph_RESTLEGHOLNDHr
        ,A.Tph_RESTLEGHOLNDOTHr
        ,A.Tph_RESTSPLHOLHr
        ,A.Tph_RESTSPLHOLOTHr
        ,A.Tph_RESTSPLHOLNDHr
        ,A.Tph_RESTSPLHOLNDOTHr
        ,A.Tph_RESTCOMPHOLHr
        ,A.Tph_RESTCOMPHOLOTHr
        ,A.Tph_RESTCOMPHOLNDHr
        ,A.Tph_RESTCOMPHOLNDOTHr
        ,A.Tph_RESTPSDHr
        ,A.Tph_RESTPSDOTHr
        ,A.Tph_RESTPSDNDHr
        ,A.Tph_RESTPSDNDOTHr
        ,A.Tph_SRGAdjHr
        ,A.Tph_SRGAdjAmt
        ,A.Tph_SOTAdjHr
        ,A.Tph_SOTAdjAmt
        ,A.Tph_SHOLAdjHr
        ,A.Tph_SHOLAdjAmt
        ,A.Tph_SNDAdjHr
        ,A.Tph_SNDAdjAmt
        ,A.Tph_SLVAdjHr
        ,A.Tph_SLVAdjAmt
        ,A.Tph_MRGAdjHr
        ,A.Tph_MRGAdjAmt
        ,A.Tph_MOTAdjHr
        ,A.Tph_MOTAdjAmt
        ,A.Tph_MHOLAdjHr
        ,A.Tph_MHOLAdjAmt
        ,A.Tph_MNDAdjHr
        ,A.Tph_MNDAdjAmt
        ,A.Tph_TotalAdjAmt
        ,A.Tph_TaxableIncomeAmt
        ,A.Tph_NontaxableIncomeAmt
        ,A.Tph_WorkDay
        ,A.Tph_PayrollType
        ,A.Tph_RetainUserEntry
        ,A.Usr_Login
        ,A.Ludatetime
From {0}..T_EmpPayTranHdrHst A
LEFT JOIN {0}..T_EmpPayTranHdrTrl B
ON A.Tph_IDNo = B.Tph_IDNo
	AND A.Tph_PayCycle = B.Tph_PayCycle
    AND B.Tph_AdjPayCycle = @AdjustPayPeriod
WHERE B.Tph_IDNo IS NULL
    AND A.Tph_PayCycle = @AffectedPayPeriod
    AND A.Tph_IDNo = '{1}'

--INSERT INTO PAYROLL TRANSACTION TRAIL DETAIL
INSERT INTO {0}..T_EmpPayTranDtlTrl
(
	   Tpd_IDNo
        ,Tpd_AdjPayCycle
        ,Tpd_PayCycle
        ,Tpd_Date
        ,Tpd_SalaryRate
        ,Tpd_HourRate
        ,Tpd_PayrollType
        ,Tpd_LTHr
        ,Tpd_UTHr
        ,Tpd_UPLVHr
        ,Tpd_ABSLEGHOLHr
        ,Tpd_ABSSPLHOLHr
        ,Tpd_ABSCOMPHOLHr
        ,Tpd_ABSPSDHr
        ,Tpd_ABSOTHHOLHr
        ,Tpd_WDABSHr
        ,Tpd_ABSHr
        ,Tpd_REGHr
        ,Tpd_PDLVHr
        ,Tpd_PDLEGHOLHr
        ,Tpd_PDSPLHOLHr
        ,Tpd_PDCOMPHOLHr
        ,Tpd_PDPSDHr
        ,Tpd_PDOTHHOLHr
        ,Tpd_PDRESTLEGHOLHr
        ,Tpd_REGOTHr
        ,Tpd_REGNDHr
        ,Tpd_REGNDOTHr
        ,Tpd_RESTHr
        ,Tpd_RESTOTHr
        ,Tpd_RESTNDHr
        ,Tpd_RESTNDOTHr
        ,Tpd_LEGHOLHr
        ,Tpd_LEGHOLOTHr
        ,Tpd_LEGHOLNDHr
        ,Tpd_LEGHOLNDOTHr
        ,Tpd_SPLHOLHr
        ,Tpd_SPLHOLOTHr
        ,Tpd_SPLHOLNDHr
        ,Tpd_SPLHOLNDOTHr
        ,Tpd_PSDHr
        ,Tpd_PSDOTHr
        ,Tpd_PSDNDHr
        ,Tpd_PSDNDOTHr
        ,Tpd_COMPHOLHr
        ,Tpd_COMPHOLOTHr
        ,Tpd_COMPHOLNDHr
        ,Tpd_COMPHOLNDOTHr
        ,Tpd_RESTLEGHOLHr
        ,Tpd_RESTLEGHOLOTHr
        ,Tpd_RESTLEGHOLNDHr
        ,Tpd_RESTLEGHOLNDOTHr
        ,Tpd_RESTSPLHOLHr
        ,Tpd_RESTSPLHOLOTHr
        ,Tpd_RESTSPLHOLNDHr
        ,Tpd_RESTSPLHOLNDOTHr
        ,Tpd_RESTCOMPHOLHr
        ,Tpd_RESTCOMPHOLOTHr
        ,Tpd_RESTCOMPHOLNDHr
        ,Tpd_RESTCOMPHOLNDOTHr
        ,Tpd_RESTPSDHr
        ,Tpd_RESTPSDOTHr
        ,Tpd_RESTPSDNDHr
        ,Tpd_RESTPSDNDOTHr
        ,Tpd_WorkDay
        ,Tpd_PremiumGrpCode
        ,Usr_Login
        ,Ludatetime
)
Select A.Tpd_IDNo
       ,@AdjustPayPeriod
       ,A.Tpd_PayCycle
        ,A.Tpd_Date
        ,A.Tpd_SalaryRate
        ,A.Tpd_HourRate
        ,A.Tpd_PayrollType
        ,A.Tpd_LTHr
        ,A.Tpd_UTHr
        ,A.Tpd_UPLVHr
        ,A.Tpd_ABSLEGHOLHr
        ,A.Tpd_ABSSPLHOLHr
        ,A.Tpd_ABSCOMPHOLHr
        ,A.Tpd_ABSPSDHr
        ,A.Tpd_ABSOTHHOLHr
        ,A.Tpd_WDABSHr
        ,A.Tpd_ABSHr
        ,A.Tpd_REGHr
        ,A.Tpd_PDLVHr
        ,A.Tpd_PDLEGHOLHr
        ,A.Tpd_PDSPLHOLHr
        ,A.Tpd_PDCOMPHOLHr
        ,A.Tpd_PDPSDHr
        ,A.Tpd_PDOTHHOLHr
        ,A.Tpd_PDRESTLEGHOLHr
        ,A.Tpd_REGOTHr
        ,A.Tpd_REGNDHr
        ,A.Tpd_REGNDOTHr
        ,A.Tpd_RESTHr
        ,A.Tpd_RESTOTHr
        ,A.Tpd_RESTNDHr
        ,A.Tpd_RESTNDOTHr
        ,A.Tpd_LEGHOLHr
        ,A.Tpd_LEGHOLOTHr
        ,A.Tpd_LEGHOLNDHr
        ,A.Tpd_LEGHOLNDOTHr
        ,A.Tpd_SPLHOLHr
        ,A.Tpd_SPLHOLOTHr
        ,A.Tpd_SPLHOLNDHr
        ,A.Tpd_SPLHOLNDOTHr
        ,A.Tpd_PSDHr
        ,A.Tpd_PSDOTHr
        ,A.Tpd_PSDNDHr
        ,A.Tpd_PSDNDOTHr
        ,A.Tpd_COMPHOLHr
        ,A.Tpd_COMPHOLOTHr
        ,A.Tpd_COMPHOLNDHr
        ,A.Tpd_COMPHOLNDOTHr
        ,A.Tpd_RESTLEGHOLHr
        ,A.Tpd_RESTLEGHOLOTHr
        ,A.Tpd_RESTLEGHOLNDHr
        ,A.Tpd_RESTLEGHOLNDOTHr
        ,A.Tpd_RESTSPLHOLHr
        ,A.Tpd_RESTSPLHOLOTHr
        ,A.Tpd_RESTSPLHOLNDHr
        ,A.Tpd_RESTSPLHOLNDOTHr
        ,A.Tpd_RESTCOMPHOLHr
        ,A.Tpd_RESTCOMPHOLOTHr
        ,A.Tpd_RESTCOMPHOLNDHr
        ,A.Tpd_RESTCOMPHOLNDOTHr
        ,A.Tpd_RESTPSDHr
        ,A.Tpd_RESTPSDOTHr
        ,A.Tpd_RESTPSDNDHr
        ,A.Tpd_RESTPSDNDOTHr
        ,A.Tpd_WorkDay
        ,A.Tpd_PremiumGrpCode
        ,A.Usr_Login
        ,A.Ludatetime
From {0}..T_EmpPayTranDtlHst A
LEFT JOIN {0}..T_EmpPayTranDtlTrl B
ON A.Tpd_IDNo = B.Tpd_IDNo
	AND A.Tpd_PayCycle = B.Tpd_PayCycle
	AND A.Tpd_Date = B.Tpd_Date
    AND B.Tpd_AdjPayCycle = @AdjustPayPeriod
WHERE B.Tpd_IDNo IS NULL
    AND A.Tpd_PayCycle = @AffectedPayPeriod
    AND A.Tpd_IDNo = '{1}'
	
--INSERT INTO PAYROLL TRANSACTION TRAIL EXT
INSERT INTO {0}..T_EmpPayTranHdrMiscTrl
(
	   Tph_IDNo
      ,Tph_AdjPayCycle
      ,Tph_PayCycle
      ,Tph_Misc1Hr
      ,Tph_Misc1OTHr
      ,Tph_Misc1NDHr
      ,Tph_Misc1NDOTHr
      ,Tph_Misc2Hr
      ,Tph_Misc2OTHr
      ,Tph_Misc2NDHr
      ,Tph_Misc2NDOTHr
      ,Tph_Misc3Hr
      ,Tph_Misc3OTHr
      ,Tph_Misc3NDHr
      ,Tph_Misc3NDOTHr
      ,Tph_Misc4Hr
      ,Tph_Misc4OTHr
      ,Tph_Misc4NDHr
      ,Tph_Misc4NDOTHr
      ,Tph_Misc5Hr
      ,Tph_Misc5OTHr
      ,Tph_Misc5NDHr
      ,Tph_Misc5NDOTHr
      ,Tph_Misc6Hr
      ,Tph_Misc6OTHr
      ,Tph_Misc6NDHr
      ,Tph_Misc6NDOTHr
      ,Usr_Login
      ,Ludatetime
)
Select A.Tph_IDNo
      ,@AdjustPayPeriod
      ,A.Tph_PayCycle
      ,A.Tph_Misc1Hr
      ,A.Tph_Misc1OTHr
      ,A.Tph_Misc1NDHr
      ,A.Tph_Misc1NDOTHr
      ,A.Tph_Misc2Hr
      ,A.Tph_Misc2OTHr
      ,A.Tph_Misc2NDHr
      ,A.Tph_Misc2NDOTHr
      ,A.Tph_Misc3Hr
      ,A.Tph_Misc3OTHr
      ,A.Tph_Misc3NDHr
      ,A.Tph_Misc3NDOTHr
      ,A.Tph_Misc4Hr
      ,A.Tph_Misc4OTHr
      ,A.Tph_Misc4NDHr
      ,A.Tph_Misc4NDOTHr
      ,A.Tph_Misc5Hr
      ,A.Tph_Misc5OTHr
      ,A.Tph_Misc5NDHr
      ,A.Tph_Misc5NDOTHr
      ,A.Tph_Misc6Hr
      ,A.Tph_Misc6OTHr
      ,A.Tph_Misc6NDHr
      ,A.Tph_Misc6NDOTHr
      ,A.Usr_Login
      ,A.Ludatetime
From {0}..T_EmpPayTranHdrMiscHst A
LEFT JOIN {0}..T_EmpPayTranHdrMiscTrl B
ON A.Tph_IDNo = B.Tph_IDNo
	AND A.Tph_PayCycle = B.Tph_PayCycle
    AND B.Tph_AdjPayCycle = @AdjustPayPeriod
WHERE B.Tph_IDNo IS NULL
    AND A.Tph_PayCycle = @AffectedPayPeriod
    AND A.Tph_IDNo = '{1}'
	
--INSERT INTO PAYROLL TRANSACTION TRAIL EXT DETAIL
INSERT INTO {0}..T_EmpPayTranDtlMiscTrl
(
	   Tpd_IDNo
        ,Tpd_AdjPayCycle
        ,Tpd_PayCycle
        ,Tpd_Date
        ,Tpd_Misc1Hr
        ,Tpd_Misc1OTHr
        ,Tpd_Misc1NDHr
        ,Tpd_Misc1NDOTHr
        ,Tpd_Misc2Hr
        ,Tpd_Misc2OTHr
        ,Tpd_Misc2NDHr
        ,Tpd_Misc2NDOTHr
        ,Tpd_Misc3Hr
        ,Tpd_Misc3OTHr
        ,Tpd_Misc3NDHr
        ,Tpd_Misc3NDOTHr
        ,Tpd_Misc4Hr
        ,Tpd_Misc4OTHr
        ,Tpd_Misc4NDHr
        ,Tpd_Misc4NDOTHr
        ,Tpd_Misc5Hr
        ,Tpd_Misc5OTHr
        ,Tpd_Misc5NDHr
        ,Tpd_Misc5NDOTHr
        ,Tpd_Misc6Hr
        ,Tpd_Misc6OTHr
        ,Tpd_Misc6NDHr
        ,Tpd_Misc6NDOTHr
        ,Usr_Login
        ,Ludatetime
)
Select A.Tpd_IDNo
        ,@AdjustPayPeriod
        ,A.Tpd_PayCycle
        ,A.Tpd_Date
        ,A.Tpd_Misc1Hr
        ,A.Tpd_Misc1OTHr
        ,A.Tpd_Misc1NDHr
        ,A.Tpd_Misc1NDOTHr
        ,A.Tpd_Misc2Hr
        ,A.Tpd_Misc2OTHr
        ,A.Tpd_Misc2NDHr
        ,A.Tpd_Misc2NDOTHr
        ,A.Tpd_Misc3Hr
        ,A.Tpd_Misc3OTHr
        ,A.Tpd_Misc3NDHr
        ,A.Tpd_Misc3NDOTHr
        ,A.Tpd_Misc4Hr
        ,A.Tpd_Misc4OTHr
        ,A.Tpd_Misc4NDHr
        ,A.Tpd_Misc4NDOTHr
        ,A.Tpd_Misc5Hr
        ,A.Tpd_Misc5OTHr
        ,A.Tpd_Misc5NDHr
        ,A.Tpd_Misc5NDOTHr
        ,A.Tpd_Misc6Hr
        ,A.Tpd_Misc6OTHr
        ,A.Tpd_Misc6NDHr
        ,A.Tpd_Misc6NDOTHr
        ,A.Usr_Login
        ,A.Ludatetime
From {0}..T_EmpPayTranDtlMiscHst A
LEFT JOIN {0}..T_EmpPayTranDtlMiscTrl B
ON A.Tpd_IDNo = B.Tpd_IDNo
	AND A.Tpd_PayCycle = B.Tpd_PayCycle
	AND A.Tpd_Date = B.Tpd_Date
    AND B.Tpd_AdjPayCycle = @AdjustPayPeriod
WHERE B.Tpd_IDNo IS NULL
    AND A.Tpd_PayCycle = @AffectedPayPeriod
    AND A.Tpd_IDNo = '{1}' ", strDBName, drArrLogLedger[x]["Ttr_IDNo"], dtProcessDate.ToShortDateString());
                    #endregion
                    dal.ExecuteNonQuery(strTrailPrevious);
                    #endregion
                }

                strUpdateQuery += string.Format(strUpdateTemplate
                                                , strDBName
                                                , drArrLogLedger[x]["Ttr_IDNo"]
                                                , drArrLogLedger[x]["Ttr_Date"]
                                                , drArrLogLedger[x]["Ttr_ActIn_1"]
                                                , drArrLogLedger[x]["Ttr_ActOut_1"]
                                                , drArrLogLedger[x]["Ttr_ActIn_2"]
                                                , drArrLogLedger[x]["Ttr_ActOut_2"]
                                                , strLogLedgerTable);
                iUpdateCtr++;
                if (iUpdateCtr == 150) //150 log ledger records per batch update
                {
                    dal.ExecuteNonQuery(strUpdateQuery);
                    strUpdateQuery = "";
                    iUpdateCtr = 0;
                }
            }

            if (strUpdateQuery != "")
                dal.ExecuteNonQuery(strUpdateQuery);

            Console.Write(" {0} Successfully Saved", drArrLogLedger.Length);
            #endregion

            #region Save DTR Records
            Console.Write("\n{0} Saving DTR...", dtProcessDate.ToShortDateString());
            drArrDTR = dtDTR.Select("Edited = 1 OR (PostFlag = 'True' AND Edited = 0)");
            strUpdateTemplate = @"UPDATE {0}..T_EmpDTR SET Tel_IsPosted = {4} WHERE Tel_IDNo = '{1}' AND Tel_LogDate = '{2}' AND Tel_LogTime = '{3}' AND Tel_LogType = '{5}' ";
            strUpdateQuery = "";
            iUpdateCtr = 0;
            for (int x = 0; x < drArrDTR.Length; x++)
            {
                strUpdateQuery += string.Format(strUpdateTemplate
                                                , Globals.DTRDBName
                                                , drArrDTR[x]["EmployeeID"]
                                                , drArrDTR[x]["LogDate"]
                                                , drArrDTR[x]["LogTime"]
                                                , drArrDTR[x]["Edited"]
                                                , drArrDTR[x]["LogType"]);
                iUpdateCtr++;
                if (iUpdateCtr == 100) //from 150 to 100 DTR records per batch update
                {
                    dal.ExecuteNonQuery(strUpdateQuery);
                    strUpdateQuery = "";
                    iUpdateCtr = 0;
                }
            }

            if (strUpdateQuery != "")
                dal.ExecuteNonQuery(strUpdateQuery);

            Console.Write(" {0} Successfully Saved", drArrDTR.Length);
            #endregion

            Console.WriteLine("\n{0} Updating Log Control record...", dtProcessDate.ToShortDateString());
            UpdateLogControlTable(strDBName, dtProcessDate, strLogLedgerTable, dal);

            Console.WriteLine("End: {0:M/dd/yyyy HH:mm:ss} ", DateTime.Now);
            return drArrDTR.Length;
        }

        public int PostLogsMultiplePockets(string strDBName, string strCompanyCode, DateTime dtProcessDate, bool bPostPartial, string strSelectedEmployeeID, DALHelper dal)
        {
            #region Variables
            DataRow[] drArrLogLedger, drArrDTR;
            string strEmployeeID, strScheduleType;
            int iActualTimeIn1Min, iActualTimeOut1Min, iActualTimeIn2Min, iActualTimeOut2Min,
                iActualTimeIn3Min, iActualTimeOut3Min, iActualTimeIn4Min, iActualTimeOut4Min,
                iActualTimeIn5Min, iActualTimeOut5Min, iActualTimeIn6Min, iActualTimeOut6Min,
                iActualTimeIn7Min, iActualTimeOut7Min, iActualTimeIn8Min, iActualTimeOut8Min,
                iActualTimeIn9Min, iActualTimeOut9Min, iActualTimeIn10Min, iActualTimeOut10Min,
                iActualTimeIn11Min, iActualTimeOut11Min, iActualTimeIn12Min, iActualTimeOut12Min;
            int iShiftTimeIn1Min, iShiftTimeOut1Min, iShiftTimeIn2Min, iShiftTimeOut2Min;
            DateTime dtDTRDate;
            int iLogTime, iLogTimeOrig;
            bool bPostFlag, bEdited, bIsInHistTable = false;
            string SkipServiceFlag = "";
            const int GRAVEYARD24 = 1440;
            bool bNextDayLogs = false;
            //bool RequiredLogsOnBreak = false;
            int iFirstHalf = 0;
            #endregion

            Console.WriteLine("\nStart: {0:M/dd/yyyy HH:mm:ss} ", DateTime.Now);
            Console.WriteLine("{0} Getting Log Ledger records...", dtProcessDate.ToShortDateString());
            dtEmployeeLogLedger = GetLogLedgerRecordsPerDateMultiplePockets(strDBName, strCompanyCode, dtProcessDate, strSelectedEmployeeID, dal);
            Console.WriteLine("{0} Getting DTR records...", dtProcessDate.ToShortDateString());
            dtDTR = GetDTRRecordsPerDateMultiplePockets(strDBName, dtProcessDate, strSelectedEmployeeID, dal);

            for (int i = 0; i < dtEmployeeLogLedger.Rows.Count; i++)
            {
                Console.Write("\r{0} Posting logs {1} of {2}...", dtProcessDate.ToShortDateString(), i + 1, dtEmployeeLogLedger.Rows.Count);

                #region Initialize Variables
                bEdited = false;
                
                strEmployeeID = dtEmployeeLogLedger.Rows[i]["Ttm_IDNo"].ToString();
                strScheduleType = dtEmployeeLogLedger.Rows[i]["ScheduleType"].ToString();
                SkipServiceFlag = dtEmployeeLogLedger.Rows[i]["SkipService"].ToString();

                iActualTimeIn1Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttm_ActIn_01"].ToString());
                iActualTimeOut1Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttm_ActOut_01"].ToString());
                iActualTimeIn2Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttm_ActIn_02"].ToString());
                iActualTimeOut2Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttm_ActOut_02"].ToString());
                iActualTimeIn3Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttm_ActIn_03"].ToString());
                iActualTimeOut3Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttm_ActOut_03"].ToString());
                iActualTimeIn4Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttm_ActIn_04"].ToString());
                iActualTimeOut4Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttm_ActOut_04"].ToString());
                iActualTimeIn5Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttm_ActIn_05"].ToString());
                iActualTimeOut5Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttm_ActOut_05"].ToString());
                iActualTimeIn6Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttm_ActIn_06"].ToString());
                iActualTimeOut6Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttm_ActOut_06"].ToString());
                iActualTimeIn7Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttm_ActIn_07"].ToString());
                iActualTimeOut7Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttm_ActOut_07"].ToString());
                iActualTimeIn8Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttm_ActIn_08"].ToString());
                iActualTimeOut8Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttm_ActOut_08"].ToString());
                iActualTimeIn9Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttm_ActIn_09"].ToString());
                iActualTimeOut9Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttm_ActOut_09"].ToString());
                iActualTimeIn10Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttm_ActIn_10"].ToString());
                iActualTimeOut10Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttm_ActOut_10"].ToString());
                iActualTimeIn11Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttm_ActIn_11"].ToString());
                iActualTimeOut11Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttm_ActOut_11"].ToString());
                iActualTimeIn12Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttm_ActIn_12"].ToString());
                iActualTimeOut12Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttm_ActOut_12"].ToString());
                
                iShiftTimeIn1Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["ShiftTimeIn"].ToString());
                iShiftTimeOut1Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["ShiftBreakStart"].ToString());
                iShiftTimeIn2Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["ShiftBreakEnd"].ToString());
                iShiftTimeOut2Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["ShiftTimeOut"].ToString());
                //RequiredLogsOnBreak = Convert.ToBoolean(dtEmployeeLogLedger.Rows[i]["RequiredLogsOnBreak"].ToString()); //TRUE = BASIC 4 + Extension; FALSE = Extension

                if (dtEmployeeLogLedger.Rows[i]["TableName"].ToString() == "T_EmpTimeRegisterMiscHst")
                    bIsInHistTable = true;

                //Convert to Graveyard
                if (iShiftTimeIn1Min > iShiftTimeOut1Min)
                    iShiftTimeOut1Min = iShiftTimeOut1Min + GRAVEYARD24;
                if (iShiftTimeIn1Min > iShiftTimeIn2Min)
                    iShiftTimeIn2Min = iShiftTimeIn2Min + GRAVEYARD24;
                if (iShiftTimeIn1Min > iShiftTimeOut2Min)
                    iShiftTimeOut2Min = iShiftTimeOut2Min + GRAVEYARD24;

                drArrDTR = dtDTR.Select(string.Format("EmployeeID = '{0}'", strEmployeeID));
                iFirstHalf = 0;
                #endregion

                if (SkipServiceFlag.Equals("N"))
                {
                    #region Loop through DTR Logs (Regular FILO Posting)
                    for (int x = 0; x < drArrDTR.Length; x++)
                    {
                        #region Initialize Variables
                        dtDTRDate = Convert.ToDateTime(drArrDTR[x]["LogDate"].ToString());
                        iLogTime = GetMinsFromHourStr(drArrDTR[x]["LogTime"].ToString());
                        iLogTimeOrig = iLogTime;
                        bPostFlag = (drArrDTR[x]["PostFlag"].ToString().Equals("1") ? true : false);
                        bNextDayLogs = false;

                        if (dtProcessDate.AddDays(1) == dtDTRDate 
                            && iLogTime < (iShiftTimeIn1Min - Globals.EXTENDIN1) //Cut-off between last out of previous and first in of the current day
                            && (iLogTime + GRAVEYARD24) <= (iShiftTimeOut1Min + GRAVEYARD24)) //Do not post if greater than Shift Break of Next Day
                        {
                            bNextDayLogs = true;
                            iLogTimeOrig = iLogTime + GRAVEYARD24;
                            iLogTime = iLogTime + GRAVEYARD24;
                        }
                        #endregion

                        #region Clean-up
                        if ((dtProcessDate == dtDTRDate && iLogTime != 0 && iLogTime >= (iShiftTimeIn1Min - Globals.EXTENDIN1))
                            || (iLogTime != 0 && bNextDayLogs))
                        {
                            if (x == 0 && iLogTime >= (iShiftTimeIn1Min - Globals.EXTENDIN1) && iLogTime <= iShiftTimeIn1Min)
                            {
                                iActualTimeIn1Min = iLogTimeOrig;
                                drArrDTR[x]["PostFlag"] = 1;
                                drArrDTR[x]["Edited"] = 1;
                                bEdited = true;
                                if (iLogTime < iShiftTimeIn2Min)
                                    iFirstHalf++;
                            }
                            else
                            {

                                if (iActualTimeIn1Min == 0)
                                {
                                    iActualTimeIn1Min = iLogTimeOrig;
                                    drArrDTR[x]["PostFlag"] = 1;
                                    drArrDTR[x]["Edited"] = 1;
                                    bEdited = true;
                                    if (iLogTime < iShiftTimeIn2Min)
                                        iFirstHalf++;
                                }
                                else if (iActualTimeOut1Min == 0)
                                {
                                    if ((iLogTime - iActualTimeIn1Min) >= Globals.POCKETTIME)
                                    {
                                        iActualTimeOut1Min = iLogTimeOrig;
                                        drArrDTR[x]["PostFlag"] = 1;
                                        drArrDTR[x]["Edited"] = 1;
                                        bEdited = true;
                                        if (iLogTime < iShiftTimeIn2Min)
                                            iFirstHalf++;
                                    }

                                }
                                else if (iActualTimeIn2Min == 0)
                                {
                                    if ((iLogTime - iActualTimeOut1Min) >= Globals.POCKETGAP)
                                    {
                                        iActualTimeIn2Min = iLogTimeOrig;
                                        drArrDTR[x]["PostFlag"] = 1;
                                        drArrDTR[x]["Edited"] = 1;
                                        bEdited = true;
                                        if (iLogTime < iShiftTimeIn2Min)
                                            iFirstHalf++;
                                    }

                                }
                                else if (iActualTimeOut2Min == 0)
                                {
                                    if ((iLogTime - iActualTimeIn2Min) >= Globals.POCKETTIME)
                                    {
                                        iActualTimeOut2Min = iLogTimeOrig;
                                        drArrDTR[x]["PostFlag"] = 1;
                                        drArrDTR[x]["Edited"] = 1;
                                        bEdited = true;
                                        if (iLogTime < iShiftTimeIn2Min)
                                            iFirstHalf++;
                                    }

                                }
                                else if (iActualTimeIn3Min == 0)
                                {
                                    if ((iLogTime - iActualTimeOut2Min) >= Globals.POCKETGAP)
                                    {
                                        iActualTimeIn3Min = iLogTimeOrig;
                                        drArrDTR[x]["PostFlag"] = 1;
                                        drArrDTR[x]["Edited"] = 1;
                                        bEdited = true;
                                        if (iLogTime < iShiftTimeIn2Min)
                                            iFirstHalf++;
                                    }

                                }
                                else if (iActualTimeOut3Min == 0)
                                {
                                    if ((iLogTime - iActualTimeIn3Min) >= Globals.POCKETTIME)
                                    {
                                        iActualTimeOut3Min = iLogTimeOrig;
                                        drArrDTR[x]["PostFlag"] = 1;
                                        drArrDTR[x]["Edited"] = 1;
                                        bEdited = true;
                                        if (iLogTime < iShiftTimeIn2Min)
                                            iFirstHalf++;
                                    }

                                }
                                else if (iActualTimeIn4Min == 0)
                                {
                                    if ((iLogTime - iActualTimeOut3Min) >= Globals.POCKETGAP)
                                    {
                                        iActualTimeIn4Min = iLogTimeOrig;
                                        drArrDTR[x]["PostFlag"] = 1;
                                        drArrDTR[x]["Edited"] = 1;
                                        bEdited = true;
                                        if (iLogTime < iShiftTimeIn2Min)
                                            iFirstHalf++;
                                    }

                                }
                                else if (iActualTimeOut4Min == 0)
                                {
                                    if ((iLogTime - iActualTimeIn4Min) >= Globals.POCKETTIME)
                                    {
                                        iActualTimeOut4Min = iLogTimeOrig;
                                        drArrDTR[x]["PostFlag"] = 1;
                                        drArrDTR[x]["Edited"] = 1;
                                        bEdited = true;
                                        if (iLogTime < iShiftTimeIn2Min)
                                            iFirstHalf++;
                                    }

                                }
                                else if (iActualTimeIn5Min == 0)
                                {
                                    if ((iLogTime - iActualTimeOut4Min) >= Globals.POCKETGAP)
                                    {
                                        iActualTimeIn5Min = iLogTimeOrig;
                                        drArrDTR[x]["PostFlag"] = 1;
                                        drArrDTR[x]["Edited"] = 1;
                                        bEdited = true;
                                        if (iLogTime < iShiftTimeIn2Min)
                                            iFirstHalf++;
                                    }

                                }
                                else if (iActualTimeOut5Min == 0)
                                {
                                    if ((iLogTime - iActualTimeIn5Min) >= Globals.POCKETTIME)
                                    {
                                        iActualTimeOut5Min = iLogTimeOrig;
                                        drArrDTR[x]["PostFlag"] = 1;
                                        drArrDTR[x]["Edited"] = 1;
                                        bEdited = true;
                                        if (iLogTime < iShiftTimeIn2Min)
                                            iFirstHalf++;
                                    }

                                }
                                else if (iActualTimeIn6Min == 0)
                                {
                                    if ((iLogTime - iActualTimeOut5Min) >= Globals.POCKETGAP)
                                    {
                                        iActualTimeIn6Min = iLogTimeOrig;
                                        drArrDTR[x]["PostFlag"] = 1;
                                        drArrDTR[x]["Edited"] = 1;
                                        bEdited = true;
                                        if (iLogTime < iShiftTimeIn2Min)
                                            iFirstHalf++;
                                    }

                                }
                                else if (iActualTimeOut6Min == 0)
                                {
                                    if ((iLogTime - iActualTimeIn6Min) >= Globals.POCKETTIME)
                                    {
                                        iActualTimeOut6Min = iLogTimeOrig;
                                        drArrDTR[x]["PostFlag"] = 1;
                                        drArrDTR[x]["Edited"] = 1;
                                        bEdited = true;
                                        if (iLogTime < iShiftTimeIn2Min)
                                            iFirstHalf++;
                                    }

                                }
                                else if (iActualTimeIn7Min == 0)
                                {
                                    if ((iLogTime - iActualTimeOut6Min) >= Globals.POCKETGAP)
                                    {
                                        iActualTimeIn7Min = iLogTimeOrig;
                                        drArrDTR[x]["PostFlag"] = 1;
                                        drArrDTR[x]["Edited"] = 1;
                                        bEdited = true;
                                        if (iLogTime < iShiftTimeIn2Min)
                                            iFirstHalf++;
                                    }

                                }
                                else if (iActualTimeOut7Min == 0)
                                {
                                    if ((iLogTime - iActualTimeIn7Min) >= Globals.POCKETTIME)
                                    {
                                        iActualTimeOut7Min = iLogTimeOrig;
                                        drArrDTR[x]["PostFlag"] = 1;
                                        drArrDTR[x]["Edited"] = 1;
                                        bEdited = true;
                                        if (iLogTime < iShiftTimeIn2Min)
                                            iFirstHalf++;
                                    }

                                }
                                else if (iActualTimeIn8Min == 0)
                                {
                                    if ((iLogTime - iActualTimeOut7Min) >= Globals.POCKETGAP)
                                    {
                                        iActualTimeIn8Min = iLogTimeOrig;
                                        drArrDTR[x]["PostFlag"] = 1;
                                        drArrDTR[x]["Edited"] = 1;
                                        bEdited = true;
                                        if (iLogTime < iShiftTimeIn2Min)
                                            iFirstHalf++;
                                    }

                                }
                                else if (iActualTimeOut8Min == 0)
                                {
                                    if ((iLogTime - iActualTimeIn8Min) >= Globals.POCKETTIME)
                                    {
                                        iActualTimeOut8Min = iLogTimeOrig;
                                        drArrDTR[x]["PostFlag"] = 1;
                                        drArrDTR[x]["Edited"] = 1;
                                        bEdited = true;
                                        if (iLogTime < iShiftTimeIn2Min)
                                            iFirstHalf++;
                                    }

                                }
                                else if (iActualTimeIn9Min == 0)
                                {
                                    if ((iLogTime - iActualTimeOut8Min) >= Globals.POCKETGAP)
                                    {
                                        iActualTimeIn9Min = iLogTimeOrig;
                                        drArrDTR[x]["PostFlag"] = 1;
                                        drArrDTR[x]["Edited"] = 1;
                                        bEdited = true;
                                        if (iLogTime < iShiftTimeIn2Min)
                                            iFirstHalf++;
                                    }

                                }
                                else if (iActualTimeOut9Min == 0)
                                {
                                    if ((iLogTime - iActualTimeIn9Min) >= Globals.POCKETTIME)
                                    {
                                        iActualTimeOut9Min = iLogTimeOrig;
                                        drArrDTR[x]["PostFlag"] = 1;
                                        drArrDTR[x]["Edited"] = 1;
                                        bEdited = true;
                                        if (iLogTime < iShiftTimeIn2Min)
                                            iFirstHalf++;
                                    }

                                }
                                else if (iActualTimeIn10Min == 0)
                                {
                                    if ((iLogTime - iActualTimeOut9Min) >= Globals.POCKETGAP)
                                    {
                                        iActualTimeIn10Min = iLogTimeOrig;
                                        drArrDTR[x]["PostFlag"] = 1;
                                        drArrDTR[x]["Edited"] = 1;
                                        bEdited = true;
                                        if (iLogTime < iShiftTimeIn2Min)
                                            iFirstHalf++;
                                    }

                                }
                                else if (iActualTimeOut10Min == 0)
                                {
                                    if ((iLogTime - iActualTimeIn10Min) >= Globals.POCKETTIME)
                                    {
                                        iActualTimeOut10Min = iLogTimeOrig;
                                        drArrDTR[x]["PostFlag"] = 1;
                                        drArrDTR[x]["Edited"] = 1;
                                        bEdited = true;
                                        if (iLogTime < iShiftTimeIn2Min)
                                            iFirstHalf++;
                                    }

                                }
                                else if (iActualTimeIn11Min == 0)
                                {
                                    if ((iLogTime - iActualTimeOut10Min) >= Globals.POCKETGAP)
                                    {
                                        iActualTimeIn11Min = iLogTimeOrig;
                                        drArrDTR[x]["PostFlag"] = 1;
                                        drArrDTR[x]["Edited"] = 1;
                                        bEdited = true;
                                        if (iLogTime < iShiftTimeIn2Min)
                                            iFirstHalf++;
                                    }

                                }
                                else if (iActualTimeOut11Min == 0)
                                {
                                    if ((iLogTime - iActualTimeIn11Min) >= Globals.POCKETTIME)
                                    {
                                        iActualTimeOut11Min = iLogTimeOrig;
                                        drArrDTR[x]["PostFlag"] = 1;
                                        drArrDTR[x]["Edited"] = 1;
                                        bEdited = true;
                                        if (iLogTime < iShiftTimeIn2Min)
                                            iFirstHalf++;
                                    }

                                }
                                else if (iActualTimeIn12Min == 0)
                                {
                                    if ((iLogTime - iActualTimeOut11Min) >= Globals.POCKETGAP)
                                    {
                                        iActualTimeIn12Min = iLogTimeOrig;
                                        drArrDTR[x]["PostFlag"] = 1;
                                        drArrDTR[x]["Edited"] = 1;
                                        bEdited = true;
                                        if (iLogTime < iShiftTimeIn2Min)
                                            iFirstHalf++;
                                    }

                                }
                                else if (iActualTimeOut12Min == 0)
                                {
                                    if ((iLogTime - iActualTimeIn12Min) >= Globals.POCKETTIME)
                                    {
                                        iActualTimeOut12Min = iLogTimeOrig;
                                        drArrDTR[x]["PostFlag"] = 1;
                                        drArrDTR[x]["Edited"] = 1;
                                        bEdited = true;
                                        if (iLogTime < iShiftTimeIn2Min)
                                            iFirstHalf++;
                                    }

                                }
                            }
                        }
                        #endregion
                    }
                    #endregion
 
                    if (bEdited && iFirstHalf <= 2) //Has No Morning Undertime
                    {
                        #region Basic Four + Extension Logic

                        #region Initialization
                        int[] tmpBasicFour = new int[] {
                            iActualTimeIn1Min
                            , iActualTimeOut1Min
                            , iActualTimeIn2Min
                            , iActualTimeOut2Min
                            , iActualTimeIn3Min
                            , iActualTimeOut3Min
                            , iActualTimeIn4Min
                            , iActualTimeOut4Min
                            , iActualTimeIn5Min
                            , iActualTimeOut5Min
                            , iActualTimeIn6Min
                            , iActualTimeOut6Min
                            , iActualTimeIn7Min
                            , iActualTimeOut7Min
                            , iActualTimeIn8Min
                            , iActualTimeOut8Min
                            , iActualTimeIn9Min
                            , iActualTimeOut9Min
                            , iActualTimeIn10Min
                            , iActualTimeOut10Min
                            , iActualTimeIn11Min
                            , iActualTimeOut11Min
                            , iActualTimeIn12Min
                            , iActualTimeOut12Min
                        };
                        Array.Sort(tmpBasicFour);

                        iActualTimeIn1Min   = 0;
                        iActualTimeOut1Min  = 0;
                        iActualTimeIn2Min   = 0;
                        iActualTimeOut2Min  = 0;
                        iActualTimeIn3Min   = 0;
                        iActualTimeOut3Min  = 0;
                        iActualTimeIn4Min   = 0;
                        iActualTimeOut4Min  = 0;
                        iActualTimeIn5Min   = 0;
                        iActualTimeOut5Min  = 0;
                        iActualTimeIn6Min   = 0;
                        iActualTimeOut6Min  = 0;
                        iActualTimeIn7Min   = 0;
                        iActualTimeOut7Min  = 0;
                        iActualTimeIn8Min   = 0;
                        iActualTimeOut8Min  = 0;
                        iActualTimeIn9Min   = 0;
                        iActualTimeOut9Min  = 0;
                        iActualTimeIn10Min  = 0;
                        iActualTimeOut10Min = 0;
                        iActualTimeIn11Min  = 0;
                        iActualTimeOut11Min = 0;
                        iActualTimeIn12Min  = 0;
                        iActualTimeOut12Min = 0;

                        #endregion

                        #region Process
                        for (int x = 0; x < tmpBasicFour.Length; x++)
                        {
                            if (tmpBasicFour[x] != 0)
                            { 
                                if (tmpBasicFour[x] != 0
                                    && (tmpBasicFour[x] >= (iShiftTimeIn1Min - Globals.EXTENDIN1) 
                                    && tmpBasicFour[x] < iShiftTimeOut1Min)
                                    )
                                {
                                    #region EARLY IN OR LESS THAN OUT1
                                    if (iActualTimeIn1Min == 0 && (tmpBasicFour[x] < iShiftTimeOut1Min))
                                    {
                                        iActualTimeIn1Min = tmpBasicFour[x];
                                    }
                                    else if (iActualTimeOut1Min == 0 && (tmpBasicFour[x] - iActualTimeIn1Min) >= Globals.POCKETTIME)
                                    {
                                        iActualTimeOut1Min = tmpBasicFour[x];
                                    }
                                    #endregion
                                }
                                else if (tmpBasicFour[x] != 0 &&
                                    (tmpBasicFour[x] >= iShiftTimeOut1Min && tmpBasicFour[x] < iShiftTimeIn2Min))
                                {
                                    #region GREATER THAN OR EQUAL SHIFT OUT1 AND LESS THAN SHIFT IN2 [LUNCHBREAK]
                                    if (iActualTimeOut1Min == 0 && (tmpBasicFour[x] - iActualTimeIn1Min) >= Globals.POCKETTIME)
                                    {
                                        iActualTimeOut1Min = tmpBasicFour[x];
                                    }
                                    else if (iActualTimeIn2Min == 0 && (tmpBasicFour[x] - iActualTimeOut1Min) >= Globals.POCKETGAP)
                                    {
                                        iActualTimeIn2Min = tmpBasicFour[x];
                                    }
                                    #endregion
                                }
                                else if (tmpBasicFour[x] != 0
                                    && (tmpBasicFour[x] >= iShiftTimeIn2Min 
                                    && tmpBasicFour[x] < iShiftTimeOut2Min)
                                    && (tmpBasicFour[x] - iActualTimeOut1Min) >= Globals.POCKETGAP
                                    && iActualTimeIn2Min == 0)
                                {
                                    #region LATE IN2
                                    iActualTimeIn2Min = tmpBasicFour[x];
                                    #endregion
                                }
                                else if (tmpBasicFour[x] != 0
                                    && tmpBasicFour[x] > iShiftTimeIn2Min)
                                {
                                    #region GREATER THAN SHIFT IN2
                                    if (iActualTimeOut2Min == 0 && (tmpBasicFour[x] - iActualTimeIn2Min) >= Globals.POCKETTIME)
                                    {
                                        iActualTimeOut2Min = tmpBasicFour[x];
                                    }
                                    else if (iActualTimeOut2Min != 0)
                                    {
                                        if (iActualTimeIn3Min == 0 && (tmpBasicFour[x] - iActualTimeOut2Min) >= Globals.POCKETGAP)
                                            iActualTimeIn3Min = tmpBasicFour[x];
                                        else if (iActualTimeOut3Min == 0 && (tmpBasicFour[x] - iActualTimeIn3Min) >= Globals.POCKETTIME)
                                            iActualTimeOut3Min = tmpBasicFour[x];
                                        else if (iActualTimeIn4Min == 0 && (tmpBasicFour[x] - iActualTimeOut3Min) >= Globals.POCKETGAP)
                                            iActualTimeIn4Min = tmpBasicFour[x];
                                        else if (iActualTimeOut4Min == 0 && (tmpBasicFour[x] - iActualTimeIn4Min) >= Globals.POCKETTIME)
                                            iActualTimeOut4Min = tmpBasicFour[x];
                                        else if (iActualTimeIn5Min == 0 && (tmpBasicFour[x] - iActualTimeOut4Min) >= Globals.POCKETGAP)
                                            iActualTimeIn5Min = tmpBasicFour[x];
                                        else if (iActualTimeOut5Min == 0 && (tmpBasicFour[x] - iActualTimeIn5Min) >= Globals.POCKETTIME)
                                            iActualTimeOut5Min = tmpBasicFour[x];
                                        else if (iActualTimeIn6Min == 0 && (tmpBasicFour[x] - iActualTimeOut5Min) >= Globals.POCKETGAP)
                                            iActualTimeIn6Min = tmpBasicFour[x];
                                        else if (iActualTimeOut6Min == 0 && (tmpBasicFour[x] - iActualTimeIn6Min) >= Globals.POCKETTIME)
                                            iActualTimeOut6Min = tmpBasicFour[x];
                                        else if (iActualTimeIn7Min == 0 && (tmpBasicFour[x] - iActualTimeOut6Min) >= Globals.POCKETGAP)
                                            iActualTimeIn7Min = tmpBasicFour[x];
                                        else if (iActualTimeOut7Min == 0 && (tmpBasicFour[x] - iActualTimeIn7Min) >= Globals.POCKETTIME)
                                            iActualTimeOut7Min = tmpBasicFour[x];
                                        else if (iActualTimeIn8Min == 0 && (tmpBasicFour[x] - iActualTimeOut7Min) >= Globals.POCKETGAP)
                                            iActualTimeIn8Min = tmpBasicFour[x];
                                        else if (iActualTimeOut8Min == 0 && (tmpBasicFour[x] - iActualTimeIn8Min) >= Globals.POCKETTIME)
                                            iActualTimeOut8Min = tmpBasicFour[x];
                                        else if (iActualTimeIn9Min == 0 && (tmpBasicFour[x] - iActualTimeOut8Min) >= Globals.POCKETGAP)
                                            iActualTimeIn9Min = tmpBasicFour[x];
                                        else if (iActualTimeOut9Min == 0 && (tmpBasicFour[x] - iActualTimeIn9Min) >= Globals.POCKETTIME)
                                            iActualTimeOut9Min = tmpBasicFour[x];
                                        else if (iActualTimeIn10Min == 0 && (tmpBasicFour[x] - iActualTimeOut9Min) >= Globals.POCKETGAP)
                                            iActualTimeIn10Min = tmpBasicFour[x];
                                        else if (iActualTimeOut10Min == 0 && (tmpBasicFour[x] - iActualTimeIn10Min) >= Globals.POCKETTIME)
                                            iActualTimeOut10Min = tmpBasicFour[x];
                                        else if (iActualTimeIn11Min == 0 && (tmpBasicFour[x] - iActualTimeOut10Min) >= Globals.POCKETGAP)
                                            iActualTimeIn11Min = tmpBasicFour[x];
                                        else if (iActualTimeOut11Min == 0 && (tmpBasicFour[x] - iActualTimeIn11Min) >= Globals.POCKETTIME)
                                            iActualTimeOut11Min = tmpBasicFour[x];
                                        else if (iActualTimeIn12Min == 0 && (tmpBasicFour[x] - iActualTimeOut11Min) >= Globals.POCKETGAP)
                                            iActualTimeIn12Min = tmpBasicFour[x];
                                        else if (iActualTimeOut12Min == 0 && (tmpBasicFour[x] - iActualTimeIn12Min) >= Globals.POCKETTIME)
                                            iActualTimeOut12Min = tmpBasicFour[x];
                                    }
                                    #endregion
                                }
                            }
                        }
                        #endregion

                        #region Secondary Pass
                        //if (iActualTimeOut1Min > 0 && iActualTimeOut1Min < iShiftTimeOut1Min
                        //&& iActualTimeIn1Min == 0 && iActualTimeOut1Min < iLogTime)
                        //{//1.IF OUT1 != 0 AND IN1 == 0 AND OUT1 HAS A NEW ENTRY : COMPARE IF OUT1 LESS THAN NEW ENTRY 
                        // //THEN MOVE OUT1 INTO IN1 AND UPDATE OUT1 WITH THE NEW ENTRY
                        //    iActualTimeIn1Min = iActualTimeOut1Min;
                        //    iActualTimeOut1Min = iLogTimeOrig;
                        //    drArrDTR[x]["PostFlag"] = 1;
                        //    drArrDTR[x]["Edited"] = 1;
                        //    bEdited = true;
                        //}

                        //if (iActualTimeOut2Min > 0 && iActualTimeIn1Min == 0
                        //    && iActualTimeOut1Min > 0
                        //    && iActualTimeOut1Min < iShiftTimeOut1Min)
                        //{//2.IF OUT2 != 0 AND IN1 == 0 AND OUT1 != 0 THEN COPY OUT1 INTO IN1 AND DO NOT ERASE OUT1
                        //    iActualTimeIn1Min = iActualTimeOut1Min;
                        //}

                        if (iActualTimeIn1Min == 0 && iActualTimeOut1Min > 0
                            && iActualTimeIn2Min == 0 && iActualTimeOut2Min > 0)
                        { //3.HALF DAY: MOVE OUT1 INTO IN2
                            iActualTimeIn2Min = iActualTimeOut1Min;
                            iActualTimeOut1Min = 0;
                        }

                        //if (dtProcessDate < DateTime.Now
                        //    && iActualTimeIn2Min != 0 && iActualTimeOut2Min == 0
                        //    && x == (drArrDTR.Length - 1))
                        //{//Previous day checking to correct unposted undertime
                        //    DataRow[] tempdrArrDTR = dtDTR.Select(string.Format("EmployeeID = '{0}' AND LogDate = '{1}'", strEmployeeID, dtProcessDate));
                        //    for (int y = 0; y < tempdrArrDTR.Length; y++)
                        //    {
                        //        DateTime tempDTRDate = Convert.ToDateTime(tempdrArrDTR[y]["LogDate"].ToString());
                        //        int tempLogTime = GetMinsFromHourStr(tempdrArrDTR[y]["LogTime"].ToString());

                        //        if (tempLogTime != 0 && tempLogTime != iActualTimeIn2Min
                        //            && tempLogTime > iShiftTimeIn2Min
                        //            && dtProcessDate == tempDTRDate)
                        //        {
                        //            iActualTimeOut2Min = tempLogTime;
                        //            drArrDTR[x]["PostFlag"] = 1;
                        //            drArrDTR[x]["Edited"] = 1;
                        //            bEdited = true;
                        //        }
                        //    }
                        //}
                        #endregion

                        #endregion
                    }
                    else if (bEdited && iFirstHalf > 2) //With Morning Undertime
                    {
                        #region Trim Break Start and Break End Min and Max Logs

                        #region Initialization
                        int[] tmpMultiple = new int[] {
                            iActualTimeIn1Min
                            , iActualTimeOut1Min
                            , iActualTimeIn2Min
                            , iActualTimeOut2Min
                            , iActualTimeIn3Min
                            , iActualTimeOut3Min
                            , iActualTimeIn4Min
                            , iActualTimeOut4Min
                            , iActualTimeIn5Min
                            , iActualTimeOut5Min
                            , iActualTimeIn6Min
                            , iActualTimeOut6Min
                            , iActualTimeIn7Min
                            , iActualTimeOut7Min
                            , iActualTimeIn8Min
                            , iActualTimeOut8Min
                            , iActualTimeIn9Min
                            , iActualTimeOut9Min
                            , iActualTimeIn10Min
                            , iActualTimeOut10Min
                            , iActualTimeIn11Min
                            , iActualTimeOut11Min
                            , iActualTimeIn12Min
                            , iActualTimeOut12Min
                        };
                        Array.Sort(tmpMultiple);

                        #endregion

                        #region Remove extra logs from break start to break end
                        int tmpStartBreak = 0;
                        bool bTrimmed = false;

                        for (int x = 0; x < tmpMultiple.Length; x++)
                        {
                            if (tmpMultiple[x] != 0 && (tmpMultiple[x] >= iShiftTimeOut1Min && tmpMultiple[x] <= iShiftTimeIn2Min))
                            {
                                if (tmpStartBreak == 0)
                                {
                                    tmpStartBreak = tmpMultiple[x];
                                }  
                                else
                                {
                                    if ((x+1) < tmpMultiple.Length && tmpMultiple[x+1] <= iShiftTimeIn2Min)
                                    {
                                        tmpMultiple[x] = 0;
                                        bTrimmed = true;
                                    }
                                }
                            }
                        }
                        #endregion

                        #region Re-assign values
                        if (bTrimmed)
                        {
                            iActualTimeIn1Min   = 0;
                            iActualTimeOut1Min  = 0;
                            iActualTimeIn2Min   = 0;
                            iActualTimeOut2Min  = 0;
                            iActualTimeIn3Min   = 0;
                            iActualTimeOut3Min  = 0;
                            iActualTimeIn4Min   = 0;
                            iActualTimeOut4Min  = 0;
                            iActualTimeIn5Min   = 0;
                            iActualTimeOut5Min  = 0;
                            iActualTimeIn6Min   = 0;
                            iActualTimeOut6Min  = 0;
                            iActualTimeIn7Min   = 0;
                            iActualTimeOut7Min  = 0;
                            iActualTimeIn8Min   = 0;
                            iActualTimeOut8Min  = 0;
                            iActualTimeIn9Min   = 0;
                            iActualTimeOut9Min  = 0;
                            iActualTimeIn10Min  = 0;
                            iActualTimeOut10Min = 0;
                            iActualTimeIn11Min  = 0;
                            iActualTimeOut11Min = 0;
                            iActualTimeIn12Min  = 0;
                            iActualTimeOut12Min = 0;

                            for (int x = 0; x < tmpMultiple.Length; x++)
                            {
                                if (tmpMultiple[x] != 0)
                                {
                                    if (iActualTimeIn1Min == 0)
                                        iActualTimeIn1Min = tmpMultiple[x];
                                    else if (iActualTimeOut1Min == 0)
                                        iActualTimeOut1Min = tmpMultiple[x];
                                    else if (iActualTimeIn2Min == 0)
                                        iActualTimeIn2Min = tmpMultiple[x];
                                    else if (iActualTimeOut2Min == 0)
                                        iActualTimeOut2Min = tmpMultiple[x];
                                    else if (iActualTimeIn3Min == 0)
                                        iActualTimeIn3Min = tmpMultiple[x];
                                    else if (iActualTimeOut3Min == 0)
                                        iActualTimeOut3Min = tmpMultiple[x];
                                    else if (iActualTimeIn4Min == 0)
                                        iActualTimeIn4Min = tmpMultiple[x];
                                    else if (iActualTimeOut4Min == 0)
                                        iActualTimeOut4Min = tmpMultiple[x];
                                    else if (iActualTimeIn5Min == 0)
                                        iActualTimeIn5Min = tmpMultiple[x];
                                    else if (iActualTimeOut5Min == 0)
                                        iActualTimeOut5Min = tmpMultiple[x];
                                    else if (iActualTimeIn6Min == 0)
                                        iActualTimeIn6Min = tmpMultiple[x];
                                    else if (iActualTimeOut6Min == 0)
                                        iActualTimeOut6Min = tmpMultiple[x];
                                    else if (iActualTimeIn7Min == 0)
                                        iActualTimeIn7Min = tmpMultiple[x];
                                    else if (iActualTimeOut7Min == 0)
                                        iActualTimeOut7Min = tmpMultiple[x];
                                    else if (iActualTimeIn8Min == 0)
                                        iActualTimeIn8Min = tmpMultiple[x];
                                    else if (iActualTimeOut8Min == 0)
                                        iActualTimeOut8Min = tmpMultiple[x];
                                    else if (iActualTimeIn9Min == 0)
                                        iActualTimeIn9Min = tmpMultiple[x];
                                    else if (iActualTimeOut9Min == 0)
                                        iActualTimeOut9Min = tmpMultiple[x];
                                    else if (iActualTimeIn10Min == 0)
                                        iActualTimeIn10Min = tmpMultiple[x];
                                    else if (iActualTimeOut10Min == 0)
                                        iActualTimeOut10Min = tmpMultiple[x];
                                    else if (iActualTimeIn11Min == 0)
                                        iActualTimeIn11Min = tmpMultiple[x];
                                    else if (iActualTimeOut11Min == 0)
                                        iActualTimeOut11Min = tmpMultiple[x];
                                    else if (iActualTimeIn12Min == 0)
                                        iActualTimeIn12Min = tmpMultiple[x];
                                    else if (iActualTimeOut12Min == 0)
                                        iActualTimeOut12Min = tmpMultiple[x];
                                }
                            }
                        }
                        
                        #endregion

                        #endregion
                    }
                }

                #region Save Values
                if (bEdited)
                {
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActIn_01"]     = GetHourStrFromMins(iActualTimeIn1Min);
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActOut_01"]    = GetHourStrFromMins(iActualTimeOut1Min);
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActIn_02"]     = GetHourStrFromMins(iActualTimeIn2Min);
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActOut_02"]    = GetHourStrFromMins(iActualTimeOut2Min);
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActIn_03"]     = GetHourStrFromMins(iActualTimeIn3Min);
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActOut_03"]    = GetHourStrFromMins(iActualTimeOut3Min);
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActIn_04"]     = GetHourStrFromMins(iActualTimeIn4Min);
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActOut_04"]    = GetHourStrFromMins(iActualTimeOut4Min);
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActIn_05"]     = GetHourStrFromMins(iActualTimeIn5Min);
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActOut_05"]    = GetHourStrFromMins(iActualTimeOut5Min);
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActIn_06"]     = GetHourStrFromMins(iActualTimeIn6Min);
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActOut_06"]    = GetHourStrFromMins(iActualTimeOut6Min);
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActIn_07"]     = GetHourStrFromMins(iActualTimeIn7Min);
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActOut_07"]    = GetHourStrFromMins(iActualTimeOut7Min);
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActIn_08"]     = GetHourStrFromMins(iActualTimeIn8Min);
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActOut_08"]    = GetHourStrFromMins(iActualTimeOut8Min);
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActIn_09"]     = GetHourStrFromMins(iActualTimeIn9Min);
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActOut_09"]    = GetHourStrFromMins(iActualTimeOut9Min);
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActIn_10"]     = GetHourStrFromMins(iActualTimeIn10Min);
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActOut_10"]    = GetHourStrFromMins(iActualTimeOut10Min);
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActIn_11"]     = GetHourStrFromMins(iActualTimeIn11Min);
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActOut_11"]    = GetHourStrFromMins(iActualTimeOut11Min);
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActIn_12"]     = GetHourStrFromMins(iActualTimeIn12Min);
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActOut_12"]    = GetHourStrFromMins(iActualTimeOut12Min);
                    dtEmployeeLogLedger.Rows[i]["Edited"]           = 1;
                }
                #endregion
            }

            #region Save Log Ledger Records
            Console.Write("\n{0} Saving log ledger...", dtProcessDate.ToShortDateString());
            drArrLogLedger = dtEmployeeLogLedger.Select("Edited = 1");
            string strUpdateQuery = "";
            int iUpdateCtr = 0;

            string strLogLedgerMiscTable = "T_EmpTimeRegisterMisc";
            if (bIsInHistTable == true)
                strLogLedgerMiscTable = "T_EmpTimeRegisterMiscHst";

            string strTrailPrevious = "";
            string strUpdateTemplate = @"UPDATE {0}..{1} SET Ttm_ActIn_01 = '{4}', Ttm_ActOut_01 = '{5}', Ttm_ActIn_02 = '{6}', Ttm_ActOut_02 = '{7}', Ttm_ActIn_03 = '{8}', Ttm_ActOut_03 = '{9}', Ttm_ActIn_04 = '{10}', Ttm_ActOut_04 = '{11}', Ttm_ActIn_05 = '{12}', Ttm_ActOut_05 = '{13}', Ttm_ActIn_06 = '{14}', Ttm_ActOut_06 = '{15}', Ttm_ActIn_07 = '{16}', Ttm_ActOut_07 = '{17}', Ttm_ActIn_08 = '{18}', Ttm_ActOut_08 = '{19}', Ttm_ActIn_09 = '{20}', Ttm_ActOut_09 = '{21}', Ttm_ActIn_10 = '{22}', Ttm_ActOut_10 = '{23}', Ttm_ActIn_11 = '{24}', Ttm_ActOut_11 = '{25}', Ttm_ActIn_12 = '{26}', Ttm_ActOut_12 = '{27}', Usr_Login = 'LOGUPLOADING', Ludatetime = GETDATE() WHERE Ttm_IDNo = '{2}' AND Ttm_Date = '{3}' ";
            for (int x = 0; x < drArrLogLedger.Length; x++)
            {
                if (bIsInHistTable == true)
                {
                    #region Create Trail Records
                    #region Query
                    strTrailPrevious = string.Format(@" 
DECLARE @AffectedPayPeriod VARCHAR(7) = (Select TOP(1) Tps_PayCycle 
                                        From {0}..T_PaySchedule 
                                        Where Tps_CycleIndicator = 'P'
                                            And '{2}' BETWEEN Tps_StartCycle AND Tps_EndCycle
                                        ORDER BY Tps_StartCycle DESC)
DECLARE @AdjustPayPeriod VARCHAR(7) = (Select Tps_PayCycle 
									   From {0}..T_PaySchedule 
									   Where Tps_CycleIndicator='C' 
										and Tps_RecordStatus = 'A')

--INSERT INTO LOG LEDGER TRAIL
INSERT INTO {0}..T_EmpTimeRegisterTrl
(
	    Ttr_IDNo
        ,Ttr_Date
        ,Ttr_AdjPayCycle
        ,Ttr_PayCycle
        ,Ttr_DayCode
        ,Ttr_ShiftCode
        ,Ttr_HolidayFlag
        ,Ttr_RestDayFlag
        ,Ttr_ActIn_1
        ,Ttr_ActOut_1
        ,Ttr_ActIn_2
        ,Ttr_ActOut_2
        ,Ttr_WFPayLVCode
        ,Ttr_WFPayLVHr
        ,Ttr_PayLVMin
        ,Ttr_ExcLVMin
        ,Ttr_WFNoPayLVCode
        ,Ttr_WFNoPayLVHr
        ,Ttr_NoPayLVMin
        ,Ttr_WFOTAdvHr
        ,Ttr_WFOTPostHr
        ,Ttr_OTMin
        ,Ttr_CompOTMin
        ,Ttr_OffsetOTMin
        ,Ttr_WFTimeMod
        ,Ttr_WFFlexTime
        ,Ttr_Amnesty
        ,Ttr_SkipServiceFlag
        ,Ttr_SkipServiceBy
        ,Ttr_SkipServiceDate
        ,Ttr_AssumedFlag
        ,Ttr_AssumedBy
        ,Ttr_AssumedDate
        ,Ttr_AssumedPost
        ,Ttr_ConvIn_1Min
        ,Ttr_ConvOut_1Min
        ,Ttr_ConvIn_2Min
        ,Ttr_ConvOut_2Min
        ,Ttr_CompIn_1Min
        ,Ttr_CompOut_1Min
        ,Ttr_CompIn_2Min
        ,Ttr_CompOut_2Min
        ,Ttr_CompAdvOTMin
        ,Ttr_ShiftIn_1Min
        ,Ttr_ShiftOut_1Min
        ,Ttr_ShiftIn_2Min
        ,Ttr_ShiftOut_2Min
        ,Ttr_ShiftMin
        ,Ttr_ScheduleType
        ,Ttr_ActLT1Min
        ,Ttr_ActLT2Min
        ,Ttr_CompLT1Min
        ,Ttr_CompLT2Min
        ,Ttr_ActUT1Min
        ,Ttr_ActUT2Min
        ,Ttr_CompUT1Min
        ,Ttr_CompUT2Min
        ,Ttr_InitialABSMin
        ,Ttr_CompABSMin
        ,Ttr_CompREGMin
        ,Ttr_CompWorkMin
        ,Ttr_CompNDMin
        ,Ttr_CompNDOTMin
        ,Ttr_PrvDayWorkMin
        ,Ttr_PrvDayHolRef
        ,Ttr_PDHOLHour
        ,Ttr_PDRESTLEGHOLDay
        ,Ttr_WorkDay
        ,Ttr_EXPHour
        ,Ttr_ABSHour
        ,Ttr_REGHour
        ,Ttr_OTHour
        ,Ttr_NDHour
        ,Ttr_NDOTHour
        ,Ttr_LVHour
        ,Ttr_PaidBreakHour
        ,Ttr_OBHour
        ,Ttr_RegPlusHour
        ,Ttr_TBAmt01
        ,Ttr_TBAmt02
        ,Ttr_TBAmt03
        ,Ttr_TBAmt04
        ,Ttr_TBAmt05
        ,Ttr_TBAmt06
        ,Ttr_TBAmt07
        ,Ttr_TBAmt08
        ,Ttr_TBAmt09
        ,Ttr_TBAmt10
        ,Ttr_TBAmt11
        ,Ttr_TBAmt12
        ,Ttr_WorkLocationCode
        ,Ttr_CalendarGroup
        ,Ttr_PremiumGrpCode
        ,Ttr_PayrollGroup
        ,Ttr_CostcenterCode
        ,Ttr_EmploymentStatusCode
        ,Ttr_PayrollType
        ,Ttr_Grade
        ,Usr_Login
        ,Ludatetime
)
SELECT A.Ttr_IDNo
	,A.Ttr_Date
    ,@AdjustPayPeriod
    ,A.Ttr_PayCycle
    ,A.Ttr_DayCode
    ,A.Ttr_ShiftCode
    ,A.Ttr_HolidayFlag
    ,A.Ttr_RestDayFlag
    ,A.Ttr_ActIn_1
    ,A.Ttr_ActOut_1
    ,A.Ttr_ActIn_2
    ,A.Ttr_ActOut_2
    ,A.Ttr_WFPayLVCode
    ,A.Ttr_WFPayLVHr
    ,A.Ttr_PayLVMin
    ,A.Ttr_ExcLVMin
    ,A.Ttr_WFNoPayLVCode
    ,A.Ttr_WFNoPayLVHr
    ,A.Ttr_NoPayLVMin
    ,A.Ttr_WFOTAdvHr
    ,A.Ttr_WFOTPostHr
    ,A.Ttr_OTMin
    ,A.Ttr_CompOTMin
    ,A.Ttr_OffsetOTMin
    ,A.Ttr_WFTimeMod
    ,A.Ttr_WFFlexTime
    ,A.Ttr_Amnesty
    ,A.Ttr_SkipServiceFlag
    ,A.Ttr_SkipServiceBy
    ,A.Ttr_SkipServiceDate
    ,A.Ttr_AssumedFlag
    ,A.Ttr_AssumedBy
    ,A.Ttr_AssumedDate
    ,A.Ttr_AssumedPost
    ,A.Ttr_ConvIn_1Min
    ,A.Ttr_ConvOut_1Min
    ,A.Ttr_ConvIn_2Min
    ,A.Ttr_ConvOut_2Min
    ,A.Ttr_CompIn_1Min
    ,A.Ttr_CompOut_1Min
    ,A.Ttr_CompIn_2Min
    ,A.Ttr_CompOut_2Min
    ,A.Ttr_CompAdvOTMin
    ,A.Ttr_ShiftIn_1Min
    ,A.Ttr_ShiftOut_1Min
    ,A.Ttr_ShiftIn_2Min
    ,A.Ttr_ShiftOut_2Min
    ,A.Ttr_ShiftMin
    ,A.Ttr_ScheduleType
    ,A.Ttr_ActLT1Min
    ,A.Ttr_ActLT2Min
    ,A.Ttr_CompLT1Min
    ,A.Ttr_CompLT2Min
    ,A.Ttr_ActUT1Min
    ,A.Ttr_ActUT2Min
    ,A.Ttr_CompUT1Min
    ,A.Ttr_CompUT2Min
    ,A.Ttr_InitialABSMin
    ,A.Ttr_CompABSMin
    ,A.Ttr_CompREGMin
    ,A.Ttr_CompWorkMin
    ,A.Ttr_CompNDMin
    ,A.Ttr_CompNDOTMin
    ,A.Ttr_PrvDayWorkMin
    ,A.Ttr_PrvDayHolRef
    ,A.Ttr_PDHOLHour
    ,A.Ttr_PDRESTLEGHOLDay
    ,A.Ttr_WorkDay
    ,A.Ttr_EXPHour
    ,A.Ttr_ABSHour
    ,A.Ttr_REGHour
    ,A.Ttr_OTHour
    ,A.Ttr_NDHour
    ,A.Ttr_NDOTHour
    ,A.Ttr_LVHour
    ,A.Ttr_PaidBreakHour
    ,A.Ttr_OBHour
    ,A.Ttr_RegPlusHour
    ,A.Ttr_TBAmt01
    ,A.Ttr_TBAmt02
    ,A.Ttr_TBAmt03
    ,A.Ttr_TBAmt04
    ,A.Ttr_TBAmt05
    ,A.Ttr_TBAmt06
    ,A.Ttr_TBAmt07
    ,A.Ttr_TBAmt08
    ,A.Ttr_TBAmt09
    ,A.Ttr_TBAmt10
    ,A.Ttr_TBAmt11
    ,A.Ttr_TBAmt12
    ,A.Ttr_WorkLocationCode
    ,A.Ttr_CalendarGroup
    ,A.Ttr_PremiumGrpCode
    ,A.Ttr_PayrollGroup
    ,A.Ttr_CostcenterCode
    ,A.Ttr_EmploymentStatusCode
    ,A.Ttr_PayrollType
    ,A.Ttr_Grade
    ,A.Usr_Login
    ,A.Ludatetime
FROM {0}..T_EmpTimeRegisterHst A
LEFT JOIN {0}..T_EmpTimeRegisterTrl B
ON A.Ttr_IDNo = B.Ttr_IDNo
	AND A.Ttr_Date = B.Ttr_Date
    AND A.Ttr_PayCycle = B.Ttr_PayCycle
	AND B.Ttr_AdjPayCycle = @AdjustPayPeriod
WHERE B.Ttr_IDNo IS NULL
    AND A.Ttr_PayCycle = @AffectedPayPeriod
    AND A.Ttr_IDNo = '{1}'

--INSERT INTO LOG LEDGER EXT TRAIL
INSERT INTO {0}..T_EmpTimeRegisterMiscTrl
(       Ttm_IDNo
        ,Ttm_Date
        ,Ttm_AdjPayCycle
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
        ,Ttm_Result
        ,Ttm_LT1
        ,Ttm_LT2
        ,Ttm_UT1
        ,Ttm_UT2
        ,Usr_Login
        ,Ludatetime
)
SELECT A.Ttm_IDNo
        ,A.Ttm_Date
        ,@AdjustPayPeriod
        ,A.Ttm_PayCycle
        ,A.Ttm_ActIn_01
        ,A.Ttm_ActOut_01
        ,A.Ttm_ActIn_02
        ,A.Ttm_ActOut_02
        ,A.Ttm_ActIn_03
        ,A.Ttm_ActOut_03
        ,A.Ttm_ActIn_04
        ,A.Ttm_ActOut_04
        ,A.Ttm_ActIn_05
        ,A.Ttm_ActOut_05
        ,A.Ttm_ActIn_06
        ,A.Ttm_ActOut_06
        ,A.Ttm_ActIn_07
        ,A.Ttm_ActOut_07
        ,A.Ttm_ActIn_08
        ,A.Ttm_ActOut_08
        ,A.Ttm_ActIn_09
        ,A.Ttm_ActOut_09
        ,A.Ttm_ActIn_10
        ,A.Ttm_ActOut_10
        ,A.Ttm_ActIn_11
        ,A.Ttm_ActOut_11
        ,A.Ttm_ActIn_12
        ,A.Ttm_ActOut_12
        ,A.Ttm_Result
        ,A.Ttm_LT1
        ,A.Ttm_LT2
        ,A.Ttm_UT1
        ,A.Ttm_UT2
        ,A.Usr_Login
        ,A.Ludatetime
FROM {0}..T_EmpTimeRegisterMiscHst A
LEFT JOIN {0}..T_EmpTimeRegisterMiscTrl B
ON A.Ttm_IDNo = B.Ttm_IDNo
	AND A.Ttm_Date = B.Ttm_Date
    AND A.Ttm_PayCycle = B.Ttm_PayCycle
	AND B.Ttm_AdjPayCycle = @AdjustPayPeriod
WHERE B.Ttm_IDNo IS NULL
    AND A.Ttm_PayCycle = @AffectedPayPeriod
    AND A.Ttm_IDNo = '{1}'

--INSERT INTO PAYROLL TRANSACTION TRAIL
INSERT INTO {0}..T_EmpPayTranHdrTrl
(
	  Tph_IDNo
        ,Tph_AdjPayCycle
        ,Tph_PayCycle
        ,Tph_LTHr
        ,Tph_UTHr
        ,Tph_UPLVHr
        ,Tph_ABSLEGHOLHr
        ,Tph_ABSSPLHOLHr
        ,Tph_ABSCOMPHOLHr
        ,Tph_ABSPSDHr
        ,Tph_ABSOTHHOLHr
        ,Tph_WDABSHr
        ,Tph_LTUTMaxHr
        ,Tph_ABSHr
        ,Tph_REGHr
        ,Tph_PDLVHr
        ,Tph_PDLEGHOLHr
        ,Tph_PDSPLHOLHr
        ,Tph_PDCOMPHOLHr
        ,Tph_PDPSDHr
        ,Tph_PDOTHHOLHr
        ,Tph_PDRESTLEGHOLHr
        ,Tph_REGOTHr
        ,Tph_REGNDHr
        ,Tph_REGNDOTHr
        ,Tph_RESTHr
        ,Tph_RESTOTHr
        ,Tph_RESTNDHr
        ,Tph_RESTNDOTHr
        ,Tph_LEGHOLHr
        ,Tph_LEGHOLOTHr
        ,Tph_LEGHOLNDHr
        ,Tph_LEGHOLNDOTHr
        ,Tph_SPLHOLHr
        ,Tph_SPLHOLOTHr
        ,Tph_SPLHOLNDHr
        ,Tph_SPLHOLNDOTHr
        ,Tph_PSDHr
        ,Tph_PSDOTHr
        ,Tph_PSDNDHr
        ,Tph_PSDNDOTHr
        ,Tph_COMPHOLHr
        ,Tph_COMPHOLOTHr
        ,Tph_COMPHOLNDHr
        ,Tph_COMPHOLNDOTHr
        ,Tph_RESTLEGHOLHr
        ,Tph_RESTLEGHOLOTHr
        ,Tph_RESTLEGHOLNDHr
        ,Tph_RESTLEGHOLNDOTHr
        ,Tph_RESTSPLHOLHr
        ,Tph_RESTSPLHOLOTHr
        ,Tph_RESTSPLHOLNDHr
        ,Tph_RESTSPLHOLNDOTHr
        ,Tph_RESTCOMPHOLHr
        ,Tph_RESTCOMPHOLOTHr
        ,Tph_RESTCOMPHOLNDHr
        ,Tph_RESTCOMPHOLNDOTHr
        ,Tph_RESTPSDHr
        ,Tph_RESTPSDOTHr
        ,Tph_RESTPSDNDHr
        ,Tph_RESTPSDNDOTHr
        ,Tph_SRGAdjHr
        ,Tph_SRGAdjAmt
        ,Tph_SOTAdjHr
        ,Tph_SOTAdjAmt
        ,Tph_SHOLAdjHr
        ,Tph_SHOLAdjAmt
        ,Tph_SNDAdjHr
        ,Tph_SNDAdjAmt
        ,Tph_SLVAdjHr
        ,Tph_SLVAdjAmt
        ,Tph_MRGAdjHr
        ,Tph_MRGAdjAmt
        ,Tph_MOTAdjHr
        ,Tph_MOTAdjAmt
        ,Tph_MHOLAdjHr
        ,Tph_MHOLAdjAmt
        ,Tph_MNDAdjHr
        ,Tph_MNDAdjAmt
        ,Tph_TotalAdjAmt
        ,Tph_TaxableIncomeAmt
        ,Tph_NontaxableIncomeAmt
        ,Tph_WorkDay
        ,Tph_PayrollType
        ,Tph_RetainUserEntry
        ,Usr_Login
        ,Ludatetime
)
SELECT A.Tph_IDNo
        ,@AdjustPayPeriod
        ,A.Tph_PayCycle
        ,A.Tph_LTHr
        ,A.Tph_UTHr
        ,A.Tph_UPLVHr
        ,A.Tph_ABSLEGHOLHr
        ,A.Tph_ABSSPLHOLHr
        ,A.Tph_ABSCOMPHOLHr
        ,A.Tph_ABSPSDHr
        ,A.Tph_ABSOTHHOLHr
        ,A.Tph_WDABSHr
        ,A.Tph_LTUTMaxHr
        ,A.Tph_ABSHr
        ,A.Tph_REGHr
        ,A.Tph_PDLVHr
        ,A.Tph_PDLEGHOLHr
        ,A.Tph_PDSPLHOLHr
        ,A.Tph_PDCOMPHOLHr
        ,A.Tph_PDPSDHr
        ,A.Tph_PDOTHHOLHr
        ,A.Tph_PDRESTLEGHOLHr
        ,A.Tph_REGOTHr
        ,A.Tph_REGNDHr
        ,A.Tph_REGNDOTHr
        ,A.Tph_RESTHr
        ,A.Tph_RESTOTHr
        ,A.Tph_RESTNDHr
        ,A.Tph_RESTNDOTHr
        ,A.Tph_LEGHOLHr
        ,A.Tph_LEGHOLOTHr
        ,A.Tph_LEGHOLNDHr
        ,A.Tph_LEGHOLNDOTHr
        ,A.Tph_SPLHOLHr
        ,A.Tph_SPLHOLOTHr
        ,A.Tph_SPLHOLNDHr
        ,A.Tph_SPLHOLNDOTHr
        ,A.Tph_PSDHr
        ,A.Tph_PSDOTHr
        ,A.Tph_PSDNDHr
        ,A.Tph_PSDNDOTHr
        ,A.Tph_COMPHOLHr
        ,A.Tph_COMPHOLOTHr
        ,A.Tph_COMPHOLNDHr
        ,A.Tph_COMPHOLNDOTHr
        ,A.Tph_RESTLEGHOLHr
        ,A.Tph_RESTLEGHOLOTHr
        ,A.Tph_RESTLEGHOLNDHr
        ,A.Tph_RESTLEGHOLNDOTHr
        ,A.Tph_RESTSPLHOLHr
        ,A.Tph_RESTSPLHOLOTHr
        ,A.Tph_RESTSPLHOLNDHr
        ,A.Tph_RESTSPLHOLNDOTHr
        ,A.Tph_RESTCOMPHOLHr
        ,A.Tph_RESTCOMPHOLOTHr
        ,A.Tph_RESTCOMPHOLNDHr
        ,A.Tph_RESTCOMPHOLNDOTHr
        ,A.Tph_RESTPSDHr
        ,A.Tph_RESTPSDOTHr
        ,A.Tph_RESTPSDNDHr
        ,A.Tph_RESTPSDNDOTHr
        ,A.Tph_SRGAdjHr
        ,A.Tph_SRGAdjAmt
        ,A.Tph_SOTAdjHr
        ,A.Tph_SOTAdjAmt
        ,A.Tph_SHOLAdjHr
        ,A.Tph_SHOLAdjAmt
        ,A.Tph_SNDAdjHr
        ,A.Tph_SNDAdjAmt
        ,A.Tph_SLVAdjHr
        ,A.Tph_SLVAdjAmt
        ,A.Tph_MRGAdjHr
        ,A.Tph_MRGAdjAmt
        ,A.Tph_MOTAdjHr
        ,A.Tph_MOTAdjAmt
        ,A.Tph_MHOLAdjHr
        ,A.Tph_MHOLAdjAmt
        ,A.Tph_MNDAdjHr
        ,A.Tph_MNDAdjAmt
        ,A.Tph_TotalAdjAmt
        ,A.Tph_TaxableIncomeAmt
        ,A.Tph_NontaxableIncomeAmt
        ,A.Tph_WorkDay
        ,A.Tph_PayrollType
        ,A.Tph_RetainUserEntry
        ,A.Usr_Login
        ,A.Ludatetime
FROM {0}..T_EmpPayTranHdrHst A
LEFT JOIN {0}..T_EmpPayTranHdrTrl B
ON A.Tph_IDNo = B.Tph_IDNo
	AND A.Tph_PayCycle = B.Tph_PayCycle
    AND B.Tph_AdjPayCycle = @AdjustPayPeriod
WHERE B.Tph_IDNo IS NULL
    AND A.Tph_PayCycle = @AffectedPayPeriod
    AND A.Tph_IDNo = '{1}'

--INSERT INTO PAYROLL TRANSACTION TRAIL DETAIL
INSERT INTO {0}..T_EmpPayTranDtlTrl
(
	   Tpd_IDNo
        ,Tpd_AdjPayCycle
        ,Tpd_PayCycle
        ,Tpd_Date
        ,Tpd_LTHr
        ,Tpd_UTHr
        ,Tpd_UPLVHr
        ,Tpd_ABSLEGHOLHr
        ,Tpd_ABSSPLHOLHr
        ,Tpd_ABSCOMPHOLHr
        ,Tpd_ABSPSDHr
        ,Tpd_ABSOTHHOLHr
        ,Tpd_WDABSHr
        ,Tpd_LTUTMaxHr
        ,Tpd_ABSHr
        ,Tpd_REGHr
        ,Tpd_PDLVHr
        ,Tpd_PDLEGHOLHr
        ,Tpd_PDSPLHOLHr
        ,Tpd_PDCOMPHOLHr
        ,Tpd_PDPSDHr
        ,Tpd_PDOTHHOLHr
        ,Tpd_PDRESTLEGHOLHr
        ,Tpd_REGOTHr
        ,Tpd_REGNDHr
        ,Tpd_REGNDOTHr
        ,Tpd_RESTHr
        ,Tpd_RESTOTHr
        ,Tpd_RESTNDHr
        ,Tpd_RESTNDOTHr
        ,Tpd_LEGHOLHr
        ,Tpd_LEGHOLOTHr
        ,Tpd_LEGHOLNDHr
        ,Tpd_LEGHOLNDOTHr
        ,Tpd_SPLHOLHr
        ,Tpd_SPLHOLOTHr
        ,Tpd_SPLHOLNDHr
        ,Tpd_SPLHOLNDOTHr
        ,Tpd_PSDHr
        ,Tpd_PSDOTHr
        ,Tpd_PSDNDHr
        ,Tpd_PSDNDOTHr
        ,Tpd_COMPHOLHr
        ,Tpd_COMPHOLOTHr
        ,Tpd_COMPHOLNDHr
        ,Tpd_COMPHOLNDOTHr
        ,Tpd_RESTLEGHOLHr
        ,Tpd_RESTLEGHOLOTHr
        ,Tpd_RESTLEGHOLNDHr
        ,Tpd_RESTLEGHOLNDOTHr
        ,Tpd_RESTSPLHOLHr
        ,Tpd_RESTSPLHOLOTHr
        ,Tpd_RESTSPLHOLNDHr
        ,Tpd_RESTSPLHOLNDOTHr
        ,Tpd_RESTCOMPHOLHr
        ,Tpd_RESTCOMPHOLOTHr
        ,Tpd_RESTCOMPHOLNDHr
        ,Tpd_RESTCOMPHOLNDOTHr
        ,Tpd_RESTPSDHr
        ,Tpd_RESTPSDOTHr
        ,Tpd_RESTPSDNDHr
        ,Tpd_RESTPSDNDOTHr
        ,Tpd_WorkDay
        ,Tpd_PayrollType
        ,Tpd_PremiumGrpCode
        ,Usr_Login
        ,Ludatetime
)
SELECT A.Tpd_IDNo
        ,@AdjustPayPeriod
        ,A.Tpd_PayCycle
        ,A.Tpd_Date
        ,A.Tpd_LTHr
        ,A.Tpd_UTHr
        ,A.Tpd_UPLVHr
        ,A.Tpd_ABSLEGHOLHr
        ,A.Tpd_ABSSPLHOLHr
        ,A.Tpd_ABSCOMPHOLHr
        ,A.Tpd_ABSPSDHr
        ,A.Tpd_ABSOTHHOLHr
        ,A.Tpd_WDABSHr
        ,A.Tpd_LTUTMaxHr
        ,A.Tpd_ABSHr
        ,A.Tpd_REGHr
        ,A.Tpd_PDLVHr
        ,A.Tpd_PDLEGHOLHr
        ,A.Tpd_PDSPLHOLHr
        ,A.Tpd_PDCOMPHOLHr
        ,A.Tpd_PDPSDHr
        ,A.Tpd_PDOTHHOLHr
        ,A.Tpd_PDRESTLEGHOLHr
        ,A.Tpd_REGOTHr
        ,A.Tpd_REGNDHr
        ,A.Tpd_REGNDOTHr
        ,A.Tpd_RESTHr
        ,A.Tpd_RESTOTHr
        ,A.Tpd_RESTNDHr
        ,A.Tpd_RESTNDOTHr
        ,A.Tpd_LEGHOLHr
        ,A.Tpd_LEGHOLOTHr
        ,A.Tpd_LEGHOLNDHr
        ,A.Tpd_LEGHOLNDOTHr
        ,A.Tpd_SPLHOLHr
        ,A.Tpd_SPLHOLOTHr
        ,A.Tpd_SPLHOLNDHr
        ,A.Tpd_SPLHOLNDOTHr
        ,A.Tpd_PSDHr
        ,A.Tpd_PSDOTHr
        ,A.Tpd_PSDNDHr
        ,A.Tpd_PSDNDOTHr
        ,A.Tpd_COMPHOLHr
        ,A.Tpd_COMPHOLOTHr
        ,A.Tpd_COMPHOLNDHr
        ,A.Tpd_COMPHOLNDOTHr
        ,A.Tpd_RESTLEGHOLHr
        ,A.Tpd_RESTLEGHOLOTHr
        ,A.Tpd_RESTLEGHOLNDHr
        ,A.Tpd_RESTLEGHOLNDOTHr
        ,A.Tpd_RESTSPLHOLHr
        ,A.Tpd_RESTSPLHOLOTHr
        ,A.Tpd_RESTSPLHOLNDHr
        ,A.Tpd_RESTSPLHOLNDOTHr
        ,A.Tpd_RESTCOMPHOLHr
        ,A.Tpd_RESTCOMPHOLOTHr
        ,A.Tpd_RESTCOMPHOLNDHr
        ,A.Tpd_RESTCOMPHOLNDOTHr
        ,A.Tpd_RESTPSDHr
        ,A.Tpd_RESTPSDOTHr
        ,A.Tpd_RESTPSDNDHr
        ,A.Tpd_RESTPSDNDOTHr
        ,A.Tpd_WorkDay
        ,A.Tpd_PayrollType
        ,A.Tpd_PremiumGrpCode
        ,A.Usr_Login
        ,A.Ludatetime
FROM {0}..T_EmpPayTranDtlHst A
LEFT JOIN {0}..T_EmpPayTranDtlTrl B
ON A.Tpd_IDNo = B.Tpd_IDNo
	AND A.Tpd_PayCycle = B.Tpd_PayCycle
	AND A.Tpd_Date = B.Tpd_Date
    AND B.Tpd_AdjPayCycle = @AdjustPayPeriod
WHERE B.Tpd_IDNo IS NULL
    AND A.Tpd_PayCycle = @AffectedPayPeriod
    AND A.Tpd_IDNo = '{1}'
	
--INSERT INTO PAYROLL TRANSACTION TRAIL EXT
INSERT INTO {0}..T_EmpPayTranHdrMiscTrl
(
	   Tph_IDNo
      ,Tph_AdjPayCycle
      ,Tph_PayCycle
      ,Tph_Misc1Hr
      ,Tph_Misc1OTHr
      ,Tph_Misc1NDHr
      ,Tph_Misc1NDOTHr
      ,Tph_Misc2Hr
      ,Tph_Misc2OTHr
      ,Tph_Misc2NDHr
      ,Tph_Misc2NDOTHr
      ,Tph_Misc3Hr
      ,Tph_Misc3OTHr
      ,Tph_Misc3NDHr
      ,Tph_Misc3NDOTHr
      ,Tph_Misc4Hr
      ,Tph_Misc4OTHr
      ,Tph_Misc4NDHr
      ,Tph_Misc4NDOTHr
      ,Tph_Misc5Hr
      ,Tph_Misc5OTHr
      ,Tph_Misc5NDHr
      ,Tph_Misc5NDOTHr
      ,Tph_Misc6Hr
      ,Tph_Misc6OTHr
      ,Tph_Misc6NDHr
      ,Tph_Misc6NDOTHr
      ,Usr_Login
      ,Ludatetime
)
SELECT A.Tph_IDNo
      ,@AdjustPayPeriod
      ,A.Tph_PayCycle
      ,A.Tph_Misc1Hr
      ,A.Tph_Misc1OTHr
      ,A.Tph_Misc1NDHr
      ,A.Tph_Misc1NDOTHr
      ,A.Tph_Misc2Hr
      ,A.Tph_Misc2OTHr
      ,A.Tph_Misc2NDHr
      ,A.Tph_Misc2NDOTHr
      ,A.Tph_Misc3Hr
      ,A.Tph_Misc3OTHr
      ,A.Tph_Misc3NDHr
      ,A.Tph_Misc3NDOTHr
      ,A.Tph_Misc4Hr
      ,A.Tph_Misc4OTHr
      ,A.Tph_Misc4NDHr
      ,A.Tph_Misc4NDOTHr
      ,A.Tph_Misc5Hr
      ,A.Tph_Misc5OTHr
      ,A.Tph_Misc5NDHr
      ,A.Tph_Misc5NDOTHr
      ,A.Tph_Misc6Hr
      ,A.Tph_Misc6OTHr
      ,A.Tph_Misc6NDHr
      ,A.Tph_Misc6NDOTHr
      ,A.Usr_Login
      ,A.Ludatetime
FROM {0}..T_EmpPayTranHdrMiscHst A
LEFT JOIN {0}..T_EmpPayTranHdrMiscTrl B
ON A.Tph_IDNo = B.Tph_IDNo
	AND A.Tph_PayCycle = B.Tph_PayCycle
    AND B.Tph_AdjPayCycle = @AdjustPayPeriod
WHERE B.Tph_IDNo IS NULL
    AND A.Tph_PayCycle = @AffectedPayPeriod
    AND A.Tph_IDNo = '{1}'
	
--INSERT INTO PAYROLL TRANSACTION TRAIL EXT DETAIL
INSERT INTO {0}..T_EmpPayTranDtlMiscTrl
(
	   Tpd_IDNo
        ,Tpd_AdjPayCycle
        ,Tpd_PayCycle
        ,Tpd_Date
        ,Tpd_Misc1Hr
        ,Tpd_Misc1OTHr
        ,Tpd_Misc1NDHr
        ,Tpd_Misc1NDOTHr
        ,Tpd_Misc2Hr
        ,Tpd_Misc2OTHr
        ,Tpd_Misc2NDHr
        ,Tpd_Misc2NDOTHr
        ,Tpd_Misc3Hr
        ,Tpd_Misc3OTHr
        ,Tpd_Misc3NDHr
        ,Tpd_Misc3NDOTHr
        ,Tpd_Misc4Hr
        ,Tpd_Misc4OTHr
        ,Tpd_Misc4NDHr
        ,Tpd_Misc4NDOTHr
        ,Tpd_Misc5Hr
        ,Tpd_Misc5OTHr
        ,Tpd_Misc5NDHr
        ,Tpd_Misc5NDOTHr
        ,Tpd_Misc6Hr
        ,Tpd_Misc6OTHr
        ,Tpd_Misc6NDHr
        ,Tpd_Misc6NDOTHr
        ,Usr_Login
        ,Ludatetime
)
SELECT A.Tpd_IDNo
        ,@AdjustPayPeriod
        ,A.Tpd_PayCycle
        ,A.Tpd_Date
        ,A.Tpd_Misc1Hr
        ,A.Tpd_Misc1OTHr
        ,A.Tpd_Misc1NDHr
        ,A.Tpd_Misc1NDOTHr
        ,A.Tpd_Misc2Hr
        ,A.Tpd_Misc2OTHr
        ,A.Tpd_Misc2NDHr
        ,A.Tpd_Misc2NDOTHr
        ,A.Tpd_Misc3Hr
        ,A.Tpd_Misc3OTHr
        ,A.Tpd_Misc3NDHr
        ,A.Tpd_Misc3NDOTHr
        ,A.Tpd_Misc4Hr
        ,A.Tpd_Misc4OTHr
        ,A.Tpd_Misc4NDHr
        ,A.Tpd_Misc4NDOTHr
        ,A.Tpd_Misc5Hr
        ,A.Tpd_Misc5OTHr
        ,A.Tpd_Misc5NDHr
        ,A.Tpd_Misc5NDOTHr
        ,A.Tpd_Misc6Hr
        ,A.Tpd_Misc6OTHr
        ,A.Tpd_Misc6NDHr
        ,A.Tpd_Misc6NDOTHr
        ,A.Usr_Login
        ,A.Ludatetime
FROM {0}..T_EmpPayTranDtlMiscHst A
LEFT JOIN {0}..T_EmpPayTranDtlMiscTrl B
ON A.Tpd_IDNo = B.Tpd_IDNo
	AND A.Tpd_PayCycle = B.Tpd_PayCycle
	AND A.Tpd_Date = B.Tpd_Date
    AND B.Tpd_AdjPayCycle = @AdjustPayPeriod
WHERE B.Tpd_IDNo IS NULL
    AND A.Tpd_PayCycle = @AffectedPayPeriod
    AND A.Tpd_IDNo = '{1}' ", strDBName, drArrLogLedger[x]["Ttm_IDNo"], dtProcessDate.ToShortDateString());
                    #endregion
                    //dal.ExecuteNonQuery(strTrailPrevious);
                    #endregion
                }

                strUpdateQuery += string.Format(strUpdateTemplate
                                                , strDBName
                                                , strLogLedgerMiscTable
                                                , drArrLogLedger[x]["Ttm_IDNo"]
                                                , drArrLogLedger[x]["Ttm_Date"]
                                                , drArrLogLedger[x]["Ttm_ActIn_01"]
                                                , drArrLogLedger[x]["Ttm_ActOut_01"]                //5
                                                , drArrLogLedger[x]["Ttm_ActIn_02"]
                                                , drArrLogLedger[x]["Ttm_ActOut_02"]
                                                , drArrLogLedger[x]["Ttm_ActIn_03"]
                                                , drArrLogLedger[x]["Ttm_ActOut_03"]
                                                , drArrLogLedger[x]["Ttm_ActIn_04"]                 //10
                                                , drArrLogLedger[x]["Ttm_ActOut_04"]
                                                , drArrLogLedger[x]["Ttm_ActIn_05"]
                                                , drArrLogLedger[x]["Ttm_ActOut_05"]
                                                , drArrLogLedger[x]["Ttm_ActIn_06"]
                                                , drArrLogLedger[x]["Ttm_ActOut_06"]                //15
                                                , drArrLogLedger[x]["Ttm_ActIn_07"]
                                                , drArrLogLedger[x]["Ttm_ActOut_07"]
                                                , drArrLogLedger[x]["Ttm_ActIn_08"]
                                                , drArrLogLedger[x]["Ttm_ActOut_08"]
                                                , drArrLogLedger[x]["Ttm_ActIn_09"]                 //20
                                                , drArrLogLedger[x]["Ttm_ActOut_09"]
                                                , drArrLogLedger[x]["Ttm_ActIn_10"]
                                                , drArrLogLedger[x]["Ttm_ActOut_10"]
                                                , drArrLogLedger[x]["Ttm_ActIn_11"]
                                                , drArrLogLedger[x]["Ttm_ActOut_11"]                //25
                                                , drArrLogLedger[x]["Ttm_ActIn_12"]
                                                , drArrLogLedger[x]["Ttm_ActOut_12"]
                                                );
                iUpdateCtr++;
                if (iUpdateCtr == 150) //150 log ledger records per batch update
                {
                    dal.ExecuteNonQuery(strUpdateQuery);
                    strUpdateQuery = "";
                    iUpdateCtr = 0;
                }
            }

            if (strUpdateQuery != "")
                dal.ExecuteNonQuery(strUpdateQuery);

            Console.Write(" {0} Successfully Saved", drArrLogLedger.Length);
            #endregion

            #region Save DTR Records
            Console.Write("\n{0} Saving DTR...", dtProcessDate.ToShortDateString());
            drArrDTR = dtDTR.Select("Edited = 1 OR (PostFlag = 1 AND Edited = 0)");
            strUpdateTemplate = @"UPDATE {0}..T_EmpDTR SET Tel_IsPosted = {4} WHERE Tel_IDNo = '{1}' AND Tel_LogDate = '{2}' AND Tel_LogTime = '{3}' AND Tel_LogType = '{5}' ";
            strUpdateQuery = "";
            iUpdateCtr = 0;
            for (int x = 0; x < drArrDTR.Length; x++)
            {
                strUpdateQuery += string.Format(strUpdateTemplate
                                                , Globals.DTRDBName
                                                , drArrDTR[x]["EmployeeID"]
                                                , drArrDTR[x]["LogDate"]
                                                , drArrDTR[x]["LogTime"]
                                                , drArrDTR[x]["Edited"]
                                                , drArrDTR[x]["LogType"]);
                iUpdateCtr++;
                if (iUpdateCtr == 100) //from 150 to 100 DTR records per batch update
                {
                    dal.ExecuteNonQuery(strUpdateQuery);
                    strUpdateQuery = "";
                    iUpdateCtr = 0;
                }
            }

            if (strUpdateQuery != "")
                dal.ExecuteNonQuery(strUpdateQuery);

            Console.Write(" {0} Successfully Saved", drArrDTR.Length);
            #endregion

            Console.WriteLine("End: {0:M/dd/yyyy HH:mm:ss} ", DateTime.Now);
            return drArrDTR.Length;
        }

        public int PostLogsBasic4PlusExtension(string strDBName, string strCompanyCode, DateTime dtProcessDate, bool bPostPartial, string strSelectedEmployeeID, DALHelper dal)
        {
            #region Variables
            DataRow[] drArrLogLedger, drArrDTR, drArrTimeCor;
            string strEmployeeID, strScheduleType;

            int iActualTimeIn1Min, iActualTimeOut1Min, iActualTimeIn2Min, iActualTimeOut2Min,
                iActualTimeIn3Min, iActualTimeOut3Min, iActualTimeIn4Min, iActualTimeOut4Min,
                iActualTimeIn5Min, iActualTimeOut5Min, iActualTimeIn6Min, iActualTimeOut6Min,
                iActualTimeIn7Min, iActualTimeOut7Min, iActualTimeIn8Min, iActualTimeOut8Min,
                iActualTimeIn9Min, iActualTimeOut9Min, iActualTimeIn10Min, iActualTimeOut10Min,
                iActualTimeIn11Min, iActualTimeOut11Min, iActualTimeIn12Min, iActualTimeOut12Min;

            int iShiftTimeIn1Min, iShiftTimeOut1Min, iShiftTimeIn2Min, iShiftTimeOut2Min;
            int iBasicIn1Min, iBasicOut1Min, iBasicIn2Min, iBasicOut2Min;
            DateTime dtDTRDate;
            bool bEdited, bIsInHistTable = false;
            string SkipServiceFlag      = "";
            const int GRAVEYARD24       = 1440;
            bool RequiredLogsOnBreak    = false;
            #endregion

            Console.WriteLine("\nStart: {0:M/dd/yyyy HH:mm:ss} ", DateTime.Now);
            Console.WriteLine("{0} Getting Log Ledger records...", dtProcessDate.ToShortDateString());
            dtEmployeeLogLedger = GetLogLedgerRecordsPerDateMultiplePockets(strDBName, strCompanyCode, dtProcessDate, strSelectedEmployeeID, dal);
            Console.WriteLine("{0} Getting DTR records...", dtProcessDate.ToShortDateString());
            dtDTR = GetDTRRecordsPerDate(dtProcessDate, strSelectedEmployeeID, dal);
            //Console.WriteLine("{0} Getting DTR Override records...", dtProcessDate.ToShortDateString());
            //dtDtrOverride = GetDtrOverrideRecordsPerDate(strDBName, dtProcessDate, strSelectedEmployeeID, dal);
            Console.WriteLine("{0} Getting Time Correction records...", dtProcessDate.ToShortDateString());
            dtTimeCor = GetTimeCorrectionRecordsPerDateMultiplePockets(strDBName, dtProcessDate, strSelectedEmployeeID, dal);

            lstEmpDTR.Clear();
            lstEmpDTR = DataTableToList(dtDTR);

            /*LOG REMARKS
             A - Advance 
             M - Middle
             B - Break
             P - Pocketgap
            */

            for (int i = 0; i < dtEmployeeLogLedger.Rows.Count; i++)
            {
                Console.Write("\r{0} Posting logs {1} of {2}...", dtProcessDate.ToShortDateString(), i + 1, dtEmployeeLogLedger.Rows.Count);

                #region Initialize Variables
                bEdited             = false;

                strEmployeeID       = dtEmployeeLogLedger.Rows[i]["Ttm_IDNo"].ToString();
                strScheduleType     = dtEmployeeLogLedger.Rows[i]["ScheduleType"].ToString();
                SkipServiceFlag     = dtEmployeeLogLedger.Rows[i]["SkipService"].ToString();

                iActualTimeIn1Min   = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttm_ActIn_01"].ToString());
                iActualTimeOut1Min  = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttm_ActOut_01"].ToString());
                iActualTimeIn2Min   = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttm_ActIn_02"].ToString());
                iActualTimeOut2Min  = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttm_ActOut_02"].ToString());
                iActualTimeIn3Min   = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttm_ActIn_03"].ToString());
                iActualTimeOut3Min  = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttm_ActOut_03"].ToString());
                iActualTimeIn4Min   = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttm_ActIn_04"].ToString());
                iActualTimeOut4Min  = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttm_ActOut_04"].ToString());
                iActualTimeIn5Min   = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttm_ActIn_05"].ToString());
                iActualTimeOut5Min  = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttm_ActOut_05"].ToString());
                iActualTimeIn6Min   = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttm_ActIn_06"].ToString());
                iActualTimeOut6Min  = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttm_ActOut_06"].ToString());
                iActualTimeIn7Min   = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttm_ActIn_07"].ToString());
                iActualTimeOut7Min  = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttm_ActOut_07"].ToString());
                iActualTimeIn8Min   = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttm_ActIn_08"].ToString());
                iActualTimeOut8Min  = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttm_ActOut_08"].ToString());
                iActualTimeIn9Min   = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttm_ActIn_09"].ToString());
                iActualTimeOut9Min  = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttm_ActOut_09"].ToString());
                iActualTimeIn10Min  = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttm_ActIn_10"].ToString());
                iActualTimeOut10Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttm_ActOut_10"].ToString());
                iActualTimeIn11Min  = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttm_ActIn_11"].ToString());
                iActualTimeOut11Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttm_ActOut_11"].ToString());
                iActualTimeIn12Min  = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttm_ActIn_12"].ToString());
                iActualTimeOut12Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ttm_ActOut_12"].ToString());

                iShiftTimeIn1Min    = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["ShiftTimeIn"].ToString());
                iShiftTimeOut1Min   = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["ShiftBreakStart"].ToString());
                iShiftTimeIn2Min    = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["ShiftBreakEnd"].ToString());
                iShiftTimeOut2Min   = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["ShiftTimeOut"].ToString());
                RequiredLogsOnBreak = Convert.ToBoolean(dtEmployeeLogLedger.Rows[i]["RequiredLogsOnBreak"].ToString()); //TRUE = BASIC 4 + Extension; FALSE = Extension

                if (dtEmployeeLogLedger.Rows[i]["TableName"].ToString() == "T_EmpTimeRegisterMiscHst")
                    bIsInHistTable = true;

                //Convert to Graveyard
                if (iShiftTimeIn1Min > iShiftTimeOut1Min)
                    iShiftTimeOut1Min = iShiftTimeOut1Min + GRAVEYARD24;
                if (iShiftTimeIn1Min > iShiftTimeIn2Min)
                    iShiftTimeIn2Min = iShiftTimeIn2Min + GRAVEYARD24;
                if (iShiftTimeIn1Min > iShiftTimeOut2Min)
                    iShiftTimeOut2Min = iShiftTimeOut2Min + GRAVEYARD24;

                iBasicIn1Min        = 0;
                iBasicOut1Min       = 0;
                iBasicIn2Min        = 0;
                iBasicOut2Min       = 0;

                #endregion

                #region Process
                if (SkipServiceFlag.Equals("N"))
                {
                    var ArrDTR = lstEmpDTR.Where(e => e.IDNo == strEmployeeID).ToList();
                    if (ArrDTR != null && ArrDTR.Count() > 0)
                    {
                        #region Loop through DTR Logs (Regular FILO Posting)
                        for (int x = 0; x < ArrDTR.Count(); x++)
                        {
                            #region INITIALIZE VARIABLES
                            dtDTRDate                   = ArrDTR[x].LogDate;
                            ArrDTR[x].LogTimeMins       = GetMinsFromHourStr(ArrDTR[x].LogTime);
                            ArrDTR[x].LogTimeMinsOrig   = ArrDTR[x].LogTimeMins;                         

                            if (strScheduleType == "G" && dtProcessDate.AddDays(1) == dtDTRDate && ArrDTR[x].LogTimeMins != 0) //Graveyard Shift
                                ArrDTR[x].LogTimeMins = ArrDTR[x].LogTimeMins + GRAVEYARD24;
                            #endregion

                            #region EARLY IN OR IS EQUAL TO SHIFT IN1
                            if (((strScheduleType == "G" && dtProcessDate.AddDays(1) == dtDTRDate) || dtProcessDate == dtDTRDate) 
                                && ArrDTR[x].LogTimeMins != 0 && (ArrDTR[x].LogTimeMins >= (iShiftTimeIn1Min - Globals.EXTENDIN1) && ArrDTR[x].LogTimeMins <= iShiftTimeIn1Min)
                                && iActualTimeIn1Min == 0) //IN1 If TIME <= IN 1 : Post in IN 1 
                            {
                                iActualTimeIn1Min           = ArrDTR[x].LogTimeMins;
                                ArrDTR[x].PostFlag          = true;
                                ArrDTR[x].PocketNo          = 1;
                                ArrDTR[x].Remarks           = "IN1";
                                ArrDTR[x].Edited            = bEdited = true;
                            }
                            #endregion

                            #region LATE | LUNCHBREAK | UNDERTIME (IN BETWEEN)
                            else if (((strScheduleType == "G" && dtProcessDate.AddDays(1) == dtDTRDate) || dtProcessDate == dtDTRDate)
                                    && ArrDTR[x].LogTimeMins != 0
                                    && (ArrDTR[x].LogTimeMins > iShiftTimeIn1Min && ArrDTR[x].LogTimeMins < iShiftTimeOut2Min))
                            {
                                if (iActualTimeIn1Min == 0 &&
                                    (ArrDTR[x].LogTimeMins > iShiftTimeIn1Min && ArrDTR[x].LogTimeMins < iShiftTimeOut1Min))  //LATE IN1
                                {   //TIME > IN1 AND < OUT1 : Post in IN 1
                                    iActualTimeIn1Min       = ArrDTR[x].LogTimeMins;
                                    ArrDTR[x].PostFlag      = true;
                                    ArrDTR[x].PocketNo      = 1;
                                    ArrDTR[x].Remarks       = "IN1";
                                    ArrDTR[x].Edited        = bEdited = true;
                                }
                                else if (iActualTimeOut1Min == 0
                                    && (ArrDTR[x].LogTimeMins >= (iShiftTimeOut1Min - 60) && ArrDTR[x].LogTimeMins < iShiftTimeOut1Min)
                                    && (ArrDTR[x].LogTimeMins - iActualTimeIn1Min) >= Globals.POCKETTIME) //EARLY OUT || EARLY LUNCHBREAK
                                {   //IF TIME > IN1 AND < EARLY OUT
                                    iActualTimeOut1Min      = ArrDTR[x].LogTimeMins;
                                    ArrDTR[x].PostFlag      = true;
                                    ArrDTR[x].PocketNo      = 2;
                                    ArrDTR[x].Remarks       = "OUT1";
                                    ArrDTR[x].Edited        = bEdited = true;
                                }
                                else if (ArrDTR[x].LogTimeMins >= iShiftTimeOut1Min && ArrDTR[x].LogTimeMins < iShiftTimeIn2Min) //LUNCHBREAK
                                {  //IF TIME >= OUT1 and < IN 2 : POST OUT1
                                    if (iActualTimeOut1Min == 0
                                        && (ArrDTR[x].LogTimeMins - iActualTimeIn1Min) >= Globals.POCKETTIME
                                        && ArrDTR[x].LogTimeMins != iActualTimeIn2Min)
                                    {
                                        iActualTimeOut1Min  = ArrDTR[x].LogTimeMins;
                                        ArrDTR[x].PostFlag  = true;
                                        ArrDTR[x].PocketNo  = 2;
                                        ArrDTR[x].Remarks   = "OUT1";
                                        ArrDTR[x].Edited    = bEdited = true;
                                        ArrDTR[x].LogRemark = "B"; //break
                                    }
                                    else if (iActualTimeOut1Min > 0 //&& iActualTimeIn2Min == 0
                                        && Globals.POCKETGAP > 0 && (ArrDTR[x].LogTimeMins - iActualTimeOut1Min) >= Globals.POCKETGAP)
                                    { //IF TIME > = OUT1  and < IN2 and OUT1 is not Empty: POST IN2
                                        iActualTimeIn2Min   = ArrDTR[x].LogTimeMins;
                                        ArrDTR[x].PostFlag  = true;
                                        ArrDTR[x].PocketNo  = 3;
                                        ArrDTR[x].Remarks   = "IN2";
                                        ArrDTR[x].Edited    = bEdited = true;
                                        ArrDTR[x].LogRemark = "B"; //break
                                    }
                                }
                                else if (iActualTimeIn2Min == 0 &&
                                    (ArrDTR[x].LogTimeMins >= iShiftTimeIn2Min && ArrDTR[x].LogTimeMins < iShiftTimeOut2Min)
                                    && Globals.POCKETGAP > 0 && (ArrDTR[x].LogTimeMins - iActualTimeOut1Min) >= Globals.POCKETGAP)
                                {  //LATE IN2
                                    iActualTimeIn2Min       = ArrDTR[x].LogTimeMins;
                                    ArrDTR[x].PostFlag      = true;
                                    ArrDTR[x].PocketNo      = 3;
                                    ArrDTR[x].Remarks       = "IN2";
                                    ArrDTR[x].Edited        = bEdited = true;

                                    if (ArrDTR[x].LogTimeMins == iShiftTimeIn2Min)
                                        ArrDTR[x].LogRemark = "B"; //break
                                }
                                else if (iActualTimeIn2Min > 0
                                        && ArrDTR[x].LogTimeMins < iShiftTimeOut2Min
                                        && (ArrDTR[x].LogTimeMins - iActualTimeIn2Min) >= Globals.POCKETTIME) //UNDERTIME
                                {   // IF TIME > IN 2 and < OUT 2 : Post in OUT 2 
                                    iActualTimeOut2Min      = ArrDTR[x].LogTimeMins;
                                    ArrDTR[x].PostFlag      = true;
                                    ArrDTR[x].PocketNo      = 4;
                                    ArrDTR[x].Remarks       = "OUT2";
                                    ArrDTR[x].Edited        = bEdited = true;
                                }
                            }
                            #endregion

                            #region EQUAL OR AFTER SHIFT
                            else if (    (strScheduleType != "G" && (ArrDTR[x].LogTimeMins >= iShiftTimeOut2Min || (dtProcessDate.AddDays(1) == dtDTRDate && ArrDTR[x].LogTimeMins < (iShiftTimeIn1Min - Globals.EXTENDIN1))))
                                      || (strScheduleType == "G" && (dtProcessDate.AddDays(1) == dtDTRDate && ArrDTR[x].LogTimeMins < (iShiftTimeIn1Min + GRAVEYARD24) - Globals.EXTENDIN1))
                                    )
                            { //OUT2   If TIME >= OUT 2 : Post in Out 2
                                int iActualLogOutTemp = 0;

                                if (ArrDTR[x].LogTimeMins == 0)
                                    ArrDTR[x].LogTimeMinsOrig = ArrDTR[x].LogTimeMins = 1;

                                if (ArrDTR[x].LogTimeMins != 0 && strScheduleType != "G" && dtProcessDate.AddDays(1) == dtDTRDate) //INOUT on next day
                                {
                                    ArrDTR[x].LogTimeMins = ArrDTR[x].LogTimeMins + GRAVEYARD24;
                                    ArrDTR[x].LogTimeMinsOrig = ArrDTR[x].LogTimeMinsOrig + GRAVEYARD24;
                                } 

                                if (iActualTimeIn1Min == 0 || iActualTimeOut2Min == 0 || iActualTimeOut2Min > iActualTimeIn1Min)
                                    iActualLogOutTemp = iActualTimeOut2Min;
                                else
                                    iActualLogOutTemp = iActualTimeOut2Min + GRAVEYARD24;

                                if (((iActualTimeOut2Min == 0 && ArrDTR[x].LogTimeMins <= iShiftTimeOut1Min + GRAVEYARD24) //Do not post if greater than Shift Break of Next Day
                                    || (strScheduleType != "G" && dtProcessDate == dtDTRDate && ArrDTR[x].LogTimeMins > iActualLogOutTemp)
                                    || (strScheduleType != "G" && dtProcessDate.AddDays(1) == dtDTRDate && ArrDTR[x].LogTimeMins > iActualLogOutTemp && ArrDTR[x].LogTimeMins < ((iShiftTimeIn1Min + GRAVEYARD24) - Globals.EXTENDIN1))
                                    || (strScheduleType == "G" && ArrDTR[x].LogTimeMins > iActualLogOutTemp)))
                                {
                                    if (iActualTimeIn2Min == 0
                                        || (dtProcessDate == dtDTRDate && iActualTimeIn2Min > 0 && (ArrDTR[x].LogTimeMinsOrig - iActualTimeIn2Min) >= Globals.POCKETTIME)
                                        || (dtProcessDate != dtDTRDate && iActualTimeIn2Min > 0 && (ArrDTR[x].LogTimeMins - iActualTimeIn2Min) >= Globals.POCKETTIME)
                                        ) //Gap Validation
                                    {
                                        iActualTimeOut2Min      = ArrDTR[x].LogTimeMins;
                                        ArrDTR[x].PostFlag      = true; 
                                        ArrDTR[x].PocketNo      = 4;
                                        ArrDTR[x].Remarks       = "OUT2";
                                        ArrDTR[x].Edited        = bEdited = true;
                                    }
                                }
                            }
                            #endregion
                        }
                        #endregion

                        #region Secondary Pass
                        if (dtProcessDate < DateTime.Now) //Previous day checking
                        {
                            #region HALF DAY: MOVE IN2 INTO OUT1 (AM)
                            if (iActualTimeIn1Min != 0 && iActualTimeOut1Min == 0 && iActualTimeIn2Min != 0 && iActualTimeOut2Min == 0)
                            {   
                                //HALF DAY: MOVE IN2 INTO OUT1 (AM)
                                int iTempLogTime        = iActualTimeIn2Min;

                                if ((iTempLogTime - iActualTimeIn1Min) >= Globals.POCKETTIME
                                    && iTempLogTime <= iShiftTimeIn2Min)
                                {
                                    iActualTimeOut1Min      = iTempLogTime;
                                    int idx = ArrDTR.FindIndex(s => s.LogTimeMins == iTempLogTime
                                                                    && ((strScheduleType != "G" && (s.LogDate == dtProcessDate || s.LogDate == dtProcessDate.AddDays(1))) 
                                                                        || (strScheduleType == "G" && s.LogDate == dtProcessDate.AddDays(1)))
                                                               );
                                    iActualTimeIn2Min       = 0;
                                    ArrDTR[idx].PostFlag    = true;
                                    ArrDTR[idx].PocketNo    = 2;
                                    ArrDTR[idx].Remarks     = "OUT1";
                                    ArrDTR[idx].Edited      = bEdited = true;
                                }
                                else if ((iTempLogTime - iActualTimeIn1Min) >= Globals.POCKETTIME
                                         && iTempLogTime > iShiftTimeIn2Min && iTempLogTime < iShiftTimeOut2Min
                                         && !RequiredLogsOnBreak)
                                {
                                    //if ((dtProcessDate == dtDTRDate) || (strScheduleType == "G" && dtProcessDate.AddDays(1) == dtDTRDate))
                                    //    iActualTimeOut2Min = iTempLogTimeOrig;
                                    //else
                                    //    iActualTimeOut2Min = iTempLogTime; //+2400 if Next Day

                                    iActualTimeOut2Min      = iTempLogTime;
                                    int idx = ArrDTR.FindIndex(s => s.LogTimeMins == iTempLogTime
                                                                    && ((strScheduleType != "G" && (s.LogDate == dtProcessDate || s.LogDate == dtProcessDate.AddDays(1))) 
                                                                        || (strScheduleType == "G" && s.LogDate == dtProcessDate.AddDays(1)))
                                                               );
                                    iActualTimeIn2Min       = 0;
                                    ArrDTR[idx].PostFlag    = true;
                                    ArrDTR[idx].PocketNo    = 4;
                                    ArrDTR[idx].Remarks     = "OUT2";
                                    ArrDTR[idx].Edited      = bEdited = true;
                                }

                            }
                            #endregion

                            #region HALF DAY: MOVE OUT1 INTO IN2 (PM)
                            if (iActualTimeIn1Min == 0 && iActualTimeOut1Min != 0 && iActualTimeIn2Min == 0 && iActualTimeOut2Min != 0
                                && (iActualTimeOut2Min - iActualTimeOut1Min) >= Globals.POCKETTIME)
                            {   //HALF DAY: MOVE OUT1 INTO IN2 (PM)
                                iActualTimeIn2Min = iActualTimeOut1Min;
                                int idx = ArrDTR.FindIndex(s => s.LogTimeMins == iActualTimeOut1Min
                                                                && ((strScheduleType != "G" && (s.LogDate == dtProcessDate || s.LogDate == dtProcessDate.AddDays(1))) 
                                                                    || (strScheduleType == "G" && s.LogDate == dtProcessDate.AddDays(1)))
                                                           );
                                iActualTimeOut1Min          = 0;
                                ArrDTR[idx].PostFlag        = true;
                                ArrDTR[idx].PocketNo        = 3;
                                ArrDTR[idx].Remarks         = "IN2";
                                ArrDTR[idx].Edited          = bEdited = true;
                            }
                            #endregion

                            if (iActualTimeIn1Min != 0 && iActualTimeOut1Min == 0 && iActualTimeIn2Min == 0 && iActualTimeOut2Min == 0)
                            {
                                #region UNPOSTED UNDERTIME (AM)
                                for (int x = 0; x < ArrDTR.Count(); x++)
                                {
                                    if (ArrDTR[x].LogTimeMins != 0 && ArrDTR[x].LogTimeMins != iActualTimeIn1Min
                                        && ArrDTR[x].LogTimeMins > iShiftTimeIn1Min && ArrDTR[x].LogTimeMins < iShiftTimeOut1Min
                                        && ArrDTR[x].Remarks == ""
                                        && ((strScheduleType != "G" && ArrDTR[x].LogDate == dtProcessDate)
                                            || (strScheduleType == "G" && ArrDTR[x].LogDate == dtProcessDate.AddDays(1)))
                                        && (ArrDTR[x].LogTimeMins - iActualTimeIn1Min) >= Globals.POCKETTIME)
                                    {
                                        iActualTimeOut1Min      = ArrDTR[x].LogTimeMins;
                                        ArrDTR[x].PostFlag      = true;
                                        ArrDTR[x].PocketNo      = 2;
                                        ArrDTR[x].Remarks       = "OUT1";
                                        ArrDTR[x].Edited        = bEdited = true;
                                    }

                                    if (ArrDTR[x].LogTimeMins >= iShiftTimeOut1Min)
                                        break;
                                }
                                #endregion
                            }

                            if (iActualTimeIn2Min != 0 && iActualTimeOut2Min == 0)
                            {
                                #region UNPOSTED UNDERTIME (PM)
                                for (int x = 0; x < ArrDTR.Count(); x++)
                                {
                                    if (ArrDTR[x].LogTimeMins != 0 && ArrDTR[x].LogTimeMins != iActualTimeIn2Min
                                        && ArrDTR[x].Remarks == ""
                                        && ((strScheduleType != "G" && ((dtProcessDate == ArrDTR[x].LogDate && ArrDTR[x].LogTimeMins > iShiftTimeIn2Min && ArrDTR[x].LogTimeMins < iShiftTimeIn2Min) || (dtProcessDate.AddDays(1) == ArrDTR[x].LogDate && ArrDTR[x].LogTimeMins > iShiftTimeIn2Min && ArrDTR[x].LogTimeMins < (iShiftTimeIn1Min - Globals.EXTENDIN1))))
                                            || (strScheduleType == "G" && (dtProcessDate.AddDays(1) == ArrDTR[x].LogDate && ArrDTR[x].LogTimeMins > iShiftTimeIn2Min && ArrDTR[x].LogTimeMins < (iShiftTimeIn1Min + GRAVEYARD24) - Globals.EXTENDIN1)))
                                        && ((dtProcessDate == ArrDTR[x].LogDate && (ArrDTR[x].LogTimeMins - iActualTimeIn2Min) >= Globals.POCKETTIME) || (dtProcessDate != ArrDTR[x].LogDate && (ArrDTR[x].LogTimeMins - iActualTimeIn2Min) >= Globals.POCKETTIME)))
                                    {
                                        iActualTimeOut2Min      = ArrDTR[x].LogTimeMins;
                                        ArrDTR[x].PostFlag      = true;
                                        ArrDTR[x].PocketNo      = 4;
                                        ArrDTR[x].Remarks       = "OUT2";
                                        ArrDTR[x].Edited        = bEdited = true;
                                    }
                                }
                                #endregion
                            }

                            if (iActualTimeIn1Min != 0 && iActualTimeOut1Min != 0 && iActualTimeIn2Min == 0 && iActualTimeOut2Min != 0)
                            {
                                #region UNPAIRED BREAK - NO IN2
                                var idxOut1 = ArrDTR.FindIndex(s => s.LogTimeMins == iActualTimeOut1Min);
                                for (int x = (idxOut1-1); x > 0; x--)
                                {
                                    if (ArrDTR[x].LogTimeMins != 0 && ArrDTR[x].LogTimeMins != iActualTimeIn1Min
                                         && ((strScheduleType != "G" && ArrDTR[x].LogDate == dtProcessDate)
                                            || (strScheduleType == "G" && ArrDTR[x].LogDate == dtProcessDate.AddDays(1)))
                                         && (ArrDTR[x].LogTimeMins - iActualTimeIn1Min) >= Globals.POCKETTIME
                                         && (iActualTimeOut1Min - ArrDTR[x].LogTimeMins) >= Globals.POCKETGAP)
                                    {
                                        iActualTimeIn2Min = iActualTimeOut1Min;
                                        ArrDTR[idxOut1].PostFlag    = true;
                                        ArrDTR[idxOut1].PocketNo    = 3;
                                        ArrDTR[idxOut1].Remarks     = "IN2";
                                        ArrDTR[idxOut1].Edited      = bEdited = true;

                                        iActualTimeOut1Min          = ArrDTR[x].LogTimeMins;
                                        ArrDTR[x].PostFlag          = true;
                                        ArrDTR[x].PocketNo          = 2;
                                        ArrDTR[x].Remarks           = "OUT1";
                                        ArrDTR[x].Edited            = bEdited = true;
                                        break;
                                    }
                                }
                                #endregion
                            }

                            if (iActualTimeIn1Min != 0 && iActualTimeOut1Min == 0 && iActualTimeIn2Min != 0 && iActualTimeOut2Min != 0)
                            {
                                #region UNPAIRED BREAK - NO OUT1
                                var idxIn2 = ArrDTR.FindIndex(s => s.LogTimeMins == iActualTimeIn2Min);
                                for (int x = (idxIn2-1); x > 0; x--)
                                {
                                    if (ArrDTR[x].LogTimeMins != 0 && ArrDTR[x].LogTimeMins != iActualTimeIn1Min
                                        && ((strScheduleType != "G" && ArrDTR[x].LogDate == dtProcessDate)
                                            || (strScheduleType == "G" && ArrDTR[x].LogDate == dtProcessDate.AddDays(1)))
                                         && (ArrDTR[x].LogTimeMins - iActualTimeIn1Min) >= Globals.POCKETTIME
                                         && (iActualTimeIn2Min - ArrDTR[x].LogTimeMins) >= Globals.POCKETGAP)
                                    {
                                        iActualTimeOut1Min  = ArrDTR[x].LogTimeMins;
                                        ArrDTR[x].PostFlag  = true;
                                        ArrDTR[x].PocketNo  = 2;
                                        ArrDTR[x].Remarks   = "OUT1";
                                        ArrDTR[x].Edited    = bEdited = true;
                                    }
                                }
                                #endregion
                            }
                            if (iActualTimeIn1Min != 0 && iActualTimeOut1Min != 0 && iActualTimeIn2Min != 0 && iActualTimeOut2Min != 0)
                            {
                                #region MAX LATE (PM) 
                                var tmpArr = ArrDTR.Where(s => (s.LogTimeMins != 0
                                                            && s.LogTimeMins != iActualTimeIn2Min
                                                            && s.LogDate == dtProcessDate
                                                            && s.LogTimeMins > iShiftTimeIn2Min
                                                            && s.LogTimeMins <= (iShiftTimeIn2Min + Globals.LATEMAX2))
                                                       || (strScheduleType == "G"
                                                           && s.LogTimeMins != 0
                                                           && s.LogTimeMins != iActualTimeIn2Min
                                                           && s.LogDate == dtProcessDate.AddDays(1)
                                                           && s.LogTimeMins > iShiftTimeIn2Min
                                                           && s.LogTimeMins <= (iShiftTimeIn2Min + Globals.LATEMAX2)));
                                if (tmpArr != null && tmpArr.Count() > 0)
                                {
                                    var idxIn2 = ArrDTR.FindIndex(s => (s.LogTimeMins != 0
                                                                            && s.LogTimeMins != iActualTimeIn2Min
                                                                            && s.LogDate == dtProcessDate
                                                                            && s.LogTimeMins > iShiftTimeIn2Min
                                                                            && s.LogTimeMins <= (iShiftTimeIn2Min + Globals.LATEMAX2))
                                                                       || (strScheduleType == "G"
                                                                           && s.LogTimeMins != 0
                                                                           && s.LogTimeMins != iActualTimeIn2Min
                                                                           && s.LogDate == dtProcessDate.AddDays(1)
                                                                           && s.LogTimeMins > iShiftTimeIn2Min
                                                                           && s.LogTimeMins <= (iShiftTimeIn2Min + Globals.LATEMAX2)));

                                    if ((ArrDTR[idxIn2].LogTimeMins - iActualTimeOut1Min) >= Globals.POCKETGAP
                                        && ArrDTR[idxIn2].LogTimeMins != iActualTimeIn2Min
                                        && ArrDTR[idxIn2].LogTimeMins != iActualTimeOut2Min)
                                    {
                                        iActualTimeIn2Min           = ArrDTR[idxIn2].LogTimeMins;
                                        ArrDTR[idxIn2].PostFlag     = true;
                                        ArrDTR[idxIn2].PocketNo     = 3;
                                        ArrDTR[idxIn2].Remarks      = "IN2";
                                        ArrDTR[idxIn2].Edited       = bEdited = true;
                                    }
                                }
                                #endregion
                            }
                        }
                        #endregion

                        #region Basic Four Marker Clean-up
                        CleanupMin(ArrDTR, "IN1");
                        CleanupMin(ArrDTR, "OUT1");
                        CleanupMax(ArrDTR, "IN2");
                        CleanupMax(ArrDTR, "OUT2");
                        #endregion

                        #region Convert to Multiple Pockets
                        bool bFirstHalf         = SplitBasicFour(ArrDTR, iShiftTimeIn1Min, "IN1", "OUT1");
                        bool bSecondHalf        = SplitBasicFour(ArrDTR, iShiftTimeIn1Min, "IN2", "OUT2");
                        #endregion
                        #region PocketGap/PocketTime Tagging
                        for (int x = 0; x < ArrDTR.Count(); x++)
                        {
                            if (x + 1 < ArrDTR.Count())
                            {
                                if ((strScheduleType != "G" && (dtProcessDate.AddDays(1) == ArrDTR[x + 1].LogDate && ArrDTR[x + 1].LogTimeMinsOrig >= (iShiftTimeIn1Min - Globals.EXTENDIN1)))
                                    || (strScheduleType == "G" && (dtProcessDate.AddDays(1) == ArrDTR[x + 1].LogDate && ArrDTR[x + 1].LogTimeMins >= iShiftTimeIn1Min - Globals.EXTENDIN1))
                                    )
                                    break;

                                if ((ArrDTR[x+1].LogTimeMins - ArrDTR[x].LogTimeMins) < Globals.POCKETGAP)
                                {
                                    if (!ArrDTR[x].PostFlag && ArrDTR[x].LogRemark != "B") //not equal to break
                                        ArrDTR[x].LogRemark = "P";

                                    if (!ArrDTR[x+1].PostFlag && ArrDTR[x+1].LogRemark != "B") //not equal to break
                                        ArrDTR[x+1].LogRemark = "P";
                                }
                            }

                            #region BY POSTFLAG LOGIC
                            //if (bStartProc)
                            //{
                            //    if (!ArrDTR[x].PostFlag && ArrDTR[x].PocketNo == 0)
                            //    {
                            //        if (ArrDTR[idxlstIndex].PocketNo % 2 != 0
                            //            && (ArrDTR[x].LogTimeMins - ArrDTR[idxlstIndex].LogTimeMins) < Globals.POCKETTIME) //IN
                            //        {
                            //            ArrDTR[x].LogRemark = "P";
                            //            ArrDTR[x].Edited = true;
                            //        }
                            //        else if (ArrDTR[idxlstIndex].PocketNo % 2 == 0
                            //            && (ArrDTR[x].LogTimeMins - ArrDTR[idxlstIndex].LogTimeMins) < Globals.POCKETGAP) //OUT
                            //        {
                            //            ArrDTR[x].LogRemark = "P";
                            //            ArrDTR[x].Edited = true;
                            //        }
                            //    }

                            //    if (ArrDTR[x].PocketNo != 0)
                            //        idxlstIndex = x;
                            //}
                            //else
                            //{
                            //    if (ArrDTR[x].PocketNo != 0)
                            //    {
                            //        idxlstIndex = x;
                            //        bStartProc = true;
                            //    }
                            //}

                            ////if ((strScheduleType != "G" && ((dtProcessDate == ArrDTR[x].LogDate && ArrDTR[x].LogTimeMins >= (iShiftTimeIn1Min - Globals.EXTENDIN1)) && (dtProcessDate.AddDays(1) == ArrDTR[x].LogDate && ArrDTR[x].LogTimeMinsOrig < (iShiftTimeIn1Min - Globals.EXTENDIN1))))
                            ////||  (strScheduleType == "G" && (((dtProcessDate == ArrDTR[x].LogDate || dtProcessDate.AddDays(1) == ArrDTR[x].LogDate) && ArrDTR[x].LogTimeMins >= (iShiftTimeIn1Min - Globals.EXTENDIN1)) && (dtProcessDate.AddDays(1) == ArrDTR[x].LogDate && ArrDTR[x].LogTimeMins < iShiftTimeIn1Min - Globals.EXTENDIN1)))
                            ////   )
                            #endregion

                        }
                        #endregion

                        #region Middle Logs Pass
                        if (Globals.LOGPOSTINGTYPE == "O")
                        {
                            #region Oppressive|Strict Posting
                            for (int x = 0; x < ArrDTR.Count(); x++)
                            {
                                if (ArrDTR[x].LogRemark == "M" && ArrDTR[x].PostFlag == false)
                                {
                                    if (ArrDTR[x].Remarks == "" && ArrDTR[x + 1].Remarks == "OUT1"
                                        && (ArrDTR[x].LogTimeMins - iActualTimeIn1Min) >= Globals.POCKETTIME)
                                    {
                                        #region UPDATE MAX UNPAIRED TO OUT1 (AM)
                                        ArrDTR[x].PostFlag = true;
                                        ArrDTR[x].PocketNo = ArrDTR[x + 1].PocketNo;
                                        ArrDTR[x].LogRemark = "";
                                        ArrDTR[x].Remarks = "OUT1";
                                        ArrDTR[x].Edited = bEdited = true;

                                        ArrDTR[x + 1].PostFlag = false;
                                        ArrDTR[x + 1].PocketNo = 0;
                                        ArrDTR[x + 1].LogRemark = "";
                                        ArrDTR[x + 1].Remarks = "";
                                        ArrDTR[x + 1].Edited = false;
                                        #endregion
                                    }
                                    else if (ArrDTR[x].Remarks == "" && ArrDTR[x + 1].Remarks == "OUT2"
                                        && (ArrDTR[x].LogTimeMins - iActualTimeIn2Min) >= Globals.POCKETTIME)
                                    {
                                        #region UPDATE MAX UNPAIRED TO OUT2 (PM)
                                        ArrDTR[x].PostFlag = true;
                                        ArrDTR[x].PocketNo = ArrDTR[x + 1].PocketNo;
                                        ArrDTR[x].LogRemark = "";
                                        ArrDTR[x].Remarks = "OUT2";
                                        ArrDTR[x].Edited = bEdited = true;

                                        ArrDTR[x + 1].PostFlag = false;
                                        ArrDTR[x + 1].PocketNo = 0;
                                        ArrDTR[x + 1].LogRemark = "";
                                        ArrDTR[x + 1].Remarks = "";
                                        ArrDTR[x + 1].Edited = false;
                                        break;
                                        #endregion
                                    }
                                }
                            }
                            #endregion
                        }
                        #endregion

                        #region Reassign Values
                        for (int x = 0; x < ArrDTR.Count(); x++)
                        {
                            switch (ArrDTR[x].PocketNo)
                            {
                                case 1:
                                    iActualTimeIn1Min = ArrDTR[x].LogTimeMinsOrig;
                                    bEdited = true;
                                    break;
                                case 2:
                                    iActualTimeOut1Min = ArrDTR[x].LogTimeMinsOrig;
                                    bEdited = true;
                                    break;
                                case 3:
                                    iActualTimeIn2Min = ArrDTR[x].LogTimeMinsOrig;
                                    bEdited = true;
                                    break;
                                case 4:
                                    iActualTimeOut2Min = ArrDTR[x].LogTimeMinsOrig;
                                    break;
                                case 5:
                                    iActualTimeIn3Min = ArrDTR[x].LogTimeMinsOrig;
                                    bEdited = true;
                                    break;
                                case 6:
                                    iActualTimeOut3Min = ArrDTR[x].LogTimeMinsOrig;
                                    bEdited = true;
                                    break;
                                case 7:
                                    iActualTimeIn4Min = ArrDTR[x].LogTimeMinsOrig;
                                    bEdited = true;
                                    break;
                                case 8:
                                    iActualTimeOut4Min = ArrDTR[x].LogTimeMinsOrig;
                                    bEdited = true;
                                    break;
                                case 9:
                                    iActualTimeIn5Min = ArrDTR[x].LogTimeMinsOrig;
                                    bEdited = true;
                                    break;
                                case 10:
                                    iActualTimeOut5Min = ArrDTR[x].LogTimeMinsOrig;
                                    break;
                                case 11:
                                    iActualTimeIn6Min = ArrDTR[x].LogTimeMinsOrig;
                                    bEdited = true;
                                    break;
                                case 12:
                                    iActualTimeOut6Min = ArrDTR[x].LogTimeMinsOrig;
                                    bEdited = true;
                                    break;
                                case 13:
                                    iActualTimeIn7Min = ArrDTR[x].LogTimeMinsOrig;
                                    bEdited = true;
                                    break;
                                case 14:
                                    iActualTimeOut7Min = ArrDTR[x].LogTimeMinsOrig;
                                    bEdited = true;
                                    break;
                                case 15:
                                    iActualTimeIn8Min = ArrDTR[x].LogTimeMinsOrig;
                                    bEdited = true;
                                    break;
                                case 16:
                                    iActualTimeOut8Min = ArrDTR[x].LogTimeMinsOrig;
                                    bEdited = true;
                                    break;
                                case 17:
                                    iActualTimeIn9Min = ArrDTR[x].LogTimeMinsOrig;
                                    bEdited = true;
                                    break;
                                case 18:
                                    iActualTimeOut9Min = ArrDTR[x].LogTimeMinsOrig;
                                    bEdited = true;
                                    break;
                                case 19:
                                    iActualTimeIn10Min = ArrDTR[x].LogTimeMinsOrig;
                                    bEdited = true;
                                    break;
                                case 20:
                                    iActualTimeOut10Min = ArrDTR[x].LogTimeMinsOrig;
                                    bEdited = true;
                                    break;
                                case 21:
                                    iActualTimeIn11Min = ArrDTR[x].LogTimeMinsOrig;
                                    bEdited = true;
                                    break;
                                case 22:
                                    iActualTimeOut11Min = ArrDTR[x].LogTimeMinsOrig;
                                    bEdited = true;
                                    break;
                                case 23:
                                    iActualTimeIn12Min = ArrDTR[x].LogTimeMinsOrig;
                                    bEdited = true;
                                    break;
                                case 24:
                                    iActualTimeOut12Min = ArrDTR[x].LogTimeMinsOrig;
                                    bEdited = true;
                                    break;
                            }

                        }
                        #endregion

                        #region Save DTR Values 
                        for (int x = 0; x < ArrDTR.Count(); x++)
                        {
                            if (ArrDTR[x].Edited || !ArrDTR[x].LogRemark.Equals(string.Empty))
                            {
                                drArrDTR = dtDTR.Select(string.Format("EmployeeID = '{0}' AND LogDate = '{1}' AND LogTime = '{2}' AND [Seq] = {3}", strEmployeeID, ArrDTR[x].LogDate, ArrDTR[x].LogTime, ArrDTR[x].Seq));
                                if (drArrDTR != null && drArrDTR.Length > 0)
                                {
                                    drArrDTR[0]["PostFlag"]     = (ArrDTR[x].PostFlag ? 1 : 0);
                                    drArrDTR[0]["Edited"]       = 1;
                                    drArrDTR[0]["LogRemark"]    = ArrDTR[x].LogRemark;
                                }
                            }

                            #region Assign Marker
                            if (ArrDTR[x].Remarks == "IN1")
                                iBasicIn1Min = ArrDTR[x].LogTimeMinsOrig;
                            else if (ArrDTR[x].Remarks == "OUT1")
                                iBasicOut1Min = ArrDTR[x].LogTimeMinsOrig;
                            else if (ArrDTR[x].Remarks == "IN2")
                                iBasicIn2Min = ArrDTR[x].LogTimeMinsOrig;
                            else if (ArrDTR[x].Remarks == "OUT2")
                                iBasicOut2Min = ArrDTR[x].LogTimeMinsOrig;
                            #endregion
                        }
                        #endregion
                    }
                }
                #endregion

                #region //DTR Override Reposting
                    //drDTROverride = dtDtrOverride.Select(string.Format("Tdo_IDNo = '{0}'", strEmployeeID));
                    //if (drDTROverride.Length > 0)
                    //{
                    //    //Get First Sequence Record Only
                    //    iTrailTimeIn1Min = GetMinsFromHourStr(drDTROverride[0]["Tdo_Time"].ToString());

                    //    #region Post according to DTR Override Type
                    //    switch (drDTROverride[0]["Tdo_Type"].ToString())
                    //    {
                    //        case "I1": //I1-IN1
                    //            if (iTrailTimeIn1Min != 0)
                    //            {
                    //                iActualTimeIn1Min = iTrailTimeIn1Min;
                    //                bEdited = true;
                    //            }
                    //            break;
                    //        case "I2": //I2-IN2
                    //            if (iTrailTimeIn1Min != 0)
                    //            {
                    //                iActualTimeIn2Min = iTrailTimeIn1Min;
                    //                bEdited = true;
                    //            }
                    //            break;
                    //        case "O1": //O1-OUT1
                    //            if (iTrailTimeIn1Min != 0)
                    //            {
                    //                iActualTimeOut1Min = iTrailTimeIn1Min;
                    //                bEdited = true;
                    //            }
                    //            break;
                    //        case "O2": //O2-OUT2
                    //            if (iTrailTimeIn1Min != 0)
                    //            {
                    //                iActualTimeOut2Min = iTrailTimeIn1Min;
                    //                bEdited = true;
                    //            }
                    //            break;
                    //    }
                    //    #endregion

                    //}
                    #endregion

                #region Time Correction Reposting
                    drArrTimeCor = dtTimeCor.Select(string.Format("Ttm_IDNo = '{0}'", strEmployeeID));
                if (drArrTimeCor.Length > 0)
                {
                    #region Post according to Pocket No
                    for (int x = 0; x < drArrTimeCor.Length; x++)
                    {
                        switch(Convert.ToInt32(drArrTimeCor[x]["PocketNo"]))
                        {
                            case 1:
                                iActualTimeIn1Min = GetMinsFromHourStr(drArrTimeCor[x]["LogTime"].ToString());
                                bEdited = true;
                                break;
                            case 2:
                                iActualTimeOut1Min = GetMinsFromHourStr(drArrTimeCor[x]["LogTime"].ToString());
                                bEdited = true;
                                break;
                            case 3:
                                iActualTimeIn2Min = GetMinsFromHourStr(drArrTimeCor[x]["LogTime"].ToString());
                                bEdited = true;
                                break;
                            case 4:
                                iActualTimeOut2Min = GetMinsFromHourStr(drArrTimeCor[x]["LogTime"].ToString());
                                bEdited = true;
                                break;
                            case 5:
                                iActualTimeIn3Min = GetMinsFromHourStr(drArrTimeCor[x]["LogTime"].ToString());
                                bEdited = true;
                                break;
                            case 6:
                                iActualTimeOut3Min = GetMinsFromHourStr(drArrTimeCor[x]["LogTime"].ToString());
                                bEdited = true;
                                break;
                            case 7:
                                iActualTimeIn4Min = GetMinsFromHourStr(drArrTimeCor[x]["LogTime"].ToString());
                                bEdited = true;
                                break;
                            case 8:
                                iActualTimeOut4Min = GetMinsFromHourStr(drArrTimeCor[x]["LogTime"].ToString());
                                bEdited = true;
                                break;
                            case 9:
                                iActualTimeIn5Min = GetMinsFromHourStr(drArrTimeCor[x]["LogTime"].ToString());
                                bEdited = true;
                                break;
                            case 10:
                                iActualTimeOut5Min = GetMinsFromHourStr(drArrTimeCor[x]["LogTime"].ToString());
                                bEdited = true;
                                break;
                            case 11:
                                iActualTimeIn6Min = GetMinsFromHourStr(drArrTimeCor[x]["LogTime"].ToString());
                                bEdited = true;
                                break;
                            case 12:
                                iActualTimeOut6Min = GetMinsFromHourStr(drArrTimeCor[x]["LogTime"].ToString());
                                bEdited = true;
                                break;
                            case 13:
                                iActualTimeIn7Min = GetMinsFromHourStr(drArrTimeCor[x]["LogTime"].ToString());
                                bEdited = true;
                                break;
                            case 14:
                                iActualTimeOut7Min = GetMinsFromHourStr(drArrTimeCor[x]["LogTime"].ToString());
                                bEdited = true;
                                break;
                            case 15:
                                iActualTimeIn8Min = GetMinsFromHourStr(drArrTimeCor[x]["LogTime"].ToString());
                                bEdited = true;
                                break;
                            case 16:
                                iActualTimeOut8Min = GetMinsFromHourStr(drArrTimeCor[x]["LogTime"].ToString());
                                bEdited = true;
                                break;
                            case 17:
                                iActualTimeIn9Min = GetMinsFromHourStr(drArrTimeCor[x]["LogTime"].ToString());
                                bEdited = true;
                                break;
                            case 18:
                                iActualTimeOut9Min = GetMinsFromHourStr(drArrTimeCor[x]["LogTime"].ToString());
                                bEdited = true;
                                break;
                            case 19:
                                iActualTimeIn10Min = GetMinsFromHourStr(drArrTimeCor[x]["LogTime"].ToString());
                                bEdited = true;
                                break;
                            case 20:
                                iActualTimeOut10Min = GetMinsFromHourStr(drArrTimeCor[x]["LogTime"].ToString());
                                bEdited = true;
                                break;
                            case 21:
                                iActualTimeIn11Min = GetMinsFromHourStr(drArrTimeCor[x]["LogTime"].ToString());
                                bEdited = true;
                                break;
                            case 22:
                                iActualTimeOut11Min = GetMinsFromHourStr(drArrTimeCor[x]["LogTime"].ToString());
                                bEdited = true;
                                break;
                            case 23:
                                iActualTimeIn12Min = GetMinsFromHourStr(drArrTimeCor[x]["LogTime"].ToString());
                                bEdited = true;
                                break;
                            case 24:
                                iActualTimeOut12Min = GetMinsFromHourStr(drArrTimeCor[x]["LogTime"].ToString());
                                bEdited = true;
                                break;
                        }
                        
                    }
                    #endregion
                }
                #endregion

                #region Save Values
                if (bEdited)
                {
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActIn_01"]     = GetHourStrFromMins(iActualTimeIn1Min);
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActOut_01"]    = GetHourStrFromMins(iActualTimeOut1Min);
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActIn_02"]     = GetHourStrFromMins(iActualTimeIn2Min);
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActOut_02"]    = GetHourStrFromMins(iActualTimeOut2Min);
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActIn_03"]     = GetHourStrFromMins(iActualTimeIn3Min);
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActOut_03"]    = GetHourStrFromMins(iActualTimeOut3Min);
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActIn_04"]     = GetHourStrFromMins(iActualTimeIn4Min);
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActOut_04"]    = GetHourStrFromMins(iActualTimeOut4Min);
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActIn_05"]     = GetHourStrFromMins(iActualTimeIn5Min);
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActOut_05"]    = GetHourStrFromMins(iActualTimeOut5Min);
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActIn_06"]     = GetHourStrFromMins(iActualTimeIn6Min);
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActOut_06"]    = GetHourStrFromMins(iActualTimeOut6Min);
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActIn_07"]     = GetHourStrFromMins(iActualTimeIn7Min);
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActOut_07"]    = GetHourStrFromMins(iActualTimeOut7Min);
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActIn_08"]     = GetHourStrFromMins(iActualTimeIn8Min);
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActOut_08"]    = GetHourStrFromMins(iActualTimeOut8Min);
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActIn_09"]     = GetHourStrFromMins(iActualTimeIn9Min);
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActOut_09"]    = GetHourStrFromMins(iActualTimeOut9Min);
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActIn_10"]     = GetHourStrFromMins(iActualTimeIn10Min);
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActOut_10"]    = GetHourStrFromMins(iActualTimeOut10Min);
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActIn_11"]     = GetHourStrFromMins(iActualTimeIn11Min);
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActOut_11"]    = GetHourStrFromMins(iActualTimeOut11Min);
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActIn_12"]     = GetHourStrFromMins(iActualTimeIn12Min);
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActOut_12"]    = GetHourStrFromMins(iActualTimeOut12Min);
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActIn1"]       = GetHourStrFromMins(iBasicIn1Min);
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActOut1"]      = GetHourStrFromMins(iBasicOut1Min);
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActIn2"]       = GetHourStrFromMins(iBasicIn2Min);
                    dtEmployeeLogLedger.Rows[i]["Ttm_ActOut2"]      = GetHourStrFromMins(iBasicOut2Min);
                    dtEmployeeLogLedger.Rows[i]["Edited"]           = 1;
                }
                #endregion
            }

            #region Save Log Ledger Records
            Console.Write("\n{0} Saving log ledger...", dtProcessDate.ToShortDateString());
            drArrLogLedger = dtEmployeeLogLedger.Select("Edited = 1");
            string strShiftTable = "";
            string strUpdateQuery = "";
            string strUpdateQuery2 = "";
            string strUpdateQuery3 = "";
            int iUpdateCtr = 0;

            string strLogLedgerMiscTable = "T_EmpTimeRegisterMisc";
            string strLogLedgerTable = "T_EmpTimeRegister";
            if (bIsInHistTable == true)
            {
                strLogLedgerMiscTable = "T_EmpTimeRegisterMiscHst";
                strLogLedgerTable = "T_EmpTimeRegisterHst";
            }

            string strCreateFlexShiftTable = @"IF OBJECT_ID('tempdb..#ShiftCodeMaster') IS NOT NULL
                                                DROP TABLE #ShiftCodeMaster
										   
                                                SELECT  ISNULL(PARSENAME(REPLACE(CUR.Msh_ShiftCode, '-', '.'),2), CUR.Msh_ShiftCode) AS FlexCode
	                                                , PRV.Msh_ShiftCode AS [PRV Shift]
	                                                , CUR.Msh_ShiftCode AS [CUR Shift]
	                                                , NXT.Msh_ShiftCode AS [NXT Shift]
	                                                , ISNULL((SELECT TOP 1 Msh_ShiftCode			
		                                                FROM {0}..M_Shift		
		                                                WHERE Msh_IsDefaultShift = 1		
			                                                AND Msh_RecordStatus = 'A'
			                                                AND Msh_CompanyCode = CUR.Msh_CompanyCode	
			                                                AND ISNULL(PARSENAME(REPLACE(Msh_ShiftCode, '-', '.'),2), Msh_ShiftCode) 	
				                                                = ISNULL(PARSENAME(REPLACE(CUR.Msh_ShiftCode, '-', '.'),2), CUR.Msh_ShiftCode)), CUR.Msh_ShiftCode) AS DefaultShiftCode
	                                                , PRV.Msh_ShiftOut1 AS [PRV OUT1]
	                                                , CUR.Msh_ShiftOut1 AS [CUR OUT1]
	                                                , NXT.Msh_ShiftOut1 AS [NXT OUT1]
                                                INTO #ShiftCodeMaster
                                                FROM {0}..M_Shift CUR				
                                                LEFT JOIN {0}..M_Shift PRV ON PRV.Msh_ShiftCode = (SELECT MAX(Msh_ShiftCode) FROM {0}..M_Shift
										                                                WHERE Msh_ShiftCode < CUR.Msh_ShiftCode  AND Msh_CompanyCode = CUR.Msh_CompanyCode
											                                                AND Msh_FlexShift = 1 AND Msh_RecordStatus = 'A'  
											                                                AND PARSENAME(REPLACE(PRV.Msh_ShiftCode, '-', '.'),2) = PARSENAME(REPLACE(CUR.Msh_ShiftCode, '-', '.'),2))
	                                                AND PARSENAME(REPLACE(PRV.Msh_ShiftCode, '-', '.'),2) = PARSENAME(REPLACE(CUR.Msh_ShiftCode, '-', '.'),2)			
	                                                AND PRV.Msh_CompanyCode = CUR.Msh_CompanyCode
	                                                AND PRV.Msh_FlexShift = 1
	                                                AND PRV.Msh_RecordStatus = 'A'
                                                LEFT JOIN {0}..M_Shift NXT ON NXT.Msh_ShiftCode = (SELECT MIN(Msh_ShiftCode) FROM {0}..M_Shift 
										                                                WHERE Msh_ShiftCode > CUR.Msh_ShiftCode AND Msh_CompanyCode = CUR.Msh_CompanyCode
										                                                AND Msh_FlexShift = 1
										                                                AND Msh_RecordStatus = 'A'  
										                                                AND PARSENAME(REPLACE(NXT.Msh_ShiftCode, '-', '.'),2) = PARSENAME(REPLACE(CUR.Msh_ShiftCode, '-', '.'),2))				
	                                                AND PARSENAME(REPLACE(NXT.Msh_ShiftCode, '-', '.'),2) = PARSENAME(REPLACE(CUR.Msh_ShiftCode, '-', '.'),2)			
                                                    AND NXT.Msh_CompanyCode = CUR.Msh_CompanyCode
	                                                AND NXT.Msh_FlexShift = 1
	                                                AND NXT.Msh_RecordStatus = 'A'	
                                                WHERE CUR.Msh_CompanyCode = '{1}'
	                                                AND CUR.Msh_FlexShift = 1
	                                                AND CUR.Msh_RecordStatus = 'A' ";

            if (Globals.isAutoChangeShift == true && Globals.ShiftPocket == "OUT1")
            {
                strShiftTable = string.Format(strCreateFlexShiftTable
                                                   , Globals.CentralProfile
                                                   , strCompanyCode);
                dal.ExecuteNonQuery(strShiftTable);
            }

            string strTrailPrevious = "";

            //update logs in time register
            string strUpdateTemplate = @"UPDATE {0}..{1} SET Ttm_ActIn_01 = '{4}', Ttm_ActOut_01 = '{5}', Ttm_ActIn_02 = '{6}', Ttm_ActOut_02 = '{7}', Ttm_ActIn_03 = '{8}', Ttm_ActOut_03 = '{9}', Ttm_ActIn_04 = '{10}', Ttm_ActOut_04 = '{11}', Ttm_ActIn_05 = '{12}', Ttm_ActOut_05 = '{13}', Ttm_ActIn_06 = '{14}', Ttm_ActOut_06 = '{15}', Ttm_ActIn_07 = '{16}', Ttm_ActOut_07 = '{17}', Ttm_ActIn_08 = '{18}', Ttm_ActOut_08 = '{19}', Ttm_ActIn_09 = '{20}', Ttm_ActOut_09 = '{21}', Ttm_ActIn_10 = '{22}', Ttm_ActOut_10 = '{23}', Ttm_ActIn_11 = '{24}', Ttm_ActOut_11 = '{25}', Ttm_ActIn_12 = '{26}', Ttm_ActOut_12 = '{27}', Ttm_ActIn1 = '{28}', Ttm_ActOut1 = '{29}', Ttm_ActIn2 = '{30}', Ttm_ActOut2 = '{31}', Usr_Login = 'LOGUPLOADING', Ludatetime = GETDATE() WHERE Ttm_IDNo = '{2}' AND Ttm_Date = '{3}' ";

            // create trail records
            string strUpdateTemplate2 = @"INSERT INTO {0}..T_EmpTimeRegisterLogMisc
                                        SELECT Ttm_IDNo
                                        , Ttm_Date
                                        , Ttr_DayCode
                                        , Ttr_RestDayFlag
                                        , Ttr_HolidayFlag
                                        , Ttr_ShiftCode
                                        , Ttm_ActIn_01
                                        , Ttm_ActOut_01
                                        , Ttm_ActIn_02
                                        , Ttm_ActOut_02
                                        , Ttm_ActIn_03
                                        , Ttm_ActOut_03
                                        , Ttm_ActIn_04
                                        , Ttm_ActOut_04
                                        , Ttm_ActIn_05
                                        , Ttm_ActOut_05
                                        , Ttm_ActIn_06
                                        , Ttm_ActOut_06
                                        , Ttm_ActIn_07
                                        , Ttm_ActOut_07
                                        , Ttm_ActIn_08
                                        , Ttm_ActOut_08
                                        , Ttm_ActIn_09
                                        , Ttm_ActOut_09
                                        , Ttm_ActIn_10
                                        , Ttm_ActOut_10
                                        , Ttm_ActIn_11
                                        , Ttm_ActOut_11
                                        , Ttm_ActIn_12
                                        , Ttm_ActOut_12
                                        , Ttr_SkipService
                                        , Ttr_AssumedFlag
                                        , Ttr_Amnesty
                                        , Ttr_Remarks = ''
                                        , Ttm_DocumentBatchNo =''
                                        , Usr_Login = T_EmpTimeRegisterMisc.Usr_Login
                                        , Ludatetime = T_EmpTimeRegisterMisc.Ludatetime
                                        FROM {0}..{1}
                                        INNER JOIN {0}..{2} ON Ttm_IDNo = Ttr_IDNo 
                                        AND Ttm_Date = Ttr_Date 
                                        LEFT JOIN #ShiftCodeMaster Temp ON (( Ttm_ActOut_01 <= [CUR OUT1] 
                                        AND (Ttm_ActOut_01 > [PRV OUT1] OR [PRV OUT1] IS NULL))
                                        OR (Ttm_ActOut_01 > [CUR OUT1]  AND [NXT OUT1] IS NULL))
                                        AND ISNULL(PARSENAME(REPLACE(Ttr_ShiftCode, '-', '.'),2),Ttr_ShiftCode) = FlexCode
                                        LEFT JOIN {5}..M_Shift Flex ON Flex.Msh_ShiftCode = CASE WHEN Ttm_ActOut_01 = '0000' THEN Ttr_ShiftCode
												                                        WHEN Temp.[NXT Shift] IS NULL AND Ttm_ActOut_01 > [CUR OUT1]  THEN [CUR Shift]
												                                        WHEN Temp.[NXT Shift] IS NOT NULL AND Ttm_ActOut_01 <= [CUR OUT1]  AND Temp.[PRV Shift] IS NULL THEN [CUR Shift] 
                                                                                        WHEN Temp.[NXT Shift] IS NOT NULL AND Ttm_ActOut_01 = [CUR OUT1] AND Temp.[PRV Shift] IS NOT NULL THEN [CUR Shift] 
											                                        ELSE 
												                                        Temp.[PRV Shift]
											                                        END
				                                        AND Flex.Msh_CompanyCode = '{6}'
                                        WHERE CHARINDEX('-',Ttr_ShiftCode) > 0 
                                            AND Ttm_ActOut_01 <> '0000'
                                            AND Ttr_IDNo = '{3}' AND Ttr_Date = '{4}' 

                                        UNION ALL

                                        SELECT Ttm_IDNo
                                        , Ttm_Date
                                        , Ttr_DayCode
                                        , Ttr_RestDayFlag
                                        , Ttr_HolidayFlag
                                        , COALESCE(Flex.Msh_ShiftCode, Ttr_ShiftCode)
                                        , Ttm_ActIn_01
                                        , Ttm_ActOut_01
                                        , Ttm_ActIn_02
                                        , Ttm_ActOut_02
                                        , Ttm_ActIn_03
                                        , Ttm_ActOut_03
                                        , Ttm_ActIn_04
                                        , Ttm_ActOut_04
                                        , Ttm_ActIn_05
                                        , Ttm_ActOut_05
                                        , Ttm_ActIn_06
                                        , Ttm_ActOut_06
                                        , Ttm_ActIn_07
                                        , Ttm_ActOut_07
                                        , Ttm_ActIn_08
                                        , Ttm_ActOut_08
                                        , Ttm_ActIn_09
                                        , Ttm_ActOut_09
                                        , Ttm_ActIn_10
                                        , Ttm_ActOut_10
                                        , Ttm_ActIn_11
                                        , Ttm_ActOut_11
                                        , Ttm_ActIn_12
                                        , Ttm_ActOut_12
                                        , Ttr_SkipService
                                        , Ttr_AssumedFlag
                                        , Ttr_Amnesty
                                        , Ttr_Remarks = 'LOG SERVICE'
                                        , Ttm_DocumentBatchNo =''
                                        , Usr_Login ='LOGUPLOADING'
                                        , Ludatetime = GETDATE()
                                        FROM {0}..{1}
                                        INNER JOIN {0}..{2} ON Ttm_IDNo = Ttr_IDNo 
                                        AND Ttm_Date = Ttr_Date 
                                        LEFT JOIN #ShiftCodeMaster Temp ON (( Ttm_ActOut_01 <= [CUR OUT1] 
                                        AND (Ttm_ActOut_01 > [PRV OUT1] OR [PRV OUT1] IS NULL))
                                        OR (Ttm_ActOut_01 > [CUR OUT1]  AND [NXT OUT1] IS NULL))
                                        AND ISNULL(PARSENAME(REPLACE(Ttr_ShiftCode, '-', '.'),2),Ttr_ShiftCode) = FlexCode
                                        LEFT JOIN {5}..M_Shift Flex ON Flex.Msh_ShiftCode = CASE WHEN Ttm_ActOut_01 = '0000' THEN Ttr_ShiftCode
												                                        WHEN Temp.[NXT Shift] IS NULL AND Ttm_ActOut_01 > [CUR OUT1]  THEN [CUR Shift]
												                                        WHEN Temp.[NXT Shift] IS NOT NULL AND Ttm_ActOut_01 <= [CUR OUT1]  AND Temp.[PRV Shift] IS NULL THEN [CUR Shift] 
                                                                                        WHEN Temp.[NXT Shift] IS NOT NULL AND Ttm_ActOut_01 = [CUR OUT1] AND Temp.[PRV Shift] IS NOT NULL THEN [CUR Shift] 
											                                        ELSE 
												                                        Temp.[PRV Shift]
											                                        END
				                                        AND Flex.Msh_CompanyCode = '{6}'
                                        WHERE CHARINDEX('-',Ttr_ShiftCode) > 0 
                                            AND Ttm_ActOut_01 <> '0000'
                                            AND Ttr_IDNo = '{3}' AND Ttr_Date = '{4}'";

            //update shift code of Flex Schedules
            string strUpdateTemplate3 = @"UPDATE {0}..{1}
                                        SET Ttr_ShiftCode = COALESCE(Flex.Msh_ShiftCode, Ttr_ShiftCode), Usr_Login = 'LOGUPLOADING', Ludatetime = GETDATE() 
                                        FROM {0}..{1}
                                        INNER JOIN {0}..{2} ON Ttm_IDNo = Ttr_IDNo 
	                                        AND Ttm_Date = Ttr_Date 
                                        LEFT JOIN #ShiftCodeMaster Temp ON (( Ttm_ActOut_01 <= [CUR OUT1] 
	                                           AND (Ttm_ActOut_01 > [PRV OUT1] OR [PRV OUT1] IS NULL))
	                                           OR (Ttm_ActOut_01 > [CUR OUT1]  AND [NXT OUT1] IS NULL))
	                                        AND ISNULL(PARSENAME(REPLACE(Ttr_ShiftCode, '-', '.'),2),Ttr_ShiftCode) = FlexCode
                                        LEFT JOIN {5}..M_Shift Flex ON Flex.Msh_ShiftCode = CASE WHEN Ttm_ActOut_01 = '0000' THEN Ttr_ShiftCode
																	                                        WHEN Temp.[NXT Shift] IS NULL AND Ttm_ActOut_01 > [CUR OUT1]  THEN [CUR Shift]
																	                                        WHEN Temp.[NXT Shift] IS NOT NULL AND Ttm_ActOut_01 <= [CUR OUT1]  AND Temp.[PRV Shift] IS NULL THEN [CUR Shift] 
                                                                                                            WHEN Temp.[NXT Shift] IS NOT NULL AND Ttm_ActOut_01 = [CUR OUT1] AND Temp.[PRV Shift] IS NOT NULL THEN [CUR Shift] 
																                                        ELSE 
																	                                        Temp.[PRV Shift]
																                                        END
									                                        AND Flex.Msh_CompanyCode = '{6}'
                                        WHERE CHARINDEX('-',Ttr_ShiftCode) > 0 
	                                        AND Ttm_ActOut_01 <> '0000'
                                            AND Ttr_IDNo = '{3}' AND Ttr_Date = '{4}'";

            for (int x = 0; x < drArrLogLedger.Length; x++)
            {
                if (bIsInHistTable == true)
                {
                    #region Create Trail Records
                    #region Query
                    strTrailPrevious = string.Format(@" 
                        DECLARE @AffectedPayPeriod VARCHAR(7) = (Select TOP(1) Tps_PayCycle 
                                                                From {0}..T_PaySchedule 
                                                                Where Tps_CycleIndicator = 'P'
                                                                    And '{2}' BETWEEN Tps_StartCycle AND Tps_EndCycle
                                                                ORDER BY Tps_StartCycle DESC)
                        DECLARE @AdjustPayPeriod VARCHAR(7) = (Select Tps_PayCycle 
									                           From {0}..T_PaySchedule 
									                           Where Tps_CycleIndicator='C' 
										                        and Tps_RecordStatus = 'A')

                        --INSERT INTO LOG LEDGER TRAIL
                        INSERT INTO {0}..T_EmpTimeRegisterTrl
                        (
	                            Ttr_IDNo
                                ,Ttr_Date
                                ,Ttr_AdjPayCycle
                                ,Ttr_PayCycle
                                ,Ttr_DayCode
                                ,Ttr_ShiftCode
                                ,Ttr_HolidayFlag
                                ,Ttr_RestDayFlag
                                ,Ttr_ActIn_1
                                ,Ttr_ActOut_1
                                ,Ttr_ActIn_2
                                ,Ttr_ActOut_2
                                ,Ttr_WFPayLVCode
                                ,Ttr_WFPayLVHr
                                ,Ttr_PayLVMin
                                ,Ttr_ExcLVMin
                                ,Ttr_WFNoPayLVCode
                                ,Ttr_WFNoPayLVHr
                                ,Ttr_NoPayLVMin
                                ,Ttr_WFOTAdvHr
                                ,Ttr_WFOTPostHr
                                ,Ttr_OTMin
                                ,Ttr_CompOTMin
                                ,Ttr_OffsetOTMin
                                ,Ttr_WFTimeMod
                                ,Ttr_WFFlexTime
                                ,Ttr_Amnesty
                                ,Ttr_SkipService
                                ,Ttr_SkipServiceBy
                                ,Ttr_SkipServiceDate
                                ,Ttr_AssumedFlag
                                ,Ttr_AssumedBy
                                ,Ttr_AssumedDate
                                ,Ttr_AssumedPost
                                ,Ttr_ConvIn_1Min
                                ,Ttr_ConvOut_1Min
                                ,Ttr_ConvIn_2Min
                                ,Ttr_ConvOut_2Min
                                ,Ttr_CompIn_1Min
                                ,Ttr_CompOut_1Min
                                ,Ttr_CompIn_2Min
                                ,Ttr_CompOut_2Min
                                ,Ttr_CompAdvOTMin
                                ,Ttr_ShiftIn_1Min
                                ,Ttr_ShiftOut_1Min
                                ,Ttr_ShiftIn_2Min
                                ,Ttr_ShiftOut_2Min
                                ,Ttr_ShiftMin
                                ,Ttr_ScheduleType
                                ,Ttr_ActLT1Min
                                ,Ttr_ActLT2Min
                                ,Ttr_CompLT1Min
                                ,Ttr_CompLT2Min
                                ,Ttr_ActUT1Min
                                ,Ttr_ActUT2Min
                                ,Ttr_CompUT1Min
                                ,Ttr_CompUT2Min
                                ,Ttr_InitialABSMin
                                ,Ttr_CompABSMin
                                ,Ttr_CompREGMin
                                ,Ttr_CompWorkMin
                                ,Ttr_CompNDMin
                                ,Ttr_CompNDOTMin
                                ,Ttr_PrvDayWorkMin
                                ,Ttr_PrvDayHolRef
                                ,Ttr_PDHOLHour
                                ,Ttr_PDRESTLEGHOLDay
                                ,Ttr_WorkDay
                                ,Ttr_EXPHour
                                ,Ttr_ABSHour
                                ,Ttr_REGHour
                                ,Ttr_OTHour
                                ,Ttr_NDHour
                                ,Ttr_NDOTHour
                                ,Ttr_LVHour
                                ,Ttr_PaidBreakHour
                                ,Ttr_OBHour
                                ,Ttr_RegPlusHour
                                ,Ttr_TBAmt01
                                ,Ttr_TBAmt02
                                ,Ttr_TBAmt03
                                ,Ttr_TBAmt04
                                ,Ttr_TBAmt05
                                ,Ttr_TBAmt06
                                ,Ttr_TBAmt07
                                ,Ttr_TBAmt08
                                ,Ttr_TBAmt09
                                ,Ttr_TBAmt10
                                ,Ttr_TBAmt11
                                ,Ttr_TBAmt12
                                ,Ttr_WorkLocationCode
                                ,Ttr_CalendarGroup
                                ,Ttr_PremiumGrpCode
                                ,Ttr_PayrollGroup
                                ,Ttr_CostcenterCode
                                ,Ttr_EmploymentStatusCode
                                ,Ttr_PayrollType
                                ,Ttr_Grade
                                ,Usr_Login
                                ,Ludatetime
                        )
                        SELECT A.Ttr_IDNo
	                        ,A.Ttr_Date
                            ,@AdjustPayPeriod
                            ,A.Ttr_PayCycle
                            ,A.Ttr_DayCode
                            ,A.Ttr_ShiftCode
                            ,A.Ttr_HolidayFlag
                            ,A.Ttr_RestDayFlag
                            ,A.Ttr_ActIn_1
                            ,A.Ttr_ActOut_1
                            ,A.Ttr_ActIn_2
                            ,A.Ttr_ActOut_2
                            ,A.Ttr_WFPayLVCode
                            ,A.Ttr_WFPayLVHr
                            ,A.Ttr_PayLVMin
                            ,A.Ttr_ExcLVMin
                            ,A.Ttr_WFNoPayLVCode
                            ,A.Ttr_WFNoPayLVHr
                            ,A.Ttr_NoPayLVMin
                            ,A.Ttr_WFOTAdvHr
                            ,A.Ttr_WFOTPostHr
                            ,A.Ttr_OTMin
                            ,A.Ttr_CompOTMin
                            ,A.Ttr_OffsetOTMin
                            ,A.Ttr_WFTimeMod
                            ,A.Ttr_WFFlexTime
                            ,A.Ttr_Amnesty
                            ,A.Ttr_SkipService
                            ,A.Ttr_SkipServiceBy
                            ,A.Ttr_SkipServiceDate
                            ,A.Ttr_AssumedFlag
                            ,A.Ttr_AssumedBy
                            ,A.Ttr_AssumedDate
                            ,A.Ttr_AssumedPost
                            ,A.Ttr_ConvIn_1Min
                            ,A.Ttr_ConvOut_1Min
                            ,A.Ttr_ConvIn_2Min
                            ,A.Ttr_ConvOut_2Min
                            ,A.Ttr_CompIn_1Min
                            ,A.Ttr_CompOut_1Min
                            ,A.Ttr_CompIn_2Min
                            ,A.Ttr_CompOut_2Min
                            ,A.Ttr_CompAdvOTMin
                            ,A.Ttr_ShiftIn_1Min
                            ,A.Ttr_ShiftOut_1Min
                            ,A.Ttr_ShiftIn_2Min
                            ,A.Ttr_ShiftOut_2Min
                            ,A.Ttr_ShiftMin
                            ,A.Ttr_ScheduleType
                            ,A.Ttr_ActLT1Min
                            ,A.Ttr_ActLT2Min
                            ,A.Ttr_CompLT1Min
                            ,A.Ttr_CompLT2Min
                            ,A.Ttr_ActUT1Min
                            ,A.Ttr_ActUT2Min
                            ,A.Ttr_CompUT1Min
                            ,A.Ttr_CompUT2Min
                            ,A.Ttr_InitialABSMin
                            ,A.Ttr_CompABSMin
                            ,A.Ttr_CompREGMin
                            ,A.Ttr_CompWorkMin
                            ,A.Ttr_CompNDMin
                            ,A.Ttr_CompNDOTMin
                            ,A.Ttr_PrvDayWorkMin
                            ,A.Ttr_PrvDayHolRef
                            ,A.Ttr_PDHOLHour
                            ,A.Ttr_PDRESTLEGHOLDay
                            ,A.Ttr_WorkDay
                            ,A.Ttr_EXPHour
                            ,A.Ttr_ABSHour
                            ,A.Ttr_REGHour
                            ,A.Ttr_OTHour
                            ,A.Ttr_NDHour
                            ,A.Ttr_NDOTHour
                            ,A.Ttr_LVHour
                            ,A.Ttr_PaidBreakHour
                            ,A.Ttr_OBHour
                            ,A.Ttr_RegPlusHour
                            ,A.Ttr_TBAmt01
                            ,A.Ttr_TBAmt02
                            ,A.Ttr_TBAmt03
                            ,A.Ttr_TBAmt04
                            ,A.Ttr_TBAmt05
                            ,A.Ttr_TBAmt06
                            ,A.Ttr_TBAmt07
                            ,A.Ttr_TBAmt08
                            ,A.Ttr_TBAmt09
                            ,A.Ttr_TBAmt10
                            ,A.Ttr_TBAmt11
                            ,A.Ttr_TBAmt12
                            ,A.Ttr_WorkLocationCode
                            ,A.Ttr_CalendarGroup
                            ,A.Ttr_PremiumGrpCode
                            ,A.Ttr_PayrollGroup
                            ,A.Ttr_CostcenterCode
                            ,A.Ttr_EmploymentStatusCode
                            ,A.Ttr_PayrollType
                            ,A.Ttr_Grade
                            ,A.Usr_Login
                            ,A.Ludatetime
                        FROM {0}..T_EmpTimeRegisterHst A
                        LEFT JOIN {0}..T_EmpTimeRegisterTrl B
                        ON A.Ttr_IDNo = B.Ttr_IDNo
	                        AND A.Ttr_Date = B.Ttr_Date
                            AND A.Ttr_PayCycle = B.Ttr_PayCycle
	                        AND B.Ttr_AdjPayCycle = @AdjustPayPeriod
                        WHERE B.Ttr_IDNo IS NULL
                            AND A.Ttr_PayCycle = @AffectedPayPeriod
                            AND A.Ttr_IDNo = '{1}'

                        --INSERT INTO LOG LEDGER EXT TRAIL
                        INSERT INTO {0}..T_EmpTimeRegisterMiscTrl
                        (       Ttm_IDNo
                                ,Ttm_Date
                                ,Ttm_AdjPayCycle
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
                                ,Ttm_Result
                                ,Ttm_ActIn1
                                ,Ttm_ActIn2
                                ,Ttm_ActOut1
                                ,Ttm_ActOut2
                                ,Usr_Login
                                ,Ludatetime
                        )
                        SELECT A.Ttm_IDNo
                                ,A.Ttm_Date
                                ,@AdjustPayPeriod
                                ,A.Ttm_PayCycle
                                ,A.Ttm_ActIn_01
                                ,A.Ttm_ActOut_01
                                ,A.Ttm_ActIn_02
                                ,A.Ttm_ActOut_02
                                ,A.Ttm_ActIn_03
                                ,A.Ttm_ActOut_03
                                ,A.Ttm_ActIn_04
                                ,A.Ttm_ActOut_04
                                ,A.Ttm_ActIn_05
                                ,A.Ttm_ActOut_05
                                ,A.Ttm_ActIn_06
                                ,A.Ttm_ActOut_06
                                ,A.Ttm_ActIn_07
                                ,A.Ttm_ActOut_07
                                ,A.Ttm_ActIn_08
                                ,A.Ttm_ActOut_08
                                ,A.Ttm_ActIn_09
                                ,A.Ttm_ActOut_09
                                ,A.Ttm_ActIn_10
                                ,A.Ttm_ActOut_10
                                ,A.Ttm_ActIn_11
                                ,A.Ttm_ActOut_11
                                ,A.Ttm_ActIn_12
                                ,A.Ttm_ActOut_12
                                ,A.Ttm_Result
                                ,A.Ttm_ActIn1
                                ,A.Ttm_ActIn2
                                ,A.Ttm_ActOut1
                                ,A.Ttm_ActOut2
                                ,A.Usr_Login
                                ,A.Ludatetime
                        FROM {0}..T_EmpTimeRegisterMiscHst A
                        LEFT JOIN {0}..T_EmpTimeRegisterMiscTrl B
                        ON A.Ttm_IDNo = B.Ttm_IDNo
	                        AND A.Ttm_Date = B.Ttm_Date
                            AND A.Ttm_PayCycle = B.Ttm_PayCycle
	                        AND B.Ttm_AdjPayCycle = @AdjustPayPeriod
                        WHERE B.Ttm_IDNo IS NULL
                            AND A.Ttm_PayCycle = @AffectedPayPeriod
                            AND A.Ttm_IDNo = '{1}'

                        --INSERT INTO PAYROLL TRANSACTION TRAIL
                        INSERT INTO {0}..T_EmpPayTranHdrTrl
                        (
	                        Tph_IDNo
                            ,Tph_AdjPayCycle
                            ,Tph_PayCycle
                            ,Tph_LTHr
                            ,Tph_UTHr
                            ,Tph_UPLVHr
                            ,Tph_ABSLEGHOLHr
                            ,Tph_ABSSPLHOLHr
                            ,Tph_ABSCOMPHOLHr
                            ,Tph_ABSPSDHr
                            ,Tph_ABSOTHHOLHr
                            ,Tph_WDABSHr
                            ,Tph_LTUTMaxHr
                            ,Tph_ABSHr
                            ,Tph_REGHr
                            ,Tph_PDLVHr
                            ,Tph_PDLEGHOLHr
                            ,Tph_PDSPLHOLHr
                            ,Tph_PDCOMPHOLHr
                            ,Tph_PDPSDHr
                            ,Tph_PDOTHHOLHr
                            ,Tph_PDRESTLEGHOLHr
                            ,Tph_REGOTHr
                            ,Tph_REGNDHr
                            ,Tph_REGNDOTHr
                            ,Tph_RESTHr
                            ,Tph_RESTOTHr
                            ,Tph_RESTNDHr
                            ,Tph_RESTNDOTHr
                            ,Tph_LEGHOLHr
                            ,Tph_LEGHOLOTHr
                            ,Tph_LEGHOLNDHr
                            ,Tph_LEGHOLNDOTHr
                            ,Tph_SPLHOLHr
                            ,Tph_SPLHOLOTHr
                            ,Tph_SPLHOLNDHr
                            ,Tph_SPLHOLNDOTHr
                            ,Tph_PSDHr
                            ,Tph_PSDOTHr
                            ,Tph_PSDNDHr
                            ,Tph_PSDNDOTHr
                            ,Tph_COMPHOLHr
                            ,Tph_COMPHOLOTHr
                            ,Tph_COMPHOLNDHr
                            ,Tph_COMPHOLNDOTHr
                            ,Tph_RESTLEGHOLHr
                            ,Tph_RESTLEGHOLOTHr
                            ,Tph_RESTLEGHOLNDHr
                            ,Tph_RESTLEGHOLNDOTHr
                            ,Tph_RESTSPLHOLHr
                            ,Tph_RESTSPLHOLOTHr
                            ,Tph_RESTSPLHOLNDHr
                            ,Tph_RESTSPLHOLNDOTHr
                            ,Tph_RESTCOMPHOLHr
                            ,Tph_RESTCOMPHOLOTHr
                            ,Tph_RESTCOMPHOLNDHr
                            ,Tph_RESTCOMPHOLNDOTHr
                            ,Tph_RESTPSDHr
                            ,Tph_RESTPSDOTHr
                            ,Tph_RESTPSDNDHr
                            ,Tph_RESTPSDNDOTHr
                            ,Tph_SRGAdjHr
                            ,Tph_SRGAdjAmt
                            ,Tph_SOTAdjHr
                            ,Tph_SOTAdjAmt
                            ,Tph_SHOLAdjHr
                            ,Tph_SHOLAdjAmt
                            ,Tph_SNDAdjHr
                            ,Tph_SNDAdjAmt
                            ,Tph_SLVAdjHr
                            ,Tph_SLVAdjAmt
                            ,Tph_MRGAdjHr
                            ,Tph_MRGAdjAmt
                            ,Tph_MOTAdjHr
                            ,Tph_MOTAdjAmt
                            ,Tph_MHOLAdjHr
                            ,Tph_MHOLAdjAmt
                            ,Tph_MNDAdjHr
                            ,Tph_MNDAdjAmt
                            ,Tph_TotalAdjAmt
                            ,Tph_TaxableIncomeAmt
                            ,Tph_NontaxableIncomeAmt
                            ,Tph_WorkDay
                            ,Tph_PayrollType
                            ,Tph_PremiumGrpCode
                            ,Tph_RetainUserEntry
                            ,Usr_Login
                            ,Ludatetime
                        )
                        SELECT A.Tph_IDNo
                                ,@AdjustPayPeriod
                                ,A.Tph_PayCycle
                                ,A.Tph_LTHr
                                ,A.Tph_UTHr
                                ,A.Tph_UPLVHr
                                ,A.Tph_ABSLEGHOLHr
                                ,A.Tph_ABSSPLHOLHr
                                ,A.Tph_ABSCOMPHOLHr
                                ,A.Tph_ABSPSDHr
                                ,A.Tph_ABSOTHHOLHr
                                ,A.Tph_WDABSHr
                                ,A.Tph_LTUTMaxHr
                                ,A.Tph_ABSHr
                                ,A.Tph_REGHr
                                ,A.Tph_PDLVHr
                                ,A.Tph_PDLEGHOLHr
                                ,A.Tph_PDSPLHOLHr
                                ,A.Tph_PDCOMPHOLHr
                                ,A.Tph_PDPSDHr
                                ,A.Tph_PDOTHHOLHr
                                ,A.Tph_PDRESTLEGHOLHr
                                ,A.Tph_REGOTHr
                                ,A.Tph_REGNDHr
                                ,A.Tph_REGNDOTHr
                                ,A.Tph_RESTHr
                                ,A.Tph_RESTOTHr
                                ,A.Tph_RESTNDHr
                                ,A.Tph_RESTNDOTHr
                                ,A.Tph_LEGHOLHr
                                ,A.Tph_LEGHOLOTHr
                                ,A.Tph_LEGHOLNDHr
                                ,A.Tph_LEGHOLNDOTHr
                                ,A.Tph_SPLHOLHr
                                ,A.Tph_SPLHOLOTHr
                                ,A.Tph_SPLHOLNDHr
                                ,A.Tph_SPLHOLNDOTHr
                                ,A.Tph_PSDHr
                                ,A.Tph_PSDOTHr
                                ,A.Tph_PSDNDHr
                                ,A.Tph_PSDNDOTHr
                                ,A.Tph_COMPHOLHr
                                ,A.Tph_COMPHOLOTHr
                                ,A.Tph_COMPHOLNDHr
                                ,A.Tph_COMPHOLNDOTHr
                                ,A.Tph_RESTLEGHOLHr
                                ,A.Tph_RESTLEGHOLOTHr
                                ,A.Tph_RESTLEGHOLNDHr
                                ,A.Tph_RESTLEGHOLNDOTHr
                                ,A.Tph_RESTSPLHOLHr
                                ,A.Tph_RESTSPLHOLOTHr
                                ,A.Tph_RESTSPLHOLNDHr
                                ,A.Tph_RESTSPLHOLNDOTHr
                                ,A.Tph_RESTCOMPHOLHr
                                ,A.Tph_RESTCOMPHOLOTHr
                                ,A.Tph_RESTCOMPHOLNDHr
                                ,A.Tph_RESTCOMPHOLNDOTHr
                                ,A.Tph_RESTPSDHr
                                ,A.Tph_RESTPSDOTHr
                                ,A.Tph_RESTPSDNDHr
                                ,A.Tph_RESTPSDNDOTHr
                                ,A.Tph_SRGAdjHr
                                ,A.Tph_SRGAdjAmt
                                ,A.Tph_SOTAdjHr
                                ,A.Tph_SOTAdjAmt
                                ,A.Tph_SHOLAdjHr
                                ,A.Tph_SHOLAdjAmt
                                ,A.Tph_SNDAdjHr
                                ,A.Tph_SNDAdjAmt
                                ,A.Tph_SLVAdjHr
                                ,A.Tph_SLVAdjAmt
                                ,A.Tph_MRGAdjHr
                                ,A.Tph_MRGAdjAmt
                                ,A.Tph_MOTAdjHr
                                ,A.Tph_MOTAdjAmt
                                ,A.Tph_MHOLAdjHr
                                ,A.Tph_MHOLAdjAmt
                                ,A.Tph_MNDAdjHr
                                ,A.Tph_MNDAdjAmt
                                ,A.Tph_TotalAdjAmt
                                ,A.Tph_TaxableIncomeAmt
                                ,A.Tph_NontaxableIncomeAmt
                                ,A.Tph_WorkDay
                                ,A.Tph_PayrollType
                                ,A.Tph_PremiumGrpCode
                                ,A.Tph_RetainUserEntry
                                ,A.Usr_Login
                                ,A.Ludatetime
                        FROM {0}..T_EmpPayTranHdrHst A
                        LEFT JOIN {0}..T_EmpPayTranHdrTrl B
                        ON A.Tph_IDNo = B.Tph_IDNo
	                        AND A.Tph_PayCycle = B.Tph_PayCycle
                            AND B.Tph_AdjPayCycle = @AdjustPayPeriod
                        WHERE B.Tph_IDNo IS NULL
                            AND A.Tph_PayCycle = @AffectedPayPeriod
                            AND A.Tph_IDNo = '{1}'

                        --INSERT INTO PAYROLL TRANSACTION TRAIL DETAIL
                        INSERT INTO {0}..T_EmpPayTranDtlTrl
                        (
	                         Tpd_IDNo
                            ,Tpd_AdjPayCycle
                            ,Tpd_PayCycle
                            ,Tpd_Date
                            ,Tpd_LTHr
                            ,Tpd_UTHr
                            ,Tpd_UPLVHr
                            ,Tpd_ABSLEGHOLHr
                            ,Tpd_ABSSPLHOLHr
                            ,Tpd_ABSCOMPHOLHr
                            ,Tpd_ABSPSDHr
                            ,Tpd_ABSOTHHOLHr
                            ,Tpd_WDABSHr
                            ,Tpd_LTUTMaxHr
                            ,Tpd_ABSHr
                            ,Tpd_REGHr
                            ,Tpd_PDLVHr
                            ,Tpd_PDLEGHOLHr
                            ,Tpd_PDSPLHOLHr
                            ,Tpd_PDCOMPHOLHr
                            ,Tpd_PDPSDHr
                            ,Tpd_PDOTHHOLHr
                            ,Tpd_PDRESTLEGHOLHr
                            ,Tpd_REGOTHr
                            ,Tpd_REGNDHr
                            ,Tpd_REGNDOTHr
                            ,Tpd_RESTHr
                            ,Tpd_RESTOTHr
                            ,Tpd_RESTNDHr
                            ,Tpd_RESTNDOTHr
                            ,Tpd_LEGHOLHr
                            ,Tpd_LEGHOLOTHr
                            ,Tpd_LEGHOLNDHr
                            ,Tpd_LEGHOLNDOTHr
                            ,Tpd_SPLHOLHr
                            ,Tpd_SPLHOLOTHr
                            ,Tpd_SPLHOLNDHr
                            ,Tpd_SPLHOLNDOTHr
                            ,Tpd_PSDHr
                            ,Tpd_PSDOTHr
                            ,Tpd_PSDNDHr
                            ,Tpd_PSDNDOTHr
                            ,Tpd_COMPHOLHr
                            ,Tpd_COMPHOLOTHr
                            ,Tpd_COMPHOLNDHr
                            ,Tpd_COMPHOLNDOTHr
                            ,Tpd_RESTLEGHOLHr
                            ,Tpd_RESTLEGHOLOTHr
                            ,Tpd_RESTLEGHOLNDHr
                            ,Tpd_RESTLEGHOLNDOTHr
                            ,Tpd_RESTSPLHOLHr
                            ,Tpd_RESTSPLHOLOTHr
                            ,Tpd_RESTSPLHOLNDHr
                            ,Tpd_RESTSPLHOLNDOTHr
                            ,Tpd_RESTCOMPHOLHr
                            ,Tpd_RESTCOMPHOLOTHr
                            ,Tpd_RESTCOMPHOLNDHr
                            ,Tpd_RESTCOMPHOLNDOTHr
                            ,Tpd_RESTPSDHr
                            ,Tpd_RESTPSDOTHr
                            ,Tpd_RESTPSDNDHr
                            ,Tpd_RESTPSDNDOTHr
                            ,Tpd_WorkDay
                            ,Tpd_PayrollType
                            ,Tpd_PremiumGrpCode
                            ,Usr_Login
                            ,Ludatetime
                        )
                        SELECT A.Tpd_IDNo
                            ,@AdjustPayPeriod
                            ,A.Tpd_PayCycle
                            ,A.Tpd_Date
                            ,A.Tpd_LTHr
                            ,A.Tpd_UTHr
                            ,A.Tpd_UPLVHr
                            ,A.Tpd_ABSLEGHOLHr
                            ,A.Tpd_ABSSPLHOLHr
                            ,A.Tpd_ABSCOMPHOLHr
                            ,A.Tpd_ABSPSDHr
                            ,A.Tpd_ABSOTHHOLHr
                            ,A.Tpd_WDABSHr
                            ,A.Tpd_LTUTMaxHr
                            ,A.Tpd_ABSHr
                            ,A.Tpd_REGHr
                            ,A.Tpd_PDLVHr
                            ,A.Tpd_PDLEGHOLHr
                            ,A.Tpd_PDSPLHOLHr
                            ,A.Tpd_PDCOMPHOLHr
                            ,A.Tpd_PDPSDHr
                            ,A.Tpd_PDOTHHOLHr
                            ,A.Tpd_PDRESTLEGHOLHr
                            ,A.Tpd_REGOTHr
                            ,A.Tpd_REGNDHr
                            ,A.Tpd_REGNDOTHr
                            ,A.Tpd_RESTHr
                            ,A.Tpd_RESTOTHr
                            ,A.Tpd_RESTNDHr
                            ,A.Tpd_RESTNDOTHr
                            ,A.Tpd_LEGHOLHr
                            ,A.Tpd_LEGHOLOTHr
                            ,A.Tpd_LEGHOLNDHr
                            ,A.Tpd_LEGHOLNDOTHr
                            ,A.Tpd_SPLHOLHr
                            ,A.Tpd_SPLHOLOTHr
                            ,A.Tpd_SPLHOLNDHr
                            ,A.Tpd_SPLHOLNDOTHr
                            ,A.Tpd_PSDHr
                            ,A.Tpd_PSDOTHr
                            ,A.Tpd_PSDNDHr
                            ,A.Tpd_PSDNDOTHr
                            ,A.Tpd_COMPHOLHr
                            ,A.Tpd_COMPHOLOTHr
                            ,A.Tpd_COMPHOLNDHr
                            ,A.Tpd_COMPHOLNDOTHr
                            ,A.Tpd_RESTLEGHOLHr
                            ,A.Tpd_RESTLEGHOLOTHr
                            ,A.Tpd_RESTLEGHOLNDHr
                            ,A.Tpd_RESTLEGHOLNDOTHr
                            ,A.Tpd_RESTSPLHOLHr
                            ,A.Tpd_RESTSPLHOLOTHr
                            ,A.Tpd_RESTSPLHOLNDHr
                            ,A.Tpd_RESTSPLHOLNDOTHr
                            ,A.Tpd_RESTCOMPHOLHr
                            ,A.Tpd_RESTCOMPHOLOTHr
                            ,A.Tpd_RESTCOMPHOLNDHr
                            ,A.Tpd_RESTCOMPHOLNDOTHr
                            ,A.Tpd_RESTPSDHr
                            ,A.Tpd_RESTPSDOTHr
                            ,A.Tpd_RESTPSDNDHr
                            ,A.Tpd_RESTPSDNDOTHr
                            ,A.Tpd_WorkDay
                            ,A.Tpd_PayrollType
                            ,A.Tpd_PremiumGrpCode
                            ,A.Usr_Login
                            ,A.Ludatetime
                        FROM {0}..T_EmpPayTranDtlHst A
                        LEFT JOIN {0}..T_EmpPayTranDtlTrl B
                        ON A.Tpd_IDNo = B.Tpd_IDNo
	                        AND A.Tpd_PayCycle = B.Tpd_PayCycle
	                        AND A.Tpd_Date = B.Tpd_Date
                            AND B.Tpd_AdjPayCycle = @AdjustPayPeriod
                        WHERE B.Tpd_IDNo IS NULL
                            AND A.Tpd_PayCycle = @AffectedPayPeriod
                            AND A.Tpd_IDNo = '{1}'
	
                        --INSERT INTO PAYROLL TRANSACTION TRAIL EXT
                        INSERT INTO {0}..T_EmpPayTranHdrMiscTrl
                        (
	                         Tph_IDNo
                            ,Tph_AdjPayCycle
                            ,Tph_PayCycle
                            ,Tph_Misc1Hr
                            ,Tph_Misc1OTHr
                            ,Tph_Misc1NDHr
                            ,Tph_Misc1NDOTHr
                            ,Tph_Misc2Hr
                            ,Tph_Misc2OTHr
                            ,Tph_Misc2NDHr
                            ,Tph_Misc2NDOTHr
                            ,Tph_Misc3Hr
                            ,Tph_Misc3OTHr
                            ,Tph_Misc3NDHr
                            ,Tph_Misc3NDOTHr
                            ,Tph_Misc4Hr
                            ,Tph_Misc4OTHr
                            ,Tph_Misc4NDHr
                            ,Tph_Misc4NDOTHr
                            ,Tph_Misc5Hr
                            ,Tph_Misc5OTHr
                            ,Tph_Misc5NDHr
                            ,Tph_Misc5NDOTHr
                            ,Tph_Misc6Hr
                            ,Tph_Misc6OTHr
                            ,Tph_Misc6NDHr
                            ,Tph_Misc6NDOTHr
                            ,Usr_Login
                            ,Ludatetime
                        )
                        SELECT A.Tph_IDNo
                            ,@AdjustPayPeriod
                            ,A.Tph_PayCycle
                            ,A.Tph_Misc1Hr
                            ,A.Tph_Misc1OTHr
                            ,A.Tph_Misc1NDHr
                            ,A.Tph_Misc1NDOTHr
                            ,A.Tph_Misc2Hr
                            ,A.Tph_Misc2OTHr
                            ,A.Tph_Misc2NDHr
                            ,A.Tph_Misc2NDOTHr
                            ,A.Tph_Misc3Hr
                            ,A.Tph_Misc3OTHr
                            ,A.Tph_Misc3NDHr
                            ,A.Tph_Misc3NDOTHr
                            ,A.Tph_Misc4Hr
                            ,A.Tph_Misc4OTHr
                            ,A.Tph_Misc4NDHr
                            ,A.Tph_Misc4NDOTHr
                            ,A.Tph_Misc5Hr
                            ,A.Tph_Misc5OTHr
                            ,A.Tph_Misc5NDHr
                            ,A.Tph_Misc5NDOTHr
                            ,A.Tph_Misc6Hr
                            ,A.Tph_Misc6OTHr
                            ,A.Tph_Misc6NDHr
                            ,A.Tph_Misc6NDOTHr
                            ,A.Usr_Login
                            ,A.Ludatetime
                        FROM {0}..T_EmpPayTranHdrMiscHst A
                        LEFT JOIN {0}..T_EmpPayTranHdrMiscTrl B
                        ON A.Tph_IDNo = B.Tph_IDNo
	                        AND A.Tph_PayCycle = B.Tph_PayCycle
                            AND B.Tph_AdjPayCycle = @AdjustPayPeriod
                        WHERE B.Tph_IDNo IS NULL
                            AND A.Tph_PayCycle = @AffectedPayPeriod
                            AND A.Tph_IDNo = '{1}'
	
                        --INSERT INTO PAYROLL TRANSACTION TRAIL EXT DETAIL
                        INSERT INTO {0}..T_EmpPayTranDtlMiscTrl
                        (
	                         Tpd_IDNo
                            ,Tpd_AdjPayCycle
                            ,Tpd_PayCycle
                            ,Tpd_Date
                            ,Tpd_Misc1Hr
                            ,Tpd_Misc1OTHr
                            ,Tpd_Misc1NDHr
                            ,Tpd_Misc1NDOTHr
                            ,Tpd_Misc2Hr
                            ,Tpd_Misc2OTHr
                            ,Tpd_Misc2NDHr
                            ,Tpd_Misc2NDOTHr
                            ,Tpd_Misc3Hr
                            ,Tpd_Misc3OTHr
                            ,Tpd_Misc3NDHr
                            ,Tpd_Misc3NDOTHr
                            ,Tpd_Misc4Hr
                            ,Tpd_Misc4OTHr
                            ,Tpd_Misc4NDHr
                            ,Tpd_Misc4NDOTHr
                            ,Tpd_Misc5Hr
                            ,Tpd_Misc5OTHr
                            ,Tpd_Misc5NDHr
                            ,Tpd_Misc5NDOTHr
                            ,Tpd_Misc6Hr
                            ,Tpd_Misc6OTHr
                            ,Tpd_Misc6NDHr
                            ,Tpd_Misc6NDOTHr
                            ,Usr_Login
                            ,Ludatetime
                        )
                        SELECT A.Tpd_IDNo
                            ,@AdjustPayPeriod
                            ,A.Tpd_PayCycle
                            ,A.Tpd_Date
                            ,A.Tpd_Misc1Hr
                            ,A.Tpd_Misc1OTHr
                            ,A.Tpd_Misc1NDHr
                            ,A.Tpd_Misc1NDOTHr
                            ,A.Tpd_Misc2Hr
                            ,A.Tpd_Misc2OTHr
                            ,A.Tpd_Misc2NDHr
                            ,A.Tpd_Misc2NDOTHr
                            ,A.Tpd_Misc3Hr
                            ,A.Tpd_Misc3OTHr
                            ,A.Tpd_Misc3NDHr
                            ,A.Tpd_Misc3NDOTHr
                            ,A.Tpd_Misc4Hr
                            ,A.Tpd_Misc4OTHr
                            ,A.Tpd_Misc4NDHr
                            ,A.Tpd_Misc4NDOTHr
                            ,A.Tpd_Misc5Hr
                            ,A.Tpd_Misc5OTHr
                            ,A.Tpd_Misc5NDHr
                            ,A.Tpd_Misc5NDOTHr
                            ,A.Tpd_Misc6Hr
                            ,A.Tpd_Misc6OTHr
                            ,A.Tpd_Misc6NDHr
                            ,A.Tpd_Misc6NDOTHr
                            ,A.Usr_Login
                            ,A.Ludatetime
                        FROM {0}..T_EmpPayTranDtlMiscHst A
                        LEFT JOIN {0}..T_EmpPayTranDtlMiscTrl B
                        ON A.Tpd_IDNo = B.Tpd_IDNo
	                        AND A.Tpd_PayCycle = B.Tpd_PayCycle
	                        AND A.Tpd_Date = B.Tpd_Date
                            AND B.Tpd_AdjPayCycle = @AdjustPayPeriod
                        WHERE B.Tpd_IDNo IS NULL
                            AND A.Tpd_PayCycle = @AffectedPayPeriod
                            AND A.Tpd_IDNo = '{1}' "
                        , strDBName
                        , drArrLogLedger[x]["Ttm_IDNo"]
                        , dtProcessDate.ToShortDateString());
                    #endregion
                    dal.ExecuteNonQuery(strTrailPrevious);
                    #endregion
                }

                strUpdateQuery += string.Format(strUpdateTemplate
                                                , strDBName
                                                , strLogLedgerMiscTable
                                                , drArrLogLedger[x]["Ttm_IDNo"]
                                                , drArrLogLedger[x]["Ttm_Date"]
                                                , drArrLogLedger[x]["Ttm_ActIn_01"]
                                                , drArrLogLedger[x]["Ttm_ActOut_01"]                //5
                                                , drArrLogLedger[x]["Ttm_ActIn_02"]
                                                , drArrLogLedger[x]["Ttm_ActOut_02"]
                                                , drArrLogLedger[x]["Ttm_ActIn_03"]
                                                , drArrLogLedger[x]["Ttm_ActOut_03"]
                                                , drArrLogLedger[x]["Ttm_ActIn_04"]                 //10
                                                , drArrLogLedger[x]["Ttm_ActOut_04"]
                                                , drArrLogLedger[x]["Ttm_ActIn_05"]
                                                , drArrLogLedger[x]["Ttm_ActOut_05"]
                                                , drArrLogLedger[x]["Ttm_ActIn_06"]
                                                , drArrLogLedger[x]["Ttm_ActOut_06"]                //15
                                                , drArrLogLedger[x]["Ttm_ActIn_07"]
                                                , drArrLogLedger[x]["Ttm_ActOut_07"]
                                                , drArrLogLedger[x]["Ttm_ActIn_08"]
                                                , drArrLogLedger[x]["Ttm_ActOut_08"]
                                                , drArrLogLedger[x]["Ttm_ActIn_09"]                 //20
                                                , drArrLogLedger[x]["Ttm_ActOut_09"]
                                                , drArrLogLedger[x]["Ttm_ActIn_10"]
                                                , drArrLogLedger[x]["Ttm_ActOut_10"]
                                                , drArrLogLedger[x]["Ttm_ActIn_11"]
                                                , drArrLogLedger[x]["Ttm_ActOut_11"]                //25
                                                , drArrLogLedger[x]["Ttm_ActIn_12"]
                                                , drArrLogLedger[x]["Ttm_ActOut_12"]
                                                , drArrLogLedger[x]["Ttm_ActIn1"]
                                                , drArrLogLedger[x]["Ttm_ActOut1"]
                                                , drArrLogLedger[x]["Ttm_ActIn2"]                   //30
                                                , drArrLogLedger[x]["Ttm_ActOut2"]
                                                );

                if (Globals.isAutoChangeShift == true && Globals.ShiftPocket == "OUT1")
                #region
                {
                    strUpdateQuery2 += string.Format(strUpdateTemplate2
                                                           , strDBName
                                                           , strLogLedgerTable
                                                           , strLogLedgerMiscTable
                                                           , drArrLogLedger[x]["Ttm_IDNo"]
                                                           , drArrLogLedger[x]["Ttm_Date"]
                                                           , Globals.CentralProfile
                                                           , strCompanyCode);

                    if (bIsInHistTable == false)
                        strUpdateQuery3 += string.Format(strUpdateTemplate3
                                                           , strDBName
                                                           , strLogLedgerTable
                                                           , strLogLedgerMiscTable
                                                           , drArrLogLedger[x]["Ttm_IDNo"]
                                                           , drArrLogLedger[x]["Ttm_Date"]
                                                           , Globals.CentralProfile
                                                           , strCompanyCode);

                }
                #endregion

                iUpdateCtr++;
                if (iUpdateCtr == 150) //150 log ledger records per batch update
                {
                    dal.ExecuteNonQuery(strUpdateQuery);
                    strUpdateQuery = "";
                    if (Globals.isAutoChangeShift == true && Globals.ShiftPocket == "OUT1")
                    {
                        dal.ExecuteNonQuery(strUpdateQuery2);
                        strUpdateQuery2 = "";

                        dal.ExecuteNonQuery(strUpdateQuery3);
                        strUpdateQuery3 = "";
                    }

                    iUpdateCtr = 0;
                }
            }

            if (strUpdateQuery != "")
                dal.ExecuteNonQuery(strUpdateQuery);

            if (Globals.isAutoChangeShift == true && Globals.ShiftPocket == "OUT1")
            {
                if (strUpdateQuery2 != "")
                    dal.ExecuteNonQuery(strUpdateQuery2);

                if (strUpdateQuery3 != "")
                    dal.ExecuteNonQuery(strUpdateQuery3);
            }

            Console.Write(" {0} Successfully Saved", drArrLogLedger.Length);
            #endregion

            #region Save DTR Records
            Console.Write("\n{0} Saving DTR...", dtProcessDate.ToShortDateString());
            drArrDTR = dtDTR.Select("Edited = 1 OR (PostFlag = 1 AND Edited = 0)");
            strUpdateTemplate = @"UPDATE {0}..T_EmpDTR SET Tel_IsPosted = '{4}', Tel_Remark = '{6}'  WHERE Tel_IDNo = '{1}' AND Tel_LogDate = '{2}' AND Tel_LogTime = '{3}' AND Tel_LogType = '{5}' ";
            strUpdateQuery = "";
            iUpdateCtr = 0;
            for (int x = 0; x < drArrDTR.Length; x++)
            {
                strUpdateQuery += string.Format(strUpdateTemplate
                                                , Globals.DTRDBName         //0
                                                , drArrDTR[x]["EmployeeID"] //1
                                                , drArrDTR[x]["LogDate"]    //2
                                                , drArrDTR[x]["LogTime"]    //3
                                                , drArrDTR[x]["PostFlag"]   //4
                                                , drArrDTR[x]["LogType"]    //5
                                                , drArrDTR[x]["LogRemark"]); //6
                iUpdateCtr++;
                if (iUpdateCtr == 100) //from 150 to 100 DTR records per batch update
                {
                    dal.ExecuteNonQuery(strUpdateQuery);
                    strUpdateQuery = "";
                    iUpdateCtr = 0;
                }
            }

            if (strUpdateQuery != "")
                dal.ExecuteNonQuery(strUpdateQuery);

            Console.Write(" {0} Successfully Saved", drArrDTR.Length);
            #endregion

            Console.WriteLine("End: {0:M/dd/yyyy HH:mm:ss} ", DateTime.Now);
            return drArrDTR.Length;
        }

        private void CreateLogControlRecordIfNotExists(string DBName, DALHelper dal)
        {
            #region Query
            string strQuery = string.Format(@"
DECLARE @MONTHSTART AS VARCHAR(6)
DECLARE @MONTHEND AS VARCHAR(6)
DECLARE @MONTHNOW AS VARCHAR(6)

SELECT @MONTHSTART = CONVERT(VARCHAR, DATEPART(YEAR, Tps_StartCycle)) + RIGHT('00' + CONVERT(VARCHAR, DATEPART(MONTH, Tps_StartCycle)), 2)
	, @MONTHEND = CONVERT(VARCHAR, DATEPART(YEAR, Tps_EndCycle)) + RIGHT('00' + CONVERT(VARCHAR, DATEPART(MONTH, Tps_EndCycle)), 2)
	, @MONTHNOW = CONVERT(VARCHAR, DATEPART(YEAR, GETDATE())) + RIGHT('00' + CONVERT(VARCHAR, DATEPART(MONTH, GETDATE())), 2)
FROM {0}..T_PaySchedule
WHERE Tps_CycleIndicator = 'C'

INSERT INTO {1}..T_EmpLogControl
	(Tlc_YearMonth
      ,Tlc_IDNo
      ,Tlc_Day01
      ,Tlc_Day02
      ,Tlc_Day03
      ,Tlc_Day04
      ,Tlc_Day05
      ,Tlc_Day06
      ,Tlc_Day07
      ,Tlc_Day08
      ,Tlc_Day09
      ,Tlc_Day10
      ,Tlc_Day11
      ,Tlc_Day12
      ,Tlc_Day13
      ,Tlc_Day14
      ,Tlc_Day15
      ,Tlc_Day16
      ,Tlc_Day17
      ,Tlc_Day18
      ,Tlc_Day19
      ,Tlc_Day20
      ,Tlc_Day21
      ,Tlc_Day22
      ,Tlc_Day23
      ,Tlc_Day24
      ,Tlc_Day25
      ,Tlc_Day26
      ,Tlc_Day27
      ,Tlc_Day28
      ,Tlc_Day29
      ,Tlc_Day30
      ,Tlc_Day31
      ,Tlc_RecordStatus
      ,Tlc_CreatedBy
      ,Tlc_CreatedDate)
SELECT @MONTHSTART
      ,Mem_IDNo
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day01
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day02
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day03
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day04
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day05
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day06
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day07
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day08
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day09
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day10
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day11
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day12
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day13
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day14
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day15
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day16
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day17
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day18
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day19
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day20
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day21
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day22
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day23
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day24
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day25
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day26
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day27
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day28
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day29
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day30
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day31
      ,'A'
      ,'SA'
      ,GETDATE()
FROM {0}..M_Employee
LEFT JOIN {1}..T_EmpLogControl
ON Mem_IDNo = Tlc_IDNo
	AND Tlc_YearMonth = @MONTHSTART
WHERE Tlc_IDNo IS NULL

INSERT INTO {1}..T_EmpLogControl
	(Tlc_YearMonth
      ,Tlc_IDNo
      ,Tlc_Day01
      ,Tlc_Day02
      ,Tlc_Day03
      ,Tlc_Day04
      ,Tlc_Day05
      ,Tlc_Day06
      ,Tlc_Day07
      ,Tlc_Day08
      ,Tlc_Day09
      ,Tlc_Day10
      ,Tlc_Day11
      ,Tlc_Day12
      ,Tlc_Day13
      ,Tlc_Day14
      ,Tlc_Day15
      ,Tlc_Day16
      ,Tlc_Day17
      ,Tlc_Day18
      ,Tlc_Day19
      ,Tlc_Day20
      ,Tlc_Day21
      ,Tlc_Day22
      ,Tlc_Day23
      ,Tlc_Day24
      ,Tlc_Day25
      ,Tlc_Day26
      ,Tlc_Day27
      ,Tlc_Day28
      ,Tlc_Day29
      ,Tlc_Day30
      ,Tlc_Day31
      ,Tlc_RecordStatus
      ,Tlc_CreatedBy
      ,Tlc_CreatedDate)
SELECT @MONTHEND
      ,Mem_IDNo
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day01
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day02
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day03
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day04
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day05
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day06
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day07
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day08
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day09
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day10
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day11
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day12
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day13
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day14
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day15
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day16
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day17
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day18
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day19
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day20
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day21
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day22
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day23
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day24
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day25
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day26
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day27
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day28
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day29
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day30
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day31
      ,'A'
      ,'SA'
      ,GETDATE()
FROM {0}..M_Employee
LEFT JOIN {1}..T_EmpLogControl
ON Mem_IDNo = Tlc_IDNo
	AND Tlc_YearMonth = @MONTHEND
WHERE Tlc_IDNo IS NULL

INSERT INTO {1}..T_EmpLogControl
	(Tlc_YearMonth
      ,Tlc_IDNo
      ,Tlc_Day01
      ,Tlc_Day02
      ,Tlc_Day03
      ,Tlc_Day04
      ,Tlc_Day05
      ,Tlc_Day06
      ,Tlc_Day07
      ,Tlc_Day08
      ,Tlc_Day09
      ,Tlc_Day10
      ,Tlc_Day11
      ,Tlc_Day12
      ,Tlc_Day13
      ,Tlc_Day14
      ,Tlc_Day15
      ,Tlc_Day16
      ,Tlc_Day17
      ,Tlc_Day18
      ,Tlc_Day19
      ,Tlc_Day20
      ,Tlc_Day21
      ,Tlc_Day22
      ,Tlc_Day23
      ,Tlc_Day24
      ,Tlc_Day25
      ,Tlc_Day26
      ,Tlc_Day27
      ,Tlc_Day28
      ,Tlc_Day29
      ,Tlc_Day30
      ,Tlc_Day31
      ,Tlc_RecordStatus
      ,Tlc_CreatedBy
      ,Tlc_CreatedDate)
SELECT @MONTHNOW
      ,Mem_IDNo
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day01
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day02
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day03
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day04
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day05
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day06
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day07
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day08
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day09
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day10
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day11
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day12
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day13
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day14
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day15
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day16
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day17
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day18
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day19
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day20
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day21
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day22
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day23
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day24
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day25
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day26
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day27
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day28
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day29
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day30
      ,'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114) AS Tlc_Day31
      ,'A'
      ,'SA'
      ,GETDATE()
FROM {0}..M_Employee
LEFT JOIN {1}..T_EmpLogControl
ON Mem_IDNo = Tlc_IDNo
	AND Tlc_YearMonth = @MONTHNOW
WHERE Tlc_IDNo IS NULL", DBName, Globals.CentralProfile);
            #endregion

            dal.ExecuteNonQuery(strQuery);
        }
        private List<clsEmpDTR> DataTableToList(DataTable dt)
        {
            var convertedList = (from rw in dt.AsEnumerable()
                                 select new clsEmpDTR()
                                 {
                                     Seq        = Convert.ToInt32(GetValue(rw["Seq"])),
                                     IDNo       = GetValue(rw["EmployeeID"]),
                                     LogDate    = Convert.ToDateTime(rw["LogDate"]),
                                     LogTime    = GetValue(rw["LogTime"]),
                                     LogTimeMins = 0,
                                     LogType    = GetValue(rw["LogType"]),
                                     PostFlag   = Convert.ToBoolean(GetValue(rw["PostFlag"])),
                                     LogRemark  = GetValue(rw["LogRemark"]),
                                     PocketNo   = 0,
                                     Remarks    = "",
                                     Edited     = false
                                 }).ToList();

            return convertedList;
        }
        public DataTable GetMinimumPasPeriods(string DBName, string CompanyCode, DALHelper dal)
        {
            #region Query
            string strQuery = string.Format(@"
            SELECT *
            FROM (
	            SELECT ROW_NUMBER() OVER(ORDER BY Tps_PayCycle DESC) AS PeriodCount
			            , Tps_PayCycle
			            , Tps_StartCycle
                        , Tps_EndCycle
	            FROM {0}..T_PaySchedule
	            WHERE Tps_CycleIndicator IN ('P')
            ) TEMP
            WHERE PeriodCount <= {1}
            ORDER BY 1 DESC", DBName, Globals.TKADJCYCLE);
            #endregion
            DataTable dtPayPeriod = dal.ExecuteDataSet(strQuery).Tables[0];

            return dtPayPeriod;
        }

        private DataTable GetCurrentPayPeriod(string DBName, DALHelper dal)
        {
            #region Query
            string strQuery = string.Format(@"
            SELECT Tps_StartCycle
                , Tps_EndCycle
            FROM {0}..T_PaySchedule
            WHERE Tps_CycleIndicator = 'C'", DBName);
            #endregion

            DataTable dtPayPeriod = dal.ExecuteDataSet(strQuery).Tables[0];

            return dtPayPeriod;
        }

        private DataTable GetNextPayPeriod(string DBName, DALHelper dal)
        {
            #region Query
            string strQuery = string.Format(@"
                DECLARE @EndDate as Datetime
                Set @EndDate = (Select Tps_EndCycle From {0}..T_PaySchedule
                Where Tps_CycleIndicator = 'C' and Tps_RecordStatus = 'A')

                SELECT Tps_StartCycle
                    , Tps_EndCycle
                FROM {0}..T_PaySchedule
                Where Tps_StartCycle = dateadd(dd,1,@EndDate)
                AND Tps_CycleIndicator <> 'S'
                AND Tps_RecordStatus = 'A'", DBName);
            #endregion

            DataTable dtPayPeriod = dal.ExecuteDataSet(strQuery).Tables[0];

            return dtPayPeriod;
        }

        private DataTable GetLogLedgerRecordsPerDate(string DBName, string CompanyCode, DateTime dtProcessDate, bool bGetPartial, string strEmployeeID, DALHelper dal)
        {
            #region Query to Get Partial Logs
            string strGetFromLogControl = "";
            if (bGetPartial == true)
            {
                strGetFromLogControl = string.Format(@"
                    INNER JOIN (
                        SELECT Tlc_IDNo
                        FROM {0}..T_EmpLogControl
                        WHERE Tlc_YearMonth = '{1}'
	                        AND LEFT(Tlc_Day{2}, 1) != 'F') LOGCONTROL
                    ON ledger.Ttr_IDNo = Tlc_IDNo
                    ", Globals.CentralProfile, dtProcessDate.ToString("yyyyMM"), dtProcessDate.ToString("dd"));
            }
            #endregion

            #region Query to Force Repost Logs
            string strRepostLogs = "";
            if (Globals.isOverwrite == false)
            {
                strRepostLogs = @"
		ledger.Ttr_ActIn_1 ,
		ledger.Ttr_ActOut_1 ,
		ledger.Ttr_ActIn_2 ,
		ledger.Ttr_ActOut_2 ,";
            }
            else
            {
                strRepostLogs = @"
		'0000' AS Ttr_ActIn_1 ,
		'0000' AS Ttr_ActOut_1 ,
		'0000' AS Ttr_ActIn_2 ,
		'0000' AS Ttr_ActOut_2 ,";
            }
            #endregion

            #region Query to Filter Employee ID
            string strProcessIndividual = "";
            if (strEmployeeID != "")
            {
                strProcessIndividual = string.Format(@" AND ledger.Ttr_IDNo = '{0}'", strEmployeeID);
            }
            #endregion

            #region Query
            string strTableName = "T_EmpTimeRegister";
            string strQuery = @"
            SELECT ledger.Ttr_IDNo ,
		            ledger.Ttr_Date ,
            {3}
		            Shift.Msh_Schedule AS ScheduleType ,
		            Shift.Msh_ShiftIn1 AS ShiftTimeIn ,
		            Shift.Msh_ShiftOut1 AS ShiftBreakStart ,
		            Shift.Msh_ShiftIn2 AS ShiftBreakEnd ,
		            Shift.Msh_ShiftOut2 AS ShiftTimeOut ,
                    0 AS Edited ,
                    ledger.Ttr_SkipService AS SkipService ,
                    '{5}' AS TableName 
	            FROM {0}..{5} AS ledger
            INNER JOIN {6}..M_Shift AS shift ON ledger.Ttr_ShiftCode = shift.Msh_ShiftCode
                    AND Msh_CompanyCode = '{7}'
            {2}
	            WHERE ledger.Ttr_Date = '{1}' {4}
            ORDER BY ledger.Ttr_IDNo, ledger.Ttr_Date";
            #endregion

            DataTable dtLogLedgerRec = dal.ExecuteDataSet(string.Format(strQuery, DBName
                                                                    , dtProcessDate.ToShortDateString()
                                                                    , strGetFromLogControl
                                                                    , strRepostLogs
                                                                    , strProcessIndividual
                                                                    , strTableName
                                                                    , Globals.CentralProfile
                                                                    , CompanyCode)).Tables[0];

            if (dtLogLedgerRec.Rows.Count == 0)
            {
                strTableName = "T_EmpTimeRegisterHst";
                dtLogLedgerRec = dal.ExecuteDataSet(string.Format(strQuery, DBName
                                                                    , dtProcessDate.ToShortDateString()
                                                                    , strGetFromLogControl
                                                                    , strRepostLogs
                                                                    , strProcessIndividual
                                                                    , strTableName
                                                                    , Globals.CentralProfile
                                                                    , CompanyCode)).Tables[0];
            }

            return dtLogLedgerRec;
        }

        private DataTable GetLogLedgerRecordsPerDateMultiplePockets(string DBName, string CompanyCode, DateTime dtProcessDate, string strEmployeeID, DALHelper dal)
        {
            #region Query to Force Repost Logs
            string strRepostLogs = "";
            if (Globals.isOverwrite == false)
            {
                strRepostLogs = @"
		            ledgerMisc.Ttm_ActIn_01 ,
					ledgerMisc.Ttm_ActOut_01 ,
					ledgerMisc.Ttm_ActIn_02 ,
					ledgerMisc.Ttm_ActOut_02 ,
					ledgerMisc.Ttm_ActIn_03 ,
					ledgerMisc.Ttm_ActOut_03 ,
					ledgerMisc.Ttm_ActIn_04 ,
					ledgerMisc.Ttm_ActOut_04 ,
					ledgerMisc.Ttm_ActIn_05 ,
					ledgerMisc.Ttm_ActOut_05 ,
					ledgerMisc.Ttm_ActIn_06 ,
					ledgerMisc.Ttm_ActOut_06 ,
					ledgerMisc.Ttm_ActIn_07 ,
					ledgerMisc.Ttm_ActOut_07 ,
					ledgerMisc.Ttm_ActIn_08 ,
					ledgerMisc.Ttm_ActOut_08 ,
					ledgerMisc.Ttm_ActIn_09 ,
					ledgerMisc.Ttm_ActOut_09 ,
					ledgerMisc.Ttm_ActIn_10 ,
					ledgerMisc.Ttm_ActOut_10 ,
					ledgerMisc.Ttm_ActIn_11 ,
					ledgerMisc.Ttm_ActOut_11 ,
					ledgerMisc.Ttm_ActIn_12 ,
					ledgerMisc.Ttm_ActOut_12 ,
                    ledgerMisc.Ttm_ActIn1 ,
                    ledgerMisc.Ttm_ActOut1 ,
                    ledgerMisc.Ttm_ActIn2 ,
                    ledgerMisc.Ttm_ActOut2 , ";
            }
            else
            {
                strRepostLogs = @"
                    '0000' AS Ttm_ActIn_01 ,
					'0000' AS Ttm_ActOut_01 ,
					'0000' AS Ttm_ActIn_02 ,
					'0000' AS Ttm_ActOut_02 ,
					'0000' AS Ttm_ActIn_03 ,
					'0000' AS Ttm_ActOut_03 ,
					'0000' AS Ttm_ActIn_04 ,
					'0000' AS Ttm_ActOut_04 ,
					'0000' AS Ttm_ActIn_05 ,
					'0000' AS Ttm_ActOut_05 ,
					'0000' AS Ttm_ActIn_06 ,
					'0000' AS Ttm_ActOut_06 ,
					'0000' AS Ttm_ActIn_07 ,
					'0000' AS Ttm_ActOut_07 ,
					'0000' AS Ttm_ActIn_08 ,
					'0000' AS Ttm_ActOut_08 ,
					'0000' AS Ttm_ActIn_09 ,
					'0000' AS Ttm_ActOut_09 ,
					'0000' AS Ttm_ActIn_10 ,
					'0000' AS Ttm_ActOut_10 ,
					'0000' AS Ttm_ActIn_11 ,
					'0000' AS Ttm_ActOut_11 ,
					'0000' AS Ttm_ActIn_12 ,
					'0000' AS Ttm_ActOut_12 ,
                    '0000' AS Ttm_ActIn1 ,
                    '0000' AS Ttm_ActOut1 ,
                    '0000' AS Ttm_ActIn2 ,
                    '0000' AS Ttm_ActOut2 , ";
            }
            #endregion

            #region Query to Filter Employee ID
            string strProcessIndividual = "";
            if (strEmployeeID != "")
            {
                strProcessIndividual = string.Format(@" AND ledgerMisc.Ttm_IDNo = '{0}'", strEmployeeID);
            }
            #endregion

            #region Query
            string strTableName = "T_EmpTimeRegister";
            string strTableNameMisc = "T_EmpTimeRegisterMisc";
            string strQuery = @"
            SELECT ledgerMisc.Ttm_IDNo ,
		            ledgerMisc.Ttm_Date ,
                    {3}
		            [SHIFT].Msh_Schedule AS ScheduleType,
		            [SHIFT].Msh_ShiftIn1 AS ShiftTimeIn,
		            [SHIFT].Msh_ShiftOut1 AS ShiftBreakStart,
		            [SHIFT].Msh_ShiftIn2 AS ShiftBreakEnd,
		            [SHIFT].Msh_ShiftOut2 AS ShiftTimeOut,
                    [SHIFT].Msh_RequiredLogsOnBreak AS RequiredLogsOnBreak,
                    ledger.Ttr_SkipService AS SkipService,
                    0 AS Edited ,
                    '{2}' AS TableName 
	            FROM {0}..{2} AS ledgerMisc
                INNER JOIN {0}..{5} AS ledger 
					ON ledger.Ttr_IDNo = ledgerMisc.Ttm_IDNo
					AND ledger.Ttr_Date = ledgerMisc.Ttm_Date
                INNER JOIN {6}..M_Shift AS [SHIFT] ON ledger.Ttr_ShiftCode = [SHIFT].Msh_ShiftCode
                    AND [SHIFT].Msh_CompanyCode = '{7}'
	            WHERE ledgerMisc.Ttm_Date = '{1}' {4}
                ORDER BY ledgerMisc.Ttm_IDNo, ledgerMisc.Ttm_Date";
            #endregion

            DataTable dtLogLedgerRec = dal.ExecuteDataSet(string.Format(strQuery, DBName
                                                                    , dtProcessDate.ToShortDateString()
                                                                    , strTableNameMisc
                                                                    , strRepostLogs
                                                                    , strProcessIndividual
                                                                    , strTableName
                                                                    , Globals.CentralProfile
                                                                    , CompanyCode)).Tables[0];

            if (dtLogLedgerRec.Rows.Count == 0)
            {
                strTableName = "T_EmpTimeRegisterHst";
                strTableNameMisc = "T_EmpTimeRegisterMiscHst";
                dtLogLedgerRec = dal.ExecuteDataSet(string.Format(strQuery, DBName
                                                                    , dtProcessDate.ToShortDateString()
                                                                    , strTableNameMisc
                                                                    , strRepostLogs
                                                                    , strProcessIndividual
                                                                    , strTableName
                                                                    , Globals.CentralProfile
                                                                    , CompanyCode)).Tables[0];
            }

            return dtLogLedgerRec;
        }

        private DataTable GetDTRRecordsPerDate(DateTime dtProcessDate, string strEmployeeID, DALHelper dal)
        {
            #region Query to Filter Employee ID
            string strProcessIndividual = "";
            if (strEmployeeID != "")
            {
                strProcessIndividual = string.Format(@" AND dtr.Tel_IDNo = '{0}'", strEmployeeID);
            }
            else
            {
                strProcessIndividual = "";
            }
            #endregion

            #region Query
            string strQuery = string.Format(@"
                                SELECT ROW_NUMBER() OVER (ORDER BY dtr.Tel_IDNo,CAST(CONVERT(CHAR(10),dtr.Tel_LogDate) + ' ' + Substring(dtr.Tel_LogTime, 1, 2) + ':' + Substring(dtr.Tel_LogTime, 3, 2) AS DateTime),dtr.LudateTime) [Seq]
                                       , dtr.Tel_IDNo AS EmployeeID 
                                       , dtr.Tel_LogDate AS LogDate
                                       , dtr.Tel_LogTime AS LogTime 
                                       , dtr.Tel_LogType AS LogType 
                                       , dtr.Tel_IsPosted AS PostFlag 
                                       , '' AS [LogRemark]
                                       , 0 AS Edited
                                       , '' AS [Remarks]
                                FROM {0}..T_EmpDTR AS dtr
                                WHERE ISDATE(CONVERT(CHAR(10),dtr.Tel_LogDate) + ' ' + Substring(dtr.Tel_LogTime, 1, 2) + ':' + Substring(dtr.Tel_LogTime, 3, 2)) = 1
                                     AND dtr.Tel_LogType <> 'X'  --FOR FEP 
		                             AND Tel_LogDate BETWEEN '{1}' AND '{2}'
                                     {3}
                                ", Globals.DTRDBName
                                 , dtProcessDate.ToShortDateString()
                                 , dtProcessDate.AddDays(1).ToShortDateString()
                                 , strProcessIndividual);
            #endregion

            DataTable dtDTRRec = dal.ExecuteDataSet(strQuery).Tables[0];

            return dtDTRRec;
        }

        private DataTable GetDTRRecordsPerDateMultiplePockets(string DBName, DateTime dtProcessDate, string strEmployeeID, DALHelper dal)
        {
            #region Query to Filter Employee ID
            string strIndividualDTR             = "";
            string strIndividualDTROverride     = "";
            string strIndividualTimeCorrection  = "";

            if (strEmployeeID != "")
            {
                strIndividualDTR = string.Format(@" AND dtr.Tel_IDNo = '{0}'", strEmployeeID);
                strIndividualDTROverride = string.Format(@" AND Tdo_IDNo = '{0}'", strEmployeeID);
                strIndividualTimeCorrection = string.Format(@" AND Availment.Ttm_IDNo = '{0}'", strEmployeeID);
            }
            
            #endregion

            #region Query
            string strQuery = string.Format(@"
            SELECT dtr.Tel_IDNo AS EmployeeID 
                   , dtr.Tel_LogDate AS LogDate 
                   , dtr.Tel_LogTime AS LogTime 
                   , dtr.Tel_LogType AS LogType 
                   , dtr.Tel_IsPosted AS PostFlag 
                   , 0 AS Edited
            FROM {0}..T_EmpDTR AS dtr
            WHERE ISDATE(CONVERT(CHAR(10),dtr.Tel_LogDate) + ' ' + Substring(dtr.Tel_LogTime, 1, 2) + ':' + Substring(dtr.Tel_LogTime, 3, 2)) = 1
                    AND dtr.Tel_LogType <> 'X' 
		            AND Tel_LogDate BETWEEN '{1}' AND '{2}'
                    {3}

            UNION ALL

            SELECT Tdo_IDNo AS EmployeeID 
                , Tdo_Date AS LogDate 
                , Tdo_Time AS LogTime 
                , Tdo_Type AS LogType 
                , 0 AS PostFlag 
                , 0 AS Edited
            FROM {6}..T_EmpDtrOverride
            WHERE Tdo_Date  BETWEEN '{1}' AND '{2}'
                {4}

            UNION ALL

            SELECT Availment.Ttm_IDNo AS EmployeeID 
                , Availment.Ttm_TimeCorDate AS LogDate 
                , IOTime AS LogTime 
	            , '' AS LogType 
                , 0 AS PostFlag 
                , 0 AS Edited
            FROM {6}..Udv_TimeCorrection  Availment
            LEFT JOIN {6}..Udv_TimeCorrection Cancellation on Cancellation.Ttm_OriginalDocumentNo = Availment.Ttm_DocumentNo
	            AND Cancellation.Ttm_TimeCorStatus = '15' 
	            AND Cancellation.Ttm_RequestType = 'C' 
            CROSS APPLY ( VALUES ('01', Availment.Ttm_TimeIn01, SUBSTRING(Availment.Ttm_LogControl,1,1)),
					             ('02', Availment.Ttm_TimeOut01, SUBSTRING(Availment.Ttm_LogControl,2,1)),
					             ('03', Availment.Ttm_TimeIn02, SUBSTRING(Availment.Ttm_LogControl,3,1)),
					             ('04', Availment.Ttm_TimeOut02, SUBSTRING(Availment.Ttm_LogControl,4,1)),					
					             ('05', Availment.Ttm_TimeIn03, SUBSTRING(Availment.Ttm_LogControl,5,1)),
					             ('06', Availment.Ttm_TimeOut03, SUBSTRING(Availment.Ttm_LogControl,6,1)),					
					             ('07', Availment.Ttm_TimeIn04, SUBSTRING(Availment.Ttm_LogControl,7,1)),
					             ('08', Availment.Ttm_TimeOut04, SUBSTRING(Availment.Ttm_LogControl,8,1)),						
					             ('09', Availment.Ttm_TimeIn05, SUBSTRING(Availment.Ttm_LogControl,9,1)),
					             ('10', Availment.Ttm_TimeOut05, SUBSTRING(Availment.Ttm_LogControl,10,1)),								
					             ('11', Availment.Ttm_TimeIn06, SUBSTRING(Availment.Ttm_LogControl,11,1)),
					             ('12', Availment.Ttm_TimeOut06, SUBSTRING(Availment.Ttm_LogControl,12,1)),	
					             ('13', Availment.Ttm_TimeIn07, SUBSTRING(Availment.Ttm_LogControl,13,1)),
					             ('14', Availment.Ttm_TimeOut07, SUBSTRING(Availment.Ttm_LogControl,14,1)),						 					
					             ('15', Availment.Ttm_TimeIn08, SUBSTRING(Availment.Ttm_LogControl,15,1)),
					             ('16', Availment.Ttm_TimeOut08, SUBSTRING(Availment.Ttm_LogControl,16,1)),	
					             ('17', Availment.Ttm_TimeIn09, SUBSTRING(Availment.Ttm_LogControl,17,1)),
					             ('18', Availment.Ttm_TimeOut09, SUBSTRING(Availment.Ttm_LogControl,18,1)),						 
					             ('19', Availment.Ttm_TimeIn10, SUBSTRING(Availment.Ttm_LogControl,19,1)),
					             ('20', Availment.Ttm_TimeOut10, SUBSTRING(Availment.Ttm_LogControl,20,1)),							 
					             ('21', Availment.Ttm_TimeIn11, SUBSTRING(Availment.Ttm_LogControl,21,1)),
					             ('22', Availment.Ttm_TimeOut11, SUBSTRING(Availment.Ttm_LogControl,22,1)),						 
					             ('23', Availment.Ttm_TimeIn12, SUBSTRING(Availment.Ttm_LogControl,23,1)),
					             ('24', Availment.Ttm_TimeOut12, SUBSTRING(Availment.Ttm_LogControl,24,1))					 
					 					            ) temp (Seq, IOTime, LogCtrl)
					
            WHERE LogCtrl = '0'
	            AND Availment.Ttm_TimeCorDate BETWEEN '{1}' AND '{2}'
	            AND Availment.Ttm_TimeCorStatus = '14'
	            AND Cancellation.Ttm_DocumentNo IS NULL  
                {5}

            ORDER BY EmployeeID, LogDate, LogTime"
            , Globals.DTRDBName
            , dtProcessDate.ToShortDateString()
            , dtProcessDate.AddDays(1).ToShortDateString()
            , strIndividualDTR
            , strIndividualDTROverride
            , strIndividualTimeCorrection
            , DBName);
            #endregion

            DataTable dtDTRRec = dal.ExecuteDataSet(strQuery).Tables[0];

            return dtDTRRec;
        }

        private DataTable GetLogTrailRecordsPerDate(string DBName, DateTime dtProcessDate, string strEmployeeID, DALHelper dal)
        {
            #region Query to Filter Employee ID
            string strProcessIndividual = "";
            if (strEmployeeID != "")
            {
                strProcessIndividual = string.Format(@" AND Ttl_IDNo = '{0}'", strEmployeeID);
            }
            else
            {
                strProcessIndividual = "";
            }
            #endregion

            #region Query
            string strQuery = string.Format(@"
                SELECT *
                FROM (
                SELECT Row_Number() OVER (Partition by Ttl_IDNo, Ttl_Date Order by Ttl_LineNo DESC) [Row]
	                , Ttl_IDNo
	                , Ttl_Date
	                , Ttl_ActIn_1
	                , Ttl_ActOut_1
	                , Ttl_ActIn_2
	                , Ttl_ActOut_2
                FROM {0}..T_EmpTimeRegisterLog A
                WHERE Ttl_Date = '{1}'
                {2}
                ) TEMP
                WHERE [Row] = 1", DBName, dtProcessDate.ToShortDateString(), strProcessIndividual);

            #endregion

            DataTable dtLogTrailRec = dal.ExecuteDataSet(strQuery).Tables[0];

            return dtLogTrailRec;
        }

        private DataTable GetTimeCorrectionRecordsPerDate(string DBName, DateTime dtProcessDate, string strEmployeeID, DALHelper dal)
        {
            #region Query to Filter Employee ID
            string strProcessIndividual = "";
            if (strEmployeeID != "")
            {
                strProcessIndividual = string.Format(@" AND Ttm_IDNo = '{0}'", strEmployeeID);
            }
            else
            {
                strProcessIndividual = "";
            }
            #endregion

            #region Query
            string strQuery = string.Format(@"
            SELECT Ttm_IDNo
	            ,Ttm_TimeCorDate
	            ,Ttm_TimeCorType
	            ,Ttm_TimeIn1
	            ,Ttm_TimeOut1
	            ,Ttm_TimeIn2
	            ,Ttm_TimeOut2
            FROM {0}..T_EmpTimeCorrection
            WHERE Ttm_TimeCorDate = '{1}'
                {2}
	            AND Ttm_TimeCorStatus = '14'", DBName, dtProcessDate.ToShortDateString(), strProcessIndividual);
            #endregion

            DataTable dtTimeModRec = dal.ExecuteDataSet(strQuery).Tables[0];

            return dtTimeModRec;
        }

        private DataTable GetTimeCorrectionRecordsPerDateMultiplePockets(string DBName, DateTime dtProcessDate, string strEmployeeID, DALHelper dal)
        {
            #region Query to Filter Employee ID
            string strProcessIndividual = "";
            if (strEmployeeID != "")
            {
                strProcessIndividual = string.Format(@" AND Availment.Ttm_IDNo = '{0}'", strEmployeeID);
            }
            else
            {
                strProcessIndividual = "";
            }
            #endregion

            #region Query
            string strQuery = string.Format(@"
            SELECT Availment.Ttm_IDNo AS Ttm_IDNo 
                , Availment.Ttm_TimeCorDate AS LogDate 
                , IOTime AS LogTime 
	            , '' AS LogType 
                , Seq AS PocketNo 
            FROM {0}..Udv_TimeCorrection  Availment
            LEFT JOIN {0}..Udv_TimeCorrection Cancellation on Cancellation.Ttm_OriginalDocumentNo = Availment.Ttm_DocumentNo
	            AND Cancellation.Ttm_TimeCorStatus = '15' 
	            AND Cancellation.Ttm_RequestType = 'C' 
            CROSS APPLY ( VALUES ('01', Availment.Ttm_TimeIn01, SUBSTRING(Availment.Ttm_LogControl,1,1)),
					             ('02', Availment.Ttm_TimeOut01, SUBSTRING(Availment.Ttm_LogControl,2,1)),
					             ('03', Availment.Ttm_TimeIn02, SUBSTRING(Availment.Ttm_LogControl,3,1)),
					             ('04', Availment.Ttm_TimeOut02, SUBSTRING(Availment.Ttm_LogControl,4,1)),					
					             ('05', Availment.Ttm_TimeIn03, SUBSTRING(Availment.Ttm_LogControl,5,1)),
					             ('06', Availment.Ttm_TimeOut03, SUBSTRING(Availment.Ttm_LogControl,6,1)),					
					             ('07', Availment.Ttm_TimeIn04, SUBSTRING(Availment.Ttm_LogControl,7,1)),
					             ('08', Availment.Ttm_TimeOut04, SUBSTRING(Availment.Ttm_LogControl,8,1)),						
					             ('09', Availment.Ttm_TimeIn05, SUBSTRING(Availment.Ttm_LogControl,9,1)),
					             ('10', Availment.Ttm_TimeOut05, SUBSTRING(Availment.Ttm_LogControl,10,1)),								
					             ('11', Availment.Ttm_TimeIn06, SUBSTRING(Availment.Ttm_LogControl,11,1)),
					             ('12', Availment.Ttm_TimeOut06, SUBSTRING(Availment.Ttm_LogControl,12,1)),	
					             ('13', Availment.Ttm_TimeIn07, SUBSTRING(Availment.Ttm_LogControl,13,1)),
					             ('14', Availment.Ttm_TimeOut07, SUBSTRING(Availment.Ttm_LogControl,14,1)),						 					
					             ('15', Availment.Ttm_TimeIn08, SUBSTRING(Availment.Ttm_LogControl,15,1)),
					             ('16', Availment.Ttm_TimeOut08, SUBSTRING(Availment.Ttm_LogControl,16,1)),	
					             ('17', Availment.Ttm_TimeIn09, SUBSTRING(Availment.Ttm_LogControl,17,1)),
					             ('18', Availment.Ttm_TimeOut09, SUBSTRING(Availment.Ttm_LogControl,18,1)),						 
					             ('19', Availment.Ttm_TimeIn10, SUBSTRING(Availment.Ttm_LogControl,19,1)),
					             ('20', Availment.Ttm_TimeOut10, SUBSTRING(Availment.Ttm_LogControl,20,1)),							 
					             ('21', Availment.Ttm_TimeIn11, SUBSTRING(Availment.Ttm_LogControl,21,1)),
					             ('22', Availment.Ttm_TimeOut11, SUBSTRING(Availment.Ttm_LogControl,22,1)),						 
					             ('23', Availment.Ttm_TimeIn12, SUBSTRING(Availment.Ttm_LogControl,23,1)),
					             ('24', Availment.Ttm_TimeOut12, SUBSTRING(Availment.Ttm_LogControl,24,1))					 
					 					            ) temp (Seq, IOTime, LogCtrl)
					
            WHERE LogCtrl = '0'
	            AND Availment.Ttm_TimeCorDate = '{1}'
	            AND Availment.Ttm_TimeCorStatus = '14'
	            AND Cancellation.Ttm_DocumentNo IS NULL  
                {2}
            --ORDER BY Availment.Ttm_IDNo, Availment.Ttm_TimeCorDate, IOTime", DBName, dtProcessDate.ToShortDateString(), strProcessIndividual);
            #endregion

            DataTable dtTimeModRec = dal.ExecuteDataSet(strQuery).Tables[0];

            return dtTimeModRec;
        }

        private DataTable GetDtrOverrideRecordsPerDate(string DBName, DateTime dtProcessDate, string strEmployeeID, DALHelper dal)
        {
            #region Query to Filter Employee ID
            string strProcessIndividual = "";
            if (strEmployeeID != "")
            {
                strProcessIndividual = string.Format(@" AND SS.Tdo_IDNo = '{0}'", strEmployeeID);
            }
            else
            {
                strProcessIndividual = "";
            }
            #endregion

            #region Query
            string strQuery = string.Format(@"
                SELECT SS.Tdo_IDNo, SS.Tdo_Date, SS.Tdo_Type, SS.Tdo_Time, SS.Ludatetime
                FROM    {0}..T_EmpDTROverride SS 
                INNER JOIN 
                        (
                        SELECT DISTINCT Tdo_IDNo, Tdo_Type, Tdo_Date, MAX(Ludatetime) AS MaxDateTime 
                        FROM    {0}..T_EmpDTROverride 
                        GROUP BY
                                Tdo_IDNo, Tdo_Type,Tdo_Date
                        ) groupedSS
                ON SS.Tdo_IDNo = groupedSS.Tdo_IDNo
                        AND SS.Ludatetime = groupedSS.MaxDateTime
		                AND SS.Tdo_Date = groupedSS.Tdo_Date
		                AND SS.Tdo_Type = groupedSS.Tdo_Type
                WHERE SS.Tdo_Date = '{1}'
	                {2}
                    ", DBName, dtProcessDate.ToShortDateString(), strProcessIndividual);
            #endregion

            DataTable dtOverride = dal.ExecuteDataSet(strQuery).Tables[0];

            return dtOverride;
        }

        private void CleanupMin(List<clsEmpDTR> ArrDTR, string strIN)
        {
            var ctrIn = ArrDTR.Where(s => s.Remarks == strIN);
            if (ctrIn.Count() > 1)
            {
                var minLogTimeMins = ArrDTR.Where(s => s.Remarks == strIN).Select(s => s.LogTimeMinsOrig).Min();
                for (int x = 0; x < ArrDTR.Count(); x++)
                {
                    if (ArrDTR[x].Remarks == strIN && ArrDTR[x].LogTimeMinsOrig != minLogTimeMins)
                    {
                        ArrDTR[x].PostFlag  = false;
                        ArrDTR[x].PocketNo  = 0;
                        ArrDTR[x].Remarks   = "";
                        ArrDTR[x].Edited    = false;
                    }
                }
            }
        }

        private void CleanupMax(List<clsEmpDTR> ArrDTR, string strOUT)
        {
            var ctrOut = ArrDTR.Where(s => s.Remarks == strOUT);
            if (ctrOut.Count() > 1)
            {
                var maxLogTimeMins = ArrDTR.Where(s => s.Remarks == strOUT).Select(s => s.LogTimeMinsOrig).Max();
                for (int x = 0; x < ArrDTR.Count(); x++)
                {
                    if (ArrDTR[x].Remarks == strOUT && ArrDTR[x].LogTimeMinsOrig != maxLogTimeMins)
                    {
                        ArrDTR[x].PostFlag  = false;
                        ArrDTR[x].PocketNo  = 0;
                        ArrDTR[x].Remarks   = "";
                        ArrDTR[x].Edited    = false;
                    }
                }
            }
        }

        private bool SplitBasicFour(List<clsEmpDTR> ArrDTR, int iShiftTimeIn1Min, string strIN, string strOUT)
        {
            bool bMulti = false;
            int idxIN = ArrDTR.FindIndex(s => s.Remarks == strIN);
            int idxOUT = ArrDTR.FindIndex(s => s.Remarks == strOUT);
            if (ArrDTR.Where(s => s.Remarks == strIN).Any()
                && ArrDTR.Where(s => s.Remarks == strOUT).Any()
                && (idxOUT - idxIN) > 1)
            {
                bool isPaired       = false;
                int idxMultIN       = 0;
                int idxMultOUT      = 0;
                int idxLastPocket   = idxIN;
                for (int x = (idxIN + 1); x < idxOUT; x++)
                {
                    int tmpPrevMins = ArrDTR[x - 1].LogTimeMins;
                    int tmpCurMins = ArrDTR[x].LogTimeMins;

                    if (tmpCurMins < iShiftTimeIn1Min)
                    {
                        #region Tag Advance Logs
                        ArrDTR[x].LogRemark = "A";
                        ArrDTR[x].PostFlag  = false;
                        #endregion
                    }
                    else if (tmpCurMins >= iShiftTimeIn1Min)
                    {
                        #region Tag Middle Logs
                        ArrDTR[x].LogRemark = "M";
                        ArrDTR[x].PostFlag  = false;
                        #endregion

                        if ((x % 2 == 0 && (tmpCurMins - tmpPrevMins) >= Globals.POCKETTIME))
                        {
                            if (idxMultIN == 0)
                                idxMultIN = x;
                            else
                            {
                                int tmpNextMins = ArrDTR[((x+1) < idxOUT ? (x+1) : idxOUT)].LogTimeMins;
                                if (tmpNextMins != 0 && (tmpNextMins - tmpCurMins) >= Globals.POCKETGAP)
                                {
                                    idxMultOUT = x;
                                    isPaired = true;
                                }
                            }
                        }
                        else if (x % 2 != 0 && (tmpCurMins - tmpPrevMins) >= Globals.POCKETGAP)
                        {
                            if (idxMultIN == 0)
                                idxMultIN = x;
                            else
                            {
                                idxMultOUT = x;
                                isPaired = true;
                            }
                        }

                        if (isPaired)
                        {
                            bMulti = true;
                            ArrDTR[idxMultIN].PostFlag  = true;
                            ArrDTR[idxMultIN].PocketNo  = ArrDTR[idxLastPocket].PocketNo + 1;
                            ArrDTR[idxMultIN].Edited    = true;

                            ArrDTR[idxMultOUT].PostFlag = true;
                            ArrDTR[idxMultOUT].PocketNo = ArrDTR[idxMultIN].PocketNo + 1;
                            ArrDTR[idxMultOUT].Edited   = true;

                            ArrDTR[idxOUT].PocketNo     = ArrDTR[idxMultOUT].PocketNo + 1;
                            for (int xx = (idxOUT + 1); xx < ArrDTR.Count(); xx++)
                            {
                                if (ArrDTR[xx].PocketNo != 0)
                                    ArrDTR[xx].PocketNo += 2;
                            }
                            isPaired        = false;
                            idxLastPocket   = idxMultOUT;
                            idxMultIN       = 0;
                            idxMultOUT      = 0;
                        }
                    }
                }
            }
            return bMulti;
        }

        private void UpdateLogControlTable(string DBName, DateTime dtProcessDate, string LogLedgerTable, DALHelper dal)
        {
            #region Query to Force Repost Logs
            string strRepostLogs = "";
            if (Globals.isOverwrite == false)
            {
                strRepostLogs = string.Format(@"WHERE LEFT(Tlc_Day{0}, 1) != 'F'", dtProcessDate.ToString("dd"));
            }
            else 
            {
                strRepostLogs = @""; //No condition, so that it will update the log control status of all employees
            }
            #endregion

            #region Query
            //string strQuery = string.Format(@"
            //UPDATE L
            //SET Tlc_Day{2} = LogStatus
            //    , ludatetime = GETDATE()
            //FROM {0}..T_EmpLogControl L
            //INNER JOIN (
            //	SELECT Ttr_IDNo
            //		, Ttr_Date
            //		, Ttr_ActIn_1
            //		, Ttr_ActOut_1
            //		, Ttr_ActIn_2
            //		, Ttr_ActOut_2
            //		, CASE WHEN (Ttr_ActIn_1 != '0000' AND Ttr_ActOut_2 != '0000')
            //					OR (Ttr_ActIn_1 != '0000' AND Ttr_ActOut_1 != '0000' 
            //						AND Ttr_ActIn_2 != '0000' AND Ttr_ActOut_2 != '0000')
            //				THEN 'F,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114)
            //				WHEN Ttr_ActIn_1 + Ttr_ActOut_1 + Ttr_ActIn_2 + Ttr_ActOut_2 != '0000000000000000'
            //				THEN 'P,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114)
            //				ELSE 'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114)
            //				END AS LogStatus
            //	FROM {0}..{5}
            //	WHERE Ttr_Date = '{3}'
            //) TEMP
            //ON Tlc_IDNo = Ttr_IDNo
            //	AND Tlc_YearMonth = '{1}'
            //{4}", DBName, dtProcessDate.ToString("yyyyMM"), dtProcessDate.ToString("dd"), 
            //dtProcessDate.ToShortDateString(), strRepostLogs, LogLedgerTable);

            string strQuery = string.Format(@"
            UPDATE L
            SET Tlc_Day{2} = LogStatus
                , Tlc_UpdatedDate = GETDATE()
            FROM {6}..T_EmpLogControl L
            INNER JOIN (
            	SELECT Ttr_IDNo
            		, Ttr_Date
            		, Ttr_ActIn_1
            		, Ttr_ActOut_1
            		, Ttr_ActIn_2
            		, Ttr_ActOut_2
            		, CASE WHEN ((Ttr_ActIn_1= '0000'	 and Ttr_ActOut_1= '0000'	 and Ttr_ActIn_2= '0000'	and Ttr_ActOut_2<> '0000') 
								OR (Ttr_ActIn_1= '0000'	 and Ttr_ActOut_1<> '0000'	 and Ttr_ActIn_2<> '0000'	and Ttr_ActOut_2<> '0000') 
								OR  (Ttr_ActIn_1= '0000'	 and Ttr_ActOut_1<> '0000'	 and Ttr_ActIn_2= '0000'	and Ttr_ActOut_2= '0000')  
								OR  (Ttr_ActIn_1= '0000'	 and Ttr_ActOut_1<> '0000'	 and Ttr_ActIn_2= '0000'	and Ttr_ActOut_2<> '0000') 
								OR  (Ttr_ActIn_1= '0000'	 and Ttr_ActOut_1<> '0000'	 and Ttr_ActIn_2<> '0000'	and Ttr_ActOut_2= '0000')  
								OR  (Ttr_ActIn_1<> '0000'	 and Ttr_ActOut_1<> '0000'	 and Ttr_ActIn_2= '0000'	and Ttr_ActOut_2<> '0000') 
								OR  (Ttr_ActIn_1<> '0000'	 and Ttr_ActOut_1= '0000'	 and Ttr_ActIn_2= '0000'	and Ttr_ActOut_2= '0000') 
								OR  (Ttr_ActIn_1<> '0000'	 and Ttr_ActOut_1= '0000'	 and Ttr_ActIn_2<> '0000'	and Ttr_ActOut_2<> '0000') 
								OR  (Ttr_ActIn_1<> '0000'	 and Ttr_ActOut_1= '0000'	 and Ttr_ActIn_2<> '0000'	and Ttr_ActOut_2= '0000')  
								OR  (Ttr_ActIn_1= '0000'	 and Ttr_ActOut_1= '0000'	 and Ttr_ActIn_2<> '0000'	and Ttr_ActOut_2= '0000') 
								OR  (Ttr_ActIn_1<> '0000'	 and Ttr_ActOut_1<> '0000'	 and Ttr_ActIn_2<> '0000'	and Ttr_ActOut_2= '0000')) 
								THEN 'P,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114)
								WHEN (Ttr_ActIn_1 != '0000' AND Ttr_ActOut_2 != '0000')
					                OR (Ttr_ActIn_1 != '0000' AND Ttr_ActOut_1 != '0000' 
						                AND Ttr_ActIn_2 != '0000' AND Ttr_ActOut_2 != '0000')
				                THEN 'F,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114)
							    ELSE 'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114)
				                END AS LogStatus
            	    FROM {0}..{5}
            	    WHERE Ttr_Date = '{3}'
            ) TEMP
            ON Tlc_IDNo = Ttr_IDNo
            	AND Tlc_YearMonth = '{1}'
            {4}", DBName, dtProcessDate.ToString("yyyyMM"), dtProcessDate.ToString("dd"),
            dtProcessDate.ToShortDateString(), strRepostLogs, LogLedgerTable, Globals.CentralProfile);
            #endregion

            dal.ExecuteNonQuery(strQuery);
        }

        public string GetHourStrFromMins(int minutes)
        {
            int iHours, iMinutes;
            string strHours, strMinutes;

            iHours = minutes / 60;
            strHours = iHours.ToString();
            iMinutes = minutes % 60;
            strMinutes = iMinutes.ToString();

            // Pad left zeros
            if (strHours.Length < 2)
            {
                strHours = "0" + strHours;
            }
            if (strMinutes.Length < 2)
            {
                strMinutes = "0" + strMinutes;
            }

            // Concatenate hour and minutes
            return strHours + strMinutes;
        }

        public int GetMinsFromHourStr(string hour)
        {
            if (hour.Length == 4)
                return (Convert.ToInt32(hour.Substring(0, 2)) * 60) + Convert.ToInt32(hour.Substring(2, 2));
            else
                return 0;
        }
        #endregion
    }
}
