using System;
using System.Collections.Generic;
using System.Data;
using CommonLibrary;
using Payroll.DAL;
using System.Configuration;

namespace Payroll.BLogic
{
    public class NewSpecialPayrollCalculationBL : BaseBL
    {
        #region <Global Variables>
        DALHelper dal;
        //SystemCycleProcessingBL SystemCycleProcessingBL;
        NewPayrollCalculationBL NewPayrollCalculationBL = new NewPayrollCalculationBL();
        CommonBL commonBL;

        //Flags and parameters
        //I used decimal for more precision during computation
        public decimal MDIVISOR         = 0;
        public decimal MINNETPAY        = 1;
        public string NETBASE           = string.Empty;
        public decimal NETPCT           = 0;
        public decimal HRLYRTEDEC       = 0;

        public string UserLogin         = "";
        public string CompanyCode       = "";
        public string CentralProfile    = "";
        public string MenuCode          = "";

        string ProcessPayrollPeriod     = "";
        string PayrollStart             = "";
        string PayrollEnd               = "";

        //DataTable dtEmployeelist;
        DataTable dtEmployeeListLocal;
        DataTable dtEmployeeListLocal2;
        string EmployeeList = string.Empty;
        DataTable dtErrList = new DataTable();

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

        #region Deduction Variables
        decimal dWTaxAmt            = 0;
        decimal dWTaxRef            = 0;
        decimal dPartialDeduction   = 0;
        decimal dCurrentNetPay      = 0;
        decimal dMinimumNetPay      = 0;
        decimal dOtherDeduction     = 0;
        decimal dTotalDeduction     = 0;
        decimal deductionAmt        = 0;
        decimal dNetPayAmt          = 0;
        #endregion

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

        public NewSpecialPayrollCalculationBL()
        {
        }

