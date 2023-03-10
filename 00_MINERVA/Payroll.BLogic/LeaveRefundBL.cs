using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using CommonLibrary;
using Payroll.DAL;
using System.Threading;
using System.Collections;

namespace Payroll.BLogic
{
    /// <summary>
    /// Description : Leave Refund BL
    /// </summary>
    /// 
    public class LeaveRefundBL : BaseBL
    {
        #region <Global Variables>
        DALHelper dal;
        SystemCycleProcessingBL SystemCycleProcessingBL;
        NewPayrollCalculationBL NewPayrollCalculationBL = new NewPayrollCalculationBL();
        CommonBL commonBL;

        string LoginUser = "";
        string CompanyCode = "";
        string CentralProfile = "";
        string PayCycleCode = "";

        //Storage tables
        DataTable dtLeaveMaster = null;

        //Flags and parameters
        public bool LVHRENTRY               = false;
        public decimal MDIVISOR             = 0;
        public decimal HRLYRTEDEC           = 0;
        public string FPLVREFUNDDIVISOR     = "";
        public string FPLVREFUNDDIVIDEND    = "";
        public string FPLVREFUNDRULE        = "";
        public string FPLVREFUNDFORMULA     = "";
        #endregion

        #region Overrides

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

        public override DataSet FetchAll()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override DataRow Fetch(string code)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region Event handlers for Leave Refund processing
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

        public string ComputeLeaveRefund(bool ProcessSeparated, bool Recompute, string PayrollPeriod, string EmployeeID, string UserLogin, string companyCode, string centralProfile, string profileCode, string strControlNumber, string LeaveYear, DALHelper dalHelper)
        {
            return ComputeLeaveRefund(false, ProcessSeparated, Recompute, PayrollPeriod, EmployeeID, UserLogin, companyCode, centralProfile, profileCode, "", "", "", "", "", "", "", "", "" , strControlNumber, "", "", "", LeaveYear, dalHelper);
        }

