using System;
using System.Collections.Generic;
using System.Data;
using CommonLibrary;
using Payroll.DAL;
using System.Configuration;

namespace Payroll.BLogic
{
    public class NewRetroPayComputationBL : BaseBL
    {
        #region <Global Variables>
        DALHelper dal;
        SystemCycleProcessingBL SystemCycleProcessingBL;
        CommonBL commonBL;

        public string CompanyCode = "";
        public string CentralProfile = "";
        public string MenuCode = "";
        public string LoginUser = "";

        DataTable dtEmpPayrollRetro = null;

        DataTable dtEmpPayroll = null;
        DataTable dtEmpPayrollMisc = null;
        DataTable dtEmpPayrollDtl = null;
        DataTable dtEmpPayrollDtlMisc = null;
        DataTable dtEmpSystemAdj2 = null;
        DataTable dtEmpSystemAdj2Misc = null;
        DataRow drEmpPayroll = null;
        DataRow drEmpPayrollMisc = null;
        DataRow drEmpPayrollDtl = null;
        DataRow drEmpPayrollDtlMisc = null;
        DataRow drEmpSystemAdj2 = null;
        DataRow drEmpSystemAdj2Misc = null;
        DataTable dtPayrollError = null;

        #region Pay Code Rate Variables
        string LTRate               = "N";
        string UTRate               = "N";
        string UPLVRate             = "N";
        string WDABSRate            = "N";
        string LTUTMAXRate          = "N";
        string ABSLEGHOLRate        = "N";
        string ABSSPLHOLRate        = "N";
        string ABSCOMPHOLRate       = "N";
        string ABSPSDRate           = "N";
        string ABSOTHHOLRate        = "N";

        string REGRate              = "N";
        string PDLVRate             = "N";
        string PDLEGHOLRate         = "N";
        string PDSPLHOLRate         = "N";
        string PDCOMPHOLRate        = "N";
        string PDPSDRate            = "N";
        string PDOTHHOLRate         = "N";
        string PDRESTLEGHOLRate     = "N";

        string RESTRate             = "N";
        string LEGHOLRate           = "N";
        string RESTLEGHOLRate       = "N";
        string SPLHOLRate           = "N";
        string RESTSPLHOLRate       = "N";
        string PSDRate              = "N";
        string RESTPSDRate          = "N";
        string COMPHOLRate          = "N";
        string RESTCOMPHOLRate      = "N";

        string Misc1Rate            = "N";
        string Misc2Rate            = "N";
        string Misc3Rate            = "N";
        string Misc4Rate            = "N";
        string Misc5Rate            = "N";
        string Misc6Rate            = "N";
        #endregion


        //Payroll Error Report structure
        struct structRetroErrorReport
        {
            public string strEmployeeId;
            public string strLastName;
            public string strFirstName;
            public string strPayCycle;
            public decimal dAmount;
            public decimal dAmount2;
            public string strRemarks;

            public structRetroErrorReport(string EmployeeId, string LastName, string FirstName, string PayCycle, decimal Amount, decimal Amount2, string Remarks)
            {
                strEmployeeId = EmployeeId;
                strLastName = LastName;
                strPayCycle = PayCycle;
                strFirstName = FirstName;
                dAmount = Amount;
                dAmount2 = Amount2;
                strRemarks = Remarks;

            }
        }

        List<structRetroErrorReport> listPayrollRept = new List<structRetroErrorReport>();

        //Miscellaneous
        string ProcessPayrollPeriod = "";
        string PayrollStart = "";
        string PayrollEnd = "";

        string EmployeeList = string.Empty;

        public decimal MDIVISOR         = 0;
        public decimal HRLYRTEDEC       = 0;
        public string MULTSAL           = "";
        public string MULTSALDM         = "";
        public string MULTSALMD         = "";
        public string NEWHIRE           = "";
        public string PAYSPECIALRATE    = "";
        public string SPECIALRATEFORMULA_SRATE = "";
        public string SPECIALRATEFORMULA_HRATE = "";
        public DataTable PAYCODERATE    = null;
        public DataTable LATEBRCKTQ     = null;
        public int NDBRCKTCNT           = 1;
        public decimal NP1_RATE         = 0;
        public decimal NP2_RATE         = 0;
        public decimal LCSFORCEBAL      = 0;

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

        #region Event handlers for Retro Pay Calculation Process
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

        public NewRetroPayComputationBL()
        {
        }

        public DataTable ComputePayrollRetro(string EmployeeList, string PayrollPeriod, string UserLogin, string companyCode, string centralProfile, string menucode, DALHelper dalHelper)
        {
            this.EmployeeList = EmployeeList;
            return ComputePayrollRetro(true, PayrollPeriod, "", UserLogin, companyCode, centralProfile, menucode, dalHelper);
        }

        public DataTable ComputePayrollRetro(bool ProcessAll, string PayrollPeriod, string EmployeeId, string UserLogin, string companyCode, string centralProfile, string menucode, DALHelper dalHelper)
        {
            #region Variables
            string RetroProcessingErrorMessage = "";
            CompanyCode                 = companyCode;
            CentralProfile              = centralProfile;
            MenuCode                    = menucode;
            #region Employee Variables
            string curEmployeeId        = "";
            string PayrollType          = "";
            string EmploymentStatus     = "";
            string strRegRule              = "";
            decimal SalaryRate          = 0;
            decimal HourlyRate          = 0;
            decimal SpecialSalaryRate   = 0;
            decimal SpecialHourlyRate   = 0;
            string[] AdjustArrPayCycles = null;
            string AdjustPayCycleStart = "";
            string AdjustPayCycleEnd = "";
            object SalaryDate;

            decimal EmpRGRetroAmt       = 0;
            decimal EmpOTRetroAmt       = 0;
            decimal EmpNDRetroAmt       = 0;
            decimal EmpHLRetroAmt       = 0;
            decimal EmpLVRetroAmt       = 0;
            decimal EmpTotalRetroAmt    = 0;
            #endregion


            bool bHasDayCodeExt                 = false;
            DataTable dtDayCodePremiums         = null;
            DataTable dtDayCodePremiumFillers   = null;
            DataRow[] drDayCodePremiumFiller    = null;

            DataTable dtPayrollDtlForProcess    = null;
            DataTable dtPayrollForProcess       = null;
            DataTable dtSystemAdj2ForProcess    = null;
            DataTable dtEmployeeSalary          = null;
            DataTable dtComputedDailyEmployees  = null;
            DataTable dtNewlyHiredEmployees     = null;

            #region Premium Group Variables
            //Regular and Overtime Rates
            decimal Reg = 0;
            decimal RegOT = 0;
            decimal Rest = 0;
            decimal RestOT = 0;
            decimal Hol = 0;
            decimal HolOT = 0;
            decimal SPL = 0;
            decimal SPLOT = 0;
            decimal PSD = 0;
            decimal PSDOT = 0;
            decimal Comp = 0;
            decimal CompOT = 0;
            decimal RestHol = 0;
            decimal RestHolOT = 0;
            decimal RestSPL = 0;
            decimal RestSPLOT = 0;
            decimal RestComp = 0;
            decimal RestCompOT = 0;
            decimal RestPSD = 0;
            decimal RestPSDOT = 0;

            //Regular Night Premium and Overtime Night Premium Rates
            decimal RegND = 0;
            decimal RegOTND = 0;
            decimal RestND = 0;
            decimal RestOTND = 0;
            decimal HolND = 0;
            decimal HolOTND = 0;
            decimal SPLND = 0;
            decimal SPLOTND = 0;
            decimal PSDND = 0;
            decimal PSDOTND = 0;
            decimal CompND = 0;
            decimal CompOTND = 0;
            decimal RestHolND = 0;
            decimal RestHolOTND = 0;
            decimal RestSPLND = 0;
            decimal RestSPLOTND = 0;
            decimal RestCompND = 0;
            decimal RestCompOTND = 0;
            decimal RestPSDND = 0;
            decimal RestPSDOTND = 0;

            //Regular Night Premium and Overtime Night Premium Percentages
            decimal RegNDPercent = 0;
            decimal RegOTNDPercent = 0;
            decimal RestNDPercent = 0;
            decimal RestOTNDPercent = 0;
            decimal HolNDPercent = 0;
            decimal HolOTNDPercent = 0;
            decimal SPLNDPercent = 0;
            decimal SPLOTNDPercent = 0;
            decimal PSDNDPercent = 0;
            decimal PSDOTNDPercent = 0;
            decimal CompNDPercent = 0;
            decimal CompOTNDPercent = 0;
            decimal RestHolNDPercent = 0;
            decimal RestHolOTNDPercent = 0;
            decimal RestSPLNDPercent = 0;
            decimal RestSPLOTNDPercent = 0;
            decimal RestCompNDPercent = 0;
            decimal RestCompOTNDPercent = 0;
            decimal RestPSDNDPercent = 0;
            decimal RestPSDOTNDPercent = 0;
            #endregion

            #region Payroll Calc Variables
            decimal LateAmt = 0;
            decimal UndertimeAmt = 0;
            decimal UnpaidLeaveAmt = 0;
            decimal AbsentLegalHolidayAmt = 0;
            decimal AbsentSpecialHolidayAmt = 0;
            decimal AbsentCompanyHolidayAmt = 0;
            decimal AbsentPlantShutdownAmt = 0;
            decimal AbsentFillerHolidayAmt = 0;
            decimal WholeDayAbsentAmt = 0;
            decimal LateUndertimeMaxAbsentAmt = 0;
            decimal AbsentAmt = 0;

            decimal RegularAmt = 0;
            decimal PaidLeaveAmt = 0;
            decimal PaidLegalHolidayAmt = 0;
            decimal PaidSpecialHolidayAmt = 0;
            decimal PaidCompanyHolidayAmt = 0;
            decimal PaidFillerHolidayAmt = 0;
            decimal PaidPlantShutdownHolidayAmt = 0;
            decimal PaidRestdayLegalHolidayAmt = 0;

            decimal RegularOTAmt = 0;
            decimal RegularNDAmt = 0;
            decimal RegularOTNDAmt = 0;
            decimal RestdayAmt = 0;
            decimal RestdayOTAmt = 0;
            decimal RestdayNDAmt = 0;
            decimal RestdayOTNDAmt = 0;
            decimal LegalHolidayAmt = 0;
            decimal LegalHolidayOTAmt = 0;
            decimal LegalHolidayNDAmt = 0;
            decimal LegalHolidayOTNDAmt = 0;
            decimal SpecialHolidayAmt = 0;
            decimal SpecialHolidayOTAmt = 0;
            decimal SpecialHolidayNDAmt = 0;
            decimal SpecialHolidayOTNDAmt = 0;
            decimal PlantShutdownAmt = 0;
            decimal PlantShutdownOTAmt = 0;
            decimal PlantShutdownNDAmt = 0;
            decimal PlantShutdownOTNDAmt = 0;
            decimal CompanyHolidayAmt = 0;
            decimal CompanyHolidayOTAmt = 0;
            decimal CompanyHolidayNDAmt = 0;
            decimal CompanyHolidayOTNDAmt = 0;
            decimal RestdayLegalHolidayAmt = 0;
            decimal RestdayLegalHolidayOTAmt = 0;
            decimal RestdayLegalHolidayNDAmt = 0;
            decimal RestdayLegalHolidayOTNDAmt = 0;
            decimal RestdaySpecialHolidayAmt = 0;
            decimal RestdaySpecialHolidayOTAmt = 0;
            decimal RestdaySpecialHolidayNDAmt = 0;
            decimal RestdaySpecialHolidayOTNDAmt = 0;
            decimal RestdayCompanyHolidayAmt = 0;
            decimal RestdayCompanyHolidayOTAmt = 0;
            decimal RestdayCompanyHolidayNDAmt = 0;
            decimal RestdayCompanyHolidayOTNDAmt = 0;
            decimal RestdayPlantShutdownAmt = 0;
            decimal RestdayPlantShutdownOTAmt = 0;
            decimal RestdayPlantShutdownNDAmt = 0;
            decimal RestdayPlantShutdownOTNDAmt = 0;

            decimal RGRetroAmt = 0;
            decimal OTRetroAmt = 0;
            decimal NDRetroAmt = 0;
            decimal HLRetroAmt = 0;
            decimal LVRetroAmt = 0;

            #endregion

            #region MultSal Variables
            int WorkingDayCnt = 0;
            int WorkingDayCntUsingNewRate = 0;
            decimal OldDailyRate = 0;
            decimal NewDailyRate = 0;
            decimal OldSalaryRate = 0;
            bool bMonthlyToDailyPayrollType             = false;
            DataRow[] drArrEmpComputedDailyEmployees    = null;
            DataRow[] drArrEmpNewlyHiredEmployees       = null;
            #endregion

            #region Payroll Calc Ext Variables
            string fillerHrCol          = "";
            string fillerOTHrCol        = "";
            string fillerNDHrCol        = "";
            string fillerOTNDHrCol      = "";
            string fillerAmtCol         = "";
            string fillerOTAmtCol       = "";
            string fillerNDAmtCol       = "";
            string fillerOTNDAmtCol     = "";

            decimal Misc1Amt            = 0;
            decimal Misc1OTAmt          = 0;
            decimal Misc1NDAmt          = 0;
            decimal Misc1NDOTAmt        = 0;
            decimal Misc2Amt            = 0;
            decimal Misc2OTAmt          = 0;
            decimal Misc2NDAmt          = 0;
            decimal Misc2NDOTAmt        = 0;
            decimal Misc3Amt            = 0;
            decimal Misc3OTAmt          = 0;
            decimal Misc3NDAmt          = 0;
            decimal Misc3NDOTAmt        = 0;
            decimal Misc4Amt            = 0;
            decimal Misc4OTAmt          = 0;
            decimal Misc4NDAmt          = 0;
            decimal Misc4NDOTAmt        = 0;
            decimal Misc5Amt            = 0;
            decimal Misc5OTAmt          = 0;
            decimal Misc5NDAmt          = 0;
            decimal Misc5NDOTAmt        = 0;
            decimal Misc6Amt            = 0;
            decimal Misc6OTAmt          = 0;
            decimal Misc6NDAmt          = 0;
            decimal Misc6NDOTAmt        = 0;

            #endregion

            #endregion

            try
            {
                #region Initial Setup
                this.dal = dalHelper;
                SystemCycleProcessingBL = new SystemCycleProcessingBL(dal, PayrollPeriod, UserLogin, CompanyCode, CentralProfile);
                commonBL = new CommonBL();
                //-----------------------------
                //Check for Existing Day Codes
                if (commonBL.GetFillerDayCodesCount(CompanyCode, CentralProfile, dal) > 0)
                {
                    bHasDayCodeExt = true;
                    dtDayCodePremiumFillers = commonBL.GetDayCodePremiumFillers(CompanyCode, CentralProfile, dal);
                }
                dtDayCodePremiums = GetDayCodePremiums(CompanyCode, CentralProfile, dal);
                //-----------------------------
                ProcessPayrollPeriod = PayrollPeriod;
                LoginUser = UserLogin;

                DataSet dsPayPeriod = commonBL.GetPayCycleStartendDate(PayrollPeriod, dal);
                if (dsPayPeriod.Tables.Count > 0 && dsPayPeriod.Tables[0].Rows.Count > 0)
                {
                    PayrollStart = dsPayPeriod.Tables[0].Rows[0]["Tps_StartCycle"].ToString();
                    PayrollEnd = dsPayPeriod.Tables[0].Rows[0]["Tps_EndCycle"].ToString();
                }
                #endregion
                //-----------------------------
                //SetParameters();
                InitializePayrollReport();
                //-----------------------------
                #region Create Retro table
                DALHelper dalTemp = new DALHelper();
                dtEmpPayroll = DbRecord.GenerateTable(CommonConstants.TableName.T_EmpPayrollRetro, dalTemp);
                dtEmpPayrollMisc = DbRecord.GenerateTable(CommonConstants.TableName.T_EmpPayrollMiscRetro, dalTemp);
                dtEmpPayrollDtl = DbRecord.GenerateTable(CommonConstants.TableName.T_EmpPayrollDtlRetro, dalTemp);
                dtEmpPayrollDtlMisc = DbRecord.GenerateTable(CommonConstants.TableName.T_EmpPayrollDtlMiscRetro, dalTemp);
                dtEmpSystemAdj2 = DbRecord.GenerateTable(CommonConstants.TableName.T_EmpSystemAdj2Retro, dalTemp);
                dtEmpSystemAdj2Misc = DbRecord.GenerateTable(CommonConstants.TableName.T_EmpSystemAdjMisc2Retro, dalTemp);
                //Create Retro Error Table
                dtPayrollError = DbRecord.GenerateTable(CommonConstants.TableName.T_EmpProcessCheck, dalTemp);
                #endregion
                //-----------------------------
                StatusHandler(this, new StatusEventArgs("Cleaning Retro Pay Tables", false));
                ClearPayrollTables(ProcessAll, EmployeeId);
                StatusHandler(this, new StatusEventArgs("Cleaning Retro Pay Tables", true));
                //-----------------------------
                StatusHandler(this, new StatusEventArgs("Extracting Payroll Calculation Records", false));
                dtEmpPayrollRetro = GetAllEmployeeRetroPayForProcess(ProcessAll, EmployeeId, EmployeeList, dal);
                StatusHandler(this, new StatusEventArgs("Extracting Payroll Calculation Records", true));
                //-------------------------------- START MAIN PROCESS ----------------------------------
                if (dtEmpPayrollRetro.Rows.Count > 0)
                {
                    for (int i = 0; i < dtEmpPayrollRetro.Rows.Count; i++)
                    {
                        try
                        {
                            #region Get Payroll data
                            curEmployeeId       = dtEmpPayrollRetro.Rows[i]["Ter_IDNo"].ToString();
                            AdjustArrPayCycles  = dtEmpPayrollRetro.Rows[i]["Ter_AdjustPayCycles"].ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            SalaryDate          = dtEmpPayrollRetro.Rows[i]["Ter_SalaryDate"];

                            SalaryRate          = 0;
                            SalaryRate          = Convert.ToDecimal(dtEmpPayrollRetro.Rows[i]["Ter_SalaryRate"]);
                            PayrollType         = dtEmpPayrollRetro.Rows[i]["Ter_PayrollType"].ToString();

                            EmploymentStatus    = dtEmpPayrollRetro.Rows[i]["Mem_EmploymentStatusCode"].ToString();
                            MDIVISOR            = Convert.ToDecimal(GetScalarValue("Mpd_ParamValue", "M_PolicyDtl", string.Format("Mpd_PolicyCode = 'MDIVISOR' AND Mpd_SubCode = '{0}' AND Mpd_CompanyCode = '{1}' AND Mpd_RecordStatus = 'A'", EmploymentStatus, CompanyCode), dalHelper));

                            EmpRGRetroAmt       = 0;
                            EmpOTRetroAmt       = 0;
                            EmpNDRetroAmt       = 0;
                            EmpHLRetroAmt       = 0;
                            EmpLVRetroAmt       = 0;
                            EmpTotalRetroAmt    = 0;

                            #region Get Hourly Rate
                            HourlyRate          = 0;
                            HourlyRate          = GetHourlyRate(SalaryRate, PayrollType, MDIVISOR);
                            #endregion

                            #region Get Special Rate
                            if (Convert.ToBoolean(PAYSPECIALRATE))
                            {
                                SpecialSalaryRate = 0;
                                SpecialHourlyRate = 0;

                                int idxx = 0;
                                ParameterInfo[] paramDtl = new ParameterInfo[8];
                                paramDtl[idxx++] = new ParameterInfo("@IDNUMBER", curEmployeeId);
                                paramDtl[idxx++] = new ParameterInfo("@PAYROLLTYPE", PayrollType);
                                paramDtl[idxx++] = new ParameterInfo("@MDIVISOR", MDIVISOR);
                                paramDtl[idxx++] = new ParameterInfo("@HDIVISOR", CommonBL.HOURSINDAY);
                                paramDtl[idxx++] = new ParameterInfo("@PAYCYCLE", PayrollPeriod);
                                paramDtl[idxx++] = new ParameterInfo("@STARTCYCLE", PayrollStart);
                                paramDtl[idxx++] = new ParameterInfo("@ENDCYCLE", PayrollEnd);
                                paramDtl[idxx++] = new ParameterInfo("@COMPANYCODE", CompanyCode);

                                if (!SPECIALRATEFORMULA_SRATE.Equals(""))
                                    SpecialSalaryRate = GetFormulaQueryDecimalValue(SPECIALRATEFORMULA_SRATE.Replace("@CENTRALDB", CentralProfile), paramDtl);

                                if (!SPECIALRATEFORMULA_HRATE.Equals(""))
                                {
                                    SpecialHourlyRate = GetFormulaQueryDecimalValue(SPECIALRATEFORMULA_HRATE.Replace("@CENTRALDB", CentralProfile), paramDtl);

                                    if (HRLYRTEDEC > 0)
                                        SpecialHourlyRate = Math.Round(SpecialHourlyRate, Convert.ToInt32(HRLYRTEDEC), MidpointRounding.AwayFromZero);
                                }
                            }
                            #endregion

                            #endregion

                            foreach (string strPayCycle in AdjustArrPayCycles)
                            {
                                #region Get Pay Cycle Details
                                AdjustPayCycleStart = "";
                                AdjustPayCycleEnd = "";
                                DataTable dtPayPeriod = GetPayPeriodCycle(strPayCycle);
                                if (dtPayPeriod.Rows.Count > 0)
                                {
                                    AdjustPayCycleStart = dtPayPeriod.Rows[0]["Tps_StartCycle"].ToString();
                                    AdjustPayCycleEnd = dtPayPeriod.Rows[0]["Tps_EndCycle"].ToString();
                                }
                                else
                                    RetroProcessingErrorMessage += strPayCycle + " - Pay Cycle not found.\n";
                                #endregion

                                //Check for errors
                                if (RetroProcessingErrorMessage != "")
                                    throw new Exception(RetroProcessingErrorMessage);

                                dtPayrollForProcess         = GetAllEmployeePayrollForProcess(curEmployeeId, strPayCycle);
                                dtComputedDailyEmployees    = GetComputedDailyinCurrentPayPeriod(AdjustPayCycleStart, AdjustPayCycleEnd);
                                dtNewlyHiredEmployees       = GetNewlyHiredinCurrentPayPeriod(AdjustPayCycleStart, AdjustPayCycleEnd);
                                dtEmployeeSalary            = GetEmployeeSalaryinPayPeriod(curEmployeeId, AdjustPayCycleStart, AdjustPayCycleEnd);
                                for (int idh = 0; idh < dtPayrollForProcess.Rows.Count; idh++)
                                {
                                    drEmpPayroll = dtEmpPayroll.NewRow();
                                    if (bHasDayCodeExt)
                                        drEmpPayrollMisc = dtEmpPayrollMisc.NewRow();

                                    #region Initialize Payroll Calc Row
                                    drEmpPayroll["Tpy_IDNo"]                        = curEmployeeId;
                                    drEmpPayroll["Tpy_RetroPayCycle"]               = PayrollPeriod;
                                    drEmpPayroll["Tpy_PayCycle"]                    = strPayCycle; //Affected Pay Cycles
                                    drEmpPayroll["Tpy_SalaryRate"]                  = SalaryRate;
                                    drEmpPayroll["Tpy_HourRate"]                    = HourlyRate;
                                    drEmpPayroll["Tpy_PayrollType"]                 = PayrollType;
                                    drEmpPayroll["Tpy_SpecialRate"]                 = SpecialSalaryRate; //FORMULA
                                    drEmpPayroll["Tpy_SpecialHourRate"]             = SpecialHourlyRate;
                                    decimal brLateHr = GetLateDeductionFromBracket(Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_LTHr"]));
                                    if (brLateHr > 0)
                                        drEmpPayroll["Tpy_LTHr"]                    = brLateHr;
                                    else
                                        drEmpPayroll["Tpy_LTHr"]                    = dtPayrollForProcess.Rows[idh]["Tpy_LTHr"];

                                    decimal brUndertimeHr = GetLateDeductionFromBracket(Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_UTHr"]));
                                    if (brUndertimeHr > 0)
                                        drEmpPayroll["Tpy_UTHr"]                    = brUndertimeHr;
                                    else
                                        drEmpPayroll["Tpy_UTHr"]                    = dtPayrollForProcess.Rows[idh]["Tpy_UTHr"];

                                    drEmpPayroll["Tpy_UPLVHr"]                      = dtPayrollForProcess.Rows[idh]["Tpy_UPLVHr"];
                                    drEmpPayroll["Tpy_ABSLEGHOLHr"]                 = dtPayrollForProcess.Rows[idh]["Tpy_ABSLEGHOLHr"];
                                    drEmpPayroll["Tpy_ABSSPLHOLHr"]                 = dtPayrollForProcess.Rows[idh]["Tpy_ABSSPLHOLHr"];
                                    drEmpPayroll["Tpy_ABSCOMPHOLHr"]                = dtPayrollForProcess.Rows[idh]["Tpy_ABSCOMPHOLHr"];
                                    drEmpPayroll["Tpy_ABSPSDHr"]                    = dtPayrollForProcess.Rows[idh]["Tpy_ABSPSDHr"];
                                    drEmpPayroll["Tpy_ABSOTHHOLHr"]                 = dtPayrollForProcess.Rows[idh]["Tpy_ABSOTHHOLHr"];
                                    drEmpPayroll["Tpy_WDABSHr"]                     = dtPayrollForProcess.Rows[idh]["Tpy_WDABSHr"];
                                    drEmpPayroll["Tpy_LTUTMaxHr"]                   = dtPayrollForProcess.Rows[idh]["Tpy_LTUTMaxHr"];
                                    //Total Absent Hour
                                    if ((brLateHr + brUndertimeHr) > 0)
                                        drEmpPayroll["Tpy_ABSHr"]                   = Convert.ToDecimal(drEmpPayroll["Tpy_LTHr"])
                                                                                                                    + Convert.ToDecimal(drEmpPayroll["Tpy_UTHr"])
                                                                                                                    + Convert.ToDecimal(drEmpPayroll["Tpy_UPLVHr"])
                                                                                                                    + Convert.ToDecimal(drEmpPayroll["Tpy_WDABSHr"])
                                                                                                                    + Convert.ToDecimal(drEmpPayroll["Tpy_LTUTMaxHr"]);

                                    drEmpPayroll["Tpy_ABSHr"]                       = Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_ABSHr"])
                                                                                                                    + Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_ABSLEGHOLHr"])
                                                                                                                    + Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_ABSSPLHOLHr"])
                                                                                                                    + Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_ABSCOMPHOLHr"])
                                                                                                                    + Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_ABSPSDHr"])
                                                                                                                    + Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_ABSOTHHOLHr"]);

                                    drEmpPayroll["Tpy_REGHr"]                       = dtPayrollForProcess.Rows[idh]["Tpy_REGHr"];
                                    drEmpPayroll["Tpy_PDLVHr"]                      = dtPayrollForProcess.Rows[idh]["Tpy_PDLVHr"];
                                    drEmpPayroll["Tpy_PDLEGHOLHr"]                  = dtPayrollForProcess.Rows[idh]["Tpy_PDLEGHOLHr"];
                                    drEmpPayroll["Tpy_PDSPLHOLHr"]                  = dtPayrollForProcess.Rows[idh]["Tpy_PDSPLHOLHr"];
                                    drEmpPayroll["Tpy_PDCOMPHOLHr"]                 = dtPayrollForProcess.Rows[idh]["Tpy_PDCOMPHOLHr"];
                                    drEmpPayroll["Tpy_PDPSDHr"]                     = dtPayrollForProcess.Rows[idh]["Tpy_PDPSDHr"];
                                    drEmpPayroll["Tpy_PDOTHHOLHr"]                  = dtPayrollForProcess.Rows[idh]["Tpy_PDOTHHOLHr"];
                                    drEmpPayroll["Tpy_PDRESTLEGHOLHr"]              = dtPayrollForProcess.Rows[idh]["Tpy_PDRESTLEGHOLHr"];

                                    drEmpPayroll["Tpy_REGOTHr"]                     = dtPayrollForProcess.Rows[idh]["Tpy_REGOTHr"];
                                    drEmpPayroll["Tpy_REGNDHr"]                     = dtPayrollForProcess.Rows[idh]["Tpy_REGNDHr"];
                                    drEmpPayroll["Tpy_REGNDOTHr"]                   = dtPayrollForProcess.Rows[idh]["Tpy_REGNDOTHr"];
                                    drEmpPayroll["Tpy_RESTHr"]                      = dtPayrollForProcess.Rows[idh]["Tpy_RESTHr"];
                                    drEmpPayroll["Tpy_RESTOTHr"]                    = dtPayrollForProcess.Rows[idh]["Tpy_RESTOTHr"];
                                    drEmpPayroll["Tpy_RESTNDHr"]                    = dtPayrollForProcess.Rows[idh]["Tpy_RESTNDHr"];
                                    drEmpPayroll["Tpy_RESTNDOTHr"]                  = dtPayrollForProcess.Rows[idh]["Tpy_RESTNDOTHr"];
                                    drEmpPayroll["Tpy_LEGHOLHr"]                    = dtPayrollForProcess.Rows[idh]["Tpy_LEGHOLHr"];
                                    drEmpPayroll["Tpy_LEGHOLOTHr"]                  = dtPayrollForProcess.Rows[idh]["Tpy_LEGHOLOTHr"];
                                    drEmpPayroll["Tpy_LEGHOLNDHr"]                  = dtPayrollForProcess.Rows[idh]["Tpy_LEGHOLNDHr"];
                                    drEmpPayroll["Tpy_LEGHOLNDOTHr"]                = dtPayrollForProcess.Rows[idh]["Tpy_LEGHOLNDOTHr"];
                                    drEmpPayroll["Tpy_SPLHOLHr"]                    = dtPayrollForProcess.Rows[idh]["Tpy_SPLHOLHr"];
                                    drEmpPayroll["Tpy_SPLHOLOTHr"]                  = dtPayrollForProcess.Rows[idh]["Tpy_SPLHOLOTHr"];
                                    drEmpPayroll["Tpy_SPLHOLNDHr"]                  = dtPayrollForProcess.Rows[idh]["Tpy_SPLHOLNDHr"];
                                    drEmpPayroll["Tpy_SPLHOLNDOTHr"]                = dtPayrollForProcess.Rows[idh]["Tpy_SPLHOLNDOTHr"];
                                    drEmpPayroll["Tpy_PSDHr"]                       = dtPayrollForProcess.Rows[idh]["Tpy_PSDHr"];
                                    drEmpPayroll["Tpy_PSDOTHr"]                     = dtPayrollForProcess.Rows[idh]["Tpy_PSDOTHr"];
                                    drEmpPayroll["Tpy_PSDNDHr"]                     = dtPayrollForProcess.Rows[idh]["Tpy_PSDNDHr"];
                                    drEmpPayroll["Tpy_PSDNDOTHr"]                   = dtPayrollForProcess.Rows[idh]["Tpy_PSDNDOTHr"];
                                    drEmpPayroll["Tpy_COMPHOLHr"]                   = dtPayrollForProcess.Rows[idh]["Tpy_COMPHOLHr"];
                                    drEmpPayroll["Tpy_COMPHOLOTHr"]                 = dtPayrollForProcess.Rows[idh]["Tpy_COMPHOLOTHr"];
                                    drEmpPayroll["Tpy_COMPHOLNDHr"]                 = dtPayrollForProcess.Rows[idh]["Tpy_COMPHOLNDHr"];
                                    drEmpPayroll["Tpy_COMPHOLNDOTHr"]               = dtPayrollForProcess.Rows[idh]["Tpy_COMPHOLNDOTHr"];
                                    drEmpPayroll["Tpy_RESTLEGHOLHr"]                = dtPayrollForProcess.Rows[idh]["Tpy_RESTLEGHOLHr"];
                                    drEmpPayroll["Tpy_RESTLEGHOLOTHr"]              = dtPayrollForProcess.Rows[idh]["Tpy_RESTLEGHOLOTHr"];
                                    drEmpPayroll["Tpy_RESTLEGHOLNDHr"]              = dtPayrollForProcess.Rows[idh]["Tpy_RESTLEGHOLNDHr"];
                                    drEmpPayroll["Tpy_RESTLEGHOLNDOTHr"]            = dtPayrollForProcess.Rows[idh]["Tpy_RESTLEGHOLNDOTHr"];
                                    drEmpPayroll["Tpy_RESTSPLHOLHr"]                = dtPayrollForProcess.Rows[idh]["Tpy_RESTSPLHOLHr"];
                                    drEmpPayroll["Tpy_RESTSPLHOLOTHr"]              = dtPayrollForProcess.Rows[idh]["Tpy_RESTSPLHOLOTHr"];
                                    drEmpPayroll["Tpy_RESTSPLHOLNDHr"]              = dtPayrollForProcess.Rows[idh]["Tpy_RESTSPLHOLNDHr"];
                                    drEmpPayroll["Tpy_RESTSPLHOLNDOTHr"]            = dtPayrollForProcess.Rows[idh]["Tpy_RESTSPLHOLNDOTHr"];
                                    drEmpPayroll["Tpy_RESTCOMPHOLHr"]               = dtPayrollForProcess.Rows[idh]["Tpy_RESTCOMPHOLHr"];
                                    drEmpPayroll["Tpy_RESTCOMPHOLOTHr"]             = dtPayrollForProcess.Rows[idh]["Tpy_RESTCOMPHOLOTHr"];
                                    drEmpPayroll["Tpy_RESTCOMPHOLNDHr"]             = dtPayrollForProcess.Rows[idh]["Tpy_RESTCOMPHOLNDHr"];
                                    drEmpPayroll["Tpy_RESTCOMPHOLNDOTHr"]           = dtPayrollForProcess.Rows[idh]["Tpy_RESTCOMPHOLNDOTHr"];
                                    drEmpPayroll["Tpy_RESTPSDHr"]                   = dtPayrollForProcess.Rows[idh]["Tpy_RESTPSDHr"];
                                    drEmpPayroll["Tpy_RESTPSDOTHr"]                 = dtPayrollForProcess.Rows[idh]["Tpy_RESTPSDOTHr"];
                                    drEmpPayroll["Tpy_RESTPSDNDHr"]                 = dtPayrollForProcess.Rows[idh]["Tpy_RESTPSDNDHr"];
                                    drEmpPayroll["Tpy_RESTPSDNDOTHr"]               = dtPayrollForProcess.Rows[idh]["Tpy_RESTPSDNDOTHr"];
                                    //Total Regular Hour
                                    drEmpPayroll["Tpy_TotalREGHr"]                  = Convert.ToDouble(dtPayrollForProcess.Rows[idh]["Tpy_REGHr"])
                                                                                                                    + Convert.ToDouble(dtPayrollForProcess.Rows[idh]["Tpy_PDLVHr"])
                                                                                                                    + Convert.ToDouble(dtPayrollForProcess.Rows[idh]["Tpy_PDLEGHOLHr"])
                                                                                                                    + Convert.ToDouble(dtPayrollForProcess.Rows[idh]["Tpy_PDSPLHOLHr"])
                                                                                                                    + Convert.ToDouble(dtPayrollForProcess.Rows[idh]["Tpy_PDCOMPHOLHr"])
                                                                                                                    + Convert.ToDouble(dtPayrollForProcess.Rows[idh]["Tpy_PDPSDHr"])
                                                                                                                    + Convert.ToDouble(dtPayrollForProcess.Rows[idh]["Tpy_PDOTHHOLHr"])
                                                                                                                    + Convert.ToDouble(dtPayrollForProcess.Rows[idh]["Tpy_PDRESTLEGHOLHr"]);

                                    drEmpPayroll["Tpy_LTAmt"]                       = 0;
                                    drEmpPayroll["Tpy_LTVarAmt"]                    = 0;
                                    drEmpPayroll["Tpy_UTAmt"]                       = 0;
                                    drEmpPayroll["Tpy_UTVarAmt"]                    = 0;
                                    drEmpPayroll["Tpy_UPLVAmt"]                     = 0;
                                    drEmpPayroll["Tpy_UPLVVarAmt"]                  = 0;
                                    drEmpPayroll["Tpy_ABSLEGHOLAmt"]                = 0;
                                    drEmpPayroll["Tpy_ABSLEGHOLVarAmt"]             = 0;
                                    drEmpPayroll["Tpy_ABSSPLHOLAmt"]                = 0;
                                    drEmpPayroll["Tpy_ABSSPLHOLVarAmt"]             = 0;
                                    drEmpPayroll["Tpy_ABSCOMPHOLAmt"]               = 0;
                                    drEmpPayroll["Tpy_ABSCOMPHOLVarAmt"]            = 0;
                                    drEmpPayroll["Tpy_ABSPSDAmt"]                   = 0;
                                    drEmpPayroll["Tpy_ABSPSDVarAmt"]                = 0;
                                    drEmpPayroll["Tpy_ABSOTHHOLAmt"]                = 0;
                                    drEmpPayroll["Tpy_ABSOTHHOLVarAmt"]             = 0;
                                    drEmpPayroll["Tpy_WDABSAmt"]                    = 0;
                                    drEmpPayroll["Tpy_WDABSVarAmt"]                 = 0;
                                    drEmpPayroll["Tpy_LTUTMaxAmt"]                  = 0;
                                    drEmpPayroll["Tpy_LTUTMaxVarAmt"]               = 0;
                                    drEmpPayroll["Tpy_ABSAmt"]                      = 0;
                                    drEmpPayroll["Tpy_ABSVarAmt"]                   = 0;
                                    drEmpPayroll["Tpy_REGAmt"]                      = 0;
                                    drEmpPayroll["Tpy_REGVarAmt"]                   = 0;
                                    drEmpPayroll["Tpy_PDLVAmt"]                     = 0;
                                    drEmpPayroll["Tpy_PDLVVarAmt"]                  = 0;
                                    drEmpPayroll["Tpy_PDLEGHOLAmt"]                 = 0;
                                    drEmpPayroll["Tpy_PDLEGHOLVarAmt"]              = 0;
                                    drEmpPayroll["Tpy_PDSPLHOLAmt"]                 = 0;
                                    drEmpPayroll["Tpy_PDSPLHOLVarAmt"]              = 0;
                                    drEmpPayroll["Tpy_PDCOMPHOLAmt"]                = 0;
                                    drEmpPayroll["Tpy_PDCOMPHOLVarAmt"]             = 0;
                                    drEmpPayroll["Tpy_PDPSDAmt"]                    = 0;
                                    drEmpPayroll["Tpy_PDPSDVarAmt"]                 = 0;
                                    drEmpPayroll["Tpy_PDOTHHOLAmt"]                 = 0;
                                    drEmpPayroll["Tpy_PDOTHHOLVarAmt"]              = 0;
                                    drEmpPayroll["Tpy_PDRESTLEGHOLAmt"]             = 0;
                                    drEmpPayroll["Tpy_PDRESTLEGHOLVarAmt"]          = 0;
                                    drEmpPayroll["Tpy_TotalREGAmt"]                 = 0;
                                    drEmpPayroll["Tpy_REGOTAmt"]                    = 0;
                                    drEmpPayroll["Tpy_REGOTVarAmt"]                 = 0;
                                    drEmpPayroll["Tpy_REGNDAmt"]                    = 0;
                                    drEmpPayroll["Tpy_REGNDVarAmt"]                 = 0;
                                    drEmpPayroll["Tpy_REGNDOTAmt"]                  = 0;
                                    drEmpPayroll["Tpy_REGNDOTVarAmt"]               = 0;
                                    drEmpPayroll["Tpy_RESTAmt"]                     = 0;
                                    drEmpPayroll["Tpy_RESTVarAmt"]                  = 0;
                                    drEmpPayroll["Tpy_RESTOTAmt"]                   = 0;
                                    drEmpPayroll["Tpy_RESTOTVarAmt"]                = 0;
                                    drEmpPayroll["Tpy_RESTNDAmt"]                   = 0;
                                    drEmpPayroll["Tpy_RESTNDVarAmt"]                = 0;
                                    drEmpPayroll["Tpy_RESTNDOTAmt"]                 = 0;
                                    drEmpPayroll["Tpy_RESTNDOTVarAmt"]              = 0;
                                    drEmpPayroll["Tpy_LEGHOLAmt"]                   = 0;
                                    drEmpPayroll["Tpy_LEGHOLVarAmt"]                = 0;
                                    drEmpPayroll["Tpy_LEGHOLOTAmt"]                 = 0;
                                    drEmpPayroll["Tpy_LEGHOLOTVarAmt"]              = 0;
                                    drEmpPayroll["Tpy_LEGHOLNDAmt"]                 = 0;
                                    drEmpPayroll["Tpy_LEGHOLNDVarAmt"]              = 0;
                                    drEmpPayroll["Tpy_LEGHOLNDOTAmt"]               = 0;
                                    drEmpPayroll["Tpy_LEGHOLNDOTVarAmt"]            = 0;
                                    drEmpPayroll["Tpy_SPLHOLAmt"]                   = 0;
                                    drEmpPayroll["Tpy_SPLHOLVarAmt"]                = 0;
                                    drEmpPayroll["Tpy_SPLHOLOTAmt"]                 = 0;
                                    drEmpPayroll["Tpy_SPLHOLOTVarAmt"]              = 0;
                                    drEmpPayroll["Tpy_SPLHOLNDAmt"]                 = 0;
                                    drEmpPayroll["Tpy_SPLHOLNDVarAmt"]              = 0;
                                    drEmpPayroll["Tpy_SPLHOLNDOTAmt"]               = 0;
                                    drEmpPayroll["Tpy_SPLHOLNDOTVarAmt"]            = 0;
                                    drEmpPayroll["Tpy_PSDAmt"]                      = 0;
                                    drEmpPayroll["Tpy_PSDVarAmt"]                   = 0;
                                    drEmpPayroll["Tpy_PSDOTAmt"]                    = 0;
                                    drEmpPayroll["Tpy_PSDOTVarAmt"]                 = 0;
                                    drEmpPayroll["Tpy_PSDNDAmt"]                    = 0;
                                    drEmpPayroll["Tpy_PSDNDVarAmt"]                 = 0;
                                    drEmpPayroll["Tpy_PSDNDOTAmt"]                  = 0;
                                    drEmpPayroll["Tpy_PSDNDOTVarAmt"]               = 0;
                                    drEmpPayroll["Tpy_COMPHOLAmt"]                  = 0;
                                    drEmpPayroll["Tpy_COMPHOLVarAmt"]               = 0;
                                    drEmpPayroll["Tpy_COMPHOLOTAmt"]                = 0;
                                    drEmpPayroll["Tpy_COMPHOLOTVarAmt"]             = 0;
                                    drEmpPayroll["Tpy_COMPHOLNDAmt"]                = 0;
                                    drEmpPayroll["Tpy_COMPHOLNDVarAmt"]             = 0;
                                    drEmpPayroll["Tpy_COMPHOLNDOTAmt"]              = 0;
                                    drEmpPayroll["Tpy_COMPHOLNDOTVarAmt"]           = 0;
                                    drEmpPayroll["Tpy_RESTLEGHOLAmt"]               = 0;
                                    drEmpPayroll["Tpy_RESTLEGHOLVarAmt"]            = 0;
                                    drEmpPayroll["Tpy_RESTLEGHOLOTAmt"]             = 0;
                                    drEmpPayroll["Tpy_RESTLEGHOLOTVarAmt"]          = 0;
                                    drEmpPayroll["Tpy_RESTLEGHOLNDAmt"]             = 0;
                                    drEmpPayroll["Tpy_RESTLEGHOLNDVarAmt"]          = 0;
                                    drEmpPayroll["Tpy_RESTLEGHOLNDOTAmt"]           = 0;
                                    drEmpPayroll["Tpy_RESTLEGHOLNDOTVarAmt"]        = 0;
                                    drEmpPayroll["Tpy_RESTSPLHOLAmt"]               = 0;
                                    drEmpPayroll["Tpy_RESTSPLHOLVarAmt"]            = 0;
                                    drEmpPayroll["Tpy_RESTSPLHOLOTAmt"]             = 0;
                                    drEmpPayroll["Tpy_RESTSPLHOLOTVarAmt"]          = 0;
                                    drEmpPayroll["Tpy_RESTSPLHOLNDAmt"]             = 0;
                                    drEmpPayroll["Tpy_RESTSPLHOLNDVarAmt"]          = 0;
                                    drEmpPayroll["Tpy_RESTSPLHOLNDOTAmt"]           = 0;
                                    drEmpPayroll["Tpy_RESTSPLHOLNDOTVarAmt"]        = 0;
                                    drEmpPayroll["Tpy_RESTCOMPHOLAmt"]              = 0;
                                    drEmpPayroll["Tpy_RESTCOMPHOLVarAmt"]           = 0;
                                    drEmpPayroll["Tpy_RESTCOMPHOLOTAmt"]            = 0;
                                    drEmpPayroll["Tpy_RESTCOMPHOLOTVarAmt"]         = 0;
                                    drEmpPayroll["Tpy_RESTCOMPHOLNDAmt"]            = 0;
                                    drEmpPayroll["Tpy_RESTCOMPHOLNDVarAmt"]         = 0;
                                    drEmpPayroll["Tpy_RESTCOMPHOLNDOTAmt"]          = 0;
                                    drEmpPayroll["Tpy_RESTCOMPHOLNDOTVarAmt"]       = 0;
                                    drEmpPayroll["Tpy_RESTPSDAmt"]                  = 0;
                                    drEmpPayroll["Tpy_RESTPSDVarAmt"]               = 0;
                                    drEmpPayroll["Tpy_RESTPSDOTAmt"]                = 0;
                                    drEmpPayroll["Tpy_RESTPSDOTVarAmt"]             = 0;
                                    drEmpPayroll["Tpy_RESTPSDNDAmt"]                = 0;
                                    drEmpPayroll["Tpy_RESTPSDNDVarAmt"]             = 0;
                                    drEmpPayroll["Tpy_RESTPSDNDOTAmt"]              = 0;
                                    drEmpPayroll["Tpy_RESTPSDNDOTVarAmt"]           = 0;
                                    drEmpPayroll["Tpy_TotalOTNDAmt"]                = 0;

                                    drEmpPayroll["Tpy_DayCountOldSalaryRate"]       = 0;
                                    drEmpPayroll["Tpy_IsMultipleSalary"]            = false;
                                    drEmpPayroll["Tpy_RegRule"]                     = "";

                                    drEmpPayroll["Tpy_RGRetroAmt"]                  = 0;
                                    drEmpPayroll["Tpy_OTRetroAmt"]                  = 0;
                                    drEmpPayroll["Tpy_NDRetroAmt"]                  = 0;
                                    drEmpPayroll["Tpy_HLRetroAmt"]                  = 0;
                                    drEmpPayroll["Tpy_LVRetroAmt"]                  = 0;
                                    drEmpPayroll["Tpy_TotalRetroAmt"]               = 0;
                                    drEmpPayroll["Tpy_CostcenterCode"]              = dtPayrollForProcess.Rows[idh]["Mem_CostcenterCode"];
                                    drEmpPayroll["Tpy_EmploymentStatus"]            = dtPayrollForProcess.Rows[idh]["Mem_EmploymentStatusCode"];
                                    drEmpPayroll["Tpy_PayrollGroup"]                = dtPayrollForProcess.Rows[idh]["Mem_PayrollGroup"];
                                    drEmpPayroll["Usr_login"]                       = LoginUser;
                                    drEmpPayroll["Ludatetime"]                      = DateTime.Now;

                                    #endregion

                                    #region Initialize Payroll Calc Ext Row
                                    if (bHasDayCodeExt)
                                    {
                                        drEmpPayrollMisc["Tpm_IDNo"]                = curEmployeeId;
                                        drEmpPayrollMisc["Tpm_RetroPayCycle"]       = ProcessPayrollPeriod;
                                        drEmpPayrollMisc["Tpm_PayCycle"]            = strPayCycle;
                                        drEmpPayrollMisc["Tpm_Misc1Hr"]             = dtPayrollForProcess.Rows[idh]["Tpm_Misc1Hr"];
                                        drEmpPayrollMisc["Tpm_Misc1OTHr"]           = dtPayrollForProcess.Rows[idh]["Tpm_Misc1OTHr"];
                                        drEmpPayrollMisc["Tpm_Misc1NDHr"]           = dtPayrollForProcess.Rows[idh]["Tpm_Misc1NDHr"];
                                        drEmpPayrollMisc["Tpm_Misc1NDOTHr"]         = dtPayrollForProcess.Rows[idh]["Tpm_Misc1NDOTHr"];
                                        drEmpPayrollMisc["Tpm_Misc2Hr"]             = dtPayrollForProcess.Rows[idh]["Tpm_Misc2Hr"];
                                        drEmpPayrollMisc["Tpm_Misc2OTHr"]           = dtPayrollForProcess.Rows[idh]["Tpm_Misc2OTHr"];
                                        drEmpPayrollMisc["Tpm_Misc2NDHr"]           = dtPayrollForProcess.Rows[idh]["Tpm_Misc2NDHr"];
                                        drEmpPayrollMisc["Tpm_Misc2NDOTHr"]         = dtPayrollForProcess.Rows[idh]["Tpm_Misc2NDOTHr"];
                                        drEmpPayrollMisc["Tpm_Misc3Hr"]             = dtPayrollForProcess.Rows[idh]["Tpm_Misc3Hr"];
                                        drEmpPayrollMisc["Tpm_Misc3OTHr"]           = dtPayrollForProcess.Rows[idh]["Tpm_Misc3OTHr"];
                                        drEmpPayrollMisc["Tpm_Misc3NDHr"]           = dtPayrollForProcess.Rows[idh]["Tpm_Misc3NDHr"];
                                        drEmpPayrollMisc["Tpm_Misc3NDOTHr"]         = dtPayrollForProcess.Rows[idh]["Tpm_Misc3NDOTHr"];
                                        drEmpPayrollMisc["Tpm_Misc4Hr"]             = dtPayrollForProcess.Rows[idh]["Tpm_Misc4Hr"];
                                        drEmpPayrollMisc["Tpm_Misc4OTHr"]           = dtPayrollForProcess.Rows[idh]["Tpm_Misc4OTHr"];
                                        drEmpPayrollMisc["Tpm_Misc4NDHr"]           = dtPayrollForProcess.Rows[idh]["Tpm_Misc4NDHr"];
                                        drEmpPayrollMisc["Tpm_Misc4NDOTHr"]         = dtPayrollForProcess.Rows[idh]["Tpm_Misc4NDOTHr"];
                                        drEmpPayrollMisc["Tpm_Misc5Hr"]             = dtPayrollForProcess.Rows[idh]["Tpm_Misc5Hr"];
                                        drEmpPayrollMisc["Tpm_Misc5OTHr"]           = dtPayrollForProcess.Rows[idh]["Tpm_Misc5OTHr"];
                                        drEmpPayrollMisc["Tpm_Misc5NDHr"]           = dtPayrollForProcess.Rows[idh]["Tpm_Misc5NDHr"];
                                        drEmpPayrollMisc["Tpm_Misc5NDOTHr"]         = dtPayrollForProcess.Rows[idh]["Tpm_Misc5NDOTHr"];
                                        drEmpPayrollMisc["Tpm_Misc6Hr"]             = dtPayrollForProcess.Rows[idh]["Tpm_Misc6Hr"];
                                        drEmpPayrollMisc["Tpm_Misc6OTHr"]           = dtPayrollForProcess.Rows[idh]["Tpm_Misc6OTHr"];
                                        drEmpPayrollMisc["Tpm_Misc6NDHr"]           = dtPayrollForProcess.Rows[idh]["Tpm_Misc6NDHr"];
                                        drEmpPayrollMisc["Tpm_Misc6NDOTHr"]         = dtPayrollForProcess.Rows[idh]["Tpm_Misc6NDOTHr"];
                                        drEmpPayrollMisc["Tpm_Misc1Amt"]            = 0;
                                        drEmpPayrollMisc["Tpm_Misc1VarAmt"]         = 0;
                                        drEmpPayrollMisc["Tpm_Misc1OTAmt"]          = 0;
                                        drEmpPayrollMisc["Tpm_Misc1OTVarAmt"]       = 0;
                                        drEmpPayrollMisc["Tpm_Misc1NDAmt"]          = 0;
                                        drEmpPayrollMisc["Tpm_Misc1NDVarAmt"]       = 0;
                                        drEmpPayrollMisc["Tpm_Misc1NDOTAmt"]        = 0;
                                        drEmpPayrollMisc["Tpm_Misc1NDOTVarAmt"]     = 0;
                                        drEmpPayrollMisc["Tpm_Misc2Amt"]            = 0;
                                        drEmpPayrollMisc["Tpm_Misc2VarAmt"]         = 0;
                                        drEmpPayrollMisc["Tpm_Misc2OTAmt"]          = 0;
                                        drEmpPayrollMisc["Tpm_Misc2OTVarAmt"]       = 0;
                                        drEmpPayrollMisc["Tpm_Misc2NDAmt"]          = 0;
                                        drEmpPayrollMisc["Tpm_Misc2NDVarAmt"]       = 0;
                                        drEmpPayrollMisc["Tpm_Misc2NDOTAmt"]        = 0;
                                        drEmpPayrollMisc["Tpm_Misc2NDOTVarAmt"]     = 0;
                                        drEmpPayrollMisc["Tpm_Misc3Amt"]            = 0;
                                        drEmpPayrollMisc["Tpm_Misc3VarAmt"]         = 0;
                                        drEmpPayrollMisc["Tpm_Misc3OTAmt"]          = 0;
                                        drEmpPayrollMisc["Tpm_Misc3OTVarAmt"]       = 0;
                                        drEmpPayrollMisc["Tpm_Misc3NDAmt"]          = 0;
                                        drEmpPayrollMisc["Tpm_Misc3NDVarAmt"]       = 0;
                                        drEmpPayrollMisc["Tpm_Misc3NDOTAmt"]        = 0;
                                        drEmpPayrollMisc["Tpm_Misc3NDOTVarAmt"]     = 0;
                                        drEmpPayrollMisc["Tpm_Misc4Amt"]            = 0;
                                        drEmpPayrollMisc["Tpm_Misc4VarAmt"]         = 0;
                                        drEmpPayrollMisc["Tpm_Misc4OTAmt"]          = 0;
                                        drEmpPayrollMisc["Tpm_Misc4OTVarAmt"]       = 0;
                                        drEmpPayrollMisc["Tpm_Misc4NDAmt"]          = 0;
                                        drEmpPayrollMisc["Tpm_Misc4NDVarAmt"]       = 0;
                                        drEmpPayrollMisc["Tpm_Misc4NDOTAmt"]        = 0;
                                        drEmpPayrollMisc["Tpm_Misc4NDOTVarAmt"]     = 0;
                                        drEmpPayrollMisc["Tpm_Misc5Amt"]            = 0;
                                        drEmpPayrollMisc["Tpm_Misc5VarAmt"]         = 0;
                                        drEmpPayrollMisc["Tpm_Misc5OTAmt"]          = 0;
                                        drEmpPayrollMisc["Tpm_Misc5OTVarAmt"]       = 0;
                                        drEmpPayrollMisc["Tpm_Misc5NDAmt"]          = 0;
                                        drEmpPayrollMisc["Tpm_Misc5NDVarAmt"]       = 0;
                                        drEmpPayrollMisc["Tpm_Misc5NDOTAmt"]        = 0;
                                        drEmpPayrollMisc["Tpm_Misc5NDOTVarAmt"]     = 0;
                                        drEmpPayrollMisc["Tpm_Misc6Amt"]            = 0;
                                        drEmpPayrollMisc["Tpm_Misc6VarAmt"]         = 0;
                                        drEmpPayrollMisc["Tpm_Misc6OTAmt"]          = 0;
                                        drEmpPayrollMisc["Tpm_Misc6OTVarAmt"]       = 0;
                                        drEmpPayrollMisc["Tpm_Misc6NDAmt"]          = 0;
                                        drEmpPayrollMisc["Tpm_Misc6NDVarAmt"]       = 0;
                                        drEmpPayrollMisc["Tpm_Misc6NDOTAmt"]        = 0;
                                        drEmpPayrollMisc["Tpm_Misc6NDOTVarAmt"]     = 0;
                                        drEmpPayrollMisc["Usr_Login"]               = LoginUser;
                                        drEmpPayrollMisc["Ludatetime"]              = DateTime.Now;
                                    }
                                    #endregion

                                    #region Initialize Payroll Calc Amount
                                    LateAmt                                         = 0;
                                    UndertimeAmt                                    = 0;
                                    UnpaidLeaveAmt                                  = 0;
                                    AbsentLegalHolidayAmt                           = 0;
                                    AbsentSpecialHolidayAmt                         = 0;
                                    AbsentCompanyHolidayAmt                         = 0;
                                    AbsentPlantShutdownAmt                          = 0;
                                    AbsentFillerHolidayAmt                          = 0;
                                    WholeDayAbsentAmt                               = 0;
                                    LateUndertimeMaxAbsentAmt                       = 0;
                                    AbsentAmt                                       = 0;

                                    RegularAmt                                      = 0;
                                    PaidLeaveAmt                                    = 0;
                                    PaidLegalHolidayAmt                             = 0;
                                    PaidSpecialHolidayAmt                           = 0;
                                    PaidCompanyHolidayAmt                           = 0;
                                    PaidFillerHolidayAmt                            = 0;
                                    PaidPlantShutdownHolidayAmt                     = 0;
                                    PaidRestdayLegalHolidayAmt                      = 0;

                                    RegularOTAmt                                    = 0;
                                    RegularNDAmt                                    = 0;
                                    RegularOTNDAmt                                  = 0;
                                    RestdayAmt                                      = 0;
                                    RestdayOTAmt                                    = 0;
                                    RestdayNDAmt                                    = 0;
                                    RestdayOTNDAmt                                  = 0;
                                    LegalHolidayAmt                                 = 0;
                                    LegalHolidayOTAmt                               = 0;
                                    LegalHolidayNDAmt                               = 0;
                                    LegalHolidayOTNDAmt                             = 0;
                                    SpecialHolidayAmt                               = 0;
                                    SpecialHolidayOTAmt                             = 0;
                                    SpecialHolidayNDAmt                             = 0;
                                    SpecialHolidayOTNDAmt                           = 0;
                                    PlantShutdownAmt                                = 0;
                                    PlantShutdownOTAmt                              = 0;
                                    PlantShutdownNDAmt                              = 0;
                                    PlantShutdownOTNDAmt                            = 0;
                                    CompanyHolidayAmt                               = 0;
                                    CompanyHolidayOTAmt                             = 0;
                                    CompanyHolidayNDAmt                             = 0;
                                    CompanyHolidayOTNDAmt                           = 0;
                                    RestdayLegalHolidayAmt                          = 0;
                                    RestdayLegalHolidayOTAmt                        = 0;
                                    RestdayLegalHolidayNDAmt                        = 0;
                                    RestdayLegalHolidayOTNDAmt                      = 0;
                                    RestdaySpecialHolidayAmt                        = 0;
                                    RestdaySpecialHolidayOTAmt                      = 0;
                                    RestdaySpecialHolidayNDAmt                      = 0;
                                    RestdaySpecialHolidayOTNDAmt                    = 0;
                                    RestdayCompanyHolidayAmt                        = 0;
                                    RestdayCompanyHolidayOTAmt                      = 0;
                                    RestdayCompanyHolidayNDAmt                      = 0;
                                    RestdayCompanyHolidayOTNDAmt                    = 0;
                                    RestdayPlantShutdownAmt                         = 0;
                                    RestdayPlantShutdownOTAmt                       = 0;
                                    RestdayPlantShutdownNDAmt                       = 0;
                                    RestdayPlantShutdownOTNDAmt                     = 0;

                                    OldSalaryRate                                   = 0;
                                    OldDailyRate                                    = 0;
                                    NewDailyRate                                    = 0;
                                    WorkingDayCnt                                   = 0;
                                    WorkingDayCntUsingNewRate                       = 0;

                                    Misc1Amt                                        = 0;
                                    Misc1OTAmt                                      = 0;
                                    Misc1NDAmt                                      = 0;
                                    Misc1NDOTAmt                                    = 0;
                                    Misc2Amt                                        = 0;
                                    Misc2OTAmt                                      = 0;
                                    Misc2NDAmt                                      = 0;
                                    Misc2NDOTAmt                                    = 0;
                                    Misc3Amt                                        = 0;
                                    Misc3OTAmt                                      = 0;
                                    Misc3NDAmt                                      = 0;
                                    Misc3NDOTAmt                                    = 0;
                                    Misc4Amt                                        = 0;
                                    Misc4OTAmt                                      = 0;
                                    Misc4NDAmt                                      = 0;
                                    Misc4NDOTAmt                                    = 0;
                                    Misc5Amt                                        = 0;
                                    Misc5OTAmt                                      = 0;
                                    Misc5NDAmt                                      = 0;
                                    Misc5NDOTAmt                                    = 0;
                                    Misc6Amt                                        = 0;
                                    Misc6OTAmt                                      = 0;
                                    Misc6NDAmt                                      = 0;
                                    Misc6NDOTAmt                                    = 0;

                                    RGRetroAmt                                      = 0;
                                    OTRetroAmt                                      = 0;
                                    NDRetroAmt                                      = 0;
                                    HLRetroAmt                                      = 0;
                                    LVRetroAmt                                      = 0;

                                    #endregion

                                    #region Initialize Reg Rule
                                    strRegRule = "";
                                    bMonthlyToDailyPayrollType = false;
                                    if (dtEmployeeSalary.Rows.Count > 0) //MULTIPLE SALARY
                                    {
                                        OldSalaryRate = Convert.ToDecimal(dtEmployeeSalary.Rows[0]["Tsl_SalaryRate"]);
                                        if (dtEmployeeSalary.Rows[0]["Tsl_PayrollType"].ToString() == dtEmployeeSalary.Rows[dtEmployeeSalary.Rows.Count - 1]["Tsl_PayrollType"].ToString())
                                        {
                                            if (dtEmployeeSalary.Rows[0]["Tsl_PayrollType"].ToString() == "M")
                                                drEmpPayroll["Tpy_RegRule"] = "A";
                                            else if (dtEmployeeSalary.Rows[0]["Tsl_PayrollType"].ToString() == "D")
                                                drEmpPayroll["Tpy_RegRule"] = "P";
                                        }
                                        else if (dtEmployeeSalary.Rows[0]["Tsl_PayrollType"].ToString() != dtEmployeeSalary.Rows[dtEmployeeSalary.Rows.Count - 1]["Tsl_PayrollType"].ToString()
                                            && dtEmployeeSalary.Rows[0]["Tsl_PayrollType"].ToString() == "M"
                                            && dtEmployeeSalary.Rows[dtEmployeeSalary.Rows.Count - 1]["Tsl_PayrollType"].ToString() == "D")
                                        {
                                            drEmpPayroll["Tpy_RegRule"] = MULTSALMD;
                                            bMonthlyToDailyPayrollType = true;
                                        }
                                        else if (dtEmployeeSalary.Rows[0]["Tsl_PayrollType"].ToString() != dtEmployeeSalary.Rows[dtEmployeeSalary.Rows.Count - 1]["Tsl_PayrollType"].ToString()
                                            && dtEmployeeSalary.Rows[0]["Tsl_PayrollType"].ToString() == "D"
                                            && dtEmployeeSalary.Rows[dtEmployeeSalary.Rows.Count - 1]["Tsl_PayrollType"].ToString() == "M")
                                            drEmpPayroll["Tpy_RegRule"] = MULTSALDM;
                                    }
                                    else
                                    {
                                        drArrEmpNewlyHiredEmployees = dtNewlyHiredEmployees.Select("Mem_IDNo = '" + curEmployeeId + "'");
                                        if (PayrollType == "D")
                                            drEmpPayroll["Tpy_RegRule"] = "P";
                                        else if (drArrEmpNewlyHiredEmployees.Length > 0 && PayrollType == "M")
                                        {
                                            drEmpPayroll["Tpy_RegRule"] = NEWHIRE;
                                            if (Convert.ToDateTime(drArrEmpNewlyHiredEmployees[0]["Mem_IntakeDate"]) == Convert.ToDateTime(PayrollStart))
                                                drEmpPayroll["Tpy_RegRule"] = "A";
                                        }

                                        else if (drArrEmpNewlyHiredEmployees.Length == 0 && PayrollType == "M")
                                            drEmpPayroll["Tpy_RegRule"] = "A";
                                    }

                                    #region New Hire Regular Amount Proration (HOGP)
                                    drArrEmpComputedDailyEmployees = dtComputedDailyEmployees.Select("Mem_IDNo = '" + curEmployeeId + "'");
                                    if (drArrEmpComputedDailyEmployees.Length > 0 && PayrollType == "M")
                                    {
                                        drEmpPayroll["Tpy_RegRule"] = "P"; //computed regular amount as daily paid
                                    }
                                    #endregion

                                    strRegRule = Convert.ToString(drEmpPayroll["Tpy_RegRule"]);

                                    #endregion

                                    #region Payroll Dtl
                                    dtPayrollDtlForProcess = GetAllEmployeePayrollDtlForProcess(curEmployeeId, strPayCycle, SalaryDate, AdjustPayCycleStart, AdjustPayCycleEnd);
                                    for (int idtl = 0; idtl < dtPayrollDtlForProcess.Rows.Count; idtl++)
                                    {
                                        #region Initialize Payroll Dtl Calc Hour
                                        drEmpPayrollDtl = dtEmpPayrollDtl.NewRow();

                                        drEmpPayrollDtl["Tpd_IDNo"]                     = curEmployeeId;
                                        drEmpPayrollDtl["Tpd_RetroPayCycle"]            = PayrollPeriod;
                                        drEmpPayrollDtl["Tpd_PayCycle"]                 = strPayCycle; //Affected Pay Cycles
                                        drEmpPayrollDtl["Tpd_Date"]                     = dtPayrollDtlForProcess.Rows[idtl]["Tpd_Date"];
                                        drEmpPayrollDtl["Tpd_DayCode"]                  = dtPayrollDtlForProcess.Rows[idtl]["Tpd_DayCode"];
                                        drEmpPayrollDtl["Tpd_RestDayFlag"]              = dtPayrollDtlForProcess.Rows[idtl]["Tpd_RestDayFlag"];
                                        drEmpPayrollDtl["Tpd_SalaryRate"]               = dtPayrollDtlForProcess.Rows[idtl]["Tpd_SalaryRate"]; //T_EmpSalary
                                        drEmpPayrollDtl["Tpd_PayrollType"]              = dtPayrollDtlForProcess.Rows[idtl]["Tpd_PayrollType"]; //T_EmpSalary
                                        
                                        drEmpPayrollDtl["Tpd_PremiumGrpCode"]           = dtPayrollDtlForProcess.Rows[idtl]["Tpd_PremiumGrpCode"];
                                        drEmpPayrollDtl["Tpd_LTHr"]                     = dtPayrollDtlForProcess.Rows[idtl]["Tpd_LTHr"];
                                        drEmpPayrollDtl["Tpd_UTHr"]                     = dtPayrollDtlForProcess.Rows[idtl]["Tpd_UTHr"];
                                        drEmpPayrollDtl["Tpd_UPLVHr"]                   = dtPayrollDtlForProcess.Rows[idtl]["Tpd_UPLVHr"];
                                        drEmpPayrollDtl["Tpd_ABSLEGHOLHr"]              = dtPayrollDtlForProcess.Rows[idtl]["Tpd_ABSLEGHOLHr"];
                                        drEmpPayrollDtl["Tpd_ABSSPLHOLHr"]              = dtPayrollDtlForProcess.Rows[idtl]["Tpd_ABSSPLHOLHr"];
                                        drEmpPayrollDtl["Tpd_ABSCOMPHOLHr"]             = dtPayrollDtlForProcess.Rows[idtl]["Tpd_ABSCOMPHOLHr"];
                                        drEmpPayrollDtl["Tpd_ABSPSDHr"]                 = dtPayrollDtlForProcess.Rows[idtl]["Tpd_ABSPSDHr"];
                                        drEmpPayrollDtl["Tpd_ABSOTHHOLHr"]              = dtPayrollDtlForProcess.Rows[idtl]["Tpd_ABSOTHHOLHr"];
                                        drEmpPayrollDtl["Tpd_WDABSHr"]                  = dtPayrollDtlForProcess.Rows[idtl]["Tpd_WDABSHr"];
                                        drEmpPayrollDtl["Tpd_LTUTMaxHr"]                = dtPayrollDtlForProcess.Rows[idtl]["Tpd_LTUTMaxHr"];
                                        drEmpPayrollDtl["Tpd_ABSHr"]                    = dtPayrollDtlForProcess.Rows[idtl]["Tpd_ABSHr"];

                                        drEmpPayrollDtl["Tpd_ABSHr"]                    = Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSHr"])
                                                                                                            + Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_ABSLEGHOLHr"])
                                                                                                            + Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_ABSSPLHOLHr"])
                                                                                                            + Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_ABSCOMPHOLHr"])
                                                                                                            + Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_ABSPSDHr"])
                                                                                                            + Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_ABSOTHHOLHr"]);

                                        drEmpPayrollDtl["Tpd_PDLVHr"]                   = dtPayrollDtlForProcess.Rows[idtl]["Tpd_PDLVHr"];
                                        drEmpPayrollDtl["Tpd_PDLEGHOLHr"]               = dtPayrollDtlForProcess.Rows[idtl]["Tpd_PDLEGHOLHr"];
                                        drEmpPayrollDtl["Tpd_PDSPLHOLHr"]               = dtPayrollDtlForProcess.Rows[idtl]["Tpd_PDSPLHOLHr"];
                                        drEmpPayrollDtl["Tpd_PDCOMPHOLHr"]              = dtPayrollDtlForProcess.Rows[idtl]["Tpd_PDCOMPHOLHr"];
                                        drEmpPayrollDtl["Tpd_PDPSDHr"]                  = dtPayrollDtlForProcess.Rows[idtl]["Tpd_PDPSDHr"];
                                        drEmpPayrollDtl["Tpd_PDOTHHOLHr"]               = dtPayrollDtlForProcess.Rows[idtl]["Tpd_PDOTHHOLHr"];
                                        drEmpPayrollDtl["Tpd_PDRESTLEGHOLHr"]           = dtPayrollDtlForProcess.Rows[idtl]["Tpd_PDRESTLEGHOLHr"];

                                        if (strRegRule == "P")
                                        {
                                            drEmpPayrollDtl["Tpd_REGHr"]                = dtPayrollDtlForProcess.Rows[idtl]["Tpd_REGHr"];
                                            drEmpPayrollDtl["Tpd_TotalREGHr"]           = Convert.ToDouble(dtPayrollDtlForProcess.Rows[idtl]["Tpd_REGHr"])
                                                                                              + Convert.ToDouble(dtPayrollDtlForProcess.Rows[idtl]["Tpd_PDLVHr"])
                                                                                              + Convert.ToDouble(dtPayrollDtlForProcess.Rows[idtl]["Tpd_PDLEGHOLHr"])
                                                                                              + Convert.ToDouble(dtPayrollDtlForProcess.Rows[idtl]["Tpd_PDSPLHOLHr"])
                                                                                              + Convert.ToDouble(dtPayrollDtlForProcess.Rows[idtl]["Tpd_PDCOMPHOLHr"])
                                                                                              + Convert.ToDouble(dtPayrollDtlForProcess.Rows[idtl]["Tpd_PDPSDHr"])
                                                                                              + Convert.ToDouble(dtPayrollDtlForProcess.Rows[idtl]["Tpd_PDOTHHOLHr"])
                                                                                              + Convert.ToDouble(dtPayrollDtlForProcess.Rows[idtl]["Tpd_PDRESTLEGHOLHr"]);
                                        }
                                        else
                                        {
                                            drEmpPayrollDtl["Tpd_REGHr"]                = 0;
                                            drEmpPayrollDtl["Tpd_TotalREGHr"]           = 0;
                                        }

                                        drEmpPayrollDtl["Tpd_REGOTHr"]                  = dtPayrollDtlForProcess.Rows[idtl]["Tpd_REGOTHr"];
                                        drEmpPayrollDtl["Tpd_REGNDHr"]                  = dtPayrollDtlForProcess.Rows[idtl]["Tpd_REGNDHr"];
                                        drEmpPayrollDtl["Tpd_REGNDOTHr"]                = dtPayrollDtlForProcess.Rows[idtl]["Tpd_REGNDOTHr"];
                                        drEmpPayrollDtl["Tpd_RESTHr"]                   = dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTHr"];
                                        drEmpPayrollDtl["Tpd_RESTOTHr"]                 = dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTOTHr"];
                                        drEmpPayrollDtl["Tpd_RESTNDHr"]                 = dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTNDHr"];
                                        drEmpPayrollDtl["Tpd_RESTNDOTHr"]               = dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTNDOTHr"];
                                        drEmpPayrollDtl["Tpd_LEGHOLHr"]                 = dtPayrollDtlForProcess.Rows[idtl]["Tpd_LEGHOLHr"];
                                        drEmpPayrollDtl["Tpd_LEGHOLOTHr"]               = dtPayrollDtlForProcess.Rows[idtl]["Tpd_LEGHOLOTHr"];
                                        drEmpPayrollDtl["Tpd_LEGHOLNDHr"]               = dtPayrollDtlForProcess.Rows[idtl]["Tpd_LEGHOLNDHr"];
                                        drEmpPayrollDtl["Tpd_LEGHOLNDOTHr"]             = dtPayrollDtlForProcess.Rows[idtl]["Tpd_LEGHOLNDOTHr"];
                                        drEmpPayrollDtl["Tpd_SPLHOLHr"]                 = dtPayrollDtlForProcess.Rows[idtl]["Tpd_SPLHOLHr"];
                                        drEmpPayrollDtl["Tpd_SPLHOLOTHr"]               = dtPayrollDtlForProcess.Rows[idtl]["Tpd_SPLHOLOTHr"];
                                        drEmpPayrollDtl["Tpd_SPLHOLNDHr"]               = dtPayrollDtlForProcess.Rows[idtl]["Tpd_SPLHOLNDHr"];
                                        drEmpPayrollDtl["Tpd_SPLHOLNDOTHr"]             = dtPayrollDtlForProcess.Rows[idtl]["Tpd_SPLHOLNDOTHr"];
                                        drEmpPayrollDtl["Tpd_PSDHr"]                    = dtPayrollDtlForProcess.Rows[idtl]["Tpd_PSDHr"];
                                        drEmpPayrollDtl["Tpd_PSDOTHr"]                  = dtPayrollDtlForProcess.Rows[idtl]["Tpd_PSDOTHr"];
                                        drEmpPayrollDtl["Tpd_PSDNDHr"]                  = dtPayrollDtlForProcess.Rows[idtl]["Tpd_PSDNDHr"];
                                        drEmpPayrollDtl["Tpd_PSDNDOTHr"]                = dtPayrollDtlForProcess.Rows[idtl]["Tpd_PSDNDOTHr"];
                                        drEmpPayrollDtl["Tpd_COMPHOLHr"]                = dtPayrollDtlForProcess.Rows[idtl]["Tpd_COMPHOLHr"];
                                        drEmpPayrollDtl["Tpd_COMPHOLOTHr"]              = dtPayrollDtlForProcess.Rows[idtl]["Tpd_COMPHOLOTHr"];
                                        drEmpPayrollDtl["Tpd_COMPHOLNDHr"]              = dtPayrollDtlForProcess.Rows[idtl]["Tpd_COMPHOLNDHr"];
                                        drEmpPayrollDtl["Tpd_COMPHOLNDOTHr"]            = dtPayrollDtlForProcess.Rows[idtl]["Tpd_COMPHOLNDOTHr"];
                                        drEmpPayrollDtl["Tpd_RESTLEGHOLHr"]             = dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTLEGHOLHr"];
                                        drEmpPayrollDtl["Tpd_RESTLEGHOLOTHr"]           = dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTLEGHOLOTHr"];
                                        drEmpPayrollDtl["Tpd_RESTLEGHOLNDHr"]           = dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTLEGHOLNDHr"];
                                        drEmpPayrollDtl["Tpd_RESTLEGHOLNDOTHr"]         = dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTLEGHOLNDOTHr"];
                                        drEmpPayrollDtl["Tpd_RESTSPLHOLHr"]             = dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTSPLHOLHr"];
                                        drEmpPayrollDtl["Tpd_RESTSPLHOLOTHr"]           = dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTSPLHOLOTHr"];
                                        drEmpPayrollDtl["Tpd_RESTSPLHOLNDHr"]           = dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTSPLHOLNDHr"];
                                        drEmpPayrollDtl["Tpd_RESTSPLHOLNDOTHr"]         = dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTSPLHOLNDOTHr"];
                                        drEmpPayrollDtl["Tpd_RESTCOMPHOLHr"]            = dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTCOMPHOLHr"];
                                        drEmpPayrollDtl["Tpd_RESTCOMPHOLOTHr"]          = dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTCOMPHOLOTHr"];
                                        drEmpPayrollDtl["Tpd_RESTCOMPHOLNDHr"]          = dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTCOMPHOLNDHr"];
                                        drEmpPayrollDtl["Tpd_RESTCOMPHOLNDOTHr"]        = dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTCOMPHOLNDOTHr"];
                                        drEmpPayrollDtl["Tpd_RESTPSDHr"]                = dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTPSDHr"];
                                        drEmpPayrollDtl["Tpd_RESTPSDOTHr"]              = dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTPSDOTHr"];
                                        drEmpPayrollDtl["Tpd_RESTPSDNDHr"]              = dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTPSDNDHr"];
                                        drEmpPayrollDtl["Tpd_RESTPSDNDOTHr"]            = dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTPSDNDOTHr"];
                                        drEmpPayrollDtl["Usr_login"]                    = LoginUser;
                                        drEmpPayrollDtl["Ludatetime"]                   = DateTime.Now;

                                        WorkingDayCnt                                   += Convert.ToInt32(dtPayrollDtlForProcess.Rows[idtl]["Tpd_WorkDay"]);
                                        #endregion

                                        if (bHasDayCodeExt)
                                        {
                                            #region Initialize Payroll Dtl Misc Calc Hour
                                            drEmpPayrollDtlMisc = dtEmpPayrollDtlMisc.NewRow();

                                            drEmpPayrollDtlMisc["Tpm_IDNo"]                 = dtPayrollDtlForProcess.Rows[idtl]["Tpd_IDNo"];
                                            drEmpPayrollDtlMisc["Tpm_RetroPayCycle"]        = PayrollPeriod;
                                            drEmpPayrollDtlMisc["Tpm_PayCycle"]             = strPayCycle; //Affected Pay Cycles
                                            drEmpPayrollDtlMisc["Tpm_Date"]                 = dtPayrollDtlForProcess.Rows[idtl]["Tpd_Date"];
                                            drEmpPayrollDtlMisc["Tpm_Misc1Hr"]              = dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc1Hr"];
                                            drEmpPayrollDtlMisc["Tpm_Misc1OTHr"]            = dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc1OTHr"];
                                            drEmpPayrollDtlMisc["Tpm_Misc1NDHr"]            = dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc1NDHr"];
                                            drEmpPayrollDtlMisc["Tpm_Misc1NDOTHr"]          = dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc1NDOTHr"];
                                            drEmpPayrollDtlMisc["Tpm_Misc2Hr"]              = dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc2Hr"];
                                            drEmpPayrollDtlMisc["Tpm_Misc2OTHr"]            = dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc2OTHr"];
                                            drEmpPayrollDtlMisc["Tpm_Misc2NDHr"]            = dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc2NDHr"];
                                            drEmpPayrollDtlMisc["Tpm_Misc2NDOTHr"]          = dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc2NDOTHr"];
                                            drEmpPayrollDtlMisc["Tpm_Misc3Hr"]              = dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc3Hr"];
                                            drEmpPayrollDtlMisc["Tpm_Misc3OTHr"]            = dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc3OTHr"];
                                            drEmpPayrollDtlMisc["Tpm_Misc3NDHr"]            = dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc3NDHr"];
                                            drEmpPayrollDtlMisc["Tpm_Misc3NDOTHr"]          = dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc3NDOTHr"];
                                            drEmpPayrollDtlMisc["Tpm_Misc4Hr"]              = dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc4Hr"];
                                            drEmpPayrollDtlMisc["Tpm_Misc4OTHr"]            = dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc4OTHr"];
                                            drEmpPayrollDtlMisc["Tpm_Misc4NDHr"]            = dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc4NDHr"];
                                            drEmpPayrollDtlMisc["Tpm_Misc4NDOTHr"]          = dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc4NDOTHr"];
                                            drEmpPayrollDtlMisc["Tpm_Misc5Hr"]              = dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc5Hr"];
                                            drEmpPayrollDtlMisc["Tpm_Misc5OTHr"]            = dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc5OTHr"];
                                            drEmpPayrollDtlMisc["Tpm_Misc5NDHr"]            = dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc5NDHr"];
                                            drEmpPayrollDtlMisc["Tpm_Misc5NDOTHr"]          = dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc5NDOTHr"];
                                            drEmpPayrollDtlMisc["Tpm_Misc6Hr"]              = dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc6Hr"];
                                            drEmpPayrollDtlMisc["Tpm_Misc6OTHr"]            = dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc6OTHr"];
                                            drEmpPayrollDtlMisc["Tpm_Misc6NDHr"]            = dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc6NDHr"];
                                            drEmpPayrollDtlMisc["Tpm_Misc6NDOTHr"]          = dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc6NDOTHr"];
                                            drEmpPayrollDtlMisc["Usr_login"]                = LoginUser;
                                            drEmpPayrollDtlMisc["Ludatetime"]               = DateTime.Now;
                                            #endregion
                                        }

                                        if (!Convert.ToBoolean(dtPayrollDtlForProcess.Rows[idtl]["RetroIndicator"]))
                                        {
                                            drEmpPayrollDtl["Tpd_HourRate"]         = dtPayrollDtlForProcess.Rows[idtl]["Tpd_HourRate"];
                                            drEmpPayrollDtl["Tpd_SpecialRate"]      = dtPayrollDtlForProcess.Rows[idtl]["Tpd_SpecialRate"];
                                            drEmpPayrollDtl["Tpd_SpecialHourRate"]  = dtPayrollDtlForProcess.Rows[idtl]["Tpd_SpecialHourRate"];

                                            #region Initialize Payroll Dtl Calc Amount
                                            drEmpPayrollDtl["Tpd_LTAmt"]                    = dtPayrollDtlForProcess.Rows[idtl]["Tpd_LTAmt"];
                                            drEmpPayrollDtl["Tpd_UTAmt"]                    = dtPayrollDtlForProcess.Rows[idtl]["Tpd_UTAmt"];
                                            drEmpPayrollDtl["Tpd_UPLVAmt"]                  = dtPayrollDtlForProcess.Rows[idtl]["Tpd_UPLVAmt"];
                                            drEmpPayrollDtl["Tpd_ABSLEGHOLAmt"]             = dtPayrollDtlForProcess.Rows[idtl]["Tpd_ABSLEGHOLAmt"];
                                            drEmpPayrollDtl["Tpd_ABSSPLHOLAmt"]             = dtPayrollDtlForProcess.Rows[idtl]["Tpd_ABSSPLHOLAmt"];
                                            drEmpPayrollDtl["Tpd_ABSCOMPHOLAmt"]            = dtPayrollDtlForProcess.Rows[idtl]["Tpd_ABSCOMPHOLAmt"];
                                            drEmpPayrollDtl["Tpd_ABSPSDAmt"]                = dtPayrollDtlForProcess.Rows[idtl]["Tpd_ABSPSDAmt"];
                                            drEmpPayrollDtl["Tpd_ABSOTHHOLAmt"]             = dtPayrollDtlForProcess.Rows[idtl]["Tpd_ABSOTHHOLAmt"];
                                            drEmpPayrollDtl["Tpd_WDABSAmt"]                 = dtPayrollDtlForProcess.Rows[idtl]["Tpd_WDABSAmt"];
                                            drEmpPayrollDtl["Tpd_LTUTMaxAmt"]               = dtPayrollDtlForProcess.Rows[idtl]["Tpd_LTUTMaxAmt"];
                                            drEmpPayrollDtl["Tpd_ABSAmt"]                   = dtPayrollDtlForProcess.Rows[idtl]["Tpd_ABSAmt"];
                                            if (PayrollType == "D")
                                                drEmpPayrollDtl["Tpd_REGAmt"]               = dtPayrollDtlForProcess.Rows[idtl]["Tpd_REGAmt"];
                                            else if (PayrollType == "M")
                                                drEmpPayrollDtl["Tpd_REGAmt"]               = 0;
                                            drEmpPayrollDtl["Tpd_PDLVAmt"]                  = dtPayrollDtlForProcess.Rows[idtl]["Tpd_PDLVAmt"];
                                            drEmpPayrollDtl["Tpd_PDLEGHOLAmt"]              = dtPayrollDtlForProcess.Rows[idtl]["Tpd_PDLEGHOLAmt"];
                                            drEmpPayrollDtl["Tpd_PDSPLHOLAmt"]              = dtPayrollDtlForProcess.Rows[idtl]["Tpd_PDSPLHOLAmt"];
                                            drEmpPayrollDtl["Tpd_PDCOMPHOLAmt"]             = dtPayrollDtlForProcess.Rows[idtl]["Tpd_PDCOMPHOLAmt"];
                                            drEmpPayrollDtl["Tpd_PDPSDAmt"]                 = dtPayrollDtlForProcess.Rows[idtl]["Tpd_PDPSDAmt"];
                                            drEmpPayrollDtl["Tpd_PDOTHHOLAmt"]              = dtPayrollDtlForProcess.Rows[idtl]["Tpd_PDOTHHOLAmt"];
                                            drEmpPayrollDtl["Tpd_PDRESTLEGHOLAmt"]          = dtPayrollDtlForProcess.Rows[idtl]["Tpd_PDRESTLEGHOLAmt"];
                                            drEmpPayrollDtl["Tpd_TotalREGAmt"]              = dtPayrollDtlForProcess.Rows[idtl]["Tpd_TotalREGAmt"];

                                            drEmpPayrollDtl["Tpd_REGOTAmt"]                 = dtPayrollDtlForProcess.Rows[idtl]["Tpd_REGOTAmt"];
                                            drEmpPayrollDtl["Tpd_REGNDAmt"]                 = dtPayrollDtlForProcess.Rows[idtl]["Tpd_REGNDAmt"];
                                            drEmpPayrollDtl["Tpd_REGNDOTAmt"]               = dtPayrollDtlForProcess.Rows[idtl]["Tpd_REGNDOTAmt"];

                                            drEmpPayrollDtl["Tpd_RESTAmt"]                  = dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTAmt"];
                                            drEmpPayrollDtl["Tpd_RESTOTAmt"]                = dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTOTAmt"];
                                            drEmpPayrollDtl["Tpd_RESTNDAmt"]                = dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTNDAmt"];
                                            drEmpPayrollDtl["Tpd_RESTNDOTAmt"]              = dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTNDOTAmt"];

                                            drEmpPayrollDtl["Tpd_LEGHOLAmt"]                = dtPayrollDtlForProcess.Rows[idtl]["Tpd_LEGHOLAmt"];
                                            drEmpPayrollDtl["Tpd_LEGHOLOTAmt"]              = dtPayrollDtlForProcess.Rows[idtl]["Tpd_LEGHOLOTAmt"];
                                            drEmpPayrollDtl["Tpd_LEGHOLNDAmt"]              = dtPayrollDtlForProcess.Rows[idtl]["Tpd_LEGHOLNDAmt"];
                                            drEmpPayrollDtl["Tpd_LEGHOLNDOTAmt"]            = dtPayrollDtlForProcess.Rows[idtl]["Tpd_LEGHOLNDOTAmt"];

                                            drEmpPayrollDtl["Tpd_SPLHOLAmt"]                = dtPayrollDtlForProcess.Rows[idtl]["Tpd_SPLHOLAmt"];
                                            drEmpPayrollDtl["Tpd_SPLHOLOTAmt"]              = dtPayrollDtlForProcess.Rows[idtl]["Tpd_SPLHOLOTAmt"];
                                            drEmpPayrollDtl["Tpd_SPLHOLNDAmt"]              = dtPayrollDtlForProcess.Rows[idtl]["Tpd_SPLHOLNDAmt"];
                                            drEmpPayrollDtl["Tpd_SPLHOLNDOTAmt"]            = dtPayrollDtlForProcess.Rows[idtl]["Tpd_SPLHOLNDOTAmt"];

                                            drEmpPayrollDtl["Tpd_PSDAmt"]                   = dtPayrollDtlForProcess.Rows[idtl]["Tpd_PSDAmt"];
                                            drEmpPayrollDtl["Tpd_PSDOTAmt"]                 = dtPayrollDtlForProcess.Rows[idtl]["Tpd_PSDOTAmt"];
                                            drEmpPayrollDtl["Tpd_PSDNDAmt"]                 = dtPayrollDtlForProcess.Rows[idtl]["Tpd_PSDNDAmt"];
                                            drEmpPayrollDtl["Tpd_PSDNDOTAmt"]               = dtPayrollDtlForProcess.Rows[idtl]["Tpd_PSDNDOTAmt"];

                                            drEmpPayrollDtl["Tpd_COMPHOLAmt"]               = dtPayrollDtlForProcess.Rows[idtl]["Tpd_COMPHOLAmt"];
                                            drEmpPayrollDtl["Tpd_COMPHOLOTAmt"]             = dtPayrollDtlForProcess.Rows[idtl]["Tpd_COMPHOLOTAmt"];
                                            drEmpPayrollDtl["Tpd_COMPHOLNDAmt"]             = dtPayrollDtlForProcess.Rows[idtl]["Tpd_COMPHOLNDAmt"];
                                            drEmpPayrollDtl["Tpd_COMPHOLNDOTAmt"]           = dtPayrollDtlForProcess.Rows[idtl]["Tpd_COMPHOLNDOTAmt"];

                                            drEmpPayrollDtl["Tpd_RESTLEGHOLAmt"]            = dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTLEGHOLAmt"];
                                            drEmpPayrollDtl["Tpd_RESTLEGHOLOTAmt"]          = dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTLEGHOLOTAmt"];
                                            drEmpPayrollDtl["Tpd_RESTLEGHOLNDAmt"]          = dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTLEGHOLNDAmt"];
                                            drEmpPayrollDtl["Tpd_RESTLEGHOLNDOTAmt"]        = dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTLEGHOLNDOTAmt"];

                                            drEmpPayrollDtl["Tpd_RESTSPLHOLAmt"]            = dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTSPLHOLAmt"];
                                            drEmpPayrollDtl["Tpd_RESTSPLHOLOTAmt"]          = dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTSPLHOLOTAmt"];
                                            drEmpPayrollDtl["Tpd_RESTSPLHOLNDAmt"]          = dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTSPLHOLNDAmt"];
                                            drEmpPayrollDtl["Tpd_RESTSPLHOLNDOTAmt"]        = dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTSPLHOLNDOTAmt"];

                                            drEmpPayrollDtl["Tpd_RESTCOMPHOLAmt"]           = dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTCOMPHOLAmt"];
                                            drEmpPayrollDtl["Tpd_RESTCOMPHOLOTAmt"]         = dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTCOMPHOLOTAmt"];
                                            drEmpPayrollDtl["Tpd_RESTCOMPHOLNDAmt"]         = dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTCOMPHOLNDAmt"];
                                            drEmpPayrollDtl["Tpd_RESTCOMPHOLNDOTAmt"]       = dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTCOMPHOLNDOTAmt"];

                                            drEmpPayrollDtl["Tpd_RESTPSDAmt"]               = dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTPSDAmt"];
                                            drEmpPayrollDtl["Tpd_RESTPSDOTAmt"]             = dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTPSDOTAmt"];
                                            drEmpPayrollDtl["Tpd_RESTPSDNDAmt"]             = dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTPSDNDAmt"];
                                            drEmpPayrollDtl["Tpd_RESTPSDNDOTAmt"]           = dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTPSDNDOTAmt"];

                                            #endregion

                                            #region Initialize Payroll Dtl Calc Variance Amount
                                            drEmpPayrollDtl["Tpd_LTVarAmt"]                 = 0;
                                            drEmpPayrollDtl["Tpd_UTVarAmt"]                 = 0;
                                            drEmpPayrollDtl["Tpd_UPLVVarAmt"]               = 0;
                                            drEmpPayrollDtl["Tpd_ABSLEGHOLVarAmt"]          = 0;
                                            drEmpPayrollDtl["Tpd_ABSSPLHOLVarAmt"]          = 0;
                                            drEmpPayrollDtl["Tpd_ABSCOMPHOLVarAmt"]         = 0;
                                            drEmpPayrollDtl["Tpd_ABSPSDVarAmt"]             = 0;
                                            drEmpPayrollDtl["Tpd_ABSOTHHOLVarAmt"]          = 0;
                                            drEmpPayrollDtl["Tpd_WDABSVarAmt"]              = 0;
                                            drEmpPayrollDtl["Tpd_LTUTMaxVarAmt"]            = 0;
                                            drEmpPayrollDtl["Tpd_ABSVarAmt"]                = 0;
                                            drEmpPayrollDtl["Tpd_REGVarAmt"]                = 0;
                                            drEmpPayrollDtl["Tpd_PDLVVarAmt"]               = 0;
                                            drEmpPayrollDtl["Tpd_PDLEGHOLVarAmt"]           = 0;
                                            drEmpPayrollDtl["Tpd_PDSPLHOLVarAmt"]           = 0;
                                            drEmpPayrollDtl["Tpd_PDCOMPHOLVarAmt"]          = 0;
                                            drEmpPayrollDtl["Tpd_PDPSDVarAmt"]              = 0;
                                            drEmpPayrollDtl["Tpd_PDOTHHOLVarAmt"]           = 0;
                                            drEmpPayrollDtl["Tpd_PDRESTLEGHOLVarAmt"]       = 0;
                                            drEmpPayrollDtl["Tpd_TotalREGVarAmt"]           = 0;
                                            drEmpPayrollDtl["Tpd_REGOTVarAmt"]              = 0;
                                            drEmpPayrollDtl["Tpd_REGNDVarAmt"]              = 0;
                                            drEmpPayrollDtl["Tpd_REGNDOTVarAmt"]            = 0;
                                            drEmpPayrollDtl["Tpd_RESTVarAmt"]               = 0;
                                            drEmpPayrollDtl["Tpd_RESTOTVarAmt"]             = 0;
                                            drEmpPayrollDtl["Tpd_RESTNDVarAmt"]             = 0;
                                            drEmpPayrollDtl["Tpd_RESTNDOTVarAmt"]           = 0;
                                            drEmpPayrollDtl["Tpd_LEGHOLVarAmt"]             = 0;
                                            drEmpPayrollDtl["Tpd_LEGHOLOTVarAmt"]           = 0;
                                            drEmpPayrollDtl["Tpd_LEGHOLNDVarAmt"]           = 0;
                                            drEmpPayrollDtl["Tpd_LEGHOLNDOTVarAmt"]         = 0;
                                            drEmpPayrollDtl["Tpd_SPLHOLVarAmt"]             = 0;
                                            drEmpPayrollDtl["Tpd_SPLHOLOTVarAmt"]           = 0;
                                            drEmpPayrollDtl["Tpd_SPLHOLNDVarAmt"]           = 0;
                                            drEmpPayrollDtl["Tpd_SPLHOLNDOTVarAmt"]         = 0;
                                            drEmpPayrollDtl["Tpd_PSDVarAmt"]                = 0;
                                            drEmpPayrollDtl["Tpd_PSDOTVarAmt"]              = 0;
                                            drEmpPayrollDtl["Tpd_PSDNDVarAmt"]              = 0;
                                            drEmpPayrollDtl["Tpd_PSDNDOTVarAmt"]            = 0;
                                            drEmpPayrollDtl["Tpd_COMPHOLVarAmt"]            = 0;
                                            drEmpPayrollDtl["Tpd_COMPHOLOTVarAmt"]          = 0;
                                            drEmpPayrollDtl["Tpd_COMPHOLNDVarAmt"]          = 0;
                                            drEmpPayrollDtl["Tpd_COMPHOLNDOTVarAmt"]        = 0;
                                            drEmpPayrollDtl["Tpd_RESTLEGHOLVarAmt"]         = 0;
                                            drEmpPayrollDtl["Tpd_RESTLEGHOLOTVarAmt"]       = 0;
                                            drEmpPayrollDtl["Tpd_RESTLEGHOLNDVarAmt"]       = 0;
                                            drEmpPayrollDtl["Tpd_RESTLEGHOLNDOTVarAmt"]     = 0;
                                            drEmpPayrollDtl["Tpd_RESTSPLHOLVarAmt"]         = 0;
                                            drEmpPayrollDtl["Tpd_RESTSPLHOLOTVarAmt"]       = 0;
                                            drEmpPayrollDtl["Tpd_RESTSPLHOLNDVarAmt"]       = 0;
                                            drEmpPayrollDtl["Tpd_RESTSPLHOLNDOTVarAmt"]     = 0;
                                            drEmpPayrollDtl["Tpd_RESTCOMPHOLVarAmt"]        = 0;
                                            drEmpPayrollDtl["Tpd_RESTCOMPHOLOTVarAmt"]      = 0;
                                            drEmpPayrollDtl["Tpd_RESTCOMPHOLNDVarAmt"]      = 0;
                                            drEmpPayrollDtl["Tpd_RESTCOMPHOLNDOTVarAmt"]    = 0;
                                            drEmpPayrollDtl["Tpd_RESTPSDVarAmt"]            = 0;
                                            drEmpPayrollDtl["Tpd_RESTPSDOTVarAmt"]          = 0;
                                            drEmpPayrollDtl["Tpd_RESTPSDNDVarAmt"]          = 0;
                                            drEmpPayrollDtl["Tpd_RESTPSDNDOTVarAmt"]        = 0;

                                            drEmpPayrollDtl["Tpd_RGRetroAmt"]               = 0;
                                            drEmpPayrollDtl["Tpd_OTRetroAmt"]               = 0;
                                            drEmpPayrollDtl["Tpd_NDRetroAmt"]               = 0;
                                            drEmpPayrollDtl["Tpd_HLRetroAmt"]               = 0;
                                            drEmpPayrollDtl["Tpd_LVRetroAmt"]               = 0;
                                            drEmpPayrollDtl["Tpd_TotalRetroAmt"]            = 0;
                                            #endregion

                                            #region Add to Hdr Amounts
                                            LateAmt += Convert.ToDecimal(drEmpPayrollDtl["Tpd_LTAmt"]);
                                            UndertimeAmt                    += Convert.ToDecimal(drEmpPayrollDtl["Tpd_UTAmt"]);
                                            UnpaidLeaveAmt                  += Convert.ToDecimal(drEmpPayrollDtl["Tpd_UPLVAmt"]);
                                            AbsentLegalHolidayAmt           += Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSLEGHOLAmt"]);
                                            AbsentSpecialHolidayAmt         += Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSSPLHOLAmt"]);
                                            AbsentCompanyHolidayAmt         += Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSCOMPHOLAmt"]);
                                            AbsentPlantShutdownAmt          += Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSPSDAmt"]);
                                            AbsentFillerHolidayAmt          += Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSOTHHOLAmt"]);
                                            WholeDayAbsentAmt               += Convert.ToDecimal(drEmpPayrollDtl["Tpd_WDABSAmt"]);
                                            LateUndertimeMaxAbsentAmt       += Convert.ToDecimal(drEmpPayrollDtl["Tpd_LTUTMaxAmt"]);
                                            AbsentAmt                       += Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSAmt"]);

                                            RegularAmt                      += Convert.ToDecimal(drEmpPayrollDtl["Tpd_REGAmt"]);
                                            PaidLeaveAmt                    += Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDLVAmt"]);

                                            PaidLegalHolidayAmt             += Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDLEGHOLAmt"]);
                                            PaidSpecialHolidayAmt           += Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDSPLHOLAmt"]);
                                            PaidCompanyHolidayAmt           += Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDCOMPHOLAmt"]);
                                            PaidPlantShutdownHolidayAmt     += Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDPSDAmt"]);
                                            PaidFillerHolidayAmt            += Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDOTHHOLAmt"]);
                                            PaidRestdayLegalHolidayAmt      += Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDRESTLEGHOLAmt"]);

                                            RegularOTAmt                    += Convert.ToDecimal(drEmpPayrollDtl["Tpd_REGOTAmt"]);
                                            RegularNDAmt                    += Convert.ToDecimal(drEmpPayrollDtl["Tpd_REGNDAmt"]);
                                            RegularOTNDAmt                  += Convert.ToDecimal(drEmpPayrollDtl["Tpd_REGNDOTAmt"]);

                                            RestdayAmt                      += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTAmt"]);
                                            RestdayOTAmt                    += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTOTAmt"]);
                                            RestdayNDAmt                    += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTNDAmt"]);
                                            RestdayOTNDAmt                  += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTNDOTAmt"]);

                                            LegalHolidayAmt                 += Convert.ToDecimal(drEmpPayrollDtl["Tpd_LEGHOLAmt"]);
                                            LegalHolidayOTAmt               += Convert.ToDecimal(drEmpPayrollDtl["Tpd_LEGHOLOTAmt"]);
                                            LegalHolidayNDAmt               += Convert.ToDecimal(drEmpPayrollDtl["Tpd_LEGHOLNDAmt"]);
                                            LegalHolidayOTNDAmt             += Convert.ToDecimal(drEmpPayrollDtl["Tpd_LEGHOLNDOTAmt"]);

                                            SpecialHolidayAmt               += Convert.ToDecimal(drEmpPayrollDtl["Tpd_SPLHOLAmt"]);
                                            SpecialHolidayOTAmt             += Convert.ToDecimal(drEmpPayrollDtl["Tpd_SPLHOLOTAmt"]);
                                            SpecialHolidayNDAmt             += Convert.ToDecimal(drEmpPayrollDtl["Tpd_SPLHOLNDAmt"]);
                                            SpecialHolidayOTNDAmt           += Convert.ToDecimal(drEmpPayrollDtl["Tpd_SPLHOLNDOTAmt"]);

                                            PlantShutdownAmt                += Convert.ToDecimal(drEmpPayrollDtl["Tpd_PSDAmt"]);
                                            PlantShutdownOTAmt              += Convert.ToDecimal(drEmpPayrollDtl["Tpd_PSDOTAmt"]);
                                            PlantShutdownNDAmt              += Convert.ToDecimal(drEmpPayrollDtl["Tpd_PSDNDAmt"]);
                                            PlantShutdownOTNDAmt            += Convert.ToDecimal(drEmpPayrollDtl["Tpd_PSDNDOTAmt"]);

                                            CompanyHolidayAmt               += Convert.ToDecimal(drEmpPayrollDtl["Tpd_COMPHOLAmt"]);
                                            CompanyHolidayOTAmt             += Convert.ToDecimal(drEmpPayrollDtl["Tpd_COMPHOLOTAmt"]);
                                            CompanyHolidayNDAmt             += Convert.ToDecimal(drEmpPayrollDtl["Tpd_COMPHOLNDAmt"]);
                                            CompanyHolidayOTNDAmt           += Convert.ToDecimal(drEmpPayrollDtl["Tpd_COMPHOLNDOTAmt"]);

                                            RestdayLegalHolidayAmt          += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTLEGHOLAmt"]);
                                            RestdayLegalHolidayOTAmt        += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTLEGHOLOTAmt"]);
                                            RestdayLegalHolidayNDAmt        += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTLEGHOLNDAmt"]);
                                            RestdayLegalHolidayOTNDAmt      += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTLEGHOLNDOTAmt"]);

                                            RestdaySpecialHolidayAmt        += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTSPLHOLAmt"]);
                                            RestdaySpecialHolidayOTAmt      += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTSPLHOLOTAmt"]);
                                            RestdaySpecialHolidayNDAmt      += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTSPLHOLNDAmt"]);
                                            RestdaySpecialHolidayOTNDAmt    += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTSPLHOLNDOTAmt"]);

                                            RestdayCompanyHolidayAmt        += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTCOMPHOLAmt"]);
                                            RestdayCompanyHolidayOTAmt      += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTCOMPHOLOTAmt"]);
                                            RestdayCompanyHolidayNDAmt      += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTCOMPHOLNDAmt"]);
                                            RestdayCompanyHolidayOTNDAmt    += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTCOMPHOLNDOTAmt"]);

                                            RestdayPlantShutdownAmt         += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTPSDAmt"]);
                                            RestdayPlantShutdownOTAmt       += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTPSDOTAmt"]);
                                            RestdayPlantShutdownNDAmt       += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTPSDNDAmt"]);
                                            RestdayPlantShutdownOTNDAmt     += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTPSDNDOTAmt"]);
                                            #endregion

                                            if (bHasDayCodeExt)
                                            {
                                                #region Initialize Payroll Dtl Misc Calc Amount
                                                drEmpPayrollDtlMisc["Tpm_Misc1Amt"]     = dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc1Amt"];
                                                drEmpPayrollDtlMisc["Tpm_Misc1OTAmt"]   = dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc1OTAmt"];
                                                drEmpPayrollDtlMisc["Tpm_Misc1NDAmt"]   = dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc1NDAmt"];
                                                drEmpPayrollDtlMisc["Tpm_Misc1NDOTAmt"] = dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc1NDOTAmt"];
                                                drEmpPayrollDtlMisc["Tpm_Misc2Amt"]     = dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc2Amt"];
                                                drEmpPayrollDtlMisc["Tpm_Misc2OTAmt"]   = dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc2OTAmt"];
                                                drEmpPayrollDtlMisc["Tpm_Misc2NDAmt"]   = dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc2NDAmt"];
                                                drEmpPayrollDtlMisc["Tpm_Misc2NDOTAmt"] = dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc2NDOTAmt"];
                                                drEmpPayrollDtlMisc["Tpm_Misc3Amt"]     = dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc3Amt"];
                                                drEmpPayrollDtlMisc["Tpm_Misc3OTAmt"]   = dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc3OTAmt"];
                                                drEmpPayrollDtlMisc["Tpm_Misc3NDAmt"]   = dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc3NDAmt"];
                                                drEmpPayrollDtlMisc["Tpm_Misc3NDOTAmt"] = dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc3NDOTAmt"];
                                                drEmpPayrollDtlMisc["Tpm_Misc4Amt"]     = dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc4Amt"];
                                                drEmpPayrollDtlMisc["Tpm_Misc4OTAmt"]   = dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc4OTAmt"];
                                                drEmpPayrollDtlMisc["Tpm_Misc4NDAmt"]   = dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc4NDAmt"];
                                                drEmpPayrollDtlMisc["Tpm_Misc4NDOTAmt"] = dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc4NDOTAmt"];
                                                drEmpPayrollDtlMisc["Tpm_Misc5Amt"]     = dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc5Amt"];
                                                drEmpPayrollDtlMisc["Tpm_Misc5OTAmt"]   = dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc5OTAmt"];
                                                drEmpPayrollDtlMisc["Tpm_Misc5NDAmt"]   = dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc5NDAmt"];
                                                drEmpPayrollDtlMisc["Tpm_Misc5NDOTAmt"] = dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc5NDOTAmt"];
                                                drEmpPayrollDtlMisc["Tpm_Misc6Amt"]     = dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc6Amt"];
                                                drEmpPayrollDtlMisc["Tpm_Misc6OTAmt"]   = dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc6OTAmt"];
                                                drEmpPayrollDtlMisc["Tpm_Misc6NDAmt"]   = dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc6NDAmt"];
                                                drEmpPayrollDtlMisc["Tpm_Misc6NDOTAmt"] = dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc6NDOTAmt"];
                                                #endregion

                                                #region Initialize Payroll Dtl Misc Calc Variance Amount
                                                drEmpPayrollDtlMisc["Tpm_Misc1VarAmt"]      = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc1OTVarAmt"]    = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc1NDVarAmt"]    = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc1NDOTVarAmt"]  = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc2VarAmt"]      = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc2OTVarAmt"]    = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc2NDVarAmt"]    = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc2NDOTVarAmt"]  = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc3VarAmt"]      = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc3OTVarAmt"]    = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc3NDVarAmt"]    = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc3NDOTVarAmt"]  = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc4VarAmt"]      = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc4OTVarAmt"]    = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc4NDVarAmt"]    = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc4NDOTVarAmt"]  = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc5VarAmt"]      = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc5OTVarAmt"]    = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc5NDVarAmt"]    = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc5NDOTVarAmt"]  = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc6VarAmt"]      = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc6OTVarAmt"]    = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc6NDVarAmt"]    = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc6NDOTVarAmt"]  = 0;
                                                #endregion

                                                #region Add to Hdr Ext Amounts
                                                Misc1Amt += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc1Amt"]);
                                                Misc1OTAmt      += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc1OTAmt"]);
                                                Misc1NDAmt      += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc1NDAmt"]);
                                                Misc1NDOTAmt    += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc1NDOTAmt"]);
                                                Misc2Amt        += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc2Amt"]);
                                                Misc2OTAmt      += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc2OTAmt"]);
                                                Misc2NDAmt      += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc2NDAmt"]);
                                                Misc2NDOTAmt    += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc2NDOTAmt"]);
                                                Misc3Amt        += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc3Amt"]);
                                                Misc3OTAmt      += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc3OTAmt"]);
                                                Misc3NDAmt      += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc3NDAmt"]);
                                                Misc3NDOTAmt    += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc3NDOTAmt"]);
                                                Misc4Amt        += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc4Amt"]);
                                                Misc4OTAmt      += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc4OTAmt"]);
                                                Misc4NDAmt      += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc4NDAmt"]);
                                                Misc4NDOTAmt    += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc4NDOTAmt"]);
                                                Misc5Amt        += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc5Amt"]);
                                                Misc5OTAmt      += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc5OTAmt"]);
                                                Misc5NDAmt      += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc5NDAmt"]);
                                                Misc5NDOTAmt    += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc5NDOTAmt"]);
                                                Misc6Amt        += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc6Amt"]);
                                                Misc6OTAmt      += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc6OTAmt"]);
                                                Misc6NDAmt      += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc6NDAmt"]);
                                                Misc6NDOTAmt    += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc6NDOTAmt"]);
                                                #endregion
                                            }

                                        }
                                        else
                                        {
                                            //Update to New Rate Info
                                            drEmpPayrollDtl["Tpd_HourRate"]         = HourlyRate;
                                            drEmpPayrollDtl["Tpd_SpecialRate"]      = SpecialSalaryRate;
                                            drEmpPayrollDtl["Tpd_SpecialHourRate"]  = SpecialHourlyRate;
                                            WorkingDayCntUsingNewRate               += Convert.ToInt32(dtPayrollDtlForProcess.Rows[idtl]["Tpd_WorkDay"]);

                                            #region Initialize Payroll Dtl Calc Amount
                                            drEmpPayrollDtl["Tpd_LTAmt"]                    = 0;
                                            drEmpPayrollDtl["Tpd_UTAmt"]                    = 0;
                                            drEmpPayrollDtl["Tpd_UPLVAmt"]                  = 0;
                                            drEmpPayrollDtl["Tpd_ABSLEGHOLAmt"]             = 0;
                                            drEmpPayrollDtl["Tpd_ABSSPLHOLAmt"]             = 0;
                                            drEmpPayrollDtl["Tpd_ABSCOMPHOLAmt"]            = 0;
                                            drEmpPayrollDtl["Tpd_ABSPSDAmt"]                = 0;
                                            drEmpPayrollDtl["Tpd_ABSOTHHOLAmt"]             = 0;
                                            drEmpPayrollDtl["Tpd_WDABSAmt"]                 = 0;
                                            drEmpPayrollDtl["Tpd_LTUTMaxAmt"]               = 0;
                                            drEmpPayrollDtl["Tpd_ABSAmt"]                   = 0;

                                            drEmpPayrollDtl["Tpd_REGAmt"]                   = 0;
                                            drEmpPayrollDtl["Tpd_PDLVAmt"]                  = 0;
                                            drEmpPayrollDtl["Tpd_PDLEGHOLAmt"]              = 0;
                                            drEmpPayrollDtl["Tpd_PDSPLHOLAmt"]              = 0;
                                            drEmpPayrollDtl["Tpd_PDCOMPHOLAmt"]             = 0;
                                            drEmpPayrollDtl["Tpd_PDPSDAmt"]                 = 0;
                                            drEmpPayrollDtl["Tpd_PDOTHHOLAmt"]              = 0;
                                            drEmpPayrollDtl["Tpd_PDRESTLEGHOLAmt"]          = 0;
                                            drEmpPayrollDtl["Tpd_TotalREGAmt"]              = 0;

                                            drEmpPayrollDtl["Tpd_REGOTAmt"]                 = 0;
                                            drEmpPayrollDtl["Tpd_REGNDAmt"]                 = 0;
                                            drEmpPayrollDtl["Tpd_REGNDOTAmt"]               = 0;

                                            drEmpPayrollDtl["Tpd_RESTAmt"]                  = 0;
                                            drEmpPayrollDtl["Tpd_RESTOTAmt"]                = 0;
                                            drEmpPayrollDtl["Tpd_RESTNDAmt"]                = 0;
                                            drEmpPayrollDtl["Tpd_RESTNDOTAmt"]              = 0;

                                            drEmpPayrollDtl["Tpd_LEGHOLAmt"]                = 0;
                                            drEmpPayrollDtl["Tpd_LEGHOLOTAmt"]              = 0;
                                            drEmpPayrollDtl["Tpd_LEGHOLNDAmt"]              = 0;
                                            drEmpPayrollDtl["Tpd_LEGHOLNDOTAmt"]            = 0;

                                            drEmpPayrollDtl["Tpd_SPLHOLAmt"]                = 0;
                                            drEmpPayrollDtl["Tpd_SPLHOLOTAmt"]              = 0;
                                            drEmpPayrollDtl["Tpd_SPLHOLNDAmt"]              = 0;
                                            drEmpPayrollDtl["Tpd_SPLHOLNDOTAmt"]            = 0;

                                            drEmpPayrollDtl["Tpd_PSDAmt"]                   = 0;
                                            drEmpPayrollDtl["Tpd_PSDOTAmt"]                 = 0;
                                            drEmpPayrollDtl["Tpd_PSDNDAmt"]                 = 0;
                                            drEmpPayrollDtl["Tpd_PSDNDOTAmt"]               = 0;

                                            drEmpPayrollDtl["Tpd_COMPHOLAmt"]               = 0;
                                            drEmpPayrollDtl["Tpd_COMPHOLOTAmt"]             = 0;
                                            drEmpPayrollDtl["Tpd_COMPHOLNDAmt"]             = 0;
                                            drEmpPayrollDtl["Tpd_COMPHOLNDOTAmt"]           = 0;

                                            drEmpPayrollDtl["Tpd_RESTLEGHOLAmt"]            = 0;
                                            drEmpPayrollDtl["Tpd_RESTLEGHOLOTAmt"]          = 0;
                                            drEmpPayrollDtl["Tpd_RESTLEGHOLNDAmt"]          = 0;
                                            drEmpPayrollDtl["Tpd_RESTLEGHOLNDOTAmt"]        = 0;

                                            drEmpPayrollDtl["Tpd_RESTSPLHOLAmt"]            = 0;
                                            drEmpPayrollDtl["Tpd_RESTSPLHOLOTAmt"]          = 0;
                                            drEmpPayrollDtl["Tpd_RESTSPLHOLNDAmt"]          = 0;
                                            drEmpPayrollDtl["Tpd_RESTSPLHOLNDOTAmt"]        = 0;

                                            drEmpPayrollDtl["Tpd_RESTCOMPHOLAmt"]           = 0;
                                            drEmpPayrollDtl["Tpd_RESTCOMPHOLOTAmt"]         = 0;
                                            drEmpPayrollDtl["Tpd_RESTCOMPHOLNDAmt"]         = 0;
                                            drEmpPayrollDtl["Tpd_RESTCOMPHOLNDOTAmt"]       = 0;

                                            drEmpPayrollDtl["Tpd_RESTPSDAmt"]               = 0;
                                            drEmpPayrollDtl["Tpd_RESTPSDOTAmt"]             = 0;
                                            drEmpPayrollDtl["Tpd_RESTPSDNDAmt"]             = 0;
                                            drEmpPayrollDtl["Tpd_RESTPSDNDOTAmt"]           = 0;
                                            #endregion

                                            #region Initialize Payroll Dtl Calc Variance Amount
                                            drEmpPayrollDtl["Tpd_LTVarAmt"]             = 0;
                                            drEmpPayrollDtl["Tpd_UTVarAmt"]             = 0;
                                            drEmpPayrollDtl["Tpd_UPLVVarAmt"]           = 0;
                                            drEmpPayrollDtl["Tpd_ABSLEGHOLVarAmt"]      = 0;
                                            drEmpPayrollDtl["Tpd_ABSSPLHOLVarAmt"]      = 0;
                                            drEmpPayrollDtl["Tpd_ABSCOMPHOLVarAmt"]     = 0;
                                            drEmpPayrollDtl["Tpd_ABSPSDVarAmt"]         = 0;
                                            drEmpPayrollDtl["Tpd_ABSOTHHOLVarAmt"]      = 0;
                                            drEmpPayrollDtl["Tpd_WDABSVarAmt"]          = 0;
                                            drEmpPayrollDtl["Tpd_LTUTMaxVarAmt"]        = 0;
                                            drEmpPayrollDtl["Tpd_ABSVarAmt"]            = 0;
                                            drEmpPayrollDtl["Tpd_REGVarAmt"]            = 0;
                                            drEmpPayrollDtl["Tpd_PDLVVarAmt"]           = 0;
                                            drEmpPayrollDtl["Tpd_PDLEGHOLVarAmt"]       = 0;
                                            drEmpPayrollDtl["Tpd_PDSPLHOLVarAmt"]       = 0;
                                            drEmpPayrollDtl["Tpd_PDCOMPHOLVarAmt"]      = 0;
                                            drEmpPayrollDtl["Tpd_PDPSDVarAmt"]          = 0;
                                            drEmpPayrollDtl["Tpd_PDOTHHOLVarAmt"]       = 0;
                                            drEmpPayrollDtl["Tpd_PDRESTLEGHOLVarAmt"]   = 0;
                                            drEmpPayrollDtl["Tpd_TotalREGVarAmt"]       = 0;
                                            drEmpPayrollDtl["Tpd_REGOTVarAmt"]          = 0;
                                            drEmpPayrollDtl["Tpd_REGNDVarAmt"]          = 0;
                                            drEmpPayrollDtl["Tpd_REGNDOTVarAmt"]        = 0;
                                            drEmpPayrollDtl["Tpd_RESTVarAmt"]           = 0;
                                            drEmpPayrollDtl["Tpd_RESTOTVarAmt"]         = 0;
                                            drEmpPayrollDtl["Tpd_RESTNDVarAmt"]         = 0;
                                            drEmpPayrollDtl["Tpd_RESTNDOTVarAmt"]       = 0;
                                            drEmpPayrollDtl["Tpd_LEGHOLVarAmt"]         = 0;
                                            drEmpPayrollDtl["Tpd_LEGHOLOTVarAmt"]       = 0;
                                            drEmpPayrollDtl["Tpd_LEGHOLNDVarAmt"]       = 0;
                                            drEmpPayrollDtl["Tpd_LEGHOLNDOTVarAmt"]     = 0;
                                            drEmpPayrollDtl["Tpd_SPLHOLVarAmt"]         = 0;
                                            drEmpPayrollDtl["Tpd_SPLHOLOTVarAmt"]       = 0;
                                            drEmpPayrollDtl["Tpd_SPLHOLNDVarAmt"]       = 0;
                                            drEmpPayrollDtl["Tpd_SPLHOLNDOTVarAmt"]     = 0;
                                            drEmpPayrollDtl["Tpd_PSDVarAmt"]            = 0;
                                            drEmpPayrollDtl["Tpd_PSDOTVarAmt"]          = 0;
                                            drEmpPayrollDtl["Tpd_PSDNDVarAmt"]          = 0;
                                            drEmpPayrollDtl["Tpd_PSDNDOTVarAmt"]        = 0;
                                            drEmpPayrollDtl["Tpd_COMPHOLVarAmt"]        = 0;
                                            drEmpPayrollDtl["Tpd_COMPHOLOTVarAmt"]      = 0;
                                            drEmpPayrollDtl["Tpd_COMPHOLNDVarAmt"]      = 0;
                                            drEmpPayrollDtl["Tpd_COMPHOLNDOTVarAmt"]    = 0;
                                            drEmpPayrollDtl["Tpd_RESTLEGHOLVarAmt"]     = 0;
                                            drEmpPayrollDtl["Tpd_RESTLEGHOLOTVarAmt"]   = 0;
                                            drEmpPayrollDtl["Tpd_RESTLEGHOLNDVarAmt"]   = 0;
                                            drEmpPayrollDtl["Tpd_RESTLEGHOLNDOTVarAmt"] = 0;
                                            drEmpPayrollDtl["Tpd_RESTSPLHOLVarAmt"]     = 0;
                                            drEmpPayrollDtl["Tpd_RESTSPLHOLOTVarAmt"]   = 0;
                                            drEmpPayrollDtl["Tpd_RESTSPLHOLNDVarAmt"]   = 0;
                                            drEmpPayrollDtl["Tpd_RESTSPLHOLNDOTVarAmt"] = 0;
                                            drEmpPayrollDtl["Tpd_RESTCOMPHOLVarAmt"]    = 0;
                                            drEmpPayrollDtl["Tpd_RESTCOMPHOLOTVarAmt"]  = 0;
                                            drEmpPayrollDtl["Tpd_RESTCOMPHOLNDVarAmt"]  = 0;
                                            drEmpPayrollDtl["Tpd_RESTCOMPHOLNDOTVarAmt"] = 0;
                                            drEmpPayrollDtl["Tpd_RESTPSDVarAmt"]        = 0;
                                            drEmpPayrollDtl["Tpd_RESTPSDOTVarAmt"]      = 0;
                                            drEmpPayrollDtl["Tpd_RESTPSDNDVarAmt"]      = 0;
                                            drEmpPayrollDtl["Tpd_RESTPSDNDOTVarAmt"]    = 0;

                                            drEmpPayrollDtl["Tpd_RGRetroAmt"]           = 0;
                                            drEmpPayrollDtl["Tpd_OTRetroAmt"]           = 0;
                                            drEmpPayrollDtl["Tpd_NDRetroAmt"]           = 0;
                                            drEmpPayrollDtl["Tpd_HLRetroAmt"]           = 0;
                                            drEmpPayrollDtl["Tpd_LVRetroAmt"]           = 0;
                                            drEmpPayrollDtl["Tpd_TotalRetroAmt"]        = 0;
                                            #endregion

                                            #region Initiaize Group Premium Variables
                                            //Regular and Overtime Rates
                                            Reg = 0;
                                            RegOT = 0;
                                            Rest = 0;
                                            RestOT = 0;
                                            Hol = 0;
                                            HolOT = 0;
                                            SPL = 0;
                                            SPLOT = 0;
                                            PSD = 0;
                                            PSDOT = 0;
                                            Comp = 0;
                                            CompOT = 0;
                                            RestHol = 0;
                                            RestHolOT = 0;
                                            RestSPL = 0;
                                            RestSPLOT = 0;
                                            RestComp = 0;
                                            RestCompOT = 0;
                                            RestPSD = 0;
                                            RestPSDOT = 0;

                                            //Regular Night Premium and Overtime Night Premium Rates
                                            RegND = 0;
                                            RegOTND = 0;
                                            RestND = 0;
                                            RestOTND = 0;
                                            HolND = 0;
                                            HolOTND = 0;
                                            SPLND = 0;
                                            SPLOTND = 0;
                                            PSDND = 0;
                                            PSDOTND = 0;
                                            CompND = 0;
                                            CompOTND = 0;
                                            RestHolND = 0;
                                            RestHolOTND = 0;
                                            RestSPLND = 0;
                                            RestSPLOTND = 0;
                                            RestCompND = 0;
                                            RestCompOTND = 0;
                                            RestPSDND = 0;
                                            RestPSDOTND = 0;

                                            //Regular Night Premium and Overtime Night Premium Percentages
                                            RegNDPercent = 0;
                                            RegOTNDPercent = 0;
                                            RestNDPercent = 0;
                                            RestOTNDPercent = 0;
                                            HolNDPercent = 0;
                                            HolOTNDPercent = 0;
                                            SPLNDPercent = 0;
                                            SPLOTNDPercent = 0;
                                            PSDNDPercent = 0;
                                            PSDOTNDPercent = 0;
                                            CompNDPercent = 0;
                                            CompOTNDPercent = 0;
                                            RestHolNDPercent = 0;
                                            RestHolOTNDPercent = 0;
                                            RestSPLNDPercent = 0;
                                            RestSPLOTNDPercent = 0;
                                            RestCompNDPercent = 0;
                                            RestCompOTNDPercent = 0;
                                            RestPSDNDPercent = 0;
                                            RestPSDOTNDPercent = 0;

                                            #endregion

                                            #region Load Group Premiums
                                            if (PayrollType != "" && dtPayrollDtlForProcess.Rows[idtl]["Tpd_PremiumGrpCode"].ToString() != "")
                                            {
                                                GetPremium(dtDayCodePremiums, PayrollType, dtPayrollDtlForProcess.Rows[idtl]["Tpd_PremiumGrpCode"].ToString(), "REG", false, ref Reg, ref RegOT, ref RegND, ref RegOTND, ref RegNDPercent, ref RegOTNDPercent);
                                                GetPremium(dtDayCodePremiums, PayrollType, dtPayrollDtlForProcess.Rows[idtl]["Tpd_PremiumGrpCode"].ToString(), "REST", true, ref Rest, ref RestOT, ref RestND, ref RestOTND, ref RestNDPercent, ref RestOTNDPercent);
                                                GetPremium(dtDayCodePremiums, PayrollType, dtPayrollDtlForProcess.Rows[idtl]["Tpd_PremiumGrpCode"].ToString(), "SPL", false, ref SPL, ref SPLOT, ref SPLND, ref SPLOTND, ref SPLNDPercent, ref SPLOTNDPercent);
                                                GetPremium(dtDayCodePremiums, PayrollType, dtPayrollDtlForProcess.Rows[idtl]["Tpd_PremiumGrpCode"].ToString(), "HOL", false, ref Hol, ref HolOT, ref HolND, ref HolOTND, ref HolNDPercent, ref HolOTNDPercent);
                                                GetPremium(dtDayCodePremiums, PayrollType, dtPayrollDtlForProcess.Rows[idtl]["Tpd_PremiumGrpCode"].ToString(), "PSD", false, ref PSD, ref PSDOT, ref PSDND, ref PSDOTND, ref PSDNDPercent, ref PSDOTNDPercent);
                                                GetPremium(dtDayCodePremiums, PayrollType, dtPayrollDtlForProcess.Rows[idtl]["Tpd_PremiumGrpCode"].ToString(), "COMP", false, ref Comp, ref CompOT, ref CompND, ref CompOTND, ref CompNDPercent, ref CompOTNDPercent);
                                                GetPremium(dtDayCodePremiums, PayrollType, dtPayrollDtlForProcess.Rows[idtl]["Tpd_PremiumGrpCode"].ToString(), "SPL", true, ref RestSPL, ref RestSPLOT, ref RestSPLND, ref RestSPLOTND, ref RestSPLNDPercent, ref RestSPLOTNDPercent);
                                                GetPremium(dtDayCodePremiums, PayrollType, dtPayrollDtlForProcess.Rows[idtl]["Tpd_PremiumGrpCode"].ToString(), "HOL", true, ref RestHol, ref RestHolOT, ref RestHolND, ref RestHolOTND, ref RestHolNDPercent, ref RestHolOTNDPercent);
                                                GetPremium(dtDayCodePremiums, PayrollType, dtPayrollDtlForProcess.Rows[idtl]["Tpd_PremiumGrpCode"].ToString(), "PSD", true, ref RestPSD, ref RestPSDOT, ref RestPSDND, ref RestPSDOTND, ref RestPSDNDPercent, ref RestPSDOTNDPercent);
                                                GetPremium(dtDayCodePremiums, PayrollType, dtPayrollDtlForProcess.Rows[idtl]["Tpd_PremiumGrpCode"].ToString(), "COMP", true, ref RestComp, ref RestCompOT, ref RestCompND, ref RestCompOTND, ref RestCompNDPercent, ref RestCompOTNDPercent);
                                            }
                                            else
                                                throw new PayrollException("Cannot Load Day Premium Table");
                                            #endregion

                                            #region ND Bracket Rates (Sharp)
                                            if (NDBRCKTCNT == 2)
                                            {
                                                RegNDPercent = NP1_RATE;
                                                RegOTNDPercent = NP2_RATE;
                                                RestNDPercent = NP1_RATE;
                                                RestOTNDPercent = NP2_RATE;
                                                HolNDPercent = NP1_RATE;
                                                HolOTNDPercent = NP2_RATE;
                                                SPLNDPercent = NP1_RATE;
                                                SPLOTNDPercent = NP2_RATE;
                                                PSDNDPercent = NP1_RATE;
                                                PSDOTNDPercent = NP2_RATE;
                                                CompNDPercent = NP1_RATE;
                                                CompOTNDPercent = NP2_RATE;
                                                RestHolNDPercent = NP1_RATE;
                                                RestHolOTNDPercent = NP2_RATE;
                                                RestSPLNDPercent = NP1_RATE;
                                                RestSPLOTNDPercent = NP2_RATE;
                                                RestCompNDPercent = NP1_RATE;
                                                RestCompOTNDPercent = NP2_RATE;
                                                RestPSDNDPercent = NP1_RATE;
                                                RestPSDOTNDPercent = NP2_RATE;
                                            }

                                            #endregion

                                            #region Compute Hour Premiums

                                            #region Absent Amount Computation
                                            if (Convert.ToBoolean(PAYSPECIALRATE))
                                            {
                                                drEmpPayrollDtl["Tpd_LTAmt"]                = Convert.ToDecimal(drEmpPayrollDtl["Tpd_LTHr"]) * (LTRate == "S" ? SpecialHourlyRate : HourlyRate);
                                                drEmpPayrollDtl["Tpd_UTAmt"]                = Convert.ToDecimal(drEmpPayrollDtl["Tpd_UTHr"]) * (UTRate == "S" ? SpecialHourlyRate : HourlyRate);
                                                drEmpPayrollDtl["Tpd_UPLVAmt"]              = Convert.ToDecimal(drEmpPayrollDtl["Tpd_UPLVHr"]) * (UPLVRate == "S" ? SpecialHourlyRate : HourlyRate);
                                                drEmpPayrollDtl["Tpd_ABSLEGHOLAmt"]         = Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSLEGHOLHr"]) * (ABSLEGHOLRate == "S" ? SpecialHourlyRate : HourlyRate);
                                                drEmpPayrollDtl["Tpd_ABSSPLHOLAmt"]         = Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSSPLHOLHr"]) * (ABSSPLHOLRate == "S" ? SpecialHourlyRate : HourlyRate);
                                                drEmpPayrollDtl["Tpd_ABSCOMPHOLAmt"]        = Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSCOMPHOLHr"]) * (ABSCOMPHOLRate == "S" ? SpecialHourlyRate : HourlyRate);
                                                drEmpPayrollDtl["Tpd_ABSPSDAmt"]            = Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSPSDHr"]) * (ABSPSDRate == "S" ? SpecialHourlyRate : HourlyRate);
                                                drEmpPayrollDtl["Tpd_ABSOTHHOLAmt"]         = Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSOTHHOLHr"]) * (ABSOTHHOLRate == "S" ? SpecialHourlyRate : HourlyRate);
                                                drEmpPayrollDtl["Tpd_WDABSAmt"]             = Convert.ToDecimal(drEmpPayrollDtl["Tpd_WDABSHr"]) * (WDABSRate == "S" ? SpecialHourlyRate : HourlyRate);
                                                drEmpPayrollDtl["Tpd_LTUTMaxAmt"]           = Convert.ToDecimal(drEmpPayrollDtl["Tpd_LTUTMaxHr"]) * (LTUTMAXRate == "S" ? SpecialHourlyRate : HourlyRate);
                                            }
                                            else
                                            {
                                                drEmpPayrollDtl["Tpd_LTAmt"]                = Convert.ToDecimal(drEmpPayrollDtl["Tpd_LTHr"]) * HourlyRate;
                                                drEmpPayrollDtl["Tpd_UTAmt"]                = Convert.ToDecimal(drEmpPayrollDtl["Tpd_UTHr"]) * HourlyRate;
                                                drEmpPayrollDtl["Tpd_UPLVAmt"]              = Convert.ToDecimal(drEmpPayrollDtl["Tpd_UPLVHr"]) * HourlyRate;
                                                drEmpPayrollDtl["Tpd_ABSLEGHOLAmt"]         = Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSLEGHOLHr"]) * HourlyRate;
                                                drEmpPayrollDtl["Tpd_ABSSPLHOLAmt"]         = Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSSPLHOLHr"]) * HourlyRate;
                                                drEmpPayrollDtl["Tpd_ABSCOMPHOLAmt"]        = Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSCOMPHOLHr"]) * HourlyRate;
                                                drEmpPayrollDtl["Tpd_ABSPSDAmt"]            = Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSPSDHr"]) * HourlyRate;
                                                drEmpPayrollDtl["Tpd_ABSOTHHOLAmt"]         = Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSOTHHOLHr"]) * HourlyRate;
                                                drEmpPayrollDtl["Tpd_WDABSAmt"]             = Convert.ToDecimal(drEmpPayrollDtl["Tpd_WDABSHr"]) * HourlyRate;
                                                drEmpPayrollDtl["Tpd_LTUTMaxAmt"]           = Convert.ToDecimal(drEmpPayrollDtl["Tpd_LTUTMaxHr"]) * HourlyRate;
                                                //drEmpPayrollDtl["Tpd_ABSAmt"]               = Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSHr"]) * HourlyRate;
                                            }

                                            drEmpPayrollDtl["Tpd_ABSAmt"]                   = Convert.ToDecimal(drEmpPayrollDtl["Tpd_LTAmt"])
                                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_UTAmt"])
                                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_UPLVAmt"])
                                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_WDABSAmt"])
                                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_LTUTMaxAmt"]) 
                                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSLEGHOLAmt"])
                                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSSPLHOLAmt"])
                                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSCOMPHOLAmt"])
                                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSPSDAmt"])
                                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSOTHHOLAmt"]);


                                            LateAmt                                         += Convert.ToDecimal(drEmpPayrollDtl["Tpd_LTAmt"]);
                                            UndertimeAmt                                    += Convert.ToDecimal(drEmpPayrollDtl["Tpd_UTAmt"]);
                                            UnpaidLeaveAmt                                  += Convert.ToDecimal(drEmpPayrollDtl["Tpd_UPLVAmt"]);
                                            AbsentLegalHolidayAmt                           += Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSLEGHOLAmt"]);
                                            AbsentSpecialHolidayAmt                         += Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSSPLHOLAmt"]);
                                            AbsentCompanyHolidayAmt                         += Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSCOMPHOLAmt"]);
                                            AbsentPlantShutdownAmt                          += Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSPSDAmt"]);
                                            AbsentFillerHolidayAmt                          += Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSOTHHOLAmt"]);
                                            WholeDayAbsentAmt                               += Convert.ToDecimal(drEmpPayrollDtl["Tpd_WDABSAmt"]);
                                            LateUndertimeMaxAbsentAmt                       += Convert.ToDecimal(drEmpPayrollDtl["Tpd_LTUTMaxAmt"]);
                                            AbsentAmt                                       += Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSAmt"]);

                                            #endregion

                                            #region Regular Amount Computation

                                            if (Convert.ToBoolean(PAYSPECIALRATE))
                                            {
                                                if (strRegRule == "P")
                                                    drEmpPayrollDtl["Tpd_REGAmt"]           = Convert.ToDecimal(drEmpPayrollDtl["Tpd_REGHr"]) * (REGRate == "S" ? SpecialHourlyRate : HourlyRate);

                                                drEmpPayrollDtl["Tpd_PDLVAmt"]              = Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDLVHr"]) * (PDLVRate == "S" ? SpecialHourlyRate : HourlyRate);
                                                drEmpPayrollDtl["Tpd_PDLEGHOLAmt"]          = Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDLEGHOLHr"]) * (PDLEGHOLRate == "S" ? SpecialHourlyRate : HourlyRate);
                                                drEmpPayrollDtl["Tpd_PDSPLHOLAmt"]          = Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDSPLHOLHr"]) * (PDSPLHOLRate == "S" ? SpecialHourlyRate : HourlyRate);
                                                drEmpPayrollDtl["Tpd_PDCOMPHOLAmt"]         = Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDCOMPHOLHr"]) * (PDCOMPHOLRate == "S" ? SpecialHourlyRate : HourlyRate);
                                                drEmpPayrollDtl["Tpd_PDPSDAmt"]             = Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDPSDHr"]) * (PDPSDRate == "S" ? SpecialHourlyRate : HourlyRate);
                                                drEmpPayrollDtl["Tpd_PDOTHHOLAmt"]          = Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDOTHHOLHr"]) * (PDOTHHOLRate == "S" ? SpecialHourlyRate : HourlyRate);
                                                drEmpPayrollDtl["Tpd_PDRESTLEGHOLAmt"]      = Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDRESTLEGHOLHr"]) * (PDRESTLEGHOLRate == "S" ? SpecialHourlyRate : HourlyRate);
                                            }
                                            else
                                            {
                                                if (strRegRule == "P")
                                                    drEmpPayrollDtl["Tpd_REGAmt"]           = Convert.ToDecimal(drEmpPayrollDtl["Tpd_REGHr"]) * HourlyRate;

                                                drEmpPayrollDtl["Tpd_PDLVAmt"]              = Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDLVHr"]) * HourlyRate;
                                                drEmpPayrollDtl["Tpd_PDLEGHOLAmt"]          = Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDLEGHOLHr"]) * HourlyRate;
                                                drEmpPayrollDtl["Tpd_PDSPLHOLAmt"]          = Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDSPLHOLHr"]) * HourlyRate;
                                                drEmpPayrollDtl["Tpd_PDCOMPHOLAmt"]         = Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDCOMPHOLHr"]) * HourlyRate;
                                                drEmpPayrollDtl["Tpd_PDPSDAmt"]             = Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDPSDHr"]) * HourlyRate;
                                                drEmpPayrollDtl["Tpd_PDOTHHOLAmt"]          = Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDOTHHOLHr"]) * HourlyRate;
                                                drEmpPayrollDtl["Tpd_PDRESTLEGHOLAmt"]      = Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDRESTLEGHOLHr"]) * HourlyRate;
                                            }


                                            RegularAmt                                      += Convert.ToDecimal(drEmpPayrollDtl["Tpd_REGAmt"]);
                                            PaidLeaveAmt                                    += Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDLVAmt"]);

                                            PaidLegalHolidayAmt                             += Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDLEGHOLAmt"]);
                                            PaidSpecialHolidayAmt                           += Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDSPLHOLAmt"]);
                                            PaidCompanyHolidayAmt                           += Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDCOMPHOLAmt"]);
                                            PaidPlantShutdownHolidayAmt                     += Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDPSDAmt"]);
                                            PaidFillerHolidayAmt                            += Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDOTHHOLAmt"]);
                                            PaidRestdayLegalHolidayAmt                      += Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDRESTLEGHOLAmt"]);

                                            if (strRegRule == "P")
                                                drEmpPayrollDtl["Tpd_TotalREGAmt"]          = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_REGAmt"])
                                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDLVAmt"])
                                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDLEGHOLAmt"])
                                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDSPLHOLAmt"])
                                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDCOMPHOLAmt"])
                                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDOTHHOLAmt"])
                                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDRESTLEGHOLAmt"]), 2);

                                            
                                            #endregion

                                            #region Overtime Amount Computation
                                            if (Convert.ToBoolean(PAYSPECIALRATE))
                                            {
                                                drEmpPayrollDtl["Tpd_REGOTAmt"]             = Convert.ToDecimal(drEmpPayrollDtl["Tpd_REGOTHr"]) * (REGRate == "S" ? SpecialHourlyRate : HourlyRate) * RegOT;

                                                drEmpPayrollDtl["Tpd_REGNDAmt"]             = Convert.ToDecimal(drEmpPayrollDtl["Tpd_REGNDHr"]) * (REGRate == "S" ? SpecialHourlyRate : HourlyRate) * RegND * RegNDPercent;
                                                drEmpPayrollDtl["Tpd_REGNDOTAmt"]           = Convert.ToDecimal(drEmpPayrollDtl["Tpd_REGNDOTHr"]) * (REGRate == "S" ? SpecialHourlyRate : HourlyRate) * RegOTND * RegOTNDPercent;

                                                drEmpPayrollDtl["Tpd_RESTAmt"]              = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTHr"]) * (RESTRate == "S" ? SpecialHourlyRate : HourlyRate) * Rest;
                                                drEmpPayrollDtl["Tpd_RESTOTAmt"]            = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTOTHr"]) * (RESTRate == "S" ? SpecialHourlyRate : HourlyRate) * RestOT;
                                                drEmpPayrollDtl["Tpd_RESTNDAmt"]            = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTNDHr"]) * (RESTRate == "S" ? SpecialHourlyRate : HourlyRate) * RestND * RestNDPercent;
                                                drEmpPayrollDtl["Tpd_RESTNDOTAmt"]          = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTNDOTHr"]) * (RESTRate == "S" ? SpecialHourlyRate : HourlyRate) * RestOTND * RestOTNDPercent;

                                                drEmpPayrollDtl["Tpd_LEGHOLAmt"]            = Convert.ToDecimal(drEmpPayrollDtl["Tpd_LEGHOLHr"]) * (LEGHOLRate == "S" ? SpecialHourlyRate : HourlyRate) * Hol;
                                                drEmpPayrollDtl["Tpd_LEGHOLOTAmt"]          = Convert.ToDecimal(drEmpPayrollDtl["Tpd_LEGHOLOTHr"]) * (LEGHOLRate == "S" ? SpecialHourlyRate : HourlyRate) * HolOT;
                                                drEmpPayrollDtl["Tpd_LEGHOLNDAmt"]          = Convert.ToDecimal(drEmpPayrollDtl["Tpd_LEGHOLNDHr"]) * (LEGHOLRate == "S" ? SpecialHourlyRate : HourlyRate) * HolND * HolNDPercent;
                                                drEmpPayrollDtl["Tpd_LEGHOLNDOTAmt"]        = Convert.ToDecimal(drEmpPayrollDtl["Tpd_LEGHOLNDOTHr"]) * (LEGHOLRate == "S" ? SpecialHourlyRate : HourlyRate) * HolOTND * HolOTNDPercent;

                                                drEmpPayrollDtl["Tpd_SPLHOLAmt"]            = Convert.ToDecimal(drEmpPayrollDtl["Tpd_SPLHOLHr"]) * (SPLHOLRate == "S" ? SpecialHourlyRate : HourlyRate) * SPL;
                                                drEmpPayrollDtl["Tpd_SPLHOLOTAmt"]          = Convert.ToDecimal(drEmpPayrollDtl["Tpd_SPLHOLOTHr"]) * (SPLHOLRate == "S" ? SpecialHourlyRate : HourlyRate) * SPLOT;
                                                drEmpPayrollDtl["Tpd_SPLHOLNDAmt"]          = Convert.ToDecimal(drEmpPayrollDtl["Tpd_SPLHOLNDHr"]) * (SPLHOLRate == "S" ? SpecialHourlyRate : HourlyRate) * SPLND * SPLNDPercent;
                                                drEmpPayrollDtl["Tpd_SPLHOLNDOTAmt"]        = Convert.ToDecimal(drEmpPayrollDtl["Tpd_SPLHOLNDOTHr"]) * (SPLHOLRate == "S" ? SpecialHourlyRate : HourlyRate) * SPLOTND * SPLOTNDPercent;

                                                drEmpPayrollDtl["Tpd_PSDAmt"]               = Convert.ToDecimal(drEmpPayrollDtl["Tpd_PSDHr"]) * (PSDRate == "S" ? SpecialHourlyRate : HourlyRate) * PSD;
                                                drEmpPayrollDtl["Tpd_PSDOTAmt"]             = Convert.ToDecimal(drEmpPayrollDtl["Tpd_PSDOTHr"]) * (PSDRate == "S" ? SpecialHourlyRate : HourlyRate) * PSDOT;
                                                drEmpPayrollDtl["Tpd_PSDNDAmt"]             = Convert.ToDecimal(drEmpPayrollDtl["Tpd_PSDNDHr"]) * (PSDRate == "S" ? SpecialHourlyRate : HourlyRate) * PSDND * PSDNDPercent;
                                                drEmpPayrollDtl["Tpd_PSDNDOTAmt"]           = Convert.ToDecimal(drEmpPayrollDtl["Tpd_PSDNDOTHr"]) * (PSDRate == "S" ? SpecialHourlyRate : HourlyRate) * PSDOTND * PSDOTNDPercent;

                                                drEmpPayrollDtl["Tpd_COMPHOLAmt"]           = Convert.ToDecimal(drEmpPayrollDtl["Tpd_COMPHOLHr"]) * (COMPHOLRate == "S" ? SpecialHourlyRate : HourlyRate) * Comp;
                                                drEmpPayrollDtl["Tpd_COMPHOLOTAmt"]         = Convert.ToDecimal(drEmpPayrollDtl["Tpd_COMPHOLOTHr"]) * (COMPHOLRate == "S" ? SpecialHourlyRate : HourlyRate) * CompOT;
                                                drEmpPayrollDtl["Tpd_COMPHOLNDAmt"]         = Convert.ToDecimal(drEmpPayrollDtl["Tpd_COMPHOLNDHr"]) * (COMPHOLRate == "S" ? SpecialHourlyRate : HourlyRate) * CompND * CompNDPercent;
                                                drEmpPayrollDtl["Tpd_COMPHOLNDOTAmt"]       = Convert.ToDecimal(drEmpPayrollDtl["Tpd_COMPHOLNDOTHr"]) * (COMPHOLRate == "S" ? SpecialHourlyRate : HourlyRate) * CompOTND * CompOTNDPercent;

                                                drEmpPayrollDtl["Tpd_RESTLEGHOLAmt"]        = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTLEGHOLHr"]) * (RESTLEGHOLRate == "S" ? SpecialHourlyRate : HourlyRate) * RestHol;
                                                drEmpPayrollDtl["Tpd_RESTLEGHOLOTAmt"]      = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTLEGHOLOTHr"]) * (RESTLEGHOLRate == "S" ? SpecialHourlyRate : HourlyRate) * RestHolOT;
                                                drEmpPayrollDtl["Tpd_RESTLEGHOLNDAmt"]      = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTLEGHOLNDHr"]) * (RESTLEGHOLRate == "S" ? SpecialHourlyRate : HourlyRate) * RestHolND * RestHolNDPercent;
                                                drEmpPayrollDtl["Tpd_RESTLEGHOLNDOTAmt"]    = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTLEGHOLNDOTHr"]) * (RESTLEGHOLRate == "S" ? SpecialHourlyRate : HourlyRate) * RestHolOTND * RestHolOTNDPercent;

                                                drEmpPayrollDtl["Tpd_RESTSPLHOLAmt"]        = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTSPLHOLHr"]) * (RESTSPLHOLRate == "S" ? SpecialHourlyRate : HourlyRate) * RestSPL;
                                                drEmpPayrollDtl["Tpd_RESTSPLHOLOTAmt"]      = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTSPLHOLOTHr"]) * (RESTSPLHOLRate == "S" ? SpecialHourlyRate : HourlyRate) * RestSPLOT;
                                                drEmpPayrollDtl["Tpd_RESTSPLHOLNDAmt"]      = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTSPLHOLNDHr"]) * (RESTSPLHOLRate == "S" ? SpecialHourlyRate : HourlyRate) * RestSPLND * RestSPLNDPercent;
                                                drEmpPayrollDtl["Tpd_RESTSPLHOLNDOTAmt"]    = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTSPLHOLNDOTHr"]) * (RESTSPLHOLRate == "S" ? SpecialHourlyRate : HourlyRate) * RestSPLOTND * RestSPLOTNDPercent;

                                                drEmpPayrollDtl["Tpd_RESTCOMPHOLAmt"]       = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTCOMPHOLHr"]) * (RESTCOMPHOLRate == "S" ? SpecialHourlyRate : HourlyRate) * RestComp;
                                                drEmpPayrollDtl["Tpd_RESTCOMPHOLOTAmt"]     = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTCOMPHOLOTHr"]) * (RESTCOMPHOLRate == "S" ? SpecialHourlyRate : HourlyRate) * RestCompOT;
                                                drEmpPayrollDtl["Tpd_RESTCOMPHOLNDAmt"]     = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTCOMPHOLNDHr"]) * (RESTCOMPHOLRate == "S" ? SpecialHourlyRate : HourlyRate) * RestCompND * RestCompNDPercent;
                                                drEmpPayrollDtl["Tpd_RESTCOMPHOLNDOTAmt"]   = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTCOMPHOLNDOTHr"]) * (RESTCOMPHOLRate == "S" ? SpecialHourlyRate : HourlyRate) * RestCompOTND * RestCompOTNDPercent;

                                                drEmpPayrollDtl["Tpd_RESTPSDAmt"]           = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTPSDHr"]) * (RESTPSDRate == "S" ? SpecialHourlyRate : HourlyRate) * RestPSD;
                                                drEmpPayrollDtl["Tpd_RESTPSDOTAmt"]         = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTPSDOTHr"]) * (RESTPSDRate == "S" ? SpecialHourlyRate : HourlyRate) * RestPSDOT;
                                                drEmpPayrollDtl["Tpd_RESTPSDNDAmt"]         = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTPSDNDHr"]) * (RESTPSDRate == "S" ? SpecialHourlyRate : HourlyRate) * RestPSDND * RestPSDNDPercent;
                                                drEmpPayrollDtl["Tpd_RESTPSDNDOTAmt"]       = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTPSDNDOTHr"]) * (RESTPSDRate == "S" ? SpecialHourlyRate : HourlyRate) * RestPSDOTND * RestPSDOTNDPercent;
                                            }
                                            else
                                            {
                                                drEmpPayrollDtl["Tpd_REGOTAmt"]             = Convert.ToDecimal(drEmpPayrollDtl["Tpd_REGOTHr"]) * HourlyRate * RegOT;

                                                drEmpPayrollDtl["Tpd_REGNDAmt"]             = Convert.ToDecimal(drEmpPayrollDtl["Tpd_REGNDHr"]) * HourlyRate * RegND * RegNDPercent;
                                                drEmpPayrollDtl["Tpd_REGNDOTAmt"]           = Convert.ToDecimal(drEmpPayrollDtl["Tpd_REGNDOTHr"]) * HourlyRate * RegOTND * RegOTNDPercent;

                                                drEmpPayrollDtl["Tpd_RESTAmt"]              = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTHr"]) * HourlyRate * Rest;
                                                drEmpPayrollDtl["Tpd_RESTOTAmt"]            = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTOTHr"]) * HourlyRate * RestOT;
                                                drEmpPayrollDtl["Tpd_RESTNDAmt"]            = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTNDHr"]) * HourlyRate * RestND * RestNDPercent;
                                                drEmpPayrollDtl["Tpd_RESTNDOTAmt"]          = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTNDOTHr"]) * HourlyRate * RestOTND * RestOTNDPercent;

                                                drEmpPayrollDtl["Tpd_LEGHOLAmt"]            = Convert.ToDecimal(drEmpPayrollDtl["Tpd_LEGHOLHr"]) * HourlyRate * Hol;
                                                drEmpPayrollDtl["Tpd_LEGHOLOTAmt"]          = Convert.ToDecimal(drEmpPayrollDtl["Tpd_LEGHOLOTHr"]) * HourlyRate * HolOT;
                                                drEmpPayrollDtl["Tpd_LEGHOLNDAmt"]          = Convert.ToDecimal(drEmpPayrollDtl["Tpd_LEGHOLNDHr"]) * HourlyRate * HolND * HolNDPercent;
                                                drEmpPayrollDtl["Tpd_LEGHOLNDOTAmt"]        = Convert.ToDecimal(drEmpPayrollDtl["Tpd_LEGHOLNDOTHr"]) * HourlyRate * HolOTND * HolOTNDPercent;

                                                drEmpPayrollDtl["Tpd_SPLHOLAmt"]            = Convert.ToDecimal(drEmpPayrollDtl["Tpd_SPLHOLHr"]) * HourlyRate * SPL;
                                                drEmpPayrollDtl["Tpd_SPLHOLOTAmt"]          = Convert.ToDecimal(drEmpPayrollDtl["Tpd_SPLHOLOTHr"]) * HourlyRate * SPLOT;
                                                drEmpPayrollDtl["Tpd_SPLHOLNDAmt"]          = Convert.ToDecimal(drEmpPayrollDtl["Tpd_SPLHOLNDHr"]) * HourlyRate * SPLND * SPLNDPercent;
                                                drEmpPayrollDtl["Tpd_SPLHOLNDOTAmt"]        = Convert.ToDecimal(drEmpPayrollDtl["Tpd_SPLHOLNDOTHr"]) * HourlyRate * SPLOTND * SPLOTNDPercent;

                                                drEmpPayrollDtl["Tpd_PSDAmt"]               = Convert.ToDecimal(drEmpPayrollDtl["Tpd_PSDHr"]) * HourlyRate * PSD;
                                                drEmpPayrollDtl["Tpd_PSDOTAmt"]             = Convert.ToDecimal(drEmpPayrollDtl["Tpd_PSDOTHr"]) * HourlyRate * PSDOT;
                                                drEmpPayrollDtl["Tpd_PSDNDAmt"]             = Convert.ToDecimal(drEmpPayrollDtl["Tpd_PSDNDHr"]) * HourlyRate * PSDND * PSDNDPercent;
                                                drEmpPayrollDtl["Tpd_PSDNDOTAmt"]           = Convert.ToDecimal(drEmpPayrollDtl["Tpd_PSDNDOTHr"]) * HourlyRate * PSDOTND * PSDOTNDPercent;

                                                drEmpPayrollDtl["Tpd_COMPHOLAmt"]           = Convert.ToDecimal(drEmpPayrollDtl["Tpd_COMPHOLHr"]) * HourlyRate * Comp;
                                                drEmpPayrollDtl["Tpd_COMPHOLOTAmt"]         = Convert.ToDecimal(drEmpPayrollDtl["Tpd_COMPHOLOTHr"]) * HourlyRate * CompOT;
                                                drEmpPayrollDtl["Tpd_COMPHOLNDAmt"]         = Convert.ToDecimal(drEmpPayrollDtl["Tpd_COMPHOLNDHr"]) * HourlyRate * CompND * CompNDPercent;
                                                drEmpPayrollDtl["Tpd_COMPHOLNDOTAmt"]       = Convert.ToDecimal(drEmpPayrollDtl["Tpd_COMPHOLNDOTHr"]) * HourlyRate * CompOTND * CompOTNDPercent;

                                                drEmpPayrollDtl["Tpd_RESTLEGHOLAmt"]        = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTLEGHOLHr"]) * HourlyRate * RestHol;
                                                drEmpPayrollDtl["Tpd_RESTLEGHOLOTAmt"]      = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTLEGHOLOTHr"]) * HourlyRate * RestHolOT;
                                                drEmpPayrollDtl["Tpd_RESTLEGHOLNDAmt"]      = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTLEGHOLNDHr"]) * HourlyRate * RestHolND * RestHolNDPercent;
                                                drEmpPayrollDtl["Tpd_RESTLEGHOLNDOTAmt"]    = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTLEGHOLNDOTHr"]) * HourlyRate * RestHolOTND * RestHolOTNDPercent;

                                                drEmpPayrollDtl["Tpd_RESTSPLHOLAmt"]        = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTSPLHOLHr"]) * HourlyRate * RestSPL;
                                                drEmpPayrollDtl["Tpd_RESTSPLHOLOTAmt"]      = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTSPLHOLOTHr"]) * HourlyRate * RestSPLOT;
                                                drEmpPayrollDtl["Tpd_RESTSPLHOLNDAmt"]      = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTSPLHOLNDHr"]) * HourlyRate * RestSPLND * RestSPLNDPercent;
                                                drEmpPayrollDtl["Tpd_RESTSPLHOLNDOTAmt"]    = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTSPLHOLNDOTHr"]) * HourlyRate * RestSPLOTND * RestSPLOTNDPercent;

                                                drEmpPayrollDtl["Tpd_RESTCOMPHOLAmt"]       = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTCOMPHOLHr"]) * HourlyRate * RestComp;
                                                drEmpPayrollDtl["Tpd_RESTCOMPHOLOTAmt"]     = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTCOMPHOLOTHr"]) * HourlyRate * RestCompOT;
                                                drEmpPayrollDtl["Tpd_RESTCOMPHOLNDAmt"]     = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTCOMPHOLNDHr"]) * HourlyRate * RestCompND * RestCompNDPercent;
                                                drEmpPayrollDtl["Tpd_RESTCOMPHOLNDOTAmt"]   = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTCOMPHOLNDOTHr"]) * HourlyRate * RestCompOTND * RestCompOTNDPercent;

                                                drEmpPayrollDtl["Tpd_RESTPSDAmt"]           = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTPSDHr"]) * HourlyRate * RestPSD;
                                                drEmpPayrollDtl["Tpd_RESTPSDOTAmt"]         = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTPSDOTHr"]) * HourlyRate * RestPSDOT;
                                                drEmpPayrollDtl["Tpd_RESTPSDNDAmt"]         = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTPSDNDHr"]) * HourlyRate * RestPSDND * RestPSDNDPercent;
                                                drEmpPayrollDtl["Tpd_RESTPSDNDOTAmt"]       = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTPSDNDOTHr"]) * HourlyRate * RestPSDOTND * RestPSDOTNDPercent;
                                            }

                                            RegularOTAmt                                    += Convert.ToDecimal(drEmpPayrollDtl["Tpd_REGOTAmt"]);
                                            RegularNDAmt                                    += Convert.ToDecimal(drEmpPayrollDtl["Tpd_REGNDAmt"]);
                                            RegularOTNDAmt                                  += Convert.ToDecimal(drEmpPayrollDtl["Tpd_REGNDOTAmt"]);

                                            RestdayAmt                                      += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTAmt"]);
                                            RestdayOTAmt                                    += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTOTAmt"]);
                                            RestdayNDAmt                                    += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTNDAmt"]);
                                            RestdayOTNDAmt                                  += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTNDOTAmt"]);

                                            LegalHolidayAmt                                 += Convert.ToDecimal(drEmpPayrollDtl["Tpd_LEGHOLAmt"]);
                                            LegalHolidayOTAmt                               += Convert.ToDecimal(drEmpPayrollDtl["Tpd_LEGHOLOTAmt"]);
                                            LegalHolidayNDAmt                               += Convert.ToDecimal(drEmpPayrollDtl["Tpd_LEGHOLNDAmt"]);
                                            LegalHolidayOTNDAmt                             += Convert.ToDecimal(drEmpPayrollDtl["Tpd_LEGHOLNDOTAmt"]);

                                            SpecialHolidayAmt                               += Convert.ToDecimal(drEmpPayrollDtl["Tpd_SPLHOLAmt"]);
                                            SpecialHolidayOTAmt                             += Convert.ToDecimal(drEmpPayrollDtl["Tpd_SPLHOLOTAmt"]);
                                            SpecialHolidayNDAmt                             += Convert.ToDecimal(drEmpPayrollDtl["Tpd_SPLHOLNDAmt"]);
                                            SpecialHolidayOTNDAmt                           += Convert.ToDecimal(drEmpPayrollDtl["Tpd_SPLHOLNDOTAmt"]);

                                            PlantShutdownAmt                                += Convert.ToDecimal(drEmpPayrollDtl["Tpd_PSDAmt"]);
                                            PlantShutdownOTAmt                              += Convert.ToDecimal(drEmpPayrollDtl["Tpd_PSDOTAmt"]);
                                            PlantShutdownNDAmt                              += Convert.ToDecimal(drEmpPayrollDtl["Tpd_PSDNDAmt"]);
                                            PlantShutdownOTNDAmt                            += Convert.ToDecimal(drEmpPayrollDtl["Tpd_PSDNDOTAmt"]);

                                            CompanyHolidayAmt                               += Convert.ToDecimal(drEmpPayrollDtl["Tpd_COMPHOLAmt"]);
                                            CompanyHolidayOTAmt                             += Convert.ToDecimal(drEmpPayrollDtl["Tpd_COMPHOLOTAmt"]);
                                            CompanyHolidayNDAmt                             += Convert.ToDecimal(drEmpPayrollDtl["Tpd_COMPHOLNDAmt"]);
                                            CompanyHolidayOTNDAmt                           += Convert.ToDecimal(drEmpPayrollDtl["Tpd_COMPHOLNDOTAmt"]);

                                            RestdayLegalHolidayAmt                          += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTLEGHOLAmt"]);
                                            RestdayLegalHolidayOTAmt                        += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTLEGHOLOTAmt"]);
                                            RestdayLegalHolidayNDAmt                        += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTLEGHOLNDAmt"]);
                                            RestdayLegalHolidayOTNDAmt                      += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTLEGHOLNDOTAmt"]);

                                            RestdaySpecialHolidayAmt                        += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTSPLHOLAmt"]);
                                            RestdaySpecialHolidayOTAmt                      += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTSPLHOLOTAmt"]);
                                            RestdaySpecialHolidayNDAmt                      += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTSPLHOLNDAmt"]);
                                            RestdaySpecialHolidayOTNDAmt                    += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTSPLHOLNDOTAmt"]);

                                            RestdayCompanyHolidayAmt                        += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTCOMPHOLAmt"]);
                                            RestdayCompanyHolidayOTAmt                      += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTCOMPHOLOTAmt"]);
                                            RestdayCompanyHolidayNDAmt                      += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTCOMPHOLNDAmt"]);
                                            RestdayCompanyHolidayOTNDAmt                    += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTCOMPHOLNDOTAmt"]);

                                            RestdayPlantShutdownAmt                         += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTPSDAmt"]);
                                            RestdayPlantShutdownOTAmt                       += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTPSDOTAmt"]);
                                            RestdayPlantShutdownNDAmt                       += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTPSDNDAmt"]);
                                            RestdayPlantShutdownOTNDAmt                     += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTPSDNDOTAmt"]);
                                            #endregion

                                            #region Payroll Dtl Misc
                                            if (bHasDayCodeExt)
                                            {
                                                #region Initialize Payroll Dtl Misc Calc Amount
                                                drEmpPayrollDtlMisc["Tpm_Misc1Amt"]         = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc1OTAmt"]       = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc1NDAmt"]       = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc1NDOTAmt"]     = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc2Amt"]         = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc2OTAmt"]       = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc2NDAmt"]       = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc2NDOTAmt"]     = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc3Amt"]         = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc3OTAmt"]       = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc3NDAmt"]       = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc3NDOTAmt"]     = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc4Amt"]         = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc4OTAmt"]       = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc4NDAmt"]       = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc4NDOTAmt"]     = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc5Amt"]         = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc5OTAmt"]       = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc5NDAmt"]       = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc5NDOTAmt"]     = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc6Amt"]         = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc6OTAmt"]       = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc6NDAmt"]       = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc6NDOTAmt"]     = 0;
                                                #endregion

                                                #region Initialize Payroll Dtl Misc Calc Variance Amount
                                                drEmpPayrollDtlMisc["Tpm_Misc1VarAmt"]      = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc1OTVarAmt"]    = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc1NDVarAmt"]    = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc1NDOTVarAmt"]  = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc2VarAmt"]      = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc2OTVarAmt"]    = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc2NDVarAmt"]    = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc2NDOTVarAmt"]  = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc3VarAmt"]      = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc3OTVarAmt"]    = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc3NDVarAmt"]    = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc3NDOTVarAmt"]  = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc4VarAmt"]      = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc4OTVarAmt"]    = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc4NDVarAmt"]    = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc4NDOTVarAmt"]  = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc5VarAmt"]      = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc5OTVarAmt"]    = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc5NDVarAmt"]    = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc5NDOTVarAmt"]  = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc6VarAmt"]      = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc6OTVarAmt"]    = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc6NDVarAmt"]    = 0;
                                                drEmpPayrollDtlMisc["Tpm_Misc6NDOTVarAmt"]  = 0;
                                                #endregion

                                                #region Misc Amount Computation
                                                drDayCodePremiumFiller = dtDayCodePremiumFillers.Select(string.Format("Mdp_PremiumGrpCode = '{0}' AND Mdp_PayrollType = '{1}'", dtPayrollDtlForProcess.Rows[idtl]["Tpd_PremiumGrpCode"].ToString(), PayrollType));
                                                if (drDayCodePremiumFiller.Length > 0)
                                                {
                                                    for (int premFill = 0; premFill < drDayCodePremiumFiller.Length; premFill++)
                                                    {
                                                        fillerHrCol = string.Format("Tpm_Misc{0:0}Hr", drDayCodePremiumFiller[premFill]["Mmd_MiscDayID"]);
                                                        fillerOTHrCol = string.Format("Tpm_Misc{0:0}OTHr", drDayCodePremiumFiller[premFill]["Mmd_MiscDayID"]);
                                                        fillerNDHrCol = string.Format("Tpm_Misc{0:0}NDHr", drDayCodePremiumFiller[premFill]["Mmd_MiscDayID"]);
                                                        fillerOTNDHrCol = string.Format("Tpm_Misc{0:0}NDOTHr", drDayCodePremiumFiller[premFill]["Mmd_MiscDayID"]);
                                                        fillerAmtCol = string.Format("Tpm_Misc{0:0}Amt", drDayCodePremiumFiller[premFill]["Mmd_MiscDayID"]);
                                                        fillerOTAmtCol = string.Format("Tpm_Misc{0:0}OTAmt", drDayCodePremiumFiller[premFill]["Mmd_MiscDayID"]);
                                                        fillerNDAmtCol = string.Format("Tpm_Misc{0:0}NDAmt", drDayCodePremiumFiller[premFill]["Mmd_MiscDayID"]);
                                                        fillerOTNDAmtCol = string.Format("Tpm_Misc{0:0}NDOTAmt", drDayCodePremiumFiller[premFill]["Mmd_MiscDayID"]);

                                                        if (Convert.ToBoolean(PAYSPECIALRATE))
                                                        {
                                                            switch (GetValue(drDayCodePremiumFiller[premFill]["Mmd_MiscDayID"]))
                                                            {
                                                                case "1":
                                                                    drEmpPayrollDtlMisc[fillerAmtCol] = Convert.ToDecimal(drEmpPayrollDtlMisc[fillerHrCol]) * (Misc1Rate == "S" ? SpecialHourlyRate : HourlyRate) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_RGPremiumRate"]) / 100);
                                                                    drEmpPayrollDtlMisc[fillerOTAmtCol] = Convert.ToDecimal(drEmpPayrollDtlMisc[fillerOTHrCol]) * (Misc1Rate == "S" ? SpecialHourlyRate : HourlyRate) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_OTPremiumRate"]) / 100);
                                                                    drEmpPayrollDtlMisc[fillerNDAmtCol] = Convert.ToDecimal(drEmpPayrollDtlMisc[fillerNDHrCol]) * (Misc1Rate == "S" ? SpecialHourlyRate : HourlyRate) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_RGNDPremiumRate"]) / 100) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_RGNDPercentageRate"]) / 100);
                                                                    drEmpPayrollDtlMisc[fillerOTNDAmtCol] = Convert.ToDecimal(drEmpPayrollDtlMisc[fillerOTNDHrCol]) * (Misc1Rate == "S" ? SpecialHourlyRate : HourlyRate) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_OTNDPremiumRate"]) / 100) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_OTNDPercentageRate"]) / 100);
                                                                    break;
                                                                case "2":
                                                                    drEmpPayrollDtlMisc[fillerAmtCol] = Convert.ToDecimal(drEmpPayrollDtlMisc[fillerHrCol]) * (Misc2Rate == "S" ? SpecialHourlyRate : HourlyRate) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_RGPremiumRate"]) / 100);
                                                                    drEmpPayrollDtlMisc[fillerOTAmtCol] = Convert.ToDecimal(drEmpPayrollDtlMisc[fillerOTHrCol]) * (Misc2Rate == "S" ? SpecialHourlyRate : HourlyRate) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_OTPremiumRate"]) / 100);
                                                                    drEmpPayrollDtlMisc[fillerNDAmtCol] = Convert.ToDecimal(drEmpPayrollDtlMisc[fillerNDHrCol]) * (Misc2Rate == "S" ? SpecialHourlyRate : HourlyRate) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_RGNDPremiumRate"]) / 100) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_RGNDPercentageRate"]) / 100);
                                                                    drEmpPayrollDtlMisc[fillerOTNDAmtCol] = Convert.ToDecimal(drEmpPayrollDtlMisc[fillerOTNDHrCol]) * (Misc2Rate == "S" ? SpecialHourlyRate : HourlyRate) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_OTNDPremiumRate"]) / 100) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_OTNDPercentageRate"]) / 100);
                                                                    break;
                                                                case "3":
                                                                    drEmpPayrollDtlMisc[fillerAmtCol] = Convert.ToDecimal(drEmpPayrollDtlMisc[fillerHrCol]) * (Misc3Rate == "S" ? SpecialHourlyRate : HourlyRate) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_RGPremiumRate"]) / 100);
                                                                    drEmpPayrollDtlMisc[fillerOTAmtCol] = Convert.ToDecimal(drEmpPayrollDtlMisc[fillerOTHrCol]) * (Misc3Rate == "S" ? SpecialHourlyRate : HourlyRate) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_OTPremiumRate"]) / 100);
                                                                    drEmpPayrollDtlMisc[fillerNDAmtCol] = Convert.ToDecimal(drEmpPayrollDtlMisc[fillerNDHrCol]) * (Misc3Rate == "S" ? SpecialHourlyRate : HourlyRate) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_RGNDPremiumRate"]) / 100) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_RGNDPercentageRate"]) / 100);
                                                                    drEmpPayrollDtlMisc[fillerOTNDAmtCol] = Convert.ToDecimal(drEmpPayrollDtlMisc[fillerOTNDHrCol]) * (Misc3Rate == "S" ? SpecialHourlyRate : HourlyRate) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_OTNDPremiumRate"]) / 100) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_OTNDPercentageRate"]) / 100);
                                                                    break;
                                                                case "4":
                                                                    drEmpPayrollDtlMisc[fillerAmtCol] = Convert.ToDecimal(drEmpPayrollDtlMisc[fillerHrCol]) * (Misc4Rate == "S" ? SpecialHourlyRate : HourlyRate) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_RGPremiumRate"]) / 100);
                                                                    drEmpPayrollDtlMisc[fillerOTAmtCol] = Convert.ToDecimal(drEmpPayrollDtlMisc[fillerOTHrCol]) * (Misc4Rate == "S" ? SpecialHourlyRate : HourlyRate) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_OTPremiumRate"]) / 100);
                                                                    drEmpPayrollDtlMisc[fillerNDAmtCol] = Convert.ToDecimal(drEmpPayrollDtlMisc[fillerNDHrCol]) * (Misc4Rate == "S" ? SpecialHourlyRate : HourlyRate) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_RGNDPremiumRate"]) / 100) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_RGNDPercentageRate"]) / 100);
                                                                    drEmpPayrollDtlMisc[fillerOTNDAmtCol] = Convert.ToDecimal(drEmpPayrollDtlMisc[fillerOTNDHrCol]) * (Misc4Rate == "S" ? SpecialHourlyRate : HourlyRate) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_OTNDPremiumRate"]) / 100) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_OTNDPercentageRate"]) / 100);
                                                                    break;
                                                                case "5":
                                                                    drEmpPayrollDtlMisc[fillerAmtCol] = Convert.ToDecimal(drEmpPayrollDtlMisc[fillerHrCol]) * (Misc5Rate == "S" ? SpecialHourlyRate : HourlyRate) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_RGPremiumRate"]) / 100);
                                                                    drEmpPayrollDtlMisc[fillerOTAmtCol] = Convert.ToDecimal(drEmpPayrollDtlMisc[fillerOTHrCol]) * (Misc5Rate == "S" ? SpecialHourlyRate : HourlyRate) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_OTPremiumRate"]) / 100);
                                                                    drEmpPayrollDtlMisc[fillerNDAmtCol] = Convert.ToDecimal(drEmpPayrollDtlMisc[fillerNDHrCol]) * (Misc5Rate == "S" ? SpecialHourlyRate : HourlyRate) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_RGNDPremiumRate"]) / 100) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_RGNDPercentageRate"]) / 100);
                                                                    drEmpPayrollDtlMisc[fillerOTNDAmtCol] = Convert.ToDecimal(drEmpPayrollDtlMisc[fillerOTNDHrCol]) * (Misc5Rate == "S" ? SpecialHourlyRate : HourlyRate) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_OTNDPremiumRate"]) / 100) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_OTNDPercentageRate"]) / 100);
                                                                    break;
                                                                case "6":
                                                                    drEmpPayrollDtlMisc[fillerAmtCol] = Convert.ToDecimal(drEmpPayrollDtlMisc[fillerHrCol]) * (Misc6Rate == "S" ? SpecialHourlyRate : HourlyRate) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_RGPremiumRate"]) / 100);
                                                                    drEmpPayrollDtlMisc[fillerOTAmtCol] = Convert.ToDecimal(drEmpPayrollDtlMisc[fillerOTHrCol]) * (Misc6Rate == "S" ? SpecialHourlyRate : HourlyRate) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_OTPremiumRate"]) / 100);
                                                                    drEmpPayrollDtlMisc[fillerNDAmtCol] = Convert.ToDecimal(drEmpPayrollDtlMisc[fillerNDHrCol]) * (Misc6Rate == "S" ? SpecialHourlyRate : HourlyRate) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_RGNDPremiumRate"]) / 100) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_RGNDPercentageRate"]) / 100);
                                                                    drEmpPayrollDtlMisc[fillerOTNDAmtCol] = Convert.ToDecimal(drEmpPayrollDtlMisc[fillerOTNDHrCol]) * (Misc6Rate == "S" ? SpecialHourlyRate : HourlyRate) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_OTNDPremiumRate"]) / 100) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_OTNDPercentageRate"]) / 100);
                                                                    break;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            drEmpPayrollDtlMisc[fillerAmtCol] = Convert.ToDecimal(drEmpPayrollDtlMisc[fillerHrCol]) * HourlyRate * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_RGPremiumRate"]) / 100);
                                                            drEmpPayrollDtlMisc[fillerOTAmtCol] = Convert.ToDecimal(drEmpPayrollDtlMisc[fillerOTHrCol]) * HourlyRate * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_OTPremiumRate"]) / 100);
                                                            drEmpPayrollDtlMisc[fillerNDAmtCol] = Convert.ToDecimal(drEmpPayrollDtlMisc[fillerNDHrCol]) * HourlyRate * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_RGNDPremiumRate"]) / 100) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_RGNDPercentageRate"]) / 100);
                                                            drEmpPayrollDtlMisc[fillerOTNDAmtCol] = Convert.ToDecimal(drEmpPayrollDtlMisc[fillerOTNDHrCol]) * HourlyRate * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_OTNDPremiumRate"]) / 100) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_OTNDPercentageRate"]) / 100);
                                                        }
                                                    }
                                                }
                                                else
                                                    throw new PayrollException("Day Premium Group [" + dtPayrollDtlForProcess.Rows[idtl]["Tpd_PremiumGrpCode"].ToString() + "] has No Premium for " + PayrollType + " Payroll Type");

                                                #endregion

                                                #region Add To Total OT ND
                                                DataRow[] drTotalOT = dtEmpPayrollDtl.Select("Tpd_IDNo = '" + curEmployeeId + "' AND Tpd_Date = '" + drEmpPayrollDtlMisc["Tpm_Date"].ToString() + "'");
                                                if (drTotalOT.Length > 0)
                                                {
                                                    drTotalOT[0]["Tpd_TotalOTNDAmt"] = Convert.ToDecimal(drTotalOT[0]["Tpd_TotalOTNDAmt"])
                                                                                            + Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc1Amt"])
                                                                                            + Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc1OTAmt"])
                                                                                            + Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc1NDAmt"])
                                                                                            + Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc1NDOTAmt"])
                                                                                            + Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc2Amt"])
                                                                                            + Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc2OTAmt"])
                                                                                            + Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc2NDAmt"])
                                                                                            + Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc2NDOTAmt"])
                                                                                            + Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc3Amt"])
                                                                                            + Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc3OTAmt"])
                                                                                            + Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc3NDAmt"])
                                                                                            + Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc3NDOTAmt"])
                                                                                            + Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc4Amt"])
                                                                                            + Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc4OTAmt"])
                                                                                            + Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc4NDAmt"])
                                                                                            + Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc4NDOTAmt"])
                                                                                            + Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc5Amt"])
                                                                                            + Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc5OTAmt"])
                                                                                            + Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc5NDAmt"])
                                                                                            + Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc5NDOTAmt"])
                                                                                            + Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc6Amt"])
                                                                                            + Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc6OTAmt"])
                                                                                            + Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc6NDAmt"])
                                                                                            + Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc6NDOTAmt"]);
                                                    

                                                    #region Add Misc Amount to Hdr
                                                    Misc1Amt            += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc1Amt"]);
                                                    Misc1OTAmt          += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc1OTAmt"]);
                                                    Misc1NDAmt          += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc1NDAmt"]);
                                                    Misc1NDOTAmt        += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc1NDOTAmt"]);
                                                    Misc2Amt            += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc2Amt"]);
                                                    Misc2OTAmt          += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc2OTAmt"]);
                                                    Misc2NDAmt          += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc2NDAmt"]);
                                                    Misc2NDOTAmt        += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc2NDOTAmt"]);
                                                    Misc3Amt            += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc3Amt"]);
                                                    Misc3OTAmt          += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc3OTAmt"]);
                                                    Misc3NDAmt          += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc3NDAmt"]);
                                                    Misc3NDOTAmt        += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc3NDOTAmt"]);
                                                    Misc4Amt            += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc4Amt"]);
                                                    Misc4OTAmt          += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc4OTAmt"]);
                                                    Misc4NDAmt          += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc4NDAmt"]);
                                                    Misc4NDOTAmt        += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc4NDOTAmt"]);
                                                    Misc5Amt            += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc5Amt"]);
                                                    Misc5OTAmt          += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc5OTAmt"]);
                                                    Misc5NDAmt          += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc5NDAmt"]);
                                                    Misc5NDOTAmt        += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc5NDOTAmt"]);
                                                    Misc6Amt            += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc6Amt"]);
                                                    Misc6OTAmt          += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc6OTAmt"]);
                                                    Misc6NDAmt          += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc6NDAmt"]);
                                                    Misc6NDOTAmt        += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc6NDOTAmt"]);
                                                    #endregion
                                                }
                                                #endregion
                                            }
                                            #endregion

                                            #region Variance Computation
                                            if (strRegRule == "P")
                                            {
                                                drEmpPayrollDtl["Tpd_REGVarAmt"]            = Convert.ToDecimal(drEmpPayrollDtl["Tpd_REGAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_REGAmt"]);
                                                drEmpPayrollDtl["Tpd_RGRetroAmt"]           = Convert.ToDecimal(drEmpPayrollDtl["Tpd_REGAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_REGAmt"]);
                                                drEmpPayrollDtl["Tpd_TotalREGVarAmt"]       = Convert.ToDecimal(drEmpPayrollDtl["Tpd_TotalREGAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_TotalREGAmt"]);
                                            }
                                            drEmpPayrollDtl["Tpd_LTVarAmt"]                 = Convert.ToDecimal(drEmpPayrollDtl["Tpd_LTAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_LTAmt"]);
                                            drEmpPayrollDtl["Tpd_UTVarAmt"]                 = Convert.ToDecimal(drEmpPayrollDtl["Tpd_UTAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_UTAmt"]);
                                            drEmpPayrollDtl["Tpd_UPLVVarAmt"]               = Convert.ToDecimal(drEmpPayrollDtl["Tpd_UPLVAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_UPLVAmt"]);
                                            drEmpPayrollDtl["Tpd_ABSLEGHOLVarAmt"]          = Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSLEGHOLAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_ABSLEGHOLAmt"]);
                                            drEmpPayrollDtl["Tpd_ABSSPLHOLVarAmt"]          = Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSSPLHOLAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_ABSSPLHOLAmt"]);
                                            drEmpPayrollDtl["Tpd_ABSCOMPHOLVarAmt"]         = Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSCOMPHOLAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_ABSCOMPHOLAmt"]);
                                            drEmpPayrollDtl["Tpd_ABSPSDVarAmt"]             = Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSPSDAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_ABSPSDAmt"]);
                                            drEmpPayrollDtl["Tpd_ABSOTHHOLVarAmt"]          = Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSOTHHOLAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_ABSOTHHOLAmt"]);
                                            drEmpPayrollDtl["Tpd_WDABSVarAmt"]              = Convert.ToDecimal(drEmpPayrollDtl["Tpd_WDABSAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_WDABSAmt"]);
                                            drEmpPayrollDtl["Tpd_LTUTMaxVarAmt"]            = Convert.ToDecimal(drEmpPayrollDtl["Tpd_LTUTMaxAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_LTUTMaxAmt"]);
                                            drEmpPayrollDtl["Tpd_ABSVarAmt"]                = Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_ABSAmt"]);

                                            drEmpPayrollDtl["Tpd_PDLVVarAmt"]               = Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDLVAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_PDLVAmt"]);
                                            drEmpPayrollDtl["Tpd_PDLEGHOLVarAmt"]           = Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDLEGHOLAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_PDLEGHOLAmt"]);
                                            drEmpPayrollDtl["Tpd_PDSPLHOLVarAmt"]           = Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDSPLHOLAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_PDSPLHOLAmt"]);
                                            drEmpPayrollDtl["Tpd_PDCOMPHOLVarAmt"]          = Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDCOMPHOLAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_PDCOMPHOLAmt"]);
                                            drEmpPayrollDtl["Tpd_PDPSDVarAmt"]              = Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDPSDAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_PDPSDAmt"]);
                                            drEmpPayrollDtl["Tpd_PDOTHHOLVarAmt"]           = Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDOTHHOLAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_PDOTHHOLAmt"]);
                                            drEmpPayrollDtl["Tpd_PDRESTLEGHOLVarAmt"]       = Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDRESTLEGHOLAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_PDRESTLEGHOLAmt"]);
                                            
                                            drEmpPayrollDtl["Tpd_REGOTVarAmt"]              = Convert.ToDecimal(drEmpPayrollDtl["Tpd_REGOTAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_REGOTAmt"]);
                                            drEmpPayrollDtl["Tpd_REGNDVarAmt"]              = Convert.ToDecimal(drEmpPayrollDtl["Tpd_REGNDAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_REGNDAmt"]);
                                            drEmpPayrollDtl["Tpd_REGNDOTVarAmt"]            = Convert.ToDecimal(drEmpPayrollDtl["Tpd_REGNDOTAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_REGNDOTAmt"]);
                                            drEmpPayrollDtl["Tpd_RESTVarAmt"]               = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTAmt"]);
                                            drEmpPayrollDtl["Tpd_RESTOTVarAmt"]             = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTOTAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTOTAmt"]);
                                            drEmpPayrollDtl["Tpd_RESTNDVarAmt"]             = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTNDAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTNDAmt"]);
                                            drEmpPayrollDtl["Tpd_RESTNDOTVarAmt"]           = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTNDOTAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTNDOTAmt"]);
                                            drEmpPayrollDtl["Tpd_LEGHOLVarAmt"]             = Convert.ToDecimal(drEmpPayrollDtl["Tpd_LEGHOLAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_LEGHOLAmt"]);
                                            drEmpPayrollDtl["Tpd_LEGHOLOTVarAmt"]           = Convert.ToDecimal(drEmpPayrollDtl["Tpd_LEGHOLOTAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_LEGHOLOTAmt"]);
                                            drEmpPayrollDtl["Tpd_LEGHOLNDVarAmt"]           = Convert.ToDecimal(drEmpPayrollDtl["Tpd_LEGHOLNDAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_LEGHOLNDAmt"]);
                                            drEmpPayrollDtl["Tpd_LEGHOLNDOTVarAmt"]         = Convert.ToDecimal(drEmpPayrollDtl["Tpd_LEGHOLNDOTAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_LEGHOLNDOTAmt"]);
                                            drEmpPayrollDtl["Tpd_SPLHOLVarAmt"]             = Convert.ToDecimal(drEmpPayrollDtl["Tpd_SPLHOLAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_SPLHOLAmt"]);
                                            drEmpPayrollDtl["Tpd_SPLHOLOTVarAmt"]           = Convert.ToDecimal(drEmpPayrollDtl["Tpd_SPLHOLOTAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_SPLHOLOTAmt"]);
                                            drEmpPayrollDtl["Tpd_SPLHOLNDVarAmt"]           = Convert.ToDecimal(drEmpPayrollDtl["Tpd_SPLHOLNDAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_SPLHOLNDAmt"]);
                                            drEmpPayrollDtl["Tpd_SPLHOLNDOTVarAmt"]         = Convert.ToDecimal(drEmpPayrollDtl["Tpd_SPLHOLNDOTAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_SPLHOLNDOTAmt"]);
                                            drEmpPayrollDtl["Tpd_PSDVarAmt"]                = Convert.ToDecimal(drEmpPayrollDtl["Tpd_PSDAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_PSDAmt"]);
                                            drEmpPayrollDtl["Tpd_PSDOTVarAmt"]              = Convert.ToDecimal(drEmpPayrollDtl["Tpd_PSDOTAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_PSDOTAmt"]);
                                            drEmpPayrollDtl["Tpd_PSDNDVarAmt"]              = Convert.ToDecimal(drEmpPayrollDtl["Tpd_PSDNDAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_PSDNDAmt"]);
                                            drEmpPayrollDtl["Tpd_PSDNDOTVarAmt"]            = Convert.ToDecimal(drEmpPayrollDtl["Tpd_PSDNDOTAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_PSDNDOTAmt"]);
                                            drEmpPayrollDtl["Tpd_COMPHOLVarAmt"]            = Convert.ToDecimal(drEmpPayrollDtl["Tpd_COMPHOLAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_COMPHOLAmt"]);
                                            drEmpPayrollDtl["Tpd_COMPHOLOTVarAmt"]          = Convert.ToDecimal(drEmpPayrollDtl["Tpd_COMPHOLOTAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_COMPHOLOTAmt"]);
                                            drEmpPayrollDtl["Tpd_COMPHOLNDVarAmt"]          = Convert.ToDecimal(drEmpPayrollDtl["Tpd_COMPHOLNDAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_COMPHOLNDAmt"]);
                                            drEmpPayrollDtl["Tpd_COMPHOLNDOTVarAmt"]        = Convert.ToDecimal(drEmpPayrollDtl["Tpd_COMPHOLNDOTAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_COMPHOLNDOTAmt"]);
                                            drEmpPayrollDtl["Tpd_RESTLEGHOLVarAmt"]         = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTLEGHOLAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTLEGHOLAmt"]);
                                            drEmpPayrollDtl["Tpd_RESTLEGHOLOTVarAmt"]       = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTLEGHOLOTAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTLEGHOLOTAmt"]);
                                            drEmpPayrollDtl["Tpd_RESTLEGHOLNDVarAmt"]       = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTLEGHOLNDAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTLEGHOLNDAmt"]);
                                            drEmpPayrollDtl["Tpd_RESTLEGHOLNDOTVarAmt"]     = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTLEGHOLNDOTAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTLEGHOLNDOTAmt"]);
                                            drEmpPayrollDtl["Tpd_RESTSPLHOLVarAmt"]         = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTSPLHOLAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTSPLHOLAmt"]);
                                            drEmpPayrollDtl["Tpd_RESTSPLHOLOTVarAmt"]       = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTSPLHOLOTAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTSPLHOLOTAmt"]);
                                            drEmpPayrollDtl["Tpd_RESTSPLHOLNDVarAmt"]       = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTSPLHOLNDAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTSPLHOLNDAmt"]);
                                            drEmpPayrollDtl["Tpd_RESTSPLHOLNDOTVarAmt"]     = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTSPLHOLNDOTAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTSPLHOLNDOTAmt"]);
                                            drEmpPayrollDtl["Tpd_RESTCOMPHOLVarAmt"]        = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTCOMPHOLAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTCOMPHOLAmt"]);
                                            drEmpPayrollDtl["Tpd_RESTCOMPHOLOTVarAmt"]      = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTCOMPHOLOTAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTCOMPHOLOTAmt"]);
                                            drEmpPayrollDtl["Tpd_RESTCOMPHOLNDVarAmt"]      = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTCOMPHOLNDAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTCOMPHOLNDAmt"]);
                                            drEmpPayrollDtl["Tpd_RESTCOMPHOLNDOTVarAmt"]    = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTCOMPHOLNDOTAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTCOMPHOLNDOTAmt"]);
                                            drEmpPayrollDtl["Tpd_RESTPSDVarAmt"]            = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTPSDAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTPSDAmt"]);
                                            drEmpPayrollDtl["Tpd_RESTPSDOTVarAmt"]          = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTPSDOTAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTPSDOTAmt"]);
                                            drEmpPayrollDtl["Tpd_RESTPSDNDVarAmt"]          = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTPSDNDAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTPSDNDAmt"]);
                                            drEmpPayrollDtl["Tpd_RESTPSDNDOTVarAmt"]        = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTPSDNDOTAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpd_RESTPSDNDOTAmt"]);

                                            drEmpPayrollDtl["Tpd_OTRetroAmt"]               = (Convert.ToDecimal(drEmpPayrollDtl["Tpd_REGOTVarAmt"])
                                                                                                    + Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTVarAmt"]) + Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTOTVarAmt"])
                                                                                                    + Convert.ToDecimal(drEmpPayrollDtl["Tpd_LEGHOLVarAmt"]) + Convert.ToDecimal(drEmpPayrollDtl["Tpd_LEGHOLOTVarAmt"])
                                                                                                    + Convert.ToDecimal(drEmpPayrollDtl["Tpd_SPLHOLVarAmt"]) + Convert.ToDecimal(drEmpPayrollDtl["Tpd_SPLHOLOTVarAmt"])
                                                                                                    + Convert.ToDecimal(drEmpPayrollDtl["Tpd_PSDVarAmt"]) + Convert.ToDecimal(drEmpPayrollDtl["Tpd_PSDOTVarAmt"])
                                                                                                    + Convert.ToDecimal(drEmpPayrollDtl["Tpd_COMPHOLVarAmt"]) + Convert.ToDecimal(drEmpPayrollDtl["Tpd_COMPHOLOTVarAmt"])
                                                                                                    + Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTLEGHOLVarAmt"]) + Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTLEGHOLOTVarAmt"])
                                                                                                    + Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTSPLHOLVarAmt"]) + Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTSPLHOLOTVarAmt"])
                                                                                                    + Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTCOMPHOLVarAmt"]) + Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTCOMPHOLOTVarAmt"])
                                                                                                    + Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTPSDVarAmt"]) + Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTPSDOTVarAmt"]));

                                            drEmpPayrollDtl["Tpd_NDRetroAmt"]               = (Convert.ToDecimal(drEmpPayrollDtl["Tpd_REGNDVarAmt"]) + Convert.ToDecimal(drEmpPayrollDtl["Tpd_REGNDOTVarAmt"])
                                                                                                    + Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTNDVarAmt"]) + Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTNDOTVarAmt"])
                                                                                                    + Convert.ToDecimal(drEmpPayrollDtl["Tpd_LEGHOLNDVarAmt"]) + Convert.ToDecimal(drEmpPayrollDtl["Tpd_LEGHOLNDOTVarAmt"])
                                                                                                    + Convert.ToDecimal(drEmpPayrollDtl["Tpd_SPLHOLNDVarAmt"]) + Convert.ToDecimal(drEmpPayrollDtl["Tpd_SPLHOLNDVarAmt"])
                                                                                                    + Convert.ToDecimal(drEmpPayrollDtl["Tpd_PSDNDVarAmt"]) + Convert.ToDecimal(drEmpPayrollDtl["Tpd_PSDNDOTVarAmt"])
                                                                                                    + Convert.ToDecimal(drEmpPayrollDtl["Tpd_COMPHOLNDVarAmt"]) + Convert.ToDecimal(drEmpPayrollDtl["Tpd_COMPHOLNDOTVarAmt"])
                                                                                                    + Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTLEGHOLNDVarAmt"]) + Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTLEGHOLNDOTVarAmt"])
                                                                                                    + Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTSPLHOLNDVarAmt"]) + Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTSPLHOLNDOTVarAmt"])
                                                                                                    + Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTCOMPHOLNDVarAmt"]) + Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTCOMPHOLNDOTVarAmt"])
                                                                                                    + Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTPSDNDVarAmt"]) + Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTPSDNDOTVarAmt"]));


                                            if (bHasDayCodeExt)
                                            {
                                                #region Initialize Payroll Dtl Misc Calc Variance Amount
                                                drEmpPayrollDtlMisc["Tpm_Misc1VarAmt"]      = Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc1Amt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc1Amt"]);
                                                drEmpPayrollDtlMisc["Tpm_Misc1OTVarAmt"]    = Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc1OTAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc1OTAmt"]);
                                                drEmpPayrollDtlMisc["Tpm_Misc1NDVarAmt"]    = Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc1NDAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc1NDAmt"]);
                                                drEmpPayrollDtlMisc["Tpm_Misc1NDOTVarAmt"]  = Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc1NDOTAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc1NDOTAmt"]);
                                                drEmpPayrollDtlMisc["Tpm_Misc2VarAmt"]      = Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc2Amt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc2Amt"]);
                                                drEmpPayrollDtlMisc["Tpm_Misc2OTVarAmt"]    = Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc2OTAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc2OTAmt"]);
                                                drEmpPayrollDtlMisc["Tpm_Misc2NDVarAmt"]    = Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc2NDAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc2NDAmt"]);
                                                drEmpPayrollDtlMisc["Tpm_Misc2NDOTVarAmt"]  = Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc2NDOTAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc2NDOTAmt"]);
                                                drEmpPayrollDtlMisc["Tpm_Misc3VarAmt"]      = Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc3Amt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc3Amt"]);
                                                drEmpPayrollDtlMisc["Tpm_Misc3OTVarAmt"]    = Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc3OTAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc3OTAmt"]);
                                                drEmpPayrollDtlMisc["Tpm_Misc3NDVarAmt"]    = Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc3NDAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc3NDAmt"]);
                                                drEmpPayrollDtlMisc["Tpm_Misc3NDOTVarAmt"]  = Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc3NDOTAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc3NDOTAmt"]);
                                                drEmpPayrollDtlMisc["Tpm_Misc4VarAmt"]      = Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc4Amt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc4Amt"]);
                                                drEmpPayrollDtlMisc["Tpm_Misc4OTVarAmt"]    = Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc4OTAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc4OTAmt"]);
                                                drEmpPayrollDtlMisc["Tpm_Misc4NDVarAmt"]    = Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc4NDAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc4NDAmt"]);
                                                drEmpPayrollDtlMisc["Tpm_Misc4NDOTVarAmt"]  = Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc4NDOTAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc4NDOTAmt"]);
                                                drEmpPayrollDtlMisc["Tpm_Misc5VarAmt"]      = Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc5Amt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc5Amt"]);
                                                drEmpPayrollDtlMisc["Tpm_Misc5OTVarAmt"]    = Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc5OTAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc5OTAmt"]);
                                                drEmpPayrollDtlMisc["Tpm_Misc5NDVarAmt"]    = Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc5NDAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc5NDAmt"]);
                                                drEmpPayrollDtlMisc["Tpm_Misc5NDOTVarAmt"]  = Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc5NDOTAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc5NDOTAmt"]);
                                                drEmpPayrollDtlMisc["Tpm_Misc6VarAmt"]      = Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc6Amt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc6Amt"]);
                                                drEmpPayrollDtlMisc["Tpm_Misc6OTVarAmt"]    = Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc6OTAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc6OTAmt"]);
                                                drEmpPayrollDtlMisc["Tpm_Misc6NDVarAmt"]    = Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc6NDAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc6NDAmt"]);
                                                drEmpPayrollDtlMisc["Tpm_Misc6NDOTVarAmt"]  = Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc6NDOTAmt"]) - Convert.ToDecimal(dtPayrollDtlForProcess.Rows[idtl]["Tpm_Misc6NDOTAmt"]);
                                                #endregion

                                                drEmpPayrollDtl["Tpd_OTRetroAmt"]           = (Convert.ToDecimal(drEmpPayrollDtl["Tpd_OTRetroAmt"])
                                                                                                    + Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc1VarAmt"]) + Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc1OTVarAmt"])
                                                                                                    + Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc2VarAmt"]) + Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc2OTVarAmt"])
                                                                                                    + Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc3VarAmt"]) + Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc3OTVarAmt"])
                                                                                                    + Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc4VarAmt"]) + Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc4OTVarAmt"])
                                                                                                    + Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc5VarAmt"]) + Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc5OTVarAmt"])
                                                                                                    + Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc6VarAmt"]) + Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc6OTVarAmt"]));

                                                drEmpPayrollDtl["Tpd_NDRetroAmt"]           = (Convert.ToDecimal(drEmpPayrollDtl["Tpd_NDRetroAmt"])
                                                                                                    + Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc1NDVarAmt"]) + Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc1NDOTVarAmt"])
                                                                                                    + Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc2NDVarAmt"]) + Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc2NDOTVarAmt"])
                                                                                                    + Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc3NDVarAmt"]) + Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc3NDOTVarAmt"])
                                                                                                    + Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc4NDVarAmt"]) + Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc4NDOTVarAmt"])
                                                                                                    + Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc5NDVarAmt"]) + Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc5NDOTVarAmt"])
                                                                                                    + Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc6NDVarAmt"]) + Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc6NDOTVarAmt"]));
                                                
                                            }

                                            drEmpPayrollDtl["Tpd_HLRetroAmt"]               = (Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDLEGHOLVarAmt"])
                                                                                                    + Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDSPLHOLVarAmt"])
                                                                                                    + Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDCOMPHOLVarAmt"])
                                                                                                    + Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDPSDVarAmt"])
                                                                                                    + Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDOTHHOLVarAmt"])
                                                                                                    + Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDRESTLEGHOLVarAmt"]));

                                            drEmpPayrollDtl["Tpd_LVRetroAmt"]               = Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDLVVarAmt"]);
                                            drEmpPayrollDtl["Tpd_TotalRetroAmt"]            = Convert.ToDecimal(drEmpPayrollDtl["Tpd_RGRetroAmt"])
                                                                                                    + Convert.ToDecimal(drEmpPayrollDtl["Tpd_OTRetroAmt"])
                                                                                                    + Convert.ToDecimal(drEmpPayrollDtl["Tpd_NDRetroAmt"])
                                                                                                    + Convert.ToDecimal(drEmpPayrollDtl["Tpd_HLRetroAmt"])
                                                                                                    + Convert.ToDecimal(drEmpPayrollDtl["Tpd_LVRetroAmt"]);

                                            RGRetroAmt                                      += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RGRetroAmt"]);
                                            OTRetroAmt                                      += Convert.ToDecimal(drEmpPayrollDtl["Tpd_OTRetroAmt"]);
                                            NDRetroAmt                                      += Convert.ToDecimal(drEmpPayrollDtl["Tpd_NDRetroAmt"]);
                                            HLRetroAmt                                      += Convert.ToDecimal(drEmpPayrollDtl["Tpd_HLRetroAmt"]);
                                            LVRetroAmt                                      += Convert.ToDecimal(drEmpPayrollDtl["Tpd_LVRetroAmt"]);

                                            #endregion

                                            #region Rounding Off of Dtl Amounts
                                            drEmpPayrollDtl["Tpd_LTAmt"]                    = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_LTAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_LTVarAmt"]                 = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_LTVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_UTAmt"]                    = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_UTAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_UTVarAmt"]                 = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_UTVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_UPLVAmt"]                  = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_UPLVAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_UPLVVarAmt"]               = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_UPLVVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_ABSLEGHOLAmt"]             = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSLEGHOLAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_ABSLEGHOLVarAmt"]          = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSLEGHOLVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_ABSSPLHOLAmt"]             = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSSPLHOLAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_ABSSPLHOLVarAmt"]          = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSSPLHOLVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_ABSCOMPHOLAmt"]            = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSCOMPHOLAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_ABSCOMPHOLVarAmt"]         = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSCOMPHOLVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_ABSPSDAmt"]                = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSPSDAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_ABSPSDVarAmt"]             = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSPSDVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_ABSOTHHOLAmt"]             = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSOTHHOLAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_ABSOTHHOLVarAmt"]          = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSOTHHOLVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_WDABSAmt"]                 = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_WDABSAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_WDABSVarAmt"]              = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_WDABSVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_LTUTMaxAmt"]               = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_LTUTMaxAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_LTUTMaxVarAmt"]            = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_LTUTMaxVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_ABSAmt"]                   = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_ABSVarAmt"]                = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_REGAmt"]                   = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_REGAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_REGVarAmt"]                = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_REGVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_PDLVAmt"]                  = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDLVAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_PDLVVarAmt"]               = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDLVVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_PDLEGHOLAmt"]              = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDLEGHOLAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_PDLEGHOLVarAmt"]           = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDLEGHOLVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_PDSPLHOLAmt"]              = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDSPLHOLAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_PDSPLHOLVarAmt"]           = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDSPLHOLVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_PDCOMPHOLAmt"]             = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDCOMPHOLAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_PDCOMPHOLVarAmt"]          = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDCOMPHOLVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_PDPSDAmt"]                 = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDPSDAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_PDPSDVarAmt"]              = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDPSDVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_PDOTHHOLAmt"]              = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDOTHHOLAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_PDOTHHOLVarAmt"]           = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDOTHHOLVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_PDRESTLEGHOLAmt"]          = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDRESTLEGHOLAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_PDRESTLEGHOLVarAmt"]       = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDRESTLEGHOLVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_TotalREGAmt"]              = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_TotalREGAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_TotalREGVarAmt"]           = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_TotalREGVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_REGOTAmt"]                 = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_REGOTAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_REGOTVarAmt"]              = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_REGOTVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_REGNDAmt"]                 = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_REGNDAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_REGNDVarAmt"]              = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_REGNDVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_REGNDOTAmt"]               = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_REGNDOTAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_REGNDOTVarAmt"]            = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_REGNDOTVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_RESTAmt"]                  = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_RESTVarAmt"]               = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_RESTOTAmt"]                = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTOTAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_RESTOTVarAmt"]             = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTOTVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_RESTNDAmt"]                = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTNDAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_RESTNDVarAmt"]             = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTNDVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_RESTNDOTAmt"]              = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTNDOTAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_RESTNDOTVarAmt"]           = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTNDOTVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_LEGHOLAmt"]                = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_LEGHOLAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_LEGHOLVarAmt"]             = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_LEGHOLVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_LEGHOLOTAmt"]              = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_LEGHOLOTAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_LEGHOLOTVarAmt"]           = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_LEGHOLOTVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_LEGHOLNDAmt"]              = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_LEGHOLNDAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_LEGHOLNDVarAmt"]           = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_LEGHOLNDVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_LEGHOLNDOTAmt"]            = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_LEGHOLNDOTAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_LEGHOLNDOTVarAmt"]         = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_LEGHOLNDOTVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_SPLHOLAmt"]                = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_SPLHOLAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_SPLHOLVarAmt"]             = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_SPLHOLVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_SPLHOLOTAmt"]              = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_SPLHOLOTAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_SPLHOLOTVarAmt"]           = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_SPLHOLOTVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_SPLHOLNDAmt"]              = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_SPLHOLNDAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_SPLHOLNDVarAmt"]           = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_SPLHOLNDVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_SPLHOLNDOTAmt"]            = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_SPLHOLNDOTAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_SPLHOLNDOTVarAmt"]         = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_SPLHOLNDOTVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_PSDAmt"]                   = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_PSDAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_PSDVarAmt"]                = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_PSDVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_PSDOTAmt"]                 = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_PSDOTAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_PSDOTVarAmt"]              = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_PSDOTVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_PSDNDAmt"]                 = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_PSDNDAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_PSDNDVarAmt"]              = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_PSDNDVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_PSDNDOTAmt"]               = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_PSDNDOTAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_PSDNDOTVarAmt"]            = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_PSDNDOTVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_COMPHOLAmt"]               = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_COMPHOLAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_COMPHOLVarAmt"]            = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_COMPHOLVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_COMPHOLOTAmt"]             = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_COMPHOLOTAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_COMPHOLOTVarAmt"]          = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_COMPHOLOTVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_COMPHOLNDAmt"]             = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_COMPHOLNDAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_COMPHOLNDVarAmt"]          = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_COMPHOLNDVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_COMPHOLNDOTAmt"]           = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_COMPHOLNDOTAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_COMPHOLNDOTVarAmt"]        = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_COMPHOLNDOTVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_RESTLEGHOLAmt"]            = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTLEGHOLAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_RESTLEGHOLVarAmt"]         = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTLEGHOLVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_RESTLEGHOLOTAmt"]          = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTLEGHOLOTAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_RESTLEGHOLOTVarAmt"]       = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTLEGHOLOTVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_RESTLEGHOLNDAmt"]          = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTLEGHOLNDAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_RESTLEGHOLNDVarAmt"]       = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTLEGHOLNDVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_RESTLEGHOLNDOTAmt"]        = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTLEGHOLNDOTAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_RESTLEGHOLNDOTVarAmt"]     = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTLEGHOLNDOTVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_RESTSPLHOLAmt"]            = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTSPLHOLAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_RESTSPLHOLVarAmt"]         = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTSPLHOLVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_RESTSPLHOLOTAmt"]          = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTSPLHOLOTAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_RESTSPLHOLOTVarAmt"]       = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTSPLHOLOTVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_RESTSPLHOLNDAmt"]          = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTSPLHOLNDAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_RESTSPLHOLNDVarAmt"]       = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTSPLHOLNDVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_RESTSPLHOLNDOTAmt"]        = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTSPLHOLNDOTAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_RESTSPLHOLNDOTVarAmt"]     = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTSPLHOLNDOTVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_RESTCOMPHOLAmt"]           = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTCOMPHOLAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_RESTCOMPHOLVarAmt"]        = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTCOMPHOLVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_RESTCOMPHOLOTAmt"]         = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTCOMPHOLOTAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_RESTCOMPHOLOTVarAmt"]      = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTCOMPHOLOTVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_RESTCOMPHOLNDAmt"]         = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTCOMPHOLNDAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_RESTCOMPHOLNDVarAmt"]      = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTCOMPHOLNDVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_RESTCOMPHOLNDOTAmt"]       = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTCOMPHOLNDOTAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_RESTCOMPHOLNDOTVarAmt"]    = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTCOMPHOLNDOTVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_RESTPSDAmt"]               = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTPSDAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_RESTPSDVarAmt"]            = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTPSDVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_RESTPSDOTAmt"]             = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTPSDOTAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_RESTPSDOTVarAmt"]          = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTPSDOTVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_RESTPSDNDAmt"]             = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTPSDNDAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_RESTPSDNDVarAmt"]          = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTPSDNDVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_RESTPSDNDOTAmt"]           = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTPSDNDOTAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_RESTPSDNDOTVarAmt"]        = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTPSDNDOTVarAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_RGRetroAmt"]               = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RGRetroAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_OTRetroAmt"]               = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_OTRetroAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_NDRetroAmt"]               = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_NDRetroAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_HLRetroAmt"]               = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_HLRetroAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_LVRetroAmt"]               = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_LVRetroAmt"]), 2);
                                            drEmpPayrollDtl["Tpd_TotalRetroAmt"]            = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_TotalRetroAmt"]), 2);

                                            if (bHasDayCodeExt)
                                            {
                                                drEmpPayrollDtlMisc["Tpm_Misc1Amt"]         = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc1Amt"]), 2);
                                                drEmpPayrollDtlMisc["Tpm_Misc1VarAmt"]      = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc1VarAmt"]), 2);
                                                drEmpPayrollDtlMisc["Tpm_Misc1OTAmt"]       = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc1OTAmt"]), 2);
                                                drEmpPayrollDtlMisc["Tpm_Misc1OTVarAmt"]    = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc1OTVarAmt"]), 2);
                                                drEmpPayrollDtlMisc["Tpm_Misc1NDAmt"]       = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc1NDAmt"]), 2);
                                                drEmpPayrollDtlMisc["Tpm_Misc1NDVarAmt"]    = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc1NDVarAmt"]), 2);
                                                drEmpPayrollDtlMisc["Tpm_Misc1NDOTAmt"]     = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc1NDOTAmt"]), 2);
                                                drEmpPayrollDtlMisc["Tpm_Misc1NDOTVarAmt"]  = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc1NDOTVarAmt"]), 2);
                                                drEmpPayrollDtlMisc["Tpm_Misc2Amt"]         = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc2Amt"]), 2);
                                                drEmpPayrollDtlMisc["Tpm_Misc2VarAmt"]      = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc2VarAmt"]), 2);
                                                drEmpPayrollDtlMisc["Tpm_Misc2OTAmt"]       = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc2OTAmt"]), 2);
                                                drEmpPayrollDtlMisc["Tpm_Misc2OTVarAmt"]    = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc2OTVarAmt"]), 2);
                                                drEmpPayrollDtlMisc["Tpm_Misc2NDAmt"]       = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc2NDAmt"]), 2);
                                                drEmpPayrollDtlMisc["Tpm_Misc2NDVarAmt"]    = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc2NDVarAmt"]), 2);
                                                drEmpPayrollDtlMisc["Tpm_Misc2NDOTAmt"]     = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc2NDOTAmt"]), 2);
                                                drEmpPayrollDtlMisc["Tpm_Misc2NDOTVarAmt"]  = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc2NDOTVarAmt"]), 2);
                                                drEmpPayrollDtlMisc["Tpm_Misc3Amt"]         = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc3Amt"]), 2);
                                                drEmpPayrollDtlMisc["Tpm_Misc3VarAmt"]      = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc3VarAmt"]), 2);
                                                drEmpPayrollDtlMisc["Tpm_Misc3OTAmt"]       = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc3OTAmt"]), 2);
                                                drEmpPayrollDtlMisc["Tpm_Misc3OTVarAmt"]    = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc3OTVarAmt"]), 2);
                                                drEmpPayrollDtlMisc["Tpm_Misc3NDAmt"]       = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc3NDAmt"]), 2);
                                                drEmpPayrollDtlMisc["Tpm_Misc3NDVarAmt"]    = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc3NDVarAmt"]), 2);
                                                drEmpPayrollDtlMisc["Tpm_Misc3NDOTAmt"]     = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc3NDOTAmt"]), 2);
                                                drEmpPayrollDtlMisc["Tpm_Misc3NDOTVarAmt"]  = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc3NDOTVarAmt"]), 2);
                                                drEmpPayrollDtlMisc["Tpm_Misc4Amt"]         = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc4Amt"]), 2);
                                                drEmpPayrollDtlMisc["Tpm_Misc4VarAmt"]      = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc4VarAmt"]), 2);
                                                drEmpPayrollDtlMisc["Tpm_Misc4OTAmt"]       = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc4OTAmt"]), 2);
                                                drEmpPayrollDtlMisc["Tpm_Misc4OTVarAmt"]    = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc4OTVarAmt"]), 2);
                                                drEmpPayrollDtlMisc["Tpm_Misc4NDAmt"]       = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc4NDAmt"]), 2);
                                                drEmpPayrollDtlMisc["Tpm_Misc4NDVarAmt"]    = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc4NDVarAmt"]), 2);
                                                drEmpPayrollDtlMisc["Tpm_Misc4NDOTAmt"]     = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc4NDOTAmt"]), 2);
                                                drEmpPayrollDtlMisc["Tpm_Misc4NDOTVarAmt"]  = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc4NDOTVarAmt"]), 2);
                                                drEmpPayrollDtlMisc["Tpm_Misc5Amt"]         = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc5Amt"]), 2);
                                                drEmpPayrollDtlMisc["Tpm_Misc5VarAmt"]      = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc5VarAmt"]), 2);
                                                drEmpPayrollDtlMisc["Tpm_Misc5OTAmt"]       = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc5OTAmt"]), 2);
                                                drEmpPayrollDtlMisc["Tpm_Misc5OTVarAmt"]    = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc5OTVarAmt"]), 2);
                                                drEmpPayrollDtlMisc["Tpm_Misc5NDAmt"]       = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc5NDAmt"]), 2);
                                                drEmpPayrollDtlMisc["Tpm_Misc5NDVarAmt"]    = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc5NDVarAmt"]), 2);
                                                drEmpPayrollDtlMisc["Tpm_Misc5NDOTAmt"]     = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc5NDOTAmt"]), 2);
                                                drEmpPayrollDtlMisc["Tpm_Misc5NDOTVarAmt"]  = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc5NDOTVarAmt"]), 2);
                                                drEmpPayrollDtlMisc["Tpm_Misc6Amt"]         = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc6Amt"]), 2);
                                                drEmpPayrollDtlMisc["Tpm_Misc6VarAmt"]      = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc6VarAmt"]), 2);
                                                drEmpPayrollDtlMisc["Tpm_Misc6OTAmt"]       = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc6OTAmt"]), 2);
                                                drEmpPayrollDtlMisc["Tpm_Misc6OTVarAmt"]    = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc6OTVarAmt"]), 2);
                                                drEmpPayrollDtlMisc["Tpm_Misc6NDAmt"]       = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc6NDAmt"]), 2);
                                                drEmpPayrollDtlMisc["Tpm_Misc6NDVarAmt"]    = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc6NDVarAmt"]), 2);
                                                drEmpPayrollDtlMisc["Tpm_Misc6NDOTAmt"]     = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc6NDOTAmt"]), 2);
                                                drEmpPayrollDtlMisc["Tpm_Misc6NDOTVarAmt"]  = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc6NDOTVarAmt"]), 2);
                                            }
                                            #endregion

                                            #endregion
                                        }

                                        //Add to payroll tables
                                        dtEmpPayrollDtl.Rows.Add(drEmpPayrollDtl);
                                        if (bHasDayCodeExt)
                                            dtEmpPayrollDtlMisc.Rows.Add(drEmpPayrollDtlMisc);
                                    }
                                    
                                    #endregion

                                    #region Payroll Hdr

                                    #region Compute Hour Amounts

                                    #region Compute Hour premiums

                                    #region Absent Amount Computation
                                    drEmpPayroll["Tpy_UPLVAmt"]                             = UnpaidLeaveAmt;
                                    drEmpPayroll["Tpy_ABSLEGHOLAmt"]                        = AbsentLegalHolidayAmt;
                                    drEmpPayroll["Tpy_ABSSPLHOLAmt"]                        = AbsentSpecialHolidayAmt;
                                    drEmpPayroll["Tpy_ABSCOMPHOLAmt"]                       = AbsentCompanyHolidayAmt;
                                    drEmpPayroll["Tpy_ABSPSDAmt"]                           = AbsentPlantShutdownAmt;
                                    drEmpPayroll["Tpy_ABSOTHHOLAmt"]                        = AbsentFillerHolidayAmt;
                                    drEmpPayroll["Tpy_WDABSAmt"]                            = WholeDayAbsentAmt;
                                    drEmpPayroll["Tpy_LTUTMaxAmt"]                          = LateUndertimeMaxAbsentAmt;


                                    if (Convert.ToBoolean(PAYSPECIALRATE))
                                    {
                                        if (brLateHr > 0)
                                            drEmpPayroll["Tpy_LTAmt"]                       = brLateHr * (LTRate == "S" ? SpecialHourlyRate : HourlyRate);
                                        else
                                            drEmpPayroll["Tpy_LTAmt"]                       = LateAmt;

                                        if (brUndertimeHr > 0)
                                            drEmpPayroll["Tpy_UTAmt"]                       = brUndertimeHr * (UTRate == "S" ? SpecialHourlyRate : HourlyRate);
                                        else
                                            drEmpPayroll["Tpy_UTAmt"]                       = UndertimeAmt;

                                        if ((brLateHr + brUndertimeHr) > 0)
                                            drEmpPayroll["Tpy_ABSAmt"]                      = Convert.ToDecimal(drEmpPayroll["Tpy_LTAmt"])
                                                                                                    + Convert.ToDecimal(drEmpPayroll["Tpy_UTAmt"])
                                                                                                    + Convert.ToDecimal(drEmpPayroll["Tpy_UPLVAmt"])
                                                                                                    + Convert.ToDecimal(drEmpPayroll["Tpy_WDABSAmt"])
                                                                                                    + Convert.ToDecimal(drEmpPayroll["Tpy_LTUTMaxAmt"]);
                                        else
                                            drEmpPayroll["Tpy_ABSAmt"]                      = AbsentAmt;
                                    }
                                    else
                                    {
                                        if (brLateHr > 0)
                                            drEmpPayroll["Tpy_LTAmt"]                       = brLateHr * HourlyRate;
                                        else
                                            drEmpPayroll["Tpy_LTAmt"]                       = LateAmt;

                                        if (brUndertimeHr > 0)
                                            drEmpPayroll["Tpy_UTAmt"]                       = brUndertimeHr * HourlyRate;
                                        else
                                            drEmpPayroll["Tpy_UTAmt"]                       = UndertimeAmt;

                                        if ((brLateHr + brUndertimeHr) > 0)
                                            drEmpPayroll["Tpy_ABSAmt"]                      = Convert.ToDecimal(drEmpPayroll["Tpy_ABSHr"]) * HourlyRate;
                                        else
                                            drEmpPayroll["Tpy_ABSAmt"]                      = AbsentAmt;
                                    }

                                    #endregion

                                    #region Regular Amount Computation
                                    drEmpPayroll["Tpy_REGAmt"]                              = RegularAmt;
                                    drEmpPayroll["Tpy_PDLVAmt"]                             = PaidLeaveAmt;

                                    drEmpPayroll["Tpy_PDLEGHOLAmt"]                         = PaidLegalHolidayAmt;
                                    drEmpPayroll["Tpy_PDSPLHOLAmt"]                         = PaidSpecialHolidayAmt;
                                    drEmpPayroll["Tpy_PDCOMPHOLAmt"]                        = PaidCompanyHolidayAmt;
                                    drEmpPayroll["Tpy_PDPSDAmt"]                            = PaidPlantShutdownHolidayAmt;
                                    drEmpPayroll["Tpy_PDOTHHOLAmt"]                         = PaidFillerHolidayAmt;
                                    drEmpPayroll["Tpy_PDRESTLEGHOLAmt"]                     = PaidRestdayLegalHolidayAmt;
                                    #endregion

                                    #region Total Regular Amount Computation
                                    decimal RegularPayAmt = 0;
                                    if (drEmpPayroll["Tpy_RegRule"].ToString() == "P")
                                    {
                                        drEmpPayroll["Tpy_TotalREGAmt"] = Math.Round(RegularAmt
                                                                                                + PaidLeaveAmt
                                                                                                + PaidLegalHolidayAmt
                                                                                                + PaidSpecialHolidayAmt
                                                                                                + PaidCompanyHolidayAmt
                                                                                                + PaidFillerHolidayAmt
                                                                                                + PaidRestdayLegalHolidayAmt, 2);
                                        RegularPayAmt = Convert.ToDecimal(drEmpPayroll["Tpy_TotalREGAmt"]);
                                    }
                                    else if (drEmpPayroll["Tpy_RegRule"].ToString() == "A")
                                    {
                                        if (Convert.ToBoolean(MULTSAL) && WorkingDayCntUsingNewRate != WorkingDayCnt)
                                        {
                                            #region Multiple Salary Rate
                                            drEmpPayroll["Tpy_ABSAmt"] = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_ABSAmt"])
                                                                                                                   + AbsentLegalHolidayAmt
                                                                                                                   + AbsentSpecialHolidayAmt
                                                                                                                   + AbsentCompanyHolidayAmt
                                                                                                                   + AbsentPlantShutdownAmt
                                                                                                                   + AbsentFillerHolidayAmt, 2);

                                            OldDailyRate = Math.Round(OldSalaryRate * 12 / MDIVISOR, 2, MidpointRounding.AwayFromZero);
                                            NewDailyRate = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_SalaryRate"]) * 12 / MDIVISOR, 2, MidpointRounding.AwayFromZero);

                                            decimal abs = Convert.ToDecimal(drEmpPayroll["Tpy_ABSAmt"]);
                                            if (!bMonthlyToDailyPayrollType)
                                                drEmpPayroll["Tpy_TotalREGAmt"] = Math.Round((Convert.ToDecimal(drEmpPayroll["Tpy_SalaryRate"]) / 2)
                                                                                                                    + ((OldDailyRate - NewDailyRate) * (WorkingDayCnt - WorkingDayCntUsingNewRate))
                                                                                                                    - Convert.ToDecimal(drEmpPayroll["Tpy_ABSAmt"])
                                                                                                                    + PaidRestdayLegalHolidayAmt, 2);
                                            else
                                                drEmpPayroll["Tpy_TotalREGAmt"] = Math.Round((((Convert.ToDecimal(drEmpPayroll["Tpy_SalaryRate"]) * MDIVISOR) / 12) / 2) //EMR
                                                                                                                    + ((OldDailyRate - NewDailyRate) * (WorkingDayCnt - WorkingDayCntUsingNewRate))
                                                                                                                    - Convert.ToDecimal(drEmpPayroll["Tpy_ABSAmt"])
                                                                                                                    + PaidRestdayLegalHolidayAmt, 2);

                                            //Derive RegularAmt from TotalRegularAmt
                                            drEmpPayroll["Tpy_REGAmt"] = 0;
                                            drEmpPayroll["Tpy_REGAmt"] = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_TotalREGAmt"])
                                                                                                                     - PaidLegalHolidayAmt
                                                                                                                     - PaidSpecialHolidayAmt
                                                                                                                     - PaidCompanyHolidayAmt
                                                                                                                     - PaidFillerHolidayAmt
                                                                                                                     - PaidLeaveAmt
                                                                                                                     - PaidRestdayLegalHolidayAmt, 2);

                                            decimal regamt = Convert.ToDecimal(drEmpPayroll["Tpy_REGAmt"]);
                                            drEmpPayroll["Tpy_DayCountOldSalaryRate"] = (WorkingDayCnt - WorkingDayCntUsingNewRate); //NO OF DAYS USING OLD DAILY RATE
                                            RegularPayAmt = Convert.ToDecimal(drEmpPayroll["Tpy_TotalREGAmt"]);
                                            drEmpPayroll["Tpy_IsMultipleSalary"] = true;
                                            #endregion
                                        }
                                        else
                                        {
                                            #region Normal Flow
                                            RegularPayAmt = Math.Round((SalaryRate / 2) - Convert.ToDecimal(drEmpPayroll["Tpy_ABSAmt"]), 2);

                                            //Set Regular Amt as balancing factor
                                            drEmpPayroll["Tpy_REGAmt"] = 0;
                                            drEmpPayroll["Tpy_REGAmt"] = RegularPayAmt
                                                                                                            - Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_PDLEGHOLAmt"]), 2)
                                                                                                            - Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_PDSPLHOLAmt"]), 2)
                                                                                                            - Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_PDCOMPHOLAmt"]), 2)
                                                                                                            - Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_PDPSDAmt"]), 2)
                                                                                                            - Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_PDOTHHOLAmt"]), 2)
                                                                                                            - Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_PDLVAmt"]), 2);

                                            RegularPayAmt                   += Convert.ToDecimal(drEmpPayroll["Tpy_PDRESTLEGHOLAmt"]);

                                            if (Convert.ToDecimal(drEmpPayroll["Tpy_REGAmt"]) < 0)
                                                drEmpPayroll["Tpy_REGAmt"] = 0;

                                            drEmpPayroll["Tpy_TotalREGAmt"] = RegularPayAmt; 

                                            #endregion
                                        }

                                        #region Cleanup for Regular Amount Considerations 
                                        // - Regular Amount computed above is 0 (more absences than the present days/half-month pay)
                                        // - Sum of Regular Hours is 0 (possibly on Leave for the whole quincena)
                                        if ((PayrollType == "M" && Convert.ToDecimal(drEmpPayroll["Tpy_TotalREGHr"]) == 0)
                                            || Convert.ToDecimal(drEmpPayroll["Tpy_TotalREGAmt"]) < 0)
                                        {
                                            RegularPayAmt = 0;
                                            drEmpPayroll["Tpy_TotalREGAmt"]     = 0;
                                            drEmpPayroll["Tpy_TotalREGHr"]      = 0;

                                            drEmpPayroll["Tpy_REGAmt"]          = 0;
                                            drEmpPayroll["Tpy_PDLVAmt"]         = 0;

                                            drEmpPayroll["Tpy_PDLEGHOLHr"]      = 0;
                                            drEmpPayroll["Tpy_PDSPLHOLHr"]      = 0;
                                            drEmpPayroll["Tpy_PDCOMPHOLHr"]     = 0;
                                            drEmpPayroll["Tpy_PDPSDHr"]         = 0;
                                            drEmpPayroll["Tpy_PDOTHHOLHr"]      = 0;

                                            drEmpPayroll["Tpy_PDLEGHOLAmt"]     = 0;
                                            drEmpPayroll["Tpy_PDSPLHOLAmt"]     = 0;
                                            drEmpPayroll["Tpy_PDCOMPHOLAmt"]    = 0;
                                            drEmpPayroll["Tpy_PDPSDAmt"]        = 0;
                                            drEmpPayroll["Tpy_PDOTHHOLAmt"]     = 0;

                                            drEmpPayroll["Tpy_PDRESTLEGHOLHr"]  = 0;
                                            drEmpPayroll["Tpy_PDRESTLEGHOLAmt"] = 0;
                                        }
                                        #endregion
                                    }

                                    //Auto-correct Regular Amount if it is greater than Total Regular Amount
                                    if (Convert.ToDecimal(drEmpPayroll["Tpy_REGAmt"]) > RegularPayAmt)
                                        drEmpPayroll["Tpy_REGAmt"]                          = RegularPayAmt;

                                    //decimal xxx = Convert.ToDecimal(drEmpPayroll["Tpy_REGAmt"]);
                                    //decimal xxxx = Convert.ToDecimal(drEmpPayroll["Tpy_REGHr"]);
                                    if (Convert.ToDecimal(drEmpPayroll["Tpy_REGAmt"]) < 0
                                        && (Convert.ToDecimal(drEmpPayroll["Tpy_REGHr"]) == 0 || Convert.ToDecimal(drEmpPayroll["Tpy_REGHr"]) > 0))
                                    {
                                        drEmpPayroll["Tpy_REGHr"]       = 0;
                                        drEmpPayroll["Tpy_REGAmt"]      = 0;
                                    }

                                    #endregion

                                    #region Overtime Amount Computation
                                    drEmpPayroll["Tpy_REGOTAmt"]                            = RegularOTAmt;
                                    drEmpPayroll["Tpy_REGNDAmt"]                            = RegularNDAmt;
                                    drEmpPayroll["Tpy_REGNDOTAmt"]                          = RegularOTNDAmt;

                                    drEmpPayroll["Tpy_RESTAmt"]                             = RestdayAmt;
                                    drEmpPayroll["Tpy_RESTOTAmt"]                           = RestdayOTAmt;
                                    drEmpPayroll["Tpy_RESTNDAmt"]                           = RestdayNDAmt;
                                    drEmpPayroll["Tpy_RESTNDOTAmt"]                         = RestdayOTNDAmt;

                                    drEmpPayroll["Tpy_LEGHOLAmt"]                           = LegalHolidayAmt;
                                    drEmpPayroll["Tpy_LEGHOLOTAmt"]                         = LegalHolidayOTAmt;
                                    drEmpPayroll["Tpy_LEGHOLNDAmt"]                         = LegalHolidayNDAmt;
                                    drEmpPayroll["Tpy_LEGHOLNDOTAmt"]                       = LegalHolidayOTNDAmt;

                                    drEmpPayroll["Tpy_SPLHOLAmt"]                           = SpecialHolidayAmt;
                                    drEmpPayroll["Tpy_SPLHOLOTAmt"]                         = SpecialHolidayOTAmt;
                                    drEmpPayroll["Tpy_SPLHOLNDAmt"]                         = SpecialHolidayNDAmt;
                                    drEmpPayroll["Tpy_SPLHOLNDOTAmt"]                       = SpecialHolidayOTNDAmt;

                                    drEmpPayroll["Tpy_PSDAmt"]                              = PlantShutdownAmt;
                                    drEmpPayroll["Tpy_PSDOTAmt"]                            = PlantShutdownOTAmt;
                                    drEmpPayroll["Tpy_PSDNDAmt"]                            = PlantShutdownNDAmt;
                                    drEmpPayroll["Tpy_PSDNDOTAmt"]                          = PlantShutdownOTNDAmt;

                                    drEmpPayroll["Tpy_COMPHOLAmt"]                          = CompanyHolidayAmt;
                                    drEmpPayroll["Tpy_COMPHOLOTAmt"]                        = CompanyHolidayOTAmt;
                                    drEmpPayroll["Tpy_COMPHOLNDAmt"]                        = CompanyHolidayNDAmt;
                                    drEmpPayroll["Tpy_COMPHOLNDOTAmt"]                      = CompanyHolidayOTNDAmt;

                                    drEmpPayroll["Tpy_RESTLEGHOLAmt"]                       = RestdayLegalHolidayAmt;
                                    drEmpPayroll["Tpy_RESTLEGHOLOTAmt"]                     = RestdayLegalHolidayOTAmt;
                                    drEmpPayroll["Tpy_RESTLEGHOLNDAmt"]                     = RestdayLegalHolidayNDAmt;
                                    drEmpPayroll["Tpy_RESTLEGHOLNDOTAmt"]                   = RestdayLegalHolidayOTNDAmt;

                                    drEmpPayroll["Tpy_RESTSPLHOLAmt"]                       = RestdaySpecialHolidayAmt;
                                    drEmpPayroll["Tpy_RESTSPLHOLNDAmt"]                     = RestdaySpecialHolidayOTAmt;
                                    drEmpPayroll["Tpy_RESTSPLHOLOTAmt"]                     = RestdaySpecialHolidayNDAmt;
                                    drEmpPayroll["Tpy_RESTSPLHOLNDOTAmt"]                   = RestdaySpecialHolidayOTNDAmt;

                                    drEmpPayroll["Tpy_RESTCOMPHOLAmt"]                      = RestdayCompanyHolidayAmt;
                                    drEmpPayroll["Tpy_RESTCOMPHOLNDAmt"]                    = RestdayCompanyHolidayOTAmt;
                                    drEmpPayroll["Tpy_RESTCOMPHOLOTAmt"]                    = RestdayCompanyHolidayNDAmt;
                                    drEmpPayroll["Tpy_RESTCOMPHOLNDOTAmt"]                  = RestdayCompanyHolidayOTNDAmt;

                                    drEmpPayroll["Tpy_RESTPSDAmt"]                          = RestdayPlantShutdownAmt;
                                    drEmpPayroll["Tpy_RESTPSDNDAmt"]                        = RestdayPlantShutdownOTAmt;
                                    drEmpPayroll["Tpy_RESTPSDOTAmt"]                        = RestdayPlantShutdownNDAmt;
                                    drEmpPayroll["Tpy_RESTPSDNDOTAmt"]                      = RestdayPlantShutdownOTNDAmt;

                                    #endregion

                                    #region Variance Amount Computation
                                    drEmpPayroll["Tpy_LTVarAmt"]            = Convert.ToDecimal(drEmpPayroll["Tpy_LTAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_LTAmt"]);
                                    drEmpPayroll["Tpy_UTVarAmt"]            = Convert.ToDecimal(drEmpPayroll["Tpy_UTAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_UTAmt"]);
                                    drEmpPayroll["Tpy_UPLVVarAmt"]          = Convert.ToDecimal(drEmpPayroll["Tpy_UPLVAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_UPLVAmt"]);
                                    drEmpPayroll["Tpy_ABSLEGHOLVarAmt"]     = Convert.ToDecimal(drEmpPayroll["Tpy_ABSLEGHOLAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_ABSLEGHOLAmt"]);
                                    drEmpPayroll["Tpy_ABSSPLHOLVarAmt"]     = Convert.ToDecimal(drEmpPayroll["Tpy_ABSSPLHOLAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_ABSSPLHOLAmt"]);
                                    drEmpPayroll["Tpy_ABSCOMPHOLVarAmt"]    = Convert.ToDecimal(drEmpPayroll["Tpy_ABSCOMPHOLAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_ABSCOMPHOLAmt"]);
                                    drEmpPayroll["Tpy_ABSPSDVarAmt"]        = Convert.ToDecimal(drEmpPayroll["Tpy_ABSPSDAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_ABSPSDAmt"]);
                                    drEmpPayroll["Tpy_ABSOTHHOLVarAmt"]     = Convert.ToDecimal(drEmpPayroll["Tpy_ABSOTHHOLAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_ABSOTHHOLAmt"]);
                                    drEmpPayroll["Tpy_WDABSVarAmt"]         = Convert.ToDecimal(drEmpPayroll["Tpy_WDABSAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_WDABSAmt"]);
                                    drEmpPayroll["Tpy_LTUTMaxVarAmt"]       = Convert.ToDecimal(drEmpPayroll["Tpy_LTUTMaxAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_LTUTMaxAmt"]);
                                    drEmpPayroll["Tpy_ABSVarAmt"]           = Convert.ToDecimal(drEmpPayroll["Tpy_ABSAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_ABSAmt"]);

                                    drEmpPayroll["Tpy_REGVarAmt"]           = Convert.ToDecimal(drEmpPayroll["Tpy_REGAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_REGAmt"]);
                                    drEmpPayroll["Tpy_PDLVVarAmt"]          = Convert.ToDecimal(drEmpPayroll["Tpy_PDLVAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_PDLVAmt"]);
                                    drEmpPayroll["Tpy_PDLEGHOLVarAmt"]      = Convert.ToDecimal(drEmpPayroll["Tpy_PDLEGHOLAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_PDLEGHOLAmt"]);
                                    drEmpPayroll["Tpy_PDSPLHOLVarAmt"]      = Convert.ToDecimal(drEmpPayroll["Tpy_PDSPLHOLAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_PDSPLHOLAmt"]);
                                    drEmpPayroll["Tpy_PDCOMPHOLVarAmt"]     = Convert.ToDecimal(drEmpPayroll["Tpy_PDCOMPHOLAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_PDCOMPHOLAmt"]);
                                    drEmpPayroll["Tpy_PDPSDVarAmt"]         = Convert.ToDecimal(drEmpPayroll["Tpy_PDPSDAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_PDPSDAmt"]);
                                    drEmpPayroll["Tpy_PDOTHHOLVarAmt"]      = Convert.ToDecimal(drEmpPayroll["Tpy_PDOTHHOLAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_PDOTHHOLAmt"]);
                                    drEmpPayroll["Tpy_PDRESTLEGHOLVarAmt"]  = Convert.ToDecimal(drEmpPayroll["Tpy_PDRESTLEGHOLAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_PDRESTLEGHOLAmt"]);
                                    drEmpPayroll["Tpy_TotalREGVarAmt"]      = Convert.ToDecimal(drEmpPayroll["Tpy_TotalREGAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_TotalREGAmt"]);

                                    drEmpPayroll["Tpy_REGOTVarAmt"]         = Convert.ToDecimal(drEmpPayroll["Tpy_REGOTAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_REGOTAmt"]);
                                    drEmpPayroll["Tpy_REGNDVarAmt"]         = Convert.ToDecimal(drEmpPayroll["Tpy_REGNDAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_REGNDAmt"]);
                                    drEmpPayroll["Tpy_REGNDOTVarAmt"]       = Convert.ToDecimal(drEmpPayroll["Tpy_REGNDOTAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_REGNDOTAmt"]);
                                    drEmpPayroll["Tpy_RESTVarAmt"]          = Convert.ToDecimal(drEmpPayroll["Tpy_RESTAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_RESTAmt"]);
                                    drEmpPayroll["Tpy_RESTOTVarAmt"]        = Convert.ToDecimal(drEmpPayroll["Tpy_RESTOTAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_RESTOTAmt"]);
                                    drEmpPayroll["Tpy_RESTNDVarAmt"]        = Convert.ToDecimal(drEmpPayroll["Tpy_RESTNDAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_RESTNDAmt"]);
                                    drEmpPayroll["Tpy_RESTNDOTVarAmt"]      = Convert.ToDecimal(drEmpPayroll["Tpy_RESTNDOTAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_RESTNDOTAmt"]);
                                    drEmpPayroll["Tpy_LEGHOLVarAmt"]        = Convert.ToDecimal(drEmpPayroll["Tpy_LEGHOLAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_LEGHOLAmt"]);
                                    drEmpPayroll["Tpy_LEGHOLOTVarAmt"]      = Convert.ToDecimal(drEmpPayroll["Tpy_LEGHOLOTAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_LEGHOLOTAmt"]);
                                    drEmpPayroll["Tpy_LEGHOLNDVarAmt"]      = Convert.ToDecimal(drEmpPayroll["Tpy_LEGHOLNDAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_LEGHOLNDAmt"]);
                                    drEmpPayroll["Tpy_LEGHOLNDOTVarAmt"]    = Convert.ToDecimal(drEmpPayroll["Tpy_LEGHOLNDOTAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_LEGHOLNDOTAmt"]);
                                    drEmpPayroll["Tpy_SPLHOLVarAmt"]        = Convert.ToDecimal(drEmpPayroll["Tpy_SPLHOLAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_SPLHOLAmt"]);
                                    drEmpPayroll["Tpy_SPLHOLOTVarAmt"]      = Convert.ToDecimal(drEmpPayroll["Tpy_SPLHOLOTAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_SPLHOLOTAmt"]);
                                    drEmpPayroll["Tpy_SPLHOLNDVarAmt"]      = Convert.ToDecimal(drEmpPayroll["Tpy_SPLHOLNDAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_SPLHOLNDAmt"]);
                                    drEmpPayroll["Tpy_SPLHOLNDOTVarAmt"]    = Convert.ToDecimal(drEmpPayroll["Tpy_SPLHOLNDOTAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_SPLHOLNDOTAmt"]);
                                    drEmpPayroll["Tpy_PSDVarAmt"]           = Convert.ToDecimal(drEmpPayroll["Tpy_PSDAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_PSDAmt"]);
                                    drEmpPayroll["Tpy_PSDOTVarAmt"]         = Convert.ToDecimal(drEmpPayroll["Tpy_PSDOTAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_PSDOTAmt"]);
                                    drEmpPayroll["Tpy_PSDNDVarAmt"]         = Convert.ToDecimal(drEmpPayroll["Tpy_PSDNDAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_PSDNDAmt"]);
                                    drEmpPayroll["Tpy_PSDNDOTVarAmt"]       = Convert.ToDecimal(drEmpPayroll["Tpy_PSDNDOTAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_PSDNDOTAmt"]);
                                    drEmpPayroll["Tpy_COMPHOLVarAmt"]       = Convert.ToDecimal(drEmpPayroll["Tpy_COMPHOLAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_COMPHOLAmt"]);
                                    drEmpPayroll["Tpy_COMPHOLOTVarAmt"]     = Convert.ToDecimal(drEmpPayroll["Tpy_COMPHOLOTAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_COMPHOLOTAmt"]);
                                    drEmpPayroll["Tpy_COMPHOLNDVarAmt"]     = Convert.ToDecimal(drEmpPayroll["Tpy_COMPHOLNDAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_COMPHOLNDAmt"]);
                                    drEmpPayroll["Tpy_COMPHOLNDOTVarAmt"]   = Convert.ToDecimal(drEmpPayroll["Tpy_COMPHOLNDOTAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_COMPHOLNDOTAmt"]);
                                    drEmpPayroll["Tpy_RESTLEGHOLVarAmt"]    = Convert.ToDecimal(drEmpPayroll["Tpy_RESTLEGHOLAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_RESTLEGHOLAmt"]);
                                    drEmpPayroll["Tpy_RESTLEGHOLOTVarAmt"]  = Convert.ToDecimal(drEmpPayroll["Tpy_RESTLEGHOLOTAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_RESTLEGHOLOTAmt"]);
                                    drEmpPayroll["Tpy_RESTLEGHOLNDVarAmt"]  = Convert.ToDecimal(drEmpPayroll["Tpy_RESTLEGHOLNDAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_RESTLEGHOLNDAmt"]);
                                    drEmpPayroll["Tpy_RESTLEGHOLNDOTVarAmt"] = Convert.ToDecimal(drEmpPayroll["Tpy_RESTLEGHOLNDOTAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_RESTLEGHOLNDOTAmt"]);
                                    drEmpPayroll["Tpy_RESTSPLHOLVarAmt"]    = Convert.ToDecimal(drEmpPayroll["Tpy_RESTSPLHOLAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_RESTSPLHOLAmt"]);
                                    drEmpPayroll["Tpy_RESTSPLHOLOTVarAmt"]  = Convert.ToDecimal(drEmpPayroll["Tpy_RESTSPLHOLOTAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_RESTSPLHOLOTAmt"]);
                                    drEmpPayroll["Tpy_RESTSPLHOLNDVarAmt"]  = Convert.ToDecimal(drEmpPayroll["Tpy_RESTSPLHOLNDAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_RESTSPLHOLNDAmt"]);
                                    drEmpPayroll["Tpy_RESTSPLHOLNDOTVarAmt"] = Convert.ToDecimal(drEmpPayroll["Tpy_RESTSPLHOLNDOTAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_RESTSPLHOLNDOTAmt"]);
                                    drEmpPayroll["Tpy_RESTCOMPHOLVarAmt"]   = Convert.ToDecimal(drEmpPayroll["Tpy_RESTCOMPHOLAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_RESTCOMPHOLAmt"]);
                                    drEmpPayroll["Tpy_RESTCOMPHOLOTVarAmt"] = Convert.ToDecimal(drEmpPayroll["Tpy_RESTCOMPHOLOTAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_RESTCOMPHOLOTAmt"]);
                                    drEmpPayroll["Tpy_RESTCOMPHOLNDVarAmt"] = Convert.ToDecimal(drEmpPayroll["Tpy_RESTCOMPHOLNDAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_RESTCOMPHOLNDAmt"]);
                                    drEmpPayroll["Tpy_RESTCOMPHOLNDOTVarAmt"] = Convert.ToDecimal(drEmpPayroll["Tpy_RESTCOMPHOLNDOTAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_RESTCOMPHOLNDOTAmt"]);
                                    drEmpPayroll["Tpy_RESTPSDVarAmt"]       = Convert.ToDecimal(drEmpPayroll["Tpy_RESTPSDAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_RESTPSDAmt"]);
                                    drEmpPayroll["Tpy_RESTPSDOTVarAmt"]     = Convert.ToDecimal(drEmpPayroll["Tpy_RESTPSDOTAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_RESTPSDOTAmt"]);
                                    drEmpPayroll["Tpy_RESTPSDNDVarAmt"]     = Convert.ToDecimal(drEmpPayroll["Tpy_RESTPSDNDAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_RESTPSDNDAmt"]);
                                    drEmpPayroll["Tpy_RESTPSDNDOTVarAmt"]   = Convert.ToDecimal(drEmpPayroll["Tpy_RESTPSDNDOTAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_RESTPSDNDOTAmt"]);
                                    #endregion

                                    #region Total Regular Amount Force Balance 
                                    if (PayrollType == "M")
                                    {
                                        RGRetroAmt = Convert.ToDecimal(drEmpPayroll["Tpy_REGAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_REGAmt"]);
                                        decimal dRetroAdjRegAmt = RGRetroAmt + HLRetroAmt + LVRetroAmt;
                                        decimal dSalaryDiff = ((Convert.ToDecimal(drEmpPayroll["Tpy_SalaryRate"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_SalaryRate"])) / 2);
                                        if (dRetroAdjRegAmt > dSalaryDiff)
                                        {
                                            decimal dRetroAdjForceBalDiff = dRetroAdjRegAmt - dSalaryDiff;
                                            if (dRetroAdjForceBalDiff <= LCSFORCEBAL)
                                            {
                                                drEmpPayroll["Tpy_REGAmt"]          = Convert.ToDecimal(drEmpPayroll["Tpy_REGAmt"]) - dRetroAdjForceBalDiff;
                                                drEmpPayroll["Tpy_TotalREGAmt"]     = Convert.ToDecimal(drEmpPayroll["Tpy_TotalREGAmt"]) - dRetroAdjForceBalDiff;
                                                
                                                RGRetroAmt = 0;
                                                RGRetroAmt = Convert.ToDecimal(drEmpPayroll["Tpy_REGAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpy_REGAmt"]);
                                            }  
                                        }

                                    }
                                    #endregion

                                    #region Total Retro Amount Computation
                                    drEmpPayroll["Tpy_RGRetroAmt"]                          = RGRetroAmt;
                                    drEmpPayroll["Tpy_OTRetroAmt"]                          = OTRetroAmt;
                                    drEmpPayroll["Tpy_NDRetroAmt"]                          = NDRetroAmt;
                                    drEmpPayroll["Tpy_HLRetroAmt"]                          = HLRetroAmt;
                                    drEmpPayroll["Tpy_LVRetroAmt"]                          = LVRetroAmt;
                                    #endregion

                                    #endregion

                                    #region Misc Amount Computation
                                    if (bHasDayCodeExt)
                                    {
                                        drEmpPayrollMisc["Tpm_Misc1Amt"]                    = Misc1Amt;
                                        drEmpPayrollMisc["Tpm_Misc1OTAmt"]                  = Misc1OTAmt;
                                        drEmpPayrollMisc["Tpm_Misc1NDAmt"]                  = Misc1NDAmt;
                                        drEmpPayrollMisc["Tpm_Misc1NDOTAmt"]                = Misc1NDOTAmt;
                                        drEmpPayrollMisc["Tpm_Misc2Amt"]                    = Misc2Amt;
                                        drEmpPayrollMisc["Tpm_Misc2OTAmt"]                  = Misc2OTAmt;
                                        drEmpPayrollMisc["Tpm_Misc2NDAmt"]                  = Misc2NDAmt;
                                        drEmpPayrollMisc["Tpm_Misc2NDOTAmt"]                = Misc2NDOTAmt;
                                        drEmpPayrollMisc["Tpm_Misc3Amt"]                    = Misc3Amt;
                                        drEmpPayrollMisc["Tpm_Misc3OTAmt"]                  = Misc3OTAmt;
                                        drEmpPayrollMisc["Tpm_Misc3NDAmt"]                  = Misc3NDAmt;
                                        drEmpPayrollMisc["Tpm_Misc3NDOTAmt"]                = Misc3NDOTAmt;
                                        drEmpPayrollMisc["Tpm_Misc4Amt"]                    = Misc4Amt;
                                        drEmpPayrollMisc["Tpm_Misc4OTAmt"]                  = Misc4OTAmt;
                                        drEmpPayrollMisc["Tpm_Misc4NDAmt"]                  = Misc4NDAmt;
                                        drEmpPayrollMisc["Tpm_Misc4NDOTAmt"]                = Misc4NDOTAmt;
                                        drEmpPayrollMisc["Tpm_Misc5Amt"]                    = Misc5Amt;
                                        drEmpPayrollMisc["Tpm_Misc5OTAmt"]                  = Misc5OTAmt;
                                        drEmpPayrollMisc["Tpm_Misc5NDAmt"]                  = Misc5NDAmt;
                                        drEmpPayrollMisc["Tpm_Misc5NDOTAmt"]                = Misc5NDOTAmt;
                                        drEmpPayrollMisc["Tpm_Misc6Amt"]                    = Misc6Amt;
                                        drEmpPayrollMisc["Tpm_Misc6OTAmt"]                  = Misc6OTAmt;
                                        drEmpPayrollMisc["Tpm_Misc6NDAmt"]                  = Misc6NDAmt;
                                        drEmpPayrollMisc["Tpm_Misc6NDOTAmt"]                = Misc6NDOTAmt;

                                        #region Variance Misc Computation
                                        drEmpPayrollMisc["Tpm_Misc1VarAmt"]                 = Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc1Amt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpm_Misc1Amt"]);
                                        drEmpPayrollMisc["Tpm_Misc1OTVarAmt"]               = Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc1OTAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpm_Misc1OTAmt"]);
                                        drEmpPayrollMisc["Tpm_Misc1NDVarAmt"]               = Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc1NDAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpm_Misc1NDAmt"]);
                                        drEmpPayrollMisc["Tpm_Misc1NDOTVarAmt"]             = Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc1NDOTAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpm_Misc1NDOTAmt"]);
                                        drEmpPayrollMisc["Tpm_Misc2VarAmt"]                 = Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc2Amt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpm_Misc2Amt"]);
                                        drEmpPayrollMisc["Tpm_Misc2OTVarAmt"]               = Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc2OTAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpm_Misc2OTAmt"]);
                                        drEmpPayrollMisc["Tpm_Misc2NDVarAmt"]               = Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc2NDAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpm_Misc2NDAmt"]);
                                        drEmpPayrollMisc["Tpm_Misc2NDOTVarAmt"]             = Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc2NDOTAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpm_Misc2NDOTAmt"]);
                                        drEmpPayrollMisc["Tpm_Misc3VarAmt"]                 = Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc3Amt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpm_Misc3Amt"]);
                                        drEmpPayrollMisc["Tpm_Misc3OTVarAmt"]               = Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc3OTAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpm_Misc3OTAmt"]);
                                        drEmpPayrollMisc["Tpm_Misc3NDVarAmt"]               = Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc3NDAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpm_Misc3NDAmt"]);
                                        drEmpPayrollMisc["Tpm_Misc3NDOTVarAmt"]             = Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc3NDOTAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpm_Misc3NDOTAmt"]);
                                        drEmpPayrollMisc["Tpm_Misc4VarAmt"]                 = Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc4Amt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpm_Misc4Amt"]);
                                        drEmpPayrollMisc["Tpm_Misc4OTVarAmt"]               = Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc4OTAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpm_Misc4OTAmt"]);
                                        drEmpPayrollMisc["Tpm_Misc4NDVarAmt"]               = Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc4NDAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpm_Misc4NDAmt"]);
                                        drEmpPayrollMisc["Tpm_Misc4NDOTVarAmt"]             = Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc4NDOTAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpm_Misc4NDOTAmt"]);
                                        drEmpPayrollMisc["Tpm_Misc5VarAmt"]                 = Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc5Amt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpm_Misc5Amt"]);
                                        drEmpPayrollMisc["Tpm_Misc5OTVarAmt"]               = Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc5OTAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpm_Misc5OTAmt"]);
                                        drEmpPayrollMisc["Tpm_Misc5NDVarAmt"]               = Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc5NDAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpm_Misc5NDAmt"]);
                                        drEmpPayrollMisc["Tpm_Misc5NDOTVarAmt"]             = Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc5NDOTAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpm_Misc5NDOTAmt"]);
                                        drEmpPayrollMisc["Tpm_Misc6VarAmt"]                 = Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc6Amt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpm_Misc6Amt"]);
                                        drEmpPayrollMisc["Tpm_Misc6OTVarAmt"]               = Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc6OTAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpm_Misc6OTAmt"]);
                                        drEmpPayrollMisc["Tpm_Misc6NDVarAmt"]               = Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc6NDAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpm_Misc6NDAmt"]);
                                        drEmpPayrollMisc["Tpm_Misc6NDOTVarAmt"]             = Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc6NDOTAmt"]) - Convert.ToDecimal(dtPayrollForProcess.Rows[idh]["Tpm_Misc6NDOTAmt"]);
                                        #endregion
                                    }
                                    #endregion

                                    #endregion

                                    #region Rounding Off of Amounts
                                    drEmpPayroll["Tpy_LTAmt"]                               = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_LTAmt"]), 2);
                                    drEmpPayroll["Tpy_LTVarAmt"]                            = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_LTVarAmt"]), 2);
                                    drEmpPayroll["Tpy_UTAmt"]                               = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_UTAmt"]), 2);
                                    drEmpPayroll["Tpy_UTVarAmt"]                            = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_UTVarAmt"]), 2);
                                    drEmpPayroll["Tpy_UPLVAmt"]                             = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_UPLVAmt"]), 2);
                                    drEmpPayroll["Tpy_UPLVVarAmt"]                          = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_UPLVVarAmt"]), 2);
                                    drEmpPayroll["Tpy_ABSLEGHOLAmt"]                        = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_ABSLEGHOLAmt"]), 2);
                                    drEmpPayroll["Tpy_ABSLEGHOLVarAmt"]                     = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_ABSLEGHOLVarAmt"]), 2);
                                    drEmpPayroll["Tpy_ABSSPLHOLAmt"]                        = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_ABSSPLHOLAmt"]), 2);
                                    drEmpPayroll["Tpy_ABSSPLHOLVarAmt"]                     = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_ABSSPLHOLVarAmt"]), 2);
                                    drEmpPayroll["Tpy_ABSCOMPHOLAmt"]                       = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_ABSCOMPHOLAmt"]), 2);
                                    drEmpPayroll["Tpy_ABSCOMPHOLVarAmt"]                    = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_ABSCOMPHOLVarAmt"]), 2);
                                    drEmpPayroll["Tpy_ABSPSDAmt"]                           = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_ABSPSDAmt"]), 2);
                                    drEmpPayroll["Tpy_ABSPSDVarAmt"]                        = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_ABSPSDVarAmt"]), 2);
                                    drEmpPayroll["Tpy_ABSOTHHOLAmt"]                        = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_ABSOTHHOLAmt"]), 2);
                                    drEmpPayroll["Tpy_ABSOTHHOLVarAmt"]                     = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_ABSOTHHOLVarAmt"]), 2);
                                    drEmpPayroll["Tpy_WDABSAmt"]                            = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_WDABSAmt"]), 2);
                                    drEmpPayroll["Tpy_WDABSVarAmt"]                         = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_WDABSVarAmt"]), 2);
                                    drEmpPayroll["Tpy_LTUTMaxAmt"]                          = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_LTUTMaxAmt"]), 2);
                                    drEmpPayroll["Tpy_LTUTMaxVarAmt"]                       = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_LTUTMaxVarAmt"]), 2);
                                    drEmpPayroll["Tpy_ABSAmt"]                              = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_ABSAmt"]), 2);
                                    drEmpPayroll["Tpy_ABSVarAmt"]                           = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_ABSVarAmt"]), 2);
                                    drEmpPayroll["Tpy_REGAmt"]                              = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_REGAmt"]), 2);
                                    drEmpPayroll["Tpy_REGVarAmt"]                           = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_REGVarAmt"]), 2);
                                    drEmpPayroll["Tpy_PDLVAmt"]                             = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_PDLVAmt"]), 2);
                                    drEmpPayroll["Tpy_PDLVVarAmt"]                          = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_PDLVVarAmt"]), 2);
                                    drEmpPayroll["Tpy_PDLEGHOLAmt"]                         = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_PDLEGHOLAmt"]), 2);
                                    drEmpPayroll["Tpy_PDLEGHOLVarAmt"]                      = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_PDLEGHOLVarAmt"]), 2);
                                    drEmpPayroll["Tpy_PDSPLHOLAmt"]                         = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_PDSPLHOLAmt"]), 2);
                                    drEmpPayroll["Tpy_PDSPLHOLVarAmt"]                      = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_PDSPLHOLVarAmt"]), 2);
                                    drEmpPayroll["Tpy_PDCOMPHOLAmt"]                        = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_PDCOMPHOLAmt"]), 2);
                                    drEmpPayroll["Tpy_PDCOMPHOLVarAmt"]                     = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_PDCOMPHOLVarAmt"]), 2);
                                    drEmpPayroll["Tpy_PDPSDAmt"]                            = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_PDPSDAmt"]), 2);
                                    drEmpPayroll["Tpy_PDPSDVarAmt"]                         = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_PDPSDVarAmt"]), 2);
                                    drEmpPayroll["Tpy_PDOTHHOLAmt"]                         = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_PDOTHHOLAmt"]), 2);
                                    drEmpPayroll["Tpy_PDOTHHOLVarAmt"]                      = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_PDOTHHOLVarAmt"]), 2);
                                    drEmpPayroll["Tpy_PDRESTLEGHOLAmt"]                     = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_PDRESTLEGHOLAmt"]), 2);
                                    drEmpPayroll["Tpy_PDRESTLEGHOLVarAmt"]                  = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_PDRESTLEGHOLVarAmt"]), 2);
                                    drEmpPayroll["Tpy_TotalREGAmt"]                         = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_TotalREGAmt"]), 2);
                                    drEmpPayroll["Tpy_TotalREGVarAmt"]                      = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_TotalREGVarAmt"]), 2);
                                    drEmpPayroll["Tpy_REGOTAmt"]                            = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_REGOTAmt"]), 2);
                                    drEmpPayroll["Tpy_REGOTVarAmt"]                         = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_REGOTVarAmt"]), 2);
                                    drEmpPayroll["Tpy_REGNDAmt"]                            = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_REGNDAmt"]), 2);
                                    drEmpPayroll["Tpy_REGNDVarAmt"]                         = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_REGNDVarAmt"]), 2);
                                    drEmpPayroll["Tpy_REGNDOTAmt"]                          = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_REGNDOTAmt"]), 2);
                                    drEmpPayroll["Tpy_REGNDOTVarAmt"]                       = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_REGNDOTVarAmt"]), 2);
                                    drEmpPayroll["Tpy_RESTAmt"]                             = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTAmt"]), 2);
                                    drEmpPayroll["Tpy_RESTVarAmt"]                          = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTVarAmt"]), 2);
                                    drEmpPayroll["Tpy_RESTOTAmt"]                           = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTOTAmt"]), 2);
                                    drEmpPayroll["Tpy_RESTOTVarAmt"]                        = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTOTVarAmt"]), 2);
                                    drEmpPayroll["Tpy_RESTNDAmt"]                           = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTNDAmt"]), 2);
                                    drEmpPayroll["Tpy_RESTNDVarAmt"]                        = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTNDVarAmt"]), 2);
                                    drEmpPayroll["Tpy_RESTNDOTAmt"]                         = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTNDOTAmt"]), 2);
                                    drEmpPayroll["Tpy_RESTNDOTVarAmt"]                      = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTNDOTVarAmt"]), 2);
                                    drEmpPayroll["Tpy_LEGHOLAmt"]                           = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_LEGHOLAmt"]), 2);
                                    drEmpPayroll["Tpy_LEGHOLVarAmt"]                        = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_LEGHOLVarAmt"]), 2);
                                    drEmpPayroll["Tpy_LEGHOLOTAmt"]                         = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_LEGHOLOTAmt"]), 2);
                                    drEmpPayroll["Tpy_LEGHOLOTVarAmt"]                      = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_LEGHOLOTVarAmt"]), 2);
                                    drEmpPayroll["Tpy_LEGHOLNDAmt"]                         = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_LEGHOLNDAmt"]), 2);
                                    drEmpPayroll["Tpy_LEGHOLNDVarAmt"]                      = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_LEGHOLNDVarAmt"]), 2);
                                    drEmpPayroll["Tpy_LEGHOLNDOTAmt"]                       = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_LEGHOLNDOTAmt"]), 2);
                                    drEmpPayroll["Tpy_LEGHOLNDOTVarAmt"]                    = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_LEGHOLNDOTVarAmt"]), 2);
                                    drEmpPayroll["Tpy_SPLHOLAmt"]                           = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_SPLHOLAmt"]), 2);
                                    drEmpPayroll["Tpy_SPLHOLVarAmt"]                        = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_SPLHOLVarAmt"]), 2);
                                    drEmpPayroll["Tpy_SPLHOLOTAmt"]                         = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_SPLHOLOTAmt"]), 2);
                                    drEmpPayroll["Tpy_SPLHOLOTVarAmt"]                      = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_SPLHOLOTVarAmt"]), 2);
                                    drEmpPayroll["Tpy_SPLHOLNDAmt"]                         = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_SPLHOLNDAmt"]), 2);
                                    drEmpPayroll["Tpy_SPLHOLNDVarAmt"]                      = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_SPLHOLNDVarAmt"]), 2);
                                    drEmpPayroll["Tpy_SPLHOLNDOTAmt"]                       = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_SPLHOLNDOTAmt"]), 2);
                                    drEmpPayroll["Tpy_SPLHOLNDOTVarAmt"]                    = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_SPLHOLNDOTVarAmt"]), 2);
                                    drEmpPayroll["Tpy_PSDAmt"]                              = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_PSDAmt"]), 2);
                                    drEmpPayroll["Tpy_PSDVarAmt"]                           = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_PSDVarAmt"]), 2);
                                    drEmpPayroll["Tpy_PSDOTAmt"]                            = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_PSDOTAmt"]), 2);
                                    drEmpPayroll["Tpy_PSDOTVarAmt"]                         = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_PSDOTVarAmt"]), 2);
                                    drEmpPayroll["Tpy_PSDNDAmt"]                            = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_PSDNDAmt"]), 2);
                                    drEmpPayroll["Tpy_PSDNDVarAmt"]                         = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_PSDNDVarAmt"]), 2);
                                    drEmpPayroll["Tpy_PSDNDOTAmt"]                          = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_PSDNDOTAmt"]), 2);
                                    drEmpPayroll["Tpy_PSDNDOTVarAmt"]                       = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_PSDNDOTVarAmt"]), 2);
                                    drEmpPayroll["Tpy_COMPHOLAmt"]                          = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_COMPHOLAmt"]), 2);
                                    drEmpPayroll["Tpy_COMPHOLVarAmt"]                       = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_COMPHOLVarAmt"]), 2);
                                    drEmpPayroll["Tpy_COMPHOLOTAmt"]                        = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_COMPHOLOTAmt"]), 2);
                                    drEmpPayroll["Tpy_COMPHOLOTVarAmt"]                     = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_COMPHOLOTVarAmt"]), 2);
                                    drEmpPayroll["Tpy_COMPHOLNDAmt"]                        = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_COMPHOLNDAmt"]), 2);
                                    drEmpPayroll["Tpy_COMPHOLNDVarAmt"]                     = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_COMPHOLNDVarAmt"]), 2);
                                    drEmpPayroll["Tpy_COMPHOLNDOTAmt"]                      = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_COMPHOLNDOTAmt"]), 2);
                                    drEmpPayroll["Tpy_COMPHOLNDOTVarAmt"]                   = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_COMPHOLNDOTVarAmt"]), 2);
                                    drEmpPayroll["Tpy_RESTLEGHOLAmt"]                       = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTLEGHOLAmt"]), 2);
                                    drEmpPayroll["Tpy_RESTLEGHOLVarAmt"]                    = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTLEGHOLVarAmt"]), 2);
                                    drEmpPayroll["Tpy_RESTLEGHOLOTAmt"]                     = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTLEGHOLOTAmt"]), 2);
                                    drEmpPayroll["Tpy_RESTLEGHOLOTVarAmt"]                  = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTLEGHOLOTVarAmt"]), 2);
                                    drEmpPayroll["Tpy_RESTLEGHOLNDAmt"]                     = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTLEGHOLNDAmt"]), 2);
                                    drEmpPayroll["Tpy_RESTLEGHOLNDVarAmt"]                  = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTLEGHOLNDVarAmt"]), 2);
                                    drEmpPayroll["Tpy_RESTLEGHOLNDOTAmt"]                   = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTLEGHOLNDOTAmt"]), 2);
                                    drEmpPayroll["Tpy_RESTLEGHOLNDOTVarAmt"]                = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTLEGHOLNDOTVarAmt"]), 2);
                                    drEmpPayroll["Tpy_RESTSPLHOLAmt"]                       = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTSPLHOLAmt"]), 2);
                                    drEmpPayroll["Tpy_RESTSPLHOLVarAmt"]                    = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTSPLHOLVarAmt"]), 2);
                                    drEmpPayroll["Tpy_RESTSPLHOLOTAmt"]                     = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTSPLHOLOTAmt"]), 2);
                                    drEmpPayroll["Tpy_RESTSPLHOLOTVarAmt"]                  = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTSPLHOLOTVarAmt"]), 2);
                                    drEmpPayroll["Tpy_RESTSPLHOLNDAmt"]                     = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTSPLHOLNDAmt"]), 2);
                                    drEmpPayroll["Tpy_RESTSPLHOLNDVarAmt"]                  = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTSPLHOLNDVarAmt"]), 2);
                                    drEmpPayroll["Tpy_RESTSPLHOLNDOTAmt"]                   = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTSPLHOLNDOTAmt"]), 2);
                                    drEmpPayroll["Tpy_RESTSPLHOLNDOTVarAmt"]                = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTSPLHOLNDOTVarAmt"]), 2);
                                    drEmpPayroll["Tpy_RESTCOMPHOLAmt"]                      = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTCOMPHOLAmt"]), 2);
                                    drEmpPayroll["Tpy_RESTCOMPHOLVarAmt"]                   = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTCOMPHOLVarAmt"]), 2);
                                    drEmpPayroll["Tpy_RESTCOMPHOLOTAmt"]                    = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTCOMPHOLOTAmt"]), 2);
                                    drEmpPayroll["Tpy_RESTCOMPHOLOTVarAmt"]                 = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTCOMPHOLOTVarAmt"]), 2);
                                    drEmpPayroll["Tpy_RESTCOMPHOLNDAmt"]                    = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTCOMPHOLNDAmt"]), 2);
                                    drEmpPayroll["Tpy_RESTCOMPHOLNDVarAmt"]                 = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTCOMPHOLNDVarAmt"]), 2);
                                    drEmpPayroll["Tpy_RESTCOMPHOLNDOTAmt"]                  = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTCOMPHOLNDOTAmt"]), 2);
                                    drEmpPayroll["Tpy_RESTCOMPHOLNDOTVarAmt"]               = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTCOMPHOLNDOTVarAmt"]), 2);
                                    drEmpPayroll["Tpy_RESTPSDAmt"]                          = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTPSDAmt"]), 2);
                                    drEmpPayroll["Tpy_RESTPSDVarAmt"]                       = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTPSDVarAmt"]), 2);
                                    drEmpPayroll["Tpy_RESTPSDOTAmt"]                        = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTPSDOTAmt"]), 2);
                                    drEmpPayroll["Tpy_RESTPSDOTVarAmt"]                     = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTPSDOTVarAmt"]), 2);
                                    drEmpPayroll["Tpy_RESTPSDNDAmt"]                        = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTPSDNDAmt"]), 2);
                                    drEmpPayroll["Tpy_RESTPSDNDVarAmt"]                     = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTPSDNDVarAmt"]), 2);
                                    drEmpPayroll["Tpy_RESTPSDNDOTAmt"]                      = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTPSDNDOTAmt"]), 2);
                                    drEmpPayroll["Tpy_RESTPSDNDOTVarAmt"]                   = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTPSDNDOTVarAmt"]), 2);
                                    drEmpPayroll["Tpy_RGRetroAmt"]                          = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RGRetroAmt"]), 2);
                                    drEmpPayroll["Tpy_OTRetroAmt"]                          = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_OTRetroAmt"]), 2);
                                    drEmpPayroll["Tpy_NDRetroAmt"]                          = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_NDRetroAmt"]), 2);
                                    drEmpPayroll["Tpy_HLRetroAmt"]                          = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_HLRetroAmt"]), 2);
                                    drEmpPayroll["Tpy_LVRetroAmt"]                          = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_LVRetroAmt"]), 2);
                                    drEmpPayroll["Tpy_TotalRetroAmt"]                       = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_TotalRetroAmt"]), 2);

                                    if (bHasDayCodeExt)
                                    {
                                        drEmpPayrollMisc["Tpm_Misc1Amt"]                    = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc1Amt"]), 2);
                                        drEmpPayrollMisc["Tpm_Misc1VarAmt"]                 = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc1VarAmt"]), 2);
                                        drEmpPayrollMisc["Tpm_Misc1OTAmt"]                  = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc1OTAmt"]), 2);
                                        drEmpPayrollMisc["Tpm_Misc1OTVarAmt"]               = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc1OTVarAmt"]), 2);
                                        drEmpPayrollMisc["Tpm_Misc1NDAmt"]                  = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc1NDAmt"]), 2);
                                        drEmpPayrollMisc["Tpm_Misc1NDVarAmt"]               = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc1NDVarAmt"]), 2);
                                        drEmpPayrollMisc["Tpm_Misc1NDOTAmt"]                = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc1NDOTAmt"]), 2);
                                        drEmpPayrollMisc["Tpm_Misc1NDOTVarAmt"]             = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc1NDOTVarAmt"]), 2);
                                        drEmpPayrollMisc["Tpm_Misc2Amt"]                    = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc2Amt"]), 2);
                                        drEmpPayrollMisc["Tpm_Misc2VarAmt"]                 = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc2VarAmt"]), 2);
                                        drEmpPayrollMisc["Tpm_Misc2OTAmt"]                  = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc2OTAmt"]), 2);
                                        drEmpPayrollMisc["Tpm_Misc2OTVarAmt"]               = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc2OTVarAmt"]), 2);
                                        drEmpPayrollMisc["Tpm_Misc2NDAmt"]                  = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc2NDAmt"]), 2);
                                        drEmpPayrollMisc["Tpm_Misc2NDVarAmt"]               = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc2NDVarAmt"]), 2);
                                        drEmpPayrollMisc["Tpm_Misc2NDOTAmt"]                = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc2NDOTAmt"]), 2);
                                        drEmpPayrollMisc["Tpm_Misc2NDOTVarAmt"]             = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc2NDOTVarAmt"]), 2);
                                        drEmpPayrollMisc["Tpm_Misc3Amt"]                    = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc3Amt"]), 2);
                                        drEmpPayrollMisc["Tpm_Misc3VarAmt"]                 = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc3VarAmt"]), 2);
                                        drEmpPayrollMisc["Tpm_Misc3OTAmt"]                  = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc3OTAmt"]), 2);
                                        drEmpPayrollMisc["Tpm_Misc3OTVarAmt"]               = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc3OTVarAmt"]), 2);
                                        drEmpPayrollMisc["Tpm_Misc3NDAmt"]                  = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc3NDAmt"]), 2);
                                        drEmpPayrollMisc["Tpm_Misc3NDVarAmt"]               = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc3NDVarAmt"]), 2);
                                        drEmpPayrollMisc["Tpm_Misc3NDOTAmt"]                = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc3NDOTAmt"]), 2);
                                        drEmpPayrollMisc["Tpm_Misc3NDOTVarAmt"]             = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc3NDOTVarAmt"]), 2);
                                        drEmpPayrollMisc["Tpm_Misc4Amt"]                    = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc4Amt"]), 2);
                                        drEmpPayrollMisc["Tpm_Misc4VarAmt"]                 = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc4VarAmt"]), 2);
                                        drEmpPayrollMisc["Tpm_Misc4OTAmt"]                  = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc4OTAmt"]), 2);
                                        drEmpPayrollMisc["Tpm_Misc4OTVarAmt"]               = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc4OTVarAmt"]), 2);
                                        drEmpPayrollMisc["Tpm_Misc4NDAmt"]                  = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc4NDAmt"]), 2);
                                        drEmpPayrollMisc["Tpm_Misc4NDVarAmt"]               = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc4NDVarAmt"]), 2);
                                        drEmpPayrollMisc["Tpm_Misc4NDOTAmt"]                = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc4NDOTAmt"]), 2);
                                        drEmpPayrollMisc["Tpm_Misc4NDOTVarAmt"]             = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc4NDOTVarAmt"]), 2);
                                        drEmpPayrollMisc["Tpm_Misc5Amt"]                    = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc5Amt"]), 2);
                                        drEmpPayrollMisc["Tpm_Misc5VarAmt"]                 = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc5VarAmt"]), 2);
                                        drEmpPayrollMisc["Tpm_Misc5OTAmt"]                  = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc5OTAmt"]), 2);
                                        drEmpPayrollMisc["Tpm_Misc5OTVarAmt"]               = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc5OTVarAmt"]), 2);
                                        drEmpPayrollMisc["Tpm_Misc5NDAmt"]                  = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc5NDAmt"]), 2);
                                        drEmpPayrollMisc["Tpm_Misc5NDVarAmt"]               = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc5NDVarAmt"]), 2);
                                        drEmpPayrollMisc["Tpm_Misc5NDOTAmt"]                = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc5NDOTAmt"]), 2);
                                        drEmpPayrollMisc["Tpm_Misc5NDOTVarAmt"]             = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc5NDOTVarAmt"]), 2);
                                        drEmpPayrollMisc["Tpm_Misc6Amt"]                    = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc6Amt"]), 2);
                                        drEmpPayrollMisc["Tpm_Misc6VarAmt"]                 = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc6VarAmt"]), 2);
                                        drEmpPayrollMisc["Tpm_Misc6OTAmt"]                  = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc6OTAmt"]), 2);
                                        drEmpPayrollMisc["Tpm_Misc6OTVarAmt"]               = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc6OTVarAmt"]), 2);
                                        drEmpPayrollMisc["Tpm_Misc6NDAmt"]                  = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc6NDAmt"]), 2);
                                        drEmpPayrollMisc["Tpm_Misc6NDVarAmt"]               = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc6NDVarAmt"]), 2);
                                        drEmpPayrollMisc["Tpm_Misc6NDOTAmt"]                = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc6NDOTAmt"]), 2);
                                        drEmpPayrollMisc["Tpm_Misc6NDVarAmt"]               = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc6NDVarAmt"]), 2);
                                    }
                                    #endregion

                                    #region Grand Total Retro Amount Computation
                                    drEmpPayroll["Tpy_TotalRetroAmt"] = Convert.ToDecimal(drEmpPayroll["Tpy_RGRetroAmt"]) 
                                                                           + Convert.ToDecimal(drEmpPayroll["Tpy_OTRetroAmt"])
                                                                           + Convert.ToDecimal(drEmpPayroll["Tpy_NDRetroAmt"]) 
                                                                           + Convert.ToDecimal(drEmpPayroll["Tpy_HLRetroAmt"])
                                                                           + Convert.ToDecimal(drEmpPayroll["Tpy_LVRetroAmt"]);

                                    #endregion

                                    #region Add to Retropay Total Amount
                                    EmpRGRetroAmt += Convert.ToDecimal(drEmpPayroll["Tpy_RGRetroAmt"]);
                                    EmpOTRetroAmt += Convert.ToDecimal(drEmpPayroll["Tpy_OTRetroAmt"]);
                                    EmpNDRetroAmt += Convert.ToDecimal(drEmpPayroll["Tpy_NDRetroAmt"]);
                                    EmpHLRetroAmt += Convert.ToDecimal(drEmpPayroll["Tpy_HLRetroAmt"]);
                                    EmpLVRetroAmt += Convert.ToDecimal(drEmpPayroll["Tpy_LVRetroAmt"]);
                                    EmpTotalRetroAmt += Convert.ToDecimal(drEmpPayroll["Tpy_TotalRetroAmt"]);
                                    #endregion

                                    #region Total OT and ND Computation
                                    drEmpPayroll["Tpy_TotalOTNDAmt"] = Convert.ToDecimal(drEmpPayroll["Tpy_REGOTAmt"])
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_REGNDAmt"])
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_REGNDOTAmt"])
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_RESTAmt"])
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_RESTOTAmt"])
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_RESTNDAmt"])
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_RESTNDOTAmt"])
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_LEGHOLAmt"])
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_LEGHOLOTAmt"])
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_LEGHOLNDAmt"])
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_LEGHOLNDOTAmt"])
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_SPLHOLAmt"])
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_SPLHOLOTAmt"])
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_SPLHOLNDAmt"])
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_SPLHOLNDOTAmt"])
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_PSDAmt"])
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_PSDOTAmt"])
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_PSDNDAmt"])
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_PSDNDOTAmt"])
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_COMPHOLAmt"])
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_COMPHOLOTAmt"])
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_COMPHOLNDAmt"])
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_COMPHOLNDOTAmt"])
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_RESTLEGHOLAmt"])
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_RESTLEGHOLOTAmt"])
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_RESTLEGHOLNDAmt"])
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_RESTLEGHOLNDOTAmt"])
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_RESTSPLHOLAmt"])
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_RESTSPLHOLOTAmt"])
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_RESTSPLHOLNDAmt"])
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_RESTSPLHOLNDOTAmt"])
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_RESTCOMPHOLAmt"])
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_RESTCOMPHOLOTAmt"])
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_RESTCOMPHOLNDAmt"])
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_RESTCOMPHOLNDOTAmt"])
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_RESTPSDAmt"])
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_RESTPSDOTAmt"])
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_RESTPSDNDAmt"])
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_RESTPSDNDOTAmt"]);

                                    if (bHasDayCodeExt)
                                    {
                                        drEmpPayroll["Tpy_TotalOTNDAmt"] = Convert.ToDecimal(drEmpPayroll["Tpy_TotalOTNDAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc1Amt"])
                                                                                + Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc1OTAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc1NDAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc1NDOTAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc2Amt"])
                                                                                + Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc2OTAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc2NDAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc2NDOTAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc3Amt"])
                                                                                + Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc3OTAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc3NDAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc3NDOTAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc4Amt"])
                                                                                + Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc4OTAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc4NDAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc4NDOTAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc5Amt"])
                                                                                + Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc5OTAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc5NDAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc5NDOTAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc6Amt"])
                                                                                + Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc6OTAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc6NDAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc6NDOTAmt"]);
                                    }

                                    #endregion

                                    #endregion

                                    //Add to payroll tables
                                    dtEmpPayroll.Rows.Add(drEmpPayroll);

                                    if (bHasDayCodeExt)
                                        dtEmpPayrollMisc.Rows.Add(drEmpPayrollMisc);
                                }
                            }

                            #region System Adjustment
                            dtSystemAdj2ForProcess = GetAllEmployeeSystemAdjustmentForProcess(curEmployeeId, SalaryDate);
                            for (int ids = 0; ids < dtSystemAdj2ForProcess.Rows.Count; ids++)
                            {
                                #region Initialize System Adj2 Row
                                drEmpSystemAdj2 = dtEmpSystemAdj2.NewRow();

                                drEmpSystemAdj2["Tsa_IDNo"]                             = curEmployeeId;
                                drEmpSystemAdj2["Tsa_RetroPayCycle"]                    = PayrollPeriod;
                                drEmpSystemAdj2["Tsa_AdjPayCycle"]                      = dtSystemAdj2ForProcess.Rows[ids]["Tsa_AdjPayCycle"].ToString();
                                drEmpSystemAdj2["Tsa_OrigAdjPayCycle"]                  = dtSystemAdj2ForProcess.Rows[ids]["Tsa_OrigAdjPayCycle"].ToString();
                                drEmpSystemAdj2["Tsa_PayCycle"]                         = dtSystemAdj2ForProcess.Rows[ids]["Tsa_PayCycle"].ToString();
                                drEmpSystemAdj2["Tsa_Date"]                             = dtSystemAdj2ForProcess.Rows[ids]["Tsa_Date"];
                                drEmpSystemAdj2["Tsa_SalaryRate"]                       = SalaryRate;
                                drEmpSystemAdj2["Tsa_HourRate"]                         = HourlyRate;
                                drEmpSystemAdj2["Tsa_PayrollType"]                      = PayrollType;
                                drEmpSystemAdj2["Tsa_SpecialRate"]                      = SpecialSalaryRate;
                                drEmpSystemAdj2["Tsa_SpecialHourRate"]                  = SpecialHourlyRate;
                                drEmpSystemAdj2["Tsa_PremiumGrpCode"]                   = dtSystemAdj2ForProcess.Rows[ids]["Tsa_PremiumGrpCode"].ToString();
                                drEmpSystemAdj2["Tsa_LTAmt"]                            = 0;
                                //drEmpSystemAdj2["Tsa_LTVarAmt"]                         = 0;
                                drEmpSystemAdj2["Tsa_UTAmt"]                            = 0;
                                //drEmpSystemAdj2["Tsa_UTVarAmt"]                         = 0;
                                drEmpSystemAdj2["Tsa_UPLVAmt"]                          = 0;
                                //drEmpSystemAdj2["Tsa_UPLVVarAmt"]                       = 0;
                                drEmpSystemAdj2["Tsa_ABSLEGHOLAmt"]                     = 0;
                                //drEmpSystemAdj2["Tsa_ABSLEGHOLVarAmt"]                  = 0;
                                drEmpSystemAdj2["Tsa_ABSSPLHOLAmt"]                     = 0;
                                //drEmpSystemAdj2["Tsa_ABSSPLHOLVarAmt"]                  = 0;
                                drEmpSystemAdj2["Tsa_ABSCOMPHOLAmt"]                    = 0;
                                //drEmpSystemAdj2["Tsa_ABSCOMPHOLVarAmt"]                 = 0;
                                drEmpSystemAdj2["Tsa_ABSPSDAmt"]                        = 0;
                                //drEmpSystemAdj2["Tsa_ABSPSDVarAmt"]                     = 0;
                                drEmpSystemAdj2["Tsa_ABSOTHHOLAmt"]                     = 0;
                                //drEmpSystemAdj2["Tsa_ABSOTHHOLVarAmt"]                  = 0;
                                drEmpSystemAdj2["Tsa_WDABSAmt"]                         = 0;
                                //drEmpSystemAdj2["Tsa_WDABSVarAmt"]                      = 0;
                                drEmpSystemAdj2["Tsa_LTUTMaxAmt"]                       = 0;
                                //drEmpSystemAdj2["Tsa_LTUTMaxVarAmt"]                    = 0;
                                drEmpSystemAdj2["Tsa_ABSAmt"]                           = 0;
                                //drEmpSystemAdj2["Tsa_ABSVarAmt"]                        = 0;
                                drEmpSystemAdj2["Tsa_REGAmt"]                           = 0;
                                //drEmpSystemAdj2["Tsa_REGVarAmt"]                        = 0;
                                drEmpSystemAdj2["Tsa_PDLVAmt"]                          = 0;
                                //drEmpSystemAdj2["Tsa_PDLVVarAmt"]                       = 0;
                                drEmpSystemAdj2["Tsa_PDLEGHOLAmt"]                      = 0;
                                //drEmpSystemAdj2["Tsa_PDLEGHOLVarAmt"]                   = 0;
                                drEmpSystemAdj2["Tsa_PDSPLHOLAmt"]                      = 0;
                                //drEmpSystemAdj2["Tsa_PDSPLHOLVarAmt"]                   = 0;
                                drEmpSystemAdj2["Tsa_PDCOMPHOLAmt"]                     = 0;
                                //drEmpSystemAdj2["Tsa_PDCOMPHOLVarAmt"]                  = 0;
                                drEmpSystemAdj2["Tsa_PDPSDAmt"]                         = 0;
                                //drEmpSystemAdj2["Tsa_PDPSDVarAmt"]                      = 0;
                                drEmpSystemAdj2["Tsa_PDOTHHOLAmt"]                      = 0;
                                //drEmpSystemAdj2["Tsa_PDOTHHOLVarAmt"]                   = 0;
                                drEmpSystemAdj2["Tsa_PDRESTLEGHOLAmt"]                  = 0;
                                //drEmpSystemAdj2["Tsa_PDRESTLEGHOLVarAmt"]               = 0;
                                drEmpSystemAdj2["Tsa_REGOTAmt"]                         = 0;
                                //drEmpSystemAdj2["Tsa_REGOTVarAmt"]                      = 0;
                                drEmpSystemAdj2["Tsa_REGNDAmt"]                         = 0;
                                //drEmpSystemAdj2["Tsa_REGNDVarAmt"]                      = 0;
                                drEmpSystemAdj2["Tsa_REGNDOTAmt"]                       = 0;
                                //drEmpSystemAdj2["Tsa_REGNDOTVarAmt"]                    = 0;
                                drEmpSystemAdj2["Tsa_RESTAmt"]                          = 0;
                                //drEmpSystemAdj2["Tsa_RESTVarAmt"]                       = 0;
                                drEmpSystemAdj2["Tsa_RESTOTAmt"]                        = 0;
                                //drEmpSystemAdj2["Tsa_RESTOTVarAmt"]                     = 0;
                                drEmpSystemAdj2["Tsa_RESTNDAmt"]                        = 0;
                                //drEmpSystemAdj2["Tsa_RESTNDVarAmt"]                     = 0;
                                drEmpSystemAdj2["Tsa_RESTNDOTAmt"]                      = 0;
                                //drEmpSystemAdj2["Tsa_RESTNDOTVarAmt"]                   = 0;
                                drEmpSystemAdj2["Tsa_LEGHOLAmt"]                        = 0;
                                //drEmpSystemAdj2["Tsa_LEGHOLVarAmt"]                     = 0;
                                drEmpSystemAdj2["Tsa_LEGHOLOTAmt"]                      = 0;
                                //drEmpSystemAdj2["Tsa_LEGHOLOTVarAmt"]                   = 0;
                                drEmpSystemAdj2["Tsa_LEGHOLNDAmt"]                      = 0;
                                //drEmpSystemAdj2["Tsa_LEGHOLNDVarAmt"]                   = 0;
                                drEmpSystemAdj2["Tsa_LEGHOLNDOTAmt"]                    = 0;
                                //drEmpSystemAdj2["Tsa_LEGHOLNDOTVarAmt"]                 = 0;
                                drEmpSystemAdj2["Tsa_SPLHOLAmt"]                        = 0;
                                //drEmpSystemAdj2["Tsa_SPLHOLVarAmt"]                     = 0;
                                drEmpSystemAdj2["Tsa_SPLHOLOTAmt"]                      = 0;
                                //drEmpSystemAdj2["Tsa_SPLHOLOTVarAmt"]                   = 0;
                                drEmpSystemAdj2["Tsa_SPLHOLNDAmt"]                      = 0;
                                //drEmpSystemAdj2["Tsa_SPLHOLNDVarAmt"]                   = 0;
                                drEmpSystemAdj2["Tsa_SPLHOLNDOTAmt"]                    = 0;
                                //drEmpSystemAdj2["Tsa_SPLHOLNDOTVarAmt"]                 = 0;
                                drEmpSystemAdj2["Tsa_PSDAmt"]                           = 0;
                                //drEmpSystemAdj2["Tsa_PSDVarAmt"]                        = 0;
                                drEmpSystemAdj2["Tsa_PSDOTAmt"]                         = 0;
                                //drEmpSystemAdj2["Tsa_PSDOTVarAmt"]                      = 0;
                                drEmpSystemAdj2["Tsa_PSDNDAmt"]                         = 0;
                                //drEmpSystemAdj2["Tsa_PSDNDVarAmt"]                      = 0;
                                drEmpSystemAdj2["Tsa_PSDNDOTAmt"]                       = 0;
                                //drEmpSystemAdj2["Tsa_PSDNDOTVarAmt"]                    = 0;
                                drEmpSystemAdj2["Tsa_COMPHOLAmt"]                       = 0;
                                //drEmpSystemAdj2["Tsa_COMPHOLVarAmt"]                    = 0;
                                drEmpSystemAdj2["Tsa_COMPHOLOTAmt"]                     = 0;
                                //drEmpSystemAdj2["Tsa_COMPHOLOTVarAmt"]                  = 0;
                                drEmpSystemAdj2["Tsa_COMPHOLNDAmt"]                     = 0;
                                //drEmpSystemAdj2["Tsa_COMPHOLNDVarAmt"]                  = 0;
                                drEmpSystemAdj2["Tsa_COMPHOLNDOTAmt"]                   = 0;
                                //drEmpSystemAdj2["Tsa_COMPHOLNDOTVarAmt"]                = 0;
                                drEmpSystemAdj2["Tsa_RESTLEGHOLAmt"]                    = 0;
                                //drEmpSystemAdj2["Tsa_RESTLEGHOLVarAmt"]                 = 0;
                                drEmpSystemAdj2["Tsa_RESTLEGHOLOTAmt"]                  = 0;
                                //drEmpSystemAdj2["Tsa_RESTLEGHOLOTVarAmt"]               = 0;
                                drEmpSystemAdj2["Tsa_RESTLEGHOLNDAmt"]                  = 0;
                                //drEmpSystemAdj2["Tsa_RESTLEGHOLNDVarAmt"]               = 0;
                                drEmpSystemAdj2["Tsa_RESTLEGHOLNDOTAmt"]                = 0;
                                //drEmpSystemAdj2["Tsa_RESTLEGHOLNDOTVarAmt"]             = 0;
                                drEmpSystemAdj2["Tsa_RESTSPLHOLAmt"]                    = 0;
                                //drEmpSystemAdj2["Tsa_RESTSPLHOLVarAmt"]                 = 0;
                                drEmpSystemAdj2["Tsa_RESTSPLHOLOTAmt"]                  = 0;
                                //drEmpSystemAdj2["Tsa_RESTSPLHOLOTVarAmt"]               = 0;
                                drEmpSystemAdj2["Tsa_RESTSPLHOLNDAmt"]                  = 0;
                                //drEmpSystemAdj2["Tsa_RESTSPLHOLNDVarAmt"]               = 0;
                                drEmpSystemAdj2["Tsa_RESTSPLHOLNDOTAmt"]                = 0;
                                //drEmpSystemAdj2["Tsa_RESTSPLHOLNDOTVarAmt"]             = 0;
                                drEmpSystemAdj2["Tsa_RESTCOMPHOLAmt"]                   = 0;
                                //drEmpSystemAdj2["Tsa_RESTCOMPHOLVarAmt"]                = 0;
                                drEmpSystemAdj2["Tsa_RESTCOMPHOLOTAmt"]                 = 0;
                                //drEmpSystemAdj2["Tsa_RESTCOMPHOLOTVarAmt"]              = 0;
                                drEmpSystemAdj2["Tsa_RESTCOMPHOLNDAmt"]                 = 0;
                                //drEmpSystemAdj2["Tsa_RESTCOMPHOLNDVarAmt"]              = 0;
                                drEmpSystemAdj2["Tsa_RESTCOMPHOLNDOTAmt"]               = 0;
                                //drEmpSystemAdj2["Tsa_RESTCOMPHOLNDOTVarAmt"]            = 0;
                                drEmpSystemAdj2["Tsa_RESTPSDAmt"]                       = 0;
                                drEmpSystemAdj2["Tsa_RESTPSDVarAmt"]                    = 0;
                                drEmpSystemAdj2["Tsa_RESTPSDOTAmt"]                     = 0;
                                drEmpSystemAdj2["Tsa_RESTPSDOTVarAmt"]                  = 0;
                                drEmpSystemAdj2["Tsa_RESTPSDNDAmt"]                     = 0;
                                drEmpSystemAdj2["Tsa_RESTPSDNDVarAmt"]                  = 0;
                                drEmpSystemAdj2["Tsa_RESTPSDNDOTAmt"]                   = 0;
                                drEmpSystemAdj2["Tsa_RESTPSDNDOTVarAmt"]                = 0;
                                drEmpSystemAdj2["Tsa_RGAdjAmt"]                         = 0;
                                drEmpSystemAdj2["Tsa_OTAdjAmt"]                         = 0;
                                drEmpSystemAdj2["Tsa_HOLAdjAmt"]                        = 0;
                                drEmpSystemAdj2["Tsa_NDAdjAmt"]                         = 0;
                                drEmpSystemAdj2["Tsa_LVAdjAmt"]                         = 0;
                                drEmpSystemAdj2["Tsa_TotalAdjAmt"]                      = 0;
                                drEmpSystemAdj2["Tsa_RGRetroAmt"]                       = 0;
                                drEmpSystemAdj2["Tsa_OTRetroAmt"]                       = 0;
                                drEmpSystemAdj2["Tsa_NDRetroAmt"]                       = 0;
                                drEmpSystemAdj2["Tsa_HLRetroAmt"]                       = 0;
                                drEmpSystemAdj2["Tsa_LVRetroAmt"]                       = 0;
                                drEmpSystemAdj2["Tsa_TotalRetroAmt"]                    = 0;
                                drEmpSystemAdj2["Usr_Login"]                            = LoginUser;
                                drEmpSystemAdj2["Ludatetime"]                           = DateTime.Now;
                                #endregion

                                if (bHasDayCodeExt)
                                {
                                    #region Initialize System Adj2 Misc Row
                                    drEmpSystemAdj2Misc = dtEmpSystemAdj2Misc.NewRow();

                                    drEmpSystemAdj2Misc["Tsm_IDNo"]                     = curEmployeeId;
                                    drEmpSystemAdj2Misc["Tsm_RetroPayCycle"]            = PayrollPeriod;
                                    drEmpSystemAdj2Misc["Tsm_AdjPayCycle"]              = dtSystemAdj2ForProcess.Rows[ids]["Tsa_AdjPayCycle"].ToString();
                                    drEmpSystemAdj2Misc["Tsm_OrigAdjPayCycle"]          = dtSystemAdj2ForProcess.Rows[ids]["Tsa_OrigAdjPayCycle"].ToString();
                                    drEmpSystemAdj2Misc["Tsm_PayCycle"]                 = dtSystemAdj2ForProcess.Rows[ids]["Tsa_PayCycle"].ToString();
                                    drEmpSystemAdj2Misc["Tsm_Date"]                     = dtSystemAdj2ForProcess.Rows[ids]["Tsa_Date"];
                                    drEmpSystemAdj2Misc["Tsm_Misc1Amt"]                 = 0;
                                    drEmpSystemAdj2Misc["Tsm_Misc1OTAmt"]               = 0;
                                    drEmpSystemAdj2Misc["Tsm_Misc1NDAmt"]               = 0;
                                    drEmpSystemAdj2Misc["Tsm_Misc1NDOTAmt"]             = 0;
                                    drEmpSystemAdj2Misc["Tsm_Misc2Amt"]                 = 0;
                                    drEmpSystemAdj2Misc["Tsm_Misc2OTAmt"]               = 0;
                                    drEmpSystemAdj2Misc["Tsm_Misc2NDAmt"]               = 0;
                                    drEmpSystemAdj2Misc["Tsm_Misc2NDOTAmt"]             = 0;
                                    drEmpSystemAdj2Misc["Tsm_Misc3Amt"]                 = 0;
                                    drEmpSystemAdj2Misc["Tsm_Misc3OTAmt"]               = 0;
                                    drEmpSystemAdj2Misc["Tsm_Misc3NDAmt"]               = 0;
                                    drEmpSystemAdj2Misc["Tsm_Misc3NDOTAmt"]             = 0;
                                    drEmpSystemAdj2Misc["Tsm_Misc4Amt"]                 = 0;
                                    drEmpSystemAdj2Misc["Tsm_Misc4OTAmt"]               = 0;
                                    drEmpSystemAdj2Misc["Tsm_Misc4NDAmt"]               = 0;
                                    drEmpSystemAdj2Misc["Tsm_Misc4NDOTAmt"]             = 0;
                                    drEmpSystemAdj2Misc["Tsm_Misc5Amt"]                 = 0;
                                    drEmpSystemAdj2Misc["Tsm_Misc5OTAmt"]               = 0;
                                    drEmpSystemAdj2Misc["Tsm_Misc5NDAmt"]               = 0;
                                    drEmpSystemAdj2Misc["Tsm_Misc5NDOTAmt"]             = 0;
                                    drEmpSystemAdj2Misc["Tsm_Misc6Amt"]                 = 0;
                                    drEmpSystemAdj2Misc["Tsm_Misc6OTAmt"]               = 0;
                                    drEmpSystemAdj2Misc["Tsm_Misc6NDAmt"]               = 0;
                                    drEmpSystemAdj2Misc["Tsm_Misc6NDOTAmt"]             = 0;
                                    drEmpSystemAdj2Misc["Usr_Login"]                    = LoginUser;
                                    drEmpSystemAdj2Misc["Ludatetime"]                   = DateTime.Now;
                                    #endregion
                                }

                                #region Initiaize Group Premium Variables
                                //Regular and Overtime Rates
                                Reg         = 0;
                                RegOT       = 0;
                                Rest        = 0;
                                RestOT      = 0;
                                Hol         = 0;
                                HolOT       = 0;
                                SPL         = 0;
                                SPLOT       = 0;
                                PSD         = 0;
                                PSDOT       = 0;
                                Comp        = 0;
                                CompOT      = 0;
                                RestHol     = 0;
                                RestHolOT   = 0;
                                RestSPL     = 0;
                                RestSPLOT   = 0;
                                RestComp    = 0;
                                RestCompOT  = 0;
                                RestPSD     = 0;
                                RestPSDOT   = 0;

                                //Regular Night Premium and Overtime Night Premium Rates
                                RegND       = 0;
                                RegOTND     = 0;
                                RestND      = 0;
                                RestOTND    = 0;
                                HolND       = 0;
                                HolOTND     = 0;
                                SPLND       = 0;
                                SPLOTND     = 0;
                                PSDND       = 0;
                                PSDOTND     = 0;
                                CompND      = 0;
                                CompOTND    = 0;
                                RestHolND   = 0;
                                RestHolOTND = 0;
                                RestSPLND   = 0;
                                RestSPLOTND = 0;
                                RestCompND  = 0;
                                RestCompOTND = 0;
                                RestPSDND   = 0;
                                RestPSDOTND = 0;

                                //Regular Night Premium and Overtime Night Premium Percentages
                                RegNDPercent        = 0;
                                RegOTNDPercent      = 0;
                                RestNDPercent       = 0;
                                RestOTNDPercent     = 0;
                                HolNDPercent        = 0;
                                HolOTNDPercent      = 0;
                                SPLNDPercent        = 0;
                                SPLOTNDPercent      = 0;
                                PSDNDPercent        = 0;
                                PSDOTNDPercent      = 0;
                                CompNDPercent       = 0;
                                CompOTNDPercent     = 0;
                                RestHolNDPercent    = 0;
                                RestHolOTNDPercent  = 0;
                                RestSPLNDPercent    = 0;
                                RestSPLOTNDPercent  = 0;
                                RestCompNDPercent   = 0;
                                RestCompOTNDPercent = 0;
                                RestPSDNDPercent    = 0;
                                RestPSDOTNDPercent  = 0;

                                #endregion

                                #region Load Group Premiums
                                if (PayrollType != "" && dtSystemAdj2ForProcess.Rows[ids]["Tsa_PremiumGrpCode"].ToString() != "")
                                {
                                    GetPremium(dtDayCodePremiums, PayrollType, dtSystemAdj2ForProcess.Rows[ids]["Tsa_PremiumGrpCode"].ToString(), "REG", false, ref Reg, ref RegOT, ref RegND, ref RegOTND, ref RegNDPercent, ref RegOTNDPercent);
                                    GetPremium(dtDayCodePremiums, PayrollType, dtSystemAdj2ForProcess.Rows[ids]["Tsa_PremiumGrpCode"].ToString(), "REST", true, ref Rest, ref RestOT, ref RestND, ref RestOTND, ref RestNDPercent, ref RestOTNDPercent);
                                    GetPremium(dtDayCodePremiums, PayrollType, dtSystemAdj2ForProcess.Rows[ids]["Tsa_PremiumGrpCode"].ToString(), "SPL", false, ref SPL, ref SPLOT, ref SPLND, ref SPLOTND, ref SPLNDPercent, ref SPLOTNDPercent);
                                    GetPremium(dtDayCodePremiums, PayrollType, dtSystemAdj2ForProcess.Rows[ids]["Tsa_PremiumGrpCode"].ToString(), "HOL", false, ref Hol, ref HolOT, ref HolND, ref HolOTND, ref HolNDPercent, ref HolOTNDPercent);
                                    GetPremium(dtDayCodePremiums, PayrollType, dtSystemAdj2ForProcess.Rows[ids]["Tsa_PremiumGrpCode"].ToString(), "PSD", false, ref PSD, ref PSDOT, ref PSDND, ref PSDOTND, ref PSDNDPercent, ref PSDOTNDPercent);
                                    GetPremium(dtDayCodePremiums, PayrollType, dtSystemAdj2ForProcess.Rows[ids]["Tsa_PremiumGrpCode"].ToString(), "COMP", false, ref Comp, ref CompOT, ref CompND, ref CompOTND, ref CompNDPercent, ref CompOTNDPercent);
                                    GetPremium(dtDayCodePremiums, PayrollType, dtSystemAdj2ForProcess.Rows[ids]["Tsa_PremiumGrpCode"].ToString(), "SPL", true, ref RestSPL, ref RestSPLOT, ref RestSPLND, ref RestSPLOTND, ref RestSPLNDPercent, ref RestSPLOTNDPercent);
                                    GetPremium(dtDayCodePremiums, PayrollType, dtSystemAdj2ForProcess.Rows[ids]["Tsa_PremiumGrpCode"].ToString(), "HOL", true, ref RestHol, ref RestHolOT, ref RestHolND, ref RestHolOTND, ref RestHolNDPercent, ref RestHolOTNDPercent);
                                    GetPremium(dtDayCodePremiums, PayrollType, dtSystemAdj2ForProcess.Rows[ids]["Tsa_PremiumGrpCode"].ToString(), "PSD", true, ref RestPSD, ref RestPSDOT, ref RestPSDND, ref RestPSDOTND, ref RestPSDNDPercent, ref RestPSDOTNDPercent);
                                    GetPremium(dtDayCodePremiums, PayrollType, dtSystemAdj2ForProcess.Rows[ids]["Tsa_PremiumGrpCode"].ToString(), "COMP", true, ref RestComp, ref RestCompOT, ref RestCompND, ref RestCompOTND, ref RestCompNDPercent, ref RestCompOTNDPercent);
                                }
                                else
                                    throw new PayrollException("Cannot Load Day Premium Table");
                                #endregion

                                #region ND Bracket Rates (Sharp)
                                if (NDBRCKTCNT == 2)
                                {
                                    RegNDPercent        = NP1_RATE;
                                    RegOTNDPercent      = NP2_RATE;
                                    RestNDPercent       = NP1_RATE;
                                    RestOTNDPercent     = NP2_RATE;
                                    HolNDPercent        = NP1_RATE;
                                    HolOTNDPercent      = NP2_RATE;
                                    SPLNDPercent        = NP1_RATE;
                                    SPLOTNDPercent      = NP2_RATE;
                                    PSDNDPercent        = NP1_RATE;
                                    PSDOTNDPercent      = NP2_RATE;
                                    CompNDPercent       = NP1_RATE;
                                    CompOTNDPercent     = NP2_RATE;
                                    RestHolNDPercent    = NP1_RATE;
                                    RestHolOTNDPercent  = NP2_RATE;
                                    RestSPLNDPercent    = NP1_RATE;
                                    RestSPLOTNDPercent  = NP2_RATE;
                                    RestCompNDPercent   = NP1_RATE;
                                    RestCompOTNDPercent = NP2_RATE;
                                    RestPSDNDPercent    = NP1_RATE;
                                    RestPSDOTNDPercent  = NP2_RATE;
                                }

                                #endregion

                                #region Compute Hour Premiums

                                #region Absent Amount Computation
                                if (Convert.ToBoolean(PAYSPECIALRATE))
                                {
                                    drEmpSystemAdj2["Tsa_LTAmt"]                        = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_LTHr"]) * (LTRate == "S" ? SpecialHourlyRate : HourlyRate);
                                    drEmpSystemAdj2["Tsa_UTAmt"]                        = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_UTHr"]) * (UTRate == "S" ? SpecialHourlyRate : HourlyRate);
                                    drEmpSystemAdj2["Tsa_UPLVAmt"]                      = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_UPLVHr"]) * (UPLVRate == "S" ? SpecialHourlyRate : HourlyRate);
                                    drEmpSystemAdj2["Tsa_ABSLEGHOLAmt"]                 = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_ABSLEGHOLHr"]) * (ABSLEGHOLRate == "S" ? SpecialHourlyRate : HourlyRate);
                                    drEmpSystemAdj2["Tsa_ABSSPLHOLAmt"]                 = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_ABSSPLHOLHr"]) * (ABSSPLHOLRate == "S" ? SpecialHourlyRate : HourlyRate);
                                    drEmpSystemAdj2["Tsa_ABSCOMPHOLAmt"]                = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_ABSCOMPHOLHr"]) * (ABSCOMPHOLRate == "S" ? SpecialHourlyRate : HourlyRate);
                                    drEmpSystemAdj2["Tsa_ABSPSDAmt"]                    = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_ABSPSDHr"]) * (ABSPSDRate == "S" ? SpecialHourlyRate : HourlyRate);
                                    drEmpSystemAdj2["Tsa_ABSOTHHOLAmt"]                 = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_ABSOTHHOLHr"]) * (ABSOTHHOLRate == "S" ? SpecialHourlyRate : HourlyRate);
                                    drEmpSystemAdj2["Tsa_WDABSAmt"]                     = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_WDABSHr"]) * (WDABSRate == "S" ? SpecialHourlyRate : HourlyRate);
                                    drEmpSystemAdj2["Tsa_LTUTMaxAmt"]                   = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_LTUTMaxHr"]) * (LTUTMAXRate == "S" ? SpecialHourlyRate : HourlyRate);
                                    drEmpSystemAdj2["Tsa_ABSAmt"]                       = Convert.ToDecimal(drEmpSystemAdj2["Tsa_LTAmt"])
                                                                                            + Convert.ToDecimal(drEmpSystemAdj2["Tsa_UTAmt"])
                                                                                            + Convert.ToDecimal(drEmpSystemAdj2["Tsa_UPLVAmt"])
                                                                                            + Convert.ToDecimal(drEmpSystemAdj2["Tsa_WDABSAmt"])
                                                                                            + Convert.ToDecimal(drEmpSystemAdj2["Tsa_LTUTMaxAmt"]);
                                }
                                else
                                {
                                    drEmpSystemAdj2["Tsa_LTAmt"]                        = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_LTHr"]) * HourlyRate;
                                    drEmpSystemAdj2["Tsa_UTAmt"]                        = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_UTHr"]) * HourlyRate;
                                    drEmpSystemAdj2["Tsa_UPLVAmt"]                      = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_UPLVHr"]) * HourlyRate;
                                    drEmpSystemAdj2["Tsa_ABSLEGHOLAmt"]                 = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_ABSLEGHOLHr"]) * HourlyRate;
                                    drEmpSystemAdj2["Tsa_ABSSPLHOLAmt"]                 = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_ABSSPLHOLHr"]) * HourlyRate;
                                    drEmpSystemAdj2["Tsa_ABSCOMPHOLAmt"]                = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_ABSCOMPHOLHr"]) * HourlyRate;
                                    drEmpSystemAdj2["Tsa_ABSPSDAmt"]                    = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_ABSPSDHr"]) * HourlyRate;
                                    drEmpSystemAdj2["Tsa_ABSOTHHOLAmt"]                 = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_ABSOTHHOLHr"]) * HourlyRate;
                                    drEmpSystemAdj2["Tsa_WDABSAmt"]                     = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_WDABSHr"]) * HourlyRate;
                                    drEmpSystemAdj2["Tsa_LTUTMaxAmt"]                   = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_LTUTMaxHr"]) * HourlyRate;
                                    drEmpSystemAdj2["Tsa_ABSAmt"]                       = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_ABSHr"]) * HourlyRate;
                                }

                                #endregion

                                #region Regular Amount Computation

                                if (Convert.ToBoolean(PAYSPECIALRATE))
                                {
                                    drEmpSystemAdj2["Tsa_REGAmt"]                       = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_REGHr"]) * (REGRate == "S" ? SpecialHourlyRate : HourlyRate);
                                    drEmpSystemAdj2["Tsa_PDLVAmt"]                      = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_PDLVHr"]) * (PDLVRate == "S" ? SpecialHourlyRate : HourlyRate);
                                    drEmpSystemAdj2["Tsa_PDLEGHOLAmt"]                  = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_PDLEGHOLHr"]) * (PDLEGHOLRate == "S" ? SpecialHourlyRate : HourlyRate);
                                    drEmpSystemAdj2["Tsa_PDSPLHOLAmt"]                  = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_PDSPLHOLHr"]) * (PDSPLHOLRate == "S" ? SpecialHourlyRate : HourlyRate);
                                    drEmpSystemAdj2["Tsa_PDCOMPHOLAmt"]                 = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_PDCOMPHOLHr"]) * (PDCOMPHOLRate == "S" ? SpecialHourlyRate : HourlyRate);
                                    drEmpSystemAdj2["Tsa_PDPSDAmt"]                     = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_PDPSDHr"]) * (PDPSDRate == "S" ? SpecialHourlyRate : HourlyRate);
                                    drEmpSystemAdj2["Tsa_PDOTHHOLAmt"]                  = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_PDOTHHOLHr"]) * (PDOTHHOLRate == "S" ? SpecialHourlyRate : HourlyRate);
                                    drEmpSystemAdj2["Tsa_PDRESTLEGHOLAmt"]              = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_PDRESTLEGHOLHr"]) * (PDRESTLEGHOLRate == "S" ? SpecialHourlyRate : HourlyRate);
                                }
                                else
                                {
                                    drEmpSystemAdj2["Tsa_REGAmt"]                       = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_REGHr"]) * HourlyRate;
                                    drEmpSystemAdj2["Tsa_PDLVAmt"]                      = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_PDLVHr"]) * HourlyRate;
                                    drEmpSystemAdj2["Tsa_PDLEGHOLAmt"]                  = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_PDLEGHOLHr"]) * HourlyRate;
                                    drEmpSystemAdj2["Tsa_PDSPLHOLAmt"]                  = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_PDSPLHOLHr"]) * HourlyRate;
                                    drEmpSystemAdj2["Tsa_PDCOMPHOLAmt"]                 = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_PDCOMPHOLHr"]) * HourlyRate;
                                    drEmpSystemAdj2["Tsa_PDPSDAmt"]                     = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_PDPSDHr"]) * HourlyRate;
                                    drEmpSystemAdj2["Tsa_PDOTHHOLAmt"]                  = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_PDOTHHOLHr"]) * HourlyRate;
                                    drEmpSystemAdj2["Tsa_PDRESTLEGHOLAmt"]              = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_PDRESTLEGHOLHr"]) * HourlyRate;
                                }

                                if (PayrollType == "D")
                                {
                                    drEmpPayrollDtl["Tpd_TotalREGAmt"]                  = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_REGAmt"])
                                                                                            + Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDLVAmt"])
                                                                                            + Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDLEGHOLAmt"])
                                                                                            + Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDSPLHOLAmt"])
                                                                                            + Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDCOMPHOLAmt"])
                                                                                            + Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDOTHHOLAmt"])
                                                                                            + Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDRESTLEGHOLAmt"]), 2);
                                }

                                ////Cleanup for Regular Amount Considerations 
                                //// - Sum of Regular Hours is 0 (possibly on Leave for the whole quincena)
                                //// - Regular Amount computed above is 0 (more absences than the present days/half-month pay)
                                //if ((PayrollType == "M" && (Convert.ToDecimal(drEmpPayrollDtl["Tpd_REGHr"]) + Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDLVHr"])) == 0)
                                //    || Convert.ToDecimal(drEmpPayrollDtl["Tpd_TotalREGAmt"]) < 0)
                                //{
                                //    drEmpPayrollDtl["Tpd_TotalREGAmt"] = 0;
                                //    drEmpPayrollDtl["Tpd_TotalREGHr"] = 0;

                                //    drEmpPayrollDtl["Tpd_PDLEGHOLHr"] = 0;
                                //    drEmpPayrollDtl["Tpd_PDSPLHOLHr"] = 0;
                                //    drEmpPayrollDtl["Tpd_PDCOMPHOLHr"] = 0;
                                //    drEmpPayrollDtl["Tpd_PDPSDHr"] = 0;
                                //    drEmpPayrollDtl["Tpd_PDOTHHOLHr"] = 0;

                                //    drEmpPayrollDtl["Tpd_REGAmt"] = 0;
                                //    drEmpPayrollDtl["Tpd_PDLVAmt"] = 0;

                                //    drEmpPayrollDtl["Tpd_PDLEGHOLAmt"] = 0;
                                //    drEmpPayrollDtl["Tpd_PDSPLHOLAmt"] = 0;
                                //    drEmpPayrollDtl["Tpd_PDCOMPHOLAmt"] = 0;
                                //    drEmpPayrollDtl["Tpd_PDPSDAmt"] = 0;
                                //    drEmpPayrollDtl["Tpd_PDOTHHOLAmt"] = 0;

                                //    drEmpPayrollDtl["Tpd_PDRESTLEGHOLHr"] = 0;
                                //    drEmpPayrollDtl["Tpd_PDRESTLEGHOLAmt"] = 0;
                                //}

                                #endregion

                                #region Overtime Amount Computation
                                if (Convert.ToBoolean(PAYSPECIALRATE))
                                {
                                    drEmpSystemAdj2["Tsa_REGOTAmt"]                 = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_REGOTHr"]) * (REGRate == "S" ? SpecialHourlyRate : HourlyRate) * RegOT;

                                    drEmpSystemAdj2["Tsa_REGNDAmt"]                 = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_REGNDHr"]) * (REGRate == "S" ? SpecialHourlyRate : HourlyRate) * RegND * RegNDPercent;
                                    drEmpSystemAdj2["Tsa_REGNDOTAmt"]               = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_REGNDOTHr"]) * (REGRate == "S" ? SpecialHourlyRate : HourlyRate) * RegOTND * RegOTNDPercent;

                                    drEmpSystemAdj2["Tsa_RESTAmt"]                  = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_RESTHr"]) * (RESTRate == "S" ? SpecialHourlyRate : HourlyRate) * Rest;
                                    drEmpSystemAdj2["Tsa_RESTOTAmt"]                = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_RESTOTHr"]) * (RESTRate == "S" ? SpecialHourlyRate : HourlyRate) * RestOT;
                                    drEmpSystemAdj2["Tsa_RESTNDAmt"]                = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_RESTNDHr"]) * (RESTRate == "S" ? SpecialHourlyRate : HourlyRate) * RestND * RestNDPercent;
                                    drEmpSystemAdj2["Tsa_RESTNDOTAmt"]              = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_RESTNDOTHr"]) * (RESTRate == "S" ? SpecialHourlyRate : HourlyRate) * RestOTND * RestOTNDPercent;

                                    drEmpSystemAdj2["Tsa_LEGHOLAmt"]                = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_LEGHOLHr"]) * (LEGHOLRate == "S" ? SpecialHourlyRate : HourlyRate) * Hol;
                                    drEmpSystemAdj2["Tsa_LEGHOLOTAmt"]              = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_LEGHOLOTHr"]) * (LEGHOLRate == "S" ? SpecialHourlyRate : HourlyRate) * HolOT;
                                    drEmpSystemAdj2["Tsa_LEGHOLNDAmt"]              = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_LEGHOLNDHr"]) * (LEGHOLRate == "S" ? SpecialHourlyRate : HourlyRate) * HolND * HolNDPercent;
                                    drEmpSystemAdj2["Tsa_LEGHOLNDOTAmt"]            = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_LEGHOLNDOTHr"]) * (LEGHOLRate == "S" ? SpecialHourlyRate : HourlyRate) * HolOTND * HolOTNDPercent;

                                    drEmpSystemAdj2["Tsa_SPLHOLAmt"]                = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_SPLHOLHr"]) * (SPLHOLRate == "S" ? SpecialHourlyRate : HourlyRate) * SPL;
                                    drEmpSystemAdj2["Tsa_SPLHOLOTAmt"]              = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_SPLHOLOTHr"]) * (SPLHOLRate == "S" ? SpecialHourlyRate : HourlyRate) * SPLOT;
                                    drEmpSystemAdj2["Tsa_SPLHOLNDAmt"]              = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_SPLHOLNDHr"]) * (SPLHOLRate == "S" ? SpecialHourlyRate : HourlyRate) * SPLND * SPLNDPercent;
                                    drEmpSystemAdj2["Tsa_SPLHOLNDOTAmt"]            = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_SPLHOLNDOTHr"]) * (SPLHOLRate == "S" ? SpecialHourlyRate : HourlyRate) * SPLOTND * SPLOTNDPercent;

                                    drEmpSystemAdj2["Tsa_PSDAmt"]                   = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_PSDHr"]) * (PSDRate == "S" ? SpecialHourlyRate : HourlyRate) * PSD;
                                    drEmpSystemAdj2["Tsa_PSDOTAmt"]                 = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_PSDOTHr"]) * (PSDRate == "S" ? SpecialHourlyRate : HourlyRate) * PSDOT;
                                    drEmpSystemAdj2["Tsa_PSDNDAmt"]                 = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_PSDNDHr"]) * (PSDRate == "S" ? SpecialHourlyRate : HourlyRate) * PSDND * PSDNDPercent;
                                    drEmpSystemAdj2["Tsa_PSDNDOTAmt"]               = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_PSDNDOTHr"]) * (PSDRate == "S" ? SpecialHourlyRate : HourlyRate) * PSDOTND * PSDOTNDPercent;

                                    drEmpSystemAdj2["Tsa_COMPHOLAmt"]               = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_COMPHOLHr"]) * (COMPHOLRate == "S" ? SpecialHourlyRate : HourlyRate) * Comp;
                                    drEmpSystemAdj2["Tsa_COMPHOLOTAmt"]             = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_COMPHOLOTHr"]) * (COMPHOLRate == "S" ? SpecialHourlyRate : HourlyRate) * CompOT;
                                    drEmpSystemAdj2["Tsa_COMPHOLNDAmt"]             = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_COMPHOLNDHr"]) * (COMPHOLRate == "S" ? SpecialHourlyRate : HourlyRate) * CompND * CompNDPercent;
                                    drEmpSystemAdj2["Tsa_COMPHOLNDOTAmt"]           = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_COMPHOLNDOTHr"]) * (COMPHOLRate == "S" ? SpecialHourlyRate : HourlyRate) * CompOTND * CompOTNDPercent;

                                    drEmpSystemAdj2["Tsa_RESTLEGHOLAmt"]            = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_RESTLEGHOLHr"]) * (RESTLEGHOLRate == "S" ? SpecialHourlyRate : HourlyRate) * RestHol;
                                    drEmpSystemAdj2["Tsa_RESTLEGHOLOTAmt"]          = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_RESTLEGHOLOTHr"]) * (RESTLEGHOLRate == "S" ? SpecialHourlyRate : HourlyRate) * RestHolOT;
                                    drEmpSystemAdj2["Tsa_RESTLEGHOLNDAmt"]          = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_RESTLEGHOLNDHr"]) * (RESTLEGHOLRate == "S" ? SpecialHourlyRate : HourlyRate) * RestHolND * RestHolNDPercent;
                                    drEmpSystemAdj2["Tsa_RESTLEGHOLNDOTAmt"]        = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_RESTLEGHOLNDOTHr"]) * (RESTLEGHOLRate == "S" ? SpecialHourlyRate : HourlyRate) * RestHolOTND * RestHolOTNDPercent;

                                    drEmpSystemAdj2["Tsa_RESTSPLHOLAmt"]            = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_RESTSPLHOLHr"]) * (RESTSPLHOLRate == "S" ? SpecialHourlyRate : HourlyRate) * RestSPL;
                                    drEmpSystemAdj2["Tsa_RESTSPLHOLOTAmt"]          = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_RESTSPLHOLOTHr"]) * (RESTSPLHOLRate == "S" ? SpecialHourlyRate : HourlyRate) * RestSPLOT;
                                    drEmpSystemAdj2["Tsa_RESTSPLHOLNDAmt"]          = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_RESTSPLHOLNDHr"]) * (RESTSPLHOLRate == "S" ? SpecialHourlyRate : HourlyRate) * RestSPLND * RestSPLNDPercent;
                                    drEmpSystemAdj2["Tsa_RESTSPLHOLNDOTAmt"]        = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_RESTSPLHOLNDOTHr"]) * (RESTSPLHOLRate == "S" ? SpecialHourlyRate : HourlyRate) * RestSPLOTND * RestSPLOTNDPercent;

                                    drEmpSystemAdj2["Tsa_RESTCOMPHOLAmt"]           = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_RESTCOMPHOLHr"]) * (RESTCOMPHOLRate == "S" ? SpecialHourlyRate : HourlyRate) * RestComp;
                                    drEmpSystemAdj2["Tsa_RESTCOMPHOLOTAmt"]         = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_RESTCOMPHOLOTHr"]) * (RESTCOMPHOLRate == "S" ? SpecialHourlyRate : HourlyRate) * RestCompOT;
                                    drEmpSystemAdj2["Tsa_RESTCOMPHOLNDAmt"]         = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_RESTCOMPHOLNDHr"]) * (RESTCOMPHOLRate == "S" ? SpecialHourlyRate : HourlyRate) * RestCompND * RestCompNDPercent;
                                    drEmpSystemAdj2["Tsa_RESTCOMPHOLNDOTAmt"]       = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_RESTCOMPHOLNDOTHr"]) * (RESTCOMPHOLRate == "S" ? SpecialHourlyRate : HourlyRate) * RestCompOTND * RestCompOTNDPercent;

                                    drEmpSystemAdj2["Tsa_RESTPSDAmt"]               = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_RESTPSDHr"]) * (RESTPSDRate == "S" ? SpecialHourlyRate : HourlyRate) * RestPSD;
                                    drEmpSystemAdj2["Tsa_RESTPSDOTAmt"]             = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_RESTPSDOTHr"]) * (RESTPSDRate == "S" ? SpecialHourlyRate : HourlyRate) * RestPSDOT;
                                    drEmpSystemAdj2["Tsa_RESTPSDNDAmt"]             = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_RESTPSDNDHr"]) * (RESTPSDRate == "S" ? SpecialHourlyRate : HourlyRate) * RestPSDND * RestPSDNDPercent;
                                    drEmpSystemAdj2["Tsa_RESTPSDNDOTAmt"]           = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_RESTPSDNDOTHr"]) * (RESTPSDRate == "S" ? SpecialHourlyRate : HourlyRate) * RestPSDOTND * RestPSDOTNDPercent;
                                }
                                else
                                {
                                    drEmpSystemAdj2["Tsa_REGOTAmt"]                 = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_REGOTHr"]) * HourlyRate * RegOT;

                                    drEmpSystemAdj2["Tsa_REGNDAmt"]                 = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_REGNDHr"]) * HourlyRate * RegND * RegNDPercent;
                                    drEmpSystemAdj2["Tsa_REGNDOTAmt"]               = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_REGNDOTHr"]) * HourlyRate * RegOTND * RegOTNDPercent;

                                    drEmpSystemAdj2["Tsa_RESTAmt"]                  = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_RESTHr"]) * HourlyRate * Rest;
                                    drEmpSystemAdj2["Tsa_RESTOTAmt"]                = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_RESTOTHr"]) * HourlyRate * RestOT;
                                    drEmpSystemAdj2["Tsa_RESTNDAmt"]                = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_RESTNDHr"]) * HourlyRate * RestND * RestNDPercent;
                                    drEmpSystemAdj2["Tsa_RESTNDOTAmt"]              = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_RESTNDOTHr"]) * HourlyRate * RestOTND * RestOTNDPercent;

                                    drEmpSystemAdj2["Tsa_LEGHOLAmt"]                = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_LEGHOLHr"]) * HourlyRate * Hol;
                                    drEmpSystemAdj2["Tsa_LEGHOLOTAmt"]              = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_LEGHOLOTHr"]) * HourlyRate * HolOT;
                                    drEmpSystemAdj2["Tsa_LEGHOLNDAmt"]              = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_LEGHOLNDHr"]) * HourlyRate * HolND * HolNDPercent;
                                    drEmpSystemAdj2["Tsa_LEGHOLNDOTAmt"]            = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_LEGHOLNDOTHr"]) * HourlyRate * HolOTND * HolOTNDPercent;

                                    drEmpSystemAdj2["Tsa_SPLHOLAmt"]                = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_SPLHOLHr"]) * HourlyRate * SPL;
                                    drEmpSystemAdj2["Tsa_SPLHOLOTAmt"]              = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_SPLHOLOTHr"]) * HourlyRate * SPLOT;
                                    drEmpSystemAdj2["Tsa_SPLHOLNDAmt"]              = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_SPLHOLNDHr"]) * HourlyRate * SPLND * SPLNDPercent;
                                    drEmpSystemAdj2["Tsa_SPLHOLNDOTAmt"]            = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_SPLHOLNDOTHr"]) * HourlyRate * SPLOTND * SPLOTNDPercent;

                                    drEmpSystemAdj2["Tsa_PSDAmt"]                   = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_PSDHr"]) * HourlyRate * PSD;
                                    drEmpSystemAdj2["Tsa_PSDOTAmt"]                 = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_PSDOTHr"]) * HourlyRate * PSDOT;
                                    drEmpSystemAdj2["Tsa_PSDNDAmt"]                 = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_PSDNDHr"]) * HourlyRate * PSDND * PSDNDPercent;
                                    drEmpSystemAdj2["Tsa_PSDNDOTAmt"]               = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_PSDNDOTHr"]) * HourlyRate * PSDOTND * PSDOTNDPercent;

                                    drEmpSystemAdj2["Tsa_COMPHOLAmt"]               = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_COMPHOLHr"]) * HourlyRate * Comp;
                                    drEmpSystemAdj2["Tsa_COMPHOLOTAmt"]             = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_COMPHOLOTHr"]) * HourlyRate * CompOT;
                                    drEmpSystemAdj2["Tsa_COMPHOLNDAmt"]             = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_COMPHOLNDHr"]) * HourlyRate * CompND * CompNDPercent;
                                    drEmpSystemAdj2["Tsa_COMPHOLNDOTAmt"]           = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_COMPHOLNDOTHr"]) * HourlyRate * CompOTND * CompOTNDPercent;

                                    drEmpSystemAdj2["Tsa_RESTLEGHOLAmt"]            = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_RESTLEGHOLHr"]) * HourlyRate * RestHol;
                                    drEmpSystemAdj2["Tsa_RESTLEGHOLOTAmt"]          = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_RESTLEGHOLOTHr"]) * HourlyRate * RestHolOT;
                                    drEmpSystemAdj2["Tsa_RESTLEGHOLNDAmt"]          = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_RESTLEGHOLNDHr"]) * HourlyRate * RestHolND * RestHolNDPercent;
                                    drEmpSystemAdj2["Tsa_RESTLEGHOLNDOTAmt"]        = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_RESTLEGHOLNDOTHr"]) * HourlyRate * RestHolOTND * RestHolOTNDPercent;

                                    drEmpSystemAdj2["Tsa_RESTSPLHOLAmt"]            = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_RESTSPLHOLHr"]) * HourlyRate * RestSPL;
                                    drEmpSystemAdj2["Tsa_RESTSPLHOLOTAmt"]          = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_RESTSPLHOLOTHr"]) * HourlyRate * RestSPLOT;
                                    drEmpSystemAdj2["Tsa_RESTSPLHOLNDAmt"]          = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_RESTSPLHOLNDHr"]) * HourlyRate * RestSPLND * RestSPLNDPercent;
                                    drEmpSystemAdj2["Tsa_RESTSPLHOLNDOTAmt"]        = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_RESTSPLHOLNDOTHr"]) * HourlyRate * RestSPLOTND * RestSPLOTNDPercent;

                                    drEmpSystemAdj2["Tsa_RESTCOMPHOLAmt"]           = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_RESTCOMPHOLHr"]) * HourlyRate * RestComp;
                                    drEmpSystemAdj2["Tsa_RESTCOMPHOLOTAmt"]         = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_RESTCOMPHOLOTHr"]) * HourlyRate * RestCompOT;
                                    drEmpSystemAdj2["Tsa_RESTCOMPHOLNDAmt"]         = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_RESTCOMPHOLNDHr"]) * HourlyRate * RestCompND * RestCompNDPercent;
                                    drEmpSystemAdj2["Tsa_RESTCOMPHOLNDOTAmt"]       = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_RESTCOMPHOLNDOTHr"]) * HourlyRate * RestCompOTND * RestCompOTNDPercent;

                                    drEmpSystemAdj2["Tsa_RESTPSDAmt"]               = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_RESTPSDHr"]) * HourlyRate * RestPSD;
                                    drEmpSystemAdj2["Tsa_RESTPSDOTAmt"]             = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_RESTPSDOTHr"]) * HourlyRate * RestPSDOT;
                                    drEmpSystemAdj2["Tsa_RESTPSDNDAmt"]             = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_RESTPSDNDHr"]) * HourlyRate * RestPSDND * RestPSDNDPercent;
                                    drEmpSystemAdj2["Tsa_RESTPSDNDOTAmt"]           = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_RESTPSDNDOTHr"]) * HourlyRate * RestPSDOTND * RestPSDOTNDPercent;
                                }


                                #endregion

                                #region System Adj2 Misc
                                if (bHasDayCodeExt)
                                {
                                    drDayCodePremiumFiller = dtDayCodePremiumFillers.Select(string.Format("Mdp_PremiumGrpCode = '{0}' AND Mdp_PayrollType = '{1}'", dtSystemAdj2ForProcess.Rows[ids]["Tsa_PremiumGrpCode"].ToString(), PayrollType));
                                    for (int premFill = 0; premFill < drDayCodePremiumFiller.Length; premFill++)
                                    {
                                        fillerHrCol         = string.Format("Tsm_Misc{0:0}Hr", drDayCodePremiumFiller[premFill]["Mmd_MiscDayID"]);
                                        fillerOTHrCol       = string.Format("Tsm_Misc{0:0}OTHr", drDayCodePremiumFiller[premFill]["Mmd_MiscDayID"]);
                                        fillerNDHrCol       = string.Format("Tsm_Misc{0:0}NDHr", drDayCodePremiumFiller[premFill]["Mmd_MiscDayID"]);
                                        fillerOTNDHrCol     = string.Format("Tsm_Misc{0:0}NDOTHr", drDayCodePremiumFiller[premFill]["Mmd_MiscDayID"]);
                                        fillerAmtCol        = string.Format("Tsm_Misc{0:0}Amt", drDayCodePremiumFiller[premFill]["Mmd_MiscDayID"]);
                                        fillerOTAmtCol      = string.Format("Tsm_Misc{0:0}OTAmt", drDayCodePremiumFiller[premFill]["Mmd_MiscDayID"]);
                                        fillerNDAmtCol      = string.Format("Tsm_Misc{0:0}NDAmt", drDayCodePremiumFiller[premFill]["Mmd_MiscDayID"]);
                                        fillerOTNDAmtCol    = string.Format("Tsm_Misc{0:0}NDOTAmt", drDayCodePremiumFiller[premFill]["Mmd_MiscDayID"]);

                                        if (Convert.ToBoolean(PAYSPECIALRATE))
                                        {
                                            switch (GetValue(drDayCodePremiumFiller[premFill]["Mmd_MiscDayID"]))
                                            {
                                                case "1":
                                                    drEmpSystemAdj2Misc[fillerAmtCol] = Convert.ToDecimal(drEmpSystemAdj2Misc[fillerHrCol]) * (Misc1Rate == "S" ? SpecialHourlyRate : HourlyRate) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_RGPremiumRate"]) / 100);
                                                    drEmpSystemAdj2Misc[fillerOTAmtCol] = Convert.ToDecimal(drEmpSystemAdj2Misc[fillerOTHrCol]) * (Misc1Rate == "S" ? SpecialHourlyRate : HourlyRate) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_OTPremiumRate"]) / 100);
                                                    drEmpSystemAdj2Misc[fillerNDAmtCol] = Convert.ToDecimal(drEmpSystemAdj2Misc[fillerNDHrCol]) * (Misc1Rate == "S" ? SpecialHourlyRate : HourlyRate) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_RGNDPremiumRate"]) / 100) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_RGNDPercentageRate"]) / 100);
                                                    drEmpSystemAdj2Misc[fillerOTNDAmtCol] = Convert.ToDecimal(drEmpSystemAdj2Misc[fillerOTNDHrCol]) * (Misc1Rate == "S" ? SpecialHourlyRate : HourlyRate) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_OTNDPremiumRate"]) / 100) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_OTNDPercentageRate"]) / 100);
                                                    break;
                                                case "2":
                                                    drEmpSystemAdj2Misc[fillerAmtCol] = Convert.ToDecimal(drEmpSystemAdj2Misc[fillerHrCol]) * (Misc2Rate == "S" ? SpecialHourlyRate : HourlyRate) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_RGPremiumRate"]) / 100);
                                                    drEmpSystemAdj2Misc[fillerOTAmtCol] = Convert.ToDecimal(drEmpSystemAdj2Misc[fillerOTHrCol]) * (Misc2Rate == "S" ? SpecialHourlyRate : HourlyRate) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_OTPremiumRate"]) / 100);
                                                    drEmpSystemAdj2Misc[fillerNDAmtCol] = Convert.ToDecimal(drEmpSystemAdj2Misc[fillerNDHrCol]) * (Misc2Rate == "S" ? SpecialHourlyRate : HourlyRate) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_RGNDPremiumRate"]) / 100) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_RGNDPercentageRate"]) / 100);
                                                    drEmpSystemAdj2Misc[fillerOTNDAmtCol] = Convert.ToDecimal(drEmpSystemAdj2Misc[fillerOTNDHrCol]) * (Misc2Rate == "S" ? SpecialHourlyRate : HourlyRate) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_OTNDPremiumRate"]) / 100) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_OTNDPercentageRate"]) / 100);
                                                    break;
                                                case "3":
                                                    drEmpSystemAdj2Misc[fillerAmtCol] = Convert.ToDecimal(drEmpSystemAdj2Misc[fillerHrCol]) * (Misc3Rate == "S" ? SpecialHourlyRate : HourlyRate) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_RGPremiumRate"]) / 100);
                                                    drEmpSystemAdj2Misc[fillerOTAmtCol] = Convert.ToDecimal(drEmpSystemAdj2Misc[fillerOTHrCol]) * (Misc3Rate == "S" ? SpecialHourlyRate : HourlyRate) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_OTPremiumRate"]) / 100);
                                                    drEmpSystemAdj2Misc[fillerNDAmtCol] = Convert.ToDecimal(drEmpSystemAdj2Misc[fillerNDHrCol]) * (Misc3Rate == "S" ? SpecialHourlyRate : HourlyRate) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_RGNDPremiumRate"]) / 100) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_RGNDPercentageRate"]) / 100);
                                                    drEmpSystemAdj2Misc[fillerOTNDAmtCol] = Convert.ToDecimal(drEmpSystemAdj2Misc[fillerOTNDHrCol]) * (Misc3Rate == "S" ? SpecialHourlyRate : HourlyRate) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_OTNDPremiumRate"]) / 100) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_OTNDPercentageRate"]) / 100);
                                                    break;
                                                case "4":
                                                    drEmpSystemAdj2Misc[fillerAmtCol] = Convert.ToDecimal(drEmpSystemAdj2Misc[fillerHrCol]) * (Misc4Rate == "S" ? SpecialHourlyRate : HourlyRate) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_RGPremiumRate"]) / 100);
                                                    drEmpSystemAdj2Misc[fillerOTAmtCol] = Convert.ToDecimal(drEmpSystemAdj2Misc[fillerOTHrCol]) * (Misc4Rate == "S" ? SpecialHourlyRate : HourlyRate) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_OTPremiumRate"]) / 100);
                                                    drEmpSystemAdj2Misc[fillerNDAmtCol] = Convert.ToDecimal(drEmpSystemAdj2Misc[fillerNDHrCol]) * (Misc4Rate == "S" ? SpecialHourlyRate : HourlyRate) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_RGNDPremiumRate"]) / 100) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_RGNDPercentageRate"]) / 100);
                                                    drEmpSystemAdj2Misc[fillerOTNDAmtCol] = Convert.ToDecimal(drEmpSystemAdj2Misc[fillerOTNDHrCol]) * (Misc4Rate == "S" ? SpecialHourlyRate : HourlyRate) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_OTNDPremiumRate"]) / 100) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_OTNDPercentageRate"]) / 100);
                                                    break;
                                                case "5":
                                                    drEmpSystemAdj2Misc[fillerAmtCol] = Convert.ToDecimal(drEmpSystemAdj2Misc[fillerHrCol]) * (Misc5Rate == "S" ? SpecialHourlyRate : HourlyRate) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_RGPremiumRate"]) / 100);
                                                    drEmpSystemAdj2Misc[fillerOTAmtCol] = Convert.ToDecimal(drEmpSystemAdj2Misc[fillerOTHrCol]) * (Misc5Rate == "S" ? SpecialHourlyRate : HourlyRate) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_OTPremiumRate"]) / 100);
                                                    drEmpSystemAdj2Misc[fillerNDAmtCol] = Convert.ToDecimal(drEmpSystemAdj2Misc[fillerNDHrCol]) * (Misc5Rate == "S" ? SpecialHourlyRate : HourlyRate) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_RGNDPremiumRate"]) / 100) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_RGNDPercentageRate"]) / 100);
                                                    drEmpSystemAdj2Misc[fillerOTNDAmtCol] = Convert.ToDecimal(drEmpSystemAdj2Misc[fillerOTNDHrCol]) * (Misc5Rate == "S" ? SpecialHourlyRate : HourlyRate) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_OTNDPremiumRate"]) / 100) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_OTNDPercentageRate"]) / 100);
                                                    break;
                                                case "6":
                                                    drEmpSystemAdj2Misc[fillerAmtCol] = Convert.ToDecimal(drEmpSystemAdj2Misc[fillerHrCol]) * (Misc6Rate == "S" ? SpecialHourlyRate : HourlyRate) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_RGPremiumRate"]) / 100);
                                                    drEmpSystemAdj2Misc[fillerOTAmtCol] = Convert.ToDecimal(drEmpSystemAdj2Misc[fillerOTHrCol]) * (Misc6Rate == "S" ? SpecialHourlyRate : HourlyRate) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_OTPremiumRate"]) / 100);
                                                    drEmpSystemAdj2Misc[fillerNDAmtCol] = Convert.ToDecimal(drEmpSystemAdj2Misc[fillerNDHrCol]) * (Misc6Rate == "S" ? SpecialHourlyRate : HourlyRate) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_RGNDPremiumRate"]) / 100) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_RGNDPercentageRate"]) / 100);
                                                    drEmpSystemAdj2Misc[fillerOTNDAmtCol] = Convert.ToDecimal(drEmpSystemAdj2Misc[fillerOTNDHrCol]) * (Misc6Rate == "S" ? SpecialHourlyRate : HourlyRate) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_OTNDPremiumRate"]) / 100) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_OTNDPercentageRate"]) / 100);
                                                    break;
                                            }
                                        }
                                        else
                                        {
                                            drEmpSystemAdj2Misc[fillerAmtCol]       = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids][fillerHrCol]) * HourlyRate * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_RGPremiumRate"]) / 100);
                                            drEmpSystemAdj2Misc[fillerOTAmtCol]     = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids][fillerOTHrCol]) * HourlyRate * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_OTPremiumRate"]) / 100);
                                            drEmpSystemAdj2Misc[fillerNDAmtCol]     = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids][fillerNDHrCol]) * HourlyRate * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_RGNDPremiumRate"]) / 100) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_RGNDPercentageRate"]) / 100);
                                            drEmpSystemAdj2Misc[fillerOTNDAmtCol]   = Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids][fillerOTNDHrCol]) * HourlyRate * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_OTNDPremiumRate"]) / 100) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_OTNDPercentageRate"]) / 100);
                                        }
                                    }
                                }
                                #endregion

                                #region Compute System Adj2 Retro Totals
                                drEmpSystemAdj2["Tsa_RGAdjAmt"]                     = Convert.ToDecimal(drEmpSystemAdj2["Tsa_REGAmt"]) - Convert.ToDecimal(drEmpSystemAdj2["Tsa_ABSAmt"]);
                                drEmpSystemAdj2["Tsa_OTAdjAmt"]                     = (Convert.ToDecimal(drEmpSystemAdj2["Tsa_REGOTAmt"])
                                                                                            + Convert.ToDecimal(drEmpSystemAdj2["Tsa_RESTAmt"]) + Convert.ToDecimal(drEmpSystemAdj2["Tsa_RESTOTAmt"])
                                                                                            + Convert.ToDecimal(drEmpSystemAdj2["Tsa_LEGHOLAmt"]) + Convert.ToDecimal(drEmpSystemAdj2["Tsa_LEGHOLOTAmt"])
                                                                                            + Convert.ToDecimal(drEmpSystemAdj2["Tsa_SPLHOLAmt"]) + Convert.ToDecimal(drEmpSystemAdj2["Tsa_SPLHOLOTAmt"])
                                                                                            + Convert.ToDecimal(drEmpSystemAdj2["Tsa_PSDAmt"]) + Convert.ToDecimal(drEmpSystemAdj2["Tsa_PSDOTAmt"])
                                                                                            + Convert.ToDecimal(drEmpSystemAdj2["Tsa_COMPHOLAmt"]) + Convert.ToDecimal(drEmpSystemAdj2["Tsa_COMPHOLOTAmt"])
                                                                                            + Convert.ToDecimal(drEmpSystemAdj2["Tsa_RESTLEGHOLAmt"]) + Convert.ToDecimal(drEmpSystemAdj2["Tsa_RESTLEGHOLOTAmt"])
                                                                                            + Convert.ToDecimal(drEmpSystemAdj2["Tsa_RESTSPLHOLAmt"]) + Convert.ToDecimal(drEmpSystemAdj2["Tsa_RESTSPLHOLOTAmt"])
                                                                                            + Convert.ToDecimal(drEmpSystemAdj2["Tsa_RESTCOMPHOLAmt"]) + Convert.ToDecimal(drEmpSystemAdj2["Tsa_RESTCOMPHOLOTAmt"])
                                                                                            + Convert.ToDecimal(drEmpSystemAdj2["Tsa_RESTPSDAmt"]) + Convert.ToDecimal(drEmpSystemAdj2["Tsa_RESTPSDOTAmt"]));
                                drEmpSystemAdj2["Tsa_HOLAdjAmt"]                    = (Convert.ToDecimal(drEmpSystemAdj2["Tsa_PDLEGHOLAmt"])
                                                                                            + Convert.ToDecimal(drEmpSystemAdj2["Tsa_PDSPLHOLAmt"])
                                                                                            + Convert.ToDecimal(drEmpSystemAdj2["Tsa_PDCOMPHOLAmt"])
                                                                                            + Convert.ToDecimal(drEmpSystemAdj2["Tsa_PDPSDAmt"])
                                                                                            + Convert.ToDecimal(drEmpSystemAdj2["Tsa_PDOTHHOLAmt"]))
                                                                                    - (Convert.ToDecimal(drEmpSystemAdj2["Tsa_ABSLEGHOLAmt"])
                                                                                            + Convert.ToDecimal(drEmpSystemAdj2["Tsa_ABSSPLHOLAmt"])
                                                                                            + Convert.ToDecimal(drEmpSystemAdj2["Tsa_ABSCOMPHOLAmt"])
                                                                                            + Convert.ToDecimal(drEmpSystemAdj2["Tsa_ABSPSDAmt"])
                                                                                            + Convert.ToDecimal(drEmpSystemAdj2["Tsa_ABSOTHHOLAmt"]));
                                drEmpSystemAdj2["Tsa_NDAdjAmt"]                     = (Convert.ToDecimal(drEmpSystemAdj2["Tsa_REGNDAmt"]) + Convert.ToDecimal(drEmpSystemAdj2["Tsa_REGNDOTAmt"])
                                                                                            + Convert.ToDecimal(drEmpSystemAdj2["Tsa_RESTNDAmt"]) + Convert.ToDecimal(drEmpSystemAdj2["Tsa_RESTNDOTAmt"])
                                                                                            + Convert.ToDecimal(drEmpSystemAdj2["Tsa_LEGHOLNDAmt"]) + Convert.ToDecimal(drEmpSystemAdj2["Tsa_LEGHOLNDOTAmt"])
                                                                                            + Convert.ToDecimal(drEmpSystemAdj2["Tsa_SPLHOLNDAmt"]) + Convert.ToDecimal(drEmpSystemAdj2["Tsa_SPLHOLNDOTAmt"])
                                                                                            + Convert.ToDecimal(drEmpSystemAdj2["Tsa_PSDNDAmt"]) + Convert.ToDecimal(drEmpSystemAdj2["Tsa_PSDNDOTAmt"])
                                                                                            + Convert.ToDecimal(drEmpSystemAdj2["Tsa_COMPHOLNDAmt"]) + Convert.ToDecimal(drEmpSystemAdj2["Tsa_COMPHOLNDOTAmt"])
                                                                                            + Convert.ToDecimal(drEmpSystemAdj2["Tsa_RESTLEGHOLNDAmt"]) + Convert.ToDecimal(drEmpSystemAdj2["Tsa_RESTLEGHOLNDOTAmt"])
                                                                                            + Convert.ToDecimal(drEmpSystemAdj2["Tsa_RESTSPLHOLNDAmt"]) + Convert.ToDecimal(drEmpSystemAdj2["Tsa_RESTSPLHOLNDOTAmt"])
                                                                                            + Convert.ToDecimal(drEmpSystemAdj2["Tsa_RESTCOMPHOLNDAmt"]) + Convert.ToDecimal(drEmpSystemAdj2["Tsa_RESTCOMPHOLNDOTAmt"])
                                                                                            + Convert.ToDecimal(drEmpSystemAdj2["Tsa_RESTPSDNDAmt"]) + Convert.ToDecimal(drEmpSystemAdj2["Tsa_RESTPSDNDOTAmt"]));
                                drEmpSystemAdj2["Tsa_LVAdjAmt"]                     = Convert.ToDecimal(drEmpSystemAdj2["Tsa_PDLVAmt"]);
                                
                                if (bHasDayCodeExt)
                                {

                                    drEmpSystemAdj2["Tsa_OTAdjAmt"] = (Convert.ToDecimal(drEmpSystemAdj2["Tsa_OTAdjAmt"]) 
                                                                                            + Convert.ToDecimal(drEmpSystemAdj2Misc["Tsm_Misc1Amt"]) + Convert.ToDecimal(drEmpSystemAdj2Misc["Tsm_Misc1OTAmt"])
                                                                                            + Convert.ToDecimal(drEmpSystemAdj2Misc["Tsm_Misc2Amt"]) + Convert.ToDecimal(drEmpSystemAdj2Misc["Tsm_Misc2OTAmt"])
                                                                                            + Convert.ToDecimal(drEmpSystemAdj2Misc["Tsm_Misc3Amt"]) + Convert.ToDecimal(drEmpSystemAdj2Misc["Tsm_Misc3OTAmt"])
                                                                                            + Convert.ToDecimal(drEmpSystemAdj2Misc["Tsm_Misc4Amt"]) + Convert.ToDecimal(drEmpSystemAdj2Misc["Tsm_Misc4OTAmt"])
                                                                                            + Convert.ToDecimal(drEmpSystemAdj2Misc["Tsm_Misc5Amt"]) + Convert.ToDecimal(drEmpSystemAdj2Misc["Tsm_Misc5OTAmt"])
                                                                                            + Convert.ToDecimal(drEmpSystemAdj2Misc["Tsm_Misc6Amt"]) + Convert.ToDecimal(drEmpSystemAdj2Misc["Tsm_Misc6OTAmt"]));

                                    drEmpSystemAdj2["Tsa_NDAdjAmt"] = (Convert.ToDecimal(drEmpSystemAdj2["Tsa_NDAdjAmt"]) +
                                                                                            +Convert.ToDecimal(drEmpSystemAdj2Misc["Tsm_Misc1NDAmt"]) + Convert.ToDecimal(drEmpSystemAdj2Misc["Tsm_Misc1NDOTAmt"])
                                                                                            + Convert.ToDecimal(drEmpSystemAdj2Misc["Tsm_Misc2NDAmt"]) + Convert.ToDecimal(drEmpSystemAdj2Misc["Tsm_Misc2NDOTAmt"])
                                                                                            + Convert.ToDecimal(drEmpSystemAdj2Misc["Tsm_Misc3NDAmt"]) + Convert.ToDecimal(drEmpSystemAdj2Misc["Tsm_Misc3NDOTAmt"])
                                                                                            + Convert.ToDecimal(drEmpSystemAdj2Misc["Tsm_Misc4NDAmt"]) + Convert.ToDecimal(drEmpSystemAdj2Misc["Tsm_Misc4NDOTAmt"])
                                                                                            + Convert.ToDecimal(drEmpSystemAdj2Misc["Tsm_Misc5NDAmt"]) + Convert.ToDecimal(drEmpSystemAdj2Misc["Tsm_Misc5NDOTAmt"])
                                                                                            + Convert.ToDecimal(drEmpSystemAdj2Misc["Tsm_Misc6NDAmt"]) + Convert.ToDecimal(drEmpSystemAdj2Misc["Tsm_Misc6NDOTAmt"]));
                                }

                                drEmpSystemAdj2["Tsa_TotalAdjAmt"] = Convert.ToDecimal(drEmpSystemAdj2["Tsa_RGAdjAmt"])
                                                                                            + Convert.ToDecimal(drEmpSystemAdj2["Tsa_OTAdjAmt"])
                                                                                            + Convert.ToDecimal(drEmpSystemAdj2["Tsa_HOLAdjAmt"])
                                                                                            + Convert.ToDecimal(drEmpSystemAdj2["Tsa_NDAdjAmt"])
                                                                                            + Convert.ToDecimal(drEmpSystemAdj2["Tsa_LVAdjAmt"]);
                                #endregion

                                #region Compute Difference
                                drEmpSystemAdj2["Tsa_RGRetroAmt"]                   = Convert.ToDecimal(drEmpSystemAdj2["Tsa_RGAdjAmt"]) - Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_RGAdjAmt"]);
                                drEmpSystemAdj2["Tsa_OTRetroAmt"]                   = Convert.ToDecimal(drEmpSystemAdj2["Tsa_OTAdjAmt"]) - Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_OTAdjAmt"]);
                                drEmpSystemAdj2["Tsa_NDRetroAmt"]                   = Convert.ToDecimal(drEmpSystemAdj2["Tsa_HOLAdjAmt"]) - Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_HOLAdjAmt"]);
                                drEmpSystemAdj2["Tsa_HLRetroAmt"]                   = Convert.ToDecimal(drEmpSystemAdj2["Tsa_NDAdjAmt"]) - Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_NDAdjAmt"]);
                                drEmpSystemAdj2["Tsa_LVRetroAmt"]                   = Convert.ToDecimal(drEmpSystemAdj2["Tsa_LVAdjAmt"]) - Convert.ToDecimal(dtSystemAdj2ForProcess.Rows[ids]["Tsa_LVAdjAmt"]);
                                #endregion

                                #region Rounding Off of Amounts
                                drEmpSystemAdj2["Tsa_LTAmt"]                        = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_LTAmt"]), 2);
                                drEmpSystemAdj2["Tsa_LTAmt"]                        = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_LTAmt"]), 2);
                                drEmpSystemAdj2["Tsa_UTAmt"]                        = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_UTAmt"]), 2);
                                drEmpSystemAdj2["Tsa_UPLVAmt"]                      = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_UPLVAmt"]), 2);
                                drEmpSystemAdj2["Tsa_ABSLEGHOLAmt"]                 = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_ABSLEGHOLAmt"]), 2);
                                drEmpSystemAdj2["Tsa_ABSSPLHOLAmt"]                 = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_ABSSPLHOLAmt"]), 2);
                                drEmpSystemAdj2["Tsa_ABSCOMPHOLAmt"]                = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_ABSCOMPHOLAmt"]), 2);
                                drEmpSystemAdj2["Tsa_ABSPSDAmt"]                    = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_ABSPSDAmt"]), 2);
                                drEmpSystemAdj2["Tsa_ABSOTHHOLAmt"]                 = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_ABSOTHHOLAmt"]), 2);
                                drEmpSystemAdj2["Tsa_WDABSAmt"]                     = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_WDABSAmt"]), 2);
                                drEmpSystemAdj2["Tsa_LTUTMaxAmt"]                   = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_LTUTMaxAmt"]), 2);
                                drEmpSystemAdj2["Tsa_ABSAmt"]                       = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_ABSAmt"]), 2);
                                drEmpSystemAdj2["Tsa_REGAmt"]                       = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_REGAmt"]), 2);
                                drEmpSystemAdj2["Tsa_PDLVAmt"]                      = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_PDLVAmt"]), 2);
                                drEmpSystemAdj2["Tsa_PDLEGHOLAmt"]                  = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_PDLEGHOLAmt"]), 2);
                                drEmpSystemAdj2["Tsa_PDSPLHOLAmt"]                  = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_PDSPLHOLAmt"]), 2);
                                drEmpSystemAdj2["Tsa_PDCOMPHOLAmt"]                 = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_PDCOMPHOLAmt"]), 2);
                                drEmpSystemAdj2["Tsa_PDPSDAmt"]                     = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_PDPSDAmt"]), 2);
                                drEmpSystemAdj2["Tsa_PDOTHHOLAmt"]                  = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_PDOTHHOLAmt"]), 2);
                                drEmpSystemAdj2["Tsa_PDRESTLEGHOLAmt"]              = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_PDRESTLEGHOLAmt"]), 2);
                                drEmpSystemAdj2["Tsa_REGOTAmt"]                     = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_REGOTAmt"]), 2);
                                drEmpSystemAdj2["Tsa_REGNDAmt"]                     = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_REGNDAmt"]), 2);
                                drEmpSystemAdj2["Tsa_REGNDOTAmt"]                   = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_REGNDOTAmt"]), 2);
                                drEmpSystemAdj2["Tsa_RESTAmt"]                      = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_RESTAmt"]), 2);
                                drEmpSystemAdj2["Tsa_RESTOTAmt"]                    = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_RESTOTAmt"]), 2);
                                drEmpSystemAdj2["Tsa_RESTNDAmt"]                    = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_RESTNDAmt"]), 2);
                                drEmpSystemAdj2["Tsa_RESTNDOTAmt"]                  = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_RESTNDOTAmt"]), 2);
                                drEmpSystemAdj2["Tsa_LEGHOLAmt"]                    = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_LEGHOLAmt"]), 2);
                                drEmpSystemAdj2["Tsa_LEGHOLOTAmt"]                  = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_LEGHOLOTAmt"]), 2);
                                drEmpSystemAdj2["Tsa_LEGHOLNDAmt"]                  = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_LEGHOLNDAmt"]), 2);
                                drEmpSystemAdj2["Tsa_LEGHOLNDOTAmt"]                = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_LEGHOLNDOTAmt"]), 2);
                                drEmpSystemAdj2["Tsa_SPLHOLAmt"]                    = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_SPLHOLAmt"]), 2);
                                drEmpSystemAdj2["Tsa_SPLHOLOTAmt"]                  = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_SPLHOLOTAmt"]), 2);
                                drEmpSystemAdj2["Tsa_SPLHOLNDAmt"]                  = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_SPLHOLNDAmt"]), 2);
                                drEmpSystemAdj2["Tsa_SPLHOLNDOTAmt"]                = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_SPLHOLNDOTAmt"]), 2);
                                drEmpSystemAdj2["Tsa_PSDAmt"]                       = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_PSDAmt"]), 2);
                                drEmpSystemAdj2["Tsa_PSDOTAmt"]                     = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_PSDOTAmt"]), 2);
                                drEmpSystemAdj2["Tsa_PSDNDAmt"]                     = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_PSDNDAmt"]), 2);
                                drEmpSystemAdj2["Tsa_PSDNDOTAmt"]                   = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_PSDNDOTAmt"]), 2);
                                drEmpSystemAdj2["Tsa_COMPHOLAmt"]                   = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_COMPHOLAmt"]), 2);
                                drEmpSystemAdj2["Tsa_COMPHOLOTAmt"]                 = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_COMPHOLOTAmt"]), 2);
                                drEmpSystemAdj2["Tsa_COMPHOLNDAmt"]                 = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_COMPHOLNDAmt"]), 2);
                                drEmpSystemAdj2["Tsa_COMPHOLNDOTAmt"]               = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_COMPHOLNDOTAmt"]), 2);
                                drEmpSystemAdj2["Tsa_RESTLEGHOLAmt"]                = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_RESTLEGHOLAmt"]), 2);
                                drEmpSystemAdj2["Tsa_RESTLEGHOLOTAmt"]              = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_RESTLEGHOLOTAmt"]), 2);
                                drEmpSystemAdj2["Tsa_RESTLEGHOLNDAmt"]              = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_RESTLEGHOLNDAmt"]), 2);
                                drEmpSystemAdj2["Tsa_RESTLEGHOLNDOTAmt"]            = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_RESTLEGHOLNDOTAmt"]), 2);
                                drEmpSystemAdj2["Tsa_RESTSPLHOLAmt"]                = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_RESTSPLHOLAmt"]), 2);
                                drEmpSystemAdj2["Tsa_RESTSPLHOLOTAmt"]              = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_RESTSPLHOLOTAmt"]), 2);
                                drEmpSystemAdj2["Tsa_RESTSPLHOLNDAmt"]              = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_RESTSPLHOLNDAmt"]), 2);
                                drEmpSystemAdj2["Tsa_RESTSPLHOLNDOTAmt"]            = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_RESTSPLHOLNDOTAmt"]), 2);
                                drEmpSystemAdj2["Tsa_RESTCOMPHOLAmt"]               = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_RESTCOMPHOLAmt"]), 2);
                                drEmpSystemAdj2["Tsa_RESTCOMPHOLOTAmt"]             = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_RESTCOMPHOLOTAmt"]), 2);
                                drEmpSystemAdj2["Tsa_RESTCOMPHOLNDAmt"]             = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_RESTCOMPHOLNDAmt"]), 2);
                                drEmpSystemAdj2["Tsa_RESTCOMPHOLNDOTAmt"]           = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_RESTCOMPHOLNDOTAmt"]), 2);
                                drEmpSystemAdj2["Tsa_RESTPSDAmt"]                   = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_RESTPSDAmt"]), 2);
                                drEmpSystemAdj2["Tsa_RESTPSDOTAmt"]                 = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_RESTPSDOTAmt"]), 2);
                                drEmpSystemAdj2["Tsa_RESTPSDNDAmt"]                 = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_RESTPSDNDAmt"]), 2);
                                drEmpSystemAdj2["Tsa_RESTPSDNDOTAmt"]               = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_RESTPSDNDOTAmt"]), 2);
                                drEmpSystemAdj2["Tsa_RGAdjAmt"]                     = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_RGAdjAmt"]), 2);
                                drEmpSystemAdj2["Tsa_OTAdjAmt"]                     = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_OTAdjAmt"]), 2);
                                drEmpSystemAdj2["Tsa_HOLAdjAmt"]                    = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_HOLAdjAmt"]), 2);
                                drEmpSystemAdj2["Tsa_NDAdjAmt"]                     = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_NDAdjAmt"]), 2);
                                drEmpSystemAdj2["Tsa_LVAdjAmt"]                     = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_LVAdjAmt"]), 2);
                                drEmpSystemAdj2["Tsa_TotalAdjAmt"]                  = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_TotalAdjAmt"]), 2);
                                drEmpSystemAdj2["Tsa_RGRetroAmt"]                   = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_RGRetroAmt"]), 2);
                                drEmpSystemAdj2["Tsa_OTRetroAmt"]                   = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_OTRetroAmt"]), 2);
                                drEmpSystemAdj2["Tsa_NDRetroAmt"]                   = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_NDRetroAmt"]), 2);
                                drEmpSystemAdj2["Tsa_HLRetroAmt"]                   = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_HLRetroAmt"]), 2);
                                drEmpSystemAdj2["Tsa_LVRetroAmt"]                   = Math.Round(Convert.ToDecimal(drEmpSystemAdj2["Tsa_LVRetroAmt"]), 2);

                                if (bHasDayCodeExt)
                                {
                                    drEmpSystemAdj2Misc["Tsm_Misc1Amt"]             = Math.Round(Convert.ToDecimal(drEmpSystemAdj2Misc["Tsm_Misc1Amt"]), 2);
                                    drEmpSystemAdj2Misc["Tsm_Misc1OTAmt"]           = Math.Round(Convert.ToDecimal(drEmpSystemAdj2Misc["Tsm_Misc1OTAmt"]), 2);
                                    drEmpSystemAdj2Misc["Tsm_Misc1NDAmt"]           = Math.Round(Convert.ToDecimal(drEmpSystemAdj2Misc["Tsm_Misc1NDAmt"]), 2);
                                    drEmpSystemAdj2Misc["Tsm_Misc1NDOTAmt"]         = Math.Round(Convert.ToDecimal(drEmpSystemAdj2Misc["Tsm_Misc1NDOTAmt"]), 2);
                                    drEmpSystemAdj2Misc["Tsm_Misc2Amt"]             = Math.Round(Convert.ToDecimal(drEmpSystemAdj2Misc["Tsm_Misc2Amt"]), 2);
                                    drEmpSystemAdj2Misc["Tsm_Misc2OTAmt"]           = Math.Round(Convert.ToDecimal(drEmpSystemAdj2Misc["Tsm_Misc2OTAmt"]), 2);
                                    drEmpSystemAdj2Misc["Tsm_Misc2NDAmt"]           = Math.Round(Convert.ToDecimal(drEmpSystemAdj2Misc["Tsm_Misc2NDAmt"]), 2);
                                    drEmpSystemAdj2Misc["Tsm_Misc2NDOTAmt"]         = Math.Round(Convert.ToDecimal(drEmpSystemAdj2Misc["Tsm_Misc2NDOTAmt"]), 2);
                                    drEmpSystemAdj2Misc["Tsm_Misc3Amt"]             = Math.Round(Convert.ToDecimal(drEmpSystemAdj2Misc["Tsm_Misc3Amt"]), 2);
                                    drEmpSystemAdj2Misc["Tsm_Misc3OTAmt"]           = Math.Round(Convert.ToDecimal(drEmpSystemAdj2Misc["Tsm_Misc3OTAmt"]), 2);
                                    drEmpSystemAdj2Misc["Tsm_Misc3NDAmt"]           = Math.Round(Convert.ToDecimal(drEmpSystemAdj2Misc["Tsm_Misc3NDAmt"]), 2);
                                    drEmpSystemAdj2Misc["Tsm_Misc3NDOTAmt"]         = Math.Round(Convert.ToDecimal(drEmpSystemAdj2Misc["Tsm_Misc3NDOTAmt"]), 2);
                                    drEmpSystemAdj2Misc["Tsm_Misc4Amt"]             = Math.Round(Convert.ToDecimal(drEmpSystemAdj2Misc["Tsm_Misc4Amt"]), 2);
                                    drEmpSystemAdj2Misc["Tsm_Misc4OTAmt"]           = Math.Round(Convert.ToDecimal(drEmpSystemAdj2Misc["Tsm_Misc4OTAmt"]), 2);
                                    drEmpSystemAdj2Misc["Tsm_Misc4NDAmt"]           = Math.Round(Convert.ToDecimal(drEmpSystemAdj2Misc["Tsm_Misc4NDAmt"]), 2);
                                    drEmpSystemAdj2Misc["Tsm_Misc4NDOTAmt"]         = Math.Round(Convert.ToDecimal(drEmpSystemAdj2Misc["Tsm_Misc4NDOTAmt"]), 2);
                                    drEmpSystemAdj2Misc["Tsm_Misc5Amt"]             = Math.Round(Convert.ToDecimal(drEmpSystemAdj2Misc["Tsm_Misc5Amt"]), 2);
                                    drEmpSystemAdj2Misc["Tsm_Misc5OTAmt"]           = Math.Round(Convert.ToDecimal(drEmpSystemAdj2Misc["Tsm_Misc5OTAmt"]), 2);
                                    drEmpSystemAdj2Misc["Tsm_Misc5NDAmt"]           = Math.Round(Convert.ToDecimal(drEmpSystemAdj2Misc["Tsm_Misc5NDAmt"]), 2);
                                    drEmpSystemAdj2Misc["Tsm_Misc5NDOTAmt"]         = Math.Round(Convert.ToDecimal(drEmpSystemAdj2Misc["Tsm_Misc5NDOTAmt"]), 2);
                                    drEmpSystemAdj2Misc["Tsm_Misc6Amt"]             = Math.Round(Convert.ToDecimal(drEmpSystemAdj2Misc["Tsm_Misc6Amt"]), 2);
                                    drEmpSystemAdj2Misc["Tsm_Misc6OTAmt"]           = Math.Round(Convert.ToDecimal(drEmpSystemAdj2Misc["Tsm_Misc6OTAmt"]), 2);
                                    drEmpSystemAdj2Misc["Tsm_Misc6NDAmt"]           = Math.Round(Convert.ToDecimal(drEmpSystemAdj2Misc["Tsm_Misc6NDAmt"]), 2);
                                    drEmpSystemAdj2Misc["Tsm_Misc6NDOTAmt"]         = Math.Round(Convert.ToDecimal(drEmpSystemAdj2Misc["Tsm_Misc6NDOTAmt"]), 2);
                                }

                                #endregion

                                #region 

                                #region Grand Total Retro Amount Computation
                                drEmpSystemAdj2["Tsa_TotalRetroAmt"] = Convert.ToDecimal(drEmpSystemAdj2["Tsa_RGRetroAmt"])
                                                                            + Convert.ToDecimal(drEmpSystemAdj2["Tsa_OTRetroAmt"])
                                                                            + Convert.ToDecimal(drEmpSystemAdj2["Tsa_NDRetroAmt"])
                                                                            + Convert.ToDecimal(drEmpSystemAdj2["Tsa_HLRetroAmt"])
                                                                            + Convert.ToDecimal(drEmpSystemAdj2["Tsa_LVRetroAmt"]);
                                #endregion

                                #region Add to Retropay Total Amount
                                EmpRGRetroAmt += Convert.ToDecimal(drEmpSystemAdj2["Tsa_RGRetroAmt"]);
                                EmpOTRetroAmt       += Convert.ToDecimal(drEmpSystemAdj2["Tsa_OTRetroAmt"]);
                                EmpNDRetroAmt       += Convert.ToDecimal(drEmpSystemAdj2["Tsa_NDRetroAmt"]);
                                EmpHLRetroAmt       += Convert.ToDecimal(drEmpSystemAdj2["Tsa_HLRetroAmt"]);
                                EmpLVRetroAmt       += Convert.ToDecimal(drEmpSystemAdj2["Tsa_LVRetroAmt"]);
                                EmpTotalRetroAmt    += Convert.ToDecimal(drEmpSystemAdj2["Tsa_TotalRetroAmt"]);
                                #endregion

                                #endregion

                                #endregion

                                //Add to System Adj2 tables
                                dtEmpSystemAdj2.Rows.Add(drEmpSystemAdj2);
                                if (bHasDayCodeExt)
                                    dtEmpSystemAdj2Misc.Rows.Add(drEmpSystemAdj2Misc);
                            }
                            #endregion

                            #region T_EmpRetroPay Update
                            string strQuery = string.Format(@"UPDATE T_EmpRetroPay 
                                                              SET Ter_RGRetroAmt = {2}
                                                                    , Ter_OTRetroAmt = {3}
                                                                    , Ter_NDRetroAmt = {4}
                                                                    , Ter_HLRetroAmt = {5}
                                                                    , Ter_LVRetroAmt = {6}
                                                                    , Ter_TotalRetroAmt = {7}
                                                                    , Usr_Login = '{8}'
                                                                    , Ludatetime = GETDATE()
                                                              WHERE Ter_IDNo = '{0}' 
                                                                    AND Ter_PayCycle = '{1}'"
                                                                , curEmployeeId
                                                                , PayrollPeriod
                                                                , EmpRGRetroAmt
                                                                , EmpOTRetroAmt
                                                                , EmpNDRetroAmt
                                                                , EmpHLRetroAmt
                                                                , EmpLVRetroAmt
                                                                , EmpTotalRetroAmt
                                                                , LoginUser
                                                                );
                            #endregion

                            dal.ExecuteNonQuery(strQuery);
                            EmpDispHandler(this, new EmpDispEventArgs(curEmployeeId, dtEmpPayrollRetro.Rows[i]["Mem_LastName"].ToString(), dtEmpPayrollRetro.Rows[i]["Mem_FirstName"].ToString()));
                        }
                        catch (Exception ex)
                        {
                            CommonProcedures.logErrorToFile(ex.ToString());
                            EmpDispHandler(this, new EmpDispEventArgs(curEmployeeId, dtEmpPayrollRetro.Rows[i]["Mem_LastName"].ToString(), dtEmpPayrollRetro.Rows[i]["Mem_FirstName"].ToString(), "Error in Retro Pay Calculation : " + ex.Message));
                            AddErrorToRetroReport(curEmployeeId, dtEmpPayrollRetro.Rows[i]["Mem_LastName"].ToString(), dtEmpPayrollRetro.Rows[i]["Mem_FirstName"].ToString(), ProcessPayrollPeriod, 0, 0, ex.Message.Substring(0, Math.Min(ex.Message.Length, 1000)));
                        }
                    }
                }
                //-----------------------------END MAIN PROCESS---------------------------
                StatusHandler(this, new StatusEventArgs("Saving Retro Pay Records", false));
                //Save Payroll Table
                string strUpdateRecordTemplate;
                string strUpdateQuery = "";
                int iUpdateCtr = 0;
                #region Payroll Record Insert
                strUpdateRecordTemplate = @" INSERT INTO T_EmpPayrollRetro (Tpy_IDNo,Tpy_RetroPayCycle,Tpy_PayCycle,Tpy_SalaryRate,Tpy_HourRate,Tpy_PayrollType,Tpy_SpecialRate,Tpy_SpecialHourRate,Tpy_LTHr,Tpy_LTAmt,Tpy_LTVarAmt,Tpy_UTHr,Tpy_UTAmt,Tpy_UTVarAmt,Tpy_UPLVHr,Tpy_UPLVAmt,Tpy_UPLVVarAmt,Tpy_ABSLEGHOLHr,Tpy_ABSLEGHOLAmt,Tpy_ABSLEGHOLVarAmt,Tpy_ABSSPLHOLHr,Tpy_ABSSPLHOLAmt,Tpy_ABSSPLHOLVarAmt,Tpy_ABSCOMPHOLHr,Tpy_ABSCOMPHOLAmt,Tpy_ABSCOMPHOLVarAmt,Tpy_ABSPSDHr,Tpy_ABSPSDAmt,Tpy_ABSPSDVarAmt,Tpy_ABSOTHHOLHr,Tpy_ABSOTHHOLAmt,Tpy_ABSOTHHOLVarAmt,Tpy_WDABSHr,Tpy_WDABSAmt,Tpy_WDABSVarAmt,Tpy_LTUTMaxHr,Tpy_LTUTMaxAmt,Tpy_LTUTMaxVarAmt,Tpy_ABSHr,Tpy_ABSAmt,Tpy_ABSVarAmt,Tpy_REGHr,Tpy_REGAmt,Tpy_REGVarAmt,Tpy_PDLVHr,Tpy_PDLVAmt,Tpy_PDLVVarAmt,Tpy_PDLEGHOLHr,Tpy_PDLEGHOLAmt,Tpy_PDLEGHOLVarAmt,Tpy_PDSPLHOLHr,Tpy_PDSPLHOLAmt,Tpy_PDSPLHOLVarAmt,Tpy_PDCOMPHOLHr,Tpy_PDCOMPHOLAmt,Tpy_PDCOMPHOLVarAmt,Tpy_PDPSDHr,Tpy_PDPSDAmt,Tpy_PDPSDVarAmt,Tpy_PDOTHHOLHr,Tpy_PDOTHHOLAmt,Tpy_PDOTHHOLVarAmt,Tpy_PDRESTLEGHOLHr,Tpy_PDRESTLEGHOLAmt,Tpy_PDRESTLEGHOLVarAmt,Tpy_TotalREGHr,Tpy_TotalREGAmt,Tpy_TotalREGVarAmt,Tpy_REGOTHr,Tpy_REGOTAmt,Tpy_REGOTVarAmt,Tpy_REGNDHr,Tpy_REGNDAmt,Tpy_REGNDVarAmt,Tpy_REGNDOTHr,Tpy_REGNDOTAmt,Tpy_REGNDOTVarAmt,Tpy_RESTHr,Tpy_RESTAmt,Tpy_RESTVarAmt,Tpy_RESTOTHr,Tpy_RESTOTAmt,Tpy_RESTOTVarAmt,Tpy_RESTNDHr,Tpy_RESTNDAmt,Tpy_RESTNDVarAmt,Tpy_RESTNDOTHr,Tpy_RESTNDOTAmt,Tpy_RESTNDOTVarAmt,Tpy_LEGHOLHr,Tpy_LEGHOLAmt,Tpy_LEGHOLVarAmt,Tpy_LEGHOLOTHr,Tpy_LEGHOLOTAmt,Tpy_LEGHOLOTVarAmt,Tpy_LEGHOLNDHr,Tpy_LEGHOLNDAmt,Tpy_LEGHOLNDVarAmt,Tpy_LEGHOLNDOTHr,Tpy_LEGHOLNDOTAmt,Tpy_LEGHOLNDOTVarAmt,Tpy_SPLHOLHr,Tpy_SPLHOLAmt,Tpy_SPLHOLVarAmt,Tpy_SPLHOLOTHr,Tpy_SPLHOLOTAmt,Tpy_SPLHOLOTVarAmt,Tpy_SPLHOLNDHr,Tpy_SPLHOLNDAmt,Tpy_SPLHOLNDVarAmt,Tpy_SPLHOLNDOTHr,Tpy_SPLHOLNDOTAmt,Tpy_SPLHOLNDOTVarAmt,Tpy_PSDHr,Tpy_PSDAmt,Tpy_PSDVarAmt,Tpy_PSDOTHr,Tpy_PSDOTAmt,Tpy_PSDOTVarAmt,Tpy_PSDNDHr,Tpy_PSDNDAmt,Tpy_PSDNDVarAmt,Tpy_PSDNDOTHr,Tpy_PSDNDOTAmt,Tpy_PSDNDOTVarAmt,Tpy_COMPHOLHr,Tpy_COMPHOLAmt,Tpy_COMPHOLVarAmt,Tpy_COMPHOLOTHr,Tpy_COMPHOLOTAmt,Tpy_COMPHOLOTVarAmt,Tpy_COMPHOLNDHr,Tpy_COMPHOLNDAmt,Tpy_COMPHOLNDVarAmt,Tpy_COMPHOLNDOTHr,Tpy_COMPHOLNDOTAmt,Tpy_COMPHOLNDOTVarAmt,Tpy_RESTLEGHOLHr,Tpy_RESTLEGHOLAmt,Tpy_RESTLEGHOLVarAmt,Tpy_RESTLEGHOLOTHr,Tpy_RESTLEGHOLOTAmt,Tpy_RESTLEGHOLOTVarAmt,Tpy_RESTLEGHOLNDHr,Tpy_RESTLEGHOLNDAmt,Tpy_RESTLEGHOLNDVarAmt,Tpy_RESTLEGHOLNDOTHr,Tpy_RESTLEGHOLNDOTAmt,Tpy_RESTLEGHOLNDOTVarAmt,Tpy_RESTSPLHOLHr,Tpy_RESTSPLHOLAmt,Tpy_RESTSPLHOLVarAmt,Tpy_RESTSPLHOLOTHr,Tpy_RESTSPLHOLOTAmt,Tpy_RESTSPLHOLOTVarAmt,Tpy_RESTSPLHOLNDHr,Tpy_RESTSPLHOLNDAmt,Tpy_RESTSPLHOLNDVarAmt,Tpy_RESTSPLHOLNDOTHr,Tpy_RESTSPLHOLNDOTAmt,Tpy_RESTSPLHOLNDOTVarAmt,Tpy_RESTCOMPHOLHr,Tpy_RESTCOMPHOLAmt,Tpy_RESTCOMPHOLVarAmt,Tpy_RESTCOMPHOLOTHr,Tpy_RESTCOMPHOLOTAmt,Tpy_RESTCOMPHOLOTVarAmt,Tpy_RESTCOMPHOLNDHr,Tpy_RESTCOMPHOLNDAmt,Tpy_RESTCOMPHOLNDVarAmt,Tpy_RESTCOMPHOLNDOTHr,Tpy_RESTCOMPHOLNDOTAmt,Tpy_RESTCOMPHOLNDOTVarAmt,Tpy_RESTPSDHr,Tpy_RESTPSDAmt,Tpy_RESTPSDVarAmt,Tpy_RESTPSDOTHr,Tpy_RESTPSDOTAmt,Tpy_RESTPSDOTVarAmt,Tpy_RESTPSDNDHr,Tpy_RESTPSDNDAmt,Tpy_RESTPSDNDVarAmt,Tpy_RESTPSDNDOTHr,Tpy_RESTPSDNDOTAmt,Tpy_RESTPSDNDOTVarAmt,Tpy_TotalOTNDAmt,Tpy_DayCountOldSalaryRate,Tpy_IsMultipleSalary,Tpy_RegRule,Tpy_RGRetroAmt,Tpy_OTRetroAmt,Tpy_NDRetroAmt,Tpy_HLRetroAmt,Tpy_LVRetroAmt,Tpy_TotalRetroAmt,Tpy_CostcenterCode,Tpy_EmploymentStatus,Tpy_PayrollGroup,Usr_Login,Ludatetime) VALUES ('{0}','{1}','{2}',{3},{4},'{5}',{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34},{35},{36},{37},{38},{39},{40},{41},{42},{43},{44},{45},{46},{47},{48},{49},{50},{51},{52},{53},{54},{55},{56},{57},{58},{59},{60},{61},{62},{63},{64},{65},{66},{67},{68},{69},{70},{71},{72},{73},{74},{75},{76},{77},{78},{79},{80},{81},{82},{83},{84},{85},{86},{87},{88},{89},{90},{91},{92},{93},{94},{95},{96},{97},{98},{99},{100},{101},{102},{103},{104},{105},{106},{107},{108},{109},{110},{111},{112},{113},{114},{115},{116},{117},{118},{119},{120},{121},{122},{123},{124},{125},{126},{127},{128},{129},{130},{131},{132},{133},{134},{135},{136},{137},{138},{139},{140},{141},{142},{143},{144},{145},{146},{147},{148},{149},{150},{151},{152},{153},{154},{155},{156},{157},{158},{159},{160},{161},{162},{163},{164},{165},{166},{167},{168},{169},{170},{171},{172},{173},{174},{175},{176},{177},{178},{179},{180},{181},{182},{183},{184},{185},{186},'{187}','{188}',{189},{190},{191},{192},{193},{194},'{195}','{196}','{197}','{198}',GETDATE()) ";
                #endregion
                foreach (DataRow drPayrollCalc in dtEmpPayroll.Rows)
                {
                    #region Payroll Retro Insert
                    strUpdateQuery += string.Format(strUpdateRecordTemplate
                                                       , drPayrollCalc["Tpy_IDNo"]                          //0
                                                       , drPayrollCalc["Tpy_RetroPayCycle"]                 //1
                                                       , drPayrollCalc["Tpy_PayCycle"]                      //2
                                                       , drPayrollCalc["Tpy_SalaryRate"]                    //3
                                                       , drPayrollCalc["Tpy_HourRate"]                      //4
                                                       , drPayrollCalc["Tpy_PayrollType"]                   //5
                                                       , drPayrollCalc["Tpy_SpecialRate"]                   
                                                       , drPayrollCalc["Tpy_SpecialHourRate"]                
                                                       , drPayrollCalc["Tpy_LTHr"]
                                                       , drPayrollCalc["Tpy_LTAmt"]
                                                       , drPayrollCalc["Tpy_LTVarAmt"]                      //10
                                                       , drPayrollCalc["Tpy_UTHr"]                          
                                                       , drPayrollCalc["Tpy_UTAmt"]
                                                       , drPayrollCalc["Tpy_UTVarAmt"]
                                                       , drPayrollCalc["Tpy_UPLVHr"]
                                                       , drPayrollCalc["Tpy_UPLVAmt"]                       //15
                                                       , drPayrollCalc["Tpy_UPLVVarAmt"]
                                                       , drPayrollCalc["Tpy_ABSLEGHOLHr"]
                                                       , drPayrollCalc["Tpy_ABSLEGHOLAmt"]                  
                                                       , drPayrollCalc["Tpy_ABSLEGHOLVarAmt"]
                                                       , drPayrollCalc["Tpy_ABSSPLHOLHr"]                   //20                
                                                       , drPayrollCalc["Tpy_ABSSPLHOLAmt"]
                                                       , drPayrollCalc["Tpy_ABSSPLHOLVarAmt"]
                                                       , drPayrollCalc["Tpy_ABSCOMPHOLHr"]
                                                       , drPayrollCalc["Tpy_ABSCOMPHOLAmt"]
                                                       , drPayrollCalc["Tpy_ABSCOMPHOLVarAmt"]              //25
                                                       , drPayrollCalc["Tpy_ABSPSDHr"]                      
                                                       , drPayrollCalc["Tpy_ABSPSDAmt"]
                                                       , drPayrollCalc["Tpy_ABSPSDVarAmt"]
                                                       , drPayrollCalc["Tpy_ABSOTHHOLHr"]
                                                       , drPayrollCalc["Tpy_ABSOTHHOLAmt"]                  //30
                                                       , drPayrollCalc["Tpy_ABSOTHHOLVarAmt"]
                                                       , drPayrollCalc["Tpy_WDABSHr"]
                                                       , drPayrollCalc["Tpy_WDABSAmt"]                      
                                                       , drPayrollCalc["Tpy_WDABSVarAmt"]
                                                       , drPayrollCalc["Tpy_LTUTMaxHr"]                     //35
                                                       , drPayrollCalc["Tpy_LTUTMaxAmt"]
                                                       , drPayrollCalc["Tpy_LTUTMaxVarAmt"]
                                                       , drPayrollCalc["Tpy_ABSHr"]                         
                                                       , drPayrollCalc["Tpy_ABSAmt"]
                                                       , drPayrollCalc["Tpy_ABSVarAmt"]                     //40
                                                       , drPayrollCalc["Tpy_REGHr"]                         
                                                       , drPayrollCalc["Tpy_REGAmt"]
                                                       , drPayrollCalc["Tpy_REGVarAmt"]
                                                       , drPayrollCalc["Tpy_PDLVHr"]                        
                                                       , drPayrollCalc["Tpy_PDLVAmt"]                       //45
                                                       , drPayrollCalc["Tpy_PDLVVarAmt"]                    
                                                       , drPayrollCalc["Tpy_PDLEGHOLHr"]
                                                       , drPayrollCalc["Tpy_PDLEGHOLAmt"]                   
                                                       , drPayrollCalc["Tpy_PDLEGHOLVarAmt"]
                                                       , drPayrollCalc["Tpy_PDSPLHOLHr"]                    //50
                                                       , drPayrollCalc["Tpy_PDSPLHOLAmt"]
                                                       , drPayrollCalc["Tpy_PDSPLHOLVarAmt"]
                                                       , drPayrollCalc["Tpy_PDCOMPHOLHr"]                   
                                                       , drPayrollCalc["Tpy_PDCOMPHOLAmt"]
                                                       , drPayrollCalc["Tpy_PDCOMPHOLVarAmt"]               //55
                                                       , drPayrollCalc["Tpy_PDPSDHr"]                       
                                                       , drPayrollCalc["Tpy_PDPSDAmt"]
                                                       , drPayrollCalc["Tpy_PDPSDVarAmt"]
                                                       , drPayrollCalc["Tpy_PDOTHHOLHr"]                    
                                                       , drPayrollCalc["Tpy_PDOTHHOLAmt"]                   //60
                                                       , drPayrollCalc["Tpy_PDOTHHOLVarAmt"]
                                                       , drPayrollCalc["Tpy_PDRESTLEGHOLHr"]
                                                       , drPayrollCalc["Tpy_PDRESTLEGHOLAmt"]               
                                                       , drPayrollCalc["Tpy_PDRESTLEGHOLVarAmt"]
                                                       , drPayrollCalc["Tpy_TotalREGHr"]                    //65
                                                       , drPayrollCalc["Tpy_TotalREGAmt"]
                                                       , drPayrollCalc["Tpy_TotalREGVarAmt"]
                                                       , drPayrollCalc["Tpy_REGOTHr"]                       
                                                       , drPayrollCalc["Tpy_REGOTAmt"]
                                                       , drPayrollCalc["Tpy_REGOTVarAmt"]                   //70
                                                       , drPayrollCalc["Tpy_REGNDHr"]                    
                                                       , drPayrollCalc["Tpy_REGNDAmt"]
                                                       , drPayrollCalc["Tpy_REGNDVarAmt"]
                                                       , drPayrollCalc["Tpy_REGNDOTHr"]                     
                                                       , drPayrollCalc["Tpy_REGNDOTAmt"]                    //75
                                                       , drPayrollCalc["Tpy_REGNDOTVarAmt"]
                                                       , drPayrollCalc["Tpy_RESTHr"]
                                                       , drPayrollCalc["Tpy_RESTAmt"]                       
                                                       , drPayrollCalc["Tpy_RESTVarAmt"]
                                                       , drPayrollCalc["Tpy_RESTOTHr"]                      //80
                                                       , drPayrollCalc["Tpy_RESTOTAmt"]
                                                       , drPayrollCalc["Tpy_RESTOTVarAmt"]
                                                       , drPayrollCalc["Tpy_RESTNDHr"]                      
                                                       , drPayrollCalc["Tpy_RESTNDAmt"]
                                                       , drPayrollCalc["Tpy_RESTNDVarAmt"]                  //85
                                                       , drPayrollCalc["Tpy_RESTNDOTHr"]                    
                                                       , drPayrollCalc["Tpy_RESTNDOTAmt"]
                                                       , drPayrollCalc["Tpy_RESTNDOTVarAmt"]
                                                       , drPayrollCalc["Tpy_LEGHOLHr"]                      
                                                       , drPayrollCalc["Tpy_LEGHOLAmt"]                     //90
                                                       , drPayrollCalc["Tpy_LEGHOLVarAmt"]
                                                       , drPayrollCalc["Tpy_LEGHOLOTHr"]
                                                       , drPayrollCalc["Tpy_LEGHOLOTAmt"]                   
                                                       , drPayrollCalc["Tpy_LEGHOLOTVarAmt"]
                                                       , drPayrollCalc["Tpy_LEGHOLNDHr"]                    //95
                                                       , drPayrollCalc["Tpy_LEGHOLNDAmt"]
                                                       , drPayrollCalc["Tpy_LEGHOLNDVarAmt"]
                                                       , drPayrollCalc["Tpy_LEGHOLNDOTHr"]                  
                                                       , drPayrollCalc["Tpy_LEGHOLNDOTAmt"]
                                                       , drPayrollCalc["Tpy_LEGHOLNDOTVarAmt"]              //100
                                                       , drPayrollCalc["Tpy_SPLHOLHr"]                      
                                                       , drPayrollCalc["Tpy_SPLHOLAmt"]
                                                       , drPayrollCalc["Tpy_SPLHOLVarAmt"]
                                                       , drPayrollCalc["Tpy_SPLHOLOTHr"]                   
                                                       , drPayrollCalc["Tpy_SPLHOLOTAmt"]                   //105
                                                       , drPayrollCalc["Tpy_SPLHOLOTVarAmt"]
                                                       , drPayrollCalc["Tpy_SPLHOLNDHr"]
                                                       , drPayrollCalc["Tpy_SPLHOLNDAmt"]                   
                                                       , drPayrollCalc["Tpy_SPLHOLNDVarAmt"]
                                                       , drPayrollCalc["Tpy_SPLHOLNDOTHr"]                  //110
                                                       , drPayrollCalc["Tpy_SPLHOLNDOTAmt"]
                                                       , drPayrollCalc["Tpy_SPLHOLNDOTVarAmt"]
                                                       , drPayrollCalc["Tpy_PSDHr"]                         
                                                       , drPayrollCalc["Tpy_PSDAmt"]
                                                       , drPayrollCalc["Tpy_PSDVarAmt"]                     //115
                                                       , drPayrollCalc["Tpy_PSDOTHr"]                       
                                                       , drPayrollCalc["Tpy_PSDOTAmt"]
                                                       , drPayrollCalc["Tpy_PSDOTVarAmt"]
                                                       , drPayrollCalc["Tpy_PSDNDHr"]                       
                                                       , drPayrollCalc["Tpy_PSDNDAmt"]                      //120
                                                       , drPayrollCalc["Tpy_PSDNDVarAmt"]
                                                       , drPayrollCalc["Tpy_PSDNDOTHr"]
                                                       , drPayrollCalc["Tpy_PSDNDOTAmt"]                    
                                                       , drPayrollCalc["Tpy_PSDNDOTVarAmt"]
                                                       , drPayrollCalc["Tpy_COMPHOLHr"]                     //125
                                                       , drPayrollCalc["Tpy_COMPHOLAmt"]
                                                       , drPayrollCalc["Tpy_COMPHOLVarAmt"]
                                                       , drPayrollCalc["Tpy_COMPHOLOTHr"]                   
                                                       , drPayrollCalc["Tpy_COMPHOLOTAmt"]
                                                       , drPayrollCalc["Tpy_COMPHOLOTVarAmt"]               //130
                                                       , drPayrollCalc["Tpy_COMPHOLNDHr"]                   
                                                       , drPayrollCalc["Tpy_COMPHOLNDAmt"]                  
                                                       , drPayrollCalc["Tpy_COMPHOLNDVarAmt"]
                                                       , drPayrollCalc["Tpy_COMPHOLNDOTHr"]                 
                                                       , drPayrollCalc["Tpy_COMPHOLNDOTAmt"]                //135
                                                       , drPayrollCalc["Tpy_COMPHOLNDOTVarAmt"]
                                                       , drPayrollCalc["Tpy_RESTLEGHOLHr"]
                                                       , drPayrollCalc["Tpy_RESTLEGHOLAmt"]
                                                       , drPayrollCalc["Tpy_RESTLEGHOLVarAmt"]
                                                       , drPayrollCalc["Tpy_RESTLEGHOLOTHr"]                //140              
                                                       , drPayrollCalc["Tpy_RESTLEGHOLOTAmt"]
                                                       , drPayrollCalc["Tpy_RESTLEGHOLOTVarAmt"]
                                                       , drPayrollCalc["Tpy_RESTLEGHOLNDHr"]                
                                                       , drPayrollCalc["Tpy_RESTLEGHOLNDAmt"]
                                                       , drPayrollCalc["Tpy_RESTLEGHOLNDVarAmt"]            //145
                                                       , drPayrollCalc["Tpy_RESTLEGHOLNDOTHr"]              
                                                       , drPayrollCalc["Tpy_RESTLEGHOLNDOTAmt"]
                                                       , drPayrollCalc["Tpy_RESTLEGHOLNDOTVarAmt"]
                                                       , drPayrollCalc["Tpy_RESTSPLHOLHr"]                  
                                                       , drPayrollCalc["Tpy_RESTSPLHOLAmt"]                 //150
                                                       , drPayrollCalc["Tpy_RESTSPLHOLVarAmt"]
                                                       , drPayrollCalc["Tpy_RESTSPLHOLOTHr"]
                                                       , drPayrollCalc["Tpy_RESTSPLHOLOTAmt"]               
                                                       , drPayrollCalc["Tpy_RESTSPLHOLOTVarAmt"]
                                                       , drPayrollCalc["Tpy_RESTSPLHOLNDHr"]                //155
                                                       , drPayrollCalc["Tpy_RESTSPLHOLNDAmt"]
                                                       , drPayrollCalc["Tpy_RESTSPLHOLNDVarAmt"]
                                                       , drPayrollCalc["Tpy_RESTSPLHOLNDOTHr"]              
                                                       , drPayrollCalc["Tpy_RESTSPLHOLNDOTAmt"]
                                                       , drPayrollCalc["Tpy_RESTSPLHOLNDOTVarAmt"]          //160
                                                       , drPayrollCalc["Tpy_RESTCOMPHOLHr"]                 
                                                       , drPayrollCalc["Tpy_RESTCOMPHOLAmt"]
                                                       , drPayrollCalc["Tpy_RESTCOMPHOLVarAmt"]
                                                       , drPayrollCalc["Tpy_RESTCOMPHOLOTHr"]               
                                                       , drPayrollCalc["Tpy_RESTCOMPHOLOTAmt"]              //165
                                                       , drPayrollCalc["Tpy_RESTCOMPHOLOTVarAmt"]
                                                       , drPayrollCalc["Tpy_RESTCOMPHOLNDHr"]
                                                       , drPayrollCalc["Tpy_RESTCOMPHOLNDAmt"]              
                                                       , drPayrollCalc["Tpy_RESTCOMPHOLNDVarAmt"]
                                                       , drPayrollCalc["Tpy_RESTCOMPHOLNDOTHr"]             //170
                                                       , drPayrollCalc["Tpy_RESTCOMPHOLNDOTAmt"]
                                                       , drPayrollCalc["Tpy_RESTCOMPHOLNDOTVarAmt"]
                                                       , drPayrollCalc["Tpy_RESTPSDHr"]                     
                                                       , drPayrollCalc["Tpy_RESTPSDAmt"]
                                                       , drPayrollCalc["Tpy_RESTPSDVarAmt"]                 //175
                                                       , drPayrollCalc["Tpy_RESTPSDOTHr"]                   
                                                       , drPayrollCalc["Tpy_RESTPSDOTAmt"]
                                                       , drPayrollCalc["Tpy_RESTPSDOTVarAmt"]
                                                       , drPayrollCalc["Tpy_RESTPSDNDHr"]                   
                                                       , drPayrollCalc["Tpy_RESTPSDNDAmt"]                  //180
                                                       , drPayrollCalc["Tpy_RESTPSDNDVarAmt"]
                                                       , drPayrollCalc["Tpy_RESTPSDNDOTHr"]
                                                       , drPayrollCalc["Tpy_RESTPSDNDOTAmt"]                
                                                       , drPayrollCalc["Tpy_RESTPSDNDOTVarAmt"]
                                                       , drPayrollCalc["Tpy_TotalOTNDAmt"]                  //185
                                                       , drPayrollCalc["Tpy_DayCountOldSalaryRate"]
                                                       , drPayrollCalc["Tpy_IsMultipleSalary"]
                                                       , drPayrollCalc["Tpy_RegRule"]
                                                       , drPayrollCalc["Tpy_RGRetroAmt"]                                        
                                                       , drPayrollCalc["Tpy_OTRetroAmt"]                    //190              
                                                       , drPayrollCalc["Tpy_NDRetroAmt"]                    
                                                       , drPayrollCalc["Tpy_HLRetroAmt"]
                                                       , drPayrollCalc["Tpy_LVRetroAmt"]                    
                                                       , drPayrollCalc["Tpy_TotalRetroAmt"]                  
                                                       , drPayrollCalc["Tpy_CostcenterCode"]                //195  
                                                       , drPayrollCalc["Tpy_EmploymentStatus"]              
                                                       , drPayrollCalc["Tpy_PayrollGroup"]                                  
                                                       , drPayrollCalc["Usr_Login"]                         //198          
                                                       );
                    #endregion
                    iUpdateCtr++;
                    if (iUpdateCtr == 100)
                    {
                        dal.ExecuteNonQuery(strUpdateQuery);
                        strUpdateQuery = "";
                        iUpdateCtr = 0;
                    }
                }
                if (strUpdateQuery != "")
                    dal.ExecuteNonQuery(strUpdateQuery);
                //-----------------------------
                strUpdateQuery = "";
                iUpdateCtr = 0;
                #region Payroll Dtl Record Insert
                strUpdateRecordTemplate = @" INSERT INTO T_EmpPayrollDtlRetro (Tpd_IDNo,Tpd_RetroPayCycle,Tpd_PayCycle,Tpd_Date,Tpd_DayCode,Tpd_RestDayFlag,Tpd_SalaryRate,Tpd_HourRate,Tpd_PayrollType,Tpd_SpecialRate,Tpd_SpecialHourRate,Tpd_PremiumGrpCode,Tpd_LTHr,Tpd_LTAmt,Tpd_LTVarAmt,Tpd_UTHr,Tpd_UTAmt,Tpd_UTVarAmt,Tpd_UPLVHr,Tpd_UPLVAmt,Tpd_UPLVVarAmt,Tpd_ABSLEGHOLHr,Tpd_ABSLEGHOLAmt,Tpd_ABSLEGHOLVarAmt,Tpd_ABSSPLHOLHr,Tpd_ABSSPLHOLAmt,Tpd_ABSSPLHOLVarAmt,Tpd_ABSCOMPHOLHr,Tpd_ABSCOMPHOLAmt,Tpd_ABSCOMPHOLVarAmt,Tpd_ABSPSDHr,Tpd_ABSPSDAmt,Tpd_ABSPSDVarAmt,Tpd_ABSOTHHOLHr,Tpd_ABSOTHHOLAmt,Tpd_ABSOTHHOLVarAmt,Tpd_WDABSHr,Tpd_WDABSAmt,Tpd_WDABSVarAmt,Tpd_LTUTMaxHr,Tpd_LTUTMaxAmt,Tpd_LTUTMaxVarAmt,Tpd_ABSHr,Tpd_ABSAmt,Tpd_ABSVarAmt,Tpd_REGHr,Tpd_REGAmt,Tpd_REGVarAmt,Tpd_PDLVHr,Tpd_PDLVAmt,Tpd_PDLVVarAmt,Tpd_PDLEGHOLHr,Tpd_PDLEGHOLAmt,Tpd_PDLEGHOLVarAmt,Tpd_PDSPLHOLHr,Tpd_PDSPLHOLAmt,Tpd_PDSPLHOLVarAmt,Tpd_PDCOMPHOLHr,Tpd_PDCOMPHOLAmt,Tpd_PDCOMPHOLVarAmt,Tpd_PDPSDHr,Tpd_PDPSDAmt,Tpd_PDPSDVarAmt,Tpd_PDOTHHOLHr,Tpd_PDOTHHOLAmt,Tpd_PDOTHHOLVarAmt,Tpd_PDRESTLEGHOLHr,Tpd_PDRESTLEGHOLAmt,Tpd_PDRESTLEGHOLVarAmt,Tpd_TotalREGHr,Tpd_TotalREGAmt,Tpd_TotalREGVarAmt,Tpd_REGOTHr,Tpd_REGOTAmt,Tpd_REGOTVarAmt,Tpd_REGNDHr,Tpd_REGNDAmt,Tpd_REGNDVarAmt,Tpd_REGNDOTHr,Tpd_REGNDOTAmt,Tpd_REGNDOTVarAmt,Tpd_RESTHr,Tpd_RESTAmt,Tpd_RESTVarAmt,Tpd_RESTOTHr,Tpd_RESTOTAmt,Tpd_RESTOTVarAmt,Tpd_RESTNDHr,Tpd_RESTNDAmt,Tpd_RESTNDVarAmt,Tpd_RESTNDOTHr,Tpd_RESTNDOTAmt,Tpd_RESTNDOTVarAmt,Tpd_LEGHOLHr,Tpd_LEGHOLAmt,Tpd_LEGHOLVarAmt,Tpd_LEGHOLOTHr,Tpd_LEGHOLOTAmt,Tpd_LEGHOLOTVarAmt,Tpd_LEGHOLNDHr,Tpd_LEGHOLNDAmt,Tpd_LEGHOLNDVarAmt,Tpd_LEGHOLNDOTHr,Tpd_LEGHOLNDOTAmt,Tpd_LEGHOLNDOTVarAmt,Tpd_SPLHOLHr,Tpd_SPLHOLAmt,Tpd_SPLHOLVarAmt,Tpd_SPLHOLOTHr,Tpd_SPLHOLOTAmt,Tpd_SPLHOLOTVarAmt,Tpd_SPLHOLNDHr,Tpd_SPLHOLNDAmt,Tpd_SPLHOLNDVarAmt,Tpd_SPLHOLNDOTHr,Tpd_SPLHOLNDOTAmt,Tpd_SPLHOLNDOTVarAmt,Tpd_PSDHr,Tpd_PSDAmt,Tpd_PSDVarAmt,Tpd_PSDOTHr,Tpd_PSDOTAmt,Tpd_PSDOTVarAmt,Tpd_PSDNDHr,Tpd_PSDNDAmt,Tpd_PSDNDVarAmt,Tpd_PSDNDOTHr,Tpd_PSDNDOTAmt,Tpd_PSDNDOTVarAmt,Tpd_COMPHOLHr,Tpd_COMPHOLAmt,Tpd_COMPHOLVarAmt,Tpd_COMPHOLOTHr,Tpd_COMPHOLOTAmt,Tpd_COMPHOLOTVarAmt,Tpd_COMPHOLNDHr,Tpd_COMPHOLNDAmt,Tpd_COMPHOLNDVarAmt,Tpd_COMPHOLNDOTHr,Tpd_COMPHOLNDOTAmt,Tpd_COMPHOLNDOTVarAmt,Tpd_RESTLEGHOLHr,Tpd_RESTLEGHOLAmt,Tpd_RESTLEGHOLVarAmt,Tpd_RESTLEGHOLOTHr,Tpd_RESTLEGHOLOTAmt,Tpd_RESTLEGHOLOTVarAmt,Tpd_RESTLEGHOLNDHr,Tpd_RESTLEGHOLNDAmt,Tpd_RESTLEGHOLNDVarAmt,Tpd_RESTLEGHOLNDOTHr,Tpd_RESTLEGHOLNDOTAmt,Tpd_RESTLEGHOLNDOTVarAmt,Tpd_RESTSPLHOLHr,Tpd_RESTSPLHOLAmt,Tpd_RESTSPLHOLVarAmt,Tpd_RESTSPLHOLOTHr,Tpd_RESTSPLHOLOTAmt,Tpd_RESTSPLHOLOTVarAmt,Tpd_RESTSPLHOLNDHr,Tpd_RESTSPLHOLNDAmt,Tpd_RESTSPLHOLNDVarAmt,Tpd_RESTSPLHOLNDOTHr,Tpd_RESTSPLHOLNDOTAmt,Tpd_RESTSPLHOLNDOTVarAmt,Tpd_RESTCOMPHOLHr,Tpd_RESTCOMPHOLAmt,Tpd_RESTCOMPHOLVarAmt,Tpd_RESTCOMPHOLOTHr,Tpd_RESTCOMPHOLOTAmt,Tpd_RESTCOMPHOLOTVarAmt,Tpd_RESTCOMPHOLNDHr,Tpd_RESTCOMPHOLNDAmt,Tpd_RESTCOMPHOLNDVarAmt,Tpd_RESTCOMPHOLNDOTHr,Tpd_RESTCOMPHOLNDOTAmt,Tpd_RESTCOMPHOLNDOTVarAmt,Tpd_RESTPSDHr,Tpd_RESTPSDAmt,Tpd_RESTPSDVarAmt,Tpd_RESTPSDOTHr,Tpd_RESTPSDOTAmt,Tpd_RESTPSDOTVarAmt,Tpd_RESTPSDNDHr,Tpd_RESTPSDNDAmt,Tpd_RESTPSDNDVarAmt,Tpd_RESTPSDNDOTHr,Tpd_RESTPSDNDOTAmt,Tpd_RESTPSDNDOTVarAmt,Tpd_RGRetroAmt,Tpd_OTRetroAmt,Tpd_NDRetroAmt,Tpd_HLRetroAmt,Tpd_LVRetroAmt,Tpd_TotalRetroAmt,Usr_Login,Ludatetime ) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}',{6},{7},'{8}',{9},{10},'{11}',{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34},{35},{36},{37},{38},{39},{40},{41},{42},{43},{44},{45},{46},{47},{48},{49},{50},{51},{52},{53},{54},{55},{56},{57},{58},{59},{60},{61},{62},{63},{64},{65},{66},{67},{68},{69},{70},{71},{72},{73},{74},{75},{76},{77},{78},{79},{80},{81},{82},{83},{84},{85},{86},{87},{88},{89},{90},{91},{92},{93},{94},{95},{96},{97},{98},{99},{100},{101},{102},{103},{104},{105},{106},{107},{108},{109},{110},{111},{112},{113},{114},{115},{116},{117},{118},{119},{120},{121},{122},{123},{124},{125},{126},{127},{128},{129},{130},{131},{132},{133},{134},{135},{136},{137},{138},{139},{140},{141},{142},{143},{144},{145},{146},{147},{148},{149},{150},{151},{152},{153},{154},{155},{156},{157},{158},{159},{160},{161},{162},{163},{164},{165},{166},{167},{168},{169},{170},{171},{172},{173},{174},{175},{176},{177},{178},{179},{180},{181},{182},{183},{184},{185},{186},{187},{188},{189},{190},{191},{192},{193},{194},'{195}',GETDATE()) ";
                #endregion
                foreach (DataRow drPayrollCalcDtl in dtEmpPayrollDtl.Rows)
                {
                    //Save Payroll Dtl Table
                    #region Payroll Dtl Insert
                    strUpdateQuery += string.Format(strUpdateRecordTemplate
                                                        , drPayrollCalcDtl["Tpd_IDNo"]                          //0
                                                        , drPayrollCalcDtl["Tpd_RetroPayCycle"]                 //1
                                                        , drPayrollCalcDtl["Tpd_PayCycle"]                      //2
                                                        , drPayrollCalcDtl["Tpd_Date"]                          //3
                                                        , drPayrollCalcDtl["Tpd_DayCode"]                       //4
                                                        , drPayrollCalcDtl["Tpd_RestDayFlag"]                   //5
                                                        , drPayrollCalcDtl["Tpd_SalaryRate"]
                                                        , drPayrollCalcDtl["Tpd_HourRate"]
                                                        , drPayrollCalcDtl["Tpd_PayrollType"]
                                                        , drPayrollCalcDtl["Tpd_SpecialRate"]
                                                        , drPayrollCalcDtl["Tpd_SpecialHourRate"]               //10
                                                        , drPayrollCalcDtl["Tpd_PremiumGrpCode"]
                                                        , drPayrollCalcDtl["Tpd_LTHr"]
                                                        , drPayrollCalcDtl["Tpd_LTAmt"]
                                                        , drPayrollCalcDtl["Tpd_LTVarAmt"]
                                                        , drPayrollCalcDtl["Tpd_UTHr"]                          //15
                                                        , drPayrollCalcDtl["Tpd_UTAmt"]
                                                        , drPayrollCalcDtl["Tpd_UTVarAmt"]                         
                                                        , drPayrollCalcDtl["Tpd_UPLVHr"]
                                                        , drPayrollCalcDtl["Tpd_UPLVAmt"]
                                                        , drPayrollCalcDtl["Tpd_UPLVVarAmt"]                    //20
                                                        , drPayrollCalcDtl["Tpd_ABSLEGHOLHr"]
                                                        , drPayrollCalcDtl["Tpd_ABSLEGHOLAmt"]
                                                        , drPayrollCalcDtl["Tpd_ABSLEGHOLVarAmt"]
                                                        , drPayrollCalcDtl["Tpd_ABSSPLHOLHr"]                   
                                                        , drPayrollCalcDtl["Tpd_ABSSPLHOLAmt"]                  //25
                                                        , drPayrollCalcDtl["Tpd_ABSSPLHOLVarAmt"]
                                                        , drPayrollCalcDtl["Tpd_ABSCOMPHOLHr"]
                                                        , drPayrollCalcDtl["Tpd_ABSCOMPHOLAmt"]
                                                        , drPayrollCalcDtl["Tpd_ABSCOMPHOLVarAmt"]
                                                        , drPayrollCalcDtl["Tpd_ABSPSDHr"]                      //30
                                                        , drPayrollCalcDtl["Tpd_ABSPSDAmt"]                     
                                                        , drPayrollCalcDtl["Tpd_ABSPSDVarAmt"]
                                                        , drPayrollCalcDtl["Tpd_ABSOTHHOLHr"]
                                                        , drPayrollCalcDtl["Tpd_ABSOTHHOLAmt"]
                                                        , drPayrollCalcDtl["Tpd_ABSOTHHOLVarAmt"]               //35
                                                        , drPayrollCalcDtl["Tpd_WDABSHr"]
                                                        , drPayrollCalcDtl["Tpd_WDABSAmt"]
                                                        , drPayrollCalcDtl["Tpd_WDABSVarAmt"]
                                                        , drPayrollCalcDtl["Tpd_LTUTMaxHr"]                     
                                                        , drPayrollCalcDtl["Tpd_LTUTMaxAmt"]                    //40
                                                        , drPayrollCalcDtl["Tpd_LTUTMaxVarAmt"]
                                                        , drPayrollCalcDtl["Tpd_ABSHr"]                         
                                                        , drPayrollCalcDtl["Tpd_ABSAmt"]
                                                        , drPayrollCalcDtl["Tpd_ABSVarAmt"]
                                                        , drPayrollCalcDtl["Tpd_REGHr"]                         //45
                                                        , drPayrollCalcDtl["Tpd_REGAmt"]                        
                                                        , drPayrollCalcDtl["Tpd_REGVarAmt"]
                                                        , drPayrollCalcDtl["Tpd_PDLVHr"]
                                                        , drPayrollCalcDtl["Tpd_PDLVAmt"]
                                                        , drPayrollCalcDtl["Tpd_PDLVVarAmt"]                    //50
                                                        , drPayrollCalcDtl["Tpd_PDLEGHOLHr"]
                                                        , drPayrollCalcDtl["Tpd_PDLEGHOLAmt"]
                                                        , drPayrollCalcDtl["Tpd_PDLEGHOLVarAmt"]
                                                        , drPayrollCalcDtl["Tpd_PDSPLHOLHr"]                    
                                                        , drPayrollCalcDtl["Tpd_PDSPLHOLAmt"]                   //55
                                                        , drPayrollCalcDtl["Tpd_PDSPLHOLVarAmt"]
                                                        , drPayrollCalcDtl["Tpd_PDCOMPHOLHr"]                   
                                                        , drPayrollCalcDtl["Tpd_PDCOMPHOLAmt"]
                                                        , drPayrollCalcDtl["Tpd_PDCOMPHOLVarAmt"]
                                                        , drPayrollCalcDtl["Tpd_PDPSDHr"]                       //60
                                                        , drPayrollCalcDtl["Tpd_PDPSDAmt"]          
                                                        , drPayrollCalcDtl["Tpd_PDPSDVarAmt"]
                                                        , drPayrollCalcDtl["Tpd_PDOTHHOLHr"]
                                                        , drPayrollCalcDtl["Tpd_PDOTHHOLAmt"]
                                                        , drPayrollCalcDtl["Tpd_PDOTHHOLVarAmt"]                //65
                                                        , drPayrollCalcDtl["Tpd_PDRESTLEGHOLHr"]
                                                        , drPayrollCalcDtl["Tpd_PDRESTLEGHOLAmt"]
                                                        , drPayrollCalcDtl["Tpd_PDRESTLEGHOLVarAmt"]
                                                        , drPayrollCalcDtl["Tpd_TotalREGHr"]                    
                                                        , drPayrollCalcDtl["Tpd_TotalREGAmt"]                   //70
                                                        , drPayrollCalcDtl["Tpd_TotalREGVarAmt"]
                                                        , drPayrollCalcDtl["Tpd_REGOTHr"]                       
                                                        , drPayrollCalcDtl["Tpd_REGOTAmt"]
                                                        , drPayrollCalcDtl["Tpd_REGOTVarAmt"]
                                                        , drPayrollCalcDtl["Tpd_REGNDHr"]                       //75
                                                        , drPayrollCalcDtl["Tpd_REGNDAmt"]                      
                                                        , drPayrollCalcDtl["Tpd_REGNDVarAmt"]
                                                        , drPayrollCalcDtl["Tpd_REGNDOTHr"]
                                                        , drPayrollCalcDtl["Tpd_REGNDOTAmt"]
                                                        , drPayrollCalcDtl["Tpd_REGNDOTVarAmt"]                 //80
                                                        , drPayrollCalcDtl["Tpd_RESTHr"]
                                                        , drPayrollCalcDtl["Tpd_RESTAmt"]
                                                        , drPayrollCalcDtl["Tpd_RESTVarAmt"]
                                                        , drPayrollCalcDtl["Tpd_RESTOTHr"]                      
                                                        , drPayrollCalcDtl["Tpd_RESTOTAmt"]                     //85
                                                        , drPayrollCalcDtl["Tpd_RESTOTVarAmt"]
                                                        , drPayrollCalcDtl["Tpd_RESTNDHr"]                      
                                                        , drPayrollCalcDtl["Tpd_RESTNDAmt"]
                                                        , drPayrollCalcDtl["Tpd_RESTNDVarAmt"]
                                                        , drPayrollCalcDtl["Tpd_RESTNDOTHr"]                    //90
                                                        , drPayrollCalcDtl["Tpd_RESTNDOTAmt"]                   
                                                        , drPayrollCalcDtl["Tpd_RESTNDOTVarAmt"]
                                                        , drPayrollCalcDtl["Tpd_LEGHOLHr"]
                                                        , drPayrollCalcDtl["Tpd_LEGHOLAmt"]
                                                        , drPayrollCalcDtl["Tpd_LEGHOLVarAmt"]                  //95
                                                        , drPayrollCalcDtl["Tpd_LEGHOLOTHr"]
                                                        , drPayrollCalcDtl["Tpd_LEGHOLOTAmt"]
                                                        , drPayrollCalcDtl["Tpd_LEGHOLOTVarAmt"]
                                                        , drPayrollCalcDtl["Tpd_LEGHOLNDHr"]                    
                                                        , drPayrollCalcDtl["Tpd_LEGHOLNDAmt"]                   //100
                                                        , drPayrollCalcDtl["Tpd_LEGHOLNDVarAmt"]
                                                        , drPayrollCalcDtl["Tpd_LEGHOLNDOTHr"]                  
                                                        , drPayrollCalcDtl["Tpd_LEGHOLNDOTAmt"]
                                                        , drPayrollCalcDtl["Tpd_LEGHOLNDOTVarAmt"]
                                                        , drPayrollCalcDtl["Tpd_SPLHOLHr"]                      //105
                                                        , drPayrollCalcDtl["Tpd_SPLHOLAmt"]                     
                                                        , drPayrollCalcDtl["Tpd_SPLHOLVarAmt"]
                                                        , drPayrollCalcDtl["Tpd_SPLHOLOTHr"]
                                                        , drPayrollCalcDtl["Tpd_SPLHOLOTAmt"]
                                                        , drPayrollCalcDtl["Tpd_SPLHOLOTVarAmt"]                //110
                                                        , drPayrollCalcDtl["Tpd_SPLHOLNDHr"]
                                                        , drPayrollCalcDtl["Tpd_SPLHOLNDAmt"]
                                                        , drPayrollCalcDtl["Tpd_SPLHOLNDVarAmt"]
                                                        , drPayrollCalcDtl["Tpd_SPLHOLNDOTHr"]                  
                                                        , drPayrollCalcDtl["Tpd_SPLHOLNDOTAmt"]                 //115
                                                        , drPayrollCalcDtl["Tpd_SPLHOLNDOTVarAmt"]
                                                        , drPayrollCalcDtl["Tpd_PSDHr"]                         
                                                        , drPayrollCalcDtl["Tpd_PSDAmt"]
                                                        , drPayrollCalcDtl["Tpd_PSDVarAmt"]
                                                        , drPayrollCalcDtl["Tpd_PSDOTHr"]                       //120
                                                        , drPayrollCalcDtl["Tpd_PSDOTAmt"]                      
                                                        , drPayrollCalcDtl["Tpd_PSDOTVarAmt"]
                                                        , drPayrollCalcDtl["Tpd_PSDNDHr"]
                                                        , drPayrollCalcDtl["Tpd_PSDNDAmt"]
                                                        , drPayrollCalcDtl["Tpd_PSDNDVarAmt"]                   //125
                                                        , drPayrollCalcDtl["Tpd_PSDNDOTHr"]
                                                        , drPayrollCalcDtl["Tpd_PSDNDOTAmt"]
                                                        , drPayrollCalcDtl["Tpd_PSDNDOTVarAmt"]
                                                        , drPayrollCalcDtl["Tpd_COMPHOLHr"]                     
                                                        , drPayrollCalcDtl["Tpd_COMPHOLAmt"]                    //130
                                                        , drPayrollCalcDtl["Tpd_COMPHOLVarAmt"]
                                                        , drPayrollCalcDtl["Tpd_COMPHOLOTHr"]                   
                                                        , drPayrollCalcDtl["Tpd_COMPHOLOTAmt"]
                                                        , drPayrollCalcDtl["Tpd_COMPHOLOTVarAmt"]
                                                        , drPayrollCalcDtl["Tpd_COMPHOLNDHr"]                   //135
                                                        , drPayrollCalcDtl["Tpd_COMPHOLNDAmt"]                  
                                                        , drPayrollCalcDtl["Tpd_COMPHOLNDVarAmt"]
                                                        , drPayrollCalcDtl["Tpd_COMPHOLNDOTHr"]
                                                        , drPayrollCalcDtl["Tpd_COMPHOLNDOTAmt"]
                                                        , drPayrollCalcDtl["Tpd_COMPHOLNDOTVarAmt"]             //140
                                                        , drPayrollCalcDtl["Tpd_RESTLEGHOLHr"]
                                                        , drPayrollCalcDtl["Tpd_RESTLEGHOLAmt"]
                                                        , drPayrollCalcDtl["Tpd_RESTLEGHOLVarAmt"]
                                                        , drPayrollCalcDtl["Tpd_RESTLEGHOLOTHr"]                
                                                        , drPayrollCalcDtl["Tpd_RESTLEGHOLOTAmt"]               //145
                                                        , drPayrollCalcDtl["Tpd_RESTLEGHOLOTVarAmt"]
                                                        , drPayrollCalcDtl["Tpd_RESTLEGHOLNDHr"]                
                                                        , drPayrollCalcDtl["Tpd_RESTLEGHOLNDAmt"]
                                                        , drPayrollCalcDtl["Tpd_RESTLEGHOLNDVarAmt"]
                                                        , drPayrollCalcDtl["Tpd_RESTLEGHOLNDOTHr"]              //150
                                                        , drPayrollCalcDtl["Tpd_RESTLEGHOLNDOTAmt"]             
                                                        , drPayrollCalcDtl["Tpd_RESTLEGHOLNDOTVarAmt"]
                                                        , drPayrollCalcDtl["Tpd_RESTSPLHOLHr"]
                                                        , drPayrollCalcDtl["Tpd_RESTSPLHOLAmt"] 
                                                        , drPayrollCalcDtl["Tpd_RESTSPLHOLVarAmt"]              //155
                                                        , drPayrollCalcDtl["Tpd_RESTSPLHOLOTHr"]
                                                        , drPayrollCalcDtl["Tpd_RESTSPLHOLOTAmt"]
                                                        , drPayrollCalcDtl["Tpd_RESTSPLHOLOTVarAmt"]
                                                        , drPayrollCalcDtl["Tpd_RESTSPLHOLNDHr"]                
                                                        , drPayrollCalcDtl["Tpd_RESTSPLHOLNDAmt"]               //160
                                                        , drPayrollCalcDtl["Tpd_RESTSPLHOLNDVarAmt"]
                                                        , drPayrollCalcDtl["Tpd_RESTSPLHOLNDOTHr"]              
                                                        , drPayrollCalcDtl["Tpd_RESTSPLHOLNDOTAmt"]
                                                        , drPayrollCalcDtl["Tpd_RESTSPLHOLNDOTVarAmt"]
                                                        , drPayrollCalcDtl["Tpd_RESTCOMPHOLHr"]                 //165
                                                        , drPayrollCalcDtl["Tpd_RESTCOMPHOLAmt"]                
                                                        , drPayrollCalcDtl["Tpd_RESTCOMPHOLVarAmt"]
                                                        , drPayrollCalcDtl["Tpd_RESTCOMPHOLOTHr"]
                                                        , drPayrollCalcDtl["Tpd_RESTCOMPHOLOTAmt"]
                                                        , drPayrollCalcDtl["Tpd_RESTCOMPHOLOTVarAmt"]           //170
                                                        , drPayrollCalcDtl["Tpd_RESTCOMPHOLNDHr"]
                                                        , drPayrollCalcDtl["Tpd_RESTCOMPHOLNDAmt"]
                                                        , drPayrollCalcDtl["Tpd_RESTCOMPHOLNDVarAmt"]
                                                        , drPayrollCalcDtl["Tpd_RESTCOMPHOLNDOTHr"]             
                                                        , drPayrollCalcDtl["Tpd_RESTCOMPHOLNDOTAmt"]            //175
                                                        , drPayrollCalcDtl["Tpd_RESTCOMPHOLNDOTVarAmt"]
                                                        , drPayrollCalcDtl["Tpd_RESTPSDHr"]                     
                                                        , drPayrollCalcDtl["Tpd_RESTPSDAmt"]
                                                        , drPayrollCalcDtl["Tpd_RESTPSDVarAmt"]
                                                        , drPayrollCalcDtl["Tpd_RESTPSDOTHr"]                   //180
                                                        , drPayrollCalcDtl["Tpd_RESTPSDOTAmt"]                  
                                                        , drPayrollCalcDtl["Tpd_RESTPSDOTVarAmt"]
                                                        , drPayrollCalcDtl["Tpd_RESTPSDNDHr"]
                                                        , drPayrollCalcDtl["Tpd_RESTPSDNDAmt"]
                                                        , drPayrollCalcDtl["Tpd_RESTPSDNDVarAmt"]               //185
                                                        , drPayrollCalcDtl["Tpd_RESTPSDNDOTHr"]
                                                        , drPayrollCalcDtl["Tpd_RESTPSDNDOTAmt"]
                                                        , drPayrollCalcDtl["Tpd_RESTPSDNDOTVarAmt"]
                                                        , drPayrollCalcDtl["Tpd_RGRetroAmt"]                    
                                                        , drPayrollCalcDtl["Tpd_OTRetroAmt"]                    //190
                                                        , drPayrollCalcDtl["Tpd_NDRetroAmt"]                    
                                                        , drPayrollCalcDtl["Tpd_HLRetroAmt"]
                                                        , drPayrollCalcDtl["Tpd_LVRetroAmt"]
                                                        , drPayrollCalcDtl["Tpd_TotalRetroAmt"]                 
                                                        , drPayrollCalcDtl["Usr_Login"]);                       //195         
                    #endregion
                    iUpdateCtr++;
                    if (iUpdateCtr == 100)
                    {
                        dal.ExecuteNonQuery(strUpdateQuery);
                        strUpdateQuery = "";
                        iUpdateCtr = 0;
                    }
                }
                if (strUpdateQuery != "")
                    dal.ExecuteNonQuery(strUpdateQuery);
                //-----------------------------
                strUpdateQuery = "";
                iUpdateCtr = 0;
                #region System Adj2 Record Insert
                strUpdateRecordTemplate = @" INSERT INTO T_EmpSystemAdj2Retro (Tsa_IDNo,Tsa_RetroPayCycle,Tsa_AdjPayCycle,Tsa_OrigAdjPayCycle,Tsa_PayCycle,Tsa_Date,Tsa_SalaryRate,Tsa_HourRate,Tsa_PayrollType,Tsa_SpecialRate,Tsa_SpecialHourRate,Tsa_PremiumGrpCode,Tsa_LTAmt,Tsa_UTAmt,Tsa_UPLVAmt,Tsa_ABSLEGHOLAmt,Tsa_ABSSPLHOLAmt,Tsa_ABSCOMPHOLAmt,Tsa_ABSPSDAmt,Tsa_ABSOTHHOLAmt,Tsa_WDABSAmt,Tsa_LTUTMaxAmt,Tsa_ABSAmt,Tsa_REGAmt,Tsa_PDLVAmt,Tsa_PDLEGHOLAmt,Tsa_PDSPLHOLAmt,Tsa_PDCOMPHOLAmt,Tsa_PDPSDAmt,Tsa_PDOTHHOLAmt,Tsa_PDRESTLEGHOLAmt,Tsa_REGOTAmt,Tsa_REGNDAmt,Tsa_REGNDOTAmt,Tsa_RESTAmt,Tsa_RESTOTAmt,Tsa_RESTNDAmt,Tsa_RESTNDOTAmt,Tsa_LEGHOLAmt,Tsa_LEGHOLOTAmt,Tsa_LEGHOLNDAmt,Tsa_LEGHOLNDOTAmt,Tsa_SPLHOLAmt,Tsa_SPLHOLOTAmt,Tsa_SPLHOLNDAmt,Tsa_SPLHOLNDOTAmt,Tsa_PSDAmt,Tsa_PSDOTAmt,Tsa_PSDNDAmt,Tsa_PSDNDOTAmt,Tsa_COMPHOLAmt,Tsa_COMPHOLOTAmt,Tsa_COMPHOLNDAmt,Tsa_COMPHOLNDOTAmt,Tsa_RESTLEGHOLAmt,Tsa_RESTLEGHOLOTAmt,Tsa_RESTLEGHOLNDAmt,Tsa_RESTLEGHOLNDOTAmt,Tsa_RESTSPLHOLAmt,Tsa_RESTSPLHOLOTAmt,Tsa_RESTSPLHOLNDAmt,Tsa_RESTSPLHOLNDOTAmt,Tsa_RESTCOMPHOLAmt,Tsa_RESTCOMPHOLOTAmt,Tsa_RESTCOMPHOLNDAmt,Tsa_RESTCOMPHOLNDOTAmt,Tsa_RESTPSDAmt,Tsa_RESTPSDOTAmt,Tsa_RESTPSDNDAmt,Tsa_RESTPSDNDOTAmt,Tsa_RGAdjAmt,Tsa_OTAdjAmt,Tsa_HOLAdjAmt,Tsa_NDAdjAmt,Tsa_LVAdjAmt,Tsa_TotalAdjAmt,Tsa_RGRetroAmt,Tsa_OTRetroAmt,Tsa_NDRetroAmt,Tsa_HLRetroAmt,Tsa_LVRetroAmt,Tsa_TotalRetroAmt,Usr_Login,Ludatetime) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}',{6},{7},'{8}',{9},{10},'{11}',{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34},{35},{36},{37},{38},{39},{40},{41},{42},{43},{44},{45},{46},{47},{48},{49},{50},{51},{52},{53},{54},{55},{56},{57},{58},{59},{60},{61},{62},{63},{64},{65},{66},{67},{68},{69},{70},{71},{72},{73},{74},{75},{76},{77},{78},{79},{80},{81},'{82}',GETDATE()) ";
                #endregion
                foreach (DataRow drSystemAdj2 in dtEmpSystemAdj2.Rows)
                {
                    //Save System Adj2 Table
                    #region System Adj2 Insert
                    strUpdateQuery += string.Format(strUpdateRecordTemplate
                                                        , drSystemAdj2["Tsa_IDNo"]                              //0
                                                        , drSystemAdj2["Tsa_RetroPayCycle"]                     //1
                                                        , drSystemAdj2["Tsa_AdjPayCycle"]                       
                                                        , drSystemAdj2["Tsa_OrigAdjPayCycle"]                   
                                                        , drSystemAdj2["Tsa_PayCycle"]                          
                                                        , drSystemAdj2["Tsa_Date"]                              //5
                                                        , drSystemAdj2["Tsa_SalaryRate"]
                                                        , drSystemAdj2["Tsa_HourRate"]
                                                        , drSystemAdj2["Tsa_PayrollType"]
                                                        , drSystemAdj2["Tsa_SpecialRate"]                       
                                                        , drSystemAdj2["Tsa_SpecialHourRate"]                   //10
                                                        , drSystemAdj2["Tsa_PremiumGrpCode"]
                                                        , drSystemAdj2["Tsa_LTAmt"]                                              
                                                        , drSystemAdj2["Tsa_UTAmt"]                                           
                                                        , drSystemAdj2["Tsa_UPLVAmt"]
                                                        , drSystemAdj2["Tsa_ABSLEGHOLAmt"]                      //15                
                                                        , drSystemAdj2["Tsa_ABSSPLHOLAmt"]
                                                        , drSystemAdj2["Tsa_ABSCOMPHOLAmt"]
                                                        , drSystemAdj2["Tsa_ABSPSDAmt"]                    
                                                        , drSystemAdj2["Tsa_ABSOTHHOLAmt"]
                                                        , drSystemAdj2["Tsa_WDABSAmt"]                          //20
                                                        , drSystemAdj2["Tsa_LTUTMaxAmt"]                          
                                                        , drSystemAdj2["Tsa_ABSAmt"]
                                                        , drSystemAdj2["Tsa_REGAmt"]
                                                        , drSystemAdj2["Tsa_PDLVAmt"]                  
                                                        , drSystemAdj2["Tsa_PDLEGHOLAmt"]                       //25
                                                        , drSystemAdj2["Tsa_PDSPLHOLAmt"]                       
                                                        , drSystemAdj2["Tsa_PDCOMPHOLAmt"]
                                                        , drSystemAdj2["Tsa_PDPSDAmt"]
                                                        , drSystemAdj2["Tsa_PDOTHHOLAmt"]                  
                                                        , drSystemAdj2["Tsa_PDRESTLEGHOLAmt"]                   //30
                                                        , drSystemAdj2["Tsa_REGOTAmt"]                          
                                                        , drSystemAdj2["Tsa_REGNDAmt"]
                                                        , drSystemAdj2["Tsa_REGNDOTAmt"]                    
                                                        , drSystemAdj2["Tsa_RESTAmt"]
                                                        , drSystemAdj2["Tsa_RESTOTAmt"]                         //35
                                                        , drSystemAdj2["Tsa_RESTNDAmt"]                         
                                                        , drSystemAdj2["Tsa_RESTNDOTAmt"]
                                                        , drSystemAdj2["Tsa_LEGHOLAmt"]                     
                                                        , drSystemAdj2["Tsa_LEGHOLOTAmt"]
                                                        , drSystemAdj2["Tsa_LEGHOLNDAmt"]                       //40
                                                        , drSystemAdj2["Tsa_LEGHOLNDOTAmt"]                     
                                                        , drSystemAdj2["Tsa_SPLHOLAmt"]
                                                        , drSystemAdj2["Tsa_SPLHOLOTAmt"]                   
                                                        , drSystemAdj2["Tsa_SPLHOLNDAmt"]
                                                        , drSystemAdj2["Tsa_SPLHOLNDOTAmt"]                     //45
                                                        , drSystemAdj2["Tsa_PSDAmt"]                            
                                                        , drSystemAdj2["Tsa_PSDOTAmt"]
                                                        , drSystemAdj2["Tsa_PSDNDAmt"]                     
                                                        , drSystemAdj2["Tsa_PSDNDOTAmt"]
                                                        , drSystemAdj2["Tsa_COMPHOLAmt"]                        //50
                                                        , drSystemAdj2["Tsa_COMPHOLOTAmt"]                      
                                                        , drSystemAdj2["Tsa_COMPHOLNDAmt"]
                                                        , drSystemAdj2["Tsa_COMPHOLNDOTAmt"]                
                                                        , drSystemAdj2["Tsa_RESTLEGHOLAmt"]
                                                        , drSystemAdj2["Tsa_RESTLEGHOLOTAmt"]                   //55
                                                        , drSystemAdj2["Tsa_RESTLEGHOLNDAmt"]                   
                                                        , drSystemAdj2["Tsa_RESTLEGHOLNDOTAmt"]
                                                        , drSystemAdj2["Tsa_RESTSPLHOLAmt"]                 
                                                        , drSystemAdj2["Tsa_RESTSPLHOLOTAmt"]
                                                        , drSystemAdj2["Tsa_RESTSPLHOLNDAmt"]                   //60
                                                        , drSystemAdj2["Tsa_RESTSPLHOLNDOTAmt"]                 
                                                        , drSystemAdj2["Tsa_RESTCOMPHOLAmt"]
                                                        , drSystemAdj2["Tsa_RESTCOMPHOLOTAmt"]              
                                                        , drSystemAdj2["Tsa_RESTCOMPHOLNDAmt"]
                                                        , drSystemAdj2["Tsa_RESTCOMPHOLNDOTAmt"]                //65
                                                        , drSystemAdj2["Tsa_RESTPSDAmt"]                        
                                                        , drSystemAdj2["Tsa_RESTPSDOTAmt"]
                                                        , drSystemAdj2["Tsa_RESTPSDNDAmt"]                  
                                                        , drSystemAdj2["Tsa_RESTPSDNDOTAmt"]
                                                        , drSystemAdj2["Tsa_RGAdjAmt"]                          //70
                                                        , drSystemAdj2["Tsa_OTAdjAmt"]                          
                                                        , drSystemAdj2["Tsa_HOLAdjAmt"]                    
                                                        , drSystemAdj2["Tsa_NDAdjAmt"]
                                                        , drSystemAdj2["Tsa_LVAdjAmt"]
                                                        , drSystemAdj2["Tsa_TotalAdjAmt"]                       //75
                                                        , drSystemAdj2["Tsa_RGRetroAmt"]                       
                                                        , drSystemAdj2["Tsa_OTRetroAmt"]                          
                                                        , drSystemAdj2["Tsa_NDRetroAmt"]
                                                        , drSystemAdj2["Tsa_HLRetroAmt"]
                                                        , drSystemAdj2["Tsa_LVRetroAmt"]                        //80
                                                        , drSystemAdj2["Tsa_TotalRetroAmt"]                    
                                                        , drSystemAdj2["Usr_Login"]);                           //82               
                    #endregion
                    iUpdateCtr++;
                    if (iUpdateCtr == 100)
                    {
                        dal.ExecuteNonQuery(strUpdateQuery);
                        strUpdateQuery = "";
                        iUpdateCtr = 0;
                    }
                }
                if (strUpdateQuery != "")
                    dal.ExecuteNonQuery(strUpdateQuery);
                //-----------------------------
                if (bHasDayCodeExt)
                {
                    #region Saving Ext
                    //Save Payroll Ext Table
                    strUpdateQuery = "";
                    iUpdateCtr = 0;
                    #region Payroll Ext Record Insert
                    strUpdateRecordTemplate = @" INSERT INTO T_EmpPayrollMiscRetro (Tpm_IDNo,Tpm_RetroPayCycle,Tpm_PayCycle,Tpm_Misc1Hr,Tpm_Misc1Amt,Tpm_Misc1VarAmt,Tpm_Misc1OTHr,Tpm_Misc1OTAmt,Tpm_Misc1OTVarAmt,Tpm_Misc1NDHr,Tpm_Misc1NDAmt,Tpm_Misc1NDVarAmt,Tpm_Misc1NDOTHr,Tpm_Misc1NDOTAmt,Tpm_Misc1NDOTVarAmt,Tpm_Misc2Hr,Tpm_Misc2Amt,Tpm_Misc2VarAmt,Tpm_Misc2OTHr,Tpm_Misc2OTAmt,Tpm_Misc2OTVarAmt,Tpm_Misc2NDHr,Tpm_Misc2NDAmt,Tpm_Misc2NDVarAmt,Tpm_Misc2NDOTHr,Tpm_Misc2NDOTAmt,Tpm_Misc2NDOTVarAmt,Tpm_Misc3Hr,Tpm_Misc3Amt,Tpm_Misc3VarAmt,Tpm_Misc3OTHr,Tpm_Misc3OTAmt,Tpm_Misc3OTVarAmt,Tpm_Misc3NDHr,Tpm_Misc3NDAmt,Tpm_Misc3NDVarAmt,Tpm_Misc3NDOTHr,Tpm_Misc3NDOTAmt,Tpm_Misc3NDOTVarAmt,Tpm_Misc4Hr,Tpm_Misc4Amt,Tpm_Misc4VarAmt,Tpm_Misc4OTHr,Tpm_Misc4OTAmt,Tpm_Misc4OTVarAmt,Tpm_Misc4NDHr,Tpm_Misc4NDAmt,Tpm_Misc4NDVarAmt,Tpm_Misc4NDOTHr,Tpm_Misc4NDOTAmt,Tpm_Misc4NDOTVarAmt,Tpm_Misc5Hr,Tpm_Misc5Amt,Tpm_Misc5VarAmt,Tpm_Misc5OTHr,Tpm_Misc5OTAmt,Tpm_Misc5OTVarAmt,Tpm_Misc5NDHr,Tpm_Misc5NDAmt,Tpm_Misc5NDVarAmt,Tpm_Misc5NDOTHr,Tpm_Misc5NDOTAmt,Tpm_Misc5NDOTVarAmt,Tpm_Misc6Hr,Tpm_Misc6Amt,Tpm_Misc6VarAmt,Tpm_Misc6OTHr,Tpm_Misc6OTAmt,Tpm_Misc6OTVarAmt,Tpm_Misc6NDHr,Tpm_Misc6NDAmt,Tpm_Misc6NDVarAmt,Tpm_Misc6NDOTHr,Tpm_Misc6NDOTAmt,Tpm_Misc6NDOTVarAmt,Usr_Login,Ludatetime) VALUES ('{0}','{1}','{2}',{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34},{35},{36},{37},{38},{39},{40},{41},{42},{43},{44},{45},{46},{47},{48},{49},{50},{51},{52},{53},{54},{55},{56},{57},{58},{59},{60},{61},{62},{63},{64},{65},{66},{67},{68},{69},{70},{71},{72},{73},{74},'{75}',GETDATE()) ";
                    #endregion
                    foreach (DataRow drPayrollCalcExt in dtEmpPayrollMisc.Rows)
                    {
                        #region Payroll Ext Insert
                        strUpdateQuery += string.Format(strUpdateRecordTemplate
                                                           , drPayrollCalcExt["Tpm_IDNo"]
                                                           , drPayrollCalcExt["Tpm_RetroPayCycle"]          //1
                                                           , drPayrollCalcExt["Tpm_PayCycle"]
                                                           , drPayrollCalcExt["Tpm_Misc1Hr"]
                                                           , drPayrollCalcExt["Tpm_Misc1Amt"]
                                                           , drPayrollCalcExt["Tpm_Misc1VarAmt"]            //5
                                                           , drPayrollCalcExt["Tpm_Misc1OTHr"]              
                                                           , drPayrollCalcExt["Tpm_Misc1OTAmt"]
                                                           , drPayrollCalcExt["Tpm_Misc1OTVarAmt"]
                                                           , drPayrollCalcExt["Tpm_Misc1NDHr"]
                                                           , drPayrollCalcExt["Tpm_Misc1NDAmt"]             //10
                                                           , drPayrollCalcExt["Tpm_Misc1NDVarAmt"]
                                                           , drPayrollCalcExt["Tpm_Misc1NDOTHr"]
                                                           , drPayrollCalcExt["Tpm_Misc1NDOTAmt"]           
                                                           , drPayrollCalcExt["Tpm_Misc1NDOTVarAmt"]
                                                           , drPayrollCalcExt["Tpm_Misc2Hr"]                //15
                                                           , drPayrollCalcExt["Tpm_Misc2Amt"]
                                                           , drPayrollCalcExt["Tpm_Misc2VarAmt"]
                                                           , drPayrollCalcExt["Tpm_Misc2OTHr"]
                                                           , drPayrollCalcExt["Tpm_Misc2OTAmt"]
                                                           , drPayrollCalcExt["Tpm_Misc2OTVarAmt"]          //20
                                                           , drPayrollCalcExt["Tpm_Misc2NDHr"]              
                                                           , drPayrollCalcExt["Tpm_Misc2NDAmt"]
                                                           , drPayrollCalcExt["Tpm_Misc2NDVarAmt"]
                                                           , drPayrollCalcExt["Tpm_Misc2NDOTHr"]
                                                           , drPayrollCalcExt["Tpm_Misc2NDOTAmt"]           //25
                                                           , drPayrollCalcExt["Tpm_Misc2NDOTVarAmt"]
                                                           , drPayrollCalcExt["Tpm_Misc3Hr"]
                                                           , drPayrollCalcExt["Tpm_Misc3Amt"]               
                                                           , drPayrollCalcExt["Tpm_Misc3VarAmt"]
                                                           , drPayrollCalcExt["Tpm_Misc3OTHr"]              //30
                                                           , drPayrollCalcExt["Tpm_Misc3OTAmt"]
                                                           , drPayrollCalcExt["Tpm_Misc3OTVarAmt"]
                                                           , drPayrollCalcExt["Tpm_Misc3NDHr"]
                                                           , drPayrollCalcExt["Tpm_Misc3NDAmt"]
                                                           , drPayrollCalcExt["Tpm_Misc3NDVarAmt"]          //35
                                                           , drPayrollCalcExt["Tpm_Misc3NDOTHr"]            
                                                           , drPayrollCalcExt["Tpm_Misc3NDOTAmt"]
                                                           , drPayrollCalcExt["Tpm_Misc3NDOTVarAmt"]
                                                           , drPayrollCalcExt["Tpm_Misc4Hr"]
                                                           , drPayrollCalcExt["Tpm_Misc4Amt"]               //40
                                                           , drPayrollCalcExt["Tpm_Misc4VarAmt"]
                                                           , drPayrollCalcExt["Tpm_Misc4OTHr"]
                                                           , drPayrollCalcExt["Tpm_Misc4OTAmt"]             
                                                           , drPayrollCalcExt["Tpm_Misc4OTVarAmt"]
                                                           , drPayrollCalcExt["Tpm_Misc4NDHr"]              //45
                                                           , drPayrollCalcExt["Tpm_Misc4NDAmt"]
                                                           , drPayrollCalcExt["Tpm_Misc4NDVarAmt"]
                                                           , drPayrollCalcExt["Tpm_Misc4NDOTHr"]
                                                           , drPayrollCalcExt["Tpm_Misc4NDOTAmt"]
                                                           , drPayrollCalcExt["Tpm_Misc4NDOTVarAmt"]        //50
                                                           , drPayrollCalcExt["Tpm_Misc5Hr"]                
                                                           , drPayrollCalcExt["Tpm_Misc5Amt"]
                                                           , drPayrollCalcExt["Tpm_Misc5VarAmt"]
                                                           , drPayrollCalcExt["Tpm_Misc5OTHr"]
                                                           , drPayrollCalcExt["Tpm_Misc5OTAmt"]             //55
                                                           , drPayrollCalcExt["Tpm_Misc5OTVarAmt"]
                                                           , drPayrollCalcExt["Tpm_Misc5NDHr"]
                                                           , drPayrollCalcExt["Tpm_Misc5NDAmt"]             
                                                           , drPayrollCalcExt["Tpm_Misc5NDVarAmt"]
                                                           , drPayrollCalcExt["Tpm_Misc5NDOTHr"]            //60
                                                           , drPayrollCalcExt["Tpm_Misc5NDOTAmt"]
                                                           , drPayrollCalcExt["Tpm_Misc5NDOTVarAmt"]
                                                           , drPayrollCalcExt["Tpm_Misc6Hr"]
                                                           , drPayrollCalcExt["Tpm_Misc6Amt"]
                                                           , drPayrollCalcExt["Tpm_Misc6VarAmt"]            //65
                                                           , drPayrollCalcExt["Tpm_Misc6OTHr"]              
                                                           , drPayrollCalcExt["Tpm_Misc6OTAmt"]
                                                           , drPayrollCalcExt["Tpm_Misc6OTVarAmt"]
                                                           , drPayrollCalcExt["Tpm_Misc6NDHr"]
                                                           , drPayrollCalcExt["Tpm_Misc6NDAmt"]             //70
                                                           , drPayrollCalcExt["Tpm_Misc6NDVarAmt"]
                                                           , drPayrollCalcExt["Tpm_Misc6NDOTHr"]
                                                           , drPayrollCalcExt["Tpm_Misc6NDOTAmt"]           
                                                           , drPayrollCalcExt["Tpm_Misc6NDOTVarAmt"]
                                                           , drPayrollCalcExt["Usr_Login"]);                //75
                        #endregion
                        iUpdateCtr++;
                        if (iUpdateCtr == 100)
                        {
                            dal.ExecuteNonQuery(strUpdateQuery);
                            strUpdateQuery = "";
                            iUpdateCtr = 0;
                        }
                    }
                    if (strUpdateQuery != "")
                        dal.ExecuteNonQuery(strUpdateQuery);
                    //-----------------------------
                    //Save Payroll Dtl Ext Table
                    strUpdateQuery = "";
                    iUpdateCtr = 0;
                    #region Payroll Dtl Ext Record Insert
                    strUpdateRecordTemplate = @" INSERT INTO T_EmpPayrollDtlMiscRetro (Tpm_IDNo,Tpm_RetroPayCycle,Tpm_PayCycle,Tpm_Date,Tpm_Misc1Hr,Tpm_Misc1Amt,Tpm_Misc1VarAmt,Tpm_Misc1OTHr,Tpm_Misc1OTAmt,Tpm_Misc1OTVarAmt,Tpm_Misc1NDHr,Tpm_Misc1NDAmt,Tpm_Misc1NDVarAmt,Tpm_Misc1NDOTHr,Tpm_Misc1NDOTAmt,Tpm_Misc1NDOTVarAmt,Tpm_Misc2Hr,Tpm_Misc2Amt,Tpm_Misc2VarAmt,Tpm_Misc2OTHr,Tpm_Misc2OTAmt,Tpm_Misc2OTVarAmt,Tpm_Misc2NDHr,Tpm_Misc2NDAmt,Tpm_Misc2NDVarAmt,Tpm_Misc2NDOTHr,Tpm_Misc2NDOTAmt,Tpm_Misc2NDOTVarAmt,Tpm_Misc3Hr,Tpm_Misc3Amt,Tpm_Misc3VarAmt,Tpm_Misc3OTHr,Tpm_Misc3OTAmt,Tpm_Misc3OTVarAmt,Tpm_Misc3NDHr,Tpm_Misc3NDAmt,Tpm_Misc3NDVarAmt,Tpm_Misc3NDOTHr,Tpm_Misc3NDOTAmt,Tpm_Misc3NDOTVarAmt,Tpm_Misc4Hr,Tpm_Misc4Amt,Tpm_Misc4VarAmt,Tpm_Misc4OTHr,Tpm_Misc4OTAmt,Tpm_Misc4OTVarAmt,Tpm_Misc4NDHr,Tpm_Misc4NDAmt,Tpm_Misc4NDVarAmt,Tpm_Misc4NDOTHr,Tpm_Misc4NDOTAmt,Tpm_Misc4NDOTVarAmt,Tpm_Misc5Hr,Tpm_Misc5Amt,Tpm_Misc5VarAmt,Tpm_Misc5OTHr,Tpm_Misc5OTAmt,Tpm_Misc5OTVarAmt,Tpm_Misc5NDHr,Tpm_Misc5NDAmt,Tpm_Misc5NDVarAmt,Tpm_Misc5NDOTHr,Tpm_Misc5NDOTAmt,Tpm_Misc5NDOTVarAmt,Tpm_Misc6Hr,Tpm_Misc6Amt,Tpm_Misc6VarAmt,Tpm_Misc6OTHr,Tpm_Misc6OTAmt,Tpm_Misc6OTVarAmt,Tpm_Misc6NDHr,Tpm_Misc6NDAmt,Tpm_Misc6NDVarAmt,Tpm_Misc6NDOTHr,Tpm_Misc6NDOTAmt,Tpm_Misc6NDOTVarAmt,Usr_Login,Ludatetime) VALUES ('{0}','{1}','{2}','{3}',{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34},{35},{36},{37},{38},{39},{40},{41},{42},{43},{44},{45},{46},{47},{48},{49},{50},{51},{52},{53},{54},{55},{56},{57},{58},{59},{60},{61},{62},{63},{64},{65},{66},{67},{68},{69},{70},{71},{72},{73},{74},{75},'{76}',GETDATE()) ";
                    #endregion
                    foreach (DataRow drPayrollCalcDtlExt in dtEmpPayrollDtlMisc.Rows)
                    {
                        #region Payroll Ext Insert
                        strUpdateQuery += string.Format(strUpdateRecordTemplate
                                                           , drPayrollCalcDtlExt["Tpm_IDNo"]                   //0
                                                           , drPayrollCalcDtlExt["Tpm_RetroPayCycle"]          //1
                                                           , drPayrollCalcDtlExt["Tpm_PayCycle"]
                                                           , drPayrollCalcDtlExt["Tpm_Date"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc1Hr"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc1Amt"]               //5
                                                           , drPayrollCalcDtlExt["Tpm_Misc1VarAmt"]            
                                                           , drPayrollCalcDtlExt["Tpm_Misc1OTHr"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc1OTAmt"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc1OTVarAmt"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc1NDHr"]               //10
                                                           , drPayrollCalcDtlExt["Tpm_Misc1NDAmt"]             
                                                           , drPayrollCalcDtlExt["Tpm_Misc1NDVarAmt"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc1NDOTHr"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc1NDOTAmt"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc1NDOTVarAmt"]         //15
                                                           , drPayrollCalcDtlExt["Tpm_Misc2Hr"]                
                                                           , drPayrollCalcDtlExt["Tpm_Misc2Amt"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc2VarAmt"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc2OTHr"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc2OTAmt"]              //20
                                                           , drPayrollCalcDtlExt["Tpm_Misc2OTVarAmt"]          
                                                           , drPayrollCalcDtlExt["Tpm_Misc2NDHr"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc2NDAmt"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc2NDVarAmt"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc2NDOTHr"]             //25
                                                           , drPayrollCalcDtlExt["Tpm_Misc2NDOTAmt"]           
                                                           , drPayrollCalcDtlExt["Tpm_Misc2NDOTVarAmt"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc3Hr"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc3Amt"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc3VarAmt"]             //30
                                                           , drPayrollCalcDtlExt["Tpm_Misc3OTHr"]              
                                                           , drPayrollCalcDtlExt["Tpm_Misc3OTAmt"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc3OTVarAmt"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc3NDHr"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc3NDAmt"]              //35
                                                           , drPayrollCalcDtlExt["Tpm_Misc3NDVarAmt"]          
                                                           , drPayrollCalcDtlExt["Tpm_Misc3NDOTHr"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc3NDOTAmt"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc3NDOTVarAmt"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc4Hr"]                 //40
                                                           , drPayrollCalcDtlExt["Tpm_Misc4Amt"]               
                                                           , drPayrollCalcDtlExt["Tpm_Misc4VarAmt"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc4OTHr"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc4OTAmt"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc4OTVarAmt"]           //45
                                                           , drPayrollCalcDtlExt["Tpm_Misc4NDHr"]              
                                                           , drPayrollCalcDtlExt["Tpm_Misc4NDAmt"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc4NDVarAmt"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc4NDOTHr"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc4NDOTAmt"]            //50
                                                           , drPayrollCalcDtlExt["Tpm_Misc4NDOTVarAmt"]        
                                                           , drPayrollCalcDtlExt["Tpm_Misc5Hr"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc5Amt"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc5VarAmt"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc5OTHr"]               //55
                                                           , drPayrollCalcDtlExt["Tpm_Misc5OTAmt"]             
                                                           , drPayrollCalcDtlExt["Tpm_Misc5OTVarAmt"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc5NDHr"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc5NDAmt"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc5NDVarAmt"]           //60
                                                           , drPayrollCalcDtlExt["Tpm_Misc5NDOTHr"]            
                                                           , drPayrollCalcDtlExt["Tpm_Misc5NDOTAmt"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc5NDOTVarAmt"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc6Hr"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc6Amt"]                //65
                                                           , drPayrollCalcDtlExt["Tpm_Misc6VarAmt"]            
                                                           , drPayrollCalcDtlExt["Tpm_Misc6OTHr"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc6OTAmt"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc6OTVarAmt"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc6NDHr"]               //70
                                                           , drPayrollCalcDtlExt["Tpm_Misc6NDAmt"]             
                                                           , drPayrollCalcDtlExt["Tpm_Misc6NDVarAmt"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc6NDOTHr"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc6NDOTAmt"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc6NDOTVarAmt"]         //75
                                                           , drPayrollCalcDtlExt["Usr_Login"]);                          
                        #endregion
                        iUpdateCtr++;
                        if (iUpdateCtr == 100)
                        {
                            dal.ExecuteNonQuery(strUpdateQuery);
                            strUpdateQuery = "";
                            iUpdateCtr = 0;
                        }
                    }
                    if (strUpdateQuery != "")
                        dal.ExecuteNonQuery(strUpdateQuery);
                    //-----------------------------
                    //Save System Adj2 Ext Table
                    strUpdateQuery = "";
                    iUpdateCtr = 0;
                    #region System Adj2 Ext Record Insert
                    strUpdateRecordTemplate = @" INSERT INTO T_EmpSystemAdjMisc2Retro (Tsm_IDNo,Tsm_RetroPayCycle,Tsm_AdjPayCycle,Tsm_OrigAdjPayCycle,Tsm_PayCycle,Tsm_Date,Tsm_Misc1Amt,Tsm_Misc1OTAmt,Tsm_Misc1NDAmt,Tsm_Misc1NDOTAmt,Tsm_Misc2Amt,Tsm_Misc2OTAmt,Tsm_Misc2NDAmt,Tsm_Misc2NDOTAmt,Tsm_Misc3Amt,Tsm_Misc3OTAmt,Tsm_Misc3NDAmt,Tsm_Misc3NDOTAmt,Tsm_Misc4Amt,Tsm_Misc4OTAmt,Tsm_Misc4NDAmt,Tsm_Misc4NDOTAmt,Tsm_Misc5Amt,Tsm_Misc5OTAmt,Tsm_Misc5NDAmt,Tsm_Misc5NDOTAmt,Tsm_Misc6Amt,Tsm_Misc6OTAmt,Tsm_Misc6NDAmt,Tsm_Misc6NDOTAmt,Usr_Login,Ludatetime) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}',{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},'{30}',GETDATE()) ";
                    #endregion
                    foreach (DataRow drSystemAdj2Ext in dtEmpSystemAdj2Misc.Rows)
                    {
                        #region System Adj2 Ext Insert
                        strUpdateQuery += string.Format(strUpdateRecordTemplate
                                                           , drSystemAdj2Ext["Tsm_IDNo"]                                //0
                                                           , drSystemAdj2Ext["Tsm_RetroPayCycle"]                       //1
                                                           , drSystemAdj2Ext["Tsm_AdjPayCycle"]
                                                           , drSystemAdj2Ext["Tsm_OrigAdjPayCycle"]
                                                           , drSystemAdj2Ext["Tsm_PayCycle"]
                                                           , drSystemAdj2Ext["Tsm_Date"]                                //5
                                                           , drSystemAdj2Ext["Tsm_Misc1Amt"]
                                                           , drSystemAdj2Ext["Tsm_Misc1OTAmt"]                            
                                                           , drSystemAdj2Ext["Tsm_Misc1NDAmt"]
                                                           , drSystemAdj2Ext["Tsm_Misc1NDOTAmt"]
                                                           , drSystemAdj2Ext["Tsm_Misc2Amt"]                            //10
                                                           , drSystemAdj2Ext["Tsm_Misc2OTAmt"]
                                                           , drSystemAdj2Ext["Tsm_Misc2NDAmt"]                         
                                                           , drSystemAdj2Ext["Tsm_Misc2NDOTAmt"]
                                                           , drSystemAdj2Ext["Tsm_Misc3Amt"]
                                                           , drSystemAdj2Ext["Tsm_Misc3OTAmt"]                          //15
                                                           , drSystemAdj2Ext["Tsm_Misc3NDAmt"]
                                                           , drSystemAdj2Ext["Tsm_Misc3NDOTAmt"]                          
                                                           , drSystemAdj2Ext["Tsm_Misc4Amt"]
                                                           , drSystemAdj2Ext["Tsm_Misc4OTAmt"]
                                                           , drSystemAdj2Ext["Tsm_Misc4NDAmt"]                          //20
                                                           , drSystemAdj2Ext["Tsm_Misc4NDOTAmt"]
                                                           , drSystemAdj2Ext["Tsm_Misc5Amt"]                             
                                                           , drSystemAdj2Ext["Tsm_Misc5OTAmt"]
                                                           , drSystemAdj2Ext["Tsm_Misc5NDAmt"]
                                                           , drSystemAdj2Ext["Tsm_Misc5NDOTAmt"]                        //25
                                                           , drSystemAdj2Ext["Tsm_Misc6Amt"]
                                                           , drSystemAdj2Ext["Tsm_Misc6OTAmt"]                          
                                                           , drSystemAdj2Ext["Tsm_Misc6NDAmt"]
                                                           , drSystemAdj2Ext["Tsm_Misc6NDOTAmt"]
                                                           , drSystemAdj2Ext["Usr_Login"]);                             //30
                        #endregion
                        iUpdateCtr++;
                        if (iUpdateCtr == 100)
                        {
                            dal.ExecuteNonQuery(strUpdateQuery);
                            strUpdateQuery = "";
                            iUpdateCtr = 0;
                        }
                    }
                    if (strUpdateQuery != "")
                        dal.ExecuteNonQuery(strUpdateQuery);
                    #endregion
                }
                StatusHandler(this, new StatusEventArgs("Saving Retro Pay Records", true));
                //-----------------------------
            }
            catch (Exception ex)
            {
                //dal.RollBackTransactionSnapshot();
                CommonProcedures.logErrorToFile(ex.ToString());
                throw new Exception("Retro Pay Calculation has encountered some errors: \n" + ex.Message);
            }
            finally
            {
                //dal.CloseDB();
            }

            return new DataTable();
        }

        public string SetParameters(string companyCode, string centralProfile, DALHelper dalHelper)
        {
            CompanyCode = companyCode;
            CentralProfile = centralProfile;
            this.dal = dalHelper;
            this.commonBL = new CommonBL();
            string setupErrorMessages = SetParameters();
            return setupErrorMessages;
        }

        public string SetParameters()
        {
            string ProcessingErrorMessage = "";
            string strResult = string.Empty;
            #region Parameters
            StatusHandler(this, new StatusEventArgs("Loading Parameters", false));
            StatusHandler(this, new StatusEventArgs("Loading Parameters", true));

            strResult = commonBL.GetParameterValueFromPayroll("MDIVISOR", CompanyCode, dal);
            MDIVISOR = strResult.Equals(string.Empty) ? 0 : Convert.ToDecimal(strResult);
            StatusHandler(this, new StatusEventArgs(string.Format(" MDIVISOR = {0} ", MDIVISOR), false));
            StatusHandler(this, new StatusEventArgs(string.Format(" MDIVISOR = {0} ", MDIVISOR), true));

            strResult = commonBL.GetParameterValueFromPayroll("HRLYRTEDEC", CompanyCode, dal);
            HRLYRTEDEC = strResult.Equals(string.Empty) ? 0 : Convert.ToDecimal(strResult);
            StatusHandler(this, new StatusEventArgs(string.Format(" HRLYRTEDEC = {0} ", HRLYRTEDEC), false));
            StatusHandler(this, new StatusEventArgs(string.Format(" HRLYRTEDEC = {0} ", HRLYRTEDEC), true));

            strResult = commonBL.GetParameterValueFromPayroll("NDBRCKTCNT", CompanyCode, dal);
            NDBRCKTCNT = strResult.Equals(string.Empty) ? 1 : Convert.ToInt32(Convert.ToDouble(strResult));
            StatusHandler(this, new StatusEventArgs(string.Format(" NDBRCKTCNT = {0} ", NDBRCKTCNT), false));
            StatusHandler(this, new StatusEventArgs(string.Format(" NDBRCKTCNT = {0} ", NDBRCKTCNT), true));

            strResult = commonBL.GetParameterValueFromCentral("LCSFORCEBAL", CompanyCode, CentralProfile, dal);
            LCSFORCEBAL = strResult.Equals(string.Empty) ? 0 : Convert.ToDecimal(strResult);
            StatusHandler(this, new StatusEventArgs(string.Format("  LCSFORCEBAL = {0}", LCSFORCEBAL), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  LCSFORCEBAL = {0}", LCSFORCEBAL), true));

            MULTSAL = commonBL.GetParameterValueFromPayroll("MULTSAL", CompanyCode, dal);
            StatusHandler(this, new StatusEventArgs(string.Format(" MULTSAL = {0} ", MULTSAL), false));
            StatusHandler(this, new StatusEventArgs(string.Format(" MULTSAL = {0} ", MULTSAL), true));

            PAYSPECIALRATE = commonBL.GetParameterValueFromPayroll("PAYSPECIALRATE", CompanyCode, dal);
            StatusHandler(this, new StatusEventArgs(string.Format(" PAYSPECIALRATE = {0} ", PAYSPECIALRATE), false));
            StatusHandler(this, new StatusEventArgs(string.Format(" PAYSPECIALRATE = {0} ", PAYSPECIALRATE), true));

            strResult = commonBL.GetParameterDtlValueFromPayroll("NDBRACKET", "NP1_RATE", CompanyCode, dal);
            NP1_RATE = strResult.Equals(string.Empty) ? 0 : Convert.ToDecimal(strResult) / 100;
            StatusHandler(this, new StatusEventArgs(string.Format("NDBRACKET = {0}", NP1_RATE), false));
            StatusHandler(this, new StatusEventArgs(string.Format("NDBRACKET = {0}", NP1_RATE), true));

            strResult = commonBL.GetParameterDtlValueFromPayroll("NDBRACKET", "NP2_RATE", CompanyCode, dal);
            NP2_RATE = strResult.Equals(string.Empty) ? 0 : Convert.ToDecimal(strResult) / 100;
            StatusHandler(this, new StatusEventArgs(string.Format("NDBRACKET = {0}", NP2_RATE), false));
            StatusHandler(this, new StatusEventArgs(string.Format("NDBRACKET = {0}", NP2_RATE), true));

            LATEBRCKTQ = GetBracketParameter("LATEBRCKTQ", dal);
            StatusHandler(this, new StatusEventArgs(string.Format("Getting late bracket deduction"), false));
            StatusHandler(this, new StatusEventArgs(string.Format("Getting late bracket deduction"), true));

            MULTSALMD = commonBL.GetParameterDtlValueFromPayroll("REGPAYRULE", "MULTSALMD", CompanyCode, dal);
            if (MULTSALMD == "")
                ProcessingErrorMessage += "Multiple Salary from Monthlies to Dailies is not set-up in Master.\n";
            StatusHandler(this, new StatusEventArgs(string.Format("Getting Multiple Salary from Monthlies to Dailies set-up"), false));
            StatusHandler(this, new StatusEventArgs(string.Format("Getting Multiple Salary from Monthlies to Dailies set-up"), true));

            MULTSALDM = commonBL.GetParameterDtlValueFromPayroll("REGPAYRULE", "MULTSALDM", CompanyCode, dal);
            if (MULTSALDM == "")
                ProcessingErrorMessage += "Multiple Salary from Dailies to Monthlies is not set-up in Master.\n";
            StatusHandler(this, new StatusEventArgs(string.Format("Getting Multiple Salary from Dailies to Monthlies set-up"), false));
            StatusHandler(this, new StatusEventArgs(string.Format("Getting Multiple Salary from Dailies to Monthlies set-up"), true));

            NEWHIRE = commonBL.GetParameterDtlValueFromPayroll("REGPAYRULE", "NEWHIRE", CompanyCode, dal);
            if (NEWHIRE == "")
                ProcessingErrorMessage += "Newly-Hired Monthlies Employees is not set-up in Master.\n";
            StatusHandler(this, new StatusEventArgs(string.Format("Getting Newly-Hired Monthlies Employees set-up"), false));
            StatusHandler(this, new StatusEventArgs(string.Format("Getting Newly-Hired Monthlies Employees set-up"), true));

            if (Convert.ToBoolean(PAYSPECIALRATE))
            {
                PAYCODERATE = commonBL.GetParameterDtlListfromPayroll("PAYCODERATE", CompanyCode, dal);
                if (PAYCODERATE.Rows.Count == 0)
                    ProcessingErrorMessage += "Special Pay Code Rate is not set-up in Master.\n";
                StatusHandler(this, new StatusEventArgs(string.Format("Getting special rate set-up"), false));
                StatusHandler(this, new StatusEventArgs(string.Format("Getting special rate set-up"), true));
                GetSpecialRate();

                if (commonBL.GetFillerDayCodesCount(CompanyCode, CentralProfile, dal) > 0)
                {
                    #region Misc Special Rate
                    DataTable dtMiscDay = dal.ExecuteDataSet(string.Format(@"SELECT Mmd_MiscDayID, Mmd_DayCode 
								                                            FROM {0}..M_MiscellaneousDay 
								                                            WHERE Mmd_CompanyCode = '{1}'
									                                            AND Mmd_RecordStatus = 'A'", CentralProfile, CompanyCode)
                                                             ).Tables[0];
                    for (int idx = 0; idx < dtMiscDay.Rows.Count; idx++)
                    {
                        DataRow[] drRow = PAYCODERATE.Select("Mpd_SubCode = '" + dtMiscDay.Rows[idx][1].ToString() + "'");
                        if (drRow.Length > 0)
                        {
                            switch (dtMiscDay.Rows[idx][0].ToString())
                            {
                                case "1":
                                    Misc1Rate = drRow[0][1].ToString();
                                    break;
                                case "2":
                                    Misc2Rate = drRow[0][1].ToString();
                                    break;
                                case "3":
                                    Misc3Rate = drRow[0][1].ToString();
                                    break;
                                case "4":
                                    Misc4Rate = drRow[0][1].ToString();
                                    break;
                                case "5":
                                    Misc5Rate = drRow[0][1].ToString();
                                    break;
                                case "6":
                                    Misc6Rate = drRow[0][1].ToString();
                                    break;
                            }
                        }
                    }
                    #endregion
                }

                SPECIALRATEFORMULA_SRATE = commonBL.GetParameterFormulaFromPayroll("SPECIALRATEFRM-SRATE", CompanyCode, dal);
                if (SPECIALRATEFORMULA_SRATE == "")
                    ProcessingErrorMessage += "Special Salary Rate is not set-up in Master.\n";
                StatusHandler(this, new StatusEventArgs(string.Format("Getting special salary rate set-up"), false));
                StatusHandler(this, new StatusEventArgs(string.Format("Getting special salary rate set-up"), true));

                SPECIALRATEFORMULA_HRATE = commonBL.GetParameterFormulaFromPayroll("SPECIALRATEFRM-HRATE", CompanyCode, dal);
                if (SPECIALRATEFORMULA_HRATE == "")
                    ProcessingErrorMessage += "Special Hourly Rate is not set-up in Master.\n";
                StatusHandler(this, new StatusEventArgs(string.Format("Getting special hourly rate set-up"), false));
                StatusHandler(this, new StatusEventArgs(string.Format("Getting special hourly rate set-up"), true));
            }

            #endregion
            return ProcessingErrorMessage;
        }

        private void GetSpecialRate()
        {
            for (int idx = 0; idx < PAYCODERATE.Rows.Count; idx++)
            {
                switch (PAYCODERATE.Rows[idx][0].ToString())
                {
                    case "LT":
                        LTRate = PAYCODERATE.Rows[idx][1].ToString();
                        break;
                    case "UT":
                        UTRate = PAYCODERATE.Rows[idx][1].ToString();
                        break;
                    case "UPLV":
                        UPLVRate = PAYCODERATE.Rows[idx][1].ToString();
                        break;
                    case "WDABS":
                        WDABSRate = PAYCODERATE.Rows[idx][1].ToString();
                        break;
                    case "LTUTMAX":
                        LTUTMAXRate = PAYCODERATE.Rows[idx][1].ToString();
                        break;
                    case "ABSLEGHOL":
                        ABSLEGHOLRate = PAYCODERATE.Rows[idx][1].ToString();
                        break;
                    case "ABSSPLHOL":
                        ABSSPLHOLRate = PAYCODERATE.Rows[idx][1].ToString();
                        break;
                    case "ABSCOMPHOL":
                        ABSCOMPHOLRate = PAYCODERATE.Rows[idx][1].ToString();
                        break;
                    case "ABSPSD":
                        ABSPSDRate = PAYCODERATE.Rows[idx][1].ToString();
                        break;
                    case "ABSOTHHOL":
                        ABSOTHHOLRate = PAYCODERATE.Rows[idx][1].ToString();
                        break;

                    case "REG":
                        REGRate = PAYCODERATE.Rows[idx][1].ToString();
                        break;
                    case "PDLV":
                        PDLVRate = PAYCODERATE.Rows[idx][1].ToString();
                        break;
                    case "PDLEGHOL":
                        PDLEGHOLRate = PAYCODERATE.Rows[idx][1].ToString();
                        break;
                    case "PDSPLHOL":
                        PDSPLHOLRate = PAYCODERATE.Rows[idx][1].ToString();
                        break;
                    case "PDCOMPHOL":
                        PDCOMPHOLRate = PAYCODERATE.Rows[idx][1].ToString();
                        break;
                    case "PDPSD":
                        PDPSDRate = PAYCODERATE.Rows[idx][1].ToString();
                        break;
                    case "PDOTHHOL":
                        PDOTHHOLRate = PAYCODERATE.Rows[idx][1].ToString();
                        break;
                    case "PDRESTLEGHOL":
                        PDRESTLEGHOLRate = PAYCODERATE.Rows[idx][1].ToString();
                        break;

                    case "REST":
                        RESTRate = PAYCODERATE.Rows[idx][1].ToString();
                        break;
                    case "LEGHOL":
                        LEGHOLRate = PAYCODERATE.Rows[idx][1].ToString();
                        break;
                    case "SPLHOL":
                        SPLHOLRate = PAYCODERATE.Rows[idx][1].ToString();
                        break;
                    case "PSD":
                        PSDRate = PAYCODERATE.Rows[idx][1].ToString();
                        break;
                    case "COMPHOL":
                        COMPHOLRate = PAYCODERATE.Rows[idx][1].ToString();
                        break;
                    case "RESTLEGHOL":
                        RESTLEGHOLRate = PAYCODERATE.Rows[idx][1].ToString();
                        break;
                    case "RESTSPLHOL":
                        RESTSPLHOLRate = PAYCODERATE.Rows[idx][1].ToString();
                        break;
                    case "RESTPSD":
                        RESTPSDRate = PAYCODERATE.Rows[idx][1].ToString();
                        break;
                    case "RESTCOMPHOL":
                        RESTCOMPHOLRate = PAYCODERATE.Rows[idx][1].ToString();
                        break;
                }
            }
        }

        private DataTable GetBracketParameter(string ParameterID, DALHelper dalHelper)
        {
            #region query
            string qString = string.Format(@" SELECT CONVERT(DECIMAL(4,3), Mpd_SubCode) AS Mpd_SubCode, Mpd_ParamValue 
                                                FROM M_PolicyDtl
                                                WHERE Mpd_PolicyCode = '{0}'
                                                    AND Mpd_CompanyCode = '{1}'
	                                                AND Mpd_RecordStatus = 'A'
                                                ORDER BY 1", ParameterID, CompanyCode);
            #endregion
            DataSet ds = new DataSet();
            ds = dalHelper.ExecuteDataSet(qString);

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0];
            else
                return new DataTable();
        }

        //private bool CheckIfHasSpecialRate(string DayCode)
        //{
        //    bool bSpecialRate = false;
        //    DataRow[] drSpecialPayRate = PAYCODERATE.Select(string.Format("Mpd_SubCode = '{0}'", DayCode));
        //    if (drSpecialPayRate.Length > 0 && drSpecialPayRate[0]["Mpd_ParamValue"].ToString() == "S")
        //        bSpecialRate = true;
        //    return bSpecialRate;
        //}

        //private bool CheckIfHasSpecialRate(DataTable dtSpecialRate, string DayCode)
        //{
        //    bool bSpecialRate = false;
        //    DataRow[] drSpecialPayRate = dtSpecialRate.Select(string.Format("[DayCode] = '{0}'", DayCode));
        //    if (drSpecialPayRate.Length > 0 && drSpecialPayRate[0]["SpecialRate"].ToString() == "S")
        //        bSpecialRate = true;
        //    return bSpecialRate;
        //}

        //public DataTable GetPayCodeRateOvertime(bool bMiscellaneous, string PayrollType, string PremiumGroup)
        //{
        //    string condition = string.Empty;
        //    if (bMiscellaneous)
        //        condition = "AND ISNULL(Mmd_MiscDayID,0) > 0";
        //    else condition = "AND ISNULL(Mmd_MiscDayID,0) = 0";

        //    string query = string.Format(@"
        //                            SELECT CASE WHEN Mdp_RestDayFlag = 1 and Mdp_DayCode = 'REST' then ''
        //                                WHEN Mdp_RestDayFlag = 1 and Mdp_DayCode <> 'REST' then 'REST'
        //                                ELSE '' END +   
        //                                CASE 
        //                                    WHEN Mdp_DayCode = 'HOL' then 'LEGHOL'
        //                                    WHEN Mdp_DayCode =  'SPL' then 'SPLHOL'
        //                                    WHEN Mdp_DayCode =  'COMP' then 'COMPHOL'
        //                                ELSE  Mdp_DayCode END as [DayCode]
        //	    , Mpd_ParamValue as [SpecialRate]
        //                             FROM {0}..M_DayPremium
        //	 INNER JOIN M_PolicyDtl
        //			ON RTRIM(Mpd_PolicyCode) = 'PAYCODERATE'
        //			AND RTRIM(Mpd_SubCode) = Mdp_DayCode
        //			AND RTRIM(Mpd_CompanyCode) = '{1}'
        //                             LEFT JOIN {0}..M_Day ON Mdp_DayCode = Mdy_DayCode
        //                                    AND Mdp_CompanyCode = Mdy_CompanyCode
        //                                    AND Mdy_RecordStatus = 'A'
        //                             LEFT JOIN {0}..M_MiscellaneousDay
        //                                    ON Mmd_DayCode = Mdp_DayCode
        //                                    AND Mmd_RestDayFlag = Mdp_RestDayFlag
        //                                    AND Mmd_CompanyCode = Mdp_CompanyCode
        //                                    AND Mmd_RecordStatus = 'A'
        //       WHERE Mdp_PayrollType = '{2}' 
        //           AND Mdp_RecordStatus = 'A'
        //                                AND Mdp_CompanyCode = '{1}'
        //                                AND Mdp_PremiumGrpCode = '{3}'
        //           {4}
        //                            ORDER BY Mdp_SequenceOfDisplay"
        //                            , CentralProfile
        //                            , CompanyCode
        //                            , PayrollType
        //                            , PremiumGroup
        //                            , condition);

        //    DataTable dtResult;
        //    dtResult = dal.ExecuteDataSet(query).Tables[0];

        //    return dtResult;
        //}
        public void GetPremium(DataTable dtDayCodePremiums, string PayrollType, string PremiumGroup, string DayCode, bool isRestDay, ref decimal RG, ref decimal OT, ref decimal ND, ref decimal OTND, ref decimal RGNDPercent, ref decimal OTNDPercent)
        {
            DataRow[] drDayCodePremium = dtDayCodePremiums.Select(string.Format("Mdp_PremiumGrpCode = '{0}' AND Mdp_PayrollType = '{1}' AND Mdp_DayCode = '{2}' AND Mdp_RestDayFlag = {3}", PremiumGroup, PayrollType, DayCode, isRestDay));
            if (drDayCodePremium.Length > 0)
            {
                RG = Convert.ToDecimal(drDayCodePremium[0]["Mdp_RGPremiumRate"]) / 100;
                OT = Convert.ToDecimal(drDayCodePremium[0]["Mdp_OTPremiumRate"]) / 100;
                ND = Convert.ToDecimal(drDayCodePremium[0]["Mdp_RGNDPremiumRate"]) / 100;
                OTND = Convert.ToDecimal(drDayCodePremium[0]["Mdp_OTNDPremiumRate"]) / 100;
                RGNDPercent = Convert.ToDecimal(drDayCodePremium[0]["Mdp_RGNDPercentageRate"]) / 100;
                OTNDPercent = Convert.ToDecimal(drDayCodePremium[0]["Mdp_OTNDPercentageRate"]) / 100;

            }
            else
            {
                drDayCodePremium = dtDayCodePremiums.Select(string.Format("Mdp_PremiumGrpCode = '{0}' AND Mdp_PayrollType = '{1}' AND Mdp_DayCode = '{2}' AND Mdp_RestDayFlag = {3}", "DEFAULTGRP", PayrollType, DayCode, isRestDay));
                if (drDayCodePremium.Length > 0)
                {
                    RG = Convert.ToDecimal(drDayCodePremium[0]["Mdp_RGPremiumRate"]) / 100;
                    OT = Convert.ToDecimal(drDayCodePremium[0]["Mdp_OTPremiumRate"]) / 100;
                    ND = Convert.ToDecimal(drDayCodePremium[0]["Mdp_RGNDPremiumRate"]) / 100;
                    OTND = Convert.ToDecimal(drDayCodePremium[0]["Mdp_OTNDPremiumRate"]) / 100;
                    RGNDPercent = Convert.ToDecimal(drDayCodePremium[0]["Mdp_RGNDPercentageRate"]) / 100;
                    OTNDPercent = Convert.ToDecimal(drDayCodePremium[0]["Mdp_OTNDPercentageRate"]) / 100;
                }
                else
                    throw new PayrollException("Day Premium Group [" + PremiumGroup + "] has No Premium for " + DayCode + " Day Code");
            }
        }

        private decimal GetHourlyRate(decimal SalaryRate, string PayrollType, decimal mdivisor)
        {
            decimal HourlyRate = 0;

            if (PayrollType == "D")
                HourlyRate = SalaryRate / CommonBL.HOURSINDAY;
            else if (PayrollType == "M")
                HourlyRate = SalaryRate * 12 / mdivisor / CommonBL.HOURSINDAY;

            if (HRLYRTEDEC > 0)
            {
                if (PayrollType == "D")
                    HourlyRate = Math.Round(SalaryRate / CommonBL.HOURSINDAY, Convert.ToInt32(HRLYRTEDEC), MidpointRounding.AwayFromZero);
                else if (PayrollType == "M")
                    HourlyRate = Math.Round(SalaryRate * 12 / mdivisor / CommonBL.HOURSINDAY, Convert.ToInt32(HRLYRTEDEC), MidpointRounding.AwayFromZero);
            }
            return HourlyRate;
        }

        public DataTable GetEmployeeSalaryinPayPeriod(string EmployeeId, string AdjustPayCycleStart, string AdjustPayCycleEnd)
        {
            #region query
            string query = @"SELECT a.Tsl_IDNo
	                            , ROW_NUMBER() OVER (PARTITION BY a.Tsl_IDNo ORDER BY a.Tsl_Startdate) AS [Ctr]
	                            , CASE WHEN a.Tsl_StartDate < '{2}' THEN '{2}' ELSE a.Tsl_StartDate END AS Tsl_StartDate
	                            , CASE WHEN ISNULL(a.Tsl_EndDate,'{3}') >= '{3}' THEN '{3}' ELSE a.Tsl_EndDate END AS Tsl_EndDate
	                            , a.Tsl_SalaryRate
	                            , a.Tsl_PayrollType
                            FROM {0}..T_EmpSalary a
                            INNER JOIN (SELECT Tsl_IDNo
			                            FROM {0}..T_EmpSalary 
			                            INNER JOIN M_Employee ON Mem_IDNo = Tsl_IDNo
			                            WHERE  ISNULL(Tsl_EndDate, '{3}') >= '{2}' AND '{3}' >= Tsl_StartDate
                                            AND Tsl_IDNo = '{4}'
			                            GROUP BY Tsl_IDNo
			                            HAVING COUNT(Tsl_IDNo) > 1 ) EmpSalary ON a.Tsl_IDNo = EmpSalary.Tsl_IDNo
                            WHERE ISNULL(a.Tsl_EndDate,'{3}') >= '{2}' AND '{3}' >= a.Tsl_StartDate
                                AND a.Tsl_IDNo = '{4}'";
            #endregion
            query = string.Format(query, CentralProfile, CompanyCode, AdjustPayCycleStart, AdjustPayCycleEnd, EmployeeId);
            DataTable dtResult;
            dtResult = dal.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public DataTable GetComputedDailyinCurrentPayPeriod(string AdjustPayCycleStart, string AdjustPayCycleEnd)
        {
            #region query
            string qString = string.Format(@"SELECT Mem_IDNo, Mem_IntakeDate 
                                             FROM M_Employee
                                             WHERE Mem_IsComputedPerDay = 1 
                                                    ---(Mem_IntakeDate BETWEEN '{0}' AND '{1}')
                                                    ", AdjustPayCycleStart, AdjustPayCycleEnd);
            #endregion
            DataTable dtResult = dal.ExecuteDataSet(qString).Tables[0];
            return dtResult;
        }

        public DataTable GetNewlyHiredinCurrentPayPeriod(string AdjustPayCycleStart, string AdjustPayCycleEnd)
        {
            #region query
            string qString = string.Format(@"SELECT Mem_IDNo, Mem_IntakeDate 
                                             FROM M_Employee
                                             WHERE Mem_IntakeDate BETWEEN '{0}' AND '{1}'
                                                AND Mem_PayrollType = 'M'
                                                    ", AdjustPayCycleStart, AdjustPayCycleEnd);
            #endregion
            DataTable dtResult = dal.ExecuteDataSet(qString).Tables[0];
            return dtResult;
        }

        public DataTable GetAllEmployeeRetroPayForProcess(bool ProcessAll, string EmployeeId, string EmployeeList, DALHelper dal)
        {
            string EmployeeCondition = "";
            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition += " AND Ter_IDNo = '" + EmployeeId + "' ";
            else if (ProcessAll && EmployeeList != "")
                EmployeeCondition += " AND Ter_IDNo IN (" + EmployeeList + ") ";

            #region query
            string query = string.Format(@"SELECT Ter_IDNo 
                                                ,Mem_LastName
                                                ,Mem_FirstName
                                                ,Mem_MiddleName
                                                ,Mem_EmploymentStatusCode
                                                ,Mem_PremiumGrpCode
                                                ,Ter_SalaryDate
                                                ,Ter_SalaryRate
                                                ,Ter_PayrollType
                                                ,Ter_RGRetroAmt
                                                ,Ter_OTRetroAmt
                                                ,Ter_NDRetroAmt
                                                ,Ter_HLRetroAmt
                                                ,Ter_LVRetroAmt
                                                ,Ter_TotalRetroAmt
                                                ,Ter_AdjustPayCycles
                                                ,Ter_PostFlag
                                                FROM T_EmpRetroPay
                                                INNER JOIN M_Employee ON Ter_IDNo = Mem_IDNo  
                                                WHERE Ter_PayCycle = '{0}'
                                                {1}
                                                ", ProcessPayrollPeriod
                                                , EmployeeCondition);
            #endregion
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public DataTable GetAllEmployeePayrollDtlForProcess(string EmployeeId, string AfftectedPayCycle, object SalaryDate, string AdjustPayCycleStart, string AdjustPayCycleEnd)
        {
            #region query
            string query = string.Format(@"SELECT Tpd_IDNo
                                            ,Tpd_PayCycle
                                            ,Tpd_Date
                                            ,Tpd_DayCode
                                            ,Tpd_RestDayFlag
                                            ,Tpd_PremiumGrpCode
                                            ,ISNULL(ISNULL(A.Tsl_SalaryRate,B.Tsl_SalaryRate), 0) AS Tpd_SalaryRate
                                            ,ISNULL(ISNULL(A.Tsl_PayrollType, B.Tsl_PayrollType), '') AS Tpd_PayrollType
                                            ,Tpd_HourRate
                                            ,Tpd_SpecialRate
                                            ,Tpd_SpecialHourRate  
                                            ,Tpd_WorkDay 
                                            ,Tpd_LTHr
                                            ,Tpd_LTAmt
                                            ,Tpd_UTHr
                                            ,Tpd_UTAmt
                                            ,Tpd_UPLVHr
                                            ,Tpd_UPLVAmt
                                            ,Tpd_ABSLEGHOLHr
                                            ,Tpd_ABSLEGHOLAmt
                                            ,Tpd_ABSSPLHOLHr
                                            ,Tpd_ABSSPLHOLAmt
                                            ,Tpd_ABSCOMPHOLHr
                                            ,Tpd_ABSCOMPHOLAmt
                                            ,Tpd_ABSPSDHr
                                            ,Tpd_ABSPSDAmt
                                            ,Tpd_ABSOTHHOLHr
                                            ,Tpd_ABSOTHHOLAmt
                                            ,Tpd_WDABSHr
                                            ,Tpd_WDABSAmt
                                            ,Tpd_LTUTMaxHr
                                            ,Tpd_LTUTMaxAmt
                                            ,Tpd_ABSHr
                                            ,Tpd_ABSAmt
                                            ,Tpd_REGHr
                                            ,Tpd_REGAmt
                                            ,Tpd_PDLVHr
                                            ,Tpd_PDLVAmt
                                            ,Tpd_PDLEGHOLHr
                                            ,Tpd_PDLEGHOLAmt
                                            ,Tpd_PDSPLHOLHr
                                            ,Tpd_PDSPLHOLAmt
                                            ,Tpd_PDCOMPHOLHr
                                            ,Tpd_PDCOMPHOLAmt
                                            ,Tpd_PDPSDHr
                                            ,Tpd_PDPSDAmt
                                            ,Tpd_PDOTHHOLHr
                                            ,Tpd_PDOTHHOLAmt
                                            ,Tpd_PDRESTLEGHOLHr
                                            ,Tpd_PDRESTLEGHOLAmt
                                            ,Tpd_TotalREGHr
                                            ,Tpd_TotalREGAmt
                                            ,Tpd_REGOTHr
                                            ,Tpd_REGOTAmt
                                            ,Tpd_REGNDHr
                                            ,Tpd_REGNDAmt
                                            ,Tpd_REGNDOTHr
                                            ,Tpd_REGNDOTAmt
                                            ,Tpd_RESTHr
                                            ,Tpd_RESTAmt
                                            ,Tpd_RESTOTHr
                                            ,Tpd_RESTOTAmt
                                            ,Tpd_RESTNDHr
                                            ,Tpd_RESTNDAmt
                                            ,Tpd_RESTNDOTHr
                                            ,Tpd_RESTNDOTAmt
                                            ,Tpd_LEGHOLHr
                                            ,Tpd_LEGHOLAmt
                                            ,Tpd_LEGHOLOTHr
                                            ,Tpd_LEGHOLOTAmt
                                            ,Tpd_LEGHOLNDHr
                                            ,Tpd_LEGHOLNDAmt
                                            ,Tpd_LEGHOLNDOTHr
                                            ,Tpd_LEGHOLNDOTAmt
                                            ,Tpd_SPLHOLHr
                                            ,Tpd_SPLHOLAmt
                                            ,Tpd_SPLHOLOTHr
                                            ,Tpd_SPLHOLOTAmt
                                            ,Tpd_SPLHOLNDHr
                                            ,Tpd_SPLHOLNDAmt
                                            ,Tpd_SPLHOLNDOTHr
                                            ,Tpd_SPLHOLNDOTAmt
                                            ,Tpd_PSDHr
                                            ,Tpd_PSDAmt
                                            ,Tpd_PSDOTHr
                                            ,Tpd_PSDOTAmt
                                            ,Tpd_PSDNDHr
                                            ,Tpd_PSDNDAmt
                                            ,Tpd_PSDNDOTHr
                                            ,Tpd_PSDNDOTAmt
                                            ,Tpd_COMPHOLHr
                                            ,Tpd_COMPHOLAmt
                                            ,Tpd_COMPHOLOTHr
                                            ,Tpd_COMPHOLOTAmt
                                            ,Tpd_COMPHOLNDHr
                                            ,Tpd_COMPHOLNDAmt
                                            ,Tpd_COMPHOLNDOTHr
                                            ,Tpd_COMPHOLNDOTAmt
                                            ,Tpd_RESTLEGHOLHr
                                            ,Tpd_RESTLEGHOLAmt
                                            ,Tpd_RESTLEGHOLOTHr
                                            ,Tpd_RESTLEGHOLOTAmt
                                            ,Tpd_RESTLEGHOLNDHr
                                            ,Tpd_RESTLEGHOLNDAmt
                                            ,Tpd_RESTLEGHOLNDOTHr
                                            ,Tpd_RESTLEGHOLNDOTAmt
                                            ,Tpd_RESTSPLHOLHr
                                            ,Tpd_RESTSPLHOLAmt
                                            ,Tpd_RESTSPLHOLOTHr
                                            ,Tpd_RESTSPLHOLOTAmt
                                            ,Tpd_RESTSPLHOLNDHr
                                            ,Tpd_RESTSPLHOLNDAmt
                                            ,Tpd_RESTSPLHOLNDOTHr
                                            ,Tpd_RESTSPLHOLNDOTAmt
                                            ,Tpd_RESTCOMPHOLHr
                                            ,Tpd_RESTCOMPHOLAmt
                                            ,Tpd_RESTCOMPHOLOTHr
                                            ,Tpd_RESTCOMPHOLOTAmt
                                            ,Tpd_RESTCOMPHOLNDHr
                                            ,Tpd_RESTCOMPHOLNDAmt
                                            ,Tpd_RESTCOMPHOLNDOTHr
                                            ,Tpd_RESTCOMPHOLNDOTAmt
                                            ,Tpd_RESTPSDHr
                                            ,Tpd_RESTPSDAmt
                                            ,Tpd_RESTPSDOTHr
                                            ,Tpd_RESTPSDOTAmt
                                            ,Tpd_RESTPSDNDHr
                                            ,Tpd_RESTPSDNDAmt
                                            ,Tpd_RESTPSDNDOTHr
                                            ,Tpd_RESTPSDNDOTAmt
                                            ,ISNULL(Tpm_Misc1Hr,0) AS Tpm_Misc1Hr
                                            ,ISNULL(Tpm_Misc1Amt,0) AS Tpm_Misc1Amt
                                            ,ISNULL(Tpm_Misc1OTHr,0) AS Tpm_Misc1OTHr
                                            ,ISNULL(Tpm_Misc1OTAmt,0) AS Tpm_Misc1OTAmt
                                            ,ISNULL(Tpm_Misc1NDHr,0) AS Tpm_Misc1NDHr
                                            ,ISNULL(Tpm_Misc1NDAmt,0) AS Tpm_Misc1NDAmt
                                            ,ISNULL(Tpm_Misc1NDOTHr,0) AS Tpm_Misc1NDOTHr
                                            ,ISNULL(Tpm_Misc1NDOTAmt,0) AS Tpm_Misc1NDOTAmt
                                            ,ISNULL(Tpm_Misc2Hr,0) AS Tpm_Misc2Hr
                                            ,ISNULL(Tpm_Misc2Amt,0) AS Tpm_Misc2Amt
                                            ,ISNULL(Tpm_Misc2OTHr,0) AS Tpm_Misc2OTHr
                                            ,ISNULL(Tpm_Misc2OTAmt,0) AS Tpm_Misc2OTAmt
                                            ,ISNULL(Tpm_Misc2NDHr,0) AS Tpm_Misc2NDHr
                                            ,ISNULL(Tpm_Misc2NDAmt,0) AS Tpm_Misc2NDAmt
                                            ,ISNULL(Tpm_Misc2NDOTHr,0) AS Tpm_Misc2NDOTHr
                                            ,ISNULL(Tpm_Misc2NDOTAmt,0) AS Tpm_Misc2NDOTAmt
                                            ,ISNULL(Tpm_Misc3Hr,0) AS Tpm_Misc3Hr
                                            ,ISNULL(Tpm_Misc3Amt,0) AS Tpm_Misc3Amt
                                            ,ISNULL(Tpm_Misc3OTHr,0) AS Tpm_Misc3OTHr
                                            ,ISNULL(Tpm_Misc3OTAmt,0) AS Tpm_Misc3OTAmt
                                            ,ISNULL(Tpm_Misc3NDHr,0) AS Tpm_Misc3NDHr
                                            ,ISNULL(Tpm_Misc3NDAmt,0) AS Tpm_Misc3NDAmt
                                            ,ISNULL(Tpm_Misc3NDOTHr,0) AS Tpm_Misc3NDOTHr
                                            ,ISNULL(Tpm_Misc3NDOTAmt,0) AS Tpm_Misc3NDOTAmt
                                            ,ISNULL(Tpm_Misc4Hr,0) AS Tpm_Misc4Hr
                                            ,ISNULL(Tpm_Misc4Amt,0) AS Tpm_Misc4Amt
                                            ,ISNULL(Tpm_Misc4OTHr,0) AS Tpm_Misc4OTHr
                                            ,ISNULL(Tpm_Misc4OTAmt,0) AS Tpm_Misc4OTAmt
                                            ,ISNULL(Tpm_Misc4NDHr,0) AS Tpm_Misc4NDHr
                                            ,ISNULL(Tpm_Misc4NDAmt,0) AS Tpm_Misc4NDAmt
                                            ,ISNULL(Tpm_Misc4NDOTHr,0) AS Tpm_Misc4NDOTHr
                                            ,ISNULL(Tpm_Misc4NDOTAmt,0) AS Tpm_Misc4NDOTAmt
                                            ,ISNULL(Tpm_Misc5Hr,0) AS Tpm_Misc5Hr
                                            ,ISNULL(Tpm_Misc5Amt,0) AS Tpm_Misc5Amt
                                            ,ISNULL(Tpm_Misc5OTHr,0) AS Tpm_Misc5OTHr
                                            ,ISNULL(Tpm_Misc5OTAmt,0) AS Tpm_Misc5OTAmt
                                            ,ISNULL(Tpm_Misc5NDHr,0) AS Tpm_Misc5NDHr
                                            ,ISNULL(Tpm_Misc5NDAmt,0) AS Tpm_Misc5NDAmt
                                            ,ISNULL(Tpm_Misc5NDOTHr,0) AS Tpm_Misc5NDOTHr
                                            ,ISNULL(Tpm_Misc5NDOTAmt,0) AS Tpm_Misc5NDOTAmt
                                            ,ISNULL(Tpm_Misc6Hr,0) AS Tpm_Misc6Hr
                                            ,ISNULL(Tpm_Misc6Amt,0) AS Tpm_Misc6Amt
                                            ,ISNULL(Tpm_Misc6OTHr,0) AS Tpm_Misc6OTHr
                                            ,ISNULL(Tpm_Misc6OTAmt,0) AS Tpm_Misc6OTAmt
                                            ,ISNULL(Tpm_Misc6NDHr,0) AS Tpm_Misc6NDHr
                                            ,ISNULL(Tpm_Misc6NDAmt,0) AS Tpm_Misc6NDAmt
                                            ,ISNULL(Tpm_Misc6NDOTHr,0) AS Tpm_Misc6NDOTHr
                                            ,ISNULL(Tpm_Misc6NDOTAmt,0) AS Tpm_Misc6NDOTAmt
                                            ,CASE WHEN Tpd_Date >= @SALARYDATE THEN 1 ELSE 0 END AS [RetroIndicator]
                                            FROM Udv_PayrollDtl
                                            INNER JOIN M_Employee ON Tpd_IDNo = Mem_IDNo
                                                 AND Mem_CompanyCode = @COMPANYCODE
                                            LEFT JOIN {0}..T_EmpSalary A ON Tpd_IDNo = A.Tsl_IDNo
												AND  Tpd_Date BETWEEN A.Tsl_StartDate  AND ISNULL(A.Tsl_EndDate, CASE WHEN A.Tsl_StartDate >= @ENDCYCLE THEN A.Tsl_StartDate ELSE 
                                                                                                    CASE WHEN A.Tsl_EndDate IS NULL THEN @ENDCYCLE ELSE A.Tsl_EndDate END 
                                                                                                    END)
											LEFT JOIN {0}..T_EmpSalary B ON Tpd_IDNo = B.Tsl_IDNo
												AND CONVERT(DATE,Mem_IntakeDate) BETWEEN B.Tsl_StartDate  AND ISNULL(B.Tsl_EndDate, CASE WHEN B.Tsl_StartDate >= @ENDCYCLE THEN B.Tsl_StartDate ELSE 
                                                                                                    CASE WHEN B.Tsl_EndDate IS NULL THEN @ENDCYCLE ELSE  B.Tsl_EndDate END 
                                                                                                    END)
                                            WHERE Tpd_IDNo = @IDNUMBER
                                                 AND Tpd_PayCycle = @PAYCYCLECODE
                                            ORDER BY Tpd_Date
                                            ", CentralProfile);
            #endregion
            int idxx = 0;
            ParameterInfo[] paramInfo = new ParameterInfo[5];
            paramInfo[idxx++] = new ParameterInfo("@IDNUMBER", EmployeeId);
            paramInfo[idxx++] = new ParameterInfo("@PAYCYCLECODE", AfftectedPayCycle);
            paramInfo[idxx++] = new ParameterInfo("@SALARYDATE", SalaryDate, SqlDbType.Date);
            paramInfo[idxx++] = new ParameterInfo("@ENDCYCLE", AdjustPayCycleEnd, SqlDbType.Date);
            paramInfo[idxx++] = new ParameterInfo("@COMPANYCODE", CompanyCode);

            DataTable dtResult = dal.ExecuteDataSet(query, CommandType.Text, paramInfo).Tables[0];
            return dtResult;
        }

        public DataTable GetAllEmployeeSystemAdjustmentForProcess(string EmployeeId, object SalaryDate)
        {
            #region query
            string query = string.Format(@"SELECT Tsa_IDNo
                                            ,Tsa_AdjPayCycle
                                            ,Tsa_OrigAdjPayCycle
                                            ,Tsa_PayCycle
                                            ,Tsa_Date
                                            ,Tsa_PremiumGrpCode
                                            ,Tsa_LTHr
                                            ,Tsa_UTHr
                                            ,Tsa_UPLVHr
                                            ,Tsa_ABSLEGHOLHr
                                            ,Tsa_ABSSPLHOLHr
                                            ,Tsa_ABSCOMPHOLHr
                                            ,Tsa_ABSPSDHr
                                            ,Tsa_ABSOTHHOLHr
                                            ,Tsa_WDABSHr
                                            ,Tsa_LTUTMaxHr
                                            ,Tsa_ABSHr
                                            ,Tsa_REGHr
                                            ,Tsa_PDLVHr
                                            ,Tsa_PDLEGHOLHr
                                            ,Tsa_PDSPLHOLHr
                                            ,Tsa_PDCOMPHOLHr
                                            ,Tsa_PDPSDHr
                                            ,Tsa_PDOTHHOLHr
                                            ,Tsa_PDRESTLEGHOLHr
                                            ,Tsa_REGOTHr
                                            ,Tsa_REGNDHr
                                            ,Tsa_REGNDOTHr
                                            ,Tsa_RESTHr
                                            ,Tsa_RESTOTHr
                                            ,Tsa_RESTNDHr
                                            ,Tsa_RESTNDOTHr
                                            ,Tsa_LEGHOLHr
                                            ,Tsa_LEGHOLOTHr
                                            ,Tsa_LEGHOLNDHr
                                            ,Tsa_LEGHOLNDOTHr
                                            ,Tsa_SPLHOLHr
                                            ,Tsa_SPLHOLOTHr
                                            ,Tsa_SPLHOLNDHr
                                            ,Tsa_SPLHOLNDOTHr
                                            ,Tsa_PSDHr
                                            ,Tsa_PSDOTHr
                                            ,Tsa_PSDNDHr
                                            ,Tsa_PSDNDOTHr
                                            ,Tsa_COMPHOLHr
                                            ,Tsa_COMPHOLOTHr
                                            ,Tsa_COMPHOLNDHr
                                            ,Tsa_COMPHOLNDOTHr
                                            ,Tsa_RESTLEGHOLHr
                                            ,Tsa_RESTLEGHOLOTHr
                                            ,Tsa_RESTLEGHOLNDHr
                                            ,Tsa_RESTLEGHOLNDOTHr
                                            ,Tsa_RESTSPLHOLHr
                                            ,Tsa_RESTSPLHOLOTHr
                                            ,Tsa_RESTSPLHOLNDHr
                                            ,Tsa_RESTSPLHOLNDOTHr
                                            ,Tsa_RESTCOMPHOLHr
                                            ,Tsa_RESTCOMPHOLOTHr
                                            ,Tsa_RESTCOMPHOLNDHr
                                            ,Tsa_RESTCOMPHOLNDOTHr
                                            ,Tsa_RESTPSDHr
                                            ,Tsa_RESTPSDOTHr
                                            ,Tsa_RESTPSDNDHr
                                            ,Tsa_RESTPSDNDOTHr

                                            ,Tsa_LTAmt
                                            ,Tsa_UTAmt
                                            ,Tsa_UPLVAmt
                                            ,Tsa_ABSLEGHOLAmt
                                            ,Tsa_ABSSPLHOLAmt
                                            ,Tsa_ABSCOMPHOLAmt
                                            ,Tsa_ABSPSDAmt
                                            ,Tsa_ABSOTHHOLAmt
                                            ,Tsa_WDABSAmt
                                            ,Tsa_LTUTMaxAmt
                                            ,Tsa_ABSAmt
                                            ,Tsa_REGAmt
                                            ,Tsa_PDLVAmt
                                            ,Tsa_PDLEGHOLAmt
                                            ,Tsa_PDSPLHOLAmt
                                            ,Tsa_PDCOMPHOLAmt
                                            ,Tsa_PDPSDAmt
                                            ,Tsa_PDOTHHOLAmt
                                            ,Tsa_PDRESTLEGHOLAmt
                                            ,Tsa_REGOTAmt
                                            ,Tsa_REGNDAmt
                                            ,Tsa_REGNDOTAmt
                                            ,Tsa_RESTAmt
                                            ,Tsa_RESTOTAmt
                                            ,Tsa_RESTNDAmt
                                            ,Tsa_RESTNDOTAmt
                                            ,Tsa_LEGHOLAmt
                                            ,Tsa_LEGHOLOTAmt
                                            ,Tsa_LEGHOLNDAmt
                                            ,Tsa_LEGHOLNDOTAmt
                                            ,Tsa_SPLHOLAmt
                                            ,Tsa_SPLHOLOTAmt
                                            ,Tsa_SPLHOLNDAmt
                                            ,Tsa_SPLHOLNDOTAmt
                                            ,Tsa_PSDAmt
                                            ,Tsa_PSDOTAmt
                                            ,Tsa_PSDNDAmt
                                            ,Tsa_PSDNDOTAmt
                                            ,Tsa_COMPHOLAmt
                                            ,Tsa_COMPHOLOTAmt
                                            ,Tsa_COMPHOLNDAmt
                                            ,Tsa_COMPHOLNDOTAmt
                                            ,Tsa_RESTLEGHOLAmt
                                            ,Tsa_RESTLEGHOLOTAmt
                                            ,Tsa_RESTLEGHOLNDAmt
                                            ,Tsa_RESTLEGHOLNDOTAmt
                                            ,Tsa_RESTSPLHOLAmt
                                            ,Tsa_RESTSPLHOLOTAmt
                                            ,Tsa_RESTSPLHOLNDAmt
                                            ,Tsa_RESTSPLHOLNDOTAmt
                                            ,Tsa_RESTCOMPHOLAmt
                                            ,Tsa_RESTCOMPHOLOTAmt
                                            ,Tsa_RESTCOMPHOLNDAmt
                                            ,Tsa_RESTCOMPHOLNDOTAmt
                                            ,Tsa_RESTPSDAmt
                                            ,Tsa_RESTPSDOTAmt
                                            ,Tsa_RESTPSDNDAmt
                                            ,Tsa_RESTPSDNDOTAmt
                                            ,Tsa_RGAdjAmt
                                            ,Tsa_OTAdjAmt
                                            ,Tsa_HOLAdjAmt
                                            ,Tsa_NDAdjAmt
                                            ,Tsa_LVAdjAmt
                                            ,Tsa_TotalAdjAmt
                                            ,ISNULL(Tsm_Misc1Hr, 0) AS Tsm_Misc1Hr
                                            ,ISNULL(Tsm_Misc1OTHr , 0) AS Tsm_Misc1OTHr
                                            ,ISNULL(Tsm_Misc1NDHr, 0) AS Tsm_Misc1NDHr
                                            ,ISNULL(Tsm_Misc1NDOTHr, 0) AS Tsm_Misc1NDOTHr
                                            ,ISNULL(Tsm_Misc2Hr, 0) AS Tsm_Misc2Hr
                                            ,ISNULL(Tsm_Misc2OTHr, 0) AS Tsm_Misc2OTHr
                                            ,ISNULL(Tsm_Misc2NDHr, 0) AS Tsm_Misc2NDHr
                                            ,ISNULL(Tsm_Misc2NDOTHr, 0) AS Tsm_Misc2NDOTHr
                                            ,ISNULL(Tsm_Misc3Hr, 0) AS Tsm_Misc3Hr
                                            ,ISNULL(Tsm_Misc3OTHr, 0) AS Tsm_Misc3OTHr
                                            ,ISNULL(Tsm_Misc3NDHr, 0) AS Tsm_Misc3NDHr
                                            ,ISNULL(Tsm_Misc3NDOTHr, 0) AS Tsm_Misc3NDOTHr
                                            ,ISNULL(Tsm_Misc4Hr, 0) AS Tsm_Misc4Hr
                                            ,ISNULL(Tsm_Misc4OTHr, 0) AS Tsm_Misc4OTHr
                                            ,ISNULL(Tsm_Misc4NDHr, 0) AS Tsm_Misc4NDHr
                                            ,ISNULL(Tsm_Misc4NDOTHr, 0) AS Tsm_Misc4NDOTHr
                                            ,ISNULL(Tsm_Misc5Hr, 0) AS Tsm_Misc5Hr
                                            ,ISNULL(Tsm_Misc5OTHr, 0) AS Tsm_Misc5OTHr
                                            ,ISNULL(Tsm_Misc5NDHr, 0) AS Tsm_Misc5NDHr
                                            ,ISNULL(Tsm_Misc5NDOTHr, 0) AS Tsm_Misc5NDOTHr
                                            ,ISNULL(Tsm_Misc6Hr, 0) AS Tsm_Misc6Hr
                                            ,ISNULL(Tsm_Misc6OTHr, 0) AS Tsm_Misc6OTHr
                                            ,ISNULL(Tsm_Misc6NDHr, 0) AS Tsm_Misc6NDHr
                                            ,ISNULL(Tsm_Misc6NDOTHr, 0) AS Tsm_Misc6NDOTHr
                                            ,ISNULL(Tsm_Misc1Amt, 0) AS Tsm_Misc1Amt
                                            ,ISNULL(Tsm_Misc1OTAmt, 0) AS Tsm_Misc1OTAmt
                                            ,ISNULL(Tsm_Misc1NDAmt, 0) AS Tsm_Misc1NDAmt
                                            ,ISNULL(Tsm_Misc1NDOTAmt, 0) AS Tsm_Misc1NDOTAmt
                                            ,ISNULL(Tsm_Misc2Amt, 0) AS Tsm_Misc2Amt
                                            ,ISNULL(Tsm_Misc2OTAmt, 0) AS Tsm_Misc2OTAmt
                                            ,ISNULL(Tsm_Misc2NDAmt, 0) AS Tsm_Misc2NDAmt
                                            ,ISNULL(Tsm_Misc2NDOTAmt, 0) AS Tsm_Misc2NDOTAmt
                                            ,ISNULL(Tsm_Misc3Amt, 0) AS Tsm_Misc3Amt
                                            ,ISNULL(Tsm_Misc3OTAmt, 0) AS Tsm_Misc3OTAmt
                                            ,ISNULL(Tsm_Misc3NDAmt, 0) AS Tsm_Misc3NDAmt
                                            ,ISNULL(Tsm_Misc3NDOTAmt, 0) AS Tsm_Misc3NDOTAmt
                                            ,ISNULL(Tsm_Misc4Amt, 0) AS Tsm_Misc4Amt
                                            ,ISNULL(Tsm_Misc4OTAmt, 0) AS Tsm_Misc4OTAmt
                                            ,ISNULL(Tsm_Misc4NDAmt, 0) AS Tsm_Misc4NDAmt
                                            ,ISNULL(Tsm_Misc4NDOTAmt, 0) AS Tsm_Misc4NDOTAmt
                                            ,ISNULL(Tsm_Misc5Amt, 0) AS Tsm_Misc5Amt
                                            ,ISNULL(Tsm_Misc5OTAmt, 0) AS Tsm_Misc5OTAmt
                                            ,ISNULL(Tsm_Misc5NDAmt, 0) AS Tsm_Misc5NDAmt
                                            ,ISNULL(Tsm_Misc5NDOTAmt, 0) AS Tsm_Misc5NDOTAmt
                                            ,ISNULL(Tsm_Misc6Amt, 0) AS Tsm_Misc6Amt
                                            ,ISNULL(Tsm_Misc6OTAmt, 0) AS Tsm_Misc6OTAmt
                                            ,ISNULL(Tsm_Misc6NDAmt, 0) AS Tsm_Misc6NDAmt
                                            ,ISNULL(Tsm_Misc6NDOTAmt, 0) AS Tsm_Misc6NDOTAmt
                                            FROM Udv_SystemAdjustment
                                            WHERE Tsa_IDNo = @IDNUMBER
                                                AND Tsa_Date >= @SALARYDATE
                                                AND Tsa_PostFlag = 1
                                                AND Tps_CycleIndicator <> 'C'
                                            ");
            #endregion
            int idxx = 0;
            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[idxx++] = new ParameterInfo("@IDNUMBER", EmployeeId);
            paramInfo[idxx++] = new ParameterInfo("@SALARYDATE", (GetValue(SalaryDate) != string.Empty ? SalaryDate : DBNull.Value), SqlDbType.Date);
            DataTable dtResult = dal.ExecuteDataSet(query, CommandType.Text, paramInfo).Tables[0];
            return dtResult;
        }

        public DataTable GetAllEmployeePayrollForProcess(string EmployeeId, string AfftectedPayCycle)
        {
            #region query
            string query = string.Format(@"SELECT Tpy_IDNo
                                            ,Tpy_PayCycle
                                            ,Tpy_SalaryRate
                                            ,Tpy_HourRate
                                            ,Tpy_SpecialRate
                                            ,Tpy_SpecialHourRate
                                            ,Tpy_PremiumGrpCode
                                            ,Tpy_LTHr
                                            ,Tpy_LTAmt
                                            ,Tpy_UTHr
                                            ,Tpy_UTAmt
                                            ,Tpy_UPLVHr
                                            ,Tpy_UPLVAmt
                                            ,Tpy_ABSLEGHOLHr
                                            ,Tpy_ABSLEGHOLAmt
                                            ,Tpy_ABSSPLHOLHr
                                            ,Tpy_ABSSPLHOLAmt
                                            ,Tpy_ABSCOMPHOLHr
                                            ,Tpy_ABSCOMPHOLAmt
                                            ,Tpy_ABSPSDHr
                                            ,Tpy_ABSPSDAmt
                                            ,Tpy_ABSOTHHOLHr
                                            ,Tpy_ABSOTHHOLAmt
                                            ,Tpy_WDABSHr
                                            ,Tpy_WDABSAmt
                                            ,Tpy_LTUTMaxHr
                                            ,Tpy_LTUTMaxAmt
                                            ,Tpy_ABSHr
                                            ,Tpy_ABSAmt
                                            ,Tpy_REGHr
                                            ,Tpy_REGAmt
                                            ,Tpy_PDLVHr
                                            ,Tpy_PDLVAmt
                                            ,Tpy_PDLEGHOLHr
                                            ,Tpy_PDLEGHOLAmt
                                            ,Tpy_PDSPLHOLHr
                                            ,Tpy_PDSPLHOLAmt
                                            ,Tpy_PDCOMPHOLHr
                                            ,Tpy_PDCOMPHOLAmt
                                            ,Tpy_PDPSDHr
                                            ,Tpy_PDPSDAmt
                                            ,Tpy_PDOTHHOLHr
                                            ,Tpy_PDOTHHOLAmt
                                            ,Tpy_PDRESTLEGHOLHr
                                            ,Tpy_PDRESTLEGHOLAmt
                                            ,Tpy_TotalREGHr
                                            ,Tpy_TotalREGAmt
                                            ,Tpy_REGOTHr
                                            ,Tpy_REGOTAmt
                                            ,Tpy_REGNDHr
                                            ,Tpy_REGNDAmt
                                            ,Tpy_REGNDOTHr
                                            ,Tpy_REGNDOTAmt
                                            ,Tpy_RESTHr
                                            ,Tpy_RESTAmt
                                            ,Tpy_RESTOTHr
                                            ,Tpy_RESTOTAmt
                                            ,Tpy_RESTNDHr
                                            ,Tpy_RESTNDAmt
                                            ,Tpy_RESTNDOTHr
                                            ,Tpy_RESTNDOTAmt
                                            ,Tpy_LEGHOLHr
                                            ,Tpy_LEGHOLAmt
                                            ,Tpy_LEGHOLOTHr
                                            ,Tpy_LEGHOLOTAmt
                                            ,Tpy_LEGHOLNDHr
                                            ,Tpy_LEGHOLNDAmt
                                            ,Tpy_LEGHOLNDOTHr
                                            ,Tpy_LEGHOLNDOTAmt
                                            ,Tpy_SPLHOLHr
                                            ,Tpy_SPLHOLAmt
                                            ,Tpy_SPLHOLOTHr
                                            ,Tpy_SPLHOLOTAmt
                                            ,Tpy_SPLHOLNDHr
                                            ,Tpy_SPLHOLNDAmt
                                            ,Tpy_SPLHOLNDOTHr
                                            ,Tpy_SPLHOLNDOTAmt
                                            ,Tpy_PSDHr
                                            ,Tpy_PSDAmt
                                            ,Tpy_PSDOTHr
                                            ,Tpy_PSDOTAmt
                                            ,Tpy_PSDNDHr
                                            ,Tpy_PSDNDAmt
                                            ,Tpy_PSDNDOTHr
                                            ,Tpy_PSDNDOTAmt
                                            ,Tpy_COMPHOLHr
                                            ,Tpy_COMPHOLAmt
                                            ,Tpy_COMPHOLOTHr
                                            ,Tpy_COMPHOLOTAmt
                                            ,Tpy_COMPHOLNDHr
                                            ,Tpy_COMPHOLNDAmt
                                            ,Tpy_COMPHOLNDOTHr
                                            ,Tpy_COMPHOLNDOTAmt
                                            ,Tpy_RESTLEGHOLHr
                                            ,Tpy_RESTLEGHOLAmt
                                            ,Tpy_RESTLEGHOLOTHr
                                            ,Tpy_RESTLEGHOLOTAmt
                                            ,Tpy_RESTLEGHOLNDHr
                                            ,Tpy_RESTLEGHOLNDAmt
                                            ,Tpy_RESTLEGHOLNDOTHr
                                            ,Tpy_RESTLEGHOLNDOTAmt
                                            ,Tpy_RESTSPLHOLHr
                                            ,Tpy_RESTSPLHOLAmt
                                            ,Tpy_RESTSPLHOLOTHr
                                            ,Tpy_RESTSPLHOLOTAmt
                                            ,Tpy_RESTSPLHOLNDHr
                                            ,Tpy_RESTSPLHOLNDAmt
                                            ,Tpy_RESTSPLHOLNDOTHr
                                            ,Tpy_RESTSPLHOLNDOTAmt
                                            ,Tpy_RESTCOMPHOLHr
                                            ,Tpy_RESTCOMPHOLAmt
                                            ,Tpy_RESTCOMPHOLOTHr
                                            ,Tpy_RESTCOMPHOLOTAmt
                                            ,Tpy_RESTCOMPHOLNDHr
                                            ,Tpy_RESTCOMPHOLNDAmt
                                            ,Tpy_RESTCOMPHOLNDOTHr
                                            ,Tpy_RESTCOMPHOLNDOTAmt
                                            ,Tpy_RESTPSDHr
                                            ,Tpy_RESTPSDAmt
                                            ,Tpy_RESTPSDOTHr
                                            ,Tpy_RESTPSDOTAmt
                                            ,Tpy_RESTPSDNDHr
                                            ,Tpy_RESTPSDNDAmt
                                            ,Tpy_RESTPSDNDOTHr
                                            ,Tpy_RESTPSDNDOTAmt
                                            ,Tpy_TotalOTNDAmt
                                            ,Tpy_WorkDay
                                            ,Tpy_DayCountOldSalaryRate
                                            ,Tpy_SRGAdjHr
                                            ,Tpy_SRGAdjAmt
                                            ,Tpy_SOTAdjHr
                                            ,Tpy_SOTAdjAmt
                                            ,Tpy_SHOLAdjHr
                                            ,Tpy_SHOLAdjAmt
                                            ,Tpy_SNDAdjHr
                                            ,Tpy_SNDAdjAmt
                                            ,Tpy_SLVAdjHr
                                            ,Tpy_SLVAdjAmt
                                            ,Tpy_MRGAdjHr
                                            ,Tpy_MRGAdjAmt
                                            ,Tpy_MOTAdjHr
                                            ,Tpy_MOTAdjAmt
                                            ,Tpy_MHOLAdjHr
                                            ,Tpy_MHOLAdjAmt
                                            ,Tpy_MNDAdjHr
                                            ,Tpy_MNDAdjAmt
                                            ,Tpy_TotalAdjAmt
                                            ,Tpy_TaxableIncomeAmt
                                            ,Tpy_TotalTaxableIncomeAmt
                                            ,Tpy_NontaxableIncomeAmt
                                            ,Tpy_GrossIncomeAmt
                                            ,ISNULL(Tpm_Misc1Hr,0) AS Tpm_Misc1Hr
                                            ,ISNULL(Tpm_Misc1Amt,0) AS Tpm_Misc1Amt
                                            ,ISNULL(Tpm_Misc1OTHr,0) AS Tpm_Misc1OTHr
                                            ,ISNULL(Tpm_Misc1OTAmt,0) AS Tpm_Misc1OTAmt
                                            ,ISNULL(Tpm_Misc1NDHr,0) AS Tpm_Misc1NDHr
                                            ,ISNULL(Tpm_Misc1NDAmt,0) AS Tpm_Misc1NDAmt
                                            ,ISNULL(Tpm_Misc1NDOTHr,0) AS Tpm_Misc1NDOTHr
                                            ,ISNULL(Tpm_Misc1NDOTAmt,0) AS Tpm_Misc1NDOTAmt
                                            ,ISNULL(Tpm_Misc2Hr,0) AS Tpm_Misc2Hr
                                            ,ISNULL(Tpm_Misc2Amt,0) AS Tpm_Misc2Amt
                                            ,ISNULL(Tpm_Misc2OTHr,0) AS Tpm_Misc2OTHr
                                            ,ISNULL(Tpm_Misc2OTAmt,0) AS Tpm_Misc2OTAmt
                                            ,ISNULL(Tpm_Misc2NDHr,0) AS Tpm_Misc2NDHr
                                            ,ISNULL(Tpm_Misc2NDAmt,0) AS Tpm_Misc2NDAmt
                                            ,ISNULL(Tpm_Misc2NDOTHr,0) AS Tpm_Misc2NDOTHr
                                            ,ISNULL(Tpm_Misc2NDOTAmt,0) AS Tpm_Misc2NDOTAmt
                                            ,ISNULL(Tpm_Misc3Hr,0) AS Tpm_Misc3Hr
                                            ,ISNULL(Tpm_Misc3Amt,0) AS Tpm_Misc3Amt
                                            ,ISNULL(Tpm_Misc3OTHr,0) AS Tpm_Misc3OTHr
                                            ,ISNULL(Tpm_Misc3OTAmt,0) AS Tpm_Misc3OTAmt
                                            ,ISNULL(Tpm_Misc3NDHr,0) AS Tpm_Misc3NDHr
                                            ,ISNULL(Tpm_Misc3NDAmt,0) AS Tpm_Misc3NDAmt
                                            ,ISNULL(Tpm_Misc3NDOTHr,0) AS Tpm_Misc3NDOTHr
                                            ,ISNULL(Tpm_Misc3NDOTAmt,0) AS Tpm_Misc3NDOTAmt
                                            ,ISNULL(Tpm_Misc4Hr,0) AS Tpm_Misc4Hr
                                            ,ISNULL(Tpm_Misc4Amt,0) AS Tpm_Misc4Amt
                                            ,ISNULL(Tpm_Misc4OTHr,0) AS Tpm_Misc4OTHr
                                            ,ISNULL(Tpm_Misc4OTAmt,0) AS Tpm_Misc4OTAmt
                                            ,ISNULL(Tpm_Misc4NDHr,0) AS Tpm_Misc4NDHr
                                            ,ISNULL(Tpm_Misc4NDAmt,0) AS Tpm_Misc4NDAmt
                                            ,ISNULL(Tpm_Misc4NDOTHr,0) AS Tpm_Misc4NDOTHr
                                            ,ISNULL(Tpm_Misc4NDOTAmt,0) AS Tpm_Misc4NDOTAmt
                                            ,ISNULL(Tpm_Misc5Hr,0) AS Tpm_Misc5Hr
                                            ,ISNULL(Tpm_Misc5Amt,0) AS Tpm_Misc5Amt
                                            ,ISNULL(Tpm_Misc5OTHr,0) AS Tpm_Misc5OTHr
                                            ,ISNULL(Tpm_Misc5OTAmt,0) AS Tpm_Misc5OTAmt
                                            ,ISNULL(Tpm_Misc5NDHr,0) AS Tpm_Misc5NDHr
                                            ,ISNULL(Tpm_Misc5NDAmt,0) AS Tpm_Misc5NDAmt
                                            ,ISNULL(Tpm_Misc5NDOTHr,0) AS Tpm_Misc5NDOTHr
                                            ,ISNULL(Tpm_Misc5NDOTAmt,0) AS Tpm_Misc5NDOTAmt
                                            ,ISNULL(Tpm_Misc6Hr,0) AS Tpm_Misc6Hr
                                            ,ISNULL(Tpm_Misc6Amt,0) AS Tpm_Misc6Amt
                                            ,ISNULL(Tpm_Misc6OTHr,0) AS Tpm_Misc6OTHr
                                            ,ISNULL(Tpm_Misc6OTAmt,0) AS Tpm_Misc6OTAmt
                                            ,ISNULL(Tpm_Misc6NDHr,0) AS Tpm_Misc6NDHr
                                            ,ISNULL(Tpm_Misc6NDAmt,0) AS Tpm_Misc6NDAmt
                                            ,ISNULL(Tpm_Misc6NDOTHr,0) AS Tpm_Misc6NDOTHr
                                            ,ISNULL(Tpm_Misc6NDOTAmt,0) AS Tpm_Misc6NDOTAmt
                                            ,Mem_CostcenterCode
                                            ,Mem_EmploymentStatusCode
                                            ,Mem_PayrollGroup
                                            FROM Udv_Payroll
                                            LEFT JOIN M_EmpLoyee ON Tpy_IDNo = Mem_IDNo
                                            WHERE Tpy_IDNo = '{0}'
                                                 AND Tpy_PayCycle = '{1}'
                                            ORDER BY Mem_LastName, Mem_FirstName
                                                ", EmployeeId
                                                 , AfftectedPayCycle);
            #endregion
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public DataTable GetDayCodePremiums(string CompanyCode, string CentralProfile, DALHelper dal)
        {
            string query = string.Format(@"SELECT Mdp_PremiumGrpCode
                                          , Mdp_PayrollType
                                          , Mdp_DayCode
                                          , Mdp_RestDayFlag
                                          , Mdp_RGPremiumRate
                                          , Mdp_OTPremiumRate
                                          , Mdp_RGNDPremiumRate 
                                          , Mdp_OTNDPremiumRate 
                                          , Mdp_RGNDPercentageRate
                                          , Mdp_OTNDPercentageRate
                                      FROM {1}..M_DayPremium
                                      WHERE Mdp_CompanyCode = '{0}'
                                        AND Mdp_RecordStatus = 'A'", CompanyCode, CentralProfile);

            DataTable dtResult;
            dtResult = dal.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public DataTable GetPayPeriodCycle(string PayPeriod)
        {
            string strQuery = string.Format(@"SELECT	Tps_PayCycle
		                                                , Tps_StartCycle
		                                                , Tps_EndCycle
                                                        , Tps_CycleIndicator
                                                FROM	T_PaySchedule 
                                                WHERE	Tps_PayCycle = '{0}' 
                                                AND		Tps_RecordStatus = 'A'", PayPeriod);

            DataTable dtResult = dal.ExecuteDataSet(strQuery).Tables[0];
            return dtResult;
        }

        public void AddErrorToRetroReport(string strEmployeeId, string LastName, string FirstName, string PayCycle, decimal Amount, decimal Amount2, string strRemarks)
        {
            listPayrollRept.Add(new structRetroErrorReport(strEmployeeId, LastName, FirstName, PayCycle, Amount, Amount2, strRemarks));
        }

        public void InitializePayrollReport()
        {
            listPayrollRept.Clear();
        }

        public void ClearPayrollTables(bool ProcessAll, string EmployeeId)
        {
            string query = @"DELETE FROM T_EmpPayrollRetro WHERE Tpy_RetroPayCycle = '{0}' {1}
                             DELETE FROM T_EmpPayrollMiscRetro WHERE Tpm_RetroPayCycle = '{0}' {2}
                             DELETE FROM T_EmpPayrollDtlRetro WHERE Tpd_RetroPayCycle = '{0}' {3}
                             DELETE FROM T_EmpPayrollDtlMiscRetro WHERE Tpm_RetroPayCycle = '{0}' {2} 
                             DELETE FROM T_EmpSystemAdj2Retro WHERE Tsa_RetroPayCycle = '{0}' {4}
                             DELETE FROM T_EmpSystemAdjMisc2Retro WHERE Tsm_RetroPayCycle = '{0}' {5}
                             DELETE FROM T_EmpIncome WHERE Tin_IncomeCode LIKE 'RETRO_%' AND Tin_PayCycle = '{0}' AND Tin_OrigPayCycle = '{0}' {6}
                             UPDATE T_EmpRetroPay SET Ter_RGRetroAmt = 0, Ter_OTRetroAmt = 0, Ter_NDRetroAmt = 0, Ter_HLRetroAmt = 0, Ter_LVRetroAmt = 0, Ter_TotalRetroAmt = 0, Usr_Login = '" + LoginUser + @"', Ludatetime = GETDATE() WHERE Ter_PayCycle = '{0}' {7} ";

            if (ProcessAll == true)
                query = string.Format(query, ProcessPayrollPeriod, "", "", "", "", "", "", "");
            else
                query = string.Format(query, ProcessPayrollPeriod
                                            , string.Format("AND Tpy_IDNo = '{0}'", EmployeeId)
                                            , string.Format("AND Tpm_IDNo = '{0}'", EmployeeId)
                                            , string.Format("AND Tpd_IDNo = '{0}'", EmployeeId)
                                            , string.Format("AND Tsa_IDNo = '{0}'", EmployeeId)
                                            , string.Format("AND Tsm_IDNo = '{0}'", EmployeeId)
                                            , string.Format("AND Tin_IDNo = '{0}'", EmployeeId)
                                            , string.Format("AND Ter_IDNo = '{0}'", EmployeeId));

            dal.ExecuteNonQuery(query);

        }

        public decimal GetFormulaQueryDecimalValue(string query, ParameterInfo[] paramInfo)
        {
            decimal dValue = 0;
            DataTable dtResult = dal.ExecuteDataSet(query, CommandType.Text, paramInfo).Tables[0];
            if (dtResult.Rows.Count > 0)
            {
                dValue = Convert.ToDecimal(dtResult.Rows[0][0]);
            }
            return dValue;
        }

        public decimal GetLateDeductionFromBracket(decimal LateHours)
        {
            decimal DeductHours = 0;
            if (LateHours > 0 && LATEBRCKTQ != null)
            {
                for (int i = 0; i < LATEBRCKTQ.Rows.Count; i++)
                {
                    //Example: (Please take note of the ranging of values)
                    //16 = 30 
                    //46 = 60 
                    //60 = 0
                    //This means, 
                    //16-45 late mins = 30 mins deduction
                    //46-59 late mins = 60 mins deduction
                    if (i + 1 != LATEBRCKTQ.Rows.Count
                        && LateHours >= Convert.ToDecimal(LATEBRCKTQ.Rows[i]["Mpd_SubCode"])
                        && LateHours < Convert.ToDecimal(LATEBRCKTQ.Rows[i + 1]["Mpd_SubCode"]))
                    {
                        DeductHours = Convert.ToDecimal(LATEBRCKTQ.Rows[i]["Mpd_ParamValue"]) - LateHours;
                        break;
                    }
                }
            }
            return DeductHours;
        }

        public int PostToIncome(string paycyclecode, string userLogin, string companyCode, string centralProfile, DALHelper dalhelper)
        {
            int retVal = 0;
            string Condition = string.Empty;

            #region Query
            string query = string.Format(@"INSERT INTO {1}..M_Income
                                                        (Min_CompanyCode
                                                        ,Min_IncomeCode
                                                        ,Min_IncomeName
                                                        ,Min_TaxClass
                                                        ,Min_IsRecurring
														,Min_ApplicablePayCycle
														,Min_IncomeGroup
														,Min_AlphalistCategory
														,Min_IsSystemReserved
                                                        ,Min_AccountGrp
														,Min_RemittanceLoanType
                                                        ,Min_CycleOpenUnpostedRule
                                                        ,Min_RecordStatus
                                                        ,Min_CreatedBy
                                                        ,Min_CreatedDate)
                                                    SELECT '{2}'
                                                        ,Retro.Code
	                                                    ,Retro.IncomeName
                                                        ,'T'
                                                        ,0
                                                        ,0
                                                        ,Retro.IncomeGroup
                                                        ,Retro.AlpgalistCategory
                                                        ,1
                                                        ,''
                                                        ,''
                                                        ,'N'
														,'A'
                                                        ,'{3}'
                                                        ,GETDATE()
                                                    FROM 
                                                    (
	                                                    SELECT 'RETRO_RG' AS [Code], 'RETRO-REGULAR PAY' AS [IncomeName], 'REG' AS [IncomeGroup], 'T-BASICSAL' AS [AlpgalistCategory] UNION ALL
														SELECT 'RETRO_OT' AS [Code], 'RETRO-OVERTIME PAY' AS [IncomeName], 'OVT' AS [IncomeGroup], 'T-OVERTIMEPY' AS [AlpgalistCategory] UNION ALL
														SELECT 'RETRO_ND' AS [Code], 'RETRO-NIGHT PREMIUM PAY' AS [IncomeName], 'NDF' AS [IncomeGroup], 'T-NDPAY' AS [AlpgalistCategory] UNION ALL
														SELECT 'RETRO_LV' AS [Code], 'RETRO-LEAVE PAY' AS [IncomeName], 'LVE' AS [IncomeGroup], 'T-BASICSAL' AS [AlpgalistCategory] UNION ALL
														SELECT 'RETRO_HL' AS [Code], 'RETRO-HOLIDAY PAY' AS [IncomeName], 'HOL' AS [IncomeGroup], 'T-BASICSAL' AS [AlpgalistCategory]
                                                    ) Retro
                                                    LEFT JOIN {1}..M_Income
                                                    ON Retro.Code = Min_IncomeCode
                                                        AND Min_CompanyCode = '{2}'
                                                    WHERE Min_IncomeCode IS NULL
                                                        

                                                    DELETE FROM T_EmpIncome
                                                    WHERE Tin_IncomeCode LIKE 'RETRO_%'
                                                        AND Tin_PayCycle = '{0}'
                                                        AND Tin_OrigPayCycle = '{0}'
                                                    

                                                    INSERT INTO T_EmpIncome
                                                               (Tin_IDNo
                                                               ,Tin_PayCycle
                                                               ,Tin_OrigPayCycle
                                                               ,Tin_IncomeCode
                                                               ,Tin_PostFlag
                                                               ,Tin_IncomeAmt
                                                               ,Usr_Login
                                                               ,Ludatetime)
                                                       SELECT Ter_IDNo
                                                          ,'{0}'
                                                          ,'{0}'
                                                          ,'RETRO_RG'
                                                          ,0
                                                          ,Ter_RGRetroAmt
                                                          ,'{3}'
                                                          ,GETDATE()
                                                      FROM T_EmpRetroPay
                                                      WHERE Ter_RGRetroAmt != 0
														AND Ter_PayCycle = '{0}'

													 UNION ALL 

													 SELECT Ter_IDNo
                                                          ,'{0}'
                                                          ,'{0}'
                                                          ,'RETRO_OT'
                                                          ,0
                                                          ,Ter_OTRetroAmt
                                                          ,'{3}'
                                                          ,GETDATE()
                                                      FROM T_EmpRetroPay
                                                      WHERE Ter_OTRetroAmt != 0
														AND Ter_PayCycle = '{0}'

													UNION ALL

													SELECT Ter_IDNo
                                                          ,'{0}'
                                                          ,'{0}'
                                                          ,'RETRO_ND'
                                                          ,0
                                                          ,Ter_NDRetroAmt
                                                          ,'{3}'
                                                          ,GETDATE()
                                                      FROM T_EmpRetroPay
                                                      WHERE Ter_NDRetroAmt != 0
														AND Ter_PayCycle = '{0}'

													 UNION ALL

													 SELECT Ter_IDNo
                                                          ,'{0}'
                                                          ,'{0}'
                                                          ,'RETRO_HL'
                                                          ,0
                                                          ,Ter_HLRetroAmt
                                                          ,'{3}'
                                                          ,GETDATE()
                                                      FROM T_EmpRetroPay
                                                      WHERE Ter_HLRetroAmt != 0
														AND Ter_PayCycle = '{0}'

													UNION ALL

													SELECT Ter_IDNo
                                                          ,'{0}'
                                                          ,'{0}'
                                                          ,'RETRO_LV'
                                                          ,0
                                                          ,Ter_LVRetroAmt
                                                          ,'{3}'
                                                          ,GETDATE()
                                                      FROM T_EmpRetroPay
                                                      WHERE Ter_LVRetroAmt != 0
														AND Ter_PayCycle = '{0}'


                                                      UPDATE T_EmpRetroPay
                                                      SET Ter_PostFlag = 1
                                                        , Usr_login = '{3}'
                                                        , Ludatetime = GETDATE()
                                                      WHERE Ter_TotalRetroAmt != 0
                                                        AND Ter_PayCycle = '{0}'
                                                   ", paycyclecode
                                                   , centralProfile
                                                   , companyCode
                                                   , userLogin);
            #endregion
            try
            {
                retVal = dalhelper.ExecuteNonQuery(query);
            }
            catch (Exception e)
            {
                throw new PayrollException(e);
            }
            return retVal;
        }

        public DataTable RetroPayExceptionList(string profileCode, string centralProfile, string PayCycle, string EmployeeID, string userlogin, string companyCode, string ErrorType, string menucode, DALHelper dalHelper)
        {
            #region Query
            string condition = string.Empty;
            if (ErrorType != string.Empty)
            {
                condition = string.Format(@"
                                            And Tpc_Type LIKE '%{0}'", ErrorType);
            }
            string query = string.Format(@"
                        SELECT CASE WHEN Tpc_Type IN ('AW','BW') THEN 'WARNING' 
                                    WHEN Tpc_Type IN ('AE','BE') THEN 'ERROR' END as [Type]
                                    , Tpc_IDNo AS [ID Number] 
									, Mem_LastName + CASE WHEN LEN(ISNULL(Mem_ExtensionName,'')) > 0 
										THEN  ' ' + Mem_ExtensionName ELSE '' END as [Last Name]
									, Mem_FirstName as [First Name]
									, Mem_MiddleName as [Middle Name]
                                    , Tpc_NumValue1 as [Summary Amount]
                                    , Tpc_NumValue2 as [Detail Amount]
                                    , Tpc_Remarks as [Remarks]
                                    FROM (SELECT * FROM T_EmpProcessCheck 
                                          UNION ALL
                                          SELECT * FROM T_EmpProcessCheckHst ) Tmp
									INNER JOIN M_Employee ON Mem_IDNo = Tpc_IDNo
                                    @USERCOSTCENTERACCESSCONDITION
                                    WHERE Tpc_ModuleCode = '{4}'
                                    AND Tpc_PayCycle = '{0}'
                                    AND ('{1}' = '' OR Tpc_IDNo = '{1}')
                                    @CONDITIONS	
                                    "
                                    , PayCycle, EmployeeID, companyCode, centralProfile, menucode);
            #endregion
            query = query.Replace("@CONDITIONS", condition);
            query = query.Replace("@USERCOSTCENTERACCESSCONDITION", (new HRCReportsBL()).UserCostCenterAccessTmpQuery(profileCode, "PAYROLL", userlogin, "Mem_CostcenterCode", "Mem_PayrollGroup", "Mem_EmploymentStatusCode", "Mem_PayrollType", companyCode, centralProfile, false));
            DataTable dtResult;
            dtResult = dalHelper.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public void RetroPostProcessingCheck(string ProfileCode, string centralProfile, string PayCycle, string EmployeeID, string CostCenter, string UserLogin, string companyCode, string menucode, DALHelper dalHelper)
        {
            #region Query
            string condition = string.Empty;
            if (CostCenter != string.Empty)
            {
                condition += string.Format("AND RETRO.Tpy_CostcenterCode IN ({0})", CostCenter);
            }
            string query = string.Format(@"
            DELETE FROM T_EmpProcessCheck WHERE Tpc_ModuleCode = '{5}' AND Tpc_SystemDefined = 1
                AND Tpc_PayCycle = '{3}'
                AND ('{2}' = '' OR Tpc_IDNo = '{2}')

            INSERT INTO T_EmpProcessCheck

            SELECT '{5}'
                , Tpd_IDNo
                , 'AE'
                , '{3}'
                , [Value 1]
                , 0
                , Remarks
                , 1
                , '{4}'
                , GETDATE()
            FROM (
                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_LTHr > 0 AND Tpd_LTAmt = 0' AS [Remarks], Tpd_LTHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_LTHr > 0 AND Tpd_LTAmt = 0
                    AND Tpd_PayCycle = '{3}'
                UNION	

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_UTHr > 0 AND Tpd_UTAmt = 0' AS [Remarks], Tpd_UTHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_UTHr > 0 AND Tpd_UTAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_UPLVHr > 0 AND Tpd_UPLVAmt = 0' AS [Remarks], Tpd_UPLVHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_UPLVHr > 0 AND Tpd_UPLVAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_ABSLEGHOLHr > 0 AND Tpd_ABSLEGHOLAmt = 0' AS [Remarks], Tpd_ABSLEGHOLHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_ABSLEGHOLHr > 0 AND Tpd_ABSLEGHOLAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_ABSSPLHOLHr > 0 AND Tpd_ABSSPLHOLAmt = 0' AS [Remarks], Tpd_ABSSPLHOLHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_ABSSPLHOLHr > 0 AND Tpd_ABSSPLHOLAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_ABSCOMPHOLHr > 0 AND Tpd_ABSCOMPHOLAmt = 0' AS [Remarks], Tpd_ABSCOMPHOLHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_ABSCOMPHOLHr > 0 AND Tpd_ABSCOMPHOLAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_ABSPSDHr > 0 AND Tpd_ABSPSDAmt = 0' AS [Remarks], Tpd_ABSPSDHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_ABSPSDHr > 0 AND Tpd_ABSPSDAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_ABSOTHHOLHr > 0 AND Tpd_ABSOTHHOLAmt = 0' AS [Remarks], Tpd_ABSOTHHOLHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_ABSOTHHOLHr > 0 AND Tpd_ABSOTHHOLAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_WDABSHr > 0 AND Tpd_WDABSAmt = 0' AS [Remarks], Tpd_WDABSHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_WDABSHr > 0 AND Tpd_WDABSAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_LTUTMaxHr > 0 AND Tpd_LTUTMaxAmt = 0' AS [Remarks], Tpd_LTUTMaxHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_LTUTMaxHr > 0 AND Tpd_LTUTMaxAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_ABSHr > 0 AND Tpd_ABSAmt = 0' AS [Remarks], Tpd_ABSHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_ABSHr > 0 AND Tpd_ABSAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_REGHr > 0 AND Tpd_REGAmt = 0' AS [Remarks], Tpd_REGHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_REGHr > 0 AND Tpd_REGAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_PDLVHr > 0 AND Tpd_PDLVAmt = 0' AS [Remarks], Tpd_PDLVHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_PDLVHr > 0 AND Tpd_PDLVAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_PDLEGHOLHr > 0 AND Tpd_PDLEGHOLAmt = 0' AS [Remarks], Tpd_PDLEGHOLHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_PDLEGHOLHr > 0 AND Tpd_PDLEGHOLAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_PDSPLHOLHr > 0 AND Tpd_PDSPLHOLAmt = 0' AS [Remarks], Tpd_PDSPLHOLHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_PDSPLHOLHr > 0 AND Tpd_PDSPLHOLAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_PDCOMPHOLHr > 0 AND Tpd_PDCOMPHOLAmt = 0' AS [Remarks], Tpd_PDCOMPHOLHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_PDCOMPHOLHr > 0 AND Tpd_PDCOMPHOLAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_PDPSDHr > 0 AND Tpd_PDPSDAmt = 0' AS [Remarks], Tpd_PDPSDHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_PDPSDHr > 0 AND Tpd_PDPSDAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_PDOTHHOLHr > 0 AND Tpd_PDOTHHOLAmt = 0' AS [Remarks], Tpd_PDOTHHOLHr AS [Value 1]FROM T_EmpPayrollDtlYearly
                WHERE Tpd_PDOTHHOLHr > 0 AND Tpd_PDOTHHOLAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_PDRESTLEGHOLHr > 0 AND Tpd_PDRESTLEGHOLAmt = 0' AS [Remarks], Tpd_PDRESTLEGHOLHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_PDRESTLEGHOLHr > 0 AND Tpd_PDRESTLEGHOLAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_TotalREGHr > 0 AND Tpd_TotalREGAmt = 0' AS [Remarks], Tpd_TotalREGHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_TotalREGHr > 0 AND Tpd_TotalREGAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_REGOTHr > 0 AND Tpd_REGOTAmt = 0' AS [Remarks], Tpd_REGOTHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_REGOTHr > 0 AND Tpd_REGOTAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_REGNDHr > 0 AND Tpd_REGNDAmt = 0' AS [Remarks], Tpd_REGNDHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_REGNDHr > 0 AND Tpd_REGNDAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_REGNDOTHr > 0 AND Tpd_REGNDOTAmt = 0' AS [Remarks], Tpd_REGNDOTHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_REGNDOTHr > 0 AND Tpd_REGNDOTAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_RESTHr > 0 AND Tpd_RESTAmt = 0' AS [Remarks], Tpd_RESTHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_RESTHr > 0 AND Tpd_RESTAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_RESTOTHr > 0 AND Tpd_RESTOTAmt = 0' AS [Remarks], Tpd_RESTOTHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_RESTOTHr > 0 AND Tpd_RESTOTAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_RESTNDHr > 0 AND Tpd_RESTNDAmt = 0' AS [Remarks], Tpd_RESTNDHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_RESTNDHr > 0 AND Tpd_RESTNDAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_RESTNDOTHr > 0 AND Tpd_RESTNDOTAmt = 0' AS [Remarks], Tpd_RESTNDOTHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_RESTNDOTHr > 0 AND Tpd_RESTNDOTAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_LEGHOLHr > 0 AND Tpd_LEGHOLAmt = 0' AS [Remarks], Tpd_LEGHOLHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_LEGHOLHr > 0 AND Tpd_LEGHOLAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_LEGHOLOTHr > 0 AND Tpd_LEGHOLOTAmt = 0' AS [Remarks], Tpd_LEGHOLOTHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_LEGHOLOTHr > 0 AND Tpd_LEGHOLOTAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_LEGHOLNDHr > 0 AND Tpd_LEGHOLNDAmt = 0' AS [Remarks], Tpd_LEGHOLNDHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_LEGHOLNDHr > 0 AND Tpd_LEGHOLNDAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_LEGHOLNDOTHr > 0 AND Tpd_LEGHOLNDOTAmt = 0' AS [Remarks], Tpd_LEGHOLNDOTHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_LEGHOLNDOTHr > 0 AND Tpd_LEGHOLNDOTAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_SPLHOLHr > 0 AND Tpd_SPLHOLAmt = 0' AS [Remarks], Tpd_SPLHOLHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_SPLHOLHr > 0 AND Tpd_SPLHOLAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_SPLHOLOTHr > 0 AND Tpd_SPLHOLOTAmt = 0' AS [Remarks], Tpd_SPLHOLOTHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_SPLHOLOTHr > 0 AND Tpd_SPLHOLOTAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_SPLHOLNDHr > 0 AND Tpd_SPLHOLNDAmt = 0' AS [Remarks], Tpd_SPLHOLNDHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_SPLHOLNDHr > 0 AND Tpd_SPLHOLNDAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_SPLHOLNDOTHr > 0 AND Tpd_SPLHOLNDOTAmt = 0' AS [Remarks], Tpd_SPLHOLNDOTHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_SPLHOLNDOTHr > 0 AND Tpd_SPLHOLNDOTAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_PSDHr > 0 AND Tpd_PSDAmt = 0' AS [Remarks], Tpd_PSDHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_PSDHr > 0 AND Tpd_PSDAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_PSDOTHr > 0 AND Tpd_PSDOTAmt = 0' AS [Remarks], Tpd_PSDOTHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_PSDOTHr > 0 AND Tpd_PSDOTAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_PSDNDHr > 0 AND Tpd_PSDNDAmt = 0' AS [Remarks], Tpd_PSDNDHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_PSDNDHr > 0 AND Tpd_PSDNDAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_PSDNDOTHr > 0 AND Tpd_PSDNDOTAmt = 0' AS [Remarks], Tpd_PSDNDOTHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_PSDNDOTHr > 0 AND Tpd_PSDNDOTAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_COMPHOLHr > 0 AND Tpd_COMPHOLAmt = 0' AS [Remarks], Tpd_COMPHOLHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_COMPHOLHr > 0 AND Tpd_COMPHOLAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_COMPHOLOTHr > 0 AND Tpd_COMPHOLOTAmt = 0' AS [Remarks], Tpd_COMPHOLOTHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_COMPHOLOTHr > 0 AND Tpd_COMPHOLOTAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_COMPHOLNDHr > 0 AND Tpd_COMPHOLNDAmt = 0' AS [Remarks], Tpd_COMPHOLNDHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_COMPHOLNDHr > 0 AND Tpd_COMPHOLNDAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_COMPHOLNDOTHr > 0 AND Tpd_COMPHOLNDOTAmt = 0' AS [Remarks], Tpd_COMPHOLNDOTHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_COMPHOLNDOTHr > 0 AND Tpd_COMPHOLNDOTAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_RESTLEGHOLHr > 0 AND Tpd_RESTLEGHOLAmt = 0' AS [Remarks], Tpd_RESTLEGHOLHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_RESTLEGHOLHr > 0 AND Tpd_RESTLEGHOLAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_RESTLEGHOLOTHr > 0 AND Tpd_RESTLEGHOLOTAmt = 0' AS [Remarks], Tpd_RESTLEGHOLOTHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_RESTLEGHOLOTHr > 0 AND Tpd_RESTLEGHOLOTAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_RESTLEGHOLNDHr > 0 AND Tpd_RESTLEGHOLNDAmt = 0' AS [Remarks], Tpd_RESTLEGHOLNDHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_RESTLEGHOLNDHr > 0 AND Tpd_RESTLEGHOLNDAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_RESTLEGHOLNDOTHr > 0 AND Tpd_RESTLEGHOLNDOTAmt = 0' AS [Remarks], Tpd_RESTLEGHOLNDOTHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_RESTLEGHOLNDOTHr > 0 AND Tpd_RESTLEGHOLNDOTAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_RESTSPLHOLHr > 0 AND Tpd_RESTSPLHOLAmt = 0' AS [Remarks], Tpd_RESTSPLHOLHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_RESTSPLHOLHr > 0 AND Tpd_RESTSPLHOLAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_RESTSPLHOLOTHr > 0 AND Tpd_RESTSPLHOLOTAmt = 0' AS [Remarks], Tpd_RESTSPLHOLOTHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_RESTSPLHOLOTHr > 0 AND Tpd_RESTSPLHOLOTAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_RESTSPLHOLNDHr > 0 AND Tpd_RESTSPLHOLNDAmt = 0' AS [Remarks], Tpd_RESTSPLHOLNDHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_RESTSPLHOLNDHr > 0 AND Tpd_RESTSPLHOLNDAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_RESTSPLHOLNDOTHr > 0 AND Tpd_RESTSPLHOLNDOTAmt = 0' AS [Remarks], Tpd_RESTSPLHOLNDOTHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_RESTSPLHOLNDOTHr > 0 AND Tpd_RESTSPLHOLNDOTAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_RESTCOMPHOLHr > 0 AND Tpd_RESTCOMPHOLAmt = 0' AS [Remarks], Tpd_RESTCOMPHOLHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_RESTCOMPHOLHr > 0 AND Tpd_RESTCOMPHOLAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_RESTCOMPHOLOTHr > 0 AND Tpd_RESTCOMPHOLOTAmt = 0' AS [Remarks], Tpd_RESTCOMPHOLOTHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_RESTCOMPHOLOTHr > 0 AND Tpd_RESTCOMPHOLOTAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_RESTCOMPHOLNDHr > 0 AND Tpd_RESTCOMPHOLNDAmt = 0' AS [Remarks], Tpd_RESTCOMPHOLNDHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_RESTCOMPHOLNDHr > 0 AND Tpd_RESTCOMPHOLNDAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_RESTCOMPHOLNDOTHr > 0 AND Tpd_RESTCOMPHOLNDOTAmt = 0' , Tpd_RESTCOMPHOLNDOTHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_RESTCOMPHOLNDOTHr > 0 AND Tpd_RESTCOMPHOLNDOTAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_RESTPSDHr > 0 AND Tpd_RESTPSDAmt = 0' AS [Remarks], Tpd_RESTPSDHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_RESTPSDHr > 0 AND Tpd_RESTPSDAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_RESTPSDOTHr > 0 AND Tpd_RESTPSDOTAmt = 0' AS [Remarks], Tpd_RESTPSDOTHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_RESTPSDOTHr > 0 AND Tpd_RESTPSDOTAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_RESTPSDNDHr > 0 AND Tpd_RESTPSDNDAmt = 0' AS [Remarks], Tpd_RESTPSDNDHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_RESTPSDNDHr > 0 AND Tpd_RESTPSDNDAmt = 0
                    AND Tpd_PayCycle = '{3}'

                UNION ALL

                SELECT Tpd_IDNo,Tpd_PayCycle, 'Tpd_RESTPSDNDOTHr > 0 AND Tpd_RESTPSDNDOTAmt = 0' AS [Remarks], Tpd_RESTPSDNDOTHr AS [Value 1] FROM T_EmpPayrollDtlYearly
                WHERE Tpd_RESTPSDNDOTHr > 0 AND Tpd_RESTPSDNDOTAmt = 0
                    AND Tpd_PayCycle = '{3}'

                    ) EmpPayrollDtlYearly
            INNER JOIN T_EmpPayrollYearly CALC ON Tpd_IDNo = CALC.Tpy_IDNo
					AND Tpd_PayCycle = CALC.Tpy_PayCycle
            INNER JOIN T_EmpPayrollRetro RETRO ON CALC.Tpy_IDNo = RETRO.Tpy_IDNo
                    AND CALC.Tpy_PayCycle = RETRO.Tpy_PayCycle
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_LTHr
                , 0
                , RETRO.Tpy_Paycycle + ' LTHr > 0 AND LTAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_LTHr > 0 AND Tpy_LTAmt = 0 
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_UTHr
                , 0
                , RETRO.Tpy_Paycycle + ' UTHr > 0 AND UTAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_UTHr > 0 AND Tpy_UTAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_UPLVHr
                , 0
                , RETRO.Tpy_Paycycle + ' UPLVHr > 0 AND UPLVAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_UPLVHr > 0 AND Tpy_UPLVAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_ABSLEGHOLHr
                , 0
                , RETRO.Tpy_Paycycle + ' ABSLEGHOLHr > 0 AND ABSLEGHOLAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_ABSLEGHOLHr > 0 AND Tpy_ABSLEGHOLAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_ABSSPLHOLHr
                , 0
                , RETRO.Tpy_Paycycle + ' ABSSPLHOLHr > 0 AND ABSSPLHOLAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_ABSSPLHOLHr > 0 AND Tpy_ABSSPLHOLAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_ABSCOMPHOLHr
                , 0
                , RETRO.Tpy_Paycycle + ' ABSCOMPHOLHr > 0 AND ABSCOMPHOLAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_ABSCOMPHOLHr > 0 AND Tpy_ABSCOMPHOLAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_ABSPSDHr
                , 0
                , RETRO.Tpy_Paycycle + ' ABSPSDHr > 0 AND ABSPSDAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_ABSPSDHr > 0 AND Tpy_ABSPSDAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_ABSOTHHOLHr
                , 0
                , RETRO.Tpy_Paycycle + ' ABSOTHHOLHr > 0 AND ABSOTHHOLAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_ABSOTHHOLHr > 0 AND Tpy_ABSOTHHOLAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_WDABSHr
                , 0
                , RETRO.Tpy_Paycycle + ' WDABSHr > 0 AND WDABSAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_WDABSHr > 0 AND Tpy_WDABSAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_LTUTMaxHr
                , 0
                , RETRO.Tpy_Paycycle + ' LTUTMaxHr > 0 AND LTUTMaxAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_LTUTMaxHr > 0 AND Tpy_LTUTMaxAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_ABSHr
                , 0
                , RETRO.Tpy_Paycycle + ' ABSHr > 0 AND ABSAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_ABSHr > 0 AND Tpy_ABSAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_REGHr
                , 0
                , RETRO.Tpy_Paycycle + ' REGHr > 0 AND REGAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_REGHr > 0 AND Tpy_REGAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_PDLVHr
                , 0
                , RETRO.Tpy_Paycycle + ' PDLVHr > 0 AND PDLVAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_PDLVHr > 0 AND Tpy_PDLVAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_PDLEGHOLHr
                , 0
                , RETRO.Tpy_Paycycle + ' PDLEGHOLHr > 0 AND PDLEGHOLAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_PDLEGHOLHr > 0 AND Tpy_PDLEGHOLAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_PDSPLHOLHr
                , 0
                , RETRO.Tpy_Paycycle + ' PDSPLHOLHr > 0 AND PDSPLHOLAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_PDSPLHOLHr > 0 AND Tpy_PDSPLHOLAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_PDCOMPHOLHr
                , 0
                , RETRO.Tpy_Paycycle + ' PDCOMPHOLHr > 0 AND PDCOMPHOLAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_PDCOMPHOLHr > 0 AND Tpy_PDCOMPHOLAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_PDPSDHr
                , 0
                , RETRO.Tpy_Paycycle + ' PDPSDHr > 0 AND PDPSDAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_PDPSDHr > 0 AND Tpy_PDPSDAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_PDOTHHOLHr
                , 0
                , RETRO.Tpy_Paycycle + ' PDOTHHOLHr > 0 AND PDOTHHOLAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_PDOTHHOLHr > 0 AND Tpy_PDOTHHOLAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_PDRESTLEGHOLHr
                , 0
                , RETRO.Tpy_Paycycle + ' PDRESTLEGHOLHr > 0 AND PDRESTLEGHOLAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_PDRESTLEGHOLHr > 0 AND Tpy_PDRESTLEGHOLAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_TotalREGHr
                , 0
                , RETRO.Tpy_Paycycle + ' TotalREGHr > 0 AND TotalREGAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_TotalREGHr > 0 AND Tpy_TotalREGAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_REGOTHr
                , 0
                , RETRO.Tpy_Paycycle + ' REGOTHr > 0 AND REGOTAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_REGOTHr > 0 AND Tpy_REGOTAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_REGNDHr
                , 0
                , RETRO.Tpy_Paycycle + ' REGNDHr > 0 AND REGNDAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_REGNDHr > 0 AND Tpy_REGNDAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_REGNDOTHr
                , 0
                , RETRO.Tpy_Paycycle + ' REGNDOTHr > 0 AND REGNDOTAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_REGNDOTHr > 0 AND Tpy_REGNDOTAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_RESTHr
                , 0
                , RETRO.Tpy_Paycycle + ' RESTHr > 0 AND RESTAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_RESTHr > 0 AND Tpy_RESTAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_RESTOTHr
                , 0
                , RETRO.Tpy_Paycycle + ' RESTOTHr > 0 AND RESTOTAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_RESTOTHr > 0 AND Tpy_RESTOTAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_RESTNDHr
                , 0
                , RETRO.Tpy_Paycycle + ' RESTNDHr > 0 AND RESTNDAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_RESTNDHr > 0 AND Tpy_RESTNDAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_RESTNDOTHr
                , 0
                , RETRO.Tpy_Paycycle + ' RESTNDOTHr > 0 AND RESTNDOTAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_RESTNDOTHr > 0 AND Tpy_RESTNDOTAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_LEGHOLHr
                , 0
                , RETRO.Tpy_Paycycle + ' LEGHOLHr > 0 AND LEGHOLAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_LEGHOLHr > 0 AND Tpy_LEGHOLAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_LEGHOLOTHr
                , 0
                , RETRO.Tpy_Paycycle + ' LEGHOLOTHr > 0 AND LEGHOLOTAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_LEGHOLOTHr > 0 AND Tpy_LEGHOLOTAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_LEGHOLNDHr
                , 0
                , RETRO.Tpy_Paycycle + ' LEGHOLNDHr > 0 AND LEGHOLNDAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_LEGHOLNDHr > 0 AND Tpy_LEGHOLNDAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_LEGHOLNDOTHr
                , 0
                , RETRO.Tpy_Paycycle + ' LEGHOLNDOTHr > 0 AND LEGHOLNDOTAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_LEGHOLNDOTHr > 0 AND Tpy_LEGHOLNDOTAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_SPLHOLHr
                , 0
                , RETRO.Tpy_Paycycle + ' SPLHOLHr > 0 AND SPLHOLAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_SPLHOLHr > 0 AND Tpy_SPLHOLAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_SPLHOLOTHr
                , 0
                , RETRO.Tpy_Paycycle + ' SPLHOLOTHr > 0 AND SPLHOLOTAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_SPLHOLOTHr > 0 AND Tpy_SPLHOLOTAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_SPLHOLNDHr
                , 0
                , RETRO.Tpy_Paycycle + ' SPLHOLNDHr > 0 AND SPLHOLNDAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_SPLHOLNDHr > 0 AND Tpy_SPLHOLNDAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_SPLHOLNDOTHr
                , 0
                , RETRO.Tpy_Paycycle + ' SPLHOLNDOTHr > 0 AND SPLHOLNDOTAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_SPLHOLNDOTHr > 0 AND Tpy_SPLHOLNDOTAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_PSDHr
                , 0
                , RETRO.Tpy_Paycycle + ' PSDHr > 0 AND PSDAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_PSDHr > 0 AND Tpy_PSDAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_PSDOTHr
                , 0
                , RETRO.Tpy_Paycycle + ' PSDOTHr > 0 AND PSDOTAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_PSDOTHr > 0 AND Tpy_PSDOTAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_PSDNDHr
                , 0
                , RETRO.Tpy_Paycycle + ' PSDNDHr > 0 AND PSDNDAmt = 0'
                , 1
                , '{4}'
                , GETDATE() 
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_PSDNDHr > 0 AND Tpy_PSDNDAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_PSDNDOTHr
                , 0
                , RETRO.Tpy_Paycycle + ' PSDNDOTHr > 0 AND PSDNDOTAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_PSDNDOTHr > 0 AND Tpy_PSDNDOTAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_COMPHOLHr
                , 0
                , RETRO.Tpy_Paycycle + ' COMPHOLHr > 0 AND COMPHOLAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_COMPHOLHr > 0 AND Tpy_COMPHOLAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_COMPHOLOTHr
                , 0
                , RETRO.Tpy_Paycycle + ' COMPHOLOTHr > 0 AND COMPHOLOTAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_COMPHOLOTHr > 0 AND Tpy_COMPHOLOTAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_COMPHOLNDHr
                , 0
                , RETRO.Tpy_Paycycle + ' COMPHOLNDHr > 0 AND COMPHOLNDAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_COMPHOLNDHr > 0 AND Tpy_COMPHOLNDAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_COMPHOLNDOTHr
                , 0
                , RETRO.Tpy_Paycycle + ' COMPHOLNDOTHr > 0 AND COMPHOLNDOTAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_COMPHOLNDOTHr > 0 AND Tpy_COMPHOLNDOTAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_RESTLEGHOLHr
                , 0
                , RETRO.Tpy_Paycycle + ' RESTLEGHOLHr > 0 AND RESTLEGHOLAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_RESTLEGHOLHr > 0 AND Tpy_RESTLEGHOLAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_RESTLEGHOLOTHr
                , 0
                , RETRO.Tpy_Paycycle + ' RESTLEGHOLOTHr > 0 AND RESTLEGHOLOTAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_RESTLEGHOLOTHr > 0 AND Tpy_RESTLEGHOLOTAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_RESTLEGHOLNDHr
                , 0
                , RETRO.Tpy_Paycycle + ' RESTLEGHOLNDHr > 0 AND RESTLEGHOLNDAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_RESTLEGHOLNDHr > 0 AND Tpy_RESTLEGHOLNDAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_RESTLEGHOLNDOTHr
                , 0
                , RETRO.Tpy_Paycycle + ' RESTLEGHOLNDOTHr > 0 AND RESTLEGHOLNDOTAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_RESTLEGHOLNDOTHr > 0 AND Tpy_RESTLEGHOLNDOTAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_RESTSPLHOLHr
                , 0
                , RETRO.Tpy_Paycycle + ' RESTSPLHOLHr > 0 AND RESTSPLHOLAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_RESTSPLHOLHr > 0 AND Tpy_RESTSPLHOLAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_RESTSPLHOLOTHr
                , 0
                , RETRO.Tpy_Paycycle + ' RESTSPLHOLOTHr > 0 AND RESTSPLHOLOTAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_RESTSPLHOLOTHr > 0 AND Tpy_RESTSPLHOLOTAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_RESTSPLHOLNDHr
                , 0
                , RETRO.Tpy_Paycycle + ' RESTSPLHOLNDHr > 0 AND RESTSPLHOLNDAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_RESTSPLHOLNDHr > 0 AND Tpy_RESTSPLHOLNDAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_RESTSPLHOLNDOTHr
                , 0
                , RETRO.Tpy_Paycycle + ' RESTSPLHOLNDOTHr > 0 AND RESTSPLHOLNDOTAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_RESTSPLHOLNDOTHr > 0 AND Tpy_RESTSPLHOLNDOTAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_RESTCOMPHOLHr
                , 0
                , RETRO.Tpy_Paycycle + ' RESTCOMPHOLHr > 0 AND RESTCOMPHOLAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_RESTCOMPHOLHr > 0 AND Tpy_RESTCOMPHOLAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_RESTCOMPHOLOTHr
                , 0
                , RETRO.Tpy_Paycycle + ' RESTCOMPHOLOTHr > 0 AND RESTCOMPHOLOTAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_RESTCOMPHOLOTHr > 0 AND Tpy_RESTCOMPHOLOTAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_RESTCOMPHOLNDHr
                , 0
                , RETRO.Tpy_Paycycle + ' RESTCOMPHOLNDHr > 0 AND RESTCOMPHOLNDAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_RESTCOMPHOLNDHr > 0 AND Tpy_RESTCOMPHOLNDAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_RESTCOMPHOLNDOTHr
                , 0
                , RETRO.Tpy_Paycycle + ' RESTCOMPHOLNDOTHr > 0 AND RESTCOMPHOLNDOTAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_RESTCOMPHOLNDOTHr > 0 AND Tpy_RESTCOMPHOLNDOTAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_RESTPSDHr
                , 0
                , RETRO.Tpy_Paycycle + ' RESTPSDHr > 0 AND RESTPSDAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_RESTPSDHr > 0 AND Tpy_RESTPSDAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_RESTPSDOTHr
                , 0
                , RETRO.Tpy_Paycycle + ' RESTPSDOTHr > 0 AND RESTPSDOTAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_RESTPSDOTHr > 0 AND Tpy_RESTPSDOTAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_RESTPSDNDHr
                , 0
                , RETRO.Tpy_Paycycle + ' RESTPSDNDHr > 0 AND RESTPSDNDAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_RESTPSDNDHr > 0 AND Tpy_RESTPSDNDAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpy_IDNo
                , 'AE'
                , '{3}'
                , Tpy_RESTPSDNDOTHr
                , 0
                , RETRO.Tpy_Paycycle + ' RESTPSDNDOTHr > 0 AND RESTPSDNDOTAmt = 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                AND Tpy_RESTPSDNDOTHr > 0 AND Tpy_RESTPSDNDOTAmt = 0
             @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_RGRetroAmt + RETRO.Tpy_HLRetroAmt + RETRO.Tpy_LVRetroAmt 
                , (RETRO.Tpy_SalaryRate - CALC.Tpy_SalaryRate)/2.000
                , RETRO.Tpy_Paycycle + ' Regular + Holiday + Leave > Half Month' 
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO 
            INNER JOIN Udv_Payroll CALC ON CALC.Tpy_IDNo = RETRO.Tpy_IDNo
	            AND CALC.Tpy_PayCycle = RETRO.Tpy_PayCycle
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND  RETRO.Tpy_RGRetroAmt + RETRO.Tpy_HLRetroAmt + RETRO.Tpy_LVRetroAmt > (RETRO.Tpy_SalaryRate-CALC.Tpy_SalaryRate)/2.000
             @CONDITIONS 

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_TotalOTNDAmt
                , CALC.Tpy_TotalOTNDAmt
                , RETRO.Tpy_Paycycle + ' (RETRO TotalOTNDAmt - PAYROLL TotalOTNDAmt) < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            INNER JOIN Udv_Payroll CALC ON CALC.Tpy_IDNo = RETRO.Tpy_IDNo
	            AND CALC.Tpy_PayCycle = RETRO.Tpy_PayCycle
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND (RETRO.Tpy_TotalOTNDAmt - CALC.Tpy_TotalOTNDAmt) < 0
             @CONDITIONS  

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_LTVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' LTVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_LTVarAmt < 0
                @CONDITIONS 

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_UTVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' UTVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_UTVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_UPLVVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' UPLVVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_UPLVVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_ABSLEGHOLVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' ABSLEGHOLVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_ABSLEGHOLVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_ABSSPLHOLVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' ABSSPLHOLVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_ABSSPLHOLVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_ABSCOMPHOLVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' ABSCOMPHOLVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_ABSCOMPHOLVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_ABSPSDVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' ABSPSDVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_ABSPSDVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_ABSOTHHOLVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' ABSOTHHOLVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_ABSOTHHOLVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_ABSOTHHOLVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' ABSOTHHOLVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_ABSOTHHOLVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_WDABSVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' WDABSVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_WDABSVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_LTUTMaxVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' LTUTMaxVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_LTUTMaxVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_ABSVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' ABSVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_ABSVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_REGVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' REGVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_REGVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_PDLVVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' PDLVVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_PDLVVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_PDLEGHOLVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' PDLEGHOLVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_PDLEGHOLVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_PDSPLHOLVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' PDSPLHOLVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_PDSPLHOLVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_PDCOMPHOLVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' PDCOMPHOLVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_PDCOMPHOLVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_PDPSDVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' PDPSDVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_PDPSDVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_PDOTHHOLVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' PDOTHHOLVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_PDOTHHOLVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_PDRESTLEGHOLVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' PDRESTLEGHOLVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_PDRESTLEGHOLVarAmt < 0
                @CONDITIONS

            --UNION ALL

            --SELECT '{5}'
            --    , RETRO.Tpy_IDNo
            --    , 'AE'
            --    , '{3}'
            --    , RETRO.Tpy_TotalREGVarAmt
            --    , 0
            --   , 'TotalREGVarAmt < 0'
            --    , 1
            --    , '{4}'
            --    , GETDATE()
            --FROM T_EmpPayrollRetro RETRO
            --WHERE  Tpy_RetroPayCycle = '{3}'
            --    AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
            --    AND RETRO.Tpy_TotalREGVarAmt < 0
            --    @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_REGOTVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' REGOTVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_REGOTVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_REGNDVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' REGNDVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_REGNDVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_REGNDOTVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' REGNDOTVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_REGNDOTVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_RESTVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' RESTVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_RESTVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_RESTOTVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' RESTOTVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_RESTOTVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_RESTNDVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' RESTNDVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_RESTNDVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_RESTNDOTVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' RESTNDOTVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_RESTNDOTVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_LEGHOLVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' LEGHOLVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_LEGHOLVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_LEGHOLOTVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' LEGHOLOTVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_LEGHOLOTVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_LEGHOLNDVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' LEGHOLNDVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_LEGHOLNDVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_LEGHOLNDOTVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' LEGHOLNDOTVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_LEGHOLNDOTVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_SPLHOLVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' SPLHOLVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_SPLHOLVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_SPLHOLOTVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' SPLHOLOTVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_SPLHOLOTVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_SPLHOLNDVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' SPLHOLNDVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_SPLHOLNDVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_SPLHOLNDOTVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' SPLHOLNDOTVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_SPLHOLNDOTVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_PSDVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' PSDVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_PSDVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_PSDOTVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' PSDOTVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_PSDOTVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_PSDNDVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' PSDNDVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_PSDNDVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_PSDNDOTVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' PSDNDOTVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_PSDNDOTVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_COMPHOLVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' COMPHOLVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_COMPHOLVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_COMPHOLOTVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' COMPHOLOTVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_COMPHOLOTVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_COMPHOLNDVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' COMPHOLNDVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_COMPHOLNDVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_COMPHOLNDOTVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' COMPHOLNDOTVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_COMPHOLNDOTVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_RESTLEGHOLVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' RESTLEGHOLVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_RESTLEGHOLVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_RESTLEGHOLOTVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' RESTLEGHOLOTVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_RESTLEGHOLOTVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_RESTLEGHOLNDVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' RESTLEGHOLNDVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_RESTLEGHOLNDVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_RESTLEGHOLNDOTVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' RESTLEGHOLNDOTVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_RESTLEGHOLNDOTVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_RESTSPLHOLVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' RESTSPLHOLVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_RESTSPLHOLVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_RESTSPLHOLNDVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' RESTSPLHOLNDVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_RESTSPLHOLNDVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_RESTSPLHOLNDOTVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' RESTSPLHOLNDOTVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_RESTSPLHOLNDOTVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_RESTCOMPHOLVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' RESTCOMPHOLVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_RESTCOMPHOLVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_RESTCOMPHOLOTVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' RESTCOMPHOLOTVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_RESTCOMPHOLOTVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_RESTCOMPHOLNDVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' RESTCOMPHOLNDVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_RESTCOMPHOLNDVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_RESTCOMPHOLNDOTVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' RESTCOMPHOLNDOTVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_RESTCOMPHOLNDOTVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_RESTPSDVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' RESTPSDVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_RESTPSDVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_RESTPSDOTVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' RESTPSDOTVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_RESTPSDOTVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_RESTPSDNDVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' RESTPSDNDVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_RESTPSDNDVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , RETRO.Tpy_IDNo
                , 'AE'
                , '{3}'
                , RETRO.Tpy_RESTPSDNDVarAmt
                , 0
                , RETRO.Tpy_Paycycle + ' RESTPSDNDVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollRetro RETRO
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpy_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR RETRO.Tpy_IDNo = '{2}')
                AND RETRO.Tpy_RESTPSDNDVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpm_IDNo
                , 'AE'
                , '{3}'
                , Tpm_Misc1VarAmt
                , 0
                , Tpm_PayCycle + ' Misc1VarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollMiscRetro
            INNER JOIN T_EmpPayrollRetro RETRO ON RETRO.Tpy_IDNo = Tpm_IDNo
	            AND RETRO.Tpy_RetroPayCycle = Tpm_RetroPayCycle
	            AND RETRO.Tpy_PayCycle = Tpm_PayCycle
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpm_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpm_IDNo = '{2}')
                AND Tpm_Misc1VarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpm_IDNo
                , 'AE'
                , '{3}'
                , Tpm_Misc1OTVarAmt
                , 0
                , Tpm_PayCycle + ' Misc1OTVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollMiscRetro
            INNER JOIN T_EmpPayrollRetro RETRO ON RETRO.Tpy_IDNo = Tpm_IDNo
	            AND RETRO.Tpy_RetroPayCycle = Tpm_RetroPayCycle
	            AND RETRO.Tpy_PayCycle = Tpm_PayCycle
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpm_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpm_IDNo = '{2}')
                AND Tpm_Misc1OTVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpm_IDNo
                , 'AE'
                , '{3}'
                , Tpm_Misc1NDVarAmt
                , 0
                , Tpm_PayCycle + ' Misc1NDVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollMiscRetro
            INNER JOIN T_EmpPayrollRetro RETRO ON RETRO.Tpy_IDNo = Tpm_IDNo
	            AND RETRO.Tpy_RetroPayCycle = Tpm_RetroPayCycle
	            AND RETRO.Tpy_PayCycle = Tpm_PayCycle
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpm_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpm_IDNo = '{2}')
                AND Tpm_Misc1NDVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpm_IDNo
                , 'AE'
                , '{3}'
                , Tpm_Misc1NDOTVarAmt
                , 0
                , Tpm_PayCycle + ' Misc1NDOTVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollMiscRetro 
            INNER JOIN T_EmpPayrollRetro RETRO ON RETRO.Tpy_IDNo = Tpm_IDNo
	            AND RETRO.Tpy_RetroPayCycle = Tpm_RetroPayCycle
	            AND RETRO.Tpy_PayCycle = Tpm_PayCycle
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpm_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpm_IDNo = '{2}')
                AND Tpm_Misc1NDOTVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpm_IDNo
                , 'AE'
                , '{3}'
                , Tpm_Misc2VarAmt
                , 0
                , Tpm_PayCycle + ' Misc2VarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollMiscRetro 
            INNER JOIN T_EmpPayrollRetro RETRO ON RETRO.Tpy_IDNo = Tpm_IDNo
	            AND RETRO.Tpy_RetroPayCycle = Tpm_RetroPayCycle
	            AND RETRO.Tpy_PayCycle = Tpm_PayCycle
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpm_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpm_IDNo = '{2}')
                AND Tpm_Misc2VarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpm_IDNo
                , 'AE'
                , '{3}'
                , Tpm_Misc2OTVarAmt
                , 0
                , Tpm_PayCycle + ' Misc2OTVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollMiscRetro 
            INNER JOIN T_EmpPayrollRetro RETRO ON RETRO.Tpy_IDNo = Tpm_IDNo
	            AND RETRO.Tpy_RetroPayCycle = Tpm_RetroPayCycle
	            AND RETRO.Tpy_PayCycle = Tpm_PayCycle
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpm_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpm_IDNo = '{2}')
                AND Tpm_Misc2OTVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpm_IDNo
                , 'AE'
                , '{3}'
                , Tpm_Misc2NDVarAmt
                , 0
                , Tpm_PayCycle + ' Misc2NDVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollMiscRetro 
            INNER JOIN T_EmpPayrollRetro RETRO ON RETRO.Tpy_IDNo = Tpm_IDNo
	            AND RETRO.Tpy_RetroPayCycle = Tpm_RetroPayCycle
	            AND RETRO.Tpy_PayCycle = Tpm_PayCycle
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpm_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpm_IDNo = '{2}')
                AND Tpm_Misc2NDVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpm_IDNo
                , 'AE'
                , '{3}'
                , Tpm_Misc2NDOTVarAmt
                , 0
                , Tpm_PayCycle + ' Misc2NDOTVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollMiscRetro 
            INNER JOIN T_EmpPayrollRetro RETRO ON RETRO.Tpy_IDNo = Tpm_IDNo
	            AND RETRO.Tpy_RetroPayCycle = Tpm_RetroPayCycle
	            AND RETRO.Tpy_PayCycle = Tpm_PayCycle
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpm_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpm_IDNo = '{2}')
                AND Tpm_Misc2NDOTVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpm_IDNo
                , 'AE'
                , '{3}'
                , Tpm_Misc3VarAmt
                , 0
                , Tpm_PayCycle + ' Misc3VarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollMiscRetro 
            INNER JOIN T_EmpPayrollRetro RETRO ON RETRO.Tpy_IDNo = Tpm_IDNo
	            AND RETRO.Tpy_RetroPayCycle = Tpm_RetroPayCycle
	            AND RETRO.Tpy_PayCycle = Tpm_PayCycle
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpm_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpm_IDNo = '{2}')
                AND Tpm_Misc3VarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpm_IDNo
                , 'AE'
                , '{3}'
                , Tpm_Misc3OTVarAmt
                , 0
                , Tpm_PayCycle + ' Misc3OTVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollMiscRetro 
            INNER JOIN T_EmpPayrollRetro RETRO ON RETRO.Tpy_IDNo = Tpm_IDNo
	            AND RETRO.Tpy_RetroPayCycle = Tpm_RetroPayCycle
	            AND RETRO.Tpy_PayCycle = Tpm_PayCycle
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpm_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpm_IDNo = '{2}')
                AND Tpm_Misc3OTVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpm_IDNo
                , 'AE'
                , '{3}'
                , Tpm_Misc3NDVarAmt
                , 0
                , Tpm_PayCycle + ' Misc3NDVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollMiscRetro 
            INNER JOIN T_EmpPayrollRetro RETRO ON RETRO.Tpy_IDNo = Tpm_IDNo
	            AND RETRO.Tpy_RetroPayCycle = Tpm_RetroPayCycle
	            AND RETRO.Tpy_PayCycle = Tpm_PayCycle
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpm_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpm_IDNo = '{2}')
                AND Tpm_Misc3NDVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpm_IDNo
                , 'AE'
                , '{3}'
                , Tpm_Misc3NDOTVarAmt
                , 0
                , Tpm_PayCycle + ' Misc3NDOTVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollMiscRetro 
            INNER JOIN T_EmpPayrollRetro RETRO ON RETRO.Tpy_IDNo = Tpm_IDNo
	            AND RETRO.Tpy_RetroPayCycle = Tpm_RetroPayCycle
	            AND RETRO.Tpy_PayCycle = Tpm_PayCycle
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpm_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpm_IDNo = '{2}')
                AND Tpm_Misc3NDOTVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpm_IDNo
                , 'AE'
                , '{3}'
                , Tpm_Misc4VarAmt
                , 0
                , Tpm_PayCycle + ' Misc4VarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollMiscRetro 
            INNER JOIN T_EmpPayrollRetro RETRO ON RETRO.Tpy_IDNo = Tpm_IDNo
	            AND RETRO.Tpy_RetroPayCycle = Tpm_RetroPayCycle
	            AND RETRO.Tpy_PayCycle = Tpm_PayCycle
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpm_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpm_IDNo = '{2}')
                AND Tpm_Misc4VarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpm_IDNo
                , 'AE'
                , '{3}'
                , Tpm_Misc4OTVarAmt
                , 0
                , Tpm_PayCycle + ' Misc4OTVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollMiscRetro 
            INNER JOIN T_EmpPayrollRetro RETRO ON RETRO.Tpy_IDNo = Tpm_IDNo
	            AND RETRO.Tpy_RetroPayCycle = Tpm_RetroPayCycle
	            AND RETRO.Tpy_PayCycle = Tpm_PayCycle
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpm_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpm_IDNo = '{2}')
                AND Tpm_Misc4OTVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpm_IDNo
                , 'AE'
                , '{3}'
                , Tpm_Misc4NDVarAmt
                , 0
                , Tpm_PayCycle + ' Misc4NDVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollMiscRetro 
            INNER JOIN T_EmpPayrollRetro RETRO ON RETRO.Tpy_IDNo = Tpm_IDNo
	            AND RETRO.Tpy_RetroPayCycle = Tpm_RetroPayCycle
	            AND RETRO.Tpy_PayCycle = Tpm_PayCycle
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpm_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpm_IDNo = '{2}')
                AND Tpm_Misc4NDVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpm_IDNo
                , 'AE'
                , '{3}'
                , Tpm_Misc4NDOTVarAmt
                , 0
                , Tpm_PayCycle + ' Misc4NDOTVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollMiscRetro 
            INNER JOIN T_EmpPayrollRetro RETRO ON RETRO.Tpy_IDNo = Tpm_IDNo
	            AND RETRO.Tpy_RetroPayCycle = Tpm_RetroPayCycle
	            AND RETRO.Tpy_PayCycle = Tpm_PayCycle
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpm_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpm_IDNo = '{2}')
                AND Tpm_Misc4NDOTVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpm_IDNo
                , 'AE'
                , '{3}'
                , Tpm_Misc5VarAmt
                , 0
                , Tpm_PayCycle + ' Misc5VarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollMiscRetro 
            INNER JOIN T_EmpPayrollRetro RETRO ON RETRO.Tpy_IDNo = Tpm_IDNo
	            AND RETRO.Tpy_RetroPayCycle = Tpm_RetroPayCycle
	            AND RETRO.Tpy_PayCycle = Tpm_PayCycle
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpm_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpm_IDNo = '{2}')
                AND Tpm_Misc5VarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpm_IDNo
                , 'AE'
                , '{3}'
                , Tpm_Misc5OTVarAmt
                , 0
                , Tpm_PayCycle + ' Misc5OTVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollMiscRetro 
            INNER JOIN T_EmpPayrollRetro RETRO ON RETRO.Tpy_IDNo = Tpm_IDNo
	            AND RETRO.Tpy_RetroPayCycle = Tpm_RetroPayCycle
	            AND RETRO.Tpy_PayCycle = Tpm_PayCycle
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpm_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpm_IDNo = '{2}')
                AND Tpm_Misc5OTVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpm_IDNo
                , 'AE'
                , '{3}'
                , Tpm_Misc5NDVarAmt
                , 0
                , Tpm_PayCycle + ' Misc5NDVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollMiscRetro 
            INNER JOIN T_EmpPayrollRetro RETRO ON RETRO.Tpy_IDNo = Tpm_IDNo
	            AND RETRO.Tpy_RetroPayCycle = Tpm_RetroPayCycle
	            AND RETRO.Tpy_PayCycle = Tpm_PayCycle
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpm_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpm_IDNo = '{2}')
                AND Tpm_Misc5NDVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpm_IDNo
                , 'AE'
                , '{3}'
                , Tpm_Misc5NDOTVarAmt
                , 0
                , Tpm_PayCycle + ' Misc5NDOTVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollMiscRetro 
            INNER JOIN T_EmpPayrollRetro RETRO ON RETRO.Tpy_IDNo = Tpm_IDNo
	            AND RETRO.Tpy_RetroPayCycle = Tpm_RetroPayCycle
	            AND RETRO.Tpy_PayCycle = Tpm_PayCycle
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpm_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpm_IDNo = '{2}')
                AND Tpm_Misc5NDOTVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpm_IDNo
                , 'AE'
                , '{3}'
                , Tpm_Misc6VarAmt
                , 0
                , Tpm_PayCycle + ' Misc6VarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollMiscRetro 
            INNER JOIN T_EmpPayrollRetro RETRO ON RETRO.Tpy_IDNo = Tpm_IDNo
	            AND RETRO.Tpy_RetroPayCycle = Tpm_RetroPayCycle
	            AND RETRO.Tpy_PayCycle = Tpm_PayCycle
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpm_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpm_IDNo = '{2}')
                AND Tpm_Misc6VarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpm_IDNo
                , 'AE'
                , '{3}'
                , Tpm_Misc6OTVarAmt
                , 0
                , Tpm_PayCycle + ' Misc6OTVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollMiscRetro 
            INNER JOIN T_EmpPayrollRetro RETRO ON RETRO.Tpy_IDNo = Tpm_IDNo
	            AND RETRO.Tpy_RetroPayCycle = Tpm_RetroPayCycle
	            AND RETRO.Tpy_PayCycle = Tpm_PayCycle
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpm_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpm_IDNo = '{2}')
                AND Tpm_Misc6OTVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpm_IDNo
                , 'AE'
                , '{3}'
                , Tpm_Misc6NDVarAmt
                , 0
                , Tpm_PayCycle + ' Misc6NDVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollMiscRetro 
            INNER JOIN T_EmpPayrollRetro RETRO ON RETRO.Tpy_IDNo = Tpm_IDNo
	            AND RETRO.Tpy_RetroPayCycle = Tpm_RetroPayCycle
	            AND RETRO.Tpy_PayCycle = Tpm_PayCycle
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpm_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpm_IDNo = '{2}')
                AND Tpm_Misc6NDVarAmt < 0
                @CONDITIONS

            UNION ALL

            SELECT '{5}'
                , Tpm_IDNo
                , 'AE'
                , '{3}'
                , Tpm_Misc6NDOTVarAmt
                , 0
                , Tpm_PayCycle + ' Misc6NDOTVarAmt < 0'
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollMiscRetro 
            INNER JOIN T_EmpPayrollRetro RETRO ON RETRO.Tpy_IDNo = Tpm_IDNo
	            AND RETRO.Tpy_RetroPayCycle = Tpm_RetroPayCycle
	            AND RETRO.Tpy_PayCycle = Tpm_PayCycle
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpm_RetroPayCycle = '{3}'
                AND ('{2}' = '' OR Tpm_IDNo = '{2}')
                AND Tpm_Misc6NDOTVarAmt < 0
                @CONDITIONS
                    	
           "
           , centralProfile, companyCode, EmployeeID, PayCycle, UserLogin, menucode);
            #endregion
            query = query.Replace("@CONDITIONS", condition);
            query = query.Replace("@USERCOSTCENTERACCESSCONDITION", (new HRCReportsBL()).UserCostCenterAccessTmpQuery(ProfileCode, "PAYROLL", UserLogin, "RETRO.Tpy_CostcenterCode", "RETRO.Tpy_PayrollGroup", "RETRO.Tpy_EmploymentStatus", "RETRO.Tpy_PayrollType", companyCode, centralProfile, false));
            dalHelper.ExecuteNonQuery(query);
        }

    }
}
