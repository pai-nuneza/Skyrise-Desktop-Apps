using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using CommonPostingLibrary;
using System.Collections;
using Posting.DAL;

namespace Posting.BLogic
{
    public class NewLaborHoursGenerationBL2 : BaseBL
    {
        #region <Global Variables>
        DALHelper dal;
        SystemCycleProcessingBL2 SystemCycleProcessingBL;
        CommonBL commonBL;

        //Storage tables
        DataTable dtEmployeeLogLedger = null;
        DataTable dtEmployeeLogLedgerExt = null;
        DataTable dtEmployeePayrollTransaction = null;
        DataTable dtEmployeePayrollTransactionExt = null;
        DataTable dtEmployeePayrollTransactionDetail = null;
        DataTable dtEmployeePayrollTransactionExtDetail = null;
        DataRow drEmployeePayrollTransaction = null;
        DataRow drEmployeePayrollTransactionExt = null;
        DataRow drEmployeePayrollTransactionDetail = null;
        DataRow drEmployeePayrollTransactionExtDetail = null;
        DataRow drEmployeePayrollTransactionDetailPrev = null;
        DataTable dtOvertimeTable = null;
        DataTable dtLeaveTable = null;
        DataTable dtUnpaidLeaveTable = null;
        DataTable dtOffsetTable = null;
        DataTable dtDayCodeMaster = null;
        DataTable dtDayCodeFillers = null;
        DataTable dtDefaultShift = null;
        DataTable dtNextDayCodeLastDay = null;

        //Temporary variable for the Hours
        double AbsentHr = 0.0;
        double RegularHr = 0.0;
        double RegularOTHr = 0.0;
        double RegularNDHr = 0.0;
        double RegularOTNDHr = 0.0;
        double RestdayHr = 0.0;
        double RestdayOTHr = 0.0;
        double RestdayNDHr = 0.0;
        double RestdayOTNDHr = 0.0;
        double LegalHolidayHr = 0.0;
        double LegalHolidayOTHr = 0.0;
        double LegalHolidayNDHr = 0.0;
        double LegalHolidayOTNDHr = 0.0;
        double SpecialHolidayHr = 0.0;
        double SpecialHolidayOTHr = 0.0;
        double SpecialHolidayNDHr = 0.0;
        double SpecialHolidayOTNDHr = 0.0;
        double PlantShutdownHr = 0.0;
        double PlantShutdownOTHr = 0.0;
        double PlantShutdownNDHr = 0.0;
        double PlantShutdownOTNDHr = 0.0;
        double CompanyHolidayHr = 0.0;
        double CompanyHolidayOTHr = 0.0;
        double CompanyHolidayNDHr = 0.0;
        double CompanyHolidayOTNDHr = 0.0;
        double RestdayLegalHolidayHr = 0.0;
        double RestdayLegalHolidayOTHr = 0.0;
        double RestdayLegalHolidayNDHr = 0.0;
        double RestdayLegalHolidayOTNDHr = 0.0;
        double RestdaySpecialHolidayHr = 0.0;
        double RestdaySpecialHolidayOTHr = 0.0;
        double RestdaySpecialHolidayNDHr = 0.0;
        double RestdaySpecialHolidayOTNDHr = 0.0;
        double RestdayCompanyHolidayHr = 0.0;
        double RestdayCompanyHolidayOTHr = 0.0;
        double RestdayCompanyHolidayNDHr = 0.0;
        double RestdayCompanyHolidayOTNDHr = 0.0;
        double RestdayPlantShutdownHr = 0.0;
        double RestdayPlantShutdownOTHr = 0.0;
        double RestdayPlantShutdownNDHr = 0.0;
        double RestdayPlantShutdownOTNDHr = 0.0;
        int RestdayLegalHolidayCount = 0;
        int WorkingDay = 0;
        double RegularHrMonthlyDailyPay = 0.0;
        double LateHours = 0.0;
        double UndertimeHours = 0.0;
        double PaidLeaveHours = 0.0;
        double UnpaidLeaveHours = 0.0;
        double WholeDayAbsentHours = 0.0;
        double AbsentLegalHolidayHr = 0.0;
        double AbsentSpecialHolidayHr = 0.0;
        double AbsentCompanyHolidayHr = 0.0;
        double AbsentPlantShutdownHr = 0.0;
        double AbsentFillerHolidayHr = 0.0;
        double PaidLegalHolidayHr = 0.0;
        double PaidSpecialHolidayHr = 0.0;
        double PaidCompanyHolidayHr = 0.0;
        double PaidFillerHolidayHr = 0.0;

        //Flags and parameters
        public string OTOFSETABS = string.Empty;
        public string ONEPREVDAY = string.Empty;
        public string REGPREVDAY = string.Empty;
        public string LVHRENTRY = string.Empty;
        public string NOABSNWHRE = string.Empty;
        public string OTFORMGR = string.Empty;
        public string MULTSAL = string.Empty;
        public string ALWPOST = string.Empty;
        public string PSDMONTHLY = string.Empty;
        public string NONDOTDAY = string.Empty;
        public string NDPREM1ST8 = string.Empty;
        public string NDSPLTSHFT = string.Empty;
        public string NDCNTBREAK = string.Empty;
        public string HRFRCLBRHR = string.Empty;
        public string EXTREGLVE = string.Empty;
        public string EXTREGULVE = string.Empty;
        public string ATLVEADJ = string.Empty;
        public string OBCOMPOT = string.Empty;
        public string NDREGSHIFT = string.Empty;
        public string MLPAYHOL = string.Empty;
        public string OTROUNDING = string.Empty;
        public string LEGHOLINRG = string.Empty;
        public string CNTPDBRK = string.Empty;
        public string HOURFRACFORMULA1 = string.Empty;
        public string HOURFRACFORMULA2 = string.Empty;
        public string RNDOTFRAC = string.Empty;
        public string FLEXSHIFT = string.Empty;

        public double MINOTHR = 0;
        public double MINOTHR_ORIG = 0;
        public double OTFRACT = 0;
        public double MAXLATEMIN = 0;
        public double MAXUTMIN = 0;
        public double NDFRACTION = 0;
        public int ABSFRACTRG = 0;
        public int ABSFRACT = 0;
        public int TIMEFRAC = 0;
        public double LATECHARGE = 0;
        public double MDIVISOR = 0;
        public int LOGPAD = 0;
        public bool MIDOT = false;
        public int POCKETGAP = 0;
        public int POCKETTIME = 0;
        public int POCKETSIZE = 0;
        public int NDBRCKTCNT = 1;
        public int NP1_BEGTIME = 0;
        public int NP1_ENDTIME = 0;
        public int NP2_BEGTIME = 0;
        public int NP2_ENDTIME = 0;

        public string LATECHARGEFREQUENCY = "";
        public DataTable LATEBRACKETDEDUCTION = null;
        public DataTable UNDERTIMEBRACKETDEDUCTION = null;
        public DataTable REGHRSREQD = null;
        public DataTable MINOTHR_TBL = null;
        public DataTable ULPREVDAY = null;

        public double OTLIMITHR = 0;
        public double OTLIMITHR_ORIG = 0;
        public DataTable OTLIMITHR_TBL = null;
        public double OTLIMITEQV = 0;
        public double OTLIMITEQV_ORIG = 0;
        public DataTable OTLIMITEQV_TBL = null;
        public double OTLIMITAPP = 0;

        //Constants
        public int GRAVEYARD24 = 24 * 60;
        public int NIGHTDIFFGRAVEYARDSTART = 22 * 60;
        public int NIGHTDIFFGRAVEYARDEND = 30 * 60;
        public int NIGHTDIFFAMSTART = 0 * 60;
        public int NIGHTDIFFAMEND = 6 * 60;
        public int FILLERCNT = 6;

        //Labor Hour Report structure
        struct structLaborHourErrorReport
        {
            public string strEmployeeId;
            public string strLastName;
            public string strFirstName;
            public string strMiddleName;
            public string strProcessDate;
            public string strRemarks;

            public structLaborHourErrorReport(string EmployeeId, string LastName, string FirstName, string MiddleName, string ProcessDate, string Remarks)
            {
                strEmployeeId = EmployeeId;
                strLastName = LastName;
                strFirstName = FirstName;
                strMiddleName = MiddleName;
                strProcessDate = ProcessDate;
                strRemarks = Remarks;
            }
        }
        List<structLaborHourErrorReport> listLbrHrRept = new List<structLaborHourErrorReport>();

        //Hour Fraction structure
        public enum HourType { RegHour, OTHour, NDHour, NDOTHour };
        struct structHourFract
        {
            public int iStartMin;
            public int iEndMin;
            public int iCurrentDayMin;
            public int iNextDayMin;
            public HourType strHourType;

            public structHourFract(int StartMin, int EndMin, int CurrentDayMin, int NextDayMin, HourType HourType)
            {
                iStartMin = StartMin;
                iEndMin = EndMin;
                iCurrentDayMin = CurrentDayMin;
                iNextDayMin = NextDayMin;
                strHourType = HourType;
            }
        }
        List<structHourFract> listHourFract = new List<structHourFract>();

        //Index counters
        int indexPayTrans = -1;
        int indexPayTransExt = -1;

        //Hour Fraction
        int Cutoff = 1440;
        int CurrentDayMin, NextDayMin;
        int ActualStart, ActualEnd;
        int ActualStart2, ActualEnd2;
        int OTMins;

        //Miscellaneous
        string ProcessPayrollPeriod = "";
        string PayrollStart = "";
        string PayrollEnd = "";
        string LoginUser = "";
        string EmployeeLogLedgerTable = "T_EmployeeLogLedger";
        string EmployeeLogLedgerExtTable = "T_EmployeeLogLedgerExt";
        string EmployeePayrollTransactionTable = "T_EmployeePayrollTransaction";
        bool bHasDayCodeExt = false;
        bool bProcessTrail = false;
        string AdjustPayrollPeriod = "";
        string EmployeeList = "";
        bool bHasAddedCurrentHrs = false;
        private string MainDB = "";
        #endregion

        #region <Override Functions>

        public override int Add(System.Data.DataRow row)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override int Update(System.Data.DataRow row)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override int Delete(string code, string userLogin)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override System.Data.DataSet FetchAll()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override System.Data.DataRow Fetch(string code)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region Event handlers for labor hours generation process
        public delegate void EmpDispEventHandler(object sender, EmpDispEventArgs e);
        public class EmpDispEventArgs : System.EventArgs
        {
            public string EmployeeId;
            public string LastName;
            public string FirstName;
            public string StatusMsg;

            public EmpDispEventArgs(string strEmployeeId, string strLast, string strFirst)
            {
                EmployeeId = strEmployeeId;
                LastName = strLast;
                FirstName = strFirst;
                StatusMsg = "Successful";
            }

            public EmpDispEventArgs(string strEmployeeId, string strLast, string strFirst, string strStatusMsg)
            {
                EmployeeId = strEmployeeId;
                LastName = strLast;
                FirstName = strFirst;
                StatusMsg = strStatusMsg;
            }
        }
        public event EmpDispEventHandler EmpDispHandler;

        public delegate void StatusEventHandler(object sender, StatusEventArgs e);
        public class StatusEventArgs : System.EventArgs
        {
            public string Status;
            public bool Done;

            public StatusEventArgs(string strStat, bool bDone)
            {
                Status = strStat;
                Done = bDone;
            }
        }
        public event StatusEventHandler StatusHandler;
        #endregion

        #region Main Functions
        public DataTable GenerateLaborHours(bool ProcessAll, bool ProcessCurrentPeriod, bool ProcessSeparated, string PayrollPeriod, string AdjustPayPeriod, bool ProcessTrail, string EmployeeId, string EmpList, string UserLogin, DALHelper dalHelper, string MainDB)
        {
            this.MainDB = MainDB;
            return GenerateLaborHours(ProcessAll, ProcessCurrentPeriod, ProcessSeparated, PayrollPeriod, AdjustPayPeriod, ProcessTrail, EmployeeId, EmpList, UserLogin, dalHelper);
        }

        public DataTable GenerateLaborHours(bool ProcessAll, bool ProcessCurrentPeriod, bool ProcessSeparated, string PayrollPeriod, string AdjustPayPeriod, bool ProcessTrail, string EmployeeId, string EmpList, string UserLogin, DALHelper dalHelper)
        {
            AdjustPayrollPeriod = AdjustPayPeriod;
            bProcessTrail = ProcessTrail;
            EmployeeList = EmpList;

            return GenerateLaborHours(ProcessAll, ProcessCurrentPeriod, ProcessSeparated, PayrollPeriod, EmployeeId, UserLogin, dalHelper);
        }

        public DataTable GenerateLaborHours(bool ProcessAll, bool ProcessCurrentPeriod, bool ProcessSeparated, string PayrollPeriod, string EmployeeId, string UserLogin, DALHelper dalHelper)
        {
            #region Variables
            string prevEmployeeID = "", curEmployeeID = "";
            string shiftCode = "";

            DataTable dtUserGeneratedPayTrans = null;
            DataRow[] drArrUserGeneratedPayTrans = null;
            bool bUserGeneratedPayTrans = false;

            DataTable dtHolidays = null;
            int iDecrement;
            bool bIsFound;
            int iHolPrevDayInMin;
            DataRow drHol = null;
            DataRow[] drArrPrevDay = null;
            DataRow drPrevDay = null;
            int iPrevCompDayWorkMin;
            string strPrevDayCode;
            int iPaidHolidayHrs;
            bool bPrevRestDay;
            int iSundayHolidayCount;
            int iShiftInHours;
            bool bIsNewHireOrResigned;
            bool bMetHolidayPreviousDay;
            DataRow[] drArrDayCode = null;
            DataRow[] drArrDayCodeFiller = null;

            string strDayCode;
            string strProcessDate;
            bool bIsRestDay, bIsHoliday;
            bool bApplyLateChargeQuincena;
            bool bIsGraveyard;
            bool bIsOutsideShift;
            bool bIsRegOrReg5DayCode;
            bool bOverrideGraveyardConv;
            bool bDailiesNoWorkNoPay;

            int iActualTimeIn1Min, iActualTimeOut1Min, iActualTimeIn2Min, iActualTimeOut2Min;
            int iConvTimeIn1Min, iConvTimeOut1Min, iConvTimeIn2Min, iConvTimeOut2Min;
            int iCompTimeIn1Min, iCompTimeOut1Min, iCompTimeIn2Min, iCompTimeOut2Min;
            int iShiftTimeIn1Min, iShiftTimeOut1Min, iShiftTimeIn2Min, iShiftTimeOut2Min;
            int iMasterShiftTimeIn1Min, iMasterShiftTimeOut1Min, iMasterShiftTimeIn2Min, iMasterShiftTimeOut2Min;
            string strMasterShiftTimeIn1, strMasterShiftTimeOut1, strMasterShiftTimeIn2, strMasterShiftTimeOut2;

            int iAbsFraction;
            int iComputedDayWorkMin;
            int iComputedRegularMin;
            int iShiftMin;
            int iInitialAbsentMin;
            int iComputedAbsentMin;
            int iTotalComputedAbsentMin;
            int iOffsetOvertimeMin;
            int iPayLeaveMin;
            int iOrigPayLeaveMin;
            int iNoPayLeaveMin;
            int iExcessLeaveMin;
            int iLeaveMinToBeAddedToReg;
            int iLeaveMinOnPaidBreak;
            string strPayType;
            string strPayLeaveType;
            string strScheduleType = "";
            int iComputedOvertimeMin;
            int iAdvOTMin;
            int iAdjShiftMin; //Advanced OT column
            int iPaidBreak;
            bool bDonePaidBreakAdd;
            int iMasterPaidBreak;
            int iNDFraction;
            int iCompRegNightPremMin;
            int iCompOvertimeNightPremMin;
            int iTimeMinTemp;
            int iTimeMinTemp2;
            int iTimeMinTemp3;
            int iTimeMinTemp4;
            int iOTFraction;
            int iOTFractionZero;
            int iOTTemp;
            int iOTTemp2;
            int iBreakMin;
            int iEncodedOvertimeMin;
            bool bIsOutsideShiftComputedOT;
            int iNDSum;
            int iComputedLateMin;
            int iComputedLate2Min;
            int iComputedUndertime1Min;
            int iComputedUndertime2Min;
            bool bIsNewHire = false;
            string strPreviousDayReference;
            string strPreviousDayLeaveType;
            string strHireDate;
            DataTable dtNewHire = null;
            DataTable dtLeaveType = null;
            DataRow[] drArrLeaveType;

            DataRow[] drArrOTApp; //Temporary
            DataRow[] drArrOTApproved;
            int iOTStartMin;
            int iOTEndMin;
            string strOTType;
            bool bCountOTFraction;

            DataRow[] drArrLeaveAppPaid;
            DataRow[] drArrLeaveAppUnpaid;

            //DataRow[] drArrOffsetApp;
            //int iOffsetStartMin;
            //int iOffsetEndMin;
            //int iOffsetAmMins;
            //int iOffsetPmMins;
            //int iExcessOffset;
            //int iForOffsetMin;
            //int iAccumulatedMins;

            int iActualTimeIn1MinOrig;
            int iActualTimeOut2MinOrig;
            int iActualLate1Mins;
            int iActualLate2Mins;
            int iActualUT1Mins;
            int iActualUT2Mins;
            int iPayLeaveMinsDummy;
            int iNoPayLeaveMinDummy;
            int iExcessLeaveMinDummy;
            int iLeaveMinToBeAddedToRegDummy;
            int iLeaveMinOnPaidBreakDummy;

            string fillerHrCol = "";
            string fillerOTHrCol = "";
            string fillerNDHrCol = "";
            string fillerOTNDHrCol = "";

            string previousPayPeriod = "";
            DataSet dsManagerLogLedger = null;
            DataTable dtEmpWithSalaryMovement = null;
            DataRow[] drArrEmpWithSalaryMovement = null;
            DataTable dtErrList = new DataTable();

            //Multiple Pocket Logic
            DataRow[] drArrEmployeeLogLedgerExt = null;
            int iActualTimeInExtMin = 0;
            int iActualTimeOutExtMin = 0;
            int iConvTimeInExtMin = 0;
            int iConvTimeOutExtMin = 0;
            int iPrevConvTimeOutExtMin = 0; //For POCKETGAP Checking
            int iTempMultPockVar = 0;
            #endregion

            try
            {
                #region Initial Setup
                //dal = new DALHelper();
                this.dal = dalHelper;
                SystemCycleProcessingBL = new SystemCycleProcessingBL2(dal, PayrollPeriod, UserLogin);
                commonBL = new CommonBL(MainDB);
                CommonBL.HOURSINDAY = CommonBL.GetHoursInDay();
                //-----------------------------
                //Check for Existing Day Codes
                dtDayCodeMaster = GetDayCodeMasterData();
                if (GetFillerDayCodesCount() > 0)
                {
                    bHasDayCodeExt = true;
                    dtDayCodeFillers = GetDayCodeFillers();
                }
                else
                {
                    bHasDayCodeExt = false;
                    dtDayCodeFillers = null;
                }
                dtDefaultShift = GetDefaultShift();
                //-----------------------------
                //Create and initialize payroll transaction tables
                #region Create payroll trans table
                DALHelper dalTemp = new DALHelper(MainDB, false);
                if (ProcessCurrentPeriod)
                {
                    if (ProcessSeparated)
                    {
                        EmployeePayrollTransactionTable = "T_EmployeePayrollTransactionSep";
                        dtEmployeePayrollTransaction = DbRecord.GenerateTable(CommonConstants.TableName.T_EmployeePayrollTransactionSep, dalTemp);
                        dtEmployeePayrollTransactionExt = DbRecord.GenerateTable(CommonConstants.TableName.T_EmployeePayrollTransactionSepExt, dalTemp);
                        dtEmployeePayrollTransactionDetail = DbRecord.GenerateTable(CommonConstants.TableName.T_EmployeePayrollTransactionSepDetail, dalTemp);
                        dtEmployeePayrollTransactionExtDetail = DbRecord.GenerateTable(CommonConstants.TableName.T_EmployeePayrollTransactionSepExtDetail, dalTemp);
                    }
                    else
                    {
                        EmployeePayrollTransactionTable = "T_EmployeePayrollTransaction";
                        dtEmployeePayrollTransaction = DbRecord.GenerateTable(CommonConstants.TableName.T_EmployeePayrollTransaction, dalTemp);
                        dtEmployeePayrollTransactionExt = DbRecord.GenerateTable(CommonConstants.TableName.T_EmployeePayrollTransactionExt, dalTemp);
                        dtEmployeePayrollTransactionDetail = DbRecord.GenerateTable(CommonConstants.TableName.T_EmployeePayrollTransactionDetail, dalTemp);
                        dtEmployeePayrollTransactionExtDetail = DbRecord.GenerateTable(CommonConstants.TableName.T_EmployeePayrollTransactionExtDetail, dalTemp);
                    }
                }
                else
                {
                    if (!ProcessSeparated)
                    {
                        EmployeePayrollTransactionTable = "T_EmployeePayrollTransactionHist";
                        dtEmployeePayrollTransaction = DbRecord.GenerateTable(CommonConstants.TableName.T_EmployeePayrollTransactionHist, dalTemp);
                        dtEmployeePayrollTransactionExt = DbRecord.GenerateTable(CommonConstants.TableName.T_EmployeePayrollTransactionHistExt, dalTemp);
                        dtEmployeePayrollTransactionDetail = DbRecord.GenerateTable(CommonConstants.TableName.T_EmployeePayrollTransactionHistDetail, dalTemp);
                        dtEmployeePayrollTransactionExtDetail = DbRecord.GenerateTable(CommonConstants.TableName.T_EmployeePayrollTransactionHistExtDetail, dalTemp);
                    }
                    else
                    {
                        EmployeePayrollTransactionTable = "T_EmployeePayrollTransactionSep";
                        dtEmployeePayrollTransaction = DbRecord.GenerateTable(CommonConstants.TableName.T_EmployeePayrollTransactionSep, dalTemp);
                        dtEmployeePayrollTransactionExt = DbRecord.GenerateTable(CommonConstants.TableName.T_EmployeePayrollTransactionSepExt, dalTemp);
                        dtEmployeePayrollTransactionDetail = DbRecord.GenerateTable(CommonConstants.TableName.T_EmployeePayrollTransactionSepDetail, dalTemp);
                        dtEmployeePayrollTransactionExtDetail = DbRecord.GenerateTable(CommonConstants.TableName.T_EmployeePayrollTransactionSepExtDetail, dalTemp);
                    }

                    if (bProcessTrail)
                    {
                        EmployeePayrollTransactionTable = "T_EmployeePayrollTransactionTrail";
                        dtEmployeePayrollTransaction = DbRecord.GenerateTable(CommonConstants.TableName.T_EmployeePayrollTransactionTrail, dalTemp);
                        dtEmployeePayrollTransactionExt = DbRecord.GenerateTable(CommonConstants.TableName.T_EmployeePayrollTransactionTrailExt, dalTemp);
                        dtEmployeePayrollTransactionDetail = DbRecord.GenerateTable(CommonConstants.TableName.T_EmployeePayrollTransactionTrailDetail, dalTemp);
                        dtEmployeePayrollTransactionExtDetail = DbRecord.GenerateTable(CommonConstants.TableName.T_EmployeePayrollTransactionTrailExtDetail, dalTemp);
                    }
                }
                indexPayTrans = -1;
                indexPayTransExt = -1;
                #endregion
                #region Initialize payroll trans row
                drEmployeePayrollTransaction = dtEmployeePayrollTransaction.NewRow();
                if (bHasDayCodeExt)
                    drEmployeePayrollTransactionExt = dtEmployeePayrollTransactionExt.NewRow();

                drEmployeePayrollTransactionDetailPrev = null;
                drEmployeePayrollTransactionDetail = dtEmployeePayrollTransactionDetail.NewRow();
                if (bHasDayCodeExt)
                    drEmployeePayrollTransactionExtDetail = dtEmployeePayrollTransactionExtDetail.NewRow();
                #region Initialize payroll trans detail hours
                drEmployeePayrollTransactionDetail["Ept_RegularHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_RegularOTHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_RegularNDHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_RegularOTNDHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_RestdayHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_RestdayOTHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_RestdayNDHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_RestdayOTNDHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_LegalHolidayHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_LegalHolidayOTHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_LegalHolidayNDHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_LegalHolidayOTNDHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_SpecialHolidayHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_SpecialHolidayOTHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_SpecialHolidayNDHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_SpecialHolidayOTNDHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_PlantShutdownHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_PlantShutdownOTHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_PlantShutdownNDHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_PlantShutdownOTNDHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_CompanyHolidayHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_CompanyHolidayOTHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_CompanyHolidayNDHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_CompanyHolidayOTNDHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayOTHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayNDHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayOTNDHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayOTHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayNDHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayOTNDHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayOTHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayNDHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayOTNDHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownOTHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownNDHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownOTNDHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_LateHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_UndertimeHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_UnpaidLeaveHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_WholeDayAbsentHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_AbsentLegalHolidayHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_AbsentSpecialHolidayHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_AbsentCompanyHolidayHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_AbsentPlantShutdownHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_AbsentFillerHolidayHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_PaidLeaveHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_PaidLegalHolidayHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_PaidSpecialHolidayHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_PaidCompanyHolidayHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_PaidFillerHolidayHr"] = 0;
                drEmployeePayrollTransactionDetail["Ept_OvertimeAdjustmentAmt"] = 0;
                #endregion
                #region Initialize payroll trans detail ext hours
                if (bHasDayCodeExt)
                {
                    for (int i = 1; i <= FILLERCNT; i++)
                    {
                        //initialize
                        fillerHrCol = string.Format("Ept_Filler{0:00}_Hr", i);
                        fillerOTHrCol = string.Format("Ept_Filler{0:00}_OTHr", i);
                        fillerNDHrCol = string.Format("Ept_Filler{0:00}_NDHr", i);
                        fillerOTNDHrCol = string.Format("Ept_Filler{0:00}_OTNDHr", i);
                        drEmployeePayrollTransactionExt[fillerHrCol] = 0;
                        drEmployeePayrollTransactionExt[fillerOTHrCol] = 0;
                        drEmployeePayrollTransactionExt[fillerNDHrCol] = 0;
                        drEmployeePayrollTransactionExt[fillerOTNDHrCol] = 0;
                        drEmployeePayrollTransactionExtDetail[fillerHrCol] = 0;
                        drEmployeePayrollTransactionExtDetail[fillerOTHrCol] = 0;
                        drEmployeePayrollTransactionExtDetail[fillerNDHrCol] = 0;
                        drEmployeePayrollTransactionExtDetail[fillerOTNDHrCol] = 0;
                    }
                }
                #endregion
                #endregion
                //-----------------------------
                //dal.OpenDB();
                //dal.BeginTransactionSnapshot();
                //code start
                //-----------------------------
                if (!ProcessCurrentPeriod)
                {
                    EmployeeLogLedgerTable = "T_EmployeeLogLedgerHist";
                    EmployeeLogLedgerExtTable = "T_EmployeeLogLedgerExtHist";
                    if (bProcessTrail)
                        EmployeeLogLedgerTable = "T_EmployeeLogLedgerTrail";
                }
                else
                {
                    EmployeeLogLedgerTable = "T_EmployeeLogLedger";
                    EmployeeLogLedgerExtTable = "T_EmployeeLogLedgerExt";
                }
                ProcessPayrollPeriod = PayrollPeriod;
                LoginUser = UserLogin;

                DataTable dtPayPeriod = GetPayPeriodCycle(ProcessPayrollPeriod);
                if (dtPayPeriod.Rows.Count > 0)
                {
                    PayrollStart = dtPayPeriod.Rows[0][1].ToString();
                    PayrollEnd = dtPayPeriod.Rows[0][2].ToString();
                }
                //-----------------------------
                SetProcessFlags();
                InitializeLaborHourReport();
                //-----------------------------
                //No OT for managers during first quincena
                if (!Convert.ToBoolean(OTFORMGR))
                    previousPayPeriod = GetPrevPayPeriod(ProcessPayrollPeriod, dal);
                //-----------------------------
                StatusHandler(this, new StatusEventArgs("Execute Pre-Labor Hours Generation Procedures", false));
                string strFormulaQuery = GetPreLaborHoursFormula();
                if (strFormulaQuery != "")
                {
                    if (!ProcessAll && EmployeeId != "")
                        dal.ExecuteNonQuery(strFormulaQuery.Replace("@PayPeriod", "'" + ProcessPayrollPeriod + "'").Replace("@EmployeeID", "'" + EmployeeId + "'").Replace("@EmployeeList", "''").Replace("@IsEmployeeList", (EmployeeList != "") ? "1" : "0"));
                    else if (ProcessAll == true && EmployeeList != "")
                        dal.ExecuteNonQuery(strFormulaQuery.Replace("@PayPeriod", "'" + ProcessPayrollPeriod + "'").Replace("@EmployeeID", "''").Replace("@EmployeeList", EmployeeList).Replace("@IsEmployeeList", (EmployeeList != "") ? "1" : "0"));
                    else if (ProcessAll == true)
                        dal.ExecuteNonQuery(strFormulaQuery.Replace("@PayPeriod", "'" + ProcessPayrollPeriod + "'").Replace("@EmployeeID", "''").Replace("@EmployeeList", "''").Replace("@IsEmployeeList", (EmployeeList != "") ? "1" : "0"));
                }
                StatusHandler(this, new StatusEventArgs("Execute Pre-Labor Hours Generation Procedures", true));
                //-----------------------------
                //Prepare master data for processing
                dtUserGeneratedPayTrans = GetUserGeneratedPayrollTransactionRecords(ProcessAll, EmployeeId);
                StatusHandler(this, new StatusEventArgs("Clean Transaction Tables and Log Ledger Records", false));
                if (ProcessCurrentPeriod || ProcessSeparated)
                {
                    ClearTransactionTables(ProcessAll, EmployeeId);
                    CleanUpBeforeGeneration(ProcessAll, EmployeeId);
                }
                else if (!ProcessCurrentPeriod && !bProcessTrail)
                {
                    ClearTransactionHistTables(ProcessAll, EmployeeId, PayrollPeriod);
                }
                StatusHandler(this, new StatusEventArgs("Clean Transaction Tables and Log Ledger Records", true));

                StatusHandler(this, new StatusEventArgs("Get All Log Ledger Records", false));
                dtEmployeeLogLedger = GetAllEmployeeForProcess(ProcessAll, EmployeeId, Convert.ToBoolean(FLEXSHIFT));
                dtEmployeeLogLedgerExt = GetLogLedgerExtensionRecords(ProcessAll, EmployeeId);
                StatusHandler(this, new StatusEventArgs("Get All Log Ledger Records", true));

                if (!bProcessTrail)
                {
                    StatusHandler(this, new StatusEventArgs("Get All Overtime and Leave Records", false));
                    dtOvertimeTable = GetAllOvertimeRecords(ProcessAll, EmployeeId, Convert.ToBoolean(FLEXSHIFT));
                    dtLeaveTable = GetAllLeaveAvailmentRecords(ProcessAll, EmployeeId);
                    dtUnpaidLeaveTable = GetAllUnpaidLeaveAvailmentRecords(ProcessAll, EmployeeId);
                    //if (Convert.ToBoolean(TKOFFSET))
                    //    dtOffsetTable = GetAllOffsetApplications(ProcessAll, EmployeeId);
                    StatusHandler(this, new StatusEventArgs("Get All Overtime and Leave Records", true));

                    StatusHandler(this, new StatusEventArgs("Get All Holiday Records", false));
                    dtHolidays = GetAllHolidaysForCurrentPeriod();
                    StatusHandler(this, new StatusEventArgs("Get All Holiday Records", true));

                    dtNewHire = GetNewHireInCurrentPayPeriod(ProcessAll, EmployeeId);
                    dtLeaveType = GetLeaveTypes();
                }
                #endregion
                //-----------------------------START MAIN PROCESS
                if (dtEmployeeLogLedger.Rows.Count > 0)
                {
                    //Initialize some variables
                    bApplyLateChargeQuincena = false;
                    dtEmpWithSalaryMovement = GetEmployeesWithSalaryMovement(ProcessAll, EmployeeId);
                    if (Convert.ToBoolean(HRFRCLBRHR))
                        dtNextDayCodeLastDay = GetNextDayCodeForLastDay(Convert.ToDateTime(PayrollEnd), ProcessAll, EmployeeId);

                    for (int i = 0; i < dtEmployeeLogLedger.Rows.Count + 1; i++) //add extra loop to save last employee record
                    {
                        try
                        {
                            //checking of current employee processed
                            if (i < dtEmployeeLogLedger.Rows.Count)
                                curEmployeeID = dtEmployeeLogLedger.Rows[i]["Ell_EmployeeId"].ToString();
                            else
                                curEmployeeID = "";

                            #region Save to Payroll Transaction
                            if (curEmployeeID != prevEmployeeID)
                            {
                                //process previous employee record
                                if (indexPayTrans >= 0)
                                {
                                    EmpDispHandler(this, new EmpDispEventArgs(prevEmployeeID, dtEmployeeLogLedger.Rows[i - 1]["Emt_LastName"].ToString(), dtEmployeeLogLedger.Rows[i - 1]["Emt_FirstName"].ToString()));

                                    #region Check OT Hours Quota
                                    if (!Convert.ToBoolean(OTFORMGR))
                                    {
                                        if (GetApplicableHrsFromCommaDelimitedTable(OTLIMITHR_TBL
                                                                                    , dtEmployeeLogLedger.Rows[i - 1]["Emt_JobLevel"].ToString()
                                                                                    , dtEmployeeLogLedger.Rows[i - 1]["Emt_JobStatus"].ToString()
                                                                                    , dtEmployeeLogLedger.Rows[i - 1]["Emt_EmploymentStatus"].ToString()
                                                                                    , dtEmployeeLogLedger.Rows[i - 1]["Emt_PayrollType"].ToString()
                                                                                    , dtEmployeeLogLedger.Rows[i - 1]["Emt_PositionCode"].ToString()
                                                                                    , OTLIMITHR_ORIG
                                                                                    , ref OTLIMITHR) == true)
                                        {
                                            if (OTLIMITAPP == 2)
                                            {
                                                if (ProcessPayrollPeriod.Substring(6, 1).Equals("1"))
                                                {
                                                    #region Zero-out all overtime hours
                                                    RegularOTHr = 0;
                                                    RestdayHr = 0;
                                                    RestdayOTHr = 0;
                                                    LegalHolidayHr = 0;
                                                    LegalHolidayOTHr = 0;
                                                    SpecialHolidayHr = 0;
                                                    SpecialHolidayOTHr = 0;
                                                    PlantShutdownHr = 0;
                                                    PlantShutdownOTHr = 0;
                                                    CompanyHolidayHr = 0;
                                                    CompanyHolidayOTHr = 0;
                                                    RestdayLegalHolidayHr = 0;
                                                    RestdayLegalHolidayOTHr = 0;
                                                    RestdaySpecialHolidayHr = 0;
                                                    RestdaySpecialHolidayOTHr = 0;
                                                    RestdayCompanyHolidayHr = 0;
                                                    RestdayCompanyHolidayOTHr = 0;
                                                    RestdayPlantShutdownHr = 0;
                                                    RestdayPlantShutdownOTHr = 0;

                                                    #region Payroll Transaction Detail Update
                                                    if (!ProcessCurrentPeriod || Convert.ToBoolean(HRFRCLBRHR) == true || Convert.ToBoolean(MULTSAL) == true)
                                                    {
                                                        drArrOTApp = dtEmployeePayrollTransactionDetail.Select("Ept_EmployeeId = '" + prevEmployeeID + "'");
                                                        foreach (DataRow drRow in drArrOTApp)
                                                        {
                                                            drRow["Ept_RegularOTHr"] = 0;
                                                            drRow["Ept_RestdayHr"] = 0;
                                                            drRow["Ept_RestdayOTHr"] = 0;
                                                            drRow["Ept_LegalHolidayHr"] = 0;
                                                            drRow["Ept_LegalHolidayOTHr"] = 0;
                                                            drRow["Ept_SpecialHolidayHr"] = 0;
                                                            drRow["Ept_SpecialHolidayOTHr"] = 0;
                                                            drRow["Ept_PlantShutdownHr"] = 0;
                                                            drRow["Ept_PlantShutdownOTHr"] = 0;
                                                            drRow["Ept_CompanyHolidayHr"] = 0;
                                                            drRow["Ept_CompanyHolidayOTHr"] = 0;
                                                            drRow["Ept_RestdayLegalHolidayHr"] = 0;
                                                            drRow["Ept_RestdayLegalHolidayOTHr"] = 0;
                                                            drRow["Ept_RestdaySpecialHolidayHr"] = 0;
                                                            drRow["Ept_RestdaySpecialHolidayOTHr"] = 0;
                                                            drRow["Ept_RestdayCompanyHolidayHr"] = 0;
                                                            drRow["Ept_RestdayCompanyHolidayOTHr"] = 0;
                                                            drRow["Ept_RestdayPlantShutdownHr"] = 0;
                                                            drRow["Ept_RestdayPlantShutdownOTHr"] = 0;
                                                        }

                                                        if (bHasDayCodeExt)
                                                        {
                                                            drArrOTApp = dtEmployeePayrollTransactionExtDetail.Select("Ept_EmployeeId = '" + prevEmployeeID + "'");
                                                            foreach (DataRow drRow in drArrOTApp)
                                                            {
                                                                for (int k = 1; k <= FILLERCNT; k++)
                                                                {
                                                                    fillerHrCol = string.Format("Ept_Filler{0:00}_Hr", k);
                                                                    fillerOTHrCol = string.Format("Ept_Filler{0:00}_OTHr", k);
                                                                    drRow[fillerHrCol] = 0;
                                                                    drRow[fillerOTHrCol] = 0;
                                                                }
                                                            }
                                                        }
                                                    }
                                                    #endregion

                                                    if (bHasDayCodeExt)
                                                    {
                                                        for (int k = 1; k <= FILLERCNT; k++)
                                                        {
                                                            fillerHrCol = string.Format("Ept_Filler{0:00}_Hr", k);
                                                            fillerOTHrCol = string.Format("Ept_Filler{0:00}_OTHr", k);
                                                            drEmployeePayrollTransactionExt[fillerHrCol] = 0;
                                                            drEmployeePayrollTransactionExt[fillerOTHrCol] = 0;
                                                        }
                                                    }
                                                    #endregion
                                                }
                                                else if (ProcessPayrollPeriod.Substring(6, 1).Equals("2"))
                                                {
                                                    #region Check if exceed OT quota
                                                    dsManagerLogLedger = GetPrevPayPeriodOTSumAndJobStatus(prevEmployeeID, previousPayPeriod, dal);
                                                    if (dsManagerLogLedger.Tables[1].Rows.Count > 0)
                                                    {
                                                        if (GetApplicableHrsFromCommaDelimitedTable(OTLIMITHR_TBL
                                                                                                    , dsManagerLogLedger.Tables[1].Rows[0][0].ToString()
                                                                                                    , dsManagerLogLedger.Tables[1].Rows[0][1].ToString()
                                                                                                    , dsManagerLogLedger.Tables[1].Rows[0][2].ToString()
                                                                                                    , dsManagerLogLedger.Tables[1].Rows[0][3].ToString()
                                                                                                    , dsManagerLogLedger.Tables[1].Rows[0][4].ToString()
                                                                                                    , OTLIMITHR_ORIG
                                                                                                    , ref OTLIMITHR) == true)
                                                        {
                                                            if (dsManagerLogLedger.Tables[0].Rows.Count > 0)
                                                            {
                                                                #region Add hours to current
                                                                RegularOTHr += Convert.ToDouble(dsManagerLogLedger.Tables[0].Rows[0]["Ept_RegularOTHrTemp"]);
                                                                RestdayOTHr += Convert.ToDouble(dsManagerLogLedger.Tables[0].Rows[0]["Ept_RestdayOTHr"]);
                                                                LegalHolidayOTHr += Convert.ToDouble(dsManagerLogLedger.Tables[0].Rows[0]["Ept_LegalHolidayOTHr"]);
                                                                SpecialHolidayOTHr += Convert.ToDouble(dsManagerLogLedger.Tables[0].Rows[0]["Ept_SpecialHolidayOTHr"]);
                                                                PlantShutdownOTHr += Convert.ToDouble(dsManagerLogLedger.Tables[0].Rows[0]["Ept_PlantShutdownOTHr"]);
                                                                CompanyHolidayOTHr += Convert.ToDouble(dsManagerLogLedger.Tables[0].Rows[0]["Ept_CompanyHolidayOTHr"]);
                                                                RestdayLegalHolidayOTHr += Convert.ToDouble(dsManagerLogLedger.Tables[0].Rows[0]["Ept_RestdayLegalHolidayOTHr"]);
                                                                RestdaySpecialHolidayOTHr += Convert.ToDouble(dsManagerLogLedger.Tables[0].Rows[0]["Ept_RestdaySpecialHolidayOTHr"]);
                                                                RestdayCompanyHolidayOTHr += Convert.ToDouble(dsManagerLogLedger.Tables[0].Rows[0]["Ept_RestdayCompanyHolidayOTHr"]);
                                                                RestdayPlantShutdownOTHr += Convert.ToDouble(dsManagerLogLedger.Tables[0].Rows[0]["Ept_RestdayPlantShutdownOTHr"]);
                                                                //Added By Rendell Uy - 1/3/2011 (Counted as OT)
                                                                RestdayHr += Convert.ToDouble(dsManagerLogLedger.Tables[0].Rows[0]["Ept_RestdayHr"]);
                                                                LegalHolidayHr += Convert.ToDouble(dsManagerLogLedger.Tables[0].Rows[0]["Ept_LegalHolidayHr"]);
                                                                SpecialHolidayHr += Convert.ToDouble(dsManagerLogLedger.Tables[0].Rows[0]["Ept_SpecialHolidayHr"]);
                                                                PlantShutdownHr += Convert.ToDouble(dsManagerLogLedger.Tables[0].Rows[0]["Ept_PlantShutdownHr"]);
                                                                CompanyHolidayHr += Convert.ToDouble(dsManagerLogLedger.Tables[0].Rows[0]["Ept_CompanyHolidayHr"]);
                                                                RestdayLegalHolidayHr += Convert.ToDouble(dsManagerLogLedger.Tables[0].Rows[0]["Ept_RestdayLegalHolidayHr"]);
                                                                RestdaySpecialHolidayHr += Convert.ToDouble(dsManagerLogLedger.Tables[0].Rows[0]["Ept_RestdaySpecialHolidayHr"]);
                                                                RestdayCompanyHolidayHr += Convert.ToDouble(dsManagerLogLedger.Tables[0].Rows[0]["Ept_RestdayCompanyHolidayHr"]);
                                                                RestdayPlantShutdownHr += Convert.ToDouble(dsManagerLogLedger.Tables[0].Rows[0]["Ept_RestdayPlantShutdownHr"]);
                                                                #endregion
                                                            }
                                                        }
                                                    }
                                                    #endregion
                                                }
                                            }

                                            //Get sum
                                            double OTSum = 0.0;
                                            OTSum += RegularOTHr + RestdayOTHr + LegalHolidayOTHr + SpecialHolidayOTHr + PlantShutdownOTHr + CompanyHolidayOTHr
                                                    + RestdayLegalHolidayOTHr + RestdaySpecialHolidayOTHr + RestdayCompanyHolidayOTHr + RestdayPlantShutdownOTHr
                                                    + RestdayHr + LegalHolidayHr + SpecialHolidayHr + PlantShutdownHr + CompanyHolidayHr
                                                    + RestdayLegalHolidayHr + RestdaySpecialHolidayHr + RestdayCompanyHolidayHr + RestdayPlantShutdownHr;
                                            if (bHasDayCodeExt)
                                            {
                                                for (int l = 1; l <= FILLERCNT; l++)
                                                {
                                                    fillerHrCol = string.Format("Ept_Filler{0:00}_Hr", l);
                                                    fillerOTHrCol = string.Format("Ept_Filler{0:00}_OTHr", l);
                                                    OTSum += Convert.ToDouble(drEmployeePayrollTransactionExt[fillerHrCol]);
                                                    OTSum += Convert.ToDouble(drEmployeePayrollTransactionExt[fillerOTHrCol]);
                                                }
                                            }

                                            #region Zero-out all overtime hours
                                            RegularOTHr = 0;
                                            RestdayHr = 0;
                                            RestdayOTHr = 0;
                                            LegalHolidayHr = 0;
                                            LegalHolidayOTHr = 0;
                                            SpecialHolidayHr = 0;
                                            SpecialHolidayOTHr = 0;
                                            PlantShutdownHr = 0;
                                            PlantShutdownOTHr = 0;
                                            CompanyHolidayHr = 0;
                                            CompanyHolidayOTHr = 0;
                                            RestdayLegalHolidayHr = 0;
                                            RestdayLegalHolidayOTHr = 0;
                                            RestdaySpecialHolidayHr = 0;
                                            RestdaySpecialHolidayOTHr = 0;
                                            RestdayCompanyHolidayHr = 0;
                                            RestdayCompanyHolidayOTHr = 0;
                                            RestdayPlantShutdownHr = 0;
                                            RestdayPlantShutdownOTHr = 0;

                                            #region Payroll Transaction Detail Update
                                            if (!ProcessCurrentPeriod || Convert.ToBoolean(HRFRCLBRHR) == true || Convert.ToBoolean(MULTSAL) == true)
                                            {
                                                drArrOTApp = dtEmployeePayrollTransactionDetail.Select("Ept_EmployeeId = '" + prevEmployeeID + "'");
                                                foreach (DataRow drRow in drArrOTApp)
                                                {
                                                    drRow["Ept_RegularOTHr"] = 0;
                                                    drRow["Ept_RestdayHr"] = 0;
                                                    drRow["Ept_RestdayOTHr"] = 0;
                                                    drRow["Ept_LegalHolidayHr"] = 0;
                                                    drRow["Ept_LegalHolidayOTHr"] = 0;
                                                    drRow["Ept_SpecialHolidayHr"] = 0;
                                                    drRow["Ept_SpecialHolidayOTHr"] = 0;
                                                    drRow["Ept_PlantShutdownHr"] = 0;
                                                    drRow["Ept_PlantShutdownOTHr"] = 0;
                                                    drRow["Ept_CompanyHolidayHr"] = 0;
                                                    drRow["Ept_CompanyHolidayOTHr"] = 0;
                                                    drRow["Ept_RestdayLegalHolidayHr"] = 0;
                                                    drRow["Ept_RestdayLegalHolidayOTHr"] = 0;
                                                    drRow["Ept_RestdaySpecialHolidayHr"] = 0;
                                                    drRow["Ept_RestdaySpecialHolidayOTHr"] = 0;
                                                    drRow["Ept_RestdayCompanyHolidayHr"] = 0;
                                                    drRow["Ept_RestdayCompanyHolidayOTHr"] = 0;
                                                    drRow["Ept_RestdayPlantShutdownHr"] = 0;
                                                    drRow["Ept_RestdayPlantShutdownOTHr"] = 0;
                                                }

                                                if (bHasDayCodeExt)
                                                {
                                                    drArrOTApp = dtEmployeePayrollTransactionExtDetail.Select("Ept_EmployeeId = '" + prevEmployeeID + "'");
                                                    foreach (DataRow drRow in drArrOTApp)
                                                    {
                                                        for (int k = 1; k <= FILLERCNT; k++)
                                                        {
                                                            fillerHrCol = string.Format("Ept_Filler{0:00}_Hr", k);
                                                            fillerOTHrCol = string.Format("Ept_Filler{0:00}_OTHr", k);
                                                            drRow[fillerHrCol] = 0;
                                                            drRow[fillerOTHrCol] = 0;
                                                        }
                                                    }
                                                }
                                            }
                                            #endregion

                                            if (bHasDayCodeExt)
                                            {
                                                for (int m = 1; m <= FILLERCNT; m++)
                                                {
                                                    fillerHrCol = string.Format("Ept_Filler{0:00}_Hr", m);
                                                    fillerOTHrCol = string.Format("Ept_Filler{0:00}_OTHr", m);
                                                    drEmployeePayrollTransactionExt[fillerHrCol] = 0;
                                                    drEmployeePayrollTransactionExt[fillerOTHrCol] = 0;
                                                }
                                            }
                                            #endregion

                                            GetApplicableHrsFromCommaDelimitedTable(OTLIMITEQV_TBL
                                                                                    , dtEmployeeLogLedger.Rows[i - 1]["Emt_JobLevel"].ToString()
                                                                                    , dtEmployeeLogLedger.Rows[i - 1]["Emt_JobStatus"].ToString()
                                                                                    , dtEmployeeLogLedger.Rows[i - 1]["Emt_EmploymentStatus"].ToString()
                                                                                    , dtEmployeeLogLedger.Rows[i - 1]["Emt_PayrollType"].ToString()
                                                                                    , dtEmployeeLogLedger.Rows[i - 1]["Emt_PositionCode"].ToString()
                                                                                    , OTLIMITEQV_ORIG
                                                                                    , ref OTLIMITEQV);
                                            if (OTSum >= OTLIMITHR)
                                            {
                                                RegularOTHr = OTLIMITEQV;

                                                drArrOTApp = dtEmployeePayrollTransactionDetail.Select("Ept_EmployeeId = '" + prevEmployeeID + "'"); //
                                                if (drArrOTApp.Length > 0)
                                                    drArrOTApp[0]["Ept_RegularOTHr"] = OTLIMITEQV;
                                            }
                                        }
                                    }
                                    #endregion

                                    #region Copy to employee payroll transaction table
                                    if (bUserGeneratedPayTrans == false || !ProcessCurrentPeriod) //allow save if past pay period
                                    {
                                        drEmployeePayrollTransaction["Ept_EmployeeId"] = prevEmployeeID;
                                        drEmployeePayrollTransaction["Ept_CurrentPayPeriod"] = ProcessPayrollPeriod;
                                        drEmployeePayrollTransaction["Ept_AbsentHr"] = AbsentHr;
                                        drEmployeePayrollTransaction["Ept_RegularHr"] = RegularHrMonthlyDailyPay;
                                        drEmployeePayrollTransaction["Ept_RegularOTHr"] = RegularOTHr;
                                        drEmployeePayrollTransaction["Ept_RegularNDHr"] = RegularNDHr;
                                        drEmployeePayrollTransaction["Ept_RegularOTNDHr"] = RegularOTNDHr;
                                        drEmployeePayrollTransaction["Ept_RestdayHr"] = RestdayHr;
                                        drEmployeePayrollTransaction["Ept_RestdayOTHr"] = RestdayOTHr;
                                        drEmployeePayrollTransaction["Ept_RestdayNDHr"] = RestdayNDHr;
                                        drEmployeePayrollTransaction["Ept_RestdayOTNDHr"] = RestdayOTNDHr;
                                        drEmployeePayrollTransaction["Ept_LegalHolidayHr"] = LegalHolidayHr;
                                        drEmployeePayrollTransaction["Ept_LegalHolidayOTHr"] = LegalHolidayOTHr;
                                        drEmployeePayrollTransaction["Ept_LegalHolidayNDHr"] = LegalHolidayNDHr;
                                        drEmployeePayrollTransaction["Ept_LegalHolidayOTNDHr"] = LegalHolidayOTNDHr;
                                        drEmployeePayrollTransaction["Ept_SpecialHolidayHr"] = SpecialHolidayHr;
                                        drEmployeePayrollTransaction["Ept_SpecialHolidayOTHr"] = SpecialHolidayOTHr;
                                        drEmployeePayrollTransaction["Ept_SpecialHolidayNDHr"] = SpecialHolidayNDHr;
                                        drEmployeePayrollTransaction["Ept_SpecialHolidayOTNDHr"] = SpecialHolidayOTNDHr;
                                        drEmployeePayrollTransaction["Ept_PlantShutdownHr"] = PlantShutdownHr;
                                        drEmployeePayrollTransaction["Ept_PlantShutdownOTHr"] = PlantShutdownOTHr;
                                        drEmployeePayrollTransaction["Ept_PlantShutdownNDHr"] = PlantShutdownNDHr;
                                        drEmployeePayrollTransaction["Ept_PlantShutdownOTNDHr"] = PlantShutdownOTNDHr;
                                        drEmployeePayrollTransaction["Ept_CompanyHolidayHr"] = CompanyHolidayHr;
                                        drEmployeePayrollTransaction["Ept_CompanyHolidayOTHr"] = CompanyHolidayOTHr;
                                        drEmployeePayrollTransaction["Ept_CompanyHolidayNDHr"] = CompanyHolidayNDHr;
                                        drEmployeePayrollTransaction["Ept_CompanyHolidayOTNDHr"] = CompanyHolidayOTNDHr;
                                        drEmployeePayrollTransaction["Ept_RestdayLegalHolidayHr"] = RestdayLegalHolidayHr;
                                        drEmployeePayrollTransaction["Ept_RestdayLegalHolidayOTHr"] = RestdayLegalHolidayOTHr;
                                        drEmployeePayrollTransaction["Ept_RestdayLegalHolidayNDHr"] = RestdayLegalHolidayNDHr;
                                        drEmployeePayrollTransaction["Ept_RestdayLegalHolidayOTNDHr"] = RestdayLegalHolidayOTNDHr;
                                        drEmployeePayrollTransaction["Ept_RestdaySpecialHolidayHr"] = RestdaySpecialHolidayHr;
                                        drEmployeePayrollTransaction["Ept_RestdaySpecialHolidayOTHr"] = RestdaySpecialHolidayOTHr;
                                        drEmployeePayrollTransaction["Ept_RestdaySpecialHolidayNDHr"] = RestdaySpecialHolidayNDHr;
                                        drEmployeePayrollTransaction["Ept_RestdaySpecialHolidayOTNDHr"] = RestdaySpecialHolidayOTNDHr;
                                        drEmployeePayrollTransaction["Ept_RestdayCompanyHolidayHr"] = RestdayCompanyHolidayHr;
                                        drEmployeePayrollTransaction["Ept_RestdayCompanyHolidayOTHr"] = RestdayCompanyHolidayOTHr;
                                        drEmployeePayrollTransaction["Ept_RestdayCompanyHolidayNDHr"] = RestdayCompanyHolidayNDHr;
                                        drEmployeePayrollTransaction["Ept_RestdayCompanyHolidayOTNDHr"] = RestdayCompanyHolidayOTNDHr;
                                        drEmployeePayrollTransaction["Ept_RestdayPlantShutdownHr"] = RestdayPlantShutdownHr;
                                        drEmployeePayrollTransaction["Ept_RestdayPlantShutdownOTHr"] = RestdayPlantShutdownOTHr;
                                        drEmployeePayrollTransaction["Ept_RestdayPlantShutdownNDHr"] = RestdayPlantShutdownNDHr;
                                        drEmployeePayrollTransaction["Ept_RestdayPlantShutdownOTNDHr"] = RestdayPlantShutdownOTNDHr;
                                        drEmployeePayrollTransaction["Ept_RestdayLegalHolidayCount"] = RestdayLegalHolidayCount;
                                        drEmployeePayrollTransaction["Ept_WorkingDay"] = WorkingDay;
                                        drEmployeePayrollTransaction["Ept_LaborHrsAdjustmentAmt"] = 0;
                                        drEmployeePayrollTransaction["Ept_TaxAdjustmentAmt"] = 0;
                                        drEmployeePayrollTransaction["Ept_NonTaxAdjustmentAmt"] = 0;
                                        drEmployeePayrollTransaction["Ept_TaxAllowanceAmt"] = 0;
                                        drEmployeePayrollTransaction["Ept_NonTaxAllowanceAmt"] = 0;
                                        drEmployeePayrollTransaction["Ept_PayrollType"] = dtEmployeeLogLedger.Rows[i - 1]["Emt_PayrollType"];
                                        drEmployeePayrollTransaction["Usr_Login"] = LoginUser;
                                        drEmployeePayrollTransaction["Ludatetime"] = DateTime.Now;
                                        drEmployeePayrollTransaction["Ept_LateHr"] = LateHours;
                                        drEmployeePayrollTransaction["Ept_UndertimeHr"] = UndertimeHours;
                                        drEmployeePayrollTransaction["Ept_PaidLeaveHr"] = PaidLeaveHours;
                                        drEmployeePayrollTransaction["Ept_UnpaidLeaveHr"] = UnpaidLeaveHours;
                                        drEmployeePayrollTransaction["Ept_WholeDayAbsentHr"] = WholeDayAbsentHours;
                                        drEmployeePayrollTransaction["Ept_AbsentLegalHolidayHr"] = AbsentLegalHolidayHr;
                                        drEmployeePayrollTransaction["Ept_AbsentSpecialHolidayHr"] = AbsentSpecialHolidayHr;
                                        drEmployeePayrollTransaction["Ept_AbsentCompanyHolidayHr"] = AbsentCompanyHolidayHr;
                                        drEmployeePayrollTransaction["Ept_AbsentPlantShutdownHr"] = AbsentPlantShutdownHr;
                                        drEmployeePayrollTransaction["Ept_AbsentFillerHolidayHr"] = AbsentFillerHolidayHr;
                                        drEmployeePayrollTransaction["Ept_PaidLegalHolidayHr"] = PaidLegalHolidayHr;
                                        drEmployeePayrollTransaction["Ept_PaidSpecialHolidayHr"] = PaidSpecialHolidayHr;
                                        drEmployeePayrollTransaction["Ept_PaidCompanyHolidayHr"] = PaidCompanyHolidayHr;
                                        drEmployeePayrollTransaction["Ept_PaidFillerHolidayHr"] = PaidFillerHolidayHr;
                                        drEmployeePayrollTransaction["Ept_OvertimeAdjustmentAmt"] = 0;
                                        drEmployeePayrollTransaction["Ept_UserGenerated"] = 0;

                                        if (bProcessTrail)
                                            drEmployeePayrollTransaction["Ept_AdjustPayPeriod"] = AdjustPayrollPeriod;

                                        if (bHasDayCodeExt)
                                        {
                                            drEmployeePayrollTransactionExt["Ept_EmployeeId"] = prevEmployeeID;
                                            drEmployeePayrollTransactionExt["Ept_CurrentPayPeriod"] = ProcessPayrollPeriod;
                                            drEmployeePayrollTransactionExt["Usr_Login"] = LoginUser;
                                            drEmployeePayrollTransactionExt["Ludatetime"] = DateTime.Now;
                                            if (bProcessTrail)
                                                drEmployeePayrollTransactionExt["Ept_AdjustPayPeriod"] = AdjustPayrollPeriod;
                                        }

                                        //copy to table
                                        dtEmployeePayrollTransaction.Rows.Add(drEmployeePayrollTransaction);
                                        if (bHasDayCodeExt)
                                            dtEmployeePayrollTransactionExt.Rows.Add(drEmployeePayrollTransactionExt);
                                    }
                                    #endregion

                                    //initialize
                                    #region Initialize payroll trans variables
                                    AbsentHr = 0.0;
                                    RegularHr = 0.0;
                                    RegularOTHr = 0.0;
                                    RegularNDHr = 0.0;
                                    RegularOTNDHr = 0.0;
                                    RestdayHr = 0.0;
                                    RestdayOTHr = 0.0;
                                    RestdayNDHr = 0.0;
                                    RestdayOTNDHr = 0.0;
                                    LegalHolidayHr = 0.0;
                                    LegalHolidayOTHr = 0.0;
                                    LegalHolidayNDHr = 0.0;
                                    LegalHolidayOTNDHr = 0.0;
                                    SpecialHolidayHr = 0.0;
                                    SpecialHolidayOTHr = 0.0;
                                    SpecialHolidayNDHr = 0.0;
                                    SpecialHolidayOTNDHr = 0.0;
                                    PlantShutdownHr = 0.0;
                                    PlantShutdownOTHr = 0.0;
                                    PlantShutdownNDHr = 0.0;
                                    PlantShutdownOTNDHr = 0.0;
                                    CompanyHolidayHr = 0.0;
                                    CompanyHolidayOTHr = 0.0;
                                    CompanyHolidayNDHr = 0.0;
                                    CompanyHolidayOTNDHr = 0.0;
                                    RestdayLegalHolidayHr = 0.0;
                                    RestdayLegalHolidayOTHr = 0.0;
                                    RestdayLegalHolidayNDHr = 0.0;
                                    RestdayLegalHolidayOTNDHr = 0.0;
                                    RestdaySpecialHolidayHr = 0.0;
                                    RestdaySpecialHolidayOTHr = 0.0;
                                    RestdaySpecialHolidayNDHr = 0.0;
                                    RestdaySpecialHolidayOTNDHr = 0.0;
                                    RestdayCompanyHolidayHr = 0.0;
                                    RestdayCompanyHolidayOTHr = 0.0;
                                    RestdayCompanyHolidayNDHr = 0.0;
                                    RestdayCompanyHolidayOTNDHr = 0.0;
                                    RestdayPlantShutdownHr = 0.0;
                                    RestdayPlantShutdownOTHr = 0.0;
                                    RestdayPlantShutdownNDHr = 0.0;
                                    RestdayPlantShutdownOTNDHr = 0.0;
                                    RestdayLegalHolidayCount = 0;
                                    WorkingDay = 0;
                                    RegularHrMonthlyDailyPay = 0.0;
                                    LateHours = 0.0;
                                    UndertimeHours = 0.0;
                                    PaidLeaveHours = 0.0;
                                    UnpaidLeaveHours = 0.0;
                                    WholeDayAbsentHours = 0.0;
                                    AbsentLegalHolidayHr = 0.0;
                                    AbsentSpecialHolidayHr = 0.0;
                                    AbsentCompanyHolidayHr = 0.0;
                                    AbsentPlantShutdownHr = 0.0;
                                    AbsentFillerHolidayHr = 0.0;
                                    PaidLegalHolidayHr = 0.0;
                                    PaidSpecialHolidayHr = 0.0;
                                    PaidCompanyHolidayHr = 0.0;
                                    PaidFillerHolidayHr = 0.0;
                                    #endregion

                                    #region Initialize payroll trans row
                                    drEmployeePayrollTransaction = dtEmployeePayrollTransaction.NewRow();
                                    if (bHasDayCodeExt)
                                    {
                                        drEmployeePayrollTransactionExt = dtEmployeePayrollTransactionExt.NewRow();
                                        for (int j = 1; j <= FILLERCNT; j++)
                                        {
                                            //initialize
                                            fillerHrCol = string.Format("Ept_Filler{0:00}_Hr", j);
                                            fillerOTHrCol = string.Format("Ept_Filler{0:00}_OTHr", j);
                                            fillerNDHrCol = string.Format("Ept_Filler{0:00}_NDHr", j);
                                            fillerOTNDHrCol = string.Format("Ept_Filler{0:00}_OTNDHr", j);
                                            drEmployeePayrollTransactionExt[fillerHrCol] = 0;
                                            drEmployeePayrollTransactionExt[fillerOTHrCol] = 0;
                                            drEmployeePayrollTransactionExt[fillerNDHrCol] = 0;
                                            drEmployeePayrollTransactionExt[fillerOTNDHrCol] = 0;
                                        }
                                    }
                                    drEmployeePayrollTransactionDetailPrev = null; //reset previous record
                                    #endregion
                                }

                                //increment payroll transaction row count
                                indexPayTrans++;
                                //increment payroll transaction ext row count
                                if (bHasDayCodeExt)
                                    indexPayTransExt++;
                            }
                            #endregion

                            if (i == dtEmployeeLogLedger.Rows.Count)
                                break; //exit main loop

                            strProcessDate = dtEmployeeLogLedger.Rows[i]["Ell_ProcessDate"].ToString();
                            strPayType = dtEmployeeLogLedger.Rows[i]["Emt_PayrollType"].ToString();

                            if (!bProcessTrail)
                            {
                                #region Save initial data
                                dtEmployeeLogLedger.Rows[i]["Ell_ShiftMin"] = Convert.ToInt32(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Scm_ShiftHours"]) * 60);
                                dtEmployeeLogLedger.Rows[i]["Ell_ScheduleType"] = dtEmployeeLogLedger.Rows[i]["Scm_ScheduleType"].ToString();

                                //Check user-generated flag
                                drArrUserGeneratedPayTrans = dtUserGeneratedPayTrans.Select("Ept_EmployeeID = '" + curEmployeeID + "'");
                                bUserGeneratedPayTrans = false;
                                if (drArrUserGeneratedPayTrans.Length > 0)
                                    bUserGeneratedPayTrans = true;
                                #endregion

                                #region Variable Initialization
                                #region Retrieve Necessary Columns
                                shiftCode = dtEmployeeLogLedger.Rows[i]["Ell_ShiftCode"].ToString();
                                strDayCode = dtEmployeeLogLedger.Rows[i]["Ell_DayCode"].ToString();
                                strProcessDate = dtEmployeeLogLedger.Rows[i]["Ell_ProcessDate"].ToString();

                                bIsRestDay = (dtEmployeeLogLedger.Rows[i]["Ell_RestDay"].ToString().Equals("False")) ? false : true;
                                bIsHoliday = (dtEmployeeLogLedger.Rows[i]["Ell_Holiday"].ToString().Equals("False")) ? false : true;

                                iActualTimeIn1Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ell_ActualTimeIn_1"].ToString());
                                iActualTimeOut1Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ell_ActualTimeOut_1"].ToString());
                                iActualTimeIn2Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ell_ActualTimeIn_2"].ToString());
                                iActualTimeOut2Min = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Ell_ActualTimeOut_2"].ToString());

                                iConvTimeIn1Min = Convert.ToInt32(dtEmployeeLogLedger.Rows[i]["Ell_ConvertedTimeIn_1Min"].ToString());
                                iConvTimeOut1Min = Convert.ToInt32(dtEmployeeLogLedger.Rows[i]["Ell_ConvertedTimeOut_1Min"].ToString());
                                iConvTimeIn2Min = Convert.ToInt32(dtEmployeeLogLedger.Rows[i]["Ell_ConvertedTimeIn_2Min"].ToString());
                                iConvTimeOut2Min = Convert.ToInt32(dtEmployeeLogLedger.Rows[i]["Ell_ConvertedTimeOut_2Min"].ToString());

                                iCompTimeIn1Min = Convert.ToInt32(dtEmployeeLogLedger.Rows[i]["Ell_ComputedTimeIn_1Min"].ToString());
                                iCompTimeOut1Min = Convert.ToInt32(dtEmployeeLogLedger.Rows[i]["Ell_ComputedTimeOut_1Min"].ToString());
                                iCompTimeIn2Min = Convert.ToInt32(dtEmployeeLogLedger.Rows[i]["Ell_ComputedTimeIn_2Min"].ToString());
                                iCompTimeOut2Min = Convert.ToInt32(dtEmployeeLogLedger.Rows[i]["Ell_ComputedTimeOut_2Min"].ToString());

                                iShiftTimeIn1Min = Convert.ToInt32(dtEmployeeLogLedger.Rows[i]["Ell_ShiftTimeIn_1Min"].ToString());
                                iShiftTimeOut1Min = Convert.ToInt32(dtEmployeeLogLedger.Rows[i]["Ell_ShiftTimeOut_1Min"].ToString());
                                iShiftTimeIn2Min = Convert.ToInt32(dtEmployeeLogLedger.Rows[i]["Ell_ShiftTimeIn_2Min"].ToString());
                                iShiftTimeOut2Min = Convert.ToInt32(dtEmployeeLogLedger.Rows[i]["Ell_ShiftTimeOut_2Min"].ToString());

                                strMasterShiftTimeIn1 = dtEmployeeLogLedger.Rows[i]["Scm_ShiftTimeIn"].ToString();
                                strMasterShiftTimeOut1 = dtEmployeeLogLedger.Rows[i]["Scm_ShiftBreakStart"].ToString();
                                strMasterShiftTimeIn2 = dtEmployeeLogLedger.Rows[i]["Scm_ShiftBreakEnd"].ToString();
                                strMasterShiftTimeOut2 = dtEmployeeLogLedger.Rows[i]["Scm_ShiftTimeOut"].ToString();
                                #endregion

                                #region Miscellaneous Variables
                                iMasterShiftTimeIn1Min = 0;
                                iMasterShiftTimeOut1Min = 0;
                                iMasterShiftTimeIn2Min = 0;
                                iMasterShiftTimeOut2Min = 0;
                                iComputedLateMin = 0;
                                iComputedLate2Min = 0;
                                iComputedUndertime1Min = 0;
                                iComputedUndertime2Min = 0;
                                iPayLeaveMin = 0;
                                iNoPayLeaveMin = 0;
                                iExcessLeaveMin = 0;
                                iLeaveMinToBeAddedToReg = 0;
                                iLeaveMinOnPaidBreak = 0;
                                iComputedDayWorkMin = 0;
                                iComputedRegularMin = 0;
                                iInitialAbsentMin = 0;
                                iComputedAbsentMin = 0;
                                iTotalComputedAbsentMin = 0;
                                iOffsetOvertimeMin = 0;
                                iShiftMin = Convert.ToInt32(dtEmployeeLogLedger.Rows[i]["Ell_ShiftMin"].ToString());
                                strPayLeaveType = dtEmployeeLogLedger.Rows[i]["Ell_EncodedPayLeaveType"].ToString();
                                strScheduleType = dtEmployeeLogLedger.Rows[i]["Scm_ScheduleType"].ToString();
                                //iExcessOffset = 0;
                                //iForOffsetMin = 0;
                                iTimeMinTemp = 0;
                                iTimeMinTemp2 = 0;
                                iOTFraction = Convert.ToInt32(OTFRACT);
                                iOTFractionZero = (iOTFraction == 0) ? 1 : iOTFraction;
                                iEncodedOvertimeMin = 0;

                                //For Actual Late and Undertime
                                iActualTimeIn1MinOrig = iActualTimeIn1Min;
                                iActualTimeOut2MinOrig = iActualTimeOut2Min;
                                iActualLate1Mins = 0;
                                iActualLate2Mins = 0;
                                iActualUT1Mins = 0;
                                iActualUT2Mins = 0;

                                //Initialize Dummy Variables
                                iPayLeaveMinsDummy = 0;
                                iNoPayLeaveMinDummy = 0;
                                iExcessLeaveMinDummy = 0;
                                iLeaveMinToBeAddedToRegDummy = 0;
                                iLeaveMinOnPaidBreakDummy = 0;

                                //Check if New Hire
                                strHireDate = "";
                                bIsNewHire = false;
                                drArrOTApp = dtNewHire.Select(string.Format("Ell_EmployeeId = '{0}' AND Ell_ProcessDate = '{1}'", curEmployeeID, strProcessDate));
                                if (drArrOTApp.Length > 0)
                                {
                                    bIsNewHire = true;
                                    strHireDate = drArrOTApp[0]["Emt_HireDate"].ToString();
                                }

                                //Get Minimum OT Hr
                                GetApplicableHrsFromCommaDelimitedTable(MINOTHR_TBL
                                                                        , dtEmployeeLogLedger.Rows[i]["Emt_JobLevel"].ToString()
                                                                        , dtEmployeeLogLedger.Rows[i]["Emt_JobStatus"].ToString()
                                                                        , dtEmployeeLogLedger.Rows[i]["Emt_EmploymentStatus"].ToString()
                                                                        , dtEmployeeLogLedger.Rows[i]["Emt_PayrollType"].ToString()
                                                                        , dtEmployeeLogLedger.Rows[i]["Emt_PositionCode"].ToString()
                                                                        , MINOTHR_ORIG
                                                                        , ref MINOTHR);

                                //Set Flags
                                bIsGraveyard = dtEmployeeLogLedger.Rows[i]["Scm_ScheduleType"].ToString().Equals("G");
                                bOverrideGraveyardConv = false;
                                bIsOutsideShift = false;
                                bIsRegOrReg5DayCode = false;
                                bDailiesNoWorkNoPay = false;
                                #endregion

                                #region Day Code Initialization
                                drArrDayCode = dtDayCodeMaster.Select(string.Format("Dcm_DayCode = '{0}'", strDayCode));
                                if (drArrDayCode != null && drArrDayCode.Length > 0)
                                {
                                    //For Regular Days
                                    if (strDayCode.Equals("REG") || strDayCode.Equals("REG5") || strDayCode.Equals("REGN")
                                        || (Convert.ToBoolean(drArrDayCode[0]["Dcm_Restday"]) == false && Convert.ToBoolean(drArrDayCode[0]["Dcm_Holiday"]) == false))
                                    {
                                        bIsRegOrReg5DayCode = true;
                                        bIsRestDay = false;
                                        bIsHoliday = false;
                                        dtEmployeeLogLedger.Rows[i]["Ell_RestDay"] = false;
                                        dtEmployeeLogLedger.Rows[i]["Ell_Holiday"] = false;
                                    }
                                    //For Holidays
                                    if (strDayCode.Equals("HOL") || strDayCode.Equals("SPL") || strDayCode.Equals("PSD") 
                                        || bIsHoliday == true
                                        || Convert.ToBoolean(drArrDayCode[0]["Dcm_Holiday"]) == true)
                                    {
                                        drArrPrevDay = dtHolidays.Select(string.Format("Hmt_HolidayDate = '{0}' AND (Hmt_ApplicCity = 'ALL' OR Hmt_ApplicCity = '{1}')", strProcessDate, dtEmployeeLogLedger.Rows[i]["Ell_LocationCode"]));
                                        if (drArrPrevDay != null && drArrPrevDay.Length > 0 && drArrPrevDay[0]["Hmt_HolidayCode"].ToString() == strDayCode)
                                        {
                                            bIsHoliday = true;
                                            dtEmployeeLogLedger.Rows[i]["Ell_Holiday"] = true;
                                        }
                                        else
                                        {
                                            #region Day Code Not in Holiday Master
                                            AddErrorToLaborHourReport(curEmployeeID, dtEmployeeLogLedger.Rows[i]["Emt_LastName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_FirstName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_MiddleName"].ToString(), strProcessDate, "Day Code Not in Holiday Master");
                                            strDayCode = "REG";
                                            bIsRegOrReg5DayCode = true;
                                            bIsRestDay = false;
                                            bIsHoliday = false;
                                            iActualTimeIn1Min = 0;
                                            iActualTimeOut1Min = 0;
                                            iActualTimeIn2Min = 0;
                                            iActualTimeOut2Min = 0;
                                            dtEmployeeLogLedger.Rows[i]["Ell_DayCode"] = "REG";
                                            dtEmployeeLogLedger.Rows[i]["Ell_RestDay"] = false;
                                            dtEmployeeLogLedger.Rows[i]["Ell_Holiday"] = false;
                                            dtEmployeeLogLedger.Rows[i]["Ell_ActualTimeIn_1"] = "0000";
                                            dtEmployeeLogLedger.Rows[i]["Ell_ActualTimeOut_1"] = "0000";
                                            dtEmployeeLogLedger.Rows[i]["Ell_ActualTimeIn_2"] = "0000";
                                            dtEmployeeLogLedger.Rows[i]["Ell_ActualTimeOut_2"] = "0000";
                                            dtEmployeeLogLedger.Rows[i]["Ell_AssumedPresent"] = false;
                                            #endregion
                                        }
                                    }
                                    //For Restdays (Normal)
                                    if (strDayCode.Equals("REST")
                                        || (Convert.ToBoolean(drArrDayCode[0]["Dcm_Restday"]) == true && Convert.ToBoolean(drArrDayCode[0]["Dcm_Holiday"]) == false))
                                    {
                                        bIsRestDay = true;
                                        bIsHoliday = false;
                                        dtEmployeeLogLedger.Rows[i]["Ell_RestDay"] = true;
                                        dtEmployeeLogLedger.Rows[i]["Ell_Holiday"] = false;
                                    }
                                }
                                else
                                {
                                    #region Invalid Day Code
                                    AddErrorToLaborHourReport(curEmployeeID, dtEmployeeLogLedger.Rows[i]["Emt_LastName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_FirstName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_MiddleName"].ToString(), strProcessDate, "Invalid Day Code");
                                    strDayCode = "REG";
                                    bIsRegOrReg5DayCode = true;
                                    bIsRestDay = false;
                                    bIsHoliday = false;
                                    iActualTimeIn1Min = 0;
                                    iActualTimeOut1Min = 0;
                                    iActualTimeIn2Min = 0;
                                    iActualTimeOut2Min = 0;
                                    dtEmployeeLogLedger.Rows[i]["Ell_DayCode"] = "REG";
                                    dtEmployeeLogLedger.Rows[i]["Ell_RestDay"] = false;
                                    dtEmployeeLogLedger.Rows[i]["Ell_Holiday"] = false;
                                    dtEmployeeLogLedger.Rows[i]["Ell_ActualTimeIn_1"] = "0000";
                                    dtEmployeeLogLedger.Rows[i]["Ell_ActualTimeOut_1"] = "0000";
                                    dtEmployeeLogLedger.Rows[i]["Ell_ActualTimeIn_2"] = "0000";
                                    dtEmployeeLogLedger.Rows[i]["Ell_ActualTimeOut_2"] = "0000";
                                    dtEmployeeLogLedger.Rows[i]["Ell_AssumedPresent"] = false;
                                    #endregion
                                }

                                if ((!bIsRestDay && strDayCode.Equals("PSD") && !strPayType.Equals("M")) //Plant Shutdown for Daily/Hourly
                                        || (!bIsRestDay && strDayCode.Equals("PSD") && strPayType.Equals("M") && !Convert.ToBoolean(PSDMONTHLY)) //Plant Shutdown for Monthly
                                        || (!bIsRestDay && strDayCode.Equals("SPL") && strPayType.Equals("D")) //Special Holiday for Daily
                                        || (!bIsRestDay && strDayCode.Equals("CMPY") && strPayType.Equals("D"))) //Company Holiday for Daily (HOGP)
                                {
                                    bDailiesNoWorkNoPay = true;
                                }
                                #endregion

                                #region Shift Initialization
                                if (iShiftMin <= 0)
                                {
                                    #region Invalid Shift Code
                                    if (dtDefaultShift.Rows.Count > 0)
                                    {
                                        AddErrorToLaborHourReport(curEmployeeID, dtEmployeeLogLedger.Rows[i]["Emt_LastName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_FirstName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_MiddleName"].ToString(), strProcessDate, "Invalid Shift Code");
                                        strDayCode = "REG";
                                        bIsRegOrReg5DayCode = true;
                                        bIsRestDay = false;
                                        bIsHoliday = false;
                                        iActualTimeIn1Min = 0;
                                        iActualTimeOut1Min = 0;
                                        iActualTimeIn2Min = 0;
                                        iActualTimeOut2Min = 0;
                                        dtEmployeeLogLedger.Rows[i]["Ell_DayCode"] = "REG";
                                        dtEmployeeLogLedger.Rows[i]["Ell_RestDay"] = false;
                                        dtEmployeeLogLedger.Rows[i]["Ell_Holiday"] = false;
                                        dtEmployeeLogLedger.Rows[i]["Ell_ActualTimeIn_1"] = "0000";
                                        dtEmployeeLogLedger.Rows[i]["Ell_ActualTimeOut_1"] = "0000";
                                        dtEmployeeLogLedger.Rows[i]["Ell_ActualTimeIn_2"] = "0000";
                                        dtEmployeeLogLedger.Rows[i]["Ell_ActualTimeOut_2"] = "0000";
                                        dtEmployeeLogLedger.Rows[i]["Ell_AssumedPresent"] = false;

                                        shiftCode = dtDefaultShift.Rows[0]["Scm_ShiftCode"].ToString();
                                        strMasterShiftTimeIn1 = dtDefaultShift.Rows[0]["Scm_ShiftTimeIn"].ToString();
                                        strMasterShiftTimeOut1 = dtDefaultShift.Rows[0]["Scm_ShiftBreakStart"].ToString();
                                        strMasterShiftTimeIn2 = dtDefaultShift.Rows[0]["Scm_ShiftBreakEnd"].ToString();
                                        strMasterShiftTimeOut2 = dtDefaultShift.Rows[0]["Scm_ShiftTimeOut"].ToString();
                                        iShiftMin = Convert.ToInt32(Convert.ToDouble(dtDefaultShift.Rows[0]["Scm_ShiftHours"]) * 60);
                                        strScheduleType = dtDefaultShift.Rows[0]["Scm_ScheduleType"].ToString();
                                        dtEmployeeLogLedger.Rows[i]["Ell_ShiftCode"] = shiftCode;
                                        dtEmployeeLogLedger.Rows[i]["Scm_ShiftTimeIn"] = strMasterShiftTimeIn1;
                                        dtEmployeeLogLedger.Rows[i]["Scm_ShiftBreakStart"] = strMasterShiftTimeOut1;
                                        dtEmployeeLogLedger.Rows[i]["Scm_ShiftBreakEnd"] = strMasterShiftTimeIn2;
                                        dtEmployeeLogLedger.Rows[i]["Scm_ShiftTimeOut"] = strMasterShiftTimeOut2;
                                        dtEmployeeLogLedger.Rows[i]["Ell_ShiftMin"] = iShiftMin;
                                        dtEmployeeLogLedger.Rows[i]["Ell_ScheduleType"] = strScheduleType;
                                        dtEmployeeLogLedger.Rows[i]["Scm_PaidBreak"] = Convert.ToDouble(dtDefaultShift.Rows[0]["Scm_PaidBreak"]);
                                    }
                                    else
                                        throw new Exception("Invalid Shift Code");
                                    #endregion
                                }

                                //HOUR FRACTION CUTOFF TIME
                                if (dtEmployeeLogLedger.Rows[i]["Scm_HourFracCutoff"].ToString() == "")
                                    Cutoff = 1440; //24:00
                                else
                                    Cutoff = GetMinsFromHourStr(dtEmployeeLogLedger.Rows[i]["Scm_HourFracCutoff"].ToString());

                                //SHIFT TIME IN 1 
                                iMasterShiftTimeIn1Min = GetMinsFromHourStr(strMasterShiftTimeIn1);
                                iShiftTimeIn1Min = iMasterShiftTimeIn1Min;

                                //SHIFT TIME OUT 1 
                                iMasterShiftTimeOut1Min = GetMinsFromHourStr(strMasterShiftTimeOut1);
                                if (iMasterShiftTimeIn1Min > iMasterShiftTimeOut1Min)
                                {
                                    iMasterShiftTimeOut1Min = ConvertToGraveyardTime(iMasterShiftTimeOut1Min, bIsGraveyard);
                                }
                                iShiftTimeOut1Min = iMasterShiftTimeOut1Min;

                                //SHIFT TIME IN 2 
                                iMasterShiftTimeIn2Min = GetMinsFromHourStr(strMasterShiftTimeIn2);
                                if (iMasterShiftTimeIn1Min > iMasterShiftTimeIn2Min)
                                {
                                    iMasterShiftTimeIn2Min = ConvertToGraveyardTime(iMasterShiftTimeIn2Min, bIsGraveyard);
                                }
                                iShiftTimeIn2Min = iMasterShiftTimeIn2Min;

                                //SHIFT TIME OUT 2 
                                iMasterShiftTimeOut2Min = GetMinsFromHourStr(strMasterShiftTimeOut2);
                                if (iMasterShiftTimeIn1Min > iMasterShiftTimeOut2Min)
                                {
                                    iMasterShiftTimeOut2Min = ConvertToGraveyardTime(iMasterShiftTimeOut2Min, bIsGraveyard);
                                }
                                iShiftTimeOut2Min = iMasterShiftTimeOut2Min;
                                #endregion

                                #region OT Variables
                                //Get All Approved Overtime Records (with Flex Shift checking)
                                drArrOTApproved = GetCorrectedOvertimeRecords(curEmployeeID
                                                                                , strProcessDate
                                                                                , !bIsRestDay && !bIsHoliday && bIsRegOrReg5DayCode
                                                                                , bIsGraveyard && !bOverrideGraveyardConv
                                                                                , iShiftTimeIn1Min
                                                                                , iShiftTimeOut1Min
                                                                                , iShiftTimeIn2Min
                                                                                , iShiftTimeOut2Min
                                                                                , (iActualTimeIn1Min == 0) ? iActualTimeIn2Min : iActualTimeIn1Min
                                                                                , Convert.ToBoolean(FLEXSHIFT)
                                                                                , strPayType);

                                iComputedOvertimeMin = 0;
                                iAdvOTMin = 0;
                                iAdjShiftMin = 0; //Advanced OT column
                                iPaidBreak = 0;
                                iMasterPaidBreak = Convert.ToInt32(dtEmployeeLogLedger.Rows[i]["Scm_PaidBreak"].ToString());

                                iNDFraction = Convert.ToInt32(Convert.ToDecimal(NDFRACTION));
                                iCompRegNightPremMin = 0;
                                iCompOvertimeNightPremMin = 0;
                                iTimeMinTemp2 = 0;
                                iOTTemp = 0;
                                iOTTemp2 = 0;
                                iBreakMin = 0;
                                bIsOutsideShiftComputedOT = false; //Added By Rendell Uy - 09/06/2010 (Solution to Duplicate OT computation)
                                #endregion
                                #endregion

                                if (iActualTimeIn1Min == 6039 && POCKETSIZE > 0) //99:99 (Flag for log ledger extension)
                                {
                                    #region Multiple Pockets Logic
                                    iTempMultPockVar = 0;
                                    iExcessLeaveMin = 0;
                                    iLeaveMinToBeAddedToReg = 0;
                                    iPrevConvTimeOutExtMin = 0;
                                    bDonePaidBreakAdd = false;

                                    //Initialize Hour Fraction Table
                                    InitializeHourFractionTable();

                                    drArrEmployeeLogLedgerExt = dtEmployeeLogLedgerExt.Select(string.Format("Ell_EmployeeId = '{0}' AND Ell_ProcessDate = '{1}'", curEmployeeID, strProcessDate));
                                    if (drArrEmployeeLogLedgerExt.Length > 0)
                                    {
                                        for (int iPocket = 1; iPocket <= POCKETSIZE; iPocket++)
                                        {
                                            iActualTimeInExtMin = GetMinsFromHourStr(drArrEmployeeLogLedgerExt[0][string.Format("Ell_ActualTimeIn_{0:00}", iPocket)].ToString());
                                            iActualTimeOutExtMin = GetMinsFromHourStr(drArrEmployeeLogLedgerExt[0][string.Format("Ell_ActualTimeOut_{0:00}", iPocket)].ToString());

                                            if (iActualTimeInExtMin != 0 && iActualTimeOutExtMin != 0)
                                            {
                                                #region Late and Undertime Bracket Deduction
                                                if (iPocket == 1 && bIsRegOrReg5DayCode == true)
                                                {
                                                    iActualTimeInExtMin = GetTimeInWithLateBracketFilter(iActualTimeInExtMin, iShiftTimeIn1Min);
                                                    iActualTimeOutExtMin = GetTimeOutWithUndertimeBracketFilter(iActualTimeOutExtMin, iShiftTimeOut2Min);
                                                }
                                                #endregion

                                                #region Round Logs Based on Time Fraction
                                                if (TIMEFRAC > 0)
                                                {
                                                    iActualTimeInExtMin = CleanUpByRoundHigh(iActualTimeInExtMin, TIMEFRAC, dal);
                                                    iActualTimeOutExtMin = CleanUpByRoundLow(iActualTimeOutExtMin, TIMEFRAC, dal);
                                                }
                                                #endregion

                                                #region Converted Time
                                                //CONVERTED TIME 
                                                iAbsFraction = (bIsRegOrReg5DayCode == true) ? ABSFRACTRG : ABSFRACT;
                                                iConvTimeInExtMin = GenerateLaborHours_GetConvertedTimeIn1(iActualTimeInExtMin, iMasterShiftTimeIn1Min, dtEmployeeLogLedger.Rows[i]["Scm_PadShiftTimeIn"].ToString(), iAbsFraction, bIsGraveyard);
                                                iConvTimeOutExtMin = GenerateLaborHours_GetConvertedTimeOut1(iConvTimeInExtMin, iActualTimeOutExtMin, iMasterShiftTimeOut1Min, dtEmployeeLogLedger.Rows[i]["Scm_PadShiftTimeOut"].ToString(), iAbsFraction, bIsGraveyard);

                                                //Check if time logs are outside shift range
                                                if (iConvTimeInExtMin >= iMasterShiftTimeOut2Min || (iConvTimeOutExtMin > 0 && iConvTimeOutExtMin <= iMasterShiftTimeIn1Min))
                                                {
                                                    bIsOutsideShift = true; //Enable flag used to check for logs outside shift ranges
                                                }
                                                #endregion

                                                #region Pocket Gap and Pocket Time Gap
                                                //POCKET GAP
                                                if (iPocket != 1)
                                                {
                                                    if (iConvTimeInExtMin < iPrevConvTimeOutExtMin + POCKETGAP)
                                                        iConvTimeInExtMin = iPrevConvTimeOutExtMin + POCKETGAP;
                                                }
                                                //POCKET TIME GAP
                                                if (iConvTimeOutExtMin - iConvTimeInExtMin < POCKETTIME)
                                                {
                                                    iConvTimeOutExtMin = 0;
                                                    iConvTimeInExtMin = 0;
                                                }
                                                iPrevConvTimeOutExtMin = iConvTimeOutExtMin;
                                                #endregion

                                                #region Late and Undertime

                                                #endregion

                                                #region Computed Regular Minutes
                                                //COMPUTED REGULAR MINUTES
                                                iTempMultPockVar = 0;
                                                iTempMultPockVar += GetOTHoursInMinutes(iConvTimeInExtMin, iConvTimeOutExtMin, iShiftTimeIn1Min, iShiftTimeOut1Min);
                                                iTempMultPockVar += GetOTHoursInMinutes(iConvTimeInExtMin, iConvTimeOutExtMin, iShiftTimeIn2Min, iShiftTimeOut2Min);
                                                if (iTempMultPockVar > 0)
                                                {
                                                    iComputedRegularMin += iTempMultPockVar;
                                                    #region Insert Computed Time to Hour Fraction Table
                                                    if (!bIsRestDay && !bIsHoliday)
                                                    {
                                                        InsertRegularTimeToHourFractionTable(Math.Max(iConvTimeInExtMin, iShiftTimeIn1Min), Math.Min(iConvTimeOutExtMin, iShiftTimeOut1Min));
                                                        InsertRegularTimeToHourFractionTable(Math.Max(iConvTimeInExtMin, iShiftTimeIn2Min), Math.Min(iConvTimeOutExtMin, iShiftTimeOut2Min));
                                                    }
                                                    #endregion
                                                }
                                                #endregion

                                                #region Overtime and Night Premium Computation
                                                #region Overtime Applications Loop
                                                if (bIsRestDay || bIsHoliday || !bIsRegOrReg5DayCode)
                                                    iComputedOvertimeMin = 0; //initialize OT variable

                                                foreach (DataRow drOTApp in drArrOTApproved)
                                                {
                                                    iOTStartMin = GetMinsFromHourStr(drOTApp["Eot_StartTime"].ToString());
                                                    iOTEndMin = GetMinsFromHourStr(drOTApp["Eot_EndTime"].ToString());
                                                    strOTType = drOTApp["Eot_OvertimeType"].ToString();

                                                    #region OT Application Validation
                                                    if (strOTType.Equals("A") && iOTEndMin > iShiftTimeIn1Min)
                                                    {
                                                        AddErrorToLaborHourReport(curEmployeeID, dtEmployeeLogLedger.Rows[i]["Emt_LastName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_FirstName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_MiddleName"].ToString(), strProcessDate, "Advance OT Application Error");
                                                        continue; //skip erroneous OT application
                                                    }

                                                    bCountOTFraction = false;
                                                    if (Convert.ToBoolean(RNDOTFRAC) == true
                                                        && (Convert.ToInt32(iShiftTimeOut1Min / iOTFraction) * iOTFraction == iShiftTimeOut1Min
                                                            || Convert.ToInt32(iShiftTimeIn2Min / iOTFraction) * iOTFraction == iShiftTimeIn2Min))
                                                        bCountOTFraction = true;

                                                    if (bIsGraveyard && strOTType.Equals("P")) //Graveyard shift and Post-overtime
                                                    {
                                                        if (iOTStartMin < (iShiftTimeIn1Min - LOGPAD))
                                                        {
                                                            iOTStartMin += GRAVEYARD24;
                                                        }
                                                        if (iOTEndMin < (iShiftTimeOut2Min - LOGPAD))
                                                        {
                                                            iOTEndMin += GRAVEYARD24;
                                                        }
                                                    }
                                                    #endregion

                                                    if (bIsRestDay || bIsHoliday || !bIsRegOrReg5DayCode) //Rest day or holiday
                                                    {
                                                        #region Computed Overtime Minutes
                                                        //[In-between OT = Get OT between iConvTimeInExtMin and iConvTimeOutExtMin]
                                                        iAdvOTMin = GetOTHoursInMinutes(iOTStartMin, iOTEndMin, iConvTimeInExtMin, Math.Min(iConvTimeOutExtMin, iShiftTimeOut1Min));
                                                        if (bCountOTFraction == true)
                                                        {
                                                            iAdvOTMin = Convert.ToInt32(iAdvOTMin / iOTFraction) * iOTFraction;
                                                        }
                                                        if (strOTType.Equals("A"))
                                                        {
                                                            iAdjShiftMin += iAdvOTMin;
                                                        }
                                                        iComputedOvertimeMin += iAdvOTMin;

                                                        #region Insert Overtime to Hour Fraction Table
                                                        InsertOTToHourFractionTable(iOTStartMin, iOTEndMin, iConvTimeInExtMin, Math.Min(iConvTimeOutExtMin, iShiftTimeOut1Min), iOTFraction, bCountOTFraction);
                                                        #endregion

                                                        if (bIsOutsideShift && iComputedOvertimeMin > 0)
                                                        {
                                                            bIsOutsideShiftComputedOT = true;
                                                        }

                                                        //[In-between OT = Get OT between iConvTimeInExtMin and iConvTimeOutExtMin]
                                                        iAdvOTMin = GetOTHoursInMinutes(iOTStartMin, iOTEndMin, Math.Max(iConvTimeInExtMin, iShiftTimeIn2Min), iConvTimeOutExtMin);
                                                        if (bCountOTFraction == true)
                                                        {
                                                            iAdvOTMin = Convert.ToInt32(iAdvOTMin / iOTFraction) * iOTFraction;
                                                        }
                                                        if (strOTType.Equals("A"))
                                                        {
                                                            iAdjShiftMin += iAdvOTMin;
                                                        }

                                                        if (!bIsOutsideShift || !bIsOutsideShiftComputedOT)
                                                        {
                                                            iComputedOvertimeMin += iAdvOTMin;

                                                            #region Insert Overtime to Hour Fraction Table
                                                            InsertOTToHourFractionTable(iOTStartMin, iOTEndMin, Math.Max(iConvTimeInExtMin, iShiftTimeIn2Min), iConvTimeOutExtMin, iOTFraction, bCountOTFraction);
                                                            #endregion
                                                        }

                                                        if (bCountOTFraction == false)
                                                        {
                                                            iComputedOvertimeMin = Convert.ToInt32(iComputedOvertimeMin / iOTFraction) * iOTFraction;
                                                            CorrectOTHourFraction(iComputedOvertimeMin);
                                                        }

                                                        //Paid Break for Rest day
                                                        if (iConvTimeInExtMin > 0 && iConvTimeOutExtMin > 0)
                                                        {
                                                            iOTTemp = (iShiftTimeOut1Min > iOTStartMin) ? iShiftTimeOut1Min : iOTStartMin;
                                                            iOTTemp2 = (iShiftTimeIn2Min < iOTEndMin) ? iShiftTimeIn2Min : iOTEndMin;
                                                            iPaidBreak += GetOTHoursInMinutes(iOTTemp, iOTTemp2, iConvTimeInExtMin, iConvTimeOutExtMin);

                                                            if (iPaidBreak > iMasterPaidBreak) //Must not exceed the set paid break
                                                            {
                                                                iPaidBreak = iMasterPaidBreak;
                                                            }
                                                            else
                                                            {
                                                                if (MIDOT == false)
                                                                {
                                                                    #region Insert Paid Break to Hour Fraction Table
                                                                    InsertOTToHourFractionTable(iOTTemp, iOTTemp2, iConvTimeInExtMin, iConvTimeOutExtMin, iOTFraction, bCountOTFraction);
                                                                    #endregion
                                                                }
                                                            }
                                                        }

                                                        if (MIDOT == true && strOTType.Equals("M"))
                                                        {
                                                            iPaidBreak = 0;
                                                        }
                                                        #endregion

                                                        #region Computed Overtime Night Premium
                                                        iTimeMinTemp = (iConvTimeInExtMin > iOTStartMin) ? iConvTimeInExtMin : iOTStartMin;
                                                        iTimeMinTemp2 = (iConvTimeOutExtMin < iOTEndMin) ? iConvTimeOutExtMin : iOTEndMin;

                                                        ///OVERTIME NIGHT PREMIUM FOR DAY SHIFTS
                                                        if (!Convert.ToBoolean(NONDOTDAY)) //HOYA
                                                        {
                                                            //[NDOTHr = Get NDOT between 00:00/OT Start to 06:00/OT End]
                                                            iOTTemp = (NIGHTDIFFAMSTART > iOTStartMin) ? NIGHTDIFFAMSTART : iOTStartMin;
                                                            iOTTemp2 = (NIGHTDIFFAMEND < iOTEndMin) ? NIGHTDIFFAMEND : iOTEndMin;
                                                            iCompOvertimeNightPremMin += GetOTHoursInMinutes(iOTTemp, iOTTemp2, iTimeMinTemp, iTimeMinTemp2);
                                                            #region Insert ND Hour to Hour Fraction Table
                                                            InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iTimeMinTemp, iTimeMinTemp2, NDFRACTION, HourType.NDOTHour);
                                                            #endregion
                                                        }

                                                        ///OVERTIME NIGHT PREMIUM FOR GRAVEYARD SHIFTS
                                                        //[NDOTHr = Get NDOT between 22:00/OT Start to 30:00/OT End]
                                                        iOTTemp = (NIGHTDIFFGRAVEYARDSTART > iOTStartMin) ? NIGHTDIFFGRAVEYARDSTART : iOTStartMin;
                                                        iOTTemp2 = (NIGHTDIFFGRAVEYARDEND < iOTEndMin) ? NIGHTDIFFGRAVEYARDEND : iOTEndMin;
                                                        iCompOvertimeNightPremMin += GetOTHoursInMinutes(iOTTemp, iOTTemp2, iTimeMinTemp, iTimeMinTemp2);
                                                        #region Insert ND Hour to Hour Fraction Table
                                                        InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iTimeMinTemp, iTimeMinTemp2, NDFRACTION, HourType.NDOTHour);
                                                        #endregion
                                                        #endregion
                                                    }
                                                    else //Regular day
                                                    {
                                                        #region Computed Overtime Minutes
                                                        //[Get OT between iConvTimeInExtMin and iConvTimeOutExtMin]
                                                        iAdvOTMin = GetOTHoursInMinutes(iOTStartMin, iOTEndMin, iConvTimeInExtMin, iConvTimeOutExtMin);
                                                        if (bCountOTFraction == true)
                                                        {
                                                            iAdvOTMin = Convert.ToInt32(iAdvOTMin / iOTFraction) * iOTFraction;
                                                        }
                                                        iAdjShiftMin += iAdvOTMin;
                                                        iComputedOvertimeMin += iAdvOTMin;

                                                        #region Insert Overtime to Hour Fraction Table
                                                        InsertOTToHourFractionTable(iOTStartMin, iOTEndMin, iConvTimeInExtMin, iConvTimeOutExtMin, iOTFraction, bCountOTFraction);
                                                        #endregion

                                                        if (bCountOTFraction == false)
                                                        {
                                                            iComputedOvertimeMin = Convert.ToInt32(iComputedOvertimeMin / iOTFraction) * iOTFraction;
                                                            CorrectOTHourFraction(iComputedOvertimeMin);
                                                        }
                                                        #endregion

                                                        #region Computed Overtime Night Premium
                                                        iTimeMinTemp = (iConvTimeInExtMin > iShiftTimeIn1Min) ? iConvTimeInExtMin : iShiftTimeIn1Min;
                                                        iTimeMinTemp2 = (iConvTimeOutExtMin < iShiftTimeOut2Min) ? iConvTimeOutExtMin : iShiftTimeOut2Min;

                                                        if (bIsGraveyard) //Graveyard shift
                                                        {
                                                            //[NDOTHr = Get NDOT between 22:00 to iConvTimeInExtMin]
                                                            iTimeMinTemp = (NIGHTDIFFGRAVEYARDSTART > iTimeMinTemp) ? NIGHTDIFFGRAVEYARDSTART : iTimeMinTemp;
                                                            iTempMultPockVar = (iConvTimeInExtMin > iShiftTimeIn1Min) ? iConvTimeInExtMin : iShiftTimeIn1Min;
                                                            iCompOvertimeNightPremMin += GetOTHoursInMinutes(iOTStartMin, iOTEndMin, iTimeMinTemp, iTempMultPockVar);
                                                            #region Insert ND Hour to Hour Fraction Table
                                                            InsertNDToHourFractionTable(iOTStartMin, iOTEndMin, iTimeMinTemp, iTempMultPockVar, NDFRACTION, HourType.NDOTHour);
                                                            #endregion

                                                            //[NDOTHr = Get NDOT between iConvTimeOutExtMin to 30:00]
                                                            iTimeMinTemp2 = (iTimeMinTemp2 < NIGHTDIFFGRAVEYARDEND) ? iTimeMinTemp2 : NIGHTDIFFGRAVEYARDEND;
                                                            iTempMultPockVar = (iConvTimeOutExtMin < iShiftTimeOut2Min) ? iConvTimeOutExtMin : iShiftTimeOut2Min;
                                                            iCompOvertimeNightPremMin += GetOTHoursInMinutes(iOTStartMin, iOTEndMin, iTempMultPockVar, iTimeMinTemp2);
                                                            #region Insert ND Hour to Hour Fraction Table
                                                            InsertNDToHourFractionTable(iOTStartMin, iOTEndMin, iTempMultPockVar, iTimeMinTemp2, NDFRACTION, HourType.NDOTHour);
                                                            #endregion
                                                        }
                                                        else
                                                        {
                                                            if (!Convert.ToBoolean(NONDOTDAY)) //HOYA
                                                            {
                                                                //[NDOTHr = Get NDOT between iConvTimeInExtMin to 06:00]
                                                                iTimeMinTemp2 = (NIGHTDIFFAMEND < iTimeMinTemp2) ? NIGHTDIFFAMEND : iTimeMinTemp2;
                                                                iCompOvertimeNightPremMin += GetOTHoursInMinutes(iOTStartMin, iOTEndMin, iConvTimeInExtMin, iTimeMinTemp2);
                                                                #region Insert ND Hour to Hour Fraction Table
                                                                InsertNDToHourFractionTable(iOTStartMin, iOTEndMin, iConvTimeInExtMin, iTimeMinTemp2, NDFRACTION, HourType.NDOTHour);
                                                                #endregion
                                                            }
                                                            //[NDOTHr = Get NDOT between 22:00/ComputedOut2 to iConvTimeOutExtMin]
                                                            iTimeMinTemp = (iTimeMinTemp > NIGHTDIFFGRAVEYARDSTART) ? iTimeMinTemp : NIGHTDIFFGRAVEYARDSTART;
                                                            iOTEndMin = (iOTEndMin > NIGHTDIFFGRAVEYARDEND) ? NIGHTDIFFGRAVEYARDEND : iOTEndMin;
                                                            iCompOvertimeNightPremMin += GetOTHoursInMinutes(iOTStartMin, iOTEndMin, iTimeMinTemp, iConvTimeOutExtMin);
                                                            #region Insert ND Hour to Hour Fraction Table
                                                            InsertNDToHourFractionTable(iOTStartMin, iOTEndMin, iTimeMinTemp, iConvTimeOutExtMin, NDFRACTION, HourType.NDOTHour);
                                                            #endregion
                                                        }
                                                        #endregion
                                                    }
                                                }
                                                #endregion

                                                #region Computed Regular Night Premium for Regular Day
                                                if (!bIsRestDay && !bIsHoliday && bIsRegOrReg5DayCode) //Regular day
                                                {
                                                    //COMPUTED REGULAR NIGHT PREMIUM MIN
                                                    iTimeMinTemp = (iConvTimeInExtMin > iShiftTimeIn1Min) ? iConvTimeInExtMin : iShiftTimeIn1Min;
                                                    iTimeMinTemp2 = (iConvTimeOutExtMin < iShiftTimeOut2Min) ? iConvTimeOutExtMin : iShiftTimeOut2Min;

                                                    if (iTimeMinTemp > 0 && iTimeMinTemp2 > 0)
                                                    {
                                                        if (!Convert.ToBoolean(NONDOTDAY)) //HOYA
                                                        {
                                                            //[NDHr = Get ND between iConvTimeInExtMin/iShiftTimeIn1Min to iConvTimeOutExtMin/iShiftTimeOut2Min]
                                                            iCompRegNightPremMin += GetOTHoursInMinutes(NIGHTDIFFAMSTART, NIGHTDIFFAMEND, iTimeMinTemp, iTimeMinTemp2);
                                                            #region Insert ND Hour to Hour Fraction Table
                                                            InsertNDToHourFractionTable(NIGHTDIFFAMSTART, NIGHTDIFFAMEND, iTimeMinTemp, iTimeMinTemp2, NDFRACTION, HourType.NDHour);
                                                            #endregion
                                                        }
                                                        //[NDHr = Get ND between iConvTimeInExtMin/iShiftTimeIn1Min to iConvTimeOutExtMin/iShiftTimeOut2Min]
                                                        iCompRegNightPremMin += GetOTHoursInMinutes(NIGHTDIFFGRAVEYARDSTART, NIGHTDIFFGRAVEYARDEND, iTimeMinTemp, iTimeMinTemp2);
                                                        #region Insert ND Hour to Hour Fraction Table
                                                        InsertNDToHourFractionTable(NIGHTDIFFGRAVEYARDSTART, NIGHTDIFFGRAVEYARDEND, iTimeMinTemp, iTimeMinTemp2, NDFRACTION, HourType.NDHour);
                                                        #endregion
                                                    }
                                                }
                                                #endregion

                                                #region NDFraction Filter
                                                iCompRegNightPremMin = Convert.ToInt32((iCompRegNightPremMin / iNDFraction)) * iNDFraction;
                                                iCompOvertimeNightPremMin = Convert.ToInt32((iCompOvertimeNightPremMin / iNDFraction)) * iNDFraction;
                                                ////HOYA
                                                if (strDayCode.Equals("REG5") || (Convert.ToBoolean(NDPREM1ST8))) //&& (bIsRestDay || bIsHoliday))) //Commented by Rendell Uy (10/6/2015)
                                                {
                                                    iNDSum = iCompRegNightPremMin + iCompOvertimeNightPremMin;
                                                    if (iNDSum > 8 * 60)
                                                    {
                                                        iCompRegNightPremMin = 8 * 60; //ND hours is set to 8 hours
                                                        iCompOvertimeNightPremMin = iNDSum - iCompRegNightPremMin; //excess 8 hours is set to NDOT hours
                                                    }
                                                    else
                                                    {
                                                        iCompRegNightPremMin = iNDSum; //all ND and NDOT hours to ND premium
                                                        iCompOvertimeNightPremMin = 0; //no NDOT hours
                                                    }
                                                }
                                                #endregion
                                                #endregion

                                                #region Paid Break for Regular Day
                                                if (iConvTimeInExtMin > 0 && iConvTimeOutExtMin > 0)
                                                {
                                                    if (!bIsRestDay && !bIsHoliday && bIsRegOrReg5DayCode) //Regular day
                                                    {
                                                        iPaidBreak += GetOTHoursInMinutes(iShiftTimeOut1Min, iShiftTimeIn2Min, iConvTimeInExtMin, iConvTimeOutExtMin);

                                                        if (iPaidBreak > iMasterPaidBreak) //Must not exceed the set paid break
                                                        {
                                                            iPaidBreak = iMasterPaidBreak;
                                                        }
                                                        else
                                                        {
                                                            #region Insert Computed Time to Hour Fraction Table
                                                            if (!bIsRestDay && !bIsHoliday)
                                                                InsertRegularTimeToHourFractionTable(iShiftTimeOut1Min, iShiftTimeIn2Min, iConvTimeInExtMin, iConvTimeOutExtMin);
                                                            #endregion
                                                        }
                                                    }
                                                }
                                                #endregion

                                                #region Initial Computed Regular and Day Work Minutes
                                                if (bIsRestDay || bIsHoliday || !bIsRegOrReg5DayCode || strDayCode.Equals("REGA")) //Rest day or holiday
                                                {
                                                    //Overtime hour is pre-added with paid break hours
                                                    if (iComputedOvertimeMin > 0 && bDonePaidBreakAdd == false)
                                                    {
                                                        iComputedOvertimeMin += iPaidBreak;
                                                        bDonePaidBreakAdd = true;
                                                    }
                                                    //COMPUTED DAY WORK MIN
                                                    iComputedDayWorkMin += iComputedOvertimeMin;
                                                    //COMPUTED REGULAR MIN
                                                    iComputedRegularMin = (iComputedDayWorkMin > iShiftMin) ? iShiftMin : iComputedDayWorkMin;
                                                    //COMPUTED OVERTIME MIN
                                                    iComputedOvertimeMin = (iComputedDayWorkMin > iShiftMin) ? iComputedDayWorkMin - iShiftMin : 0;
                                                }
                                                else //Regular day
                                                {
                                                    //COMPUTED REGULAR MIN
                                                    if (bDonePaidBreakAdd == false)
                                                    {
                                                        iComputedRegularMin += iPaidBreak;
                                                        bDonePaidBreakAdd = true;
                                                    }
                                                    //COMPUTED DAY WORK MIN
                                                    iComputedDayWorkMin += iComputedRegularMin;
                                                }
                                                #endregion

                                                #region Actual Leave Hour Computation
                                                iPayLeaveMin = Convert.ToInt32(Convert.ToDecimal(dtEmployeeLogLedger.Rows[i]["Ell_EncodedPayLeaveHr"]) * 60);
                                                if ((strDayCode.Equals("PSD") || (strDayCode.Equals("SPL") && strPayType.Equals("D")) || (strDayCode.Equals("CMPY") && strPayType.Equals("D")) || (!bIsRestDay && !bIsHoliday)) &&
                                                     iPayLeaveMin > 0) //Condition that will allow filing of leave
                                                {
                                                    iOrigPayLeaveMin = iPayLeaveMin;

                                                    //Get consumed leave hours
                                                    iPayLeaveMin = GetActualLeaveHourTwoPackets(curEmployeeID, strProcessDate, iConvTimeInExtMin, iConvTimeOutExtMin
                                                                                                , iShiftTimeIn1Min, iShiftTimeOut1Min, iShiftTimeIn2Min, iShiftTimeOut2Min
                                                                                                , ref iExcessLeaveMin, ref iLeaveMinToBeAddedToReg, bIsGraveyard, iMasterPaidBreak, true);

                                                    if (iExcessLeaveMin > 0 && iExcessLeaveMin < iOrigPayLeaveMin - iPayLeaveMin)
                                                    {
                                                        iExcessLeaveMin += iOrigPayLeaveMin - iPayLeaveMin; //in case leave hours exceed shift range
                                                    }
                                                }
                                                else
                                                {
                                                    iPayLeaveMin = 0; //just to be sure; for other day codes
                                                }
                                                #endregion
                                            }
                                        }
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region Initial Actual Logs Cleanup
                                    if (iActualTimeIn1Min == iActualTimeOut1Min)
                                    {
                                        iActualTimeIn1Min = 0;
                                        iActualTimeOut1Min = 0;
                                    }
                                    if (iActualTimeIn2Min == iActualTimeOut2Min)
                                    {
                                        iActualTimeIn2Min = 0;
                                        iActualTimeOut2Min = 0;
                                    }
                                    if (iActualTimeIn1Min == iActualTimeOut2Min)
                                    {
                                        iActualTimeIn1Min = 0;
                                        iActualTimeOut2Min = 0;
                                    }
                                    #endregion

                                    #region Assumed Present
                                    if (!bIsRestDay && !bIsHoliday && bIsRegOrReg5DayCode) //Regular Day
                                    {
                                        if ((Convert.ToBoolean(dtEmployeeLogLedger.Rows[i]["Ell_AssumedPresent"]) == true
                                            && (dtEmployeeLogLedger.Rows[i]["Ell_AssumedPostBack"] == DBNull.Value
                                                || dtEmployeeLogLedger.Rows[i]["Ell_AssumedPostBack"].ToString() == ""
                                                || dtEmployeeLogLedger.Rows[i]["Ell_AssumedPostBack"].ToString().Equals("T"))
                                            ) || (dtEmployeeLogLedger.Rows[i]["Ell_AssumedPostBack"] != DBNull.Value
                                                && dtEmployeeLogLedger.Rows[i]["Ell_AssumedPostBack"].ToString().Equals("T")))
                                        {
                                            iActualTimeIn1Min = iShiftTimeIn1Min;
                                            iActualTimeOut1Min = iShiftTimeOut1Min;
                                            iActualTimeIn2Min = iShiftTimeIn2Min;
                                            iActualTimeOut2Min = iShiftTimeOut2Min;
                                        }
                                    }
                                    else //Rest Day
                                    {
                                        if (dtEmployeeLogLedger.Rows[i]["Ell_AssumedPostBack"] != DBNull.Value
                                            && dtEmployeeLogLedger.Rows[i]["Ell_AssumedPostBack"].ToString().Equals("T"))
                                        {
                                            iActualTimeIn1Min = 0;
                                            iActualTimeOut1Min = 0;
                                            iActualTimeIn2Min = 0;
                                            iActualTimeOut2Min = 0;
                                        }
                                    }
                                    #endregion

                                    #region Late Charge (Daily)
                                    //LATE CHARGE DAILY 
                                    if (bIsRegOrReg5DayCode == true && LATECHARGEFREQUENCY.Equals("D"))
                                    {
                                        iActualTimeIn1Min = GetTimeWithLateChargeDaily(iActualTimeIn1Min, iShiftTimeIn1Min);
                                    }
                                    #endregion

                                    #region Late Charge (Quincena)
                                    //LATE CHARGE QUINCENA INITIALIZATION
                                    if (curEmployeeID != prevEmployeeID && LATECHARGEFREQUENCY.Equals("Q"))
                                    {
                                        bApplyLateChargeQuincena = IsLateChargeQuincenaMet(curEmployeeID, ProcessPayrollPeriod, dal);
                                    }

                                    if (iActualTimeIn1Min != 0 && bApplyLateChargeQuincena)
                                    {
                                        iActualTimeIn1Min = iShiftTimeIn1Min;
                                    }
                                    #endregion

                                    #region Late and Undertime Bracket Deduction
                                    if (bIsRegOrReg5DayCode == true)
                                    {
                                        if (LATEBRACKETDEDUCTION != null || UNDERTIMEBRACKETDEDUCTION != null)
                                        {
                                            //Wrong placement - AM Logs
                                            if (iActualTimeIn1Min > 0 &&
                                                iActualTimeIn2Min == 0 &&
                                                iActualTimeOut2Min > 0 &&
                                                iActualTimeOut1Min == 0 &&
                                                iActualTimeIn1Min < iActualTimeOut2Min &&
                                                iActualTimeOut2Min < iShiftTimeIn2Min)
                                            {
                                                iActualTimeOut1Min = iActualTimeOut2Min;
                                                iActualTimeOut2Min = 0;
                                            }

                                            //Wrong placement - AM Logs (Scenario 2)
                                            if (iActualTimeIn1Min > 0 &&
                                                iActualTimeIn2Min == 0 &&
                                                iActualTimeOut2Min == 0 &&
                                                iActualTimeOut1Min > 0 &&
                                                iActualTimeIn1Min < iActualTimeOut1Min &&
                                                iActualTimeOut1Min >= iShiftTimeIn2Min)
                                            {
                                                iActualTimeOut2Min = iActualTimeOut1Min;
                                                iActualTimeOut1Min = 0;
                                            }

                                            //Wrong placement - PM Logs
                                            if (iActualTimeIn1Min > 0 &&
                                                iActualTimeIn2Min == 0 &&
                                                iActualTimeOut2Min > 0 &&
                                                iActualTimeOut1Min == 0 &&
                                                iActualTimeIn1Min < iActualTimeOut2Min &&
                                                iActualTimeIn1Min > iShiftTimeOut1Min)
                                            {
                                                iActualTimeIn2Min = iActualTimeIn1Min;
                                                iActualTimeIn1Min = 0;
                                            }

                                            //Wrong placement - PM Logs (Scenario 2)
                                            if (iActualTimeIn1Min == 0 &&
                                                iActualTimeIn2Min > 0 &&
                                                iActualTimeOut2Min > 0 &&
                                                iActualTimeOut1Min == 0 &&
                                                iActualTimeIn2Min < iActualTimeOut2Min &&
                                                iActualTimeIn2Min <= iShiftTimeOut1Min)
                                            {
                                                iActualTimeIn1Min = iActualTimeIn2Min;
                                                iActualTimeIn2Min = 0;
                                            }
                                        }

                                        if (iActualTimeIn1Min != 0) //check IN1 first before IN2
                                            iActualTimeIn1Min = GetTimeInWithLateBracketFilter(iActualTimeIn1Min, iShiftTimeIn1Min);
                                        else if (iActualTimeIn2Min != 0)
                                            iActualTimeIn2Min = GetTimeInWithLatePMBracketFilter(iActualTimeIn2Min, iShiftTimeIn1Min, iShiftTimeOut1Min, iShiftTimeIn2Min);

                                        if (iActualTimeOut2Min != 0) //check OUT2 first before OUT1
                                            iActualTimeOut2Min = GetTimeOutWithUndertimeBracketFilter(iActualTimeOut2Min, iShiftTimeOut2Min);
                                        else if (iActualTimeOut1Min != 0)
                                            iActualTimeOut1Min = GetTimeOutWithUndertimePMBracketFilter(iActualTimeOut1Min, iShiftTimeOut1Min, iShiftTimeIn2Min, iShiftTimeOut2Min);
                                    }
                                    #endregion

                                    #region Round Logs Based on Time Fraction
                                    if (TIMEFRAC > 0)
                                    {
                                        //Added by Rendell Uy - 10/13/2010 (For Chiyoda)
                                        iActualTimeIn1Min = CleanUpByRoundHigh(iActualTimeIn1Min, TIMEFRAC, dal);
                                        iActualTimeOut1Min = CleanUpByRoundLow(iActualTimeOut1Min, TIMEFRAC, dal);
                                        iActualTimeIn2Min = CleanUpByRoundHigh(iActualTimeIn2Min, TIMEFRAC, dal);
                                        iActualTimeOut2Min = CleanUpByRoundLow(iActualTimeOut2Min, TIMEFRAC, dal);
                                    }
                                    #endregion

                                    #region Initial Actual Logs Cleanup (2nd pass)
                                    if (iActualTimeIn1Min == iActualTimeOut1Min)
                                    {
                                        iActualTimeIn1Min = 0;
                                        iActualTimeOut1Min = 0;
                                    }
                                    if (iActualTimeIn2Min == iActualTimeOut2Min)
                                    {
                                        iActualTimeIn2Min = 0;
                                        iActualTimeOut2Min = 0;
                                    }
                                    if (iActualTimeIn1Min == iActualTimeOut2Min)
                                    {
                                        iActualTimeIn1Min = 0;
                                        iActualTimeOut2Min = 0;
                                    }
                                    #endregion

                                    #region Converted Time
                                    //CONVERTED TIME 
                                    iAbsFraction = (bIsRegOrReg5DayCode == true) ? ABSFRACTRG : ABSFRACT;
                                    iConvTimeIn1Min = GenerateLaborHours_GetConvertedTimeIn1(iActualTimeIn1Min, iMasterShiftTimeIn1Min, dtEmployeeLogLedger.Rows[i]["Scm_PadShiftTimeIn"].ToString(), iAbsFraction, bIsGraveyard && !bOverrideGraveyardConv);
                                    iConvTimeOut1Min = GenerateLaborHours_GetConvertedTimeOut1(iConvTimeIn1Min, iActualTimeOut1Min, iMasterShiftTimeOut1Min, dtEmployeeLogLedger.Rows[i]["Scm_PadShiftBreakStart"].ToString(), iAbsFraction, bIsGraveyard && !bOverrideGraveyardConv);
                                    iConvTimeIn2Min = GenerateLaborHours_GetConvertedTimeIn2(iConvTimeIn1Min, iActualTimeIn2Min, iMasterShiftTimeIn2Min, dtEmployeeLogLedger.Rows[i]["Scm_PadShiftBreakEnd"].ToString(), iAbsFraction, bIsGraveyard && !bOverrideGraveyardConv);
                                    iConvTimeOut2Min = GenerateLaborHours_GetConvertedTimeOut2(iConvTimeIn1Min, iConvTimeIn2Min, iActualTimeOut2Min, iMasterShiftTimeOut2Min, dtEmployeeLogLedger.Rows[i]["Scm_PadShiftTimeOut"].ToString(), iAbsFraction, bIsGraveyard && !bOverrideGraveyardConv);
                                    #endregion

                                    #region Converted Time for Outside Shift
                                    //Check if time logs are outside shift range
                                    if (iConvTimeIn1Min >= iMasterShiftTimeOut2Min
                                        || (bIsGraveyard == false && iConvTimeOut2Min > 0 && iConvTimeOut2Min <= iMasterShiftTimeOut1Min))
                                    {
                                        bIsOutsideShift = true; //Enable flag used to check for logs outside shift ranges
                                        iConvTimeOut1Min = 0;
                                        iConvTimeIn2Min = 0;
                                        if (!bIsRestDay && !bIsHoliday && bIsRegOrReg5DayCode
                                            && ((iConvTimeIn1Min <= iConvTimeOut2Min && bIsGraveyard == true)
                                                || (iConvTimeIn1Min >= iConvTimeOut2Min && bIsGraveyard == false))) //Erase logs to avoid overpayment during day to graveyard conversion or vice versa
                                        {
                                            iConvTimeIn1Min = 0;
                                            iConvTimeOut2Min = 0;
                                        }

                                        AddErrorToLaborHourReport(curEmployeeID, dtEmployeeLogLedger.Rows[i]["Emt_LastName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_FirstName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_MiddleName"].ToString(), strProcessDate, "Logs Not Aligned With Shift");
                                    }
                                    #endregion

                                    #region Waived Late and Undertime
                                    if (dtEmployeeLogLedger.Rows[i]["Ell_TagFlex"].ToString() != "")
                                    {
                                        if (dtEmployeeLogLedger.Rows[i]["Ell_TagFlex"].ToString() == "L") //waived late
                                        {
                                            if (iConvTimeIn1Min > 0 && iConvTimeIn1Min > iShiftTimeIn1Min)
                                                iConvTimeIn1Min = iShiftTimeIn1Min;
                                            if (iConvTimeIn2Min > 0 && iConvTimeIn2Min > iShiftTimeIn2Min)
                                                iConvTimeIn2Min = iShiftTimeIn2Min;
                                        }
                                        else if (dtEmployeeLogLedger.Rows[i]["Ell_TagFlex"].ToString() == "U") //waived undertime
                                        {
                                            if (iConvTimeOut1Min > 0 && iConvTimeOut1Min < iShiftTimeOut1Min)
                                                iConvTimeOut1Min = iShiftTimeOut1Min;
                                            if (iConvTimeOut2Min > 0 && iConvTimeOut2Min < iShiftTimeOut2Min)
                                                iConvTimeOut2Min = iShiftTimeOut2Min;
                                        }
                                        else if (dtEmployeeLogLedger.Rows[i]["Ell_TagFlex"].ToString() == "B") //waived late and undertime
                                        {
                                            if (iConvTimeIn1Min > 0 && iConvTimeIn1Min > iShiftTimeIn1Min)
                                                iConvTimeIn1Min = iShiftTimeIn1Min;
                                            if (iConvTimeIn2Min > 0 && iConvTimeIn2Min > iShiftTimeIn2Min)
                                                iConvTimeIn2Min = iShiftTimeIn2Min;
                                            if (iConvTimeOut1Min > 0 && iConvTimeOut1Min < iShiftTimeOut1Min)
                                                iConvTimeOut1Min = iShiftTimeOut1Min;
                                            if (iConvTimeOut2Min > 0 && iConvTimeOut2Min < iShiftTimeOut2Min)
                                                iConvTimeOut2Min = iShiftTimeOut2Min;

                                            if (iConvTimeIn2Min > 0 && iConvTimeIn1Min == 0)
                                            {
                                                iConvTimeIn1Min = iShiftTimeIn1Min;
                                                iConvTimeIn2Min = 0;
                                            }
                                            if (iConvTimeOut1Min > 0 && iConvTimeOut2Min == 0)
                                            {
                                                iConvTimeOut1Min = 0;
                                                iConvTimeOut2Min = iShiftTimeOut2Min;
                                            }
                                        }
                                    }
                                    #endregion

                                    #region Clean Up Converted Time
                                    //NoIN1And2 -> CleanUpOUT1And2
                                    if (iConvTimeIn1Min == 0 && iConvTimeIn2Min == 0 &&
                                        (iConvTimeOut1Min + iConvTimeOut2Min) > 0)
                                    {
                                        iConvTimeOut1Min = 0;
                                        iConvTimeOut2Min = 0;
                                        AddErrorToLaborHourReport(curEmployeeID, dtEmployeeLogLedger.Rows[i]["Emt_LastName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_FirstName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_MiddleName"].ToString(), strProcessDate, "No Actual Time In 1 and 2");
                                    }

                                    //NoOUT1And2 -> CleanUpIN1And2
                                    if (iConvTimeOut1Min == 0 && iConvTimeOut2Min == 0 &&
                                        (iConvTimeIn1Min + iConvTimeIn2Min) > 0)
                                    {
                                        iConvTimeIn1Min = 0;
                                        iConvTimeIn2Min = 0;
                                        AddErrorToLaborHourReport(curEmployeeID, dtEmployeeLogLedger.Rows[i]["Emt_LastName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_FirstName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_MiddleName"].ToString(), strProcessDate, "No Actual Time Out 1 and 2");
                                    }

                                    //NoIN2 -> CleanUpOUT2
                                    if (iConvTimeIn1Min > 0 &&
                                        iConvTimeOut1Min > 0 &&
                                        iConvTimeOut2Min > 0 &&
                                        iConvTimeIn2Min == 0)
                                    {
                                        iConvTimeOut2Min = 0;
                                        AddErrorToLaborHourReport(curEmployeeID, dtEmployeeLogLedger.Rows[i]["Emt_LastName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_FirstName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_MiddleName"].ToString(), strProcessDate, "No Actual Time In 2");
                                    }

                                    //NoIN1 -> CleanUpOUT1
                                    if (iConvTimeIn2Min > 0 &&
                                        iConvTimeOut1Min > 0 &&
                                        iConvTimeOut2Min > 0 &&
                                        iConvTimeIn1Min == 0)
                                    {
                                        iConvTimeOut1Min = 0;
                                        AddErrorToLaborHourReport(curEmployeeID, dtEmployeeLogLedger.Rows[i]["Emt_LastName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_FirstName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_MiddleName"].ToString(), strProcessDate, "No Actual Time In 1");
                                    }

                                    //NoOUT2 -> CleanUpIN2
                                    if (iConvTimeIn1Min > 0 &&
                                        iConvTimeIn2Min > 0 &&
                                        iConvTimeOut1Min > 0 &&
                                        iConvTimeOut2Min == 0)
                                    {
                                        iConvTimeIn2Min = 0;
                                        AddErrorToLaborHourReport(curEmployeeID, dtEmployeeLogLedger.Rows[i]["Emt_LastName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_FirstName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_MiddleName"].ToString(), strProcessDate, "No Actual Time Out 2");
                                    }

                                    //NoOUT1 -> CleanUpIN1
                                    if (iConvTimeIn1Min > 0 &&
                                        iConvTimeIn2Min > 0 &&
                                        iConvTimeOut2Min > 0 &&
                                        iConvTimeOut1Min == 0 &&
                                        iActualTimeIn1Min != 0 &&
                                        iActualTimeIn2Min != 0)
                                    {
                                        iConvTimeIn1Min = 0;
                                        AddErrorToLaborHourReport(curEmployeeID, dtEmployeeLogLedger.Rows[i]["Emt_LastName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_FirstName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_MiddleName"].ToString(), strProcessDate, "No Actual Time Out 1");
                                    }

                                    //Wrong placement - AM Logs
                                    if (bOverrideGraveyardConv == false &&
                                        iConvTimeIn1Min > 0 &&
                                        iConvTimeIn2Min == 0 &&
                                        iConvTimeOut2Min > 0 &&
                                        iConvTimeOut1Min == 0 &&
                                        iConvTimeIn1Min < iConvTimeOut2Min &&
                                        iConvTimeOut2Min < iShiftTimeIn2Min)
                                    {
                                        iConvTimeOut1Min = iConvTimeOut2Min;
                                        iConvTimeOut2Min = 0;
                                    }

                                    //Wrong placement - AM Logs (Scenario 2)
                                    if (bOverrideGraveyardConv == false &&
                                        iConvTimeIn1Min > 0 &&
                                        iConvTimeIn2Min == 0 &&
                                        iConvTimeOut2Min == 0 &&
                                        iConvTimeOut1Min > 0 &&
                                        iConvTimeIn1Min < iConvTimeOut1Min &&
                                        iConvTimeOut1Min >= iShiftTimeIn2Min)
                                    {
                                        iConvTimeOut2Min = iConvTimeOut1Min;
                                        iConvTimeOut1Min = 0;
                                    }

                                    //Wrong placement - PM Logs
                                    if (bOverrideGraveyardConv == false &&
                                        iConvTimeIn1Min > 0 &&
                                        iConvTimeIn2Min == 0 &&
                                        iConvTimeOut2Min > 0 &&
                                        iConvTimeOut1Min == 0 &&
                                        iConvTimeIn1Min < iConvTimeOut2Min &&
                                        iConvTimeIn1Min > iShiftTimeOut1Min)
                                    {
                                        iConvTimeIn2Min = iConvTimeIn1Min;
                                        iConvTimeIn1Min = 0;
                                    }

                                    //Wrong placement - PM Logs (Scenario 2)
                                    if (bOverrideGraveyardConv == false &&
                                        iConvTimeIn1Min == 0 &&
                                        iConvTimeIn2Min > 0 &&
                                        iConvTimeOut2Min > 0 &&
                                        iConvTimeOut1Min == 0 &&
                                        iConvTimeIn2Min < iConvTimeOut2Min &&
                                        iConvTimeIn2Min <= iShiftTimeOut1Min)
                                    {
                                        iConvTimeIn1Min = iConvTimeIn2Min;
                                        iConvTimeIn2Min = 0;
                                    }

                                    //Negative Logs (OUT < IN)
                                    if (bIsGraveyard == false)
                                    {
                                        if (iConvTimeIn1Min > 0 && iConvTimeIn2Min == 0 &&
                                            iConvTimeOut2Min > 0 && iConvTimeOut1Min == 0 &&
                                            iConvTimeIn1Min > iConvTimeOut2Min)
                                        {
                                            iConvTimeIn1Min = 0;
                                            iConvTimeOut2Min = 0;
                                        }
                                        else if (iConvTimeIn1Min > 0 && iConvTimeOut1Min > 0 &&
                                                   iConvTimeIn1Min > iConvTimeOut1Min)
                                        {
                                            iConvTimeIn1Min = 0;
                                            iConvTimeOut1Min = 0;
                                        }
                                        else if (iConvTimeIn2Min > 0 && iConvTimeOut2Min > 0 &&
                                                   iConvTimeIn2Min > iConvTimeOut2Min)
                                        {
                                            iConvTimeIn2Min = 0;
                                            iConvTimeOut2Min = 0;
                                        }
                                    }
                                    #endregion

                                    #region Check Leave Parameters
                                    drArrLeaveAppPaid = GetCorrectedLeaveRecords(curEmployeeID, strProcessDate, true, bIsGraveyard && !bOverrideGraveyardConv, iShiftTimeIn1Min, iShiftTimeOut2Min);
                                    drArrLeaveAppUnpaid = GetCorrectedLeaveRecords(curEmployeeID, strProcessDate, false, bIsGraveyard && !bOverrideGraveyardConv, iShiftTimeIn1Min, iShiftTimeOut2Min);

                                    //Update Encoded Paid and Unpaid Leave if Flex Shift
                                    //if (Convert.ToBoolean(FLEXSHIFT) == true)
                                    //{
                                    //Encoded Paid Leave
                                    dtEmployeeLogLedger.Rows[i]["Ell_EncodedPayLeaveType"] = "";
                                    dtEmployeeLogLedger.Rows[i]["Ell_EncodedPayLeaveHr"] = 0;
                                    foreach (DataRow drRow in drArrLeaveAppPaid)
                                    {
                                        if (Convert.ToDouble(drRow["LeaveHours"]) != 0)
                                        {
                                            //Check if combined leave
                                            if (dtEmployeeLogLedger.Rows[i]["Ell_EncodedPayLeaveType"].ToString() != ""
                                                && dtEmployeeLogLedger.Rows[i]["Ell_EncodedPayLeaveType"].ToString() != drRow["LeaveType"].ToString())
                                            {
                                                drArrLeaveType = dtLeaveType.Select(string.Format("Ltm_LeaveDesc = '{0} + {1}' OR Ltm_LeaveDesc = '{1} + {0}'", dtEmployeeLogLedger.Rows[i]["Ell_EncodedPayLeaveType"].ToString(), drRow["LeaveType"].ToString()));
                                                if (drArrLeaveType.Length > 0)
                                                    dtEmployeeLogLedger.Rows[i]["Ell_EncodedPayLeaveType"] = drArrLeaveType[0]["Ltm_LeaveType"];
                                                else
                                                    dtEmployeeLogLedger.Rows[i]["Ell_EncodedPayLeaveType"] = dtEmployeeLogLedger.Rows[i]["Ell_EncodedPayLeaveType"].ToString().Substring(0, 1) + drRow["LeaveType"].ToString().Substring(0, 1);
                                            }
                                            else
                                                dtEmployeeLogLedger.Rows[i]["Ell_EncodedPayLeaveType"] = drRow["LeaveType"];
                                            dtEmployeeLogLedger.Rows[i]["Ell_EncodedPayLeaveHr"] = Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_EncodedPayLeaveHr"]) + Convert.ToDouble(drRow["LeaveHours"]);
                                        }
                                    }

                                    //Encoded Unpaid Leave
                                    dtEmployeeLogLedger.Rows[i]["Ell_EncodedNoPayLeaveType"] = "";
                                    dtEmployeeLogLedger.Rows[i]["Ell_EncodedNoPayLeaveHr"] = 0;
                                    foreach (DataRow drRow in drArrLeaveAppUnpaid)
                                    {
                                        if (Convert.ToDouble(drRow["LeaveHours"]) != 0)
                                        {
                                            //Check if combined leave
                                            if (dtEmployeeLogLedger.Rows[i]["Ell_EncodedNoPayLeaveType"].ToString() != ""
                                                && dtEmployeeLogLedger.Rows[i]["Ell_EncodedNoPayLeaveType"].ToString() != drRow["LeaveType"].ToString())
                                            {
                                                drArrLeaveType = dtLeaveType.Select(string.Format("Ltm_LeaveDesc = '{0} + {1}' OR Ltm_LeaveDesc = '{1} + {0}'", dtEmployeeLogLedger.Rows[i]["Ell_EncodedNoPayLeaveType"].ToString(), drRow["LeaveType"].ToString()));
                                                if (drArrLeaveType.Length > 0)
                                                    dtEmployeeLogLedger.Rows[i]["Ell_EncodedNoPayLeaveType"] = drArrLeaveType[0]["Ltm_LeaveType"];
                                                else
                                                    dtEmployeeLogLedger.Rows[i]["Ell_EncodedNoPayLeaveType"] = dtEmployeeLogLedger.Rows[i]["Ell_EncodedNoPayLeaveType"].ToString().Substring(0, 1) + drRow["LeaveType"].ToString().Substring(0, 1);
                                            }
                                            else
                                                dtEmployeeLogLedger.Rows[i]["Ell_EncodedNoPayLeaveType"] = drRow["LeaveType"];
                                            dtEmployeeLogLedger.Rows[i]["Ell_EncodedNoPayLeaveHr"] = Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_EncodedNoPayLeaveHr"]) + Convert.ToDouble(drRow["LeaveHours"]);
                                        }
                                    }
                                    //}

                                    //Clear Leave Availments if Assumed Postback is set to TRUE
                                    if (dtEmployeeLogLedger.Rows[i]["Ell_AssumedPostBack"] != DBNull.Value //Assumed Postback flag is equal to T
                                        && dtEmployeeLogLedger.Rows[i]["Ell_AssumedPostBack"].ToString().Equals("T") == true)
                                    {
                                        drArrLeaveAppPaid = dtLeaveTable.Select("EmployeeId = ''");
                                        drArrLeaveAppUnpaid = dtLeaveTable.Select("EmployeeId = ''");
                                    }

                                    //Automatic Leave Availment Adjust to Shift
                                    if (Convert.ToBoolean(ATLVEADJ))
                                    {
                                        AdjustLeaveBasedOnShift(drArrLeaveAppPaid, curEmployeeID, strProcessDate, iShiftTimeIn1Min, iShiftTimeOut1Min, iShiftTimeIn2Min, iShiftTimeOut2Min);
                                        AdjustLeaveBasedOnShift(drArrLeaveAppUnpaid, curEmployeeID, strProcessDate, iShiftTimeIn1Min, iShiftTimeOut1Min, iShiftTimeIn2Min, iShiftTimeOut2Min);
                                    }

                                    //Extend Regular Hours to Leave Hours (Paid Leave)
                                    if (!Convert.ToBoolean(EXTREGLVE))
                                    {
                                        CorrectConvertedTimeWithExtRegLveFlag(drArrLeaveAppPaid, ref iConvTimeIn1Min, ref iConvTimeOut1Min, ref iConvTimeIn2Min, ref iConvTimeOut2Min, iShiftTimeIn1Min, iShiftTimeOut1Min, iShiftTimeIn2Min, iShiftTimeOut2Min, iShiftMin, drArrOTApproved, bIsGraveyard && !bOverrideGraveyardConv);
                                    }

                                    //Extend Regular Hours to Leave Hours (Unpaid Leave)
                                    if (!Convert.ToBoolean(EXTREGULVE))
                                    {
                                        CorrectConvertedTimeWithExtRegLveFlag(drArrLeaveAppUnpaid, ref iConvTimeIn1Min, ref iConvTimeOut1Min, ref iConvTimeIn2Min, ref iConvTimeOut2Min, iShiftTimeIn1Min, iShiftTimeOut1Min, iShiftTimeIn2Min, iShiftTimeOut2Min, iShiftMin, drArrOTApproved, bIsGraveyard && !bOverrideGraveyardConv);
                                    }
                                    #endregion

                                    #region Clean Up Converted Time (2nd pass)
                                    //NoIN1And2 -> CleanUpOUT1And2
                                    if (iConvTimeIn1Min == 0 && iConvTimeIn2Min == 0 &&
                                        (iConvTimeOut1Min + iConvTimeOut2Min) > 0)
                                    {
                                        iConvTimeOut1Min = 0;
                                        iConvTimeOut2Min = 0;
                                        AddErrorToLaborHourReport(curEmployeeID, dtEmployeeLogLedger.Rows[i]["Emt_LastName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_FirstName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_MiddleName"].ToString(), strProcessDate, "No Actual Time In 1 and 2");
                                    }

                                    //NoOUT1And2 -> CleanUpIN1And2
                                    if (iConvTimeOut1Min == 0 && iConvTimeOut2Min == 0 &&
                                        (iConvTimeIn1Min + iConvTimeIn2Min) > 0)
                                    {
                                        iConvTimeIn1Min = 0;
                                        iConvTimeIn2Min = 0;
                                        AddErrorToLaborHourReport(curEmployeeID, dtEmployeeLogLedger.Rows[i]["Emt_LastName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_FirstName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_MiddleName"].ToString(), strProcessDate, "No Actual Time Out 1 and 2");
                                    }

                                    //NoIN2 -> CleanUpOUT2
                                    if (iConvTimeIn1Min > 0 &&
                                        iConvTimeOut1Min > 0 &&
                                        iConvTimeOut2Min > 0 &&
                                        iConvTimeIn2Min == 0)
                                    {
                                        iConvTimeOut2Min = 0;
                                        AddErrorToLaborHourReport(curEmployeeID, dtEmployeeLogLedger.Rows[i]["Emt_LastName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_FirstName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_MiddleName"].ToString(), strProcessDate, "No Actual Time In 2");
                                    }

                                    //NoIN1 -> CleanUpOUT1
                                    if (iConvTimeIn2Min > 0 &&
                                        iConvTimeOut1Min > 0 &&
                                        iConvTimeOut2Min > 0 &&
                                        iConvTimeIn1Min == 0)
                                    {
                                        iConvTimeOut1Min = 0;
                                        AddErrorToLaborHourReport(curEmployeeID, dtEmployeeLogLedger.Rows[i]["Emt_LastName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_FirstName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_MiddleName"].ToString(), strProcessDate, "No Actual Time In 1");
                                    }

                                    //NoOUT2 -> CleanUpIN2
                                    if (iConvTimeIn1Min > 0 &&
                                        iConvTimeIn2Min > 0 &&
                                        iConvTimeOut1Min > 0 &&
                                        iConvTimeOut2Min == 0)
                                    {
                                        iConvTimeIn2Min = 0;
                                        AddErrorToLaborHourReport(curEmployeeID, dtEmployeeLogLedger.Rows[i]["Emt_LastName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_FirstName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_MiddleName"].ToString(), strProcessDate, "No Actual Time Out 2");
                                    }

                                    //NoOUT1 -> CleanUpIN1
                                    if (iConvTimeIn1Min > 0 &&
                                        iConvTimeIn2Min > 0 &&
                                        iConvTimeOut2Min > 0 &&
                                        iConvTimeOut1Min == 0 &&
                                        iActualTimeIn1Min != 0 &&
                                        iActualTimeIn2Min != 0)
                                    {
                                        iConvTimeIn1Min = 0;
                                        AddErrorToLaborHourReport(curEmployeeID, dtEmployeeLogLedger.Rows[i]["Emt_LastName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_FirstName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_MiddleName"].ToString(), strProcessDate, "No Actual Time Out 1");
                                    }

                                    //Wrong placement - AM Logs
                                    if (bOverrideGraveyardConv == false &&
                                        iConvTimeIn1Min > 0 &&
                                        iConvTimeIn2Min == 0 &&
                                        iConvTimeOut2Min > 0 &&
                                        iConvTimeOut1Min == 0 &&
                                        iConvTimeIn1Min < iConvTimeOut2Min &&
                                        iConvTimeOut2Min < iShiftTimeIn2Min)
                                    {
                                        iConvTimeOut1Min = iConvTimeOut2Min;
                                        iConvTimeOut2Min = 0;
                                    }

                                    //Wrong placement - AM Logs (Scenario 2)
                                    if (bOverrideGraveyardConv == false &&
                                        iConvTimeIn1Min > 0 &&
                                        iConvTimeIn2Min == 0 &&
                                        iConvTimeOut2Min == 0 &&
                                        iConvTimeOut1Min > 0 &&
                                        iConvTimeIn1Min < iConvTimeOut1Min &&
                                        iConvTimeOut1Min >= iShiftTimeIn2Min)
                                    {
                                        iConvTimeOut2Min = iConvTimeOut1Min;
                                        iConvTimeOut1Min = 0;
                                    }

                                    //Wrong placement - PM Logs
                                    if (bOverrideGraveyardConv == false &&
                                        iConvTimeIn1Min > 0 &&
                                        iConvTimeIn2Min == 0 &&
                                        iConvTimeOut2Min > 0 &&
                                        iConvTimeOut1Min == 0 &&
                                        iConvTimeIn1Min < iConvTimeOut2Min &&
                                        iConvTimeIn1Min > iShiftTimeOut1Min)
                                    {
                                        iConvTimeIn2Min = iConvTimeIn1Min;
                                        iConvTimeIn1Min = 0;
                                    }

                                    //Wrong placement - PM Logs (Scenario 2)
                                    if (bOverrideGraveyardConv == false &&
                                        iConvTimeIn1Min == 0 &&
                                        iConvTimeIn2Min > 0 &&
                                        iConvTimeOut2Min > 0 &&
                                        iConvTimeOut1Min == 0 &&
                                        iConvTimeIn2Min < iConvTimeOut2Min &&
                                        iConvTimeIn2Min <= iShiftTimeOut1Min)
                                    {
                                        iConvTimeIn1Min = iConvTimeIn2Min;
                                        iConvTimeIn2Min = 0;
                                    }

                                    //Negative Logs (OUT < IN)
                                    if (bIsGraveyard == false)
                                    {
                                        if (iConvTimeIn1Min > 0 && iConvTimeIn2Min == 0 &&
                                            iConvTimeOut2Min > 0 && iConvTimeOut1Min == 0 &&
                                            iConvTimeIn1Min > iConvTimeOut2Min)
                                        {
                                            iConvTimeIn1Min = 0;
                                            iConvTimeOut2Min = 0;
                                        }
                                        else if (iConvTimeIn1Min > 0 && iConvTimeOut1Min > 0 &&
                                                   iConvTimeIn1Min > iConvTimeOut1Min)
                                        {
                                            iConvTimeIn1Min = 0;
                                            iConvTimeOut1Min = 0;
                                        }
                                        else if (iConvTimeIn2Min > 0 && iConvTimeOut2Min > 0 &&
                                                   iConvTimeIn2Min > iConvTimeOut2Min)
                                        {
                                            iConvTimeIn2Min = 0;
                                            iConvTimeOut2Min = 0;
                                        }
                                    }
                                    #endregion

                                    #region Computed Time
                                    //COMPUTED TIME IN 1
                                    iCompTimeIn1Min = 0;
                                    if (iConvTimeIn1Min != 0)
                                    {
                                        iCompTimeIn1Min = (iConvTimeIn1Min <= iShiftTimeIn1Min) ? iShiftTimeIn1Min : iConvTimeIn1Min;
                                    }

                                    //COMPUTED TIME OUT 2
                                    iCompTimeOut2Min = 0;
                                    if (iConvTimeOut2Min != 0 && (iConvTimeIn1Min > 0 || iConvTimeIn2Min > 0))
                                    {
                                        iCompTimeOut2Min = (iConvTimeOut2Min < iShiftTimeOut2Min) ? iConvTimeOut2Min : iShiftTimeOut2Min;
                                    }

                                    //COMPUTED TIME OUT 1
                                    iCompTimeOut1Min = 0;
                                    if (iConvTimeOut1Min == 0)
                                    {
                                        iCompTimeOut1Min = iShiftTimeOut1Min;
                                    }
                                    else
                                    {
                                        iCompTimeOut1Min = (iConvTimeOut1Min < iShiftTimeOut1Min) ? iConvTimeOut1Min : iShiftTimeOut1Min;
                                    }
                                    //Special case
                                    if (!(iConvTimeIn1Min > 0 && (iConvTimeOut1Min > 0 || iConvTimeOut2Min > 0)))
                                    {
                                        iCompTimeOut1Min = 0;
                                    }

                                    //COMPUTED TIME IN 2
                                    iCompTimeIn2Min = 0;
                                    if (iConvTimeIn2Min == 0)
                                    {
                                        iCompTimeIn2Min = iShiftTimeIn2Min;
                                    }
                                    else
                                    {
                                        iCompTimeIn2Min = (iConvTimeIn2Min < iShiftTimeIn2Min) ? iShiftTimeIn2Min : iConvTimeIn2Min;
                                    }
                                    //Special case
                                    if (!(iCompTimeOut2Min > 0))
                                    {
                                        iCompTimeIn2Min = 0;
                                    }

                                    //Manipulate computed time records which are outside shift range
                                    if (bIsOutsideShift)
                                    {
                                        if (iConvTimeIn1Min > 0 && iConvTimeIn1Min >= iShiftTimeOut2Min)
                                        {
                                            iCompTimeIn1Min = 0;
                                            iCompTimeOut1Min = 0;
                                            iCompTimeIn2Min = iConvTimeIn1Min;
                                            iCompTimeOut2Min = iConvTimeOut2Min;
                                        }
                                        else if (iConvTimeOut2Min > 0 && iConvTimeOut2Min <= iShiftTimeIn1Min)
                                        {
                                            iCompTimeIn1Min = iConvTimeIn1Min;
                                            iCompTimeOut1Min = iConvTimeOut2Min;
                                            iCompTimeIn2Min = 0;
                                            iCompTimeOut2Min = 0;
                                        }
                                    }
                                    #endregion

                                    #region //Offset Hours Checking
                                    //if (Convert.ToBoolean(TKOFFSET))
                                    //{
                                    //    #region Accumulated (diminish hours)
                                    //    iOTTemp = 0;
                                    //    iOTTemp2 = 0;
                                    //    drArrOffsetApp = dtOffsetTable.Select("Eof_AccumulatedDate = '" + strProcessDate + "'");
                                    //    foreach (DataRow drOffset in drArrOffsetApp)
                                    //    {
                                    //        iOffsetStartMin = GetMinsFromHourStr(drOffset["Eof_AccumulatedDateTimeStart"].ToString());
                                    //        iOffsetEndMin = GetMinsFromHourStr(drOffset["Eof_AccumulatedDateTimeEnd"].ToString());

                                    //        //iOffsetAmMins = GetOTHoursInTime(iOffsetStartMin, iOffsetEndMin, iConvTimeIn1Min, iCompTimeOut1Min, ref iOTTemp, ref iOTTemp2);
                                    //        //if (iOffsetAmMins > 0)
                                    //        //{
                                    //        //    iConvTimeIn1Min = iOTTemp2;
                                    //        //    if (iConvTimeIn1Min == iCompTimeOut1Min)
                                    //        //    {
                                    //        //        iConvTimeIn1Min = 0;
                                    //        //        iCompTimeOut1Min = 0;
                                    //        //    }
                                    //        //}

                                    //        //iOffsetPmMins = GetOTHoursInTime(iOffsetStartMin, iOffsetEndMin, iCompTimeIn2Min, iConvTimeOut2Min, ref iOTTemp, ref iOTTemp2);
                                    //        //if (iOffsetPmMins > 0)
                                    //        //{
                                    //        //    iConvTimeOut2Min = iOTTemp;
                                    //        //    if (iConvTimeOut2Min == iCompTimeIn2Min)
                                    //        //    {
                                    //        //        iConvTimeOut2Min = 0;
                                    //        //        iCompTimeIn2Min = 0;
                                    //        //    }
                                    //        //}

                                    //        iOffsetAmMins = GetOTHoursInTime(iOffsetStartMin, iOffsetEndMin, iConvTimeIn1Min, iConvTimeOut2Min, ref iOTTemp, ref iOTTemp2);
                                    //        iOffsetPmMins = 0;
                                    //        if (iOffsetAmMins > 0)
                                    //        {
                                    //            iConvTimeIn1Min = 0;
                                    //            iCompTimeOut1Min = 0;
                                    //            iConvTimeOut2Min = 0;
                                    //            iCompTimeIn2Min = 0;
                                    //        }

                                    //        //Update Actual Accumulated Hours
                                    //        if (iOffsetAmMins + iOffsetPmMins > 0)
                                    //        {
                                    //            iForOffsetMin = (iOffsetAmMins + iOffsetPmMins) * -1;
                                    //            drOffset["Eof_ActualAccumulatedMin"] = iOffsetAmMins + iOffsetPmMins;
                                    //            UpdateAccumulatedHours(drOffset["Eof_CurrentPayPeriod"].ToString(), drOffset["Eof_EmployeeId"].ToString(), drOffset["Eof_SeqNo"].ToString(), drOffset["Eof_OffsetDate"].ToString(), iOffsetAmMins + iOffsetPmMins, LoginUser);
                                    //        }
                                    //    }
                                    //    #endregion
                                    //}
                                    #endregion

                                    #region Computed Regular Minutes
                                    //Initialize Hour Fraction Table
                                    InitializeHourFractionTable();

                                    //COMPUTED REGULAR MINUTES
                                    if ((iShiftTimeOut1Min + iShiftTimeIn2Min == 0
                                            || iShiftTimeOut1Min == iShiftTimeIn2Min) //No break time in shift
                                        && (iCompTimeOut1Min + iCompTimeIn2Min == 0
                                            || iCompTimeOut1Min == iCompTimeIn2Min)
                                        && (iCompTimeIn1Min > 0 && (iCompTimeOut2Min - iCompTimeIn1Min) > 0))
                                    {
                                        iComputedRegularMin = iCompTimeOut2Min - iCompTimeIn1Min;
                                        if (iComputedRegularMin < 0)
                                            iComputedRegularMin = 0;
                                        #region Insert Computed Time to Hour Fraction Table
                                        if (!bIsRestDay && !bIsHoliday)
                                            InsertRegularTimeToHourFractionTable(iCompTimeIn1Min, iCompTimeOut2Min);
                                        #endregion
                                    }
                                    else ///With break time in shift
                                    {
                                        if ((iCompTimeOut1Min - iCompTimeIn1Min) > 0)
                                        {
                                            if ((iCompTimeOut2Min - iCompTimeIn2Min) >= 0 || (iShiftTimeIn2Min == 0 && iShiftTimeOut2Min == 0)) //special condition: with Present 2 records OR half-day
                                            {
                                                if ((iCompTimeOut1Min - iCompTimeIn1Min) > 0)
                                                    iComputedRegularMin += (iCompTimeOut1Min - iCompTimeIn1Min); //Add Present 1
                                                #region Insert Computed Time to Hour Fraction Table
                                                if (!bIsRestDay && !bIsHoliday)
                                                    InsertRegularTimeToHourFractionTable(iCompTimeIn1Min, iCompTimeOut1Min);
                                                #endregion
                                            }
                                        }

                                        if ((iCompTimeOut2Min - iCompTimeIn2Min) > 0)
                                        {
                                            if ((iCompTimeOut2Min - iCompTimeIn2Min) > 0)
                                                iComputedRegularMin += (iCompTimeOut2Min - iCompTimeIn2Min); //Add Present 2
                                            #region Insert Computed Time to Hour Fraction Table
                                            if (!bIsRestDay && !bIsHoliday)
                                                InsertRegularTimeToHourFractionTable(iCompTimeIn2Min, iCompTimeOut2Min);
                                            #endregion
                                        }

                                        #region Paid Break for Regular Day
                                        iPaidBreak = 0;
                                        if ((!bIsRestDay && !bIsHoliday && bIsRegOrReg5DayCode) || bDailiesNoWorkNoPay)
                                        {
                                            if ((Convert.ToBoolean(CNTPDBRK) == true
                                                    && ((iConvTimeIn1Min > 0 && iConvTimeOut1Min > 0 && iConvTimeOut1Min >= iShiftTimeOut1Min)
                                                        || (iConvTimeIn2Min > 0 && iConvTimeOut2Min > 0 && iConvTimeIn2Min <= iShiftTimeIn2Min))
                                                    )
                                                || (iConvTimeIn1Min > 0 && iConvTimeOut2Min > 0 && iConvTimeIn1Min <= iShiftTimeOut1Min && iConvTimeOut2Min >= iShiftTimeIn2Min)
                                                ) //Condition to meet paid break
                                            {
                                                iPaidBreak = GetOTHoursInMinutes(iShiftTimeOut1Min, iShiftTimeIn2Min, iShiftTimeOut1Min, iShiftTimeIn2Min);

                                                if (iPaidBreak > iMasterPaidBreak) //Must not exceed the set paid break
                                                {
                                                    iPaidBreak = iMasterPaidBreak;
                                                }
                                                else
                                                {
                                                    #region Insert Computed Time to Hour Fraction Table
                                                    if (!bIsRestDay && !bIsHoliday)
                                                        InsertRegularTimeToHourFractionTable(iShiftTimeOut1Min, iShiftTimeOut1Min + iPaidBreak);
                                                    #endregion
                                                }
                                            }
                                        }
                                        iComputedRegularMin += iPaidBreak;
                                        #endregion

                                        //Special Condition (Override if undertime)
                                        if (iCompTimeIn1Min > 0 && iCompTimeOut2Min > 0 && iCompTimeOut2Min > iCompTimeIn1Min)
                                        {
                                            if (iCompTimeIn2Min > iCompTimeOut2Min) //undertime before break
                                            {
                                                iComputedRegularMin = Math.Min(iCompTimeOut1Min, iCompTimeOut2Min) - iCompTimeIn1Min;
                                                if (iComputedRegularMin < 0)
                                                    iComputedRegularMin = 0;
                                                else
                                                {
                                                    #region Insert Computed Time to Hour Fraction Table
                                                    if (!bIsRestDay && !bIsHoliday)
                                                    {
                                                        InitializeHourFractionTable();
                                                        InsertRegularTimeToHourFractionTable(iCompTimeIn1Min, Math.Min(iCompTimeOut1Min, iCompTimeOut2Min));
                                                    }
                                                    #endregion
                                                }
                                            }
                                            else if (iCompTimeIn1Min > iCompTimeOut1Min) //undertime after break
                                            {
                                                iComputedRegularMin = iCompTimeOut2Min - Math.Max(iCompTimeIn1Min, iCompTimeIn2Min);
                                                if (iComputedRegularMin < 0)
                                                    iComputedRegularMin = 0;
                                                else
                                                {
                                                    #region Insert Computed Time to Hour Fraction Table
                                                    if (!bIsRestDay && !bIsHoliday)
                                                    {
                                                        InitializeHourFractionTable();
                                                        InsertRegularTimeToHourFractionTable(Math.Max(iCompTimeIn1Min, iCompTimeIn2Min), iCompTimeOut2Min);
                                                    }
                                                    #endregion
                                                }
                                            }
                                        }
                                    }
                                    #endregion

                                    #region Computed Late, Undertime, Absent and Leave Minutes
                                    //Condition for regular hour deduction
                                    if ((!bIsRestDay && !bIsHoliday && bIsRegOrReg5DayCode) || bDailiesNoWorkNoPay)
                                    {
                                        //INITIAL COMPUTED DAY WORK MIN
                                        iComputedDayWorkMin = iComputedRegularMin;

                                        ComputeLeaveLateUndertime(drArrLeaveAppPaid, drArrLeaveAppUnpaid, curEmployeeID, strProcessDate
                                                                    , iConvTimeIn1Min, iCompTimeOut1Min, iCompTimeIn2Min, iConvTimeOut2Min
                                                                    , iShiftTimeIn1Min, iShiftTimeOut1Min, iShiftTimeIn2Min, iShiftTimeOut2Min
                                                                    , bIsGraveyard && !bOverrideGraveyardConv, iMasterPaidBreak
                                                                    , ref iComputedLateMin, ref iComputedLate2Min, ref iComputedUndertime1Min, ref iComputedUndertime2Min
                                                                    , ref iPayLeaveMin, ref iNoPayLeaveMin, ref iExcessLeaveMin, ref iLeaveMinToBeAddedToReg, ref iLeaveMinOnPaidBreak);

                                        //Actual Late and Undertime Computation (without parameters)
                                        ComputeLeaveLateUndertime(drArrLeaveAppPaid, drArrLeaveAppUnpaid, curEmployeeID, strProcessDate
                                                                    , iActualTimeIn1MinOrig, iCompTimeOut1Min, iCompTimeIn2Min, iActualTimeOut2MinOrig
                                                                    , iShiftTimeIn1Min, iShiftTimeOut1Min, iShiftTimeIn2Min, iShiftTimeOut2Min
                                                                    , bIsGraveyard && !bOverrideGraveyardConv, iMasterPaidBreak
                                                                    , ref iActualLate1Mins, ref iActualLate2Mins, ref iActualUT1Mins, ref iActualUT2Mins
                                                                    , ref iPayLeaveMinsDummy, ref iNoPayLeaveMinDummy, ref iExcessLeaveMinDummy, ref iLeaveMinToBeAddedToRegDummy, ref iLeaveMinOnPaidBreakDummy);

                                        #region Whole Day Absent
                                        if (iConvTimeIn1Min == 0 && iConvTimeOut1Min == 0 && iConvTimeIn2Min == 0 && iConvTimeOut2Min == 0 && iPayLeaveMin == 0 && iNoPayLeaveMin == 0 && iLeaveMinToBeAddedToReg == 0)
                                        {
                                            //iInitialAbsentMin = Whole Day Absent
                                            iComputedLateMin = 0;
                                            iComputedLate2Min = 0;
                                            iComputedUndertime1Min = 0;
                                            iComputedUndertime2Min = 0;
                                            if (iNoPayLeaveMin == iShiftMin)
                                                iInitialAbsentMin = 0;
                                            else
                                                iInitialAbsentMin = iShiftMin;
                                            iComputedAbsentMin = iInitialAbsentMin;
                                        }
                                        else
                                        {
                                            //Paid Break
                                            if (iMasterPaidBreak > 0 && iPaidBreak == 0 && iLeaveMinOnPaidBreak == 0)
                                            {
                                                if (iPayLeaveMin > 0 && iPayLeaveMin + iMasterPaidBreak == iShiftMin)
                                                {
                                                    //Add to Regular Min
                                                    iPaidBreak = iMasterPaidBreak;
                                                    iComputedRegularMin += iPaidBreak;
                                                    #region Insert Computed Time to Hour Fraction Table
                                                    if (!bIsRestDay && !bIsHoliday)
                                                        InsertRegularTimeToHourFractionTable(iConvTimeIn1Min, iConvTimeIn1Min + iPaidBreak);
                                                    #endregion
                                                }
                                                else
                                                {
                                                    //Add to Undertime Min
                                                    iComputedUndertime2Min += iMasterPaidBreak;
                                                }
                                            }
                                        }

                                        if (iActualTimeIn1MinOrig == 0 && iActualTimeOut2MinOrig == 0 && iPayLeaveMin == 0 && iNoPayLeaveMin == 0 && iLeaveMinToBeAddedToReg == 0)
                                        {
                                            iActualLate1Mins = 0;
                                            iActualLate2Mins = 0;
                                            iActualUT1Mins = 0;
                                            iActualUT2Mins = 0;
                                        }
                                        #endregion

                                        #region Max Late and Undertime Minutes
                                        if (MAXLATEMIN > 0 && iComputedLateMin + iComputedLate2Min > MAXLATEMIN)
                                        {
                                            iInitialAbsentMin += iComputedLateMin + iComputedLate2Min;
                                            iComputedLateMin = 0;
                                            iComputedLate2Min = 0;
                                        }
                                        if (MAXUTMIN > 0 && iComputedUndertime1Min + iComputedUndertime2Min > MAXUTMIN)
                                        {
                                            iInitialAbsentMin += iComputedUndertime1Min + iComputedUndertime2Min;
                                            iComputedUndertime1Min = 0;
                                            iComputedUndertime2Min = 0;
                                        }
                                        #endregion

                                        #region Paid and Unpaid Leave
                                        //Add leave hours with no credit (e.g. OB)
                                        if (((!bIsRestDay && !bIsHoliday && bIsRegOrReg5DayCode) || bDailiesNoWorkNoPay)
                                            && iLeaveMinToBeAddedToReg > 0)
                                        {
                                            if (iComputedRegularMin < iShiftMin && iComputedRegularMin + iLeaveMinToBeAddedToReg <= iShiftMin)
                                            {
                                                iComputedRegularMin += iLeaveMinToBeAddedToReg;
                                                iComputedDayWorkMin += iLeaveMinToBeAddedToReg;
                                                #region Insert Computed Time to Hour Fraction Table
                                                InsertRegularTimeToHourFractionTable(iConvTimeIn1Min, iConvTimeIn1Min + iLeaveMinToBeAddedToReg);
                                                #endregion
                                            }
                                            else if (iShiftMin - iComputedRegularMin < iLeaveMinToBeAddedToReg)
                                            {
                                                iComputedRegularMin += (iShiftMin - iComputedRegularMin);
                                                iComputedDayWorkMin += (iShiftMin - iComputedRegularMin);
                                                #region Insert Computed Time to Hour Fraction Table
                                                InsertRegularTimeToHourFractionTable(iConvTimeIn1Min, iConvTimeIn1Min + iShiftMin - iComputedRegularMin);
                                                #endregion
                                            }
                                        }

                                        if (iPayLeaveMin < 0)
                                        {
                                            AddErrorToLaborHourReport(curEmployeeID, dtEmployeeLogLedger.Rows[i]["Emt_LastName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_FirstName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_MiddleName"].ToString(), strProcessDate, "Negative Paid Leave");
                                        }
                                        if (iNoPayLeaveMin < 0)
                                        {
                                            AddErrorToLaborHourReport(curEmployeeID, dtEmployeeLogLedger.Rows[i]["Emt_LastName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_FirstName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_MiddleName"].ToString(), strProcessDate, "Negative Unpaid Leave");
                                        }
                                        if (iComputedRegularMin > iShiftMin)
                                        {
                                            AddErrorToLaborHourReport(curEmployeeID, dtEmployeeLogLedger.Rows[i]["Emt_LastName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_FirstName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_MiddleName"].ToString(), strProcessDate, "Regular Hrs > Shift Hrs");
                                        }
                                        #endregion

                                        #region Compute Deduction Minutes
                                        //Computed Absent Min
                                        if (iComputedRegularMin > 0 && iShiftMin - iComputedRegularMin > 0)
                                        {
                                            iComputedAbsentMin = iShiftMin - iComputedRegularMin;
                                        }
                                        else if (iComputedRegularMin == 0)
                                        {
                                            iComputedAbsentMin = iShiftMin;

                                            //Zero-out Actual Late and Undertime because employee has no worked hours
                                            iActualLate1Mins = 0;
                                            iActualLate2Mins = 0;
                                            iActualUT1Mins = 0;
                                            iActualUT2Mins = 0;
                                        }
                                        else
                                        {
                                            iComputedAbsentMin = 0;
                                        }

                                        //Cleanup Paid Leave
                                        if (iPayLeaveMin > 0)
                                        {
                                            //Just in case the leave hours filed is greater than the shift hours
                                            if (iShiftMin - iPayLeaveMin < 0)
                                                iPayLeaveMin = iShiftMin;

                                            if (iPayLeaveMin <= iComputedAbsentMin)
                                            {
                                                iComputedAbsentMin -= iPayLeaveMin;
                                                iComputedDayWorkMin += iPayLeaveMin;
                                            }
                                            else
                                            {
                                                if (Convert.ToBoolean(FLEXSHIFT) == true && Convert.ToBoolean(EXTREGLVE) == false)
                                                {
                                                    iComputedRegularMin = iShiftMin - iPayLeaveMin;
                                                    iComputedDayWorkMin = iShiftMin;
                                                }
                                                else
                                                {
                                                    iPayLeaveMin = iComputedAbsentMin;
                                                    iComputedDayWorkMin += iComputedAbsentMin;
                                                }

                                                iComputedAbsentMin = 0;
                                                iComputedLateMin = 0;
                                                iComputedLate2Min = 0;
                                                iComputedUndertime1Min = 0;
                                                iComputedUndertime2Min = 0;
                                                iInitialAbsentMin = 0;
                                                iActualLate1Mins = 0;
                                                iActualLate2Mins = 0;
                                                iActualUT1Mins = 0;
                                                iActualUT2Mins = 0;
                                            }
                                        }

                                        //Cleanup Unpaid Leave
                                        if (iNoPayLeaveMin > 0)
                                        {
                                            //Just in case the leave hours filed is greater than the shift hours
                                            if (iShiftMin - iNoPayLeaveMin < 0)
                                                iNoPayLeaveMin = iShiftMin;

                                            if (iNoPayLeaveMin > iComputedAbsentMin)
                                            {
                                                if (Convert.ToBoolean(FLEXSHIFT) == true && Convert.ToBoolean(EXTREGULVE) == false)
                                                {
                                                    iComputedRegularMin = iShiftMin - iNoPayLeaveMin;
                                                    iComputedDayWorkMin = iComputedRegularMin;
                                                }
                                                else
                                                {
                                                    iNoPayLeaveMin = iComputedAbsentMin;
                                                }

                                                iComputedAbsentMin = 0;
                                                iComputedLateMin = 0;
                                                iComputedLate2Min = 0;
                                                iComputedUndertime1Min = 0;
                                                iComputedUndertime2Min = 0;
                                                iInitialAbsentMin = 0;
                                                iActualLate1Mins = 0;
                                                iActualLate2Mins = 0;
                                                iActualUT1Mins = 0;
                                                iActualUT2Mins = 0;
                                            }
                                        }

                                        //Total Absent Minutes
                                        iTotalComputedAbsentMin = iComputedLateMin + iComputedLate2Min + iComputedUndertime1Min + iComputedUndertime2Min + iInitialAbsentMin + iNoPayLeaveMin;
                                        #endregion
                                    }
                                    #endregion

                                    #region Overtime and Night Premium Computation
                                    #region Overtime Applications Loop
                                    //Initialize Encoded Advance and Post Overtime if Flex Shift
                                    if (Convert.ToBoolean(FLEXSHIFT) == true)
                                    {
                                        dtEmployeeLogLedger.Rows[i]["Ell_EncodedOvertimeAdvHr"] = 0;
                                        dtEmployeeLogLedger.Rows[i]["Ell_EncodedOvertimePostHr"] = 0;
                                    }
                                    foreach (DataRow drOTApp in drArrOTApproved)
                                    {
                                        iOTStartMin = GetMinsFromHourStr(drOTApp["Eot_StartTime"].ToString());
                                        iOTEndMin = GetMinsFromHourStr(drOTApp["Eot_EndTime"].ToString());
                                        strOTType = drOTApp["Eot_OvertimeType"].ToString();

                                        #region OT Application Validation
                                        if (strOTType.Equals("A") && iOTEndMin > iShiftTimeIn1Min)
                                        {
                                            AddErrorToLaborHourReport(curEmployeeID, dtEmployeeLogLedger.Rows[i]["Emt_LastName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_FirstName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_MiddleName"].ToString(), strProcessDate, "Advance OT Application Error");
                                            continue; //skip erroneous OT application
                                        }

                                        bCountOTFraction = false;
                                        if (Convert.ToBoolean(RNDOTFRAC) == true
                                            && (Convert.ToInt32(iShiftTimeOut1Min / iOTFraction) * iOTFraction == iShiftTimeOut1Min
                                                || Convert.ToInt32(iShiftTimeIn2Min / iOTFraction) * iOTFraction == iShiftTimeIn2Min))
                                            bCountOTFraction = true;

                                        if (bIsGraveyard && strOTType.Equals("P") && !bOverrideGraveyardConv) //Graveyard shift and Post-overtime
                                        {
                                            if (iOTStartMin < (iShiftTimeIn1Min - LOGPAD))
                                            {
                                                iOTStartMin += GRAVEYARD24;
                                            }
                                            if (iOTEndMin < (iShiftTimeOut2Min - LOGPAD))
                                            {
                                                iOTEndMin += GRAVEYARD24;
                                            }
                                        }

                                        //Encoded Overtime Hours and Minutes
                                        if (iOTEndMin - iOTStartMin > 0)
                                            iEncodedOvertimeMin += (iOTEndMin - iOTStartMin);

                                        //Rounding of OT in favor of Employee
                                        if (Convert.ToBoolean(OTROUNDING) == true)
                                        {
                                            iConvTimeIn1Min = CleanUpByRoundLow(iConvTimeIn1Min, iOTFraction, dal);
                                            iConvTimeOut1Min = CleanUpByRoundHigh(iConvTimeOut1Min, iOTFraction, dal);
                                            iConvTimeIn2Min = CleanUpByRoundLow(iConvTimeIn2Min, iOTFraction, dal);
                                            iConvTimeOut2Min = CleanUpByRoundHigh(iConvTimeOut2Min, iOTFraction, dal);
                                        }

                                        //Update Encoded Advance and Post Overtime Hour if Flex Shift
                                        if (Convert.ToBoolean(FLEXSHIFT) == true)
                                        {
                                            if (strOTType.Equals("A"))
                                            {
                                                dtEmployeeLogLedger.Rows[i]["Ell_EncodedOvertimeAdvHr"] = Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_EncodedOvertimeAdvHr"]) + ((iOTEndMin - iOTStartMin) / 60.0);
                                                if (iOTEndMin - iOTStartMin > 0)
                                                    iEncodedOvertimeMin += (iOTEndMin - iOTStartMin);
                                            }
                                            if (strOTType.Equals("P"))
                                            {
                                                dtEmployeeLogLedger.Rows[i]["Ell_EncodedOvertimePostHr"] = Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_EncodedOvertimePostHr"]) + ((iOTEndMin - iOTStartMin) / 60.0);
                                                if (iOTEndMin - iOTStartMin > 0)
                                                    iEncodedOvertimeMin += (iOTEndMin - iOTStartMin);
                                            }
                                        }
                                        #endregion

                                        if (bIsRestDay || bIsHoliday || !bIsRegOrReg5DayCode) //Rest day or holiday
                                        {
                                            #region Computed Overtime Minutes
                                            //[In-between OT = Get OT between ConvertedIn1 and ConvertedOut2]
                                            iAdvOTMin = GetOTHoursInMinutes(iOTStartMin, iOTEndMin, iConvTimeIn1Min, iCompTimeOut1Min);
                                            if (bCountOTFraction == true)
                                            {
                                                iAdvOTMin = Convert.ToInt32(iAdvOTMin / iOTFraction) * iOTFraction;
                                            }
                                            if (strOTType.Equals("A"))
                                            {
                                                iAdjShiftMin += iAdvOTMin;
                                            }
                                            iComputedOvertimeMin += iAdvOTMin;

                                            #region Insert Overtime to Hour Fraction Table
                                            InsertOTToHourFractionTable(iOTStartMin, iOTEndMin, iConvTimeIn1Min, iCompTimeOut1Min, iOTFraction, bCountOTFraction);
                                            #endregion

                                            if (bIsOutsideShift && iComputedOvertimeMin > 0)
                                            {
                                                bIsOutsideShiftComputedOT = true;
                                            }
                                            iAdvOTMin = GetOTHoursInMinutes(iOTStartMin, iOTEndMin, iCompTimeIn2Min, iConvTimeOut2Min);
                                            if (bCountOTFraction == true)
                                            {
                                                iAdvOTMin = Convert.ToInt32(iAdvOTMin / iOTFraction) * iOTFraction;
                                            }
                                            if (strOTType.Equals("A"))
                                            {
                                                iAdjShiftMin += iAdvOTMin;
                                            }
                                            if (!bIsOutsideShift || !bIsOutsideShiftComputedOT)
                                            {
                                                iComputedOvertimeMin += iAdvOTMin;

                                                #region Insert Overtime to Hour Fraction Table
                                                InsertOTToHourFractionTable(iOTStartMin, iOTEndMin, iCompTimeIn2Min, iConvTimeOut2Min, iOTFraction, bCountOTFraction);
                                                #endregion
                                            }

                                            if (bCountOTFraction == false)
                                            {
                                                iComputedOvertimeMin = Convert.ToInt32(iComputedOvertimeMin / iOTFraction) * iOTFraction;
                                                CorrectOTHourFraction(iComputedOvertimeMin);
                                            }

                                            //Paid Break for Rest day
                                            if ((iConvTimeIn1Min > 0 && iConvTimeOut2Min > 0 && iConvTimeIn1Min <= iShiftTimeOut1Min && iConvTimeOut2Min >= iShiftTimeIn2Min)
                                                //|| (iConvTimeIn1Min > 0 && iConvTimeOut1Min > 0 && iConvTimeOut1Min >= iShiftTimeIn2Min)
                                                //|| (iConvTimeIn2Min > 0 && iConvTimeOut2Min > 0 && iConvTimeIn2Min <= iShiftTimeIn2Min)
                                                ) //Condition to meet paid break
                                            {
                                                iPaidBreak += GetOTHoursInMinutes(iShiftTimeOut1Min, iShiftTimeIn2Min, iOTStartMin, iOTEndMin);

                                                if (iPaidBreak > iMasterPaidBreak) //Must not exceed the set paid break
                                                {
                                                    iPaidBreak = iMasterPaidBreak;
                                                }
                                                else
                                                {
                                                    if (MIDOT == false)
                                                    {
                                                        #region Insert Paid Break to Hour Fraction Table
                                                        InsertOTToHourFractionTable(iShiftTimeOut1Min, iShiftTimeIn2Min, iOTStartMin, iOTEndMin, iOTFraction, bCountOTFraction);
                                                        #endregion
                                                    }
                                                }
                                            }

                                            if (MIDOT == true && strOTType.Equals("M"))
                                            {
                                                //[Mid OT = Get OT between ComputedOut1 and ComputedIn2]
                                                iAdvOTMin = GetOTHoursInMinutes(iOTStartMin, iOTEndMin
                                                                                , Math.Max(Math.Max(iCompTimeOut1Min, iConvTimeIn2Min), iConvTimeIn1Min)
                                                                                , Math.Min(Math.Max(iCompTimeIn2Min, iConvTimeOut1Min), iConvTimeOut2Min));
                                                if (bCountOTFraction == true)
                                                {
                                                    iAdvOTMin = Convert.ToInt32(iAdvOTMin / iOTFraction) * iOTFraction;
                                                }
                                                iComputedOvertimeMin += iAdvOTMin;

                                                #region Insert Overtime to Hour Fraction Table
                                                InsertOTToHourFractionTable(iOTStartMin, iOTEndMin
                                                                            , Math.Max(Math.Max(iCompTimeOut1Min, iConvTimeIn2Min), iConvTimeIn1Min)
                                                                            , Math.Max(Math.Max(iCompTimeIn2Min, iConvTimeOut1Min), iConvTimeOut2Min)
                                                                            , iOTFraction, bCountOTFraction);
                                                #endregion
                                                iPaidBreak = 0;
                                            }
                                            #endregion

                                            if (NDBRCKTCNT == 2)
                                            {
                                                #region Computed Overtime Night Premium (Sharp)
                                                if ((iConvTimeIn1Min > 0 && iConvTimeOut2Min > 0 && iConvTimeIn1Min <= iShiftTimeOut1Min && iConvTimeOut2Min >= iShiftTimeIn2Min)
                                                    //|| (iConvTimeIn1Min > 0 && iConvTimeOut1Min > 0 && iConvTimeOut1Min >= iShiftTimeIn2Min)
                                                    //|| (iConvTimeIn2Min > 0 && iConvTimeOut2Min > 0 && iConvTimeIn2Min <= iShiftTimeIn2Min)
                                                    ) //Condition to meet paid break
                                                {
                                                    iBreakMin = (iCompTimeIn1Min > iCompTimeOut1Min) ? iCompTimeIn1Min : iCompTimeOut1Min;

                                                    #region ND Bracket 1
                                                    iOTTemp = (NP1_BEGTIME > iOTStartMin) ? NP1_BEGTIME : iOTStartMin;
                                                    iOTTemp2 = (NP1_ENDTIME < iOTEndMin) ? NP1_ENDTIME : iOTEndMin;

                                                    iCompRegNightPremMin += GetOTHoursInMinutes(iOTTemp, iOTTemp2, iConvTimeIn1Min, iCompTimeOut1Min);
                                                    #region Insert ND Hour to Hour Fraction Table
                                                    InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iConvTimeIn1Min, iCompTimeOut1Min, NDFRACTION, HourType.NDHour);
                                                    #endregion

                                                    iCompRegNightPremMin += GetOTHoursInMinutes(iOTTemp, iOTTemp2, iCompTimeIn2Min, iConvTimeOut2Min);
                                                    #region Insert ND Hour to Hour Fraction Table
                                                    InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iCompTimeIn2Min, iConvTimeOut2Min, NDFRACTION, HourType.NDHour);
                                                    #endregion

                                                    iTimeMinTemp = GetOTHoursInMinutes(iOTTemp, iOTTemp2, iBreakMin, iCompTimeIn2Min);
                                                    if (iTimeMinTemp > 0)
                                                    {
                                                        iCompRegNightPremMin += iTimeMinTemp;
                                                        #region Insert ND Hour to Hour Fraction Table
                                                        InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iBreakMin, iCompTimeIn2Min, NDFRACTION, HourType.NDHour);
                                                        #endregion
                                                    }
                                                    #endregion

                                                    #region ND Bracket 1 (Early ND)
                                                    if (NP1_ENDTIME < NP1_BEGTIME
                                                        || NP1_BEGTIME > GRAVEYARD24
                                                        || NP1_ENDTIME > GRAVEYARD24)
                                                    {
                                                        if (NP1_BEGTIME > GRAVEYARD24)
                                                            iTimeMinTemp = NP1_BEGTIME - GRAVEYARD24;
                                                        else
                                                            iTimeMinTemp = NP1_BEGTIME;

                                                        if (NP1_ENDTIME > GRAVEYARD24)
                                                            iTimeMinTemp2 = NP1_ENDTIME - GRAVEYARD24;
                                                        else
                                                            iTimeMinTemp2 = NP1_ENDTIME;

                                                        if (iTimeMinTemp2 < iTimeMinTemp)
                                                            iTimeMinTemp = 0;

                                                        iOTTemp = (iTimeMinTemp > iOTStartMin) ? iTimeMinTemp : iOTStartMin;
                                                        iOTTemp2 = (iTimeMinTemp2 < iOTEndMin) ? iTimeMinTemp2 : iOTEndMin;

                                                        iCompRegNightPremMin += GetOTHoursInMinutes(iOTTemp, iOTTemp2, iConvTimeIn1Min, iCompTimeOut1Min);
                                                        #region Insert ND Hour to Hour Fraction Table
                                                        InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iConvTimeIn1Min, iCompTimeOut1Min, NDFRACTION, HourType.NDHour);
                                                        #endregion

                                                        iCompRegNightPremMin += GetOTHoursInMinutes(iOTTemp, iOTTemp2, iCompTimeIn2Min, iConvTimeOut2Min);
                                                        #region Insert ND Hour to Hour Fraction Table
                                                        InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iCompTimeIn2Min, iConvTimeOut2Min, NDFRACTION, HourType.NDHour);
                                                        #endregion

                                                        iTimeMinTemp = GetOTHoursInMinutes(iOTTemp, iOTTemp2, iBreakMin, iCompTimeIn2Min);
                                                        if (iTimeMinTemp > 0)
                                                        {
                                                            iCompRegNightPremMin += iTimeMinTemp;
                                                            #region Insert ND Hour to Hour Fraction Table
                                                            InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iBreakMin, iCompTimeIn2Min, NDFRACTION, HourType.NDHour);
                                                            #endregion
                                                        }
                                                    }
                                                    #endregion

                                                    #region ND Bracket 2
                                                    iOTTemp = (NP2_BEGTIME > iOTStartMin) ? NP2_BEGTIME : iOTStartMin;
                                                    iOTTemp2 = (NP2_ENDTIME < iOTEndMin) ? NP2_ENDTIME : iOTEndMin;

                                                    iCompOvertimeNightPremMin += GetOTHoursInMinutes(iOTTemp, iOTTemp2, iConvTimeIn1Min, iCompTimeOut1Min);
                                                    #region Insert ND Hour to Hour Fraction Table
                                                    InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iConvTimeIn1Min, iCompTimeOut1Min, NDFRACTION, HourType.NDOTHour);
                                                    #endregion

                                                    iCompOvertimeNightPremMin += GetOTHoursInMinutes(iOTTemp, iOTTemp2, iCompTimeIn2Min, iConvTimeOut2Min);
                                                    #region Insert ND Hour to Hour Fraction Table
                                                    InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iCompTimeIn2Min, iConvTimeOut2Min, NDFRACTION, HourType.NDOTHour);
                                                    #endregion

                                                    iTimeMinTemp = GetOTHoursInMinutes(iOTTemp, iOTTemp2, iBreakMin, iCompTimeIn2Min);
                                                    if (iTimeMinTemp > 0)
                                                    {
                                                        iCompOvertimeNightPremMin += iTimeMinTemp;
                                                        #region Insert ND Hour to Hour Fraction Table
                                                        InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iBreakMin, iCompTimeIn2Min, NDFRACTION, HourType.NDOTHour);
                                                        #endregion
                                                    }
                                                    #endregion

                                                    #region ND Bracket 2 (Early ND)
                                                    if (NP2_ENDTIME < NP2_BEGTIME
                                                        || NP2_BEGTIME > GRAVEYARD24
                                                        || NP2_ENDTIME > GRAVEYARD24)
                                                    {
                                                        if (NP2_BEGTIME > GRAVEYARD24)
                                                            iTimeMinTemp = NP2_BEGTIME - GRAVEYARD24;
                                                        else
                                                            iTimeMinTemp = NP2_BEGTIME;

                                                        if (NP2_ENDTIME > GRAVEYARD24)
                                                            iTimeMinTemp2 = NP2_ENDTIME - GRAVEYARD24;
                                                        else
                                                            iTimeMinTemp2 = NP2_ENDTIME;

                                                        if (iTimeMinTemp2 < iTimeMinTemp)
                                                            iTimeMinTemp = 0;

                                                        iOTTemp = (iTimeMinTemp > iOTStartMin) ? iTimeMinTemp : iOTStartMin;
                                                        iOTTemp2 = (iTimeMinTemp2 < iOTEndMin) ? iTimeMinTemp2 : iOTEndMin;

                                                        iCompOvertimeNightPremMin += GetOTHoursInMinutes(iOTTemp, iOTTemp2, iConvTimeIn1Min, iCompTimeOut1Min);
                                                        #region Insert ND Hour to Hour Fraction Table
                                                        InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iConvTimeIn1Min, iCompTimeOut1Min, NDFRACTION, HourType.NDOTHour);
                                                        #endregion

                                                        iCompOvertimeNightPremMin += GetOTHoursInMinutes(iOTTemp, iOTTemp2, iCompTimeIn2Min, iConvTimeOut2Min);
                                                        #region Insert ND Hour to Hour Fraction Table
                                                        InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iCompTimeIn2Min, iConvTimeOut2Min, NDFRACTION, HourType.NDOTHour);
                                                        #endregion

                                                        iTimeMinTemp = GetOTHoursInMinutes(iOTTemp, iOTTemp2, iBreakMin, iCompTimeIn2Min);
                                                        if (iTimeMinTemp > 0)
                                                        {
                                                            iCompOvertimeNightPremMin += iTimeMinTemp;
                                                            #region Insert ND Hour to Hour Fraction Table
                                                            InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iBreakMin, iCompTimeIn2Min, NDFRACTION, HourType.NDOTHour);
                                                            #endregion
                                                        }
                                                    }
                                                    #endregion
                                                }
                                                else
                                                {
                                                    iTimeMinTemp = (iConvTimeIn1Min > 0) ? iConvTimeIn1Min : iConvTimeIn2Min;
                                                    iTimeMinTemp2 = (iConvTimeOut2Min > 0) ? iConvTimeOut2Min : iConvTimeOut1Min;
                                                    if (iTimeMinTemp > 0 && iTimeMinTemp2 > 0)
                                                    {
                                                        #region ND Bracket 1
                                                        iOTTemp = (NP1_BEGTIME > iOTStartMin) ? NP1_BEGTIME : iOTStartMin;
                                                        iOTTemp2 = (NP1_ENDTIME < iOTEndMin) ? NP1_ENDTIME : iOTEndMin;
                                                        iCompRegNightPremMin += GetOTHoursInMinutes(iOTTemp, iOTTemp2, iTimeMinTemp, iTimeMinTemp2);
                                                        #region Insert ND Hour to Hour Fraction Table
                                                        InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iTimeMinTemp, iTimeMinTemp2, NDFRACTION, HourType.NDHour);
                                                        #endregion
                                                        #endregion

                                                        #region ND Bracket 1 (Early ND)
                                                        if (NP1_ENDTIME < NP1_BEGTIME
                                                            || NP1_BEGTIME > GRAVEYARD24
                                                            || NP1_ENDTIME > GRAVEYARD24)
                                                        {
                                                            if (NP1_BEGTIME > GRAVEYARD24)
                                                                iTimeMinTemp3 = NP1_BEGTIME - GRAVEYARD24;
                                                            else
                                                                iTimeMinTemp3 = NP1_BEGTIME;

                                                            if (NP1_ENDTIME > GRAVEYARD24)
                                                                iTimeMinTemp4 = NP1_ENDTIME - GRAVEYARD24;
                                                            else
                                                                iTimeMinTemp4 = NP1_ENDTIME;

                                                            if (iTimeMinTemp4 < iTimeMinTemp3)
                                                                iTimeMinTemp3 = 0;

                                                            iOTTemp = (iTimeMinTemp3 > iOTStartMin) ? iTimeMinTemp3 : iOTStartMin;
                                                            iOTTemp2 = (iTimeMinTemp4 < iOTEndMin) ? iTimeMinTemp4 : iOTEndMin;
                                                            iCompRegNightPremMin += GetOTHoursInMinutes(iOTTemp, iOTTemp2, iTimeMinTemp, iTimeMinTemp2);
                                                            #region Insert ND Hour to Hour Fraction Table
                                                            InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iTimeMinTemp, iTimeMinTemp2, NDFRACTION, HourType.NDHour);
                                                            #endregion
                                                        }
                                                        #endregion

                                                        #region ND Bracket 2
                                                        iOTTemp = (NP2_BEGTIME > iOTStartMin) ? NP2_BEGTIME : iOTStartMin;
                                                        iOTTemp2 = (NP2_ENDTIME < iOTEndMin) ? NP2_ENDTIME : iOTEndMin;
                                                        iCompOvertimeNightPremMin += GetOTHoursInMinutes(iOTTemp, iOTTemp2, iTimeMinTemp, iTimeMinTemp2);
                                                        #region Insert ND Hour to Hour Fraction Table
                                                        InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iTimeMinTemp, iTimeMinTemp2, NDFRACTION, HourType.NDOTHour);
                                                        #endregion
                                                        #endregion

                                                        #region ND Bracket 2 (Early ND)
                                                        if (NP2_ENDTIME < NP2_BEGTIME
                                                            || NP2_BEGTIME > GRAVEYARD24
                                                            || NP2_ENDTIME > GRAVEYARD24)
                                                        {
                                                            if (NP2_BEGTIME > GRAVEYARD24)
                                                                iTimeMinTemp3 = NP2_BEGTIME - GRAVEYARD24;
                                                            else
                                                                iTimeMinTemp3 = NP2_BEGTIME;

                                                            if (NP2_ENDTIME > GRAVEYARD24)
                                                                iTimeMinTemp4 = NP2_ENDTIME - GRAVEYARD24;
                                                            else
                                                                iTimeMinTemp4 = NP2_ENDTIME;

                                                            if (iTimeMinTemp4 < iTimeMinTemp3)
                                                                iTimeMinTemp3 = 0;

                                                            iOTTemp = (iTimeMinTemp3 > iOTStartMin) ? iTimeMinTemp3 : iOTStartMin;
                                                            iOTTemp2 = (iTimeMinTemp4 < iOTEndMin) ? iTimeMinTemp4 : iOTEndMin;
                                                            iCompOvertimeNightPremMin += GetOTHoursInMinutes(iOTTemp, iOTTemp2, iTimeMinTemp, iTimeMinTemp2);
                                                            #region Insert ND Hour to Hour Fraction Table
                                                            InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iTimeMinTemp, iTimeMinTemp2, NDFRACTION, HourType.NDOTHour);
                                                            #endregion
                                                        }
                                                        #endregion
                                                    }
                                                }
                                                #endregion
                                            }
                                            else
                                            {
                                                #region Computed Overtime Night Premium (Normal)
                                                if ((iConvTimeIn1Min > 0 && iConvTimeOut2Min > 0 && iConvTimeIn1Min <= iShiftTimeOut1Min && iConvTimeOut2Min >= iShiftTimeIn2Min)
                                                    //|| (iConvTimeIn1Min > 0 && iConvTimeOut1Min > 0 && iConvTimeOut1Min >= iShiftTimeIn2Min)
                                                    //|| (iConvTimeIn2Min > 0 && iConvTimeOut2Min > 0 && iConvTimeIn2Min <= iShiftTimeIn2Min)
                                                    ) //Condition to meet paid break
                                                {
                                                    //Break between shifts
                                                    iBreakMin = (iCompTimeIn1Min > iCompTimeOut1Min) ? iCompTimeIn1Min : iCompTimeOut1Min;

                                                    ///OVERTIME NIGHT PREMIUM FOR DAY SHIFTS
                                                    if (!Convert.ToBoolean(NONDOTDAY)) //HOYA
                                                    {
                                                        if (Convert.ToBoolean(NDSPLTSHFT)) //HOEP: Special Case
                                                        {
                                                            iOTTemp = (NIGHTDIFFAMSTART > iOTStartMin) ? NIGHTDIFFAMSTART : iOTStartMin;
                                                            iOTTemp2 = (NIGHTDIFFAMEND < iOTEndMin) ? NIGHTDIFFAMEND : iOTEndMin;

                                                            iCompOvertimeNightPremMin += GetOTHoursInMinutes(iOTTemp, iOTTemp2, iConvTimeIn1Min, iCompTimeIn1Min);
                                                            #region Insert ND Hour to Hour Fraction Table
                                                            InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iConvTimeIn1Min, iCompTimeIn1Min, NDFRACTION, HourType.NDOTHour);
                                                            #endregion

                                                            iCompRegNightPremMin += GetOTHoursInMinutes(iOTTemp, iOTTemp2, iCompTimeIn1Min, iCompTimeOut1Min);
                                                            #region Insert ND Hour to Hour Fraction Table
                                                            InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iCompTimeIn1Min, iCompTimeOut1Min, NDFRACTION, HourType.NDHour);
                                                            #endregion

                                                            if (Convert.ToBoolean(NDCNTBREAK))
                                                            {
                                                                iCompRegNightPremMin += GetOTHoursInMinutes(iOTTemp, iOTTemp2, iCompTimeOut1Min, iCompTimeIn2Min);
                                                                #region Insert ND Hour to Hour Fraction Table
                                                                InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iCompTimeOut1Min, iCompTimeIn2Min, NDFRACTION, HourType.NDHour);
                                                                #endregion
                                                            }

                                                            iCompRegNightPremMin += GetOTHoursInMinutes(iOTTemp, iOTTemp2, iCompTimeIn2Min, iCompTimeOut2Min);
                                                            #region Insert ND Hour to Hour Fraction Table
                                                            InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iCompTimeIn2Min, iCompTimeOut2Min, NDFRACTION, HourType.NDHour);
                                                            #endregion

                                                            iCompOvertimeNightPremMin += GetOTHoursInMinutes(iOTTemp, iOTTemp2, iCompTimeOut2Min, iConvTimeOut2Min);
                                                            #region Insert ND Hour to Hour Fraction Table
                                                            InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iCompTimeOut2Min, iConvTimeOut2Min, NDFRACTION, HourType.NDOTHour);
                                                            #endregion
                                                        }
                                                        else
                                                        {
                                                            iOTTemp = (NIGHTDIFFAMSTART > iOTStartMin) ? NIGHTDIFFAMSTART : iOTStartMin;
                                                            iOTTemp2 = (NIGHTDIFFAMEND < iOTEndMin) ? NIGHTDIFFAMEND : iOTEndMin;

                                                            iCompOvertimeNightPremMin += GetOTHoursInMinutes(iOTTemp, iOTTemp2, iConvTimeIn1Min, iCompTimeOut1Min);
                                                            #region Insert ND Hour to Hour Fraction Table
                                                            InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iConvTimeIn1Min, iCompTimeOut1Min, NDFRACTION, HourType.NDOTHour);
                                                            #endregion

                                                            iCompOvertimeNightPremMin += GetOTHoursInMinutes(iOTTemp, iOTTemp2, iCompTimeIn2Min, iConvTimeOut2Min);
                                                            #region Insert ND Hour to Hour Fraction Table
                                                            InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iCompTimeIn2Min, iConvTimeOut2Min, NDFRACTION, HourType.NDOTHour);
                                                            #endregion

                                                            iTimeMinTemp = GetOTHoursInMinutes(iOTTemp, iOTTemp2, iBreakMin, iCompTimeIn2Min);
                                                            //if (iTimeMinTemp > 0 && iTimeMinTemp <= iMasterPaidBreak)
                                                            if (iTimeMinTemp > 0 && Convert.ToBoolean(NDCNTBREAK))
                                                            {
                                                                iCompOvertimeNightPremMin += iTimeMinTemp;
                                                                #region Insert ND Hour to Hour Fraction Table
                                                                InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iBreakMin, iCompTimeIn2Min, NDFRACTION, HourType.NDOTHour);
                                                                #endregion
                                                            }
                                                        }
                                                    }

                                                    ///OVERTIME NIGHT PREMIUM FOR GRAVEYARD SHIFTS
                                                    if (Convert.ToBoolean(NDSPLTSHFT)) //HOEP: Special Case
                                                    {
                                                        iOTTemp = (NIGHTDIFFGRAVEYARDSTART > iOTStartMin) ? NIGHTDIFFGRAVEYARDSTART : iOTStartMin;
                                                        iOTTemp2 = (NIGHTDIFFGRAVEYARDEND < iOTEndMin) ? NIGHTDIFFGRAVEYARDEND : iOTEndMin;

                                                        iCompOvertimeNightPremMin += GetOTHoursInMinutes(iOTTemp, iOTTemp2, iConvTimeIn1Min, iCompTimeIn1Min);
                                                        #region Insert ND Hour to Hour Fraction Table
                                                        InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iConvTimeIn1Min, iCompTimeIn1Min, NDFRACTION, HourType.NDOTHour);
                                                        #endregion

                                                        iCompRegNightPremMin += GetOTHoursInMinutes(iOTTemp, iOTTemp2, iCompTimeIn1Min, iCompTimeOut1Min);
                                                        #region Insert ND Hour to Hour Fraction Table
                                                        InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iCompTimeIn1Min, iCompTimeOut1Min, NDFRACTION, HourType.NDHour);
                                                        #endregion

                                                        if (Convert.ToBoolean(NDCNTBREAK))
                                                        {
                                                            iCompRegNightPremMin += GetOTHoursInMinutes(iOTTemp, iOTTemp2, iCompTimeOut1Min, iCompTimeIn2Min);
                                                            #region Insert ND Hour to Hour Fraction Table
                                                            InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iCompTimeOut1Min, iCompTimeIn2Min, NDFRACTION, HourType.NDHour);
                                                            #endregion
                                                        }

                                                        iCompRegNightPremMin += GetOTHoursInMinutes(iOTTemp, iOTTemp2, iCompTimeIn2Min, iCompTimeOut2Min);
                                                        #region Insert ND Hour to Hour Fraction Table
                                                        InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iCompTimeIn2Min, iCompTimeOut2Min, NDFRACTION, HourType.NDHour);
                                                        #endregion

                                                        iCompOvertimeNightPremMin += GetOTHoursInMinutes(iOTTemp, iOTTemp2, iCompTimeOut2Min, iConvTimeOut2Min);
                                                        #region Insert ND Hour to Hour Fraction Table
                                                        InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iCompTimeOut2Min, iConvTimeOut2Min, NDFRACTION, HourType.NDOTHour);
                                                        #endregion
                                                    }
                                                    else
                                                    {
                                                        iOTTemp = (NIGHTDIFFGRAVEYARDSTART > iOTStartMin) ? NIGHTDIFFGRAVEYARDSTART : iOTStartMin;
                                                        iOTTemp2 = (NIGHTDIFFGRAVEYARDEND < iOTEndMin) ? NIGHTDIFFGRAVEYARDEND : iOTEndMin;

                                                        iCompOvertimeNightPremMin += GetOTHoursInMinutes(iOTTemp, iOTTemp2, iConvTimeIn1Min, iCompTimeOut1Min);
                                                        #region Insert ND Hour to Hour Fraction Table
                                                        InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iConvTimeIn1Min, iCompTimeOut1Min, NDFRACTION, HourType.NDOTHour);
                                                        #endregion

                                                        iCompOvertimeNightPremMin += GetOTHoursInMinutes(iOTTemp, iOTTemp2, iCompTimeIn2Min, iConvTimeOut2Min);
                                                        #region Insert ND Hour to Hour Fraction Table
                                                        InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iCompTimeIn2Min, iConvTimeOut2Min, NDFRACTION, HourType.NDOTHour);
                                                        #endregion

                                                        iTimeMinTemp = GetOTHoursInMinutes(iOTTemp, iOTTemp2, iBreakMin, iCompTimeIn2Min);
                                                        //if (iTimeMinTemp > 0 && iTimeMinTemp <= iMasterPaidBreak)
                                                        if (iTimeMinTemp > 0 && Convert.ToBoolean(NDCNTBREAK))
                                                        {
                                                            iCompOvertimeNightPremMin += iTimeMinTemp;
                                                            #region Insert ND Hour to Hour Fraction Table
                                                            InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iBreakMin, iCompTimeIn2Min, NDFRACTION, HourType.NDOTHour);
                                                            #endregion
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    iTimeMinTemp = (iConvTimeIn1Min > 0) ? iConvTimeIn1Min : iConvTimeIn2Min;
                                                    iTimeMinTemp2 = (iConvTimeOut2Min > 0) ? iConvTimeOut2Min : iConvTimeOut1Min;
                                                    if (iTimeMinTemp > 0 && iTimeMinTemp2 > 0)
                                                    {
                                                        ///OVERTIME NIGHT PREMIUM FOR DAY SHIFTS
                                                        if (!Convert.ToBoolean(NONDOTDAY)) //HOYA
                                                        {
                                                            if (Convert.ToBoolean(NDSPLTSHFT)) //HOEP: Special Case
                                                            {
                                                                iOTTemp = (NIGHTDIFFAMSTART > iOTStartMin) ? NIGHTDIFFAMSTART : iOTStartMin;
                                                                iOTTemp2 = (NIGHTDIFFAMEND < iOTEndMin) ? NIGHTDIFFAMEND : iOTEndMin;

                                                                iCompOvertimeNightPremMin += GetOTHoursInMinutes(iOTTemp, iOTTemp2, iConvTimeIn1Min, iCompTimeIn1Min);
                                                                #region Insert ND Hour to Hour Fraction Table
                                                                InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iConvTimeIn1Min, iCompTimeIn1Min, NDFRACTION, HourType.NDOTHour);
                                                                #endregion

                                                                iCompRegNightPremMin += GetOTHoursInMinutes(iOTTemp, iOTTemp2, iCompTimeIn1Min, iCompTimeOut1Min);
                                                                #region Insert ND Hour to Hour Fraction Table
                                                                InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iCompTimeIn1Min, iCompTimeOut1Min, NDFRACTION, HourType.NDHour);
                                                                #endregion

                                                                if (Convert.ToBoolean(NDCNTBREAK))
                                                                {
                                                                    iCompRegNightPremMin += GetOTHoursInMinutes(iOTTemp, iOTTemp2, iCompTimeOut1Min, iCompTimeIn2Min);
                                                                    #region Insert ND Hour to Hour Fraction Table
                                                                    InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iCompTimeOut1Min, iCompTimeIn2Min, NDFRACTION, HourType.NDHour);
                                                                    #endregion
                                                                }

                                                                iCompRegNightPremMin += GetOTHoursInMinutes(iOTTemp, iOTTemp2, iCompTimeIn2Min, iCompTimeOut2Min);
                                                                #region Insert ND Hour to Hour Fraction Table
                                                                InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iCompTimeIn2Min, iCompTimeOut2Min, NDFRACTION, HourType.NDHour);
                                                                #endregion

                                                                iCompOvertimeNightPremMin += GetOTHoursInMinutes(iOTTemp, iOTTemp2, iCompTimeOut2Min, iConvTimeOut2Min);
                                                                #region Insert ND Hour to Hour Fraction Table
                                                                InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iCompTimeOut2Min, iConvTimeOut2Min, NDFRACTION, HourType.NDOTHour);
                                                                #endregion
                                                            }
                                                            else
                                                            {
                                                                iOTTemp = (NIGHTDIFFAMSTART > iOTStartMin) ? NIGHTDIFFAMSTART : iOTStartMin;
                                                                iOTTemp2 = (NIGHTDIFFAMEND < iOTEndMin) ? NIGHTDIFFAMEND : iOTEndMin;
                                                                iCompOvertimeNightPremMin += GetOTHoursInMinutes(iOTTemp, iOTTemp2, iTimeMinTemp, iTimeMinTemp2);
                                                                #region Insert ND Hour to Hour Fraction Table
                                                                InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iTimeMinTemp, iTimeMinTemp2, NDFRACTION, HourType.NDOTHour);
                                                                #endregion
                                                            }
                                                        }

                                                        ///OVERTIME NIGHT PREMIUM FOR GRAVEYARD SHIFTS
                                                        if (Convert.ToBoolean(NDSPLTSHFT)) //HOEP: Special Case
                                                        {
                                                            iOTTemp = (NIGHTDIFFGRAVEYARDSTART > iOTStartMin) ? NIGHTDIFFGRAVEYARDSTART : iOTStartMin;
                                                            iOTTemp2 = (NIGHTDIFFGRAVEYARDEND < iOTEndMin) ? NIGHTDIFFGRAVEYARDEND : iOTEndMin;

                                                            iCompOvertimeNightPremMin += GetOTHoursInMinutes(iOTTemp, iOTTemp2, iConvTimeIn1Min, iCompTimeIn1Min);
                                                            #region Insert ND Hour to Hour Fraction Table
                                                            InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iConvTimeIn1Min, iCompTimeIn1Min, NDFRACTION, HourType.NDOTHour);
                                                            #endregion

                                                            iCompRegNightPremMin += GetOTHoursInMinutes(iOTTemp, iOTTemp2, iCompTimeIn1Min, iCompTimeOut1Min);
                                                            #region Insert ND Hour to Hour Fraction Table
                                                            InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iCompTimeIn1Min, iCompTimeOut1Min, NDFRACTION, HourType.NDHour);
                                                            #endregion

                                                            if (Convert.ToBoolean(NDCNTBREAK))
                                                            {
                                                                iCompRegNightPremMin += GetOTHoursInMinutes(iOTTemp, iOTTemp2, iCompTimeOut1Min, iCompTimeIn2Min);
                                                                #region Insert ND Hour to Hour Fraction Table
                                                                InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iCompTimeOut1Min, iCompTimeIn2Min, NDFRACTION, HourType.NDHour);
                                                                #endregion
                                                            }

                                                            iCompRegNightPremMin += GetOTHoursInMinutes(iOTTemp, iOTTemp2, iCompTimeIn2Min, iCompTimeOut2Min);
                                                            #region Insert ND Hour to Hour Fraction Table
                                                            InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iCompTimeIn2Min, iCompTimeOut2Min, NDFRACTION, HourType.NDHour);
                                                            #endregion

                                                            iCompOvertimeNightPremMin += GetOTHoursInMinutes(iOTTemp, iOTTemp2, iCompTimeOut2Min, iConvTimeOut2Min);
                                                            #region Insert ND Hour to Hour Fraction Table
                                                            InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iCompTimeOut2Min, iConvTimeOut2Min, NDFRACTION, HourType.NDOTHour);
                                                            #endregion
                                                        }
                                                        else
                                                        {
                                                            iOTTemp = (NIGHTDIFFGRAVEYARDSTART > iOTStartMin) ? NIGHTDIFFGRAVEYARDSTART : iOTStartMin;
                                                            iOTTemp2 = (NIGHTDIFFGRAVEYARDEND < iOTEndMin) ? NIGHTDIFFGRAVEYARDEND : iOTEndMin;
                                                            iCompOvertimeNightPremMin += GetOTHoursInMinutes(iOTTemp, iOTTemp2, iTimeMinTemp, iTimeMinTemp2);
                                                            #region Insert ND Hour to Hour Fraction Table
                                                            InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iTimeMinTemp, iTimeMinTemp2, NDFRACTION, HourType.NDOTHour);
                                                            #endregion
                                                        }
                                                    }
                                                }
                                                #endregion
                                            }
                                        }
                                        else //Regular day
                                        {
                                            #region Computed Overtime Minutes
                                            //[Adv OT = Get OT between ConvertedIn1 and ComputedIn1]
                                            if (strOTType.Equals("A")) //Advance OT Type
                                            {
                                                iOTTemp = (iCompTimeIn1Min < iCompTimeOut1Min) ? iCompTimeIn1Min : iCompTimeOut1Min;
                                                iAdvOTMin = GetOTHoursInMinutes(iOTStartMin, iOTEndMin, iConvTimeIn1Min, iOTTemp);
                                                if (bCountOTFraction == true)
                                                {
                                                    iAdvOTMin = Convert.ToInt32(iAdvOTMin / iOTFraction) * iOTFraction;
                                                }
                                                iAdjShiftMin += iAdvOTMin;
                                                iComputedOvertimeMin += iAdvOTMin;

                                                #region Insert Overtime to Hour Fraction Table
                                                InsertOTToHourFractionTable(iOTStartMin, iOTEndMin, iConvTimeIn1Min, iCompTimeIn1Min, iOTFraction, bCountOTFraction);
                                                #endregion
                                            }

                                            //[Mid OT = Get OT between ComputedOut1 and ComputedIn2]
                                            if (MIDOT == true && strOTType.Equals("M"))
                                            {
                                                iAdvOTMin = GetOTHoursInMinutes(iOTStartMin, iOTEndMin
                                                                                , Math.Max(Math.Max(iCompTimeOut1Min, iConvTimeIn2Min), iConvTimeIn1Min)
                                                                                , Math.Min(Math.Max(iCompTimeIn2Min, iConvTimeOut1Min), iConvTimeOut2Min));
                                                if (bCountOTFraction == true)
                                                {
                                                    iAdvOTMin = Convert.ToInt32(iAdvOTMin / iOTFraction) * iOTFraction;
                                                }
                                                iComputedOvertimeMin += iAdvOTMin;

                                                #region Insert Overtime to Hour Fraction Table
                                                InsertOTToHourFractionTable(iOTStartMin, iOTEndMin
                                                                            , Math.Max(Math.Max(iCompTimeOut1Min, iConvTimeIn2Min), iConvTimeIn1Min)
                                                                            , Math.Max(Math.Max(iCompTimeIn2Min, iConvTimeOut1Min), iConvTimeOut2Min)
                                                                            , iOTFraction, bCountOTFraction);
                                                #endregion
                                            }

                                            //[Post OT = Get OT between ComputedOut2 and ConvertedOut2]
                                            iOTTemp = (iCompTimeIn2Min > iCompTimeOut2Min) ? iCompTimeIn2Min : iCompTimeOut2Min;
                                            iAdvOTMin = GetOTHoursInMinutes(iOTStartMin, iOTEndMin, iOTTemp, iConvTimeOut2Min);
                                            if (bCountOTFraction == true)
                                            {
                                                iAdvOTMin = Convert.ToInt32(iAdvOTMin / iOTFraction) * iOTFraction;
                                            }
                                            iComputedOvertimeMin += iAdvOTMin;

                                            #region Insert Overtime to Hour Fraction Table
                                            InsertOTToHourFractionTable(iOTStartMin, iOTEndMin, iCompTimeOut2Min, iConvTimeOut2Min, iOTFraction, bCountOTFraction);
                                            #endregion

                                            if (bCountOTFraction == false)
                                            {
                                                iComputedOvertimeMin = Convert.ToInt32(iComputedOvertimeMin / iOTFraction) * iOTFraction;
                                                CorrectOTHourFraction(iComputedOvertimeMin);
                                            }
                                            #endregion

                                            if (NDBRCKTCNT == 2)
                                            {
                                                #region Computed Overtime Night Premium (Sharp)
                                                if (bIsGraveyard) //Graveyard shift
                                                {
                                                    #region ND Bracket 1
                                                    //[NDOTHr = Get NDOT between NP1_BEGTIME/ConvertedIn1 to ComputedIn1]
                                                    iTimeMinTemp = (NP1_BEGTIME > iConvTimeIn1Min) ? NP1_BEGTIME : iConvTimeIn1Min;
                                                    iCompRegNightPremMin += GetOTHoursInMinutes(iOTStartMin, iOTEndMin, iTimeMinTemp, iCompTimeIn1Min);
                                                    #region Insert ND Hour to Hour Fraction Table
                                                    InsertNDToHourFractionTable(iOTStartMin, iOTEndMin, iTimeMinTemp, iCompTimeIn1Min, NDFRACTION, HourType.NDHour);
                                                    #endregion

                                                    //[NDOTHr = Get NDOT between ComputedOut2 to NP1_ENDTIME/ConvertedOut2]
                                                    iTimeMinTemp = (iConvTimeOut2Min < NP1_ENDTIME) ? iConvTimeOut2Min : NP1_ENDTIME;
                                                    iCompRegNightPremMin += GetOTHoursInMinutes(iOTStartMin, iOTEndMin, iCompTimeOut2Min, iTimeMinTemp);
                                                    #region Insert ND Hour to Hour Fraction Table
                                                    InsertNDToHourFractionTable(iOTStartMin, iOTEndMin, iCompTimeOut2Min, iTimeMinTemp, NDFRACTION, HourType.NDHour);
                                                    #endregion
                                                    #endregion

                                                    #region ND Bracket 2
                                                    //[NDOTHr = Get NDOT between NP2_BEGTIME/ConvertedIn1 to ComputedIn1]
                                                    iTimeMinTemp = (NP2_BEGTIME > iConvTimeIn1Min) ? NP2_BEGTIME : iConvTimeIn1Min;
                                                    iCompOvertimeNightPremMin += GetOTHoursInMinutes(iOTStartMin, iOTEndMin, iTimeMinTemp, iCompTimeIn1Min);
                                                    #region Insert ND Hour to Hour Fraction Table
                                                    InsertNDToHourFractionTable(iOTStartMin, iOTEndMin, iTimeMinTemp, iCompTimeIn1Min, NDFRACTION, HourType.NDOTHour);
                                                    #endregion

                                                    //[NDOTHr = Get NDOT between ComputedOut2 to NP2_ENDTIME/ConvertedOut2]
                                                    iTimeMinTemp = (iConvTimeOut2Min < NP2_ENDTIME) ? iConvTimeOut2Min : NP2_ENDTIME;
                                                    iCompOvertimeNightPremMin += GetOTHoursInMinutes(iOTStartMin, iOTEndMin, iCompTimeOut2Min, iTimeMinTemp);
                                                    #region Insert ND Hour to Hour Fraction Table
                                                    InsertNDToHourFractionTable(iOTStartMin, iOTEndMin, iCompTimeOut2Min, iTimeMinTemp, NDFRACTION, HourType.NDOTHour);
                                                    #endregion
                                                    #endregion
                                                }
                                                else
                                                {
                                                    #region ND Bracket 1
                                                    //[NDOTHr = Get NDOT between NP1_BEGTIME/ComputedOut2 to ConvertedOut2]
                                                    iTimeMinTemp = (iCompTimeOut2Min > NP1_BEGTIME) ? iCompTimeOut2Min : NP1_BEGTIME;
                                                    iTimeMinTemp2 = (iOTEndMin > NP1_ENDTIME) ? NP1_ENDTIME : iOTEndMin;
                                                    iCompRegNightPremMin += GetOTHoursInMinutes(iOTStartMin, iTimeMinTemp2, iTimeMinTemp, iConvTimeOut2Min);
                                                    #region Insert ND Hour to Hour Fraction Table
                                                    InsertNDToHourFractionTable(iOTStartMin, iTimeMinTemp2, iTimeMinTemp, iConvTimeOut2Min, NDFRACTION, HourType.NDHour);
                                                    #endregion
                                                    #endregion

                                                    #region ND Bracket 1 (Early ND)
                                                    if (NP1_ENDTIME < NP1_BEGTIME
                                                        || NP1_BEGTIME > GRAVEYARD24
                                                        || NP1_ENDTIME > GRAVEYARD24)
                                                    {
                                                        if (NP1_BEGTIME > GRAVEYARD24)
                                                            iTimeMinTemp3 = NP1_BEGTIME - GRAVEYARD24;
                                                        else
                                                            iTimeMinTemp3 = NP1_BEGTIME;

                                                        if (NP1_ENDTIME > GRAVEYARD24)
                                                            iTimeMinTemp4 = NP1_ENDTIME - GRAVEYARD24;
                                                        else
                                                            iTimeMinTemp4 = NP1_ENDTIME;

                                                        if (iTimeMinTemp4 < iTimeMinTemp3)
                                                            iTimeMinTemp3 = 0;

                                                        iOTTemp = (iTimeMinTemp3 > iOTStartMin) ? iTimeMinTemp3 : iOTStartMin;
                                                        iOTTemp2 = (iTimeMinTemp4 < iOTEndMin) ? iTimeMinTemp4 : iOTEndMin;
                                                        iTimeMinTemp = (iTimeMinTemp2 < iCompTimeIn1Min) ? iTimeMinTemp2 : iCompTimeIn1Min;
                                                        iCompRegNightPremMin += GetOTHoursInMinutes(iOTTemp, iOTTemp2, iConvTimeIn1Min, iTimeMinTemp);
                                                        #region Insert ND Hour to Hour Fraction Table
                                                        InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iConvTimeIn1Min, iTimeMinTemp, NDFRACTION, HourType.NDHour);
                                                        #endregion
                                                    }
                                                    #endregion

                                                    #region ND Bracket 2
                                                    //[NDOTHr = Get NDOT between NP2_BEGTIME/ComputedOut2 to ConvertedOut2]
                                                    iTimeMinTemp = (iCompTimeOut2Min > NP2_BEGTIME) ? iCompTimeOut2Min : NP2_BEGTIME;
                                                    iTimeMinTemp2 = (iOTEndMin > NP2_ENDTIME) ? NP2_ENDTIME : iOTEndMin;
                                                    iCompOvertimeNightPremMin += GetOTHoursInMinutes(iOTStartMin, iTimeMinTemp2, iTimeMinTemp, iConvTimeOut2Min);
                                                    #region Insert ND Hour to Hour Fraction Table
                                                    InsertNDToHourFractionTable(iOTStartMin, iTimeMinTemp2, iTimeMinTemp, iConvTimeOut2Min, NDFRACTION, HourType.NDOTHour);
                                                    #endregion
                                                    #endregion

                                                    #region ND Bracket 2 (Early ND)
                                                    if (NP2_ENDTIME < NP2_BEGTIME
                                                        || NP2_BEGTIME > GRAVEYARD24
                                                        || NP2_ENDTIME > GRAVEYARD24)
                                                    {
                                                        if (NP2_BEGTIME > GRAVEYARD24)
                                                            iTimeMinTemp3 = NP2_BEGTIME - GRAVEYARD24;
                                                        else
                                                            iTimeMinTemp3 = NP2_BEGTIME;

                                                        if (NP2_ENDTIME > GRAVEYARD24)
                                                            iTimeMinTemp4 = NP2_ENDTIME - GRAVEYARD24;
                                                        else
                                                            iTimeMinTemp4 = NP2_ENDTIME;

                                                        if (iTimeMinTemp4 < iTimeMinTemp3)
                                                            iTimeMinTemp3 = 0;

                                                        iOTTemp = (iTimeMinTemp3 > iOTStartMin) ? iTimeMinTemp3 : iOTStartMin;
                                                        iOTTemp2 = (iTimeMinTemp4 < iOTEndMin) ? iTimeMinTemp4 : iOTEndMin;
                                                        iTimeMinTemp = (iTimeMinTemp2 < iCompTimeIn1Min) ? iTimeMinTemp2 : iCompTimeIn1Min;
                                                        iCompOvertimeNightPremMin += GetOTHoursInMinutes(iOTTemp, iOTTemp2, iConvTimeIn1Min, iTimeMinTemp);
                                                        #region Insert ND Hour to Hour Fraction Table
                                                        InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iConvTimeIn1Min, iTimeMinTemp, NDFRACTION, HourType.NDOTHour);
                                                        #endregion
                                                    }
                                                    #endregion
                                                }
                                                #endregion
                                            }
                                            else
                                            {
                                                #region Computed Overtime Night Premium (Normal)
                                                if (bIsGraveyard) //Graveyard shift
                                                {
                                                    //[NDOTHr = Get NDOT between 22:00/ConvertedIn1 to ComputedIn1]
                                                    iTimeMinTemp = (NIGHTDIFFGRAVEYARDSTART > iConvTimeIn1Min) ? NIGHTDIFFGRAVEYARDSTART : iConvTimeIn1Min;
                                                    iCompOvertimeNightPremMin += GetOTHoursInMinutes(iOTStartMin, iOTEndMin, iTimeMinTemp, iCompTimeIn1Min);
                                                    #region Insert ND Hour to Hour Fraction Table
                                                    InsertNDToHourFractionTable(iOTStartMin, iOTEndMin, iTimeMinTemp, iCompTimeIn1Min, NDFRACTION, HourType.NDOTHour);
                                                    #endregion

                                                    //[NDOTHr = Get NDOT between ComputedOut2 to 30:00/ConvertedOut2]
                                                    iTimeMinTemp = (iConvTimeOut2Min < NIGHTDIFFGRAVEYARDEND) ? iConvTimeOut2Min : NIGHTDIFFGRAVEYARDEND;
                                                    iCompOvertimeNightPremMin += GetOTHoursInMinutes(iOTStartMin, iOTEndMin, iCompTimeOut2Min, iTimeMinTemp);
                                                    #region Insert ND Hour to Hour Fraction Table
                                                    InsertNDToHourFractionTable(iOTStartMin, iOTEndMin, iCompTimeOut2Min, iTimeMinTemp, NDFRACTION, HourType.NDOTHour);
                                                    #endregion
                                                }
                                                else
                                                {
                                                    if (!Convert.ToBoolean(NONDOTDAY)) //HOYA
                                                    {
                                                        //[NDOTHr = Get NDOT between ConvertedIn1 to 06:00/ComputedIn1]
                                                        iTimeMinTemp = (NIGHTDIFFAMEND < iCompTimeIn1Min) ? NIGHTDIFFAMEND : iCompTimeIn1Min;
                                                        iCompOvertimeNightPremMin += GetOTHoursInMinutes(iOTStartMin, iOTEndMin, iConvTimeIn1Min, iTimeMinTemp);
                                                        #region Insert ND Hour to Hour Fraction Table
                                                        InsertNDToHourFractionTable(iOTStartMin, iOTEndMin, iConvTimeIn1Min, iTimeMinTemp, NDFRACTION, HourType.NDOTHour);
                                                        #endregion
                                                    }
                                                    //[NDOTHr = Get NDOT between 22:00/ComputedOut2 to ConvertedOut2]
                                                    iTimeMinTemp = (iCompTimeOut2Min > NIGHTDIFFGRAVEYARDSTART) ? iCompTimeOut2Min : NIGHTDIFFGRAVEYARDSTART;
                                                    iTimeMinTemp2 = (iOTEndMin > NIGHTDIFFGRAVEYARDEND) ? NIGHTDIFFGRAVEYARDEND : iOTEndMin;
                                                    iCompOvertimeNightPremMin += GetOTHoursInMinutes(iOTStartMin, iTimeMinTemp2, iTimeMinTemp, iConvTimeOut2Min);
                                                    #region Insert ND Hour to Hour Fraction Table
                                                    InsertNDToHourFractionTable(iOTStartMin, iTimeMinTemp2, iTimeMinTemp, iConvTimeOut2Min, NDFRACTION, HourType.NDOTHour);
                                                    #endregion
                                                }
                                                #endregion
                                            }
                                        }
                                    }
                                    #endregion

                                    if (NDBRCKTCNT == 2)
                                    {
                                        #region Computed Regular Night Premium for Regular Day (Sharp)
                                        if (!bIsRestDay && !bIsHoliday && bIsRegOrReg5DayCode //Regular Day
                                            && (dtEmployeeLogLedger.Rows[i]["Ell_AssumedPostBack"] == DBNull.Value //Assumed Postback flag is NULL or not equal to T
                                                    || dtEmployeeLogLedger.Rows[i]["Ell_AssumedPostBack"].ToString().Equals("T") == false))
                                        {
                                            //COMPUTED REGULAR NIGHT PREMIUM MIN
                                            if ((iConvTimeIn1Min > 0 && iConvTimeOut2Min > 0 && iConvTimeIn1Min <= iShiftTimeOut1Min && iConvTimeOut2Min >= iShiftTimeIn2Min)
                                                //|| (iConvTimeIn1Min > 0 && iConvTimeOut1Min > 0 && iConvTimeOut1Min >= iShiftTimeIn2Min)
                                                //|| (iConvTimeIn2Min > 0 && iConvTimeOut2Min > 0 && iConvTimeIn2Min <= iShiftTimeIn2Min)
                                                    ) //Condition to meet paid break
                                            {
                                                #region ND Bracket 1
                                                iCompRegNightPremMin += GetOTHoursInMinutes(NP1_BEGTIME, NP1_ENDTIME, iCompTimeIn1Min, iCompTimeOut1Min);
                                                #region Insert ND Hour to Hour Fraction Table
                                                InsertNDToHourFractionTable(NP1_BEGTIME, NP1_ENDTIME, iCompTimeIn1Min, iCompTimeOut1Min, NDFRACTION, HourType.NDHour);
                                                #endregion

                                                iCompRegNightPremMin += GetOTHoursInMinutes(NP1_BEGTIME, NP1_ENDTIME, iCompTimeIn2Min, iCompTimeOut2Min);
                                                #region Insert ND Hour to Hour Fraction Table
                                                InsertNDToHourFractionTable(NP1_BEGTIME, NP1_ENDTIME, iCompTimeIn2Min, iCompTimeOut2Min, NDFRACTION, HourType.NDHour);
                                                #endregion

                                                //Break between shifts
                                                iPaidBreak = GetOTHoursInMinutes(iShiftTimeOut1Min, iShiftTimeIn2Min, iCompTimeIn1Min, iCompTimeOut2Min);
                                                if (iMasterPaidBreak > 0 && iPaidBreak <= iMasterPaidBreak) //Must not exceed the set paid break
                                                {
                                                    iTimeMinTemp = (iCompTimeIn1Min > iCompTimeOut1Min) ? iCompTimeIn1Min : iCompTimeOut1Min;
                                                    iCompRegNightPremMin += GetOTHoursInMinutes(NP1_BEGTIME, NP1_ENDTIME, iTimeMinTemp, iCompTimeIn2Min);
                                                    #region Insert ND Hour to Hour Fraction Table
                                                    InsertNDToHourFractionTable(NP1_BEGTIME, NP1_ENDTIME, iTimeMinTemp, iCompTimeIn2Min, NDFRACTION, HourType.NDHour);
                                                    #endregion
                                                }
                                                #endregion

                                                #region ND Bracket 1 (Early ND)
                                                if (NP1_ENDTIME < NP1_BEGTIME
                                                    || NP1_BEGTIME > GRAVEYARD24
                                                    || NP1_ENDTIME > GRAVEYARD24)
                                                {
                                                    if (NP1_BEGTIME > GRAVEYARD24)
                                                        iTimeMinTemp3 = NP1_BEGTIME - GRAVEYARD24;
                                                    else
                                                        iTimeMinTemp3 = NP1_BEGTIME;

                                                    if (NP1_ENDTIME > GRAVEYARD24)
                                                        iTimeMinTemp4 = NP1_ENDTIME - GRAVEYARD24;
                                                    else
                                                        iTimeMinTemp4 = NP1_ENDTIME;

                                                    if (iTimeMinTemp4 < iTimeMinTemp3)
                                                        iTimeMinTemp3 = 0;

                                                    iCompRegNightPremMin += GetOTHoursInMinutes(iTimeMinTemp3, iTimeMinTemp4, iCompTimeIn1Min, iCompTimeOut1Min);
                                                    #region Insert ND Hour to Hour Fraction Table
                                                    InsertNDToHourFractionTable(iTimeMinTemp3, iTimeMinTemp4, iCompTimeIn1Min, iCompTimeOut1Min, NDFRACTION, HourType.NDHour);
                                                    #endregion

                                                    iCompRegNightPremMin += GetOTHoursInMinutes(iTimeMinTemp3, iTimeMinTemp4, iCompTimeIn2Min, iCompTimeOut2Min);
                                                    #region Insert ND Hour to Hour Fraction Table
                                                    InsertNDToHourFractionTable(iTimeMinTemp3, iTimeMinTemp4, iCompTimeIn2Min, iCompTimeOut2Min, NDFRACTION, HourType.NDHour);
                                                    #endregion
                                                }
                                                #endregion

                                                #region ND Bracket 2
                                                iCompOvertimeNightPremMin += GetOTHoursInMinutes(NP2_BEGTIME, NP2_ENDTIME, iCompTimeIn1Min, iCompTimeOut1Min);
                                                #region Insert ND Hour to Hour Fraction Table
                                                InsertNDToHourFractionTable(NP2_BEGTIME, NP2_ENDTIME, iCompTimeIn1Min, iCompTimeOut1Min, NDFRACTION, HourType.NDOTHour);
                                                #endregion

                                                iCompOvertimeNightPremMin += GetOTHoursInMinutes(NP2_BEGTIME, NP2_ENDTIME, iCompTimeIn2Min, iCompTimeOut2Min);
                                                #region Insert ND Hour to Hour Fraction Table
                                                InsertNDToHourFractionTable(NP2_BEGTIME, NP2_ENDTIME, iCompTimeIn2Min, iCompTimeOut2Min, NDFRACTION, HourType.NDOTHour);
                                                #endregion

                                                //Break between shifts
                                                iPaidBreak = GetOTHoursInMinutes(iShiftTimeOut1Min, iShiftTimeIn2Min, iCompTimeIn1Min, iCompTimeOut2Min);
                                                if (iMasterPaidBreak > 0 && iPaidBreak <= iMasterPaidBreak) //Must not exceed the set paid break
                                                {
                                                    iTimeMinTemp = (iCompTimeIn1Min > iCompTimeOut1Min) ? iCompTimeIn1Min : iCompTimeOut1Min;
                                                    iCompOvertimeNightPremMin += GetOTHoursInMinutes(NP2_BEGTIME, NP2_ENDTIME, iTimeMinTemp, iCompTimeIn2Min);
                                                    #region Insert ND Hour to Hour Fraction Table
                                                    InsertNDToHourFractionTable(NP2_BEGTIME, NP2_ENDTIME, iTimeMinTemp, iCompTimeIn2Min, NDFRACTION, HourType.NDOTHour);
                                                    #endregion
                                                }
                                                #endregion

                                                #region ND Bracket 2 (Early ND)
                                                if (NP2_ENDTIME < NP2_BEGTIME
                                                    || NP2_BEGTIME > GRAVEYARD24
                                                    || NP2_ENDTIME > GRAVEYARD24)
                                                {
                                                    if (NP2_BEGTIME > GRAVEYARD24)
                                                        iTimeMinTemp3 = NP2_BEGTIME - GRAVEYARD24;
                                                    else
                                                        iTimeMinTemp3 = NP2_BEGTIME;

                                                    if (NP2_ENDTIME > GRAVEYARD24)
                                                        iTimeMinTemp4 = NP2_ENDTIME - GRAVEYARD24;
                                                    else
                                                        iTimeMinTemp4 = NP2_ENDTIME;

                                                    if (iTimeMinTemp4 < iTimeMinTemp3)
                                                        iTimeMinTemp3 = 0;

                                                    iCompOvertimeNightPremMin += GetOTHoursInMinutes(iTimeMinTemp3, iTimeMinTemp4, iCompTimeIn1Min, iCompTimeOut1Min);
                                                    #region Insert ND Hour to Hour Fraction Table
                                                    InsertNDToHourFractionTable(iTimeMinTemp3, iTimeMinTemp4, iCompTimeIn1Min, iCompTimeOut1Min, NDFRACTION, HourType.NDOTHour);
                                                    #endregion

                                                    iCompOvertimeNightPremMin += GetOTHoursInMinutes(iTimeMinTemp3, iTimeMinTemp4, iCompTimeIn2Min, iCompTimeOut2Min);
                                                    #region Insert ND Hour to Hour Fraction Table
                                                    InsertNDToHourFractionTable(iTimeMinTemp3, iTimeMinTemp4, iCompTimeIn2Min, iCompTimeOut2Min, NDFRACTION, HourType.NDOTHour);
                                                    #endregion
                                                }
                                                #endregion
                                            }
                                            else
                                            {
                                                iTimeMinTemp = (iCompTimeIn1Min > 0) ? iCompTimeIn1Min : iCompTimeIn2Min;
                                                iTimeMinTemp2 = (iCompTimeOut2Min > 0) ? iCompTimeOut2Min : iCompTimeOut1Min;
                                                if (iTimeMinTemp > 0 && iTimeMinTemp2 > 0)
                                                {
                                                    #region ND Bracket 1
                                                    iCompRegNightPremMin += GetOTHoursInMinutes(NP1_BEGTIME, NP1_ENDTIME, iTimeMinTemp, iTimeMinTemp2);
                                                    #region Insert ND Hour to Hour Fraction Table
                                                    InsertNDToHourFractionTable(NP1_BEGTIME, NP1_ENDTIME, iTimeMinTemp, iTimeMinTemp2, NDFRACTION, HourType.NDHour);
                                                    #endregion
                                                    #endregion

                                                    #region ND Bracket 1 (Early ND)
                                                    if (NP1_ENDTIME < NP1_BEGTIME
                                                        || NP1_BEGTIME > GRAVEYARD24
                                                        || NP1_ENDTIME > GRAVEYARD24)
                                                    {
                                                        if (NP1_BEGTIME > GRAVEYARD24)
                                                            iTimeMinTemp3 = NP1_BEGTIME - GRAVEYARD24;
                                                        else
                                                            iTimeMinTemp3 = NP1_BEGTIME;

                                                        if (NP1_ENDTIME > GRAVEYARD24)
                                                            iTimeMinTemp4 = NP1_ENDTIME - GRAVEYARD24;
                                                        else
                                                            iTimeMinTemp4 = NP1_ENDTIME;

                                                        if (iTimeMinTemp4 < iTimeMinTemp3)
                                                            iTimeMinTemp3 = 0;

                                                        iCompRegNightPremMin += GetOTHoursInMinutes(iTimeMinTemp3, iTimeMinTemp4, iTimeMinTemp, iTimeMinTemp2);
                                                        #region Insert ND Hour to Hour Fraction Table
                                                        InsertNDToHourFractionTable(iTimeMinTemp3, iTimeMinTemp4, iTimeMinTemp, iTimeMinTemp2, NDFRACTION, HourType.NDHour);
                                                        #endregion
                                                    }
                                                    #endregion

                                                    #region ND Bracket 2
                                                    iCompOvertimeNightPremMin += GetOTHoursInMinutes(NP2_BEGTIME, NP2_ENDTIME, iTimeMinTemp, iTimeMinTemp2);
                                                    #region Insert ND Hour to Hour Fraction Table
                                                    InsertNDToHourFractionTable(NP2_BEGTIME, NP2_ENDTIME, iTimeMinTemp, iTimeMinTemp2, NDFRACTION, HourType.NDOTHour);
                                                    #endregion
                                                    #endregion

                                                    #region ND Bracket 2 (Early ND)
                                                    if (NP2_ENDTIME < NP2_BEGTIME
                                                        || NP2_BEGTIME > GRAVEYARD24
                                                        || NP2_ENDTIME > GRAVEYARD24)
                                                    {
                                                        if (NP2_BEGTIME > GRAVEYARD24)
                                                            iTimeMinTemp3 = NP2_BEGTIME - GRAVEYARD24;
                                                        else
                                                            iTimeMinTemp3 = NP2_BEGTIME;

                                                        if (NP2_ENDTIME > GRAVEYARD24)
                                                            iTimeMinTemp4 = NP2_ENDTIME - GRAVEYARD24;
                                                        else
                                                            iTimeMinTemp4 = NP2_ENDTIME;

                                                        if (iTimeMinTemp4 < iTimeMinTemp3)
                                                            iTimeMinTemp3 = 0;

                                                        iCompOvertimeNightPremMin += GetOTHoursInMinutes(iTimeMinTemp3, iTimeMinTemp4, iTimeMinTemp, iTimeMinTemp2);
                                                        #region Insert ND Hour to Hour Fraction Table
                                                        InsertNDToHourFractionTable(iTimeMinTemp3, iTimeMinTemp4, iTimeMinTemp, iTimeMinTemp2, NDFRACTION, HourType.NDOTHour);
                                                        #endregion
                                                    }
                                                    #endregion
                                                }
                                            }
                                        }
                                        #endregion

                                        #region NDFraction Filter (Sharp)
                                        iCompRegNightPremMin = Convert.ToInt32((iCompRegNightPremMin / iNDFraction)) * iNDFraction;
                                        iCompOvertimeNightPremMin = Convert.ToInt32((iCompOvertimeNightPremMin / iNDFraction)) * iNDFraction;
                                        #endregion
                                    }
                                    else
                                    {
                                        #region Computed Regular Night Premium for Regular Day (Normal)
                                        if (!bIsRestDay && !bIsHoliday && bIsRegOrReg5DayCode //Regular Day
                                            && (Convert.ToBoolean(NDREGSHIFT) == true || bIsGraveyard == true) //NDREGSHIFT flag is true / graveyard shift
                                            && (dtEmployeeLogLedger.Rows[i]["Ell_AssumedPostBack"] == DBNull.Value //Assumed Postback flag is NULL or not equal to T
                                                    || dtEmployeeLogLedger.Rows[i]["Ell_AssumedPostBack"].ToString().Equals("T") == false))
                                        {
                                            //COMPUTED REGULAR NIGHT PREMIUM MIN
                                            if ((iConvTimeIn1Min > 0 && iConvTimeOut2Min > 0 && iConvTimeIn1Min <= iShiftTimeOut1Min && iConvTimeOut2Min >= iShiftTimeIn2Min)
                                                //|| (iConvTimeIn1Min > 0 && iConvTimeOut1Min > 0 && iConvTimeOut1Min >= iShiftTimeIn2Min)
                                                //|| (iConvTimeIn2Min > 0 && iConvTimeOut2Min > 0 && iConvTimeIn2Min <= iShiftTimeIn2Min)
                                                    ) //Condition to meet paid break
                                            {
                                                if (!Convert.ToBoolean(NONDOTDAY)) //HOYA
                                                {
                                                    iCompRegNightPremMin += GetOTHoursInMinutes(NIGHTDIFFAMSTART, NIGHTDIFFAMEND, iCompTimeIn1Min, iCompTimeOut1Min);
                                                    #region Insert ND Hour to Hour Fraction Table
                                                    InsertNDToHourFractionTable(NIGHTDIFFAMSTART, NIGHTDIFFAMEND, iCompTimeIn1Min, iCompTimeOut1Min, NDFRACTION, HourType.NDHour);
                                                    #endregion

                                                    iCompRegNightPremMin += GetOTHoursInMinutes(NIGHTDIFFAMSTART, NIGHTDIFFAMEND, iCompTimeIn2Min, iCompTimeOut2Min);
                                                    #region Insert ND Hour to Hour Fraction Table
                                                    InsertNDToHourFractionTable(NIGHTDIFFAMSTART, NIGHTDIFFAMEND, iCompTimeIn2Min, iCompTimeOut2Min, NDFRACTION, HourType.NDHour);
                                                    #endregion
                                                }
                                                iCompRegNightPremMin += GetOTHoursInMinutes(NIGHTDIFFGRAVEYARDSTART, NIGHTDIFFGRAVEYARDEND, iCompTimeIn1Min, iCompTimeOut1Min);
                                                #region Insert ND Hour to Hour Fraction Table
                                                InsertNDToHourFractionTable(NIGHTDIFFGRAVEYARDSTART, NIGHTDIFFGRAVEYARDEND, iCompTimeIn1Min, iCompTimeOut1Min, NDFRACTION, HourType.NDHour);
                                                #endregion

                                                iCompRegNightPremMin += GetOTHoursInMinutes(NIGHTDIFFGRAVEYARDSTART, NIGHTDIFFGRAVEYARDEND, iCompTimeIn2Min, iCompTimeOut2Min);
                                                #region Insert ND Hour to Hour Fraction Table
                                                InsertNDToHourFractionTable(NIGHTDIFFGRAVEYARDSTART, NIGHTDIFFGRAVEYARDEND, iCompTimeIn2Min, iCompTimeOut2Min, NDFRACTION, HourType.NDHour);
                                                #endregion

                                                //Break between shifts
                                                iPaidBreak = GetOTHoursInMinutes(iShiftTimeOut1Min, iShiftTimeIn2Min, iCompTimeIn1Min, iCompTimeOut2Min);
                                                if ((iMasterPaidBreak > 0 && iPaidBreak <= iMasterPaidBreak) //Must not exceed the set paid break
                                                    || Convert.ToBoolean(NDCNTBREAK) == true) //NDCNTBREAK for regular days
                                                {
                                                    iTimeMinTemp = (iCompTimeIn1Min > iCompTimeOut1Min) ? iCompTimeIn1Min : iCompTimeOut1Min;
                                                    if (!Convert.ToBoolean(NONDOTDAY)) //HOYA
                                                    {
                                                        iCompRegNightPremMin += GetOTHoursInMinutes(NIGHTDIFFAMSTART, NIGHTDIFFAMEND, iTimeMinTemp, iCompTimeIn2Min);
                                                        #region Insert ND Hour to Hour Fraction Table
                                                        InsertNDToHourFractionTable(NIGHTDIFFAMSTART, NIGHTDIFFAMEND, iTimeMinTemp, iCompTimeIn2Min, NDFRACTION, HourType.NDHour);
                                                        #endregion
                                                    }
                                                    iCompRegNightPremMin += GetOTHoursInMinutes(NIGHTDIFFGRAVEYARDSTART, NIGHTDIFFGRAVEYARDEND, iTimeMinTemp, iCompTimeIn2Min);
                                                    #region Insert ND Hour to Hour Fraction Table
                                                    InsertNDToHourFractionTable(NIGHTDIFFGRAVEYARDSTART, NIGHTDIFFGRAVEYARDEND, iTimeMinTemp, iCompTimeIn2Min, NDFRACTION, HourType.NDHour);
                                                    #endregion
                                                }
                                            }
                                            else
                                            {
                                                iTimeMinTemp = (iCompTimeIn1Min > 0) ? iCompTimeIn1Min : iCompTimeIn2Min;
                                                iTimeMinTemp2 = (iCompTimeOut2Min > 0) ? iCompTimeOut2Min : iCompTimeOut1Min;
                                                if (iTimeMinTemp > 0 && iTimeMinTemp2 > 0)
                                                {
                                                    if (!Convert.ToBoolean(NONDOTDAY)) //HOYA
                                                    {
                                                        iCompRegNightPremMin += GetOTHoursInMinutes(NIGHTDIFFAMSTART, NIGHTDIFFAMEND, iTimeMinTemp, iTimeMinTemp2);
                                                        #region Insert ND Hour to Hour Fraction Table
                                                        InsertNDToHourFractionTable(NIGHTDIFFAMSTART, NIGHTDIFFAMEND, iTimeMinTemp, iTimeMinTemp2, NDFRACTION, HourType.NDHour);
                                                        #endregion
                                                    }

                                                    iCompRegNightPremMin += GetOTHoursInMinutes(NIGHTDIFFGRAVEYARDSTART, NIGHTDIFFGRAVEYARDEND, iTimeMinTemp, iTimeMinTemp2);
                                                    #region Insert ND Hour to Hour Fraction Table
                                                    InsertNDToHourFractionTable(NIGHTDIFFGRAVEYARDSTART, NIGHTDIFFGRAVEYARDEND, iTimeMinTemp, iTimeMinTemp2, NDFRACTION, HourType.NDHour);
                                                    #endregion
                                                }
                                            }
                                        }
                                        #endregion

                                        #region NDFraction Filter (Normal)
                                        iCompRegNightPremMin = Convert.ToInt32((iCompRegNightPremMin / iNDFraction)) * iNDFraction;
                                        iCompOvertimeNightPremMin = Convert.ToInt32((iCompOvertimeNightPremMin / iNDFraction)) * iNDFraction;
                                        ////HOYA
                                        if (strDayCode.Equals("REG5") || (Convert.ToBoolean(NDPREM1ST8))) //&& (bIsRestDay || bIsHoliday))) //Commented by Rendell Uy (10/6/2015)
                                        {
                                            iNDSum = iCompRegNightPremMin + iCompOvertimeNightPremMin;
                                            if (iNDSum > 8 * 60)
                                            {
                                                iCompRegNightPremMin = 8 * 60; //ND hours is set to 8 hours
                                                iCompOvertimeNightPremMin = iNDSum - iCompRegNightPremMin; //excess 8 hours is set to NDOT hours
                                            }
                                            else
                                            {
                                                iCompRegNightPremMin = iNDSum; //all ND and NDOT hours to ND premium
                                                iCompOvertimeNightPremMin = 0; //no NDOT hours
                                            }
                                        }
                                        #endregion
                                    }
                                    #endregion

                                    #region Computed Day Work Minutes for Restday or Holiday
                                    if (bIsRestDay || bIsHoliday || !bIsRegOrReg5DayCode || strDayCode.Equals("REGA"))
                                    {
                                        //Overtime Hour is Pre-added with Paid Break Hours
                                        if (iComputedOvertimeMin > 0)
                                            iComputedOvertimeMin += iPaidBreak;
                                        //COMPUTED DAY WORK MIN
                                        iComputedDayWorkMin = iComputedOvertimeMin;
                                        //COMPUTED REGULAR MIN
                                        iComputedRegularMin = (iComputedDayWorkMin > iShiftMin) ? iShiftMin : iComputedDayWorkMin;
                                        //COMPUTED OVERTIME MIN
                                        iComputedOvertimeMin = (iComputedOvertimeMin > iShiftMin) ? iComputedOvertimeMin - iShiftMin : 0;

                                        #region Re-evaluate Absent Minutes (in case of holidays)
                                        iComputedLateMin = 0;
                                        iComputedLate2Min = 0;
                                        iComputedUndertime1Min = 0;
                                        iComputedUndertime2Min = 0;

                                        if (bDailiesNoWorkNoPay == false) //Legal Holiday, Company Holiday, etc
                                        {
                                            iInitialAbsentMin = 0;
                                            iComputedAbsentMin = 0;
                                            iNoPayLeaveMin = 0;
                                            iTotalComputedAbsentMin = 0;
                                        }
                                        else  //SPL, CMPY and PSD (Dailies)
                                        {
                                            #region Compute Deduction Minutes
                                            //Computed Absent Min
                                            if (iComputedRegularMin > 0 && iShiftMin - iComputedRegularMin > 0)
                                            {
                                                iComputedAbsentMin = iShiftMin - iComputedRegularMin;
                                            }
                                            else if (iComputedRegularMin == 0)
                                            {
                                                iComputedAbsentMin = iShiftMin;

                                                //Zero-out Actual Late and Undertime because employee has no worked hours
                                                iActualLate1Mins = 0;
                                                iActualLate2Mins = 0;
                                                iActualUT1Mins = 0;
                                                iActualUT2Mins = 0;
                                            }
                                            else
                                            {
                                                iComputedAbsentMin = 0;
                                            }

                                            //Cleanup Paid Leave
                                            if (iPayLeaveMin > 0)
                                            {
                                                //Just in case the leave hours filed is greater than the shift hours
                                                if (iShiftMin - iPayLeaveMin < 0)
                                                    iPayLeaveMin = iShiftMin;

                                                if (iPayLeaveMin <= iComputedAbsentMin)
                                                {
                                                    iComputedAbsentMin -= iPayLeaveMin;
                                                    iComputedDayWorkMin += iPayLeaveMin;
                                                }
                                                else
                                                {
                                                    if (Convert.ToBoolean(FLEXSHIFT) == true && Convert.ToBoolean(EXTREGLVE) == false)
                                                    {
                                                        iComputedRegularMin = iShiftMin - iPayLeaveMin;
                                                        iComputedDayWorkMin = iShiftMin;
                                                    }
                                                    else
                                                    {
                                                        iPayLeaveMin = iComputedAbsentMin;
                                                        iComputedDayWorkMin += iComputedAbsentMin;
                                                    }

                                                    iComputedAbsentMin = 0;
                                                    iInitialAbsentMin = 0;
                                                }
                                            }

                                            //Cleanup Unpaid Leave
                                            if (iNoPayLeaveMin > 0)
                                            {
                                                //Just in case the leave hours filed is greater than the shift hours
                                                if (iShiftMin - iNoPayLeaveMin < 0)
                                                    iNoPayLeaveMin = iShiftMin;

                                                if (iNoPayLeaveMin > iComputedAbsentMin)
                                                {
                                                    if (Convert.ToBoolean(FLEXSHIFT) == true && Convert.ToBoolean(EXTREGULVE) == false)
                                                    {
                                                        iComputedRegularMin = iShiftMin - iNoPayLeaveMin;
                                                        iComputedDayWorkMin = iComputedRegularMin;
                                                    }
                                                    else
                                                    {
                                                        iNoPayLeaveMin = iComputedAbsentMin;
                                                    }
                                                }

                                                iInitialAbsentMin = 0;
                                                iComputedAbsentMin = 0;
                                            }

                                            //Total Absent Minutes for Holidays
                                            iTotalComputedAbsentMin = iComputedAbsentMin + iNoPayLeaveMin;
                                            #endregion
                                        }
                                        #endregion
                                    }
                                    else
                                        iComputedDayWorkMin += iComputedOvertimeMin;
                                    #endregion

                                    #region //Offset Hours Checking
                                    //if (Convert.ToBoolean(TKOFFSET))
                                    //{
                                    //    iAccumulatedMins = 0;
                                    //    drArrOffsetApp = dtOffsetTable.Select("Eof_OffsetDate = '" + strProcessDate + "'");
                                    //    //loop to sum all accumulated hours
                                    //    foreach (DataRow drOffset in drArrOffsetApp)
                                    //    {
                                    //        iAccumulatedMins += Convert.ToInt32(drOffset["Eof_ActualAccumulatedMin"]);
                                    //    }

                                    //    if (drArrOffsetApp.Length > 0) // && iComputedRegularMin + iAccumulatedMins >= iShiftMin)
                                    //    {
                                    //        //offset applications valid
                                    //        //loop to get actual offset hours
                                    //        iExcessOffset = 0;
                                    //        iForOffsetMin = 0;
                                    //        foreach (DataRow drOffset in drArrOffsetApp)
                                    //        {
                                    //            iAccumulatedMins = Convert.ToInt32(drOffset["Eof_ActualAccumulatedMin"]);
                                    //            if ((iComputedRegularMin + iAccumulatedMins) - iShiftMin > 0)
                                    //            {
                                    //                iExcessOffset += (iComputedRegularMin + iAccumulatedMins) - iShiftMin;
                                    //                iForOffsetMin += iShiftMin - iComputedRegularMin;
                                    //                iComputedRegularMin = iShiftMin;
                                    //                iComputedDayWorkMin = iShiftMin;
                                    //                drOffset["Eof_ActualOffsetMin"] = iShiftMin - iComputedRegularMin;
                                    //                UpdateOffsetHours(drOffset["Eof_CurrentPayPeriod"].ToString(), drOffset["Eof_EmployeeId"].ToString(), drOffset["Eof_SeqNo"].ToString(), drOffset["Eof_OffsetDate"].ToString(), iShiftMin - iComputedRegularMin, LoginUser);
                                    //            }
                                    //            else
                                    //            {
                                    //                iForOffsetMin += iAccumulatedMins;
                                    //                iComputedRegularMin += iAccumulatedMins;
                                    //                iComputedDayWorkMin += iAccumulatedMins;
                                    //                drOffset["Eof_ActualOffsetMin"] = iAccumulatedMins;
                                    //                UpdateOffsetHours(drOffset["Eof_CurrentPayPeriod"].ToString(), drOffset["Eof_EmployeeId"].ToString(), drOffset["Eof_SeqNo"].ToString(), drOffset["Eof_OffsetDate"].ToString(), iAccumulatedMins, LoginUser);
                                    //            }
                                    //        }
                                    //    }
                                    //}
                                    #endregion
                                }

                                #region Overtime with OB Computation
                                if (Convert.ToBoolean(OBCOMPOT))
                                {
                                    bCountOTFraction = false;
                                    if (Convert.ToBoolean(RNDOTFRAC) == true
                                            && (Convert.ToInt32(iShiftTimeOut1Min / iOTFraction) * iOTFraction == iShiftTimeOut1Min
                                                || Convert.ToInt32(iShiftTimeIn2Min / iOTFraction) * iOTFraction == iShiftTimeIn2Min))
                                        bCountOTFraction = true;

                                    iAdvOTMin = 0;
                                    iTimeMinTemp = 0;
                                    iTimeMinTemp2 = 0;
                                    ComputeOvertimeForOBEx(curEmployeeID, strProcessDate
                                                            , iShiftTimeIn1Min, iShiftTimeOut1Min, iShiftTimeIn2Min, iShiftTimeOut2Min
                                                            , ref iAdvOTMin, ref iTimeMinTemp, ref iTimeMinTemp2
                                                            , strDayCode, (bIsRestDay || bIsHoliday || !bIsRegOrReg5DayCode), bIsGraveyard && !bOverrideGraveyardConv
                                                            , bCountOTFraction, iOTFraction, iNDFraction, iMasterPaidBreak, true
                                                            , drArrOTApproved);

                                    iComputedOvertimeMin += iAdvOTMin;
                                    iCompRegNightPremMin += iTimeMinTemp;
                                    iCompOvertimeNightPremMin += iTimeMinTemp2;
                                    if (bIsRestDay || bIsHoliday || !bIsRegOrReg5DayCode || strDayCode.Equals("REGA")) //if restday/holiday, re-evaluate..
                                    {
                                        //COMPUTED DAY WORK MIN
                                        iComputedDayWorkMin = iComputedRegularMin + iComputedOvertimeMin;
                                        //COMPUTED REGULAR MIN
                                        iComputedRegularMin = (iComputedDayWorkMin > iShiftMin) ? iShiftMin : iComputedDayWorkMin;
                                        //COMPUTED OVERTIME MIN
                                        iComputedOvertimeMin = (iComputedDayWorkMin > iShiftMin) ? iComputedDayWorkMin - iShiftMin : 0;
                                    }
                                }
                                #endregion

                                #region Offset Absence Checking
                                if (strDayCode.Equals("REGA"))
                                {
                                    #region COZO
                                    iComputedLateMin = 0;
                                    iComputedLate2Min = 0;
                                    iComputedUndertime1Min = 0;
                                    iComputedUndertime2Min = 0;
                                    iInitialAbsentMin = 0;
                                    iComputedAbsentMin = 0;
                                    iTotalComputedAbsentMin = 0;
                                    #endregion
                                }
                                else if (OTOFSETABS.Equals("True") && iTotalComputedAbsentMin > 0)
                                {
                                    #region HOGP (Overtime offsets Absence)
                                    if (iComputedOvertimeMin > 0)
                                    {
                                        if (iTotalComputedAbsentMin >= iComputedOvertimeMin)
                                        {
                                            //COMPUTED REGULAR MIN
                                            iComputedRegularMin += iComputedOvertimeMin;
                                            iComputedDayWorkMin = iComputedRegularMin;
                                            iComputedRegularMin = (iComputedDayWorkMin > iShiftMin) ? iShiftMin : iComputedDayWorkMin;
                                            //COMPUTED OVERTIME MIN
                                            iTimeMinTemp = iComputedOvertimeMin;
                                            iComputedOvertimeMin = (iComputedDayWorkMin > iShiftMin) ? iComputedDayWorkMin - iShiftMin : 0;
                                            //COMPUTED ABSENT MIN
                                            iTotalComputedAbsentMin -= iTimeMinTemp;
                                            //OFFSET OVERTIME MIN
                                            iOffsetOvertimeMin = iTimeMinTemp;

                                            #region Subtract absent details
                                            //Subtract absent details
                                            ///Whole-day Absent
                                            if (iInitialAbsentMin > 0 && iTimeMinTemp > 0)
                                            {
                                                if (iInitialAbsentMin == iTimeMinTemp)
                                                {
                                                    iTimeMinTemp -= iInitialAbsentMin;
                                                    iInitialAbsentMin = 0;
                                                }
                                            }
                                            ///Unpaid Leave
                                            if (iNoPayLeaveMin > 0 && iTimeMinTemp > 0)
                                            {
                                                if (iNoPayLeaveMin <= iTimeMinTemp)
                                                {
                                                    iTimeMinTemp -= iNoPayLeaveMin;
                                                    iNoPayLeaveMin = 0;
                                                }
                                                else
                                                {
                                                    iNoPayLeaveMin -= iTimeMinTemp;
                                                    iTimeMinTemp = 0;
                                                }
                                            }
                                            ///Late AM
                                            if (iComputedLateMin > 0 && iTimeMinTemp > 0)
                                            {
                                                if (iComputedLateMin <= iTimeMinTemp)
                                                {
                                                    iTimeMinTemp -= iComputedLateMin;
                                                    iComputedLateMin = 0;
                                                }
                                                else
                                                {
                                                    iComputedLateMin -= iTimeMinTemp;
                                                    iTimeMinTemp = 0;
                                                }
                                            }
                                            ///Late PM
                                            if (iComputedLate2Min > 0 && iTimeMinTemp > 0)
                                            {
                                                if (iComputedLate2Min <= iTimeMinTemp)
                                                {
                                                    iTimeMinTemp -= iComputedLate2Min;
                                                    iComputedLate2Min = 0;
                                                }
                                                else
                                                {
                                                    iComputedLate2Min -= iTimeMinTemp;
                                                    iTimeMinTemp = 0;
                                                }
                                            }
                                            ///Undertime AM
                                            if (iComputedUndertime1Min > 0 && iTimeMinTemp > 0)
                                            {
                                                if (iComputedUndertime1Min <= iTimeMinTemp)
                                                {
                                                    iTimeMinTemp -= iComputedUndertime1Min;
                                                    iComputedUndertime1Min = 0;
                                                }
                                                else
                                                {
                                                    iComputedUndertime1Min -= iTimeMinTemp;
                                                    iTimeMinTemp = 0;
                                                }
                                            }
                                            ///Undertime PM
                                            if (iComputedUndertime2Min > 0 && iTimeMinTemp > 0)
                                            {
                                                if (iComputedUndertime2Min <= iTimeMinTemp)
                                                {
                                                    iTimeMinTemp -= iComputedUndertime2Min;
                                                    iComputedUndertime2Min = 0;
                                                }
                                                else
                                                {
                                                    iComputedUndertime2Min -= iTimeMinTemp;
                                                    iTimeMinTemp = 0;
                                                }
                                            }
                                            #endregion
                                        }
                                        else
                                        {
                                            //COMPUTED OVERTIME MIN
                                            iComputedOvertimeMin -= iTotalComputedAbsentMin;
                                            //COMPUTED REGULAR MIN
                                            if (iComputedRegularMin + iTotalComputedAbsentMin <= iShiftMin)
                                                iComputedRegularMin += iTotalComputedAbsentMin;
                                            //OFFSET OVERTIME MIN
                                            iOffsetOvertimeMin = iTotalComputedAbsentMin;
                                            //COMPUTED DAY WORK MIN
                                            iComputedDayWorkMin = iComputedRegularMin + iComputedOvertimeMin;
                                            //COMPUTED ABSENT MIN
                                            iComputedLateMin = 0;
                                            iComputedLate2Min = 0;
                                            iComputedUndertime1Min = 0;
                                            iComputedUndertime2Min = 0;
                                            iInitialAbsentMin = 0;
                                            iComputedAbsentMin = 0;
                                            iTotalComputedAbsentMin = 0;
                                            iNoPayLeaveMin = 0;
                                        }
                                    }
                                    #endregion
                                }
                                #endregion

                                #region Checking of Minimum OT Hour
                                if ((bIsRestDay || bIsHoliday || !bIsRegOrReg5DayCode) && bDailiesNoWorkNoPay == false) //Rest day or holiday
                                {
                                    if (iComputedRegularMin + iComputedOvertimeMin < MINOTHR * 60)
                                    {
                                        iComputedRegularMin = 0;
                                        iComputedOvertimeMin = 0;
                                        iComputedDayWorkMin = 0;
                                    }
                                }
                                else if (iComputedOvertimeMin < MINOTHR * 60)
                                {
                                    iComputedDayWorkMin -= iComputedOvertimeMin;
                                    iComputedOvertimeMin = 0;
                                }
                                #endregion

                                #region No Absent for New Hirees
                                //DASH
                                if (bIsNewHire && Convert.ToBoolean(NOABSNWHRE))
                                {
                                    iComputedLateMin = 0;
                                    iComputedLate2Min = 0;
                                    iComputedUndertime1Min = 0;
                                    iComputedUndertime2Min = 0;
                                    iInitialAbsentMin = 0;
                                    iComputedAbsentMin = 0;
                                    iNoPayLeaveMin = 0;
                                    iTotalComputedAbsentMin = 0;
                                }
                                #endregion

                                #region Holiday Previous Day Processing
                                dtEmployeeLogLedger.Rows[i]["Ell_PreviousDayHolidayReference"] = DBNull.Value;
                                dtEmployeeLogLedger.Rows[i]["Ell_PreviousDayWorkMin"] = 0;
                                dtEmployeeLogLedger.Rows[i]["Ell_SundayHolidayCount"] = 0;
                                dtEmployeeLogLedger.Rows[i]["Ell_ForceLeave"] = 0;
                                drArrPrevDay = dtHolidays.Select(string.Format("Hmt_HolidayDate = '{0}' AND (Hmt_ApplicCity = 'ALL' OR Hmt_ApplicCity = '{1}')", dtEmployeeLogLedger.Rows[i]["Ell_ProcessDate"], dtEmployeeLogLedger.Rows[i]["Ell_LocationCode"]));
                                if (drArrPrevDay != null && drArrPrevDay.Length > 0)
                                {
                                    drHol = drArrPrevDay[0];
                                    iHolPrevDayInMin = Convert.ToInt32(Convert.ToDecimal(drHol["Hmt_PrvDayHr"].ToString()) * 60);
                                    iDecrement = 1;
                                    bIsFound = false;
                                    do
                                    {
                                        #region Get Previous Day Record
                                        if (Convert.ToDateTime(drHol["Hmt_HolidayDate"].ToString()).AddDays(-iDecrement) >= Convert.ToDateTime(PayrollStart))
                                        {
                                            drArrPrevDay = dtEmployeeLogLedger.Select("Ell_EmployeeId = '" + dtEmployeeLogLedger.Rows[i]["Ell_EmployeeId"].ToString()
                                                                                            + "' AND Ell_ProcessDate = '" + Convert.ToDateTime(drHol["Hmt_HolidayDate"].ToString()).AddDays(-iDecrement).ToString("MM/dd/yyyy") + "'", "");
                                            drPrevDay = drArrPrevDay[0];
                                        }
                                        else
                                        {
                                            drPrevDay = GetHolidayPrevDayHist(dtEmployeeLogLedger.Rows[i]["Ell_EmployeeId"].ToString()
                                                                                    , Convert.ToDateTime(drHol["Hmt_HolidayDate"].ToString()).AddDays(-iDecrement).ToString("MM/dd/yyyy"));
                                        }
                                        #endregion

                                        if (drPrevDay != null
                                            || (bIsNewHire == true && strHireDate != "" && Convert.ToDateTime(strHireDate) >= Convert.ToDateTime(dtEmployeeLogLedger.Rows[i]["Ell_ProcessDate"])))
                                        {
                                            #region Previous Day Values
                                            if (drPrevDay == null)
                                            {
                                                strPreviousDayReference = Convert.ToDateTime(drHol["Hmt_HolidayDate"].ToString()).AddDays(-iDecrement).ToString("MM/dd/yyyy");
                                                iPrevCompDayWorkMin = 0;
                                                strPrevDayCode = "REG";
                                                bPrevRestDay = false;
                                                strPreviousDayLeaveType = "";
                                            }
                                            else
                                            {
                                                strPreviousDayReference = drPrevDay["Ell_ProcessDate"].ToString();
                                                iPrevCompDayWorkMin = Convert.ToInt32((Convert.ToDouble(drPrevDay["Ell_RegularHour"]) + Convert.ToDouble(drPrevDay["Ell_LeaveHour"]) + Convert.ToDouble(drPrevDay["Ell_OvertimeHour"])) * 60);
                                                strPrevDayCode = drPrevDay["Ell_DayCode"].ToString();
                                                bPrevRestDay = Convert.ToBoolean(drPrevDay["Ell_Restday"]);
                                                strPreviousDayLeaveType = drPrevDay["Ell_EncodedNoPayLeaveType"].ToString();
                                            }
                                            #endregion

                                            ///COMPARISON OF REGPREVDAY AND ONEPREVDAY:
                                            ///REGPREVDAY   ONEPREVDAY      RESULT
                                            /// TRUE         TRUE        PREVIOUS DAY MUST BE THE NEAREST REGULAR DAY (OVERRIDE ONEPREVDAY CONDITION)
                                            /// FALSE        TRUE        PREVIOUS DAY IS THE DAY PRIOR TO THE HOLIDAY, REGARDLESS IF REGULAR OR REST DAY
                                            /// TRUE         FALSE       PREVIOUS DAY MUST BE THE NEAREST REGULAR DAY
                                            /// FALSE        FALSE       PREVIOUS DAY CAN EITHER BE THE NEAREST REGULAR DAY, OR IF THE COMPUTED HOURS OF THE NEAREST DAY IS EQUAL TO THE PREVIOUS DAY HOUR PARAMETER (DEFAULT)
                                            if ((Convert.ToBoolean(REGPREVDAY) == true && (strPrevDayCode.Substring(0, 3).Equals("REG") || strPrevDayCode.Substring(0, 3).Equals("REGN")))
                                                || (Convert.ToBoolean(REGPREVDAY) == false && (iPrevCompDayWorkMin >= iHolPrevDayInMin || strPrevDayCode.Substring(0, 3).Equals("REG") || strPrevDayCode.Substring(0, 3).Equals("REGN") || (Convert.ToBoolean(ONEPREVDAY) == true && bPrevRestDay))))
                                            {
                                                bIsFound = true; //Found previous day record

                                                if (strDayCode == "HOL"
                                                    || (drHol["Hmt_PayTypeExclusion"].ToString() == ""
                                                        && drHol["Hmt_JobStatusExclusion"].ToString() == ""
                                                        && drHol["Hmt_EmpStatusExclusion"].ToString() == ""
                                                        && drHol["Hmt_WorktypegroupExclusion"].ToString() == "")
                                                    || (CheckIfExistsInCommaDelString(dtEmployeeLogLedger.Rows[i]["Emt_PayrollType"].ToString(), drHol["Hmt_PayTypeExclusion"].ToString(), ',') == false
                                                        && CheckIfExistsInCommaDelString(dtEmployeeLogLedger.Rows[i]["Emt_JobStatus"].ToString(), drHol["Hmt_JobStatusExclusion"].ToString(), ',') == false
                                                        && CheckIfExistsInCommaDelString(dtEmployeeLogLedger.Rows[i]["Emt_EmploymentStatus"].ToString(), drHol["Hmt_EmpStatusExclusion"].ToString(), ',') == false
                                                        && CheckIfExistsInCommaDelString(dtEmployeeLogLedger.Rows[i]["Ell_WorkType"].ToString()
                                                                                        + "/" + dtEmployeeLogLedger.Rows[i]["Ell_WorkGroup"].ToString(), drHol["Hmt_WorktypegroupExclusion"].ToString(), ',') == false))
                                                {
                                                    #region Initialize
                                                    iShiftInHours = iShiftMin / 60;
                                                    iPaidHolidayHrs = 0;
                                                    iSundayHolidayCount = 0;

                                                    //Check Previous Day Requirement
                                                    if ((Convert.ToBoolean(ONEPREVDAY) && bPrevRestDay == true && bIsRestDay == true && iPrevCompDayWorkMin >= iHolPrevDayInMin)
                                                            || (bIsRestDay == true && iPrevCompDayWorkMin >= iHolPrevDayInMin)
                                                            || (bIsRestDay == false && iPrevCompDayWorkMin >= iHolPrevDayInMin)
                                                            || (Convert.ToBoolean(MLPAYHOL) && (strPreviousDayLeaveType == "ML" || strPreviousDayLeaveType == "SW") && strPayType.Equals("D"))
                                                            || (ULPREVDAY != null && ULPREVDAY.Rows.Count > 0 && commonBL.IsFoundInParameterTable(ULPREVDAY, strPreviousDayLeaveType) == true))
                                                        bMetHolidayPreviousDay = true;
                                                    else
                                                        bMetHolidayPreviousDay = false;

                                                    if (strDayCode == "PSD" && iComputedRegularMin + iPayLeaveMin + iComputedOvertimeMin == 0)
                                                        bMetHolidayPreviousDay = false;

                                                    //Check if New Hire or Resigned
                                                    if ((bIsNewHire && !Convert.ToBoolean(NOABSNWHRE))
                                                        || (dtEmployeeLogLedger.Rows[i]["Emt_SeparationEffectivityDate"].ToString() != ""
                                                            && Convert.ToDateTime(strProcessDate) >= Convert.ToDateTime(dtEmployeeLogLedger.Rows[i]["Emt_SeparationEffectivityDate"])))
                                                        bIsNewHireOrResigned = true;
                                                    else
                                                        bIsNewHireOrResigned = false;
                                                    #endregion

                                                    #region Checking of Holiday Code
                                                    if (iComputedRegularMin + iPayLeaveMin + iComputedOvertimeMin == 0 //applicable on unworked holidays
                                                        || (strDayCode == "HOL" && bIsRestDay == false && Convert.ToBoolean(LEGHOLINRG) == true)) //or if worked on legal holidays and when LEGHOLINRG is true
                                                    {
                                                        if (strDayCode == "HOL") //only for legal holidays NOT falling on restdays
                                                        {
                                                            #region HOL
                                                            if (bIsRestDay == false)
                                                            {
                                                                if ((bMetHolidayPreviousDay && !bIsNewHireOrResigned)
                                                                    || (Convert.ToBoolean(LEGHOLINRG) == true && iComputedRegularMin + iPayLeaveMin + iComputedOvertimeMin != 0))
                                                                {
                                                                    iPaidHolidayHrs = iShiftInHours;
                                                                    PaidLegalHolidayHr += iShiftInHours;
                                                                }
                                                                else
                                                                {
                                                                    iPaidHolidayHrs = iShiftInHours * -1;
                                                                    AbsentLegalHolidayHr += iShiftInHours;
                                                                }
                                                            }
                                                            #endregion
                                                        }
                                                        else if (strDayCode == "SPL")
                                                        {
                                                            #region SPL
                                                            if (strPayType.Equals("M")) //Monthly only
                                                            {
                                                                if (bMetHolidayPreviousDay && !bIsNewHireOrResigned)
                                                                {
                                                                    iPaidHolidayHrs = iShiftInHours;
                                                                    PaidSpecialHolidayHr += iShiftInHours;
                                                                }
                                                                else if (bIsRestDay == false)
                                                                {
                                                                    iPaidHolidayHrs = iShiftInHours * -1;
                                                                    AbsentSpecialHolidayHr += iShiftInHours;
                                                                }
                                                            }
                                                            #endregion
                                                        }
                                                        else if (strDayCode == "COMP")
                                                        {
                                                            #region COMP
                                                            if (bMetHolidayPreviousDay && !bIsNewHireOrResigned)
                                                            {
                                                                iPaidHolidayHrs = iShiftInHours;
                                                                PaidCompanyHolidayHr += iShiftInHours;
                                                            }
                                                            else if (bIsRestDay == false)
                                                            {
                                                                iPaidHolidayHrs = iShiftInHours * -1;
                                                                AbsentCompanyHolidayHr += iShiftInHours;
                                                            }
                                                            #endregion
                                                        }
                                                        else if (strDayCode == "PSD")
                                                        {
                                                            #region PSD
                                                            if (!bMetHolidayPreviousDay || bIsNewHireOrResigned)
                                                            {
                                                                if (bIsRestDay == false)
                                                                {
                                                                    iPaidHolidayHrs = iShiftInHours * -1;
                                                                    AbsentPlantShutdownHr += iShiftInHours;
                                                                    iComputedLateMin = 0;
                                                                    iComputedLate2Min = 0;
                                                                    iComputedUndertime1Min = 0;
                                                                    iComputedUndertime2Min = 0;
                                                                    iInitialAbsentMin = 0;
                                                                    iComputedAbsentMin = 0;
                                                                    iTotalComputedAbsentMin = 0;
                                                                }
                                                            }
                                                            #endregion
                                                        }
                                                        else if (bHasDayCodeExt)
                                                        {
                                                            #region Filler
                                                            drArrDayCodeFiller = dtDayCodeFillers.Select(string.Format("Dcf_DayCode = '{0}'", strDayCode));
                                                            if (drArrDayCodeFiller.Length > 0)
                                                            {
                                                                if (strDayCode.Equals("CMPY")) //CMPY for HOGP
                                                                {
                                                                    if (strPayType.Equals("M")) //Monthly only
                                                                    {
                                                                        if (bMetHolidayPreviousDay && !bIsNewHireOrResigned)
                                                                        {
                                                                            iPaidHolidayHrs = iShiftInHours;
                                                                            PaidFillerHolidayHr += iShiftInHours;
                                                                        }
                                                                        else if (bIsRestDay == false)
                                                                        {
                                                                            iPaidHolidayHrs = iShiftInHours * -1;
                                                                            AbsentFillerHolidayHr += iShiftInHours;
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (bMetHolidayPreviousDay && !bIsNewHireOrResigned)
                                                                    {
                                                                        iPaidHolidayHrs = iShiftInHours;
                                                                        PaidFillerHolidayHr += iShiftInHours;
                                                                    }
                                                                    else if (bIsRestDay == false)
                                                                    {
                                                                        iPaidHolidayHrs = iShiftInHours * -1;
                                                                        AbsentFillerHolidayHr += iShiftInHours;
                                                                    }
                                                                }
                                                            }
                                                            #endregion
                                                        }
                                                    }
                                                    #endregion

                                                    #region Sunday Holiday Count (Legal Holiday Falling on a Restday)
                                                    if (iComputedRegularMin + iPayLeaveMin + iComputedOvertimeMin == 0 && strDayCode == "HOL" && bMetHolidayPreviousDay == true && bIsRestDay == true && bIsNewHireOrResigned == false)
                                                    {
                                                        if (drHol["Hmt_PaidRestHol"].ToString() != "" 
                                                            && Convert.ToBoolean(drHol["Hmt_PaidRestHol"]) == true
                                                            && ((drHol["Hmt_PayTypeExclusion"].ToString() == ""
                                                                    && drHol["Hmt_JobStatusExclusion"].ToString() == ""
                                                                    && drHol["Hmt_EmpStatusExclusion"].ToString() == ""
                                                                    && drHol["Hmt_WorktypegroupExclusion"].ToString() == "")
                                                                || (CheckIfExistsInCommaDelString(dtEmployeeLogLedger.Rows[i]["Emt_PayrollType"].ToString(), drHol["Hmt_PayTypeExclusion"].ToString(), ',') == false
                                                                    && CheckIfExistsInCommaDelString(dtEmployeeLogLedger.Rows[i]["Emt_JobStatus"].ToString(), drHol["Hmt_JobStatusExclusion"].ToString(), ',') == false
                                                                    && CheckIfExistsInCommaDelString(dtEmployeeLogLedger.Rows[i]["Emt_EmploymentStatus"].ToString(), drHol["Hmt_EmpStatusExclusion"].ToString(), ',') == false
                                                                    && CheckIfExistsInCommaDelString(dtEmployeeLogLedger.Rows[i]["Ell_WorkType"].ToString()
                                                                                                    + "/" + dtEmployeeLogLedger.Rows[i]["Ell_WorkGroup"].ToString(), drHol["Hmt_WorktypegroupExclusion"].ToString(), ',') == false))
                                                            )
                                                        {
                                                            iSundayHolidayCount = 1;
                                                        }
                                                        else
                                                            AddErrorToLaborHourReport(curEmployeeID, dtEmployeeLogLedger.Rows[i]["Emt_LastName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_FirstName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_MiddleName"].ToString(), strProcessDate, "Excluded from Restday Legal Holiday Condition");
                                                    }
                                                    #endregion

                                                    #region Update Columns
                                                    if (strDayCode != "PSD")
                                                    {
                                                        dtEmployeeLogLedger.Rows[i]["Ell_PreviousDayHolidayReference"] = strPreviousDayReference;
                                                        dtEmployeeLogLedger.Rows[i]["Ell_PreviousDayWorkMin"] = iPrevCompDayWorkMin;
                                                    }
                                                    dtEmployeeLogLedger.Rows[i]["Ell_SundayHolidayCount"] = iSundayHolidayCount;
                                                    dtEmployeeLogLedger.Rows[i]["Ell_ForceLeave"] = iPaidHolidayHrs;
                                                    #endregion
                                                }
                                                else
                                                    AddErrorToLaborHourReport(curEmployeeID, dtEmployeeLogLedger.Rows[i]["Emt_LastName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_FirstName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_MiddleName"].ToString(), strProcessDate, "Excluded from Holiday Condition");
                                            }
                                        }
                                        iDecrement++;
                                    } while (!bIsFound && iDecrement <= 30);
                                }
                                #endregion

                                #region Working Day
                                if (!Convert.ToBoolean(dtEmployeeLogLedger.Rows[i]["Ell_RestDay"]))
                                {
                                    dtEmployeeLogLedger.Rows[i]["Ell_WorkingDay"] = 1;

                                    //Check if New Hire or Resigned
                                    if ((bIsNewHire && !Convert.ToBoolean(NOABSNWHRE))
                                        || (dtEmployeeLogLedger.Rows[i]["Emt_SeparationEffectivityDate"].ToString() != ""
                                            && Convert.ToDateTime(strProcessDate) > Convert.ToDateTime(dtEmployeeLogLedger.Rows[i]["Emt_SeparationEffectivityDate"])))
                                        dtEmployeeLogLedger.Rows[i]["Ell_WorkingDay"] = 0;
                                }
                                else
                                    dtEmployeeLogLedger.Rows[i]["Ell_WorkingDay"] = 0;
                                #endregion

                                #region Check for Errors
                                if ((!bIsRestDay && !bIsHoliday && bIsRegOrReg5DayCode) && iComputedRegularMin + iPayLeaveMin + iTotalComputedAbsentMin != iShiftMin)
                                    AddErrorToLaborHourReport(curEmployeeID, dtEmployeeLogLedger.Rows[i]["Emt_LastName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_FirstName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_MiddleName"].ToString(), strProcessDate, "Reg Mins + Abs Mins <> Shift Mins");
                                if (iComputedRegularMin > iShiftMin)
                                    AddErrorToLaborHourReport(curEmployeeID, dtEmployeeLogLedger.Rows[i]["Emt_LastName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_FirstName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_MiddleName"].ToString(), strProcessDate, "Reg Hr > Shift Hr");
                                if (iTotalComputedAbsentMin > iShiftMin)
                                    AddErrorToLaborHourReport(curEmployeeID, dtEmployeeLogLedger.Rows[i]["Emt_LastName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_FirstName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_MiddleName"].ToString(), strProcessDate, "Abs Hr > Shift Hr");
                                if (iComputedRegularMin < 0)
                                    AddErrorToLaborHourReport(curEmployeeID, dtEmployeeLogLedger.Rows[i]["Emt_LastName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_FirstName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_MiddleName"].ToString(), strProcessDate, "Reg Hr < 0");
                                if (iTotalComputedAbsentMin < 0)
                                    AddErrorToLaborHourReport(curEmployeeID, dtEmployeeLogLedger.Rows[i]["Emt_LastName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_FirstName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_MiddleName"].ToString(), strProcessDate, "Abs Hr < 0");
                                if (iEncodedOvertimeMin < iComputedOvertimeMin)
                                    AddErrorToLaborHourReport(curEmployeeID, dtEmployeeLogLedger.Rows[i]["Emt_LastName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_FirstName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_MiddleName"].ToString(), strProcessDate, "Overpaid OT");
                                if (Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_EncodedOvertimeAdvHr"]) < 0)
                                    AddErrorToLaborHourReport(curEmployeeID, dtEmployeeLogLedger.Rows[i]["Emt_LastName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_FirstName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_MiddleName"].ToString(), strProcessDate, "Invalid Adv OT");
                                if (Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_EncodedOvertimePostHr"]) < 0)
                                    AddErrorToLaborHourReport(curEmployeeID, dtEmployeeLogLedger.Rows[i]["Emt_LastName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_FirstName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_MiddleName"].ToString(), strProcessDate, "Invalid Post OT");
                                if ((bIsRestDay || bIsHoliday || !bIsRegOrReg5DayCode || strDayCode.Equals("REGA"))
                                        && iComputedRegularMin + iComputedOvertimeMin != iComputedDayWorkMin)
                                    AddErrorToLaborHourReport(curEmployeeID, dtEmployeeLogLedger.Rows[i]["Emt_LastName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_FirstName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_MiddleName"].ToString(), strProcessDate, "Incorrect Total Working Hours");
                                if ((!bIsRestDay && !bIsHoliday && bIsRegOrReg5DayCode)
                                        && iComputedRegularMin + iComputedOvertimeMin + iPayLeaveMin != iComputedDayWorkMin)
                                    AddErrorToLaborHourReport(curEmployeeID, dtEmployeeLogLedger.Rows[i]["Emt_LastName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_FirstName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_MiddleName"].ToString(), strProcessDate, "Incorrect Total Working Hours");
                                #endregion

                                #region Save Changes
                                dtEmployeeLogLedger.Rows[i]["Ell_ConvertedTimeIn_1Min"] = iConvTimeIn1Min;
                                dtEmployeeLogLedger.Rows[i]["Ell_ConvertedTimeOut_1Min"] = iConvTimeOut1Min;
                                dtEmployeeLogLedger.Rows[i]["Ell_ConvertedTimeIn_2Min"] = iConvTimeIn2Min;
                                dtEmployeeLogLedger.Rows[i]["Ell_ConvertedTimeOut_2Min"] = iConvTimeOut2Min;
                                dtEmployeeLogLedger.Rows[i]["Ell_ShiftTimeIn_1Min"] = iShiftTimeIn1Min;
                                dtEmployeeLogLedger.Rows[i]["Ell_ShiftTimeOut_1Min"] = iShiftTimeOut1Min;
                                dtEmployeeLogLedger.Rows[i]["Ell_ShiftTimeIn_2Min"] = iShiftTimeIn2Min;
                                dtEmployeeLogLedger.Rows[i]["Ell_ShiftTimeOut_2Min"] = iShiftTimeOut2Min;
                                dtEmployeeLogLedger.Rows[i]["Ell_EncodedOvertimeMin"] = iEncodedOvertimeMin;
                                dtEmployeeLogLedger.Rows[i]["Ell_ComputedLateMin"] = iComputedLateMin;
                                dtEmployeeLogLedger.Rows[i]["Ell_ComputedTimeIn_1Min"] = iCompTimeIn1Min;
                                dtEmployeeLogLedger.Rows[i]["Ell_ComputedTimeOut_1Min"] = iCompTimeOut1Min;
                                dtEmployeeLogLedger.Rows[i]["Ell_ComputedTimeIn_2Min"] = iCompTimeIn2Min;
                                dtEmployeeLogLedger.Rows[i]["Ell_ComputedTimeOut_2Min"] = iCompTimeOut2Min;
                                dtEmployeeLogLedger.Rows[i]["Ell_ComputedRegularMin"] = iComputedRegularMin;
                                dtEmployeeLogLedger.Rows[i]["Ell_ComputedOvertimeMin"] = iComputedOvertimeMin;
                                dtEmployeeLogLedger.Rows[i]["Ell_ComputedDayWorkMin"] = iComputedDayWorkMin;
                                dtEmployeeLogLedger.Rows[i]["Ell_InitialAbsentMin"] = iInitialAbsentMin;
                                dtEmployeeLogLedger.Rows[i]["Ell_ComputedAbsentMin"] = iComputedAbsentMin;
                                dtEmployeeLogLedger.Rows[i]["Ell_OffsetOvertimeMin"] = iOffsetOvertimeMin;
                                dtEmployeeLogLedger.Rows[i]["Ell_PayLeaveMin"] = iPayLeaveMin;
                                dtEmployeeLogLedger.Rows[i]["Ell_NoPayLeaveMin"] = iNoPayLeaveMin;
                                dtEmployeeLogLedger.Rows[i]["Ell_ExcessLeaveMin"] = iExcessLeaveMin;
                                dtEmployeeLogLedger.Rows[i]["Ell_AdjustShiftMin"] = iAdjShiftMin;
                                dtEmployeeLogLedger.Rows[i]["Ell_ComputedRegularNightPremMin"] = iCompRegNightPremMin;
                                dtEmployeeLogLedger.Rows[i]["Ell_ComputedOvertimeNightPremMin"] = iCompOvertimeNightPremMin;
                                dtEmployeeLogLedger.Rows[i]["Ell_ForOffsetMin"] = iActualLate1Mins + iActualLate2Mins;
                                dtEmployeeLogLedger.Rows[i]["Ell_ExcessOffset"] = iActualUT1Mins + iActualUT2Mins;
                                dtEmployeeLogLedger.Rows[i]["Ell_ComputedLate2Min"] = iComputedLate2Min;
                                dtEmployeeLogLedger.Rows[i]["Ell_ComputedUndertimeMin"] = iComputedUndertime1Min;
                                dtEmployeeLogLedger.Rows[i]["Ell_ComputedUndertime2Min"] = iComputedUndertime2Min;
                                #endregion

                                #region Convert to Hours
                                dtEmployeeLogLedger.Rows[i]["Ell_AbsentHour"] = Math.Round(iTotalComputedAbsentMin / 60.0, 2);
                                dtEmployeeLogLedger.Rows[i]["Ell_RegularHour"] = Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_ComputedRegularMin"]) / 60.0, 2);
                                dtEmployeeLogLedger.Rows[i]["Ell_OvertimeHour"] = Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_ComputedOvertimeMin"]) / 60.0, 2);
                                dtEmployeeLogLedger.Rows[i]["Ell_RegularNightPremHour"] = Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_ComputedRegularNightPremMin"]) / 60.0, 2);
                                dtEmployeeLogLedger.Rows[i]["Ell_OvertimeNightPremHour"] = Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_ComputedOvertimeNightPremMin"]) / 60.0, 2);
                                dtEmployeeLogLedger.Rows[i]["Ell_LeaveHour"] = Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_PayLeaveMin"]) / 60.0, 2);

                                #region Actual Hours Value
                                dtEmployeeLogLedger.Rows[i]["Ell_ExpectedHour"] = 0;
                                if (Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_ComputedTimeOut_1Min"]) - Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_ConvertedTimeIn_1Min"]) > 0)
                                {
                                    dtEmployeeLogLedger.Rows[i]["Ell_ExpectedHour"] = Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_ComputedTimeOut_1Min"]) - Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_ConvertedTimeIn_1Min"]), 2);
                                }
                                if (Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_ConvertedTimeOut_2Min"]) != 0 && Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_ConvertedTimeOut_2Min"]) < Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_ShiftTimeIn_1Min"]))
                                {
                                    if ((Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_ConvertedTimeOut_2Min"]) + 1440) - Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_ComputedTimeIn_2Min"]) > 0)
                                        dtEmployeeLogLedger.Rows[i]["Ell_ExpectedHour"] = Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_ExpectedHour"]) + (Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_ConvertedTimeOut_2Min"]) + 1440) - Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_ComputedTimeIn_2Min"]), 2);
                                }
                                else
                                {
                                    if (Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_ConvertedTimeOut_2Min"]) - Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_ComputedTimeIn_2Min"]) > 0)
                                        dtEmployeeLogLedger.Rows[i]["Ell_ExpectedHour"] = Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_ExpectedHour"]) + Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_ConvertedTimeOut_2Min"]) - Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_ComputedTimeIn_2Min"]), 2);
                                }
                                dtEmployeeLogLedger.Rows[i]["Ell_ExpectedHour"] = Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_ExpectedHour"]) / 60.0, 2);
                                #endregion

                                if ((!bIsRestDay && !bIsHoliday && bIsRegOrReg5DayCode)
                                    && Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_RegularHour"]) + Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_LeaveHour"]) + Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_AbsentHour"]), 2)
                                        != Math.Round(Convert.ToDouble(iShiftMin / 60.0), 2))
                                    AddErrorToLaborHourReport(curEmployeeID, dtEmployeeLogLedger.Rows[i]["Emt_LastName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_FirstName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_MiddleName"].ToString(), strProcessDate, "Reg Hr + Abs Hr <> Shift Hr");
                                #endregion
                            }

                            #region Payroll Transaction Totals
                            iShiftMin = Convert.ToInt32(dtEmployeeLogLedger.Rows[i]["Ell_ShiftMin"].ToString());

                            //Regular Hours Requirement
                            if (HasMetRegularHourRequirement(dtEmployeeLogLedger.Rows[i]["Emt_JobStatus"].ToString(), Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_RegularHour"]), Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_LeaveHour"]), dtEmployeeLogLedger.Rows[i]["Ell_DayCode"].ToString()))
                            {
                                dtEmployeeLogLedger.Rows[i]["Ell_RegularHour"] = (iShiftMin / 60.0) - Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_LeaveHour"]);
                                dtEmployeeLogLedger.Rows[i]["Ell_AbsentHour"] = 0;
                                dtEmployeeLogLedger.Rows[i]["Ell_ComputedLateMin"] = 0;
                                dtEmployeeLogLedger.Rows[i]["Ell_ComputedLate2Min"] = 0;
                                dtEmployeeLogLedger.Rows[i]["Ell_ComputedUndertimeMin"] = 0;
                                dtEmployeeLogLedger.Rows[i]["Ell_ComputedUndertime2Min"] = 0;
                                dtEmployeeLogLedger.Rows[i]["Ell_InitialAbsentMin"] = 0;
                                dtEmployeeLogLedger.Rows[i]["Ell_ComputedAbsentMin"] = 0;
                            }

                            //Filter Regular Hours for REG only
                            if (Convert.ToBoolean(dtEmployeeLogLedger.Rows[i]["Ell_Holiday"]) == false
                                && Convert.ToBoolean(dtEmployeeLogLedger.Rows[i]["Ell_RestDay"]) == false
                                && (dtEmployeeLogLedger.Rows[i]["Ell_DayCode"].ToString() == "REG"
                                    || dtEmployeeLogLedger.Rows[i]["Ell_DayCode"].ToString() == "REGN"))
                            {
                                RegularHrMonthlyDailyPay += Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_RegularHour"]), 2);
                            }

                            LateHours += Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_ComputedLateMin"]) / 60.0, 2);
                            LateHours += Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_ComputedLate2Min"]) / 60.0, 2);
                            UndertimeHours += Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_ComputedUndertimeMin"]) / 60.0, 2);
                            UndertimeHours += Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_ComputedUndertime2Min"]) / 60.0, 2);
                            PaidLeaveHours += Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_PayLeaveMin"]) / 60.0, 2);
                            UnpaidLeaveHours += Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_NoPayLeaveMin"]) / 60.0, 2);
                            WholeDayAbsentHours += Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_InitialAbsentMin"]) / 60.0, 2);
                            AbsentHr += Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_AbsentHour"]), 2);
                            RestdayLegalHolidayCount += Convert.ToInt32(dtEmployeeLogLedger.Rows[i]["Ell_SundayHolidayCount"]);
                            WorkingDay += Convert.ToInt32(dtEmployeeLogLedger.Rows[i]["Ell_WorkingDay"]);
                            #endregion

                            drArrEmpWithSalaryMovement = dtEmpWithSalaryMovement.Select("Esm_EmployeeId = '" + curEmployeeID + "'");
                            if (!bProcessTrail)
                            {
                                if (!Convert.ToBoolean(HRFRCLBRHR)) //Normal
                                {
                                    #region Payroll Transaction variables
                                    if ((dtEmployeeLogLedger.Rows[i]["Ell_DayCode"].ToString().Equals("REG")
                                            || dtEmployeeLogLedger.Rows[i]["Ell_DayCode"].ToString().Equals("REGN"))
                                        && !Convert.ToBoolean(dtEmployeeLogLedger.Rows[i]["Ell_RestDay"]))
                                    {
                                        RegularHr += Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_RegularHour"]), 2);
                                        RegularOTHr += Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_OvertimeHour"]), 2);
                                        RegularNDHr += Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_RegularNightPremHour"]), 2);
                                        RegularOTNDHr += Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_OvertimeNightPremHour"]), 2);
                                    }
                                    else if (dtEmployeeLogLedger.Rows[i]["Ell_DayCode"].ToString().Equals("REST") && Convert.ToBoolean(dtEmployeeLogLedger.Rows[i]["Ell_RestDay"]))
                                    {
                                        RestdayHr += Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_RegularHour"]), 2);
                                        RestdayOTHr += Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_OvertimeHour"]), 2);
                                        RestdayNDHr += Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_RegularNightPremHour"]), 2);
                                        RestdayOTNDHr += Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_OvertimeNightPremHour"]), 2);
                                    }
                                    else if (dtEmployeeLogLedger.Rows[i]["Ell_DayCode"].ToString().Equals("HOL") && !Convert.ToBoolean(dtEmployeeLogLedger.Rows[i]["Ell_RestDay"]))
                                    {
                                        LegalHolidayHr += Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_RegularHour"]), 2);
                                        LegalHolidayOTHr += Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_OvertimeHour"]), 2);
                                        LegalHolidayNDHr += Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_RegularNightPremHour"]), 2);
                                        LegalHolidayOTNDHr += Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_OvertimeNightPremHour"]), 2);
                                    }
                                    else if (dtEmployeeLogLedger.Rows[i]["Ell_DayCode"].ToString().Equals("SPL") && !Convert.ToBoolean(dtEmployeeLogLedger.Rows[i]["Ell_RestDay"]))
                                    {
                                        SpecialHolidayHr += Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_RegularHour"]), 2);
                                        SpecialHolidayOTHr += Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_OvertimeHour"]), 2);
                                        SpecialHolidayNDHr += Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_RegularNightPremHour"]), 2);
                                        SpecialHolidayOTNDHr += Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_OvertimeNightPremHour"]), 2);
                                    }
                                    else if (dtEmployeeLogLedger.Rows[i]["Ell_DayCode"].ToString().Equals("PSD") && !Convert.ToBoolean(dtEmployeeLogLedger.Rows[i]["Ell_RestDay"]))
                                    {
                                        PlantShutdownHr += Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_RegularHour"]), 2);
                                        PlantShutdownOTHr += Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_OvertimeHour"]), 2);
                                        PlantShutdownNDHr += Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_RegularNightPremHour"]), 2);
                                        PlantShutdownOTNDHr += Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_OvertimeNightPremHour"]), 2);
                                    }
                                    else if (dtEmployeeLogLedger.Rows[i]["Ell_DayCode"].ToString().Equals("COMP") && !Convert.ToBoolean(dtEmployeeLogLedger.Rows[i]["Ell_RestDay"]))
                                    {
                                        CompanyHolidayHr += Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_RegularHour"]), 2);
                                        CompanyHolidayOTHr += Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_OvertimeHour"]), 2);
                                        CompanyHolidayNDHr += Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_RegularNightPremHour"]), 2);
                                        CompanyHolidayOTNDHr += Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_OvertimeNightPremHour"]), 2);
                                    }
                                    else if (dtEmployeeLogLedger.Rows[i]["Ell_DayCode"].ToString().Equals("HOL") && Convert.ToBoolean(dtEmployeeLogLedger.Rows[i]["Ell_RestDay"]))
                                    {
                                        RestdayLegalHolidayHr += Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_RegularHour"]), 2);
                                        RestdayLegalHolidayOTHr += Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_OvertimeHour"]), 2);
                                        RestdayLegalHolidayNDHr += Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_RegularNightPremHour"]), 2);
                                        RestdayLegalHolidayOTNDHr += Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_OvertimeNightPremHour"]), 2);
                                    }
                                    else if (dtEmployeeLogLedger.Rows[i]["Ell_DayCode"].ToString().Equals("SPL") && Convert.ToBoolean(dtEmployeeLogLedger.Rows[i]["Ell_RestDay"]))
                                    {
                                        RestdaySpecialHolidayHr += Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_RegularHour"]), 2);
                                        RestdaySpecialHolidayOTHr += Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_OvertimeHour"]), 2);
                                        RestdaySpecialHolidayNDHr += Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_RegularNightPremHour"]), 2);
                                        RestdaySpecialHolidayOTNDHr += Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_OvertimeNightPremHour"]), 2);
                                    }
                                    else if (dtEmployeeLogLedger.Rows[i]["Ell_DayCode"].ToString().Equals("COMP") && Convert.ToBoolean(dtEmployeeLogLedger.Rows[i]["Ell_RestDay"]))
                                    {
                                        RestdayCompanyHolidayHr += Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_RegularHour"]), 2);
                                        RestdayCompanyHolidayOTHr += Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_OvertimeHour"]), 2);
                                        RestdayCompanyHolidayNDHr += Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_RegularNightPremHour"]), 2);
                                        RestdayCompanyHolidayOTNDHr += Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_OvertimeNightPremHour"]), 2);
                                    }
                                    else if (dtEmployeeLogLedger.Rows[i]["Ell_DayCode"].ToString().Equals("PSD") && Convert.ToBoolean(dtEmployeeLogLedger.Rows[i]["Ell_RestDay"]))
                                    {
                                        RestdayPlantShutdownHr += Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_RegularHour"]), 2);
                                        RestdayPlantShutdownOTHr += Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_OvertimeHour"]), 2);
                                        RestdayPlantShutdownNDHr += Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_RegularNightPremHour"]), 2);
                                        RestdayPlantShutdownOTNDHr += Math.Round(Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_OvertimeNightPremHour"]), 2);
                                    }
                                    else if (bHasDayCodeExt)
                                    {
                                        foreach (DataRow drFiller in dtDayCodeFillers.Rows)
                                        {
                                            fillerHrCol = string.Format("Ept_Filler{0}_Hr", drFiller["Dcf_FillerSeq"]);
                                            fillerOTHrCol = string.Format("Ept_Filler{0}_OTHr", drFiller["Dcf_FillerSeq"]);
                                            fillerNDHrCol = string.Format("Ept_Filler{0}_NDHr", drFiller["Dcf_FillerSeq"]);
                                            fillerOTNDHrCol = string.Format("Ept_Filler{0}_OTNDHr", drFiller["Dcf_FillerSeq"]);
                                            if (dtEmployeeLogLedger.Rows[i]["Ell_DayCode"].ToString().Equals(drFiller["Dcf_DayCode"].ToString()) && Convert.ToBoolean(drFiller["Dcf_Restday"]) == Convert.ToBoolean(dtEmployeeLogLedger.Rows[i]["Ell_RestDay"]))
                                            {
                                                drEmployeePayrollTransactionExt[fillerHrCol] = Math.Round(Convert.ToDouble(drEmployeePayrollTransactionExt[fillerHrCol]) + Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_RegularHour"]), 2);
                                                drEmployeePayrollTransactionExt[fillerOTHrCol] = Math.Round(Convert.ToDouble(drEmployeePayrollTransactionExt[fillerOTHrCol]) + Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_OvertimeHour"]), 2);
                                                drEmployeePayrollTransactionExt[fillerNDHrCol] = Math.Round(Convert.ToDouble(drEmployeePayrollTransactionExt[fillerNDHrCol]) + Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_RegularNightPremHour"]), 2);
                                                drEmployeePayrollTransactionExt[fillerOTNDHrCol] = Math.Round(Convert.ToDouble(drEmployeePayrollTransactionExt[fillerOTNDHrCol]) + Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_OvertimeNightPremHour"]), 2);
                                            }
                                        }
                                    }
                                    #endregion
                                }
                                else //Hour Fraction Computation
                                {
                                    #region Payroll Transaction Detail variables
                                    SavePayrollTransactionAmounts(curEmployeeID
                                                                    , Convert.ToDateTime(dtEmployeeLogLedger.Rows[i]["Ell_ProcessDate"])
                                                                    , dtEmployeeLogLedger.Rows[i]["Ell_DayCode"].ToString()
                                                                    , Convert.ToBoolean(dtEmployeeLogLedger.Rows[i]["Ell_RestDay"]));

                                    #region Minimum OT Checking
                                    if (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RegularOTHr"]) < MINOTHR)
                                        drEmployeePayrollTransactionDetail["Ept_RegularOTHr"] = 0;
                                    if (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayOTHr"]) < MINOTHR)
                                    {
                                        drEmployeePayrollTransactionDetail["Ept_RestdayHr"] = 0;
                                        drEmployeePayrollTransactionDetail["Ept_RestdayOTHr"] = 0;
                                    }
                                    if (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_LegalHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_LegalHolidayOTHr"]) < MINOTHR)
                                    {
                                        drEmployeePayrollTransactionDetail["Ept_LegalHolidayHr"] = 0;
                                        drEmployeePayrollTransactionDetail["Ept_LegalHolidayOTHr"] = 0;
                                    }
                                    if (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_SpecialHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_SpecialHolidayOTHr"]) < MINOTHR)
                                    {
                                        drEmployeePayrollTransactionDetail["Ept_SpecialHolidayHr"] = 0;
                                        drEmployeePayrollTransactionDetail["Ept_SpecialHolidayOTHr"] = 0;
                                    }
                                    if (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_PlantShutdownHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_PlantShutdownOTHr"]) < MINOTHR)
                                    {
                                        drEmployeePayrollTransactionDetail["Ept_PlantShutdownHr"] = 0;
                                        drEmployeePayrollTransactionDetail["Ept_PlantShutdownOTHr"] = 0;
                                    }
                                    if (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_CompanyHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_CompanyHolidayOTHr"]) < MINOTHR)
                                    {
                                        drEmployeePayrollTransactionDetail["Ept_CompanyHolidayHr"] = 0;
                                        drEmployeePayrollTransactionDetail["Ept_CompanyHolidayOTHr"] = 0;
                                    }
                                    if (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayOTHr"]) < MINOTHR)
                                    {
                                        drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayHr"] = 0;
                                        drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayOTHr"] = 0;
                                    }
                                    if (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayOTHr"]) < MINOTHR)
                                    {
                                        drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayHr"] = 0;
                                        drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayOTHr"] = 0;
                                    }
                                    if (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayOTHr"]) < MINOTHR)
                                    {
                                        drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayHr"] = 0;
                                        drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayOTHr"] = 0;
                                    }
                                    if (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownOTHr"]) < MINOTHR)
                                    {
                                        drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownHr"] = 0;
                                        drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownOTHr"] = 0;
                                    }
                                    if (bHasDayCodeExt)
                                    {
                                        foreach (DataRow drFiller in dtDayCodeFillers.Rows)
                                        {
                                            fillerHrCol = string.Format("Ept_Filler{0}_Hr", drFiller["Dcf_FillerSeq"]);
                                            fillerOTHrCol = string.Format("Ept_Filler{0}_OTHr", drFiller["Dcf_FillerSeq"]);
                                            if (Convert.ToDouble(drEmployeePayrollTransactionExtDetail[fillerHrCol]) + Convert.ToDouble(drEmployeePayrollTransactionExtDetail[fillerOTHrCol]) < MINOTHR)
                                            {
                                                drEmployeePayrollTransactionExt[fillerHrCol] = Convert.ToDouble(drEmployeePayrollTransactionExt[fillerHrCol]) - Convert.ToDouble(drEmployeePayrollTransactionExtDetail[fillerHrCol]);
                                                drEmployeePayrollTransactionExt[fillerOTHrCol] = Convert.ToDouble(drEmployeePayrollTransactionExt[fillerOTHrCol]) - Convert.ToDouble(drEmployeePayrollTransactionExtDetail[fillerOTHrCol]);
                                                drEmployeePayrollTransactionExtDetail[fillerHrCol] = 0;
                                                drEmployeePayrollTransactionExtDetail[fillerOTHrCol] = 0;
                                            }
                                        }
                                    }
                                    #endregion

                                    #region Add to Total Hours
                                    if (HOURFRACFORMULA2 == "1"
                                        || (HOURFRACFORMULA1 == "1" && bHasAddedCurrentHrs == false)
                                        || (HOURFRACFORMULA1 == "0" && HOURFRACFORMULA2 == "0"))
                                    {
                                        RegularHr += Math.Round(Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RegularHr"]), 2);
                                        RegularOTHr += Math.Round(Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RegularOTHr"]), 2);
                                        RestdayHr += Math.Round(Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayHr"]), 2);
                                        RestdayOTHr += Math.Round(Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayOTHr"]), 2);
                                        LegalHolidayHr += Math.Round(Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_LegalHolidayHr"]), 2);
                                        LegalHolidayOTHr += Math.Round(Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_LegalHolidayOTHr"]), 2);
                                        SpecialHolidayHr += Math.Round(Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_SpecialHolidayHr"]), 2);
                                        SpecialHolidayOTHr += Math.Round(Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_SpecialHolidayOTHr"]), 2);
                                        PlantShutdownHr += Math.Round(Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_PlantShutdownHr"]), 2);
                                        PlantShutdownOTHr += Math.Round(Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_PlantShutdownOTHr"]), 2);
                                        CompanyHolidayHr += Math.Round(Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_CompanyHolidayHr"]), 2);
                                        CompanyHolidayOTHr += Math.Round(Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_CompanyHolidayOTHr"]), 2);
                                        RestdayLegalHolidayHr += Math.Round(Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayHr"]), 2);
                                        RestdayLegalHolidayOTHr += Math.Round(Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayOTHr"]), 2);
                                        RestdaySpecialHolidayHr += Math.Round(Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayHr"]), 2);
                                        RestdaySpecialHolidayOTHr += Math.Round(Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayOTHr"]), 2);
                                        RestdayCompanyHolidayHr += Math.Round(Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayHr"]), 2);
                                        RestdayCompanyHolidayOTHr += Math.Round(Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayOTHr"]), 2);
                                        RestdayPlantShutdownHr += Math.Round(Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownHr"]), 2);
                                        RestdayPlantShutdownOTHr += Math.Round(Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownOTHr"]), 2);
                                    }

                                    RegularNDHr += Math.Round(Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RegularNDHr"]), 2);
                                    RegularOTNDHr += Math.Round(Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RegularOTNDHr"]), 2);
                                    RestdayNDHr += Math.Round(Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayNDHr"]), 2);
                                    RestdayOTNDHr += Math.Round(Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayOTNDHr"]), 2);
                                    LegalHolidayNDHr += Math.Round(Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_LegalHolidayNDHr"]), 2);
                                    LegalHolidayOTNDHr += Math.Round(Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_LegalHolidayOTNDHr"]), 2);
                                    SpecialHolidayNDHr += Math.Round(Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_SpecialHolidayNDHr"]), 2);
                                    SpecialHolidayOTNDHr += Math.Round(Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_SpecialHolidayOTNDHr"]), 2);
                                    PlantShutdownNDHr += Math.Round(Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_PlantShutdownNDHr"]), 2);
                                    PlantShutdownOTNDHr += Math.Round(Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_PlantShutdownOTNDHr"]), 2);
                                    CompanyHolidayNDHr += Math.Round(Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_CompanyHolidayNDHr"]), 2);
                                    CompanyHolidayOTNDHr += Math.Round(Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_CompanyHolidayOTNDHr"]), 2);
                                    RestdayLegalHolidayNDHr += Math.Round(Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayNDHr"]), 2);
                                    RestdayLegalHolidayOTNDHr += Math.Round(Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayOTNDHr"]), 2);
                                    RestdaySpecialHolidayNDHr += Math.Round(Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayNDHr"]), 2);
                                    RestdaySpecialHolidayOTNDHr += Math.Round(Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayOTNDHr"]), 2);
                                    RestdayCompanyHolidayNDHr += Math.Round(Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayNDHr"]), 2);
                                    RestdayCompanyHolidayOTNDHr += Math.Round(Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayOTNDHr"]), 2);
                                    RestdayPlantShutdownNDHr += Math.Round(Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownNDHr"]), 2);
                                    RestdayPlantShutdownOTNDHr += Math.Round(Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownOTNDHr"]), 2);

                                    bHasAddedCurrentHrs = false; //reset per date to FALSE
                                    #endregion

                                    //Regular Hours Requirement
                                    if (HasMetRegularHourRequirement(dtEmployeeLogLedger.Rows[i]["Emt_JobStatus"].ToString(), Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RegularHr"]), Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_LeaveHour"]), dtEmployeeLogLedger.Rows[i]["Ell_DayCode"].ToString()))
                                    {
                                        drEmployeePayrollTransactionDetail["Ept_RegularHr"] = (iShiftMin / 60.0) - Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_LeaveHour"]);
                                        drEmployeePayrollTransactionDetail["Ept_AbsentHr"] = 0;
                                    }
                                    #endregion
                                }
                            }

                            #region Copy to employee payroll transaction detail table
                            if (bUserGeneratedPayTrans == false || !ProcessCurrentPeriod) //allow save if past pay period
                            {
                                drEmployeePayrollTransactionDetail["Ept_EmployeeId"] = curEmployeeID;
                                drEmployeePayrollTransactionDetail["Ept_CurrentPayPeriod"] = ProcessPayrollPeriod;
                                drEmployeePayrollTransactionDetail["Ept_AbsentHr"] = Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_AbsentHour"]);
                                drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayCount"] = Convert.ToInt32(dtEmployeeLogLedger.Rows[i]["Ell_SundayHolidayCount"]);
                                drEmployeePayrollTransactionDetail["Ept_WorkingDay"] = Convert.ToInt32(dtEmployeeLogLedger.Rows[i]["Ell_WorkingDay"]);
                                drEmployeePayrollTransactionDetail["Ept_ProcessDate"] = dtEmployeeLogLedger.Rows[i]["Ell_ProcessDate"];

                                drEmployeePayrollTransactionDetail["Ept_LateHr"] = (Convert.ToInt32(dtEmployeeLogLedger.Rows[i]["Ell_ComputedLateMin"]) + Convert.ToInt32(dtEmployeeLogLedger.Rows[i]["Ell_ComputedLate2Min"])) / 60.0;
                                drEmployeePayrollTransactionDetail["Ept_UndertimeHr"] = (Convert.ToInt32(dtEmployeeLogLedger.Rows[i]["Ell_ComputedUndertimeMin"]) + Convert.ToInt32(dtEmployeeLogLedger.Rows[i]["Ell_ComputedUndertime2Min"])) / 60.0;
                                drEmployeePayrollTransactionDetail["Ept_PaidLeaveHr"] = Convert.ToInt32(dtEmployeeLogLedger.Rows[i]["Ell_PayLeaveMin"]) / 60.0;
                                drEmployeePayrollTransactionDetail["Ept_UnpaidLeaveHr"] = Convert.ToInt32(dtEmployeeLogLedger.Rows[i]["Ell_NoPayLeaveMin"]) / 60.0;
                                drEmployeePayrollTransactionDetail["Ept_WholeDayAbsentHr"] = Convert.ToInt32(dtEmployeeLogLedger.Rows[i]["Ell_InitialAbsentMin"]) / 60.0;

                                #region If Not Hour Fraction, Copy to Payroll Transaction Detail
                                if (!Convert.ToBoolean(HRFRCLBRHR))
                                {
                                    if ((dtEmployeeLogLedger.Rows[i]["Ell_DayCode"].ToString().Equals("REG")
                                            || dtEmployeeLogLedger.Rows[i]["Ell_DayCode"].ToString().Equals("REGN"))
                                        && !Convert.ToBoolean(dtEmployeeLogLedger.Rows[i]["Ell_RestDay"]))
                                    {
                                        drEmployeePayrollTransactionDetail["Ept_RegularHr"] = Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_RegularHour"]);
                                        drEmployeePayrollTransactionDetail["Ept_RegularOTHr"] = Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_OvertimeHour"]);
                                        drEmployeePayrollTransactionDetail["Ept_RegularNDHr"] = Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_RegularNightPremHour"]);
                                        drEmployeePayrollTransactionDetail["Ept_RegularOTNDHr"] = Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_OvertimeNightPremHour"]);
                                    }
                                    else if (dtEmployeeLogLedger.Rows[i]["Ell_DayCode"].ToString().Equals("REST") && Convert.ToBoolean(dtEmployeeLogLedger.Rows[i]["Ell_RestDay"]))
                                    {
                                        drEmployeePayrollTransactionDetail["Ept_RestdayHr"] = Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_RegularHour"]);
                                        drEmployeePayrollTransactionDetail["Ept_RestdayOTHr"] = Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_OvertimeHour"]);
                                        drEmployeePayrollTransactionDetail["Ept_RestdayNDHr"] = Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_RegularNightPremHour"]);
                                        drEmployeePayrollTransactionDetail["Ept_RestdayOTNDHr"] = Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_OvertimeNightPremHour"]);
                                    }
                                    else if (dtEmployeeLogLedger.Rows[i]["Ell_DayCode"].ToString().Equals("HOL") && !Convert.ToBoolean(dtEmployeeLogLedger.Rows[i]["Ell_RestDay"]))
                                    {
                                        drEmployeePayrollTransactionDetail["Ept_LegalHolidayHr"] = Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_RegularHour"]);
                                        drEmployeePayrollTransactionDetail["Ept_LegalHolidayOTHr"] = Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_OvertimeHour"]);
                                        drEmployeePayrollTransactionDetail["Ept_LegalHolidayNDHr"] = Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_RegularNightPremHour"]);
                                        drEmployeePayrollTransactionDetail["Ept_LegalHolidayOTNDHr"] = Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_OvertimeNightPremHour"]);
                                    }
                                    else if (dtEmployeeLogLedger.Rows[i]["Ell_DayCode"].ToString().Equals("SPL") && !Convert.ToBoolean(dtEmployeeLogLedger.Rows[i]["Ell_RestDay"]))
                                    {
                                        drEmployeePayrollTransactionDetail["Ept_SpecialHolidayHr"] = Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_RegularHour"]);
                                        drEmployeePayrollTransactionDetail["Ept_SpecialHolidayOTHr"] = Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_OvertimeHour"]);
                                        drEmployeePayrollTransactionDetail["Ept_SpecialHolidayNDHr"] = Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_RegularNightPremHour"]);
                                        drEmployeePayrollTransactionDetail["Ept_SpecialHolidayOTNDHr"] = Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_OvertimeNightPremHour"]);
                                    }
                                    else if (dtEmployeeLogLedger.Rows[i]["Ell_DayCode"].ToString().Equals("PSD") && !Convert.ToBoolean(dtEmployeeLogLedger.Rows[i]["Ell_RestDay"]))
                                    {
                                        drEmployeePayrollTransactionDetail["Ept_PlantShutdownHr"] = Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_RegularHour"]);
                                        drEmployeePayrollTransactionDetail["Ept_PlantShutdownOTHr"] = Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_OvertimeHour"]);
                                        drEmployeePayrollTransactionDetail["Ept_PlantShutdownNDHr"] = Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_RegularNightPremHour"]);
                                        drEmployeePayrollTransactionDetail["Ept_PlantShutdownOTNDHr"] = Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_OvertimeNightPremHour"]);
                                    }
                                    else if (dtEmployeeLogLedger.Rows[i]["Ell_DayCode"].ToString().Equals("COMP") && !Convert.ToBoolean(dtEmployeeLogLedger.Rows[i]["Ell_RestDay"]))
                                    {
                                        drEmployeePayrollTransactionDetail["Ept_CompanyHolidayHr"] = Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_RegularHour"]);
                                        drEmployeePayrollTransactionDetail["Ept_CompanyHolidayOTHr"] = Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_OvertimeHour"]);
                                        drEmployeePayrollTransactionDetail["Ept_CompanyHolidayNDHr"] = Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_RegularNightPremHour"]);
                                        drEmployeePayrollTransactionDetail["Ept_CompanyHolidayOTNDHr"] = Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_OvertimeNightPremHour"]);
                                    }
                                    else if (dtEmployeeLogLedger.Rows[i]["Ell_DayCode"].ToString().Equals("HOL") && Convert.ToBoolean(dtEmployeeLogLedger.Rows[i]["Ell_RestDay"]))
                                    {
                                        drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayHr"] = Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_RegularHour"]);
                                        drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayOTHr"] = Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_OvertimeHour"]);
                                        drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayNDHr"] = Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_RegularNightPremHour"]);
                                        drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayOTNDHr"] = Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_OvertimeNightPremHour"]);
                                    }
                                    else if (dtEmployeeLogLedger.Rows[i]["Ell_DayCode"].ToString().Equals("SPL") && Convert.ToBoolean(dtEmployeeLogLedger.Rows[i]["Ell_RestDay"]))
                                    {
                                        drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayHr"] = Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_RegularHour"]);
                                        drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayOTHr"] = Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_OvertimeHour"]);
                                        drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayNDHr"] = Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_RegularNightPremHour"]);
                                        drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayOTNDHr"] = Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_OvertimeNightPremHour"]);
                                    }
                                    else if (dtEmployeeLogLedger.Rows[i]["Ell_DayCode"].ToString().Equals("COMP") && Convert.ToBoolean(dtEmployeeLogLedger.Rows[i]["Ell_RestDay"]))
                                    {
                                        drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayHr"] = Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_RegularHour"]);
                                        drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayOTHr"] = Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_OvertimeHour"]);
                                        drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayNDHr"] = Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_RegularNightPremHour"]);
                                        drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayOTNDHr"] = Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_OvertimeNightPremHour"]);
                                    }
                                    else if (dtEmployeeLogLedger.Rows[i]["Ell_DayCode"].ToString().Equals("PSD") && Convert.ToBoolean(dtEmployeeLogLedger.Rows[i]["Ell_RestDay"]))
                                    {
                                        drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownHr"] = Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_RegularHour"]);
                                        drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownOTHr"] = Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_OvertimeHour"]);
                                        drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownNDHr"] = Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_RegularNightPremHour"]);
                                        drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownOTNDHr"] = Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_OvertimeNightPremHour"]);
                                    }
                                    else if (bHasDayCodeExt)
                                    {
                                        foreach (DataRow drFiller in dtDayCodeFillers.Rows)
                                        {
                                            fillerHrCol = string.Format("Ept_Filler{0}_Hr", drFiller["Dcf_FillerSeq"]);
                                            fillerOTHrCol = string.Format("Ept_Filler{0}_OTHr", drFiller["Dcf_FillerSeq"]);
                                            fillerNDHrCol = string.Format("Ept_Filler{0}_NDHr", drFiller["Dcf_FillerSeq"]);
                                            fillerOTNDHrCol = string.Format("Ept_Filler{0}_OTNDHr", drFiller["Dcf_FillerSeq"]);
                                            if (dtEmployeeLogLedger.Rows[i]["Ell_DayCode"].ToString().Equals(drFiller["Dcf_DayCode"].ToString()) && Convert.ToBoolean(drFiller["Dcf_Restday"]) == Convert.ToBoolean(dtEmployeeLogLedger.Rows[i]["Ell_RestDay"]))
                                            {
                                                drEmployeePayrollTransactionExtDetail[fillerHrCol] = Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_RegularHour"]);
                                                drEmployeePayrollTransactionExtDetail[fillerOTHrCol] = Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_OvertimeHour"]);
                                                drEmployeePayrollTransactionExtDetail[fillerNDHrCol] = Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_RegularNightPremHour"]);
                                                drEmployeePayrollTransactionExtDetail[fillerOTNDHrCol] = Convert.ToDouble(dtEmployeeLogLedger.Rows[i]["Ell_OvertimeNightPremHour"]);
                                            }
                                        }
                                    }
                                }
                                #endregion

                                #region Paid Holiday Hours
                                if (dtEmployeeLogLedger.Rows[i]["Ell_ForceLeave"].ToString() != "" && Convert.ToInt32(dtEmployeeLogLedger.Rows[i]["Ell_ForceLeave"]) != 0)
                                {
                                    if (dtEmployeeLogLedger.Rows[i]["Ell_DayCode"].ToString() == "HOL")
                                    {
                                        if (Convert.ToInt32(dtEmployeeLogLedger.Rows[i]["Ell_ForceLeave"]) > 0)
                                            drEmployeePayrollTransactionDetail["Ept_PaidLegalHolidayHr"] = Convert.ToInt32(dtEmployeeLogLedger.Rows[i]["Ell_ForceLeave"]);
                                        else if (Convert.ToInt32(dtEmployeeLogLedger.Rows[i]["Ell_ForceLeave"]) < 0)
                                            drEmployeePayrollTransactionDetail["Ept_AbsentLegalHolidayHr"] = Convert.ToInt32(dtEmployeeLogLedger.Rows[i]["Ell_ForceLeave"]) * -1;
                                    }
                                    else if (dtEmployeeLogLedger.Rows[i]["Ell_DayCode"].ToString() == "SPL")
                                    {
                                        if (Convert.ToInt32(dtEmployeeLogLedger.Rows[i]["Ell_ForceLeave"]) > 0)
                                            drEmployeePayrollTransactionDetail["Ept_PaidSpecialHolidayHr"] = Convert.ToInt32(dtEmployeeLogLedger.Rows[i]["Ell_ForceLeave"]);
                                        else if (Convert.ToInt32(dtEmployeeLogLedger.Rows[i]["Ell_ForceLeave"]) < 0)
                                            drEmployeePayrollTransactionDetail["Ept_AbsentSpecialHolidayHr"] = Convert.ToInt32(dtEmployeeLogLedger.Rows[i]["Ell_ForceLeave"]) * -1;
                                    }
                                    else if (dtEmployeeLogLedger.Rows[i]["Ell_DayCode"].ToString() == "COMP")
                                    {
                                        if (Convert.ToInt32(dtEmployeeLogLedger.Rows[i]["Ell_ForceLeave"]) > 0)
                                            drEmployeePayrollTransactionDetail["Ept_PaidCompanyHolidayHr"] = Convert.ToInt32(dtEmployeeLogLedger.Rows[i]["Ell_ForceLeave"]);
                                        else if (Convert.ToInt32(dtEmployeeLogLedger.Rows[i]["Ell_ForceLeave"]) < 0)
                                            drEmployeePayrollTransactionDetail["Ept_AbsentCompanyHolidayHr"] = Convert.ToInt32(dtEmployeeLogLedger.Rows[i]["Ell_ForceLeave"]) * -1;
                                    }
                                    else if (dtEmployeeLogLedger.Rows[i]["Ell_DayCode"].ToString() == "PSD")
                                    {
                                        if (Convert.ToInt32(dtEmployeeLogLedger.Rows[i]["Ell_ForceLeave"]) < 0)
                                            drEmployeePayrollTransactionDetail["Ept_AbsentPlantShutdownHr"] = Convert.ToInt32(dtEmployeeLogLedger.Rows[i]["Ell_ForceLeave"]) * -1;
                                    }
                                    else
                                    {
                                        drArrDayCodeFiller = dtDayCodeFillers.Select(string.Format("Dcf_DayCode = '{0}'", dtEmployeeLogLedger.Rows[i]["Ell_DayCode"].ToString()));
                                        if (drArrDayCodeFiller.Length > 0)
                                        {
                                            if (Convert.ToInt32(dtEmployeeLogLedger.Rows[i]["Ell_ForceLeave"]) > 0)
                                                drEmployeePayrollTransactionDetail["Ept_PaidFillerHolidayHr"] = Convert.ToInt32(dtEmployeeLogLedger.Rows[i]["Ell_ForceLeave"]);
                                            else if (Convert.ToInt32(dtEmployeeLogLedger.Rows[i]["Ell_ForceLeave"]) < 0)
                                                drEmployeePayrollTransactionDetail["Ept_AbsentFillerHolidayHr"] = Convert.ToInt32(dtEmployeeLogLedger.Rows[i]["Ell_ForceLeave"]) * -1;
                                        }
                                    }
                                }
                                #endregion

                                drEmployeePayrollTransactionDetail["Ept_SalaryRate"] = 0; //temp
                                drEmployeePayrollTransactionDetail["Ept_HourlyRate"] = 0; //temp
                                drEmployeePayrollTransactionDetail["Ept_SalaryType"] = "B"; //temp
                                drEmployeePayrollTransactionDetail["Ept_PayrollType"] = strPayType; //temp
                                drEmployeePayrollTransactionDetail["Usr_Login"] = LoginUser;
                                drEmployeePayrollTransactionDetail["Ludatetime"] = DateTime.Now;
                                if (bProcessTrail)
                                    drEmployeePayrollTransactionDetail["Ept_AdjustPayPeriod"] = AdjustPayrollPeriod;

                                if (bHasDayCodeExt)
                                {
                                    drEmployeePayrollTransactionExtDetail["Ept_EmployeeId"] = curEmployeeID;
                                    drEmployeePayrollTransactionExtDetail["Ept_CurrentPayPeriod"] = ProcessPayrollPeriod;
                                    drEmployeePayrollTransactionExtDetail["Usr_Login"] = LoginUser;
                                    drEmployeePayrollTransactionExtDetail["Ludatetime"] = DateTime.Now;
                                    drEmployeePayrollTransactionExtDetail["Ept_ProcessDate"] = dtEmployeeLogLedger.Rows[i]["Ell_ProcessDate"];
                                    drEmployeePayrollTransactionExtDetail["Ept_SalaryRate"] = 0; //temp
                                    drEmployeePayrollTransactionExtDetail["Ept_HourlyRate"] = 0; //temp
                                    drEmployeePayrollTransactionExtDetail["Ept_SalaryType"] = "B"; //temp
                                    drEmployeePayrollTransactionExtDetail["Ept_PayrollType"] = strPayType; //temp
                                    if (bProcessTrail)
                                        drEmployeePayrollTransactionExtDetail["Ept_AdjustPayPeriod"] = AdjustPayrollPeriod;
                                }

                                //copy to table
                                dtEmployeePayrollTransactionDetail.Rows.Add(drEmployeePayrollTransactionDetail);
                                if (bHasDayCodeExt)
                                    dtEmployeePayrollTransactionExtDetail.Rows.Add(drEmployeePayrollTransactionExtDetail);
                            }
                            #endregion

                            #region Initialize payroll trans detail row
                            if (strScheduleType == "G") //Hour Fraction feature: lookup to previous day only works for if the previous day's shift is graveyard
                                drEmployeePayrollTransactionDetailPrev = drEmployeePayrollTransactionDetail;
                            else
                                drEmployeePayrollTransactionDetailPrev = null;

                            drEmployeePayrollTransactionDetail = dtEmployeePayrollTransactionDetail.NewRow();
                            if (bHasDayCodeExt)
                                drEmployeePayrollTransactionExtDetail = dtEmployeePayrollTransactionExtDetail.NewRow();
                            #region Initialize payroll trans detail hours
                            drEmployeePayrollTransactionDetail["Ept_RegularHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_RegularOTHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_RegularNDHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_RegularOTNDHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_RestdayHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_RestdayOTHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_RestdayNDHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_RestdayOTNDHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_LegalHolidayHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_LegalHolidayOTHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_LegalHolidayNDHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_LegalHolidayOTNDHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_SpecialHolidayHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_SpecialHolidayOTHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_SpecialHolidayNDHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_SpecialHolidayOTNDHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_PlantShutdownHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_PlantShutdownOTHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_PlantShutdownNDHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_PlantShutdownOTNDHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_CompanyHolidayHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_CompanyHolidayOTHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_CompanyHolidayNDHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_CompanyHolidayOTNDHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayOTHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayNDHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayOTNDHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayOTHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayNDHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayOTNDHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayOTHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayNDHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayOTNDHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownOTHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownNDHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownOTNDHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_LateHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_UndertimeHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_UnpaidLeaveHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_WholeDayAbsentHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_AbsentLegalHolidayHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_AbsentSpecialHolidayHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_AbsentCompanyHolidayHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_AbsentPlantShutdownHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_AbsentFillerHolidayHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_PaidLeaveHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_PaidLegalHolidayHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_PaidSpecialHolidayHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_PaidCompanyHolidayHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_PaidFillerHolidayHr"] = 0;
                            drEmployeePayrollTransactionDetail["Ept_OvertimeAdjustmentAmt"] = 0;
                            #endregion
                            #region Initialize payroll trans detail ext hours
                            if (bHasDayCodeExt)
                            {
                                for (int n = 1; n <= FILLERCNT; n++)
                                {
                                    //initialize
                                    fillerHrCol = string.Format("Ept_Filler{0:00}_Hr", n);
                                    fillerOTHrCol = string.Format("Ept_Filler{0:00}_OTHr", n);
                                    fillerNDHrCol = string.Format("Ept_Filler{0:00}_NDHr", n);
                                    fillerOTNDHrCol = string.Format("Ept_Filler{0:00}_OTNDHr", n);
                                    drEmployeePayrollTransactionExtDetail[fillerHrCol] = 0;
                                    drEmployeePayrollTransactionExtDetail[fillerOTHrCol] = 0;
                                    drEmployeePayrollTransactionExtDetail[fillerNDHrCol] = 0;
                                    drEmployeePayrollTransactionExtDetail[fillerOTNDHrCol] = 0;
                                }
                            }
                            #endregion
                            #endregion
                        }
                        catch (System.Data.DataException ex)
                        {
                            CommonProcedures.logErrorToFile(ex.ToString());
                            string strError = string.Format("Wrong Log Ledger Setup");
                            EmpDispHandler(this, new EmpDispEventArgs(dtEmployeeLogLedger.Rows[i]["Ell_EmployeeId"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_LastName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_FirstName"].ToString(), strError));
                            AddErrorToLaborHourReport(dtEmployeeLogLedger.Rows[i]["Ell_EmployeeId"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_LastName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_FirstName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_MiddleName"].ToString(), dtEmployeeLogLedger.Rows[i]["Ell_ProcessDate"].ToString(), strError);
                        }
                        catch (Exception ex)
                        {
                            CommonProcedures.logErrorToFile(ex.ToString());
                            EmpDispHandler(this, new EmpDispEventArgs(dtEmployeeLogLedger.Rows[i]["Ell_EmployeeId"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_LastName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_FirstName"].ToString(), "Error in Labor Hours Generation : " + ex.Message));
                            AddErrorToLaborHourReport(dtEmployeeLogLedger.Rows[i]["Ell_EmployeeId"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_LastName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_FirstName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_MiddleName"].ToString(), dtEmployeeLogLedger.Rows[i]["Ell_ProcessDate"].ToString(), ex.Message.Substring(0, Math.Min(ex.Message.Length, 1000)));
                        }

                        //assign to prev
                        prevEmployeeID = curEmployeeID;
                    }
                }
                //-----------------------------END MAIN PROCESS
                //Save Log Ledger Table
                StatusHandler(this, new StatusEventArgs("Saving of Log Ledger Records", false));
                string strInsertIntoTemplate;
                string strUpdateRecordTemplate;
                string strUpdateQuery = "";

                string strAdjPayPeriod = "";
                string strAdjPayPeriodCol = "";
                string strAdjPayPeriodVal = "";
                if (bProcessTrail)
                {
                    strAdjPayPeriod = " AND Ell_AdjustPayPeriod = '" + AdjustPayrollPeriod + "' AND Ell_PayPeriod = '" + ProcessPayrollPeriod + "'";
                    strAdjPayPeriodCol = "Ept_AdjustPayPeriod,";
                    strAdjPayPeriodVal = string.Format("'{0}',", AdjustPayrollPeriod);
                }

                int iUpdateCtr;
                if (!bProcessTrail)
                {
                    iUpdateCtr = 0;
                    #region Log Ledger Update
                    strUpdateRecordTemplate = @"UPDATE {0} SET Ell_ConvertedTimeIn_1Min = {3}, Ell_ConvertedTimeOut_1Min = {4}, Ell_ConvertedTimeIn_2Min = {5}, Ell_ConvertedTimeOut_2Min = {6}, Ell_ComputedTimeIn_1Min = {7}, Ell_ComputedTimeOut_1Min = {8}, Ell_ComputedTimeIn_2Min = {9}, Ell_ComputedTimeOut_2Min = {10}, Ell_AdjustShiftMin = {11}, Ell_ShiftTimeIn_1Min = {12}, Ell_ShiftTimeOut_1Min = {13}, Ell_ShiftTimeIn_2Min = {14}, Ell_ShiftTimeOut_2Min = {15}, Ell_ComputedOvertimeMin = {16}, Ell_ComputedLateMin = {17}, Ell_InitialAbsentMin = {18}, Ell_ComputedAbsentMin = {19}, Ell_ComputedRegularMin = {20}, Ell_ComputedDayWorkMin = {21}, Ell_ComputedRegularNightPremMin = {22}, Ell_ComputedOvertimeNightPremMin = {23}, Ell_PreviousDayWorkMin = {24}, Ell_PreviousDayHolidayReference = {25}, Ell_SundayHolidayCount = {26}, Ell_WorkingDay = {27}, Ell_ExpectedHour = {28}, Ell_AbsentHour = {29}, Ell_RegularHour = {30}, Ell_OvertimeHour = {31}, Ell_RegularNightPremHour = {32}, Ell_OvertimeNightPremHour = {33}, Ell_LeaveHour = {34}, Ell_ShiftMin = {35}, Ell_ScheduleType = '{36}', Ell_PayLeaveMin = {37}, Ell_NoPayLeaveMin = {38}, Ell_EncodedOvertimeMin = {39}, Ell_OffsetOvertimeMin = {40}, Ell_ExcessLeaveMin = {41}, Ell_ExcessOffset = {42}, Ell_ForOffsetMin = {43}, Usr_Login = '{55}', Ludatetime = getdate(), Ell_ComputedLate2Min = {44}, Ell_ComputedUndertimeMin = {45}, Ell_ComputedUndertime2Min = {46}, Ell_ForceLeave = {48}, Ell_EncodedPayLeaveType = '{49}', Ell_EncodedPayLeaveHr = {50}, Ell_EncodedNoPayLeaveType = '{51}', Ell_EncodedNoPayLeaveHr = {52}, Ell_EncodedOvertimeAdvHr = {53}, Ell_EncodedOvertimePostHr = {54} WHERE Ell_EmployeeId = '{1}' AND Ell_ProcessDate = '{2}' {47} ";
                    #endregion
                    foreach (DataRow drLogLedger in dtEmployeeLogLedger.Rows)
                    {
                        #region Log Ledger Save
                        strUpdateQuery += string.Format(strUpdateRecordTemplate
                                                        , EmployeeLogLedgerTable                        //0
                                                        , drLogLedger["Ell_EmployeeId"]
                                                        , drLogLedger["Ell_ProcessDate"]
                                                        , drLogLedger["Ell_ConvertedTimeIn_1Min"]
                                                        , drLogLedger["Ell_ConvertedTimeOut_1Min"]
                                                        , drLogLedger["Ell_ConvertedTimeIn_2Min"]
                                                        , drLogLedger["Ell_ConvertedTimeOut_2Min"]
                                                        , drLogLedger["Ell_ComputedTimeIn_1Min"]
                                                        , drLogLedger["Ell_ComputedTimeOut_1Min"]
                                                        , drLogLedger["Ell_ComputedTimeIn_2Min"]
                                                        , drLogLedger["Ell_ComputedTimeOut_2Min"]       //10
                                                        , drLogLedger["Ell_AdjustShiftMin"]
                                                        , drLogLedger["Ell_ShiftTimeIn_1Min"]
                                                        , drLogLedger["Ell_ShiftTimeOut_1Min"]
                                                        , drLogLedger["Ell_ShiftTimeIn_2Min"]
                                                        , drLogLedger["Ell_ShiftTimeOut_2Min"]
                                                        , drLogLedger["Ell_ComputedOvertimeMin"]
                                                        , drLogLedger["Ell_ComputedLateMin"]
                                                        , drLogLedger["Ell_InitialAbsentMin"]
                                                        , drLogLedger["Ell_ComputedAbsentMin"]
                                                        , drLogLedger["Ell_ComputedRegularMin"]         //20
                                                        , drLogLedger["Ell_ComputedDayWorkMin"]
                                                        , drLogLedger["Ell_ComputedRegularNightPremMin"]
                                                        , drLogLedger["Ell_ComputedOvertimeNightPremMin"]
                                                        , drLogLedger["Ell_PreviousDayWorkMin"]
                                                        , (drLogLedger["Ell_PreviousDayHolidayReference"] == DBNull.Value) ? "null" : "'" + drLogLedger["Ell_PreviousDayHolidayReference"].ToString() + "'"
                                                        , drLogLedger["Ell_SundayHolidayCount"]
                                                        , drLogLedger["Ell_WorkingDay"]
                                                        , drLogLedger["Ell_ExpectedHour"]
                                                        , drLogLedger["Ell_AbsentHour"]
                                                        , drLogLedger["Ell_RegularHour"]                //30
                                                        , drLogLedger["Ell_OvertimeHour"]
                                                        , drLogLedger["Ell_RegularNightPremHour"]
                                                        , drLogLedger["Ell_OvertimeNightPremHour"]
                                                        , drLogLedger["Ell_LeaveHour"]
                                                        , drLogLedger["Ell_ShiftMin"]
                                                        , drLogLedger["Ell_ScheduleType"]
                                                        , drLogLedger["Ell_PayLeaveMin"]
                                                        , drLogLedger["Ell_NoPayLeaveMin"]
                                                        , drLogLedger["Ell_EncodedOvertimeMin"]
                                                        , drLogLedger["Ell_OffsetOvertimeMin"]              //40
                                                        , drLogLedger["Ell_ExcessLeaveMin"]
                                                        , drLogLedger["Ell_ExcessOffset"]
                                                        , drLogLedger["Ell_ForOffsetMin"]
                                                        , drLogLedger["Ell_ComputedLate2Min"]
                                                        , drLogLedger["Ell_ComputedUndertimeMin"]
                                                        , drLogLedger["Ell_ComputedUndertime2Min"]
                                                        , strAdjPayPeriod
                                                        , drLogLedger["Ell_ForceLeave"]
                                                        , drLogLedger["Ell_EncodedPayLeaveType"]
                                                        , drLogLedger["Ell_EncodedPayLeaveHr"]              //50
                                                        , drLogLedger["Ell_EncodedNoPayLeaveType"]
                                                        , drLogLedger["Ell_EncodedNoPayLeaveHr"]
                                                        , drLogLedger["Ell_EncodedOvertimeAdvHr"]
                                                        , drLogLedger["Ell_EncodedOvertimePostHr"]
                                                        , LoginUser);
                        #endregion
                        iUpdateCtr++;
                        if (iUpdateCtr == 150) //approx 10 employees
                        {
                            dal.ExecuteNonQuery(strUpdateQuery);
                            strUpdateQuery = "";
                            iUpdateCtr = 0;
                        }
                    }
                    if (strUpdateQuery != "")
                        dal.ExecuteNonQuery(strUpdateQuery);
                }
                StatusHandler(this, new StatusEventArgs("Saving of Log Ledger Records", true));
                //-----------------------------
                //Save Payroll Transaction Table
                StatusHandler(this, new StatusEventArgs("Saving of Payroll Transaction Records", false));
                strUpdateQuery = "";
                iUpdateCtr = 0;
                #region Payroll Transaction Update
                strInsertIntoTemplate = string.Format(@" INSERT INTO {0} (Ept_EmployeeId,Ept_CurrentPayPeriod,{1}Ept_AbsentHr,Ept_RegularHr,Ept_RegularOTHr,Ept_RegularNDHr,Ept_RegularOTNDHr,Ept_RestdayHr,Ept_RestdayOTHr,Ept_RestdayNDHr,Ept_RestdayOTNDHr,Ept_LegalHolidayHr,Ept_LegalHolidayOTHr,Ept_LegalHolidayNDHr,Ept_LegalHolidayOTNDHr,Ept_SpecialHolidayHr,Ept_SpecialHolidayOTHr,Ept_SpecialHolidayNDHr,Ept_SpecialHolidayOTNDHr,Ept_PlantShutdownHr,Ept_PlantShutdownOTHr,Ept_PlantShutdownNDHr,Ept_PlantShutdownOTNDHr,Ept_CompanyHolidayHr,Ept_CompanyHolidayOTHr,Ept_CompanyHolidayNDHr,Ept_CompanyHolidayOTNDHr,Ept_RestdayLegalHolidayHr,Ept_RestdayLegalHolidayOTHr,Ept_RestdayLegalHolidayNDHr,Ept_RestdayLegalHolidayOTNDHr,Ept_RestdaySpecialHolidayHr,Ept_RestdaySpecialHolidayOTHr,Ept_RestdaySpecialHolidayNDHr,Ept_RestdaySpecialHolidayOTNDHr,Ept_RestdayCompanyHolidayHr,Ept_RestdayCompanyHolidayOTHr,Ept_RestdayCompanyHolidayNDHr,Ept_RestdayCompanyHolidayOTNDHr,Ept_LaborHrsAdjustmentAmt,Ept_TaxAdjustmentAmt,Ept_NonTaxAdjustmentAmt,Ept_TaxAllowanceAmt,Ept_NonTaxAllowanceAmt,Ept_RestdayLegalHolidayCount,Ept_WorkingDay,Ept_PayrollType,Ept_RestdayPlantShutdownHr,Ept_RestdayPlantShutdownOTHr,Ept_RestdayPlantShutdownNDHr,Ept_RestdayPlantShutdownOTNDHr,Usr_Login,Ludatetime,Ept_LateHr,Ept_UndertimeHr,Ept_UnpaidLeaveHr,Ept_AbsentLegalHolidayHr,Ept_AbsentSpecialHolidayHr,Ept_AbsentCompanyHolidayHr,Ept_AbsentPlantShutdownHr,Ept_AbsentFillerHolidayHr,Ept_PaidLeaveHr,Ept_PaidLegalHolidayHr,Ept_PaidSpecialHolidayHr,Ept_PaidCompanyHolidayHr,Ept_PaidFillerHolidayHr,Ept_OvertimeAdjustmentAmt,Ept_WholeDayAbsentHr,Ept_UserGenerated) ", EmployeePayrollTransactionTable, strAdjPayPeriodCol);
                strUpdateRecordTemplate = @"{0}{48} SELECT '{1}','{2}',{49}{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34},{35},{36},{37},{38},{39},'0.00','0.00','0.00','0.00','0.00',{40},{41},'{42}',{43},{44},{45},{46},'{47}',GetDate(),{50},{51},{52},{53},{54},{55},{56},{57},{58},{59},{60},{61},{62},{63},{64},'{65}' UNION ALL ";

                //strUpdateRecordTemplate = @" INSERT INTO {0} (Ept_EmployeeId,Ept_CurrentPayPeriod,{48}Ept_AbsentHr,Ept_RegularHr,Ept_RegularOTHr,Ept_RegularNDHr,Ept_RegularOTNDHr,Ept_RestdayHr,Ept_RestdayOTHr,Ept_RestdayNDHr,Ept_RestdayOTNDHr,Ept_LegalHolidayHr,Ept_LegalHolidayOTHr,Ept_LegalHolidayNDHr,Ept_LegalHolidayOTNDHr,Ept_SpecialHolidayHr,Ept_SpecialHolidayOTHr,Ept_SpecialHolidayNDHr,Ept_SpecialHolidayOTNDHr,Ept_PlantShutdownHr,Ept_PlantShutdownOTHr,Ept_PlantShutdownNDHr,Ept_PlantShutdownOTNDHr,Ept_CompanyHolidayHr,Ept_CompanyHolidayOTHr,Ept_CompanyHolidayNDHr,Ept_CompanyHolidayOTNDHr,Ept_RestdayLegalHolidayHr,Ept_RestdayLegalHolidayOTHr,Ept_RestdayLegalHolidayNDHr,Ept_RestdayLegalHolidayOTNDHr,Ept_RestdaySpecialHolidayHr,Ept_RestdaySpecialHolidayOTHr,Ept_RestdaySpecialHolidayNDHr,Ept_RestdaySpecialHolidayOTNDHr,Ept_RestdayCompanyHolidayHr,Ept_RestdayCompanyHolidayOTHr,Ept_RestdayCompanyHolidayNDHr,Ept_RestdayCompanyHolidayOTNDHr,Ept_LaborHrsAdjustmentAmt,Ept_TaxAdjustmentAmt,Ept_NonTaxAdjustmentAmt,Ept_TaxAllowanceAmt,Ept_NonTaxAllowanceAmt,Ept_RestdayLegalHolidayCount,Ept_WorkingDay,Ept_PayrollType,Ept_RestdayPlantShutdownHr,Ept_RestdayPlantShutdownOTHr,Ept_RestdayPlantShutdownNDHr,Ept_RestdayPlantShutdownOTNDHr,Usr_Login,Ludatetime,Ept_LateHr,Ept_UndertimeHr,Ept_UnpaidLeaveHr,Ept_AbsentLegalHolidayHr,Ept_AbsentSpecialHolidayHr,Ept_AbsentCompanyHolidayHr,Ept_AbsentPlantShutdownHr,Ept_AbsentFillerHolidayHr,Ept_PaidLeaveHr,Ept_PaidLegalHolidayHr,Ept_PaidSpecialHolidayHr,Ept_PaidCompanyHolidayHr,Ept_PaidFillerHolidayHr,Ept_OvertimeAdjustmentAmt,Ept_WholeDayAbsentHr,Ept_UserGenerated) VALUES('{1}','{2}',{49}{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34},{35},{36},{37},{38},{39},'0.00','0.00','0.00','0.00','0.00',{40},{41},'{42}',{43},{44},{45},{46},'{47}',GetDate(),{50},{51},{52},{53},{54},{55},{56},{57},{58},{59},{60},{61},{62},{63},{64},'{65}') ";
                #endregion
                foreach (DataRow drEmployeePayrollTrans in dtEmployeePayrollTransaction.Rows)
                {
                    #region Payroll Transaction Save
                    strUpdateQuery += string.Format(strUpdateRecordTemplate
                                                        , "" //EmployeePayrollTransactionTable                                                       //0
                                                        , drEmployeePayrollTrans["Ept_EmployeeId"]
                                                        , drEmployeePayrollTrans["Ept_CurrentPayPeriod"]
                                                        , drEmployeePayrollTrans["Ept_AbsentHr"]
                                                        , drEmployeePayrollTrans["Ept_RegularHr"]
                                                        , drEmployeePayrollTrans["Ept_RegularOTHr"]
                                                        , drEmployeePayrollTrans["Ept_RegularNDHr"]
                                                        , drEmployeePayrollTrans["Ept_RegularOTNDHr"]
                                                        , drEmployeePayrollTrans["Ept_RestdayHr"]
                                                        , drEmployeePayrollTrans["Ept_RestdayOTHr"]
                                                        , drEmployeePayrollTrans["Ept_RestdayNDHr"]                                           //10
                                                        , drEmployeePayrollTrans["Ept_RestdayOTNDHr"]
                                                        , drEmployeePayrollTrans["Ept_LegalHolidayHr"]
                                                        , drEmployeePayrollTrans["Ept_LegalHolidayOTHr"]
                                                        , drEmployeePayrollTrans["Ept_LegalHolidayNDHr"]
                                                        , drEmployeePayrollTrans["Ept_LegalHolidayOTNDHr"]
                                                        , drEmployeePayrollTrans["Ept_SpecialHolidayHr"]
                                                        , drEmployeePayrollTrans["Ept_SpecialHolidayOTHr"]
                                                        , drEmployeePayrollTrans["Ept_SpecialHolidayNDHr"]
                                                        , drEmployeePayrollTrans["Ept_SpecialHolidayOTNDHr"]
                                                        , drEmployeePayrollTrans["Ept_PlantShutdownHr"]                                       //20
                                                        , drEmployeePayrollTrans["Ept_PlantShutdownOTHr"]
                                                        , drEmployeePayrollTrans["Ept_PlantShutdownNDHr"]
                                                        , drEmployeePayrollTrans["Ept_PlantShutdownOTNDHr"]
                                                        , drEmployeePayrollTrans["Ept_CompanyHolidayHr"]
                                                        , drEmployeePayrollTrans["Ept_CompanyHolidayOTHr"]
                                                        , drEmployeePayrollTrans["Ept_CompanyHolidayNDHr"]
                                                        , drEmployeePayrollTrans["Ept_CompanyHolidayOTNDHr"]
                                                        , drEmployeePayrollTrans["Ept_RestdayLegalHolidayHr"]
                                                        , drEmployeePayrollTrans["Ept_RestdayLegalHolidayOTHr"]
                                                        , drEmployeePayrollTrans["Ept_RestdayLegalHolidayNDHr"]                               //30
                                                        , drEmployeePayrollTrans["Ept_RestdayLegalHolidayOTNDHr"]
                                                        , drEmployeePayrollTrans["Ept_RestdaySpecialHolidayHr"]
                                                        , drEmployeePayrollTrans["Ept_RestdaySpecialHolidayOTHr"]
                                                        , drEmployeePayrollTrans["Ept_RestdaySpecialHolidayNDHr"]
                                                        , drEmployeePayrollTrans["Ept_RestdaySpecialHolidayOTNDHr"]
                                                        , drEmployeePayrollTrans["Ept_RestdayCompanyHolidayHr"]
                                                        , drEmployeePayrollTrans["Ept_RestdayCompanyHolidayOTHr"]
                                                        , drEmployeePayrollTrans["Ept_RestdayCompanyHolidayNDHr"]
                                                        , drEmployeePayrollTrans["Ept_RestdayCompanyHolidayOTNDHr"]
                                                        , drEmployeePayrollTrans["Ept_RestdayLegalHolidayCount"]                              //40
                                                        , drEmployeePayrollTrans["Ept_WorkingDay"]
                                                        , drEmployeePayrollTrans["Ept_PayrollType"]
                                                        , drEmployeePayrollTrans["Ept_RestdayPlantShutdownHr"]
                                                        , drEmployeePayrollTrans["Ept_RestdayPlantShutdownOTHr"]
                                                        , drEmployeePayrollTrans["Ept_RestdayPlantShutdownNDHr"]
                                                        , drEmployeePayrollTrans["Ept_RestdayPlantShutdownOTNDHr"]
                                                        , drEmployeePayrollTrans["Usr_Login"]
                                                        , "" //strAdjPayPeriodCol
                                                        , strAdjPayPeriodVal
                                                        , drEmployeePayrollTrans["Ept_LateHr"]                                                //50
                                                        , drEmployeePayrollTrans["Ept_UndertimeHr"]
                                                        , drEmployeePayrollTrans["Ept_UnpaidLeaveHr"]
                                                        , drEmployeePayrollTrans["Ept_AbsentLegalHolidayHr"]
                                                        , drEmployeePayrollTrans["Ept_AbsentSpecialHolidayHr"]
                                                        , drEmployeePayrollTrans["Ept_AbsentCompanyHolidayHr"]
                                                        , drEmployeePayrollTrans["Ept_AbsentPlantShutdownHr"]
                                                        , drEmployeePayrollTrans["Ept_AbsentFillerHolidayHr"]
                                                        , drEmployeePayrollTrans["Ept_PaidLeaveHr"]
                                                        , drEmployeePayrollTrans["Ept_PaidLegalHolidayHr"]
                                                        , drEmployeePayrollTrans["Ept_PaidSpecialHolidayHr"]                                   //60
                                                        , drEmployeePayrollTrans["Ept_PaidCompanyHolidayHr"]
                                                        , drEmployeePayrollTrans["Ept_PaidFillerHolidayHr"]
                                                        , drEmployeePayrollTrans["Ept_OvertimeAdjustmentAmt"]
                                                        , drEmployeePayrollTrans["Ept_WholeDayAbsentHr"]
                                                        , drEmployeePayrollTrans["Ept_UserGenerated"]);
                    #endregion
                    iUpdateCtr++;
                    if (iUpdateCtr == 50)
                    {
                        if (strUpdateQuery != "")
                            strUpdateQuery = strUpdateQuery.Substring(0, strUpdateQuery.Length - 10);

                        dal.ExecuteNonQuery(strInsertIntoTemplate + strUpdateQuery);
                        strUpdateQuery = "";
                        iUpdateCtr = 0;

                    }
                }
                if (strUpdateQuery != "")
                {
                    strUpdateQuery = strUpdateQuery.Substring(0, strUpdateQuery.Length - 10);
                    dal.ExecuteNonQuery(strInsertIntoTemplate + strUpdateQuery);
                }
                //-----------------------------
                //Save Payroll Transaction Ext Table
                if (bHasDayCodeExt)
                {
                    strUpdateQuery = "";
                    iUpdateCtr = 0;
                    #region Payroll Transaction Ext Update
                    strInsertIntoTemplate = string.Format(@" INSERT INTO {0} (Ept_EmployeeId,Ept_CurrentPayPeriod,{1}Ept_FIller01_Hr,Ept_FIller01_OTHr,Ept_FIller01_NDHr,Ept_FIller01_OTNDHr,Ept_FIller02_Hr,Ept_FIller02_OTHr,Ept_FIller02_NDHr,Ept_FIller02_OTNDHr,Ept_FIller03_Hr,Ept_FIller03_OTHr,Ept_FIller03_NDHr,Ept_FIller03_OTNDHr,Ept_FIller04_Hr,Ept_FIller04_OTHr,Ept_FIller04_NDHr,Ept_FIller04_OTNDHr,Ept_FIller05_Hr,Ept_FIller05_OTHr,Ept_FIller05_NDHr,Ept_FIller05_OTNDHr,Ept_FIller06_Hr,Ept_FIller06_OTHr,Ept_FIller06_NDHr,Ept_FIller06_OTNDHr,Usr_Login,Ludatetime) ", EmployeePayrollTransactionTable + "Ext", strAdjPayPeriodCol);
                    strUpdateRecordTemplate = @"{0}{28} SELECT '{1}','{2}',{29}{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},'{27}',GetDate() UNION ALL ";
                    //strUpdateRecordTemplate = @" INSERT INTO {0} (Ept_EmployeeId,Ept_CurrentPayPeriod,{28}Ept_FIller01_Hr,Ept_FIller01_OTHr,Ept_FIller01_NDHr,Ept_FIller01_OTNDHr,Ept_FIller02_Hr,Ept_FIller02_OTHr,Ept_FIller02_NDHr,Ept_FIller02_OTNDHr,Ept_FIller03_Hr,Ept_FIller03_OTHr,Ept_FIller03_NDHr,Ept_FIller03_OTNDHr,Ept_FIller04_Hr,Ept_FIller04_OTHr,Ept_FIller04_NDHr,Ept_FIller04_OTNDHr,Ept_FIller05_Hr,Ept_FIller05_OTHr,Ept_FIller05_NDHr,Ept_FIller05_OTNDHr,Ept_FIller06_Hr,Ept_FIller06_OTHr,Ept_FIller06_NDHr,Ept_FIller06_OTNDHr,Usr_Login,Ludatetime) VALUES('{1}','{2}',{29}{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},'{27}',GetDate()) ";
                    #endregion
                    foreach (DataRow drEmployeePayrollTransExt in dtEmployeePayrollTransactionExt.Rows)
                    {
                        #region Payroll Transaction Ext Save
                        strUpdateQuery += string.Format(strUpdateRecordTemplate
                                                            , "" //EmployeePayrollTransactionTable + "Ext"                     //0
                                                            , drEmployeePayrollTransExt["Ept_EmployeeId"]
                                                            , drEmployeePayrollTransExt["Ept_CurrentPayPeriod"]
                                                            , drEmployeePayrollTransExt["Ept_FIller01_Hr"]
                                                            , drEmployeePayrollTransExt["Ept_FIller01_OTHr"]
                                                            , drEmployeePayrollTransExt["Ept_FIller01_NDHr"]
                                                            , drEmployeePayrollTransExt["Ept_FIller01_OTNDHr"]
                                                            , drEmployeePayrollTransExt["Ept_FIller02_Hr"]
                                                            , drEmployeePayrollTransExt["Ept_FIller02_OTHr"]
                                                            , drEmployeePayrollTransExt["Ept_FIller02_NDHr"]
                                                            , drEmployeePayrollTransExt["Ept_FIller02_OTNDHr"]                //10
                                                            , drEmployeePayrollTransExt["Ept_FIller03_Hr"]
                                                            , drEmployeePayrollTransExt["Ept_FIller03_OTHr"]
                                                            , drEmployeePayrollTransExt["Ept_FIller03_NDHr"]
                                                            , drEmployeePayrollTransExt["Ept_FIller03_OTNDHr"]
                                                            , drEmployeePayrollTransExt["Ept_FIller04_Hr"]
                                                            , drEmployeePayrollTransExt["Ept_FIller04_OTHr"]
                                                            , drEmployeePayrollTransExt["Ept_FIller04_NDHr"]
                                                            , drEmployeePayrollTransExt["Ept_FIller04_OTNDHr"]
                                                            , drEmployeePayrollTransExt["Ept_FIller05_Hr"]
                                                            , drEmployeePayrollTransExt["Ept_FIller05_OTHr"]                //20
                                                            , drEmployeePayrollTransExt["Ept_FIller05_NDHr"]
                                                            , drEmployeePayrollTransExt["Ept_FIller05_OTNDHr"]
                                                            , drEmployeePayrollTransExt["Ept_FIller06_Hr"]
                                                            , drEmployeePayrollTransExt["Ept_FIller06_OTHr"]
                                                            , drEmployeePayrollTransExt["Ept_FIller06_NDHr"]
                                                            , drEmployeePayrollTransExt["Ept_FIller06_OTNDHr"]
                                                            , drEmployeePayrollTransExt["Usr_Login"]
                                                            , "" //strAdjPayPeriodCol
                                                            , strAdjPayPeriodVal);
                        #endregion
                        iUpdateCtr++;
                        if (iUpdateCtr == 50)
                        {
                            if (strUpdateQuery != "")
                                strUpdateQuery = strUpdateQuery.Substring(0, strUpdateQuery.Length - 10);

                            dal.ExecuteNonQuery(strInsertIntoTemplate + strUpdateQuery);
                            strUpdateQuery = "";
                            iUpdateCtr = 0;
                        }
                    }
                    if (strUpdateQuery != "")
                    {
                        strUpdateQuery = strUpdateQuery.Substring(0, strUpdateQuery.Length - 10);
                        dal.ExecuteNonQuery(strInsertIntoTemplate + strUpdateQuery);
                    }
                }
                //-----------------------------
                //if (!ProcessCurrentPeriod || Convert.ToBoolean(MULTSAL))
                //{
                //Save Payroll Transaction Detail Table
                strUpdateQuery = "";
                iUpdateCtr = 0;
                #region Payroll Transaction Detail Update
                strInsertIntoTemplate = string.Format(@" INSERT INTO {0} (Ept_EmployeeId,Ept_CurrentPayPeriod,{1}Ept_AbsentHr,Ept_RegularHr,Ept_RegularOTHr,Ept_RegularNDHr,Ept_RegularOTNDHr,Ept_RestdayHr,Ept_RestdayOTHr,Ept_RestdayNDHr,Ept_RestdayOTNDHr,Ept_LegalHolidayHr,Ept_LegalHolidayOTHr,Ept_LegalHolidayNDHr,Ept_LegalHolidayOTNDHr,Ept_SpecialHolidayHr,Ept_SpecialHolidayOTHr,Ept_SpecialHolidayNDHr,Ept_SpecialHolidayOTNDHr,Ept_PlantShutdownHr,Ept_PlantShutdownOTHr,Ept_PlantShutdownNDHr,Ept_PlantShutdownOTNDHr,Ept_CompanyHolidayHr,Ept_CompanyHolidayOTHr,Ept_CompanyHolidayNDHr,Ept_CompanyHolidayOTNDHr,Ept_RestdayLegalHolidayHr,Ept_RestdayLegalHolidayOTHr,Ept_RestdayLegalHolidayNDHr,Ept_RestdayLegalHolidayOTNDHr,Ept_RestdaySpecialHolidayHr,Ept_RestdaySpecialHolidayOTHr,Ept_RestdaySpecialHolidayNDHr,Ept_RestdaySpecialHolidayOTNDHr,Ept_RestdayCompanyHolidayHr,Ept_RestdayCompanyHolidayOTHr,Ept_RestdayCompanyHolidayNDHr,Ept_RestdayCompanyHolidayOTNDHr,Ept_RestdayLegalHolidayCount,Ept_WorkingDay,Ept_PayrollType,Ept_RestdayPlantShutdownHr,Ept_RestdayPlantShutdownOTHr,Ept_RestdayPlantShutdownNDHr,Ept_RestdayPlantShutdownOTNDHr,Usr_Login,Ludatetime,Ept_ProcessDate,Ept_SalaryRate,Ept_HourlyRate,Ept_SalaryType,Ept_LateHr,Ept_UndertimeHr,Ept_UnpaidLeaveHr,Ept_AbsentLegalHolidayHr,Ept_AbsentSpecialHolidayHr,Ept_AbsentCompanyHolidayHr,Ept_AbsentPlantShutdownHr,Ept_AbsentFillerHolidayHr,Ept_PaidLeaveHr,Ept_PaidLegalHolidayHr,Ept_PaidSpecialHolidayHr,Ept_PaidCompanyHolidayHr,Ept_PaidFillerHolidayHr,Ept_OvertimeAdjustmentAmt,Ept_WholeDayAbsentHr) ", EmployeePayrollTransactionTable + "Detail", strAdjPayPeriodCol);
                strUpdateRecordTemplate = @"{0}{52} SELECT '{1}','{2}',{53}{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34},{35},{36},{37},{38},{39},{40},{41},'{42}',{43},{44},{45},{46},'{47}',GetDate(),'{48}',{49},{50},'{51}',{54},{55},{56},{57},{58},{59},{60},{61},{62},{63},{64},{65},{66},{67},{68} UNION ALL ";
                //strUpdateRecordTemplate = @" INSERT INTO {0} (Ept_EmployeeId,Ept_CurrentPayPeriod,{52}Ept_AbsentHr,Ept_RegularHr,Ept_RegularOTHr,Ept_RegularNDHr,Ept_RegularOTNDHr,Ept_RestdayHr,Ept_RestdayOTHr,Ept_RestdayNDHr,Ept_RestdayOTNDHr,Ept_LegalHolidayHr,Ept_LegalHolidayOTHr,Ept_LegalHolidayNDHr,Ept_LegalHolidayOTNDHr,Ept_SpecialHolidayHr,Ept_SpecialHolidayOTHr,Ept_SpecialHolidayNDHr,Ept_SpecialHolidayOTNDHr,Ept_PlantShutdownHr,Ept_PlantShutdownOTHr,Ept_PlantShutdownNDHr,Ept_PlantShutdownOTNDHr,Ept_CompanyHolidayHr,Ept_CompanyHolidayOTHr,Ept_CompanyHolidayNDHr,Ept_CompanyHolidayOTNDHr,Ept_RestdayLegalHolidayHr,Ept_RestdayLegalHolidayOTHr,Ept_RestdayLegalHolidayNDHr,Ept_RestdayLegalHolidayOTNDHr,Ept_RestdaySpecialHolidayHr,Ept_RestdaySpecialHolidayOTHr,Ept_RestdaySpecialHolidayNDHr,Ept_RestdaySpecialHolidayOTNDHr,Ept_RestdayCompanyHolidayHr,Ept_RestdayCompanyHolidayOTHr,Ept_RestdayCompanyHolidayNDHr,Ept_RestdayCompanyHolidayOTNDHr,Ept_RestdayLegalHolidayCount,Ept_WorkingDay,Ept_PayrollType,Ept_RestdayPlantShutdownHr,Ept_RestdayPlantShutdownOTHr,Ept_RestdayPlantShutdownNDHr,Ept_RestdayPlantShutdownOTNDHr,Usr_Login,Ludatetime,Ept_ProcessDate,Ept_SalaryRate,Ept_HourlyRate,Ept_SalaryType,Ept_LateHr,Ept_UndertimeHr,Ept_UnpaidLeaveHr,Ept_AbsentLegalHolidayHr,Ept_AbsentSpecialHolidayHr,Ept_AbsentCompanyHolidayHr,Ept_AbsentPlantShutdownHr,Ept_AbsentFillerHolidayHr,Ept_PaidLeaveHr,Ept_PaidLegalHolidayHr,Ept_PaidSpecialHolidayHr,Ept_PaidCompanyHolidayHr,Ept_PaidFillerHolidayHr,Ept_OvertimeAdjustmentAmt,Ept_WholeDayAbsentHr) VALUES('{1}','{2}',{53}{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34},{35},{36},{37},{38},{39},{40},{41},'{42}',{43},{44},{45},{46},'{47}',GetDate(),'{48}',{49},{50},'{51}',{54},{55},{56},{57},{58},{59},{60},{61},{62},{63},{64},{65},{66},{67},{68}) ";
                #endregion
                foreach (DataRow drEmployeePayrollTransDetail in dtEmployeePayrollTransactionDetail.Rows)
                {
                    #region Payroll Transaction Detail Save
                    strUpdateQuery += string.Format(strUpdateRecordTemplate
                                                        , "" //EmployeePayrollTransactionTable + "Detail"                                                       //0
                                                        , drEmployeePayrollTransDetail["Ept_EmployeeId"]
                                                        , drEmployeePayrollTransDetail["Ept_CurrentPayPeriod"]
                                                        , drEmployeePayrollTransDetail["Ept_AbsentHr"]
                                                        , drEmployeePayrollTransDetail["Ept_RegularHr"]
                                                        , drEmployeePayrollTransDetail["Ept_RegularOTHr"]
                                                        , drEmployeePayrollTransDetail["Ept_RegularNDHr"]
                                                        , drEmployeePayrollTransDetail["Ept_RegularOTNDHr"]
                                                        , drEmployeePayrollTransDetail["Ept_RestdayHr"]
                                                        , drEmployeePayrollTransDetail["Ept_RestdayOTHr"]
                                                        , drEmployeePayrollTransDetail["Ept_RestdayNDHr"]                                           //10
                                                        , drEmployeePayrollTransDetail["Ept_RestdayOTNDHr"]
                                                        , drEmployeePayrollTransDetail["Ept_LegalHolidayHr"]
                                                        , drEmployeePayrollTransDetail["Ept_LegalHolidayOTHr"]
                                                        , drEmployeePayrollTransDetail["Ept_LegalHolidayNDHr"]
                                                        , drEmployeePayrollTransDetail["Ept_LegalHolidayOTNDHr"]
                                                        , drEmployeePayrollTransDetail["Ept_SpecialHolidayHr"]
                                                        , drEmployeePayrollTransDetail["Ept_SpecialHolidayOTHr"]
                                                        , drEmployeePayrollTransDetail["Ept_SpecialHolidayNDHr"]
                                                        , drEmployeePayrollTransDetail["Ept_SpecialHolidayOTNDHr"]
                                                        , drEmployeePayrollTransDetail["Ept_PlantShutdownHr"]                                       //20
                                                        , drEmployeePayrollTransDetail["Ept_PlantShutdownOTHr"]
                                                        , drEmployeePayrollTransDetail["Ept_PlantShutdownNDHr"]
                                                        , drEmployeePayrollTransDetail["Ept_PlantShutdownOTNDHr"]
                                                        , drEmployeePayrollTransDetail["Ept_CompanyHolidayHr"]
                                                        , drEmployeePayrollTransDetail["Ept_CompanyHolidayOTHr"]
                                                        , drEmployeePayrollTransDetail["Ept_CompanyHolidayNDHr"]
                                                        , drEmployeePayrollTransDetail["Ept_CompanyHolidayOTNDHr"]
                                                        , drEmployeePayrollTransDetail["Ept_RestdayLegalHolidayHr"]
                                                        , drEmployeePayrollTransDetail["Ept_RestdayLegalHolidayOTHr"]
                                                        , drEmployeePayrollTransDetail["Ept_RestdayLegalHolidayNDHr"]                               //30
                                                        , drEmployeePayrollTransDetail["Ept_RestdayLegalHolidayOTNDHr"]
                                                        , drEmployeePayrollTransDetail["Ept_RestdaySpecialHolidayHr"]
                                                        , drEmployeePayrollTransDetail["Ept_RestdaySpecialHolidayOTHr"]
                                                        , drEmployeePayrollTransDetail["Ept_RestdaySpecialHolidayNDHr"]
                                                        , drEmployeePayrollTransDetail["Ept_RestdaySpecialHolidayOTNDHr"]
                                                        , drEmployeePayrollTransDetail["Ept_RestdayCompanyHolidayHr"]
                                                        , drEmployeePayrollTransDetail["Ept_RestdayCompanyHolidayOTHr"]
                                                        , drEmployeePayrollTransDetail["Ept_RestdayCompanyHolidayNDHr"]
                                                        , drEmployeePayrollTransDetail["Ept_RestdayCompanyHolidayOTNDHr"]
                                                        , drEmployeePayrollTransDetail["Ept_RestdayLegalHolidayCount"]                              //40
                                                        , drEmployeePayrollTransDetail["Ept_WorkingDay"]
                                                        , drEmployeePayrollTransDetail["Ept_PayrollType"]
                                                        , drEmployeePayrollTransDetail["Ept_RestdayPlantShutdownHr"]
                                                        , drEmployeePayrollTransDetail["Ept_RestdayPlantShutdownOTHr"]
                                                        , drEmployeePayrollTransDetail["Ept_RestdayPlantShutdownNDHr"]
                                                        , drEmployeePayrollTransDetail["Ept_RestdayPlantShutdownOTNDHr"]
                                                        , drEmployeePayrollTransDetail["Usr_Login"]
                                                        , drEmployeePayrollTransDetail["Ept_ProcessDate"]
                                                        , drEmployeePayrollTransDetail["Ept_SalaryRate"]
                                                        , drEmployeePayrollTransDetail["Ept_HourlyRate"]                                            //50
                                                        , drEmployeePayrollTransDetail["Ept_SalaryType"]
                                                        , "" //strAdjPayPeriodCol
                                                        , strAdjPayPeriodVal
                                                        , drEmployeePayrollTransDetail["Ept_LateHr"]
                                                        , drEmployeePayrollTransDetail["Ept_UndertimeHr"]
                                                        , drEmployeePayrollTransDetail["Ept_UnpaidLeaveHr"]
                                                        , drEmployeePayrollTransDetail["Ept_AbsentLegalHolidayHr"]
                                                        , drEmployeePayrollTransDetail["Ept_AbsentSpecialHolidayHr"]
                                                        , drEmployeePayrollTransDetail["Ept_AbsentCompanyHolidayHr"]
                                                        , drEmployeePayrollTransDetail["Ept_AbsentPlantShutdownHr"]                                       //60
                                                        , drEmployeePayrollTransDetail["Ept_AbsentFillerHolidayHr"]
                                                        , drEmployeePayrollTransDetail["Ept_PaidLeaveHr"]
                                                        , drEmployeePayrollTransDetail["Ept_PaidLegalHolidayHr"]
                                                        , drEmployeePayrollTransDetail["Ept_PaidSpecialHolidayHr"]
                                                        , drEmployeePayrollTransDetail["Ept_PaidCompanyHolidayHr"]
                                                        , drEmployeePayrollTransDetail["Ept_PaidFillerHolidayHr"]
                                                        , drEmployeePayrollTransDetail["Ept_OvertimeAdjustmentAmt"]
                                                        , drEmployeePayrollTransDetail["Ept_WholeDayAbsentHr"]);
                    #endregion
                    iUpdateCtr++;
                    if (iUpdateCtr == 50)
                    {
                        if (strUpdateQuery != "")
                            strUpdateQuery = strUpdateQuery.Substring(0, strUpdateQuery.Length - 10);

                        dal.ExecuteNonQuery(strInsertIntoTemplate + strUpdateQuery);
                        strUpdateQuery = "";
                        iUpdateCtr = 0;
                    }
                }
                if (strUpdateQuery != "")
                {
                    strUpdateQuery = strUpdateQuery.Substring(0, strUpdateQuery.Length - 10);
                    dal.ExecuteNonQuery(strInsertIntoTemplate + strUpdateQuery);
                }
                //-----------------------------
                //Save Payroll Transaction Ext Detail Table
                if (bHasDayCodeExt)
                {
                    strUpdateQuery = "";
                    iUpdateCtr = 0;
                    #region Payroll Transaction Ext Detail Update
                    strInsertIntoTemplate = string.Format(@" INSERT INTO {0} (Ept_EmployeeId,Ept_CurrentPayPeriod,{1}Ept_FIller01_Hr,Ept_FIller01_OTHr,Ept_FIller01_NDHr,Ept_FIller01_OTNDHr,Ept_FIller02_Hr,Ept_FIller02_OTHr,Ept_FIller02_NDHr,Ept_FIller02_OTNDHr,Ept_FIller03_Hr,Ept_FIller03_OTHr,Ept_FIller03_NDHr,Ept_FIller03_OTNDHr,Ept_FIller04_Hr,Ept_FIller04_OTHr,Ept_FIller04_NDHr,Ept_FIller04_OTNDHr,Ept_FIller05_Hr,Ept_FIller05_OTHr,Ept_FIller05_NDHr,Ept_FIller05_OTNDHr,Ept_FIller06_Hr,Ept_FIller06_OTHr,Ept_FIller06_NDHr,Ept_FIller06_OTNDHr,Usr_Login,Ludatetime,Ept_ProcessDate,Ept_SalaryRate,Ept_HourlyRate,Ept_SalaryType,Ept_PayrollType) ", EmployeePayrollTransactionTable + "ExtDetail", strAdjPayPeriodCol);
                    strUpdateRecordTemplate = @"{0}{33} SELECT '{1}','{2}',{34}{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},'{27}',GetDate(),'{28}',{29},{30},'{31}','{32}' UNION ALL ";
                    //strUpdateRecordTemplate = @" INSERT INTO {0} (Ept_EmployeeId,Ept_CurrentPayPeriod,{33}Ept_FIller01_Hr,Ept_FIller01_OTHr,Ept_FIller01_NDHr,Ept_FIller01_OTNDHr,Ept_FIller02_Hr,Ept_FIller02_OTHr,Ept_FIller02_NDHr,Ept_FIller02_OTNDHr,Ept_FIller03_Hr,Ept_FIller03_OTHr,Ept_FIller03_NDHr,Ept_FIller03_OTNDHr,Ept_FIller04_Hr,Ept_FIller04_OTHr,Ept_FIller04_NDHr,Ept_FIller04_OTNDHr,Ept_FIller05_Hr,Ept_FIller05_OTHr,Ept_FIller05_NDHr,Ept_FIller05_OTNDHr,Ept_FIller06_Hr,Ept_FIller06_OTHr,Ept_FIller06_NDHr,Ept_FIller06_OTNDHr,Usr_Login,Ludatetime,Ept_ProcessDate,Ept_SalaryRate,Ept_HourlyRate,Ept_SalaryType,Ept_PayrollType) VALUES('{1}','{2}',{34}{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},'{27}',GetDate(),'{28}',{29},{30},'{31}','{32}') ";
                    #endregion
                    foreach (DataRow drEmployeePayrollTransExtDetail in dtEmployeePayrollTransactionExtDetail.Rows)
                    {
                        #region Payroll Transaction Ext Detail Save
                        strUpdateQuery += string.Format(strUpdateRecordTemplate
                                                            , "" //EmployeePayrollTransactionTable + "ExtDetail"                     //0
                                                            , drEmployeePayrollTransExtDetail["Ept_EmployeeId"]
                                                            , drEmployeePayrollTransExtDetail["Ept_CurrentPayPeriod"]
                                                            , drEmployeePayrollTransExtDetail["Ept_FIller01_Hr"]
                                                            , drEmployeePayrollTransExtDetail["Ept_FIller01_OTHr"]
                                                            , drEmployeePayrollTransExtDetail["Ept_FIller01_NDHr"]
                                                            , drEmployeePayrollTransExtDetail["Ept_FIller01_OTNDHr"]
                                                            , drEmployeePayrollTransExtDetail["Ept_FIller02_Hr"]
                                                            , drEmployeePayrollTransExtDetail["Ept_FIller02_OTHr"]
                                                            , drEmployeePayrollTransExtDetail["Ept_FIller02_NDHr"]
                                                            , drEmployeePayrollTransExtDetail["Ept_FIller02_OTNDHr"]                //10
                                                            , drEmployeePayrollTransExtDetail["Ept_FIller03_Hr"]
                                                            , drEmployeePayrollTransExtDetail["Ept_FIller03_OTHr"]
                                                            , drEmployeePayrollTransExtDetail["Ept_FIller03_NDHr"]
                                                            , drEmployeePayrollTransExtDetail["Ept_FIller03_OTNDHr"]
                                                            , drEmployeePayrollTransExtDetail["Ept_FIller04_Hr"]
                                                            , drEmployeePayrollTransExtDetail["Ept_FIller04_OTHr"]
                                                            , drEmployeePayrollTransExtDetail["Ept_FIller04_NDHr"]
                                                            , drEmployeePayrollTransExtDetail["Ept_FIller04_OTNDHr"]
                                                            , drEmployeePayrollTransExtDetail["Ept_FIller05_Hr"]
                                                            , drEmployeePayrollTransExtDetail["Ept_FIller05_OTHr"]                //20
                                                            , drEmployeePayrollTransExtDetail["Ept_FIller05_NDHr"]
                                                            , drEmployeePayrollTransExtDetail["Ept_FIller05_OTNDHr"]
                                                            , drEmployeePayrollTransExtDetail["Ept_FIller06_Hr"]
                                                            , drEmployeePayrollTransExtDetail["Ept_FIller06_OTHr"]
                                                            , drEmployeePayrollTransExtDetail["Ept_FIller06_NDHr"]
                                                            , drEmployeePayrollTransExtDetail["Ept_FIller06_OTNDHr"]
                                                            , drEmployeePayrollTransExtDetail["Usr_Login"]
                                                            , drEmployeePayrollTransExtDetail["Ept_ProcessDate"]
                                                            , drEmployeePayrollTransExtDetail["Ept_SalaryRate"]
                                                            , drEmployeePayrollTransExtDetail["Ept_HourlyRate"]                   //30                                  
                                                            , drEmployeePayrollTransExtDetail["Ept_SalaryType"]
                                                            , drEmployeePayrollTransExtDetail["Ept_PayrollType"]
                                                            , "" //strAdjPayPeriodCol
                                                            , strAdjPayPeriodVal);
                        #endregion
                        iUpdateCtr++;
                        if (iUpdateCtr == 50)
                        {
                            if (strUpdateQuery != "")
                                strUpdateQuery = strUpdateQuery.Substring(0, strUpdateQuery.Length - 10);

                            dal.ExecuteNonQuery(strInsertIntoTemplate + strUpdateQuery);
                            strUpdateQuery = "";
                            iUpdateCtr = 0;
                        }
                    }
                    if (strUpdateQuery != "")
                    {
                        strUpdateQuery = strUpdateQuery.Substring(0, strUpdateQuery.Length - 10);
                        dal.ExecuteNonQuery(strInsertIntoTemplate + strUpdateQuery);
                    }
                }

                //Update Payroll Rates
                UpdateSalaryRatePerDay(ProcessAll, EmployeeId);
                //}
                StatusHandler(this, new StatusEventArgs("Saving of Payroll Transaction Records", true));
                //-----------------------------
                StatusHandler(this, new StatusEventArgs("Process Daily Allowances", false));
                //Process Daily Allowance
                if (ProcessAll && ProcessCurrentPeriod && !ProcessSeparated)
                {
                    SystemCycleProcessingBL.GenerateOtherAllowances(ProcessAll, "", EmployeeList, ProcessCurrentPeriod, "");
                    SystemCycleProcessingBL.PostOtherAllowances(ProcessAll, "", EmployeeList, ProcessPayrollPeriod);
                }
                else if (((!ProcessAll && EmployeeId != "") || EmployeeList != "") && !ProcessSeparated)
                {
                    SystemCycleProcessingBL.GenerateOtherAllowances(ProcessAll, EmployeeId, EmployeeList, ProcessCurrentPeriod, "");
                    if (ProcessCurrentPeriod)
                        SystemCycleProcessingBL.PostOtherAllowances(ProcessAll, EmployeeId, EmployeeList, ProcessPayrollPeriod);
                }
                StatusHandler(this, new StatusEventArgs("Process Daily Allowances", true));
                //-----------------------------
                StatusHandler(this, new StatusEventArgs("Generate Payroll Transaction Error List", false));
                if (ProcessCurrentPeriod)
                {
                    #region Generate Payroll Transaction Error List
                    DataSet tempds = GetLaborHoursNegative(ProcessAll, EmployeeId);
                    if (tempds.Tables[0].Rows.Count > 0)
                        dtErrList = CreateErrorListForPayTrans(dtErrList, tempds);

                    //Append labor hour generation errors
                    dtErrList = SaveLaborHourErrorReportList(dtErrList);

                    if (dtErrList.Rows.Count > 0)
                        InsertToLaborHrErr(dtErrList);
                    #endregion
                }
                StatusHandler(this, new StatusEventArgs("Generate Payroll Transaction Error List", true));
                //-----------------------------
                //code end
                //dal.CommitTransactionSnapshot();
            }
            catch (Exception ex)
            {
                //dal.RollBackTransactionSnapshot();
                CommonProcedures.logErrorToFile(ex.ToString());
                throw new Exception("Labor hours generation has encountered some errors: \n" + ex.Message);
            }
            finally
            {
                //dal.CloseDB();
            }

            return dtErrList;
        }
        #endregion

        #region Minor Functions
        public void SetProcessFlags()
        {
            string strResult = string.Empty;

            #region Parameters
            StatusHandler(this, new StatusEventArgs("Loading Parameters", false));
            StatusHandler(this, new StatusEventArgs("Loading Parameters", true));

            strResult = commonBL.GetNumericValue("MINOTHR");
            MINOTHR = strResult.Equals(string.Empty) ? 0 : Convert.ToDouble(strResult);
            MINOTHR_ORIG = MINOTHR;
            StatusHandler(this, new StatusEventArgs(string.Format("  MINOTHR = {0}", MINOTHR), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  MINOTHR = {0}", MINOTHR), true));

            strResult = commonBL.GetNumericValue("NDFRACTION");
            NDFRACTION = strResult.Equals(string.Empty) ? 0 : Convert.ToDouble(strResult);
            StatusHandler(this, new StatusEventArgs(string.Format("  NDFRACTION = {0}", NDFRACTION), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  NDFRACTION = {0}", NDFRACTION), true));

            strResult = commonBL.GetNumericValue("ABSFRACTRG");
            ABSFRACTRG = strResult.Equals(string.Empty) ? 0 : Convert.ToInt32(Convert.ToDouble(strResult));
            StatusHandler(this, new StatusEventArgs(string.Format("  ABSFRACTRG = {0}", ABSFRACTRG), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  ABSFRACTRG = {0}", ABSFRACTRG), true));

            strResult = commonBL.GetNumericValue("ABSFRACT");
            ABSFRACT = strResult.Equals(string.Empty) ? 0 : Convert.ToInt32(Convert.ToDouble(strResult));
            StatusHandler(this, new StatusEventArgs(string.Format("  ABSFRACT = {0}", ABSFRACT), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  ABSFRACT = {0}", ABSFRACT), true));

            strResult = commonBL.GetNumericValue("OTLIMITHR");
            OTLIMITHR = strResult.Equals(string.Empty) ? 0 : Convert.ToDouble(strResult);
            OTLIMITHR_ORIG = OTLIMITHR;
            StatusHandler(this, new StatusEventArgs(string.Format("  OTLIMITHR = {0}", OTLIMITHR), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  OTLIMITHR = {0}", OTLIMITHR), true));

            strResult = commonBL.GetNumericValue("OTLIMITEQV");
            OTLIMITEQV = strResult.Equals(string.Empty) ? 0 : Convert.ToDouble(strResult);
            OTLIMITEQV_ORIG = OTLIMITEQV;
            StatusHandler(this, new StatusEventArgs(string.Format("  OTLIMITEQV = {0}", OTLIMITEQV), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  OTLIMITEQV = {0}", OTLIMITEQV), true));

            strResult = commonBL.GetNumericValue("OTLIMITAPP");
            OTLIMITAPP = strResult.Equals(string.Empty) ? 0 : Convert.ToDouble(strResult);
            StatusHandler(this, new StatusEventArgs(string.Format("  OTLIMITAPP = {0}", OTLIMITAPP), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  OTLIMITAPP = {0}", OTLIMITAPP), true));

            strResult = commonBL.GetNumericValue("TIMEFRAC");
            TIMEFRAC = strResult.Equals(string.Empty) ? 0 : Convert.ToInt32(Convert.ToDouble(strResult));
            StatusHandler(this, new StatusEventArgs(string.Format("  TIMEFRAC = {0}", TIMEFRAC), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  TIMEFRAC = {0}", TIMEFRAC), true));

            strResult = commonBL.GetNumericValue("LATECHARGE");
            LATECHARGE = strResult.Equals(string.Empty) ? 0 : Convert.ToDouble(strResult);
            StatusHandler(this, new StatusEventArgs(string.Format("  LATECHARGE = {0}", LATECHARGE), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  LATECHARGE = {0}", LATECHARGE), true));

            strResult = commonBL.GetNumericValue("MDIVISOR");
            MDIVISOR = strResult.Equals(string.Empty) ? 0 : Convert.ToDouble(strResult);
            StatusHandler(this, new StatusEventArgs(string.Format("  MDIVISOR = {0}", MDIVISOR), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  MDIVISOR = {0}", MDIVISOR), true));

            strResult = commonBL.GetNumericValue("OTFRACTION");
            OTFRACT = strResult.Equals(string.Empty) ? 0 : Convert.ToDouble(strResult);
            StatusHandler(this, new StatusEventArgs(string.Format("  OTFRACTION = {0}", OTFRACT), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  OTFRACTION = {0}", OTFRACT), true));

            strResult = commonBL.GetNumericValue("LOGPAD");
            LOGPAD = strResult.Equals(string.Empty) ? 0 : Convert.ToInt32(Convert.ToDouble(strResult) * 60);
            StatusHandler(this, new StatusEventArgs(string.Format("  LOGPAD = {0}", LOGPAD), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  LOGPAD = {0}", LOGPAD), true));

            strResult = commonBL.GetNumericValue("POCKETGAP");
            POCKETGAP = strResult.Equals(string.Empty) ? 0 : Convert.ToInt32(Convert.ToDouble(strResult));
            StatusHandler(this, new StatusEventArgs(string.Format("  POCKETGAP = {0}", POCKETGAP), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  POCKETGAP = {0}", POCKETGAP), true));

            strResult = commonBL.GetNumericValue("POCKETTIME");
            POCKETTIME = strResult.Equals(string.Empty) ? 0 : Convert.ToInt32(Convert.ToDouble(strResult));
            StatusHandler(this, new StatusEventArgs(string.Format("  POCKETTIME = {0}", POCKETTIME), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  POCKETTIME = {0}", POCKETTIME), true));

            strResult = commonBL.GetNumericValue("POCKETSIZE");
            POCKETSIZE = strResult.Equals(string.Empty) ? 0 : Convert.ToInt32(Convert.ToDouble(strResult));
            StatusHandler(this, new StatusEventArgs(string.Format("  POCKETSIZE = {0}", POCKETSIZE), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  POCKETSIZE = {0}", POCKETSIZE), true));

            strResult = commonBL.GetNumericValue("MAXLATEMIN");
            MAXLATEMIN = strResult.Equals(string.Empty) ? 0 : Convert.ToInt32(Convert.ToDouble(strResult));
            StatusHandler(this, new StatusEventArgs(string.Format("  MAXLATEMIN = {0}", MAXLATEMIN), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  MAXLATEMIN = {0}", MAXLATEMIN), true));

            strResult = commonBL.GetNumericValue("MAXUTMIN");
            MAXUTMIN = strResult.Equals(string.Empty) ? 0 : Convert.ToInt32(Convert.ToDouble(strResult));
            StatusHandler(this, new StatusEventArgs(string.Format("  MAXUTMIN = {0}", MAXUTMIN), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  MAXUTMIN = {0}", MAXUTMIN), true));

            strResult = commonBL.GetNumericValue("NDBRCKTCNT");
            NDBRCKTCNT = strResult.Equals(string.Empty) ? 1 : Convert.ToInt32(Convert.ToDouble(strResult));
            StatusHandler(this, new StatusEventArgs(string.Format("  NDBRCKTCNT = {0}", NDBRCKTCNT), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  NDBRCKTCNT = {0}", NDBRCKTCNT), true));
            #endregion

            #region Control Flags
            StatusHandler(this, new StatusEventArgs("Loading Process Flags", false));
            StatusHandler(this, new StatusEventArgs("Loading Process Flags", true));

            ONEPREVDAY = Convert.ToString(commonBL.GetProcessControlFlag("TIMEKEEP", "ONEPREVDAY"));
            StatusHandler(this, new StatusEventArgs(string.Format("  ONEPREVDAY = {0}", ONEPREVDAY), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  ONEPREVDAY = {0}", ONEPREVDAY), true));

            REGPREVDAY = Convert.ToString(commonBL.GetProcessControlFlag("TIMEKEEP", "REGPREVDAY"));
            StatusHandler(this, new StatusEventArgs(string.Format("  REGPREVDAY = {0}", REGPREVDAY), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  REGPREVDAY = {0}", REGPREVDAY), true));

            strResult = Convert.ToString(commonBL.GetProcessControlFlag("TIMEKEEP", "OTOFSETABS"));
            OTOFSETABS = strResult.Equals(string.Empty) ? Convert.ToString(0) : strResult;
            StatusHandler(this, new StatusEventArgs(string.Format("  OTOFSETABS = {0}", OTOFSETABS), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  OTOFSETABS = {0}", OTOFSETABS), true));

            LVHRENTRY = Convert.ToString(commonBL.GetProcessControlFlag("LEAVE", "LVHRENTRY"));
            StatusHandler(this, new StatusEventArgs(string.Format("  LVHRENTRY = {0}", LVHRENTRY), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  LVHRENTRY = {0}", LVHRENTRY), true));

            NOABSNWHRE = Convert.ToString(commonBL.GetProcessControlFlag("TIMEKEEP", "NOABSNWHRE"));
            StatusHandler(this, new StatusEventArgs(string.Format("  NOABSNWHRE = {0}", NOABSNWHRE), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  NOABSNWHRE = {0}", NOABSNWHRE), true));

            OTFORMGR = Convert.ToString(commonBL.GetProcessControlFlag("TIMEKEEP", "OTFORMGR"));
            StatusHandler(this, new StatusEventArgs(string.Format("  OTFORMGR = {0}", OTFORMGR), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  OTFORMGR = {0}", OTFORMGR), true));

            MULTSAL = Convert.ToString(commonBL.GetProcessControlFlag("PAYROLL", "MULTSAL"));
            StatusHandler(this, new StatusEventArgs(string.Format("  MULTSAL = {0}", MULTSAL), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  MULTSAL = {0}", MULTSAL), true));

            PSDMONTHLY = Convert.ToString(commonBL.GetProcessControlFlag("TIMEKEEP", "PSDMONTHLY"));
            StatusHandler(this, new StatusEventArgs(string.Format("  PSDMONTHLY = {0}", PSDMONTHLY), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  PSDMONTHLY = {0}", PSDMONTHLY), true));

            NONDOTDAY = Convert.ToString(commonBL.GetProcessControlFlag("TIMEKEEP", "NONDOTDAY"));
            StatusHandler(this, new StatusEventArgs(string.Format("  NONDOTDAY = {0}", NONDOTDAY), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  NONDOTDAY = {0}", NONDOTDAY), true));

            NDPREM1ST8 = Convert.ToString(commonBL.GetProcessControlFlag("TIMEKEEP", "NDPREM1ST8"));
            StatusHandler(this, new StatusEventArgs(string.Format("  NDPREM1ST8 = {0}", NDPREM1ST8), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  NDPREM1ST8 = {0}", NDPREM1ST8), true));

            NDSPLTSHFT = Convert.ToString(commonBL.GetProcessControlFlag("TIMEKEEP", "NDSPLTSHFT"));
            StatusHandler(this, new StatusEventArgs(string.Format("  NDSPLTSHFT = {0}", NDSPLTSHFT), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  NDSPLTSHFT = {0}", NDSPLTSHFT), true));

            NDCNTBREAK = Convert.ToString(commonBL.GetProcessControlFlag("TIMEKEEP", "NDCNTBREAK"));
            StatusHandler(this, new StatusEventArgs(string.Format("  NDCNTBREAK = {0}", NDCNTBREAK), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  NDCNTBREAK = {0}", NDCNTBREAK), true));

            HRFRCLBRHR = Convert.ToString(commonBL.GetProcessControlFlag("TIMEKEEP", "HRFRCLBRHR"));
            StatusHandler(this, new StatusEventArgs(string.Format("  HRFRCLBRHR = {0}", HRFRCLBRHR), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  HRFRCLBRHR = {0}", HRFRCLBRHR), true));

            EXTREGLVE = Convert.ToString(commonBL.GetProcessControlFlag("TIMEKEEP", "EXTREGLVE"));
            StatusHandler(this, new StatusEventArgs(string.Format("  EXTREGLVE = {0}", EXTREGLVE), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  EXTREGLVE = {0}", EXTREGLVE), true));

            EXTREGULVE = Convert.ToString(commonBL.GetProcessControlFlag("TIMEKEEP", "EXTREGULVE"));
            StatusHandler(this, new StatusEventArgs(string.Format("  EXTREGULVE = {0}", EXTREGULVE), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  EXTREGULVE = {0}", EXTREGULVE), true));

            ATLVEADJ = Convert.ToString(commonBL.GetProcessControlFlag("TIMEKEEP", "ATLVEADJ"));
            StatusHandler(this, new StatusEventArgs(string.Format("  ATLVEADJ = {0}", ATLVEADJ), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  ATLVEADJ = {0}", ATLVEADJ), true));

            OBCOMPOT = Convert.ToString(commonBL.GetProcessControlFlag("TIMEKEEP", "OBCOMPOT"));
            StatusHandler(this, new StatusEventArgs(string.Format("  OBCOMPOT = {0}", OBCOMPOT), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  OBCOMPOT = {0}", OBCOMPOT), true));

            NDREGSHIFT = Convert.ToString(commonBL.GetProcessControlFlag("TIMEKEEP", "NDREGSHIFT"));
            StatusHandler(this, new StatusEventArgs(string.Format("  NDREGSHIFT = {0}", NDREGSHIFT), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  NDREGSHIFT = {0}", NDREGSHIFT), true));

            MLPAYHOL = Convert.ToString(commonBL.GetProcessControlFlag("TIMEKEEP", "MLPAYHOL"));
            StatusHandler(this, new StatusEventArgs(string.Format("  MLPAYHOL = {0}", MLPAYHOL), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  MLPAYHOL = {0}", MLPAYHOL), true));

            OTROUNDING = Convert.ToString(commonBL.GetProcessControlFlag("TIMEKEEP", "OTROUNDING"));
            StatusHandler(this, new StatusEventArgs(string.Format("  OTROUNDING = {0}", OTROUNDING), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  OTROUNDING = {0}", OTROUNDING), true));

            LEGHOLINRG = Convert.ToString(commonBL.GetProcessControlFlag("TIMEKEEP", "LEGHOLINRG"));
            StatusHandler(this, new StatusEventArgs(string.Format("  LEGHOLINRG = {0}", LEGHOLINRG), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  LEGHOLINRG = {0}", LEGHOLINRG), true));

            CNTPDBRK = Convert.ToString(commonBL.GetProcessControlFlag("TIMEKEEP", "CNTPDBRK"));
            StatusHandler(this, new StatusEventArgs(string.Format("  CNTPDBRK = {0}", CNTPDBRK), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  CNTPDBRK = {0}", CNTPDBRK), true));

            RNDOTFRAC = Convert.ToString(commonBL.GetProcessControlFlag("TIMEKEEP", "RNDOTFRAC"));
            StatusHandler(this, new StatusEventArgs(string.Format("  RNDOTFRAC = {0}", RNDOTFRAC), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  RNDOTFRAC = {0}", RNDOTFRAC), true));

            FLEXSHIFT = Convert.ToString(commonBL.GetProcessControlFlag("TIMEKEEP", "FLEXSHIFT"));
            StatusHandler(this, new StatusEventArgs(string.Format("  FLEXSHIFT = {0}", FLEXSHIFT), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  FLEXSHIFT = {0}", FLEXSHIFT), true));
            #endregion

            #region Parameter List
            DataTable dtResult = commonBL.GetParameterListAllowed("LATECHRGFQ");
            if (dtResult.Rows.Count > 0)
                LATECHARGEFREQUENCY = dtResult.Rows[0]["Pmx_Classification"].ToString();
            else
                LATECHARGEFREQUENCY = "";
            StatusHandler(this, new StatusEventArgs(string.Format("  LATECHRGFQ = {0}", LATECHARGEFREQUENCY), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  LATECHRGFQ = {0}", LATECHARGEFREQUENCY), true));

            LATEBRACKETDEDUCTION = GetBracketParameter("LATEBRCKT");
            StatusHandler(this, new StatusEventArgs(string.Format("  Getting late bracket deduction"), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  Getting late bracket deduction"), true));

            UNDERTIMEBRACKETDEDUCTION = GetBracketParameter("UTIMEBRCKT");
            StatusHandler(this, new StatusEventArgs(string.Format("  Getting undertime bracket deduction"), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  Getting undertime bracket deduction"), true));

            REGHRSREQD = commonBL.GetParameterList("REGHRSREQD");
            StatusHandler(this, new StatusEventArgs(string.Format("  Getting regular hours requirement"), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  Getting regular hours requirement"), true));

            MINOTHR_TBL = commonBL.GetParameterList("MINOTHR");
            StatusHandler(this, new StatusEventArgs(string.Format("  Getting minimum overtime hour"), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  Getting minimum overtime hour"), true));

            OTLIMITHR_TBL = commonBL.GetParameterList("OTLIMITHR");
            OTLIMITEQV_TBL = commonBL.GetParameterList("OTLIMITEQV");
            StatusHandler(this, new StatusEventArgs(string.Format("  Getting overtime limit for managers"), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  Getting overtime limit for managers"), true));

            ULPREVDAY = commonBL.GetParameterList("ULPREVDAY");
            StatusHandler(this, new StatusEventArgs(string.Format("  Getting unpaid leaves allowed before holiday"), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  Getting unpaid leaves allowed before holiday"), true));

            MIDOT = CheckIfMidOTIsEnabled();
            StatusHandler(this, new StatusEventArgs(string.Format("  Getting mid-overtime status"), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  Getting mid-overtime status"), true));

            HOURFRACFORMULA1 = commonBL.GetParameterValue("HRFRCLBRHR", "FORMULA1");
            HOURFRACFORMULA2 = commonBL.GetParameterValue("HRFRCLBRHR", "FORMULA2");
            StatusHandler(this, new StatusEventArgs(string.Format("  Getting hour fraction formula"), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  Getting hour fraction formula"), true));

            NP1_BEGTIME = GetMinsFromHourStr(commonBL.GetParameterValue("NDBRACKET", "NP1_BEGTIME"));
            NP1_ENDTIME = GetMinsFromHourStr(commonBL.GetParameterValue("NDBRACKET", "NP1_ENDTIME"));
            if (NP1_ENDTIME < NP1_BEGTIME)
                NP1_ENDTIME += GRAVEYARD24;
            NP2_BEGTIME = GetMinsFromHourStr(commonBL.GetParameterValue("NDBRACKET", "NP2_BEGTIME"));
            NP2_ENDTIME = GetMinsFromHourStr(commonBL.GetParameterValue("NDBRACKET", "NP2_ENDTIME"));
            if (NP2_ENDTIME < NP2_BEGTIME)
                NP2_ENDTIME += GRAVEYARD24;
            StatusHandler(this, new StatusEventArgs(string.Format("  Getting night diff bracket parameters"), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  Getting night diff bracket parameters"), true));
            #endregion
        }

        private bool CheckIfMidOTIsEnabled()
        {
            DataTable dtResult = new DataTable();
            bool bMidOTExists = false;

            #region query
            string qString = @" select Pmx_Status 
                                from T_ParameterMasterExt
                                where Pmx_ParameterID = 'OTTYPE'
                                and Pmx_Classification = 'DEFMID'";
            #endregion

            using (DALHelper dal = new DALHelper(MainDB, false))
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(qString).Tables[0];
                if (dtResult.Rows.Count > 0 && dtResult.Rows[0][0].ToString() == "A")
                {
                    bMidOTExists = true;
                }
                dal.CloseDB();
            }

            return bMidOTExists;
        }

        public DataTable CheckPreProcessingErrorExists(string PayPeriod, string UserLogin, DALHelper dalHelper)
        {
            return CheckPreProcessingErrorExists(PayPeriod, "", UserLogin, dalHelper);
        }

        public DataTable CheckPreProcessingErrorExists(string PayPeriod, string TypeFilter, string UserLogin, DALHelper dalHelper)
        {
            return CheckPreProcessingErrorExists(PayPeriod, "", TypeFilter, "", UserLogin, dalHelper);
        }

        public DataTable CheckPreProcessingErrorExists(string PayPeriod, string EmployeeID, string TypeFilter, string CostCenter, string UserLogin, DALHelper dalHelper)
        {
            #region Query
            string condition = string.Empty;
            if (CostCenter != string.Empty)
            {
                condition += string.Format("And Emt_CostcenterCode IN ({0})", CostCenter);
            }
            string query = string.Format(@"
--VARIABLE DECLARATION AND INITIALIZATION
DECLARE @Type VARCHAR(10) = '{2}'
DECLARE @EmployeeID VARCHAR(15) = '{1}'
DECLARE @PayrollPeriod VARCHAR(7)
DECLARE @PayrollCycle CHAR(1)
DECLARE @EndPayPeriod DATETIME
DECLARE @PayrollRate VARCHAR(10) = 'B' --BASIC RATE
DECLARE @MMaxRate DECIMAL(18, 2) = 0
DECLARE @DMaxRate DECIMAL(18, 2) = 0
DECLARE @HMaxRate DECIMAL(18, 2) = 0 
DECLARE @MMinRate DECIMAL(18, 2) = 0
DECLARE @DMinRate DECIMAL(18, 2) = 0
DECLARE @HMinRate DECIMAL(18, 2) = 0 
DECLARE @CompTaxSched CHAR(1) = 'S' --SEMI-MONTHLY
DECLARE @TaxApplicPayPeriod VARCHAR(7)
DECLARE @SSSApplicPayPeriod VARCHAR(7)
DECLARE @PhApplicPayPeriod VARCHAR(7)
DECLARE @CurPPCnt as tinyint
DECLARE @MaxEndDate as datetime
DECLARE @CurStartDate as datetime

SELECT @PayrollCycle = SUBSTRING(Ppm_PayPeriod, 7, 1)
		, @PayrollPeriod = Ppm_PayPeriod
		, @EndPayPeriod = Ppm_EndCycle
FROM T_PayPeriodMaster
WHERE Ppm_PayPeriod = '{0}'

select @CurPPCnt = datediff(dd,Ppm_StartCycle,Ppm_EndCycle+1)
        , @CurStartDate = Ppm_StartCycle
        , @MaxEndDate = Ppm_EndCycle
from t_payperiodmaster
where Ppm_PayPeriod = '{0}'

SELECT @MMaxRate = Pmx_ParameterValue
FROM T_ParameterMasterExt
WHERE Pmx_ParameterID = 'MAXRATE'
	AND Pmx_Classification = 'MMAXRATE'
	
SELECT @DMaxRate = Pmx_ParameterValue
FROM T_ParameterMasterExt
WHERE Pmx_ParameterID = 'MAXRATE'
	AND Pmx_Classification = 'DMAXRATE'
	
SELECT @HMaxRate = Pmx_ParameterValue
FROM T_ParameterMasterExt
WHERE Pmx_ParameterID = 'MAXRATE'
	AND Pmx_Classification = 'HMAXRATE'
	
SELECT @MMINRate = Pmx_ParameterValue
FROM T_ParameterMasterExt
WHERE Pmx_ParameterID = 'MINRATE'
	AND Pmx_Classification = 'MMINRATE'
	
SELECT @DMINRate = Pmx_ParameterValue
FROM T_ParameterMasterExt
WHERE Pmx_ParameterID = 'MINRATE'
	AND Pmx_Classification = 'DMINRATE'
	
SELECT @HMINRate = Pmx_ParameterValue
FROM T_ParameterMasterExt
WHERE Pmx_ParameterID = 'MINRATE'
	AND Pmx_Classification = 'HMINRATE'

SELECT @CompTaxSched = CASE WHEN Ccd_TaxSchedule IN (3, 4, 5) AND @PayrollCycle = 2
						THEN 'M'
						ELSE 'S'
						END
FROM T_CompanyMaster
INNER JOIN T_AccountDetail
    ON Adt_AccountCode = Ccd_TaxSchedule
    AND Adt_AccountType = 'TAXBASE'
    AND Adt_Status = 'A'
    
SELECT @TaxApplicPayPeriod = Max(Tsh_PayPeriod)
FROM dbo.T_TaxScheduleHeader
WHERE Tsh_TaxSchedule = @CompTaxSched
    and Tsh_PayPeriod <= @PayrollPeriod
    and Tsh_Status = 'A'
    
SELECT @SSSApplicPayPeriod = Max(Pcm_Payperiod)
FROM dbo.T_PremiumContributionMaster
WHERE Pcm_Payperiod <= @PayrollPeriod           
	and Pcm_DeductionCode = 'SSSPREM'
	and Pcm_Status = 'A'
	
SELECT @PhApplicPayPeriod = Max(Pcm_Payperiod)
FROM dbo.T_PremiumContributionMaster
WHERE Pcm_Payperiod <= @PayrollPeriod           
	and Pcm_DeductionCode = 'PHPREM'
	and Pcm_Status = 'A'

SELECT ROW_NUMBER() OVER (ORDER BY [Remarks], [Last Name], [First Name]) AS Row
	, [Type]
	, [Employee ID]
	, [Last Name]
	, [First Name]
	, [Process Date]
	, [Remarks]
FROM (
SELECT * FROM ( 
-----------------------------------------
--SELECT 'TIMEKEEP MESSAGES'
-----------------------------------------
Select 'ERROR' as [Type] 
		, Ell_EmployeeID as 'Employee ID'
		, Emt_LastName  as 'Last Name'
		, Emt_FirstName as 'First Name'
		, Left(Emt_MiddleName, 1) as MI 
		, null as 'Process Date'
		, 'Restday Not Set-up' as Remarks
From T_EmployeeLogLedger   
INNER JOIN T_EmployeeMaster on Emt_Employeeid = Ell_EmployeeId  
@USERCOSTCENTERACCESSCONDITION
INNER JOIN (
    SELECT Pmx_Classification 
    FROM T_ParameterMasterExt 
    WHERE Pmx_ParameterID = 'EMPSTATPAY'
        AND Pmx_ParameterValue = 1
) EMPSTAT
ON Emt_EmploymentStatus = Pmx_Classification
WHERE Ell_PayPeriod = @PayrollPeriod
    AND Emt_PayrollStatus = 1
    AND  Emt_JobStatus NOT IN ('IN','IM')
    AND (@EmployeeID = '' OR Emt_Employeeid = @EmployeeID)
@CONDITIONS
Group by Ell_EmployeeID  
	, Emt_LastName 
	, Emt_FirstName 
	, Left(Emt_MiddleName, 1) 
Having Sum(Case when Ell_RestDay = 1 then 1 else 0 end) = 0  

UNION  

SELECT 'ERROR' as [Type] 
	, Ell_EmployeeId  as 'Employee ID'
	, Emt_LastName as 'Last Name'
	, Emt_FirstName   as 'First Name'
	, Left(Emt_MiddleName, 1) as MI  
	,convert(char(10), Ell_ProcessDate, 101) as 'Process Date' 
	, RTRIM(Ell_ShiftCode) + Case when Scm_ShiftCode is null then ' - Shift Not Registered' else ' - Invalid Shift Code' + ' ' + RTRIM(Ell_ShiftCode)  end as Remarks   
FROM T_EmployeeLogLedger   
LEFT JOIN T_ShiftCodeMaster ON Scm_ShiftCode = Ell_ShiftCode   
INNER JOIN T_EmployeeMaster on Emt_Employeeid = Ell_EmployeeId   
@USERCOSTCENTERACCESSCONDITION
INNER JOIN (
    SELECT Pmx_Classification 
    FROM T_ParameterMasterExt 
    WHERE Pmx_ParameterID = 'EMPSTATPAY'
        AND Pmx_ParameterValue = 1
) EMPSTAT
ON Emt_EmploymentStatus = Pmx_Classification
WHERE Ell_PayPeriod = @PayrollPeriod
	AND ( Scm_ShiftCode is null or Scm_Status = 'C')   
    AND Emt_PayrollStatus = 1
    AND  Emt_JobStatus NOT IN ('IN','IM')
    AND (@EmployeeID = '' OR Emt_Employeeid = @EmployeeID)
@CONDITIONS

UNION   

SELECT 'ERROR' as [Type] 
	, Ell_EmployeeId as 'Employee ID'   
	, Emt_LastName as 'Last Name'  
	, Emt_FirstName  as 'First Name' 
	, Left(Emt_MiddleName, 1) as MI  
	, convert(char(10), Ell_ProcessDate, 101) as 'Process Date'   
	, RTRIM(Ell_ShiftCode) + ' - Non-Regular Days not 8 Hours' as Remarks  
FROM T_EmployeeLogLedger   
INNER JOIN T_EmployeeMaster on Emt_Employeeid = Ell_EmployeeId  
@USERCOSTCENTERACCESSCONDITION
INNER JOIN (
    SELECT Pmx_Classification 
    FROM T_ParameterMasterExt 
    WHERE Pmx_ParameterID = 'EMPSTATPAY'
        AND Pmx_ParameterValue = 1
) EMPSTAT
ON Emt_EmploymentStatus = Pmx_Classification 
LEFT JOIN T_ShiftCodeMaster ON Scm_ShiftCode = Ell_ShiftCode   
WHERE Ell_PayPeriod = @PayrollPeriod   
	AND Scm_ShiftHours > 8   
	AND Ell_DayCode <>  'REG' 
    AND Ell_ActualTimeIn_1 + Ell_ActualTimeOut_1 + Ell_ActualTimeIn_2 + Ell_ActualTimeOut_2 != '0000000000000000'  
    AND Emt_PayrollStatus = 1
    AND  Emt_JobStatus NOT IN ('IN','IM')
    AND (@EmployeeID = '' OR Emt_Employeeid = @EmployeeID)
@CONDITIONS

UNION   

SELECT 'ERROR' as [Type] 
	, Ell_EmployeeId  as 'Employee ID'  
	, Emt_LastName    as 'Last Name'
	, Emt_FirstName as 'First Name'   
	, Left(Emt_MiddleName, 1) as MI   
	, convert(char(10), Ell_ProcessDate, 101) as 'Process Date'   
	, 'No Work Location Set-up' as Remarks   
FROM T_EmployeeLogLedger   
INNER JOIN T_EmployeeMaster on Emt_Employeeid = Ell_EmployeeId  
@USERCOSTCENTERACCESSCONDITION
INNER JOIN (
    SELECT Pmx_Classification 
    FROM T_ParameterMasterExt 
    WHERE Pmx_ParameterID = 'EMPSTATPAY'
        AND Pmx_ParameterValue = 1
) EMPSTAT
ON Emt_EmploymentStatus = Pmx_Classification 
WHERE Ell_PayPeriod = @PayrollPeriod     
	And (Ell_LocationCode is null or len(rtrim(Ell_LocationCode))=0)  
    AND Emt_PayrollStatus = 1
    AND  Emt_JobStatus NOT IN ('IN','IM')
    AND (@EmployeeID = '' OR Emt_Employeeid = @EmployeeID)
@CONDITIONS

UNION

select 'ERROR' as [Type] 
	, Ell_EmployeeId as 'Employee ID'
	, Emt_LastName as 'Last Name'
	, Emt_FirstName  as 'First Name'  
	, Left(Emt_MiddleName, 1) as MI   
	, convert(char(10), Ell_ProcessDate, 101) as 'Process Date'
	, 'Incorrect Holiday Code' as Remarks
FROM T_EmployeeLogLedger
INNER JOIN T_EmployeeMaster on Emt_Employeeid = Ell_EmployeeId   
@USERCOSTCENTERACCESSCONDITION
INNER JOIN (
    SELECT Pmx_Classification 
    FROM T_ParameterMasterExt 
    WHERE Pmx_ParameterID = 'EMPSTATPAY'
        AND Pmx_ParameterValue = 1
) EMPSTAT
ON Emt_EmploymentStatus = Pmx_Classification
LEFT JOIN T_HolidayMaster on Hmt_HolidayDate = Ell_Processdate
    and (Hmt_ApplicCity = Ell_Locationcode or Hmt_ApplicCity = 'ALL')
INNER JOIN T_DayCodeMaster ON Ell_DayCode = Dcm_DayCode
    AND Dcm_Holiday = 1
where (ell_daycode != Hmt_HolidayCode 
	or (Ell_Holiday = 1 and Hmt_HolidayCode IS NULL))
	and Ell_PayPeriod = @PayrollPeriod
    AND Emt_PayrollStatus = 1
    AND  Emt_JobStatus NOT IN ('IN','IM')
    AND (@EmployeeID = '' OR Emt_Employeeid = @EmployeeID)
@CONDITIONS

UNION

SELECT 'WARNING' as [Type] 
	, Lhe_EmployeeId as 'Employee ID'
	, Emt_LastName as 'Last Name'
	, Emt_FirstName  as 'First Name'  
	, Left(Emt_MiddleName, 1) as MI   
	, convert(char(10), Lhe_ProcessDate, 101) as 'Process Date'
	, Lhe_Remarks
FROM T_LaborHourError
INNER JOIN T_EmployeeMaster
ON Lhe_EmployeeId = Emt_EmployeeID
@USERCOSTCENTERACCESSCONDITION
INNER JOIN (
    SELECT Pmx_Classification 
    FROM T_ParameterMasterExt 
    WHERE Pmx_ParameterID = 'EMPSTATPAY'
        AND Pmx_ParameterValue = 1
) EMPSTAT
ON Emt_EmploymentStatus = Pmx_Classification
WHERE Lhe_Remarks LIKE 'No Actual%'
	AND Lhe_CurrentPayperiod = @PayrollPeriod
    AND Emt_PayrollStatus = 1
    AND  Emt_JobStatus NOT IN ('IN','IM')
    AND (@EmployeeID = '' OR Emt_Employeeid = @EmployeeID)
@CONDITIONS

UNION

SELECT 'ERROR' as [Type] 
	, Lhe_EmployeeId as 'Employee ID'
	, Emt_LastName as 'Last Name'
	, Emt_FirstName  as 'First Name'  
	, Left(Emt_MiddleName, 1) as MI   
	, convert(char(10), Lhe_ProcessDate, 101) as 'Process Date'
	, Lhe_Remarks
FROM T_LaborHourError
INNER JOIN T_EmployeeMaster
ON Lhe_EmployeeId = Emt_EmployeeID
@USERCOSTCENTERACCESSCONDITION
INNER JOIN (
    SELECT Pmx_Classification 
    FROM T_ParameterMasterExt 
    WHERE Pmx_ParameterID = 'EMPSTATPAY'
        AND Pmx_ParameterValue = 1
) EMPSTAT
ON Emt_EmploymentStatus = Pmx_Classification
WHERE Lhe_Remarks NOT LIKE 'No Actual%'
	AND Lhe_CurrentPayperiod = @PayrollPeriod
    AND Emt_PayrollStatus = 1
    AND  Emt_JobStatus NOT IN ('IN','IM')
    AND (@EmployeeID = '' OR Emt_Employeeid = @EmployeeID)
@CONDITIONS

UNION

SELECT 'WARNING' as [Type] 
	, Lhe_EmployeeId as 'Employee ID'
	, Emt_LastName as 'Last Name'
	, Emt_FirstName  as 'First Name'  
	, Left(Emt_MiddleName, 1) as MI   
	, convert(char(10), Lhe_ProcessDate, 101) as 'Process Date'
	, Lhe_Remarks
FROM T_LaborHourErrorHist
INNER JOIN T_EmployeeMaster
ON Lhe_EmployeeId = Emt_EmployeeID
@USERCOSTCENTERACCESSCONDITION
INNER JOIN (
    SELECT Pmx_Classification 
    FROM T_ParameterMasterExt 
    WHERE Pmx_ParameterID = 'EMPSTATPAY'
        AND Pmx_ParameterValue = 1
) EMPSTAT
ON Emt_EmploymentStatus = Pmx_Classification
WHERE Lhe_Remarks LIKE 'No Actual%'
	AND Lhe_CurrentPayperiod = @PayrollPeriod
    AND Emt_PayrollStatus = 1
    AND  Emt_JobStatus NOT IN ('IN','IM')
    AND (@EmployeeID = '' OR Emt_Employeeid = @EmployeeID)
@CONDITIONS

UNION

SELECT 'ERROR' as [Type] 
	, Lhe_EmployeeId as 'Employee ID'
	, Emt_LastName as 'Last Name'
	, Emt_FirstName  as 'First Name'  
	, Left(Emt_MiddleName, 1) as MI   
	, convert(char(10), Lhe_ProcessDate, 101) as 'Process Date'
	, Lhe_Remarks
FROM T_LaborHourErrorHist
INNER JOIN T_EmployeeMaster
ON Lhe_EmployeeId = Emt_EmployeeID
@USERCOSTCENTERACCESSCONDITION
INNER JOIN (
    SELECT Pmx_Classification 
    FROM T_ParameterMasterExt 
    WHERE Pmx_ParameterID = 'EMPSTATPAY'
        AND Pmx_ParameterValue = 1
) EMPSTAT
ON Emt_EmploymentStatus = Pmx_Classification
WHERE Lhe_Remarks NOT LIKE 'No Actual%'
	AND Lhe_CurrentPayperiod = @PayrollPeriod
    AND Emt_PayrollStatus = 1
    AND  Emt_JobStatus NOT IN ('IN','IM')
    AND (@EmployeeID = '' OR Emt_Employeeid = @EmployeeID)
@CONDITIONS

UNION

select 'WARNING' as [Type] 
	, Emt_EmployeeID as 'Employee ID'
	, Emt_LastName as 'Last Name'
	, Emt_FirstName  as 'First Name'  
	, Left(Emt_MiddleName, 1) as MI   
	, ''
	, 'Without logs for the entire pay period' as Remarks
from T_EmployeeMaster
@USERCOSTCENTERACCESSCONDITION
INNER JOIN (
    SELECT Pmx_Classification 
    FROM T_ParameterMasterExt 
    WHERE Pmx_ParameterID = 'EMPSTATPAY'
        AND Pmx_ParameterValue = 1
) EMPSTAT
ON Emt_EmploymentStatus = Pmx_Classification
where  Emt_JobStatus NOT IN ('IN','IM') 
    AND Emt_PayrollStatus = 1
	and Emt_EmployeeID IN (select ell_Employeeid
							from t_employeelogledger
							where Ell_ActualTimeIn_1 + Ell_ActualTimeOut_1 + Ell_ActualTimeIn_2 + Ell_ActualTimeOut_2 = '0000000000000000'
								and ell_payperiod = '{0}'
								and ell_assumedpresent = 0
							group by ell_Employeeid
							having count(ell_Employeeid) = @CurPPCnt
								and sum(Ell_EncodedPayLeaveHr) = 0)
    AND (@EmployeeID = '' OR Emt_Employeeid = @EmployeeID)
@CONDITIONS

) TEMP
WHERE (@Type = '' OR [Type] = @Type)
) TEMP2
", PayPeriod, EmployeeID, TypeFilter);
            query = query.Replace("@CONDITIONS", condition);
            query = query.Replace("@USERCOSTCENTERACCESSCONDITION", (new HRCReportsBL()).UserCostCenterAccessTmpQuery("", "PAYROLL", UserLogin, "Emt_CostcenterCode", "Emt_JobStatus", "Emt_EmploymentStatus", false));
            #endregion
            DataTable dtResult;
            dtResult = dalHelper.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public void UpdateEncodedOvertime(bool ProcessAll, bool ProcessCurrentPeriod, string EmployeeId)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@CurPayPeriod", ProcessPayrollPeriod);

            string HistExtension = "";
            if (!ProcessCurrentPeriod)
                HistExtension = "Hist";

            string EmployeeCondition = "";
            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition = " AND Ell_EmployeeID = '" + EmployeeId + "'";

            #region query
            string query = string.Format(@"UPDATE T_EmployeeLogLedger{0}
                                        SET Ell_EncodedOvertimeAdvHr = 0
                                             , Ell_EncodedOvertimePostHr = 0
                                        WHERE Ell_PayPeriod = @CurPayPeriod  {1}

                                        UPDATE T_EmployeeLogLedger{0}
                                        SET Ell_EncodedOvertimeAdvHr = Isnull(AdvOvtHr,0) 
                                             , Ell_EncodedOvertimePostHr = Isnull(PostOvtHr,0) + Isnull(MidOvtHr,0)
                                        FROM T_EmployeeLogLedger{0}
                                        INNER JOIN (SELECT Eot_EmployeeId
                                                    , Eot_OvertimeDate
                                                    , AdvOvtHr = Sum(Case when Eot_OvertimeType = 'A' Then Eot_OvertimeHour Else 0 End)
                                                    , MidOvtHr = Sum(Case when Eot_OvertimeType = 'M' Then Eot_OvertimeHour Else 0 End)
                                                    , PostOvtHr = Sum(Case when Eot_OvertimeType = 'P' Then Eot_OvertimeHour Else 0 End)
                                                    FROM T_EmployeeOvertime
                                                    WHERE Eot_Status in ('A', '9')
                                                    GROUP BY Eot_EmployeeId
                                                    , Eot_OvertimeDate ) OvtTrn 
                                        ON  Eot_EmployeeId = Ell_EmployeeId
                                            AND Eot_OvertimeDate = Ell_ProcessDate
                                        WHERE Ell_PayPeriod = @CurPayPeriod  {1}

                                        UPDATE T_EmployeeLogLedger{0}
                                        SET Ell_EncodedOvertimeAdvHr = Ell_EncodedOvertimeAdvHr + Isnull(AdvOvtHr,0) 
                                             , Ell_EncodedOvertimePostHr = Ell_EncodedOvertimePostHr + Isnull(PostOvtHr,0) + Isnull(MidOvtHr,0)
                                        FROM T_EmployeeLogLedger{0}
                                        INNER JOIN (SELECT Eot_EmployeeId
                                                    , Eot_OvertimeDate
                                                    , AdvOvtHr = Sum(Case when Eot_OvertimeType = 'A' Then Eot_OvertimeHour Else 0 End)
                                                    , MidOvtHr = Sum(Case when Eot_OvertimeType = 'M' Then Eot_OvertimeHour Else 0 End)
                                                    , PostOvtHr = Sum(Case when Eot_OvertimeType = 'P' Then Eot_OvertimeHour Else 0 End)
                                                    FROM T_EmployeeOvertimeHist
                                                    WHERE Eot_Status in ('A', '9')
                                                    GROUP BY Eot_EmployeeId
                                                    , Eot_OvertimeDate ) OvtTrn 
                                        ON  Eot_EmployeeId = Ell_EmployeeId
                                            AND Eot_OvertimeDate = Ell_ProcessDate
                                        WHERE Ell_PayPeriod = @CurPayPeriod  {1}", HistExtension, EmployeeCondition);
            #endregion
            dal.ExecuteNonQuery(query, CommandType.Text, paramInfo);

            DataSet dsTemp = dal.ExecuteDataSet(string.Format(@"SELECT Ell_EmployeeId, Ell_ProcessDate, Ell_EncodedOvertimeAdvHr, Ell_EncodedOvertimePostHr 
                                                                FROM T_EmployeeLogLedger{0} 
                                                                WHERE Ell_PayPeriod = @CurPayPeriod {1}", HistExtension, EmployeeCondition), CommandType.Text, paramInfo);
        }

        public DataTable GetPayPeriodCycle(string PayPeriod)
        {
            string strQuery = string.Format(@"SELECT	Ppm_PayPeriod
		                                                ,Ppm_StartCycle
		                                                ,Ppm_EndCycle
                                                FROM	T_PayPeriodMaster 
                                                WHERE	Ppm_PayPeriod = '{0}' 
                                                AND		Ppm_Status = 'A'", PayPeriod);

            DataTable dtResult = dal.ExecuteDataSet(strQuery).Tables[0];
            return dtResult;
        }

        public void CheckEncodedPaidLeave(bool ProcessAll, string EmployeeId, bool ProcessCurrentPeriod, DALHelper dalHelper)
        {
            //Get all active leave applications
            ParameterInfo[] paramPayPeriod = new ParameterInfo[2];
            paramPayPeriod[0] = new ParameterInfo("@StartCycle", PayrollStart);
            paramPayPeriod[1] = new ParameterInfo("@EndCycle", PayrollEnd);

            string sqlLeaveList = @"SELECT Elt_EmployeeId
                                          ,Elt_LeaveDate
                                          ,Elt_LeaveType
                                          ,Sum(Elt_LeaveHour) Elt_LeaveHour
                                    FROM
                                    (
                                        SELECT Elt_EmployeeId
                                              ,Elt_LeaveDate
                                              ,Elt_LeaveType
                                              ,Sum(Elt_LeaveHour) Elt_LeaveHour
                                          FROM T_EmployeeLeaveAvailment
                                    INNER JOIN T_LeaveTypeMaster
                                            ON Ltm_LeaveType = Elt_LeaveType
                                           AND Ltm_PaidLeave = 1
                                         WHERE Elt_LeaveDate BETWEEN @StartCycle AND @EndCycle          
                                           AND Elt_Status in ('A', '9', '0')  {0}
                                      GROUP BY Elt_EmployeeId
                                              ,Elt_LeaveDate
                                              ,Elt_LeaveType
	                                    UNION
                                        SELECT Elt_EmployeeId
                                              ,Elt_LeaveDate
                                              ,Elt_LeaveType
                                              ,Sum(Elt_LeaveHour) Elt_LeaveHour
                                          FROM T_EmployeeLeaveAvailmentHist
                                    INNER JOIN T_LeaveTypeMaster
                                            ON Ltm_LeaveType = Elt_LeaveType
                                           AND Ltm_PaidLeave = 1
                                         WHERE Elt_LeaveDate BETWEEN @StartCycle AND @EndCycle         
                                           AND Elt_Status in ('A', '9', '0')  {0}
                                      GROUP BY Elt_EmployeeId
                                              ,Elt_LeaveDate
                                              ,Elt_LeaveType
                                    ) temp
                                    GROUP BY Elt_EmployeeId
		                                    ,Elt_LeaveDate
		                                    ,Elt_LeaveType
                                    ORDER BY Elt_EmployeeId
		                                    ,Elt_LeaveDate";

            if (!ProcessAll && EmployeeId != "")
                sqlLeaveList = string.Format(sqlLeaveList, " AND Elt_EmployeeId = '" + EmployeeId + "'");
            else
                sqlLeaveList = string.Format(sqlLeaveList, "");
            DataTable dtLeaveList = dalHelper.ExecuteDataSet(sqlLeaveList, CommandType.Text, paramPayPeriod).Tables[0];

            //Initialize query to update leave type and hour
            ParameterInfo[] ParamLogLedger = new ParameterInfo[4];
            ParamLogLedger[0] = new ParameterInfo("@Ell_EmployeeId", DBNull.Value);
            ParamLogLedger[1] = new ParameterInfo("@Ell_ProcessDate", DBNull.Value);
            ParamLogLedger[2] = new ParameterInfo("@Ell_EncodedPayLeaveType", DBNull.Value);
            ParamLogLedger[3] = new ParameterInfo("@Ell_EncodedPayLeaveHr", DBNull.Value);

            string sqlUpdateLogLedgerCombined = string.Format(@"UPDATE [T_EmployeeLogLedger]
                                                                 SET [Ell_EncodedPayLeaveType] = [Ltm_LeaveType]
                                                                    ,[Ell_EncodedPayLeaveHr] = @Ell_EncodedPayLeaveHr
                                                                FROM [T_EmployeeLogLedger]
                                                                JOIN [T_LeaveTypeMaster]
                                                                  ON RTRIM([Ltm_LeaveDesc]) = @Ell_EncodedPayLeaveType
                                                               WHERE [Ell_EmployeeId] = @Ell_EmployeeId
                                                                 AND [Ell_ProcessDate] = @Ell_ProcessDate

                                                                UPDATE [T_EmployeeLogLedgerHist]
                                                                 SET [Ell_EncodedPayLeaveType] = [Ltm_LeaveType]
                                                                    ,[Ell_EncodedPayLeaveHr] = @Ell_EncodedPayLeaveHr
                                                                FROM [T_EmployeeLogLedgerHist]
                                                                JOIN [T_LeaveTypeMaster]
                                                                  ON RTRIM([Ltm_LeaveDesc]) = @Ell_EncodedPayLeaveType
                                                               WHERE [Ell_EmployeeId] = @Ell_EmployeeId
                                                                 AND [Ell_ProcessDate] = @Ell_ProcessDate");

            string sqlUpdateLogLedger = string.Format(@"UPDATE [T_EmployeeLogLedger]
                                                         SET [Ell_EncodedPayLeaveType] = @Ell_EncodedPayLeaveType
                                                            ,[Ell_EncodedPayLeaveHr] = @Ell_EncodedPayLeaveHr                                                  
                                                       WHERE [Ell_EmployeeId] = @Ell_EmployeeId
                                                         AND [Ell_ProcessDate] = @Ell_ProcessDate

                                                        UPDATE [T_EmployeeLogLedgerHist]
                                                         SET [Ell_EncodedPayLeaveType] = @Ell_EncodedPayLeaveType
                                                            ,[Ell_EncodedPayLeaveHr] = @Ell_EncodedPayLeaveHr                                                  
                                                       WHERE [Ell_EmployeeId] = @Ell_EmployeeId
                                                         AND [Ell_ProcessDate] = @Ell_ProcessDate");

            //START PROCESSING OF LEAVE TYPE AND HOUR UPDATE
            string EmployeeID = string.Empty;
            DateTime ProcessDate = DateTime.Now;
            ArrayList ListRows = new ArrayList();

            //Added by Rendell Uy - 4/12/2011 (Initialize Leave Type and Hour)
            string sqlInitializeLeaveType = @"UPDATE [T_EmployeeLogLedger]
                                             SET [Ell_EncodedPayLeaveType] = ''
                                                ,[Ell_EncodedPayLeaveHr] = 0
                                             WHERE Ell_PayPeriod = '{0}' {1} 

                                             UPDATE [T_EmployeeLogLedgerHist]
                                             SET [Ell_EncodedPayLeaveType] = ''
                                                ,[Ell_EncodedPayLeaveHr] = 0
                                             WHERE Ell_PayPeriod = '{0}' {1}";
            if (!ProcessAll && EmployeeId != "")
                sqlInitializeLeaveType = string.Format(sqlInitializeLeaveType, ProcessPayrollPeriod, " AND Ell_EmployeeId = '" + EmployeeId + "'");
            else
                sqlInitializeLeaveType = string.Format(sqlInitializeLeaveType, ProcessPayrollPeriod, "");
            dalHelper.ExecuteNonQuery(sqlInitializeLeaveType);

            foreach (DataRow drLeave in dtLeaveList.Rows)
            {
                //something's wrong in this condition; investigate further
                if (EmployeeID != GetValue(drLeave["Elt_EmployeeId"]) || ProcessDate != Convert.ToDateTime(drLeave["Elt_LeaveDate"]))
                {
                    if (ListRows.Count > 1)
                    {
                        string CombinedLeave = string.Empty;
                        decimal TotalLeave = 0;

                        foreach (DataRow drCombinedLeave in ListRows)
                        {
                            ParamLogLedger[0].Value = drCombinedLeave["Elt_EmployeeId"];
                            ParamLogLedger[1].Value = drCombinedLeave["Elt_LeaveDate"];
                            CombinedLeave += " + " + GetValue(drCombinedLeave["Elt_LeaveType"]);
                            TotalLeave += getDecimalValue(drCombinedLeave["Elt_LeaveHour"]);
                        }

                        ParamLogLedger[2].Value = CombinedLeave.Remove(0, 3);
                        ParamLogLedger[3].Value = TotalLeave;

                        dalHelper.ExecuteNonQuery(sqlUpdateLogLedgerCombined, CommandType.Text, ParamLogLedger);
                    }

                    ListRows = new ArrayList();
                }

                if (Convert.ToDouble(drLeave["Elt_LeaveHour"].ToString()) > 0)
                {
                    EmployeeID = GetValue(drLeave["Elt_EmployeeId"]);
                    ProcessDate = Convert.ToDateTime(drLeave["Elt_LeaveDate"]);

                    ParamLogLedger[0].Value = drLeave["Elt_EmployeeId"];
                    ParamLogLedger[1].Value = drLeave["Elt_LeaveDate"];
                    ParamLogLedger[2].Value = drLeave["Elt_LeaveType"];
                    ParamLogLedger[3].Value = drLeave["Elt_LeaveHour"];
                    dalHelper.ExecuteNonQuery(sqlUpdateLogLedger, CommandType.Text, ParamLogLedger);

                    ListRows.Add(drLeave);
                }
            }

            //Set leave type to blank for those records with zero hours
            dalHelper.ExecuteNonQuery(CommonConstants.Queries.updateEncodedPaidLeave);
            //END
        }

        public void CheckEncodedNoPayLeave(bool ProcessAll, string EmployeeId, bool ProcessCurrentPeriod, DALHelper dalHelper)
        {
            //Get all active leave applications
            ParameterInfo[] paramPayPeriod = new ParameterInfo[2];
            paramPayPeriod[0] = new ParameterInfo("@StartCycle", PayrollStart);
            paramPayPeriod[1] = new ParameterInfo("@EndCycle", PayrollEnd);

            string sqlLeaveList1 = @"SELECT Elt_EmployeeId
                                          ,Elt_LeaveDate
                                          ,Elt_LeaveType
                                          ,Sum(Elt_LeaveHour) Elt_LeaveHour
                                    FROM
                                    (
                                        SELECT Elt_EmployeeId
                                              ,Elt_LeaveDate
                                              ,Elt_LeaveType
                                              ,Sum(Elt_LeaveHour) Elt_LeaveHour
                                          FROM T_EmployeeLeaveAvailment
                                    INNER JOIN T_LeaveTypeMaster
                                            ON Ltm_LeaveType = Elt_LeaveType
                                           AND Ltm_PaidLeave = 0
                                         WHERE Elt_LeaveDate BETWEEN @StartCycle AND @EndCycle          
                                           AND Elt_Status in ('A', '9', '0')  {0}
                                      GROUP BY Elt_EmployeeId
                                              ,Elt_LeaveDate
                                              ,Elt_LeaveType
	                                    UNION
                                        SELECT Elt_EmployeeId
                                              ,Elt_LeaveDate
                                              ,Elt_LeaveType
                                              ,Sum(Elt_LeaveHour) Elt_LeaveHour
                                          FROM T_EmployeeLeaveAvailmentHist
                                    INNER JOIN T_LeaveTypeMaster
                                            ON Ltm_LeaveType = Elt_LeaveType
                                           AND Ltm_PaidLeave = 0
                                         WHERE Elt_LeaveDate BETWEEN @StartCycle AND @EndCycle         
                                           AND Elt_Status in ('A', '9', '0')  {0}
                                      GROUP BY Elt_EmployeeId
                                              ,Elt_LeaveDate
                                              ,Elt_LeaveType
                                    ) temp
                                    GROUP BY Elt_EmployeeId
		                                    ,Elt_LeaveDate
		                                    ,Elt_LeaveType
                                    ORDER BY Elt_EmployeeId
		                                    ,Elt_LeaveDate";

            if (!ProcessAll && EmployeeId != "")
                sqlLeaveList1 = string.Format(sqlLeaveList1, " AND Elt_EmployeeId = '" + EmployeeId + "'");
            else
                sqlLeaveList1 = string.Format(sqlLeaveList1, "");
            DataTable dtLeaveList1 = dalHelper.ExecuteDataSet(sqlLeaveList1, CommandType.Text, paramPayPeriod).Tables[0];

            //Initialize query to update leave type and hour
            ParameterInfo[] ParamLogLedger1 = new ParameterInfo[4];
            ParamLogLedger1[0] = new ParameterInfo("@Ell_EmployeeId", DBNull.Value);
            ParamLogLedger1[1] = new ParameterInfo("@Ell_ProcessDate", DBNull.Value);
            ParamLogLedger1[2] = new ParameterInfo("@Ell_EncodedPayLeaveType", DBNull.Value);
            ParamLogLedger1[3] = new ParameterInfo("@Ell_EncodedPayLeaveHr", DBNull.Value);

            string sqlUpdateLogLedgerCombined1 = string.Format(@"UPDATE [T_EmployeeLogLedger]
                                                                 SET [Ell_EncodedNoPayLeaveType] = [Ltm_LeaveType]
                                                                    ,[Ell_EncodedNoPayLeaveHr] = @Ell_EncodedPayLeaveHr
                                                                FROM [T_EmployeeLogLedger]
                                                                JOIN [T_LeaveTypeMaster]
                                                                  ON RTRIM([Ltm_LeaveDesc]) = @Ell_EncodedPayLeaveType
                                                               WHERE [Ell_EmployeeId] = @Ell_EmployeeId
                                                                 AND [Ell_ProcessDate] = @Ell_ProcessDate

                                                                UPDATE [T_EmployeeLogLedgerHist]
                                                                 SET [Ell_EncodedNoPayLeaveType] = [Ltm_LeaveType]
                                                                    ,[Ell_EncodedNoPayLeaveHr] = @Ell_EncodedPayLeaveHr
                                                                FROM [T_EmployeeLogLedgerHist]
                                                                JOIN [T_LeaveTypeMaster]
                                                                  ON RTRIM([Ltm_LeaveDesc]) = @Ell_EncodedPayLeaveType
                                                               WHERE [Ell_EmployeeId] = @Ell_EmployeeId
                                                                 AND [Ell_ProcessDate] = @Ell_ProcessDate");

            string sqlUpdateLogLedger1 = string.Format(@"UPDATE [T_EmployeeLogLedger]
                                                         SET [Ell_EncodedNoPayLeaveType] = @Ell_EncodedPayLeaveType
                                                            ,[Ell_EncodedNoPayLeaveHr] = @Ell_EncodedPayLeaveHr                                                  
                                                       WHERE [Ell_EmployeeId] = @Ell_EmployeeId
                                                         AND [Ell_ProcessDate] = @Ell_ProcessDate

                                                        UPDATE [T_EmployeeLogLedgerHist]
                                                         SET [Ell_EncodedNoPayLeaveType] = @Ell_EncodedPayLeaveType
                                                            ,[Ell_EncodedNoPayLeaveHr] = @Ell_EncodedPayLeaveHr                                                  
                                                       WHERE [Ell_EmployeeId] = @Ell_EmployeeId
                                                         AND [Ell_ProcessDate] = @Ell_ProcessDate");

            //START PROCESSING OF LEAVE TYPE AND HOUR UPDATE
            string EmployeeID1 = string.Empty;
            DateTime ProcessDate1 = DateTime.Now;
            ArrayList ListRows1 = new ArrayList();

            //Added by Rendell Uy - 4/12/2011 (Initialize Leave Type and Hour)
            string sqlInitializeLeaveType = @"UPDATE [T_EmployeeLogLedger]
                                             SET [Ell_EncodedNoPayLeaveType] = ''
                                                ,[Ell_EncodedNoPayLeaveHr] = 0
                                             WHERE Ell_PayPeriod = '{0}' {1} 

                                             UPDATE [T_EmployeeLogLedgerHist]
                                             SET [Ell_EncodedNoPayLeaveType] = ''
                                                ,[Ell_EncodedNoPayLeaveHr] = 0
                                             WHERE Ell_PayPeriod = '{0}' {1}";
            if (!ProcessAll && EmployeeId != "")
                sqlInitializeLeaveType = string.Format(sqlInitializeLeaveType, ProcessPayrollPeriod, " AND Ell_EmployeeId = '" + EmployeeId + "'");
            else
                sqlInitializeLeaveType = string.Format(sqlInitializeLeaveType, ProcessPayrollPeriod, "");
            dalHelper.ExecuteNonQuery(sqlInitializeLeaveType);

            foreach (DataRow drLeave in dtLeaveList1.Rows)
            {
                if (EmployeeID1 != GetValue(drLeave["Elt_EmployeeId"]) || ProcessDate1 != Convert.ToDateTime(drLeave["Elt_LeaveDate"]))
                {
                    if (ListRows1.Count > 1)
                    {
                        string CombinedLeave = string.Empty;
                        decimal TotalLeave = 0;

                        foreach (DataRow drCombinedLeave in ListRows1)
                        {
                            ParamLogLedger1[0].Value = drCombinedLeave["Elt_EmployeeId"];
                            ParamLogLedger1[1].Value = drCombinedLeave["Elt_LeaveDate"];
                            CombinedLeave += " + " + GetValue(drCombinedLeave["Elt_LeaveType"]);
                            TotalLeave += getDecimalValue(drCombinedLeave["Elt_LeaveHour"]);
                        }

                        ParamLogLedger1[2].Value = CombinedLeave.Remove(0, 3);
                        ParamLogLedger1[3].Value = TotalLeave;

                        dalHelper.ExecuteNonQuery(sqlUpdateLogLedgerCombined1, CommandType.Text, ParamLogLedger1);
                    }

                    ListRows1 = new ArrayList();
                }

                if (Convert.ToDouble(drLeave["Elt_LeaveHour"].ToString()) > 0)
                {
                    EmployeeID1 = GetValue(drLeave["Elt_EmployeeId"]);
                    ProcessDate1 = Convert.ToDateTime(drLeave["Elt_LeaveDate"]);

                    ParamLogLedger1[0].Value = drLeave["Elt_EmployeeId"];
                    ParamLogLedger1[1].Value = drLeave["Elt_LeaveDate"];
                    ParamLogLedger1[2].Value = drLeave["Elt_LeaveType"];
                    ParamLogLedger1[3].Value = drLeave["Elt_LeaveHour"];
                    dalHelper.ExecuteNonQuery(sqlUpdateLogLedger1, CommandType.Text, ParamLogLedger1);

                    ListRows1.Add(drLeave);
                }
            }

            //Set leave type to blank for those records with zero hours
            dalHelper.ExecuteNonQuery(CommonConstants.Queries.updateEncodedNoPayLeave);
            //END
        }

        public DataTable GetUserGeneratedPayrollTransactionRecords(bool ProcessAll, string EmployeeId)
        {
            string query = @"SELECT Ept_EmployeeId 
                             FROM {1}
                             WHERE Ept_UserGenerated = 1
                                AND Ept_CurrentPayPeriod = '{2}' {0}";
            if (!ProcessAll && EmployeeId != "")
                query = string.Format(query, " AND Ept_EmployeeID = '" + EmployeeId + "'"
                                            , EmployeePayrollTransactionTable
                                            , ProcessPayrollPeriod);
            else if (ProcessAll == true && EmployeeList != "")
                query = string.Format(query, " AND Ept_EmployeeID IN (" + EmployeeList + ")"
                                            , EmployeePayrollTransactionTable
                                            , ProcessPayrollPeriod);
            else if (ProcessAll == true)
                query = string.Format(query, "", EmployeePayrollTransactionTable, ProcessPayrollPeriod);
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public void ClearTransactionTables(bool ProcessAll, string EmployeeId)
        {
            #region Query
            string query = @"DELETE A
                                FROM {3}Ext A
                                LEFT JOIN {3} B
                                ON A.Ept_EmployeeId = B.Ept_EmployeeId
                                AND A.Ept_CurrentPayPeriod = B.Ept_CurrentPayPeriod
                                WHERE A.Ept_CurrentPayPeriod = '{4}'
                                AND (B.Ept_UserGenerated = 0
                                OR B.Ept_EmployeeId IS NULL) {1}

                             DELETE A
                                FROM {3}Detail A
                                LEFT JOIN {3} B
                                ON A.Ept_EmployeeId = B.Ept_EmployeeId
                                AND A.Ept_CurrentPayPeriod = B.Ept_CurrentPayPeriod
                                WHERE A.Ept_CurrentPayPeriod = '{4}'
                                AND (B.Ept_UserGenerated = 0
                                OR B.Ept_EmployeeId IS NULL) {1}

                             DELETE A
                                FROM {3}ExtDetail A
                                LEFT JOIN {3} B
                                ON A.Ept_EmployeeId = B.Ept_EmployeeId
                                AND A.Ept_CurrentPayPeriod = B.Ept_CurrentPayPeriod
                                WHERE A.Ept_CurrentPayPeriod = '{4}'
                                AND (B.Ept_UserGenerated = 0
                                OR B.Ept_EmployeeId IS NULL) {1}

                             DELETE FROM {3} WHERE (Ept_UserGenerated IS NULL OR Ept_UserGenerated = 0) AND Ept_CurrentPayPeriod = '{4}' {0}

                             DELETE FROM T_LaborHourError WHERE Lhe_CurrentPayPeriod = '{4}' {2}";
            #endregion
            if (!ProcessAll && EmployeeId != "")
                query = string.Format(query, " AND Ept_EmployeeID = '" + EmployeeId + "'"
                                            , " AND A.Ept_EmployeeID = '" + EmployeeId + "'"
                                            , " AND Lhe_EmployeeID = '" + EmployeeId + "'"
                                            , EmployeePayrollTransactionTable
                                            , ProcessPayrollPeriod);
            else if (ProcessAll == true && EmployeeList != "")
                query = string.Format(query, " AND Ept_EmployeeID IN (" + EmployeeList + ")"
                                            , " AND A.Ept_EmployeeID IN (" + EmployeeList + ")"
                                            , " AND Lhe_EmployeeID IN (" + EmployeeList + ")"
                                            , EmployeePayrollTransactionTable
                                            , ProcessPayrollPeriod);
            else if (ProcessAll == true)
                query = string.Format(query, "", "", "", EmployeePayrollTransactionTable, ProcessPayrollPeriod);
            dal.ExecuteNonQuery(query);
        }

        public void ClearTransactionHistTables(bool ProcessAll, string EmployeeId, string PayPeriod)
        {
            #region Query
            string query = @"DELETE A
                                FROM {3}Ext A
                                LEFT JOIN {3} B
                                ON A.Ept_EmployeeId = B.Ept_EmployeeId
                                AND A.Ept_CurrentPayPeriod = B.Ept_CurrentPayPeriod
                                WHERE A.Ept_CurrentPayPeriod = '{4}'
                                {1}

                             DELETE A
                                FROM {3}Detail A
                                LEFT JOIN {3} B
                                ON A.Ept_EmployeeId = B.Ept_EmployeeId
                                AND A.Ept_CurrentPayPeriod = B.Ept_CurrentPayPeriod
                                WHERE A.Ept_CurrentPayPeriod = '{4}'
                                {1}

                             DELETE A
                                FROM {3}ExtDetail A
                                LEFT JOIN {3} B
                                ON A.Ept_EmployeeId = B.Ept_EmployeeId
                                AND A.Ept_CurrentPayPeriod = B.Ept_CurrentPayPeriod
                                WHERE A.Ept_CurrentPayPeriod = '{4}'
                                {1}

                             DELETE FROM {3} WHERE Ept_CurrentPayPeriod = '{4}' {0}

                             --DELETE FROM T_LaborHourError WHERE Lhe_CurrentPayPeriod = '{4}' {2}";
            #endregion
            if (!ProcessAll && EmployeeId != "")
                query = string.Format(query, " AND Ept_EmployeeID = '" + EmployeeId + "'"
                                            , " AND A.Ept_EmployeeID = '" + EmployeeId + "'"
                                            , " AND Lhe_EmployeeID = '" + EmployeeId + "'"
                                            , EmployeePayrollTransactionTable
                                            , PayPeriod);
            else if (ProcessAll == true && EmployeeList != "")
                query = string.Format(query, " AND Ept_EmployeeID IN (" + EmployeeList + ")"
                                            , " AND A.Ept_EmployeeID IN (" + EmployeeList + ")"
                                            , " AND Lhe_EmployeeID IN (" + EmployeeList + ")"
                                            , EmployeePayrollTransactionTable
                                            , PayPeriod);
            else if (ProcessAll == true)
                query = string.Format(query, "", "", "", EmployeePayrollTransactionTable, PayPeriod);
            dal.ExecuteNonQuery(query);
        }

        public void ClearTransactionTrailTables(bool ProcessAll, string EmployeeId, string PayPeriod)
        {
            string EmployeeCondition = "";
            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition += " AND Ept_EmployeeID = '" + EmployeeId + "'";

            string query = string.Format(@" DELETE FROM T_EmployeePayrollTransactionTrail WHERE Ept_CurrentPayPeriod = '{0}' AND Ept_AdjustPayPeriod = '{1}' {2}
                                            DELETE FROM T_EmployeePayrollTransactionTrailExt WHERE Ept_CurrentPayPeriod = '{0}' AND Ept_AdjustPayPeriod = '{1}' {2}
                                            DELETE FROM T_EmployeePayrollTransactionTrailDetail WHERE Ept_CurrentPayPeriod = '{0}' AND Ept_AdjustPayPeriod = '{1}' {2}
                                            DELETE FROM T_EmployeePayrollTransactionTrailExtDetail WHERE Ept_CurrentPayPeriod = '{0}' AND Ept_AdjustPayPeriod = '{1}' {2}", PayPeriod, AdjustPayrollPeriod, EmployeeCondition);
            dal.ExecuteNonQuery(query);
        }

        public string GetPrevPayPeriod(string Ppm_Payperiod, DALHelper dal)
        {
            DataSet ds = new DataSet();
            string _prevpayperiod = string.Empty;

            #region query
            string qString = string.Format(@"Select Ppm_PayPeriod 
                                            From T_PayPeriodMaster 
                                            Where Ppm_EndCycle = (Select dateadd(dd, -1, Ppm_StartCycle)
                                                                From T_PayPeriodMaster
                                                                Where  Ppm_PayPeriod = '{0}'
						                                            and Ppm_Status = 'A'
						                                            and Right(Ppm_Payperiod,1) in ('1','2'))
                                            And  Ppm_Status = 'A'
                                            And Right(Ppm_Payperiod,1) in ('1','2')", Ppm_Payperiod);
            #endregion

            ds = dal.ExecuteDataSet(qString);

            if (ds.Tables[0].Rows.Count != 0)
                _prevpayperiod = ds.Tables[0].Rows[0]["Ppm_PayPeriod"].ToString();

            return _prevpayperiod;
        }

        public void CleanUpBeforeGeneration(bool ProcessAll, string EmployeeId)
        {
            string qString = string.Empty;
            #region query
            qString = string.Format(@"UPDATE {0}
                                         Set        Ell_ConvertedTimeIn_1Min           = 0
                                                    , Ell_ConvertedTimeOut_1Min        = 0
                                                    , Ell_ConvertedTimeIn_2Min         = 0
                                                    , Ell_ConvertedTimeOut_2Min        = 0
                                                    , Ell_ComputedTimeIn_1Min          = 0
                                                    , Ell_ComputedTimeOut_1Min         = 0
                                                    , Ell_ComputedTimeIn_2Min          = 0
                                                    , Ell_ComputedTimeOut_2Min         = 0
                                                    , Ell_AdjustShiftMin               = 0
                                                    , Ell_ShiftTimeIn_1Min             = 0
                                                    , Ell_ShiftTimeOut_1Min            = 0
                                                    , Ell_ShiftTimeIn_2Min             = 0
                                                    , Ell_ShiftTimeOut_2Min            = 0
                                                    , Ell_ShiftMin                     = 0
                                                    , Ell_ScheduleType                 = ''
                                                    , Ell_PayLeaveMin                  = 0
                                                    , Ell_ExcessLeaveMin               = 0
                                                    , Ell_EncodedOvertimeMin           = 0
                                                    , Ell_ComputedOvertimeMin          = 0
                                                    , Ell_OffsetOvertimeMin            = 0
                                                    , Ell_ComputedLateMin              = 0
                                                    , Ell_LatePost                     = 0
                                                    , Ell_InitialAbsentMin             = 0
                                                    , Ell_ComputedAbsentMin            = 0
                                                    , Ell_ComputedRegularMin           = 0
                                                    , Ell_ComputedDayWorkMin           = 0
                                                    , Ell_ComputedRegularNightPremMin  = 0
                                                    , Ell_ComputedOvertimeNightPremMin = 0
                                                    , Ell_PreviousDayWorkMin           = 0
                                                    , Ell_PreviousDayHolidayReference  = null
                                                    , Ell_ExcessOffset                 = 0
                                                    , Ell_EarnedSatOff                 = 0
                                                    , Ell_SundayHolidayCount           = 0
                                                    , Ell_WorkingDay                   = 0
                                                    , Ell_MealDay                      = 0
                                                    , Ell_ExpectedHour                 = 0.00
                                                    , Ell_AbsentHour                   = 0.00
                                                    , Ell_RegularHour                  = 0.00
                                                    , Ell_OvertimeHour                 = 0.00
                                                    , Ell_RegularNightPremHour         = 0.00
                                                    , Ell_OvertimeNightPremHour        = 0.00
                                                    , Ell_LeaveHour                    = 0.00
                                                    , Ell_ForwardedNextDayHour         = 0.00
                                                    , Ell_AllowanceAmt01               = 0.00
                                                    , Ell_AllowanceAmt02               = 0.00
                                                    , Ell_AllowanceAmt03               = 0.00
                                                    , Ell_AllowanceAmt04               = 0.00
                                                    , Ell_AllowanceAmt05               = 0.00
                                                    , Ell_AllowanceAmt06               = 0.00
                                                    , Ell_AllowanceAmt07               = 0.00
                                                    , Ell_AllowanceAmt08               = 0.00
                                                    , Ell_AllowanceAmt09               = 0.00
                                                    , Ell_AllowanceAmt10               = 0.00
                                                    , Ell_AllowanceAmt11               = 0.00
                                                    , Ell_AllowanceAmt12               = 0.00
                                                    , Ell_ComputedLate2Min = 0.00
                                                    , Ell_ComputedUndertimeMin = 0.00
                                                    , Ell_ComputedUndertime2Min = 0.00
                                            WHERE  Ell_PayPeriod = '{1}'", EmployeeLogLedgerTable, ProcessPayrollPeriod);
            #endregion
            if (!ProcessAll && EmployeeId != "")
                qString += " AND Ell_EmployeeID = '" + EmployeeId + "'";
            else if (ProcessAll == true && EmployeeList != "")
                qString += " AND Ell_EmployeeID IN (" + EmployeeList + ")";
            dal.ExecuteNonQuery(qString);
        }

        private string GetPreLaborHoursFormula()
        {
            using (DALHelper dal = new DALHelper(MainDB, false))
            {
                string query = string.Format(@"SELECT Frm_Formula
                                                FROM T_FormulaMaster
                                                WHERE Frm_MenuCode = '{0}'
	                                                AND Frm_SubCode = '{1}'", "PAYROLLCALC", "PRELABRHRS");
                DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
                string strFormula = "";
                if (dtResult.Rows.Count > 0)
                {
                    strFormula = dtResult.Rows[0][0].ToString();
                }
                return strFormula;
            }
        }

        public DataTable GetAllEmployeeForProcess(bool ProcessAll, string EmployeeId, bool IsFlexShift)
        {
            string EmployeeCondition = "";
            string PayPeriodCondition = "";
            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition = " AND Ell_EmployeeID = '" + EmployeeId + "' ";
            else if (ProcessAll == true && EmployeeList != "")
                EmployeeCondition = " AND Ell_EmployeeID IN (" + EmployeeList + ") ";
            if (bProcessTrail)
                PayPeriodCondition = " AND Ell_AdjustPayPeriod = '" + AdjustPayrollPeriod + "'";

            string query = "";
            if (IsFlexShift == false)
            {
                #region Normal Query
                query = string.Format(@"Select Ell_EmployeeId
                                            , Ell_ProcessDate
                                            , Ell_PayPeriod
                                            , Ell_DayCode
                                            , Ell_ShiftCode
                                            , Ell_Holiday
                                            , Ell_RestDay
                                            , Ell_ActualTimeIn_1
                                            , Ell_ActualTimeOut_1
                                            , Ell_ActualTimeIn_2
                                            , Ell_ActualTimeOut_2
                                            , Ell_ConvertedTimeIn_1Min
                                            , Ell_ConvertedTimeOut_1Min
                                            , Ell_ConvertedTimeIn_2Min
                                            , Ell_ConvertedTimeOut_2Min
                                            , Ell_ComputedTimeIn_1Min
                                            , Ell_ComputedTimeOut_1Min
                                            , Ell_ComputedTimeIn_2Min
                                            , Ell_ComputedTimeOut_2Min
                                            , Ell_AdjustShiftMin
                                            , Ell_ShiftTimeIn_1Min
                                            , Ell_ShiftTimeOut_1Min
                                            , Ell_ShiftTimeIn_2Min
                                            , Ell_ShiftTimeOut_2Min
                                            , Ell_ShiftMin
                                            , Ell_ScheduleType
                                            , Ell_EncodedPayLeaveType
                                            , Ell_EncodedPayLeaveHr
                                            , Ell_PayLeaveMin
                                            , Ell_ExcessLeaveMin
                                            , Ell_EncodedNoPayLeaveType
                                            , Ell_EncodedNoPayLeaveHr
                                            , Ell_NoPayLeaveMin
                                            , Ell_EncodedOvertimeAdvHr
                                            , Ell_EncodedOvertimePostHr
                                            , Ell_EncodedOvertimeMin
                                            , Ell_ComputedOvertimeMin
                                            , Ell_OffsetOvertimeMin
                                            , Ell_ComputedLateMin
                                            , Ell_LatePost
                                            , Ell_InitialAbsentMin
                                            , Ell_ComputedAbsentMin
                                            , Ell_ComputedRegularMin
                                            , Ell_ComputedDayWorkMin
                                            , Ell_ComputedRegularNightPremMin
                                            , Ell_ComputedOvertimeNightPremMin
                                            , Ell_PreviousDayWorkMin
                                            , Ell_PreviousDayHolidayReference
                                            , Ell_GraveyardPost
                                            , Ell_GraveyardPostBy
                                            , Ell_GraveyardPostDate
                                            , Ell_AssumedPresent
                                            , Ell_AssumedPresentBy
                                            , Ell_AssumedPresentDate
                                            , Ell_ForceLeave
                                            , Ell_ForceLeaveBy
                                            , Ell_ForceLeaveDate
                                            , Ell_ForOffsetMin
                                            , Ell_ExcessOffset
                                            , Ell_EarnedSatOff
                                            , Ell_SundayHolidayCount
                                            , Ell_WorkingDay
                                            , Ell_MealDay
                                            , Ell_ExpectedHour
                                            , Ell_AbsentHour
                                            , Ell_RegularHour
                                            , Ell_OvertimeHour
                                            , Ell_RegularNightPremHour
                                            , Ell_OvertimeNightPremHour
                                            , Ell_LeaveHour
                                            , Ell_ForwardedNextDayHour
                                            , Ell_AllowanceAmt01
                                            , Ell_AllowanceAmt02
                                            , Ell_AllowanceAmt03
                                            , Ell_AllowanceAmt04
                                            , Ell_AllowanceAmt05
                                            , Ell_AllowanceAmt06
                                            , Ell_AllowanceAmt07
                                            , Ell_AllowanceAmt08
                                            , Ell_AllowanceAmt09
                                            , Ell_AllowanceAmt10
                                            , Ell_AllowanceAmt11
                                            , Ell_AllowanceAmt12
                                            , Ell_LocationCode
                                            , Ell_Flex
                                            , Ell_TagFlex
                                            , Ell_TagTimeMod
                                            , Ell_WorkType
                                            , Ell_WorkGroup
                                            , Ell_AssumedPostBack
                                            , ISNULL(Ell_ComputedLate2Min, 0) AS Ell_ComputedLate2Min
                                            , ISNULL(Ell_ComputedUndertimeMin, 0) AS Ell_ComputedUndertimeMin
                                            , ISNULL(Ell_ComputedUndertime2Min, 0) AS Ell_ComputedUndertime2Min
                                            --, Ell_InitialOffsetMin
                                            --, Ell_AppliedOffsetMin
                                            --, Ell_ComputedOffsetMin
                                            , Emt_LastName
                                            , Emt_FirstName
                                            , Emt_MiddleName
                                            , Emt_EmploymentStatus
                                            , Emt_LocationCode
                                            , Emt_PositionCode
                                            , Emt_PayrollStatus
                                            , Emt_PayrollType
                                            , Emt_JobStatus
                                            , Emt_JobLevel
                                            , Emt_SeparationEffectivityDate
                                            , ISNULL(Scm_ScheduleType, '') as Scm_ScheduleType
                                            , ISNULL(Scm_ShiftTimeIn, '0000') as Scm_ShiftTimeIn
                                            , ISNULL(Scm_PadShiftTimeIn, 0) as Scm_PadShiftTimeIn 
                                            , ISNULL(Scm_ShiftBreakStart, '0000') as Scm_ShiftBreakStart
                                            , ISNULL(Scm_PadShiftBreakStart, 0) as Scm_PadShiftBreakStart
                                            , ISNULL(Scm_ShiftBreakEnd, '0000') as Scm_ShiftBreakEnd
                                            , ISNULL(Scm_PadShiftBreakEnd, 0) as Scm_PadShiftBreakEnd
                                            , ISNULL(Scm_ShiftTimeOut, '0000') as Scm_ShiftTimeOut
                                            , ISNULL(Scm_PadShiftTimeOut, 0) as Scm_PadShiftTimeOut
                                            , ISNULL(Scm_ShiftHours, 0) as Scm_ShiftHours
                                            , ISNULL(Scm_EquivalentShiftCode, '') as Scm_EquivalentShiftCode
                                            , ISNULL(Scm_PaidBreak, 0) as Scm_PaidBreak
                                            , ISNULL(Scm_HourFracCutoff, '') as Scm_HourFracCutoff
                                        From {2} 
                                        Inner join T_EmployeeMaster 
                                            on Emt_EmployeeID = Ell_EmployeeID
                                        INNER JOIN (
                                            SELECT Pmx_Classification 
                                            FROM T_ParameterMasterExt 
                                            WHERE Pmx_ParameterID = 'EMPSTATPAY'
                                                AND Pmx_ParameterValue = 1
                                        ) EMPSTAT
                                            ON Emt_EmploymentStatus = Pmx_Classification
                                        Left Join T_ShiftCodeMaster
	                                        on Ell_ShiftCode = Scm_ShiftCode
                                        Where Ell_PayPeriod = '{0}' {1} {3}
                                        Order By Ell_EmployeeID, Ell_ProcessDate", ProcessPayrollPeriod, EmployeeCondition, EmployeeLogLedgerTable, PayPeriodCondition);
                #endregion
            }
            else
            {
                #region Flex Shift Query
                query = string.Format(@"IF OBJECT_ID('tempdb..#ShiftCodeMaster') IS NOT NULL
                                           DROP TABLE #ShiftCodeMaster

                                        SELECT ISNULL(PARSENAME(REPLACE(CUR.Scm_ShiftCode, '-', '.'),2), CUR.Scm_ShiftCode) AS FlexCode
	                                        , CUR.Scm_ShiftCode AS CurShiftCode 
	                                        , CUR.Scm_ShiftTimeIn AS CurTimeIN
	                                        , PREV.Scm_ShiftCode AS PrevShiftCode
	                                        , PREV.Scm_ShiftTimeIn AS PrevTimeIN
	                                        , NEX.Scm_ShiftCode AS NextShiftCode
	                                        , NEX.Scm_ShiftTimeIn AS NextTimeIN
	                                        , ISNULL((SELECT TOP 1 Scm_ShiftCode
		                                        FROM T_ShiftCodeMaster
		                                        WHERE Scm_DefaultShift = 1
			                                        AND Scm_Status = 'A'
			                                        AND ISNULL(PARSENAME(REPLACE(Scm_ShiftCode, '-', '.'),2), Scm_ShiftCode) 
				                                        = ISNULL(PARSENAME(REPLACE(CUR.Scm_ShiftCode, '-', '.'),2), CUR.Scm_ShiftCode)), CUR.Scm_ShiftCode) AS DefaultShiftCode
                                        INTO #ShiftCodeMaster
                                        FROM T_ShiftCodeMaster CUR
                                        LEFT JOIN T_ShiftCodeMaster PREV
                                        ON PREV.Scm_ShiftCode = (SELECT MAX(Scm_ShiftCode) FROM T_ShiftCodeMaster WHERE Scm_ShiftCode < CUR.Scm_ShiftCode)
	                                        AND PARSENAME(REPLACE(CUR.Scm_ShiftCode, '-', '.'),2) = PARSENAME(REPLACE(PREV.Scm_ShiftCode, '-', '.'),2)
	                                        AND PREV.Scm_Status = 'A'
                                        LEFT JOIN T_ShiftCodeMaster NEX
                                        ON NEX.Scm_ShiftCode = (SELECT MIN(Scm_ShiftCode) FROM T_ShiftCodeMaster WHERE Scm_ShiftCode > CUR.Scm_ShiftCode)
	                                        AND PARSENAME(REPLACE(CUR.Scm_ShiftCode, '-', '.'),2) = PARSENAME(REPLACE(NEX.Scm_ShiftCode, '-', '.'),2)
	                                        AND NEX.Scm_Status = 'A'
                                        WHERE CUR.Scm_Status = 'A'

                                        Select Ell_EmployeeId
                                            , Ell_ProcessDate
                                            , Ell_PayPeriod
                                            , Ell_DayCode
                                            , COALESCE(Flex.Scm_ShiftCode, Ell_ShiftCode) as Ell_ShiftCode
                                            , Ell_Holiday
                                            , Ell_RestDay
                                            , Ell_ActualTimeIn_1
                                            , Ell_ActualTimeOut_1
                                            , Ell_ActualTimeIn_2
                                            , Ell_ActualTimeOut_2
                                            , Ell_ConvertedTimeIn_1Min
                                            , Ell_ConvertedTimeOut_1Min
                                            , Ell_ConvertedTimeIn_2Min
                                            , Ell_ConvertedTimeOut_2Min
                                            , Ell_ComputedTimeIn_1Min
                                            , Ell_ComputedTimeOut_1Min
                                            , Ell_ComputedTimeIn_2Min
                                            , Ell_ComputedTimeOut_2Min
                                            , Ell_AdjustShiftMin
                                            , Ell_ShiftTimeIn_1Min
                                            , Ell_ShiftTimeOut_1Min
                                            , Ell_ShiftTimeIn_2Min
                                            , Ell_ShiftTimeOut_2Min
                                            , Ell_ShiftMin
                                            , Ell_ScheduleType
                                            , Ell_EncodedPayLeaveType
                                            , Ell_EncodedPayLeaveHr
                                            , Ell_PayLeaveMin
                                            , Ell_ExcessLeaveMin
                                            , Ell_EncodedNoPayLeaveType
                                            , Ell_EncodedNoPayLeaveHr
                                            , Ell_NoPayLeaveMin
                                            , Ell_EncodedOvertimeAdvHr
                                            , Ell_EncodedOvertimePostHr
                                            , Ell_EncodedOvertimeMin
                                            , Ell_ComputedOvertimeMin
                                            , Ell_OffsetOvertimeMin
                                            , Ell_ComputedLateMin
                                            , Ell_LatePost
                                            , Ell_InitialAbsentMin
                                            , Ell_ComputedAbsentMin
                                            , Ell_ComputedRegularMin
                                            , Ell_ComputedDayWorkMin
                                            , Ell_ComputedRegularNightPremMin
                                            , Ell_ComputedOvertimeNightPremMin
                                            , Ell_PreviousDayWorkMin
                                            , Ell_PreviousDayHolidayReference
                                            , Ell_GraveyardPost
                                            , Ell_GraveyardPostBy
                                            , Ell_GraveyardPostDate
                                            , Ell_AssumedPresent
                                            , Ell_AssumedPresentBy
                                            , Ell_AssumedPresentDate
                                            , Ell_ForceLeave
                                            , Ell_ForceLeaveBy
                                            , Ell_ForceLeaveDate
                                            , Ell_ForOffsetMin
                                            , Ell_ExcessOffset
                                            , Ell_EarnedSatOff
                                            , Ell_SundayHolidayCount
                                            , Ell_WorkingDay
                                            , Ell_MealDay
                                            , Ell_ExpectedHour
                                            , Ell_AbsentHour
                                            , Ell_RegularHour
                                            , Ell_OvertimeHour
                                            , Ell_RegularNightPremHour
                                            , Ell_OvertimeNightPremHour
                                            , Ell_LeaveHour
                                            , Ell_ForwardedNextDayHour
                                            , Ell_AllowanceAmt01
                                            , Ell_AllowanceAmt02
                                            , Ell_AllowanceAmt03
                                            , Ell_AllowanceAmt04
                                            , Ell_AllowanceAmt05
                                            , Ell_AllowanceAmt06
                                            , Ell_AllowanceAmt07
                                            , Ell_AllowanceAmt08
                                            , Ell_AllowanceAmt09
                                            , Ell_AllowanceAmt10
                                            , Ell_AllowanceAmt11
                                            , Ell_AllowanceAmt12
                                            , Ell_LocationCode
                                            , Ell_Flex
                                            , Ell_TagFlex
                                            , Ell_TagTimeMod
                                            , Ell_WorkType
                                            , Ell_WorkGroup
                                            , Ell_AssumedPostBack
                                            , ISNULL(Ell_ComputedLate2Min, 0) AS Ell_ComputedLate2Min
                                            , ISNULL(Ell_ComputedUndertimeMin, 0) AS Ell_ComputedUndertimeMin
                                            , ISNULL(Ell_ComputedUndertime2Min, 0) AS Ell_ComputedUndertime2Min
                                            --, Ell_InitialOffsetMin
                                            --, Ell_AppliedOffsetMin
                                            --, Ell_ComputedOffsetMin
                                            , Emt_LastName
                                            , Emt_FirstName
                                            , Emt_MiddleName
                                            , Emt_EmploymentStatus
                                            , Emt_LocationCode
                                            , Emt_PositionCode
                                            , Emt_PayrollStatus
                                            , Emt_PayrollType
                                            , Emt_JobStatus
                                            , Emt_JobLevel
                                            , Emt_SeparationEffectivityDate
                                            , COALESCE(Flex.Scm_ScheduleType, Normal.Scm_ScheduleType, '') as Scm_ScheduleType
                                            , COALESCE(Flex.Scm_ShiftTimeIn, Normal.Scm_ShiftTimeIn, '0000') as Scm_ShiftTimeIn
                                            , COALESCE(Flex.Scm_PadShiftTimeIn, Normal.Scm_PadShiftTimeIn, 0) as Scm_PadShiftTimeIn 
                                            , COALESCE(Flex.Scm_ShiftBreakStart, Normal.Scm_ShiftBreakStart, '0000') as Scm_ShiftBreakStart
                                            , COALESCE(Flex.Scm_PadShiftBreakStart, Normal.Scm_PadShiftBreakStart, 0) as Scm_PadShiftBreakStart
                                            , COALESCE(Flex.Scm_ShiftBreakEnd, Normal.Scm_ShiftBreakEnd, '0000') as Scm_ShiftBreakEnd
                                            , COALESCE(Flex.Scm_PadShiftBreakEnd, Normal.Scm_PadShiftBreakEnd, 0) as Scm_PadShiftBreakEnd
                                            , COALESCE(Flex.Scm_ShiftTimeOut, Normal.Scm_ShiftTimeOut, '0000') as Scm_ShiftTimeOut
                                            , COALESCE(Flex.Scm_PadShiftTimeOut, Normal.Scm_PadShiftTimeOut, 0) as Scm_PadShiftTimeOut
                                            , COALESCE(Flex.Scm_ShiftHours, Normal.Scm_ShiftHours, 0) as Scm_ShiftHours
                                            , COALESCE(Flex.Scm_EquivalentShiftCode, Normal.Scm_EquivalentShiftCode, '') as Scm_EquivalentShiftCode
                                            , COALESCE(Flex.Scm_PaidBreak, Normal.Scm_PaidBreak, 0) as Scm_PaidBreak
                                            , COALESCE(Flex.Scm_HourFracCutoff, Normal.Scm_HourFracCutoff, '') as Scm_HourFracCutoff
                                        From {2} 
                                        Inner join T_EmployeeMaster 
                                            on Emt_EmployeeID = Ell_EmployeeID
                                        INNER JOIN (
                                            SELECT Pmx_Classification 
                                            FROM T_ParameterMasterExt 
                                            WHERE Pmx_ParameterID = 'EMPSTATPAY'
                                                AND Pmx_ParameterValue = 1
                                        ) EMPSTAT
                                            ON Emt_EmploymentStatus = Pmx_Classification
                                        Left Join #ShiftCodeMaster Temp
                                            ON (((CASE WHEN Ell_ActualTimeIn_1 = '0000' AND Ell_ActualTimeOut_1 = '0000' THEN Ell_ActualTimeIn_2 ELSE Ell_ActualTimeIn_1 END) <= CurTimeIN 
												AND ((CASE WHEN Ell_ActualTimeIn_1 = '0000' AND Ell_ActualTimeOut_1 = '0000' THEN Ell_ActualTimeIn_2 ELSE Ell_ActualTimeIn_1 END) > PrevTimeIN
													OR PrevTimeIN IS NULL))
											OR ((CASE WHEN Ell_ActualTimeIn_1 = '0000' AND Ell_ActualTimeOut_1 = '0000' THEN Ell_ActualTimeIn_2 ELSE Ell_ActualTimeIn_1 END) > CurTimeIN
												AND NextTimeIN IS NULL))
	                                        AND ISNULL(PARSENAME(REPLACE(Ell_ShiftCode, '-', '.'),2),Ell_ShiftCode) = FlexCode
										Left Join T_ShiftCodeMaster Normal
	                                        on Normal.Scm_ShiftCode = Ell_ShiftCode
										Left Join T_ShiftCodeMaster Flex
	                                        on Flex.Scm_ShiftCode = CASE WHEN (CASE WHEN Ell_ActualTimeIn_1 = '0000' AND Ell_ActualTimeOut_1 = '0000' THEN Ell_ActualTimeIn_2 ELSE Ell_ActualTimeIn_1 END) = '0000'
																	THEN Ell_ShiftCode
                                                                    WHEN Temp.NextShiftCode IS NULL AND (CASE WHEN Ell_ActualTimeIn_1 = '0000' AND Ell_ActualTimeOut_1 = '0000' THEN Ell_ActualTimeIn_2 ELSE Ell_ActualTimeIn_1 END) > CurTimeIN
																	THEN Temp.DefaultShiftCode
																	ELSE Temp.CurShiftCode
																	END
                                        Where Ell_PayPeriod = '{0}' {1} {3}
                                        Order By Ell_EmployeeID, Ell_ProcessDate", ProcessPayrollPeriod, EmployeeCondition, EmployeeLogLedgerTable, PayPeriodCondition);
                #endregion
            }
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public DataTable GetLogLedgerExtensionRecords(bool ProcessAll, string EmployeeId)
        {
            string EmployeeCondition = "";
            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition = " AND Ell_EmployeeID = '" + EmployeeId + "' ";
            else if (ProcessAll == true && EmployeeList != "")
                EmployeeCondition = " AND Ell_EmployeeID IN (" + EmployeeList + ") ";
            #region query
            string query = string.Format(@"SELECT Ell_EmployeeId
                                              ,Ell_ProcessDate
                                              ,Ell_PayPeriod
                                              ,Ell_ActualTimeIn_01
                                              ,Ell_ActualTimeOut_01
                                              ,Ell_ActualTimeIn_02
                                              ,Ell_ActualTimeOut_02
                                              ,Ell_ActualTimeIn_03
                                              ,Ell_ActualTimeOut_03
                                              ,Ell_ActualTimeIn_04
                                              ,Ell_ActualTimeOut_04
                                              ,Ell_ActualTimeIn_05
                                              ,Ell_ActualTimeOut_05
                                              ,Ell_ActualTimeIn_06
                                              ,Ell_ActualTimeOut_06
                                              ,Ell_ActualTimeIn_07
                                              ,Ell_ActualTimeOut_07
                                              ,Ell_ActualTimeIn_08
                                              ,Ell_ActualTimeOut_08
                                              ,Ell_ActualTimeIn_09
                                              ,Ell_ActualTimeOut_09
                                              ,Ell_ActualTimeIn_10
                                              ,Ell_ActualTimeOut_10
                                              ,A.Usr_Login
                                              ,A.Ludatetime
                                          FROM {2} A
                                            Inner join T_EmployeeMaster 
                                                on Emt_EmployeeID = Ell_EmployeeID
                                            INNER JOIN (
                                                SELECT Pmx_Classification 
                                                FROM T_ParameterMasterExt 
                                                WHERE Pmx_ParameterID = 'EMPSTATPAY'
                                                    AND Pmx_ParameterValue = 1
                                            ) EMPSTAT
                                                ON Emt_EmploymentStatus = Pmx_Classification
                                          Where Ell_PayPeriod = '{0}' {1} 
                                          Order By Ell_EmployeeID, Ell_ProcessDate", ProcessPayrollPeriod, EmployeeCondition, EmployeeLogLedgerExtTable);
            #endregion
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public DataTable GetAllOvertimeRecords(bool ProcessAll, string EmployeeId, bool bIsFlex)
        {
            string EmployeeCondition = "";
            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition = " AND Eot_EmployeeID = '" + EmployeeId + "' ";
            else if (ProcessAll == true && EmployeeList != "")
                EmployeeCondition = " AND Eot_EmployeeID IN (" + EmployeeList + ") ";

            string query = "";
            #region query
            if (bIsFlex == false)
            {
                query = string.Format(@"DECLARE @CurPeriod AS CHAR(7)
                                       SET @CurPeriod = (SELECT Ppm_PayPeriod FROM T_PayPeriodMaster WHERE Ppm_CycleIndicator = 'C')

                                       SELECT DISTINCT Eot_EmployeeId, Eot_StartTime, Eot_EndTime, Eot_CurrentPayPeriod, Eot_OvertimeDate, Eot_OvertimeType, Eot_OvertimeHour
                                       FROM T_EmployeeOvertime 
                                       WHERE Eot_OvertimeDate >= '{1}' AND Eot_OvertimeDate <= '{2}' --Eot_CurrentPayPeriod <= @CurPeriod
                                             AND Eot_Status IN ('A','9') {0}
                                       UNION
                                       SELECT DISTINCT Eot_EmployeeId, Eot_StartTime, Eot_EndTime, Eot_CurrentPayPeriod, Eot_OvertimeDate, Eot_OvertimeType, Eot_OvertimeHour
                                       FROM T_EmployeeOvertimeHist
                                       WHERE Eot_Status IN ('A','9') {0}
                                             AND Eot_OvertimeDate >= '{1}' AND Eot_OvertimeDate <= '{2}'
                                       ORDER BY Eot_EmployeeId, Eot_OvertimeDate, Eot_StartTime, Eot_EndTime", EmployeeCondition, PayrollStart, PayrollEnd);
            }
            else //Allow same records if FLEXSHIFT is set to TRUE
            {
                query = string.Format(@"DECLARE @CurPeriod AS CHAR(7)
                                       SET @CurPeriod = (SELECT Ppm_PayPeriod FROM T_PayPeriodMaster WHERE Ppm_CycleIndicator = 'C')

                                       SELECT Eot_EmployeeId, Eot_StartTime, Eot_EndTime, Eot_CurrentPayPeriod, Eot_OvertimeDate, Eot_OvertimeType, Eot_OvertimeHour
                                       FROM T_EmployeeOvertime 
                                       WHERE Eot_OvertimeDate >= '{1}' AND Eot_OvertimeDate <= '{2}' --Eot_CurrentPayPeriod <= @CurPeriod
                                             AND Eot_Status IN ('A','9') {0}
                                       UNION ALL
                                       SELECT Eot_EmployeeId, Eot_StartTime, Eot_EndTime, Eot_CurrentPayPeriod, Eot_OvertimeDate, Eot_OvertimeType, Eot_OvertimeHour
                                       FROM T_EmployeeOvertimeHist
                                       WHERE Eot_Status IN ('A','9') {0}
                                             AND Eot_OvertimeDate >= '{1}' AND Eot_OvertimeDate <= '{2}'
                                       ORDER BY Eot_EmployeeId, Eot_OvertimeDate, Eot_StartTime, Eot_EndTime", EmployeeCondition, PayrollStart, PayrollEnd);
            }
            #endregion
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public DataRow[] GetCorrectedOvertimeRecords(string EmployeeId, string ProcessDate, bool bIsRegularDay, bool bIsGraveyard, int iShiftTimeIn1Min, int iShiftTimeOut1Min, int iShiftTimeIn2Min, int iShiftTimeOut2Min, int iActualTimeIn1Min, bool bIsFlex, string strPayrollType)
        {
            DataRow[] drArrOvertimeApps = dtOvertimeTable.Select("Eot_EmployeeId = '" + EmployeeId + "' AND Eot_OvertimeDate = '" + ProcessDate + "'"
                                                                            , "Eot_StartTime ASC, Eot_EndTime ASC");
            int iStartOT1, iEndOT1, iEndOT1Orig;
            int iStartOT2, iEndOT2;
            int iStartOTMin, iEndOTMax;

            if (bIsFlex == false)
            {
                #region Non-Flex
                if (drArrOvertimeApps.Length > 1)
                {
                    for (int i = 0; i < drArrOvertimeApps.Length; i++)
                    {
                        iStartOT1 = GetMinsFromHourStr(drArrOvertimeApps[i]["Eot_StartTime"].ToString());
                        iEndOT1 = GetMinsFromHourStr(drArrOvertimeApps[i]["Eot_EndTime"].ToString());
                        iEndOT1Orig = iEndOT1;

                        if (iStartOT1 != 0 && iEndOT1 != 0 && bIsGraveyard && drArrOvertimeApps[i]["Eot_OvertimeType"].ToString().Equals("P")) //Graveyard shift and Post-overtime
                        {
                            if (iStartOT1 < (iShiftTimeIn1Min - LOGPAD))
                            {
                                iStartOT1 += GRAVEYARD24;
                            }
                            if (iEndOT1 < (iShiftTimeOut2Min - LOGPAD))
                            {
                                iEndOT1 += GRAVEYARD24;
                            }
                        }

                        for (int j = 0; j < drArrOvertimeApps.Length; j++)
                        {
                            if (i != j)
                            {
                                iStartOT2 = GetMinsFromHourStr(drArrOvertimeApps[j]["Eot_StartTime"].ToString());
                                iEndOT2 = GetMinsFromHourStr(drArrOvertimeApps[j]["Eot_EndTime"].ToString());

                                if (iStartOT2 != 0 && iEndOT2 != 0 && iStartOT1 != 0 && iEndOT1 != 0
                                    && bIsGraveyard && drArrOvertimeApps[j]["Eot_OvertimeType"].ToString().Equals("P")) //Graveyard shift and Post-overtime
                                {
                                    if (iStartOT2 < (iShiftTimeIn1Min - LOGPAD))
                                    {
                                        iStartOT2 += GRAVEYARD24;
                                    }
                                    if (iEndOT2 < (iShiftTimeOut2Min - LOGPAD))
                                    {
                                        iEndOT2 += GRAVEYARD24;
                                    }
                                }

                                if (iStartOT1 != 0 && iStartOT2 != 0)
                                    iStartOTMin = Math.Min(iStartOT1, iStartOT2);
                                else
                                    iStartOTMin = iStartOT1;
                                if (iEndOT1 != 0 && iEndOT2 != 0)
                                    iEndOTMax = Math.Max(iEndOT1, iEndOT2);
                                else
                                    iEndOTMax = iEndOT1;

                                //update overlapping records
                                if ((iEndOT1 >= iStartOT2 && iEndOT2 >= iStartOT1)
                                    && drArrOvertimeApps[i]["Eot_OvertimeType"].ToString().Equals(drArrOvertimeApps[j]["Eot_OvertimeType"].ToString())) //overlap
                                {
                                    drArrOvertimeApps[i]["Eot_StartTime"] = GetHourStrFromMins(iStartOTMin);
                                    drArrOvertimeApps[i]["Eot_EndTime"] = GetHourStrFromMins(iEndOTMax);
                                    drArrOvertimeApps[j]["Eot_StartTime"] = "0000";
                                    drArrOvertimeApps[j]["Eot_EndTime"] = "0000";
                                    iStartOT1 = iStartOTMin;
                                    iEndOT1 = iEndOTMax;
                                }
                            }
                        }
                    }
                }
                #endregion
            }
            else
            {
                #region Flex
                //If No Logs, get OB Application
                if (iActualTimeIn1Min == 0)
                {
                    DataRow[] drArrLeaveApp = dtLeaveTable.Select("EmployeeId = '" + EmployeeId + "' AND LeaveDate = '" + ProcessDate + "' AND WithCredit = 0 AND CombinedLeave = 0 AND PaidLeave = 1"
                                                                , "LeaveType ASC, LeaveHours ASC, StartTime ASC, EndTime ASC");
                    if (drArrLeaveApp.Length > 0)
                    {
                        iActualTimeIn1Min = GetMinsFromHourStr(drArrLeaveApp[0]["StartTime"].ToString());
                        //Compare OB Start vs OT Start (if Restday or Holiday)
                        if (drArrOvertimeApps.Length > 0 && bIsRegularDay == false)
                        {
                            iStartOT1 = GetMinsFromHourStr(drArrOvertimeApps[0]["Eot_StartTime"].ToString());
                            if (iStartOT1 > iActualTimeIn1Min)
                                iActualTimeIn1Min = iStartOT1;
                        }
                    }
                }

                double dAdvOT = 0, dMidOT = 0, dPostOT = 0;
                for (int j = 0; j < drArrOvertimeApps.Length; j++)
                {
                    if (drArrOvertimeApps[j]["Eot_OvertimeType"].ToString().Equals("A"))
                        dAdvOT += Convert.ToDouble(drArrOvertimeApps[j]["Eot_OvertimeHour"]);
                    else if (drArrOvertimeApps[j]["Eot_OvertimeType"].ToString().Equals("M"))
                        dMidOT += Convert.ToDouble(drArrOvertimeApps[j]["Eot_OvertimeHour"]);
                    else if (drArrOvertimeApps[j]["Eot_OvertimeType"].ToString().Equals("P"))
                        dPostOT += Convert.ToDouble(drArrOvertimeApps[j]["Eot_OvertimeHour"]);
                }

                bool bAdvOTDone = false, bMidOTDone = false, bPostOTDone = false;
                for (int j = 0; j < drArrOvertimeApps.Length; j++)
                {
                    if (drArrOvertimeApps[j]["Eot_OvertimeType"].ToString().Equals("A"))
                    {
                        if (bAdvOTDone == false)
                        {
                            bAdvOTDone = true;
                            if (iShiftTimeIn1Min - (int)(dAdvOT * 60) > 0)
                                drArrOvertimeApps[j]["Eot_StartTime"] = GetHourStrFromMins(iShiftTimeIn1Min - (int)(dAdvOT * 60));
                            else
                                drArrOvertimeApps[j]["Eot_StartTime"] = GetHourStrFromMins(ConvertToGraveyardTime(iShiftTimeIn1Min - (int)(dAdvOT * 60), true)); //graveyard
                            drArrOvertimeApps[j]["Eot_EndTime"] = GetHourStrFromMins(iShiftTimeIn1Min);
                            drArrOvertimeApps[j]["Eot_OvertimeHour"] = dAdvOT;
                        }
                        else
                        {
                            drArrOvertimeApps[j]["Eot_StartTime"] = "0000";
                            drArrOvertimeApps[j]["Eot_EndTime"] = "0000";
                            drArrOvertimeApps[j]["Eot_OvertimeHour"] = 0;
                        }
                    }
                    else if (drArrOvertimeApps[j]["Eot_OvertimeType"].ToString().Equals("M"))
                    {
                        if (bMidOTDone == false)
                        {
                            bMidOTDone = true;
                            drArrOvertimeApps[j]["Eot_StartTime"] = GetHourStrFromMins(iShiftTimeOut1Min);
                            drArrOvertimeApps[j]["Eot_EndTime"] = GetHourStrFromMins(iShiftTimeOut1Min + (int)(dMidOT * 60));
                        }
                        else
                        {
                            drArrOvertimeApps[j]["Eot_StartTime"] = "0000";
                            drArrOvertimeApps[j]["Eot_EndTime"] = "0000";
                            drArrOvertimeApps[j]["Eot_OvertimeHour"] = 0;
                        }
                    }
                    else if (drArrOvertimeApps[j]["Eot_OvertimeType"].ToString().Equals("P"))
                    {
                        if (bPostOTDone == false)
                        {
                            bPostOTDone = true;
                            if (bIsRegularDay == true)
                            {
                                drArrOvertimeApps[j]["Eot_StartTime"] = GetHourStrFromMins(iShiftTimeOut2Min);
                                drArrOvertimeApps[j]["Eot_EndTime"] = GetHourStrFromMins(iShiftTimeOut2Min + (int)(dPostOT * 60));
                            }
                            else //Restday or Holiday
                            {
                                if (strPayrollType == "M") //Hardcoded for LEAR (Monthlies = Start is IN1; Dailies = Start is Shift IN)
                                {
                                    drArrOvertimeApps[j]["Eot_StartTime"] = GetHourStrFromMins(iActualTimeIn1Min);
                                    drArrOvertimeApps[j]["Eot_EndTime"] = GetHourStrFromMins(iActualTimeIn1Min + (int)(dPostOT * 60));
                                }
                                else
                                {
                                    drArrOvertimeApps[j]["Eot_StartTime"] = GetHourStrFromMins(iShiftTimeIn1Min);
                                    drArrOvertimeApps[j]["Eot_EndTime"] = GetHourStrFromMins(iShiftTimeIn1Min + (int)(dPostOT * 60));
                                }
                            }
                            drArrOvertimeApps[j]["Eot_OvertimeHour"] = dPostOT;
                        }
                        else
                        {
                            drArrOvertimeApps[j]["Eot_StartTime"] = "0000";
                            drArrOvertimeApps[j]["Eot_EndTime"] = "0000";
                            drArrOvertimeApps[j]["Eot_OvertimeHour"] = 0;
                        }
                    }
                }
                #endregion
            }

            return drArrOvertimeApps;
        }

        public int GetFillerDayCodesCount()
        {
            string strQuery = @"SELECT Isnull(Count(Dcf_DayCode),0)
                                FROM T_DayCodeFiller
                                INNER JOIN T_DayCodeMaster on Dcf_DayCode = Dcm_DayCode
                                WHERE Dcf_Status = 'A' 
                                AND Dcm_Status = 'A'";

            DataTable dtResult = dal.ExecuteDataSet(strQuery).Tables[0];
            return Convert.ToInt32(dtResult.Rows[0][0]);
        }

        public int GenerateLaborHours_GetConvertedTimeIn1(int iActualTimeIn1Min, int iMasterShiftTimeIn1Min, string strPadShiftTimeIn, int iAbsFraction, bool bIsGraveyard)
        {
            if (iActualTimeIn1Min > 0 && iActualTimeIn1Min < iMasterShiftTimeIn1Min - LOGPAD)
            {
                iActualTimeIn1Min = ConvertToGraveyardTime(iActualTimeIn1Min, bIsGraveyard);
            }
            iActualTimeIn1Min = GetMinsFromHourStr(
                                        GetTimeINBasedOnShift(
                                            GetHourStrFromMins(iActualTimeIn1Min)
                                            , GetHourStrFromMins(iMasterShiftTimeIn1Min)
                                            , Convert.ToInt32(strPadShiftTimeIn)
                                            , iAbsFraction
                                        )
                                    );
            return iActualTimeIn1Min;
        }

        public int GenerateLaborHours_GetConvertedTimeOut1(int iConvTimeIn1Min, int iActualTimeOut1Min, int iMasterShiftTimeOut1Min, string strPadShiftBreakStart, int iAbsFraction, bool bIsGraveyard)
        {
            if (iActualTimeOut1Min > 0 && iActualTimeOut1Min < iConvTimeIn1Min)
            {
                iActualTimeOut1Min = ConvertToGraveyardTime(iActualTimeOut1Min, bIsGraveyard);
            }
            iActualTimeOut1Min = GetMinsFromHourStr(
                                        GetTimeOUTBasedOnShift(
                                            GetHourStrFromMins(iActualTimeOut1Min)
                                            , GetHourStrFromMins(iMasterShiftTimeOut1Min)
                                            , Convert.ToInt32(strPadShiftBreakStart)
                                            , iAbsFraction
                                        )
                                    );
            return iActualTimeOut1Min;
        }

        public int GenerateLaborHours_GetConvertedTimeIn2(int iConvTimeIn1Min, int iActualTimeIn2Min, int iMasterShiftTimeIn2Min, string strPadShiftBreakEnd, int iAbsFraction, bool bIsGraveyard)
        {
            if ((iActualTimeIn2Min > 0 && iActualTimeIn2Min < iConvTimeIn1Min)
                || (iConvTimeIn1Min == 0 && iActualTimeIn2Min > 0 && iActualTimeIn2Min < iMasterShiftTimeIn2Min - LOGPAD))
            {
                iActualTimeIn2Min = ConvertToGraveyardTime(iActualTimeIn2Min, bIsGraveyard);
            }
            iActualTimeIn2Min = GetMinsFromHourStr(
                                        GetTimeINBasedOnShift(
                                            GetHourStrFromMins(iActualTimeIn2Min)
                                            , GetHourStrFromMins(iMasterShiftTimeIn2Min)
                                            , Convert.ToInt32(strPadShiftBreakEnd)
                                            , iAbsFraction
                                        )
                                    );
            return iActualTimeIn2Min;
        }

        public int GenerateLaborHours_GetConvertedTimeOut2(int iConvTimeIn1Min, int iConvTimeIn2Min, int iActualTimeOut2Min, int iMasterShiftTimeOut2Min, string strPadShiftTimeOut, int iAbsFraction, bool bIsGraveyard)
        {
            if ((iActualTimeOut2Min > 0 && iActualTimeOut2Min < iConvTimeIn1Min)
                || (iConvTimeIn1Min == 0 && iActualTimeOut2Min > 0 && iActualTimeOut2Min < iConvTimeIn2Min))
            {
                iActualTimeOut2Min = ConvertToGraveyardTime(iActualTimeOut2Min, bIsGraveyard);
            }
            iActualTimeOut2Min = GetMinsFromHourStr(
                                        GetTimeOUTBasedOnShift(
                                            GetHourStrFromMins(iActualTimeOut2Min)
                                            , GetHourStrFromMins(iMasterShiftTimeOut2Min)
                                            , Convert.ToInt32(strPadShiftTimeOut)
                                            , iAbsFraction
                                        )
                                    );
            return iActualTimeOut2Min;
        }

        public string GetTimeINBasedOnShift(string strActualIN, string strShiftIN, int iPadIN, int iPadValue)
        {
            if (Convert.ToInt32(AddMinutesToHourStr(strShiftIN, iPadIN)) >= Convert.ToInt32(strActualIN))
            {
                return strActualIN;
            }
            else
            {
                if (strActualIN.Equals("0000"))
                {
                    return strActualIN;
                }
                else
                {
                    int iActualIN = GetMinsFromHourStr(strActualIN);
                    int iShiftIN = GetMinsFromHourStr(strShiftIN);
                    do
                    {
                        iShiftIN += iPadValue;
                    } while (iActualIN > iShiftIN);
                    strShiftIN = GetHourStrFromMins(iShiftIN);
                    return strShiftIN;
                }
            }
        }

        public string GetTimeOUTBasedOnShift(string strActualOUT, string strShiftOUT, int iPadOUT, int iPadValue)
        {
            if (Convert.ToInt32(AddMinutesToHourStr(strShiftOUT, iPadOUT)) <= Convert.ToInt32(strActualOUT))
            {
                return strActualOUT;
            }
            else
            {
                if (strActualOUT.Equals("0000"))
                {
                    return strActualOUT;
                }
                else
                {
                    int iActualOUT = GetMinsFromHourStr(strActualOUT);
                    int iShiftOUT = GetMinsFromHourStr(strShiftOUT);
                    iPadValue *= -1;
                    do
                    {
                        iShiftOUT += iPadValue;
                    } while (iActualOUT < iShiftOUT);
                    strShiftOUT = GetHourStrFromMins(iShiftOUT);
                    return strShiftOUT;
                }
            }
        }

        public DataTable GetNewHireInCurrentPayPeriod(bool ProcessAll, string EmployeeId)
        {
            string EmployeeCondition = "";
            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition = " AND Ell_EmployeeID = '" + EmployeeId + "' ";
            else if (ProcessAll == true && EmployeeList != "")
                EmployeeCondition = " AND Ell_EmployeeID IN (" + EmployeeList + ") ";

            #region query
            string qString = string.Format(@"SELECT Ell_EmployeeId, Ell_ProcessDate, Emt_HireDate
                                            FROM {0} 
                                            inner join t_employeemaster
	                                            on emt_employeeid = ell_employeeid
	                                            join t_payperiodmaster
                                                on Ppm_PayPeriod = Ell_PayPeriod 
                                                and Ell_PayPeriod = '{1}'
                                            WHERE emt_hiredate between ppm_startcycle and ppm_endcycle
                                                And ell_processdate < emt_hiredate
                                                {2}", EmployeeLogLedgerTable, ProcessPayrollPeriod, EmployeeCondition);
            #endregion
            DataTable dtResult = dal.ExecuteDataSet(qString).Tables[0];
            return dtResult;
        }

        public DataTable GetLeaveTypes()
        {
            #region query
            string qString = string.Format(@"SELECT Ltm_LeaveType, Ltm_LeaveDesc
                                            FROM T_LeaveTypeMaster
                                            WHERE Ltm_Status = 'A'");
            #endregion
            DataTable dtResult = dal.ExecuteDataSet(qString).Tables[0];
            return dtResult;
        }

        public DataTable GetAllHolidaysForCurrentPeriod()
        {
            string query = string.Format(@"SELECT * 
                                            FROM T_HolidayMaster
                                            INNER JOIN T_DayCodeMaster
                                                ON Hmt_HolidayCode = Dcm_DayCode
                                             WHERE Hmt_PrvDayHr >= 0
                                                 AND Hmt_HolidayDate >= '{0}'
                                                 AND Hmt_HolidayDate <= '{1}'
                                                 AND Dcm_Holiday = 1", PayrollStart, PayrollEnd);
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public DataTable GetDefaultShift()
        {
            string strQueryShift = @"DECLARE @DefaultShift VARCHAR(10)
                                    DECLARE @TopOneShift VARCHAR(10)
                                    SET @DefaultShift = (SELECT Ccd_DefaultShift 
		                                                FROM T_CompanyMaster)
                                    SET @TopOneShift = (SELECT TOP 1 Scm_ShiftCode 
		                                                FROM T_ShiftCodeMaster
		                                                WHERE Scm_Status = 'A'
		                                                ORDER BY 1)

                                    IF @DefaultShift IS NULL OR @DefaultShift = ''
                                    SET @DefaultShift = @TopOneShift
                                    ELSE
                                    BEGIN 
                                        SET @DefaultShift = (SELECT Scm_ShiftCode 
			                                                FROM T_ShiftCodeMaster
			                                                WHERE Scm_ShiftCode = @DefaultShift)
                                        IF @DefaultShift IS NULL OR @DefaultShift = ''
                                        SET @DefaultShift = @TopOneShift
                                    END

                                    SELECT * FROM T_ShiftCodeMaster WHERE Scm_ShiftCode = @DefaultShift";

            DataTable dtResult;
            dtResult = dal.ExecuteDataSet(strQueryShift).Tables[0];
            return dtResult;
        }

        public DataTable GetDayCodeMasterData()
        {
            string strQueryDayCode = @"SELECT Dcm_DayCode,Dcm_DayDesc,Dcm_Restday,Dcm_Holiday FROM T_DayCodeMaster WHERE Dcm_Status = 'A'";

            DataTable dtResult;
            dtResult = dal.ExecuteDataSet(strQueryDayCode).Tables[0];
            return dtResult;
        }

        public DataTable GetDayCodeFillers()
        {
            string strQueryDayCodeFillers = @"SELECT Dcf_FillerSeq, Dcf_DayCode, Dcf_Restday FROM t_DayCodeFiller WHERE Dcf_Status = 'A'";

            DataTable dtResult;
            dtResult = dal.ExecuteDataSet(strQueryDayCodeFillers).Tables[0];
            return dtResult;
        }

        public DataSet GetPrevPayPeriodOTSumAndJobStatus(string EmployeeId, string PayPeriod, DALHelper dal)
        {
            #region query
            string query = @"SELECT Ell_EmployeeId as Ept_EmployeeId
                                    , Ept_RegularOTHrTemp = SUM(CASE WHEN   Ell_DayCode = 'REG' and   Ell_RestDay = 0 THEN
                                                                    Ell_OvertimeHour  
                                                           ELSE
                                                                  0 
                                                          END)
                                    , Ept_RestdayHr = SUM(CASE WHEN   Ell_DayCode = 'REST' and   Ell_RestDay = 1 THEN
                                                                    Ell_RegularHour  
                                                          ELSE
                                                                  0 
                                                          END)
                                    , Ept_RestdayOTHr = SUM(CASE WHEN   Ell_DayCode = 'REST' and   Ell_RestDay = 1 THEN
                                                                    Ell_OvertimeHour  
                                                          ELSE
                                                                  0 
                                                          END)
                                    , Ept_LegalHolidayHr = SUM(CASE WHEN   Ell_DayCode = 'HOL' and   Ell_RestDay = 0 THEN
                                                                         Ell_RegularHour  
                                                               ELSE
                                                                       0 
                                                               END)
                                    , Ept_LegalHolidayOTHr = SUM(CASE WHEN   Ell_DayCode = 'HOL' and   Ell_RestDay = 0 THEN
                                                                         Ell_OvertimeHour  
                                                               ELSE
                                                                       0 
                                                               END)
                                    , Ept_SpecialHolidayHr = SUM(CASE WHEN   Ell_DayCode = 'SPL' and   Ell_RestDay = 0 THEN
                                                                        Ell_RegularHour  
                                                                 ELSE
                                                                      0 
                                                                 END)
                                    , Ept_SpecialHolidayOTHr = SUM(CASE WHEN   Ell_DayCode = 'SPL' and   Ell_RestDay = 0 THEN
                                                                        Ell_OvertimeHour  
                                                                 ELSE
                                                                      0 
                                                                 END)
                                    , Ept_PlantShutdownHr = SUM(CASE WHEN   Ell_DayCode = 'PSD' and   Ell_RestDay = 0 THEN
                                                                       Ell_RegularHour   
                                                                 ELSE
                                                                     0 
                                                                 END) 
                                    , Ept_PlantShutdownOTHr = SUM(CASE WHEN   Ell_DayCode = 'PSD' and   Ell_RestDay = 0 THEN
                                                                       Ell_OvertimeHour   
                                                                 ELSE
                                                                     0 
                                                                 END) 
                                    , Ept_CompanyHolidayHr = SUM(CASE WHEN   Ell_DayCode = 'COMP' and   Ell_RestDay = 0 THEN
                                                                        Ell_RegularHour  
                                                                 ELSE
                                                                      0 
                                                                 END)
                                    , Ept_CompanyHolidayOTHr = SUM(CASE WHEN   Ell_DayCode = 'COMP' and   Ell_RestDay = 0 THEN
                                                                        Ell_OvertimeHour  
                                                                 ELSE
                                                                      0 
                                                                 END)
                                    , Ept_RestdayLegalHolidayHr = SUM(CASE WHEN   Ell_DayCode = 'HOL' and   Ell_RestDay = 1 THEN
                                                                           Ell_RegularHour  
                                                                        ELSE
                                                                             0 
                                                                        END)
                                    , Ept_RestdayLegalHolidayOTHr = SUM(CASE WHEN   Ell_DayCode = 'HOL' and   Ell_RestDay = 1 THEN
                                                                           Ell_OvertimeHour  
                                                                        ELSE
                                                                             0 
                                                                        END)
                                    , Ept_RestdaySpecialHolidayHr = SUM(CASE WHEN   Ell_DayCode = 'SPL' and   Ell_RestDay = 1 THEN
                                                                           Ell_RegularHour  
                                                                        ELSE
                                                                             0 
                                                                        END)
                                    , Ept_RestdaySpecialHolidayOTHr = SUM(CASE WHEN   Ell_DayCode = 'SPL' and   Ell_RestDay = 1 THEN
                                                                           Ell_OvertimeHour  
                                                                        ELSE
                                                                             0 
                                                                        END)
                                    , Ept_RestdayCompanyHolidayHr = SUM(CASE WHEN   Ell_DayCode = 'COMP' and   Ell_RestDay = 1 THEN
                                                                           Ell_RegularHour  
                                                                        ELSE
                                                                             0 
                                                                        END)
                                    , Ept_RestdayCompanyHolidayOTHr = SUM(CASE WHEN   Ell_DayCode = 'COMP' and   Ell_RestDay = 1 THEN
                                                                           Ell_OvertimeHour  
                                                                        ELSE
                                                                             0 
                                                                        END)
                                    , Ept_RestdayPlantShutdownHr = SUM(CASE WHEN  Ell_DayCode = 'PSD' and  Ell_RestDay = 1 THEN
                                                                           Ell_RegularHour  
                                                                         ELSE
                                                                           0 
                                                                         END) 
                                    , Ept_RestdayPlantShutdownOTHr = SUM(CASE WHEN  Ell_DayCode = 'PSD' and  Ell_RestDay = 1 THEN
                                                                           Ell_OvertimeHour  
                                                                         ELSE
                                                                           0 
                                                                         END)    
                                FROM T_EmployeeLogLedgerHist
                                WHERE Ell_EmployeeId = '{0}'
                                      And Ell_PayPeriod = '{1}' and Left(Ell_LocationCode, 1) <> 'D'
                                GROUP BY  Ell_EmployeeId

                                SELECT Emt_JobLevel, Emt_JobStatus, Emt_EmploymentStatus, Emt_PayrollType, Emt_PositionCode
                                 FROM T_EmployeeMasterHist
                                 WHERE Emt_EmployeeID = '{0}'
                                    AND Emt_PayPeriod = '{1}'";
            #endregion
            query = string.Format(query, EmployeeId, PayPeriod);
            DataSet dsResult;
            dsResult = dal.ExecuteDataSet(query);
            return dsResult;
        }

        public DataRow GetHolidayPrevDayHist(string EmployeeID, string ProcessDate)
        {
            #region query
            string query = string.Format(@"SELECT Ell_ProcessDate, Ell_RegularHour, Ell_LeaveHour, Ell_OvertimeHour, Ell_DayCode, Ell_RestDay, Ell_EncodedNoPayLeaveType
                                            FROM T_EmployeeLogLedgerHist
                                            WHERE Ell_EmployeeId = '{0}'
                                            AND Ell_ProcessDate = '{1}'", EmployeeID, ProcessDate);
            #endregion
            DataTable dtResult;
            dtResult = dal.ExecuteDataSet(query).Tables[0];
            if (dtResult.Rows.Count > 0)
                return dtResult.Rows[0];
            else
                return null;
        }

        public int CleanUpByRoundHigh(int TimeIn, int TIMEFRAC, DALHelper dal)
        {
            if (TIMEFRAC != 0 && TimeIn != 0)
            {
                int temp = Convert.ToInt32(TimeIn / TIMEFRAC) * TIMEFRAC;
                if (temp != TimeIn)
                    TimeIn = temp + TIMEFRAC;
            }
            return TimeIn;
        }

        public int CleanUpByRoundLow(int TimeOut, int TIMEFRAC, DALHelper dal)
        {
            if (TIMEFRAC != 0 && TimeOut != 0)
            {
                TimeOut = Convert.ToInt32(TimeOut / TIMEFRAC) * TIMEFRAC;
            }
            return TimeOut;
        }

        public int GetTimeWithLateChargeDaily(int iActualTimeIn, int iShiftTimeIn)
        {
            if (iActualTimeIn - iShiftTimeIn > 0 && iActualTimeIn - iShiftTimeIn <= LATECHARGE)
                return iShiftTimeIn;
            else
                return iActualTimeIn;
        }

        public bool IsLateChargeQuincenaMet(string EmployeeID, string PayPeriod, DALHelper dal)
        {
            string query = string.Format(@"SELECT Ell_ProcessDate, Ell_ActualTimeIn_1, Scm_ShiftTimeIn, AdjustedTimeIn = '0000'
                                            FROM T_EmployeeLogLedger
                                            INNER JOIN T_ShiftCodeMaster
                                                ON Ell_ShiftCode = Scm_ShiftCode
                                            WHERE Ell_EmployeeId = '{0}'
                                                AND Ell_PayPeriod = '{1}'", EmployeeID, PayPeriod);

            int iLateCnt = 0, iLateVal = 0;
            bool bIsLateChargeMet = true;
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            if (dtResult.Rows.Count > 0)
            {
                foreach (DataRow drRow in dtResult.Rows)
                {
                    iLateVal = GetHourDiffInMinutes(drRow["Ell_ActualTimeIn_1"].ToString(), drRow["Scm_ShiftTimeIn"].ToString());
                    if (iLateVal > 0)
                    {
                        iLateCnt += iLateVal;
                        if (iLateCnt <= LATECHARGE)
                        {
                            drRow["AdjustedTimeIn"] = drRow["Scm_ShiftTimeIn"];
                        }
                        else
                        {
                            bIsLateChargeMet = false;
                            break;
                        }
                    }
                }

                return bIsLateChargeMet;
            }
            return false;
        }

        public int GetTimeInWithLateBracketFilter(int ActualTimeIn, int ShiftTimeIn)
        {
            if (ActualTimeIn > ShiftTimeIn && LATEBRACKETDEDUCTION != null)
            {
                for (int i = 0; i < LATEBRACKETDEDUCTION.Rows.Count; i++)
                {
                    //Example: (Please take note of the ranging of values)
                    //16 = 30 
                    //46 = 60 
                    //60 = 0
                    //This means, 
                    //16-45 late mins = 30 mins deduction
                    //46-59 late mins = 60 mins deduction
                    if (i + 1 != LATEBRACKETDEDUCTION.Rows.Count
                        && (ActualTimeIn - ShiftTimeIn) >= Convert.ToInt32(LATEBRACKETDEDUCTION.Rows[i]["Pmx_Classification"])
                        && (ActualTimeIn - ShiftTimeIn) < Convert.ToInt32(LATEBRACKETDEDUCTION.Rows[i + 1]["Pmx_Classification"])
                        && Convert.ToInt32(Convert.ToDouble(LATEBRACKETDEDUCTION.Rows[i]["Pmx_ParameterValue"])) > 0)
                    {
                        ActualTimeIn = ShiftTimeIn + Convert.ToInt32(Convert.ToDouble(LATEBRACKETDEDUCTION.Rows[i]["Pmx_ParameterValue"]));
                        break;
                    }
                }
            }
            return ActualTimeIn;
        }

        public int GetTimeInWithLatePMBracketFilter(int ActualTimeIn, int ShiftTimeIn1, int ShiftTimeOut1, int ShiftTimeIn2)
        {
            if (ActualTimeIn > ShiftTimeIn1 && LATEBRACKETDEDUCTION != null)
            {
                int iBreakTime = GetOTHoursInMinutes(ShiftTimeOut1, ShiftTimeIn2, ShiftTimeOut1, ActualTimeIn);
                for (int i = 0; i < LATEBRACKETDEDUCTION.Rows.Count; i++)
                {
                    //Example: (Please take note of the ranging of values)
                    //16 = 30 
                    //46 = 60 
                    //60 = 0
                    //This means, 
                    //16-45 late mins = 30 mins deduction
                    //46-59 late mins = 60 mins deduction
                    if (i + 1 != LATEBRACKETDEDUCTION.Rows.Count
                        && (ActualTimeIn - ShiftTimeIn1 - iBreakTime) >= Convert.ToInt32(LATEBRACKETDEDUCTION.Rows[i]["Pmx_Classification"])
                        && (ActualTimeIn - ShiftTimeIn1 - iBreakTime) < Convert.ToInt32(LATEBRACKETDEDUCTION.Rows[i + 1]["Pmx_Classification"])
                        && Convert.ToInt32(Convert.ToDouble(LATEBRACKETDEDUCTION.Rows[i]["Pmx_ParameterValue"])) > 0)
                    {
                        ActualTimeIn = ShiftTimeIn1 + iBreakTime + Convert.ToInt32(Convert.ToDouble(LATEBRACKETDEDUCTION.Rows[i]["Pmx_ParameterValue"]));
                        break;
                    }
                }
            }
            return ActualTimeIn;
        }

        public int GetTimeOutWithUndertimeBracketFilter(int ActualTimeOut, int ShiftTimeOut)
        {
            if (ActualTimeOut < ShiftTimeOut && UNDERTIMEBRACKETDEDUCTION != null)
            {
                for (int i = 0; i < UNDERTIMEBRACKETDEDUCTION.Rows.Count; i++)
                {
                    //Example: (Please take note of the ranging of values)
                    //16 = 30 
                    //46 = 60 
                    //60 = 0
                    //This means, 
                    //16-45 undertime mins = 30 mins deduction
                    //46-59 undertime mins = 60 mins deduction
                    if (i + 1 != UNDERTIMEBRACKETDEDUCTION.Rows.Count
                        && (ShiftTimeOut - ActualTimeOut) >= Convert.ToInt32(UNDERTIMEBRACKETDEDUCTION.Rows[i]["Pmx_Classification"])
                        && (ShiftTimeOut - ActualTimeOut) < Convert.ToInt32(UNDERTIMEBRACKETDEDUCTION.Rows[i + 1]["Pmx_Classification"])
                        && Convert.ToInt32(Convert.ToDouble(UNDERTIMEBRACKETDEDUCTION.Rows[i]["Pmx_ParameterValue"])) > -1)//CHANGE MERGE 09222015
                    {
                        if (ShiftTimeOut - Convert.ToInt32(Convert.ToDouble(UNDERTIMEBRACKETDEDUCTION.Rows[i]["Pmx_ParameterValue"])) >= 0)
                            ActualTimeOut = ShiftTimeOut - Convert.ToInt32(Convert.ToDouble(UNDERTIMEBRACKETDEDUCTION.Rows[i]["Pmx_ParameterValue"]));
                        break;
                    }
                }
            }
            return ActualTimeOut;
        }

        public int GetTimeOutWithUndertimePMBracketFilter(int ActualTimeOut, int ShiftTimeOut1, int ShiftTimeIn2, int ShiftTimeOut2)
        {
            if (ActualTimeOut < ShiftTimeOut2 && UNDERTIMEBRACKETDEDUCTION != null)
            {
                int iBreakTime = GetOTHoursInMinutes(ShiftTimeOut1, ShiftTimeIn2, ActualTimeOut, ShiftTimeIn2);
                for (int i = 0; i < UNDERTIMEBRACKETDEDUCTION.Rows.Count; i++)
                {
                    //Example: (Please take note of the ranging of values)
                    //16 = 30 
                    //46 = 60 
                    //60 = 0
                    //This means, 
                    //16-45 undertime mins = 30 mins deduction
                    //46-59 undertime mins = 60 mins deduction
                    if (i + 1 != UNDERTIMEBRACKETDEDUCTION.Rows.Count
                        && (ShiftTimeOut2 - ActualTimeOut - iBreakTime) >= Convert.ToInt32(UNDERTIMEBRACKETDEDUCTION.Rows[i]["Pmx_Classification"])
                        && (ShiftTimeOut2 - ActualTimeOut - iBreakTime) < Convert.ToInt32(UNDERTIMEBRACKETDEDUCTION.Rows[i + 1]["Pmx_Classification"])
                        && Convert.ToInt32(Convert.ToDouble(UNDERTIMEBRACKETDEDUCTION.Rows[i]["Pmx_ParameterValue"])) > -1)
                    {
                        if (ShiftTimeOut2 - iBreakTime - Convert.ToInt32(Convert.ToDouble(UNDERTIMEBRACKETDEDUCTION.Rows[i]["Pmx_ParameterValue"])) >= 0)
                            ActualTimeOut = ShiftTimeOut2 - iBreakTime - Convert.ToInt32(Convert.ToDouble(UNDERTIMEBRACKETDEDUCTION.Rows[i]["Pmx_ParameterValue"]));
                        break;
                    }
                }
            }
            return ActualTimeOut;
        }

        private DataTable GetBracketParameter(string ParameterID)
        {
            DataSet ds = new DataSet();

            #region query
            string qString = string.Format(@" SELECT CONVERT(INT, Pmx_Classification) AS Pmx_Classification, Pmx_ParameterValue 
                                                FROM T_ParameterMasterExt
                                                WHERE Pmx_ParameterID = '{0}'
	                                                AND Pmx_Status = 'A'
                                                ORDER BY 1", ParameterID);
            #endregion

            using (DALHelper dal = new DALHelper(MainDB, false))
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString);
                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0];
            else
                return new DataTable();
        }

        public void UpdateSalaryRatePerDay(bool ProcessAll, string EmployeeId)
        {
            string EmployeeCondition = "";
            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition = " AND Ept_EmployeeID = '" + EmployeeId + "' ";
            else if (ProcessAll == true && EmployeeList != "")
                EmployeeCondition = " AND Ept_EmployeeID IN (" + EmployeeList + ") ";

            #region Salary info update query
            string strSalaryRateQuery = string.Format(@"UPDATE {0}
                                                        SET Ept_SalaryRate = ISNULL(Esm_SalaryRate, Emt_SalaryRate)
                                                            , Ept_HourlyRate = CASE WHEN (SELECT Pcm_ProcessFlag FROM T_ProcessControlMaster WHERE Pcm_ProcessID = 'HRRTINCDEM' AND Pcm_SystemID = 'PAYROLL') = 0 
	                                                                           THEN CASE ISNULL(Esm_PayrollType, Emt_PayrollType) 
			                                                                        WHEN 'D' THEN 
					                                                                        ISNULL(Esm_SalaryRate, Emt_SalaryRate) / @HOURSINDAY
			                                                                        WHEN 'M' THEN 
					                                                                        (ISNULL(Esm_SalaryRate, Emt_SalaryRate) * 12) / ISNULL((SELECT Pmx_ParameterValue
                                                                                                                                                    FROM T_ParameterMasterExt
                                                                                                                                                    WHERE Pmx_ParameterID = 'MDIVISOR'
                                                                                                                                                        AND Pmx_Classification = Emt_EmploymentStatus
                                                                                                                                                        AND Pmx_Status = 'A'), 0) / @HOURSINDAY
			                                                                        WHEN 'H' THEN 
					                                                                        ISNULL(Esm_SalaryRate, Emt_SalaryRate)
			                                                                        ELSE 0 END   
	                                                                           ELSE
			                                                                        DBO.GetHourlyRateWithDeMinimis(Ept_EmployeeID, '{4}')
	                                                                           END 
                                                            , Ept_SalaryType = ISNULL(Esm_Type, 'B')
                                                            , Ept_PayrollType = ISNULL(Esm_PayrollType, Emt_PayrollType)
                                                        FROM {0}
                                                        INNER JOIN T_EmployeeMaster
                                                            ON Ept_EmployeeId = Emt_EmployeeId
                                                        LEFT JOIN T_EmployeeSalaryMovement
                                                            ON Ept_EmployeeId = Esm_EmployeeId
                                                            AND Esm_Type = 'B'
                                                        WHERE Ept_ProcessDate >= ISNULL(Esm_StartDate, '{5}') 
                                                            AND Ept_ProcessDate <= ISNULL(Esm_EndDate, '{3}')
                                                            {2}", EmployeePayrollTransactionTable + "Detail", MDIVISOR, EmployeeCondition, PayrollEnd, ProcessPayrollPeriod, PayrollStart).Replace("@HOURSINDAY", CommonBL.HOURSINDAY.ToString());
            #endregion
            dal.ExecuteNonQuery(strSalaryRateQuery);

            if (Convert.ToBoolean(NOABSNWHRE) == false)
            {
                #region Salary info update query (Override for New Hires)
                strSalaryRateQuery = string.Format(@"UPDATE {0}
                                                SET Ept_SalaryRate = ISNULL(Esm_SalaryRate, Emt_SalaryRate) 
                                                    , Ept_HourlyRate = CASE WHEN (SELECT Pcm_ProcessFlag FROM T_ProcessControlMaster WHERE Pcm_ProcessID = 'HRRTINCDEM' AND Pcm_SystemID = 'PAYROLL') = 0 
	                                                                   THEN CASE ISNULL(Esm_PayrollType, Emt_PayrollType)
			                                                                WHEN 'D' THEN 
					                                                                ISNULL(Esm_SalaryRate, Emt_SalaryRate) / @HOURSINDAY
			                                                                WHEN 'M' THEN 
					                                                                (ISNULL(Esm_SalaryRate, Emt_SalaryRate) * 12) / ISNULL((SELECT Pmx_ParameterValue
                                                                                                                                                    FROM T_ParameterMasterExt
                                                                                                                                                    WHERE Pmx_ParameterID = 'MDIVISOR'
                                                                                                                                                        AND Pmx_Classification = Emt_EmploymentStatus
                                                                                                                                                        AND Pmx_Status = 'A'), 0) / @HOURSINDAY
			                                                                WHEN 'H' THEN 
					                                                                ISNULL(Esm_SalaryRate, Emt_SalaryRate)
			                                                                ELSE 0 END   
	                                                                   ELSE
			                                                                DBO.GetHourlyRateWithDeMinimis(Ept_EmployeeID, '{5}')
	                                                                   END 
                                                    , Ept_SalaryType = ISNULL(Esm_Type, 'B')
                                                    , Ept_PayrollType = ISNULL(Esm_PayrollType, Emt_PayrollType)
                                                FROM {0}
                                                INNER JOIN T_EmployeeMaster on Emt_EmployeeID = Ept_EmployeeId
                                                LEFT JOIN T_EmployeeSalaryMovement
                                                    ON Ept_EmployeeId = Esm_EmployeeId
                                                    AND Esm_Type = 'B'
                                                WHERE Ept_ProcessDate >= 
                                                    ISNULL(( Case when emt_hiredate between '{3}' and '{4}' then
                                                            Case when Esm_StartDate > '{3}' then 
                                                                '{3}' 
                                                            else 
                                                                Esm_StartDate end
                                                      Else Esm_StartDate End ), '{3}')
                                                    AND Ept_ProcessDate <= ISNULL(Esm_EndDate, '{4}')
                                                    {2}", EmployeePayrollTransactionTable + "Detail", MDIVISOR, EmployeeCondition, PayrollStart, PayrollEnd, ProcessPayrollPeriod).Replace("@HOURSINDAY", CommonBL.HOURSINDAY.ToString());
                #endregion
                dal.ExecuteNonQuery(strSalaryRateQuery);
            }

            #region Salary info update query (for Ext)
            strSalaryRateQuery = string.Format(@"UPDATE {0}
                                                SET Ept_SalaryRate = ISNULL(Esm_SalaryRate, Emt_SalaryRate) 
                                                    , Ept_HourlyRate = CASE WHEN (SELECT Pcm_ProcessFlag FROM T_ProcessControlMaster WHERE Pcm_ProcessID = 'HRRTINCDEM' AND Pcm_SystemID = 'PAYROLL') = 0 
	                                                                   THEN CASE ISNULL(Esm_PayrollType, Emt_PayrollType) 
			                                                                WHEN 'D' THEN 
					                                                                ISNULL(Esm_SalaryRate, Emt_SalaryRate) / @HOURSINDAY
			                                                                WHEN 'M' THEN 
					                                                                (ISNULL(Esm_SalaryRate, Emt_SalaryRate) * 12) / ISNULL((SELECT Pmx_ParameterValue
                                                                                                                                                    FROM T_ParameterMasterExt
                                                                                                                                                    WHERE Pmx_ParameterID = 'MDIVISOR'
                                                                                                                                                        AND Pmx_Classification = Emt_EmploymentStatus
                                                                                                                                                        AND Pmx_Status = 'A'), 0) / @HOURSINDAY
			                                                                WHEN 'H' THEN 
					                                                                ISNULL(Esm_SalaryRate, Emt_SalaryRate)
			                                                                ELSE 0 END   
	                                                                   ELSE
			                                                                DBO.GetHourlyRateWithDeMinimis(Ept_EmployeeID, '{4}')
	                                                                   END 
                                                    , Ept_SalaryType = ISNULL(Esm_Type, 'B')
                                                    , Ept_PayrollType = ISNULL(Esm_PayrollType, Emt_PayrollType)
                                                FROM {0}
                                                INNER JOIN T_EmployeeMaster
                                                    ON Ept_EmployeeId = Emt_EmployeeId
                                                LEFT JOIN T_EmployeeSalaryMovement
                                                    ON Ept_EmployeeId = Esm_EmployeeId
                                                    AND Esm_Type = 'B'
                                                WHERE Ept_ProcessDate >= ISNULL(Esm_StartDate, '{5}')
                                                    AND Ept_ProcessDate <= ISNULL(Esm_EndDate, '{3}')
                                                    {2}", EmployeePayrollTransactionTable + "ExtDetail", MDIVISOR, EmployeeCondition, PayrollEnd, ProcessPayrollPeriod, PayrollStart).Replace("@HOURSINDAY", CommonBL.HOURSINDAY.ToString());
            #endregion
            dal.ExecuteNonQuery(strSalaryRateQuery);

            if (Convert.ToBoolean(NOABSNWHRE) == false)
            {
                #region Salary info update query (for Ext - Override for New Hires)
                strSalaryRateQuery = string.Format(@"UPDATE {0}
                                                SET Ept_SalaryRate = ISNULL(Esm_SalaryRate, Emt_SalaryRate) 
                                                    , Ept_HourlyRate = CASE WHEN (SELECT Pcm_ProcessFlag FROM T_ProcessControlMaster WHERE Pcm_ProcessID = 'HRRTINCDEM' AND Pcm_SystemID = 'PAYROLL') = 0 
	                                                                   THEN CASE ISNULL(Esm_PayrollType, Emt_PayrollType) 
			                                                                WHEN 'D' THEN 
					                                                                ISNULL(Esm_SalaryRate, Emt_SalaryRate) / @HOURSINDAY
			                                                                WHEN 'M' THEN 
					                                                                (ISNULL(Esm_SalaryRate, Emt_SalaryRate) * 12) / ISNULL((SELECT Pmx_ParameterValue
                                                                                                                                                    FROM T_ParameterMasterExt
                                                                                                                                                    WHERE Pmx_ParameterID = 'MDIVISOR'
                                                                                                                                                        AND Pmx_Classification = Emt_EmploymentStatus
                                                                                                                                                        AND Pmx_Status = 'A'), 0) / @HOURSINDAY
			                                                                WHEN 'H' THEN 
					                                                                ISNULL(Esm_SalaryRate, Emt_SalaryRate)
			                                                                ELSE 0 END   
	                                                                   ELSE
			                                                                DBO.GetHourlyRateWithDeMinimis(Ept_EmployeeID, '{5}')
	                                                                   END
                                                    , Ept_SalaryType = ISNULL(Esm_Type, 'B')
                                                    , Ept_PayrollType = ISNULL(Esm_PayrollType, Emt_PayrollType)
                                                FROM {0}
                                                INNER JOIN T_EmployeeMaster on Emt_EmployeeID = Ept_EmployeeId
                                                LEFT JOIN T_EmployeeSalaryMovement
                                                    ON Ept_EmployeeId = Esm_EmployeeId
                                                    AND Esm_Type = 'B'
                                                WHERE Ept_ProcessDate >= 
                                                    ISNULL(( Case when emt_hiredate between '{3}' and '{4}' then
                                                            Case when Esm_StartDate > '{3}' then 
                                                                '{3}' 
                                                            else 
                                                                Esm_StartDate end
                                                      Else Esm_StartDate End ), '{3}')
                                                    AND Ept_ProcessDate <= ISNULL(Esm_EndDate, '{4}')
                                                    {2}", EmployeePayrollTransactionTable + "ExtDetail", MDIVISOR, EmployeeCondition, PayrollStart, PayrollEnd, ProcessPayrollPeriod).Replace("@HOURSINDAY", CommonBL.HOURSINDAY.ToString());
                #endregion
                dal.ExecuteNonQuery(strSalaryRateQuery);
            }
        }

        public DataTable GetEmployeesWithSalaryMovement(bool ProcessAll, string EmployeeId)
        {
            string EmployeeCondition = "";
            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition = " AND Esm_EmployeeID = '" + EmployeeId + "' ";
            else if (ProcessAll == true && EmployeeList != "")
                EmployeeCondition = " AND Esm_EmployeeID IN (" + EmployeeList + ") ";

            string query = string.Format(@"SELECT DISTINCT Esm_EmployeeId
                                              FROM [T_EmployeeSalaryMovement]
                                              WHERE Esm_Type = 'B'
                                              AND [Esm_StartDate] > '{0}'
                                              AND [Esm_StartDate] <= '{1}' {2}", PayrollStart, PayrollEnd, EmployeeCondition);

            DataTable dtResult;
            dtResult = dal.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public DataTable GetAllLeaveAvailmentRecords(bool ProcessAll, string EmployeeId)
        {
            string EmployeeCondition = "";
            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition = " AND Elt_EmployeeID = '" + EmployeeId + "' ";
            else if (ProcessAll == true && EmployeeList != "")
                EmployeeCondition = " AND Elt_EmployeeID IN (" + EmployeeList + ") ";
            #region query
            string query = string.Format(@"DECLARE @Leave as table
                                                ( EmployeeId varchar(15)
	                                            , LeaveDate datetime
	                                            , LeaveType char(2)                    
	                                            , LeaveHours decimal(5,2)
	                                            , StartTime char(4)
	                                            , EndTime char(4)  
                                                , CombinedLeave bit
                                                , PaidLeave bit
	                                            , WithCredit bit
                                                , ControlNo varchar(12)
                                                , DayUnit char(2))

                                                 INSERT INTO @Leave
                                              
  	                                            SELECT Elt_EmployeeId
		                                            , Elt_LeaveDate
		                                            , Elt_LeaveType
		                                            , Elt_LeaveHour
		                                            , Elt_StartTime
		                                            , Elt_EndTime
                                                    , Ltm_CombinedLeave
                                                    , Ltm_PaidLeave
		                                            , Ltm_WithCredit
                                                    , Elt_ControlNo
                                                    , Elt_DayUnit
	                                            FROM T_EmployeeLeaveAvailment
	                                            INNER JOIN T_LeavetypeMaster on Ltm_LeaveType = Elt_LeaveType
		                                            and Ltm_PaidLeave =  1
	                                            WHERE Elt_LeaveDate >= '{2}' AND Elt_LeaveDate <= '{3}' --Elt_CurrentPayPeriod <= '{1}' 
		                                            and Elt_Status in ('A','9','0')
                                                    {0}

	                                            UNION

	                                            SELECT Elt_EmployeeId
		                                            , Elt_LeaveDate
		                                            , Elt_LeaveType
		                                            , Elt_LeaveHour
		                                            , Elt_StartTime
		                                            , Elt_EndTime
                                                    , Ltm_CombinedLeave
                                                    , Ltm_PaidLeave
		                                            , Ltm_WithCredit
                                                    , Elt_ControlNo
                                                    , Elt_DayUnit
	                                            FROM T_EmployeeLeaveAvailmentHist
	                                            INNER JOIN T_LeavetypeMaster on Ltm_LeaveType = Elt_LeaveType
		                                            and Ltm_PaidLeave =  1
	                                            WHERE Elt_Status in ('A','9','0')
		                                            and  Elt_LeaveDate >= '{2}' AND Elt_LeaveDate <= '{3}'
                                                    {0}

	                                            DELETE FROM @Leave
	                                            FROM @LEAVE
	                                            WHERE EmployeeId + CONVERT(CHAR(10),LeaveDate,101) +  LeaveType IN 
	                                            ( SELECT EmployeeId + CONVERT(CHAR(10),LeaveDate,101) +  LeaveType 
	                                            FROM @Leave
	                                            GROUP BY  EmployeeId 
		                                            , LeaveDate 
		                                            , LeaveType 
	                                            HAVING SUM(LeaveHours) = 0)

	                                            SELECT * FROM @Leave", EmployeeCondition, ProcessPayrollPeriod, PayrollStart, PayrollEnd);
            #endregion
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public DataTable GetAllUnpaidLeaveAvailmentRecords(bool ProcessAll, string EmployeeId)
        {
            string EmployeeCondition = "";
            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition = " AND Elt_EmployeeID = '" + EmployeeId + "' ";
            else if (ProcessAll == true && EmployeeList != "")
                EmployeeCondition = " AND Elt_EmployeeID IN (" + EmployeeList + ") ";
            #region query
            string query = string.Format(@"DECLARE @Leave as table
                                                ( EmployeeId varchar(15)
	                                            , LeaveDate datetime
	                                            , LeaveType char(2)                    
	                                            , LeaveHours decimal(5,2)
	                                            , StartTime char(4)
	                                            , EndTime char(4)  
                                                , CombinedLeave bit
                                                , PaidLeave bit 
	                                            , WithCredit bit
                                                , ControlNo varchar(12)
                                                , DayUnit char(2))

                                                 INSERT INTO @Leave
                                              
  	                                            SELECT Elt_EmployeeId
		                                            , Elt_LeaveDate
		                                            , Elt_LeaveType
		                                            , Elt_LeaveHour
		                                            , Elt_StartTime
		                                            , Elt_EndTime
                                                    , Ltm_CombinedLeave
                                                    , Ltm_PaidLeave
		                                            , Ltm_WithCredit
                                                    , Elt_ControlNo
                                                    , Elt_DayUnit
	                                            FROM T_EmployeeLeaveAvailment
	                                            INNER JOIN T_LeavetypeMaster on Ltm_LeaveType = Elt_LeaveType
		                                            and Ltm_PaidLeave =  0
	                                            WHERE Elt_LeaveDate >= '{2}' AND Elt_LeaveDate <= '{3}' --Elt_CurrentPayPeriod <= '{1}' 
		                                            and Elt_Status in ('A','9','0')
                                                    {0}

	                                            UNION

	                                            SELECT Elt_EmployeeId
		                                            , Elt_LeaveDate
		                                            , Elt_LeaveType
		                                            , Elt_LeaveHour
		                                            , Elt_StartTime
		                                            , Elt_EndTime
                                                    , Ltm_CombinedLeave
                                                    , Ltm_PaidLeave
		                                            , Ltm_WithCredit
                                                    , Elt_ControlNo
                                                    , Elt_DayUnit
	                                            FROM T_EmployeeLeaveAvailmentHist
	                                            INNER JOIN T_LeavetypeMaster on Ltm_LeaveType = Elt_LeaveType
		                                            and Ltm_PaidLeave =  0
	                                            WHERE Elt_Status in ('A','9','0')
		                                            and  Elt_LeaveDate >= '{2}' AND Elt_LeaveDate <= '{3}'
                                                    {0}

	                                            DELETE FROM @Leave
	                                            FROM @LEAVE
	                                            WHERE EmployeeId + CONVERT(CHAR(10),LeaveDate,101) +  LeaveType IN 
	                                            ( SELECT EmployeeId + CONVERT(CHAR(10),LeaveDate,101) +  LeaveType 
	                                            FROM @Leave
	                                            GROUP BY  EmployeeId 
		                                            , LeaveDate 
		                                            , LeaveType 
	                                            HAVING SUM(LeaveHours) = 0)

	                                            SELECT * FROM @Leave", EmployeeCondition, ProcessPayrollPeriod, PayrollStart, PayrollEnd);
            #endregion
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public void ComputeLeaveLateUndertime(DataRow[] drArrLeaveAppPaid, DataRow[] drArrLeaveAppUnpaid, string EmployeeId, string ProcessDate
                                            , int ActualIN1, int ActualOUT1, int ActualIN2, int ActualOUT2
                                            , int ShiftStart, int ShiftBreakStart, int ShiftBreakEnd, int ShiftEnd
                                            , bool IsGraveyard, int PaidBreakMins
                                            , ref int iLate1, ref int iLate2, ref int iUndertime1, ref int iUndertime2
                                            , ref int iPaidLeave, ref int iUnpaidLeave, ref int iExcessLeave, ref int iOBLeave, ref int iLeaveOnPaidBreak)
        {
            #region Variable Declaration
            int LeaveStartMin;
            int LeaveEndMin;
            int iLeaveMins = 0;
            int iRegMins = 0;
            int iLeaveOnLate1 = 0;
            int iExcessLeaveInAM = 0;
            int iLeaveOnLate2 = 0;
            int iLeaveOnUndertime1 = 0;
            int iExcessLeaveInPM = 0;
            int iLeaveOnUndertime2 = 0;
            int iDeductPaidLeaveHours = 0;
            int iDeductUnpaidLeaveHours = 0;
            bool bPaidLeave = false;
            bool bLeaveWithCredit = false;
            bool bCancelledLeave = false;
            int iOrigPaidLeaveMins = 0;
            int iOrigUnpaidLeaveMins = 0;
            #endregion

            #region Initialization
            //assume leaves don't overlap
            //leave applications are unique
            DataRow[] drArrLeaveApp = new DataRow[drArrLeaveAppPaid.Length + drArrLeaveAppUnpaid.Length];
            Array.Copy(drArrLeaveAppPaid, drArrLeaveApp, drArrLeaveAppPaid.Length);
            Array.Copy(drArrLeaveAppUnpaid, 0, drArrLeaveApp, drArrLeaveAppPaid.Length, drArrLeaveAppUnpaid.Length);

            if (ActualIN1 == ActualOUT1)
            {
                ActualIN1 = 0;
                ActualOUT1 = 0;
            }
            if (ActualIN2 == ActualOUT2)
            {
                ActualIN2 = 0;
                ActualOUT2 = 0;
            }
            if (ActualIN1 == ActualOUT2)
            {
                ActualIN1 = 0;
                ActualOUT2 = 0;
            }

            //initial late and undertime
            iLate1 = GetOTHoursInMinutes(ShiftStart, ShiftBreakStart, ShiftStart, ActualIN1);
            iUndertime1 = GetOTHoursInMinutes(ShiftStart, ShiftBreakStart, ActualOUT1, ShiftBreakStart);
            iLate2 = GetOTHoursInMinutes(ShiftBreakEnd, ShiftEnd, ShiftBreakEnd, ActualIN2);
            iUndertime2 = GetOTHoursInMinutes(ShiftBreakEnd, ShiftEnd, ActualOUT2, ShiftEnd);
            #endregion

            foreach (DataRow drLeaveApp in drArrLeaveApp)
            {
                #region Loop Initialization
                LeaveStartMin = GetMinsFromHourStr(drLeaveApp["StartTime"].ToString());
                LeaveEndMin = GetMinsFromHourStr(drLeaveApp["EndTime"].ToString());
                if (LeaveStartMin == 0 && LeaveEndMin == 0)
                    continue;

                bPaidLeave = Convert.ToBoolean(drLeaveApp["PaidLeave"]);
                bLeaveWithCredit = Convert.ToBoolean(drLeaveApp["WithCredit"]);

                if (Convert.ToDouble(drLeaveApp["LeaveHours"]) < 0)
                    bCancelledLeave = true;
                else
                    bCancelledLeave = false;

                if (IsGraveyard) //Graveyard shift 
                {
                    if (LeaveStartMin < (ShiftStart - LOGPAD))
                    {
                        LeaveStartMin += GRAVEYARD24;
                    }
                    if (LeaveEndMin < (ShiftEnd - LOGPAD))
                    {
                        LeaveEndMin += GRAVEYARD24;
                    }
                }

                if (Convert.ToBoolean(FLEXSHIFT) == true)
                {
                    if (bPaidLeave == true)
                        iOrigPaidLeaveMins += Convert.ToInt32(Convert.ToDouble(drLeaveApp["LeaveHours"]) * 60);
                    else
                        iOrigUnpaidLeaveMins += Convert.ToInt32(Convert.ToDouble(drLeaveApp["LeaveHours"]) * 60);
                }
                #endregion

                if (Convert.ToBoolean(FLEXSHIFT) == false)
                {
                    #region AM
                    //Late
                    iLeaveOnLate1 = 0;
                    if (ActualIN1 != 0 && ActualIN1 > ShiftStart && ActualIN1 < ShiftBreakStart)
                    {
                        iLeaveOnLate1 = GetOTHoursInMinutes(LeaveStartMin, LeaveEndMin, ShiftStart, ActualIN1);
                        if (bPaidLeave == true)
                        {
                            iLate1 -= iLeaveOnLate1;
                            if (bLeaveWithCredit == true)
                                iPaidLeave += iLeaveOnLate1;
                        }
                        else
                        {
                            iLate1 -= iLeaveOnLate1;
                            iUnpaidLeave += iLeaveOnLate1;
                        }
                    }

                    //Undertime
                    iLeaveOnUndertime1 = 0;
                    if (ActualOUT1 != 0 && ActualOUT1 > ShiftStart && ActualOUT1 < ShiftBreakStart)
                    {
                        iLeaveOnUndertime1 = GetOTHoursInMinutes(LeaveStartMin, LeaveEndMin, ActualOUT1, ShiftBreakStart);
                        if (bPaidLeave == true)
                        {
                            iUndertime1 -= iLeaveOnUndertime1;
                            if (bLeaveWithCredit == true)
                                iPaidLeave += iLeaveOnUndertime1;
                        }
                        else
                        {
                            if (drLeaveApp["LeaveType"].ToString() != "UN")
                            {
                                iUndertime1 -= iLeaveOnUndertime1;
                                iUnpaidLeave += iLeaveOnUndertime1;
                            }
                        }
                    }

                    //Leave
                    if (ActualIN1 > 0 && ActualOUT1 > 0)
                    {
                        if (ActualIN1 != ActualOUT1 && Math.Min(Math.Min(ShiftBreakStart, ActualOUT1), LeaveEndMin) - Math.Max(Math.Max(ShiftStart, ActualIN1), LeaveStartMin) >= 0)
                            iRegMins = Math.Min(Math.Min(ShiftBreakStart, ActualOUT1), LeaveEndMin) - Math.Max(Math.Max(ShiftStart, ActualIN1), LeaveStartMin);
                        else
                            iRegMins = -1;
                    }
                    else
                        iRegMins = 0;
                    iLeaveMins = Math.Min(ShiftBreakStart, LeaveEndMin) - Math.Max(ShiftStart, LeaveStartMin);
                    if (bPaidLeave == true && bLeaveWithCredit == false)
                        iExcessLeaveInAM = GetOTHoursInMinutes(ActualIN1, ActualOUT1, Math.Max(ShiftStart, LeaveStartMin), Math.Min(ShiftBreakStart, LeaveEndMin));
                    else
                        iExcessLeaveInAM = GetOTHoursInMinutes(ActualIN1, ActualOUT1, LeaveStartMin, LeaveEndMin);

                    //Checking of excess leave
                    if (bLeaveWithCredit == true)
                        iExcessLeave += iExcessLeaveInAM;

                    if (bPaidLeave == true) //Paid leave and OB
                    {
                        if (iExcessLeaveInAM > 0) //with regular hours
                        {
                            if (bLeaveWithCredit == true)
                            {
                                if (iRegMins - iExcessLeaveInAM > 0)
                                {
                                    iPaidLeave += (iRegMins - iExcessLeaveInAM);
                                    if (bCancelledLeave)
                                        iDeductPaidLeaveHours += (iRegMins - iExcessLeaveInAM); //for cancelled leave hour accumulation
                                }
                            }
                            else
                                iOBLeave += iLeaveMins; ///OB
                        }
                        else if (iLeaveMins > 0
                                    && ((bLeaveWithCredit == true && iLeaveOnLate1 + iLeaveOnUndertime1 == 0 && (iRegMins == 0 || iRegMins == -1))
                                        || (bLeaveWithCredit == false && iRegMins <= 0))) //no logs or OB
                        {
                            if (bLeaveWithCredit == true)
                            {
                                iPaidLeave += iLeaveMins;
                                if (bCancelledLeave)
                                    iDeductPaidLeaveHours += iLeaveMins; //for cancelled leave hour accumulation
                            }
                            else
                                iOBLeave += iLeaveMins; ///OB

                            if (iRegMins == 0 || iRegMins == -1)
                            {
                                if (iLate1 > iLeaveMins)
                                    iLate1 -= iLeaveMins;
                                else
                                    iLate1 = 0;
                                if (iUndertime1 > iLeaveMins)
                                    iUndertime1 -= iLeaveMins;
                                else
                                    iUndertime1 = 0;
                            }
                        }
                    }
                    else if (bPaidLeave == false) //Unpaid Leave
                    {
                        if (iRegMins == 0 && iLeaveOnLate1 + iLeaveOnUndertime1 == 0 && iLeaveMins > 0 && drLeaveApp["LeaveType"].ToString() != "UN") //no logs
                        {
                            iUnpaidLeave += iLeaveMins;
                            if (bCancelledLeave)
                                iDeductUnpaidLeaveHours += iLeaveMins; //for cancelled leave hour accumulation

                            if (iLate1 > iLeaveMins)
                                iLate1 -= iLeaveMins;
                            else
                                iLate1 = 0;
                            if (iUndertime1 > iLeaveMins)
                                iUndertime1 -= iLeaveMins;
                            else
                                iUndertime1 = 0;
                        }
                    }
                    #endregion

                    #region PM
                    //Late
                    iLeaveOnLate2 = 0;
                    if (ActualIN2 != 0 && ActualIN2 > ShiftBreakEnd && ActualIN2 < ShiftEnd)
                    {
                        iLeaveOnLate2 = GetOTHoursInMinutes(LeaveStartMin, LeaveEndMin, ShiftBreakEnd, ActualIN2);
                        if (bPaidLeave == true)
                        {
                            iLate2 -= iLeaveOnLate2;
                            if (bLeaveWithCredit == true)
                                iPaidLeave += iLeaveOnLate2;
                        }
                        else
                        {
                            iLate2 -= iLeaveOnLate2;
                            iUnpaidLeave += iLeaveOnLate2;
                        }
                    }

                    //Undertime
                    iLeaveOnUndertime2 = 0;
                    if (ActualOUT2 != 0 && ActualOUT2 > ShiftBreakEnd && ActualOUT2 < ShiftEnd)
                    {
                        iLeaveOnUndertime2 = GetOTHoursInMinutes(LeaveStartMin, LeaveEndMin, ActualOUT2, ShiftEnd);
                        if (bPaidLeave == true)
                        {
                            iUndertime2 -= iLeaveOnUndertime2;
                            if (bLeaveWithCredit == true)
                                iPaidLeave += iLeaveOnUndertime2;
                        }
                        else
                        {
                            if (drLeaveApp["LeaveType"].ToString() != "UN")
                            {
                                iUndertime2 -= iLeaveOnUndertime2;
                                iUnpaidLeave += iLeaveOnUndertime2;
                            }
                        }
                    }

                    //Leave
                    if (ActualIN2 > 0 && ActualOUT2 > 0)
                    {
                        if (ActualIN2 != ActualOUT2 && Math.Min(Math.Min(ShiftEnd, ActualOUT2), LeaveEndMin) - Math.Max(Math.Max(ShiftBreakEnd, ActualIN2), LeaveStartMin) >= 0)
                            iRegMins = Math.Min(Math.Min(ShiftEnd, ActualOUT2), LeaveEndMin) - Math.Max(Math.Max(ShiftBreakEnd, ActualIN2), LeaveStartMin);
                        else
                            iRegMins = -1;
                    }
                    else
                        iRegMins = 0;
                    iLeaveMins = Math.Min(ShiftEnd, LeaveEndMin) - Math.Max(ShiftBreakEnd, LeaveStartMin);
                    if (bPaidLeave == true && bLeaveWithCredit == false)
                        iExcessLeaveInPM = GetOTHoursInMinutes(ActualIN2, ActualOUT2, Math.Max(ShiftBreakEnd, LeaveStartMin), Math.Min(ShiftEnd, LeaveEndMin));
                    else
                        iExcessLeaveInPM = GetOTHoursInMinutes(ActualIN2, ActualOUT2, LeaveStartMin, LeaveEndMin);

                    //Checking of excess leave
                    if (bLeaveWithCredit == true)
                        iExcessLeave += iExcessLeaveInPM;

                    //Checking of Paid Break
                    if (PaidBreakMins > 0
                        && (//(ActualIN1 > 0 && ActualOUT1 <= ShiftBreakStart && ActualIN2 >= ShiftBreakEnd && ActualOUT2 > 0) ||
                            (ActualIN1 > 0 && ActualOUT1 <= ShiftBreakStart && ActualIN2 == 0 && ActualOUT2 == 0)
                            || (ActualIN1 == 0 && ActualOUT1 == 0 && ActualIN2 >= ShiftBreakEnd && ActualOUT2 > 0)
                            || (ActualIN1 == 0 && ActualOUT1 == 0 && ActualIN2 == 0 && ActualOUT2 == 0))
                        ) //include paid break in leave only when undertime in the morning or afternoon, or whole day absent
                    {
                        iLeaveOnPaidBreak = GetOTHoursInMinutes(ShiftBreakStart, ShiftBreakEnd, LeaveStartMin, LeaveEndMin);
                        if (bPaidLeave == true)
                        {
                            if (bLeaveWithCredit == true)
                                iPaidLeave += iLeaveOnPaidBreak;
                            else
                                iOBLeave += iLeaveOnPaidBreak;
                        }
                        else
                        {
                            if (drLeaveApp["LeaveType"].ToString() != "UN")
                                iUnpaidLeave += iLeaveOnPaidBreak;
                            else
                                iLeaveOnPaidBreak = 0;
                        }
                    }

                    if (bPaidLeave == true) //Paid leave and OB
                    {
                        if (iExcessLeaveInPM > 0) //with regular hours
                        {
                            if (bLeaveWithCredit == true)
                            {
                                if (iRegMins - iExcessLeaveInPM > 0)
                                {
                                    iPaidLeave += (iRegMins - iExcessLeaveInPM);
                                    if (bCancelledLeave)
                                        iDeductPaidLeaveHours += (iRegMins - iExcessLeaveInPM); //for cancelled leave hour accumulation
                                }
                            }
                            else
                                iOBLeave += iLeaveMins; ///OB
                        }
                        else if (iLeaveMins > 0
                                    && ((bLeaveWithCredit == true && iLeaveOnLate2 + iLeaveOnUndertime2 == 0 && (iRegMins == 0 || iRegMins == -1))
                                        || (bLeaveWithCredit == false && iRegMins <= 0))) //no logs or OB
                        {
                            if (bLeaveWithCredit == true)
                            {
                                iPaidLeave += iLeaveMins;
                                if (bCancelledLeave)
                                    iDeductPaidLeaveHours += iLeaveMins; //for cancelled leave hour accumulation
                            }
                            else
                                iOBLeave += iLeaveMins; ///OB

                            if (iRegMins == 0 || iRegMins == -1)
                            {
                                if (iLate2 > iLeaveMins)
                                    iLate2 -= iLeaveMins;
                                else
                                    iLate2 = 0;
                                if (iUndertime2 > iLeaveMins)
                                    iUndertime2 -= iLeaveMins;
                                else
                                    iUndertime2 = 0;
                            }
                        }
                    }
                    else if (bPaidLeave == false) //Unpaid Leave
                    {
                        if (iRegMins == 0 && iLeaveOnLate2 + iLeaveOnUndertime2 == 0 && iLeaveMins > 0 && drLeaveApp["LeaveType"].ToString() != "UN") //no logs
                        {
                            iUnpaidLeave += iLeaveMins;
                            if (bCancelledLeave)
                                iDeductUnpaidLeaveHours += iLeaveMins; //for cancelled leave hour accumulation

                            if (iLate2 > iLeaveMins)
                                iLate2 -= iLeaveMins;
                            else
                                iLate2 = 0;
                            if (iUndertime2 > iLeaveMins)
                                iUndertime2 -= iLeaveMins;
                            else
                                iUndertime2 = 0;
                        }
                    }
                    #endregion
                }
            }

            iPaidLeave = iPaidLeave - (iDeductPaidLeaveHours * 2);
            iUnpaidLeave = iUnpaidLeave - (iDeductUnpaidLeaveHours * 2);

            if (Convert.ToBoolean(FLEXSHIFT) == true
                && (iOrigPaidLeaveMins > 0 || iOrigUnpaidLeaveMins > 0))
            {
                iPaidLeave = iOrigPaidLeaveMins - (iDeductPaidLeaveHours * 2);
                iUnpaidLeave = iOrigUnpaidLeaveMins - (iDeductUnpaidLeaveHours * 2);

                iUndertime2 = iLate1 + iLate2 + iUndertime1 + iUndertime2;
                iLate1 = 0;
                iLate2 = 0;
                iUndertime1 = 0;

                if (iPaidLeave > 0)
                {
                    if (iUndertime2 - iPaidLeave > 0)
                        iUndertime2 = iUndertime2 - iPaidLeave;
                    else
                        iUndertime2 = 0;
                }
                if (iUnpaidLeave > 0)
                {
                    if (iUndertime2 - iUnpaidLeave > 0)
                        iUndertime2 = iUndertime2 - iUnpaidLeave;
                    else
                        iUndertime2 = 0;
                }
            }
        }

        public int GetActualLeaveHourTwoPackets(string EmployeeId, string ProcessDate
                                                , int ActualIN, int ActualOUT, int ShiftStart, int ShiftBreakStart, int ShiftBreakEnd, int ShiftEnd
                                                , ref int ExcessLeave, ref int LeaveMinToBeAddedToReg, bool IsGraveyard, int PaidBreak, bool bIsPaidLeave)
        {
            DataRow[] drArrLeaveApp;
            int LeaveStartMin;
            int LeaveEndMin;
            int LeaveMins = 0;
            int tempIN;
            int tempOUT;
            int tempUsedLeaveMins;
            bool bPaidBreakPassed = false;
            int iDeductLeaveHours = 0;
            bool bCancelledLeave = false;
            bool bPaidBreakWithCredit = false;

            if (bIsPaidLeave == true)
                drArrLeaveApp = GetCorrectedLeaveRecords(EmployeeId, ProcessDate, true, IsGraveyard, ShiftStart, ShiftEnd);
            else
                drArrLeaveApp = GetCorrectedLeaveRecords(EmployeeId, ProcessDate, false, IsGraveyard, ShiftStart, ShiftEnd);

            foreach (DataRow drLeaveApp in drArrLeaveApp)
            {
                LeaveStartMin = GetMinsFromHourStr(drLeaveApp["StartTime"].ToString());
                LeaveEndMin = GetMinsFromHourStr(drLeaveApp["EndTime"].ToString());

                if (Convert.ToDouble(drLeaveApp["LeaveHours"]) < 0)
                    bCancelledLeave = true;
                else
                    bCancelledLeave = false;

                if (IsGraveyard) //Graveyard shift 
                {
                    if (LeaveStartMin < (ShiftStart - LOGPAD))
                    {
                        LeaveStartMin += GRAVEYARD24;
                    }
                    if (LeaveEndMin < (ShiftEnd - LOGPAD))
                    {
                        LeaveEndMin += GRAVEYARD24;
                    }
                }

                tempIN = Math.Max(ShiftStart, LeaveStartMin);
                tempOUT = Math.Min(ShiftEnd, LeaveEndMin);
                tempUsedLeaveMins = GetOTHoursInMinutes(ActualIN, ActualOUT, tempIN, tempOUT);

                //checking of excess leave
                if (Convert.ToBoolean(drLeaveApp["WithCredit"]))
                    ExcessLeave += tempUsedLeaveMins;
                if (tempOUT - tempIN - tempUsedLeaveMins > 0)
                {
                    if (Convert.ToBoolean(drLeaveApp["WithCredit"]))
                    {
                        LeaveMins += (tempOUT - tempIN - tempUsedLeaveMins);
                        if (bCancelledLeave)
                            iDeductLeaveHours += (tempOUT - tempIN - tempUsedLeaveMins); //for cancelled leave hour accumulation
                    }
                    else
                        LeaveMinToBeAddedToReg += (tempOUT - tempIN - tempUsedLeaveMins); ///OB
                }

                if (GetOTHoursInMinutes(ShiftBreakStart, ShiftBreakEnd, LeaveStartMin, LeaveEndMin) > 0)
                {
                    bPaidBreakPassed = true;
                    bPaidBreakWithCredit = Convert.ToBoolean(drLeaveApp["WithCredit"]);
                }
            }

            if (PaidBreak > 0 && bPaidBreakPassed)
            {
                if (bPaidBreakWithCredit)
                    LeaveMins += PaidBreak;
                else
                    LeaveMinToBeAddedToReg += PaidBreak;
            }

            LeaveMins = LeaveMins - (iDeductLeaveHours * 2);
            return LeaveMins;
        }

        public void ComputeOvertimeForOBEx(string EmployeeId, string ProcessDate
                                            , int iShiftTimeIn1Min, int iShiftTimeOut1Min, int iShiftTimeIn2Min, int iShiftTimeOut2Min
                                            , ref int iComputedOvertimeMin, ref int iCompRegNightPremMin, ref int iCompOvertimeNightPremMin
                                            , string strDayCode, bool bRestDay, bool bIsGraveyard
                                            , bool bCountOTFraction, int iOTFraction, int iNDFraction, int iMasterPaidBreak, bool CheckOT
                                            , DataRow[] drArrOTApproved)
        {
            int LeaveStartMin, LeaveEndMin;
            int iOTStartMin, iOTEndMin;
            int iConvTimeInExtMin, iConvTimeOutExtMin;
            int i, iAdvOTMin, iOTTemp, iOTTemp2, iTimeMinTemp, iTimeMinTemp2, iTempMultPockVar, iNDSum, iPaidBreak = 0;
            string strOTType;
            DataRow[] drArrLeaveApp = dtLeaveTable.Select("EmployeeId = '" + EmployeeId + "' AND LeaveDate = '" + ProcessDate + "' AND WithCredit = 0 AND CombinedLeave = 0 AND PaidLeave = 1"
                                                            , "LeaveType ASC, LeaveHours ASC, StartTime ASC, EndTime ASC");

            foreach (DataRow drLeave in drArrLeaveApp)
            {
                if (Convert.ToDouble(drLeave["LeaveHours"]) != 0)
                {
                    #region Leave Initialization
                    LeaveStartMin = GetMinsFromHourStr(drLeave["StartTime"].ToString());
                    LeaveEndMin = GetMinsFromHourStr(drLeave["EndTime"].ToString());
                    if (bIsGraveyard == true)
                    {
                        if (LeaveStartMin < (iShiftTimeIn1Min - LOGPAD))
                        {
                            LeaveStartMin += GRAVEYARD24;
                        }
                        if (LeaveEndMin < (iShiftTimeOut2Min - LOGPAD))
                        {
                            LeaveEndMin += GRAVEYARD24;
                        }
                    }
                    #endregion

                    i = -1; //initial value
                    do
                    {
                        #region OT Initialization
                        i++;
                        iConvTimeInExtMin = 0;
                        iConvTimeOutExtMin = 0;
                        iOTStartMin = 0;
                        iOTEndMin = 0;
                        strOTType = "";

                        if (CheckOT == true && i < drArrOTApproved.Length)
                        {
                            iOTStartMin = GetMinsFromHourStr(drArrOTApproved[i]["Eot_StartTime"].ToString());
                            iOTEndMin = GetMinsFromHourStr(drArrOTApproved[i]["Eot_EndTime"].ToString());
                            strOTType = drArrOTApproved[i]["Eot_OvertimeType"].ToString();

                            #region OT Application Validation
                            if (strOTType.Equals("A") && iOTEndMin > iShiftTimeIn1Min)
                            {
                                AddErrorToLaborHourReport(EmployeeId, dtEmployeeLogLedger.Rows[i]["Emt_LastName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_FirstName"].ToString(), dtEmployeeLogLedger.Rows[i]["Emt_MiddleName"].ToString(), ProcessDate, "Advance OT Application Error");
                                continue; //skip erroneous OT application
                            }

                            if (bIsGraveyard && strOTType.Equals("P")) //Graveyard shift and Post-overtime
                            {
                                if (iOTStartMin < (iShiftTimeIn1Min - LOGPAD))
                                {
                                    iOTStartMin += GRAVEYARD24;
                                }
                                if (iOTEndMin < (iShiftTimeOut2Min - LOGPAD))
                                {
                                    iOTEndMin += GRAVEYARD24;
                                }
                            }
                            #endregion

                            iConvTimeInExtMin = (LeaveStartMin > iOTStartMin) ? LeaveStartMin : iOTStartMin;
                            iConvTimeOutExtMin = (LeaveEndMin < iOTEndMin) ? LeaveEndMin : iOTEndMin;
                        }
                        else if (CheckOT == false)
                        {
                            iConvTimeInExtMin = LeaveStartMin;
                            iConvTimeOutExtMin = LeaveEndMin;
                        }
                        #endregion

                        if (CheckOT == false || drArrOTApproved.Length > 0)
                        {
                            if (bRestDay) //Rest day or holiday
                            {
                                #region Computed Overtime Minutes
                                //[AM OT = Get OT between iConvTimeInExtMin and Break Start]
                                iOTTemp = iConvTimeInExtMin;
                                //iOTTemp = (iShiftTimeIn1Min > iConvTimeInExtMin) ? iShiftTimeIn1Min : iConvTimeInExtMin;
                                iOTTemp2 = (iShiftTimeOut1Min < iConvTimeOutExtMin) ? iShiftTimeOut1Min : iConvTimeOutExtMin;
                                iAdvOTMin = GetOTHoursInMinutes(iOTStartMin, iOTEndMin, iOTTemp, iOTTemp2);
                                if (bCountOTFraction == true)
                                {
                                    iAdvOTMin = Convert.ToInt32(iAdvOTMin / iOTFraction) * iOTFraction;
                                }
                                iComputedOvertimeMin += iAdvOTMin;

                                #region Insert Overtime to Hour Fraction Table
                                InsertOTToHourFractionTable(iOTStartMin, iOTEndMin, iOTTemp, iOTTemp2, iOTFraction, bCountOTFraction);
                                #endregion

                                //[PM OT = Get OT between Break End and iConvTimeOutExtMin]
                                iOTTemp = (iShiftTimeIn2Min > iConvTimeInExtMin) ? iShiftTimeIn2Min : iConvTimeInExtMin;
                                //iOTTemp2 = (iShiftTimeOut2Min < iConvTimeOutExtMin) ? iShiftTimeOut2Min : iConvTimeOutExtMin;
                                iOTTemp2 = iConvTimeOutExtMin;
                                iAdvOTMin = GetOTHoursInMinutes(iOTStartMin, iOTEndMin, iOTTemp, iOTTemp2);
                                if (bCountOTFraction == true)
                                {
                                    iAdvOTMin = Convert.ToInt32(iAdvOTMin / iOTFraction) * iOTFraction;
                                }
                                iComputedOvertimeMin += iAdvOTMin;

                                #region Insert Overtime to Hour Fraction Table
                                InsertOTToHourFractionTable(iOTStartMin, iOTEndMin, iOTTemp, iOTTemp2, iOTFraction, bCountOTFraction);
                                #endregion

                                if (bCountOTFraction == false)
                                {
                                    iComputedOvertimeMin = Convert.ToInt32(iComputedOvertimeMin / iOTFraction) * iOTFraction;
                                    CorrectOTHourFraction(iComputedOvertimeMin);
                                }

                                //Paid Break for Rest day
                                if (iConvTimeInExtMin > 0 && iConvTimeOutExtMin > 0)
                                {
                                    iOTTemp = (iShiftTimeOut1Min > iOTStartMin) ? iShiftTimeOut1Min : iOTStartMin;
                                    iOTTemp2 = (iShiftTimeIn2Min < iOTEndMin) ? iShiftTimeIn2Min : iOTEndMin;
                                    iPaidBreak += GetOTHoursInMinutes(iOTTemp, iOTTemp2, iConvTimeInExtMin, iConvTimeOutExtMin);

                                    if (iPaidBreak > iMasterPaidBreak) //Must not exceed the set paid break
                                    {
                                        iPaidBreak = iMasterPaidBreak;
                                    }
                                    else
                                    {
                                        if (MIDOT == false)
                                        {
                                            #region Insert Paid Break to Hour Fraction Table
                                            InsertOTToHourFractionTable(iOTTemp, iOTTemp2, iConvTimeInExtMin, iConvTimeOutExtMin, iOTFraction, bCountOTFraction);
                                            #endregion
                                        }
                                    }
                                }

                                if (MIDOT == true && strOTType.Equals("M"))
                                {
                                    iPaidBreak = 0;
                                }
                                #endregion

                                if (NDBRCKTCNT == 2)
                                {
                                    #region Computed Overtime Night Premium (Sharp)
                                    iTimeMinTemp = (iConvTimeInExtMin > iOTStartMin) ? iConvTimeInExtMin : iOTStartMin;
                                    iTimeMinTemp2 = (iConvTimeOutExtMin < iOTEndMin) ? iConvTimeOutExtMin : iOTEndMin;

                                    #region ND Bracket 1
                                    //[NDOTHr = Get NDOT between NP1_BEGTIME/OT Start to NP1_ENDTIME/OT End]
                                    iOTTemp = (NP1_BEGTIME > iOTStartMin) ? NP1_BEGTIME : iOTStartMin;
                                    iOTTemp2 = (NP1_ENDTIME < iOTEndMin) ? NP1_ENDTIME : iOTEndMin;
                                    iCompRegNightPremMin += GetOTHoursInMinutes(iOTTemp, iOTTemp2, iTimeMinTemp, iTimeMinTemp2);
                                    #region Insert ND Hour to Hour Fraction Table
                                    InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iTimeMinTemp, iTimeMinTemp2, NDFRACTION, HourType.NDHour);
                                    #endregion
                                    #endregion

                                    #region ND Bracket 2
                                    //[NDOTHr = Get NDOT between NP2_BEGTIME/OT Start to NP2_ENDTIME/OT End]
                                    iOTTemp = (NP2_BEGTIME > iOTStartMin) ? NP2_BEGTIME : iOTStartMin;
                                    iOTTemp2 = (NP2_ENDTIME < iOTEndMin) ? NP2_ENDTIME : iOTEndMin;
                                    iCompOvertimeNightPremMin += GetOTHoursInMinutes(iOTTemp, iOTTemp2, iTimeMinTemp, iTimeMinTemp2);
                                    #region Insert ND Hour to Hour Fraction Table
                                    InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iTimeMinTemp, iTimeMinTemp2, NDFRACTION, HourType.NDOTHour);
                                    #endregion
                                    #endregion
                                    #endregion
                                }
                                else
                                {
                                    #region Computed Overtime Night Premium (Normal)
                                    iTimeMinTemp = (iConvTimeInExtMin > iOTStartMin) ? iConvTimeInExtMin : iOTStartMin;
                                    iTimeMinTemp2 = (iConvTimeOutExtMin < iOTEndMin) ? iConvTimeOutExtMin : iOTEndMin;

                                    ///OVERTIME NIGHT PREMIUM FOR DAY SHIFTS
                                    if (!Convert.ToBoolean(NONDOTDAY)) //HOYA
                                    {
                                        //[NDOTHr = Get NDOT between 00:00/OT Start to 06:00/OT End]
                                        iOTTemp = (NIGHTDIFFAMSTART > iOTStartMin) ? NIGHTDIFFAMSTART : iOTStartMin;
                                        iOTTemp2 = (NIGHTDIFFAMEND < iOTEndMin) ? NIGHTDIFFAMEND : iOTEndMin;
                                        iCompOvertimeNightPremMin += GetOTHoursInMinutes(iOTTemp, iOTTemp2, iTimeMinTemp, iTimeMinTemp2);
                                        #region Insert ND Hour to Hour Fraction Table
                                        InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iTimeMinTemp, iTimeMinTemp2, NDFRACTION, HourType.NDOTHour);
                                        #endregion
                                    }

                                    ///OVERTIME NIGHT PREMIUM FOR GRAVEYARD SHIFTS
                                    //[NDOTHr = Get NDOT between 22:00/OT Start to 30:00/OT End]
                                    iOTTemp = (NIGHTDIFFGRAVEYARDSTART > iOTStartMin) ? NIGHTDIFFGRAVEYARDSTART : iOTStartMin;
                                    iOTTemp2 = (NIGHTDIFFGRAVEYARDEND < iOTEndMin) ? NIGHTDIFFGRAVEYARDEND : iOTEndMin;
                                    iCompOvertimeNightPremMin += GetOTHoursInMinutes(iOTTemp, iOTTemp2, iTimeMinTemp, iTimeMinTemp2);
                                    #region Insert ND Hour to Hour Fraction Table
                                    InsertNDToHourFractionTable(iOTTemp, iOTTemp2, iTimeMinTemp, iTimeMinTemp2, NDFRACTION, HourType.NDOTHour);
                                    #endregion
                                    #endregion
                                }
                            }
                            else //Regular day
                            {
                                #region Computed Overtime Minutes
                                //[Get OT between iConvTimeInExtMin and iConvTimeOutExtMin]
                                iAdvOTMin = GetOTHoursInMinutes(iOTStartMin, iOTEndMin, iConvTimeInExtMin, iConvTimeOutExtMin);
                                if (bCountOTFraction == true)
                                {
                                    iAdvOTMin = Convert.ToInt32(iAdvOTMin / iOTFraction) * iOTFraction;
                                }
                                iComputedOvertimeMin += iAdvOTMin;

                                #region Insert Overtime to Hour Fraction Table
                                InsertOTToHourFractionTable(iOTStartMin, iOTEndMin, iConvTimeInExtMin, iConvTimeOutExtMin, iOTFraction, bCountOTFraction);
                                #endregion

                                if (bCountOTFraction == false)
                                {
                                    iComputedOvertimeMin = Convert.ToInt32(iComputedOvertimeMin / iOTFraction) * iOTFraction;
                                    CorrectOTHourFraction(iComputedOvertimeMin);
                                }
                                #endregion

                                if (NDBRCKTCNT == 2)
                                {
                                    #region Computed Overtime Night Premium (Sharp)
                                    iTimeMinTemp = iConvTimeInExtMin;
                                    iTimeMinTemp2 = iConvTimeOutExtMin;

                                    if (bIsGraveyard) //Graveyard shift
                                    {
                                        #region ND Bracket 1
                                        //[NDOTHr = Get NDOT between NP1_BEGTIME to iConvTimeInExtMin]
                                        iTimeMinTemp = (NP1_BEGTIME > iTimeMinTemp) ? NP1_BEGTIME : iTimeMinTemp;
                                        iTempMultPockVar = (iConvTimeInExtMin > iShiftTimeIn1Min) ? iConvTimeInExtMin : iShiftTimeIn1Min;
                                        iCompRegNightPremMin += GetOTHoursInMinutes(iOTStartMin, iOTEndMin, iTimeMinTemp, iTempMultPockVar);
                                        #region Insert ND Hour to Hour Fraction Table
                                        InsertNDToHourFractionTable(iOTStartMin, iOTEndMin, iTimeMinTemp, iTempMultPockVar, NDFRACTION, HourType.NDHour);
                                        #endregion

                                        //[NDOTHr = Get NDOT between iConvTimeOutExtMin to NP1_ENDTIME]
                                        iTimeMinTemp2 = (iTimeMinTemp2 < NP1_ENDTIME) ? iTimeMinTemp2 : NP1_ENDTIME;
                                        iTempMultPockVar = (iConvTimeOutExtMin < iShiftTimeOut2Min) ? iConvTimeOutExtMin : iShiftTimeOut2Min;
                                        iCompRegNightPremMin += GetOTHoursInMinutes(iOTStartMin, iOTEndMin, iTempMultPockVar, iTimeMinTemp2);
                                        #region Insert ND Hour to Hour Fraction Table
                                        InsertNDToHourFractionTable(iOTStartMin, iOTEndMin, iTempMultPockVar, iTimeMinTemp2, NDFRACTION, HourType.NDHour);
                                        #endregion
                                        #endregion

                                        #region ND Bracket 2
                                        //[NDOTHr = Get NDOT between NP2_BEGTIME to iConvTimeInExtMin]
                                        iTimeMinTemp = (NP2_BEGTIME > iTimeMinTemp) ? NP2_BEGTIME : iTimeMinTemp;
                                        iTempMultPockVar = (iConvTimeInExtMin > iShiftTimeIn1Min) ? iConvTimeInExtMin : iShiftTimeIn1Min;
                                        iCompOvertimeNightPremMin += GetOTHoursInMinutes(iOTStartMin, iOTEndMin, iTimeMinTemp, iTempMultPockVar);
                                        #region Insert ND Hour to Hour Fraction Table
                                        InsertNDToHourFractionTable(iOTStartMin, iOTEndMin, iTimeMinTemp, iTempMultPockVar, NDFRACTION, HourType.NDOTHour);
                                        #endregion

                                        //[NDOTHr = Get NDOT between iConvTimeOutExtMin to NP2_ENDTIME]
                                        iTimeMinTemp2 = (iTimeMinTemp2 < NP2_ENDTIME) ? iTimeMinTemp2 : NP2_ENDTIME;
                                        iTempMultPockVar = (iConvTimeOutExtMin < iShiftTimeOut2Min) ? iConvTimeOutExtMin : iShiftTimeOut2Min;
                                        iCompOvertimeNightPremMin += GetOTHoursInMinutes(iOTStartMin, iOTEndMin, iTempMultPockVar, iTimeMinTemp2);
                                        #region Insert ND Hour to Hour Fraction Table
                                        InsertNDToHourFractionTable(iOTStartMin, iOTEndMin, iTempMultPockVar, iTimeMinTemp2, NDFRACTION, HourType.NDOTHour);
                                        #endregion
                                        #endregion
                                    }
                                    else
                                    {
                                        #region ND Bracket 1
                                        //[NDOTHr = Get NDOT between NP1_BEGTIME/ComputedOut2 to iConvTimeOutExtMin]
                                        iTimeMinTemp = (iTimeMinTemp > NP1_BEGTIME) ? iTimeMinTemp : NP1_BEGTIME;
                                        iOTEndMin = (iOTEndMin > NP1_ENDTIME) ? NP1_ENDTIME : iOTEndMin;
                                        iCompRegNightPremMin += GetOTHoursInMinutes(iOTStartMin, iOTEndMin, iTimeMinTemp, iConvTimeOutExtMin);
                                        #region Insert ND Hour to Hour Fraction Table
                                        InsertNDToHourFractionTable(iOTStartMin, iOTEndMin, iTimeMinTemp, iConvTimeOutExtMin, NDFRACTION, HourType.NDHour);
                                        #endregion
                                        #endregion

                                        #region ND Bracket 2
                                        //[NDOTHr = Get NDOT between NP2_BEGTIME/ComputedOut2 to iConvTimeOutExtMin]
                                        iTimeMinTemp = (iTimeMinTemp > NP2_BEGTIME) ? iTimeMinTemp : NP2_BEGTIME;
                                        iOTEndMin = (iOTEndMin > NP2_ENDTIME) ? NP2_ENDTIME : iOTEndMin;
                                        iCompOvertimeNightPremMin += GetOTHoursInMinutes(iOTStartMin, iOTEndMin, iTimeMinTemp, iConvTimeOutExtMin);
                                        #region Insert ND Hour to Hour Fraction Table
                                        InsertNDToHourFractionTable(iOTStartMin, iOTEndMin, iTimeMinTemp, iConvTimeOutExtMin, NDFRACTION, HourType.NDOTHour);
                                        #endregion
                                        #endregion
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region Computed Overtime Night Premium (Normal)
                                    iTimeMinTemp = iConvTimeInExtMin;
                                    iTimeMinTemp2 = iConvTimeOutExtMin;

                                    if (bIsGraveyard) //Graveyard shift
                                    {
                                        //[NDOTHr = Get NDOT between 22:00 to iConvTimeInExtMin]
                                        iTimeMinTemp = (NIGHTDIFFGRAVEYARDSTART > iTimeMinTemp) ? NIGHTDIFFGRAVEYARDSTART : iTimeMinTemp;
                                        iTempMultPockVar = (iConvTimeInExtMin > iShiftTimeIn1Min) ? iConvTimeInExtMin : iShiftTimeIn1Min;
                                        iCompOvertimeNightPremMin += GetOTHoursInMinutes(iOTStartMin, iOTEndMin, iTimeMinTemp, iTempMultPockVar);
                                        #region Insert ND Hour to Hour Fraction Table
                                        InsertNDToHourFractionTable(iOTStartMin, iOTEndMin, iTimeMinTemp, iTempMultPockVar, NDFRACTION, HourType.NDOTHour);
                                        #endregion

                                        //[NDOTHr = Get NDOT between iConvTimeOutExtMin to 30:00]
                                        iTimeMinTemp2 = (iTimeMinTemp2 < NIGHTDIFFGRAVEYARDEND) ? iTimeMinTemp2 : NIGHTDIFFGRAVEYARDEND;
                                        iTempMultPockVar = (iConvTimeOutExtMin < iShiftTimeOut2Min) ? iConvTimeOutExtMin : iShiftTimeOut2Min;
                                        iCompOvertimeNightPremMin += GetOTHoursInMinutes(iOTStartMin, iOTEndMin, iTempMultPockVar, iTimeMinTemp2);
                                        #region Insert ND Hour to Hour Fraction Table
                                        InsertNDToHourFractionTable(iOTStartMin, iOTEndMin, iTempMultPockVar, iTimeMinTemp2, NDFRACTION, HourType.NDOTHour);
                                        #endregion
                                    }
                                    else
                                    {
                                        if (!Convert.ToBoolean(NONDOTDAY)) //HOYA
                                        {
                                            //[NDOTHr = Get NDOT between iConvTimeInExtMin to 06:00]
                                            iTimeMinTemp2 = (NIGHTDIFFAMEND < iTimeMinTemp2) ? NIGHTDIFFAMEND : iTimeMinTemp2;
                                            iCompOvertimeNightPremMin += GetOTHoursInMinutes(iOTStartMin, iOTEndMin, iConvTimeInExtMin, iTimeMinTemp2);
                                            #region Insert ND Hour to Hour Fraction Table
                                            InsertNDToHourFractionTable(iOTStartMin, iOTEndMin, iConvTimeInExtMin, iTimeMinTemp2, NDFRACTION, HourType.NDOTHour);
                                            #endregion
                                        }
                                        //[NDOTHr = Get NDOT between 22:00/ComputedOut2 to iConvTimeOutExtMin]
                                        iTimeMinTemp = (iTimeMinTemp > NIGHTDIFFGRAVEYARDSTART) ? iTimeMinTemp : NIGHTDIFFGRAVEYARDSTART;
                                        iOTEndMin = (iOTEndMin > NIGHTDIFFGRAVEYARDEND) ? NIGHTDIFFGRAVEYARDEND : iOTEndMin;
                                        iCompOvertimeNightPremMin += GetOTHoursInMinutes(iOTStartMin, iOTEndMin, iTimeMinTemp, iConvTimeOutExtMin);
                                        #region Insert ND Hour to Hour Fraction Table
                                        InsertNDToHourFractionTable(iOTStartMin, iOTEndMin, iTimeMinTemp, iConvTimeOutExtMin, NDFRACTION, HourType.NDOTHour);
                                        #endregion
                                    }
                                    #endregion
                                }
                            }
                        }
                    }
                    while (i < drArrOTApproved.Length - 1);

                    if (NDBRCKTCNT == 2)
                    {
                        #region Computed Regular Night Premium for Regular Day (Sharp)
                        if (!bRestDay) //Regular day
                        {
                            #region ND Bracket 1
                            //[NDHr = Get ND between iShiftTimeIn1Min to iShiftTimeOut1Min]
                            iTimeMinTemp = (LeaveStartMin > iShiftTimeIn1Min) ? LeaveStartMin : iShiftTimeIn1Min;
                            iTimeMinTemp2 = (LeaveEndMin < iShiftTimeOut1Min) ? LeaveEndMin : iShiftTimeOut1Min;
                            iCompRegNightPremMin += GetOTHoursInMinutes(NP1_BEGTIME, NP1_ENDTIME, iTimeMinTemp, iTimeMinTemp2);
                            #region Insert ND Hour to Hour Fraction Table
                            InsertNDToHourFractionTable(NP1_BEGTIME, NP1_ENDTIME, iTimeMinTemp, iTimeMinTemp2, NDFRACTION, HourType.NDHour);
                            #endregion

                            //[NDHr = Get ND between iShiftTimeIn2Min to iShiftTimeOut2Min]
                            iTimeMinTemp = (LeaveStartMin > iShiftTimeIn2Min) ? LeaveStartMin : iShiftTimeIn2Min;
                            iTimeMinTemp2 = (LeaveEndMin < iShiftTimeOut2Min) ? LeaveEndMin : iShiftTimeOut2Min;
                            iCompRegNightPremMin += GetOTHoursInMinutes(NP1_BEGTIME, NP1_ENDTIME, iTimeMinTemp, iTimeMinTemp2);
                            #region Insert ND Hour to Hour Fraction Table
                            InsertNDToHourFractionTable(NP1_BEGTIME, NP1_ENDTIME, iTimeMinTemp, iTimeMinTemp2, NDFRACTION, HourType.NDHour);
                            #endregion

                            //Break between shifts
                            iTimeMinTemp = (LeaveStartMin > iShiftTimeOut1Min) ? LeaveStartMin : iShiftTimeOut1Min;
                            iTimeMinTemp2 = (LeaveEndMin < iShiftTimeIn2Min) ? LeaveEndMin : iShiftTimeIn2Min;
                            iCompRegNightPremMin += GetOTHoursInMinutes(NP1_BEGTIME, NP1_ENDTIME, iTimeMinTemp, iTimeMinTemp2);
                            #region Insert ND Hour to Hour Fraction Table
                            InsertNDToHourFractionTable(NP1_BEGTIME, NP1_ENDTIME, iTimeMinTemp, iTimeMinTemp2, NDFRACTION, HourType.NDHour);
                            #endregion
                            #endregion

                            #region ND Bracket 2
                            //[NDHr = Get ND between iShiftTimeIn1Min to iShiftTimeOut1Min]
                            iTimeMinTemp = (LeaveStartMin > iShiftTimeIn1Min) ? LeaveStartMin : iShiftTimeIn1Min;
                            iTimeMinTemp2 = (LeaveEndMin < iShiftTimeOut1Min) ? LeaveEndMin : iShiftTimeOut1Min;
                            iCompOvertimeNightPremMin += GetOTHoursInMinutes(NP2_BEGTIME, NP2_ENDTIME, iTimeMinTemp, iTimeMinTemp2);
                            #region Insert ND Hour to Hour Fraction Table
                            InsertNDToHourFractionTable(NP2_BEGTIME, NP2_ENDTIME, iTimeMinTemp, iTimeMinTemp2, NDFRACTION, HourType.NDOTHour);
                            #endregion

                            //[NDHr = Get ND between iShiftTimeIn2Min to iShiftTimeOut2Min]
                            iTimeMinTemp = (LeaveStartMin > iShiftTimeIn2Min) ? LeaveStartMin : iShiftTimeIn2Min;
                            iTimeMinTemp2 = (LeaveEndMin < iShiftTimeOut2Min) ? LeaveEndMin : iShiftTimeOut2Min;
                            iCompOvertimeNightPremMin += GetOTHoursInMinutes(NP2_BEGTIME, NP2_ENDTIME, iTimeMinTemp, iTimeMinTemp2);
                            #region Insert ND Hour to Hour Fraction Table
                            InsertNDToHourFractionTable(NP2_BEGTIME, NP2_ENDTIME, iTimeMinTemp, iTimeMinTemp2, NDFRACTION, HourType.NDOTHour);
                            #endregion

                            //Break between shifts
                            iTimeMinTemp = (LeaveStartMin > iShiftTimeOut1Min) ? LeaveStartMin : iShiftTimeOut1Min;
                            iTimeMinTemp2 = (LeaveEndMin < iShiftTimeIn2Min) ? LeaveEndMin : iShiftTimeIn2Min;
                            iCompOvertimeNightPremMin += GetOTHoursInMinutes(NP2_BEGTIME, NP2_ENDTIME, iTimeMinTemp, iTimeMinTemp2);
                            #region Insert ND Hour to Hour Fraction Table
                            InsertNDToHourFractionTable(NP2_BEGTIME, NP2_ENDTIME, iTimeMinTemp, iTimeMinTemp2, NDFRACTION, HourType.NDOTHour);
                            #endregion
                            #endregion
                        }
                        #endregion

                        #region NDFraction Filter (Sharp)
                        iCompRegNightPremMin = Convert.ToInt32((iCompRegNightPremMin / iNDFraction)) * iNDFraction;
                        iCompOvertimeNightPremMin = Convert.ToInt32((iCompOvertimeNightPremMin / iNDFraction)) * iNDFraction;
                        #endregion
                    }
                    else
                    {
                        #region Computed Regular Night Premium for Regular Day
                        if (!bRestDay) //Regular day
                        {
                            //COMPUTED REGULAR NIGHT PREMIUM MIN
                            if (!Convert.ToBoolean(NONDOTDAY)) //HOYA
                            {
                                //[NDHr = Get ND between iShiftTimeIn1Min to iShiftTimeOut1Min]
                                iTimeMinTemp = (LeaveStartMin > iShiftTimeIn1Min) ? LeaveStartMin : iShiftTimeIn1Min;
                                iTimeMinTemp2 = (LeaveEndMin < iShiftTimeOut1Min) ? LeaveEndMin : iShiftTimeOut1Min;
                                iCompRegNightPremMin += GetOTHoursInMinutes(NIGHTDIFFAMSTART, NIGHTDIFFAMEND, iTimeMinTemp, iTimeMinTemp2);
                                #region Insert ND Hour to Hour Fraction Table
                                InsertNDToHourFractionTable(NIGHTDIFFAMSTART, NIGHTDIFFAMEND, iTimeMinTemp, iTimeMinTemp2, NDFRACTION, HourType.NDHour);
                                #endregion

                                //[NDHr = Get ND between iShiftTimeIn2Min to iShiftTimeOut2Min]
                                iTimeMinTemp = (LeaveStartMin > iShiftTimeIn2Min) ? LeaveStartMin : iShiftTimeIn2Min;
                                iTimeMinTemp2 = (LeaveEndMin < iShiftTimeOut2Min) ? LeaveEndMin : iShiftTimeOut2Min;
                                iCompRegNightPremMin += GetOTHoursInMinutes(NIGHTDIFFAMSTART, NIGHTDIFFAMEND, iTimeMinTemp, iTimeMinTemp2);
                                #region Insert ND Hour to Hour Fraction Table
                                InsertNDToHourFractionTable(NIGHTDIFFAMSTART, NIGHTDIFFAMEND, iTimeMinTemp, iTimeMinTemp2, NDFRACTION, HourType.NDHour);
                                #endregion
                            }

                            //[NDHr = Get ND between iShiftTimeIn1Min to iShiftTimeOut1Min]
                            iTimeMinTemp = (LeaveStartMin > iShiftTimeIn1Min) ? LeaveStartMin : iShiftTimeIn1Min;
                            iTimeMinTemp2 = (LeaveEndMin < iShiftTimeOut1Min) ? LeaveEndMin : iShiftTimeOut1Min;
                            iCompRegNightPremMin += GetOTHoursInMinutes(NIGHTDIFFGRAVEYARDSTART, NIGHTDIFFGRAVEYARDEND, iTimeMinTemp, iTimeMinTemp2);
                            #region Insert ND Hour to Hour Fraction Table
                            InsertNDToHourFractionTable(NIGHTDIFFGRAVEYARDSTART, NIGHTDIFFGRAVEYARDEND, iTimeMinTemp, iTimeMinTemp2, NDFRACTION, HourType.NDHour);
                            #endregion

                            //[NDHr = Get ND between iShiftTimeIn2Min to iShiftTimeOut2Min]
                            iTimeMinTemp = (LeaveStartMin > iShiftTimeIn2Min) ? LeaveStartMin : iShiftTimeIn2Min;
                            iTimeMinTemp2 = (LeaveEndMin < iShiftTimeOut2Min) ? LeaveEndMin : iShiftTimeOut2Min;
                            iCompRegNightPremMin += GetOTHoursInMinutes(NIGHTDIFFGRAVEYARDSTART, NIGHTDIFFGRAVEYARDEND, iTimeMinTemp, iTimeMinTemp2);
                            #region Insert ND Hour to Hour Fraction Table
                            InsertNDToHourFractionTable(NIGHTDIFFGRAVEYARDSTART, NIGHTDIFFGRAVEYARDEND, iTimeMinTemp, iTimeMinTemp2, NDFRACTION, HourType.NDHour);
                            #endregion

                            //Break between shifts
                            iTimeMinTemp = (LeaveStartMin > iShiftTimeOut1Min) ? LeaveStartMin : iShiftTimeOut1Min;
                            iTimeMinTemp2 = (LeaveEndMin < iShiftTimeIn2Min) ? LeaveEndMin : iShiftTimeIn2Min;
                            if (!Convert.ToBoolean(NONDOTDAY)) //HOYA
                            {
                                iCompRegNightPremMin += GetOTHoursInMinutes(NIGHTDIFFAMSTART, NIGHTDIFFAMEND, iTimeMinTemp, iTimeMinTemp2);
                                #region Insert ND Hour to Hour Fraction Table
                                InsertNDToHourFractionTable(NIGHTDIFFAMSTART, NIGHTDIFFAMEND, iTimeMinTemp, iTimeMinTemp2, NDFRACTION, HourType.NDHour);
                                #endregion
                            }
                            iCompRegNightPremMin += GetOTHoursInMinutes(NIGHTDIFFGRAVEYARDSTART, NIGHTDIFFGRAVEYARDEND, iTimeMinTemp, iTimeMinTemp2);
                            #region Insert ND Hour to Hour Fraction Table
                            InsertNDToHourFractionTable(NIGHTDIFFGRAVEYARDSTART, NIGHTDIFFGRAVEYARDEND, iTimeMinTemp, iTimeMinTemp2, NDFRACTION, HourType.NDHour);
                            #endregion
                        }
                        #endregion

                        #region NDFraction Filter
                        iCompRegNightPremMin = Convert.ToInt32((iCompRegNightPremMin / iNDFraction)) * iNDFraction;
                        iCompOvertimeNightPremMin = Convert.ToInt32((iCompOvertimeNightPremMin / iNDFraction)) * iNDFraction;
                        ////HOYA
                        if (strDayCode.Equals("REG5") || (Convert.ToBoolean(NDPREM1ST8))) //&& bRestDay)) //Commented by Rendell Uy (10/6/2015)
                        {
                            iNDSum = iCompRegNightPremMin + iCompOvertimeNightPremMin;
                            if (iNDSum > 8 * 60)
                            {
                                iCompRegNightPremMin = 8 * 60; //ND hours is set to 8 hours
                                iCompOvertimeNightPremMin = iNDSum - iCompRegNightPremMin; //excess 8 hours is set to NDOT hours
                            }
                            else
                            {
                                iCompRegNightPremMin = iNDSum; //all ND and NDOT hours to ND premium
                                iCompOvertimeNightPremMin = 0; //no NDOT hours
                            }
                        }
                        #endregion
                    }
                }
            }
        }

        public DataRow[] GetCorrectedLeaveRecords(string EmployeeId, string ProcessDate, bool PaidLeave, bool bIsGraveyard, int iShiftTimeIn1Min, int iShiftTimeOut2Min)
        {
            DataRow[] drArrLeaveApp;
            int iStartLeave1, iEndLeave1;
            int iStartLeave2, iEndLeave2;
            int iStartLeaveMin, iEndLeaveMax;

            if (PaidLeave == true)
                drArrLeaveApp = dtLeaveTable.Select("EmployeeId = '" + EmployeeId + "' AND LeaveDate = '" + ProcessDate + "'"
                                                            , "LeaveType ASC, LeaveHours ASC, StartTime ASC, EndTime ASC");
            else
                drArrLeaveApp = dtUnpaidLeaveTable.Select("EmployeeId = '" + EmployeeId + "' AND LeaveDate = '" + ProcessDate + "'"
                                                            , "LeaveType ASC, LeaveHours ASC, StartTime ASC, EndTime ASC");

            if (drArrLeaveApp.Length > 1)
            {
                for (int i = 0; i < drArrLeaveApp.Length; i++)
                {
                    iStartLeave1 = GetMinsFromHourStr(drArrLeaveApp[i]["StartTime"].ToString());
                    iEndLeave1 = GetMinsFromHourStr(drArrLeaveApp[i]["EndTime"].ToString());

                    if (iStartLeave1 != 0 && iEndLeave1 != 0 && bIsGraveyard) //Graveyard shift 
                    {
                        if (iStartLeave1 < (iShiftTimeIn1Min - LOGPAD))
                        {
                            iStartLeave1 += GRAVEYARD24;
                        }
                        if (iEndLeave1 < (iShiftTimeOut2Min - LOGPAD))
                        {
                            iEndLeave1 += GRAVEYARD24;
                        }
                    }

                    for (int j = 0; j < drArrLeaveApp.Length; j++)
                    {
                        if (i != j)
                        {
                            iStartLeave2 = GetMinsFromHourStr(drArrLeaveApp[j]["StartTime"].ToString());
                            iEndLeave2 = GetMinsFromHourStr(drArrLeaveApp[j]["EndTime"].ToString());

                            if (iStartLeave2 != 0 && iEndLeave2 != 0 && iStartLeave1 != 0 && iEndLeave1 != 0
                                && bIsGraveyard) //Graveyard shift
                            {
                                if (iStartLeave2 < (iShiftTimeIn1Min - LOGPAD))
                                {
                                    iStartLeave2 += GRAVEYARD24;
                                }
                                if (iEndLeave2 < (iShiftTimeOut2Min - LOGPAD))
                                {
                                    iEndLeave2 += GRAVEYARD24;
                                }
                            }

                            if (iStartLeave1 != 0 && iStartLeave2 != 0)
                                iStartLeaveMin = Math.Min(iStartLeave1, iStartLeave2);
                            else
                                iStartLeaveMin = iStartLeave1;
                            if (iEndLeave1 != 0 && iEndLeave2 != 0)
                                iEndLeaveMax = Math.Max(iEndLeave1, iEndLeave2);
                            else
                                iEndLeaveMax = iEndLeave1;

                            //remove cancelled leave
                            if (i < drArrLeaveApp.Length
                                    && Convert.ToDouble(drArrLeaveApp[i]["LeaveHours"]) != 0 && Convert.ToDouble(drArrLeaveApp[j]["LeaveHours"]) != 0
                                    && drArrLeaveApp[i]["LeaveType"].ToString().Equals(drArrLeaveApp[j]["LeaveType"].ToString())
                                    && drArrLeaveApp[i]["StartTime"].ToString().Equals(drArrLeaveApp[j]["StartTime"].ToString())
                                    && drArrLeaveApp[i]["EndTime"].ToString().Equals(drArrLeaveApp[j]["EndTime"].ToString())
                                    && Convert.ToDouble(drArrLeaveApp[i]["LeaveHours"]) + Convert.ToDouble(drArrLeaveApp[j]["LeaveHours"]) == 0)
                            {
                                drArrLeaveApp[i]["StartTime"] = "0000";
                                drArrLeaveApp[i]["EndTime"] = "0000";
                                drArrLeaveApp[i]["LeaveHours"] = 0;
                                drArrLeaveApp[j]["StartTime"] = "0000";
                                drArrLeaveApp[j]["EndTime"] = "0000";
                                drArrLeaveApp[j]["LeaveHours"] = 0;
                                break;
                            }
                            //remove duplicate
                            if (i < drArrLeaveApp.Length
                                    && Convert.ToDouble(drArrLeaveApp[i]["LeaveHours"]) != 0 && Convert.ToDouble(drArrLeaveApp[j]["LeaveHours"]) != 0
                                    && drArrLeaveApp[i]["LeaveType"].ToString().Equals(drArrLeaveApp[j]["LeaveType"].ToString())
                                    && drArrLeaveApp[i]["StartTime"].ToString().Equals(drArrLeaveApp[j]["StartTime"].ToString())
                                    && drArrLeaveApp[i]["EndTime"].ToString().Equals(drArrLeaveApp[j]["EndTime"].ToString())
                                    && Convert.ToDouble(drArrLeaveApp[i]["LeaveHours"]) == Convert.ToDouble(drArrLeaveApp[j]["LeaveHours"]))
                            {
                                drArrLeaveApp[j]["StartTime"] = "0000";
                                drArrLeaveApp[j]["EndTime"] = "0000";
                                drArrLeaveApp[j]["LeaveHours"] = 0;
                                break;
                            }
                            //update overlapping records
                            if (i < drArrLeaveApp.Length
                                    && Convert.ToDouble(drArrLeaveApp[i]["LeaveHours"]) > 0 && Convert.ToDouble(drArrLeaveApp[j]["LeaveHours"]) > 0 //not cancelled
                                    && (iEndLeave1 >= iStartLeave2 && iEndLeave2 >= iStartLeave1)
                                    && drArrLeaveApp[i]["LeaveType"].ToString().Equals(drArrLeaveApp[j]["LeaveType"].ToString())) //overlap
                            {
                                drArrLeaveApp[i]["StartTime"] = GetHourStrFromMins(iStartLeaveMin);
                                drArrLeaveApp[i]["EndTime"] = GetHourStrFromMins(iEndLeaveMax);
                                drArrLeaveApp[i]["LeaveHours"] = Convert.ToDouble(drArrLeaveApp[i]["LeaveHours"]) + Convert.ToDouble(drArrLeaveApp[j]["LeaveHours"]);
                                drArrLeaveApp[j]["StartTime"] = "0000";
                                drArrLeaveApp[j]["EndTime"] = "0000";
                                drArrLeaveApp[j]["LeaveHours"] = 0;
                                iStartLeave1 = iStartLeaveMin;
                                iEndLeave1 = iEndLeaveMax;
                            }
                        }
                    }
                }
            }

            return drArrLeaveApp;
        }

        public void AdjustLeaveBasedOnShift(DataRow[] drArrLeaveApp, string EmployeeId, string ProcessDate, int iShiftTimeIn1Min, int iShiftTimeOut1Min, int iShiftTimeIn2Min, int iShiftTimeOut2Min)
        {
            int iLeaveStartMin;
            int iLeaveEndMin;
            for (int iLve = 0; iLve < drArrLeaveApp.Length; iLve++)
            {
                iLeaveStartMin = GetMinsFromHourStr(drArrLeaveApp[iLve]["StartTime"].ToString());
                iLeaveEndMin = GetMinsFromHourStr(drArrLeaveApp[iLve]["EndTime"].ToString());
                if (iLeaveStartMin != 0 && iLeaveEndMin != 0)
                {
                    if (drArrLeaveApp[iLve]["DayUnit"].ToString().Equals("WH") && !drArrLeaveApp[iLve]["LeaveType"].ToString().Equals("OB"))
                    {
                        drArrLeaveApp[iLve]["StartTime"] = GetHourStrFromMins(iShiftTimeIn1Min);
                        drArrLeaveApp[iLve]["EndTime"] = GetHourStrFromMins(iShiftTimeOut2Min);
                    }
                }
            }
        }

        public void CorrectConvertedTimeWithExtRegLveFlag(DataRow[] drArrLeaveApp
                                                            , ref int iConvTimeIn1Min, ref int iConvTimeOut1Min, ref int iConvTimeIn2Min, ref int iConvTimeOut2Min
                                                            , int iShiftTimeIn1Min, int iShiftTimeOut1Min, int iShiftTimeIn2Min, int iShiftTimeOut2Min
                                                            , int iShiftMin, DataRow[] drArrOTApproved, bool bIsGraveyard)
        {
            int iLeaveStartMin;
            int iLeaveEndMin;
            int iTimeTemp;
            for (int iLve = 0; iLve < drArrLeaveApp.Length; iLve++)
            {
                #region Leave Initialization
                iLeaveStartMin = GetMinsFromHourStr(drArrLeaveApp[iLve]["StartTime"].ToString());
                iLeaveEndMin = GetMinsFromHourStr(drArrLeaveApp[iLve]["EndTime"].ToString());
                if (bIsGraveyard == true)
                {
                    if (iLeaveStartMin < (iShiftTimeIn1Min - LOGPAD))
                    {
                        iLeaveStartMin += GRAVEYARD24;
                    }
                    if (iLeaveEndMin < (iShiftTimeOut2Min - LOGPAD))
                    {
                        iLeaveEndMin += GRAVEYARD24;
                    }
                }
                #endregion

                if (iLeaveStartMin != 0 && iLeaveEndMin != 0 && drArrLeaveApp[iLve]["LeaveType"].ToString() != "OB")
                {
                    if (drArrLeaveApp[iLve]["DayUnit"].ToString().Equals("WH")
                        || Convert.ToDouble(drArrLeaveApp[iLve]["LeaveHours"]) == (double)(iShiftMin / 60.0))
                    {
                        #region Whole Day
                        //Allow OT during leave
                        if (iConvTimeIn1Min > 0 && iConvTimeIn1Min < iShiftTimeIn1Min && drArrOTApproved.Length > 0
                            && drArrOTApproved[0]["Eot_OvertimeType"].ToString() == "A") //early OT
                        {
                            //iConvTimeOut1Min = iShiftTimeIn1Min;
                        }
                        else
                        {
                            if (iConvTimeOut2Min > 0 && iConvTimeOut2Min > iShiftTimeOut2Min && drArrOTApproved.Length > 0
                                && drArrOTApproved[drArrOTApproved.Length - 1]["Eot_OvertimeType"].ToString() == "P") //post OT
                            {
                                //iConvTimeIn2Min = iShiftTimeOut2Min;
                            }
                            else
                            {
                                iConvTimeIn1Min = 0;
                                iConvTimeOut1Min = 0;
                                iConvTimeIn2Min = 0;
                                iConvTimeOut2Min = 0;
                            }
                        }
                        #endregion
                    }
                    else if (drArrLeaveApp[iLve]["DayUnit"].ToString().Equals("HA"))
                    {
                        #region Half Day AM
                        //Allow OT during leave
                        if (iConvTimeIn1Min > 0 && iConvTimeIn1Min < iShiftTimeIn1Min && drArrOTApproved.Length > 0
                            && drArrOTApproved[0]["Eot_OvertimeType"].ToString() == "A") //early OT
                        {
                            //iConvTimeOut1Min = iShiftTimeIn1Min;
                            //if (iConvTimeOut2Min > 0 && iConvTimeIn2Min == 0)
                            //    iConvTimeIn2Min = iShiftTimeIn2Min;
                        }
                        else
                        {
                            if (Math.Max(iConvTimeIn1Min, iConvTimeIn2Min) < iShiftTimeIn2Min)
                            {
                                iConvTimeIn1Min = Math.Max(Math.Max(iConvTimeIn1Min, iConvTimeIn2Min), iShiftTimeOut1Min); //unpaid break is included for MIDOT purposes
                                iConvTimeOut1Min = 0;
                                iConvTimeIn2Min = 0;
                            }
                            else
                            {
                                iConvTimeIn2Min = Math.Max(iConvTimeIn1Min, iConvTimeIn2Min);
                                iConvTimeIn1Min = 0;
                                iConvTimeOut1Min = 0;
                            }
                        }
                        #endregion
                    }
                    else if (drArrLeaveApp[iLve]["DayUnit"].ToString().Equals("HP"))
                    {
                        #region Half Day PM
                        //Allow OT during leave
                        if (iConvTimeOut2Min > 0 && iConvTimeOut2Min > iShiftTimeOut2Min && drArrOTApproved.Length > 0
                            && drArrOTApproved[drArrOTApproved.Length - 1]["Eot_OvertimeType"].ToString() == "P") //post OT
                        {
                            //iConvTimeIn2Min = iShiftTimeOut2Min;
                            //if (iConvTimeIn1Min > 0 && iConvTimeOut1Min == 0)
                            //    iConvTimeOut1Min = iShiftTimeOut1Min;
                        }
                        else
                        {
                            iConvTimeOut2Min = Math.Min(Math.Max(iConvTimeOut1Min, iConvTimeOut2Min), iShiftTimeIn2Min); //unpaid break is included for MIDOT purposes
                            iConvTimeOut1Min = 0;
                            iConvTimeIn2Min = 0;
                        }
                        #endregion
                    }
                    else if (Convert.ToBoolean(FLEXSHIFT) == false) //Logic is only applicable if Not Flex Shift
                    {
                        #region Hourly
                        //Leave starts at beginning of shift and ends before/after breaktime
                        if (iLeaveStartMin == iShiftTimeIn1Min)
                        {
                            if (((iConvTimeIn1Min > 0) ? iConvTimeIn1Min : iConvTimeIn2Min) > 0)
                            {
                                iTimeTemp = Math.Max(iLeaveEndMin, ((iConvTimeIn1Min > 0) ? iConvTimeIn1Min : iConvTimeIn2Min));
                                if (iTimeTemp < iShiftTimeOut1Min)
                                {
                                    iConvTimeIn1Min = iTimeTemp;
                                    iConvTimeIn2Min = 0;
                                }
                                else
                                {
                                    iConvTimeIn1Min = 0;
                                    iConvTimeIn2Min = iTimeTemp;
                                }
                            }
                        }
                        //Leave starts before/after breaktime towards end of shift
                        else if (iLeaveEndMin == iShiftTimeOut2Min)
                        {
                            if (((iConvTimeOut2Min > 0) ? iConvTimeOut2Min : iConvTimeOut1Min) > 0)
                            {
                                iTimeTemp = Math.Min(iLeaveStartMin, ((iConvTimeOut2Min > 0) ? iConvTimeOut2Min : iConvTimeOut1Min));
                                if (iTimeTemp <= iShiftTimeOut1Min)
                                {
                                    iConvTimeOut1Min = iTimeTemp;
                                    iConvTimeOut2Min = 0;
                                }
                                else
                                {
                                    iConvTimeOut1Min = 0;
                                    iConvTimeOut2Min = iTimeTemp;
                                    if (iConvTimeIn1Min > 0 && iConvTimeIn2Min > 0)
                                        iConvTimeIn2Min = 0;
                                }
                            }
                        }
                        //Leave starts at middle of the shift and ends before/after breaktime
                        else if (iLeaveStartMin > iShiftTimeIn1Min && iLeaveEndMin < iShiftTimeOut2Min)
                        {
                            //IN
                            if (((iConvTimeIn1Min > 0) ? iConvTimeIn1Min : iConvTimeIn2Min) > 0)
                            {
                                iTimeTemp = Math.Max(iLeaveEndMin, ((iConvTimeIn2Min > 0) ? iConvTimeIn2Min : iConvTimeIn1Min));
                                if (iTimeTemp < iShiftTimeOut1Min)
                                    iConvTimeIn1Min = iTimeTemp;
                                else
                                    iConvTimeIn2Min = iTimeTemp;
                            }
                            //OUT
                            if (((iConvTimeOut2Min > 0) ? iConvTimeOut2Min : iConvTimeOut1Min) > 0)
                            {
                                iTimeTemp = Math.Min(iLeaveStartMin, ((iConvTimeOut1Min > 0) ? iConvTimeOut1Min : iConvTimeOut2Min));
                                if (iTimeTemp <= iShiftTimeOut1Min && iTimeTemp > iConvTimeIn1Min)
                                {
                                    iConvTimeOut1Min = iTimeTemp;
                                }
                                else if (iTimeTemp > iShiftTimeOut1Min && iTimeTemp > ((iConvTimeIn2Min > 0) ? iConvTimeIn2Min : iConvTimeIn1Min))
                                {
                                    iConvTimeOut2Min = iTimeTemp;
                                    if (iConvTimeIn1Min > 0 && iConvTimeIn2Min > 0 && iConvTimeIn2Min > iShiftTimeIn2Min)
                                        iConvTimeOut1Min = iShiftTimeOut1Min;
                                }
                            }
                        }
                        #endregion
                    }
                }
            }
        }

        public bool HasMetRegularHourRequirement(string JobStatus, double RegularHours, double LeaveHours, string DayCode)
        {
            bool bHasMet = false;
            if (REGHRSREQD != null)
            {
                foreach (DataRow drRow in REGHRSREQD.Rows)
                {
                    if ((DayCode == "REG" || DayCode == "REGN")
                        && drRow["Pmx_Classification"].ToString() == JobStatus
                        && drRow["Pmx_ParameterValue"].ToString() != "-1"
                        && RegularHours + LeaveHours >= Convert.ToDouble(drRow["Pmx_ParameterValue"]))
                        bHasMet = true;
                }
            }
            return bHasMet;
        }

        public bool GetApplicableHrsFromCommaDelimitedTable(DataTable dtTable, string JobLevel, string JobStatus, string EmploymentStatus, string PayrollType, string Position, double dMinOT, ref double dValue)
        {
            string ParamJobLevel, ParamJobStatus, ParamEmploymentStatus, ParamPayrollType, ParamPosition, temp;
            string[] strQualifiers;
            bool bFound = false;
            int iIndex;

            dValue = dMinOT; //initialize
            if (dtTable != null)
            {
                foreach (DataRow drRow in dtTable.Rows)
                {
                    iIndex = 0;
                    ParamJobLevel = "";
                    ParamJobStatus = "";
                    ParamEmploymentStatus = "";
                    ParamPayrollType = "";
                    ParamPosition = "";

                    strQualifiers = drRow["Pmx_Classification"].ToString().Split(new char[] { ',' });
                    foreach (string col in strQualifiers)
                    {
                        temp = col.Trim();
                        switch (iIndex)
                        {
                            case 0: ParamJobLevel = temp;
                                break;
                            case 1: ParamJobStatus = temp;
                                break;
                            case 2: ParamEmploymentStatus = temp;
                                break;
                            case 3: ParamPayrollType = temp;
                                break;
                            case 4: ParamPosition = temp;
                                break;
                        }
                        iIndex++;
                    }

                    if ((ParamJobLevel == "" || ParamJobLevel == JobLevel)
                        && (ParamJobStatus == "" || ParamJobStatus == JobStatus)
                        && (ParamEmploymentStatus == "" || ParamEmploymentStatus == EmploymentStatus)
                        && (ParamPayrollType == "" || ParamPayrollType == PayrollType)
                        && (ParamPosition == "" || ParamPosition == Position))
                    {
                        dValue = Convert.ToDouble(drRow["Pmx_ParameterValue"]);
                        bFound = true;
                        break;
                    }
                }
            }
            return bFound;
        }

        public bool CheckIfExistsInCommaDelString(string lookupString, string sourceString, char delimiter)
        {
            bool bFound = false;
            string[] strArray = sourceString.Split(new char[] { delimiter });
            foreach (string value in strArray)
            {
                if (value.Trim() == lookupString)
                    bFound = true;
            }
            return bFound;
        }
        #endregion

        #region Labor Hour Report functions
        public void AddErrorToLaborHourReport(string strEmployeeId, string strLastName, string strFirstName, string strMiddleName, string strProcessDate, string strRemarks)
        {
            listLbrHrRept.Add(new structLaborHourErrorReport(strEmployeeId, strLastName, strFirstName, strMiddleName, strProcessDate, strRemarks));
        }

        public void InitializeLaborHourReport()
        {
            listLbrHrRept.Clear();
        }

        private DataTable SaveLaborHourErrorReportList(DataTable dtErrList)
        {
            if (dtErrList.Columns.Count == 0)
            {
                dtErrList.Columns.Add("IDNumber");
                dtErrList.Columns.Add("Last Name");
                dtErrList.Columns.Add("First Name");
                dtErrList.Columns.Add("Middle Name");
                dtErrList.Columns.Add("Date");
                dtErrList.Columns.Add("Remarks");
            }
            for (int i = 0; i < listLbrHrRept.Count; i++)
            {
                dtErrList.Rows.Add();
                dtErrList.Rows[dtErrList.Rows.Count - 1]["IDNumber"] = listLbrHrRept[i].strEmployeeId;
                dtErrList.Rows[dtErrList.Rows.Count - 1]["Last Name"] = listLbrHrRept[i].strLastName;
                dtErrList.Rows[dtErrList.Rows.Count - 1]["First Name"] = listLbrHrRept[i].strFirstName;
                dtErrList.Rows[dtErrList.Rows.Count - 1]["Middle Name"] = listLbrHrRept[i].strMiddleName;
                dtErrList.Rows[dtErrList.Rows.Count - 1]["Date"] = Convert.ToDateTime(listLbrHrRept[i].strProcessDate).ToShortDateString();
                dtErrList.Rows[dtErrList.Rows.Count - 1]["Remarks"] = listLbrHrRept[i].strRemarks;
            }

            return dtErrList;
        }

        public DataSet GetLaborHoursNegative(bool ProcessAll, string EmployeeId)
        {
            #region query
            string qString = @"	SELECT  Ell_EmployeeId
                                        ,Emt_LastName
                                        ,Emt_FirstName
                                        ,Emt_MiddleName
	                                    ,Ell_ProcessDate
                                        ,Ell_AbsentHour
                                        ,Ell_RegularHour
                                        ,Ell_OvertimeHour
                                        ,Ell_RegularNightPremHour
                                        ,Ell_OvertimeNightPremHour
                                        ,Ell_LeaveHour
	                            FROM {0} 
                                INNER JOIN T_EmployeeMaster 
                                ON Emt_EmployeeId = Ell_EmployeeId
                                WHERE (Ell_AbsentHour < 0 or
		                                Ell_RegularHour  < 0 or
		                                Ell_OvertimeHour  < 0 or
		                                Ell_RegularNightPremHour < 0 or
		                                Ell_OvertimeNightPremHour  < 0 or
		                                Ell_LeaveHour < 0 )
                                    And Ell_PayPeriod = '{1}'";
            #endregion
            qString = string.Format(qString, EmployeeLogLedgerTable, ProcessPayrollPeriod);
            if (!ProcessAll && EmployeeId != "")
                qString += " And Ell_EmployeeID = '" + EmployeeId + "'";
            else if (ProcessAll == true && EmployeeList != "")
                qString += " And Ell_EmployeeID IN (" + EmployeeList + ")";
            DataSet dsResult = dal.ExecuteDataSet(qString);
            return dsResult;
        }

        private DataTable CreateErrorListForPayTrans(DataTable dtErrList, DataSet tempds)
        {
            if (dtErrList.Columns.Count == 0)
            {
                dtErrList.Columns.Add("IDNumber");
                dtErrList.Columns.Add("Last Name");
                dtErrList.Columns.Add("First Name");
                dtErrList.Columns.Add("Middle Name");
                dtErrList.Columns.Add("Date");
                dtErrList.Columns.Add("Remarks");
            }
            for (int i = 0; i < tempds.Tables[0].Rows.Count; i++)
            {
                #region <Add Error List>
                if (Convert.ToDecimal(tempds.Tables[0].Rows[i]["Ell_AbsentHour"].ToString().Trim()) < 0)
                {
                    getIntValue("1.0");
                    dtErrList.Rows.Add();
                    dtErrList.Rows[dtErrList.Rows.Count - 1]["IDNumber"] = tempds.Tables[0].Rows[i]["Ell_EmployeeId"].ToString();
                    dtErrList.Rows[dtErrList.Rows.Count - 1]["Last Name"] = tempds.Tables[0].Rows[i]["Emt_LastName"].ToString();
                    dtErrList.Rows[dtErrList.Rows.Count - 1]["First Name"] = tempds.Tables[0].Rows[i]["Emt_FirstName"].ToString();
                    dtErrList.Rows[dtErrList.Rows.Count - 1]["Middle Name"] = tempds.Tables[0].Rows[i]["Emt_MiddleName"].ToString();
                    dtErrList.Rows[dtErrList.Rows.Count - 1]["Date"] = Convert.ToDateTime(tempds.Tables[0].Rows[i]["Ell_ProcessDate"].ToString()).ToShortDateString();
                    dtErrList.Rows[dtErrList.Rows.Count - 1]["Remarks"] = "Negative Absent Hour";
                }
                if (Convert.ToDecimal(tempds.Tables[0].Rows[i]["Ell_RegularHour"].ToString().Trim()) < 0)
                {
                    dtErrList.Rows.Add();
                    dtErrList.Rows[dtErrList.Rows.Count - 1]["IDNumber"] = tempds.Tables[0].Rows[i]["Ell_EmployeeId"].ToString();
                    dtErrList.Rows[dtErrList.Rows.Count - 1]["Last Name"] = tempds.Tables[0].Rows[i]["Emt_LastName"].ToString();
                    dtErrList.Rows[dtErrList.Rows.Count - 1]["First Name"] = tempds.Tables[0].Rows[i]["Emt_FirstName"].ToString();
                    dtErrList.Rows[dtErrList.Rows.Count - 1]["Middle Name"] = tempds.Tables[0].Rows[i]["Emt_MiddleName"].ToString();
                    dtErrList.Rows[dtErrList.Rows.Count - 1]["Date"] = Convert.ToDateTime(tempds.Tables[0].Rows[i]["Ell_ProcessDate"].ToString()).ToShortDateString();
                    dtErrList.Rows[dtErrList.Rows.Count - 1]["Remarks"] = "Negative Reg. Hour";
                }
                if (Convert.ToDecimal(tempds.Tables[0].Rows[i]["Ell_OvertimeHour"].ToString().Trim()) < 0)
                {
                    dtErrList.Rows.Add();
                    dtErrList.Rows[dtErrList.Rows.Count - 1]["IDNumber"] = tempds.Tables[0].Rows[i]["Ell_EmployeeId"].ToString();
                    dtErrList.Rows[dtErrList.Rows.Count - 1]["Last Name"] = tempds.Tables[0].Rows[i]["Emt_LastName"].ToString();
                    dtErrList.Rows[dtErrList.Rows.Count - 1]["First Name"] = tempds.Tables[0].Rows[i]["Emt_FirstName"].ToString();
                    dtErrList.Rows[dtErrList.Rows.Count - 1]["Middle Name"] = tempds.Tables[0].Rows[i]["Emt_MiddleName"].ToString();
                    dtErrList.Rows[dtErrList.Rows.Count - 1]["Date"] = Convert.ToDateTime(tempds.Tables[0].Rows[i]["Ell_ProcessDate"].ToString()).ToShortDateString();
                    dtErrList.Rows[dtErrList.Rows.Count - 1]["Remarks"] = "Negative OT Hour";
                }
                if (Convert.ToDecimal(tempds.Tables[0].Rows[i]["Ell_RegularNightPremHour"].ToString().Trim()) < 0)
                {
                    dtErrList.Rows.Add();
                    dtErrList.Rows[dtErrList.Rows.Count - 1]["IDNumber"] = tempds.Tables[0].Rows[i]["Ell_EmployeeId"].ToString();
                    dtErrList.Rows[dtErrList.Rows.Count - 1]["Last Name"] = tempds.Tables[0].Rows[i]["Emt_LastName"].ToString();
                    dtErrList.Rows[dtErrList.Rows.Count - 1]["First Name"] = tempds.Tables[0].Rows[i]["Emt_FirstName"].ToString();
                    dtErrList.Rows[dtErrList.Rows.Count - 1]["Middle Name"] = tempds.Tables[0].Rows[i]["Emt_MiddleName"].ToString();
                    dtErrList.Rows[dtErrList.Rows.Count - 1]["Date"] = Convert.ToDateTime(tempds.Tables[0].Rows[i]["Ell_ProcessDate"].ToString()).ToShortDateString();
                    dtErrList.Rows[dtErrList.Rows.Count - 1]["Remarks"] = "Negative Reg. Night Prem. Hr";
                }
                if (Convert.ToDecimal(tempds.Tables[0].Rows[i]["Ell_OvertimeNightPremHour"].ToString().Trim()) < 0)
                {
                    dtErrList.Rows.Add();
                    dtErrList.Rows[dtErrList.Rows.Count - 1]["IDNumber"] = tempds.Tables[0].Rows[i]["Ell_EmployeeId"].ToString();
                    dtErrList.Rows[dtErrList.Rows.Count - 1]["Last Name"] = tempds.Tables[0].Rows[i]["Emt_LastName"].ToString();
                    dtErrList.Rows[dtErrList.Rows.Count - 1]["First Name"] = tempds.Tables[0].Rows[i]["Emt_FirstName"].ToString();
                    dtErrList.Rows[dtErrList.Rows.Count - 1]["Middle Name"] = tempds.Tables[0].Rows[i]["Emt_MiddleName"].ToString();
                    dtErrList.Rows[dtErrList.Rows.Count - 1]["Date"] = Convert.ToDateTime(tempds.Tables[0].Rows[i]["Ell_ProcessDate"].ToString()).ToShortDateString();
                    dtErrList.Rows[dtErrList.Rows.Count - 1]["Remarks"] = "Negative OT Night Prem. Hr";
                }
                #endregion
            }

            return dtErrList;
        }

        public int InsertToLaborHrErr(DataTable dt)
        {
            int retVal = 0;
            string qString = "";

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                qString = string.Format(@"INSERT INTO T_LaborHourError 
                                                   (Lhe_EmployeeId
                                                   ,Lhe_CurrentPayperiod
                                                   ,Lhe_ProcessDate
                                                   ,Lhe_Remarks
                                                   ,usr_login
                                                   ,ludatetime)
                                             VALUES
                                                   ('{0}'
                                                   ,'{1}'
                                                   ,'{2}'
                                                   ,'{3}'
                                                   ,'{4}'
                                                   ,GetDate())", dt.Rows[i]["IDNumber"].ToString()
                                                               , ProcessPayrollPeriod
                                                               , dt.Rows[i]["Date"].ToString()
                                                               , dt.Rows[i]["Remarks"].ToString().Replace("'", "")
                                                               , LoginUser);
                try
                {
                    retVal = dal.ExecuteNonQuery(qString);
                }
                catch
                {
                    retVal = 0;
                }
            }
            return retVal;
        }
        #endregion

        #region Hour Fraction functions
        public void AddToHourFractionTable(int StartMin, int EndMin, int CurrentDayMin, int NextDayMin, HourType HourType)
        {
            listHourFract.Add(new structHourFract(StartMin, EndMin, CurrentDayMin, NextDayMin, HourType));
        }

        public void InitializeHourFractionTable()
        {
            listHourFract.Clear();
        }

        public void InsertRegularTimeToHourFractionTable(int StartTime, int EndTime)
        {
            if (Convert.ToBoolean(HRFRCLBRHR))
            {
                //Cutoff = 1440;

                CurrentDayMin = Math.Max(Math.Min(EndTime, Cutoff), 0) - Math.Max(Math.Min(StartTime, Cutoff), 0);
                NextDayMin = Math.Max(Math.Max(EndTime, Cutoff), 0) - Math.Max(Math.Max(StartTime, Cutoff), 0);

                if (CurrentDayMin > 0 || NextDayMin > 0)
                {
                    AddToHourFractionTable(StartTime, EndTime, Math.Max(CurrentDayMin, 0), Math.Max(NextDayMin, 0), HourType.RegHour);
                }
            }
        }

        public void InsertRegularTimeToHourFractionTable(int StartTime, int EndTime, int BaseTimeIn, int BaseTimeOut)
        {
            if (Convert.ToBoolean(HRFRCLBRHR))
            {
                //Cutoff = 1440;
                ActualStart = 0;
                ActualEnd = 0;

                OTMins = GetOTHoursInTime(StartTime, EndTime, BaseTimeIn, BaseTimeOut, ref ActualStart, ref ActualEnd);

                if (OTMins > 0)
                {
                    ActualStart = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(ActualStart)));
                    ActualEnd = Convert.ToInt32(Math.Floor(Convert.ToDouble(ActualEnd)));

                    CurrentDayMin = Math.Max(Math.Min(ActualEnd, Cutoff), 0) - Math.Max(Math.Min(ActualStart, Cutoff), 0);
                    NextDayMin = Math.Max(Math.Max(ActualEnd, Cutoff), 0) - Math.Max(Math.Max(ActualStart, Cutoff), 0);

                    if (CurrentDayMin > 0 || NextDayMin > 0)
                    {
                        AddToHourFractionTable(StartTime, EndTime, Math.Max(CurrentDayMin, 0), Math.Max(NextDayMin, 0), HourType.RegHour);
                    }
                }
            }
        }

        public void InsertLeaveTimeToHourFractionTable(int StartTime, int EndTime, int BaseTimeIn, int BaseTimeOut)
        {
            if (Convert.ToBoolean(HRFRCLBRHR))
            {
                //Cutoff = 1440;
                ActualStart = 0;
                ActualEnd = 0;
                ActualStart2 = 0;
                ActualEnd2 = 0;

                GetOTHoursExcludedTime(StartTime, EndTime, BaseTimeIn, BaseTimeOut, ref ActualStart, ref ActualEnd, ref ActualStart2, ref ActualEnd2);

                ActualStart = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(ActualStart)));
                ActualEnd = Convert.ToInt32(Math.Floor(Convert.ToDouble(ActualEnd)));

                ActualStart2 = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(ActualStart2)));
                ActualEnd2 = Convert.ToInt32(Math.Floor(Convert.ToDouble(ActualEnd2)));

                CurrentDayMin = Math.Max(Math.Min(ActualEnd, Cutoff), 0) - Math.Max(Math.Min(ActualStart, Cutoff), 0);
                NextDayMin = Math.Max(Math.Max(ActualEnd, Cutoff), 0) - Math.Max(Math.Max(ActualStart, Cutoff), 0);

                if (CurrentDayMin > 0 || NextDayMin > 0)
                {
                    AddToHourFractionTable(StartTime, EndTime, Math.Max(CurrentDayMin, 0), Math.Max(NextDayMin, 0), HourType.RegHour);
                }

                CurrentDayMin = Math.Max(Math.Min(ActualEnd2, Cutoff), 0) - Math.Max(Math.Min(ActualStart2, Cutoff), 0);
                NextDayMin = Math.Max(Math.Max(ActualEnd2, Cutoff), 0) - Math.Max(Math.Max(ActualStart2, Cutoff), 0);

                if (CurrentDayMin > 0 || NextDayMin > 0)
                {
                    AddToHourFractionTable(StartTime, EndTime, Math.Max(CurrentDayMin, 0), Math.Max(NextDayMin, 0), HourType.RegHour);
                }
            }
        }

        public void InsertOTToHourFractionTable(int StartTime, int EndTime, int BaseTimeIn, int BaseTimeOut, int OTFraction, bool CountOTFraction)
        {
            if (Convert.ToBoolean(HRFRCLBRHR))
            {
                //Cutoff = 1440;
                ActualStart = 0;
                ActualEnd = 0;

                OTMins = GetOTHoursInTime(StartTime, EndTime, BaseTimeIn, BaseTimeOut, ref ActualStart, ref ActualEnd);

                if (OTMins > 0)
                {
                    if (CountOTFraction)
                    {
                        ActualStart = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(ActualStart) / OTFraction)) * OTFraction;
                        ActualEnd = Convert.ToInt32(Math.Floor(Convert.ToDouble(ActualEnd) / OTFraction)) * OTFraction;
                    }
                    CurrentDayMin = Math.Max(Math.Min(ActualEnd, Cutoff), 0) - Math.Max(Math.Min(ActualStart, Cutoff), 0);
                    NextDayMin = Math.Max(Math.Max(ActualEnd, Cutoff), 0) - Math.Max(Math.Max(ActualStart, Cutoff), 0);

                    if (CurrentDayMin > 0 || NextDayMin > 0)
                    {
                        AddToHourFractionTable(StartTime, EndTime, Math.Max(CurrentDayMin, 0), Math.Max(NextDayMin, 0), HourType.OTHour);
                    }
                }
            }
        }

        public void CorrectOTHourFraction(int ActualComputedOT)
        {
            int iTotalCurrentDayMin = 0, iTotalNextDayMin = 0;
            int iLastIdx = -1;
            for (int i = 0; i < listHourFract.Count; i++)
            {
                if (listHourFract[i].strHourType == HourType.OTHour)
                {
                    if (listHourFract[i].iCurrentDayMin > 0)
                        iTotalCurrentDayMin += listHourFract[i].iCurrentDayMin;
                    if (listHourFract[i].iNextDayMin > 0)
                        iTotalNextDayMin += listHourFract[i].iNextDayMin;
                    iLastIdx = i;
                }
            }
            //Adjust OT minutes according to the computed OT minutes in Log Ledger
            if (iLastIdx != -1 && iTotalCurrentDayMin + iTotalNextDayMin != ActualComputedOT)
            {
                int iOffsetCurrentDayMin = 0, iOffsetNextDayMin = 0;
                if (iTotalNextDayMin > 0)
                    iOffsetNextDayMin = ActualComputedOT - (iTotalCurrentDayMin + iTotalNextDayMin);
                else
                    iOffsetCurrentDayMin = ActualComputedOT - (iTotalCurrentDayMin + iTotalNextDayMin);

                AddToHourFractionTable(0, 0, iOffsetCurrentDayMin, iOffsetNextDayMin, HourType.OTHour);
            }
        }

        public void InsertNDToHourFractionTable(int StartTime, int EndTime, int BaseTimeIn, int BaseTimeOut, double NDFraction, HourType hrType)
        {
            if (Convert.ToBoolean(HRFRCLBRHR))
            {
                //Cutoff = 1440;
                ActualStart = 0;
                ActualEnd = 0;

                OTMins = GetOTHoursInTime(StartTime, EndTime, BaseTimeIn, BaseTimeOut, ref ActualStart, ref ActualEnd);

                if (OTMins > 0)
                {
                    ActualStart = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(ActualStart) / NDFraction) * NDFraction);
                    ActualEnd = Convert.ToInt32(Math.Floor(Convert.ToDouble(ActualEnd) / NDFraction) * NDFraction);

                    CurrentDayMin = Math.Max(Math.Min(ActualEnd, Cutoff), 0) - Math.Max(Math.Min(ActualStart, Cutoff), 0);
                    NextDayMin = Math.Max(Math.Max(ActualEnd, Cutoff), 0) - Math.Max(Math.Max(ActualStart, Cutoff), 0);

                    if (CurrentDayMin > 0 || NextDayMin > 0)
                    {
                        AddToHourFractionTable(StartTime, EndTime, Math.Max(CurrentDayMin, 0), Math.Max(NextDayMin, 0), hrType);
                    }
                }
            }
        }

        public void SavePayrollTransactionAmounts(string EmployeeId, DateTime ProcessDate, string DayCode, bool Restday)
        {
            string NextDayCode = "";
            bool NextRestday = false;

            //Merge Records with Same Hour Type
            for (int i = 0; i < listHourFract.Count; i++)
            {
                for (int j = i + 1; j < listHourFract.Count; j++)
                {
                    if (listHourFract[i].strHourType == listHourFract[j].strHourType
                        && (listHourFract[i].iCurrentDayMin > 0 || listHourFract[i].iNextDayMin > 0))
                    {
                        listHourFract[i] = new structHourFract(listHourFract[i].iStartMin
                                                                , listHourFract[i].iEndMin
                                                                , listHourFract[i].iCurrentDayMin + listHourFract[j].iCurrentDayMin
                                                                , listHourFract[i].iNextDayMin + listHourFract[j].iNextDayMin
                                                                , listHourFract[i].strHourType);
                        listHourFract[j] = new structHourFract(listHourFract[j].iStartMin
                                                                , listHourFract[j].iEndMin
                                                                , 0
                                                                , 0
                                                                , listHourFract[j].strHourType);
                    }
                }
            }
            for (int i = 0; i < listHourFract.Count; i++)
            {
                if (listHourFract[i].iCurrentDayMin != 0)
                {
                    SavePayrollTransactionAmount(DayCode, Restday, listHourFract[i].iCurrentDayMin / 60.0, listHourFract[i].strHourType, "");
                }
                if (listHourFract[i].iNextDayMin != 0)
                {
                    GetNextDayDetails(EmployeeId, ProcessDate, ref NextDayCode, ref NextRestday);
                    SavePayrollTransactionAmount(NextDayCode, NextRestday, listHourFract[i].iNextDayMin / 60.0, listHourFract[i].strHourType, DayCode);
                }
            }
        }

        public void GetNextDayDetails(string EmployeeId, DateTime ProcessDate, ref string DayCode, ref bool Restday)
        {
            DataRow[] drArrNextDay = dtEmployeeLogLedger.Select(string.Format(" Ell_EmployeeId = '{0}' AND Ell_ProcessDate = '{1}'", EmployeeId, ProcessDate.AddDays(1)));

            if (drArrNextDay.Length > 0)
            {
                DayCode = drArrNextDay[0]["Ell_DayCode"].ToString();
                Restday = Convert.ToBoolean(drArrNextDay[0]["Ell_RestDay"]);
            }
            else
            {
                drArrNextDay = dtNextDayCodeLastDay.Select(string.Format(" Ell_EmployeeId = '{0}' ", EmployeeId));

                if (drArrNextDay.Length > 0)
                {
                    DayCode = drArrNextDay[0]["Ell_DayCode"].ToString();
                    Restday = Convert.ToBoolean(drArrNextDay[0]["Ell_Restday"]);
                }
            }
        }

        public DataTable GetNextDayCodeForLastDay(DateTime dtLastDay, bool ProcessAll, string EmployeeId)
        {
            string EmployeeCondition = "";
            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition = " AND Ell_EmployeeID = '" + EmployeeId + "' ";
            if (ProcessAll == true && EmployeeList != "")
                EmployeeCondition = " AND Ell_EmployeeID IN (" + EmployeeList + ") ";

            string query = string.Format(@"SELECT Ell_EmployeeId, Ell_DayCode, Ell_RestDay 
                                                FROM T_EmployeeLogLedger
                                                WHERE Ell_ProcessDate = '{0}' {1}", dtLastDay.AddDays(1), EmployeeCondition);
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public void SavePayrollTransactionAmount(string DayCode, bool Restday, double Amount, HourType hrType, string PrevDayCode)
        {
            double dHr = 0, dOTHr = 0, dNDHr = 0, dNDOTHr = 0;
            #region Regular Day
            if ((DayCode.Equals("REG") || DayCode.Equals("REGN")) && !Restday)
            {
                if (hrType == HourType.RegHour)
                {
                    drEmployeePayrollTransactionDetail["Ept_RegularHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RegularHr"]) + Amount;
                }
                else if (hrType == HourType.OTHour)
                {
                    drEmployeePayrollTransactionDetail["Ept_RegularOTHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RegularOTHr"]) + Amount;
                }
                else if (hrType == HourType.NDHour)
                {
                    drEmployeePayrollTransactionDetail["Ept_RegularNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RegularNDHr"]) + Amount;
                }
                else if (hrType == HourType.NDOTHour)
                {
                    drEmployeePayrollTransactionDetail["Ept_RegularOTNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RegularOTNDHr"]) + Amount;
                }

                //Night Diff
                if (Convert.ToBoolean(NDPREM1ST8) == true)
                {
                    if (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RegularNDHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RegularOTNDHr"]) <= 8)
                    {
                        drEmployeePayrollTransactionDetail["Ept_RegularNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RegularNDHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RegularOTNDHr"]);
                        drEmployeePayrollTransactionDetail["Ept_RegularOTNDHr"] = 0;
                    }
                    else
                    {
                        drEmployeePayrollTransactionDetail["Ept_RegularOTNDHr"] = (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RegularNDHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RegularOTNDHr"])) - 8;
                        drEmployeePayrollTransactionDetail["Ept_RegularNDHr"] = 8;
                    }
                }
            }
            #endregion
            #region Rest Day
            else if (DayCode.Equals("REST") && Restday)
            {
                dHr = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayHr"]);
                dOTHr = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayOTHr"]);
                dNDHr = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayNDHr"]);
                dNDOTHr = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayOTNDHr"]);

                if (hrType == HourType.RegHour)
                {
                    drEmployeePayrollTransactionDetail["Ept_RestdayHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayHr"]) + Amount;
                }
                else if (hrType == HourType.OTHour)
                {
                    drEmployeePayrollTransactionDetail["Ept_RestdayOTHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayOTHr"]) + Amount;
                }
                else if (hrType == HourType.NDHour)
                {
                    drEmployeePayrollTransactionDetail["Ept_RestdayNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayNDHr"]) + Amount;
                }
                else if (hrType == HourType.NDOTHour)
                {
                    drEmployeePayrollTransactionDetail["Ept_RestdayOTNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayOTNDHr"]) + Amount;
                }

                if (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayOTHr"]) <= 8)
                {
                    drEmployeePayrollTransactionDetail["Ept_RestdayHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayOTHr"]);
                    drEmployeePayrollTransactionDetail["Ept_RestdayOTHr"] = 0;
                }
                else
                {
                    drEmployeePayrollTransactionDetail["Ept_RestdayOTHr"] = (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayOTHr"])) - 8;
                    drEmployeePayrollTransactionDetail["Ept_RestdayHr"] = 8;
                }

                //Night Diff
                if (Convert.ToBoolean(NDPREM1ST8) == true
                    || (Convert.ToBoolean(NDSPLTSHFT) == true && PrevDayCode != "" && PrevDayCode != DayCode))
                {
                    if (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayNDHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayOTNDHr"]) <= 8)
                    {
                        drEmployeePayrollTransactionDetail["Ept_RestdayNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayNDHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayOTNDHr"]);
                        drEmployeePayrollTransactionDetail["Ept_RestdayOTNDHr"] = 0;
                    }
                    else
                    {
                        drEmployeePayrollTransactionDetail["Ept_RestdayOTNDHr"] = (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayNDHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayOTNDHr"])) - 8;
                        drEmployeePayrollTransactionDetail["Ept_RestdayNDHr"] = 8;
                    }
                }

                if (drEmployeePayrollTransactionDetailPrev != null && Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayOTHr"]) > 0)
                {
                    if (HOURFRACFORMULA1 == "1")
                    {
                        //Restore values of the payroll transaction column
                        if (dHr
                            + dOTHr
                            + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayHr"])
                            + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayOTHr"]) <= 8)
                        {
                            RestdayHr -= (dHr + dOTHr + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayOTHr"]));
                        }
                        else
                        {
                            RestdayOTHr -= (dHr + dOTHr + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayOTHr"])) - 8;
                            RestdayHr -= 8;
                        }
                        bHasAddedCurrentHrs = true; //Added by Rendell Uy (6/10/2013) For proper distribution of hours if HRFRCLBRHR is enabled

                        if (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayOTHr"])
                            + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayOTHr"]) <= 8)
                        {
                            RestdayHr += Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayOTHr"])
                                                + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayOTHr"]);
                        }
                        else
                        {
                            RestdayOTHr += (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayOTHr"])
                                                + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayOTHr"]))
                                                - 8;
                            RestdayHr += 8;

                            drEmployeePayrollTransactionDetail["Ept_RestdayOTHr"] = (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayOTHr"])
                                                                                            + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayOTHr"]))
                                                                                        - 8 - Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayOTHr"]);
                            drEmployeePayrollTransactionDetail["Ept_RestdayHr"] = 8 - (Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayHr"])
                                                                                        + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayOTHr"]));
                        }
                    }
                    else if (HOURFRACFORMULA2 == "1")
                    {
                        if (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayOTHr"])
                        + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayOTHr"]) <= 8)
                        {
                            drEmployeePayrollTransactionDetail["Ept_RestdayHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayOTHr"])
                                                                                    + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayOTHr"]);
                            drEmployeePayrollTransactionDetail["Ept_RestdayOTHr"] = 0;
                        }
                        else
                        {
                            drEmployeePayrollTransactionDetail["Ept_RestdayOTHr"] = (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayOTHr"])
                                                                                            + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayOTHr"]))
                                                                                        - 8 - Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayOTHr"]);
                            drEmployeePayrollTransactionDetail["Ept_RestdayHr"] = 8 - Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayHr"]);
                        }
                    }
                }
            }
            #endregion
            #region Legal Holiday
            else if (DayCode.Equals("HOL") && !Restday)
            {
                dHr = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_LegalHolidayHr"]);
                dOTHr = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_LegalHolidayOTHr"]);
                dNDHr = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_LegalHolidayNDHr"]);
                dNDOTHr = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_LegalHolidayOTNDHr"]);

                if (hrType == HourType.RegHour)
                {
                    drEmployeePayrollTransactionDetail["Ept_LegalHolidayHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_LegalHolidayHr"]) + Amount;
                }
                else if (hrType == HourType.OTHour)
                {
                    drEmployeePayrollTransactionDetail["Ept_LegalHolidayOTHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_LegalHolidayOTHr"]) + Amount;
                }
                else if (hrType == HourType.NDHour)
                {
                    drEmployeePayrollTransactionDetail["Ept_LegalHolidayNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_LegalHolidayNDHr"]) + Amount;
                }
                else if (hrType == HourType.NDOTHour)
                {
                    drEmployeePayrollTransactionDetail["Ept_LegalHolidayOTNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_LegalHolidayOTNDHr"]) + Amount;
                }

                if (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_LegalHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_LegalHolidayOTHr"]) <= 8)
                {
                    drEmployeePayrollTransactionDetail["Ept_LegalHolidayHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_LegalHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_LegalHolidayOTHr"]);
                    drEmployeePayrollTransactionDetail["Ept_LegalHolidayOTHr"] = 0;
                }
                else
                {
                    drEmployeePayrollTransactionDetail["Ept_LegalHolidayOTHr"] = (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_LegalHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_LegalHolidayOTHr"])) - 8;
                    drEmployeePayrollTransactionDetail["Ept_LegalHolidayHr"] = 8;
                }

                //Night Diff
                if (Convert.ToBoolean(NDPREM1ST8) == true
                    || (Convert.ToBoolean(NDSPLTSHFT) == true && PrevDayCode != "" && PrevDayCode != DayCode))
                {
                    if (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_LegalHolidayNDHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_LegalHolidayOTNDHr"]) <= 8)
                    {
                        drEmployeePayrollTransactionDetail["Ept_LegalHolidayNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_LegalHolidayNDHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_LegalHolidayOTNDHr"]);
                        drEmployeePayrollTransactionDetail["Ept_LegalHolidayOTNDHr"] = 0;
                    }
                    else
                    {
                        drEmployeePayrollTransactionDetail["Ept_LegalHolidayOTNDHr"] = (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_LegalHolidayNDHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_LegalHolidayOTNDHr"])) - 8;
                        drEmployeePayrollTransactionDetail["Ept_LegalHolidayNDHr"] = 8;
                    }
                }

                if (drEmployeePayrollTransactionDetailPrev != null && Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_LegalHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_LegalHolidayOTHr"]) > 0)
                {
                    if (HOURFRACFORMULA1 == "1")
                    {
                        //Restore values of the payroll transaction column
                        if (dHr
                            + dOTHr
                            + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_LegalHolidayHr"])
                            + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_LegalHolidayOTHr"]) <= 8)
                        {
                            LegalHolidayHr -= (dHr + dOTHr + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_LegalHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_LegalHolidayOTHr"]));
                        }
                        else
                        {
                            LegalHolidayOTHr -= (dHr + dOTHr + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_LegalHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_LegalHolidayOTHr"])) - 8;
                            LegalHolidayHr -= 8;
                        }
                        bHasAddedCurrentHrs = true; //Added by Rendell Uy (6/10/2013) For proper distribution of hours if HRFRCLBRHR is enabled

                        if (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_LegalHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_LegalHolidayOTHr"])
                            + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_LegalHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_LegalHolidayOTHr"]) <= 8)
                        {
                            LegalHolidayHr += Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_LegalHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_LegalHolidayOTHr"])
                                                + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_LegalHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_LegalHolidayOTHr"]);
                        }
                        else
                        {
                            LegalHolidayOTHr += (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_LegalHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_LegalHolidayOTHr"])
                                                + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_LegalHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_LegalHolidayOTHr"]))
                                                - 8;
                            LegalHolidayHr += 8;

                            drEmployeePayrollTransactionDetail["Ept_LegalHolidayOTHr"] = (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_LegalHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_LegalHolidayOTHr"])
                                                                                            + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_LegalHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_LegalHolidayOTHr"]))
                                                                                        - 8 - Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_LegalHolidayOTHr"]);
                            drEmployeePayrollTransactionDetail["Ept_LegalHolidayHr"] = 8 - (Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_LegalHolidayHr"])
                                                                                        + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_LegalHolidayOTHr"]));
                        }
                    }
                    else if (HOURFRACFORMULA2 == "1")
                    {
                        if (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_LegalHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_LegalHolidayOTHr"])
                        + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_LegalHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_LegalHolidayOTHr"]) <= 8)
                        {
                            drEmployeePayrollTransactionDetail["Ept_LegalHolidayHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_LegalHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_LegalHolidayOTHr"])
                                                                                    + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_LegalHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_LegalHolidayOTHr"]);
                            drEmployeePayrollTransactionDetail["Ept_LegalHolidayOTHr"] = 0;
                        }
                        else
                        {
                            drEmployeePayrollTransactionDetail["Ept_LegalHolidayOTHr"] = (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_LegalHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_LegalHolidayOTHr"])
                                                                                                + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_LegalHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_LegalHolidayOTHr"]))
                                                                                            - 8 - Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_LegalHolidayOTHr"]);
                            drEmployeePayrollTransactionDetail["Ept_LegalHolidayHr"] = 8 - Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_LegalHolidayHr"]);
                        }
                    }
                }
            }
            #endregion
            #region Special Holiday
            else if (DayCode.Equals("SPL") && !Restday)
            {
                dHr = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_SpecialHolidayHr"]);
                dOTHr = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_SpecialHolidayOTHr"]);
                dNDHr = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_SpecialHolidayNDHr"]);
                dNDOTHr = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_SpecialHolidayOTNDHr"]);

                if (hrType == HourType.RegHour)
                {
                    drEmployeePayrollTransactionDetail["Ept_SpecialHolidayHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_SpecialHolidayHr"]) + Amount;
                }
                else if (hrType == HourType.OTHour)
                {
                    drEmployeePayrollTransactionDetail["Ept_SpecialHolidayOTHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_SpecialHolidayOTHr"]) + Amount;
                }
                else if (hrType == HourType.NDHour)
                {
                    drEmployeePayrollTransactionDetail["Ept_SpecialHolidayNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_SpecialHolidayNDHr"]) + Amount;
                }
                else if (hrType == HourType.NDOTHour)
                {
                    drEmployeePayrollTransactionDetail["Ept_SpecialHolidayOTNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_SpecialHolidayOTNDHr"]) + Amount;
                }

                if (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_SpecialHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_SpecialHolidayOTHr"]) <= 8)
                {
                    drEmployeePayrollTransactionDetail["Ept_SpecialHolidayHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_SpecialHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_SpecialHolidayOTHr"]);
                    drEmployeePayrollTransactionDetail["Ept_SpecialHolidayOTHr"] = 0;
                }
                else
                {
                    drEmployeePayrollTransactionDetail["Ept_SpecialHolidayOTHr"] = (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_SpecialHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_SpecialHolidayOTHr"])) - 8;
                    drEmployeePayrollTransactionDetail["Ept_SpecialHolidayHr"] = 8;
                }

                //Night Diff
                if (Convert.ToBoolean(NDPREM1ST8) == true
                    || (Convert.ToBoolean(NDSPLTSHFT) == true && PrevDayCode != "" && PrevDayCode != DayCode))
                {
                    if (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_SpecialHolidayNDHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_SpecialHolidayOTNDHr"]) <= 8)
                    {
                        drEmployeePayrollTransactionDetail["Ept_SpecialHolidayNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_SpecialHolidayNDHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_SpecialHolidayOTNDHr"]);
                        drEmployeePayrollTransactionDetail["Ept_SpecialHolidayOTNDHr"] = 0;
                    }
                    else
                    {
                        drEmployeePayrollTransactionDetail["Ept_SpecialHolidayOTNDHr"] = (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_SpecialHolidayNDHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_SpecialHolidayOTNDHr"])) - 8;
                        drEmployeePayrollTransactionDetail["Ept_SpecialHolidayNDHr"] = 8;
                    }
                }

                if (drEmployeePayrollTransactionDetailPrev != null && Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_SpecialHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_SpecialHolidayOTHr"]) > 0)
                {
                    if (HOURFRACFORMULA1 == "1")
                    {
                        //Restore values of the payroll transaction column
                        if (dHr
                            + dOTHr
                            + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_SpecialHolidayHr"])
                            + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_SpecialHolidayOTHr"]) <= 8)
                        {
                            SpecialHolidayHr -= (dHr + dOTHr + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_SpecialHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_SpecialHolidayOTHr"]));
                        }
                        else
                        {
                            SpecialHolidayOTHr -= (dHr + dOTHr + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_SpecialHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_SpecialHolidayOTHr"])) - 8;
                            SpecialHolidayHr -= 8;
                        }
                        bHasAddedCurrentHrs = true; //Added by Rendell Uy (6/10/2013) For proper distribution of hours if HRFRCLBRHR is enabled

                        if (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_SpecialHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_SpecialHolidayOTHr"])
                            + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_SpecialHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_SpecialHolidayOTHr"]) <= 8)
                        {
                            SpecialHolidayHr += Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_SpecialHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_SpecialHolidayOTHr"])
                                                + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_SpecialHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_SpecialHolidayOTHr"]);
                        }
                        else
                        {
                            SpecialHolidayOTHr += (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_SpecialHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_SpecialHolidayOTHr"])
                                                + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_SpecialHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_SpecialHolidayOTHr"]))
                                                - 8;
                            SpecialHolidayHr += 8;

                            drEmployeePayrollTransactionDetail["Ept_SpecialHolidayOTHr"] = (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_SpecialHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_SpecialHolidayOTHr"])
                                                                                            + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_SpecialHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_SpecialHolidayOTHr"]))
                                                                                        - 8 - Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_SpecialHolidayOTHr"]);
                            drEmployeePayrollTransactionDetail["Ept_SpecialHolidayHr"] = 8 - (Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_SpecialHolidayHr"])
                                                                                        + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_SpecialHolidayOTHr"]));
                        }
                    }
                    else if (HOURFRACFORMULA2 == "1")
                    {
                        if (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_SpecialHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_SpecialHolidayOTHr"])
                        + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_SpecialHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_SpecialHolidayOTHr"]) <= 8)
                        {
                            drEmployeePayrollTransactionDetail["Ept_SpecialHolidayHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_SpecialHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_SpecialHolidayOTHr"])
                                                                                    + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_SpecialHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_SpecialHolidayOTHr"]);
                            drEmployeePayrollTransactionDetail["Ept_SpecialHolidayOTHr"] = 0;
                        }
                        else
                        {
                            drEmployeePayrollTransactionDetail["Ept_SpecialHolidayOTHr"] = (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_SpecialHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_SpecialHolidayOTHr"])
                                                                                                + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_SpecialHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_SpecialHolidayOTHr"]))
                                                                                            - 8 - Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_SpecialHolidayOTHr"]);
                            drEmployeePayrollTransactionDetail["Ept_SpecialHolidayHr"] = 8 - Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_SpecialHolidayHr"]);
                        }
                    }
                }
            }
            #endregion
            #region Plant Shutdown
            else if (DayCode.Equals("PSD") && !Restday)
            {
                dHr = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_PlantShutdownHr"]);
                dOTHr = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_PlantShutdownOTHr"]);
                dNDHr = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_PlantShutdownNDHr"]);
                dNDOTHr = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_PlantShutdownOTNDHr"]);

                if (hrType == HourType.RegHour)
                {
                    drEmployeePayrollTransactionDetail["Ept_PlantShutdownHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_PlantShutdownHr"]) + Amount;
                }
                else if (hrType == HourType.OTHour)
                {
                    drEmployeePayrollTransactionDetail["Ept_PlantShutdownOTHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_PlantShutdownOTHr"]) + Amount;
                }
                else if (hrType == HourType.NDHour)
                {
                    drEmployeePayrollTransactionDetail["Ept_PlantShutdownNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_PlantShutdownNDHr"]) + Amount;
                }
                else if (hrType == HourType.NDOTHour)
                {
                    drEmployeePayrollTransactionDetail["Ept_PlantShutdownOTNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_PlantShutdownOTNDHr"]) + Amount;
                }

                if (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_PlantShutdownHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_PlantShutdownOTHr"]) <= 8)
                {
                    drEmployeePayrollTransactionDetail["Ept_PlantShutdownHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_PlantShutdownHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_PlantShutdownOTHr"]);
                    drEmployeePayrollTransactionDetail["Ept_PlantShutdownOTHr"] = 0;
                }
                else
                {
                    drEmployeePayrollTransactionDetail["Ept_PlantShutdownOTHr"] = (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_PlantShutdownHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_PlantShutdownOTHr"])) - 8;
                    drEmployeePayrollTransactionDetail["Ept_PlantShutdownHr"] = 8;
                }

                //Night Diff
                if (Convert.ToBoolean(NDPREM1ST8) == true
                    || (Convert.ToBoolean(NDSPLTSHFT) == true && PrevDayCode != "" && PrevDayCode != DayCode))
                {
                    if (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_PlantShutdownNDHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_PlantShutdownOTNDHr"]) <= 8)
                    {
                        drEmployeePayrollTransactionDetail["Ept_PlantShutdownNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_PlantShutdownNDHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_PlantShutdownOTNDHr"]);
                        drEmployeePayrollTransactionDetail["Ept_PlantShutdownOTNDHr"] = 0;
                    }
                    else
                    {
                        drEmployeePayrollTransactionDetail["Ept_PlantShutdownOTNDHr"] = (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_PlantShutdownNDHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_PlantShutdownOTNDHr"])) - 8;
                        drEmployeePayrollTransactionDetail["Ept_PlantShutdownNDHr"] = 8;
                    }
                }

                if (drEmployeePayrollTransactionDetailPrev != null && Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_PlantShutdownHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_PlantShutdownOTHr"]) > 0)
                {
                    if (HOURFRACFORMULA1 == "1")
                    {
                        //Restore values of the payroll transaction column
                        if (dHr
                            + dOTHr
                            + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_PlantShutdownHr"])
                            + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_PlantShutdownOTHr"]) <= 8)
                        {
                            PlantShutdownHr -= (dHr + dOTHr + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_PlantShutdownHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_PlantShutdownOTHr"]));
                        }
                        else
                        {
                            PlantShutdownOTHr -= (dHr + dOTHr + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_PlantShutdownHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_PlantShutdownOTHr"])) - 8;
                            PlantShutdownHr -= 8;
                        }
                        bHasAddedCurrentHrs = true; //Added by Rendell Uy (6/10/2013) For proper distribution of hours if HRFRCLBRHR is enabled

                        if (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_PlantShutdownHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_PlantShutdownOTHr"])
                            + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_PlantShutdownHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_PlantShutdownOTHr"]) <= 8)
                        {
                            PlantShutdownHr += Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_PlantShutdownHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_PlantShutdownOTHr"])
                                                + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_PlantShutdownHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_PlantShutdownOTHr"]);
                        }
                        else
                        {
                            PlantShutdownOTHr += (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_PlantShutdownHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_PlantShutdownOTHr"])
                                                + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_PlantShutdownHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_PlantShutdownOTHr"]))
                                                - 8;
                            PlantShutdownHr += 8;

                            drEmployeePayrollTransactionDetail["Ept_PlantShutdownOTHr"] = (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_PlantShutdownHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_PlantShutdownOTHr"])
                                                                                            + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_PlantShutdownHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_PlantShutdownOTHr"]))
                                                                                        - 8 - Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_PlantShutdownOTHr"]);
                            drEmployeePayrollTransactionDetail["Ept_PlantShutdownHr"] = 8 - (Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_PlantShutdownHr"])
                                                                                        + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_PlantShutdownOTHr"]));
                        }
                    }
                    else if (HOURFRACFORMULA2 == "1")
                    {
                        if (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_PlantShutdownHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_PlantShutdownOTHr"])
                        + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_PlantShutdownHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_PlantShutdownOTHr"]) <= 8)
                        {
                            drEmployeePayrollTransactionDetail["Ept_PlantShutdownHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_PlantShutdownHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_PlantShutdownOTHr"])
                                                                                    + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_PlantShutdownHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_PlantShutdownOTHr"]);
                            drEmployeePayrollTransactionDetail["Ept_PlantShutdownOTHr"] = 0;
                        }
                        else
                        {
                            drEmployeePayrollTransactionDetail["Ept_PlantShutdownOTHr"] = (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_PlantShutdownHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_PlantShutdownOTHr"])
                                                                                                + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_PlantShutdownHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_PlantShutdownOTHr"]))
                                                                                            - 8 - Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_PlantShutdownOTHr"]);
                            drEmployeePayrollTransactionDetail["Ept_PlantShutdownHr"] = 8 - Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_PlantShutdownHr"]);
                        }
                    }
                }
            }
            #endregion
            #region Company Holiday
            else if (DayCode.Equals("COMP") && !Restday)
            {
                dHr = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_CompanyHolidayHr"]);
                dOTHr = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_CompanyHolidayOTHr"]);
                dNDHr = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_CompanyHolidayNDHr"]);
                dNDOTHr = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_CompanyHolidayOTNDHr"]);

                if (hrType == HourType.RegHour)
                {
                    drEmployeePayrollTransactionDetail["Ept_CompanyHolidayHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_CompanyHolidayHr"]) + Amount;
                }
                else if (hrType == HourType.OTHour)
                {
                    drEmployeePayrollTransactionDetail["Ept_CompanyHolidayOTHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_CompanyHolidayOTHr"]) + Amount;
                }
                else if (hrType == HourType.NDHour)
                {
                    drEmployeePayrollTransactionDetail["Ept_CompanyHolidayNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_CompanyHolidayNDHr"]) + Amount;
                }
                else if (hrType == HourType.NDOTHour)
                {
                    drEmployeePayrollTransactionDetail["Ept_CompanyHolidayOTNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_CompanyHolidayOTNDHr"]) + Amount;
                }

                if (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_CompanyHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_CompanyHolidayOTHr"]) <= 8)
                {
                    drEmployeePayrollTransactionDetail["Ept_CompanyHolidayHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_CompanyHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_CompanyHolidayOTHr"]);
                    drEmployeePayrollTransactionDetail["Ept_CompanyHolidayOTHr"] = 0;
                }
                else
                {
                    drEmployeePayrollTransactionDetail["Ept_CompanyHolidayOTHr"] = (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_CompanyHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_CompanyHolidayOTHr"])) - 8;
                    drEmployeePayrollTransactionDetail["Ept_CompanyHolidayHr"] = 8;
                }

                //Night Diff
                if (Convert.ToBoolean(NDPREM1ST8) == true
                    || (Convert.ToBoolean(NDSPLTSHFT) == true && PrevDayCode != "" && PrevDayCode != DayCode))
                {
                    if (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_CompanyHolidayNDHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_CompanyHolidayOTNDHr"]) <= 8)
                    {
                        drEmployeePayrollTransactionDetail["Ept_CompanyHolidayNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_CompanyHolidayNDHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_CompanyHolidayOTNDHr"]);
                        drEmployeePayrollTransactionDetail["Ept_CompanyHolidayOTNDHr"] = 0;
                    }
                    else
                    {
                        drEmployeePayrollTransactionDetail["Ept_CompanyHolidayOTNDHr"] = (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_CompanyHolidayNDHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_CompanyHolidayOTNDHr"])) - 8;
                        drEmployeePayrollTransactionDetail["Ept_CompanyHolidayNDHr"] = 8;
                    }
                }

                if (drEmployeePayrollTransactionDetailPrev != null && Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_CompanyHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_CompanyHolidayOTHr"]) > 0)
                {
                    if (HOURFRACFORMULA1 == "1")
                    {
                        //Restore values of the payroll transaction column
                        if (dHr
                            + dOTHr
                            + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_CompanyHolidayHr"])
                            + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_CompanyHolidayOTHr"]) <= 8)
                        {
                            CompanyHolidayHr -= (dHr + dOTHr + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_CompanyHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_CompanyHolidayOTHr"]));
                        }
                        else
                        {
                            CompanyHolidayOTHr -= (dHr + dOTHr + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_CompanyHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_CompanyHolidayOTHr"])) - 8;
                            CompanyHolidayHr -= 8;
                        }
                        bHasAddedCurrentHrs = true; //Added by Rendell Uy (6/10/2013) For proper distribution of hours if HRFRCLBRHR is enabled

                        if (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_CompanyHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_CompanyHolidayOTHr"])
                            + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_CompanyHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_CompanyHolidayOTHr"]) <= 8)
                        {
                            CompanyHolidayHr += Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_CompanyHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_CompanyHolidayOTHr"])
                                                + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_CompanyHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_CompanyHolidayOTHr"]);
                        }
                        else
                        {
                            CompanyHolidayOTHr += (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_CompanyHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_CompanyHolidayOTHr"])
                                                + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_CompanyHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_CompanyHolidayOTHr"]))
                                                - 8;
                            CompanyHolidayHr += 8;

                            drEmployeePayrollTransactionDetail["Ept_CompanyHolidayOTHr"] = (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_CompanyHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_CompanyHolidayOTHr"])
                                                                                            + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_CompanyHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_CompanyHolidayOTHr"]))
                                                                                        - 8 - Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_CompanyHolidayOTHr"]);
                            drEmployeePayrollTransactionDetail["Ept_CompanyHolidayHr"] = 8 - (Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_CompanyHolidayHr"])
                                                                                        + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_CompanyHolidayOTHr"]));
                        }
                    }
                    else if (HOURFRACFORMULA2 == "1")
                    {
                        if (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_CompanyHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_CompanyHolidayOTHr"])
                        + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_CompanyHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_CompanyHolidayOTHr"]) <= 8)
                        {
                            drEmployeePayrollTransactionDetail["Ept_CompanyHolidayHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_CompanyHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_CompanyHolidayOTHr"])
                                                                                    + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_CompanyHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_CompanyHolidayOTHr"]);
                            drEmployeePayrollTransactionDetail["Ept_CompanyHolidayOTHr"] = 0;
                        }
                        else
                        {
                            drEmployeePayrollTransactionDetail["Ept_CompanyHolidayOTHr"] = (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_CompanyHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_CompanyHolidayOTHr"])
                                                                                                + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_CompanyHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_CompanyHolidayOTHr"]))
                                                                                            - 8 - Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_CompanyHolidayOTHr"]);
                            drEmployeePayrollTransactionDetail["Ept_CompanyHolidayHr"] = 8 - Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_CompanyHolidayHr"]);
                        }
                    }
                }
            }
            #endregion
            #region Restday Legal Holiday
            else if (DayCode.Equals("HOL") && Restday)
            {
                dHr = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayHr"]);
                dOTHr = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayOTHr"]);
                dNDHr = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayNDHr"]);
                dNDOTHr = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayOTNDHr"]);

                if (hrType == HourType.RegHour)
                {
                    drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayHr"]) + Amount;
                }
                else if (hrType == HourType.OTHour)
                {
                    drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayOTHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayOTHr"]) + Amount;
                }
                else if (hrType == HourType.NDHour)
                {
                    drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayNDHr"]) + Amount;
                }
                else if (hrType == HourType.NDOTHour)
                {
                    drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayOTNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayOTNDHr"]) + Amount;
                }

                if (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayOTHr"]) <= 8)
                {
                    drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayOTHr"]);
                    drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayOTHr"] = 0;
                }
                else
                {
                    drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayOTHr"] = (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayOTHr"])) - 8;
                    drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayHr"] = 8;
                }

                //Night Diff
                if (Convert.ToBoolean(NDPREM1ST8) == true
                    || (Convert.ToBoolean(NDSPLTSHFT) == true && PrevDayCode != "" && PrevDayCode != DayCode))
                {
                    if (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayNDHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayOTNDHr"]) <= 8)
                    {
                        drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayNDHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayOTNDHr"]);
                        drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayOTNDHr"] = 0;
                    }
                    else
                    {
                        drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayOTNDHr"] = (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayNDHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayOTNDHr"])) - 8;
                        drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayNDHr"] = 8;
                    }
                }

                if (drEmployeePayrollTransactionDetailPrev != null && Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayLegalHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayLegalHolidayOTHr"]) > 0)
                {
                    if (HOURFRACFORMULA1 == "1")
                    {
                        //Restore values of the payroll transaction column
                        if (dHr
                            + dOTHr
                            + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayLegalHolidayHr"])
                            + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayLegalHolidayOTHr"]) <= 8)
                        {
                            RestdayLegalHolidayHr -= (dHr + dOTHr + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayLegalHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayLegalHolidayOTHr"]));
                        }
                        else
                        {
                            RestdayLegalHolidayOTHr -= (dHr + dOTHr + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayLegalHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayLegalHolidayOTHr"])) - 8;
                            RestdayLegalHolidayHr -= 8;
                        }
                        bHasAddedCurrentHrs = true; //Added by Rendell Uy (6/10/2013) For proper distribution of hours if HRFRCLBRHR is enabled

                        if (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayOTHr"])
                            + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayLegalHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayLegalHolidayOTHr"]) <= 8)
                        {
                            RestdayLegalHolidayHr += Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayOTHr"])
                                                + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayLegalHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayLegalHolidayOTHr"]);
                        }
                        else
                        {
                            RestdayLegalHolidayOTHr += (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayOTHr"])
                                                + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayLegalHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayLegalHolidayOTHr"]))
                                                - 8;
                            RestdayLegalHolidayHr += 8;

                            drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayOTHr"] = (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayOTHr"])
                                                                                            + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayLegalHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayLegalHolidayOTHr"]))
                                                                                        - 8 - Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayLegalHolidayOTHr"]);
                            drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayHr"] = 8 - (Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayLegalHolidayHr"])
                                                                                        + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayLegalHolidayOTHr"]));
                        }
                    }
                    else if (HOURFRACFORMULA2 == "1")
                    {
                        if (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayOTHr"])
                        + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayLegalHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayLegalHolidayOTHr"]) <= 8)
                        {
                            drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayOTHr"])
                                                                                    + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayLegalHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayLegalHolidayOTHr"]);
                            drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayOTHr"] = 0;
                        }
                        else
                        {
                            drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayOTHr"] = (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayOTHr"])
                                                                                                + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayLegalHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayLegalHolidayOTHr"]))
                                                                                            - 8 - Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayLegalHolidayOTHr"]);
                            drEmployeePayrollTransactionDetail["Ept_RestdayLegalHolidayHr"] = 8 - Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayLegalHolidayHr"]);
                        }
                    }
                }
            }
            #endregion
            #region Restday Special Holiday
            else if (DayCode.Equals("SPL") && Restday)
            {
                dHr = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayHr"]);
                dOTHr = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayOTHr"]);
                dNDHr = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayNDHr"]);
                dNDOTHr = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayOTNDHr"]);

                if (hrType == HourType.RegHour)
                {
                    drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayHr"]) + Amount;
                }
                else if (hrType == HourType.OTHour)
                {
                    drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayOTHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayOTHr"]) + Amount;
                }
                else if (hrType == HourType.NDHour)
                {
                    drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayNDHr"]) + Amount;
                }
                else if (hrType == HourType.NDOTHour)
                {
                    drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayOTNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayOTNDHr"]) + Amount;
                }

                if (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayOTHr"]) <= 8)
                {
                    drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayOTHr"]);
                    drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayOTHr"] = 0;
                }
                else
                {
                    drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayOTHr"] = (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayOTHr"])) - 8;
                    drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayHr"] = 8;
                }

                //Night Diff
                if (Convert.ToBoolean(NDPREM1ST8) == true
                    || (Convert.ToBoolean(NDSPLTSHFT) == true && PrevDayCode != "" && PrevDayCode != DayCode))
                {
                    if (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayNDHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayOTNDHr"]) <= 8)
                    {
                        drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayNDHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayOTNDHr"]);
                        drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayOTNDHr"] = 0;
                    }
                    else
                    {
                        drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayOTNDHr"] = (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayNDHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayOTNDHr"])) - 8;
                        drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayNDHr"] = 8;
                    }
                }

                if (drEmployeePayrollTransactionDetailPrev != null && Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdaySpecialHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdaySpecialHolidayOTHr"]) > 0)
                {
                    if (HOURFRACFORMULA1 == "1")
                    {
                        //Restore values of the payroll transaction column
                        if (dHr
                            + dOTHr
                            + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdaySpecialHolidayHr"])
                            + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdaySpecialHolidayOTHr"]) <= 8)
                        {
                            RestdaySpecialHolidayHr -= (dHr + dOTHr + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdaySpecialHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdaySpecialHolidayOTHr"]));
                        }
                        else
                        {
                            RestdaySpecialHolidayOTHr -= (dHr + dOTHr + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdaySpecialHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdaySpecialHolidayOTHr"])) - 8;
                            RestdaySpecialHolidayHr -= 8;
                        }
                        bHasAddedCurrentHrs = true; //Added by Rendell Uy (6/10/2013) For proper distribution of hours if HRFRCLBRHR is enabled

                        if (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayOTHr"])
                            + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdaySpecialHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdaySpecialHolidayOTHr"]) <= 8)
                        {
                            RestdaySpecialHolidayHr += Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayOTHr"])
                                                + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdaySpecialHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdaySpecialHolidayOTHr"]);
                        }
                        else
                        {
                            RestdaySpecialHolidayOTHr += (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayOTHr"])
                                                + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdaySpecialHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdaySpecialHolidayOTHr"]))
                                                - 8;
                            RestdaySpecialHolidayHr += 8;

                            drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayOTHr"] = (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayOTHr"])
                                                                                            + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdaySpecialHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdaySpecialHolidayOTHr"]))
                                                                                        - 8 - Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdaySpecialHolidayOTHr"]);
                            drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayHr"] = 8 - (Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdaySpecialHolidayHr"])
                                                                                        + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdaySpecialHolidayOTHr"]));
                        }
                    }
                    else if (HOURFRACFORMULA2 == "1")
                    {
                        if (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayOTHr"])
                        + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdaySpecialHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdaySpecialHolidayOTHr"]) <= 8)
                        {
                            drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayOTHr"])
                                                                                    + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdaySpecialHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdaySpecialHolidayOTHr"]);
                            drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayOTHr"] = 0;
                        }
                        else
                        {
                            drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayOTHr"] = (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayOTHr"])
                                                                                                + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdaySpecialHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdaySpecialHolidayOTHr"]))
                                                                                            - 8 - Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdaySpecialHolidayOTHr"]);
                            drEmployeePayrollTransactionDetail["Ept_RestdaySpecialHolidayHr"] = 8 - Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdaySpecialHolidayHr"]);
                        }
                    }
                }
            }
            #endregion
            #region Restday Company Holiday
            else if (DayCode.Equals("COMP") && Restday)
            {
                dHr = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayHr"]);
                dOTHr = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayOTHr"]);
                dNDHr = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayNDHr"]);
                dNDOTHr = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayOTNDHr"]);

                if (hrType == HourType.RegHour)
                {
                    drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayHr"]) + Amount;
                }
                else if (hrType == HourType.OTHour)
                {
                    drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayOTHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayOTHr"]) + Amount;
                }
                else if (hrType == HourType.NDHour)
                {
                    drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayNDHr"]) + Amount;
                }
                else if (hrType == HourType.NDOTHour)
                {
                    drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayOTNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayOTNDHr"]) + Amount;
                }

                if (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayOTHr"]) <= 8)
                {
                    drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayOTHr"]);
                    drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayOTHr"] = 0;
                }
                else
                {
                    drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayOTHr"] = (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayOTHr"])) - 8;
                    drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayHr"] = 8;
                }

                //Night Diff
                if (Convert.ToBoolean(NDPREM1ST8) == true
                    || (Convert.ToBoolean(NDSPLTSHFT) == true && PrevDayCode != "" && PrevDayCode != DayCode))
                {
                    if (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayNDHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayOTNDHr"]) <= 8)
                    {
                        drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayNDHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayOTNDHr"]);
                        drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayOTNDHr"] = 0;
                    }
                    else
                    {
                        drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayOTNDHr"] = (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayNDHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayOTNDHr"])) - 8;
                        drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayNDHr"] = 8;
                    }
                }

                if (drEmployeePayrollTransactionDetailPrev != null && Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayCompanyHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayCompanyHolidayOTHr"]) > 0)
                {
                    if (HOURFRACFORMULA1 == "1")
                    {
                        //Restore values of the payroll transaction column
                        if (dHr
                            + dOTHr
                            + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayCompanyHolidayHr"])
                            + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayCompanyHolidayOTHr"]) <= 8)
                        {
                            RestdayCompanyHolidayHr -= (dHr + dOTHr + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayCompanyHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayCompanyHolidayOTHr"]));
                        }
                        else
                        {
                            RestdayCompanyHolidayOTHr -= (dHr + dOTHr + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayCompanyHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayCompanyHolidayOTHr"])) - 8;
                            RestdayCompanyHolidayHr -= 8;
                        }
                        bHasAddedCurrentHrs = true; //Added by Rendell Uy (6/10/2013) For proper distribution of hours if HRFRCLBRHR is enabled

                        if (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayOTHr"])
                            + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayCompanyHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayCompanyHolidayOTHr"]) <= 8)
                        {
                            RestdayCompanyHolidayHr += Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayOTHr"])
                                                + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayCompanyHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayCompanyHolidayOTHr"]);
                        }
                        else
                        {
                            RestdayCompanyHolidayOTHr += (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayOTHr"])
                                                + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayCompanyHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayCompanyHolidayOTHr"]))
                                                - 8;
                            RestdayCompanyHolidayHr += 8;

                            drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayOTHr"] = (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayOTHr"])
                                                                                            + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayCompanyHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayCompanyHolidayOTHr"]))
                                                                                        - 8 - Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayCompanyHolidayOTHr"]);
                            drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayHr"] = 8 - (Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayCompanyHolidayHr"])
                                                                                        + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayCompanyHolidayOTHr"]));
                        }
                    }
                    else if (HOURFRACFORMULA2 == "1")
                    {
                        if (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayOTHr"])
                        + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayCompanyHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayCompanyHolidayOTHr"]) <= 8)
                        {
                            drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayOTHr"])
                                                                                    + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayCompanyHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayCompanyHolidayOTHr"]);
                            drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayOTHr"] = 0;
                        }
                        else
                        {
                            drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayOTHr"] = (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayOTHr"])
                                                                                                + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayCompanyHolidayHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayCompanyHolidayOTHr"]))
                                                                                            - 8 - Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayCompanyHolidayOTHr"]);
                            drEmployeePayrollTransactionDetail["Ept_RestdayCompanyHolidayHr"] = 8 - Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayCompanyHolidayHr"]);
                        }
                    }
                }
            }
            #endregion
            #region Restday Plant Shutdown
            else if (DayCode.Equals("PSD") && Restday)
            {
                dHr = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownHr"]);
                dOTHr = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownOTHr"]);
                dNDHr = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownNDHr"]);
                dNDOTHr = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownOTNDHr"]);

                if (hrType == HourType.RegHour)
                {
                    drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownHr"]) + Amount;
                }
                else if (hrType == HourType.OTHour)
                {
                    drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownOTHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownOTHr"]) + Amount;
                }
                else if (hrType == HourType.NDHour)
                {
                    drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownNDHr"]) + Amount;
                }
                else if (hrType == HourType.NDOTHour)
                {
                    drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownOTNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownOTNDHr"]) + Amount;
                }

                if (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownOTHr"]) <= 8)
                {
                    drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownOTHr"]);
                    drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownOTHr"] = 0;
                }
                else
                {
                    drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownOTHr"] = (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownOTHr"])) - 8;
                    drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownHr"] = 8;
                }

                //Night Diff
                if (Convert.ToBoolean(NDPREM1ST8) == true
                    || (Convert.ToBoolean(NDSPLTSHFT) == true && PrevDayCode != "" && PrevDayCode != DayCode))
                {
                    if (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownNDHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownOTNDHr"]) <= 8)
                    {
                        drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownNDHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownOTNDHr"]);
                        drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownOTNDHr"] = 0;
                    }
                    else
                    {
                        drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownOTNDHr"] = (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownNDHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownOTNDHr"])) - 8;
                        drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownNDHr"] = 8;
                    }
                }

                if (drEmployeePayrollTransactionDetailPrev != null && Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayPlantShutdownHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayPlantShutdownOTHr"]) > 0)
                {
                    if (HOURFRACFORMULA1 == "1")
                    {
                        //Restore values of the payroll transaction column
                        if (dHr
                            + dOTHr
                            + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayPlantShutdownHr"])
                            + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayPlantShutdownOTHr"]) <= 8)
                        {
                            RestdayPlantShutdownHr -= (dHr + dOTHr + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayPlantShutdownHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayPlantShutdownOTHr"]));
                        }
                        else
                        {
                            RestdayPlantShutdownOTHr -= (dHr + dOTHr + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayPlantShutdownHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayPlantShutdownOTHr"])) - 8;
                            RestdayPlantShutdownHr -= 8;
                        }
                        bHasAddedCurrentHrs = true; //Added by Rendell Uy (6/10/2013) For proper distribution of hours if HRFRCLBRHR is enabled

                        if (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownOTHr"])
                            + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayPlantShutdownHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayPlantShutdownOTHr"]) <= 8)
                        {
                            RestdayPlantShutdownHr += Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownOTHr"])
                                                + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayPlantShutdownHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayPlantShutdownOTHr"]);
                        }
                        else
                        {
                            RestdayPlantShutdownOTHr += (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownOTHr"])
                                                + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayPlantShutdownHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayPlantShutdownOTHr"]))
                                                - 8;
                            RestdayPlantShutdownHr += 8;

                            drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownOTHr"] = (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownOTHr"])
                                                                                            + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayPlantShutdownHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayPlantShutdownOTHr"]))
                                                                                        - 8 - Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayPlantShutdownOTHr"]);
                            drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownHr"] = 8 - (Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayPlantShutdownHr"])
                                                                                        + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayPlantShutdownOTHr"]));
                        }
                    }
                    else if (HOURFRACFORMULA2 == "1")
                    {
                        if (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownOTHr"])
                        + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayPlantShutdownHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayPlantShutdownOTHr"]) <= 8)
                        {
                            drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownHr"] = Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownOTHr"])
                                                                                    + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayPlantShutdownHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayPlantShutdownOTHr"]);
                            drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownOTHr"] = 0;
                        }
                        else
                        {
                            drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownOTHr"] = (Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownOTHr"])
                                                                                                + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayPlantShutdownHr"]) + Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayPlantShutdownOTHr"]))
                                                                                            - 8 - Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayPlantShutdownOTHr"]);
                            drEmployeePayrollTransactionDetail["Ept_RestdayPlantShutdownHr"] = 8 - Convert.ToDouble(drEmployeePayrollTransactionDetailPrev["Ept_RestdayPlantShutdownHr"]);
                        }
                    }
                }
            }
            #endregion
            #region Fillers
            else if (bHasDayCodeExt)
            {
                string fillerHrCol, fillerOTHrCol, fillerNDHrCol, fillerOTNDHrCol;
                double dRegAmt, dOTAmt;
                foreach (DataRow drFiller in dtDayCodeFillers.Rows)
                {
                    dRegAmt = 0;
                    dOTAmt = 0;
                    fillerHrCol = string.Format("Ept_Filler{0}_Hr", drFiller["Dcf_FillerSeq"]);
                    fillerOTHrCol = string.Format("Ept_Filler{0}_OTHr", drFiller["Dcf_FillerSeq"]);
                    fillerNDHrCol = string.Format("Ept_Filler{0}_NDHr", drFiller["Dcf_FillerSeq"]);
                    fillerOTNDHrCol = string.Format("Ept_Filler{0}_OTNDHr", drFiller["Dcf_FillerSeq"]);
                    if (DayCode.Equals(drFiller["Dcf_DayCode"].ToString()) && Convert.ToBoolean(drFiller["Dcf_Restday"]) == Restday)
                    {
                        //Initialize Reg Hr and OT Hr
                        drEmployeePayrollTransactionExt[fillerHrCol] = Convert.ToDouble(drEmployeePayrollTransactionExt[fillerHrCol]) - Convert.ToDouble(drEmployeePayrollTransactionExtDetail[fillerHrCol]);
                        drEmployeePayrollTransactionExt[fillerOTHrCol] = Convert.ToDouble(drEmployeePayrollTransactionExt[fillerOTHrCol]) - Convert.ToDouble(drEmployeePayrollTransactionExtDetail[fillerOTHrCol]);

                        if (hrType == HourType.RegHour)
                        {
                            //drEmployeePayrollTransactionExt[fillerHrCol] = Convert.ToDouble(drEmployeePayrollTransactionExt[fillerHrCol]) + Amount;
                            drEmployeePayrollTransactionExtDetail[fillerHrCol] = Convert.ToDouble(drEmployeePayrollTransactionExtDetail[fillerHrCol]) + Amount;
                        }
                        else if (hrType == HourType.OTHour)
                        {
                            //drEmployeePayrollTransactionExt[fillerOTHrCol] = Convert.ToDouble(drEmployeePayrollTransactionExt[fillerOTHrCol]) + Amount;
                            drEmployeePayrollTransactionExtDetail[fillerOTHrCol] = Convert.ToDouble(drEmployeePayrollTransactionExtDetail[fillerOTHrCol]) + Amount;
                        }
                        else if (hrType == HourType.NDHour)
                        {
                            drEmployeePayrollTransactionExt[fillerNDHrCol] = Convert.ToDouble(drEmployeePayrollTransactionExt[fillerNDHrCol]) + Amount;
                            drEmployeePayrollTransactionExtDetail[fillerNDHrCol] = Convert.ToDouble(drEmployeePayrollTransactionExtDetail[fillerNDHrCol]) + Amount;
                        }
                        else if (hrType == HourType.NDOTHour)
                        {
                            drEmployeePayrollTransactionExt[fillerOTNDHrCol] = Convert.ToDouble(drEmployeePayrollTransactionExt[fillerOTNDHrCol]) + Amount;
                            drEmployeePayrollTransactionExtDetail[fillerOTNDHrCol] = Convert.ToDouble(drEmployeePayrollTransactionExtDetail[fillerOTNDHrCol]) + Amount;
                        }

                        //Evaluate if excess 8 hours
                        if (Convert.ToDouble(drEmployeePayrollTransactionExtDetail[fillerHrCol]) + Convert.ToDouble(drEmployeePayrollTransactionExtDetail[fillerOTHrCol]) <= 8)
                        {
                            drEmployeePayrollTransactionExtDetail[fillerHrCol] = Convert.ToDouble(drEmployeePayrollTransactionExtDetail[fillerHrCol]) + Convert.ToDouble(drEmployeePayrollTransactionExtDetail[fillerOTHrCol]);
                            drEmployeePayrollTransactionExtDetail[fillerOTHrCol] = 0;
                            dRegAmt = Convert.ToDouble(drEmployeePayrollTransactionExtDetail[fillerHrCol]);
                            dOTAmt = Convert.ToDouble(drEmployeePayrollTransactionExtDetail[fillerOTHrCol]);
                        }
                        else
                        {
                            drEmployeePayrollTransactionExtDetail[fillerOTHrCol] = (Convert.ToDouble(drEmployeePayrollTransactionExtDetail[fillerHrCol]) + Convert.ToDouble(drEmployeePayrollTransactionExtDetail[fillerOTHrCol])) - 8;
                            drEmployeePayrollTransactionExtDetail[fillerHrCol] = 8;
                            dRegAmt = Convert.ToDouble(drEmployeePayrollTransactionExtDetail[fillerHrCol]);
                            dOTAmt = Convert.ToDouble(drEmployeePayrollTransactionExtDetail[fillerOTHrCol]);
                        }

                        //Update Reg Hr and OT Hr
                        drEmployeePayrollTransactionExt[fillerHrCol] = Convert.ToDouble(drEmployeePayrollTransactionExt[fillerHrCol]) + dRegAmt;
                        drEmployeePayrollTransactionExt[fillerOTHrCol] = Convert.ToDouble(drEmployeePayrollTransactionExt[fillerOTHrCol]) + dOTAmt;
                    }
                }
            }
            #endregion
        }
        #endregion

        #region Timekeeping Utilities
        public int GetOTHoursInMinutes(string strStartOT, string strEndOT, string strStartPunch, string strEndPunch)
        {
            int iStartOT = GetMinsFromHourStr(strStartOT);
            int iEndOT = GetMinsFromHourStr(strEndOT);
            int iStartPunch = GetMinsFromHourStr(strStartPunch);
            int iEndPunch = GetMinsFromHourStr(strEndPunch);
            int iTimeStart, iTimeEnd;

            iTimeStart = (iStartOT > iStartPunch) ? iStartOT : iStartPunch; //Get Max
            iTimeEnd = (iEndOT < iEndPunch) ? iEndOT : iEndPunch; //Get Min

            if (iTimeEnd > iTimeStart)
            {
                return iTimeEnd - iTimeStart;
            }
            else
            {
                return 0;
            }
        }

        public int GetOTHoursInMinutes(int iStartOT, int iEndOT, int iStartPunch, int iEndPunch)
        {
            int iTimeStart, iTimeEnd;

            iTimeStart = (iStartOT > iStartPunch) ? iStartOT : iStartPunch; //Get Max
            iTimeEnd = (iEndOT < iEndPunch) ? iEndOT : iEndPunch; //Get Min

            if (iTimeEnd > iTimeStart)
            {
                return iTimeEnd - iTimeStart;
            }
            else
            {
                return 0;
            }
        }

        public int GetOTHoursInTime(int iStartOT, int iEndOT, int iStartPunch, int iEndPunch, ref int iTimeStart, ref int iTimeEnd)
        {
            iTimeStart = (iStartOT > iStartPunch) ? iStartOT : iStartPunch; //Get Max
            iTimeEnd = (iEndOT < iEndPunch) ? iEndOT : iEndPunch; //Get Min

            if (iTimeEnd > iTimeStart)
            {
                return iTimeEnd - iTimeStart;
            }
            else
            {
                return 0;
            }
        }

        public void GetOTHoursExcludedTime(int iStartOT, int iEndOT, int iStartPunch, int iEndPunch, ref int iTimeStart1, ref int iTimeEnd1, ref int iTimeStart2, ref int iTimeEnd2)
        {
            iTimeStart1 = (iEndOT > iStartPunch) ? iEndOT : iStartPunch; //Get Max
            iTimeEnd1 = (iStartOT > iEndPunch) ? iStartOT : iEndPunch; //Get Max

            iTimeStart2 = (iEndOT < iStartPunch) ? iEndOT : iStartPunch; //Get Min
            iTimeEnd2 = (iStartOT < iEndPunch) ? iStartOT : iEndPunch; //Get Min
        }

        public int GetHourDiffInMinutes(string strTimeOut, string strTimeIn)
        {
            int iTimeIn = GetMinsFromHourStr(strTimeIn);
            int iTimeOut = GetMinsFromHourStr(strTimeOut);

            if (iTimeOut < iTimeIn)
            {
                return 0;
            }
            else
            {
                return iTimeOut - iTimeIn;
            }
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

        public string AddMinutesToHourStr(string baseHour, int minutes)
        {
            int iSumInMins = GetMinsFromHourStr(baseHour) + minutes;
            return GetHourStrFromMins(iSumInMins);
        }

        public string AddMinutesToMinStr(string baseMin, int minutes)
        {
            int iSumInMins = Convert.ToInt32(baseMin) + minutes;
            return Convert.ToString(iSumInMins);
        }

        public int ConvertToGraveyardTime(int iMinutes, bool bIsGraveyard)
        {
            if (bIsGraveyard)
            {
                iMinutes += GRAVEYARD24;
            }
            return iMinutes;
        }
        #endregion

    }
}
