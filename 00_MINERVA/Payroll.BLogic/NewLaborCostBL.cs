using System;
using System.Collections.Generic;
using System.Data;
using CommonLibrary;
using Payroll.DAL;
using System.Configuration;
using System.Text.RegularExpressions;

namespace Payroll.BLogic
{
    public class NewLaborCostBL : BaseBL
    {
        #region <Global Variables>
        DALHelper dal;
        CommonBL commonBL;

        //string CycleType = "";
        string ProfileCode = "";
        string LoginUser = "";
        string PayCycleCode = "";
        string CompanyCode = "";
        string CentralProfile = "";
        string MenuCode = "";
        DateTime PostDate;

        //Parameters Labor Cost
        public string LCSCCTRMAP = "";
        public string LCSENTRYSETGROUP = "";
        public string LCSSUBDESC = "";
        public string LCSITEMDESC = "";
        public decimal LCSFORCEBAL = 0;
        //Parameters Labor Detail
        public string LCSCOMPID = "";
        //public string LCSALLSUBSIDIARY = "";

        public DataTable LCSENTRYGROUPMAP = null;
        public DataTable LCSVARIABLEPRORATION = null;
        public DataTable LCSPAYTYPE = null;

        //Accounting Parameters
        public string ACCOUNTINGDB = "";
        public string BASECURRENCY = "";
        public string REPORTCURRENCY = "";
        public decimal FOREXRATE = 0;
        public decimal REPORTFOREXRATE = 0;
        public bool REPORTAMOUNTFLAG = false;
        public int BASECURRENCYFLOATINGPOINT = 0;
        public int REPORTCURRENCYFLOATINGPOINT = 0;
        string EmployeeList = string.Empty;

        struct structLaborCostErrorReport
        {
            public string strEmployeeId;
            public string strType;
            public string strPayCycle;
            public decimal dAmount;
            public decimal dAmount2;
            public string strRemarks;

            public structLaborCostErrorReport(string EmployeeId, string Type, string PayCycle, decimal Amount, decimal Amount2, string Remarks)
            {
                strEmployeeId = EmployeeId;
                strType = Type;
                strPayCycle = PayCycle;
                dAmount = Amount;
                dAmount2 = Amount2;
                strRemarks = Remarks;

            }
        }

        List<structLaborCostErrorReport> listLaborCostRept = new List<structLaborCostErrorReport>();
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

        #region Event handlers for labor cost process
        public delegate void EmpDispEventHandler(object sender, EmpDispEventArgs e);
        public class EmpDispEventArgs : System.EventArgs
        {
            public string IDNumber;
            public string LastName;
            public string FirstName;
            public string StatusMsg;

            public EmpDispEventArgs(string strEmployeeId, string strLast, string strFirst)
            {
                IDNumber = strEmployeeId;
                LastName = strLast;
                FirstName = strFirst;
                StatusMsg = "Successful";
            }

