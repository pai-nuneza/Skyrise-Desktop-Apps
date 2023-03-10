using System;
using System.Data;
using CommonLibrary;
using Payroll.DAL;

namespace Payroll.BLogic
{
    public struct structPayPeriodGrossFormula
    {
        public string strPayPeriod;
        public string strAmount;

        public structPayPeriodGrossFormula(string PayPeriod, string Formula)
        {
            strPayPeriod = PayPeriod;
            strAmount = Formula;
        }
    }

    public struct structEmployeeDeduction
    {
        public string strEmployeeId;
        public string strDeductionCodes;

        public structEmployeeDeduction(string EmployeeId, string DeductionCodes)
        {
            strEmployeeId = EmployeeId;
            strDeductionCodes = DeductionCodes;
        }
    }

    public class NewLastPayComputationBL : BaseBL
    {
        #region <Global Variables>
        DALHelper dal;
        DALHelper dalCentral;
        SystemCycleProcessingBL SystemCycleProcessingBL = new SystemCycleProcessingBL();
        CommonBL commonBL = new CommonBL();
        NewLaborHoursGenerationBL NewLaborHoursGenerationBL = new NewLaborHoursGenerationBL();
        NewPayrollCalculationBL NewPayrollCalculationBL = new NewPayrollCalculationBL();
        NewAlphalistProcessingBL NewAlphalistProcessingBL = new NewAlphalistProcessingBL();
        BonusComputation13thMonthBL BonusComputation13thMonthBL = new BonusComputation13thMonthBL();
        LeaveRefundBL LeaveRefundBL = new LeaveRefundBL();

        //Flags and parameters
        decimal MDIVISOR = 0;
        decimal MINNETPAY = 0;
        decimal M13EXCLTAX = 0;
        decimal HRLYRTEDEC = 0;

        string MaxTaxAnnual;
        string MaxSSSPremPayCycle;

        string LVHRENTRY = "";
        //string FPREGPAYRULE = "";
        //string FPFIXALLOWRULE = "";
        string FPLVREFUNDRULE       = "";  //F | C
        string FPLVREFUNDFORMULA    = "";
        //string FPLABORHOURSRULE = "";  //Not in use
        string FPRETIREMENTFORMULA  = "";
        string FPRETIREMENTTENURE   = "";
        //string FPSEPARATERETIREMENT = "";
        string FP13THMONTHNORMAL    = "";
        string FPLVREFUNDDIVIDEND   = "";
        string FPLVREFUNDDIVISOR    = "";
        string FPPROCESSYEARLVE     = "";
        string FPPROCESSYEARBON     = "";
        string FPPROCESSYEARTAX     = "";
        DataTable FPDEDUCTIONGROUP;
        DataTable FPRETIREMENTCODE;
        string FPFIXAMOUNT          = "";
        string FPFIXALLOWDIVIDEND   = "";
        string FPFIXALLOWDIVISOR    = "";

        //Constants
        public int FILLERCNT = 6;

        //Miscellaneous
        string ProcessPayrollPeriod     = "";
        string PayrollStart             = "";
        string PayrollEnd               = "";
        string PayrollAssume13thMonth   = "";
        string LoginUser                = "";
        string CompanyCode              = "";
        string CentralProfile           = "";
        string MenuCode                 = "";
        string DBCollation              = "";
        bool bHasDayCodeExt             = false;

        DataTable dtEmployeeFinalPay = null;
        DataTable dtBonusMaster = null;

        DataTable dtTemp;
        DataRow[] drTemp;
 
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

        #region Event handlers for last pay calculation process
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

        public NewLastPayComputationBL()
        {
        }

        public NewLastPayComputationBL(DALHelper dalHelper)
        {
            this.dal = dalHelper;
        }

