using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using CommonLibrary;
using Payroll.DAL;
using System.Collections;
using Payroll;
using System.Configuration;

namespace Payroll.BLogic
{
    public class NewLaborHoursAdjustmentBL : BaseBL
    {
        #region <Global Variables>
        DALHelper dal;
        SystemCycleProcessingBL SystemCycleProcessingBL;
        CommonBL commonBL;
        NewLaborHoursGenerationBL newLaborHoursGen;
        NewPayrollCalculationBL NewPayrollCalculationBL;

        //Constants
        public int FILLERCNT = 6;

        //Storage tables
        DataTable dtLaborHrsAdjustment = null;
        DataTable dtLaborHrsAdjustmentExt = null;
        DataTable dtLaborHrsAdjustmentExt2 = null;
        DataTable dtLaborHrsAdjustment2 = null;
        DataTable dtEmployeePayrollTransactionHist = null;
        DataTable dtEmpPayTranHdrTrl = null;

        DataRow drLaborHrsAdjustment = null;
        DataRow drLaborHrsAdjustment2 = null;
        DataRow drLaborHrsAdjustmentExt = null;
        
        DataRow drLaborHrsAdjustmentExt2 = null;
        DataRow[] drEmpPayTranDtlHst = null;

        //Miscellaneous
        string ProcessPayrollPeriod     = "";
        string PayrollStart             = "";
        string PayrollEnd               = "";
        string LoginUser                = "";
        string CompanyCode              = "";
        string CentralProfile           = "";
        string DBCollation              = "";
        bool bHasDayCodeExt             = false;
        string EmployeeList             = string.Empty;

        //Flags and parameters
        public decimal MDIVISOR = 0;
        public decimal HRLYRTEDEC = 0;
        //public string HRRTINCDEM = ""; Change to PAYSPECIALRATE
        public DataTable PAYCODERATE = null;
        public string PAYSPECIALRATE = "";
        public string SPECIALRATEFORMULA_SRATE = "";
        public string SPECIALRATEFORMULA_HRATE = "";
        public int NDBRCKTCNT = 1;
        public decimal NP1_RATE = 0;
        public decimal NP2_RATE = 0;
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

        #region Event handlers for labor hours adjustment process
        public delegate void EmpDispEventHandler(object sender, EmpDispEventArgs e);
        public class EmpDispEventArgs : System.EventArgs
        {
            public string EmployeeId;
            public string LastName;
            public string FirstName;
            public string Process;

            public EmpDispEventArgs(string strEmployeeId, string strLast, string strFirst)
            {
                EmployeeId = strEmployeeId;
                LastName = strLast;
                FirstName = strFirst;
                Process = "ADJUSTMENT";
            }

            public EmpDispEventArgs(string strEmployeeId, string strLast, string strFirst, string strProcess)
            {
                EmployeeId = strEmployeeId;
                LastName = strLast;
                FirstName = strFirst;
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

        public void AdjustLaborHours(string PayrollPeriod, string EmployeeId, string UserLogin, string CompanyCode, string CentralProfile, string dbCollation, DALHelper dalHelper)
        {
            AdjustLaborHours(false, PayrollPeriod, EmployeeId, UserLogin, CompanyCode, CentralProfile, dbCollation, dalHelper);
        }

        public void AdjustLaborHours(string PayrollPeriod, string UserLogin, string CompanyCode, string CentralProfile, string dbCollation, DALHelper dalHelper, string EmployeeList)
        {
            this.EmployeeList = EmployeeList;
            AdjustLaborHours(true, PayrollPeriod, "", UserLogin, CompanyCode, CentralProfile, dbCollation, dalHelper);
        }

        public void AdjustLaborHours(bool ProcessAll, string PayrollPeriod, string EmployeeId, string UserLogin, string companyCode, string centralProfile,string dbCollation, DALHelper dalHelper)
        {
            #region Variables
            DataTable dtEmpForAdjustment        = null;
            DataTable dtAllowances              = null;
            DataRow[] drArrAllowance            = null;
            DataTable dtDayCodePremiums         = null;
            DataTable dtDayCodePremiumFillers   = null;
            //DataRow[] drDayCodePremium          = null;
            DataRow[] drDayCodePremiumFiller    = null;
            string strEmployeeID                = "";
            string strPrevEmployeeID            = "";
            string strProcessDate;

            //Regular and Overtime Rates
            decimal Reg                         = 0;
            decimal RegOT                       = 0;
            decimal Rest                        = 0;
            decimal RestOT                      = 0;
            decimal Hol                         = 0;
            decimal HolOT                       = 0;
            decimal SPL                         = 0;
            decimal SPLOT                       = 0;
            decimal PSD                         = 0;
            decimal PSDOT                       = 0;
            decimal Comp                        = 0;
            decimal CompOT                      = 0;
            decimal RestHol                     = 0;
            decimal RestHolOT                   = 0;
            decimal RestSPL                     = 0;
            decimal RestSPLOT                   = 0;
            decimal RestComp                    = 0;
            decimal RestCompOT                  = 0;
            decimal RestPSD                     = 0;
            decimal RestPSDOT                   = 0;

            //Regular Night Premium and Overtime Night Premium Rates
            decimal RegND                       = 0;
            decimal RegOTND                     = 0;
            decimal RestND                      = 0;
            decimal RestOTND                    = 0;
            decimal HolND                       = 0;
            decimal HolOTND                     = 0;
            decimal SPLND                       = 0;
            decimal SPLOTND                     = 0;
            decimal PSDND                       = 0;
            decimal PSDOTND                     = 0;
            decimal CompND                      = 0;
            decimal CompOTND                    = 0;
            decimal RestHolND                   = 0;
            decimal RestHolOTND                 = 0;
            decimal RestSPLND                   = 0;
            decimal RestSPLOTND                 = 0;
            decimal RestCompND                  = 0;
            decimal RestCompOTND                = 0;
            decimal RestPSDND                   = 0;
            decimal RestPSDOTND                 = 0;

            //Regular Night Premium and Overtime Night Premium Percentages
            decimal RegNDPercent                = 0;
            decimal RegOTNDPercent              = 0;
            decimal RestNDPercent               = 0;
            decimal RestOTNDPercent             = 0;
            decimal HolNDPercent                = 0;
            decimal HolOTNDPercent              = 0;
            decimal SPLNDPercent                = 0;
            decimal SPLOTNDPercent              = 0;
            decimal PSDNDPercent                = 0;
            decimal PSDOTNDPercent              = 0;
            decimal CompNDPercent               = 0;
            decimal CompOTNDPercent             = 0;
            decimal RestHolNDPercent            = 0;
            decimal RestHolOTNDPercent          = 0;
            decimal RestSPLNDPercent            = 0;
            decimal RestSPLOTNDPercent          = 0;
            decimal RestCompNDPercent           = 0;
            decimal RestCompOTNDPercent         = 0;
            decimal RestPSDNDPercent            = 0;
            decimal RestPSDOTNDPercent          = 0;

            string fillerHrCol                  = "";
            string fillerOTHrCol                = "";
            string fillerNDHrCol                = "";
            string fillerOTNDHrCol              = "";
            string fillerAmtCol                 = "";
            string fillerOTAmtCol               = "";
            string fillerNDAmtCol               = "";
            string fillerOTNDAmtCol             = "";

            string strAllowanceCol              = "";
            decimal dAlwAmt                     = 0;
            string PayrollType                  = "";
            string PremiumGroup                 = "";
            string EmploymentStatus             = "";
            decimal SalaryRate                  = 0;
            decimal HourlyRate                  = 0;
            decimal SpecialSalaryRate           = 0;
            decimal SpecialHourlyRate           = 0;
            double dEquivShiftHoursTrail        = 0;
            double dEquivShiftHoursHist         = 0;
            double dShiftHoursHist              = 0;
            #endregion

            try
            {
                this.dal = dalHelper;
                SystemCycleProcessingBL = new SystemCycleProcessingBL(dal, PayrollPeriod, UserLogin, companyCode, centralProfile);
                newLaborHoursGen = new NewLaborHoursGenerationBL();
                NewPayrollCalculationBL = new NewPayrollCalculationBL();
                newLaborHoursGen.EmpDispHandler += new NewLaborHoursGenerationBL.EmpDispEventHandler(ShowEmployeeName);
                newLaborHoursGen.StatusHandler += new NewLaborHoursGenerationBL.StatusEventHandler(ShowLaborHourStatus);
                commonBL = new CommonBL();

                ProcessPayrollPeriod = PayrollPeriod;
                LoginUser = UserLogin;
                CompanyCode = companyCode;
                CentralProfile = centralProfile;
                DBCollation = dbCollation;
                //-----------------------------
                //Check for Existing Day Codes
                if (commonBL.GetFillerDayCodesCount(companyCode, centralProfile, dal) > 0)
                {
                    bHasDayCodeExt = true;
                    dtDayCodePremiumFillers = commonBL.GetDayCodePremiumFillers(companyCode, centralProfile, dal);
                }
                else
                {
                    bHasDayCodeExt = false;
                    dtDayCodePremiumFillers = null;
                }
                //-----------------------------
                SetProcessFlags();
                //-----------------------------
               
                DataTable dtPayPeriod = commonBL.GetPayCycleStartendDate(ProcessPayrollPeriod);
                if (dtPayPeriod.Rows.Count > 0)
                {
                    PayrollStart = dtPayPeriod.Rows[0][1].ToString();
                    PayrollEnd   = dtPayPeriod.Rows[0][2].ToString();
                }
                //-----------------------------
                dtLaborHrsAdjustment            = DbRecord.GenerateTable(CommonConstants.TableName.T_EmpSystemAdj);
                dtLaborHrsAdjustmentExt         = DbRecord.GenerateTable(CommonConstants.TableName.T_EmpSystemAdjMisc);
                dtLaborHrsAdjustment2           = DbRecord.GenerateTable(CommonConstants.TableName.T_EmpSystemAdj2);
                dtLaborHrsAdjustmentExt2        = DbRecord.GenerateTable(CommonConstants.TableName.T_EmpSystemAdjMisc2);
                //-----------------------------
                //Delete labor hours adjustment record first
                StatusHandler(this, new StatusEventArgs("Cleaning Adjustment Tables", false));
                ClearAdjustmentTable(ProcessAll, EmployeeId);
                StatusHandler(this, new StatusEventArgs("Cleaning Adjustment Tables", true));
                //-----------------------------
                //Get all allowances
                dtAllowances = commonBL.GetAllowanceHeader(companyCode, centralProfile, dal);
                //Get day code premiums
                dtDayCodePremiums = NewPayrollCalculationBL.GetDayCodePremiums(CompanyCode, CentralProfile, dal);
                //-----------------------------
                //StatusHandler(this, new StatusEventArgs("Initialize Labor Hours Generation", false));
                dtEmpForAdjustment = GetAllEmployeesForAdjustmentMultiPockets(ProcessAll, EmployeeId);
                //StatusHandler(this, new StatusEventArgs("Initialize Labor Hours Generation", true));
                DataView dvEmpWithAdj = new DataView(dtEmpForAdjustment);
                DataTable dtEmpWithAdj = dvEmpWithAdj.ToTable(true, "Ttr_PayCycle");
                DataRow[] drArrEmpWithAdj;
                //newLaborHoursGen.SetProcessFlags(CompanyCode, CentralProfile, dalHelper);
                string strEmpList;
                for (int i = 0; i < dtEmpWithAdj.Rows.Count; i++)
                {
                    drArrEmpWithAdj = dtEmpForAdjustment.Select("Ttr_PayCycle = '" + dtEmpWithAdj.Rows[i]["Ttr_PayCycle"].ToString() + "'");
                    if (drArrEmpWithAdj.Length > 0)
                    {
                        strEmpList = "";
                        foreach (DataRow drRow in drArrEmpWithAdj)
                        {
                            strEmpList += string.Format("'{0}',", drRow["Ttr_IDNo"].ToString());
                            //EmpDispHandler(this, new EmpDispEventArgs(drRow["Ttr_IDNo"].ToString(), drRow["Mem_LastName"].ToString(), drRow["Mem_FirstName"].ToString()));
                        }
                        if (strEmpList != "")
                        {
                            strEmpList = strEmpList.Substring(0, strEmpList.Length - 1);
                            newLaborHoursGen.GenerateLaborHours(true, false, false, dtEmpWithAdj.Rows[i]["Ttr_PayCycle"].ToString(), "", false, "", strEmpList, UserLogin, CompanyCode, CentralProfile, DBCollation, dalHelper);
                        }
                    }
                }
                //-----------------------------
                StatusHandler(this, new StatusEventArgs("Extracting Time & Attendance Records with Adjustments", false));
                dtEmpPayTranHdrTrl = GetEmployeePayrollTransactionTrailRecordsMultiPockets(ProcessAll, EmployeeId, dalHelper);
                dtEmployeePayrollTransactionHist = GetEmployeePayrollTransactionHistRecordsMultiPockets(ProcessAll, EmployeeId, dalHelper);
                StatusHandler(this, new StatusEventArgs("Extracting Time & Attendance Records with Adjustments", true));
                //-----------------------------
                if (dtEmpPayTranHdrTrl.Rows.Count > 0)
                {
                    StatusHandler(this, new StatusEventArgs("Computing Adjustment Amount", false));
                    StatusHandler(this, new StatusEventArgs("Computing Adjustment Amount", true));
                    for (int i = 0; i < dtEmpPayTranHdrTrl.Rows.Count; i++)
                    {
                        try
                        {
                            strEmployeeID = dtEmpPayTranHdrTrl.Rows[i]["Tpd_IDNo"].ToString();
                            strProcessDate = dtEmpPayTranHdrTrl.Rows[i]["Tpd_Date"].ToString();

                            if (strEmployeeID != strPrevEmployeeID)
                                EmpDispHandler(this, new EmpDispEventArgs(dtEmpPayTranHdrTrl.Rows[i]["Tpd_IDNo"].ToString(), dtEmpPayTranHdrTrl.Rows[i]["Mem_LastName"].ToString(), dtEmpPayTranHdrTrl.Rows[i]["Mem_FirstName"].ToString()));

                            //Get Hist Detail record
                            drEmpPayTranDtlHst = dtEmployeePayrollTransactionHist.Select("Tpd_IDNo = '" + strEmployeeID + "' AND Tpd_Date = '" + strProcessDate + "'");

                            #region Initialize Labor Hours Adjustment and Extension Rows
                            drLaborHrsAdjustment = dtLaborHrsAdjustment.NewRow();
                            drLaborHrsAdjustment2 = dtLaborHrsAdjustment2.NewRow();
                            if (bHasDayCodeExt)
                            {
                                drLaborHrsAdjustmentExt = dtLaborHrsAdjustmentExt.NewRow();
                                for (int j = 1; j <= FILLERCNT; j++)
                                {
                                    //initialize
                                    fillerHrCol = string.Format("Tsm_Misc{0:0}Hr", j);
                                    fillerOTHrCol = string.Format("Tsm_Misc{0:0}OTHr", j);
                                    fillerNDHrCol = string.Format("Tsm_Misc{0:0}NDHr", j);
                                    fillerOTNDHrCol = string.Format("Tsm_Misc{0:0}NDOTHr", j);
                                    drLaborHrsAdjustmentExt[fillerHrCol] = 0;
                                    drLaborHrsAdjustmentExt[fillerOTHrCol] = 0;
                                    drLaborHrsAdjustmentExt[fillerNDHrCol] = 0;
                                    drLaborHrsAdjustmentExt[fillerOTNDHrCol] = 0;
                                }
                                drLaborHrsAdjustmentExt2 = dtLaborHrsAdjustmentExt2.NewRow();
                                for (int j = 1; j <= FILLERCNT; j++)
                                {
                                    //initialize
                                    fillerHrCol = string.Format("Tsm_Misc{0:0}Amt", j);
                                    fillerOTHrCol = string.Format("Tsm_Misc{0:0}OTAmt", j);
                                    fillerNDHrCol = string.Format("Tsm_Misc{0:0}NDAmt", j);
                                    fillerOTNDHrCol = string.Format("Tsm_Misc{0:0}NDOTAmt", j);
                                    drLaborHrsAdjustmentExt2[fillerHrCol] = 0;
                                    drLaborHrsAdjustmentExt2[fillerOTHrCol] = 0;
                                    drLaborHrsAdjustmentExt2[fillerNDHrCol] = 0;
                                    drLaborHrsAdjustmentExt2[fillerOTNDHrCol] = 0;
                                }
                            }
                            #endregion

                            #region Update Labor Hours Adjustment Row (Part 1)
                            drLaborHrsAdjustment["Tsa_LTHr"]                    = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_LTHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_LTHr"]);
                            drLaborHrsAdjustment["Tsa_UTHr"]                    = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_UTHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_UTHr"]);
                            drLaborHrsAdjustment["Tsa_UPLVHr"]                  = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_UPLVHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_UPLVHr"]);
                            drLaborHrsAdjustment["Tsa_ABSLEGHOLHr"]             = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_ABSLEGHOLHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_ABSLEGHOLHr"]);
                            drLaborHrsAdjustment["Tsa_ABSSPLHOLHr"]             = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_ABSSPLHOLHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_ABSSPLHOLHr"]);
                            drLaborHrsAdjustment["Tsa_ABSCOMPHOLHr"]            = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_ABSCOMPHOLHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_ABSCOMPHOLHr"]);
                            drLaborHrsAdjustment["Tsa_ABSPSDHr"]                = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_ABSPSDHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_ABSPSDHr"]);
                            drLaborHrsAdjustment["Tsa_ABSOTHHOLHr"]             = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_ABSOTHHOLHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_ABSOTHHOLHr"]);
                            drLaborHrsAdjustment["Tsa_WDABSHr"]                 = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_WDABSHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_WDABSHr"]);
                            drLaborHrsAdjustment["Tsa_LTUTMaxHr"]               = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_LTUTMaxHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_LTUTMaxHr"]);
                            drLaborHrsAdjustment["Tsa_ABSHr"]                   = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_ABSHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_ABSHr"]);
                            drLaborHrsAdjustment["Tsa_REGHr"]                   = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_REGHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_REGHr"]);
                            drLaborHrsAdjustment["Tsa_PDLVHr"]                  = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_PDLVHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_PDLVHr"]);
                            drLaborHrsAdjustment["Tsa_PDLEGHOLHr"]              = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_PDLEGHOLHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_PDLEGHOLHr"]);
                            drLaborHrsAdjustment["Tsa_PDSPLHOLHr"]              = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_PDSPLHOLHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_PDSPLHOLHr"]);
                            drLaborHrsAdjustment["Tsa_PDCOMPHOLHr"]             = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_PDCOMPHOLHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_PDCOMPHOLHr"]);
                            drLaborHrsAdjustment["Tsa_PDPSDHr"]                 = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_PDPSDHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_PDPSDHr"]);
                            drLaborHrsAdjustment["Tsa_PDOTHHOLHr"]              = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_PDOTHHOLHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_PDOTHHOLHr"]);
                            drLaborHrsAdjustment["Tsa_PDRESTLEGHOLHr"]          = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_PDRESTLEGHOLHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_PDRESTLEGHOLHr"]);
                            drLaborHrsAdjustment["Tsa_REGOTHr"]                 = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_REGOTHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_REGOTHr"]);
                            drLaborHrsAdjustment["Tsa_REGNDHr"]                 = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_REGNDHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_REGNDHr"]);
                            drLaborHrsAdjustment["Tsa_REGNDOTHr"]               = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_REGNDOTHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_REGNDOTHr"]);
                            drLaborHrsAdjustment["Tsa_RESTHr"]                  = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_RESTHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_RESTHr"]);
                            drLaborHrsAdjustment["Tsa_RESTOTHr"]                = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_RESTOTHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_RESTOTHr"]);
                            drLaborHrsAdjustment["Tsa_RESTNDHr"]                = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_RESTNDHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_RESTNDHr"]);
                            drLaborHrsAdjustment["Tsa_RESTNDOTHr"]              = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_RESTNDOTHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_RESTNDOTHr"]);
                            drLaborHrsAdjustment["Tsa_LEGHOLHr"]                = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_LEGHOLHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_LEGHOLHr"]);
                            drLaborHrsAdjustment["Tsa_LEGHOLOTHr"]              = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_LEGHOLOTHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_LEGHOLOTHr"]);
                            drLaborHrsAdjustment["Tsa_LEGHOLNDHr"]              = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_LEGHOLNDHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_LEGHOLNDHr"]);
                            drLaborHrsAdjustment["Tsa_LEGHOLNDOTHr"]            = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_LEGHOLNDOTHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_LEGHOLNDOTHr"]);
                            drLaborHrsAdjustment["Tsa_SPLHOLHr"]                = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_SPLHOLHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_SPLHOLHr"]);
                            drLaborHrsAdjustment["Tsa_SPLHOLOTHr"]              = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_SPLHOLOTHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_SPLHOLOTHr"]);
                            drLaborHrsAdjustment["Tsa_SPLHOLNDHr"]              = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_SPLHOLNDHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_SPLHOLNDHr"]);
                            drLaborHrsAdjustment["Tsa_SPLHOLNDOTHr"]            = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_SPLHOLNDOTHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_SPLHOLNDOTHr"]);
                            drLaborHrsAdjustment["Tsa_PSDHr"]                   = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_PSDHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_PSDHr"]);
                            drLaborHrsAdjustment["Tsa_PSDOTHr"]                 = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_PSDOTHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_PSDOTHr"]);
                            drLaborHrsAdjustment["Tsa_PSDNDHr"]                 = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_PSDNDHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_PSDNDHr"]);
                            drLaborHrsAdjustment["Tsa_PSDNDOTHr"]               = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_PSDNDOTHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_PSDNDOTHr"]);
                            drLaborHrsAdjustment["Tsa_COMPHOLHr"]               = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_COMPHOLHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_COMPHOLHr"]);
                            drLaborHrsAdjustment["Tsa_COMPHOLOTHr"]             = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_COMPHOLOTHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_COMPHOLOTHr"]);
                            drLaborHrsAdjustment["Tsa_COMPHOLNDHr"]             = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_COMPHOLNDHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_COMPHOLNDHr"]);
                            drLaborHrsAdjustment["Tsa_COMPHOLNDOTHr"]           = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_COMPHOLNDOTHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_COMPHOLNDOTHr"]);
                            drLaborHrsAdjustment["Tsa_RESTLEGHOLHr"]            = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_RESTLEGHOLHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_RESTLEGHOLHr"]);
                            drLaborHrsAdjustment["Tsa_RESTLEGHOLOTHr"]          = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_RESTLEGHOLOTHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_RESTLEGHOLOTHr"]);
                            drLaborHrsAdjustment["Tsa_RESTLEGHOLNDHr"]          = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_RESTLEGHOLNDHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_RESTLEGHOLNDHr"]);
                            drLaborHrsAdjustment["Tsa_RESTLEGHOLNDOTHr"]        = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_RESTLEGHOLNDOTHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_RESTLEGHOLNDOTHr"]);
                            drLaborHrsAdjustment["Tsa_RESTSPLHOLHr"]            = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_RESTSPLHOLHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_RESTSPLHOLHr"]);
                            drLaborHrsAdjustment["Tsa_RESTSPLHOLOTHr"]          = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_RESTSPLHOLOTHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_RESTSPLHOLOTHr"]);
                            drLaborHrsAdjustment["Tsa_RESTSPLHOLNDHr"]          = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_RESTSPLHOLNDHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_RESTSPLHOLNDHr"]);
                            drLaborHrsAdjustment["Tsa_RESTSPLHOLNDOTHr"]        = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_RESTSPLHOLNDOTHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_RESTSPLHOLNDOTHr"]);
                            drLaborHrsAdjustment["Tsa_RESTCOMPHOLHr"]           = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_RESTCOMPHOLHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_RESTCOMPHOLHr"]);
                            drLaborHrsAdjustment["Tsa_RESTCOMPHOLOTHr"]         = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_RESTCOMPHOLOTHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_RESTCOMPHOLOTHr"]);
                            drLaborHrsAdjustment["Tsa_RESTCOMPHOLNDHr"]         = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_RESTCOMPHOLNDHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_RESTCOMPHOLNDHr"]);
                            drLaborHrsAdjustment["Tsa_RESTCOMPHOLNDOTHr"]       = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_RESTCOMPHOLNDOTHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_RESTCOMPHOLNDOTHr"]);
                            drLaborHrsAdjustment["Tsa_RESTPSDHr"]               = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_RESTPSDHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_RESTPSDHr"]);
                            drLaborHrsAdjustment["Tsa_RESTPSDOTHr"]             = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_RESTPSDOTHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_RESTPSDOTHr"]);
                            drLaborHrsAdjustment["Tsa_RESTPSDNDHr"]             = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_RESTPSDNDHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_RESTPSDNDHr"]);
                            drLaborHrsAdjustment["Tsa_RESTPSDNDOTHr"]           = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_RESTPSDNDOTHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_RESTPSDNDOTHr"]);
                            drLaborHrsAdjustment["Tsa_PostFlag"]                = 0;
                            drLaborHrsAdjustment["Usr_Login"]                   = LoginUser;
                            drLaborHrsAdjustment["Ludatetime"]                  = DateTime.Now;
                            #endregion

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
                            RegND               = 0;
                            RegOTND             = 0;
                            RestND              = 0;
                            RestOTND            = 0;
                            HolND               = 0;
                            HolOTND             = 0;
                            SPLND               = 0;
                            SPLOTND             = 0;
                            PSDND               = 0;
                            PSDOTND             = 0;
                            CompND              = 0;
                            CompOTND            = 0;
                            RestHolND           = 0;
                            RestHolOTND         = 0;
                            RestSPLND           = 0;
                            RestSPLOTND         = 0;
                            RestCompND          = 0;
                            RestCompOTND        = 0;
                            RestPSDND           = 0;
                            RestPSDOTND         = 0;

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

                            #region Update Labor Hours Adjustment Row (Part 2)
                            drLaborHrsAdjustment["Tsa_IDNo"]                    = strEmployeeID;
                            drLaborHrsAdjustment["Tsa_AdjPayCycle"]             = dtEmpPayTranHdrTrl.Rows[i]["Tpd_AdjPayCycle"].ToString();
                            drLaborHrsAdjustment["Tsa_OrigAdjPayCycle"]         = dtEmpPayTranHdrTrl.Rows[i]["Tpd_AdjPayCycle"].ToString();
                            drLaborHrsAdjustment["Tsa_PayCycle"]                = dtEmpPayTranHdrTrl.Rows[i]["Tpd_PayCycle"].ToString();
                            drLaborHrsAdjustment["Tsa_Date"]                    = strProcessDate;

                            SalaryRate                                          = Convert.ToDecimal(drEmpPayTranDtlHst[0]["Tpd_SalaryRate"]);
                            PayrollType                                         = drEmpPayTranDtlHst[0]["Tpd_PayrollType"].ToString();
                            PremiumGroup                                        = drEmpPayTranDtlHst[0]["Tpd_PremiumGrpCode"].ToString();
                            EmploymentStatus                                    = drEmpPayTranDtlHst[0]["Ttr_EmploymentStatusCode"].ToString();
                            MDIVISOR                                            = Convert.ToDecimal(GetScalarValue("Mpd_ParamValue", "M_PolicyDtl", string.Format("Mpd_PolicyCode = 'MDIVISOR' AND Mpd_SubCode = '{0}' AND Mpd_RecordStatus = 'A'", EmploymentStatus), dalHelper));

                            #region Get Hourly Rate
                            HourlyRate = 0;
                            HourlyRate = GetHourlyRate(SalaryRate, PayrollType, MDIVISOR);
                            #endregion

                            #region Get Special Rate
                            if (Convert.ToBoolean(PAYSPECIALRATE))
                            {
                                SpecialSalaryRate = 0;
                                SpecialHourlyRate = 0;

                                int idxx = 0;
                                ParameterInfo[] paramDtl = new ParameterInfo[8];
                                paramDtl[idxx++] = new ParameterInfo("@IDNUMBER", strEmployeeID);
                                paramDtl[idxx++] = new ParameterInfo("@PAYROLLTYPE", PayrollType);
                                paramDtl[idxx++] = new ParameterInfo("@MDIVISOR", MDIVISOR);
                                paramDtl[idxx++] = new ParameterInfo("@HDIVISOR", CommonBL.HOURSINDAY);
                                paramDtl[idxx++] = new ParameterInfo("@PAYCYCLE", PayrollPeriod);
                                paramDtl[idxx++] = new ParameterInfo("@STARTCYCLE", PayrollStart);
                                paramDtl[idxx++] = new ParameterInfo("@ENDCYCLE", PayrollEnd);
                                paramDtl[idxx++] = new ParameterInfo("@COMPANYCODE", CompanyCode);

                                if (!SPECIALRATEFORMULA_SRATE.Equals(""))
                                    SpecialSalaryRate = NewPayrollCalculationBL.GetFormulaQueryDecimalValue(SPECIALRATEFORMULA_SRATE.Replace("@CENTRALDB", CentralProfile), paramDtl, dal);

                                if (!SPECIALRATEFORMULA_HRATE.Equals(""))
                                {
                                    SpecialHourlyRate = NewPayrollCalculationBL.GetFormulaQueryDecimalValue(SPECIALRATEFORMULA_HRATE.Replace("@CENTRALDB", CentralProfile), paramDtl, dal);

                                    if (HRLYRTEDEC > 0)
                                        SpecialHourlyRate = Math.Round(SpecialHourlyRate, Convert.ToInt32(HRLYRTEDEC), MidpointRounding.AwayFromZero);
                                }
                            }
                            #endregion
                            
                            #endregion

                            #region Update Labor Hours Adjustment 2 Row
                            drLaborHrsAdjustment2["Tsa_IDNo"]                   = strEmployeeID;
                            drLaborHrsAdjustment2["Tsa_AdjPayCycle"]            = dtEmpPayTranHdrTrl.Rows[i]["Tpd_AdjPayCycle"].ToString();
                            drLaborHrsAdjustment2["Tsa_OrigAdjPayCycle"]        = dtEmpPayTranHdrTrl.Rows[i]["Tpd_AdjPayCycle"].ToString();
                            drLaborHrsAdjustment2["Tsa_PayCycle"]               = dtEmpPayTranHdrTrl.Rows[i]["Tpd_PayCycle"].ToString();
                            drLaborHrsAdjustment2["Tsa_Date"]                   = strProcessDate;
                            drLaborHrsAdjustment2["Tsa_SalaryRate"]             = drEmpPayTranDtlHst[0]["Tpd_SalaryRate"].ToString();
                            drLaborHrsAdjustment2["Tsa_HourRate"]               = HourlyRate;
                            drLaborHrsAdjustment2["Tsa_PayrollType"]            = drEmpPayTranDtlHst[0]["Tpd_PayrollType"].ToString();
                            drLaborHrsAdjustment2["Tsa_SpecialRate"]            = SpecialSalaryRate;
                            drLaborHrsAdjustment2["Tsa_SpecialHourRate"]        = SpecialHourlyRate;
                            drLaborHrsAdjustment2["Tsa_PremiumGrpCode"]         = drEmpPayTranDtlHst[0]["Tpd_PremiumGrpCode"].ToString();
                            drLaborHrsAdjustment2["Tsa_LTAmt"]                  = 0;
                            drLaborHrsAdjustment2["Tsa_UTAmt"]                  = 0;
                            drLaborHrsAdjustment2["Tsa_UPLVAmt"]                = 0;
                            drLaborHrsAdjustment2["Tsa_ABSLEGHOLAmt"]           = 0;
                            drLaborHrsAdjustment2["Tsa_ABSSPLHOLAmt"]           = 0;
                            drLaborHrsAdjustment2["Tsa_ABSCOMPHOLAmt"]          = 0;
                            drLaborHrsAdjustment2["Tsa_ABSPSDAmt"]              = 0;
                            drLaborHrsAdjustment2["Tsa_ABSOTHHOLAmt"]           = 0;
                            drLaborHrsAdjustment2["Tsa_WDABSAmt"]               = 0;
                            drLaborHrsAdjustment2["Tsa_LTUTMaxAmt"]             = 0;
                            drLaborHrsAdjustment2["Tsa_ABSAmt"]                 = 0;
                            drLaborHrsAdjustment2["Tsa_REGAmt"]                 = 0;
                            drLaborHrsAdjustment2["Tsa_PDLVAmt"]                = 0;
                            drLaborHrsAdjustment2["Tsa_PDLEGHOLAmt"]            = 0;
                            drLaborHrsAdjustment2["Tsa_PDSPLHOLAmt"]            = 0;
                            drLaborHrsAdjustment2["Tsa_PDCOMPHOLAmt"]           = 0;
                            drLaborHrsAdjustment2["Tsa_PDPSDAmt"]               = 0;
                            drLaborHrsAdjustment2["Tsa_PDOTHHOLAmt"]            = 0;
                            drLaborHrsAdjustment2["Tsa_PDRESTLEGHOLAmt"]        = 0;
                            drLaborHrsAdjustment2["Tsa_RGAdjAmt"]               = 0;
                            drLaborHrsAdjustment2["Tsa_OTAdjAmt"]               = 0;
                            drLaborHrsAdjustment2["Tsa_HOLAdjAmt"]              = 0;
                            drLaborHrsAdjustment2["Tsa_NDAdjAmt"]               = 0;
                            drLaborHrsAdjustment2["Tsa_LVAdjAmt"]               = 0;
                            drLaborHrsAdjustment2["Tsa_TotalAdjAmt"]            = 0;
                            drLaborHrsAdjustment2["Usr_Login"]                  = LoginUser;
                            drLaborHrsAdjustment2["Ludatetime"]                 = DateTime.Now;
                            #endregion

                            #region Update Labor Hours Adjustment Extension Row
                            if (bHasDayCodeExt)
                            {
                                #region Update Labor Hours Adjustment Ext Row (Part 1)
                                drLaborHrsAdjustmentExt["Tsm_Misc1Hr"]          = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_Misc1Hr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_Misc1Hr"]);
                                drLaborHrsAdjustmentExt["Tsm_Misc1OTHr"]        = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_Misc1OTHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_Misc1OTHr"]);
                                drLaborHrsAdjustmentExt["Tsm_Misc1NDHr"]        = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_Misc1NDHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_Misc1NDHr"]);
                                drLaborHrsAdjustmentExt["Tsm_Misc1NDOTHr"]      = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_Misc1NDOTHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_Misc1NDOTHr"]);
                                drLaborHrsAdjustmentExt["Tsm_Misc2Hr"]          = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_Misc2Hr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_Misc2Hr"]);
                                drLaborHrsAdjustmentExt["Tsm_Misc2OTHr"]        = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_Misc2OTHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_Misc2OTHr"]);
                                drLaborHrsAdjustmentExt["Tsm_Misc2NDHr"]        = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_Misc2NDHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_Misc2NDHr"]);
                                drLaborHrsAdjustmentExt["Tsm_Misc2NDOTHr"]      = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_Misc2NDOTHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_Misc2NDOTHr"]);
                                drLaborHrsAdjustmentExt["Tsm_Misc3Hr"]          = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_Misc3Hr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_Misc3Hr"]);
                                drLaborHrsAdjustmentExt["Tsm_Misc3OTHr"]        = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_Misc3OTHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_Misc3OTHr"]);
                                drLaborHrsAdjustmentExt["Tsm_Misc3NDHr"]        = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_Misc3NDHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_Misc3NDHr"]);
                                drLaborHrsAdjustmentExt["Tsm_Misc3NDOTHr"]      = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_Misc3NDOTHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_Misc3NDOTHr"]);
                                drLaborHrsAdjustmentExt["Tsm_Misc4Hr"]          = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_Misc4Hr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_Misc4Hr"]);
                                drLaborHrsAdjustmentExt["Tsm_Misc4OTHr"]        = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_Misc4OTHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_Misc4OTHr"]);
                                drLaborHrsAdjustmentExt["Tsm_Misc4NDHr"]        = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_Misc4NDHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_Misc4NDHr"]);
                                drLaborHrsAdjustmentExt["Tsm_Misc4NDOTHr"]      = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_Misc4NDOTHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_Misc4NDOTHr"]);
                                drLaborHrsAdjustmentExt["Tsm_Misc5Hr"]          = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_Misc5Hr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_Misc5Hr"]);
                                drLaborHrsAdjustmentExt["Tsm_Misc5OTHr"]        = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_Misc5OTHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_Misc5OTHr"]);
                                drLaborHrsAdjustmentExt["Tsm_Misc5NDHr"]        = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_Misc5NDHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_Misc5NDHr"]);
                                drLaborHrsAdjustmentExt["Tsm_Misc5NDOTHr"]      = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_Misc5NDOTHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_Misc5NDOTHr"]);
                                drLaborHrsAdjustmentExt["Tsm_Misc6Hr"]          = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_Misc6Hr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_Misc6Hr"]);
                                drLaborHrsAdjustmentExt["Tsm_Misc6OTHr"]        = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_Misc6OTHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_Misc6OTHr"]);
                                drLaborHrsAdjustmentExt["Tsm_Misc6NDHr"]        = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_Misc6NDHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_Misc6NDHr"]);
                                drLaborHrsAdjustmentExt["Tsm_Misc6NDOTHr"]      = Convert.ToDouble(drEmpPayTranDtlHst[0]["Tpd_Misc6NDOTHr"]) - Convert.ToDouble(dtEmpPayTranHdrTrl.Rows[i]["Tpd_Misc6NDOTHr"]);
                                #endregion

                                #region Update Labor Hours Adjustment Ext Row (Part 2)
                                drLaborHrsAdjustmentExt["Tsm_IDNo"]             = strEmployeeID;
                                drLaborHrsAdjustmentExt["Tsm_AdjPayCycle"]      = dtEmpPayTranHdrTrl.Rows[i]["Tpd_AdjPayCycle"].ToString();
                                drLaborHrsAdjustmentExt["Tsm_OrigAdjPayCycle"]  = dtEmpPayTranHdrTrl.Rows[i]["Tpd_AdjPayCycle"].ToString();
                                drLaborHrsAdjustmentExt["Tsm_PayCycle"]         = dtEmpPayTranHdrTrl.Rows[i]["Tpd_PayCycle"].ToString();
                                drLaborHrsAdjustmentExt["Tsm_Date"]             = strProcessDate;
                                drLaborHrsAdjustmentExt["Usr_Login"]            = LoginUser;
                                drLaborHrsAdjustmentExt["Ludatetime"]           = DateTime.Now;
                                #endregion

                                #region Update Labor Hours Adjustment Ext Detail Row
                                drLaborHrsAdjustmentExt2["Tsm_IDNo"]            = strEmployeeID;
                                drLaborHrsAdjustmentExt2["Tsm_AdjPayCycle"]     = dtEmpPayTranHdrTrl.Rows[i]["Tpd_AdjPayCycle"].ToString();
                                drLaborHrsAdjustmentExt2["Tsm_OrigAdjPayCycle"] = dtEmpPayTranHdrTrl.Rows[i]["Tpd_AdjPayCycle"].ToString();
                                drLaborHrsAdjustmentExt2["Tsm_PayCycle"]        = dtEmpPayTranHdrTrl.Rows[i]["Tpd_PayCycle"].ToString();
                                drLaborHrsAdjustmentExt2["Tsm_Date"]            = strProcessDate;
                                drLaborHrsAdjustmentExt2["Usr_Login"]           = LoginUser;
                                drLaborHrsAdjustmentExt2["Ludatetime"]          = DateTime.Now;
                                #endregion
                            }
                            #endregion

                            #region Insert Allowances
                            for (int iAlwIdx = 1; iAlwIdx <= 12; iAlwIdx++)
                            {
                                strAllowanceCol = string.Format("Ttr_TBAmt{0:0}", iAlwIdx);
                                drArrAllowance = dtAllowances.Select(string.Format("Mvh_TimeBaseID = '{0:0}'", iAlwIdx));
                                if (drArrAllowance.Length > 0)
                                {
                                    dAlwAmt = Convert.ToDecimal(drEmpPayTranDtlHst[0][strAllowanceCol]) - Convert.ToDecimal(dtEmpPayTranHdrTrl.Rows[i][strAllowanceCol]);
                                    if (dAlwAmt != 0)
                                        InsertEmployeeDailyAllowance(strEmployeeID, drArrAllowance[0]["Mvh_AdjustmentCode"].ToString(), dAlwAmt, "P", LoginUser);
                                }
                            }
                            #endregion

                            #region Load group premiums
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
                                throw new PayrollException("Error loading day premium table for " + strEmployeeID);
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

                            #region Compute Adjustment Amount

                            #region OT Adjustment Amount Details
                            if (Convert.ToBoolean(PAYSPECIALRATE))
                            {
                                DataTable dtPayCodeRateOvertime = NewPayrollCalculationBL.GetPayCodeRateOvertime(false, PayrollType, PremiumGroup, dal);

                                drLaborHrsAdjustment2["Tsa_REGOTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_REGOTHr"]) * (CheckIfHasSpecialRate(dtPayCodeRateOvertime, "REG") ? SpecialHourlyRate : HourlyRate) * RegOT;
                                drLaborHrsAdjustment2["Tsa_REGNDAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_REGNDHr"]) * (CheckIfHasSpecialRate(dtPayCodeRateOvertime, "REG") ? SpecialHourlyRate : HourlyRate) * RegND * RegNDPercent;
                                drLaborHrsAdjustment2["Tsa_REGNDOTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_REGNDOTHr"]) * (CheckIfHasSpecialRate(dtPayCodeRateOvertime, "REG") ? SpecialHourlyRate : HourlyRate) * RegOTND * RegOTNDPercent;

                                drLaborHrsAdjustment2["Tsa_RESTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_RESTHr"]) * (CheckIfHasSpecialRate(dtPayCodeRateOvertime, "REST") ? SpecialHourlyRate : HourlyRate) * Rest;
                                drLaborHrsAdjustment2["Tsa_RESTOTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_RESTOTHr"]) * (CheckIfHasSpecialRate(dtPayCodeRateOvertime, "REST") ? SpecialHourlyRate : HourlyRate) * RestOT;
                                drLaborHrsAdjustment2["Tsa_RESTNDAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_RESTNDHr"]) * (CheckIfHasSpecialRate(dtPayCodeRateOvertime, "REST") ? SpecialHourlyRate : HourlyRate) * RestND * RestNDPercent;
                                drLaborHrsAdjustment2["Tsa_RESTNDOTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_RESTNDOTHr"]) * (CheckIfHasSpecialRate(dtPayCodeRateOvertime, "REST") ? SpecialHourlyRate : HourlyRate) * RestOTND * RestOTNDPercent;