        public DataTable ComputeSpecialPayroll(bool ProcessAll, string PayrollPeriod, string IncludedDeductions, string DeductionAmount, string strIncludedIncome, bool bComputeTax, string TaxScheduleCode, string ComputationMethodCode, string usrlogin, string companyCode, string centralProfile, string menucode, DALHelper dal, string employeelist)
        {
            this.EmployeeList = employeelist;
            return ComputeSpecialPayroll(ProcessAll, PayrollPeriod, "", IncludedDeductions, DeductionAmount, strIncludedIncome, bComputeTax, TaxScheduleCode, ComputationMethodCode, usrlogin, companyCode, centralProfile, menucode, dal);
        }
        public DataTable ComputeSpecialPayroll(bool ProcessAll, string PayrollPeriod, string EmployeeId, string IncludedDeductions, string DeductionAmount, string strIncludedIncome, bool bComputeTax, string TaxScheduleCode, string ComputationMethodCode, string usrlogin, string companyCode, string centralProfile, string menucode, DALHelper dal)
        {
            try
            {
                #region Variables
                string PayrollProcessingErrorMessage = "";
                this.dal = dal;
                CompanyCode = companyCode;
                CentralProfile = centralProfile;
                MenuCode = menucode;
                ProcessPayrollPeriod = PayrollPeriod;
                UserLogin = usrlogin;
                bool bHasDayCodeExt = false;
                #endregion

                DataTable dtPayPeriod = GetPayPeriodCycle(PayrollPeriod);
                if (dtPayPeriod.Rows.Count > 0)
                {
                    PayrollStart = dtPayPeriod.Rows[0]["Tps_StartCycle"].ToString();
                    PayrollEnd = dtPayPeriod.Rows[0]["Tps_EndCycle"].ToString();
                }
                else
                    PayrollProcessingErrorMessage += "Pay Cycle not found.\n";

                //Declare Handlers
                NewPayrollCalculationBL = new NewPayrollCalculationBL();
                NewPayrollCalculationBL.EmpDispHandler += new NewPayrollCalculationBL.EmpDispEventHandler(ShowPayrollCalcEmployeeName);
                NewPayrollCalculationBL.StatusHandler += new NewPayrollCalculationBL.StatusEventHandler(ShowPayrollStatus);

                //Check for errors
                if (PayrollProcessingErrorMessage != "")
                    throw new Exception(PayrollProcessingErrorMessage);

                ClearPayrollTables(ProcessAll, EmployeeId, PayrollPeriod);

                if (commonBL.GetFillerDayCodesCount(companyCode, centralProfile, dal) > 0)
                    bHasDayCodeExt = true;

                #region Condition Set-up
                string PayrollCondition = string.Format(@" WHERE Tpy_PayCycle = '{0}'", PayrollPeriod);
                string EmployeeCondition = "";
                if (!ProcessAll && EmployeeId != "")
                {
                    PayrollCondition += " AND Tpy_IDNo = '" + EmployeeId + "' ";
                    EmployeeCondition = " WHERE Mem_IDNo = '" + EmployeeId + "' ";
                }
                else if (ProcessAll && EmployeeList != "")
                {
                    PayrollCondition += " AND Tpy_IDNo IN (" + EmployeeList + ") ";
                    EmployeeCondition = " WHERE Mem_IDNo IN (" + EmployeeList + ") ";
                }

                #endregion

                #region Insert Payroll Record
                string query = string.Format(@"
                                            INSERT INTO T_EmpPayroll(Tpy_IDNo,Tpy_PayCycle,Tpy_SalaryRate,Tpy_HourRate,Tpy_SpecialRate,Tpy_SpecialHourRate,Tpy_LTHr,Tpy_LTAmt,Tpy_UTHr,Tpy_UTAmt,Tpy_UPLVHr,Tpy_UPLVAmt,Tpy_ABSLEGHOLHr,Tpy_ABSLEGHOLAmt,Tpy_ABSSPLHOLHr,Tpy_ABSSPLHOLAmt,Tpy_ABSCOMPHOLHr,Tpy_ABSCOMPHOLAmt,Tpy_ABSPSDHr,Tpy_ABSPSDAmt,Tpy_ABSOTHHOLHr,Tpy_ABSOTHHOLAmt,Tpy_WDABSHr,Tpy_WDABSAmt,Tpy_LTUTMaxHr,Tpy_LTUTMaxAmt,Tpy_ABSHr,Tpy_ABSAmt,Tpy_REGHr,Tpy_REGAmt,Tpy_PDLVHr,Tpy_PDLVAmt,Tpy_PDLEGHOLHr,Tpy_PDLEGHOLAmt,Tpy_PDSPLHOLHr,Tpy_PDSPLHOLAmt,Tpy_PDCOMPHOLHr,Tpy_PDCOMPHOLAmt,Tpy_PDPSDHr,Tpy_PDPSDAmt,Tpy_PDOTHHOLHr,Tpy_PDOTHHOLAmt,Tpy_PDRESTLEGHOLHr,Tpy_PDRESTLEGHOLAmt,Tpy_TotalREGHr,Tpy_TotalREGAmt,Tpy_REGOTHr,Tpy_REGOTAmt,Tpy_REGNDHr,Tpy_REGNDAmt,Tpy_REGNDOTHr,Tpy_REGNDOTAmt,Tpy_RESTHr,Tpy_RESTAmt,Tpy_RESTOTHr,Tpy_RESTOTAmt,Tpy_RESTNDHr,Tpy_RESTNDAmt,Tpy_RESTNDOTHr,Tpy_RESTNDOTAmt,Tpy_LEGHOLHr,Tpy_LEGHOLAmt,Tpy_LEGHOLOTHr,Tpy_LEGHOLOTAmt,Tpy_LEGHOLNDHr,Tpy_LEGHOLNDAmt,Tpy_LEGHOLNDOTHr,Tpy_LEGHOLNDOTAmt,Tpy_SPLHOLHr,Tpy_SPLHOLAmt,Tpy_SPLHOLOTHr,Tpy_SPLHOLOTAmt,Tpy_SPLHOLNDHr,Tpy_SPLHOLNDAmt,Tpy_SPLHOLNDOTHr,Tpy_SPLHOLNDOTAmt,Tpy_PSDHr,Tpy_PSDAmt,Tpy_PSDOTHr,Tpy_PSDOTAmt,Tpy_PSDNDHr,Tpy_PSDNDAmt,Tpy_PSDNDOTHr,Tpy_PSDNDOTAmt,Tpy_COMPHOLHr,Tpy_COMPHOLAmt,Tpy_COMPHOLOTHr,Tpy_COMPHOLOTAmt,Tpy_COMPHOLNDHr,Tpy_COMPHOLNDAmt,Tpy_COMPHOLNDOTHr,Tpy_COMPHOLNDOTAmt,Tpy_RESTLEGHOLHr,Tpy_RESTLEGHOLAmt,Tpy_RESTLEGHOLOTHr,Tpy_RESTLEGHOLOTAmt,Tpy_RESTLEGHOLNDHr,Tpy_RESTLEGHOLNDAmt,Tpy_RESTLEGHOLNDOTHr,Tpy_RESTLEGHOLNDOTAmt,Tpy_RESTSPLHOLHr,Tpy_RESTSPLHOLAmt,Tpy_RESTSPLHOLOTHr,Tpy_RESTSPLHOLOTAmt,Tpy_RESTSPLHOLNDHr,Tpy_RESTSPLHOLNDAmt,Tpy_RESTSPLHOLNDOTHr,Tpy_RESTSPLHOLNDOTAmt,Tpy_RESTCOMPHOLHr,Tpy_RESTCOMPHOLAmt,Tpy_RESTCOMPHOLOTHr,Tpy_RESTCOMPHOLOTAmt,Tpy_RESTCOMPHOLNDHr,Tpy_RESTCOMPHOLNDAmt,Tpy_RESTCOMPHOLNDOTHr,Tpy_RESTCOMPHOLNDOTAmt,Tpy_RESTPSDHr,Tpy_RESTPSDAmt,Tpy_RESTPSDOTHr,Tpy_RESTPSDOTAmt,Tpy_RESTPSDNDHr,Tpy_RESTPSDNDAmt,Tpy_RESTPSDNDOTHr,Tpy_RESTPSDNDOTAmt,Tpy_TotalOTNDAmt,Tpy_WorkDay,Tpy_DayCountOldSalaryRate,Tpy_SRGAdjHr,Tpy_SRGAdjAmt,Tpy_SOTAdjHr,Tpy_SOTAdjAmt,Tpy_SHOLAdjHr,Tpy_SHOLAdjAmt,Tpy_SNDAdjHr,Tpy_SNDAdjAmt,Tpy_SLVAdjHr,Tpy_SLVAdjAmt,Tpy_MRGAdjHr,Tpy_MRGAdjAmt,Tpy_MOTAdjHr,Tpy_MOTAdjAmt,Tpy_MHOLAdjHr,Tpy_MHOLAdjAmt,Tpy_MNDAdjHr,Tpy_MNDAdjAmt,Tpy_TotalAdjAmt,Tpy_TaxableIncomeAmt,Tpy_TotalTaxableIncomeAmt,Tpy_NontaxableIncomeAmt,Tpy_GrossIncomeAmt,Tpy_WtaxAmt,Tpy_TaxBaseAmt,Tpy_TaxRule,Tpy_TaxShare,Tpy_TaxBracket,Tpy_TaxCode,Tpy_IsTaxExempted,Tpy_SSSEE,Tpy_SSSER,Tpy_ECFundAmt,Tpy_SSSBaseAmt,Tpy_SSSRule,Tpy_SSSShare,Tpy_MPFEE,Tpy_MPFER,Tpy_PhilhealthEE,Tpy_PhilhealthER,Tpy_PhilhealthBaseAmt,Tpy_PhilhealthRule,Tpy_PhilhealthShare,Tpy_PagIbigEE,Tpy_PagIbigER,Tpy_PagIbigTaxEE,Tpy_PagIbigBaseAmt,Tpy_PagIbigRule,Tpy_PagIbigShare,Tpy_UnionAmt,Tpy_UnionRule,Tpy_UnionShare,Tpy_UnionBaseAmt,Tpy_OtherDeductionAmt,Tpy_TotalDeductionAmt,Tpy_NetAmt,Tpy_CostcenterCode,Tpy_PositionCode,Tpy_EmploymentStatus,Tpy_PayrollType,Tpy_PaymentMode,Tpy_BankCode,Tpy_BankAcctNo,Tpy_PayrollGroup,Tpy_CalendarGrpCode,Tpy_WorkLocationCode,Tpy_AltAcctCode,Tpy_ExpenseClass,Tpy_PositionGrade,Tpy_WorkStatus,Tpy_PremiumGrpCode,Tpy_IsComputedPerDay,Tpy_IsMultipleSalary,Tpy_RegRule,Usr_Login,Ludatetime)
                                            SELECT Tpy_IDNo             = Mem_IDNo
                                            ,Tpy_PayCycle               = '{1}'
                                            ,Tpy_SalaryRate             = Mem_Salary
                                            ,Tpy_HourRate               = 0
                                            ,Tpy_SpecialRate            = 0
                                            ,Tpy_SpecialHourRate        = 0
                                            ,Tpy_LTHr                   = 0
                                            ,Tpy_LTAmt                  = 0
                                            ,Tpy_UTHr                   = 0
                                            ,Tpy_UTAmt                  = 0
                                            ,Tpy_UPLVHr                 = 0
                                            ,Tpy_UPLVAmt                = 0
                                            ,Tpy_ABSLEGHOLHr            = 0
                                            ,Tpy_ABSLEGHOLAmt           = 0
                                            ,Tpy_ABSSPLHOLHr            = 0
                                            ,Tpy_ABSSPLHOLAmt           = 0
                                            ,Tpy_ABSCOMPHOLHr           = 0
                                            ,Tpy_ABSCOMPHOLAmt          = 0
                                            ,Tpy_ABSPSDHr               = 0
                                            ,Tpy_ABSPSDAmt              = 0
                                            ,Tpy_ABSOTHHOLHr            = 0
                                            ,Tpy_ABSOTHHOLAmt           = 0
                                            ,Tpy_WDABSHr                = 0
                                            ,Tpy_WDABSAmt               = 0
                                            ,Tpy_LTUTMaxHr              = 0
                                            ,Tpy_LTUTMaxAmt             = 0
                                            ,Tpy_ABSHr                  = 0
                                            ,Tpy_ABSAmt                 = 0
                                            ,Tpy_REGHr                  = 0
                                            ,Tpy_REGAmt                 = 0
                                            ,Tpy_PDLVHr                 = 0
                                            ,Tpy_PDLVAmt                = 0
                                            ,Tpy_PDLEGHOLHr             = 0
                                            ,Tpy_PDLEGHOLAmt            = 0
                                            ,Tpy_PDSPLHOLHr             = 0
                                            ,Tpy_PDSPLHOLAmt            = 0
                                            ,Tpy_PDCOMPHOLHr            = 0
                                            ,Tpy_PDCOMPHOLAmt           = 0
                                            ,Tpy_PDPSDHr                = 0
                                            ,Tpy_PDPSDAmt               = 0
                                            ,Tpy_PDOTHHOLHr             = 0
                                            ,Tpy_PDOTHHOLAmt            = 0
                                            ,Tpy_PDRESTLEGHOLHr         = 0
                                            ,Tpy_PDRESTLEGHOLAmt        = 0
                                            ,Tpy_TotalREGHr             = 0
                                            ,Tpy_TotalREGAmt            = 0
                                            ,Tpy_REGOTHr                = 0
                                            ,Tpy_REGOTAmt               = 0
                                            ,Tpy_REGNDHr                = 0
                                            ,Tpy_REGNDAmt               = 0
                                            ,Tpy_REGNDOTHr              = 0
                                            ,Tpy_REGNDOTAmt             = 0
                                            ,Tpy_RESTHr                 = 0
                                            ,Tpy_RESTAmt                = 0
                                            ,Tpy_RESTOTHr               = 0
                                            ,Tpy_RESTOTAmt              = 0
                                            ,Tpy_RESTNDHr               = 0
                                            ,Tpy_RESTNDAmt              = 0
                                            ,Tpy_RESTNDOTHr             = 0
                                            ,Tpy_RESTNDOTAmt            = 0
                                            ,Tpy_LEGHOLHr               = 0
                                            ,Tpy_LEGHOLAmt              = 0
                                            ,Tpy_LEGHOLOTHr             = 0
                                            ,Tpy_LEGHOLOTAmt            = 0
                                            ,Tpy_LEGHOLNDHr             = 0
                                            ,Tpy_LEGHOLNDAmt            = 0
                                            ,Tpy_LEGHOLNDOTHr           = 0
                                            ,Tpy_LEGHOLNDOTAmt          = 0
                                            ,Tpy_SPLHOLHr               = 0
                                            ,Tpy_SPLHOLAmt              = 0
                                            ,Tpy_SPLHOLOTHr             = 0
                                            ,Tpy_SPLHOLOTAmt            = 0
                                            ,Tpy_SPLHOLNDHr             = 0
                                            ,Tpy_SPLHOLNDAmt            = 0
                                            ,Tpy_SPLHOLNDOTHr           = 0
                                            ,Tpy_SPLHOLNDOTAmt          = 0
                                            ,Tpy_PSDHr                  = 0
                                            ,Tpy_PSDAmt                 = 0
                                            ,Tpy_PSDOTHr                = 0
                                            ,Tpy_PSDOTAmt               = 0
                                            ,Tpy_PSDNDHr                = 0
                                            ,Tpy_PSDNDAmt               = 0
                                            ,Tpy_PSDNDOTHr              = 0
                                            ,Tpy_PSDNDOTAmt             = 0
                                            ,Tpy_COMPHOLHr              = 0
                                            ,Tpy_COMPHOLAmt             = 0
                                            ,Tpy_COMPHOLOTHr            = 0
                                            ,Tpy_COMPHOLOTAmt           = 0
                                            ,Tpy_COMPHOLNDHr            = 0
                                            ,Tpy_COMPHOLNDAmt           = 0
                                            ,Tpy_COMPHOLNDOTHr          = 0
                                            ,Tpy_COMPHOLNDOTAmt         = 0
                                            ,Tpy_RESTLEGHOLHr           = 0
                                            ,Tpy_RESTLEGHOLAmt          = 0
                                            ,Tpy_RESTLEGHOLOTHr         = 0
                                            ,Tpy_RESTLEGHOLOTAmt        = 0
                                            ,Tpy_RESTLEGHOLNDHr         = 0
                                            ,Tpy_RESTLEGHOLNDAmt        = 0
                                            ,Tpy_RESTLEGHOLNDOTHr       = 0
                                            ,Tpy_RESTLEGHOLNDOTAmt      = 0
                                            ,Tpy_RESTSPLHOLHr           = 0
                                            ,Tpy_RESTSPLHOLAmt          = 0
                                            ,Tpy_RESTSPLHOLOTHr         = 0
                                            ,Tpy_RESTSPLHOLOTAmt        = 0
                                            ,Tpy_RESTSPLHOLNDHr         = 0
                                            ,Tpy_RESTSPLHOLNDAmt        = 0
                                            ,Tpy_RESTSPLHOLNDOTHr       = 0
                                            ,Tpy_RESTSPLHOLNDOTAmt      = 0
                                            ,Tpy_RESTCOMPHOLHr          = 0
                                            ,Tpy_RESTCOMPHOLAmt         = 0
                                            ,Tpy_RESTCOMPHOLOTHr        = 0
                                            ,Tpy_RESTCOMPHOLOTAmt       = 0
                                            ,Tpy_RESTCOMPHOLNDHr        = 0
                                            ,Tpy_RESTCOMPHOLNDAmt       = 0
                                            ,Tpy_RESTCOMPHOLNDOTHr      = 0
                                            ,Tpy_RESTCOMPHOLNDOTAmt     = 0
                                            ,Tpy_RESTPSDHr              = 0
                                            ,Tpy_RESTPSDAmt             = 0
                                            ,Tpy_RESTPSDOTHr            = 0
                                            ,Tpy_RESTPSDOTAmt           = 0
                                            ,Tpy_RESTPSDNDHr            = 0
                                            ,Tpy_RESTPSDNDAmt           = 0
                                            ,Tpy_RESTPSDNDOTHr          = 0
                                            ,Tpy_RESTPSDNDOTAmt         = 0
                                            ,Tpy_TotalOTNDAmt           = 0
                                            ,Tpy_WorkDay                = 0
                                            ,Tpy_DayCountOldSalaryRate  = 0
                                            ,Tpy_SRGAdjHr               = 0
                                            ,Tpy_SRGAdjAmt              = 0
                                            ,Tpy_SOTAdjHr               = 0
                                            ,Tpy_SOTAdjAmt              = 0
                                            ,Tpy_SHOLAdjHr              = 0
                                            ,Tpy_SHOLAdjAmt             = 0
                                            ,Tpy_SNDAdjHr               = 0
                                            ,Tpy_SNDAdjAmt              = 0
                                            ,Tpy_SLVAdjHr               = 0
                                            ,Tpy_SLVAdjAmt              = 0
                                            ,Tpy_MRGAdjHr               = 0
                                            ,Tpy_MRGAdjAmt              = 0
                                            ,Tpy_MOTAdjHr               = 0
                                            ,Tpy_MOTAdjAmt              = 0
                                            ,Tpy_MHOLAdjHr              = 0
                                            ,Tpy_MHOLAdjAmt             = 0
                                            ,Tpy_MNDAdjHr               = 0
                                            ,Tpy_MNDAdjAmt              = 0
                                            ,Tpy_TotalAdjAmt            = 0
                                            ,Tpy_TaxableIncomeAmt       = 0
                                            ,Tpy_TotalTaxableIncomeAmt  = 0
                                            ,Tpy_NontaxableIncomeAmt    = 0
                                            ,Tpy_GrossIncomeAmt         = 0
                                            ,Tpy_WtaxAmt                = 0
                                            ,Tpy_TaxBaseAmt             = 0
                                            ,Tpy_TaxRule                = Mem_TaxRule
                                            ,Tpy_TaxShare               = Mem_TaxShare
                                            ,Tpy_TaxBracket             = 0
                                            ,Tpy_TaxCode                = Mem_TaxCode
                                            ,Tpy_IsTaxExempted          = Mem_IsTaxExempted
                                            ,Tpy_SSSEE                  = 0
                                            ,Tpy_SSSER                  = 0
                                            ,Tpy_ECFundAmt              = 0
                                            ,Tpy_SSSBaseAmt             = 0
                                            ,Tpy_SSSRule                = Mem_SSSRule
                                            ,Tpy_SSSShare               = Mem_SSSShare
                                            ,Tpy_MPFEE                  = 0
                                            ,Tpy_MPFER                  = 0
                                            ,Tpy_PhilhealthEE           = 0
                                            ,Tpy_PhilhealthER           = 0
                                            ,Tpy_PhilhealthBaseAmt      = 0
                                            ,Tpy_PhilhealthRule         = Mem_PHRule
                                            ,Tpy_PhilhealthShare        = Mem_PHShare
                                            ,Tpy_PagIbigEE              = 0
                                            ,Tpy_PagIbigER              = 0
                                            ,Tpy_PagIbigTaxEE           = 0
                                            ,Tpy_PagIbigBaseAmt         = 0
                                            ,Tpy_PagIbigRule            = Mem_PagIbigRule
                                            ,Tpy_PagIbigShare           = Mem_PagIbigShare
                                            ,Tpy_UnionAmt               = 0
                                            ,Tpy_UnionRule              = Mem_UnionRule
                                            ,Tpy_UnionShare             = Mem_UnionShare
                                            ,Tpy_UnionBaseAmt           = 0
                                            ,Tpy_OtherDeductionAmt      = 0
                                            ,Tpy_TotalDeductionAmt      = 0
                                            ,Tpy_NetAmt                 = 0
                                            ,Tpy_CostcenterCode         = Mem_CostcenterCode
                                            ,Tpy_PositionCode           = Mem_PositionCode
                                            ,Tpy_EmploymentStatus       = Mem_EmploymentStatusCode
                                            ,Tpy_PayrollType            = Mem_PayrollType
                                            ,Tpy_PaymentMode            = Mem_PaymentMode
                                            ,Tpy_BankCode               = Mem_PayrollBankCode
                                            ,Tpy_BankAcctNo             = Mem_BankAcctNo
                                            ,Tpy_PayrollGroup           = Mem_PayrollGroup
                                            ,Tpy_CalendarGrpCode        = Mem_CalendarGrpCode
                                            ,Tpy_WorkLocationCode       = Mem_WorkLocationCode
                                            ,Tpy_AltAcctCode            = Mem_AltAcctCode
                                            ,Tpy_ExpenseClass           = Mem_ExpenseClass
                                            ,Tpy_PositionGrade          = Mem_PositionGrade
                                            ,Tpy_WorkStatus             = Mem_WorkStatus
                                            ,Tpy_PremiumGrpCode         = Mem_PremiumGrpCode
                                            ,Tpy_IsComputedPerDay       = Mem_IsComputedPerDay
                                            ,Tpy_IsMultipleSalary       = Mem_IsMultipleSalary
                                            ,Tpy_RegRule                = ''
                                            ,Usr_Login                  = '{2}'
                                            ,Ludatetime                 = GETDATE()
                                            FROM M_Employee
                                            {0}

                                    
                                            INSERT INTO T_EmpPayroll2 (Tpy_IDNo,Tpy_PayCycle,Tpy_RemainingPayCycle,Tpy_YTDRegularAmtBefore,Tpy_AssumeRegularAmt,Tpy_RegularTotal,Tpy_YTDRecurringAllowanceAmtBefore,Tpy_YTDRecurringAllowanceAmt,Tpy_RecurringAllowanceAmt,Tpy_AssumeRecurringAllowanceAmt,Tpy_RecurringAllowanceTotal,Tpy_YTDBonusAmtBefore,Tpy_BonusNontaxAmt,Tpy_BonusTaxAmt,Tpy_Assume13thMonthAmt,Tpy_BonusTotal,Tpy_BonusTaxRevaluated,Tpy_YTDBonusTaxBefore,Tpy_YTDOtherTaxableIncomeAmtBefore,Tpy_OtherTaxableIncomeAmt,Tpy_OtherTaxableIncomeTotal,Tpy_YTDSSSAmtBefore,Tpy_AssumeSSSAmt,Tpy_SSSTotal,Tpy_YTDPhilhealthAmtBefore,Tpy_AssumePhilhealthAmt,Tpy_PhilhealthTotal,Tpy_YTDPagIbigAmtBefore,Tpy_AssumePagIbigAmt,Tpy_PagIbigTotal,Tpy_YTDPagIbigTaxAmtBefore,Tpy_AssumePagIbigTaxAmt,Tpy_TotalTaxableIncomeWAssumeAmt,Tpy_YTDUnionAmtBefore,Tpy_AssumeUnionAmt,Tpy_UnionTotal,Tpy_PremiumPaidOnHealth,Tpy_TotalExemptions,Tpy_NetTaxableIncomeAmt,Tpy_YTDREGAmt,Tpy_YTDTotalTaxableIncomeAmtBefore,Tpy_YTDTotalTaxableIncomeAmt,Tpy_YTDWtaxAmtBefore,Tpy_YTDWtaxAmt,Tpy_YTDSSSAmt,Tpy_YTDPhilhealthAmt,Tpy_YTDPagIbigAmt,Tpy_YTDPagIbigTaxAmt,Tpy_YTDUnionAmt,Tpy_YTDNontaxableAmtBefore,Tpy_YTDNontaxableAmt,Tpy_BIRTotalAmountofCompensation,Tpy_BIRStatutoryMinimumWage,Tpy_BIRHolidayOvertimeNightShiftHazard,Tpy_BIR13thMonthPayOtherBenefits,Tpy_BIRDeMinimisBenefits,Tpy_BIRSSSGSISPHICHDMFUnionDues,Tpy_BIROtherNonTaxableCompensation,Tpy_BIRTotalTaxableCompensation,Tpy_BIRTaxableCompensationNotSubjectToWithholdingTax,Tpy_BIRTotalTaxesWithheld,Tpy_TaxDue,Tpy_AssumeMonthlyRegularAmt,Tpy_AssumeMonthlySSSAmt,Tpy_AssumeMonthlyPhilhealthAmt,Tpy_AssumeMonthlyPagibigAmt,Tpy_AssumeMonthlyUnionAmt,Tpy_YTDMPFAmtBefore,Tpy_YTDMPFAmt,Usr_Login,Ludatetime)
                                            SELECT Tpy_IDNo                     = Mem_IDNo
                                            ,Tpy_PayCycle                       = '{1}'
                                            ,Tpy_RemainingPayCycle              =0
                                            ,Tpy_YTDRegularAmtBefore            =0
                                            ,Tpy_AssumeRegularAmt               =0
                                            ,Tpy_RegularTotal                   =0
                                            ,Tpy_YTDRecurringAllowanceAmtBefore =0
                                            ,Tpy_YTDRecurringAllowanceAmt       =0
                                            ,Tpy_RecurringAllowanceAmt          =0
                                            ,Tpy_AssumeRecurringAllowanceAmt    =0
                                            ,Tpy_RecurringAllowanceTotal        =0
                                            ,Tpy_YTDBonusAmtBefore              =0
                                            ,Tpy_BonusNontaxAmt                 =0
                                            ,Tpy_BonusTaxAmt                    =0
                                            ,Tpy_Assume13thMonthAmt             =0
                                            ,Tpy_BonusTotal                     =0
                                            ,Tpy_BonusTaxRevaluated             =0
                                            ,Tpy_YTDBonusTaxBefore              =0
                                            ,Tpy_YTDOtherTaxableIncomeAmtBefore =0
                                            ,Tpy_OtherTaxableIncomeAmt          =0
                                            ,Tpy_OtherTaxableIncomeTotal        =0
                                            ,Tpy_YTDSSSAmtBefore                =0
                                            ,Tpy_AssumeSSSAmt                   =0
                                            ,Tpy_SSSTotal                       =0
                                            ,Tpy_YTDPhilhealthAmtBefore         =0
                                            ,Tpy_AssumePhilhealthAmt            =0
                                            ,Tpy_PhilhealthTotal                =0
                                            ,Tpy_YTDPagIbigAmtBefore            =0
                                            ,Tpy_AssumePagIbigAmt               =0
                                            ,Tpy_PagIbigTotal                   =0
                                            ,Tpy_YTDPagIbigTaxAmtBefore         =0
                                            ,Tpy_AssumePagIbigTaxAmt            =0
                                            ,Tpy_TotalTaxableIncomeWAssumeAmt   =0
                                            ,Tpy_YTDUnionAmtBefore              =0
                                            ,Tpy_AssumeUnionAmt                 =0
                                            ,Tpy_UnionTotal                     =0
                                            ,Tpy_PremiumPaidOnHealth            =0
                                            ,Tpy_TotalExemptions                =0
                                            ,Tpy_NetTaxableIncomeAmt            =0
                                            ,Tpy_YTDREGAmt                      =0
                                            ,Tpy_YTDTotalTaxableIncomeAmtBefore =0
                                            ,Tpy_YTDTotalTaxableIncomeAmt       =0
                                            ,Tpy_YTDWtaxAmtBefore               =0
                                            ,Tpy_YTDWtaxAmt                     =0
                                            ,Tpy_YTDSSSAmt                      =0
                                            ,Tpy_YTDPhilhealthAmt               =0
                                            ,Tpy_YTDPagIbigAmt                  =0
                                            ,Tpy_YTDPagIbigTaxAmt               =0
                                            ,Tpy_YTDUnionAmt                    =0
                                            ,Tpy_YTDNontaxableAmtBefore         =0
                                            ,Tpy_YTDNontaxableAmt               =0
                                            ,Tpy_BIRTotalAmountofCompensation   =0
                                            ,Tpy_BIRStatutoryMinimumWage        =0
                                            ,Tpy_BIRHolidayOvertimeNightShiftHazard=0
                                            ,Tpy_BIR13thMonthPayOtherBenefits   =0
                                            ,Tpy_BIRDeMinimisBenefits           =0
                                            ,Tpy_BIRSSSGSISPHICHDMFUnionDues    =0
                                            ,Tpy_BIROtherNonTaxableCompensation =0
                                            ,Tpy_BIRTotalTaxableCompensation    =0
                                            ,Tpy_BIRTaxableCompensationNotSubjectToWithholdingTax=0
                                            ,Tpy_BIRTotalTaxesWithheld          =0
                                            ,Tpy_TaxDue                         =0
                                            ,Tpy_AssumeMonthlyRegularAmt        =0
                                            ,Tpy_AssumeMonthlySSSAmt            =0
                                            ,Tpy_AssumeMonthlyPhilhealthAmt     =0
                                            ,Tpy_AssumeMonthlyPagibigAmt        =0
                                            ,Tpy_AssumeMonthlyUnionAmt          =0
                                            ,Tpy_YTDMPFAmtBefore                =0
                                            ,Tpy_YTDMPFAmt                      =0
                                            ,Usr_Login                          = '{2}'
                                            ,Ludatetime                         = GETDATE()
                                            FROM M_Employee
                                            {0}

                                            ", EmployeeCondition
                                             , PayrollPeriod
                                             , UserLogin);
                dal.ExecuteNonQuery(query);
                #endregion

                #region Insert Payroll Calc Ext
                if (bHasDayCodeExt)
                {
                    query = string.Format(@"
                                        INSERT INTO T_EmpPayrollMisc
                                           (Tpm_IDNo,Tpm_PayCycle,Tpm_Misc1Hr,Tpm_Misc1Amt,Tpm_Misc1OTHr,Tpm_Misc1OTAmt,Tpm_Misc1NDHr,Tpm_Misc1NDAmt,Tpm_Misc1NDOTHr,Tpm_Misc1NDOTAmt,Tpm_Misc2Hr,Tpm_Misc2Amt,Tpm_Misc2OTHr,Tpm_Misc2OTAmt,Tpm_Misc2NDHr,Tpm_Misc2NDAmt,Tpm_Misc2NDOTHr,Tpm_Misc2NDOTAmt,Tpm_Misc3Hr,Tpm_Misc3Amt,Tpm_Misc3OTHr,Tpm_Misc3OTAmt,Tpm_Misc3NDHr,Tpm_Misc3NDAmt,Tpm_Misc3NDOTHr,Tpm_Misc3NDOTAmt,Tpm_Misc4Hr,Tpm_Misc4Amt,Tpm_Misc4OTHr,Tpm_Misc4OTAmt,Tpm_Misc4NDHr,Tpm_Misc4NDAmt,Tpm_Misc4NDOTHr,Tpm_Misc4NDOTAmt,Tpm_Misc5Hr,Tpm_Misc5Amt,Tpm_Misc5OTHr,Tpm_Misc5OTAmt,Tpm_Misc5NDHr,Tpm_Misc5NDAmt,Tpm_Misc5NDOTHr,Tpm_Misc5NDOTAmt,Tpm_Misc6Hr,Tpm_Misc6Amt,Tpm_Misc6OTHr,Tpm_Misc6OTAmt,Tpm_Misc6NDHr,Tpm_Misc6NDAmt,Tpm_Misc6NDOTHr,Tpm_Misc6NDOTAmt,Usr_Login,Ludatetime)
                                        SELECT Mem_IDNo,'{1}',0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,'{2}',GETDATE()
                                        FROM M_Employee
                                        {0}"
                                            , EmployeeCondition
                                            , PayrollPeriod
                                            , UserLogin);
                    dal.ExecuteNonQuery(query);
                }
                #endregion

                #region Post Employee Allowances
                query = string.Format(@"UPDATE T_EmpIncome
                                    SET Tin_PostFlag = 0
                                    {1}

                                    UPDATE T_EmpPayroll
                                    SET  Tpy_TaxableIncomeAmt = TaxAllowanceAmt
                                        ,Tpy_NontaxableIncomeAmt = NonTaxAllowanceAmt
                                        ,Tpy_TotalTaxableIncomeAmt = TaxAllowanceAmt
                                        ,Tpy_GrossIncomeAmt = TaxAllowanceAmt + NonTaxAllowanceAmt
                                        ,Usr_Login = '{2}'
                                        ,Ludatetime = GETDATE()
                                    FROM T_EmpPayroll 
                                    INNER JOIN (SELECT Tin_IDNo
                                                        ,TaxAllowanceAmt = SUM(CASE WHEN Min_TaxClass = 'T' THEN Tin_IncomeAmt ELSE 0 END)
                                                        ,NonTaxAllowanceAmt = SUM(CASE WHEN Min_TaxClass = 'N' THEN Tin_IncomeAmt ELSE 0 END)
                                                    FROM T_EmpIncome 
                                                    INNER JOIN {5}..M_Income
                                                        ON Min_IncomeCode = Tin_IncomeCode
                                                        AND Min_CompanyCode = '{4}'
                                                    {1}
                                                        {3}
                                                    GROUP BY Tin_IDNo) Allowance 
                                    ON Tpy_IDNo = Tin_IDNo
                                    {0}

                                    UPDATE T_EmpIncome
                                    SET Tin_PostFlag = 1
                                    FROM T_EmpIncome 
                                    INNER JOIN {5}..M_Income
	                                    ON Min_IncomeCode = Tin_IncomeCode 
                                        AND Min_CompanyCode = '{4}'
                                    INNER JOIN T_EmpPayroll
                                        ON Tin_IDNo = Tpy_IDNo
                                        AND Tin_PayCycle = Tpy_PayCycle
                                   {1}
                                        {3}", PayrollCondition
                                            , PayrollCondition.Replace("Tpy_", "Tin_")
                                            , UserLogin
                                            , strIncludedIncome
                                            , companyCode
                                            , CentralProfile);
                dal.ExecuteNonQuery(query);
                #endregion

                #region Get Payroll Calc Record
                dtEmployeeListLocal = GetPayrollCalcRecord(ProcessAll, "T_EmpPayroll", EmployeeId, PayrollPeriod);
                dtEmployeeListLocal2 = GetPayrollCalcRecord(ProcessAll, "T_EmpPayroll2", EmployeeId, PayrollPeriod);
                #endregion
                    
                #region Compute Deductions and Net Pay
                StatusHandler(this, new StatusEventArgs(string.Format("Compute Tax, Deductions and Net Pay"), false));
                #region Get Record Details and Compute Amounts
                for (int i = 0; i < dtEmployeeListLocal.Rows.Count; i++) //only one record
                {
                    #region Compute Tax
                    if (bComputeTax)
                    {
                        //ComputeTax(EmployeeId, PayrollPeriod, TaxScheduleCode, ComputationMethodCode);
                        StatusHandler(this, new StatusEventArgs(string.Format("Compute Tax"), false));

                        NewPayrollCalculationBL.ComputeWithholdingTax(false
                                                                        , false
                                                                        , TaxScheduleCode
                                                                        , ComputationMethodCode
                                                                        , true
                                                                        , dtEmployeeListLocal.Select("Tpy_IDNo = '" + dtEmployeeListLocal.Rows[i]["Tpy_IDNo"].ToString() + "'")[0]
                                                                        , dtEmployeeListLocal2.Select("Tpy_IDNo = '" + dtEmployeeListLocal.Rows[i]["Tpy_IDNo"].ToString() + "'")[0]
                                                                        , !Convert.ToBoolean(dtEmployeeListLocal.Rows[i]["Tpy_IsTaxExempted"])
                                                                        , CompanyCode
                                                                        , CentralProfile
                                                                        , ""
                                                                        , dal);

                        #region Save Computed Tax Records
                        query = string.Format(@"UPDATE T_EmpPayroll 
                                                SET Tpy_WtaxAmt = {2}
                                                    , Tpy_TaxBracket = '{3}'
                                                WHERE Tpy_IDNo = '{0}'
                                                AND Tpy_PayCycle = '{1}'"
                                                                , dtEmployeeListLocal.Rows[i]["Tpy_IDNo"]
                                                                , dtEmployeeListLocal.Rows[i]["Tpy_PayCycle"]
                                                                , dtEmployeeListLocal.Rows[i]["Tpy_WtaxAmt"]
                                                                , dtEmployeeListLocal.Rows[i]["Tpy_TaxBracket"]);
                        dal.ExecuteNonQuery(query);
                        #endregion
                    }
                    #endregion

                    #region Compute Hourly Rate
                    MDIVISOR = Convert.ToDecimal(GetScalarValue("Mpd_ParamValue", "M_PolicyDtl", string.Format("Mpd_PolicyCode = 'MDIVISOR' AND Mpd_SubCode = '{0}' AND Mpd_CompanyCode = '{1}' AND Mpd_RecordStatus = 'A'", dtEmployeeListLocal.Rows[i]["Tpy_EmploymentStatus"], companyCode), dal));
                    dtEmployeeListLocal.Rows[i]["Tpy_HourRate"] = GetHourlyRate(Convert.ToDecimal(dtEmployeeListLocal.Rows[i]["Tpy_SalaryRate"]), dtEmployeeListLocal.Rows[i]["Tpy_PayrollType"].ToString(), MDIVISOR);
                    #endregion

                    #region Compute Deduction and Net Pay
                    dWTaxAmt = 0;
                    dWTaxRef            = 0;
                    dPartialDeduction   = 0;
                    dCurrentNetPay      = 0;
                    dMinimumNetPay      = 0;
                    dOtherDeduction     = 0;
                    dTotalDeduction     = 0;
                    deductionAmt        = 0;
                    dNetPayAmt          = 0;

                    //Only deduct tax if the amount is positive
                    if (Convert.ToDecimal(dtEmployeeListLocal.Rows[i]["Tpy_WtaxAmt"].ToString()) >= 0)
                        dWTaxAmt = Convert.ToDecimal(dtEmployeeListLocal.Rows[i]["Tpy_WtaxAmt"].ToString());
                    else
                    {
                        dWTaxRef = Convert.ToDecimal(dtEmployeeListLocal.Rows[i]["Tpy_WtaxAmt"].ToString());
                        dWTaxRef = dWTaxRef * -1;
                        dWTaxAmt = 0;
                    }

                    dOtherDeduction = 0;
                    dPartialDeduction = dWTaxAmt +
                                          Convert.ToDecimal(dtEmployeeListLocal.Rows[i]["Tpy_OtherDeductionAmt"].ToString());

                    dCurrentNetPay = Convert.ToDecimal(dtEmployeeListLocal.Rows[i]["Tpy_GrossIncomeAmt"].ToString()) -
                                       dPartialDeduction +
                                       dWTaxRef;

                    //Get gross pay percentage
                    if (NETBASE == "T")
                        dMinimumNetPay = Convert.ToDecimal(dtEmployeeListLocal.Rows[i]["Tpy_TotalTaxableIncomeAmt"].ToString()) * (NETPCT / 100);
                    else if (NETBASE == "G")
                        dMinimumNetPay = (Convert.ToDecimal(dtEmployeeListLocal.Rows[i]["Tpy_GrossIncomeAmt"].ToString()))
                                                    * (NETPCT / 100);

                    //Compare with minimum take home pay parameter
                    if (MINNETPAY > 0 && dMinimumNetPay < MINNETPAY)
                        dMinimumNetPay = MINNETPAY;

                    CreateCurrentDeduction(ProcessAll, PayrollPeriod, EmployeeId, IncludedDeductions);
                    DataSet dsDeductions = GetDeductionDetails(dtEmployeeListLocal.Rows[i]["Tpy_IDNo"].ToString(), PayrollPeriod);
                    deductionAmt = 0;
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
                    dtEmployeeListLocal.Rows[i]["Tpy_OtherDeductionAmt"] = dOtherDeduction;

                    dTotalDeduction = dWTaxAmt + dOtherDeduction;
                    //Update total deduction amount
                    dTotalDeduction = Math.Round(dTotalDeduction, 2);
                    dtEmployeeListLocal.Rows[i]["Tpy_TotalDeductionAmt"] = dTotalDeduction;

                    dNetPayAmt = Convert.ToDecimal(dtEmployeeListLocal.Rows[i]["Tpy_GrossIncomeAmt"].ToString()) +
                                  dWTaxRef -
                                  dTotalDeduction;
                    //Update total deduction amount
                    dNetPayAmt = Math.Round(dNetPayAmt, 2);
                    dtEmployeeListLocal.Rows[i]["Tpy_NetAmt"] = dNetPayAmt;
                    #endregion

                    #region BIR
                    ComputeIncomeAndDeductionAdjustments(dtEmployeeListLocal.Select("Tpy_IDNo = '" + dtEmployeeListLocal.Rows[i]["Tpy_IDNo"].ToString() + "'")[0], dtEmployeeListLocal2.Select("Tpy_IDNo = '" + dtEmployeeListLocal.Rows[i]["Tpy_IDNo"].ToString() + "'")[0]);
                    #endregion

                    #region Save records
                    query = string.Format(@"UPDATE T_EmpPayroll 
                                                    SET Tpy_OtherDeductionAmt = {2}
                                                       ,Tpy_TotalDeductionAmt = {3}
                                                       ,Tpy_NetAmt = {4}
                                                       ,Tpy_HourRate = {5}
                                                    WHERE Tpy_IDNo = '{0}'
                                                    AND Tpy_PayCycle = '{1}'"
                                                            , dtEmployeeListLocal.Rows[i]["Tpy_IDNo"]
                                                            , dtEmployeeListLocal.Rows[i]["Tpy_PayCycle"]
                                                            , dtEmployeeListLocal.Rows[i]["Tpy_OtherDeductionAmt"]
                                                            , dtEmployeeListLocal.Rows[i]["Tpy_TotalDeductionAmt"]
                                                            , dtEmployeeListLocal.Rows[i]["Tpy_NetAmt"]
                                                            , dtEmployeeListLocal.Rows[i]["Tpy_HourRate"]);
                    dal.ExecuteNonQuery(query);

                    query = string.Format(@"UPDATE T_EmpPayroll2
                                                SET Tpy_RemainingPayCycle                   ={2}
                                                    ,Tpy_YTDRegularAmtBefore                ={3}
                                                    ,Tpy_AssumeRegularAmt                   ={4}
                                                    ,Tpy_RegularTotal                       ={5}
                                                    ,Tpy_YTDRecurringAllowanceAmtBefore     ={6}
                                                    ,Tpy_YTDRecurringAllowanceAmt           ={7}
                                                    ,Tpy_RecurringAllowanceAmt              ={8}
                                                    ,Tpy_AssumeRecurringAllowanceAmt        ={9}
                                                    ,Tpy_RecurringAllowanceTotal            ={10}
                                                    ,Tpy_YTDBonusAmtBefore                  ={11}
                                                    ,Tpy_BonusNontaxAmt                     ={12}
                                                    ,Tpy_BonusTaxAmt                        ={13}
                                                    ,Tpy_Assume13thMonthAmt                 ={14}
                                                    ,Tpy_BonusTotal                         ={15}
                                                    ,Tpy_BonusTaxRevaluated                 ={16}
                                                    ,Tpy_YTDBonusTaxBefore                  ={17}
                                                    ,Tpy_YTDOtherTaxableIncomeAmtBefore     ={18}
                                                    ,Tpy_OtherTaxableIncomeAmt              ={19}
                                                    ,Tpy_OtherTaxableIncomeTotal            ={20}
                                                    ,Tpy_YTDSSSAmtBefore                    ={21}
                                                    ,Tpy_AssumeSSSAmt                       ={22}
                                                    ,Tpy_SSSTotal                           ={23}
                                                    ,Tpy_YTDPhilhealthAmtBefore             ={24}
                                                    ,Tpy_AssumePhilhealthAmt                ={25}
                                                    ,Tpy_PhilhealthTotal                    ={26}
                                                    ,Tpy_YTDPagIbigAmtBefore                ={27}
                                                    ,Tpy_AssumePagIbigAmt                   ={28}
                                                    ,Tpy_PagIbigTotal                       ={29}
                                                    ,Tpy_YTDPagIbigTaxAmtBefore             ={30}
                                                    ,Tpy_AssumePagIbigTaxAmt                ={31}
                                                    ,Tpy_TotalTaxableIncomeWAssumeAmt       ={32}
                                                    ,Tpy_YTDUnionAmtBefore                  ={33}
                                                    ,Tpy_AssumeUnionAmt                     ={34}
                                                    ,Tpy_UnionTotal                         ={35}
                                                    ,Tpy_PremiumPaidOnHealth                ={36}
                                                    ,Tpy_TotalExemptions                    ={37}
                                                    ,Tpy_NetTaxableIncomeAmt                ={38}
                                                    ,Tpy_YTDREGAmt                          ={39}
                                                    ,Tpy_YTDTotalTaxableIncomeAmtBefore     ={40}
                                                    ,Tpy_YTDTotalTaxableIncomeAmt           ={41}
                                                    ,Tpy_YTDWtaxAmtBefore                   ={42}
                                                    ,Tpy_YTDWtaxAmt                         ={43}
                                                    ,Tpy_YTDSSSAmt                          ={44}
                                                    ,Tpy_YTDPhilhealthAmt                   ={45}
                                                    ,Tpy_YTDPagIbigAmt                      ={46}
                                                    ,Tpy_YTDPagIbigTaxAmt                   ={47}
                                                    ,Tpy_YTDUnionAmt                        ={48}
                                                    ,Tpy_YTDNontaxableAmtBefore             ={49}
                                                    ,Tpy_YTDNontaxableAmt                   ={50}
                                                    ,Tpy_BIRTotalAmountofCompensation       ={51}
                                                    ,Tpy_BIRStatutoryMinimumWage            ={52}
                                                    ,Tpy_BIRHolidayOvertimeNightShiftHazard ={53}
                                                    ,Tpy_BIR13thMonthPayOtherBenefits       ={54}
                                                    ,Tpy_BIRDeMinimisBenefits               ={55}
                                                    ,Tpy_BIRSSSGSISPHICHDMFUnionDues        ={56}
                                                    ,Tpy_BIROtherNonTaxableCompensation     ={57}
                                                    ,Tpy_BIRTotalTaxableCompensation        ={58}
                                                    ,Tpy_BIRTaxableCompensationNotSubjectToWithholdingTax = {59}
                                                    ,Tpy_BIRTotalTaxesWithheld              ={60}
                                                    ,Tpy_YTDMPFAmtBefore                    ={61}
                                                    ,Tpy_YTDMPFAmt                          ={62}
                                                WHERE Tpy_IDNo = '{0}'
                                                AND Tpy_PayCycle = '{1}'"
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_IDNo"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_PayCycle"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_RemainingPayCycle"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_YTDRegularAmtBefore"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_AssumeRegularAmt"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_RegularTotal"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_YTDRecurringAllowanceAmtBefore"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_YTDRecurringAllowanceAmt"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_RecurringAllowanceAmt"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_AssumeRecurringAllowanceAmt"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_RecurringAllowanceTotal"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_YTDBonusAmtBefore"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_BonusNontaxAmt"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_BonusTaxAmt"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_Assume13thMonthAmt"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_BonusTotal"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_BonusTaxRevaluated"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_YTDBonusTaxBefore"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_YTDOtherTaxableIncomeAmtBefore"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_OtherTaxableIncomeAmt"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_OtherTaxableIncomeTotal"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_YTDSSSAmtBefore"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_AssumeSSSAmt"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_SSSTotal"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_YTDPhilhealthAmtBefore"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_AssumePhilhealthAmt"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_PhilhealthTotal"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_YTDPagIbigAmtBefore"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_AssumePagIbigAmt"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_PagIbigTotal"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_YTDPagIbigTaxAmtBefore"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_AssumePagIbigTaxAmt"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_TotalTaxableIncomeWAssumeAmt"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_YTDUnionAmtBefore"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_AssumeUnionAmt"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_UnionTotal"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_PremiumPaidOnHealth"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_TotalExemptions"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_NetTaxableIncomeAmt"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_YTDREGAmt"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_YTDTotalTaxableIncomeAmtBefore"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_YTDTotalTaxableIncomeAmt"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_YTDWtaxAmtBefore"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_YTDWtaxAmt"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_YTDSSSAmt"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_YTDPhilhealthAmt"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_YTDPagIbigAmt"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_YTDPagIbigTaxAmt"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_YTDUnionAmt"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_YTDNontaxableAmtBefore"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_YTDNontaxableAmt"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_BIRTotalAmountofCompensation"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_BIRStatutoryMinimumWage"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_BIRHolidayOvertimeNightShiftHazard"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_BIR13thMonthPayOtherBenefits"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_BIRDeMinimisBenefits"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_BIRSSSGSISPHICHDMFUnionDues"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_BIROtherNonTaxableCompensation"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_BIRTotalTaxableCompensation"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_BIRTaxableCompensationNotSubjectToWithholdingTax"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_BIRTotalTaxesWithheld"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_YTDMPFAmtBefore"]
                                                            , dtEmployeeListLocal2.Rows[i]["Tpy_YTDMPFAmt"]
                                                            );
                    dal.ExecuteNonQuery(query);
                    #endregion

                    #region Insert Special Payroll Record
                    if (IncludedDeductions != string.Empty)
                    {
                        query = string.Format(@"
                                    DELETE FROM T_EmpSpecialPayroll
                                    WHERE Tpy_IDNo = '{0}'
                                        AND Tpy_PayCycle = '{1}'             

                                    INSERT INTO T_EmpSpecialPayroll (Tpy_IDNo,Tpy_PayCycle,Tpy_DeductionCode,Tpy_Amount,Usr_Login,Ludatetime)
                                    VALUES ('{0}','{1}','{2}',{3},'{4}',GETDATE())", dtEmployeeListLocal.Rows[i]["Tpy_IDNo"]
                                                                                       , PayrollPeriod
                                                                                       , IncludedDeductions
                                                                                       , DeductionAmount
                                                                                       , UserLogin);
                        dal.ExecuteNonQuery(query);
                    }
                    #endregion

                    #region Insert Deduction to Payslip
                    InsertToDeductionLedgerPayslip(PayrollPeriod, dtEmployeeListLocal.Rows[i]["Tpy_IDNo"].ToString());
                    #endregion

                    EmpDispHandler(this, new EmpDispEventArgs(dtEmployeeListLocal.Rows[i]["Tpy_IDNo"].ToString(), dtEmployeeListLocal.Rows[i]["Mem_LastName"].ToString(), dtEmployeeListLocal.Rows[i]["Mem_FirstName"].ToString()));
                }
                #endregion                
                StatusHandler(this, new StatusEventArgs(string.Format("Compute Tax, Deductions and Net Pay"), true));
                #endregion

                #region Generate Payroll Error List
                dtErrList = SavePayrollErrorReportList();
                if (dtErrList.Rows.Count > 0)
                    InsertToEmpPayrollCheckTable(dtErrList);
                #endregion
            }
            catch (Exception ex)
            {
                //dal.RollBackTransactionSnapshot();
                CommonProcedures.logErrorToFile(ex.ToString());
                throw new Exception("Special Payroll Calculation has encountered some errors: \n" + ex.Message);
            }
            finally
            {
                //dal.CloseDB();
            }