        public void ComputeLastPay(
            bool ProcessAll
            , string PayrollPeriod
            , string NormalPayrollPeriod
            , string[] EmployeeList
            , bool PostTimeBasedAllowance
            , string UserLogin
            , string companyCode
            , string centralProfile
            , string profileCode
            , string menuCode
            , string dbCollation
            , DALHelper dalHelper)
        {
            #region Variables
            //For Labor Hours
            DataTable dtHoldPayPeriods;
            DataTable dtValidPayPeriods;
            DataTable dtPayCalcRec;
            DataTable dtPayCalcExt2Rec;
            DataRow[] drArrPayCycleHold = null;
            DataRow[] drArEmpPayCycleSeparation = null;
            DataRow[] drArrEmpDetails;
            string strEmployeeListCommaDelimited;
            string strCycIndicator;
            string strLogLedgerTableName = "T_EmpTimeRegister";
            string strTempQuery;

            //For YTD
            string YTDYear;

            //For Deduction
            string insertDeductionDetailSepExtension;
            decimal dWTaxAmt                    = 0;
            decimal dWTaxRef                    = 0;
            decimal dPartialDeduction           = 0;
            decimal dCurrentNetPay              = 0;
            decimal dMinimumNetPay              = 0;
            decimal dOtherDeduction             = 0;
            decimal dTotalDeduction             = 0;
            decimal deductionAmt                = 0;
            decimal dNetPayAmt                  = 0;
            decimal dGrossIncomeAmt             = 0;
            DataSet dsDeductions;

            //bool bCompute13thMonthPay = false;
            bool bComputeLeaveRefund            = false;
            bool bComputeFixAllowance           = false;
            bool bComputeRetirement             = false;
            bool bComputeHoldSalary             = false;
            bool bRetirementSeparateFromLastPay = false;
            bool bPayERShare                    = false;
            string strFixAllowRule              = "";
            object strSepDate;
            string strPayrollType               = "";
            string strEmploymentStatus          = "";
            string strRetirementType            = "";
            string strRetirementIncomeCode      = "";
            string strRetirememntTaxClass       = "";
            string strAge                       = "";
            string strDeductions                = "";
            string strTaxMethod                 = "";
            //string strTaxSchedule               = "";

            //For Retirement
            decimal dTenureYears                = 0;
            decimal dRetirementSalary           = 0;
            decimal dRetirementTaxAmount        = 0;
            decimal dRetirementAmount           = 0;
            decimal dRetirementRate             = 0;
            decimal dOtherRetirementTaxAmount   = 0;
            decimal dOtherRetirementNontaxAmount = 0;
            decimal dTotalRetirementTaxAmount   = 0;

            int dRetirementTaxBracket = 0;

            //string curEmployeeId = "";
            string PayCycleSeparation           = "";
            string PayrollSeparationPayCycle    = "";
            string MaxTimeRegisterPayCycle      = "";
            bool bComputeTax                    = false;

            #endregion

            try
            {
                #region Initialization
                this.dal                = dalHelper;
                ProcessPayrollPeriod    = PayrollPeriod;
                LoginUser               = UserLogin;
                CompanyCode             = companyCode;
                CentralProfile          = centralProfile;
                MenuCode                = menuCode;
                DBCollation             = dbCollation;
                commonBL = new CommonBL();

                DataSet dsPayPeriod = commonBL.GetPayCycleDtl(ProcessPayrollPeriod, companyCode, centralProfile, dal);
                if (dsPayPeriod.Tables.Count > 0 && dsPayPeriod.Tables[0].Rows.Count > 0)
                {
                    PayrollStart = dsPayPeriod.Tables[0].Rows[0]["Start Cycle"].ToString();
                    PayrollEnd = dsPayPeriod.Tables[0].Rows[0]["End Cycle"].ToString();
                    PayrollAssume13thMonth = dsPayPeriod.Tables[0].Rows[0]["Assume 13th Month"].ToString();
                    bComputeTax = Convert.ToBoolean(dsPayPeriod.Tables[0].Rows[0]["Compute Tax"].ToString());
                }

                //Declare Handlers
                NewLaborHoursGenerationBL = new NewLaborHoursGenerationBL();
                NewPayrollCalculationBL = new NewPayrollCalculationBL();
                NewAlphalistProcessingBL = new NewAlphalistProcessingBL();
                BonusComputation13thMonthBL = new BonusComputation13thMonthBL();
                LeaveRefundBL = new LeaveRefundBL();
                NewLaborHoursGenerationBL.EmpDispHandler += new NewLaborHoursGenerationBL.EmpDispEventHandler(ShowLaborHrsGenEmployeeName);
                NewLaborHoursGenerationBL.StatusHandler += new NewLaborHoursGenerationBL.StatusEventHandler(ShowLaborHourGenStatus);
                NewPayrollCalculationBL.EmpDispHandler += new NewPayrollCalculationBL.EmpDispEventHandler(ShowPayrollCalcEmployeeName);
                NewPayrollCalculationBL.StatusHandler += new NewPayrollCalculationBL.StatusEventHandler(ShowPayrollStatus);
                NewAlphalistProcessingBL.EmpDispHandler += new NewAlphalistProcessingBL.EmpDispEventHandler(ShowW2EmployeeName);
                NewAlphalistProcessingBL.StatusHandler += new NewAlphalistProcessingBL.StatusEventHandler(ShowW2Status);
                LeaveRefundBL.EmpDispHandler += new LeaveRefundBL.EmpDispEventHandler(ShowLeaveRefundName);
                LeaveRefundBL.StatusHandler += new LeaveRefundBL.StatusEventHandler(ShowLeaveRefundStatus);
                BonusComputation13thMonthBL.EmpDispHandler += new BonusComputation13thMonthBL.EmpDispEventHandler(ShowBonusName);
                BonusComputation13thMonthBL.StatusHandler += new BonusComputation13thMonthBL.StatusEventHandler(ShowBonusStatus);

                //Check for Existing Day Codes
                if (commonBL.GetFillerDayCodesCount(companyCode, centralProfile, dal) > 0)
                    bHasDayCodeExt = true;
                else
                    bHasDayCodeExt = false;

                //Create Comma-delimited String for the Employee IDs
                strEmployeeListCommaDelimited = JoinEmployeesFromStringArray(EmployeeList, true);
                dtEmployeeFinalPay = GetAllEmployeeForProcess(strEmployeeListCommaDelimited, ProcessPayrollPeriod);

                //Retrieve Hold Pay Periods
                dtHoldPayPeriods = GetHoldPayrollDetails(strEmployeeListCommaDelimited, ProcessPayrollPeriod);

                StatusHandler(this, new StatusEventArgs("Clean Payroll Tables", false));
                #region Clean Payroll Tables
                ClearLastPayTables(ProcessAll, strEmployeeListCommaDelimited, ProcessPayrollPeriod, NormalPayrollPeriod);
                #endregion
                StatusHandler(this, new StatusEventArgs("Clean Payroll Tables", true));

                dtValidPayPeriods = GetValidPayPeriodsToProcessPerEmployee(strEmployeeListCommaDelimited);

                //Get Parameters and Process Flags
                string LastPayProcessingErrorMessage = SetProcessFlags();

                if (LastPayProcessingErrorMessage != "")
                    throw new Exception(LastPayProcessingErrorMessage);

                #endregion
                //-----------------------------START MAIN PROCESS
                if (dtValidPayPeriods.Rows.Count > 0)
                {
                    #region PART 1
                    for (int j = 0; j < EmployeeList.Length; j++)
                    {
                        drArrEmpDetails = dtEmployeeFinalPay.Select(string.Format("Mem_IDNo = '{0}'", EmployeeList[j]));
                        if (drArrEmpDetails.Length > 0)
                        {
                            bRetirementSeparateFromLastPay          = Convert.ToBoolean(drArrEmpDetails[0]["Tef_RetirementSeparateFromLastPay"]);
                            bComputeRetirement                      = Convert.ToBoolean(drArrEmpDetails[0]["Tef_ComputeRetirement"]);
                            bComputeLeaveRefund                     = Convert.ToBoolean(drArrEmpDetails[0]["Tef_ComputeLeaveRefund"]);
                            bComputeFixAllowance                    = Convert.ToBoolean(drArrEmpDetails[0]["Tef_ComputeFixAllowance"]);
                            bComputeHoldSalary                      = Convert.ToBoolean(drArrEmpDetails[0]["Tef_ComputeHoldSalary"]);
                            
                            strFixAllowRule                         = GetValue(drArrEmpDetails[0]["Tef_FixAllowanceRule"]);
                            strSepDate                              = drArrEmpDetails[0]["Mem_SeparationDate"];
                            strPayrollType                          = GetValue(drArrEmpDetails[0]["Mem_PayrollType"]);
                            strEmploymentStatus                     = GetValue(drArrEmpDetails[0]["Mem_EmploymentStatusCode"]);
                            strRetirementType                       = GetValue(drArrEmpDetails[0]["Tef_RetirementType"]);
                            strRetirementIncomeCode                 = commonBL.GetParameterDtlValueFromPayroll("FPRETIREMENTCODE", strRetirementType, companyCode, dal);
                            dRetirementSalary                       = Convert.ToDecimal(drArrEmpDetails[0]["Mem_Salary"]);
                            dOtherRetirementTaxAmount               = Convert.ToDecimal(drArrEmpDetails[0]["Tef_OtherRetirementTaxableAmt"]);
                            dOtherRetirementNontaxAmount            = Convert.ToDecimal(drArrEmpDetails[0]["Tef_OtherRetirementNontaxableAmt"]);
                            strAge                                  = GetValue(drArrEmpDetails[0]["Mem_Age"]);
                            strDeductions                           = GetEmployeeIncludedDeductions(EmployeeList[j], JoinEmployeesFromDataTableArray(FPDEDUCTIONGROUP, true));
                            strTaxMethod                            = GetValue(drArrEmpDetails[0]["Tef_TaxMethod"]);

                            drArEmpPayCycleSeparation               = dtValidPayPeriods.Select(string.Format("Mem_IDNo = '{0}'", EmployeeList[j]));
                            PayrollSeparationPayCycle               = GetValue(drArEmpPayCycleSeparation[0]["StartPayPeriod"]);
                            PayCycleSeparation                      = GetValue(drArEmpPayCycleSeparation[0]["EndPayPeriod"]);
                            MaxTimeRegisterPayCycle                 = GetValue(drArEmpPayCycleSeparation[0]["MaxTimeRegisterPayPeriod"]);
                            if (drArEmpPayCycleSeparation.Length > 0 && PayrollSeparationPayCycle != PayCycleSeparation)
                            {
                                SystemCycleProcessingBL             = new SystemCycleProcessingBL(dal, PayCycleSeparation, LoginUser, companyCode, centralProfile);
                                strCycIndicator                     = commonBL.GetCycleIndicator(PayCycleSeparation);

                                #region [LABOR HOURS GENERATION]
                                StatusHandler(this, new StatusEventArgs(string.Format("Generate Labor Hours ({0}/{1})", j + 1, EmployeeList.Length), false));
                                if (strCycIndicator == "C")
                                {
                                    NewLaborHoursGenerationBL.GenerateLaborHours(false, true, true, PayCycleSeparation, EmployeeList[j], LoginUser, companyCode, centralProfile, DBCollation, dal);
                                    SystemCycleProcessingBL.GenerateOtherAllowances(EmployeeList[j], true, "--");
                                    strLogLedgerTableName = "T_EmpTimeRegister";
                                }
                                else if (strCycIndicator == "P")
                                {
                                    NewLaborHoursGenerationBL.GenerateLaborHours(false, false, true, PayCycleSeparation, EmployeeList[j], LoginUser, companyCode, centralProfile, DBCollation, dal);
                                    SystemCycleProcessingBL.GenerateOtherAllowances(EmployeeList[j], false, "--");
                                    strLogLedgerTableName = "T_EmpTimeRegisterHst";
                                }
                                StatusHandler(this, new StatusEventArgs(string.Format("Generate Labor Hours ({0}/{1})", j + 1, EmployeeList.Length), true));
                                #endregion

                                #region [COMPUTE VARIABLE ALLOWANCES]
                                StatusHandler(this, new StatusEventArgs(string.Format("Compute Variable Allowances ({0}/{1})", j + 1, EmployeeList.Length), false));
                                for (int k = 1; k <= 12; k++)
                                {
                                    strTempQuery = "";
                                    strTempQuery = string.Format(@"
                                                                INSERT INTO T_EmpTimeBaseAllowanceCycleFinalPay
                                                                SELECT [Ttr_IDNo] [Tta_IDNo]
                                                                    ,@PayCycleCode [Tta_PayCycle]
                                                                    ,@PayCycleCode [Tta_OrigPayCycle]
                                                                    ,[Mvh_AllowanceCode] [Tta_IncomeCode]
                                                                    ,0 [Tta_PostFlag]
                                                                    ,SUM([Ttr_TBAmt{0}]) [Tta_IncomeAmt]
                                                                    ,'P' [Tta_CycleIndicator]
                                                                    , @UserLogin
                                                                    ,GETDATE()		
                                                                FROM {1}
                                                                JOIN {2}..M_VarianceAllowanceHdr
                                                                    ON Mvh_TimeBaseID = '{0}'
                                                                    AND Mvh_CompanyCode = @CompanyCode
                                                                WHERE [Ttr_TBAmt{0}] > 0
                                                                    AND Ttr_PayCycle = @PayCycleCode
                                                                    AND Ttr_IDNo = @IDNo
                                                                GROUP BY [Ttr_IDNo]
                                                                    ,[Ttr_PayCycle]
                                                                    ,Mvh_AllowanceCode"
                                                                    , k.ToString().PadLeft(2, '0')
                                                                    , strLogLedgerTableName
                                                                    , CentralProfile);

                                    ParameterInfo[] paramTimeBased = new ParameterInfo[4];
                                    paramTimeBased[0] = new ParameterInfo("@IDNo", EmployeeList[j]);
                                    paramTimeBased[1] = new ParameterInfo("@PayCycleCode", PayCycleSeparation);
                                    paramTimeBased[2] = new ParameterInfo("@CompanyCode", CompanyCode);
                                    paramTimeBased[3] = new ParameterInfo("@UserLogin", LoginUser);

                                    dal.ExecuteNonQuery(strTempQuery, CommandType.Text, paramTimeBased);
                                }
                                StatusHandler(this, new StatusEventArgs(string.Format("Compute Variable Allowances ({0}/{1})", j + 1, EmployeeList.Length), true));
                                #endregion

                                #region [POSTING VARIABLE ALLOWANCES]
                                if (PostTimeBasedAllowance == true)
                                {
                                    StatusHandler(this, new StatusEventArgs(string.Format("Post Variable Allowances for ({0}/{1})", j + 1, EmployeeList.Length), false));
                                    #region [POSTING OF VARIABLE ALLOWANCES TO EMPLOYEE ALLOWANCE]
                                    strTempQuery = "";
                                    strTempQuery = string.Format(@"
                                                                    INSERT INTO T_EmpIncomeFinalPay
                                                                               (Tin_IDNo
                                                                               ,Tin_PayCycle
                                                                               ,Tin_OrigPayCycle
                                                                               ,Tin_IncomeCode
                                                                               ,Tin_PostFlag
                                                                               ,Tin_IncomeAmt
                                                                               ,Usr_Login
                                                                               ,Ludatetime)
                                                                    SELECT Tta_IDNo 
		                                                                    ,Tta_PayCycle
                                                                            ,Tta_OrigPayCycle
		                                                                    ,Tta_IncomeCode 
		                                                                    ,0
		                                                                    ,Tta_IncomeAmt 
		                                                                    ,@UserLogin
		                                                                    ,GETDATE()
                                                                    FROM T_EmpTimeBaseAllowanceCycleFinalPay 
                                                                    INNER JOIN {1}..M_VarianceAllowanceHdr
                                                                    ON (Tta_IncomeCode = Mvh_AllowanceCode
	                                                                    OR Tta_IncomeCode = Mvh_AdjustmentCode)
	                                                                    AND Mvh_IsIncludeInPayroll = 1
                                                                        AND Mvh_CompanyCode = @CompanyCode
                                                                    WHERE Tta_CycleIndicator IN ('C', 'P')
	                                                                    AND Tta_PayCycle = @PayCycleCode
                                                                        AND Tta_IDNo IN ({0})

                                                                    UPDATE T_EmpTimeBaseAllowanceCycleFinalPay
                                                                    SET Tta_PostFlag = 1
                                                                    FROM T_EmpTimeBaseAllowanceCycleFinalPay 
                                                                    INNER JOIN {1}..M_VarianceAllowanceHdr
                                                                    ON (Tta_IncomeCode = Mvh_AllowanceCode
	                                                                    OR Tta_IncomeCode = Mvh_AdjustmentCode)
	                                                                    AND Mvh_IsIncludeInPayroll = 1
                                                                        AND Mvh_CompanyCode = @CompanyCode
                                                                    WHERE Tta_CycleIndicator IN ('C', 'P')
	                                                                    AND Tta_PayCycle = @PayCycleCode
									                                    AND Tta_IDNo IN ({0})"
                                                            , strEmployeeListCommaDelimited
                                                            , CentralProfile);
                                    #endregion

                                    ParameterInfo[] paramTimeBased = new ParameterInfo[3];
                                    paramTimeBased[0] = new ParameterInfo("@PayCycleCode", ProcessPayrollPeriod);
                                    paramTimeBased[1] = new ParameterInfo("@CompanyCode", CompanyCode);
                                    paramTimeBased[2] = new ParameterInfo("@UserLogin", LoginUser);
                                    dal.ExecuteNonQuery(strTempQuery, CommandType.Text, paramTimeBased);

                                    StatusHandler(this, new StatusEventArgs(string.Format("Post Variable Allowances for ({0}/{1})", j + 1, EmployeeList.Length), true));
                                }
                                #endregion

                                #region [POSTING FIX ALLOWANCES]

                                if (drArrEmpDetails.Length > 0)
                                {
                                    if (bComputeFixAllowance) //Tef_ComputeFixAllowance
                                    {
                                        StatusHandler(this, new StatusEventArgs(string.Format("Post Fix Allowances for ({0}/{1})", j + 1, EmployeeList.Length), false));
                                        #region [PRORATE FIX ALLOWANCES AND POSTING TO EMPLOYEE ALLOWANCE]

                                        strTempQuery = "";
                                        strTempQuery = string.Format(@"
                                            DECLARE @VAL1 AS DECIMAL(10,2) = 0
                                            DECLARE @VAL2 AS DECIMAL(10,2) = 0
                                            DECLARE @MDIVISOR AS INT

                                            SELECT @MDIVISOR = ISNULL((SELECT Mpd_ParamValue
                                                                            FROM M_PolicyDtl
                                                                            WHERE Mpd_PolicyCode = 'MDIVISOR'
                                                                                  AND Mpd_SubCode = Mem_EmploymentStatusCode
                                                                                  AND Mpd_CompanyCode = @CompanyCode
                                                                                  AND Mpd_RecordStatus = 'A'), 0)
                                            FROM M_Employee
                                            WHERE Mem_IDNo = @IDNo

                                            IF (@FIXALLOWRULE = 'D')    ----Prorate by days present                                         
                                                SET @VAL1 = ISNULL((SELECT COUNT(Ttr_IDNo) FROM Udv_TimeRegister			
                                                                WHERE Ttr_IDNo = @IDNo		
                                                                    AND Ttr_PayCycle = @SeparationPayCycleCode			
                                                                    AND Ttr_RestDayFlag = 0			
                                                                    AND Ttr_HolidayFlag = 0			
                                                                    AND Ttr_ActIn_1 + Ttr_ActOut_1 + Ttr_ActIn_2 + Ttr_ActOut_2 <> '0000000000000000'	
                                                                    AND CONVERT(DATE,Ttr_Date) < @SeparationDate),0)

			                                ELSE IF (@FIXALLOWRULE = 'X') -----Compute by days present & assumed                                       
                                                SET @VAL1 = ISNULL((SELECT COUNT(Ttr_IDNo) FROM Udv_TimeRegister			
                                                                WHERE Ttr_IDNo = @IDNo	
                                                                    AND Ttr_PayCycle = @SeparationPayCycleCode			
                                                                    AND Ttr_RestDayFlag = 0			
                                                                    AND Ttr_HolidayFlag = 0			
                                                                    AND (Ttr_ActIn_1 + Ttr_ActOut_1 + Ttr_ActIn_2 + Ttr_ActOut_2 <> '0000000000000000'
                                                                            OR Ttr_AssumedFlag = 1)		
                                                                    AND CONVERT(DATE,Ttr_Date) < @SeparationDate),0)

                                            ELSE IF (@FIXALLOWRULE = 'A') -----Compute by hours absent
                                                SET @VAL1 = ISNULL((SELECT Tph_ABSHr
                                                                FROM T_EmpPayTranHdrFinalPay
                                                                WHERE Tph_IDNo = @IDNo
	                                                                AND Tph_PayCycle = @SeparationPayCycleCode),0)

                                            ELSE IF (@FIXALLOWRULE = 'P') -----Compute by hours present
                                                SET @VAL1 = ISNULL((SELECT SUM(Ttr_REGHour) FROM Udv_TimeRegister			
                                                                WHERE Ttr_IDNo = @IDNo	
                                                                    AND Ttr_PayCycle = @SeparationPayCycleCode		
                                                                    AND Ttr_RestDayFlag = 0			
                                                                    AND Ttr_HolidayFlag = 0			
                                                                    AND Ttr_ActIn_1 + Ttr_ActOut_1 + Ttr_ActIn_2 + Ttr_ActOut_2 <> '0000000000000000'		
                                                                    AND CONVERT(DATE,Ttr_Date) < @SeparationDate),0)
                                            
                                            ELSE IF (@FIXALLOWRULE = 'E') -----Compute by hours present & assumed
                                            	SET @VAL1 = ISNULL((SELECT SUM(CASE WHEN Ttr_AssumedFlag = 1 
																			THEN Ttr_ShiftMin / 60 ELSE Ttr_REGHour END) FROM Udv_TimeRegister			
                                                                            WHERE Ttr_IDNo = @IDNo	
                                                                                AND Ttr_PayCycle = @SeparationPayCycleCode		
                                                                                AND Ttr_RestDayFlag = 0			
                                                                                AND Ttr_HolidayFlag = 0			
                                                                                AND (Ttr_ActIn_1 + Ttr_ActOut_1 + Ttr_ActIn_2 + Ttr_ActOut_2 <> '0000000000000000'
                                                                                     OR Ttr_AssumedFlag = 1)		
                                                                                AND CONVERT(DATE,Ttr_Date) < @SeparationDate),0)
                                            ELSE IF (@FIXALLOWRULE = 'F') -----Prorate by formula
                                                SET @VAL1  = @FPFIXALLOWDIVIDEND

                                            IF (@FIXALLOWRULE = 'D' OR @FIXALLOWRULE = 'X') ----Divisor
                                            SET @VAL2 = ISNULL((SELECT COUNT(Ttr_IDNo) FROM Udv_TimeRegister		
                                                                WHERE Ttr_IDNo = @IDNo		
                                                                    AND Ttr_PayCycle = @SeparationPayCycleCode
                                                                    AND Ttr_RestDayFlag = 0		
                                                                    AND Ttr_HolidayFlag = 0),0)	

                                            IF (@FIXALLOWRULE = 'F') -----Prorate by formula
                                                SET @VAL2  = @FPFIXALLOWDIVISOR                                          

                                        
                                            SELECT RecurAllow.Tfa_IDNo
		                                           , RecurAllow.Tfa_AllowanceCode
												   , RecurAllow.Tfa_Amount
												   , CASE WHEN Min_ApplicablePayCycle = '0' THEN RecurAllow.Tfa_Amount * 2 ELSE RecurAllow.Tfa_Amount END AS [Tfa_MonthlyAllowance]
                                            INTO #TempList
                                            FROM {0}..T_EmpFixAllowance RecurAllow
                                            INNER JOIN {0}..M_Income 
                                            ON Min_IncomeCode = RecurAllow.Tfa_AllowanceCode
	                                                 AND Min_IsRecurring = 1
                                                     AND Min_CompanyCode = @CompanyCode
	                                                 AND Min_RecordStatus = 'A'
	                                                 {1}
 	                                        WHERE Tfa_RecordStatus = 'A'
                                                     AND Tfa_IDNo = @IDNo
	                                                 AND Tfa_StartPayCycle <= @SeparationPayCycleCode
                                                     AND (ISNULL(Tfa_EndPayCycle,'') = '' OR Tfa_EndPayCycle >= @SeparationPayCycleCode)
                                                     AND Tfa_Amount <> 0

                                            INSERT INTO [T_EmpIncomeFinalPay]	
                                                   (Tin_IDNo	
                                                   ,Tin_PayCycle	
                                                   ,Tin_OrigPayCycle	
                                                   ,Tin_IncomeCode	
                                                   ,Tin_PostFlag	
                                                   ,Tin_IncomeAmt	
                                                   ,Usr_Login	
                                                   ,Ludatetime)	
                                            SELECT Tfa_IDNo
                                                   , @PayCycleCode
                                                   , @PayCycleCode
                                                   , Tfa_AllowanceCode
                                                   , 0
                                                   , CAST(CASE WHEN (@FIXALLOWRULE = 'D' OR @FIXALLOWRULE = 'X' OR @FIXALLOWRULE = 'F') THEN ROUND(Tfa_MonthlyAllowance * (@VAL1 / @VAL2),2) 
                                                          WHEN (@FIXALLOWRULE = 'A') THEN ROUND(Tfa_Amount - ROUND(((Tfa_Amount * 12 / @MDIVISOR / @HOURSINDAY) * @VAL1), @HRLYRTEDEC),2) 
                                                          WHEN (@FIXALLOWRULE = 'P' OR @FIXALLOWRULE = 'E') THEN ROUND(ROUND(Tfa_MonthlyAllowance * 12 / (@MDIVISOR) / @HOURSINDAY, @HRLYRTEDEC) * @VAL1 ,2) END AS DECIMAL(9,2))
                                                   , @UserLogin
                                                   , GETDATE()
                                            FROM #TempList
                                            WHERE CAST(CASE WHEN (@FIXALLOWRULE = 'D' OR @FIXALLOWRULE = 'X' OR @FIXALLOWRULE = 'F') THEN ROUND(Tfa_MonthlyAllowance * (@VAL1 / @VAL2),2) 
                                                          WHEN (@FIXALLOWRULE = 'A') THEN ROUND(Tfa_Amount - ROUND(((Tfa_Amount * 12 / @MDIVISOR / @HOURSINDAY) * @VAL1), @HRLYRTEDEC),2) 
                                                          WHEN (@FIXALLOWRULE = 'P' OR @FIXALLOWRULE = 'E') THEN ROUND(ROUND(Tfa_MonthlyAllowance * 12 / (@MDIVISOR) / @HOURSINDAY, @HRLYRTEDEC) * @VAL1 ,2) END AS DECIMAL(9,2)) <> 0

                                            DROP TABLE #TempList

                                            UPDATE T_EmpFinalPay 
                                                   SET Tef_FixAllowanceNumVal1 = @VAL1
                                                   , Tef_FixAllowanceNumVal2 = @VAL2
                                            WHERE Tef_IDNo = @IDNo
                                                AND Tef_PayCycle = @PayCycleCode
                                            ", CentralProfile
                                            , (FPFIXAMOUNT == "A" ? "" : "AND (Min_ApplicablePayCycle = '0' OR Min_ApplicablePayCycle = RIGHT(@SeparationPayCycleCode,1))"));

                                        decimal tmpdividend = 0;
                                        decimal tmpdivisor = 0;
                                        int idxx = 0;
                                        if (strFixAllowRule == "F") //Prorate by formula
                                        {
                                            ParameterInfo[] paramFixAllowFormula = new ParameterInfo[6];
                                            paramFixAllowFormula[idxx++] = new ParameterInfo("@IDNUMBER", EmployeeList[j]);
                                            paramFixAllowFormula[idxx++] = new ParameterInfo("@SEPARATIONDATE", Convert.ToDateTime(strSepDate), SqlDbType.Date);
                                            paramFixAllowFormula[idxx++] = new ParameterInfo("@PAYCYCLECODE", ProcessPayrollPeriod);
                                            paramFixAllowFormula[idxx++] = new ParameterInfo("@SEPARATIONPAYCYCLECODE", PayCycleSeparation);
                                            paramFixAllowFormula[idxx++] = new ParameterInfo("@COMPANYCODE", CompanyCode);
                                            paramFixAllowFormula[idxx++] = new ParameterInfo("@USERLOGIN", LoginUser);
                                            if (!string.IsNullOrEmpty(FPFIXALLOWDIVIDEND))
                                                tmpdividend = GetFormulaQueryDecimalValue(FPFIXALLOWDIVIDEND.Replace("@CENTRALDB", centralProfile), paramFixAllowFormula);
                                            if (!string.IsNullOrEmpty(FPFIXALLOWDIVISOR))
                                                tmpdivisor = GetFormulaQueryDecimalValue(FPFIXALLOWDIVISOR.Replace("@CENTRALDB", centralProfile), paramFixAllowFormula);
                                        }

                                        idxx = 0;
                                        ParameterInfo[] paramFixAllow = new ParameterInfo[11];
                                        paramFixAllow[idxx++] = new ParameterInfo("@IDNo", EmployeeList[j]);
                                        paramFixAllow[idxx++] = new ParameterInfo("@SeparationDate", Convert.ToDateTime(strSepDate), SqlDbType.Date);
                                        paramFixAllow[idxx++] = new ParameterInfo("@PayCycleCode", ProcessPayrollPeriod);
                                        paramFixAllow[idxx++] = new ParameterInfo("@SeparationPayCycleCode", PayCycleSeparation); 
                                        paramFixAllow[idxx++] = new ParameterInfo("@CompanyCode", CompanyCode);
                                        paramFixAllow[idxx++] = new ParameterInfo("@UserLogin", LoginUser);
                                        paramFixAllow[idxx++] = new ParameterInfo("@FIXALLOWRULE", strFixAllowRule, SqlDbType.Char, 1);
                                        paramFixAllow[idxx++] = new ParameterInfo("@HRLYRTEDEC", HRLYRTEDEC, SqlDbType.TinyInt);
                                        paramFixAllow[idxx++] = new ParameterInfo("@HOURSINDAY", CommonBL.HOURSINDAY, SqlDbType.Decimal);
                                        paramFixAllow[idxx++] = new ParameterInfo("@FPFIXALLOWDIVIDEND", tmpdividend, SqlDbType.Decimal);
                                        paramFixAllow[idxx++] = new ParameterInfo("@FPFIXALLOWDIVISOR", tmpdivisor, SqlDbType.Decimal);

                                        if (strTempQuery != "")
                                            dal.ExecuteNonQuery(strTempQuery, CommandType.Text, paramFixAllow);

                                        #endregion
                                        StatusHandler(this, new StatusEventArgs(string.Format("Post Fix Allowances for ({0}/{1})", j + 1, EmployeeList.Length), true));
                                    }
                                }
                                #endregion

                                #region [UPDATE PAY CYCLE TO SPECIAL]
                                strTempQuery = "";
                                strTempQuery = string.Format(@" UPDATE T_EmpPayTranHdrFinalPay
                                                                SET Tph_PayCycle = @PayCycleCode
                                                                WHERE Tph_IDNo = @IDNo
	                                                                AND Tph_PayCycle = @SeparationPayCycleCode

                                                                UPDATE T_EmpPayTranHdrMiscFinalPay
                                                                SET Tph_PayCycle = @PayCycleCode
                                                                WHERE Tph_IDNo = @IDNo
	                                                                AND Tph_PayCycle = @SeparationPayCycleCode

                                                                UPDATE T_EmpPayTranDtlFinalPay
                                                                SET Tpd_PayCycle = @PayCycleCode
                                                                WHERE Tpd_IDNo = @IDNo
	                                                                AND Tpd_PayCycle = @SeparationPayCycleCode

                                                                UPDATE T_EmpPayTranDtlMiscFinalPay
                                                                SET Tpd_PayCycle = @PayCycleCode
                                                                WHERE Tpd_IDNo = @IDNo
	                                                                AND Tpd_PayCycle = @SeparationPayCycleCode

                                                                ---UPDATE T_EmpTimeBaseAllowanceCycleFinalPay  ----BALIKANAN
                                                                ---SET Tta_PayCycle = @PayCycleCode
                                                                ---WHERE Tta_IDNo IN ()
                                                                ---        AND Tta_PayCycle = @SeparationPayCycleCode
                                                                ---        AND Tta_OrigPayCycle = @SeparationPayCycleCode");

                                ParameterInfo[] paramPayroll = new ParameterInfo[3];
                                paramPayroll[0] = new ParameterInfo("@IDNo", EmployeeList[j]);
                                paramPayroll[1] = new ParameterInfo("@PayCycleCode", ProcessPayrollPeriod);
                                paramPayroll[2] = new ParameterInfo("@SeparationPayCycleCode", PayCycleSeparation);

                                dal.ExecuteNonQuery(strTempQuery, CommandType.Text, paramPayroll);
                                #endregion
                            }
                            else
                                InitializePayrollTables(EmployeeList[j], ProcessPayrollPeriod, LoginUser);
                            
                            #region [UPDATE T_EmpFinalPay]
                            strTempQuery = "";
                            strTempQuery = string.Format(@"UPDATE T_EmpFinalPay 
                                                          SET Tef_LastPayPriorSeparation = @Tef_LastPayPriorSeparation
                                                              , Tef_PayCycleSeparation = @Tef_PayCycleSeparation
                                                              , Tef_HireDate = @Tef_HireDate
                                                              , Tef_SeparationDate = @Tef_SeparationDate
                                                              , Tef_SeparationCodeExternal = @Tef_SeparationCodeExternal
                                                              , Tef_BirthDate = @Tef_BirthDate
                                                              , Tef_ZeroOutDeductionCodes = @Tef_ZeroOutDeductionCodes
                                                              , Usr_Login = @Usr_Login
                                                              , Ludatetime = GETDATE()
                                                          WHERE Tef_IDNo = @Tef_IDNo
                                                                AND Tef_PayCycle = @Tef_PayCycle ");

                            ParameterInfo[] paramFinalPay = new ParameterInfo[10];
                            paramFinalPay[0] = new ParameterInfo("@Tef_IDNo", EmployeeList[j]);
                            paramFinalPay[1] = new ParameterInfo("@Tef_PayCycle", ProcessPayrollPeriod);
                            paramFinalPay[2] = new ParameterInfo("@Tef_LastPayPriorSeparation", PayrollSeparationPayCycle);
                            paramFinalPay[3] = new ParameterInfo("@Tef_PayCycleSeparation", PayCycleSeparation);
                            paramFinalPay[4] = new ParameterInfo("@Tef_HireDate", drArrEmpDetails[0]["Mem_IntakeDate"], SqlDbType.Date);
                            paramFinalPay[5] = new ParameterInfo("@Tef_SeparationDate", drArrEmpDetails[0]["Mem_SeparationDate"], SqlDbType.Date);
                            paramFinalPay[6] = new ParameterInfo("@Tef_SeparationCodeExternal", GetValue(drArrEmpDetails[0]["Mem_SeparationCodeExternal"]));
                            paramFinalPay[7] = new ParameterInfo("@Tef_BirthDate", drArrEmpDetails[0]["Mem_BirthDate"], SqlDbType.Date);
                            paramFinalPay[8] = new ParameterInfo("@Tef_ZeroOutDeductionCodes", strDeductions, SqlDbType.VarChar, strDeductions.Length);
                            paramFinalPay[9] = new ParameterInfo("@Usr_Login", LoginUser);

                            dal.ExecuteNonQuery(strTempQuery, CommandType.Text, paramFinalPay);
                            #endregion

                            #region [COMPUTE LEAVE REFUND]
                            StatusHandler(this, new StatusEventArgs(string.Format("Compute Leave Refund ({0}/{1})", j + 1, EmployeeList.Length), false));
                            if (CheckIfLEaveRefundAlreadyExists(EmployeeList[j], ProcessPayrollPeriod))
                            {
                                bComputeLeaveRefund = false;
                            }

                            if (bComputeLeaveRefund)  //Tef_ComputeLeaveRefund
                            {
                                bool Recompute = false;
                                string ControlNumber = LeaveRefundControlNumberExists(ProcessPayrollPeriod, dal);
                                if (ControlNumber != "")
                                    Recompute = true;

                                LeaveRefundBL.ComputeLeaveRefund(true
                                                                    , Recompute
                                                                    , ProcessPayrollPeriod
                                                                    , EmployeeList[j]
                                                                    , LoginUser
                                                                    , companyCode
                                                                    , centralProfile
                                                                    , profileCode
                                                                    , ControlNumber
                                                                    , (FPPROCESSYEARLVE == "P" ? ProcessPayrollPeriod.Substring(0, 4) : Convert.ToDateTime(strSepDate).Year.ToString())
                                                                    , dal);

                                #region [UPDATE T_EmpFinalPay] 
                                strTempQuery = "";
                                strTempQuery = string.Format(@"UPDATE T_EmpFinalPay 
                                                               SET Tef_LeaveTypeYear = (SELECT REPLACE(STUFF((SELECT ',' + [Leave Type - Year] AS 'data()' 
	                                                                                   FROM (SELECT Tlr_ControlNo, Tlr_LeaveCode + Tlr_LeaveYear AS [Leave Type - Year] 
												                                                FROM T_EmpLeaveRefundRule 
                                                                                            ) RefundRule
				                                                                                WHERE Tlr_ControlNo = @ControlNo
					                                                                                FOR XML PATH('')), 1, 1, ''),' ,',','))
                                                               , Tef_LeaveRefundFormula = @FPLVREFUNDFORMULA
                                                               , Usr_Login = @Usr_Login
                                                               , Ludatetime = GETDATE()
                                                            WHERE Tef_IDNo IN ({0})
                                                               AND Tef_PayCycle = @Tef_PayCycle"
                                                    , strEmployeeListCommaDelimited);

                                ParameterInfo[] paramLeaveRefund = new ParameterInfo[4];
                                paramLeaveRefund[0] = new ParameterInfo("@ControlNo", ControlNumber);
                                paramLeaveRefund[1] = new ParameterInfo("@Tef_PayCycle", ProcessPayrollPeriod);
                                paramLeaveRefund[2] = new ParameterInfo("@FPLVREFUNDFORMULA", FPLVREFUNDFORMULA, SqlDbType.NVarChar, 4000);
                                paramLeaveRefund[3] = new ParameterInfo("@Usr_Login", LoginUser);

                                dal.ExecuteNonQuery(strTempQuery, CommandType.Text, paramLeaveRefund);
                                #endregion

                                #region [POSTING OF LEAVE CONVERSION TO EMPLOYEE ALLOWANCE]
                                LeaveRefundBL.PostToIncome(true, ProcessPayrollPeriod, strEmployeeListCommaDelimited, ControlNumber, LoginUser, CompanyCode, CentralProfile, dal);
                                #endregion

                            }
                            StatusHandler(this, new StatusEventArgs(string.Format("Compute Leave Refund ({0}/{1})", j + 1, EmployeeList.Length), true));
                            #endregion

                            #region [COMPUTE RETIREMENT PAY]
                            dRetirementTaxAmount        = 0;
                            dRetirementAmount           = 0;
                            dTenureYears                = 0;
                            dRetirementRate             = 0;
                            dRetirementTaxBracket       = 0;
                            dTotalRetirementTaxAmount   = 0;

                            if (strPayrollType == "D")
                            {
                                MDIVISOR = Convert.ToDecimal(GetScalarValue("Mpd_ParamValue", "M_PolicyDtl", string.Format("Mpd_PolicyCode = 'MDIVISOR' AND Mpd_SubCode = '{0}' AND Mpd_CompanyCode = '{1}' AND Mpd_RecordStatus = 'A'", strEmploymentStatus, CompanyCode), dal));
                                dRetirementSalary = ((dRetirementSalary * MDIVISOR) / 12);
                            }

                            if (FPRETIREMENTTENURE != "")
                            {
                                int idxx = 0;
                                ParameterInfo[] paramDtl = new ParameterInfo[1];
                                paramDtl[idxx++] = new ParameterInfo("@IDNUMBER", EmployeeList[j]);
                                dTenureYears = BonusComputation13thMonthBL.GetFormulaQueryDecimalValue(FPRETIREMENTTENURE.Replace("@CENTRALDB", CentralProfile), paramDtl, dal);
                            }

                            dRetirementRate = GetFormulaQueryDecimalValue(string.Format(@"SELECT Mrt_RetirementRate FROM {4}..M_Retirement
                                                                                            WHERE Mrt_CompanyCode = '{0}'
                                                                                            AND Mrt_RetirementType = '{1}'
                                                                                            AND {3} BETWEEN Mrt_AgeMin AND Mrt_AgeMax
                                                                                            AND {2} BETWEEN Mrt_TenureYearsMin AND Mrt_TenureYearsMax"
                                                                                                                , companyCode
                                                                                                                , strRetirementType
                                                                                                                , dTenureYears
                                                                                                                , strAge
                                                                                                                , CentralProfile));

                            if (dRetirementRate > 0)
                            {
                                if (FPRETIREMENTFORMULA != "")
                                {
                                    int idxx = 0;
                                    ParameterInfo[] paramDtl = new ParameterInfo[7];
                                    paramDtl[idxx++] = new ParameterInfo("@IDNUMBER", EmployeeList[j]);
                                    paramDtl[idxx++] = new ParameterInfo("@PAYCYCLE", ProcessPayrollPeriod);
                                    paramDtl[idxx++] = new ParameterInfo("@TENURE", dTenureYears, SqlDbType.Decimal);
                                    paramDtl[idxx++] = new ParameterInfo("@SALARYRATE", Math.Round(dRetirementSalary, 2), SqlDbType.Decimal);
                                    paramDtl[idxx++] = new ParameterInfo("@RETIREMENTTYPE", strRetirementType);
                                    paramDtl[idxx++] = new ParameterInfo("@RETIREMENTRATE", Math.Round(dRetirementRate, 2), SqlDbType.Decimal);
                                    paramDtl[idxx++] = new ParameterInfo("@YEAR", ProcessPayrollPeriod.Substring(0,4));
                                    dRetirementAmount = BonusComputation13thMonthBL.GetFormulaQueryDecimalValue(FPRETIREMENTFORMULA.Replace("@CENTRALDB", CentralProfile), paramDtl, dal);
                                }

                                if (bRetirementSeparateFromLastPay)
                                {
                                    strRetirememntTaxClass = GetScalarValue("Min_TaxClass", string.Format("{0}..M_Income", CentralProfile), string.Format("Min_IncomeCode = '{0}' AND Min_CompanyCode = '{1}'", strRetirementIncomeCode, companyCode), dal);
                                    //if (strRetirememntTaxClass == "T")
                                    dTotalRetirementTaxAmount = dRetirementAmount + dOtherRetirementTaxAmount;
                                    //else
                                        //dTotalRetirementTaxAmount = dOtherRetirementTaxAmount;

                                    if (strRetirememntTaxClass == "T" && dTotalRetirementTaxAmount > 0 )
                                    {
                                        DataSet dsAnnualTaxAmt = NewPayrollCalculationBL.GetAnnualizedTaxAmount(dTotalRetirementTaxAmount
                                                                                                        , NewPayrollCalculationBL.GetMaxPayPeriodAnnualTaxSchedule(ProcessPayrollPeriod, companyCode, centralProfile, dal)
                                                                                                        , companyCode
                                                                                                        , centralProfile
                                                                                                        , dal);

                                        if (dsAnnualTaxAmt.Tables[0].Rows.Count > 0)
                                        {
                                            dRetirementTaxAmount = Math.Round(Convert.ToDecimal(dsAnnualTaxAmt.Tables[0].Rows[0][0].ToString()), 2);
                                            dRetirementTaxBracket = Convert.ToInt32(dsAnnualTaxAmt.Tables[0].Rows[0][1].ToString());
                                        }
                                    }
                                }

                                #region [POST RETIREMENT PAY TO EMPLOYEE ALLOWANCE]
                                if (!bRetirementSeparateFromLastPay && bComputeRetirement)
                                {
                                    strTempQuery = string.Format(@"
                                                                    INSERT INTO T_EmpIncomeFinalPay
                                                                           (Tin_IDNo
                                                                           ,Tin_PayCycle
                                                                           ,Tin_OrigPayCycle
                                                                           ,Tin_IncomeCode
                                                                           ,Tin_PostFlag
                                                                           ,Tin_IncomeAmt
                                                                           ,Usr_Login
                                                                           ,Ludatetime)
                                                                    VALUES (@Tin_IDNo
                                                                            , @Tin_PayCycle
                                                                            , @Tin_PayCycle
                                                                            , @Tin_IncomeCode
                                                                            , 0
                                                                            , @Tin_IncomeAmt
                                                                            , @Usr_Login
                                                                            , GETDATE())");

                                    ParameterInfo[] paramIncome = new ParameterInfo[5];
                                    paramIncome[0] = new ParameterInfo("@Tin_IDNo", EmployeeList[j]);
                                    paramIncome[1] = new ParameterInfo("@Tin_PayCycle", ProcessPayrollPeriod);
                                    paramIncome[2] = new ParameterInfo("@Tin_IncomeCode", strRetirementIncomeCode);
                                    paramIncome[3] = new ParameterInfo("@Tin_IncomeAmt", dRetirementAmount, SqlDbType.Decimal);
                                    paramIncome[4] = new ParameterInfo("@Usr_Login", LoginUser);

                                    dal.ExecuteNonQuery(strTempQuery, CommandType.Text, paramIncome);
                                }
                                #endregion
                            }
                            #endregion

                            #region [UPDATE T_EmpFinalPay]
                            ParameterInfo[] paramUpdateRetirement = new ParameterInfo[12];
                            paramUpdateRetirement[0] = new ParameterInfo("@Tef_IDNo", EmployeeList[j]);
                            paramUpdateRetirement[1] = new ParameterInfo("@Tef_PayCycle", ProcessPayrollPeriod);
                            paramUpdateRetirement[2] = new ParameterInfo("@Tef_TotalRetirementAmt", Math.Round(dRetirementAmount + dOtherRetirementTaxAmount + dOtherRetirementNontaxAmount,2), SqlDbType.Decimal);
                            paramUpdateRetirement[3] = new ParameterInfo("@Tef_RetirementFormula", FPRETIREMENTFORMULA, SqlDbType.NVarChar, 4000);
                            paramUpdateRetirement[4] = new ParameterInfo("@Tef_RetirementSalary", Math.Round(dRetirementSalary, 2), SqlDbType.Decimal);
                            paramUpdateRetirement[5] = new ParameterInfo("@Tef_RetirementAmt", Math.Round(dRetirementAmount, 2), SqlDbType.Decimal);
                            paramUpdateRetirement[6] = new ParameterInfo("@Tef_RetirementRate", Math.Round(dRetirementRate, 2), SqlDbType.Decimal);
                            paramUpdateRetirement[7] = new ParameterInfo("@Tef_RetirementTax", Math.Round(dRetirementTaxAmount, 2), SqlDbType.Decimal);
                            paramUpdateRetirement[8] = new ParameterInfo("@Tef_RetirementNetTax", (dTotalRetirementTaxAmount > 0 ? Math.Round(dTotalRetirementTaxAmount - dRetirementTaxAmount, 2) : 0), SqlDbType.Decimal);
                            paramUpdateRetirement[9] = new ParameterInfo("@Tef_TenureFormula", FPRETIREMENTTENURE, SqlDbType.NVarChar, 4000);
                            paramUpdateRetirement[10] = new ParameterInfo("@Tef_TenureYears", dTenureYears, SqlDbType.Decimal);
                            paramUpdateRetirement[11] = new ParameterInfo("@Tef_RetirementTaxBracket", dRetirementTaxBracket, SqlDbType.TinyInt);

                            strTempQuery = "";
                            if (!bRetirementSeparateFromLastPay && bComputeRetirement)
                            {
                                strTempQuery = string.Format(@"UPDATE T_EmpFinalPay 
                                                                 SET Tef_RetirementFormula = @Tef_RetirementFormula
                                                                , Tef_TenureFormula = @Tef_TenureFormula
                                                                , Tef_TenureYears = @Tef_TenureYears
                                                                , Tef_RetirementSalary = @Tef_RetirementSalary
                                                                , Tef_RetirementAmt = @Tef_RetirementAmt
                                                                , Tef_RetirementRate = @Tef_RetirementRate
                                                               WHERE Tef_IDNo = @Tef_IDNo
                                                                AND Tef_PayCycle = @Tef_PayCycle");
                                dal.ExecuteNonQuery(strTempQuery, CommandType.Text, paramUpdateRetirement);
                            }
                            else if (bRetirementSeparateFromLastPay)
                            {
                                strTempQuery = string.Format(@"UPDATE T_EmpFinalPay 
                                                                 SET Tef_RetirementFormula = @Tef_RetirementFormula
                                                                , Tef_TenureFormula = @Tef_TenureFormula
                                                                , Tef_TenureYears = @Tef_TenureYears
                                                                , Tef_RetirementSalary = @Tef_RetirementSalary
                                                                , Tef_RetirementAmt = @Tef_RetirementAmt
                                                                , Tef_RetirementRate = @Tef_RetirementRate
                                                                , Tef_RetirementTax = @Tef_RetirementTax
                                                                , Tef_RetirementNetTax = @Tef_RetirementNetTax
                                                                , Tef_RetirementTaxBracket = @Tef_RetirementTaxBracket
                                                                , Tef_TotalRetirementAmt = @Tef_TotalRetirementAmt
                                                               WHERE Tef_IDNo = @Tef_IDNo
                                                                AND Tef_PayCycle = @Tef_PayCycle");
                                dal.ExecuteNonQuery(strTempQuery, CommandType.Text, paramUpdateRetirement);
                            }
                            #endregion

                            #region [POSTING HOLD IN PAYROLL TO EMPLOYEE ALLOWANCE]
                            if (bComputeHoldSalary)
                            {
                                if (dtHoldPayPeriods != null && dtHoldPayPeriods.Rows.Count > 0)
                                {
                                    drArrPayCycleHold = dtHoldPayPeriods.Select(string.Format("IDNo = '{0}'", EmployeeList[j]));
                                    if (drArrPayCycleHold != null && drArrPayCycleHold.Length > 0)
                                    {
                                        #region INSERT Income Master
                                        strTempQuery = "";
                                        strTempQuery = string.Format(@"
                                             IF NOT EXISTS(SELECT * FROM {0}..M_Income WHERE Min_IncomeCode = 'HOLDSALARY' AND Min_CompanyCode = @Min_CompanyCode)
                                             BEGIN
                                                                  INSERT INTO {0}..M_Income
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
                                                                            ,Min_RecordStatus
                                                                            ,Min_CreatedBy
                                                                            ,Min_CreatedDate)
                                                                      VALUES
                                                                            (@Min_CompanyCode
                                                                            ,'HOLDSALARY'
                                                                            ,'HOLD SALARY NONTAXABLE'
                                                                            ,'N'
                                                                            ,0
                                                                            ,0
                                                                            ,'HSA'
                                                                            ,'N-NOTINCLUDE'
                                                                            , 1
                                                                            ,''
                                                                            ,''
                        				                                    ,'A'
                                                                            ,@Min_CreatedBy
                                                                            ,GETDATE())
                                             END", CentralProfile);
                                        ParameterInfo[] paramIncomeMaster = new ParameterInfo[2];
                                        paramIncomeMaster[0] = new ParameterInfo("@Min_CompanyCode", CompanyCode);
                                        paramIncomeMaster[1] = new ParameterInfo("@Min_CreatedBy", LoginUser);

                                        dal.ExecuteNonQuery(strTempQuery, CommandType.Text, paramIncomeMaster);
                                        #endregion

                                        StatusHandler(this, new StatusEventArgs(string.Format("Post Hold Payroll to Income ({0}/{1})", j + 1, EmployeeList.Length), false));
                                        for (int dr = 0; dr < drArrPayCycleHold.Length; dr++)
                                        {
                                            #region [POSTING OF HOLD IN PAYROLL TO EMPLOYEE ALLOWANCE]
                                            strTempQuery = "";
                                            strTempQuery = string.Format(@"
                                             INSERT INTO T_EmpIncomeFinalPay
                                                        (Tin_IDNo
                                                        ,Tin_PayCycle
                                                        ,Tin_OrigPayCycle
                                                        ,Tin_IncomeCode
                                                        ,Tin_PostFlag
                                                        ,Tin_IncomeAmt
                                                        ,Usr_Login
                                                        ,Ludatetime)
                                            VALUES(@Tin_IDNo
                                                         , @Tin_PayCycle
                                                         , @Tin_OrigPayCycle
                                                         , 'HOLDSALARY'
                                                         , 0
                                                         , @Tin_IncomeAmt
                                                         , @Usr_Login
                                                         , GETDATE()) ");
                                            #endregion

                                            ParameterInfo[] paramIncome = new ParameterInfo[5];
                                            paramIncome[0] = new ParameterInfo("@Tin_IDNo", EmployeeList[j]);
                                            paramIncome[1] = new ParameterInfo("@Tin_PayCycle", ProcessPayrollPeriod);
                                            paramIncome[2] = new ParameterInfo("@Tin_OrigPayCycle", drArrPayCycleHold[dr]["Pay Cycle"]);
                                            paramIncome[3] = new ParameterInfo("@Tin_IncomeAmt", Convert.ToDecimal(drArrPayCycleHold[dr]["Amount"]), SqlDbType.Decimal);
                                            paramIncome[4] = new ParameterInfo("@Usr_Login", LoginUser);

                                            dal.ExecuteNonQuery(strTempQuery, CommandType.Text, paramIncome);
                                        }
                                        StatusHandler(this, new StatusEventArgs(string.Format("Post Hold Payroll to Income ({0}/{1})", j + 1, EmployeeList.Length), true));
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                    #endregion

                    #region [POSTING ALLOWANCES | SYSTEM AND MANUAL ADJUSTMENTS TO PAYROLL TRANSACTION]
                    StatusHandler(this, new StatusEventArgs("Post Adjustments", false));
                    #region [POSTING TO PAYROLL TRANSACTION]
                        strTempQuery = "";
                        strTempQuery = string.Format(@"
                            --INITIALIZE START
                            UPDATE T_EmpSystemAdjFinalPay
                            SET Tsa_PostFlag = 0 
                            WHERE Tsa_AdjPayCycle = '{0}'
                                AND Tsa_IDNo IN ({1})

                            UPDATE T_EmpManualAdjFinalPay
                            SET Tma_PostFlag = 0
                            WHERE Tma_PayCycle = '{0}'
                                AND Tma_IDNo IN ({1})

                            UPDATE T_EmpIncomeFinalPay
                            SET Tin_PostFlag = 0
                            WHERE Tin_PayCycle = '{0}'
                                AND Tin_IDNo IN ({1})
                            --INITIALIZE END

                            UPDATE T_EmpPayTranHdrFinalPay
                            SET  Tph_SRGAdjHr = '0.00'
								, Tph_SRGAdjAmt = '0.00'
								, Tph_SOTAdjHr = '0.00'
								, Tph_SOTAdjAmt = '0.00'
								, Tph_SHOLAdjHr = '0.00'
								, Tph_SHOLAdjAmt = '0.00'
								, Tph_SNDAdjHr = '0.00'
								, Tph_SNDAdjAmt = '0.00'
								, Tph_SLVAdjHr = '0.00'
								, Tph_SLVAdjAmt = '0.00'
								, Tph_MRGAdjHr = '0.00'
								, Tph_MRGAdjAmt = '0.00'
								, Tph_MOTAdjHr = '0.00'
								, Tph_MOTAdjAmt = '0.00'
								, Tph_MHOLAdjHr = '0.00'
								, Tph_MHOLAdjAmt = '0.00'
								, Tph_MNDAdjHr = '0.00'
								, Tph_MNDAdjAmt = '0.00'
								, Tph_TotalAdjAmt = '0.00'
                                , Tph_TaxableIncomeAmt = '0.00'
                                , Tph_NontaxableIncomeAmt = '0.00'
                            WHERE Tph_PayCycle = '{0}'
                                AND Tph_IDNo IN ({1})
                                AND (Tph_SRGAdjHr <> 0
									OR Tph_SRGAdjAmt <> 0
									OR Tph_SOTAdjHr <> 0
									OR Tph_SOTAdjAmt <> 0
									OR Tph_SHOLAdjHr <> 0
									OR Tph_SHOLAdjAmt <> 0
									OR Tph_SNDAdjHr <> 0
									OR Tph_SNDAdjAmt <> 0
									OR Tph_SLVAdjHr <> 0
									OR Tph_SLVAdjAmt <> 0
									OR Tph_MRGAdjHr <> 0
									OR Tph_MRGAdjAmt <> 0
									OR Tph_MOTAdjHr <> 0
									OR Tph_MOTAdjAmt <> 0
									OR Tph_MHOLAdjHr <> 0
									OR Tph_MHOLAdjAmt <> 0
									OR Tph_MNDAdjHr <> 0
									OR Tph_MNDAdjAmt <> 0
									OR Tph_TotalAdjAmt <> 0 
                                    OR Tph_TaxableIncomeAmt <> 0
                                    OR Tph_NontaxableIncomeAmt <> 0)

							UPDATE T_EmpPayTranHdrFinalPay
                            SET Tph_SRGAdjHr = Tsa_REGHr
								, Tph_SRGAdjAmt = Tsa_RGAdjAmt
								, Tph_SOTAdjHr = Tsa_OTAdjHr
								, Tph_SOTAdjAmt = Tsa_OTAdjAmt
								, Tph_SHOLAdjHr = Tsa_HOLAdjHr
								, Tph_SHOLAdjAmt = Tsa_HOLAdjAmt
								, Tph_SNDAdjHr = Tsa_NDAdjHr
								, Tph_SNDAdjAmt = Tsa_NDAdjAmt
								, Tph_SLVAdjHr = Tsa_LVAdjHr
								, Tph_SLVAdjAmt = Tsa_LVAdjAmt
								, Tph_TotalAdjAmt = Tsa_TotalAdjAmt
                                , Usr_Login = '{2}'
                                , Ludatetime = GETDATE()
                            FROM T_EmpPayTranHdrFinalPay
                            INNER JOIN (
								SELECT A.Tsa_IDNo
										, A.Tsa_AdjPayCycle
										, SUM(A.Tsa_REGHr) AS Tsa_REGHr
										, SUM(B.Tsa_RGAdjAmt) AS Tsa_RGAdjAmt
										, SUM(A.Tsa_REGOTHr          
											+ A.Tsa_RESTHr + A.Tsa_RESTOTHr    
											+ A.Tsa_LEGHOLHr + A.Tsa_LEGHOLOTHr    
											+ A.Tsa_SPLHOLHr + A.Tsa_SPLHOLOTHr    
											+ A.Tsa_PSDHr + A.Tsa_PSDOTHr    
											+ A.Tsa_COMPHOLHr + A.Tsa_COMPHOLOTHr    
											+ A.Tsa_RESTLEGHOLHr + A.Tsa_RESTLEGHOLOTHr    
											+ A.Tsa_RESTSPLHOLHr  + A.Tsa_RESTSPLHOLOTHr    
											+ A.Tsa_RESTCOMPHOLHr + A.Tsa_RESTCOMPHOLOTHr    
											+ A.Tsa_RESTPSDHr + A.Tsa_RESTPSDOTHr
											+ ISNULL(C.Tsm_Misc1Hr, 0) + ISNULL(C.Tsm_Misc1OTHr, 0)
											+ ISNULL(C.Tsm_Misc2Hr, 0) + ISNULL(C.Tsm_Misc2OTHr, 0)
											+ ISNULL(C.Tsm_Misc3Hr, 0) + ISNULL(C.Tsm_Misc3OTHr, 0)
											+ ISNULL(C.Tsm_Misc4Hr, 0) + ISNULL(C.Tsm_Misc4OTHr, 0)
											+ ISNULL(C.Tsm_Misc5Hr, 0) + ISNULL(C.Tsm_Misc5OTHr, 0)
											+ ISNULL(C.Tsm_Misc6Hr, 0) + ISNULL(C.Tsm_Misc6OTHr, 0)) AS Tsa_OTAdjHr
										, SUM(B.Tsa_OTAdjAmt) AS Tsa_OTAdjAmt
										, SUM(A.Tsa_PDLEGHOLHr 
											+ A.Tsa_PDSPLHOLHr 
											+ A.Tsa_PDCOMPHOLHr 
                                            + A.Tsa_PDPSDHr
											+ A.Tsa_PDOTHHOLHr 
											- A.Tsa_ABSLEGHOLHr 
											- A.Tsa_ABSSPLHOLHr 
											- A.Tsa_ABSCOMPHOLHr 
											- A.Tsa_ABSPSDHr 
											- A.Tsa_ABSOTHHOLHr) AS Tsa_HOLAdjHr
										, SUM(B.Tsa_HOLAdjAmt) AS Tsa_HOLAdjAmt
										, SUM(A.Tsa_REGNDHr	+ A.Tsa_REGNDOTHr		
											+ A.Tsa_RESTNDHr + A.Tsa_RESTNDOTHr		
											+ A.Tsa_LEGHOLNDHr + A.Tsa_LEGHOLNDOTHr		
											+ A.Tsa_SPLHOLNDHr + A.Tsa_SPLHOLNDOTHr		
											+ A.Tsa_PSDNDHr + A.Tsa_PSDNDOTHr		
											+ A.Tsa_COMPHOLNDHr	+ A.Tsa_COMPHOLNDOTHr		
											+ A.Tsa_RESTLEGHOLNDHr + A.Tsa_RESTLEGHOLNDOTHr		
											+ A.Tsa_RESTSPLHOLNDHr + A.Tsa_RESTSPLHOLNDOTHr		
											+ A.Tsa_RESTCOMPHOLNDHr	+ A.Tsa_RESTCOMPHOLNDOTHr		
											+ A.Tsa_RESTPSDNDHr	+ A.Tsa_RESTPSDNDOTHr
											+ ISNULL(C.Tsm_Misc1NDHr, 0) + ISNULL(C.Tsm_Misc1NDOTHr, 0)
											+ ISNULL(C.Tsm_Misc2NDHr, 0) + ISNULL(C.Tsm_Misc2NDOTHr, 0)
											+ ISNULL(C.Tsm_Misc3NDHr, 0) + ISNULL(C.Tsm_Misc3NDOTHr, 0)
											+ ISNULL(C.Tsm_Misc4NDHr, 0) + ISNULL(C.Tsm_Misc4NDOTHr, 0)
											+ ISNULL(C.Tsm_Misc5NDHr, 0) + ISNULL(C.Tsm_Misc5NDOTHr, 0)
											+ ISNULL(C.Tsm_Misc6NDHr, 0) + ISNULL(C.Tsm_Misc6NDOTHr, 0)) AS Tsa_NDAdjHr
										, SUM(B.Tsa_NDAdjAmt) AS Tsa_NDAdjAmt
										, SUM(A.Tsa_PDLVHr) AS Tsa_LVAdjHr
										, SUM(B.Tsa_LVAdjAmt) AS Tsa_LVAdjAmt
										, SUM(B.Tsa_TotalAdjAmt) AS Tsa_TotalAdjAmt
								FROM T_EmpSystemAdjFinalPay A
                                INNER JOIN T_EmpSystemAdj2FinalPay B
								    ON  A.Tsa_IDNo = B.Tsa_IDNo 
								    AND A.Tsa_AdjPayCycle = B.Tsa_AdjPayCycle
								    AND A.Tsa_PayCycle = B.Tsa_PayCycle
								    AND A.Tsa_Date = B.Tsa_Date
								LEFT JOIN T_EmpSystemAdjMiscFinalPay C 
                                    ON C.Tsm_IDNo = A.Tsa_IDNo 
								    AND C.Tsm_AdjPayCycle = A.Tsa_AdjPayCycle
								    AND C.Tsm_PayCycle = A.Tsa_PayCycle
								    AND C.Tsm_Date = A.Tsa_Date
								WHERE A.Tsa_AdjPayCycle = '{0}'
                                    AND A.Tsa_IDNo IN ({1})
								GROUP BY A.Tsa_IDNo, A.Tsa_AdjPayCycle 
                            ) xx 
                            ON xx.Tsa_IDNo = Tph_IDNo
                                And xx.Tsa_AdjPayCycle = Tph_PayCycle
                            WHERE Tph_PayCycle = '{0}'
                                AND Tph_IDNo IN ({1})
                            
							UPDATE T_EmpPayTranHdrFinalPay
                            SET Tph_MRGAdjHr = Tma_RGAdjHr
								, Tph_MRGAdjAmt = Tma_RGAdjAmt
								, Tph_MOTAdjHr = Tma_OTAdjHr
								, Tph_MOTAdjAmt = Tma_OTAdjAmt
								, Tph_MHOLAdjHr = Tma_HOLAdjHr
								, Tph_MHOLAdjAmt = Tma_HOLAdjAmt
								, Tph_MNDAdjHr = Tma_NDAdjHr
								, Tph_MNDAdjAmt = Tma_NDAdjAmt
								, Tph_TotalAdjAmt = Tph_TotalAdjAmt + Tma_TotalAdjAmt
                                , Usr_Login = '{2}'
                                , Ludatetime = GETDATE()
                            FROM T_EmpPayTranHdrFinalPay
                            INNER JOIN (
								SELECT A.Tma_IDNo
										, A.Tma_PayCycle
										, SUM(Tma_REGHr) AS Tma_RGAdjHr
										, SUM(B.Tma_RGAdjAmt) AS Tma_RGAdjAmt
										, SUM(A.Tma_REGOTHr										
											+ A.Tma_RESTHr + A.Tma_RESTOTHr
											+ A.Tma_LEGHOLHr + A.Tma_LEGHOLOTHr
											+ A.Tma_SPLHOLHr + A.Tma_SPLHOLOTHr
											+ A.Tma_PSDHr + A.Tma_PSDOTHr
											+ A.Tma_COMPHOLHr + A.Tma_COMPHOLOTHr
											+ A.Tma_RESTLEGHOLHr + A.Tma_RESTLEGHOLOTHr
											+ A.Tma_RESTSPLHOLHr + A.Tma_RESTSPLHOLOTHr
											+ A.Tma_RESTCOMPHOLHr + A.Tma_RESTCOMPHOLOTHr
											+ A.Tma_RESTPSDHr + A.Tma_RESTPSDOTHr
											+ ISNULL(C.Tmm_Misc1Hr, 0) + ISNULL(C.Tmm_Misc1OTHr, 0)
											+ ISNULL(C.Tmm_Misc2Hr, 0) + ISNULL(C.Tmm_Misc2OTHr, 0)
											+ ISNULL(C.Tmm_Misc3Hr, 0) + ISNULL(C.Tmm_Misc3OTHr, 0)
											+ ISNULL(C.Tmm_Misc4Hr, 0) + ISNULL(C.Tmm_Misc4OTHr, 0)
											+ ISNULL(C.Tmm_Misc5Hr, 0) + ISNULL(C.Tmm_Misc5OTHr, 0)
											+ ISNULL(C.Tmm_Misc6Hr, 0) + ISNULL(C.Tmm_Misc6OTHr, 0)) AS Tma_OTAdjHr
										, SUM(ISNULL(B.Tma_OTAdjAmt, 0)) AS Tma_OTAdjAmt
										, SUM(A.Tma_PDLEGHOLHr 
											+ A.Tma_PDSPLHOLHr 
											+ A.Tma_PDCOMPHOLHr
                                            + A.Tma_PDPSDHr
                                            + A.Tma_PDOTHHOLHr) AS Tma_HOLAdjHr
										, SUM(B.Tma_HOLAdjAmt) AS Tma_HOLAdjAmt
										, SUM(A.Tma_REGNDHr + A.Tma_REGNDOTHr									
												+ A.Tma_RESTNDHr + A.Tma_RESTNDOTHr
												+ A.Tma_LEGHOLNDHr + A.Tma_LEGHOLNDOTHr
												+ A.Tma_SPLHOLNDHr + A.Tma_SPLHOLNDOTHr
												+ A.Tma_PSDNDHr + A.Tma_PSDNDOTHr
												+ A.Tma_COMPHOLNDHr + A.Tma_COMPHOLNDOTHr
												+ A.Tma_RESTLEGHOLNDHr + A.Tma_RESTLEGHOLNDOTHr
												+ A.Tma_RESTSPLHOLNDHr + A.Tma_RESTSPLHOLNDOTHr
												+ A.Tma_RESTCOMPHOLNDHr + A.Tma_RESTCOMPHOLNDOTHr
												+ A.Tma_RESTPSDNDHr + A.Tma_RESTPSDNDOTHr
												+ ISNULL(C.Tmm_Misc1NDHr, 0) + ISNULL(C.Tmm_Misc1NDOTHr, 0)
												+ ISNULL(C.Tmm_Misc2NDHr, 0) + ISNULL(c.Tmm_Misc2NDOTHr, 0)
												+ ISNULL(C.Tmm_Misc3NDHr, 0) + ISNULL(C.Tmm_Misc3NDOTHr, 0)
												+ ISNULL(C.Tmm_Misc4NDHr, 0) + ISNULL(C.Tmm_Misc4NDOTHr, 0)
												+ ISNULL(C.Tmm_Misc5NDHr, 0) + ISNULL(C.Tmm_Misc5NDOTHr, 0)
												+ ISNULL(C.Tmm_Misc6NDHr, 0) + ISNULL(C.Tmm_Misc6NDOTHr, 0)) AS Tma_NDAdjHr
										, SUM(B.Tma_NDAdjAmt) AS Tma_NDAdjAmt
										, SUM(B.Tma_TotalAdjAmt) AS Tma_TotalAdjAmt
								FROM T_EmpManualAdjFinalPay A
                                INNER JOIN T_EmpManualAdj2FinalPay B
									ON A.Tma_IDNo = B.Tma_IDNo
									AND A.Tma_PayCycle = B.Tma_PayCycle
                                    AND A.Tma_OrigPayCycle = B.Tma_OrigPayCycle
									AND A.Tma_SalaryRate = B.Tma_SalaryRate
								LEFT JOIN T_EmpManualAdjMiscFinalPay C	
									ON C.Tmm_IDNo = A.Tma_IDNo 
									AND C.Tmm_PayCycle = A.Tma_PayCycle
                                    AND C.Tmm_OrigPayCycle = A.Tma_OrigPayCycle
									AND C.Tmm_SalaryRate = A.Tma_SalaryRate
								WHERE A.Tma_PayCycle = '{0}'
									AND (A.Tma_RetainUserEntry IS NULL OR A.Tma_RetainUserEntry = 0)
                                    AND A.Tma_IDNo IN  ({1})
								GROUP BY A.Tma_IDNo, A.Tma_PayCycle 
                            ) xx 
                            ON xx.Tma_IDNo = Tph_IDNo
                                And xx.Tma_PayCycle = Tph_PayCycle
                            WHERE Tph_PayCycle = '{0}'
                                AND Tph_IDNo IN ({1})
                           
                           UPDATE T_EmpPayTranHdrFinalPay
                            SET Tph_MRGAdjHr = Tma_RGAdjHr
								, Tph_MRGAdjAmt = Tma_RGAdjAmt
								, Tph_MOTAdjHr = Tma_OTAdjHr
								, Tph_MOTAdjAmt = Tma_OTAdjAmt
								, Tph_MHOLAdjHr = Tma_HOLAdjHr
								, Tph_MHOLAdjAmt = Tma_HOLAdjAmt
								, Tph_MNDAdjHr = Tma_NDAdjHr
								, Tph_MNDAdjAmt = Tma_NDAdjAmt
								, Tph_TotalAdjAmt = Tma_TotalAdjAmt
                                , Usr_Login = '{2}'
                                , Ludatetime = GETDATE()
                            FROM T_EmpPayTranHdrFinalPay
                            INNER JOIN (
								SELECT A.Tma_IDNo
										, A.Tma_PayCycle
										, SUM(Tma_REGHr) AS Tma_RGAdjHr
										, SUM(B.Tma_RGAdjAmt) AS Tma_RGAdjAmt
										, SUM(A.Tma_REGOTHr										
											+ A.Tma_RESTHr + A.Tma_RESTOTHr
											+ A.Tma_LEGHOLHr + A.Tma_LEGHOLOTHr
											+ A.Tma_SPLHOLHr + A.Tma_SPLHOLOTHr
											+ A.Tma_PSDHr + A.Tma_PSDOTHr
											+ A.Tma_COMPHOLHr + A.Tma_COMPHOLOTHr
											+ A.Tma_RESTLEGHOLHr + A.Tma_RESTLEGHOLOTHr
											+ A.Tma_RESTSPLHOLHr + A.Tma_RESTSPLHOLOTHr
											+ A.Tma_RESTCOMPHOLHr + A.Tma_RESTCOMPHOLOTHr
											+ A.Tma_RESTPSDHr + A.Tma_RESTPSDOTHr
											+ ISNULL(C.Tmm_Misc1Hr, 0) + ISNULL(C.Tmm_Misc1OTHr, 0)
											+ ISNULL(C.Tmm_Misc2Hr, 0) + ISNULL(C.Tmm_Misc2OTHr, 0)
											+ ISNULL(C.Tmm_Misc3Hr, 0) + ISNULL(C.Tmm_Misc3OTHr, 0)
											+ ISNULL(C.Tmm_Misc4Hr, 0) + ISNULL(C.Tmm_Misc4OTHr, 0)
											+ ISNULL(C.Tmm_Misc5Hr, 0) + ISNULL(C.Tmm_Misc5OTHr, 0)
											+ ISNULL(C.Tmm_Misc6Hr, 0) + ISNULL(C.Tmm_Misc6OTHr, 0)) AS Tma_OTAdjHr
										, SUM(ISNULL(B.Tma_OTAdjAmt, 0)) AS Tma_OTAdjAmt
										, SUM(A.Tma_PDLEGHOLHr 
											+ A.Tma_PDSPLHOLHr 
											+ A.Tma_PDCOMPHOLHr
                                            + A.Tma_PDPSDHr
                                            + A.Tma_PDOTHHOLHr) AS Tma_HOLAdjHr
										, SUM(B.Tma_HOLAdjAmt) AS Tma_HOLAdjAmt
										, SUM(A.Tma_REGNDHr + A.Tma_REGNDOTHr									
												+ A.Tma_RESTNDHr + A.Tma_RESTNDOTHr
												+ A.Tma_LEGHOLNDHr + A.Tma_LEGHOLNDOTHr
												+ A.Tma_SPLHOLNDHr + A.Tma_SPLHOLNDOTHr
												+ A.Tma_PSDNDHr + A.Tma_PSDNDOTHr
												+ A.Tma_COMPHOLNDHr + A.Tma_COMPHOLNDOTHr
												+ A.Tma_RESTLEGHOLNDHr + A.Tma_RESTLEGHOLNDOTHr
												+ A.Tma_RESTSPLHOLNDHr + A.Tma_RESTSPLHOLNDOTHr
												+ A.Tma_RESTCOMPHOLNDHr + A.Tma_RESTCOMPHOLNDOTHr
												+ A.Tma_RESTPSDNDHr + A.Tma_RESTPSDNDOTHr
												+ ISNULL(C.Tmm_Misc1NDHr, 0) + ISNULL(C.Tmm_Misc1NDOTHr, 0)
												+ ISNULL(C.Tmm_Misc2NDHr, 0) + ISNULL(c.Tmm_Misc2NDOTHr, 0)
												+ ISNULL(C.Tmm_Misc3NDHr, 0) + ISNULL(C.Tmm_Misc3NDOTHr, 0)
												+ ISNULL(C.Tmm_Misc4NDHr, 0) + ISNULL(C.Tmm_Misc4NDOTHr, 0)
												+ ISNULL(C.Tmm_Misc5NDHr, 0) + ISNULL(C.Tmm_Misc5NDOTHr, 0)
												+ ISNULL(C.Tmm_Misc6NDHr, 0) + ISNULL(C.Tmm_Misc6NDOTHr, 0)) AS Tma_NDAdjHr
										, SUM(B.Tma_NDAdjAmt) AS Tma_NDAdjAmt
										, SUM(B.Tma_TotalAdjAmt) AS Tma_TotalAdjAmt
								FROM T_EmpManualAdjFinalPay A
                                INNER JOIN T_EmpManualAdj2FinalPay B
									ON A.Tma_IDNo = B.Tma_IDNo
									AND A.Tma_PayCycle = B.Tma_PayCycle
                                    AND A.Tma_OrigPayCycle = B.Tma_OrigPayCycle
									AND A.Tma_SalaryRate = B.Tma_SalaryRate
								LEFT JOIN T_EmpManualAdjMiscFinalPay C	
									ON C.Tmm_IDNo = A.Tma_IDNo 
									AND C.Tmm_PayCycle = A.Tma_PayCycle
                                    AND C.Tmm_OrigPayCycle = A.Tma_OrigPayCycle
									AND C.Tmm_SalaryRate = A.Tma_SalaryRate
								WHERE A.Tma_PayCycle = '{0}'
									AND A.Tma_RetainUserEntry = 1
                                    AND A.Tma_IDNo IN ({1})
								GROUP BY A.Tma_IDNo, A.Tma_PayCycle 
                            ) xx 
                            ON xx.Tma_IDNo = Tph_IDNo
                                And xx.Tma_PayCycle = Tph_PayCycle
                            WHERE Tph_PayCycle = '{0}'
                                AND Tph_IDNo IN ({1})

                           UPDATE T_EmpPayTranHdrFinalPay
                            SET  Tph_TaxableIncomeAmt = TaxAllowanceAmt
                                ,Tph_NontaxableIncomeAmt = NonTaxAllowanceAmt
                                ,Usr_Login = '{2}'
                                ,Ludatetime = GETDATE()
                            FROM T_EmpPayTranHdrFinalPay 
                            INNER JOIN (SELECT Tin_IDNo
                                                ,TaxAllowanceAmt = SUM(CASE WHEN Min_TaxClass = 'T' THEN Tin_IncomeAmt ELSE 0 END)
                                                ,NonTaxAllowanceAmt = SUM(CASE WHEN Min_TaxClass = 'N' THEN Tin_IncomeAmt ELSE 0 END)
                                         FROM T_EmpIncomeFinalPay 
                                         INNER JOIN {4}..M_Income
                                            ON Min_IncomeCode = Tin_IncomeCode
                                            AND Min_CompanyCode = '{3}'
                                         WHERE Tin_PayCycle = '{0}'
                                            AND Min_IncomeGroup NOT IN ('LVR','BON','RET','HSA')
                                            AND Tin_IDNo IN ({1})
                                         GROUP BY Tin_IDNo) Allowance 
                            ON Tph_IDNo = Tin_IDNo
                            WHERE Tph_PayCycle = '{0}'
                                AND Tph_IDNo IN ({1})

                            UPDATE T_EmpSystemAdjFinalPay
                            SET Tsa_PostFlag = 1
                            FROM T_EmpSystemAdjFinalPay
                            INNER JOIN T_EmpPayTranHdrFinalPay
                                ON Tsa_IDNo = Tph_IDNo
                                AND Tsa_AdjPayCycle = Tph_PayCycle
                                AND Tph_IDNo IN ({1})
                            WHERE Tsa_AdjPayCycle = '{0}'
                                AND Tsa_IDNo IN ({1})

                            UPDATE T_EmpSystemAdjFinalPay
                            SET Tsa_PostFlag = 0 ---Unpost if Tma_RetainUserEntry is true
                            FROM T_EmpSystemAdjFinalPay
                            INNER JOIN T_EmpPayTranHdrFinalPay
                                ON Tsa_IDNo = Tph_IDNo
                                AND Tsa_AdjPayCycle = Tph_PayCycle
                            INNER JOIN T_EmpManualAdjFinalPay
                                ON Tma_IDNo = Tsa_IDNo
                                AND Tma_PayCycle = Tsa_AdjPayCycle
                                AND Tma_PayCycle = '{0}'
                                AND Tma_RetainUserEntry = 1
                                AND Tph_IDNo IN ({1})
                            WHERE Tsa_AdjPayCycle = '{0}'
                                AND Tsa_IDNo IN ({1})

                            UPDATE T_EmpManualAdjFinalPay
                            SET Tma_PostFlag = 1
                            FROM T_EmpManualAdjFinalPay 
                            INNER JOIN T_EmpPayTranHdrFinalPay
                                On Tma_IDNo = Tph_IDNo
                                And Tma_PayCycle = Tph_PayCycle
                                AND Tph_IDNo IN ({1})
                            WHERE Tma_PayCycle = '{0}'
                                AND Tma_IDNo IN ({1})

                            UPDATE T_EmpIncomeFinalPay
                            SET Tin_PostFlag = 1
                            FROM T_EmpIncomeFinalPay 
							INNER JOIN {4}..M_Income
								ON Min_IncomeCode = Tin_IncomeCode
                                AND Min_CompanyCode = '{3}' 
							INNER JOIN T_EmpPayTranHdrFinalPay
                                ON Tin_IDNo = Tph_IDNo
                                AND Tin_PayCycle = Tph_PayCycle
                                AND Tph_IDNo IN ({1})
                            WHERE Tin_PayCycle = '{0}'
                                AND Min_IncomeGroup NOT IN ('LVR','BON','RET','HSA') 
                                AND Tin_IDNo IN ({1})"
                            , ProcessPayrollPeriod
                            , strEmployeeListCommaDelimited
                            , LoginUser
                            , CompanyCode
                            , CentralProfile);
                        #endregion
                    dal.ExecuteNonQuery(strTempQuery);
                    StatusHandler(this, new StatusEventArgs("Post Adjustments", true));
                    #endregion

                    for (int j = 0; j < EmployeeList.Length; j++)
                    {
                        drArrEmpDetails = dtEmployeeFinalPay.Select(string.Format("Mem_IDNo = '{0}'", EmployeeList[j]));
                        if (drArrEmpDetails.Length > 0)
                        {
                            #region [COMPUTE PARTIAL GROSS PAY]
                            NewPayrollCalculationBL.SetParameters(companyCode, centralProfile, dal);
                            StatusHandler(this, new StatusEventArgs(string.Format("Compute Gross Pay ({0}/{1})", j + 1, EmployeeList.Length), false));
                            NewPayrollCalculationBL.ComputePayroll(GetValue(drArrEmpDetails[0]["Tef_RegularPayRule"]), ProcessPayrollPeriod, EmployeeList[j], LoginUser, CompanyCode, CentralProfile, menuCode, dal);
                            StatusHandler(this, new StatusEventArgs(string.Format("Compute Gross Pay ({0}/{1})", j + 1, EmployeeList.Length), true));
                            #endregion
                        }
                    }

                    #region [POSTING LEAVE REFUND | RETIREMENT | HOLD SALARY TO PAYROLL TRANSACTION]
                    strTempQuery = "";
                    strTempQuery = string.Format(@"UPDATE T_EmpPayTranHdrFinalPay
                                                        SET  Tph_TaxableIncomeAmt = Tph_TaxableIncomeAmt + TaxAllowanceAmt
                                                           ,Tph_NontaxableIncomeAmt = Tph_NontaxableIncomeAmt + NonTaxAllowanceAmt
                                                           ,Usr_Login = '{4}'
                                                           ,Ludatetime = GETDATE()
                                                        FROM T_EmpPayTranHdrFinalPay 
                                                        INNER JOIN (SELECT Tin_IDNo
                                                                    ,TaxAllowanceAmt = SUM(CASE WHEN Min_TaxClass = 'T' THEN Tin_IncomeAmt ELSE 0 END)
                                                                    ,NonTaxAllowanceAmt = SUM(CASE WHEN Min_TaxClass = 'N' THEN Tin_IncomeAmt ELSE 0 END)
                                                                    FROM T_EmpIncomeFinalPay 
                                                                    INNER JOIN {3}..M_Income
                                                                    ON Min_IncomeCode = Tin_IncomeCode
                                                                    AND Min_CompanyCode = '{2}'
                                                                    WHERE Tin_PayCycle = '{0}'
                                                                        AND Tin_IDNo IN ({1})
                                                                        AND Min_IncomeGroup IN ('LVR','RET','HSA')
                                                                    GROUP BY Tin_IDNo) Allowance 
                                                        ON Tph_IDNo = Tin_IDNo
                                                        WHERE Tph_PayCycle = '{0}'
                                                           AND Tph_IDNo IN ({1})
                                                        
                                                        UPDATE T_EmpPayrollFinalPay
                                                        SET  
                                                            Tpy_TaxableIncomeAmt = Tpy_TaxableIncomeAmt + TaxAllowanceAmt
                                                            ,Tpy_NontaxableIncomeAmt = Tpy_NontaxableIncomeAmt + NonTaxAllowanceAmt
                                                            ,Tpy_TotalTaxableIncomeAmt = Tpy_TotalTaxableIncomeAmt + TaxAllowanceAmt
                                                            ,Tpy_GrossIncomeAmt = Tpy_GrossIncomeAmt + TaxAllowanceAmt + NonTaxAllowanceAmt
                                                            ,Usr_Login = '{4}'
                                                            ,Ludatetime = GETDATE()
                                                        FROM T_EmpPayrollFinalPay 
                                                        INNER JOIN (SELECT Tin_IDNo
                                                                            ,TaxAllowanceAmt = SUM(CASE WHEN Min_TaxClass = 'T' THEN Tin_IncomeAmt ELSE 0 END)
                                                                            ,NonTaxAllowanceAmt = SUM(CASE WHEN Min_TaxClass = 'N' THEN Tin_IncomeAmt ELSE 0 END)
                                                                     FROM T_EmpIncomeFinalPay 
                                                                     INNER JOIN {3}..M_Income
                                                                        ON Min_IncomeCode = Tin_IncomeCode
                                                                        AND Min_CompanyCode = '{2}'
                                                                     WHERE Tin_PayCycle = '{0}'
                                                                        AND Tin_IDNo IN ({1})
                                                                        AND Min_IncomeGroup IN ('LVR','RET','HSA')
                                                                     GROUP BY Tin_IDNo) Allowance 
                                                        ON Tpy_IDNo = Tin_IDNo
                                                        WHERE Tpy_PayCycle = '{0}'
                                                            AND Tpy_IDNo IN ({1}) 

                                                        UPDATE T_EmpFinalPay
                                                        SET Tef_HoldSalary = Tef_HoldSalary + Tin_IncomeAmt
                                                            ,Usr_Login = '{4}'
                                                            ,Ludatetime = GETDATE()
                                                        FROM T_EmpFinalPay 
                                                        INNER JOIN (SELECT Tin_IDNo
                                                                            , SUM(Tin_IncomeAmt) as Tin_IncomeAmt
                                                                     FROM T_EmpIncomeFinalPay 
                                                                     INNER JOIN {3}..M_Income
                                                                        ON Min_IncomeCode = Tin_IncomeCode
                                                                        AND Min_CompanyCode = '{2}'
                                                                     WHERE Tin_PayCycle = '{0}'
                                                                        AND Tin_IDNo IN ({1})
                                                                        AND Min_IncomeGroup IN ('HSA')
                                                                     GROUP BY Tin_IDNo) Allowance 
														ON Tef_IDNo = Tin_IDNo
                                                        WHERE Tef_PayCycle = '{0}'
                                                            AND Tef_IDNo IN ({1}) 

                                                        UPDATE T_EmpFinalPay
                                                        SET Tef_TotalRetirementAmt = ISNULL(Tin_IncomeAmt,0)
                                                            ,Usr_Login = '{4}'
                                                            ,Ludatetime = GETDATE()
                                                        FROM T_EmpFinalPay 
                                                        INNER JOIN (SELECT Tin_IDNo
                                                                            , SUM(Tin_IncomeAmt) as Tin_IncomeAmt
                                                                     FROM T_EmpIncomeFinalPay 
                                                                     INNER JOIN {3}..M_Income
                                                                        ON Min_IncomeCode = Tin_IncomeCode
                                                                        AND Min_CompanyCode = '{2}'
                                                                     WHERE Tin_PayCycle = '{0}'
                                                                        AND Tin_IDNo IN ({1})
                                                                        AND Min_IncomeGroup IN ('RET')
                                                                     GROUP BY Tin_IDNo) Allowance 
														ON Tef_IDNo = Tin_IDNo
                                                        WHERE Tef_PayCycle = '{0}'
                                                            AND Tef_IDNo IN ({1}) 

                                                        UPDATE T_EmpFinalPay
                                                        SET Tef_LeaveRefund = Tef_LeaveRefund + Tin_IncomeAmt
                                                            ,Usr_Login = '{4}'
                                                            ,Ludatetime = GETDATE()
                                                        FROM T_EmpFinalPay 
                                                        INNER JOIN (SELECT Tin_IDNo
                                                                            , SUM(Tin_IncomeAmt) as Tin_IncomeAmt
                                                                     FROM T_EmpIncomeFinalPay 
                                                                     INNER JOIN {3}..M_Income
                                                                        ON Min_IncomeCode = Tin_IncomeCode
                                                                        AND Min_CompanyCode = '{2}'
                                                                     WHERE Tin_PayCycle = '{0}'
                                                                        AND Tin_IDNo IN ({1})
                                                                        AND Min_IncomeGroup IN ('LVR')
                                                                     GROUP BY Tin_IDNo) Allowance 
														ON Tef_IDNo = Tin_IDNo
                                                        WHERE Tef_PayCycle = '{0}'
                                                            AND Tef_IDNo IN ({1}) 
                    
														UPDATE T_EmpIncomeFinalPay
														SET Tin_PostFlag = 1
														From T_EmpIncomeFinalPay 
														INNER JOIN {3}..M_Income
															ON Min_IncomeCode = Tin_IncomeCode
															AND Min_CompanyCode = '{2}' 
														WHERE Tin_PayCycle = '{0}'
															AND Min_IncomeGroup IN ('LVR','RET','HSA')
															AND Tin_IDNo IN ({1})
                                                        "
                                                , ProcessPayrollPeriod
                                                , strEmployeeListCommaDelimited
                                                , CompanyCode
                                                , CentralProfile
                                                , LoginUser);
                    dal.ExecuteNonQuery(strTempQuery);
                    #endregion

                    dal.CommitTransactionSnapshot();
                    dal.BeginTransactionSnapshot();

                    #region [COMPUTE 13TH MONTH PAY]
                    for (int j = 0; j < EmployeeList.Length; j++)
                    {
                        StatusHandler(this, new StatusEventArgs(string.Format("Compute 13th Month Pay ({0}/{1})", j + 1, EmployeeList.Length), false));
                        drArrEmpDetails = dtEmployeeFinalPay.Select(string.Format("Mem_IDNo = '{0}'", EmployeeList[j]));
                        if (drArrEmpDetails.Length > 0)
                        {
                            if (CheckIfBonusAlreadyExists(EmployeeList[j], ProcessPayrollPeriod))
                            {
                                //BONUS ALREADY GIVEN
                            }
                            else
                            {
                                if (Convert.ToBoolean(drArrEmpDetails[0]["Tef_Compute13thMonthPay"])) //Tef_Compute13thMonthPay
                                {
                                    string ControlNumber = string.Empty;
                                    string tmpBonusPayPeriodStart = string.Empty;
                                    string tmpBonusPayPeriodEnd = string.Empty;
                                    bool Recompute = true;
                                    string[] strArrBonusItems = FP13THMONTHNORMAL.Split(new char[] { ',' });
                                    for (int idb = 0; idb < strArrBonusItems.Length; idb++)
                                    {
                                        string BonusCode = GetValue(strArrBonusItems[idb]);

                                        DataRow[] drBonusMaster = dtBonusMaster.Select(string.Format("Mbn_BonusCode = '{0}FP'", BonusCode));
                                        if (drBonusMaster.Length > 0)
                                        {
                                            string BonusNontaxableCode = drBonusMaster[0]["Mbn_NontaxableCode"].ToString().Trim();
                                            string BonusTaxableCode = drBonusMaster[0]["Mbn_TaxableCode"].ToString().Trim();
                                            string BonusFormula = drBonusMaster[0]["Mbn_BonusFormula"].ToString().Trim();

                                            #region Convert Bonus Coverage
                                            string MasterCoverageSelection = drBonusMaster[0]["Mbn_CoverageSelection"].ToString().ToUpper().Trim();
                                            string MasterCoverageStart = drBonusMaster[0]["Mbn_StartCoverage"].ToString().Trim();
                                            string MasterCoverageEnd = drBonusMaster[0]["Mbn_EndCoverage"].ToString().Trim();
                                            string StartYear = (MasterCoverageEnd != string.Empty ? MasterCoverageStart.Substring(MasterCoverageStart.Length - 1) : string.Empty);
                                            string EndYear = (MasterCoverageEnd != string.Empty ? MasterCoverageEnd.Substring(MasterCoverageEnd.Length - 1) : string.Empty);
                                            DateTime dt = Convert.ToDateTime("01/01/" + ProcessPayrollPeriod.Substring(0, 4));
                                            if (FPPROCESSYEARBON != "P") //Separation Year
                                                dt = Convert.ToDateTime(drArrEmpDetails[0]["Mem_SeparationDate"]);

                                            if (MasterCoverageSelection == "P")
                                            {
                                                tmpBonusPayPeriodStart = (StartYear == "P" ? dt.AddYears(-1).Year + MasterCoverageStart.Replace(StartYear, "") : dt.Year + MasterCoverageStart.Replace(StartYear, ""));
                                                tmpBonusPayPeriodEnd = (EndYear == "P" ? dt.AddYears(-1).Year + MasterCoverageEnd.Replace(EndYear, "") : dt.Year + MasterCoverageEnd.Replace(EndYear, ""));
                                            }
                                            else if (MasterCoverageSelection == "D")
                                            {
                                                tmpBonusPayPeriodStart = (StartYear == "P" ? MasterCoverageStart.Replace(StartYear, "") + "/" + dt.AddYears(-1).Year : MasterCoverageStart.Replace(StartYear, "") + "/" + dt.Year);
                                                tmpBonusPayPeriodEnd = (EndYear == "P" ? MasterCoverageEnd.Replace(EndYear, "") + "/" + dt.AddYears(-1).Year : MasterCoverageEnd.Replace(EndYear, "") + "/" + dt.Year);
                                            }
                                            else if (MasterCoverageSelection == "E")
                                            {
                                                //OPEN ENTRY
                                            }
                                            #endregion

                                            ControlNumber = BonusControlNumberExists(ProcessPayrollPeriod, string.Format("{0}FP", BonusCode), dal);
                                            if (ControlNumber == "")
                                                Recompute = false;

                                            ControlNumber = BonusComputation13thMonthBL.ComputeBonus(Recompute
                                                                                                    , ProcessPayrollPeriod
                                                                                                    , EmployeeList[j]
                                                                                                    , LoginUser
                                                                                                    , companyCode
                                                                                                    , CentralProfile
                                                                                                    , profileCode
                                                                                                    , ControlNumber
                                                                                                    , string.Format("WHERE Mem_IDNo = '{0}'", EmployeeList[j])
                                                                                                    , string.Format("{0}FP", BonusCode)
                                                                                                    , tmpBonusPayPeriodStart
                                                                                                    , tmpBonusPayPeriodEnd
                                                                                                    , dal);



                                            #region [POSTING OF BONUS TO EMPLOYEE ALLOWANCE]
                                            BonusComputation13thMonthBL.PostToIncome(true, EmployeeList[j], ProcessPayrollPeriod, ControlNumber
                                                    , BonusNontaxableCode, BonusTaxableCode, LoginUser, companyCode, CentralProfile, dal);

                                            #endregion

                                            #region [UPDATE T_EmpFinalPay]
                                            ParameterInfo[] paramUpdateBonus = new ParameterInfo[6];
                                            paramUpdateBonus[0] = new ParameterInfo("@Tef_IDNo", EmployeeList[j], SqlDbType.NVarChar, 4000);
                                            paramUpdateBonus[1] = new ParameterInfo("@Tef_PayCycle", ProcessPayrollPeriod);
                                            paramUpdateBonus[2] = new ParameterInfo("@Tef_13thMonthPayStart", tmpBonusPayPeriodStart);
                                            paramUpdateBonus[3] = new ParameterInfo("@Tef_13thMonthPayEnd", tmpBonusPayPeriodEnd);
                                            paramUpdateBonus[4] = new ParameterInfo("@Tef_13thMonthPayFormula", BonusFormula, SqlDbType.NVarChar, 4000);
                                            paramUpdateBonus[5] = new ParameterInfo("@Usr_Login", LoginUser);

                                            strTempQuery = "";
                                            strTempQuery = @"UPDATE T_EmpFinalPay 
                                                           SET Tef_13thMonthPayStart = @Tef_13thMonthPayStart
                                                           , Tef_13thMonthPayEnd = @Tef_13thMonthPayEnd
                                                           , Tef_13thMonthPayFormula = @Tef_13thMonthPayFormula
                                                           , Usr_Login = @Usr_Login
                                                           , Ludatetime = GETDATE()
                                                           WHERE Tef_IDNo = @Tef_IDNo
                                                                AND Tef_PayCycle = @Tef_PayCycle";
                                            dal.ExecuteNonQuery(strTempQuery, CommandType.Text, paramUpdateBonus);
                                            #endregion
                                        }
                                    }
                                }

                                #region [POSTING OF BONUS ALLOWANCE TO PAYROLL TRANSACTION]
                                strTempQuery = "";
                                strTempQuery = string.Format(@"
                                                        UPDATE T_EmpPayTranHdrFinalPay
                                                         SET  Tph_TaxableIncomeAmt = Tph_TaxableIncomeAmt + TaxAllowanceAmt
                                                           ,Tph_NontaxableIncomeAmt = Tph_NontaxableIncomeAmt + NonTaxAllowanceAmt
                                                            ,Usr_Login = '{4}'
                                                            ,Ludatetime = GETDATE()
                                                         FROM T_EmpPayTranHdrFinalPay 
                                                        INNER JOIN (SELECT Tin_IDNo
                                                                           ,TaxAllowanceAmt = SUM(CASE WHEN Min_TaxClass = 'T' THEN Tin_IncomeAmt ELSE 0 END)
                                                                           ,NonTaxAllowanceAmt = SUM(CASE WHEN Min_TaxClass = 'N' THEN Tin_IncomeAmt ELSE 0 END)
                                                                    FROM T_EmpIncomeFinalPay 
                                                                     INNER JOIN {3}..M_Income
                                                                        ON Min_IncomeCode = Tin_IncomeCode
                                                                        AND Min_CompanyCode = '{2}'
                                                                    WHERE Tin_PayCycle = '{0}'
                                                                       AND Tin_IDNo = '{1}'
                                                                       AND Min_IncomeGroup IN ('BON')
                                                                    GROUP BY Tin_IDNo) Allowance 
                                                        ON Tph_IDNo = Tin_IDNo
                                                        WHERE Tph_PayCycle = '{0}'
                                                            AND Tph_IDNo = '{1}'
                                                        
                                                        UPDATE T_EmpPayrollFinalPay
                                                        SET  
                                                            Tpy_TaxableIncomeAmt = Tpy_TaxableIncomeAmt + TaxAllowanceAmt
                                                            ,Tpy_NontaxableIncomeAmt = Tpy_NontaxableIncomeAmt + NonTaxAllowanceAmt
                                                            ,Tpy_TotalTaxableIncomeAmt = Tpy_TotalTaxableIncomeAmt + TaxAllowanceAmt
                                                            ,Tpy_GrossIncomeAmt = Tpy_GrossIncomeAmt + TaxAllowanceAmt + NonTaxAllowanceAmt
                                                            ,Usr_Login = '{4}'
                                                            ,Ludatetime = GETDATE()
                                                        FROM T_EmpPayrollFinalPay 
                                                        INNER JOIN (SELECT Tin_IDNo
                                                                            ,TaxAllowanceAmt = SUM(CASE WHEN Min_TaxClass = 'T' THEN Tin_IncomeAmt ELSE 0 END)
                                                                            ,NonTaxAllowanceAmt = SUM(CASE WHEN Min_TaxClass = 'N' THEN Tin_IncomeAmt ELSE 0 END)
                                                                     FROM T_EmpIncomeFinalPay 
                                                                     INNER JOIN {3}..M_Income
                                                                        ON Min_IncomeCode = Tin_IncomeCode
                                                                        AND Min_CompanyCode = '{2}'
                                                                     WHERE Tin_PayCycle = '{0}'
                                                                        AND Tin_IDNo = '{1}'
                                                                        AND Min_IncomeGroup IN ('BON')
                                                                     GROUP BY Tin_IDNo) Allowance 
                                                        ON Tpy_IDNo = Tin_IDNo
                                                        WHERE Tpy_PayCycle = '{0}'
                                                            AND Tpy_IDNo = '{1}'
                                                        
                                                        UPDATE T_EmpFinalPay
                                                        SET  Tef_13thMonthPay = Tef_13thMonthPay + Tin_IncomeAmt
                                                            ,Usr_Login = '{4}'
                                                            ,Ludatetime = GETDATE()
                                                        FROM T_EmpFinalPay 
                                                        INNER JOIN (SELECT Tin_IDNo, Tin_PayCycle, SUM(Tin_IncomeAmt) AS Tin_IncomeAmt
                                                                     FROM T_EmpIncomeFinalPay 
                                                                     INNER JOIN {3}..M_Income
                                                                        ON Min_IncomeCode = Tin_IncomeCode
                                                                        AND Min_CompanyCode = '{2}'
                                                                     WHERE Tin_PayCycle = '{0}'
                                                                        AND Tin_IDNo = '{1}'
                                                                        AND Min_IncomeGroup IN ('BON')
                                                                     GROUP BY Tin_IDNo, Tin_PayCycle) Allowance 
                                                                        ON Tef_IDNo = Allowance.Tin_IDNo
																		AND Tef_PayCycle = Allowance.Tin_PayCycle
                                                        WHERE Tef_PayCycle = '{0}'
                                                            AND Tef_IDNo IN ({1}) 

                                                        UPDATE T_EmpIncomeFinalPay
														SET Tin_PostFlag = 1
														FROM T_EmpIncomeFinalPay 
														INNER JOIN {3}..M_Income ON Min_IncomeCode = Tin_IncomeCode
															AND Min_CompanyCode = '{2}' 
														WHERE Tin_PayCycle = '{0}'
															AND Min_IncomeGroup IN ('BON')
															AND Tin_IDNo = '{1}'"
                                                            , ProcessPayrollPeriod
                                                            , EmployeeList[j]
                                                            , CompanyCode
                                                            , CentralProfile
                                                            , LoginUser);
                                dal.ExecuteNonQuery(strTempQuery);
                                #endregion

                                
                            }
                        }
                        StatusHandler(this, new StatusEventArgs(string.Format("Compute 13th Month Pay ({0}/{1})", j + 1, EmployeeList.Length), true));
                    }
                    #endregion

                    #region [UPDATE LAST SALARY]
                    for (int j = 0; j < EmployeeList.Length; j++)
                    {
                        strTempQuery = "";
                        strTempQuery = string.Format(@"UPDATE T_EmpFinalPay 
                                                        SET Tef_LastSalary = (SELECT ISNULL((SELECT Tpy_GrossIncomeAmt  
		                                                                                FROM T_EmpPayrollFinalPay   
		                                                                                WHERE Tpy_PayCycle = '{1}'   
		                                                                                AND Tpy_IDNo = '{0}'),0) -
                                                                              (SELECT ISNULL(SUM(Tin_IncomeAmt), 0)  
                                                                              FROM T_EmpIncomeFinalPay  
                                                                              WHERE Tin_PayCycle = '{1}' 
                                                                                AND Tin_IDNo = '{0}' 
                                                                                AND Tin_PostFlag =  1
                                                                                AND Tin_IncomeCode IN (SELECT Min_IncomeCode FROM {3}..M_Income
	                                                                                                            WHERE Min_CompanyCode = '{2}'
	                                                                                                            AND Min_IncomeGroup IN ('LVR','BON','RET','HSA'))
                                                                                    ))
                                                        WHERE Tef_IDNo = '{0}'
                                                        AND Tef_PayCycle = '{1}'"
                                                        , EmployeeList[j]
                                                        , ProcessPayrollPeriod
                                                        , CompanyCode
                                                        , CentralProfile);
                        dal.ExecuteNonQuery(strTempQuery);
                    }

                    #endregion

                    dal.CommitTransactionSnapshot();
                    dal.BeginTransactionSnapshot();

                    #region [COMPUTE TAX, DEDUCTIONS AND NET PAY]
                    if (bComputeTax)
                    {
                        dtEmployeeFinalPay = null;
                        dtEmployeeFinalPay = GetAllEmployeeForProcess(strEmployeeListCommaDelimited, ProcessPayrollPeriod);

                        for (int j = 0; j < EmployeeList.Length; j++)
                        {
                            StatusHandler(this, new StatusEventArgs(string.Format("Compute Tax, Deductions and Net Pay"), false));
                            drArrEmpDetails = dtEmployeeFinalPay.Select(string.Format("Mem_IDNo = '{0}'", EmployeeList[j]));
                            if (drArrEmpDetails.Length > 0)
                            {
                                strTempQuery = string.Format(@"SELECT * FROM T_EmpPayrollFinalPay
                                                    WHERE Tpy_IDNo = '{0}'  
                                                        AND Tpy_PayCycle = '{1}'"
                                                        , EmployeeList[j]
                                                        , ProcessPayrollPeriod);
                                dtPayCalcRec = dal.ExecuteDataSet(strTempQuery).Tables[0];

                                strTempQuery = string.Format(@"SELECT * FROM T_EmpPayroll2FinalPay
                                                    WHERE Tpy_IDNo = '{0}'
                                                        AND Tpy_PayCycle = '{1}'"
                                                                , EmployeeList[j]
                                                                , ProcessPayrollPeriod);
                                dtPayCalcExt2Rec = dal.ExecuteDataSet(strTempQuery).Tables[0];

                                if (dtPayCalcRec.Rows.Count > 0)
                                {
                                    #region Tax Computation
                                    bool bAnnualize = false;
                                    if (strTaxMethod.Equals("A"))
                                        bAnnualize = true;

                                    #region Insert Deductions
                                    insertDeductionDetailSepExtension = "";
                                    insertDeductionDetailSepExtension = string.Format(" AND Mdn_DeductionCode in ({0}) ", EncodeFilterItems(GetValue(drArrEmpDetails[0]["Tef_ZeroOutDeductionCodes"])));
                                    #region Create Deduction Details
                                    strTempQuery = string.Format(@"
                                                        DELETE FROM T_EmpDeductionDtlFinalPay WHERE Tdd_IDNo = '{2}'
                                                        INSERT INTO T_EmpDeductionDtlFinalPay
                                                        (Tdd_IDNo
                                                           ,Tdd_DeductionCode
                                                           ,Tdd_StartDate
                                                           ,Tdd_ThisPayCycle
                                                           ,Tdd_PayCycle
                                                           ,Tdd_LineNo
                                                           ,Tdd_PaymentType
                                                           ,Tdd_Amount
                                                           ,Tdd_DeferredFlag
                                                           ,Tdd_PaymentFlag
                                                           ,Usr_Login
                                                           ,Ludatetime)
                                                        -- For Deffered Cycles
                                                        SELECT T_EmpDeductionDefer.Tdd_IDNo
                                                                , T_EmpDeductionDefer.Tdd_DeductionCode
                                                                , T_EmpDeductionDefer.Tdd_StartDate
                                                                , '{3}' AS Tdd_ThisPayCycle
                                                                , T_EmpDeductionDefer.Tdd_PayCycle AS Tdd_PayCycle
                                                                , T_EmpDeductionDefer.Tdd_LineNo AS Tdd_LineNo
                                                                , 'P' AS Tdd_PaymentType
                                                                , T_EmpDeductionDefer.Tdd_DeferredAmt AS Tdd_Amount
                                                                , 1 AS Tdd_DeferredFlag
                                                                , 1 AS Tdd_PaymentFlag
                                                                , '{4}'
                                                                , GETDATE()
                                                        FROM {1}..T_EmpDeductionHdr
                                                        INNER JOIN T_EmpDeductionDefer ON T_EmpDeductionDefer.Tdd_IDNo = T_EmpDeductionHdr.Tdh_IDNo
                                                            AND T_EmpDeductionDefer.Tdd_DeductionCode = T_EmpDeductionHdr.Tdh_DeductionCode 
                                                            AND T_EmpDeductionDefer.Tdd_StartDate = T_EmpDeductionHdr.Tdh_StartDate 
                                                        INNER JOIN {1}..M_Deduction ON Mdn_DeductionCode = Tdh_DeductionCode
                                                            AND Mdn_CompanyCode = '{5}'
                                                            {0}
                                                        WHERE T_EmpDeductionHdr.Tdh_IDNo = '{2}' 
                                                            AND Tdh_ExemptInPayroll = 0

                                                        UNION
                                                        -- For Current Cycles
                                                        SELECT T_EmpDeductionHdr.Tdh_IDNo
                                                                , T_EmpDeductionHdr.Tdh_DeductionCode
                                                                , T_EmpDeductionHdr.Tdh_StartDate
                                                                , '{3}' AS Tdd_ThisPayCycle
                                                                , '{3}' AS Tdd_PayCycle
                                                                , '00' AS Tdd_LineNo
                                                                , 'P' AS Tdd_PaymentType
                                                                , CASE WHEN (Tdh_DeferredAmount + Tdh_PaidAmount + Tdh_CycleAmount) > Tdh_DeductionAmount THEN
                                                                       Tdh_DeductionAmount - (Tdh_DeferredAmount + Tdh_PaidAmount)
                                                                  ELSE
                                                                       (Tdh_DeductionAmount - Tdh_PaidAmount) - Tdh_DeferredAmount
                                                                  END AS Tdd_Amount
                                                                , 0 AS Tdd_DeferredFlag
                                                                , 1 AS Tdd_PaymentFlag
                                                                , '{4}'
                                                                , GETDATE()
                                                        FROM {1}..T_EmpDeductionHdr
                                                        INNER JOIN {1}..M_Deduction ON Mdn_DeductionCode = Tdh_DeductionCode
                                                            AND Mdn_CompanyCode = '{5}'
                                                            {0}
                                                        WHERE T_EmpDeductionHdr.Tdh_IDNo = '{2}'
                                                            AND Tdh_DeferredAmount + Tdh_PaidAmount < Tdh_DeductionAmount
                                                            AND Tdh_CycleAmount > 0
                                                            AND Tdh_ExemptInPayroll = 0 ", insertDeductionDetailSepExtension
                                                                                         , CentralProfile //DeductFullAmount.ToString().ToUpper()
                                                                                         , EmployeeList[j]
                                                                                         , ProcessPayrollPeriod
                                                                                         , LoginUser
                                                                                         , CompanyCode);
                                    #endregion
                                    dal.ExecuteNonQuery(strTempQuery);
                                    #endregion

                                    NewPayrollCalculationBL.ComputeWithholdingTax(bAnnualize
                                                                                        , true
                                                                                        , GetValue(drArrEmpDetails[0]["Tps_TaxSchedule"])
                                                                                        , strTaxMethod
                                                                                        , false
                                                                                        , dtPayCalcRec.Rows[0]
                                                                                        , dtPayCalcExt2Rec.Rows[0]
                                                                                        , !Convert.ToBoolean(dtPayCalcRec.Rows[0]["Tpy_IsTaxExempted"])
                                                                                        , CompanyCode
                                                                                        , CentralProfile
                                                                                        , PayrollAssume13thMonth
                                                                                        , dal);

                                    bPayERShare = Convert.ToBoolean(drArrEmpDetails[0]["Tef_PayERShare"]);
                                    if (!bPayERShare)
                                    {
                                        #region Update Employer Share and EC Fund
                                        DataTable dtPremiumMaxShare = NewPayrollCalculationBL.GetMaxPrem(ProcessPayrollPeriod);
                                        drTemp = dtPremiumMaxShare.Select("DeductionCode = 'SSSPREM'");
                                        if (drTemp.Length > 0)
                                            MaxSSSPremPayCycle = GetValue(drTemp[0]["MaxPayCycle"]);

                                        dtTemp = GetEmployerShareAndECFund(EmployeeList[j], ProcessPayrollPeriod, MaxSSSPremPayCycle, CompanyCode, CentralProfile, dal);
                                        if (dtTemp != null && dtTemp.Rows.Count > 0)
                                        {
                                            for (int i = 0; i < dtTemp.Rows.Count; i++)
                                            {
                                                if (dtTemp.Rows[i]["DeductionGroup"].ToString() == "SA")
                                                {
                                                    dtPayCalcRec.Rows[0]["Tpy_SSSER"] = dtTemp.Rows[i]["ERShare"];
                                                    dtPayCalcRec.Rows[0]["Tpy_ECFundAmt"] = dtTemp.Rows[i]["ECFund"];
                                                }
                                                else if (dtTemp.Rows[i]["DeductionGroup"].ToString() == "MA")
                                                    dtPayCalcRec.Rows[0]["Tpy_MPFER"] = dtTemp.Rows[i]["ERShare"];
                                                else if (dtTemp.Rows[i]["DeductionGroup"].ToString() == "PA")
                                                    dtPayCalcRec.Rows[0]["Tpy_PhilhealthER"] = dtTemp.Rows[i]["ERShare"];
                                                else if (dtTemp.Rows[i]["DeductionGroup"].ToString() == "HA")
                                                    dtPayCalcRec.Rows[0]["Tpy_PagIbigER"] = dtTemp.Rows[i]["ERShare"];

                                            }
                                        }
                                        #endregion
                                    }

                                    #endregion

                                    #region Compute Deductions and Net Pay
                                    #region Variable Declaration and Initialization
                                    dWTaxAmt            = 0;
                                    dWTaxRef            = 0;
                                    dPartialDeduction   = 0;
                                    dCurrentNetPay      = 0;
                                    dMinimumNetPay      = 0;
                                    dOtherDeduction     = 0;
                                    dTotalDeduction     = 0;
                                    deductionAmt        = 0;
                                    dNetPayAmt          = 0;
                                    dGrossIncomeAmt     = 0;
                                    #endregion

                                    #region Get Record Details and Compute Amounts
                                    //only deduct tax if the amount is positive
                                    if (Convert.ToDecimal(dtPayCalcRec.Rows[0]["Tpy_WtaxAmt"].ToString()) > 0)
                                        dWTaxAmt = Convert.ToDecimal(dtPayCalcRec.Rows[0]["Tpy_WtaxAmt"].ToString());
                                    else
                                    {
                                        dWTaxRef = Convert.ToDecimal(dtPayCalcRec.Rows[0]["Tpy_WtaxAmt"].ToString());
                                        dWTaxRef = dWTaxRef * -1;
                                    }

                                    dOtherDeduction = 0;
                                    dPartialDeduction = dWTaxAmt +
                                                          Convert.ToDecimal(dtPayCalcRec.Rows[0]["Tpy_SSSEE"].ToString()) +
                                                          Convert.ToDecimal(dtPayCalcRec.Rows[0]["Tpy_PhilhealthEE"].ToString()) +
                                                          Convert.ToDecimal(dtPayCalcRec.Rows[0]["Tpy_PagIbigEE"].ToString()) +
                                                          Convert.ToDecimal(dtPayCalcRec.Rows[0]["Tpy_UnionAmt"].ToString()) +
                                                          Convert.ToDecimal(dtPayCalcRec.Rows[0]["Tpy_OtherDeductionAmt"].ToString());

                                    dCurrentNetPay = Convert.ToDecimal(dtPayCalcRec.Rows[0]["Tpy_TotalTaxableIncomeAmt"].ToString()) -
                                                       dPartialDeduction +
                                                       dWTaxRef +
                                                       Convert.ToDecimal(dtPayCalcRec.Rows[0]["Tpy_NontaxableIncomeAmt"].ToString()); //+
                                                                                                                                      //Convert.ToDecimal(dtPayCalcRec.Rows[0]["Tpy_NontaxableAdjAmt"].ToString());

                                    //Compare with minimum take home pay parameter
                                    if (MINNETPAY > 0 && dMinimumNetPay < MINNETPAY)
                                        dMinimumNetPay = MINNETPAY;

                                    dsDeductions = GetDeductionDetails(dtPayCalcRec.Rows[0]["Tpy_IDNo"].ToString(), dal);
                                    deductionAmt = 0;
                                    foreach (DataRow drDeduction in dsDeductions.Tables[0].Rows)
                                    {
                                        deductionAmt = Convert.ToDecimal(drDeduction["Tdd_Amount"].ToString());

                                        //if (CheckMinimumNetPay == true)
                                        //{
                                        //    if (dCurrentNetPay - Convert.ToDecimal(drDeduction["Tdd_Amount"].ToString()) >= dMinimumNetPay)
                                        //    {
                                        //        dCurrentNetPay -= Convert.ToDecimal(drDeduction["Tdd_Amount"].ToString());
                                        //        UpdateEmployeeDeductionDetail(drDeduction, dal);
                                        //        dOtherDeduction += Convert.ToDecimal(drDeduction["Tdd_Amount"].ToString());
                                        //    }
                                        //}
                                        //else
                                        //{
                                        //Force Deduct 
                                        dCurrentNetPay -= Convert.ToDecimal(drDeduction["Tdd_Amount"].ToString());
                                        UpdateEmployeeDeductionDetail(drDeduction, dal);
                                        dOtherDeduction += Convert.ToDecimal(drDeduction["Tdd_Amount"].ToString());
                                        //}
                                    }

                                    //Update other deduction amount
                                    dtPayCalcRec.Rows[0]["Tpy_OtherDeductionAmt"] = dOtherDeduction;

                                    dTotalDeduction = dWTaxAmt +
                                                      Convert.ToDecimal(dtPayCalcRec.Rows[0]["Tpy_SSSEE"].ToString()) +
                                                      Convert.ToDecimal(dtPayCalcRec.Rows[0]["Tpy_PhilhealthEE"].ToString()) +
                                                      Convert.ToDecimal(dtPayCalcRec.Rows[0]["Tpy_PagIbigEE"].ToString()) +
                                                      Convert.ToDecimal(dtPayCalcRec.Rows[0]["Tpy_UnionAmt"].ToString()) +
                                                      //Convert.ToDecimal(dtPayCalcExt2Rec.Rows[0]["Tpy_OtherPrem2Share"].ToString()) +
                                                      dOtherDeduction;
                                    //Update total deduction amount
                                    dtPayCalcRec.Rows[0]["Tpy_TotalDeductionAmt"] = dTotalDeduction;

                                    dNetPayAmt = Convert.ToDecimal(dtPayCalcRec.Rows[0]["Tpy_TotalTaxableIncomeAmt"].ToString()) +
                                                  Convert.ToDecimal(dtPayCalcRec.Rows[0]["Tpy_NontaxableIncomeAmt"].ToString()) -
                                                  //Convert.ToDecimal(dtPayCalcRec.Rows[0]["Tpy_NontaxableAdjAmt"].ToString()) -
                                                  dTotalDeduction +
                                                  dWTaxRef;
                                    //Update total deduction amount
                                    dtPayCalcRec.Rows[0]["Tpy_NetAmt"] = dNetPayAmt;

                                    dGrossIncomeAmt = Convert.ToDecimal(dtPayCalcRec.Rows[0]["Tpy_GrossIncomeAmt"]) + dWTaxRef;
                                    dtPayCalcRec.Rows[0]["Tpy_GrossIncomeAmt"] = dGrossIncomeAmt;

                                    #endregion

                                    #region Payroll2
                                    NewPayrollCalculationBL.ComputeIncomeAndDeductionAdjustments(true, dtPayCalcRec.Rows[0], dtPayCalcExt2Rec.Rows[0]);
                                    #endregion

                                    #region Save records
                                    strTempQuery = "";
                                    strTempQuery = string.Format(@"UPDATE T_EmpPayrollFinalPay 
                                                        SET Tpy_WtaxAmt = {2}
                                                            ,Tpy_OtherDeductionAmt = {3}
                                                            ,Tpy_TotalDeductionAmt = {4}
                                                            ,Tpy_NetAmt = {5}
                                                            ,Tpy_TaxBracket = {6}
                                                            ,Tpy_GrossIncomeAmt = {7}
                                                            ,Tpy_SSSER = {8}
                                                            ,Tpy_ECFundAmt = {9}
                                                            ,Tpy_MPFER = {10}
                                                            ,Tpy_PhilhealthER = {11}
                                                            ,Tpy_PagIbigER = {12}
                                                        WHERE Tpy_IDNo = '{0}'
                                                        AND Tpy_PayCycle = '{1}'"
                                                                                    , dtPayCalcRec.Rows[0]["Tpy_IDNo"]
                                                                                    , dtPayCalcRec.Rows[0]["Tpy_PayCycle"]
                                                                                    , dtPayCalcRec.Rows[0]["Tpy_WtaxAmt"]
                                                                                    , dtPayCalcRec.Rows[0]["Tpy_OtherDeductionAmt"]
                                                                                    , dtPayCalcRec.Rows[0]["Tpy_TotalDeductionAmt"]
                                                                                    , dtPayCalcRec.Rows[0]["Tpy_NetAmt"]
                                                                                    , dtPayCalcRec.Rows[0]["Tpy_TaxBracket"]
                                                                                    , dtPayCalcRec.Rows[0]["Tpy_GrossIncomeAmt"]
                                                                                    , dtPayCalcRec.Rows[0]["Tpy_SSSER"]
                                                                                    , dtPayCalcRec.Rows[0]["Tpy_ECFundAmt"]
                                                                                    , dtPayCalcRec.Rows[0]["Tpy_MPFER"]
                                                                                    , dtPayCalcRec.Rows[0]["Tpy_PhilhealthER"]
                                                                                    , dtPayCalcRec.Rows[0]["Tpy_PagIbigER"]);
                                    dal.ExecuteNonQuery(strTempQuery);
                                    strTempQuery = "";
                                    strTempQuery = string.Format(@"UPDATE T_EmpPayroll2FinalPay
                                                SET Tpy_RemainingPayCycle={2}
                                                    ,Tpy_YTDRegularAmtBefore={3}
                                                    ,Tpy_AssumeRegularAmt={4}
                                                    ,Tpy_RegularTotal={5}
                                                    ,Tpy_YTDRecurringAllowanceAmtBefore={6}
                                                    ,Tpy_YTDRecurringAllowanceAmt = {7}
                                                    ,Tpy_RecurringAllowanceAmt={8}
                                                    ,Tpy_AssumeRecurringAllowanceAmt={9}
                                                    ,Tpy_RecurringAllowanceTotal={10}
                                                    ,Tpy_YTDBonusAmtBefore={11}
                                                    ,Tpy_BonusNontaxAmt={12}
                                                    ,Tpy_BonusTaxAmt={13}
                                                    ,Tpy_Assume13thMonthAmt={14}
                                                    ,Tpy_BonusTotal={15}
                                                    ,Tpy_BonusTaxRevaluated={16}
                                                    ,Tpy_YTDBonusTaxBefore={17}
                                                    ,Tpy_YTDOtherTaxableIncomeAmtBefore={18}
                                                    ,Tpy_OtherTaxableIncomeAmt={19}
                                                    ,Tpy_OtherTaxableIncomeTotal={20}
                                                    ,Tpy_YTDSSSAmtBefore={21} 
                                                    ,Tpy_AssumeSSSAmt={22}
                                                    ,Tpy_SSSTotal={23}
                                                    ,Tpy_YTDPhilhealthAmtBefore={24}
                                                    ,Tpy_AssumePhilhealthAmt={25}
                                                    ,Tpy_PhilhealthTotal={26}
                                                    ,Tpy_YTDPagIbigAmtBefore={27}
                                                    ,Tpy_AssumePagIbigAmt={28}
                                                    ,Tpy_PagIbigTotal={29}
                                                    ,Tpy_YTDPagIbigTaxAmtBefore={30}
                                                    ,Tpy_AssumePagIbigTaxAmt={31}
                                                    ,Tpy_TotalTaxableIncomeWAssumeAmt={32}
                                                    ,Tpy_YTDUnionAmtBefore={33}
                                                    ,Tpy_AssumeUnionAmt={34}
                                                    ,Tpy_UnionTotal={35}
                                                    ,Tpy_PremiumPaidOnHealth={36}
                                                    ,Tpy_TotalExemptions={37}
                                                    ,Tpy_NetTaxableIncomeAmt={38}
                                                    ,Tpy_YTDREGAmt={39}
                                                    ,Tpy_YTDTotalTaxableIncomeAmtBefore={40}
                                                    ,Tpy_YTDTotalTaxableIncomeAmt={41}
                                                    ,Tpy_YTDWtaxAmtBefore={42}
                                                    ,Tpy_YTDWtaxAmt={43}
                                                    ,Tpy_YTDSSSAmt={44}
                                                    ,Tpy_YTDPhilhealthAmt={45}
                                                    ,Tpy_YTDPagIbigAmt={46}
                                                    ,Tpy_YTDPagIbigTaxAmt={47}
                                                    ,Tpy_YTDUnionAmt={48}
                                                    ,Tpy_YTDNontaxableAmtBefore={49}
                                                    ,Tpy_YTDNontaxableAmt={50}
                                                    ,Tpy_BIRTotalAmountofCompensation={51}
                                                    ,Tpy_BIRStatutoryMinimumWage={52}
                                                    ,Tpy_BIRHolidayOvertimeNightShiftHazard={53}
                                                    ,Tpy_BIR13thMonthPayOtherBenefits={54}
                                                    ,Tpy_BIRDeMinimisBenefits={55}
                                                    ,Tpy_BIRSSSGSISPHICHDMFUnionDues={56}
                                                    ,Tpy_BIROtherNonTaxableCompensation={57}
                                                    ,Tpy_BIRTotalTaxableCompensation={58}
                                                    ,Tpy_BIRTaxableCompensationNotSubjectToWithholdingTax={59}
                                                    ,Tpy_BIRTotalTaxesWithheld={60}
                                                    ,Tpy_TaxDue={61}
                                                    ,Tpy_AssumeMonthlyRegularAmt={62}
                                                    ,Tpy_AssumeMonthlySSSAmt={63}
                                                    ,Tpy_AssumeMonthlyPhilhealthAmt={64}
                                                    ,Tpy_AssumeMonthlyPagibigAmt={65}
                                                    ,Tpy_AssumeMonthlyUnionAmt={66}
                                                    ,Tpy_YTDMPFAmtBefore={67}
                                                    ,Tpy_YTDMPFAmt={68}
                                                WHERE Tpy_IDNo = '{0}'
                                                AND Tpy_PayCycle = '{1}'"
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_IDNo"]                              //0
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_PayCycle"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_RemainingPayCycle"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_YTDRegularAmtBefore"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_AssumeRegularAmt"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_RegularTotal"]                      //5
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_YTDRecurringAllowanceAmtBefore"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_YTDRecurringAllowanceAmt"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_RecurringAllowanceAmt"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_AssumeRecurringAllowanceAmt"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_RecurringAllowanceTotal"]           //10
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_YTDBonusAmtBefore"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_BonusNontaxAmt"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_BonusTaxAmt"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_Assume13thMonthAmt"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_BonusTotal"]                        //15
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_BonusTaxRevaluated"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_YTDBonusTaxBefore"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_YTDOtherTaxableIncomeAmtBefore"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_OtherTaxableIncomeAmt"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_OtherTaxableIncomeTotal"]           //20
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_YTDSSSAmtBefore"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_AssumeSSSAmt"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_SSSTotal"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_YTDPhilhealthAmtBefore"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_AssumePhilhealthAmt"]               //25
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_PhilhealthTotal"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_YTDPagIbigAmtBefore"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_AssumePagIbigAmt"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_PagIbigTotal"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_YTDPagIbigTaxAmtBefore"]            //30
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_AssumePagIbigTaxAmt"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_TotalTaxableIncomeWAssumeAmt"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_YTDUnionAmtBefore"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_AssumeUnionAmt"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_UnionTotal"]                        //35
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_PremiumPaidOnHealth"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_TotalExemptions"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_NetTaxableIncomeAmt"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_YTDREGAmt"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_YTDTotalTaxableIncomeAmtBefore"]    //40
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_YTDTotalTaxableIncomeAmt"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_YTDWtaxAmtBefore"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_YTDWtaxAmt"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_YTDSSSAmt"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_YTDPhilhealthAmt"]                  //45
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_YTDPagIbigAmt"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_YTDPagIbigTaxAmt"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_YTDUnionAmt"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_YTDNontaxableAmtBefore"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_YTDNontaxableAmt"]                  //50
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_BIRTotalAmountofCompensation"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_BIRStatutoryMinimumWage"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_BIRHolidayOvertimeNightShiftHazard"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_BIR13thMonthPayOtherBenefits"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_BIRDeMinimisBenefits"]              //55
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_BIRSSSGSISPHICHDMFUnionDues"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_BIROtherNonTaxableCompensation"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_BIRTotalTaxableCompensation"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_BIRTaxableCompensationNotSubjectToWithholdingTax"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_BIRTotalTaxesWithheld"]             //60
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_TaxDue"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_AssumeMonthlyRegularAmt"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_AssumeMonthlySSSAmt"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_AssumeMonthlyPhilhealthAmt"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_AssumeMonthlyPagibigAmt"]           //65
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_AssumeMonthlyUnionAmt"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_YTDMPFAmtBefore"]
                                                                        , dtPayCalcExt2Rec.Rows[0]["Tpy_YTDMPFAmt"]);
                                    dal.ExecuteNonQuery(strTempQuery);
                                    #endregion

                                    //Insert to Deduction Ledger Payslip Table
                                    InsertToDeductionLedgerPayslip(ProcessPayrollPeriod, EmployeeList[j], dal);
                                    #endregion
                                }
                            }
                            StatusHandler(this, new StatusEventArgs(string.Format("Compute Tax, Deductions and Net Pay"), true));
                        }
                    }
                    #endregion

                    dal.CommitTransactionSnapshot();
                    dal.BeginTransactionSnapshot();
                }

                #region Process Alphalist
                string LoginDBName = commonBL.GetDatabaseName(profileCode, centralProfile);
                StatusHandler(this, new StatusEventArgs("Process Alphalist Records", false));
                dalCentral = new DALHelper(CentralProfile, false);
                dalCentral.OpenDB();
                dalCentral.BeginTransaction();

                for (int j = 0; j < EmployeeList.Length; j++)
                {
                    #region Get YTD Year based on Separation Date
                    YTDYear = ProcessPayrollPeriod.Substring(0, 4);
                    drArrEmpDetails = dtEmployeeFinalPay.Select(string.Format("Mem_IDNo = '{0}'", EmployeeList[j]));
                    if (drArrEmpDetails.Length > 0)
                    {
                        strTaxMethod = GetValue(drArrEmpDetails[0]["Tef_TaxMethod"]);
                        strSepDate = drArrEmpDetails[0]["Mem_SeparationDate"].ToString();
                        try
                        {
                            YTDYear = commonBL.GetPayPeriodGivenDate(strSepDate.ToString());
                            if (YTDYear == "")
                                YTDYear = Convert.ToDateTime(strSepDate).Year.ToString();
                            else
                                YTDYear = YTDYear.Substring(0, 4);
                        }
                        catch
                        {
                            YTDYear = ProcessPayrollPeriod.Substring(0, 4);
                        }
                    }
                    #endregion

                    NewAlphalistProcessingBL.ProcessAlphalist(YTDYear, false, true, true, EmployeeList[j], "", LoginUser, CompanyCode, CentralProfile, ProcessPayrollPeriod, LoginDBName, strTaxMethod, menuCode, dalCentral);
                }
                dalCentral.CommitTransaction();
                dalCentral.CloseDB();
                StatusHandler(this, new StatusEventArgs("Process Alphalist Records", true));
                #endregion

            }
            catch (Exception ex)
            {
                //dal.RollBackTransaction();
                //dal.CloseDB();
                CommonProcedures.logErrorToFile(ex.ToString());
                throw new Exception("Final Pay Calculation has encountered some errors: \n" + ex.Message);
            }
        }

        public void CheckPreProcessingLastPayExists(string profileCode, string centralProfile, string PayCycle, string EmployeeID, string UserLogin, string companyCode, string menucode, DALHelper dalHelper)
        {
            #region Query
            string condition = string.Empty;
            string query = string.Format(@"
                            DECLARE @EndCycle DATETIME
                            DECLARE @CurPPCnt AS TINYINT
                                
                            SELECT @CurPPCnt = datediff(dd,Tps_StartCycle,Tps_EndCycle+1)
                                    , @EndCycle = Tps_EndCycle
                            FROM T_PaySchedule
                            WHERE Tps_PayCycle = '{3}'

                            DELETE FROM T_EmpProcessCheck WHERE Tpc_ModuleCode = '{0}' AND Tpc_SystemDefined = 1
                                 AND Tpc_PayCycle = '{3}'
                                 AND ('{2}' = '' OR Tpc_IDNo = '{2}')

                            INSERT INTO T_EmpProcessCheck

                            SELECT '{0}'
                            , Tef_IDNo 
                            , 'BW'
                            , '{3}'
                            , NULL
                            , NULL
                            , 'Employee separation pay cycle is equal to last calculated in normal pay cycle including current'
                            , 1
                            , '{4}'
                            , GETDATE()
                            FROM T_EmpFinalPay 
                            INNER JOIN M_Employee ON Tef_IDNo = Mem_IDNo
                            @USERCOSTCENTERACCESSCONDITION
                            INNER JOIN Udv_Payroll ON Tpy_IDNo = Tef_IDNo
                                AND Tpy_PayCycle = Tef_PayCycleSeparation 
                            WHERE Tef_PayCycle = '{3}'
                                AND ('{2}' = '' OR Tef_IDNo = '{2}')
                            "
                                , menucode
                                , companyCode
                                , EmployeeID
                                , PayCycle
                                , UserLogin);
            #endregion
            query = query.Replace("@USERCOSTCENTERACCESSCONDITION", (new HRCReportsBL()).UserCostCenterAccessTmpQuery(profileCode, "PAYROLL", UserLogin, "Mem_CostcenterCode", "Mem_PayrollGroup", "Mem_EmploymentStatusCode", "Mem_PayrollType", companyCode, centralProfile, false));
            dalHelper.ExecuteNonQuery(query);
        }

        public string GetEmployeeDeduction(string strEmployeeID, structEmployeeDeduction[] EmployeeDedArr, string strDefDeduction)
        {
            string strDeduction = "";
            for (int i = 0; i < EmployeeDedArr.Length; i++)
            {
                if (strEmployeeID == EmployeeDedArr[i].strEmployeeId)
                {
                    strDeduction = EmployeeDedArr[i].strDeductionCodes;
                    break;
                }
            }
            if (strDeduction == "")
                strDeduction = strDefDeduction;
            return strDeduction;
        }

        public string JoinEmployeesFromStringArray(string[] strArrData, bool bWithQuotes)
        {
            string strEmployeeListCommaDelimited = "";
            foreach (string strData in strArrData)
            {
                if (bWithQuotes == true)
                    strEmployeeListCommaDelimited += string.Format(@"'{0}',", strData);
                else
                    strEmployeeListCommaDelimited += string.Format(@"{0},", strData);
            }
            if (strEmployeeListCommaDelimited != "")
                strEmployeeListCommaDelimited = strEmployeeListCommaDelimited.Substring(0, strEmployeeListCommaDelimited.Length - 1);
            else
                strEmployeeListCommaDelimited = "''";

            return strEmployeeListCommaDelimited;
        }

        public string JoinEmployeesFromStringArray(string[] strArrData, string Separator, bool bWithQuotes)
        {
            string strEmployeeListCommaDelimited = "";
            foreach (string strData in strArrData)
            {
                if (bWithQuotes == true)
                    strEmployeeListCommaDelimited += string.Format(@"'{0}'{1}", strData, Separator);
                else
                    strEmployeeListCommaDelimited += string.Format(@"{0}{1}", strData, Separator);
            }
            if (strEmployeeListCommaDelimited != "")
                strEmployeeListCommaDelimited = strEmployeeListCommaDelimited.Substring(0, strEmployeeListCommaDelimited.Length - 1);

            return strEmployeeListCommaDelimited;
        }

        public string JoinEmployeesFromDataRowArray(DataRow[] drArrData, bool bWithQuotes)
        {
            string strListCommaDelimited = "";
            for (int i = 0; i < drArrData.Length; i++)
            {
                if (bWithQuotes == true)
                    strListCommaDelimited += string.Format(@"'{0}',", GetValue(drArrData[i][0]));
                else
                    strListCommaDelimited += string.Format(@"{0},", GetValue(drArrData[i][0]));
            }
            if (strListCommaDelimited != "")
                strListCommaDelimited = strListCommaDelimited.Substring(0, strListCommaDelimited.Length - 1);

            return strListCommaDelimited;
        }

        public string JoinEmployeesFromDataTableArray(DataTable dtData, bool bWithQuotes)
        {
            string strListCommaDelimited = "";
            for (int i = 0; i < dtData.Rows.Count; i++)
            {
                if (bWithQuotes == true)
                    strListCommaDelimited += string.Format(@"'{0}',", GetValue(dtData.Rows[i][0]));
                else
                    strListCommaDelimited += string.Format(@"{0},", GetValue(dtData.Rows[i][0]));
            }
            if (strListCommaDelimited != "")
                strListCommaDelimited = strListCommaDelimited.Substring(0, strListCommaDelimited.Length - 1);

            return strListCommaDelimited;
        }

        public string JoinEmployeesFromDataRowArrayHoldPayroll(DataTable dtData, bool bWithQuotes)
        {
            string strListCommaDelimited = "";
            for (int i = 0; i < dtData.Rows.Count; i++)
            {
                if (bWithQuotes == true)
                    strListCommaDelimited += string.Format(@"'{0}:{1}',", GetValue(dtData.Rows[i][0]), GetValue(dtData.Rows[i][2]));
                else
                    strListCommaDelimited += string.Format(@"{0}:{1},", GetValue(dtData.Rows[i][0]), GetValue(dtData.Rows[i][2]));
            }
            if (strListCommaDelimited != "")
                strListCommaDelimited = strListCommaDelimited.Substring(0, strListCommaDelimited.Length - 1);

            return strListCommaDelimited;
        }

        private decimal GetFormulaQueryDecimalValue(string query)
        {
            decimal dValue = 0;
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            if (dtResult.Rows.Count > 0)
            {
                dValue = Convert.ToDecimal(dtResult.Rows[0][0]);
            }
            return dValue;
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

        private string GetFormulaQueryStringValue(string query)
        {
            string strValue = string.Empty;
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            if (dtResult.Rows.Count > 0)
            {
                strValue = dtResult.Rows[0][0].ToString();
            }
            return strValue;
        }

        #region <Delegates>
        private void ShowLaborHrsGenEmployeeName(object sender, NewLaborHoursGenerationBL.EmpDispEventArgs e)
        {
        }

        private void ShowLaborHourGenStatus(object sender, NewLaborHoursGenerationBL.StatusEventArgs e)
        {
        }

        private void ShowPayrollCalcEmployeeName(object sender, NewPayrollCalculationBL.EmpDispEventArgs e)
        {
        }

        private void ShowPayrollStatus(object sender, NewPayrollCalculationBL.StatusEventArgs e)
        {
        }

        private void ShowW2EmployeeName(object sender, NewAlphalistProcessingBL.EmpDispEventArgs e)
        {
        }

        private void ShowW2Status(object sender, NewAlphalistProcessingBL.StatusEventArgs e)
        {
        }

        private void ShowLeaveRefundName(object sender, LeaveRefundBL.EmpDispEventArgs e)
        {
        }

        private void ShowLeaveRefundStatus(object sender, LeaveRefundBL.StatusEventArgs e)
        {
        }

        private void ShowBonusName(object sender, BonusComputation13thMonthBL.EmpDispEventArgs e)
        {
        }

        private void ShowBonusStatus(object sender, BonusComputation13thMonthBL.StatusEventArgs e)
        {
        }
        #endregion

        private DataTable GetAllEmployeeForProcess(string EmployeeList, string PayCycleCode)
        {
            string query = string.Format(@"SELECT Mem_IDNo 
	                                            , Mem_LastName 
	                                            , Mem_FirstName 
	                                            , Mem_MiddleName
	                                            , Mem_Age 
                                                , Mem_Tenure
	                                            , Mem_PayrollType
	                                            , Mem_PremiumGrpCode 
                                                , Mem_PayrollGroup
								                , Mem_CostcenterCode
								                , Mem_EmploymentStatusCode
								                , Mem_PositionCode
								                , Mem_PositionGrade
                                                , Mem_Salary
                                                , Mem_IntakeDate
                                                , Mem_SeparationDate
                                                , Mem_SeparationCodeExternal
                                                , Mem_BirthDate
                                                , Tef_PayCycleSeparation
                                                , Tef_HoldPayCycles
                                                , Tef_ZeroOutDeductionCodes
                                                , Tef_RetirementType
                                                , Tef_RegularPayRule
                                                , Tef_FixAllowanceRule
                                                , Tef_Compute13thMonthPay
                                                , Tef_ComputeLeaveRefund
                                                , Tef_ComputeFixAllowance
                                                , Tef_ComputeRetirement
                                                , Tef_ComputeHoldSalary
                                                , Tef_RetirementSeparateFromLastPay
                                                , Tef_OtherRetirementTaxableAmt
                                                , Tef_OtherRetirementNontaxableAmt
                                                , Tef_TaxMethod
                                                , Tef_PayERShare
                                                , Tps_TaxSchedule
                                        FROM M_Employee
                                        LEFT JOIN T_EmpFinalPay ON Mem_IDNo = Tef_IDNo
                                        LEFT JOIN T_PaySchedule ON Tps_PayCycle = Tef_PayCycleSeparation
                                        WHERE Mem_IDNo IN ({0})
                                            AND Tef_PayCycle = '{1}'
                                        ORDER BY Mem_LastName, Mem_FirstName", EmployeeList, PayCycleCode);

            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public string SetProcessFlags()
        {
            string strResult = string.Empty;
            string LastPayProcessingErrorMessage = "";

            #region Parameters
            StatusHandler(this, new StatusEventArgs("Loading Parameters", false));
            StatusHandler(this, new StatusEventArgs("Loading Parameters", true));

            strResult = commonBL.GetParameterValueFromPayroll("MDIVISOR", CompanyCode, dal);
            MDIVISOR = strResult.Equals(string.Empty) ? 0 : Convert.ToDecimal(strResult);
            StatusHandler(this, new StatusEventArgs(string.Format("  MDIVISOR = {0}", MDIVISOR), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  MDIVISOR = {0}", MDIVISOR), true));

            strResult = commonBL.GetParameterValueFromPayroll("MINNETPAY", CompanyCode, dal);
            MINNETPAY = strResult.Equals(string.Empty) ? 0 : Convert.ToDecimal(strResult);
            StatusHandler(this, new StatusEventArgs(string.Format("  MINNETPAY = {0}", MINNETPAY), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  MINNETPAY = {0}", MINNETPAY), true));

            strResult = commonBL.GetParameterValueFromPayroll("M13EXCLTAX", CompanyCode, dal);
            M13EXCLTAX = strResult.Equals(string.Empty) ? 0 : Convert.ToDecimal(strResult);
            StatusHandler(this, new StatusEventArgs(string.Format("  M13EXCLTAX = {0}", M13EXCLTAX), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  M13EXCLTAX = {0}", M13EXCLTAX), true));

            strResult = commonBL.GetParameterValueFromPayroll("HRLYRTEDEC", CompanyCode, dal);
            HRLYRTEDEC = strResult.Equals(string.Empty) ? 0 : Convert.ToDecimal(strResult);
            StatusHandler(this, new StatusEventArgs(string.Format("  HRLYRTEDEC = {0}", HRLYRTEDEC), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  HRLYRTEDEC = {0}", HRLYRTEDEC), true));

            LVHRENTRY = commonBL.GetParameterValueFromPayroll("LVHRENTRY", CompanyCode, dal);
            StatusHandler(this, new StatusEventArgs(string.Format("  LVHRENTRY = {0}", LVHRENTRY), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  LVHRENTRY = {0}", LVHRENTRY), true));

            #endregion

            #region Parameter List
            FP13THMONTHNORMAL = commonBL.GetParameterValueFromPayroll("FP13THMONTHNORMAL", CompanyCode, dal);
            if (FP13THMONTHNORMAL == "")
                LastPayProcessingErrorMessage += "13th Month Pay Bonus Code(s) Calculated on Normal Pay not set-up.";
            StatusHandler(this, new StatusEventArgs(string.Format("  FP13THMONTHNORMAL = {0}", FP13THMONTHNORMAL), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  FP13THMONTHNORMAL = {0}", FP13THMONTHNORMAL), true));

            FPLVREFUNDRULE = commonBL.GetParameterValueFromPayroll("FPLVREFUNDRULE", CompanyCode, dal);
            if (FPLVREFUNDRULE == "")
                LastPayProcessingErrorMessage += "Leave Refund Rule not set-up.";
            StatusHandler(this, new StatusEventArgs(string.Format("  FPLVREFUNDRULE = {0}", FPLVREFUNDRULE), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  FPLVREFUNDRULE = {0}", FPLVREFUNDRULE), true));

            FPLVREFUNDFORMULA = commonBL.GetParameterFormulaFromPayroll("FPLVREFUNDFORMULA", CompanyCode, dal);
            if (FPLVREFUNDFORMULA == "")
                LastPayProcessingErrorMessage += "Leave Refund Formula not set-up.";
            StatusHandler(this, new StatusEventArgs(string.Format("  FPLVREFUNDFORMULA = {0}", FPLVREFUNDFORMULA), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  FPLVREFUNDFORMULA = {0}", FPLVREFUNDFORMULA), true));

            FPRETIREMENTFORMULA = commonBL.GetParameterFormulaFromPayroll("FPRETIREMENTFORMULA", CompanyCode, dal);
            if (FPRETIREMENTFORMULA == "")
                LastPayProcessingErrorMessage += "Retirement Formula not set-up.";
            StatusHandler(this, new StatusEventArgs(string.Format("  FPRETIREMENTFORMULA = {0}", FPRETIREMENTFORMULA), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  FPRETIREMENTFORMULA = {0}", FPRETIREMENTFORMULA), true));

            FPRETIREMENTTENURE = commonBL.GetParameterFormulaFromPayroll("FPRETIREMENTTENURE", CompanyCode, dal);
            if (FPRETIREMENTTENURE == "")
                LastPayProcessingErrorMessage += "Tenure Formula not set-up.";
            StatusHandler(this, new StatusEventArgs(string.Format("  FPRETIREMENTTENURE = {0}", FPRETIREMENTTENURE), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  FPRETIREMENTTENURE = {0}", FPRETIREMENTTENURE), true));

            FPPROCESSYEARLVE = commonBL.GetParameterDtlValueFromPayroll("FPPROCESSYEAR", "LVREFUND", CompanyCode, dal);
            if (FPPROCESSYEARLVE == "")
                LastPayProcessingErrorMessage += "Leave Refund Year not set-up.";
            StatusHandler(this, new StatusEventArgs(string.Format("  FPPROCESSYEARLVE = {0}", FPPROCESSYEARLVE), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  FPPROCESSYEARLVE = {0}", FPPROCESSYEARLVE), true));

            FPPROCESSYEARBON = commonBL.GetParameterDtlValueFromPayroll("FPPROCESSYEAR", "13THMONTH", CompanyCode, dal);
            if (FPPROCESSYEARBON == "")
                LastPayProcessingErrorMessage += "13th month pay and other bonuses year not set-up.";
            StatusHandler(this, new StatusEventArgs(string.Format("  FPPROCESSYEARBON = {0}", FPPROCESSYEARBON), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  FPPROCESSYEARBON = {0}", FPPROCESSYEARBON), true));

            FPPROCESSYEARTAX = commonBL.GetParameterDtlValueFromPayroll("FPPROCESSYEAR", "TAX", CompanyCode, dal);
            if (FPPROCESSYEARTAX == "")
                LastPayProcessingErrorMessage += "Tax Year not set-up.";
            StatusHandler(this, new StatusEventArgs(string.Format("  FPPROCESSYEARTAX = {0}", FPPROCESSYEARTAX), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  FPPROCESSYEARTAX = {0}", FPPROCESSYEARTAX), true));

            FPFIXAMOUNT = commonBL.GetParameterValueFromPayroll("FPFIXAMOUNT", CompanyCode, dal);
            StatusHandler(this, new StatusEventArgs(string.Format("  FPFIXAMOUNT = {0}", FPFIXAMOUNT), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  FPFIXAMOUNT = {0}", FPFIXAMOUNT), true));

            FPFIXALLOWDIVIDEND = commonBL.GetParameterFormulaFromPayroll("FPFIXALLOWDIVIDEND", CompanyCode, dal);
            StatusHandler(this, new StatusEventArgs(string.Format("  FPFIXALLOWDIVIDEND = {0}", FPFIXAMOUNT), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  FPFIXALLOWDIVIDEND = {0}", FPFIXAMOUNT), true));

            FPFIXALLOWDIVISOR = commonBL.GetParameterFormulaFromPayroll("FPFIXALLOWDIVISOR", CompanyCode, dal);
            StatusHandler(this, new StatusEventArgs(string.Format("  FPFIXALLOWDIVISOR = {0}", FPFIXAMOUNT), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  FPFIXALLOWDIVISOR = {0}", FPFIXAMOUNT), true));

            FPDEDUCTIONGROUP = commonBL.GetParameterDtlListAllowedfromPayroll("FPDEDUCTIONGROUP", CompanyCode, dal);
            FPRETIREMENTCODE = commonBL.GetParameterDtlListfromPayroll("FPRETIREMENTCODE", CompanyCode, dal);

            if (FPRETIREMENTCODE.Rows.Count > 0)
            {
                for (int i = 0; i < FPRETIREMENTCODE.Rows.Count; i++)
                {
                    if (!bIncomeMasterCodExists(FPRETIREMENTCODE.Rows[i]["Mpd_SubCode"].ToString()))
                        LastPayProcessingErrorMessage += string.Format("{0} not set-up in Income Master."
                                                            , FPRETIREMENTCODE.Rows[i]["Mpd_SubCode"].ToString());
                }
            }

            dtBonusMaster = BonusComputation13thMonthBL.GetBonusMasterDetails(CompanyCode, CentralProfile, dal);
            #endregion

            MaxTaxAnnual = GetMaxPayPeriodAnnualTaxSchedule();

            return LastPayProcessingErrorMessage;
        }

        private void ClearLastPayTables(bool bProcessAll, string EmployeeList, string PayPeriod, string NormalPayPeriod)
        {
            #region Initialize Adjustment and Income Tables (Individual or All)
            dal.ExecuteNonQuery(string.Format(@"UPDATE T_EmpManualAdjFinalPay
                                                SET Tma_PostFlag = 0
                                                WHERE Tma_IDNo IN ({0})
	                                                AND Tma_PayCycle = '{1}'

                                                DELETE FROM T_EmpIncomeFinalPay
                                                FROM T_EmpIncomeFinalPay
                                                INNER JOIN {3}..M_Income ON Min_IncomeCode = Tin_IncomeCode
	                                                AND Min_CompanyCode = '{2}'
	                                                AND Min_IsSystemReserved = 1
                                                WHERE Tin_IDNo IN ({0})
	                                                AND Tin_Paycycle = '{1}'

                                                DELETE FROM T_EmpIncomeFinalPay
                                                FROM T_EmpIncomeFinalPay FP  
                                                INNER JOIN {3}..M_Income ON Min_IncomeCode = FP.Tin_IncomeCode
	                                                AND Min_CompanyCode = '{2}'
                                                    AND Min_IsSystemReserved = 0
                                                LEFT JOIN T_EmpIncome CUR ON CUR.Tin_IDNo = FP.Tin_IDNo 
	                                                AND  CUR.Tin_PayCycle = '{5}' --current normal pay cycle
	                                                AND  CUR.Tin_OrigPayCycle = FP.Tin_OrigPayCycle
	                                                AND  CUR.Tin_IncomeCode = FP.Tin_IncomeCode
	                                                AND  CUR.Tin_IDNo = FP.Tin_IDNo
                                                WHERE FP.Tin_PayCycle = '{1}'
	                                                AND FP.Tin_IDNo IN ({0})
                                                    AND FP.Tin_OrigPayCycle <> FP.Tin_PayCycle
	                                                AND CUR.Tin_IDNo IS NULL

                                                INSERT INTO T_EmpIncomeFinalPay
                                                SELECT CUR.Tin_IDNo
	                                                , '{1}'
	                                                , CUR.Tin_OrigPayCycle
	                                                , CUR.Tin_IncomeCode
	                                                , CUR.Tin_PostFlag
	                                                , CUR.Tin_IncomeAmt
	                                                , CUR.Usr_Login
	                                                , CUR.Ludatetime
                                                FROM T_EmpIncome CUR 
                                                INNER JOIN {3}..M_Income ON Min_IncomeCode = CUR.Tin_IncomeCode
	                                                AND Min_CompanyCode = '{2}'
	                                                AND Min_IsSystemReserved = 0
                                                LEFT JOIN T_EmpIncomeFinalPay FP ON FP.Tin_IDNo = CUR.Tin_IDNo 
	                                                AND FP.Tin_PayCycle = '{1}'
	                                                AND FP.Tin_OrigPayCycle = CUR.Tin_OrigPayCycle 
	                                                AND FP.Tin_IncomeCode =  CUR.Tin_IncomeCode
	                                                AND FP.Tin_IDNo = CUR.Tin_IDNo
                                                WHERE CUR.Tin_PayCycle = '{5}' --current normal pay cycle
	                                                AND CUR.Tin_IDNo IN ({0})
	                                                AND CUR.Tin_PostFlag = 0
	                                                AND FP.Tin_IDNo IS NULL
	                                                
                                                UPDATE T_EmpFinalPay SET Tef_LastPayPriorSeparation = ''
                                                    , Tef_PayCycleSeparation = ''
                                                    , Tef_ZeroOutDeductionCodes = ''
                                                    , Tef_13thMonthPayStart = ''
                                                    , Tef_13thMonthPayEnd = ''
                                                    , Tef_13thMonthPayFormula = ''
                                                    , Tef_LeaveTypeYear = ''
                                                    , Tef_LeaveRefundFormula = ''
                                                    , Tef_RetirementFormula = ''
                                                    , Tef_RetirementSalary = 0
                                                    , Tef_RetirementAmt = 0
                                                    , Tef_OtherRetirementTaxableAmt = 0
                                                    , Tef_OtherRetirementNontaxableAmt = 0
                                                    , Tef_RetirementRate = 0
                                                    , Tef_RetirementTax = 0
                                                    , Tef_RetirementTaxBracket = 0
                                                    , Tef_RetirementNetTax = 0
                                                    , Tef_TenureFormula = ''
                                                    , Tef_TenureYears = 0
                                                    , Tef_FixAllowanceNumVal1 = 0
                                                    , Tef_FixAllowanceNumVal2 = 0
                                                    , Tef_LastSalary = 0
                                                    , Tef_HoldSalary = 0
                                                    , Tef_13thMonthPay = 0
                                                    , Tef_LeaveRefund = 0
                                                WHERE Tef_IDNo IN ({0})
                                                AND Tef_PayCycle = '{1}'

                                                ---VARIABLE ALLOWANCES
                                                DELETE FROM T_EmpIncomeFinalPay
                                                WHERE Tin_PayCycle = '{1}'
                                                AND Tin_OrigPayCycle = '{1}'
	                                            AND Tin_IDNo IN ({0})
	                                            AND Tin_IncomeCode IN (
			                                            SELECT DISTINCT Mvh_AllowanceCode 
			                                            FROM {3}..M_VarianceAllowanceHdr 
                                                        WHERE Mvh_CompanyCode = '{2}'  
			                                            UNION
			                                            SELECT DISTINCT Mvh_AdjustmentCode 
			                                            FROM {3}..M_VarianceAllowanceHdr
                                                        WHERE Mvh_CompanyCode = '{2}'
				                                        )

                                                ---FIX ALLOWANCE
                                                DELETE A
                                                FROM T_EmpIncomeFinalPay A
                                                INNER JOIN {3}..M_Income ON Tin_IncomeCode = Min_IncomeCode
                                                    AND Min_CompanyCode = '{2}'
                                                WHERE Min_IsRecurring = 1
                                                    AND Tin_IDNo IN ({0})
                                                    AND Tin_PayCycle = '{1}'
                                                    AND Tin_OrigPayCycle = '{1}'

                                                ----LEAVE REFUND
                                                DELETE FROM T_EmpIncomeFinalPay
                                                WHERE Tin_IncomeCode IN (SELECT DISTINCT Tld_LeaveCode+[Name] AS [Code]
                                                                         FROM T_EmpLeaveRefundDtl
                                                                         LEFT JOIN (SELECT 'N' AS [Class],'NONTAXFP' AS [Name]
				                                                                    UNION
				                                                                    SELECT 'T' AS [Class],'TAXFP' AS [Name] ) taxclass ON 1=1) 
                                                        AND Tin_IDNo IN ({0})
                                                        AND Tin_PayCycle = '{1}'
                                                        AND Tin_OrigPayCycle = '{1}'

                                                ----RETIREMENT BENEFIT
                                                DELETE A
                                                FROM T_EmpIncomeFinalPay A
                                                INNER JOIN {3}..M_Income ON Tin_IncomeCode = Min_IncomeCode
                                                    AND Min_CompanyCode = '{2}'
                                                WHERE Tin_IDNo IN ({0})
                                                    AND Tin_PayCycle = '{1}'
                                                    AND Tin_OrigPayCycle = '{1}'
                                                    AND Tin_IncomeCode IN (SELECT Mpd_ParamValue 
                                                                           FROM M_PolicyDtl
                                                                            WHERE RTRIM(Mpd_PolicyCode) = 'FPRETIREMENTCODE' 
                                                                                AND RTRIM(Mpd_CompanyCode) = '{2}')
                                                    AND Min_IsSystemReserved = 1
 
                                               ----HOLD SALARY
                                                DELETE A
                                                    FROM T_EmpIncomeFinalPay A
                                                    INNER JOIN {3}..M_Income ON Tin_IncomeCode = Min_IncomeCode
                                                        AND Min_CompanyCode = '{2}'
                                                    WHERE Tin_IDNo IN ({0})
                                                        AND Tin_PayCycle = '{1}'
                                                        ---AND Tin_OrigPayCycle = '{1}'
                                                        AND Min_IncomeGroup = 'HSA'
                                                        AND Min_IsSystemReserved = 1

                                                ----13TH MONTH PAY AND OTHER BONUSES
                                                DELETE A
                                                    FROM T_EmpIncomeFinalPay A 
                                                    INNER JOIN {3}..M_Income ON Tin_IncomeCode = Min_IncomeCode
                                                        AND Min_CompanyCode = '{2}'
                                                    WHERE Tin_IDNo IN ({0})
                                                        AND Tin_PayCycle = '{1}'
                                                        AND Tin_OrigPayCycle = '{1}'
                                                        AND Min_IncomeGroup = 'BON'
                                                        AND Min_IsSystemReserved = 1


                                                ", EmployeeList, PayPeriod, CompanyCode, CentralProfile, MenuCode, NormalPayPeriod));
            #endregion

            string strQuery = "";
            if (bProcessAll == true)
            {
                #region Initialize Payroll Tables (All)
                strQuery = string.Format(@"     DELETE FROM T_EmpPayTranHdrFinalPay 
                                                WHERE ISNULL(Tph_RetainUserEntry, 0) = 0
                                                AND Tph_PayCycle = '{0}'

                                                DELETE FROM T_EmpPayTranDtlFinalPay 
                                                WHERE Tpd_PayCycle = '{0}'

                                                DELETE FROM T_EmpPayTranHdrMiscFinalPay 
                                                WHERE Tph_PayCycle = '{0}'

                                                DELETE FROM T_EmpPayTranDtlMiscFinalPay 
                                                WHERE Tpd_PayCycle = '{0}'

                                                DELETE FROM T_EmpTimeBaseAllowanceCycleFinalPay 

                                                DELETE FROM T_EmpPayrollFinalPay 
                                                WHERE Tpy_PayCycle = '{0}'

                                                DELETE FROM T_EmpPayroll2FinalPay 
                                                WHERE Tpy_PayCycle = '{0}'

                                                DELETE FROM T_EmpPayrollMiscFinalPay 
                                                WHERE Tpm_PayCycle = '{0}'

                                                DELETE FROM T_EmpDeductionDtlFinalPay 
                                                WHERE Tdd_ThisPayCycle = '{0}'
                                                ", PayPeriod);
                #endregion
            }
            else
            {
                #region Initialize Payroll Tables (Individual)
                strQuery = string.Format(@"     DELETE FROM T_EmpPayTranHdrFinalPay 
                                                WHERE ISNULL(Tph_RetainUserEntry, 0) = 0
                                                    AND Tph_PayCycle = '{0}'
												    AND Tph_IDNo IN ({1})

                                                DELETE FROM T_EmpPayTranDtlFinalPay 
                                                WHERE Tpd_PayCycle = '{0}'
												    AND Tpd_IDNo IN ({1})

                                                DELETE FROM T_EmpPayTranHdrMiscFinalPay 
                                                WHERE Tph_PayCycle = '{0}'
												    AND Tph_IDNo IN ({1})

                                                DELETE FROM T_EmpPayTranDtlMiscFinalPay 
                                                WHERE Tpd_PayCycle = '{0}'
												AND Tpd_IDNo IN ({1})

                                                DELETE FROM T_EmpTimeBaseAllowanceCycleFinalPay 
												WHERE Tta_IDNo IN ({1})

                                                DELETE FROM T_EmpPayrollFinalPay 
                                                WHERE Tpy_PayCycle = '{0}'
												    AND Tpy_IDNo IN ({1})

                                                DELETE FROM T_EmpPayroll2FinalPay 
                                                WHERE Tpy_PayCycle = '{0}'
                                                    AND Tpy_IDNo IN ({1})

                                                DELETE FROM T_EmpPayrollMiscFinalPay 
                                                WHERE Tpm_PayCycle = '{0}'
												    AND Tpm_IDNo IN ({1})

                                                DELETE FROM T_EmpDeductionDtlFinalPay 
                                                WHERE Tdd_ThisPayCycle = '{0}'
												    AND Tdd_IDNo IN ({1})
	                                            ", PayPeriod, EmployeeList);
                #endregion
            }
            dal.ExecuteNonQuery(strQuery);
        }

        private void InitializePayrollTables(string IDNumber, string CurrentPayPeriod, string UserLogin)
        {
            #region Payroll Transaction Sep and Detail
            string qString = string.Format(@"
INSERT INTO T_EmpPayTranHdrFinalPay
	  (Tph_IDNo
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
        ,Tph_PDOTHHOLHr
        ,Tph_PDPSDHr
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
        ,Ludatetime)
SELECT Mem_IDNo
      ,'{1}'
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,Mem_PayrollType
        ,Mem_PremiumGrpCode
        ,0
        ,'{2}'
        ,GETDATE()
FROM M_Employee 
WHERE Mem_IDNo = '{0}'

INSERT INTO T_EmpPayTranDtlFinalPay
	  (Tpd_IDNo
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
        ,Ludatetime)
SELECT Mem_IDNo
      ,Ttr_PayCycle
      ,Ttr_Date
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,Mem_PremiumGrpCode
        ,'{2}'
        ,GETDATE()
FROM T_EmpTimeRegisterHst
INNER JOIN M_Employee
	ON Ttr_IDNo = Mem_IDNo
WHERE Ttr_IDNo ='{0}'
	AND Ttr_PayCycle = '{1}'"
, IDNumber
, CurrentPayPeriod
, UserLogin
, CompanyCode
, CentralProfile
, PayrollStart
, PayrollEnd
, commonBL.GetParameterValueFromPayroll("HRRTINCDEM", CompanyCode)).Replace("@HOURSINDAY", CommonBL.HOURSINDAY.ToString());
            #endregion

            #region Payroll Transaction Sep Ext and Detail
            if (bHasDayCodeExt == true)
            {
                qString += string.Format(@" 
INSERT INTO T_EmpPayTranHdrMiscFinalPay
	(Tph_IDNo
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
        ,Ludatetime)
SELECT Mem_IDNo
        ,'{1}'
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,'{2}'
        ,GETDATE()
FROM M_Employee
WHERE Mem_IDNo ='{0}'

INSERT INTO T_EmpPayTranDtlMiscFinalPay
	(Tpd_IDNo
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
        ,Ludatetime)
SELECT Mem_IDNo
        ,Ttr_PayCycle
        ,Ttr_Date
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,0
        ,'{2}'
        ,GETDATE()
FROM T_EmpTimeRegisterHst
INNER JOIN M_Employee
	ON Ttr_IDNo = Mem_IDNo
WHERE Ttr_IDNo ='{0}'
	AND Ttr_PayCycle = '{1}'", IDNumber, CurrentPayPeriod, UserLogin);
            }
            #endregion

            #region Payroll Calc Sep
            qString += string.Format(@"
            INSERT INTO T_EmpPayrollFinalPay
            	(Tpy_IDNo
                ,Tpy_PayCycle
                ,Tpy_SalaryRate
                ,Tpy_HourRate
                ,Tpy_SpecialRate
                ,Tpy_SpecialHourRate
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
                ,Tpy_WtaxAmt
                ,Tpy_TaxBaseAmt
                ,Tpy_TaxRule
                ,Tpy_TaxShare
                ,Tpy_TaxBracket
                ,Tpy_TaxCode
                ,Tpy_IsTaxExempted
                ,Tpy_SSSEE
                ,Tpy_SSSER
                ,Tpy_ECFundAmt
                ,Tpy_SSSBaseAmt
                ,Tpy_SSSRule
                ,Tpy_SSSShare
                ,Tpy_MPFEE
                ,Tpy_MPFER
                ,Tpy_PhilhealthEE
                ,Tpy_PhilhealthER
                ,Tpy_PhilhealthBaseAmt
                ,Tpy_PhilhealthRule
                ,Tpy_PhilhealthShare
                ,Tpy_PagIbigEE
                ,Tpy_PagIbigER
                ,Tpy_PagIbigTaxEE
                ,Tpy_PagIbigBaseAmt
                ,Tpy_PagIbigRule
                ,Tpy_PagIbigShare
                ,Tpy_UnionAmt
                ,Tpy_UnionRule
                ,Tpy_UnionShare
                ,Tpy_UnionBaseAmt
                ,Tpy_OtherDeductionAmt
                ,Tpy_TotalDeductionAmt
                ,Tpy_NetAmt
                ,Tpy_CostcenterCode
                ,Tpy_PositionCode
                ,Tpy_EmploymentStatus
                ,Tpy_PayrollType
                ,Tpy_PaymentMode
                ,Tpy_BankCode
                ,Tpy_BankAcctNo
                ,Tpy_PayrollGroup
                ,Tpy_CalendarGrpCode
                ,Tpy_WorkLocationCode
                ,Tpy_AltAcctCode
                ,Tpy_ExpenseClass
                ,Tpy_PositionGrade
                ,Tpy_WorkStatus
                ,Tpy_PremiumGrpCode
                ,Tpy_IsComputedPerDay
                ,Tpy_RegRule
                ,Tpy_IsMultipleSalary
                ,Usr_Login
                ,Ludatetime)
            SELECT Mem_IDNo 
                  , '{1}'
                  ,Mem_Salary
                  ,CASE WHEN '{7}' = 'FALSE' 
            	    THEN CASE Mem_PayrollType 
            			WHEN 'D' THEN 
            					Mem_Salary / @HOURSINDAY
            			WHEN 'M' THEN 
            					(Mem_Salary * 12) / ISNULL((SELECT Mpd_ParamValue
                                                                FROM M_PolicyDtl
                                                                WHERE Mpd_PolicyCode = 'MDIVISOR'
                                                                    AND Mpd_SubCode = Mem_EmploymentStatusCode
                                                                    AND Mpd_CompanyCode = '{3}'
                                                                    AND Mpd_RecordStatus = 'A'), 0) / @HOURSINDAY
            			WHEN 'H' THEN 
            					Mem_Salary
            			ELSE 0 END   
            	    ELSE
            			{4}.dbo.GetHourlyRateWithDeMinimis(Mem_IDNo, '{3}', '{1}','{5}','{6}', (SELECT Mpd_ParamValue
                                                                                                         FROM M_PolicyDtl
                                                                                                         WHERE Mpd_PolicyCode = 'MDIVISOR'
                                                                                                              AND Mpd_SubCode = Mem_EmploymentStatusCode
                                                                                                                   AND Mpd_CompanyCode = '{3}'
                                                                                                                   AND Mpd_RecordStatus = 'A'))
            	    END [Tpd_HourRate]
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                    ,0
                   ,Mem_CostcenterCode
                   ,Mem_PositionCode
                   ,Mem_EmploymentStatusCode
                   ,Mem_PayrollType
                   ,Mem_PaymentMode
                   ,Mem_PayrollBankCode
                   ,Mem_BankAcctNo
                   ,Mem_PayrollGroup
                   ,Mem_CalendarGrpCode
                   ,Mem_WorkLocationCode
                   ,Mem_AltAcctCode
                   ,Mem_ExpenseClass
                   ,Mem_PositionGrade
                   ,Mem_WorkStatus
                   ,Mem_PremiumGrpCode
                   ,Mem_IsComputedPerDay
                   ,Mem_IsMultipleSalary
                    , ''
                   ,'{2}'
                   ,GETDATE()
            FROM M_Employee
            	WHERE Mem_IDNo ='{0}' "
    , IDNumber
    , CurrentPayPeriod
    , UserLogin
    , CompanyCode
    , CentralProfile
    , PayrollStart
    , PayrollEnd
    , commonBL.GetParameterValueFromPayroll("HRRTINCDEM", CompanyCode)).Replace("@HOURSINDAY", CommonBL.HOURSINDAY.ToString());

            #endregion

            #region Payroll Calc2 Sep
            qString += string.Format(@"
            INSERT INTO T_EmpPayroll2FinalPay
            	(Tpy_IDNo
                , Tpy_PayCycle
                , Tpy_RemainingPayCycle
                , Tpy_YTDRegularAmtBefore
                , Tpy_AssumeRegularAmt
                , Tpy_RegularTotal
                , Tpy_YTDRecurringAllowanceAmtBefore
                , Tpy_YTDRecurringAllowanceAmt
                , Tpy_RecurringAllowanceAmt
                , Tpy_AssumeRecurringAllowanceAmt
                , Tpy_RecurringAllowanceTotal
                , Tpy_YTDBonusAmtBefore
                , Tpy_BonusNontaxAmt
                , Tpy_BonusTaxAmt
                , Tpy_Assume13thMonthAmt
                , Tpy_BonusTotal
                , Tpy_BonusTaxRevaluated
                , Tpy_YTDBonusTaxBefore
                , Tpy_YTDOtherTaxableIncomeAmtBefore
                , Tpy_OtherTaxableIncomeAmt
                , Tpy_OtherTaxableIncomeTotal
                , Tpy_YTDSSSAmtBefore
                , Tpy_AssumeSSSAmt
                , Tpy_SSSTotal
                , Tpy_YTDPhilhealthAmtBefore
                , Tpy_AssumePhilhealthAmt
                , Tpy_PhilhealthTotal
                , Tpy_YTDPagIbigAmtBefore
                , Tpy_AssumePagIbigAmt
                , Tpy_PagIbigTotal
                , Tpy_YTDPagIbigTaxAmtBefore
                , Tpy_AssumePagIbigTaxAmt
                , Tpy_TotalTaxableIncomeWAssumeAmt
                , Tpy_YTDUnionAmtBefore
                , Tpy_AssumeUnionAmt
                , Tpy_UnionTotal
                , Tpy_PremiumPaidOnHealth
                , Tpy_TotalExemptions
                , Tpy_NetTaxableIncomeAmt
                , Tpy_YTDREGAmt
                , Tpy_YTDTotalTaxableIncomeAmtBefore
                , Tpy_YTDTotalTaxableIncomeAmt
                , Tpy_YTDWtaxAmtBefore
                , Tpy_YTDWtaxAmt
                , Tpy_YTDSSSAmt
                , Tpy_YTDPhilhealthAmt
                , Tpy_YTDPagIbigAmt
                , Tpy_YTDPagIbigTaxAmt
                , Tpy_YTDUnionAmt
                , Tpy_YTDNontaxableAmtBefore
                , Tpy_YTDNontaxableAmt
                , Tpy_BIRTotalAmountofCompensation
                , Tpy_BIRStatutoryMinimumWage
                , Tpy_BIRHolidayOvertimeNightShiftHazard
                , Tpy_BIR13thMonthPayOtherBenefits
                , Tpy_BIRDeMinimisBenefits
                , Tpy_BIRSSSGSISPHICHDMFUnionDues
                , Tpy_BIROtherNonTaxableCompensation
                , Tpy_BIRTotalTaxableCompensation
                , Tpy_BIRTaxableCompensationNotSubjectToWithholdingTax
                , Tpy_BIRTotalTaxesWithheld
                , Tpy_TaxDue
                , Tpy_AssumeMonthlyRegularAmt
                , Tpy_AssumeMonthlySSSAmt
                , Tpy_AssumeMonthlyPhilhealthAmt
                , Tpy_AssumeMonthlyPagibigAmt
                , Tpy_AssumeMonthlyUnionAmt
                , Tpy_YTDMPFAmtBefore
                , Tpy_YTDMPFAmt
                , Usr_Login
                , Ludatetime)
            SELECT Mem_IDNo 
                , '{1}'
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,0
                ,'{2}'
                ,GETDATE()

            FROM M_Employee
            	WHERE Mem_IDNo ='{0}' "
    , IDNumber
    , CurrentPayPeriod
    , UserLogin);

            #endregion

            #region Payroll Calc Sep Ext

            if (bHasDayCodeExt == true)
            {
                qString += string.Format(@"
                INSERT INTO T_EmpPayrollMiscFinalPay
                           (Tpm_IDNo
                            ,Tpm_PayCycle
                            ,Tpm_Misc1Hr
                            ,Tpm_Misc1Amt
                            ,Tpm_Misc1OTHr
                            ,Tpm_Misc1OTAmt
                            ,Tpm_Misc1NDHr
                            ,Tpm_Misc1NDAmt
                            ,Tpm_Misc1NDOTHr
                            ,Tpm_Misc1NDOTAmt
                            ,Tpm_Misc2Hr
                            ,Tpm_Misc2Amt
                            ,Tpm_Misc2OTHr
                            ,Tpm_Misc2OTAmt
                            ,Tpm_Misc2NDHr
                            ,Tpm_Misc2NDAmt
                            ,Tpm_Misc2NDOTHr
                            ,Tpm_Misc2NDOTAmt
                            ,Tpm_Misc3Hr
                            ,Tpm_Misc3Amt
                            ,Tpm_Misc3OTHr
                            ,Tpm_Misc3OTAmt
                            ,Tpm_Misc3NDHr
                            ,Tpm_Misc3NDAmt
                            ,Tpm_Misc3NDOTHr
                            ,Tpm_Misc3NDOTAmt
                            ,Tpm_Misc4Hr
                            ,Tpm_Misc4Amt
                            ,Tpm_Misc4OTHr
                            ,Tpm_Misc4OTAmt
                            ,Tpm_Misc4NDHr
                            ,Tpm_Misc4NDAmt
                            ,Tpm_Misc4NDOTHr
                            ,Tpm_Misc4NDOTAmt
                            ,Tpm_Misc5Hr
                            ,Tpm_Misc5Amt
                            ,Tpm_Misc5OTHr
                            ,Tpm_Misc5OTAmt
                            ,Tpm_Misc5NDHr
                            ,Tpm_Misc5NDAmt
                            ,Tpm_Misc5NDOTHr
                            ,Tpm_Misc5NDOTAmt
                            ,Tpm_Misc6Hr
                            ,Tpm_Misc6Amt
                            ,Tpm_Misc6OTHr
                            ,Tpm_Misc6OTAmt
                            ,Tpm_Misc6NDHr
                            ,Tpm_Misc6NDAmt
                            ,Tpm_Misc6NDOTHr
                            ,Tpm_Misc6NDOTAmt
                            ,Usr_Login
                            ,Ludatetime)
                SELECT Mem_IDNo
                            ,'{1}'
                            ,0
                            ,0
                            ,0
                            ,0
                            ,0
                            ,0
                            ,0
                            ,0
                            ,0
                            ,0
                            ,0
                            ,0
                            ,0
                            ,0
                            ,0
                            ,0
                            ,0
                            ,0
                            ,0
                            ,0
                            ,0
                            ,0
                            ,0
                            ,0
                            ,0
                            ,0
                            ,0
                            ,0
                            ,0
                            ,0
                            ,0
                            ,0
                            ,0
                            ,0
                            ,0
                            ,0
                            ,0
                            ,0
                            ,0
                            ,0
                            ,0
                            ,0
                            ,0
                            ,0
                            ,0
                            ,0
                            ,0
                            ,0
                           ,'{2}'
                           ,GETDATE()
                FROM M_Employee
                WHERE Mem_IDNo ='{0}'", IDNumber, CurrentPayPeriod, UserLogin);
            }

            #endregion

            dal.ExecuteNonQuery(qString);
        }

        private bool CheckIfBonusAlreadyExists(string IDNumber, string PayCycle)
        {
            string query = string.Format(@"SELECT A.* FROM Udv_Bonus A
                                            INNER JOIN Udv_FinalPay ON Tef_IDNo = Tbd_IDNo
                                            AND Tef_PayCycle  = Tbh_PayCycle
                                            WHERE Tbd_IDNo = '{0}'
                                                AND Tbh_PayCycle = '{1}'
                                                AND Tbh_EndCoverage >= Tef_13thMonthPayStart 
                                                AND Tef_13thMonthPayEnd >= Tbh_StartCoverage
                                                AND Tps_CycleType <> 'L'"
                                                , IDNumber
                                                , PayCycle);
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            if (dtResult.Rows.Count > 0)
                return true;
            else
                return false;
        }

        private bool CheckIfLEaveRefundAlreadyExists(string IDNumber, string PayCycle)
        {
            string query = string.Format(@"SELECT A.* FROM Udv_LeaveRefund A
                                            INNER JOIN Udv_FinalPay ON Tef_IDNo = Tld_IDNo
                                            AND Tef_PayCycle = Tlh_PayCycle 
                                            CROSS APPLY [{0}].dbo.Udf_Split(Tef_LeaveTypeYear, ',') cs
                                            WHERE Tld_IDNo =  @Tld_IDNo
                                                AND Tlh_PayCycle = @Tlh_PayCycle
                                                AND LEFT(Data, 2) = Tld_LeaveCode
                                                AND SUBSTRING(Data,3,4) = Tlr_LeaveYear
                                                AND Tps_CycleType <> 'L'", CentralProfile);

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Tld_IDNo", IDNumber);
            paramInfo[1] = new ParameterInfo("@Tlh_PayCycle", PayCycle);

            DataTable dtResult = dal.ExecuteDataSet(query, CommandType.Text, paramInfo).Tables[0];
            if (dtResult.Rows.Count > 0)
                return true;
            else
                return false;
        }

        public DataTable GetHoldPayPeriodPerEmployee(string IDNumber, string PayCycleCode, DALHelper dalHelper)
        {
            string query = string.Format(@"SELECT Tpy_PayCycle, Tpy_IDNo, Tpy_NetAmt  
                                                FROM
		                                        (SELECT Tpy_IDNo, Tpy_PayCycle, Tpy_NetAmt , Tpy_PaymentMode
		                                        FROM T_EmpPayrollYearly
		                                        UNION ALL
		                                        SELECT Tpy_IDNo, Tpy_PayCycle, Tpy_NetAmt,Tpy_PaymentMode
		                                        FROM T_EmpPayrollHst) HoldSalary
                                                INNER JOIN T_PaySchedule 
                                                ON Tps_PayCycle = Tpy_PayCycle
                                                WHERE Tpy_PaymentMode = 'H'
	                                                AND Tpy_IDNo = '{0}'
	                                                AND Tpy_PayCycle <> '{1}'", IDNumber, PayCycleCode);
            DataTable dtResult = dalHelper.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        private DataTable GetValidPayPeriodsToProcessPerEmployee(string EmployeeList)
        {
            string query = string.Format(@"
            SELECT Mem_IDNo
	            , ( SELECT MAX(MaxPayPeriod) FROM
		            ( SELECT Tpy_PayCycle AS MaxPayPeriod 
			            FROM  Udv_Payroll
			            WHERE Tpy_IDNo = Mem_IDNo
                            AND Tps_CycleType = 'N'
                            AND Tps_CycleIndicator <> 'C') Temp) AS [StartPayPeriod]
	            , (SELECT TOP 1 Tps_PayCycle 
		            FROM T_PaySchedule 
		            WHERE DATEADD(dd, -1,Mem_SeparationDate) BETWEEN Tps_StartCycle AND Tps_EndCycle 
			            AND Tps_CycleIndicator != 'S' 
			            AND Tps_RecordStatus = 'A'
		            ORDER BY Tps_StartCycle DESC) AS [EndPayPeriod]
                , ( SELECT MAX(MaxPayPeriod) FROM
		            ( SELECT Ttr_PayCycle AS MaxPayPeriod 
			            FROM  Udv_TimeRegister
			            WHERE Ttr_IDNo = Mem_IDNo
                        AND Ttr_PayCycle <= (SELECT TOP 1 Tps_PayCycle 
		                                    FROM T_PaySchedule 
		                                    WHERE DATEADD(dd, -1,Mem_SeparationDate) BETWEEN Tps_StartCycle AND Tps_EndCycle 
			                                    AND Tps_CycleIndicator != 'S' 
			                                    AND Tps_RecordStatus = 'A'
		                                    ORDER BY Tps_StartCycle DESC)
                    ) Temp) AS [MaxTimeRegisterPayPeriod]
            FROM M_Employee
            WHERE Mem_IDNo IN ({0})", EmployeeList);
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public DataTable GetSeparationPayCycle(string EmployeeList, bool bFutureOnly, DALHelper dalHelper)
        {
            string condition = "AND Tps_CycleIndicator <> 'F'";
            if (bFutureOnly)
                condition = "AND Tps_CycleIndicator = 'F'";

            string query = string.Format(@"
            SELECT IDNo, SeparationPayCycle, Tps_CycleIndicator 
            FROM
            (SELECT Mem_IDNo as IDNo
					            ,(SELECT TOP 1 Tps_PayCycle	FROM T_PaySchedule 
		                        WHERE Mem_SeparationDate BETWEEN Tps_StartCycle AND Tps_EndCycle 
					            AND Tps_CycleIndicator != 'S' 
					            AND Tps_RecordStatus = 'A'
					            ORDER BY Tps_StartCycle DESC) AS SeparationPayCycle
            FROM M_Employee 
            ) tmp 
            LEFT JOIN T_PaySchedule
            ON Tps_PayCycle = SeparationPayCycle
            WHERE IDNo IN ({0})
            {1}"
            , EmployeeList
            , condition);

            DataTable dtResult = dalHelper.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        private string BonusControlNumberExists(string PayCyle, string BonusCode, DALHelper dal)
        {
            string query = string.Format(@"SELECT Tbh_ControlNo FROM T_EmpBonusHdr 
                                           WHERE Tbh_PayCycle = '{0}'
                                               AND Tbh_BonusCode = '{1}'", PayCyle, BonusCode);
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            string ControlNumber = "";
            if (dtResult.Rows.Count > 0)
            {
                ControlNumber = dtResult.Rows[0][0].ToString();
            }
            return ControlNumber;
        }

        private string LeaveRefundControlNumberExists(string PayCyle, DALHelper dal)
        {
            string query = string.Format(@"SELECT Tlh_ControlNo FROM T_EmpLeaveRefundHdr 
                                           WHERE Tlh_PayCycle = '{0}'", PayCyle);
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            string ControlNumber = "";
            if (dtResult.Rows.Count > 0)
            {
                ControlNumber = dtResult.Rows[0][0].ToString();
            }
            return ControlNumber;
        }

        private bool bIncomeMasterCodExists(string IncomeCode)
        {
            string query = string.Format(@"SELECT * FROM {2}..M_Income 
                                           WHERE Min_CompanyCode = '{1}'
                                                AND Min_IncomeCode = '{0}'"
                                                , IncomeCode
                                                , CompanyCode
                                                , CentralProfile);
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            if (dtResult.Rows.Count > 0)
                return true;
            else
                return false;
        }

        public string GetMaxPayPeriodAnnualTaxSchedule()
        {
            #region query
            string query = string.Format(@"SELECT MAX(Myt_PayCycle) as [PayCycle]
                                           FROM {0}..M_YearlyTaxSchedule
                                           WHERE Myt_PayCycle <= '{1}'
                                                AND Myt_CompanyCode = '{2}'
                                                AND Myt_RecordStatus = 'A'"
                                        , CentralProfile, ProcessPayrollPeriod, CompanyCode);
            #endregion
            DataTable dtResult;
            string PayPeriod = "";
            dtResult = dal.ExecuteDataSet(query).Tables[0];
            if (dtResult.Rows.Count > 0)
                PayPeriod = dtResult.Rows[0][0].ToString();
            return PayPeriod;
        }

        private string EncodeFilterItems(string strDelimited)
        {
            string[] strArrFilterItems = strDelimited.Split(new char[] { ',' });
            string strFilterItem = "";
            foreach (string col in strArrFilterItems)
            {
                strFilterItem += "'" + col.Trim() + "',";
            }
            if (strFilterItem != "")
                strFilterItem = strFilterItem.Substring(0, strFilterItem.Length - 1);
            return strFilterItem;
        }

        public void InsertToDeductionLedgerPayslip(string PayPeriod, string EmployeeId, DALHelper dalHelper)
        {
            string EmployeeDeductionLedgerPayslipTable = "T_EmpDeductionHdrCycleFinalPay";
            string EmpDeductionDtlTable = "T_EmpDeductionDtlFinalPay";

            string EmployeeCondition1 = "";
            string EmployeeCondition2 = "";
            if (EmployeeId != "")
            {
                EmployeeCondition1 += " AND Tdh_IDNo = '" + EmployeeId + "' ";
                EmployeeCondition2 += " AND Tdh_IDNo = '" + EmployeeId + "' ";
            }

            try
            {
                #region query
                string sqlquery = string.Format(@"DELETE FROM {3} WHERE Tdh_PayCycle = '{0}' {1}
                                                    INSERT INTO {3}
                                                       (Tdh_PayCycle
                                                       ,Tdh_IDNo
                                                       ,Tdh_DeductionCode
                                                       ,Tdh_StartDate
                                                       ,Tdh_DeductionAmount
                                                       ,Tdh_AccumulatedPaidAmount
                                                       ,Tdh_CycleAmount
                                                       ,Tdh_PaidAmountThisCycle
                                                       ,Usr_Login
                                                       ,Ludatetime)
                                                  SELECT '{0}' AS Tdh_PayCycle
                                                        , Tdh_IDNo
                                                        , Tdh_DeductionCode
                                                        , Tdh_StartDate
                                                        , Tdh_DeductionAmount
                                                        , Tdh_PaidAmount
                                                        , Tdh_CycleAmount
                                                        , ISNULL((SELECT SUM(Tdd_Amount) 
			                                                FROM {4}
			                                                WHERE  Tdd_PaymentFlag = 1 
				                                                AND Tdd_IDNo = Tdh_IDNo 
				                                                AND Tdd_DeductionCode = Tdh_DeductionCode 
				                                                AND Tdd_StartDate = Tdh_StartDate
                                                                AND Tdd_ThisPayCycle = '{0}'
			                                                GROUP BY Tdd_IDNo
					                                                , Tdd_DeductionCode
					                                                , Tdd_StartDate), 0) AS Tdh_PaidAmountThisCycle
                                                        , Usr_Login
                                                        , Ludatetime 
                                                FROM {5}..T_EmpDeductionHdr 
                                                WHERE Tdh_DeductionAmount <> Tdh_PaidAmount {2}"
                                                , PayPeriod
                                                , EmployeeCondition1
                                                , EmployeeCondition2
                                                , EmployeeDeductionLedgerPayslipTable
                                                , EmpDeductionDtlTable
                                                , CentralProfile);
                #endregion
                dalHelper.ExecuteNonQuery(sqlquery);
            }
            catch
            {
                throw new PayrollException("Error in inserting data to payslip.");
            }
        }

        public void UpdateEmployeeDeductionDetail(DataRow drDeduction, DALHelper dalHelper)
        {
            #region parameters
            int paramIndexUpdPayFlg = 0;
            ParameterInfo[] paramUpdPayFlg = new ParameterInfo[6];
            paramUpdPayFlg[paramIndexUpdPayFlg++] = new ParameterInfo("@Tdd_IDNo", drDeduction["Tdd_IDNo"]);
            paramUpdPayFlg[paramIndexUpdPayFlg++] = new ParameterInfo("@Tdd_DeductionCode", drDeduction["Tdd_DeductionCode"]);
            paramUpdPayFlg[paramIndexUpdPayFlg++] = new ParameterInfo("@Tdd_StartDate", drDeduction["Tdd_StartDate"]);
            paramUpdPayFlg[paramIndexUpdPayFlg++] = new ParameterInfo("@Tdd_ThisPayCycle", drDeduction["Tdd_ThisPayCycle"]);
            paramUpdPayFlg[paramIndexUpdPayFlg++] = new ParameterInfo("@Tdd_PayCycle", drDeduction["Tdd_PayCycle"]);
            paramUpdPayFlg[paramIndexUpdPayFlg++] = new ParameterInfo("@Tdd_LineNo", drDeduction["Tdd_LineNo"]);
            #endregion

            #region query
            string query = string.Format(@"UPDATE T_EmpDeductionDtlFinalPay
                                          SET Tdd_PaymentFlag = 1
                                          WHERE Tdd_IDNo = @Tdd_IDNo 
                                              AND Tdd_DeductionCode = @Tdd_DeductionCode 
                                              AND Tdd_StartDate = @Tdd_StartDate
                                              AND Tdd_ThisPayCycle = @Tdd_ThisPayCycle
                                              AND Tdd_PayCycle = @Tdd_PayCycle
                                              AND Tdd_LineNo = @Tdd_LineNo");
            #endregion
            dalHelper.ExecuteNonQuery(query, CommandType.Text, paramUpdPayFlg);

        }


        public DataSet GetDeductionDetails(string EmployeeId, DALHelper dalHelper)
        {
            #region query
            string query = string.Format(@"SELECT M_Deduction.*, T_EmpDeductionDtlFinalPay.* 
                                          FROM T_EmpDeductionDtlFinalPay
                                          INNER JOIN {2}..M_Deduction ON Mdn_DeductionCode = Tdd_DeductionCode
                                                AND Mdn_CompanyCode = '{1}'
                                          WHERE Tdd_IDNo = '{0}'
                                          ORDER BY Mdn_PriorityNo
                                               ,Tdd_DeductionCode
                                               ,Tdd_StartDate
                                               ,Tdd_PayCycle "
                                        , EmployeeId
                                        , CompanyCode
                                        , CentralProfile);
            #endregion
            DataSet ds = null;
            ds = dalHelper.ExecuteDataSet(query);
            return ds;
        }

        public string GetEmployeeIncludedDeductions(string IDNumber, string strDeductionGroup)
        {
            if (strDeductionGroup != "")
            {
                DataTable dtResult;
                #region Employee Master and Deduction Ledger query
                string query = @"SELECT DISTINCT Tdh_DeductionCode 
                                FROM {2}..T_EmpDeductionHdr 
											INNER JOIN {2}..M_Deduction
											ON  Tdh_DeductionCode = Mdn_DeductionCode
												AND Mdn_CompanyCode = '{3}'
                                WHERE Tdh_IDNo ='{0}'
									AND Mdn_DeductionGroup IN ({1})";
                #endregion
                query = string.Format(query, IDNumber, strDeductionGroup, CentralProfile, CompanyCode);
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                if (dtResult.Rows.Count > 0)
                    return JoinEmployeesFromDataTableArray(dtResult, false);
                else
                    return "";
            }
            return "";
        }

        public DataTable GetPayrollDetails(bool bIndividual, string IDNumber, string PayCycleCode, string companyCode, string centralProfile, DALHelper dalhelper)
        {
            #region Get Payroll Records
            PayrollRegisterBL PayrollRegisterBL = new PayrollRegisterBL();
            commonBL = new CommonBL();
            string mainQuery    = string.Empty;
            string condition    = string.Empty;
            string IDNo         = string.Empty;
            string PayPeriod    = string.Empty;
            string PayrollType  = string.Empty;
            string PremiumGrp   = string.Empty;
            string RegRule      = string.Empty;

            decimal Hr = 0;
            decimal OTHr = 0;
            decimal NDHr = 0;
            decimal NDOTHr = 0;
            decimal OTAmt = 0;

            if (bIndividual)
                condition = string.Format(@"AND Tpy_IDNo = '{0}'", IDNumber);
            else
                condition = string.Format(@"AND Tpy_IDNo IN ({0})", IDNumber);
            #region Main Query
            if (mainQuery != string.Empty)
                mainQuery += @"
                        UNION ALL 
                             ";

            mainQuery += string.Format(@"SELECT * 
                                        FROM Udv_PayrollFinalPay
                                        WHERE Tpy_PayCycle = '{0}'
                                        {1}    
                                        ", PayCycleCode
                                         , condition);

            #endregion
            //Condition = Condition.Replace("START", "");
            //mainQuery = mainQuery.Replace("@COND", Condition);
            DataTable dtPayrollDetails = dalhelper.ExecuteDataSet(mainQuery).Tables[0];

            #endregion

            DataTable dtDayCode = new DataTable();
            dtDayCode.Columns.Add("Day Column");
            dtDayCode.Columns.Add("IDNo");
            dtDayCode.Columns.Add("PayCycle");
            dtDayCode.Columns.Add("Seq", typeof(int));
            dtDayCode.Columns.Add("Hr", typeof(decimal));
            dtDayCode.Columns.Add("Amt", typeof(decimal));
            dtDayCode.Columns.Add("Orig Pay Cycle");

            if (dtPayrollDetails.Rows.Count > 0)
            {
                for (int idx = 0; idx < dtPayrollDetails.Rows.Count; idx++)
                {
                    IDNo        = string.Empty;
                    PayPeriod   = string.Empty;
                    PayrollType = string.Empty;
                    PremiumGrp  = string.Empty;
                    RegRule     = string.Empty;
                    Hr = 0;
                    OTHr = 0;
                    NDHr = 0;
                    NDOTHr = 0;
                    OTAmt = 0;

                    IDNo        = dtPayrollDetails.Rows[idx]["Tpy_IDNo"].ToString().Trim();
                    PayPeriod   = dtPayrollDetails.Rows[idx]["Tpy_PayCycle"].ToString().Trim();
                    PayrollType = dtPayrollDetails.Rows[idx]["Tpy_PayrollType"].ToString().Trim();
                    PremiumGrp  = dtPayrollDetails.Rows[idx]["Tpy_PremiumGrpCode"].ToString().Trim();
                    RegRule     = dtPayrollDetails.Rows[idx]["Tpy_RegRule"].ToString().Trim();

                    DataRow dr = null;

                    //if (RegRule == "A") //Hours Absent
                    //{
                    //    #region 3. >>> 
                    //    DataTable dtDayCodeABS = PayrollRegisterBL.GetDayCodeREGABS("A", companyCode, centralProfile);
                    //    for (int x = 0; x < dtDayCodeABS.Rows.Count; x++)
                    //    {
                    //        if (dtDayCodeABS.Rows[x]["DayCode"].ToString().Trim() != "ABS"
                    //            && (Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpy_" + dtDayCodeABS.Rows[x]["DayCode"].ToString().Trim() + "Hr"]) != 0
                    //            || Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpy_" + dtDayCodeABS.Rows[x]["DayCode"].ToString().Trim() + "Amt"]) != 0))
                    //        {
                    //            dr = dtDayCode.NewRow();
                    //            dr["Day Column"]    = dtDayCodeABS.Rows[x]["DayCode"].ToString().Trim();
                    //            dr["IDNo"]          = IDNo;
                    //            dr["PayCycle"]      = PayPeriod;
                    //            dr["Seq"]           = Convert.ToInt32(dtDayCodeABS.Rows[x]["Seq"].ToString().Trim()) + 2;
                    //            dr["Hr"]            = Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpy_" + dtDayCodeABS.Rows[x]["DayCode"].ToString().Trim() + "Hr"]) * -1;
                    //            dr["Amt"]           = Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpy_" + dtDayCodeABS.Rows[x]["DayCode"].ToString().Trim() + "Amt"]) * -1;
                    //            dtDayCode.Rows.Add(dr);
                    //        }
                    //    }
                    //    #endregion
                    //}

                    //POSITIVE
                    DataTable dtDayCodeREG = PayrollRegisterBL.GetDayCodeREGABS("R", companyCode, centralProfile);
                    for (int x = 0; x < dtDayCodeREG.Rows.Count; x++)
                    {
                        #region 20 >>>
                        if (Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpy_" + dtDayCodeREG.Rows[x]["DayCode"].ToString().Trim() + "Hr"]) != 0
                        || Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpy_" + dtDayCodeREG.Rows[x]["DayCode"].ToString().Trim() + "Amt"]) != 0)
                        {
                            dr = dtDayCode.NewRow();
                            dr["Day Column"]    = dtDayCodeREG.Rows[x]["DayCode"].ToString().Trim();
                            dr["IDNo"]          = IDNo;
                            dr["PayCycle"]      = PayPeriod;
                            dr["Seq"]           = Convert.ToInt32(dtDayCodeREG.Rows[x]["Seq"].ToString().Trim()) + 20;
                            dr["Hr"]            = Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpy_" + dtDayCodeREG.Rows[x]["DayCode"].ToString().Trim() + "Hr"]);
                            dr["Amt"]           = Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpy_" + dtDayCodeREG.Rows[x]["DayCode"].ToString().Trim() + "Amt"]);
                            dtDayCode.Rows.Add(dr);
                        }
                        #endregion
                    }

                    #region 31. REGULAR OVERTIME
                    if (Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpy_REGOTHr"]) != 0
                        || Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpy_REGOTAmt"]) != 0
                        || Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpy_REGNDHr"]) != 0
                        || Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpy_REGNDOTHr"]) != 0)
                    {
                        dr = dtDayCode.NewRow();
                        dr["Day Column"]    = "REG OT";
                        dr["IDNo"]          = IDNo;
                        dr["PayCycle"]      = PayPeriod;
                        dr["Seq"]           = 31;
                        dr["Hr"]            = Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpy_REGOTHr"])
                                                + Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpy_REGNDHr"])
                                                + Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpy_REGNDOTHr"]);
                        dr["Amt"]           = Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpy_REGOTAmt"])
                                                + Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpy_REGNDAmt"])
                                                + Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpy_REGNDOTAmt"]);
                        dtDayCode.Rows.Add(dr);

                        OTAmt += (Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpy_REGOTAmt"])
                                + Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpy_REGNDAmt"])
                                + Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpy_REGNDOTAmt"]));
                        OTHr += Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpy_REGOTHr"]);
                        NDHr += Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpy_REGNDHr"]);
                        NDOTHr += Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpy_REGNDOTHr"]);
                    }
                    #endregion

                    DataTable dtDayCodeOT = PayrollRegisterBL.GetDayCodeOT(PayrollType, PremiumGrp, companyCode, centralProfile);
                    for (int x = 0; x < dtDayCodeOT.Rows.Count; x++)
                    {
                        string colPrefix = "Tpy_" + dtDayCodeOT.Rows[x]["DayCode"].ToString().Trim();
                        if (Convert.ToInt32(dtDayCodeOT.Rows[x]["DayID"].ToString().Trim()) > 0)
                            colPrefix = "Tpm_Misc" + dtDayCodeOT.Rows[x]["DayID"].ToString().Trim();

                        #region 31 >>>
                        if (dtDayCodeOT.Rows[x]["DayCode"].ToString().Trim().ToUpper() != "REG"
                                && (Convert.ToDecimal(dtPayrollDetails.Rows[idx][colPrefix + "Hr"]) != 0
                                || Convert.ToDecimal(dtPayrollDetails.Rows[idx][colPrefix + "Amt"]) != 0
                                || Convert.ToDecimal(dtPayrollDetails.Rows[idx][colPrefix + "OTHr"]) != 0
                                || Convert.ToDecimal(dtPayrollDetails.Rows[idx][colPrefix + "OTAmt"]) != 0
                                || Convert.ToDecimal(dtPayrollDetails.Rows[idx][colPrefix + "NDHr"]) != 0
                                || Convert.ToDecimal(dtPayrollDetails.Rows[idx][colPrefix + "NDAmt"]) != 0
                                || Convert.ToDecimal(dtPayrollDetails.Rows[idx][colPrefix + "NDOTHr"]) != 0
                                || Convert.ToDecimal(dtPayrollDetails.Rows[idx][colPrefix + "NDOTAmt"]) != 0))
                        {
                            #region DAY CODE
                            dr = dtDayCode.NewRow();
                            dr["Day Column"]    = dtDayCodeOT.Rows[x]["DayCode"].ToString().Trim();
                            dr["IDNo"]          = IDNo;
                            dr["PayCycle"]      = PayPeriod;
                            dr["Seq"]           = Convert.ToInt32(dtDayCodeOT.Rows[x]["Seq"].ToString().Trim()) + 31;
                            dr["Hr"]            = Convert.ToDecimal(dtPayrollDetails.Rows[idx][colPrefix + "Hr"])
                                                    + Convert.ToDecimal(dtPayrollDetails.Rows[idx][colPrefix + "OTHr"])
                                                    + Convert.ToDecimal(dtPayrollDetails.Rows[idx][colPrefix + "NDHr"])
                                                    + Convert.ToDecimal(dtPayrollDetails.Rows[idx][colPrefix + "NDOTHr"]);
                            dr["Amt"]           = Convert.ToDecimal(dtPayrollDetails.Rows[idx][colPrefix + "Amt"])
                                                    + Convert.ToDecimal(dtPayrollDetails.Rows[idx][colPrefix + "OTAmt"])
                                                    + Convert.ToDecimal(dtPayrollDetails.Rows[idx][colPrefix + "NDAmt"])
                                                    + Convert.ToDecimal(dtPayrollDetails.Rows[idx][colPrefix + "NDOTAmt"]);
                            dtDayCode.Rows.Add(dr);

                            OTAmt       += Convert.ToDecimal(dtPayrollDetails.Rows[idx][colPrefix + "Amt"])
                                            + Convert.ToDecimal(dtPayrollDetails.Rows[idx][colPrefix + "OTAmt"])
                                            + Convert.ToDecimal(dtPayrollDetails.Rows[idx][colPrefix + "NDAmt"])
                                            + Convert.ToDecimal(dtPayrollDetails.Rows[idx][colPrefix + "NDOTAmt"]);

                            Hr          += Convert.ToDecimal(dtPayrollDetails.Rows[idx][colPrefix + "Hr"]);
                            OTHr        += Convert.ToDecimal(dtPayrollDetails.Rows[idx][colPrefix + "OTHr"]);
                            NDHr        += Convert.ToDecimal(dtPayrollDetails.Rows[idx][colPrefix + "NDHr"]);
                            NDOTHr      += Convert.ToDecimal(dtPayrollDetails.Rows[idx][colPrefix + "NDOTHr"]);
                            #endregion
                        }
                        #endregion
                    }

                    #region //Fillers 1-6
                    //DataTable dtDayCodeFILLER = PayrollRegisterBL.GetDayCodeOT(PayrollType, PremiumGrp, companyCode, centralProfile);
                    //for (int x = 0; x < dtDayCodeFILLER.Rows.Count; x++)
                    //{
                    //    #region 45 >>>
                    //    if (Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpm_Misc" + dtDayCodeFILLER.Rows[x]["DayID"].ToString().Trim() + "Hr"]) != 0
                    //        || Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpm_Misc" + dtDayCodeFILLER.Rows[x]["DayID"].ToString().Trim() + "Amt"]) != 0
                    //        || Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpm_Misc" + dtDayCodeFILLER.Rows[x]["DayID"].ToString().Trim() + "OTHr"]) != 0
                    //        || Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpm_Misc" + dtDayCodeFILLER.Rows[x]["DayID"].ToString().Trim() + "OTAmt"]) != 0
                    //        || Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpm_Misc" + dtDayCodeFILLER.Rows[x]["DayID"].ToString().Trim() + "NDHr"]) != 0
                    //        || Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpm_Misc" + dtDayCodeFILLER.Rows[x]["DayID"].ToString().Trim() + "NDAmt"]) != 0
                    //        || Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpm_Misc" + dtDayCodeFILLER.Rows[x]["DayID"].ToString().Trim() + "NDOTHr"]) != 0
                    //        || Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpm_Misc" + dtDayCodeFILLER.Rows[x]["DayID"].ToString().Trim() + "NDOTAmt"]) != 0)
                    //    {
                    //        #region DAY FILLER
                    //        dr = dtDayCode.NewRow();
                    //        dr["Day Column"] = dtDayCodeFILLER.Rows[x]["DayCode"].ToString().Trim();
                    //        dr["IDNo"] = IDNo;
                    //        dr["PayCycle"] = PayPeriod;
                    //        dr["Seq"] = Convert.ToInt32(dtDayCodeFILLER.Rows[x]["Seq"].ToString().Trim()) + 45;
                    //        dr["Hr"] = Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpm_Misc" + dtDayCodeFILLER.Rows[x]["DayID"].ToString().Trim() + "Hr"])
                    //                    + Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpm_Misc" + dtDayCodeFILLER.Rows[x]["DayID"].ToString().Trim() + "OTHr"])
                    //                    + Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpm_Misc" + dtDayCodeFILLER.Rows[x]["DayID"].ToString().Trim() + "NDHr"])
                    //                    + Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpm_Misc" + dtDayCodeFILLER.Rows[x]["DayID"].ToString().Trim() + "NDOTHr"]);

                    //        dr["Amt"] = Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpm_Misc" + dtDayCodeFILLER.Rows[x]["DayID"].ToString().Trim() + "Amt"])
                    //                    + Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpm_Misc" + dtDayCodeFILLER.Rows[x]["DayID"].ToString().Trim() + "OTAmt"])
                    //                    + Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpm_Misc" + dtDayCodeFILLER.Rows[x]["DayID"].ToString().Trim() + "NDAmt"])
                    //                    + Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpm_Misc" + dtDayCodeFILLER.Rows[x]["DayID"].ToString().Trim() + "NDOTAmt"]);
                    //        dtDayCode.Rows.Add(dr);

                    //        OTAmt += Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpm_Misc" + dtDayCodeFILLER.Rows[x]["DayID"].ToString().Trim() + "OTAmt"])
                    //            + Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpm_Misc" + dtDayCodeFILLER.Rows[x]["DayID"].ToString().Trim() + "Amt"])
                    //            + Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpm_Misc" + dtDayCodeFILLER.Rows[x]["DayID"].ToString().Trim() + "NDAmt"])
                    //            + Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpm_Misc" + dtDayCodeFILLER.Rows[x]["DayID"].ToString().Trim() + "NDOTAmt"]);

                    //        Hr += Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpm_Misc" + dtDayCodeFILLER.Rows[x]["DayID"].ToString().Trim() + "Hr"]);
                    //        OTHr += Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpm_Misc" + dtDayCodeFILLER.Rows[x]["DayID"].ToString().Trim() + "OTHr"]);
                    //        NDHr += Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpm_Misc" + dtDayCodeFILLER.Rows[x]["DayID"].ToString().Trim() + "NDHr"]);
                    //        NDOTHr += Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpm_Misc" + dtDayCodeFILLER.Rows[x]["DayID"].ToString().Trim() + "NDOTHr"]);
                    //        #endregion
                    //    }
                    //    #endregion
                    //}
                    #endregion

                    DataTable dtDayCodeOTHERTAXABLE = PayrollRegisterBL.GetDayCodeOtherTaxable();
                    for (int x = 0; x < dtDayCodeOTHERTAXABLE.Rows.Count; x++)
                    {
                        #region 60 >>>
                        if (Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpy_" + dtDayCodeOTHERTAXABLE.Rows[x]["DayCode"].ToString().Trim() + "Hr"]) != 0
                        || Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpy_" + dtDayCodeOTHERTAXABLE.Rows[x]["DayCode"].ToString().Trim() + "Amt"]) != 0)
                        {
                            dr = dtDayCode.NewRow();
                            dr["Day Column"] = dtDayCodeOTHERTAXABLE.Rows[x]["DayName"].ToString().Trim();
                            dr["IDNo"] = IDNo;
                            dr["PayCycle"] = PayPeriod;
                            dr["Seq"] = Convert.ToInt32(dtDayCodeOTHERTAXABLE.Rows[x]["Seq"].ToString().Trim()) + 60;
                            dr["Hr"] = Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpy_" + dtDayCodeOTHERTAXABLE.Rows[x]["DayCode"].ToString().Trim() + "Hr"]);
                            dr["Amt"] = Convert.ToDecimal(dtPayrollDetails.Rows[idx]["Tpy_" + dtDayCodeOTHERTAXABLE.Rows[x]["DayCode"].ToString().Trim() + "Amt"]);
                            dtDayCode.Rows.Add(dr);
                        }
                        #endregion
                    }

                    DataTable dtOtherTaxIncome = GetOtherTaxNonTaxIncome(true, IDNo, PayCycleCode, commonBL.GetSpecialCycleIndicator(PayCycleCode), "", "T", companyCode, centralProfile, dalhelper);
                    
                    for (int x = 0; x < dtOtherTaxIncome.Rows.Count; x++)
                    {
                        #region Header
                        if (x == 0)
                        {
                            dr = dtDayCode.NewRow();
                            dr["Day Column"] = "OTHER TAXABLE INCOME";
                            dr["IDNo"] = IDNo;
                            dr["PayCycle"] = PayPeriod;
                            dr["Seq"] = x + 81;
                            dr["Hr"] = DBNull.Value;
                            dr["Amt"] = DBNull.Value;
                            dtDayCode.Rows.Add(dr);
                        }
                        #endregion
                        #region Details
                        dr = dtDayCode.NewRow();
                        dr["Day Column"] = "   " + dtOtherTaxIncome.Rows[x]["Code"].ToString().Trim();
                        dr["IDNo"] = IDNo;
                        dr["PayCycle"] = PayPeriod;
                        dr["Seq"] = x + 82;
                        dr["Hr"] = 0.00;
                        dr["Amt"] = Convert.ToDecimal(dtOtherTaxIncome.Rows[x]["Amount"]);
                        dtDayCode.Rows.Add(dr);
                        #endregion
                    }
                    DataTable dtOtherNontaxIncome = GetOtherTaxNonTaxIncome(true, IDNo, PayCycleCode, commonBL.GetSpecialCycleIndicator(PayCycleCode), "", "N", companyCode, centralProfile, dalhelper);
                    for (int x = 0; x < dtOtherNontaxIncome.Rows.Count; x++)
                    {
                        #region Header
                        if (x == 0)
                        {
                            dr = dtDayCode.NewRow();
                            dr["Day Column"] = "OTHER NONTAXABLE INCOME";
                            dr["IDNo"] = IDNo;
                            dr["PayCycle"] = PayPeriod;
                            dr["Seq"] = x + 151;
                            dr["Hr"] = DBNull.Value;
                            dr["Amt"] = DBNull.Value;
                            dtDayCode.Rows.Add(dr);
                        }
                        #endregion
                        #region Details
                        dr = dtDayCode.NewRow();
                        dr["Day Column"] = "   " + dtOtherNontaxIncome.Rows[x]["Code"].ToString().Trim();
                        dr["IDNo"] = IDNo;
                        dr["PayCycle"] = PayPeriod;
                        dr["Seq"] = x + 152;
                        dr["Hr"] = 0.00;
                        dr["Amt"] = Convert.ToDecimal(dtOtherNontaxIncome.Rows[x]["Amount"]);
                        dtDayCode.Rows.Add(dr);
                        #endregion
                    }
                }
            }
            return dtDayCode;
        }

        public DataSet GetBonusHeading(string PayCycleCode, string companyCode, string centralProfile, DALHelper dalhelper)
        {
            #region query
            string query = string.Format(@"SELECT Mbn_BaseAmt1Heading
                                            ,Mbn_BaseAmt2Heading
                                            ,Mbn_BaseNumHeading
                                            ,Mbn_BaseCharHeading
                                            FROM Udv_Bonus
                                            LEFT JOIN {0}..M_Bonus
                                                ON Tbh_BonusCode = Mbn_BonusCode
                                                AND Mbn_CompanyCode = '{1}'
                                            WHERE Tbh_PayCycle = '{2}'
                                            ", centralProfile
                                             , companyCode
                                             , PayCycleCode);

            #endregion

            DataSet ds = dalhelper.ExecuteDataSet(query);
            return ds;
        }

        public DataTable GetDeductionDetails(bool Individual, string IDNumber, string PayCycleCode, string CycleIndicatorSpecial, string companyCode, string centralProfile, DALHelper dalhelper)
        {
            string condition = string.Empty;
            if (Individual)
                condition = string.Format("AND Tdd_IDNo = '{0}'", IDNumber);
            else
                condition = string.Format("AND Tdd_IDNo IN ({0})", IDNumber);

            string table = "T_EmpDeductionDtlFinalPay";
            if (CycleIndicatorSpecial == "P")
                table = "T_EmpDeductionDtlFullPayHst";

            #region Query
            string query = string.Format(@"SELECT Tdd_IDNo as IDNo
                                        , Tdd_PayCycle AS [Original Pay Cycle]
                                        , CONVERT(CHAR(10), Tdd_StartDate,101) AS Tdd_StartDate
                                        , Tdd_DeductionCode
                                        , Tdd_Amount
                                        FROM {1}
                                        WHERE Tdd_ThisPayCycle = '{0}'
                                        AND Tdd_Amount <> 0
                                        @COND
                                        ", PayCycleCode
                                         , table
                                         , centralProfile
                                         , companyCode);

            #endregion
            query = query.Replace("@COND", condition);
            DataTable dtResult = dalhelper.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public DataTable GetHoldPayrollDetails(string IDNumber, string PayCycleCode, string CycleIndicatorSpecial, string companyCode, string centralProfile, DALHelper dalhelper)
        {
            #region Query
            string query = string.Format(@"DECLARE @col NVARCHAR(MAX)
                                        SELECT @col= COALESCE(@col, '') + ',' + replace(Tef_HoldPayCycles,':', ':'+Tef_IDNo+ ';')
	                                        FROM Udv_FinalPay
	                                        WHERE Tef_PayCycle = '{0}'
		                                        AND Tef_IDNo = '{1}'

                                        SELECT LEFT(Data,7) as [Pay Cycle]
                                            , SUBSTRING(Data,len(LEFT(Data,CHARINDEX(':',Data)-1))+2,CHARINDEX(';',Data) - (len(LEFT(Data,CHARINDEX(':',Data)-1))+2)) as [IDNo]
                                            , LEFT(Data,CHARINDEX(':',Data)-1) as [Pay Cycle Code]
	                                        , CAST(RIGHT(Data,LEN(Data)-CHARINDEX(';',Data)) AS DECIMAl(9,2)) as [Amount]
                                        FROM {2}.dbo.Udf_Split(@col, ',')
                                        ", PayCycleCode
                                         , IDNumber
                                         , centralProfile
                                         , companyCode);

            #endregion
            DataTable dtResult = dalhelper.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public DataTable GetHoldPayrollDetails(string IDNumber, string PayCycleCode)
        {
            #region Query
            string query = string.Format(@"DECLARE @col NVARCHAR(MAX)
                                           SELECT @col= COALESCE(@col, '') + ', ' + REPLACE(Tef_HoldPayCycles,':', ':'+Tef_IDNo+ '|')
	                                       FROM T_EmpFinalPay 
	                                       WHERE Tef_PayCycle = '{0}'
                                               AND ISNULL(Tef_HoldPayCycles,'') <> '' 
		                                       AND Tef_IDNo IN ({1})

                                           SET @col = STUFF(@col, 1, 2, '')

                                         IF EXISTS(SELECT Tef_IDNo FROM T_EmpFinalPay WHERE Tef_PayCycle = '{0}' AND Tef_IDNo IN ({1}) AND LEN(Tef_HoldPayCycles) > 0)
                                            SELECT LEFT(UPPER(DATENAME(mm, LEFT(Tps_PayCycle,6)+'01')),3) + ' ' + FREQ.Mcd_Name  + ', ' + LEFT(Tps_PayCycle,4) + CASE WHEN Tps_CycleIndicator = 'S' THEN ' (' + ISNULL(CYCLETYPE.Mcd_Name,'') + ')' ELSE '' END as [Pay Cycle Name]
                                                , SUBSTRING(Data,LEN(LEFT(Data,CHARINDEX(':',Data)-1))+2,CHARINDEX('|',Data) - (LEN(LEFT(Data,CHARINDEX(':',Data)-1))+2)) as [IDNo]
                                                , LEFT(Data,CHARINDEX(':',Data)-1) as [Pay Cycle]
	                                            , CAST(ISNULL(RIGHT(Data,LEN(Data)-CHARINDEX('|',Data)),0) AS DECIMAl(9,2)) as [Amount]
                                            FROM {2}.dbo.Udf_Split(@col, ',')
                                            INNER JOIN T_PaySchedule PayCycleSeparation ON LEFT(Data,7) = Tps_PayCycle
                                            LEFT JOIN {2}..M_CodeDtl CYCLETYPE ON CYCLETYPE.Mcd_Code = Tps_CycleType      
	                                            AND CYCLETYPE.Mcd_CodeType = 'CYCLETYPE'     
	                                            AND CYCLETYPE.Mcd_CompanyCode = '{3}'  
                                            LEFT JOIN {2}..M_CodeDtl FREQ ON FREQ.Mcd_Code = RIGHT(Tps_PayCycle,1)      
                                                AND FREQ.Mcd_CodeType = 'PAYFREQ'     
                                                AND FREQ.Mcd_CompanyCode = '{3}'
                                        ", PayCycleCode
                                         , IDNumber
                                         , CentralProfile
                                         , CompanyCode);

            #endregion
            DataSet dsResult = dal.ExecuteDataSet(query);
            return (dsResult != null && dsResult.Tables.Count > 0 ? dsResult.Tables[0] : null);
        }

        public DataTable GetRetirementSeparateFromFinalPayDetails(bool Individual, string IDNumber, string PayCycleCode, string CycleIndicatorSpecial, string Condition, bool bRetirementSeparateFromLastPay, string companyCode, string centralProfile, DALHelper dalhelper)
        {
            string query = "";
            if (Condition == "")
                Condition = @"Tef_IDNo
                                            , Tef_PayCycle
                                            , Mem_LastName + CASE WHEN LEN(ISNULL(Mem_ExtensionName,'')) > 0 
												THEN  ' ' + Mem_ExtensionName ELSE '' END + ', ' + Mem_FirstName + ' ' + Mem_MiddleName as 'FullName'
                                            , CONVERT(CHAR(10), Mem_IntakeDate, 101) as Mem_IntakeDate
                                            , CONVERT(CHAR(10), Mem_SeparationDate, 101) as Mem_SeparationDate";


            #region Query
            if (bRetirementSeparateFromLastPay)
                query = @"SELECT {4}
                                            , Tef_RetirementSalary as [Monthly Rate]
                                            , Tef_TenureYears as [Service Years]
                                            , Tef_RetirementRate as [Retirement Rate]
                                            , Tef_RetirementTax as [Taxable Amount]
											, Tef_RetirementNetTax as [Nontaxable Amount]
                                            , Tef_OtherRetirementTaxableAmt as [Other Taxable Amount]
                                            , Tef_OtherRetirementNontaxableAmt as [Other Nontaxable Amount]
                                            , Myt_ExcessOverCompensationLevel as [Less Compensation Level]
                                            , Tef_RetirementTax - Myt_ExcessOverCompensationLevel as [Net Excess of Compensation Level] 
                                            , Myt_TaxOnExcess as [Tax on Excess]
                                            , Myt_TaxOnCompensationLevel as [Add Tax Amount on Compensation Level]
											, Tef_RetirementTax	as [Less Withholding Tax]
                                            , Tef_RetirementNetTax as [Retirement Net of Withholding Tax]
                                            FROM Udv_FinalPay
                                            INNER JOIN M_Employee On Tef_IDNo = Mem_IDNo
                                            LEFT JOIN {2}..M_YearlyTaxSchedule ON Myt_BracketNo =  Tef_RetirementTaxBracket
                                                AND Myt_CompanyCode  = '{3}'
                                            WHERE Tef_PayCycle= '{0}'
                                                AND Tef_RetirementSeparateFromLastPay = 1
                                            {1}
                                        ";
            else
                query = @"SELECT {4}
                                            , Tef_RetirementSalary as [Monthly Rate]
                                            , Tef_TenureYears as [Service Years]
                                            , Tef_RetirementRate as [Retirement Rate]
                                            , TaxAllowanceAmt as [Taxable Amount]
											, NonTaxAllowanceAmt as [Nontaxable Amount]
                                            FROM Udv_FinalPay
                                            INNER JOIN M_Employee ON Tef_IDNo = Mem_IDNo
                                            INNER JOIN (SELECT Tin_IDNo
											            ,Tin_PayCycle
                                                        ,TaxAllowanceAmt = SUM(CASE WHEN Min_TaxClass = 'T' THEN Tin_IncomeAmt ELSE 0 END)
                                                        ,NonTaxAllowanceAmt = SUM(CASE WHEN Min_TaxClass = 'N' THEN Tin_IncomeAmt ELSE 0 END)
												         FROM T_EmpIncomeFinalPay 
												         INNER JOIN {2}..M_Income
													        ON Min_IncomeCode = Tin_IncomeCode
													        AND Min_CompanyCode = '{3}'
												         WHERE Min_IncomeGroup = 'RET'
												         GROUP BY Tin_IDNo, Tin_PayCycle) Allowance ON Tef_IDNo = Tin_IDNo
													AND Tef_PayCycle = Tin_PayCycle
                                            WHERE Tef_PayCycle= '{0}'
                                            {1}
                                        ";

            if (Individual)
                query = string.Format(query
                                         , PayCycleCode
                                         , string.Format("AND Tef_IDNo = '{0}'", IDNumber)
                                         , centralProfile
                                         , companyCode
                                         , Condition);
            else
                query = string.Format(query
                                         , PayCycleCode
                                         , ""
                                         , centralProfile
                                         , companyCode
                                         , Condition);
            #endregion
            DataTable dtResult = dalhelper.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public DataTable GetLeaveRefund(bool Individual, string IDNumber, string PayCycleCode, string CycleIndicatorSpecial, string companyCode, string centralProfile, DALHelper dalhelper)
        {
            string table2 = "T_EmpLeaveRefundHdr";
            string table3 = "T_EmpLeaveRefundDtl";
            if (CycleIndicatorSpecial == "P")
            {
                table2 = "T_EmpLeaveRefundHdrHst";
                table3 = "T_EmpLeaveRefundDtlHst";
            }
            LVHRENTRY = commonBL.GetParameterValueFromPayroll("LVHRENTRY", companyCode, dalhelper);
            FPLVREFUNDDIVIDEND = commonBL.GetParameterDataTypeValueFromPayroll("FPLVREFUNDDIVIDEND", companyCode, dalhelper);
            FPLVREFUNDDIVISOR = commonBL.GetParameterDataTypeValueFromPayroll("FPLVREFUNDDIVISOR", companyCode, dalhelper);

            #region query
            string query = @"SELECT Tld_LeaveCode as [Code]
		                                    , Tld_IDNo as [IDNo]
		                                    , Tlh_PayCycle as [Pay Cycle Code]
                                            , {5}  as [DividendValue]
										    , {6}  as [DivisorValue]
                                            , Tld_HourlyDailyRate as [HourlyDailyRate]
                                            , CONVERT(VARCHAR(20), CASE WHEN '{4}' = 'TRUE' THEN Tld_BegCredit ELSE Tld_BegCredit / @HOURSINDAY END) + ' * (' +  CONVERT(VARCHAR(10), {5}) + '/' + CONVERT(VARCHAR(10), {6}) + ')'  as [Entitlement]
                                            , CASE WHEN '{4}' = 'TRUE' THEN Tld_BegCredit ELSE Tld_BegCredit / @HOURSINDAY END as [BegCredit]
                                            , CASE WHEN '{4}' = 'TRUE' THEN Tld_CarryForwardCredit ELSE Tld_CarryForwardCredit / @HOURSINDAY END as [CarryForward]
										    , CASE WHEN '{4}' = 'TRUE' THEN Tld_UsedCredit ELSE Tld_UsedCredit / @HOURSINDAY END as [Usage]
                                            , CASE WHEN '{4}' = 'TRUE' THEN Tld_LeaveHr ELSE Tld_LeaveHr / @HOURSINDAY END as [Balance]
                                            , CASE WHEN '{4}' = 'TRUE' THEN Tld_LeaveTaxableHr ELSE Tld_LeaveTaxableHr / @HOURSINDAY END as [Taxable]
                                            , CASE WHEN '{4}' = 'TRUE' THEN Tld_LeaveNontaxableHr ELSE Tld_LeaveNontaxableHr / @HOURSINDAY END as [Nontaxable]
                                            , Tld_SalaryRate as [Salary Rate]
                                            , Tld_HourlyDailyRate as [{7}]
                                            , Tld_LeaveAmt as [Amount]
		                                   FROM {3}
		                                   INNER JOIN {2} ON Tld_ControlNo = Tlh_ControlNo
		                                   WHERE Tlh_PayCycle = '{0}'
				                                AND Tld_PostFlag = 1
				                                AND Tld_LeaveHr <> 0
				                                {1}
                            ";
            #endregion
            query = string.Format(query
                                    , PayCycleCode
                                    , (Individual ? string.Format("AND Tld_IDNo = '{0}'", IDNumber) : string.Format("AND Tld_IDNo IN ({0})", IDNumber))
                                    , table2
                                    , table3
                                    , LVHRENTRY
                                    , (FPLVREFUNDDIVIDEND == "I" ? "CAST(Tld_DividendValue as tinyint)" : "Tld_DividendValue")
                                    , (FPLVREFUNDDIVISOR == "I" ? "CAST(Tld_DivisorValue as tinyint)" : "Tld_DivisorValue")
                                    , (Convert.ToBoolean(LVHRENTRY) ? "Hourly Rate" : "Daily Rate"));

            query = query.Replace("@HOURSINDAY", CommonBL.HOURSINDAY.ToString());

            DataTable dtResult = null;
            dtResult = dalhelper.ExecuteDataSet(query).Tables[0];

            return dtResult;
        }

        public DataTable GetTaxRefundPayable(bool bTaxRefund, bool bForReport, bool Individual, string IDNumber, string PayCycleCode, string Year, string TaxCode, string companyCode, string centralProfile, DALHelper dalhelper)
        {
            try
            {
                string condition = string.Empty;
                if (Individual)
                    condition = string.Format("AND Tpy_IDNo = '{0}'", IDNumber);
                else
                    condition = string.Format("AND Tpy_IDNo IN ({0})", IDNumber);

                string query = string.Format(@"
                                SELECT {6}
                                --, Tpy_YTDTotalTaxableIncomeAmtBefore + Tpy_TotalTaxableIncomeAmt as [Total Taxable Compensation Income]        
                                , Tpy_YTDTotalTaxableIncomeAmt + (Tpy_YTDSSSAmt + Tpy_YTDMPFAmt + Tpy_YTDPhilhealthAmt + Tpy_YTDPagIbigAmt + Tpy_YTDUnionAmt + Tpy_TotalExemptions + Tpy_PremiumPaidOnHealth) as [Total Taxable Compensation Income]   
                                , 0 as [Less:]
                                , CONVERT(VARCHAR, CONVERT(MONEY, (Tpy_YTDSSSAmt + Tpy_YTDMPFAmt + Tpy_YTDPhilhealthAmt + Tpy_YTDPagIbigAmt + ISNULL(Tpy_YTDUnionAmt,0))), 1) 
														as  [       SSS,GSIS,PHIC,Pag-ibig Contributions and Union Dues]
                                ,Tpy_TotalExemptions	as  [       Personal and Additional Exemption {5}]
                                ,Tpy_PremiumPaidOnHealth as [       Premium Paid on Health and/or Hospital Insurance]
                                ,Tpy_NetTaxableIncomeAmt as [Net Taxable Compensation Income]
                                ,CONVERT(VARCHAR, CONVERT(MONEY, Myt_ExcessOverCompensationLevel), 1) as [Less Compensation Level]
                                ,CONVERT(VARCHAR, CONVERT(MONEY, Tpy_NetTaxableIncomeAmt - Myt_ExcessOverCompensationLevel), 1) as [Net Excess of Compensation Level]
                                ,CONVERT(INT,Myt_TaxOnExcess) as [Tax on Excess]
                                ,CONVERT(VARCHAR, CONVERT(MONEY, Myt_TaxOnCompensationLevel), 1) as [Add Tax Amount on Compensation Level]
								,Tpy_YTDWtaxAmt as [Tax Due]
								,Tpy_YTDWtaxAmtBefore as [Total Withholding Tax]
                                {4}
                                FROM Udv_PayrollFinalPay
                                LEFT JOIN {3}..M_YearlyTaxSchedule ON Myt_BracketNo =  Tpy_TaxBracket
							        AND Myt_PayCycle = (SELECT MAX(Myt_PayCycle) as [PayCycle]
                                                        FROM {3}..M_YearlyTaxSchedule
                                                        WHERE Myt_PayCycle <= '{1}'
                                                          AND Myt_CompanyCode = '{2}'
                                                          AND Myt_RecordStatus = 'A')
                                   AND Myt_CompanyCode = '{2}'
                                WHERE Tpy_PayCycle = '{1}'
                                    {0}
                                    {7}"
                                , condition
                                , PayCycleCode
                                , companyCode
                                , centralProfile
                                , (bTaxRefund ? ",ABS(Tpy_Wtaxamt) as [Tax Refund]" : ",Tpy_Wtaxamt as [Tax Payable]")
                                , (Convert.ToInt32(PayCycleCode.Substring(0, 4)) < 2018 ? string.Format("({0})", TaxCode) : "")
                                , (bForReport ? "Tpy_IDNo as IDNumber" : "'Amount' as Item")
                                , (bTaxRefund ? "AND Tpy_Wtaxamt < 0" : "AND Tpy_Wtaxamt > 0")); 


                DataTable dtResult = null;
                dtResult = dalhelper.ExecuteDataSet(query).Tables[0];

                return dtResult;
            }
            catch
            {
                return new DataTable();
            }
        }

        public DataTable GetTaxRefundPayable(bool bTaxRefund, bool Individual, string IDNumber, string PayCycleCode, string Year, string companyCode, string centralProfile, DALHelper dalhelper)
        {
            try
            {
                string condition = string.Empty;
                if (Individual)
                    condition = string.Format("AND Tpy_IDNo = '{0}'", IDNumber);
                else
                    condition = string.Format("AND Tpy_IDNo IN ({0})", IDNumber);

                string query = string.Format(@"
                                SELECT Tpy_IDNo as IDNumber
                                , Tpy_YTDTotalTaxableIncomeAmtBefore + Tpy_TotalTaxableIncomeAmt as [Total Taxable Compensation Income]        
                                , 0 as [Less:]
                                , CONVERT(VARCHAR, CONVERT(MONEY, (Tpy_YTDSSSAmt + Tpy_YTDPhilhealthAmt + Tpy_YTDPagIbigAmt + ISNULL(Tpy_YTDUnionAmt,0))), 1) 
														as  [       SSS,GSIS,PHIC,Pag-ibig Contributions and Union Dues]
                                ,Tpy_TotalExemptions	as  [       Personal and Additional Exemption ]
                                ,Tpy_PremiumPaidOnHealth as [       Premium Paid on Health and/or Hospital Insurance]
                                ,Tpy_NetTaxableIncomeAmt as [Net Taxable Compensation Income]
                                ,CONVERT(VARCHAR, CONVERT(MONEY, Myt_ExcessOverCompensationLevel), 1) as [Less Compensation Level]
                                ,CONVERT(VARCHAR, CONVERT(MONEY, Tpy_NetTaxableIncomeAmt - Myt_ExcessOverCompensationLevel), 1) as [Net Excess of Compensation Level]
                                ,CONVERT(INT,Myt_TaxOnExcess) as [Tax on Excess]
                                ,CONVERT(VARCHAR, CONVERT(MONEY, Myt_TaxOnCompensationLevel), 1) as [Add Tax Amount on Compensation Level]
								,Tpy_YTDWtaxAmt as [Tax Due]
								,Tpy_YTDWtaxAmtBefore as [Total Withholding Tax]
                                {4}
                                FROM Udv_PayrollFinalPay
                                LEFT JOIN {3}..M_YearlyTaxSchedule
                                ON Myt_BracketNo =  Tpy_TaxBracket
							    AND Myt_PayCycle = (SELECT MAX(Myt_PayCycle) as [PayCycle]
                                                    FROM {3}..M_YearlyTaxSchedule
                                                    WHERE Myt_PayCycle <= '{1}'
                                                          AND Myt_CompanyCode = '{2}'
                                                          AND Myt_RecordStatus = 'A')
                                                    AND Myt_CompanyCode = '{2}'
                                WHERE Tpy_PayCycle = '{1}'
                                    {0}
                                    {5}"
                                , condition
                                , PayCycleCode
                                , companyCode
                                , centralProfile
                                , (bTaxRefund ? ",ABS(Tpy_Wtaxamt) as [Tax Refund]" : ",Tpy_Wtaxamt as [Tax Payable]")
                                , (bTaxRefund ? "AND Tpy_Wtaxamt < 0" : "AND Tpy_Wtaxamt > 0"));


                DataTable dtResult = null;
                dtResult = dalhelper.ExecuteDataSet(query).Tables[0];

                return dtResult;
            }
            catch
            {
                return new DataTable();
            }
        }

        public DataTable GetOtherTaxNonTaxIncome(bool Individual, string IDNumber, string PayCycleCode, string CycleIndicatorSpecial, string Condition, string TaxClass, string companyCode, string centralProfile, DALHelper dalhelper)
        {
            string table = "T_EmpIncomeFinalPay";

            if (CycleIndicatorSpecial == "P")
                table = "T_EmpIncomeFinalPayHst";

            #region query
            string query = @"SELECT Tin_IncomeCode + ' (' + Tin_OrigPayCycle + ')' AS [Code]
                                , Tin_IncomeAmt AS [Amount]
                                FROM {4}				
                                INNER JOIN {3}..M_Income 
	                                ON Min_CompanyCode = '{2}'	
	                                AND Min_IncomeCode = Tin_IncomeCode 
                                    AND Min_TaxClass = '{5}'	
                                WHERE Tin_PayCycle = '{0}'	
                                    {1} 
	                                AND Tin_PostFlag =  1 
                                    AND Tin_IncomeCode NOT IN (SELECT Min_IncomeCode FROM {3}..M_Income 
                                                                WHERE Min_CompanyCode = '{2}'	
				                                                AND Min_IncomeGroup IN ('LVR','BON','RET','HSA'))";
            #endregion
            query = string.Format(query, PayCycleCode
                                        , (Individual ? string.Format("AND Tin_IDNo = '{0}'", IDNumber) : string.Format("AND Tin_IDNo IN ({0})", IDNumber))
                                        , companyCode
                                        , centralProfile
                                        , table
                                        , TaxClass);

            DataTable dtResult = null;
            dtResult = dalhelper.ExecuteDataSet(query).Tables[0];

            return dtResult;
        }

        public DataTable GetOtherTaxNonTaxPostedUnpostedIncome(bool Individual, string IDNumber, string PayCycleCode, string CycleIndicatorSpecial, string Condition, string TaxClass, string companyCode, string centralProfile, DALHelper dalhelper)
        {
            string table = "T_EmpIncomeFinalPay";

            if (CycleIndicatorSpecial == "P")
                table = "T_EmpIncomeFinalPayHst";

            #region query
            string query = @"SELECT Tin_IncomeCode AS [Code]
                                , Tin_IDNo as [IDNo]
                                , Tin_PayCycle AS [Pay Cycle Code]
                                , Tin_OrigPayCycle AS [Orig Pay Cycle Code]
                                , CASE WHEN Tin_PostFlag = 1 THEN Tin_IncomeAmt ELSE 0 END [Posted Amount]
						        , CASE WHEN Tin_PostFlag = 0 THEN Tin_IncomeAmt ELSE 0 END [Unposted Amount]
                                , Tin_OrigPayCycle AS [Original Pay Cycle]
                                FROM {4}				
                                INNER JOIN {3}..M_Income 
	                                ON Min_CompanyCode = '{2}'	
	                                AND Min_IncomeCode = Tin_IncomeCode 
                                    AND Min_TaxClass = '{5}'	
                                WHERE Tin_PayCycle = '{0}'	
                                    {1} 
                                    AND Tin_IncomeCode NOT IN 
                                            (SELECT Min_IncomeCode FROM {3}..M_Income 
                                                WHERE Min_CompanyCode = '{2}'	
				                                AND Min_IncomeGroup IN ('LVR','BON','RET','HSA')
			                                UNION ALL  
			                                SELECT 'HOLDSALARY')
                                ORDER BY 2, 4";
            #endregion
            if (Individual)
            query = string.Format(query, PayCycleCode
                                        , string.Format("AND Tin_IDNo = '{0}'", IDNumber)
                                        , companyCode
                                        , centralProfile
                                        , table
                                        , TaxClass);
            else
                query = string.Format(query, PayCycleCode
                                        , ""
                                        , companyCode
                                        , centralProfile
                                        , table
                                        , TaxClass);
            DataTable dtResult = null;
            dtResult = dalhelper.ExecuteDataSet(query).Tables[0];

            return dtResult;
        }

        public DataSet GetPayrollTrack(string paycyclecode, string cycleTypeCode, string profilecode, string companyCode, string centralProfile, DALHelper dalhelper)
        {
            string condition = string.Empty;
            string query = string.Format(@"SELECT 
                                            Mem_IDNo as [ID Number]
                                            , Mem_LastName + CASE WHEN LEN(ISNULL(Mem_ExtensionName,'')) > 0 
																                THEN  ' ' + Mem_ExtensionName ELSE '' END as [Last Name]
                                            , Mem_FirstName as [First Name]
                                            , Mem_MiddleName as [Middle Name] 
                                            , Tpt_ModuleCode as [Module Code]
                                            FROM M_Employee
                                            INNER JOIN T_EmpFinalPay ON Mem_IDNo = Tef_IDNo
                                                AND Tef_PayCycle = '{1}'
                                            @USERCOSTCENTERACCESSCONDITION
                                            INNER JOIN T_EmpPayrollTrack
                                            ON Tpt_IDNo = Mem_IDNo
                                            WHERE 1=1
                                                AND Tpt_Type = '{0}'
                                            ---@CONDITIONS
                                            GROUP BY Mem_IDNo, Mem_LastName, Mem_FirstName,  Mem_MiddleName, Mem_ExtensionName, Tpt_ModuleCode", cycleTypeCode, paycyclecode);
            //query = query.Replace("@CONDITIONS", condition);
            query = query.Replace("@USERCOSTCENTERACCESSCONDITION", (new HRCReportsBL()).UserCostCenterAccessTmpQuery(profilecode, "PAYROLL", LoginInfo.getUser().UserCode, "Mem_CostcenterCode", "Mem_PayrollGroup", "Mem_EmploymentStatusCode", "Mem_PayrollType", companyCode, centralProfile, false));
            DataSet ds = null;
            try
            {
                ds = dalhelper.ExecuteDataSet(query);
            }
            catch (Exception)
            {

                throw;
            }
            return ds;
        }

        public void DeleteEmployeePayrollDifferential(string EmpList, string CycleType, DALHelper dalHelper)
        {
            string strEmployeeIdQuery = "AND Tpt_IDNo IN (" + EmpList + ")";
            string query = @"DELETE FROM T_EmpPayrollTrack
                             WHERE Tpt_Type = '{1}'
                            {0}";
            query = string.Format(query, strEmployeeIdQuery, CycleType);
            dalHelper.ExecuteNonQuery(query);
        }

        public DataTable FinalPayExceptionList(string profileCode, string centralProfile, string PayCycle, string EmployeeID, string userlogin, string companyCode, string ErrorType, string menucode, DALHelper dalHelper)
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

        public DataTable GetFinalPaySummary(bool bIndividual, string IDNumber, string PayCycle, string companyCode, string centralProfile, string CycleIndicatorSpecial, string CCTRDSPLY, DALHelper dalHelper)
        {
            string condition = string.Empty;
            if (bIndividual)
                condition = string.Format("AND CALC.Tpy_IDNo = '{0}'", IDNumber);
            else
                condition = string.Format("AND CALC.Tpy_IDNo IN ({0})", IDNumber);

            #region query
            string query = @"
                                DECLARE @MDIVISOR INT

                                SET @MDIVISOR = (SELECT DISTINCT Mpd_ParamValue 
                                                    FROM M_PolicyDtl 
                                                    INNER JOIN Udv_PayrollFinalPay CALC
                                                    ON CALC.Tpy_EmploymentStatus = M_PolicyDtl.Mpd_SubCode
                                                    WHERE Mpd_PolicyCode = 'MDIVISOR'
                                                        AND M_PolicyDtl.Mpd_CompanyCode = '{2}'
													    AND M_PolicyDtl.Mpd_RecordStatus = 'A'
                                                    ---{1}
                                                )

                                      SELECT CALC.Tpy_IDNo as IDNo
                                     , Mem_LastName
                                     , Mem_FirstName
                                     , Mem_MiddleName
                                     , Mem_LastName + CASE WHEN LEN(ISNULL(Mem_ExtensionName,'')) > 0 
											THEN  ' ' + Mem_ExtensionName ELSE '' END + ', ' + Mem_FirstName + ' ' + Mem_MiddleName as 'FullName'
                                     , Mem_TaxCode
                                     , CALC.Tpy_PayCycle
                                     , CASE when Tpy_PayrollType = 'M'
							            then Tpy_SalaryRate 
							            when  Tpy_PayrollType = 'D'
							            then ((Tpy_SalaryRate * @MDIVISOR) / 12) 
							            end [BasicSalary]
						             , CASE when Tpy_PayrollType = 'M'
							            then ((Tpy_SalaryRate * 12) / @MDIVISOR)
							            when Tpy_PayrollType = 'D'
							            then Tpy_SalaryRate
							            end [DailyRate]
                                        ,Tpy_HourRate
                                        ,Tpy_ABSAmt	
                                        ,Tpy_TotalREGAmt
                                        ,Tpy_TotalOTNDAmt
                                        ,Tpy_TotalAdjAmt
                                        ,Tpy_TaxableIncomeAmt
                                        ,Tpy_TotalTaxableIncomeAmt
                                        ,Tpy_NontaxableIncomeAmt
                                        ,Tpy_GrossIncomeAmt
                                        ,Tpy_WtaxAmt
                                        ,CASE WHEN Tpy_WtaxAmt < 0 THEN (Tpy_WtaxAmt * -1) ELSE 0 END [Tax Refund]
                                        ,CASE WHEN Tpy_WtaxAmt < 0 THEN 0 ELSE Tpy_WtaxAmt END [Tax Payable]		
                                        ,Tpy_TaxBaseAmt
                                        ,Tpy_TaxRule
                                        ,Tpy_TaxShare
                                        ,Tpy_TaxBracket
                                        ,Tpy_TaxCode
                                        ,Tpy_IsTaxExempted
                                        ,Tpy_SSSEE
                                        ,Tpy_SSSER
                                        ,Tpy_MPFEE
                                        ,Tpy_MPFER
                                        ,Tpy_ECFundAmt
                                        ,Tpy_SSSBaseAmt
                                        ,Tpy_SSSRule
                                        ,Tpy_SSSShare
                                        ,Tpy_PhilhealthEE
                                        ,Tpy_PhilhealthER
                                        ,Tpy_PhilhealthBaseAmt
                                        ,Tpy_PhilhealthRule
                                        ,Tpy_PhilhealthShare
                                        ,Tpy_PagIbigEE
                                        ,Tpy_PagIbigER
                                        ,Tpy_PagIbigTaxEE
                                        ,Tpy_PagIbigBaseAmt
                                        ,Tpy_PagIbigRule
                                        ,Tpy_PagIbigShare
                                        ,Tpy_UnionAmt
                                        ,Tpy_UnionRule
                                        ,Tpy_UnionShare
                                        ,Tpy_UnionBaseAmt                   
                                        ,Tpy_OtherDeductionAmt
                                        ,Tpy_TotalDeductionAmt
                                        ,Tpy_NetAmt
                                        ,Tpy_CostcenterCode
                                        ,{0}.dbo.Udf_DisplayCostCenterName('{2}', Tpy_CostcenterCode, '{3}') AS CostCenterName
                                        ,Tpy_PayrollType
                                        ,Tpy_PaymentMode
                                        ,Tpy_PremiumGrpCode
                                        ,Tpy_PayrollGroup
                                        ,Tpy_WorkStatus
                                        ,PAYROLLTYPE.Mcd_Name AS 'PayrollType'
									    ,EMPSTAT.Mcd_Name AS 'EmploymentStatus'
                                        ,PAYMODE.Mcd_Name AS 'PaymentMode'
                                        ,CONVERT(VARCHAR(10),Mem_IntakeDate,101) AS Mem_IntakeDate
                                        ,CONVERT(VARCHAR(10),Mem_RegularDate,101) AS Mem_RegularDate
                                        ,CONVERT(VARCHAR(10),Mem_SeparationDate,101) AS Mem_SeparationDate
                                        ,Tpy_RemainingPayCycle
                                        ,Tpy_YTDTotalTaxableIncomeAmtBefore
                                        ,Tpy_YTDTotalTaxableIncomeAmt
                                        ,Tpy_YTDREGAmt
                                        ,Tpy_YTDWtaxAmtBefore
                                        ,Tpy_YTDWtaxAmt
                                        ,Tpy_YTDSSSAmtBefore
                                        ,Tpy_YTDSSSAmt
                                        ,Tpy_YTDPhilhealthAmtBefore
                                        ,Tpy_YTDPhilhealthAmt
									    ,Tpy_YTDPagIbigTaxAmtBefore
                                        ,Tpy_YTDPagIbigTaxAmt
                                        ,Tpy_YTDPagIbigAmtBefore
                                        ,Tpy_YTDPagIbigAmt
                                        ,Tpy_YTDNontaxableAmtBefore
                                        ,Tpy_YTDNontaxableAmt
                                        ,Tpy_TotalExemptions
                                        ,Tpy_YTDUnionAmtBefore
                                        ,Tpy_YTDUnionAmt
                                        ,Tpy_YTDMPFAmtBefore
                                        ,Tpy_YTDMPFAmt 
                                        ,Mem_Tenure
                                        ,CONVERT(DATE,Mem_BirthDate) AS Mem_BirthDate
                                        ,Mem_Age
                                        ,Tef_LastPayPriorSeparation
                                        ,Tef_PayCycleSeparation
                                        ,CONVERT(CHAR(10), Tps_StartCycle, 101) + ' - ' +  CONVERT(CHAR(10), Tps_EndCycle, 101) as [Pay Cycle Separation Range]
                                        ,Tef_RegularPayRule  --M_PolicyHdr
                                        ,Tef_ZeroOutDeductionCodes
                                        ,Tef_ComputeHoldSalary
                                        ,Tef_HoldPayCycles
                                        ,Tef_ComputeFixAllowance
                                        ,Tef_FixAllowanceRule --M_PolicyHdr
                                        ,Tef_Compute13thMonthPay
                                        ,Tef_13thMonthPayStart
                                        ,Tef_13thMonthPayEnd
                                        ,Tef_ComputeLeaveRefund
                                        ,Tef_LeaveTypeYear
                                        ,Tef_ComputeRetirement
                                        ,Tef_RetirementType
                                        ,RETIRETYPE.Mcd_Name AS 'RetirementType'
                                        ,Tef_RetirementSeparateFromLastPay
                                        ,Tef_RetirementSalary
                                        ,Tef_RetirementAmt
                                        ,Tef_RetirementRate
                                        ,Tef_RetirementTax
                                        ,Tef_RetirementNetTax
                                        ,Tef_TenureYears
                                        ,Tef_LastSalary
                                        ,Tef_HoldSalary
                                        ,Tef_13thMonthPay
                                        ,Tef_LeaveRefund
                                        ,Tbd_BaseAmt1
                                        ,Tbd_BaseAmt2
                                        ,Tbd_BaseChar
                                        ,Tbd_BaseNum
                                        ,Tbd_TotalBonusAmt
                                        ,Tbd_AccumBonusBeforeThisCycle
                                        ,POSITION.Mcd_Name AS 'Position'
                                FROM Udv_PayrollFinalPay CALC
                                INNER JOIN M_Employee ON CALC.Tpy_IDNo = Mem_IDNo
                                LEFT JOIN Udv_FinalPay ON CALC.Tpy_IDNo = Tef_IDNo
                                     AND CALC.Tpy_PayCycle = Tef_PayCycle
                                LEFT JOIN T_PaySchedule ON Tef_PayCycleSeparation = Tps_PayCycle
                                LEFT JOIN Udv_Bonus ON  CALC.Tpy_IDNo = Tbd_IDNo
									AND CALC.Tpy_PayCycle = Tbh_PayCycle
                                LEFT JOIN {0}..M_CodeDtl PAYGRP ON PAYGRP.Mcd_Code = Tpy_PayrollGroup
                                    AND PAYGRP.Mcd_CodeType = 'PAYGRP' 
                                    AND PAYGRP.Mcd_CompanyCode = '{2}'
                               LEFT JOIN {0}..M_CodeDtl PAYROLLTYPE ON PAYROLLTYPE.Mcd_Code = Tpy_PayrollType
                                    AND PAYROLLTYPE.Mcd_CodeType = 'PAYTYPE' 
                                    AND PAYROLLTYPE.Mcd_CompanyCode = '{2}'
							   LEFT JOIN {0}..M_CodeDtl EMPSTAT ON EMPSTAT.Mcd_Code = Tpy_EmploymentStatus
                                    AND EMPSTAT.Mcd_CodeType = 'EMPSTAT' 
                                    AND EMPSTAT.Mcd_CompanyCode = '{2}'
                               LEFT JOIN {0}..M_CodeDtl PAYMODE ON PAYMODE.Mcd_Code = Tpy_PaymentMode
                                    AND PAYMODE.Mcd_CodeType = 'PAYMODE' 
                                    AND PAYMODE.Mcd_CompanyCode = '{2}'
                               LEFT JOIN {0}..M_CodeDtl RETIRETYPE ON RETIRETYPE.Mcd_Code = Tef_RetirementType
                                    AND RETIRETYPE.Mcd_CodeType = 'RETIRETYPE' 
                                    AND RETIRETYPE.Mcd_CompanyCode = '{2}' 
                               LEFT JOIN {0}..M_CodeDtl POSITION ON POSITION.Mcd_Code = Tpy_PositionCode
                                    AND POSITION.Mcd_CodeType = 'POSITION'
	                                AND POSITION.Mcd_CompanyCode = '{2}'
                               WHERE CALC.Tpy_PayCycle = '{1}' 
                                    {4}";
            #endregion
            query = string.Format(query
                                    , centralProfile
                                    , PayCycle
                                    , companyCode
                                    , CCTRDSPLY
                                    , condition);

            DataTable dtResult = null;
            dtResult = dalHelper.ExecuteDataSet(query).Tables[0];

            return dtResult;
        }

        public DataTable GetIncomeByIncomeGroup(bool bIsIndividual, string IDNumber, string PayCycle, string IncomeGroup, bool bWithPostedUnposted, string strSpecialCycleIndicator, string companyCode, string centralProfile, DALHelper dalHelper)
        {
            string table = "T_EmpIncomeFinalPay";

            try
            {
                if (strSpecialCycleIndicator == "P")
                    table = "T_EmpIncomeFinalPayHst";
                else
                {
                    table = "T_EmpIncomeFinalPay";
                }

            }
            catch { }


            #region query
            string query = @"SELECT CASE WHEN '{5}' = 'HSA' THEN Tin_IncomeCode + ' (' + Tin_OrigPayCycle + ')' 
                                    ELSE Tin_IncomeCode END as [Code]
                                    , Tin_IDNo as  [IDNo]
                                    , Tin_PayCycle as [Pay Cycle Code]
                                    , Tin_OrigPayCycle as [Orig Pay Cycle Code]
	                                , CASE WHEN Min_Taxclass = 'T' THEN 'TAXABLE' ELSE 'NONTAXABLE' END as [Tax Class]
                                    , CASE WHEN Tin_PostFlag = 1 THEN Tin_IncomeAmt ELSE 0 END [Posted Amount]
						            , CASE WHEN Tin_PostFlag = 0 THEN Tin_IncomeAmt ELSE 0 END [Unposted Amount]
                                    , Tin_OrigPayCycle AS [Original Pay Cycle]
                                FROM {4}
                                INNER JOIN {3}..M_Income on Min_incomecode = Tin_incomecode
	                                AND Min_CompanyCode = '{2}'
	                                AND Min_IncomeGroup = '{5}'
                                WHERE Tin_PayCycle =  '{0}'
                                    {1}
                                ORDER BY 1, 4
                            ";

            if (!bWithPostedUnposted) // FOR VIEWING
                query = @"SELECT CASE WHEN '{5}' = 'HSA' THEN Tin_IncomeCode + ' (' + Tin_OrigPayCycle + ')' 
                                    ELSE Tin_IncomeCode END as [Code]
                                    , Tin_IDNo as  [IDNo]
                                    , Tin_PayCycle as [Pay Cycle Code]
                                    , Tin_OrigPayCycle as [Orig Pay Cycle Code]
	                                , CASE WHEN Min_Taxclass = 'T' THEN 'TAXABLE' ELSE 'NONTAXABLE' END as [Tax Class]
                                    , Tin_IncomeAmt as [Amount]
                                    , Tin_OrigPayCycle AS [Original Pay Cycle]
                                FROM {4}
                                INNER JOIN {3}..M_Income on Min_incomecode = Tin_incomecode
	                                AND Min_CompanyCode = '{2}'
	                                AND Min_IncomeGroup = '{5}'
                                WHERE Tin_PayCycle =  '{0}'
                                    {1}
                                    AND Tin_PostFlag = 1
                                ORDER BY 1, 4
                            ";

            #endregion
            if (bIsIndividual == true)
                query = string.Format(query
                                    , PayCycle
                                    , string.Format("AND Tin_IDNo = '{0}'", IDNumber)
                                    , companyCode
                                    , centralProfile
                                    , table
                                    , IncomeGroup
                                    );
            else
                query = string.Format(query
                                    , PayCycle
                                    , string.Format("AND Tin_IDNo IN ({0})", IDNumber)
                                    , companyCode
                                    , centralProfile
                                    , table
                                    , IncomeGroup
                                    );

            DataTable dtResult = null;
            dtResult = dalHelper.ExecuteDataSet(query).Tables[0];

            return dtResult;
        }

        public DataTable ConvertTaxRefundPayable(DataTable tbl, string PayCycle)
        {
            var tblPivot = new DataTable();
            tblPivot.Columns.Add("IDNo", typeof(string));
            tblPivot.Columns.Add("PayCycle", typeof(string));
            tblPivot.Columns.Add("Item", typeof(string));
            tblPivot.Columns.Add("Amount", typeof(decimal));

            for (int i = 0; i < tbl.Rows.Count; i++)
            {
                for (int col = 1; col < tbl.Columns.Count; col++)
                {
                    DataRow dr = tblPivot.NewRow();
                    dr[0] = GetValue(tbl.Rows[i][0]);
                    dr[1] = PayCycle;
                    dr[2] = tbl.Columns[col].ColumnName.ToString();
                    dr[3] = (GetValue(tbl.Rows[i][col]).Equals(string.Empty) ? 0 : tbl.Rows[i][col]);
                    tblPivot.Rows.Add(dr);
                }
            }
            return tblPivot;
        }

        public DataTable GetEmployerShareAndECFund(string IDNumber, string PayCycle, string MaxSSSPremPayCycle, string companyCode, string centralProfile, DALHelper dalHelper)
        {
            string EmployeeCondition = " AND DTL.Tdd_IDNo = '" + IDNumber + "' ";
            string query = string.Format(@"SELECT DTL.Tdd_IDNo AS [IDNo]
	                                            , DO.Mdn_DeductionGroup AS [DeductionGroup]
	                                            , ISNULL(SUM(CASE WHEN DO.Mdn_DeductionGroup = 'SA' THEN SSS.Mps_ERShare
		                                            WHEN DO.Mdn_DeductionGroup = 'MA' THEN MPF.Mps_MPFERShare
		                                            WHEN DO.Mdn_DeductionGroup = 'PA' THEN DTL.Tdd_Amount
		                                            WHEN DO.Mdn_DeductionGroup = 'HA' AND DTL.Tdd_Amount >= 100 THEN 100.00
		                                            WHEN DO.Mdn_DeductionGroup = 'HA' AND DTL.Tdd_Amount < 100 THEN DTL.Tdd_Amount
		                                            ELSE 0 END),0) AS [ERShare]
	                                            , ISNULL(SUM(CASE WHEN DO.Mdn_DeductionGroup = 'SA' THEN SSS.Mps_ECShare
	                                            ELSE 0 END), 0) AS [ECFund]
                                            FROM T_EmpDeductionDtlFinalPay DTL
                                            INNER JOIN {1}..M_Deduction DO ON DO.Mdn_DeductionCode = DTL.Tdd_DeductionCode
	                                            AND DO.Mdn_CompanyCode = '{2}'
	                                            AND DO.Mdn_DeductionGroup IN ('SA','PA','HA','MA')
                                            LEFT JOIN (SELECT Tdd_IDNo, Tdd_StartDate, Tdd_Amount FROM T_EmpDeductionDtlFinalPay FP
			                                            INNER JOIN {1}..M_Deduction MI ON MI.Mdn_DeductionCode = FP.Tdd_DeductionCode
				                                            AND MI.Mdn_CompanyCode = '{2}'
				                                            AND MI.Mdn_DeductionGroup = 'MA') MPFAmt ON MPFAmt.Tdd_IDNo = DTL.Tdd_IDNo
                                                            AND MPFAmt.Tdd_StartDate = DTL.Tdd_StartDate
                                            LEFT JOIN {1}..M_PremiumSchedule SSS ON SSS.Mps_PayCycle = '{4}'
	                                            AND SSS.Mps_DeductionCode = 'SSSPREM'
	                                            AND SSS.Mps_EEShare = CASE WHEN DO.Mdn_DeductionGroup = 'SA' THEN DTL.Tdd_Amount ELSE 0 END
	                                            AND SSS.Mps_MPFEEShare = CASE WHEN MPFAmt.Tdd_Amount IS NULL THEN 0 ELSE MPFAmt.Tdd_Amount END
                                            LEFT JOIN {1}..M_PremiumSchedule MPF ON MPF.Mps_PayCycle = '{4}'
	                                            AND MPF.Mps_DeductionCode = 'SSSPREM'
	                                            AND MPF.Mps_MPFEEShare = CASE WHEN DO.Mdn_DeductionGroup = 'MA' THEN DTL.Tdd_Amount ELSE 0 END
	                                            AND MPF.Mps_MPFEEShare > 0
                                            WHERE DTL.Tdd_ThisPayCycle = '{3}'
	                                            {0} 
                                            GROUP BY DTL.Tdd_IDNo
	                                            , DO.Mdn_DeductionGroup "
                                            , EmployeeCondition
                                            , centralProfile
                                            , companyCode
                                            , PayCycle
                                            , MaxSSSPremPayCycle);

            try
            {
                DataSet dsResult = dalHelper.ExecuteDataSet(query);
                return (dsResult != null && dsResult.Tables.Count > 0 ? dsResult.Tables[0] : null);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public DataTable GetEmployeeFinalPay(bool bIndividual, string IDNumber, string PayCycle, string companyCode, string centralProfile, string CCTRDSPLY, DALHelper dalHelper)
        {
            string condition = string.Empty;
            if (bIndividual)
                condition = string.Format("AND Tef_IDNo = '{0}'", IDNumber);

            string FPREGPAYRULEFORMULA = new CommonBL().GetParameterFormulaFromPayroll("FPREGPAYRULE", companyCode, dalHelper);
            string FPFIXALLOWRULEFORMULA = new CommonBL().GetParameterFormulaFromPayroll("FPFIXALLOWRULE", companyCode, dalHelper);
            string FPTAXMETHODFORMULA = new CommonBL().GetParameterFormulaFromPayroll("FPTAXMETHOD", companyCode, dalHelper);

            string query = string.Format(@"SELECT Tef_IDNo as 'ID Number'
		                                            , Mem_LastName + CASE WHEN LEN(ISNULL(Mem_ExtensionName,'')) > 0 
							                                THEN  ' ' + Mem_ExtensionName ELSE '' END as 'Last Name'
		                                            , Mem_FirstName as 'First Name'
		                                            , Mem_MiddleName as 'Middle Name'
                                                    , Mem_BirthDate as [Birth Date]
		                                            , Mem_Age as [Age]
		                                            , Mem_IntakeDate as [Intake Date]
                                                    , Mem_RegularDate as [Regular Date]
                                                    , Tef_TenureYears as [Tenure Years]
		                                            , Mem_SeparationDate as [Separation Date]
                                                    , Mem_ClearedDate as [Cleared Date]
                                                    , SEP_EXT.Mcd_Name AS [Separation Reason]
		                                            , EMPSTATN.Mcd_Name as [Employment Status]
		                                            , {2}.dbo.Udf_DisplayCostCenterName('{1}',Mem_CostcenterCode,'{3}') as [Cost Center]
                                                    , Tef_Compute13thMonthPay as [Compute 13th Month Pay]
                                                    , Tef_ComputeLeaveRefund as [Compute Leave Refund]
                                                    , Tef_ComputeFixAllowance as [Compute Fix Allowance]
                                                    , Tef_ComputeHoldSalary as [Compute Hold Salary]
                                                    , Tef_ComputeRetirement as [Compute Retirement]
                                                    , Tef_RetirementSeparateFromLastPay as [Compute Retirement Separately]
                                                    , Tef_LastPayPriorSeparation as [Last Normal Pay Cycle]
                                                    , Tef_PayCycleSeparation as [Separation Pay Cycle]   
                                                    , REGPAYRULE.Code as [Regular Pay Rule Code]  ---M_PolicyHdr
                                                    , REGPAYRULE.Name as [Regular Pay Rule]  ---M_PolicyHdr
                                                    , Tef_ZeroOutDeductionCodes 
                                                    , Tef_HoldPayCycles
                                                    , FIXALLOWRULE.Code as [Fix Allowance Rule Code]  ---M_PolicyHdr
                                                    , FIXALLOWRULE.Name as [Fix Allowance Rule] ---M_PolicyHdr
                                                    , Tef_FixAllowanceNumVal1 as [Value 1]
                                                    , Tef_FixAllowanceNumVal2 as [Value 2]
                                                    , Tef_13thMonthPayStart as [13th Month Pay Start]
                                                    , Tef_13thMonthPayEnd as [13th Month Pay End]
                                                    , Tef_LeaveTypeYear
                                                    , Tef_RetirementType
                                                    , RETIRETYPE.Mpd_SubName AS [Retirement Type]
                                                    , Tef_OtherRetirementTaxableAmt AS [Other Retirement Tax Amount]
                                                    , Tef_OtherRetirementNontaxableAmt AS [Other Retirement Nontax Amount]
                                                    , TAXMETHOD.Code as [Tax Method Code]  ---M_PolicyHdr
                                                    , TAXMETHOD.Name as [Tax Method] ---M_PolicyHdr
                                                    , TAXSCHED.Mcd_Name as [Tax Schedule]
                                                    , Tef_PayERShare as [Pay Employer Share]
                                                    , Mem_PayrollType
                                            FROM Udv_FinalPay
                                            INNER JOIN M_Employee ON Tef_IDNo = Mem_IDNo
                                            LEFT JOIN T_PaySchedule ON Tef_PayCycleSeparation = Tps_PayCycle
										    LEFT JOIN M_PolicyDtl RETIRETYPE ON RETIRETYPE.Mpd_SubCode = Tef_RetirementType
											    AND RETIRETYPE.Mpd_PolicyCode = 'FPRETIREMENTCODE' 
											    AND RETIRETYPE.Mpd_CompanyCode = '{1}'
                                            LEFT JOIN {2}..M_CodeDtl EMPSTATN ON EMPSTATN.Mcd_Code = Mem_EmploymentStatusCode
	                                            AND EMPSTATN.Mcd_CodeType='EMPSTAT'
                                                AND EMPSTATN.Mcd_CompanyCode = '{1}'
                                            LEFT JOIN {2}..M_CodeDtl SEP_EXT ON SEP_EXT.Mcd_Code = Tef_SeparationCodeExternal
	                                            AND SEP_EXT.Mcd_CodeType='SEP_EXT'
                                                AND SEP_EXT.Mcd_CompanyCode = '{1}'
                                            LEFT JOIN {2}..M_CodeDtl TAXSCHED ON TAXSCHED.Mcd_Code = Tps_TaxSchedule						
	                                            AND TAXSCHED.Mcd_CodeType = 'TAXSCHED'					
	                                            AND TAXSCHED.Mcd_CompanyCode = '{1}'	
                                            LEFT JOIN 
                                            (
                                                {4}
                                            ) REGPAYRULE ON REGPAYRULE.Code = Tef_RegularPayRule
                                            LEFT JOIN 
                                            (
                                                {5}
                                            ) FIXALLOWRULE ON FIXALLOWRULE.Code = Tef_FixAllowanceRule
                                            LEFT JOIN 
                                            (
                                                {6}
                                            ) TAXMETHOD ON TAXMETHOD.Code = Tef_TaxMethod
                                            WHERE Tef_PayCycle = '{0}'
                                                {7}
                                            ORDER BY Mem_LastName, Mem_FirstName"
                                            , PayCycle
                                            , companyCode
                                            , centralProfile
                                            , CCTRDSPLY
                                            , FPREGPAYRULEFORMULA
                                            , FPFIXALLOWRULEFORMULA
                                            , FPTAXMETHODFORMULA
                                            , condition);

            try
            {
                DataSet dsResult = dalHelper.ExecuteDataSet(query);
                return (dsResult != null && dsResult.Tables.Count > 0 ? dsResult.Tables[0] : null);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