                                drLaborHrsAdjustment2["Tsa_LEGHOLAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_LEGHOLHr"]) * (CheckIfHasSpecialRate(dtPayCodeRateOvertime, "LEGHOL") ? SpecialHourlyRate : HourlyRate) * (Hol);
                                drLaborHrsAdjustment2["Tsa_LEGHOLOTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_LEGHOLOTHr"]) * (CheckIfHasSpecialRate(dtPayCodeRateOvertime, "LEGHOL") ? SpecialHourlyRate : HourlyRate) * HolOT;
                                drLaborHrsAdjustment2["Tsa_LEGHOLNDAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_LEGHOLNDHr"]) * (CheckIfHasSpecialRate(dtPayCodeRateOvertime, "LEGHOL") ? SpecialHourlyRate : HourlyRate) * (HolND) * HolNDPercent;
                                drLaborHrsAdjustment2["Tsa_LEGHOLNDOTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_LEGHOLNDOTHr"]) * (CheckIfHasSpecialRate(dtPayCodeRateOvertime, "LEGHOL") ? SpecialHourlyRate : HourlyRate) * HolOTND * HolOTNDPercent;

                                drLaborHrsAdjustment2["Tsa_SPLHOLAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_SPLHOLHr"]) * (CheckIfHasSpecialRate(dtPayCodeRateOvertime, "SPLHOL") ? SpecialHourlyRate : HourlyRate) * (SPL);
                                drLaborHrsAdjustment2["Tsa_SPLHOLOTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_SPLHOLOTHr"]) * (CheckIfHasSpecialRate(dtPayCodeRateOvertime, "SPLHOL") ? SpecialHourlyRate : HourlyRate) * SPLOT;
                                drLaborHrsAdjustment2["Tsa_SPLHOLNDAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_SPLHOLNDHr"]) * (CheckIfHasSpecialRate(dtPayCodeRateOvertime, "SPLHOL") ? SpecialHourlyRate : HourlyRate) * (SPLND) * SPLNDPercent;
                                drLaborHrsAdjustment2["Tsa_SPLHOLNDOTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_SPLHOLNDOTHr"]) * (CheckIfHasSpecialRate(dtPayCodeRateOvertime, "SPLHOL") ? SpecialHourlyRate : HourlyRate) * SPLOTND * SPLOTNDPercent;

                                drLaborHrsAdjustment2["Tsa_PSDAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_PSDHr"]) * (CheckIfHasSpecialRate(dtPayCodeRateOvertime, "PSD") ? SpecialHourlyRate : HourlyRate) * (PSD);
                                drLaborHrsAdjustment2["Tsa_PSDOTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_PSDOTHr"]) * (CheckIfHasSpecialRate(dtPayCodeRateOvertime, "PSD") ? SpecialHourlyRate : HourlyRate) * PSDOT;
                                drLaborHrsAdjustment2["Tsa_PSDNDAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_PSDNDHr"]) * (CheckIfHasSpecialRate(dtPayCodeRateOvertime, "PSD") ? SpecialHourlyRate : HourlyRate) * (PSDND) * PSDNDPercent;
                                drLaborHrsAdjustment2["Tsa_PSDNDOTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_PSDNDOTHr"]) * (CheckIfHasSpecialRate(dtPayCodeRateOvertime, "PSD") ? SpecialHourlyRate : HourlyRate) * PSDOTND * PSDOTNDPercent;

                                drLaborHrsAdjustment2["Tsa_COMPHOLAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_COMPHOLHr"]) * (CheckIfHasSpecialRate(dtPayCodeRateOvertime, "COMPHOL") ? SpecialHourlyRate : HourlyRate) * (Comp);
                                drLaborHrsAdjustment2["Tsa_COMPHOLOTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_COMPHOLOTHr"]) * (CheckIfHasSpecialRate(dtPayCodeRateOvertime, "COMPHOL") ? SpecialHourlyRate : HourlyRate) * CompOT;
                                drLaborHrsAdjustment2["Tsa_COMPHOLNDAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_COMPHOLNDHr"]) * (CheckIfHasSpecialRate(dtPayCodeRateOvertime, "COMPHOL") ? SpecialHourlyRate : HourlyRate) * (CompND) * CompNDPercent;
                                drLaborHrsAdjustment2["Tsa_COMPHOLNDOTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_COMPHOLNDOTHr"]) * (CheckIfHasSpecialRate(dtPayCodeRateOvertime, "COMPHOL") ? SpecialHourlyRate : HourlyRate) * CompOTND * CompOTNDPercent;

                                drLaborHrsAdjustment2["Tsa_RESTLEGHOLAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_RESTLEGHOLHr"]) * (CheckIfHasSpecialRate(dtPayCodeRateOvertime, "RESTLEGHOL") ? SpecialHourlyRate : HourlyRate) * RestHol;
                                drLaborHrsAdjustment2["Tsa_RESTLEGHOLOTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_RESTLEGHOLOTHr"]) * (CheckIfHasSpecialRate(dtPayCodeRateOvertime, "RESTLEGHOL") ? SpecialHourlyRate : HourlyRate) * RestHolOT;
                                drLaborHrsAdjustment2["Tsa_RESTLEGHOLNDAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_RESTLEGHOLNDHr"]) * (CheckIfHasSpecialRate(dtPayCodeRateOvertime, "RESTLEGHOL") ? SpecialHourlyRate : HourlyRate) * RestHolND * RestHolNDPercent;
                                drLaborHrsAdjustment2["Tsa_RESTLEGHOLNDOTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_RESTLEGHOLNDOTHr"]) * (CheckIfHasSpecialRate(dtPayCodeRateOvertime, "RESTLEGHOL") ? SpecialHourlyRate : HourlyRate) * RestHolOTND * RestHolOTNDPercent;

                                drLaborHrsAdjustment2["Tsa_RESTSPLHOLAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_RESTSPLHOLHr"]) * (CheckIfHasSpecialRate(dtPayCodeRateOvertime, "RESTSPLHOL") ? SpecialHourlyRate : HourlyRate) * RestSPL;
                                drLaborHrsAdjustment2["Tsa_RESTSPLHOLOTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_RESTSPLHOLOTHr"]) * (CheckIfHasSpecialRate(dtPayCodeRateOvertime, "RESTSPLHOL") ? SpecialHourlyRate : HourlyRate) * RestSPLOT;
                                drLaborHrsAdjustment2["Tsa_RESTSPLHOLNDAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_RESTSPLHOLNDHr"]) * (CheckIfHasSpecialRate(dtPayCodeRateOvertime, "RESTSPLHOL") ? SpecialHourlyRate : HourlyRate) * RestSPLND * RestSPLNDPercent;
                                drLaborHrsAdjustment2["Tsa_RESTSPLHOLNDOTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_RESTSPLHOLNDOTHr"]) * (CheckIfHasSpecialRate(dtPayCodeRateOvertime, "RESTSPLHOL") ? SpecialHourlyRate : HourlyRate) * RestSPLOTND * RestSPLOTNDPercent;

                                drLaborHrsAdjustment2["Tsa_RESTCOMPHOLAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_RESTCOMPHOLHr"]) * (CheckIfHasSpecialRate(dtPayCodeRateOvertime, "RESTCOMPHOL") ? SpecialHourlyRate : HourlyRate) * RestComp;
                                drLaborHrsAdjustment2["Tsa_RESTCOMPHOLOTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_RESTCOMPHOLOTHr"]) * (CheckIfHasSpecialRate(dtPayCodeRateOvertime, "RESTCOMPHOL") ? SpecialHourlyRate : HourlyRate) * RestCompOT;
                                drLaborHrsAdjustment2["Tsa_RESTCOMPHOLNDAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_RESTCOMPHOLNDHr"]) * (CheckIfHasSpecialRate(dtPayCodeRateOvertime, "RESTCOMPHOL") ? SpecialHourlyRate : HourlyRate) * RestCompND * RestCompNDPercent;
                                drLaborHrsAdjustment2["Tsa_RESTCOMPHOLNDOTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_RESTCOMPHOLNDOTHr"]) * (CheckIfHasSpecialRate(dtPayCodeRateOvertime, "RESTCOMPHOL") ? SpecialHourlyRate : HourlyRate) * RestCompOTND * RestCompOTNDPercent;

                                drLaborHrsAdjustment2["Tsa_RESTPSDAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_RESTPSDHr"]) * (CheckIfHasSpecialRate(dtPayCodeRateOvertime, "RESTPSD") ? SpecialHourlyRate : HourlyRate) * RestPSD;
                                drLaborHrsAdjustment2["Tsa_RESTPSDOTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_RESTPSDOTHr"]) * (CheckIfHasSpecialRate(dtPayCodeRateOvertime, "RESTPSD") ? SpecialHourlyRate : HourlyRate) * RestPSDOT;
                                drLaborHrsAdjustment2["Tsa_RESTPSDNDAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_RESTPSDNDHr"]) * (CheckIfHasSpecialRate(dtPayCodeRateOvertime, "RESTPSD") ? SpecialHourlyRate : HourlyRate) * RestPSDND * RestPSDNDPercent;
                                drLaborHrsAdjustment2["Tsa_RESTPSDNDOTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_RESTPSDNDOTHr"]) * (CheckIfHasSpecialRate(dtPayCodeRateOvertime, "RESTPSD") ? SpecialHourlyRate : HourlyRate) * RestPSDOTND * RestPSDOTNDPercent;
                            }
                            else
                            {
                                drLaborHrsAdjustment2["Tsa_REGOTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_REGOTHr"]) * HourlyRate * RegOT;
                                drLaborHrsAdjustment2["Tsa_REGNDAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_REGNDHr"]) * HourlyRate * RegND * RegNDPercent;
                                drLaborHrsAdjustment2["Tsa_REGNDOTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_REGNDOTHr"]) * HourlyRate * RegOTND * RegOTNDPercent;

                                drLaborHrsAdjustment2["Tsa_RESTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_RESTHr"]) * HourlyRate * Rest;
                                drLaborHrsAdjustment2["Tsa_RESTOTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_RESTOTHr"]) * HourlyRate * RestOT;
                                drLaborHrsAdjustment2["Tsa_RESTNDAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_RESTNDHr"]) * HourlyRate * RestND * RestNDPercent;
                                drLaborHrsAdjustment2["Tsa_RESTNDOTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_RESTNDOTHr"]) * HourlyRate * RestOTND * RestOTNDPercent;

                                drLaborHrsAdjustment2["Tsa_LEGHOLAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_LEGHOLHr"]) * HourlyRate * (Hol);
                                drLaborHrsAdjustment2["Tsa_LEGHOLOTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_LEGHOLOTHr"]) * HourlyRate * HolOT;
                                drLaborHrsAdjustment2["Tsa_LEGHOLNDAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_LEGHOLNDHr"]) * HourlyRate * (HolND) * HolNDPercent;
                                drLaborHrsAdjustment2["Tsa_LEGHOLNDOTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_LEGHOLNDOTHr"]) * HourlyRate * HolOTND * HolOTNDPercent;

                                drLaborHrsAdjustment2["Tsa_SPLHOLAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_SPLHOLHr"]) * HourlyRate * (SPL);
                                drLaborHrsAdjustment2["Tsa_SPLHOLOTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_SPLHOLOTHr"]) * HourlyRate * SPLOT;
                                drLaborHrsAdjustment2["Tsa_SPLHOLNDAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_SPLHOLNDHr"]) * HourlyRate * (SPLND) * SPLNDPercent;
                                drLaborHrsAdjustment2["Tsa_SPLHOLNDOTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_SPLHOLNDOTHr"]) * HourlyRate * SPLOTND * SPLOTNDPercent;

                                drLaborHrsAdjustment2["Tsa_PSDAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_PSDHr"]) * HourlyRate * (PSD);
                                drLaborHrsAdjustment2["Tsa_PSDOTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_PSDOTHr"]) * HourlyRate * PSDOT;
                                drLaborHrsAdjustment2["Tsa_PSDNDAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_PSDNDHr"]) * HourlyRate * (PSDND) * PSDNDPercent;
                                drLaborHrsAdjustment2["Tsa_PSDNDOTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_PSDNDOTHr"]) * HourlyRate * PSDOTND * PSDOTNDPercent;

                                drLaborHrsAdjustment2["Tsa_COMPHOLAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_COMPHOLHr"]) * HourlyRate * (Comp);
                                drLaborHrsAdjustment2["Tsa_COMPHOLOTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_COMPHOLOTHr"]) * HourlyRate * CompOT;
                                drLaborHrsAdjustment2["Tsa_COMPHOLNDAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_COMPHOLNDHr"]) * HourlyRate * (CompND) * CompNDPercent;
                                drLaborHrsAdjustment2["Tsa_COMPHOLNDOTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_COMPHOLNDOTHr"]) * HourlyRate * CompOTND * CompOTNDPercent;

                                drLaborHrsAdjustment2["Tsa_RESTLEGHOLAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_RESTLEGHOLHr"]) * HourlyRate * RestHol;
                                drLaborHrsAdjustment2["Tsa_RESTLEGHOLOTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_RESTLEGHOLOTHr"]) * HourlyRate * RestHolOT;
                                drLaborHrsAdjustment2["Tsa_RESTLEGHOLNDAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_RESTLEGHOLNDHr"]) * HourlyRate * RestHolND * RestHolNDPercent;
                                drLaborHrsAdjustment2["Tsa_RESTLEGHOLNDOTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_RESTLEGHOLNDOTHr"]) * HourlyRate * RestHolOTND * RestHolOTNDPercent;

                                drLaborHrsAdjustment2["Tsa_RESTSPLHOLAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_RESTSPLHOLHr"]) * HourlyRate * RestSPL;
                                drLaborHrsAdjustment2["Tsa_RESTSPLHOLOTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_RESTSPLHOLOTHr"]) * HourlyRate * RestSPLOT;
                                drLaborHrsAdjustment2["Tsa_RESTSPLHOLNDAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_RESTSPLHOLNDHr"]) * HourlyRate * RestSPLND * RestSPLNDPercent;
                                drLaborHrsAdjustment2["Tsa_RESTSPLHOLNDOTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_RESTSPLHOLNDOTHr"]) * HourlyRate * RestSPLOTND * RestSPLOTNDPercent;

                                drLaborHrsAdjustment2["Tsa_RESTCOMPHOLAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_RESTCOMPHOLHr"]) * HourlyRate * RestComp;
                                drLaborHrsAdjustment2["Tsa_RESTCOMPHOLOTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_RESTCOMPHOLOTHr"]) * HourlyRate * RestCompOT;
                                drLaborHrsAdjustment2["Tsa_RESTCOMPHOLNDAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_RESTCOMPHOLNDHr"]) * HourlyRate * RestCompND * RestCompNDPercent;
                                drLaborHrsAdjustment2["Tsa_RESTCOMPHOLNDOTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_RESTCOMPHOLNDOTHr"]) * HourlyRate * RestCompOTND * RestCompOTNDPercent;

                                drLaborHrsAdjustment2["Tsa_RESTPSDAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_RESTPSDHr"]) * HourlyRate * RestPSD;
                                drLaborHrsAdjustment2["Tsa_RESTPSDOTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_RESTPSDOTHr"]) * HourlyRate * RestPSDOT;
                                drLaborHrsAdjustment2["Tsa_RESTPSDNDAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_RESTPSDNDHr"]) * HourlyRate * RestPSDND * RestPSDNDPercent;
                                drLaborHrsAdjustment2["Tsa_RESTPSDNDOTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_RESTPSDNDOTHr"]) * HourlyRate * RestPSDOTND * RestPSDOTNDPercent;
                            }
                            if (bHasDayCodeExt)
                            {
                                drDayCodePremiumFiller = dtDayCodePremiumFillers.Select(string.Format("Mdp_PremiumGrpCode = '{0}' AND Mdp_PayrollType = '{1}'", PremiumGroup, PayrollType));
                                for (int premFill = 0; premFill < drDayCodePremiumFiller.Length; premFill++)
                                {
                                    fillerHrCol      = string.Format("Tsm_Misc{0:0}Hr", drDayCodePremiumFiller[premFill]["Mmd_MiscDayID"]);
                                    fillerOTHrCol    = string.Format("Tsm_Misc{0:0}OTHr", drDayCodePremiumFiller[premFill]["Mmd_MiscDayID"]);
                                    fillerNDHrCol    = string.Format("Tsm_Misc{0:0}NDHr", drDayCodePremiumFiller[premFill]["Mmd_MiscDayID"]);
                                    fillerOTNDHrCol  = string.Format("Tsm_Misc{0:0}NDOTHr", drDayCodePremiumFiller[premFill]["Mmd_MiscDayID"]);
                                    fillerAmtCol     = string.Format("Tsm_Misc{0:0}Amt", drDayCodePremiumFiller[premFill]["Mmd_MiscDayID"]);
                                    fillerOTAmtCol   = string.Format("Tsm_Misc{0:0}OTAmt", drDayCodePremiumFiller[premFill]["Mmd_MiscDayID"]);
                                    fillerNDAmtCol   = string.Format("Tsm_Misc{0:0}NDAmt", drDayCodePremiumFiller[premFill]["Mmd_MiscDayID"]);
                                    fillerOTNDAmtCol = string.Format("Tsm_Misc{0:0}NDOTAmt", drDayCodePremiumFiller[premFill]["Mmd_MiscDayID"]);
                                    if (Convert.ToBoolean(PAYSPECIALRATE))
                                    {
                                        DataTable dtPayCodeRateOvertime = NewPayrollCalculationBL.GetPayCodeRateOvertime(true, PayrollType, PremiumGroup, dal);
                                        drLaborHrsAdjustmentExt2[fillerAmtCol] = Convert.ToDecimal(drLaborHrsAdjustmentExt[fillerHrCol]) * (CheckIfHasSpecialRate(dtPayCodeRateOvertime, drDayCodePremiumFiller[premFill]["Mmd_MiscDayID"].ToString()) ? SpecialHourlyRate : HourlyRate) * ((Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_RGPremiumRate"]) / 100));
                                        drLaborHrsAdjustmentExt2[fillerOTAmtCol] = Convert.ToDecimal(drLaborHrsAdjustmentExt[fillerOTHrCol]) * (CheckIfHasSpecialRate(dtPayCodeRateOvertime, drDayCodePremiumFiller[premFill]["Mmd_MiscDayID"].ToString()) ? SpecialHourlyRate : HourlyRate) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_OTPremiumRate"]) / 100);
                                        drLaborHrsAdjustmentExt2[fillerNDAmtCol] = Convert.ToDecimal(drLaborHrsAdjustmentExt[fillerNDHrCol]) * (CheckIfHasSpecialRate(dtPayCodeRateOvertime, drDayCodePremiumFiller[premFill]["Mmd_MiscDayID"].ToString()) ? SpecialHourlyRate : HourlyRate) * ((Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_RGNDPremiumRate"]) / 100)) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_RGNDPercentageRate"]) / 100);
                                        drLaborHrsAdjustmentExt2[fillerOTNDAmtCol] = Convert.ToDecimal(drLaborHrsAdjustmentExt[fillerOTNDHrCol]) * (CheckIfHasSpecialRate(dtPayCodeRateOvertime, drDayCodePremiumFiller[premFill]["Mmd_MiscDayID"].ToString()) ? SpecialHourlyRate : HourlyRate) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_OTNDPremiumRate"]) / 100) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_OTNDPercentageRate"]) / 100);
                                    }
                                    else
                                    {
                                        drLaborHrsAdjustmentExt2[fillerAmtCol] = Convert.ToDecimal(drLaborHrsAdjustmentExt[fillerHrCol]) * HourlyRate * ((Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_RGPremiumRate"]) / 100));
                                        drLaborHrsAdjustmentExt2[fillerOTAmtCol] = Convert.ToDecimal(drLaborHrsAdjustmentExt[fillerOTHrCol]) * HourlyRate * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_OTPremiumRate"]) / 100);
                                        drLaborHrsAdjustmentExt2[fillerNDAmtCol] = Convert.ToDecimal(drLaborHrsAdjustmentExt[fillerNDHrCol]) * HourlyRate * ((Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_RGNDPremiumRate"]) / 100)) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_RGNDPercentageRate"]) / 100);
                                        drLaborHrsAdjustmentExt2[fillerOTNDAmtCol] = Convert.ToDecimal(drLaborHrsAdjustmentExt[fillerOTNDHrCol]) * HourlyRate * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_OTNDPremiumRate"]) / 100) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Mdp_OTNDPercentageRate"]) / 100);
                                    }
                                        
                                }
                            }
                            #endregion

                            #region Regular Adjustment Amount Details
 
                            if (PayrollType == "D") 
                            {
                                if (Convert.ToBoolean(PAYSPECIALRATE))
                                {
                                    drLaborHrsAdjustment2["Tsa_REGAmt"] = (Convert.ToDecimal(drLaborHrsAdjustment["Tsa_REGHr"]) * (CheckIfHasSpecialRate("REG") ? SpecialHourlyRate : HourlyRate));
                                    drLaborHrsAdjustment2["Tsa_PDLVAmt"] = (Convert.ToDecimal(drLaborHrsAdjustment["Tsa_PDLVHr"]) * (CheckIfHasSpecialRate("PDLV") ? SpecialHourlyRate : HourlyRate));
                                }
                                else
                                {
                                    drLaborHrsAdjustment2["Tsa_REGAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_REGHr"]) * HourlyRate;
                                    drLaborHrsAdjustment2["Tsa_PDLVAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_PDLVHr"]) * HourlyRate;
                                }    
                            }
                            #endregion

                            #region Holiday Adjustment Amount Details
                            if (PayrollType != "M") //Only Dailies will be ADDED by Unwork Holidays
                            {
                                if (Convert.ToBoolean(PAYSPECIALRATE))
                                {
                                    if (drEmpPayTranDtlHst[0]["Ttr_RestDayFlag"].ToString() == "False")
                                    {
                                        drLaborHrsAdjustment2["Tsa_PDLEGHOLAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_PDLEGHOLHr"]) * (CheckIfHasSpecialRate("PDLEGHOL") ? SpecialHourlyRate : HourlyRate);
                                    }
                                    else
                                    {
                                        drLaborHrsAdjustment["Tsa_PDLEGHOLHr"] = 0; //NO ADJUSTMENT
                                        drLaborHrsAdjustment2["Tsa_PDLEGHOLAmt"] = 0;
                                    }

                                    drLaborHrsAdjustment2["Tsa_PDSPLHOLAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_PDSPLHOLHr"]) * (CheckIfHasSpecialRate("PDSPLHOL") ? SpecialHourlyRate : HourlyRate);
                                    drLaborHrsAdjustment2["Tsa_PDCOMPHOLAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_PDCOMPHOLHr"]) * (CheckIfHasSpecialRate("PDCOMPHOL") ? SpecialHourlyRate : HourlyRate);
                                    drLaborHrsAdjustment2["Tsa_PDOTHHOLAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_PDOTHHOLHr"]) * (CheckIfHasSpecialRate("PDOTHHOL") ? SpecialHourlyRate : HourlyRate);
                                }
                                else
                                {
                                    if (drEmpPayTranDtlHst[0]["Ttr_RestDayFlag"].ToString() == "False")
                                    {
                                        drLaborHrsAdjustment2["Tsa_PDLEGHOLAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_PDLEGHOLHr"]) * HourlyRate;
                                    }
                                    else
                                    {
                                        drLaborHrsAdjustment["Tsa_PDLEGHOLHr"] = 0; //NO ADJUSTMENT
                                        drLaborHrsAdjustment2["Tsa_PDLEGHOLAmt"] = 0;
                                    }

                                    drLaborHrsAdjustment2["Tsa_PDSPLHOLAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_PDSPLHOLHr"]) * HourlyRate;
                                    drLaborHrsAdjustment2["Tsa_PDCOMPHOLAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_PDCOMPHOLHr"]) * HourlyRate;
                                    drLaborHrsAdjustment2["Tsa_PDOTHHOLAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_PDOTHHOLHr"]) * HourlyRate;
                                }  
                            }

                            if (PayrollType == "M") //Only Monthlies will be DEDUCTED by Absent Amount
                            {
                                if (Convert.ToBoolean(PAYSPECIALRATE))
                                {
                                    drLaborHrsAdjustment2["Tsa_REGAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_REGHr"]) * (CheckIfHasSpecialRate("REG") ? SpecialHourlyRate : HourlyRate);

                                    if (Convert.ToDecimal(drLaborHrsAdjustment["Tsa_REGHr"]) == 0)
                                        drLaborHrsAdjustment2["Tsa_ABSAmt"] = Convert.ToDecimal(drLaborHrsAdjustment2["Tpd_LTAmt"])
                                                                                    + Convert.ToDecimal(drLaborHrsAdjustment2["Tpd_UTAmt"])
                                                                                    + Convert.ToDecimal(drLaborHrsAdjustment2["Tpd_UPLVAmt"])
                                                                                    + Convert.ToDecimal(drLaborHrsAdjustment2["Tpd_WDABSAmt"])
                                                                                    + Convert.ToDecimal(drLaborHrsAdjustment2["Tpd_LTUTMaxAmt"]);

                                    if (Convert.ToDecimal(drLaborHrsAdjustment["Tsa_PDLEGHOLHr"]) <= 0
                                        || Convert.ToDecimal(drLaborHrsAdjustment["Tsa_ABSLEGHOLHr"]) <= 0)
                                        drLaborHrsAdjustment2["Tsa_ABSLEGHOLAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_ABSLEGHOLHr"]) * (CheckIfHasSpecialRate("ABSLEGHOL") ? SpecialHourlyRate : HourlyRate);

                                    if (Convert.ToDecimal(drLaborHrsAdjustment["Tsa_PDSPLHOLHr"]) <= 0
                                        || Convert.ToDecimal(drLaborHrsAdjustment["Tsa_ABSSPLHOLHr"]) <= 0)
                                        drLaborHrsAdjustment2["Tsa_ABSSPLHOLAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_ABSSPLHOLHr"]) * (CheckIfHasSpecialRate("ABSSPLHOL") ? SpecialHourlyRate : HourlyRate);

                                    if (Convert.ToDecimal(drLaborHrsAdjustment["Tsa_PDCOMPHOLHr"]) <= 0
                                        || Convert.ToDecimal(drLaborHrsAdjustment["Tsa_ABSCOMPHOLHr"]) <= 0)
                                        drLaborHrsAdjustment2["Tsa_ABSCOMPHOLAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_ABSCOMPHOLHr"]) * (CheckIfHasSpecialRate("ABSCOMPHOL") ? SpecialHourlyRate : HourlyRate);

                                    if (Convert.ToDecimal(drLaborHrsAdjustment["Tsa_PDPSDHr"]) <= 0
                                        || Convert.ToDecimal(drLaborHrsAdjustment["Tsa_ABSPSHr"]) <= 0)
                                        drLaborHrsAdjustment2["Tsa_ABSPSDAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_ABSPSDHr"]) * (CheckIfHasSpecialRate("ABSPSD") ? SpecialHourlyRate : HourlyRate);

                                    if (Convert.ToDecimal(drLaborHrsAdjustment["Tsa_PDOTHHOLHr"]) <= 0
                                        || Convert.ToDecimal(drLaborHrsAdjustment["Tsa_ABSOTHHOLHr"]) <= 0)
                                        drLaborHrsAdjustment2["Tsa_ABSOTHHOLAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_ABSOTHHOLHr"]) * (CheckIfHasSpecialRate("ABSOTHHOL") ? SpecialHourlyRate : HourlyRate);
                                }
                                else
                                {
                                    drLaborHrsAdjustment2["Tsa_REGAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_REGHr"]) * HourlyRate;
                                    //drLaborHrsAdjustment2["Tsa_PDLVAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_PDLVHr"]) * HourlyRate;

                                    if (Convert.ToDecimal(drLaborHrsAdjustment["Tsa_REGHr"]) == 0)
                                        drLaborHrsAdjustment2["Tsa_ABSAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_ABSHr"]) * HourlyRate;

                                    drLaborHrsAdjustment2["Tsa_PDLEGHOLAmt"]  = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_PDLEGHOLHr"]) * HourlyRate;
                                    drLaborHrsAdjustment2["Tsa_PDSPLHOLAmt"]  = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_PDSPLHOLHr"]) * HourlyRate;
                                    drLaborHrsAdjustment2["Tsa_PDCOMPHOLAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_PDCOMPHOLHr"]) * HourlyRate;
                                    drLaborHrsAdjustment2["Tsa_PDPSDAmt"]     = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_PDPSDHr"]) * HourlyRate;
                                    drLaborHrsAdjustment2["Tsa_PDOTHHOLAmt"]  = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_PDOTHHOLHr"]) * HourlyRate;
                                }

                            }

                            //Add "Restday falling on a Holiday" Adjustment to Holiday Amount
                            if (Convert.ToDecimal(drEmpPayTranDtlHst[0]["Tpd_PDRESTLEGHOLHr"])
                                - Convert.ToDecimal(dtEmpPayTranHdrTrl.Rows[i]["Tpd_PDRESTLEGHOLHr"]) != 0)
                            {
                                drLaborHrsAdjustment["Tsa_PDRESTLEGHOLHr"] = Convert.ToDecimal(drEmpPayTranDtlHst[0]["Tpd_PDRESTLEGHOLHr"]) - Convert.ToDecimal(dtEmpPayTranHdrTrl.Rows[i]["Tpd_PDRESTLEGHOLHr"]);
                                if (Convert.ToBoolean(PAYSPECIALRATE))
                                    drLaborHrsAdjustment2["Tsa_PDRESTLEGHOLAmt"] = (Convert.ToDecimal(drLaborHrsAdjustment["Tsa_PDRESTLEGHOLHr"]) * (CheckIfHasSpecialRate("PDRESTLEGHOL") ? SpecialHourlyRate : HourlyRate));
                                else
                                    drLaborHrsAdjustment2["Tsa_PDRESTLEGHOLAmt"] = (Convert.ToDecimal(drLaborHrsAdjustment["Tsa_PDRESTLEGHOLHr"]) * HourlyRate);
                            }
                            #endregion

                            #region Absent Adjustment Amount Details
                            if (Convert.ToBoolean(PAYSPECIALRATE))
                            {
                                //Other Absent Amount Details
                                drLaborHrsAdjustment2["Tsa_LTAmt"]      = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_LTHr"]) * (CheckIfHasSpecialRate("LT") ? SpecialHourlyRate : HourlyRate);
                                drLaborHrsAdjustment2["Tsa_UTAmt"]      = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_UTHr"]) * (CheckIfHasSpecialRate("UT") ? SpecialHourlyRate : HourlyRate);
                                drLaborHrsAdjustment2["Tsa_UPLVAmt"]    = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_UPLVHr"]) * (CheckIfHasSpecialRate("UPLV") ? SpecialHourlyRate : HourlyRate);
                                drLaborHrsAdjustment2["Tsa_WDABSAmt"]   = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_WDABSHr"]) * (CheckIfHasSpecialRate("WDABS") ? SpecialHourlyRate : HourlyRate);
                                drLaborHrsAdjustment2["Tsa_LTUTMaxAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_LTUTMaxHr"]) * (CheckIfHasSpecialRate("LTUTMAX") ? SpecialHourlyRate : HourlyRate);
                            }
                            else
                            {
                                //Other Absent Amount Details
                                drLaborHrsAdjustment2["Tsa_LTAmt"]      = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_LTHr"]) * HourlyRate;
                                drLaborHrsAdjustment2["Tsa_UTAmt"]      = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_UTHr"]) * HourlyRate;
                                drLaborHrsAdjustment2["Tsa_UPLVAmt"]    = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_UPLVHr"]) * HourlyRate;
                                drLaborHrsAdjustment2["Tsa_WDABSAmt"]   = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_WDABSHr"]) * HourlyRate;
                                drLaborHrsAdjustment2["Tsa_LTUTMaxAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Tsa_LTUTMaxHr"]) * HourlyRate;
                            }
                            #endregion

                            #endregion

                            #region Rounding Off of Amounts
                            drLaborHrsAdjustment2["Tsa_LTAmt"]                  = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_LTAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_UTAmt"]                  = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_UTAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_UPLVAmt"]                = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_UPLVAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_ABSLEGHOLAmt"]           = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_ABSLEGHOLAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_ABSSPLHOLAmt"]           = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_ABSSPLHOLAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_ABSCOMPHOLAmt"]          = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_ABSCOMPHOLAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_ABSPSDAmt"]              = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_ABSPSDAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_ABSOTHHOLAmt"]           = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_ABSOTHHOLAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_WDABSAmt"]               = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_WDABSAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_LTUTMaxAmt"]             = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_LTUTMaxAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_ABSAmt"]                 = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_ABSAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_REGAmt"]                 = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_REGAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_PDLVAmt"]                = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_PDLVAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_PDLEGHOLAmt"]            = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_PDLEGHOLAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_PDSPLHOLAmt"]            = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_PDSPLHOLAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_PDCOMPHOLAmt"]           = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_PDCOMPHOLAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_PDPSDAmt"]               = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_PDPSDAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_PDOTHHOLAmt"]            = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_PDOTHHOLAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_REGOTAmt"]               = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_REGOTAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_REGNDAmt"]               = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_REGNDAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_REGNDOTAmt"]             = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_REGNDOTAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_RESTAmt"]                = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_RESTAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_RESTOTAmt"]              = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_RESTOTAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_RESTNDAmt"]              = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_RESTNDAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_RESTNDOTAmt"]            = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_RESTNDOTAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_LEGHOLAmt"]              = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_LEGHOLAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_LEGHOLOTAmt"]            = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_LEGHOLOTAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_LEGHOLNDAmt"]            = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_LEGHOLNDAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_LEGHOLNDOTAmt"]          = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_LEGHOLNDOTAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_SPLHOLAmt"]              = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_SPLHOLAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_SPLHOLOTAmt"]            = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_SPLHOLOTAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_SPLHOLNDAmt"]            = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_SPLHOLNDAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_SPLHOLNDOTAmt"]          = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_SPLHOLNDOTAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_PSDAmt"]                 = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_PSDAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_PSDOTAmt"]               = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_PSDOTAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_PSDNDAmt"]               = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_PSDNDAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_PSDNDOTAmt"]             = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_PSDNDOTAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_COMPHOLAmt"]             = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_COMPHOLAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_COMPHOLOTAmt"]           = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_COMPHOLOTAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_COMPHOLNDAmt"]           = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_COMPHOLNDAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_COMPHOLNDOTAmt"]         = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_COMPHOLNDOTAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_RESTLEGHOLAmt"]          = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_RESTLEGHOLAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_RESTLEGHOLOTAmt"]        = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_RESTLEGHOLOTAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_RESTLEGHOLNDAmt"]        = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_RESTLEGHOLNDAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_RESTLEGHOLNDOTAmt"]      = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_RESTLEGHOLNDOTAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_RESTSPLHOLAmt"]          = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_RESTSPLHOLAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_RESTSPLHOLOTAmt"]        = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_RESTSPLHOLOTAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_RESTSPLHOLNDAmt"]        = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_RESTSPLHOLNDAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_RESTSPLHOLNDOTAmt"]      = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_RESTSPLHOLNDOTAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_RESTCOMPHOLAmt"]         = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_RESTCOMPHOLAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_RESTCOMPHOLOTAmt"]       = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_RESTCOMPHOLOTAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_RESTCOMPHOLNDAmt"]       = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_RESTCOMPHOLNDAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_RESTCOMPHOLNDOTAmt"]     = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_RESTCOMPHOLNDOTAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_RESTPSDAmt"]             = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_RESTPSDAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_RESTPSDOTAmt"]           = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_RESTPSDOTAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_RESTPSDNDAmt"]           = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_RESTPSDNDAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_RESTPSDNDOTAmt"]         = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_RESTPSDNDOTAmt"]), 2);
                           
                            if (bHasDayCodeExt)
                            {
                                drLaborHrsAdjustmentExt2["Tsm_Misc1Amt"]       = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentExt2["Tsm_Misc1Amt"]), 2);
                                drLaborHrsAdjustmentExt2["Tsm_Misc1OTAmt"]     = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentExt2["Tsm_Misc1OTAmt"]), 2);
                                drLaborHrsAdjustmentExt2["Tsm_Misc1NDAmt"]     = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentExt2["Tsm_Misc1NDAmt"]), 2);
                                drLaborHrsAdjustmentExt2["Tsm_Misc1NDOTAmt"]   = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentExt2["Tsm_Misc1NDOTAmt"]), 2);
                                drLaborHrsAdjustmentExt2["Tsm_Misc2Amt"]       = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentExt2["Tsm_Misc2Amt"]), 2);
                                drLaborHrsAdjustmentExt2["Tsm_Misc2OTAmt"]     = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentExt2["Tsm_Misc2OTAmt"]), 2);
                                drLaborHrsAdjustmentExt2["Tsm_Misc2NDAmt"]     = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentExt2["Tsm_Misc2NDAmt"]), 2);
                                drLaborHrsAdjustmentExt2["Tsm_Misc2NDOTAmt"]   = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentExt2["Tsm_Misc2NDOTAmt"]), 2);
                                drLaborHrsAdjustmentExt2["Tsm_Misc3Amt"]       = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentExt2["Tsm_Misc3Amt"]), 2);
                                drLaborHrsAdjustmentExt2["Tsm_Misc3OTAmt"]     = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentExt2["Tsm_Misc3OTAmt"]), 2);
                                drLaborHrsAdjustmentExt2["Tsm_Misc3NDAmt"]     = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentExt2["Tsm_Misc3NDAmt"]), 2);
                                drLaborHrsAdjustmentExt2["Tsm_Misc3NDOTAmt"]   = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentExt2["Tsm_Misc3NDOTAmt"]), 2);
                                drLaborHrsAdjustmentExt2["Tsm_Misc4Amt"]       = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentExt2["Tsm_Misc4Amt"]), 2);
                                drLaborHrsAdjustmentExt2["Tsm_Misc4OTAmt"]     = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentExt2["Tsm_Misc4OTAmt"]), 2);
                                drLaborHrsAdjustmentExt2["Tsm_Misc4NDAmt"]     = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentExt2["Tsm_Misc4NDAmt"]), 2);
                                drLaborHrsAdjustmentExt2["Tsm_Misc4NDOTAmt"]   = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentExt2["Tsm_Misc4NDOTAmt"]), 2);
                                drLaborHrsAdjustmentExt2["Tsm_Misc5Amt"]       = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentExt2["Tsm_Misc5Amt"]), 2);
                                drLaborHrsAdjustmentExt2["Tsm_Misc5OTAmt"]     = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentExt2["Tsm_Misc5OTAmt"]), 2);
                                drLaborHrsAdjustmentExt2["Tsm_Misc5NDAmt"]     = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentExt2["Tsm_Misc5NDAmt"]), 2);
                                drLaborHrsAdjustmentExt2["Tsm_Misc5NDOTAmt"]   = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentExt2["Tsm_Misc5NDOTAmt"]), 2);
                                drLaborHrsAdjustmentExt2["Tsm_Misc6Amt"]       = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentExt2["Tsm_Misc6Amt"]), 2);
                                drLaborHrsAdjustmentExt2["Tsm_Misc6OTAmt"]     = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentExt2["Tsm_Misc6OTAmt"]), 2);
                                drLaborHrsAdjustmentExt2["Tsm_Misc6NDAmt"]     = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentExt2["Tsm_Misc6NDAmt"]), 2);
                                drLaborHrsAdjustmentExt2["Tsm_Misc6NDOTAmt"]   = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentExt2["Tsm_Misc6NDOTAmt"]), 2);
                            }
                            #endregion

                            #region Labor Hours Adjustment Computation
                            drLaborHrsAdjustment2["Tsa_OTAdjAmt"] = Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_REGOTAmt"])
                                                                            + Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_RESTAmt"])
                                                                            + Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_RESTOTAmt"])
                                                                            + Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_LEGHOLAmt"])
                                                                            + Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_LEGHOLOTAmt"])
                                                                            + Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_SPLHOLAmt"])
                                                                            + Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_SPLHOLOTAmt"])
                                                                            + Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_PSDAmt"])
                                                                            + Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_PSDOTAmt"])
                                                                            + Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_COMPHOLAmt"])
                                                                            + Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_COMPHOLOTAmt"])
                                                                            + Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_RESTLEGHOLAmt"])
                                                                            + Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_RESTLEGHOLOTAmt"])
                                                                            + Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_RESTSPLHOLAmt"])
                                                                            + Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_RESTSPLHOLOTAmt"])
                                                                            + Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_RESTCOMPHOLAmt"])
                                                                            + Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_RESTCOMPHOLOTAmt"])
                                                                            + Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_RESTPSDAmt"])
                                                                            + Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_RESTPSDOTAmt"]);

                            drLaborHrsAdjustment2["Tsa_RGAdjAmt"] = Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_REGAmt"])
                                                                             - Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_ABSAmt"]);

                            drLaborHrsAdjustment2["Tsa_HOLAdjAmt"] = Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_HOLAdjAmt"])
                                                                                        + Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_PDLEGHOLAmt"])
                                                                                        + Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_PDSPLHOLAmt"])
                                                                                        + Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_PDCOMPHOLAmt"])
                                                                                        + Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_PDOTHHOLAmt"])
                                                                                        + Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_PDRESTLEGHOLAmt"])
                                                                                        - Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_ABSLEGHOLAmt"])
                                                                                        - Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_ABSSPLHOLAmt"])
                                                                                        - Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_ABSCOMPHOLAmt"])
                                                                                        - Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_ABSPSDAmt"])
                                                                                        - Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_ABSOTHHOLAmt"]);

                            drLaborHrsAdjustment2["Tsa_NDAdjAmt"] = Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_REGNDAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_REGNDOTAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_RESTNDAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_RESTNDOTAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_LEGHOLNDAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_LEGHOLNDOTAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_SPLHOLNDAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_SPLHOLNDOTAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_PSDNDAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_PSDNDOTAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_COMPHOLNDAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_COMPHOLNDOTAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_RESTLEGHOLNDAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_RESTLEGHOLNDOTAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_RESTSPLHOLNDAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_RESTSPLHOLNDOTAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_RESTCOMPHOLNDAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_RESTCOMPHOLNDOTAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_RESTPSDNDAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_RESTPSDNDOTAmt"]);

                            drLaborHrsAdjustment2["Tsa_LVAdjAmt"] = Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_PDLVAmt"]);

                            if (bHasDayCodeExt)
                            {
                                drLaborHrsAdjustment2["Tsa_OTAdjAmt"] = Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_OTAdjAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentExt2["Tsm_Misc1Amt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentExt2["Tsm_Misc1OTAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentExt2["Tsm_Misc2Amt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentExt2["Tsm_Misc2OTAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentExt2["Tsm_Misc3Amt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentExt2["Tsm_Misc3OTAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentExt2["Tsm_Misc4Amt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentExt2["Tsm_Misc4OTAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentExt2["Tsm_Misc5Amt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentExt2["Tsm_Misc5OTAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentExt2["Tsm_Misc6Amt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentExt2["Tsm_Misc6OTAmt"]);

                                drLaborHrsAdjustment2["Tsa_NDAdjAmt"] = Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_NDAdjAmt"])
                                                                                                + Convert.ToDecimal(drLaborHrsAdjustmentExt2["Tsm_Misc1NDAmt"])
                                                                                                + Convert.ToDecimal(drLaborHrsAdjustmentExt2["Tsm_Misc1NDOTAmt"])
                                                                                                + Convert.ToDecimal(drLaborHrsAdjustmentExt2["Tsm_Misc2NDAmt"])
                                                                                                + Convert.ToDecimal(drLaborHrsAdjustmentExt2["Tsm_Misc2NDOTAmt"])
                                                                                                + Convert.ToDecimal(drLaborHrsAdjustmentExt2["Tsm_Misc3NDAmt"])
                                                                                                + Convert.ToDecimal(drLaborHrsAdjustmentExt2["Tsm_Misc3NDOTAmt"])
                                                                                                + Convert.ToDecimal(drLaborHrsAdjustmentExt2["Tsm_Misc4NDAmt"])
                                                                                                + Convert.ToDecimal(drLaborHrsAdjustmentExt2["Tsm_Misc4NDOTAmt"])
                                                                                                + Convert.ToDecimal(drLaborHrsAdjustmentExt2["Tsm_Misc5NDAmt"])
                                                                                                + Convert.ToDecimal(drLaborHrsAdjustmentExt2["Tsm_Misc5NDOTAmt"])
                                                                                                + Convert.ToDecimal(drLaborHrsAdjustmentExt2["Tsm_Misc6NDAmt"])
                                                                                                + Convert.ToDecimal(drLaborHrsAdjustmentExt2["Tsm_Misc6NDOTAmt"]);
                            }
                            #endregion

                            #region Rounding Off of Total Amounts
                            drLaborHrsAdjustment2["Tsa_RGAdjAmt"]    = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_RGAdjAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_OTAdjAmt"]    = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_OTAdjAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_HOLAdjAmt"]   = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_HOLAdjAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_NDAdjAmt"]    = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_NDAdjAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_LVAdjAmt"]    = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_LVAdjAmt"]), 2);
                            drLaborHrsAdjustment2["Tsa_TotalAdjAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_RGAdjAmt"])
                                                                            + Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_OTAdjAmt"])
                                                                            + Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_HOLAdjAmt"])
                                                                            + Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_NDAdjAmt"])
                                                                            + Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_LVAdjAmt"]), 2);
                            #endregion

                            #region Add to Adjustment Table
                            drLaborHrsAdjustment2["Tsa_TotalAdjAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_RGAdjAmt"])
                                                                            + Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_OTAdjAmt"])
                                                                            + Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_HOLAdjAmt"])
                                                                            + Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_NDAdjAmt"])
                                                                            + Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_LVAdjAmt"]), 2);


                            if (Convert.ToDecimal(drLaborHrsAdjustment2["Tsa_TotalAdjAmt"]) != 0)
                            {
                                dtLaborHrsAdjustment.Rows.Add(drLaborHrsAdjustment);
                                dtLaborHrsAdjustment2.Rows.Add(drLaborHrsAdjustment2);
                                if (bHasDayCodeExt)
                                {
                                    dtLaborHrsAdjustmentExt.Rows.Add(drLaborHrsAdjustmentExt);
                                    dtLaborHrsAdjustmentExt2.Rows.Add(drLaborHrsAdjustmentExt2);
                                }
                            }
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            CommonProcedures.logErrorToFile(ex.ToString());
                        }

                        //assign to prev
                        strPrevEmployeeID = strEmployeeID;
                    }
                }
                //-----------------------------
                //Save Adjustment Table
                StatusHandler(this, new StatusEventArgs("Saving Adjustment Records", false));
                string strUpdateRecordTemplate;
                string strUpdateQuery = "";
                int iUpdateCtr = 0;
                #region Adjustment Record Insert
                strUpdateRecordTemplate = @" INSERT INTO T_EmpSystemAdj (Tsa_IDNo,Tsa_AdjPayCycle,Tsa_OrigAdjPayCycle,Tsa_PayCycle,Tsa_Date,Tsa_LTHr,Tsa_UTHr,Tsa_UPLVHr,Tsa_ABSLEGHOLHr,Tsa_ABSSPLHOLHr,Tsa_ABSCOMPHOLHr,Tsa_ABSPSDHr,Tsa_ABSOTHHOLHr,Tsa_WDABSHr,Tsa_LTUTMaxHr,Tsa_ABSHr,Tsa_REGHr,Tsa_PDLVHr,Tsa_PDLEGHOLHr,Tsa_PDSPLHOLHr,Tsa_PDCOMPHOLHr,Tsa_PDPSDHr,Tsa_PDOTHHOLHr,Tsa_PDRESTLEGHOLHr,Tsa_REGOTHr,Tsa_REGNDHr,Tsa_REGNDOTHr,Tsa_RESTHr,Tsa_RESTOTHr,Tsa_RESTNDHr,Tsa_RESTNDOTHr,Tsa_LEGHOLHr,Tsa_LEGHOLOTHr,Tsa_LEGHOLNDHr,Tsa_LEGHOLNDOTHr,Tsa_SPLHOLHr,Tsa_SPLHOLOTHr,Tsa_SPLHOLNDHr,Tsa_SPLHOLNDOTHr,Tsa_PSDHr,Tsa_PSDOTHr,Tsa_PSDNDHr,Tsa_PSDNDOTHr,Tsa_COMPHOLHr,Tsa_COMPHOLOTHr,Tsa_COMPHOLNDHr,Tsa_COMPHOLNDOTHr,Tsa_RESTLEGHOLHr,Tsa_RESTLEGHOLOTHr,Tsa_RESTLEGHOLNDHr,Tsa_RESTLEGHOLNDOTHr,Tsa_RESTSPLHOLHr,Tsa_RESTSPLHOLOTHr,Tsa_RESTSPLHOLNDHr,Tsa_RESTSPLHOLNDOTHr,Tsa_RESTCOMPHOLHr,Tsa_RESTCOMPHOLOTHr,Tsa_RESTCOMPHOLNDHr,Tsa_RESTCOMPHOLNDOTHr,Tsa_RESTPSDHr,Tsa_RESTPSDOTHr,Tsa_RESTPSDNDHr,Tsa_RESTPSDNDOTHr,Tsa_PostFlag,Usr_Login,Ludatetime) VALUES('{0}','{1}','{2}','{3}','{4}',{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34},{35},{36},{37},{38},{39},{40},{41},{42},{43},{44},{45},{46},{47},{48},{49},{50},{51},{52},{53},{54},{55},{56},{57},{58},{59},{60},{61},{62},'{63}','{64}',GETDATE()) ";
                #endregion
                foreach (DataRow drLaborHrsAdjustment in dtLaborHrsAdjustment.Rows)
                {
                    #region Adjustment Insert
                    strUpdateQuery += string.Format(strUpdateRecordTemplate
                                                    , drLaborHrsAdjustment["Tsa_IDNo"]                       //0
                                                    , drLaborHrsAdjustment["Tsa_AdjPayCycle"]
                                                    , drLaborHrsAdjustment["Tsa_OrigAdjPayCycle"]           
                                                    , drLaborHrsAdjustment["Tsa_PayCycle"]
                                                    , drLaborHrsAdjustment["Tsa_Date"]
                                                    , drLaborHrsAdjustment["Tsa_LTHr"]                      //5
                                                    , drLaborHrsAdjustment["Tsa_UTHr"]                       
                                                    , drLaborHrsAdjustment["Tsa_UPLVHr"]
                                                    , drLaborHrsAdjustment["Tsa_ABSLEGHOLHr"]
                                                    , drLaborHrsAdjustment["Tsa_ABSSPLHOLHr"]        
                                                    , drLaborHrsAdjustment["Tsa_ABSCOMPHOLHr"]              //10
                                                    , drLaborHrsAdjustment["Tsa_ABSPSDHr"]                   
                                                    , drLaborHrsAdjustment["Tsa_ABSOTHHOLHr"]
                                                    , drLaborHrsAdjustment["Tsa_WDABSHr"]
                                                    , drLaborHrsAdjustment["Tsa_LTUTMaxHr"]
                                                    , drLaborHrsAdjustment["Tsa_ABSHr"]                     //15
                                                    , drLaborHrsAdjustment["Tsa_REGHr"]                     
                                                    , drLaborHrsAdjustment["Tsa_PDLVHr"]                    
                                                    , drLaborHrsAdjustment["Tsa_PDLEGHOLHr"]
                                                    , drLaborHrsAdjustment["Tsa_PDSPLHOLHr"]
                                                    , drLaborHrsAdjustment["Tsa_PDCOMPHOLHr"]               //20
                                                    , drLaborHrsAdjustment["Tsa_PDPSDHr"]                   
                                                    , drLaborHrsAdjustment["Tsa_PDOTHHOLHr"]                
                                                    , drLaborHrsAdjustment["Tsa_PDRESTLEGHOLHr"]
                                                    , drLaborHrsAdjustment["Tsa_REGOTHr"]
                                                    , drLaborHrsAdjustment["Tsa_REGNDHr"]                   //25
                                                    , drLaborHrsAdjustment["Tsa_REGNDOTHr"]                 
                                                    , drLaborHrsAdjustment["Tsa_RESTHr"]                    
                                                    , drLaborHrsAdjustment["Tsa_RESTOTHr"]
                                                    , drLaborHrsAdjustment["Tsa_RESTNDHr"]                   
                                                    , drLaborHrsAdjustment["Tsa_RESTNDOTHr"]                //30
                                                    , drLaborHrsAdjustment["Tsa_LEGHOLHr"]                  
                                                    , drLaborHrsAdjustment["Tsa_LEGHOLOTHr"]                
                                                    , drLaborHrsAdjustment["Tsa_LEGHOLNDHr"]
                                                    , drLaborHrsAdjustment["Tsa_LEGHOLNDOTHr"]
                                                    , drLaborHrsAdjustment["Tsa_SPLHOLHr"]                  //35
                                                    , drLaborHrsAdjustment["Tsa_SPLHOLOTHr"]                
                                                    , drLaborHrsAdjustment["Tsa_SPLHOLNDHr"]                
                                                    , drLaborHrsAdjustment["Tsa_SPLHOLNDOTHr"]
                                                    , drLaborHrsAdjustment["Tsa_PSDHr"]               
                                                    , drLaborHrsAdjustment["Tsa_PSDOTHr"]                   //40
                                                    , drLaborHrsAdjustment["Tsa_PSDNDHr"]                   
                                                    , drLaborHrsAdjustment["Tsa_PSDNDOTHr"]                 
                                                    , drLaborHrsAdjustment["Tsa_COMPHOLHr"]
                                                    , drLaborHrsAdjustment["Tsa_COMPHOLOTHr"]
                                                    , drLaborHrsAdjustment["Tsa_COMPHOLNDHr"]               //45
                                                    , drLaborHrsAdjustment["Tsa_COMPHOLNDOTHr"]             
                                                    , drLaborHrsAdjustment["Tsa_RESTLEGHOLHr"]              
                                                    , drLaborHrsAdjustment["Tsa_RESTLEGHOLOTHr"]
                                                    , drLaborHrsAdjustment["Tsa_RESTLEGHOLNDHr"]       
                                                    , drLaborHrsAdjustment["Tsa_RESTLEGHOLNDOTHr"]          //50
                                                    , drLaborHrsAdjustment["Tsa_RESTSPLHOLHr"]              
                                                    , drLaborHrsAdjustment["Tsa_RESTSPLHOLOTHr"]            
                                                    , drLaborHrsAdjustment["Tsa_RESTSPLHOLNDHr"]
                                                    , drLaborHrsAdjustment["Tsa_RESTSPLHOLNDOTHr"]
                                                    , drLaborHrsAdjustment["Tsa_RESTCOMPHOLHr"]             //55
                                                    , drLaborHrsAdjustment["Tsa_RESTCOMPHOLOTHr"]           
                                                    , drLaborHrsAdjustment["Tsa_RESTCOMPHOLNDHr"]           
                                                    , drLaborHrsAdjustment["Tsa_RESTCOMPHOLNDOTHr"]
                                                    , drLaborHrsAdjustment["Tsa_RESTPSDHr"]        
                                                    , drLaborHrsAdjustment["Tsa_RESTPSDOTHr"]               //60
                                                    , drLaborHrsAdjustment["Tsa_RESTPSDNDHr"]               
                                                    , drLaborHrsAdjustment["Tsa_RESTPSDNDOTHr"]             
                                                    , drLaborHrsAdjustment["Tsa_PostFlag"]                  
                                                    , drLaborHrsAdjustment["Usr_Login"]);                   //64
                    #endregion
                    iUpdateCtr++;
                    if (iUpdateCtr == 50)
                    {
                        dal.ExecuteNonQuery(strUpdateQuery);
                        strUpdateQuery = "";
                        iUpdateCtr = 0;
                    }
                }
                if (strUpdateQuery != "")
                    dal.ExecuteNonQuery(strUpdateQuery);
                //-----------------------------
                //Save Adjustment Detail Table
                strUpdateQuery = "";
                iUpdateCtr = 0;
                #region Adjustment Detail Record Insert
                strUpdateRecordTemplate = @" INSERT INTO T_EmpSystemAdj2 (Tsa_IDNo,Tsa_AdjPayCycle,Tsa_OrigAdjPayCycle,Tsa_PayCycle,Tsa_Date,Tsa_SalaryRate,Tsa_HourRate,Tsa_PayrollType,Tsa_SpecialRate,Tsa_SpecialHourRate,Tsa_PremiumGrpCode,Tsa_LTAmt,Tsa_UTAmt,Tsa_UPLVAmt,Tsa_ABSLEGHOLAmt,Tsa_ABSSPLHOLAmt,Tsa_ABSCOMPHOLAmt,Tsa_ABSPSDAmt,Tsa_ABSOTHHOLAmt,Tsa_WDABSAmt,Tsa_LTUTMaxAmt,Tsa_ABSAmt,Tsa_REGAmt,Tsa_PDLVAmt,Tsa_PDLEGHOLAmt,Tsa_PDSPLHOLAmt,Tsa_PDCOMPHOLAmt,Tsa_PDPSDAmt,Tsa_PDOTHHOLAmt,Tsa_PDRESTLEGHOLAmt,Tsa_REGOTAmt,Tsa_REGNDAmt,Tsa_REGNDOTAmt,Tsa_RESTAmt,Tsa_RESTOTAmt,Tsa_RESTNDAmt,Tsa_RESTNDOTAmt,Tsa_LEGHOLAmt,Tsa_LEGHOLOTAmt,Tsa_LEGHOLNDAmt,Tsa_LEGHOLNDOTAmt,Tsa_SPLHOLAmt,Tsa_SPLHOLOTAmt,Tsa_SPLHOLNDAmt,Tsa_SPLHOLNDOTAmt,Tsa_PSDAmt,Tsa_PSDOTAmt,Tsa_PSDNDAmt,Tsa_PSDNDOTAmt,Tsa_COMPHOLAmt,Tsa_COMPHOLOTAmt,Tsa_COMPHOLNDAmt,Tsa_COMPHOLNDOTAmt,Tsa_RESTLEGHOLAmt,Tsa_RESTLEGHOLOTAmt,Tsa_RESTLEGHOLNDAmt,Tsa_RESTLEGHOLNDOTAmt,Tsa_RESTSPLHOLAmt,Tsa_RESTSPLHOLOTAmt,Tsa_RESTSPLHOLNDAmt,Tsa_RESTSPLHOLNDOTAmt,Tsa_RESTCOMPHOLAmt,Tsa_RESTCOMPHOLOTAmt,Tsa_RESTCOMPHOLNDAmt,Tsa_RESTCOMPHOLNDOTAmt,Tsa_RESTPSDAmt,Tsa_RESTPSDOTAmt,Tsa_RESTPSDNDAmt,Tsa_RESTPSDNDOTAmt,Tsa_RGAdjAmt,Tsa_OTAdjAmt,Tsa_HOLAdjAmt,Tsa_NDAdjAmt,Tsa_LVAdjAmt,Tsa_TotalAdjAmt,Usr_Login,Ludatetime) VALUES ('{0}','{1}','{2}','{3}','{4}',{5},{6},'{7}',{8},{9},'{10}',{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34},{35},{36},{37},{38},{39},{40},{41},{42},{43},{44},{45},{46},{47},{48},{49},{50},{51},{52},{53},{54},{55},{56},{57},{58},{59},{60},{61},{62},{63},{64},{65},{66},{67},{68},{69},{70},{71},{72},{73},{74},'{75}',GETDATE()) ";
                #endregion
                foreach (DataRow drLaborHrsAdjustment2 in dtLaborHrsAdjustment2.Rows)
                {
                    #region Adjustment Detail Insert
                    strUpdateQuery += string.Format(strUpdateRecordTemplate
                                                    , drLaborHrsAdjustment2["Tsa_IDNo"]                         //0
                                                    , drLaborHrsAdjustment2["Tsa_AdjPayCycle"]                  //1
                                                    , drLaborHrsAdjustment2["Tsa_OrigAdjPayCycle"]
                                                    , drLaborHrsAdjustment2["Tsa_PayCycle"]
                                                    , drLaborHrsAdjustment2["Tsa_Date"]
                                                    , drLaborHrsAdjustment2["Tsa_SalaryRate"]                   //5
                                                    , drLaborHrsAdjustment2["Tsa_HourRate"]                     
                                                    , drLaborHrsAdjustment2["Tsa_PayrollType"]
                                                    , drLaborHrsAdjustment2["Tsa_SpecialRate"]
                                                    , drLaborHrsAdjustment2["Tsa_SpecialHourRate"]
                                                    , drLaborHrsAdjustment2["Tsa_PremiumGrpCode"]               //10
                                                    , drLaborHrsAdjustment2["Tsa_LTAmt"] 
                                                    , drLaborHrsAdjustment2["Tsa_UTAmt"]
                                                    , drLaborHrsAdjustment2["Tsa_UPLVAmt"]                                   
                                                    , drLaborHrsAdjustment2["Tsa_ABSLEGHOLAmt"]                 
                                                    , drLaborHrsAdjustment2["Tsa_ABSSPLHOLAmt"]                 //15
                                                    , drLaborHrsAdjustment2["Tsa_ABSCOMPHOLAmt"]
                                                    , drLaborHrsAdjustment2["Tsa_ABSPSDAmt"]
                                                    , drLaborHrsAdjustment2["Tsa_ABSOTHHOLAmt"]                 
                                                    , drLaborHrsAdjustment2["Tsa_WDABSAmt"]
                                                    , drLaborHrsAdjustment2["Tsa_LTUTMaxAmt"]                   //20
                                                    , drLaborHrsAdjustment2["Tsa_ABSAmt"]                       
                                                    , drLaborHrsAdjustment2["Tsa_REGAmt"]
                                                    , drLaborHrsAdjustment2["Tsa_PDLVAmt"]
                                                    , drLaborHrsAdjustment2["Tsa_PDLEGHOLAmt"]                  
                                                    , drLaborHrsAdjustment2["Tsa_PDSPLHOLAmt"]                  //25               
                                                    , drLaborHrsAdjustment2["Tsa_PDCOMPHOLAmt"]                 
                                                    , drLaborHrsAdjustment2["Tsa_PDPSDAmt"]
                                                    , drLaborHrsAdjustment2["Tsa_PDOTHHOLAmt"]
                                                    , drLaborHrsAdjustment2["Tsa_PDRESTLEGHOLAmt"]              
                                                    , drLaborHrsAdjustment2["Tsa_REGOTAmt"]                    //30        
                                                    , drLaborHrsAdjustment2["Tsa_REGNDAmt"]                     
                                                    , drLaborHrsAdjustment2["Tsa_REGNDOTAmt"]
                                                    , drLaborHrsAdjustment2["Tsa_RESTAmt"]
                                                    , drLaborHrsAdjustment2["Tsa_RESTOTAmt"]                    
                                                    , drLaborHrsAdjustment2["Tsa_RESTNDAmt"]                   //35        
                                                    , drLaborHrsAdjustment2["Tsa_RESTNDOTAmt"]                  
                                                    , drLaborHrsAdjustment2["Tsa_LEGHOLAmt"]
                                                    , drLaborHrsAdjustment2["Tsa_LEGHOLOTAmt"]
                                                    , drLaborHrsAdjustment2["Tsa_LEGHOLNDAmt"]                  
                                                    , drLaborHrsAdjustment2["Tsa_LEGHOLNDOTAmt"]                //40
                                                    , drLaborHrsAdjustment2["Tsa_SPLHOLAmt"]                    
                                                    , drLaborHrsAdjustment2["Tsa_SPLHOLOTAmt"]
                                                    , drLaborHrsAdjustment2["Tsa_SPLHOLNDAmt"]
                                                    , drLaborHrsAdjustment2["Tsa_SPLHOLNDOTAmt"]                
                                                    , drLaborHrsAdjustment2["Tsa_PSDAmt"]                       //45            
                                                    , drLaborHrsAdjustment2["Tsa_PSDOTAmt"]                     
                                                    , drLaborHrsAdjustment2["Tsa_PSDNDAmt"]
                                                    , drLaborHrsAdjustment2["Tsa_PSDNDOTAmt"]
                                                    , drLaborHrsAdjustment2["Tsa_COMPHOLAmt"]                   
                                                    , drLaborHrsAdjustment2["Tsa_COMPHOLOTAmt"]                 //50
                                                    , drLaborHrsAdjustment2["Tsa_COMPHOLNDAmt"]                 
                                                    , drLaborHrsAdjustment2["Tsa_COMPHOLNDOTAmt"]
                                                    , drLaborHrsAdjustment2["Tsa_RESTLEGHOLAmt"]
                                                    , drLaborHrsAdjustment2["Tsa_RESTLEGHOLOTAmt"]              
                                                    , drLaborHrsAdjustment2["Tsa_RESTLEGHOLNDAmt"]              //55
                                                    , drLaborHrsAdjustment2["Tsa_RESTLEGHOLNDOTAmt"]            
                                                    , drLaborHrsAdjustment2["Tsa_RESTSPLHOLAmt"]
                                                    , drLaborHrsAdjustment2["Tsa_RESTSPLHOLOTAmt"]
                                                    , drLaborHrsAdjustment2["Tsa_RESTSPLHOLNDAmt"]              
                                                    , drLaborHrsAdjustment2["Tsa_RESTSPLHOLNDOTAmt"]            //60
                                                    , drLaborHrsAdjustment2["Tsa_RESTCOMPHOLAmt"]               
                                                    , drLaborHrsAdjustment2["Tsa_RESTCOMPHOLOTAmt"]
                                                    , drLaborHrsAdjustment2["Tsa_RESTCOMPHOLNDAmt"]
                                                    , drLaborHrsAdjustment2["Tsa_RESTCOMPHOLNDOTAmt"]           
                                                    , drLaborHrsAdjustment2["Tsa_RESTPSDAmt"]                   //65
                                                    , drLaborHrsAdjustment2["Tsa_RESTPSDOTAmt"]                                 
                                                    , drLaborHrsAdjustment2["Tsa_RESTPSDNDAmt"]
                                                    , drLaborHrsAdjustment2["Tsa_RESTPSDNDOTAmt"]
                                                    , drLaborHrsAdjustment2["Tsa_RGAdjAmt"]                     
                                                    , drLaborHrsAdjustment2["Tsa_OTAdjAmt"]                     //70
                                                    , drLaborHrsAdjustment2["Tsa_HOLAdjAmt"]                                    
                                                    , drLaborHrsAdjustment2["Tsa_NDAdjAmt"]
                                                    , drLaborHrsAdjustment2["Tsa_LVAdjAmt"]
                                                    , drLaborHrsAdjustment2["Tsa_TotalAdjAmt"]                  
                                                    , drLaborHrsAdjustment2["Usr_Login"]);                      //75                      
                    #endregion
                    iUpdateCtr++;
                    if (iUpdateCtr == 50)
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
                    //Save Adjustment Ext Table
                    strUpdateQuery = "";
                    iUpdateCtr = 0;
                    #region Adjustment Ext Record Insert
                    strUpdateRecordTemplate = @" INSERT INTO T_EmpSystemAdjMisc (Tsm_IDNo,Tsm_AdjPayCycle,Tsm_OrigAdjPayCycle,Tsm_PayCycle,Tsm_Date,Tsm_Misc1Hr,Tsm_Misc1OTHr,Tsm_Misc1NDHr,Tsm_Misc1NDOTHr,Tsm_Misc2Hr,Tsm_Misc2OTHr,Tsm_Misc2NDHr,Tsm_Misc2NDOTHr,Tsm_Misc3Hr,Tsm_Misc3OTHr,Tsm_Misc3NDHr,Tsm_Misc3NDOTHr,Tsm_Misc4Hr,Tsm_Misc4OTHr,Tsm_Misc4NDHr,Tsm_Misc4NDOTHr,Tsm_Misc5Hr,Tsm_Misc5OTHr,Tsm_Misc5NDHr,Tsm_Misc5NDOTHr,Tsm_Misc6Hr,Tsm_Misc6OTHr,Tsm_Misc6NDHr,Tsm_Misc6NDOTHr,Usr_Login,Ludatetime) VALUES('{0}','{1}','{2}','{3}','{4}',{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},'{29}',GETDATE()) ";
                    #endregion
                    foreach (DataRow drLaborHrsAdjustmentExt in dtLaborHrsAdjustmentExt.Rows)
                    {
                        #region Adjustment Ext Insert
                        strUpdateQuery += string.Format(strUpdateRecordTemplate
                                                          , drLaborHrsAdjustmentExt["Tsm_IDNo"]                     //0
                                                          , drLaborHrsAdjustmentExt["Tsm_AdjPayCycle"]
                                                          , drLaborHrsAdjustmentExt["Tsm_OrigAdjPayCycle"]
                                                          , drLaborHrsAdjustmentExt["Tsm_PayCycle"]
                                                          , drLaborHrsAdjustmentExt["Tsm_Date"]
                                                          , drLaborHrsAdjustmentExt["Tsm_Misc1Hr"]                  //5
                                                          , drLaborHrsAdjustmentExt["Tsm_Misc1OTHr"]                
                                                          , drLaborHrsAdjustmentExt["Tsm_Misc1NDHr"]
                                                          , drLaborHrsAdjustmentExt["Tsm_Misc1NDOTHr"]
                                                          , drLaborHrsAdjustmentExt["Tsm_Misc2Hr"]
                                                          , drLaborHrsAdjustmentExt["Tsm_Misc2OTHr"]                //10
                                                          , drLaborHrsAdjustmentExt["Tsm_Misc2NDHr"]                
                                                          , drLaborHrsAdjustmentExt["Tsm_Misc2NDOTHr"]
                                                          , drLaborHrsAdjustmentExt["Tsm_Misc3Hr"]
                                                          , drLaborHrsAdjustmentExt["Tsm_Misc3OTHr"]
                                                          , drLaborHrsAdjustmentExt["Tsm_Misc3NDHr"]                //15
                                                          , drLaborHrsAdjustmentExt["Tsm_Misc3NDOTHr"]              
                                                          , drLaborHrsAdjustmentExt["Tsm_Misc4Hr"]
                                                          , drLaborHrsAdjustmentExt["Tsm_Misc4OTHr"]
                                                          , drLaborHrsAdjustmentExt["Tsm_Misc4NDHr"]
                                                          , drLaborHrsAdjustmentExt["Tsm_Misc4NDOTHr"]              //20
                                                          , drLaborHrsAdjustmentExt["Tsm_Misc5Hr"]                  
                                                          , drLaborHrsAdjustmentExt["Tsm_Misc5OTHr"]
                                                          , drLaborHrsAdjustmentExt["Tsm_Misc5NDHr"]
                                                          , drLaborHrsAdjustmentExt["Tsm_Misc5NDOTHr"]
                                                          , drLaborHrsAdjustmentExt["Tsm_Misc6Hr"]                  //25
                                                          , drLaborHrsAdjustmentExt["Tsm_Misc6OTHr"]                
                                                          , drLaborHrsAdjustmentExt["Tsm_Misc6NDHr"]
                                                          , drLaborHrsAdjustmentExt["Tsm_Misc6NDOTHr"]
                                                          , drLaborHrsAdjustmentExt["Usr_Login"]);                  //29
                        #endregion
                        iUpdateCtr++;
                        if (iUpdateCtr == 50)
                        {
                            dal.ExecuteNonQuery(strUpdateQuery);
                            strUpdateQuery = "";
                            iUpdateCtr = 0;
                        }
                    }
                    if (strUpdateQuery != "")
                        dal.ExecuteNonQuery(strUpdateQuery);
                    //-----------------------------
                    //Save Adjustment Ext Detail Table
                    strUpdateQuery = "";
                    iUpdateCtr = 0;
                    #region Adjustment Ext Detail Record Insert
                    strUpdateRecordTemplate = @" INSERT INTO T_EmpSystemAdjMisc2 (Tsm_IDNo,Tsm_AdjPayCycle,Tsm_OrigAdjPayCycle,Tsm_PayCycle,Tsm_Date,Tsm_Misc1Amt,Tsm_Misc1OTAmt,Tsm_Misc1NDAmt,Tsm_Misc1NDOTAmt,Tsm_Misc2Amt,Tsm_Misc2OTAmt,Tsm_Misc2NDAmt,Tsm_Misc2NDOTAmt,Tsm_Misc3Amt,Tsm_Misc3OTAmt,Tsm_Misc3NDAmt,Tsm_Misc3NDOTAmt,Tsm_Misc4Amt,Tsm_Misc4OTAmt,Tsm_Misc4NDAmt,Tsm_Misc4NDOTAmt,Tsm_Misc5Amt,Tsm_Misc5OTAmt,Tsm_Misc5NDAmt,Tsm_Misc5NDOTAmt,Tsm_Misc6Amt,Tsm_Misc6OTAmt,Tsm_Misc6NDAmt,Tsm_Misc6NDOTAmt,Usr_Login,Ludatetime) VALUES ('{0}','{1}','{2}','{3}','{4}',{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},'{29}',GETDATE()) ";
                    #endregion
                    foreach (DataRow drLaborHrsAdjustmentExt2 in dtLaborHrsAdjustmentExt2.Rows)
                    {
                        #region Adjustment Ext Detail Insert
                        strUpdateQuery += string.Format(strUpdateRecordTemplate
                                                          , drLaborHrsAdjustmentExt2["Tsm_IDNo"]                   //0
                                                          , drLaborHrsAdjustmentExt2["Tsm_AdjPayCycle"]
                                                          , drLaborHrsAdjustmentExt2["Tsm_OrigAdjPayCycle"]
                                                          , drLaborHrsAdjustmentExt2["Tsm_PayCycle"]
                                                          , drLaborHrsAdjustmentExt2["Tsm_Date"]
                                                          , drLaborHrsAdjustmentExt2["Tsm_Misc1Amt"]                //5
                                                          , drLaborHrsAdjustmentExt2["Tsm_Misc1OTAmt"]
                                                          , drLaborHrsAdjustmentExt2["Tsm_Misc1NDAmt"]
                                                          , drLaborHrsAdjustmentExt2["Tsm_Misc1NDOTAmt"]
                                                          , drLaborHrsAdjustmentExt2["Tsm_Misc2Amt"]
                                                          , drLaborHrsAdjustmentExt2["Tsm_Misc2OTAmt"]              //10
                                                          , drLaborHrsAdjustmentExt2["Tsm_Misc2NDAmt"]                
                                                          , drLaborHrsAdjustmentExt2["Tsm_Misc2NDOTAmt"]
                                                          , drLaborHrsAdjustmentExt2["Tsm_Misc3Amt"]
                                                          , drLaborHrsAdjustmentExt2["Tsm_Misc3OTAmt"]
                                                          , drLaborHrsAdjustmentExt2["Tsm_Misc3NDAmt"]              //15
                                                          , drLaborHrsAdjustmentExt2["Tsm_Misc3NDOTAmt"]
                                                          , drLaborHrsAdjustmentExt2["Tsm_Misc4Amt"]
                                                          , drLaborHrsAdjustmentExt2["Tsm_Misc4OTAmt"]
                                                          , drLaborHrsAdjustmentExt2["Tsm_Misc4NDAmt"]
                                                          , drLaborHrsAdjustmentExt2["Tsm_Misc4NDOTAmt"]            //20
                                                          , drLaborHrsAdjustmentExt2["Tsm_Misc5Amt"]                  
                                                          , drLaborHrsAdjustmentExt2["Tsm_Misc5OTAmt"]
                                                          , drLaborHrsAdjustmentExt2["Tsm_Misc5NDAmt"]
                                                          , drLaborHrsAdjustmentExt2["Tsm_Misc5NDOTAmt"]
                                                          , drLaborHrsAdjustmentExt2["Tsm_Misc6Amt"]                //25
                                                          , drLaborHrsAdjustmentExt2["Tsm_Misc6OTAmt"]
                                                          , drLaborHrsAdjustmentExt2["Tsm_Misc6NDAmt"]
                                                          , drLaborHrsAdjustmentExt2["Tsm_Misc6NDOTAmt"]
                                                          , drLaborHrsAdjustmentExt2["Usr_Login"]);                 //29
                        #endregion
                        iUpdateCtr++;
                        if (iUpdateCtr == 50)
                        {
                            dal.ExecuteNonQuery(strUpdateQuery);
                            strUpdateQuery = "";
                            iUpdateCtr = 0;
                        }
                    }
                    if (strUpdateQuery != "")
                        dal.ExecuteNonQuery(strUpdateQuery);
                }
                StatusHandler(this, new StatusEventArgs("Saving Adjustment Records", true));
                //-----------------------------
                //code end
                //dal.CommitTransactionSnapshot();
            }
            catch (Exception ex)
            {
                //dal.RollBackTransactionSnapshot();
                CommonProcedures.logErrorToFile(ex.ToString());
                throw new Exception("Labor hours adjustment has encountered some errors: \n" + ex.Message);
            }
            finally
            {
                //dal.CloseDB();
            }
        }

        public void SetProcessFlags()
        {
            string strResult = string.Empty;

            strResult = commonBL.GetParameterValueFromPayroll("MDIVISOR", CompanyCode, dal);
            MDIVISOR = strResult.Equals(string.Empty) ? 0 : Convert.ToDecimal(strResult);

            strResult = commonBL.GetParameterValueFromPayroll("HRLYRTEDEC", CompanyCode, dal);
            HRLYRTEDEC = strResult.Equals(string.Empty) ? 0 : Convert.ToDecimal(strResult);

            PAYSPECIALRATE = commonBL.GetParameterValueFromPayroll("HRRTINCDEM", CompanyCode, dal);
            if (Convert.ToBoolean(PAYSPECIALRATE))
            {
                PAYCODERATE = commonBL.GetParameterDtlListfromPayroll("PAYCODERATE", CompanyCode, dal);
                //if (PAYCODERATE.Rows.Count == 0)
                //    PayrollProcessingErrorMessage += "Special Pay Code Rate is not set-up in Master.\n";
                //StatusHandler(this, new StatusEventArgs(string.Format("Getting special rate set-up"), false));
                //StatusHandler(this, new StatusEventArgs(string.Format("Getting special rate set-up"), true));

                SPECIALRATEFORMULA_SRATE = commonBL.GetParameterFormulaFromPayroll("SPECIALRATEFRM-SRATE", CompanyCode, dal);
                //if (SPECIALRATEFORMULA_SRATE == "")
                //    PayrollProcessingErrorMessage += "Special Salary Rate is not set-up in Master.\n";
                //StatusHandler(this, new StatusEventArgs(string.Format("Getting special salary rate set-up"), false));
                //StatusHandler(this, new StatusEventArgs(string.Format("Getting special salary rate set-up"), true));

                SPECIALRATEFORMULA_HRATE = commonBL.GetParameterFormulaFromPayroll("SPECIALRATEFRM-HRATE", CompanyCode, dal);
                //if (SPECIALRATEFORMULA_HRATE == "")
                //    PayrollProcessingErrorMessage += "Special Hourly Rate is not set-up in Master.\n";
                //StatusHandler(this, new StatusEventArgs(string.Format("Getting special hourly rate set-up"), false));
                //StatusHandler(this, new StatusEventArgs(string.Format("Getting special hourly rate set-up"), true));
            }

            strResult = commonBL.GetParameterValueFromPayroll("NDBRCKTCNT", CompanyCode, dal);
            NDBRCKTCNT = strResult.Equals(string.Empty) ? 1 : Convert.ToInt32(Convert.ToDouble(strResult));

            NP1_RATE = Convert.ToDecimal(commonBL.GetParameterDtlValueFromPayroll("NDBRACKET", "NP1_RATE", CompanyCode)) / 100;
            NP2_RATE = Convert.ToDecimal(commonBL.GetParameterDtlValueFromPayroll("NDBRACKET", "NP2_RATE", CompanyCode)) / 100;
        }

        private void ShowEmployeeName(object sender, NewLaborHoursGenerationBL.EmpDispEventArgs e)
        {
            EmpDispHandler(this, new EmpDispEventArgs(e.EmployeeId, e.LastName, e.FirstName, "LABOR HOURS (HISTORY)"));
        }

        private void ShowLaborHourStatus(object sender, NewLaborHoursGenerationBL.StatusEventArgs e)
        {
            StatusHandler(this, new StatusEventArgs(e.Status, true));
        }

        public int CountAllEmployeeForProcess(string PayPeriod, DALHelper dalHelper)
        {
            string query = string.Format(@"SELECT count(DISTINCT Ttr_IDNo)
                                            FROM	T_EmpTimeRegisterTrl 
                                            JOIN	M_Employee
                                            ON		Mem_IDNo = Ttr_IDNo
                                            WHERE	Ttr_AdjPayCycle = '{0}'", PayPeriod);
            DataTable dtResult = dalHelper.ExecuteDataSet(query).Tables[0];
            return Convert.ToInt32(dtResult.Rows[0][0].ToString());
        }

        public void ClearTransactionTables(bool ProcessAll, string EmployeeId, string PayPeriod)
        {
            string EmployeeConditionHdr = "";
            string EmployeeConditionDtl = ""; //new
            if (!ProcessAll && EmployeeId != "")
                EmployeeConditionHdr += " AND Tph_IDNo = '" + EmployeeId + "'";
                EmployeeConditionDtl += " AND Tpd_IDNo = '" + EmployeeId + "'"; //new

            string query = string.Format(@"DELETE FROM T_EmpPayTranHdrHst WHERE Tph_PayCycle = '{0}' {1}
                                           DELETE FROM T_EmpPayTranHdrMiscHst WHERE Tph_PayCycle = '{0}' {1}
                                           DELETE FROM T_EmpPayTranDtlHst WHERE Tpd_PayCycle = '{0}' {2}
                                           DELETE FROM T_EmpPayTranDtlMiscHst WHERE Tpd_PayCycle = '{0}' {2}", PayPeriod, EmployeeConditionHdr,EmployeeConditionDtl);
            dal.ExecuteNonQuery(query);

            query = string.Format(@"DELETE FROM T_EmpPayTranHdrTrl WHERE Tph_PayCycle = '{0}' AND Tph_AdjPayCycle = '{1}' {2}
                                    DELETE FROM T_EmpPayTranHdrMiscTrl WHERE Tph_PayCycle = '{0}' AND Tpm_AdjPayCycle = '{1}' {2}
                                    DELETE FROM T_EmpPayTranDtlTrl WHERE Tpd_PayCycle = '{0}' AND Tpd_AdjPayCycle = '{1}' {3}
                                    DELETE FROM T_EmpPayTranDtlMiscTrl WHERE Tpd_PayCycle = '{0}' AND Tpd_AdjPayCycle = '{1}' {3}", PayPeriod, ProcessPayrollPeriod, EmployeeConditionHdr,EmployeeConditionDtl);
            dal.ExecuteNonQuery(query);
        }

        public void ClearAdjustmentTable(bool ProcessAll, string EmployeeId)
        {
            string EmployeeCondition1 = "";
            string EmployeeCondition2 = "";
            string EmployeeCondition3 = "";
            if (!ProcessAll && EmployeeId != "")
            {
                EmployeeCondition1 += " AND Tsa_IDNo = '" + EmployeeId + "'";
                EmployeeCondition2 += " AND Tta_IDNo = '" + EmployeeId + "'";
                EmployeeCondition3 += " AND Tsm_IDNo = '" + EmployeeId + "'";
            }
            else if (ProcessAll && EmployeeList != "")
            {
                EmployeeCondition1 += " AND Tsa_IDNo IN (" + EmployeeList + ")";
                EmployeeCondition2 += " AND Tta_IDNo IN (" + EmployeeList + ")";
                EmployeeCondition3 += " AND Tsm_IDNo IN (" + EmployeeList + ")";
            }

            string query = string.Format(@"DELETE FROM T_EmpSystemAdj WHERE Tsa_AdjPayCycle = Tsa_OrigAdjPayCycle AND Tsa_AdjPayCycle = '{0}' {1}
                                           DELETE FROM T_EmpSystemAdjMisc WHERE Tsm_AdjPayCycle = Tsm_OrigAdjPayCycle AND Tsm_AdjPayCycle = '{0}' {3}
                                           DELETE FROM T_EmpSystemAdj2 WHERE Tsa_AdjPayCycle = Tsa_OrigAdjPayCycle AND Tsa_AdjPayCycle = '{0}' {1}
                                           DELETE FROM T_EmpSystemAdjMisc2 WHERE Tsm_AdjPayCycle = Tsm_OrigAdjPayCycle AND Tsm_AdjPayCycle = '{0}' {3}
                                           DELETE FROM T_EmpTimeBaseAllowanceCycle WHERE Tta_CycleIndicator = 'P' AND Tta_PayCycle = Tta_OrigPayCycle AND Tta_PayCycle = '{0}' {2}", ProcessPayrollPeriod, EmployeeCondition1, EmployeeCondition2, EmployeeCondition3);
            dal.ExecuteNonQuery(query);
        }

        public DataTable GetAllEmployeesForAdjustment(bool ProcessAll, string EmployeeId)
        {
            string EmployeeCondition = "";
            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition += " AND TRAIL.Ttr_IDNo = '" + EmployeeId + "'";
            else if (ProcessAll && EmployeeList != "")
                EmployeeCondition += " AND TRAIL.Ttr_IDNo IN (" + EmployeeList + ")";

            string query = string.Format(@"SELECT DISTINCT TRAIL.Ttr_IDNo
			                                            , TRAIL.Ttr_PayCycle
			                                            , Ttr_AdjPayCycle
			                                            , Mem_LastName
			                                            , Mem_FirstName
                                            FROM T_EmpTimeRegisterTrl TRAIL
                                            INNER JOIN M_Employee ON Mem_IDNo = TRAIL.Ttr_IDNo
                                            ---INNER JOIN (
                                             ----   SELECT Mpd_SubCode 
                                            ---    FROM M_PolicyDtl 
                                            ---    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                                            ---        AND Mpd_ParamValue = 1
                                            ---        AND Mpd_CompanyCode = '{2}'
                                           --- ) EMPSTAT
                                            ----    ON Mem_EmploymentStatusCode = Mpd_SubCode
                                            INNER JOIN T_EmpTimeRegisterHst HIST
                                            ON HIST.Ttr_IDNo = TRAIL.Ttr_IDNo
	                                            AND HIST.Ttr_PayCycle = TRAIL.Ttr_PayCycle
	                                            AND HIST.Ttr_Date = TRAIL.Ttr_Date
	                                            AND TRAIL.Ttr_AdjPayCycle = '{0}'
	                                            AND (HIST.Ttr_ActIn_1 != TRAIL.Ttr_ActIn_1
		                                            OR HIST.Ttr_ActOut_1 != TRAIL.Ttr_ActOut_1
		                                            OR HIST.Ttr_ActIn_2 != TRAIL.Ttr_ActIn_2
		                                            OR HIST.Ttr_ActOut_2 != TRAIL.Ttr_ActOut_2
		                                            OR HIST.Ttr_ShiftCode != TRAIL.Ttr_ShiftCode
		                                            OR HIST.Ttr_DayCode != TRAIL.Ttr_DayCode
		                                            OR HIST.Ttr_RestDayFlag != TRAIL.Ttr_RestDayFlag
		                                            OR HIST.Ttr_HolidayFlag != TRAIL.Ttr_HolidayFlag
		                                            OR HIST.Ttr_WFPayLVCode != TRAIL.Ttr_WFPayLVCode
		                                            OR HIST.Ttr_WFPayLVHr != TRAIL.Ttr_WFPayLVHr
		                                            OR HIST.Ttr_WFNoPayLVCode != TRAIL.Ttr_WFNoPayLVCode
		                                            OR HIST.Ttr_WFNoPayLVHr != TRAIL.Ttr_WFNoPayLVHr
		                                            OR HIST.Ttr_WFOTAdvHr != TRAIL.Ttr_WFOTAdvHr
		                                            OR HIST.Ttr_WFOTPostHr != TRAIL.Ttr_WFOTPostHr
		                                            OR HIST.Ttr_OTMin != TRAIL.Ttr_OTMin
		                                            OR HIST.Ttr_REGHour != TRAIL.Ttr_REGHour
		                                            OR HIST.Ttr_OTHour != TRAIL.Ttr_OTHour
													OR HIST.Ttr_NDHour != TRAIL.Ttr_NDHour
		                                            OR HIST.Ttr_NDOTHour != TRAIL.Ttr_NDOTHour
													OR HIST.Ttr_LVHour != TRAIL.Ttr_LVHour
		                                            OR HIST.Ttr_ABSHour != TRAIL.Ttr_ABSHour
                                                    OR ISNULL(HIST.Ttr_AssumedPost, '') != ISNULL(TRAIL.Ttr_AssumedPost, '')
		                                            OR HIST.Ttr_AssumedFlag != TRAIL.Ttr_AssumedFlag
		                                            OR HIST.Ttr_PDRESTLEGHOLDay != TRAIL.Ttr_PDRESTLEGHOLDay
                                                    OR HIST.Ttr_Amnesty != TRAIL.Ttr_Amnesty
                                                    OR ISNULL(HIST.Ttr_PDHOLHour, 0) != ISNULL(TRAIL.Ttr_PDHOLHour, 0)
													OR HIST.Ttr_TBAmt01 != TRAIL.Ttr_TBAmt01
													OR HIST.Ttr_TBAmt02 != TRAIL.Ttr_TBAmt02
													OR HIST.Ttr_TBAmt03 != TRAIL.Ttr_TBAmt03
													OR HIST.Ttr_TBAmt04 != TRAIL.Ttr_TBAmt04
													OR HIST.Ttr_TBAmt05 != TRAIL.Ttr_TBAmt05
													OR HIST.Ttr_TBAmt06 != TRAIL.Ttr_TBAmt06
													OR HIST.Ttr_TBAmt07 != TRAIL.Ttr_TBAmt07
													OR HIST.Ttr_TBAmt08 != TRAIL.Ttr_TBAmt08
													OR HIST.Ttr_TBAmt09 != TRAIL.Ttr_TBAmt09
													OR HIST.Ttr_TBAmt10 != TRAIL.Ttr_TBAmt10
													OR HIST.Ttr_TBAmt11 != TRAIL.Ttr_TBAmt11
													OR HIST.Ttr_TBAmt12 != TRAIL.Ttr_TBAmt12)
                                                {1}"
                                                    , ProcessPayrollPeriod
                                                    , EmployeeCondition
                                                    , CompanyCode);
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public DataTable GetAllEmployeesForAdjustmentMultiPockets(bool ProcessAll, string EmployeeId)
        {
            string EmployeeCondition = "";
            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition += " AND TRAIL.Ttr_IDNo = '" + EmployeeId + "'";
            else if (ProcessAll && EmployeeList != "")
                EmployeeCondition += " AND TRAIL.Ttr_IDNo IN (" + EmployeeList + ")";

            string query = string.Format(@"SELECT DISTINCT TRAIL.Ttr_IDNo
			                                  , TRAIL.Ttr_PayCycle
			                                  , Ttr_AdjPayCycle
			                                  , Mem_LastName
			                                  , Mem_FirstName
                                            FROM T_EmpTimeRegisterTrl TRAIL
                                            INNER JOIN M_Employee ON Mem_IDNo = TRAIL.Ttr_IDNo
                                            INNER JOIN T_EmpTimeRegisterHst HIST ON HIST.Ttr_IDNo = TRAIL.Ttr_IDNo
	                                            AND HIST.Ttr_PayCycle = TRAIL.Ttr_PayCycle
	                                            AND HIST.Ttr_Date = TRAIL.Ttr_Date
                                            INNER JOIN T_EmpTimeRegisterMiscTrl MISCTRL ON MISCTRL.Ttm_IDNo = TRAIL.Ttr_IDNo
	                                            AND MISCTRL.Ttm_Date = TRAIL.Ttr_Date
	                                            AND MISCTRL.Ttm_AdjPayCycle = TRAIL.Ttr_AdjPayCycle
                                            INNER JOIN T_EmpTimeRegisterMiscHst MISCHST ON MISCHST.Ttm_IDNo = HIST.Ttr_IDNo 
	                                            AND MISCHST.Ttm_Date = HIST.Ttr_Date
	                                        WHERE TRAIL.Ttr_AdjPayCycle = '{0}'
                                                {1}
	                                            AND (MISCHST.Ttm_ActIn_01 != MISCTRL.Ttm_ActIn_01
		                                            OR MISCHST.Ttm_ActOut_01 != MISCTRL.Ttm_ActOut_01
		                                            OR MISCHST.Ttm_ActIn_02 != MISCTRL.Ttm_ActIn_02
		                                            OR MISCHST.Ttm_ActOut_02 != MISCTRL.Ttm_ActOut_02
		                                            OR MISCHST.Ttm_ActIn_03 != MISCTRL.Ttm_ActIn_03
		                                            OR MISCHST.Ttm_ActOut_03 != MISCTRL.Ttm_ActOut_03
		                                            OR MISCHST.Ttm_ActIn_04 != MISCTRL.Ttm_ActIn_04
		                                            OR MISCHST.Ttm_ActOut_04 != MISCTRL.Ttm_ActOut_04
		                                            OR MISCHST.Ttm_ActIn_05 != MISCTRL.Ttm_ActIn_05
		                                            OR MISCHST.Ttm_ActOut_05 != MISCTRL.Ttm_ActOut_05
		                                            OR MISCHST.Ttm_ActIn_06 != MISCTRL.Ttm_ActIn_06
		                                            OR MISCHST.Ttm_ActOut_06 != MISCTRL.Ttm_ActOut_06
		                                            OR MISCHST.Ttm_ActIn_07 != MISCTRL.Ttm_ActIn_07
		                                            OR MISCHST.Ttm_ActOut_07 != MISCTRL.Ttm_ActOut_07
		                                            OR MISCHST.Ttm_ActIn_08 != MISCTRL.Ttm_ActIn_08
		                                            OR MISCHST.Ttm_ActOut_08 != MISCTRL.Ttm_ActOut_08
		                                            OR MISCHST.Ttm_ActIn_09 != MISCTRL.Ttm_ActIn_09
		                                            OR MISCHST.Ttm_ActOut_09 != MISCTRL.Ttm_ActOut_09
		                                            OR MISCHST.Ttm_ActIn_10 != MISCTRL.Ttm_ActIn_10
		                                            OR MISCHST.Ttm_ActOut_10 != MISCTRL.Ttm_ActOut_10
		                                            OR MISCHST.Ttm_ActIn_11 != MISCTRL.Ttm_ActIn_11
		                                            OR MISCHST.Ttm_ActOut_11 != MISCTRL.Ttm_ActOut_11
		                                            OR MISCHST.Ttm_ActIn_12 != MISCTRL.Ttm_ActIn_12
		                                            OR MISCHST.Ttm_ActOut_12 != MISCTRL.Ttm_ActOut_12
		                                            OR HIST.Ttr_ShiftCode != TRAIL.Ttr_ShiftCode
		                                            OR HIST.Ttr_DayCode != TRAIL.Ttr_DayCode
		                                            OR HIST.Ttr_RestDayFlag != TRAIL.Ttr_RestDayFlag
		                                            OR HIST.Ttr_HolidayFlag != TRAIL.Ttr_HolidayFlag
		                                            OR HIST.Ttr_WFPayLVCode != TRAIL.Ttr_WFPayLVCode
		                                            OR HIST.Ttr_WFPayLVHr != TRAIL.Ttr_WFPayLVHr
		                                            OR HIST.Ttr_WFNoPayLVCode != TRAIL.Ttr_WFNoPayLVCode
		                                            OR HIST.Ttr_WFNoPayLVHr != TRAIL.Ttr_WFNoPayLVHr
		                                            OR HIST.Ttr_WFOTAdvHr != TRAIL.Ttr_WFOTAdvHr
		                                            OR HIST.Ttr_WFOTPostHr != TRAIL.Ttr_WFOTPostHr
		                                            OR HIST.Ttr_OTMin != TRAIL.Ttr_OTMin
		                                            OR HIST.Ttr_REGHour != TRAIL.Ttr_REGHour
		                                            OR HIST.Ttr_OTHour != TRAIL.Ttr_OTHour
													OR HIST.Ttr_NDHour != TRAIL.Ttr_NDHour
		                                            OR HIST.Ttr_NDOTHour != TRAIL.Ttr_NDOTHour
													OR HIST.Ttr_LVHour != TRAIL.Ttr_LVHour
		                                            OR HIST.Ttr_ABSHour != TRAIL.Ttr_ABSHour
                                                    OR ISNULL(HIST.Ttr_AssumedPost, '') != ISNULL(TRAIL.Ttr_AssumedPost, '')
		                                            OR HIST.Ttr_AssumedFlag != TRAIL.Ttr_AssumedFlag
		                                            OR HIST.Ttr_PDRESTLEGHOLDay != TRAIL.Ttr_PDRESTLEGHOLDay
                                                    OR HIST.Ttr_Amnesty != TRAIL.Ttr_Amnesty
                                                    OR ISNULL(HIST.Ttr_PDHOLHour, 0) != ISNULL(TRAIL.Ttr_PDHOLHour, 0)
													OR HIST.Ttr_TBAmt01 != TRAIL.Ttr_TBAmt01
													OR HIST.Ttr_TBAmt02 != TRAIL.Ttr_TBAmt02
													OR HIST.Ttr_TBAmt03 != TRAIL.Ttr_TBAmt03
													OR HIST.Ttr_TBAmt04 != TRAIL.Ttr_TBAmt04
													OR HIST.Ttr_TBAmt05 != TRAIL.Ttr_TBAmt05
													OR HIST.Ttr_TBAmt06 != TRAIL.Ttr_TBAmt06
													OR HIST.Ttr_TBAmt07 != TRAIL.Ttr_TBAmt07
													OR HIST.Ttr_TBAmt08 != TRAIL.Ttr_TBAmt08
													OR HIST.Ttr_TBAmt09 != TRAIL.Ttr_TBAmt09
													OR HIST.Ttr_TBAmt10 != TRAIL.Ttr_TBAmt10
													OR HIST.Ttr_TBAmt11 != TRAIL.Ttr_TBAmt11
													OR HIST.Ttr_TBAmt12 != TRAIL.Ttr_TBAmt12)"
                                                    , ProcessPayrollPeriod
                                                    , EmployeeCondition);
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public DataTable GetEmployeePayrollTransactionTrailRecords(bool ProcessAll, string EmployeeId, DALHelper dal)
        {
            string EmployeeCondition = "";
            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition += " AND TRAIL.Ttr_IDNo = '" + EmployeeId + "'";
            else if (ProcessAll && EmployeeList != "")
                EmployeeCondition += " AND TRAIL.Ttr_IDNo IN (" + EmployeeList + ")";

            #region query
            string query = string.Format(@"SELECT A.Tpd_IDNo AS Tpd_IDNo
                                                  ,A.Tpd_AdjPayCycle AS Tpd_AdjPayCycle
                                                  ,A.Tpd_PayCycle AS Tpd_PayCycle
                                                  ,A.Tpd_Date AS Tpd_Date
                                                  ,A.Tpd_PayrollType AS Tpd_PayrollType
                                                  ,A.Tpd_PremiumGrpCode AS Tpd_PremiumGrpCode
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
                                                  ,Tpd_RESTCOMPHOLNDOTHr
                                                  ,Tpd_RESTPSDHr
                                                  ,Tpd_RESTPSDOTHr
                                                  ,Tpd_RESTPSDNDHr
                                                  ,Tpd_RESTPSDNDOTHr
                                                  ,ISNULL(Tpd_LTHr, 0) AS Tpd_LTHr
                                                  ,ISNULL(Tpd_UTHr, 0) AS Tpd_UTHr
                                                  ,ISNULL(Tpd_UPLVHr, 0) AS Tpd_UPLVHr
                                                  ,ISNULL(Tpd_WDABSHr, 0) AS Tpd_WDABSHr
                                                  ,ISNULL(Tpd_LTUTMaxHr, 0) AS Tpd_LTUTMaxHr
                                                  ,ISNULL(Tpd_ABSLEGHOLHr, 0) AS Tpd_ABSLEGHOLHr
                                                  ,ISNULL(Tpd_ABSSPLHOLHr, 0) AS Tpd_ABSSPLHOLHr
                                                  ,ISNULL(Tpd_ABSCOMPHOLHr, 0) AS Tpd_ABSCOMPHOLHr
                                                  ,ISNULL(Tpd_ABSPSDHr, 0) AS Tpd_ABSPSDHr
                                                  ,ISNULL(Tpd_ABSOTHHOLHr, 0) AS Tpd_ABSOTHHOLHr
                                                  ,ISNULL(Tpd_PDLVHr, 0) AS Tpd_PDLVHr
                                                  ,ISNULL(Tpd_PDLEGHOLHr, 0) AS Tpd_PDLEGHOLHr
                                                  ,ISNULL(Tpd_PDSPLHOLHr, 0) AS Tpd_PDSPLHOLHr
                                                  ,ISNULL(Tpd_PDCOMPHOLHr, 0) AS Tpd_PDCOMPHOLHr
                                                  ,ISNULL(Tpd_PDPSDHr, 0) AS Tpd_PDPSDHr
                                                  ,ISNULL(Tpd_PDOTHHOLHr, 0) AS Tpd_PDOTHHOLHr
                                                  ,ISNULL(Tpd_PDRESTLEGHOLHr, 0) AS Tpd_PDRESTLEGHOLHr
                                                  ,Tpd_WorkDay
                                                  ,ISNULL(Tpd_Misc1Hr, 0) AS Tpd_Misc1Hr
                                                  ,ISNULL(Tpd_Misc1OTHr, 0) AS Tpd_Misc1OTHr
                                                  ,ISNULL(Tpd_Misc1NDHr, 0) AS Tpd_Misc1NDHr
                                                  ,ISNULL(Tpd_Misc1NDOTHr, 0) AS Tpd_Misc1NDOTHr
                                                  ,ISNULL(Tpd_Misc2Hr, 0) AS Tpd_Misc2Hr
                                                  ,ISNULL(Tpd_Misc2OTHr, 0) AS Tpd_Misc2OTHr
                                                  ,ISNULL(Tpd_Misc2NDHr, 0) AS Tpd_Misc2NDHr
                                                  ,ISNULL(Tpd_Misc2NDOTHr, 0) AS Tpd_Misc2NDOTHr
                                                  ,ISNULL(Tpd_Misc3Hr, 0) AS Tpd_Misc3Hr
                                                  ,ISNULL(Tpd_Misc3OTHr, 0) AS Tpd_Misc3OTHr
                                                  ,ISNULL(Tpd_Misc3NDHr, 0) AS Tpd_Misc3NDHr
                                                  ,ISNULL(Tpd_Misc3NDOTHr, 0) AS Tpd_Misc3NDOTHr
                                                  ,ISNULL(Tpd_Misc4Hr, 0) AS Tpd_Misc4Hr
                                                  ,ISNULL(Tpd_Misc4OTHr, 0) AS Tpd_Misc4OTHr
                                                  ,ISNULL(Tpd_Misc4NDHr, 0) AS Tpd_Misc4NDHr
                                                  ,ISNULL(Tpd_Misc4NDOTHr, 0) AS Tpd_Misc4NDOTHr
                                                  ,ISNULL(Tpd_Misc5Hr, 0) AS Tpd_Misc5Hr
                                                  ,ISNULL(Tpd_Misc5OTHr, 0) AS Tpd_Misc5OTHr
                                                  ,ISNULL(Tpd_Misc5NDHr, 0) AS Tpd_Misc5NDHr
                                                  ,ISNULL(Tpd_Misc5NDOTHr, 0) AS Tpd_Misc5NDOTHr
                                                  ,ISNULL(Tpd_Misc6Hr, 0) AS Tpd_Misc6Hr
                                                  ,ISNULL(Tpd_Misc6OTHr, 0) AS Tpd_Misc6OTHr
                                                  ,ISNULL(Tpd_Misc6NDHr, 0) AS Tpd_Misc6NDHr
                                                  ,ISNULL(Tpd_Misc6NDOTHr, 0) AS Tpd_Misc6NDOTHr
                                                  ,TRAIL.Ttr_TBAmt01
                                                  ,TRAIL.Ttr_TBAmt02
                                                  ,TRAIL.Ttr_TBAmt03
                                                  ,TRAIL.Ttr_TBAmt04
                                                  ,TRAIL.Ttr_TBAmt05
                                                  ,TRAIL.Ttr_TBAmt06
                                                  ,TRAIL.Ttr_TBAmt07
                                                  ,TRAIL.Ttr_TBAmt08
                                                  ,TRAIL.Ttr_TBAmt09
                                                  ,TRAIL.Ttr_TBAmt10
                                                  ,TRAIL.Ttr_TBAmt11
                                                  ,TRAIL.Ttr_TBAmt12
                                                  ,Mem_LastName
                                                  ,Mem_FirstName 
                                                  ,TRAIL.Ttr_RestDayFlag
                                                  ,TRAIL.Ttr_HolidayFlag
                                                  ,TRAIL.Ttr_EmploymentStatusCode
                                            FROM T_EmpPayTranDtlTrl A
                                            INNER JOIN T_EmpTimeRegisterTrl TRAIL ON A.Tpd_IDNo = Ttr_IDNo
	                                            AND A.Tpd_Date = Ttr_Date
	                                            AND A.Tpd_AdjPayCycle = Ttr_AdjPayCycle
	                                            AND A.Tpd_PayCycle = Ttr_PayCycle
                                                {1}
                                            INNER JOIN T_EmpTimeRegisterHst HIST ON HIST.Ttr_IDNo = TRAIL.Ttr_IDNo
	                                            AND HIST.Ttr_PayCycle = TRAIL.Ttr_PayCycle
	                                            AND HIST.Ttr_Date = TRAIL.Ttr_Date
	                                            AND TRAIL.Ttr_AdjPayCycle = '{0}'
	                                            AND (HIST.Ttr_ActIn_1 != TRAIL.Ttr_ActIn_1
		                                            OR HIST.Ttr_ActOut_1 != TRAIL.Ttr_ActOut_1
		                                            OR HIST.Ttr_ActIn_2 != TRAIL.Ttr_ActIn_2
		                                            OR HIST.Ttr_ActOut_2 != TRAIL.Ttr_ActOut_2
		                                            OR HIST.Ttr_ShiftCode != TRAIL.Ttr_ShiftCode
		                                            OR HIST.Ttr_DayCode != TRAIL.Ttr_DayCode
		                                            OR HIST.Ttr_RestDayFlag != TRAIL.Ttr_RestDayFlag
		                                            OR HIST.Ttr_HolidayFlag != TRAIL.Ttr_HolidayFlag
		                                            OR HIST.Ttr_WFPayLVCode != TRAIL.Ttr_WFPayLVCode
		                                            OR HIST.Ttr_WFPayLVHr != TRAIL.Ttr_WFPayLVHr
		                                            OR HIST.Ttr_WFNoPayLVCode != TRAIL.Ttr_WFNoPayLVCode
		                                            OR HIST.Ttr_WFNoPayLVHr != TRAIL.Ttr_WFNoPayLVHr
		                                            OR HIST.Ttr_WFOTAdvHr != TRAIL.Ttr_WFOTAdvHr
		                                            OR HIST.Ttr_WFOTPostHr != TRAIL.Ttr_WFOTPostHr
		                                            OR HIST.Ttr_OTMin != TRAIL.Ttr_OTMin
		                                            OR HIST.Ttr_REGHour != TRAIL.Ttr_REGHour
		                                            OR HIST.Ttr_OTHour != TRAIL.Ttr_OTHour
													OR HIST.Ttr_NDHour != TRAIL.Ttr_NDHour
		                                            OR HIST.Ttr_NDOTHour != TRAIL.Ttr_NDOTHour
													OR HIST.Ttr_LVHour != TRAIL.Ttr_LVHour
		                                            OR HIST.Ttr_ABSHour != TRAIL.Ttr_ABSHour
		                                            OR ISNULL(HIST.Ttr_AssumedPost, '') != ISNULL(TRAIL.Ttr_AssumedPost, '')
		                                            OR HIST.Ttr_AssumedFlag != TRAIL.Ttr_AssumedFlag
		                                            OR HIST.Ttr_PDRESTLEGHOLDay != TRAIL.Ttr_PDRESTLEGHOLDay
                                                    OR HIST.Ttr_Amnesty != TRAIL.Ttr_Amnesty
                                                    OR ISNULL(HIST.Ttr_PDHOLHour, 0) != ISNULL(TRAIL.Ttr_PDHOLHour, 0)
													OR HIST.Ttr_TBAmt01 != TRAIL.Ttr_TBAmt01
													OR HIST.Ttr_TBAmt02 != TRAIL.Ttr_TBAmt02
													OR HIST.Ttr_TBAmt03 != TRAIL.Ttr_TBAmt03
													OR HIST.Ttr_TBAmt04 != TRAIL.Ttr_TBAmt04
													OR HIST.Ttr_TBAmt05 != TRAIL.Ttr_TBAmt05
													OR HIST.Ttr_TBAmt06 != TRAIL.Ttr_TBAmt06
													OR HIST.Ttr_TBAmt07 != TRAIL.Ttr_TBAmt07
													OR HIST.Ttr_TBAmt08 != TRAIL.Ttr_TBAmt08
													OR HIST.Ttr_TBAmt09 != TRAIL.Ttr_TBAmt09
													OR HIST.Ttr_TBAmt10 != TRAIL.Ttr_TBAmt10
													OR HIST.Ttr_TBAmt11 != TRAIL.Ttr_TBAmt11
													OR HIST.Ttr_TBAmt12 != TRAIL.Ttr_TBAmt12)
                                            LEFT JOIN T_EmpPayTranDtlMiscTrl B ON A.Tpd_IDNo = B.Tpd_IDNo
	                                            AND A.Tpd_Date = B.Tpd_Date
	                                            AND A.Tpd_PayCycle = B.Tpd_PayCycle
	                                            AND A.Tpd_AdjPayCycle = B.Tpd_AdjPayCycle
                                            LEFT JOIN T_EmpPayrollYearly CalcAnnual ON CalcAnnual.Tpy_IDNo = A.Tpd_IDNo
												AND CalcAnnual.Tpy_PayCycle = A.Tpd_PayCycle
											LEFT JOIN T_EmpPayrollHst CalcHist ON CalcHist.Tpy_IDNo = A.Tpd_IDNo
												AND CalcHist.Tpy_PayCycle = A.Tpd_PayCycle
                                            INNER JOIN M_Employee ON A.Tpd_IDNo = Mem_IDNo
                                            WHERE A.Tpd_AdjPayCycle = '{0}'
                                                AND (CalcHist.Tpy_IDNo IS NOT NULL
													OR CalcAnnual.Tpy_IDNo IS NOT NULL)
                                            ORDER BY Mem_LastName, Mem_FirstName, A.Tpd_AdjPayCycle, A.Tpd_PayCycle, A.Tpd_Date"
                                            , ProcessPayrollPeriod
                                            , EmployeeCondition);
            #endregion
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public DataTable GetEmployeePayrollTransactionTrailRecordsMultiPockets(bool ProcessAll, string EmployeeId, DALHelper dal)
        {
            string EmployeeCondition = "";
            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition += " AND TRAIL.Ttr_IDNo = '" + EmployeeId + "'";
            else if (ProcessAll && EmployeeList != "")
                EmployeeCondition += " AND TRAIL.Ttr_IDNo IN (" + EmployeeList + ")";

            #region query
            string query = string.Format(@"SELECT A.Tpd_IDNo AS Tpd_IDNo
                                                  ,A.Tpd_AdjPayCycle AS Tpd_AdjPayCycle
                                                  ,A.Tpd_PayCycle AS Tpd_PayCycle
                                                  ,A.Tpd_Date AS Tpd_Date
                                                  ,A.Tpd_PayrollType AS Tpd_PayrollType
                                                  ,A.Tpd_PremiumGrpCode AS Tpd_PremiumGrpCode
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
                                                  ,Tpd_RESTCOMPHOLNDOTHr
                                                  ,Tpd_RESTPSDHr
                                                  ,Tpd_RESTPSDOTHr
                                                  ,Tpd_RESTPSDNDHr
                                                  ,Tpd_RESTPSDNDOTHr
                                                  ,ISNULL(Tpd_LTHr, 0) AS Tpd_LTHr
                                                  ,ISNULL(Tpd_UTHr, 0) AS Tpd_UTHr
                                                  ,ISNULL(Tpd_UPLVHr, 0) AS Tpd_UPLVHr
                                                  ,ISNULL(Tpd_WDABSHr, 0) AS Tpd_WDABSHr
                                                  ,ISNULL(Tpd_LTUTMaxHr, 0) AS Tpd_LTUTMaxHr
                                                  ,ISNULL(Tpd_ABSLEGHOLHr, 0) AS Tpd_ABSLEGHOLHr
                                                  ,ISNULL(Tpd_ABSSPLHOLHr, 0) AS Tpd_ABSSPLHOLHr
                                                  ,ISNULL(Tpd_ABSCOMPHOLHr, 0) AS Tpd_ABSCOMPHOLHr
                                                  ,ISNULL(Tpd_ABSPSDHr, 0) AS Tpd_ABSPSDHr
                                                  ,ISNULL(Tpd_ABSOTHHOLHr, 0) AS Tpd_ABSOTHHOLHr
                                                  ,ISNULL(Tpd_PDLVHr, 0) AS Tpd_PDLVHr
                                                  ,ISNULL(Tpd_PDLEGHOLHr, 0) AS Tpd_PDLEGHOLHr
                                                  ,ISNULL(Tpd_PDSPLHOLHr, 0) AS Tpd_PDSPLHOLHr
                                                  ,ISNULL(Tpd_PDCOMPHOLHr, 0) AS Tpd_PDCOMPHOLHr
                                                  ,ISNULL(Tpd_PDPSDHr, 0) AS Tpd_PDPSDHr
                                                  ,ISNULL(Tpd_PDOTHHOLHr, 0) AS Tpd_PDOTHHOLHr
                                                  ,ISNULL(Tpd_PDRESTLEGHOLHr, 0) AS Tpd_PDRESTLEGHOLHr
                                                  ,Tpd_WorkDay
                                                  ,ISNULL(Tpd_Misc1Hr, 0) AS Tpd_Misc1Hr
                                                  ,ISNULL(Tpd_Misc1OTHr, 0) AS Tpd_Misc1OTHr
                                                  ,ISNULL(Tpd_Misc1NDHr, 0) AS Tpd_Misc1NDHr
                                                  ,ISNULL(Tpd_Misc1NDOTHr, 0) AS Tpd_Misc1NDOTHr
                                                  ,ISNULL(Tpd_Misc2Hr, 0) AS Tpd_Misc2Hr
                                                  ,ISNULL(Tpd_Misc2OTHr, 0) AS Tpd_Misc2OTHr
                                                  ,ISNULL(Tpd_Misc2NDHr, 0) AS Tpd_Misc2NDHr
                                                  ,ISNULL(Tpd_Misc2NDOTHr, 0) AS Tpd_Misc2NDOTHr
                                                  ,ISNULL(Tpd_Misc3Hr, 0) AS Tpd_Misc3Hr
                                                  ,ISNULL(Tpd_Misc3OTHr, 0) AS Tpd_Misc3OTHr
                                                  ,ISNULL(Tpd_Misc3NDHr, 0) AS Tpd_Misc3NDHr
                                                  ,ISNULL(Tpd_Misc3NDOTHr, 0) AS Tpd_Misc3NDOTHr
                                                  ,ISNULL(Tpd_Misc4Hr, 0) AS Tpd_Misc4Hr
                                                  ,ISNULL(Tpd_Misc4OTHr, 0) AS Tpd_Misc4OTHr
                                                  ,ISNULL(Tpd_Misc4NDHr, 0) AS Tpd_Misc4NDHr
                                                  ,ISNULL(Tpd_Misc4NDOTHr, 0) AS Tpd_Misc4NDOTHr
                                                  ,ISNULL(Tpd_Misc5Hr, 0) AS Tpd_Misc5Hr
                                                  ,ISNULL(Tpd_Misc5OTHr, 0) AS Tpd_Misc5OTHr
                                                  ,ISNULL(Tpd_Misc5NDHr, 0) AS Tpd_Misc5NDHr
                                                  ,ISNULL(Tpd_Misc5NDOTHr, 0) AS Tpd_Misc5NDOTHr
                                                  ,ISNULL(Tpd_Misc6Hr, 0) AS Tpd_Misc6Hr
                                                  ,ISNULL(Tpd_Misc6OTHr, 0) AS Tpd_Misc6OTHr
                                                  ,ISNULL(Tpd_Misc6NDHr, 0) AS Tpd_Misc6NDHr
                                                  ,ISNULL(Tpd_Misc6NDOTHr, 0) AS Tpd_Misc6NDOTHr
                                                  ,TRAIL.Ttr_TBAmt01
                                                  ,TRAIL.Ttr_TBAmt02
                                                  ,TRAIL.Ttr_TBAmt03
                                                  ,TRAIL.Ttr_TBAmt04
                                                  ,TRAIL.Ttr_TBAmt05
                                                  ,TRAIL.Ttr_TBAmt06
                                                  ,TRAIL.Ttr_TBAmt07
                                                  ,TRAIL.Ttr_TBAmt08
                                                  ,TRAIL.Ttr_TBAmt09
                                                  ,TRAIL.Ttr_TBAmt10
                                                  ,TRAIL.Ttr_TBAmt11
                                                  ,TRAIL.Ttr_TBAmt12
                                                  ,Mem_LastName
                                                  ,Mem_FirstName 
                                                  ,TRAIL.Ttr_RestDayFlag
                                                  ,TRAIL.Ttr_HolidayFlag
                                                  ,TRAIL.Ttr_EmploymentStatusCode
                                                  ,SHIFT.Msh_ShiftHours
                                            FROM T_EmpPayTranDtlTrl A
                                            INNER JOIN T_EmpTimeRegisterTrl TRAIL ON A.Tpd_IDNo = TRAIL.Ttr_IDNo
	                                            AND A.Tpd_Date              = TRAIL.Ttr_Date
	                                            AND A.Tpd_AdjPayCycle       = TRAIL.Ttr_AdjPayCycle
	                                            AND A.Tpd_PayCycle          = TRAIL.Ttr_PayCycle
                                            INNER JOIN T_EmpTimeRegisterHst HIST ON HIST.Ttr_IDNo = TRAIL.Ttr_IDNo
                                                AND HIST.Ttr_Date           = TRAIL.Ttr_Date
	                                            AND HIST.Ttr_PayCycle       = TRAIL.Ttr_PayCycle
	                                        INNER JOIN T_EmpTimeRegisterMiscTrl MISCTRL ON MISCTRL.Ttm_IDNo = TRAIL.Ttr_IDNo
	                                            AND MISCTRL.Ttm_Date        = TRAIL.Ttr_Date
	                                            AND MISCTRL.Ttm_AdjPayCycle = TRAIL.Ttr_AdjPayCycle
                                            INNER JOIN T_EmpTimeRegisterMiscHst MISCHST ON MISCHST.Ttm_IDNo = HIST.Ttr_IDNo 
	                                            AND MISCHST.Ttm_Date        = HIST.Ttr_Date   
                                            LEFT JOIN T_EmpPayTranDtlMiscTrl B ON A.Tpd_IDNo = B.Tpd_IDNo
	                                            AND A.Tpd_Date              = B.Tpd_Date
	                                            AND A.Tpd_PayCycle          = B.Tpd_PayCycle
	                                            AND A.Tpd_AdjPayCycle       = B.Tpd_AdjPayCycle
                                            LEFT JOIN T_EmpPayrollYearly CalcAnnual ON CalcAnnual.Tpy_IDNo = A.Tpd_IDNo
												AND CalcAnnual.Tpy_PayCycle = A.Tpd_PayCycle
											LEFT JOIN T_EmpPayrollHst CalcHist ON CalcHist.Tpy_IDNo = A.Tpd_IDNo
												AND CalcHist.Tpy_PayCycle   = A.Tpd_PayCycle
                                            INNER JOIN M_Employee ON A.Tpd_IDNo = Mem_IDNo
                                            LEFT JOIN {2}..M_Shift SHIFT ON SHIFT.Msh_ShiftCode = TRAIL.Ttr_ShiftCode
									            AND SHIFT.Msh_CompanyCode = '{3}'
                                            WHERE A.Tpd_AdjPayCycle = '{0}'
                                                {1}
                                                AND (MISCHST.Ttm_ActIn_01 != MISCTRL.Ttm_ActIn_01
		                                            OR MISCHST.Ttm_ActOut_01 != MISCTRL.Ttm_ActOut_01
		                                            OR MISCHST.Ttm_ActIn_02 != MISCTRL.Ttm_ActIn_02
		                                            OR MISCHST.Ttm_ActOut_02 != MISCTRL.Ttm_ActOut_02
		                                            OR MISCHST.Ttm_ActIn_03 != MISCTRL.Ttm_ActIn_03
		                                            OR MISCHST.Ttm_ActOut_03 != MISCTRL.Ttm_ActOut_03
		                                            OR MISCHST.Ttm_ActIn_04 != MISCTRL.Ttm_ActIn_04
		                                            OR MISCHST.Ttm_ActOut_04 != MISCTRL.Ttm_ActOut_04
		                                            OR MISCHST.Ttm_ActIn_05 != MISCTRL.Ttm_ActIn_05
		                                            OR MISCHST.Ttm_ActOut_05 != MISCTRL.Ttm_ActOut_05
		                                            OR MISCHST.Ttm_ActIn_06 != MISCTRL.Ttm_ActIn_06
		                                            OR MISCHST.Ttm_ActOut_06 != MISCTRL.Ttm_ActOut_06
		                                            OR MISCHST.Ttm_ActIn_07 != MISCTRL.Ttm_ActIn_07
		                                            OR MISCHST.Ttm_ActOut_07 != MISCTRL.Ttm_ActOut_07
		                                            OR MISCHST.Ttm_ActIn_08 != MISCTRL.Ttm_ActIn_08
		                                            OR MISCHST.Ttm_ActOut_08 != MISCTRL.Ttm_ActOut_08
		                                            OR MISCHST.Ttm_ActIn_09 != MISCTRL.Ttm_ActIn_09
		                                            OR MISCHST.Ttm_ActOut_09 != MISCTRL.Ttm_ActOut_09
		                                            OR MISCHST.Ttm_ActIn_10 != MISCTRL.Ttm_ActIn_10
		                                            OR MISCHST.Ttm_ActOut_10 != MISCTRL.Ttm_ActOut_10
		                                            OR MISCHST.Ttm_ActIn_11 != MISCTRL.Ttm_ActIn_11
		                                            OR MISCHST.Ttm_ActOut_11 != MISCTRL.Ttm_ActOut_11
		                                            OR MISCHST.Ttm_ActIn_12 != MISCTRL.Ttm_ActIn_12
		                                            OR MISCHST.Ttm_ActOut_12 != MISCTRL.Ttm_ActOut_12
		                                            OR HIST.Ttr_ShiftCode != TRAIL.Ttr_ShiftCode
		                                            OR HIST.Ttr_DayCode != TRAIL.Ttr_DayCode
		                                            OR HIST.Ttr_RestDayFlag != TRAIL.Ttr_RestDayFlag
		                                            OR HIST.Ttr_HolidayFlag != TRAIL.Ttr_HolidayFlag
		                                            OR HIST.Ttr_WFPayLVCode != TRAIL.Ttr_WFPayLVCode
		                                            OR HIST.Ttr_WFPayLVHr != TRAIL.Ttr_WFPayLVHr
		                                            OR HIST.Ttr_WFNoPayLVCode != TRAIL.Ttr_WFNoPayLVCode
		                                            OR HIST.Ttr_WFNoPayLVHr != TRAIL.Ttr_WFNoPayLVHr
		                                            OR HIST.Ttr_WFOTAdvHr != TRAIL.Ttr_WFOTAdvHr
		                                            OR HIST.Ttr_WFOTPostHr != TRAIL.Ttr_WFOTPostHr
		                                            OR HIST.Ttr_OTMin != TRAIL.Ttr_OTMin
		                                            OR HIST.Ttr_REGHour != TRAIL.Ttr_REGHour
		                                            OR HIST.Ttr_OTHour != TRAIL.Ttr_OTHour
													OR HIST.Ttr_NDHour != TRAIL.Ttr_NDHour
		                                            OR HIST.Ttr_NDOTHour != TRAIL.Ttr_NDOTHour
													OR HIST.Ttr_LVHour != TRAIL.Ttr_LVHour
		                                            OR HIST.Ttr_ABSHour != TRAIL.Ttr_ABSHour
		                                            OR ISNULL(HIST.Ttr_AssumedPost, '') != ISNULL(TRAIL.Ttr_AssumedPost, '')
		                                            OR HIST.Ttr_AssumedFlag != TRAIL.Ttr_AssumedFlag
		                                            OR HIST.Ttr_PDRESTLEGHOLDay != TRAIL.Ttr_PDRESTLEGHOLDay
                                                    OR HIST.Ttr_Amnesty != TRAIL.Ttr_Amnesty
                                                    OR ISNULL(HIST.Ttr_PDHOLHour, 0) != ISNULL(TRAIL.Ttr_PDHOLHour, 0)
													OR HIST.Ttr_TBAmt01 != TRAIL.Ttr_TBAmt01
													OR HIST.Ttr_TBAmt02 != TRAIL.Ttr_TBAmt02
													OR HIST.Ttr_TBAmt03 != TRAIL.Ttr_TBAmt03
													OR HIST.Ttr_TBAmt04 != TRAIL.Ttr_TBAmt04
													OR HIST.Ttr_TBAmt05 != TRAIL.Ttr_TBAmt05
													OR HIST.Ttr_TBAmt06 != TRAIL.Ttr_TBAmt06
													OR HIST.Ttr_TBAmt07 != TRAIL.Ttr_TBAmt07
													OR HIST.Ttr_TBAmt08 != TRAIL.Ttr_TBAmt08
													OR HIST.Ttr_TBAmt09 != TRAIL.Ttr_TBAmt09
													OR HIST.Ttr_TBAmt10 != TRAIL.Ttr_TBAmt10
													OR HIST.Ttr_TBAmt11 != TRAIL.Ttr_TBAmt11
													OR HIST.Ttr_TBAmt12 != TRAIL.Ttr_TBAmt12)
                                                AND (CalcHist.Tpy_IDNo IS NOT NULL
													OR CalcAnnual.Tpy_IDNo IS NOT NULL)
                                            ORDER BY Mem_LastName, Mem_FirstName, A.Tpd_AdjPayCycle, A.Tpd_PayCycle, A.Tpd_Date"
                                            , ProcessPayrollPeriod
                                            , EmployeeCondition
                                            , CentralProfile
                                            , CompanyCode);
            #endregion
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public DataTable GetEmployeePayrollTransactionHistRecordsMultiPockets(bool ProcessAll, string EmployeeId, DALHelper dal)
        {
            string EmployeeCondition = "";
            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition += " AND TRAIL.Ttr_IDNo = '" + EmployeeId + "'";
            if (ProcessAll && EmployeeList != "")
                EmployeeCondition += " AND TRAIL.Ttr_IDNo IN (" + EmployeeList + ")";

            #region query
            string query = string.Format(@"SELECT  A.Tpd_IDNo AS Tpd_IDNo
		                                            ,A.Tpd_PayCycle AS Tpd_PayCycle
		                                            ,A.Tpd_Date AS Tpd_Date
                                                    ,ISNULL(ISNULL(CalcDtlAnnual.Tpd_SalaryRate, CalcDtlHist.Tpd_SalaryRate),0) AS Tpd_SalaryRate
		                                            ,A.Tpd_PayrollType AS Tpd_PayrollType
                                                    ,A.Tpd_PremiumGrpCode AS Tpd_PremiumGrpCode
                                                    ,ISNULL(A.Tpd_LTHr, 0) AS Tpd_LTHr
                                                    ,ISNULL(A.Tpd_UTHr, 0) AS Tpd_UTHr
                                                    ,ISNULL(A.Tpd_UPLVHr, 0) AS Tpd_UPLVHr
                                                    ,ISNULL(A.Tpd_ABSLEGHOLHr, 0) AS Tpd_ABSLEGHOLHr
                                                    ,ISNULL(A.Tpd_ABSSPLHOLHr, 0) AS Tpd_ABSSPLHOLHr
                                                    ,ISNULL(A.Tpd_ABSCOMPHOLHr, 0) AS Tpd_ABSCOMPHOLHr
                                                    ,ISNULL(A.Tpd_ABSPSDHr, 0) AS Tpd_ABSPSDHr
                                                    ,ISNULL(A.Tpd_ABSOTHHOLHr, 0) AS Tpd_ABSOTHHOLHr
                                                    ,ISNULL(A.Tpd_WDABSHr, 0) AS Tpd_WDABSHr
                                                    ,ISNULL(A.Tpd_LTUTMaxHr, 0) AS Tpd_LTUTMaxHr
		                                            ,A.Tpd_ABSHr
		                                            ,A.Tpd_REGHr
                                                    ,ISNULL(A.Tpd_PDLVHr, 0) AS Tpd_PDLVHr
                                                    ,ISNULL(A.Tpd_PDLEGHOLHr, 0) AS Tpd_PDLEGHOLHr
                                                    ,ISNULL(A.Tpd_PDSPLHOLHr, 0) AS Tpd_PDSPLHOLHr
                                                    ,ISNULL(A.Tpd_PDCOMPHOLHr, 0) AS Tpd_PDCOMPHOLHr
                                                    ,ISNULL(A.Tpd_PDPSDHr, 0) AS Tpd_PDPSDHr
                                                    ,ISNULL(A.Tpd_PDOTHHOLHr, 0) AS Tpd_PDOTHHOLHr
		                                            ,ISNULL(A.Tpd_PDRESTLEGHOLHr,0) AS Tpd_PDRESTLEGHOLHr
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
                                                    ,ISNULL(Tpd_Misc1Hr, 0) AS Tpd_Misc1Hr
                                                    ,ISNULL(Tpd_Misc1OTHr, 0) AS Tpd_Misc1OTHr
                                                    ,ISNULL(Tpd_Misc1NDHr, 0) AS Tpd_Misc1NDHr
                                                    ,ISNULL(Tpd_Misc1NDOTHr, 0) AS Tpd_Misc1NDOTHr
                                                    ,ISNULL(Tpd_Misc2Hr, 0) AS Tpd_Misc2Hr
                                                    ,ISNULL(Tpd_Misc2OTHr, 0) AS Tpd_Misc2OTHr
                                                    ,ISNULL(Tpd_Misc2NDHr, 0) AS Tpd_Misc2NDHr
                                                    ,ISNULL(Tpd_Misc2NDOTHr, 0) AS Tpd_Misc2NDOTHr
                                                    ,ISNULL(Tpd_Misc3Hr, 0) AS Tpd_Misc3Hr
                                                    ,ISNULL(Tpd_Misc3OTHr, 0) AS Tpd_Misc3OTHr
                                                    ,ISNULL(Tpd_Misc3NDHr, 0) AS Tpd_Misc3NDHr
                                                    ,ISNULL(Tpd_Misc3NDOTHr, 0) AS Tpd_Misc3NDOTHr
                                                    ,ISNULL(Tpd_Misc4Hr, 0) AS Tpd_Misc4Hr
                                                    ,ISNULL(Tpd_Misc4OTHr, 0) AS Tpd_Misc4OTHr
                                                    ,ISNULL(Tpd_Misc4NDHr, 0) AS Tpd_Misc4NDHr
                                                    ,ISNULL(Tpd_Misc4NDOTHr, 0) AS Tpd_Misc4NDOTHr
                                                    ,ISNULL(Tpd_Misc5Hr, 0) AS Tpd_Misc5Hr
                                                    ,ISNULL(Tpd_Misc5OTHr, 0) AS Tpd_Misc5OTHr
                                                    ,ISNULL(Tpd_Misc5NDHr, 0) AS Tpd_Misc5NDHr
                                                    ,ISNULL(Tpd_Misc5NDOTHr, 0) AS Tpd_Misc5NDOTHr
                                                    ,ISNULL(Tpd_Misc6Hr, 0) AS Tpd_Misc6Hr
                                                    ,ISNULL(Tpd_Misc6OTHr, 0) AS Tpd_Misc6OTHr
                                                    ,ISNULL(Tpd_Misc6NDHr, 0) AS Tpd_Misc6NDHr
                                                    ,ISNULL(Tpd_Misc6NDOTHr, 0) AS Tpd_Misc6NDOTHr
		                                            ,HIST.Ttr_TBAmt01
		                                            ,HIST.Ttr_TBAmt02
		                                            ,HIST.Ttr_TBAmt03
		                                            ,HIST.Ttr_TBAmt04
		                                            ,HIST.Ttr_TBAmt05
		                                            ,HIST.Ttr_TBAmt06
		                                            ,HIST.Ttr_TBAmt07
		                                            ,HIST.Ttr_TBAmt08
		                                            ,HIST.Ttr_TBAmt09
		                                            ,HIST.Ttr_TBAmt10
		                                            ,HIST.Ttr_TBAmt11
		                                            ,HIST.Ttr_TBAmt12
                                                    ,HIST.Ttr_RestDayFlag
                                                    ,HIST.Ttr_HolidayFlag
                                                    ,HIST.Ttr_EmploymentStatusCode
                                                    ,SHIFT.Msh_ShiftHours
                                            FROM T_EmpPayTranDtlHst A
                                            INNER JOIN T_EmpTimeRegisterTrl TRAIL ON A.Tpd_IDNo = TRAIL.Ttr_IDNo
	                                            AND A.Tpd_Date              = TRAIL.Ttr_Date
	                                            AND A.Tpd_PayCycle          = TRAIL.Ttr_PayCycle
                                            INNER JOIN T_EmpTimeRegisterHst HIST ON HIST.Ttr_IDNo = TRAIL.Ttr_IDNo
                                                AND HIST.Ttr_Date           = TRAIL.Ttr_Date
	                                            AND HIST.Ttr_PayCycle       = TRAIL.Ttr_PayCycle
                                            INNER JOIN T_EmpTimeRegisterMiscTrl MISCTRL ON MISCTRL.Ttm_IDNo = TRAIL.Ttr_IDNo
	                                            AND MISCTRL.Ttm_Date = TRAIL.Ttr_Date
	                                            AND MISCTRL.Ttm_AdjPayCycle = TRAIL.Ttr_AdjPayCycle
                                            INNER JOIN T_EmpTimeRegisterMiscHst MISCHST ON MISCHST.Ttm_IDNo = HIST.Ttr_IDNo 
	                                            AND MISCHST.Ttm_Date        = HIST.Ttr_Date   
                                            LEFT JOIN T_EmpPayTranDtlMiscHst B  ON A.Tpd_IDNo = B.Tpd_IDNo
	                                            AND A.Tpd_Date              = B.Tpd_Date
                                            LEFT JOIN T_EmpPayrollYearly CalcAnnual ON CalcAnnual.Tpy_IDNo = A.Tpd_IDNo
												AND CalcAnnual.Tpy_PayCycle = A.Tpd_PayCycle
											LEFT JOIN T_EmpPayrollHst CalcHist ON CalcHist.Tpy_IDNo = A.Tpd_IDNo
												AND CalcHist.Tpy_PayCycle   = A.Tpd_PayCycle
                                            LEFT JOIN T_EmpPayrollDtlYearly CalcDtlAnnual ON  CalcDtlAnnual.Tpd_IDNo = A.Tpd_IDNo
												AND CalcDtlAnnual.Tpd_PayCycle = A.Tpd_PayCycle
												AND CalcDtlAnnual.Tpd_Date  = A.Tpd_Date
											LEFT JOIN T_EmpPayrollDtlHst CalcDtlHist ON  CalcDtlHist.Tpd_IDNo = A.Tpd_IDNo
												AND CalcDtlHist.Tpd_PayCycle = A.Tpd_PayCycle
												AND CalcDtlHist.Tpd_Date    = A.Tpd_Date
                                            INNER JOIN M_Employee ON A.Tpd_IDNo = Mem_IDNo
                                            LEFT JOIN {2}..M_Shift SHIFT ON SHIFT.Msh_ShiftCode = HIST.Ttr_ShiftCode
									            AND SHIFT.Msh_CompanyCode = '{3}'
                                            WHERE TRAIL.Ttr_AdjPayCycle = '{0}'
                                                {1}
	                                            AND (MISCHST.Ttm_ActIn_01 != MISCTRL.Ttm_ActIn_01
		                                            OR MISCHST.Ttm_ActOut_01 != MISCTRL.Ttm_ActOut_01
		                                            OR MISCHST.Ttm_ActIn_02 != MISCTRL.Ttm_ActIn_02
		                                            OR MISCHST.Ttm_ActOut_02 != MISCTRL.Ttm_ActOut_02
		                                            OR MISCHST.Ttm_ActIn_03 != MISCTRL.Ttm_ActIn_03
		                                            OR MISCHST.Ttm_ActOut_03 != MISCTRL.Ttm_ActOut_03
		                                            OR MISCHST.Ttm_ActIn_04 != MISCTRL.Ttm_ActIn_04
		                                            OR MISCHST.Ttm_ActOut_04 != MISCTRL.Ttm_ActOut_04
		                                            OR MISCHST.Ttm_ActIn_05 != MISCTRL.Ttm_ActIn_05
		                                            OR MISCHST.Ttm_ActOut_05 != MISCTRL.Ttm_ActOut_05
		                                            OR MISCHST.Ttm_ActIn_06 != MISCTRL.Ttm_ActIn_06
		                                            OR MISCHST.Ttm_ActOut_06 != MISCTRL.Ttm_ActOut_06
		                                            OR MISCHST.Ttm_ActIn_07 != MISCTRL.Ttm_ActIn_07
		                                            OR MISCHST.Ttm_ActOut_07 != MISCTRL.Ttm_ActOut_07
		                                            OR MISCHST.Ttm_ActIn_08 != MISCTRL.Ttm_ActIn_08
		                                            OR MISCHST.Ttm_ActOut_08 != MISCTRL.Ttm_ActOut_08
		                                            OR MISCHST.Ttm_ActIn_09 != MISCTRL.Ttm_ActIn_09
		                                            OR MISCHST.Ttm_ActOut_09 != MISCTRL.Ttm_ActOut_09
		                                            OR MISCHST.Ttm_ActIn_10 != MISCTRL.Ttm_ActIn_10
		                                            OR MISCHST.Ttm_ActOut_10 != MISCTRL.Ttm_ActOut_10
		                                            OR MISCHST.Ttm_ActIn_11 != MISCTRL.Ttm_ActIn_11
		                                            OR MISCHST.Ttm_ActOut_11 != MISCTRL.Ttm_ActOut_11
		                                            OR MISCHST.Ttm_ActIn_12 != MISCTRL.Ttm_ActIn_12
		                                            OR MISCHST.Ttm_ActOut_12 != MISCTRL.Ttm_ActOut_12
		                                            OR HIST.Ttr_ShiftCode != TRAIL.Ttr_ShiftCode
		                                            OR HIST.Ttr_DayCode != TRAIL.Ttr_DayCode
		                                            OR HIST.Ttr_RestDayFlag != TRAIL.Ttr_RestDayFlag
		                                            OR HIST.Ttr_HolidayFlag != TRAIL.Ttr_HolidayFlag
		                                            OR HIST.Ttr_WFPayLVCode != TRAIL.Ttr_WFPayLVCode
		                                            OR HIST.Ttr_WFPayLVHr != TRAIL.Ttr_WFPayLVHr
		                                            OR HIST.Ttr_WFNoPayLVCode != TRAIL.Ttr_WFNoPayLVCode
		                                            OR HIST.Ttr_WFNoPayLVHr != TRAIL.Ttr_WFNoPayLVHr
		                                            OR HIST.Ttr_WFOTAdvHr != TRAIL.Ttr_WFOTAdvHr
		                                            OR HIST.Ttr_WFOTPostHr != TRAIL.Ttr_WFOTPostHr
		                                            OR HIST.Ttr_OTMin != TRAIL.Ttr_OTMin
		                                            OR HIST.Ttr_REGHour != TRAIL.Ttr_REGHour
		                                            OR HIST.Ttr_OTHour != TRAIL.Ttr_OTHour
													OR HIST.Ttr_NDHour != TRAIL.Ttr_NDHour
		                                            OR HIST.Ttr_NDOTHour != TRAIL.Ttr_NDOTHour
													OR HIST.Ttr_LVHour != TRAIL.Ttr_LVHour
		                                            OR HIST.Ttr_ABSHour != TRAIL.Ttr_ABSHour
		                                            OR ISNULL(HIST.Ttr_AssumedPost, '') != ISNULL(TRAIL.Ttr_AssumedPost, '')
		                                            OR HIST.Ttr_AssumedFlag != TRAIL.Ttr_AssumedFlag
		                                            OR HIST.Ttr_PDRESTLEGHOLDay != TRAIL.Ttr_PDRESTLEGHOLDay
                                                    OR HIST.Ttr_Amnesty != TRAIL.Ttr_Amnesty
                                                    OR ISNULL(HIST.Ttr_PDHOLHour, 0) != ISNULL(TRAIL.Ttr_PDHOLHour, 0)
													OR HIST.Ttr_TBAmt01 != TRAIL.Ttr_TBAmt01
													OR HIST.Ttr_TBAmt02 != TRAIL.Ttr_TBAmt02
													OR HIST.Ttr_TBAmt03 != TRAIL.Ttr_TBAmt03
													OR HIST.Ttr_TBAmt04 != TRAIL.Ttr_TBAmt04
													OR HIST.Ttr_TBAmt05 != TRAIL.Ttr_TBAmt05
													OR HIST.Ttr_TBAmt06 != TRAIL.Ttr_TBAmt06
													OR HIST.Ttr_TBAmt07 != TRAIL.Ttr_TBAmt07
													OR HIST.Ttr_TBAmt08 != TRAIL.Ttr_TBAmt08
													OR HIST.Ttr_TBAmt09 != TRAIL.Ttr_TBAmt09
													OR HIST.Ttr_TBAmt10 != TRAIL.Ttr_TBAmt10
													OR HIST.Ttr_TBAmt11 != TRAIL.Ttr_TBAmt11
													OR HIST.Ttr_TBAmt12 != TRAIL.Ttr_TBAmt12)
                                                AND (CalcHist.Tpy_IDNo IS NOT NULL
													OR CalcAnnual.Tpy_IDNo IS NOT NULL)"
                                        , ProcessPayrollPeriod
                                        , EmployeeCondition
                                        , CentralProfile
                                        , CompanyCode);
            #endregion
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public void InsertEmployeeDailyAllowance(string EmployeeId, string AllowanceCode, decimal AllowanceAmt, string Cycle, string UserLogin)
        {
            string query;

            //Check if exist
            query = string.Format(@"SELECT * FROM T_EmpTimeBaseAllowanceCycle
                                    WHERE Tta_IDNo = '{0}'
                                      AND Tta_PayCycle = '{1}'
                                      AND Tta_IncomeCode = '{2}'", EmployeeId, ProcessPayrollPeriod, AllowanceCode);
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];

            if (dtResult.Rows.Count > 0)
            {
                #region update query
                query = string.Format(@"UPDATE [T_EmpTimeBaseAllowanceCycle]
                                        SET [Tta_IncomeAmt] = [Tta_IncomeAmt] + {3}
                                            ,[Usr_Login] = '{4}'
                                            ,[Ludatetime] = GetDate()
                                        WHERE [Tta_IDNo] = '{0}'
                                          AND [Tta_PayCycle] = '{1}'
                                          AND [Tta_IncomeCode] = '{2}'", EmployeeId, ProcessPayrollPeriod, AllowanceCode, AllowanceAmt, UserLogin);
                #endregion
            }
            else
            {
                #region insert query
                query = string.Format(@"INSERT INTO [T_EmpTimeBaseAllowanceCycle]
                                                   ([Tta_IDNo]
                                                   ,[Tta_PayCycle]
                                                   ,[Tta_IncomeCode]
                                                   ,[Tta_PostFlag]
                                                   ,[Tta_IncomeAmt]
                                                   ,[Tta_CycleIndicator]
                                                   ,[Usr_Login]
                                                   ,[Ludatetime])
                                         VALUES
                                                   ('{0}'
                                                   ,'{1}'
                                                   ,'{2}'
                                                   ,'false'
                                                   ,{3}
                                                   ,'{4}'
                                                   ,'{5}'
                                                   ,GetDate())", EmployeeId, ProcessPayrollPeriod, AllowanceCode, AllowanceAmt, Cycle, UserLogin);
                #endregion
            }
            dal.ExecuteNonQuery(query);
        }

        public void InsertToDailyAllowance(bool ProcessAll, string EmployeeId)
        {
            string EmployeeCondition = "";
            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition += " AND Tsa_IDNo = '" + EmployeeId + "'";

            #region query
            string query = string.Format(@"DECLARE @SqlScript1 varchar(max)
                                            DECLARE @SqlScript2 varchar(max)
                                            DECLARE @Cnt smallint
                                            DECLARE @Code varchar(5)
                                            DECLARE @Payperiod varchar(10)
                                            DECLARE @user varchar(40)

                                            set @Cnt = 0
                                            set @SqlScript1 = ''
                                            set @SqlScript2 = ''
                                            set @Code =''
                                            set @Payperiod = '{0}'
                                            set @user = '{1}'
                                            while (@Cnt < 6)
                                            begin
	                                            set @Cnt = @Cnt + 1
	                                            set @Code = (select right('0' + convert(varchar(2),@Cnt) ,2))
	                                            set @SqlScript1 = @SqlScript1 + 
	                                            '
	                                            INSERT INTO T_EmpTimeBaseAllowanceCycle
	                                            select
	                                            T_EmpSystemAdj.Tsa_IDNo
	                                            , '''+ @Payperiod + '''
	                                            , Mah_AdjustmentCode
	                                            , ''false''
	                                            , (ISNULL(sum(T_EmpTimeRegisterHst.Ttr_TBAmt'+ @Code +'),0) - ISNULL(sum(T_EmpTimeRegisterTrl.Ttr_TBAmt'+ @Code +'),0))
	                                            , ''P''
	                                            , '''+ @user + '''
	                                            , GetDate()
	                                            FROM T_EmpSystemAdj 
	                                            INNER JOIN T_EmpTimeRegisterTrl on T_EmpTimeRegisterTrl.Ttr_IDNo  = T_EmpSystemAdj.Tsa_IDNo
		                                            and T_EmpTimeRegisterTrl.Ttr_Date = T_EmpSystemAdj.Tsa_Date   
		                                            and T_EmpTimeRegisterTrl.Ttr_AdjPayCycle  = T_EmpSystemAdj.Tsa_AdjPayCycle   
	                                            INNER JOIN T_EmpTimeRegisterHst on T_EmpTimeRegisterHst.Ttr_IDNo   = T_EmpSystemAdj.Tsa_IDNo  
		                                            and T_EmpTimeRegisterHst.Ttr_Date = T_EmpSystemAdj.Tsa_Date  
	                                            INNER JOIN {3}..M_VarianceAllowanceHdr ON Mvh_TimeBaseID = ''' + @Code + '''
                                                            AND Mvh_CompanyCode = ''' +  + '''
	                                            WHERE T_EmpTimeRegisterTrl.Ttr_AdjPayCycle = ''' + @Payperiod + '''
	                                            AND (ISNULL(T_EmpTimeRegisterHst.Ttr_TBAmt'+ @Code + ',0) - ISNULL(T_EmpTimeRegisterTrl.Ttr_TBAmt'+ @Code + ',0)) <> 0
                                                {2}
                                            group by T_EmpSystemAdj.Tsa_IDNo,Mvh_AdjustmentCode'
                                            end

                                            while (@Cnt < 12)
                                            begin
	                                            set @Cnt = @Cnt + 1
	                                            set @Code = (select right('0' + convert(varchar(2),@Cnt) ,2))
	                                            set @SqlScript2 = @SqlScript2 + 
	                                            '
	                                            INSERT INTO T_EmpTimeBaseAllowanceCycle
	                                            select
                                                Mvh_CompanyCode
	                                            ,T_EmpSystemAdj.Tsa_IDNo
	                                            , '''+ @Payperiod + '''
	                                            , Mvh_AdjustmentCode
	                                            , ''false''
	                                            , (ISNULL(sum(T_EmpTimeRegisterHst.Ttr_TBAmt'+ @Code +'),0) - ISNULL(sum(T_EmpTimeRegisterTrl.Ttr_TBAmt'+ @Code +'),0))
	                                            , ''P''
	                                            , '''+ @user + '''
	                                            , GETDATE()
	                                            FROM T_EmpSystemAdj 
	                                            INNER JOIN T_EmpTimeRegisterTrl on T_EmpTimeRegisterTrl.Ttr_IDNo  = T_EmpSystemAdj.Tsa_IDNo
		                                            and T_EmpTimeRegisterTrl.Ttr_Date = T_EmpSystemAdj.Tsa_Date   
		                                            and T_EmpTimeRegisterTrl.Ttr_AdjPayCycle  = T_EmpSystemAdj.Tsa_AdjPayCycle   
	                                            INNER JOIN T_EmpTimeRegisterHst on T_EmpTimeRegisterHst.Ttr_IDNo   = T_EmpSystemAdj.Tsa_IDNo  
		                                            and T_EmpTimeRegisterHst.Ttr_Date = T_EmpSystemAdj.Tsa_Date  
	                                            INNER JOIN {3}..M_VarianceAllowanceHdr ON Mvh_TimeBaseID = ''' + @Code + '''
                                                        AND Mvh_CompanyCode = ''' +  + '''
	                                            WHERE T_EmpTimeRegisterTrl.Ttr_AdjPayCycle = ''' + @Payperiod + '''
	                                            AND (ISNULL(T_EmpTimeRegisterHst.Ttr_TBAmt'+ @Code + ',0) - ISNULL(T_EmpTimeRegisterTrl.Ttr_TBAmt'+ @Code + ',0)) <> 0
                                                {2}
                                            group by T_EmpSystemAdj.Tsa_IDNo,Mvh_AdjustmentCode'
                                            end

                                            exec (@SqlScript1 + @SqlScript2)", ProcessPayrollPeriod, LoginUser, EmployeeCondition, CentralProfile);
            #endregion
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

        private bool CheckIfHasSpecialRate(string DayCode)
        {
            bool bSpecialRate = false;
            DataRow[] drSpecialPayRate = PAYCODERATE.Select(string.Format("Mpd_SubCode = '{0}'", DayCode));
            if (drSpecialPayRate.Length > 0 && drSpecialPayRate[0]["Mpd_ParamValue"].ToString() == "S")
                bSpecialRate = true;
            return bSpecialRate;
        }

        public bool CheckIfHasSpecialRate(DataTable dtSpecialRate, string DayCode)
        {
            bool bSpecialRate = false;
            DataRow[] drSpecialPayRate = dtSpecialRate.Select(string.Format("[DayCode] = '{0}'", DayCode));
            if (drSpecialPayRate.Length > 0 && drSpecialPayRate[0]["SpecialRate"].ToString() == "S")
                bSpecialRate = true;
            return bSpecialRate;
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

    }
}
