using System;
using System.Collections.Generic;
using System.Data;
using CommonLibrary;
using Payroll.DAL;
using System.Configuration;

namespace Payroll.BLogic
{
    public class NewPayrollCalculationBL : BaseBL
    {
        #region <Global Variables>
        DALHelper dal;
        SystemCycleProcessingBL SystemCycleProcessingBL;
        CommonBL commonBL;

        //Storage tables000
        DataTable dtEmpPayroll                  = null;
        DataTable dtEmpPayrollMisc              = null;
        DataTable dtEmpPayrollDtl               = null;
        DataTable dtEmpPayrollDtlMisc           = null;
        DataTable dtEmpPayroll2                 = null;
        DataTable dtEmpPayrollYearly            = null;
        DataTable dtEmpPayrollYearly1stAnd2ndQuin = null;
        DataTable dtEmpPayrollYearly1stAnd2ndQuinTaxableOnly = null;
        DataTable dtEmpPayroll2Yearly           = null;
        DataTable dtEmpPayrollMisc2Yearly_1stAnd2ndQuin = null;
        DataTable dtEmpPayTranHdr               = null;
        DataTable dtEmpPayTranHdrMisc           = null;
        DataTable dtEmpPayTranDtl               = null;
        DataTable dtEmpPayTranDtlMisc           = null;
        DataTable dtPayrollError                = null;
        DataTable dtPremiumContribution         = null;
        DataSet dsAllYTDAdjustment;
        DataRow drEmpPayroll                    = null;
        DataRow drEmpPayrollMisc                = null;
        DataRow drEmpPayrollDtl                 = null;
        DataRow drEmpPayrollDtlMisc             = null;
        DataRow drEmpPayroll2                   = null;

        //Flags and parameters
        //I used decimal for more precision during computation
        public decimal MDIVISOR                 = 0;
        public decimal NETPCT                   = 0;
        public decimal MAXREGPAY                = 0;
        public decimal MAXTAXINC                = 0;
        public decimal MAXDEDN                  = 0;
        public decimal MAXNETPAY                = 0;
        public decimal MINNETPAY                = 0;
        public decimal HRLYRTEDEC               = 0;
        public decimal M13EXCLTAX               = 0;
        public string MULTSAL                   = "";
        public string NEGTAXINCOME              = "";
        //public string LHDPLUSBSC = "FALSE";
        //public string NWHREDLYPD = ""; Change to NEWHIRE under REGPAYRULE in M_PolicyDtl
        //public string HRRTINCDEM = ""; Change to PAYSPECIALRATE
        public string PAYSPECIALRATE            = "";
        public string SPECIALRATEFORMULA_SRATE  = "";
        public string SPECIALRATEFORMULA_HRATE  = "";
        public DataTable PAYCODERATE            = null;
        public string TXANNLQUINFORMULA1        = "";
        public string TXANNLQUINFORMULA2        = "";
        public int NDBRCKTCNT                   = 1;
        public decimal NP1_RATE                 = 0;
        public decimal NP2_RATE                 = 0;
        public string NETBASE                   = "";
        public DataTable LATEBRCKTQ             = null;
        public bool ISLASTCYCLE                 = false;
        public bool UNIONDUES                   = false;
        public bool HDMFEARLYCYCLEFULLDEDN      = false;
        public bool PHEARLYCYCLEFULLDEDN        = false;
        public bool SSSEARLYCYCLEFULLDEDN       = false;
        public bool UNIONEARLYCYCLEFULLDEDN     = false;
        public bool HDMFREFERPREVINCOMEPREM     = false;
        public bool PHREFERPREVINCOMEPREM       = false;
        public bool SSSREFERPREVINCOMEPREM      = false;
        public bool UNIONREFERPREVINCOMEPREM    = false;
        public bool REFERPREVINCOMETAX          = false;
        public bool ASSUMERECURRINGALLOWANCE    = false;
        public string HDMFREMMAXSHARE           = "";
        //public string PHREMMAXSHARE             = "";
        //public string SSSREMMAXSHARE            = "";
        public string PAYFREQNCY                = "";
        public int PAYFREQNCYCOUNT              = 0;
        public string TAXRULE                   = "";
        public string MULTSALDM                 = "";
        public string MULTSALMD                 = "";
        public string NEWHIRE                   = "";
        public string CompanyCode               = "";
        public string CentralProfile            = "";
        public string MenuCode                  = "";
        public string BIREMPSTATTOPROCESS       = "";

        #region Pay Code Rate Variables
        string LTRate                           = "N";
        string UTRate                           = "N";
        string UPLVRate                         = "N";
        string WDABSRate                        = "N";
        string LTUTMAXRate                      = "N";
        string ABSLEGHOLRate                    = "N";
        string ABSSPLHOLRate                    = "N";
        string ABSCOMPHOLRate                   = "N";
        string ABSPSDRate                       = "N";
        string ABSOTHHOLRate                    = "N";

        string REGRate                          = "N";
        string PDLVRate                         = "N";
        string PDLEGHOLRate                     = "N";
        string PDSPLHOLRate                     = "N";
        string PDCOMPHOLRate                    = "N";
        string PDPSDRate                        = "N";
        string PDOTHHOLRate                     = "N";
        string PDRESTLEGHOLRate                 = "N";

        string RESTRate                         = "N";
        string LEGHOLRate                       = "N";
        string RESTLEGHOLRate                   = "N";
        string SPLHOLRate                       = "N";
        string RESTSPLHOLRate                   = "N";
        string PSDRate                          = "N";
        string RESTPSDRate                      = "N";
        string COMPHOLRate                      = "N";
        string RESTCOMPHOLRate                  = "N";

        string Misc1Rate                        = "N";
        string Misc2Rate                        = "N";
        string Misc3Rate                        = "N";
        string Misc4Rate                        = "N";
        string Misc5Rate                        = "N";
        string Misc6Rate                        = "N";
        #endregion

        //Constants
        public int FILLERCNT = 6;

        //Payroll Error Report structure
        struct structPayrollErrorReport
        {
            public string strEmployeeId;
            public string strLastName;
            public string strFirstName;
            public string strPayCycle;
            public decimal dAmount;
            public decimal dAmount2;
            public string strRemarks;

            public structPayrollErrorReport(string EmployeeId, string LastName, string FirstName, string PayCycle, decimal Amount, decimal Amount2, string Remarks)
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
        List<structPayrollErrorReport> listPayrollRept = new List<structPayrollErrorReport>();

        //Miscellaneous
        string ProcessPayrollPeriod             = "";
        string PayrollStart                     = "";
        string PayrollEnd                       = "";
        string PayrollYearMonth                 = "";
        string PayrollCycle                     = "";
        string PayrollAssume13thMonth           = "";
        
        //bool bIsCurrent = true;

        //Payroll Configuration
        bool bComputeSSS = false;
        bool bComputePH = false;
        bool bComputeHDMF = false;
        bool bComputeTax = false;
        bool bComputeUnion = false;
        bool bMonthEnd = false;

        string LoginUser                        = "";
        //string EmpTimeRegisterTable             = "T_EmpTimeRegister";
        string EmpPayTranHdrTable               = "T_EmpPayTranHdr";
        string EmpPayTranHdrMiscTable           = "T_EmpPayTranHdrMisc";
        string EmpPayTranDtlTable               = "T_EmpPayTranDtl";
        string EmpPayTranDtlMiscTable           = "T_EmpPayTranDtlMisc";
        string EmpPayrollTable                  = "T_EmpPayroll";
        string EmpPayrollMiscTable              = "T_EmpPayrollMisc";
        string EmpPayroll2Table                 = "T_EmpPayroll2";
        string EmpPayrollYearlyTable            = "T_EmpPayrollYearly";
        string EmpPayrollYearly2Table           = "T_EmpPayroll2Yearly";
        string EmpDeductionDtlTable             = "T_EmpDeductionDtl";
        //string EmpDeductionDtlHstTable          = "T_EmpDeductionDtlHst";
        string EmpDeductionHdrCycleTable        = "T_EmpDeductionHdrCycle";
        string EmpIncomeTable                   = "T_EmpIncome";
        string EmpIncomeHstTable                = "T_EmpIncomeHst";
        //New
        string EmpPayrollDtlTable               = "T_EmpPayrollDtl";
        string EmpPayrollDtlMiscTable           = "T_EmpPayrollDtlMisc";


        string TaxSchedule                      = "";
        string MaxSSSPremPayCycle               = "";
        string MaxPhilHealthPremPayCycle        = "";
        string MaxHDMFPremPayCycle              = "";
        string MaxUNIONDUESPremPayCycle         = "";
        string SSSBaseMaxShare                  = "";
        string PhilHealthBaseMaxShare           = "";
        //string HDMFBaseMaxShare                 = "";
        string UNIONDUESBaseMaxShare            = "";
        string MaxTaxHeader                     = "";
        string MaxTaxAnnual                     = "";
        string MaxTaxExemption                  = "";
        string ApplicPHPrem                     = "";
        string ApplicSSSPrem                    = "";
        string ApplicHDMFPrem                   = "";
        string ApplicUnionPrem                  = "";
        string ApplicTax                        = "";
        decimal TaxExemption1                   = 0;
        decimal TaxExemption2                   = 0;
        decimal TaxExemption3                   = 0;
        decimal TaxExemption4                   = 0;

        string EmployeeList = string.Empty;

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

        #region Event handlers for payroll calculation process
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

        public NewPayrollCalculationBL()
        {
        }

        //For Final Pay
        public DataTable ComputePayroll(string PayRule, string PayrollPeriod, string EmployeeId, string UserLogin, string companyCode, string centralProfile, string menucode, DALHelper dalHelper)
        {
            return ComputePayroll(false, true, true, PayRule, PayrollPeriod, EmployeeId, UserLogin, companyCode, centralProfile, menucode, "", dalHelper);
        }

        public DataTable ComputePayroll(bool ProcessSeparated, bool Annualize, string PayRule, string PayrollPeriod, string EmployeeId, string UserLogin, string companyCode, string centralProfile, string menucode, string assume13thmonth, DALHelper dalHelper)
        {
            return ComputePayroll(false, ProcessSeparated, Annualize, PayRule, PayrollPeriod, EmployeeId, UserLogin, companyCode, centralProfile, menucode, assume13thmonth, dalHelper);
        }

        public DataTable ComputePayroll(bool ProcessSeparated, bool Annualize, string PayRule, string PayrollPeriod, string UserLogin, string companyCode, string centralProfile, string menucode, string assume13thmonth, DALHelper dalHelper, string EmployeeList)
        {
            this.EmployeeList = EmployeeList;
            return ComputePayroll(true, ProcessSeparated, Annualize, PayRule, PayrollPeriod, "", UserLogin, companyCode, centralProfile, menucode, assume13thmonth, dalHelper);
        }

        public DataTable ComputePayroll(bool ProcessAll, bool ProcessSeparated, bool Annualize, string PayRule, string PayrollPeriod, string EmployeeId, string UserLogin, string companyCode, string centralProfile, string menucode, string Assume13thMonth, DALHelper dalHelper)
        {
            #region Variables
            string PayrollProcessingErrorMessage        = "";
            CompanyCode                                 = companyCode;
            CentralProfile                              = centralProfile;
            MenuCode                                    = menucode;

            bool bHasDayCodeExt                         = false;
            DataTable dtDayCodePremiums                 = null;
            DataTable dtDayCodePremiumFillers           = null;
            //DataRow[] drDayCodePremium                  = null;
            DataRow[] drDayCodePremiumFiller            = null;
            DataRow[] drPremiumContribution             = null;

            string PayrollType                          = "";
            string PremiumGroup                         = "";
            string EmploymentStatus                     = "";
            decimal SalaryRate                          = 0;
            decimal HourlyRate                          = 0;
            decimal SpecialSalaryRate                   = 0;
            decimal SpecialHourlyRate                   = 0;

            decimal GrossPayAmt                         = 0;
            decimal RegularPayAmt                       = 0;
            decimal TotalOTNDAmt                        = 0;
            decimal GrossIncomeAmt                      = 0;
            string curEmployeeId                        = "";
            string strRegRule                           = "";

            string strHDMFRule                          = "";
            decimal dHDMFEmployeeShare                  = 0;
            decimal dHDMFEmployerShare                  = 0;
            decimal dMonthlyHDMFEmployeeShare           = 0;
            decimal dMonthlyHDMFEmployeeShareNonTax     = 0;
            decimal dMonthlyHDMFEmployerShare           = 0;
            decimal dHDMFMaxAmt                         = 0;
            decimal dHDMFERAmt                          = 0; 
            decimal dServiceYear                        = 0;

            string strSSSRule                           = "";
            decimal dBaseSalaryAmt                      = 0;
            decimal dMonthlyBaseSalaryAmt               = 0;
            decimal dSSSEmployeeShare                   = 0;
            decimal dSSSEmployerShare                   = 0;
            decimal dECFundAmt                          = 0;
            decimal dMonthlySSSEmployeeShare            = 0;
            decimal dMonthlySSSEmployerShare            = 0;
            decimal dMonthlyECFundAmt                   = 0;
            decimal amount;

            decimal dMPFEmployeeShare                   = 0;
            decimal dMPFEmployerShare                   = 0;
            decimal dMonthlyMPFEmployeeShare            = 0;
            decimal dMonthlyMPFEmployerShare            = 0;

            string strPHRule                            = "";
            decimal dPHEmployeeShare                    = 0;
            decimal dPHEmployerShare                    = 0;
            decimal dMonthlyPHEmployeeShare             = 0;
            decimal dMonthlyPHEmployerShare             = 0;

            string strUNIONRule                         = "";
            decimal dUNIONEmployeeShare                 = 0;
            decimal dMonthlyUNIONEmployeeShare          = 0;

            string strTaxRule                           = "";
            decimal dWTaxAmt                            = 0;
            decimal dWTaxRef                            = 0;
            decimal dPartialDeduction                   = 0;
            decimal dCurrentNetPay                      = 0;
            decimal dMinimumNetPay                      = 0;

            decimal dOtherDeduction                     = 0;
            DataSet dsDeductions;
            decimal dTotalDeduction                     = 0;
            decimal dNetPayAmt                          = 0;

            DataTable dtComputedDailyEmployees          = null;
            DataTable dtNewlyHiredEmployees             = null;
            DataTable dtEmployeeSalary                  = null;

            DataTable dtErrList                         = new DataTable();
            DataRow[] drArrEmpComputedDailyEmployees    = null;
            DataRow[] drArrEmpNewlyHiredEmployees       = null;
            DataRow[] drArrEmployeeSalary               = null;
            DataRow[] drArrEmployeePayrollTransExt      = null;
            DataRow[] drArrEmpPayrollYearly             = null;
            DataRow[] drArrEmpPayrollMisc2Yearly        = null;

            DataRow[] drArrEmpPayrollDtl                = null;
            DataRow[] drArrEmpPayrollDtlMisc            = null;

            decimal dTempNetPay                         = 0;
            bool bMonthlyToDailyPayrollType             = false;
            //DataSet dsBasicPay;
            //decimal dPrevBasicPay;

            #region Premium Group Variables
            //Regular and Overtime Rates
            decimal Reg                     = 0;
            decimal RegOT                   = 0;
            decimal Rest                    = 0;
            decimal RestOT                  = 0;
            decimal Hol                     = 0;
            decimal HolOT                   = 0;
            decimal SPL                     = 0;
            decimal SPLOT                   = 0;
            decimal PSD                     = 0;
            decimal PSDOT                   = 0;
            decimal Comp                    = 0;
            decimal CompOT                  = 0;
            decimal RestHol                 = 0;
            decimal RestHolOT               = 0;
            decimal RestSPL                 = 0;
            decimal RestSPLOT               = 0;
            decimal RestComp                = 0;
            decimal RestCompOT              = 0;
            decimal RestPSD                 = 0;
            decimal RestPSDOT               = 0;

            //Regular Night Premium and Overtime Night Premium Rates
            decimal RegND                   = 0;
            decimal RegOTND                 = 0;
            decimal RestND                  = 0;
            decimal RestOTND                = 0;
            decimal HolND                   = 0;
            decimal HolOTND                 = 0;
            decimal SPLND                   = 0;
            decimal SPLOTND                 = 0;
            decimal PSDND                   = 0;
            decimal PSDOTND                 = 0;
            decimal CompND                  = 0;
            decimal CompOTND                = 0;
            decimal RestHolND               = 0;
            decimal RestHolOTND             = 0;
            decimal RestSPLND               = 0;
            decimal RestSPLOTND             = 0;
            decimal RestCompND              = 0;
            decimal RestCompOTND            = 0;
            decimal RestPSDND               = 0;
            decimal RestPSDOTND             = 0;

            //Regular Night Premium and Overtime Night Premium Percentages
            decimal RegNDPercent            = 0;
            decimal RegOTNDPercent          = 0;
            decimal RestNDPercent           = 0;
            decimal RestOTNDPercent         = 0;
            decimal HolNDPercent            = 0;
            decimal HolOTNDPercent          = 0;
            decimal SPLNDPercent            = 0;
            decimal SPLOTNDPercent          = 0;
            decimal PSDNDPercent            = 0;
            decimal PSDOTNDPercent          = 0;
            decimal CompNDPercent           = 0;
            decimal CompOTNDPercent         = 0;
            decimal RestHolNDPercent        = 0;
            decimal RestHolOTNDPercent      = 0;
            decimal RestSPLNDPercent        = 0;
            decimal RestSPLOTNDPercent      = 0;
            decimal RestCompNDPercent       = 0;
            decimal RestCompOTNDPercent     = 0;
            decimal RestPSDNDPercent        = 0;
            decimal RestPSDOTNDPercent      = 0;
            #endregion

            #region MultSal Variables
            int WorkingDayCnt                           = 0;
            int WorkingDayCntUsingNewRate               = 0;
            decimal OldDailyRate                        = 0;
            decimal NewDailyRate                        = 0;
            #endregion

            #region Payroll Calc Variables
            decimal LateAmt                             = 0;
            decimal UndertimeAmt                        = 0;
            decimal UnpaidLeaveAmt                      = 0;
            decimal AbsentLegalHolidayAmt               = 0;
            decimal AbsentSpecialHolidayAmt             = 0;
            decimal AbsentCompanyHolidayAmt             = 0;
            decimal AbsentPlantShutdownAmt              = 0;
            decimal AbsentFillerHolidayAmt              = 0;
            decimal WholeDayAbsentAmt                   = 0;
            decimal LateUndertimeMaxAbsentAmt           = 0;
            decimal AbsentAmt                           = 0;

            decimal RegularAmt                          = 0;
            decimal PaidLeaveAmt                        = 0;
            decimal PaidLegalHolidayAmt                 = 0;
            decimal PaidSpecialHolidayAmt               = 0;
            decimal PaidCompanyHolidayAmt               = 0;
            decimal PaidFillerHolidayAmt                = 0;
            decimal PaidPlantShutdownHolidayAmt         = 0;
            decimal PaidRestdayLegalHolidayAmt          = 0;

            decimal RegularOTAmt                        = 0;
            decimal RegularNDAmt                        = 0;
            decimal RegularOTNDAmt                      = 0;
            decimal RestdayAmt                          = 0;
            decimal RestdayOTAmt                        = 0;
            decimal RestdayNDAmt                        = 0;
            decimal RestdayOTNDAmt                      = 0;
            decimal LegalHolidayAmt                     = 0;
            decimal LegalHolidayOTAmt                   = 0;
            decimal LegalHolidayNDAmt                   = 0;
            decimal LegalHolidayOTNDAmt                 = 0;
            decimal SpecialHolidayAmt                   = 0;
            decimal SpecialHolidayOTAmt                 = 0;
            decimal SpecialHolidayNDAmt                 = 0;
            decimal SpecialHolidayOTNDAmt               = 0;
            decimal PlantShutdownAmt                    = 0;
            decimal PlantShutdownOTAmt                  = 0;
            decimal PlantShutdownNDAmt                  = 0;
            decimal PlantShutdownOTNDAmt                = 0;
            decimal CompanyHolidayAmt                   = 0;
            decimal CompanyHolidayOTAmt                 = 0;
            decimal CompanyHolidayNDAmt                 = 0;
            decimal CompanyHolidayOTNDAmt               = 0;
            decimal RestdayLegalHolidayAmt              = 0;
            decimal RestdayLegalHolidayOTAmt            = 0;
            decimal RestdayLegalHolidayNDAmt            = 0;
            decimal RestdayLegalHolidayOTNDAmt          = 0;
            decimal RestdaySpecialHolidayAmt            = 0;
            decimal RestdaySpecialHolidayOTAmt          = 0;
            decimal RestdaySpecialHolidayNDAmt          = 0;
            decimal RestdaySpecialHolidayOTNDAmt        = 0;
            decimal RestdayCompanyHolidayAmt            = 0;
            decimal RestdayCompanyHolidayOTAmt          = 0;
            decimal RestdayCompanyHolidayNDAmt          = 0;
            decimal RestdayCompanyHolidayOTNDAmt        = 0;
            decimal RestdayPlantShutdownAmt             = 0;
            decimal RestdayPlantShutdownOTAmt           = 0;
            decimal RestdayPlantShutdownNDAmt           = 0;
            decimal RestdayPlantShutdownOTNDAmt         = 0;
            #endregion

            #region Payroll Calc Ext Variables
            string fillerHrCol                          = "";
            string fillerOTHrCol                        = "";
            string fillerNDHrCol                        = "";
            string fillerOTNDHrCol                      = "";
            string fillerAmtCol                         = "";
            string fillerOTAmtCol                       = "";
            string fillerNDAmtCol                       = "";
            string fillerOTNDAmtCol                     = "";

            decimal Misc1Amt                            = 0;
            decimal Misc1OTAmt                          = 0;
            decimal Misc1NDAmt                          = 0;
            decimal Misc1NDOTAmt                        = 0;
            decimal Misc2Amt                            = 0;
            decimal Misc2OTAmt                          = 0;
            decimal Misc2NDAmt                          = 0;
            decimal Misc2NDOTAmt                        = 0;
            decimal Misc3Amt                            = 0;
            decimal Misc3OTAmt                          = 0;
            decimal Misc3NDAmt                           = 0;
            decimal Misc3NDOTAmt                        = 0;
            decimal Misc4Amt                            = 0;
            decimal Misc4OTAmt                          = 0;
            decimal Misc4NDAmt                          = 0;
            decimal Misc4NDOTAmt                        = 0;
            decimal Misc5Amt                            = 0;
            decimal Misc5OTAmt                          = 0;
            decimal Misc5NDAmt                          = 0;
            decimal Misc5NDOTAmt                        = 0;
            decimal Misc6Amt                            = 0;
            decimal Misc6OTAmt                          = 0;
            decimal Misc6NDAmt                          = 0;
            decimal Misc6NDOTAmt                        = 0;

            #endregion

            #endregion

            try
            {
                #region Initial Setup
                this.dal = dalHelper;
                SystemCycleProcessingBL = new SystemCycleProcessingBL(dal, PayrollPeriod, UserLogin, CompanyCode,  CentralProfile);
                commonBL = new CommonBL();
                //-----------------------------
                //Check for Existing Day Codes
                if (commonBL.GetFillerDayCodesCount(CompanyCode, CentralProfile, dal) > 0)
                {
                    bHasDayCodeExt = true;
                    dtDayCodePremiumFillers = commonBL.GetDayCodePremiumFillers(CompanyCode, CentralProfile, dal);
                }
                dtDayCodePremiums = GetDayCodePremiums(CompanyCode, CentralProfile);
                //-----------------------------
                //SetParameters();
                InitializePayrollReport();
                //-----------------------------
                //Create and initialize payroll tables
                #region Create payroll table
                DALHelper dalTemp = new DALHelper();
                if (!ProcessSeparated)
                {
                    dtEmpPayroll                    = DbRecord.GenerateTable(CommonConstants.TableName.T_EmpPayroll, dalTemp);
                    dtEmpPayrollDtl                 = DbRecord.GenerateTable(CommonConstants.TableName.T_EmpPayrollDtl, dalTemp);
                    dtEmpPayrollMisc                = DbRecord.GenerateTable(CommonConstants.TableName.T_EmpPayrollMisc, dalTemp);
                    dtEmpPayrollDtlMisc             = DbRecord.GenerateTable(CommonConstants.TableName.T_EmpPayrollDtlMisc, dalTemp);
                    dtEmpPayroll2                   = DbRecord.GenerateTable(CommonConstants.TableName.T_EmpPayroll2, dalTemp);
                }
                else
                {
                    dtEmpPayroll                    = DbRecord.GenerateTable(CommonConstants.TableName.T_EmpPayrollFinalPay, dalTemp);
                    dtEmpPayrollDtl                 = DbRecord.GenerateTable(CommonConstants.TableName.T_EmpPayrollDtlFinalPay, dalTemp);
                    dtEmpPayrollMisc                = DbRecord.GenerateTable(CommonConstants.TableName.T_EmpPayrollMiscFinalPay, dalTemp);
                    dtEmpPayrollDtlMisc             = DbRecord.GenerateTable(CommonConstants.TableName.T_EmpPayrollDtlMiscFinalPay, dalTemp);
                    dtEmpPayroll2                   = DbRecord.GenerateTable(CommonConstants.TableName.T_EmpPayroll2FinalPay, dalTemp);
                }
                //Create Payroll Error Table
                dtPayrollError                      = DbRecord.GenerateTable(CommonConstants.TableName.T_EmpProcessCheck, dalTemp);
                #endregion
                //-----------------------------
                //dal.OpenDB();
                //dal.BeginTransactionSnapshot();
                //code start
                //-----------------------------
                ProcessPayrollPeriod = PayrollPeriod;
                PayrollAssume13thMonth = Assume13thMonth;
                LoginUser = UserLogin;

                DataTable dtPayPeriod = GetPayPeriodCycle(ProcessPayrollPeriod);
                if (dtPayPeriod.Rows.Count > 0)
                {
                    PayrollStart                    = dtPayPeriod.Rows[0]["Tps_StartCycle"].ToString();
                    PayrollEnd                      = dtPayPeriod.Rows[0]["Tps_EndCycle"].ToString();
                    PayrollYearMonth                = ProcessPayrollPeriod.Substring(0, 6);
                    PayrollCycle                    = ProcessPayrollPeriod.Substring(6, 1);
                    //if (dtPayPeriod.Rows[0]["Tps_CycleIndicator"].ToString() == "S")
                    //    bIsCurrent = (dtPayPeriod.Rows[0]["Tps_CycleIndicatorSpecial"].ToString() == "C") ? true : false;
                    //else
                    //    bIsCurrent = (dtPayPeriod.Rows[0]["Tps_CycleIndicator"].ToString() == "C") ? true : false;
                    TaxSchedule                     = dtPayPeriod.Rows[0]["Tps_TaxSchedule"].ToString();
                    bComputeHDMF                    = Convert.ToBoolean(dtPayPeriod.Rows[0]["Tps_ComputePagIbig"].ToString());
                    bComputeSSS                     = Convert.ToBoolean(dtPayPeriod.Rows[0]["Tps_ComputeSSS"].ToString());
                    bComputePH                      = Convert.ToBoolean(dtPayPeriod.Rows[0]["Tps_ComputePHIC"].ToString());
                    bComputeTax                     = Convert.ToBoolean(dtPayPeriod.Rows[0]["Tps_ComputeTax"].ToString());
                    bComputeUnion                   = Convert.ToBoolean(dtPayPeriod.Rows[0]["Tps_ComputeUnion"].ToString());
                    bMonthEnd                       = Convert.ToBoolean(dtPayPeriod.Rows[0]["Tps_MonthEnd"].ToString());
                }
                else
                    PayrollProcessingErrorMessage += "Pay Cycle not found.\n";
                //-----------------------------             
                PayrollProcessingErrorMessage += GetPayrollParameters(PayrollPeriod, TaxSchedule, Annualize);
                ISLASTCYCLE = IsLastQuincena(ProcessPayrollPeriod);
                //-----------------------------
                //Check for errors
                if (PayrollProcessingErrorMessage != "")
                    throw new Exception(PayrollProcessingErrorMessage);
                //-----------------------------
                #region Get Payroll-related Table Names
                EmpPayrollTable                     = "T_EmpPayroll";
                EmpPayrollMiscTable                 = "T_EmpPayrollMisc";
                EmpPayroll2Table                    = "T_EmpPayroll2";
                EmpPayrollYearlyTable               = "T_EmpPayrollYearly";
                EmpPayrollYearly2Table              = "T_EmpPayroll2Yearly";
                EmpDeductionDtlTable                = "T_EmpDeductionDtl";

                EmpPayrollDtlTable                  = "T_EmpPayrollDtl";
                EmpPayrollDtlMiscTable              = "T_EmpPayrollDtlMisc";
                //EmpPayrollDtlYearlyTable            = "T_EmpPayrollDtlYearly";

                if (!ProcessSeparated)
                {
                    EmpPayTranHdrTable              = "T_EmpPayTranHdr";
                    EmpPayTranHdrMiscTable          = "T_EmpPayTranHdrMisc";
                    EmpPayTranDtlTable              = "T_EmpPayTranDtl";
                    EmpPayTranDtlMiscTable          = "T_EmpPayTranDtlMisc";
                }
                else
                {
                    EmpPayrollTable                 = "T_EmpPayrollFinalPay";
                    EmpPayrollMiscTable             = "T_EmpPayrollMiscFinalPay";
                    EmpPayroll2Table                = "T_EmpPayroll2FinalPay";
                    EmpPayTranHdrTable              = "T_EmpPayTranHdrFinalPay";
                    EmpPayTranHdrMiscTable          = "T_EmpPayTranHdrMiscFinalPay";
                    EmpPayTranDtlTable              = "T_EmpPayTranDtlFinalPay";
                    EmpPayTranDtlMiscTable          = "T_EmpPayTranDtlMiscFinalPay";
                    EmpDeductionDtlTable            = "T_EmpDeductionDtlFinalPay";
                    EmpPayrollDtlTable              = "T_EmpPayrollDtlFinalPay";
                    EmpPayrollDtlMiscTable          = "T_EmpPayrollDtlMiscFinalPay";

                    EmpPayrollDtlTable              = "T_EmpPayrollDtlFinalPay";
                    EmpPayrollDtlMiscTable          = "T_EmpPayrollDtlMiscFinalPay";
                }

                //if (bIsCurrent)
                //    EmpTimeRegisterTable            = "T_EmpTimeRegister";
                //else
                //    EmpTimeRegisterTable            = "T_EmpTimeRegisterHst";
                #endregion
                //-----------------------------
                StatusHandler(this, new StatusEventArgs("Cleaning Payroll and Other Transaction Tables", false));
                ClearPayrollTables(ProcessAll, ProcessSeparated, EmployeeId, ProcessPayrollPeriod);
                StatusHandler(this, new StatusEventArgs("Cleaning Payroll and Other Transaction Tables", true));
                //-----------------------------
                StatusHandler(this, new StatusEventArgs("Creating Deduction Records", false));
                if (!ProcessSeparated)
                {
                    CreateCurrentDeduction(ProcessAll, true, EmployeeId);
                    CreateCurrentDeduction(ProcessAll, false, EmployeeId); //Create Deduction Detail of Excluded Employees
                }
                    
                StatusHandler(this, new StatusEventArgs("Creating Deduction Records", true));
                //-----------------------------
                StatusHandler(this, new StatusEventArgs("Extracting Payroll Transaction Records", false));

                dtEmpPayTranHdr = GetAllEmployeePayTransForProcess(ProcessAll, ProcessSeparated, EmployeeId, ProcessPayrollPeriod);
                if (bHasDayCodeExt)
                    dtEmpPayTranHdrMisc = GetAllEmployeePayTransExtForProcess(ProcessAll, EmployeeId, ProcessPayrollPeriod);
                dtEmpPayTranDtl = GetAllEmployeePayTransDetailForProcess(ProcessAll, ProcessSeparated, EmployeeId, ProcessPayrollPeriod);
                if (bHasDayCodeExt)
                    dtEmpPayTranDtlMisc = GetAllEmployeePayTransExtDetailForProcess(ProcessAll, ProcessSeparated, EmployeeId, ProcessPayrollPeriod);

                StatusHandler(this, new StatusEventArgs("Extracting Payroll Transaction Records", true));

                StatusHandler(this, new StatusEventArgs("Extracting Payroll History Records", false));
                dtEmpPayrollYearly = GetAllEmployeePayrollAnnualRecordsForProcess(ProcessAll, EmployeeId, EmployeeList, false, true, true, EmpPayrollYearlyTable , dal);
                dtEmpPayrollYearly1stAnd2ndQuin = GetAllEmployeePayrollAnnualRecordsForProcess(ProcessAll, EmployeeId, EmployeeList, true, true, false, EmpPayrollYearlyTable , dal);
                dtEmpPayroll2Yearly = GetAllEmployeePayrollAnnualExtensionRecordsForProcess(ProcessAll, EmployeeId, EmployeeList, false, true, true, EmpPayrollYearlyTable , EmpPayrollYearly2Table, dal);
                dtEmpPayrollMisc2Yearly_1stAnd2ndQuin = GetAllEmployeePayrollAnnualExtensionRecordsForProcess(ProcessAll, EmployeeId, EmployeeList, true, true, false, EmpPayrollYearlyTable , EmpPayrollYearly2Table, dal);
                dsAllYTDAdjustment = GetYTDAdjustment("", ProcessPayrollPeriod, dal);
                StatusHandler(this, new StatusEventArgs("Extracting Payroll History Records", true));

                //StatusHandler(this, new StatusEventArgs("Get Premium Contribution Table", false));
                dtPremiumContribution = GetPremiumContributionTable();
                //StatusHandler(this, new StatusEventArgs("Get Premium Contribution Table", true));
                #endregion
                //-----------------------------START MAIN PROCESS
                if (dtEmpPayTranHdr.Rows.Count > 0)
                {
                    dtComputedDailyEmployees = GetComputedDailyinCurrentPayPeriod();
                    dtNewlyHiredEmployees = GetNewlyHiredinCurrentPayPeriod();
                    dtEmployeeSalary = GetEmployeeSalaryinCurrentPayPeriod();

                    for (int i = 0; i < dtEmpPayTranHdr.Rows.Count; i++)
                    {
                        try
                        {
                            curEmployeeId = dtEmpPayTranHdr.Rows[i]["Tph_IDNo"].ToString();

                            drEmpPayroll = dtEmpPayroll.NewRow();
                            drEmpPayroll2 = dtEmpPayroll2.NewRow();
                            if (bHasDayCodeExt)
                                drEmpPayrollMisc = dtEmpPayrollMisc.NewRow();

                            //Initialize rows
                            #region Initialize Payroll Calc Row
                            drEmpPayroll["Tpy_IDNo"]                                                = curEmployeeId;
                            drEmpPayroll["Tpy_PayCycle"]                                            = dtEmpPayTranHdr.Rows[i]["Tph_PayCycle"];
                            drEmpPayroll["Tpy_SalaryRate"]                                          = dtEmpPayTranHdr.Rows[i]["Mem_Salary"];
                            drEmpPayroll["Tpy_HourRate"]                                            = 0;
                            drEmpPayroll["Tpy_SpecialRate"]                                         = 0; //FORMULA
                            drEmpPayroll["Tpy_SpecialHourRate"]                                     = 0;
                            decimal brLateHr = GetLateDeductionFromBracket(Convert.ToDecimal(dtEmpPayTranHdr.Rows[i]["Tph_LTHr"]));
                            if (brLateHr > 0)
                                drEmpPayroll["Tpy_LTHr"] = brLateHr;
                            else 
                                drEmpPayroll["Tpy_LTHr"]                                            = dtEmpPayTranHdr.Rows[i]["Tph_LTHr"];

                            decimal brUndertimeHr = GetLateDeductionFromBracket(Convert.ToDecimal(dtEmpPayTranHdr.Rows[i]["Tph_UTHr"]));
                            if (brUndertimeHr > 0)
                                drEmpPayroll["Tpy_UTHr"] = brUndertimeHr;
                            else
                                drEmpPayroll["Tpy_UTHr"]                                            = dtEmpPayTranHdr.Rows[i]["Tph_UTHr"];

                            drEmpPayroll["Tpy_UPLVHr"]                                              = dtEmpPayTranHdr.Rows[i]["Tph_UPLVHr"];
                            drEmpPayroll["Tpy_ABSLEGHOLHr"]                                         = dtEmpPayTranHdr.Rows[i]["Tph_ABSLEGHOLHr"];
                            drEmpPayroll["Tpy_ABSSPLHOLHr"]                                         = dtEmpPayTranHdr.Rows[i]["Tph_ABSSPLHOLHr"];
                            drEmpPayroll["Tpy_ABSCOMPHOLHr"]                                        = dtEmpPayTranHdr.Rows[i]["Tph_ABSCOMPHOLHr"];
                            drEmpPayroll["Tpy_ABSPSDHr"]                                            = dtEmpPayTranHdr.Rows[i]["Tph_ABSPSDHr"];
                            drEmpPayroll["Tpy_ABSOTHHOLHr"]                                         = dtEmpPayTranHdr.Rows[i]["Tph_ABSOTHHOLHr"];
                            drEmpPayroll["Tpy_WDABSHr"]                                             = dtEmpPayTranHdr.Rows[i]["Tph_WDABSHr"];
                            drEmpPayroll["Tpy_LTUTMaxHr"]                                           = dtEmpPayTranHdr.Rows[i]["Tph_LTUTMaxHr"];
                            //Total Absent Hour
                            if ((brLateHr + brUndertimeHr) > 0)
                                drEmpPayroll["Tpy_ABSHr"]                                           = Convert.ToDecimal(drEmpPayroll["Tpy_LTHr"])
                                                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_UTHr"])
                                                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_UPLVHr"])
                                                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_WDABSHr"])
                                                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_LTUTMaxHr"]);

                            drEmpPayroll["Tpy_ABSHr"]                                               = Convert.ToDecimal(dtEmpPayTranHdr.Rows[i]["Tph_ABSHr"])
                                                                                                            + Convert.ToDecimal(dtEmpPayTranHdr.Rows[i]["Tph_ABSLEGHOLHr"])
                                                                                                            + Convert.ToDecimal(dtEmpPayTranHdr.Rows[i]["Tph_ABSSPLHOLHr"])
                                                                                                            + Convert.ToDecimal(dtEmpPayTranHdr.Rows[i]["Tph_ABSCOMPHOLHr"])
                                                                                                            + Convert.ToDecimal(dtEmpPayTranHdr.Rows[i]["Tph_ABSPSDHr"])
                                                                                                            + Convert.ToDecimal(dtEmpPayTranHdr.Rows[i]["Tph_ABSOTHHOLHr"]);

                            drEmpPayroll["Tpy_REGHr"]                                               = dtEmpPayTranHdr.Rows[i]["Tph_REGHr"];
                            drEmpPayroll["Tpy_PDLVHr"]                                              = dtEmpPayTranHdr.Rows[i]["Tph_PDLVHr"];
                            drEmpPayroll["Tpy_PDLEGHOLHr"]                                          = dtEmpPayTranHdr.Rows[i]["Tph_PDLEGHOLHr"];
                            drEmpPayroll["Tpy_PDSPLHOLHr"]                                          = dtEmpPayTranHdr.Rows[i]["Tph_PDSPLHOLHr"];
                            drEmpPayroll["Tpy_PDCOMPHOLHr"]                                         = dtEmpPayTranHdr.Rows[i]["Tph_PDCOMPHOLHr"];
                            drEmpPayroll["Tpy_PDPSDHr"]                                             = dtEmpPayTranHdr.Rows[i]["Tph_PDPSDHr"];
                            drEmpPayroll["Tpy_PDOTHHOLHr"]                                          = dtEmpPayTranHdr.Rows[i]["Tph_PDOTHHOLHr"];
                            drEmpPayroll["Tpy_PDRESTLEGHOLHr"]                                      = dtEmpPayTranHdr.Rows[i]["Tph_PDRESTLEGHOLHr"];

                            drEmpPayroll["Tpy_REGOTHr"]                                             = dtEmpPayTranHdr.Rows[i]["Tph_REGOTHr"];
                            drEmpPayroll["Tpy_REGNDHr"]                                             = dtEmpPayTranHdr.Rows[i]["Tph_REGNDHr"];
                            drEmpPayroll["Tpy_REGNDOTHr"]                                           = dtEmpPayTranHdr.Rows[i]["Tph_REGNDOTHr"];
                            drEmpPayroll["Tpy_RESTHr"]                                              = dtEmpPayTranHdr.Rows[i]["Tph_RESTHr"];
                            drEmpPayroll["Tpy_RESTOTHr"]                                            = dtEmpPayTranHdr.Rows[i]["Tph_RESTOTHr"];
                            drEmpPayroll["Tpy_RESTNDHr"]                                            = dtEmpPayTranHdr.Rows[i]["Tph_RESTNDHr"];
                            drEmpPayroll["Tpy_RESTNDOTHr"]                                          = dtEmpPayTranHdr.Rows[i]["Tph_RESTNDOTHr"];
                            drEmpPayroll["Tpy_LEGHOLHr"]                                            = dtEmpPayTranHdr.Rows[i]["Tph_LEGHOLHr"];
                            drEmpPayroll["Tpy_LEGHOLOTHr"]                                          = dtEmpPayTranHdr.Rows[i]["Tph_LEGHOLOTHr"];
                            drEmpPayroll["Tpy_LEGHOLNDHr"]                                          = dtEmpPayTranHdr.Rows[i]["Tph_LEGHOLNDHr"];
                            drEmpPayroll["Tpy_LEGHOLNDOTHr"]                                        = dtEmpPayTranHdr.Rows[i]["Tph_LEGHOLNDOTHr"];
                            drEmpPayroll["Tpy_SPLHOLHr"]                                            = dtEmpPayTranHdr.Rows[i]["Tph_SPLHOLHr"];
                            drEmpPayroll["Tpy_SPLHOLOTHr"]                                          = dtEmpPayTranHdr.Rows[i]["Tph_SPLHOLOTHr"];
                            drEmpPayroll["Tpy_SPLHOLNDHr"]                                          = dtEmpPayTranHdr.Rows[i]["Tph_SPLHOLNDHr"];
                            drEmpPayroll["Tpy_SPLHOLNDOTHr"]                                        = dtEmpPayTranHdr.Rows[i]["Tph_SPLHOLNDOTHr"];
                            drEmpPayroll["Tpy_PSDHr"]                                               = dtEmpPayTranHdr.Rows[i]["Tph_PSDHr"];
                            drEmpPayroll["Tpy_PSDOTHr"]                                             = dtEmpPayTranHdr.Rows[i]["Tph_PSDOTHr"];
                            drEmpPayroll["Tpy_PSDNDHr"]                                             = dtEmpPayTranHdr.Rows[i]["Tph_PSDNDHr"];
                            drEmpPayroll["Tpy_PSDNDOTHr"]                                           = dtEmpPayTranHdr.Rows[i]["Tph_PSDNDOTHr"];
                            drEmpPayroll["Tpy_COMPHOLHr"]                                           = dtEmpPayTranHdr.Rows[i]["Tph_COMPHOLHr"];
                            drEmpPayroll["Tpy_COMPHOLOTHr"]                                         = dtEmpPayTranHdr.Rows[i]["Tph_COMPHOLOTHr"];
                            drEmpPayroll["Tpy_COMPHOLNDHr"]                                         = dtEmpPayTranHdr.Rows[i]["Tph_COMPHOLNDHr"];
                            drEmpPayroll["Tpy_COMPHOLNDOTHr"]                                       = dtEmpPayTranHdr.Rows[i]["Tph_COMPHOLNDOTHr"];
                            drEmpPayroll["Tpy_RESTLEGHOLHr"]                                        = dtEmpPayTranHdr.Rows[i]["Tph_RESTLEGHOLHr"];
                            drEmpPayroll["Tpy_RESTLEGHOLOTHr"]                                      = dtEmpPayTranHdr.Rows[i]["Tph_RESTLEGHOLOTHr"];
                            drEmpPayroll["Tpy_RESTLEGHOLNDHr"]                                      = dtEmpPayTranHdr.Rows[i]["Tph_RESTLEGHOLNDHr"];
                            drEmpPayroll["Tpy_RESTLEGHOLNDOTHr"]                                    = dtEmpPayTranHdr.Rows[i]["Tph_RESTLEGHOLNDOTHr"];
                            drEmpPayroll["Tpy_RESTSPLHOLHr"]                                        = dtEmpPayTranHdr.Rows[i]["Tph_RESTSPLHOLHr"];
                            drEmpPayroll["Tpy_RESTSPLHOLOTHr"]                                      = dtEmpPayTranHdr.Rows[i]["Tph_RESTSPLHOLOTHr"];
                            drEmpPayroll["Tpy_RESTSPLHOLNDHr"]                                      = dtEmpPayTranHdr.Rows[i]["Tph_RESTSPLHOLNDHr"];
                            drEmpPayroll["Tpy_RESTSPLHOLNDOTHr"]                                    = dtEmpPayTranHdr.Rows[i]["Tph_RESTSPLHOLNDOTHr"];
                            drEmpPayroll["Tpy_RESTCOMPHOLHr"]                                       = dtEmpPayTranHdr.Rows[i]["Tph_RESTCOMPHOLHr"];
                            drEmpPayroll["Tpy_RESTCOMPHOLOTHr"]                                     = dtEmpPayTranHdr.Rows[i]["Tph_RESTCOMPHOLOTHr"];
                            drEmpPayroll["Tpy_RESTCOMPHOLNDHr"]                                     = dtEmpPayTranHdr.Rows[i]["Tph_RESTCOMPHOLNDHr"];
                            drEmpPayroll["Tpy_RESTCOMPHOLNDOTHr"]                                   = dtEmpPayTranHdr.Rows[i]["Tph_RESTCOMPHOLNDOTHr"];
                            drEmpPayroll["Tpy_RESTPSDHr"]                                           = dtEmpPayTranHdr.Rows[i]["Tph_RESTPSDHr"];
                            drEmpPayroll["Tpy_RESTPSDOTHr"]                                         = dtEmpPayTranHdr.Rows[i]["Tph_RESTPSDOTHr"];
                            drEmpPayroll["Tpy_RESTPSDNDHr"]                                         = dtEmpPayTranHdr.Rows[i]["Tph_RESTPSDNDHr"];
                            drEmpPayroll["Tpy_RESTPSDNDOTHr"]                                       = dtEmpPayTranHdr.Rows[i]["Tph_RESTPSDNDOTHr"];
                            //Total Regular Hour
                            drEmpPayroll["Tpy_TotalREGHr"]                                          = Convert.ToDouble(dtEmpPayTranHdr.Rows[i]["Tph_REGHr"])
                                                                                                            + Convert.ToDouble(dtEmpPayTranHdr.Rows[i]["Tph_PDLVHr"])
                                                                                                            + Convert.ToDouble(dtEmpPayTranHdr.Rows[i]["Tph_PDLEGHOLHr"])
                                                                                                            + Convert.ToDouble(dtEmpPayTranHdr.Rows[i]["Tph_PDSPLHOLHr"])
                                                                                                            + Convert.ToDouble(dtEmpPayTranHdr.Rows[i]["Tph_PDCOMPHOLHr"])
                                                                                                            + Convert.ToDouble(dtEmpPayTranHdr.Rows[i]["Tph_PDPSDHr"])
                                                                                                            + Convert.ToDouble(dtEmpPayTranHdr.Rows[i]["Tph_PDOTHHOLHr"])
                                                                                                            + Convert.ToDouble(dtEmpPayTranHdr.Rows[i]["Tph_PDRESTLEGHOLHr"]);

                            drEmpPayroll["Tpy_LTAmt"]                                               = 0;
                            drEmpPayroll["Tpy_UTAmt"]                                               = 0;
                            drEmpPayroll["Tpy_UPLVAmt"]                                             = 0;
                            drEmpPayroll["Tpy_ABSLEGHOLAmt"]                                        = 0;
                            drEmpPayroll["Tpy_ABSSPLHOLAmt"]                                        = 0;
                            drEmpPayroll["Tpy_ABSCOMPHOLAmt"]                                       = 0;
                            drEmpPayroll["Tpy_ABSPSDAmt"]                                           = 0;
                            drEmpPayroll["Tpy_ABSOTHHOLAmt"]                                        = 0;
                            drEmpPayroll["Tpy_WDABSAmt"]                                            = 0;
                            drEmpPayroll["Tpy_LTUTMaxAmt"]                                          = 0;
                            drEmpPayroll["Tpy_ABSAmt"]                                              = 0;
                            drEmpPayroll["Tpy_PDLVAmt"]                                             = 0;
                            drEmpPayroll["Tpy_PDLEGHOLAmt"]                                         = 0;
                            drEmpPayroll["Tpy_PDSPLHOLAmt"]                                         = 0;
                            drEmpPayroll["Tpy_PDCOMPHOLAmt"]                                        = 0;
                            drEmpPayroll["Tpy_PDPSDAmt"]                                            = 0;
                            drEmpPayroll["Tpy_PDOTHHOLAmt"]                                         = 0;
                            drEmpPayroll["Tpy_PDRESTLEGHOLAmt"]                                     = 0;
                            drEmpPayroll["Tpy_TotalREGAmt"]                                         = 0;
                            drEmpPayroll["Tpy_REGOTAmt"]                                            = 0;
                            drEmpPayroll["Tpy_REGNDAmt"]                                            = 0;
                            drEmpPayroll["Tpy_REGNDOTAmt"]                                          = 0;
                            drEmpPayroll["Tpy_RESTAmt"]                                             = 0;
                            drEmpPayroll["Tpy_RESTOTAmt"]                                           = 0;
                            drEmpPayroll["Tpy_RESTNDAmt"]                                           = 0;
                            drEmpPayroll["Tpy_RESTNDOTAmt"]                                         = 0;
                            drEmpPayroll["Tpy_LEGHOLAmt"]                                           = 0;
                            drEmpPayroll["Tpy_LEGHOLOTAmt"]                                         = 0;
                            drEmpPayroll["Tpy_LEGHOLNDAmt"]                                         = 0;
                            drEmpPayroll["Tpy_LEGHOLNDOTAmt"]                                       = 0;
                            drEmpPayroll["Tpy_SPLHOLAmt"]                                           = 0;
                            drEmpPayroll["Tpy_SPLHOLOTAmt"]                                         = 0;
                            drEmpPayroll["Tpy_SPLHOLNDAmt"]                                         = 0;
                            drEmpPayroll["Tpy_SPLHOLNDOTAmt"]                                       = 0;
                            drEmpPayroll["Tpy_PSDAmt"]                                              = 0;
                            drEmpPayroll["Tpy_PSDOTAmt"]                                            = 0;
                            drEmpPayroll["Tpy_PSDNDAmt"]                                            = 0;
                            drEmpPayroll["Tpy_PSDNDOTAmt"]                                          = 0;
                            drEmpPayroll["Tpy_COMPHOLAmt"]                                          = 0;
                            drEmpPayroll["Tpy_COMPHOLOTAmt"]                                        = 0;
                            drEmpPayroll["Tpy_COMPHOLNDAmt"]                                        = 0;
                            drEmpPayroll["Tpy_COMPHOLNDOTAmt"]                                      = 0;
                            drEmpPayroll["Tpy_RESTLEGHOLAmt"]                                       = 0;
                            drEmpPayroll["Tpy_RESTLEGHOLOTAmt"]                                     = 0;
                            drEmpPayroll["Tpy_RESTLEGHOLNDAmt"]                                     = 0;
                            drEmpPayroll["Tpy_RESTLEGHOLNDOTAmt"]                                   = 0;
                            drEmpPayroll["Tpy_RESTSPLHOLAmt"]                                       = 0;
                            drEmpPayroll["Tpy_RESTSPLHOLOTAmt"]                                     = 0;
                            drEmpPayroll["Tpy_RESTSPLHOLNDAmt"]                                     = 0;
                            drEmpPayroll["Tpy_RESTSPLHOLNDOTAmt"]                                   = 0;
                            drEmpPayroll["Tpy_RESTCOMPHOLAmt"]                                      = 0;
                            drEmpPayroll["Tpy_RESTCOMPHOLOTAmt"]                                    = 0;
                            drEmpPayroll["Tpy_RESTCOMPHOLNDAmt"]                                    = 0;
                            drEmpPayroll["Tpy_RESTCOMPHOLNDOTAmt"]                                  = 0;
                            drEmpPayroll["Tpy_RESTPSDAmt"]                                          = 0;
                            drEmpPayroll["Tpy_RESTPSDOTAmt"]                                        = 0;
                            drEmpPayroll["Tpy_RESTPSDNDAmt"]                                        = 0;
                            drEmpPayroll["Tpy_RESTPSDNDOTAmt"]                                      = 0;
                            drEmpPayroll["Tpy_TotalOTNDAmt"]                                        = 0;

                            drEmpPayroll["Tpy_WorkDay"]                                             = dtEmpPayTranHdr.Rows[i]["Tph_WorkDay"];
                            drEmpPayroll["Tpy_DayCountOldSalaryRate"]                               = 0;

                            drEmpPayroll["Tpy_SRGAdjHr"]                                            = dtEmpPayTranHdr.Rows[i]["Tph_SRGAdjHr"];
                            drEmpPayroll["Tpy_SRGAdjAmt"]                                           = dtEmpPayTranHdr.Rows[i]["Tph_SRGAdjAmt"];
                            drEmpPayroll["Tpy_SOTAdjHr"]                                            = dtEmpPayTranHdr.Rows[i]["Tph_SOTAdjHr"];
                            drEmpPayroll["Tpy_SOTAdjAmt"]                                           = dtEmpPayTranHdr.Rows[i]["Tph_SOTAdjAmt"];
                            drEmpPayroll["Tpy_SHOLAdjHr"]                                           = dtEmpPayTranHdr.Rows[i]["Tph_SHOLAdjHr"];
                            drEmpPayroll["Tpy_SHOLAdjAmt"]                                          = dtEmpPayTranHdr.Rows[i]["Tph_SHOLAdjAmt"];
                            drEmpPayroll["Tpy_SNDAdjHr"]                                            = dtEmpPayTranHdr.Rows[i]["Tph_SNDAdjHr"];
                            drEmpPayroll["Tpy_SNDAdjAmt"]                                           = dtEmpPayTranHdr.Rows[i]["Tph_SNDAdjAmt"];
                            drEmpPayroll["Tpy_SLVAdjHr"]                                            = dtEmpPayTranHdr.Rows[i]["Tph_SLVAdjHr"];
                            drEmpPayroll["Tpy_SLVAdjAmt"]                                           = dtEmpPayTranHdr.Rows[i]["Tph_SLVAdjAmt"];
                            drEmpPayroll["Tpy_MRGAdjHr"]                                            = dtEmpPayTranHdr.Rows[i]["Tph_MRGAdjHr"];
                            drEmpPayroll["Tpy_MRGAdjAmt"]                                           = dtEmpPayTranHdr.Rows[i]["Tph_MRGAdjAmt"];
                            drEmpPayroll["Tpy_MOTAdjHr"]                                            = dtEmpPayTranHdr.Rows[i]["Tph_MOTAdjHr"];
                            drEmpPayroll["Tpy_MOTAdjAmt"]                                           = dtEmpPayTranHdr.Rows[i]["Tph_MOTAdjAmt"];
                            drEmpPayroll["Tpy_MHOLAdjHr"]                                           = dtEmpPayTranHdr.Rows[i]["Tph_MHOLAdjHr"];
                            drEmpPayroll["Tpy_MHOLAdjAmt"]                                          = dtEmpPayTranHdr.Rows[i]["Tph_MHOLAdjAmt"];
                            drEmpPayroll["Tpy_MNDAdjHr"]                                            = dtEmpPayTranHdr.Rows[i]["Tph_MNDAdjHr"];
                            drEmpPayroll["Tpy_MNDAdjAmt"]                                           = dtEmpPayTranHdr.Rows[i]["Tph_MNDAdjAmt"];
                            drEmpPayroll["Tpy_TotalAdjAmt"]                                         = dtEmpPayTranHdr.Rows[i]["Tph_TotalAdjAmt"];
                            
                            drEmpPayroll["Tpy_TaxableIncomeAmt"]                                    = dtEmpPayTranHdr.Rows[i]["Tph_TaxableIncomeAmt"];
                            drEmpPayroll["Tpy_TotalTaxableIncomeAmt"]                               = 0;
                            drEmpPayroll["Tpy_NontaxableIncomeAmt"]                                 = dtEmpPayTranHdr.Rows[i]["Tph_NontaxableIncomeAmt"];
                            drEmpPayroll["Tpy_GrossIncomeAmt"]                                      = 0;
                            drEmpPayroll["Tpy_WtaxAmt"]                                             = 0;
                            drEmpPayroll["Tpy_TaxBaseAmt"]                                          = 0;
                            drEmpPayroll["Tpy_TaxRule"]                                             = dtEmpPayTranHdr.Rows[i]["Mem_TaxRule"];
                            drEmpPayroll["Tpy_TaxShare"]                                            = dtEmpPayTranHdr.Rows[i]["Mem_TaxShare"];
                            drEmpPayroll["Tpy_TaxBracket"]                                          = 0;
                            drEmpPayroll["Tpy_TaxCode"]                                             = dtEmpPayTranHdr.Rows[i]["Mem_TaxCode"];
                            drEmpPayroll["Tpy_IsTaxExempted"]                                       = dtEmpPayTranHdr.Rows[i]["Mem_IsTaxExempted"];

                            drEmpPayroll["Tpy_SSSEE"]                                               = 0;
                            drEmpPayroll["Tpy_SSSER"]                                               = 0;
                            drEmpPayroll["Tpy_ECFundAmt"]                                           = 0;
                            drEmpPayroll["Tpy_SSSBaseAmt"]                                          = 0;
                            drEmpPayroll["Tpy_SSSRule"]                                             = dtEmpPayTranHdr.Rows[i]["Mem_SSSRule"];
                            drEmpPayroll["Tpy_SSSShare"]                                            = dtEmpPayTranHdr.Rows[i]["Mem_SSSShare"];
                            drEmpPayroll["Tpy_MPFEE"]                                               = 0;
                            drEmpPayroll["Tpy_MPFER"]                                               = 0;

                            drEmpPayroll["Tpy_PhilhealthEE"]                                        = 0;
                            drEmpPayroll["Tpy_PhilhealthER"]                                        = 0;
                            drEmpPayroll["Tpy_PhilhealthBaseAmt"]                                   = 0;
                            drEmpPayroll["Tpy_PhilhealthRule"]                                      = dtEmpPayTranHdr.Rows[i]["Mem_PHRule"];
                            drEmpPayroll["Tpy_PhilhealthShare"]                                     = dtEmpPayTranHdr.Rows[i]["Mem_PHShare"];
                            drEmpPayroll["Tpy_PagIbigEE"]                                           = 0;
                            drEmpPayroll["Tpy_PagIbigER"]                                           = 0;
                            drEmpPayroll["Tpy_PagIbigTaxEE"]                                        = 0;
                            drEmpPayroll["Tpy_PagIbigBaseAmt"]                                      = 0;
                            drEmpPayroll["Tpy_PagIbigRule"]                                         = dtEmpPayTranHdr.Rows[i]["Mem_PagIbigRule"];
                            drEmpPayroll["Tpy_PagIbigShare"]                                        = dtEmpPayTranHdr.Rows[i]["Mem_PagIbigShare"];
                            drEmpPayroll["Tpy_UnionAmt"]                                            = 0;
                            drEmpPayroll["Tpy_UnionRule"]                                           = dtEmpPayTranHdr.Rows[i]["Mem_UnionRule"];
                            drEmpPayroll["Tpy_UnionShare"]                                          = dtEmpPayTranHdr.Rows[i]["Mem_UnionShare"];
                            drEmpPayroll["Tpy_UnionBaseAmt"]                                        = 0;

                            drEmpPayroll["Tpy_OtherDeductionAmt"]                                   = 0;
                            drEmpPayroll["Tpy_TotalDeductionAmt"]                                   = 0;
                            drEmpPayroll["Tpy_NetAmt"]                                              = 0;
                            drEmpPayroll["Tpy_CostcenterCode"]                                      = dtEmpPayTranHdr.Rows[i]["Mem_CostcenterCode"];
                            drEmpPayroll["Tpy_PositionCode"]                                        = dtEmpPayTranHdr.Rows[i]["Mem_PositionCode"];
                            drEmpPayroll["Tpy_EmploymentStatus"]                                    = dtEmpPayTranHdr.Rows[i]["Mem_EmploymentStatusCode"];
                            drEmpPayroll["Tpy_PayrollType"]                                         = dtEmpPayTranHdr.Rows[i]["Tph_PayrollType"];
                            drEmpPayroll["Tpy_PaymentMode"]                                         = dtEmpPayTranHdr.Rows[i]["Mem_PaymentMode"];
                            drEmpPayroll["Tpy_BankCode"]                                            = dtEmpPayTranHdr.Rows[i]["Mem_PayrollBankCode"];
                            drEmpPayroll["Tpy_BankAcctNo"]                                          = dtEmpPayTranHdr.Rows[i]["Mem_BankAcctNo"];
                            drEmpPayroll["Tpy_PayrollGroup"]                                        = dtEmpPayTranHdr.Rows[i]["Mem_PayrollGroup"];
                            drEmpPayroll["Tpy_CalendarGrpCode"]                                     = dtEmpPayTranHdr.Rows[i]["Mem_CalendarGrpCode"];
                            drEmpPayroll["Tpy_WorkLocationCode"]                                    = dtEmpPayTranHdr.Rows[i]["Mem_WorkLocationCode"];
                            drEmpPayroll["Tpy_AltAcctCode"]                                         = dtEmpPayTranHdr.Rows[i]["Mem_AltAcctCode"];
                            drEmpPayroll["Tpy_ExpenseClass"]                                        = dtEmpPayTranHdr.Rows[i]["Mem_ExpenseClass"];
                            drEmpPayroll["Tpy_PositionGrade"]                                       = dtEmpPayTranHdr.Rows[i]["Mem_PositionGrade"];
                            drEmpPayroll["Tpy_WorkStatus"]                                          = dtEmpPayTranHdr.Rows[i]["Mem_WorkStatus"];
                            drEmpPayroll["Tpy_PremiumGrpCode"]                                      = dtEmpPayTranHdr.Rows[i]["Tph_PremiumGrpCode"];
                            drEmpPayroll["Tpy_IsComputedPerDay"]                                    = dtEmpPayTranHdr.Rows[i]["Mem_IsComputedPerDay"];
                            drEmpPayroll["Tpy_IsMultipleSalary"]                                    = dtEmpPayTranHdr.Rows[i]["Mem_IsMultipleSalary"];
                            drEmpPayroll["Tpy_RegRule"]                                             = "";
                            drEmpPayroll["Usr_login"]                                               = LoginUser;
                            drEmpPayroll["Ludatetime"]                                              = DateTime.Now;

                            #endregion

                            #region Initialize Payroll Calc Ext2 Row
                            drEmpPayroll2["Tpy_IDNo"]                                               = curEmployeeId;
                            drEmpPayroll2["Tpy_PayCycle"]                                           = dtEmpPayTranHdr.Rows[i]["Tph_PayCycle"];
                            drEmpPayroll2["Tpy_RemainingPayCycle"]                                  = 0;
                            drEmpPayroll2["Tpy_YTDRegularAmtBefore"]                                = 0;
                            drEmpPayroll2["Tpy_AssumeRegularAmt"]                                   = 0;
                            drEmpPayroll2["Tpy_RegularTotal"]                                       = 0;
                            drEmpPayroll2["Tpy_YTDRecurringAllowanceAmtBefore"]                     = 0;
                            drEmpPayroll2["Tpy_YTDRecurringAllowanceAmt"]                           = 0; 
                            drEmpPayroll2["Tpy_RecurringAllowanceAmt"]                              = 0;
                            drEmpPayroll2["Tpy_AssumeRecurringAllowanceAmt"]                        = 0;
                            drEmpPayroll2["Tpy_RecurringAllowanceTotal"]                            = 0;
                            drEmpPayroll2["Tpy_YTDBonusAmtBefore"]                                  = 0;
                            drEmpPayroll2["Tpy_BonusNontaxAmt"]                                     = 0;
                            drEmpPayroll2["Tpy_BonusTaxAmt"]                                        = 0;
                            drEmpPayroll2["Tpy_Assume13thMonthAmt"]                                 = 0;
                            drEmpPayroll2["Tpy_BonusTotal"]                                         = 0;
                            drEmpPayroll2["Tpy_BonusTaxRevaluated"]                                 = 0;
                            drEmpPayroll2["Tpy_YTDBonusTaxBefore"]                                  = 0;
                            drEmpPayroll2["Tpy_YTDOtherTaxableIncomeAmtBefore"]                     = 0;
                            drEmpPayroll2["Tpy_OtherTaxableIncomeAmt"]                              = 0;
                            drEmpPayroll2["Tpy_OtherTaxableIncomeTotal"]                            = 0;
                            drEmpPayroll2["Tpy_YTDSSSAmtBefore"]                                    = 0;
                            drEmpPayroll2["Tpy_AssumeSSSAmt"]                                       = 0;
                            drEmpPayroll2["Tpy_SSSTotal"]                                           = 0;
                            drEmpPayroll2["Tpy_YTDPhilhealthAmtBefore"]                             = 0;
                            drEmpPayroll2["Tpy_AssumePhilhealthAmt"]                                = 0;
                            drEmpPayroll2["Tpy_PhilhealthTotal"]                                    = 0;
                            drEmpPayroll2["Tpy_YTDPagIbigAmtBefore"]                                = 0;
                            drEmpPayroll2["Tpy_AssumePagIbigAmt"]                                   = 0;
                            drEmpPayroll2["Tpy_PagIbigTotal"]                                       = 0;
                            drEmpPayroll2["Tpy_YTDPagIbigTaxAmtBefore"]                             = 0;
                            drEmpPayroll2["Tpy_AssumePagIbigTaxAmt"]                                = 0;
                            drEmpPayroll2["Tpy_TotalTaxableIncomeWAssumeAmt"]                       = 0;
                            drEmpPayroll2["Tpy_YTDUnionAmtBefore"]                                  = 0;
                            drEmpPayroll2["Tpy_AssumeUnionAmt"]                                     = 0;
                            drEmpPayroll2["Tpy_UnionTotal"]                                         = 0;
                            drEmpPayroll2["Tpy_PremiumPaidOnHealth"]                                = 0;
                            drEmpPayroll2["Tpy_TotalExemptions"]                                    = 0;
                            drEmpPayroll2["Tpy_NetTaxableIncomeAmt"]                                = 0;
                            drEmpPayroll2["Tpy_YTDREGAmt"]                                          = 0;
                            drEmpPayroll2["Tpy_YTDTotalTaxableIncomeAmtBefore"]                     = 0;
                            drEmpPayroll2["Tpy_YTDTotalTaxableIncomeAmt"]                           = 0;
                            drEmpPayroll2["Tpy_YTDWtaxAmtBefore"]                                   = 0;
                            drEmpPayroll2["Tpy_YTDWtaxAmt"]                                         = 0;
                            drEmpPayroll2["Tpy_YTDSSSAmt"]                                          = 0;
                            drEmpPayroll2["Tpy_YTDPhilhealthAmt"]                                   = 0;
                            drEmpPayroll2["Tpy_YTDPagIbigAmt"]                                      = 0;
                            drEmpPayroll2["Tpy_YTDPagIbigTaxAmt"]                                   = 0;
                            drEmpPayroll2["Tpy_YTDUnionAmt"]                                        = 0;
                            drEmpPayroll2["Tpy_YTDNontaxableAmtBefore"]                             = 0;
                            drEmpPayroll2["Tpy_YTDNontaxableAmt"]                                   = 0;
                            drEmpPayroll2["Tpy_BIRTotalAmountofCompensation"]                       = 0;
                            drEmpPayroll2["Tpy_BIRStatutoryMinimumWage"]                            = 0;
                            drEmpPayroll2["Tpy_BIRHolidayOvertimeNightShiftHazard"]                 = 0;
                            drEmpPayroll2["Tpy_BIR13thMonthPayOtherBenefits"]                       = 0;
                            drEmpPayroll2["Tpy_BIRDeMinimisBenefits"]                               = 0;
                            drEmpPayroll2["Tpy_BIRSSSGSISPHICHDMFUnionDues"]                        = 0;
                            drEmpPayroll2["Tpy_BIROtherNonTaxableCompensation"]                     = 0;
                            drEmpPayroll2["Tpy_BIRTotalTaxableCompensation"]                        = 0;
                            drEmpPayroll2["Tpy_BIRTaxableCompensationNotSubjectToWithholdingTax"]   = 0;
                            drEmpPayroll2["Tpy_BIRTotalTaxesWithheld"]                              = 0;
                            drEmpPayroll2["Tpy_TaxDue"]                                             = 0;
                            drEmpPayroll2["Tpy_AssumeMonthlyRegularAmt"]                            = 0;
                            drEmpPayroll2["Tpy_AssumeMonthlySSSAmt"]                                = 0;
                            drEmpPayroll2["Tpy_AssumeMonthlyPhilhealthAmt"]                         = 0;
                            drEmpPayroll2["Tpy_AssumeMonthlyPagibigAmt"]                            = 0;
                            drEmpPayroll2["Tpy_AssumeMonthlyUnionAmt"]                              = 0;
                            drEmpPayroll2["Tpy_YTDMPFAmtBefore"]                                    = 0;
                            drEmpPayroll2["Tpy_YTDMPFAmt"]                                          = 0;
                            drEmpPayroll2["Usr_Login"]                                              = LoginUser;
                            drEmpPayroll2["Ludatetime"]                                             = DateTime.Now;

                            #endregion

                            #region Initialize Payroll Calc Ext Row
                            if (bHasDayCodeExt)
                            {
                                drEmpPayrollMisc["Tpm_IDNo"]                                        = curEmployeeId;
                                drEmpPayrollMisc["Tpm_PayCycle"]                                    = ProcessPayrollPeriod;
                                drEmpPayrollMisc["Tpm_Misc1Hr"]                                     = 0;
                                drEmpPayrollMisc["Tpm_Misc1OTHr"]                                   = 0;
                                drEmpPayrollMisc["Tpm_Misc1NDHr"]                                   = 0;
                                drEmpPayrollMisc["Tpm_Misc1NDOTHr"]                                 = 0;
                                drEmpPayrollMisc["Tpm_Misc2Hr"]                                     = 0;
                                drEmpPayrollMisc["Tpm_Misc2OTHr"]                                   = 0;
                                drEmpPayrollMisc["Tpm_Misc2NDHr"]                                   = 0;
                                drEmpPayrollMisc["Tpm_Misc2NDOTHr"]                                 = 0;
                                drEmpPayrollMisc["Tpm_Misc3Hr"]                                     = 0;
                                drEmpPayrollMisc["Tpm_Misc3OTHr"]                                   = 0;
                                drEmpPayrollMisc["Tpm_Misc3NDHr"]                                   = 0;
                                drEmpPayrollMisc["Tpm_Misc3NDOTHr"]                                 = 0;
                                drEmpPayrollMisc["Tpm_Misc4Hr"]                                     = 0;
                                drEmpPayrollMisc["Tpm_Misc4OTHr"]                                   = 0;
                                drEmpPayrollMisc["Tpm_Misc4NDHr"]                                   = 0;
                                drEmpPayrollMisc["Tpm_Misc4NDOTHr"]                                 = 0;
                                drEmpPayrollMisc["Tpm_Misc5Hr"]                                     = 0;
                                drEmpPayrollMisc["Tpm_Misc5OTHr"]                                   = 0;
                                drEmpPayrollMisc["Tpm_Misc5NDHr"]                                   = 0;
                                drEmpPayrollMisc["Tpm_Misc5NDOTHr"]                                 = 0;
                                drEmpPayrollMisc["Tpm_Misc6Hr"]                                     = 0;
                                drEmpPayrollMisc["Tpm_Misc6OTHr"]                                   = 0;
                                drEmpPayrollMisc["Tpm_Misc6NDHr"]                                   = 0;
                                drEmpPayrollMisc["Tpm_Misc6NDOTHr"]                                 = 0;
                                drEmpPayrollMisc["Tpm_Misc1Amt"]                                    = 0;
                                drEmpPayrollMisc["Tpm_Misc1OTAmt"]                                  = 0;
                                drEmpPayrollMisc["Tpm_Misc1NDAmt"]                                  = 0;
                                drEmpPayrollMisc["Tpm_Misc1NDOTAmt"]                                = 0;
                                drEmpPayrollMisc["Tpm_Misc2Amt"]                                    = 0;
                                drEmpPayrollMisc["Tpm_Misc2OTAmt"]                                  = 0;
                                drEmpPayrollMisc["Tpm_Misc2NDAmt"]                                  = 0;
                                drEmpPayrollMisc["Tpm_Misc2NDOTAmt"]                                = 0;
                                drEmpPayrollMisc["Tpm_Misc3Amt"]                                    = 0;
                                drEmpPayrollMisc["Tpm_Misc3OTAmt"]                                  = 0;
                                drEmpPayrollMisc["Tpm_Misc3NDAmt"]                                  = 0;
                                drEmpPayrollMisc["Tpm_Misc3NDOTAmt"]                                = 0;
                                drEmpPayrollMisc["Tpm_Misc4Amt"]                                    = 0;
                                drEmpPayrollMisc["Tpm_Misc4OTAmt"]                                  = 0;
                                drEmpPayrollMisc["Tpm_Misc4NDAmt"]                                  = 0;
                                drEmpPayrollMisc["Tpm_Misc4NDOTAmt"]                                = 0;
                                drEmpPayrollMisc["Tpm_Misc5Amt"]                                    = 0;
                                drEmpPayrollMisc["Tpm_Misc5OTAmt"]                                  = 0;
                                drEmpPayrollMisc["Tpm_Misc5NDAmt"]                                  = 0;
                                drEmpPayrollMisc["Tpm_Misc5NDOTAmt"]                                = 0;
                                drEmpPayrollMisc["Tpm_Misc6Amt"]                                    = 0;
                                drEmpPayrollMisc["Tpm_Misc6OTAmt"]                                  = 0;
                                drEmpPayrollMisc["Tpm_Misc6NDAmt"]                                  = 0;
                                drEmpPayrollMisc["Tpm_Misc6NDOTAmt"]                                = 0;
                                drEmpPayrollMisc["Usr_Login"]                                       = LoginUser;
                                drEmpPayrollMisc["Ludatetime"]                                      = DateTime.Now;

                                drArrEmployeePayrollTransExt = dtEmpPayTranHdrMisc.Select("Tph_IDNo = '" + curEmployeeId + "'");
                                if (drArrEmployeePayrollTransExt.Length > 0)
                                {
                                    drEmpPayrollMisc["Tpm_Misc1Hr"]                                 = drArrEmployeePayrollTransExt[0]["Tph_Misc1Hr"];
                                    drEmpPayrollMisc["Tpm_Misc1OTHr"]                               = drArrEmployeePayrollTransExt[0]["Tph_Misc1OTHr"];
                                    drEmpPayrollMisc["Tpm_Misc1NDHr"]                               = drArrEmployeePayrollTransExt[0]["Tph_Misc1NDHr"];
                                    drEmpPayrollMisc["Tpm_Misc1NDOTHr"]                             = drArrEmployeePayrollTransExt[0]["Tph_Misc1NDOTHr"];
                                    drEmpPayrollMisc["Tpm_Misc2Hr"]                                 = drArrEmployeePayrollTransExt[0]["Tph_Misc2Hr"];
                                    drEmpPayrollMisc["Tpm_Misc2OTHr"]                               = drArrEmployeePayrollTransExt[0]["Tph_Misc2OTHr"];
                                    drEmpPayrollMisc["Tpm_Misc2NDHr"]                               = drArrEmployeePayrollTransExt[0]["Tph_Misc2NDHr"];
                                    drEmpPayrollMisc["Tpm_Misc2NDOTHr"]                             = drArrEmployeePayrollTransExt[0]["Tph_Misc2NDOTHr"];
                                    drEmpPayrollMisc["Tpm_Misc3Hr"]                                 = drArrEmployeePayrollTransExt[0]["Tph_Misc3Hr"];
                                    drEmpPayrollMisc["Tpm_Misc3OTHr"]                               = drArrEmployeePayrollTransExt[0]["Tph_Misc3OTHr"];
                                    drEmpPayrollMisc["Tpm_Misc3NDHr"]                               = drArrEmployeePayrollTransExt[0]["Tph_Misc3NDHr"];
                                    drEmpPayrollMisc["Tpm_Misc3NDOTHr"]                             = drArrEmployeePayrollTransExt[0]["Tph_Misc3NDOTHr"];
                                    drEmpPayrollMisc["Tpm_Misc4Hr"]                                 = drArrEmployeePayrollTransExt[0]["Tph_Misc4Hr"];
                                    drEmpPayrollMisc["Tpm_Misc4OTHr"]                               = drArrEmployeePayrollTransExt[0]["Tph_Misc4OTHr"];
                                    drEmpPayrollMisc["Tpm_Misc4NDHr"]                               = drArrEmployeePayrollTransExt[0]["Tph_Misc4NDHr"];
                                    drEmpPayrollMisc["Tpm_Misc4NDOTHr"]                             = drArrEmployeePayrollTransExt[0]["Tph_Misc4NDOTHr"];
                                    drEmpPayrollMisc["Tpm_Misc5Hr"]                                 = drArrEmployeePayrollTransExt[0]["Tph_Misc5Hr"];
                                    drEmpPayrollMisc["Tpm_Misc5OTHr"]                               = drArrEmployeePayrollTransExt[0]["Tph_Misc5OTHr"];
                                    drEmpPayrollMisc["Tpm_Misc5NDHr"]                               = drArrEmployeePayrollTransExt[0]["Tph_Misc5NDHr"];
                                    drEmpPayrollMisc["Tpm_Misc5NDOTHr"]                             = drArrEmployeePayrollTransExt[0]["Tph_Misc5NDOTHr"];
                                    drEmpPayrollMisc["Tpm_Misc6Hr"]                                 = drArrEmployeePayrollTransExt[0]["Tph_Misc6Hr"];
                                    drEmpPayrollMisc["Tpm_Misc6OTHr"]                               = drArrEmployeePayrollTransExt[0]["Tph_Misc6OTHr"];
                                    drEmpPayrollMisc["Tpm_Misc6NDHr"]                               = drArrEmployeePayrollTransExt[0]["Tph_Misc6NDHr"];
                                    drEmpPayrollMisc["Tpm_Misc6NDOTHr"]                             = drArrEmployeePayrollTransExt[0]["Tph_Misc6NDOTHr"];
                                }
                            }
                            #endregion

                            #region Initialize Payroll Calc Amount
                            LateAmt                                                                 = 0;
                            UndertimeAmt                                                            = 0;
                            UnpaidLeaveAmt                                                          = 0;
                            AbsentLegalHolidayAmt                                                   = 0;
                            AbsentSpecialHolidayAmt                                                 = 0;
                            AbsentCompanyHolidayAmt                                                 = 0;
                            AbsentPlantShutdownAmt                                                  = 0;
                            AbsentFillerHolidayAmt                                                  = 0;
                            WholeDayAbsentAmt                                                       = 0;
                            LateUndertimeMaxAbsentAmt                                               = 0;
                            AbsentAmt                                                               = 0;

                            RegularAmt                                                              = 0;
                            PaidLeaveAmt                                                            = 0;
                            PaidLegalHolidayAmt                                                     = 0;
                            PaidSpecialHolidayAmt                                                   = 0;
                            PaidCompanyHolidayAmt                                                   = 0;
                            PaidFillerHolidayAmt                                                    = 0;
                            PaidPlantShutdownHolidayAmt                                             = 0;
                            PaidRestdayLegalHolidayAmt                                              = 0;

                            RegularOTAmt                                                            = 0;
                            RegularNDAmt                                                            = 0;
                            RegularOTNDAmt                                                          = 0;
                            RestdayAmt                                                              = 0;
                            RestdayOTAmt                                                            = 0;
                            RestdayNDAmt                                                            = 0;
                            RestdayOTNDAmt                                                          = 0;
                            LegalHolidayAmt                                                         = 0;
                            LegalHolidayOTAmt                                                       = 0;
                            LegalHolidayNDAmt                                                       = 0;
                            LegalHolidayOTNDAmt                                                     = 0;
                            SpecialHolidayAmt                                                       = 0;
                            SpecialHolidayOTAmt                                                     = 0;
                            SpecialHolidayNDAmt                                                     = 0;
                            SpecialHolidayOTNDAmt                                                   = 0;
                            PlantShutdownAmt                                                        = 0;
                            PlantShutdownOTAmt                                                      = 0;
                            PlantShutdownNDAmt                                                      = 0;
                            PlantShutdownOTNDAmt                                                    = 0;
                            CompanyHolidayAmt                                                       = 0;
                            CompanyHolidayOTAmt                                                     = 0;
                            CompanyHolidayNDAmt                                                     = 0;
                            CompanyHolidayOTNDAmt                                                   = 0;
                            RestdayLegalHolidayAmt                                                  = 0;
                            RestdayLegalHolidayOTAmt                                                = 0;
                            RestdayLegalHolidayNDAmt                                                = 0;
                            RestdayLegalHolidayOTNDAmt                                              = 0;
                            RestdaySpecialHolidayAmt                                                = 0;
                            RestdaySpecialHolidayOTAmt                                              = 0;
                            RestdaySpecialHolidayNDAmt                                              = 0;
                            RestdaySpecialHolidayOTNDAmt                                            = 0;
                            RestdayCompanyHolidayAmt                                                = 0;
                            RestdayCompanyHolidayOTAmt                                              = 0;
                            RestdayCompanyHolidayNDAmt                                              = 0;
                            RestdayCompanyHolidayOTNDAmt                                            = 0;
                            RestdayPlantShutdownAmt                                                 = 0;
                            RestdayPlantShutdownOTAmt                                               = 0;
                            RestdayPlantShutdownNDAmt                                               = 0;
                            RestdayPlantShutdownOTNDAmt                                             = 0;

                            OldDailyRate                                                            = 0;
                            NewDailyRate                                                            = 0;
                            WorkingDayCnt                                                           = 0;
                            WorkingDayCntUsingNewRate                                               = 0;

                            Misc1Amt                                                                = 0;
                            Misc1OTAmt                                                              = 0;
                            Misc1NDAmt                                                              = 0;
                            Misc1NDOTAmt                                                            = 0;
                            Misc2Amt                                                                = 0;
                            Misc2OTAmt                                                              = 0;
                            Misc2NDAmt                                                              = 0;
                            Misc2NDOTAmt                                                            = 0;
                            Misc3Amt                                                                = 0;
                            Misc3OTAmt                                                              = 0;
                            Misc3NDAmt                                                              = 0;
                            Misc3NDOTAmt                                                            = 0;
                            Misc4Amt                                                                = 0;
                            Misc4OTAmt                                                              = 0;
                            Misc4NDAmt                                                              = 0;
                            Misc4NDOTAmt                                                            = 0;
                            Misc5Amt                                                                = 0;
                            Misc5OTAmt                                                              = 0;
                            Misc5NDAmt                                                              = 0;
                            Misc5NDOTAmt                                                            = 0;
                            Misc6Amt                                                                = 0;
                            Misc6OTAmt                                                              = 0;
                            Misc6NDAmt                                                              = 0;
                            Misc6NDOTAmt                                                            = 0;
                            #endregion

                            #region Initialize Reg Rule
                            if (!ProcessSeparated)
                            {
                                bMonthlyToDailyPayrollType  = false;
                                strRegRule                  = "";
                                PayrollType                 = drEmpPayroll["Tpy_PayrollType"].ToString();
                                drArrEmployeeSalary = dtEmployeeSalary.Select("Tsl_IDNo = '" + curEmployeeId + "'");
                                if (drArrEmployeeSalary.Length > 0) //MULTIPLE SALARY
                                {
                                    if (drArrEmployeeSalary[0]["Tsl_PayrollType"].ToString() == drArrEmployeeSalary[drArrEmployeeSalary.Length - 1]["Tsl_PayrollType"].ToString())
                                    {
                                        if (drArrEmployeeSalary[0]["Tsl_PayrollType"].ToString() == "M")
                                            drEmpPayroll["Tpy_RegRule"] = "A";
                                        else if (drArrEmployeeSalary[0]["Tsl_PayrollType"].ToString() == "D")
                                            drEmpPayroll["Tpy_RegRule"] = "P";
                                    }
                                    else if (drArrEmployeeSalary[0]["Tsl_PayrollType"].ToString() != drArrEmployeeSalary[drArrEmployeeSalary.Length - 1]["Tsl_PayrollType"].ToString()
                                        && drArrEmployeeSalary[0]["Tsl_PayrollType"].ToString() == "M"
                                        && drArrEmployeeSalary[drArrEmployeeSalary.Length - 1]["Tsl_PayrollType"].ToString() == "D")
                                    {
                                        drEmpPayroll["Tpy_RegRule"] = MULTSALMD;
                                        bMonthlyToDailyPayrollType = true;
                                    }
                                    else if (drArrEmployeeSalary[0]["Tsl_PayrollType"].ToString() != drArrEmployeeSalary[drArrEmployeeSalary.Length - 1]["Tsl_PayrollType"].ToString()
                                        && drArrEmployeeSalary[0]["Tsl_PayrollType"].ToString() == "D"
                                        && drArrEmployeeSalary[drArrEmployeeSalary.Length - 1]["Tsl_PayrollType"].ToString() == "M")
                                        drEmpPayroll["Tpy_RegRule"] = MULTSALDM;
                                }
                                else
                                {
                                    drArrEmpNewlyHiredEmployees = dtNewlyHiredEmployees.Select("Mem_IDNo = '" + curEmployeeId + "'");
                                    if (PayrollType == "D")
                                        drEmpPayroll["Tpy_RegRule"]     = "P";
                                    else if (drArrEmpNewlyHiredEmployees.Length > 0 && PayrollType == "M")
                                    {
                                        drEmpPayroll["Tpy_RegRule"] = NEWHIRE;
                                        if (Convert.ToDateTime(drArrEmpNewlyHiredEmployees[0]["Mem_IntakeDate"]) == Convert.ToDateTime(PayrollStart))
                                            drEmpPayroll["Tpy_RegRule"] = "A";
                                    }
                                    else if (drArrEmpNewlyHiredEmployees.Length == 0 && PayrollType == "M")
                                        drEmpPayroll["Tpy_RegRule"]     = "A";
                                }

                                #region New Hire Regular Amount Proration (HOGP)
                                drArrEmpComputedDailyEmployees = dtComputedDailyEmployees.Select("Mem_IDNo = '" + curEmployeeId + "'");
                                if (drArrEmpComputedDailyEmployees.Length > 0 && PayrollType == "M")
                                {
                                    drEmpPayroll["Tpy_RegRule"] = "P"; //computed regular amount as daily paid
                                }
                                #endregion
                                strRegRule = Convert.ToString(drEmpPayroll["Tpy_RegRule"]);
                            }
                            else if (ProcessSeparated)
                            {
                                #region Separated
                                strRegRule = PayRule;
                                drEmpPayroll["Tpy_RegRule"] = PayRule;
                                #endregion
                                
                            }
                            #endregion

                            #region Payroll Dtl
                            drArrEmpPayrollDtl = dtEmpPayTranDtl.Select("Tpd_IDNo = '" + curEmployeeId + "'");
                            for (int idx = 0; idx < drArrEmpPayrollDtl.Length; idx++)
                            {
                                #region Initialize Payroll Dtl Calc Row
                                drEmpPayrollDtl = dtEmpPayrollDtl.NewRow();

                                drEmpPayrollDtl["Tpd_IDNo"]                                         = curEmployeeId;
                                drEmpPayrollDtl["Tpd_PayCycle"]                                     = drArrEmpPayrollDtl[idx]["Tpd_PayCycle"];
                                drEmpPayrollDtl["Tpd_Date"]                                         = drArrEmpPayrollDtl[idx]["Tpd_Date"];
                                drEmpPayrollDtl["Tpd_DayCode"]                                      = drArrEmpPayrollDtl[idx]["Ttr_DayCode"];  //TimeRegister
                                drEmpPayrollDtl["Tpd_RestDayFlag"]                                  = drArrEmpPayrollDtl[idx]["Ttr_RestDayFlag"]; //TimeRegister
                                drEmpPayrollDtl["Tpd_WorkDay"]                                      = drArrEmpPayrollDtl[idx]["Tpd_WorkDay"]; 
                                drEmpPayrollDtl["Tpd_SalaryRate"]                                   = drArrEmpPayrollDtl[idx]["Tpd_SalaryRate"]; //T_EmpSalary
                                drEmpPayrollDtl["Tpd_HourRate"]                                     = 0; //Computed
                                drEmpPayrollDtl["Tpd_PayrollType"]                                  = drArrEmpPayrollDtl[idx]["Tpd_PayrollType"]; //T_EmpSalary
                                drEmpPayrollDtl["Tpd_SpecialRate"]                                  = 0; //FORMULA
                                drEmpPayrollDtl["Tpd_SpecialHourRate"]                              = 0; 
                                drEmpPayrollDtl["Tpd_PremiumGrpCode"]                               = drArrEmpPayrollDtl[idx]["Tpd_PremiumGrpCode"];  
                                drEmpPayrollDtl["Tpd_LTHr"]                                         = drArrEmpPayrollDtl[idx]["Tpd_LTHr"];
                                drEmpPayrollDtl["Tpd_UTHr"]                                         = drArrEmpPayrollDtl[idx]["Tpd_UTHr"];
                                drEmpPayrollDtl["Tpd_UPLVHr"]                                       = drArrEmpPayrollDtl[idx]["Tpd_UPLVHr"];
                                drEmpPayrollDtl["Tpd_ABSLEGHOLHr"]                                  = drArrEmpPayrollDtl[idx]["Tpd_ABSLEGHOLHr"];
                                drEmpPayrollDtl["Tpd_ABSSPLHOLHr"]                                  = drArrEmpPayrollDtl[idx]["Tpd_ABSSPLHOLHr"];
                                drEmpPayrollDtl["Tpd_ABSCOMPHOLHr"]                                 = drArrEmpPayrollDtl[idx]["Tpd_ABSCOMPHOLHr"];
                                drEmpPayrollDtl["Tpd_ABSPSDHr"]                                     = drArrEmpPayrollDtl[idx]["Tpd_ABSPSDHr"];
                                drEmpPayrollDtl["Tpd_ABSOTHHOLHr"]                                  = drArrEmpPayrollDtl[idx]["Tpd_ABSOTHHOLHr"];
                                drEmpPayrollDtl["Tpd_WDABSHr"]                                      = drArrEmpPayrollDtl[idx]["Tpd_WDABSHr"];
                                drEmpPayrollDtl["Tpd_LTUTMaxHr"]                                    = drArrEmpPayrollDtl[idx]["Tpd_LTUTMaxHr"];
                                drEmpPayrollDtl["Tpd_ABSHr"]                                        = drArrEmpPayrollDtl[idx]["Tpd_ABSHr"];

                                drEmpPayrollDtl["Tpd_ABSHr"]                                        = Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSHr"])
                                                                                                            + Convert.ToDecimal(drArrEmpPayrollDtl[idx]["Tpd_ABSLEGHOLHr"])
                                                                                                            + Convert.ToDecimal(drArrEmpPayrollDtl[idx]["Tpd_ABSSPLHOLHr"])
                                                                                                            + Convert.ToDecimal(drArrEmpPayrollDtl[idx]["Tpd_ABSCOMPHOLHr"])
                                                                                                            + Convert.ToDecimal(drArrEmpPayrollDtl[idx]["Tpd_ABSPSDHr"])
                                                                                                            + Convert.ToDecimal(drArrEmpPayrollDtl[idx]["Tpd_ABSOTHHOLHr"]);

                                drEmpPayrollDtl["Tpd_PDLVHr"]                                       = drArrEmpPayrollDtl[idx]["Tpd_PDLVHr"];
                                drEmpPayrollDtl["Tpd_PDLEGHOLHr"]                                   = drArrEmpPayrollDtl[idx]["Tpd_PDLEGHOLHr"];
                                drEmpPayrollDtl["Tpd_PDSPLHOLHr"]                                   = drArrEmpPayrollDtl[idx]["Tpd_PDSPLHOLHr"];
                                drEmpPayrollDtl["Tpd_PDCOMPHOLHr"]                                  = drArrEmpPayrollDtl[idx]["Tpd_PDCOMPHOLHr"];
                                drEmpPayrollDtl["Tpd_PDPSDHr"]                                      = drArrEmpPayrollDtl[idx]["Tpd_PDPSDHr"];
                                drEmpPayrollDtl["Tpd_PDOTHHOLHr"]                                   = drArrEmpPayrollDtl[idx]["Tpd_PDOTHHOLHr"];
                                drEmpPayrollDtl["Tpd_PDRESTLEGHOLHr"]                               = drArrEmpPayrollDtl[idx]["Tpd_PDRESTLEGHOLHr"];

                                if (strRegRule == "P")
                                {
                                    drEmpPayrollDtl["Tpd_REGHr"]                                    = drArrEmpPayrollDtl[idx]["Tpd_REGHr"];
                                    drEmpPayrollDtl["Tpd_TotalREGHr"]                               = Convert.ToDouble(drArrEmpPayrollDtl[idx]["Tpd_REGHr"])
                                                                                                            + Convert.ToDouble(drArrEmpPayrollDtl[idx]["Tpd_PDLVHr"])
                                                                                                            + Convert.ToDouble(drArrEmpPayrollDtl[idx]["Tpd_PDLEGHOLHr"])
                                                                                                            + Convert.ToDouble(drArrEmpPayrollDtl[idx]["Tpd_PDSPLHOLHr"])
                                                                                                            + Convert.ToDouble(drArrEmpPayrollDtl[idx]["Tpd_PDCOMPHOLHr"])
                                                                                                            + Convert.ToDouble(drArrEmpPayrollDtl[idx]["Tpd_PDPSDHr"])
                                                                                                            + Convert.ToDouble(drArrEmpPayrollDtl[idx]["Tpd_PDOTHHOLHr"])
                                                                                                            + Convert.ToDouble(drArrEmpPayrollDtl[idx]["Tpd_PDRESTLEGHOLHr"]);
                                }
                                else
                                {
                                    drEmpPayrollDtl["Tpd_REGHr"]                                    = 0;
                                    drEmpPayrollDtl["Tpd_TotalREGHr"]                               = 0;
                                }

                                drEmpPayrollDtl["Tpd_REGOTHr"]                                      = drArrEmpPayrollDtl[idx]["Tpd_REGOTHr"];
                                drEmpPayrollDtl["Tpd_REGNDHr"]                                      = drArrEmpPayrollDtl[idx]["Tpd_REGNDHr"];
                                drEmpPayrollDtl["Tpd_REGNDOTHr"]                                    = drArrEmpPayrollDtl[idx]["Tpd_REGNDOTHr"];
                                drEmpPayrollDtl["Tpd_RESTHr"]                                       = drArrEmpPayrollDtl[idx]["Tpd_RESTHr"];
                                drEmpPayrollDtl["Tpd_RESTOTHr"]                                     = drArrEmpPayrollDtl[idx]["Tpd_RESTOTHr"];
                                drEmpPayrollDtl["Tpd_RESTNDHr"]                                     = drArrEmpPayrollDtl[idx]["Tpd_RESTNDHr"];
                                drEmpPayrollDtl["Tpd_RESTNDOTHr"]                                   = drArrEmpPayrollDtl[idx]["Tpd_RESTNDOTHr"];
                                drEmpPayrollDtl["Tpd_LEGHOLHr"]                                     = drArrEmpPayrollDtl[idx]["Tpd_LEGHOLHr"];
                                drEmpPayrollDtl["Tpd_LEGHOLOTHr"]                                   = drArrEmpPayrollDtl[idx]["Tpd_LEGHOLOTHr"];
                                drEmpPayrollDtl["Tpd_LEGHOLNDHr"]                                   = drArrEmpPayrollDtl[idx]["Tpd_LEGHOLNDHr"];
                                drEmpPayrollDtl["Tpd_LEGHOLNDOTHr"]                                 = drArrEmpPayrollDtl[idx]["Tpd_LEGHOLNDOTHr"];
                                drEmpPayrollDtl["Tpd_SPLHOLHr"]                                     = drArrEmpPayrollDtl[idx]["Tpd_SPLHOLHr"];
                                drEmpPayrollDtl["Tpd_SPLHOLOTHr"]                                   = drArrEmpPayrollDtl[idx]["Tpd_SPLHOLOTHr"];
                                drEmpPayrollDtl["Tpd_SPLHOLNDHr"]                                   = drArrEmpPayrollDtl[idx]["Tpd_SPLHOLNDHr"];
                                drEmpPayrollDtl["Tpd_SPLHOLNDOTHr"]                                 = drArrEmpPayrollDtl[idx]["Tpd_SPLHOLNDOTHr"];
                                drEmpPayrollDtl["Tpd_PSDHr"]                                        = drArrEmpPayrollDtl[idx]["Tpd_PSDHr"];
                                drEmpPayrollDtl["Tpd_PSDOTHr"]                                      = drArrEmpPayrollDtl[idx]["Tpd_PSDOTHr"];
                                drEmpPayrollDtl["Tpd_PSDNDHr"]                                      = drArrEmpPayrollDtl[idx]["Tpd_PSDNDHr"];
                                drEmpPayrollDtl["Tpd_PSDNDOTHr"]                                    = drArrEmpPayrollDtl[idx]["Tpd_PSDNDOTHr"];
                                drEmpPayrollDtl["Tpd_COMPHOLHr"]                                    = drArrEmpPayrollDtl[idx]["Tpd_COMPHOLHr"];
                                drEmpPayrollDtl["Tpd_COMPHOLOTHr"]                                  = drArrEmpPayrollDtl[idx]["Tpd_COMPHOLOTHr"];
                                drEmpPayrollDtl["Tpd_COMPHOLNDHr"]                                  = drArrEmpPayrollDtl[idx]["Tpd_COMPHOLNDHr"];
                                drEmpPayrollDtl["Tpd_COMPHOLNDOTHr"]                                = drArrEmpPayrollDtl[idx]["Tpd_COMPHOLNDOTHr"];
                                drEmpPayrollDtl["Tpd_RESTLEGHOLHr"]                                 = drArrEmpPayrollDtl[idx]["Tpd_RESTLEGHOLHr"];
                                drEmpPayrollDtl["Tpd_RESTLEGHOLOTHr"]                               = drArrEmpPayrollDtl[idx]["Tpd_RESTLEGHOLOTHr"];
                                drEmpPayrollDtl["Tpd_RESTLEGHOLNDHr"]                               = drArrEmpPayrollDtl[idx]["Tpd_RESTLEGHOLNDHr"];
                                drEmpPayrollDtl["Tpd_RESTLEGHOLNDOTHr"]                             = drArrEmpPayrollDtl[idx]["Tpd_RESTLEGHOLNDOTHr"];
                                drEmpPayrollDtl["Tpd_RESTSPLHOLHr"]                                 = drArrEmpPayrollDtl[idx]["Tpd_RESTSPLHOLHr"];
                                drEmpPayrollDtl["Tpd_RESTSPLHOLOTHr"]                               = drArrEmpPayrollDtl[idx]["Tpd_RESTSPLHOLOTHr"];
                                drEmpPayrollDtl["Tpd_RESTSPLHOLNDHr"]                               = drArrEmpPayrollDtl[idx]["Tpd_RESTSPLHOLNDHr"];
                                drEmpPayrollDtl["Tpd_RESTSPLHOLNDOTHr"]                             = drArrEmpPayrollDtl[idx]["Tpd_RESTSPLHOLNDOTHr"];
                                drEmpPayrollDtl["Tpd_RESTCOMPHOLHr"]                                = drArrEmpPayrollDtl[idx]["Tpd_RESTCOMPHOLHr"];
                                drEmpPayrollDtl["Tpd_RESTCOMPHOLOTHr"]                              = drArrEmpPayrollDtl[idx]["Tpd_RESTCOMPHOLOTHr"];
                                drEmpPayrollDtl["Tpd_RESTCOMPHOLNDHr"]                              = drArrEmpPayrollDtl[idx]["Tpd_RESTCOMPHOLNDHr"];
                                drEmpPayrollDtl["Tpd_RESTCOMPHOLNDOTHr"]                            = drArrEmpPayrollDtl[idx]["Tpd_RESTCOMPHOLNDOTHr"];
                                drEmpPayrollDtl["Tpd_RESTPSDHr"]                                    = drArrEmpPayrollDtl[idx]["Tpd_RESTPSDHr"];
                                drEmpPayrollDtl["Tpd_RESTPSDOTHr"]                                  = drArrEmpPayrollDtl[idx]["Tpd_RESTPSDOTHr"];
                                drEmpPayrollDtl["Tpd_RESTPSDNDHr"]                                  = drArrEmpPayrollDtl[idx]["Tpd_RESTPSDNDHr"];
                                drEmpPayrollDtl["Tpd_RESTPSDNDOTHr"]                                = drArrEmpPayrollDtl[idx]["Tpd_RESTPSDNDOTHr"];
                                drEmpPayrollDtl["Usr_login"]                                        = LoginUser;
                                drEmpPayrollDtl["Ludatetime"]                                       = DateTime.Now;

                                drEmpPayrollDtl["Tpd_LTAmt"]                                        = 0;
                                drEmpPayrollDtl["Tpd_UTAmt"]                                        = 0;
                                drEmpPayrollDtl["Tpd_UPLVAmt"]                                      = 0;
                                drEmpPayrollDtl["Tpd_ABSLEGHOLAmt"]                                 = 0;
                                drEmpPayrollDtl["Tpd_ABSSPLHOLAmt"]                                 = 0;
                                drEmpPayrollDtl["Tpd_ABSCOMPHOLAmt"]                                = 0;
                                drEmpPayrollDtl["Tpd_ABSPSDAmt"]                                    = 0;
                                drEmpPayrollDtl["Tpd_ABSOTHHOLAmt"]                                 = 0;
                                drEmpPayrollDtl["Tpd_WDABSAmt"]                                     = 0;
                                drEmpPayrollDtl["Tpd_LTUTMaxAmt"]                                   = 0;
                                drEmpPayrollDtl["Tpd_ABSAmt"]                                       = 0;

                                drEmpPayrollDtl["Tpd_REGAmt"]                                       = 0;
                                drEmpPayrollDtl["Tpd_PDLVAmt"]                                      = 0;
                                drEmpPayrollDtl["Tpd_PDLEGHOLAmt"]                                  = 0;
                                drEmpPayrollDtl["Tpd_PDSPLHOLAmt"]                                  = 0;
                                drEmpPayrollDtl["Tpd_PDCOMPHOLAmt"]                                 = 0;
                                drEmpPayrollDtl["Tpd_PDPSDAmt"]                                     = 0;
                                drEmpPayrollDtl["Tpd_PDOTHHOLAmt"]                                  = 0;
                                drEmpPayrollDtl["Tpd_PDRESTLEGHOLAmt"]                              = 0;
                                drEmpPayrollDtl["Tpd_TotalREGAmt"]                                  = 0;

                                drEmpPayrollDtl["Tpd_REGOTAmt"]                                     = 0;
                                drEmpPayrollDtl["Tpd_REGNDAmt"]                                     = 0;
                                drEmpPayrollDtl["Tpd_REGNDOTAmt"]                                   = 0;

                                drEmpPayrollDtl["Tpd_RESTAmt"]                                      = 0;
                                drEmpPayrollDtl["Tpd_RESTOTAmt"]                                    = 0;
                                drEmpPayrollDtl["Tpd_RESTNDAmt"]                                    = 0;
                                drEmpPayrollDtl["Tpd_RESTNDOTAmt"]                                  = 0;

                                drEmpPayrollDtl["Tpd_LEGHOLAmt"]                                    = 0;
                                drEmpPayrollDtl["Tpd_LEGHOLOTAmt"]                                  = 0;
                                drEmpPayrollDtl["Tpd_LEGHOLNDAmt"]                                  = 0;
                                drEmpPayrollDtl["Tpd_LEGHOLNDOTAmt"]                                = 0;

                                drEmpPayrollDtl["Tpd_SPLHOLAmt"]                                    = 0;
                                drEmpPayrollDtl["Tpd_SPLHOLOTAmt"]                                  = 0;
                                drEmpPayrollDtl["Tpd_SPLHOLNDAmt"]                                  = 0;
                                drEmpPayrollDtl["Tpd_SPLHOLNDOTAmt"]                                = 0;

                                drEmpPayrollDtl["Tpd_PSDAmt"]                                       = 0;
                                drEmpPayrollDtl["Tpd_PSDOTAmt"]                                     = 0;
                                drEmpPayrollDtl["Tpd_PSDNDAmt"]                                     = 0;
                                drEmpPayrollDtl["Tpd_PSDNDOTAmt"]                                   = 0;

                                drEmpPayrollDtl["Tpd_COMPHOLAmt"]                                   = 0;
                                drEmpPayrollDtl["Tpd_COMPHOLOTAmt"]                                 = 0;
                                drEmpPayrollDtl["Tpd_COMPHOLNDAmt"]                                 = 0;
                                drEmpPayrollDtl["Tpd_COMPHOLNDOTAmt"]                               = 0;

                                drEmpPayrollDtl["Tpd_RESTLEGHOLAmt"]                                = 0;
                                drEmpPayrollDtl["Tpd_RESTLEGHOLOTAmt"]                              = 0;
                                drEmpPayrollDtl["Tpd_RESTLEGHOLNDAmt"]                              = 0;
                                drEmpPayrollDtl["Tpd_RESTLEGHOLNDOTAmt"]                            = 0;

                                drEmpPayrollDtl["Tpd_RESTSPLHOLAmt"]                                = 0;
                                drEmpPayrollDtl["Tpd_RESTSPLHOLOTAmt"]                              = 0;
                                drEmpPayrollDtl["Tpd_RESTSPLHOLNDAmt"]                              = 0;
                                drEmpPayrollDtl["Tpd_RESTSPLHOLNDOTAmt"]                            = 0;

                                drEmpPayrollDtl["Tpd_RESTCOMPHOLAmt"]                               = 0;
                                drEmpPayrollDtl["Tpd_RESTCOMPHOLOTAmt"]                             = 0;
                                drEmpPayrollDtl["Tpd_RESTCOMPHOLNDAmt"]                             = 0;
                                drEmpPayrollDtl["Tpd_RESTCOMPHOLNDOTAmt"]                           = 0;

                                drEmpPayrollDtl["Tpd_RESTPSDAmt"]                                   = 0;
                                drEmpPayrollDtl["Tpd_RESTPSDOTAmt"]                                 = 0;
                                drEmpPayrollDtl["Tpd_RESTPSDNDAmt"]                                 = 0;
                                drEmpPayrollDtl["Tpd_RESTPSDNDOTAmt"]                               = 0;
                                drEmpPayrollDtl["Tpd_TotalOTNDAmt"]                                 = 0;
                                #endregion

                                #region Initiaize Group Premium Variables
                                //Regular and Overtime Rates
                                Reg                                                                 = 0;
                                RegOT                                                               = 0;
                                Rest                                                                = 0;
                                RestOT                                                              = 0;
                                Hol                                                                 = 0;
                                HolOT                                                               = 0;
                                SPL                                                                 = 0;
                                SPLOT                                                               = 0;
                                PSD                                                                 = 0;
                                PSDOT                                                               = 0;
                                Comp                                                                = 0;
                                CompOT                                                              = 0;
                                RestHol                                                             = 0;
                                RestHolOT                                                           = 0;
                                RestSPL                                                             = 0;
                                RestSPLOT                                                           = 0;
                                RestComp                                                            = 0;
                                RestCompOT                                                          = 0;
                                RestPSD                                                             = 0;
                                RestPSDOT                                                           = 0;

                                //Regular Night Premium and Overtime Night Premium Rates
                                RegND                                                               = 0;
                                RegOTND                                                             = 0;
                                RestND                                                              = 0;
                                RestOTND                                                            = 0;
                                HolND                                                               = 0;
                                HolOTND                                                             = 0;
                                SPLND                                                               = 0;
                                SPLOTND                                                             = 0;
                                PSDND                                                               = 0;
                                PSDOTND                                                             = 0;
                                CompND                                                              = 0;
                                CompOTND                                                            = 0;
                                RestHolND                                                           = 0;
                                RestHolOTND                                                         = 0;
                                RestSPLND                                                           = 0;
                                RestSPLOTND                                                         = 0;
                                RestCompND                                                          = 0;
                                RestCompOTND                                                        = 0;
                                RestPSDND                                                           = 0;
                                RestPSDOTND                                                         = 0;

                                //Regular Night Premium and Overtime Night Premium Percentages
                                RegNDPercent                                                        = 0;
                                RegOTNDPercent                                                      = 0;
                                RestNDPercent                                                       = 0;
                                RestOTNDPercent                                                     = 0;
                                HolNDPercent                                                        = 0;
                                HolOTNDPercent                                                      = 0;
                                SPLNDPercent                                                        = 0;
                                SPLOTNDPercent                                                      = 0;
                                PSDNDPercent                                                        = 0;
                                PSDOTNDPercent                                                      = 0;
                                CompNDPercent                                                       = 0;
                                CompOTNDPercent                                                     = 0;
                                RestHolNDPercent                                                    = 0;
                                RestHolOTNDPercent                                                  = 0;
                                RestSPLNDPercent                                                    = 0;
                                RestSPLOTNDPercent                                                  = 0;
                                RestCompNDPercent                                                   = 0;
                                RestCompOTNDPercent                                                 = 0;
                                RestPSDNDPercent                                                    = 0;
                                RestPSDOTNDPercent                                                  = 0;

                                #endregion

                                #region Get Payroll data
                                SalaryRate = 0;
                                SalaryRate                                      = Convert.ToDecimal(drEmpPayrollDtl["Tpd_SalaryRate"]);
                                PayrollType                                     = drEmpPayrollDtl["Tpd_PayrollType"].ToString();

                                PremiumGroup                                    = drArrEmpPayrollDtl[idx]["Tpd_PremiumGrpCode"].ToString();  //TimeRegister
                                EmploymentStatus                                = drArrEmpPayrollDtl[idx]["Ttr_EmploymentStatusCode"].ToString();  //TimeRegister
                                MDIVISOR                                        = Convert.ToDecimal(GetScalarValue("Mpd_ParamValue", "M_PolicyDtl", string.Format("Mpd_PolicyCode = 'MDIVISOR' AND Mpd_SubCode = '{0}' AND Mpd_CompanyCode = '{1}' AND Mpd_RecordStatus = 'A'", EmploymentStatus, CompanyCode), dalHelper));

                                if (PayrollType == "" && PremiumGroup == "")
                                    throw new PayrollException("Employee Payroll Type and Premium Group is blank.");

                                #region Get Hourly Rate
                                HourlyRate                                      = 0;
                                HourlyRate                                      = GetHourlyRate(SalaryRate, PayrollType, MDIVISOR);
                                drEmpPayrollDtl["Tpd_HourRate"]                 = HourlyRate;
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
                                        SpecialSalaryRate = GetFormulaQueryDecimalValue(SPECIALRATEFORMULA_SRATE.Replace("@CENTRALDB", CentralProfile)
                                                                                        , paramDtl);

                                    if (!SPECIALRATEFORMULA_HRATE.Equals(""))
                                    {
                                        SpecialHourlyRate = GetFormulaQueryDecimalValue(SPECIALRATEFORMULA_HRATE.Replace("@CENTRALDB", CentralProfile)
                                                                                        , paramDtl);

                                        if (HRLYRTEDEC > 0)
                                            SpecialHourlyRate = Math.Round(SpecialHourlyRate, Convert.ToInt32(HRLYRTEDEC), MidpointRounding.AwayFromZero);
                                    }
                                    drEmpPayrollDtl["Tpd_SpecialRate"]          = SpecialSalaryRate;
                                    drEmpPayrollDtl["Tpd_SpecialHourRate"]      = SpecialHourlyRate;
                                }
                                #endregion

                                //for MULTSALFORMULA2
                                if (NewDailyRate != SalaryRate)
                                {
                                    NewDailyRate = SalaryRate;
                                    WorkingDayCntUsingNewRate = 0;
                                }
                                WorkingDayCntUsingNewRate                       += Convert.ToInt32(drArrEmpPayrollDtl[idx]["Tpd_WorkDay"]);
                                WorkingDayCnt                                   += Convert.ToInt32(drArrEmpPayrollDtl[idx]["Tpd_WorkDay"]);
                                
                                #endregion

                                #region Load Group Premiums
                                if (PayrollType != "" && PremiumGroup != "")
                                {
                                    GetPremium(dtDayCodePremiums, PayrollType, PremiumGroup, "REG", false, ref Reg, ref RegOT, ref RegND, ref RegOTND, ref RegNDPercent, ref RegOTNDPercent);
                                    GetPremium(dtDayCodePremiums, PayrollType, PremiumGroup, "REST", true, ref Rest, ref RestOT, ref RestND, ref RestOTND, ref RestNDPercent, ref RestOTNDPercent);
                                    GetPremium(dtDayCodePremiums, PayrollType, PremiumGroup, "SPL", false, ref SPL, ref SPLOT, ref SPLND, ref SPLOTND, ref SPLNDPercent, ref SPLOTNDPercent);
                                    GetPremium(dtDayCodePremiums, PayrollType, PremiumGroup, "HOL", false, ref Hol, ref HolOT, ref HolND, ref HolOTND, ref HolNDPercent, ref HolOTNDPercent);
                                    GetPremium(dtDayCodePremiums, PayrollType, PremiumGroup, "PSD", false, ref PSD, ref PSDOT, ref PSDND, ref PSDOTND, ref PSDNDPercent, ref PSDOTNDPercent);
                                    GetPremium(dtDayCodePremiums, PayrollType, PremiumGroup, "COMP", false, ref Comp, ref CompOT, ref CompND, ref CompOTND, ref CompNDPercent, ref CompOTNDPercent);
                                    GetPremium(dtDayCodePremiums, PayrollType, PremiumGroup, "SPL", true, ref RestSPL, ref RestSPLOT, ref RestSPLND, ref RestSPLOTND, ref RestSPLNDPercent, ref RestSPLOTNDPercent);
                                    GetPremium(dtDayCodePremiums, PayrollType, PremiumGroup, "HOL", true, ref RestHol, ref RestHolOT, ref RestHolND, ref RestHolOTND, ref RestHolNDPercent, ref RestHolOTNDPercent);
                                    GetPremium(dtDayCodePremiums, PayrollType, PremiumGroup, "PSD", true, ref RestPSD, ref RestPSDOT, ref RestPSDND, ref RestPSDOTND, ref RestPSDNDPercent, ref RestPSDOTNDPercent);
                                    GetPremium(dtDayCodePremiums, PayrollType, PremiumGroup, "COMP", true, ref RestComp, ref RestCompOT, ref RestCompND, ref RestCompOTND, ref RestCompNDPercent, ref RestCompOTNDPercent);
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
                                    //drEmpPayrollDtl["Tpd_ABSAmt"]             = Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSHr"]) * HourlyRate;
                                }

                                drEmpPayrollDtl["Tpd_ABSAmt"] = Convert.ToDecimal(drEmpPayrollDtl["Tpd_LTAmt"])
                                                                                    + Convert.ToDecimal(drEmpPayrollDtl["Tpd_UTAmt"])
                                                                                    + Convert.ToDecimal(drEmpPayrollDtl["Tpd_UPLVAmt"])
                                                                                    + Convert.ToDecimal(drEmpPayrollDtl["Tpd_WDABSAmt"])
                                                                                    + Convert.ToDecimal(drEmpPayrollDtl["Tpd_LTUTMaxAmt"])
                                                                                    + Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSLEGHOLAmt"])
                                                                                    + Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSSPLHOLAmt"])
                                                                                    + Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSCOMPHOLAmt"])
                                                                                    + Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSPSDAmt"])
                                                                                    + Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSOTHHOLAmt"]);

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

                                if (strRegRule == "P")
                                {
                                    drEmpPayrollDtl["Tpd_TotalREGAmt"]          = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_REGAmt"])
                                                                                    + Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDLVAmt"])
                                                                                    + Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDLEGHOLAmt"])
                                                                                    + Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDSPLHOLAmt"])
                                                                                    + Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDCOMPHOLAmt"])
                                                                                    + Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDOTHHOLAmt"])
                                                                                    + Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDRESTLEGHOLAmt"]), 2);
                                }

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

                                #endregion

                                #region Rounding Off of Dtl Amounts
                                drEmpPayrollDtl["Tpd_LTAmt"]                    = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_LTAmt"]), 2);
                                drEmpPayrollDtl["Tpd_UTAmt"]                    = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_UTAmt"]), 2);
                                drEmpPayrollDtl["Tpd_UPLVAmt"]                  = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_UPLVAmt"]), 2);
                                drEmpPayrollDtl["Tpd_ABSLEGHOLAmt"]             = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSLEGHOLAmt"]), 2);
                                drEmpPayrollDtl["Tpd_ABSSPLHOLAmt"]             = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSSPLHOLAmt"]), 2);
                                drEmpPayrollDtl["Tpd_ABSCOMPHOLAmt"]            = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSCOMPHOLAmt"]), 2);
                                drEmpPayrollDtl["Tpd_ABSPSDAmt"]                = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSPSDAmt"]), 2);
                                drEmpPayrollDtl["Tpd_ABSOTHHOLAmt"]             = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSOTHHOLAmt"]), 2);
                                drEmpPayrollDtl["Tpd_WDABSAmt"]                 = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_WDABSAmt"]), 2);
                                drEmpPayrollDtl["Tpd_LTUTMaxAmt"]               = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_LTUTMaxAmt"]), 2);
                                drEmpPayrollDtl["Tpd_ABSAmt"]                   = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSAmt"]), 2);
                                drEmpPayrollDtl["Tpd_REGAmt"]                   = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_REGAmt"]), 2);
                                drEmpPayrollDtl["Tpd_PDLVAmt"]                  = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDLVAmt"]), 2);
                                drEmpPayrollDtl["Tpd_PDLEGHOLAmt"]              = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDLEGHOLAmt"]), 2);
                                drEmpPayrollDtl["Tpd_PDSPLHOLAmt"]              = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDSPLHOLAmt"]), 2);
                                drEmpPayrollDtl["Tpd_PDCOMPHOLAmt"]             = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDCOMPHOLAmt"]), 2);
                                drEmpPayrollDtl["Tpd_PDPSDAmt"]                 = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDPSDAmt"]), 2);
                                drEmpPayrollDtl["Tpd_PDOTHHOLAmt"]              = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDOTHHOLAmt"]), 2);
                                drEmpPayrollDtl["Tpd_PDRESTLEGHOLAmt"]          = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDRESTLEGHOLAmt"]), 2);
                                drEmpPayrollDtl["Tpd_TotalREGAmt"]              = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_TotalREGAmt"]), 2);
                                drEmpPayrollDtl["Tpd_REGOTAmt"]                 = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_REGOTAmt"]), 2);
                                drEmpPayrollDtl["Tpd_REGNDAmt"]                 = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_REGNDAmt"]), 2);
                                drEmpPayrollDtl["Tpd_REGNDOTAmt"]               = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_REGNDOTAmt"]), 2);
                                drEmpPayrollDtl["Tpd_RESTAmt"]                  = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTAmt"]), 2);
                                drEmpPayrollDtl["Tpd_RESTOTAmt"]                = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTOTAmt"]), 2);
                                drEmpPayrollDtl["Tpd_RESTNDAmt"]                = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTNDAmt"]), 2);
                                drEmpPayrollDtl["Tpd_RESTNDOTAmt"]              = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTNDOTAmt"]), 2);
                                drEmpPayrollDtl["Tpd_LEGHOLAmt"]                = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_LEGHOLAmt"]), 2);
                                drEmpPayrollDtl["Tpd_LEGHOLOTAmt"]              = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_LEGHOLOTAmt"]), 2);
                                drEmpPayrollDtl["Tpd_LEGHOLNDAmt"]              = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_LEGHOLNDAmt"]), 2);
                                drEmpPayrollDtl["Tpd_LEGHOLNDOTAmt"]            = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_LEGHOLNDOTAmt"]), 2);
                                drEmpPayrollDtl["Tpd_SPLHOLAmt"]                = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_SPLHOLAmt"]), 2);
                                drEmpPayrollDtl["Tpd_SPLHOLOTAmt"]              = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_SPLHOLOTAmt"]), 2);
                                drEmpPayrollDtl["Tpd_SPLHOLNDAmt"]              = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_SPLHOLNDAmt"]), 2);
                                drEmpPayrollDtl["Tpd_SPLHOLNDOTAmt"]            = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_SPLHOLNDOTAmt"]), 2);
                                drEmpPayrollDtl["Tpd_PSDAmt"]                   = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_PSDAmt"]), 2);
                                drEmpPayrollDtl["Tpd_PSDOTAmt"]                 = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_PSDOTAmt"]), 2);
                                drEmpPayrollDtl["Tpd_PSDNDAmt"]                 = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_PSDNDAmt"]), 2);
                                drEmpPayrollDtl["Tpd_PSDNDOTAmt"]               = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_PSDNDOTAmt"]), 2);
                                drEmpPayrollDtl["Tpd_COMPHOLAmt"]               = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_COMPHOLAmt"]), 2);
                                drEmpPayrollDtl["Tpd_COMPHOLOTAmt"]             = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_COMPHOLOTAmt"]), 2);
                                drEmpPayrollDtl["Tpd_COMPHOLNDAmt"]             = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_COMPHOLNDAmt"]), 2);
                                drEmpPayrollDtl["Tpd_COMPHOLNDOTAmt"]           = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_COMPHOLNDOTAmt"]), 2);
                                drEmpPayrollDtl["Tpd_RESTLEGHOLAmt"]            = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTLEGHOLAmt"]), 2);
                                drEmpPayrollDtl["Tpd_RESTLEGHOLOTAmt"]          = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTLEGHOLOTAmt"]), 2);
                                drEmpPayrollDtl["Tpd_RESTLEGHOLNDAmt"]          = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTLEGHOLNDAmt"]), 2);
                                drEmpPayrollDtl["Tpd_RESTLEGHOLNDOTAmt"]        = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTLEGHOLNDOTAmt"]), 2);
                                drEmpPayrollDtl["Tpd_RESTSPLHOLAmt"]            = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTSPLHOLAmt"]), 2);
                                drEmpPayrollDtl["Tpd_RESTSPLHOLOTAmt"]          = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTSPLHOLOTAmt"]), 2);
                                drEmpPayrollDtl["Tpd_RESTSPLHOLNDAmt"]          = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTSPLHOLNDAmt"]), 2);
                                drEmpPayrollDtl["Tpd_RESTSPLHOLNDOTAmt"]        = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTSPLHOLNDOTAmt"]), 2);
                                drEmpPayrollDtl["Tpd_RESTCOMPHOLAmt"]           = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTCOMPHOLAmt"]), 2);
                                drEmpPayrollDtl["Tpd_RESTCOMPHOLOTAmt"]         = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTCOMPHOLOTAmt"]), 2);
                                drEmpPayrollDtl["Tpd_RESTCOMPHOLNDAmt"]         = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTCOMPHOLNDAmt"]), 2);
                                drEmpPayrollDtl["Tpd_RESTCOMPHOLNDOTAmt"]       = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTCOMPHOLNDOTAmt"]), 2);
                                drEmpPayrollDtl["Tpd_RESTPSDAmt"]               = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTPSDAmt"]), 2);
                                drEmpPayrollDtl["Tpd_RESTPSDOTAmt"]             = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTPSDOTAmt"]), 2);
                                drEmpPayrollDtl["Tpd_RESTPSDNDAmt"]             = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTPSDNDAmt"]), 2);
                                drEmpPayrollDtl["Tpd_RESTPSDNDOTAmt"]           = Math.Round(Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTPSDNDOTAmt"]), 2);

                                #endregion

                                #region Total OT ND Computation
                                drEmpPayrollDtl["Tpd_TotalOTNDAmt"] = Convert.ToDecimal(drEmpPayrollDtl["Tpd_REGOTAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_REGNDAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_REGNDOTAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTOTAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTNDAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTNDOTAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_LEGHOLAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_LEGHOLOTAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_LEGHOLNDAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_LEGHOLNDOTAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_SPLHOLAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_SPLHOLOTAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_SPLHOLNDAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_SPLHOLNDOTAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_PSDAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_PSDOTAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_PSDNDAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_PSDNDOTAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_COMPHOLAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_COMPHOLOTAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_COMPHOLNDAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_COMPHOLNDOTAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTLEGHOLAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTLEGHOLOTAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTLEGHOLNDAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTLEGHOLNDOTAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTSPLHOLAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTSPLHOLOTAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTSPLHOLNDAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTSPLHOLNDOTAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTCOMPHOLAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTCOMPHOLOTAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTCOMPHOLNDAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTCOMPHOLNDOTAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTPSDAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTPSDOTAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTPSDNDAmt"])
                                                                                + Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTPSDNDOTAmt"]);
                                #endregion

                                #region Add to Hdr Amounts
                                LateAmt                                     += Convert.ToDecimal(drEmpPayrollDtl["Tpd_LTAmt"]);
                                UndertimeAmt                                += Convert.ToDecimal(drEmpPayrollDtl["Tpd_UTAmt"]);
                                UnpaidLeaveAmt                              += Convert.ToDecimal(drEmpPayrollDtl["Tpd_UPLVAmt"]);
                                AbsentLegalHolidayAmt                       += Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSLEGHOLAmt"]);
                                AbsentSpecialHolidayAmt                     += Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSSPLHOLAmt"]);
                                AbsentCompanyHolidayAmt                     += Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSCOMPHOLAmt"]);
                                AbsentPlantShutdownAmt                      += Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSPSDAmt"]);
                                AbsentFillerHolidayAmt                      += Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSOTHHOLAmt"]);
                                WholeDayAbsentAmt                           += Convert.ToDecimal(drEmpPayrollDtl["Tpd_WDABSAmt"]);
                                LateUndertimeMaxAbsentAmt                   += Convert.ToDecimal(drEmpPayrollDtl["Tpd_LTUTMaxAmt"]);
                                AbsentAmt                                   += Convert.ToDecimal(drEmpPayrollDtl["Tpd_ABSAmt"]);

                                RegularAmt                                  += Convert.ToDecimal(drEmpPayrollDtl["Tpd_REGAmt"]);
                                PaidLeaveAmt                                += Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDLVAmt"]);

                                PaidLegalHolidayAmt                         += Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDLEGHOLAmt"]);
                                PaidSpecialHolidayAmt                       += Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDSPLHOLAmt"]);
                                PaidCompanyHolidayAmt                       += Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDCOMPHOLAmt"]);
                                PaidPlantShutdownHolidayAmt                 += Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDPSDAmt"]);
                                PaidFillerHolidayAmt                        += Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDOTHHOLAmt"]);
                                PaidRestdayLegalHolidayAmt                  += Convert.ToDecimal(drEmpPayrollDtl["Tpd_PDRESTLEGHOLAmt"]);

                                RegularOTAmt                                += Convert.ToDecimal(drEmpPayrollDtl["Tpd_REGOTAmt"]);
                                RegularNDAmt                                += Convert.ToDecimal(drEmpPayrollDtl["Tpd_REGNDAmt"]);
                                RegularOTNDAmt                              += Convert.ToDecimal(drEmpPayrollDtl["Tpd_REGNDOTAmt"]);

                                RestdayAmt                                  += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTAmt"]);
                                RestdayOTAmt                                += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTOTAmt"]);
                                RestdayNDAmt                                += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTNDAmt"]);
                                RestdayOTNDAmt                              += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTNDOTAmt"]);

                                LegalHolidayAmt                             += Convert.ToDecimal(drEmpPayrollDtl["Tpd_LEGHOLAmt"]);
                                LegalHolidayOTAmt                           += Convert.ToDecimal(drEmpPayrollDtl["Tpd_LEGHOLOTAmt"]);
                                LegalHolidayNDAmt                           += Convert.ToDecimal(drEmpPayrollDtl["Tpd_LEGHOLNDAmt"]);
                                LegalHolidayOTNDAmt                         += Convert.ToDecimal(drEmpPayrollDtl["Tpd_LEGHOLNDOTAmt"]);

                                SpecialHolidayAmt                           += Convert.ToDecimal(drEmpPayrollDtl["Tpd_SPLHOLAmt"]);
                                SpecialHolidayOTAmt                         += Convert.ToDecimal(drEmpPayrollDtl["Tpd_SPLHOLOTAmt"]);
                                SpecialHolidayNDAmt                         += Convert.ToDecimal(drEmpPayrollDtl["Tpd_SPLHOLNDAmt"]);
                                SpecialHolidayOTNDAmt                       += Convert.ToDecimal(drEmpPayrollDtl["Tpd_SPLHOLNDOTAmt"]);

                                PlantShutdownAmt                            += Convert.ToDecimal(drEmpPayrollDtl["Tpd_PSDAmt"]);
                                PlantShutdownOTAmt                          += Convert.ToDecimal(drEmpPayrollDtl["Tpd_PSDOTAmt"]);
                                PlantShutdownNDAmt                          += Convert.ToDecimal(drEmpPayrollDtl["Tpd_PSDNDAmt"]);
                                PlantShutdownOTNDAmt                        += Convert.ToDecimal(drEmpPayrollDtl["Tpd_PSDNDOTAmt"]);

                                CompanyHolidayAmt                           += Convert.ToDecimal(drEmpPayrollDtl["Tpd_COMPHOLAmt"]);
                                CompanyHolidayOTAmt                         += Convert.ToDecimal(drEmpPayrollDtl["Tpd_COMPHOLOTAmt"]);
                                CompanyHolidayNDAmt                         += Convert.ToDecimal(drEmpPayrollDtl["Tpd_COMPHOLNDAmt"]);
                                CompanyHolidayOTNDAmt                       += Convert.ToDecimal(drEmpPayrollDtl["Tpd_COMPHOLNDOTAmt"]);

                                RestdayLegalHolidayAmt                      += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTLEGHOLAmt"]);
                                RestdayLegalHolidayOTAmt                    += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTLEGHOLOTAmt"]);
                                RestdayLegalHolidayNDAmt                    += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTLEGHOLNDAmt"]);
                                RestdayLegalHolidayOTNDAmt                  += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTLEGHOLNDOTAmt"]);

                                RestdaySpecialHolidayAmt                    += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTSPLHOLAmt"]);
                                RestdaySpecialHolidayOTAmt                  += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTSPLHOLOTAmt"]);
                                RestdaySpecialHolidayNDAmt                  += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTSPLHOLNDAmt"]);
                                RestdaySpecialHolidayOTNDAmt                += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTSPLHOLNDOTAmt"]);

                                RestdayCompanyHolidayAmt                    += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTCOMPHOLAmt"]);
                                RestdayCompanyHolidayOTAmt                  += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTCOMPHOLOTAmt"]);
                                RestdayCompanyHolidayNDAmt                  += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTCOMPHOLNDAmt"]);
                                RestdayCompanyHolidayOTNDAmt                += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTCOMPHOLNDOTAmt"]);

                                RestdayPlantShutdownAmt                     += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTPSDAmt"]);
                                RestdayPlantShutdownOTAmt                   += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTPSDOTAmt"]);
                                RestdayPlantShutdownNDAmt                   += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTPSDNDAmt"]);
                                RestdayPlantShutdownOTNDAmt                 += Convert.ToDecimal(drEmpPayrollDtl["Tpd_RESTPSDNDOTAmt"]);

                                #endregion

                                #endregion

                                //Add to payroll tables
                                dtEmpPayrollDtl.Rows.Add(drEmpPayrollDtl);
                            }

                            if (bHasDayCodeExt)
                            {
                                drArrEmpPayrollDtlMisc = dtEmpPayTranDtlMisc.Select("Tpd_IDNo = '" + curEmployeeId + "'");
                                for (int idm = 0; idm < drArrEmpPayrollDtlMisc.Length; idm++)
                                {
                                    #region Initialize Payroll Dtl Misc Calc Row
                                    drEmpPayrollDtlMisc = dtEmpPayrollDtlMisc.NewRow();

                                    drEmpPayrollDtlMisc["Tpm_IDNo"]                         = drArrEmpPayrollDtlMisc[idm]["Tpd_IDNo"];
                                    drEmpPayrollDtlMisc["Tpm_PayCycle"]                     = drArrEmpPayrollDtlMisc[idm]["Tpd_PayCycle"];
                                    drEmpPayrollDtlMisc["Tpm_Date"]                         = drArrEmpPayrollDtlMisc[idm]["Tpd_Date"];
                                    drEmpPayrollDtlMisc["Tpm_Misc1Hr"]                      = drArrEmpPayrollDtlMisc[idm]["Tpd_Misc1Hr"];
                                    drEmpPayrollDtlMisc["Tpm_Misc1OTHr"]                    = drArrEmpPayrollDtlMisc[idm]["Tpd_Misc1OTHr"];
                                    drEmpPayrollDtlMisc["Tpm_Misc1NDHr"]                    = drArrEmpPayrollDtlMisc[idm]["Tpd_Misc1NDHr"];
                                    drEmpPayrollDtlMisc["Tpm_Misc1NDOTHr"]                  = drArrEmpPayrollDtlMisc[idm]["Tpd_Misc1NDOTHr"];
                                    drEmpPayrollDtlMisc["Tpm_Misc2Hr"]                      = drArrEmpPayrollDtlMisc[idm]["Tpd_Misc2Hr"];
                                    drEmpPayrollDtlMisc["Tpm_Misc2OTHr"]                    = drArrEmpPayrollDtlMisc[idm]["Tpd_Misc2OTHr"];
                                    drEmpPayrollDtlMisc["Tpm_Misc2NDHr"]                    = drArrEmpPayrollDtlMisc[idm]["Tpd_Misc2NDHr"];
                                    drEmpPayrollDtlMisc["Tpm_Misc2NDOTHr"]                  = drArrEmpPayrollDtlMisc[idm]["Tpd_Misc2NDOTHr"];
                                    drEmpPayrollDtlMisc["Tpm_Misc3Hr"]                      = drArrEmpPayrollDtlMisc[idm]["Tpd_Misc3Hr"];
                                    drEmpPayrollDtlMisc["Tpm_Misc3OTHr"]                    = drArrEmpPayrollDtlMisc[idm]["Tpd_Misc3OTHr"];
                                    drEmpPayrollDtlMisc["Tpm_Misc3NDHr"]                    = drArrEmpPayrollDtlMisc[idm]["Tpd_Misc3NDHr"];
                                    drEmpPayrollDtlMisc["Tpm_Misc3NDOTHr"]                  = drArrEmpPayrollDtlMisc[idm]["Tpd_Misc3NDOTHr"];
                                    drEmpPayrollDtlMisc["Tpm_Misc4Hr"]                      = drArrEmpPayrollDtlMisc[idm]["Tpd_Misc4Hr"];
                                    drEmpPayrollDtlMisc["Tpm_Misc4OTHr"]                    = drArrEmpPayrollDtlMisc[idm]["Tpd_Misc4OTHr"];
                                    drEmpPayrollDtlMisc["Tpm_Misc4NDHr"]                    = drArrEmpPayrollDtlMisc[idm]["Tpd_Misc4NDHr"];
                                    drEmpPayrollDtlMisc["Tpm_Misc4NDOTHr"]                  = drArrEmpPayrollDtlMisc[idm]["Tpd_Misc4NDOTHr"];
                                    drEmpPayrollDtlMisc["Tpm_Misc5Hr"]                      = drArrEmpPayrollDtlMisc[idm]["Tpd_Misc5Hr"];
                                    drEmpPayrollDtlMisc["Tpm_Misc5OTHr"]                    = drArrEmpPayrollDtlMisc[idm]["Tpd_Misc5OTHr"];
                                    drEmpPayrollDtlMisc["Tpm_Misc5NDHr"]                    = drArrEmpPayrollDtlMisc[idm]["Tpd_Misc5NDHr"];
                                    drEmpPayrollDtlMisc["Tpm_Misc5NDOTHr"]                  = drArrEmpPayrollDtlMisc[idm]["Tpd_Misc5NDOTHr"];
                                    drEmpPayrollDtlMisc["Tpm_Misc6Hr"]                      = drArrEmpPayrollDtlMisc[idm]["Tpd_Misc6Hr"];
                                    drEmpPayrollDtlMisc["Tpm_Misc6OTHr"]                    = drArrEmpPayrollDtlMisc[idm]["Tpd_Misc6OTHr"];
                                    drEmpPayrollDtlMisc["Tpm_Misc6NDHr"]                    = drArrEmpPayrollDtlMisc[idm]["Tpd_Misc6NDHr"];
                                    drEmpPayrollDtlMisc["Tpm_Misc6NDOTHr"]                  = drArrEmpPayrollDtlMisc[idm]["Tpd_Misc6NDOTHr"];
                                    drEmpPayrollDtlMisc["Usr_login"]                        = LoginUser;
                                    drEmpPayrollDtlMisc["Ludatetime"]                       = DateTime.Now;

                                    drEmpPayrollDtlMisc["Tpm_Misc1Amt"]                     = 0;
                                    drEmpPayrollDtlMisc["Tpm_Misc1OTAmt"]                   = 0;
                                    drEmpPayrollDtlMisc["Tpm_Misc1NDAmt"]                   = 0;
                                    drEmpPayrollDtlMisc["Tpm_Misc1NDOTAmt"]                 = 0;
                                    drEmpPayrollDtlMisc["Tpm_Misc2Amt"]                     = 0;
                                    drEmpPayrollDtlMisc["Tpm_Misc2OTAmt"]                   = 0;
                                    drEmpPayrollDtlMisc["Tpm_Misc2NDAmt"]                   = 0;
                                    drEmpPayrollDtlMisc["Tpm_Misc2NDOTAmt"]                 = 0;
                                    drEmpPayrollDtlMisc["Tpm_Misc3Amt"]                     = 0;
                                    drEmpPayrollDtlMisc["Tpm_Misc3OTAmt"]                   = 0;
                                    drEmpPayrollDtlMisc["Tpm_Misc3NDAmt"]                   = 0;
                                    drEmpPayrollDtlMisc["Tpm_Misc3NDOTAmt"]                 = 0;
                                    drEmpPayrollDtlMisc["Tpm_Misc4Amt"]                     = 0;
                                    drEmpPayrollDtlMisc["Tpm_Misc4OTAmt"]                   = 0;
                                    drEmpPayrollDtlMisc["Tpm_Misc4NDAmt"]                   = 0;
                                    drEmpPayrollDtlMisc["Tpm_Misc4NDOTAmt"]                 = 0;
                                    drEmpPayrollDtlMisc["Tpm_Misc5Amt"]                     = 0;
                                    drEmpPayrollDtlMisc["Tpm_Misc5OTAmt"]                   = 0;
                                    drEmpPayrollDtlMisc["Tpm_Misc5NDAmt"]                   = 0;
                                    drEmpPayrollDtlMisc["Tpm_Misc5NDOTAmt"]                 = 0;
                                    drEmpPayrollDtlMisc["Tpm_Misc6Amt"]                     = 0;
                                    drEmpPayrollDtlMisc["Tpm_Misc6OTAmt"]                   = 0;
                                    drEmpPayrollDtlMisc["Tpm_Misc6NDAmt"]                   = 0;
                                    drEmpPayrollDtlMisc["Tpm_Misc6NDOTAmt"]                 = 0;

                                    #endregion

                                    #region Get Payroll data
                                    SalaryRate                                              = 0;
                                    SalaryRate                                              = Convert.ToDecimal(drArrEmpPayrollDtlMisc[idm]["Tpd_SalaryRate"]);
                                    PayrollType                                             = drArrEmpPayrollDtlMisc[idm]["Tpd_PayrollType"].ToString();
                                    PremiumGroup                                            = drArrEmpPayrollDtl[idm]["Tpd_PremiumGrpCode"].ToString();
                                    EmploymentStatus                                        = drArrEmpPayrollDtl[idm]["Ttr_EmploymentStatusCode"].ToString(); //TimeRegister
                                    MDIVISOR                                                = Convert.ToDecimal(GetScalarValue("Mpd_ParamValue", "M_PolicyDtl", string.Format("Mpd_PolicyCode = 'MDIVISOR' AND Mpd_SubCode = '{0}' AND Mpd_CompanyCode = '{1}' AND Mpd_RecordStatus = 'A'", EmploymentStatus, CompanyCode), dalHelper));

                                    #endregion

                                    #region Get Hourly Rate
                                    HourlyRate = 0;
                                    HourlyRate = GetHourlyRate(SalaryRate, PayrollType, MDIVISOR);
                                    #endregion

                                    #region Get Special Hourly Rate
                                    if (Convert.ToBoolean(PAYSPECIALRATE))
                                    {
                                        SpecialHourlyRate = 0;
                                        int idxxExt = 0;
                                        ParameterInfo[] paramDtlMisc = new ParameterInfo[8];
                                        paramDtlMisc[idxxExt++] = new ParameterInfo("@IDNUMBER", curEmployeeId);
                                        paramDtlMisc[idxxExt++] = new ParameterInfo("@PAYROLLTYPE", PayrollType);
                                        paramDtlMisc[idxxExt++] = new ParameterInfo("@MDIVISOR", MDIVISOR);
                                        paramDtlMisc[idxxExt++] = new ParameterInfo("@HDIVISOR", CommonBL.HOURSINDAY);
                                        paramDtlMisc[idxxExt++] = new ParameterInfo("@PAYCYCLE", PayrollPeriod);
                                        paramDtlMisc[idxxExt++] = new ParameterInfo("@STARTCYCLE", PayrollStart, SqlDbType.Date);
                                        paramDtlMisc[idxxExt++] = new ParameterInfo("@ENDCYCLE", PayrollEnd, SqlDbType.Date);
                                        paramDtlMisc[idxxExt++] = new ParameterInfo("@COMPANYCODE", CompanyCode);

                                        if (!SPECIALRATEFORMULA_HRATE.Equals(""))
                                        {
                                            SpecialHourlyRate = GetFormulaQueryDecimalValue(SPECIALRATEFORMULA_HRATE.Replace("@CENTRALDB", CentralProfile), paramDtlMisc);

                                            if (HRLYRTEDEC > 0)
                                                SpecialHourlyRate = Math.Round(SpecialHourlyRate, Convert.ToInt32(HRLYRTEDEC), MidpointRounding.AwayFromZero);
                                        }
                                    }
                                    #endregion

                                    #region Compute Hour Ext Premiums
                                    drArrEmpPayrollDtlMisc = dtEmpPayTranDtlMisc.Select("Tpd_IDNo = '" + curEmployeeId + "'");
                                    drDayCodePremiumFiller = dtDayCodePremiumFillers.Select(string.Format("Mdp_PremiumGrpCode = '{0}' AND Mdp_PayrollType = '{1}'", PremiumGroup, PayrollType));
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
                                                switch(GetValue(drDayCodePremiumFiller[premFill]["Mmd_MiscDayID"]))
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
                                        throw new PayrollException("Day Premium Group [" + PremiumGroup + "] has No Premium for " + PayrollType + " Payroll Type");
                                    #endregion

                                    #region Add To Total OT ND
                                    DataRow[] drTotalOT = dtEmpPayrollDtl.Select("Tpd_IDNo = '" + curEmployeeId + "' AND Tpd_Date = '" + drEmpPayrollDtlMisc["Tpm_Date"].ToString() + "'");
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
                                    #endregion

                                    #region Rounding Off of Dtl Ext Amounts
                                    drEmpPayrollDtlMisc["Tpm_Misc1Amt"]     = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc1Amt"]), 2);
                                    drEmpPayrollDtlMisc["Tpm_Misc1OTAmt"]   = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc1OTAmt"]), 2);
                                    drEmpPayrollDtlMisc["Tpm_Misc1NDAmt"]   = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc1NDAmt"]), 2);
                                    drEmpPayrollDtlMisc["Tpm_Misc1NDOTAmt"] = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc1NDOTAmt"]), 2);
                                    drEmpPayrollDtlMisc["Tpm_Misc2Amt"]     = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc2Amt"]), 2);
                                    drEmpPayrollDtlMisc["Tpm_Misc2OTAmt"]   = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc2OTAmt"]), 2);
                                    drEmpPayrollDtlMisc["Tpm_Misc2NDAmt"]   = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc2NDAmt"]), 2);
                                    drEmpPayrollDtlMisc["Tpm_Misc2NDOTAmt"] = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc2NDOTAmt"]), 2);
                                    drEmpPayrollDtlMisc["Tpm_Misc3Amt"]     = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc3Amt"]), 2);
                                    drEmpPayrollDtlMisc["Tpm_Misc3OTAmt"]   = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc3OTAmt"]), 2);
                                    drEmpPayrollDtlMisc["Tpm_Misc3NDAmt"]   = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc3NDAmt"]), 2);
                                    drEmpPayrollDtlMisc["Tpm_Misc3NDOTAmt"] = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc3NDOTAmt"]), 2);
                                    drEmpPayrollDtlMisc["Tpm_Misc4Amt"]     = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc4Amt"]), 2);
                                    drEmpPayrollDtlMisc["Tpm_Misc4OTAmt"]   = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc4OTAmt"]), 2);
                                    drEmpPayrollDtlMisc["Tpm_Misc4NDAmt"]   = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc4NDAmt"]), 2);
                                    drEmpPayrollDtlMisc["Tpm_Misc4NDOTAmt"] = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc4NDOTAmt"]), 2);
                                    drEmpPayrollDtlMisc["Tpm_Misc5Amt"]     = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc5Amt"]), 2);
                                    drEmpPayrollDtlMisc["Tpm_Misc5OTAmt"]   = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc5OTAmt"]), 2);
                                    drEmpPayrollDtlMisc["Tpm_Misc5NDAmt"]   = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc5NDAmt"]), 2);
                                    drEmpPayrollDtlMisc["Tpm_Misc5NDOTAmt"] = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc5NDOTAmt"]), 2);
                                    drEmpPayrollDtlMisc["Tpm_Misc6Amt"]     = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc6Amt"]), 2);
                                    drEmpPayrollDtlMisc["Tpm_Misc6OTAmt"]   = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc6OTAmt"]), 2);
                                    drEmpPayrollDtlMisc["Tpm_Misc6NDAmt"]   = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc6NDAmt"]), 2);
                                    drEmpPayrollDtlMisc["Tpm_Misc6NDOTAmt"] = Math.Round(Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc6NDOTAmt"]), 2);

                                    #endregion

                                    #region Add to Hdr Ext Amounts
                                    Misc1Amt                                += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc1Amt"]);
                                    Misc1OTAmt                              += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc1OTAmt"]);
                                    Misc1NDAmt                              += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc1NDAmt"]);
                                    Misc1NDOTAmt                            += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc1NDOTAmt"]);
                                    Misc2Amt                                += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc2Amt"]);
                                    Misc2OTAmt                              += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc2OTAmt"]);
                                    Misc2NDAmt                              += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc2NDAmt"]);
                                    Misc2NDOTAmt                            += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc2NDOTAmt"]);
                                    Misc3Amt                                += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc3Amt"]);
                                    Misc3OTAmt                              += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc3OTAmt"]);
                                    Misc3NDAmt                              += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc3NDAmt"]);
                                    Misc3NDOTAmt                            += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc3NDOTAmt"]);
                                    Misc4Amt                                += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc4Amt"]);
                                    Misc4OTAmt                              += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc4OTAmt"]);
                                    Misc4NDAmt                              += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc4NDAmt"]);
                                    Misc4NDOTAmt                            += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc4NDOTAmt"]);
                                    Misc5Amt                                += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc5Amt"]);
                                    Misc5OTAmt                              += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc5OTAmt"]);
                                    Misc5NDAmt                              += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc5NDAmt"]);
                                    Misc5NDOTAmt                            += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc5NDOTAmt"]);
                                    Misc6Amt                                += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc6Amt"]);
                                    Misc6OTAmt                              += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc6OTAmt"]);
                                    Misc6NDAmt                              += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc6NDAmt"]);
                                    Misc6NDOTAmt                            += Convert.ToDecimal(drEmpPayrollDtlMisc["Tpm_Misc6NDOTAmt"]);
                                    #endregion

                                    dtEmpPayrollDtlMisc.Rows.Add(drEmpPayrollDtlMisc);
                                }

                            }

                            #endregion

                            #region Payroll Calc Hdr

                            #region Get Payroll data
                            SalaryRate = 0;
                            SalaryRate = Convert.ToDecimal(drEmpPayroll["Tpy_SalaryRate"]);
                            PayrollType = drEmpPayroll["Tpy_PayrollType"].ToString();
                            PremiumGroup = drEmpPayroll["Tpy_PremiumGrpCode"].ToString();
                            EmploymentStatus = drEmpPayroll["Tpy_EmploymentStatus"].ToString();
                            MDIVISOR = Convert.ToDecimal(GetScalarValue("Mpd_ParamValue", "M_PolicyDtl", string.Format("Mpd_PolicyCode = 'MDIVISOR' AND Mpd_SubCode = '{0}' AND Mpd_CompanyCode = '{1}' AND Mpd_RecordStatus = 'A'", EmploymentStatus, CompanyCode), dalHelper));

                            #region Get Hourly Rate
                            HourlyRate = 0;
                            HourlyRate = GetHourlyRate(SalaryRate, PayrollType, MDIVISOR);
                            drEmpPayroll["Tpy_HourRate"] = HourlyRate;
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
                                drEmpPayroll["Tpy_SpecialRate"] = SpecialSalaryRate;
                                drEmpPayroll["Tpy_SpecialHourRate"] = SpecialHourlyRate;
                            }
                            #endregion

                            #endregion

                            #region Compute Hour Amounts

                            #region Compute hour premiums

                            #region Absent Amount Computation
                            drEmpPayroll["Tpy_UPLVAmt"]                                 = UnpaidLeaveAmt;
                            drEmpPayroll["Tpy_ABSLEGHOLAmt"]                            = AbsentLegalHolidayAmt;
                            drEmpPayroll["Tpy_ABSSPLHOLAmt"]                            = AbsentSpecialHolidayAmt;
                            drEmpPayroll["Tpy_ABSCOMPHOLAmt"]                           = AbsentCompanyHolidayAmt;
                            drEmpPayroll["Tpy_ABSPSDAmt"]                               = AbsentPlantShutdownAmt;
                            drEmpPayroll["Tpy_ABSOTHHOLAmt"]                            = AbsentFillerHolidayAmt;
                            drEmpPayroll["Tpy_WDABSAmt"]                                = WholeDayAbsentAmt;
                            drEmpPayroll["Tpy_LTUTMaxAmt"]                              = LateUndertimeMaxAbsentAmt; 

                            if (Convert.ToBoolean(PAYSPECIALRATE))
                            {
                                if (brLateHr > 0)
                                    drEmpPayroll["Tpy_LTAmt"] = brLateHr * (LTRate == "S" ? SpecialHourlyRate : HourlyRate);
                                else
                                    drEmpPayroll["Tpy_LTAmt"] = LateAmt;

                                if (brUndertimeHr > 0)
                                    drEmpPayroll["Tpy_UTAmt"] = brUndertimeHr * (UTRate == "S" ? SpecialHourlyRate : HourlyRate);
                                else
                                    drEmpPayroll["Tpy_UTAmt"] = UndertimeAmt;

                                if ((brLateHr + brUndertimeHr) > 0)
                                    drEmpPayroll["Tpy_ABSAmt"] = Convert.ToDecimal(drEmpPayroll["Tpy_LTAmt"])
                                                                                    + Convert.ToDecimal(drEmpPayroll["Tpy_UTAmt"])
                                                                                    + Convert.ToDecimal(drEmpPayroll["Tpy_UPLVAmt"])
                                                                                    + Convert.ToDecimal(drEmpPayroll["Tpy_WDABSAmt"])
                                                                                    + Convert.ToDecimal(drEmpPayroll["Tpy_LTUTMaxAmt"]);
                                else
                                    drEmpPayroll["Tpy_ABSAmt"] = AbsentAmt;
                            }
                            else
                            {
                                if (brLateHr > 0)
                                    drEmpPayroll["Tpy_LTAmt"] = brLateHr * HourlyRate;
                                else
                                    drEmpPayroll["Tpy_LTAmt"] = LateAmt;

                                if (brUndertimeHr > 0)
                                    drEmpPayroll["Tpy_UTAmt"] = brUndertimeHr * HourlyRate;
                                else
                                    drEmpPayroll["Tpy_UTAmt"] = UndertimeAmt;

                                if ((brLateHr + brUndertimeHr) > 0)
                                    drEmpPayroll["Tpy_ABSAmt"] = Convert.ToDecimal(drEmpPayroll["Tpy_ABSHr"]) * HourlyRate;
                                else
                                    drEmpPayroll["Tpy_ABSAmt"] = AbsentAmt;
                            }
                            #endregion

                            #region Regular Amount Computation
                            drEmpPayroll["Tpy_REGAmt"]                                  = RegularAmt;
                            drEmpPayroll["Tpy_PDLVAmt"]                                 = PaidLeaveAmt;

                            drEmpPayroll["Tpy_PDLEGHOLAmt"]                             = PaidLegalHolidayAmt;
                            drEmpPayroll["Tpy_PDSPLHOLAmt"]                             = PaidSpecialHolidayAmt;
                            drEmpPayroll["Tpy_PDCOMPHOLAmt"]                            = PaidCompanyHolidayAmt;
                            drEmpPayroll["Tpy_PDPSDAmt"]                                = PaidPlantShutdownHolidayAmt;
                            drEmpPayroll["Tpy_PDOTHHOLAmt"]                             = PaidFillerHolidayAmt;
                            drEmpPayroll["Tpy_PDRESTLEGHOLAmt"]                         = PaidRestdayLegalHolidayAmt;
                            #endregion

                            #region Total Regular Amount Computation

                            if (!ProcessSeparated)
                            {
                                RegularPayAmt = 0;
                                if (drEmpPayroll["Tpy_RegRule"].ToString() == "P")
                                {
                                    drEmpPayroll["Tpy_TotalREGAmt"]                     = Math.Round(RegularAmt
                                                                                            + PaidLeaveAmt
                                                                                            + PaidLegalHolidayAmt
                                                                                            + PaidSpecialHolidayAmt
                                                                                            + PaidCompanyHolidayAmt
                                                                                            + PaidFillerHolidayAmt
                                                                                            + PaidRestdayLegalHolidayAmt, 2);
                                    RegularPayAmt                                       = Convert.ToDecimal(drEmpPayroll["Tpy_TotalREGAmt"]);
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

                                        OldDailyRate = Math.Round(Convert.ToDecimal(drArrEmpPayrollDtl[0]["Tpd_SalaryRate"]) * 12 / MDIVISOR, 2, MidpointRounding.AwayFromZero);
                                        NewDailyRate = Math.Round(Convert.ToDecimal(drArrEmpPayrollDtl[drArrEmpPayrollDtl.Length - 1]["Tpd_SalaryRate"]) * 12 / MDIVISOR, 2, MidpointRounding.AwayFromZero);

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

                                        drEmpPayroll["Tpy_DayCountOldSalaryRate"] = (WorkingDayCnt - WorkingDayCntUsingNewRate); //NO OF DAYS USING OLD DAILY RATE
                                        RegularPayAmt = Convert.ToDecimal(drEmpPayroll["Tpy_TotalREGAmt"]);
                                        drEmpPayroll["Tpy_IsMultipleSalary"] = true;
                                        #endregion
                                    }
                                    else
                                    {
                                        #region Normal Flow
                                        RegularPayAmt                                           = Math.Round((SalaryRate / 2) - Convert.ToDecimal(drEmpPayroll["Tpy_ABSAmt"]), 2);

                                        //Set Regular Amt as balancing factor
                                        drEmpPayroll["Tpy_REGAmt"]                              = 0;
                                        drEmpPayroll["Tpy_REGAmt"]                              = RegularPayAmt
                                                                                                        - Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_PDLEGHOLAmt"]), 2)
                                                                                                        - Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_PDSPLHOLAmt"]), 2)
                                                                                                        - Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_PDCOMPHOLAmt"]), 2)
                                                                                                        - Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_PDPSDAmt"]), 2)
                                                                                                        - Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_PDOTHHOLAmt"]), 2)
                                                                                                        - Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_PDLVAmt"]), 2);

                                        RegularPayAmt                                           += Convert.ToDecimal(drEmpPayroll["Tpy_PDRESTLEGHOLAmt"]);

                                        if (Convert.ToDecimal(drEmpPayroll["Tpy_REGAmt"]) < 0)
                                            drEmpPayroll["Tpy_REGAmt"] = 0;

                                        drEmpPayroll["Tpy_TotalREGAmt"]                         = RegularPayAmt;

                                        #endregion
                                    }

                                    #region Cleanup for Regular Amount Considerations 
                                    // - Sum of Regular Hours is 0 (possibly on Leave for the whole quincena)
                                    // - Regular Amount computed above is 0 (more absences than the present days/half-month pay)
                                    if ((PayrollType == "M" && (Convert.ToDecimal(drEmpPayroll["Tpy_REGHr"]) + Convert.ToDecimal(drEmpPayroll["Tpy_PDLVHr"])) == 0) 
                                        || Convert.ToDecimal(drEmpPayroll["Tpy_TotalREGAmt"]) < 0)
                                    {
                                        RegularPayAmt                                           = 0;
                                        drEmpPayroll["Tpy_TotalREGAmt"]                         = 0;
                                        drEmpPayroll["Tpy_TotalREGHr"]                          = 0;

                                        drEmpPayroll["Tpy_REGAmt"]                              = 0;
                                        drEmpPayroll["Tpy_PDLVAmt"]                             = 0;

                                        drEmpPayroll["Tpy_PDLEGHOLHr"]                          = 0;
                                        drEmpPayroll["Tpy_PDSPLHOLHr"]                          = 0;
                                        drEmpPayroll["Tpy_PDCOMPHOLHr"]                         = 0;
                                        drEmpPayroll["Tpy_PDPSDHr"]                             = 0;
                                        drEmpPayroll["Tpy_PDOTHHOLHr"]                          = 0;

                                        drEmpPayroll["Tpy_PDLEGHOLAmt"]                         = 0;
                                        drEmpPayroll["Tpy_PDSPLHOLAmt"]                         = 0;
                                        drEmpPayroll["Tpy_PDCOMPHOLAmt"]                        = 0;
                                        drEmpPayroll["Tpy_PDPSDAmt"]                            = 0;
                                        drEmpPayroll["Tpy_PDOTHHOLAmt"]                         = 0;

                                        drEmpPayroll["Tpy_PDRESTLEGHOLHr"]                      = 0;
                                        drEmpPayroll["Tpy_PDRESTLEGHOLAmt"]                     = 0;
                                    }
                                    #endregion
                                }

                                //Auto-correct Regular Amount if it is greater than Total Regular Amount
                                if (Convert.ToDecimal(drEmpPayroll["Tpy_REGAmt"]) > RegularPayAmt)
                                    drEmpPayroll["Tpy_REGAmt"] = RegularPayAmt;
                            }
                            else if (ProcessSeparated)
                            {
                                #region Separated
                                RegularPayAmt = 0;
                                if (PayrollType == "M")
                                {
                                    if (PayRule == "A") //Hours Absent
                                    {
                                        if ((Convert.ToDecimal(drEmpPayroll["Tpy_TotalREGHr"])) == 0)
                                            RegularPayAmt = 0;
                                        else
                                            RegularPayAmt = Math.Round((SalaryRate / 2) - Convert.ToDecimal(drEmpPayroll["Tpy_ABSAmt"]), 2);

                                        //Set Regular Amt as balancing factor
                                        drEmpPayroll["Tpy_REGAmt"]              = RegularPayAmt
                                                                                    - Convert.ToDecimal(drEmpPayroll["Tpy_PDLEGHOLAmt"])
                                                                                    - Convert.ToDecimal(drEmpPayroll["Tpy_PDSPLHOLAmt"])
                                                                                    - Convert.ToDecimal(drEmpPayroll["Tpy_PDCOMPHOLAmt"])
                                                                                    - Convert.ToDecimal(drEmpPayroll["Tpy_PDPSDAmt"])
                                                                                    - Convert.ToDecimal(drEmpPayroll["Tpy_PDOTHHOLAmt"])
                                                                                    - Convert.ToDecimal(drEmpPayroll["Tpy_PDLVAmt"]);

                                        RegularPayAmt += Convert.ToDecimal(drEmpPayroll["Tpy_PDRESTLEGHOLAmt"]);

                                        if (Convert.ToDecimal(drEmpPayroll["Tpy_REGAmt"]) < 0)
                                            drEmpPayroll["Tpy_REGAmt"] = 0;
                                    }
                                    else if (PayRule == "P") //Computed as Daily Rate = Hours Present
                                    {
                                        RegularPayAmt                           = Math.Round(RegularAmt
                                                                                                + PaidLeaveAmt
                                                                                                + PaidLegalHolidayAmt
                                                                                                + PaidSpecialHolidayAmt
                                                                                                + PaidCompanyHolidayAmt
                                                                                                + PaidFillerHolidayAmt
                                                                                                + PaidRestdayLegalHolidayAmt, 2);
                                    }
                                }
                                else if (PayrollType == "D")
                                {
                                    RegularPayAmt                               = Math.Round(RegularAmt
                                                                                            + PaidLeaveAmt
                                                                                            + PaidLegalHolidayAmt
                                                                                            + PaidSpecialHolidayAmt
                                                                                            + PaidCompanyHolidayAmt
                                                                                            + PaidFillerHolidayAmt
                                                                                            + PaidRestdayLegalHolidayAmt, 2);
                                }

                                if (PayrollType != "M" || PayRule != "A")
                                {
                                    #region Zero-out Absent Hours and Amounts
                                    drEmpPayroll["Tpy_ABSHr"]                                       = 0;
                                    drEmpPayroll["Tpy_ABSAmt"]                                      = 0;

                                    drEmpPayroll["Tpy_LTHr"]                                        = 0;
                                    drEmpPayroll["Tpy_LTAmt"]                                       = 0;

                                    drEmpPayroll["Tpy_UTHr"]                                        = 0;
                                    drEmpPayroll["Tpy_UTAmt"]                                       = 0;

                                    drEmpPayroll["Tpy_UPLVHr"]                                      = 0;
                                    drEmpPayroll["Tpy_UPLVAmt"]                                     = 0;

                                    drEmpPayroll["Tpy_WDABSHr"]                                     = 0;
                                    drEmpPayroll["Tpy_WDABSAmt"]                                    = 0;

                                    drEmpPayroll["Tpy_LTUTMaxHr"]                                   = 0;
                                    drEmpPayroll["Tpy_LTUTMaxAmt"]                                  = 0;

                                    drEmpPayroll["Tpy_ABSLEGHOLHr"]                                 = 0;
                                    drEmpPayroll["Tpy_ABSLEGHOLAmt"]                                = 0;

                                    drEmpPayroll["Tpy_ABSSPLHOLHr"]                                 = 0;
                                    drEmpPayroll["Tpy_ABSSPLHOLAmt"]                                = 0;

                                    drEmpPayroll["Tpy_ABSCOMPHOLHr"]                                = 0;
                                    drEmpPayroll["Tpy_ABSCOMPHOLAmt"]                               = 0;

                                    drEmpPayroll["Tpy_ABSPSDHr"]                                    = 0;
                                    drEmpPayroll["Tpy_ABSPSDAmt"]                                   = 0;

                                    drEmpPayroll["Tpy_ABSOTHHOLHr"]                                 = 0;
                                    drEmpPayroll["Tpy_ABSOTHHOLAmt"]                                = 0;
                                    #endregion
                                }

                                if (PayRule == "Z")
                                {
                                    #region Zero-out All Amounts
                                    RegularPayAmt = 0;
                                    drEmpPayroll["Tpy_PDLVHr"]                                      = 0;
                                    drEmpPayroll["Tpy_PDLEGHOLHr"]                                  = 0;
                                    drEmpPayroll["Tpy_PDSPLHOLHr"]                                  = 0;
                                    drEmpPayroll["Tpy_PDCOMPHOLHr"]                                 = 0;
                                    drEmpPayroll["Tpy_PDOTHHOLHr"]                                  = 0;
                                    drEmpPayroll["Tpy_TotalREGHr"]                                  = 0;
                                    drEmpPayroll["Tpy_REGHr"]                                       = 0;
                                    drEmpPayroll["Tpy_REGOTHr"]                                     = 0;
                                    drEmpPayroll["Tpy_REGNDHr"]                                     = 0;
                                    drEmpPayroll["Tpy_REGNDOTHr"]                                   = 0;
                                    drEmpPayroll["Tpy_RESTHr"]                                      = 0;
                                    drEmpPayroll["Tpy_RESTOTHr"]                                    = 0;
                                    drEmpPayroll["Tpy_RESTNDHr"]                                    = 0;
                                    drEmpPayroll["Tpy_RESTNDOTHr"]                                  = 0;
                                    drEmpPayroll["Tpy_LEGHOLHr"]                                    = 0;
                                    drEmpPayroll["Tpy_LEGHOLOTHr"]                                  = 0;
                                    drEmpPayroll["Tpy_LEGHOLNDHr"]                                  = 0;
                                    drEmpPayroll["Tpy_LEGHOLNDOTHr"]                                = 0;
                                    drEmpPayroll["Tpy_SPLHOLHr"]                                    = 0;
                                    drEmpPayroll["Tpy_SPLHOLOTHr"]                                  = 0;
                                    drEmpPayroll["Tpy_SPLHOLNDHr"]                                  = 0;
                                    drEmpPayroll["Tpy_SPLHOLNDOTHr"]                                = 0;
                                    drEmpPayroll["Tpy_PSDHr"]                                       = 0;
                                    drEmpPayroll["Tpy_PSDOTHr"]                                     = 0;
                                    drEmpPayroll["Tpy_PSDNDHr"]                                     = 0;
                                    drEmpPayroll["Tpy_PSDNDOTHr"]                                   = 0;
                                    drEmpPayroll["Tpy_COMPHOLHr"]                                   = 0;
                                    drEmpPayroll["Tpy_COMPHOLOTHr"]                                 = 0;
                                    drEmpPayroll["Tpy_COMPHOLNDHr"]                                 = 0;
                                    drEmpPayroll["Tpy_COMPHOLNDOTHr"]                               = 0;
                                    drEmpPayroll["Tpy_RESTLEGHOLHr"]                                = 0;
                                    drEmpPayroll["Tpy_RESTLEGHOLOTHr"]                              = 0;
                                    drEmpPayroll["Tpy_RESTLEGHOLNDHr"]                              = 0;
                                    drEmpPayroll["Tpy_RESTLEGHOLNDOTHr"]                            = 0;
                                    drEmpPayroll["Tpy_RESTSPLHOLHr"]                                = 0;
                                    drEmpPayroll["Tpy_RESTSPLHOLOTHr"]                              = 0;
                                    drEmpPayroll["Tpy_RESTSPLHOLNDHr"]                              = 0;
                                    drEmpPayroll["Tpy_RESTSPLHOLNDOTHr"]                            = 0;
                                    drEmpPayroll["Tpy_RESTCOMPHOLHr"]                               = 0;
                                    drEmpPayroll["Tpy_RESTCOMPHOLOTHr"]                             = 0;
                                    drEmpPayroll["Tpy_RESTCOMPHOLNDHr"]                             = 0;
                                    drEmpPayroll["Tpy_RESTCOMPHOLNDOTHr"]                           = 0;
                                    drEmpPayroll["Tpy_RESTPSDHr"]                                   = 0;
                                    drEmpPayroll["Tpy_RESTPSDOTHr"]                                 = 0;
                                    drEmpPayroll["Tpy_RESTPSDNDHr"]                                 = 0;
                                    drEmpPayroll["Tpy_RESTPSDNDOTHr"]                               = 0;
                                    drEmpPayroll["Tpy_PDRESTLEGHOLHr"]                              = 0;
                                    drEmpPayroll["Tpy_PDRESTLEGHOLAmt"]                             = 0;
                                    drEmpPayroll["Tpy_TotalAdjAmt"]                                 = 0;
                                    drEmpPayroll["Tpy_OTAdjAmt"]                                    = 0;
                                    drEmpPayroll["Tpy_REGAmt"]                                      = 0;
                                    drEmpPayroll["Tpy_PDLVAmt"]                                     = 0;
                                    drEmpPayroll["Tpy_PDLEGHOLAmt"]                                 = 0;
                                    drEmpPayroll["Tpy_PDSPLHOLAmt"]                                 = 0;
                                    drEmpPayroll["Tpy_PDCOMPHOLAmt"]                                = 0;
                                    drEmpPayroll["Tpy_PDPSDAmt"]                                    = 0;
                                    drEmpPayroll["Tpy_PDOTHHOLAmt"]                                 = 0;

                                    #endregion
                                }

                                if ((Convert.ToDecimal(drEmpPayroll["Tpy_TotalREGHr"])) == 0)
                                    drEmpPayroll["Tpy_TotalREGAmt"] = 0;
                                else
                                    drEmpPayroll["Tpy_TotalREGAmt"] = RegularPayAmt;
                                #endregion
                            }
                            #endregion

                            #region Overtime Amount Computation
                            drEmpPayroll["Tpy_REGOTAmt"]                                            = RegularOTAmt;
                            drEmpPayroll["Tpy_REGNDAmt"]                                            = RegularNDAmt;
                            drEmpPayroll["Tpy_REGNDOTAmt"]                                          = RegularOTNDAmt;

                            drEmpPayroll["Tpy_RESTAmt"]                                             = RestdayAmt;
                            drEmpPayroll["Tpy_RESTOTAmt"]                                           = RestdayOTAmt;
                            drEmpPayroll["Tpy_RESTNDAmt"]                                           = RestdayNDAmt;
                            drEmpPayroll["Tpy_RESTNDOTAmt"]                                         = RestdayOTNDAmt;

                            drEmpPayroll["Tpy_LEGHOLAmt"]                                           = LegalHolidayAmt;
                            drEmpPayroll["Tpy_LEGHOLOTAmt"]                                         = LegalHolidayOTAmt;
                            drEmpPayroll["Tpy_LEGHOLNDAmt"]                                         = LegalHolidayNDAmt;
                            drEmpPayroll["Tpy_LEGHOLNDOTAmt"]                                       = LegalHolidayOTNDAmt;

                            drEmpPayroll["Tpy_SPLHOLAmt"]                                           = SpecialHolidayAmt;
                            drEmpPayroll["Tpy_SPLHOLOTAmt"]                                         = SpecialHolidayOTAmt;
                            drEmpPayroll["Tpy_SPLHOLNDAmt"]                                         = SpecialHolidayNDAmt;
                            drEmpPayroll["Tpy_SPLHOLNDOTAmt"]                                       = SpecialHolidayOTNDAmt;

                            drEmpPayroll["Tpy_PSDAmt"]                                              = PlantShutdownAmt;
                            drEmpPayroll["Tpy_PSDOTAmt"]                                            = PlantShutdownOTAmt;
                            drEmpPayroll["Tpy_PSDNDAmt"]                                            = PlantShutdownNDAmt;
                            drEmpPayroll["Tpy_PSDNDOTAmt"]                                          = PlantShutdownOTNDAmt;

                            drEmpPayroll["Tpy_COMPHOLAmt"]                                          = CompanyHolidayAmt;
                            drEmpPayroll["Tpy_COMPHOLOTAmt"]                                        = CompanyHolidayOTAmt;
                            drEmpPayroll["Tpy_COMPHOLNDAmt"]                                        = CompanyHolidayNDAmt;
                            drEmpPayroll["Tpy_COMPHOLNDOTAmt"]                                      = CompanyHolidayOTNDAmt;

                            drEmpPayroll["Tpy_RESTLEGHOLAmt"]                                       = RestdayLegalHolidayAmt;
                            drEmpPayroll["Tpy_RESTLEGHOLOTAmt"]                                     = RestdayLegalHolidayOTAmt;
                            drEmpPayroll["Tpy_RESTLEGHOLNDAmt"]                                     = RestdayLegalHolidayNDAmt;
                            drEmpPayroll["Tpy_RESTLEGHOLNDOTAmt"]                                   = RestdayLegalHolidayOTNDAmt;

                            drEmpPayroll["Tpy_RESTSPLHOLAmt"]                                       = RestdaySpecialHolidayAmt;
                            drEmpPayroll["Tpy_RESTSPLHOLOTAmt"]                                     = RestdaySpecialHolidayOTAmt;
                            drEmpPayroll["Tpy_RESTSPLHOLNDAmt"]                                     = RestdaySpecialHolidayNDAmt;
                            drEmpPayroll["Tpy_RESTSPLHOLNDOTAmt"]                                   = RestdaySpecialHolidayOTNDAmt;

                            drEmpPayroll["Tpy_RESTCOMPHOLAmt"]                                      = RestdayCompanyHolidayAmt;
                            drEmpPayroll["Tpy_RESTCOMPHOLOTAmt"]                                    = RestdayCompanyHolidayOTAmt;
                            drEmpPayroll["Tpy_RESTCOMPHOLNDAmt"]                                    = RestdayCompanyHolidayNDAmt;
                            drEmpPayroll["Tpy_RESTCOMPHOLNDOTAmt"]                                  = RestdayCompanyHolidayOTNDAmt;

                            drEmpPayroll["Tpy_RESTPSDAmt"]                                          = RestdayPlantShutdownAmt;
                            drEmpPayroll["Tpy_RESTPSDOTAmt"]                                        = RestdayPlantShutdownOTAmt;
                            drEmpPayroll["Tpy_RESTPSDNDAmt"]                                        = RestdayPlantShutdownNDAmt;
                            drEmpPayroll["Tpy_RESTPSDNDOTAmt"]                                      = RestdayPlantShutdownOTNDAmt;
                            #endregion

                            #endregion

                            #region Compute hour ext premiums
                            if (bHasDayCodeExt)
                            {
                                drEmpPayrollMisc["Tpm_Misc1Amt"]                        = Misc1Amt;
                                drEmpPayrollMisc["Tpm_Misc1OTAmt"]                      = Misc1OTAmt;
                                drEmpPayrollMisc["Tpm_Misc1NDAmt"]                      = Misc1NDAmt;
                                drEmpPayrollMisc["Tpm_Misc1NDOTAmt"]                    = Misc1NDOTAmt;
                                drEmpPayrollMisc["Tpm_Misc2Amt"]                        = Misc2Amt;
                                drEmpPayrollMisc["Tpm_Misc2OTAmt"]                      = Misc2OTAmt;
                                drEmpPayrollMisc["Tpm_Misc2NDAmt"]                      = Misc2NDAmt;
                                drEmpPayrollMisc["Tpm_Misc2NDOTAmt"]                    = Misc2NDOTAmt;
                                drEmpPayrollMisc["Tpm_Misc3Amt"]                        = Misc3Amt;
                                drEmpPayrollMisc["Tpm_Misc3OTAmt"]                      = Misc3OTAmt;
                                drEmpPayrollMisc["Tpm_Misc3NDAmt"]                      = Misc3NDAmt;
                                drEmpPayrollMisc["Tpm_Misc3NDOTAmt"]                    = Misc3NDOTAmt;
                                drEmpPayrollMisc["Tpm_Misc4Amt"]                        = Misc4Amt;
                                drEmpPayrollMisc["Tpm_Misc4OTAmt"]                      = Misc4OTAmt;
                                drEmpPayrollMisc["Tpm_Misc4NDAmt"]                      = Misc4NDAmt;
                                drEmpPayrollMisc["Tpm_Misc4NDOTAmt"]                    = Misc4NDOTAmt;
                                drEmpPayrollMisc["Tpm_Misc5Amt"]                        = Misc5Amt;
                                drEmpPayrollMisc["Tpm_Misc5OTAmt"]                      = Misc5OTAmt;
                                drEmpPayrollMisc["Tpm_Misc5NDAmt"]                      = Misc5NDAmt;
                                drEmpPayrollMisc["Tpm_Misc5NDOTAmt"]                    = Misc5NDOTAmt;
                                drEmpPayrollMisc["Tpm_Misc6Amt"]                        = Misc6Amt;
                                drEmpPayrollMisc["Tpm_Misc6OTAmt"]                      = Misc6OTAmt;
                                drEmpPayrollMisc["Tpm_Misc6NDAmt"]                      = Misc6NDAmt;
                                drEmpPayrollMisc["Tpm_Misc6NDOTAmt"]                    = Misc6NDOTAmt;

                            }
                            #endregion
                            
                            #endregion

                            #region Rounding Off of Amounts
                            drEmpPayroll["Tpy_LTAmt"]                   = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_LTAmt"]), 2);
                            drEmpPayroll["Tpy_UTAmt"]                   = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_UTAmt"]), 2);
                            drEmpPayroll["Tpy_UPLVAmt"]                 = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_UPLVAmt"]), 2);
                            drEmpPayroll["Tpy_ABSLEGHOLAmt"]            = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_ABSLEGHOLAmt"]), 2);
                            drEmpPayroll["Tpy_ABSSPLHOLAmt"]            = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_ABSSPLHOLAmt"]), 2);
                            drEmpPayroll["Tpy_ABSCOMPHOLAmt"]           = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_ABSCOMPHOLAmt"]), 2);
                            drEmpPayroll["Tpy_ABSPSDAmt"]               = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_ABSPSDAmt"]), 2);
                            drEmpPayroll["Tpy_ABSOTHHOLAmt"]            = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_ABSOTHHOLAmt"]), 2);
                            drEmpPayroll["Tpy_WDABSAmt"]                = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_WDABSAmt"]), 2);
                            drEmpPayroll["Tpy_LTUTMaxAmt"]              = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_LTUTMaxAmt"]), 2);
                            drEmpPayroll["Tpy_ABSAmt"]                  = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_ABSAmt"]), 2);
                            drEmpPayroll["Tpy_REGAmt"]                  = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_REGAmt"]), 2);
                            drEmpPayroll["Tpy_PDLVAmt"]                 = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_PDLVAmt"]), 2);
                            drEmpPayroll["Tpy_PDLEGHOLAmt"]             = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_PDLEGHOLAmt"]), 2);
                            drEmpPayroll["Tpy_PDSPLHOLAmt"]             = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_PDSPLHOLAmt"]), 2);
                            drEmpPayroll["Tpy_PDCOMPHOLAmt"]            = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_PDCOMPHOLAmt"]), 2);
                            drEmpPayroll["Tpy_PDPSDAmt"]                = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_PDPSDAmt"]), 2);
                            drEmpPayroll["Tpy_PDOTHHOLAmt"]             = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_PDOTHHOLAmt"]), 2);
                            drEmpPayroll["Tpy_PDRESTLEGHOLAmt"]         = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_PDRESTLEGHOLAmt"]), 2);
                            drEmpPayroll["Tpy_TotalREGAmt"]             = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_TotalREGAmt"]), 2);
                            drEmpPayroll["Tpy_REGOTAmt"]                = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_REGOTAmt"]), 2);
                            drEmpPayroll["Tpy_REGNDAmt"]                = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_REGNDAmt"]), 2);
                            drEmpPayroll["Tpy_REGNDOTAmt"]              = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_REGNDOTAmt"]), 2);
                            drEmpPayroll["Tpy_RESTAmt"]                 = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTAmt"]), 2);
                            drEmpPayroll["Tpy_RESTOTAmt"]               = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTOTAmt"]), 2);
                            drEmpPayroll["Tpy_RESTNDAmt"]               = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTNDAmt"]), 2);
                            drEmpPayroll["Tpy_RESTNDOTAmt"]             = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTNDOTAmt"]), 2);
                            drEmpPayroll["Tpy_LEGHOLAmt"]               = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_LEGHOLAmt"]), 2);
                            drEmpPayroll["Tpy_LEGHOLOTAmt"]             = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_LEGHOLOTAmt"]), 2);
                            drEmpPayroll["Tpy_LEGHOLNDAmt"]             = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_LEGHOLNDAmt"]), 2);
                            drEmpPayroll["Tpy_LEGHOLNDOTAmt"]           = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_LEGHOLNDOTAmt"]), 2);
                            drEmpPayroll["Tpy_SPLHOLAmt"]               = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_SPLHOLAmt"]), 2);
                            drEmpPayroll["Tpy_SPLHOLOTAmt"]             = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_SPLHOLOTAmt"]), 2);
                            drEmpPayroll["Tpy_SPLHOLNDAmt"]             = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_SPLHOLNDAmt"]), 2);
                            drEmpPayroll["Tpy_SPLHOLNDOTAmt"]           = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_SPLHOLNDOTAmt"]), 2);
                            drEmpPayroll["Tpy_PSDAmt"]                  = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_PSDAmt"]), 2);
                            drEmpPayroll["Tpy_PSDOTAmt"]                = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_PSDOTAmt"]), 2);
                            drEmpPayroll["Tpy_PSDNDAmt"]                = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_PSDNDAmt"]), 2);
                            drEmpPayroll["Tpy_PSDNDOTAmt"]              = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_PSDNDOTAmt"]), 2);
                            drEmpPayroll["Tpy_COMPHOLAmt"]              = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_COMPHOLAmt"]), 2);
                            drEmpPayroll["Tpy_COMPHOLOTAmt"]            = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_COMPHOLOTAmt"]), 2);
                            drEmpPayroll["Tpy_COMPHOLNDAmt"]            = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_COMPHOLNDAmt"]), 2);
                            drEmpPayroll["Tpy_COMPHOLNDOTAmt"]          = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_COMPHOLNDOTAmt"]), 2);
                            drEmpPayroll["Tpy_RESTLEGHOLAmt"]           = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTLEGHOLAmt"]), 2);
                            drEmpPayroll["Tpy_RESTLEGHOLOTAmt"]         = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTLEGHOLOTAmt"]), 2);
                            drEmpPayroll["Tpy_RESTLEGHOLNDAmt"]         = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTLEGHOLNDAmt"]), 2);
                            drEmpPayroll["Tpy_RESTLEGHOLNDOTAmt"]       = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTLEGHOLNDOTAmt"]), 2);
                            drEmpPayroll["Tpy_RESTSPLHOLAmt"]           = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTSPLHOLAmt"]), 2);
                            drEmpPayroll["Tpy_RESTSPLHOLOTAmt"]         = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTSPLHOLOTAmt"]), 2);
                            drEmpPayroll["Tpy_RESTSPLHOLNDAmt"]         = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTSPLHOLNDAmt"]), 2);
                            drEmpPayroll["Tpy_RESTSPLHOLNDOTAmt"]       = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTSPLHOLNDOTAmt"]), 2);
                            drEmpPayroll["Tpy_RESTCOMPHOLAmt"]          = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTCOMPHOLAmt"]), 2);
                            drEmpPayroll["Tpy_RESTCOMPHOLOTAmt"]        = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTCOMPHOLOTAmt"]), 2);
                            drEmpPayroll["Tpy_RESTCOMPHOLNDAmt"]        = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTCOMPHOLNDAmt"]), 2);
                            drEmpPayroll["Tpy_RESTCOMPHOLNDOTAmt"]      = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTCOMPHOLNDOTAmt"]), 2);
                            drEmpPayroll["Tpy_RESTPSDAmt"]              = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTPSDAmt"]), 2);
                            drEmpPayroll["Tpy_RESTPSDOTAmt"]            = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTPSDOTAmt"]), 2);
                            drEmpPayroll["Tpy_RESTPSDNDAmt"]            = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTPSDNDAmt"]), 2);
                            drEmpPayroll["Tpy_RESTPSDNDOTAmt"]          = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_RESTPSDNDOTAmt"]), 2);
                            
                            if (bHasDayCodeExt)
                            {
                                drEmpPayrollMisc["Tpm_Misc1Amt"]     = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc1Amt"]), 2);
                                drEmpPayrollMisc["Tpm_Misc1OTAmt"]   = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc1OTAmt"]), 2);
                                drEmpPayrollMisc["Tpm_Misc1NDAmt"]   = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc1NDAmt"]), 2);
                                drEmpPayrollMisc["Tpm_Misc1NDOTAmt"] = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc1NDOTAmt"]), 2);
                                drEmpPayrollMisc["Tpm_Misc2Amt"]     = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc2Amt"]), 2);
                                drEmpPayrollMisc["Tpm_Misc2OTAmt"]   = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc2OTAmt"]), 2);
                                drEmpPayrollMisc["Tpm_Misc2NDAmt"]   = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc2NDAmt"]), 2);
                                drEmpPayrollMisc["Tpm_Misc2NDOTAmt"] = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc2NDOTAmt"]), 2);
                                drEmpPayrollMisc["Tpm_Misc3Amt"]     = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc3Amt"]), 2);
                                drEmpPayrollMisc["Tpm_Misc3OTAmt"]   = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc3OTAmt"]), 2);
                                drEmpPayrollMisc["Tpm_Misc3NDAmt"]   = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc3NDAmt"]), 2);
                                drEmpPayrollMisc["Tpm_Misc3NDOTAmt"] = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc3NDOTAmt"]), 2);
                                drEmpPayrollMisc["Tpm_Misc4Amt"]     = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc4Amt"]), 2);
                                drEmpPayrollMisc["Tpm_Misc4OTAmt"]   = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc4OTAmt"]), 2);
                                drEmpPayrollMisc["Tpm_Misc4NDAmt"]   = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc4NDAmt"]), 2);
                                drEmpPayrollMisc["Tpm_Misc4NDOTAmt"] = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc4NDOTAmt"]), 2);
                                drEmpPayrollMisc["Tpm_Misc5Amt"]     = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc5Amt"]), 2);
                                drEmpPayrollMisc["Tpm_Misc5OTAmt"]   = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc5OTAmt"]), 2);
                                drEmpPayrollMisc["Tpm_Misc5NDAmt"]   = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc5NDAmt"]), 2);
                                drEmpPayrollMisc["Tpm_Misc5NDOTAmt"] = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc5NDOTAmt"]), 2);
                                drEmpPayrollMisc["Tpm_Misc6Amt"]     = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc6Amt"]), 2);
                                drEmpPayrollMisc["Tpm_Misc6OTAmt"]   = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc6OTAmt"]), 2);
                                drEmpPayrollMisc["Tpm_Misc6NDAmt"]   = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc6NDAmt"]), 2);
                                drEmpPayrollMisc["Tpm_Misc6NDOTAmt"] = Math.Round(Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc6NDOTAmt"]), 2);
                            }
                            #endregion

                            #region Gross Pay Computation
                            GrossPayAmt = Convert.ToDecimal(drEmpPayroll["Tpy_TotalREGAmt"])
                                            + Convert.ToDecimal(drEmpPayroll["Tpy_REGOTAmt"])
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
                                            + Convert.ToDecimal(drEmpPayroll["Tpy_RESTPSDNDOTAmt"])
                                            + Convert.ToDecimal(drEmpPayroll["Tpy_TotalAdjAmt"])
                                            + Convert.ToDecimal(drEmpPayroll["Tpy_TaxableIncomeAmt"]);

                            TotalOTNDAmt = Convert.ToDecimal(drEmpPayroll["Tpy_REGOTAmt"])
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
                                GrossPayAmt += Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc1Amt"])
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

                                TotalOTNDAmt += Convert.ToDecimal(drEmpPayrollMisc["Tpm_Misc1Amt"])
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

                            //Set Gross Pay Amount
                            GrossPayAmt = Math.Round(GrossPayAmt, 2);
                            drEmpPayroll["Tpy_TotalTaxableIncomeAmt"] = GrossPayAmt;
                            // Set Total OT AND ND Amount
                            TotalOTNDAmt = Math.Round(TotalOTNDAmt, 2);
                            drEmpPayroll["Tpy_TotalOTNDAmt"] = TotalOTNDAmt;

                            #endregion

                            #region Info Messages
                            if (!ProcessSeparated)
                            {
                                if (RegularPayAmt == 0)
                                {
                                    AddErrorToPayrollReport(drEmpPayroll["Tpy_IDNo"].ToString(), dtEmpPayTranHdr.Rows[i]["Mem_LastName"].ToString(), dtEmpPayTranHdr.Rows[i]["Mem_FirstName"].ToString(), ProcessPayrollPeriod, RegularPayAmt, 0, "Zero Regular Pay");
                                }
                                if (RegularPayAmt < 0)
                                {
                                    AddErrorToPayrollReport(drEmpPayroll["Tpy_IDNo"].ToString(), dtEmpPayTranHdr.Rows[i]["Mem_LastName"].ToString(), dtEmpPayTranHdr.Rows[i]["Mem_FirstName"].ToString(), ProcessPayrollPeriod, RegularPayAmt, 0, "Negative Regular Pay");
                                }
                                if (GrossPayAmt == 0)
                                {
                                    AddErrorToPayrollReport(drEmpPayroll["Tpy_IDNo"].ToString(), dtEmpPayTranHdr.Rows[i]["Mem_LastName"].ToString(), dtEmpPayTranHdr.Rows[i]["Mem_FirstName"].ToString(), ProcessPayrollPeriod, GrossPayAmt, 0, "Zero Gross Pay");
                                }
                                if (GrossPayAmt < 0)
                                {
                                    AddErrorToPayrollReport(drEmpPayroll["Tpy_IDNo"].ToString(), dtEmpPayTranHdr.Rows[i]["Mem_LastName"].ToString(), dtEmpPayTranHdr.Rows[i]["Mem_FirstName"].ToString(), ProcessPayrollPeriod, GrossPayAmt, 0, "Negative Gross Pay");
                                }
                            }
                            #endregion

                            if ((!ProcessSeparated && ((GrossPayAmt >= 0 || Convert.ToBoolean(NEGTAXINCOME) == true))) || ProcessSeparated)

                                {
                                    #region Info Messages
                                    if (RegularPayAmt > MAXREGPAY)
                                {
                                    AddErrorToPayrollReport(drEmpPayroll["Tpy_IDNo"].ToString(), dtEmpPayTranHdr.Rows[i]["Mem_LastName"].ToString(), dtEmpPayTranHdr.Rows[i]["Mem_FirstName"].ToString(), ProcessPayrollPeriod, RegularPayAmt, 0, string.Format("Regular Pay > {0:0.00}", MAXREGPAY));
                                }
                                if (GrossPayAmt > MAXTAXINC)
                                {
                                    AddErrorToPayrollReport(drEmpPayroll["Tpy_IDNo"].ToString(), dtEmpPayTranHdr.Rows[i]["Mem_LastName"].ToString(), dtEmpPayTranHdr.Rows[i]["Mem_FirstName"].ToString(), ProcessPayrollPeriod, GrossPayAmt, 0, string.Format("Gross Pay > {0:0.00}", MAXTAXINC));
                                }
                                #endregion

                                //Get historical payroll records
                                drArrEmpPayrollYearly = dtEmpPayrollYearly1stAnd2ndQuin.Select("Tpy_IDNo = '" + curEmployeeId + "' AND Tpy_PayCycle LIKE '" + PayrollYearMonth + "*'");
                                drArrEmpPayrollMisc2Yearly = dtEmpPayrollMisc2Yearly_1stAnd2ndQuin.Select("Tpy_IDNo = '" + curEmployeeId + "' AND Tpy_PayCycle LIKE '" + PayrollYearMonth + "*'");

                                #region Compute HDMF Premium
                                PAYFREQNCYCOUNT = GetPremiumPayCounter(ProcessPayrollPeriod, "HDMFPREM");
                                strHDMFRule = drEmpPayroll["Tpy_PagIbigRule"].ToString();
                                if (bComputeHDMF && !strHDMFRule.Equals("N"))
                                {
                                    dBaseSalaryAmt                  = 0;
                                    dMonthlyBaseSalaryAmt           = 0;
                                    dHDMFEmployeeShare              = 0;
                                    dHDMFEmployerShare              = 0;
                                    dMonthlyHDMFEmployeeShareNonTax = 0;
                                    dMonthlyHDMFEmployeeShare       = 0;
                                    dMonthlyHDMFEmployerShare       = 0;
                                    dHDMFERAmt                      = 0;

                                    #region Initialization

                                    dBaseSalaryAmt = ComputeBaseAmount(true, curEmployeeId
                                                        , ProcessPayrollPeriod
                                                        , strHDMFRule
                                                        , Convert.ToDecimal(drEmpPayroll["Tpy_TotalTaxableIncomeAmt"].ToString())
                                                        , Convert.ToDecimal(drEmpPayroll["Tpy_TotalREGAmt"].ToString())
                                                        , Convert.ToDecimal(drEmpPayroll["Tpy_SRGAdjAmt"].ToString())
                                                                + Convert.ToDecimal(drEmpPayroll["Tpy_SHOLAdjAmt"].ToString())
                                                                + Convert.ToDecimal(drEmpPayroll["Tpy_SLVAdjAmt"].ToString())
                                                                + Convert.ToDecimal(drEmpPayroll["Tpy_MRGAdjAmt"].ToString())
                                                                + Convert.ToDecimal(drEmpPayroll["Tpy_MHOLAdjAmt"].ToString())
                                                        , Convert.ToDecimal(drEmpPayroll["Tpy_TaxableIncomeAmt"].ToString())
                                                        , drEmpPayroll["Tpy_SalaryRate"].ToString()
                                                        , drEmpPayroll["Tpy_PayrollType"].ToString()
                                                        , drEmpPayroll["Tpy_PagIbigShare"].ToString()
                                                        , HDMFREMMAXSHARE //GovRemittance MaxShare only used in HDMF
                                                        , ApplicHDMFPrem
                                                        , HDMFEARLYCYCLEFULLDEDN
                                                        , HDMFREFERPREVINCOMEPREM); //CompensationFrom



                                    if (drArrEmpPayrollYearly.Length > 0)
                                    {
                                        dMonthlyBaseSalaryAmt = ComputeBaseAmount(false, curEmployeeId
                                                            , PayrollYearMonth
                                                            , strHDMFRule
                                                            , Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_TotalTaxableIncomeAmt"].ToString())
                                                            , Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_TotalREGAmt"].ToString())
                                                            , Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_SRGAdjAmt"].ToString())
                                                                    + Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_SHOLAdjAmt"].ToString())
                                                                    + Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_SLVAdjAmt"].ToString())
                                                                    + Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_MRGAdjAmt"].ToString())
                                                                    + Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_MHOLAdjAmt"].ToString())
                                                            , Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_TaxableIncomeAmt"].ToString())
                                                            , drArrEmpPayrollYearly[0]["Tpy_SalaryRate"].ToString()
                                                            , drArrEmpPayrollYearly[0]["Tpy_PayrollType"].ToString()
                                                            , "0"
                                                            , "0"
                                                            , ApplicHDMFPrem
                                                            , HDMFEARLYCYCLEFULLDEDN
                                                            , HDMFREFERPREVINCOMEPREM);

                                        dMonthlyHDMFEmployeeShareNonTax = Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_PagIbigEE"].ToString());
                                        dMonthlyHDMFEmployeeShare       = Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_PagIbigEE"].ToString()) + Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_PagIbigTaxEE"].ToString());
                                        dMonthlyHDMFEmployerShare       = Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_PagIbigER"].ToString());
                                    }
                                    #endregion

                                    #region Computation
                                    /////Fixed Contribution/Special/Maximum
                                    if (strHDMFRule.Equals("F") || strHDMFRule.Equals("S") || strHDMFRule.Equals("M") || strHDMFRule.Equals("E")) 
                                    {
                                        if (strHDMFRule.Equals("E")) 
                                        {
                                            dServiceYear = Convert.ToDecimal(dtEmpPayTranHdr.Rows[i]["ServiceYear"].ToString()); 
                                            drPremiumContribution = dtPremiumContribution.Select(string.Format("Mps_DeductionCode = 'HDMFPREM' AND Mps_CompensationFrom <= {0} AND Mps_PayCycle = '{1}'", dServiceYear, MaxHDMFPremPayCycle), "Mps_CompensationFrom DESC");
                                            if (drPremiumContribution.Length > 0)
                                                dHDMFERAmt = Convert.ToDecimal(drPremiumContribution[0]["Mps_ERShare"].ToString());
                                            if (dBaseSalaryAmt < dHDMFERAmt)
                                                dHDMFERAmt = dBaseSalaryAmt;
                                        }
                                        else
                                            dHDMFERAmt = 100;

                                        if (ApplicHDMFPrem.Equals("0"))
                                        {
                                            if (HDMFEARLYCYCLEFULLDEDN || bMonthEnd) //SHOULD BE FULL IN FIRST RUN
                                            {
                                                //Employee
                                                if (dBaseSalaryAmt != 0)
                                                    dHDMFEmployeeShare = dBaseSalaryAmt - dMonthlyHDMFEmployeeShare;
                                                //Employer
                                                dHDMFEmployerShare = dHDMFERAmt - dMonthlyHDMFEmployerShare;
                                            }
                                            else
                                            {
                                                //Employee
                                                dHDMFEmployeeShare = dBaseSalaryAmt;
                                                //Employer
                                                dHDMFEmployerShare = dHDMFERAmt;
                                            }
                                        }
                                        else
                                        {
                                            if (PAYFREQNCY == "W")
                                            {
                                                //Employee
                                                dHDMFEmployeeShare = dBaseSalaryAmt / PAYFREQNCYCOUNT;
                                                //Employer
                                                dHDMFEmployerShare = dHDMFERAmt / PAYFREQNCYCOUNT;
                                            }
                                            else
                                            {
                                                //Employee
                                                dHDMFEmployeeShare = dBaseSalaryAmt;
                                                //Employer
                                                dHDMFEmployerShare = dHDMFERAmt;
                                            }
                                        }

                                        #region ER
                                        dHDMFEmployerShare = CheckMaxShareCeiling(dHDMFEmployerShare, dMonthlyHDMFEmployerShare, dHDMFERAmt);
                                        #endregion

                                        //As per Revenue Memorandum Circular No. 27-2011
                                        if (strHDMFRule.Equals("S") || strHDMFRule.Equals("E"))
                                        {
                                            amount = 0;
                                            if (ApplicHDMFPrem.Equals("0"))
                                            {
                                                if (HDMFEARLYCYCLEFULLDEDN || (bMonthEnd && dMonthlyHDMFEmployeeShareNonTax <= 100))
                                                    amount = 100 - dMonthlyHDMFEmployeeShareNonTax;
                                                else
                                                {
                                                    amount = 100;
                                                }
                                            }
                                            else
                                                amount = 100;

                                            if (dHDMFEmployeeShare > amount)
                                            {
                                                drEmpPayroll["Tpy_PagIbigTaxEE"] = Math.Round(dHDMFEmployeeShare - amount, 2);
                                                dHDMFEmployeeShare = amount;
                                            }
                                        }
                                    }

                                    /////Regular/Total Taxable Income/Overtime/Basic/Declared Tax Amount Pay
                                    else if (strHDMFRule.Equals("R") || strHDMFRule.Equals("T") || strHDMFRule.Equals("O") || strHDMFRule.Equals("B") || strHDMFRule.Equals("A"))
                                    {
                                        amount = dBaseSalaryAmt;
                                        //Check if HDMF premium will look in first quincena of the month
                                        if (HDMFREFERPREVINCOMEPREM)
                                            amount = dBaseSalaryAmt + dMonthlyBaseSalaryAmt;

                                        drPremiumContribution = dtPremiumContribution.Select(string.Format("Mps_DeductionCode = 'HDMFPREM' AND Mps_CompensationFrom <= {0} AND Mps_PayCycle = '{1}'", amount, MaxHDMFPremPayCycle), "Mps_CompensationFrom DESC");
                                        if (drPremiumContribution.Length > 0)
                                        {
                                            if (Convert.ToBoolean(drPremiumContribution[0]["Mps_IsPercentage"]) == false)
                                            {
                                                dHDMFEmployeeShare += Convert.ToDecimal(drPremiumContribution[0]["Mps_EEShare"].ToString());
                                                dHDMFEmployerShare += Convert.ToDecimal(drPremiumContribution[0]["Mps_ERShare"].ToString());
                                            }
                                            else
                                            {
                                                dHDMFEmployeeShare += (Convert.ToDecimal(drPremiumContribution[0]["Mps_EEShare"].ToString()) / 100.0m) * amount;
                                                dHDMFEmployerShare += (Convert.ToDecimal(drPremiumContribution[0]["Mps_ERShare"].ToString()) / 100.0m) * amount;
                                            }
                                        }

                                        #region Check of Max Amount Contribution (HDMF)
                                        dHDMFMaxAmt = Convert.ToDecimal(HDMFREMMAXSHARE);

                                        if (dHDMFMaxAmt > 0)
                                        {
                                            dHDMFEmployeeShare = CheckMaxShareCeiling(dHDMFEmployeeShare, dMonthlyHDMFEmployeeShare, dHDMFMaxAmt);
                                            dHDMFEmployerShare = CheckMaxShareCeiling(dHDMFEmployerShare, dMonthlyHDMFEmployerShare, 100);
                                        }
                                        #endregion
                                    }

                                    #endregion

                                    //Save HDMF premiums
                                    dHDMFEmployeeShare                      = Math.Round(dHDMFEmployeeShare, 2);
                                    drEmpPayroll["Tpy_PagIbigEE"]           = dHDMFEmployeeShare;
                                    dHDMFEmployerShare                      = Math.Round(dHDMFEmployerShare, 2);
                                    drEmpPayroll["Tpy_PagIbigER"]           = dHDMFEmployerShare;

                                    if (HDMFREFERPREVINCOMEPREM)
                                        dBaseSalaryAmt += dMonthlyBaseSalaryAmt;

                                    dBaseSalaryAmt                          = Math.Round(dBaseSalaryAmt, 2);
                                    drEmpPayroll["Tpy_PagIbigBaseAmt"]      = dBaseSalaryAmt;

                                }
                                #endregion

                                #region Compute SSS and EC Premium
                                PAYFREQNCYCOUNT = GetPremiumPayCounter(ProcessPayrollPeriod, "SSSPREM");
                                strSSSRule = drEmpPayroll["Tpy_SSSRule"].ToString();
                                if (bComputeSSS && !strSSSRule.Equals("N"))
                                {
                                    dBaseSalaryAmt                          = 0;
                                    dMonthlyBaseSalaryAmt                   = 0;
                                    dSSSEmployeeShare                       = 0;
                                    dSSSEmployerShare                       = 0;
                                    dECFundAmt                              = 0;
                                    dMonthlySSSEmployeeShare                = 0;
                                    dMonthlySSSEmployerShare                = 0;
                                    dMonthlyECFundAmt                       = 0;

                                    dMPFEmployeeShare                       = 0;
                                    dMPFEmployerShare                       = 0;
                                    dMonthlyMPFEmployeeShare                = 0;
                                    dMonthlyMPFEmployerShare                = 0;

                                    #region Initialization
                                    dBaseSalaryAmt = ComputeBaseAmount(true, curEmployeeId
                                                                   , ProcessPayrollPeriod
                                                                   , strSSSRule
                                                                   , Convert.ToDecimal(drEmpPayroll["Tpy_TotalTaxableIncomeAmt"].ToString())
                                                                   , Convert.ToDecimal(drEmpPayroll["Tpy_TotalREGAmt"].ToString())
                                                                   , Convert.ToDecimal(drEmpPayroll["Tpy_SRGAdjAmt"].ToString())
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_SHOLAdjAmt"].ToString())
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_SLVAdjAmt"].ToString())
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_MRGAdjAmt"].ToString())
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_MHOLAdjAmt"].ToString())
                                                                   , Convert.ToDecimal(drEmpPayroll["Tpy_TaxableIncomeAmt"].ToString())
                                                                   , drEmpPayroll["Tpy_SalaryRate"].ToString()
                                                                   , drEmpPayroll["Tpy_PayrollType"].ToString()
                                                                   , drEmpPayroll["Tpy_SSSShare"].ToString()
                                                                   , SSSBaseMaxShare
                                                                   , ApplicSSSPrem
                                                                   , SSSEARLYCYCLEFULLDEDN
                                                                   , SSSREFERPREVINCOMEPREM);

                                    if (drArrEmpPayrollYearly.Length > 0)
                                    {
                                        dMonthlyBaseSalaryAmt = ComputeBaseAmount(false, curEmployeeId
                                                                   , PayrollYearMonth
                                                                   , strSSSRule
                                                                   , Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_TotalTaxableIncomeAmt"].ToString())
                                                                   , Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_TotalREGAmt"].ToString())
                                                                   , Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_SRGAdjAmt"].ToString())
                                                                          + Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_SHOLAdjAmt"].ToString())
                                                                          + Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_SLVAdjAmt"].ToString())
                                                                          + Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_MRGAdjAmt"].ToString())
                                                                          + Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_MHOLAdjAmt"].ToString())
                                                                   , Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_TaxableIncomeAmt"].ToString())
                                                                   , drArrEmpPayrollYearly[0]["Tpy_SalaryRate"].ToString()
                                                                   , drArrEmpPayrollYearly[0]["Tpy_PayrollType"].ToString()
                                                                   , "0"
                                                                   , "0"
                                                                   , ApplicSSSPrem
                                                                   , SSSEARLYCYCLEFULLDEDN
                                                                   , SSSREFERPREVINCOMEPREM);

                                        dMonthlySSSEmployeeShare = Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_SSSEE"].ToString());
                                        dMonthlySSSEmployerShare = Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_SSSER"].ToString());
                                        dMonthlyECFundAmt        = Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_ECFundAmt"].ToString());
                                        dMonthlyMPFEmployeeShare = Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_MPFEE"].ToString());
                                        dMonthlyMPFEmployerShare = Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_MPFER"].ToString());
                                    }
                                    #endregion

                                    #region Computation
                                    /////Fixed Contribution|Special|Maximum
                                    if (strSSSRule.Equals("F") || strSSSRule.Equals("S"))
                                    {
                                        //SSS Employee Premium 
                                        if (ApplicSSSPrem.Equals("0"))
                                        {
                                            if ((SSSEARLYCYCLEFULLDEDN || bMonthEnd) && dBaseSalaryAmt != 0) //SHOULD BE FULL IN FIRST RUN
                                            {
                                                dSSSEmployeeShare = dBaseSalaryAmt - dMonthlySSSEmployeeShare;
                                                dMPFEmployeeShare = dBaseSalaryAmt - dMonthlyMPFEmployeeShare;
                                            }
                                            else
                                            {
                                                if (PAYFREQNCY == "W")
                                                {
                                                    dSSSEmployeeShare = dBaseSalaryAmt / PAYFREQNCYCOUNT;
                                                    dMPFEmployeeShare = dBaseSalaryAmt / PAYFREQNCYCOUNT;
                                                }  
                                                else //Semi
                                                {
                                                    dSSSEmployeeShare = dBaseSalaryAmt / 2;
                                                    dMPFEmployeeShare = dBaseSalaryAmt / 2;
                                                }  
                                            }
                                        }
                                        else
                                        {
                                            dSSSEmployeeShare = dBaseSalaryAmt;
                                            dMPFEmployeeShare = dBaseSalaryAmt;
                                        }
                                            
                                        //SSS Employer Premium and EC Fund Premium
                                        decimal dSSSSalaryBracket = 0;
                                        drPremiumContribution = dtPremiumContribution.Select(string.Format("Mps_DeductionCode = 'SSSPREM' AND Mps_EEShare = {0} AND Mps_PayCycle = '{1}'", drEmpPayroll["Tpy_SSSShare"].ToString(), MaxSSSPremPayCycle));
                                        if (drPremiumContribution.Length > 0) //if fixed contribution bracket is found
                                            dSSSSalaryBracket = Convert.ToDecimal(drPremiumContribution[0]["Mps_CompensationFrom"].ToString());

                                        drPremiumContribution = dtPremiumContribution.Select(string.Format("Mps_DeductionCode = 'SSSPREM' AND Mps_CompensationFrom = {0} AND Mps_PayCycle = '{1}'", dSSSSalaryBracket, MaxSSSPremPayCycle));
                                        if (drPremiumContribution.Length > 0)
                                        {
                                            if (ApplicSSSPrem.Equals("0"))
                                            {
                                                if (SSSEARLYCYCLEFULLDEDN || bMonthEnd) //SHOULD BE FULL IN FIRST RUN
                                                {
                                                    dSSSEmployerShare += Convert.ToDecimal(drPremiumContribution[0]["Mps_ERShare"].ToString()) - dMonthlySSSEmployerShare;
                                                    dECFundAmt += Convert.ToDecimal(drPremiumContribution[0]["Mps_ECShare"].ToString()) - dMonthlyECFundAmt;
                                                    dMPFEmployerShare += Convert.ToDecimal(drPremiumContribution[0]["Mps_MPFERShare"].ToString()) - dMonthlyMPFEmployerShare;
                                                }   
                                                else
                                                {
                                                    if (PAYFREQNCY == "W")
                                                    {
                                                        dSSSEmployerShare += Convert.ToDecimal(drPremiumContribution[0]["Mps_ERShare"].ToString()) / PAYFREQNCYCOUNT;
                                                        dECFundAmt += Convert.ToDecimal(drPremiumContribution[0]["Mps_ECShare"].ToString()) / PAYFREQNCYCOUNT;
                                                        dMPFEmployerShare += Convert.ToDecimal(drPremiumContribution[0]["Mps_MPFERShare"].ToString()) / PAYFREQNCYCOUNT;
                                                    } 
                                                    else //Semi
                                                    {
                                                        dSSSEmployerShare += Convert.ToDecimal(drPremiumContribution[0]["Mps_ERShare"].ToString()) / 2;
                                                        dECFundAmt += Convert.ToDecimal(drPremiumContribution[0]["Mps_ECShare"].ToString()) / 2;
                                                        dMPFEmployerShare += Convert.ToDecimal(drPremiumContribution[0]["Mps_MPFERShare"].ToString()) / 2;
                                                    }
                                                }
                                            } 
                                            else
                                            {
                                                dSSSEmployerShare += Convert.ToDecimal(drPremiumContribution[0]["Mps_ERShare"].ToString());
                                                dECFundAmt += Convert.ToDecimal(drPremiumContribution[0]["Mps_ECShare"].ToString());
                                                dMPFEmployerShare += Convert.ToDecimal(drPremiumContribution[0]["Mps_MPFERShare"].ToString());
                                            } 
                                        }
                                    }
                                    /////Other Else ("T", "R", "O", "A", "B")
                                    else if (strSSSRule.Equals("T") || strSSSRule.Equals("R") || strSSSRule.Equals("O") || strSSSRule.Equals("A") || strSSSRule.Equals("B") || strSSSRule.Equals("M"))
                                    {
                                        amount = dBaseSalaryAmt;
                                        //Check if SSS premium will look in first quincena of the month
                                        if (SSSREFERPREVINCOMEPREM)
                                        {
                                            if (dMonthlySSSEmployeeShare == 0 && ApplicSSSPrem.Equals("0") && strSSSRule.Equals("B") & bMonthEnd) 
                                                amount = dBaseSalaryAmt * 2;
                                            else
                                                amount = dBaseSalaryAmt + dMonthlyBaseSalaryAmt;
                                        }

                                        //SSS Premium
                                        drPremiumContribution = dtPremiumContribution.Select(string.Format("Mps_DeductionCode = 'SSSPREM' AND Mps_CompensationFrom <= {0} AND Mps_PayCycle = '{1}'", amount, MaxSSSPremPayCycle), "Mps_CompensationFrom DESC");
                                        if (drPremiumContribution.Length > 0)
                                        {
                                            dSSSEmployeeShare       += Convert.ToDecimal(drPremiumContribution[0]["Mps_EEShare"].ToString());
                                            dSSSEmployerShare       += Convert.ToDecimal(drPremiumContribution[0]["Mps_ERShare"].ToString());
                                            dECFundAmt              += Convert.ToDecimal(drPremiumContribution[0]["Mps_ECShare"].ToString());
                                            dMPFEmployeeShare       += Convert.ToDecimal(drPremiumContribution[0]["Mps_MPFEEShare"].ToString());
                                            dMPFEmployerShare       += Convert.ToDecimal(drPremiumContribution[0]["Mps_MPFERShare"].ToString());
                                        }

                                        if (SSSREFERPREVINCOMEPREM)
                                        {
                                            dSSSEmployeeShare       -= dMonthlySSSEmployeeShare;
                                            dSSSEmployerShare       -= dMonthlySSSEmployerShare;
                                            dECFundAmt              -= dMonthlyECFundAmt;
                                            dMPFEmployeeShare       -= dMonthlyMPFEmployeeShare;
                                            dMPFEmployerShare       -= dMonthlyMPFEmployerShare;
                                        }
                                    }
                                    #endregion

                                    //Save SSS and EC premiums
                                    dSSSEmployeeShare                       = Math.Round(dSSSEmployeeShare, 2);
                                    drEmpPayroll["Tpy_SSSEE"]               = dSSSEmployeeShare;
                                    dSSSEmployerShare                       = Math.Round(dSSSEmployerShare, 2);
                                    drEmpPayroll["Tpy_SSSER"]               = dSSSEmployerShare;
                                    dECFundAmt                              = Math.Round(dECFundAmt, 2);
                                    drEmpPayroll["Tpy_ECFundAmt"]           = dECFundAmt;

                                    dMPFEmployeeShare                       = Math.Round(dMPFEmployeeShare, 2);
                                    drEmpPayroll["Tpy_MPFEE"]               = dMPFEmployeeShare;
                                    dMPFEmployerShare                       = Math.Round(dMPFEmployerShare, 2);
                                    drEmpPayroll["Tpy_MPFER"]               = dMPFEmployerShare;

                                    if (SSSREFERPREVINCOMEPREM)
                                    {
                                        if (dMonthlySSSEmployeeShare == 0 && ApplicSSSPrem.Equals("0") && strSSSRule.Equals("B") & bMonthEnd)
                                            dBaseSalaryAmt = dBaseSalaryAmt * 2;
                                        else
                                            dBaseSalaryAmt += dMonthlyBaseSalaryAmt;
                                    }

                                    dBaseSalaryAmt                          = Math.Round(dBaseSalaryAmt, 2);
                                    drEmpPayroll["Tpy_SSSBaseAmt"]          = dBaseSalaryAmt;

                                }
                                #endregion

                                #region Compute PhilHealth Premium
                                PAYFREQNCYCOUNT = GetPremiumPayCounter(ProcessPayrollPeriod, "PHICPREM");
                                strPHRule = drEmpPayroll["Tpy_PhilhealthRule"].ToString();
                                if (bComputePH && !strPHRule.Equals("N"))
                                {
                                    dBaseSalaryAmt          = 0;
                                    dMonthlyBaseSalaryAmt   = 0;
                                    dPHEmployeeShare        = 0;
                                    dPHEmployerShare        = 0;
                                    dMonthlyPHEmployeeShare = 0;
                                    dMonthlyPHEmployerShare = 0;

                                    #region Initialization

                                    dBaseSalaryAmt = ComputeBaseAmount(true, curEmployeeId
                                                       , ProcessPayrollPeriod
                                                       , strPHRule
                                                       , Convert.ToDecimal(drEmpPayroll["Tpy_TotalTaxableIncomeAmt"].ToString())
                                                       , Convert.ToDecimal(drEmpPayroll["Tpy_TotalREGAmt"].ToString())
                                                       , Convert.ToDecimal(drEmpPayroll["Tpy_SRGAdjAmt"].ToString())
                                                              + Convert.ToDecimal(drEmpPayroll["Tpy_SHOLAdjAmt"].ToString())
                                                              + Convert.ToDecimal(drEmpPayroll["Tpy_SLVAdjAmt"].ToString())
                                                              + Convert.ToDecimal(drEmpPayroll["Tpy_MRGAdjAmt"].ToString())
                                                              + Convert.ToDecimal(drEmpPayroll["Tpy_MHOLAdjAmt"].ToString())
                                                       , Convert.ToDecimal(drEmpPayroll["Tpy_TaxableIncomeAmt"].ToString())
                                                       , drEmpPayroll["Tpy_SalaryRate"].ToString()
                                                       , drEmpPayroll["Tpy_PayrollType"].ToString()
                                                       , drEmpPayroll["Tpy_PhilhealthShare"].ToString()
                                                       , PhilHealthBaseMaxShare
                                                       , ApplicPHPrem
                                                       , PHEARLYCYCLEFULLDEDN
                                                       , PHREFERPREVINCOMEPREM);

                                    

                                    if (drArrEmpPayrollYearly.Length > 0)
                                    {
                                        dMonthlyBaseSalaryAmt = ComputeBaseAmount(false, curEmployeeId
                                                       , PayrollYearMonth
                                                       , strPHRule
                                                       , Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_TotalTaxableIncomeAmt"].ToString())
                                                       , Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_TotalREGAmt"].ToString())
                                                       , Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_SRGAdjAmt"].ToString())
                                                              + Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_SHOLAdjAmt"].ToString())
                                                              + Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_SLVAdjAmt"].ToString())
                                                              + Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_MRGAdjAmt"].ToString())
                                                              + Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_MHOLAdjAmt"].ToString())
                                                       , Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_TaxableIncomeAmt"].ToString())
                                                       , drArrEmpPayrollYearly[0]["Tpy_SalaryRate"].ToString()
                                                       , drArrEmpPayrollYearly[0]["Tpy_PayrollType"].ToString()
                                                       , "0"
                                                       , "0"
                                                       , ApplicPHPrem
                                                       , PHEARLYCYCLEFULLDEDN
                                                       , PHREFERPREVINCOMEPREM);

                                        dMonthlyPHEmployeeShare = Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_PhilhealthEE"].ToString());
                                        dMonthlyPHEmployerShare = Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_PhilhealthER"].ToString());
                                    }
                                    #endregion

                                    #region Computation
                                    /////Fixed Contribution/Special/Maximum
                                    if (strPHRule.Equals("F") || strPHRule.Equals("S"))
                                    {
                                        //PhilHealth Employee Premium
                                        if (ApplicPHPrem.Equals("0"))
                                        {
                                            if ((PHEARLYCYCLEFULLDEDN || bMonthEnd) && dBaseSalaryAmt != 0)  //SHOULD BE FULL IN FIRST RUN
                                                dPHEmployeeShare = dBaseSalaryAmt - dMonthlyPHEmployeeShare;
                                            else
                                            {
                                                if (PAYFREQNCY == "W")
                                                    dPHEmployeeShare = dBaseSalaryAmt / PAYFREQNCYCOUNT;
                                                else //Semi
                                                    dPHEmployeeShare = dBaseSalaryAmt / 2;
                                            }
                                        }
                                        else
                                            dPHEmployeeShare = dBaseSalaryAmt;

                                        dPHEmployerShare = dPHEmployeeShare; //same amount for EE and ER
                                    }
                                    /////Other Else ("T", "R", "O", "A")
                                    else if (strPHRule.Equals("T") || strPHRule.Equals("R") || strPHRule.Equals("O") || strPHRule.Equals("A"))
                                    {
                                        amount = dBaseSalaryAmt;
                                        //Check if PH premium will look in first quincena of the month
                                        if (PHREFERPREVINCOMEPREM)
                                            amount = dBaseSalaryAmt + dMonthlyBaseSalaryAmt;

                                        //PhilHealth Premium
                                        drPremiumContribution = dtPremiumContribution.Select(string.Format("Mps_DeductionCode = 'PHICPREM' AND Mps_CompensationFrom <= {0} AND Mps_PayCycle = '{1}'", amount, MaxPhilHealthPremPayCycle), "Mps_CompensationFrom DESC");
                                        if (drPremiumContribution.Length > 0)
                                        {
                                            if (Convert.ToBoolean(drPremiumContribution[0]["Mps_IsPercentage"]) == false)
                                            {
                                                dPHEmployeeShare += Convert.ToDecimal(drPremiumContribution[0]["Mps_EEShare"].ToString());
                                                dPHEmployerShare += Convert.ToDecimal(drPremiumContribution[0]["Mps_ERShare"].ToString());
                                            }
                                            else
                                            {
                                                dPHEmployeeShare += (Convert.ToDecimal(drPremiumContribution[0]["Mps_EEShare"].ToString()) / 100.0m) * amount;
                                                dPHEmployerShare += (Convert.ToDecimal(drPremiumContribution[0]["Mps_ERShare"].ToString()) / 100.0m) * amount;
                                            }
                                        }
                                        //Deduct first quincena addon
                                        if (PHREFERPREVINCOMEPREM)
                                        {
                                            dPHEmployeeShare -= dMonthlyPHEmployeeShare;
                                            dPHEmployerShare -= dMonthlyPHEmployerShare;
                                        }

                                    }
                                    else if (strPHRule.Equals("B") || strPHRule.Equals("M")) 
                                    {
                                        if (bMonthEnd && dMonthlyPHEmployeeShare != 0 && PHEARLYCYCLEFULLDEDN)
                                        {
                                            dBaseSalaryAmt          = 0;
                                            dMonthlyBaseSalaryAmt   = 0;
                                            dPHEmployeeShare        = 0;
                                            dPHEmployerShare        = 0;
                                            dMonthlyPHEmployeeShare = 0;
                                            dMonthlyPHEmployerShare = 0;
                                        }
                                        else
                                        {
                                            amount = dBaseSalaryAmt;
                                            //Check if PH premium will look in first quincena of the month
                                            if (PHREFERPREVINCOMEPREM)
                                            {
                                                if (dMonthlyPHEmployeeShare == 0 && ApplicPHPrem.Equals("0") && strPHRule.Equals("B") & bMonthEnd)
                                                    amount = dBaseSalaryAmt * 2;
                                                else
                                                    amount = dBaseSalaryAmt + dMonthlyBaseSalaryAmt;
                                            }

                                            //PhilHealth Premium
                                            drPremiumContribution = dtPremiumContribution.Select(string.Format("Mps_DeductionCode = 'PHICPREM' AND Mps_CompensationFrom <= {0} AND Mps_PayCycle = '{1}'", amount, MaxPhilHealthPremPayCycle), "Mps_CompensationFrom DESC");
                                            if (drPremiumContribution.Length > 0)
                                            {
                                                if (Convert.ToBoolean(drPremiumContribution[0]["Mps_IsPercentage"]) == false)
                                                {
                                                    dPHEmployeeShare += Convert.ToDecimal(drPremiumContribution[0]["Mps_EEShare"].ToString());
                                                    dPHEmployerShare += Convert.ToDecimal(drPremiumContribution[0]["Mps_ERShare"].ToString());
                                                }
                                                else
                                                {
                                                    dPHEmployeeShare += (Convert.ToDecimal(drPremiumContribution[0]["Mps_EEShare"].ToString()) / 100.0m) * amount;
                                                    dPHEmployerShare += (Convert.ToDecimal(drPremiumContribution[0]["Mps_ERShare"].ToString()) / 100.0m) * amount;
                                                }
                                            }
                                            //Deduct first quincena addon
                                            if (PHREFERPREVINCOMEPREM)
                                            {
                                                dPHEmployeeShare -= dMonthlyPHEmployeeShare;
                                                dPHEmployerShare -= dMonthlyPHEmployerShare;
                                            }
                                        }
                                    }
                                    #endregion

                                    //Save PhilHealth premiums
                                    dPHEmployeeShare                        = Math.Round(dPHEmployeeShare, 2);
                                    drEmpPayroll["Tpy_PhilhealthEE"]        = dPHEmployeeShare;
                                    dPHEmployerShare                        = Math.Round(dPHEmployerShare, 2);
                                    drEmpPayroll["Tpy_PhilhealthER"]        = dPHEmployerShare;

                                    if (PHREFERPREVINCOMEPREM)
                                    {
                                        if (dMonthlyPHEmployeeShare == 0 && ApplicPHPrem.Equals("0") && strPHRule.Equals("B") & bMonthEnd)
                                            dBaseSalaryAmt = dBaseSalaryAmt * 2;
                                        else
                                            dBaseSalaryAmt += dMonthlyBaseSalaryAmt;
                                    }

                                    dBaseSalaryAmt                          = Math.Round(dBaseSalaryAmt, 2);
                                    drEmpPayroll["Tpy_PhilhealthBaseAmt"]   = dBaseSalaryAmt;

                                }
                                #endregion

                                #region Compute Union Dues
                                strUNIONRule = drEmpPayroll["Tpy_UnionRule"].ToString(); 
                                if (bComputeUnion && !strUNIONRule.Equals("N"))
                                {
                                    dBaseSalaryAmt                          = 0;
                                    dMonthlyBaseSalaryAmt                   = 0;
                                    dUNIONEmployeeShare                     = 0;
                                    dMonthlyUNIONEmployeeShare              = 0;

                                    #region Initialization

                                    dBaseSalaryAmt = ComputeBaseAmount(true, curEmployeeId
                                                   , ProcessPayrollPeriod
                                                   , strUNIONRule
                                                   , Convert.ToDecimal(drEmpPayroll["Tpy_TotalTaxableIncomeAmt"].ToString())
                                                   , Convert.ToDecimal(drEmpPayroll["Tpy_TotalREGAmt"].ToString())
                                                   , Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_SRGAdjAmt"].ToString())
                                                              + Convert.ToDecimal(drEmpPayroll["Tpy_SHOLAdjAmt"].ToString())
                                                              + Convert.ToDecimal(drEmpPayroll["Tpy_SLVAdjAmt"].ToString())
                                                              + Convert.ToDecimal(drEmpPayroll["Tpy_MRGAdjAmt"].ToString())
                                                              + Convert.ToDecimal(drEmpPayroll["Tpy_MHOLAdjAmt"].ToString())
                                                   , Convert.ToDecimal(drEmpPayroll["Tpy_TaxableIncomeAmt"].ToString())
                                                   , drEmpPayroll["Tpy_SalaryRate"].ToString()
                                                   , drEmpPayroll["Tpy_PayrollType"].ToString()
                                                   , drEmpPayroll["Tpy_UnionShare"].ToString()
                                                   , "0"
                                                   , ApplicUnionPrem
                                                   , UNIONEARLYCYCLEFULLDEDN
                                                   , UNIONREFERPREVINCOMEPREM);


                                    if (drArrEmpPayrollYearly.Length > 0)
                                    {

                                        dMonthlyBaseSalaryAmt = ComputeBaseAmount(false, curEmployeeId
                                                   , PayrollYearMonth
                                                   , strUNIONRule
                                                   , Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_TotalTaxableIncomeAmt"].ToString())
                                                   , Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_TotalREGAmt"].ToString())
                                                   , Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_SRGAdjAmt"].ToString())
                                                                    + Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_SHOLAdjAmt"].ToString())
                                                                    + Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_SLVAdjAmt"].ToString())
                                                                    + Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_MRGAdjAmt"].ToString())
                                                                    + Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_MHOLAdjAmt"].ToString())
                                                   , Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_TaxableIncomeAmt"].ToString())
                                                   , drArrEmpPayrollYearly[0]["Tpy_SalaryRate"].ToString()
                                                   , drArrEmpPayrollYearly[0]["Tpy_PayrollType"].ToString()
                                                   , "0"
                                                   , "0"
                                                   , ApplicUnionPrem
                                                   , UNIONEARLYCYCLEFULLDEDN
                                                   , UNIONREFERPREVINCOMEPREM);

                                        dMonthlyUNIONEmployeeShare = Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_UnionAmt"].ToString());
                                    }
                                    #endregion

                                    #region Computation
                                    /////Fixed Contribution/Special/Maximum
                                    if (strUNIONRule.Equals("F") || strUNIONRule.Equals("S") || strUNIONRule.Equals("M"))
                                    {
                                        if (ApplicUnionPrem.Equals("0"))
                                        {
                                            if ((UNIONEARLYCYCLEFULLDEDN || bMonthEnd) && dBaseSalaryAmt != 0)
                                                dUNIONEmployeeShare = dBaseSalaryAmt - dMonthlyUNIONEmployeeShare;
                                        }
                                        else
                                            dUNIONEmployeeShare = dBaseSalaryAmt;
                                    }
                                    /////Other Else ("G", "R", "O", "A", "B")
                                    else if (strUNIONRule.Equals("T") || strUNIONRule.Equals("R") || strUNIONRule.Equals("O") || strUNIONRule.Equals("A") || strUNIONRule.Equals("B"))
                                    {
                                        amount = dBaseSalaryAmt;
                                        //Check if Other premium will look in first quincena of the month
                                        if (UNIONREFERPREVINCOMEPREM)
                                            amount = dBaseSalaryAmt + dMonthlyBaseSalaryAmt;

                                        //Union Dues
                                        drPremiumContribution = dtPremiumContribution.Select(string.Format("Mps_DeductionCode = 'UNIONDUES' AND Mps_CompensationFrom <= {0} AND Mps_PayCycle <= '{1}'", amount, ProcessPayrollPeriod), "Mps_PayCycle DESC, Mps_CompensationFrom DESC");
                                        if (drPremiumContribution.Length > 0)
                                        {
                                            if (Convert.ToBoolean(drPremiumContribution[0]["Mps_IsPercentage"]) == false)
                                            {
                                                dUNIONEmployeeShare += Convert.ToDecimal(drPremiumContribution[0]["Mps_EEShare"].ToString());
                                            }
                                            else
                                            {
                                                dUNIONEmployeeShare += (Convert.ToDecimal(drPremiumContribution[0]["Mps_EEShare"].ToString()) / 100.0m) * amount;
                                            }
                                        }
                                        if (UNIONREFERPREVINCOMEPREM)
                                        {
                                            dUNIONEmployeeShare -= dMonthlyUNIONEmployeeShare;
                                        }
                                    }
                                    #endregion

                                    //Save Union Dues
                                    dUNIONEmployeeShare                         = Math.Round(dUNIONEmployeeShare, 2);
                                    drEmpPayroll["Tpy_UnionAmt"]                = dUNIONEmployeeShare;

                                    if (UNIONREFERPREVINCOMEPREM)
                                        dBaseSalaryAmt += dMonthlyBaseSalaryAmt;

                                    dBaseSalaryAmt                              = Math.Round(dBaseSalaryAmt, 2);
                                    drEmpPayroll["Tpy_UnionBaseAmt"]            = dBaseSalaryAmt;

                                }
                                #endregion

                                #region Check Premium Payment Capacity
                                if (ProcessSeparated)
                                {
                                    //No Government Premiums
                                    drEmpPayroll["Tpy_PagIbigEE"]               = 0;
                                    drEmpPayroll["Tpy_PagIbigER"]               = 0;
                                    drEmpPayroll["Tpy_PagIbigTaxEE"]            = 0;
                                    drEmpPayroll["Tpy_PagIbigBaseAmt"]          = 0;
                                    drEmpPayroll["Tpy_SSSEE"]                   = 0;
                                    drEmpPayroll["Tpy_SSSER"]                   = 0;
                                    drEmpPayroll["Tpy_ECFundAmt"]               = 0;
                                    drEmpPayroll["Tpy_SSSBaseAmt"]              = 0;
                                    drEmpPayroll["Tpy_PhilhealthEE"]            = 0;
                                    drEmpPayroll["Tpy_PhilhealthER"]            = 0;
                                    drEmpPayroll["Tpy_PhilhealthBaseAmt"]       = 0;
                                    drEmpPayroll["Tpy_UnionAmt"]                = 0;
                                    drEmpPayroll["Tpy_UnionBaseAmt"]            = 0;
                                    drEmpPayroll["Tpy_MPFEE"]                   = 0;
                                    drEmpPayroll["Tpy_MPFER"]                   = 0;
                                }
                                else
                                {
                                    #region DYNAMIC DEDUCTION OF PREMIUMS
                                    DataTable dtOrdering = Ordering(centralProfile);
                                    dTempNetPay = GrossPayAmt;
                                    for (int j = 0; j < dtOrdering.Rows.Count; j++)
                                    {
                                        string dedCode = dtOrdering.Rows[j][0].ToString(); 
                                        if (dedCode == "HDMFPREM")
                                        {
                                            dTempNetPay = dTempNetPay - dHDMFEmployeeShare;
                                            if (dTempNetPay < 0)
                                            {
                                                dTempNetPay = dTempNetPay + dHDMFEmployeeShare;
                                                //Set HDMF contribution to 0 amount
                                                drEmpPayroll["Tpy_PagIbigEE"]    = 0;
                                                drEmpPayroll["Tpy_PagIbigER"]    = 0;
                                                drEmpPayroll["Tpy_PagIbigTaxEE"] = 0;
                                            }
                                        }
                                        else if (dedCode == "PHICPREM")
                                        {
                                            dTempNetPay = dTempNetPay - dPHEmployeeShare;
                                            if (dTempNetPay < 0)
                                            {
                                                dTempNetPay = dTempNetPay + dPHEmployeeShare;
                                                //Set PhilHealth contribution to 0 amount
                                                drEmpPayroll["Tpy_PhilhealthEE"] = 0;
                                                drEmpPayroll["Tpy_PhilhealthER"] = 0;
                                            }
                                        }
                                        else if (dedCode == "SSSPREM")
                                        {
                                            dTempNetPay = dTempNetPay - dSSSEmployeeShare;
                                            if (dTempNetPay < 0)
                                            {
                                                dTempNetPay = dTempNetPay + dSSSEmployeeShare;
                                                //Set SSS contribution to 0 amount
                                                drEmpPayroll["Tpy_SSSEE"]       = 0;
                                                drEmpPayroll["Tpy_SSSER"]       = 0;
                                                drEmpPayroll["Tpy_ECFundAmt"]   = 0;
                                                drEmpPayroll["Tpy_MPFEE"]       = 0;
                                                drEmpPayroll["Tpy_MPFER"]       = 0;
                                            }
                                        }
                                        else if (dedCode == "UNIONDUES")
                                        {
                                            dTempNetPay = dTempNetPay - dUNIONEmployeeShare;
                                            if (dTempNetPay < 0)
                                            {
                                                dTempNetPay = dTempNetPay + dUNIONEmployeeShare;
                                                //Set Union Dues contribution to 0                                                    
                                                drEmpPayroll["Tpy_UnionAmt"] = 0;
                                            }
                                        }
                                        
                                    }
                                    #endregion
                                }
                                #endregion

                                #region Compute Year-To-Date Totals
                                PopulateYTDValues(curEmployeeId
                                                    , drEmpPayroll
                                                    , drEmpPayroll2
                                                    , dtEmpPayrollYearly.Select("Tpy_IDNo = '" + curEmployeeId + "'")
                                                    , null
                                                    , dtEmpPayroll2Yearly.Select("Tpy_IDNo = '" + curEmployeeId + "'")
                                                    , null
                                                    , dsAllYTDAdjustment.Tables[0].Select("Tyt_IDNo = '" + curEmployeeId + "'")
                                                    , dsAllYTDAdjustment.Tables[1].Select("Tyt_IDNo = '" + curEmployeeId + "'")
                                                    , null
                                                    , (Annualize)); //|| TaxBase.Equals("7")
                                #endregion

                                if (ProcessSeparated)
                                {
                                    drEmpPayroll["Tpy_TaxCode"]             = ""; //Empty TaxCode
                                    GrossIncomeAmt                          = Convert.ToDecimal(drEmpPayroll["Tpy_TotalTaxableIncomeAmt"].ToString()) + Convert.ToDecimal(drEmpPayroll["Tpy_NontaxableIncomeAmt"].ToString());
                                    GrossIncomeAmt                          = Math.Round(GrossIncomeAmt, 2);
                                    drEmpPayroll["Tpy_GrossIncomeAmt"]      = GrossIncomeAmt;
                                }
                                else
                                {
                                    #region Compute Tax
                                    dWTaxAmt = 0;
                                    strTaxRule = drEmpPayroll["Tpy_TaxRule"].ToString();
                                    if (bComputeTax && !strTaxRule.Equals("N"))
                                    {
                                        ComputeWithholdingTax(Annualize
                                                            , ProcessSeparated
                                                            , drEmpPayroll
                                                            , drEmpPayroll2
                                                            , (strTaxRule == "N" ? false : true)
                                                            , curEmployeeId
                                                            , ProcessPayrollPeriod
                                                            , PayrollCycle
                                                            , PayrollYearMonth
                                                            , dal);
                                        dWTaxAmt = Math.Round(Convert.ToDecimal(drEmpPayroll["Tpy_WtaxAmt"]), 2);
                                        drEmpPayroll["Tpy_WtaxAmt"] = dWTaxAmt;
                                    }

                                    #endregion

                                    #region Compute net pay and other deductions
                                    dWTaxRef = 0;
                                    dPartialDeduction = 0;
                                    dCurrentNetPay = 0;
                                    dMinimumNetPay = 0;

                                    ////Hold Payroll (FEP)
                                    //if (bPostHoldPayroll == true)
                                    //{
                                    //drEmpPayroll["Tpy_NontaxableAdjAmt"] = GetHoldPayAmount(curEmployeeId);
                                    //}

                                    //Only deduct tax if the amount is positive
                                    if (dWTaxAmt < 0)
                                    {
                                        dWTaxRef = dWTaxAmt;
                                        dWTaxRef = dWTaxRef * -1;
                                        dWTaxAmt = 0;
                                    }

                                    dPartialDeduction = dWTaxAmt +
                                                          Convert.ToDecimal(drEmpPayroll["Tpy_SSSEE"].ToString()) +
                                                          Convert.ToDecimal(drEmpPayroll["Tpy_MPFEE"].ToString()) +
                                                          Convert.ToDecimal(drEmpPayroll["Tpy_PhilhealthEE"].ToString()) +
                                                          Convert.ToDecimal(drEmpPayroll["Tpy_PagIbigEE"].ToString()) +
                                                          Convert.ToDecimal(drEmpPayroll["Tpy_PagIbigTaxEE"].ToString()) +
                                                          Convert.ToDecimal(drEmpPayroll["Tpy_UnionAmt"].ToString()) +
                                                          Convert.ToDecimal(drEmpPayroll["Tpy_OtherDeductionAmt"].ToString());

                                    dCurrentNetPay = Convert.ToDecimal(drEmpPayroll["Tpy_TotalTaxableIncomeAmt"].ToString()) -
                                                       dPartialDeduction +
                                                       dWTaxRef +
                                                       Convert.ToDecimal(drEmpPayroll["Tpy_NontaxableIncomeAmt"].ToString()); 

                                    //Get gross pay percentage
                                    if (NETBASE == "T")
                                        dMinimumNetPay = Convert.ToDecimal(drEmpPayroll["Tpy_TotalTaxableIncomeAmt"].ToString()) * (NETPCT / 100);
                                    else if (NETBASE == "G")
                                        dMinimumNetPay = (Convert.ToDecimal(drEmpPayroll["Tpy_TotalTaxableIncomeAmt"].ToString())
                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_NontaxableIncomeAmt"].ToString()))
                                                                * (NETPCT / 100);

                                    //Compare with minimum take home pay parameter
                                    if (MINNETPAY > 0 && dMinimumNetPay < MINNETPAY)
                                        dMinimumNetPay = MINNETPAY;

                                    dOtherDeduction = 0;
                                    dsDeductions = GetDeductionDetails(curEmployeeId);
                                    decimal deductionAmt = 0;
                                    foreach (DataRow drDeduction in dsDeductions.Tables[0].Rows)
                                    {
                                        deductionAmt = Convert.ToDecimal(drDeduction["Tdd_Amount"].ToString());
                                        if (dCurrentNetPay - Convert.ToDecimal(drDeduction["Tdd_Amount"].ToString()) >= dMinimumNetPay)
                                        {
                                            dCurrentNetPay -= Convert.ToDecimal(drDeduction["Tdd_Amount"].ToString());
                                            UpdateEmployeeDeductionDetail(drDeduction);
                                            dOtherDeduction += Convert.ToDecimal(drDeduction["Tdd_Amount"].ToString());
                                        }
                                        else if (Convert.ToBoolean(drDeduction["Mdn_IsAllowPartialPayment"].ToString()) && dCurrentNetPay >= dMinimumNetPay)
                                        {
                                            deductionAmt = SplitEmployeeDeductionDetail(dCurrentNetPay, dMinimumNetPay, drDeduction);
                                            dCurrentNetPay -= deductionAmt;
                                            dOtherDeduction += deductionAmt;
                                        }
                                    }
                                    //Update other deduction amount
                                    dOtherDeduction = Math.Round(dOtherDeduction, 2);
                                    drEmpPayroll["Tpy_OtherDeductionAmt"] = dOtherDeduction;

                                    dTotalDeduction = dWTaxAmt +
                                                      Convert.ToDecimal(drEmpPayroll["Tpy_SSSEE"].ToString()) +
                                                      Convert.ToDecimal(drEmpPayroll["Tpy_MPFEE"].ToString()) +
                                                      Convert.ToDecimal(drEmpPayroll["Tpy_PhilhealthEE"].ToString()) +
                                                      Convert.ToDecimal(drEmpPayroll["Tpy_PagIbigEE"].ToString()) +
                                                      Convert.ToDecimal(drEmpPayroll["Tpy_PagIbigTaxEE"].ToString()) +
                                                      Convert.ToDecimal(drEmpPayroll["Tpy_UnionAmt"].ToString()) +
                                                      Convert.ToDecimal(drEmpPayroll["Tpy_OtherDeductionAmt"].ToString());
                                    //Update total deduction amount
                                    dTotalDeduction = Math.Round(dTotalDeduction, 2);
                                    drEmpPayroll["Tpy_TotalDeductionAmt"] = dTotalDeduction;

                                    dNetPayAmt = Convert.ToDecimal(drEmpPayroll["Tpy_TotalTaxableIncomeAmt"].ToString()) +
                                                  Convert.ToDecimal(drEmpPayroll["Tpy_NontaxableIncomeAmt"].ToString()) +
                                                  dWTaxRef -
                                                  Convert.ToDecimal(drEmpPayroll["Tpy_TotalDeductionAmt"].ToString());
                                    //Update net pay amount
                                    dNetPayAmt = Math.Round(dNetPayAmt, 2);
                                    drEmpPayroll["Tpy_NetAmt"] = dNetPayAmt;

                                    //Update Gross Income Amount
                                    GrossIncomeAmt = Convert.ToDecimal(drEmpPayroll["Tpy_TotalTaxableIncomeAmt"].ToString()) +
                                                  Convert.ToDecimal(drEmpPayroll["Tpy_NontaxableIncomeAmt"].ToString()) +
                                                  dWTaxRef;
                                    GrossIncomeAmt = Math.Round(GrossIncomeAmt, 2);
                                    drEmpPayroll["Tpy_GrossIncomeAmt"] = GrossIncomeAmt;
                                    #endregion

                                    #region BIR
                                    string[] BirEmpStat = BIREMPSTATTOPROCESS.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries); 
                                    if (BirEmpStat.Length > 0 && Array.IndexOf(BirEmpStat, EmploymentStatus) >= 0)
                                        ComputeIncomeAndDeductionAdjustments(false, drEmpPayroll, drEmpPayroll2);
                                   #endregion

                                    #region Info Messages
                                    if (dNetPayAmt > MAXNETPAY)
                                        AddErrorToPayrollReport(drEmpPayroll["Tpy_IDNo"].ToString(), dtEmpPayTranHdr.Rows[i]["Mem_LastName"].ToString(), dtEmpPayTranHdr.Rows[i]["Mem_FirstName"].ToString(), ProcessPayrollPeriod, dNetPayAmt, 0, string.Format("Net Pay > {0:0.00}", MAXNETPAY));
                                    if (dOtherDeduction > MAXDEDN)
                                        AddErrorToPayrollReport(drEmpPayroll["Tpy_IDNo"].ToString(), dtEmpPayTranHdr.Rows[i]["Mem_LastName"].ToString(), dtEmpPayTranHdr.Rows[i]["Mem_FirstName"].ToString(), ProcessPayrollPeriod, dOtherDeduction, 0, string.Format("Other Ded > {0:0.00}", MAXDEDN));
                                    if (dNetPayAmt > 0 && dNetPayAmt < MINNETPAY)
                                        AddErrorToPayrollReport(drEmpPayroll["Tpy_IDNo"].ToString(), dtEmpPayTranHdr.Rows[i]["Mem_LastName"].ToString(), dtEmpPayTranHdr.Rows[i]["Mem_FirstName"].ToString(), ProcessPayrollPeriod, dNetPayAmt, 0, string.Format("Net Pay < {0:0.00} Min pay", MINNETPAY));
                                    if (dNetPayAmt <= 0)
                                        AddErrorToPayrollReport(drEmpPayroll["Tpy_IDNo"].ToString(), dtEmpPayTranHdr.Rows[i]["Mem_LastName"].ToString(), dtEmpPayTranHdr.Rows[i]["Mem_FirstName"].ToString(), ProcessPayrollPeriod, dNetPayAmt, 0, string.Format("Net Pay <= 0.00"));
                                    #endregion
                                }

                                //Add to payroll tables
                                dtEmpPayroll.Rows.Add(drEmpPayroll);
                                dtEmpPayroll2.Rows.Add(drEmpPayroll2);
                                if (bHasDayCodeExt)
                                    dtEmpPayrollMisc.Rows.Add(drEmpPayrollMisc);
                                EmpDispHandler(this, new EmpDispEventArgs(curEmployeeId, dtEmpPayTranHdr.Rows[i]["Mem_LastName"].ToString(), dtEmpPayTranHdr.Rows[i]["Mem_FirstName"].ToString()));
                            }
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            CommonProcedures.logErrorToFile(ex.ToString());
                            EmpDispHandler(this, new EmpDispEventArgs(curEmployeeId, dtEmpPayTranHdr.Rows[i]["Mem_LastName"].ToString(), dtEmpPayTranHdr.Rows[i]["Mem_FirstName"].ToString(), "Error in Payroll Calculation : " + ex.Message));
                            AddErrorToPayrollReport(curEmployeeId, dtEmpPayTranHdr.Rows[i]["Mem_LastName"].ToString(), dtEmpPayTranHdr.Rows[i]["Mem_FirstName"].ToString(), ProcessPayrollPeriod, 0, 0, ex.Message.Substring(0, Math.Min(ex.Message.Length, 1000)));
                        }
                    }
                }
                //-----------------------------END MAIN PROCESS
                //Save Payroll Table
                StatusHandler(this, new StatusEventArgs("Saving Payroll Records", false));
                string strUpdateRecordTemplate;
                string strUpdateQuery = "";
                int iUpdateCtr = 0;
                #region Payroll Record Insert
                strUpdateRecordTemplate = @" INSERT INTO {0} (Tpy_IDNo,Tpy_PayCycle,Tpy_SalaryRate,Tpy_HourRate,Tpy_SpecialRate,Tpy_SpecialHourRate,Tpy_LTHr,Tpy_LTAmt,Tpy_UTHr,Tpy_UTAmt,Tpy_UPLVHr,Tpy_UPLVAmt,Tpy_ABSLEGHOLHr,Tpy_ABSLEGHOLAmt,Tpy_ABSSPLHOLHr,Tpy_ABSSPLHOLAmt,Tpy_ABSCOMPHOLHr,Tpy_ABSCOMPHOLAmt,Tpy_ABSPSDHr,Tpy_ABSPSDAmt,Tpy_ABSOTHHOLHr,Tpy_ABSOTHHOLAmt,Tpy_WDABSHr,Tpy_WDABSAmt,Tpy_LTUTMaxHr,Tpy_LTUTMaxAmt,Tpy_ABSHr,Tpy_ABSAmt,Tpy_REGHr,Tpy_REGAmt,Tpy_PDLVHr,Tpy_PDLVAmt,Tpy_PDLEGHOLHr,Tpy_PDLEGHOLAmt,Tpy_PDSPLHOLHr,Tpy_PDSPLHOLAmt,Tpy_PDCOMPHOLHr,Tpy_PDCOMPHOLAmt,Tpy_PDPSDHr,Tpy_PDPSDAmt,Tpy_PDOTHHOLHr,Tpy_PDOTHHOLAmt,Tpy_PDRESTLEGHOLHr,Tpy_PDRESTLEGHOLAmt,Tpy_TotalREGHr,Tpy_TotalREGAmt,Tpy_REGOTHr,Tpy_REGOTAmt,Tpy_REGNDHr,Tpy_REGNDAmt,Tpy_REGNDOTHr,Tpy_REGNDOTAmt,Tpy_RESTHr,Tpy_RESTAmt,Tpy_RESTOTHr,Tpy_RESTOTAmt,Tpy_RESTNDHr,Tpy_RESTNDAmt,Tpy_RESTNDOTHr,Tpy_RESTNDOTAmt,Tpy_LEGHOLHr,Tpy_LEGHOLAmt,Tpy_LEGHOLOTHr,Tpy_LEGHOLOTAmt,Tpy_LEGHOLNDHr,Tpy_LEGHOLNDAmt,Tpy_LEGHOLNDOTHr,Tpy_LEGHOLNDOTAmt,Tpy_SPLHOLHr,Tpy_SPLHOLAmt,Tpy_SPLHOLOTHr,Tpy_SPLHOLOTAmt,Tpy_SPLHOLNDHr,Tpy_SPLHOLNDAmt,Tpy_SPLHOLNDOTHr,Tpy_SPLHOLNDOTAmt,Tpy_PSDHr,Tpy_PSDAmt,Tpy_PSDOTHr,Tpy_PSDOTAmt,Tpy_PSDNDHr,Tpy_PSDNDAmt,Tpy_PSDNDOTHr,Tpy_PSDNDOTAmt,Tpy_COMPHOLHr,Tpy_COMPHOLAmt,Tpy_COMPHOLOTHr,Tpy_COMPHOLOTAmt,Tpy_COMPHOLNDHr,Tpy_COMPHOLNDAmt,Tpy_COMPHOLNDOTHr,Tpy_COMPHOLNDOTAmt,Tpy_RESTLEGHOLHr,Tpy_RESTLEGHOLAmt,Tpy_RESTLEGHOLOTHr,Tpy_RESTLEGHOLOTAmt,Tpy_RESTLEGHOLNDHr,Tpy_RESTLEGHOLNDAmt,Tpy_RESTLEGHOLNDOTHr,Tpy_RESTLEGHOLNDOTAmt,Tpy_RESTSPLHOLHr,Tpy_RESTSPLHOLAmt,Tpy_RESTSPLHOLOTHr,Tpy_RESTSPLHOLOTAmt,Tpy_RESTSPLHOLNDHr,Tpy_RESTSPLHOLNDAmt,Tpy_RESTSPLHOLNDOTHr,Tpy_RESTSPLHOLNDOTAmt,Tpy_RESTCOMPHOLHr,Tpy_RESTCOMPHOLAmt,Tpy_RESTCOMPHOLOTHr,Tpy_RESTCOMPHOLOTAmt,Tpy_RESTCOMPHOLNDHr,Tpy_RESTCOMPHOLNDAmt,Tpy_RESTCOMPHOLNDOTHr,Tpy_RESTCOMPHOLNDOTAmt,Tpy_RESTPSDHr,Tpy_RESTPSDAmt,Tpy_RESTPSDOTHr,Tpy_RESTPSDOTAmt,Tpy_RESTPSDNDHr,Tpy_RESTPSDNDAmt,Tpy_RESTPSDNDOTHr,Tpy_RESTPSDNDOTAmt,Tpy_TotalOTNDAmt,Tpy_WorkDay,Tpy_DayCountOldSalaryRate,Tpy_SRGAdjHr,Tpy_SRGAdjAmt,Tpy_SOTAdjHr,Tpy_SOTAdjAmt,Tpy_SHOLAdjHr,Tpy_SHOLAdjAmt,Tpy_SNDAdjHr,Tpy_SNDAdjAmt,Tpy_SLVAdjHr,Tpy_SLVAdjAmt,Tpy_MRGAdjHr,Tpy_MRGAdjAmt,Tpy_MOTAdjHr,Tpy_MOTAdjAmt,Tpy_MHOLAdjHr,Tpy_MHOLAdjAmt,Tpy_MNDAdjHr,Tpy_MNDAdjAmt,Tpy_TotalAdjAmt,Tpy_TaxableIncomeAmt,Tpy_TotalTaxableIncomeAmt,Tpy_NontaxableIncomeAmt,Tpy_GrossIncomeAmt,Tpy_WtaxAmt,Tpy_TaxBaseAmt,Tpy_TaxRule,Tpy_TaxShare,Tpy_TaxBracket,Tpy_TaxCode,Tpy_IsTaxExempted,Tpy_SSSEE,Tpy_SSSER,Tpy_ECFundAmt,Tpy_SSSBaseAmt,Tpy_SSSRule,Tpy_SSSShare,Tpy_MPFEE,Tpy_MPFER,Tpy_PhilhealthEE,Tpy_PhilhealthER,Tpy_PhilhealthBaseAmt,Tpy_PhilhealthRule,Tpy_PhilhealthShare,Tpy_PagIbigEE,Tpy_PagIbigER,Tpy_PagIbigTaxEE,Tpy_PagIbigBaseAmt,Tpy_PagIbigRule,Tpy_PagIbigShare,Tpy_UnionAmt,Tpy_UnionRule,Tpy_UnionShare,Tpy_UnionBaseAmt,Tpy_OtherDeductionAmt,Tpy_TotalDeductionAmt,Tpy_NetAmt,Tpy_CostcenterCode,Tpy_PositionCode,Tpy_EmploymentStatus,Tpy_PayrollType,Tpy_PaymentMode,Tpy_BankCode,Tpy_BankAcctNo,Tpy_PayrollGroup,Tpy_CalendarGrpCode,Tpy_WorkLocationCode,Tpy_AltAcctCode,Tpy_ExpenseClass,Tpy_PositionGrade,Tpy_WorkStatus,Tpy_PremiumGrpCode,Tpy_IsComputedPerDay,Tpy_IsMultipleSalary,Tpy_RegRule,Usr_Login,Ludatetime) VALUES ('{1}','{2}',{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34},{35},{36},{37},{38},{39},{40},{41},{42},{43},{44},{45},{46},{47},{48},{49},{50},{51},{52},{53},{54},{55},{56},{57},{58},{59},{60},{61},{62},{63},{64},{65},{66},{67},{68},{69},{70},{71},{72},{73},{74},{75},{76},{77},{78},{79},{80},{81},{82},{83},{84},{85},{86},{87},{88},{89},{90},{91},{92},{93},{94},{95},{96},{97},{98},{99},{100},{101},{102},{103},{104},{105},{106},{107},{108},{109},{110},{111},{112},{113},{114},{115},{116},{117},{118},{119},{120},{121},{122},{123},{124},{125},{126},{127},{128},{129},{130},{131},{132},{133},{134},{135},{136},{137},{138},{139},{140},{141},{142},{143},{144},{145},{146},{147},{148},{149},{150},{151},{152},'{153}',{154},{155},'{156}','{157}',{158},{159},{160},{161},'{162}',{163},{164},{165},{166},{167},{168},'{169}',{170},{171},{172},{173},{174},'{175}',{176},{177},'{178}',{179},{180},{181},{182},{183},'{184}','{185}','{186}','{187}','{188}','{189}','{190}','{191}','{192}','{193}','{194}','{195}','{196}','{197}','{198}','{199}','{200}','{201}','{202}',GETDATE()) ";
                #endregion
                foreach (DataRow drPayrollCalc in dtEmpPayroll.Rows)
                {
                    #region Payroll Insert
                    strUpdateQuery += string.Format(strUpdateRecordTemplate
                                                       , EmpPayrollTable
                                                       , drPayrollCalc["Tpy_IDNo"]                          //1
                                                       , drPayrollCalc["Tpy_PayCycle"]                      //2
                                                       , drPayrollCalc["Tpy_SalaryRate"]                    //3
                                                       , drPayrollCalc["Tpy_HourRate"]                      //4
                                                       , drPayrollCalc["Tpy_SpecialRate"]                   //5    
                                                       , drPayrollCalc["Tpy_SpecialHourRate"]               //6    
                                                       , drPayrollCalc["Tpy_LTHr"]                  
                                                       , drPayrollCalc["Tpy_LTAmt"]                 
                                                       , drPayrollCalc["Tpy_UTHr"]                  
                                                       , drPayrollCalc["Tpy_UTAmt"]                         //10
                                                       , drPayrollCalc["Tpy_UPLVHr"]                
                                                       , drPayrollCalc["Tpy_UPLVAmt"]               
                                                       , drPayrollCalc["Tpy_ABSLEGHOLHr"]           
                                                       , drPayrollCalc["Tpy_ABSLEGHOLAmt"]          
                                                       , drPayrollCalc["Tpy_ABSSPLHOLHr"]                   //15
                                                       , drPayrollCalc["Tpy_ABSSPLHOLAmt"]          
                                                       , drPayrollCalc["Tpy_ABSCOMPHOLHr"]          
                                                       , drPayrollCalc["Tpy_ABSCOMPHOLAmt"]         
                                                       , drPayrollCalc["Tpy_ABSPSDHr"]              
                                                       , drPayrollCalc["Tpy_ABSPSDAmt"]                     //20
                                                       , drPayrollCalc["Tpy_ABSOTHHOLHr"]           
                                                       , drPayrollCalc["Tpy_ABSOTHHOLAmt"]          
                                                       , drPayrollCalc["Tpy_WDABSHr"]               
                                                       , drPayrollCalc["Tpy_WDABSAmt"]
                                                       , drPayrollCalc["Tpy_LTUTMaxHr"]                     //25
                                                       , drPayrollCalc["Tpy_LTUTMaxAmt"]
                                                       , drPayrollCalc["Tpy_ABSHr"]                         
                                                       , drPayrollCalc["Tpy_ABSAmt"]                
                                                       , drPayrollCalc["Tpy_REGHr"]                 
                                                       , drPayrollCalc["Tpy_REGAmt"]                        //30            
                                                       , drPayrollCalc["Tpy_PDLVHr"]                
                                                       , drPayrollCalc["Tpy_PDLVAmt"]                       
                                                       , drPayrollCalc["Tpy_PDLEGHOLHr"]            
                                                       , drPayrollCalc["Tpy_PDLEGHOLAmt"]           
                                                       , drPayrollCalc["Tpy_PDSPLHOLHr"]                    //35           
                                                       , drPayrollCalc["Tpy_PDSPLHOLAmt"]           
                                                       , drPayrollCalc["Tpy_PDCOMPHOLHr"]                   
                                                       , drPayrollCalc["Tpy_PDCOMPHOLAmt"]          
                                                       , drPayrollCalc["Tpy_PDPSDHr"]               
                                                       , drPayrollCalc["Tpy_PDPSDAmt"]                      //40            
                                                       , drPayrollCalc["Tpy_PDOTHHOLHr"]            
                                                       , drPayrollCalc["Tpy_PDOTHHOLAmt"]                   
                                                       , drPayrollCalc["Tpy_PDRESTLEGHOLHr"]        
                                                       , drPayrollCalc["Tpy_PDRESTLEGHOLAmt"]       
                                                       , drPayrollCalc["Tpy_TotalREGHr"]                    //45     
                                                       , drPayrollCalc["Tpy_TotalREGAmt"]           
                                                       , drPayrollCalc["Tpy_REGOTHr"]                       
                                                       , drPayrollCalc["Tpy_REGOTAmt"]             
                                                       , drPayrollCalc["Tpy_REGNDHr"]               
                                                       , drPayrollCalc["Tpy_REGNDAmt"]                      //50         
                                                       , drPayrollCalc["Tpy_REGNDOTHr"]             
                                                       , drPayrollCalc["Tpy_REGNDOTAmt"]                    
                                                       , drPayrollCalc["Tpy_RESTHr"]                
                                                       , drPayrollCalc["Tpy_RESTAmt"]               
                                                       , drPayrollCalc["Tpy_RESTOTHr"]                      //55
                                                       , drPayrollCalc["Tpy_RESTOTAmt"]             
                                                       , drPayrollCalc["Tpy_RESTNDHr"]                      
                                                       , drPayrollCalc["Tpy_RESTNDAmt"]             
                                                       , drPayrollCalc["Tpy_RESTNDOTHr"]            
                                                       , drPayrollCalc["Tpy_RESTNDOTAmt"]                   //60   
                                                       , drPayrollCalc["Tpy_LEGHOLHr"]              
                                                       , drPayrollCalc["Tpy_LEGHOLAmt"]                     
                                                       , drPayrollCalc["Tpy_LEGHOLOTHr"]            
                                                       , drPayrollCalc["Tpy_LEGHOLOTAmt"]           
                                                       , drPayrollCalc["Tpy_LEGHOLNDHr"]                    //65       
                                                       , drPayrollCalc["Tpy_LEGHOLNDAmt"]           
                                                       , drPayrollCalc["Tpy_LEGHOLNDOTHr"]                  
                                                       , drPayrollCalc["Tpy_LEGHOLNDOTAmt"]         
                                                       , drPayrollCalc["Tpy_SPLHOLHr"]              
                                                       , drPayrollCalc["Tpy_SPLHOLAmt"]                     //70           
                                                       , drPayrollCalc["Tpy_SPLHOLOTHr"]            
                                                       , drPayrollCalc["Tpy_SPLHOLOTAmt"]                   
                                                       , drPayrollCalc["Tpy_SPLHOLNDHr"]           
                                                       , drPayrollCalc["Tpy_SPLHOLNDAmt"]           
                                                       , drPayrollCalc["Tpy_SPLHOLNDOTHr"]                  //75          
                                                       , drPayrollCalc["Tpy_SPLHOLNDOTAmt"]         
                                                       , drPayrollCalc["Tpy_PSDHr"]                         
                                                       , drPayrollCalc["Tpy_PSDAmt"]                
                                                       , drPayrollCalc["Tpy_PSDOTHr"]               
                                                       , drPayrollCalc["Tpy_PSDOTAmt"]                      //80
                                                       , drPayrollCalc["Tpy_PSDNDHr"]               
                                                       , drPayrollCalc["Tpy_PSDNDAmt"]                      
                                                       , drPayrollCalc["Tpy_PSDNDOTHr"]             
                                                       , drPayrollCalc["Tpy_PSDNDOTAmt"]            
                                                       , drPayrollCalc["Tpy_COMPHOLHr"]                     //85
                                                       , drPayrollCalc["Tpy_COMPHOLAmt"]            
                                                       , drPayrollCalc["Tpy_COMPHOLOTHr"]                   
                                                       , drPayrollCalc["Tpy_COMPHOLOTAmt"]
                                                       , drPayrollCalc["Tpy_COMPHOLNDHr"]          
                                                       , drPayrollCalc["Tpy_COMPHOLNDAmt"]                  //90
                                                       , drPayrollCalc["Tpy_COMPHOLNDOTHr"]         
                                                       , drPayrollCalc["Tpy_COMPHOLNDOTAmt"]                
                                                       , drPayrollCalc["Tpy_RESTLEGHOLHr"]
                                                       , drPayrollCalc["Tpy_RESTLEGHOLAmt"]         
                                                       , drPayrollCalc["Tpy_RESTLEGHOLOTHr"]                //95
                                                       , drPayrollCalc["Tpy_RESTLEGHOLOTAmt"]       
                                                       , drPayrollCalc["Tpy_RESTLEGHOLNDHr"]                
                                                       , drPayrollCalc["Tpy_RESTLEGHOLNDAmt"]           
                                                       , drPayrollCalc["Tpy_RESTLEGHOLNDOTHr"]          
                                                       , drPayrollCalc["Tpy_RESTLEGHOLNDOTAmt"]             //100
                                                       , drPayrollCalc["Tpy_RESTSPLHOLHr"]          
                                                       , drPayrollCalc["Tpy_RESTSPLHOLAmt"]                 
                                                       , drPayrollCalc["Tpy_RESTSPLHOLOTHr"]
                                                       , drPayrollCalc["Tpy_RESTSPLHOLOTAmt"]           
                                                       , drPayrollCalc["Tpy_RESTSPLHOLNDHr"]                //105
                                                       , drPayrollCalc["Tpy_RESTSPLHOLNDAmt"]       
                                                       , drPayrollCalc["Tpy_RESTSPLHOLNDOTHr"]              
                                                       , drPayrollCalc["Tpy_RESTSPLHOLNDOTAmt"]
                                                       , drPayrollCalc["Tpy_RESTCOMPHOLHr"]             
                                                       , drPayrollCalc["Tpy_RESTCOMPHOLAmt"]                //110
                                                       , drPayrollCalc["Tpy_RESTCOMPHOLOTHr"]       
                                                       , drPayrollCalc["Tpy_RESTCOMPHOLOTAmt"]              
                                                       , drPayrollCalc["Tpy_RESTCOMPHOLNDHr"]
                                                       , drPayrollCalc["Tpy_RESTCOMPHOLNDAmt"]          
                                                       , drPayrollCalc["Tpy_RESTCOMPHOLNDOTHr"]             //115
                                                       , drPayrollCalc["Tpy_RESTCOMPHOLNDOTAmt"]    
                                                       , drPayrollCalc["Tpy_RESTPSDHr"]                     
                                                       , drPayrollCalc["Tpy_RESTPSDAmt"]
                                                       , drPayrollCalc["Tpy_RESTPSDOTHr"]               
                                                       , drPayrollCalc["Tpy_RESTPSDOTAmt"]                  //120
                                                       , drPayrollCalc["Tpy_RESTPSDNDHr"]           
                                                       , drPayrollCalc["Tpy_RESTPSDNDAmt"]                  
                                                       , drPayrollCalc["Tpy_RESTPSDNDOTHr"]
                                                       , drPayrollCalc["Tpy_RESTPSDNDOTAmt"]
                                                       , drPayrollCalc["Tpy_TotalOTNDAmt"]                  //125                       
                                                       , drPayrollCalc["Tpy_WorkDay"]
                                                       , drPayrollCalc["Tpy_DayCountOldSalaryRate"]         
                                                       , drPayrollCalc["Tpy_SRGAdjHr"]                      
                                                       , drPayrollCalc["Tpy_SRGAdjAmt"]
                                                       , drPayrollCalc["Tpy_SOTAdjHr"]                      //130 
                                                       , drPayrollCalc["Tpy_SOTAdjAmt"]             
                                                       , drPayrollCalc["Tpy_SHOLAdjHr"]                     
                                                       , drPayrollCalc["Tpy_SHOLAdjAmt"]                    
                                                       , drPayrollCalc["Tpy_SNDAdjHr"]
                                                       , drPayrollCalc["Tpy_SNDAdjAmt"]                     //135
                                                       , drPayrollCalc["Tpy_SLVAdjHr"]
                                                       , drPayrollCalc["Tpy_SLVAdjAmt"]                     
                                                       , drPayrollCalc["Tpy_MRGAdjHr"]                      
                                                       , drPayrollCalc["Tpy_MRGAdjAmt"]
                                                       , drPayrollCalc["Tpy_MOTAdjHr"]                      //140
                                                       , drPayrollCalc["Tpy_MOTAdjAmt"]
                                                       , drPayrollCalc["Tpy_MHOLAdjHr"]                     
                                                       , drPayrollCalc["Tpy_MHOLAdjAmt"]                    
                                                       , drPayrollCalc["Tpy_MNDAdjHr"]
                                                       , drPayrollCalc["Tpy_MNDAdjAmt"]                     //145                 
                                                       , drPayrollCalc["Tpy_TotalAdjAmt"]
                                                       , drPayrollCalc["Tpy_TaxableIncomeAmt"]              
                                                       , drPayrollCalc["Tpy_TotalTaxableIncomeAmt"]         
                                                       , drPayrollCalc["Tpy_NontaxableIncomeAmt"]
                                                       , drPayrollCalc["Tpy_GrossIncomeAmt"]                //150         
                                                       , drPayrollCalc["Tpy_WtaxAmt"]
                                                       , drPayrollCalc["Tpy_TaxBaseAmt"]                    
                                                       , drPayrollCalc["Tpy_TaxRule"]                        
                                                       , drPayrollCalc["Tpy_TaxShare"]
                                                       , drPayrollCalc["Tpy_TaxBracket"]                    //155               
                                                       , drPayrollCalc["Tpy_TaxCode"]               
                                                       , drPayrollCalc["Tpy_IsTaxExempted"]                 
                                                       , drPayrollCalc["Tpy_SSSEE"]                                      
                                                       , drPayrollCalc["Tpy_SSSER"]                 
                                                       , drPayrollCalc["Tpy_ECFundAmt"]                     //160
                                                       , drPayrollCalc["Tpy_SSSBaseAmt"]            
                                                       , drPayrollCalc["Tpy_SSSRule"]                       
                                                       , drPayrollCalc["Tpy_SSSShare"]
                                                       , drPayrollCalc["Tpy_MPFEE"]                         
                                                       , drPayrollCalc["Tpy_MPFER"]                         //165
                                                       , drPayrollCalc["Tpy_PhilhealthEE"]
                                                       , drPayrollCalc["Tpy_PhilhealthER"]                  
                                                       , drPayrollCalc["Tpy_PhilhealthBaseAmt"]
                                                       , drPayrollCalc["Tpy_PhilhealthRule"]                
                                                       , drPayrollCalc["Tpy_PhilhealthShare"]               //170            
                                                       , drPayrollCalc["Tpy_PagIbigEE"]
                                                       , drPayrollCalc["Tpy_PagIbigER"]                           
                                                       , drPayrollCalc["Tpy_PagIbigTaxEE"]
                                                       , drPayrollCalc["Tpy_PagIbigBaseAmt"]                
                                                       , drPayrollCalc["Tpy_PagIbigRule"]                   //175                     
                                                       , drPayrollCalc["Tpy_PagIbigShare"]
                                                       , drPayrollCalc["Tpy_UnionAmt"]                      
                                                       , drPayrollCalc["Tpy_UnionRule"]                     
                                                       , drPayrollCalc["Tpy_UnionShare"]                    
                                                       , drPayrollCalc["Tpy_UnionBaseAmt"]                  //180                 
                                                       , drPayrollCalc["Tpy_OtherDeductionAmt"]
                                                       , drPayrollCalc["Tpy_TotalDeductionAmt"]             
                                                       , drPayrollCalc["Tpy_NetAmt"]
                                                       , drPayrollCalc["Tpy_CostcenterCode"]                
                                                       , drPayrollCalc["Tpy_PositionCode"]                  //185                 
                                                       , drPayrollCalc["Tpy_EmploymentStatus"]
                                                       , drPayrollCalc["Tpy_PayrollType"]                   
                                                       , drPayrollCalc["Tpy_PaymentMode"]
                                                       , drPayrollCalc["Tpy_BankCode"]                      
                                                       , drPayrollCalc["Tpy_BankAcctNo"]                    //190                    
                                                       , drPayrollCalc["Tpy_PayrollGroup"]
                                                       , drPayrollCalc["Tpy_CalendarGrpCode"]                        
                                                       , drPayrollCalc["Tpy_WorkLocationCode"]
                                                       , drPayrollCalc["Tpy_AltAcctCode"]                   
                                                       , drPayrollCalc["Tpy_ExpenseClass"]                  //195                 
                                                       , drPayrollCalc["Tpy_PositionGrade"]
                                                       , drPayrollCalc["Tpy_WorkStatus"]                          
                                                       , drPayrollCalc["Tpy_PremiumGrpCode"]            
                                                       , drPayrollCalc["Tpy_IsComputedPerDay"]              
                                                       , drPayrollCalc["Tpy_IsMultipleSalary"]              //200
                                                       , drPayrollCalc["Tpy_RegRule"]
                                                       , drPayrollCalc["Usr_Login"]                         
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
                strUpdateRecordTemplate = @" INSERT INTO {0} (Tpd_IDNo,Tpd_PayCycle,Tpd_Date,Tpd_DayCode,Tpd_RestDayFlag,Tpd_WorkDay,Tpd_SalaryRate,Tpd_HourRate,Tpd_PayrollType,Tpd_SpecialRate,Tpd_SpecialHourRate,Tpd_PremiumGrpCode,Tpd_LTHr,Tpd_LTAmt,Tpd_UTHr,Tpd_UTAmt,Tpd_UPLVHr,Tpd_UPLVAmt,Tpd_ABSLEGHOLHr,Tpd_ABSLEGHOLAmt,Tpd_ABSSPLHOLHr,Tpd_ABSSPLHOLAmt,Tpd_ABSCOMPHOLHr,Tpd_ABSCOMPHOLAmt,Tpd_ABSPSDHr,Tpd_ABSPSDAmt,Tpd_ABSOTHHOLHr,Tpd_ABSOTHHOLAmt,Tpd_WDABSHr,Tpd_WDABSAmt,Tpd_LTUTMaxHr,Tpd_LTUTMaxAmt,Tpd_ABSHr,Tpd_ABSAmt,Tpd_REGHr,Tpd_REGAmt,Tpd_PDLVHr,Tpd_PDLVAmt,Tpd_PDLEGHOLHr,Tpd_PDLEGHOLAmt,Tpd_PDSPLHOLHr,Tpd_PDSPLHOLAmt,Tpd_PDCOMPHOLHr,Tpd_PDCOMPHOLAmt,Tpd_PDPSDHr,Tpd_PDPSDAmt,Tpd_PDOTHHOLHr,Tpd_PDOTHHOLAmt,Tpd_PDRESTLEGHOLHr,Tpd_PDRESTLEGHOLAmt,Tpd_TotalREGHr,Tpd_TotalREGAmt,Tpd_REGOTHr,Tpd_REGOTAmt,Tpd_REGNDHr,Tpd_REGNDAmt,Tpd_REGNDOTHr,Tpd_REGNDOTAmt,Tpd_RESTHr,Tpd_RESTAmt,Tpd_RESTOTHr,Tpd_RESTOTAmt,Tpd_RESTNDHr,Tpd_RESTNDAmt,Tpd_RESTNDOTHr,Tpd_RESTNDOTAmt,Tpd_LEGHOLHr,Tpd_LEGHOLAmt,Tpd_LEGHOLOTHr,Tpd_LEGHOLOTAmt,Tpd_LEGHOLNDHr,Tpd_LEGHOLNDAmt,Tpd_LEGHOLNDOTHr,Tpd_LEGHOLNDOTAmt,Tpd_SPLHOLHr,Tpd_SPLHOLAmt,Tpd_SPLHOLOTHr,Tpd_SPLHOLOTAmt,Tpd_SPLHOLNDHr,Tpd_SPLHOLNDAmt,Tpd_SPLHOLNDOTHr,Tpd_SPLHOLNDOTAmt,Tpd_PSDHr,Tpd_PSDAmt,Tpd_PSDOTHr,Tpd_PSDOTAmt,Tpd_PSDNDHr,Tpd_PSDNDAmt,Tpd_PSDNDOTHr,Tpd_PSDNDOTAmt,Tpd_COMPHOLHr,Tpd_COMPHOLAmt,Tpd_COMPHOLOTHr,Tpd_COMPHOLOTAmt,Tpd_COMPHOLNDHr,Tpd_COMPHOLNDAmt,Tpd_COMPHOLNDOTHr,Tpd_COMPHOLNDOTAmt,Tpd_RESTLEGHOLHr,Tpd_RESTLEGHOLAmt,Tpd_RESTLEGHOLOTHr,Tpd_RESTLEGHOLOTAmt,Tpd_RESTLEGHOLNDHr,Tpd_RESTLEGHOLNDAmt,Tpd_RESTLEGHOLNDOTHr,Tpd_RESTLEGHOLNDOTAmt,Tpd_RESTSPLHOLHr,Tpd_RESTSPLHOLAmt,Tpd_RESTSPLHOLOTHr,Tpd_RESTSPLHOLOTAmt,Tpd_RESTSPLHOLNDHr,Tpd_RESTSPLHOLNDAmt,Tpd_RESTSPLHOLNDOTHr,Tpd_RESTSPLHOLNDOTAmt,Tpd_RESTCOMPHOLHr,Tpd_RESTCOMPHOLAmt,Tpd_RESTCOMPHOLOTHr,Tpd_RESTCOMPHOLOTAmt,Tpd_RESTCOMPHOLNDHr,Tpd_RESTCOMPHOLNDAmt,Tpd_RESTCOMPHOLNDOTHr,Tpd_RESTCOMPHOLNDOTAmt,Tpd_RESTPSDHr,Tpd_RESTPSDAmt,Tpd_RESTPSDOTHr,Tpd_RESTPSDOTAmt,Tpd_RESTPSDNDHr,Tpd_RESTPSDNDAmt,Tpd_RESTPSDNDOTHr,Tpd_RESTPSDNDOTAmt,Tpd_TotalOTNDAmt,Usr_Login,Ludatetime) VALUES ('{1}','{2}','{3}','{4}','{5}',{6},{7},{8},'{9}',{10},{11},'{12}',{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34},{35},{36},{37},{38},{39},{40},{41},{42},{43},{44},{45},{46},{47},{48},{49},{50},{51},{52},{53},{54},{55},{56},{57},{58},{59},{60},{61},{62},{63},{64},{65},{66},{67},{68},{69},{70},{71},{72},{73},{74},{75},{76},{77},{78},{79},{80},{81},{82},{83},{84},{85},{86},{87},{88},{89},{90},{91},{92},{93},{94},{95},{96},{97},{98},{99},{100},{101},{102},{103},{104},{105},{106},{107},{108},{109},{110},{111},{112},{113},{114},{115},{116},{117},{118},{119},{120},{121},{122},{123},{124},{125},{126},{127},{128},{129},{130},{131},'{132}',GETDATE()) ";
                #endregion
                foreach (DataRow drPayrollCalcDtl in dtEmpPayrollDtl.Rows)
                {
                    //Save Payroll Dtl Table
                    #region Payroll Dtl Insert
                    strUpdateQuery += string.Format(strUpdateRecordTemplate
                                                        , EmpPayrollDtlTable
                                                        , drPayrollCalcDtl["Tpd_IDNo"]                          //1
                                                        , drPayrollCalcDtl["Tpd_PayCycle"]                      //2
                                                        , drPayrollCalcDtl["Tpd_Date"]                          //3
                                                        , drPayrollCalcDtl["Tpd_DayCode"]                       //4
                                                        , drPayrollCalcDtl["Tpd_RestDayFlag"]                   //5
                                                        , drPayrollCalcDtl["Tpd_WorkDay"]                   
                                                        , drPayrollCalcDtl["Tpd_SalaryRate"]
                                                        , drPayrollCalcDtl["Tpd_HourRate"]
                                                        , drPayrollCalcDtl["Tpd_PayrollType"]
                                                        , drPayrollCalcDtl["Tpd_SpecialRate"]                   //10
                                                        , drPayrollCalcDtl["Tpd_SpecialHourRate"]               
                                                        , drPayrollCalcDtl["Tpd_PremiumGrpCode"]
                                                        , drPayrollCalcDtl["Tpd_LTHr"]                          
                                                        , drPayrollCalcDtl["Tpd_LTAmt"]
                                                        , drPayrollCalcDtl["Tpd_UTHr"]                          //15
                                                        , drPayrollCalcDtl["Tpd_UTAmt"]                         
                                                        , drPayrollCalcDtl["Tpd_UPLVHr"]
                                                        , drPayrollCalcDtl["Tpd_UPLVAmt"]                       
                                                        , drPayrollCalcDtl["Tpd_ABSLEGHOLHr"]
                                                        , drPayrollCalcDtl["Tpd_ABSLEGHOLAmt"]                  //20
                                                        , drPayrollCalcDtl["Tpd_ABSSPLHOLHr"]                   
                                                        , drPayrollCalcDtl["Tpd_ABSSPLHOLAmt"]
                                                        , drPayrollCalcDtl["Tpd_ABSCOMPHOLHr"]                  
                                                        , drPayrollCalcDtl["Tpd_ABSCOMPHOLAmt"]
                                                        , drPayrollCalcDtl["Tpd_ABSPSDHr"]                      //25
                                                        , drPayrollCalcDtl["Tpd_ABSPSDAmt"]                     
                                                        , drPayrollCalcDtl["Tpd_ABSOTHHOLHr"]
                                                        , drPayrollCalcDtl["Tpd_ABSOTHHOLAmt"]                  
                                                        , drPayrollCalcDtl["Tpd_WDABSHr"]
                                                        , drPayrollCalcDtl["Tpd_WDABSAmt"]                      //30
                                                        , drPayrollCalcDtl["Tpd_LTUTMaxHr"]
                                                        , drPayrollCalcDtl["Tpd_LTUTMaxAmt"]                    
                                                        , drPayrollCalcDtl["Tpd_ABSHr"]                         
                                                        , drPayrollCalcDtl["Tpd_ABSAmt"]
                                                        , drPayrollCalcDtl["Tpd_REGHr"]                         //35                 
                                                        , drPayrollCalcDtl["Tpd_REGAmt"]
                                                        , drPayrollCalcDtl["Tpd_PDLVHr"]                        
                                                        , drPayrollCalcDtl["Tpd_PDLVAmt"]                       
                                                        , drPayrollCalcDtl["Tpd_PDLEGHOLHr"]
                                                        , drPayrollCalcDtl["Tpd_PDLEGHOLAmt"]                   //40                 
                                                        , drPayrollCalcDtl["Tpd_PDSPLHOLHr"]
                                                        , drPayrollCalcDtl["Tpd_PDSPLHOLAmt"]                   
                                                        , drPayrollCalcDtl["Tpd_PDCOMPHOLHr"]                   
                                                        , drPayrollCalcDtl["Tpd_PDCOMPHOLAmt"]
                                                        , drPayrollCalcDtl["Tpd_PDPSDHr"]                       //45
                                                        , drPayrollCalcDtl["Tpd_PDPSDAmt"]
                                                        , drPayrollCalcDtl["Tpd_PDOTHHOLHr"]                    
                                                        , drPayrollCalcDtl["Tpd_PDOTHHOLAmt"]                   
                                                        , drPayrollCalcDtl["Tpd_PDRESTLEGHOLHr"]
                                                        , drPayrollCalcDtl["Tpd_PDRESTLEGHOLAmt"]               //50
                                                        , drPayrollCalcDtl["Tpd_TotalREGHr"]
                                                        , drPayrollCalcDtl["Tpd_TotalREGAmt"]                   
                                                        , drPayrollCalcDtl["Tpd_REGOTHr"]                       
                                                        , drPayrollCalcDtl["Tpd_REGOTAmt"]
                                                        , drPayrollCalcDtl["Tpd_REGNDHr"]                       //55
                                                        , drPayrollCalcDtl["Tpd_REGNDAmt"]
                                                        , drPayrollCalcDtl["Tpd_REGNDOTHr"]                     
                                                        , drPayrollCalcDtl["Tpd_REGNDOTAmt"]                    
                                                        , drPayrollCalcDtl["Tpd_RESTHr"]
                                                        , drPayrollCalcDtl["Tpd_RESTAmt"]                       //60
                                                        , drPayrollCalcDtl["Tpd_RESTOTHr"]
                                                        , drPayrollCalcDtl["Tpd_RESTOTAmt"]                     
                                                        , drPayrollCalcDtl["Tpd_RESTNDHr"]                      
                                                        , drPayrollCalcDtl["Tpd_RESTNDAmt"]
                                                        , drPayrollCalcDtl["Tpd_RESTNDOTHr"]                    //65
                                                        , drPayrollCalcDtl["Tpd_RESTNDOTAmt"]
                                                        , drPayrollCalcDtl["Tpd_LEGHOLHr"]                      
                                                        , drPayrollCalcDtl["Tpd_LEGHOLAmt"]                     
                                                        , drPayrollCalcDtl["Tpd_LEGHOLOTHr"]
                                                        , drPayrollCalcDtl["Tpd_LEGHOLOTAmt"]                   //70
                                                        , drPayrollCalcDtl["Tpd_LEGHOLNDHr"]
                                                        , drPayrollCalcDtl["Tpd_LEGHOLNDAmt"]                   
                                                        , drPayrollCalcDtl["Tpd_LEGHOLNDOTHr"]                  
                                                        , drPayrollCalcDtl["Tpd_LEGHOLNDOTAmt"]
                                                        , drPayrollCalcDtl["Tpd_SPLHOLHr"]                      //75
                                                        , drPayrollCalcDtl["Tpd_SPLHOLAmt"]
                                                        , drPayrollCalcDtl["Tpd_SPLHOLOTHr"]                    
                                                        , drPayrollCalcDtl["Tpd_SPLHOLOTAmt"]                   
                                                        , drPayrollCalcDtl["Tpd_SPLHOLNDHr"]
                                                        , drPayrollCalcDtl["Tpd_SPLHOLNDAmt"]                   //80
                                                        , drPayrollCalcDtl["Tpd_SPLHOLNDOTHr"]
                                                        , drPayrollCalcDtl["Tpd_SPLHOLNDOTAmt"]                 
                                                        , drPayrollCalcDtl["Tpd_PSDHr"]                         
                                                        , drPayrollCalcDtl["Tpd_PSDAmt"]
                                                        , drPayrollCalcDtl["Tpd_PSDOTHr"]                       //85
                                                        , drPayrollCalcDtl["Tpd_PSDOTAmt"]
                                                        , drPayrollCalcDtl["Tpd_PSDNDHr"]                       
                                                        , drPayrollCalcDtl["Tpd_PSDNDAmt"]                      
                                                        , drPayrollCalcDtl["Tpd_PSDNDOTHr"]
                                                        , drPayrollCalcDtl["Tpd_PSDNDOTAmt"]                    //90
                                                        , drPayrollCalcDtl["Tpd_COMPHOLHr"]
                                                        , drPayrollCalcDtl["Tpd_COMPHOLAmt"]                    
                                                        , drPayrollCalcDtl["Tpd_COMPHOLOTHr"]                   
                                                        , drPayrollCalcDtl["Tpd_COMPHOLOTAmt"]
                                                        , drPayrollCalcDtl["Tpd_COMPHOLNDHr"]                   //95
                                                        , drPayrollCalcDtl["Tpd_COMPHOLNDAmt"]
                                                        , drPayrollCalcDtl["Tpd_COMPHOLNDOTHr"]                 
                                                        , drPayrollCalcDtl["Tpd_COMPHOLNDOTAmt"]                
                                                        , drPayrollCalcDtl["Tpd_RESTLEGHOLHr"]
                                                        , drPayrollCalcDtl["Tpd_RESTLEGHOLAmt"]                 //100
                                                        , drPayrollCalcDtl["Tpd_RESTLEGHOLOTHr"]
                                                        , drPayrollCalcDtl["Tpd_RESTLEGHOLOTAmt"]               
                                                        , drPayrollCalcDtl["Tpd_RESTLEGHOLNDHr"]                
                                                        , drPayrollCalcDtl["Tpd_RESTLEGHOLNDAmt"]
                                                        , drPayrollCalcDtl["Tpd_RESTLEGHOLNDOTHr"]              //105
                                                        , drPayrollCalcDtl["Tpd_RESTLEGHOLNDOTAmt"]
                                                        , drPayrollCalcDtl["Tpd_RESTSPLHOLHr"]                  
                                                        , drPayrollCalcDtl["Tpd_RESTSPLHOLAmt"]                 
                                                        , drPayrollCalcDtl["Tpd_RESTSPLHOLOTHr"]
                                                        , drPayrollCalcDtl["Tpd_RESTSPLHOLOTAmt"]               //110
                                                        , drPayrollCalcDtl["Tpd_RESTSPLHOLNDHr"]
                                                        , drPayrollCalcDtl["Tpd_RESTSPLHOLNDAmt"]               
                                                        , drPayrollCalcDtl["Tpd_RESTSPLHOLNDOTHr"]              
                                                        , drPayrollCalcDtl["Tpd_RESTSPLHOLNDOTAmt"]
                                                        , drPayrollCalcDtl["Tpd_RESTCOMPHOLHr"]                 //115
                                                        , drPayrollCalcDtl["Tpd_RESTCOMPHOLAmt"]
                                                        , drPayrollCalcDtl["Tpd_RESTCOMPHOLOTHr"]               
                                                        , drPayrollCalcDtl["Tpd_RESTCOMPHOLOTAmt"]              
                                                        , drPayrollCalcDtl["Tpd_RESTCOMPHOLNDHr"]
                                                        , drPayrollCalcDtl["Tpd_RESTCOMPHOLNDAmt"]              //120
                                                        , drPayrollCalcDtl["Tpd_RESTCOMPHOLNDOTHr"]
                                                        , drPayrollCalcDtl["Tpd_RESTCOMPHOLNDOTAmt"]            
                                                        , drPayrollCalcDtl["Tpd_RESTPSDHr"]                     
                                                        , drPayrollCalcDtl["Tpd_RESTPSDAmt"]
                                                        , drPayrollCalcDtl["Tpd_RESTPSDOTHr"]                   //125
                                                        , drPayrollCalcDtl["Tpd_RESTPSDOTAmt"]
                                                        , drPayrollCalcDtl["Tpd_RESTPSDNDHr"]                   
                                                        , drPayrollCalcDtl["Tpd_RESTPSDNDAmt"]                  
                                                        , drPayrollCalcDtl["Tpd_RESTPSDNDOTHr"]
                                                        , drPayrollCalcDtl["Tpd_RESTPSDNDOTAmt"]                //130
                                                        , drPayrollCalcDtl["Tpd_TotalOTNDAmt"]
                                                        , drPayrollCalcDtl["Usr_Login"]);                       //132
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
                //Save Payroll Ext2 Table
                strUpdateQuery = "";
                iUpdateCtr = 0;
                #region Payroll Ext2 Record Insert
                strUpdateRecordTemplate = @" INSERT INTO {0} (Tpy_IDNo,Tpy_PayCycle,Tpy_RemainingPayCycle,Tpy_YTDRegularAmtBefore,Tpy_AssumeRegularAmt,Tpy_RegularTotal,Tpy_YTDRecurringAllowanceAmtBefore,Tpy_YTDRecurringAllowanceAmt,Tpy_RecurringAllowanceAmt,Tpy_AssumeRecurringAllowanceAmt,Tpy_RecurringAllowanceTotal,Tpy_YTDBonusAmtBefore,Tpy_BonusNontaxAmt,Tpy_BonusTaxAmt,Tpy_Assume13thMonthAmt,Tpy_BonusTotal,Tpy_BonusTaxRevaluated,Tpy_YTDBonusTaxBefore,Tpy_YTDOtherTaxableIncomeAmtBefore,Tpy_OtherTaxableIncomeAmt,Tpy_OtherTaxableIncomeTotal,Tpy_YTDSSSAmtBefore,Tpy_AssumeSSSAmt,Tpy_SSSTotal,Tpy_YTDPhilhealthAmtBefore,Tpy_AssumePhilhealthAmt,Tpy_PhilhealthTotal,Tpy_YTDPagIbigAmtBefore,Tpy_AssumePagIbigAmt,Tpy_PagIbigTotal,Tpy_YTDPagIbigTaxAmtBefore,Tpy_AssumePagIbigTaxAmt,Tpy_TotalTaxableIncomeWAssumeAmt,Tpy_YTDUnionAmtBefore,Tpy_AssumeUnionAmt,Tpy_UnionTotal,Tpy_PremiumPaidOnHealth,Tpy_TotalExemptions,Tpy_NetTaxableIncomeAmt,Tpy_YTDREGAmt,Tpy_YTDTotalTaxableIncomeAmtBefore,Tpy_YTDTotalTaxableIncomeAmt,Tpy_YTDWtaxAmtBefore,Tpy_YTDWtaxAmt,Tpy_YTDSSSAmt,Tpy_YTDPhilhealthAmt,Tpy_YTDPagIbigAmt,Tpy_YTDPagIbigTaxAmt,Tpy_YTDUnionAmt,Tpy_YTDNontaxableAmtBefore,Tpy_YTDNontaxableAmt,Tpy_BIRTotalAmountofCompensation,Tpy_BIRStatutoryMinimumWage,Tpy_BIRHolidayOvertimeNightShiftHazard,Tpy_BIR13thMonthPayOtherBenefits,Tpy_BIRDeMinimisBenefits,Tpy_BIRSSSGSISPHICHDMFUnionDues,Tpy_BIROtherNonTaxableCompensation,Tpy_BIRTotalTaxableCompensation,Tpy_BIRTaxableCompensationNotSubjectToWithholdingTax,Tpy_BIRTotalTaxesWithheld,Tpy_TaxDue,Tpy_AssumeMonthlyRegularAmt,Tpy_AssumeMonthlySSSAmt,Tpy_AssumeMonthlyPhilhealthAmt,Tpy_AssumeMonthlyPagibigAmt,Tpy_AssumeMonthlyUnionAmt,Tpy_YTDMPFAmtBefore,Tpy_YTDMPFAmt,Usr_Login,Ludatetime) VALUES ('{1}','{2}','{3}',{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34},{35},{36},{37},{38},{39},{40},{41},{42},{43},{44},{45},{46},{47},{48},{49},{50},{51},{52},{53},{54},{55},{56},{57},{58},{59},{60},{61},{62},{63},{64},{65},{66},{67},{68},{69},'{70}',GETDATE()) ";
                #endregion
                foreach (DataRow drPayrollCalcExt2 in dtEmpPayroll2.Rows)
                {
                    #region Payroll Ext2 Insert
                    strUpdateQuery += string.Format(strUpdateRecordTemplate
                                                    , EmpPayroll2Table
                                                    , drPayrollCalcExt2["Tpy_IDNo"]                                                     //1
                                                    , drPayrollCalcExt2["Tpy_PayCycle"]                                                 
                                                    , drPayrollCalcExt2["Tpy_RemainingPayCycle"]                                        
                                                    , drPayrollCalcExt2["Tpy_YTDRegularAmtBefore"]                                      
                                                    , drPayrollCalcExt2["Tpy_AssumeRegularAmt"]                                         //5
                                                    , drPayrollCalcExt2["Tpy_RegularTotal"]                                             
                                                    , drPayrollCalcExt2["Tpy_YTDRecurringAllowanceAmtBefore"]   
                                                    , drPayrollCalcExt2["Tpy_YTDRecurringAllowanceAmt"]         
                                                    , drPayrollCalcExt2["Tpy_RecurringAllowanceAmt"]            
                                                    , drPayrollCalcExt2["Tpy_AssumeRecurringAllowanceAmt"]                              //10
                                                    , drPayrollCalcExt2["Tpy_RecurringAllowanceTotal"]          
                                                    , drPayrollCalcExt2["Tpy_YTDBonusAmtBefore"]                
                                                    , drPayrollCalcExt2["Tpy_BonusNontaxAmt"]                   
                                                    , drPayrollCalcExt2["Tpy_BonusTaxAmt"]                     
                                                    , drPayrollCalcExt2["Tpy_Assume13thMonthAmt"]                                       //15
                                                    , drPayrollCalcExt2["Tpy_BonusTotal"]                       
                                                    , drPayrollCalcExt2["Tpy_BonusTaxRevaluated"]               
                                                    , drPayrollCalcExt2["Tpy_YTDBonusTaxBefore"]                                        //67
                                                    , drPayrollCalcExt2["Tpy_YTDOtherTaxableIncomeAmtBefore"]   
                                                    , drPayrollCalcExt2["Tpy_OtherTaxableIncomeAmt"]                                    //20
                                                    , drPayrollCalcExt2["Tpy_OtherTaxableIncomeTotal"]                                  
                                                    , drPayrollCalcExt2["Tpy_YTDSSSAmtBefore"]                  
                                                    , drPayrollCalcExt2["Tpy_AssumeSSSAmt"]                     
                                                    , drPayrollCalcExt2["Tpy_SSSTotal"]                         
                                                    , drPayrollCalcExt2["Tpy_YTDPhilhealthAmtBefore"]                                   //25 
                                                    , drPayrollCalcExt2["Tpy_AssumePhilhealthAmt"]              
                                                    , drPayrollCalcExt2["Tpy_PhilhealthTotal"]                  
                                                    , drPayrollCalcExt2["Tpy_YTDPagIbigAmtBefore"]              
                                                    , drPayrollCalcExt2["Tpy_AssumePagIbigAmt"]                 
                                                    , drPayrollCalcExt2["Tpy_PagIbigTotal"]                                             //30            
                                                    , drPayrollCalcExt2["Tpy_YTDPagIbigTaxAmtBefore"]                                   
                                                    , drPayrollCalcExt2["Tpy_AssumePagIbigTaxAmt"]                                      
                                                    , drPayrollCalcExt2["Tpy_TotalTaxableIncomeWAssumeAmt"]                             
                                                    , drPayrollCalcExt2["Tpy_YTDUnionAmtBefore"]                                        
                                                    , drPayrollCalcExt2["Tpy_AssumeUnionAmt"]                                           //35
                                                    , drPayrollCalcExt2["Tpy_UnionTotal"]                                               
                                                    , drPayrollCalcExt2["Tpy_PremiumPaidOnHealth"]                                      
                                                    , drPayrollCalcExt2["Tpy_TotalExemptions"]                                          
                                                    , drPayrollCalcExt2["Tpy_NetTaxableIncomeAmt"]                                      
                                                    , drPayrollCalcExt2["Tpy_YTDREGAmt"]                                                //40
                                                    , drPayrollCalcExt2["Tpy_YTDTotalTaxableIncomeAmtBefore"]                           
                                                    , drPayrollCalcExt2["Tpy_YTDTotalTaxableIncomeAmt"]                                 
                                                    , drPayrollCalcExt2["Tpy_YTDWtaxAmtBefore"]                                         
                                                    , drPayrollCalcExt2["Tpy_YTDWtaxAmt"]                                               
                                                    , drPayrollCalcExt2["Tpy_YTDSSSAmt"]                                                //45
                                                    , drPayrollCalcExt2["Tpy_YTDPhilhealthAmt"]                                         
                                                    , drPayrollCalcExt2["Tpy_YTDPagIbigAmt"]                                            
                                                    , drPayrollCalcExt2["Tpy_YTDPagIbigTaxAmt"]                                         
                                                    , drPayrollCalcExt2["Tpy_YTDUnionAmt"]                                              
                                                    , drPayrollCalcExt2["Tpy_YTDNontaxableAmtBefore"]                                   //50
                                                    , drPayrollCalcExt2["Tpy_YTDNontaxableAmt"]                                         
                                                    , drPayrollCalcExt2["Tpy_BIRTotalAmountofCompensation"]                             
                                                    , drPayrollCalcExt2["Tpy_BIRStatutoryMinimumWage"]                                  
                                                    , drPayrollCalcExt2["Tpy_BIRHolidayOvertimeNightShiftHazard"]                       
                                                    , drPayrollCalcExt2["Tpy_BIR13thMonthPayOtherBenefits"]                             //55
                                                    , drPayrollCalcExt2["Tpy_BIRDeMinimisBenefits"]                                    
                                                    , drPayrollCalcExt2["Tpy_BIRSSSGSISPHICHDMFUnionDues"]                              
                                                    , drPayrollCalcExt2["Tpy_BIROtherNonTaxableCompensation"]                          
                                                    , drPayrollCalcExt2["Tpy_BIRTotalTaxableCompensation"]                              
                                                    , drPayrollCalcExt2["Tpy_BIRTaxableCompensationNotSubjectToWithholdingTax"]         //60
                                                    , drPayrollCalcExt2["Tpy_BIRTotalTaxesWithheld"]                                    
                                                    , drPayrollCalcExt2["Tpy_TaxDue"]                                                   
                                                    , drPayrollCalcExt2["Tpy_AssumeMonthlyRegularAmt"]                                 
                                                    , drPayrollCalcExt2["Tpy_AssumeMonthlySSSAmt"]                                     
                                                    , drPayrollCalcExt2["Tpy_AssumeMonthlyPhilhealthAmt"]                               //65
                                                    , drPayrollCalcExt2["Tpy_AssumeMonthlyPagibigAmt"]                                  
                                                    , drPayrollCalcExt2["Tpy_AssumeMonthlyUnionAmt"]                                    
                                                    , drPayrollCalcExt2["Tpy_YTDMPFAmtBefore"]                                          //68
                                                    , drPayrollCalcExt2["Tpy_YTDMPFAmt"]                                                //69
                                                    , drPayrollCalcExt2["Usr_Login"]                                                    //70

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
                if (bHasDayCodeExt)
                {
                    //Save Payroll Ext Table
                    strUpdateQuery = "";
                    iUpdateCtr = 0;
                    #region Payroll Ext Record Insert
                    strUpdateRecordTemplate = @" INSERT INTO {0} (Tpm_IDNo,Tpm_PayCycle,Tpm_Misc1Hr,Tpm_Misc1Amt,Tpm_Misc1OTHr,Tpm_Misc1OTAmt,Tpm_Misc1NDHr,Tpm_Misc1NDAmt,Tpm_Misc1NDOTHr,Tpm_Misc1NDOTAmt,Tpm_Misc2Hr,Tpm_Misc2Amt,Tpm_Misc2OTHr,Tpm_Misc2OTAmt,Tpm_Misc2NDHr,Tpm_Misc2NDAmt,Tpm_Misc2NDOTHr,Tpm_Misc2NDOTAmt,Tpm_Misc3Hr,Tpm_Misc3Amt,Tpm_Misc3OTHr,Tpm_Misc3OTAmt,Tpm_Misc3NDHr,Tpm_Misc3NDAmt,Tpm_Misc3NDOTHr,Tpm_Misc3NDOTAmt,Tpm_Misc4Hr,Tpm_Misc4Amt,Tpm_Misc4OTHr,Tpm_Misc4OTAmt,Tpm_Misc4NDHr,Tpm_Misc4NDAmt,Tpm_Misc4NDOTHr,Tpm_Misc4NDOTAmt,Tpm_Misc5Hr,Tpm_Misc5Amt,Tpm_Misc5OTHr,Tpm_Misc5OTAmt,Tpm_Misc5NDHr,Tpm_Misc5NDAmt,Tpm_Misc5NDOTHr,Tpm_Misc5NDOTAmt,Tpm_Misc6Hr,Tpm_Misc6Amt,Tpm_Misc6OTHr,Tpm_Misc6OTAmt,Tpm_Misc6NDHr,Tpm_Misc6NDAmt,Tpm_Misc6NDOTHr,Tpm_Misc6NDOTAmt,Usr_Login,Ludatetime) VALUES ('{1}','{2}',{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34},{35},{36},{37},{38},{39},{40},{41},{42},{43},{44},{45},{46},{47},{48},{49},{50},'{51}', GetDate()) ";
                    #endregion
                    foreach (DataRow drPayrollCalcExt in dtEmpPayrollMisc.Rows)
                    {
                        #region Payroll Ext Insert
                        strUpdateQuery += string.Format(strUpdateRecordTemplate
                                                           , EmpPayrollMiscTable
                                                           , drPayrollCalcExt["Tpm_IDNo"]
                                                           , drPayrollCalcExt["Tpm_PayCycle"]
                                                           , drPayrollCalcExt["Tpm_Misc1Hr"]
                                                           , drPayrollCalcExt["Tpm_Misc1Amt"]
                                                           , drPayrollCalcExt["Tpm_Misc1OTHr"]
                                                           , drPayrollCalcExt["Tpm_Misc1OTAmt"]
                                                           , drPayrollCalcExt["Tpm_Misc1NDHr"]
                                                           , drPayrollCalcExt["Tpm_Misc1NDAmt"]
                                                           , drPayrollCalcExt["Tpm_Misc1NDOTHr"]
                                                           , drPayrollCalcExt["Tpm_Misc1NDOTAmt"]
                                                           , drPayrollCalcExt["Tpm_Misc2Hr"]
                                                           , drPayrollCalcExt["Tpm_Misc2Amt"]
                                                           , drPayrollCalcExt["Tpm_Misc2OTHr"]
                                                           , drPayrollCalcExt["Tpm_Misc2OTAmt"]
                                                           , drPayrollCalcExt["Tpm_Misc2NDHr"]
                                                           , drPayrollCalcExt["Tpm_Misc2NDAmt"]
                                                           , drPayrollCalcExt["Tpm_Misc2NDOTHr"]
                                                           , drPayrollCalcExt["Tpm_Misc2NDOTAmt"]
                                                           , drPayrollCalcExt["Tpm_Misc3Hr"]
                                                           , drPayrollCalcExt["Tpm_Misc3Amt"]
                                                           , drPayrollCalcExt["Tpm_Misc3OTHr"]
                                                           , drPayrollCalcExt["Tpm_Misc3OTAmt"]
                                                           , drPayrollCalcExt["Tpm_Misc3NDHr"]
                                                           , drPayrollCalcExt["Tpm_Misc3NDAmt"]
                                                           , drPayrollCalcExt["Tpm_Misc3NDOTHr"]
                                                           , drPayrollCalcExt["Tpm_Misc3NDOTAmt"]
                                                           , drPayrollCalcExt["Tpm_Misc4Hr"]
                                                           , drPayrollCalcExt["Tpm_Misc4Amt"]
                                                           , drPayrollCalcExt["Tpm_Misc4OTHr"]
                                                           , drPayrollCalcExt["Tpm_Misc4OTAmt"]
                                                           , drPayrollCalcExt["Tpm_Misc4NDHr"]
                                                           , drPayrollCalcExt["Tpm_Misc4NDAmt"]
                                                           , drPayrollCalcExt["Tpm_Misc4NDOTHr"]
                                                           , drPayrollCalcExt["Tpm_Misc4NDOTAmt"]
                                                           , drPayrollCalcExt["Tpm_Misc5Hr"]
                                                           , drPayrollCalcExt["Tpm_Misc5Amt"]
                                                           , drPayrollCalcExt["Tpm_Misc5OTHr"]
                                                           , drPayrollCalcExt["Tpm_Misc5OTAmt"]
                                                           , drPayrollCalcExt["Tpm_Misc5NDHr"]
                                                           , drPayrollCalcExt["Tpm_Misc5NDAmt"]
                                                           , drPayrollCalcExt["Tpm_Misc5NDOTHr"]
                                                           , drPayrollCalcExt["Tpm_Misc5NDOTAmt"]
                                                           , drPayrollCalcExt["Tpm_Misc6Hr"]
                                                           , drPayrollCalcExt["Tpm_Misc6Amt"]
                                                           , drPayrollCalcExt["Tpm_Misc6OTHr"]
                                                           , drPayrollCalcExt["Tpm_Misc6OTAmt"]
                                                           , drPayrollCalcExt["Tpm_Misc6NDHr"]
                                                           , drPayrollCalcExt["Tpm_Misc6NDAmt"]
                                                           , drPayrollCalcExt["Tpm_Misc6NDOTHr"]
                                                           , drPayrollCalcExt["Tpm_Misc6NDOTAmt"]
                                                           , drPayrollCalcExt["Usr_Login"]);
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

                    //Save Payroll Dtl Ext Table
                    strUpdateQuery = "";
                    iUpdateCtr = 0;
                    #region Payroll Dtl Ext Record Insert
                    strUpdateRecordTemplate = @" INSERT INTO {0} (Tpm_IDNo,Tpm_PayCycle,Tpm_Date,Tpm_Misc1Hr,Tpm_Misc1Amt,Tpm_Misc1OTHr,Tpm_Misc1OTAmt,Tpm_Misc1NDHr,Tpm_Misc1NDAmt,Tpm_Misc1NDOTHr,Tpm_Misc1NDOTAmt,Tpm_Misc2Hr,Tpm_Misc2Amt,Tpm_Misc2OTHr,Tpm_Misc2OTAmt,Tpm_Misc2NDHr,Tpm_Misc2NDAmt,Tpm_Misc2NDOTHr,Tpm_Misc2NDOTAmt,Tpm_Misc3Hr,Tpm_Misc3Amt,Tpm_Misc3OTHr,Tpm_Misc3OTAmt,Tpm_Misc3NDHr,Tpm_Misc3NDAmt,Tpm_Misc3NDOTHr,Tpm_Misc3NDOTAmt,Tpm_Misc4Hr,Tpm_Misc4Amt,Tpm_Misc4OTHr,Tpm_Misc4OTAmt,Tpm_Misc4NDHr,Tpm_Misc4NDAmt,Tpm_Misc4NDOTHr,Tpm_Misc4NDOTAmt,Tpm_Misc5Hr,Tpm_Misc5Amt,Tpm_Misc5OTHr,Tpm_Misc5OTAmt,Tpm_Misc5NDHr,Tpm_Misc5NDAmt,Tpm_Misc5NDOTHr,Tpm_Misc5NDOTAmt,Tpm_Misc6Hr,Tpm_Misc6Amt,Tpm_Misc6OTHr,Tpm_Misc6OTAmt,Tpm_Misc6NDHr,Tpm_Misc6NDAmt,Tpm_Misc6NDOTHr,Tpm_Misc6NDOTAmt,Usr_Login,Ludatetime) VALUES ('{1}','{2}','{3}',{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34},{35},{36},{37},{38},{39},{40},{41},{42},{43},{44},{45},{46},{47},{48},{49},{50},{51},'{52}',GETDATE()) ";
                    #endregion
                    foreach (DataRow drPayrollCalcDtlExt in dtEmpPayrollDtlMisc.Rows)
                    {
                        #region Payroll Ext Insert
                        strUpdateQuery += string.Format(strUpdateRecordTemplate
                                                           , EmpPayrollDtlMiscTable
                                                           , drPayrollCalcDtlExt["Tpm_IDNo"]                                //1
                                                           , drPayrollCalcDtlExt["Tpm_PayCycle"]
                                                           , drPayrollCalcDtlExt["Tpm_Date"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc1Hr"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc1Amt"]                            //5
                                                           , drPayrollCalcDtlExt["Tpm_Misc1OTHr"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc1OTAmt"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc1NDHr"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc1NDAmt"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc1NDOTHr"]                         //10
                                                           , drPayrollCalcDtlExt["Tpm_Misc1NDOTAmt"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc2Hr"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc2Amt"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc2OTHr"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc2OTAmt"]                          //15
                                                           , drPayrollCalcDtlExt["Tpm_Misc2NDHr"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc2NDAmt"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc2NDOTHr"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc2NDOTAmt"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc3Hr"]                             //20
                                                           , drPayrollCalcDtlExt["Tpm_Misc3Amt"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc3OTHr"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc3OTAmt"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc3NDHr"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc3NDAmt"]                          //25
                                                           , drPayrollCalcDtlExt["Tpm_Misc3NDOTHr"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc3NDOTAmt"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc4Hr"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc4Amt"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc4OTHr"]                           //30
                                                           , drPayrollCalcDtlExt["Tpm_Misc4OTAmt"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc4NDHr"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc4NDAmt"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc4NDOTHr"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc4NDOTAmt"]                        //35
                                                           , drPayrollCalcDtlExt["Tpm_Misc5Hr"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc5Amt"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc5OTHr"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc5OTAmt"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc5NDHr"]                           //40
                                                           , drPayrollCalcDtlExt["Tpm_Misc5NDAmt"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc5NDOTHr"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc5NDOTAmt"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc6Hr"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc6Amt"]                            //45
                                                           , drPayrollCalcDtlExt["Tpm_Misc6OTHr"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc6OTAmt"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc6NDHr"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc6NDAmt"]
                                                           , drPayrollCalcDtlExt["Tpm_Misc6NDOTHr"]                         //50
                                                           , drPayrollCalcDtlExt["Tpm_Misc6NDOTAmt"]
                                                           , drPayrollCalcDtlExt["Usr_Login"]);                             //52
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
                }
                StatusHandler(this, new StatusEventArgs("Saving Payroll Records", true));
                //-----------------------------
                StatusHandler(this, new StatusEventArgs("Creating Payslip Records", false));
                InsertToDeductionLedgerPayslip(ProcessAll, ProcessSeparated, EmployeeId, PayrollPeriod);
                InsertToEmployeeLeavePayslip(ProcessAll, EmployeeId);
                UpdatePayrollPostFlag(ProcessAll, EmployeeId);
                StatusHandler(this, new StatusEventArgs("Creating Payslip Records", true));
                //-----------------------------
                #region Generate Payroll Error List
                dtErrList = SavePayrollErrorReportList();
                if (dtErrList.Rows.Count > 0)
                    InsertToEmpPayrollCheckTable(dtErrList);
                #endregion
                //-----------------------------
            }
            catch (Exception ex)
            {
                //dal.RollBackTransactionSnapshot();
                CommonProcedures.logErrorToFile(ex.ToString());
                throw new Exception("Payroll Calculation has encountered some errors: \n" + ex.Message);
            }
            finally
            {
                //dal.CloseDB();
            }

            return dtErrList;
        }

        public void DeletePayrollRecords(string PayPeriod, string EmployeeId, DALHelper dalHelper)
        {
            #region Query
            string query = string.Format(@"DELETE FROM T_EmpPayroll
                                            WHERE Tpy_PayCycle = '{0}'
                                                AND Tpy_IDNo = '{1}'

                                            DELETE FROM T_EmpPayroll2
                                            WHERE Tpy_PayCycle = '{0}'
                                                AND Tpy_IDNo = '{1}'

                                            DELETE FROM T_EmpPayrollMisc
                                            WHERE Tpm_PayCycle = '{0}'
                                                AND Tpm_IDNo = '{1}'

                                            DELETE FROM T_EmpDeductionHdrCycle
                                            WHERE Tdh_PayCycle = '{0}'
                                                AND Tdh_IDNo = '{1}'

                                            DELETE FROM T_EmpPayrollDtl
                                            WHERE Tpd_PayCycle = '{0}'
                                                AND Tpd_IDNo = '{1}'

                                            DELETE FROM T_EmpPayrollDtlMisc
                                            WHERE Tpm_PayCycle = '{0}'
                                                AND Tpm_IDNo = '{1}'

                                            ---DELETE FROM T_EmpPayTranHdr
                                            ---WHERE Tph_PayCycle = '{0}'
                                            ---    AND Tph_IDNo = '{1}'

                                            ---DELETE FROM T_EmpPayTranDtl
                                            ---WHERE Tpd_PayCycle = '{0}'
                                            ---    AND Tpd_IDNo = '{1}'

                                            ---DELETE FROM T_EmpPayTranHdrMisc
                                            ---WHERE Tph_PayCycle = '{0}'
                                            ---    AND Tph_IDNo = '{1}'

                                            ---DELETE FROM T_EmpPayTranDtlMisc
                                            ---WHERE Tpd_PayCycle = '{0}'
                                            ---    AND Tpd_IDNo = '{1}'

                                            DELETE FROM T_EmpPayrollTrack
                                            WHERE Tpt_IDNo = '{1}'
                                                AND Tpt_Type = 'N'
                
                                            UPDATE T_EmpDeductionDtl
                                            SET Tdd_PaymentFlag = 0
                                            WHERE Tdd_ThisPayCycle = '{0}'
                                                AND Tdd_IDNo = '{1}'

                                            UPDATE T_EmpSystemAdj
                                            SET Tsa_PostFlag = 0
                                            WHERE Tsa_AdjPayCycle = '{0}'
                                                AND Tsa_IDNo = '{1}'

                                            ---DELETE FROM T_EmpSystemAdj
                                            ---WHERE Tsa_AdjPayCycle = '{0}'
                                            ---    AND Tsa_IDNo = '{1}'

                                            ---DELETE FROM T_EmpSystemAdj2
                                            ---WHERE Tsa_AdjPayCycle = '{0}'
                                            ---    AND Tsa_IDNo = '{1}'

                                            ---DELETE FROM T_EmpSystemAdjMisc
                                            ---WHERE Tsm_AdjPayCycle = '{0}'
                                            ---    AND Tsm_IDNo = '{1}'

                                            ---DELETE FROM T_EmpSystemAdjMisc2
                                            ---WHERE Tsm_AdjPayCycle = '{0}'
                                            ---    AND Tsm_IDNo = '{1}'

                                            UPDATE T_EmpIncome
                                            SET Tin_PostFlag = 0
                                            WHERE Tin_PayCycle = '{0}'
                                                AND Tin_IDNo = '{1}'

                                            UPDATE T_EmpManualAdj
                                            SET Tma_PostFlag = 0
                                            WHERE Tma_PayCycle = '{0}'
                                                AND Tma_IDNo = '{1}'
                                            ", PayPeriod, EmployeeId);
            #endregion
            dalHelper.ExecuteNonQuery(query);
        }

		public void DeleteSpecialPayrollRecords(string PayPeriod, string EmployeeId, string CycleTypeCode, DALHelper dalHelper)
        {
            #region Query
            string query = string.Format(@"DELETE FROM T_EmpPayroll
                                            WHERE Tpy_PayCycle = '{0}'
                                                AND Tpy_IDNo = '{1}'

                                            DELETE FROM T_EmpPayroll2
                                            WHERE Tpy_PayCycle = '{0}'
                                                AND Tpy_IDNo = '{1}'

                                            DELETE FROM T_EmpPayrollMisc
                                            WHERE Tpm_PayCycle = '{0}'
                                                AND Tpm_IDNo = '{1}'

                                            DELETE FROM T_EmpPayrollDtl
                                            WHERE Tpd_PayCycle = '{0}'
                                                AND Tpd_IDNo = '{1}'

                                            DELETE FROM T_EmpPayrollDtlMisc
                                            WHERE Tpm_PayCycle = '{0}'
                                                AND Tpm_IDNo = '{1}'

                                            DELETE FROM T_EmpIncome
                                            WHERE Tin_PayCycle = '{0}'
                                                AND Tin_IDNo = '{1}'

                                            --DELETE FROM T_EmpSpecialPayroll
                                            --WHERE Tpy_PayCycle = '{0}'
                                            --    AND Tpy_IDNo = '{1}'

                                            --DELETE FROM T_EmpDeductionHdrCycle
                                            --WHERE Tdh_PayCycle = '{0}'
                                            --    AND Tdh_IDNo = '{1}'

                                            --DELETE FROM T_EmpDeductionDtl
                                            --WHERE Tdd_ThisPayCycle = '{0}'
                                            --    AND Tdd_IDNo = '{1}'

                                            DELETE FROM T_EmpPayrollTrack
                                            WHERE Tpt_IDNo = '{1}'
                                                AND Tpt_Type = '{2}'

                                            ", PayPeriod, EmployeeId, CycleTypeCode);
            #endregion
            dalHelper.ExecuteNonQuery(query);
        }
        public void DeletePayrollRecords(string PayPeriod, string EmployeeId, string DatabaseProfile, DALHelper dalHelper)
        {
            #region Query
            string query = string.Format(@"DELETE FROM {2}..T_EmpPayroll
                                            WHERE Tpy_PayCycle = '{0}'
                                                AND Tpy_IDNo = '{1}'

                                            DELETE FROM {2}..T_EmpPayroll2
                                            WHERE Tpy_PayCycle = '{0}'
                                                AND Tpy_IDNo = '{1}'

                                            DELETE FROM {2}..T_EmpPayrollMisc
                                            WHERE Tpm_PayCycle = '{0}'
                                                AND Tpm_IDNo = '{1}'

                                            DELETE FROM {2}..T_EmpDeductionHdrCycle
                                            WHERE Tdh_PayCycle = '{0}'
                                                AND Tdh_IDNo = '{1}'

                                            DELETE FROM {2}..T_EmpPayrollDtl
                                            WHERE Tpd_PayCycle = '{0}'
                                                AND Tpd_IDNo = '{1}'

                                            DELETE FROM {2}..T_EmpPayrollDtlMisc
                                            WHERE Tpm_PayCycle = '{0}'
                                                AND Tpm_IDNo = '{1}'

                                            ---DELETE FROM {2}..T_EmpPayTranHdr
                                            ---WHERE Tph_PayCycle = '{0}'
                                            ---    AND Tph_IDNo = '{1}'

                                            ---DELETE FROM {2}..T_EmpPayTranDtl
                                            ---WHERE Tpd_PayCycle = '{0}'
                                            ---    AND Tpd_IDNo = '{1}'

                                            ---DELETE FROM {2}..T_EmpPayTranHdrMisc
                                            ---WHERE Tph_PayCycle = '{0}'
                                            ---    AND Tph_IDNo = '{1}'

                                            ---DELETE FROM {2}..T_EmpPayTranDtlMisc
                                            ---WHERE Tpd_PayCycle = '{0}'
                                            ---    AND Tpd_IDNo = '{1}'

                                            DELETE FROM {2}..T_EmpSystemAdj
                                            WHERE Tsa_AdjPayCycle = '{0}'
                                                AND Tsa_IDNo = '{1}'

                                            ---DELETE FROM {2}..T_EmpSystemAdj2
                                            ---WHERE Tsa_AdjPayCycle = '{0}'
                                            ---    AND Tsa_IDNo = '{1}'

                                            ---DELETE FROM {2}..T_EmpSystemAdjMisc
                                            ---WHERE Tsm_AdjPayCycle = '{0}'
                                            ---    AND Tsm_IDNo = '{1}'

                                            ---DELETE FROM {2}..T_EmpSystemAdjMisc2
                                            ---WHERE Tsm_AdjPayCycle = '{0}'
                                            ---    AND Tsm_IDNo = '{1}'

                                            DELETE FROM {2}..T_EmpPayrollTrack
                                            WHERE Tpt_IDNo = '{1}'
                                            AND Tpt_Type = 'N'

                                            UPDATE {2}..T_EmpDeductionDtl
                                            SET Tdd_PaymentFlag = 0
                                            WHERE Tdd_ThisPayCycle = '{0}'
                                                AND Tdd_IDNo = '{1}'

                                            UPDATE {2}..T_EmpIncome
                                            SET Tin_PostFlag = 0
                                            WHERE Tin_PayCycle = '{0}'
                                                AND Tin_IDNo = '{1}'

                                            UPDATE {2}..T_EmpManualAdj
                                            SET Tma_PostFlag = 0
                                            WHERE Tma_PayCycle = '{0}'
                                                AND Tma_IDNo = '{1}'
                                            ", PayPeriod, EmployeeId, DatabaseProfile);
            #endregion
            dalHelper.ExecuteNonQuery(query);
        }

        public void DeletePayrollRecordsBatch(string PayPeriod, string EmployeeIds, DALHelper dalHelper)
        {
            #region Query
            string query = string.Format(@"DELETE FROM T_EmpPayroll
                                            WHERE Tpy_PayCycle = '{0}'
                                                AND Tpy_IDNo IN ({1})
                                            
                                            DELETE FROM T_EmpPayroll2
                                            WHERE Tpy_PayCycle = '{0}'
                                                AND Tpy_IDNo IN ({1})

                                            DELETE FROM T_EmpPayrollMisc
                                            WHERE Tpm_PayCycle = '{0}'
                                                AND Tpm_IDNo IN ({1})

                                            DELETE FROM T_EmpDeductionHdrCycle
                                            WHERE Tdh_PayCycle = '{0}'
                                                AND Tdh_IDNo IN ({1})

                                            DELETE FROM {2}..T_EmpPayrollDtl
                                            WHERE Tpd_PayCycle = '{0}'
                                                AND Tpd_IDNo IN ({1})

                                            DELETE FROM {2}..T_EmpPayrollDtlMisc
                                            WHERE Tpm_PayCycle = '{0}'
                                                AND Tpm_IDNo IN ({1})

                                            ---DELETE FROM T_EmpPayTranHdr
                                            ---WHERE Tph_PayCycle = '{0}'
                                            ---    AND Tph_IDNo IN ({1})

                                            ---DELETE FROM T_EmpPayTranDtl
                                            ---WHERE Tpd_PayCycle = '{0}'
                                            ---    AND Tpd_IDNo IN ({1})

                                            ---DELETE FROM T_EmpPayTranHdrMisc
                                            ---WHERE Tph_PayCycle = '{0}'
                                            ---    AND Tph_IDNo IN ({1})

                                            ---DELETE FROM T_EmpPayTranDtlMisc
                                            ---WHERE Tpd_PayCycle = '{0}'
                                            ---    AND Tpd_IDNo IN ({1})

                                            DELETE FROM T_EmpPayrollTrack
                                            WHERE Tpt_IDNo IN ({1})
                                                AND Tpt_Type = 'N'

                                            UPDATE T_EmpDeductionDtl
                                            SET Tdd_PaymentFlag = 0
                                            WHERE Tdd_ThisPayCycle = '{0}'
                                                AND Tdd_IDNo IN ({1})
    
                                            UPDATE T_EmpSystemAdj
                                            SET Tsa_PostFlag = 0
                                            WHERE Tsa_AdjPayCycle = '{0}'
                                                AND Tsa_IDNo IN ({1})

                                            ---DELETE FROM T_EmpSystemAdj2
                                            ---WHERE Tsa_AdjPayCycle = '{0}'
                                            ---    AND Tsa_IDNo IN ({1})

                                            ---DELETE FROM T_EmpSystemAdjMisc
                                            ---WHERE Tsm_AdjPayCycle = '{0}'
                                            ---    AND Tsm_IDNo IN ({1})

                                            ---DELETE FROM T_EmpSystemAdjMisc2
                                            ---WHERE Tsm_AdjPayCycle = '{0}'
                                            ---    AND Tsm_IDNo IN ({1})

                                            UPDATE T_EmpIncome
                                            SET Tin_PostFlag = 0
                                            WHERE Tin_PayCycle = '{0}'
                                                AND Tin_IDNo IN ({1})

                                            UPDATE T_EmpManualAdj
                                            SET Tma_PostFlag = 0
                                            WHERE Tma_PayCycle = '{0}'
                                                AND Tma_IDNo IN ({1})
                                            ", PayPeriod, EmployeeIds);
            #endregion
            dalHelper.ExecuteNonQuery(query);
        }

        public bool IsLastQuincena(string PayPeriod)
        {
            string strLastCycle = commonBL.GetParameterValueFromPayroll("LASTCYCLE", CompanyCode, dal);
            bool bLastCycle = false;
            if (strLastCycle.Length == 3 && PayPeriod.Length == 7)
            {
                if (strLastCycle.Substring(0, 2) == PayPeriod.Substring(4, 2)
                    && strLastCycle.Substring(2, 1) == PayPeriod.Substring(6, 1))
                    bLastCycle = true;
            }

            return bLastCycle;
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

        public DataTable GetPayCodeRateOvertime(bool bMiscellaneous, string PayrollType, string PremiumGroup)
        {
            string condition = string.Empty;
            if (bMiscellaneous)
                condition = "AND ISNULL(Mmd_MiscDayID,0) > 0";
            else condition = "AND ISNULL(Mmd_MiscDayID,0) = 0";

            string query = string.Format(@"
                                    SELECT CASE WHEN Mdp_RestDayFlag = 1 and Mdp_DayCode = 'REST' then ''
                                        WHEN Mdp_RestDayFlag = 1 and Mdp_DayCode <> 'REST' then 'REST'
                                        ELSE '' END +   
                                        CASE 
                                            WHEN Mdp_DayCode = 'HOL' then 'LEGHOL'
                                            WHEN Mdp_DayCode =  'SPL' then 'SPLHOL'
                                            WHEN Mdp_DayCode =  'COMP' then 'COMPHOL'
                                        ELSE  Mdp_DayCode END as [DayCode]
									    , Mpd_ParamValue as [SpecialRate]
                                     FROM {0}..M_DayPremium
									 INNER JOIN M_PolicyDtl
											ON RTRIM(Mpd_PolicyCode) = 'PAYCODERATE'
											AND RTRIM(Mpd_SubCode) = Mdp_DayCode
											AND RTRIM(Mpd_CompanyCode) = '{1}'
                                     LEFT JOIN {0}..M_Day ON Mdp_DayCode = Mdy_DayCode
                                            AND Mdp_CompanyCode = Mdy_CompanyCode
                                            AND Mdy_RecordStatus = 'A'
                                     LEFT JOIN {0}..M_MiscellaneousDay
                                            ON Mmd_DayCode = Mdp_DayCode
                                            AND Mmd_RestDayFlag = Mdp_RestDayFlag
                                            AND Mmd_CompanyCode = Mdp_CompanyCode
                                            AND Mmd_RecordStatus = 'A'
							        WHERE Mdp_PayrollType = '{2}' 
							            AND Mdp_RecordStatus = 'A'
                                        AND Mdp_CompanyCode = '{1}'
                                        AND Mdp_PremiumGrpCode = '{3}'
							            {4}
                                    ORDER BY Mdp_SequenceOfDisplay"
                                    , CentralProfile
                                    , CompanyCode
                                    , PayrollType
                                    , PremiumGroup
                                    , condition);

            DataTable dtResult;
            dtResult = dal.ExecuteDataSet(query).Tables[0];

            return dtResult;
        }

        public DataTable GetPayCodeRateOvertime(bool bMiscellaneous, string PayrollType, string PremiumGroup, DALHelper dalhelper)
        {
            string condition = string.Empty;
            if (bMiscellaneous)
                condition = "AND ISNULL(Mmd_MiscDayID,0) > 0";
            else condition = "AND ISNULL(Mmd_MiscDayID,0) = 0";

            string query = string.Format(@"
                                    SELECT CASE WHEN Mdp_RestDayFlag = 1 and Mdp_DayCode = 'REST' then ''
                                        WHEN Mdp_RestDayFlag = 1 and Mdp_DayCode <> 'REST' then 'REST'
                                        ELSE '' END +   
                                        CASE 
                                            WHEN Mdp_DayCode = 'HOL' then 'LEGHOL'
                                            WHEN Mdp_DayCode =  'SPL' then 'SPLHOL'
                                            WHEN Mdp_DayCode =  'COMP' then 'COMPHOL'
                                        ELSE  Mdp_DayCode END as [DayCode]
									    , Mpd_ParamValue as [SpecialRate]
                                     FROM {0}..M_DayPremium
									 INNER JOIN M_PolicyDtl
											ON RTRIM(Mpd_PolicyCode) = 'PAYCODERATE'
											AND RTRIM(Mpd_SubCode) = Mdp_DayCode
											AND RTRIM(Mpd_CompanyCode) = '{1}'
                                     LEFT JOIN {0}..M_Day ON Mdp_DayCode = Mdy_DayCode
                                            AND Mdp_CompanyCode = Mdy_CompanyCode
                                            AND Mdy_RecordStatus = 'A'
                                     LEFT JOIN {0}..M_MiscellaneousDay
                                            ON Mmd_DayCode = Mdp_DayCode
                                            AND Mmd_RestDayFlag = Mdp_RestDayFlag
                                            AND Mmd_CompanyCode = Mdp_CompanyCode
                                            AND Mmd_RecordStatus = 'A'
							        WHERE Mdp_PayrollType = '{2}' 
							            AND Mdp_RecordStatus = 'A'
                                        AND Mdp_CompanyCode = '{1}'
                                        AND Mdp_PremiumGrpCode = '{3}'
							            {4}
                                    ORDER BY Mdp_SequenceOfDisplay"
                                    , CentralProfile
                                    , CompanyCode
                                    , PayrollType
                                    , PremiumGroup
                                    , condition);

            DataTable dtResult;
            dtResult = dalhelper.ExecuteDataSet(query).Tables[0];

            return dtResult;
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

        public decimal GetFormulaQueryDecimalValue(string query, ParameterInfo[] paramInfo, DALHelper dalhelper)
        {
            decimal dValue = 0;
            DataTable dtResult = dalhelper.ExecuteDataSet(query, CommandType.Text, paramInfo).Tables[0];
            if (dtResult.Rows.Count > 0)
            {
                dValue = Convert.ToDecimal(dtResult.Rows[0][0]);
            }
            return dValue;
        }

        private void GetPremium(DataTable dtDayCodePremiums, string PayrollType, string PremiumGroup, string DayCode, bool isRestDay, ref decimal RG, ref decimal OT, ref decimal ND, ref decimal OTND, ref decimal RGNDPercent, ref decimal OTNDPercent)
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
                throw new PayrollException("Day Premium Group [" + PremiumGroup + "] has No Premium for " + DayCode + " Day Code");
            //else
            //{
            //    drDayCodePremium = dtDayCodePremiums.Select(string.Format("Mdp_PremiumGrpCode = '{0}' AND Mdp_PayrollType = '{1}' AND Mdp_DayCode = '{2}' AND Mdp_RestDayFlag = {3}", "DEFAULTGRP", PayrollType, DayCode, isRestDay));
            //    if (drDayCodePremium.Length > 0)
            //    {
            //        RG = Convert.ToDecimal(drDayCodePremium[0]["Mdp_RGPremiumRate"]) / 100;
            //        OT = Convert.ToDecimal(drDayCodePremium[0]["Mdp_OTPremiumRate"]) / 100;
            //        ND = Convert.ToDecimal(drDayCodePremium[0]["Mdp_RGNDPremiumRate"]) / 100;
            //        OTND = Convert.ToDecimal(drDayCodePremium[0]["Mdp_OTNDPremiumRate"]) / 100;
            //        RGNDPercent = Convert.ToDecimal(drDayCodePremium[0]["Mdp_RGNDPercentageRate"]) / 100;
            //        OTNDPercent = Convert.ToDecimal(drDayCodePremium[0]["Mdp_OTNDPercentageRate"]) / 100;
            //    }
            //    else
            //        throw new PayrollException("Day Premium Group [" + PremiumGroup + "] has No Premium for " + DayCode + " Day Code");
            //}
        }

        public string GetPayrollParameters(string PayCycleCode, string taxSchedule, bool bYearEndAdjustment)
        {
            ProcessPayrollPeriod = PayCycleCode;
            TaxSchedule = taxSchedule;
            string PayrollProcessingErrorMessage = "";

            StatusHandler(this, new StatusEventArgs("Checking for Payroll Parameters", false));
            StatusHandler(this, new StatusEventArgs("Checking for Payroll Parameters", true));
            //-----------------------------
            DataTable dtPremiumMaxShare = GetMaxPrem(PayCycleCode);
            if (dtPremiumMaxShare.Rows.Count > 0)
            {
                DataRow[] drResult = dtPremiumMaxShare.Select("DeductionCode = 'HDMFPREM'");
                if (drResult.Length > 0)
                {
                    MaxHDMFPremPayCycle = GetValue(drResult[0]["MaxPayCycle"]);
                    //HDMFBaseMaxShare = GetValue(drResult[0]["MaxShare"]);
                }

                drResult = null;
                drResult = dtPremiumMaxShare.Select("DeductionCode = 'SSSPREM'");
                if (drResult.Length > 0)
                {
                    MaxSSSPremPayCycle = GetValue(drResult[0]["MaxPayCycle"]);
                    SSSBaseMaxShare = GetValue(drResult[0]["MaxShare"]);
                }
                drResult = null;
                drResult = dtPremiumMaxShare.Select("DeductionCode = 'PHICPREM'");
                if (drResult.Length > 0)
                {
                    MaxPhilHealthPremPayCycle = GetValue(drResult[0]["MaxPayCycle"]);
                    PhilHealthBaseMaxShare = GetValue(drResult[0]["MaxShare"]);
                }

                drResult = null;
                drResult = dtPremiumMaxShare.Select("DeductionCode = 'UNIONDUES'");
                if (drResult.Length > 0)
                {
                    MaxUNIONDUESPremPayCycle = GetValue(drResult[0]["MaxPayCycle"]);
                    UNIONDUESBaseMaxShare = GetValue(drResult[0]["MaxShare"]);
                }
            }
             //----------------------------   
            //if (HDMFBaseMaxShare == "")
            //    PayrollProcessingErrorMessage += "Pagibig Premium Maximum Share is not set-up.\n";

            //StatusHandler(this, new StatusEventArgs(string.Format("Pagibig Premium Max Share = {0}", HDMFBaseMaxShare), false));
            //StatusHandler(this, new StatusEventArgs(string.Format("Pagibig Premium Max Share = {0}", HDMFBaseMaxShare), true));

            //-----------------------------
            if (MaxHDMFPremPayCycle == "")
                PayrollProcessingErrorMessage += "Applicable Pagibig Premium Pay Cycle is not set-up.\n";
            StatusHandler(this, new StatusEventArgs(string.Format("Max Pagibig Pay Cycle = {0}", MaxHDMFPremPayCycle), false));
            StatusHandler(this, new StatusEventArgs(string.Format("Max Pagibig Pay Cycle = {0}", MaxHDMFPremPayCycle), true));

            //-----------------------------
            if (SSSBaseMaxShare == "")
                PayrollProcessingErrorMessage += "SSS Premium Maximum Share is not set-up.\n";
            StatusHandler(this, new StatusEventArgs(string.Format("SSS Premium Max Share = {0}", SSSBaseMaxShare), false));
            StatusHandler(this, new StatusEventArgs(string.Format("SSS Premium Max Share = {0}", SSSBaseMaxShare), true));
            //-----------------------------
            if (MaxSSSPremPayCycle == "")
                PayrollProcessingErrorMessage += "Applicable SSS Premium Pay Cycle is not set-up.\n";
            StatusHandler(this, new StatusEventArgs(string.Format("Max SSS Pay Cycle = {0}", MaxSSSPremPayCycle), false));
            StatusHandler(this, new StatusEventArgs(string.Format("Max SSS Pay Cycle = {0}", MaxSSSPremPayCycle), true));

            //-----------------------------
            if (PhilHealthBaseMaxShare == "")
                PayrollProcessingErrorMessage += "Philhealth Premium Maximum Share is not set-up.\n";
            StatusHandler(this, new StatusEventArgs(string.Format("Philhealth Premium Max Share = {0}", PhilHealthBaseMaxShare), false));
            StatusHandler(this, new StatusEventArgs(string.Format("Philhealth Premium Max Share = {0}", PhilHealthBaseMaxShare), true));
            //-----------------------------
            
            if (MaxPhilHealthPremPayCycle == "")
                PayrollProcessingErrorMessage += "Applicable Philhealth Premium Pay Cycle is not set-up.\n";
            StatusHandler(this, new StatusEventArgs(string.Format("Max PhilHealth Pay Cycle = {0}", MaxPhilHealthPremPayCycle), false));
            StatusHandler(this, new StatusEventArgs(string.Format("Max PhilHealth Pay Cycle = {0}", MaxPhilHealthPremPayCycle), true));
            //-----------------------------
            if (UNIONDUES)
            {
                if (UNIONDUESBaseMaxShare == "")
                    PayrollProcessingErrorMessage += "Union Dues Maximum Share is not set-up.\n";
                StatusHandler(this, new StatusEventArgs(string.Format("Union Dues Max Share = {0}", UNIONDUESBaseMaxShare), false));
                StatusHandler(this, new StatusEventArgs(string.Format("Union Dues Max Share = {0}", UNIONDUESBaseMaxShare), true));
                //-----------------------------
                if (MaxUNIONDUESPremPayCycle == "")
                    PayrollProcessingErrorMessage += "Applicable UNION DUES Pay Cycle is not set-up.\n";
                StatusHandler(this, new StatusEventArgs(string.Format("Max UNION DUES Pay Cycle = {0}", MaxUNIONDUESPremPayCycle), false));
                StatusHandler(this, new StatusEventArgs(string.Format("Max UNION DUES Pay Cycle = {0}", MaxUNIONDUESPremPayCycle), true));
            }

            //-----------------------------
            MaxTaxHeader = GetMaxPayPeriodTaxScheduleHeader(TaxSchedule);
            if (MaxTaxHeader == "" && !bYearEndAdjustment)
                PayrollProcessingErrorMessage += "Applicable Regular Tax payroll period is not set-up.\n";
            StatusHandler(this, new StatusEventArgs(string.Format("Applicable Regular Tax Period = {0}", MaxTaxHeader), false));
            StatusHandler(this, new StatusEventArgs(string.Format("Applicable Regular Tax Period = {0}", MaxTaxHeader), true));
            //-----------------------------
            MaxTaxAnnual = GetMaxPayPeriodAnnualTaxSchedule();
            if (MaxTaxAnnual == "" && bYearEndAdjustment)
                PayrollProcessingErrorMessage += "Applicable Annual Tax pay cycle is not set-up.\n";
            StatusHandler(this, new StatusEventArgs(string.Format("Applicable Annual Tax Period = {0}", MaxTaxAnnual), false));
            StatusHandler(this, new StatusEventArgs(string.Format("Applicable Annual Tax Period = {0}", MaxTaxAnnual), true));
            //-----------------------------
            MaxTaxExemption = GetMaxPayPeriodTaxExemption();
            if (MaxTaxExemption == "" && bYearEndAdjustment)
                PayrollProcessingErrorMessage += "Applicable Tax Exemption pay cycle is not set-up.\n";
            StatusHandler(this, new StatusEventArgs(string.Format("Applicable Tax Exemption Period = {0}", MaxTaxExemption), false));
            StatusHandler(this, new StatusEventArgs(string.Format("Applicable Tax Exemption Period = {0}", MaxTaxExemption), true));
            //-----------------------------
            DataTable dtExemption = GetAdditionalExemptionAmount();
            if (dtExemption.Rows.Count <= 0)
                PayrollProcessingErrorMessage += "Additional tax exemption is not set-up.\n";
            else
            {
                #region Get Tax Exemption Amounts
                for (int i = 0; i < dtExemption.Rows.Count; i++)
                {
                    if (GetValue(dtExemption.Rows[i]["Mte_TaxCode"].ToString()).Equals("D1"))
                    {
                        TaxExemption1 = getDecimalValue(dtExemption.Rows[i]["Mte_Amount"]);
                    }
                    else if (GetValue(dtExemption.Rows[i]["Mte_TaxCode"].ToString()).Equals("D2"))
                    {
                        TaxExemption2 = getDecimalValue(dtExemption.Rows[i]["Mte_Amount"]);
                    }
                    else if (GetValue(dtExemption.Rows[i]["Mte_TaxCode"].ToString()).Equals("D3"))
                    {
                        TaxExemption3 = getDecimalValue(dtExemption.Rows[i]["Mte_Amount"]);
                    }
                    else if (GetValue(dtExemption.Rows[i]["Mte_TaxCode"].ToString()).Equals("D4"))
                    {
                        TaxExemption4 = getDecimalValue(dtExemption.Rows[i]["Mte_Amount"]);
                    }
                }
                TaxExemption2 = TaxExemption1 + TaxExemption2;
                TaxExemption3 = TaxExemption2 + TaxExemption3;
                TaxExemption4 = TaxExemption3 + TaxExemption4;
                #endregion
            }
            //-----------------------------
            DataTable dtGovRemittance = GetGovRemittance();
            DataRow[] drGovRem = dtGovRemittance.Select("Mgr_RemittanceCode = 'PHICPREM'");
            if (drGovRem.Length > 0)
            {
                ApplicPHPrem = GetValue(drGovRem[0]["Mgr_DeductionPayCycle"]);
                PHREFERPREVINCOMEPREM = Convert.ToBoolean(GetValue(drGovRem[0]["Mgr_ReferPreviousIncomePremium"])); ////PHPREMLOOK1
                PHEARLYCYCLEFULLDEDN = Convert.ToBoolean(GetValue(drGovRem[0]["Mgr_EarlyCycleFullDeduction"])); ////PHCONFIX1ST
            }
            else
                PayrollProcessingErrorMessage += "PHICPREM is not set-up in Government Remittance table.\n";
            StatusHandler(this, new StatusEventArgs(string.Format("Government Remittance =  PHICPREM"), false));
            StatusHandler(this, new StatusEventArgs(string.Format("Government Remittance =  PHICPREM"), true));

            //-----------------------------
            drGovRem = null;
            drGovRem = dtGovRemittance.Select("Mgr_RemittanceCode = 'SSSPREM'");
            if (drGovRem.Length > 0)
            {
                ApplicSSSPrem = GetValue(drGovRem[0]["Mgr_DeductionPayCycle"]);
                SSSREFERPREVINCOMEPREM = Convert.ToBoolean(GetValue(drGovRem[0]["Mgr_ReferPreviousIncomePremium"])); ////SSSPREMLOOK1
                SSSEARLYCYCLEFULLDEDN = Convert.ToBoolean(GetValue(drGovRem[0]["Mgr_EarlyCycleFullDeduction"])); ////SSSCONFIX1ST
            }
            else 
                PayrollProcessingErrorMessage += "SSSPREM is not set-up in Government Remittance table.\n";
            StatusHandler(this, new StatusEventArgs(string.Format("Government Remittance = SSSPREM"), false));
            StatusHandler(this, new StatusEventArgs(string.Format("Government Remittance = SSSPREM"), true));

            //-----------------------------
            drGovRem = null;
            drGovRem = dtGovRemittance.Select("Mgr_RemittanceCode = 'HDMFPREM'");
            if (drGovRem.Length > 0)
            {
                ApplicHDMFPrem = GetValue(drGovRem[0]["Mgr_DeductionPayCycle"]);
                HDMFREFERPREVINCOMEPREM = Convert.ToBoolean(GetValue(drGovRem[0]["Mgr_ReferPreviousIncomePremium"])); ////HDMFPREMLOOK1
                HDMFEARLYCYCLEFULLDEDN = Convert.ToBoolean(GetValue(drGovRem[0]["Mgr_EarlyCycleFullDeduction"])); ////HDMFCONFIX1ST
                HDMFREMMAXSHARE = GetValue(drGovRem[0]["Mgr_MaxShare"]);
                HDMFREMMAXSHARE = HDMFREMMAXSHARE.Equals(string.Empty) ? "0" : HDMFREMMAXSHARE;
            }
            else
                PayrollProcessingErrorMessage += "HDMFPREM is not set-up in Government Remittance table.\n";
            StatusHandler(this, new StatusEventArgs(string.Format("Government Remittance = HDMFPREM"), false));
            StatusHandler(this, new StatusEventArgs(string.Format("Government Remittance = HDMFPREM"), true));
            //-----------------------------
            if (UNIONDUES)
            {
                drGovRem = null;
                drGovRem = dtGovRemittance.Select("Mgr_RemittanceCode = 'UNIONDUES'");
                if (drGovRem.Length > 0)
                {
                    ApplicUnionPrem = GetValue(drGovRem[0]["Mgr_DeductionPayCycle"]);
                    UNIONREFERPREVINCOMEPREM = Convert.ToBoolean(GetValue(drGovRem[0]["Mgr_ReferPreviousIncomePremium"]));
                    UNIONEARLYCYCLEFULLDEDN = Convert.ToBoolean(GetValue(drGovRem[0]["Mgr_EarlyCycleFullDeduction"]));
                }
                else
                    PayrollProcessingErrorMessage += "Union Dues is not set-up in Government Remittance table.\n";
                StatusHandler(this, new StatusEventArgs(string.Format("Remittance = UNIONDUES"), false));
                StatusHandler(this, new StatusEventArgs(string.Format("Remittance = UNIONDUES"), true));
            }
            //-----------------------------
            DataTable dtPremiumGrpListWithoutSetup = GetPremiumGrpListWithoutSetup();
            if (dtPremiumGrpListWithoutSetup.Rows.Count > 0)
            {
                for (int idx = 0; idx < dtPremiumGrpListWithoutSetup.Rows.Count; idx++)
                {
                    PayrollProcessingErrorMessage += dtPremiumGrpListWithoutSetup.Rows[idx]["DayCode"].ToString() + " day code, "
                                                        + dtPremiumGrpListWithoutSetup.Rows[idx]["PremGrp"].ToString() + " premium group and "
                                                        + dtPremiumGrpListWithoutSetup.Rows[idx]["PayType"].ToString() + " payroll type has no set-up.\n";
                }
            }
            //-----------------------------
            if (commonBL.GetFillerDayCodesCount(CompanyCode, CentralProfile, dal) > 0)
            {
                DataTable dtPremiumGrpListWithoutSetupMisc = GetPremiumGrpListWithoutSetupMisc();
                if (dtPremiumGrpListWithoutSetupMisc.Rows.Count > 0)
                {
                    for (int i = 0; i < dtPremiumGrpListWithoutSetupMisc.Rows.Count; i++)
                    {
                        PayrollProcessingErrorMessage += dtPremiumGrpListWithoutSetupMisc.Rows[i]["DayCode"].ToString() +  " miscellaneous day, " 
                                                        + dtPremiumGrpListWithoutSetupMisc.Rows[i]["PremGrp"].ToString() + " premium group and "
                                                        + dtPremiumGrpListWithoutSetupMisc.Rows[i]["PayType"].ToString() + " payroll type has no set-up.\n";
                    }
                }
            }
            //-----------------------------
            LATEBRCKTQ = GetBracketParameter("LATEBRCKTQ");
            StatusHandler(this, new StatusEventArgs(string.Format("Getting late bracket deduction"), false));
            StatusHandler(this, new StatusEventArgs(string.Format("Getting late bracket deduction"), true));

            NEWHIRE = commonBL.GetParameterDtlValueFromPayroll("REGPAYRULE", "NEWHIRE", CompanyCode, dal);
            if (NEWHIRE == "")
                PayrollProcessingErrorMessage += "Newly-Hired Monthlies Employees is not set-up in Master.\n";
            StatusHandler(this, new StatusEventArgs(string.Format("Getting Newly-Hired Monthlies Employees set-up"), false));
            StatusHandler(this, new StatusEventArgs(string.Format("Getting Newly-Hired Monthlies Employees set-up"), true));

            MULTSALMD = commonBL.GetParameterDtlValueFromPayroll("REGPAYRULE", "MULTSALMD", CompanyCode, dal);
            if (MULTSALMD == "")
                PayrollProcessingErrorMessage += "Multiple Salary from Monthlies to Dailies is not set-up in Master.\n";
            StatusHandler(this, new StatusEventArgs(string.Format("Getting Multiple Salary from Monthlies to Dailies set-up"), false));
            StatusHandler(this, new StatusEventArgs(string.Format("Getting Multiple Salary from Monthlies to Dailies set-up"), true));

            MULTSALDM = commonBL.GetParameterDtlValueFromPayroll("REGPAYRULE", "MULTSALDM", CompanyCode, dal);
            if (MULTSALDM == "")
                PayrollProcessingErrorMessage += "Multiple Salary from Dailies to Monthlies is not set-up in Master.\n";
            StatusHandler(this, new StatusEventArgs(string.Format("Getting Multiple Salary from Dailies to Monthlies set-up"), false));
            StatusHandler(this, new StatusEventArgs(string.Format("Getting Multiple Salary from Dailies to Monthlies set-up"), true));
            //-----------------------------
            DataTable dtTax = GetTaxMaster();
            if (dtTax.Rows.Count > 0)
            {
                REFERPREVINCOMETAX = Convert.ToBoolean(dtTax.Rows[0]["Mtx_ReferPreviousIncomeTax"].ToString());
                StatusHandler(this, new StatusEventArgs(string.Format("REFERPREVINCOMETAX = {0}", REFERPREVINCOMETAX), false));
                StatusHandler(this, new StatusEventArgs(string.Format("REFERPREVINCOMETAX = {0}", REFERPREVINCOMETAX), true));

                string taxAnnualization = dtTax.Rows[0]["Mtx_TaxAnnualization"].ToString();
                if (string.IsNullOrEmpty(taxAnnualization))
                    PayrollProcessingErrorMessage += "Tax Annualization is not set-up in Master.\n";
                TXANNLQUINFORMULA1 = (taxAnnualization == "ASSUME" ? "1" : "0");
                TXANNLQUINFORMULA2 = (taxAnnualization == "AVERAGE" ? "1" : "0");
                StatusHandler(this, new StatusEventArgs(string.Format("Getting tax annualization formula"), false));
                StatusHandler(this, new StatusEventArgs(string.Format("Getting tax annualization formula"), true));

                TAXRULE = dtTax.Rows[0]["Mtx_TaxRule"].ToString();
                if (string.IsNullOrEmpty(TAXRULE))
                    PayrollProcessingErrorMessage += "Tax Rule is not set-up in Master.\n";
                StatusHandler(this, new StatusEventArgs(string.Format("TAXRULE = {0}", TAXRULE), false));
                StatusHandler(this, new StatusEventArgs(string.Format("TAXRULE = {0}", TAXRULE), true));

                BIREMPSTATTOPROCESS = dtTax.Rows[0]["Mtx_BIREmploymentStatusToProcess"].ToString();
                if (string.IsNullOrEmpty(BIREMPSTATTOPROCESS))
                    PayrollProcessingErrorMessage += "Employment Status to process is not set-up in Master.\n";
                StatusHandler(this, new StatusEventArgs(string.Format("BIREMPSTATTOPROCESS = {0}", BIREMPSTATTOPROCESS), false));
                StatusHandler(this, new StatusEventArgs(string.Format("BIREMPSTATTOPROCESS = {0}", BIREMPSTATTOPROCESS), true));

                ASSUMERECURRINGALLOWANCE = Convert.ToBoolean(dtTax.Rows[0]["Mtx_AssumeRecurringAllowance"].ToString());
                StatusHandler(this, new StatusEventArgs(string.Format("ASSUMERECURRINGALLOWANCE = {0}", ASSUMERECURRINGALLOWANCE), false));
                StatusHandler(this, new StatusEventArgs(string.Format("ASSUMERECURRINGALLOWANCE = {0}", ASSUMERECURRINGALLOWANCE), true));

                ApplicTax = dtTax.Rows[0]["Mtx_DeductionPayCycle"].ToString();
                if (string.IsNullOrEmpty(ApplicTax))
                    PayrollProcessingErrorMessage += "Applicable Paycycle is not set-up in Master.\n";
                StatusHandler(this, new StatusEventArgs(string.Format("ApplicTax = {0}", ApplicTax), false));
                StatusHandler(this, new StatusEventArgs(string.Format("ApplicTax = {0}", ApplicTax), true));

            }
            else
                PayrollProcessingErrorMessage += "No Tax Master record.\n";

            #region SPECIAL RATE
            if (Convert.ToBoolean(PAYSPECIALRATE))
            {
                PAYCODERATE = commonBL.GetParameterDtlListfromPayroll("PAYCODERATE", CompanyCode, dal);
                if (PAYCODERATE.Rows.Count == 0 )
                    PayrollProcessingErrorMessage += "Special Pay Code Rate is not set-up in Master.\n";
                StatusHandler(this, new StatusEventArgs(string.Format("Getting special rate set-up"), false));
                StatusHandler(this, new StatusEventArgs(string.Format("Getting special rate set-up"), true));
                GetSpecialRate();

                if (commonBL.GetFillerDayCodesCount(CompanyCode, CentralProfile, dal) > 0)
                {
                    DataTable dtMiscDay = dal.ExecuteDataSet(string.Format(@"SELECT Mmd_MiscDayID, Mmd_DayCode 
								                                FROM {0}..M_MiscellaneousDay 
								                                WHERE Mmd_CompanyCode = '{1}'
									                                AND Mmd_RecordStatus = 'A'", CentralProfile, CompanyCode)
                                                             ).Tables[0];
                    for(int idx = 0; idx < dtMiscDay.Rows.Count; idx++)
                    {
                        DataRow[] drRow = PAYCODERATE.Select("Mpd_SubCode = '" + dtMiscDay.Rows[idx][1].ToString() + "'");
                        if (drRow.Length > 0)
                        {
                            switch(dtMiscDay.Rows[idx][0].ToString())
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
                }

                SPECIALRATEFORMULA_SRATE = commonBL.GetParameterFormulaFromPayroll("SPECIALRATEFRM-SRATE", CompanyCode, dal);
                if (SPECIALRATEFORMULA_SRATE == "")
                    PayrollProcessingErrorMessage += "Special Salary Rate is not set-up in Master.\n";
                StatusHandler(this, new StatusEventArgs(string.Format("Getting special salary rate set-up"), false));
                StatusHandler(this, new StatusEventArgs(string.Format("Getting special salary rate set-up"), true));

                SPECIALRATEFORMULA_HRATE = commonBL.GetParameterFormulaFromPayroll("SPECIALRATEFRM-HRATE", CompanyCode, dal);
                if (SPECIALRATEFORMULA_HRATE == "")
                    PayrollProcessingErrorMessage += "Special Hourly Rate is not set-up in Master.\n";
                StatusHandler(this, new StatusEventArgs(string.Format("Getting special hourly rate set-up"), false));
                StatusHandler(this, new StatusEventArgs(string.Format("Getting special hourly rate set-up"), true));
            }
            #endregion

            return PayrollProcessingErrorMessage;
        }

        public void SetParameters(string companyCode, string centralProfile, DALHelper dalHelper)
        {
            CompanyCode = companyCode;
            CentralProfile = centralProfile;
            this.dal = dalHelper;
            this.commonBL = new CommonBL();
            SetParameters();
        }

        public void SetParameters()
        {
            string strResult = string.Empty;
            #region Parameters
            StatusHandler(this, new StatusEventArgs("Loading Parameters", false));
            StatusHandler(this, new StatusEventArgs("Loading Parameters", true));

            strResult = commonBL.GetParameterValueFromPayroll("MDIVISOR", CompanyCode , dal);
            MDIVISOR = strResult.Equals(string.Empty) ? 0 : Convert.ToDecimal(strResult);
            StatusHandler(this, new StatusEventArgs(string.Format(" MDIVISOR = {0} ", MDIVISOR), false));
            StatusHandler(this, new StatusEventArgs(string.Format(" MDIVISOR = {0} ", MDIVISOR), true));

            strResult = commonBL.GetParameterValueFromPayroll("NETPCT", CompanyCode, dal);
            NETPCT = strResult.Equals(string.Empty) ? 0 : Convert.ToDecimal(strResult);
            StatusHandler(this, new StatusEventArgs(string.Format(" NETPCT = {0} ", NETPCT), false));
            StatusHandler(this, new StatusEventArgs(string.Format(" NETPCT = {0} ", NETPCT), true));

            strResult = commonBL.GetParameterDtlValueFromPayroll("MAXPAYCHK", "MAXREGPAY", CompanyCode);
            MAXREGPAY = strResult.Equals(string.Empty) ? 0 : Convert.ToDecimal(strResult);
            StatusHandler(this, new StatusEventArgs(string.Format(" MAXREGPAY = {0} ", MAXREGPAY), false));
            StatusHandler(this, new StatusEventArgs(string.Format(" MAXREGPAY = {0} ", MAXREGPAY), true));

            strResult = commonBL.GetParameterDtlValueFromPayroll("MAXPAYCHK", "MAXTAXINC", CompanyCode);
            MAXTAXINC = strResult.Equals(string.Empty) ? 0 : Convert.ToDecimal(strResult);
            StatusHandler(this, new StatusEventArgs(string.Format(" MAXTAXINC = {0} ", MAXTAXINC), false));
            StatusHandler(this, new StatusEventArgs(string.Format(" MAXTAXINC = {0} ", MAXTAXINC), true));

            strResult = commonBL.GetParameterDtlValueFromPayroll("MAXPAYCHK", "MAXDEDN", CompanyCode);
            MAXDEDN = strResult.Equals(string.Empty) ? 0 : Convert.ToDecimal(strResult);
            StatusHandler(this, new StatusEventArgs(string.Format(" MAXDEDN = {0} ", MAXDEDN), false));
            StatusHandler(this, new StatusEventArgs(string.Format(" MAXDEDN = {0} ", MAXDEDN), true));

            strResult = commonBL.GetParameterDtlValueFromPayroll("MAXPAYCHK", "MAXNETPAY", CompanyCode);
            MAXNETPAY = strResult.Equals(string.Empty) ? 0 : Convert.ToDecimal(strResult);
            StatusHandler(this, new StatusEventArgs(string.Format(" MAXNETPAY = {0} ", MAXNETPAY), false));
            StatusHandler(this, new StatusEventArgs(string.Format(" MAXNETPAY = {0} ", MAXNETPAY), true));

            strResult = commonBL.GetParameterValueFromPayroll("MINNETPAY", CompanyCode, dal);
            MINNETPAY = strResult.Equals(string.Empty) ? 0 : Convert.ToDecimal(strResult);
            StatusHandler(this, new StatusEventArgs(string.Format(" MINNETPAY = {0} ", MINNETPAY), false));
            StatusHandler(this, new StatusEventArgs(string.Format(" MINNETPAY = {0} ", MINNETPAY), true));

            strResult = commonBL.GetParameterValueFromPayroll("HRLYRTEDEC", CompanyCode, dal);
            HRLYRTEDEC = strResult.Equals(string.Empty) ? 0 : Convert.ToDecimal(strResult);
            StatusHandler(this, new StatusEventArgs(string.Format(" HRLYRTEDEC = {0} ", HRLYRTEDEC), false));
            StatusHandler(this, new StatusEventArgs(string.Format(" HRLYRTEDEC = {0} ", HRLYRTEDEC), true));

            strResult = commonBL.GetParameterValueFromPayroll("M13EXCLTAX", CompanyCode, dal);
            M13EXCLTAX = strResult.Equals(string.Empty) ? 0 : Convert.ToDecimal(strResult);
            StatusHandler(this, new StatusEventArgs(string.Format(" M13EXCLTAX = {0} ", M13EXCLTAX), false));
            StatusHandler(this, new StatusEventArgs(string.Format(" M13EXCLTAX = {0} ", M13EXCLTAX), true));

            strResult = commonBL.GetParameterValueFromPayroll("UNIONDUES", CompanyCode, dal);
            UNIONDUES = strResult.Equals(string.Empty) ? false : Convert.ToBoolean(strResult);
            StatusHandler(this, new StatusEventArgs(string.Format(" UNIONDUES = {0} ", UNIONDUES), false));
            StatusHandler(this, new StatusEventArgs(string.Format(" UNIONDUES = {0} ", UNIONDUES), true));

            strResult = commonBL.GetParameterValueFromPayroll("NDBRCKTCNT", CompanyCode, dal);
            NDBRCKTCNT = strResult.Equals(string.Empty) ? 1 : Convert.ToInt32(Convert.ToDouble(strResult));
            StatusHandler(this, new StatusEventArgs(string.Format(" NDBRCKTCNT = {0} ", NDBRCKTCNT), false));
            StatusHandler(this, new StatusEventArgs(string.Format(" NDBRCKTCNT = {0} ", NDBRCKTCNT), true));

            PAYFREQNCY = commonBL.GetParameterValueFromPayroll("PAYFREQNCY", CompanyCode, dal);
            StatusHandler(this, new StatusEventArgs(string.Format(" PAYFREQNCY = {0} ", PAYFREQNCY), false));
            StatusHandler(this, new StatusEventArgs(string.Format(" PAYFREQNCY = {0} ", PAYFREQNCY), true));

            MULTSAL = commonBL.GetParameterValueFromPayroll("MULTSAL", CompanyCode, dal);
            StatusHandler(this, new StatusEventArgs(string.Format(" MULTSAL = {0} ", MULTSAL), false));
            StatusHandler(this, new StatusEventArgs(string.Format(" MULTSAL = {0} ", MULTSAL), true));

            NEGTAXINCOME = commonBL.GetParameterValueFromPayroll("NEGTAXINCOME", CompanyCode, dal);
            StatusHandler(this, new StatusEventArgs(string.Format(" NEGTAXINCOME = {0} ", NEGTAXINCOME), false));
            StatusHandler(this, new StatusEventArgs(string.Format(" NEGTAXINCOME = {0} ", NEGTAXINCOME), true));

            PAYSPECIALRATE = commonBL.GetParameterValueFromPayroll("PAYSPECIALRATE", CompanyCode, dal);
            StatusHandler(this, new StatusEventArgs(string.Format(" PAYSPECIALRATE = {0} ", PAYSPECIALRATE), false));
            StatusHandler(this, new StatusEventArgs(string.Format(" PAYSPECIALRATE = {0} ", PAYSPECIALRATE), true));

            NETBASE = commonBL.GetParameterValueFromPayroll("NETBASE", CompanyCode, dal);
            StatusHandler(this, new StatusEventArgs(string.Format(" NETBASE = {0} ", NETBASE), false));
            StatusHandler(this, new StatusEventArgs(string.Format(" NETBASE = {0} ", NETBASE), true));

            NP1_RATE = Convert.ToDecimal(commonBL.GetParameterDtlValueFromPayroll("NDBRACKET", "NP1_RATE", CompanyCode, dal)) / 100;
            StatusHandler(this, new StatusEventArgs(string.Format("NDBRACKET = {0}", NP1_RATE), false));
            StatusHandler(this, new StatusEventArgs(string.Format("NDBRACKET = {0}", NP1_RATE), true));

            NP2_RATE = Convert.ToDecimal(commonBL.GetParameterDtlValueFromPayroll("NDBRACKET", "NP2_RATE", CompanyCode, dal)) / 100;
            StatusHandler(this, new StatusEventArgs(string.Format("NDBRACKET = {0}", NP2_RATE), false));
            StatusHandler(this, new StatusEventArgs(string.Format("NDBRACKET = {0}", NP2_RATE), true));

            #endregion
        }

        #region DYNAMIC DEDUCTION OF PREMIUMS

        public DataTable Ordering(string CentralProfile) 
        {
            #region query
            string qry = string.Format
            (@" SELECT Mgr_RemittanceCode
                       ,Mgr_PriorityofDeduction
                FROM M_GovRemittance 
		        WHERE Mgr_RemittanceType IN ('P','U')
                    AND Mgr_CompanyCode = '{0}'
		        ORDER BY Mgr_PriorityofDeduction, Mgr_RemittanceCode ASC
              ", CompanyCode);
            #endregion
            DataTable dtResult = null;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(qry).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }
        //END
        #endregion


        private DataTable GetBracketParameter(string ParameterID)
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
            ds = dal.ExecuteDataSet(qString);

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0];
            else
                return new DataTable();
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

        public DataTable GetPayPeriodCycle(string PayPeriod)
        {
            string strQuery = string.Format(@"SELECT	Tps_PayCycle
		                                                , Tps_StartCycle
		                                                , Tps_EndCycle
                                                        , Tps_CycleIndicator
                                                        , Tps_CycleIndicatorSpecial
                                                        , ISNULL(Tps_ComputeSSS,0) Tps_ComputeSSS
                                                        , ISNULL(Tps_ComputePHIC,0) Tps_ComputePHIC
                                                        , ISNULL(Tps_ComputePagIbig,0) Tps_ComputePagIbig
                                                        , ISNULL(Tps_ComputeTax,0) Tps_ComputeTax
                                                        , ISNULL(Tps_ComputeUnion,0) Tps_ComputeUnion
                                                        , Tps_TaxSchedule
                                                        , ISNULL(Tps_MonthEnd,0) Tps_MonthEnd 
                                                FROM	T_PaySchedule 
                                                WHERE	Tps_PayCycle = '{0}' 
                                                AND		Tps_RecordStatus = 'A'", PayPeriod);

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(strQuery).Tables[0];
                dal.CloseDB();
            }

            return dtResult;
        }

        public DataTable GetTaxMaster()
        {
            #region query
            string query = string.Format(@"SELECT Mtx_ReferPreviousIncomeTax
                                                , Mtx_TaxAnnualization
                                                , Mtx_TaxRule
                                                , Mtx_BIREmploymentStatusToProcess
                                                , Mtx_AssumeRecurringAllowance
                                                , Mtx_DeductionPayCycle
	                                       FROM {1}..M_Tax
                                           WHERE  Mtx_RecordStatus = 'A'
                                                AND Mtx_CompanyCode = '{0}'", CompanyCode, CentralProfile);
            #endregion
            DataTable dtResult;
            dtResult = dal.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public int GetPremiumPayCounter(string PayCycle, string PremiumCode)
        {
            int PayCounter = 0;
            string condition = string.Empty;
            if (PremiumCode == "HDMFPREM")
                condition = "AND ISNULL(Tps_ComputePagIbig,0) = 1";
            else if (PremiumCode == "SSSPREM")
                condition = "AND ISNULL(Tps_ComputeSSS,0) = 1";
            else if (PremiumCode == "PHICPREM")
                condition = "AND ISNULL(Tps_ComputePHIC,0) = 1";

            switch (PAYFREQNCY)
            {
                case "W":
                    #region query
                    string query = string.Format(@"SELECT Tps_PayCycle FROM T_PaySchedule
                                                    WHERE Tps_PayCycle, 6 = '{0}'
                                                    AND Tps_CycleType = 'N'
                                                    {1}", PayCycle, condition);
                    #endregion
                    query = string.Format(query);
                    PayCounter = dal.ExecuteDataSet(query).Tables[0].Rows.Count;
                    break;
                case "S":
                    PayCounter = 2;
                    break;
                case "M":
                    PayCounter = 1;
                    break;
            }
            return PayCounter;
        }

        public DataTable GetGovRemittance()
        {
            #region query
            string query = @"SELECT LEFT(Mgr_DeductionPayCycle,1) as Mgr_DeductionPayCycle    ---- 0
                            , Mgr_ReferPreviousIncomePremium         ---- 1
                            , Mgr_EarlyCycleFullDeduction            ---- 2
                            , Mgr_MaxShare                           ---- 3
                            , Mgr_RemittanceCode
                           FROM {0}..M_GovRemittance
                           WHERE Mgr_CompanyCode = '{1}'
                                AND Mgr_RecordStatus = 'A'";
            #endregion
            query = string.Format(query, CentralProfile, CompanyCode);
            DataTable dtResult;
            dtResult = dal.ExecuteDataSet(query).Tables[0];
            if (dtResult.Rows.Count > 0)
                return dtResult;
            else
                return new DataTable();
        }

        public DataTable GetPremiumGrpListWithoutSetupMisc()
        {
            #region query
            string query = @"SELECT PREMGRP.Mcd_Code as [PremGrp], PAYTYPE.Mcd_Code as [PayType], Mmd_MiscDayID, Mmd_DayCode as [DayCode]
                            FROM {0}..M_CodeDtl PREMGRP 
                            FULL OUTER JOIN {0}..M_CodeDtl PAYTYPE on  PAYTYPE.Mcd_CompanyCode = PREMGRP.Mcd_CompanyCode	
	                            AND PAYTYPE.Mcd_CodeType ='PAYTYPE'	
	                            AND PAYTYPE.Mcd_RecordStatus = 'A'
                            INNER JOIN {0}..M_MiscellaneousDay ON Mmd_CompanyCode = ISNULL(PREMGRP.Mcd_CompanyCode,PAYTYPE.Mcd_CompanyCode)
	                            AND 1 = 1
                            LEFT JOIN {0}..M_DayPremium ON Mdp_CompanyCode = ISNULL(PREMGRP.Mcd_CompanyCode,PAYTYPE.Mcd_CompanyCode)
	                            AND Mdp_PremiumGrpCode = PREMGRP.Mcd_Code
	                            AND Mdp_PayrollType = PAYTYPE.Mcd_Code
	                            AND Mdp_DayCode = Mmd_DayCode
	                            AND Mdp_RestDayFlag = Mmd_RestDayFlag
                            WHERE PREMGRP.Mcd_CompanyCode = '{1}'		
	                            AND PREMGRP.Mcd_CodeType ='PREMGRP'		
	                            AND PAYTYPE.Mcd_RecordStatus = 'A'
	                            AND Mdp_PremiumGrpCode IS NULL";
            #endregion
            query = string.Format(query, CentralProfile, CompanyCode);
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            if (dtResult.Rows.Count > 0)
                return dtResult;
            else
                return new DataTable();
        }

        public DataTable GetPremiumGrpListWithoutSetup()
        {
            #region query
            string query = @"SELECT PREMGRP.Mcd_Code as [PremGrp]
                            , PAYTYPE.Mcd_Code as [PayType]
                            , Mdy_DayCode as [DayCode]
                            , Mdy_RestdayFlag
                            , Mdp_Daycode
                            , Mdp_RestDayFlag
                            FROM {0}..M_CodeDtl PREMGRP 
                            FULL OUTER JOIN {0}..M_CodeDtl PAYTYPE 
                                ON PAYTYPE.Mcd_CompanyCode = PREMGRP.Mcd_CompanyCode	
	                            AND PAYTYPE.Mcd_CodeType ='PAYTYPE'	
	                            AND PAYTYPE.Mcd_RecordStatus = 'A'
                            FULL OUTER JOIN (	SELECT Mdy_DayCode, Mdy_RestdayFlag
					                            FROM {0}..M_Day
					                            WHERE Mdy_CompanyCode = '{1}'
						                            AND Mdy_RestdayFlag = 0
						                            AND Mdy_HolidayFlag = 1
						                            AND Mdy_RecordStatus = 'A'
					                            UNION ALL
					                            SELECT Mdy_DayCode, 1 as Mdy_RestdayFlag
					                            FROM {0}..M_Day
					                            WHERE Mdy_CompanyCode = '{1}'
						                            AND Mdy_RestdayFlag = 0
						                            AND Mdy_HolidayFlag = 1
						                            AND Mdy_RecordStatus = 'A'
					                            UNION ALL
					                            SELECT Mdy_DayCode, Mdy_RestdayFlag
					                            FROM {0}..M_Day
					                            WHERE Mdy_CompanyCode = '{1}'
						                            AND Mdy_RestdayFlag = 0
						                            AND Mdy_HolidayFlag = 0
						                            AND Mdy_RecordStatus = 'A'
					                            UNION ALL
					                            SELECT Mdy_DayCode, Mdy_RestdayFlag
					                            FROM {0}..M_Day
					                            WHERE Mdy_CompanyCode = '{1}'
						                            AND Mdy_RestdayFlag = 1
						                            AND Mdy_HolidayFlag = 0
						                            AND Mdy_RecordStatus = 'A'  ) DAYMST ON  1=1
                            LEFT JOIN {0}..M_DayPremium 
                                ON Mdp_CompanyCode = ISNULL(PREMGRP.Mcd_CompanyCode,PAYTYPE.Mcd_CompanyCode)
	                            AND Mdp_PremiumGrpCode = PREMGRP.Mcd_Code
	                            AND Mdp_PayrollType = PAYTYPE.Mcd_Code
	                            AND Mdy_DayCode = Mdp_Daycode
	                            AND Mdy_RestdayFlag = Mdp_RestDayFlag
                            WHERE PREMGRP.Mcd_CompanyCode = '{1}'	
	                            AND PREMGRP.Mcd_CodeType ='PREMGRP'	
	                            AND PREMGRP.Mcd_RecordStatus = 'A'
	                            AND Mdp_PremiumGrpCode IS NULL
                            ORDER BY 1,2,3,4";
            #endregion
            query = string.Format(query, CentralProfile, CompanyCode);
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            if (dtResult.Rows.Count > 0)
                return dtResult;
            else
                return new DataTable();
        }

        public DataTable GetEmployeeSalaryinCurrentPayPeriod()
        {
            #region query
            string query = @"SELECT a.Tsl_IDNo
	                            , ROW_NUMBER() OVER (PARTITION BY a.Tsl_IDNo ORDER BY a.Tsl_Startdate) AS [Ctr]
	                            , CASE WHEN a.Tsl_StartDate < @STARTCYCLE THEN @STARTCYCLE ELSE a.Tsl_StartDate END AS Tsl_StartDate
	                            , CASE WHEN ISNULL(a.Tsl_EndDate,@ENDCYCLE) >= @ENDCYCLE THEN @ENDCYCLE ELSE a.Tsl_EndDate END AS Tsl_EndDate
	                            , a.Tsl_SalaryRate
	                            , a.Tsl_PayrollType
                            FROM {0}..T_EmpSalary a
                            INNER JOIN (SELECT Tsl_IDNo
			                            FROM {0}..T_EmpSalary 
			                            INNER JOIN M_Employee ON Mem_IDNo = Tsl_IDNo
				                            AND Mem_IsComputedPayroll = 1
			                            WHERE  ISNULL(Tsl_EndDate, @ENDCYCLE) >= @STARTCYCLE AND @ENDCYCLE >= Tsl_StartDate
			                            GROUP BY Tsl_IDNo
			                            HAVING COUNT(Tsl_IDNo) > 1 ) EmpSalary ON a.Tsl_IDNo = EmpSalary.Tsl_IDNo
                            WHERE ISNULL(a.Tsl_EndDate,@ENDCYCLE) >= @STARTCYCLE AND @ENDCYCLE >= a.Tsl_StartDate";
            #endregion
            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@STARTCYCLE", PayrollStart, SqlDbType.Date);
            paramInfo[1] = new ParameterInfo("@ENDCYCLE", PayrollEnd, SqlDbType.Date);

            query = string.Format(query, CentralProfile);
            DataTable dtResult;
            dtResult = dal.ExecuteDataSet(query, CommandType.Text, paramInfo).Tables[0];
            return dtResult;
        }

        public string GetMaxPayPeriodContribPrem(string DeductionCode)
        {
            #region query
            string query = @"SELECT MAX(Mps_PayCycle) as [PayCycle]
                           FROM {3}..M_PremiumSchedule
                           WHERE Mps_PayCycle <= '{0}'            
                                AND Mps_DeductionCode = '{1}'
                                AND Mps_CompanyCode = '{2}'
                                AND Mps_RecordStatus = 'A'";
            #endregion
            query = string.Format(query, ProcessPayrollPeriod, DeductionCode, CompanyCode, CentralProfile);
            DataTable dtResult;
            string PayPeriod = "";
            dtResult = dal.ExecuteDataSet(query).Tables[0];
            if (dtResult.Rows.Count > 0)
                PayPeriod = dtResult.Rows[0][0].ToString();
            return PayPeriod;
        }

        public DataTable GetMaxPrem(string MaxPayPeriod)
        {
            #region query
            string query = @"SELECT Mps_DeductionCode as DeductionCode
                                , MAX(Mps_CompensationFrom) as [MaxShare]
                                , MAX(Mps_PayCycle) as [MaxPayCycle]
                               FROM {2}..M_PremiumSchedule
                               WHERE Mps_PayCycle <= '{0}'            
                                    AND Mps_CompanyCode = '{1}'
                                    AND Mps_RecordStatus = 'A'
                               GROUP BY Mps_DeductionCode";
            #endregion
            query = string.Format(query, MaxPayPeriod, CompanyCode, CentralProfile);
            DataTable dtResult;
            dtResult = dal.ExecuteDataSet(query).Tables[0];
            if (dtResult.Rows.Count > 0)
                return dtResult;
            else
                return new DataTable();
        }

        public string GetMaxPayPeriodTaxScheduleHeader(string TaxSchedule, string PayCycle, string companyCode, string centralProfile, DALHelper dalhelper)
        {
            #region query
            string query = @"SELECT Max(Mth_PayCycle) as [PayCycle]
                           FROM {3}..M_TaxScheduleHdr
                           WHERE Mth_TaxSchedule = '{0}'
                                AND Mth_PayCycle <= '{1}'
                                AND Mth_CompanyCode = '{2}'
                                AND Mth_RecordStatus = 'A'";

            #endregion
            query = string.Format(query, TaxSchedule, PayCycle, companyCode, centralProfile);
            DataTable dtResult;
            string PayPeriod = "";
            dtResult = dalhelper.ExecuteDataSet(query).Tables[0];
            if (dtResult.Rows.Count > 0)
                PayPeriod = dtResult.Rows[0][0].ToString();
            return PayPeriod;
        }

        public string GetMaxPayPeriodTaxScheduleHeader(string TaxSchedule)
        {
            #region query

            string query = @"SELECT Max(Mth_PayCycle) as [PayCycle]
                           FROM {3}..M_TaxScheduleHdr
                           WHERE Mth_TaxSchedule = '{0}'
                                AND Mth_PayCycle <= '{1}'
                                AND Mth_CompanyCode = '{2}'
                                AND Mth_RecordStatus = 'A'";

            #endregion
            query = string.Format(query, TaxSchedule, ProcessPayrollPeriod, CompanyCode, CentralProfile);
            DataTable dtResult;
            string PayPeriod = "";
            dtResult = dal.ExecuteDataSet(query).Tables[0];
            if (dtResult.Rows.Count > 0)
                PayPeriod = dtResult.Rows[0][0].ToString();
            return PayPeriod;
        }

        public string GetMaxPayPeriodAnnualTaxSchedule()
        {
            #region query
            string query = @"SELECT MAX(Myt_PayCycle) as [PayCycle]
                           FROM {2}..M_YearlyTaxSchedule
                           WHERE Myt_PayCycle <= '{0}'
                                AND Myt_CompanyCode = '{1}'
                                AND Myt_RecordStatus = 'A'";
            #endregion
            query = string.Format(query, ProcessPayrollPeriod, CompanyCode, CentralProfile);
            DataTable dtResult;
            string PayPeriod = "";
            dtResult = dal.ExecuteDataSet(query).Tables[0];
            if (dtResult.Rows.Count > 0)
                PayPeriod = dtResult.Rows[0][0].ToString();
            return PayPeriod;
        }

        public string GetMaxPayPeriodTaxExemption()
        {
            #region query
            string query = @"SELECT MAX(Mte_PayCycle) as [PayCycle]
                           FROM {2}..M_TaxExemption
                           WHERE Mte_PayCycle <= '{0}'
                                AND Mte_CompanyCode = '{1}'
                                AND Mte_RecordStatus = 'A'";
            #endregion
            query = string.Format(query, ProcessPayrollPeriod, CompanyCode, CentralProfile);
            DataTable dtResult;
            string PayPeriod = "";
            dtResult = dal.ExecuteDataSet(query).Tables[0];
            if (dtResult.Rows.Count > 0)
                PayPeriod = dtResult.Rows[0][0].ToString();
            return PayPeriod;
        }

        public string GetMaxPayPeriodAnnualTaxSchedule(string PayCycle, string CompanyCode, string CentralProfile, DALHelper dalhelper)
        {
            #region query
            string query = @"SELECT MAX(Myt_PayCycle) as [PayCycle]
                           FROM {2}..M_YearlyTaxSchedule
                           WHERE Myt_PayCycle <= '{0}'
                                AND Myt_CompanyCode = '{1}'
                                AND Myt_RecordStatus = 'A'";
            #endregion
            query = string.Format(query, PayCycle, CompanyCode, CentralProfile);
            DataTable dtResult;
            string PayPeriod = "";
            dtResult = dalhelper.ExecuteDataSet(query).Tables[0];
            if (dtResult.Rows.Count > 0)
                PayPeriod = dtResult.Rows[0][0].ToString();
            return PayPeriod;
        }

        public DataTable GetAdditionalExemptionAmount()
        {
            #region query
            string query = @"SELECT Mte_TaxCode, Mte_Amount 
                              FROM {2}..M_TaxExemption 
                              WHERE Mte_TaxCode like 'D%' 
                                    AND Mte_PayCycle = '{0}' 
                                    AND Mte_CompanyCode = '{1}'
                                    AND Mte_ExemptType = 'A' 
                                    AND Mte_RecordStatus = 'A'";
            #endregion
            query = string.Format(query, MaxTaxExemption, CompanyCode, CentralProfile);
            DataTable dtResult;
            dtResult = dal.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        //public DataTable CheckPreProcessingErrorInDeductionExists(string PayPeriod, string UserLogin, DALHelper dalHelper)
        //{
        //    return CheckPreProcessingErrorInDeductionExists(PayPeriod, "", UserLogin, dalHelper);
        //}

        //public DataTable CheckPreProcessingErrorInDeductionExists(string PayPeriod, string TypeFilter, string UserLogin, DALHelper dalHelper)
        //{
        //    return CheckPreProcessingErrorInDeductionExists(PayPeriod, "", TypeFilter,"", UserLogin, dalHelper);
        //}

        //public DataTable CheckPreProcessingErrorInDeductionExists(string PayPeriod, string EmployeeID, string TypeFilter, string CostCenter, string UserLogin, DALHelper dalHelper)
        //{
        //    #region Query
        //    string condition = string.Empty;
        //    if (CostCenter != string.Empty)
        //    {
        //        condition += string.Format("And Mem_CostcenterCode IN ({0})", CostCenter);
        //    }
        //    string query = string.Format(@"
        //        --VARIABLE DECLARATION AND INITIALIZATION
        //        DECLARE @Type VARCHAR(10) = '{2}'
        //        DECLARE @EmployeeID VARCHAR(15) = '{1}'
        //        DECLARE @PayrollPeriod VARCHAR(7)
        //        DECLARE @PayrollCycle CHAR(1)
        //        DECLARE @EndPayPeriod DATETIME
        //        DECLARE @PayrollRate VARCHAR(10) = 'B' --BASIC RATE
        //        DECLARE @MMaxRate DECIMAL(18, 2) = 0
        //        DECLARE @DMaxRate DECIMAL(18, 2) = 0
        //        DECLARE @HMaxRate DECIMAL(18, 2) = 0 
        //        DECLARE @MMinRate DECIMAL(18, 2) = 0
        //        DECLARE @DMinRate DECIMAL(18, 2) = 0
        //        DECLARE @HMinRate DECIMAL(18, 2) = 0 
        //        DECLARE @CompTaxSched CHAR(1) = 'S' --SEMI-MONTHLY
        //        DECLARE @TaxApplicPayPeriod VARCHAR(7)
        //        DECLARE @SSSApplicPayPeriod VARCHAR(7)
        //        DECLARE @PhApplicPayPeriod VARCHAR(7)

        //        SELECT @PayrollCycle = SUBSTRING(Tps_PayCycle, 7, 1)
		      //          , @PayrollPeriod = Tps_PayCycle
		      //          , @EndPayPeriod = Tps_EndCycle
        //        FROM T_PaySchedule
        //        WHERE Tps_PayCycle = '{0}'

        //        SELECT @MMaxRate = Mpd_ParamValue
        //        FROM M_PolicyDtl
        //        WHERE Mpd_PolicyCode = 'MAXRATE'
	       //         AND Mpd_SubCode = 'MMAXRATE'
                	
        //        SELECT @DMaxRate = Mpd_ParamValue
        //        FROM M_PolicyDtl
        //        WHERE Mpd_PolicyCode = 'MAXRATE'
	       //         AND Mpd_SubCode = 'DMAXRATE'
                	
        //        SELECT @HMaxRate = Mpd_ParamValue
        //        FROM M_PolicyDtl
        //        WHERE Mpd_PolicyCode = 'MAXRATE'
	       //         AND Mpd_SubCode = 'HMAXRATE'
                	
        //        SELECT @MMINRate = Mpd_ParamValue
        //        FROM M_PolicyDtl
        //        WHERE Mpd_PolicyCode = 'MINRATE'
	       //         AND Mpd_SubCode = 'MMINRATE'
                	
        //        SELECT @DMINRate = Mpd_ParamValue
        //        FROM M_PolicyDtl
        //        WHERE Mpd_PolicyCode = 'MINRATE'
	       //         AND Mpd_SubCode = 'DMINRATE'
                	
        //        SELECT @HMINRate = Mpd_ParamValue
        //        FROM M_PolicyDtl
        //        WHERE Mpd_PolicyCode = 'MINRATE'
	       //         AND Mpd_SubCode = 'HMINRATE'

        //        SELECT @CompTaxSched = CASE WHEN Mcm_TaxRule IN (3, 4, 5) AND @PayrollCycle = 2
						  //              THEN 'M'
						  //              ELSE 'S'
						  //              END
        //        FROM M_Company
        //        INNER JOIN M_CodeDtl ON Mcd_Code = Mcm_TaxRule
        //            AND Mcd_CodeType = 'TAXRULE'
        //            AND Mcd_RecordStatus = 'A'
                    
        //        SELECT @TaxApplicPayPeriod = MAX(Mth_PayCycle)
        //        FROM {0}..M_TaxScheduleHdr
        //        WHERE Mth_TaxSchedule = @CompTaxSched
        //            and Mth_PayCycle <= @PayrollPeriod
        //            and Mth_RecordStatus = 'A'
                    
        //        SELECT @SSSApplicPayPeriod = MAX(Mps_PayCycle)
        //        FROM {0}..M_PremiumSchedule
        //        WHERE Mps_PayCycle <= @PayrollPeriod           
	       //         and Mps_DeductionCode = 'SSSPREM'
	       //         and Mps_RecordStatus = 'A'
                	
        //        SELECT @PhApplicPayPeriod = MAX(Mps_PayCycle)
        //        FROM {0}..M_PremiumSchedule
        //        WHERE Mps_PayCycle <= @PayrollPeriod           
	       //         and Mps_DeductionCode = 'PHICPREM'
	       //         and Mps_RecordStatus = 'A'

        //        SELECT ROW_NUMBER() OVER (ORDER BY [Remarks], [Last Name], [First Name]) AS Row
	       //         , [Type]
	       //         , [Employee ID]
	       //         , [Last Name]
	       //         , [First Name]
	       //         , [Deduction Code]
	       //         , [Start Date]
	       //         , [Remarks]
	       //         , [Deduction Amount]
	       //         , [Paid Amount]
	       //         , [Deferred Amount]
	       //         , [Details]
        //        FROM (
        //        SELECT * FROM ( 
        //        -----------------------------------------
        //        --SELECT 'DEDUCTION MESSAGES'
        //        -----------------------------------------
        //        --1. Paid + Deferred Amount > Deduction Amount
        //        SELECT 'WARNING' as [Type] 
	       //         ,Mem_IDNo as [Employee ID]
	       //         ,Mem_LastName as [Last Name]                   
	       //         ,Mem_FirstName as [First Name]
	       //         ,Tdh_DeductionCode as [Deduction Code]
	       //         ,Tdh_StartDate as [Start Date]     
	       //         ,'Paid + Deferred Amount > Deduction Amount' as [Remarks]      
	       //         ,Tdh_DeductionAmount as [Deduction Amount]
	       //         ,Tdh_PaidAmount as [Paid Amount]
	       //         ,Tdh_CycleAmount as [Amortization]
	       //         ,Tdh_DeferredAmount as [Deferred Amount]
	       //         ,0 as [Details]
        //        FROM T_EmpDeductionHdr
        //        INNER JOIN M_Employee ON Mem_IDNo = Tdh_IDNo
        //        @USERCOSTCENTERACCESSCONDITION
        //        INNER JOIN (
        //            SELECT Mpd_SubCode 
        //            FROM M_PolicyDtl 
        //            WHERE Mpd_PolicyCode = 'EMPSTATPAY'
        //                AND Mpd_ParamValue = 1
        //        ) EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
        //        WHERE Tdh_PaidAmount  + Tdh_DeferredAmount > Tdh_DeductionAmount  
        //            AND Mem_IsComputedPayroll = 1
        //            AND LEFT(Mem_WorkStatus, 1) <> 'I'
        //            AND (@EmployeeID = '' OR Mem_IDNo = @EmployeeID)
        //        @CONDITIONS

        //        UNION

        //        --2. Amortization Amount > Deduction Amount
        //        SELECT 'WARNING' as [Type] 
	       //         ,Mem_IDNo  
	       //         ,Mem_LastName                   
	       //         ,Mem_FirstName  
	       //         ,Tdh_DeductionCode    
	       //         ,Tdh_StartDate    
	       //         ,remarks = ('Amortization Amount > Deduction Amount')        
	       //         ,Tdh_DeductionAmount
	       //         ,Tdh_PaidAmount
	       //         ,Tdh_CycleAmount
	       //         ,Tdh_DeferredAmount
	       //         ,DeferredDetails = 0.00
        //        FROM T_EmpDeductionHdr
        //        INNER JOIN M_Employee ON Mem_IDNo = Tdh_IDNo
        //        @USERCOSTCENTERACCESSCONDITION
        //        INNER JOIN (
        //            SELECT Mpd_SubCode 
        //            FROM M_PolicyDtl 
        //            WHERE Mpd_PolicyCode = 'EMPSTATPAY'
        //                AND Mpd_ParamValue = 1
        //        ) EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
        //        WHERE Tdh_CycleAmount > Tdh_DeductionAmount
        //            AND Mem_IsComputedPayroll = 1
        //            AND LEFT(Mem_WorkStatus, 1) <> 'I'
        //            AND (@EmployeeID = '' OR Mem_IDNo = @EmployeeID)
        //        @CONDITIONS

        //        UNION

        //        --3. Already Fully-paid but with Deferred Amount
        //        SELECT 'WARNING' as [Type] 
	       //         ,Mem_IDNo  
	       //         ,Mem_LastName                   
	       //         ,Mem_FirstName  
	       //         ,Tdh_DeductionCode    
	       //         ,Tdh_StartDate  
	       //         ,remarks = ('Already Fully-paid but with Deferred Amount')          
	       //         ,Tdh_DeductionAmount
	       //         ,Tdh_PaidAmount
	       //         ,Tdh_CycleAmount
	       //         ,Tdh_DeferredAmount
	       //         ,DeferredDetails = 0.00
        //        FROM T_EmpDeductionHdr
        //        INNER JOIN M_Employee ON Mem_IDNo = Tdh_IDNo
        //        @USERCOSTCENTERACCESSCONDITION
        //        INNER JOIN (
        //            SELECT Mpd_SubCode 
        //            FROM M_PolicyDtl 
        //            WHERE Mpd_PolicyCode = 'EMPSTATPAY'
        //                AND Mpd_ParamValue = 1
        //        ) EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
        //        WHERE  Tdh_DeductionAmount = Tdh_PaidAmount AND Tdh_DeferredAmount > 0
        //            AND Mem_IsComputedPayroll = 1
        //            AND LEFT(Mem_WorkStatus, 1) <> 'I'
        //            AND (@EmployeeID = '' OR Mem_IDNo = @EmployeeID)
        //        @CONDITIONS

        //        UNION

        //        --4. Deduction Amount is zero or negative
        //        SELECT 'WARNING' as [Type] 
	       //         ,Mem_IDNo  
	       //         ,Mem_LastName                   
	       //         ,Mem_FirstName  
	       //         ,Tdh_DeductionCode    
	       //         ,Tdh_StartDate  
	       //         ,remarks =  ('Deduction Amount is Zero or Negative')          
	       //         ,Tdh_DeductionAmount
	       //         ,Tdh_PaidAmount
	       //         ,Tdh_CycleAmount
	       //         ,Tdh_DeferredAmount
	       //         ,DeferredDetails = 0.00
        //        FROM T_EmpDeductionHdr
        //        INNER JOIN M_Employee ON Mem_IDNo = Tdh_IDNo
        //        @USERCOSTCENTERACCESSCONDITION
        //        INNER JOIN (
        //            SELECT Mpd_SubCode 
        //            FROM M_PolicyDtl 
        //            WHERE Mpd_PolicyCode = 'EMPSTATPAY'
        //                AND Mpd_ParamValue = 1
        //        ) EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
        //        WHERE Tdh_DeductionAmount <= 0
        //            AND Mem_IsComputedPayroll = 1
        //            AND LEFT(Mem_WorkStatus, 1) <> 'I'
        //            AND (@EmployeeID = '' OR Mem_IDNo = @EmployeeID)
        //        @CONDITIONS

        //        UNION

        //        --5. Paid Amount is negative
        //        SELECT 'WARNING' as [Type] 
	       //         ,Mem_IDNo  
	       //         ,Mem_LastName                   
	       //         ,Mem_FirstName  
	       //         ,Tdh_DeductionCode    
	       //         ,Tdh_StartDate   
	       //         ,remarks =  ('Paid Amount is Negative')         
	       //         ,Tdh_DeductionAmount
	       //         ,Tdh_PaidAmount
	       //         ,Tdh_CycleAmount
	       //         ,Tdh_DeferredAmount
	       //         ,DeferredDetails = 0.00
        //        FROM T_EmpDeductionHdr
        //        INNER JOIN M_Employee ON Mem_IDNo = Tdh_IDNo
        //        @USERCOSTCENTERACCESSCONDITION
        //        INNER JOIN (
        //            SELECT Mpd_SubCode 
        //            FROM M_PolicyDtl 
        //            WHERE Mpd_PolicyCode = 'EMPSTATPAY'
        //                AND Mpd_ParamValue = 1
        //        ) EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
        //        WHERE Tdh_PaidAmount < 0
        //            AND Mem_IsComputedPayroll = 1
        //            AND LEFT(Mem_WorkStatus, 1) <> 'I'
        //            AND (@EmployeeID = '' OR Mem_IDNo = @EmployeeID)
        //        @CONDITIONS

        //        UNION

        //        --6. Amortization Amount is negative
        //        SELECT 'WARNING' as [Type] 
	       //         ,Mem_IDNo  
	       //         ,Mem_LastName                   
	       //         ,Mem_FirstName  
	       //         ,Tdh_DeductionCode    
	       //         ,Tdh_StartDate  
	       //         ,remarks =  ('Amortization Amount is Negative')          
	       //         ,Tdh_DeductionAmount
	       //         ,Tdh_PaidAmount
	       //         ,Tdh_CycleAmount
	       //         ,Tdh_DeferredAmount
	       //         ,DeferredDetails = 0.00
        //        FROM T_EmpDeductionHdr
        //        INNER JOIN M_Employee ON Mem_IDNo = Tdh_IDNo
        //        @USERCOSTCENTERACCESSCONDITION
        //        INNER JOIN (
        //            SELECT Mpd_SubCode 
        //            FROM M_PolicyDtl 
        //            WHERE Mpd_PolicyCode = 'EMPSTATPAY'
        //                AND Mpd_ParamValue = 1
        //        ) EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
        //        WHERE Tdh_CycleAmount < 0
        //            AND Mem_IsComputedPayroll = 1
        //            AND LEFT(Mem_WorkStatus, 1) <> 'I'
        //            AND (@EmployeeID = '' OR Mem_IDNo = @EmployeeID)
        //        @CONDITIONS

        //        UNION

        //        --7. Deferred Amount is negative
        //        SELECT 'WARNING' as [Type] 
	       //         ,Mem_IDNo  
	       //         ,Mem_LastName                   
	       //         ,Mem_FirstName  
	       //         ,Tdh_DeductionCode    
	       //         ,Tdh_StartDate    
	       //         ,remarks =  ('Deferred Amount is Negative')        
	       //         ,Tdh_DeductionAmount
	       //         ,Tdh_PaidAmount
	       //         ,Tdh_CycleAmount
	       //         ,Tdh_DeferredAmount
	       //         ,DeferredDetails = 0.00
        //        FROM T_EmpDeductionHdr
        //        INNER JOIN M_Employee ON Mem_IDNo = Tdh_IDNo
        //        @USERCOSTCENTERACCESSCONDITION
        //        INNER JOIN (
        //            SELECT Mpd_SubCode 
        //            FROM M_PolicyDtl 
        //            WHERE Mpd_PolicyCode = 'EMPSTATPAY'
        //                AND Mpd_ParamValue = 1
        //        ) EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
        //        WHERE  Tdh_DeferredAmount < 0
        //            AND Mem_IsComputedPayroll = 1
        //            AND LEFT(Mem_WorkStatus, 1) <> 'I'
        //            AND (@EmployeeID = '' OR Mem_IDNo = @EmployeeID)
        //        @CONDITIONS

        //        UNION

        //        --8. Deferred Amount in Ledger AND Detail not balance
        //        SELECT 'WARNING' as [Type] 
	       //         ,Mem_IDNo  
	       //         ,Mem_LastName                   
	       //         ,Mem_FirstName      
	       //         ,Tdh_DeductionCode
	       //         ,Tdh_StartDate   
	       //         ,remarks = 'Deferred Amount in Ledger AND Detail is Not Balanced'         
	       //         ,Tdh_DeductionAmount
	       //         ,Tdh_PaidAmount
	       //         ,Tdh_CycleAmount
	       //         ,Tdh_DeferredAmount
	       //         ,DeferredDetails = Amt
        //        FROM T_EmpDeductionHdr
        //        INNER JOIN M_Employee ON Mem_IDNo = Tdh_IDNo
        //        @USERCOSTCENTERACCESSCONDITION
        //        INNER JOIN (
        //            SELECT Mpd_SubCode 
        //            FROM M_PolicyDtl 
        //            WHERE Mpd_PolicyCode = 'EMPSTATPAY'
        //                AND Mpd_ParamValue = 1
        //        ) EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
        //        LEFT JOIN (SELECT Tdd_IDNo
				    //            ,Tdd_DeductionCode
				    //            ,Tdd_StartDate
				    //            ,Amt = SUM(Tdd_DeferredAmt)
			     //           FROM T_EmpDeductionDefer
			     //           GROUP BY Tdd_IDNo
				    //            ,Tdd_DeductionCode
				    //            ,Tdd_StartDate) Defer ON Defer.Tdd_IDNo = Tdh_IDNo
	       //                                         AND Defer.Tdd_DeductionCode = Tdh_DeductionCode
	       //                                         AND Defer.Tdd_StartDate =  Tdh_StartDate
        //        WHERE (ISNULL(Tdh_DeferredAmount,0) <> ISNULL(Defer.Amt,0))
        //            AND Mem_IsComputedPayroll = 1
        //            AND LEFT(Mem_WorkStatus, 1) <> 'I'
        //            AND (@EmployeeID = '' OR Mem_IDNo = @EmployeeID)
        //        @CONDITIONS

        //        UNION

        //        --9.Paid Amount in Ledger AND Detail not balance
        //        SELECT 'WARNING' as [Type] 
	       //         ,Mem_IDNo  
	       //         ,Mem_LastName                   
	       //         ,Mem_FirstName      
	       //         ,Tdh_DeductionCode
	       //         ,Tdh_StartDate 
	       //         ,remarks = 'Paid Amount in Ledger AND Detail is Not Balanced'           
	       //         ,Tdh_DeductionAmount
	       //         ,Tdh_PaidAmount
	       //         ,Tdh_CycleAmount
	       //         ,Tdh_DeferredAmount
	       //         ,DeferredDetails = Amt
        //        FROM T_EmpDeductionHdr
        //        INNER JOIN M_Employee ON Mem_IDNo = Tdh_IDNo
        //        @USERCOSTCENTERACCESSCONDITION
        //        INNER JOIN (
        //            SELECT Mpd_SubCode 
        //            FROM M_PolicyDtl 
        //            WHERE Mpd_PolicyCode = 'EMPSTATPAY'
        //                AND Mpd_ParamValue = 1
        //        ) EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
        //        LEFT JOIN (SELECT Tdd_IDNo
				    //            ,Tdd_DeductionCode
				    //            ,Tdd_StartDate
				    //            ,Amt = SUM(Tdd_Amount)
			     //           FROM T_EmpDeductionDtlHst
			     //           WHERE Tdd_PaymentFlag = 1
			     //           GROUP BY Tdd_IDNo
				    //            ,Tdd_DeductionCode
				    //            ,Tdd_StartDate) Paid ON Paid.Tdd_IDNo = Tdh_IDNo
	       //         AND Paid.Tdd_DeductionCode = Tdh_DeductionCode
	       //         AND Paid.Tdd_StartDate =  Tdh_StartDate
        //        WHERE (ISNULL(Tdh_PaidAmount,0) <> ISNULL(Paid.Amt,0))
        //            AND Mem_IsComputedPayroll = 1
        //            AND LEFT(Mem_WorkStatus, 1) <> 'I'
        //            AND (@EmployeeID = '' OR Mem_IDNo = @EmployeeID)
        //        @CONDITIONS

        //        UNION

        //        --10.With Same Deduction Record
        //        SELECT 'ERROR' as [Type] 
	       //         ,Tdh_IDNo  
	       //         ,Mem_LastName                   
	       //         ,Mem_FirstName      
	       //         ,Tdh_DeductionCode
	       //         ,Tdh_StartDate   
	       //         ,remarks = 'With Same Deduction Record'          
	       //         ,Tdh_DeductionAmount = 0
	       //         ,Tdh_PaidAmount = 0
	       //         ,Tdh_CycleAmount = 0
	       //         ,Tdh_DeferredAmount = 0
	       //         ,DeferredDetails = 0
        //        FROM (
	       //         SELECT Tdh_IDNo 
			     //           ,Mem_LastName
			     //           ,Mem_FirstName 
			     //           ,Tdh_DeductionCode 
			     //           ,Convert(varchar, MAX(Tdh_StartDate), 101) as Tdh_StartDate
	       //         FROM T_EmpDeductionHdr
	       //         INNER JOIN M_Deduction
	       //         ON Tdh_DeductionCode = Mdn_DeductionCode
		      //          AND Mdn_DeductionGroup IN ('LP','LS')
	       //         INNER JOIN M_Employee
        //            @USERCOSTCENTERACCESSCONDITION ON Tdh_IDNo = Mem_IDNo
		      //          AND Mem_IsComputedPayroll = 1
        //                AND LEFT(Mem_WorkStatus, 1) <> 'I'
        //                AND (@EmployeeID = '' OR Mem_IDNo = @EmployeeID)
        //            INNER JOIN (
        //                SELECT Mpd_SubCode 
        //                FROM M_PolicyDtl 
        //                WHERE Mpd_PolicyCode = 'EMPSTATPAY'
        //                    AND Mpd_ParamValue = 1
        //            ) EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
	       //         WHERE Tdh_StartDate <= @EndPayPeriod
		      //          AND Tdh_DeferredAmount + Tdh_PaidAmount < Tdh_DeductionAmount
		      //          AND Tdh_CycleAmount > 0
		      //          AND Tdh_ExemptInPayroll = 0
        //                AND Mem_IsComputedPayroll = 1
        //            @CONDITIONS
	       //         GROUP BY Tdh_IDNo, Mem_LastName, Mem_FirstName, Tdh_DeductionCode
	       //         HAVING COUNT(Tdh_IDNo) > 1
        //        ) temp

        //        ) TEMP
        //        WHERE (@Type = '' OR [Type] = @Type)
        //        ) TEMP2
        //        ORDER BY [Last Name], [First Name]
        //        ", PayPeriod, EmployeeID, TypeFilter);
        //    query = query.Replace("@CONDITIONS", condition);
        //    query = query.Replace("@USERCOSTCENTERACCESSCONDITION", (new HRCReportsBL()).UserCostCenterAccessTmpQuery(LoginInfo.getUser().DBNumber, "PAYROLL", UserLogin, "Mem_CostcenterCode", "Mem_WorkStatus", "Mem_EmploymentStatusCode", "Mem_PayrollType", CompanyCode, CentralProfile, false));
        //    #endregion
        //    DataTable dtResult;
        //    dtResult = dalHelper.ExecuteDataSet(query).Tables[0];
        //    return dtResult;
        //}


        public void CheckPreProcessingInPayrollExists(string ProfileCode, string centralProfile, string PayCycle, string EmployeeID, DateTime dtPayPeriodStart, DateTime dtPayPeriodEnd, string CostCenter, string UserLogin, string companyCode, string menucode, DALHelper dalHelper)
        {
            #region Query
            string condition = string.Empty;
            if (CostCenter != string.Empty)
            {
                condition += string.Format("And Mem_CostcenterCode IN ({0})", (new CommonBL()).EncodeFilterItems(CostCenter, true));
            }
            string query = string.Format(@"
            DECLARE @MMaxRate DECIMAL(18, 2) = 0
            DECLARE @DMaxRate DECIMAL(18, 2) = 0
            DECLARE @MMinRate DECIMAL(18, 2) = 0
            DECLARE @DMinRate DECIMAL(18, 2) = 0
            DECLARE @CompTaxSched CHAR(1)
            DECLARE @TaxApplicPayPeriod VARCHAR(7)
            DECLARE @SSSApplicPayPeriod VARCHAR(7)
            DECLARE @PhApplicPayPeriod VARCHAR(7)
            DECLARE @UNIONDUES BIT 
            DECLARE @MenuCode VARCHAR(20) = '{7}'
           
            SELECT @MMaxRate = Mpd_ParamValue
            FROM {0}..M_PolicyDtl
            WHERE Mpd_PolicyCode = 'MAXRATE'
	            AND Mpd_SubCode = 'M'
                AND Mpd_CompanyCode = '{1}'

            SELECT @DMaxRate = Mpd_ParamValue
            FROM {0}..M_PolicyDtl
            WHERE Mpd_PolicyCode = 'MAXRATE'
	            AND Mpd_SubCode = 'D'
                AND Mpd_CompanyCode = '{1}'

            SELECT @MMINRate = Mpd_ParamValue
            FROM {0}..M_PolicyDtl
            WHERE Mpd_PolicyCode = 'MINRATE'
	            AND Mpd_SubCode = 'M'
                AND Mpd_CompanyCode = '{1}'            	

            SELECT @DMINRate = Mpd_ParamValue
            FROM {0}..M_PolicyDtl
            WHERE Mpd_PolicyCode = 'MINRATE'
	            AND Mpd_SubCode = 'D'
                AND Mpd_CompanyCode = '{1}' 

            SELECT @SSSApplicPayPeriod = MAX(Mps_PayCycle)
                           FROM {0}..M_PremiumSchedule
                           WHERE Mps_PayCycle <= '{3}'            
                                AND Mps_DeductionCode = 'SSSPREM'
                                AND Mps_CompanyCode = '{1}'
                                AND Mps_RecordStatus = 'A' 

            SELECT @PhApplicPayPeriod = MAX(Mps_PayCycle) 
                FROM {0}..M_PremiumSchedule
                    WHERE Mps_PayCycle <= '{3}'             
	                AND Mps_DeductionCode = 'PHICPREM'
                    AND Mps_CompanyCode = '{1}'
	                AND Mps_RecordStatus = 'A'

           SELECT @CompTaxSched = Tps_TaxSchedule
                            FROM T_PaySchedule
                            WHERE Tps_PayCycle = '{3}'

          SELECT @TaxApplicPayPeriod = MAX(Mth_PayCycle)
            FROM {0}..M_TaxScheduleHdr
            WHERE Mth_TaxSchedule = @CompTaxSched
                AND Mth_PayCycle <= '{3}'
                AND Mth_CompanyCode = '{1}'
                AND Mth_RecordStatus = 'A'  

        SELECT @UNIONDUES = CASE WHEN Mph_CharValue = 'TRUE' THEN 1 ELSE 0 END 
                    FROM M_PolicyHdr 
                WHERE Mph_CompanyCode = '{1}' 
                AND Mph_PolicyCode = 'UNIONDUES'

         DELETE FROM T_EmpProcessCheck WHERE Tpc_ModuleCode = @MenuCode AND Tpc_SystemDefined = 1
            AND Tpc_PayCycle = '{3}'
            AND ('{2}' = '' OR Tpc_IDNo = '{2}')

         INSERT INTO T_EmpProcessCheck

         SELECT @MenuCode
                ,Mem_IDNo
                ,'BW' 
                ,'{3}'
	            , Mem_Salary 
	            , @MMaxRate 
	            ,'Employee Salary is more than Monthlies Maximum Salary' 
                , 1
	            ,'{4}'
	            , GETDATE()
            FROM M_Employee 
            @USERCOSTCENTERACCESSCONDITION
            INNER JOIN (SELECT Mpd_SubCode 
			            FROM M_PolicyDtl 
			            WHERE Mpd_PolicyCode = 'EMPSTATPAY'
				            AND Mpd_ParamValue = 1
                            AND Mpd_CompanyCode = '{1}') EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
            WHERE Mem_IsComputedPayroll = 1 
	            AND @MMaxRate > 0
	            AND Mem_Salary > @MMaxRate
	            AND Mem_PayrollType = 'M'
                AND ('{2}' = '' OR Mem_IDNo = '{2}')
	            @CONDITIONS	

            UNION ALL

            SELECT @MenuCode
                ,Mem_IDNo
	            ,'BW' 
	            ,'{3}'
	            , Mem_Salary 
	            , @MMinRate 
	            ,'Employee Salary is less than Monthlies Minimum Salary' 
	            , 1
                ,'{4}'
	            , GETDATE()
            FROM M_Employee 
            @USERCOSTCENTERACCESSCONDITION
            INNER JOIN (SELECT Mpd_SubCode 
			            FROM M_PolicyDtl 
			            WHERE Mpd_PolicyCode = 'EMPSTATPAY'
				            AND Mpd_ParamValue = 1
                            AND Mpd_CompanyCode = '{1}') EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
            WHERE Mem_IsComputedPayroll = 1 
	            AND @MMinRate > 0
	            AND Mem_Salary < @MMinRate
	            AND Mem_PayrollType = 'M'
	            AND ('{2}' = '' OR Mem_IDNo = '{2}')
	            @CONDITIONS
            		
            UNION ALL

            SELECT @MenuCode
                ,Mem_IDNo
                ,'BW' 
                ,'{3}'
	            , Mem_Salary 
	            , @DMaxRate 
	            ,'Employee Salary is more than Dailies Maximum Salary' 
	            , 1
                ,'{4}'
	            , GETDATE()
            FROM M_Employee 
            @USERCOSTCENTERACCESSCONDITION
            INNER JOIN (SELECT Mpd_SubCode 
			            FROM M_PolicyDtl 
			            WHERE Mpd_PolicyCode = 'EMPSTATPAY'
				            AND Mpd_ParamValue = 1
                            AND Mpd_CompanyCode = '{1}') EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
            WHERE Mem_IsComputedPayroll = 1 
	            AND @DMaxRate > 0
	            AND Mem_Salary > @DMaxRate
	            AND Mem_PayrollType = 'D'
	            AND ('{2}' = '' OR Mem_IDNo = '{2}')
	            @CONDITIONS	

            UNION ALL

            SELECT @MenuCode
                ,Mem_IDNo
                ,'BW' 
                ,'{3}'
	            , Mem_Salary 
	            , @DMinRate 
	            ,'Employee Salary is less than Dailies Minimum Salary' 
	            , 1
                ,'{4}'
	            , GETDATE()
            FROM M_Employee 
            @USERCOSTCENTERACCESSCONDITION
            INNER JOIN (SELECT Mpd_SubCode 
			            FROM M_PolicyDtl 
			            WHERE Mpd_PolicyCode = 'EMPSTATPAY'
				            AND Mpd_ParamValue = 1
                            AND Mpd_CompanyCode = '{1}') EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
            WHERE Mem_IsComputedPayroll = 1 
	            AND @DMinRate > 0
	            AND Mem_Salary < @DMinRate
	            AND Mem_PayrollType = 'D'
	            AND ('{2}' = '' OR Mem_IDNo = '{2}')
	            @CONDITIONS

            UNION ALL

            SELECT @MenuCode
                ,Mem_IDNo
                ,'BW' 
                ,'{3}'
	            ,Mem_Salary 
	            , NULL 
	            ,'Employee Salary is zero or negative' 
	            , 1
                ,'{4}'
	            , GETDATE()
            FROM M_Employee 
            @USERCOSTCENTERACCESSCONDITION
            INNER JOIN (SELECT Mpd_SubCode 
			            FROM M_PolicyDtl 
			            WHERE Mpd_PolicyCode = 'EMPSTATPAY'
				            AND Mpd_ParamValue = 1
                            AND Mpd_CompanyCode = '{1}') EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
            WHERE Mem_IsComputedPayroll = 1 
	            AND Mem_Salary <= 0
	            AND ('{2}' = '' OR Mem_IDNo = '{2}')
	            @CONDITIONS

            UNION ALL

            SELECT @MenuCode
                ,Mem_IDNo
                ,'BW' 
                ,'{3}'
	            , NULL 
	            , NULL 
	            , CASE WHEN LEN(RTRIM(Mem_TaxCode)) = 0  
			                        THEN 'No Tax Code'
			                        ELSE 'Tax Code not in Tax Table' 
		                         END  
	            , 1
                ,'{4}'
	            , GETDATE()
            FROM M_Employee 
            @USERCOSTCENTERACCESSCONDITION
            INNER JOIN (SELECT Mpd_SubCode 
			            FROM M_PolicyDtl 
			            WHERE Mpd_PolicyCode = 'EMPSTATPAY'
				            AND Mpd_ParamValue = 1
                            AND Mpd_CompanyCode = '{1}') EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
            LEFT JOIN (SELECT DISTINCT Mtd_TaxCode
			            from {0}..M_TaxScheduleDtl
			            WHERE Mtd_TaxSchedule = @CompTaxSched AND
			            Mtd_PayCycle = @TaxApplicPayPeriod AND 
                        Mtd_CompanyCode = '{1}' AND
                        Mtd_RecordStatus = 'A') TaxSched ON Mtd_TaxCode = CASE WHEN LEFT('{3}',4) < 2018 THEN Mem_TaxCode ELSE 'ALL' END  
            WHERE Mem_IsComputedPayroll = 1 
	            AND ((LEFT('{3}',4) < 2018 AND LEN(RTRIM(Mem_TaxCode)) = 0) OR Mtd_TaxCode IS NULL) 
                AND @CompTaxSched <> 'Y'
                @CONDITIONS

            UNION ALL

            SELECT @MenuCode
                ,Mem_IDNo
                ,'BW' 
                ,'{3}'
	            ,Mem_SSSShare 
	            , NULL 
	            ,'SSS EE share not in Contribution Table'  
	            , 1
                ,'{4}'
	            , GETDATE()
            FROM M_Employee 
            @USERCOSTCENTERACCESSCONDITION
            INNER JOIN (SELECT Mpd_SubCode 
			            FROM M_PolicyDtl 
			            WHERE Mpd_PolicyCode = 'EMPSTATPAY'
				            AND Mpd_ParamValue = 1
                            AND Mpd_CompanyCode = '{1}') EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
            LEFT JOIN {0}..M_PremiumSchedule ON Mps_EEShare = Mem_SSSShare AND 
	            Mps_DeductionCode = 'SSSPREM'  AND 
	            Mps_PayCycle = @SSSApplicPayPeriod  AND
                Mps_CompanyCode = '{1}' AND 
	            Mps_RecordStatus = 'A'
            WHERE Mem_IsComputedPayroll = 1 
               AND (Mem_SSSRule = 'F' AND Mps_EEShare is null)
	            AND ('{2}' = '' OR Mem_IDNo = '{2}')
	            @CONDITIONS

            UNION ALL

            SELECT @MenuCode
                    ,Mem_IDNo
                    ,'BW' 
                    ,'{3}'
		            , NULL 
		            , NULL 
                    ,'Invalid SSS Premium Rule'  
                    , 1
		            ,'{4}'
		            , GETDATE()
            FROM M_Employee
            @USERCOSTCENTERACCESSCONDITION
            INNER JOIN (SELECT Mpd_SubCode 
			            FROM M_PolicyDtl 
			            WHERE Mpd_PolicyCode = 'EMPSTATPAY'
				            AND Mpd_ParamValue = 1
                            AND Mpd_CompanyCode = '{1}') EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
            LEFT JOIN {0}..M_CodeDtl on Mcd_Code = Mem_SSSRule
	            AND Mcd_CodeType = 'PREMRULE'
                AND Mcd_CompanyCode = '{1}'
            WHERE Mem_IsComputedPayroll = 1 
                AND (LEN(RTRIM(Mem_SSSRule)) = 0 or Mcd_Code is null)
                AND ('{2}' = '' OR Mem_IDNo = '{2}')
	            @CONDITIONS

            UNION ALL

            SELECT @MenuCode
                    ,Mem_IDNo
                    ,'BW' 
                    ,'{3}'
		            , NULL 
		            , NULL 
                    ,'Invalid PHIC Premium Rule'  
                    , 1
		            ,'{4}'
		            , GETDATE()
            FROM M_Employee
            @USERCOSTCENTERACCESSCONDITION
            INNER JOIN (SELECT Mpd_SubCode 
			            FROM M_PolicyDtl 
			            WHERE Mpd_PolicyCode = 'EMPSTATPAY'
				            AND Mpd_ParamValue = 1
                            AND Mpd_CompanyCode = '{1}') EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
            LEFT JOIN {0}..M_CodeDtl on Mcd_Code = Mem_PHRule
	            AND Mcd_CodeType = 'PREMRULE'
                AND Mcd_CompanyCode = '{1}'
            WHERE Mem_IsComputedPayroll = 1 
                AND (LEN(RTRIM(Mem_PHRule)) = 0 or Mcd_Code is null)
                AND ('{2}' = '' OR Mem_IDNo = '{2}')
	            @CONDITIONS

            UNION ALL

            SELECT @MenuCode
                ,Mem_IDNo
	            ,'BW' 
	            ,'{3}'
	            ,Mem_PHShare 
	            ,NULL 
	            ,'PHIC EE share not in Contribution Table'  
                , 1
	            ,'{4}'
	            ,GETDATE()
            FROM M_Employee 
            @USERCOSTCENTERACCESSCONDITION
            INNER JOIN (SELECT Mpd_SubCode 
			            FROM M_PolicyDtl 
			            WHERE Mpd_PolicyCode = 'EMPSTATPAY'
				            AND Mpd_ParamValue = 1
                            AND Mpd_CompanyCode = '{1}') EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
            LEFT JOIN {0}..M_PremiumSchedule ON Mps_EEShare = Mem_PHShare AND 
	            Mps_DeductionCode = 'PHPREM'  AND 
	            Mps_PayCycle = @PhApplicPayPeriod  AND
                Mps_CompanyCode = '{1}' AND  
	            Mps_RecordStatus = 'A'
            WHERE Mem_IsComputedPayroll = 1 
               AND  (Mem_PHRule = 'F' AND Mps_EEShare is null)
	            AND ('{2}' = '' OR Mem_IDNo = '{2}')
	            @CONDITIONS

            UNION ALL

            SELECT  @MenuCode
                 , Mem_IDNo
                 , 'BW'
                 , '{3}'
                 , Mem_PHShare
                 , NULL
                 ,'PHIC EE share is zero'
                 , 1
                 , '{4}'
                 , GETDATE()
                FROM M_Employee
                @USERCOSTCENTERACCESSCONDITION
                INNER JOIN (SELECT Mpd_SubCode
                               FROM M_PolicyDtl
                               WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                                    AND Mpd_ParamValue = 1
                                    AND Mpd_CompanyCode = '{1}') EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
                WHERE Mem_IsComputedPayroll = 1
                    AND(Mem_PHRule = 'F' AND Mem_PHShare = 0)
                    AND ('{2}' = '' OR Mem_IDNo = '{2}')
	                @CONDITIONS

             UNION ALL

            SELECT @MenuCode
                    ,Mem_IDNo
                    ,'BW' 
                    ,'{3}'
		            , NULL 
		            , NULL 
                    ,'Invalid HDMF Premium Rule'  
                    , 1
		            ,'{4}'
		            , GETDATE()
            FROM M_Employee
            @USERCOSTCENTERACCESSCONDITION
            INNER JOIN (SELECT Mpd_SubCode 
			            FROM M_PolicyDtl 
			            WHERE Mpd_PolicyCode = 'EMPSTATPAY'
				            AND Mpd_ParamValue = 1
                            AND Mpd_CompanyCode = '{1}') EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
            LEFT JOIN {0}..M_CodeDtl on Mcd_Code = Mem_PagIbigRule
	            AND Mcd_CodeType = 'PREMRULE'
                AND Mcd_CompanyCode = '{1}'
            WHERE Mem_IsComputedPayroll = 1 
                AND (LEN(RTRIM(Mem_PagIbigRule)) = 0 or Mcd_Code is null)
                AND ('{2}' = '' OR Mem_IDNo = '{2}')
	            @CONDITIONS

            UNION ALL

            SELECT @MenuCode
                    ,Mem_IDNo
		            ,'BW' 
		            ,'{3}'
		            ,Mem_PagIbigShare 
		            ,NULL 
		            ,'HDMF EE share is zero'  
                    , 1
		            ,'{4}'
                    , GETDATE()
            FROM M_Employee 
            @USERCOSTCENTERACCESSCONDITION
            INNER JOIN (SELECT Mpd_SubCode 
			            FROM M_PolicyDtl 
			            WHERE Mpd_PolicyCode = 'EMPSTATPAY'
				            AND Mpd_ParamValue = 1
                            AND Mpd_CompanyCode = '{1}') EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
            LEFT JOIN {0}..M_PremiumSchedule on Mps_EEShare = Mem_PagIbigShare AND 
	            Mps_DeductionCode = 'PHPREM'  AND 
	            Mps_PayCycle = @PhApplicPayPeriod  AND
                Mps_CompanyCode = '{1}' AND 
	            Mps_RecordStatus = 'A'
            WHERE Mem_IsComputedPayroll = 1 
               AND  (Mem_PagIbigRule = 'F' AND Mem_PagIbigShare = 0)
	            AND ('{2}' = '' OR Mem_IDNo = '{2}')
	            @CONDITIONS

            UNION ALL

            SELECT  @MenuCode
                    ,Mem_IDNo
                    ,'BW' 
                    ,'{3}'
                    , NULL 
                    , NULL 
                    , CASE WHEN LEN(RTRIM(Mem_TaxRule)) = 0 THEN 'No Tax Rule' ELSE Mem_TaxRule + ' Tax rule is not in Master' END
                    , 1
                    ,'{4}'
                    , GETDATE()
            FROM M_Employee
            @USERCOSTCENTERACCESSCONDITION
            INNER JOIN (SELECT Mpd_SubCode 
                           FROM M_PolicyDtl 
                           WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                                AND Mpd_ParamValue = 1
                                AND Mpd_CompanyCode = '{1}') EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
            LEFT JOIN {0}..M_CodeDtl on Mcd_Code = Mem_TaxRule
                AND Mcd_CodeType = 'TAXRULE'
                AND Mcd_CompanyCode = '{1}'
            WHERE Mem_IsComputedPayroll = 1 
                AND (LEN(RTRIM(Mem_TaxRule)) = 0 OR Mcd_Code IS NULL)
                AND ('{2}' = '' OR Mem_IDNo = '{2}')
	            @CONDITIONS

            UNION ALL

            SELECT  @MenuCode
                    ,Mem_IDNo
                    ,'BW' 
                    ,'{3}'
                    , NULL 
                    , NULL 
                    , CASE WHEN LEN(RTRIM(Mem_UnionRule)) = 0 THEN 'No Union rule' ELSE Mem_UnionRule + ' Union rule is not in Master' END
                    , 1
                    ,'{4}'
                    , GETDATE()
                FROM M_Employee
                @USERCOSTCENTERACCESSCONDITION
                INNER JOIN (SELECT Mpd_SubCode 
                           FROM M_PolicyDtl 
                           WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                            AND Mpd_ParamValue = 1
                                        AND Mpd_CompanyCode = '{1}') EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
                LEFT JOIN {0}..M_CodeDtl on Mcd_Code = Mem_UnionRule
                    AND Mcd_CodeType = 'PREMRULE'
                    AND Mcd_CompanyCode = '{1}'
                WHERE Mem_IsComputedPayroll = 1 
                    AND (LEN(RTRIM(Mem_UnionRule)) = 0 or Mcd_Code IS NULL)
                    AND ('{2}' = '' OR Mem_IDNo = '{2}')
                    AND @UNIONDUES = 1
	                @CONDITIONS

            UNION ALL

            SELECT @MenuCode
                    ,Mem_IDNo
                    ,'BW' 
                    ,'{3}'
		            ,NULL 
		            ,NULL 
                    ,'No Bank Code'
                    , 1  
		            ,'{4}'
		            ,GETDATE()
            FROM M_Employee
            @USERCOSTCENTERACCESSCONDITION
            INNER JOIN (SELECT Mpd_SubCode 
			            FROM M_PolicyDtl 
			            WHERE Mpd_PolicyCode = 'EMPSTATPAY'
				            AND Mpd_ParamValue = 1
                            AND Mpd_CompanyCode = '{1}') EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
            WHERE Mem_IsComputedPayroll = 1 
                AND Mem_PaymentMode  = 'B' 
	            AND LEN(RTRIM(Mem_PayrollBankCode)) = 0
                AND ('{2}' = '' OR Mem_IDNo = '{2}')
	            @CONDITIONS

            UNION ALL

            SELECT @MenuCode
                ,Mem_IDNo
	            ,'BW' 
	            ,'{3}'
	            , NULL 
	            , NULL 
	            ,'No Bank Account'  
                , 1
	            ,'{4}'
                , GETDATE()
            FROM M_Employee
            @USERCOSTCENTERACCESSCONDITION
            INNER JOIN (SELECT Mpd_SubCode 
			            FROM M_PolicyDtl 
			            WHERE Mpd_PolicyCode = 'EMPSTATPAY'
				            AND Mpd_ParamValue = 1
                            AND Mpd_CompanyCode = '{1}') EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
            WHERE Mem_IsComputedPayroll = 1 
                AND Mem_PaymentMode  = 'B' 
	            AND LEN(RTRIM(Mem_BankAcctNo)) = 0
                AND ('{2}' = '' OR Mem_IDNo = '{2}')
	            @CONDITIONS

            UNION ALL

            SELECT @MenuCode
                ,Mem_IDNo
	            ,'BW' 
	            ,'{3}'
	            ,NULL 
	            ,NULL 
	            ,'Invalid Bank Account Length ' +  Mem_BankAcctNo  
                , 1
	            ,'{4}'
	            , GETDATE()
            FROM M_Employee
            @USERCOSTCENTERACCESSCONDITION
            INNER JOIN (SELECT Mpd_SubCode 
			            FROM M_PolicyDtl 
			            WHERE Mpd_PolicyCode = 'EMPSTATPAY'
				            AND Mpd_ParamValue = 1
                            AND Mpd_CompanyCode = '{1}') EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
            LEFT JOIN {0}..M_PolicyDtl SECNUMLEN ON SECNUMLEN.Mpd_PolicyCode = 'SECNUMLEN'
	            AND SECNUMLEN.Mpd_SubCode = Mem_PayrollBankCode
                AND SECNUMLEN.Mpd_CompanyCode = '{1}'
            WHERE Mem_IsComputedPayroll = 1 
	            AND Mem_BankAcctNo NOT IN ('APPLIED', '') 
                AND LEN(replace(Mem_BankAcctNo,'-','')) <> Mpd_ParamValue
                AND ('{2}' = '' OR Mem_IDNo = '{2}')
	            @CONDITIONS

            UNION ALL

            SELECT @MenuCode
                    ,Mem_IDNo
                    ,'BW' 
                    ,'{3}'
                    ,NULL 
                    ,NULL 
                    ,CASE WHEN LEN(RTRIM(Mem_PaymentMode)) = 0 THEN 'No Payment Mode' ELSE Mem_PaymentMode + ' Payment mode is not in Master' END
                    ,1  
                    ,'{4}'
                    ,GETDATE()
            FROM M_Employee
            @USERCOSTCENTERACCESSCONDITION
            INNER JOIN (SELECT Mpd_SubCode 
                       FROM M_PolicyDtl 
                       WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
                        AND Mpd_CompanyCode = '{1}') EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
            LEFT JOIN {0}..M_CodeDtl PAYMODE ON PAYMODE.Mcd_Code = Mem_PaymentMode
                AND PAYMODE.Mcd_CodeType = 'PAYMODE'
                AND PAYMODE.Mcd_CompanyCode  = '{1}'
            WHERE Mem_IsComputedPayroll = 1 
                AND LEN(RTRIM(Mem_PaymentMode)) = 0 OR PAYMODE.Mcd_Name IS NULL
                AND ('{2}' = '' OR Mem_IDNo = '{2}')
	            @CONDITIONS

            UNION ALL

            SELECT @MenuCode
                    ,Mem_IDNo
                    ,'BW' 
                    ,'{3}'
                    ,NULL 
                    ,NULL 
                    ,CASE WHEN LEN(RTRIM(Mem_ExpenseClass)) = 0 THEN 'No Expense Class' ELSE Mem_ExpenseClass + ' Expense class is not in Master' END
                    , 1  
                    ,'{4}'
                    ,GETDATE()
            FROM M_Employee
            @USERCOSTCENTERACCESSCONDITION
            INNER JOIN (SELECT Mpd_SubCode 
                       FROM M_PolicyDtl 
                       WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
                        AND Mpd_CompanyCode = '{1}') EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
            LEFT JOIN {0}..M_Expense ON Mex_ExpenseClass = Mem_ExpenseClass
                AND Mex_CompanyCode  = '{1}'
            WHERE Mem_IsComputedPayroll = 1 
                AND LEN(RTRIM(Mem_ExpenseClass)) = 0 or Mex_ExpenseName IS NULL
                AND ('{2}' = '' OR Mem_IDNo = '{2}')
	            @CONDITIONS

            UNION ALL

            SELECT @MenuCode
                    ,Mem_IDNo
                    ,'BW'
                    ,'{3}'
                    , NULL
                    , NULL
                    , CASE WHEN LEN(RTRIM(Mem_CostcenterCode)) = 0 THEN 'No Costcenter set'
                        WHEN Mcc_CostcenterCode IS NULL THEN Mcc_CostcenterCode + ' costcenter code is not in Master'
                        WHEN Mcc_RecordStatus = 'C' THEN Mcc_CostcenterCode + ' costcenter code is inactive' END
                    ,1
                    ,'{4}'
                    ,GETDATE()
            FROM M_Employee
            @USERCOSTCENTERACCESSCONDITION
            INNER JOIN(SELECT Mpd_SubCode
                        FROM M_PolicyDtl
                        WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                         AND Mpd_ParamValue = 1
                         AND Mpd_CompanyCode = '{1}') EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
            LEFT JOIN {0}..M_Costcenter on Mcc_CompanyCode = '{1}'
                AND Mcc_CostCenterCode = Mem_CostcenterCode
            WHERE Mem_IsComputedPayroll = 1
                AND(LEN(RTRIM(Mem_CostcenterCode)) = 0 OR Mcc_CostcenterCode IS NULL OR Mcc_RecordStatus = 'C')
                AND ('{2}' = '' OR Mem_IDNo = '{2}')
	            @CONDITIONS

            UNION ALL

            SELECT @MenuCode
                  ,Mem_IDNo
                  ,'BW' 
                  ,'{3}'
                  ,NULL 
                  ,NULL 
                  ,CASE WHEN LEN(RTRIM(Mem_PayrollType)) = 0 THEN 'No Payroll Type' ELSE Mem_PayrollType + ' Payroll type is not in Master' END
                  ,1  
                  ,'{4}'
                  ,GETDATE()
            FROM M_Employee
            @USERCOSTCENTERACCESSCONDITION
            INNER JOIN (SELECT Mpd_SubCode 
                        FROM M_PolicyDtl 
                        WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                            AND Mpd_ParamValue = 1
                            AND Mpd_CompanyCode = '{1}') EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
            LEFT JOIN {0}..M_CodeDtl ON Mcd_Code = Mem_PayrollType
                AND Mcd_CodeType = 'PAYTYPE'
                AND Mcd_CompanyCode  = '{1}'
            WHERE Mem_IsComputedPayroll = 1 
                AND LEN(RTRIM(Mem_PayrollType)) = 0 OR Mcd_Name IS NULL
                AND ('{2}' = '' OR Mem_IDNo = '{2}')
	            @CONDITIONS

            UNION ALL

            SELECT @MenuCode
                  ,Mem_IDNo
                  ,'BW' 
                  ,'{3}'
                  ,NULL 
                  ,NULL 
                  ,CASE WHEN LEN(RTRIM(Mem_PremiumGrpCode)) = 0 THEN 'No Premium Group' ELSE Mem_PremiumGrpCode + ' Premium group is not in Master' END
                  ,1  
                  ,'{4}'
                  ,GETDATE()
            FROM M_Employee
            @USERCOSTCENTERACCESSCONDITION
            INNER JOIN (SELECT Mpd_SubCode 
                       FROM M_PolicyDtl 
                       WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                            AND Mpd_ParamValue = 1
                            AND Mpd_CompanyCode = '{1}') EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
            LEFT JOIN {0}..M_CodeDtl ON Mcd_Code = Mem_PremiumGrpCode
                AND Mcd_CodeType = 'PREMGRP'
                AND Mcd_CompanyCode  = '{1}'
            WHERE Mem_IsComputedPayroll = 1 
                AND (LEN(RTRIM(Mem_PremiumGrpCode)) = 0 OR Mcd_Name IS NULL)  
                AND ('{2}' = '' OR Mem_IDNo = '{2}')
	            @CONDITIONS      
    
            UNION ALL

            SELECT @MenuCode
                  ,Tdh_IDNo
                  ,'BW' 
                  ,'{3}'
                  ,0 
                  ,0 
                  ,Tdh_DeductionCode + ' code is inactive'
                  ,1
                  ,'{4}'
                  ,GETDATE()
            FROM {0}..T_EmpDeductionHdr
            INNER JOIN M_Employee ON Mem_IDNo = Tdh_IDNo
                AND Mem_IsComputedPayroll = 1
                AND ('{2}' = '' OR Mem_IDNo = '{2}')
                @CONDITIONS 
            @USERCOSTCENTERACCESSCONDITION
            INNER JOIN {0}..M_Deduction ON Mdn_DeductionCode = Tdh_DeductionCode
                AND Mdn_CompanyCode = '{1}'
            INNER JOIN (SELECT Mpd_SubCode 
                        FROM M_PolicyDtl 
                        WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                            AND Mpd_ParamValue = 1
                            AND Mpd_CompanyCode = '{1}') EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
            WHERE  Tdh_DeductionAmount > Tdh_PaidAmount  
                AND Mdn_RecordStatus = 'C'

            UNION ALL

            SELECT @MenuCode
                  ,Tin_IDNo
                  ,'BW' 
                  ,'{3}'
                  ,0 
                  ,0 
                  ,Tin_IncomeCode + ' code is inactive'
                  ,1
                  ,'{4}'
                  ,GETDATE()
                FROM T_EmpIncome
                INNER JOIN M_Employee ON Mem_IDNo = Tin_IDNo
                    AND Mem_IsComputedPayroll = 1
                    AND ('{2}' = '' OR Mem_IDNo = '{2}')
                    @CONDITIONS 
                @USERCOSTCENTERACCESSCONDITION
                INNER JOIN {0}..M_Income ON Min_IncomeCode = Tin_IncomeCode
                    AND Min_CompanyCode = '{1}'
                    AND Min_RecordStatus = 'C'
                INNER JOIN (SELECT Mpd_SubCode 
                FROM M_PolicyDtl 
                WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                    AND Mpd_ParamValue = 1
                    AND Mpd_CompanyCode = '{1}') EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
                WHERE  Tin_IncomeAmt <> 0    

            UNION ALL    

            SELECT @MenuCode
                ,Mem_IDNo
	            ,'BE' 
	            ,'{3}'
	            ,Tdh_PaidAmount + Tdh_DeferredAmount 
	            ,Tdh_DeductionAmount 
	            ,'Payment to Date + Deferred Amount is more than Deduction Amount' 
                , 1
	            ,'{4}'
	            ,GETDATE()
            FROM {0}..T_EmpDeductionHdr
            INNER JOIN M_Employee ON Mem_IDNo = Tdh_IDNo
                AND Mem_IsComputedPayroll = 1
                AND ('{2}' = '' OR Mem_IDNo = '{2}')
                @CONDITIONS
            @USERCOSTCENTERACCESSCONDITION
            INNER JOIN (SELECT Mpd_SubCode 
			            FROM M_PolicyDtl 
			            WHERE Mpd_PolicyCode = 'EMPSTATPAY'
				            AND Mpd_ParamValue = 1
                            AND Mpd_CompanyCode = '{1}') EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
            WHERE Tdh_PaidAmount  + Tdh_DeferredAmount > Tdh_DeductionAmount    

            UNION ALL

            SELECT @MenuCode
                ,Mem_IDNo
	            ,'BE' 
	            ,'{3}'
	            , Tdh_CycleAmount 
	            , Tdh_DeductionAmount 
	            ,'Deduction per Cycle is more than Deduction Amount' 
                , 1
	            ,'{4}'
	            , GETDATE()
            FROM {0}..T_EmpDeductionHdr
            INNER JOIN M_Employee ON Mem_IDNo = Tdh_IDNo
                AND Mem_IsComputedPayroll = 1
                AND ('{2}' = '' OR Mem_IDNo = '{2}')
                @CONDITIONS
            @USERCOSTCENTERACCESSCONDITION
            INNER JOIN (SELECT Mpd_SubCode 
			            FROM M_PolicyDtl 
			            WHERE Mpd_PolicyCode = 'EMPSTATPAY'
				            AND Mpd_ParamValue = 1
                            AND Mpd_CompanyCode = '{1}') EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
            WHERE Tdh_CycleAmount > Tdh_DeductionAmount

            UNION ALL

            SELECT @MenuCode
                ,Mem_IDNo
	            ,'BE' 
	            ,'{3}'
	            ,Tdh_PaidAmount 
	            ,Tdh_DeferredAmount 
	            ,'Paid Up but with Deferred Amount' 
                , 1
	            ,'{4}'
	            ,GETDATE()
            FROM {0}..T_EmpDeductionHdr
            INNER JOIN M_Employee ON Mem_IDNo = Tdh_IDNo
                AND Mem_IsComputedPayroll = 1
                AND ('{2}' = '' OR Mem_IDNo = '{2}')
                @CONDITIONS
            @USERCOSTCENTERACCESSCONDITION
            INNER JOIN (SELECT Mpd_SubCode 
			            FROM M_PolicyDtl 
			            WHERE Mpd_PolicyCode = 'EMPSTATPAY'
				            AND Mpd_ParamValue = 1
                            AND Mpd_CompanyCode = '{1}') EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
            WHERE Tdh_DeductionAmount = Tdh_PaidAmount AND Tdh_DeferredAmount > 0   

            UNION ALL

            SELECT @MenuCode
                ,Mem_IDNo
	            ,'BE' 
	            ,'{3}'
	            ,Tdh_DeductionAmount 
	            ,NULL 
	            ,'Deduction Amount is zero or negative' 
                , 1
	            ,'{4}'
	            ,GETDATE()
            FROM {0}..T_EmpDeductionHdr
            INNER JOIN M_Employee ON Mem_IDNo = Tdh_IDNo
                AND Mem_IsComputedPayroll = 1
                AND ('{2}' = '' OR Mem_IDNo = '{2}')
                @CONDITIONS
            @USERCOSTCENTERACCESSCONDITION
            INNER JOIN (SELECT Mpd_SubCode 
			            FROM M_PolicyDtl 
			            WHERE Mpd_PolicyCode = 'EMPSTATPAY'
				            AND Mpd_ParamValue = 1
                            AND Mpd_CompanyCode = '{1}') EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
            WHERE Tdh_DeductionAmount <= 0  

            UNION ALL

            SELECT @MenuCode
                ,Mem_IDNo
	            ,'BE' 
	            ,'{3}'
	            ,Tdh_DeductionAmount 
	            ,NULL 
	            ,'Payment to date is negative' 
                , 1
	            ,'{4}'
	            ,GETDATE()
            FROM {0}..T_EmpDeductionHdr
            INNER JOIN M_Employee ON Mem_IDNo = Tdh_IDNo
                AND Mem_IsComputedPayroll = 1
                AND ('{2}' = '' OR Mem_IDNo = '{2}')
                @CONDITIONS
            @USERCOSTCENTERACCESSCONDITION
            INNER JOIN (SELECT Mpd_SubCode 
			            FROM M_PolicyDtl 
			            WHERE Mpd_PolicyCode = 'EMPSTATPAY'
				            AND Mpd_ParamValue = 1
                            AND Mpd_CompanyCode = '{1}') EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
            WHERE  Tdh_PaidAmount < 0 

            UNION ALL

            SELECT @MenuCode
                ,Mem_IDNo
	            ,'BE' 
	            ,'{3}'
	            , Tdh_DeductionAmount 
	            , NULL 
	            ,'Deduction per Cycle is negative' 
                , 1
		        , '{4}'
	            , GETDATE()
            FROM {0}..T_EmpDeductionHdr
            INNER JOIN M_Employee ON Mem_IDNo = Tdh_IDNo
                AND Mem_IsComputedPayroll = 1
                AND ('{2}' = '' OR Mem_IDNo = '{2}')
                @CONDITIONS
            @USERCOSTCENTERACCESSCONDITION
            INNER JOIN (SELECT Mpd_SubCode 
			            FROM M_PolicyDtl 
			            WHERE Mpd_PolicyCode = 'EMPSTATPAY'
				            AND Mpd_ParamValue = 1
                            AND Mpd_CompanyCode = '{1}') EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
            WHERE  Tdh_CycleAmount < 0

            UNION ALL

            SELECT @MenuCode
                ,Mem_IDNo
	            ,'BE' 
	            ,'{3}'
	            , Tdh_DeductionAmount 
	            , 0.00 
	            ,'Deferred Amount is negative' 
                , 1
	            , '{4}'
	            , GETDATE()
            FROM {0}..T_EmpDeductionHdr
            INNER JOIN M_Employee ON Mem_IDNo = Tdh_IDNo
                AND Mem_IsComputedPayroll = 1
                AND ('{2}' = '' OR Mem_IDNo = '{2}')
                @CONDITIONS
            @USERCOSTCENTERACCESSCONDITION
            INNER JOIN (SELECT Mpd_SubCode 
			            FROM M_PolicyDtl 
			            WHERE Mpd_PolicyCode = 'EMPSTATPAY'
				            AND Mpd_ParamValue = 1
                            AND Mpd_CompanyCode = '{1}') EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
            WHERE  Tdh_DeferredAmount < 0

            UNION ALL

            SELECT @MenuCode
                ,Mem_IDNo
	            ,'BE' 
	            ,'{3}'
	            , Tdh_DeferredAmount 
	            , Defer.Amt 
	            ,'Summary and Details of Deferred amount do not match: ' + Tdh_DeductionCode + ' ' + CONVERT(CHAR(10),Tdh_StartDate, 101) 
                , 1 
	            , '{4}'
	            , GETDATE()
            FROM {0}..T_EmpDeductionHdr
            INNER JOIN M_Employee ON Mem_IDNo = Tdh_IDNo
                AND Mem_IsComputedPayroll = 1
                AND ('{2}' = '' OR Mem_IDNo = '{2}')
                @CONDITIONS
            @USERCOSTCENTERACCESSCONDITION
            INNER JOIN (SELECT Mpd_SubCode 
			            FROM M_PolicyDtl 
			            WHERE Mpd_PolicyCode = 'EMPSTATPAY'
				            AND Mpd_ParamValue = 1
                            AND Mpd_CompanyCode = '{1}') EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
            LEFT JOIN (SELECT Tdd_IDNo
				            ,Tdd_DeductionCode
				            ,Tdd_StartDate
				            ,Amt = SUM(Tdd_DeferredAmt)
			            FROM T_EmpDeductionDefer
			            GROUP BY Tdd_IDNo
				            ,Tdd_DeductionCode
				            ,Tdd_StartDate) Defer ON Defer.Tdd_IDNo = Tdh_IDNo
	                                            AND Defer.Tdd_DeductionCode = Tdh_DeductionCode
	                                            AND Defer.Tdd_StartDate =  Tdh_StartDate
            WHERE Tdh_DeferredAmount <> ISNULL(Defer.Amt,0)

            UNION ALL

            SELECT @MenuCode
                ,Mem_IDNo
	            ,'BE' 
	            ,'{3}'
	            , Tdh_PaidAmount 
	            , Paid.Amt 
	            ,'Summary and Details of Paid amount do not match: ' + Tdh_DeductionCode + ' ' + CONVERT(CHAR(10),Tdh_StartDate, 101)  
                , 1
	            , '{4}'
	            , GETDATE()
            FROM {0}..T_EmpDeductionHdr
            INNER JOIN M_Employee ON Mem_IDNo = Tdh_IDNo
                AND Mem_IsComputedPayroll = 1
                AND ('{2}' = '' OR Mem_IDNo = '{2}')
                @CONDITIONS
            @USERCOSTCENTERACCESSCONDITION
            INNER JOIN (SELECT Mpd_SubCode 
			            FROM M_PolicyDtl 
			            WHERE Mpd_PolicyCode = 'EMPSTATPAY'
				            AND Mpd_ParamValue = 1
                            AND Mpd_CompanyCode = '{1}') EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
                            LEFT JOIN (SELECT Tdd_IDNo
				                            ,Tdd_DeductionCode
				                            ,Tdd_StartDate
				                            ,Amt = SUM(Tdd_Amount)
			                            FROM T_EmpDeductionDtlHst
			                            WHERE Tdd_PaymentFlag = 1
			                            GROUP BY Tdd_IDNo
				                            ,Tdd_DeductionCode
				                            ,Tdd_StartDate) Paid ON Paid.Tdd_IDNo = Tdh_IDNo
	                            AND Paid.Tdd_DeductionCode = Tdh_DeductionCode
	                            AND Paid.Tdd_StartDate =  Tdh_StartDate
           WHERE Tdh_PaidAmount <> ISNULL(Paid.Amt,0)
           
        
           UNION ALL

        SELECT @MenuCode
            ,Mem_IDNo
            ,'BW' 
            ,'{3}'
            , NULL 
            , NULL
            ,'Computed by hours present'
            , 1  
            , '{4}'
            , GETDATE()
        FROM M_Employee 
        @USERCOSTCENTERACCESSCONDITION
        WHERE Mem_IsComputedPerDay = 1
	        AND Mem_PayrollType = 'M'
            AND ('{2}' = '' OR Mem_IDNo = '{2}')
            @CONDITIONS

        UNION ALL

        SELECT @MenuCode
            ,Mem_IDNo
            ,'BW' 
            ,'{3}'
            , NULL 
            , NULL
            ,'Multiple Salary'
            , 1  
            , '{4}'
            , GETDATE()
        FROM M_Employee 
        @USERCOSTCENTERACCESSCONDITION
        WHERE Mem_IsMultipleSalary = 1
            AND ('{2}' = '' OR Mem_IDNo = '{2}')
            @CONDITIONS

        UNION ALL

        SELECT @MenuCode
                ,Tsl_IDNo
	            ,'BE' 
	            ,'{3}'
	            ,NULL 
	            ,NULL 
	            ,'End date ' + CONVERT(VARCHAR(10), Tsl_EndDate, 101) + ' is greater than Start date ' + CONVERT(VARCHAR(10), Tsl_StartDate, 101) 
                , 1
	            ,'{4}'
	            ,GETDATE()
            FROM {0}..T_EmpSalary
            INNER JOIN M_Employee ON Mem_IDNo = Tsl_IDNo
                AND Mem_IsComputedPayroll = 1
                AND ('{2}' = '' OR Mem_IDNo = '{2}')
                @CONDITIONS
            @USERCOSTCENTERACCESSCONDITION
            WHERE Tsl_EndDate IS NOT NULL AND Tsl_EndDate < Tsl_StartDate

           UNION ALL

           SELECT @MenuCode
                ,Tsl_IDNo
	            ,'BE' 
	            ,'{3}'
	            ,NULL 
	            ,NULL 
	            ,CONVERT(CHAR(10),Tsl_EndDate,101) + 'Multiple Empty Salary End Date'
                , 1
	            ,'{4}'
	            ,GETDATE()
            FROM {0}..T_EmpSalary
            INNER JOIN M_Employee ON Mem_IDNo = Tsl_IDNo
                AND Mem_IsComputedPayroll = 1
                AND ('{2}' = '' OR Mem_IDNo = '{2}')
                @CONDITIONS
            @USERCOSTCENTERACCESSCONDITION
			WHERE Tsl_EndDate IS NULL
		    GROUP BY Tsl_IDNo,Tsl_EndDate
	        HAVING COUNT(*) > 1

            UNION ALL

            SELECT @MenuCode
                ,Tsl_IDNo
	            ,'BE' 
	            ,'{3}'
	            ,NULL 
	            ,NULL 
	            ,CONVERT(CHAR(10),Tsl_EndDate,101) + ' Salary Start Date > End Date'
                , 1
	            ,'{4}'
	            ,GETDATE()
            FROM {0}..T_EmpSalary
            INNER JOIN M_Employee ON Mem_IDNo = Tsl_IDNo
                AND Mem_IsComputedPayroll = 1
                AND ('{2}' = '' OR Mem_IDNo = '{2}')
                @CONDITIONS
            @USERCOSTCENTERACCESSCONDITION
			WHERE Tsl_StartDate > Tsl_EndDate
	            AND Tsl_EndDate IS NOT NULL
                
            UNION ALL

            SELECT @MenuCode
                ,A.Tsl_IDNo
	            ,'BE' 
	            ,'{3}'
	            ,NULL 
	            ,NULL 
	            ,CONVERT(CHAR(10),A.Tsl_StartDate,101) + ' Overlap Salary Date'
                , 1
	            ,'{4}'
	            ,GETDATE()
            FROM {0}..T_EmpSalary A
            INNER JOIN {0}..T_EmpSalary B ON B.Tsl_IDNo = A.Tsl_IDNo
	            AND B.Tsl_StartDate < A.Tsl_EndDate
	            AND A.Tsl_StartDate < B.Tsl_EndDate
	            AND A.Tsl_StartDate < B.Tsl_StartDate
            INNER JOIN M_Employee ON Mem_IDNo = A.Tsl_IDNo
                AND Mem_IsComputedPayroll = 1
                AND ('{2}' = '' OR Mem_IDNo = '{2}')
                @CONDITIONS
            @USERCOSTCENTERACCESSCONDITION
			WHERE 1=1  

            UNION ALL

            SELECT @MenuCode
                ,Mem_IDNo
	            ,'BW' 
	            ,'{3}'
	            , NULL 
	            , NULL 
	            , CASE WHEN Mem_WorkStatus = 'IM' THEN 'Moved Employee Included in Payroll' 
		            WHEN Mem_WorkStatus = 'IN' THEN 'Inactive Employee Included in Payroll'  END
	            , 1
                ,'{4}'
	            , GETDATE()
            FROM M_Employee 
            @USERCOSTCENTERACCESSCONDITION
            INNER JOIN (SELECT Mpd_SubCode 
			            FROM M_PolicyDtl 
			            WHERE Mpd_PolicyCode = 'EMPSTATPAY'
				            AND Mpd_ParamValue = 1
                            AND Mpd_CompanyCode = '{1}') EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
            WHERE Mem_IsComputedPayroll = 1 
	            AND Mem_WorkStatus IN ('IN','IM') 
	            AND ('{2}' = '' OR Mem_IDNo = '{2}')
	            @CONDITIONS

            UNION ALL

            SELECT @MenuCode
                ,Tfa_IDNo
	            ,'BW' 
	            ,'{3}'
	            , NULL 
	            , NULL 
	            , Tfa_AllowanceCode + ' not created'
	            , 1
                ,'{4}'
	            , GETDATE()
             FROM
	            (SELECT RecurAllow.Tfa_IDNo
		            ,RecurAllow.Tfa_AllowanceCode
	            FROM {0}..T_EmpFixAllowance RecurAllow
	            INNER JOIN M_Employee ON Mem_IDNo = Tfa_IDNo
		            AND Mem_WorkStatus = 'A'
                    AND Mem_IsComputedPayroll = 1
                    AND ('{2}' = '' OR Mem_IDNo = '{2}')
                    @CONDITIONS
                @USERCOSTCENTERACCESSCONDITION
	            INNER JOIN {0}..M_Income ON Min_IncomeCode = RecurAllow.Tfa_AllowanceCode
		            AND Min_IsRecurring = 1
		            AND Min_CompanyCode =  '{1}'
		            AND Min_RecordStatus = 'A'
		            AND (Min_ApplicablePayCycle = '0' OR Min_ApplicablePayCycle = RIGHT('{3}',1))
	            INNER JOIN (SELECT Mpd_SubCode 
			            FROM M_PolicyDtl 
			            WHERE Mpd_PolicyCode = 'EMPSTATPAY'
				            AND Mpd_ParamValue = 1
                            AND Mpd_CompanyCode = '{1}') EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
	            WHERE Tfa_RecordStatus = 'A'
		            AND Tfa_StartPayCycle <=  '{3}'
		            AND (ISNULL(Tfa_EndPayCycle,'') = '' OR Tfa_EndPayCycle >=  '{3}')
		            AND Tfa_Amount <> 0
	            GROUP BY Tfa_IDNo,
 		            Tfa_AllowanceCode) Income 
	            LEFT JOIN T_EmpIncome ON Tin_IDNo = Tfa_IDNo
		            AND Tin_IncomeCode = Tfa_AllowanceCode
		            AND Tin_PayCycle =  '{3}'
            WHERE Tin_IDNo IS NULL 

            UNION ALL

            SELECT @MenuCode
                ,Mem_IDNo
                ,'BW' 
                ,'{3}'
                , NULL 
                , NULL 
                , Mem_ExpenseClass + ' expense class for ' + Mcc_ExpenseGroup
                , 1  
                ,'{4}'
                ,GETDATE()
            FROM M_Employee
            @USERCOSTCENTERACCESSCONDITION
            INNER JOIN (SELECT Mpd_SubCode 
                        FROM M_PolicyDtl 
                        WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                            AND Mpd_ParamValue = 1
                            AND Mpd_CompanyCode = '{1}') EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
            LEFT JOIN {0}..M_Costcenter ON Mcc_CostCenterCode = Mem_CostcenterCode
                AND Mcc_CompanyCode  = '{1}'
            LEFT JOIN {0}..M_Expense ON Mex_ExpenseClass = Mem_ExpenseClass
                AND Mex_CompanyCode = Mcc_CompanyCode 
            WHERE Mem_IsComputedPayroll = 1 
                AND Mex_ExpenseGroup <> Mcc_ExpenseGroup 
                AND ('{2}' = '' OR Mem_IDNo = '{2}')
	            @CONDITIONS    

            "
            , centralProfile
            , companyCode
            , EmployeeID
            , PayCycle
            , UserLogin
            , dtPayPeriodStart
            , dtPayPeriodEnd
            , menucode);
            #endregion
            query = query.Replace("@CONDITIONS", condition);
            query = query.Replace("@USERCOSTCENTERACCESSCONDITION", (new HRCReportsBL()).UserCostCenterAccessTmpQuery(ProfileCode, "PAYROLL", UserLogin, "Mem_CostcenterCode", "Mem_PayrollGroup", "Mem_EmploymentStatusCode", "Mem_PayrollType", companyCode, centralProfile, false));
            dalHelper.ExecuteNonQuery(query);
        }

        public void CheckPostProcessingInPayrollExists(string ProfileCode, string centralProfile, string PayCycle, string EmployeeID, string CostCenter, string UserLogin, string companyCode, string menucode, DALHelper dalHelper)
        {
           #region Query
            string condition = string.Empty;
            if (CostCenter != string.Empty)
            {
                condition += string.Format("And Tpy_CostcenterCode IN ({0})", (new CommonBL()).EncodeFilterItems(CostCenter, true));
            }
            string query = string.Format(@"
            DECLARE @LATEBRCKTQCNT as tinyint
                                
            SELECT @LATEBRCKTQCNT = (SELECT COUNT(*) FROM M_PolicyDtl
                                    WHERE Mpd_PolicyCode ='LATEBRCKTQ'
                                        AND  Mpd_CompanyCode = '{1}')

            INSERT INTO T_EmpProcessCheck

            SELECT '{5}'
                , Tpd_IDNo
                , 'AE'
                , '{3}'
                , Tpd_SalaryRate
                , 0
                , 'Zero Salary on ' + CONVERT(CHAR(10), Tpd_Date, 101) as Remarks
                , 1
                , '{4}'
                , GETDATE()
            FROM T_EmpPayrollDtl
            INNER JOIN T_EmpPayroll ON Tpd_IDNo = Tpy_IDNo
					AND Tpd_PayCycle = Tpy_PayCycle
                    @CONDITIONS
            @USERCOSTCENTERACCESSCONDITION
            WHERE  Tpd_PayCycle = '{3}'
                AND Tpd_SalaryRate = 0
                AND ('{2}' = '' OR Tpd_IDNo = '{2}')

            UNION ALL

            SELECT '{5}'
                , Tdd_IDNo
                 , 'AW'
                 , '{3}'
                 , Tdd_Amount
                 , 0
                 , Tdd_deductioncode + ' code not subtracted to employee payroll'
                 , 1
                 , '{4}'
                 , GETDATE()
                FROM T_EmpDeductionDtl
                INNER JOIN T_EmpPayroll ON Tdd_IDNo = Tpy_IDNo
					AND Tdd_ThisPayCycle = Tpy_PayCycle
                    @CONDITIONS
                INNER JOIN M_Employee ON Mem_IDNo = Tdd_IDNo
                    AND Mem_IsComputedPayroll = 1
                @USERCOSTCENTERACCESSCONDITION
                INNER JOIN(SELECT Mpd_SubCode
                               FROM M_PolicyDtl
                               WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                                AND Mpd_ParamValue = 1
                                AND Mpd_CompanyCode = '{1}') EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
                WHERE Tdd_ThisPayCycle = '{3}'
                    AND Tdd_PaymentFlag = 0
                    AND Tdd_Amount <> 0
                    AND ('{2}' = '' OR Tdd_IDNo = '{2}')

                UNION ALL

                SELECT '{5}'
                     ,Tin_IDNo
                     ,'AW' 
                     ,'{3}'
                     , Tin_IncomeAmt
                     , 0
                     ,Tin_IncomeCode + ' code not posted to employee payroll' 
                     , 1
                     , '{4}'
                     , GETDATE()
                FROM T_EmpIncome
                INNER JOIN T_EmpPayroll ON Tin_IDNo = Tpy_IDNo
					AND Tin_PayCycle = Tpy_PayCycle
                    @CONDITIONS
                INNER JOIN M_Employee ON Mem_IDNo = Tin_IDNo
                    AND Mem_IsComputedPayroll = 1
                @USERCOSTCENTERACCESSCONDITION
                INNER JOIN (SELECT Mpd_SubCode 
                           FROM M_PolicyDtl 
                           WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                                AND Mpd_ParamValue = 1
                                AND Mpd_CompanyCode = '{1}') EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
                WHERE Tin_PayCycle = '{3}'
                    AND Tin_PostFlag = 0
                    AND Tin_IncomeAmt <> 0
                    AND ('{2}' = '' OR Tin_IDNo = '{2}')

                UNION ALL

                SELECT '{5}'
                    ,Tpy_IDNo
                    , 'AE'
                    , '{3}'
                    , Tpy_SSSEE
                    , 0
                    , 'Negative SSS EE' as Remarks
                    , 1
                    , '{4}'
                    , GETDATE()
                FROM T_EmpPayroll
                @USERCOSTCENTERACCESSCONDITION
                WHERE  Tpy_PayCycle = '{3}'
                    AND Tpy_SSSEE < 0
                    AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                    @CONDITIONS

                UNION ALL

                SELECT '{5}'
                    ,Tpy_IDNo
                    , 'AE'
                    , '{3}'
                    , Tpy_MPFEE
                    , 0
                    , 'Negative MPF EE' as Remarks
                    , 1
                    , '{4}'
                    , GETDATE()
                FROM T_EmpPayroll
                @USERCOSTCENTERACCESSCONDITION
                WHERE  Tpy_PayCycle = '{3}'
                    AND Tpy_MPFEE < 0
                    AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                    @CONDITIONS

                UNION ALL

                SELECT '{5}'
                        , Tpy_IDNo
                        , 'AE'
                        , '{3}'
                        , Tpy_PhilhealthEE
                        , 0
                        , 'Negative PHIC EE' as Remarks
                        , 1
                        , '{4}'
                        , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_PhilhealthEE < 0
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                UNION ALL

                SELECT '{5}'
                        , Tpy_IDNo
                        , 'AE'
                        , '{3}'
                        , Tpy_PagIbigEE
                        , 0
                        , 'Negative HDMF EE' as Remarks
                        , 1
                        , '{4}'
                        , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_PagIbigEE < 0
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                        , Tpy_IDNo
                        , 'AE'
                        , '{3}'
                        , Tpy_PagIbigTaxEE
                        , 0
                         , 'Negative HDMF Taxable EE' as Remarks
                         , 1
                         , '{4}'
                         , GETDATE()
                        FROM T_EmpPayroll
                        @USERCOSTCENTERACCESSCONDITION
                        WHERE  Tpy_PayCycle = '{3}'
                            AND Tpy_PagIbigTaxEE < 0
                            AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                            @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                        , Tpy_IDNo
                        , 'AE'
                        , '{3}'
                        , Tpy_LTAmt
                        , Tpd_LTAmt
                        , 'Summary and Details of LT Amount do not match' as Remarks
                        , 1
                        , '{4}'
                        , GETDATE()
                     FROM T_EmpPayroll
                     @USERCOSTCENTERACCESSCONDITION
                     LEFT JOIN ( SELECT Tpd_IDNo, SUM(Tpd_LTAmt) as Tpd_LTAmt
                                   FROM T_EmpPayrollDtl 
                                   WHERE Tpd_Paycycle = '{3}'
                                   GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                     WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_LTAmt <> ISNULL(Tpd_LTAmt, 0)
                        AND @LATEBRCKTQCNT = 0
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS
    
                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_UTAmt
                            , Tpd_UTAmt
                            , 'Summary and Details of UT Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                     FROM T_EmpPayroll
                     @USERCOSTCENTERACCESSCONDITION
                     LEFT JOIN ( SELECT Tpd_IDNo, SUM(Tpd_UTAmt) as Tpd_UTAmt
                                   FROM T_EmpPayrollDtl 
                                   WHERE Tpd_Paycycle = '{3}'
                                   GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                     WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_UTAmt <> ISNULL(Tpd_UTAmt, 0)
                        AND @LATEBRCKTQCNT = 0
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_REGAmt
                            , Tpd_REGAmt
                            , 'Summary and Details of REG Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                     FROM T_EmpPayroll
                     @USERCOSTCENTERACCESSCONDITION
                     LEFT JOIN ( SELECT Tpd_IDNo, SUM(Tpd_REGAmt) as Tpd_REGAmt
                                   FROM T_EmpPayrollDtl 
                                   WHERE Tpd_Paycycle = '{3}'
                                   GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                     WHERE  Tpy_PayCycle = '{3}'
                            AND Tpy_REGAmt <>  ISNULL(Tpd_REGAmt, 0)
                            AND Tpy_RegRule = 'P'
                            AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                            @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_UPLVAmt
                            , Tpd_UPLVAmt
                            , 'Summary and Details of UPLV Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN(SELECT Tpd_IDNo, SUM(Tpd_UPLVAmt) as Tpd_UPLVAmt
                                 FROM T_EmpPayrollDtl
                                 WHERE Tpd_Paycycle = '{3}'
                                 GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_UPLVAmt <> ISNULL(Tpd_UPLVAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_ABSLEGHOLAmt
                            , Tpd_ABSLEGHOLAmt
                            , 'Summary and Details of ABSLEGHOL Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN(SELECT Tpd_IDNo, SUM(Tpd_ABSLEGHOLAmt) as Tpd_ABSLEGHOLAmt
                                 FROM T_EmpPayrollDtl
                                 WHERE Tpd_Paycycle = '{3}'
                                 GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_ABSLEGHOLAmt <> ISNULL(Tpd_ABSLEGHOLAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_ABSSPLHOLAmt
                            , Tpd_ABSSPLHOLAmt
                            , 'Summary and Details of ABSSPLHOL Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN(SELECT Tpd_IDNo, SUM(Tpd_ABSSPLHOLAmt) as Tpd_ABSSPLHOLAmt
                                 FROM T_EmpPayrollDtl
                                 WHERE Tpd_Paycycle = '{3}'
                                 GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_ABSSPLHOLAmt <> ISNULL(Tpd_ABSSPLHOLAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_ABSCOMPHOLAmt
                            , Tpd_ABSCOMPHOLAmt
                            , 'Summary and Details of ABSCOMPHOL Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN(SELECT Tpd_IDNo, SUM(Tpd_ABSCOMPHOLAmt) as Tpd_ABSCOMPHOLAmt
                                 FROM T_EmpPayrollDtl
                                 WHERE Tpd_Paycycle = '{3}'
                                 GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_ABSCOMPHOLAmt <> ISNULL(Tpd_ABSCOMPHOLAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_ABSPSDAmt
                            , Tpd_ABSPSDAmt
                            , 'Summary and Details of ABSPSD Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN(SELECT Tpd_IDNo, SUM(Tpd_ABSPSDAmt) as Tpd_ABSPSDAmt
                                 FROM T_EmpPayrollDtl
                                 WHERE Tpd_Paycycle = '{3}'
                                 GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_ABSPSDAmt <> ISNULL(Tpd_ABSPSDAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_ABSOTHHOLAmt
                            , Tpd_ABSOTHHOLAmt
                            , 'Summary and Details of ABSOTHHOL Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN(SELECT Tpd_IDNo, SUM(Tpd_ABSOTHHOLAmt) as Tpd_ABSOTHHOLAmt
                                     FROM T_EmpPayrollDtl
                                     WHERE Tpd_Paycycle = '{3}'
                                     GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_ABSOTHHOLAmt <> ISNULL(Tpd_ABSOTHHOLAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_WDABSAmt
                            , Tpd_WDABSAmt
                            , 'Summary and Details of WDABS Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN(SELECT Tpd_IDNo, SUM(Tpd_WDABSAmt) as Tpd_WDABSAmt
                                     FROM T_EmpPayrollDtl
                                     WHERE Tpd_Paycycle = '{3}'
                                     GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_WDABSAmt <> ISNULL(Tpd_WDABSAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_LTUTMaxAmt
                            , Tpd_LTUTMaxAmt
                            , 'Summary and Details of LTUTMax Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN(SELECT Tpd_IDNo, SUM(Tpd_LTUTMaxAmt) as Tpd_LTUTMaxAmt
                                     FROM T_EmpPayrollDtl
                                     WHERE Tpd_Paycycle = '{3}'
                                     GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_LTUTMaxAmt <> ISNULL(Tpd_LTUTMaxAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_PDLVAmt
                            , Tpd_PDLVAmt
                            , 'Summary and Details of PDLV Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN ( SELECT Tpd_IDNo, SUM(Tpd_PDLVAmt) as Tpd_PDLVAmt
                                 FROM T_EmpPayrollDtl 
                                 WHERE Tpd_Paycycle = '{3}'
                                 GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_PDLVAmt <>  ISNULL(Tpd_PDLVAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_PDLEGHOLAmt
                            , Tpd_PDLEGHOLAmt
                            , 'Summary and Details of PDLEGHOL Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN ( SELECT Tpd_IDNo, SUM(Tpd_PDLEGHOLAmt) as Tpd_PDLEGHOLAmt
                                     FROM T_EmpPayrollDtl 
                                     WHERE Tpd_Paycycle = '{3}'
                                     GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_PDLEGHOLAmt <>  ISNULL(Tpd_PDLEGHOLAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_PDSPLHOLAmt
                            , Tpd_PDSPLHOLAmt
                            , 'Summary and Details of PDSPLHOL Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN ( SELECT Tpd_IDNo, SUM(Tpd_PDSPLHOLAmt) as Tpd_PDSPLHOLAmt
                                     FROM T_EmpPayrollDtl 
                                     WHERE Tpd_Paycycle = '{3}'
                                     GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_PDSPLHOLAmt <>  ISNULL(Tpd_PDSPLHOLAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_PDCOMPHOLAmt
                            , Tpd_PDCOMPHOLAmt
                            , 'Summary and Details of PDCOMPHOL Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN ( SELECT Tpd_IDNo, SUM(Tpd_PDCOMPHOLAmt) as Tpd_PDCOMPHOLAmt
                                     FROM T_EmpPayrollDtl 
                                     WHERE Tpd_Paycycle = '{3}'
                                     GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_PDCOMPHOLAmt <>  ISNULL(Tpd_PDCOMPHOLAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_PDPSDAmt
                            , Tpd_PDPSDAmt
                            , 'Summary and Details of PDPSD Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN ( SELECT Tpd_IDNo, SUM(Tpd_PDPSDAmt) as Tpd_PDPSDAmt
                                     FROM T_EmpPayrollDtl 
                                     WHERE Tpd_Paycycle = '{3}'
                                     GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_PDPSDAmt <>  ISNULL(Tpd_PDPSDAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_PDOTHHOLAmt
                            , Tpd_PDOTHHOLAmt
                            , 'Summary and Details of PDOTHHOL Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN ( SELECT Tpd_IDNo, SUM(Tpd_PDOTHHOLAmt) as Tpd_PDOTHHOLAmt
                                     FROM T_EmpPayrollDtl 
                                     WHERE Tpd_Paycycle = '{3}'
                                     GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_PDOTHHOLAmt <>  ISNULL(Tpd_PDOTHHOLAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_PDRESTLEGHOLAmt
                            , Tpd_PDRESTLEGHOLAmt
                            , 'Summary and Details of PDRESTLEGHOL Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN ( SELECT Tpd_IDNo, SUM(Tpd_PDRESTLEGHOLAmt) as Tpd_PDRESTLEGHOLAmt
                                     FROM T_EmpPayrollDtl 
                                     WHERE Tpd_Paycycle = '{3}'
                                     GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_PDRESTLEGHOLAmt <>  ISNULL(Tpd_PDRESTLEGHOLAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL
                
                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_REGOTAmt
                            , Tpd_REGOTAmt
                            , 'Summary and Details of REGOT Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN ( SELECT Tpd_IDNo, SUM(Tpd_REGOTAmt) as Tpd_REGOTAmt
                                     FROM T_EmpPayrollDtl 
                                     WHERE Tpd_Paycycle = '{3}'
                                     GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_REGOTAmt <>  ISNULL(Tpd_REGOTAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_REGNDAmt
                            , Tpd_REGNDAmt
                            , 'Summary and Details of REGND Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN ( SELECT Tpd_IDNo, SUM(Tpd_REGNDAmt) as Tpd_REGNDAmt
                                     FROM T_EmpPayrollDtl 
                                     WHERE Tpd_Paycycle = '{3}'
                                     GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_REGNDAmt <>  ISNULL(Tpd_REGNDAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_REGNDOTAmt
                            , Tpd_REGNDOTAmt
                            , 'Summary and Details of REGNDOT Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN ( SELECT Tpd_IDNo, SUM(Tpd_REGNDOTAmt) as Tpd_REGNDOTAmt
                                     FROM T_EmpPayrollDtl 
                                     WHERE Tpd_Paycycle = '{3}'
                                     GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                         AND Tpy_REGNDOTAmt <>  ISNULL(Tpd_REGNDOTAmt, 0)
                         AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                         @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_RESTAmt
                            , Tpd_RESTAmt
                            , 'Summary and Details of REST Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN ( SELECT Tpd_IDNo, SUM(Tpd_RESTAmt) as Tpd_RESTAmt
                                     FROM T_EmpPayrollDtl 
                                     WHERE Tpd_Paycycle = '{3}'
                                     GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_RESTAmt <>  ISNULL(Tpd_RESTAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_RESTOTAmt
                            , Tpd_RESTOTAmt
                            , 'Summary and Details of RESTOT Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN ( SELECT Tpd_IDNo, SUM(Tpd_RESTOTAmt) as Tpd_RESTOTAmt
                                     FROM T_EmpPayrollDtl 
                                     WHERE Tpd_Paycycle = '{3}'
                                     GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_RESTOTAmt <>  ISNULL(Tpd_RESTOTAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_RESTNDAmt
                            , Tpd_RESTNDAmt
                            , 'Summary and Details of RESTND Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN ( SELECT Tpd_IDNo, SUM(Tpd_RESTNDAmt) as Tpd_RESTNDAmt
                                     FROM T_EmpPayrollDtl 
                                     WHERE Tpd_Paycycle = '{3}'
                                     GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_RESTNDAmt <>  ISNULL(Tpd_RESTNDAmt, 0) 
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS         

                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_RESTNDOTAmt
                            , Tpd_RESTNDOTAmt
                            , 'Summary and Details of RESTNDOT Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN ( SELECT Tpd_IDNo, SUM(Tpd_RESTNDOTAmt) as Tpd_RESTNDOTAmt
                                     FROM T_EmpPayrollDtl 
                                     WHERE Tpd_Paycycle = '{3}'
                                     GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_RESTNDOTAmt <>  ISNULL(Tpd_RESTNDOTAmt, 0)   
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_LEGHOLAmt
                            , Tpd_LEGHOLAmt
                            , 'Summary and Details of LEGHOL Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN ( SELECT Tpd_IDNo, SUM(Tpd_LEGHOLAmt) as Tpd_LEGHOLAmt
                                     FROM T_EmpPayrollDtl 
                                     WHERE Tpd_Paycycle = '{3}'
                                     GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_LEGHOLAmt <>  ISNULL(Tpd_LEGHOLAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_LEGHOLOTAmt
                            , Tpd_LEGHOLOTAmt
                            , 'Summary and Details of LEGHOLOT Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN ( SELECT Tpd_IDNo, SUM(Tpd_LEGHOLOTAmt) as Tpd_LEGHOLOTAmt
                                     FROM T_EmpPayrollDtl 
                                     WHERE Tpd_Paycycle = '{3}'
                                     GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_LEGHOLOTAmt <>  ISNULL(Tpd_LEGHOLOTAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_LEGHOLNDAmt
                            , Tpd_LEGHOLNDAmt
                            , 'Summary and Details of LEGHOLND Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN ( SELECT Tpd_IDNo, SUM(Tpd_LEGHOLNDAmt) as Tpd_LEGHOLNDAmt
                                     FROM T_EmpPayrollDtl 
                                     WHERE Tpd_Paycycle = '{3}'
                                     GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_LEGHOLNDAmt <>  ISNULL(Tpd_LEGHOLNDAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL 

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_LEGHOLNDOTAmt
                            , Tpd_LEGHOLNDOTAmt
                            , 'Summary and Details of LEGHOLNDOT Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN ( SELECT Tpd_IDNo, SUM(Tpd_LEGHOLNDOTAmt) as Tpd_LEGHOLNDOTAmt
                                     FROM T_EmpPayrollDtl 
                                     WHERE Tpd_Paycycle = '{3}'
                                     GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_LEGHOLNDOTAmt <>  ISNULL(Tpd_LEGHOLNDOTAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_SPLHOLAmt
                            , Tpd_SPLHOLAmt
                            , 'Summary and Details of SPLHOL Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN ( SELECT Tpd_IDNo, SUM(Tpd_SPLHOLAmt) as Tpd_SPLHOLAmt
                                     FROM T_EmpPayrollDtl 
                                     WHERE Tpd_Paycycle = '{3}'
                                     GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_SPLHOLAmt <>  ISNULL(Tpd_SPLHOLAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL    

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_SPLHOLOTAmt
                            , Tpd_SPLHOLOTAmt
                            , 'Summary and Details of SPLHOLOT Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN ( SELECT Tpd_IDNo, SUM(Tpd_SPLHOLOTAmt) as Tpd_SPLHOLOTAmt
                                     FROM T_EmpPayrollDtl 
                                     WHERE Tpd_Paycycle = '{3}'
                                     GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_SPLHOLOTAmt <>  ISNULL(Tpd_SPLHOLOTAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL 

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_SPLHOLNDAmt
                            , Tpd_SPLHOLNDAmt
                            , 'Summary and Details of SPLHOLND Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN ( SELECT Tpd_IDNo, SUM(Tpd_SPLHOLNDAmt) as Tpd_SPLHOLNDAmt
                                     FROM T_EmpPayrollDtl 
                                     WHERE Tpd_Paycycle = '{3}'
                                     GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_SPLHOLNDAmt <>  ISNULL(Tpd_SPLHOLNDAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL 

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_SPLHOLNDOTAmt
                            , Tpd_SPLHOLNDOTAmt
                            , 'Summary and Details of SPLHOLNDOT Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN ( SELECT Tpd_IDNo, SUM(Tpd_SPLHOLNDOTAmt) as Tpd_SPLHOLNDOTAmt
                                     FROM T_EmpPayrollDtl 
                                     WHERE Tpd_Paycycle = '{3}'
                                     GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_SPLHOLNDOTAmt <>  ISNULL(Tpd_SPLHOLNDOTAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_PSDAmt
                            , Tpd_PSDAmt
                            , 'Summary and Details of PSD Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN ( SELECT Tpd_IDNo, SUM(Tpd_PSDAmt) as Tpd_PSDAmt
                                     FROM T_EmpPayrollDtl 
                                     WHERE Tpd_Paycycle = '{3}'
                                     GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_PSDAmt <>  ISNULL(Tpd_PSDAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_PSDOTAmt
                            , Tpd_PSDOTAmt
                            , 'Summary and Details of PSDOT Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN ( SELECT Tpd_IDNo, SUM(Tpd_PSDOTAmt) as Tpd_PSDOTAmt
                                     FROM T_EmpPayrollDtl 
                                     WHERE Tpd_Paycycle = '{3}'
                                     GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_PSDOTAmt <>  ISNULL(Tpd_PSDOTAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_PSDNDAmt
                            , Tpd_PSDNDAmt
                            , 'Summary and Details of PSDND Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN ( SELECT Tpd_IDNo, SUM(Tpd_PSDNDAmt) as Tpd_PSDNDAmt
                                     FROM T_EmpPayrollDtl 
                                     WHERE Tpd_Paycycle = '{3}'
                                     GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_PSDNDAmt <>  ISNULL(Tpd_PSDNDAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_PSDNDOTAmt
                            , Tpd_PSDNDOTAmt
                            , 'Summary and Details of PSDNDOT Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN ( SELECT Tpd_IDNo, SUM(Tpd_PSDNDOTAmt) as Tpd_PSDNDOTAmt
                                     FROM T_EmpPayrollDtl 
                                     WHERE Tpd_Paycycle = '{3}'
                                     GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_PSDNDOTAmt <>  ISNULL(Tpd_PSDNDOTAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_COMPHOLAmt
                            , Tpd_COMPHOLAmt
                            , 'Summary and Details of COMPHOL Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN ( SELECT Tpd_IDNo, SUM(Tpd_COMPHOLAmt) as Tpd_COMPHOLAmt
                                     FROM T_EmpPayrollDtl 
                                     WHERE Tpd_Paycycle = '{3}'
                                     GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_COMPHOLAmt <>  ISNULL(Tpd_COMPHOLAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_COMPHOLOTAmt
                            , Tpd_COMPHOLOTAmt
                            , 'Summary and Details of COMPHOLOT Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN ( SELECT Tpd_IDNo, SUM(Tpd_COMPHOLOTAmt) as Tpd_COMPHOLOTAmt
                                     FROM T_EmpPayrollDtl 
                                     WHERE Tpd_Paycycle = '{3}'
                                     GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_COMPHOLOTAmt <>  ISNULL(Tpd_COMPHOLOTAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_COMPHOLNDAmt
                            , Tpd_COMPHOLNDAmt
                            , 'Summary and Details of COMPHOLND Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN ( SELECT Tpd_IDNo, SUM(Tpd_COMPHOLNDAmt) as Tpd_COMPHOLNDAmt
                                     FROM T_EmpPayrollDtl 
                                     WHERE Tpd_Paycycle = '{3}'
                                     GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_COMPHOLNDAmt <>  ISNULL(Tpd_COMPHOLNDAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_COMPHOLNDOTAmt
                            , Tpd_COMPHOLNDOTAmt
                            , 'Summary and Details of COMPHOLNDOT Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN ( SELECT Tpd_IDNo, SUM(Tpd_COMPHOLNDOTAmt) as Tpd_COMPHOLNDOTAmt
                                     FROM T_EmpPayrollDtl 
                                     WHERE Tpd_Paycycle = '{3}'
                                     GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_COMPHOLNDOTAmt <>  ISNULL(Tpd_COMPHOLNDOTAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_RESTLEGHOLAmt
                            , Tpd_RESTLEGHOLAmt
                            , 'Summary and Details of RESTLEGHOL Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN ( SELECT Tpd_IDNo, SUM(Tpd_RESTLEGHOLAmt) as Tpd_RESTLEGHOLAmt
                                     FROM T_EmpPayrollDtl 
                                     WHERE Tpd_Paycycle = '{3}'
                                     GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_RESTLEGHOLAmt <>  ISNULL(Tpd_RESTLEGHOLAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_RESTLEGHOLOTAmt
                            , Tpd_RESTLEGHOLOTAmt
                            , 'Summary and Details of RESTLEGHOLOT Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN ( SELECT Tpd_IDNo, SUM(Tpd_RESTLEGHOLOTAmt) as Tpd_RESTLEGHOLOTAmt
                                     FROM T_EmpPayrollDtl 
                                     WHERE Tpd_Paycycle = '{3}'
                                     GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_RESTLEGHOLOTAmt <>  ISNULL(Tpd_RESTLEGHOLOTAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_RESTLEGHOLNDAmt
                            , Tpd_RESTLEGHOLNDAmt
                            , 'Summary and Details of RESTLEGHOLND Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN ( SELECT Tpd_IDNo, SUM(Tpd_RESTLEGHOLNDAmt) as Tpd_RESTLEGHOLNDAmt
                                     FROM T_EmpPayrollDtl 
                                     WHERE Tpd_Paycycle = '{3}'
                                     GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_RESTLEGHOLNDAmt <>  ISNULL(Tpd_RESTLEGHOLNDAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_RESTLEGHOLNDOTAmt
                            , Tpd_RESTLEGHOLNDOTAmt
                            , 'Summary and Details of RESTLEGHOLNDOT Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN ( SELECT Tpd_IDNo, SUM(Tpd_RESTLEGHOLNDOTAmt) as Tpd_RESTLEGHOLNDOTAmt
                                     FROM T_EmpPayrollDtl 
                                     WHERE Tpd_Paycycle = '{3}'
                                     GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_RESTLEGHOLNDOTAmt <>  ISNULL(Tpd_RESTLEGHOLNDOTAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS
    
                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_RESTSPLHOLAmt
                            , Tpd_RESTSPLHOLAmt
                            , 'Summary and Details of RESTSPLHOL Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN ( SELECT Tpd_IDNo, SUM(Tpd_RESTSPLHOLAmt) as Tpd_RESTSPLHOLAmt
                                     FROM T_EmpPayrollDtl 
                                     WHERE Tpd_Paycycle = '{3}'
                                     GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_RESTSPLHOLAmt <>  ISNULL(Tpd_RESTSPLHOLAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_RESTSPLHOLOTAmt
                            , Tpd_RESTSPLHOLOTAmt
                            , 'Summary and Details of RESTSPLHOLOT Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN ( SELECT Tpd_IDNo, SUM(Tpd_RESTSPLHOLOTAmt) as Tpd_RESTSPLHOLOTAmt
                                     FROM T_EmpPayrollDtl 
                                     WHERE Tpd_Paycycle = '{3}'
                                     GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_RESTSPLHOLOTAmt <>  ISNULL(Tpd_RESTSPLHOLOTAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_RESTSPLHOLNDAmt
                            , Tpd_RESTSPLHOLNDAmt
                            , 'Summary and Details of RESTSPLHOLND Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN ( SELECT Tpd_IDNo, SUM(Tpd_RESTSPLHOLNDAmt) as Tpd_RESTSPLHOLNDAmt
                                     FROM T_EmpPayrollDtl 
                                     WHERE Tpd_Paycycle = '{3}'
                                     GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_RESTSPLHOLNDAmt <>  ISNULL(Tpd_RESTSPLHOLNDAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_RESTSPLHOLNDOTAmt
                            , Tpd_RESTSPLHOLNDOTAmt
                            , 'Summary and Details of RESTSPLHOLNDOT Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN ( SELECT Tpd_IDNo, SUM(Tpd_RESTSPLHOLNDOTAmt) as Tpd_RESTSPLHOLNDOTAmt
                                     FROM T_EmpPayrollDtl 
                                     WHERE Tpd_Paycycle = '{3}'
                                     GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_RESTSPLHOLNDOTAmt <>  ISNULL(Tpd_RESTSPLHOLNDOTAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_RESTCOMPHOLAmt
                            , Tpd_RESTCOMPHOLAmt
                            , 'Summary and Details of RESTCOMPHOL Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN ( SELECT Tpd_IDNo, SUM(Tpd_RESTCOMPHOLAmt) as Tpd_RESTCOMPHOLAmt
                                     FROM T_EmpPayrollDtl 
                                     WHERE Tpd_Paycycle = '{3}'
                                     GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_RESTCOMPHOLAmt <>  ISNULL(Tpd_RESTCOMPHOLAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_RESTCOMPHOLOTAmt
                            , Tpd_RESTCOMPHOLOTAmt
                            , 'Summary and Details of RESTCOMPHOLOT Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN ( SELECT Tpd_IDNo, SUM(Tpd_RESTCOMPHOLOTAmt) as Tpd_RESTCOMPHOLOTAmt
                                     FROM T_EmpPayrollDtl 
                                     WHERE Tpd_Paycycle = '{3}'
                                     GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_RESTCOMPHOLOTAmt <>  ISNULL(Tpd_RESTCOMPHOLOTAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_RESTCOMPHOLNDAmt
                            , Tpd_RESTCOMPHOLNDAmt
                            , 'Summary and Details of RESTCOMPHOLND Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN ( SELECT Tpd_IDNo, SUM(Tpd_RESTCOMPHOLNDAmt) as Tpd_RESTCOMPHOLNDAmt
                                     FROM T_EmpPayrollDtl 
                                     WHERE Tpd_Paycycle = '{3}'
                                     GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_RESTCOMPHOLNDAmt <>  ISNULL(Tpd_RESTCOMPHOLNDAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_RESTCOMPHOLNDOTAmt
                            , Tpd_RESTCOMPHOLNDOTAmt
                            , 'Summary and Details of RESTCOMPHOLNDOT Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN ( SELECT Tpd_IDNo, SUM(Tpd_RESTCOMPHOLNDOTAmt) as Tpd_RESTCOMPHOLNDOTAmt
                                     FROM T_EmpPayrollDtl 
                                     WHERE Tpd_Paycycle = '{3}'
                                     GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_RESTCOMPHOLNDOTAmt <>  ISNULL(Tpd_RESTCOMPHOLNDOTAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS                        

                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_RESTPSDAmt
                            , Tpd_RESTPSDAmt
                            , 'Summary and Details of RESTPSD Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN ( SELECT Tpd_IDNo, SUM(Tpd_RESTPSDAmt) as Tpd_RESTPSDAmt
                                     FROM T_EmpPayrollDtl 
                                     WHERE Tpd_Paycycle = '{3}'
                                     GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_RESTPSDAmt <>  ISNULL(Tpd_RESTPSDAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_RESTPSDOTAmt
                            , Tpd_RESTPSDOTAmt
                            , 'Summary and Details of RESTPSDOT Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN ( SELECT Tpd_IDNo, SUM(Tpd_RESTPSDOTAmt) as Tpd_RESTPSDOTAmt
                                     FROM T_EmpPayrollDtl 
                                     WHERE Tpd_Paycycle = '{3}'
                                     GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_RESTPSDOTAmt <>  ISNULL(Tpd_RESTPSDOTAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_RESTPSDNDAmt
                            , Tpd_RESTPSDNDAmt
                            , 'Summary and Details of RESTPSDND Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN ( SELECT Tpd_IDNo, SUM(Tpd_RESTPSDNDAmt) as Tpd_RESTPSDNDAmt
                                     FROM T_EmpPayrollDtl 
                                     WHERE Tpd_Paycycle = '{3}'
                                     GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_RESTPSDNDAmt <>  ISNULL(Tpd_RESTPSDNDAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AE'
                            , '{3}'
                            , Tpy_RESTPSDNDOTAmt
                            , Tpd_RESTPSDNDOTAmt
                            , 'Summary and Details of RESTPSDNDOT Amount do not match' as Remarks
                            , 1
                            , '{4}'
                            , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN ( SELECT Tpd_IDNo, SUM(Tpd_RESTPSDNDOTAmt) as Tpd_RESTPSDNDOTAmt
                                     FROM T_EmpPayrollDtl 
                                     WHERE Tpd_Paycycle = '{3}'
                                     GROUP BY Tpd_IDNo) Details ON Tpd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                        AND Tpy_RESTPSDNDOTAmt <>  ISNULL(Tpd_RESTPSDNDOTAmt, 0)
                        AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL     

                    SELECT '{5}'
                            , Tpy_IDNo
                            , 'AW'
                            , '{3}'
                            , Tpy_TotalTaxableIncomeAmt
		                    , Tpy_TotalREGAmt + Tpy_REGOTAmt + Tpy_REGNDAmt + Tpy_REGNDOTAmt + Tpy_RESTAmt + Tpy_RESTOTAmt + Tpy_RESTNDAmt 
		                    + Tpy_RESTNDOTAmt + Tpy_LEGHOLAmt + Tpy_LEGHOLOTAmt + Tpy_LEGHOLNDAmt + Tpy_LEGHOLNDOTAmt + Tpy_SPLHOLAmt + Tpy_SPLHOLOTAmt
		                    + Tpy_SPLHOLNDAmt + Tpy_SPLHOLNDOTAmt + Tpy_PSDAmt + Tpy_PSDOTAmt + Tpy_PSDNDAmt + Tpy_PSDNDOTAmt + Tpy_COMPHOLAmt
		                    + Tpy_COMPHOLOTAmt + Tpy_COMPHOLNDAmt + Tpy_COMPHOLNDOTAmt + Tpy_RESTLEGHOLAmt + Tpy_RESTLEGHOLOTAmt + Tpy_RESTLEGHOLNDAmt
		                    + Tpy_RESTLEGHOLNDOTAmt + Tpy_RESTSPLHOLAmt + Tpy_RESTSPLHOLOTAmt + Tpy_RESTSPLHOLNDAmt + Tpy_RESTSPLHOLNDOTAmt
		                    + Tpy_RESTCOMPHOLAmt + Tpy_RESTCOMPHOLOTAmt + Tpy_RESTCOMPHOLNDAmt + Tpy_RESTCOMPHOLNDOTAmt + Tpy_RESTPSDAmt
		                    + Tpy_RESTPSDOTAmt + Tpy_RESTPSDNDAmt + Tpy_RESTPSDNDOTAmt + Tpy_TotalAdjAmt + Tpy_TaxableIncomeAmt 
		                    + ISNULL(Tpm_Misc1Amt, 0) + ISNULL(Tpm_Misc1OTAmt, 0) + ISNULL(Tpm_Misc1NDAmt, 0) + ISNULL(Tpm_Misc1NDOTAmt, 0) 
		                    + ISNULL(Tpm_Misc2Amt, 0) + ISNULL(Tpm_Misc2OTAmt, 0) + ISNULL(Tpm_Misc2NDAmt, 0) + ISNULL(Tpm_Misc2NDOTAmt, 0) 
		                    + ISNULL(Tpm_Misc3Amt, 0) + ISNULL(Tpm_Misc3OTAmt, 0) + ISNULL(Tpm_Misc3NDAmt, 0) + ISNULL(Tpm_Misc3NDOTAmt, 0) 
		                    + ISNULL(Tpm_Misc4Amt, 0) + ISNULL(Tpm_Misc4OTAmt, 0) + ISNULL(Tpm_Misc4NDAmt, 0) + ISNULL(Tpm_Misc4NDOTAmt, 0) 
		                    + ISNULL(Tpm_Misc5Amt, 0) + ISNULL(Tpm_Misc5OTAmt, 0) + ISNULL(Tpm_Misc5NDAmt, 0) + ISNULL(Tpm_Misc5NDOTAmt, 0) 
		                    + ISNULL(Tpm_Misc6Amt, 0) + ISNULL(Tpm_Misc6OTAmt, 0) + ISNULL(Tpm_Misc6NDAmt, 0) + ISNULL(Tpm_Misc6NDOTAmt, 0)
		                    , 'Summary and Details of Total Taxable Income do not match' as Remarks
		                    , 1
	                        , '{4}'
	                        , GETDATE()
                    FROM T_EmpPayroll
                    LEFT JOIN T_EmpPayrollMisc ON Tpm_IDNo = Tpy_IDNo
	                    AND Tpm_PayCycle = Tpy_PayCycle
                    @USERCOSTCENTERACCESSCONDITION
                    WHERE  Tpy_PayCycle = '{3}'
	                    AND  Tpy_TotalTaxableIncomeAmt <> Tpy_TotalREGAmt + Tpy_REGOTAmt + Tpy_REGNDAmt + Tpy_REGNDOTAmt + Tpy_RESTAmt + Tpy_RESTOTAmt + Tpy_RESTNDAmt 
		                    + Tpy_RESTNDOTAmt + Tpy_LEGHOLAmt + Tpy_LEGHOLOTAmt + Tpy_LEGHOLNDAmt + Tpy_LEGHOLNDOTAmt + Tpy_SPLHOLAmt + Tpy_SPLHOLOTAmt
		                    + Tpy_SPLHOLNDAmt + Tpy_SPLHOLNDOTAmt + Tpy_PSDAmt + Tpy_PSDOTAmt + Tpy_PSDNDAmt + Tpy_PSDNDOTAmt + Tpy_COMPHOLAmt
		                    + Tpy_COMPHOLOTAmt + Tpy_COMPHOLNDAmt + Tpy_COMPHOLNDOTAmt + Tpy_RESTLEGHOLAmt + Tpy_RESTLEGHOLOTAmt + Tpy_RESTLEGHOLNDAmt
		                    + Tpy_RESTLEGHOLNDOTAmt + Tpy_RESTSPLHOLAmt + Tpy_RESTSPLHOLOTAmt + Tpy_RESTSPLHOLNDAmt + Tpy_RESTSPLHOLNDOTAmt
		                    + Tpy_RESTCOMPHOLAmt + Tpy_RESTCOMPHOLOTAmt + Tpy_RESTCOMPHOLNDAmt + Tpy_RESTCOMPHOLNDOTAmt + Tpy_RESTPSDAmt
		                    + Tpy_RESTPSDOTAmt + Tpy_RESTPSDNDAmt + Tpy_RESTPSDNDOTAmt + Tpy_TotalAdjAmt + Tpy_TaxableIncomeAmt + ISNULL(Tpm_Misc1Amt, 0)
		                    + ISNULL(Tpm_Misc1OTAmt, 0) + ISNULL(Tpm_Misc1NDAmt, 0) + ISNULL(Tpm_Misc1NDOTAmt, 0) + ISNULL(Tpm_Misc2Amt, 0)
		                    + ISNULL(Tpm_Misc2OTAmt, 0) + ISNULL(Tpm_Misc2NDAmt, 0) + ISNULL(Tpm_Misc2NDOTAmt, 0) + ISNULL(Tpm_Misc3Amt, 0)
		                    + ISNULL(Tpm_Misc3OTAmt, 0) + ISNULL(Tpm_Misc3NDAmt, 0) + ISNULL(Tpm_Misc3NDOTAmt, 0) + ISNULL(Tpm_Misc4Amt, 0)
		                    + ISNULL(Tpm_Misc4OTAmt, 0) + ISNULL(Tpm_Misc4NDAmt, 0) + ISNULL(Tpm_Misc4NDOTAmt, 0) + ISNULL(Tpm_Misc5Amt, 0)
		                    + ISNULL(Tpm_Misc5OTAmt, 0) + ISNULL(Tpm_Misc5NDAmt, 0) + ISNULL(Tpm_Misc5NDOTAmt, 0) + ISNULL(Tpm_Misc6Amt, 0)
		                    + ISNULL(Tpm_Misc6OTAmt, 0) + ISNULL(Tpm_Misc6NDAmt, 0) + ISNULL(Tpm_Misc6NDOTAmt, 0)
                     AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                     @CONDITIONS	

                     UNION ALL

                    SELECT '{5}'
                            ,Tpy_IDNo
                            ,'AW'
                            , '{3}'
		                    , Tpy_TaxableIncomeAmt
		                    , DtlAmt
		                    , 'Summary and Details of Other Taxable Income do not match'
		                    , 1
	                        , '{4}'
	                        , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN (SELECT Tin_IDNo, SUM(Tin_IncomeAmt) as DtlAmt
			                    FROM T_EmpIncome
			                    INNER JOIN {0}..M_Income ON Min_IncomeCode = Tin_IncomeCode
				                    AND Min_TaxClass = 'T'
                                    AND Min_CompanyCode = '{1}'
			                    WHERE Tin_PayCycle = '{3}'
				                    AND Tin_PostFlag = 1
			                    GROUP BY Tin_IDNo) TaxIncome ON Tin_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
	                    AND Tpy_TaxableIncomeAmt <> ISNULL(DtlAmt, 0)
	                     AND ('{2}' = '' OR Tpy_IDNo = '{2}')
	                     @CONDITIONS	

                    UNION ALL

                    SELECT  '{5}'
                            ,Tpy_IDNo
                            ,'AW'
                            , '{3}'
                            , Tpy_NontaxableIncomeAmt
		                    , DtlAmt
		                    , 'Summary and Details of Nontaxable Income do not match'
		                    , 1
	                        , '{4}'
	                        , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN (SELECT Tin_IDNo, SUM(Tin_IncomeAmt) as DtlAmt
			                    FROM T_EmpIncome
			                    INNER JOIN {0}..M_Income ON Min_IncomeCode = Tin_IncomeCode
				                    AND Min_TaxClass = 'N'
                                    AND Min_CompanyCode = '{1}'
			                    WHERE Tin_PayCycle = '{3}'
				                    AND Tin_PostFlag = 1
			                    GROUP BY Tin_IDNo) TaxIncome ON Tin_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
	                    AND Tpy_NontaxableIncomeAmt <>  ISNULL(DtlAmt, 0)
	                     AND ('{2}' = '' OR Tpy_IDNo = '{2}')
	                     @CONDITIONS	

                    UNION ALL

                    SELECT '{5}'
                            ,Tpy_IDNo
                            ,'AW'
                            , '{3}'
		                    , Tpy_OtherDeductionAmt
		                    , DtlAmt
		                    , 'Summary and Details of Other Deduction do not match'
		                    , 1
	                        , '{4}'
	                        , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    LEFT JOIN (SELECT Tdd_IDNo, SUM(Tdd_Amount) as DtlAmt
			                    FROM T_EmpDeductionDtl
			                    WHERE Tdd_ThisPayCycle = '{3}'
			                    AND Tdd_PaymentFlag = 1
			                    AND Tdd_PaymentType = 'P'
			                    GROUP BY Tdd_IDNo) Deduction ON Tdd_IDNo = Tpy_IDNo
                    WHERE  Tpy_PayCycle = '{3}'
                    AND Tpy_OtherDeductionAmt <>  ISNULL(DtlAmt, 0)
	                     AND ('{2}' = '' OR Tpy_IDNo = '{2}')
	                     @CONDITIONS	

                    UNION ALL

                    SELECT '{5}'
                            ,Tpy_IDNo
                            ,'AE'
                            , '{3}'
		                    , Tpy_TotalDeductionAmt
		                    , Tpy_SSSEE + Tpy_MPFEE + Tpy_PhilhealthEE + Tpy_PagIbigEE + Tpy_PagIbigTaxEE + CASE WHEN Tpy_WtaxAmt > 0 THEN Tpy_WtaxAmt ELSE 0 END + Tpy_OtherDeductionAmt
		                    , 'Summary and Details of Total Deduction do not match'
		                    , 1
	                        , '{4}'
	                        , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    WHERE  Tpy_PayCycle = '{3}'
	                    AND Tpy_TotalDeductionAmt <> Tpy_SSSEE + Tpy_MPFEE + Tpy_PhilhealthEE + Tpy_PagIbigEE + Tpy_PagIbigTaxEE + CASE WHEN Tpy_WtaxAmt > 0 THEN Tpy_WtaxAmt ELSE 0 END + Tpy_OtherDeductionAmt
	                     AND ('{2}' = '' OR Tpy_IDNo = '{2}')
	                     @CONDITIONS	

                    UNION ALL

                    SELECT '{5}'
                            ,Tpy_IDNo
                            ,'AW'
                            , '{3}'
		                    , Tpy_NetAmt
		                    , Tpy_GrossIncomeAmt - Tpy_TotalDeductionAmt 
		                    , 'Summary and Details of Net Pay do not match'
		                    , 1
	                        , '{4}'
	                        , GETDATE()
                    FROM T_EmpPayroll
                    @USERCOSTCENTERACCESSCONDITION
                    WHERE  Tpy_PayCycle = '{3}'
	                    AND Tpy_NetAmt <> Tpy_GrossIncomeAmt - Tpy_TotalDeductionAmt 
	                    AND ('{2}' = '' OR Tpy_IDNo = '{2}')
	                    @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                            ,Tpy_IDNo
                            ,'AE'
                            , '{3}'
		                    , Tpd_ABSAmt
		                    , Tpd_LTAmt +  Tpd_UTAmt + Tpd_UPLVAmt + Tpd_ABSLEGHOLAmt + Tpd_ABSSPLHOLAmt + Tpd_ABSCOMPHOLAmt +
		                      Tpd_ABSPSDAmt + Tpd_ABSOTHHOLAmt + Tpd_WDABSAmt + Tpd_LTUTMaxAmt
		                    , 'Total Absent Amount on '+ CONVERT(CHAR(10), tpd_date, 101) + ' does not coincide with absent details' 
		                    , 1
	                        , '{4}'
	                        , GETDATE()
                    FROM T_EmpPayrollDtl
                    INNER JOIN T_EmpPayroll ON Tpy_IDNo = Tpd_IDNo
                        AND Tpy_PayCycle = Tpd_PayCycle
                    @USERCOSTCENTERACCESSCONDITION
                    WHERE  Tpy_PayCycle = '{3}'
	                    AND (Tpd_ABSAmt <> Tpd_LTAmt +  Tpd_UTAmt + Tpd_UPLVAmt + Tpd_ABSLEGHOLAmt + Tpd_ABSSPLHOLAmt + Tpd_ABSCOMPHOLAmt +
	                    Tpd_ABSPSDAmt + Tpd_ABSOTHHOLAmt + Tpd_WDABSAmt + Tpd_LTUTMaxAmt)
                        AND Tpd_ABSAmt - (Tpd_LTAmt +  Tpd_UTAmt + Tpd_UPLVAmt + Tpd_ABSLEGHOLAmt + Tpd_ABSSPLHOLAmt + Tpd_ABSCOMPHOLAmt +
	                    Tpd_ABSPSDAmt + Tpd_ABSOTHHOLAmt + Tpd_WDABSAmt + Tpd_LTUTMaxAmt) <> 0.01
                        AND Tpy_RegRule = 'A'
	                    AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                        @CONDITIONS

                    UNION ALL

                    SELECT '{5}'
                            , Tpy_IDNo
                            ,'AE'
                            , '{3}'
		                    , Tpd_TotalREGAmt
		                    , Tpd_REGAmt + Tpd_PDLVAmt + Tpd_PDLEGHOLAmt + Tpd_PDSPLHOLAmt + Tpd_PDCOMPHOLAmt + Tpd_PDPSDAmt +
		                      Tpd_PDOTHHOLAmt + Tpd_PDRESTLEGHOLAmt 
		                    , 'Total Regular Pay on '+ CONVERT(CHAR(10), tpd_date, 101) + ' does not coincide with regular details' 
		                    , 1
	                        , '{4}'
	                        , GETDATE()
                    FROM T_EmpPayrollDtl
                    INNER JOIN T_EmpPayroll ON Tpy_IDNo = Tpd_IDNo
                        AND Tpy_PayCycle = Tpd_PayCycle
                    @USERCOSTCENTERACCESSCONDITION
                    WHERE  Tpy_PayCycle = '{3}'
	                    AND (Tpd_TotalREGAmt <> Tpd_REGAmt + Tpd_PDLVAmt + Tpd_PDLEGHOLAmt + Tpd_PDSPLHOLAmt + Tpd_PDCOMPHOLAmt + Tpd_PDPSDAmt +
		                    Tpd_PDOTHHOLAmt + Tpd_PDRESTLEGHOLAmt )
                        AND Tpd_TotalREGAmt - ( Tpd_REGAmt + Tpd_PDLVAmt + Tpd_PDLEGHOLAmt + Tpd_PDSPLHOLAmt + Tpd_PDCOMPHOLAmt + Tpd_PDPSDAmt +
		                    Tpd_PDOTHHOLAmt + Tpd_PDRESTLEGHOLAmt ) <> 0.01
                        AND Tpy_RegRule = 'P'
	                    AND ('{2}' = '' OR Tpy_IDNo = '{2}')		
                        @CONDITIONS
                        "
           , centralProfile, companyCode, EmployeeID, PayCycle, UserLogin, menucode);
           #endregion
           query = query.Replace("@CONDITIONS", condition);
           query = query.Replace("@USERCOSTCENTERACCESSCONDITION", (new HRCReportsBL()).UserCostCenterAccessTmpQuery(ProfileCode, "PAYROLL", UserLogin, "Tpy_CostcenterCode", "Tpy_PayrollGroup", "Tpy_EmploymentStatus", "Tpy_PayrollType", companyCode, centralProfile, false));
           dalHelper.ExecuteNonQuery(query);
        }

        public void PostProcessingUpdatingInPayroll(string ProfileCode, string centralProfile, string PayCycle, string EmployeeID, string CostCenter, string UserLogin, string companyCode, string menucode, DALHelper dalHelper)
        {
            #region Query
            string condition = string.Empty;
            if (CostCenter != string.Empty)
            {
                condition += string.Format("And Tpy_CostcenterCode IN ({0})", (new CommonBL()).EncodeFilterItems(CostCenter, true));
            }
            string query = string.Format(@"
                                    UPDATE M_Employee SET Mem_IsMultipleSalary = Tpy_IsMultipleSalary
                                        , Mem_UpdatedBy = '{4}'
                                        , Mem_UpdatedDate = GETDATE()
                                    FROM M_Employee 
                                    INNER JOIN T_EmpPayroll 
                                    ON Mem_IDNo = Tpy_IDNo
                                    @USERCOSTCENTERACCESSCONDITION
                                    WHERE Tpy_PayCycle = '{3}'
	                                    AND ('{2}' = '' OR Tpy_IDNo = '{2}')
	                                @CONDITIONS	
                                    "
                                    , centralProfile, companyCode, EmployeeID, PayCycle, UserLogin);
            #endregion
            query = query.Replace("@CONDITIONS", condition);
            query = query.Replace("@USERCOSTCENTERACCESSCONDITION", (new HRCReportsBL()).UserCostCenterAccessTmpQuery(ProfileCode, "PAYROLL", UserLogin, "Tpy_CostcenterCode", "Tpy_PayrollGroup", "Tpy_EmploymentStatus", "Tpy_PayrollType", companyCode, centralProfile, false));
            dalHelper.ExecuteNonQuery(query);
        }

        public DataTable PayrollExceptionList(string ProfileCode, string CentralProfile, string PayCycle, string EmployeeID, string CostCenter, string UserLogin, string CompanyCode, string ErrorType, string menucode, DALHelper dalHelper)
        {
            #region Query
            string condition = string.Empty;
            if (CostCenter != string.Empty)
            {
                condition += string.Format("And Mem_CostcenterCode IN ({0})", (new CommonBL()).EncodeFilterItems(CostCenter, true));
            }
            if (ErrorType != string.Empty)
            {
                condition += string.Format(@"
                                            And Tpc_Type LIKE '%{0}'", ErrorType);
            }
            string query = string.Format(@"
             SELECT CASE WHEN Tpc_Type IN ('AW','BW') THEN 'WARNING' 
                        WHEN Tpc_Type IN ('AE','BE') THEN 'ERROR' 
                   END as [Type]
                                    , Tpc_IDNo AS [ID Number] 
									, Mem_LastName + CASE WHEN LEN(ISNULL(Mem_ExtensionName,'')) > 0 
										THEN  ' ' + Mem_ExtensionName ELSE '' END as [Last Name]
									, Mem_FirstName as [First Name]
									, Mem_MiddleName as [Middle Name]
                                    , Tpc_NumValue1 AS [Summary Amount]
                                    , Tpc_NumValue2 AS [Detail Amount]
                                    , Tpc_Remarks AS [Remarks]
                                    , CASE WHEN Mem_IsComputedPayroll = 1 THEN 'Included in Payroll'
                                            ELSE 'Excluded from Payroll' END AS [Payroll Status]
                                    FROM (SELECT * FROM T_EmpProcessCheck 
                                          UNION ALL
                                          SELECT * FROM T_EmpProcessCheckHst ) Tmp
									INNER JOIN M_Employee ON Mem_IDNo = Tpc_IDNo
                                    @USERCOSTCENTERACCESSCONDITION   
                                    WHERE Tpc_ModuleCode = '{2}'
                                    AND Tpc_PayCycle = '{0}'
                                    AND ('{1}' = '' OR Tpc_IDNo = '{1}')
                                    @CONDITIONS	
            "
           , PayCycle, EmployeeID, menucode);
            #endregion
            query = query.Replace("@CONDITIONS", condition);
            query = query.Replace("@USERCOSTCENTERACCESSCONDITION", (new HRCReportsBL()).UserCostCenterAccessTmpQuery(ProfileCode, "PAYROLL", UserLogin, "Mem_CostcenterCode", "Mem_PayrollGroup", "Mem_EmploymentStatusCode", "Mem_PayrollType", CompanyCode, CentralProfile, false));
            DataTable dtResult;
            dtResult = dalHelper.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public void BackUpMasterTables(string CompanyCode, string CentralProfile, DALHelper dal)
        {
            string query = string.Format(@"
                    DELETE FROM B_Day
                    INSERT INTO B_Day
                    SELECT * FROM {0}..M_Day
                    WHERE Mdy_CompanyCode = '{1}'

                    DELETE FROM B_DayPremium
                    INSERT INTO B_DayPremium
                    SELECT * FROM {0}..M_DayPremium
                    WHERE Mdp_CompanyCode = '{1}'

                    DELETE FROM B_Deduction
                    INSERT INTO B_Deduction
                    SELECT * FROM {0}..M_Deduction
                    WHERE Mdn_CompanyCode = '{1}'

                    DELETE FROM B_GovRemittance
                    INSERT INTO B_GovRemittance
                    SELECT * FROM {0}..M_GovRemittance
                    WHERE Mgr_CompanyCode = '{1}'

                    DELETE FROM B_Income
                    INSERT INTO B_Income
                    SELECT * FROM {0}..M_Income
                    WHERE Min_CompanyCode = '{1}'

                    DELETE FROM B_Leave
                    INSERT INTO B_Leave
                    SELECT * FROM {0}..M_Leave
                    WHERE Mlv_CompanyCode = '{1}'

                    DELETE FROM B_MiscellaneousDay
                    INSERT INTO B_MiscellaneousDay
                    SELECT * FROM {0}..M_MiscellaneousDay
                    WHERE Mmd_CompanyCode = '{1}'

                    DELETE FROM B_PolicyDtl
                    INSERT INTO B_PolicyDtl
                    SELECT * FROM {0}..M_PolicyDtl
                    WHERE Mpd_CompanyCode = '{1}'

                    DELETE FROM B_PolicyHdr
                    INSERT INTO B_PolicyHdr
                    SELECT * FROM {0}..M_PolicyHdr
                    WHERE Mph_CompanyCode = '{1}'

                    DELETE FROM B_PremiumSchedule
                    INSERT INTO B_PremiumSchedule
                    SELECT * FROM {0}..M_PremiumSchedule
                    WHERE Mps_CompanyCode = '{1}'

                    DELETE FROM B_Shift
                    INSERT INTO B_Shift
                    SELECT * FROM {0}..M_Shift
                    WHERE Msh_CompanyCode = '{1}'

                    DELETE FROM B_Tax
                    INSERT INTO B_Tax
                    SELECT * FROM {0}..M_Tax
                    WHERE Mtx_CompanyCode = '{1}'

                    DELETE FROM B_TaxScheduleDtl
                    INSERT INTO B_TaxScheduleDtl
                    SELECT * FROM {0}..M_TaxScheduleDtl
                    WHERE Mtd_CompanyCode = '{1}'

                    DELETE FROM B_TaxScheduleHdr
                    INSERT INTO B_TaxScheduleHdr
                    SELECT * FROM {0}..M_TaxScheduleHdr
                    WHERE Mth_CompanyCode = '{1}'

                    DELETE FROM B_EmpAssignmentCycle
                    INSERT INTO B_EmpAssignmentCycle
                    SELECT T_EmpAssignmentCycle.* FROM {0}..T_EmpAssignmentCycle
                    INNER JOIN M_Employee ON Mem_IDNo = Tac_IDNo

                    DELETE FROM B_EmpAssignmentDate
                    INSERT INTO B_EmpAssignmentDate
                    SELECT T_EmpAssignmentDate.* FROM {0}..T_EmpAssignmentDate
                    INNER JOIN M_Employee ON Mem_IDNo = Tac_IDNo

                    DELETE FROM B_EmpCostcenter
                    INSERT INTO B_EmpCostcenter
                    SELECT T_EmpCostcenter.* FROM {0}..T_EmpCostcenter
                    INNER JOIN M_Employee ON Mem_IDNo = Tcc_IDNo

                    DELETE FROM B_EmpCompanyProfilePayGrp
                    INSERT INTO B_EmpCompanyProfilePayGrp
                    SELECT T_EmpCompanyProfilePayGrp.* FROM {0}..T_EmpCompanyProfilePayGrp
                    INNER JOIN M_Employee ON Mem_IDNo = Tec_IDNo                    

                    DELETE FROM B_EmpDeductionHdr
                    INSERT INTO B_EmpDeductionHdr
                    SELECT T_EmpDeductionHdr.* FROM {0}..T_EmpDeductionHdr
                    INNER JOIN M_Employee ON Mem_IDNo = Tdh_IDNo

                    DELETE FROM B_EmpGovLoanRemittance
                    INSERT INTO B_EmpGovLoanRemittance
                    SELECT T_EmpGovLoanRemittance.* FROM {0}..T_EmpGovLoanRemittance
                    INNER JOIN M_Employee ON Mem_IDNo = Tgl_IDNo

                    DELETE FROM B_EmpGovPremRemittance
                    INSERT INTO B_EmpGovPremRemittance
                    SELECT T_EmpGovPremRemittance.* FROM {0}..T_EmpGovPremRemittance
                    INNER JOIN M_Employee ON Mem_IDNo = Tgp_IDNo

                    DELETE FROM B_EmpLeaveLdg
                    INSERT INTO B_EmpLeaveLdg
                    SELECT T_EmpLeaveLdg.* FROM {0}..T_EmpLeaveLdg
                    INNER JOIN M_Employee ON Mem_IDNo = Tll_IDNo

                    DELETE FROM B_EmpPosition
                    INSERT INTO B_EmpPosition
                    SELECT T_EmpPosition.* FROM {0}..T_EmpPosition
                    INNER JOIN M_Employee ON Mem_IDNo = Tpo_IDNo

                    DELETE FROM B_EmpSalary
                    INSERT INTO B_EmpSalary
                    SELECT T_EmpSalary.* FROM {0}..T_EmpSalary
                    INNER JOIN M_Employee ON Mem_IDNo = Tsl_IDNo

                    DELETE FROM B_EmpYTD
                    INSERT INTO B_EmpYTD
                    SELECT T_EmpYTD.* FROM {0}..T_EmpYTD
                    INNER JOIN M_Employee ON Mem_IDNo = Tyt_IDNo

                    DELETE FROM B_Holiday
                    INSERT INTO B_Holiday
                    SELECT * FROM {0}..T_Holiday
                    WHERE Thl_CompanyCode = '{1}'
                    ", CentralProfile, CompanyCode);

            dal.ExecuteNonQuery(query);
        }

        public void ClearPayrollTables(bool ProcessAll, string PayPeriod, DALHelper dalHelper)
        {
            this.dal = dalHelper;
            ClearPayrollTables(ProcessAll, false, "", PayPeriod);
        }
        public void ClearPayrollTables(bool ProcessAll, bool ProcessSeparated, string EmployeeId, string PayPeriod)
        {
                string query = @"DELETE FROM {0} WHERE Tpy_PayCycle = '{6}' {3}
                                 DELETE FROM {2} WHERE Tpm_PayCycle = '{6}' {7} 
                                 DELETE FROM {5} WHERE Tpy_PayCycle = '{6}' {3}
                                 DELETE FROM {9} WHERE Tdh_PayCycle = '{6}' {8}
                                 DELETE FROM {1} WHERE Tdd_ThisPayCycle = '{6}' {4}
                                 DELETE FROM {10} WHERE Tpd_PayCycle = '{6}' {12}
                                 DELETE FROM {11} WHERE Tpm_PayCycle = '{6}' {7}
                                ";

                string query2 = @"UPDATE T_EmpSystemAdj
                                  SET Tsa_PostFlag = 0
                                  WHERE Tsa_AdjPayCycle = '{0}'
                                               
                                  UPDATE T_EmpIncome
                                  SET Tin_PostFlag = 0
                                  WHERE Tin_PayCycle = '{0}'

                                  UPDATE T_EmpManualAdj
                                  SET Tma_PostFlag = 0
                                  WHERE Tma_PayCycle = '{0}'
                                  ";

                if (!ProcessAll && EmployeeId != "")
                    query = string.Format(query, EmpPayrollTable
                                                , EmpDeductionDtlTable
                                                , EmpPayrollMiscTable
                                                , " AND Tpy_IDNo = '" + EmployeeId + "'"
                                                , " AND Tdd_IDNo = '" + EmployeeId + "'"
                                                , EmpPayroll2Table
                                                , PayPeriod
                                                , " AND Tpm_IDNo = '" + EmployeeId + "'"
                                                , " AND Tdh_IDNo = '" + EmployeeId + "'"
                                                , EmpDeductionHdrCycleTable
                                                , EmpPayrollDtlTable
                                                , EmpPayrollDtlMiscTable
                                                , " AND Tpd_IDNo = '" + EmployeeId + "'");
                else if (ProcessAll && EmployeeList != "")
                    query = string.Format(query, EmpPayrollTable
                                                , EmpDeductionDtlTable
                                                , EmpPayrollMiscTable
                                                , " AND Tpy_IDNo IN (" + EmployeeList + ") "
                                                , " AND Tdd_IDNo IN (" + EmployeeList + ") "
                                                , EmpPayroll2Table
                                                , PayPeriod
                                                , " AND Tpm_IDNo IN (" + EmployeeList + ") "
                                                , " AND Tdh_IDNo IN (" + EmployeeList + ") "
                                                , EmpDeductionHdrCycleTable
                                                , EmpPayrollDtlTable
                                                , EmpPayrollDtlMiscTable
                                                , " AND Tpd_IDNo IN (" + EmployeeList + ") ");
                else
                {
                    query = string.Format(query, EmpPayrollTable
                                                , EmpDeductionDtlTable
                                                , EmpPayrollMiscTable
                                                , ""
                                                , ""
                                                , EmpPayroll2Table
                                                , PayPeriod
                                                , ""
                                                , ""
                                                , EmpDeductionHdrCycleTable
                                                , EmpPayrollDtlTable
                                                , EmpPayrollDtlMiscTable
                                                , "");


                    query2 = string.Format(query2, PayPeriod);
                    dal.ExecuteNonQuery(query2);
                }

                dal.ExecuteNonQuery(query);
        }

        public void CreateCurrentDeduction(bool ProcessAll, bool ProcessComputed, string EmployeeId)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[4];
            int paramIndex = 0;
            paramInfo[paramIndex++] = new ParameterInfo("@EndCycleDate", PayrollEnd);
            paramInfo[paramIndex++] = new ParameterInfo("@PayPeriod", ProcessPayrollPeriod);
            paramInfo[paramIndex++] = new ParameterInfo("@ApplicPeriod", PayrollCycle);
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_login", LoginUser);

            string strEdlEmployeeIdQuery = "";
            string strEddEmployeeIdQuery = "";
            if (!ProcessAll && EmployeeId != "")
            {
                strEdlEmployeeIdQuery = "AND Tdh_IDNo = '" + EmployeeId + "'";
                strEddEmployeeIdQuery = "AND Tdd_IDNo = '" + EmployeeId + "'";
            }
            else if (ProcessAll && EmployeeList != "")
            {
                strEdlEmployeeIdQuery = "AND Tdh_IDNo IN (" + EmployeeList + ")";
                strEddEmployeeIdQuery = "AND Tdd_IDNo IN (" + EmployeeList + ")";
            }

            #region Query
            string sqlquery = @"INSERT INTO {0}
                                              ( [Tdd_IDNo]
                                               ,[Tdd_DeductionCode]
                                               ,[Tdd_StartDate]
                                               ,[Tdd_ThisPayCycle]
                                               ,[Tdd_PayCycle]
                                               ,[Tdd_LineNo]
                                               ,[Tdd_PaymentType]
                                               ,[Tdd_Amount]
                                               ,[Tdd_DeferredFlag]
                                               ,[Tdd_PaymentFlag]
                                               ,[Usr_Login]
                                               ,[Ludatetime])

                                                --- For Deffered Cycles
                                                SELECT T_EmpDeductionDefer.Tdd_IDNo
                                                    ,T_EmpDeductionDefer.Tdd_DeductionCode
                                                    ,T_EmpDeductionDefer.Tdd_StartDate
                                                    ,@PayPeriod AS Tdd_ThisPayCycle
                                                    ,T_EmpDeductionDefer.Tdd_PayCycle AS Tdd_PayCycle
                                                    ,T_EmpDeductionDefer.Tdd_LineNo AS Tdd_LineNo
                                                    ,'P' AS Tdd_PaymentType
                                                    ,T_EmpDeductionDefer.Tdd_DeferredAmt AS Tdd_Amount
                                                    ,1 AS Tdd_DeferredFlag
                                                    ,0 AS Tdd_PaymentFlag
                                                    ,@Usr_login
                                                    ,GETDATE()
                                                FROM {4}..T_EmpDeductionHdr
                                                INNER JOIN M_Employee ON T_EmpDeductionHdr.Tdh_IDNo = M_Employee.Mem_IDNo
                                                     AND LEFT(Mem_WorkStatus, 1) <> 'I'
                                                {5}
                                                INNER JOIN T_EmpDeductionDefer ON T_EmpDeductionDefer.Tdd_IDNo = T_EmpDeductionHdr.Tdh_IDNo
                                                    AND T_EmpDeductionDefer.Tdd_DeductionCode = T_EmpDeductionHdr.Tdh_DeductionCode 
                                                    AND T_EmpDeductionDefer.Tdd_StartDate = T_EmpDeductionHdr.Tdh_StartDate 
                                                WHERE T_EmpDeductionHdr.Tdh_PaidAmount < T_EmpDeductionHdr.Tdh_DeductionAmount
                                                    AND T_EmpDeductionHdr.Tdh_CycleAmount > 0
						                            AND Tdh_ExemptInPayroll = 0
                                                    AND T_EmpDeductionHdr.Tdh_StartDate <= @EndCycleDate
                                                    {1}

                                                UNION ALL

                                                --- For Current Payroll cycle
                                                SELECT T_EmpDeductionHdr.Tdh_IDNo
                                                    ,T_EmpDeductionHdr.Tdh_DeductionCode
                                                    ,T_EmpDeductionHdr.Tdh_StartDate
                                                    ,@PayPeriod AS Tdd_ThisPayCycle
                                                    ,@PayPeriod AS Tdd_PayCycle
                                                    ,'00' AS Tdd_LineNo
                                                    ,'P' AS Tdd_PaymentType
                                                    ,CASE
                                                        WHEN (Tdh_DeferredAmount + Tdh_PaidAmount + Tdh_CycleAmount) > Tdh_DeductionAmount THEN
                                                              Tdh_DeductionAmount - (Tdh_DeferredAmount + Tdh_PaidAmount)
                                                        ELSE
                                                              Tdh_CycleAmount
                                                     END AS Tdd_Amount
                                                    ,0 AS Tdd_DeferredFlag
                                                    ,0 AS Tdd_PaymentFlag
                                                    ,@Usr_login
                                                    ,GETDATE()
                                                FROM {4}..T_EmpDeductionHdr
                                                INNER JOIN M_Employee ON T_EmpDeductionHdr.Tdh_IDNo = M_Employee.Mem_IDNo
                                                    AND LEFT(Mem_WorkStatus, 1) <> 'I'
                                                {5}
                                                LEFT JOIN {4}..M_Deduction ON Mdn_DeductionCode = Tdh_DeductionCode
                                                    AND Mdn_CompanyCode = '{3}'
                                                    AND Mdn_RecordStatus = 'A'
                                                WHERE Tdh_StartDate <= @EndCycleDate
                                                    AND Tdh_DeferredAmount + Tdh_PaidAmount < Tdh_DeductionAmount
                                                    AND Tdh_CycleAmount > 0
						                            AND Tdh_ExemptInPayroll = 0
                                                    AND (   CASE WHEN LEN(rtrim(ISNULL(Tdh_ApplicablePayCycle,''))) = 0 THEN Mdn_ApplicablePayCycle ELSE Tdh_ApplicablePayCycle END = '0' 
		                                                 OR CASE WHEN LEN(rtrim(ISNULL(Tdh_ApplicablePayCycle,''))) = 0 THEN Mdn_ApplicablePayCycle ELSE Tdh_ApplicablePayCycle END =  @ApplicPeriod )
                                                    {1}

                                                DELETE FROM {0}
                                                FROM {0}
                                                INNER JOIN M_Employee ON Tdd_IDNo = M_Employee.Mem_IDNo
                                                    AND LEFT(Mem_WorkStatus, 1) <> 'I'
                                                {5}
                                                INNER JOIN T_EmpDeductionExempt on Tde_IDNo = Tdd_IDNo
                                                  AND Tde_DeductionCode = Tdd_DeductionCode
                                                  AND Tde_StartDate = Tdd_StartDate
                                                  AND Tde_PayCycle = Tdd_ThisPayCycle
                                                WHERE Tdd_ThisPayCycle = @PayPeriod
                                                    {2}";
            if (ProcessComputed)
            {
                sqlquery = string.Format(sqlquery
                                        , EmpDeductionDtlTable
                                        , strEdlEmployeeIdQuery
                                        , strEddEmployeeIdQuery
                                        , CompanyCode
                                        , CentralProfile
                                        , "");
            }
                
            else
            {
                string sqlqueryDelete = string.Format(@"
                                                DELETE FROM {0}
                                                FROM {0}
                                                INNER JOIN M_Employee ON Tdd_IDNo = M_Employee.Mem_IDNo
                                                    AND LEFT(Mem_WorkStatus, 1) = 'A'
                                                    AND Mem_IsComputedPayroll = 0
                                                ", EmpDeductionDtlTable);

                dal.ExecuteNonQuery(sqlqueryDelete);

                sqlquery = string.Format(sqlquery
                                        , EmpDeductionDtlTable
                                        , ""
                                        , ""
                                        , CompanyCode
                                        , CentralProfile
                                        , @"AND LEFT(Mem_WorkStatus, 1) = 'A'
                                            AND Mem_IsComputedPayroll = 0");
            }


            #endregion

            try
            {
                dal.ExecuteNonQuery(sqlquery, CommandType.Text, paramInfo);
            }
            catch
            {
                throw new PayrollException("Error upon creation of deductions on or before " + PayrollEnd + "." + "\n");
            }
        }

        public DataTable GetAllEmployeePayTransForProcess(bool ProcessAll, bool ProcessSeparated, string EmployeeId, string PayPeriod)
        {
            string EmployeeCondition = string.Format(@" WHERE Mem_IsComputedPayroll = 1
                                                        AND Tph_PayCycle = '{0}'", PayPeriod);

            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition += " AND Tph_IDNo = '" + EmployeeId + "' ";
            else if (ProcessAll && EmployeeList != "")
                EmployeeCondition += " AND Tph_IDNo IN (" + EmployeeList + ") ";
            if (ProcessSeparated)
            {
                if (!ProcessAll && EmployeeId != "")
                    EmployeeCondition = string.Format(" AND Tph_IDNo = '{0}' AND Tph_PayCycle = '{1}'", EmployeeId, PayPeriod);
            }

            #region query
            string query = string.Format(@"SELECT Tph_IDNo
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
                                                  ,Tph_TotalAdjAmt
                                                  ,Tph_TaxableIncomeAmt
                                                  ,Tph_NontaxableIncomeAmt
                                                  ,Tph_WorkDay
                                                  ,Tph_PayrollType
                                                  ,Tph_PremiumGrpCode
                                                  ,Mem_LastName
                                                  ,Mem_FirstName
                                                  ,Mem_MiddleName
                                                  ,Mem_CostcenterCode
                                                  ,Mem_PositionCode
                                                  ,Mem_PositionGrade
                                                  ,Mem_EmploymentStatusCode
                                                  ,Mem_WorkStatus
                                                  ,Mem_PaymentMode
                                                  ,Mem_PayrollBankCode
                                                  ,Mem_BankAcctNo
                                                  ,Mem_TaxCode
                                                  ,Mem_PayrollGroup
                                                  ,Mem_CalendarGrpCode
                                                  ,Mem_PagIbigRule
                                                  ,Mem_PagIbigShare
                                                  ,Mem_SSSRule
                                                  ,Mem_SSSShare
                                                  ,Mem_PHRule
                                                  ,Mem_PHShare
                                                  ,Mem_Salary
                                                  ,Mem_WorkLocationCode
                                                  ,Mem_AltAcctCode
                                                  ,ISNULL(Mem_IsTaxExempted, 0) AS Mem_IsTaxExempted
                                                  ,Mem_ExpenseClass
                                                  ,Mem_PositionGrade
                                                  ,Mem_IsComputedPerDay
                                                  ,Mem_IsMultipleSalary
                                                  ,Mem_TaxRule
                                                  ,Mem_TaxShare
                                                  ,Mem_UnionRule
                                                  ,Mem_UnionShare 
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
                                                  ,DATEDIFF(dd, Mem_IntakeDate, '{3}')/365.00 AS ServiceYear
                                              FROM {0}
                                              INNER JOIN M_Employee ON Tph_IDNo = Mem_IDNo
                                                INNER JOIN (
                                                    SELECT Mpd_SubCode 
                                                    FROM M_PolicyDtl 
                                                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                                                         AND Mpd_CompanyCode = '{5}'
                                                         AND Mpd_ParamValue = 1 
                                                ) EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
                                                {1}
                                              ORDER BY Mem_LastName, Mem_FirstName"
                                                , EmpPayTranHdrTable
                                                , EmployeeCondition
                                                , PayPeriod
                                                , PayrollEnd
                                                , PayrollStart
                                                , CompanyCode
                                                , CentralProfile);
            #endregion
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public DataTable GetAllEmployeePayTransExtForProcess(bool ProcessAll, string EmployeeId, string PayPeriod)
        {
            string EmployeeCondition = string.Format(@" WHERE Tph_PayCycle = '{0}'", PayPeriod);
            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition += " AND Tph_IDNo = '" + EmployeeId + "' ";
            else if (ProcessAll && EmployeeList != "")
                EmployeeCondition += " AND Tph_IDNo IN (" + EmployeeList + ") ";
            #region query
            string query = string.Format(@"SELECT Tph_IDNo
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
                                              FROM {0}
                                              INNER JOIN M_Employee ON Tph_IDNo = Mem_IDNo
                                                INNER JOIN (SELECT Mpd_SubCode 
                                                                FROM M_PolicyDtl 
                                                                WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                                                                     AND Mpd_CompanyCode = '{2}'
                                                                     AND Mpd_ParamValue = 1 ) EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
                                                {1}", EmpPayTranHdrMiscTable
                                                , EmployeeCondition
                                                , CompanyCode );
            #endregion
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public DataTable GetAllEmployeePayTransDetailForProcess(bool ProcessAll, bool ProcessSeparated, string EmployeeId, string PayPeriod)
        {
            string EmployeeCondition = string.Format(@" WHERE Tpd_PayCycle = '{0}' 
                                                        AND Tpd_Date BETWEEN '{1}' AND '{2}'", PayPeriod, PayrollStart, PayrollEnd);
            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition += " AND Tpd_IDNo = '" + EmployeeId + "' ";
            else if (ProcessAll && EmployeeList != "")
                EmployeeCondition += " AND Tpd_IDNo IN (" + EmployeeList + ") ";
            if (ProcessSeparated)
            {
                if (!ProcessAll && EmployeeId != "")
                    EmployeeCondition = string.Format(" WHERE Tpd_IDNo = '{0}' AND Tpd_PayCycle = '{1}'", EmployeeId, PayPeriod);
            }

            #region query
            string query = string.Format(@"SELECT Tpd_IDNo
                                                  ,Tpd_PayCycle
                                                  ,Tpd_Date
                                                  ,ISNULL(ISNULL(A.Tsl_SalaryRate,B.Tsl_SalaryRate), 0) AS Tpd_SalaryRate
                                                  ,ISNULL(ISNULL(A.Tsl_PayrollType, B.Tsl_PayrollType), '') AS Tpd_PayrollType
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
                                                  ,Tpd_PremiumGrpCode
                                                  ,Tpd_WorkDay
                                                  ,Msh_ShiftHours 
                                                  ,Ttr_DayCode
                                                  ,Ttr_RestDayFlag
                                                  ,Ttr_EmploymentStatusCode
                                                  ,Mem_WorkStatus
                                              FROM {0}
                                              INNER JOIN Udv_TimeRegister ON Tpd_IDNo = Ttr_IDNo
                                                 AND Tpd_Date = Ttr_Date
                                              INNER JOIN M_Employee ON Tpd_IDNo = Mem_IDNo
                                                 AND Mem_CompanyCode = '{4}'
                                              INNER JOIN (
                                                    SELECT Mpd_SubCode 
                                                    FROM M_PolicyDtl 
                                                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                                                         AND Mpd_CompanyCode = '{4}'
                                                         AND Mpd_ParamValue = 1 
                                                ) EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
                                              INNER JOIN {5}..M_Shift ON Msh_ShiftCode = Ttr_ShiftCode
                                                        AND Msh_CompanyCode = '{4}'
                                            LEFT JOIN {5}..T_EmpSalary A ON Tpd_IDNo = A.Tsl_IDNo
												AND  Tpd_Date BETWEEN A.Tsl_StartDate  AND ISNULL(A.Tsl_EndDate, CASE WHEN A.Tsl_StartDate >= '{2}' THEN A.Tsl_StartDate ELSE CASE WHEN A.Tsl_EndDate IS NULL THEN '{2}' ELSE A.Tsl_EndDate END END)
											LEFT JOIN {5}..T_EmpSalary B ON Tpd_IDNo = B.Tsl_IDNo
												AND Mem_IntakeDate BETWEEN B.Tsl_StartDate  AND ISNULL(B.Tsl_EndDate, CASE WHEN B.Tsl_StartDate >= '{2}' THEN B.Tsl_StartDate ELSE CASE WHEN B.Tsl_EndDate IS NULL THEN '{2}' ELSE  B.Tsl_EndDate END END)
                                            {3}
                                            ORDER BY Tpd_Date"
                                            , EmpPayTranDtlTable
                                            , PayrollStart
                                            , PayrollEnd
                                            , EmployeeCondition
                                            , CompanyCode
                                            , CentralProfile);
            #endregion
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public DataTable GetAllEmployeePayTransExtDetailForProcess(bool ProcessAll, bool ProcessSeparated, string EmployeeId, string PayPeriod)
        {
            string EmployeeCondition = string.Format(@" WHERE A.Tpd_PayCycle = '{0}'
                                                        AND A.Tpd_Date BETWEEN '{1}' AND '{2}'", PayPeriod, PayrollStart, PayrollEnd);
            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition += " AND A.Tpd_IDNo = '" + EmployeeId + "' ";
            else if (ProcessAll && EmployeeList != "")
                EmployeeCondition += " AND A.Tpd_IDNo IN (" + EmployeeList + ") ";
            if (ProcessSeparated)
            {
                if (!ProcessAll && EmployeeId != "")
                    EmployeeCondition = string.Format(" WHERE A.Tpd_IDNo = '{0}' AND A.Tpd_PayCycle = '{1}'", EmployeeId, PayPeriod);
            }
            #region query
            string query = string.Format(@"SELECT A.Tpd_IDNo
                                                  ,A.Tpd_PayCycle
                                                  ,A.Tpd_Date
                                                  ,ISNULL(ISNULL(C.Tsl_SalaryRate,D.Tsl_SalaryRate), 0) AS Tpd_SalaryRate
                                                  ,ISNULL(ISNULL(C.Tsl_PayrollType, D.Tsl_PayrollType), '') AS Tpd_PayrollType
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
                                                  ,Ttr_EmploymentStatusCode
                                                  ,B.Tpd_PremiumGrpCode
                                              FROM {0} A
                                              INNER JOIN {1} B
                                                    ON A.Tpd_IDNo = B.Tpd_IDNo
                                                    AND A.Tpd_PayCycle = B.Tpd_PayCycle
                                                    AND A.Tpd_Date = B.Tpd_Date
                                              INNER JOIN Udv_TimeRegister ON A.Tpd_IDNo = Ttr_IDNo
                                                    AND A.Tpd_Date = Ttr_Date
                                              INNER JOIN M_Employee ON A.Tpd_IDNo = Mem_IDNo
                                                    AND Mem_CompanyCode = '{5}'
                                              INNER JOIN (
                                                    SELECT Mpd_SubCode 
                                                    FROM M_PolicyDtl 
                                                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                                                         AND Mpd_CompanyCode = '{5}'
                                                         AND Mpd_ParamValue = 1 
                                                ) EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
                                              LEFT JOIN {6}..T_EmpSalary C ON A.Tpd_IDNo = C.Tsl_IDNo
												AND  A.Tpd_Date BETWEEN C.Tsl_StartDate  AND ISNULL(C.Tsl_EndDate, CASE WHEN C.Tsl_StartDate >= '{3}' THEN C.Tsl_StartDate ELSE CASE WHEN C.Tsl_EndDate IS NULL THEN '{3}' ELSE C.Tsl_EndDate END END)
											  LEFT JOIN {6}..T_EmpSalary D ON A.Tpd_IDNo = D.Tsl_IDNo
												AND Mem_IntakeDate BETWEEN D.Tsl_StartDate  AND ISNULL(D.Tsl_EndDate, CASE WHEN D.Tsl_StartDate >= '{3}' THEN D.Tsl_StartDate ELSE CASE WHEN D.Tsl_EndDate IS NULL THEN '{3}' ELSE  D.Tsl_EndDate END END)
                                              {4}
                                              ORDER BY A.Tpd_Date"
                                                , EmpPayTranDtlMiscTable
                                                , EmpPayTranDtlTable
                                                , PayrollStart
                                                , PayrollEnd
                                                , EmployeeCondition
                                                , CompanyCode
                                                , CentralProfile);
            #endregion
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }
        
        public DataTable GetAllEmployeePayrollAnnualRecordsForProcess(bool ProcessAll
                                                                    , string EmployeeId
                                                                    , string EmployeeList
                                                                    , bool bFirstAndSecondQuinOnly
                                                                    , bool bExcludeHoldPayroll
                                                                    , bool bGetTaxablePayPeriodsOnly
                                                                    , string strEmpPayrollYearlyTable 
                                                                    , DALHelper dal)
        {
            string EmployeeCondition = "";
            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition += " AND Tpy_IDNo = '" + EmployeeId + "' ";
            else if (ProcessAll && EmployeeList != "")
                EmployeeCondition += " AND Tpy_IDNo IN (" + EmployeeList + ") ";

            string PayPeriodCondition = "";
            if (bFirstAndSecondQuinOnly == true)
            {
                PayPeriodCondition = " AND SUBSTRING(Tpy_PayCycle, 7, 1) IN ('1','2') ";
            }

            string HoldPayrollCondition = "";
            if (bExcludeHoldPayroll == true)
            {
                HoldPayrollCondition = " AND Tpy_PaymentMode != 'H' ";
            }

            //string TaxablePayPeriodCondition = "";
            //if (bGetTaxablePayPeriodsOnly == true)
            //{
            //    TaxablePayPeriodCondition = " AND Tpy_IsTaxExempted = 0 ";
            //}
            #region query
            string query = string.Format(@"SELECT Tpy_IDNo
                                            ,LEFT(Tpy_PayCycle, 6) as Tpy_PayCycle
                                            ,MAX(Tps_StartCycle) as Tps_StartCycle
                                            ,MAX(Tps_EndCycle) as Tps_EndCycle
                                            ,MAX(Tpy_SalaryRate) as Tpy_SalaryRate
                                            ,MAX(Tpy_HourRate) as Tpy_HourRate
                                            ,SUM(Tpy_REGAmt) as Tpy_REGAmt
                                            ,SUM(Tpy_TotalAdjAmt) as Tpy_TotalAdjAmt
                                            ,SUM(Tpy_TaxableIncomeAmt) as Tpy_TaxableIncomeAmt
                                            ,SUM(Tpy_NontaxableIncomeAmt) as Tpy_NontaxableIncomeAmt
                                            ,SUM(Tpy_TotalTaxableIncomeAmt) as Tpy_TotalTaxableIncomeAmt
                                            ,SUM(Tpy_TaxShare) as Tpy_TaxShare
                                            ,SUM(Tpy_UnionAmt) as Tpy_UnionAmt
                                            ,SUM(Tpy_WtaxAmt) as Tpy_WtaxAmt
	                                        ,ISNULL(MAX(TAX), 0) as WTAX
                                            ,SUM(Tpy_SSSEE) as Tpy_SSSEE
                                            ,SUM(Tpy_SSSER) as Tpy_SSSER
                                            ,SUM(Tpy_MPFEE) as Tpy_MPFEE
                                            ,SUM(Tpy_MPFER) as Tpy_MPFER
                                            ,SUM(Tpy_PhilhealthEE) as Tpy_PhilhealthEE
                                            ,SUM(Tpy_PhilhealthER) as Tpy_PhilhealthER
                                            ,SUM(Tpy_ECFundAmt) as Tpy_ECFundAmt
                                            ,SUM(Tpy_PagIbigEE) as Tpy_PagIbigEE
                                            ,SUM(Tpy_PagIbigER) as Tpy_PagIbigER
	                                        ,ISNULL(MAX(SSS), 0) as SSSADJ
                                            ,ISNULL(MAX(MPF), 0) as MPFADJ
	                                        ,ISNULL(MAX(PHIC), 0) as PHICADJ
	                                        ,ISNULL(MAX(HDMF), 0) as HDMFADJ
	                                        ,ISNULL(MAX(SSS), 0) + ISNULL(MAX(MPF), 0) + ISNULL(MAX(PHIC), 0) + ISNULL(MAX(HDMF), 0) + ISNULL(MAX(UNIONDUES), 0) as PREMADJ
                                            ,ISNULL(MAX(UNIONDUES), 0) as UNIONADJ
                                            ,MAX(Tpy_PayrollType) as Tpy_PayrollType
                                            ,MAX(Tpy_PaymentMode) as Tpy_PaymentMode
                                            ,SUM(Tpy_PagIbigTaxEE) as Tpy_PagIbigTaxEE
                                            ,CONVERT(bit, MAX(CONVERT(int,Tpy_IsTaxExempted))) as Tpy_IsTaxExempted
                                            ,SUM(ISNULL(Tpy_TotalREGAmt, Tpy_REGAmt)) as Tpy_TotalREGAmt
                                            ,SUM(ISNULL(Tpy_SRGAdjAmt, 0)) as Tpy_SRGAdjAmt
                                            ,SUM(ISNULL(Tpy_SHOLAdjAmt, 0)) as Tpy_SHOLAdjAmt
                                            ,SUM(ISNULL(Tpy_SLVAdjAmt, 0)) as Tpy_SLVAdjAmt
                                            ,SUM(ISNULL(Tpy_MRGAdjAmt, 0)) as Tpy_MRGAdjAmt
                                            ,SUM(ISNULL(Tpy_MHOLAdjAmt, 0)) as Tpy_MHOLAdjAmt
                                        FROM {0}
                                        INNER JOIN M_Employee ON Tpy_IDNo = Mem_IDNo
                                        INNER JOIN T_PaySchedule ON Tpy_PayCycle = Tps_PayCycle
                                        INNER JOIN (SELECT Mpd_SubCode 
                                                    FROM M_PolicyDtl 
                                                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                                                         AND Mpd_CompanyCode = '{4}'
                                                         AND Mpd_RecordStatus = 'A'
                                                         AND Mpd_ParamValue = 1 ) EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
                                        LEFT JOIN (SELECT PayCycle, IDNo
				                                        , SUM(CASE WHEN DGroup IN ('SA','SR') OR DGroup IN ('GSA','GSR') THEN Amount ELSE 0 END) as SSS
                                                        , SUM(CASE WHEN DGroup IN ('MA','MR') OR DGroup IN ('GMA','GMR') THEN Amount ELSE 0 END) as MPF
				                                        , SUM(CASE WHEN DGroup IN ('PA','PR') OR DGroup IN ('GPA','GPR') THEN Amount ELSE 0 END) as PHIC
				                                        , SUM(CASE WHEN DGroup IN ('HA','HR') OR DGroup IN ('GHA','GHR') THEN Amount ELSE 0 END) as HDMF
				                                        , SUM(CASE WHEN DGroup = 'TX' OR DGroup = 'TAX' THEN Amount ELSE 0 END) as TAX
                                                        , SUM(CASE WHEN DGroup IN ('UA','UR') OR DGroup IN ('GUA','GUR') THEN Amount ELSE 0 END) as UNIONDUES
		                                         FROM (
				                                        SELECT LEFT(Tdd_ThisPayCycle,6) as PayCycle, Tdd_IDNo as IDNo, Mdn_DeductionGroup as DGroup, Tdd_Amount as Amount FROM T_EmpDeductionDtlHst
				                                         INNER JOIN {5}..M_Deduction ON Tdd_DeductionCode = Mdn_DeductionCode
					                                         AND Mdn_DeductionGroup IN ('SA','SR','MA','MR','PA','PR','HA','HR','TX','UA','UR')
                                                             AND Mdn_CompanyCode = '{4}'
                                                         INNER JOIN {0} ON Tpy_IDNo = Tdd_IDNo	
					                                        AND Tpy_PayCycle = Tdd_ThisPayCycle
				                                         WHERE Tdd_PaymentFlag = 1
 
				                                         UNION ALL
 
				                                         SELECT LEFT(Tdd_ThisPayCycle,6) as PayCycle, Tdd_IDNo as IDNo, Mdn_DeductionGroup as DGroup, Tdd_Amount as Amount FROM T_EmpDeductionDtlFullPayHst
				                                         INNER JOIN {5}..M_Deduction ON Tdd_DeductionCode = Mdn_DeductionCode
					                                        AND Mdn_DeductionGroup IN ('SA','SR','MA','MR','PA','PR','HA','HR','TX','UA','UR')
                                                            AND Mdn_CompanyCode = '{4}'
                                                         INNER JOIN {0} ON Tpy_IDNo = Tdd_IDNo	
					                                        AND Tpy_PayCycle = Tdd_ThisPayCycle
				                                         WHERE Tdd_PaymentFlag = 1

				                                         UNION ALL

				                                        SELECT LEFT(Tin_PayCycle,6) as PayCycle, Tin_IDNo as IDNo, Min_IncomeGroup as DGroup, Tin_IncomeAmt * -1 as Amount FROM T_EmpIncomeHst
				                                        INNER JOIN {5}..M_Income ON Min_IncomeCode = Tin_IncomeCode
					                                        AND Min_IncomeGroup IN ('GSA','GSR','GMA','GMR','GPA','GPR','GHA','GHR','TAX','GUA','GUR')
                                                            AND Min_CompanyCode = '{4}'
                                                         INNER JOIN {0} ON Tpy_IDNo = Tin_IDNo	
					                                        AND Tpy_PayCycle = Tin_PayCycle  
				                                        WHERE Tin_PostFlag = 1) DEDINCOME 
				                                        GROUP BY PayCycle, IDNo) TOTAL ON LEFT(Tpy_PayCycle,6) = PayCycle
						                                        AND Tpy_IDNo = IDNo
                                                        WHERE 1 = 1
                                                               {1}
                                                               {2}   
                                                               {3}                                         
                                                        GROUP BY Tpy_IDNo, LEFT(Tpy_PayCycle, 6)
                                              ", strEmpPayrollYearlyTable
                                              , EmployeeCondition
                                              , PayPeriodCondition
                                              , HoldPayrollCondition
                                              , CompanyCode
                                              , CentralProfile);
            #endregion
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }



        public DataTable GetAllEmployeePayrollAnnualExtensionRecordsForProcess(bool ProcessAll
                                                                                , string EmployeeId
                                                                                , string EmployeeList
                                                                                , bool bFirstAndSecondQuinOnly
                                                                                , bool bExcludeHoldPayroll
                                                                                , bool bGetTaxablePayPeriodsOnly
                                                                                , string strEmployeePayrollCalcAnnualTable
                                                                                , string strEmployeePayrollCalcAnnualExtTable
                                                                                , DALHelper dal)
        {
            string EmployeeCondition = "";
            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition += " AND CALCEXT.Tpy_IDNo = '" + EmployeeId + "' ";
            else if (ProcessAll && EmployeeList != "")
                EmployeeCondition += " AND CALCEXT.Tpy_IDNo IN (" + EmployeeList + ") ";

            string PayPeriodCondition = "";
            if (bFirstAndSecondQuinOnly == true)
            {
                PayPeriodCondition = " AND SUBSTRING(CALCEXT.Tpy_PayCycle, 7, 1) IN ('1','2') ";
            }

            string HoldPayrollCondition = "";
            if (bExcludeHoldPayroll == true)
            {
                HoldPayrollCondition = " AND Tpy_PaymentMode != 'H' ";
            }

            //string TaxablePayPeriodCondition = "";
            //if (bGetTaxablePayPeriodsOnly == true)
            //{
            //    TaxablePayPeriodCondition = " AND Tpy_IsTaxExempted = 0 ";
            //}
            #region query
            string query = string.Format(@"SELECT CALCEXT.Tpy_IDNo
                                                ,LEFT(CALCEXT.Tpy_PayCycle, 6) as Tpy_PayCycle
                                                ,MAX(Tps_StartCycle) as Tps_StartCycle
                                                ,MAX(Tps_EndCycle) as Tps_EndCycle
                                                ,SUM(Tpy_UnionAmt) as Tpy_UnionAmt
                                                ,SUM(Tpy_RecurringAllowanceAmt) as Tpy_RecurringAllowanceAmt
                                                ,SUM(Tpy_BonusNontaxAmt) as Tpy_BonusNontaxAmt
                                                ,SUM(Tpy_BonusTaxAmt) as Tpy_BonusTaxAmt
                                                ,SUM(Tpy_OtherTaxableIncomeAmt) as Tpy_OtherTaxableIncomeAmt
                                            FROM {0} CALCEXT
                                              INNER JOIN {1} CALC ON CALCEXT.Tpy_IDNo = CALC.Tpy_IDNo
                                                AND CALCEXT.Tpy_PayCycle = CALC.Tpy_PayCycle
                                              INNER JOIN M_Employee ON CALCEXT.Tpy_IDNo = Mem_IDNo
                                                INNER JOIN (
                                                   SELECT Mpd_SubCode 
                                                    FROM M_PolicyDtl 
                                                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                                                         AND Mpd_CompanyCode = '{5}'
                                                         AND Mpd_RecordStatus = 'A'
                                                         AND Mpd_ParamValue = 1 
                                                ) EMPSTAT ON Mem_EmploymentStatusCode = Mpd_SubCode
                                            INNER JOIN T_PaySchedule ON CALCEXT.Tpy_PayCycle = Tps_PayCycle
                                            WHERE 1 = 1
                                                {2}     
                                                {3}
                                                {4}                                      
                                            GROUP BY CALCEXT.Tpy_IDNo, LEFT(CALCEXT.Tpy_PayCycle, 6)
                                              ", strEmployeePayrollCalcAnnualTable
                                              , strEmployeePayrollCalcAnnualExtTable
                                              , EmployeeCondition
                                              , PayPeriodCondition
                                              , HoldPayrollCondition
                                              //, TaxablePayPeriodCondition
                                              , CompanyCode);
            #endregion
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public DataTable GetComputedDailyinCurrentPayPeriod()
        {
            #region query
            string qString = string.Format(@"SELECT Mem_IDNo, Mem_IntakeDate 
                                             FROM M_Employee
                                             WHERE Mem_IsComputedPerDay = 1 
                                             ");
            #endregion
            DataTable dtResult = dal.ExecuteDataSet(qString).Tables[0];
            return dtResult;
        }

        public DataTable GetNewlyHiredinCurrentPayPeriod()
        {
            #region query
            string qString = string.Format(@"SELECT Mem_IDNo, CONVERT(DATE,Mem_IntakeDate) AS Mem_IntakeDate
                                             FROM M_Employee
                                             WHERE CONVERT(DATE,Mem_IntakeDate) BETWEEN @STARTCYCLE AND @ENDCYCLE
                                                AND Mem_PayrollType = 'M'
                                             ");
            #endregion
            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@STARTCYCLE", PayrollStart, SqlDbType.Date);
            paramInfo[1] = new ParameterInfo("@ENDCYCLE", PayrollEnd, SqlDbType.Date);

            DataTable dtResult = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo).Tables[0];
            return dtResult;
        }

        public DataTable GetDayCodePremiums(string CompanyCode, string CentralProfile)
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

        public DataTable GetDayCodePremiums(string CompanyCode, string CentralProfile, DALHelper dalhelper)
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
            dtResult = dalhelper.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public DataTable GetPremiumContributionTable()
        {
            string query = string.Format(@"
                                SELECT Mps_DeductionCode
                                  ,Mps_CompensationFrom
                                  ,Mps_MonthlySalaryBracket
                                  ,Mps_MonthlySalaryCredit
                                  ,Mps_ERShare
                                  ,Mps_EEShare
                                  ,Mps_ECShare
                                  ,Mps_PayCycle
                                  ,Mps_IsPercentage
                                  ,Mps_MonthlySalaryCreditMPF
                                  ,Mps_MPFERShare
                                  ,Mps_MPFEEShare
                                FROM {1}..M_PremiumSchedule
                                WHERE Mps_RecordStatus = 'A'
                                    AND Mps_CompanyCode = '{0}'",  CompanyCode, CentralProfile);

            DataTable dtResult;
            dtResult = dal.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public void ComputeWithholdingTax(bool bAnnualize, bool bProcessLastPay, string TaxScheduleCode, string TaxComputationMethod, bool bExcludeHoldPayroll, DataRow drEmpPayroll, DataRow drEmpPayroll2, bool bIsNotTaxExempted, string companyCode, string centralProfile, string Assume13thMonth, DALHelper dalHelper)
        {
            string curEmployeeId = "";
            DataTable dtEmpPayrollHst = null;
            DataTable dtEmpPayroll2Hst = null;
            DataTable dtDeductionIncomeSeparated = null;

            string strSepYear = "";

            curEmployeeId = drEmpPayroll["Tpy_IDNo"].ToString();
            this.ProcessPayrollPeriod = drEmpPayroll["Tpy_PayCycle"].ToString();
            this.PayrollAssume13thMonth = Assume13thMonth;
            this.dal = dalHelper;
            this.commonBL = new CommonBL();
            TaxSchedule = TaxScheduleCode;
            CompanyCode = companyCode;
            CentralProfile = centralProfile;

            SetParameters();
            GetPayrollParameters(ProcessPayrollPeriod, TaxSchedule, bAnnualize);

            ISLASTCYCLE = IsLastQuincena(ProcessPayrollPeriod);
            dtPremiumContribution = GetPremiumContributionTable();
            dtEmpPayrollYearly = GetAllEmployeePayrollAnnualRecordsForProcess(false, curEmployeeId, "", false, bExcludeHoldPayroll, true, EmpPayrollYearlyTable, dal);
            dtEmpPayroll2Yearly = GetAllEmployeePayrollAnnualExtensionRecordsForProcess(false, curEmployeeId, "", false, bExcludeHoldPayroll, true, EmpPayrollYearlyTable, EmpPayrollYearly2Table, dal);
            dtEmpPayrollMisc2Yearly_1stAnd2ndQuin = GetAllEmployeePayrollAnnualExtensionRecordsForProcess(false, curEmployeeId, "", true, bExcludeHoldPayroll, false, EmpPayrollYearlyTable, EmpPayrollYearly2Table, dal);
            dsAllYTDAdjustment = GetYTDAdjustment(curEmployeeId, ProcessPayrollPeriod, dal);

            if (bProcessLastPay == true)
            {
                //TaxBase = "2";
                //TaxSchedule = "LASTPAY";
                strSepYear = GetEmployeeSeparationYear(curEmployeeId);
                if (strSepYear == "")
                    strSepYear = ProcessPayrollPeriod.Substring(0, 4);
                dtEmpPayrollHst = GetAllEmployeePayrollAnnualRecordsForProcess(false, curEmployeeId, "", false, bExcludeHoldPayroll, true, "T_EmpPayrollHst", dal);
                dtEmpPayroll2Hst = GetAllEmployeePayrollAnnualExtensionRecordsForProcess(false, curEmployeeId, "", false, bExcludeHoldPayroll, true, "T_EmpPayrollHst", "T_EmpPayroll2Hst", dal);
                dtDeductionIncomeSeparated = GetAllEmployeeIncomeAndDeductionSeparated(false, curEmployeeId, "", dal);
            }
            PopulateYTDValues(curEmployeeId
                                , drEmpPayroll
                                , drEmpPayroll2
                                , dtEmpPayrollYearly.Select("Tpy_IDNo = '" + curEmployeeId + "'")
                                , (dtEmpPayrollHst == null) ? null : dtEmpPayrollHst.Select(string.Format("Tpy_IDNo = '{0}' AND Tpy_PayCycle LIKE '{1}%'", curEmployeeId, strSepYear))
                                , dtEmpPayroll2Yearly.Select("Tpy_IDNo = '" + curEmployeeId + "'")
                                , (dtEmpPayroll2Hst == null) ? null : dtEmpPayroll2Hst.Select(string.Format("Tpy_IDNo = '{0}' AND Tpy_PayCycle LIKE '{1}%'", curEmployeeId, strSepYear))
                                , dsAllYTDAdjustment.Tables[0].Select("Tyt_IDNo = '" + curEmployeeId + "'")
                                , dsAllYTDAdjustment.Tables[1].Select("Tyt_IDNo = '" + curEmployeeId + "'")
                                , (dtDeductionIncomeSeparated == null) ? null : dtDeductionIncomeSeparated.Select("IDNo = '" + curEmployeeId + "'")
                                , bAnnualize);

            if (bProcessLastPay == true && TaxSchedule != "N") // Non-tax
            {
                ComputeWithholdingTax(bAnnualize
                                        , bProcessLastPay
                                        , drEmpPayroll
                                        , drEmpPayroll2
                                        , bIsNotTaxExempted
                                        , curEmployeeId
                                        , ProcessPayrollPeriod
                                        , ProcessPayrollPeriod.Substring(6, 1)
                                        , ProcessPayrollPeriod.Substring(0, 6)
                                        , dal);
            }
            else if (TaxComputationMethod == "T") //TAX TABLE
            {
                ComputeOffCycleWTaxBasedOnWTaxTable(drEmpPayroll, dal);
            }
            else if (TaxComputationMethod == "L") //LATEST PAYROLL TAX RATE
            {
                ComputeOffCycleWTaxBasedOnLastPayrollWTaxRate(drEmpPayroll, "", dal); //strBonusPayPeriods
            }
            else if (TaxComputationMethod == "D") //DIRECT TAX RATE
            {
                ComputeOffCycleWTaxBasedOnDirectTaxRate(drEmpPayroll, dal);
            }
        }

        public decimal ComputeBaseAmount(bool bCurrent, string curEmployeeId
                                                    , string PayrollPeriod
                                                    , string Rule
                                                    , decimal TotalTaxableIncomeAmt
                                                    , decimal REGAmt
                                                    , decimal BasicAdjustment
                                                    , decimal TaxableIncomeAmt
                                                    , string SalaryRate
                                                    , string PayrollType
                                                    , string Share
                                                    , string MaxShare
                                                    , string ApplicPrem
                                                    , bool EARLYCYCLEFULLDEDN
                                                    , bool REFERPREVINCOME)
        {
            decimal dBaseSalaryAmt = 0;
            string conditionIncome = string.Empty;
            string conditionDedn = string.Empty;
            if (bCurrent)
            {
                conditionIncome = string.Format("AND Tin_PayCycle = '{0}'", PayrollPeriod);
                conditionDedn = string.Format("AND Tdd_ThisPayCycle = '{0}'", PayrollPeriod);
            }
            else
            {
                conditionIncome = string.Format("AND LEFT(Tin_PayCycle,6) = '{0}'", PayrollPeriod);
                conditionDedn = string.Format("AND LEFT(Tdd_ThisPayCycle,6) = '{0}'", PayrollPeriod);
            }

            /////Gross Pay
            if (Rule.Equals("T"))
                dBaseSalaryAmt = TotalTaxableIncomeAmt;
            /////Gross Pay ---- GROSS PAY NOT SUPPORTED
            //else if (Rule.Equals("G"))
            //    dBaseSalaryAmt = Convert.ToDecimal(GrossAmt);
            /////Regular Pay
            else if (Rule.Equals("R"))
                dBaseSalaryAmt = REGAmt;
            /////Basic Pay
            else if (Rule.Equals("B"))
            {
                dBaseSalaryAmt = 0;
                if (PayrollType.Equals("D"))
                {
                    dBaseSalaryAmt = Convert.ToDecimal(SalaryRate);
                    dBaseSalaryAmt = dBaseSalaryAmt * MDIVISOR / 12;
                }
                else if (PayrollType.Equals("M"))
                {
                    dBaseSalaryAmt = Convert.ToDecimal(SalaryRate);
                }
                else if (PayrollType.Equals("H"))
                {
                    dBaseSalaryAmt = Convert.ToDecimal(SalaryRate);
                    dBaseSalaryAmt = dBaseSalaryAmt * CommonBL.HOURSINDAY * MDIVISOR / 12;
                }

                if (ApplicPrem.Equals("0"))
                {
                    if (!EARLYCYCLEFULLDEDN && !bMonthEnd) // first quincena
                        dBaseSalaryAmt /= PAYFREQNCYCOUNT;
                    else if (bMonthEnd) //second quincena
                    {
                        if (REFERPREVINCOME && !EARLYCYCLEFULLDEDN)
                            dBaseSalaryAmt /= PAYFREQNCYCOUNT;
                    }
                                        
                }   
            }
            /////Regular and Overtime Pay
            else if (Rule.Equals("O"))
            {
                dBaseSalaryAmt = TotalTaxableIncomeAmt - TaxableIncomeAmt;

                string query = string.Format(@"SELECT ISNULL(SUM(Tin_IncomeAmt),0) as Amount
                                               FROM {0}
                                               INNER JOIN {3}..M_Income on Tin_IncomeCode = Min_IncomeCode 
                                                    AND Min_IncomeGroup IN ('REG', 'HOL','LVE','OVT','NDF')
                                               WHERE Tin_IDNo = '{1}'
                                                    {2}
                                                    AND  Tin_PostFlag = 1
                                                            ", (bCurrent ? EmpIncomeTable : EmpIncomeHstTable)
                                                             , curEmployeeId
                                                             , conditionIncome
                                                             , CentralProfile);
                DataSet ds = dal.ExecuteDataSet(query);
                if (ds.Tables[0].Rows.Count > 0)
                    dBaseSalaryAmt += Convert.ToDecimal(ds.Tables[0].Rows[0]["Amount"].ToString());

                #region //No Deduction on Income part
                //query = string.Format(@"SELECT ISNULL(SUM(Tdd_Amount),0) as Amount
                //                                            FROM {0}  ----T_EmpDeductionDtl/T_EmpDeductionDtlHst
                //                                            INNER JOIN M_Deduction on  Mdn_Deductioncode = Tdd_DeductionCode 
                //                                                AND Mdn_DeductionGroup IN ('RG', 'HL','LV','OT','ND')
                //                                            WHERE Tdd_IDNo = '{1}'
                //                                            {2}
                //                                            AND  Tdd_PaymentFlag = 1
                //                                            AND  Tdd_PaymentType = 'P'
                //                                            ", (bCurrent ? EmpDeductionDtlTable : EmpDeductionDtlHstTable)
                //                                             , curEmployeeId
                //                                             , conditionDedn);
                //ds = dal.ExecuteDataSet(query);
                //if (ds.Tables[0].Rows.Count > 0)
                //    dBaseSalaryAmt -= Convert.ToDecimal(ds.Tables[0].Rows[0]["Amount"].ToString());
                #endregion
            }
            else if (Rule.Equals("A"))
            {
                dBaseSalaryAmt = REGAmt + BasicAdjustment;

                string query = string.Format(@"SELECT ISNULL(SUM(Tin_IncomeAmt),0) as Amount
                                                            FROM {0}
                                                            INNER JOIN {3}..M_Income on Tin_IncomeCode = Min_IncomeCode 
                                                                AND Min_IncomeGroup IN ('REG', 'HOL','LVE')
                                                            WHERE Tin_IDNo = '{1}'
                                                            {2}
                                                            AND  Tin_PostFlag = 1
                                                            ", (bCurrent ? EmpIncomeTable : EmpIncomeHstTable)
                                                            , curEmployeeId
                                                            , conditionIncome
                                                            , CentralProfile);
                DataSet ds = dal.ExecuteDataSet(query);
                if (ds.Tables[0].Rows.Count > 0)
                    dBaseSalaryAmt += Convert.ToDecimal(ds.Tables[0].Rows[0]["Amount"].ToString());

                #region //No Deduction on Income part
                //query = string.Format(@"SELECT ISNULL(SUM(Tdd_Amount),0) as Amount
                //                                            FROM {0}  ----T_EmpDeductionDtl/T_EmpDeductionDtlHst
                //                                            INNER JOIN M_Deduction on  Mdn_Deductioncode = Tdd_DeductionCode 
                //                                                AND Mdn_DeductionGroup IN ('RG', 'HL','LV')
                //                                            WHERE Tdd_IDNo = '{1}'
                //                                            {2}
                //                                            AND  Tdd_PaymentFlag = 1
                //                                            AND  Tdd_PaymentType = 'P'
                //                                            ", (bCurrent ? EmpDeductionDtlTable : EmpDeductionDtlHstTable)
                //                                             , curEmployeeId
                //                                             , conditionDedn);
                //ds = dal.ExecuteDataSet(query);
                //if (ds.Tables[0].Rows.Count > 0)
                //    dBaseSalaryAmt -= Convert.ToDecimal(ds.Tables[0].Rows[0]["Amount"].ToString());
                #endregion
            }

            /////Fixed Contribution/Special
            else if (Rule.Equals("F") || Rule.Equals("S") || Rule.Equals("E"))
            {
                dBaseSalaryAmt = Convert.ToDecimal(Share);
                if (ApplicHDMFPrem.Equals("0") && !bMonthEnd && (Rule.Equals("S") || Rule.Equals("E")))
                    dBaseSalaryAmt /= PAYFREQNCYCOUNT;
            }
            /////Maximum
            else if (Rule.Equals("M"))
            {
                dBaseSalaryAmt = Convert.ToDecimal(MaxShare);
                if (ApplicPrem.Equals("0"))
                {
                    if (!EARLYCYCLEFULLDEDN && !bMonthEnd) // first quincena
                        dBaseSalaryAmt /= PAYFREQNCYCOUNT;
                    else if (bMonthEnd) //second quincena
                    {
                        if (REFERPREVINCOME || !EARLYCYCLEFULLDEDN)
                            dBaseSalaryAmt /= PAYFREQNCYCOUNT;
                    }

                }
            }

            return dBaseSalaryAmt;
        }

        private void ComputeOffCycleWTaxBasedOnWTaxTable(DataRow drEmpPayroll, DALHelper dalHelper)
        {
            string query = string.Format(@"DECLARE @TotalTaxableIncomeAmt DECIMAL(10,2) = (SELECT Tpy_TotalTaxableIncomeAmt 
										                                                   FROM T_EmpPayroll
                                                                                           WHERE Tpy_PayCycle = '{0}'
                                                                                                 AND Tpy_IDNo = '{2}')

                                          SELECT TOP 1 ISNULL(M_TaxScheduleHdr.Mth_CompensationLevelTaxAmount + ((@TotalTaxableIncomeAmt - M_TaxScheduleDtl.Mtd_CompensationLevel) * (M_TaxScheduleHdr.Mth_TaxOnExcess/100.00)),0) AS [WtaxAmt]
                                                     , M_TaxScheduleHdr.Mth_BracketNo AS [BracketNo]
                                          FROM {5}..M_TaxScheduleDtl
                                          INNER JOIN {5}..M_TaxScheduleHdr on M_TaxScheduleHdr.Mth_TaxSchedule = M_TaxScheduleDtl.Mtd_TaxSchedule
                                                 AND M_TaxScheduleHdr.Mth_PayCycle = M_TaxScheduleDtl.Mtd_PayCycle  
                                                 AND M_TaxScheduleHdr.Mth_BracketNo = M_TaxScheduleDtl.Mtd_BracketNo 
                                                 AND M_TaxScheduleHdr.Mth_CompanyCode = M_TaxScheduleDtl.Mtd_CompanyCode
                                                 AND M_TaxScheduleHdr.Mth_RecordStatus = 'A'
                                          WHERE  M_TaxScheduleHdr.Mth_TaxSchedule = '{1}'
                                                  AND M_TaxScheduleDtl.Mtd_RecordStatus = 'A'
                                                  AND M_TaxScheduleDtl.Mtd_PayCycle = (SELECT MAX(Mth_PayCycle) as [PayPeriod]
                                                                                       FROM {5}..M_TaxScheduleHdr
                                                                                       WHERE Mth_TaxSchedule = '{1}'
                                                                                            AND Mth_PayCycle <= '{0}'
                                                                                            AND Mth_RecordStatus = 'A'
                                                                                            AND Mth_CompanyCode = '{4}')
                                                   AND M_TaxScheduleDtl.Mtd_TaxCode = ISNULL('{3}','S') 
                                                   AND M_TaxScheduleDtl.Mtd_CompensationLevel < @TotalTaxableIncomeAmt
                                                   AND M_TaxScheduleDtl.Mtd_CompanyCode = '{4}'
                                            ORDER BY M_TaxScheduleDtl.Mtd_CompensationLevel DESC"
                                                , drEmpPayroll["Tpy_PayCycle"]
                                                , TaxSchedule
                                                , drEmpPayroll["Tpy_IDNo"]
                                                , (Convert.ToInt32(MaxTaxHeader.Substring(0, 4)) < 2018 ? drEmpPayroll["Tpy_TaxCode"] : "ALL")
                                                ,  CompanyCode
                                                , CentralProfile);
            DataTable dtResult = dalHelper.ExecuteDataSet(query).Tables[0];
            if (dtResult.Rows.Count > 0)
            {
                drEmpPayroll["Tpy_WtaxAmt"] = Math.Round(Convert.ToDecimal(dtResult.Rows[0]["WtaxAmt"]), 2);
                drEmpPayroll["Tpy_TaxBracket"] = dtResult.Rows[0]["BracketNo"];
            }
        }

        private void ComputeOffCycleWTaxBasedOnLastPayrollWTaxRate(DataRow drEmpPayroll, string strBonusPayPeriods, DALHelper dalHelper)
        {
            string query = string.Format(@"
                                            DECLARE @PayPeriod as char(7) = '{0}'
                                            DECLARE @PayPeriodAnn as char(7) = (SELECT MAX(Tpy_PayCycle) 
															                    FROM T_EmpPayrollYearly
															                    WHERE Tpy_IDNo = '{2}'
															                    GROUP BY Tpy_IDNo)
                                            DECLARE @TaxBracket as TINYINT = (SELECT Tpy_TaxBracket 
                                                                              FROM Udv_Payroll
                                                                              WHERE Tpy_IDNo = '{2}'
												                                AND  Tpy_PayCycle = @PayPeriodAnn)

                                            UPDATE T_EmpPayroll
                                            SET Tpy_WtaxAmt = ([CurrentGrossAmt] * TaxRate)
                                                , Tpy_TaxBracket = CASE WHEN ([CurrentGrossAmt] * TaxRate) > 0 THEN @TaxBracket ELSE 0 END
                                            FROM (
                                            SELECT Tpy_TotalTaxableIncomeAmt as [CurrentGrossAmt]
                                                , TaxRate = ISNULL((SELECT TOP 1 (M_TaxScheduleHdr.Mth_TaxOnExcess/100.00)
                                                                    FROM {5}..M_TaxScheduleDtl
                                                                    INNER JOIN {5}..M_TaxScheduleHdr on M_TaxScheduleHdr.Mth_TaxSchedule = M_TaxScheduleDtl.Mtd_TaxSchedule
                                                                        AND M_TaxScheduleHdr.Mth_PayCycle = M_TaxScheduleDtl.Mtd_PayCycle  
                                                                        AND M_TaxScheduleHdr.Mth_BracketNo = M_TaxScheduleDtl.Mtd_BracketNo 
                                                                        AND M_TaxScheduleHdr.Mth_CompanyCode = M_TaxScheduleDtl.Mtd_CompanyCode
                                                                        AND M_TaxScheduleHdr.Mth_RecordStatus = 'A'
                                                                    WHERE  M_TaxScheduleHdr.Mth_TaxSchedule = '{1}'
                                                                           AND M_TaxScheduleDtl.Mtd_RecordStatus = 'A'
                                                                            AND M_TaxScheduleDtl.Mtd_PayCycle <= @PayPeriodAnn
                                                                           AND M_TaxScheduleDtl.Mtd_TaxCode = ISNULL('{3}','S') 
                                                                           AND M_TaxScheduleDtl.Mtd_BracketNo =  @TaxBracket 
                                                                           AND M_TaxScheduleDtl.Mtd_CompanyCode = '{4}'
                                                                    ORDER BY M_TaxScheduleDtl.Mtd_PayCycle DESC, M_TaxScheduleDtl.Mtd_CompensationLevel DESC), 0)
                                            FROM T_EmpPayroll
                                            WHERE Tpy_PayCycle = @PayPeriod
                                                AND Tpy_IDNo = '{2}'
                                            ) temp
                                            WHERE Tpy_PayCycle = @PayPeriod
                                                AND Tpy_IDNo = '{2}'

                                            SELECT Tpy_WtaxAmt,Tpy_TaxBracket
                                            FROM T_EmpPayroll
                                            WHERE Tpy_PayCycle = @PayPeriod
                                                AND Tpy_IDNo = '{2}'"
                                            , drEmpPayroll["Tpy_PayCycle"]
                                            , TaxSchedule
                                            , drEmpPayroll["Tpy_IDNo"]
                                            //, strBonusPayPeriods
                                            , (Convert.ToInt32(MaxTaxHeader.Substring(0, 4)) < 2018 ? drEmpPayroll["Tpy_TaxCode"] : "ALL")
                                            , CompanyCode
                                            , CentralProfile);
            DataTable dtResult = dalHelper.ExecuteDataSet(query).Tables[0];
            if (dtResult.Rows.Count > 0)
            {
                drEmpPayroll["Tpy_WtaxAmt"]   = Math.Round(Convert.ToDecimal(dtResult.Rows[0]["Tpy_WtaxAmt"]), 2);
                drEmpPayroll["Tpy_TaxBracket"] = Convert.ToDecimal(dtResult.Rows[0]["Tpy_TaxBracket"]);
            }
        }

        private void ComputeOffCycleWTaxBasedOnDirectTaxRate(DataRow drEmpPayroll, DALHelper dalHelper)
        {
            string query = string.Format(@"SELECT Tpy_WtaxAmt = ISNULL((SELECT TOP 1 Tpy_TotalTaxableIncomeAmt * (M_TaxScheduleHdr.Mth_TaxOnExcess/100.00)
                                                                        FROM {5}..M_TaxScheduleDtl
                                                                        INNER JOIN {5}..M_TaxScheduleHdr on M_TaxScheduleHdr.Mth_TaxSchedule = M_TaxScheduleDtl.Mtd_TaxSchedule
                                                                            AND M_TaxScheduleHdr.Mth_PayCycle = M_TaxScheduleDtl.Mtd_PayCycle  
                                                                            AND M_TaxScheduleHdr.Mth_BracketNo = M_TaxScheduleDtl.Mtd_BracketNo 
                                                                            AND M_TaxScheduleHdr.Mth_CompanyCode = M_TaxScheduleDtl.Mtd_CompanyCode
                                                                            AND M_TaxScheduleHdr.Mth_RecordStatus = 'A'
                                                                        WHERE  M_TaxScheduleHdr.Mth_TaxSchedule = '{1}'
                                                                               AND M_TaxScheduleDtl.Mtd_RecordStatus = 'A'
                                                                               AND M_TaxScheduleDtl.Mtd_PayCycle = (SELECT MAX(Mth_PayCycle) as [PayPeriod]
                                                                                                                   FROM {5}..M_TaxScheduleHdr
                                                                                                                   WHERE Mth_TaxSchedule = '{1}'
                                                                                                                        AND Mth_PayCycle <= '{0}'
                                                                                                                        AND Mth_RecordStatus = 'A'
                                                                                                                        AND Mth_CompanyCode = '{4}')
                                                                               AND M_TaxScheduleDtl.Mtd_TaxCode = ISNULL('{3}','S') 
                                                                               AND M_TaxScheduleDtl.Mtd_CompensationLevel < Tpy_TotalTaxableIncomeAmt
                                                                               AND M_TaxScheduleDtl.Mtd_CompanyCode = '{4}'
                                                                        ORDER BY M_TaxScheduleDtl.Mtd_CompensationLevel DESC), 0)
                                            FROM T_EmpPayroll
                                            WHERE Tpy_PayCycle = '{0}'
                                                AND Tpy_IDNo = '{2}'"
                                            , drEmpPayroll["Tpy_PayCycle"]
                                            , TaxSchedule
                                            , drEmpPayroll["Tpy_IDNo"]
                                            , CompanyCode
                                            , CentralProfile);
            DataTable dtResult = dalHelper.ExecuteDataSet(query).Tables[0];
            if (dtResult.Rows.Count > 0)
            {
                drEmpPayroll["Tpy_WtaxAmt"] = Math.Round(Convert.ToDecimal(dtResult.Rows[0]["Tpy_WtaxAmt"]), 2);
            }
        }

        public void PopulateYTDValues(string curEmployeeId
                                        , DataRow drEmpPayroll
                                        , DataRow drEmpPayroll2
                                        , DataRow[] drArrEmpPayrollYearly
                                        , DataRow[] drArrEmpPayrollHst
                                        , DataRow[] drArrEmpPayrollMisc2Yearly
                                        , DataRow[] drArrEmpPayrollMisc2Hst
                                        , DataRow[] drArrYTDAdjustmentPres
                                        , DataRow[] drArrYTDAdjustmentPrev
                                        , DataRow[] drDeductionIncomeSeparated
                                        , bool bAnnualizeChecked)
        {
            #region Variables
            decimal dYTDRegularBeforeAmt            = 0;
            decimal dYTDTotalTaxableIncomeBeforeAmt = 0;
            decimal dYTDWtaxBeforeAmt               = 0;
            decimal dYTDSSSBeforeAmt                = 0;
            decimal dYTDMPFBeforeAmt                = 0;
            decimal dYTDPhilhealthBeforeAmt         = 0;
            decimal dYTDHDMFBeforeAmt               = 0;
            decimal dYTDNonTaxBeforeAmt             = 0;
            decimal dYTDMWEBeforeAmt                = 0; //To be added to YTDNonTaxBeforeAmt
            decimal dYTDHDMFBeforeTaxAmt            = 0;
            decimal dPremiumPaid                    = 0;
            decimal dYTDTax13thMonth                = 0;
            decimal dYTDNontax13thMonth             = 0;
            decimal dYTDRecurringAllowanceBeforeAmt = 0;
            decimal dYTDBonusBeforeNontaxAmt        = 0;
            decimal dYTDBonusBeforeTaxAmt           = 0;
            decimal dYTDOtherTaxIncomeBeforeAmt     = 0;
            decimal dYTDTSalOth                     = 0;
            decimal dYTDOvertimePay                 = 0;
            decimal dYTDUnionDuesBeforeAmt          = 0;
            decimal dTotalTaxableIncomeAmt          = 0;
            decimal dMWEAmt                         = 0;
            #endregion

            #region PAYROLL DATA
            if (drArrEmpPayrollYearly.Length > 0)
            {
                for (int idxYTD = 0; idxYTD < drArrEmpPayrollYearly.Length; idxYTD++)
                {
                    dYTDRegularBeforeAmt            += Convert.ToDecimal(drArrEmpPayrollYearly[idxYTD]["Tpy_TotalREGAmt"]);
                    
                    dYTDWtaxBeforeAmt               += Convert.ToDecimal(drArrEmpPayrollYearly[idxYTD]["Tpy_WtaxAmt"]) + Convert.ToDecimal(drArrEmpPayrollYearly[idxYTD]["WTAX"]);
                    dYTDSSSBeforeAmt                += Convert.ToDecimal(drArrEmpPayrollYearly[idxYTD]["Tpy_SSSEE"]) + Convert.ToDecimal(drArrEmpPayrollYearly[idxYTD]["SSSADJ"]);
                    dYTDMPFBeforeAmt                += Convert.ToDecimal(drArrEmpPayrollYearly[idxYTD]["Tpy_MPFEE"]) + Convert.ToDecimal(drArrEmpPayrollYearly[idxYTD]["MPFADJ"]);
                    dYTDPhilhealthBeforeAmt         += Convert.ToDecimal(drArrEmpPayrollYearly[idxYTD]["Tpy_PhilhealthEE"]) + Convert.ToDecimal(drArrEmpPayrollYearly[idxYTD]["PHICADJ"]);
                    dYTDHDMFBeforeAmt               += Convert.ToDecimal(drArrEmpPayrollYearly[idxYTD]["Tpy_PagIbigEE"]) + Convert.ToDecimal(drArrEmpPayrollYearly[idxYTD]["HDMFADJ"]);
                    dYTDHDMFBeforeTaxAmt            += Convert.ToDecimal(drArrEmpPayrollYearly[idxYTD]["Tpy_PagIbigTaxEE"]);
                    dYTDUnionDuesBeforeAmt          += Convert.ToDecimal(drArrEmpPayrollYearly[idxYTD]["Tpy_UnionAmt"]) + Convert.ToDecimal(drArrEmpPayrollYearly[idxYTD]["UNIONADJ"]);
                    dYTDNonTaxBeforeAmt             += Convert.ToDecimal(drArrEmpPayrollYearly[idxYTD]["Tpy_NontaxableIncomeAmt"]);
                    if (bAnnualizeChecked)
                    {
                        if (Convert.ToBoolean(drEmpPayroll["Tpy_IsTaxExempted"]) == false)
                            dYTDTotalTaxableIncomeBeforeAmt += Convert.ToDecimal(drArrEmpPayrollYearly[idxYTD]["Tpy_TotalTaxableIncomeAmt"]);
                        else if (Convert.ToBoolean(drEmpPayroll["Tpy_IsTaxExempted"]) == true)
                            dYTDMWEBeforeAmt += Convert.ToDecimal(drArrEmpPayrollYearly[idxYTD]["Tpy_TotalTaxableIncomeAmt"]);
                    }
                    else
                        dYTDTotalTaxableIncomeBeforeAmt += Convert.ToDecimal(drArrEmpPayrollYearly[idxYTD]["Tpy_TotalTaxableIncomeAmt"]);
                }
            }
            if (drArrEmpPayrollHst != null && drArrEmpPayrollHst.Length > 0)
            {
                for (int idxYTD = 0; idxYTD < drArrEmpPayrollHst.Length; idxYTD++)
                {
                    dYTDRegularBeforeAmt            += Convert.ToDecimal(drArrEmpPayrollHst[idxYTD]["Tpy_TotalREGAmt"]);
                    dYTDWtaxBeforeAmt               += Convert.ToDecimal(drArrEmpPayrollHst[idxYTD]["Tpy_WtaxAmt"]) + Convert.ToDecimal(drArrEmpPayrollHst[idxYTD]["WTAX"]);
                    dYTDSSSBeforeAmt                += Convert.ToDecimal(drArrEmpPayrollHst[idxYTD]["Tpy_SSSEE"]) + Convert.ToDecimal(drArrEmpPayrollHst[idxYTD]["SSSADJ"]);
                    dYTDMPFBeforeAmt                += Convert.ToDecimal(drArrEmpPayrollHst[idxYTD]["Tpy_MPFEE"]) + Convert.ToDecimal(drArrEmpPayrollHst[idxYTD]["MPFADJ"]);
                    dYTDPhilhealthBeforeAmt         += Convert.ToDecimal(drArrEmpPayrollHst[idxYTD]["Tpy_PhilhealthEE"]) + Convert.ToDecimal(drArrEmpPayrollHst[idxYTD]["PHICADJ"]);
                    dYTDHDMFBeforeAmt               += Convert.ToDecimal(drArrEmpPayrollHst[idxYTD]["Tpy_PagIbigEE"]) + Convert.ToDecimal(drArrEmpPayrollHst[idxYTD]["HDMFADJ"]);
                    dYTDHDMFBeforeTaxAmt            += Convert.ToDecimal(drArrEmpPayrollHst[idxYTD]["Tpy_PagIbigTaxEE"]);
                    dYTDUnionDuesBeforeAmt          += Convert.ToDecimal(drArrEmpPayrollHst[idxYTD]["Tpy_UnionAmt"]) + Convert.ToDecimal(drArrEmpPayrollHst[idxYTD]["UNIONADJ"]);
                    dYTDNonTaxBeforeAmt             += Convert.ToDecimal(drArrEmpPayrollHst[idxYTD]["Tpy_NontaxableIncomeAmt"]);
                    if (bAnnualizeChecked)
                    {
                        if (Convert.ToBoolean(drEmpPayroll["Tpy_IsTaxExempted"]) == false)
                            dYTDTotalTaxableIncomeBeforeAmt += Convert.ToDecimal(drArrEmpPayrollHst[idxYTD]["Tpy_TotalTaxableIncomeAmt"]);
                        else if (Convert.ToBoolean(drEmpPayroll["Tpy_IsTaxExempted"]) == true)
                            dYTDMWEBeforeAmt += Convert.ToDecimal(drArrEmpPayrollHst[idxYTD]["Tpy_TotalTaxableIncomeAmt"]);
                    }
                    else
                        dYTDTotalTaxableIncomeBeforeAmt += Convert.ToDecimal(drArrEmpPayrollHst[idxYTD]["Tpy_TotalTaxableIncomeAmt"]);
                }
            }
            #endregion

            #region PAYROLL DATA (EXTENSION)
            if (drArrEmpPayrollMisc2Yearly.Length > 0)
            {
                for (int idxYTD = 0; idxYTD < drArrEmpPayrollMisc2Yearly.Length; idxYTD++)
                {
                    dYTDUnionDuesBeforeAmt          += Convert.ToDecimal(drArrEmpPayrollMisc2Yearly[idxYTD]["Tpy_UnionAmt"]);
                    dYTDRecurringAllowanceBeforeAmt += Convert.ToDecimal(drArrEmpPayrollMisc2Yearly[idxYTD]["Tpy_RecurringAllowanceAmt"]);
                    dYTDOtherTaxIncomeBeforeAmt     += Convert.ToDecimal(drArrEmpPayrollMisc2Yearly[idxYTD]["Tpy_OtherTaxableIncomeAmt"]);
                    dYTDBonusBeforeNontaxAmt        += Convert.ToDecimal(drArrEmpPayrollMisc2Yearly[idxYTD]["Tpy_BonusNontaxAmt"]);
                    dYTDBonusBeforeTaxAmt           += Convert.ToDecimal(drArrEmpPayrollMisc2Yearly[idxYTD]["Tpy_BonusTaxAmt"]);
                }
            }
            
            if (drArrEmpPayrollMisc2Hst != null && drArrEmpPayrollMisc2Hst.Length > 0)
            {
                for (int idxYTD = 0; idxYTD < drArrEmpPayrollMisc2Hst.Length; idxYTD++)
                {
                    dYTDUnionDuesBeforeAmt          += Convert.ToDecimal(drArrEmpPayrollMisc2Hst[idxYTD]["Tpy_UnionAmt"]);
                    dYTDRecurringAllowanceBeforeAmt += Convert.ToDecimal(drArrEmpPayrollMisc2Hst[idxYTD]["Tpy_RecurringAllowanceAmt"]);
                    dYTDOtherTaxIncomeBeforeAmt     += Convert.ToDecimal(drArrEmpPayrollMisc2Hst[idxYTD]["Tpy_OtherTaxableIncomeAmt"]);
                    dYTDBonusBeforeNontaxAmt        += Convert.ToDecimal(drArrEmpPayrollMisc2Hst[idxYTD]["Tpy_BonusNontaxAmt"]);
                    dYTDBonusBeforeTaxAmt           += Convert.ToDecimal(drArrEmpPayrollMisc2Hst[idxYTD]["Tpy_BonusTaxAmt"]);
                }
            }
            #endregion

            #region YTD PRESENT EMPLOYER
            if (drArrYTDAdjustmentPres.Length > 0)
            {
                for (int idxYTD = 0; idxYTD < drArrYTDAdjustmentPres.Length; idxYTD++)
                {
                        dYTDTotalTaxableIncomeBeforeAmt += Convert.ToDecimal(drArrYTDAdjustmentPres[idxYTD]["TaxIncomeAmt"]);
                        dYTDRegularBeforeAmt += Convert.ToDecimal(drArrYTDAdjustmentPres[idxYTD]["BasicSalary"]);
                        dYTDOvertimePay += Convert.ToDecimal(drArrYTDAdjustmentPres[idxYTD]["TOvertimePay"]);
                        dYTDTSalOth += Convert.ToDecimal(drArrYTDAdjustmentPres[idxYTD]["TSalariesOtherFormsCompensation"]);
                        dYTDMWEBeforeAmt += Convert.ToDecimal(drArrYTDAdjustmentPres[idxYTD]["MWEAmt"]);
                        dYTDNonTaxBeforeAmt += Convert.ToDecimal(drArrYTDAdjustmentPres[idxYTD]["NSalOth"]);
                        dYTDTax13thMonth += Convert.ToDecimal(drArrYTDAdjustmentPres[idxYTD]["T13thMonth"]);
                        dYTDNontax13thMonth += Convert.ToDecimal(drArrYTDAdjustmentPres[idxYTD]["N13thMonth"]);
                        dPremiumPaid        += Convert.ToDecimal(drArrYTDAdjustmentPres[idxYTD]["PremiumPaid"]);
                        dYTDWtaxBeforeAmt += Convert.ToDecimal(drArrYTDAdjustmentPres[idxYTD]["WtaxAmt"]);
                        dYTDSSSBeforeAmt += Convert.ToDecimal(drArrYTDAdjustmentPres[idxYTD]["PremiumAmt"]);
                }
            }
            #endregion

            #region YTD PREVIOUS EMPLOYER
            if (drArrYTDAdjustmentPrev.Length > 0)
            {
                for (int idxYTD = 0; idxYTD < drArrYTDAdjustmentPrev.Length; idxYTD++)
                {
                        dYTDTotalTaxableIncomeBeforeAmt += Convert.ToDecimal(drArrYTDAdjustmentPrev[idxYTD]["TaxIncomeAmt"]);
                        dYTDRegularBeforeAmt            += Convert.ToDecimal(drArrYTDAdjustmentPrev[idxYTD]["BasicSalary"]);
                        dYTDOvertimePay += Convert.ToDecimal(drArrYTDAdjustmentPrev[idxYTD]["TOvertimePay"]);
                        dYTDTSalOth                     += Convert.ToDecimal(drArrYTDAdjustmentPrev[idxYTD]["TSalariesOtherFormsCompensation"]);
                        dYTDMWEBeforeAmt += Convert.ToDecimal(drArrYTDAdjustmentPrev[idxYTD]["MWEAmt"]);
                        dYTDNonTaxBeforeAmt += Convert.ToDecimal(drArrYTDAdjustmentPrev[idxYTD]["NSalOth"]);
                        dYTDTax13thMonth += Convert.ToDecimal(drArrYTDAdjustmentPrev[idxYTD]["T13thMonth"]);
                        dYTDNontax13thMonth += Convert.ToDecimal(drArrYTDAdjustmentPrev[idxYTD]["N13thMonth"]);
                        dPremiumPaid        += Convert.ToDecimal(drArrYTDAdjustmentPrev[idxYTD]["PremiumPaid"]);
                        dYTDWtaxBeforeAmt += Convert.ToDecimal(drArrYTDAdjustmentPrev[idxYTD]["WtaxAmt"]);
                        dYTDSSSBeforeAmt += Convert.ToDecimal(drArrYTDAdjustmentPrev[idxYTD]["PremiumAmt"]);
                }
            }
            #endregion

            if (bAnnualizeChecked)
            {
                dTotalTaxableIncomeAmt = (!Convert.ToBoolean(drEmpPayroll["Tpy_IsTaxExempted"]) ? Convert.ToDecimal(drEmpPayroll["Tpy_TotalTaxableIncomeAmt"].ToString()) : 0);
                dMWEAmt =  (Convert.ToBoolean(drEmpPayroll["Tpy_IsTaxExempted"]) ? Convert.ToDecimal(drEmpPayroll["Tpy_TotalTaxableIncomeAmt"].ToString()) : 0);
            }
            else
            {
                dTotalTaxableIncomeAmt = Convert.ToDecimal(drEmpPayroll["Tpy_TotalTaxableIncomeAmt"].ToString());
            }

            #region Save YTD Values
            //Gross Amount
            drEmpPayroll2["Tpy_YTDTotalTaxableIncomeAmtBefore"] = dYTDTotalTaxableIncomeBeforeAmt;
            drEmpPayroll2["Tpy_YTDTotalTaxableIncomeAmt"]       = dYTDTotalTaxableIncomeBeforeAmt
                                                                      + dTotalTaxableIncomeAmt;
            //WTax
            drEmpPayroll2["Tpy_YTDWtaxAmtBefore"]               = dYTDWtaxBeforeAmt;
            drEmpPayroll2["Tpy_YTDWtaxAmt"]                     = dYTDWtaxBeforeAmt + Convert.ToDecimal(drEmpPayroll["Tpy_WtaxAmt"].ToString());
            if (drDeductionIncomeSeparated != null && drDeductionIncomeSeparated.Length > 0)
                drEmpPayroll2["Tpy_YTDWtaxAmt"]                 = Convert.ToDecimal(drEmpPayroll2["Tpy_YTDWtaxAmt"].ToString()) + Convert.ToDecimal(drDeductionIncomeSeparated[0]["TAX"].ToString());

            //MPF Premium
            drEmpPayroll2["Tpy_YTDMPFAmtBefore"]                = dYTDMPFBeforeAmt;
            drEmpPayroll2["Tpy_YTDMPFAmt"]                      = dYTDMPFBeforeAmt + Convert.ToDecimal(drEmpPayroll["Tpy_MPFEE"].ToString());
            if (drDeductionIncomeSeparated != null && drDeductionIncomeSeparated.Length > 0)
                drEmpPayroll2["Tpy_YTDMPFAmt"]                  = Convert.ToDecimal(drEmpPayroll2["Tpy_YTDMPFAmt"].ToString()) + Convert.ToDecimal(drDeductionIncomeSeparated[0]["MPF"].ToString());
            //SSS Premium
            drEmpPayroll2["Tpy_YTDSSSAmtBefore"]                = dYTDSSSBeforeAmt;
            drEmpPayroll2["Tpy_YTDSSSAmt"]                      = dYTDSSSBeforeAmt + Convert.ToDecimal(drEmpPayroll["Tpy_SSSEE"].ToString());
            if (drDeductionIncomeSeparated != null && drDeductionIncomeSeparated.Length > 0)
                drEmpPayroll2["Tpy_YTDSSSAmt"]                  = Convert.ToDecimal(drEmpPayroll2["Tpy_YTDSSSAmt"].ToString()) + Convert.ToDecimal(drDeductionIncomeSeparated[0]["SSS"].ToString());
            drEmpPayroll2["Tpy_SSSTotal"]                       = Convert.ToDecimal(drEmpPayroll2["Tpy_YTDSSSAmt"].ToString()) + Convert.ToDecimal(drEmpPayroll2["Tpy_YTDMPFAmt"].ToString());
            
            //PhilHealth Premium
            drEmpPayroll2["Tpy_YTDPhilhealthAmtBefore"]         = dYTDPhilhealthBeforeAmt;
            drEmpPayroll2["Tpy_YTDPhilhealthAmt"]               = dYTDPhilhealthBeforeAmt + Convert.ToDecimal(drEmpPayroll["Tpy_PhilhealthEE"].ToString());
            if (drDeductionIncomeSeparated != null && drDeductionIncomeSeparated.Length > 0)
                drEmpPayroll2["Tpy_YTDPhilhealthAmt"]           = Convert.ToDecimal(drEmpPayroll2["Tpy_YTDPhilhealthAmt"].ToString()) + Convert.ToDecimal(drDeductionIncomeSeparated[0]["PHIC"].ToString());
            drEmpPayroll2["Tpy_PhilhealthTotal"]                = drEmpPayroll2["Tpy_YTDPhilhealthAmt"];
            //HDMF Non-Taxable Premium
            drEmpPayroll2["Tpy_YTDPagIbigAmtBefore"]            = dYTDHDMFBeforeAmt;
            drEmpPayroll2["Tpy_YTDPagIbigAmt"]                  = dYTDHDMFBeforeAmt + Convert.ToDecimal(drEmpPayroll["Tpy_PagIbigEE"].ToString());
            if (drDeductionIncomeSeparated != null && drDeductionIncomeSeparated.Length > 0)
                drEmpPayroll2["Tpy_YTDPagIbigAmt"]              = Convert.ToDecimal(drEmpPayroll2["Tpy_YTDPagIbigAmt"].ToString()) + Convert.ToDecimal(drDeductionIncomeSeparated[0]["HDMF"].ToString());
            drEmpPayroll2["Tpy_PagIbigTotal"]                   = drEmpPayroll2["Tpy_YTDPagIbigAmt"];
            //HDMF Taxable Premium
            drEmpPayroll2["Tpy_YTDPagIbigTaxAmtBefore"]         = dYTDHDMFBeforeTaxAmt;
            drEmpPayroll2["Tpy_YTDPagIbigTaxAmt"]               = dYTDHDMFBeforeTaxAmt + Convert.ToDecimal(drEmpPayroll["Tpy_PagIbigTaxEE"].ToString());
            //Union Dues Premium
            drEmpPayroll2["Tpy_YTDUnionAmtBefore"]              = dYTDUnionDuesBeforeAmt;
            drEmpPayroll2["Tpy_YTDUnionAmt"]                    = dYTDUnionDuesBeforeAmt + Convert.ToDecimal(drEmpPayroll["Tpy_UnionAmt"].ToString());
            if (drDeductionIncomeSeparated != null && drDeductionIncomeSeparated.Length > 0)
                drEmpPayroll2["Tpy_YTDUnionAmt"]                = Convert.ToDecimal(drEmpPayroll2["Tpy_YTDUnionAmt"].ToString()) + Convert.ToDecimal(drDeductionIncomeSeparated[0]["UNIONDUES"].ToString());
            drEmpPayroll2["Tpy_UnionTotal"]                     = drEmpPayroll2["Tpy_YTDUnionAmt"];
            //Premium Paid on Health
            drEmpPayroll2["Tpy_PremiumPaidOnHealth"]            = dPremiumPaid;

            //Regular Amount
            drEmpPayroll2["Tpy_YTDRegularAmtBefore"]            = dYTDRegularBeforeAmt;
            drEmpPayroll2["Tpy_YTDREGAmt"]                      = dYTDRegularBeforeAmt + Convert.ToDecimal(drEmpPayroll["Tpy_TotalREGAmt"].ToString());
            drEmpPayroll2["Tpy_RegularTotal"]                   = Convert.ToDecimal(drEmpPayroll2["Tpy_YTDREGAmt"]);
            //Recurring Allowance Amount
            drEmpPayroll2["Tpy_YTDRecurringAllowanceAmtBefore"] = dYTDRecurringAllowanceBeforeAmt;
            drEmpPayroll2["Tpy_RecurringAllowanceAmt"]          = GetRecurringAllowanceTotal(curEmployeeId, true);
            drEmpPayroll2["Tpy_RecurringAllowanceTotal"]        = Convert.ToDecimal(drEmpPayroll2["Tpy_YTDRecurringAllowanceAmtBefore"])
                                                                        + Convert.ToDecimal(drEmpPayroll2["Tpy_RecurringAllowanceAmt"]);
            drEmpPayroll2["Tpy_YTDRecurringAllowanceAmt"]       = Convert.ToDecimal(drEmpPayroll2["Tpy_YTDRecurringAllowanceAmtBefore"])
                                                                        + Convert.ToDecimal(drEmpPayroll2["Tpy_RecurringAllowanceAmt"]);
            //13th Month and Other Benefits Amount
            drEmpPayroll2["Tpy_YTDBonusTaxBefore"]              = dYTDTax13thMonth + dYTDBonusBeforeTaxAmt;
            drEmpPayroll2["Tpy_YTDBonusAmtBefore"]              = dYTDNontax13thMonth + dYTDTax13thMonth + dYTDBonusBeforeNontaxAmt + dYTDBonusBeforeTaxAmt;
            drEmpPayroll2["Tpy_BonusNontaxAmt"]                 = GetAllowanceTotal(curEmployeeId, "'N-13THBEN'", true);
            drEmpPayroll2["Tpy_BonusTaxAmt"]                    = GetAllowanceTotal(curEmployeeId, "'T-13THBEN'", true);
            drEmpPayroll2["Tpy_BonusTotal"]                     = Convert.ToDecimal(drEmpPayroll2["Tpy_YTDBonusAmtBefore"])
                                                                    + Convert.ToDecimal(drEmpPayroll2["Tpy_BonusNontaxAmt"])
                                                                    + Convert.ToDecimal(drEmpPayroll2["Tpy_BonusTaxAmt"]);
            ////Add Excess of 90,000 to Taxable Amount
            if (Convert.ToDecimal(drEmpPayroll2["Tpy_BonusTotal"]) > M13EXCLTAX)
                drEmpPayroll2["Tpy_BonusTaxRevaluated"] = Convert.ToDecimal(drEmpPayroll2["Tpy_BonusTotal"]) - M13EXCLTAX;
            else
                drEmpPayroll2["Tpy_BonusTaxRevaluated"] = 0;

            //Reevaluate Gross Pay Amount (for Taxable 13th Month)
            if (bAnnualizeChecked == true)
            {
                drEmpPayroll2["Tpy_YTDTotalTaxableIncomeAmt"] = Convert.ToDecimal(drEmpPayroll2["Tpy_YTDTotalTaxableIncomeAmt"])
                                                    - dYTDTax13thMonth
                                                    - dYTDBonusBeforeTaxAmt
                                                    - Convert.ToDecimal(drEmpPayroll2["Tpy_BonusTaxAmt"]);
                drEmpPayroll2["Tpy_YTDTotalTaxableIncomeAmt"] = Convert.ToDecimal(drEmpPayroll2["Tpy_YTDTotalTaxableIncomeAmt"])
                                                    + Convert.ToDecimal(drEmpPayroll2["Tpy_BonusTaxRevaluated"]);
            }

            //Other Taxable Income Amount
            drEmpPayroll2["Tpy_YTDOtherTaxableIncomeAmtBefore"] = dYTDTSalOth + dYTDOvertimePay + dYTDOtherTaxIncomeBeforeAmt;
            drEmpPayroll2["Tpy_OtherTaxableIncomeAmt"]          = Convert.ToDecimal(drEmpPayroll["Tpy_TotalTaxableIncomeAmt"].ToString())
                                                                        - (Convert.ToDecimal(drEmpPayroll["Tpy_TotalREGAmt"].ToString())
                                                                            + Convert.ToDecimal(drEmpPayroll2["Tpy_RecurringAllowanceAmt"])
                                                                            + Convert.ToDecimal(drEmpPayroll2["Tpy_BonusTaxAmt"]));
            drEmpPayroll2["Tpy_OtherTaxableIncomeTotal"]        = Convert.ToDecimal(drEmpPayroll2["Tpy_YTDOtherTaxableIncomeAmtBefore"])
                                                                    + Convert.ToDecimal(drEmpPayroll2["Tpy_OtherTaxableIncomeAmt"]);

            //Other Non-Taxable Income Amount
            drEmpPayroll2["Tpy_YTDNontaxableAmtBefore"] = dYTDNonTaxBeforeAmt + dYTDMWEBeforeAmt 
                                                            + dYTDSSSBeforeAmt 
                                                            + dYTDMPFBeforeAmt 
                                                            + dYTDPhilhealthBeforeAmt 
                                                            + dYTDHDMFBeforeAmt 
                                                            + dYTDUnionDuesBeforeAmt;

            drEmpPayroll2["Tpy_YTDNontaxableAmt"] = Convert.ToDecimal(drEmpPayroll2["Tpy_YTDNontaxableAmtBefore"])
                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_NontaxableIncomeAmt"].ToString())
                                                            + dMWEAmt
                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_SSSEE"].ToString())
                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_MPFEE"].ToString())
                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_PhilhealthEE"].ToString())
                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_PagIbigEE"].ToString())
                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_UnionAmt"].ToString());


            //Reevaluate Non-Taxable Income Amount (for Non-Taxable 13th Month)
            drEmpPayroll2["Tpy_YTDNontaxableAmt"] = Convert.ToDecimal(drEmpPayroll2["Tpy_YTDNontaxableAmt"])
                                                        - dYTDNontax13thMonth
                                                        - dYTDBonusBeforeNontaxAmt;
            drEmpPayroll2["Tpy_YTDNontaxableAmt"] = Convert.ToDecimal(drEmpPayroll2["Tpy_YTDNontaxableAmt"])
                                                        + Convert.ToDecimal(drEmpPayroll2["Tpy_BonusTotal"])
                                                        - Convert.ToDecimal(drEmpPayroll2["Tpy_BonusTaxRevaluated"]);


            //Total Taxable Income Amount
            if (bAnnualizeChecked == true)
            {
                drEmpPayroll2["Tpy_TotalTaxableIncomeWAssumeAmt"] = Convert.ToDecimal(drEmpPayroll2["Tpy_RegularTotal"])
                                                                + Convert.ToDecimal(drEmpPayroll2["Tpy_RecurringAllowanceTotal"])
                                                                + Convert.ToDecimal(drEmpPayroll2["Tpy_BonusTaxAmt"])
                                                                + Convert.ToDecimal(drEmpPayroll2["Tpy_OtherTaxableIncomeTotal"]);
            }
            #endregion
        }

        private void ComputeWithholdingTax(bool bAnnualizeChecked, bool bProcessLastPay, DataRow drEmpPayroll, DataRow drEmpPayroll2, bool bIsNotTaxExempted, string curEmployeeId, string ProcessPayrollPeriod, string PayrollCycle, string PayrollYearMonth, DALHelper dal)
        {
            #region Variables
            decimal SalaryRate = 0;
            string PayrollType = "";
            string strTaxCode = "";
            string strTaxRule = "";
            decimal TaxShare = 0;
            DataRow[] drPremiumContribution;
            DataRow[] drArrEmpPayrollYearly;
            DataRow[] drArrEmpPayrollMisc2Yearly;
            decimal dTotalGrossAmt = 0;
            decimal dBaseAmt = 0;

            decimal dHalfMonthAmt                       = 0;
            int iPayPeriodDiv                           = 0;
            int iPayPeriodBase                          = 0;
            decimal dEstBasic                           = 0;
            decimal dEstRecurring                       = 0;
            decimal dEstSSSPrem                         = 0;
            decimal dEstPHPrem                          = 0;
            decimal dEstHDMFPrem                        = 0;
            //decimal dEstHDMFPremTax                     = 0;
            decimal dEstUnionDues                       = 0;
            decimal dAmtForTax                          = 0;
            //decimal dEstMPFPrem                         = 0;

            DataSet dsAnnualTaxAmt;
            decimal dWTaxAmt                            = 0;

            decimal dMonthlyBaseAmt                     = 0;
            decimal dMonthlySSSEmployeeShare            = 0;
            decimal dMonthlyHDMFEmployeeShare           = 0;
            decimal dMonthlyPhilhealthEmployeeShare     = 0;
            decimal dMonthlyOtherPremiumEmployeeShare   = 0;
            decimal dMonthlyWtaxAmt                     = 0;
            decimal dMonthlyMPFEmployeeShare            = 0;
            DataSet dsWTaxAmount;
            #endregion

            #region Initialize
            SalaryRate  = Convert.ToDecimal(drEmpPayroll["Tpy_SalaryRate"]);
            PayrollType = drEmpPayroll["Tpy_PayrollType"].ToString();
            strTaxCode  = drEmpPayroll["Tpy_TaxCode"].ToString();
            strTaxRule = drEmpPayroll["Tpy_TaxRule"].ToString();
            TaxShare = Convert.ToDecimal(drEmpPayroll["Tpy_TaxShare"]);
            #endregion

            if (!bAnnualizeChecked)
            {
                    #region Normal Tax Computation
                    dBaseAmt = ComputeBaseAmount(true, curEmployeeId
                                                       , ProcessPayrollPeriod
                                                       , (strTaxRule == "Y" ? TAXRULE : strTaxRule)
                                                       , Convert.ToDecimal(drEmpPayroll["Tpy_TotalTaxableIncomeAmt"].ToString())
                                                       , Convert.ToDecimal(drEmpPayroll["Tpy_TotalREGAmt"].ToString())
                                                       , Convert.ToDecimal(drEmpPayroll["Tpy_SRGAdjAmt"].ToString())
                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_SHOLAdjAmt"].ToString())
                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_SLVAdjAmt"].ToString())
                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_MRGAdjAmt"].ToString())
                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_MHOLAdjAmt"].ToString())
                                                       , Convert.ToDecimal(drEmpPayroll["Tpy_TaxableIncomeAmt"].ToString())
                                                       , drEmpPayroll["Tpy_SalaryRate"].ToString()
                                                       , drEmpPayroll["Tpy_PayrollType"].ToString()
                                                       , drEmpPayroll["Tpy_TaxShare"].ToString()
                                                       , "0"
                                                       , ApplicTax
                                                       , false
                                                       , REFERPREVINCOMETAX);

                    dAmtForTax = dBaseAmt
                                        - (Convert.ToDecimal(drEmpPayroll["Tpy_SSSEE"].ToString())
                                                    + Convert.ToDecimal(drEmpPayroll["Tpy_MPFEE"].ToString())
                                                    + Convert.ToDecimal(drEmpPayroll["Tpy_PhilhealthEE"].ToString())
                                                    + Convert.ToDecimal(drEmpPayroll["Tpy_PagIbigEE"].ToString())
                                                    + Convert.ToDecimal(drEmpPayroll["Tpy_UnionAmt"].ToString()));

                    #region Refer to 1st quincena
                    if (REFERPREVINCOMETAX)
                    {
                        dtEmpPayrollYearly1stAnd2ndQuinTaxableOnly = GetAllEmployeePayrollAnnualRecordsForProcess(false, curEmployeeId, "", true, true, true, EmpPayrollYearlyTable, dal);
                        drArrEmpPayrollYearly = dtEmpPayrollYearly1stAnd2ndQuinTaxableOnly.Select("Tpy_IDNo = '" + curEmployeeId + "' AND Tpy_PayCycle LIKE '" + PayrollYearMonth + "*'");
                        if (drArrEmpPayrollYearly.Length > 0)
                        {
                            dMonthlyBaseAmt = ComputeBaseAmount(true, curEmployeeId, PayrollYearMonth, (strTaxRule == "Y" ? TAXRULE : strTaxRule)
                                                       , Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_TotalTaxableIncomeAmt"].ToString())
                                                       , Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_TotalREGAmt"].ToString())
                                                       , Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_SRGAdjAmt"].ToString())
                                                            + Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_SHOLAdjAmt"].ToString())
                                                            + Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_SLVAdjAmt"].ToString())
                                                            + Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_MRGAdjAmt"].ToString())
                                                            + Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_MHOLAdjAmt"].ToString())
                                                       , Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_TaxableIncomeAmt"].ToString())
                                                       , drArrEmpPayrollYearly[0]["Tpy_SalaryRate"].ToString()
                                                       , drArrEmpPayrollYearly[0]["Tpy_PayrollType"].ToString()
                                                       , drArrEmpPayrollYearly[0]["Tpy_TaxShare"].ToString()
                                                       , "0"
                                                       , ApplicTax
                                                       , false
                                                       , REFERPREVINCOMETAX);

                            dMonthlySSSEmployeeShare = Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_SSSEE"].ToString());
                            dMonthlyMPFEmployeeShare = Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_MPFEE"].ToString());
                            dMonthlyPhilhealthEmployeeShare = Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_PhilhealthEE"].ToString());
                            dMonthlyHDMFEmployeeShare = Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_PagIbigEE"].ToString());
                            dMonthlyWtaxAmt = Convert.ToDecimal(drArrEmpPayrollYearly[0]["Tpy_WtaxAmt"].ToString());
                        }

                        drArrEmpPayrollMisc2Yearly = dtEmpPayrollMisc2Yearly_1stAnd2ndQuin.Select("Tpy_IDNo = '" + curEmployeeId + "' AND Tpy_PayCycle LIKE '" + PayrollYearMonth + "*'");
                        if (drArrEmpPayrollMisc2Yearly.Length > 0)
                        {
                            dMonthlyOtherPremiumEmployeeShare = Convert.ToDecimal(drArrEmpPayrollMisc2Yearly[0]["Tpy_UnionAmt"].ToString());
                        }

                    if (dMonthlyBaseAmt == 0 && ApplicTax.Equals("0") && (strTaxRule.Equals("B") & bMonthEnd))
                        dMonthlyBaseAmt = dBaseAmt;

                    //Get amount for taxation
                    dAmtForTax = 0;
                        dAmtForTax = (dBaseAmt
                                        + dMonthlyBaseAmt)
                                            - (Convert.ToDecimal(drEmpPayroll["Tpy_SSSEE"].ToString())
                                                + Convert.ToDecimal(drEmpPayroll["Tpy_MPFEE"].ToString())
                                                + Convert.ToDecimal(drEmpPayroll["Tpy_PhilhealthEE"].ToString())
                                                + Convert.ToDecimal(drEmpPayroll["Tpy_PagIbigEE"].ToString())
                                                + Convert.ToDecimal(drEmpPayroll["Tpy_UnionAmt"].ToString())
                                                + dMonthlySSSEmployeeShare
                                                + dMonthlyMPFEmployeeShare
                                                + dMonthlyPhilhealthEmployeeShare
                                                + dMonthlyHDMFEmployeeShare
                                                + dMonthlyOtherPremiumEmployeeShare);

                    }

                    dsWTaxAmount = GetWTaxAmount(dAmtForTax, (Convert.ToInt32(MaxTaxHeader.Substring(0,4)) < 2018 ? strTaxCode : "ALL"), dal); //Revised Withholding Tax effective January 1, 2018 to December 31, 2022
                    if (dsWTaxAmount.Tables[0].Rows.Count > 0)
                    {
                        //Withholding Tax
                        dWTaxAmt = Convert.ToDecimal(dsWTaxAmount.Tables[0].Rows[0][0].ToString());
                        if (REFERPREVINCOMETAX)
                        {
                            dWTaxAmt -= dMonthlyWtaxAmt;
                        }
                        decimal BracketNo = Convert.ToDecimal(dsWTaxAmount.Tables[0].Rows[0]["Mth_BracketNo"].ToString());
                        drEmpPayroll["Tpy_WtaxAmt"] = dWTaxAmt;
                        drEmpPayroll["Tpy_TaxBracket"] = BracketNo;
                    }
                    #endregion

                    if (strTaxRule == "Y") 
                    {
                        //Withholding tax
                        dWTaxAmt = dBaseAmt * (TaxShare / 100);
                        drEmpPayroll["Tpy_WtaxAmt"] = dWTaxAmt;
                    }
                    if (strTaxRule == "F") //Fix Tax Amount
                    {
                        drEmpPayroll["Tpy_WtaxAmt"] = dBaseAmt;
                    }

                    if (REFERPREVINCOMETAX)
                        drEmpPayroll["Tpy_TaxBaseAmt"] = dBaseAmt + dMonthlyBaseAmt;
                    else
                        drEmpPayroll["Tpy_TaxBaseAmt"] = dBaseAmt;

                    //Update year-to-date withholding tax
                    drEmpPayroll2["Tpy_YTDWtaxAmt"] = Convert.ToDecimal(drEmpPayroll2["Tpy_YTDWtaxAmtBefore"].ToString())
                                                        + Convert.ToDecimal(drEmpPayroll["Tpy_WtaxAmt"].ToString());
                    #endregion
            }
            else
            {
                #region Zero-out Tax Computation
                if (bIsNotTaxExempted == true) //if not tax-exempted
                {
                    //Save Total Exemptions only if Annualized
                    drEmpPayroll2["Tpy_TotalExemptions"] = GetTaxExemptionAmount(strTaxCode);

                    #region Get Taxable Amount
                    if (!bProcessLastPay
                        && (TXANNLQUINFORMULA2 == "1"
                            || (TXANNLQUINFORMULA1 == "1" && ISLASTCYCLE == false && bAnnualizeChecked == false)
                            || (TXANNLQUINFORMULA1 == "1" && ISLASTCYCLE == false && bAnnualizeChecked == true)))
                    {
                        #region Annualize Tax Every Quincena (TAIHEIYO/LEAR)
                        dAmtForTax = 0;
                        if (TXANNLQUINFORMULA1 == "1") //ASSUME GROSS PAY, PREMIUMS AND BONUS FOR FUTURE QUINCENAS (TAIHEIYO)
                        {
                            #region TAIHEIYO
                            #region Half Month Amount
                            dHalfMonthAmt = 0;
                            if (PayrollType == "M")
                                dHalfMonthAmt = SalaryRate / 2.0m;
                            else if (PayrollType == "D")
                                dHalfMonthAmt = SalaryRate * MDIVISOR / 12 / 2.0m;
                            #endregion

                            #region Estimated Basic
                            iPayPeriodDiv = GetNextQuincenaCount(ProcessPayrollPeriod, curEmployeeId);
                            dEstBasic     = Math.Round(iPayPeriodDiv * dHalfMonthAmt, 2);
                            
                            drEmpPayroll2["Tpy_RemainingPayCycle"] = iPayPeriodDiv;
                            drEmpPayroll2["Tpy_AssumeRegularAmt"]  = dEstBasic;
                            drEmpPayroll2["Tpy_RegularTotal"]      = dEstBasic
                                                                         + Convert.ToDecimal(drEmpPayroll2["Tpy_YTDREGAmt"]);

                            drEmpPayroll2["Tpy_AssumeMonthlyRegularAmt"] = dHalfMonthAmt * 2;
                            #endregion

                            #region Estimated Recurring Allowance
                            if (ASSUMERECURRINGALLOWANCE)
                            {
                                dEstRecurring = Math.Round(GetMonthlyRecurringAllowance(curEmployeeId) * (iPayPeriodDiv / 2.0m), 2);
                                drEmpPayroll2["Tpy_AssumeRecurringAllowanceAmt"]    = dEstRecurring;
                                drEmpPayroll2["Tpy_RecurringAllowanceTotal"]        = dEstRecurring
                                                                                        + Convert.ToDecimal(drEmpPayroll2["Tpy_YTDRecurringAllowanceAmtBefore"])
                                                                                        + Convert.ToDecimal(drEmpPayroll2["Tpy_RecurringAllowanceAmt"]);
                            }
                            #endregion

                            #region Estimated SSS Premium
                            dEstSSSPrem = 0;
                            drPremiumContribution = dtPremiumContribution.Select(string.Format("Mps_DeductionCode = 'SSSPREM' AND Mps_CompensationFrom <= {0} AND Mps_PayCycle = '{1}'", dHalfMonthAmt * 2, MaxSSSPremPayCycle), "Mps_CompensationFrom DESC");
                            if (drPremiumContribution.Length > 0)
                            {

                                drEmpPayroll2["Tpy_AssumeMonthlySSSAmt"] = Math.Round(Convert.ToDecimal(drPremiumContribution[0]["Mps_EEShare"].ToString()) + Convert.ToDecimal(drPremiumContribution[0]["Mps_MPFEEShare"].ToString()), 2);

                                if (!bMonthEnd && ApplicSSSPrem == "2")
                                    dEstSSSPrem += Convert.ToDecimal(drEmpPayroll2["Tpy_AssumeMonthlySSSAmt"]) + Math.Round(((Convert.ToDecimal(drPremiumContribution[0]["Mps_EEShare"].ToString()) + Convert.ToDecimal(drPremiumContribution[0]["Mps_MPFEEShare"].ToString())) * (int)(iPayPeriodDiv / 2.0m)), 2);
                                else if (ProcessPayrollPeriod.Substring(6, 1) == ApplicSSSPrem)
                                    dEstSSSPrem += Math.Round(((Convert.ToDecimal(drPremiumContribution[0]["Mps_EEShare"].ToString()) + Convert.ToDecimal(drPremiumContribution[0]["Mps_MPFEEShare"].ToString())) * (int)(iPayPeriodDiv / 2.0m)), 2);
                                else
                                    dEstSSSPrem += Math.Round(((Convert.ToDecimal(drPremiumContribution[0]["Mps_EEShare"].ToString()) + Convert.ToDecimal(drPremiumContribution[0]["Mps_MPFEEShare"].ToString())) * (iPayPeriodDiv / 2.0m)), 2);

                                drEmpPayroll2["Tpy_AssumeSSSAmt"]   = dEstSSSPrem;
                                drEmpPayroll2["Tpy_SSSTotal"]       = dEstSSSPrem 
                                                                        + Convert.ToDecimal(drEmpPayroll2["Tpy_YTDSSSAmt"])
                                                                        + Convert.ToDecimal(drEmpPayroll2["Tpy_YTDMPFAmt"]);
                            }
                            #endregion

                            #region Estimated PH Premium
                            dEstPHPrem = 0;
                            drPremiumContribution = dtPremiumContribution.Select(string.Format("Mps_DeductionCode = 'PHICPREM' AND Mps_CompensationFrom <= {0} AND Mps_PayCycle = '{1}'", dHalfMonthAmt * 2, MaxPhilHealthPremPayCycle), "Mps_CompensationFrom DESC");
                            if (drPremiumContribution.Length > 0)
                            {
                                if (!Convert.ToBoolean(drPremiumContribution[0]["Mps_IsPercentage"]))
                                    drEmpPayroll2["Tpy_AssumeMonthlyPhilhealthAmt"] = Math.Round(Convert.ToDecimal(drPremiumContribution[0]["Mps_EEShare"].ToString()), 2);
                                else
                                {
                                    drEmpPayroll2["Tpy_AssumeMonthlyPhilhealthAmt"] = Math.Round((Convert.ToDecimal(drEmpPayroll2["Tpy_AssumeMonthlyRegularAmt"]) * Convert.ToDecimal(drPremiumContribution[0]["Mps_EEShare"].ToString())) / 100, 2);
                                }

                                if (!bMonthEnd && ApplicPHPrem == "2")
                                    dEstPHPrem += Convert.ToDecimal(drEmpPayroll2["Tpy_AssumeMonthlyPhilhealthAmt"]) + Math.Round(Convert.ToDecimal(drEmpPayroll2["Tpy_AssumeMonthlyPhilhealthAmt"]) * (int)(iPayPeriodDiv / 2.0m), 2);
                                else if (ProcessPayrollPeriod.Substring(6, 1) == ApplicPHPrem)
                                    dEstPHPrem += Math.Round(Convert.ToDecimal(drEmpPayroll2["Tpy_AssumeMonthlyPhilhealthAmt"]) * (int)(iPayPeriodDiv / 2.0m), 2);
                                else
                                    dEstPHPrem += Math.Round(Convert.ToDecimal(drEmpPayroll2["Tpy_AssumeMonthlyPhilhealthAmt"]) * (iPayPeriodDiv / 2.0m), 2);

                                drEmpPayroll2["Tpy_AssumePhilhealthAmt"] = dEstPHPrem;
                                drEmpPayroll2["Tpy_PhilhealthTotal"]     = dEstPHPrem
                                                                             + Convert.ToDecimal(drEmpPayroll2["Tpy_YTDPhilhealthAmt"]);
                            }
                            #endregion

                            #region Estimated HDMF Nontax and Tax Premiums
                            dEstHDMFPrem = Convert.ToDecimal(drEmpPayroll["Tpy_PagIbigShare"].ToString());
                            if (dEstHDMFPrem > 100)
                                dEstHDMFPrem = 100;

                            drEmpPayroll2["Tpy_AssumeMonthlyPagibigAmt"] = Math.Round(dEstHDMFPrem, 2);

                            if (!bMonthEnd && ApplicHDMFPrem == "2")
                                dEstHDMFPrem = Math.Round(dEstHDMFPrem, 2) + Math.Round(dEstHDMFPrem * (int)(iPayPeriodDiv / 2.0m), 2);
                            else if (ProcessPayrollPeriod.Substring(6, 1) == ApplicHDMFPrem)
                                dEstHDMFPrem = Math.Round(dEstHDMFPrem * (int)(iPayPeriodDiv / 2.0m), 2);
                            else
                                dEstHDMFPrem = Math.Round(dEstHDMFPrem * (iPayPeriodDiv / 2.0m), 2);

                            drEmpPayroll2["Tpy_AssumePagIbigAmt"] = dEstHDMFPrem;
                            drEmpPayroll2["Tpy_PagIbigTotal"]     = dEstHDMFPrem
                                                                        + Convert.ToDecimal(drEmpPayroll2["Tpy_YTDPagIbigAmt"]);
                            #endregion

                            #region Estimated Union Dues
                            dEstUnionDues = 0;
                            drPremiumContribution = dtPremiumContribution.Select(string.Format("Mps_DeductionCode = 'UNIONDUES' AND Mps_CompensationFrom <= {0} AND Mps_PayCycle <= '{1}'", (dHalfMonthAmt * 2), ProcessPayrollPeriod), "Mps_PayCycle DESC, Mps_CompensationFrom DESC");
                            if (drPremiumContribution.Length > 0)
                            {
                                drEmpPayroll2["Tpy_AssumeMonthlyUnionAmt"] = Math.Round(Convert.ToDecimal(drPremiumContribution[0]["Mps_EEShare"].ToString()), 2);

                                if (!bMonthEnd && ApplicUnionPrem == "2")
                                    dEstUnionDues += Convert.ToDecimal(drEmpPayroll2["Tpy_AssumeMonthlyUnionAmt"]) + Math.Round(Convert.ToDecimal(drPremiumContribution[0]["Mps_EEShare"].ToString()) * (int)(iPayPeriodDiv / 2.0m), 2);
                                else if (ProcessPayrollPeriod.Substring(6, 1) == ApplicUnionPrem)
                                    dEstUnionDues += Math.Round(Convert.ToDecimal(drPremiumContribution[0]["Mps_EEShare"].ToString()) * (int)(iPayPeriodDiv / 2.0m), 2);
                                else
                                    dEstUnionDues += Math.Round(Convert.ToDecimal(drPremiumContribution[0]["Mps_EEShare"].ToString()) * (iPayPeriodDiv / 2.0m), 2);

                                drEmpPayroll2["Tpy_AssumeUnionAmt"] = dEstUnionDues;
                                drEmpPayroll2["Tpy_UnionTotal"]     = dEstUnionDues
                                                                             + Convert.ToDecimal(drEmpPayroll2["Tpy_YTDUnionAmt"]);
                            }
                            #endregion

                            #region Estimated 13th Month
                            if (PayrollAssume13thMonth == "W")
                                drEmpPayroll2["Tpy_Assume13thMonthAmt"] = Math.Round((Convert.ToDecimal(drEmpPayroll2["Tpy_RegularTotal"]) / 12.0m), 2);
                            else if (PayrollAssume13thMonth == "H")
                                drEmpPayroll2["Tpy_Assume13thMonthAmt"] = Math.Round((Convert.ToDecimal(drEmpPayroll2["Tpy_RegularTotal"]) / 12.0m) / 2, 2);

                            drEmpPayroll2["Tpy_BonusTotal"]   = Convert.ToDecimal(drEmpPayroll2["Tpy_YTDBonusAmtBefore"])
                                                                    + Convert.ToDecimal(drEmpPayroll2["Tpy_BonusNontaxAmt"])
                                                                    + Convert.ToDecimal(drEmpPayroll2["Tpy_BonusTaxAmt"])
                                                                    + Convert.ToDecimal(drEmpPayroll2["Tpy_Assume13thMonthAmt"]);

                            //Add Excess of 90,000 to Taxable Amount
                            if (Convert.ToDecimal(drEmpPayroll2["Tpy_BonusTotal"]) > M13EXCLTAX)
                                drEmpPayroll2["Tpy_BonusTaxRevaluated"] = Convert.ToDecimal(drEmpPayroll2["Tpy_BonusTotal"]) - M13EXCLTAX;
                            else
                                drEmpPayroll2["Tpy_BonusTaxRevaluated"] = 0;
                            #endregion

                            #region Initial Taxable Amount
                            dAmtForTax = Math.Round(Convert.ToDecimal(drEmpPayroll2["Tpy_RegularTotal"])
                                                    + Convert.ToDecimal(drEmpPayroll2["Tpy_RecurringAllowanceTotal"])
                                                    + Convert.ToDecimal(drEmpPayroll2["Tpy_BonusTaxRevaluated"])
                                                    + Convert.ToDecimal(drEmpPayroll2["Tpy_OtherTaxableIncomeTotal"])
                                                    - (Convert.ToDecimal(drEmpPayroll2["Tpy_SSSTotal"])
                                                        + Convert.ToDecimal(drEmpPayroll2["Tpy_PhilhealthTotal"])
                                                        + Convert.ToDecimal(drEmpPayroll2["Tpy_PagIbigTotal"])
                                                        + Convert.ToDecimal(drEmpPayroll2["Tpy_UnionTotal"])), 2);
                            #endregion

                            drEmpPayroll2["Tpy_TotalTaxableIncomeWAssumeAmt"] = dAmtForTax;
                            //drEmpPayroll2["Tpy_TotalTaxableIncomeWAssumeAmt"] = Convert.ToDecimal(drEmpPayroll2["Tpy_YTDTotalTaxableIncomeAmt"])
                            //                                                    + Convert.ToDecimal(drEmpPayroll2["Tpy_AssumeRegularAmt"]);
                            //if (bAnnualizeChecked == true)
                            //{
                            //    drEmpPayroll2["Tpy_TotalTaxableIncomeWAssumeAmt"] = Convert.ToDecimal(drEmpPayroll2["Tpy_TotalTaxableIncomeWAssumeAmt"])
                            //                                                        - (Convert.ToDecimal(drEmpPayroll2["Tpy_YTDBonusTaxBefore"]));

                            //    drEmpPayroll2["Tpy_TotalTaxableIncomeWAssumeAmt"] = Convert.ToDecimal(drEmpPayroll2["Tpy_TotalTaxableIncomeWAssumeAmt"])
                            //                                                            + Convert.ToDecimal(drEmpPayroll2["Tpy_BonusTaxRevaluated"]);
                            //}
                            #endregion
                        }
                        else if (TXANNLQUINFORMULA2 == "1") //GET AVERAGE GROSS PAY PER QUINCENA (LEAR)
                        {
                            #region LEAR
                            dEstBasic = Convert.ToDecimal(drEmpPayroll2["Tpy_YTDTotalTaxableIncomeAmt"].ToString())
                                            - (Convert.ToDecimal(drEmpPayroll2["Tpy_YTDSSSAmt"].ToString())
                                                + Convert.ToDecimal(drEmpPayroll2["Tpy_YTDMPFAmt"].ToString())
                                                + Convert.ToDecimal(drEmpPayroll2["Tpy_YTDPhilhealthAmt"].ToString())
                                                + Convert.ToDecimal(drEmpPayroll2["Tpy_YTDPagIbigAmt"].ToString())
                                                + Convert.ToDecimal(drEmpPayroll2["Tpy_YTDUnionAmt"].ToString()));

                            //Get pay period divisor
                            iPayPeriodDiv = GetTaxablePeriods(ProcessPayrollPeriod, curEmployeeId);
                            iPayPeriodBase = GetNextQuincenaCount(ProcessPayrollPeriod, curEmployeeId) + iPayPeriodDiv;
                            if (iPayPeriodDiv * iPayPeriodBase != 0)
                                dAmtForTax = dEstBasic / (iPayPeriodDiv * iPayPeriodBase);

                            drEmpPayroll2["Tpy_RemainingPayCycle"] = iPayPeriodBase;
                            drEmpPayroll2["Tpy_AssumeRegularAmt"]  = dAmtForTax;
                            drEmpPayroll2["Tpy_RegularTotal"]      = dAmtForTax
                                                                         + Convert.ToDecimal(drEmpPayroll2["Tpy_YTDREGAmt"]);
                            #endregion
                        }

                        //Get gross pay for zero-out
                        dTotalGrossAmt = Math.Round(dAmtForTax
                                                    - Convert.ToDecimal(drEmpPayroll2["Tpy_TotalExemptions"])
                                                    - Convert.ToDecimal(drEmpPayroll2["Tpy_PremiumPaidOnHealth"]), 2);
                        drEmpPayroll2["Tpy_NetTaxableIncomeAmt"] = dTotalGrossAmt;
                        #endregion
                    }
                    else
                    {
                        #region Year-end Zero-out Tax
                        dTotalGrossAmt = Math.Round(Convert.ToDecimal(drEmpPayroll2["Tpy_YTDTotalTaxableIncomeAmt"])
                                                    - (Convert.ToDecimal(drEmpPayroll2["Tpy_YTDSSSAmt"])
                                                        + Convert.ToDecimal(drEmpPayroll2["Tpy_YTDMPFAmt"])
                                                        + Convert.ToDecimal(drEmpPayroll2["Tpy_YTDPhilhealthAmt"])
                                                        + Convert.ToDecimal(drEmpPayroll2["Tpy_YTDPagIbigAmt"])
                                                        + Convert.ToDecimal(drEmpPayroll2["Tpy_YTDUnionAmt"])
                                                        + Convert.ToDecimal(drEmpPayroll2["Tpy_TotalExemptions"])
                                                        + Convert.ToDecimal(drEmpPayroll2["Tpy_PremiumPaidOnHealth"])), 2);
                        drEmpPayroll2["Tpy_NetTaxableIncomeAmt"] = dTotalGrossAmt;
                        #endregion
                    }
                    #endregion

                    #region Get Annualized Tax Amount
                    if (dTotalGrossAmt > 0)
                    {
                        dsAnnualTaxAmt = GetAnnualizedTaxAmount(dTotalGrossAmt);
                        if (dsAnnualTaxAmt.Tables[0].Rows.Count > 0)
                        {
                            drEmpPayroll["Tpy_WtaxAmt"] = Math.Round(Convert.ToDecimal(dsAnnualTaxAmt.Tables[0].Rows[0][0].ToString()), 2);
                            drEmpPayroll["Tpy_TaxBracket"] = Convert.ToInt32(dsAnnualTaxAmt.Tables[0].Rows[0][1].ToString());
                            drEmpPayroll2["Tpy_TaxDue"] = Math.Round(Convert.ToDecimal(dsAnnualTaxAmt.Tables[0].Rows[0][0].ToString()), 2);
                        }
                    }
                    #endregion

                    #region Get Tax Withheld/Refund
                    if (TXANNLQUINFORMULA2 == "1" && !bProcessLastPay) //TaxSchedule != "LASTPAY"
                    {
                        #region LEAR
                        dWTaxAmt = Convert.ToDecimal(drEmpPayroll["Tpy_WtaxAmt"].ToString());
                        if (iPayPeriodDiv * iPayPeriodBase != 0)
                        {
                            dWTaxAmt = dWTaxAmt / (iPayPeriodBase * iPayPeriodDiv);
                            dWTaxAmt -= Convert.ToDecimal(drEmpPayroll2["Tpy_YTDWtaxAmt"].ToString());
                            if (ISLASTCYCLE == false && dWTaxAmt < 0 && iPayPeriodDiv != 24)
                                dWTaxAmt = 0;
                        }
                        else
                            dWTaxAmt = 0;
                        #endregion
                    }
                    else
                    {
                        #region Year-end Zero-out Tax / TAIHEIYO
                        dWTaxAmt = Convert.ToDecimal(drEmpPayroll["Tpy_WtaxAmt"].ToString())
                                                        - Convert.ToDecimal(drEmpPayroll2["Tpy_YTDWtaxAmt"].ToString());
                        #endregion
                    }

                    #region TAIHEIYO
                    if (TXANNLQUINFORMULA1 == "1" && !bProcessLastPay) //TaxSchedule != "LASTPAY"
                    {
                        dWTaxAmt /= (iPayPeriodDiv + 1);
                    }
                    #endregion

                    #endregion

                    #region Save Payroll Calc Columns
                    //Tax Payable/Refund
                    drEmpPayroll["Tpy_WtaxAmt"] = dWTaxAmt;

                    if (Convert.ToDecimal(drEmpPayroll2["Tpy_RemainingPayCycle"]) == 0)
                        drEmpPayroll2["Tpy_TotalTaxableIncomeWAssumeAmt"] = 0;

                    //Update year-to-date gross amount
                    drEmpPayroll2["Tpy_YTDTotalTaxableIncomeAmt"] = Convert.ToDecimal(drEmpPayroll2["Tpy_YTDTotalTaxableIncomeAmt"])
                                                                        - (Convert.ToDecimal(drEmpPayroll2["Tpy_YTDSSSAmt"].ToString())
                                                                            + Convert.ToDecimal(drEmpPayroll2["Tpy_YTDMPFAmt"].ToString())
                                                                            + Convert.ToDecimal(drEmpPayroll2["Tpy_YTDPhilhealthAmt"].ToString())
                                                                            + Convert.ToDecimal(drEmpPayroll2["Tpy_YTDPagIbigAmt"].ToString()));

                    //Update year-to-date withholding tax
                    drEmpPayroll2["Tpy_YTDWtaxAmt"] = Convert.ToDecimal(drEmpPayroll2["Tpy_YTDWtaxAmtBefore"].ToString())
                                                       + Convert.ToDecimal(drEmpPayroll["Tpy_WtaxAmt"].ToString());
                    #endregion
                }
                else
                {
                    #region Tax Exempted
                    drEmpPayroll["Tpy_WtaxAmt"]    = Convert.ToDecimal(drEmpPayroll2["Tpy_YTDWtaxAmt"].ToString()) * -1;
                    drEmpPayroll2["Tpy_YTDWtaxAmt"] = 0;
                    #endregion
                }
                #endregion
            }
        }

        public void ComputeIncomeAndDeductionAdjustments(bool ProcessSeparated, DataRow drEmpPayroll, DataRow drEmpPayroll2)
        {
            DataSet dsIncomeDednAdj = GetIncomeDeductionAdjustments(ProcessSeparated, drEmpPayroll["Tpy_IDNo"].ToString());

            foreach (DataRow drIncomeDednAdj in dsIncomeDednAdj.Tables[0].Rows)
            {
                drEmpPayroll2["Tpy_BIRTotalAmountofCompensation"] = Convert.ToDecimal(drEmpPayroll["Tpy_GrossIncomeAmt"]) - Convert.ToDecimal(drIncomeDednAdj["NOTINCLUDE"].ToString());

                drEmpPayroll2["Tpy_BIR13thMonthPayOtherBenefits"] = Convert.ToDecimal(drIncomeDednAdj["N13THBEN"].ToString());
                drEmpPayroll2["Tpy_BIRDeMinimisBenefits"] = Convert.ToDecimal(drIncomeDednAdj["NDEMINIMIS"].ToString());
                drEmpPayroll2["Tpy_BIRSSSGSISPHICHDMFUnionDues"] = Convert.ToDecimal(drEmpPayroll["Tpy_SSSEE"])
                                                                    + Convert.ToDecimal(drEmpPayroll["Tpy_MPFEE"])
                                                                    + Convert.ToDecimal(drEmpPayroll["Tpy_PagIbigEE"])
                                                                    + Convert.ToDecimal(drEmpPayroll["Tpy_PhilhealthEE"])
                                                                    + Convert.ToDecimal(drEmpPayroll["Tpy_UnionAmt"])
                                                                    + Convert.ToDecimal(drIncomeDednAdj["DNPREMIUMS"].ToString())
                                                                    - Convert.ToDecimal(drIncomeDednAdj["INPREMIUMS"].ToString());

                if (Convert.ToBoolean(drEmpPayroll["Tpy_IsTaxExempted"]))
                {
                    drEmpPayroll2["Tpy_BIRStatutoryMinimumWage"] = (Convert.ToDecimal(drEmpPayroll["Tpy_REGAmt"])
                                                                    + Convert.ToDecimal(drEmpPayroll["Tpy_PDLVAmt"])
                                                                    + Convert.ToDecimal(drEmpPayroll["Tpy_SRGAdjAmt"])
                                                                    + Convert.ToDecimal(drEmpPayroll["Tpy_SLVAdjAmt"])
                                                                    + Convert.ToDecimal(drEmpPayroll["Tpy_MRGAdjAmt"])
                                                                    + Convert.ToDecimal(drIncomeDednAdj["BASICSAL"].ToString()))
                                                                    - Convert.ToDecimal(drEmpPayroll2["Tpy_BIRSSSGSISPHICHDMFUnionDues"]);

                    drEmpPayroll2["Tpy_BIRHolidayOvertimeNightShiftHazard"] = Convert.ToDecimal(drEmpPayroll["Tpy_TotalOTNDAmt"])
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_PDLEGHOLAmt"])
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_PDSPLHOLAmt"])
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_PDCOMPHOLAmt"])
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_PDOTHHOLAmt"])
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_PDRESTLEGHOLAmt"])
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_SHOLAdjAmt"])
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_MHOLAdjAmt"])
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_SOTAdjAmt"])
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_MOTAdjAmt"])
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_SNDAdjAmt"])
                                                                            + Convert.ToDecimal(drEmpPayroll["Tpy_MNDAdjAmt"])
                                                                            + Convert.ToDecimal(drIncomeDednAdj["HolidayOvertimeNightShiftHazard"].ToString());
                }

                if (Convert.ToDecimal(drEmpPayroll["Tpy_WtaxAmt"]) < 0)
                    drEmpPayroll2["Tpy_BIROtherNonTaxableCompensation"] = (Convert.ToDecimal(drEmpPayroll["Tpy_WtaxAmt"]) * -1)
                                                                            + Convert.ToDecimal(drIncomeDednAdj["NSALOTH"].ToString());
                else
                    drEmpPayroll2["Tpy_BIROtherNonTaxableCompensation"] = Convert.ToDecimal(drIncomeDednAdj["NSALOTH"].ToString());

                drEmpPayroll2["Tpy_BIRTotalTaxesWithheld"] = (Convert.ToDecimal(drEmpPayroll["Tpy_WtaxAmt"]) + Convert.ToDecimal(drIncomeDednAdj["DWTAX"].ToString()))
                                                                - Convert.ToDecimal(drIncomeDednAdj["IWTAX"].ToString());

                drEmpPayroll2["Tpy_BIRTotalTaxableCompensation"] = Convert.ToDecimal(drEmpPayroll2["Tpy_BIRTotalAmountofCompensation"])
                                                                   - (Convert.ToDecimal(drEmpPayroll2["Tpy_BIRStatutoryMinimumWage"])
                                                                        + Convert.ToDecimal(drEmpPayroll2["Tpy_BIRHolidayOvertimeNightShiftHazard"])
                                                                        + Convert.ToDecimal(drEmpPayroll2["Tpy_BIR13thMonthPayOtherBenefits"])
                                                                        + Convert.ToDecimal(drEmpPayroll2["Tpy_BIRDeMinimisBenefits"])
                                                                        + Convert.ToDecimal(drEmpPayroll2["Tpy_BIRSSSGSISPHICHDMFUnionDues"])
                                                                        + Convert.ToDecimal(drEmpPayroll2["Tpy_BIROtherNonTaxableCompensation"]));

                if (!Convert.ToBoolean(drEmpPayroll["Tpy_IsTaxExempted"]) && Convert.ToDecimal(drEmpPayroll["Tpy_WtaxAmt"]) == 0)
                    drEmpPayroll2["Tpy_BIRTaxableCompensationNotSubjectToWithholdingTax"] = Convert.ToDecimal(drEmpPayroll2["Tpy_BIRTotalTaxableCompensation"]);
            }
        }

        public DataTable GetAllEmployeeIncomeAndDeductionSeparated(bool ProcessAll
                                                                    , string EmployeeId
                                                                    , string EmployeeList
                                                                    , DALHelper dal)
        {
            string EmployeeConditionDeduction = "";
            string EmployeeConditionIncome = "";
            if (!ProcessAll && EmployeeId != "")
            {
                EmployeeConditionDeduction += " AND Tdd_IDNo = '" + EmployeeId + "' ";
                EmployeeConditionIncome += " AND Tin_IDNo = '" + EmployeeId + "' ";
            }
            
            else if (ProcessAll && EmployeeList != "")
            {
                EmployeeConditionDeduction += " AND Tdd_IDNo IN (" + EmployeeList + ") ";
                EmployeeConditionIncome += " AND Tin_IDNo IN (" + EmployeeList + ") ";
            }
                

            #region query
            string query = string.Format(@"SELECT PayCycle, IDNo
				                                        , SUM(CASE WHEN DGroup IN ('SA','SR') OR DGroup IN ('GSA','GSR') THEN Amount ELSE 0 END) as SSS
                                                        , SUM(CASE WHEN DGroup IN ('MA','MR') OR DGroup IN ('GMA','GMR') THEN Amount ELSE 0 END) as MPF
				                                        , SUM(CASE WHEN DGroup IN ('PA','PR') OR DGroup IN ('GPA','GPR') THEN Amount ELSE 0 END) as PHIC
				                                        , SUM(CASE WHEN DGroup IN ('HA','HR') OR DGroup IN ('GHA','GHR') THEN Amount ELSE 0 END) as HDMF
				                                        , SUM(CASE WHEN DGroup = 'TX' OR DGroup = 'TAX' THEN Amount ELSE 0 END) as TAX
                                                        , SUM(CASE WHEN DGroup IN ('UA','UR') OR DGroup IN ('GUA','GUR') THEN Amount ELSE 0 END) as UNIONDUES
		                                         FROM (
                                                        SELECT LEFT(Tdd_ThisPayCycle,6) as PayCycle, Tdd_IDNo as IDNo, Mdn_DeductionGroup as DGroup, Tdd_Amount as Amount 
                                                        FROM T_EmpDeductionDtlFinalPay
                                                        INNER JOIN {4}..M_Deduction ON Tdd_DeductionCode = Mdn_DeductionCode
	                                                        AND Mdn_DeductionGroup IN ('SA','SR','MA','MR','PA','PR','HA','HR','TX','UA','UR')
	                                                        AND Mdn_CompanyCode = '{3}'
                                                        INNER JOIN T_EmpPayrollFinalPay ON Tpy_IDNo = Tdd_IDNo	
	                                                        AND Tpy_PayCycle = Tdd_ThisPayCycle
                                                        WHERE Tdd_PaymentFlag = 1
	                                                        AND Tdd_ThisPayCycle = '{2}'
                                                            {0}
 
                                                        UNION ALL
 
                                                        SELECT LEFT(Tin_PayCycle,6) as PayCycle, Tin_IDNo as IDNo, Min_IncomeGroup as DGroup, Tin_IncomeAmt * -1 as Amount 
                                                        FROM T_EmpIncomeFinalPay
                                                        INNER JOIN {4}..M_Income ON Min_IncomeCode = Tin_IncomeCode
	                                                        AND Min_IncomeGroup IN ('GSA','GSR','GMA','GMR','GPA','GPR','GHA','GHR','TAX','GUA','GUR')
	                                                        AND Min_CompanyCode = '{3}'
                                                        INNER JOIN T_EmpPayrollFinalPay ON Tpy_IDNo = Tin_IDNo	
	                                                        AND Tpy_PayCycle = Tin_PayCycle  
                                                        WHERE Tin_PostFlag = 1
	                                                        AND Tin_PayCycle = '{2}'
                                                            {1} ) DEDINCOME 
                                                   GROUP BY PayCycle, IDNo
                                              ", EmployeeConditionDeduction
                                              , EmployeeConditionIncome
                                              , ProcessPayrollPeriod
                                              , CompanyCode
                                              , CentralProfile);
            #endregion
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }
        public DataSet GetYTDAdjustment(string EmployeeId, string Year, DALHelper dal)
        {
            string EmployeeCondition = "";
            if (EmployeeId != "")
                EmployeeCondition += " AND Tyt_IDNo = '" + EmployeeId + "' ";

            string query = string.Format(@"/*PRESENT*/
                                            SELECT Tyt_IDNo, 
                                             ISNULL(SUM([TaxIncomeAmt]), 0.00) [TaxIncomeAmt]
                                             , ISNULL(SUM([PremiumAmt]), 0.00) [PremiumAmt]
                                             , ISNULL(SUM([PremiumPaid]), 0.00) [PremiumPaid]
                                             , ISNULL(SUM([WtaxAmt]), 0.00) [WtaxAmt] 
                                             , ISNULL(SUM([T13thMonth]), 0.00) [T13thMonth] 
                                             , ISNULL(SUM([N13thMonth]), 0.00) [N13thMonth] 
                                             , ISNULL(SUM([BasicSalary]), 0.00) [BasicSalary] 
                                             , ISNULL(SUM([TSalariesOtherFormsCompensation]), 0.00) [TSalariesOtherFormsCompensation] 
                                             , ISNULL(SUM([TOvertimePay]), 0.00) [TOvertimePay] 
                                             , ISNULL(SUM([NSalOth]), 0.00) [NSalOth] 
                                             , ISNULL(SUM([MWEAmt]), 0.00) [MWEAmt] 
                                            FROM ( 
                                             SELECT Tyt_IDNo,
                                                  ISNULL( Tyt_Taxable13thMonth
                                                   + Tyt_TaxableBasic
                                                   + Tyt_TaxableSalariesCompensation
                                                   + Tyt_TaxableOvertime, 0)  [TaxIncomeAmt]
                                                  , ISNULL(Tyt_PremiumsUnionDues, 0) [PremiumAmt]
                                                  , ISNULL(Tyt_PremiumPaidOnHealth, 0)  [PremiumPaid]
                                                  , ISNULL(Tyt_TaxWithheld, 0) [WtaxAmt]
                                                  , ISNULL(Tyt_Taxable13thMonth, 0) [T13thMonth]
                                                  , ISNULL(Tyt_Nontaxable13thMonth, 0) [N13thMonth]
                                                  , ISNULL(Tyt_TaxableBasic, 0) [BasicSalary]
                                                  , ISNULL(Tyt_TaxableSalariesCompensation, 0) [TSalariesOtherFormsCompensation]
                                                  , ISNULL(Tyt_TaxableOvertime, 0) [TOvertimePay]
                                                  , ISNULL(Tyt_Nontaxable13thMonth
                                                            + Tyt_DeMinimis
                                                            + Tyt_NontaxableSalariesCompensation, 0) [NSalOth]
                                                  , ISNULL(Tyt_MWEBasic
                                                            + Tyt_MWEHoliday 
                                                            + Tyt_MWEOvertime 
                                                            + Tyt_MWENightShift 
                                                            + Tyt_MWEHazard, 0) [MWEAmt]
                                             FROM {2}..T_EmpYTD
                                             WHERE Tyt_EmployerType = 'C'
                                             AND Tyt_TaxYear = '{1}' {0}

                                             UNION ALL

                                             SELECT Tyt_IDNo, 
                                                  ISNULL( Tyt_Taxable13thMonth
                                                   + Tyt_TaxableBasic
                                                   + Tyt_TaxableSalariesCompensation
                                                   + Tyt_TaxableOvertime, 0)  [TaxIncomeAmt]
                                                  , ISNULL(Tyt_PremiumsUnionDues, 0) [PremiumAmt]
                                                  , ISNULL(Tyt_PremiumPaidOnHealth, 0)  [PremiumPaid]
                                                  , ISNULL(Tyt_TaxWithheld, 0) [WtaxAmt]
                                                  , ISNULL(Tyt_Taxable13thMonth, 0) [T13thMonth]
                                                  , ISNULL(Tyt_Nontaxable13thMonth, 0) [N13thMonth]
                                                  , ISNULL(Tyt_TaxableBasic, 0) [BasicSalary]
                                                  , ISNULL(Tyt_TaxableSalariesCompensation, 0) [TSalariesOtherFormsCompensation]
                                                  , ISNULL(Tyt_TaxableOvertime, 0) [TOvertimePay]
                                                  , ISNULL(Tyt_Nontaxable13thMonth
                                                            + Tyt_DeMinimis
                                                            + Tyt_NontaxableSalariesCompensation, 0) [NSalOth]
                                                  , ISNULL(Tyt_MWEBasic
                                                            + Tyt_MWEHoliday 
                                                            + Tyt_MWEOvertime 
                                                            + Tyt_MWENightShift 
                                                            + Tyt_MWEHazard, 0) [MWEAmt]
                                             FROM {2}..T_EmpYTDHst
                                             WHERE Tyt_EmployerType = 'C'
                                                AND Tyt_TaxYear = '{1}' {0} 
                                            
                                            UNION ALL

                                            SELECT Tyt_IDNo,
                                                  ISNULL( Tyt_Taxable13thMonth
                                                   + Tyt_TaxableBasic
                                                   + Tyt_TaxableSalariesCompensation
                                                   + Tyt_TaxableOvertime, 0)  [TaxIncomeAmt]
                                                  , ISNULL(Tyt_PremiumsUnionDues, 0) [PremiumAmt]
                                                  , ISNULL(Tyt_PremiumPaidOnHealth, 0)  [PremiumPaid]
                                                  , ISNULL(Tyt_TaxWithheld, 0) [WtaxAmt]
                                                  , ISNULL(Tyt_Taxable13thMonth, 0) [T13thMonth]
                                                  , ISNULL(Tyt_Nontaxable13thMonth, 0) [N13thMonth]
                                                  , ISNULL(Tyt_TaxableBasic, 0) [BasicSalary]
                                                  , ISNULL(Tyt_TaxableSalariesCompensation, 0) [TSalariesOtherFormsCompensation]
                                                  , ISNULL(Tyt_TaxableOvertime, 0) [TOvertimePay]
                                                  , ISNULL(Tyt_Nontaxable13thMonth
                                                            + Tyt_DeMinimis
                                                            + Tyt_NontaxableSalariesCompensation, 0) [NSalOth]
                                                  , ISNULL(Tyt_MWEBasic
                                                            + Tyt_MWEHoliday 
                                                            + Tyt_MWEOvertime 
                                                            + Tyt_MWENightShift 
                                                            + Tyt_MWEHazard, 0) [MWEAmt]
                                             FROM {2}..T_EmpYTD
                                             WHERE Tyt_EmployerType = 'M'
                                             AND Tyt_TaxYear = '{1}' {0} ) AS TEMPTABLE1
                                             GROUP BY Tyt_IDNo

                                            /*PREVIOUS*/
                                            SELECT Tyt_IDNo,
                                             ISNULL(SUM([TaxIncomeAmt]), 0.00) [TaxIncomeAmt]
                                                 , ISNULL(SUM([PremiumAmt]), 0.00) [PremiumAmt]
                                                 , ISNULL(SUM([PremiumPaid]), 0.00) [PremiumPaid]
                                                 , ISNULL(SUM([WtaxAmt]), 0.00) [WtaxAmt]
                                                 , ISNULL(SUM([T13thMonth]), 0.00) [T13thMonth] 
                                                 , ISNULL(SUM([N13thMonth]), 0.00) [N13thMonth] 
                                                 , ISNULL(SUM([BasicSalary]), 0.00) [BasicSalary] 
                                                 , ISNULL(SUM([TSalariesOtherFormsCompensation]), 0.00) [TSalariesOtherFormsCompensation] 
                                                 , ISNULL(SUM([TOvertimePay]), 0.00) [TOvertimePay] 
                                                 , ISNULL(SUM([NSalOth]), 0.00) [NSalOth] 
                                                 , ISNULL(SUM([MWEAmt]), 0.00) [MWEAmt]  
                                            FROM ( 
                                             SELECT Tyt_IDNo,
                                                  ISNULL( Tyt_Taxable13thMonth
                                                   + Tyt_TaxableBasic
                                                   + Tyt_TaxableSalariesCompensation
                                                   + Tyt_TaxableOvertime, 0)  [TaxIncomeAmt]
                                                  , ISNULL(Tyt_PremiumsUnionDues, 0) [PremiumAmt]
                                                  , ISNULL(Tyt_PremiumPaidOnHealth, 0)  [PremiumPaid]
                                                  , ISNULL(Tyt_TaxWithheld, 0) [WtaxAmt]
                                                  , ISNULL(Tyt_Taxable13thMonth, 0) [T13thMonth]
                                                  , ISNULL(Tyt_Nontaxable13thMonth, 0) [N13thMonth]
                                                  , ISNULL(Tyt_TaxableBasic, 0) [BasicSalary]
                                                  , ISNULL(Tyt_TaxableSalariesCompensation, 0) [TSalariesOtherFormsCompensation]
                                                  , ISNULL(Tyt_TaxableOvertime, 0) [TOvertimePay]
                                                  , ISNULL(Tyt_Nontaxable13thMonth
                                                            + Tyt_DeMinimis
                                                            + Tyt_NontaxableSalariesCompensation, 0) [NSalOth]
                                                 , ISNULL(Tyt_MWEBasic
                                                            + Tyt_MWEHoliday 
                                                            + Tyt_MWEOvertime 
                                                            + Tyt_MWENightShift 
                                                            + Tyt_MWEHazard, 0) [MWEAmt]
                                             FROM {2}..T_EmpYTD
                                             WHERE Tyt_EmployerType = 'P'
                                                AND Tyt_TaxYear = '{1}' {0}

                                             UNION ALL

                                             SELECT Tyt_IDNo, 
                                              ISNULL( Tyt_Taxable13thMonth
                                               + Tyt_TaxableBasic
                                               + Tyt_TaxableSalariesCompensation
                                               + Tyt_TaxableOvertime, 0)  [TaxIncomeAmt]
                                              , ISNULL(Tyt_PremiumsUnionDues, 0) [PremiumAmt]
                                              , ISNULL(Tyt_PremiumPaidOnHealth, 0)  [PremiumPaid]
                                              , ISNULL(Tyt_TaxWithheld, 0) [WtaxAmt]
                                              , ISNULL(Tyt_Taxable13thMonth, 0) [T13thMonth]
                                              , ISNULL(Tyt_Nontaxable13thMonth, 0) [N13thMonth]
                                              , ISNULL(Tyt_TaxableBasic, 0) [BasicSalary]
                                              , ISNULL(Tyt_TaxableSalariesCompensation, 0) [TSalariesOtherFormsCompensation]
                                              , ISNULL(Tyt_TaxableOvertime, 0) [TOvertimePay]
                                              , ISNULL(Tyt_Nontaxable13thMonth
                                                            + Tyt_DeMinimis
                                                            + Tyt_NontaxableSalariesCompensation, 0) [NSalOth]
                                              , ISNULL(Tyt_MWEBasic
                                                            + Tyt_MWEHoliday 
                                                            + Tyt_MWEOvertime 
                                                            + Tyt_MWENightShift 
                                                            + Tyt_MWEHazard, 0) [MWEAmt]
                                             FROM {2}..T_EmpYTDHst
                                             WHERE Tyt_EmployerType = 'P'
                                                AND Tyt_TaxYear = '{1}' {0} ) AS TEMPTABLE2
                                             GROUP BY Tyt_IDNo "
                                            , EmployeeCondition
                                            , Year.Substring(0, 4)
                                            , CentralProfile);
            DataSet ds = null;
            ds = dal.ExecuteDataSet(query);
            return ds;
        }

        public decimal GetTaxExemptionAmount(string TaxCode)
        {
            #region query
            string query = string.Format(@"DECLARE @TAXEXEMPTIONPERIOD AS VARCHAR(10)
                                            SET @TAXEXEMPTIONPERIOD = (SELECT MAX(Mte_PayCycle) as [PayPeriod]
                                                                       FROM {7}..M_TaxExemption
                                                                       WHERE Mte_PayCycle <= '{5}'
                                                                            AND Mte_RecordStatus = 'A'
                                                                            AND Mte_CompanyCode = '{6}')

                                            SELECT M_TaxExemption.Mte_Amount + 
                                               (SELECT CASE WHEN LEN(RTrim(Right( '{0}',1 ))) = 0 THEN 
                                                        0 
                                                ELSE  
                                                   CASE 
                                                        WHEN Right( '{0}', 1) = '1' THEN convert(decimal,{1})
                                                        WHEN Right( '{0}', 1) = '2' THEN convert(decimal,{2})
                                                        WHEN Right( '{0}', 1) = '3' THEN convert(decimal,{3})
                                                        WHEN Right( '{0}', 1) = '4' THEN convert(decimal,{4})
                                                        ELSE 0 END  
                                                    END )
                                             FROM {7}..M_TaxExemption
                                             WHERE M_TaxExemption.Mte_RecordStatus = 'A'
                                                   AND M_TaxExemption.Mte_PayCycle = @TAXEXEMPTIONPERIOD
                                                   AND M_TaxExemption.Mte_CompanyCode = '{6}'
                                                   AND M_TaxExemption.Mte_TaxCode = Rtrim(ISNULL(LEFT('{0}',2),'S'))"
                                            , TaxCode
                                            , TaxExemption1
                                            , TaxExemption2
                                            , TaxExemption3
                                            , TaxExemption4
                                            , MaxTaxAnnual
                                            , CompanyCode
                                            , CentralProfile);
            #endregion
            DataSet ds = null;
            decimal dTaxExemption = 0;
            ds = dal.ExecuteDataSet(query);
            if (ds.Tables[0].Rows.Count > 0)
                dTaxExemption = Convert.ToDecimal(ds.Tables[0].Rows[0][0]);
            return dTaxExemption;
        }

        public decimal CheckMaxShareCeiling(decimal dShare, decimal dMonthlyShare, decimal dMaxShare)
        {
            if (dShare + dMonthlyShare >= dMaxShare)
            {
                if (dShare >= dMaxShare - dMonthlyShare)
                    dShare = dMaxShare - dMonthlyShare;
                else
                    dShare = 0;
            }
            else
            {
                if (bMonthEnd)
                    dShare = dMaxShare - dMonthlyShare;
            }
            return dShare;
        }

        public int GetNextQuincenaCount(string PayPeriod, string EmployeeID)
        {
            #region query
            string query = string.Format(@"DECLARE @EmployeeID VARCHAR(15) = '{1}'
                                            DECLARE @StartPayPeriod VARCHAR(7) = '{0}'
                                            DECLARE @EndPayPeriod VARCHAR(7)
                                            DECLARE @StartCurrentCycle DATETIME

                                            SELECT @StartCurrentCycle = Tps_StartCycle
                                            FROM T_PaySchedule
                                            WHERE Tps_PayCycle = @StartPayPeriod

                                            SELECT @StartPayPeriod = CASE WHEN Mem_SeparationDate IS NOT NULL AND Mem_SeparationDate < @StartCurrentCycle
						                                            THEN (SELECT TOP 1 Tps_PayCycle
								                                            FROM T_PaySchedule
								                                            WHERE Mem_SeparationDate BETWEEN Tps_StartCycle AND Tps_EndCycle
									                                            AND Tps_CycleIndicator = 'P'
									                                            AND Tps_RecordStatus = 'A')
						                                            WHEN 'S' = (SELECT Tps_CycleIndicator
									                                            FROM T_PaySchedule
									                                            WHERE Tps_PayCycle = @StartPayPeriod
										                                            AND Tps_RecordStatus = 'A')
						                                            THEN (SELECT TOP 1 Tps_PayCycle
								                                            FROM T_PaySchedule
								                                            WHERE Tps_CycleIndicator = 'C'
									                                            AND Tps_RecordStatus = 'A')
						                                            ELSE @StartPayPeriod
						                                            END
                                            FROM M_Employee
                                            WHERE Mem_IDNo = @EmployeeID

                                            SET @EndPayPeriod = LEFT(@StartPayPeriod, 4) + '122'

                                            SELECT COUNT(Tps_PayCycle)
                                            FROM T_PaySchedule
                                            WHERE Tps_PayCycle <= @EndPayPeriod
                                                AND Tps_PayCycle > @StartPayPeriod 
                                                AND Tps_RecordStatus = 'A'
                                                AND Tps_CycleIndicator IN ('P','C','F')"
                                                , PayPeriod, EmployeeID);
            #endregion
            DataSet ds = null;
            int iCnt = 0;
            ds = dal.ExecuteDataSet(query);
            if (ds.Tables[0].Rows.Count > 0)
            {
                iCnt = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            }
            return iCnt;
        }

        public int GetTaxablePeriods(string PayPeriod, string EmployeeID)
        {
            #region query
            string query = string.Format(@"DECLARE @EmployeeID VARCHAR(15) = '{1}'
                                            DECLARE @Year VARCHAR(4) = '{0}'

											DECLARE @StartDateOfYear DATETIME = (
	                                            SELECT TOP 1 Tps_StartCycle
	                                            FROM T_PaySchedule
	                                            WHERE LEFT(Tps_PayCycle, 4) = @Year
	                                                AND Tps_CycleIndicator IN ('P','C')
	                                                AND Tps_RecordStatus = 'A'
												ORDER BY Tps_PayCycle)

                                            DECLARE @StartCycle DATETIME = (
	                                            SELECT TOP 1 Tps_StartCycle
	                                            FROM T_PaySchedule
	                                            WHERE ISNULL(
															  (SELECT TOP 1 CASE WHEN Tyt_StartDate <= @StartDateOfYear 
																		THEN @StartDateOfYear 
																		ELSE Tyt_StartDate
																		END
																FROM T_EmpYTD
																WHERE Tyt_IDNo = @EmployeeID
																	AND Tyt_EmployerType = 'P'
                                                                    AND Tyt_TaxYear = @Year
                                                                ORDER BY Tyt_StartDate DESC) --GET PREVIOUS EMPLOYER START DATE
															, (SELECT CASE WHEN Mem_IntakeDate <= @StartDateOfYear
																		THEN @StartDateOfYear
																		ELSE Mem_IntakeDate 
																		END
																FROM M_Employee
																WHERE  Mem_IDNo = @EmployeeID) --ELSE GET CURRENT EMPLOYER HIRE DATE
														) 
				                                            BETWEEN Tps_StartCycle AND Tps_EndCycle
	                                            AND Tps_CycleIndicator IN ('P','C')
	                                            AND Tps_RecordStatus = 'A')

                                            DECLARE @EndCycle DATETIME = (
	                                            SELECT TOP 1 Tps_EndCycle
	                                            FROM T_PaySchedule
	                                            WHERE Tps_CycleIndicator = 'C')

                                            --OVERRIDE END DATE IF SEPARATED
                                            SELECT @EndCycle = CASE WHEN Mem_SeparationDate IS NOT NULL 
					                                            THEN Mem_SeparationDate
					                                            ELSE @EndCycle
					                                            END
                                            FROM M_Employee
                                            WHERE Mem_IDNo = @EmployeeID

                                            --OVERRIDE START DATE IF SEPARATED
                                            SELECT @StartCycle = CASE WHEN Mem_SeparationDate IS NOT NULL 
					                                            THEN CAST(DATEPART(YEAR, Mem_SeparationDate) AS VARCHAR)
						                                            + '-'
						                                            + CAST(DATEPART(MONTH, @StartCycle) AS VARCHAR)
						                                            + '-'
						                                            + CAST(DATEPART(DAY, @StartCycle) AS VARCHAR)
					                                            ELSE @StartCycle
					                                            END
                                            FROM M_Employee
                                            WHERE Mem_IDNo = @EmployeeID

                                            SELECT ISNULL((SELECT COUNT(*) AS [CNT] 
                                            FROM T_PaySchedule
                                            WHERE @EndCycle >= Tps_StartCycle 
	                                            AND Tps_EndCycle >= @StartCycle
	                                            AND Tps_CycleIndicator IN ('P','C') --PAST AND CURRENT
	                                            AND Tps_RecordStatus = 'A'), 0)

                                            - ISNULL((SELECT COUNT(*) AS [CNT] 
                                            FROM M_EmployeeHst
                                            INNER JOIN T_PaySchedule ON Tps_PayCycle = Mem_PayCycle
	                                            AND Tps_CycleIndicator = 'P'
                                            WHERE LEFT(Mem_PayCycle, 4) = @Year
	                                            AND Mem_IsTaxExempted = 1 --NONTAXABLE PAY PERIOD (PAST)
	                                            AND Mem_IDNo = @EmployeeID
                                            GROUP BY Mem_IDNo), 0)

                                            - ISNULL((SELECT COUNT(*) AS [CNT] 
                                            FROM M_Employee
                                            WHERE Mem_IsTaxExempted = 1 --NONTAXABLE PAY PERIOD (CURRENT)
	                                            AND Mem_IDNo = @EmployeeID
                                            GROUP BY Mem_IDNo), 0)"
                                                , PayPeriod.Substring(0, 4), EmployeeID);
            #endregion
            DataSet ds = null;
            int iCnt = 0;
            ds = dal.ExecuteDataSet(query);
            if (ds.Tables[0].Rows.Count > 0)
            {
                iCnt = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            }
            return iCnt;
        }

        public DataSet GetAnnualizedTaxAmount(decimal SalaryAmount)
        {
            #region query
            string query = string.Format(@"DECLARE @TAXANNUALPERIOD AS VARCHAR(10)
                                            SET @TAXANNUALPERIOD = (SELECT MAX(Myt_PayCycle) as [PayPeriod]
                                                                   FROM {3}..M_YearlyTaxSchedule
                                                                   WHERE Myt_PayCycle <= '{1}'
                                                                        AND Myt_RecordStatus = 'A'
                                                                        AND Myt_CompanyCode  = '{2}')

                                            SELECT Myt_TaxOnCompensationLevel + (( {0:0.00} - Myt_ExcessOverCompensationLevel) * (Myt_TaxOnExcess/100))
                                            , Myt_BracketNo
                                            FROM {3}..M_YearlyTaxSchedule
                                            WHERE M_YearlyTaxSchedule.Myt_RecordStatus = 'A' 
                                                    AND M_YearlyTaxSchedule.Myt_PayCycle = @TAXANNUALPERIOD
                                                     AND M_YearlyTaxSchedule.Myt_CompanyCode = '{2}'
                                                    AND {0:0.00} >= Myt_MinCompensationLevel 
                                                    AND {0:0.00} <= Myt_MaxCompensationLevel"
                                                , SalaryAmount, MaxTaxAnnual,  CompanyCode, CentralProfile);
            #endregion
            DataSet ds = null;
            ds = dal.ExecuteDataSet(query);
            return ds;
        }

        public DataSet GetAnnualizedTaxAmount(decimal SalaryAmount, string MaxPayCycle, string CompanyCode, string CentralProfile, DALHelper dalHelper)
        {
            #region query
            string query = string.Format(@"DECLARE @TAXANNUALPERIOD AS VARCHAR(10)
                                            SET @TAXANNUALPERIOD = (SELECT MAX(Myt_PayCycle) as [PayPeriod]
                                                                   FROM {3}..M_YearlyTaxSchedule
                                                                   WHERE Myt_PayCycle <= '{1}'
                                                                        AND Myt_RecordStatus = 'A'
                                                                        AND Myt_CompanyCode  = '{2}')

                                            SELECT Myt_TaxOnCompensationLevel + (( {0:0.00} - Myt_ExcessOverCompensationLevel) * (Myt_TaxOnExcess/100))
                                            , Myt_BracketNo
                                            FROM {3}..M_YearlyTaxSchedule
                                            WHERE M_YearlyTaxSchedule.Myt_RecordStatus = 'A' 
                                                    AND M_YearlyTaxSchedule.Myt_PayCycle = @TAXANNUALPERIOD
                                                     AND M_YearlyTaxSchedule.Myt_CompanyCode = '{2}'
                                                    AND {0:0.00} >= Myt_MinCompensationLevel 
                                                    AND {0:0.00} <= Myt_MaxCompensationLevel"
                                                , SalaryAmount, MaxPayCycle, CompanyCode, CentralProfile);
            #endregion
            DataSet ds = null;
            ds = dalHelper.ExecuteDataSet(query);
            return ds;
        }

        public decimal GetAllowanceTotal(string EmployeeId, string AlphalistCategoryDelimited, bool GetCurrentPayPeriodOnly)
        {
            string strPayPeriodCondition = "";
            if (GetCurrentPayPeriodOnly == true)
                strPayPeriodCondition = string.Format(" AND Tin_PayCycle = '{0}' ", ProcessPayrollPeriod);
            else
                strPayPeriodCondition = string.Format(" AND LEFT(Tin_PayCycle, 4) = LEFT('{0}', 4) AND Tin_PayCycle != '{0}' ", ProcessPayrollPeriod);

            #region query
            string query = string.Format(@"SELECT SUM(AlwValue) AS AlwValue
                                            FROM (
	                                            SELECT ISNULL(SUM(Tin_IncomeAmt), 0) as AlwValue
	                                            FROM T_EmpIncome
	                                            INNER JOIN {4}..M_Income on Min_IncomeCode = Tin_IncomeCode
		                                            AND Min_AlphalistCategory in ({1})
                                                    AND Min_CompanyCode = '{3}'
                                                    AND Min_RecordStatus = 'A'
	                                            WHERE Tin_IDNo = '{0}'
		                                            {2}
		                                            AND Tin_PostFlag = 1
	                                            UNION ALL
	                                            SELECT ISNULL(SUM(Tin_IncomeAmt), 0) as AlwValue
	                                            FROM T_EmpIncomeHst
	                                            INNER JOIN {4}..M_Income on Min_IncomeCode = Tin_IncomeCode
		                                            AND Min_AlphalistCategory in ({1})
                                                    AND Min_CompanyCode = '{3}'
                                                    AND Min_RecordStatus = 'A'
	                                            WHERE Tin_IDNo = '{0}'
		                                            {2}
		                                            AND Tin_PostFlag = 1
	                                            UNION ALL
	                                            SELECT ISNULL(SUM(Tin_IncomeAmt), 0) as AlwValue
	                                            FROM T_EmpIncomeFinalPay
	                                            INNER JOIN {4}..M_Income on Min_IncomeCode = Tin_IncomeCode
		                                            AND Min_AlphalistCategory in ({1})
                                                    AND Min_CompanyCode = '{3}'
                                                    AND Min_RecordStatus = 'A'
	                                            WHERE Tin_IDNo = '{0}'
		                                            {2}
		                                            AND Tin_PostFlag = 1
                                            ) TEMP"
                                            , EmployeeId
                                            , AlphalistCategoryDelimited
                                            , strPayPeriodCondition
                                            , CompanyCode
                                            , CentralProfile);
            #endregion
            DataSet ds = null;
            decimal dAllowanceAmount = 0;
            ds = dal.ExecuteDataSet(query);
            if (ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows[0][0].ToString() != "")
            {
                dAllowanceAmount = Convert.ToDecimal(ds.Tables[0].Rows[0][0]);
            }
            return dAllowanceAmount;
        }

        public decimal GetRecurringAllowanceTotal(string EmployeeId, bool GetCurrentPayPeriodOnly)
        {
            string strPayPeriodCondition = "";
            if (GetCurrentPayPeriodOnly == true)
                strPayPeriodCondition = string.Format(" AND Tin_PayCycle = '{0}' ", ProcessPayrollPeriod);
            else
                strPayPeriodCondition = string.Format(" AND LEFT(Tin_PayCycle, 4) = LEFT('{0}', 4) AND Tin_PayCycle != '{0}' ", ProcessPayrollPeriod);

            #region query
            string query = string.Format(@"SELECT SUM(AlwValue) AS AlwValue
                                            FROM (
                                                SELECT ISNULL(SUM(Tin_IncomeAmt), 0) as AlwValue
                                                FROM T_EmpIncome
                                                INNER JOIN {3}..M_Income on Min_IncomeCode = Tin_IncomeCode
                                                     AND Min_TaxClass = 'T'
                                                     AND Min_IsRecurring =  1
                                                     AND Min_CompanyCode = '{2}'
                                                     AND Min_RecordStatus = 'A'
                                                WHERE Tin_IDNo = '{0}'
                                                 {1}
                                                 AND Tin_PostFlag = 1
                                                UNION ALL
                                                SELECT ISNULL(SUM(Tin_IncomeAmt), 0) as AlwValue
                                                FROM T_EmpIncomeHst
                                                INNER JOIN {3}..M_Income on Min_IncomeCode = Tin_IncomeCode
                                                     AND Min_TaxClass = 'T'
                                                     AND Min_IsRecurring =  1
                                                     AND Min_CompanyCode = '{2}'
                                                     AND Min_RecordStatus = 'A'
                                                WHERE Tin_IDNo = '{0}'
                                                 {1}
                                                 AND Tin_PostFlag = 1
                                                UNION ALL
                                                SELECT ISNULL(SUM(Tin_IncomeAmt), 0) as AlwValue
                                                FROM T_EmpIncomeFinalPay
                                                INNER JOIN {3}..M_Income on Min_IncomeCode = Tin_IncomeCode
                                                     AND Min_TaxClass = 'T'
                                                     AND Min_IsRecurring =  1
                                                     AND Min_CompanyCode = '{2}'
                                                     AND Min_RecordStatus = 'A'
                                                WHERE Tin_IDNo = '{0}'
                                                 {1}
                                                 AND Tin_PostFlag = 1
                                            ) TEMP"
                                            , EmployeeId
                                            , strPayPeriodCondition
                                            , CompanyCode
                                            , CentralProfile);
            #endregion
            DataSet ds = null;
            decimal dAllowanceAmount = 0;
            ds = dal.ExecuteDataSet(query);
            if (ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows[0][0].ToString() != "")
            {
                dAllowanceAmount = Convert.ToDecimal(ds.Tables[0].Rows[0][0]);
            }
            return dAllowanceAmount;
        }

        public decimal GetMonthlyRecurringAllowance(string EmployeeId)
        {
            #region query
            string query = string.Format(@"SELECT SUM(CASE WHEN Min_ApplicablePayCycle = 0 
                                                    THEN Tfa_Amount * 2
                                                    ELSE Tfa_Amount END)
                                            FROM {3}..T_EmpFixAllowance
                                            INNER JOIN {3}..M_Income on Min_IncomeCode = Tfa_AllowanceCode
                                                 AND Min_TaxClass = 'T'
                                                 AND Min_IsRecurring = 1
                                                 AND Min_CompanyCode = '{2}'
                                                 AND Min_RecordStatus = 'A'   
                                            WHERE  ISNULL(Tfa_EndPayCycle, '{1}') >= '{1}' 
                                                 AND '{1}' >= Tfa_StartPayCycle
                                                 AND Tfa_IDNo = '{0}'"
                                            , EmployeeId
                                            , ProcessPayrollPeriod
                                            , CompanyCode
                                            , CentralProfile);
            #endregion
            DataSet ds = null;
            decimal dAllowanceAmount = 0;
            ds = dal.ExecuteDataSet(query);
            if (ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows[0][0].ToString() != "")
            {
                dAllowanceAmount = Convert.ToDecimal(ds.Tables[0].Rows[0][0]);
            }
            return dAllowanceAmount;
        }

        public DataSet GetWTaxAmount(decimal SalaryAmount, string TaxCode, DALHelper dalhelper)
        {
            #region query
            string query = string.Format(@"SELECT TOP 1 M_TaxScheduleHdr.Mth_CompensationLevelTaxAmount + (({0} - M_TaxScheduleDtl.Mtd_CompensationLevel) * (M_TaxScheduleHdr.Mth_TaxOnExcess/100.00))
                                            , M_TaxScheduleHdr.Mth_BracketNo
                                            FROM {5}..M_TaxScheduleDtl
                                            INNER JOIN {5}..M_TaxScheduleHdr on M_TaxScheduleHdr.Mth_TaxSchedule = M_TaxScheduleDtl.Mtd_TaxSchedule
                                                AND M_TaxScheduleHdr.Mth_PayCycle = M_TaxScheduleDtl.Mtd_PayCycle  
                                                AND M_TaxScheduleHdr.Mth_BracketNo = M_TaxScheduleDtl.Mtd_BracketNo 
                                                AND M_TaxScheduleHdr.Mth_CompanyCode = M_TaxScheduleDtl.Mtd_CompanyCode
                                                AND M_TaxScheduleHdr.Mth_RecordStatus = 'A'
                                            WHERE  M_TaxScheduleHdr.Mth_TaxSchedule = '{1}'
                                                   AND M_TaxScheduleDtl.Mtd_RecordStatus = 'A'
                                                   AND M_TaxScheduleDtl.Mtd_PayCycle = '{3}'
                                                   AND M_TaxScheduleDtl.Mtd_TaxCode = ISNULL('{2}','S') 
                                                   AND M_TaxScheduleDtl.Mtd_CompanyCode = '{4}' 
                                                   AND M_TaxScheduleDtl.Mtd_CompensationLevel < {0}
                                            ORDER BY M_TaxScheduleDtl.Mtd_CompensationLevel DESC"
                                            , SalaryAmount
                                            , TaxSchedule
                                            , TaxCode
                                            , MaxTaxHeader
                                            , CompanyCode
                                            , CentralProfile);
            #endregion
            DataSet ds = null;
            ds = dalhelper.ExecuteDataSet(query);
            return ds;
        }

        public DataSet GetDeductionDetails(string EmployeeId)
        {
            #region query
            string query = string.Format(@"SELECT M_Deduction.Mdn_PriorityNo, M_Deduction.Mdn_IsAllowPartialPayment, {0}.* 
                                          FROM {0}
                                          INNER JOIN {4}..M_Deduction ON Mdn_DeductionCode = Tdd_DeductionCode
                                             AND Mdn_CompanyCode = '{3}'
                                             AND Mdn_RecordStatus = 'A'
                                          WHERE Tdd_IDNo = '{1}'
                                             AND Tdd_ThisPayCycle = '{2}'
                                          ORDER BY Mdn_PriorityNo
                                                  ,Tdd_DeductionCode
                                                  ,Tdd_StartDate
                                                  ,Tdd_PayCycle"
                                        , EmpDeductionDtlTable
                                        , EmployeeId
                                        , ProcessPayrollPeriod
                                        , CompanyCode
                                        , CentralProfile);
            #endregion
            DataSet ds = null;
            ds = dal.ExecuteDataSet(query);
            return ds;
        }

        public DataSet GetIncomeDeductionAdjustments(bool ProcessSeparated, string EmployeeId)
        {
            string IncomeTable = "T_EmpIncome";
            string DednTable = "T_EmpDeductionDtl";
            if (ProcessSeparated)
            {
                IncomeTable = "T_EmpIncomeFinalPay";
                DednTable = "T_EmpDeductionDtlFinalPay";
            }

            #region query
            string query = string.Format(@"SELECT ISNULL([BASICSAL],0) as [BASICSAL]
                                        , ISNULL([HolidayOvertimeNightShiftHazard],0) as [HolidayOvertimeNightShiftHazard]
                                        , ISNULL([N13THBEN],0) as [N13THBEN]
                                        , ISNULL([NDEMINIMIS],0) as [NDEMINIMIS]
                                        , ISNULL([INPREMIUMS],0) as [INPREMIUMS]
                                        , ISNULL([NSALOTH],0) as [NSALOTH]
                                        , ISNULL([IWTAX],0) as [IWTAX]
                                        , ISNULL([NOTINCLUDE],0) as [NOTINCLUDE]
                                        , ISNULL([DNPREMIUMS],0) as [DNPREMIUMS]
                                        , ISNULL([DWTAX],0) as [DWTAX]
                                        FROM M_Employee 
                                        LEFT JOIN (SELECT Tin_IDNo
			                                        , SUM(CASE WHEN Min_AlphalistCategory = 'T-BASICSAL' THEN Tin_IncomeAmt ELSE 0 END) AS [BASICSAL]
			                                        , SUM(CASE WHEN Min_AlphalistCategory IN ('T-HOLIDAYPY','T-OVERTIMEPY','T-NDPAY','T-HAZARDPY') THEN Tin_IncomeAmt ELSE 0 END) AS [HolidayOvertimeNightShiftHazard]
			                                        , SUM(CASE WHEN Min_AlphalistCategory = 'N-13THBEN' THEN Tin_IncomeAmt ELSE 0 END) AS [N13THBEN]
			                                        , SUM(CASE WHEN Min_AlphalistCategory = 'N-DEMINIMIS' THEN Tin_IncomeAmt ELSE 0 END) AS [NDEMINIMIS]
			                                        , SUM(CASE WHEN Min_AlphalistCategory = 'N-PREMIUMS' THEN Tin_IncomeAmt ELSE 0 END) AS [INPREMIUMS]
			                                        , SUM(CASE WHEN Min_AlphalistCategory = 'N-SALOTH' THEN Tin_IncomeAmt ELSE 0 END) AS [NSALOTH]
			                                        , SUM(CASE WHEN Min_AlphalistCategory = 'N-WTAX' THEN Tin_IncomeAmt ELSE 0 END) AS [IWTAX]
			                                        , SUM(CASE WHEN Min_AlphalistCategory = 'N-NOTINCLUDE' THEN Tin_IncomeAmt ELSE 0 END) AS [NOTINCLUDE]
		                                        FROM {4}  
		                                        INNER JOIN {3}..M_Income ON Min_IncomeCode = Tin_IncomeCode
			                                        AND  Min_CompanyCode = '{2}'
			                                        AND Min_AlphalistCategory IN ('T-BASICSAL','T-HOLIDAYPY','T-OVERTIMEPY','T-NDPAY','T-HAZARDPY','N-13THBEN','N-DEMINIMIS','N-PREMIUMS','N-SALOTH','N-WTAX', 'N-NOTINCLUDE')
		                                        WHERE Tin_Paycycle =  '{1}'
			                                        AND Tin_PostFlag =  1
		                                        GROUP BY Tin_IDNo) INCOME ON Tin_IDNo = Mem_IDNo
                                        LEFT JOIN (SELECT Tdd_IDNo
			                                        , SUM(CASE WHEN Mdn_AlphalistCategory = 'N-PREMIUMS' THEN Tdd_Amount ELSE 0 END) AS [DNPREMIUMS]
			                                        , SUM(CASE WHEN Mdn_AlphalistCategory = 'N-WTAX' THEN Tdd_Amount ELSE 0 END) AS [DWTAX]
		                                        FROM {5} 
		                                        INNER JOIN {3}..M_Deduction ON Mdn_DeductionCode = Tdd_DeductionCode
			                                        AND Mdn_CompanyCode = '{2}'
			                                        AND Mdn_AlphalistCategory IN ('N-PREMIUMS','N-WTAX')
		                                        WHERE Tdd_ThisPayCycle = '{1}'
			                                        AND Tdd_PaymentFlag =  1
		                                        GROUP BY Tdd_IDNo) DEDUCTION ON Tdd_IDNo = Mem_IDNo
                                        WHERE Mem_IDNo = '{0}'
                                        "
                                        , EmployeeId
                                        , ProcessPayrollPeriod
                                        , CompanyCode
                                        , CentralProfile
                                        , IncomeTable
                                        , DednTable);
            #endregion
            DataSet ds = null;
            ds = dal.ExecuteDataSet(query);
            return ds;
        }

        public void UpdateEmployeeDeductionDetail(DataRow drDeduction)
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
            //paramUpdPayFlg[paramIndexUpdPayFlg++] = new ParameterInfo("@Currency", Currency);
            #endregion

            #region query
            string query = string.Format(@"UPDATE {0}
                                          SET Tdd_PaymentFlag = 1
                                          WHERE Tdd_IDNo = @Tdd_IDNo 
                                              AND Tdd_DeductionCode = @Tdd_DeductionCode 
                                              AND Tdd_StartDate = @Tdd_StartDate
                                              AND Tdd_ThisPayCycle = @Tdd_ThisPayCycle
                                              AND Tdd_PayCycle = @Tdd_PayCycle
                                              AND Tdd_LineNo = @Tdd_LineNo", EmpDeductionDtlTable);
            #endregion
            dal.ExecuteNonQuery(query, CommandType.Text, paramUpdPayFlg);
        }

        public decimal SplitEmployeeDeductionDetail(decimal NetPay, decimal MinNetpay, DataRow drDeduction)
        {
            #region parameters
            int paramIndexUpdPayFlg = 0;
            ParameterInfo[] paramUpdPayFlg = new ParameterInfo[7];
            paramUpdPayFlg[paramIndexUpdPayFlg++] = new ParameterInfo("@Tdd_IDNo", drDeduction["Tdd_IDNo"]);
            paramUpdPayFlg[paramIndexUpdPayFlg++] = new ParameterInfo("@Tdd_DeductionCode", drDeduction["Tdd_DeductionCode"]);
            paramUpdPayFlg[paramIndexUpdPayFlg++] = new ParameterInfo("@Tdd_StartDate", drDeduction["Tdd_StartDate"]);
            paramUpdPayFlg[paramIndexUpdPayFlg++] = new ParameterInfo("@Tdd_ThisPayCycle", drDeduction["Tdd_ThisPayCycle"]);
            paramUpdPayFlg[paramIndexUpdPayFlg++] = new ParameterInfo("@Tdd_PayCycle", drDeduction["Tdd_PayCycle"]);
            paramUpdPayFlg[paramIndexUpdPayFlg++] = new ParameterInfo("@Tdd_LineNo", drDeduction["Tdd_LineNo"]);
            paramUpdPayFlg[paramIndexUpdPayFlg++] = new ParameterInfo("@Tdd_Amount", drDeduction["Tdd_Amount"]);
            //paramUpdPayFlg[paramIndexUpdPayFlg++] = new ParameterInfo("@Currency", Currency);
            #endregion

            #region query
            string query = string.Format(@"UPDATE {0}
                                          SET Tdd_Amount = {1}
                                            , Tdd_PaymentFlag = 1
                                          WHERE Tdd_IDNo = @Tdd_IDNo 
                                              AND Tdd_DeductionCode = @Tdd_DeductionCode 
                                              AND Tdd_StartDate = @Tdd_StartDate
                                              AND Tdd_ThisPayCycle = @Tdd_ThisPayCycle
                                              AND Tdd_PayCycle = @Tdd_PayCycle
                                              AND Tdd_LineNo = @Tdd_LineNo", EmpDeductionDtlTable, Math.Round(NetPay - MinNetpay, 2));
            #endregion
            dal.ExecuteNonQuery(query, CommandType.Text, paramUpdPayFlg);

            #region Split new deduction
            query = string.Format(@"INSERT INTO {0}
                                           ([Tdd_IDNo]
                                           ,[Tdd_DeductionCode]
                                           ,[Tdd_StartDate]
                                           ,[Tdd_ThisPayCycle]
                                           ,[Tdd_PayCycle]
                                           ,[Tdd_LineNo]
                                           ,[Tdd_PaymentType]
                                           ,[Tdd_Amount]
                                           ,[Tdd_DeferredFlag]
                                           ,[Tdd_PaymentFlag]
                                           ,[Usr_Login]
                                           ,[Ludatetime])
                                     VALUES
                                           (@Tdd_IDNo 
                                           ,@Tdd_DeductionCode 
                                           ,@Tdd_StartDate
                                           ,@Tdd_ThisPayCycle
                                           ,@Tdd_PayCycle
                                           ,'{1:00}'
                                           ,'P'
                                           ,{2}
                                           ,'false'
                                           ,'false'
                                           ,'SA'
                                           ,GetDate())", EmpDeductionDtlTable, GetLastEmployeeDeductionDetailRecord(drDeduction), Math.Round(Convert.ToDecimal(drDeduction["Tdd_Amount"]) - Math.Round(NetPay - MinNetpay, 2), 2));
            #endregion
            dal.ExecuteNonQuery(query, CommandType.Text, paramUpdPayFlg);
            return Math.Round(NetPay - MinNetpay, 2);
        }

        public decimal GetLastEmployeeDeductionDetailRecord(DataRow drDeduction)
        {
            #region parameters
            int paramIndexUpdPayFlg = 0;
            ParameterInfo[] paramUpdPayFlg = new ParameterInfo[7];
            paramUpdPayFlg[paramIndexUpdPayFlg++] = new ParameterInfo("@Tdd_IDNo", drDeduction["Tdd_IDNo"]);
            paramUpdPayFlg[paramIndexUpdPayFlg++] = new ParameterInfo("@Tdd_DeductionCode", drDeduction["Tdd_DeductionCode"]);
            paramUpdPayFlg[paramIndexUpdPayFlg++] = new ParameterInfo("@Tdd_StartDate", drDeduction["Tdd_StartDate"]);
            paramUpdPayFlg[paramIndexUpdPayFlg++] = new ParameterInfo("@Tdd_ThisPayCycle", drDeduction["Tdd_ThisPayCycle"]);
            paramUpdPayFlg[paramIndexUpdPayFlg++] = new ParameterInfo("@Tdd_PayCycle", drDeduction["Tdd_PayCycle"]);
            paramUpdPayFlg[paramIndexUpdPayFlg++] = new ParameterInfo("@Tdd_LineNo", drDeduction["Tdd_LineNo"]);
            paramUpdPayFlg[paramIndexUpdPayFlg++] = new ParameterInfo("@Tdd_Amount", drDeduction["Tdd_Amount"]);
            //paramUpdPayFlg[paramIndexUpdPayFlg++] = new ParameterInfo("@Currency", Currency);
            #endregion

            #region query
            string query = string.Format(@"SELECT Tdd_LineNo
                                           FROM {0}
                                           WHERE Tdd_IDNo = @Tdd_IDNo 
                                              AND Tdd_DeductionCode = @Tdd_DeductionCode 
                                              AND Tdd_StartDate = @Tdd_StartDate
                                              AND Tdd_ThisPayCycle = @Tdd_ThisPayCycle
                                              AND Tdd_PayCycle = @Tdd_PayCycle
                                           ORDER BY Tdd_LineNo DESC", EmpDeductionDtlTable);
            #endregion
            DataTable dtResult;
            decimal SeqNo = 99;
            dtResult = dal.ExecuteDataSet(query, CommandType.Text, paramUpdPayFlg).Tables[0];
            if (dtResult.Rows.Count > 0)
            {
                for (int i = 0; i < dtResult.Rows.Count; i++)
                {
                    if (SeqNo == Convert.ToDecimal(dtResult.Rows[i][0]))
                        SeqNo--;
                }
            }
            return SeqNo;
        }

        public void InsertToDeductionLedgerPayslip(bool ProcessAll, bool ProcessSeparated, string EmployeeId, string PayPeriod)
        {
            string EmployeeDeductionLedgerPayslipTable = "T_EmpDeductionHdrCycle";
            string EmpDeductionDtlTable = "T_EmpDeductionDtl";
            if (ProcessSeparated)
            {
                EmployeeDeductionLedgerPayslipTable = "T_EmpDeductionHdrCycleFinalPay";
                EmpDeductionDtlTable = "T_EmpDeductionDtlFinalPay";
            }

            string EmployeeCondition1 = "";
            string EmployeeCondition2 = "";
            if (!ProcessAll && EmployeeId != "")
            {
                EmployeeCondition1 += " AND Tdh_IDNo = '" + EmployeeId + "' ";
                EmployeeCondition2 += " AND Tdh_IDNo = '" + EmployeeId + "' ";
            }
            else if (ProcessAll && EmployeeList != "")
            {
                EmployeeCondition1 += " AND Tdh_IDNo IN (" + EmployeeList + ") ";
                EmployeeCondition2 += " AND Tdh_IDNo IN (" + EmployeeList + ") ";
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
                                                FROM {6}..T_EmpDeductionHdr 
                                                WHERE Tdh_DeductionAmount <> Tdh_PaidAmount {2}"
                                                , PayPeriod
                                                , EmployeeCondition1
                                                , EmployeeCondition2
                                                , EmployeeDeductionLedgerPayslipTable
                                                , EmpDeductionDtlTable
                                                , CompanyCode
                                                , CentralProfile);
                #endregion
                dal.ExecuteNonQuery(sqlquery);
            }
            catch
            {
                throw new PayrollException("Error in inserting data to deduction payslip.");
            }
        }


        public void InsertToEmployeeLeavePayslip(bool ProcessAll, string EmployeeId)
        {
            string EmployeeCondition1 = "";
            string EmployeeCondition2 = "";
            string EmployeeCondition3 = "";
            if (!ProcessAll && EmployeeId != "")
            {
                EmployeeCondition1 += " AND Tll_IDNo = '" + EmployeeId + "' ";
                EmployeeCondition2 += " AND Tlv_IDNo = '" + EmployeeId + "' ";
                EmployeeCondition3 += " AND Tll_IDNo = '" + EmployeeId + "' ";
            }
            else if (ProcessAll && EmployeeList != "")
            {
                EmployeeCondition1 += " AND Tll_IDNo IN (" + EmployeeList + ") ";
                EmployeeCondition2 += " AND Tlv_IDNo IN (" + EmployeeList + ") ";
                EmployeeCondition3 += " AND Tll_IDNo IN (" + EmployeeList + ") ";
            }

            try
            {
                #region query
                string sqlquery = string.Format(@"DELETE FROM T_EmpLeaveLdgCycle WHERE Tll_PayCycle = '{0}' {1}
                                                  INSERT INTO T_EmpLeaveLdgCycle
                                                      (Tll_PayCycle
													  ,Tll_LeaveYear
                                                      ,Tll_IDNo
                                                      ,Tll_LeaveCode
                                                      ,Tll_BegCredit
													  ,Tll_CarryForwardCredit
													  ,Tll_UsedCredit
													  ,Tll_ConvertedCredit
													  ,Tll_PendingCreditA
													  ,Tll_PendingCreditC
													  ,Tll_UsedCreditThisCycle
                                                      ,Tll_RecordStatus
													  ,Tll_CreatedBy
													  ,Tll_CreatedDate)
                                                  SELECT '{0}'
                                                        ,Tll_LeaveYear
														,Tll_IDNo
														,Tll_LeaveCode
														,Tll_BegCredit
														,Tll_CarryForwardCredit
														,Tll_UsedCredit
														,Tll_ConvertedCredit
														,Tll_PendingCreditA
														,Tll_PendingCreditC
														,0
														,'A'
                                                        ,'{3}'
                                                        ,GETDATE()
                                                  FROM {6}..T_EmpLeaveLdg
                                                  INNER JOIN T_LeaveSchedule
	                                              ON Tll_LeaveCode = Tls_LeaveCode
		                                            AND Tll_LeaveYear = Tls_LeaveYear
                                                    AND Tll_RecordStatus = 'A'
                                                  WHERE Tll_LeaveYear = '{7}'
	                                                {4}

                                                  INSERT INTO T_EmpLeaveLdgCycle
                                                      (Tll_PayCycle
													  ,Tll_LeaveYear
                                                      ,Tll_IDNo
                                                      ,Tll_LeaveCode
                                                      ,Tll_BegCredit
													  ,Tll_CarryForwardCredit
													  ,Tll_UsedCredit
													  ,Tll_ConvertedCredit
													  ,Tll_PendingCreditA
													  ,Tll_PendingCreditC
													  ,Tll_UsedCreditThisCycle
                                                      ,Tll_RecordStatus
													  ,Tll_CreatedBy
													  ,Tll_CreatedDate)
                                                  SELECT '{0}'
                                                      ,Tll_LeaveYear
                                                      ,Tll_IDNo
                                                      ,Tll_LeaveCode
                                                      ,Tll_BegCredit
													  ,Tll_CarryForwardCredit
													  ,Tll_UsedCredit
													  ,Tll_ConvertedCredit
                                                      ,Tll_PendingCreditA
                                                      ,Tll_PendingCreditC
                                                      ,0
													  ,'A'
                                                      ,'{3}'
                                                      ,GETDATE()
                                                   FROM {6}..T_EmpLeaveLdg
                                                    INNER JOIN T_LeaveSchedule
	                                                ON Tll_LeaveCode = Tls_LeaveCode
		                                            AND Tll_LeaveYear = Tls_LeaveYear
                                                    AND Tll_RecordStatus = 'A'
                                                    WHERE Tls_LeaveCode IS NULL
	                                                AND Tll_LeaveYear = '{7}'
                                                    {1}

                                                UPDATE T_EmpLeaveLdgCycle
												SET Tll_UsedCreditThisCycle = Tlv_LeaveHours
												FROM T_EmpLeaveLdgCycle
                                                INNER JOIN (
                                                    SELECT Tlv_PayCycle
													, Tlv_IDNo
													, Tlv_LeaveCode
													, SUM(Tlv_LeaveHours) as Tlv_LeaveHours
												    FROM T_EmpLeave
                                                    WHERE Tlv_PayCycle = '{0}'
	                                                    AND Tlv_LeaveStatus IN ('14', '15')
                                                        {2}
                                                    GROUP BY Tlv_PayCycle, Tlv_IDNo, Tlv_LeaveCode
                                                    HAVING  SUM(Tlv_LeaveHours) <> 0
                                                    ) TEMP 
                                                ON Tll_IDNo = Tlv_IDNo
                                                    AND Tll_PayCycle = Tlv_PayCycle
                                                    AND Tll_LeaveCode = Tlv_LeaveCode
                                                WHERE Tll_PayCycle = '{0}'  {1}"
                                                    , ProcessPayrollPeriod
                                                    , EmployeeCondition1
                                                    , EmployeeCondition2
                                                    , LoginUser
                                                    , EmployeeCondition3
                                                    , CompanyCode
                                                    , CentralProfile
                                                    , ProcessPayrollPeriod.Substring(0,4));
                #endregion
                dal.ExecuteNonQuery(sqlquery);
            }
            catch
            {
                throw new PayrollException("Error in inserting data to leave payslip.");
            }
        }

        public void UpdatePayrollPostFlag(bool ProcessAll, string EmployeeId)
        {
            string EmployeeCondition1 = "";
            string EmployeeCondition2 = "";
            string EmployeeCondition3 = "";
            string EmployeeCondition4 = "";
            if (!ProcessAll && EmployeeId != "")
            {
                EmployeeCondition1 += " AND Tin_IDNo = '" + EmployeeId + "' ";
                EmployeeCondition2 += " AND Tdd_IDNo = '" + EmployeeId + "' ";
                EmployeeCondition3 += " AND Tma_IDNo = '" + EmployeeId + "' ";
                EmployeeCondition4 += " AND Tsa_IDNo = '" + EmployeeId + "' ";
            }
            else if (ProcessAll && EmployeeList != "")
            {
                EmployeeCondition1 += " AND Tin_IDNo IN (" + EmployeeList + ") ";
                EmployeeCondition2 += " AND Tdd_IDNo IN (" + EmployeeList + ") ";
                EmployeeCondition3 += " AND Tma_IDNo IN (" + EmployeeList + ") ";
                EmployeeCondition4 += " AND Tsa_IDNo IN (" + EmployeeList + ") ";
            }

            try
            {
                #region query
                string sqlquery = string.Format(@"  UPDATE T_EmpIncome
                                                    SET Tin_PostFlag = 0
                                                    FROM T_EmpIncome
                                                    LEFT JOIN T_EmpPayroll ON Tin_IDNo = Tpy_IDNo
	                                                    AND Tin_PayCycle = Tpy_PayCycle
                                                    WHERE Tpy_IDNo IS NULL 
                                                        AND Tin_PostFlag = 1
                                                        AND Tin_PayCycle = '{0}' {1}

                                                    UPDATE T_EmpDeductionDtl
                                                    SET Tdd_PaymentFlag = 0
                                                    FROM T_EmpDeductionDtl
                                                    LEFT JOIN T_EmpPayroll ON Tdd_IDNo = Tpy_IDNo
	                                                    AND Tdd_ThisPayCycle = Tpy_PayCycle
                                                    WHERE Tpy_IDNo IS NULL 
                                                        AND Tdd_PaymentFlag = 1
                                                        AND Tdd_PaymentType = 'P'
                                                        AND Tdd_ThisPayCycle = '{0}' {2}

                                                    UPDATE T_EmpManualAdj
                                                    SET Tma_PostFlag = 0
                                                    FROM T_EmpManualAdj
                                                    LEFT JOIN T_EmpPayroll ON Tma_IDNo = Tpy_IDNo
	                                                    AND Tma_PayCycle = Tpy_PayCycle
                                                    WHERE Tpy_IDNo IS NULL 
                                                        AND Tma_PostFlag = 1
                                                        AND Tma_PayCycle = '{0}' {3}

                                                    UPDATE T_EmpSystemAdj
                                                    SET Tsa_PostFlag = 0
                                                    FROM T_EmpSystemAdj
                                                    LEFT JOIN T_EmpPayroll ON Tsa_IDNo = Tpy_IDNo
	                                                    AND Tsa_AdjPayCycle = Tpy_PayCycle
                                                    WHERE Tpy_IDNo IS NULL 
                                                        AND Tsa_PostFlag = 1
                                                        AND Tsa_AdjPayCycle = '{0}' {4}", ProcessPayrollPeriod, EmployeeCondition1, EmployeeCondition2, EmployeeCondition3, EmployeeCondition4);
                #endregion
                dal.ExecuteNonQuery(sqlquery);
            }
            catch
            {
                throw new PayrollException("Error in updating payroll post flag.");
            }
        }

        public DataTable GetOvertimeHoursPreviousMonth(bool ProcessAll, string EmployeeId, string PayPeriod, DALHelper dalhelper)
        {
            string strPreviousPayPeriod = commonBL.GetPrevPayPeriod(PayPeriod, dal);
            string strPrevious2PayPeriod = commonBL.GetPrevPayPeriod(strPreviousPayPeriod, dal);
            string EmployeeCondition = "";
            if (!ProcessAll && EmployeeId != "")
            {
                EmployeeCondition += " and Tph_IDNo = '" + EmployeeId + "' ";
            }

            string query = string.Format(@"SELECT Tph_IDNo, SUM(OTHr) as OTHr
                                            FROM (
                                            SELECT Tph_IDNo
                                                , Tph_REGOTHr+Tph_RESTHr+Tph_RESTOTHr+Tph_LEGHOLHr+Tph_LEGHOLOTHr+Tph_SPLHOLHr+Tph_SPLHOLOTHr+Tph_PSDHr+Tph_PSDOTHr+Tph_COMPHOLHr+Tph_COMPHOLOTHr
                                                +Tph_RESTLEGHOLHr+Tph_RESTLEGHOLOTHr+Tph_RESTSPLHOLHr+Tph_RESTSPLHOLOTHr+Tph_RESTCOMPHOLHr+Tph_RESTCOMPHOLOTHr+Tph_RESTPSDHr+Tph_RESTPSDOTHr as OTHr
                                            FROM T_EmpPayTranHdr
                                            WHERE (Tph_PayCycle = '{0}' or Tph_PayCycle = '{1}') {2}
                                            UNION
                                            SELECT Tph_IDNo
                                                , Tph_REGOTHr+Tph_RESTHr+Tph_RESTOTHr+Tph_LEGHOLHr+Tph_LEGHOLOTHr+Tph_SPLHOLHr+Tph_SPLHOLOTHr+Tph_PSDHr+Tph_PSDOTHr+Tph_COMPHOLHr+Tph_COMPHOLOTHr
                                                +Tph_RESTLEGHOLHr+Tph_RESTLEGHOLOTHr+Tph_RESTSPLHOLHr+Tph_RESTSPLHOLOTHr+Tph_RESTCOMPHOLHr+Tph_RESTCOMPHOLOTHr+Tph_RESTPSDHr+Tph_RESTPSDOTHr as OTHr
                                            FROM T_EmpPayTranHdrHst
                                            WHERE (Tph_PayCycle = '{0}' or Tph_PayCycle = '{1}') {2}
                                            UNION
                                            SELECT Tph_IDNo
                                                , Tph_REGOTHr+Tph_RESTHr+Tph_RESTOTHr+Tph_LEGHOLHr+Tph_LEGHOLOTHr+Tph_SPLHOLHr+Tph_SPLHOLOTHr+Tph_PSDHr+Tph_PSDOTHr+Tph_COMPHOLHr+Tph_COMPHOLOTHr
                                                +Tph_RESTLEGHOLHr+Tph_RESTLEGHOLOTHr+Tph_RESTSPLHOLHr+Tph_RESTSPLHOLOTHr+Tph_RESTCOMPHOLHr+Tph_RESTCOMPHOLOTHr+Tph_RESTPSDHr+Tph_RESTPSDOTHr as OTHr
                                            FROM T_EmpPayTranHdrFinalPay
                                            WHERE (Tph_PayCycle = '{0}' or Tph_PayCycle = '{1}') {2}
                                            ) temp
                                            GROUP BY Tph_IDNo", strPreviousPayPeriod, strPrevious2PayPeriod, EmployeeCondition);
            DataTable dtResult = dalhelper.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public DataSet GetBasicPay(string EmployeeId, string PayPeriod)
        {
            #region query
            string query = string.Format(@"SELECT Tpy_TotalTaxableIncomeAmt - Tpy_TotalAdjAmt - Tpy_TaxableIncomeAmt AS NetBasicPay, Tpy_HourRate
                                            FROM T_EmpPayroll
                                            WHERE Tpy_IDNo = '{0}'
                                                AND Tpy_PayCycle = '{1}'
                                            UNION
                                            SELECT Tpy_TotalTaxableIncomeAmt - Tpy_TotalAdjAmt - Tpy_TaxableIncomeAmt AS NetBasicPay, Tpy_HourRate
                                            FROM T_EmpPayrollYearly
                                            WHERE Tpy_IDNo = '{0}'
                                                AND Tpy_PayCycle = '{1}'
                                            UNION
                                            SELECT Tpy_TotalTaxableIncomeAmt - Tpy_TotalAdjAmt - Tpy_TaxableIncomeAmt AS NetBasicPay, Tpy_HourRate
                                            FROM T_EmpPayrollHst
                                            WHERE Tpy_IDNo = '{0}'
                                                AND Tpy_PayCycle = '{1}'", EmployeeId, PayPeriod);
            #endregion
            DataSet ds = null;
            ds = dal.ExecuteDataSet(query);
            return ds;
        }

        #region Payroll Report functions
        public void AddErrorToPayrollReport(string strEmployeeId, string LastName, string FirstName, string PayCycle, decimal Amount, decimal Amount2, string strRemarks)
        {
            listPayrollRept.Add(new structPayrollErrorReport(strEmployeeId, LastName, FirstName, PayCycle, Amount, Amount2, strRemarks));
        }

        public void InitializePayrollReport()
        {
            listPayrollRept.Clear();
        }

        private DataTable SavePayrollErrorReportList()
        {
            DataTable dtErrList = new DataTable();
            if (dtErrList.Columns.Count == 0)
            {
                dtErrList.Columns.Add("IDNumber");
                dtErrList.Columns.Add("Last Name");
                dtErrList.Columns.Add("First Name");
                dtErrList.Columns.Add("Amount1");
                dtErrList.Columns.Add("Amount2");
                dtErrList.Columns.Add("Remarks");
            }
            for (int i = 0; i < listPayrollRept.Count; i++)
            {
                dtErrList.Rows.Add();
                dtErrList.Rows[dtErrList.Rows.Count - 1]["IDNumber"]    = listPayrollRept[i].strEmployeeId;
                dtErrList.Rows[dtErrList.Rows.Count - 1]["Last Name"]   = listPayrollRept[i].strLastName;
                dtErrList.Rows[dtErrList.Rows.Count - 1]["First Name"]  = listPayrollRept[i].strFirstName;
                dtErrList.Rows[dtErrList.Rows.Count - 1]["Amount1"]    = string.Format("{0:0.00}", listPayrollRept[i].dAmount);
                dtErrList.Rows[dtErrList.Rows.Count - 1]["Amount2"]    = string.Format("{0:0.00}", listPayrollRept[i].dAmount2);
                dtErrList.Rows[dtErrList.Rows.Count - 1]["Remarks"]     = listPayrollRept[i].strRemarks;
            }

            return dtErrList;
        }

        public void InsertToEmpPayrollCheckTable(DataTable dt)
        {
            string qString = "";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                #region Insert to Payroll Error
                qString = string.Format(@"INSERT INTO T_EmpProcessCheck 
                                                   (Tpc_ModuleCode
                                                   ,Tpc_IDNo
                                                   ,Tpc_Type
                                                   ,Tpc_PayCycle
                                                   ,Tpc_NumValue1
                                                   ,Tpc_NumValue2
                                                   ,Tpc_Remarks
                                                   ,Tpc_SystemDefined
                                                   ,Usr_Login
                                                   ,Ludatetime)
                                             VALUES
                                                   ('{0}'
                                                   ,'{1}'
                                                   ,'{2}'
                                                   ,'{3}'
                                                   ,{4}
                                                   ,{5}
                                                   ,'{6}'
                                                   ,1
                                                   ,'{7}'
                                                   ,GETDATE())", MenuCode
                                                               , dt.Rows[i]["IDNumber"].ToString()
                                                               , "AW"
                                                               , ProcessPayrollPeriod
                                                               , dt.Rows[i]["Amount1"].ToString()
                                                               , dt.Rows[i]["Amount2"].ToString()
                                                               , dt.Rows[i]["Remarks"].ToString().Replace("'", "")
                                                               , LoginUser);
                dal.ExecuteNonQuery(qString);
                #endregion

                #region For Adjustment; update post flag in Adjustment and Allowance tables
                //if (Convert.ToBoolean(dt.Rows[i]["Adjustment"]))
                //{
                //    UpdateAllowanceAndAdjustment(dt.Rows[i]["IDNumber"].ToString());
                //}
                #endregion
            }
        }

       
        #endregion

        public void DeleteEmployeePayrollDifferential(string EmployeeID, string CycleType, string MenuCode, DALHelper dalhelper)
        {
            DeleteEmployeePayrollDifferential(false, EmployeeID, "", CycleType, MenuCode, dalhelper);
        }

        public void DeleteEmployeePayrollDifferential(bool ProcessAll, string EmployeeList, string CycleType, string MenuCode, DALHelper dalhelper)
        {
            DeleteEmployeePayrollDifferential(true, "", EmployeeList, CycleType, MenuCode, dalhelper);
        }

        private void DeleteEmployeePayrollDifferential(bool ProcessAll, string EmployeeID, string EmpList, string CycleType, string MenuCode, DALHelper dalhelper)
        {
            string strEmployeeIdQuery = string.Empty;
            string query = @"DELETE FROM T_EmpPayrollTrack
                             WHERE Tpt_Type = '{1}'
                             {0}

                            DELETE FROM T_EmpPayrollTrack
                            FROM T_EmpPayrollTrack
                            INNER JOIN M_Employee ON Tpt_IDNo = Mem_IDNo
	                            AND Mem_WorkStatus = 'IM'
                            WHERE Tpt_Type = '{1}'";
            if (!ProcessAll && EmployeeID != "")
            {
                strEmployeeIdQuery = "AND Tpt_IDNo = '" + EmployeeID + "'";
            }
            else if (ProcessAll && EmpList != "")
            {
                strEmployeeIdQuery = "AND Tpt_IDNo IN (" + EmpList + ")";
            }
            query = string.Format(query, strEmployeeIdQuery, CycleType, MenuCode);
            dalhelper.ExecuteNonQuery(query);
        }

        public decimal GetHoldPayAmount(string EmployeeId)
        {
            string query = string.Format(@"SELECT SUM(Tpy_NetAmt) as Tpy_NetAmt
                                            FROM (
                                            SELECT Tpy_IDNo, Tpy_NetAmt
                                            FROM T_EmpPayroll
                                            WHERE Tpy_PaymentMode = 'H'
                                                AND Tpy_IDNo = '{0}'
                                            UNION
                                            SELECT Tpy_IDNo, Tpy_NetAmt 
                                            FROM T_EmpPayrollYearly
                                            WHERE Tpy_PaymentMode = 'H'
                                                AND Tpy_IDNo = '{0}'
                                            UNION
                                            SELECT Tpy_IDNo, Tpy_NetAmt 
                                            FROM T_EmpPayrollHst
                                            WHERE Tpy_PaymentMode = 'H'
                                                AND Tpy_IDNo = '{0}'
                                            ) TEMP
                                            GROUP BY Tpy_IDNo ", EmployeeId);
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            decimal dNetPayAmt = 0;
            if (dtResult.Rows.Count > 0)
                dNetPayAmt = Convert.ToDecimal(dtResult.Rows[0][0]);
            return dNetPayAmt;
        }

        public void UpdatePayrollStatus(string strEmployeeIDs, bool PayrollStatus, string user,string CentralProfile, DALHelper dalHelper)
        {
            string[] strArrData = strEmployeeIDs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string strData in strArrData)
            {
                string query = string.Format(@"INSERT INTO T_AuditTrl
                                                   (Tat_ColId
                                                   ,Tat_IDNo
                                                   ,Tat_LineNo
                                                   ,Tat_OldValue
                                                   ,Tat_NewValue
                                                   ,Usr_Login
                                                   ,Ludatetime)
                                            SELECT 'PAYROLLSTAT'
	                                            , Mem_IDNo
	                                            , (SELECT REPLICATE(0, 2 - LEN(ISNULL(MAX(CONVERT(INT, Tat_LineNo)) + 1, 1))) + CONVERT(VARCHAR, ISNULL(MAX(CONVERT(INT, Tat_LineNo)) + 1, 1))
			                                            FROM T_AuditTrl 
			                                            WHERE Tat_IDNo = Mem_IDNo
				                                            AND Tat_ColId = 'PAYROLLSTAT') 
	                                            , CASE WHEN Mem_IsComputedPayroll = 1
                                                    THEN 'True'
                                                    ELSE 'False'
                                                    END
	                                            , '{1}'
	                                            , '{2}'
	                                            , GETDATE()
                                            FROM M_Employee
                                            WHERE Mem_IDNo = '{0}'
	                                            AND Mem_IsComputedPayroll != '{1}'

                                            INSERT INTO T_AuditTrl
                                                   (Tat_ColId
                                                   ,Tat_IDNo
                                                   ,Tat_LineNo
                                                   ,Tat_OldValue
                                                   ,Tat_NewValue
                                                   ,Usr_Login
                                                   ,Ludatetime)
                                            SELECT 'PAYROLLSTATCODE'
	                                            , Mem_IDNo
	                                            , (SELECT REPLICATE(0, 2 - LEN(ISNULL(MAX(CONVERT(INT, Tat_LineNo)) + 1, 1))) + CONVERT(VARCHAR, ISNULL(MAX(CONVERT(INT, Tat_LineNo)) + 1, 1))
			                                            FROM T_AuditTrl 
			                                            WHERE Tat_IDNo = Mem_IDNo
				                                            AND Tat_ColId = 'PAYROLLSTATCODE') 
	                                            , Mem_ComputedPayrollCode
	                                            , '{3}'
	                                            , '{2}'
	                                            , GETDATE()
                                            FROM M_Employee
                                            WHERE Mem_IDNo = '{0}'

                                            "
                                            , strData.Split('|')[0].TrimEnd()
                                            , PayrollStatus
                                            , user
                                            , (!PayrollStatus ? strData.Split('|')[1].TrimEnd() : ""));
                dalHelper.ExecuteNonQuery(query);

                query = string.Format(@"UPDATE M_Employee
                                        SET Mem_IsComputedPayroll = '{1}'
                                            , Mem_ComputedPayrollCode = '{2}'
                                            , Mem_UpdatedBy = '{3}'
                                            , Mem_UpdatedDate = GETDATE()
                                        WHERE Mem_IDNo = '{0}'"
                                        , strData.Split('|')[0].TrimEnd()
                                        , PayrollStatus
                                        , (!PayrollStatus ? strData.Split('|')[1].TrimEnd() : "")
                                        , user);
                dalHelper.ExecuteNonQuery(query);
            }
                
        }

        private string GetEmployeeSeparationYear(string EmployeeID)
        {
            string query = string.Format(@"SELECT Mem_SeparationDate
                                            FROM M_Employee
                                            WHERE Mem_IDNo = '{0}'", EmployeeID);

            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            string strSepDate = "";
            if (dtResult.Rows.Count > 0)
            {
                try
                {
                    strSepDate = Convert.ToDateTime(dtResult.Rows[0][0]).Year.ToString();
                }
                catch
                {
                }
            }
            return strSepDate;
        }

        public DataTable GetPayrollTrack(string CostCenterCode, string EmployeeID, string UsrLogin, string cycleType, string profileCode, string companyCode, string centralProfile, DALHelper dalhelper)
        {
            string condition = string.Empty;
            if (!CostCenterCode.Equals(""))
            {
                condition += string.Format(@" AND Mem_CostcenterCode IN ({0})", (new CommonBL()).EncodeFilterItems(CostCenterCode));
            }
            if (!EmployeeID.Equals(""))
            {
                condition += string.Format(@" AND Mem_IDNo IN ({0})", (new CommonBL()).EncodeFilterItems(EmployeeID));
            }
            string query = string.Format(@"SELECT 
                                            Mem_IDNo as [ID Number]
                                            , Mem_LastName + CASE WHEN LEN(ISNULL(Mem_ExtensionName,'')) > 0 
													THEN  ' ' + Mem_ExtensionName ELSE '' END as [Last Name]
                                            , Mem_FirstName as [First Name]
                                            , Mem_MiddleName as [Middle Name] 
                                            , Tpt_ModuleCode as [Module Code]
                                            , Mem_IsComputedPayroll as [Computed Payroll]
                                            FROM M_Employee
                                            @USERCOSTCENTERACCESSCONDITION
                                            INNER JOIN T_EmpPayrollTrack
                                                ON Tpt_IDNo = Mem_IDNo
                                            WHERE 1=1
                                                AND Tpt_Type = '{0}'
                                            @CONDITIONS
                                            GROUP BY Mem_IDNo, Mem_LastName, Mem_FirstName,  Mem_MiddleName, Mem_ExtensionName, Tpt_ModuleCode,Mem_IsComputedPayroll"
                                            , cycleType);
            query = query.Replace("@CONDITIONS", condition);
            query = query.Replace("@USERCOSTCENTERACCESSCONDITION", (new HRCReportsBL()).UserCostCenterAccessTmpQuery(profileCode, "PAYROLL", UsrLogin, "Mem_CostcenterCode", "Mem_PayrollGroup", "Mem_EmploymentStatusCode", "Mem_PayrollType", companyCode, centralProfile, false));
            DataTable dtResult = new DataTable();
            try
            {
                dtResult = dalhelper.ExecuteDataSet(query).Tables[0];
            }
            catch (Exception)
            {
                return null;
            }
            return dtResult;
        }

        public DataTable GetPayrollTrack(string cycleType, string UsrLogin, string profileCode, string companyCode, string centralProfile, DALHelper dalhelper)
        {
            string condition = string.Empty;
            string query = string.Format(@"SELECT 
                                            Mem_IDNo as [ID Number]
                                            , Mem_LastName + CASE WHEN LEN(ISNULL(Mem_ExtensionName,'')) > 0 
													THEN  ' ' + Mem_ExtensionName ELSE '' END as [Last Name]
                                            , Mem_FirstName as [First Name]
                                            , Mem_MiddleName as [Middle Name] 
                                            , Tpt_ModuleCode as [Module Code]
                                            , Mem_IsComputedPayroll as [Computed Payroll]
                                            , Tpt_Type as [Cycle Type]
                                            FROM M_Employee
                                            @USERCOSTCENTERACCESSCONDITION
                                            INNER JOIN T_EmpPayrollTrack ON Tpt_IDNo = Mem_IDNo
                                            INNER JOIN T_PaySchedule ON Tps_CycleType = Tpt_Type
                                            WHERE 1=1
                                                AND Tpt_Type IN ({0})
                                            @CONDITIONS
                                            GROUP BY 
                                                Mem_IDNo
                                                , Mem_LastName
                                                , Mem_FirstName
                                                , Mem_MiddleName
                                                , Mem_ExtensionName
                                                , Tpt_ModuleCode
                                                , Mem_IsComputedPayroll
                                                , Tpt_Type"
                                            , cycleType);

            query = query.Replace("@CONDITIONS", condition);
            query = query.Replace("@USERCOSTCENTERACCESSCONDITION", (new HRCReportsBL()).UserCostCenterAccessTmpQuery(profileCode, "PAYROLL", UsrLogin, "Mem_CostcenterCode", "Mem_PayrollGroup", "Mem_EmploymentStatusCode", "Mem_PayrollType", companyCode, centralProfile, false));
            DataTable dtResult = new DataTable();
            try
            {
                dtResult = dalhelper.ExecuteDataSet(query).Tables[0];
            }
            catch (Exception)
            {
                return null;
            }
            return dtResult;
        }

        public bool CheckHasMandatoryOvertimeCalendarSetup(DateTime dtStartCycle, DateTime dtEndCycle, string companyCode, string centralProfile, DALHelper dalhelper)
        {
            string query = string.Format(@"SELECT * FROM {0}..T_OvertimeGroupTmp
                                            WHERE Tot_CompanyCode = @CompanyCode
                                            AND Tot_Date BETWEEN  @StartDate AND @EndDate
                                            AND Tot_AdvHrs + Tot_MidHrs + Tot_PostHrs > 0"
                                            , centralProfile);

            int idxx = 0;
            ParameterInfo[] paramInfo = new ParameterInfo[3];
            paramInfo[idxx++] = new ParameterInfo("@CompanyCode", companyCode);
            paramInfo[idxx++] = new ParameterInfo("@StartDate", dtStartCycle, SqlDbType.Date);
            paramInfo[idxx++] = new ParameterInfo("@EndDate", dtEndCycle, SqlDbType.Date);

            DataTable dtResult = dalhelper.ExecuteDataSet(query, CommandType.Text, paramInfo).Tables[0];
            if (dtResult.Rows.Count > 0)
                return true;
            else
                return false;
        }

        public void UpdateBankinPolicyDtl(string PayCycleCode, string user, string companyCode, string centralProfile, DALHelper dalhelper)
        {
            string query = string.Format(@"UPDATE M_PolicyDtl
                                                        SET Mpd_ParamValue = 1
                                                            ,Mpd_UpdatedBy = '{1}'
                                                            ,Mpd_UpdatedDate = GETDATE()
                                                        WHERE Mpd_PolicyCode = 'ATMDISKGEN'
                                                        AND Mpd_SubCode IN (SELECT Mbn_BankCode 
									                                        FROM {2}..M_Bank
									                                        WHERE Mbn_CompanyCode = '{3}')
                                                        AND 1 = (
	                                                        SELECT
		                                                        CASE WHEN Tps_CycleIndicator = 'C'
			                                                        THEN 1
			                                                        ELSE 0
			                                                        END
	                                                        FROM T_PaySchedule
	                                                        WHERE Tps_PayCycle = '{0}'
                                                        )", PayCycleCode, user, centralProfile, companyCode);

            dalhelper.ExecuteNonQuery(query);
        }
    }
}
