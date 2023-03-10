using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using CommonPostingLibrary;
using System.Collections;
using Posting.DAL;

namespace Posting.BLogic
{
    public class NewLaborHoursAdjustmentBL2 : BaseBL
    {
        #region <Global Variables>
        DALHelper dal;
        SystemCycleProcessingBL2 SystemCycleProcessingBL;
        CommonBL commonBL;
        NewLaborHoursGenerationBL2 newLaborHoursGen;

        //Constants
        public int FILLERCNT = 6;

        //Storage tables
        DataTable dtLaborHrsAdjustment = null;
        DataTable dtLaborHrsAdjustmentExt = null;
        DataTable dtEmployeePayrollTransactionHist = null;
        DataTable dtEmployeePayrollTransactionTrail = null;
        DataTable dtLaborHrsAdjustmentDetail = null;
        DataTable dtLaborHrsAdjustmentExtDetail = null;
        DataRow drLaborHrsAdjustment = null;
        DataRow drLaborHrsAdjustmentExt = null;
        DataRow drLaborHrsAdjustmentDetail = null;
        DataRow drLaborHrsAdjustmentExtDetail = null;
        DataRow[] drEmployeePayrollTransactionHist = null;

        //Miscellaneous
        string ProcessPayrollPeriod = "";
        string PayrollStart = "";
        string PayrollEnd = "";
        string LoginUser = "";
        bool bHasDayCodeExt = false;
        string EmployeeList = string.Empty;

        //Flags and parameters
        public decimal MDIVISOR = 0;
        public decimal HRLYRTEDEC = 0;
        public string HRRTINCDEM = "";
        public int NDBRCKTCNT = 1;
        public decimal NP1_RATE = 0;
        public decimal NP2_RATE = 0;
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

        public void AdjustLaborHours(string PayrollPeriod, string UserLogin, DALHelper dalHelper, string EmployeeList, string MainDB)
        {
            this.EmployeeList = EmployeeList;
            this.MainDB = MainDB;
            AdjustLaborHours(true, PayrollPeriod, "", UserLogin, dalHelper);
        }
        
        public void AdjustLaborHours(string PayrollPeriod, string EmployeeId, string UserLogin, DALHelper dalHelper)
        {
            AdjustLaborHours(false, PayrollPeriod, EmployeeId, UserLogin, dalHelper);
        }

        public void AdjustLaborHours(string PayrollPeriod, string UserLogin, DALHelper dalHelper, string EmployeeList)
        {
            this.EmployeeList = EmployeeList;
            AdjustLaborHours(true, PayrollPeriod, "", UserLogin, dalHelper);
        }