        public string ComputeLeaveRefund(bool ProcessAll
            , bool ProcessSeparated
            , bool Recompute
            , string PayrollPeriod
            , string EmployeeID
            , string UserLogin
            , string companyCode
            , string centralProfile
            , string profileCode
            , string IDNoLIst
            , string CostCenterList
            , string PayrollGroupList
            , string EmploymentStatusList
            , string PayrollTypeList
            , string PositionList
            , string GradeList
            , string HireRegOption
            , object HireRegDate
            , string strControlNumber
            , object SalaryDate
            , string strLeaveYearType
            , string QueryCondition
            , string SeparationYear
            , DALHelper dalHelper)
        {
            #region Variables
            DataTable dtRule = null;
            string ControlNumber = string.Empty;
            int ctr = 1;
            #endregion

            try
            {
                #region Initial Setup
                this.dal = dalHelper;
                SystemCycleProcessingBL = new SystemCycleProcessingBL(dal, PayrollPeriod, UserLogin, companyCode, centralProfile);
                commonBL = new CommonBL();
                LoginUser = UserLogin;
                CompanyCode = companyCode;
                CentralProfile = centralProfile;
                PayCycleCode = PayrollPeriod;

                SetProcessFlags();
                #endregion

                if (PayrollPeriod == string.Empty)
                    throw new PayrollException("No Pay Cyle Code");

                if (!Recompute)
                {
                    ControlNumber = commonBL.GetDocumentNumber(dal, new DALHelper(CentralProfile, false), "LEAVETRN", profileCode);
                    #region Create Leave Hdr Records
                    DataRow drEmpLeaveHdr = DbRecord.Generate(CommonConstants.TableName.T_EmpLeaveRefundHdr);
                    drEmpLeaveHdr["Tlh_ControlNo"]              = ControlNumber;
                    drEmpLeaveHdr["Tlh_PayCycle"]               = PayrollPeriod;
                    drEmpLeaveHdr["Tlh_IDNoLIst"]               = IDNoLIst;
                    drEmpLeaveHdr["Tlh_CostCenterList"]         = CostCenterList;
                    drEmpLeaveHdr["Tlh_PayrollGroupList"]       = PayrollGroupList;
                    drEmpLeaveHdr["Tlh_EmploymentStatusList"]   = EmploymentStatusList;
                    drEmpLeaveHdr["Tlh_PayrollTypeList"]        = PayrollTypeList;
                    drEmpLeaveHdr["Tlh_PositionList"]           = PositionList;
                    drEmpLeaveHdr["Tlh_GradeList"]              = GradeList;
                    drEmpLeaveHdr["Tlh_HireRegOption"]          = HireRegOption;
                    if (GetValue(HireRegDate) != string.Empty)
                        drEmpLeaveHdr["Tlh_HireRegDate"]        = Convert.ToDateTime(HireRegDate);
                    else
                        drEmpLeaveHdr["Tlh_HireRegDate"]        = DBNull.Value;
                    drEmpLeaveHdr["Tlh_RecordStatus"]           = "A";
                    drEmpLeaveHdr["Usr_Login"]                  = UserLogin;

                    InsertHdr(drEmpLeaveHdr, dal);

                    #endregion
                }

                else if (Recompute)
                {
                    ControlNumber = strControlNumber;
                    #region Delete in Income
                    string tableIncome = CommonConstants.TableName.T_EmpIncome;
                    string NontaxCode = "NONTAX";
                    string TaxableCode = "TAX";
                    string Condition = string.Empty;
                    if (ProcessSeparated)
                    {
                        tableIncome = CommonConstants.TableName.T_EmpIncomeFinalPay;
                        NontaxCode = "NONTAXFP";
                        TaxableCode = "TAXFP";
                        Condition = string.Format("AND Tin_IDNo IN ({0})", EmployeeID);
                    }
                    string queryDelete = string.Format(@"DELETE FROM {3}
                                                         WHERE Tin_IncomeCode IN (SELECT DISTINCT Tld_LeaveCode+[Name] AS [Code]
                                                                              FROM T_EmpLeaveRefundDtl
                                                                              LEFT JOIN (SELECT 'N' AS [Class],'{1}' AS [Name]
				                                                                        UNION
				                                                                        SELECT 'T' AS [Class],'{2}' AS [Name] ) taxclass ON 1=1) 
                                                        AND Tin_PayCycle = '{0}'
                                                        AND Tin_OrigPayCycle = '{0}'
                                                        {4}", PayrollPeriod
                                                        , TaxableCode
                                                        , NontaxCode
                                                        , tableIncome
                                                        , Condition);
                    dal.ExecuteNonQuery(queryDelete);
                    #endregion
                    #region Update Leave Refund Hdr Records
                    DataRow drEmpLeaveHdr = DbRecord.Generate(CommonConstants.TableName.T_EmpLeaveRefundHdr);
                    #region Assign to row
                    drEmpLeaveHdr["Tlh_ControlNo"]              = ControlNumber;
                    drEmpLeaveHdr["Tlh_PayCycle"]               = PayrollPeriod;
                    drEmpLeaveHdr["Tlh_IDNoLIst"]               = IDNoLIst;
                    drEmpLeaveHdr["Tlh_CostCenterList"]         = CostCenterList;
                    drEmpLeaveHdr["Tlh_PayrollGroupList"]       = PayrollGroupList;
                    drEmpLeaveHdr["Tlh_EmploymentStatusList"]   = EmploymentStatusList;
                    drEmpLeaveHdr["Tlh_PayrollTypeList"]        = PayrollTypeList;
                    drEmpLeaveHdr["Tlh_PositionList"]           = PositionList;
                    drEmpLeaveHdr["Tlh_GradeList"]              = GradeList;
                    drEmpLeaveHdr["Tlh_HireRegOption"]          = HireRegOption;
                    if (GetValue(HireRegDate) != string.Empty)
                        drEmpLeaveHdr["Tlh_HireRegDate"]        = Convert.ToDateTime(HireRegDate);
                    else
                        drEmpLeaveHdr["Tlh_HireRegDate"]        = DBNull.Value;
                    drEmpLeaveHdr["Tlh_RecordStatus"]           = "A";
                    drEmpLeaveHdr["Usr_Login"]                  = UserLogin;

                    #endregion
                    UpdateHdr(drEmpLeaveHdr, dal);
                    #endregion
                    #region Rule Salary Date
                    dtRule = dal.ExecuteDataSet(string.Format(@"
                                                        SELECT * FROM T_EmpLeaveRefundRule 
                                                        WHERE Tlr_ControlNo = '{0}'", ControlNumber)).Tables[0];
                    #endregion
                    #region Cleanup Rule and Dtl
                    if (!ProcessSeparated)
                    dal.ExecuteNonQuery(string.Format(@"
                                                        DELETE FROM T_EmpLeaveRefundRule 
                                                        WHERE Tlr_ControlNo = '{0}'

                                                        DELETE FROM T_EmpLeaveRefundDtl 
                                                            WHERE Tld_ControlNo = '{0}'
                                                        ", ControlNumber));
                    else
                        dal.ExecuteNonQuery(string.Format(@"
                                                        DELETE FROM T_EmpLeaveRefundDtl 
                                                            WHERE Tld_ControlNo = '{0}'
                                                            AND Tld_IDNo = '{1}'
                                                        "
                                                        , ControlNumber
                                                        , EmployeeID));
                    #endregion
                }

                if (strLeaveYearType.Equals("")) //Final Pay
                    strLeaveYearType = JoinEmployeesFromDataTableArray(GetConvertibleToCashLeaveType(ProcessSeparated, SeparationYear), false);

                if (!strLeaveYearType.Equals(""))
                {
                    string[] strArrCodeItems = strLeaveYearType.Split(new char[] { ',' });
                    foreach (string LeaveYearCode in strArrCodeItems)
                    {
                        string LeaveYear = LeaveYearCode.Substring(0, 4);
                        string LeaveCode = LeaveYearCode.Substring(4, LeaveYearCode.Length - 4);
                        DataRow[] drLeaveMaster = dtLeaveMaster.Select("Mlv_LeaveCode = '" + LeaveCode + "'");
                        if (drLeaveMaster.Length > 0)
                        {
                            string EncashmentBasis              = drLeaveMaster[0]["Mlv_EncashmentBasis"].ToString().Trim();
                            string EncashmentRate               = drLeaveMaster[0]["Mlv_EncashmentRate"].ToString().Trim();
                            string EncashmentRule               = drLeaveMaster[0]["Mlv_EncashmentRule"].ToString().Trim();
                            string EncashmentCredit             = drLeaveMaster[0]["Mlv_EncashmentCredit"].ToString().Trim();
                            string EncashmentNontaxableRule     = drLeaveMaster[0]["Mlv_EncashmentNontaxableRule"].ToString().Trim();
                            string EncashmentNontaxableCredit   = drLeaveMaster[0]["Mlv_EncashmentNontaxableCredit"].ToString().Trim();
                            string EncashmentTaxableRule        = drLeaveMaster[0]["Mlv_EncashmentTaxableRule"].ToString().Trim();
                            string EncashmentTaxableCredit      = GetValue(drLeaveMaster[0]["Mlv_EncashmentTaxableCredit"].ToString().Trim());
                            string CarryForwardRule             = drLeaveMaster[0]["Mlv_CarryForwardRule"].ToString().Trim();
                            string CarryForwardCredit           = GetValue(drLeaveMaster[0]["Mlv_CarryForwardCredit"].ToString().Trim());
                            string CarryForwardLeaveCode        = GetValue(drLeaveMaster[0]["Mlv_CarryForwardLeaveCode"].ToString().Trim());

                            if (ProcessSeparated)
                                EncashmentRule = "A";

                            #region Create Rule Records
                            DataRow drEmpLeaveRule = DbRecord.Generate(CommonConstants.TableName.T_EmpLeaveRefundRule);
                            drEmpLeaveRule["Tlr_ControlNo"]         = ControlNumber;
                            drEmpLeaveRule["Tlr_LineNo"]            = GetSeqNo(ControlNumber, dal) + 1;
                            drEmpLeaveRule["Tlr_LeaveYear"]         = LeaveYear;
                            drEmpLeaveRule["Tlr_LeaveCode"]         = LeaveCode;
                            drEmpLeaveRule["Tlr_EncashmentBasis"]   = EncashmentBasis;
                            drEmpLeaveRule["Tlr_EncashmentRate"]    = EncashmentRate;
                            if (!Recompute)
                            {
                                if (SalaryDate.ToString() != "")
                                    drEmpLeaveRule["Tlr_SalaryDate"] = Convert.ToDateTime(SalaryDate);
                                else
                                    drEmpLeaveRule["Tlr_SalaryDate"] = DBNull.Value;
                            }
                            else if (Recompute)
                            {
                                DataRow[] drResult = dtRule.Select(string.Format("Tlr_LeaveYear = '{0}' AND Tlr_LeaveCode = '{1}'", LeaveYear, LeaveCode));
                                if (drResult.Length > 0)
                                {
                                    SalaryDate = GetValue(drResult[0]["Tlr_SalaryDate"]);
                                    if (SalaryDate.ToString() != string.Empty)
                                        drEmpLeaveRule["Tlr_SalaryDate"] = Convert.ToDateTime(drResult[0]["Tlr_SalaryDate"]);
                                    else
                                        drEmpLeaveRule["Tlr_SalaryDate"] = DBNull.Value;
                                }
                            }

                            drEmpLeaveRule["Tlr_EncashmentRule"]        = EncashmentRule;
                            if (EncashmentCredit != string.Empty)
                                drEmpLeaveRule["Tlr_EncashmentCredit"] = EncashmentCredit;
                            else
                                drEmpLeaveRule["Tlr_EncashmentCredit"] = DBNull.Value;
                            drEmpLeaveRule["Tlr_EncashmentNontaxableRule"] = EncashmentNontaxableRule;
                            if (EncashmentNontaxableCredit != string.Empty)
                                drEmpLeaveRule["Tlr_EncashmentNontaxableCredit"] = EncashmentNontaxableCredit;
                            else
                                drEmpLeaveRule["Tlr_EncashmentNontaxableCredit"] = DBNull.Value;
                            drEmpLeaveRule["Tlr_EncashmentTaxableRule"] = EncashmentTaxableRule;
                            if (EncashmentTaxableCredit != string.Empty)
                                drEmpLeaveRule["Tlr_EncashmentTaxableCredit"] = EncashmentTaxableCredit;
                            else
                                drEmpLeaveRule["Tlr_EncashmentTaxableCredit"] = DBNull.Value;
                            drEmpLeaveRule["Tlr_CarryForwardRule"]      = CarryForwardRule;
                            if (CarryForwardCredit != string.Empty)
                                drEmpLeaveRule["Tlr_CarryForwardCredit"] = CarryForwardCredit;
                            else
                                drEmpLeaveRule["Tlr_CarryForwardCredit"] = DBNull.Value;
                            drEmpLeaveRule["Tlr_CarryForwardLeaveCode"]  = CarryForwardLeaveCode;
                            drEmpLeaveRule["Tlr_CarryForwardPickUp"]     = false;
                            drEmpLeaveRule["Usr_Login"]                  = UserLogin;
                            if (!ProcessSeparated)
                                InsertLeaveRule(drEmpLeaveRule, dal);
                            else if (ProcessSeparated && !Recompute)
                                InsertLeaveRule(drEmpLeaveRule, dal);
                            #endregion

                            #region Query Employee
                            string query = string.Format(@"
                                                        SELECT Tll_IDNo
                                                            , Tll_BegCredit
                                                            , Tll_CarryForwardCredit
                                                            , Tll_UsedCredit
                                                            , (Tll_BegCredit + Tll_CarryForwardCredit) - Tll_UsedCredit AS [Leave Balance]
                                                            , Mem_LastName + CASE WHEN LEN(ISNULL(Mem_ExtensionName,'')) > 0 
													            THEN  ' ' + Mem_ExtensionName ELSE '' END as Mem_LastName
                                                            , Mem_FirstName
                                                            , Mem_MiddleName
                                                            , Mem_CostcenterCode
                                                            , Mem_PayrollType
                                                            , Mem_PayrollGroup
                                                            , Mem_EmploymentStatusCode
                                                            , Mem_WorkStatus
                                                            , Mem_PositionGrade
                                                            , CONVERT(DATE, Mem_SeparationDate) AS Mem_SeparationDate
                                                            , CASE WHEN YEAR(Mem_SeparationDate) = YEAR(Mem_IntakeDate) THEN CONVERT(DATE, Mem_IntakeDate) ELSE CONVERT(DATE,'01/01/' + CAST(YEAR(Mem_SeparationDate) AS VARCHAR)) END AS Mem_IntakeDate
                                                            , Tsl_SalaryRate
                                                        FROM {1}..Udv_LeaveLdg
                                                        INNER JOIN M_Employee
                                                            ON Tll_IDNo = Mem_IDNo
                                                        LEFT JOIN {1}..T_EmpSalary
                                                            ON Tll_IDNo = Tsl_IDNo
                                                            AND {2} >= Tsl_StartDate 
                                                            AND {2} <= ISNULL(Tsl_EndDate, {2})
                                                        {0}
                                                        AND Tll_LeaveCode = @LEAVECODE
                                                        AND Tll_LeaveYear = @LEAVEYEAR
                                                        {3}"
                                                        , (ProcessSeparated ? string.Format("WHERE Tll_IDNo = '{0}'", EmployeeID) : QueryCondition)
                                                        , centralProfile
                                                        , (ProcessSeparated ? "Mem_SeparationDate" : "@SALARYDATE")
                                                        , ((ProcessSeparated && FPLVREFUNDRULE == "F") ? "" : "AND (Tll_BegCredit + Tll_CarryForwardCredit) - Tll_UsedCredit > 0"));


                            #endregion

                            #region Create Dtl Records
                            ParameterInfo[] paramSelect = new ParameterInfo[3];
                            paramSelect[0] = new ParameterInfo("@LEAVECODE", LeaveCode);
                            paramSelect[1] = new ParameterInfo("@LEAVEYEAR", LeaveYear);
                            paramSelect[2] = new ParameterInfo("@SALARYDATE", (GetValue(SalaryDate) != string.Empty ? SalaryDate : DBNull.Value), SqlDbType.Date);

                            DataTable dtLeaveLdgEmployees = dal.ExecuteDataSet(query, CommandType.Text, paramSelect).Tables[0];
                            for (int i = 0; i < dtLeaveLdgEmployees.Rows.Count; i++)
                            {
                                string curEmployeeId = dtLeaveLdgEmployees.Rows[i]["Tll_IDNo"].ToString();
                                MDIVISOR = Convert.ToDecimal(commonBL.GetParameterDtlValueFromPayroll("MDIVISOR", dtLeaveLdgEmployees.Rows[i]["Mem_EmploymentStatusCode"].ToString(), companyCode, dal));
                                decimal SalaryRate = 0;
                                try
                                {
                                    SalaryRate = Convert.ToDecimal(dtLeaveLdgEmployees.Rows[i]["Tsl_SalaryRate"].ToString());
                                }
                                catch { SalaryRate = 0; }

                                DataRow drEmpLeaveDtl = DbRecord.Generate(CommonConstants.TableName.T_EmpLeaveRefundDtl);
                                drEmpLeaveDtl["Tld_ControlNo"]          = ControlNumber;
                                drEmpLeaveDtl["Tld_LineNo"]             = (!ProcessSeparated ? ctr.ToString().PadLeft(6, '0') : (GetSeqNoDtl(ControlNumber, dal) + 1).ToString().PadLeft(6, '0')); 
                                drEmpLeaveDtl["Tld_IDNo"]               = curEmployeeId;
                                drEmpLeaveDtl["Tld_LeaveCode"]          = LeaveCode;
                                drEmpLeaveDtl["Tld_LeaveHr"]            = 0;
                                drEmpLeaveDtl["Tld_LeaveAmt"]           = 0;
                                drEmpLeaveDtl["Tld_LeaveNontaxableHr"]  = 0;
                                drEmpLeaveDtl["Tld_LeaveNontaxableAmt"] = 0;
                                drEmpLeaveDtl["Tld_LeaveTaxableHr"]     = 0;
                                drEmpLeaveDtl["Tld_LeaveTaxableAmt"]    = 0;
                                drEmpLeaveDtl["Tld_SalaryRate"]         = 0;
                                drEmpLeaveDtl["Tld_HourlyDailyRate"]    = 0;
                                drEmpLeaveDtl["Tld_PostFlag"]           = 0;
                                drEmpLeaveDtl["Tld_BegCredit"]          = dtLeaveLdgEmployees.Rows[i]["Tll_BegCredit"];
                                drEmpLeaveDtl["Tld_CarryForwardCredit"] = dtLeaveLdgEmployees.Rows[i]["Tll_CarryForwardCredit"];
                                drEmpLeaveDtl["Tld_UsedCredit"]         = dtLeaveLdgEmployees.Rows[i]["Tll_UsedCredit"];
                                drEmpLeaveDtl["Tld_DividendValue"]      = 0;
                                drEmpLeaveDtl["Tld_DivisorValue"]       = 0;
                                drEmpLeaveDtl["Tld_CarryForwardCreditNextYear"] = 0;
                                drEmpLeaveDtl["Tld_CostCenter"]         = dtLeaveLdgEmployees.Rows[i]["Mem_CostcenterCode"].ToString();
                                drEmpLeaveDtl["Tld_PayrollGroup"]       = dtLeaveLdgEmployees.Rows[i]["Mem_PayrollGroup"].ToString();
                                drEmpLeaveDtl["Tld_EmploymentStatus"]   = dtLeaveLdgEmployees.Rows[i]["Mem_EmploymentStatusCode"].ToString();
                                drEmpLeaveDtl["Tld_PayrollType"]        = dtLeaveLdgEmployees.Rows[i]["Mem_PayrollType"].ToString();
                                drEmpLeaveDtl["Tld_Position"]           = dtLeaveLdgEmployees.Rows[i]["Mem_WorkStatus"].ToString();
                                drEmpLeaveDtl["Tld_Grade"]              = dtLeaveLdgEmployees.Rows[i]["Mem_PositionGrade"].ToString();
                                drEmpLeaveDtl["Usr_Login"]              = UserLogin;

                                decimal HourlyRate          = 0;
                                decimal DailyRate           = 0;
                                decimal dLeaveBalance       = 0;
                                decimal leaveHr             = 0;
                                decimal leaveAmt            = 0;
                                decimal leaveNontaxHr       = 0;
                                decimal leaveNontaxAmt      = 0;
                                decimal leaveTaxHr          = 0;
                                decimal leaveTaxAmt         = 0;
                                decimal dCarryForwardCredit = 0;
                                decimal dConvertedCredit    = 0;

                                if (!ProcessSeparated)
                                    dLeaveBalance = Convert.ToDecimal(dtLeaveLdgEmployees.Rows[i]["Leave Balance"].ToString());
                                else
                                {
                                    if (FPLVREFUNDRULE == "C") dLeaveBalance = Convert.ToDecimal(dtLeaveLdgEmployees.Rows[i]["Leave Balance"].ToString());
                                    else if (FPLVREFUNDRULE == "F") //FORMULA
                                    {

                                        int idxx = 0;
                                        ParameterInfo[] paramdiv = new ParameterInfo[3];
                                        paramdiv[idxx++] = new ParameterInfo("@IDNUMBER", curEmployeeId);
                                        paramdiv[idxx++] = new ParameterInfo("@SEPARATIONDATE", dtLeaveLdgEmployees.Rows[i]["Mem_SeparationDate"], SqlDbType.Date);
                                        paramdiv[idxx++] = new ParameterInfo("@STARTDATE", dtLeaveLdgEmployees.Rows[i]["Mem_IntakeDate"], SqlDbType.Date);
                                        drEmpLeaveDtl["Tld_DividendValue"] = GetFormulaQueryStringValue(FPLVREFUNDDIVIDEND.Replace("@CENTRALDB", centralProfile), paramdiv);
                                        drEmpLeaveDtl["Tld_DivisorValue"] = GetFormulaQueryStringValue(FPLVREFUNDDIVISOR.Replace("@CENTRALDB", centralProfile), paramdiv);

                                        idxx = 0;
                                        ParameterInfo[] paramInfo = new ParameterInfo[5];
                                        paramInfo[idxx++] = new ParameterInfo("@IDNUMBER", curEmployeeId);
                                        paramInfo[idxx++] = new ParameterInfo("@LEAVETYPE", LeaveCode);
                                        paramInfo[idxx++] = new ParameterInfo("@LEAVEYEAR", LeaveYear);
                                        paramInfo[idxx++] = new ParameterInfo("@FPLVREFUNDDIVIDEND", GetValue(drEmpLeaveDtl["Tld_DividendValue"]), SqlDbType.Decimal);
                                        paramInfo[idxx++] = new ParameterInfo("@FPLVREFUNDDIVISOR", GetValue(drEmpLeaveDtl["Tld_DivisorValue"]), SqlDbType.Decimal);
                                        dLeaveBalance = GetFormulaQueryDecimalValue(FPLVREFUNDFORMULA.Replace("@CENTRALDB", centralProfile), paramInfo);
                                    }
                                }

                                if (EncashmentBasis == "S")
                                {
                                    if (dtLeaveLdgEmployees.Rows[i]["Mem_PayrollType"].ToString() == "D")
                                    {
                                        HourlyRate = Math.Round(SalaryRate / CommonBL.HOURSINDAY, Convert.ToInt32(HRLYRTEDEC), MidpointRounding.AwayFromZero);
                                        DailyRate = Math.Round(SalaryRate, Convert.ToInt32(HRLYRTEDEC), MidpointRounding.AwayFromZero);
                                    }
                                    else if (dtLeaveLdgEmployees.Rows[i]["Mem_PayrollType"].ToString() == "M")
                                    {
                                        HourlyRate = Math.Round((SalaryRate * 12) / MDIVISOR / CommonBL.HOURSINDAY, Convert.ToInt32(HRLYRTEDEC), MidpointRounding.AwayFromZero);
                                        DailyRate = Math.Round((SalaryRate * 12) / MDIVISOR, Convert.ToInt32(HRLYRTEDEC), MidpointRounding.AwayFromZero);
                                    }
                                    else if (dtLeaveLdgEmployees.Rows[i]["Mem_PayrollType"].ToString() == "H")
                                        HourlyRate = Math.Round(SalaryRate, Convert.ToInt32(HRLYRTEDEC), MidpointRounding.AwayFromZero);

                                    drEmpLeaveDtl["Tld_SalaryRate"] = SalaryRate;
                                    drEmpLeaveDtl["Tld_HourlyDailyRate"] = (LVHRENTRY ? HourlyRate : DailyRate);
                                }
                                else if (EncashmentBasis == "F")
                                    drEmpLeaveDtl["Tld_SalaryRate"] = Convert.ToDecimal(EncashmentRate);


                                if (!EncashmentRule.Equals("N"))
                                {
                                    leaveHr = ComputeLeaveCredits(dLeaveBalance
                                                                        , EncashmentRule
                                                                        , EncashmentCredit);

                                    leaveAmt = ComputeLeaveAmount(EncashmentBasis
                                                                        , Convert.ToDecimal(EncashmentRate)
                                                                        , leaveHr
                                                                        , HourlyRate
                                                                        , DailyRate);

                                    drEmpLeaveDtl["Tld_LeaveHr"] = leaveHr;
                                    drEmpLeaveDtl["Tld_LeaveAmt"] = Math.Round(leaveAmt, 2);
                                }

                                if (!EncashmentNontaxableRule.Equals("N"))
                                {
                                    leaveNontaxHr = ComputeLeaveCredits(leaveHr
                                                                        , EncashmentNontaxableRule
                                                                        , EncashmentNontaxableCredit);

                                    leaveNontaxAmt = ComputeLeaveAmount(EncashmentBasis
                                                                        , Convert.ToDecimal(EncashmentRate)
                                                                        , leaveNontaxHr
                                                                        , HourlyRate
                                                                        , DailyRate);

                                    drEmpLeaveDtl["Tld_LeaveNontaxableHr"] = leaveNontaxHr;
                                    drEmpLeaveDtl["Tld_LeaveNontaxableAmt"] = Math.Round(leaveNontaxAmt, 2);

                                }

                                if (!EncashmentTaxableRule.Equals("N"))
                                {
                                    leaveTaxHr = ComputeLeaveCredits(leaveHr
                                                                                            , EncashmentTaxableRule
                                                                                            , EncashmentTaxableCredit);

                                    leaveTaxAmt = ComputeLeaveAmount(EncashmentBasis
                                                                    , Convert.ToDecimal(EncashmentRate)
                                                                    , leaveTaxHr
                                                                    , HourlyRate
                                                                    , DailyRate);

                                    drEmpLeaveDtl["Tld_LeaveTaxableHr"] = leaveTaxHr;
                                    drEmpLeaveDtl["Tld_LeaveTaxableAmt"] = Math.Round(leaveTaxAmt, 2);

                                    #region Force Balance Taxable Amount
                                    decimal tmpLeaveAmt         = Convert.ToDecimal(drEmpLeaveDtl["Tld_LeaveAmt"]);
                                    decimal tmpNontaxableAmt    = Convert.ToDecimal(drEmpLeaveDtl["Tld_LeaveNontaxableAmt"]);
                                    decimal tmpTaxableAmt       = Convert.ToDecimal(drEmpLeaveDtl["Tld_LeaveTaxableAmt"]);
                                    decimal dVariance           = 0;
                                    if ((tmpLeaveAmt - tmpNontaxableAmt) != tmpTaxableAmt)
                                    {
                                        decimal tmpLeaveAmtMinusNontax = (tmpLeaveAmt - tmpNontaxableAmt);
                                        if (tmpLeaveAmtMinusNontax > tmpTaxableAmt)
                                        {
                                            dVariance = tmpLeaveAmtMinusNontax - tmpTaxableAmt;
                                            if (dVariance <= 1)
                                                drEmpLeaveDtl["Tld_LeaveTaxableAmt"] = Math.Round(tmpLeaveAmtMinusNontax, 2);
                                            //else
                                            //    AddErrorToLaborCostReport(dt.Rows[0][dt.Columns[0].ColumnName].ToString(), "AE", dt.Rows[0][dt.Columns[1].ColumnName].ToString(), 0, 0, "With Variance");
                                        }
                                        else if (tmpLeaveAmtMinusNontax < tmpTaxableAmt)
                                        {
                                            dVariance = tmpTaxableAmt - tmpLeaveAmtMinusNontax;
                                            if (dVariance <= 1)
                                                drEmpLeaveDtl["Tld_LeaveTaxableAmt"] = Math.Round(tmpLeaveAmtMinusNontax, 2);
                                        }
                                    }
                                    #endregion
                                }

                                if (!ProcessSeparated)
                                {
                                    if (!CarryForwardRule.Equals("N"))
                                    {
                                        dConvertedCredit = dLeaveBalance;
                                        dCarryForwardCredit = ComputeLeaveCredits(dLeaveBalance
                                                                                   , CarryForwardRule
                                                                                   , CarryForwardCredit);

                                        drEmpLeaveDtl["Tld_CarryForwardCreditNextYear"] = dCarryForwardCredit;
                                    }
                                }

                                if (leaveAmt != 0)
                                    InsertDtl(drEmpLeaveDtl, dal);

                                ctr++;
                                EmpDispHandler(this, new EmpDispEventArgs(curEmployeeId, dtLeaveLdgEmployees.Rows[i]["Mem_LastName"].ToString(), dtLeaveLdgEmployees.Rows[i]["Mem_FirstName"].ToString()));
                            }
                            #endregion
                        }
                    }

                    #region Update T_PaySchedule.Tls_LeaveRefundPickUp
                    if (!ProcessSeparated)
                    {
                        if (dtRule != null && dtRule.Rows.Count > 0)
                        {
                            for (int x = 0; x < dtRule.Rows.Count; x++)
                            {
                                dal.ExecuteNonQuery(string.Format(@"UPDATE T_LeaveSchedule 
										                    SET Tls_LeaveRefundPickUp = 0 
										                    WHERE Tls_LeaveYear = '{0}'
                                                                AND Tls_LeaveCode = '{1}'
                                                                AND Tls_CycleIndicator = 'C'"
                                                                    , dtRule.Rows[x]["Tlr_LeaveYear"]
                                                                    , dtRule.Rows[x]["Tlr_LeaveCode"]));
                            }

                        }

                        dal.ExecuteNonQuery(string.Format(@"UPDATE T_LeaveSchedule 
										            SET Tls_LeaveRefundPickUp = 1 
										            FROM  T_LeaveSchedule 
										            INNER JOIN T_EmpLeaveRefundRule 
										            ON Tlr_LeaveYear = Tls_LeaveYear
										            AND Tlr_LeaveCode = Tls_LeaveCode
										            AND Tls_CycleIndicator = 'C'
										            WHERE Tlr_ControlNo = '{0}'", ControlNumber));
                    }

                    #endregion
                }
                else
                    throw new Exception("No Leave Code | Leave Year to process.");
            }
            catch (Exception ex)
            {
                CommonProcedures.logErrorToFile(ex.ToString());
                throw new Exception("Leave Refund Calculation has encountered some errors: \n" + ex.Message);
            }
            return ControlNumber;
        }

        public void SetProcessFlags()
        {
            string strResult = string.Empty;
            //string LastPayProcessingErrorMessage = "";

            #region Parameters
            StatusHandler(this, new StatusEventArgs("Loading Parameters", false));
            StatusHandler(this, new StatusEventArgs("Loading Parameters", true));

            strResult = commonBL.GetParameterValueFromPayroll("MDIVISOR", CompanyCode, dal);
            MDIVISOR = strResult.Equals(string.Empty) ? 0 : Convert.ToDecimal(strResult);
            StatusHandler(this, new StatusEventArgs(string.Format("  MDIVISOR = {0}", MDIVISOR), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  MDIVISOR = {0}", MDIVISOR), true));

            strResult = commonBL.GetParameterValueFromPayroll("HRLYRTEDEC", CompanyCode, dal);
            HRLYRTEDEC = strResult.Equals(string.Empty) ? 0 : Convert.ToDecimal(strResult);
            StatusHandler(this, new StatusEventArgs(string.Format("  HRLYRTEDEC = {0}", HRLYRTEDEC), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  HRLYRTEDEC = {0}", HRLYRTEDEC), true));

            strResult = commonBL.GetParameterValueFromPayroll("LVHRENTRY", CompanyCode, dal);
            LVHRENTRY = strResult.Equals(string.Empty) ? false : Convert.ToBoolean(strResult);
            StatusHandler(this, new StatusEventArgs(string.Format("  LVHRENTRY = {0}", LVHRENTRY), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  LVHRENTRY = {0}", LVHRENTRY), true));

            FPLVREFUNDDIVISOR = commonBL.GetParameterFormulaFromPayroll("FPLVREFUNDDIVISOR", CompanyCode, dal);
            StatusHandler(this, new StatusEventArgs(string.Format("  FPLVREFUNDDIVISOR = {0}", FPLVREFUNDDIVISOR), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  FPLVREFUNDDIVISOR = {0}", FPLVREFUNDDIVISOR), true));

            FPLVREFUNDDIVIDEND = commonBL.GetParameterFormulaFromPayroll("FPLVREFUNDDIVIDEND", CompanyCode, dal);
            StatusHandler(this, new StatusEventArgs(string.Format("  FPLVREFUNDDIVIDEND = {0}", FPLVREFUNDDIVIDEND), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  FPLVREFUNDDIVIDEND = {0}", FPLVREFUNDDIVIDEND), true));

            FPLVREFUNDRULE = commonBL.GetParameterValueFromPayroll("FPLVREFUNDRULE", CompanyCode, dal);
            StatusHandler(this, new StatusEventArgs(string.Format("  FPLVREFUNDRULE = {0}", FPLVREFUNDRULE), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  FPLVREFUNDRULE = {0}", FPLVREFUNDRULE), true));

            FPLVREFUNDFORMULA = commonBL.GetParameterFormulaFromPayroll("FPLVREFUNDFORMULA", CompanyCode, dal);
            StatusHandler(this, new StatusEventArgs(string.Format("  FPLVREFUNDFORMULA = {0}", FPLVREFUNDFORMULA), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  FPLVREFUNDFORMULA = {0}", FPLVREFUNDFORMULA), true));

            dtLeaveMaster = GetLeaveMasterDetails(CompanyCode, CentralProfile);
            #endregion
        }

        public DataTable GetLeaveMasterDetails(string CompanyCode, string CentralProfile)
        {
            string query = string.Format(@"SELECT Mlv_CompanyCode
                                            ,Mlv_LeaveCode
                                            ,Mlv_LeaveDescription
                                            ,Mlv_IsPaidLeave
                                            ,Mlv_ParentLeave
                                            ,Mlv_IsCombineLeave
                                            ,Mlv_WithCredit
                                            ,Mlv_WithCategory
                                            ,Mlv_DayUnit
                                            ,Mlv_MinHoursToFile
                                            ,Mlv_IncrementToFile
                                            ,Mlv_DaysPriorToNotify
                                            ,Mlv_DayCode
                                            ,Mlv_UnpaidLeaveCode
                                            ,Mlv_Gender
                                            ,Mlv_FileWithinShift
                                            ,Mlv_ConvertibleToCash
                                            ,Mlv_EncashmentBasis
                                            ,Mlv_EncashmentRate
                                            ,Mlv_EncashmentRule
                                            ,Mlv_EncashmentCredit
                                            ,Mlv_EncashmentNontaxableRule
                                            ,Mlv_EncashmentNontaxableCredit
                                            ,Mlv_EncashmentTaxableRule
                                            ,Mlv_EncashmentTaxableCredit
                                            ,Mlv_CreditSysGen
                                            ,Mlv_StartLeaveYear
                                            ,Mlv_EndLeaveYear
                                            ,Mlv_CreditTiming
                                            ,Mlv_CreditReferenceDate
                                            ,Mlv_CarryForwardRule
                                            ,Mlv_CarryForwardCredit
                                            ,Mlv_CarryForwardLeaveCode
                                            ,Mlv_RegularCreditType
                                        FROM {1}..M_Leave
                                        WHERE Mlv_RecordStatus = 'A'
                                            AND Mlv_CompanyCode = '{0}'", CompanyCode, CentralProfile);
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        private DataTable GetConvertibleToCashLeaveType(bool bProcessSeparated, string SeparationYear)
        {
            string condition = "WHERE Tls_CycleIndicator = 'C'";
            if (bProcessSeparated)
                condition = string.Format("WHERE Tls_LeaveYear = '{0}' ", SeparationYear);

            string query = string.Format(@"SELECT Tls_LeaveYear + Tls_LeaveCode AS [Leave Year - Type]
                                        FROM T_LeaveSchedule
                                        INNER JOIN {0}..M_Leave ON Mlv_LeaveCode = Tls_LeaveCode
                                            AND Mlv_ConvertibleToCash = 1
                                            AND Mlv_CompanyCode = '{1}'
                                            {2}
                                            ", CentralProfile, CompanyCode, condition);
            
            DataTable dt = dal.ExecuteDataSet(query).Tables[0];
            return dt;
        }

        public DataTable GetEmployeeMasterList()
        {
            #region query
            string query = string.Format(@"SELECT Mem_IDNo, Mem_WorkStatus
                                           FROM M_Employee");
            #endregion
            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetCostCenters(string CompanyCode, string CentralProfile)
        {
            string query = string.Format(@"SELECT Mcc_CostCenterCode as 'Cost Center Code'
                                                , dbo.Udf_DisplayCostCenterName('{0}', Mcc_CostCenterCode,'{1}') as 'Description'
                                            FROM M_CostCenter
                                            WHERE Mcc_RecordStatus = 'A'
                                                AND Mcc_CompanyCode = '{0}'"
                                            , CompanyCode, (new CommonBL()).GetParameterValueFromCentral("CCTRDSPLY", CompanyCode, CentralProfile));

            DataTable dtResult;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetAccountCode(string AccountType, string strMenuCode, string CompanyCode, string CentralProfile)
        {
            string query = string.Format(@"SELECT Mcd_Code AS 'Code'
                                               , Mcd_Name AS 'Description'
                                          FROM {2}..M_CodeDtl
                                          WHERE Mcd_RecordStatus = 'A'
                                                AND Mcd_CodeType = '{0}' 
                                                AND Mcd_CompanyCode = '{1}'
                                                AND Mcd_Code IN (SELECT Mpd_SubCode 
                                                                        FROM M_PolicyDtl 
				                                                        WHERE Mpd_PolicyCode = 'EMPSTATPAY'
				                                                        AND Mpd_ParamValue = 1)", AccountType, CompanyCode, CentralProfile);

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }


        public decimal ComputeLeaveCredits(decimal LeaveBalance
                                                , string Rule
                                                , string RuleCredit)
        {
            decimal dLeaveCredit = 0;
            if (Rule.Equals("A")) //All
                dLeaveCredit = LeaveBalance;
            else if (Rule.Equals("E")) //Excess
            {
                if (LeaveBalance >= Convert.ToDecimal(RuleCredit))
                    dLeaveCredit = LeaveBalance - Convert.ToDecimal(RuleCredit);
                else
                    dLeaveCredit = 0;
            }
                
            else if (Rule.Equals("S")) //Specific
            {
                if (LeaveBalance >= Convert.ToDecimal(RuleCredit))
                    dLeaveCredit = Convert.ToDecimal(RuleCredit);
                else
                    dLeaveCredit = LeaveBalance;
            }
                
            else if (Rule.Equals("P")) //Percentage
            {
                if (Convert.ToDecimal(RuleCredit) > 0)
                    dLeaveCredit = LeaveBalance * (Convert.ToDecimal(RuleCredit) / 100);
                else
                    dLeaveCredit = 0;
            }
            return dLeaveCredit;
        }


        public decimal ComputeLeaveAmount(string EncashmentBasis, decimal EncashmentRate, decimal Hours, decimal HourlyRate, decimal DailyRate)
        {
            decimal LeaveAmount = 0;

            if (EncashmentBasis == "S")
            {
                if (LVHRENTRY)
                    LeaveAmount = Hours * HourlyRate;
                else
                    LeaveAmount = (Hours / CommonBL.HOURSINDAY) * DailyRate;
            }
            else if (EncashmentBasis == "F")
            {
                LeaveAmount = Hours * EncashmentRate;
            }

            return LeaveAmount;
        }

        public int InsertHdr(DataRow row, DALHelper dal)
        {
            int retVal = 0;
            int paramIndex = 0;
            ParameterInfo[] paramInfo = new ParameterInfo[13];
            paramInfo[paramIndex++] = new ParameterInfo("@Tlh_ControlNo", row["Tlh_ControlNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlh_PayCycle", row["Tlh_PayCycle"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlh_IDNoLIst", row["Tlh_IDNoLIst"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlh_CostCenterList", row["Tlh_CostCenterList"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlh_PayrollGroupList", row["Tlh_PayrollGroupList"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlh_EmploymentStatusList", row["Tlh_EmploymentStatusList"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlh_PayrollTypeList", row["Tlh_PayrollTypeList"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlh_PositionList", row["Tlh_PositionList"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlh_GradeList", row["Tlh_GradeList"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlh_HireRegOption", row["Tlh_HireRegOption"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlh_HireRegDate", row["Tlh_HireRegDate"], SqlDbType.Date);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlh_RecordStatus", row["Tlh_RecordStatus"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);


            #region Insert Query
            string sqlInsert = @"INSERT INTO T_EmpLeaveRefundHdr
                                            (Tlh_ControlNo
                                            ,Tlh_PayCycle
                                            ,Tlh_IDNoLIst
                                            ,Tlh_CostCenterList
                                            ,Tlh_PayrollGroupList
                                            ,Tlh_EmploymentStatusList
                                            ,Tlh_PayrollTypeList
                                            ,Tlh_PositionList
                                            ,Tlh_GradeList
                                            ,Tlh_HireRegOption
                                            ,Tlh_HireRegDate
                                            ,Tlh_RecordStatus
                                            ,Usr_Login
                                            ,Ludatetime)

                                            VALUES(@Tlh_ControlNo
                                            , @Tlh_PayCycle
                                            , @Tlh_IDNoLIst
                                            , @Tlh_CostCenterList
                                            , @Tlh_PayrollGroupList
                                            , @Tlh_EmploymentStatusList
                                            , @Tlh_PayrollTypeList
                                            , @Tlh_PositionList
                                            , @Tlh_GradeList
                                            , @Tlh_HireRegOption
                                            , @Tlh_HireRegDate
                                            , @Tlh_RecordStatus
                                            , @Usr_Login
                                            , GETDATE())";
            #endregion

            try
            {
                //insert
                retVal = dal.ExecuteNonQuery(sqlInsert, CommandType.Text, paramInfo);
            }
            catch (Exception e)
            {

                throw new PayrollException(e);
            }
            return retVal;
        }

        public int InsertDtl(DataRow row, DALHelper dal)
        {
            int retVal = 0;
            int paramIndex = 0;
            ParameterInfo[] paramInfo = new ParameterInfo[26];
            paramInfo[paramIndex++] = new ParameterInfo("@Tld_ControlNo", row["Tld_ControlNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tld_LineNo", row["Tld_LineNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tld_IDNo", row["Tld_IDNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tld_LeaveCode", row["Tld_LeaveCode"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tld_LeaveHr", row["Tld_LeaveHr"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tld_LeaveAmt", row["Tld_LeaveAmt"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tld_LeaveNontaxableHr", row["Tld_LeaveNontaxableHr"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tld_LeaveNontaxableAmt", row["Tld_LeaveNontaxableAmt"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tld_LeaveTaxableHr", row["Tld_LeaveTaxableHr"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tld_LeaveTaxableAmt", row["Tld_LeaveTaxableAmt"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tld_SalaryRate", row["Tld_SalaryRate"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tld_HourlyDailyRate", row["Tld_HourlyDailyRate"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tld_PostFlag", row["Tld_PostFlag"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tld_BegCredit", row["Tld_BegCredit"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tld_CarryForwardCredit", row["Tld_CarryForwardCredit"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tld_UsedCredit", row["Tld_UsedCredit"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tld_DividendValue", row["Tld_DividendValue"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tld_DivisorValue", row["Tld_DivisorValue"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tld_CarryForwardCreditNextYear", row["Tld_CarryForwardCreditNextYear"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tld_CostCenter", row["Tld_CostCenter"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tld_PayrollGroup", row["Tld_PayrollGroup"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tld_EmploymentStatus", row["Tld_EmploymentStatus"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tld_PayrollType", row["Tld_PayrollType"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tld_Position", row["Tld_Position"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tld_Grade", row["Tld_Grade"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);

            #region Insert Query
            string sqlInsert = @"INSERT INTO T_EmpLeaveRefundDtl
                                            (Tld_ControlNo
                                            ,Tld_LineNo
                                            ,Tld_IDNo
                                            ,Tld_LeaveCode
                                            ,Tld_LeaveHr
                                            ,Tld_LeaveAmt
                                            ,Tld_LeaveNontaxableHr
                                            ,Tld_LeaveNontaxableAmt
                                            ,Tld_LeaveTaxableHr
                                            ,Tld_LeaveTaxableAmt
                                            ,Tld_SalaryRate
                                            ,Tld_HourlyDailyRate
                                            ,Tld_PostFlag
                                            ,Tld_BegCredit
                                            ,Tld_CarryForwardCredit
                                            ,Tld_UsedCredit
                                            ,Tld_DividendValue
                                            ,Tld_DivisorValue
                                            ,Tld_CarryForwardCreditNextYear
                                            ,Tld_CostCenter
                                            ,Tld_PayrollGroup
                                            ,Tld_EmploymentStatus
                                            ,Tld_PayrollType
                                            ,Tld_Position
                                            ,Tld_Grade
                                            ,Usr_Login
                                            ,Ludatetime)
                                            VALUES
                                            (@Tld_ControlNo
                                            ,@Tld_LineNo
                                            ,@Tld_IDNo
                                            ,@Tld_LeaveCode
                                            ,@Tld_LeaveHr
                                            ,@Tld_LeaveAmt
                                            ,@Tld_LeaveNontaxableHr
                                            ,@Tld_LeaveNontaxableAmt
                                            ,@Tld_LeaveTaxableHr
                                            ,@Tld_LeaveTaxableAmt
                                            ,@Tld_SalaryRate
                                            ,@Tld_HourlyDailyRate
                                            ,@Tld_PostFlag
                                            ,@Tld_BegCredit
                                            ,@Tld_CarryForwardCredit
                                            ,@Tld_UsedCredit
                                            ,@Tld_DividendValue
                                            ,@Tld_DivisorValue
                                            ,@Tld_CarryForwardCreditNextYear
                                            ,@Tld_CostCenter
                                            ,@Tld_PayrollGroup
                                            ,@Tld_EmploymentStatus
                                            ,@Tld_PayrollType
                                            ,@Tld_Position
                                            ,@Tld_Grade
                                            ,@Usr_Login
                                            ,GETDATE())";
            #endregion

            try
            {
                //insert
                retVal = dal.ExecuteNonQuery(sqlInsert, CommandType.Text, paramInfo);
            }
            catch (Exception e)
            {

                throw new PayrollException(e);
            }
            return retVal;
        }

        public int InsertLeaveRule(DataRow row, DALHelper dal)
        {
            int retVal = 0;
            int paramIndex = 0;
            ParameterInfo[] paramInfo = new ParameterInfo[18];
            paramInfo[paramIndex++] = new ParameterInfo("@Tlr_ControlNo", row["Tlr_ControlNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlr_LineNo", row["Tlr_LineNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlr_LeaveYear", row["Tlr_LeaveYear"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlr_LeaveCode", row["Tlr_LeaveCode"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlr_EncashmentBasis", row["Tlr_EncashmentBasis"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlr_EncashmentRate", row["Tlr_EncashmentRate"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlr_SalaryDate", (GetValue(row["Tlr_SalaryDate"]) != string.Empty ? row["Tlr_SalaryDate"] : DBNull.Value), SqlDbType.Date);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlr_EncashmentRule", row["Tlr_EncashmentRule"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlr_EncashmentCredit", row["Tlr_EncashmentCredit"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlr_EncashmentNontaxableRule", row["Tlr_EncashmentNontaxableRule"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlr_EncashmentNontaxableCredit", row["Tlr_EncashmentNontaxableCredit"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlr_EncashmentTaxableRule", row["Tlr_EncashmentTaxableRule"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlr_EncashmentTaxableCredit", row["Tlr_EncashmentTaxableCredit"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlr_CarryForwardRule", row["Tlr_CarryForwardRule"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlr_CarryForwardCredit", row["Tlr_CarryForwardCredit"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlr_CarryForwardPickUp", row["Tlr_CarryForwardPickUp"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlr_CarryForwardLeaveCode", row["Tlr_CarryForwardLeaveCode"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);

            #region Insert Query
            string sqlInsert = @"INSERT INTO T_EmpLeaveRefundRule
                                            (Tlr_ControlNo
                                            ,Tlr_LineNo
                                            ,Tlr_LeaveYear
                                            ,Tlr_LeaveCode
                                            ,Tlr_EncashmentBasis
                                            ,Tlr_EncashmentRate
                                            ,Tlr_SalaryDate
                                            ,Tlr_EncashmentRule
                                            ,Tlr_EncashmentCredit
                                            ,Tlr_EncashmentNontaxableRule
                                            ,Tlr_EncashmentNontaxableCredit
                                            ,Tlr_EncashmentTaxableRule
                                            ,Tlr_EncashmentTaxableCredit
                                            ,Tlr_CarryForwardRule
                                            ,Tlr_CarryForwardCredit
                                            ,Tlr_CarryForwardPickUp
                                            ,Tlr_CarryForwardLeaveCode
                                            ,Usr_Login
                                            ,Ludatetime)
                                            VALUES
                                            (@Tlr_ControlNo
                                            ,@Tlr_LineNo
                                            ,@Tlr_LeaveYear
                                            ,@Tlr_LeaveCode
                                            ,@Tlr_EncashmentBasis
                                            ,@Tlr_EncashmentRate
                                            ,@Tlr_SalaryDate
                                            ,@Tlr_EncashmentRule
                                            ,@Tlr_EncashmentCredit
                                            ,@Tlr_EncashmentNontaxableRule
                                            ,@Tlr_EncashmentNontaxableCredit
                                            ,@Tlr_EncashmentTaxableRule
                                            ,@Tlr_EncashmentTaxableCredit
                                            ,@Tlr_CarryForwardRule
                                            ,@Tlr_CarryForwardCredit
                                            ,@Tlr_CarryForwardPickUp
                                            ,@Tlr_CarryForwardLeaveCode
                                            ,@Usr_Login
                                            ,GETDATE())";
            #endregion

            try
            {
                //insert
                retVal = dal.ExecuteNonQuery(sqlInsert, CommandType.Text, paramInfo);
            }
            catch (Exception e)
            {

                throw new PayrollException(e);
            }
            return retVal;
        }

        public int UpdateHdr(DataRow row, DALHelper dal)
        {
            int retVal = 0;
            int paramIndex = 0;
            ParameterInfo[] paramInfo = new ParameterInfo[13];
            paramInfo[paramIndex++] = new ParameterInfo("@Tlh_ControlNo", row["Tlh_ControlNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlh_PayCycle", row["Tlh_PayCycle"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlh_IDNoLIst", row["Tlh_IDNoLIst"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlh_CostCenterList", row["Tlh_CostCenterList"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlh_PayrollGroupList", row["Tlh_PayrollGroupList"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlh_EmploymentStatusList", row["Tlh_EmploymentStatusList"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlh_PayrollTypeList", row["Tlh_PayrollTypeList"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlh_PositionList", row["Tlh_PositionList"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlh_GradeList", row["Tlh_GradeList"], SqlDbType.NVarChar, 4000);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlh_HireRegOption", row["Tlh_HireRegOption"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlh_HireRegDate", row["Tlh_HireRegDate"], SqlDbType.Date);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlh_RecordStatus", row["Tlh_RecordStatus"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);

            #region Update Query
            string sqlUpdate = @"UPDATE T_EmpLeaveRefundHdr
                                SET Tlh_IDNoLIst = @Tlh_IDNoLIst
                                    , Tlh_CostCenterList = @Tlh_CostCenterList
                                    , Tlh_PayrollGroupList = @Tlh_PayrollGroupList
                                    , Tlh_EmploymentStatusList = @Tlh_EmploymentStatusList
                                    , Tlh_PayrollTypeList = @Tlh_PayrollTypeList
                                    , Tlh_PositionList = @Tlh_PositionList
                                    , Tlh_GradeList = @Tlh_GradeList
                                    , Tlh_HireRegOption = @Tlh_HireRegOption
                                    , Tlh_HireRegDate = @Tlh_HireRegDate
                                    , Tlh_RecordStatus = @Tlh_RecordStatus
                                    , Usr_Login = @Usr_Login
                                    , Ludatetime = GETDATE()
                                WHERE Tlh_ControlNo = @Tlh_ControlNo
                                AND Tlh_PayCycle = @Tlh_PayCycle";
            #endregion

            try
            {
                retVal = dal.ExecuteNonQuery(sqlUpdate, CommandType.Text, paramInfo);
            }
            catch (Exception e)
            {

                throw new PayrollException(e);
            }
            return retVal;
        }

        public int GetSeqNo(string ControlNumber, DALHelper dal)
        {
            try
            {
                DataTable dt = new DataTable();
                ParameterInfo[] paramInfo = new ParameterInfo[1];
                paramInfo[0] = new ParameterInfo("@ControlNumber", ControlNumber);

                string sqlQuery = @"SELECT Count(Tlr_ControlNo) AS [Count] FROM T_EmpLeaveRefundRule
                                    WHERE Tlr_ControlNo = @ControlNumber";

                dt = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo).Tables[0];
                return Convert.ToInt32(dt.Rows[0][0]);
            }
            catch
            { return 0; }

        }

        public int GetSeqNoDtl(string ControlNumber, DALHelper dal)
        {
            try
            {
                DataTable dt = new DataTable();
                ParameterInfo[] paramInfo = new ParameterInfo[1];
                paramInfo[0] = new ParameterInfo("@ControlNumber", ControlNumber);

                string sqlQuery = @"SELECT MAX(Tld_LineNo) AS [Count] 
                                    FROM T_EmpLeaveRefundDtl
                                    WHERE Tld_ControlNo = @ControlNumber";

                dt = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo).Tables[0];
                return Convert.ToInt32(dt.Rows[0][0]);
            }
            catch
            { return 0; }

        }

        public void DeleteLeave(string ControlNo, string LineNo)
        {
            DataSet ds = new DataSet();
            string sqlQuery = string.Format(@"DELETE FROM T_EmpLeaveRefundDtl
                                              WHERE Tld_ControlNo = '{0}'
                                                AND Tld_LineNo = '{1}'

                                            IF NOT EXISTS (SELECT Tld_LineNo FROM T_EmpLeaveRefundDtl WHERE Tld_ControlNo = '{0}' )
                                            BEGIN
	                                            UPDATE T_EmpLeaveRefundHdr SET Tlh_RecordStatus = 'C'
	                                            WHERE Tlh_ControlNo = '{0}'
                                            END", ControlNo, LineNo);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.ExecuteNonQuery(sqlQuery);
                dal.CloseDB();
            }
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

        public string GetFormulaQueryStringValue(string query, ParameterInfo[] paramInfo)
        {
            if (query == string.Empty)
                return string.Empty;

            string sValue = string.Empty;
            DataTable dtResult = dal.ExecuteDataSet(query, CommandType.Text, paramInfo).Tables[0];
            if (dtResult.Rows.Count > 0)
            {
                sValue = GetValue(dtResult.Rows[0][0]);
            }
            return sValue;
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

        public int PostToIncome(bool ProcessSeparated, string bPayCycle, string IDNumberList, string ControlNo, string userLogin, string CompanyCode, string CentralProfile, DALHelper dalhelper)
        {
            int retVal = 0;
            string tableIncome = CommonConstants.TableName.T_EmpIncome;
            string NontaxCode = "NONTAX";
            string TaxableCode = "TAX";
            string Condition = string.Empty;
            if (ProcessSeparated)
            {
                tableIncome = CommonConstants.TableName.T_EmpIncomeFinalPay;
                NontaxCode = "NONTAXFP";
                TaxableCode = "TAXFP";
                Condition = string.Format("AND [@FIELDNAME] IN ({0})", IDNumberList);
            }

            #region Leave Refund
            string query = string.Format(@"INSERT INTO {4}..M_Income
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
                                                    --TAXABLE
                                                    SELECT '{3}'
                                                        , NewAllowCode
	                                                    ,'LEAVE REFUND (' + NewAllowCode + ')'
                                                        ,'T'
                                                        ,0
                                                        ,0
                                                        ,'LVR'
                                                        ,'T-OTHERTAX1'
                                                        , 1
                                                        ,''
                                                        ,''
														,'A'
                                                        ,'{2}'
                                                        ,GETDATE()
                                                    FROM 
                                                    (
	                                                    SELECT DISTINCT Tld_LeaveCode+[Name] AS NewAllowCode, [Class]
                                                        FROM T_EmpLeaveRefundDtl
                                                        LEFT JOIN (SELECT 'N' AS [Class],'{7}' AS [Name]
                                                                UNION
		                                                        SELECT 'T' AS [Class],'{6}' AS [Name] ) taxclass ON 1=1
	                                                    WHERE [Class] = 'T'
                                                    ) LEAVETYPE
                                                    LEFT JOIN {4}..M_Income
                                                    ON NewAllowCode = Min_IncomeCode
                                                        AND Min_CompanyCode = '{3}'
                                                    WHERE Min_IncomeCode IS NULL
                                                        

                                                    UNION

                                                    --NONTAXABLE
                                                    SELECT '{3}'
                                                        ,NewAllowCode
	                                                    ,'LEAVE REFUND (' + NewAllowCode + ')'
                                                        ,'N'
                                                        ,0
                                                        ,0
                                                        ,'LVR'
                                                        ,'N-SALOTH'
                                                        , 1
                                                        ,''
                                                        ,''
														,'A'
                                                        ,'{2}'
                                                        ,GETDATE()
                                                    FROM 
                                                    (
	                                                    SELECT DISTINCT Tld_LeaveCode+[Name] AS NewAllowCode, [Class]
                                                        FROM T_EmpLeaveRefundDtl
                                                        LEFT JOIN (SELECT 'N' AS [Class],'{7}' AS [Name]
                                                                UNION
		                                                            SELECT 'T' AS [Class],'{6}' AS [Name] ) taxclass ON 1=1
	                                                    WHERE [Class] = 'N'
                                                    ) LEAVETYPE
                                                    LEFT JOIN {4}..M_Income
                                                    ON NewAllowCode = Min_IncomeCode
                                                        AND Min_CompanyCode = '{3}'
                                                    WHERE Min_IncomeCode IS NULL
                                                        

                                                    DELETE FROM {5}
                                                    WHERE Tin_IncomeCode IN (SELECT DISTINCT Tld_LeaveCode+[Name] AS [Code]
                                                                              FROM T_EmpLeaveRefundDtl
                                                                              LEFT JOIN (SELECT 'N' AS [Class],'{7}' AS [Name]
				                                                                        UNION
				                                                                        SELECT 'T' AS [Class],'{6}' AS [Name] ) taxclass ON 1=1) 
                                                        AND Tin_PayCycle = '{0}'
                                                        AND Tin_OrigPayCycle = '{0}'
                                                        {9}
                                    

                                                    INSERT INTO {5}
                                                               (Tin_IDNo
                                                               ,Tin_PayCycle
                                                               ,Tin_OrigPayCycle
                                                               ,Tin_IncomeCode
                                                               ,Tin_PostFlag
                                                               ,Tin_IncomeAmt
                                                               ,Usr_Login
                                                               ,Ludatetime)
                                                        SELECT Tld_IDNo
                                                          ,'{0}'
                                                          ,'{0}'
                                                          ,Tld_LeaveCode + '{7}'
                                                          ,0
                                                          ,Tld_LeaveNontaxableAmt
                                                          ,'{2}'
                                                          ,GETDATE()
                                                      FROM T_EmpLeaveRefundDtl
                                                      WHERE Tld_LeaveNontaxableAmt != 0
                                                        AND Tld_ControlNo = '{1}'
                                                        {10}

                                                    INSERT INTO {5}
                                                               (Tin_IDNo
                                                               ,Tin_PayCycle
                                                               ,Tin_OrigPayCycle
                                                               ,Tin_IncomeCode
                                                               ,Tin_PostFlag
                                                               ,Tin_IncomeAmt
                                                               ,Usr_Login
                                                               ,Ludatetime)
                                                    SELECT Tld_IDNo
                                                          ,'{0}'
                                                          ,'{0}'
                                                          ,Tld_LeaveCode + '{6}'
                                                          ,0
                                                          ,Tld_LeaveTaxableAmt
                                                          ,'{2}'
                                                          ,GETDATE()
                                                      FROM T_EmpLeaveRefundDtl
                                                      WHERE Tld_LeaveTaxableAmt != 0
                                                        AND Tld_ControlNo = '{1}'
                                                        {10}
                                                      
                                                      UPDATE T_EmpLeaveRefundDtl
                                                      SET Tld_PostFlag = 1
                                                        , Usr_login = '{2}'
                                                        , Ludatetime = GETDATE()
                                                      WHERE (Tld_LeaveNontaxableAmt != 0 OR Tld_LeaveTaxableAmt != 0)
                                                        AND Tld_ControlNo = '{1}'
                                                        {10}

                                                      UPDATE T_EmpLeaveRefundDtl
                                                      SET Tld_PostFlag = 0
                                                        , Usr_login = '{2}'
                                                        , Ludatetime = GETDATE()
                                                      WHERE (Tld_LeaveNontaxableAmt = 0 AND Tld_LeaveTaxableAmt = 0)
                                                        AND Tld_ControlNo = '{1}'
                                                        {10}

                                                   ", bPayCycle
                                                   , ControlNo
                                                   , userLogin
                                                   , CompanyCode
                                                   , CentralProfile
                                                   , tableIncome
                                                   , TaxableCode
                                                   , NontaxCode
                                                   , (ProcessSeparated ? "TRUE" : "FALSE")
                                                   , Condition.Replace("@FIELDNAME", "Tin_IDNo")
                                                   , Condition.Replace("@FIELDNAME", "Tld_IDNo"));
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


        public DataTable GetOnRouteLeaveTransactions(string LeaveYearType, string CompanyCode, string CentralProfile)
        {
            string query = string.Format(@"
                                  SELECT Tlv_IDNo as [ID Number]							
	                                    , Mem_Lastname						
	                                    , Mem_Firstname						
	                                    , CONVERT(CHAR(10), Tlv_LeaveDate, 101) as [Leave Date]						
	                                    , Tlv_LeaveCode as [Leave Type]						
	                                    , LEFT(Tlv_StartTime, 2) + ':' + RIGHT(Tlv_StartTime, 2) as [Start Time]						
	                                    , LEFT(Tlv_EndTime, 2) + ':' + RIGHT(Tlv_EndTime, 2)  as [End Time]						
	                                    , Tlv_ReasonForRequest as [Reason for Leave]						
                                    FROM  T_EmpLeave							
                                    INNER JOIN M_Employee ON Mem_IDNo = Tlv_IDNo							
                                    INNER JOIN {0}..M_Leave ON Mlv_LeaveCode = Tlv_LeaveCode							
	                                    AND Mlv_CompanyCode = '{1}'						
	                                    AND Mlv_EncashmentRule <> 'N'						
                                    INNER JOIN T_LeaveSchedule ON Tls_LeaveCode = Tlv_LeaveCode							
	                                    AND Tlv_Leavedate between Tls_StartCycle AND Tls_EndCycle						
	                                    AND Tls_CycleIndicator='C'						
                                    WHERE Tlv_LeaveStatus IN ('04','06','08', '10','12' ,'22','23','24','25')
                                        AND Tlv_LeaveCode IN ({2})							
                                    ORDER BY Mem_LastName, Mem_FirstName, Tlv_LeaveDate "
                                           , CentralProfile
                                           , CompanyCode
                                           , EncodeLeaveYearTypeFilterItems(LeaveYearType));

            DataTable dt = null;
            using (DALHelper dalhelper = new DALHelper())
            {
                dalhelper.OpenDB();
                dt = dalhelper.ExecuteDataSet(query).Tables[0];
                dalhelper.CloseDB();
            }
            return dt;

        }

        public string EncodeLeaveYearTypeFilterItems(string strDelimited)
        {
            string[] strArrFilterItems = strDelimited.Split(new char[] { ',' });
            string strFilterItem = "";
            foreach (string col in strArrFilterItems)
            {
                strFilterItem += "'" + col.Substring(4, col.Length - 4).Trim() + "',";
            }
            if (strFilterItem != "")
                strFilterItem = strFilterItem.Substring(0, strFilterItem.Length - 1);
            return strFilterItem;
        }
    }
}