            public EmpDispEventArgs(string strEmployeeId, string strLast, string strFirst, string strStatusMsg)
            {
                IDNumber = strEmployeeId;
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

        public NewLaborCostBL()
        {

        }

        public DataTable ComputeLaborCost(bool bProcessAll, string PayrollPeriod, string cycletype, string EmployeeId, string UserLogin, string companyCode, string centralProfile, string profilecode, object PostingDate, DateTime dtPayrollPeriodStartDate, DateTime dtPayrollPeriodEndDate, string menucode, DALHelper dalHelper, string EmployeeList)
        {
            this.EmployeeList = EmployeeList;
            return ComputeLaborCost(bProcessAll, PayrollPeriod, cycletype, "", UserLogin, companyCode, centralProfile, profilecode, PostingDate, dtPayrollPeriodStartDate, dtPayrollPeriodEndDate, menucode, dalHelper);
        }

        public DataTable ComputeLaborCost(bool bProcessAll, string PayrollPeriod, string cycletype, string EmployeeId, string UserLogin, string companyCode, string centralProfile, string profilecode, object PostingDate, DateTime dtPayrollPeriodStartDate, DateTime dtPayrollPeriodEndDate, string menucode, DALHelper dalHelper)
        {
            #region Variables
            PayCycleCode = PayrollPeriod;
            LoginUser = UserLogin;
            ProfileCode = profilecode;
            string PayrollStart             = "";
            string PayrollEnd               = "";

            string curEmployeeId            = "";
            string curExpenseClass          = "";
            string curProrationBase         = "";
            string curCostcenterCode        = "";
            bool bClassCommon               = false;

            string ControlNumber            = "";
            string BankCode                 = "";
            string EntrySetCode             = "";
            string EntrySetGroup            = "";
            string PayrollCode              = "";
            string SubsidiaryCode           = "";
            string SubsidiaryItemCode       = "";
            string ExpenseGroup             = "";
            string ExplanationFormula       = "";
            string AccountCode              = "";
            string AccountTitle             = "";
            string AccountNature            = "";
            string SubDesc                  = "";
            string ItemDesc                 = "";
            string AcctgSubsidiaryCode      = "";

            DataTable dtErrList             = new DataTable();
            //Storage tables
            DataSet dsPayrollCodes          = null;
            DataSet dsPayrollAccounts       = null;

            DataTable dtEmpPayroll          = null;
            DataTable dtEmpCost             = null;
            DataTable dtEmpCostHours        = null;
            DataTable dtEmpProration        = null;
            DataTable dtCostHdr             = null;
            DataTable dtCostDtl             = null;
            DataTable dtBankCodes           = null;

            DataRow drEmpCostHours          = null;
            DataRow drEmpCost               = null;
            DataRow drCostHdr               = null;
            DataRow drCostDtl               = null;

            DataSet dsSIncome               = null;
            DataSet dsMIncome               = null;
            DataSet dsSDeduction            = null;
            DataSet dsMDeduction            = null;

            #endregion

            try
            {
                #region Initial Setup
                this.dal = dalHelper;
                commonBL = new CommonBL();
                CompanyCode = companyCode;
                CentralProfile = centralProfile;
                MenuCode = menucode;
                if (!GetValue(PostingDate).Equals(string.Empty))
                    PostDate = Convert.ToDateTime(PostingDate);
                //------------------------------------------------------------------
                string ParameterError = SetParameters(cycletype);
                
                InitializeLaborCostReport();

                DataTable dtPayPeriod = commonBL.GetPayCycleStartendDate(PayrollPeriod);
                if (dtPayPeriod.Rows.Count > 0)
                {
                    PayrollStart = dtPayPeriod.Rows[0]["Tps_StartCycle"].ToString();
                    PayrollEnd = dtPayPeriod.Rows[0]["Tps_EndCycle"].ToString();
                }
                else
                    ParameterError += "Pay Cycle not found.\n";

                if (ParameterError != "")
                    throw new Exception(ParameterError);

                #region Create Labor Cost Table
                DALHelper dalTemp = new DALHelper();
                dtEmpCostHours  = DbRecord.GenerateTable(CommonConstants.TableName.T_EmpCostHours, dalTemp);
                dtEmpCost       = DbRecord.GenerateTable(CommonConstants.TableName.T_EmpCost, dalTemp);
                dtCostHdr       = DbRecord.GenerateTable(CommonConstants.TableName.T_CostHdr, dalTemp);
                dtCostDtl       = DbRecord.GenerateTable(CommonConstants.TableName.T_CostDtl, dalTemp);
                #endregion
                //------------------------------------------------------------------
                StatusHandler(this, new StatusEventArgs("Get All Labor Cost Records", false));
                dtEmpPayroll = GetPayrollEmployeesForProcess(bProcessAll, EmployeeId, PayrollPeriod);
                dsPayrollCodes = GetPayrollCodes(PayrollPeriod, cycletype);
                StatusHandler(this, new StatusEventArgs("Get All Labor Cost Records", true));
                //-----------------------------------------------------------------
                StatusHandler(this, new StatusEventArgs("Initialize Proration Basis", false));
                if (LCSVARIABLEPRORATION.Rows.Count > 0) //Variable Proration
                {
                    for (int idx = 0; idx < LCSVARIABLEPRORATION.Rows.Count; idx++)
                    {
                        string strVariableProFormula = LCSVARIABLEPRORATION.Rows[idx]["Mpd_Formula"].ToString()
                                                           .Replace("@CENTRALDB", centralProfile)
                                                           .Replace("@LCSCCTRMAP", string.Format("'{0}'", LCSCCTRMAP));
                        if (!strVariableProFormula.Equals(string.Empty))
                        {
                            ParameterInfo[] paramVariable = new ParameterInfo[4];
                            paramVariable[0] = new ParameterInfo("@PAYCYCLE", PayrollPeriod);
                            paramVariable[1] = new ParameterInfo("@COMPANYCODE", companyCode);
                            paramVariable[2] = new ParameterInfo("@START", PayrollStart);
                            paramVariable[3] = new ParameterInfo("@END", PayrollEnd);
                            DataSet dsVarProHours = dal.ExecuteDataSet(strVariableProFormula + " ORDER BY 1", CommandType.Text, paramVariable);

                            #region Compute Variable Proration Rate
                            for (int i = 0; i < dsVarProHours.Tables[0].Rows.Count; i++)
                            {
                                DataRow[] drResult = dtEmpPayroll.Select(string.Format("Tpy_IDNo = '{0}'", dsVarProHours.Tables[0].Rows[i][0].ToString()));
                                if (drResult.Length > 0)
                                {
                                    if (Convert.ToBoolean(drResult[0]["Tpy_IsClassCommon"].ToString()) && drResult[0]["Tpy_ProrationBase"].ToString() == LCSVARIABLEPRORATION.Rows[idx]["Mpd_SubCode"].ToString())
                                    {
                                        DataView dv = dsVarProHours.Tables[0].DefaultView;
                                        dv.RowFilter = string.Format(@" ([{0}] = '{1}' AND [{2}] = '{3}')"
                                                        , dsVarProHours.Tables[0].Columns[0].ColumnName, dsVarProHours.Tables[0].Rows[i][0].ToString()
                                                        , dsVarProHours.Tables[0].Columns[1].ColumnName, dsVarProHours.Tables[0].Rows[i][1].ToString());
                                        DataTable dtProrationCopy = dv.ToTable();

                                        #region Initialize Cost Hours Row
                                        drEmpCostHours = dtEmpCostHours.NewRow();
                                        drEmpCostHours["Teh_IDNo"]              = dsVarProHours.Tables[0].Rows[i][0].ToString();
                                        drEmpCostHours["Teh_PayCycle"]          = dsVarProHours.Tables[0].Rows[i][1].ToString();
                                        drEmpCostHours["Teh_CostcenterCode"]    = dsVarProHours.Tables[0].Rows[i][2].ToString();
                                        drEmpCostHours["Teh_Hours"]             = 0;
                                        drEmpCostHours["Teh_ProrationRate"]     = 0;
                                        drEmpCostHours["Usr_Login"]             = UserLogin;
                                        drEmpCostHours["Ludatetime"]            = DateTime.Now;
                                        #endregion

                                        decimal dCostHours = Convert.ToDecimal(dsVarProHours.Tables[0].Rows[i][3].ToString());
                                        decimal dCostTotals = Convert.ToDecimal(dtProrationCopy.Compute("SUM([" + dtProrationCopy.Columns[3].ColumnName + "])"
                                                                                                , string.Format("[{0}] = '{1}' AND [{2}] = '{3}'", dsVarProHours.Tables[0].Columns[0].ColumnName, dsVarProHours.Tables[0].Rows[i][0].ToString()
                                                                                                                                                 , dsVarProHours.Tables[0].Columns[1].ColumnName, dsVarProHours.Tables[0].Rows[i][1].ToString())));

                                        decimal dProrationRate = (dCostHours / dCostTotals) * 100;
                                        if (dCostTotals != 0)
                                        {
                                            drEmpCostHours["Teh_Hours"]             = decimal.Round(dCostHours, 2);
                                            drEmpCostHours["Teh_ProrationRate"]     = decimal.Round(dProrationRate, 2);
                                            dtEmpCostHours.Rows.Add(drEmpCostHours);
                                        }
                                    }
                                }
                            }
                            #endregion
                        }
                    }

                    #region ForceBalance EmpCostHours
                    for (int idy = 0; idy < dtEmpPayroll.Rows.Count; idy++)
                    {
                        if (Convert.ToBoolean(dtEmpPayroll.Rows[idy]["Tpy_IsClassCommon"].ToString()))
                        {
                            DataView dvVarPro = dtEmpCostHours.DefaultView;
                            dvVarPro.RowFilter = string.Format("Teh_IDNo = '{0}'", dtEmpPayroll.Rows[idy]["Tpy_IDNo"].ToString());
                            DataTable dtVarProCopy = dvVarPro.ToTable();
                            if (dtVarProCopy.Rows.Count > 0)
                            {
                                decimal dCostTotals = Convert.ToDecimal(dtVarProCopy.Compute(string.Format("SUM(Teh_ProrationRate)"), ""));
                                if (dCostTotals != 100)
                                {
                                    ForceBalance(dtVarProCopy, 100, "Teh_ProrationRate");
                                    DataRow[] drrResult = dtEmpCostHours.Select(string.Format("Teh_IDNo = '{0}'", dtEmpPayroll.Rows[idy]["Tpy_IDNo"].ToString()));
                                    foreach (DataRow row in drrResult) //Delete Existing Records
                                    {
                                        dtEmpCostHours.Rows.Remove(row);
                                    }
                                    dtEmpCostHours.Merge(dtVarProCopy);
                                }
                            }
                        }
                    }
                    #endregion
                }

                #region Fixed Proration Percentage - query
                string strQuery = string.Format(@"SELECT Tep_IDNo as Teh_IDNo
                                                        , '{0}' as Teh_PayCycle
                                                        , Tep_CostcenterCode as Teh_CostcenterCode
                                                        , Tep_ProrationRate as Teh_ProrationRate
                                                        FROM {1}..T_EmpProration
                                                        INNER JOIN (SELECT * FROM Udv_Payroll
				                                                    UNION ALL
				                                                    SELECT * FROM  Udv_PayrollFinalPay ) Payroll 
                                                            ON Tpy_IDNo = Tep_IDNo
															AND Tpy_PayCycle = '{0}'
                                                        WHERE ISNULL(Tep_EndCycle, '{0}') >= '{0}'
                                                            AND '{0}' >= Tep_StartCycle
                                                            AND SUBSTRING(Tpy_ExpenseClass,CHARINDEX('-',Tpy_ExpenseClass)+1,LEN(Tpy_ExpenseClass)) = 'F'"
                                                , PayrollPeriod, centralProfile);
                DataSet ds = dal.ExecuteDataSet(strQuery);
                if (ds.Tables.Count > 0)
                    dtEmpProration = ds.Tables[0];
                #endregion

                StatusHandler(this, new StatusEventArgs("Initialize Proration Basis", true));
                //-------------------------------------------------------------------------------------
                #endregion

                #region Process Per Employee Payroll Codes 

                for (int x = 0; x < dsPayrollCodes.Tables[0].Rows.Count; x++)
                {
                    
                    string CostPayrollCode      = dsPayrollCodes.Tables[0].Rows[x]["Mlc_PayrollCode"].ToString();
                    string SIncomeFormula       = dsPayrollCodes.Tables[0].Rows[x]["Mlc_SIncomeFormula"].ToString();
                    string MIncomeFormula       = dsPayrollCodes.Tables[0].Rows[x]["Mlc_MIncomeFormula"].ToString();
                    string SDeductionFormula    = dsPayrollCodes.Tables[0].Rows[x]["Mlc_SDeductionFormula"].ToString();
                    string MDeductionFormula    = dsPayrollCodes.Tables[0].Rows[x]["Mlc_MDeductionFormula"].ToString();
                    bool bHasAllCostcenter      = Convert.ToBoolean(dsPayrollCodes.Tables[0].Rows[x]["Mlc_HasAllCostcenter"].ToString());

                    dsSIncome = null;
                    dsMIncome = null;
                    dsSDeduction = null;
                    dsMDeduction = null;

                    #region Execute Formulas
                    int idxx = 0;
                    ParameterInfo[] paramEmpCost = new ParameterInfo[2];
                    paramEmpCost[idxx++] = new ParameterInfo("@PAYCYCLE", PayrollPeriod);
                    paramEmpCost[idxx++] = new ParameterInfo("@COMPANYCODE", companyCode);

                    if (!SIncomeFormula.ToUpper().Replace("SELECT 0", "").Equals(string.Empty))
                       dsSIncome = dal.ExecuteDataSet(SIncomeFormula.Replace("@CENTRALDB", centralProfile), CommandType.Text, paramEmpCost);
                   if (!MIncomeFormula.ToUpper().Replace("SELECT 0", "").Equals(string.Empty))
                       dsMIncome = dal.ExecuteDataSet(MIncomeFormula.Replace("@CENTRALDB", centralProfile), CommandType.Text, paramEmpCost);
                   if (!SDeductionFormula.ToUpper().Replace("SELECT 0", "").Equals(string.Empty))
                       dsSDeduction = dal.ExecuteDataSet(SDeductionFormula.Replace("@CENTRALDB", centralProfile), CommandType.Text, paramEmpCost);
                   if (!MDeductionFormula.ToUpper().Replace("SELECT 0", "").Equals(string.Empty))
                       dsMDeduction = dal.ExecuteDataSet(MDeductionFormula.Replace("@CENTRALDB", centralProfile), CommandType.Text, paramEmpCost);
                    #endregion

                    for (int i = 0; i < dtEmpPayroll.Rows.Count; i++)
                    {
                        try
                        {
                            curEmployeeId                    = dtEmpPayroll.Rows[i]["Tpy_IDNo"].ToString();
                            curExpenseClass                  = dtEmpPayroll.Rows[i]["Tpy_ExpenseClass"].ToString();
                            curCostcenterCode                = dtEmpPayroll.Rows[i]["Tpy_CostcenterCode"].ToString();
                            bClassCommon                     = Convert.ToBoolean(dtEmpPayroll.Rows[i]["Tpy_IsClassCommon"].ToString());
                            curProrationBase                 = dtEmpPayroll.Rows[i]["Tpy_ProrationBase"].ToString();

                            #region Initialize dtEmpCost Rows
                            drEmpCost = dtEmpCost.NewRow();
                            drEmpCost["Tec_IDNo"]            = curEmployeeId;
                            drEmpCost["Tec_PayCycle"]        = PayrollPeriod;
                            drEmpCost["Tec_PayrollCode"]     = CostPayrollCode;
                            drEmpCost["Tec_CostcenterCode"]  = curCostcenterCode;
                            drEmpCost["Tec_ProrationRate"]   = 0;
                            drEmpCost["Tec_SIncomeAmt"]      = 0;
                            drEmpCost["Tec_MIncomeAmt"]      = 0;
                            drEmpCost["Tec_SDeductionAmt"]   = 0;
                            drEmpCost["Tec_MDeductionAmt"]   = 0;
                            drEmpCost["Tec_TotalAmt"]        = 0;
                            drEmpCost["Tec_EntrySetGroup"]   = ReplaceString(dtEmpPayroll.Rows[i]);
                            drEmpCost["Tec_PayrollBankCode"] = dtEmpPayroll.Rows[i]["Tpy_BankCode"].ToString();
                            drEmpCost["Tec_ExpenseGroup"]    = dtEmpPayroll.Rows[i]["Tpy_ExpenseGroup"].ToString();
                            drEmpCost["Usr_Login"]           = UserLogin;
                            #endregion

                            if (dsSIncome != null)
                            {
                                DataRow[] drArrResult = dsSIncome.Tables[0].Select(string.Format("[{0}] = '{1}'", dsSIncome.Tables[0].Columns[0].ColumnName, curEmployeeId));

                                if (drArrResult.Length > 0)
                                    drEmpCost["Tec_SIncomeAmt"] = Convert.ToDecimal(drArrResult[0][dsSIncome.Tables[0].Columns[1].ColumnName]);
                            }

                            if (dsMIncome != null)
                            {
                                DataRow[] drArrResult = dsMIncome.Tables[0].Select(string.Format("[{0}] = '{1}'", dsMIncome.Tables[0].Columns[0].ColumnName, curEmployeeId));

                                if (drArrResult.Length > 0)
                                    drEmpCost["Tec_MIncomeAmt"] = Convert.ToDecimal(drArrResult[0][dsMIncome.Tables[0].Columns[1].ColumnName]);
                            }
                            if (dsSDeduction != null)
                            {
                                DataRow[] drArrResult = dsSDeduction.Tables[0].Select(string.Format("[{0}] = '{1}'", dsSDeduction.Tables[0].Columns[0].ColumnName, curEmployeeId));

                                if (drArrResult.Length > 0)
                                    drEmpCost["Tec_SDeductionAmt"] = Convert.ToDecimal(drArrResult[0][dsSDeduction.Tables[0].Columns[1].ColumnName]);
                            }
                            if (dsMDeduction != null)
                            {
                                DataRow[] drArrResult = dsMDeduction.Tables[0].Select(string.Format("[{0}] = '{1}'", dsMDeduction.Tables[0].Columns[0].ColumnName, curEmployeeId));

                                if (drArrResult.Length > 0)
                                    drEmpCost["Tec_MDeductionAmt"] = Convert.ToDecimal(drArrResult[0][dsMDeduction.Tables[0].Columns[1].ColumnName]);
                            }

                            drEmpCost["Tec_TotalAmt"] = Convert.ToDecimal(drEmpCost["Tec_SIncomeAmt"])
                                                        + Convert.ToDecimal(drEmpCost["Tec_MIncomeAmt"])
                                                        + Convert.ToDecimal(drEmpCost["Tec_SDeductionAmt"])
                                                        + Convert.ToDecimal(drEmpCost["Tec_MDeductionAmt"]);
                            drEmpCost["Tec_ProrationRate"] = 100;
                            drEmpCost["Ludatetime"] = DateTime.Now;

                            if (Convert.ToDecimal(drEmpCost["Tec_TotalAmt"]) != 0)
                            {
                                if (bHasAllCostcenter)
                                    dtEmpCost.Rows.Add(drEmpCost);
                                else
                                {
                                    if (!bClassCommon && Convert.ToDecimal(drEmpCost["Tec_TotalAmt"]) != 0)// Specific
                                        dtEmpCost.Rows.Add(drEmpCost);
                                    else if (bClassCommon && Convert.ToDecimal(drEmpCost["Tec_TotalAmt"]) != 0) //Prorated
                                    {
                                        #region Prorated
                                        if (curCostcenterCode.Equals(string.Empty))
                                            AddErrorToLaborCostReport(curEmployeeId, "AE", PayrollPeriod, 0, 0, (Convert.ToBoolean(LCSCCTRMAP) ? "Blank Alternate Account" : "Blank Costcenter"));
                                        if (curProrationBase.Equals(string.Empty))
                                            AddErrorToLaborCostReport(curEmployeeId, "AE", PayrollPeriod, 0, 0, "Blank Proration Base");

                                        if (!curCostcenterCode.Equals(string.Empty) && !curProrationBase.Equals(string.Empty))
                                        {
                                            DataView dvHr = null;
                                            if (curProrationBase == "F")
                                                dvHr = dtEmpProration.DefaultView;
                                           else
                                                dvHr = dtEmpCostHours.DefaultView;
                                            dvHr.RowFilter = string.Format(@" ([{0}] = '{1}' AND [{2}] = '{3}')"
                                                                                    , dvHr.Table.Columns[0], curEmployeeId, dvHr.Table.Columns[1], PayrollPeriod);
                                            DataTable dtEmpCostHoursCopy = dvHr.ToTable();

                                            DataTable dtEmpCostCopy = ComputeProration(drEmpCost, dtEmpCostHoursCopy, dalTemp);
                                            if (dtEmpCostCopy.Rows.Count > 0)
                                                dtEmpCost.Merge(dtEmpCostCopy);
                                        }
                                        #endregion
                                    }
                                }
                            }
                            EmpDispHandler(this, new EmpDispEventArgs(curEmployeeId, dtEmpPayroll.Rows[i]["Last Name"].ToString(), dtEmpPayroll.Rows[i]["First Name"].ToString()));
                        }
                        catch (Exception ex)
                        {
                            CommonProcedures.logErrorToFile(ex.ToString());
                            EmpDispHandler(this, new EmpDispEventArgs(curEmployeeId, dtEmpPayroll.Rows[i]["Last Name"].ToString(), dtEmpPayroll.Rows[i]["First Name"].ToString(), "Error in Labor Cost Calculation : " + ex.Message));
                            AddErrorToLaborCostReport(curEmployeeId, "AE", PayrollPeriod, 0, 0, ex.Message.Substring(0, Math.Min(ex.Message.Length, 1000)));
                        }
                    }
                    
                }

                #endregion

                StatusHandler(this, new StatusEventArgs("Saving of Labor Cost Records", false));
                string strEmpCostTemplate;
                string strUpdateQuery = "";
                int iUpdateCtr = 0;
                #region Labor Cost Insert Template
                strEmpCostTemplate = @" INSERT INTO T_EmpCost (Tec_IDNo,Tec_PayCycle,Tec_PayrollCode,Tec_CostcenterCode,Tec_ProrationRate,Tec_SIncomeAmt,Tec_MIncomeAmt,Tec_SDeductionAmt,Tec_MDeductionAmt,Tec_TotalAmt,Tec_EntrySetGroup,Tec_PayrollBankCode,Tec_ExpenseGroup,Usr_Login,Ludatetime) VALUES ('{0}','{1}','{2}','{3}',{4},{5},{6},{7},{8},{9},'{10}','{11}','{12}','{13}',GETDATE()) ";
                #endregion
                foreach (DataRow drEmpCostCalc in dtEmpCost.Rows)
                {
                    #region EmpCost Insert
                    strUpdateQuery += string.Format(strEmpCostTemplate
                                                       , drEmpCostCalc["Tec_IDNo"]                  //0
                                                       , drEmpCostCalc["Tec_PayCycle"]              //1
                                                       , drEmpCostCalc["Tec_PayrollCode"]           //2
                                                       , drEmpCostCalc["Tec_CostcenterCode"]        //3
                                                       , drEmpCostCalc["Tec_ProrationRate"]         //4
                                                       , drEmpCostCalc["Tec_SIncomeAmt"]            //5
                                                       , drEmpCostCalc["Tec_MIncomeAmt"]            //6
                                                       , drEmpCostCalc["Tec_SDeductionAmt"]         //7
                                                       , drEmpCostCalc["Tec_MDeductionAmt"]         //8
                                                       , drEmpCostCalc["Tec_TotalAmt"]              //9
                                                       , drEmpCostCalc["Tec_EntrySetGroup"]         //10
                                                       , drEmpCostCalc["Tec_PayrollBankCode"]       //11
                                                       , drEmpCostCalc["Tec_ExpenseGroup"]          //12
                                                       , drEmpCostCalc["Usr_Login"]                 //13
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

                if  (dtEmpCostHours.Rows.Count > 0)
                {
                    strUpdateQuery = "";
                    iUpdateCtr = 0;
                    #region CostHours Insert Template
                    strEmpCostTemplate = @" INSERT INTO T_EmpCostHours (Teh_IDNo,Teh_PayCycle,Teh_CostcenterCode,Teh_Hours,Teh_ProrationRate,Usr_Login,Ludatetime) VALUES ('{0}','{1}','{2}',{3},{4},'{5}',GETDATE()) ";
                    #endregion
                    foreach (DataRow drEmpCostHoursCalc in dtEmpCostHours.Rows)
                    {
                        #region EmpCostHours Insert
                        strUpdateQuery += string.Format(strEmpCostTemplate
                                                           , drEmpCostHoursCalc["Teh_IDNo"]                  //0
                                                           , drEmpCostHoursCalc["Teh_PayCycle"]              //1
                                                           , drEmpCostHoursCalc["Teh_CostcenterCode"]        //2
                                                           , drEmpCostHoursCalc["Teh_Hours"]                 //3
                                                           , drEmpCostHoursCalc["Teh_ProrationRate"]         //4
                                                           , drEmpCostHoursCalc["Usr_Login"]                 //5
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
                }

                StatusHandler(this, new StatusEventArgs("Saving of Labor Cost Records", true));


                dal.CommitTransactionSnapshot();
                dal.setTimeOut(600);
                dal.BeginTransactionSnapshot();

                dtBankCodes = BankCodeEntrySetEntryGroup(PayrollPeriod, cycletype);
                dsPayrollAccounts = GetPayrollAccounts(PayrollPeriod, cycletype);

                #region Process Accounts Per Bank Codes | Entry Set Group | Entry Set Codes
                for (int x = 0; x < dtBankCodes.Rows.Count; x++)
                {
                    BankCode = dtBankCodes.Rows[x][0].ToString();
                    EntrySetGroup = dtBankCodes.Rows[x][1].ToString();
                    EntrySetCode = dtBankCodes.Rows[x][2].ToString();
                    int ctr = 1;
                    ControlNumber = CodeExistsInHdr(BankCode, EntrySetCode, EntrySetGroup, PayCycleCode);
                    string lcsentrygrpMap = GetEntryGroupMappingCode(EntrySetGroup);
                    string accntPayrollPeriod = string.Format("{0}-{1}-{2}{3}", (!string.IsNullOrEmpty(lcsentrygrpMap) ? lcsentrygrpMap : (cycletype.Equals("L") ? "FP" : ""))
                                                                                , PayrollPeriod.Substring(2, 4)
                                                                                , dtPayrollPeriodStartDate.Day.ToString().PadLeft(2, '0') + dtPayrollPeriodEndDate.Day.ToString().PadLeft(2, '0')
                                                                                , (cycletype.Equals("L") ? string.Format("-{0}", EntrySetGroup) : ""));

                    if (!ControlNumber.Equals(string.Empty)) //RECALC
                    {
                        #region Clean-up Dtl
                        string queryDtl = string.Format(@" UPDATE T_CostHdr 
                                                            SET Tch_PayrollPeriod = '{2}'
                                                            , Tch_RecordStatus = 'A'
                                                            , Usr_Login = '{1}'
                                                            , Ludatetime = GETDATE() 
                                                            WHERE Tch_ControlNo = '{0}'

                                                            DELETE FROM T_CostDtl WHERE Tcd_ControlNo = '{0}'"
                                                        , ControlNumber
                                                        , UserLogin
                                                        , accntPayrollPeriod);
                        dal.ExecuteNonQuery(queryDtl);
                        #endregion
                    }
                    else
                    {
                        ControlNumber = commonBL.GetDocumentNumber(dal, new DALHelper(CentralProfile, false), "COST", ProfileCode);
                        #region Initialize dtCostHdr Rows
                        drCostHdr = dtCostHdr.NewRow();
                        drCostHdr["Tch_ControlNo"]          = ControlNumber;
                        drCostHdr["Tch_EntrySetCode"]       = EntrySetCode;
                        drCostHdr["Tch_EntrySetGroup"]      = EntrySetGroup;
                        drCostHdr["Tch_PayCycle"]           = PayrollPeriod;
                        drCostHdr["Tch_BankCode"]           = BankCode;
                        drCostHdr["Tch_PostDate"]           = PostingDate;
                        drCostHdr["Tch_PayrollPeriod"]      = accntPayrollPeriod;
                        drCostHdr["Tch_RecordStatus"]       = "A";
                        drCostHdr["Usr_Login"]              = UserLogin;
                        drCostHdr["Ludatetime"]             = DateTime.Now;
                        dtCostHdr.Rows.Add(drCostHdr);
                        #endregion
                    }

                    for (int i = 0; i < dsPayrollAccounts.Tables[0].Rows.Count; i++) //M_Account
                    {
                        PayrollCode         = dsPayrollAccounts.Tables[0].Rows[i]["Mac_PayrollCode"].ToString();
                        SubsidiaryCode      = dsPayrollAccounts.Tables[0].Rows[i]["Mac_SubsidiaryCode"].ToString();
                        SubsidiaryItemCode  = dsPayrollAccounts.Tables[0].Rows[i]["Mac_SubsidiaryItemCode"].ToString();
                        ExpenseGroup        = dsPayrollAccounts.Tables[0].Rows[i]["Mac_ExpenseGroup"].ToString();
                        AccountNature       = dsPayrollAccounts.Tables[0].Rows[i]["Mac_AccountNature"].ToString();
                        AccountCode         = dsPayrollAccounts.Tables[0].Rows[i]["Mac_AccountCode"].ToString();
                        AccountTitle        = dsPayrollAccounts.Tables[0].Rows[i]["Mac_AccountTitle"].ToString();
                        AcctgSubsidiaryCode = dsPayrollAccounts.Tables[0].Rows[i]["Mac_AcctgSubsidiaryCode"].ToString();

                        #region Execute Explanation Formula Description
                        int idx = 0;
                        ParameterInfo[] paramDesc = new ParameterInfo[3];
                        paramDesc[idx++] = new ParameterInfo("@COMPANYCODE", companyCode);
                        paramDesc[idx++] = new ParameterInfo("@SUBCODE", SubsidiaryCode);
                        paramDesc[idx++] = new ParameterInfo("@ITEMCODE", SubsidiaryItemCode); 
                        if (!LCSSUBDESC.Equals(string.Empty))
                            SubDesc = GetFormulaQueryStringValue(LCSSUBDESC.Replace("@CENTRALDB", centralProfile), paramDesc, dal);
                        if (!LCSITEMDESC.Equals(string.Empty))
                            ItemDesc = GetFormulaQueryStringValue(LCSITEMDESC.Replace("@CENTRALDB", centralProfile), paramDesc, dal);
                        #endregion

                        #region Execute Explanation Formula
                        int idxx = 0;
                        ParameterInfo[] paramAccounts = new ParameterInfo[9];
                        paramAccounts[idxx++] = new ParameterInfo("@PAYCYCLE", PayrollPeriod);
                        paramAccounts[idxx++] = new ParameterInfo("@COMPANYCODE", companyCode);
                        paramAccounts[idxx++] = new ParameterInfo("@SUBCODE", SubsidiaryCode);
                        paramAccounts[idxx++] = new ParameterInfo("@ITEMCODE", SubsidiaryItemCode);
                        paramAccounts[idxx++] = new ParameterInfo("@ACCOUNTCODE", AccountCode);
                        paramAccounts[idxx++] = new ParameterInfo("@ACCOUNTTITLE", AccountTitle);
                        paramAccounts[idxx++] = new ParameterInfo("@SUBDESC", SubDesc);
                        paramAccounts[idxx++] = new ParameterInfo("@ITEMDESC", ItemDesc);
                        paramAccounts[idxx++] = new ParameterInfo("@IDNUMBER", "");

                        if (dsPayrollAccounts.Tables[0].Rows[i]["Mac_ExplanationFormula"].ToString() != string.Empty)
                            ExplanationFormula = GetFormulaQueryStringValue(dsPayrollAccounts.Tables[0].Rows[i]["Mac_ExplanationFormula"].ToString().Replace("@CENTRALDB", centralProfile), paramAccounts, dal);
                        #endregion

                        #region Identify Field Name to Sum
                        string fieldName = "";
                        if (SubsidiaryCode == "CST")
                            fieldName = "Tec_CostcenterCode";
                        else if (SubsidiaryCode == "EMP" || SubsidiaryItemCode == "EMP")
                            fieldName = "Tec_IDNo";
                        #endregion

                        DataTable dtTotals = GetCostTotals(fieldName, PayCycleCode, BankCode, PayrollCode, ExpenseGroup, EntrySetGroup);

                        for (int a = 0; a < dtTotals.Rows.Count; a++)
                        {
                            if (dtTotals.Rows[a][0].ToString() != "")
                            {
                                #region Initialize dtCostDtl Rows
                                drCostDtl = dtCostDtl.NewRow();
                                drCostDtl["Tcd_ControlNo"]          = ControlNumber;
                                drCostDtl["Tcd_LineNo"]             = ctr;
                                drCostDtl["Tcd_AccountCode"]        = AccountCode;
                                drCostDtl["Tcd_AccountNature"]      = AccountNature;
                                drCostDtl["Tcd_SubsidiaryCode"]     = "";
                                drCostDtl["Tcd_SubsidiaryItemCode"] = "";
                                drCostDtl["Tcd_Amt"]                = 0;    
                                drCostDtl["Tcd_BaseAmt"]            = 0;
                                drCostDtl["Tcd_ReportAmt"]          = 0;
                                drCostDtl["Tcd_ForexDate"]          = DBNull.Value;
                                if (!PostingDate.Equals(""))
                                    drCostDtl["Tcd_ForexDate"]      = PostingDate;
                                drCostDtl["Tcd_ForexBaseRate"]      = 0;
                                drCostDtl["Tcd_ForexReportRate"]    = 0;
                                drCostDtl["Tcd_Explanation"]        = ExplanationFormula;
                                drCostDtl["Usr_Login"]              = UserLogin;
                                drCostDtl["Ludatetime"]             = DateTime.Now;
                                #endregion

                                #region SubsidiaryCode and SubsidiaryItemCode
                                if (SubsidiaryCode == "ALL")
                                    drCostDtl["Tcd_SubsidiaryCode"] = AcctgSubsidiaryCode;
                                else 
                                {
                                    if (dtTotals.Columns[1].ColumnName == fieldName)
                                        drCostDtl["Tcd_SubsidiaryCode"] = dtTotals.Rows[a][fieldName].ToString();
                                    else if (dtTotals.Columns[2].ColumnName == fieldName)
                                        drCostDtl["Tcd_SubsidiaryCode"] = dtTotals.Rows[a][fieldName].ToString();

                                    //Explanation
                                    if (SubsidiaryCode == "EMP" && !string.IsNullOrEmpty(dsPayrollAccounts.Tables[0].Rows[i]["Mac_ExplanationFormula"].ToString()))
                                        drCostDtl["Tcd_Explanation"] = GetFormulaQueryStringValue(dsPayrollAccounts.Tables[0].Rows[i]["Mac_ExplanationFormula"].ToString().Replace("@CENTRALDB", centralProfile).Replace("@IDNUMBER", string.Format("'{0}'", dtTotals.Rows[a][fieldName].ToString())), paramAccounts, dal);
                                }
                                
                                if (SubsidiaryItemCode == "EMP")
                                {
                                    if (dtTotals.Columns[1].ColumnName == "Tec_IDNo")
                                        drCostDtl["Tcd_SubsidiaryItemCode"] = dtTotals.Rows[a]["Tec_IDNo"].ToString();
                                    else if (dtTotals.Columns[2].ColumnName == "Tec_IDNo")
                                        drCostDtl["Tcd_SubsidiaryItemCode"] = dtTotals.Rows[a]["Tec_IDNo"].ToString();
                                }
                                #endregion

                                #region Compute Amount
                                decimal dTotals = Convert.ToDecimal(dtTotals.Rows[a]["Tec_TotalAmt"].ToString());
                                if (dTotals < 0)
                                {
                                    decimal dTotalsPos = dTotals * -1;
                                    if (AccountNature == "CR")
                                        drCostDtl["Tcd_AccountNature"] = "DR";
                                    else if (AccountNature == "DR")
                                        drCostDtl["Tcd_AccountNature"] = "CR";
                                    drCostDtl["Tcd_Amt"] = dTotalsPos;
                                }
                                else
                                    drCostDtl["Tcd_Amt"] = dTotals;
                                #endregion

                                #region Compute Forex Base and Report Amount
                                drCostDtl["Tcd_ForexBaseRate"]      = FOREXRATE;
                                if (BASECURRENCY.Equals("PHP"))
                                    drCostDtl["Tcd_BaseAmt"] = Convert.ToDecimal(drCostDtl["Tcd_Amt"]) / Convert.ToDecimal(drCostDtl["Tcd_ForexBaseRate"]);
                                else if (!BASECURRENCY.Equals("PHP"))
                                    drCostDtl["Tcd_BaseAmt"]            = Math.Round(Convert.ToDecimal(drCostDtl["Tcd_Amt"]) / Convert.ToDecimal(drCostDtl["Tcd_ForexBaseRate"]), BASECURRENCYFLOATINGPOINT);

                                if (REPORTAMOUNTFLAG)
                                {
                                    drCostDtl["Tcd_ForexReportRate"] = REPORTFOREXRATE;
                                    drCostDtl["Tcd_ReportAmt"]       = Math.Round(Convert.ToDecimal(drCostDtl["Tcd_Amt"]) / Convert.ToDecimal(drCostDtl["Tcd_ForexReportRate"]), REPORTCURRENCYFLOATINGPOINT);
                                }

                                #endregion

                                dtCostDtl.Rows.Add(drCostDtl);
                                ctr++;
                            }
                        }

                        EmpDispHandler(this, new EmpDispEventArgs(string.Format("{0} | {1} | {2}", BankCode, EntrySetCode, EntrySetGroup), "", ""));
                    }
                }
                #endregion

                StatusHandler(this, new StatusEventArgs("Saving of Labor Cost Details Records", false));
                strEmpCostTemplate = "";
                strUpdateQuery = "";
                iUpdateCtr = 0;
                #region Labor Cost Header Insert Template
                strEmpCostTemplate = @" INSERT INTO T_CostHdr (Tch_ControlNo,Tch_EntrySetCode,Tch_EntrySetGroup,Tch_PayCycle,Tch_BankCode,Tch_PostDate,Tch_PayrollPeriod,Tch_RecordStatus,Usr_Login,Ludatetime) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}',GETDATE()) ";
                #endregion
                foreach (DataRow drCostHdrCalc in dtCostHdr.Rows)
                {
                    #region CostHdr Insert
                    strUpdateQuery += string.Format(strEmpCostTemplate
                                                       , drCostHdrCalc["Tch_ControlNo"]                 //0
                                                       , drCostHdrCalc["Tch_EntrySetCode"]              //1
                                                       , drCostHdrCalc["Tch_EntrySetGroup"]             //2
                                                       , drCostHdrCalc["Tch_PayCycle"]                  //3
                                                       , drCostHdrCalc["Tch_BankCode"]                  //4
                                                       , drCostHdrCalc["Tch_PostDate"]                  //5
                                                       , drCostHdrCalc["Tch_PayrollPeriod"]             //6
                                                       , drCostHdrCalc["Tch_RecordStatus"]              //7
                                                       , drCostHdrCalc["Usr_Login"]                     //8
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

                if (dtCostDtl.Rows.Count > 0)
                {
                    strUpdateQuery = "";
                    iUpdateCtr = 0;
                    #region Labor Cost Detail Insert Template
                    strEmpCostTemplate = @" INSERT INTO T_CostDtl (Tcd_ControlNo,Tcd_LineNo,Tcd_AccountCode,Tcd_AccountNature,Tcd_SubsidiaryCode,Tcd_SubsidiaryItemCode,Tcd_Amt,Tcd_BaseAmt,Tcd_ReportAmt,Tcd_ForexDate,Tcd_ForexBaseRate,Tcd_ForexReportRate,Tcd_Explanation,Usr_Login,Ludatetime) VALUES ('{0}',{1},'{2}','{3}','{4}','{5}',{6},{7},{8},{9},{10},{11},'{12}','{13}',GETDATE()) ";
                    #endregion
                    foreach (DataRow drCostDtlCalc in dtCostDtl.Rows)
                    {
                        #region CostDtl Insert
                        strUpdateQuery += string.Format(strEmpCostTemplate
                                                           , drCostDtlCalc["Tcd_ControlNo"]                     //0
                                                           , drCostDtlCalc["Tcd_LineNo"]                        //1
                                                           , drCostDtlCalc["Tcd_AccountCode"]                   //2
                                                           , drCostDtlCalc["Tcd_AccountNature"]                 //3
                                                           , drCostDtlCalc["Tcd_SubsidiaryCode"]                //4
                                                           , drCostDtlCalc["Tcd_SubsidiaryItemCode"]            //5
                                                           , drCostDtlCalc["Tcd_Amt"]                           //6
                                                           , drCostDtlCalc["Tcd_BaseAmt"]                       //7
                                                           , drCostDtlCalc["Tcd_ReportAmt"]                     //8
                                                           , (drCostDtlCalc["Tcd_ForexDate"] == DBNull.Value ? "NULL" : string.Format("'{0}'", drCostDtlCalc["Tcd_ForexDate"])) //9
                                                           , drCostDtlCalc["Tcd_ForexBaseRate"]                 //10
                                                           , drCostDtlCalc["Tcd_ForexReportRate"]               //11
                                                           , drCostDtlCalc["Tcd_Explanation"]                   //12
                                                           , drCostDtlCalc["Usr_Login"]                         //13
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
                }
                StatusHandler(this, new StatusEventArgs("Saving of Labor Cost Details Records", true));


                #region Generate Labor Hours Error List
                dtErrList = SaveLaborCostErrorReportList();
                if (dtErrList.Rows.Count > 0)
                    InsertToEmpLaborCostCheckTable(dtErrList);
                #endregion

            }
            catch (Exception ex)
            {
                //dal.RollBackTransactionSnapshot();
                CommonProcedures.logErrorToFile(ex.ToString());
                throw new Exception("Labor Cost Calculation has encountered some errors: \n" + ex.Message);
            }
            finally
            {
                //dal.CloseDB();
            }

            return new DataTable();
        }

        private DataTable ComputeProration(DataRow drEmpCostCopy, DataTable dtEmpPorationCopy, DALHelper dal)
        {
            DataTable dt = DbRecord.GenerateTable(CommonConstants.TableName.T_EmpCost, dal);
            DataRow drEmpCost = null;
            for (int i = 0; i < dtEmpPorationCopy.Rows.Count; i++)
            {
                drEmpCost = dt.NewRow();
                #region Initialize dtEmpCost Rows
                drEmpCost["Tec_IDNo"]                           = drEmpCostCopy["Tec_IDNo"].ToString();
                drEmpCost["Tec_PayCycle"]                       = drEmpCostCopy["Tec_PayCycle"].ToString();
                drEmpCost["Tec_PayrollCode"]                    = drEmpCostCopy["Tec_PayrollCode"].ToString();
                drEmpCost["Tec_CostcenterCode"]                 = dtEmpPorationCopy.Rows[i]["Teh_CostcenterCode"].ToString();
                drEmpCost["Tec_ProrationRate"]                  = 0;
                drEmpCost["Tec_SIncomeAmt"]                     = 0;
                drEmpCost["Tec_MIncomeAmt"]                     = 0;
                drEmpCost["Tec_SDeductionAmt"]                  = 0;
                drEmpCost["Tec_MDeductionAmt"]                  = 0;
                drEmpCost["Tec_TotalAmt"]                       = 0;
                drEmpCost["Tec_EntrySetGroup"]                  = drEmpCostCopy["Tec_EntrySetGroup"];
                drEmpCost["Tec_PayrollBankCode"]                = drEmpCostCopy["Tec_PayrollBankCode"];
                drEmpCost["Tec_ExpenseGroup"]                   = drEmpCostCopy["Tec_ExpenseGroup"];
                drEmpCost["Usr_Login"]                          = drEmpCostCopy["Usr_Login"];
                drEmpCost["Ludatetime"]                         = DateTime.Now;
                #endregion

                decimal dSIncomeAmtCopy                         = Convert.ToDecimal(drEmpCostCopy["Tec_SIncomeAmt"].ToString());
                decimal dMIncomeAmtCopy                         = Convert.ToDecimal(drEmpCostCopy["Tec_MIncomeAmt"].ToString());
                decimal dSDeductionAmtCopy                      = Convert.ToDecimal(drEmpCostCopy["Tec_SDeductionAmt"].ToString());
                decimal dMDeductionAmtCopy                      = Convert.ToDecimal(drEmpCostCopy["Tec_MDeductionAmt"].ToString());
                decimal dTotalAmtCopy                           = Convert.ToDecimal(drEmpCostCopy["Tec_TotalAmt"].ToString());
                decimal dProrationrate                          = Convert.ToDecimal(dtEmpPorationCopy.Rows[i]["Teh_ProrationRate"].ToString());

                if (dProrationrate != 0)
                    drEmpCost["Tec_ProrationRate"] = dProrationrate;
                if (dSIncomeAmtCopy != 0)
                    drEmpCost["Tec_SIncomeAmt"] = decimal.Round(dSIncomeAmtCopy * (dProrationrate / 100), 2);
                if (dMIncomeAmtCopy != 0)
                    drEmpCost["Tec_MIncomeAmt"] = decimal.Round(dMIncomeAmtCopy * (dProrationrate / 100), 2);
                if (dSDeductionAmtCopy != 0)
                    drEmpCost["Tec_SDeductionAmt"] = decimal.Round(dSDeductionAmtCopy * (dProrationrate / 100), 2);
                if (dMDeductionAmtCopy != 0)
                    drEmpCost["Tec_MDeductionAmt"] = decimal.Round(dMDeductionAmtCopy * (dProrationrate / 100), 2);
                if (dTotalAmtCopy != 0)
                    drEmpCost["Tec_TotalAmt"] = decimal.Round(dTotalAmtCopy * (dProrationrate / 100), 2);

                dt.Rows.Add(drEmpCost);
            }
            ForceBalance(dt, Convert.ToDecimal(drEmpCostCopy["Tec_SIncomeAmt"].ToString()), "Tec_SIncomeAmt");
            ForceBalance(dt, Convert.ToDecimal(drEmpCostCopy["Tec_MIncomeAmt"].ToString()), "Tec_MIncomeAmt");
            ForceBalance(dt, Convert.ToDecimal(drEmpCostCopy["Tec_SDeductionAmt"].ToString()), "Tec_SDeductionAmt");
            ForceBalance(dt, Convert.ToDecimal(drEmpCostCopy["Tec_MDeductionAmt"].ToString()), "Tec_MDeductionAmt");

            return (dt.Rows.Count > 0 ? ForceBalance(dt, Convert.ToDecimal(drEmpCostCopy["Tec_TotalAmt"].ToString()), "Tec_TotalAmt") : dt);
        }

        //private DataTable ForceBalance(DataTable dt, decimal dSIncomeAmt, decimal dMIncomeAmt,  
        //                            decimal dSDeductionAmt, decimal dMDeductionAmt, decimal dTotalAmt) //
        //{
        //    decimal dCostTotals = Convert.ToDecimal(dt.Compute(string.Format("SUM(Tec_TotalAmt)"), ""));
        //    if (dCostTotals != dTotalAmt)
        //    {
        //        decimal dVariance = 0;
        //        if (dTotalAmt > dCostTotals)
        //        {
        //            dVariance = dTotalAmt - dCostTotals;
        //            if (dVariance <= LCSFORCEBAL)
        //                dt.Rows[0][colAmtName] = Convert.ToDecimal(dt.Rows[0][colAmtName].ToString()) + dVariance;
        //            else
        //                AddErrorToLaborCostReport(dt.Rows[0][dt.Columns[0].ColumnName].ToString(), "AE", dt.Rows[0][dt.Columns[1].ColumnName].ToString(), 0, 0, "With Variance");
        //        }
        //        else if (dTotalAmt < dCostTotals)
        //        {
        //            dVariance = dCostTotals - dTotalAmt;
        //            if (dVariance <= LCSFORCEBAL)
        //                dt.Rows[0][colAmtName] = Convert.ToDecimal(dt.Rows[0][colAmtName].ToString()) - dVariance;
        //            else
        //                AddErrorToLaborCostReport(dt.Rows[0][dt.Columns[0].ColumnName].ToString(), "AE", dt.Rows[0][dt.Columns[1].ColumnName].ToString(), 0, 0, "With Variance");
        //        }
        //    }
        //    return dt;
        //}

        private DataTable ForceBalance(DataTable dt, decimal dTotalAmt, string colAmtName) //Tec_TotalAmt
        {
            decimal dCostTotals = Convert.ToDecimal(dt.Compute(string.Format("SUM({0})", colAmtName), ""));
            if (dCostTotals != dTotalAmt)
            {
                decimal dVariance = 0;
                if (dTotalAmt > dCostTotals)
                {
                    dVariance = dTotalAmt - dCostTotals;
                    if (dVariance <= LCSFORCEBAL)
                        dt.Rows[0][colAmtName] = Convert.ToDecimal(dt.Rows[0][colAmtName].ToString()) + dVariance;
                    else
                        AddErrorToLaborCostReport(dt.Rows[0][dt.Columns[0].ColumnName].ToString(), "AE", dt.Rows[0][dt.Columns[1].ColumnName].ToString(), 0, 0, "With Variance");
                }
                else if (dTotalAmt < dCostTotals)
                {
                    dVariance = dCostTotals - dTotalAmt;
                    if (dVariance <= LCSFORCEBAL)
                        dt.Rows[0][colAmtName] = Convert.ToDecimal(dt.Rows[0][colAmtName].ToString()) - dVariance;
                    else
                        AddErrorToLaborCostReport(dt.Rows[0][dt.Columns[0].ColumnName].ToString(), "AE", dt.Rows[0][dt.Columns[1].ColumnName].ToString(), 0, 0, "With Variance");
                }
            }
            return dt;
        }

        public DataSet GetPayrollCodes(string PayCycleCode, string CycleType)
        {
            #region query
            string query = string.Format(@"SELECT * FROM {0}..M_Cost
                                            INNER JOIN T_CostProrationControl ON Tcc_CycleType = '{3}'
                                                AND (Tcc_ApplicablePayCycle = RIGHT('{2}',1) OR Tcc_ApplicablePayCycle = '0')
                                                AND Tcc_RecordStatus = 'A'
                                            WHERE Mlc_CompanyCode = '{1}'
                                                AND Mlc_RecordStatus = 'A'
                                                AND Mlc_PayrollCode IN (SELECT DISTINCT Mac_PayrollCode FROM {0}..M_Account
                                                                            WHERE  Mac_EntrySetCode = Tcc_EntrySetCode 
                                                                                AND Mac_CompanyCode = '{1}'
                                                                                AND Mac_RecordStatus = 'A')", CentralProfile
                                                                                                            , CompanyCode
                                                                                                            , PayCycleCode
                                                                                                            , CycleType);
            #endregion
            DataSet ds = null;
            ds = dal.ExecuteDataSet(query);
            return ds;
        }

        public DataSet GetPayrollAccounts(string PayCycleCode, string CycleType)
        {
            #region query
            string query = string.Format(@"SELECT * FROM {3}..M_Account 								
                                            WHERE Mac_RecordStatus = 'A'
                                              AND Mac_CompanyCode = '{2}'   								
                                              AND Mac_EntrySetCode IN (SELECT Tcc_EntrySetCode FROM T_CostProrationControl 								
		                                                                WHERE Tcc_CycleType = '{1}'						
		                                                                AND (Tcc_ApplicablePayCycle = RIGHT('{0}',1) OR Tcc_ApplicablePayCycle = '0')						
		                                                                AND Tcc_RecordStatus = 'A')
                                                                        ORDER BY Mac_AccountNature DESC, Mac_AccountCode", PayCycleCode
                                                                                                                        , CycleType
                                                                                                                        , CompanyCode
                                                                                                                        , CentralProfile);
            #endregion
            DataSet ds = null;
            ds = dal.ExecuteDataSet(query);
            return ds;
        }

        public DataTable GetPayrollEmployeesForProcess(bool bProcessAll, string EmployeeId, string PayCycleCode)
        {
            try
            {
                string condition = string.Empty;
                if (!bProcessAll && EmployeeId != "")
                    condition += " AND Tpy_IDNo = '" + EmployeeId + "' ";
                else if (bProcessAll && EmployeeList != "")
                    condition += " AND Tpy_IDNo IN (" + EmployeeList + ") ";

                DataTable dtResult = new DataTable();
                #region Query
                string query = @"
                        SELECT Tpy_IDNo
                            , Mem_LastName + CASE WHEN LEN(ISNULL(Mem_ExtensionName,'')) > 0 
							                        THEN  ' ' + Mem_ExtensionName ELSE '' END as [Last Name]
	                        , Mem_FirstName as [First Name]
	                        , Mem_MiddleName as [Middle Name]
	                        , Tpy_PayrollGroup
	                        , Tpy_PayrollType
                            , Tpy_EmploymentStatus
                            , Tpy_PaymentMode
                            , Tpy_BankCode
                            , Tpy_WorkStatus
                            , Tpy_PositionCode
                            , Tpy_PositionGrade
                            , Tpy_ExpenseClass
                            , CASE WHEN '{3}' = 'TRUE' THEN Mcc_AlternateAccount ELSE Tpy_CostcenterCode END as Tpy_CostcenterCode
                            , ISNULL(Mex_IsClassCommon,0) as [Tpy_IsClassCommon]
                            --, CASE WHEN ISNULL(Mex_IsClassCommon,0) = 1 THEN 
                            --                                    CASE WHEN SUBSTRING(Tpy_ExpenseClass,CHARINDEX('-',Tpy_ExpenseClass)+1,LEN(Tpy_ExpenseClass)) = 'M' 
                            --                                    THEN 'Manhours' 
                            --                                        WHEN SUBSTRING(Tpy_ExpenseClass,CHARINDEX('-',Tpy_ExpenseClass)+1,LEN(Tpy_ExpenseClass)) = 'F'
                            --		                            THEN 'Fixed Proration Percentage'
                            --                                    ELSE 'Error, Not set-up' END
                            --                                ELSE '' END as [Proration Base]
                            , CASE WHEN ISNULL(Mex_IsClassCommon,0) = 1 THEN SUBSTRING(Tpy_ExpenseClass,CHARINDEX('-',Tpy_ExpenseClass)+1,LEN(Tpy_ExpenseClass))
                                ELSE '' END as [Tpy_ProrationBase]
                            , Mex_ExpenseGroup as [Tpy_ExpenseGroup]
                            FROM (SELECT * FROM Udv_Payroll
				                  UNION ALL
				                  SELECT * FROM  Udv_PayrollFinalPay ) Payroll
                            INNER JOIN M_Employee ON Tpy_IDNo = Mem_IDNo
                            LEFT JOIN {0}..M_Expense ON Tpy_ExpenseClass = Mex_ExpenseClass
							        AND Mex_CompanyCode = '{1}'
                            LEFT JOIN {0}..M_CostCenter ON Tpy_CostcenterCode = Mcc_CostCenterCode
							        AND Mcc_CompanyCode = '{1}'
                            WHERE Tpy_PayCycle = '{2}'
                            @CONDITIONS";
                #endregion
                query = string.Format(query, CentralProfile, CompanyCode, PayCycleCode, LCSCCTRMAP);
                query = query.Replace("@CONDITIONS", condition);
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                return dtResult;
            }
            catch
            {    return null;   }
        }

        public decimal GetFormulaQueryDecimalValue(string query, ParameterInfo[] paramInfo, DALHelper dal)
        {
            try
            {
                decimal dValue = 0;
                DataTable dtResult = dal.ExecuteDataSet(query, CommandType.Text, paramInfo).Tables[0];
                if (dtResult.Rows.Count > 0)
                {
                    dValue = Convert.ToDecimal(dtResult.Rows[0][0]);
                }
                return dValue;
            }
            catch
            { return 0; }
        }

        public string GetFormulaQueryStringValue(string query, ParameterInfo[] paramInfo, DALHelper dal)
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

        public string SetParameters(string CycleType)
        {
            string ProcessingErrorMessage = "";
            string strResult = string.Empty;
            #region Parameters
            StatusHandler(this, new StatusEventArgs("Loading Parameters", false));
            StatusHandler(this, new StatusEventArgs("Loading Parameters", true));

            strResult = commonBL.GetParameterValueFromCentral("LCSFORCEBAL", CompanyCode, CentralProfile, dal);
            LCSFORCEBAL = strResult.Equals(string.Empty) ? 0 : Convert.ToDecimal(strResult);
            StatusHandler(this, new StatusEventArgs(string.Format("  LCSFORCEBAL = {0}", LCSFORCEBAL), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  LCSFORCEBAL = {0}", LCSFORCEBAL), true));

            LCSCCTRMAP = commonBL.GetParameterValueFromCentral("LCSCCTRMAP", CompanyCode, CentralProfile, dal);
            StatusHandler(this, new StatusEventArgs(string.Format("  LCSCCTRMAP = {0}", LCSCCTRMAP), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  LCSCCTRMAP = {0}", LCSCCTRMAP), true));

            LCSENTRYSETGROUP = commonBL.GetParameterValueFromPayroll((CycleType.Equals("L") ? "LCSENTRYSETGROUPFP" : "LCSENTRYSETGROUP"), CompanyCode, dal);
            StatusHandler(this, new StatusEventArgs(string.Format("  LCSENTRYSETGROUP = {0}", LCSENTRYSETGROUP), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  LCSENTRYSETGROUP = {0}", LCSENTRYSETGROUP), true));

            LCSSUBDESC = commonBL.GetParameterFormulaFromCentral("LCSSUBDESC", CompanyCode, CentralProfile, dal);
            StatusHandler(this, new StatusEventArgs(string.Format("  LCSSUBDESC = {0}", LCSSUBDESC), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  LCSSUBDESC = {0}", LCSSUBDESC), true));

            LCSITEMDESC = commonBL.GetParameterFormulaFromCentral("LCSITEMDESC", CompanyCode, CentralProfile, dal);
            StatusHandler(this, new StatusEventArgs(string.Format("  LCSITEMDESC = {0}", LCSITEMDESC), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  LCSITEMDESC = {0}", LCSITEMDESC), true));

            LCSCOMPID = commonBL.GetParameterValueFromCentral("LCSCOMPID", CompanyCode, CentralProfile, dal);
            StatusHandler(this, new StatusEventArgs(string.Format("  LCSCOMPID = {0}", LCSCOMPID), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  LCSCOMPID = {0}", LCSCOMPID), true));

            LCSENTRYGROUPMAP = GetEntryGroupMappingParameter("LCSENTRYGROUPMAP");
            if (LCSENTRYGROUPMAP.Rows.Count == 0)
                ProcessingErrorMessage += "Entry Group Mapping to Accounting is not set-up in Policy Detail.\n";
            StatusHandler(this, new StatusEventArgs(string.Format("  Getting Entry Group Mapping to Accounting"), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  Getting Entry Group Mapping to Accounting"), true));

            LCSVARIABLEPRORATION = commonBL.GetPolicyDtlListFormulafromCentral("LCSVARIABLEPRORATION", CompanyCode, CentralProfile, dal);
            if (LCSVARIABLEPRORATION.Rows.Count == 0)
                ProcessingErrorMessage += "Variable Proration Formula is not set-up in Policy Detail.\n";
            StatusHandler(this, new StatusEventArgs(string.Format("  Getting Variable Proration Formula"), false));
            StatusHandler(this, new StatusEventArgs(string.Format("  Getting Variable Proration Formula"), true));
            #endregion

            #region ACCOUNTING Paramaters
            ACCOUNTINGDB = Encrypt.decryptText(ConfigurationManager.AppSettings["AccountingDBName"].ToString());
            string sql = string.Format(@"SELECT Ccm_ReportAmountCalculateFlag FROM {0}..T_CompanyConfigurationMaster", ACCOUNTINGDB);
            DataTable dtResult = dal.ExecuteDataSet(sql, CommandType.Text).Tables[0];
            if (dtResult.Rows.Count > 0)
            {
                if (dtResult.Rows[0][0] != null)
                    REPORTAMOUNTFLAG = Convert.ToBoolean(dtResult.Rows[0][0]);
            }
            //----------------------------------------------------------------
            sql = "";
            sql = string.Format(@"SELECT Cum_CurrencyID AS [Base Currency] FROM {0}..T_CurrencyMaster WHERE Cum_CurrencyType = 'B' AND Cum_Status = 'A'", ACCOUNTINGDB);
            dtResult = dal.ExecuteDataSet(sql, CommandType.Text).Tables[0];
            if (dtResult.Rows.Count > 0)
            {
                if (dtResult.Rows[0][0] != null)
                    BASECURRENCY = dtResult.Rows[0][0].ToString();
            }
            if (BASECURRENCY.Equals("PHP"))
                FOREXRATE = 1;
            else if (!BASECURRENCY.Equals("") && !BASECURRENCY.Equals("PHP"))
                FOREXRATE = GetForexRate(BASECURRENCY);

            if (FOREXRATE == 0)
                ProcessingErrorMessage += "Forex Base Rate is not set-up.\n";
            //---------------------------------------------------------------
            sql = "";
            sql = string.Format(@"SELECT Ccm_BaseCurrencyDecimalFloat AS [Base Currency Floating] FROM {0}..T_CompanyConfigurationMaster WHERE Ccm_CompanyType = 'B' AND Ccm_Status = 'A'", ACCOUNTINGDB);
            dtResult = dal.ExecuteDataSet(sql, CommandType.Text).Tables[0];
            if (dtResult.Rows.Count > 0)
            {
                if (dtResult.Rows[0][0] != null)
                    BASECURRENCYFLOATINGPOINT = Convert.ToInt32(dtResult.Rows[0][0].ToString());
            }
            //---------------------------------------------------------------
            if (REPORTAMOUNTFLAG)
            {
                sql = "";
                sql = string.Format(@"SELECT Cum_CurrencyID AS [Report Currency] FROM {0}..T_CurrencyMaster WHERE Cum_Status = 'A' AND Cum_ReportCurrencyFlag = 1", ACCOUNTINGDB);
                dtResult = dal.ExecuteDataSet(sql, CommandType.Text).Tables[0];
                if (dtResult.Rows.Count > 0)
                {
                    if (dtResult.Rows[0][0] != null)
                        REPORTCURRENCY = dtResult.Rows[0][0].ToString();
                }

                if (!REPORTCURRENCY.Equals(""))
                    REPORTFOREXRATE = GetForexRate(REPORTCURRENCY);

                if (REPORTFOREXRATE == 0)
                    ProcessingErrorMessage += "Forex Report Rate is not set-up.\n";

                //---------------------------------------------------------------
                sql = "";
                sql = string.Format(@"SELECT Ccm_ReportCurrencyDecimalFloat AS [Report Currency Floating] FROM {0}..T_CompanyConfigurationMaster WHERE Ccm_CompanyType = 'B' AND Ccm_Status = 'A'", ACCOUNTINGDB);
                dtResult = dal.ExecuteDataSet(sql, CommandType.Text).Tables[0];
                if (dtResult.Rows.Count > 0)
                {
                    if (dtResult.Rows[0][0] != null)
                        REPORTCURRENCYFLOATINGPOINT = Convert.ToInt32(dtResult.Rows[0][0].ToString());
                }
            }
            //---------------------------------------------------------------
            #endregion

            return ProcessingErrorMessage;
        }

        public string ReplaceString(DataRow dr)
        {
            string rString = "";
            #region LCSENTRYSETGROUP Replace
            string[] strArrItems = LCSENTRYSETGROUP.Split(new char[] { '|' });
            foreach (string strArr in strArrItems)
            {
                switch (strArr)
                {
                    case "EMPSTAT":
                        rString += dr["Tpy_EmploymentStatus"].ToString() + "|";
                        break;
                    case "PAYGRP":
                        rString += dr["Tpy_PayrollGroup"].ToString() + "|";
                        break;
                    case "PAYMODE":
                        rString += dr["Tpy_PaymentMode"].ToString() + "|";
                        break;
                    case "PAYTYPE":
                        rString += dr["Tpy_PayrollType"].ToString() + "|";
                        break;
                    case "POSITION":
                        rString += dr["Tpy_PositionCode"].ToString() + "|";
                        break;
                    case "GRADE":
                        rString += dr["Tpy_PositionGrade"].ToString() + "|";
                        break;
                    case "DIVI":
                        if (dr["Tpy_CostcenterCode"].ToString().Length >= 2)
                            rString += dr["Tpy_CostcenterCode"].ToString().Substring(0, 2) + "|";
                        else
                            rString += dr["Tpy_CostcenterCode"].ToString() + "|";
                        break;
                    case "DEPT":
                        if (dr["Tpy_CostcenterCode"].ToString().Length >= 4)
                            rString += dr["Tpy_CostcenterCode"].ToString().Substring(0, 4) + "|";
                        else
                            rString += dr["Tpy_CostcenterCode"].ToString() + "|";
                        break;
                    case "SECT":
                        if (dr["Tpy_CostcenterCode"].ToString().Length >= 6)
                            rString += dr["Tpy_CostcenterCode"].ToString().Substring(0, 6) + "|";
                        else
                            rString += dr["Tpy_CostcenterCode"].ToString() + "|";
                        break;
                    case "IDNO":
                        rString += dr["Tpy_IDNo"].ToString() + "|";
                        break;
                    case "NONE":
                        rString += "";
                        break;
                }
            }
            if (rString != "")
                rString = rString.Substring(0, rString.Length - 1);

            #endregion
            return rString;
        }

        public DataTable GetEntryGroupMappingParameter(string ParameterID)
        {
            DataSet ds = new DataSet();

            #region query
            string qString = string.Format(@" SELECT Mpd_SubCode, Mpd_SubName, Mpd_ParamValue
													FROM M_PolicyDtl
													WHERE Mpd_PolicyCode = '{0}'
													AND Mpd_CompanyCode = '{1}'
													AND Mpd_RecordStatus = 'A'
                                                ORDER BY 1", ParameterID, CompanyCode);
            #endregion
            ds = dal.ExecuteDataSet(qString);

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0];
            else
                return new DataTable();
        }

        public string GetEntryGroupMappingCode(string EntrySetGroup)
        {
            string groupMapping = "";
            string entrysetgrp = (EntrySetGroup != "" ? EntrySetGroup : LCSENTRYSETGROUP);
            if (LCSENTRYGROUPMAP.Rows.Count > 0)
            {
                for (int i = 0; i < LCSENTRYGROUPMAP.Rows.Count; i++)
                {
                    if (LCSENTRYGROUPMAP.Rows[i]["Mpd_SubCode"].ToString() == entrysetgrp)
                    {
                        groupMapping = LCSENTRYGROUPMAP.Rows[i]["Mpd_ParamValue"].ToString();
                    }
                }
            }
            return groupMapping;
        }

        public DataTable GetFilterItems(string Column, string PayCycleCode)
        {
            #region query
            string query = string.Format(@"SELECT DISTINCT {0} 
                                           FROM Udv_Cost
                                           WHERE Tch_PayCycle = '{1}'
                                               AND Tch_RecordStatus = 'A'
                                           ORDER BY 1", Column, PayCycleCode);
            #endregion
            DataSet ds = null;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(query, CommandType.Text);
                dal.CloseDB();
            }
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0];
            }
            else return new DataTable();
        }

        public DataTable GetGenerationToFileSetup(string PayCycleCode, string CycleIndicator)
        {
            string tableName = "T_CostHdr";
            if (CycleIndicator != "C")
                tableName = "T_CostHdrHst";
            #region query
            string query = string.Format(@"SELECT Tch_BankCode
                                                , Tch_EntrySetCode
                                                , Tch_EntrySetGroup
                                                , Tcc_GenerationRule
                                                , Tcc_FileExtension  
				                            FROM {1}
				                            LEFT JOIN T_CostProrationControl 
				                            ON Tch_EntrySetCode= Tcc_EntrySetCode
				                            WHERE Tch_PayCycle = '{0}'
                                                AND Tch_RecordStatus = 'A'", PayCycleCode, tableName);
            #endregion
            DataSet ds = null;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(query, CommandType.Text);
                dal.CloseDB();
            }
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0];
            }
            else return new DataTable();
        }

        #region Labor Cost Details
        private DataTable BankCodeEntrySetEntryGroup(string PayCycleCode, string CycleType)
        {
            DataTable dtResult = new DataTable();
            dtResult.Columns.Add("BankCode", typeof(string));
            dtResult.Columns.Add("EntrySetGroup", typeof(string));
            dtResult.Columns.Add("EntrySetCode", typeof(string));

            DataTable dtBankCodes;
            DataTable dtEntrySetGroups;
            DataTable dtEntrySetCodes;

            dtBankCodes = GetBankCodes(PayCycleCode);
            if (dtBankCodes.Rows.Count > 0)
            {
                for (int b = 0; b < dtBankCodes.Rows.Count; b++)
                {
                    dtEntrySetGroups = GetEntrySetGroups(PayCycleCode, dtBankCodes.Rows[b][0].ToString());
                    if (dtEntrySetGroups.Rows.Count > 0)
                    {
                        for (int g = 0; g < dtEntrySetGroups.Rows.Count; g++)
                        {
                            dtEntrySetCodes = GetEntrySetCodes(PayCycleCode, CycleType);
                            if (dtEntrySetCodes.Rows.Count > 0)
                            {
                                for (int c = 0; c < dtEntrySetCodes.Rows.Count; c++)
                                {
                                    dtResult.Rows.Add(new object[]{
                                                        dtBankCodes.Rows[b][0].ToString(),
                                                        dtEntrySetGroups.Rows[g][0].ToString(),
                                                        dtEntrySetCodes.Rows[c][0].ToString()
                                                   });
                                }
                            }
                        }
                    }
                    else
                        throw new Exception("No Entry Set Group retrieved in Labor Cost table.");
                }
            }
            else
                throw new Exception("No Bank Codes in Payroll Table.");

            return dtResult;
        }

        public DataTable GetBankCodes(string PayCycleCode)
        {
            #region query
            string query = string.Format(@"SELECT DISTINCT Tpy_BankCode 
                                           FROM (SELECT * FROM Udv_Payroll
				                                 UNION ALL
				                                 SELECT * FROM  Udv_PayrollFinalPay ) Payroll 								
                                           WHERE Tpy_PayCycle = '{0}'", PayCycleCode);
            #endregion
            DataTable dtResult = new DataTable();
            dtResult = dal.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public DataTable GetEntrySetCodes(string PayCycleCode, string CycleType)
        {
            #region query
            string query = string.Format(@"SELECT DISTINCT Mac_EntrySetCode FROM {3}..M_Account 								
                                            WHERE Mac_RecordStatus = 'A'	
                                            AND Mac_CompanyCode = '{2}'							
                                            AND Mac_EntrySetCode IN (SELECT Tcc_EntrySetCode FROM T_CostProrationControl 								
		                                            WHERE Tcc_CycleType = '{1}'						
		                                            AND (Tcc_ApplicablePayCycle = RIGHT('{0}',1) OR Tcc_ApplicablePayCycle = '0')						
		                                            AND Tcc_RecordStatus = 'A')", PayCycleCode, CycleType, CompanyCode, CentralProfile);
            #endregion
            DataTable dtResult = new DataTable();
            dtResult = dal.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public DataTable GetEntrySetGroups(string PayCycleCode, string BankCode)
        {
            #region query
            string query = string.Format(@"SELECT DISTINCT Tec_EntrySetGroup 
                                            FROM T_EmpCost
                                            WHERE Tec_PayCycle = '{0}'
                                            AND Tec_PayrollBankCode = '{1}'", PayCycleCode, BankCode);
            #endregion
            DataTable dtResult = new DataTable();
            dtResult = dal.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public DataTable GetCostTotals(string fieldName, string PayCycleCode, string BankCode, string PayrollCode, string ExpenseGroup, string EntrySetGroup)
        {
            int idxx = 0;
            ParameterInfo[] paramEmpCost = new ParameterInfo[5];
            paramEmpCost[idxx++] = new ParameterInfo("@PAYCYCLE", PayCycleCode);
            paramEmpCost[idxx++] = new ParameterInfo("@BANKCODE", BankCode, SqlDbType.VarChar, BankCode.Length);
            paramEmpCost[idxx++] = new ParameterInfo("@PAYROLLCODE", PayrollCode, SqlDbType.VarChar, PayrollCode.Length);
            paramEmpCost[idxx++] = new ParameterInfo("@EXPENSEGROUP", ExpenseGroup, SqlDbType.VarChar, ExpenseGroup.Length);
            paramEmpCost[idxx++] = new ParameterInfo("@ENTRYSETGRP", EntrySetGroup, SqlDbType.VarChar, EntrySetGroup.Length);  

            string grouping = "";
            string select = "SUM(Tec_TotalAmt) as Tec_TotalAmt";

            if (!fieldName.Equals(""))
            {
                if (grouping != "")
                    grouping += string.Format(", {0}", fieldName);
                else
                    grouping += string.Format("GROUP BY {0}", fieldName);

                if (select != "")
                    select += string.Format(", {0}", fieldName);
                else
                    select += fieldName;
            }

            #region query
            string query = string.Format(@"SELECT {2}
                                                FROM T_EmpCost
                                                WHERE Tec_PayCycle = @PAYCYCLE
                                                    AND Tec_PayrollBankCode = @BANKCODE
                                                    AND Tec_PayrollCode = @PAYROLLCODE
                                                    AND Tec_ExpenseGroup IN(SELECT[Data] FROM {0}.dbo.Udf_Split(@EXPENSEGROUP, ',')) 
                                                    AND Tec_EntrySetGroup = @ENTRYSETGRP
                                                    {1}", CentralProfile, grouping, select);
            #endregion
            DataSet ds = null;
            ds = dal.ExecuteDataSet(query, CommandType.Text, paramEmpCost);
            if (ds != null && ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return new DataTable();
        }

        public string CodeExistsInHdr(string BankCode, string EntrySetCode, string EntrySetGroup, string PayCycleCode)
        {
            #region query
            string query = string.Format(@"SELECT Tch_ControlNo FROM T_CostHdr 								
                                            WHERE Tch_BankCode = '{0}'
                                                AND Tch_EntrySetCode = '{1}'
                                                AND Tch_EntrySetGroup = '{2}'
                                                AND Tch_PayCycle = '{3}'"
                                            , BankCode
                                            , EntrySetCode
                                            , EntrySetGroup
                                            , PayCycleCode);
            #endregion
            DataSet ds;
            ds = dal.ExecuteDataSet(query);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows != null && ds.Tables[0].Rows.Count > 0)
            {
                return GetValue(ds.Tables[0].Rows[0][0].ToString());
            }
            else
                return string.Empty;
        }

        public DataTable GetProrationRate(string IDNumber, string PayCycle, string ProrationBase, string LCSCCTRMAP, string centralProfile, string companyCode)
        {
            try
            {
                string query = string.Empty;
                string condition = string.Empty;
                DataTable dtResult = new DataTable();
                if (ProrationBase == "F")
                {
                    #region Query
                    if (IDNumber != "")
                        query = string.Format(@"SELECT Tep_CostcenterCode as [Cost center]
                                                , Tep_ProrationRate AS [Proration Percentage]
                                                FROM {2}..T_EmpProration
                                                WHERE ISNULL(Tep_EndCycle, '{0}') >= '{0}'
                                                        AND '{0}' >= Tep_StartCycle
                                                        AND Tep_IDNo = '{1}'"
                                                , PayCycle
                                                , IDNumber
                                                , centralProfile
                                                , companyCode);
                    else
                        query = string.Format(@"SELECT Tep_IDNo as [ID Number]
                                                , Tep_CostcenterCode as [Cost center]
                                                , Tep_ProrationRate AS [Proration Percentage]
                                                FROM {1}..T_EmpProration
                                                WHERE ISNULL(Tep_EndCycle, '{0}') >= '{0}'
                                                        AND '{0}' >= Tep_StartCycle"
                                                , PayCycle
                                                , centralProfile
                                                , companyCode);
                    #endregion
                }
                else
                {
                    #region Query
                    if (IDNumber != "")
                        query = string.Format(@"
                                            SELECT CASE WHEN '{4}' = 'TRUE' THEN Mcc_AlternateAccount 
                                                ELSE Teh_CostcenterCode END AS [Cost center]
                                                , Teh_Hours AS [Hours]
                                                , Teh_ProrationRate AS [Proration Percentage]
                                            FROM T_EmpCostHours
                                            INNER JOIN {3}..M_Costcenter 
                                                ON {5} = Teh_CostcenterCode
                                                AND Mcc_CompanyCode = '{2}'
                                            WHERE Teh_PayCycle = '{0}' AND Teh_IDNo = '{1}' 
                                            ", PayCycle
                                            , IDNumber
                                            , companyCode
                                            , centralProfile
                                            , LCSCCTRMAP, (Convert.ToBoolean(LCSCCTRMAP) ? "Mcc_AlternateAccount" : "Mcc_CostcenterCode"));
                    else
                        query = string.Format(@"
                                            SELECT Teh_IDNo as [ID Number]
                                                , CASE WHEN '{3}' = 'TRUE' THEN Mcc_AlternateAccount 
                                                ELSE Teh_CostcenterCode END AS [Cost center]
                                                , Teh_Hours AS [Hours]
                                                , Teh_ProrationRate AS [Proration Percentage]
                                            FROM T_EmpCostHours
                                            INNER JOIN {2}..M_Costcenter 
                                                ON {4} = Teh_CostcenterCode
                                                AND Mcc_CompanyCode = '{1}'
                                            WHERE Teh_PayCycle = '{0}'"
                                            , PayCycle
                                            , companyCode
                                            , centralProfile
                                            , LCSCCTRMAP
                                            , (Convert.ToBoolean(LCSCCTRMAP) ? "Mcc_AlternateAccount" : "Mcc_CostcenterCode"));
                    #endregion
                }

                DataSet ds = null;
                using (DALHelper dal = new DALHelper())
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(query, CommandType.Text);
                    dal.CloseDB();
                }
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    return ds.Tables[0];
                }
                else
                    return dtResult;
            }
            catch
            {
                return null;
            }
        }

        public DataTable CheckVariance(string PayCycleCode, string CycleType, string companyCode, string centralProfile, string UsrLogin, DALHelper dalHelper)
        {
            #region query
            string query = string.Format(@"
                        INSERT INTO T_EmpCostVariance
                        SELECT						
	                        Tec_IDNo AS [ID Number]	
                            , Tec_PayCycle AS [Pay Cycle]	
                            , '{3}'
                            , GETDATE()
                        ---, Mac_EntrySetCode AS [Entry Set]								
	                    ---, SUM(CASE WHEN (Tec_TotalAmt > 0 AND Mac_AccountNature = 'DR') OR (Tec_TotalAmt < 0 AND Mac_AccountNature = 'CR') THEN ABS(Tec_TotalAmt)  ELSE 0 END) AS [Debit]							
	                    ---, SUM(CASE WHEN (Tec_TotalAmt > 0 AND Mac_AccountNature = 'CR') OR (Tec_TotalAmt < 0 AND Mac_AccountNature = 'DR') THEN ABS(Tec_TotalAmt)  ELSE 0 END) AS [Credit]							
	                    ---, SUM(CASE WHEN (Tec_TotalAmt > 0 AND Mac_AccountNature = 'DR') OR (Tec_TotalAmt < 0 AND Mac_AccountNature = 'CR') THEN ABS(Tec_TotalAmt)  ELSE 0 END) 							
	                    ---   - SUM(CASE WHEN (Tec_TotalAmt > 0 AND Mac_AccountNature = 'CR') OR (Tec_TotalAmt < 0 AND Mac_AccountNature = 'DR') THEN ABS(Tec_TotalAmt)  ELSE 0 END) AS [Variance]							
                    
                    FROM T_EmpCost 								
                    INNER JOIN {0}..M_Cost ON Mlc_PayrollCode = Tec_PayrollCode	
                        AND Mlc_CompanyCode = '{4}'							
                    INNER JOIN {0}..M_Account ON Tec_PayrollCode = Mac_PayrollCode		
                        AND Mac_CompanyCode = '{4}'						
	                    AND Tec_ExpenseGroup  IN (SELECT [Data] FROM {0}.dbo.Udf_Split (Mac_ExpenseGroup, ','))							
                    WHERE Mac_RecordStatus = 'A'								
                    AND Mac_EntrySetCode IN (SELECT Tcc_EntrySetCode FROM T_CostProrationControl 								
						                    WHERE Tcc_CycleType = '{2}'		
							                    AND (Tcc_ApplicablePayCycle = RIGHT('{1}',1) OR Tcc_ApplicablePayCycle = '0')	
							                    AND Tcc_RecordStatus = 'A')	
                    GROUP BY Tec_IDNo	
								, Tec_PayCycle	
								----, Mac_EntrySetCode							
                    HAVING SUM(CASE WHEN (Tec_TotalAmt > 0 AND Mac_AccountNature = 'DR') OR (Tec_TotalAmt < 0 AND Mac_AccountNature = 'CR') THEN ABS(Tec_TotalAmt)  ELSE 0 END) <>								
	                       SUM(CASE WHEN (Tec_TotalAmt > 0 AND Mac_AccountNature = 'CR') OR (Tec_TotalAmt < 0 AND Mac_AccountNature = 'DR') THEN ABS(Tec_TotalAmt)  ELSE 0 END)							
                    
                    SELECT * FROM T_EmpCostVariance 
					WHERE Tev_PayCycle = '{1}'

                    ", centralProfile, PayCycleCode, CycleType, UsrLogin, companyCode);
            #endregion
            DataSet ds = dalHelper.ExecuteDataSet(query, CommandType.Text);

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0];
            }
            else return new DataTable();
        }

        public int PostToAccounting(string PayCycleCode, string centralProfile, string userLogin, string LCSCOMPID, string ACCTGDB, DALHelper dal)
        {
            int retVal = 0;
            #region query
            string query = string.Format(@"DELETE FROM {0}..E_PayrollHeaderEntry
                                            WHERE RTRIM(Prh_CompanyID) = '{2}'
                                            AND RTRIM(Prh_PayrollPeriod) 
						                    IN (
                                                SELECT Tch_PayrollPeriod FROM T_CostHdr
							                    WHERE Tch_PayCycle = '{1}'
                                                    AND Tch_RecordStatus = 'A'
							                    )

                                            DELETE FROM {0}..E_PayrollDetailEntry
                                            WHERE RTRIM(Prd_CompanyID) = '{2}'
                                            AND RTRIM(Prd_PayrollPeriod) 
						                    IN (
                                                SELECT Tch_PayrollPeriod FROM T_CostHdr
							                    WHERE Tch_PayCycle = '{1}'
                                                    AND Tch_RecordStatus = 'A'
							                    )
                                            
                                            INSERT INTO {0}..E_PayrollHeaderEntry 
                                            (Prh_CompanyID
                                                ,Prh_PayrollPeriod
                                                ,Prh_BankID
                                                ,Prh_OrigCurrencyID
                                                ,Prh_DefaultAccountCodeBA
                                                ,Prh_VoucherNo
                                                ,Prh_ProcessingDate
                                                ,Prh_ProcessingFlag
                                                ,Prh_Status
                                                ,Created_ID
                                                ,Created_Date
                                                ,Updated_ID
                                                ,Updated_Date) 
                                            SELECT '{2}'
                                                ,Tch_PayrollPeriod
                                                ,Tch_BankCode
                                                ,'PHP'
                                                ,''
                                                ,''
                                                ,Tch_PostDate
                                                ,0
                                                ,'A'
                                                ,'{3}'
                                                ,GETDATE()
                                                ,'{3}'
                                                ,GETDATE()
                                            FROM T_CostHdr 
                                            WHERE Tch_PayCycle = '{1}'
                                                AND Tch_RecordStatus = 'A'

                                        INSERT INTO {0}..E_PayrollDetailEntry 
                                            (Prd_CompanyID
                                            ,Prd_PayrollPeriod
                                            ,Prd_BankID
                                            ,Prd_PayrollSeqNo
                                            ,Prd_AccountNature
                                            ,Prd_DefaultAccountCode
                                            ,Prd_SubsidiaryCode
                                            ,Prd_SubsidiaryItemCode
                                            ,Prd_OrigCurrencyID
                                            ,Prd_ForexRate
                                            ,Prd_ForexReportRate
                                            ,Prd_ForexDate
                                            ,Prd_OrigAmount
                                            ,Prd_BaseAmount
                                            ,Prd_ReportAmount
                                            ,Prd_Explanation
                                            ,Prd_BatchEntryNo
                                            ,Prd_VoucherNo
                                            ,Prd_Status
                                            ,Created_ID
                                            ,Created_Date
                                            ,Updated_ID
                                            ,Updated_Date)

                                            SELECT '{2}'
                                                , Tch_PayrollPeriod
                                                , Tch_BankCode
                                                , Tcd_LineNo
                                                , Tcd_AccountNature
                                                , Tcd_AccountCode
                                                , Tcd_SubsidiaryCode
                                                , Tcd_SubsidiaryItemCode
                                                , 'PHP'
                                                , Tcd_ForexBaseRate
                                                , Tcd_ForexReportRate
                                                , Tcd_ForexDate
                                                , Tcd_Amt
                                                , Tcd_BaseAmt
                                                , Tcd_ReportAmt
                                                , Tcd_Explanation
                                                , 1
                                                , ''
                                                , 'A'
                                                , '{3}'
                                                , GETDATE()
                                                , '{3}'
                                                , GETDATE()
                                                FROM T_CostDtl
                                                INNER JOIN T_CostHdr ON Tcd_ControlNo = Tch_ControlNo
                                                WHERE Tch_PayCycle = '{1}'
                                                    AND Tch_RecordStatus = 'A'
                                                ORDER BY Tcd_LineNo
                                            
                                            ", ACCTGDB
                                             , PayCycleCode
                                             , LCSCOMPID
                                             , userLogin);
            #endregion
            try
            {
                retVal = dal.ExecuteNonQuery(query);
            }
            catch (Exception e)
            {

                throw new PayrollException(e);
            }
            return retVal;
        }

        public DataSet GetPayrollCodes(string PayCycleCode, string companyCode, string centralProfile, string CycleType, DALHelper dal)
        {
            #region query
            string query = string.Format(@"SELECT * FROM {0}..M_Cost
                                            INNER JOIN T_CostProrationControl ON Tcc_CycleType = '{3}'
                                                AND (Tcc_ApplicablePayCycle = RIGHT('{2}',1) OR Tcc_ApplicablePayCycle = '0')
                                                AND Tcc_RecordStatus = 'A'
                                            WHERE Mlc_CompanyCode = '{1}'
                                                AND Mlc_RecordStatus = 'A'
                                                AND Mlc_PayrollCode IN (SELECT DISTINCT Mac_PayrollCode FROM {0}..M_Account
                                                                            WHERE  Mac_EntrySetCode = Tcc_EntrySetCode 
                                                                                AND Mac_CompanyCode = '{1}'
                                                                                AND Mac_RecordStatus = 'A')", centralProfile
                                                                                                            , companyCode
                                                                                                            , PayCycleCode
                                                                                                            , CycleType);
            #endregion
            DataSet ds = null;
            ds = dal.ExecuteDataSet(query);
            return ds;
        }

        public void CheckPreProcessingLaborCostExists(string profileCode, string centralProfile, string PayCycle, string EmployeeID, string UserLogin, string companyCode, string menucode, DALHelper dalHelper)
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

                            DELETE FROM T_EmpProcessCheck WHERE Tpc_ModuleCode = '{8}' AND Tpc_SystemDefined = 1
                                 AND Tpc_PayCycle = '{3}'
                                 AND ('{2}' = '' OR Tpc_IDNo = '{2}')

                            INSERT INTO T_EmpProcessCheck

                            SELECT '{8}'
                            , Teh_IDNo 
                            , 'BW'
                            , '{3}'
                            , NULL
                            , NULL
                            , CASE WHEN Scm_Status = 'C' THEN 'Costcenter Mapping is inactive ' ELSE 'Costcenter Mapping not set-up - ' END + Teh_CostcenterCode
                            , 1
                            , '{4}'
                            , GETDATE()
                            FROM T_EmpCostHours 
                            INNER JOIN M_Employee ON Teh_IDNo = Mem_IDNo
                            @USERCOSTCENTERACCESSCONDITION
                            LEFT JOIN {5}..T_SubsidiaryCodeMaster ON Scm_CompanyID = '{6}'
	                            AND Scm_SubsidiaryCode = Teh_CostcenterCode
	                            AND Scm_SubsidiaryType ='PCC'
                            WHERE (Scm_Status = 'C' OR Scm_Status IS NULL)
                                AND ('{2}' = '' OR Teh_IDNo = '{2}')
                                AND '{7}' = 'TRUE'

                            UNION ALL

                            SELECT '{8}'
                            , Mem_IDNo 
                            , 'BW'
                            , '{3}'
                            , NULL
                            , NULL
                            , 'ID Number not registered to Subsidiary Master'
                            , 1
                            , '{4}'
                            , GETDATE()
                            FROM M_Employee 
                            @USERCOSTCENTERACCESSCONDITION
                            LEFT JOIN {5}..T_SubsidiaryCodeMaster ON Scm_SubsidiaryCode = Mem_IDNo 
								AND Scm_SubsidiaryClass='EMP' 
								AND Scm_SubsidiaryType = 'EM'
                                AND Scm_CompanyID = '{6}'
                            WHERE Scm_SubsidiaryCode IS NULL
                                AND ('{2}' = '' OR Mem_IDNo = '{2}')
                                AND '{7}' = 'TRUE'

                            UNION ALL

                            SELECT '{8}'
                            , Tpy_IDNo 
                            , 'BW'
                            , '{3}'
                            , NULL
                            , NULL
                             , CASE WHEN Mex_RecordStatus = 'C' THEN 'Expense Class is inactive ' ELSE 'Expense Class not set-up - ' END + Tpy_ExpenseClass
                            , 1
                            , '{4}'
                            , GETDATE()
                            FROM T_EmpPayroll
                            INNER JOIN M_Employee ON Tpy_IDNo = Mem_IDNo
                            @USERCOSTCENTERACCESSCONDITION
                            LEFT JOIN {0}..M_Expense ON Tpy_ExpenseClass = Mex_ExpenseClass
								AND Mex_CompanyCode = '{1}'
                            WHERE (Mex_RecordStatus = 'C' OR Mex_ExpenseClass IS NULL)
                                AND ('{2}' = '' OR Tpy_IDNo = '{2}')

                            UNION ALL

                            SELECT '{8}'
                            , Tpy_IDNo 
                            , 'BW'
                            , '{3}'
                            , ProrationRate.Tep_ProrationRate
                            , NULL
                            ,'Total proration rate is not equal to 100%'
                            , 1
                            , '{4}'
                            , GETDATE()
                            FROM T_EmpPayroll
                            INNER JOIN M_Employee ON Tpy_IDNo = Mem_IDNo
                            @USERCOSTCENTERACCESSCONDITION
                            INNER JOIN (SELECT Tep_IDNo, SUM(Tep_ProrationRate) AS Tep_ProrationRate
								         FROM {0}..T_Empproration
								         WHERE ISNULL(Tep_EndCycle, '{3}') >= '{3}'
								        AND '{3}' >= Tep_StartCycle
								        GROUP BY Tep_IDNo
								        HAVING SUM(Tep_ProrationRate) <> 100) ProrationRate 
                                            ON Tpy_IDNo = ProrationRate.Tep_IDNo
                            WHERE SUBSTRING(Tpy_ExpenseClass,CHARINDEX('-',Tpy_ExpenseClass)+1,LEN(Tpy_ExpenseClass)) = 'F'
                                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                            "
                                , centralProfile
                                , companyCode
                                , EmployeeID
                                , PayCycle
                                , UserLogin
                                , Encrypt.decryptText(ConfigurationManager.AppSettings["AccountingDBName"].ToString())
                                , (new CommonBL()).GetParameterValueFromCentral("LCSCOMPID", companyCode, centralProfile, dalHelper)
                                , Encrypt.decryptText(ConfigurationManager.AppSettings["LinkToAccounting"].ToString())
                                , menucode);
            #endregion
            //query = query.Replace("@CONDITIONS", condition);
            query = query.Replace("@USERCOSTCENTERACCESSCONDITION", (new HRCReportsBL()).UserCostCenterAccessTmpQuery(profileCode, "PAYROLL", UserLogin, "Mem_CostcenterCode", "Mem_PayrollGroup", "Mem_EmploymentStatusCode", "Mem_PayrollType", companyCode, centralProfile, false));
            dalHelper.ExecuteNonQuery(query);
        }

        public void CheckPostProcessingLaborCostExists(string profileCode, string centralProfile, string PayCycle, string EmployeeID, string UserLogin, string companyCode, string cycleType, string menucode, DALHelper dalHelper)
        {
            #region Query
            string condition = string.Empty;
            string query = string.Format(@"
            INSERT INTO T_EmpProcessCheck

            SELECT '{5}'
							, Tpy_IDNo 
                            , 'AW'
                            , '{3}'
                            , NULL
                            , NULL
                            , 'No hours basis for proration -' + Mex_ExpenseName 
                            , 1
                            , '{4}'
                            , GETDATE()
                            FROM T_EmpPayroll 
                            INNER JOIN M_Employee ON Tpy_IDNo = Mem_IDNo
                            @USERCOSTCENTERACCESSCONDITION
							INNER JOIN {0}..M_Expense ON Tpy_ExpenseClass = Mex_ExpenseClass
								AND Mex_CompanyCode = '{1}'
								AND Mex_IsClassCommon = 1
							LEFT JOIN (
										SELECT Teh_IDNo, SUM(Teh_Hours) AS Teh_Hours
										FROM T_EmpCostHours 
										WHERE Teh_PayCycle = '{3}'
										GROUP BY Teh_IDNo
										) tmp ON Teh_IDNo = Tpy_IDNo
                            WHERE (Teh_Hours IS NULL)
                                AND SUBSTRING(Tpy_ExpenseClass,CHARINDEX('-',Tpy_ExpenseClass)+1,LEN(Tpy_ExpenseClass)) <> 'F'
                                AND Tpy_PayCycle = '{3}'
                                AND ('{2}' = '' OR Tpy_IDNo = '{2}')

                            UNION ALL

                            SELECT '{5}'
                            , Tpy_IDNo 
                            , 'AW'
                            , '{3}'
                            , ProrationRate.Teh_ProrationRate
                            , NULL
                            ,'Total proration rate is not equal to 100%'
                            , 1
                            , '{4}'
                            , GETDATE()
                            FROM T_EmpPayroll
                            INNER JOIN M_Employee ON Tpy_IDNo = Mem_IDNo
                            @USERCOSTCENTERACCESSCONDITION
                            INNER JOIN {0}..M_Expense ON Tpy_ExpenseClass = Mex_ExpenseClass
								AND Mex_CompanyCode = '{1}'
								AND Mex_IsClassCommon = 1
                            INNER JOIN (SELECT Teh_IDNo, SUM(Teh_ProrationRate) AS Teh_ProrationRate
								         FROM T_EmpCostHours
								         WHERE Teh_PayCycle = '{3}'
								         GROUP BY Teh_IDNo
								         HAVING SUM(Teh_ProrationRate) <> 100) ProrationRate 
                                            ON ProrationRate.Teh_IDNo = Tpy_IDNo
                            WHERE Tpy_PayCycle = '{3}'
                                AND ('{2}' = '' OR Tpy_IDNo = '{2}')
                            
                            UNION ALL

                            SELECT '{5}'
							    , Tec_IDNo 
                                , 'AE'
                                , '{3}'
                                , SUM(CASE WHEN (Tec_TotalAmt > 0 AND Mac_AccountNature = 'DR') OR (Tec_TotalAmt < 0 AND Mac_AccountNature = 'CR') THEN ABS(Tec_TotalAmt)  ELSE 0 END)
                                , SUM(CASE WHEN (Tec_TotalAmt > 0 AND Mac_AccountNature = 'CR') OR (Tec_TotalAmt < 0 AND Mac_AccountNature = 'DR') THEN ABS(Tec_TotalAmt)  ELSE 0 END)
                                , 'Not Balanced'
                                , 1
                                , '{4}'
                                , GETDATE()
                        FROM T_EmpCost 	
                        INNER JOIN M_Employee ON Tec_IDNo = Mem_IDNo	
                    	@USERCOSTCENTERACCESSCONDITION					
                        INNER JOIN {0}..M_Cost ON Mlc_PayrollCode = Tec_PayrollCode	
                            AND Mlc_CompanyCode = '{1}'							
                        INNER JOIN {0}..M_Account ON Tec_PayrollCode = Mac_PayrollCode		
                            AND Mac_CompanyCode = '{1}'						
	                        AND Tec_ExpenseGroup  IN (SELECT [Data] FROM {0}.dbo.Udf_Split (Mac_ExpenseGroup, ','))							
                        WHERE Mac_RecordStatus = 'A'								
                            AND Mac_EntrySetCode IN (SELECT Tcc_EntrySetCode FROM T_CostProrationControl 								
						                            WHERE Tcc_CycleType = '{6}'		
							                            AND (Tcc_ApplicablePayCycle = RIGHT('{3}',1) OR Tcc_ApplicablePayCycle = '0')	
							                            AND Tcc_RecordStatus = 'A')	
                            AND Tec_PayCycle = '{3}'
                            AND ('{2}' = '' OR Tec_IDNo = '{2}')
                        GROUP BY Tec_IDNo				
                        HAVING SUM(CASE WHEN (Tec_TotalAmt > 0 AND Mac_AccountNature = 'DR') OR (Tec_TotalAmt < 0 AND Mac_AccountNature = 'CR') THEN ABS(Tec_TotalAmt)  ELSE 0 END) <>								
	                           SUM(CASE WHEN (Tec_TotalAmt > 0 AND Mac_AccountNature = 'CR') OR (Tec_TotalAmt < 0 AND Mac_AccountNature = 'DR') THEN ABS(Tec_TotalAmt)  ELSE 0 END)			
           "
           , centralProfile, companyCode, EmployeeID, PayCycle, UserLogin, menucode, cycleType);
            #endregion
            //query = query.Replace("@CONDITIONS", condition);
            query = query.Replace("@USERCOSTCENTERACCESSCONDITION", (new HRCReportsBL()).UserCostCenterAccessTmpQuery(profileCode, "PAYROLL", UserLogin, "Mem_CostcenterCode", "Mem_PayrollGroup", "Mem_EmploymentStatusCode", "Mem_PayrollType", CompanyCode, CentralProfile, false));
            dalHelper.ExecuteNonQuery(query);
        }

        public DataTable LaborCostExceptionList(string profileCode, string centralProfile, string PayCycle, string EmployeeID, string userlogin, string companyCode, string ErrorType, string menucode, DALHelper dalHelper)
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
                                    , ISNULL(Mex_IsClassCommon,0) as [Is Class Common]
                                    , Tpc_NumValue1 AS [Summary Amount]
                                    , Tpc_NumValue2 AS [Detail Amount]
                                    , Tpc_Remarks AS [Remarks]
                                    FROM (SELECT * FROM T_EmpProcessCheck 
                                          UNION ALL
                                          SELECT * FROM T_EmpProcessCheckHst ) Tmp
									INNER JOIN M_Employee ON Mem_IDNo = Tpc_IDNo
                                    @USERCOSTCENTERACCESSCONDITION
                                    LEFT JOIN {3}..M_Expense ON Mem_ExpenseClass = Mex_ExpenseClass
							            AND Mex_CompanyCode = '{2}'   
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

        public decimal GetForexRate(string Currency)
        {
            decimal dForexRate = 0;
            DateTime dtForexDate = DateTime.Now;
            if (!PostDate.Equals(""))
                dtForexDate = Convert.ToDateTime(PostDate);

            ParameterInfo[] paramForexRate = new ParameterInfo[2];
            paramForexRate[0] = new ParameterInfo("@ForexDate", dtForexDate, SqlDbType.Date);
            paramForexRate[1] = new ParameterInfo("@BaseCurrencyID", Currency);

            string query = string.Format(@"SELECT Fde_ForexRate 
                                           FROM {0}..T_ForexDailyEntry
                                           WHERE Fde_ForexDate = @ForexDate
                                                 AND (Fde_OrigCurrencyID = 'PHP'
                                                      OR Fde_BaseCurrencyID = 'PHP')
                                                 AND (Fde_OrigCurrencyID = @BaseCurrencyID
                                                      OR Fde_BaseCurrencyID = @BaseCurrencyID)
                                                 AND Fde_Status = 'A'", ACCOUNTINGDB);

            dForexRate = GetFormulaQueryDecimalValue(query, paramForexRate, dal);
            return dForexRate;
        }

        #endregion

        #region Reports
        public void InitializeLaborCostReport()
        {
            listLaborCostRept.Clear();
        }

        public void AddErrorToLaborCostReport(string strEmployeeId, string Type, string PayCycle, decimal Amount, decimal Amount2, string strRemarks)
        {
            listLaborCostRept.Add(new structLaborCostErrorReport(strEmployeeId, Type, PayCycle, Amount, Amount2, strRemarks));
        }

        private DataTable SaveLaborCostErrorReportList()
        {
            DataTable dtErrList = new DataTable();
            if (dtErrList.Columns.Count == 0)
            {
                dtErrList.Columns.Add("ID Number");
                dtErrList.Columns.Add("Type");
                dtErrList.Columns.Add("Amount1");
                dtErrList.Columns.Add("Amount2");
                dtErrList.Columns.Add("Remarks");
            }
            for (int i = 0; i < listLaborCostRept.Count; i++)
            {
                dtErrList.Rows.Add();
                dtErrList.Rows[dtErrList.Rows.Count - 1]["ID Number"] = listLaborCostRept[i].strEmployeeId;
                dtErrList.Rows[dtErrList.Rows.Count - 1]["Type"] = listLaborCostRept[i].strType;
                dtErrList.Rows[dtErrList.Rows.Count - 1]["Amount1"] = string.Format("{0:0.00}", listLaborCostRept[i].dAmount);
                dtErrList.Rows[dtErrList.Rows.Count - 1]["Amount2"] = string.Format("{0:0.00}", listLaborCostRept[i].dAmount2);
                dtErrList.Rows[dtErrList.Rows.Count - 1]["Remarks"] = listLaborCostRept[i].strRemarks;
            }

            return dtErrList;
        }

        public void InsertToEmpLaborCostCheckTable(DataTable dt)
        {
            string qString = "";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                #region Insert to Cost Check Error
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
                                                               , dt.Rows[i]["ID Number"].ToString()
                                                               , dt.Rows[i]["Type"].ToString()
                                                               , PayCycleCode
                                                               , dt.Rows[i]["Amount1"].ToString()
                                                               , dt.Rows[i]["Amount2"].ToString()
                                                               , dt.Rows[i]["Remarks"].ToString().Replace("'", "")
                                                               , LoginUser);
                dal.ExecuteNonQuery(qString);
                #endregion
            }
        }
        #endregion
    }
}