        public void AdjustLaborHours(bool ProcessAll, string PayrollPeriod, string EmployeeId, string UserLogin, DALHelper dalHelper)
        {
            #region Variables
            DataTable dtEmpForAdjustment = null;
            DataTable dtAllowances = null;
            DataRow[] drArrAllowance = null;
            DataTable dtDayCodePremiums = null;
            DataTable dtDayCodePremiumFillers = null;
            DataRow[] drDayCodePremium = null;
            DataRow[] drDayCodePremiumFiller = null;
            string strEmployeeID = "";
            string strPrevEmployeeID = "";
            string strProcessDate;

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

            string fillerHrCol = "";
            string fillerOTHrCol = "";
            string fillerNDHrCol = "";
            string fillerOTNDHrCol = "";
            string fillerAmtCol = "";
            string fillerOTAmtCol = "";
            string fillerNDAmtCol = "";
            string fillerOTNDAmtCol = "";

            string strAllowanceCol = "";
            decimal dAlwAmt = 0;
            string PayrollType = "";
            string PremiumGroup = "";
            string EmploymentStatus = "";
            decimal SalaryRate = 0;
            decimal HourlyRate = 0;
            double dShiftHoursTrail = 0;
            double dShiftHoursHist = 0;
            #endregion

            try
            {
                //dal = new DALHelper();
                this.dal = dalHelper;
                SystemCycleProcessingBL = new SystemCycleProcessingBL2(dal, PayrollPeriod, UserLogin);
                newLaborHoursGen = new NewLaborHoursGenerationBL2();
                newLaborHoursGen.EmpDispHandler += new NewLaborHoursGenerationBL2.EmpDispEventHandler(ShowEmployeeName);
                newLaborHoursGen.StatusHandler += new NewLaborHoursGenerationBL2.StatusEventHandler(ShowLaborHourStatus);
                commonBL = new CommonBL(MainDB);
                CommonBL.HOURSINDAY = CommonBL.GetHoursInDay();
                //-----------------------------
                //Check for Existing Day Codes
                if (GetFillerDayCodesCount() > 0)
                {
                    bHasDayCodeExt = true;
                    dtDayCodePremiumFillers = GetDayCodePremiumFillers();
                }
                else
                {
                    bHasDayCodeExt = false;
                    dtDayCodePremiumFillers = null;
                }
                //-----------------------------
                SetProcessFlags();
                //-----------------------------
                ProcessPayrollPeriod = PayrollPeriod;
                LoginUser = UserLogin;
                DataTable dtPayPeriod = GetPayPeriodCycle(ProcessPayrollPeriod);
                if (dtPayPeriod.Rows.Count > 0)
                {
                    PayrollStart = dtPayPeriod.Rows[0][1].ToString();
                    PayrollEnd = dtPayPeriod.Rows[0][2].ToString();
                }
                //-----------------------------
                DALHelper dalTemp = new DALHelper(MainDB, false);
                dtLaborHrsAdjustment = DbRecord.GenerateTable(CommonConstants.TableName.T_LaborHrsAdjustment, dalTemp);
                dtLaborHrsAdjustmentExt = DbRecord.GenerateTable(CommonConstants.TableName.T_LaborHrsAdjustmentExt, dalTemp);
                dtLaborHrsAdjustmentDetail = DbRecord.GenerateTable("T_LaborHrsAdjustmentDetail", dalTemp);
                dtLaborHrsAdjustmentExtDetail = DbRecord.GenerateTable("T_LaborHrsAdjustmentExtDetail", dalTemp);
                //-----------------------------
                //Delete labor hours adjustment record first
                StatusHandler(this, new StatusEventArgs("Clear Labor Hours Adjustment Table", false));
                ClearAdjustmentTable(ProcessAll, EmployeeId);
                StatusHandler(this, new StatusEventArgs("Clear Labor Hours Adjustment Table", true));

                //Get all allowances
                dtAllowances = GetAllowanceHeader();
                //Get day code premiums
                dtDayCodePremiums = GetDayCodePremiums();
                //-----------------------------
                StatusHandler(this, new StatusEventArgs("Initialize Labor Hours Generation", false));
                dtEmpForAdjustment = GetAllEmployeesForAdjustment(ProcessAll, EmployeeId);
                StatusHandler(this, new StatusEventArgs("Initialize Labor Hours Generation", true));
                DataView dvEmpWithAdj = new DataView(dtEmpForAdjustment);
                DataTable dtEmpWithAdj = dvEmpWithAdj.ToTable(true, "Ell_PayPeriod");
                DataRow[] drArrEmpWithAdj;
                string strEmpList;
                for (int i = 0; i < dtEmpWithAdj.Rows.Count; i++)
                {
                    drArrEmpWithAdj = dtEmpForAdjustment.Select("Ell_PayPeriod = '" + dtEmpWithAdj.Rows[i]["Ell_PayPeriod"].ToString() + "'");
                    if (drArrEmpWithAdj.Length > 0)
                    {
                        strEmpList = "";
                        foreach (DataRow drRow in drArrEmpWithAdj)
                        {
                            strEmpList += string.Format("'{0}',", drRow["Ell_EmployeeId"].ToString());
                            //EmpDispHandler(this, new EmpDispEventArgs(drRow["Ell_EmployeeId"].ToString(), drRow["Emt_LastName"].ToString(), drRow["Emt_FirstName"].ToString()));
                        }
                        if (strEmpList != "")
                        {
                            strEmpList = strEmpList.Substring(0, strEmpList.Length - 1);
                            newLaborHoursGen.GenerateLaborHours(true, false, false, dtEmpWithAdj.Rows[i]["Ell_PayPeriod"].ToString(), "", false, "", strEmpList, UserLogin, dalHelper, MainDB);
                        }
                    }
                }
                //-----------------------------
                StatusHandler(this, new StatusEventArgs("Get Adjustment Trail and History Tables", false));
                dtEmployeePayrollTransactionTrail = GetEmployeePayrollTransactionTrailRecords(ProcessAll, EmployeeId, dalHelper);
                dtEmployeePayrollTransactionHist = GetEmployeePayrollTransactionHistRecords(ProcessAll, EmployeeId, dalHelper);
                StatusHandler(this, new StatusEventArgs("Get Adjustment Trail and History Tables", true));
                //-----------------------------
                if (dtEmployeePayrollTransactionTrail.Rows.Count > 0)
                {
                    StatusHandler(this, new StatusEventArgs("Initialize Adjustment Amount Computation", false));
                    StatusHandler(this, new StatusEventArgs("Initialize Adjustment Amount Computation", true));
                    for (int i = 0; i < dtEmployeePayrollTransactionTrail.Rows.Count; i++)
                    {
                        try
                        {
                            strEmployeeID = dtEmployeePayrollTransactionTrail.Rows[i]["Ept_EmployeeId"].ToString();
                            strProcessDate = dtEmployeePayrollTransactionTrail.Rows[i]["Ept_ProcessDate"].ToString();

                            if (strEmployeeID != strPrevEmployeeID)
                                EmpDispHandler(this, new EmpDispEventArgs(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_EmployeeId"].ToString(), dtEmployeePayrollTransactionTrail.Rows[i]["Emt_LastName"].ToString(), dtEmployeePayrollTransactionTrail.Rows[i]["Emt_FirstName"].ToString()));

                            //Get Hist Detail record
                            drEmployeePayrollTransactionHist = dtEmployeePayrollTransactionHist.Select("Ept_EmployeeId = '" + strEmployeeID + "' AND Ept_ProcessDate = '" + strProcessDate + "'");

                            #region Initialize Labor Hours Adjustment and Extension Rows
                            drLaborHrsAdjustment = dtLaborHrsAdjustment.NewRow();
                            drLaborHrsAdjustmentDetail = dtLaborHrsAdjustmentDetail.NewRow();
                            if (bHasDayCodeExt)
                            {
                                drLaborHrsAdjustmentExt = dtLaborHrsAdjustmentExt.NewRow();
                                for (int j = 1; j <= FILLERCNT; j++)
                                {
                                    //initialize
                                    fillerHrCol = string.Format("Lha_Filler{0:00}_Hr", j);
                                    fillerOTHrCol = string.Format("Lha_Filler{0:00}_OTHr", j);
                                    fillerNDHrCol = string.Format("Lha_Filler{0:00}_NDHr", j);
                                    fillerOTNDHrCol = string.Format("Lha_Filler{0:00}_OTNDHr", j);
                                    drLaborHrsAdjustmentExt[fillerHrCol] = 0;
                                    drLaborHrsAdjustmentExt[fillerOTHrCol] = 0;
                                    drLaborHrsAdjustmentExt[fillerNDHrCol] = 0;
                                    drLaborHrsAdjustmentExt[fillerOTNDHrCol] = 0;
                                }
                                drLaborHrsAdjustmentExtDetail = dtLaborHrsAdjustmentExtDetail.NewRow();
                                for (int j = 1; j <= FILLERCNT; j++)
                                {
                                    //initialize
                                    fillerHrCol = string.Format("Lhd_Filler{0:00}_Amt", j);
                                    fillerOTHrCol = string.Format("Lhd_Filler{0:00}_OTAmt", j);
                                    fillerNDHrCol = string.Format("Lhd_Filler{0:00}_NDAmt", j);
                                    fillerOTNDHrCol = string.Format("Lhd_Filler{0:00}_OTNDAmt", j);
                                    drLaborHrsAdjustmentExtDetail[fillerHrCol] = 0;
                                    drLaborHrsAdjustmentExtDetail[fillerOTHrCol] = 0;
                                    drLaborHrsAdjustmentExtDetail[fillerNDHrCol] = 0;
                                    drLaborHrsAdjustmentExtDetail[fillerOTNDHrCol] = 0;
                                }
                            }
                            #endregion

                            #region Update Labor Hours Adjustment Row (Part 1)
                            drLaborHrsAdjustment["Lha_RegularHr"] = ((Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_RegularHr"]) + Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_PaidLeaveHr"]))
                                                                        - (Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_RegularHr"]) + Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_PaidLeaveHr"])));
                            drLaborHrsAdjustment["Lha_RegularOTHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_RegularOTHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_RegularOTHr"]);
                            drLaborHrsAdjustment["Lha_RegularNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_RegularNDHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_RegularNDHr"]);
                            drLaborHrsAdjustment["Lha_RegularOTNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_RegularOTNDHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_RegularOTNDHr"]);
                            drLaborHrsAdjustment["Lha_RestdayHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_RestdayHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_RestdayHr"]);
                            drLaborHrsAdjustment["Lha_RestdayOTHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_RestdayOTHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_RestdayOTHr"]);
                            drLaborHrsAdjustment["Lha_RestdayNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_RestdayNDHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_RestdayNDHr"]);
                            drLaborHrsAdjustment["Lha_RestdayOTNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_RestdayOTNDHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_RestdayOTNDHr"]);
                            drLaborHrsAdjustment["Lha_LegalHolidayHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_LegalHolidayHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_LegalHolidayHr"]);
                            drLaborHrsAdjustment["Lha_LegalHolidayOTHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_LegalHolidayOTHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_LegalHolidayOTHr"]);
                            drLaborHrsAdjustment["Lha_LegalHolidayNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_LegalHolidayNDHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_LegalHolidayNDHr"]);
                            drLaborHrsAdjustment["Lha_LegalHolidayOTNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_LegalHolidayOTNDHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_LegalHolidayOTNDHr"]);
                            drLaborHrsAdjustment["Lha_SpecialHolidayHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_SpecialHolidayHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_SpecialHolidayHr"]);
                            drLaborHrsAdjustment["Lha_SpecialHolidayOTHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_SpecialHolidayOTHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_SpecialHolidayOTHr"]);
                            drLaborHrsAdjustment["Lha_SpecialHolidayNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_SpecialHolidayNDHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_SpecialHolidayNDHr"]);
                            drLaborHrsAdjustment["Lha_SpecialHolidayOTNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_SpecialHolidayOTNDHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_SpecialHolidayOTNDHr"]);
                            drLaborHrsAdjustment["Lha_PlantShutdownHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_PlantShutdownHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_PlantShutdownHr"]);
                            drLaborHrsAdjustment["Lha_PlantShutdownOTHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_PlantShutdownOTHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_PlantShutdownOTHr"]);
                            drLaborHrsAdjustment["Lha_PlantShutdownNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_PlantShutdownNDHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_PlantShutdownNDHr"]);
                            drLaborHrsAdjustment["Lha_PlantShutdownOTNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_PlantShutdownOTNDHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_PlantShutdownOTNDHr"]);
                            drLaborHrsAdjustment["Lha_CompanyHolidayHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_CompanyHolidayHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_CompanyHolidayHr"]);
                            drLaborHrsAdjustment["Lha_CompanyHolidayOTHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_CompanyHolidayOTHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_CompanyHolidayOTHr"]);
                            drLaborHrsAdjustment["Lha_CompanyHolidayNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_CompanyHolidayNDHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_CompanyHolidayNDHr"]);
                            drLaborHrsAdjustment["Lha_CompanyHolidayOTNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_CompanyHolidayOTNDHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_CompanyHolidayOTNDHr"]);
                            drLaborHrsAdjustment["Lha_RestdayLegalHolidayHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_RestdayLegalHolidayHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_RestdayLegalHolidayHr"]);
                            drLaborHrsAdjustment["Lha_RestdayLegalHolidayOTHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_RestdayLegalHolidayOTHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_RestdayLegalHolidayOTHr"]);
                            drLaborHrsAdjustment["Lha_RestdayLegalHolidayNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_RestdayLegalHolidayNDHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_RestdayLegalHolidayNDHr"]);
                            drLaborHrsAdjustment["Lha_RestdayLegalHolidayOTNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_RestdayLegalHolidayOTNDHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_RestdayLegalHolidayOTNDHr"]);
                            drLaborHrsAdjustment["Lha_RestdaySpecialHolidayHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_RestdaySpecialHolidayHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_RestdaySpecialHolidayHr"]);
                            drLaborHrsAdjustment["Lha_RestdaySpecialHolidayOTHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_RestdaySpecialHolidayOTHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_RestdaySpecialHolidayOTHr"]);
                            drLaborHrsAdjustment["Lha_RestdaySpecialHolidayNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_RestdaySpecialHolidayNDHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_RestdaySpecialHolidayNDHr"]);
                            drLaborHrsAdjustment["Lha_RestdaySpecialHolidayOTNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_RestdaySpecialHolidayOTNDHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_RestdaySpecialHolidayOTNDHr"]);
                            drLaborHrsAdjustment["Lha_RestdayCompanyHolidayHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_RestdayCompanyHolidayHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_RestdayCompanyHolidayHr"]);
                            drLaborHrsAdjustment["Lha_RestdayCompanyHolidayOTHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_RestdayCompanyHolidayOTHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_RestdayCompanyHolidayOTHr"]);
                            drLaborHrsAdjustment["Lha_RestdayCompanyHolidayNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_RestdayCompanyHolidayNDHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_RestdayCompanyHolidayNDHr"]);
                            drLaborHrsAdjustment["Lha_RestdayCompanyHolidayOTNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_RestdayCompanyHolidayOTNDHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_RestdayCompanyHolidayOTNDHr"]);
                            drLaborHrsAdjustment["Lha_RestdayPlantShutdownHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_RestdayPlantShutdownHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_RestdayPlantShutdownHr"]);
                            drLaborHrsAdjustment["Lha_RestdayPlantShutdownOTHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_RestdayPlantShutdownOTHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_RestdayPlantShutdownOTHr"]);
                            drLaborHrsAdjustment["Lha_RestdayPlantShutdownNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_RestdayPlantShutdownNDHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_RestdayPlantShutdownNDHr"]);
                            drLaborHrsAdjustment["Lha_RestdayPlantShutdownOTNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_RestdayPlantShutdownOTNDHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_RestdayPlantShutdownOTNDHr"]);

                            drLaborHrsAdjustment["Lha_AbsentLegalHolidayHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_AbsentLegalHolidayHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_AbsentLegalHolidayHr"]);
                            drLaborHrsAdjustment["Lha_AbsentSpecialHolidayHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_AbsentSpecialHolidayHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_AbsentSpecialHolidayHr"]);
                            drLaborHrsAdjustment["Lha_AbsentCompanyHolidayHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_AbsentCompanyHolidayHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_AbsentCompanyHolidayHr"]);
                            drLaborHrsAdjustment["Lha_AbsentPlantShutdownHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_AbsentPlantShutdownHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_AbsentPlantShutdownHr"]);
                            drLaborHrsAdjustment["Lha_AbsentFillerHolidayHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_AbsentFillerHolidayHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_AbsentFillerHolidayHr"]);
                            drLaborHrsAdjustment["Lha_PaidLegalHolidayHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_PaidLegalHolidayHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_PaidLegalHolidayHr"]);
                            drLaborHrsAdjustment["Lha_PaidSpecialHolidayHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_PaidSpecialHolidayHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_PaidSpecialHolidayHr"]);
                            drLaborHrsAdjustment["Lha_PaidCompanyHolidayHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_PaidCompanyHolidayHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_PaidCompanyHolidayHr"]);
                            drLaborHrsAdjustment["Lha_PaidFillerHolidayHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_PaidFillerHolidayHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_PaidFillerHolidayHr"]);
                            drLaborHrsAdjustment["Lha_AbsentHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_AbsentHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_AbsentHr"]);
                            #endregion

                            #region Update Labor Hours Adjustment Row (Part 2)
                            drLaborHrsAdjustment["Lha_EmployeeId"] = strEmployeeID;
                            drLaborHrsAdjustment["Lha_AdjustpayPeriod"] = dtEmployeePayrollTransactionTrail.Rows[i]["Ept_AdjustPayPeriod"].ToString();
                            drLaborHrsAdjustment["Lha_PayPeriod"] = dtEmployeePayrollTransactionTrail.Rows[i]["Ept_CurrentPayPeriod"].ToString();
                            drLaborHrsAdjustment["Lha_ProcessDate"] = strProcessDate;
                            drLaborHrsAdjustment["Lha_LaborHrsAdjustmentAmt"] = 0; //temp
                            drLaborHrsAdjustment["Lha_OvertimeAdjustmentAmt"] = 0; //temp
                            drLaborHrsAdjustment["Lha_SalaryRate"] = drEmployeePayrollTransactionHist[0]["Ept_SalaryRate"].ToString();
                            drLaborHrsAdjustment["Lha_PayrollPost"] = 0;
                            drLaborHrsAdjustment["Usr_Login"] = LoginUser;
                            drLaborHrsAdjustment["Ludatetime"] = DateTime.Now;

                            SalaryRate = Convert.ToDecimal(drEmployeePayrollTransactionHist[0]["Ept_SalaryRate"]);
                            PayrollType = drEmployeePayrollTransactionHist[0]["Ept_PayrollType"].ToString();
                            PremiumGroup = dtEmployeePayrollTransactionTrail.Rows[i]["Emt_PremiumGroup"].ToString();
                            EmploymentStatus = dtEmployeePayrollTransactionTrail.Rows[i]["Emt_EmploymentStatus"].ToString();
                            MDIVISOR = Convert.ToDecimal(GetScalarValue("Pmx_ParameterValue", "T_ParameterMasterExt", string.Format("Pmx_ParameterID = 'MDIVISOR' AND Pmx_Classification = '{0}' AND Pmx_Status = 'A'", EmploymentStatus), dalHelper));

                            //Get Hourly Rate
                            HourlyRate = 0;
                            if (Convert.ToBoolean(HRRTINCDEM) == true) //Lear
                            {
                                HourlyRate = Convert.ToDecimal(drEmployeePayrollTransactionHist[0]["HourlyRateWithDeminimis"]);
                            }
                            else
                            {
                                if (PayrollType == "D")
                                    HourlyRate = SalaryRate / CommonBL.HOURSINDAY;
                                else if (PayrollType == "M")
                                    HourlyRate = SalaryRate * 12 / MDIVISOR / CommonBL.HOURSINDAY;
                                else if (PayrollType == "H")
                                    HourlyRate = SalaryRate;
                            }
                            if (HRLYRTEDEC > 0) //FEP
                            {
                                if (PayrollType == "D")
                                    HourlyRate = Math.Round(SalaryRate / CommonBL.HOURSINDAY, Convert.ToInt32(HRLYRTEDEC), MidpointRounding.AwayFromZero);
                                else if (PayrollType == "M")
                                    HourlyRate = Math.Round(SalaryRate * 12 / MDIVISOR / CommonBL.HOURSINDAY, Convert.ToInt32(HRLYRTEDEC), MidpointRounding.AwayFromZero);
                                else if (PayrollType == "H")
                                    HourlyRate = Math.Round(SalaryRate, Convert.ToInt32(HRLYRTEDEC), MidpointRounding.AwayFromZero);
                            }
                            drLaborHrsAdjustment["Lha_HourlyRate"] = HourlyRate;
                            #endregion

                            #region Update Labor Hours Adjustment Detail Row
                            drLaborHrsAdjustmentDetail["Lhd_EmployeeId"] = strEmployeeID;
                            drLaborHrsAdjustmentDetail["Lhd_AdjustpayPeriod"] = dtEmployeePayrollTransactionTrail.Rows[i]["Ept_AdjustPayPeriod"].ToString();
                            drLaborHrsAdjustmentDetail["Lhd_PayPeriod"] = dtEmployeePayrollTransactionTrail.Rows[i]["Ept_CurrentPayPeriod"].ToString();
                            drLaborHrsAdjustmentDetail["Lhd_ProcessDate"] = strProcessDate;
                            drLaborHrsAdjustmentDetail["Lhd_SalaryRate"] = drEmployeePayrollTransactionHist[0]["Ept_SalaryRate"].ToString();
                            drLaborHrsAdjustmentDetail["Lhd_PayrollType"] = drEmployeePayrollTransactionHist[0]["Ept_PayrollType"].ToString();
                            drLaborHrsAdjustmentDetail["Lhd_HourlyRate"] = HourlyRate; 

                            drLaborHrsAdjustmentDetail["Lhd_LateHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_LateHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_LateHr"]);
                            drLaborHrsAdjustmentDetail["Lhd_UndertimeHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_UndertimeHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_UndertimeHr"]);
                            drLaborHrsAdjustmentDetail["Lhd_WholeDayAbsentHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_WholeDayAbsentHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_WholeDayAbsentHr"]);
                            drLaborHrsAdjustmentDetail["Lhd_UnpaidLeaveHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_UnpaidLeaveHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_UnpaidLeaveHr"]);
                            drLaborHrsAdjustmentDetail["Lhd_RegularHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_RegularHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_RegularHr"]);
                            drLaborHrsAdjustmentDetail["Lhd_PaidLeaveHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_PaidLeaveHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_PaidLeaveHr"]);
                            drLaborHrsAdjustmentDetail["Lhd_TotalRegularHr"] = ((Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_RegularHr"]) + Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_PaidLeaveHr"]))
                                                                                - (Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_RegularHr"]) + Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_PaidLeaveHr"])));
                            drLaborHrsAdjustmentDetail["Lhd_RestdayLegalHolidayCount"] = 0;
                            drLaborHrsAdjustmentDetail["Lhd_AbsentAmt"] = 0;
                            drLaborHrsAdjustmentDetail["Lhd_LateAmt"] = 0;
                            drLaborHrsAdjustmentDetail["Lhd_UndertimeAmt"] = 0;
                            drLaborHrsAdjustmentDetail["Lhd_WholeDayAbsentAmt"] = 0;
                            drLaborHrsAdjustmentDetail["Lhd_UnpaidLeaveAmt"] = 0;
                            drLaborHrsAdjustmentDetail["Lhd_AbsentLegalHolidayAmt"] = 0;
                            drLaborHrsAdjustmentDetail["Lhd_AbsentSpecialHolidayAmt"] = 0;
                            drLaborHrsAdjustmentDetail["Lhd_AbsentCompanyHolidayAmt"] = 0;
                            drLaborHrsAdjustmentDetail["Lhd_AbsentPlantShutdownAmt"] = 0;
                            drLaborHrsAdjustmentDetail["Lhd_AbsentFillerHolidayAmt"] = 0;
                            drLaborHrsAdjustmentDetail["Lhd_PaidLeaveAmt"] = 0;
                            drLaborHrsAdjustmentDetail["Lhd_PaidLegalHolidayAmt"] = 0;
                            drLaborHrsAdjustmentDetail["Lhd_PaidSpecialHolidayAmt"] = 0;
                            drLaborHrsAdjustmentDetail["Lhd_PaidCompanyHolidayAmt"] = 0;
                            drLaborHrsAdjustmentDetail["Lhd_PaidFillerHolidayAmt"] = 0;
                            drLaborHrsAdjustmentDetail["Lhd_RegularAdjustmentAmt"] = 0;
                            drLaborHrsAdjustmentDetail["Lhd_OvertimeAdjustmentAmt"] = 0;
                            drLaborHrsAdjustmentDetail["Lhd_HolidayAdjustmentAmt"] = 0;
                            drLaborHrsAdjustmentDetail["Lhd_NightPremiumAdjustmentAmt"] = 0;
                            drLaborHrsAdjustmentDetail["Lhd_LeaveAdjustmentAmt"] = 0;
                            drLaborHrsAdjustmentDetail["Lhd_LaborHrsAdjustmentAmt"] = 0;
                            
                            drLaborHrsAdjustmentDetail["Usr_Login"] = LoginUser;
                            drLaborHrsAdjustmentDetail["Ludatetime"] = DateTime.Now;
                            #endregion

                            #region Update Labor Hours Adjustment Extension Row
                            if (bHasDayCodeExt)
                            {
                                #region Update Labor Hours Adjustment Ext Row (Part 1)
                                drLaborHrsAdjustmentExt["Lha_Filler01_Hr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_Filler01_Hr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_Filler01_Hr"]);
                                drLaborHrsAdjustmentExt["Lha_Filler01_OTHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_Filler01_OTHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_Filler01_OTHr"]);
                                drLaborHrsAdjustmentExt["Lha_Filler01_NDHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_Filler01_NDHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_Filler01_NDHr"]);
                                drLaborHrsAdjustmentExt["Lha_Filler01_OTNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_Filler01_OTNDHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_Filler01_OTNDHr"]);
                                drLaborHrsAdjustmentExt["Lha_Filler02_Hr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_Filler02_Hr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_Filler02_Hr"]);
                                drLaborHrsAdjustmentExt["Lha_Filler02_OTHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_Filler02_OTHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_Filler02_OTHr"]);
                                drLaborHrsAdjustmentExt["Lha_Filler02_NDHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_Filler02_NDHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_Filler02_NDHr"]);
                                drLaborHrsAdjustmentExt["Lha_Filler02_OTNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_Filler02_OTNDHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_Filler02_OTNDHr"]);
                                drLaborHrsAdjustmentExt["Lha_Filler03_Hr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_Filler03_Hr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_Filler03_Hr"]);
                                drLaborHrsAdjustmentExt["Lha_Filler03_OTHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_Filler03_OTHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_Filler03_OTHr"]);
                                drLaborHrsAdjustmentExt["Lha_Filler03_NDHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_Filler03_NDHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_Filler03_NDHr"]);
                                drLaborHrsAdjustmentExt["Lha_Filler03_OTNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_Filler03_OTNDHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_Filler03_OTNDHr"]);
                                drLaborHrsAdjustmentExt["Lha_Filler04_Hr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_Filler04_Hr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_Filler04_Hr"]);
                                drLaborHrsAdjustmentExt["Lha_Filler04_OTHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_Filler04_OTHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_Filler04_OTHr"]);
                                drLaborHrsAdjustmentExt["Lha_Filler04_NDHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_Filler04_NDHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_Filler04_NDHr"]);
                                drLaborHrsAdjustmentExt["Lha_Filler04_OTNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_Filler04_OTNDHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_Filler04_OTNDHr"]);
                                drLaborHrsAdjustmentExt["Lha_Filler05_Hr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_Filler05_Hr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_Filler05_Hr"]);
                                drLaborHrsAdjustmentExt["Lha_Filler05_OTHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_Filler05_OTHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_Filler05_OTHr"]);
                                drLaborHrsAdjustmentExt["Lha_Filler05_NDHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_Filler05_NDHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_Filler05_NDHr"]);
                                drLaborHrsAdjustmentExt["Lha_Filler05_OTNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_Filler05_OTNDHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_Filler05_OTNDHr"]);
                                drLaborHrsAdjustmentExt["Lha_Filler06_Hr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_Filler06_Hr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_Filler06_Hr"]);
                                drLaborHrsAdjustmentExt["Lha_Filler06_OTHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_Filler06_OTHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_Filler06_OTHr"]);
                                drLaborHrsAdjustmentExt["Lha_Filler06_NDHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_Filler06_NDHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_Filler06_NDHr"]);
                                drLaborHrsAdjustmentExt["Lha_Filler06_OTNDHr"] = Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_Filler06_OTNDHr"]) - Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_Filler06_OTNDHr"]);
                                #endregion

                                #region Update Labor Hours Adjustment Ext Row (Part 2)
                                drLaborHrsAdjustmentExt["Lha_EmployeeId"] = strEmployeeID;
                                drLaborHrsAdjustmentExt["Lha_AdjustpayPeriod"] = dtEmployeePayrollTransactionTrail.Rows[i]["Ept_AdjustPayPeriod"].ToString();
                                drLaborHrsAdjustmentExt["Lha_PayPeriod"] = dtEmployeePayrollTransactionTrail.Rows[i]["Ept_CurrentPayPeriod"].ToString();
                                drLaborHrsAdjustmentExt["Lha_ProcessDate"] = strProcessDate;
                                drLaborHrsAdjustmentExt["Usr_Login"] = LoginUser;
                                drLaborHrsAdjustmentExt["Ludatetime"] = DateTime.Now;
                                #endregion

                                #region Update Labor Hours Adjustment Ext Detail Row
                                drLaborHrsAdjustmentExtDetail["Lhd_EmployeeId"] = strEmployeeID;
                                drLaborHrsAdjustmentExtDetail["Lhd_AdjustpayPeriod"] = dtEmployeePayrollTransactionTrail.Rows[i]["Ept_AdjustPayPeriod"].ToString();
                                drLaborHrsAdjustmentExtDetail["Lhd_PayPeriod"] = dtEmployeePayrollTransactionTrail.Rows[i]["Ept_CurrentPayPeriod"].ToString();
                                drLaborHrsAdjustmentExtDetail["Lhd_ProcessDate"] = strProcessDate;
                                drLaborHrsAdjustmentExtDetail["Usr_Login"] = LoginUser;
                                drLaborHrsAdjustmentExtDetail["Ludatetime"] = DateTime.Now;
                                #endregion
                            }
                            #endregion

                            #region Insert Allowances
                            for (int iAlwIdx = 1; iAlwIdx <= 12; iAlwIdx++)
                            {
                                strAllowanceCol = string.Format("Ell_AllowanceAmt{0:00}", iAlwIdx);
                                drArrAllowance = dtAllowances.Select(string.Format("Alh_LedgerAlwCol = '{0:00}'", iAlwIdx));
                                if (drArrAllowance.Length > 0)
                                {
                                    dAlwAmt = Convert.ToDecimal(drEmployeePayrollTransactionHist[0][strAllowanceCol]) - Convert.ToDecimal(dtEmployeePayrollTransactionTrail.Rows[i][strAllowanceCol]);
                                    if (dAlwAmt != 0)
                                        InsertEmployeeDailyAllowance(strEmployeeID, drArrAllowance[0]["Alh_AllowanceAdjCode"].ToString(), dAlwAmt, "P", LoginUser);
                                }
                            }
                            #endregion

                            #region Load group premiums
                            if (PayrollType != "" && PremiumGroup != "")
                            {
                                drDayCodePremium = dtDayCodePremiums.Select(string.Format("Dpm_PremiumGroup = '{0}' AND Dpm_Daycode = '{1}' AND Dpm_RestDay = {2} AND Dpm_PayrollType = '{3}'", PremiumGroup, "REG", false, PayrollType));
                                if (drDayCodePremium.Length > 0)
                                {
                                    Reg = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularHourPremium"]) / 100;
                                    RegOT = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeHourPremium"]) / 100;
                                    RegND = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularNDHourPremium"]) / 100;
                                    RegOTND = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeNDHourPremium"]) / 100;
                                    RegNDPercent = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularNDPercentage"]) / 100;
                                    RegOTNDPercent = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeNDPercentage"]) / 100;
                                }
                                else
                                {
                                    drDayCodePremium = dtDayCodePremiums.Select(string.Format("Dpm_PremiumGroup = '{0}' AND Dpm_Daycode = '{1}' AND Dpm_RestDay = {2} AND Dpm_PayrollType = '{3}'", "DEFAULTGRP", "REG", false, PayrollType));
                                    if (drDayCodePremium.Length > 0)
                                    {
                                        Reg = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularHourPremium"]) / 100;
                                        RegOT = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeHourPremium"]) / 100;
                                        RegND = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularNDHourPremium"]) / 100;
                                        RegOTND = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeNDHourPremium"]) / 100;
                                        RegNDPercent = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularNDPercentage"]) / 100;
                                        RegOTNDPercent = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeNDPercentage"]) / 100;
                                    }
                                    else
                                        throw new PayrollException(PremiumGroup + " premium group has no REG premium.");
                                }

                                drDayCodePremium = dtDayCodePremiums.Select(string.Format("Dpm_PremiumGroup = '{0}' AND Dpm_Daycode = '{1}' AND Dpm_RestDay = {2} AND Dpm_PayrollType = '{3}'", PremiumGroup, "REST", true, PayrollType));
                                if (drDayCodePremium.Length > 0)
                                {
                                    Rest = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularHourPremium"]) / 100;
                                    RestOT = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeHourPremium"]) / 100;
                                    RestND = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularNDHourPremium"]) / 100;
                                    RestOTND = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeNDHourPremium"]) / 100;
                                    RestNDPercent = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularNDPercentage"]) / 100;
                                    RestOTNDPercent = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeNDPercentage"]) / 100;
                                }
                                else
                                {
                                    drDayCodePremium = dtDayCodePremiums.Select(string.Format("Dpm_PremiumGroup = '{0}' AND Dpm_Daycode = '{1}' AND Dpm_RestDay = {2} AND Dpm_PayrollType = '{3}'", "DEFAULTGRP", "REST", true, PayrollType));
                                    if (drDayCodePremium.Length > 0)
                                    {
                                        Rest = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularHourPremium"]) / 100;
                                        RestOT = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeHourPremium"]) / 100;
                                        RestND = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularNDHourPremium"]) / 100;
                                        RestOTND = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeNDHourPremium"]) / 100;
                                        RestNDPercent = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularNDPercentage"]) / 100;
                                        RestOTNDPercent = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeNDPercentage"]) / 100;
                                    }
                                    else
                                        throw new PayrollException(PremiumGroup + " premium group has no REST premium.");
                                }

                                drDayCodePremium = dtDayCodePremiums.Select(string.Format("Dpm_PremiumGroup = '{0}' AND Dpm_Daycode = '{1}' AND Dpm_RestDay = {2} AND Dpm_PayrollType = '{3}'", PremiumGroup, "HOL", false, PayrollType));
                                if (drDayCodePremium.Length > 0)
                                {
                                    Hol = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularHourPremium"]) / 100;
                                    HolOT = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeHourPremium"]) / 100;
                                    HolND = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularNDHourPremium"]) / 100;
                                    HolOTND = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeNDHourPremium"]) / 100;
                                    HolNDPercent = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularNDPercentage"]) / 100;
                                    HolOTNDPercent = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeNDPercentage"]) / 100;
                                }
                                else
                                {
                                    drDayCodePremium = dtDayCodePremiums.Select(string.Format("Dpm_PremiumGroup = '{0}' AND Dpm_Daycode = '{1}' AND Dpm_RestDay = {2} AND Dpm_PayrollType = '{3}'", "DEFAULTGRP", "HOL", false, PayrollType));
                                    if (drDayCodePremium.Length > 0)
                                    {
                                        Hol = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularHourPremium"]) / 100;
                                        HolOT = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeHourPremium"]) / 100;
                                        HolND = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularNDHourPremium"]) / 100;
                                        HolOTND = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeNDHourPremium"]) / 100;
                                        HolNDPercent = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularNDPercentage"]) / 100;
                                        HolOTNDPercent = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeNDPercentage"]) / 100;
                                    }
                                    else
                                        throw new PayrollException(PremiumGroup + " premium group has no HOL premium.");
                                }

                                drDayCodePremium = dtDayCodePremiums.Select(string.Format("Dpm_PremiumGroup = '{0}' AND Dpm_Daycode = '{1}' AND Dpm_RestDay = {2} AND Dpm_PayrollType = '{3}'", PremiumGroup, "SPL", false, PayrollType));
                                if (drDayCodePremium.Length > 0)
                                {
                                    SPL = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularHourPremium"]) / 100;
                                    SPLOT = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeHourPremium"]) / 100;
                                    SPLND = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularNDHourPremium"]) / 100;
                                    SPLOTND = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeNDHourPremium"]) / 100;
                                    SPLNDPercent = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularNDPercentage"]) / 100;
                                    SPLOTNDPercent = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeNDPercentage"]) / 100;
                                }
                                else
                                {
                                    drDayCodePremium = dtDayCodePremiums.Select(string.Format("Dpm_PremiumGroup = '{0}' AND Dpm_Daycode = '{1}' AND Dpm_RestDay = {2} AND Dpm_PayrollType = '{3}'", "DEFAULTGRP", "SPL", false, PayrollType));
                                    if (drDayCodePremium.Length > 0)
                                    {
                                        SPL = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularHourPremium"]) / 100;
                                        SPLOT = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeHourPremium"]) / 100;
                                        SPLND = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularNDHourPremium"]) / 100;
                                        SPLOTND = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeNDHourPremium"]) / 100;
                                        SPLNDPercent = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularNDPercentage"]) / 100;
                                        SPLOTNDPercent = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeNDPercentage"]) / 100;
                                    }
                                    else
                                        throw new PayrollException(PremiumGroup + " premium group has no SPL premium.");
                                }

                                drDayCodePremium = dtDayCodePremiums.Select(string.Format("Dpm_PremiumGroup = '{0}' AND Dpm_Daycode = '{1}' AND Dpm_RestDay = {2} AND Dpm_PayrollType = '{3}'", PremiumGroup, "PSD", false, PayrollType));
                                if (drDayCodePremium.Length > 0)
                                {
                                    PSD = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularHourPremium"]) / 100;
                                    PSDOT = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeHourPremium"]) / 100;
                                    PSDND = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularNDHourPremium"]) / 100;
                                    PSDOTND = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeNDHourPremium"]) / 100;
                                    PSDNDPercent = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularNDPercentage"]) / 100;
                                    PSDOTNDPercent = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeNDPercentage"]) / 100;
                                }
                                else
                                {
                                    drDayCodePremium = dtDayCodePremiums.Select(string.Format("Dpm_PremiumGroup = '{0}' AND Dpm_Daycode = '{1}' AND Dpm_RestDay = {2} AND Dpm_PayrollType = '{3}'", "DEFAULTGRP", "PSD", false, PayrollType));
                                    if (drDayCodePremium.Length > 0)
                                    {
                                        PSD = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularHourPremium"]) / 100;
                                        PSDOT = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeHourPremium"]) / 100;
                                        PSDND = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularNDHourPremium"]) / 100;
                                        PSDOTND = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeNDHourPremium"]) / 100;
                                        PSDNDPercent = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularNDPercentage"]) / 100;
                                        PSDOTNDPercent = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeNDPercentage"]) / 100;
                                    }
                                    else
                                        throw new PayrollException(PremiumGroup + " premium group has no PSD premium.");
                                }

                                drDayCodePremium = dtDayCodePremiums.Select(string.Format("Dpm_PremiumGroup = '{0}' AND Dpm_Daycode = '{1}' AND Dpm_RestDay = {2} AND Dpm_PayrollType = '{3}'", PremiumGroup, "COMP", false, PayrollType));
                                if (drDayCodePremium.Length > 0)
                                {
                                    Comp = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularHourPremium"]) / 100;
                                    CompOT = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeHourPremium"]) / 100;
                                    CompND = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularNDHourPremium"]) / 100;
                                    CompOTND = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeNDHourPremium"]) / 100;
                                    CompNDPercent = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularNDPercentage"]) / 100;
                                    CompOTNDPercent = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeNDPercentage"]) / 100;
                                }
                                else
                                {
                                    drDayCodePremium = dtDayCodePremiums.Select(string.Format("Dpm_PremiumGroup = '{0}' AND Dpm_Daycode = '{1}' AND Dpm_RestDay = {2} AND Dpm_PayrollType = '{3}'", "DEFAULTGRP", "COMP", false, PayrollType));
                                    if (drDayCodePremium.Length > 0)
                                    {
                                        Comp = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularHourPremium"]) / 100;
                                        CompOT = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeHourPremium"]) / 100;
                                        CompND = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularNDHourPremium"]) / 100;
                                        CompOTND = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeNDHourPremium"]) / 100;
                                        CompNDPercent = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularNDPercentage"]) / 100;
                                        CompOTNDPercent = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeNDPercentage"]) / 100;
                                    }
                                    else
                                        throw new PayrollException(PremiumGroup + " premium group has no COMP premium.");
                                }

                                drDayCodePremium = dtDayCodePremiums.Select(string.Format("Dpm_PremiumGroup = '{0}' AND Dpm_Daycode = '{1}' AND Dpm_RestDay = {2} AND Dpm_PayrollType = '{3}'", PremiumGroup, "HOL", true, PayrollType));
                                if (drDayCodePremium.Length > 0)
                                {
                                    RestHol = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularHourPremium"]) / 100;
                                    RestHolOT = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeHourPremium"]) / 100;
                                    RestHolND = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularNDHourPremium"]) / 100;
                                    RestHolOTND = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeNDHourPremium"]) / 100;
                                    RestHolNDPercent = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularNDPercentage"]) / 100;
                                    RestHolOTNDPercent = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeNDPercentage"]) / 100;
                                }
                                else
                                {
                                    drDayCodePremium = dtDayCodePremiums.Select(string.Format("Dpm_PremiumGroup = '{0}' AND Dpm_Daycode = '{1}' AND Dpm_RestDay = {2} AND Dpm_PayrollType = '{3}'", "DEFAULTGRP", "HOL", true, PayrollType));
                                    if (drDayCodePremium.Length > 0)
                                    {
                                        RestHol = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularHourPremium"]) / 100;
                                        RestHolOT = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeHourPremium"]) / 100;
                                        RestHolND = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularNDHourPremium"]) / 100;
                                        RestHolOTND = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeNDHourPremium"]) / 100;
                                        RestHolNDPercent = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularNDPercentage"]) / 100;
                                        RestHolOTNDPercent = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeNDPercentage"]) / 100;
                                    }
                                    else
                                        throw new PayrollException(PremiumGroup + " premium group has no RESTHOL premium.");
                                }

                                drDayCodePremium = dtDayCodePremiums.Select(string.Format("Dpm_PremiumGroup = '{0}' AND Dpm_Daycode = '{1}' AND Dpm_RestDay = {2} AND Dpm_PayrollType = '{3}'", PremiumGroup, "SPL", true, PayrollType));
                                if (drDayCodePremium.Length > 0)
                                {
                                    RestSPL = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularHourPremium"]) / 100;
                                    RestSPLOT = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeHourPremium"]) / 100;
                                    RestSPLND = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularNDHourPremium"]) / 100;
                                    RestSPLOTND = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeNDHourPremium"]) / 100;
                                    RestSPLNDPercent = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularNDPercentage"]) / 100;
                                    RestSPLOTNDPercent = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeNDPercentage"]) / 100;
                                }
                                else
                                {
                                    drDayCodePremium = dtDayCodePremiums.Select(string.Format("Dpm_PremiumGroup = '{0}' AND Dpm_Daycode = '{1}' AND Dpm_RestDay = {2} AND Dpm_PayrollType = '{3}'", "DEFAULTGRP", "SPL", true, PayrollType));
                                    if (drDayCodePremium.Length > 0)
                                    {
                                        RestSPL = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularHourPremium"]) / 100;
                                        RestSPLOT = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeHourPremium"]) / 100;
                                        RestSPLND = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularNDHourPremium"]) / 100;
                                        RestSPLOTND = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeNDHourPremium"]) / 100;
                                        RestSPLNDPercent = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularNDPercentage"]) / 100;
                                        RestSPLOTNDPercent = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeNDPercentage"]) / 100;
                                    }
                                    else
                                        throw new PayrollException(PremiumGroup + " premium group has no RESTSPL premium.");
                                }

                                drDayCodePremium = dtDayCodePremiums.Select(string.Format("Dpm_PremiumGroup = '{0}' AND Dpm_Daycode = '{1}' AND Dpm_RestDay = {2} AND Dpm_PayrollType = '{3}'", PremiumGroup, "COMP", true, PayrollType));
                                if (drDayCodePremium.Length > 0)
                                {
                                    RestComp = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularHourPremium"]) / 100;
                                    RestCompOT = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeHourPremium"]) / 100;
                                    RestCompND = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularNDHourPremium"]) / 100;
                                    RestCompOTND = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeNDHourPremium"]) / 100;
                                    RestCompNDPercent = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularNDPercentage"]) / 100;
                                    RestCompOTNDPercent = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeNDPercentage"]) / 100;
                                }
                                else
                                {
                                    drDayCodePremium = dtDayCodePremiums.Select(string.Format("Dpm_PremiumGroup = '{0}' AND Dpm_Daycode = '{1}' AND Dpm_RestDay = {2} AND Dpm_PayrollType = '{3}'", "DEFAULTGRP", "COMP", true, PayrollType));
                                    if (drDayCodePremium.Length > 0)
                                    {
                                        RestComp = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularHourPremium"]) / 100;
                                        RestCompOT = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeHourPremium"]) / 100;
                                        RestCompND = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularNDHourPremium"]) / 100;
                                        RestCompOTND = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeNDHourPremium"]) / 100;
                                        RestCompNDPercent = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularNDPercentage"]) / 100;
                                        RestCompOTNDPercent = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeNDPercentage"]) / 100;
                                    }
                                    else
                                        throw new PayrollException(PremiumGroup + " premium group has no RESTCOMP premium.");
                                }

                                drDayCodePremium = dtDayCodePremiums.Select(string.Format("Dpm_PremiumGroup = '{0}' AND Dpm_Daycode = '{1}' AND Dpm_RestDay = {2} AND Dpm_PayrollType = '{3}'", PremiumGroup, "PSD", true, PayrollType));
                                if (drDayCodePremium.Length > 0)
                                {
                                    RestPSD = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularHourPremium"]) / 100;
                                    RestPSDOT = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeHourPremium"]) / 100;
                                    RestPSDND = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularNDHourPremium"]) / 100;
                                    RestPSDOTND = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeNDHourPremium"]) / 100;
                                    RestPSDNDPercent = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularNDPercentage"]) / 100;
                                    RestPSDOTNDPercent = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeNDPercentage"]) / 100;
                                }
                                else
                                {
                                    drDayCodePremium = dtDayCodePremiums.Select(string.Format("Dpm_PremiumGroup = '{0}' AND Dpm_Daycode = '{1}' AND Dpm_RestDay = {2} AND Dpm_PayrollType = '{3}'", "DEFAULTGRP", "PSD", true, PayrollType));
                                    if (drDayCodePremium.Length > 0)
                                    {
                                        RestPSD = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularHourPremium"]) / 100;
                                        RestPSDOT = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeHourPremium"]) / 100;
                                        RestPSDND = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularNDHourPremium"]) / 100;
                                        RestPSDOTND = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeNDHourPremium"]) / 100;
                                        RestPSDNDPercent = Convert.ToDecimal(drDayCodePremium[0]["Dpm_RegularNDPercentage"]) / 100;
                                        RestPSDOTNDPercent = Convert.ToDecimal(drDayCodePremium[0]["Dpm_OvertimeNDPercentage"]) / 100;
                                    }
                                    else
                                        throw new PayrollException(PremiumGroup + " premium group has no RESTPSD premium.");
                                }
                            }
                            else
                                throw new PayrollException("Error loading day premium table for " + strEmployeeID);
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

                            #region Compute Adjustment Amount
                            #region OT Adjustment Amount Details
                            drLaborHrsAdjustmentDetail["Lhd_RegularOTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Lha_RegularOTHr"]) * HourlyRate * RegOT;
                            drLaborHrsAdjustmentDetail["Lhd_RegularNDAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Lha_RegularNDHr"]) * HourlyRate * RegND * RegNDPercent;
                            drLaborHrsAdjustmentDetail["Lhd_RegularOTNDAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Lha_RegularOTNDHr"]) * HourlyRate * RegOTND * RegOTNDPercent;

                            drLaborHrsAdjustmentDetail["Lhd_RestdayAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Lha_RestdayHr"]) * HourlyRate * Rest;
                            drLaborHrsAdjustmentDetail["Lhd_RestdayOTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Lha_RestdayOTHr"]) * HourlyRate * RestOT;
                            drLaborHrsAdjustmentDetail["Lhd_RestdayNDAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Lha_RestdayNDHr"]) * HourlyRate * RestND * RestNDPercent;
                            drLaborHrsAdjustmentDetail["Lhd_RestdayOTNDAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Lha_RestdayOTNDHr"]) * HourlyRate * RestOTND * RestOTNDPercent;

                            drLaborHrsAdjustmentDetail["Lhd_LegalHolidayAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Lha_LegalHolidayHr"]) * HourlyRate * (Hol);
                            drLaborHrsAdjustmentDetail["Lhd_LegalHolidayOTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Lha_LegalHolidayOTHr"]) * HourlyRate * HolOT;
                            drLaborHrsAdjustmentDetail["Lhd_LegalHolidayNDAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Lha_LegalHolidayNDHr"]) * HourlyRate * (HolND) * HolNDPercent;
                            drLaborHrsAdjustmentDetail["Lhd_LegalHolidayOTNDAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Lha_LegalHolidayOTNDHr"]) * HourlyRate * HolOTND * HolOTNDPercent;

                            drLaborHrsAdjustmentDetail["Lhd_SpecialHolidayAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Lha_SpecialHolidayHr"]) * HourlyRate * (SPL);
                            drLaborHrsAdjustmentDetail["Lhd_SpecialHolidayOTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Lha_SpecialHolidayOTHr"]) * HourlyRate * SPLOT;
                            drLaborHrsAdjustmentDetail["Lhd_SpecialHolidayNDAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Lha_SpecialHolidayNDHr"]) * HourlyRate * (SPLND) * SPLNDPercent;
                            drLaborHrsAdjustmentDetail["Lhd_SpecialHolidayOTNDAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Lha_SpecialHolidayOTNDHr"]) * HourlyRate * SPLOTND * SPLOTNDPercent;

                            drLaborHrsAdjustmentDetail["Lhd_PlantShutdownAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Lha_PlantShutdownHr"]) * HourlyRate * (PSD);
                            drLaborHrsAdjustmentDetail["Lhd_PlantShutdownOTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Lha_PlantShutdownOTHr"]) * HourlyRate * PSDOT;
                            drLaborHrsAdjustmentDetail["Lhd_PlantShutdownNDAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Lha_PlantShutdownNDHr"]) * HourlyRate * (PSDND) * PSDNDPercent;
                            drLaborHrsAdjustmentDetail["Lhd_PlantShutdownOTNDAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Lha_PlantShutdownOTNDHr"]) * HourlyRate * PSDOTND * PSDOTNDPercent;

                            drLaborHrsAdjustmentDetail["Lhd_CompanyHolidayAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Lha_CompanyHolidayHr"]) * HourlyRate * (Comp);
                            drLaborHrsAdjustmentDetail["Lhd_CompanyHolidayOTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Lha_CompanyHolidayOTHr"]) * HourlyRate * CompOT;
                            drLaborHrsAdjustmentDetail["Lhd_CompanyHolidayNDAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Lha_CompanyHolidayNDHr"]) * HourlyRate * (CompND) * CompNDPercent;
                            drLaborHrsAdjustmentDetail["Lhd_CompanyHolidayOTNDAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Lha_CompanyHolidayOTNDHr"]) * HourlyRate * CompOTND * CompOTNDPercent;

                            drLaborHrsAdjustmentDetail["Lhd_RestdayLegalHolidayAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Lha_RestdayLegalHolidayHr"]) * HourlyRate * RestHol;
                            drLaborHrsAdjustmentDetail["Lhd_RestdayLegalHolidayOTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Lha_RestdayLegalHolidayOTHr"]) * HourlyRate * RestHolOT;
                            drLaborHrsAdjustmentDetail["Lhd_RestdayLegalHolidayNDAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Lha_RestdayLegalHolidayNDHr"]) * HourlyRate * RestHolND * RestHolNDPercent;
                            drLaborHrsAdjustmentDetail["Lhd_RestdayLegalHolidayOTNDAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Lha_RestdayLegalHolidayOTNDHr"]) * HourlyRate * RestHolOTND * RestHolOTNDPercent;

                            drLaborHrsAdjustmentDetail["Lhd_RestdaySpecialHolidayAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Lha_RestdaySpecialHolidayHr"]) * HourlyRate * RestSPL;
                            drLaborHrsAdjustmentDetail["Lhd_RestdaySpecialHolidayOTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Lha_RestdaySpecialHolidayOTHr"]) * HourlyRate * RestSPLOT;
                            drLaborHrsAdjustmentDetail["Lhd_RestdaySpecialHolidayNDAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Lha_RestdaySpecialHolidayNDHr"]) * HourlyRate * RestSPLND * RestSPLNDPercent;
                            drLaborHrsAdjustmentDetail["Lhd_RestdaySpecialHolidayOTNDAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Lha_RestdaySpecialHolidayOTNDHr"]) * HourlyRate * RestSPLOTND * RestSPLOTNDPercent;

                            drLaborHrsAdjustmentDetail["Lhd_RestdayCompanyHolidayAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Lha_RestdayCompanyHolidayHr"]) * HourlyRate * RestComp;
                            drLaborHrsAdjustmentDetail["Lhd_RestdayCompanyHolidayOTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Lha_RestdayCompanyHolidayOTHr"]) * HourlyRate * RestCompOT;
                            drLaborHrsAdjustmentDetail["Lhd_RestdayCompanyHolidayNDAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Lha_RestdayCompanyHolidayNDHr"]) * HourlyRate * RestCompND * RestCompNDPercent;
                            drLaborHrsAdjustmentDetail["Lhd_RestdayCompanyHolidayOTNDAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Lha_RestdayCompanyHolidayOTNDHr"]) * HourlyRate * RestCompOTND * RestCompOTNDPercent;

                            drLaborHrsAdjustmentDetail["Lhd_RestdayPlantShutdownAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Lha_RestdayPlantShutdownHr"]) * HourlyRate * RestPSD;
                            drLaborHrsAdjustmentDetail["Lhd_RestdayPlantShutdownOTAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Lha_RestdayPlantShutdownOTHr"]) * HourlyRate * RestPSDOT;
                            drLaborHrsAdjustmentDetail["Lhd_RestdayPlantShutdownNDAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Lha_RestdayPlantShutdownNDHr"]) * HourlyRate * RestPSDND * RestPSDNDPercent;
                            drLaborHrsAdjustmentDetail["Lhd_RestdayPlantShutdownOTNDAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Lha_RestdayPlantShutdownOTNDHr"]) * HourlyRate * RestPSDOTND * RestPSDOTNDPercent;

                            if (bHasDayCodeExt)
                            {
                                drDayCodePremiumFiller = dtDayCodePremiumFillers.Select(string.Format("Dpm_PremiumGroup = '{0}' AND Dpm_PayrollType = '{1}'", PremiumGroup, PayrollType));
                                for (int premFill = 0; premFill < drDayCodePremiumFiller.Length; premFill++)
                                {
                                    fillerHrCol = string.Format("Lha_Filler{0:00}_Hr", drDayCodePremiumFiller[premFill]["Dcf_FillerSeq"]);
                                    fillerOTHrCol = string.Format("Lha_Filler{0:00}_OTHr", drDayCodePremiumFiller[premFill]["Dcf_FillerSeq"]);
                                    fillerNDHrCol = string.Format("Lha_Filler{0:00}_NDHr", drDayCodePremiumFiller[premFill]["Dcf_FillerSeq"]);
                                    fillerOTNDHrCol = string.Format("Lha_Filler{0:00}_OTNDHr", drDayCodePremiumFiller[premFill]["Dcf_FillerSeq"]);
                                    fillerAmtCol = string.Format("Lhd_Filler{0:00}_Amt", drDayCodePremiumFiller[premFill]["Dcf_FillerSeq"]);
                                    fillerOTAmtCol = string.Format("Lhd_Filler{0:00}_OTAmt", drDayCodePremiumFiller[premFill]["Dcf_FillerSeq"]);
                                    fillerNDAmtCol = string.Format("Lhd_Filler{0:00}_NDAmt", drDayCodePremiumFiller[premFill]["Dcf_FillerSeq"]);
                                    fillerOTNDAmtCol = string.Format("Lhd_Filler{0:00}_OTNDAmt", drDayCodePremiumFiller[premFill]["Dcf_FillerSeq"]);

                                    drLaborHrsAdjustmentExtDetail[fillerAmtCol] = Convert.ToDecimal(drLaborHrsAdjustmentExt[fillerHrCol]) * HourlyRate * ((Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Dpm_RegularHourPremium"]) / 100));
                                    drLaborHrsAdjustmentExtDetail[fillerOTAmtCol] = Convert.ToDecimal(drLaborHrsAdjustmentExt[fillerOTHrCol]) * HourlyRate * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Dpm_OvertimeHourPremium"]) / 100);
                                    drLaborHrsAdjustmentExtDetail[fillerNDAmtCol] = Convert.ToDecimal(drLaborHrsAdjustmentExt[fillerNDHrCol]) * HourlyRate * ((Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Dpm_RegularNDHourPremium"]) / 100)) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Dpm_RegularNDPercentage"]) / 100);
                                    drLaborHrsAdjustmentExtDetail[fillerOTNDAmtCol] = Convert.ToDecimal(drLaborHrsAdjustmentExt[fillerOTNDHrCol]) * HourlyRate * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Dpm_OvertimeNDHourPremium"]) / 100) * (Convert.ToDecimal(drDayCodePremiumFiller[premFill]["Dpm_OvertimeNDPercentage"]) / 100);
                                }
                            }
                            #endregion

                            #region Regular Adjustment Amount Details
                            //Get Shift Hours for Hist and Trail
                            dShiftHoursHist = Math.Round(Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_RegularHr"])
                                                + Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_PaidLeaveHr"])
                                                + Convert.ToDouble(drEmployeePayrollTransactionHist[0]["Ept_AbsentHr"]), 2);
                            dShiftHoursTrail = Math.Round(Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_RegularHr"])
                                                + Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_PaidLeaveHr"])
                                                + Convert.ToDouble(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_AbsentHr"]), 2);

                            //Override REG -> REST for Monthlies with negative hour adjustment
                            //Override REST -> REG for Monthlies
                            //Override if Absent = 0
                            //Override if there is Difference of Shift Hours
                            if ((dtEmployeePayrollTransactionTrail.Rows[i]["Ell_Restday"].ToString() == "False"
                                    && dtEmployeePayrollTransactionTrail.Rows[i]["Ell_Holiday"].ToString() == "False"
                                    && drEmployeePayrollTransactionHist[0]["Ell_Restday"].ToString() == "True"
                                    && drEmployeePayrollTransactionHist[0]["Ell_Holiday"].ToString() == "False"
                                    && PayrollType == "M"
                                    && Convert.ToDecimal(drLaborHrsAdjustment["Lha_RegularHr"]) < 0)
                                || (dtEmployeePayrollTransactionTrail.Rows[i]["Ell_Restday"].ToString() == "True"
                                    && dtEmployeePayrollTransactionTrail.Rows[i]["Ell_Holiday"].ToString() == "False"
                                    && drEmployeePayrollTransactionHist[0]["Ell_Restday"].ToString() == "False"
                                    && drEmployeePayrollTransactionHist[0]["Ell_Holiday"].ToString() == "False"
                                    && PayrollType == "M")
                                || (dtEmployeePayrollTransactionTrail.Rows[i]["Ell_Restday"].ToString() == "False"
                                    && dtEmployeePayrollTransactionTrail.Rows[i]["Ell_Holiday"].ToString() == "False"
                                    && drEmployeePayrollTransactionHist[0]["Ell_Restday"].ToString() == "False"
                                    && drEmployeePayrollTransactionHist[0]["Ell_Holiday"].ToString() == "False"
                                    && PayrollType == "M"
                                    && ((Convert.ToDecimal(drLaborHrsAdjustment["Lha_RegularHr"]) < 0 && Convert.ToDecimal(drLaborHrsAdjustment["Lha_AbsentHr"]) == 0)
                                        || dShiftHoursHist != dShiftHoursTrail))
                                )
                            {
                                drLaborHrsAdjustment["Lha_RegularHr"] = 0; //NO ADJUSTMENT

                                drLaborHrsAdjustmentDetail["Lhd_RegularHr"] = 0;
                                drLaborHrsAdjustmentDetail["Lhd_PaidLeaveHr"] = 0;
                                drLaborHrsAdjustmentDetail["Lhd_TotalRegularHr"] = 0;
                            }

                            if (Convert.ToBoolean(HRRTINCDEM) == true && PayrollType == "D") //Lear
                            {
                                drLaborHrsAdjustmentDetail["Lhd_RegularAmt"] = Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RegularHr"]) * SalaryRate / 8;
                                drLaborHrsAdjustmentDetail["Lhd_PaidLeaveAmt"] = Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_PaidLeaveHr"]) * SalaryRate / 8;
                            }
                            else
                            {
                                drLaborHrsAdjustmentDetail["Lhd_RegularAmt"] = Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RegularHr"]) * HourlyRate;
                                drLaborHrsAdjustmentDetail["Lhd_PaidLeaveAmt"] = Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_PaidLeaveHr"]) * HourlyRate;
                            }
                            #endregion

                            #region Holiday and Absent Adjustment Amount Details
                            if (PayrollType != "M") //Only Dailies will be ADDED by Unwork Holidays
                            {
                                if (drEmployeePayrollTransactionHist[0]["Ell_Restday"].ToString() == "False")
                                {
                                    drLaborHrsAdjustmentDetail["Lhd_PaidLegalHolidayAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Lha_PaidLegalHolidayHr"]) * HourlyRate;
                                }
                                else
                                {
                                    drLaborHrsAdjustment["Lha_PaidLegalHolidayHr"] = 0; //NO ADJUSTMENT
                                    drLaborHrsAdjustmentDetail["Lhd_PaidLegalHolidayAmt"] = 0;
                                }
                                
                                drLaborHrsAdjustmentDetail["Lhd_PaidSpecialHolidayAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Lha_PaidSpecialHolidayHr"]) * HourlyRate;
                                drLaborHrsAdjustmentDetail["Lhd_PaidCompanyHolidayAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Lha_PaidCompanyHolidayHr"]) * HourlyRate;
                                drLaborHrsAdjustmentDetail["Lhd_PaidFillerHolidayAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Lha_PaidFillerHolidayHr"]) * HourlyRate;
                            }

                            if (PayrollType == "M") //Only Monthlies will be DEDUCTED by Absent Amount
                            {
                                if (Convert.ToDecimal(drLaborHrsAdjustment["Lha_RegularHr"]) == 0)
                                    drLaborHrsAdjustmentDetail["Lhd_AbsentAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Lha_AbsentHr"]) * HourlyRate;

                                if (Convert.ToDecimal(drLaborHrsAdjustment["Lha_PaidLegalHolidayHr"]) <= 0
                                    || Convert.ToDecimal(drLaborHrsAdjustment["Lha_AbsentLegalHolidayHr"]) <= 0)
                                    drLaborHrsAdjustmentDetail["Lhd_AbsentLegalHolidayAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Lha_AbsentLegalHolidayHr"]) * HourlyRate;

                                if (Convert.ToDecimal(drLaborHrsAdjustment["Lha_PaidSpecialHolidayHr"]) <= 0
                                    || Convert.ToDecimal(drLaborHrsAdjustment["Lha_AbsentSpecialHolidayHr"]) <= 0)
                                    drLaborHrsAdjustmentDetail["Lhd_AbsentSpecialHolidayAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Lha_AbsentSpecialHolidayHr"]) * HourlyRate;

                                if (Convert.ToDecimal(drLaborHrsAdjustment["Lha_PaidCompanyHolidayHr"]) <= 0
                                    || Convert.ToDecimal(drLaborHrsAdjustment["Lha_AbsentCompanyHolidayHr"]) <= 0)
                                    drLaborHrsAdjustmentDetail["Lhd_AbsentCompanyHolidayAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Lha_AbsentCompanyHolidayHr"]) * HourlyRate;

                                drLaborHrsAdjustmentDetail["Lhd_AbsentPlantShutdownAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Lha_AbsentPlantShutdownHr"]) * HourlyRate;

                                if (Convert.ToDecimal(drLaborHrsAdjustment["Lha_PaidFillerHolidayHr"]) <= 0
                                    || Convert.ToDecimal(drLaborHrsAdjustment["Lha_AbsentFillerHolidayHr"]) <= 0)
                                    drLaborHrsAdjustmentDetail["Lhd_AbsentFillerHolidayAmt"] = Convert.ToDecimal(drLaborHrsAdjustment["Lha_AbsentFillerHolidayHr"]) * HourlyRate;
                            }

                            //Add "Restday falling on a Holiday" Adjustment to Holiday Amount
                            if (Convert.ToDecimal(drEmployeePayrollTransactionHist[0]["Ept_RestdayLegalHolidayCount"]) 
                                - Convert.ToDecimal(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_RestdayLegalHolidayCount"]) != 0)
                            {
                                drLaborHrsAdjustmentDetail["Lhd_RestdayLegalHolidayCount"] = Convert.ToDecimal(drEmployeePayrollTransactionHist[0]["Ept_RestdayLegalHolidayCount"]) - Convert.ToDecimal(dtEmployeePayrollTransactionTrail.Rows[i]["Ept_RestdayLegalHolidayCount"]);
                                drLaborHrsAdjustmentDetail["Lhd_HolidayAdjustmentAmt"] = (Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RestdayLegalHolidayCount"]) * HourlyRate * 8);
                            }

                            //Other Absent Amount Details
                            drLaborHrsAdjustmentDetail["Lhd_LateAmt"] = Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_LateHr"]) * HourlyRate;
                            drLaborHrsAdjustmentDetail["Lhd_UndertimeAmt"] = Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_UndertimeHr"]) * HourlyRate;
                            drLaborHrsAdjustmentDetail["Lhd_WholeDayAbsentAmt"] = Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_WholeDayAbsentHr"]) * HourlyRate;
                            drLaborHrsAdjustmentDetail["Lhd_UnpaidLeaveAmt"] = Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_UnpaidLeaveHr"]) * HourlyRate;
                            #endregion
                            #endregion

                            #region Rounding Off of Amounts
                            drLaborHrsAdjustmentDetail["Lhd_RegularAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RegularAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_RegularOTAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RegularOTAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_RegularNDAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RegularNDAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_RegularOTNDAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RegularOTNDAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_RestdayAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RestdayAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_RestdayOTAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RestdayOTAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_RestdayNDAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RestdayNDAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_RestdayOTNDAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RestdayOTNDAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_LegalHolidayAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_LegalHolidayAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_LegalHolidayOTAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_LegalHolidayOTAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_LegalHolidayNDAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_LegalHolidayNDAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_LegalHolidayOTNDAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_LegalHolidayOTNDAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_SpecialHolidayAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_SpecialHolidayAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_SpecialHolidayOTAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_SpecialHolidayOTAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_SpecialHolidayNDAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_SpecialHolidayNDAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_SpecialHolidayOTNDAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_SpecialHolidayOTNDAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_PlantShutdownAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_PlantShutdownAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_PlantShutdownOTAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_PlantShutdownOTAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_PlantShutdownNDAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_PlantShutdownNDAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_PlantShutdownOTNDAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_PlantShutdownOTNDAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_CompanyHolidayAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_CompanyHolidayAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_CompanyHolidayOTAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_CompanyHolidayOTAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_CompanyHolidayNDAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_CompanyHolidayNDAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_CompanyHolidayOTNDAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_CompanyHolidayOTNDAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_RestdayLegalHolidayAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RestdayLegalHolidayAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_RestdayLegalHolidayOTAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RestdayLegalHolidayOTAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_RestdayLegalHolidayNDAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RestdayLegalHolidayNDAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_RestdayLegalHolidayOTNDAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RestdayLegalHolidayOTNDAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_RestdaySpecialHolidayAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RestdaySpecialHolidayAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_RestdaySpecialHolidayOTAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RestdaySpecialHolidayOTAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_RestdaySpecialHolidayNDAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RestdaySpecialHolidayNDAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_RestdaySpecialHolidayOTNDAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RestdaySpecialHolidayOTNDAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_RestdayCompanyHolidayAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RestdayCompanyHolidayAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_RestdayCompanyHolidayOTAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RestdayCompanyHolidayOTAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_RestdayCompanyHolidayNDAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RestdayCompanyHolidayNDAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_RestdayCompanyHolidayOTNDAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RestdayCompanyHolidayOTNDAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_RestdayPlantShutdownAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RestdayPlantShutdownAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_RestdayPlantShutdownOTAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RestdayPlantShutdownOTAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_RestdayPlantShutdownNDAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RestdayPlantShutdownNDAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_RestdayPlantShutdownOTNDAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RestdayPlantShutdownOTNDAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_AbsentAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_AbsentAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_LateAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_LateAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_UndertimeAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_UndertimeAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_UnpaidLeaveAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_UnpaidLeaveAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_AbsentLegalHolidayAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_AbsentLegalHolidayAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_AbsentSpecialHolidayAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_AbsentSpecialHolidayAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_AbsentCompanyHolidayAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_AbsentCompanyHolidayAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_AbsentPlantShutdownAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_AbsentPlantShutdownAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_AbsentFillerHolidayAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_AbsentFillerHolidayAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_WholeDayAbsentAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_WholeDayAbsentAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_PaidLeaveAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_PaidLeaveAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_PaidLegalHolidayAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_PaidLegalHolidayAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_PaidSpecialHolidayAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_PaidSpecialHolidayAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_PaidCompanyHolidayAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_PaidCompanyHolidayAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_PaidFillerHolidayAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_PaidFillerHolidayAmt"]), 2);

                            if (bHasDayCodeExt)
                            {
                                drLaborHrsAdjustmentExtDetail["Lhd_Filler01_Amt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentExtDetail["Lhd_Filler01_Amt"]), 2);
                                drLaborHrsAdjustmentExtDetail["Lhd_Filler01_OTAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentExtDetail["Lhd_Filler01_OTAmt"]), 2);
                                drLaborHrsAdjustmentExtDetail["Lhd_Filler01_NDAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentExtDetail["Lhd_Filler01_NDAmt"]), 2);
                                drLaborHrsAdjustmentExtDetail["Lhd_Filler01_OTNDAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentExtDetail["Lhd_Filler01_OTNDAmt"]), 2);
                                drLaborHrsAdjustmentExtDetail["Lhd_Filler02_Amt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentExtDetail["Lhd_Filler02_Amt"]), 2);
                                drLaborHrsAdjustmentExtDetail["Lhd_Filler02_OTAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentExtDetail["Lhd_Filler02_OTAmt"]), 2);
                                drLaborHrsAdjustmentExtDetail["Lhd_Filler02_NDAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentExtDetail["Lhd_Filler02_NDAmt"]), 2);
                                drLaborHrsAdjustmentExtDetail["Lhd_Filler02_OTNDAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentExtDetail["Lhd_Filler02_OTNDAmt"]), 2);
                                drLaborHrsAdjustmentExtDetail["Lhd_Filler03_Amt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentExtDetail["Lhd_Filler03_Amt"]), 2);
                                drLaborHrsAdjustmentExtDetail["Lhd_Filler03_OTAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentExtDetail["Lhd_Filler03_OTAmt"]), 2);
                                drLaborHrsAdjustmentExtDetail["Lhd_Filler03_NDAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentExtDetail["Lhd_Filler03_NDAmt"]), 2);
                                drLaborHrsAdjustmentExtDetail["Lhd_Filler03_OTNDAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentExtDetail["Lhd_Filler03_OTNDAmt"]), 2);
                                drLaborHrsAdjustmentExtDetail["Lhd_Filler04_Amt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentExtDetail["Lhd_Filler04_Amt"]), 2);
                                drLaborHrsAdjustmentExtDetail["Lhd_Filler04_OTAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentExtDetail["Lhd_Filler04_OTAmt"]), 2);
                                drLaborHrsAdjustmentExtDetail["Lhd_Filler04_NDAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentExtDetail["Lhd_Filler04_NDAmt"]), 2);
                                drLaborHrsAdjustmentExtDetail["Lhd_Filler04_OTNDAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentExtDetail["Lhd_Filler04_OTNDAmt"]), 2);
                                drLaborHrsAdjustmentExtDetail["Lhd_Filler05_Amt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentExtDetail["Lhd_Filler05_Amt"]), 2);
                                drLaborHrsAdjustmentExtDetail["Lhd_Filler05_OTAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentExtDetail["Lhd_Filler05_OTAmt"]), 2);
                                drLaborHrsAdjustmentExtDetail["Lhd_Filler05_NDAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentExtDetail["Lhd_Filler05_NDAmt"]), 2);
                                drLaborHrsAdjustmentExtDetail["Lhd_Filler05_OTNDAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentExtDetail["Lhd_Filler05_OTNDAmt"]), 2);
                                drLaborHrsAdjustmentExtDetail["Lhd_Filler06_Amt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentExtDetail["Lhd_Filler06_Amt"]), 2);
                                drLaborHrsAdjustmentExtDetail["Lhd_Filler06_OTAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentExtDetail["Lhd_Filler06_OTAmt"]), 2);
                                drLaborHrsAdjustmentExtDetail["Lhd_Filler06_NDAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentExtDetail["Lhd_Filler06_NDAmt"]), 2);
                                drLaborHrsAdjustmentExtDetail["Lhd_Filler06_OTNDAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentExtDetail["Lhd_Filler06_OTNDAmt"]), 2);
                            }
                            #endregion

                            #region Labor Hours Adjustment Computation
                            drLaborHrsAdjustmentDetail["Lhd_OvertimeAdjustmentAmt"] = Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RegularOTAmt"])
                                                                                        + Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RestdayAmt"])
                                                                                        + Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RestdayOTAmt"])
                                                                                        + Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_LegalHolidayAmt"])
                                                                                        + Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_LegalHolidayOTAmt"])
                                                                                        + Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_SpecialHolidayAmt"])
                                                                                        + Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_SpecialHolidayOTAmt"])
                                                                                        + Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_PlantShutdownAmt"])
                                                                                        + Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_PlantShutdownOTAmt"])
                                                                                        + Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_CompanyHolidayAmt"])
                                                                                        + Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_CompanyHolidayOTAmt"])
                                                                                        + Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RestdayLegalHolidayAmt"])
                                                                                        + Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RestdayLegalHolidayOTAmt"])
                                                                                        + Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RestdaySpecialHolidayAmt"])
                                                                                        + Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RestdaySpecialHolidayOTAmt"])
                                                                                        + Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RestdayCompanyHolidayAmt"])
                                                                                        + Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RestdayCompanyHolidayOTAmt"])
                                                                                        + Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RestdayPlantShutdownAmt"])
                                                                                        + Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RestdayPlantShutdownOTAmt"]);

                            drLaborHrsAdjustmentDetail["Lhd_RegularAdjustmentAmt"] = Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RegularAmt"])
                                                                                        - Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_AbsentAmt"]);

                            drLaborHrsAdjustmentDetail["Lhd_HolidayAdjustmentAmt"] = Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_HolidayAdjustmentAmt"])
                                                                                        + Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_PaidLegalHolidayAmt"])
                                                                                        + Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_PaidSpecialHolidayAmt"])
                                                                                        + Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_PaidCompanyHolidayAmt"])
                                                                                        + Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_PaidFillerHolidayAmt"])
                                                                                        - Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_AbsentLegalHolidayAmt"])
                                                                                        - Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_AbsentSpecialHolidayAmt"])
                                                                                        - Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_AbsentCompanyHolidayAmt"])
                                                                                        - Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_AbsentPlantShutdownAmt"])
                                                                                        - Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_AbsentFillerHolidayAmt"]);

                            drLaborHrsAdjustmentDetail["Lhd_NightPremiumAdjustmentAmt"] = Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RegularNDAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RegularOTNDAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RestdayNDAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RestdayOTNDAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_LegalHolidayNDAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_LegalHolidayOTNDAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_SpecialHolidayNDAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_SpecialHolidayOTNDAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_PlantShutdownNDAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_PlantShutdownOTNDAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_CompanyHolidayNDAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_CompanyHolidayOTNDAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RestdayLegalHolidayNDAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RestdayLegalHolidayOTNDAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RestdaySpecialHolidayNDAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RestdaySpecialHolidayOTNDAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RestdayCompanyHolidayNDAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RestdayCompanyHolidayOTNDAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RestdayPlantShutdownNDAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RestdayPlantShutdownOTNDAmt"]);

                            drLaborHrsAdjustmentDetail["Lhd_LeaveAdjustmentAmt"] = Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_PaidLeaveAmt"]);

                            if (bHasDayCodeExt)
                            {
                                drLaborHrsAdjustmentDetail["Lhd_OvertimeAdjustmentAmt"] = Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_OvertimeAdjustmentAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentExtDetail["Lhd_Filler01_Amt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentExtDetail["Lhd_Filler01_OTAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentExtDetail["Lhd_Filler02_Amt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentExtDetail["Lhd_Filler02_OTAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentExtDetail["Lhd_Filler03_Amt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentExtDetail["Lhd_Filler03_OTAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentExtDetail["Lhd_Filler04_Amt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentExtDetail["Lhd_Filler04_OTAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentExtDetail["Lhd_Filler05_Amt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentExtDetail["Lhd_Filler05_OTAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentExtDetail["Lhd_Filler06_Amt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentExtDetail["Lhd_Filler06_OTAmt"]);

                                drLaborHrsAdjustmentDetail["Lhd_NightPremiumAdjustmentAmt"] = Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_NightPremiumAdjustmentAmt"])
                                                                                                + Convert.ToDecimal(drLaborHrsAdjustmentExtDetail["Lhd_Filler01_NDAmt"])
                                                                                                + Convert.ToDecimal(drLaborHrsAdjustmentExtDetail["Lhd_Filler01_OTNDAmt"])
                                                                                                + Convert.ToDecimal(drLaborHrsAdjustmentExtDetail["Lhd_Filler02_NDAmt"])
                                                                                                + Convert.ToDecimal(drLaborHrsAdjustmentExtDetail["Lhd_Filler02_OTNDAmt"])
                                                                                                + Convert.ToDecimal(drLaborHrsAdjustmentExtDetail["Lhd_Filler03_NDAmt"])
                                                                                                + Convert.ToDecimal(drLaborHrsAdjustmentExtDetail["Lhd_Filler03_OTNDAmt"])
                                                                                                + Convert.ToDecimal(drLaborHrsAdjustmentExtDetail["Lhd_Filler04_NDAmt"])
                                                                                                + Convert.ToDecimal(drLaborHrsAdjustmentExtDetail["Lhd_Filler04_OTNDAmt"])
                                                                                                + Convert.ToDecimal(drLaborHrsAdjustmentExtDetail["Lhd_Filler05_NDAmt"])
                                                                                                + Convert.ToDecimal(drLaborHrsAdjustmentExtDetail["Lhd_Filler05_OTNDAmt"])
                                                                                                + Convert.ToDecimal(drLaborHrsAdjustmentExtDetail["Lhd_Filler06_NDAmt"])
                                                                                                + Convert.ToDecimal(drLaborHrsAdjustmentExtDetail["Lhd_Filler06_OTNDAmt"]);
                            }
                            #endregion

                            #region Rounding Off of Total Amounts
                            drLaborHrsAdjustmentDetail["Lhd_RegularAdjustmentAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RegularAdjustmentAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_OvertimeAdjustmentAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_OvertimeAdjustmentAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_HolidayAdjustmentAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_HolidayAdjustmentAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_NightPremiumAdjustmentAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_NightPremiumAdjustmentAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_LeaveAdjustmentAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_LeaveAdjustmentAmt"]), 2);
                            drLaborHrsAdjustmentDetail["Lhd_LaborHrsAdjustmentAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RegularAdjustmentAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_OvertimeAdjustmentAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_HolidayAdjustmentAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_NightPremiumAdjustmentAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_LeaveAdjustmentAmt"]), 2);
                            #endregion

                            #region Add to Adjustment Table
                            drLaborHrsAdjustment["Lha_LaborHrsAdjustmentAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_RegularAdjustmentAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_OvertimeAdjustmentAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_HolidayAdjustmentAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_NightPremiumAdjustmentAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_LeaveAdjustmentAmt"]), 2);
                            drLaborHrsAdjustment["Lha_OvertimeAdjustmentAmt"] = Math.Round(Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_OvertimeAdjustmentAmt"])
                                                                                            //+ Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_HolidayAdjustmentAmt"])
                                                                                            + Convert.ToDecimal(drLaborHrsAdjustmentDetail["Lhd_NightPremiumAdjustmentAmt"]), 2);

                            if (Convert.ToDecimal(drLaborHrsAdjustment["Lha_LaborHrsAdjustmentAmt"]) != 0)
                            {
                                dtLaborHrsAdjustment.Rows.Add(drLaborHrsAdjustment);
                                dtLaborHrsAdjustmentDetail.Rows.Add(drLaborHrsAdjustmentDetail);
                                if (bHasDayCodeExt)
                                {
                                    dtLaborHrsAdjustmentExt.Rows.Add(drLaborHrsAdjustmentExt);
                                    dtLaborHrsAdjustmentExtDetail.Rows.Add(drLaborHrsAdjustmentExtDetail);
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
                StatusHandler(this, new StatusEventArgs("Saving of Adjustment Records", false));
                string strUpdateRecordTemplate;
                string strUpdateQuery = "";
                int iUpdateCtr = 0;
                #region Adjustment Record Insert
                strUpdateRecordTemplate = @" INSERT INTO T_LaborHrsAdjustment (Lha_EmployeeId,Lha_AdjustpayPeriod,Lha_PayPeriod,Lha_ProcessDate,Lha_RegularHr,Lha_RegularOTHr,Lha_RegularNDHr,Lha_RegularOTNDHr,Lha_RestdayHr,Lha_RestdayOTHr,Lha_RestdayNDHr,Lha_RestdayOTNDHr,Lha_LegalHolidayHr,Lha_LegalHolidayOTHr,Lha_LegalHolidayNDHr,Lha_LegalHolidayOTNDHr,Lha_SpecialHolidayHr,Lha_SpecialHolidayOTHr,Lha_SpecialHolidayNDHr,Lha_SpecialHolidayOTNDHr,Lha_PlantShutdownHr,Lha_PlantShutdownOTHr,Lha_PlantShutdownNDHr,Lha_PlantShutdownOTNDHr,Lha_CompanyHolidayHr,Lha_CompanyHolidayOTHr,Lha_CompanyHolidayNDHr,Lha_CompanyHolidayOTNDHr,Lha_RestdayLegalHolidayHr,Lha_RestdayLegalHolidayOTHr,Lha_RestdayLegalHolidayNDHr,Lha_RestdayLegalHolidayOTNDHr,Lha_RestdaySpecialHolidayHr,Lha_RestdaySpecialHolidayOTHr,Lha_RestdaySpecialHolidayNDHr,Lha_RestdaySpecialHolidayOTNDHr,Lha_RestdayCompanyHolidayHr,Lha_RestdayCompanyHolidayOTHr,Lha_RestdayCompanyHolidayNDHr,Lha_RestdayCompanyHolidayOTNDHr,Lha_RestdayPlantShutdownHr,Lha_RestdayPlantShutdownOTHr,Lha_RestdayPlantShutdownNDHr,Lha_RestdayPlantShutdownOTNDHr,Lha_LaborHrsAdjustmentAmt,Lha_SalaryRate,Lha_HourlyRate,Lha_PayrollPost,Usr_Login,Ludatetime,Lha_AbsentHr,Lha_AbsentLegalHolidayHr,Lha_AbsentSpecialHolidayHr,Lha_AbsentCompanyHolidayHr,Lha_AbsentPlantShutdownHr,Lha_AbsentFillerHolidayHr,Lha_PaidLegalHolidayHr,Lha_PaidSpecialHolidayHr,Lha_PaidCompanyHolidayHr,Lha_PaidFillerHolidayHr,Lha_OvertimeAdjustmentAmt) VALUES('{0}','{1}','{2}','{3}',{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34},{35},{36},{37},{38},{39},{40},{41},{42},{43},{44},{45},{46},'false','{47}',GETDATE(),{48},{49},{50},{51},{52},{53},{54},{55},{56},{57},{58}) ";
                #endregion
                foreach (DataRow drLaborHrsAdjustment in dtLaborHrsAdjustment.Rows)
                {
                    #region Adjustment Insert
                    strUpdateQuery += string.Format(strUpdateRecordTemplate
                                                    , drLaborHrsAdjustment["Lha_EmployeeId"]                    //0
                                                    , drLaborHrsAdjustment["Lha_AdjustpayPeriod"]
                                                    , drLaborHrsAdjustment["Lha_PayPeriod"]
                                                    , drLaborHrsAdjustment["Lha_ProcessDate"]
                                                    , drLaborHrsAdjustment["Lha_RegularHr"]
                                                    , drLaborHrsAdjustment["Lha_RegularOTHr"]
                                                    , drLaborHrsAdjustment["Lha_RegularNDHr"]
                                                    , drLaborHrsAdjustment["Lha_RegularOTNDHr"]
                                                    , drLaborHrsAdjustment["Lha_RestdayHr"]
                                                    , drLaborHrsAdjustment["Lha_RestdayOTHr"]
                                                    , drLaborHrsAdjustment["Lha_RestdayNDHr"]                   //10
                                                    , drLaborHrsAdjustment["Lha_RestdayOTNDHr"]
                                                    , drLaborHrsAdjustment["Lha_LegalHolidayHr"]
                                                    , drLaborHrsAdjustment["Lha_LegalHolidayOTHr"]
                                                    , drLaborHrsAdjustment["Lha_LegalHolidayNDHr"]
                                                    , drLaborHrsAdjustment["Lha_LegalHolidayOTNDHr"]
                                                    , drLaborHrsAdjustment["Lha_SpecialHolidayHr"]
                                                    , drLaborHrsAdjustment["Lha_SpecialHolidayOTHr"]
                                                    , drLaborHrsAdjustment["Lha_SpecialHolidayNDHr"]
                                                    , drLaborHrsAdjustment["Lha_SpecialHolidayOTNDHr"]
                                                    , drLaborHrsAdjustment["Lha_PlantShutdownHr"]               //20
                                                    , drLaborHrsAdjustment["Lha_PlantShutdownOTHr"]
                                                    , drLaborHrsAdjustment["Lha_PlantShutdownNDHr"]
                                                    , drLaborHrsAdjustment["Lha_PlantShutdownOTNDHr"]
                                                    , drLaborHrsAdjustment["Lha_CompanyHolidayHr"]
                                                    , drLaborHrsAdjustment["Lha_CompanyHolidayOTHr"]
                                                    , drLaborHrsAdjustment["Lha_CompanyHolidayNDHr"]
                                                    , drLaborHrsAdjustment["Lha_CompanyHolidayOTNDHr"]
                                                    , drLaborHrsAdjustment["Lha_RestdayLegalHolidayHr"]
                                                    , drLaborHrsAdjustment["Lha_RestdayLegalHolidayOTHr"]
                                                    , drLaborHrsAdjustment["Lha_RestdayLegalHolidayNDHr"]       //30
                                                    , drLaborHrsAdjustment["Lha_RestdayLegalHolidayOTNDHr"]
                                                    , drLaborHrsAdjustment["Lha_RestdaySpecialHolidayHr"]
                                                    , drLaborHrsAdjustment["Lha_RestdaySpecialHolidayOTHr"]
                                                    , drLaborHrsAdjustment["Lha_RestdaySpecialHolidayNDHr"]
                                                    , drLaborHrsAdjustment["Lha_RestdaySpecialHolidayOTNDHr"]
                                                    , drLaborHrsAdjustment["Lha_RestdayCompanyHolidayHr"]
                                                    , drLaborHrsAdjustment["Lha_RestdayCompanyHolidayOTHr"]
                                                    , drLaborHrsAdjustment["Lha_RestdayCompanyHolidayNDHr"]
                                                    , drLaborHrsAdjustment["Lha_RestdayCompanyHolidayOTNDHr"]
                                                    , drLaborHrsAdjustment["Lha_RestdayPlantShutdownHr"]        //40
                                                    , drLaborHrsAdjustment["Lha_RestdayPlantShutdownOTHr"]
                                                    , drLaborHrsAdjustment["Lha_RestdayPlantShutdownNDHr"]
                                                    , drLaborHrsAdjustment["Lha_RestdayPlantShutdownOTNDHr"]
                                                    , drLaborHrsAdjustment["Lha_LaborHrsAdjustmentAmt"]
                                                    , drLaborHrsAdjustment["Lha_SalaryRate"]
                                                    , drLaborHrsAdjustment["Lha_HourlyRate"]
                                                    , drLaborHrsAdjustment["Usr_Login"]
                                                    , drLaborHrsAdjustment["Lha_AbsentHr"]
                                                    , drLaborHrsAdjustment["Lha_AbsentLegalHolidayHr"]
                                                    , drLaborHrsAdjustment["Lha_AbsentSpecialHolidayHr"]        //50
                                                    , drLaborHrsAdjustment["Lha_AbsentCompanyHolidayHr"]
                                                    , drLaborHrsAdjustment["Lha_AbsentPlantShutdownHr"]
                                                    , drLaborHrsAdjustment["Lha_AbsentFillerHolidayHr"]
                                                    , drLaborHrsAdjustment["Lha_PaidLegalHolidayHr"]
                                                    , drLaborHrsAdjustment["Lha_PaidSpecialHolidayHr"]
                                                    , drLaborHrsAdjustment["Lha_PaidCompanyHolidayHr"]
                                                    , drLaborHrsAdjustment["Lha_PaidFillerHolidayHr"]
                                                    , drLaborHrsAdjustment["Lha_OvertimeAdjustmentAmt"]);
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
                strUpdateRecordTemplate = @" INSERT INTO T_LaborHrsAdjustmentDetail (Lhd_EmployeeId,Lhd_AdjustpayPeriod,Lhd_PayPeriod,Lhd_ProcessDate,Lhd_SalaryRate,Lhd_HourlyRate,Lhd_PayrollType,Lhd_LateHr,Lhd_UndertimeHr,Lhd_WholeDayAbsentHr,Lhd_UnpaidLeaveHr,Lhd_RegularHr,Lhd_PaidLeaveHr,Lhd_TotalRegularHr,Lhd_RegularAmt,Lhd_RegularOTAmt,Lhd_RegularNDAmt,Lhd_RegularOTNDAmt,Lhd_RestdayAmt,Lhd_RestdayOTAmt,Lhd_RestdayNDAmt,Lhd_RestdayOTNDAmt,Lhd_LegalHolidayAmt,Lhd_LegalHolidayOTAmt,Lhd_LegalHolidayNDAmt,Lhd_LegalHolidayOTNDAmt,Lhd_SpecialHolidayAmt,Lhd_SpecialHolidayOTAmt,Lhd_SpecialHolidayNDAmt,Lhd_SpecialHolidayOTNDAmt,Lhd_PlantShutdownAmt,Lhd_PlantShutdownOTAmt,Lhd_PlantShutdownNDAmt,Lhd_PlantShutdownOTNDAmt,Lhd_CompanyHolidayAmt,Lhd_CompanyHolidayOTAmt,Lhd_CompanyHolidayNDAmt,Lhd_CompanyHolidayOTNDAmt,Lhd_RestdayLegalHolidayAmt,Lhd_RestdayLegalHolidayOTAmt,Lhd_RestdayLegalHolidayNDAmt,Lhd_RestdayLegalHolidayOTNDAmt,Lhd_RestdaySpecialHolidayAmt,Lhd_RestdaySpecialHolidayOTAmt,Lhd_RestdaySpecialHolidayNDAmt,Lhd_RestdaySpecialHolidayOTNDAmt,Lhd_RestdayCompanyHolidayAmt,Lhd_RestdayCompanyHolidayOTAmt,Lhd_RestdayCompanyHolidayNDAmt,Lhd_RestdayCompanyHolidayOTNDAmt,Lhd_RestdayPlantShutdownAmt,Lhd_RestdayPlantShutdownOTAmt,Lhd_RestdayPlantShutdownNDAmt,Lhd_RestdayPlantShutdownOTNDAmt,Lhd_RestdayLegalHolidayCount,Lhd_AbsentAmt,Lhd_LateAmt,Lhd_UndertimeAmt,Lhd_WholeDayAbsentAmt,Lhd_UnpaidLeaveAmt,Lhd_AbsentLegalHolidayAmt,Lhd_AbsentSpecialHolidayAmt,Lhd_AbsentCompanyHolidayAmt,Lhd_AbsentPlantShutdownAmt,Lhd_AbsentFillerHolidayAmt,Lhd_PaidLeaveAmt,Lhd_PaidLegalHolidayAmt,Lhd_PaidSpecialHolidayAmt,Lhd_PaidCompanyHolidayAmt,Lhd_PaidFillerHolidayAmt,Lhd_RegularAdjustmentAmt,Lhd_OvertimeAdjustmentAmt,Lhd_HolidayAdjustmentAmt,Lhd_NightPremiumAdjustmentAmt,Lhd_LeaveAdjustmentAmt,Lhd_LaborHrsAdjustmentAmt,Usr_Login,Ludatetime) VALUES ('{0}','{1}','{2}','{3}',{4},{5},'{6}',{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34},{35},{36},{37},{38},{39},{40},{41},{42},{43},{44},{45},{46},{47},{48},{49},{50},{51},{52},{53},{54},{55},{56},{57},{58},{59},{60},{61},{62},{63},{64},{65},{66},{67},{68},{69},{70},{71},{72},{73},{74},{75},'{76}',GETDATE()) ";
                #endregion
                foreach (DataRow drLaborHrsAdjustmentDetail in dtLaborHrsAdjustmentDetail.Rows)
                {
                    #region Adjustment Detail Insert
                    strUpdateQuery += string.Format(strUpdateRecordTemplate
                                                    , drLaborHrsAdjustmentDetail["Lhd_EmployeeId"]                      //0
                                                    , drLaborHrsAdjustmentDetail["Lhd_AdjustpayPeriod"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_PayPeriod"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_ProcessDate"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_SalaryRate"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_HourlyRate"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_PayrollType"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_LateHr"] 
                                                    , drLaborHrsAdjustmentDetail["Lhd_UndertimeHr"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_WholeDayAbsentHr"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_UnpaidLeaveHr"]                   //10
                                                    , drLaborHrsAdjustmentDetail["Lhd_RegularHr"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_PaidLeaveHr"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_TotalRegularHr"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_RegularAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_RegularOTAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_RegularNDAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_RegularOTNDAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_RestdayAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_RestdayOTAmt"] 
                                                    , drLaborHrsAdjustmentDetail["Lhd_RestdayNDAmt"]                    //20
                                                    , drLaborHrsAdjustmentDetail["Lhd_RestdayOTNDAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_LegalHolidayAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_LegalHolidayOTAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_LegalHolidayNDAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_LegalHolidayOTNDAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_SpecialHolidayAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_SpecialHolidayOTAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_SpecialHolidayNDAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_SpecialHolidayOTNDAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_PlantShutdownAmt"]                //30
                                                    , drLaborHrsAdjustmentDetail["Lhd_PlantShutdownOTAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_PlantShutdownNDAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_PlantShutdownOTNDAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_CompanyHolidayAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_CompanyHolidayOTAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_CompanyHolidayNDAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_CompanyHolidayOTNDAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_RestdayLegalHolidayAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_RestdayLegalHolidayOTAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_RestdayLegalHolidayNDAmt"]        //40
                                                    , drLaborHrsAdjustmentDetail["Lhd_RestdayLegalHolidayOTNDAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_RestdaySpecialHolidayAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_RestdaySpecialHolidayOTAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_RestdaySpecialHolidayNDAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_RestdaySpecialHolidayOTNDAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_RestdayCompanyHolidayAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_RestdayCompanyHolidayOTAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_RestdayCompanyHolidayNDAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_RestdayCompanyHolidayOTNDAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_RestdayPlantShutdownAmt"]         //50
                                                    , drLaborHrsAdjustmentDetail["Lhd_RestdayPlantShutdownOTAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_RestdayPlantShutdownNDAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_RestdayPlantShutdownOTNDAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_RestdayLegalHolidayCount"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_AbsentAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_LateAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_UndertimeAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_WholeDayAbsentAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_UnpaidLeaveAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_AbsentLegalHolidayAmt"]           //60
                                                    , drLaborHrsAdjustmentDetail["Lhd_AbsentSpecialHolidayAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_AbsentCompanyHolidayAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_AbsentPlantShutdownAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_AbsentFillerHolidayAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_PaidLeaveAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_PaidLegalHolidayAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_PaidSpecialHolidayAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_PaidCompanyHolidayAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_PaidFillerHolidayAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_RegularAdjustmentAmt"]            //70
                                                    , drLaborHrsAdjustmentDetail["Lhd_OvertimeAdjustmentAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_HolidayAdjustmentAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_NightPremiumAdjustmentAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_LeaveAdjustmentAmt"]
                                                    , drLaborHrsAdjustmentDetail["Lhd_LaborHrsAdjustmentAmt"]
                                                    , drLaborHrsAdjustmentDetail["Usr_Login"]);
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
                    strUpdateRecordTemplate = @" INSERT INTO T_LaborHrsAdjustmentExt (Lha_EmployeeId,Lha_AdjustpayPeriod,Lha_PayPeriod,Lha_ProcessDate,Lha_Filler01_Hr,Lha_Filler01_OTHr,Lha_Filler01_NDHr,Lha_Filler01_OTNDHr,Lha_Filler02_Hr,Lha_Filler02_OTHr,Lha_Filler02_NDHr,Lha_Filler02_OTNDHr,Lha_Filler03_Hr,Lha_Filler03_OTHr,Lha_Filler03_NDHr,Lha_Filler03_OTNDHr,Lha_Filler04_Hr,Lha_Filler04_OTHr,Lha_Filler04_NDHr,Lha_Filler04_OTNDHr,Lha_Filler05_Hr,Lha_Filler05_OTHr,Lha_Filler05_NDHr,Lha_Filler05_OTNDHr,Lha_Filler06_Hr,Lha_Filler06_OTHr,Lha_Filler06_NDHr,Lha_Filler06_OTNDHr,Usr_Login,Ludatetime) VALUES('{0}','{1}','{2}','{3}',{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},'{28}',GETDATE()) ";
                    #endregion
                    foreach (DataRow drLaborHrsAdjustmentExt in dtLaborHrsAdjustmentExt.Rows)
                    {
                        #region Adjustment Ext Insert
                        strUpdateQuery += string.Format(strUpdateRecordTemplate
                                                          , drLaborHrsAdjustmentExt["Lha_EmployeeId"]                   //0
                                                          , drLaborHrsAdjustmentExt["Lha_AdjustpayPeriod"]
                                                          , drLaborHrsAdjustmentExt["Lha_PayPeriod"]
                                                          , drLaborHrsAdjustmentExt["Lha_ProcessDate"]
                                                          , drLaborHrsAdjustmentExt["Lha_Filler01_Hr"]
                                                          , drLaborHrsAdjustmentExt["Lha_Filler01_OTHr"]
                                                          , drLaborHrsAdjustmentExt["Lha_Filler01_NDHr"]
                                                          , drLaborHrsAdjustmentExt["Lha_Filler01_OTNDHr"]
                                                          , drLaborHrsAdjustmentExt["Lha_Filler02_Hr"]
                                                          , drLaborHrsAdjustmentExt["Lha_Filler02_OTHr"]
                                                          , drLaborHrsAdjustmentExt["Lha_Filler02_NDHr"]                //10
                                                          , drLaborHrsAdjustmentExt["Lha_Filler02_OTNDHr"]
                                                          , drLaborHrsAdjustmentExt["Lha_Filler03_Hr"]
                                                          , drLaborHrsAdjustmentExt["Lha_Filler03_OTHr"]
                                                          , drLaborHrsAdjustmentExt["Lha_Filler03_NDHr"]
                                                          , drLaborHrsAdjustmentExt["Lha_Filler03_OTNDHr"]
                                                          , drLaborHrsAdjustmentExt["Lha_Filler04_Hr"]
                                                          , drLaborHrsAdjustmentExt["Lha_Filler04_OTHr"]
                                                          , drLaborHrsAdjustmentExt["Lha_Filler04_NDHr"]
                                                          , drLaborHrsAdjustmentExt["Lha_Filler04_OTNDHr"]
                                                          , drLaborHrsAdjustmentExt["Lha_Filler05_Hr"]                  //20
                                                          , drLaborHrsAdjustmentExt["Lha_Filler05_OTHr"]
                                                          , drLaborHrsAdjustmentExt["Lha_Filler05_NDHr"]
                                                          , drLaborHrsAdjustmentExt["Lha_Filler05_OTNDHr"]
                                                          , drLaborHrsAdjustmentExt["Lha_Filler06_Hr"]
                                                          , drLaborHrsAdjustmentExt["Lha_Filler06_OTHr"]
                                                          , drLaborHrsAdjustmentExt["Lha_Filler06_NDHr"]
                                                          , drLaborHrsAdjustmentExt["Lha_Filler06_OTNDHr"]
                                                          , drLaborHrsAdjustmentExt["Usr_Login"]);
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
                    strUpdateRecordTemplate = @" INSERT INTO T_LaborHrsAdjustmentExtDetail (Lhd_EmployeeId,Lhd_AdjustpayPeriod,Lhd_PayPeriod,Lhd_ProcessDate,Lhd_Filler01_Amt,Lhd_Filler01_OTAmt,Lhd_Filler01_NDAmt,Lhd_Filler01_OTNDAmt,Lhd_Filler02_Amt,Lhd_Filler02_OTAmt,Lhd_Filler02_NDAmt,Lhd_Filler02_OTNDAmt,Lhd_Filler03_Amt,Lhd_Filler03_OTAmt,Lhd_Filler03_NDAmt,Lhd_Filler03_OTNDAmt,Lhd_Filler04_Amt,Lhd_Filler04_OTAmt,Lhd_Filler04_NDAmt,Lhd_Filler04_OTNDAmt,Lhd_Filler05_Amt,Lhd_Filler05_OTAmt,Lhd_Filler05_NDAmt,Lhd_Filler05_OTNDAmt,Lhd_Filler06_Amt,Lhd_Filler06_OTAmt,Lhd_Filler06_NDAmt,Lhd_Filler06_OTNDAmt,Usr_Login,Ludatetime) VALUES ('{0}','{1}','{2}','{3}',{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},'{28}',GETDATE()) ";
                    #endregion
                    foreach (DataRow drLaborHrsAdjustmentExtDetail in dtLaborHrsAdjustmentExtDetail.Rows)
                    {
                        #region Adjustment Ext Detail Insert
                        strUpdateQuery += string.Format(strUpdateRecordTemplate
                                                          , drLaborHrsAdjustmentExtDetail["Lhd_EmployeeId"]                   //0
                                                          , drLaborHrsAdjustmentExtDetail["Lhd_AdjustpayPeriod"]
                                                          , drLaborHrsAdjustmentExtDetail["Lhd_PayPeriod"]
                                                          , drLaborHrsAdjustmentExtDetail["Lhd_ProcessDate"]
                                                          , drLaborHrsAdjustmentExtDetail["Lhd_Filler01_Amt"]
                                                          , drLaborHrsAdjustmentExtDetail["Lhd_Filler01_OTAmt"]
                                                          , drLaborHrsAdjustmentExtDetail["Lhd_Filler01_NDAmt"]
                                                          , drLaborHrsAdjustmentExtDetail["Lhd_Filler01_OTNDAmt"]
                                                          , drLaborHrsAdjustmentExtDetail["Lhd_Filler02_Amt"]
                                                          , drLaborHrsAdjustmentExtDetail["Lhd_Filler02_OTAmt"]
                                                          , drLaborHrsAdjustmentExtDetail["Lhd_Filler02_NDAmt"]                //10
                                                          , drLaborHrsAdjustmentExtDetail["Lhd_Filler02_OTNDAmt"]
                                                          , drLaborHrsAdjustmentExtDetail["Lhd_Filler03_Amt"]
                                                          , drLaborHrsAdjustmentExtDetail["Lhd_Filler03_OTAmt"]
                                                          , drLaborHrsAdjustmentExtDetail["Lhd_Filler03_NDAmt"]
                                                          , drLaborHrsAdjustmentExtDetail["Lhd_Filler03_OTNDAmt"]
                                                          , drLaborHrsAdjustmentExtDetail["Lhd_Filler04_Amt"]
                                                          , drLaborHrsAdjustmentExtDetail["Lhd_Filler04_OTAmt"]
                                                          , drLaborHrsAdjustmentExtDetail["Lhd_Filler04_NDAmt"]
                                                          , drLaborHrsAdjustmentExtDetail["Lhd_Filler04_OTNDAmt"]
                                                          , drLaborHrsAdjustmentExtDetail["Lhd_Filler05_Amt"]                  //20
                                                          , drLaborHrsAdjustmentExtDetail["Lhd_Filler05_OTAmt"]
                                                          , drLaborHrsAdjustmentExtDetail["Lhd_Filler05_NDAmt"]
                                                          , drLaborHrsAdjustmentExtDetail["Lhd_Filler05_OTNDAmt"]
                                                          , drLaborHrsAdjustmentExtDetail["Lhd_Filler06_Amt"]
                                                          , drLaborHrsAdjustmentExtDetail["Lhd_Filler06_OTAmt"]
                                                          , drLaborHrsAdjustmentExtDetail["Lhd_Filler06_NDAmt"]
                                                          , drLaborHrsAdjustmentExtDetail["Lhd_Filler06_OTNDAmt"]
                                                          , drLaborHrsAdjustmentExtDetail["Usr_Login"]);
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
                StatusHandler(this, new StatusEventArgs("Saving of Adjustment Records", true));
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

            strResult = commonBL.GetNumericValue("MDIVISOR");
            MDIVISOR = strResult.Equals(string.Empty) ? 0 : Convert.ToDecimal(strResult);

            strResult = commonBL.GetNumericValue("HRLYRTEDEC");
            HRLYRTEDEC = strResult.Equals(string.Empty) ? 0 : Convert.ToDecimal(strResult);

            HRRTINCDEM = Convert.ToString(commonBL.GetProcessControlFlag("PAYROLL", "HRRTINCDEM"));

            strResult = commonBL.GetNumericValue("NDBRCKTCNT");
            NDBRCKTCNT = strResult.Equals(string.Empty) ? 1 : Convert.ToInt32(Convert.ToDouble(strResult));

            NP1_RATE = Convert.ToDecimal(commonBL.GetParameterValue("NDBRACKET", "NP1_RATE")) / 100;
            NP2_RATE = Convert.ToDecimal(commonBL.GetParameterValue("NDBRACKET", "NP2_RATE")) / 100;
        }

        private void ShowEmployeeName(object sender, NewLaborHoursGenerationBL2.EmpDispEventArgs e)
        {
            EmpDispHandler(this, new EmpDispEventArgs(e.EmployeeId, e.LastName, e.FirstName, "LABOR HOURS (HISTORY)"));
        }

        private void ShowLaborHourStatus(object sender, NewLaborHoursGenerationBL2.StatusEventArgs e)
        {
            StatusHandler(this, new StatusEventArgs(e.Status, true));
        }

        public int CountAllEmployeeForProcess(string PayPeriod, DALHelper dalHelper)
        {
            string query = string.Format(@"SELECT count(DISTINCT Ell_EmployeeId)
                                            FROM	T_EmployeeLogLedgerTrail 
                                            JOIN	T_EmployeeMaster
                                            ON		Emt_EmployeeID = Ell_EmployeeId
                                            WHERE	Ell_AdjustpayPeriod = '{0}'", PayPeriod);
            DataTable dtResult = dalHelper.ExecuteDataSet(query).Tables[0];
            return Convert.ToInt32(dtResult.Rows[0][0].ToString());
        }

        public void ClearTransactionTables(bool ProcessAll, string EmployeeId, string PayPeriod)
        {
            string EmployeeCondition = "";
            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition += " AND Ept_EmployeeID = '" + EmployeeId + "'";

            string query = string.Format(@"DELETE FROM T_EmployeePayrollTransactionHist WHERE Ept_CurrentPayPeriod = '{0}' {1}
                                           DELETE FROM T_EmployeePayrollTransactionHistExt WHERE Ept_CurrentPayPeriod = '{0}' {1}
                                           DELETE FROM T_EmployeePayrollTransactionHistDetail WHERE Ept_CurrentPayPeriod = '{0}' {1}
                                           DELETE FROM T_EmployeePayrollTransactionHistExtDetail WHERE Ept_CurrentPayPeriod = '{0}' {1}", PayPeriod, EmployeeCondition);
            dal.ExecuteNonQuery(query);

            query = string.Format(@"DELETE FROM T_EmployeePayrollTransactionTrail WHERE Ept_CurrentPayPeriod = '{0}' AND Ept_AdjustPayPeriod = '{1}' {2}
                                    DELETE FROM T_EmployeePayrollTransactionTrailExt WHERE Ept_CurrentPayPeriod = '{0}' AND Ept_AdjustPayPeriod = '{1}' {2}
                                    DELETE FROM T_EmployeePayrollTransactionTrailDetail WHERE Ept_CurrentPayPeriod = '{0}' AND Ept_AdjustPayPeriod = '{1}' {2}
                                    DELETE FROM T_EmployeePayrollTransactionTrailExtDetail WHERE Ept_CurrentPayPeriod = '{0}' AND Ept_AdjustPayPeriod = '{1}' {2}", PayPeriod, ProcessPayrollPeriod, EmployeeCondition);
            dal.ExecuteNonQuery(query);
        }

        public void ClearAdjustmentTable(bool ProcessAll, string EmployeeId)
        {
            string EmployeeCondition1 = "";
            string EmployeeCondition2 = "";
            string EmployeeCondition3 = "";
            if (!ProcessAll && EmployeeId != "")
            {
                EmployeeCondition1 += " AND Lha_EmployeeID = '" + EmployeeId + "'";
                EmployeeCondition2 += " AND Eda_EmployeeID = '" + EmployeeId + "'";
                EmployeeCondition3 += " AND Lhd_EmployeeID = '" + EmployeeId + "'";
            }
            else if (ProcessAll && EmployeeList != "")
            {
                EmployeeCondition1 += " AND Lha_EmployeeID IN (" + EmployeeList + ")";
                EmployeeCondition2 += " AND Eda_EmployeeID IN (" + EmployeeList + ")";
                EmployeeCondition3 += " AND Lhd_EmployeeID IN (" + EmployeeList + ")";
            }

            string query = string.Format(@"DELETE FROM T_LaborHrsAdjustment WHERE Lha_AdjustpayPeriod = '{0}' {1}
                                           DELETE FROM T_LaborHrsAdjustmentExt WHERE Lha_AdjustpayPeriod = '{0}' {1}
                                           DELETE FROM T_LaborHrsAdjustmentDetail WHERE Lhd_AdjustpayPeriod = '{0}' {3}
                                           DELETE FROM T_LaborHrsAdjustmentExtDetail WHERE Lhd_AdjustpayPeriod = '{0}' {3}
                                           DELETE FROM T_EmployeeDailyAllowance WHERE Eda_Cycle = 'P' AND Eda_CurrentPayPeriod = '{0}' {2}", ProcessPayrollPeriod, EmployeeCondition1, EmployeeCondition2, EmployeeCondition3);
            dal.ExecuteNonQuery(query);
        }

        public DataTable GetAllowanceHeader()
        {
            string query = @"SELECT [Alh_LedgerAlwCol]
                                   ,[Alh_AllowanceCode]
                                   ,[Alh_AllowanceAdjCode]
                              FROM  [T_AllowanceHeader]
                              WHERE Alh_Status = 'A'";
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public DataTable GetDayCodePremiums()
        {
            string query = @"SELECT Dpm_PremiumGroup
                                  , Dpm_PayrollType
                                  , Dpm_Daycode
                                  , Dpm_RestDay
                                  , Dpm_RegularHourPremium
                                  , Dpm_OvertimeHourPremium
                                  , Dpm_RegularNDHourPremium 
                                  , Dpm_OvertimeNDHourPremium 
                                  , Dpm_RegularNDPercentage
                                  , Dpm_OvertimeNDPercentage
                              FROM T_DayPremiumMaster
                              WHERE Dpm_Status = 'A'";

            DataTable dtResult;
            dtResult = dal.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public DataTable GetDayCodePremiumFillers()
        {
            string query = @"SELECT Dpm_PremiumGroup
                                  , Dpm_PayrollType
                                  ,[Dpm_Daycode]
                                  ,[Dpm_RestDay]
                                  ,[Dpm_RegularHourPremium]
                                  ,[Dpm_OvertimeHourPremium]
                                  , Dpm_RegularNDHourPremium 
                                  , Dpm_OvertimeNDHourPremium 
                                  , Dpm_RegularNDPercentage
                                  , Dpm_OvertimeNDPercentage
                                  , Dcf_FillerSeq
                              FROM [T_DayPremiumMaster]
                              INNER JOIN T_DayCodeFiller ON Dpm_Daycode = Dcf_Daycode
                                and T_DayCodeFiller.Dcf_Restday = T_DayPremiumMaster.Dpm_RestDay
                              WHERE [Dpm_Status] = 'A'";

            DataTable dtResult;
            dtResult = dal.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public DataTable GetAllEmployeesForAdjustment(bool ProcessAll, string EmployeeId)
        {
            string EmployeeCondition = "";
            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition += " AND TRAIL.Ell_EmployeeID = '" + EmployeeId + "'";
            else if (ProcessAll && EmployeeList != "")
                EmployeeCondition += " AND TRAIL.Ell_EmployeeID IN (" + EmployeeList + ")";

            string query = string.Format(@"SELECT DISTINCT TRAIL.Ell_EmployeeId
			                                            , TRAIL.Ell_PayPeriod
			                                            , Ell_AdjustPayPeriod
			                                            , Emt_Lastname
			                                            , Emt_Firstname
                                            FROM T_EmployeeLogLedgerTrail TRAIL
                                            INNER JOIN T_EmployeeMaster
                                            ON Emt_EmployeeID = TRAIL.Ell_EmployeeId
                                            INNER JOIN (
                                                SELECT Pmx_Classification 
                                                FROM T_ParameterMasterExt 
                                                WHERE Pmx_ParameterID = 'EMPSTATPAY'
                                                    AND Pmx_ParameterValue = 1
                                            ) EMPSTAT
                                                ON Emt_EmploymentStatus = Pmx_Classification
                                            INNER JOIN T_EmployeeLogLedgerHist HIST
                                            ON HIST.Ell_EmployeeId = TRAIL.Ell_EmployeeId
	                                            AND HIST.Ell_PayPeriod = TRAIL.Ell_PayPeriod
	                                            AND HIST.Ell_ProcessDate = TRAIL.Ell_ProcessDate
	                                            AND TRAIL.Ell_AdjustpayPeriod = '{0}'
	                                            AND (HIST.Ell_ActualTimeIn_1 != TRAIL.Ell_ActualTimeIn_1
		                                            OR HIST.Ell_ActualTimeOut_1 != TRAIL.Ell_ActualTimeOut_1
		                                            OR HIST.Ell_ActualTimeIn_2 != TRAIL.Ell_ActualTimeIn_2
		                                            OR HIST.Ell_ActualTimeOut_2 != TRAIL.Ell_ActualTimeOut_2
		                                            OR HIST.Ell_ShiftCode != TRAIL.Ell_ShiftCode
		                                            OR HIST.Ell_DayCode != TRAIL.Ell_DayCode
		                                            OR HIST.Ell_RestDay != TRAIL.Ell_RestDay
		                                            OR HIST.Ell_Holiday != TRAIL.Ell_Holiday
		                                            OR HIST.Ell_EncodedPayLeaveType != TRAIL.Ell_EncodedPayLeaveType
		                                            OR HIST.Ell_EncodedPayLeaveHr != TRAIL.Ell_EncodedPayLeaveHr
		                                            OR HIST.Ell_EncodedNoPayLeaveType != TRAIL.Ell_EncodedNoPayLeaveType
		                                            OR HIST.Ell_EncodedNoPayLeaveHr != TRAIL.Ell_EncodedNoPayLeaveHr
		                                            OR HIST.Ell_EncodedOvertimeAdvHr != TRAIL.Ell_EncodedOvertimeAdvHr
		                                            OR HIST.Ell_EncodedOvertimePostHr != TRAIL.Ell_EncodedOvertimePostHr
		                                            OR HIST.Ell_EncodedOvertimeMin != TRAIL.Ell_EncodedOvertimeMin
		                                            OR HIST.Ell_RegularHour != TRAIL.Ell_RegularHour
		                                            OR HIST.Ell_OvertimeHour != TRAIL.Ell_OvertimeHour
													OR HIST.Ell_RegularNightPremHour != TRAIL.Ell_RegularNightPremHour
		                                            OR HIST.Ell_OvertimeNightPremHour != TRAIL.Ell_OvertimeNightPremHour
													OR HIST.Ell_LeaveHour != TRAIL.Ell_LeaveHour
		                                            OR HIST.Ell_AbsentHour != TRAIL.Ell_AbsentHour
                                                    OR ISNULL(HIST.Ell_AssumedPostBack, '') != ISNULL(TRAIL.Ell_AssumedPostBack, '')
		                                            OR HIST.Ell_AssumedPresent != TRAIL.Ell_AssumedPresent
		                                            OR HIST.Ell_SundayHolidayCount != TRAIL.Ell_SundayHolidayCount
                                                    OR ISNULL(HIST.Ell_ForceLeave, 0) != ISNULL(TRAIL.Ell_ForceLeave, 0)
													OR HIST.Ell_AllowanceAmt01 != TRAIL.Ell_AllowanceAmt01
													OR HIST.Ell_AllowanceAmt02 != TRAIL.Ell_AllowanceAmt02
													OR HIST.Ell_AllowanceAmt03 != TRAIL.Ell_AllowanceAmt03
													OR HIST.Ell_AllowanceAmt04 != TRAIL.Ell_AllowanceAmt04
													OR HIST.Ell_AllowanceAmt05 != TRAIL.Ell_AllowanceAmt05
													OR HIST.Ell_AllowanceAmt06 != TRAIL.Ell_AllowanceAmt06
													OR HIST.Ell_AllowanceAmt07 != TRAIL.Ell_AllowanceAmt07
													OR HIST.Ell_AllowanceAmt08 != TRAIL.Ell_AllowanceAmt08
													OR HIST.Ell_AllowanceAmt09 != TRAIL.Ell_AllowanceAmt09
													OR HIST.Ell_AllowanceAmt10 != TRAIL.Ell_AllowanceAmt10
													OR HIST.Ell_AllowanceAmt11 != TRAIL.Ell_AllowanceAmt11
													OR HIST.Ell_AllowanceAmt12 != TRAIL.Ell_AllowanceAmt12)
                                                {1}", ProcessPayrollPeriod, EmployeeCondition);
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public DataTable GetEmployeePayrollTransactionTrailRecords(bool ProcessAll, string EmployeeId, DALHelper dal)
        {
            string EmployeeCondition = "";
            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition += " AND TRAIL.Ell_EmployeeId = '" + EmployeeId + "'";
            else if (ProcessAll && EmployeeList != "")
                EmployeeCondition += " AND TRAIL.Ell_EmployeeId IN (" + EmployeeList + ")";

            #region query
            string query = string.Format(@"select A.Ept_EmployeeId AS Ept_EmployeeId
                                                  ,A.Ept_AdjustPayPeriod AS Ept_AdjustPayPeriod
                                                  ,A.Ept_CurrentPayPeriod AS Ept_CurrentPayPeriod
                                                  ,A.Ept_ProcessDate AS Ept_ProcessDate
                                                  ,A.Ept_SalaryRate AS Ept_SalaryRate
                                                  ,A.Ept_HourlyRate AS Ept_HourlyRate
                                                  ,DBO.GetHourlyRateWithDeMinimis(A.Ept_EmployeeId, '{0}') as HourlyRateWithDeminimis
                                                  ,A.Ept_SalaryType AS Ept_SalaryType
                                                  ,A.Ept_PayrollType AS Ept_PayrollType
                                                  ,Ept_AbsentHr
                                                  ,Ept_RegularHr
                                                  ,Ept_RegularOTHr
                                                  ,Ept_RegularNDHr
                                                  ,Ept_RegularOTNDHr
                                                  ,Ept_RestdayHr
                                                  ,Ept_RestdayOTHr
                                                  ,Ept_RestdayNDHr
                                                  ,Ept_RestdayOTNDHr
                                                  ,Ept_LegalHolidayHr
                                                  ,Ept_LegalHolidayOTHr
                                                  ,Ept_LegalHolidayNDHr
                                                  ,Ept_LegalHolidayOTNDHr
                                                  ,Ept_SpecialHolidayHr
                                                  ,Ept_SpecialHolidayOTHr
                                                  ,Ept_SpecialHolidayNDHr
                                                  ,Ept_SpecialHolidayOTNDHr
                                                  ,Ept_PlantShutdownHr
                                                  ,Ept_PlantShutdownOTHr
                                                  ,Ept_PlantShutdownNDHr
                                                  ,Ept_PlantShutdownOTNDHr
                                                  ,Ept_CompanyHolidayHr
                                                  ,Ept_CompanyHolidayOTHr
                                                  ,Ept_CompanyHolidayNDHr
                                                  ,Ept_CompanyHolidayOTNDHr
                                                  ,Ept_RestdayLegalHolidayHr
                                                  ,Ept_RestdayLegalHolidayOTHr
                                                  ,Ept_RestdayLegalHolidayNDHr
                                                  ,Ept_RestdayLegalHolidayOTNDHr
                                                  ,Ept_RestdaySpecialHolidayHr
                                                  ,Ept_RestdaySpecialHolidayOTHr
                                                  ,Ept_RestdaySpecialHolidayNDHr
                                                  ,Ept_RestdaySpecialHolidayOTNDHr
                                                  ,Ept_RestdayCompanyHolidayHr
                                                  ,Ept_RestdayCompanyHolidayOTHr
                                                  ,Ept_RestdayCompanyHolidayNDHr
                                                  ,Ept_RestdayCompanyHolidayOTNDHr
                                                  ,Ept_RestdayPlantShutdownHr
                                                  ,Ept_RestdayPlantShutdownOTHr
                                                  ,Ept_RestdayPlantShutdownNDHr
                                                  ,Ept_RestdayPlantShutdownOTNDHr
                                                  ,ISNULL(Ept_LateHr, 0) AS Ept_LateHr
                                                  ,ISNULL(Ept_UndertimeHr, 0) AS Ept_UndertimeHr
                                                  ,ISNULL(Ept_WholeDayAbsentHr, 0) AS Ept_WholeDayAbsentHr
                                                  ,ISNULL(Ept_UnpaidLeaveHr, 0) AS Ept_UnpaidLeaveHr
                                                  ,ISNULL(Ept_AbsentLegalHolidayHr, 0) AS Ept_AbsentLegalHolidayHr
                                                  ,ISNULL(Ept_AbsentSpecialHolidayHr, 0) AS Ept_AbsentSpecialHolidayHr
                                                  ,ISNULL(Ept_AbsentCompanyHolidayHr, 0) AS Ept_AbsentCompanyHolidayHr
                                                  ,ISNULL(Ept_AbsentPlantShutdownHr, 0) AS Ept_AbsentPlantShutdownHr
                                                  ,ISNULL(Ept_AbsentFillerHolidayHr, 0) AS Ept_AbsentFillerHolidayHr
                                                  ,ISNULL(Ept_PaidLeaveHr, 0) AS Ept_PaidLeaveHr
                                                  ,ISNULL(Ept_PaidLegalHolidayHr, 0) AS Ept_PaidLegalHolidayHr
                                                  ,ISNULL(Ept_PaidSpecialHolidayHr, 0) AS Ept_PaidSpecialHolidayHr
                                                  ,ISNULL(Ept_PaidCompanyHolidayHr, 0) AS Ept_PaidCompanyHolidayHr
                                                  ,ISNULL(Ept_PaidFillerHolidayHr, 0) AS Ept_PaidFillerHolidayHr
                                                  ,Ept_RestdayLegalHolidayCount
                                                  ,Ept_WorkingDay
                                                  ,ISNULL(Ept_Filler01_Hr, 0) AS Ept_Filler01_Hr
                                                  ,ISNULL(Ept_Filler01_OTHr, 0) AS Ept_Filler01_OTHr
                                                  ,ISNULL(Ept_Filler01_NDHr, 0) AS Ept_Filler01_NDHr
                                                  ,ISNULL(Ept_Filler01_OTNDHr, 0) AS Ept_Filler01_OTNDHr
                                                  ,ISNULL(Ept_Filler02_Hr, 0) AS Ept_Filler02_Hr
                                                  ,ISNULL(Ept_Filler02_OTHr, 0) AS Ept_Filler02_OTHr
                                                  ,ISNULL(Ept_Filler02_NDHr, 0) AS Ept_Filler02_NDHr
                                                  ,ISNULL(Ept_Filler02_OTNDHr, 0) AS Ept_Filler02_OTNDHr
                                                  ,ISNULL(Ept_Filler03_Hr, 0) AS Ept_Filler03_Hr
                                                  ,ISNULL(Ept_Filler03_OTHr, 0) AS Ept_Filler03_OTHr
                                                  ,ISNULL(Ept_Filler03_NDHr, 0) AS Ept_Filler03_NDHr
                                                  ,ISNULL(Ept_Filler03_OTNDHr, 0) AS Ept_Filler03_OTNDHr
                                                  ,ISNULL(Ept_Filler04_Hr, 0) AS Ept_Filler04_Hr
                                                  ,ISNULL(Ept_Filler04_OTHr, 0) AS Ept_Filler04_OTHr
                                                  ,ISNULL(Ept_Filler04_NDHr, 0) AS Ept_Filler04_NDHr
                                                  ,ISNULL(Ept_Filler04_OTNDHr, 0) AS Ept_Filler04_OTNDHr
                                                  ,ISNULL(Ept_Filler05_Hr, 0) AS Ept_Filler05_Hr
                                                  ,ISNULL(Ept_Filler05_OTHr, 0) AS Ept_Filler05_OTHr
                                                  ,ISNULL(Ept_Filler05_NDHr, 0) AS Ept_Filler05_NDHr
                                                  ,ISNULL(Ept_Filler05_OTNDHr, 0) AS Ept_Filler05_OTNDHr
                                                  ,ISNULL(Ept_Filler06_Hr, 0) AS Ept_Filler06_Hr
                                                  ,ISNULL(Ept_Filler06_OTHr, 0) AS Ept_Filler06_OTHr
                                                  ,ISNULL(Ept_Filler06_NDHr, 0) AS Ept_Filler06_NDHr
                                                  ,ISNULL(Ept_Filler06_OTNDHr, 0) AS Ept_Filler06_OTNDHr
                                                  ,TRAIL.Ell_AllowanceAmt01
                                                  ,TRAIL.Ell_AllowanceAmt02
                                                  ,TRAIL.Ell_AllowanceAmt03
                                                  ,TRAIL.Ell_AllowanceAmt04
                                                  ,TRAIL.Ell_AllowanceAmt05
                                                  ,TRAIL.Ell_AllowanceAmt06
                                                  ,TRAIL.Ell_AllowanceAmt07
                                                  ,TRAIL.Ell_AllowanceAmt08
                                                  ,TRAIL.Ell_AllowanceAmt09
                                                  ,TRAIL.Ell_AllowanceAmt10
                                                  ,TRAIL.Ell_AllowanceAmt11
                                                  ,TRAIL.Ell_AllowanceAmt12
                                                  ,Emt_LastName
                                                  ,Emt_FirstName 
                                                  ,TRAIL.Ell_RestDay
                                                  ,TRAIL.Ell_Holiday
                                                  ,Emt_PremiumGroup
                                                  ,Emt_EmploymentStatus
                                            FROM T_EmployeePayrollTransactionTrailDetail A
                                            INNER JOIN T_EmployeeLogLedgerTrail TRAIL
                                            ON A.Ept_EmployeeId = Ell_EmployeeId
	                                            AND A.Ept_ProcessDate = Ell_ProcessDate
	                                            AND A.Ept_AdjustPayPeriod = Ell_AdjustpayPeriod
	                                            AND A.Ept_CurrentPayPeriod = Ell_PayPeriod
                                                {1}
                                            INNER JOIN T_EmployeeLogLedgerHist HIST
                                            ON HIST.Ell_EmployeeId = TRAIL.Ell_EmployeeId
	                                            AND HIST.Ell_PayPeriod = TRAIL.Ell_PayPeriod
	                                            AND HIST.Ell_ProcessDate = TRAIL.Ell_ProcessDate
	                                            AND TRAIL.Ell_AdjustpayPeriod = '{0}'
	                                            AND (HIST.Ell_ActualTimeIn_1 != TRAIL.Ell_ActualTimeIn_1
		                                            OR HIST.Ell_ActualTimeOut_1 != TRAIL.Ell_ActualTimeOut_1
		                                            OR HIST.Ell_ActualTimeIn_2 != TRAIL.Ell_ActualTimeIn_2
		                                            OR HIST.Ell_ActualTimeOut_2 != TRAIL.Ell_ActualTimeOut_2
		                                            OR HIST.Ell_ShiftCode != TRAIL.Ell_ShiftCode
		                                            OR HIST.Ell_DayCode != TRAIL.Ell_DayCode
		                                            OR HIST.Ell_RestDay != TRAIL.Ell_RestDay
		                                            OR HIST.Ell_Holiday != TRAIL.Ell_Holiday
		                                            OR HIST.Ell_EncodedPayLeaveType != TRAIL.Ell_EncodedPayLeaveType
		                                            OR HIST.Ell_EncodedPayLeaveHr != TRAIL.Ell_EncodedPayLeaveHr
		                                            OR HIST.Ell_EncodedNoPayLeaveType != TRAIL.Ell_EncodedNoPayLeaveType
		                                            OR HIST.Ell_EncodedNoPayLeaveHr != TRAIL.Ell_EncodedNoPayLeaveHr
		                                            OR HIST.Ell_EncodedOvertimeAdvHr != TRAIL.Ell_EncodedOvertimeAdvHr
		                                            OR HIST.Ell_EncodedOvertimePostHr != TRAIL.Ell_EncodedOvertimePostHr
		                                            OR HIST.Ell_EncodedOvertimeMin != TRAIL.Ell_EncodedOvertimeMin
		                                            OR HIST.Ell_RegularHour != TRAIL.Ell_RegularHour
		                                            OR HIST.Ell_OvertimeHour != TRAIL.Ell_OvertimeHour
													OR HIST.Ell_RegularNightPremHour != TRAIL.Ell_RegularNightPremHour
		                                            OR HIST.Ell_OvertimeNightPremHour != TRAIL.Ell_OvertimeNightPremHour
													OR HIST.Ell_LeaveHour != TRAIL.Ell_LeaveHour
		                                            OR HIST.Ell_AbsentHour != TRAIL.Ell_AbsentHour
		                                            OR ISNULL(HIST.Ell_AssumedPostBack, '') != ISNULL(TRAIL.Ell_AssumedPostBack, '')
		                                            OR HIST.Ell_AssumedPresent != TRAIL.Ell_AssumedPresent
		                                            OR HIST.Ell_SundayHolidayCount != TRAIL.Ell_SundayHolidayCount
                                                    OR ISNULL(HIST.Ell_ForceLeave, 0) != ISNULL(TRAIL.Ell_ForceLeave, 0)
													OR HIST.Ell_AllowanceAmt01 != TRAIL.Ell_AllowanceAmt01
													OR HIST.Ell_AllowanceAmt02 != TRAIL.Ell_AllowanceAmt02
													OR HIST.Ell_AllowanceAmt03 != TRAIL.Ell_AllowanceAmt03
													OR HIST.Ell_AllowanceAmt04 != TRAIL.Ell_AllowanceAmt04
													OR HIST.Ell_AllowanceAmt05 != TRAIL.Ell_AllowanceAmt05
													OR HIST.Ell_AllowanceAmt06 != TRAIL.Ell_AllowanceAmt06
													OR HIST.Ell_AllowanceAmt07 != TRAIL.Ell_AllowanceAmt07
													OR HIST.Ell_AllowanceAmt08 != TRAIL.Ell_AllowanceAmt08
													OR HIST.Ell_AllowanceAmt09 != TRAIL.Ell_AllowanceAmt09
													OR HIST.Ell_AllowanceAmt10 != TRAIL.Ell_AllowanceAmt10
													OR HIST.Ell_AllowanceAmt11 != TRAIL.Ell_AllowanceAmt11
													OR HIST.Ell_AllowanceAmt12 != TRAIL.Ell_AllowanceAmt12)
                                            LEFT JOIN T_EmployeePayrollTransactionTrailExtDetail B
                                            ON A.Ept_EmployeeId = B.Ept_EmployeeId
	                                            AND A.Ept_ProcessDate = B.Ept_ProcessDate
	                                            AND A.Ept_CurrentPayPeriod = B.Ept_CurrentPayPeriod
	                                            AND A.Ept_AdjustPayPeriod = B.Ept_AdjustPayPeriod
                                            LEFT JOIN T_EmployeePayrollCalcAnnual CalcAnnual
											ON CalcAnnual.Epc_EmployeeId = A.Ept_EmployeeId
												AND CalcAnnual.Epc_CurrentPayPeriod = A.Ept_CurrentPayPeriod
											LEFT JOIN T_EmployeePayrollCalcHist CalcHist
											ON CalcHist.Epc_EmployeeId = A.Ept_EmployeeId
												AND CalcHist.Epc_CurrentPayPeriod = A.Ept_CurrentPayPeriod
                                            INNER JOIN T_EmployeeMaster
                                            ON A.Ept_EmployeeId = Emt_EmployeeID
                                            WHERE A.Ept_AdjustPayPeriod = '{0}'
                                                AND (CalcHist.Epc_EmployeeId IS NOT NULL
													OR CalcAnnual.Epc_EmployeeId IS NOT NULL)
                                            ORDER BY Emt_LastName, Emt_FirstName, A.Ept_AdjustPayPeriod, A.Ept_CurrentPayPeriod, A.Ept_ProcessDate", ProcessPayrollPeriod, EmployeeCondition);
            #endregion
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public DataTable GetEmployeePayrollTransactionHistRecords(bool ProcessAll, string EmployeeId, DALHelper dal)
        {
            string EmployeeCondition = "";
            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition += " AND TRAIL.Ell_EmployeeId = '" + EmployeeId + "'";
            if (ProcessAll && EmployeeList != "")
                EmployeeCondition += " AND TRAIL.Ell_EmployeeId IN (" + EmployeeList + ")";

            #region query
            string query = string.Format(@"SELECT  A.Ept_EmployeeId AS Ept_EmployeeId
		                                            ,A.Ept_CurrentPayPeriod AS Ept_CurrentPayPeriod
		                                            ,A.Ept_ProcessDate AS Ept_ProcessDate
		                                            ,A.Ept_SalaryRate AS Ept_SalaryRate
		                                            ,A.Ept_HourlyRate AS Ept_HourlyRate
                                                    ,DBO.GetHourlyRateWithDeMinimis(A.Ept_EmployeeId, '{0}') as HourlyRateWithDeminimis
		                                            ,A.Ept_SalaryType AS Ept_SalaryType
		                                            ,A.Ept_PayrollType AS Ept_PayrollType
		                                            ,A.Ept_AbsentHr
		                                            ,A.Ept_RegularHr
		                                            ,A.Ept_RegularOTHr
		                                            ,A.Ept_RegularNDHr
		                                            ,A.Ept_RegularOTNDHr
		                                            ,A.Ept_RestdayHr
		                                            ,A.Ept_RestdayOTHr
		                                            ,A.Ept_RestdayNDHr
		                                            ,A.Ept_RestdayOTNDHr
		                                            ,A.Ept_LegalHolidayHr
		                                            ,A.Ept_LegalHolidayOTHr
		                                            ,A.Ept_LegalHolidayNDHr
		                                            ,A.Ept_LegalHolidayOTNDHr
		                                            ,A.Ept_SpecialHolidayHr
		                                            ,A.Ept_SpecialHolidayOTHr
		                                            ,A.Ept_SpecialHolidayNDHr
		                                            ,A.Ept_SpecialHolidayOTNDHr
		                                            ,A.Ept_PlantShutdownHr
		                                            ,A.Ept_PlantShutdownOTHr
		                                            ,A.Ept_PlantShutdownNDHr
		                                            ,A.Ept_PlantShutdownOTNDHr
		                                            ,A.Ept_CompanyHolidayHr
		                                            ,A.Ept_CompanyHolidayOTHr
		                                            ,A.Ept_CompanyHolidayNDHr
		                                            ,A.Ept_CompanyHolidayOTNDHr
		                                            ,A.Ept_RestdayLegalHolidayHr
		                                            ,A.Ept_RestdayLegalHolidayOTHr
		                                            ,A.Ept_RestdayLegalHolidayNDHr
		                                            ,A.Ept_RestdayLegalHolidayOTNDHr
		                                            ,A.Ept_RestdaySpecialHolidayHr
		                                            ,A.Ept_RestdaySpecialHolidayOTHr
		                                            ,A.Ept_RestdaySpecialHolidayNDHr
		                                            ,A.Ept_RestdaySpecialHolidayOTNDHr
		                                            ,A.Ept_RestdayCompanyHolidayHr
		                                            ,A.Ept_RestdayCompanyHolidayOTHr
		                                            ,A.Ept_RestdayCompanyHolidayNDHr
		                                            ,A.Ept_RestdayCompanyHolidayOTNDHr
		                                            ,A.Ept_RestdayPlantShutdownHr
		                                            ,A.Ept_RestdayPlantShutdownOTHr
		                                            ,A.Ept_RestdayPlantShutdownNDHr
		                                            ,A.Ept_RestdayPlantShutdownOTNDHr
                                                    ,ISNULL(A.Ept_LateHr, 0) AS Ept_LateHr
                                                    ,ISNULL(A.Ept_UndertimeHr, 0) AS Ept_UndertimeHr
                                                    ,ISNULL(A.Ept_WholeDayAbsentHr, 0) AS Ept_WholeDayAbsentHr
                                                    ,ISNULL(A.Ept_UnpaidLeaveHr, 0) AS Ept_UnpaidLeaveHr
                                                    ,ISNULL(A.Ept_AbsentLegalHolidayHr, 0) AS Ept_AbsentLegalHolidayHr
                                                    ,ISNULL(A.Ept_AbsentSpecialHolidayHr, 0) AS Ept_AbsentSpecialHolidayHr
                                                    ,ISNULL(A.Ept_AbsentCompanyHolidayHr, 0) AS Ept_AbsentCompanyHolidayHr
                                                    ,ISNULL(A.Ept_AbsentPlantShutdownHr, 0) AS Ept_AbsentPlantShutdownHr
                                                    ,ISNULL(A.Ept_AbsentFillerHolidayHr, 0) AS Ept_AbsentFillerHolidayHr
                                                    ,ISNULL(A.Ept_PaidLeaveHr, 0) AS Ept_PaidLeaveHr
                                                    ,ISNULL(A.Ept_PaidLegalHolidayHr, 0) AS Ept_PaidLegalHolidayHr
                                                    ,ISNULL(A.Ept_PaidSpecialHolidayHr, 0) AS Ept_PaidSpecialHolidayHr
                                                    ,ISNULL(A.Ept_PaidCompanyHolidayHr, 0) AS Ept_PaidCompanyHolidayHr
                                                    ,ISNULL(A.Ept_PaidFillerHolidayHr, 0) AS Ept_PaidFillerHolidayHr
		                                            ,A.Ept_RestdayLegalHolidayCount
		                                            ,A.Ept_WorkingDay
                                                    ,ISNULL(Ept_Filler01_Hr, 0) AS Ept_Filler01_Hr
                                                    ,ISNULL(Ept_Filler01_OTHr, 0) AS Ept_Filler01_OTHr
                                                    ,ISNULL(Ept_Filler01_NDHr, 0) AS Ept_Filler01_NDHr
                                                    ,ISNULL(Ept_Filler01_OTNDHr, 0) AS Ept_Filler01_OTNDHr
                                                    ,ISNULL(Ept_Filler02_Hr, 0) AS Ept_Filler02_Hr
                                                    ,ISNULL(Ept_Filler02_OTHr, 0) AS Ept_Filler02_OTHr
                                                    ,ISNULL(Ept_Filler02_NDHr, 0) AS Ept_Filler02_NDHr
                                                    ,ISNULL(Ept_Filler02_OTNDHr, 0) AS Ept_Filler02_OTNDHr
                                                    ,ISNULL(Ept_Filler03_Hr, 0) AS Ept_Filler03_Hr
                                                    ,ISNULL(Ept_Filler03_OTHr, 0) AS Ept_Filler03_OTHr
                                                    ,ISNULL(Ept_Filler03_NDHr, 0) AS Ept_Filler03_NDHr
                                                    ,ISNULL(Ept_Filler03_OTNDHr, 0) AS Ept_Filler03_OTNDHr
                                                    ,ISNULL(Ept_Filler04_Hr, 0) AS Ept_Filler04_Hr
                                                    ,ISNULL(Ept_Filler04_OTHr, 0) AS Ept_Filler04_OTHr
                                                    ,ISNULL(Ept_Filler04_NDHr, 0) AS Ept_Filler04_NDHr
                                                    ,ISNULL(Ept_Filler04_OTNDHr, 0) AS Ept_Filler04_OTNDHr
                                                    ,ISNULL(Ept_Filler05_Hr, 0) AS Ept_Filler05_Hr
                                                    ,ISNULL(Ept_Filler05_OTHr, 0) AS Ept_Filler05_OTHr
                                                    ,ISNULL(Ept_Filler05_NDHr, 0) AS Ept_Filler05_NDHr
                                                    ,ISNULL(Ept_Filler05_OTNDHr, 0) AS Ept_Filler05_OTNDHr
                                                    ,ISNULL(Ept_Filler06_Hr, 0) AS Ept_Filler06_Hr
                                                    ,ISNULL(Ept_Filler06_OTHr, 0) AS Ept_Filler06_OTHr
                                                    ,ISNULL(Ept_Filler06_NDHr, 0) AS Ept_Filler06_NDHr
                                                    ,ISNULL(Ept_Filler06_OTNDHr, 0) AS Ept_Filler06_OTNDHr
		                                            ,HIST.Ell_AllowanceAmt01
		                                            ,HIST.Ell_AllowanceAmt02
		                                            ,HIST.Ell_AllowanceAmt03
		                                            ,HIST.Ell_AllowanceAmt04
		                                            ,HIST.Ell_AllowanceAmt05
		                                            ,HIST.Ell_AllowanceAmt06
		                                            ,HIST.Ell_AllowanceAmt07
		                                            ,HIST.Ell_AllowanceAmt08
		                                            ,HIST.Ell_AllowanceAmt09
		                                            ,HIST.Ell_AllowanceAmt10
		                                            ,HIST.Ell_AllowanceAmt11
		                                            ,HIST.Ell_AllowanceAmt12
                                                    ,HIST.Ell_RestDay
                                                    ,HIST.Ell_Holiday
                                            FROM T_EmployeePayrollTransactionHistDetail A
                                            INNER JOIN T_EmployeeLogLedgerTrail TRAIL
                                            ON A.Ept_EmployeeId = Ell_EmployeeId
	                                            AND A.Ept_ProcessDate = Ell_ProcessDate
	                                            AND A.Ept_CurrentPayPeriod = Ell_PayPeriod
                                                {1}
                                            INNER JOIN T_EmployeeLogLedgerHist HIST
                                            ON HIST.Ell_EmployeeId = TRAIL.Ell_EmployeeId
	                                            AND HIST.Ell_PayPeriod = TRAIL.Ell_PayPeriod
	                                            AND HIST.Ell_ProcessDate = TRAIL.Ell_ProcessDate
	                                            AND TRAIL.Ell_AdjustpayPeriod = '{0}'
	                                            AND (HIST.Ell_ActualTimeIn_1 != TRAIL.Ell_ActualTimeIn_1
		                                            OR HIST.Ell_ActualTimeOut_1 != TRAIL.Ell_ActualTimeOut_1
		                                            OR HIST.Ell_ActualTimeIn_2 != TRAIL.Ell_ActualTimeIn_2
		                                            OR HIST.Ell_ActualTimeOut_2 != TRAIL.Ell_ActualTimeOut_2
		                                            OR HIST.Ell_ShiftCode != TRAIL.Ell_ShiftCode
		                                            OR HIST.Ell_DayCode != TRAIL.Ell_DayCode
		                                            OR HIST.Ell_RestDay != TRAIL.Ell_RestDay
		                                            OR HIST.Ell_Holiday != TRAIL.Ell_Holiday
		                                            OR HIST.Ell_EncodedPayLeaveType != TRAIL.Ell_EncodedPayLeaveType
		                                            OR HIST.Ell_EncodedPayLeaveHr != TRAIL.Ell_EncodedPayLeaveHr
		                                            OR HIST.Ell_EncodedNoPayLeaveType != TRAIL.Ell_EncodedNoPayLeaveType
		                                            OR HIST.Ell_EncodedNoPayLeaveHr != TRAIL.Ell_EncodedNoPayLeaveHr
		                                            OR HIST.Ell_EncodedOvertimeAdvHr != TRAIL.Ell_EncodedOvertimeAdvHr
		                                            OR HIST.Ell_EncodedOvertimePostHr != TRAIL.Ell_EncodedOvertimePostHr
		                                            OR HIST.Ell_EncodedOvertimeMin != TRAIL.Ell_EncodedOvertimeMin
		                                            OR HIST.Ell_RegularHour != TRAIL.Ell_RegularHour
		                                            OR HIST.Ell_OvertimeHour != TRAIL.Ell_OvertimeHour
													OR HIST.Ell_RegularNightPremHour != TRAIL.Ell_RegularNightPremHour
		                                            OR HIST.Ell_OvertimeNightPremHour != TRAIL.Ell_OvertimeNightPremHour
													OR HIST.Ell_LeaveHour != TRAIL.Ell_LeaveHour
		                                            OR HIST.Ell_AbsentHour != TRAIL.Ell_AbsentHour
		                                            OR ISNULL(HIST.Ell_AssumedPostBack, '') != ISNULL(TRAIL.Ell_AssumedPostBack, '')
		                                            OR HIST.Ell_AssumedPresent != TRAIL.Ell_AssumedPresent
		                                            OR HIST.Ell_SundayHolidayCount != TRAIL.Ell_SundayHolidayCount
                                                    OR ISNULL(HIST.Ell_ForceLeave, 0) != ISNULL(TRAIL.Ell_ForceLeave, 0)
													OR HIST.Ell_AllowanceAmt01 != TRAIL.Ell_AllowanceAmt01
													OR HIST.Ell_AllowanceAmt02 != TRAIL.Ell_AllowanceAmt02
													OR HIST.Ell_AllowanceAmt03 != TRAIL.Ell_AllowanceAmt03
													OR HIST.Ell_AllowanceAmt04 != TRAIL.Ell_AllowanceAmt04
													OR HIST.Ell_AllowanceAmt05 != TRAIL.Ell_AllowanceAmt05
													OR HIST.Ell_AllowanceAmt06 != TRAIL.Ell_AllowanceAmt06
													OR HIST.Ell_AllowanceAmt07 != TRAIL.Ell_AllowanceAmt07
													OR HIST.Ell_AllowanceAmt08 != TRAIL.Ell_AllowanceAmt08
													OR HIST.Ell_AllowanceAmt09 != TRAIL.Ell_AllowanceAmt09
													OR HIST.Ell_AllowanceAmt10 != TRAIL.Ell_AllowanceAmt10
													OR HIST.Ell_AllowanceAmt11 != TRAIL.Ell_AllowanceAmt11
													OR HIST.Ell_AllowanceAmt12 != TRAIL.Ell_AllowanceAmt12)
                                            LEFT JOIN T_EmployeePayrollTransactionHistExtDetail B
                                            ON A.Ept_EmployeeId = B.Ept_EmployeeId
	                                            AND A.Ept_ProcessDate = B.Ept_ProcessDate
                                            LEFT JOIN T_EmployeePayrollCalcAnnual CalcAnnual
											ON CalcAnnual.Epc_EmployeeId = A.Ept_EmployeeId
												AND CalcAnnual.Epc_CurrentPayPeriod = A.Ept_CurrentPayPeriod
											LEFT JOIN T_EmployeePayrollCalcHist CalcHist
											ON CalcHist.Epc_EmployeeId = A.Ept_EmployeeId
												AND CalcHist.Epc_CurrentPayPeriod = A.Ept_CurrentPayPeriod
                                            WHERE (CalcHist.Epc_EmployeeId IS NOT NULL
													OR CalcAnnual.Epc_EmployeeId IS NOT NULL)", ProcessPayrollPeriod, EmployeeCondition);
            #endregion
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public void InsertEmployeeDailyAllowance(string EmployeeId, string AllowanceCode, decimal AllowanceAmt, string Cycle, string UserLogin)
        {
            string query;

            //Check if exist
            query = string.Format(@"SELECT * FROM [T_EmployeeDailyAllowance]
                                    WHERE [Eda_EmployeeId] = '{0}'
                                      AND [Eda_CurrentPayPeriod] = '{1}'
                                      AND [Eda_AllowanceCode] = '{2}'", EmployeeId, ProcessPayrollPeriod, AllowanceCode);
            DataTable dtResult = dal.ExecuteDataSet(query).Tables[0];

            if (dtResult.Rows.Count > 0)
            {
                #region update query
                query = string.Format(@"UPDATE [T_EmployeeDailyAllowance]
                                        SET [Eda_AllowanceAmt] = [Eda_AllowanceAmt] + {3}
                                            ,[Usr_Login] = '{4}'
                                            ,[Ludatetime] = getdate()
                                        WHERE [Eda_EmployeeId] = '{0}'
                                          AND [Eda_CurrentPayPeriod] = '{1}'
                                          AND [Eda_AllowanceCode] = '{2}'", EmployeeId, ProcessPayrollPeriod, AllowanceCode, AllowanceAmt, UserLogin);
                #endregion
            }
            else
            {
                #region insert query
                query = string.Format(@"INSERT INTO [T_EmployeeDailyAllowance]
                                                   ([Eda_EmployeeId]
                                                   ,[Eda_CurrentPayPeriod]
                                                   ,[Eda_AllowanceCode]
                                                   ,[Eda_PayrollPost]
                                                   ,[Eda_AllowanceAmt]
                                                   ,[Eda_Cycle]
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
                                                   ,getdate())", EmployeeId, ProcessPayrollPeriod, AllowanceCode, AllowanceAmt, Cycle, UserLogin);
                #endregion
            }
            dal.ExecuteNonQuery(query);
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

        public DataTable GetDayCodeFillers()
        {
            string strQueryDayCodeFillers = @"SELECT Dcf_FillerSeq, Dcf_DayCode, Dcf_Restday FROM t_DayCodeFiller WHERE Dcf_Status = 'A'";

            DataTable dtResult;
            dtResult = dal.ExecuteDataSet(strQueryDayCodeFillers).Tables[0];
            return dtResult;
        }

        public DataTable GetPayPeriodCycle(string PayPeriod)
        {
            string strQuery = string.Format(@"SELECT	Ppm_PayPeriod
		                                                ,Ppm_StartCycle
		                                                ,Ppm_EndCycle
                                                FROM	T_PayPeriodMaster 
                                                WHERE	Ppm_PayPeriod = '{0}' 
                                                AND		Ppm_Status = 'A'", PayPeriod);

            DataTable dtResult;
            using (DALHelper dal = new DALHelper(MainDB, false))
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(strQuery).Tables[0];
                dal.CloseDB();
            }

            return dtResult;
        }

        public void ClearAdjustmentTables(bool ProcessAll, string EmployeeId)
        {
            string query = @"DELETE FROM T_LaborHrsAdjustment WHERE Lha_AdjustpayPeriod = '{0}' {1}
                             DELETE FROM T_EmployeeDailyAllowance WHERE Eda_CurrentPayPeriod = '{0}' and Eda_Cycle = 'P' {2}";
            if (!ProcessAll && EmployeeId != "")
                query = string.Format(query
                                        , ProcessPayrollPeriod
                                        , " AND Lha_EmployeeID = '" + EmployeeId + "'"
                                        , " AND Eda_EmployeeID = '" + EmployeeId + "'");
            else
                query = string.Format(query, ProcessPayrollPeriod, "", "");
            dal.ExecuteNonQuery(query);

            if (bHasDayCodeExt)
            {
                query = @"DELETE FROM T_LaborHrsAdjustmentExt WHERE Lha_AdjustpayPeriod = '{0}' {1}";
                if (!ProcessAll && EmployeeId != "")
                    query = string.Format(query
                                            , ProcessPayrollPeriod
                                            , " AND Lha_EmployeeID = '" + EmployeeId + "'");
                else
                    query = string.Format(query, ProcessPayrollPeriod, "");
                dal.ExecuteNonQuery(query);
            }
        }

        public void CreateLaborHoursAdjRecords(bool ProcessAll, string EmployeeId)
        {
            string EmployeeCondition = "";
            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition += " AND Ell_EmployeeID = '" + EmployeeId + "'";

            #region query
            string query = string.Format(@"INSERT INTO T_LaborHrsAdjustment
                                           SELECT Ell_EmployeeId 
                                                , Ell_AdjustpayPeriod
                                                , Ell_PayPeriod
                                                , Ell_ProcessDate  
                                                , Ell_RegularHr = 0.00  
                                                , Ell_RegularOTHr = 0.00 
                                                , Ell_RegularNDHr = 0.00
                                                , Ell_RegularOTNDHr = 0.00
                                                , Ell_RestdayHr = 0.00  
                                                , Ell_RestdayOTHr = 0.00  
                                                , Ell_RestdayNDHr = 0.00 
                                                , Ell_RestdayOTNDHr = 0.00 
                                                , Ell_LegalHolidayHr = 0.00  
                                                , Ell_LegalHolidayOTHr = 0.00  
                                                , Ell_LegalHolidayNDHr = 0.00 
                                                , Ell_LegalHolidayOTNDHr = 0.00 
                                                , Ell_SpecialHolidayHr = 0.00  
                                                , Ell_SpecialHolidayOTHr = 0.00  
                                                , Ell_SpecialHolidayNDHr = 0.00 
                                                , Ell_SpecialHolidayOTNDHr = 0.00 
                                                , Ell_PlantShutdownHr = 0.00  
                                                , Ell_PlantShutdownOTHr = 0.00  
                                                , Ell_PlantShutdownNDHr = 0.00  
                                                , Ell_PlantShutdownOTNDHr = 0.00  
                                                , Ell_CompanyHolidayHr = 0.00  
                                                , Ell_CompanyHolidayOTHr = 0.00  
                                                , Ell_CompanyHolidayNDHr = 0.00  
                                                , Ell_CompanyHolidayOTNDHr = 0.00         
                                                , Ell_RestdayLegalHolidayHr = 0.00  
                                                , Ell_RestdayLegalHolidayOTHr = 0.00  
                                                , Ell_RestdayLegalHolidayNDHr = 0.00
                                                , Ell_RestdayLegalHolidayOTNDHr = 0.00
                                                , Ell_RestdaySpecialHolidayHr = 0.00  
                                                , Ell_RestdaySpecialHolidayOTHr = 0.00
                                                , Ell_RestdaySpecialHolidayNDHr = 0.00 
                                                , Ell_RestdaySpecialHolidayOTNDHr = 0.00
                                                , Ell_RestdayCompanyHolidayHr = 0.00
                                                , Ell_RestdayCompanyHolidayOTHr = 0.00
                                                , Ell_RestdayCompanyHolidayNDHr = 0.00
                                                , Ell_RestdayCompanyHolidayOTNDHr = 0.00
                                                , Lha_RestdayPlantShutdownHr = 0.00
                                                , Lha_RestdayPlantShutdownOTHr = 0.00
                                                , Lha_RestdayPlantShutdownNDHr = 0.00
                                                , Lha_RestdayPlantShutdownOTNDHr = 0.00
                                                , Ell_LaborHrsAdjustmentAmt = 0.00
                                                , 0 as SalaryRate
                                                , 0 as HourlyRate
                                                , Ell_PayrollPost = 0
                                                , Usr_Login  = '{0}'
                                                , Ludatetime = getdate() 
                                            FROM	T_EmployeeLogLedgerTrail
                                            WHERE	Ell_AdjustpayPeriod = '{1}' {2}", LoginUser, ProcessPayrollPeriod, EmployeeCondition);
            #endregion
            dal.ExecuteNonQuery(query);

            if (bHasDayCodeExt)
            {
                #region query
                query = string.Format(@"INSERT INTO T_LaborHrsAdjustmentExt
                                       SELECT Ell_EmployeeId 
                                            , Ell_AdjustpayPeriod
                                            , Ell_PayPeriod
                                            , Ell_ProcessDate 
                                            , Lha_Filler01_Hr = 0.00
                                            , Lha_Filler01_OTHr = 0.00
                                            , Lha_Filler01_NDHr = 0.00
                                            , Lha_Filler01_OTNDHr = 0.00
                                            , Lha_Filler02_Hr = 0.00
                                            , Lha_Filler02_OTHr = 0.00
                                            , Lha_Filler02_NDHr = 0.00
                                            , Lha_Filler02_OTNDHr = 0.00
                                            , Lha_Filler03_Hr = 0.00
                                            , Lha_Filler03_OTHr = 0.00
                                            , Lha_Filler03_NDHr = 0.00
                                            , Lha_Filler03_OTNDHr = 0.00
                                            , Lha_Filler04_Hr = 0.00
                                            , Lha_Filler04_OTHr = 0.00
                                            , Lha_Filler04_NDHr = 0.00
                                            , Lha_Filler04_OTNDHr = 0.00
                                            , Lha_Filler05_Hr = 0.00
                                            , Lha_Filler05_OTHr = 0.00
                                            , Lha_Filler05_NDHr = 0.00
                                            , Lha_Filler05_OTNDHr = 0.00
                                            , Lha_Filler06_Hr = 0.00
                                            , Lha_Filler06_OTHr = 0.00
                                            , Lha_Filler06_NDHr = 0.00
                                            , Lha_Filler06_OTNDHr = 0.00
                                            , Usr_Login  = '{0}'
                                            , Ludatetime = getdate() 
                                        FROM	T_EmployeeLogLedgerTrail 
                                        WHERE	Ell_AdjustpayPeriod = '{1}' {2}", LoginUser, ProcessPayrollPeriod, EmployeeCondition);
                #endregion
                dal.ExecuteNonQuery(query);
            }
        }

        public void InsertToDailyAllowance(bool ProcessAll, string EmployeeId)
        {
            string EmployeeCondition = "";
            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition += " AND Lha_EmployeeID = '" + EmployeeId + "'";

            #region query
            string query = string.Format(@"Declare @SqlScript1 varchar(max)
                                            Declare @SqlScript2 varchar(max)
                                            Declare @Cnt smallint
                                            Declare @Code varchar(5)
                                            Declare @Payperiod varchar(10)
                                            Declare @user varchar(40)

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
	                                            insert into T_EmployeeDailyAllowance
	                                            select
	                                            T_LaborHrsAdjustment.Lha_EmployeeId
	                                            , '''+ @Payperiod + '''
	                                            , Alh_AllowanceAdjCode
	                                            , ''false''
	                                            , (isnull(sum(T_EmployeeLogLedgerHist.Ell_AllowanceAmt'+ @Code +'),0) - isnull(sum(T_EmployeeLogLedgerTrail.Ell_AllowanceAmt'+ @Code +'),0))
	                                            , ''P''
	                                            , '''+ @user + '''
	                                            , GetDate()
	                                            FROM T_LaborHrsAdjustment 
	                                            INNER JOIN T_EmployeeLogLedgerTrail on T_EmployeeLogLedgerTrail.Ell_EmployeeId  = T_LaborHrsAdjustment.Lha_EmployeeId
		                                            and T_EmployeeLogLedgerTrail.Ell_ProcessDate = T_LaborHrsAdjustment.Lha_ProcessDate   
		                                            and T_EmployeeLogLedgerTrail.Ell_AdjustpayPeriod  = T_LaborHrsAdjustment.Lha_AdjustpayPeriod   
	                                            INNER JOIN T_EmployeeLogLedgerHist on T_EmployeeLogLedgerHist.Ell_EmployeeId   = T_LaborHrsAdjustment.Lha_EmployeeId  
		                                            and T_EmployeeLogLedgerHist.Ell_ProcessDate = T_LaborHrsAdjustment.Lha_ProcessDate  
	                                            INNER JOIN T_Allowanceheader ON Alh_LedgerAlwCol = ''' + @Code + '''
	                                            WHERE T_EmployeeLogLedgerTrail.Ell_AdjustpayPeriod = ''' + @Payperiod + '''
	                                            AND (isnull(T_EmployeeLogLedgerHist.Ell_AllowanceAmt'+ @Code + ',0) - isnull(T_EmployeeLogLedgerTrail.Ell_AllowanceAmt'+ @Code + ',0)) <> 0
                                                {2}
                                            group by T_LaborHrsAdjustment.Lha_EmployeeId,Alh_AllowanceAdjCode'
                                            end

                                            while (@Cnt < 12)
                                            begin
	                                            set @Cnt = @Cnt + 1
	                                            set @Code = (select right('0' + convert(varchar(2),@Cnt) ,2))
	                                            set @SqlScript2 = @SqlScript2 + 
	                                            '
	                                            insert into T_EmployeeDailyAllowance
	                                            select
	                                            T_LaborHrsAdjustment.Lha_EmployeeId
	                                            , '''+ @Payperiod + '''
	                                            , Alh_AllowanceAdjCode
	                                            , ''false''
	                                            , (isnull(sum(T_EmployeeLogLedgerHist.Ell_AllowanceAmt'+ @Code +'),0) - isnull(sum(T_EmployeeLogLedgerTrail.Ell_AllowanceAmt'+ @Code +'),0))
	                                            , ''P''
	                                            , '''+ @user + '''
	                                            , GetDate()
	                                            FROM T_LaborHrsAdjustment 
	                                            INNER JOIN T_EmployeeLogLedgerTrail on T_EmployeeLogLedgerTrail.Ell_EmployeeId  = T_LaborHrsAdjustment.Lha_EmployeeId
		                                            and T_EmployeeLogLedgerTrail.Ell_ProcessDate = T_LaborHrsAdjustment.Lha_ProcessDate   
		                                            and T_EmployeeLogLedgerTrail.Ell_AdjustpayPeriod  = T_LaborHrsAdjustment.Lha_AdjustpayPeriod   
	                                            INNER JOIN T_EmployeeLogLedgerHist on T_EmployeeLogLedgerHist.Ell_EmployeeId   = T_LaborHrsAdjustment.Lha_EmployeeId  
		                                            and T_EmployeeLogLedgerHist.Ell_ProcessDate = T_LaborHrsAdjustment.Lha_ProcessDate  
	                                            INNER JOIN T_Allowanceheader ON Alh_LedgerAlwCol = ''' + @Code + '''
	                                            WHERE T_EmployeeLogLedgerTrail.Ell_AdjustpayPeriod = ''' + @Payperiod + '''
	                                            AND (isnull(T_EmployeeLogLedgerHist.Ell_AllowanceAmt'+ @Code + ',0) - isnull(T_EmployeeLogLedgerTrail.Ell_AllowanceAmt'+ @Code + ',0)) <> 0
                                                {2}
                                            group by T_LaborHrsAdjustment.Lha_EmployeeId,Alh_AllowanceAdjCode'
                                            end

                                            exec (@SqlScript1 + @SqlScript2)", ProcessPayrollPeriod, LoginUser, EmployeeCondition);
            #endregion
            dal.ExecuteNonQuery(query);
        }

        public DataSet GetEmployeeListForLaborHoursAdjustment(string PayPeriod)
        {
            try
            {
                DataSet ds;
                DALHelper dalHelper = new DALHelper(MainDB, false);
                ds = dalHelper.ExecuteDataSet(string.Format(@"SELECT	'False' AS 'Select'
                                                                        , Emt_EmployeeID AS IDNumber
                                                                        , Emt_LastName AS LastName
                                                                        , Emt_FirstName AS FirstName
                                                                        , Emt_JobStatus
                                                                        , JobStatus.Adt_AccountDesc AS JobStatus
                                                                        , Emt_EmploymentStatus
                                                                        , EmploymentStatus.Adt_AccountDesc AS EmploymentStatus
                                                                        , Emt_PayrollType
                                                                        , PayrollType.Adt_AccountDesc AS PayrollType
                                                                        , Emt_PositionCode
                                                                        , Position.Adt_AccountDesc AS Position
                                                                        , Emt_PayrollStatus AS PayrollStatus
                                                                        , Emt_CostCenterCode
                                                                        , dbo.getCostCenterFullNameV2(dbo.T_EmployeeMaster.Emt_CostCenterCode) AS CostCenter
                                                                FROM	T_EmployeeMaster 
                                                                LEFT JOIN T_ShiftCodeMaster
                                                                    ON Scm_ShiftCode = Emt_Shiftcode	
                                                                LEFT OUTER JOIN T_AccountDetail AS EmploymentStatus 
                                                                    ON EmploymentStatus.Adt_AccountCode = T_EmployeeMaster.Emt_EmploymentStatus AND 
                                                                    EmploymentStatus.Adt_AccountType = 'EMPSTAT' 
                                                                LEFT OUTER JOIN T_AccountDetail AS Position 
                                                                    ON Position.Adt_AccountCode = T_EmployeeMaster.Emt_PositionCode AND 
                                                                    Position.Adt_AccountType = 'POSITION' 	
                                                                LEFT OUTER JOIN T_AccountDetail AS PayrollType 
                                                                    ON PayrollType.Adt_AccountCode = T_EmployeeMaster.Emt_PayrollType AND 
                                                                    PayrollType.Adt_AccountType = 'PAYTYPE' 
                                                                LEFT OUTER JOIN T_AccountDetail AS JobStatus 
                                                                    ON JobStatus.Adt_AccountCode = T_EmployeeMaster.Emt_JobStatus AND 
                                                                    JobStatus.Adt_AccountType = 'JOBSTATUS'
                                                                WHERE Emt_EmployeeID IN (
											                        SELECT DISTINCT Ell_EmployeeId FROM T_EmployeeLogLedgerTrail
											                        WHERE Ell_AdjustpayPeriod = '{0}'
                                                                )
                                                                ORDER BY Emt_LastName", PayPeriod));
                return ds;
            }
            catch (Exception err)
            {
                throw new Exception("Error: " + err.Message);
            }
        }
    }
}