            return dtErrList;
        }

        public void SetParameters(string companyCode, string centralProfile, DALHelper dalHelper)
        {
            CompanyCode = companyCode;
            CentralProfile = centralProfile;
            this.dal = dalHelper;
            this.commonBL = new CommonBL();

            string strResult;
            strResult = commonBL.GetParameterValueFromPayroll("MINNETPAY", companyCode, dal);
            MINNETPAY = strResult.Equals(string.Empty) ? 0 : Convert.ToDecimal(strResult);

            strResult = commonBL.GetParameterValueFromPayroll("NETPCT", companyCode, dal);
            NETPCT = strResult.Equals(string.Empty) ? 0 : Convert.ToDecimal(strResult);

            strResult = commonBL.GetParameterValueFromPayroll("HRLYRTEDEC", companyCode, dal);
            HRLYRTEDEC = strResult.Equals(string.Empty) ? 0 : Convert.ToDecimal(strResult);

            NETBASE = commonBL.GetParameterValueFromPayroll("NETBASE", companyCode, dal);

        }

        public void ClearPayrollTables(bool ProcessAll, string EmployeeId, string PayPeriod)
        {
            string query = @"DELETE FROM T_EmpPayroll WHERE Tpy_PayCycle = '{0}' {1}
                             DELETE FROM T_EmpPayroll2 WHERE Tpy_PayCycle = '{0}' {1}
                             DELETE FROM T_EmpDeductionDtl WHERE Tdd_ThisPayCycle = '{0}' {2}
                             DELETE FROM T_EmpPayrollMisc WHERE Tpm_PayCycle = '{0}' {3}
                             DELETE FROM T_EmpDeductionHdrCycle WHERE Tdh_PayCycle = '{0}' {4}
                             DELETE FROM T_EmpSpecialPayroll WHERE Tpy_PayCycle = '{0}' {1}";

            if (!ProcessAll && EmployeeId != "")
                query = string.Format(query, PayPeriod
                                            , " AND Tpy_IDNo = '" + EmployeeId + "'"
                                            , " AND Tdd_IDNo = '" + EmployeeId + "'"
                                            , " AND Tpm_IDNo = '" + EmployeeId + "'"
                                            , " AND Tdh_IDNo = '" + EmployeeId + "'");
            else if (ProcessAll && EmployeeList != "")
                query = string.Format(query, PayPeriod
                                            , " AND Tpy_IDNo IN (" + EmployeeList + ") "
                                            , " AND Tdd_IDNo IN (" + EmployeeList + ") "
                                            , " AND Tpm_IDNo IN (" + EmployeeList + ") "
                                            , " AND Tdh_IDNo IN (" + EmployeeList + ") ");
            else
            {
                query = string.Format(query, PayPeriod
                                            , ""
                                            , ""
                                            , ""
                                            , "");
            }
            dal.ExecuteNonQuery(query);
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
        public DataTable GetPayPeriodCycle(string PayPeriod)
        {
            string strQuery = string.Format(@"SELECT	Tps_PayCycle
		                                                , Tps_StartCycle
		                                                , Tps_EndCycle
                                                        , Tps_CycleIndicator
                                                        , Tps_TaxSchedule
                                                        , Tps_TaxComputation
                                                        , Tps_ComputeTax
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

        private DataTable GetPayrollCalcRecord(bool ProcessAll, string tableName, string EmployeeId, string PayPeriod)
        {
            string EmployeeCondition = string.Format(@" WHERE Tpy_PayCycle = '{0}'", PayPeriod);
            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition += " AND Tpy_IDNo = '" + EmployeeId + "' ";
            else if (ProcessAll && EmployeeList != "")
                EmployeeCondition += " AND Tpy_IDNo IN (" + EmployeeList + ") ";

            string query = string.Format(@"SELECT Mem_LastName, Mem_FirstName, A.* FROM {0} A
                                           LEFT JOIN M_Employee ON Mem_IDNo = Tpy_IDNo
                                           {1}"
                                        , tableName
                                        , EmployeeCondition);
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public void CreateCurrentDeduction(bool ProcessAll, string PayPeriod, string EmployeeId, string IncludedDeductions)
        {
            ParameterInfo[] paramInfo = new ParameterInfo[4];
            int paramIndex = 0;
            paramInfo[paramIndex++] = new ParameterInfo("@EndCycleDate", Convert.ToDateTime(PayrollEnd).ToShortDateString());
            paramInfo[paramIndex++] = new ParameterInfo("@PayPeriod", PayPeriod);
            paramInfo[paramIndex++] = new ParameterInfo("@ApplicPeriod", PayPeriod.Substring(6, 1));
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_login", UserLogin);

            string EmployeeDeductionDetailTable = "T_EmpDeductionDtl";
            string strEdlEmployeeIdQuery = "";
            string strEddEmployeeIdQuery = "";
            string queryExt;

            if (!ProcessAll && EmployeeId != "")
            {
                strEdlEmployeeIdQuery = "AND Tdh_IDNo = '" + EmployeeId + "'";
                strEddEmployeeIdQuery = "AND Tdd_IDNo = '" + EmployeeId + "'";
            }
            else if (ProcessAll && EmployeeList != "")
            {
                strEdlEmployeeIdQuery += " AND Tdh_IDNo IN (" + EmployeeList + ") ";
                strEddEmployeeIdQuery += " AND Tdd_IDNo IN (" + EmployeeList + ") ";
            }

            if (IncludedDeductions != "")
            {
                queryExt = string.Format(" AND Tdh_DeductionCode in ({0}) ", EncodeFilterItems(IncludedDeductions));

                #region Query
                string sqlquery = string.Format(@"DELETE FROM {0} WHERE Tdd_ThisPayCycle = @PayPeriod {2}

                                              INSERT INTO {0}
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
                                                -- For Deffered Cycles
                                                SELECT T_EmpDeductionDefer.Tdd_IDNo
                                                    ,T_EmpDeductionDefer.Tdd_DeductionCode
                                                    ,T_EmpDeductionDefer.Tdd_StartDate
                                                    ,@PayPeriod AS Tdd_ThisPayCycle
                                                    ,T_EmpDeductionDefer.Tdd_PayCycle AS Tdd_PayCycle
                                                    ,T_EmpDeductionDefer.Tdd_LineNo AS Tdd_LineNo
                                                    ,'P' AS Tdd_PaymentType
                                                    ,T_EmpDeductionDefer.Tdd_DeferredAmt AS Tdd_Amount
                                                    ,1 AS  Tdd_DeferredFlag
                                                    ,0 AS Tdd_PaymentFlag
                                                    ,@Usr_login
                                                    ,GETDATE()
                                                FROM {5}..T_EmpDeductionHdr
                                                INNER JOIN M_Employee ON T_EmpDeductionHdr.Tdh_IDNo  = M_Employee.Mem_IDNo
                                                    AND  Mem_WorkStatus NOT IN ('IN','IM')
                                                INNER JOIN T_EmpDeductionDefer ON T_EmpDeductionDefer.Tdd_IDNo = T_EmpDeductionHdr.Tdh_IDNo
                                                    and T_EmpDeductionDefer.Tdd_DeductionCode = T_EmpDeductionHdr.Tdh_DeductionCode 
                                                    and T_EmpDeductionDefer.Tdd_StartDate = T_EmpDeductionHdr.Tdh_StartDate 
                                                WHERE T_EmpDeductionHdr.Tdh_PaidAmount < T_EmpDeductionHdr.Tdh_DeductionAmount
                                                    and T_EmpDeductionHdr.Tdh_CycleAmount > 0
						                            and Tdh_ExemptInPayroll = 0
                                                    and T_EmpDeductionHdr.Tdh_StartDate <= @EndCycleDate
                                                    {1} {3}

                                                UNION ALL

                                                -- For Current Payroll cycle
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
                                                FROM {5}..T_EmpDeductionHdr
                                                INNER JOIN M_Employee ON T_EmpDeductionHdr.Tdh_IDNo  = M_Employee.Mem_IDNo
                                                    AND  Mem_WorkStatus NOT IN ('IN','IM')
                                                LEFT JOIN {5}..M_Deduction 
                                                    ON Mdn_DeductionCode = Tdh_DeductionCode
                                                    AND Mdn_CompanyCode = '{4}'
                                                WHERE Tdh_StartDate <= @EndCycleDate
                                                    AND Tdh_DeferredAmount + Tdh_PaidAmount < Tdh_DeductionAmount
                                                    AND Tdh_CycleAmount > 0
						                            AND Tdh_ExemptInPayroll = 0
                                                    AND (   case when len(rtrim(Isnull(Tdh_ApplicablePayCycle,''))) = 0 then Mdn_ApplicablePayCycle else Tdh_ApplicablePayCycle end = '0' 
		                                                 or case when len(rtrim(Isnull(Tdh_ApplicablePayCycle,''))) = 0 then Mdn_ApplicablePayCycle else Tdh_ApplicablePayCycle end =  @ApplicPeriod )

                                                    {1} {3}

                                                DELETE FROM {0}
                                                FROM {0}
                                                INNER JOIN T_EmpDeductionExempt on Tde_IDNo = Tdd_IDNo
                                                  AND Tde_DeductionCode = Tdd_DeductionCode
                                                  AND Tde_StartDate = Tdd_StartDate
                                                  AND Tde_PayCycle = Tdd_ThisPayCycle
                                                WHERE Tdd_ThisPayCycle = @PayPeriod
                                                    {2}", EmployeeDeductionDetailTable
                                                        , strEdlEmployeeIdQuery
                                                        , strEddEmployeeIdQuery
                                                        , queryExt
                                                        , CompanyCode
                                                        , CentralProfile);
                #endregion

                try
                {
                    dal.ExecuteNonQuery(sqlquery, CommandType.Text, paramInfo);
                }
                catch
                {
                    throw new PayrollException("Error upon creation of deductions on or before " + Convert.ToDateTime(PayrollEnd).ToShortDateString() + "." + "\n");
                }
            }
        }

        public DataSet GetDeductionDetails(string EmployeeId, string PayPeriod)
        {
            #region query
            string query = string.Format(@"SELECT M_Deduction.Mdn_PriorityNo, M_Deduction.Mdn_IsAllowPartialPayment, T_EmpDeductionDtl.* 
                                          FROM T_EmpDeductionDtl
                                          INNER JOIN {3}..M_Deduction 
                                            ON Mdn_DeductionCode = Tdd_DeductionCode
                                            AND Mdn_CompanyCode = '{2}'
                                          WHERE Tdd_IDNo = '{0}'
                                            AND Tdd_ThisPayCycle = '{1}'
                                          ORDER BY Mdn_PriorityNo
                                                  ,Tdd_DeductionCode
                                                  ,Tdd_StartDate
                                                  ,Tdd_PayCycle", EmployeeId, PayPeriod, CompanyCode, CentralProfile);
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
            #endregion

            #region query
            string query = string.Format(@"UPDATE T_EmpDeductionDtl
                                          SET Tdd_PaymentFlag = 1
                                          WHERE Tdd_IDNo = @Tdd_IDNo 
                                              AND Tdd_DeductionCode = @Tdd_DeductionCode 
                                              AND Tdd_StartDate = @Tdd_StartDate
                                              AND Tdd_ThisPayCycle = @Tdd_ThisPayCycle
                                              AND Tdd_PayCycle = @Tdd_PayCycle
                                              AND Tdd_LineNo = @Tdd_LineNo");
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
            #endregion

            #region query
            string query = string.Format(@"UPDATE T_EmpDeductionDtl
                                          SET Tdd_Amount = {0}
                                            , Tdd_PaymentFlag = 1
                                          WHERE Tdd_IDNo = @Tdd_IDNo 
                                              AND Tdd_DeductionCode = @Tdd_DeductionCode 
                                              AND Tdd_StartDate = @Tdd_StartDate
                                              AND Tdd_ThisPayCycle = @Tdd_ThisPayCycle
                                              AND Tdd_PayCycle = @Tdd_PayCycle
                                              AND Tdd_LineNo = @Tdd_LineNo", Math.Round(NetPay - MinNetpay, 2));
            #endregion
            dal.ExecuteNonQuery(query, CommandType.Text, paramUpdPayFlg);

            #region Split new deduction
            query = string.Format(@"INSERT INTO T_EmpDeductionDtl
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
                                           ,'{0:00}'
                                           ,'P'
                                           ,{1}
                                           ,'FALSE'
                                           ,'FALSE'
                                           ,'SA'
                                           ,GETDATE())", GetLastEmployeeDeductionDetailRecord(drDeduction), Math.Round(Convert.ToDecimal(drDeduction["Tdd_Amount"]) - Math.Round(NetPay - MinNetpay, 2)));
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
            #endregion

            #region query
            string query = string.Format(@"SELECT Tdd_LineNo
                                           FROM T_EmpDeductionDtl
                                           WHERE Tdd_IDNo = @Tdd_IDNo 
                                              AND Tdd_DeductionCode = @Tdd_DeductionCode 
                                              AND Tdd_StartDate = @Tdd_StartDate
                                              AND Tdd_ThisPayCycle = @Tdd_ThisPayCycle
                                              AND Tdd_PayCycle = @Tdd_PayCycle
                                           ORDER BY Tdd_LineNo DESC");
            #endregion
            DataTable dtResult;
            decimal SeqNo = 99;
            dtResult = dal.ExecuteDataSet(query, CommandType.Text, paramUpdPayFlg).Tables[0];
            if (dtResult.Rows.Count > 0)
            {
                for (int i = 0; i < dtResult.Rows.Count; i++)
                {
                    if (SeqNo == Convert.ToDecimal(dtResult.Rows[0][0]))
                        SeqNo--;
                }
            }
            return SeqNo;
        }

        public void InsertToDeductionLedgerPayslip(string PayPeriod, string EmployeeId)
        {
            string EmployeeDeductionLedgerPayslipTable = "T_EmpDeductionHdrCycle";
            string EmployeeDeductionDetailTable = "T_EmpDeductionDtl";

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
                                            , EmployeeDeductionDetailTable
                                            , CentralProfile);
                #endregion
                dal.ExecuteNonQuery(sqlquery);
            }
            catch
            {
                throw new PayrollException("Error in inserting data to payslip.");
            }
        }

        public void ComputeIncomeAndDeductionAdjustments(DataRow drEmpPayroll, DataRow drEmpPayroll2)
        {
            DataSet dsIncomeDednAdj = NewPayrollCalculationBL.GetIncomeDeductionAdjustments(false, drEmpPayroll["Tpy_IDNo"].ToString());

            foreach (DataRow drIncomeDednAdj in dsIncomeDednAdj.Tables[0].Rows)
            {
                drEmpPayroll2["Tpy_BIRTotalAmountofCompensation"] = Convert.ToDecimal(drEmpPayroll["Tpy_GrossIncomeAmt"]) - Convert.ToDecimal(drIncomeDednAdj["NOTINCLUDE"].ToString());

                if (Convert.ToBoolean(drEmpPayroll["Tpy_IsTaxExempted"]))
                {
                    drEmpPayroll2["Tpy_BIRStatutoryMinimumWage"] = Convert.ToDecimal(drEmpPayroll["Tpy_REGAmt"])
                                                                    + Convert.ToDecimal(drEmpPayroll["Tpy_PDLVAmt"])
                                                                    + Convert.ToDecimal(drIncomeDednAdj["BASICSAL"].ToString());

                    drEmpPayroll2["Tpy_BIRHolidayOvertimeNightShiftHazard"] = Convert.ToDecimal(drEmpPayroll["Tpy_TotalOTNDAmt"])
                                                                    + Convert.ToDecimal(drEmpPayroll["Tpy_PDLEGHOLAmt"])
                                                                    + Convert.ToDecimal(drEmpPayroll["Tpy_PDSPLHOLAmt"])
                                                                    + Convert.ToDecimal(drEmpPayroll["Tpy_PDCOMPHOLAmt"])
                                                                    + Convert.ToDecimal(drEmpPayroll["Tpy_PDOTHHOLAmt"])
                                                                    + Convert.ToDecimal(drEmpPayroll["Tpy_PDRESTLEGHOLAmt"])
                                                                    + Convert.ToDecimal(drIncomeDednAdj["HolidayOvertimeNightShiftHazard"].ToString());
                }

                drEmpPayroll2["Tpy_BIR13thMonthPayOtherBenefits"] = Convert.ToDecimal(drIncomeDednAdj["N13THBEN"].ToString());
                drEmpPayroll2["Tpy_BIRDeMinimisBenefits"] = Convert.ToDecimal(drIncomeDednAdj["NDEMINIMIS"].ToString());
                drEmpPayroll2["Tpy_BIRSSSGSISPHICHDMFUnionDues"] = Convert.ToDecimal(drEmpPayroll["Tpy_SSSEE"])
                                                                + Convert.ToDecimal(drEmpPayroll["Tpy_MPFEE"])
                                                                + Convert.ToDecimal(drEmpPayroll["Tpy_PagIbigEE"])
                                                                + Convert.ToDecimal(drEmpPayroll["Tpy_PhilhealthEE"])
                                                                + Convert.ToDecimal(drEmpPayroll["Tpy_UnionAmt"])
                                                                + Convert.ToDecimal(drIncomeDednAdj["DNPREMIUMS"].ToString())
                                                                - Convert.ToDecimal(drIncomeDednAdj["INPREMIUMS"].ToString());

                if (Convert.ToDecimal(drEmpPayroll["Tpy_WtaxAmt"]) < 0)
                    drEmpPayroll2["Tpy_BIROtherNonTaxableCompensation"] = (Convert.ToDecimal(drEmpPayroll["Tpy_WtaxAmt"]) * -1)
                                                                    + Convert.ToDecimal(drIncomeDednAdj["NSALOTH"].ToString());
                else
                    drEmpPayroll2["Tpy_BIROtherNonTaxableCompensation"] = Convert.ToDecimal(drIncomeDednAdj["NSALOTH"].ToString());

                drEmpPayroll2["Tpy_BIRTotalTaxesWithheld"] = Convert.ToDecimal(drEmpPayroll["Tpy_WtaxAmt"])
                                                            + Convert.ToDecimal(drIncomeDednAdj["DWTAX"].ToString())
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

        public void UpdateBankinPolicyDtl(string PayCycleCode, string user, string companyCode, string centralProfile, DALHelper dalhelper)
        {
            string query = string.Format(@"UPDATE M_PolicyDtl
                                                        SET Mpd_ParamValue = 1
                                                            ,Mpd_UpdatedBy = '{1}'
                                                            ,Mpd_UpdatedDate = GETDATE()
                                                        WHERE Mpd_PolicyCode = 'OFFATMGEN'
                                                        AND Mpd_SubCode = (SELECT Mbn_BankCode 
									                                        FROM {2}..M_Bank
									                                        WHERE Mbn_CompanyCode = '{3}')
                                                        AND 1 = (
	                                                        SELECT
		                                                        CASE WHEN Tps_CycleIndicator = 'S' AND Tps_CycleIndicatorSpecial = 'C'
			                                                        THEN 1
			                                                        ELSE 0
			                                                        END
	                                                        FROM T_PaySchedule
	                                                        WHERE Tps_PayCycle = '{0}'
                                                        )", PayCycleCode, user, centralProfile, companyCode);

            dalhelper.ExecuteNonQuery(query);
        }
        public string EncodeFilterItems(string strDelimited)
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
                dtErrList.Rows[dtErrList.Rows.Count - 1]["IDNumber"] = listPayrollRept[i].strEmployeeId;
                dtErrList.Rows[dtErrList.Rows.Count - 1]["Last Name"] = listPayrollRept[i].strLastName;
                dtErrList.Rows[dtErrList.Rows.Count - 1]["First Name"] = listPayrollRept[i].strFirstName;
                dtErrList.Rows[dtErrList.Rows.Count - 1]["Amount1"] = string.Format("{0:0.00}", listPayrollRept[i].dAmount);
                dtErrList.Rows[dtErrList.Rows.Count - 1]["Amount2"] = string.Format("{0:0.00}", listPayrollRept[i].dAmount2);
                dtErrList.Rows[dtErrList.Rows.Count - 1]["Remarks"] = listPayrollRept[i].strRemarks;
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
                                                               , UserLogin);
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
        #region <Delegates>
        private void ShowPayrollCalcEmployeeName(object sender, NewPayrollCalculationBL.EmpDispEventArgs e)
        {
        }
        private void ShowPayrollStatus(object sender, NewPayrollCalculationBL.StatusEventArgs e)
        {
        }

        #endregion
    }
}
