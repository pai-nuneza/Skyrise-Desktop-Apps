using System;
using System.Collections.Generic;
using Payroll.DAL;
using CommonLibrary;
using System.Data;

namespace Payroll.BLogic
{
    public class NewAlphalistProcessingBL : BaseBL
    {
        #region Class Variables
        DALHelper dalCentral;
        CommonBL commonBL;
        string MenuCode     = "";
        string LoginUser    = "";
        string TaxYear      = "";
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

        public override System.Data.DataSet FetchAll()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override System.Data.DataRow Fetch(string code)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region Event handlers for Batch Alphalist process
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

        #region Main Function
        public DataTable ProcessAlphalist(string taxYear, bool ProcessAll, bool PostHoldPayroll, string EmployeeID, string EmployeeList, string loginUser, string CompanyCode, string CentralProfile, string LoginDBName, string menuCode, DALHelper DALCentral)
        {
            return ProcessAlphalist(taxYear, ProcessAll, false, PostHoldPayroll, EmployeeID, EmployeeList, loginUser, CompanyCode, CentralProfile, "", LoginDBName, "", menuCode, DALCentral);
        }

        public DataTable ProcessAlphalist(string taxYear, bool ProcessAll, bool IsLastPay, bool PostHoldPayroll, string EmployeeID, string EmployeeList, string loginUser, string CompanyCode, string CentralProfile, string PayCycleSeparated, string LoginDBName, string FinalPayTaxMethod, string menuCode, DALHelper DALCentral)
        {
            #region Variables
            DataTable dtEmployees;
            DataTable dtBeneficiaries;
            DataRow[] drArrBeneficiaries;
            DataTable dtW2Header;
            DataRow drW2Header;
            DataTable dtW2Detail;
            DataRow drW2Detail;
            DataTable dtAlphalist;
            DataRow drAlphalist;
            DataTable dtUserGeneratedW2 = new DataTable();
            DataRow[] drArrUserGeneratedW2;
            DataTable dtAnnualizedPayrollDec2ndQ = new DataTable();
            DataRow[] drArrAnnualizedPayrollDec2ndQ;
            DataTable dtYTDPrevious = new DataTable();
            DataRow[] drArrYTDPrevious;
            DataTable dtW2StagingPayrollData = new DataTable();
            DataRow[] drArrW2StagingPayrollData;
            DataTable dtW2StagingPayrollDataYTDPrev = new DataTable();
            DataRow[] drArrW2StagingPayrollDataYTDPrev;
            DataTable dtW2StagingWTaxData = new DataTable();
            DataRow[] drArrW2StagingWTaxData;
            DataTable dtW2StagingWTaxDecData = new DataTable();
            DataTable dtTaxExemptionTable = new DataTable();
            DataRow[] drArrTaxExemptionTable;
            DataTable dtTaxMaster = new DataTable();
            DataTable dtErrList = new DataTable();
            DataTable dtCompany;

            DALHelper dalTemp = new DALHelper(CentralProfile, false);
            decimal M13EXCLTAX                  = 0;
            decimal MDIVISOR                    = 0;
            #region M_Tax Settings
            string HIREDATE                     = "";
            string WTAXCUTOFF                   = "";
            string ASSUME                       = "";
            string RCURALPHACATGY               = "";
            string BIRFORMTAXYEAR               = "";
            string BIREMPSTAT                   = "";
            string BIRSEPARATIONCODE            = "";
            string BIREMPSTATTOPROCESS          = "";
            #endregion
            string curEmployeeId                = "";
            string strCurrentPayPeriod          = "";
            bool bIsAnnualizedDec2ndQ           = false;
            bool bEmployeeWithAnnualizedDec2ndQ = false;
            decimal dTotal13thMonthPres         = 0;
            decimal dTotal13thMonthPrev         = 0;
            decimal dHalfMonthAmt               = 0;
            decimal dSalaryRate                 = 0;
            int iPayPeriodDiv                   = 0;
            decimal dEstBasic                   = 0;
            decimal dEst13thMonth               = 0;
            decimal dEstRecurring               = 0;
            decimal dEstSSSPrem                 = 0;
            decimal dEstMPFPrem                 = 0;
            decimal dEstPHPrem                  = 0;
            decimal dEstHDMFPrem                = 0;
            decimal dEstUNIONDUESPrem           = 0;
            DataTable dtPremiumContribution     = null;
            DataRow[] drArrPremiumContribution;
            DataTable dtAnnualizedTax;
            string ProcessErrorMessage          = "";
            #endregion

            #region Processing
            try
            {
                this.dalCentral = DALCentral;
                this.commonBL   = new CommonBL();
                this.MenuCode   = menuCode;
                this.LoginUser  = loginUser;
                this.TaxYear    = taxYear;
                InitializeErrorList();

                StatusHandler(this, new StatusEventArgs("Get Parameters", false));

                dtTaxMaster = GetTaxMaster(CompanyCode, CentralProfile);
                if (dtTaxMaster.Rows.Count == 0)
                    ProcessErrorMessage += "No Tax Master set-up.\n";

                #region T_LastTransaction - TAXYEAR
                string TAXYEAR_LASTTRANS = "";
                string sqlLastTransaction = string.Format(@"SELECT Tlt_LastEffectivity FROM {0}..T_LastTransaction WHERE Tlt_TransactionCode = 'TAXYEAR'", LoginDBName);
                DataTable dtLastTran = dalCentral.ExecuteDataSet(sqlLastTransaction).Tables[0];
                if (dtLastTran != null && dtLastTran.Rows.Count > 0)
                    TAXYEAR_LASTTRANS = dtLastTran.Rows[0][0].ToString();

                if (TAXYEAR_LASTTRANS == "")
                    ProcessErrorMessage += "TAXYEAR is not set-up in T_LastTransaction table.\n";
                #endregion

                StatusHandler(this, new StatusEventArgs("Get Parameters", true));

                if (ProcessErrorMessage != "")
                    throw new Exception(ProcessErrorMessage);

                #region Get Parameters
                M13EXCLTAX = Convert.ToDecimal(commonBL.GetParameterValueFromPayroll("M13EXCLTAX", CompanyCode, LoginDBName, dalCentral));
                MDIVISOR = Convert.ToDecimal(commonBL.GetParameterValueFromPayroll("MDIVISOR", CompanyCode, LoginDBName, dalCentral));

                HIREDATE            = dtTaxMaster.Rows[0]["Mtx_HireDateCutOff"].ToString(); 
                WTAXCUTOFF          = dtTaxMaster.Rows[0]["Mtx_TaxWithheldCutOff"].ToString(); 
                RCURALPHACATGY      = dtTaxMaster.Rows[0]["Mtx_RecurringAllowanceBIRreporting"].ToString();
                ASSUME              = dtTaxMaster.Rows[0]["Mtx_AssumeRemainingCycle"].ToString();
                BIRFORMTAXYEAR      = dtTaxMaster.Rows[0]["Mtx_BIRNewFormTaxYear"].ToString();
                BIREMPSTAT          = dtTaxMaster.Rows[0]["Mtx_BIREmploymentStatusCode"].ToString();
                BIRSEPARATIONCODE   = dtTaxMaster.Rows[0]["Mtx_BIRSeparationCode"].ToString();
                BIREMPSTATTOPROCESS = dtTaxMaster.Rows[0]["Mtx_BIREmploymentStatusToProcess"].ToString();
                #endregion

                dtW2Header = DbRecord.GenerateTable(CommonConstants.TableName.T_EmpAlphalistHdr, dalTemp);
                dtW2Detail = DbRecord.GenerateTable(CommonConstants.TableName.T_EmpAlphalistDtl, dalTemp);
                dtAlphalist = DbRecord.GenerateTable(CommonConstants.TableName.T_Alphalist, dalTemp);

                StatusHandler(this, new StatusEventArgs("Get Employees for Processing", false));
                dtEmployees = GetEmployees(ProcessAll, IsLastPay, EmployeeID, EmployeeList, taxYear, HIREDATE + "/" + taxYear, CompanyCode, LoginDBName, BIREMPSTATTOPROCESS, CentralProfile);
                dtBeneficiaries = GetBeneficiaries(ProcessAll, EmployeeID, EmployeeList, CompanyCode);
                StatusHandler(this, new StatusEventArgs("Get Employees for Processing", true));

                #region Get Company Record
                StatusHandler(this, new StatusEventArgs("Get Company Record", false));
                string sqlCompany = string.Format(@"SELECT Mcm_CompanyName,Mcm_TIN,Mcm_BusinessAddress,Mlh_ZipCode AS Mcm_MunicipalityCity FROM M_Company LEFT JOIN M_LocationHdr ON Mlh_CompanyCode = Mcm_CompanyCode  AND Mlh_LocationCode = Mcm_LocationCode WHERE Mcm_CompanyCode = '{0}'", CompanyCode);
                dtCompany = dalCentral.ExecuteDataSet(sqlCompany).Tables[0];
                StatusHandler(this, new StatusEventArgs("Get Company Record", true));
                #endregion

                #region Initialize Alphalist Settings
                drAlphalist = dtAlphalist.NewRow();
                drAlphalist["Tal_CompanyCode"]                  = CompanyCode;
                drAlphalist["Tal_TaxYear"]                      = TaxYear;
                drAlphalist["Tal_AssumeRemainingCycle"]         = ASSUME;
                drAlphalist["Tal_Assume13thMonthCutOff"]        = dtTaxMaster.Rows[0]["Mtx_Assume13thMonthCutOff"].ToString();
                drAlphalist["Tal_AssumeRecurringAllowance"]     = dtTaxMaster.Rows[0]["Mtx_AssumeRecurringAllowance"].ToString();
                drAlphalist["Tal_MonthlyPremiumUpToMax"]        = dtTaxMaster.Rows[0]["Mtx_MonthlyPremiumUpToMax"].ToString();
                drAlphalist["Tal_HireDateCutOff"]               = HIREDATE;
                drAlphalist["Tal_TaxWithheldCutOff"]            = WTAXCUTOFF;
                drAlphalist["Tal_MWERegion"]                    = dtTaxMaster.Rows[0]["Mtx_MWERegion"].ToString();
                drAlphalist["Tal_MWEBasicPerDay"]               = dtTaxMaster.Rows[0]["Mtx_MWEBasicPerDay"].ToString();
                drAlphalist["Tal_MWEDaysPerYear"]               = dtTaxMaster.Rows[0]["Mtx_MWEDaysPerYear"].ToString();
                drAlphalist["Tal_MWEBasicPerMonth"]             = dtTaxMaster.Rows[0]["Mtx_MWEBasicPerMonth"].ToString();
                drAlphalist["Tal_MWEBasicPerYear"]              = dtTaxMaster.Rows[0]["Mtx_MWEBasicPerYear"].ToString();
                drAlphalist["Tal_OtherTaxableSpecify1"]         = dtTaxMaster.Rows[0]["Mtx_OtherTaxableSpecify1"].ToString();
                drAlphalist["Tal_OtherTaxableSpecify2"]         = dtTaxMaster.Rows[0]["Mtx_OtherTaxableSpecify2"].ToString();
                drAlphalist["Tal_SupplementaryTaxableSpecify1"] = dtTaxMaster.Rows[0]["Mtx_SupplementaryTaxableSpecify1"].ToString();
                drAlphalist["Tal_SupplementaryTaxableSpecify2"] = dtTaxMaster.Rows[0]["Mtx_SupplementaryTaxableSpecify2"].ToString();
                drAlphalist["Tal_RecurringAllowanceBIRreporting"] = RCURALPHACATGY;
                drAlphalist["Tal_BIREmploymentStatusCode"]      = BIREMPSTAT;
                drAlphalist["Tal_BIRSeparationCode"]            = BIRSEPARATIONCODE;
                drAlphalist["Tal_BIREmploymentStatusToProcess"] = BIREMPSTATTOPROCESS;
                drAlphalist["Usr_Login"]                        = LoginUser;
                dtAlphalist.Rows.Add(drAlphalist);
                #endregion

                if (dtEmployees.Rows.Count > 0)
                {
                    StatusHandler(this, new StatusEventArgs(string.Format("Get System Year-to-Date Records for year {0}", taxYear), false));
                    PopulateYTDSystem(ProcessAll, IsLastPay, dtCompany, EmployeeID, EmployeeList, taxYear, LoginUser, CompanyCode, CentralProfile);
                    StatusHandler(this, new StatusEventArgs(string.Format("Get System Year-to-Date Records for year {0}", taxYear), true));

                    StatusHandler(this, new StatusEventArgs(string.Format("Get Payroll Records for year {0}", taxYear), false));
                    PopulateW2StagingPayroll(ProcessAll, PostHoldPayroll, IsLastPay, EmployeeID, EmployeeList, taxYear, LoginUser, CompanyCode, CentralProfile);
                    StatusHandler(this, new StatusEventArgs(string.Format("Get Payroll Records for year {0}", taxYear), true));

                    StatusHandler(this, new StatusEventArgs("Get Year-to-Date Records for Previous and Current Employers", false));
                    PopulateW2StagingYTD(ProcessAll, IsLastPay, EmployeeID, EmployeeList, taxYear, LoginUser, CompanyCode, CentralProfile);
                    //dtYTDPresent = GetYTDRecords(ProcessAll, EmployeeID, EmployeeList, TaxYear, "C");
                    dtYTDPrevious = GetYTDRecords(ProcessAll, EmployeeID, EmployeeList, taxYear, "P");
                    StatusHandler(this, new StatusEventArgs("Get Year-to-Date Records for Previous and Current Employers", true));

                    StatusHandler(this, new StatusEventArgs(string.Format("Get Income and Deduction Records for year {0}", taxYear), false));
                    UpdateW2StagingIncomeDeduction(ProcessAll, EmployeeID, EmployeeList, taxYear, LoginUser, CompanyCode, CentralProfile);
                    StatusHandler(this, new StatusEventArgs(string.Format("Get Income and Deduction Records for year {0}", taxYear), true));

                    StatusHandler(this, new StatusEventArgs("Clean Alphalist Tables", false));
                    DeleteW2Records(ProcessAll, IsLastPay, EmployeeID, EmployeeList, taxYear);
                    StatusHandler(this, new StatusEventArgs("Clean Alphalist Tables", true));

                    //StatusHandler(this, new StatusEventArgs("Execute Pre-Alphalist Processing Procedures", false));
                    #region //Pre Alphalist Process Formula
                    //string strPreFormulaQuery = GetW2Formula("PREW2");
                    //if (strPreFormulaQuery != "")
                    //{
                    //    if (!ProcessAll && EmployeeID != "")
                    //        dal.ExecuteNonQuery(strPreFormulaQuery.Replace("@TaxYear", "'" + TaxYear + "'").Replace("@EmployeeID", "'" + EmployeeID + "'").Replace("@EmployeeList", "''").Replace("@IsEmployeeList", (EmployeeList != "") ? "1" : "0"));
                    //    else if (ProcessAll == true && EmployeeList != "")
                    //        dal.ExecuteNonQuery(strPreFormulaQuery.Replace("@TaxYear", "'" + TaxYear + "'").Replace("@EmployeeID", "''").Replace("@EmployeeList", EmployeeList).Replace("@IsEmployeeList", (EmployeeList != "") ? "1" : "0"));
                    //    else if (ProcessAll == true)
                    //        dal.ExecuteNonQuery(strPreFormulaQuery.Replace("@TaxYear", "'" + TaxYear + "'").Replace("@EmployeeID", "''").Replace("@EmployeeList", "''").Replace("@IsEmployeeList", (EmployeeList != "") ? "1" : "0"));
                    //}
                    #endregion
                    //StatusHandler(this, new StatusEventArgs("Execute Pre-Alphalist Processing Procedures", true));

                    StatusHandler(this, new StatusEventArgs("Get December 2nd Quincena Payroll Records", false));
                    DataTable dtPayCycle = commonBL.GetPayCycleCurrent(CentralProfile, CompanyCode, LoginDBName, dalCentral).Tables[0];
                    if (dtPayCycle.Rows.Count > 0)
                    {
                        strCurrentPayPeriod = dtPayCycle.Rows[0]["Pay Cycle"].ToString();
                        bIsAnnualizedDec2ndQ = Convert.ToBoolean(dtPayCycle.Rows[0]["Year End Adjustment"].ToString());
                        if (strCurrentPayPeriod.CompareTo(taxYear + "122") >= 0)
                        {
                            dtAnnualizedPayrollDec2ndQ = GetEmployeeAnnualizedPayrollDec2ndQ(ProcessAll, EmployeeID, EmployeeList, taxYear + "122", LoginDBName);
                        }
                    }
                    
                    dtPremiumContribution = GetPremiumContributionTable(CompanyCode);
                    StatusHandler(this, new StatusEventArgs("Get December 2nd Quincena Payroll Records", true));

                    StatusHandler(this, new StatusEventArgs(string.Format("Get Withholding Tax Records for year {0}", taxYear), false));
                    if (IsLastPay)
                        dtW2StagingWTaxData = GetW2StagingWithholdingTaxDataFP(ProcessAll, EmployeeID, EmployeeList, PayCycleSeparated, LoginDBName, dalCentral);
                    else 
                        dtW2StagingWTaxData = GetW2StagingWithholdingTaxData(ProcessAll, strCurrentPayPeriod, bIsAnnualizedDec2ndQ, EmployeeID, EmployeeList, taxYear, WTAXCUTOFF, false);
                    dtW2StagingWTaxDecData = GetW2StagingWithholdingTaxData(ProcessAll, strCurrentPayPeriod, bIsAnnualizedDec2ndQ, EmployeeID, EmployeeList, taxYear, WTAXCUTOFF, true);
                    
                    dtTaxExemptionTable = GetTaxCodeExemptionTable(CompanyCode, LoginDBName);
                    StatusHandler(this, new StatusEventArgs(string.Format("Get Withholding Tax Records for year {0}", taxYear), true));

                    StatusHandler(this, new StatusEventArgs("Preparing Data from Staging Table", false));
                    dtUserGeneratedW2 = GetEmployeeUserGeneratedW2(ProcessAll, IsLastPay, EmployeeID, EmployeeList, taxYear);
                    dtW2StagingPayrollData        = GetW2StagingPayrollData(ProcessAll, EmployeeID, EmployeeList, taxYear, "");
                    dtW2StagingPayrollDataYTDPrev = GetW2StagingPayrollData(ProcessAll, EmployeeID, EmployeeList, taxYear, "P");
                    StatusHandler(this, new StatusEventArgs("Preparing Data from Staging Table", true));
                }

                for (int i = 0; i < dtEmployees.Rows.Count; i++) 
                {
                    try
                    {
                        curEmployeeId = dtEmployees.Rows[i]["Mem_IDNo"].ToString();

                        #region Initialize Alphalist Header Record
                        drW2Header = dtW2Header.NewRow();
                        drW2Header["Tah_IDNo"]                                  = curEmployeeId;
                        drW2Header["Tah_TaxYear"]                               = TaxYear;
                        if (GetValue(dtEmployees.Rows[i]["Mem_IntakeDate"]) != string.Empty)
                            drW2Header["Tah_StartDate"]                         = dtEmployees.Rows[i]["Mem_IntakeDate"];
                        else
                            drW2Header["Tah_StartDate"]                         = new DateTime(Convert.ToInt32(TaxYear), 1, 1);
                        if (GetValue(dtEmployees.Rows[i]["Mem_SeparationDate"]) != string.Empty)
                            drW2Header["Tah_EndDate"]                           = Convert.ToDateTime(GetValue(dtEmployees.Rows[i]["Mem_SeparationDate"])).AddDays(-1);
                        else
                            drW2Header["Tah_EndDate"]                           = new DateTime(Convert.ToInt32(TaxYear), 12, 31);
                        drW2Header["Tah_LastName"]                              = dtEmployees.Rows[i]["Mem_LastName"].ToString();
                        drW2Header["Tah_FirstName"]                             = dtEmployees.Rows[i]["Mem_FirstName"].ToString();
                        drW2Header["Tah_MiddleName"]                            = dtEmployees.Rows[i]["Mem_MiddleName"].ToString();
                        drW2Header["Tah_ExtensionName"]                         = dtEmployees.Rows[i]["Mem_ExtensionName"].ToString();
                        drW2Header["Tah_Schedule"]                              = "";
                        drW2Header["Tah_TIN"]                                   = dtEmployees.Rows[i]["Mem_TIN"].ToString();
                        drW2Header["Tah_PresCompleteAddress"]                   = dtEmployees.Rows[i]["Mem_PresMailingAddress"].ToString();
                        drW2Header["Tah_PresAddressMunicipalityCity"]           = dtEmployees.Rows[i]["Mem_PresLocationCode"].ToString();
                        drW2Header["Tah_TelephoneNo"]                           = dtEmployees.Rows[i]["Mem_PresContactNo"].ToString();
                        drW2Header["Tah_ProvCompleteAddress"]                   = dtEmployees.Rows[i]["Mem_ProvMailingAddress"].ToString();
                        drW2Header["Tah_ProvAddressMunicipalityCity"]           = dtEmployees.Rows[i]["Mem_ProvLocationCode"].ToString();
                        drW2Header["Tah_WifeClaim"]                             = (dtEmployees.Rows[i]["Mem_WifeClaim"].ToString() != "" && dtEmployees.Rows[i]["Mem_WifeClaim"].ToString() == "Y")? 1 : 0;
                        drW2Header["Tah_MWEBasicCurER"]                         = 0;
                        drW2Header["Tah_MWEHolidayCurER"]                       = 0;
                        drW2Header["Tah_MWEOvertimeCurER"]                      = 0;
                        drW2Header["Tah_MWENightShiftCurER"]                    = 0;
                        drW2Header["Tah_MWEHazardCurER"]                        = 0;
                        drW2Header["Tah_Nontaxable13thMonthCurER"]              = 0;
                        drW2Header["Tah_DeMinimisCurER"]                        = 0;
                        drW2Header["Tah_PremiumsUnionDuesCurER"]                = 0;
                        drW2Header["Tah_NontaxableSalariesCompensationCurER"]   = 0;
                        drW2Header["Tah_TaxableBasicCurER"]                     = 0;
                        drW2Header["Tah_TaxableBasicNetPremiumsCurER"]          = 0;  
                        drW2Header["Tah_TaxableOvertimeCurER"]                  = 0;
                        drW2Header["Tah_Taxable13thMonthCurER"]                 = 0;
                        drW2Header["Tah_TaxableSalariesCompensationCurER"]      = 0;
                        drW2Header["Tah_MWEBasicPrvER"]                         = 0;
                        drW2Header["Tah_MWEHolidayPrvER"]                       = 0;
                        drW2Header["Tah_MWEOvertimePrvER"]                      = 0;
                        drW2Header["Tah_MWENightShiftPrvER"]                    = 0;
                        drW2Header["Tah_MWEHazardPrvER"]                        = 0;
                        drW2Header["Tah_Nontaxable13thMonthPrvER"]              = 0;
                        drW2Header["Tah_DeMinimisPrvER"]                        = 0;
                        drW2Header["Tah_PremiumsUnionDuesPrvER"]                = 0;
                        drW2Header["Tah_NontaxableSalariesCompensationPrvER"]   = 0;
                        drW2Header["Tah_TaxableBasicPrvER"]                     = 0;
                        drW2Header["Tah_TaxableBasicNetPremiumsPrvER"]          = 0;
                        drW2Header["Tah_TaxableOvertimePrvER"]                  = 0;
                        drW2Header["Tah_Taxable13thMonthPrvER"]                 = 0;
                        drW2Header["Tah_TaxableSalariesCompensationPrvER"]      = 0;
                        drW2Header["Tah_RepresentationCurER"]                   = 0;
                        drW2Header["Tah_TransportationCurER"]                   = 0;
                        drW2Header["Tah_CostLivingAllowanceCurER"]              = 0;
                        drW2Header["Tah_FixedHousingAllowanceCurER"]            = 0;
                        drW2Header["Tah_OtherTaxable1CurER"]                    = 0;
                        drW2Header["Tah_OtherTaxable2CurER"]                    = 0;
                        drW2Header["Tah_CommisionCurER"]                        = 0;
                        drW2Header["Tah_ProfitSharingCurER"]                    = 0;
                        drW2Header["Tah_FeesCurER"]                             = 0;
                        drW2Header["Tah_HazardCurER"]                           = 0;
                        drW2Header["Tah_SupplementaryTaxable1CurER"]            = 0;
                        drW2Header["Tah_SupplementaryTaxable2CurER"]            = 0;
                        drW2Header["Tah_RepresentationPrvER"]                   = 0;
                        drW2Header["Tah_TransportationPrvER"]                   = 0;
                        drW2Header["Tah_CostLivingAllowancePrvER"]              = 0;
                        drW2Header["Tah_FixedHousingAllowancePrvER"]            = 0;
                        drW2Header["Tah_OtherTaxable1PrvER"]                    = 0;
                        drW2Header["Tah_OtherTaxable2PrvER"]                    = 0;
                        drW2Header["Tah_CommisionPrvER"]                        = 0;
                        drW2Header["Tah_ProfitSharingPrvER"]                    = 0;
                        drW2Header["Tah_FeesPrvER"]                             = 0;
                        drW2Header["Tah_HazardPrvER"]                           = 0;
                        drW2Header["Tah_SupplementaryTaxable1PrvER"]            = 0;
                        drW2Header["Tah_SupplementaryTaxable2PrvER"]            = 0;
                        drW2Header["Tah_ExemptionCode"]                         = dtEmployees.Rows[i]["Mem_TaxCode"].ToString();
                        drW2Header["Tah_ExemptionAmount"]                       = 0;
                        drW2Header["Tah_PremiumPaidOnHealth"]                   = 0;
                        drW2Header["Tah_GrossCompensationIncome"]               = 0;
                        drW2Header["Tah_NontaxableIncomeCurER"]                 = 0;
                        drW2Header["Tah_NontaxableIncomePrvER"]                 = 0;
                        drW2Header["Tah_TaxableIncomeCurER"]                    = 0;
                        drW2Header["Tah_TaxableIncomePrvER"]                    = 0;
                        drW2Header["Tah_NetTaxableIncome"]                      = 0;
                        drW2Header["Tah_TaxDue"]                                = 0;
                        drW2Header["Tah_TaxWithheldPrvER"]                      = 0;
                        drW2Header["Tah_TaxWithheldCurER"]                      = 0;
                        drW2Header["Tah_TotalTaxWithheldJanDec"]                = 0;
                        drW2Header["Tah_TotalTaxWithheld"]                      = 0;
                        drW2Header["Tah_TaxAmount"]                             = 0;
                        drW2Header["Tah_TaxBracket"]                            = 0; 
                        drW2Header["Tah_IsSubstitutedFiling"]                   = (dtEmployees.Rows[i]["Mem_WorkStatus"].ToString().Substring(0, 1) == "A")? true : false;
                        drW2Header["Tah_IsTaxExempted"]                         = dtEmployees.Rows[i]["Mem_IsTaxExempted"].ToString();
                        drW2Header["Tah_FinalPayIndicator"]                     = (IsLastPay ? true : false);
                        drW2Header["Tah_IsZeroOutLastQuinYear"]                 = false;
                        drW2Header["Tah_CostcenterCode"]                        = dtEmployees.Rows[i]["Mem_CostcenterCode"].ToString();
                        drW2Header["Tah_PayrollGroup"]                          = dtEmployees.Rows[i]["Mem_PayrollGroup"].ToString();
                        drW2Header["Tah_PayrollType"]                           = dtEmployees.Rows[i]["Mem_PayrollType"].ToString();
                        drW2Header["Tah_EmploymentStatus"]                      = dtEmployees.Rows[i]["Mem_EmploymentStatusCode"].ToString();
                        drW2Header["Tah_WorkStatus"]                            = dtEmployees.Rows[i]["Mem_WorkStatus"].ToString();
                        drW2Header["Tah_AssumeBasic"]                           = 0;
                        drW2Header["Tah_Assume13th"]                            = 0;
                        drW2Header["Tah_AssumeSalariesCompensation"]            = 0;
                        drW2Header["Tah_AssumePremiumsUnionDues"]               = 0;
                        drW2Header["Tah_AssumePayCycle"]                        = 0;
                        drW2Header["Tah_CurrentEmploymentStatus"]               = "";
                        drW2Header["Tah_Nationality"]                           = "";
                        drW2Header["Tah_SeparationReason"]                      = "";
                        drW2Header["Usr_Login"]                                 = LoginUser;
                        drW2Header["Ludatetime"]                                = DateTime.Now;
                        #endregion

                        #region Check if Annualized during December 2nd Quincena and Assign Default Values
                        bEmployeeWithAnnualizedDec2ndQ = false;
                        if (bIsAnnualizedDec2ndQ == true && dtAnnualizedPayrollDec2ndQ.Rows.Count > 0)
                        {
                            drArrAnnualizedPayrollDec2ndQ = dtAnnualizedPayrollDec2ndQ.Select(string.Format("Tpy_IDNo = '{0}'", curEmployeeId));
                            if (drArrAnnualizedPayrollDec2ndQ.Length > 0)
                            {
                                if (Convert.ToDecimal(drArrAnnualizedPayrollDec2ndQ[0]["Tpy_TotalExemptions"]) > 0)
                                {
                                    bEmployeeWithAnnualizedDec2ndQ = true;
                                    drW2Header["Tah_IsZeroOutLastQuinYear"] = true;
                                }
                                drW2Header["Tah_ExemptionCode"]         = drArrAnnualizedPayrollDec2ndQ[0]["Tpy_TaxCode"].ToString();
                                drW2Header["Tah_ExemptionAmount"]       = drArrAnnualizedPayrollDec2ndQ[0]["Tpy_TotalExemptions"].ToString();
                                drW2Header["Tah_TaxDue"]                = drArrAnnualizedPayrollDec2ndQ[0]["Tpy_YTDWtaxAmt"].ToString();
                                drW2Header["Tah_IsSubstitutedFiling"]   = (drArrAnnualizedPayrollDec2ndQ[0]["Tpy_WorkStatus"].ToString().Substring(0, 1) == "A") ? true : false;
                                drW2Header["Tah_IsTaxExempted"]         = drArrAnnualizedPayrollDec2ndQ[0]["Tpy_IsTaxExempted"].ToString();
                                drW2Header["Tah_CostcenterCode"]        = drArrAnnualizedPayrollDec2ndQ[0]["Tpy_CostcenterCode"].ToString();
                                drW2Header["Tah_PayrollType"]           = drArrAnnualizedPayrollDec2ndQ[0]["Tpy_PayrollType"].ToString();
                                drW2Header["Tah_WorkStatus"]            = drArrAnnualizedPayrollDec2ndQ[0]["Tpy_WorkStatus"].ToString();
                                drW2Header["Tah_EmploymentStatus"]      = drArrAnnualizedPayrollDec2ndQ[0]["Tpy_EmploymentStatus"].ToString();
                                drW2Header["Tah_PayrollGroup"]          = drArrAnnualizedPayrollDec2ndQ[0]["Tpy_PayrollGroup"].ToString();
                            }
                            else
                                bEmployeeWithAnnualizedDec2ndQ = false;
                        }
                        #endregion

                        #region Set Alphalist Schedule
                        if (BIRFORMTAXYEAR != "" && Convert.ToInt32(taxYear) >= Convert.ToInt32(BIRFORMTAXYEAR))
                        {
                            if (Convert.ToBoolean(drW2Header["Tah_IsTaxExempted"]) == true)
                                drW2Header["Tah_Schedule"] = "SCHED2";
                            else
                                drW2Header["Tah_Schedule"] = "SCHED1";
                        }
                        else
                        {
                            if (drW2Header["Tah_WorkStatus"].ToString() == "IN" || GetValue(dtEmployees.Rows[i]["Mem_SeparationDate"]) != string.Empty)
                            {
                                if (dtEmployees.Rows[i]["Mem_SeparationDate"] != null
                                    && GetValue(dtEmployees.Rows[i]["Mem_SeparationDate"]) != string.Empty
                                    && Convert.ToDateTime(dtEmployees.Rows[i]["Mem_SeparationDate"]) > Convert.ToDateTime(drW2Header["Tah_EndDate"]))
                                    drW2Header["Tah_Schedule"] = "SCHED7.3";
                                else
                                    drW2Header["Tah_Schedule"] = "SCHED7.1";
                            }
                            else
                            {
                                if (Convert.ToBoolean(drW2Header["Tah_IsTaxExempted"]) == false)
                                {
                                    drArrYTDPrevious = dtYTDPrevious.Select(string.Format("Tyt_IDNo = '{0}'", curEmployeeId));
                                    if (drArrYTDPrevious.Length > 0)
                                        drW2Header["Tah_Schedule"] = "SCHED7.4";
                                    else
                                        drW2Header["Tah_Schedule"] = "SCHED7.3";
                                }
                                else
                                    drW2Header["Tah_Schedule"] = "SCHED7.5";
                            }
                        }
                        
                        #endregion

                        drArrUserGeneratedW2 = dtUserGeneratedW2.Select(string.Format("Tah_IDNo = '{0}'", curEmployeeId));
                        if (drArrUserGeneratedW2.Length == 0
                            && (drW2Header["Tah_Schedule"].ToString() != "SCHED7.1" || IsLastPay == true))
                        {
                            drArrW2StagingPayrollData = dtW2StagingPayrollData.Select(string.Format("Tas_IDNo = '{0}'", curEmployeeId));
                            if (drArrW2StagingPayrollData.Length > 0)
                            {
                                #region Actual Payroll and Current Employer
                                drW2Header["Tah_MWEBasicCurER"] = Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_MWEBasic_PAY"])
                                                                        + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_MWEBasic_INC"])
                                                                        + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_MWEBasic_YTD"])
                                                                        + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_MWEBasic_ADJ"]);

                                drW2Header["Tah_MWEHolidayCurER"] = Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_MWEHoliday_PAY"])
                                                                        + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_MWEHoliday_INC"])
                                                                        + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_MWEHoliday_YTD"])
                                                                        + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_MWEHoliday_ADJ"]);

                                drW2Header["Tah_MWEOvertimeCurER"] = Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_MWEOvertime_PAY"])
                                                                        + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_MWEOvertime_INC"])
                                                                        + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_MWEOvertime_YTD"])
                                                                        + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_MWEOvertime_ADJ"]);

                                drW2Header["Tah_MWENightShiftCurER"] = Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_MWENightShift_PAY"])
                                                                        + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_MWENightShift_INC"])
                                                                        + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_MWENightShift_YTD"])
                                                                        + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_MWENightShift_ADJ"]);

                                drW2Header["Tah_MWEHazardCurER"] = Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_MWEHazard_INC"])
                                                                        + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_MWEHazard_YTD"]);


                                //drW2Header["Tah_Taxable13thMonthCurER"] = Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_13thMonthBen_INC"])
                                //                                            + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_13thMonthBen_YTD"]);

                                drW2Header["Tah_TaxableBasicCurER"] = Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_Basic_PAY"])
                                                                           + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_Basic_INC"])
                                                                           + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_Basic_YTD"])
                                                                           + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_Basic_ADJ"]);

                                drW2Header["Tah_TaxableOvertimeCurER"] = Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_Overtime_PAY"])
                                                                            + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_Overtime_INC"])
                                                                            + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_Overtime_YTD"])
                                                                            + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_Overtime_ADJ"]);

                                drW2Header["Tah_RepresentationCurER"] = Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_Representation_INC"])
                                                                        + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_Representation_YTD"]);

                                drW2Header["Tah_TransportationCurER"] = Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_Transportation_INC"])
                                                                        + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_Transportation_YTD"]);

                                drW2Header["Tah_CostLivingAllowanceCurER"] = Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_CostLivingAllowance_INC"])
                                                                            + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_CostLivingAllowance_YTD"]);

                                drW2Header["Tah_FixedHousingAllowanceCurER"] = + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_FixedHousingAllowance_INC"])
                                                                                + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_FixedHousingAllowance_YTD"]);

                                drW2Header["Tah_OtherTaxable1CurER"] = Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_RegularOtherTaxableIncome1_PAY"])
                                                                        + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_RegularOtherTaxableIncome1_INC"])
                                                                        + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_RegularOtherTaxableIncome1_YTD"]);

                                drW2Header["Tah_OtherTaxable2CurER"] = Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_RegularOtherTaxableIncome2_PAY"])
                                                                        + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_RegularOtherTaxableIncome2_INC"])
                                                                        + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_RegularOtherTaxableIncome2_YTD"]);

                                drW2Header["Tah_CommisionCurER"] = Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_Commision_INC"])
                                                                        + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_Commision_YTD"]);

                                drW2Header["Tah_ProfitSharingCurER"] = Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_ProfitSharing_INC"])
                                                                        + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_ProfitSharing_INC"]);

                                drW2Header["Tah_FeesCurER"] = Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_Fees_INC"])
                                                                        + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_Fees_YTD"]);

                                drW2Header["Tah_SupplementaryTaxable1CurER"] = Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_SupplementaryOtherTaxableIncome1_PAY"])
                                                                                + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_SupplementaryOtherTaxableIncome1_INC"])
                                                                                + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_SupplementaryOtherTaxableIncome1_YTD"]);

                                drW2Header["Tah_SupplementaryTaxable2CurER"] = Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_SupplementaryOtherTaxableIncome2_PAY"])
                                                                                + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_SupplementaryOtherTaxableIncome2_INC"])
                                                                                + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_SupplementaryOtherTaxableIncome2_YTD"]);

                                drW2Header["Tah_HazardCurER"] = Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_Hazard_INC"])
                                                                    + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_Hazard_YTD"]);

                                drW2Header["Tah_TaxableSalariesCompensationCurER"] = Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_Representation_INC"])
                                                                                + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_Representation_YTD"])
                                                                                + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_Transportation_INC"])
                                                                                + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_Transportation_YTD"])
                                                                                + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_CostLivingAllowance_INC"])
                                                                                + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_CostLivingAllowance_YTD"])
                                                                                + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_FixedHousingAllowance_INC"])
                                                                                + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_FixedHousingAllowance_YTD"])
                                                                                + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_RegularOtherTaxableIncome1_PAY"])
                                                                                + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_RegularOtherTaxableIncome1_INC"])
                                                                                + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_RegularOtherTaxableIncome1_YTD"])
                                                                                + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_RegularOtherTaxableIncome2_PAY"])
                                                                                + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_RegularOtherTaxableIncome2_INC"])
                                                                                + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_RegularOtherTaxableIncome2_YTD"])
                                                                                + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_Commision_INC"])
                                                                                + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_Commision_YTD"])
                                                                                + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_ProfitSharing_INC"])
                                                                                + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_ProfitSharing_YTD"])
                                                                                + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_Fees_INC"])
                                                                                + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_Fees_YTD"])
                                                                                + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_Hazard_INC"])
                                                                                + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_Hazard_YTD"])
                                                                                + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_SupplementaryOtherTaxableIncome1_PAY"])
                                                                                + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_SupplementaryOtherTaxableIncome1_INC"])
                                                                                + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_SupplementaryOtherTaxableIncome1_YTD"])
                                                                                + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_SupplementaryOtherTaxableIncome2_PAY"])
                                                                                + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_SupplementaryOtherTaxableIncome2_INC"])
                                                                                + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_SupplementaryOtherTaxableIncome2_YTD"]);

                                drW2Header["Tah_Nontaxable13thMonthCurER"] = Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_13thMonthBen_PAY"])
                                                                            + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_13thMonthBen_INC"])
                                                                            + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_13thMonthBen_YTD"]);

                                drW2Header["Tah_DeMinimisCurER"] = Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_DeMinimis_INC"])
                                                                                + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_DeMinimis_YTD"]);

                                drW2Header["Tah_PremiumsUnionDuesCurER"] = Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_Premiums_PAY"])
                                                                                + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_Premiums_INC"])
                                                                                + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_Premiums_DED"])
                                                                                + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_Premiums_YTD"]);

                                drW2Header["Tah_NontaxableSalariesCompensationCurER"] = Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_NontaxableSalaries_INC"])
                                                                                        + Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_NontaxableSalaries_YTD"]);

                                drW2Header["Tah_PremiumPaidOnHealth"] = Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_PremiumPaidOnHealth"]);

                                #endregion
                            }

                            drArrW2StagingPayrollDataYTDPrev = dtW2StagingPayrollDataYTDPrev.Select(string.Format("Tas_IDNo = '{0}'", curEmployeeId));
                            if (drArrW2StagingPayrollDataYTDPrev.Length > 0)
                            {
                                #region Previous Employer
                                drW2Header["Tah_MWEBasicPrvER"]                     = Convert.ToDecimal(drArrW2StagingPayrollDataYTDPrev[0]["Tas_MWEBasic_YTD"]);
                                drW2Header["Tah_MWEHolidayPrvER"]                   = Convert.ToDecimal(drArrW2StagingPayrollDataYTDPrev[0]["Tas_MWEHoliday_YTD"]);
                                drW2Header["Tah_MWEOvertimePrvER"]                  = Convert.ToDecimal(drArrW2StagingPayrollDataYTDPrev[0]["Tas_MWEOvertime_YTD"]);
                                drW2Header["Tah_MWENightShiftPrvER"]                = Convert.ToDecimal(drArrW2StagingPayrollDataYTDPrev[0]["Tas_MWENightShift_YTD"]);
                                drW2Header["Tah_MWEHazardPrvER"]                    = Convert.ToDecimal(drArrW2StagingPayrollDataYTDPrev[0]["Tas_MWEHazard_YTD"]);
                                //drW2Header["Tah_Taxable13thMonthPrvER"]             = Convert.ToDecimal(drArrW2StagingPayrollDataYTDPrev[0]["Tas_13thMonthBen_YTD"]);
                                drW2Header["Tah_TaxableBasicPrvER"]                 = Convert.ToDecimal(drArrW2StagingPayrollDataYTDPrev[0]["Tas_Basic_YTD"]);
                                drW2Header["Tah_TaxableOvertimePrvER"]              = Convert.ToDecimal(drArrW2StagingPayrollDataYTDPrev[0]["Tas_Overtime_YTD"]);
                                drW2Header["Tah_RepresentationPrvER"]               = Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_Representation_YTD"]);
                                drW2Header["Tah_TransportationPrvER"]               = Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_Transportation_YTD"]);
                                drW2Header["Tah_CostLivingAllowancePrvER"]          = Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_CostLivingAllowance_YTD"]);
                                drW2Header["Tah_FixedHousingAllowancePrvER"]        = Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_FixedHousingAllowance_YTD"]);
                                drW2Header["Tah_OtherTaxable1PrvER"]                = Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_RegularOtherTaxableIncome1_YTD"]);
                                drW2Header["Tah_OtherTaxable2PrvER"]                = Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_RegularOtherTaxableIncome2_YTD"]);
                                drW2Header["Tah_CommisionPrvER"]                    = Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_Commision_YTD"]);
                                drW2Header["Tah_ProfitSharingPrvER"]                = Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_ProfitSharing_INC"]);
                                drW2Header["Tah_FeesPrvER"]                         =  Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_Fees_YTD"]);
                                drW2Header["Tah_SupplementaryTaxable1PrvER"]        = Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_SupplementaryOtherTaxableIncome1_YTD"]);
                                drW2Header["Tah_SupplementaryTaxable2PrvER"]        = Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_SupplementaryOtherTaxableIncome2_YTD"]);
                                drW2Header["Tah_HazardPrvER"]                       = Convert.ToDecimal(drArrW2StagingPayrollData[0]["Tas_Hazard_YTD"]);
                                drW2Header["Tah_TaxableSalariesCompensationPrvER"]  = Convert.ToDecimal(drArrW2StagingPayrollDataYTDPrev[0]["Tas_Representation_YTD"])
                                                                                        + Convert.ToDecimal(drArrW2StagingPayrollDataYTDPrev[0]["Tas_Transportation_YTD"])
                                                                                        + Convert.ToDecimal(drArrW2StagingPayrollDataYTDPrev[0]["Tas_CostLivingAllowance_YTD"])
                                                                                        + Convert.ToDecimal(drArrW2StagingPayrollDataYTDPrev[0]["Tas_FixedHousingAllowance_YTD"])
                                                                                        + Convert.ToDecimal(drArrW2StagingPayrollDataYTDPrev[0]["Tas_RegularOtherTaxableIncome1_YTD"])
                                                                                        + Convert.ToDecimal(drArrW2StagingPayrollDataYTDPrev[0]["Tas_RegularOtherTaxableIncome2_YTD"])
                                                                                        + Convert.ToDecimal(drArrW2StagingPayrollDataYTDPrev[0]["Tas_Commision_YTD"])
                                                                                        + Convert.ToDecimal(drArrW2StagingPayrollDataYTDPrev[0]["Tas_ProfitSharing_YTD"])
                                                                                        + Convert.ToDecimal(drArrW2StagingPayrollDataYTDPrev[0]["Tas_Fees_YTD"])
                                                                                        + Convert.ToDecimal(drArrW2StagingPayrollDataYTDPrev[0]["Tas_Hazard_YTD"])
                                                                                        + Convert.ToDecimal(drArrW2StagingPayrollDataYTDPrev[0]["Tas_SupplementaryOtherTaxableIncome1_YTD"])
                                                                                        + Convert.ToDecimal(drArrW2StagingPayrollDataYTDPrev[0]["Tas_SupplementaryOtherTaxableIncome2_YTD"]);

                                drW2Header["Tah_Nontaxable13thMonthPrvER"]          = Convert.ToDecimal(drArrW2StagingPayrollDataYTDPrev[0]["Tas_13thMonthBen_YTD"]);
                                drW2Header["Tah_DeMinimisPrvER"]                    = Convert.ToDecimal(drArrW2StagingPayrollDataYTDPrev[0]["Tas_DeMinimis_YTD"]);
                                drW2Header["Tah_PremiumsUnionDuesPrvER"]            = Convert.ToDecimal(drArrW2StagingPayrollDataYTDPrev[0]["Tas_Premiums_YTD"]);
                                drW2Header["Tah_NontaxableSalariesCompensationPrvER"] = Convert.ToDecimal(drArrW2StagingPayrollDataYTDPrev[0]["Tas_NontaxableSalaries_YTD"]);
                                drW2Header["Tah_TaxWithheldPrvER"]                  = Convert.ToDecimal(drArrW2StagingPayrollDataYTDPrev[0]["Tas_Tax_YTD"]);
                                drW2Header["Tah_PremiumPaidOnHealth"]               = Convert.ToDecimal(drW2Header["Tah_PremiumPaidOnHealth"])
                                                                                        + Convert.ToDecimal(drArrW2StagingPayrollDataYTDPrev[0]["Tas_PremiumPaidOnHealth"]);

                                #endregion
                            }

                            #region Withholding Tax

                            drArrW2StagingWTaxData = dtW2StagingWTaxDecData.Select(string.Format("Tas_IDNo = '{0}'", curEmployeeId));
                            if (drArrW2StagingWTaxData.Length > 0)
                            {
                                drW2Header["Tah_TotalTaxWithheldJanDec"] = Convert.ToDecimal(drArrW2StagingWTaxData[0]["Tas_Tax_PAY"])
                                                                        + Convert.ToDecimal(drArrW2StagingWTaxData[0]["Tas_Tax_INC"])
                                                                        + Convert.ToDecimal(drArrW2StagingWTaxData[0]["Tas_Tax_DED"])
                                                                        + Convert.ToDecimal(drArrW2StagingWTaxData[0]["Tas_Tax_YTD"]);
                            }
                            
                            drArrW2StagingWTaxData = dtW2StagingWTaxData.Select(string.Format("Tas_IDNo = '{0}'", curEmployeeId));
                            if (drArrW2StagingWTaxData.Length > 0)
                            {
                                if (!IsLastPay)
                                {
                                    drW2Header["Tah_TaxWithheldCurER"] = Convert.ToDecimal(drArrW2StagingWTaxData[0]["Tas_Tax_PAY"])
                                                                            + Convert.ToDecimal(drArrW2StagingWTaxData[0]["Tas_Tax_INC"])
                                                                            + Convert.ToDecimal(drArrW2StagingWTaxData[0]["Tas_Tax_DED"])
                                                                             + Convert.ToDecimal(drArrW2StagingWTaxData[0]["Tas_Tax_YTD"]);
                                }
                                else 
                                    drW2Header["Tah_TaxWithheldCurER"] = Convert.ToDecimal(drArrW2StagingWTaxData[0]["Tas_YTDWtaxAmtBefore"]);
                            }

                            if (bIsAnnualizedDec2ndQ == false || bEmployeeWithAnnualizedDec2ndQ == false)
                            {
                                drArrTaxExemptionTable = dtTaxExemptionTable.Select(string.Format("TaxCode = '{0}'", drW2Header["Tah_ExemptionCode"]));
                                if (drArrTaxExemptionTable.Length > 0)
                                {
                                    string strtaxExempt = (drArrTaxExemptionTable[0]["ExemptionAmount"].ToString() == "" ? "0" : drArrTaxExemptionTable[0]["ExemptionAmount"].ToString());
                                    drW2Header["Tah_ExemptionAmount"] = Convert.ToDecimal(strtaxExempt);
                                }
                            }
                            #endregion

                            #region Estimate Values (Assume)
                            if (Convert.ToBoolean(ASSUME) == true 
                                && (bIsAnnualizedDec2ndQ == false || bEmployeeWithAnnualizedDec2ndQ == false)
                                && drW2Header["Tah_Schedule"].ToString() != "SCHED7.1"
                                && !IsLastPay)
                            {
                                dHalfMonthAmt           = 0;
                                dSalaryRate             = 0;
                                dEst13thMonth           = 0;
                                drArrW2StagingWTaxData  = dtW2StagingWTaxDecData.Select(string.Format("Tas_IDNo = '{0}'", curEmployeeId));
                                if (drArrW2StagingWTaxData.Length > 0)
                                {
                                    dSalaryRate = Convert.ToDecimal(drArrW2StagingWTaxData[0]["Tas_SalaryRate"]);
                                    dEst13thMonth = Convert.ToDecimal(drArrW2StagingWTaxData[0]["Tas_TotalREGAmt"]);
                                }

                                MDIVISOR = Convert.ToDecimal(commonBL.GetParameterDtlValueFromPayroll("MDIVISOR", drW2Header["Tah_EmploymentStatus"].ToString(), CompanyCode, LoginDBName, dalCentral));
                                if (drW2Header["Tah_PayrollType"].ToString() == "M")
                                    dHalfMonthAmt = dSalaryRate / 2.0m;
                                else if (drW2Header["Tah_PayrollType"].ToString() == "D")
                                    dHalfMonthAmt = dSalaryRate * MDIVISOR / 12 / 2.0m;
                                else if (drW2Header["Tah_PayrollType"].ToString() == "H")
                                    dHalfMonthAmt = dSalaryRate * 8 * MDIVISOR / 12 / 2.0m;

                                iPayPeriodDiv = GetNextQuincenaCount(strCurrentPayPeriod, taxYear, curEmployeeId, LoginDBName);
                                if (iPayPeriodDiv > 0)
                                {
                                    dEstBasic = Math.Round(iPayPeriodDiv * dHalfMonthAmt, 2);
                                    drW2Header["Tah_AssumeBasic"] = dEstBasic;

                                    dEst13thMonth = Math.Round((dEst13thMonth + dEstBasic) / 12.0m, 2);
                                    drW2Header["Tah_Assume13th"] = dEst13thMonth;

                                    dEstRecurring = Math.Round(GetMonthlyRecurringAllowance(curEmployeeId, strCurrentPayPeriod, CompanyCode) * (iPayPeriodDiv / 2.0m), 2);
                                    drW2Header["Tah_AssumeSalariesCompensation"] = dEstRecurring;

                                    dEstSSSPrem = 0;
                                    dEstMPFPrem = 0;
                                    drArrPremiumContribution = dtPremiumContribution.Select(string.Format("Mps_DeductionCode = 'SSSPREM' AND Mps_CompensationFrom <= {0} AND Mps_PayCycle <= '{1}'", dHalfMonthAmt * 2, strCurrentPayPeriod), "Mps_CompensationFrom DESC");
                                    if (drArrPremiumContribution.Length > 0)
                                    {
                                        dEstSSSPrem = Math.Round(Convert.ToDecimal(drArrPremiumContribution[0]["Mps_EEShare"].ToString()) * (iPayPeriodDiv / 2.0m), 2);
                                        dEstMPFPrem = Math.Round(Convert.ToDecimal(drArrPremiumContribution[0]["Mps_MPFEEShare"].ToString()) * (iPayPeriodDiv / 2.0m), 2);
                                    }

                                    dEstPHPrem = 0;
                                    drArrPremiumContribution = dtPremiumContribution.Select(string.Format("Mps_DeductionCode = 'PHICPREM' AND Mps_CompensationFrom <= {0} AND Mps_PayCycle <= '{1}'", dHalfMonthAmt * 2, strCurrentPayPeriod), "Mps_CompensationFrom DESC");
                                    if (drArrPremiumContribution.Length > 0)
                                    {
                                        dEstPHPrem = Math.Round(Convert.ToDecimal(drArrPremiumContribution[0]["Mps_EEShare"].ToString()) * (iPayPeriodDiv / 2.0m), 2);
                                    }

                                    dEstHDMFPrem = Math.Round(100 * (iPayPeriodDiv / 2.0m));

                                    dEstUNIONDUESPrem = 0;
                                    drArrPremiumContribution = dtPremiumContribution.Select(string.Format("Mps_DeductionCode = 'UNIONDUES' AND Mps_CompensationFrom <= {0} AND Mps_PayCycle <= '{1}'", (dHalfMonthAmt * 2), strCurrentPayPeriod), "Mps_PayCycle DESC, Mps_CompensationFrom DESC");
                                    if (drArrPremiumContribution.Length > 0)
                                    {
                                        dEstUNIONDUESPrem = Math.Round(Convert.ToDecimal(drArrPremiumContribution[0]["Mps_EEShare"].ToString()) * (iPayPeriodDiv / 2.0m), 2);
                                    }

                                    drW2Header["Tah_AssumePremiumsUnionDues"] = Math.Round(dEstSSSPrem + dEstMPFPrem + dEstPHPrem + dEstHDMFPrem + dEstUNIONDUESPrem, 2);
                                    drW2Header["Tah_AssumePayCycle"] = iPayPeriodDiv;

                                    //Add to W2 Main Columns
                                    drW2Header["Tah_TaxableBasicCurER"] = Convert.ToDecimal(drW2Header["Tah_TaxableBasicCurER"])
                                                                                + Convert.ToDecimal(drW2Header["Tah_AssumeBasic"]);
                                    drW2Header["Tah_Nontaxable13thMonthCurER"] = Convert.ToDecimal(drW2Header["Tah_Nontaxable13thMonthCurER"])
                                                                                + Convert.ToDecimal(drW2Header["Tah_Assume13th"]);
                                    drW2Header["Tah_TaxableSalariesCompensationCurER"] = Convert.ToDecimal(drW2Header["Tah_TaxableSalariesCompensationCurER"])
                                                                                + Convert.ToDecimal(drW2Header["Tah_AssumeSalariesCompensation"]);
                                    drW2Header["Tah_PremiumsUnionDuesCurER"] = Convert.ToDecimal(drW2Header["Tah_PremiumsUnionDuesCurER"])
                                                                                + Convert.ToDecimal(drW2Header["Tah_AssumePremiumsUnionDues"]);

                                    //Add Estimate TSALOTH Amount to the defined alphalist category in RCURALPHACATGY parameter
                                    if (RCURALPHACATGY == "T-OTHERTAX1")
                                        drW2Header["Tah_OtherTaxable1CurER"] = Convert.ToDecimal(drW2Header["Tah_OtherTaxable1CurER"])
                                                                                    + Convert.ToDecimal(drW2Header["Tah_AssumeSalariesCompensation"]);
                                    else if (RCURALPHACATGY == "T-OTHERTAX2")
                                        drW2Header["Tah_OtherTaxable2CurER"] = Convert.ToDecimal(drW2Header["Tah_OtherTaxable2CurER"])
                                                                                    + Convert.ToDecimal(drW2Header["Tah_AssumeSalariesCompensation"]);
                                    else if (RCURALPHACATGY == "T-OTHERSUPL1")
                                        drW2Header["Tah_SupplementaryTaxable1CurER"] = Convert.ToDecimal(drW2Header["Tah_SupplementaryTaxable1CurER"])
                                                                                    + Convert.ToDecimal(drW2Header["Tah_AssumeSalariesCompensation"]);
                                    else if (RCURALPHACATGY == "T-OTHERSUPL2")
                                        drW2Header["Tah_SupplementaryTaxable2CurER"] = Convert.ToDecimal(drW2Header["Tah_SupplementaryTaxable2CurER"])
                                                                                    + Convert.ToDecimal(drW2Header["Tah_AssumeSalariesCompensation"]);
                                }
                            }
                            #endregion

                            #region 13th Month Pay Tax Exemption Ceiling Checking
                            dTotal13thMonthPres = Convert.ToDecimal(drW2Header["Tah_Nontaxable13thMonthCurER"])
                                                + Convert.ToDecimal(drW2Header["Tah_Taxable13thMonthCurER"]);
                            dTotal13thMonthPrev = Convert.ToDecimal(drW2Header["Tah_Nontaxable13thMonthPrvER"])
                                                + Convert.ToDecimal(drW2Header["Tah_Taxable13thMonthPrvER"]);

                            if (dTotal13thMonthPres > M13EXCLTAX)
                            {
                                drW2Header["Tah_Nontaxable13thMonthCurER"] = M13EXCLTAX;
                                drW2Header["Tah_Taxable13thMonthCurER"] = dTotal13thMonthPres - M13EXCLTAX;
                            }
                            else
                            {
                                drW2Header["Tah_Nontaxable13thMonthCurER"] = dTotal13thMonthPres;
                                drW2Header["Tah_Taxable13thMonthCurER"] = 0;
                            }

                            if (dTotal13thMonthPrev > (M13EXCLTAX - dTotal13thMonthPres))
                            {
                                if (M13EXCLTAX - dTotal13thMonthPres > 0)
                                {
                                    drW2Header["Tah_Nontaxable13thMonthPrvER"] = M13EXCLTAX - dTotal13thMonthPres;
                                    drW2Header["Tah_Taxable13thMonthPrvER"] = dTotal13thMonthPrev - (M13EXCLTAX - dTotal13thMonthPres);
                                }
                                else
                                {
                                    drW2Header["Tah_Nontaxable13thMonthPrvER"] = 0;
                                    drW2Header["Tah_Taxable13thMonthPrvER"] = dTotal13thMonthPrev;
                                }
                            }
                            else
                            {
                                drW2Header["Tah_Nontaxable13thMonthPrvER"] = dTotal13thMonthPrev;
                                drW2Header["Tah_Taxable13thMonthPrvER"] = 0;
                            }
                            #endregion

                            #region Totals
                            if (Convert.ToDecimal(drW2Header["Tah_TaxableBasicCurER"]) != 0)
                                drW2Header["Tah_TaxableBasicNetPremiumsCurER"] = Convert.ToDecimal(drW2Header["Tah_TaxableBasicCurER"])
                                                                            - Convert.ToDecimal(drW2Header["Tah_PremiumsUnionDuesCurER"]);
                                                                            //- Convert.ToDecimal(drW2Header["Tah_AssumePremiumsUnionDues"])
                            else
                                drW2Header["Tah_TaxableBasicNetPremiumsCurER"] = Convert.ToDecimal(drW2Header["Tah_TaxableBasicCurER"]);

                            if (Convert.ToDecimal(drW2Header["Tah_TaxableBasicPrvER"]) != 0)
                                drW2Header["Tah_TaxableBasicNetPremiumsPrvER"] = Convert.ToDecimal(drW2Header["Tah_TaxableBasicPrvER"])
                                                                            - Convert.ToDecimal(drW2Header["Tah_PremiumsUnionDuesPrvER"]);
                            else
                                drW2Header["Tah_TaxableBasicNetPremiumsPrvER"] = Convert.ToDecimal(drW2Header["Tah_TaxableBasicPrvER"]);

                            drW2Header["Tah_NontaxableIncomeCurER"] = Convert.ToDecimal(drW2Header["Tah_MWEBasicCurER"])
                                                                            + Convert.ToDecimal(drW2Header["Tah_MWEHolidayCurER"])
                                                                            + Convert.ToDecimal(drW2Header["Tah_MWEOvertimeCurER"])
                                                                            + Convert.ToDecimal(drW2Header["Tah_MWENightShiftCurER"])
                                                                            + Convert.ToDecimal(drW2Header["Tah_MWEHazardCurER"])
                                                                            + Convert.ToDecimal(drW2Header["Tah_Nontaxable13thMonthCurER"])
                                                                            + Convert.ToDecimal(drW2Header["Tah_DeMinimisCurER"])
                                                                            + Convert.ToDecimal(drW2Header["Tah_PremiumsUnionDuesCurER"])
                                                                            + Convert.ToDecimal(drW2Header["Tah_NontaxableSalariesCompensationCurER"]);

                            drW2Header["Tah_NontaxableIncomePrvER"] = Convert.ToDecimal(drW2Header["Tah_MWEBasicPrvER"])
                                                                            + Convert.ToDecimal(drW2Header["Tah_MWEHolidayPrvER"])
                                                                            + Convert.ToDecimal(drW2Header["Tah_MWEOvertimePrvER"])
                                                                            + Convert.ToDecimal(drW2Header["Tah_MWENightShiftPrvER"])
                                                                            + Convert.ToDecimal(drW2Header["Tah_MWEHazardPrvER"])
                                                                            + Convert.ToDecimal(drW2Header["Tah_Nontaxable13thMonthPrvER"])
                                                                            + Convert.ToDecimal(drW2Header["Tah_DeMinimisPrvER"])
                                                                            + Convert.ToDecimal(drW2Header["Tah_PremiumsUnionDuesPrvER"])
                                                                            + Convert.ToDecimal(drW2Header["Tah_NontaxableSalariesCompensationPrvER"]);

                            drW2Header["Tah_TaxableIncomeCurER"] = Convert.ToDecimal(drW2Header["Tah_TaxableBasicNetPremiumsCurER"])
                                                                            + Convert.ToDecimal(drW2Header["Tah_TaxableOvertimeCurER"])
                                                                            + Convert.ToDecimal(drW2Header["Tah_Taxable13thMonthCurER"])
                                                                            + Convert.ToDecimal(drW2Header["Tah_TaxableSalariesCompensationCurER"]);

                            drW2Header["Tah_TaxableIncomePrvER"] = Convert.ToDecimal(drW2Header["Tah_TaxableBasicNetPremiumsPrvER"])
                                                                            + Convert.ToDecimal(drW2Header["Tah_TaxableOvertimePrvER"])
                                                                            + Convert.ToDecimal(drW2Header["Tah_Taxable13thMonthPrvER"])
                                                                            + Convert.ToDecimal(drW2Header["Tah_TaxableSalariesCompensationPrvER"]);

                            drW2Header["Tah_GrossCompensationIncome"] = Convert.ToDecimal(drW2Header["Tah_NontaxableIncomeCurER"])
                                                                            + Convert.ToDecimal(drW2Header["Tah_NontaxableIncomePrvER"])
                                                                            + Convert.ToDecimal(drW2Header["Tah_TaxableIncomeCurER"])
                                                                            + Convert.ToDecimal(drW2Header["Tah_TaxableIncomePrvER"]);

                            drW2Header["Tah_NetTaxableIncome"] = Convert.ToDecimal(drW2Header["Tah_TaxableIncomeCurER"])
                                                                            + Convert.ToDecimal(drW2Header["Tah_TaxableIncomePrvER"])
                                                                            - Convert.ToDecimal(drW2Header["Tah_ExemptionAmount"])
                                                                            - Convert.ToDecimal(drW2Header["Tah_PremiumPaidOnHealth"]);
                            if (Convert.ToDecimal(drW2Header["Tah_NetTaxableIncome"]) < 0)
                            {
                                #region Schedule 7.2 Evaluation
                                if (drW2Header["Tah_Schedule"].ToString() == "SCHED7.3")
                                    drW2Header["Tah_Schedule"] = "SCHED7.2";
                                #endregion

                                drW2Header["Tah_NetTaxableIncome"] = 0;
                            }

                            drW2Header["Tah_TotalTaxWithheld"] = Convert.ToDecimal(drW2Header["Tah_TaxWithheldCurER"])
                                                                            + Convert.ToDecimal(drW2Header["Tah_TaxWithheldPrvER"]);
                            #endregion

                            #region Tax Due
                            if (!IsLastPay || FinalPayTaxMethod.Equals("A"))
                            {
                                dtAnnualizedTax = GetAnnualizedTaxAmount(Convert.ToDecimal(drW2Header["Tah_NetTaxableIncome"]), taxYear, CompanyCode);
                                if (dtAnnualizedTax.Rows.Count > 0)
                                {
                                    drW2Header["Tah_TaxDue"] = Math.Round(Convert.ToDecimal(dtAnnualizedTax.Rows[0][0]), 2);
                                    drW2Header["Tah_TaxBracket"] = Convert.ToInt32(dtAnnualizedTax.Rows[0][1]);
                                }

                                drW2Header["Tah_TaxAmount"] = Convert.ToDecimal(drW2Header["Tah_TaxDue"])
                                                            - Convert.ToDecimal(drW2Header["Tah_TotalTaxWithheld"]);
                            }
                            #endregion

                            if (BIRFORMTAXYEAR != "" && Convert.ToInt32(taxYear) >= Convert.ToInt32(BIRFORMTAXYEAR))
                            {
                                drW2Header["Tah_Nationality"] = dtEmployees.Rows[i]["Mem_NationalityCode"].ToString();

                                #region BIREMPSTAT
                                if (!BIREMPSTAT.Equals(""))
                                {
                                    int idxx = 0;
                                    ParameterInfo[] paramEmpStat = new ParameterInfo[3];
                                    paramEmpStat[idxx++] = new ParameterInfo("@IDNUMBER", curEmployeeId);
                                    paramEmpStat[idxx++] = new ParameterInfo("@TAXYEAR", TaxYear);
                                    paramEmpStat[idxx++] = new ParameterInfo("@COMPANYCODE", CompanyCode);

                                    DataTable dtTemp = dalCentral.ExecuteDataSet(BIREMPSTAT.Replace("@PAYROLLDB", LoginDBName), CommandType.Text, paramEmpStat).Tables[0];
                                    if (dtTemp.Rows.Count > 0)
                                        drW2Header["Tah_CurrentEmploymentStatus"] = dtTemp.Rows[0][0].ToString();

                                }
                                #endregion

                                #region BIRSEPARATIONCODE
                                if (!BIRSEPARATIONCODE.Equals(""))
                                {
                                    int idxx = 0;
                                    ParameterInfo[] paramEmpStat = new ParameterInfo[3];
                                    paramEmpStat[idxx++] = new ParameterInfo("@IDNUMBER", curEmployeeId);
                                    paramEmpStat[idxx++] = new ParameterInfo("@TAXYEAR", TaxYear);
                                    paramEmpStat[idxx++] = new ParameterInfo("@COMPANYCODE", CompanyCode);

                                    DataTable dtTemp = dalCentral.ExecuteDataSet(BIRSEPARATIONCODE.Replace("@PAYROLLDB", LoginDBName), CommandType.Text, paramEmpStat).Tables[0];
                                    if (dtTemp.Rows.Count > 0)
                                        drW2Header["Tah_SeparationReason"] = dtTemp.Rows[0][0].ToString();

                                }
                                #endregion
                            }

                            //Add to Alphalist Tables
                            dtW2Header.Rows.Add(drW2Header);

                            if (BIRFORMTAXYEAR != "" && Convert.ToInt32(taxYear) < Convert.ToInt32(BIRFORMTAXYEAR))
                            {
                                #region Create Alphalist Detail Records
                                drArrBeneficiaries = dtBeneficiaries.Select(string.Format("Mef_IDNo = '{0}'", curEmployeeId));
                                for (int j = 0; j < drArrBeneficiaries.Length; j++)
                                {
                                    drW2Detail                      = dtW2Detail.NewRow();
                                    drW2Detail["Tad_IDNo"]          = drArrBeneficiaries[j]["Mef_IDNo"].ToString();
                                    drW2Detail["Tad_TaxYear"]       = taxYear;
                                    drW2Detail["Tad_LineNo"]        = string.Format("{0:D2}", j + 1);
                                    drW2Detail["Tad_LastName"]      = drArrBeneficiaries[j]["Mef_LastName"].ToString();
                                    drW2Detail["Tad_FirstName"]     = drArrBeneficiaries[j]["Mef_FirstName"].ToString();
                                    drW2Detail["Tad_MiddleName"]    = drArrBeneficiaries[j]["Mef_MiddleName"].ToString();
                                    drW2Detail["Tad_Birthdate"]     = drArrBeneficiaries[j]["Mef_BirthDate"];
                                    drW2Detail["Usr_Login"]         = LoginUser;
                                    drW2Detail["Ludatetime"]        = DateTime.Now;

                                    dtW2Detail.Rows.Add(drW2Detail);
                                }
                                #endregion
                            }

                            EmpDispHandler(this, new EmpDispEventArgs(curEmployeeId, dtEmployees.Rows[i]["Mem_LastName"].ToString(), dtEmployees.Rows[i]["Mem_FirstName"].ToString()));
                        }
                        else
                        {
                            if (drW2Header["Tah_Schedule"].ToString() == "SCHED7.1")
                            {
                                EmpDispHandler(this, new EmpDispEventArgs(curEmployeeId, dtEmployees.Rows[i]["Mem_LastName"].ToString(), dtEmployees.Rows[i]["Mem_FirstName"].ToString(), "SCHED7.1 (Excluded)"));
                                AddToErrorList(curEmployeeId, dtEmployees.Rows[i]["Mem_LastName"].ToString(), dtEmployees.Rows[i]["Mem_FirstName"].ToString(), "SCHED7.1 (Excluded from Normal Processing)");
                            }
                            else if (drArrUserGeneratedW2.Length > 0)
                            {
                                EmpDispHandler(this, new EmpDispEventArgs(curEmployeeId, dtEmployees.Rows[i]["Mem_LastName"].ToString(), dtEmployees.Rows[i]["Mem_FirstName"].ToString(), "(Retained Manual Entry)"));
                                AddToErrorList(curEmployeeId, dtEmployees.Rows[i]["Mem_LastName"].ToString(), dtEmployees.Rows[i]["Mem_FirstName"].ToString(), "Retained Manual Entry");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        CommonProcedures.logErrorToFile(e.ToString());
                        EmpDispHandler(this, new EmpDispEventArgs(curEmployeeId, dtEmployees.Rows[i]["Mem_LastName"].ToString(), dtEmployees.Rows[i]["Mem_FirstName"].ToString(), "Error in Alphalist Processing : " + e.Message));
                        AddToErrorList(curEmployeeId, dtEmployees.Rows[i]["Mem_LastName"].ToString(), dtEmployees.Rows[i]["Mem_FirstName"].ToString(), e.Message);
                    }
                }

                StatusHandler(this, new StatusEventArgs("Saving of Alphalist Records", false));
                #region Save Alphalist Header and Detail Records
                string strInsertRecordTemplate = "";
                string strUpdateQuery = "";
                int iUpdateCtr = 0;
                #region Alphalist Header Record Insert
                strInsertRecordTemplate = @" INSERT INTO T_EmpAlphalistHdr (Tah_IDNo,Tah_TaxYear,Tah_StartDate,Tah_EndDate,Tah_LastName,Tah_FirstName,Tah_MiddleName,Tah_ExtensionName,Tah_Schedule,Tah_TIN,Tah_PresCompleteAddress,Tah_PresAddressMunicipalityCity,Tah_TelephoneNo,Tah_ProvCompleteAddress,Tah_ProvAddressMunicipalityCity,Tah_WifeClaim,Tah_MWEBasicCurER,Tah_MWEHolidayCurER,Tah_MWEOvertimeCurER,Tah_MWENightShiftCurER,Tah_MWEHazardCurER,Tah_Nontaxable13thMonthCurER,Tah_DeMinimisCurER,Tah_PremiumsUnionDuesCurER,Tah_NontaxableSalariesCompensationCurER,Tah_TaxableBasicCurER,Tah_TaxableBasicNetPremiumsCurER,Tah_TaxableOvertimeCurER,Tah_Taxable13thMonthCurER,Tah_TaxableSalariesCompensationCurER,Tah_MWEBasicPrvER,Tah_MWEHolidayPrvER,Tah_MWEOvertimePrvER,Tah_MWENightShiftPrvER,Tah_MWEHazardPrvER,Tah_Nontaxable13thMonthPrvER,Tah_DeMinimisPrvER,Tah_PremiumsUnionDuesPrvER,Tah_NontaxableSalariesCompensationPrvER,Tah_TaxableBasicPrvER,Tah_TaxableBasicNetPremiumsPrvER,Tah_TaxableOvertimePrvER,Tah_Taxable13thMonthPrvER,Tah_TaxableSalariesCompensationPrvER,Tah_RepresentationCurER,Tah_TransportationCurER,Tah_CostLivingAllowanceCurER,Tah_FixedHousingAllowanceCurER,Tah_OtherTaxable1CurER,Tah_OtherTaxable2CurER,Tah_CommisionCurER,Tah_ProfitSharingCurER,Tah_FeesCurER,Tah_HazardCurER,Tah_SupplementaryTaxable1CurER,Tah_SupplementaryTaxable2CurER,Tah_RepresentationPrvER,Tah_TransportationPrvER,Tah_CostLivingAllowancePrvER,Tah_FixedHousingAllowancePrvER,Tah_OtherTaxable1PrvER,Tah_OtherTaxable2PrvER,Tah_CommisionPrvER,Tah_ProfitSharingPrvER,Tah_FeesPrvER,Tah_HazardPrvER,Tah_SupplementaryTaxable1PrvER,Tah_SupplementaryTaxable2PrvER,Tah_ExemptionCode,Tah_ExemptionAmount,Tah_PremiumPaidOnHealth,Tah_GrossCompensationIncome,Tah_NontaxableIncomeCurER,Tah_NontaxableIncomePrvER,Tah_TaxableIncomeCurER,Tah_TaxableIncomePrvER,Tah_NetTaxableIncome,Tah_TaxDue,Tah_TaxWithheldPrvER,Tah_TaxWithheldCurER,Tah_TotalTaxWithheld,Tah_TotalTaxWithheldJanDec,Tah_TaxAmount,Tah_IsSubstitutedFiling,Tah_IsTaxExempted,Tah_FinalPayIndicator,Tah_IsZeroOutLastQuinYear,Tah_CostcenterCode,Tah_PayrollGroup,Tah_PayrollType,Tah_EmploymentStatus,Tah_WorkStatus,Tah_AssumeBasic,Tah_Assume13th,Tah_AssumeSalariesCompensation,Tah_AssumePremiumsUnionDues,Tah_AssumePayCycle,Tah_TaxBracket,Tah_CurrentEmploymentStatus,Tah_Nationality,Tah_SeparationReason,Usr_Login,Ludatetime) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}',{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34},{35},{36},{37},{38},{39},{40},{41},{42},{43},{44},{45},{46},{47},{48},{49},{50},{51},{52},{53},{54},{55},{56},{57},{58},{59},{60},{61},{62},{63},{64},{65},{66},{67},'{68}',{69},{70},{71},{72},{73},{74},{75},{76},{77},{78},{79},{80},{81},{82},'{83}','{84}','{85}','{86}','{87}','{88}','{89}','{90}','{91}',{92},{93},{94},{95},{96},{97},'{98}','{99}','{100}','{101}',GETDATE()) ";
                #endregion
                foreach (DataRow drW2Record in dtW2Header.Rows)
                {
                    #region Alphalist Header Insert
                    strUpdateQuery += string.Format(strInsertRecordTemplate
                                                    , drW2Record["Tah_IDNo"]                                                        //0
                                                    , drW2Record["Tah_TaxYear"]                                                     
                                                    , drW2Record["Tah_StartDate"]   
                                                    , drW2Record["Tah_EndDate"]
                                                    , drW2Record["Tah_LastName"].ToString().Trim().Replace("'", "''")
                                                    , drW2Record["Tah_FirstName"].ToString().Trim().Replace("'", "''")              //5
                                                    , drW2Record["Tah_MiddleName"].ToString().Trim().Replace("'", "''")
                                                    , drW2Record["Tah_ExtensionName"].ToString().Trim().Replace("'", "''")
                                                    , drW2Record["Tah_Schedule"]
                                                    , drW2Record["Tah_TIN"]
                                                    , drW2Record["Tah_PresCompleteAddress"].ToString().Trim().Replace("'", "''")    //10
                                                    , drW2Record["Tah_PresAddressMunicipalityCity"]
                                                    , drW2Record["Tah_TelephoneNo"]
                                                    , drW2Record["Tah_ProvCompleteAddress"].ToString().Trim().Replace("'", "''")
                                                    , drW2Record["Tah_ProvAddressMunicipalityCity"]
                                                    , drW2Record["Tah_WifeClaim"]                                                   //15
                                                    , drW2Record["Tah_MWEBasicCurER"]       
                                                    , drW2Record["Tah_MWEHolidayCurER"]
                                                    , drW2Record["Tah_MWEOvertimeCurER"]
                                                    , drW2Record["Tah_MWENightShiftCurER"]
                                                    , drW2Record["Tah_MWEHazardCurER"]                                              //20
                                                    , drW2Record["Tah_Nontaxable13thMonthCurER"]
                                                    , drW2Record["Tah_DeMinimisCurER"]
                                                    , drW2Record["Tah_PremiumsUnionDuesCurER"]
                                                    , drW2Record["Tah_NontaxableSalariesCompensationCurER"]
                                                    , drW2Record["Tah_TaxableBasicCurER"]                                           //25
                                                    , drW2Record["Tah_TaxableBasicNetPremiumsCurER"]
                                                    , drW2Record["Tah_TaxableOvertimeCurER"]
                                                    , drW2Record["Tah_Taxable13thMonthCurER"]
                                                    , drW2Record["Tah_TaxableSalariesCompensationCurER"]
                                                    , drW2Record["Tah_MWEBasicPrvER"]                                               //30
                                                    , drW2Record["Tah_MWEHolidayPrvER"]
                                                    , drW2Record["Tah_MWEOvertimePrvER"]
                                                    , drW2Record["Tah_MWENightShiftPrvER"]
                                                    , drW2Record["Tah_MWEHazardPrvER"]
                                                    , drW2Record["Tah_Nontaxable13thMonthPrvER"]                                    //35
                                                    , drW2Record["Tah_DeMinimisPrvER"]
                                                    , drW2Record["Tah_PremiumsUnionDuesPrvER"]
                                                    , drW2Record["Tah_NontaxableSalariesCompensationPrvER"]
                                                    , drW2Record["Tah_TaxableBasicPrvER"]
                                                    , drW2Record["Tah_TaxableBasicNetPremiumsPrvER"]                                //40
                                                    , drW2Record["Tah_TaxableOvertimePrvER"]
                                                    , drW2Record["Tah_Taxable13thMonthPrvER"]
                                                    , drW2Record["Tah_TaxableSalariesCompensationPrvER"]
                                                    , drW2Record["Tah_RepresentationCurER"]
                                                    , drW2Record["Tah_TransportationCurER"]                                              //45
                                                    , drW2Record["Tah_CostLivingAllowanceCurER"]
                                                    , drW2Record["Tah_FixedHousingAllowanceCurER"]
                                                    , drW2Record["Tah_OtherTaxable1CurER"]
                                                    , drW2Record["Tah_OtherTaxable2CurER"]
                                                    , drW2Record["Tah_CommisionCurER"]                                                   //50
                                                    , drW2Record["Tah_ProfitSharingCurER"]
                                                    , drW2Record["Tah_FeesCurER"]
                                                    , drW2Record["Tah_HazardCurER"]
                                                    , drW2Record["Tah_SupplementaryTaxable1CurER"]
                                                    , drW2Record["Tah_SupplementaryTaxable2CurER"]                                       //55
                                                    , drW2Record["Tah_RepresentationPrvER"]
                                                    , drW2Record["Tah_TransportationPrvER"]                                              
                                                    , drW2Record["Tah_CostLivingAllowancePrvER"]
                                                    , drW2Record["Tah_FixedHousingAllowancePrvER"]
                                                    , drW2Record["Tah_OtherTaxable1PrvER"]                                              //60
                                                    , drW2Record["Tah_OtherTaxable2PrvER"]
                                                    , drW2Record["Tah_CommisionPrvER"]                                                   
                                                    , drW2Record["Tah_ProfitSharingPrvER"]
                                                    , drW2Record["Tah_FeesPrvER"]
                                                    , drW2Record["Tah_HazardPrvER"]                                                     //65
                                                    , drW2Record["Tah_SupplementaryTaxable1PrvER"]
                                                    , drW2Record["Tah_SupplementaryTaxable2PrvER"]                                       
                                                    , drW2Record["Tah_ExemptionCode"]   
                                                    , drW2Record["Tah_ExemptionAmount"]
                                                    , drW2Record["Tah_PremiumPaidOnHealth"]                                             //70
                                                    , drW2Record["Tah_GrossCompensationIncome"]
                                                    , drW2Record["Tah_NontaxableIncomeCurER"]                                       
                                                    , drW2Record["Tah_NontaxableIncomePrvER"]
                                                    , drW2Record["Tah_TaxableIncomeCurER"]
                                                    , drW2Record["Tah_TaxableIncomePrvER"]                                              //75
                                                    , drW2Record["Tah_NetTaxableIncome"]
                                                    , drW2Record["Tah_TaxDue"]                                                      
                                                    , drW2Record["Tah_TaxWithheldPrvER"]                                            
                                                    , drW2Record["Tah_TaxWithheldCurER"]
                                                    , drW2Record["Tah_TotalTaxWithheld"]                                                //80
                                                    , drW2Record["Tah_TotalTaxWithheldJanDec"]
                                                    , drW2Record["Tah_TaxAmount"]                                                   
                                                    , drW2Record["Tah_IsSubstitutedFiling"]                                        
                                                    , drW2Record["Tah_IsTaxExempted"]
                                                    , drW2Record["Tah_FinalPayIndicator"]                                               //85
                                                    , drW2Record["Tah_IsZeroOutLastQuinYear"]
                                                    , drW2Record["Tah_CostcenterCode"]                                              
                                                    , drW2Record["Tah_PayrollGroup"]                                                
                                                    , drW2Record["Tah_PayrollType"]
                                                    , drW2Record["Tah_EmploymentStatus"]                                                //90
                                                    , drW2Record["Tah_WorkStatus"]
                                                    , drW2Record["Tah_AssumeBasic"]                                                
                                                    , drW2Record["Tah_Assume13th"]                                                  
                                                    , drW2Record["Tah_AssumeSalariesCompensation"]
                                                    , drW2Record["Tah_AssumePremiumsUnionDues"]                                         //95
                                                    , drW2Record["Tah_AssumePayCycle"]
                                                    , drW2Record["Tah_TaxBracket"]
                                                    , drW2Record["Tah_CurrentEmploymentStatus"]                                              
                                                    , drW2Record["Tah_Nationality"]                                                    
                                                    , drW2Record["Tah_SeparationReason"]                                               //100
                                                    , drW2Record["Usr_Login"]);                                                        //101

                    #endregion
                    iUpdateCtr++;
                    if (iUpdateCtr == 100)
                    {
                        dalCentral.ExecuteNonQuery(strUpdateQuery);
                        strUpdateQuery = "";
                        iUpdateCtr = 0;
                    }
                }
                if (strUpdateQuery != "")
                    dalCentral.ExecuteNonQuery(strUpdateQuery);

                strUpdateQuery = "";
                iUpdateCtr = 0;
                #region Alphalist Detail Record Insert
                strInsertRecordTemplate = @" INSERT INTO T_EmpAlphalistDtl (Tad_IDNo,Tad_TaxYear,Tad_LineNo,Tad_LastName,Tad_FirstName,Tad_MiddleName,Tad_Birthdate,Usr_Login,Ludatetime) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}',GETDATE()) ";
                #endregion
                foreach (DataRow drW2Record in dtW2Detail.Rows)
                {
                    #region Alphalist Detail Insert
                    strUpdateQuery += string.Format(strInsertRecordTemplate
                                                    , drW2Record["Tad_IDNo"]
                                                    , drW2Record["Tad_TaxYear"]
                                                    , drW2Record["Tad_LineNo"]
                                                    , drW2Record["Tad_LastName"].ToString().Replace("'", "")
                                                    , drW2Record["Tad_FirstName"].ToString().Replace("'", "")
                                                    , drW2Record["Tad_MiddleName"].ToString().Replace("'", "")
                                                    , drW2Record["Tad_Birthdate"]
                                                    , drW2Record["Usr_Login"]
                                                    , drW2Record["Ludatetime"]);
                    #endregion
                    iUpdateCtr++;
                    if (iUpdateCtr == 100)
                    {
                        dalCentral.ExecuteNonQuery(strUpdateQuery);
                        strUpdateQuery = "";
                        iUpdateCtr = 0;
                    }
                }
                if (strUpdateQuery != "")
                    dalCentral.ExecuteNonQuery(strUpdateQuery);
                #endregion

                #region Check Alphalist Setup
                bool bAlphalistExists = false;
                if (IsLastPay)
                {
                    string sqlCheck = string.Format("SELECT Tal_TaxYear FROM T_Alphalist WHERE Tal_CompanyCode = '{0}' AND Tal_TaxYear = '{1}'", CompanyCode, taxYear);
                    DataSet dsResult = dalCentral.ExecuteDataSet(sqlCheck);
                    bAlphalistExists = ((dsResult != null && dsResult.Tables.Count > 0 && dsResult.Tables[0].Rows.Count > 0) ? true : false);
                }
                #endregion

                if (ProcessAll || (!ProcessAll && !bAlphalistExists))
                {
                    strUpdateQuery = "";
                    iUpdateCtr = 0;

                    dalCentral.ExecuteNonQuery(string.Format("DELETE FROM T_Alphalist WHERE Tal_CompanyCode = '{0}' AND Tal_TaxYear = '{1}'", CompanyCode, taxYear));
                    
                    #region Save Alphalist Settings

                    #region Alphalist Settings Record Insert
                    strInsertRecordTemplate = @" INSERT INTO T_Alphalist(Tal_CompanyCode,Tal_TaxYear,Tal_AssumeRemainingCycle,Tal_Assume13thMonthCutOff,Tal_AssumeRecurringAllowance,Tal_MonthlyPremiumUpToMax,Tal_HireDateCutOff,Tal_TaxWithheldCutOff,Tal_MWERegion,Tal_MWEBasicPerDay,Tal_MWEDaysPerYear,Tal_MWEBasicPerMonth,Tal_MWEBasicPerYear,Tal_OtherTaxableSpecify1,Tal_OtherTaxableSpecify2,Tal_SupplementaryTaxableSpecify1,Tal_SupplementaryTaxableSpecify2,Tal_RecurringAllowanceBIRreporting,Tal_BIREmploymentStatusCode,Tal_BIRSeparationCode,Tal_BIREmploymentStatusToProcess,Usr_Login,Ludatetime) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}',{9},{10},{11},{12},'{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}',GETDATE()) ";
                    #endregion

                    foreach (DataRow drAlphalistRecord in dtAlphalist.Rows)
                    {
                        #region Alphalist Settings Insert
                        strUpdateQuery += string.Format(strInsertRecordTemplate
                                                        , drAlphalistRecord["Tal_CompanyCode"]
                                                        , drAlphalistRecord["Tal_TaxYear"]
                                                        , drAlphalistRecord["Tal_AssumeRemainingCycle"]
                                                        , drAlphalistRecord["Tal_Assume13thMonthCutOff"]
                                                        , drAlphalistRecord["Tal_AssumeRecurringAllowance"]
                                                        , drAlphalistRecord["Tal_MonthlyPremiumUpToMax"]
                                                        , drAlphalistRecord["Tal_HireDateCutOff"]
                                                        , drAlphalistRecord["Tal_TaxWithheldCutOff"]
                                                        , drAlphalistRecord["Tal_MWERegion"]
                                                        , drAlphalistRecord["Tal_MWEBasicPerDay"]
                                                        , drAlphalistRecord["Tal_MWEDaysPerYear"]
                                                        , drAlphalistRecord["Tal_MWEBasicPerMonth"]
                                                        , drAlphalistRecord["Tal_MWEBasicPerYear"]
                                                        , drAlphalistRecord["Tal_OtherTaxableSpecify1"]
                                                        , drAlphalistRecord["Tal_OtherTaxableSpecify2"]
                                                        , drAlphalistRecord["Tal_SupplementaryTaxableSpecify1"]
                                                        , drAlphalistRecord["Tal_SupplementaryTaxableSpecify2"]
                                                        , drAlphalistRecord["Tal_RecurringAllowanceBIRreporting"]
                                                        , drAlphalistRecord["Tal_BIREmploymentStatusCode"].ToString().Replace("'", "''")
                                                        , drAlphalistRecord["Tal_BIRSeparationCode"].ToString().Replace("'", "''")
                                                        , drAlphalistRecord["Tal_BIREmploymentStatusToProcess"].ToString().Replace("'", "''")
                                                        , drAlphalistRecord["Usr_Login"]);
                        #endregion
                        iUpdateCtr++;
                    }
                    if (strUpdateQuery != "")
                        dalCentral.ExecuteNonQuery(strUpdateQuery);

                    #endregion
                }

                StatusHandler(this, new StatusEventArgs("Saving of Alphalist Records", true));

                StatusHandler(this, new StatusEventArgs("Post Alphalist Estimate Values", false));
                PopulateW2StagingEstimateValues(ProcessAll, IsLastPay, EmployeeID, EmployeeList, taxYear, RCURALPHACATGY, LoginUser, CompanyCode, CentralProfile);
                StatusHandler(this, new StatusEventArgs("Post Alphalist Estimate Values", true));

                #region Generate Payroll Error List
                dtErrList = SaveErrorReportList();
                if (dtErrList.Rows.Count > 0)
                    InsertToEmpPayrollCheckTable(dtErrList, LoginDBName);
                #endregion
            }
            catch (Exception ex)
            {
                CommonProcedures.logErrorToFile(ex.ToString());
                throw new Exception("Alphalist Processing has encountered some errors: \n" + ex.Message);
            }
            #endregion

            return dtErrList;
        }
        #endregion

        #region Minor Functions
        private bool CheckIfThereIsExemption(bool ProcessAll, string EmployeeId, string EmployeeList, string PayPeriod, string LoginDBName)
        {
            string EmployeeCondition = "";
            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition = " AND Tpy_IDNo = '" + EmployeeId + "' ";
            else if (ProcessAll == true && EmployeeList != "")
                EmployeeCondition = " AND Tpy_IDNo IN (" + EmployeeList + ") ";

            string strQuery = string.Format(@"SELECT TOP 1 Tpy_IDNo, Tpy_PayCycle
                                                FROM {2}..T_EmpPayroll2
                                                WHERE Tpy_TotalExemptions > 0
                                                    AND Tpy_PayCycle = '{0}'
                                                    {1}
                                                UNION ALL
                                                SELECT TOP 1 Tpy_IDNo, Tpy_PayCycle
                                                FROM {2}..T_EmpPayroll2Yearly
                                                WHERE Tpy_TotalExemptions > 0
                                                    AND Tpy_PayCycle = '{0}'
                                                    {1}
                                                UNION ALL
                                                SELECT TOP 1 Tpy_IDNo, Tpy_PayCycle
                                                FROM {2}..T_EmpPayroll2Hst
                                                WHERE Tpy_TotalExemptions > 0
                                                    AND Tpy_PayCycle = '{0}'
                                                    {1}", PayPeriod
                                                    , EmployeeCondition
                                                    , LoginDBName);

            DataTable dtResult = dalCentral.ExecuteDataSet(strQuery).Tables[0];
            if (dtResult.Rows.Count > 0)
                return true;
            else
                return false;
        }

        public DataTable GetTaxMaster(string CompanyCode, string CentralProfile)
        {
            string query = string.Format(@"SELECT Mtx_TaxComputation
                                            ,Mtx_ReferPreviousIncomeTax
                                            ,Mtx_IncludeSpecialPayrollOnNormalTax
                                            ,Mtx_DeductionPayCycle
                                            ,Mtx_TaxRule
                                            ,Mtx_TaxRate
                                            ,Mtx_TaxAnnualization
                                            ,Mtx_OffCycleTaxComputation
                                            ,Mtx_AssumeRemainingCycle
                                            ,Mtx_Assume13thMonthCutOff
                                            ,Mtx_AssumeRecurringAllowance
                                            ,Mtx_MonthlyPremiumUpToMax
                                            ,Mtx_HireDateCutOff
                                            ,Mtx_TaxWithheldCutOff
                                            ,Mtx_MWERegion
                                            ,Mtx_MWEBasicPerDay
                                            ,Mtx_MWEDaysPerYear
                                            ,Mtx_MWEBasicPerMonth
                                            ,Mtx_MWEBasicPerYear
                                            ,Mtx_OtherTaxableSpecify1
                                            ,Mtx_OtherTaxableSpecify2
                                            ,Mtx_SupplementaryTaxableSpecify1
                                            ,Mtx_SupplementaryTaxableSpecify2
                                            ,Mtx_RecurringAllowanceBIRreporting
                                            ,Mtx_LatestTaxYearTransferred
                                            ,Mtx_SignatoryID
                                            ,Mtx_SignatoryPosition
                                            ,Mtx_BIRNewFormTaxYear
                                            ,Mtx_BIREmploymentStatusCode
                                            ,Mtx_BIRSeparationCode
                                            ,ISNULL(Mtx_BIREmploymentStatusToProcess,'') AS Mtx_BIREmploymentStatusToProcess
                                           FROM M_Tax
                                           WHERE Mtx_CompanyCode ='{0}'
                                        ", CompanyCode);

            DataTable dtResult = null;
            using (DALHelper dalHelper = new DALHelper(CentralProfile, false))
            {
                dtResult = dalHelper.ExecuteDataSet(query).Tables[0];
            }
            return dtResult;
        }

        public DataTable GetAlphalistSetting(string TaxYear, string CompanyCode, string CentralProfile)
        {
            string query = string.Format(@"SELECT CASE WHEN Tal_AssumeRemainingCycle = 1 THEN 'Yes' ELSE 'No' END AS Tal_AssumeRemainingCycle
                                                ,Tal_Assume13thMonthCutOff
                                                ,CASE WHEN Tal_AssumeRecurringAllowance = 1 THEN 'Yes' ELSE 'No' END AS Tal_AssumeRecurringAllowance
                                                ,CASE WHEN Tal_MonthlyPremiumUpToMax = 1 THEN 'Yes' ELSE 'No' END AS Tal_MonthlyPremiumUpToMax
                                                ,Tal_HireDateCutOff
                                                ,Tal_TaxWithheldCutOff
                                                ,Tal_MWERegion
                                                ,Tal_MWEBasicPerDay
                                                ,Tal_MWEDaysPerYear
                                                ,Tal_MWEBasicPerMonth
                                                ,Tal_MWEBasicPerYear
                                                ,Tal_OtherTaxableSpecify1
                                                ,Tal_OtherTaxableSpecify2
                                                ,Tal_SupplementaryTaxableSpecify1
                                                ,Tal_SupplementaryTaxableSpecify2
                                                ,Tal_RecurringAllowanceBIRreporting
                                                ,Tal_BIREmploymentStatusCode
                                                ,Tal_BIRSeparationCode
                                                ,Tal_BIREmploymentStatusToProcess
                                           FROM T_Alphalist
                                           WHERE Tal_CompanyCode ='{0}'
                                                AND Tal_TaxYear = '{1}'
                                        ", CompanyCode
                                        , TaxYear);

            DataTable dtResult = null;
            using (DALHelper dalHelper = new DALHelper(CentralProfile, false))
            {
                dtResult = dalHelper.ExecuteDataSet(query).Tables[0];
            }
            return dtResult;
        }

        public DataTable GetEmployees(string EmployeeList, string TaxYear, string TaxHireCutOff, string CompanyCode, string ProfileName, string TaxBIREmploymentStatusToProcess, DALHelper dalhelper, string CentralProfile) //gcd
        {
            this.dalCentral = dalhelper;
            return GetEmployees(true, false, "", EmployeeList, TaxYear, TaxHireCutOff, CompanyCode, ProfileName, TaxBIREmploymentStatusToProcess, CentralProfile);
        }

        public DataTable GetEmployees(bool ProcessAll, bool bFinalPay, string EmployeeId, string EmployeeList, string TaxYear, string TaxHireCutOff, string CompanyCode, string ProfileName, string TaxBIREmploymentStatusToProcess, string CentralProfile)
        {
            string EmployeeCondition = "";
            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition = " AND Mem_IDNo = '" + EmployeeId + "' ";
            else if (ProcessAll == true && EmployeeList != "")
                EmployeeCondition = " AND Mem_IDNo IN (" + EmployeeList + ") ";

            if (!string.IsNullOrEmpty(TaxBIREmploymentStatusToProcess))
                EmployeeCondition += " AND Mem_EmploymentStatusCode IN (" + new CommonBL().EncodeFilterItems(TaxBIREmploymentStatusToProcess, true) + ") ";

            //DataTable dtPayCycle = commonBL.GetPayCycleStartendDate(TaxYear + "011");
            //if (dtPayCycle.Rows.Count > 0)
            //    TaxYearStart = dtPayCycle.Rows[0]["Tps_StartCycle"].ToString();
            //else
            //    throw new Exception(string.Format("Please create {0} pay cycle.", TaxYear + "011"));
            //string PayrollCondition = "";
            //if (bFinalPay == false)
            //{
            //    PayrollCondition = string.Format(@"AND Mem_IntakeDate <= '{0}'
            //                                        AND (Mem_SeparationDate IS NULL
            //                                        OR Mem_SeparationDate >= '{1}')", TaxHireCutOff, TaxYearStart);
            //}

            string strQuery = string.Format(@"SELECT Mem_IDNo
                                                    ,Mem_LastName
                                                    ,Mem_FirstName
                                                    ,Mem_MiddleName
                                                    ,Mem_ExtensionName
                                                    ,Mem_TIN
                                                    ,RTRIM(Mem_PresContactNo) AS [Mem_PresContactNo]
                                                    ,Mem_PayrollGroup
                                                    ,Mem_PositionGrade
                                                    ,Mem_EmploymentStatusCode
                                                    ,Mem_WorkStatus
                                                    ,Mem_CostcenterCode
                                                    ,Mem_PayrollType
                                                    ,CASE WHEN CONVERT(DATE,Mem_IntakeDate) >= @StartDate THEN CONVERT(DATE,Mem_IntakeDate) ELSE NULL END AS Mem_IntakeDate
                                                    ,CASE WHEN Mem_SeparationDate IS NOT NULL AND CONVERT(DATE,Mem_SeparationDate) <= @EndDate THEN CONVERT(DATE,Mem_SeparationDate) ELSE NULL END AS Mem_SeparationDate
                                                    ,Mem_PresMailingAddress
                                                    ,PRES.Mlh_ZipCode AS Mem_PresLocationCode
                                                    ,Mem_ProvMailingAddress
                                                    ,PROV.Mlh_ZipCode AS Mem_ProvLocationCode
                                                    ,Mem_WifeClaim
                                                    ,ISNULL(Mem_IsTaxExempted, 0) AS [Mem_IsTaxExempted]
                                                    ,Mem_TaxCode
                                                    ,Mem_NationalityCode
                                                FROM {1}..M_Employee
                                                LEFT JOIN {2}..M_LocationHdr PRES ON PRES.Mlh_CompanyCode = @CompanyCode
                                                    AND PRES.Mlh_LocationCode = Mem_PresLocationCode
                                                LEFT JOIN {2}..M_LocationHdr PROV ON PROV.Mlh_CompanyCode = @CompanyCode
                                                    AND PROV.Mlh_LocationCode = Mem_ProvLocationCode
                                                --INNER JOIN (
                                                --    SELECT Mpd_SubCode 
                                                --    FROM {1}..M_PolicyDtl 
                                                --    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                                                --        AND Mpd_ParamValue = 1
                                                --        AND Mpd_CompanyCode = @CompanyCode
                                                --) EMPSTAT
                                                ---ON Mem_EmploymentStatusCode = Mpd_SubCode
                                                WHERE Mem_CompanyCode = @CompanyCode
                                                {0}
                                                ORDER BY Mem_LastName, Mem_FirstName"
                                                , EmployeeCondition
                                                , ProfileName
                                                , CentralProfile);


            ParameterInfo[] paramInfo = new ParameterInfo[3];
            paramInfo[0] = new ParameterInfo("@StartDate", "01/01/" + TaxYear, SqlDbType.Date);
            paramInfo[1] = new ParameterInfo("@EndDate", TaxHireCutOff, SqlDbType.Date);
            paramInfo[2] = new ParameterInfo("@CompanyCode", CompanyCode);

            DataTable dtResult = dalCentral.ExecuteDataSet(strQuery, CommandType.Text, paramInfo).Tables[0];
            return dtResult;
        }

        private DataTable GetBeneficiaries(bool ProcessAll, string EmployeeId, string EmployeeList, string CompanyCode)
        {
            string EmployeeCondition = "";
            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition = " AND Mef_IDNo = '" + EmployeeId + "' ";
            else if (ProcessAll == true && EmployeeList != "")
                EmployeeCondition = " AND Mef_IDNo IN (" + EmployeeList + ") ";

            string strQuery = string.Format(@"SELECT Mef_IDNo
		                                                , Mef_LastName
		                                                , Mef_FirstName
		                                                , Mef_MiddleName
		                                                , CONVERT(DATE,Mef_BirthDate) AS Mef_BirthDate 
                                                FROM M_EmpFamily
                                                WHERE Mef_BIRDependent = 1
                                                AND Mef_BirthDate IS NOT NULL
                                                {0}
                                                AND Mef_CompanyCode = '{1}'
                                                ORDER BY Mef_IDNo, Mef_BirthDate"
                                                , EmployeeCondition
                                                , CompanyCode);

            DataTable dtResult = dalCentral.ExecuteDataSet(strQuery).Tables[0];
            return dtResult;
        }

        private DataTable GetActiveProfiles(string CompanyCode, string CentralProfile)
        {
            string query = string.Format(@"IF EXISTS(SELECT Mpf_DatabaseNo FROM M_Profile WHERE Mpf_RecordStatus = 'A' AND Mpf_CompanyCode = '{0}' AND Mpf_ProfileType = 'S')
                                            SELECT * FROM M_Profile WHERE Mpf_RecordStatus = 'A' AND Mpf_CompanyCode = '{0}' AND Mpf_ProfileType = 'S' AND Mpf_IncludePayroll = 1
                                           ELSE 
                                            SELECT * FROM M_Profile WHERE Mpf_RecordStatus = 'A' AND Mpf_CompanyCode = '{0}' AND Mpf_ProfileType = 'P' AND Mpf_IncludePayroll = 1", CompanyCode);

            DataTable dtResult = new DataTable();
            dtResult = dalCentral.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public DataTable GetActiveProfiles(string CompanyCode, string CentralProfile, DALHelper dalhelper)
        {
            string query = string.Format(@"IF EXISTS(SELECT Mpf_DatabaseNo FROM M_Profile WHERE Mpf_RecordStatus = 'A' AND Mpf_CompanyCode = '{0}' AND Mpf_ProfileType = 'S')
                                            SELECT * FROM M_Profile WHERE Mpf_RecordStatus = 'A' AND Mpf_CompanyCode = '{0}' AND Mpf_ProfileType = 'S' AND Mpf_IncludePayroll = 1
                                           ELSE 
                                            SELECT * FROM M_Profile WHERE Mpf_RecordStatus = 'A' AND Mpf_CompanyCode = '{0}' AND Mpf_ProfileType = 'P' AND Mpf_IncludePayroll = 1", CompanyCode);

            DataTable dtResult = new DataTable();
            dtResult = dalhelper.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        private void PopulateYTDSystem(bool ProcessAll, bool bFinalPay, DataTable dtCompany, string EmployeeId, string EmployeeList, string TaxYear, string UserLogin, string CompanyCode, string CentralProfile)
        {
            string EmployeeCondition = "";
            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition = " AND Tyt_IDNo = '" + EmployeeId + "' ";
            else if (ProcessAll == true && EmployeeList != "")
                EmployeeCondition = " AND Tyt_IDNo IN (" + EmployeeList + ") ";

            string EmployeeCondition2 = "";
            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition2 = " AND Tpy_IDNo = '" + EmployeeId + "' ";
            else if (ProcessAll == true && EmployeeList != "")
                EmployeeCondition2 = " AND Tpy_IDNo IN (" + EmployeeList + ") ";

            #region Delete Alphalist Staging Records Before Insert
            string strDeleteQuery = string.Format(@"DELETE
												    FROM T_EmpYTD
                                                    WHERE Tyt_EmployerType='S'
                                                    AND Tyt_TaxYear = '{0}'
                                                    {1} ", TaxYear
                                                        , EmployeeCondition);
            dalCentral.ExecuteNonQuery(strDeleteQuery);
            #endregion

            #region Template Insert Query
            string strQuery = @"INSERT INTO T_EmpYTD (Tyt_IDNo,Tyt_TaxYear,Tyt_StartDate,Tyt_EndDate,Tyt_EmployerType,Tyt_EmployerName,Tyt_TIN,Tyt_EmployerAddress,Tyt_EmployerZipCode,Tyt_MWEBasic,Tyt_MWEHoliday,Tyt_MWEOvertime,Tyt_MWENightShift,Tyt_MWEHazard,Tyt_Nontaxable13thMonth,Tyt_DeMinimis,Tyt_PremiumsUnionDues,Tyt_NontaxableSalariesCompensation,Tyt_TaxableBasic,Tyt_TaxableBasicNetPremiums,Tyt_Taxable13thMonth,Tyt_TaxableSalariesCompensation,Tyt_TaxableOvertime,Tyt_Hazard,Tyt_PremiumPaidOnHealth,Tyt_TaxWithheld,Tyt_Representation,Tyt_Transportation,Tyt_CostLivingAllowance,Tyt_FixedHousingAllowance,Tyt_OtherTaxable1,Tyt_OtherTaxable2,Tyt_Commision,Tyt_ProfitSharing,Tyt_Fees,Tyt_SupplementaryTaxable1,Tyt_SupplementaryTaxable2,Tyt_EmploymentStatus,Tyt_SeparationReason,Usr_Login,Ludatetime)
                                SELECT Tyt_IDNo         = Tpy_IDNo
	                            , Tyt_TaxYear           = '{0}'
	                            , Tyt_StartDate         = '12/31/{0}' 
	                            , Tyt_EndDate           = '12/31/{0}'
	                            , Tyt_EmployerType      = 'S'
	                            , Tyt_EmployerName      = '{4}' 
	                            , Tyt_TIN               = '{5}' 
	                            , Tyt_EmployerAddress   = '{6}' 
	                            , Tyt_EmployerZipCode   = '{7}' 
	                            , Tyt_MWEBasic          = 0.00
	                            , Tyt_MWEHoliday        = 0.00
	                            , Tyt_MWEOvertime       = 0.00
	                            , Tyt_MWENightShift     = 0.00
	                            , Tyt_MWEHazard         = 0.00
	                            , Tyt_Nontaxable13thMonth = 0.00
	                            , Tyt_DeMinimis         = 0.00
	                            , Tyt_PremiumsUnionDues = 0.00
	                            , Tyt_NontaxableSalariesCompensation = 0.00
	                            , Tyt_TaxableBasic = SUM(Tpy_TotalREGAmt - (Tpy_REGAmt + Tpy_PDLVAmt + Tpy_PDLEGHOLAmt + Tpy_PDSPLHOLAmt + Tpy_PDCOMPHOLAmt +Tpy_PDPSDAmt + Tpy_PDOTHHOLAmt + Tpy_PDRESTLEGHOLAmt))
	                            , Tyt_TaxableBasicNetPremiums = SUM(Tpy_TotalREGAmt - (Tpy_REGAmt + Tpy_PDLVAmt + Tpy_PDLEGHOLAmt + Tpy_PDSPLHOLAmt + Tpy_PDCOMPHOLAmt +Tpy_PDPSDAmt + Tpy_PDOTHHOLAmt + Tpy_PDRESTLEGHOLAmt))
	                            , Tyt_Taxable13thMonth = 0.00
	                            , Tyt_TaxableSalariesCompensation = 0.00
	                            , Tyt_TaxableOvertime   = 0.00
	                            , Tyt_Hazard            = 0.00
	                            , Tyt_PremiumPaidOnHealth = 0.00
	                            , Tyt_TaxWithheld       = 0.00
	                            , Tyt_Representation    = 0.00
	                            , Tyt_Transportation    = 0.00
	                            , Tyt_CostLivingAllowance = 0.00
	                            , Tyt_FixedHousingAllowance = 0.00
	                            , Tyt_OtherTaxable1     = 0.00
	                            , Tyt_OtherTaxable2     = 0.00
	                            , Tyt_Commision         = 0.00
	                            , Tyt_ProfitSharing     = 0.00
	                            , Tyt_Fees              = 0.00
	                            , Tyt_SupplementaryTaxable1 = 0.00
	                            , Tyt_SupplementaryTaxable2 = 0.00
	                            , Tyt_EmploymentStatus  =''
	                            , Tyt_SeparationReason  =''
	                            , Usr_Login             = '{3}'
	                            , Ludatetime            = GETDATE()
                            FROM (  
                                    {2}
								 ) TMP
                            WHERE LEFT(Tpy_PayCycle, 4) = '{0}'
                            {1}
                            AND (Tpy_REGAmt + Tpy_PDLVAmt + Tpy_PDLEGHOLAmt + Tpy_PDSPLHOLAmt + Tpy_PDCOMPHOLAmt +Tpy_PDPSDAmt + Tpy_PDOTHHOLAmt + Tpy_PDRESTLEGHOLAmt) <> Tpy_TotalREGAmt
                            GROUP BY Tpy_IDNo, LEFT(Tpy_PayCycle, 4)";
            #endregion

            #region Get Payroll Profiles
            DataTable dtDatabases = GetActiveProfiles(CompanyCode, CentralProfile);
            string tmpQuery = "";
            for (int i = 0; i < dtDatabases.Rows.Count; i++)
            {
                if (!string.IsNullOrEmpty(tmpQuery))
                    tmpQuery += @"UNION ALL
                        "; 
                tmpQuery += string.Format(@"SELECT Tpy_IDNo, Tpy_PayCycle, Tpy_REGAmt , Tpy_PDLVAmt , Tpy_PDLEGHOLAmt , Tpy_PDSPLHOLAmt , Tpy_PDCOMPHOLAmt , Tpy_PDPSDAmt , Tpy_PDOTHHOLAmt , Tpy_PDRESTLEGHOLAmt, Tpy_TotalREGAmt FROM {0}.dbo.Udv_Payroll ", dtDatabases.Rows[i]["Mpf_DatabaseName"]);
            }
            #endregion

            if (!string.IsNullOrEmpty(tmpQuery) && dtCompany != null)
            {
                #region Insert Alphalist Staging Records
                string query = string.Format(strQuery
                                            , TaxYear
                                            , EmployeeCondition2
                                            , tmpQuery
                                            , UserLogin
                                            , dtCompany.Rows[0]["Mcm_CompanyName"].ToString()
                                            , dtCompany.Rows[0]["Mcm_TIN"].ToString()
                                            , dtCompany.Rows[0]["Mcm_BusinessAddress"].ToString()
                                            , dtCompany.Rows[0]["Mcm_MunicipalityCity"].ToString());
                dalCentral.ExecuteNonQuery(query);
                #endregion
            }

        }

        private void PopulateW2StagingPayroll(bool ProcessAll, bool PostHoldPayroll, bool bFinalPay, string EmployeeId, string EmployeeList, string TaxYear, string UserLogin, string CompanyCode, string CentralProfile)
        {
            string EmployeeCondition = "";
            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition = " AND Tas_IDNo = '" + EmployeeId + "' ";
            else if (ProcessAll == true && EmployeeList != "")
                EmployeeCondition = " AND Tas_IDNo IN (" + EmployeeList + ") ";

            string EmployeeCondition2 = "";
            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition2 = " AND Tpy_IDNo = '" + EmployeeId + "' ";
            else if (ProcessAll == true && EmployeeList != "")
                EmployeeCondition2 = " AND Tpy_IDNo IN (" + EmployeeList + ") ";

            string HoldPayrollCondition = "";
            if (PostHoldPayroll == false)
                HoldPayrollCondition = " AND Tpy_PaymentMode != 'H' ";

            string FinalPayCondition = "";
            if (bFinalPay)
                    FinalPayCondition = string.Format(@"AND ((Tps_CycleIndicator = 'P' OR Tps_CycleIndicatorSpecial = 'P') OR (Tps_CycleIndicator = 'S' AND Tps_CycleIndicatorSpecial = 'C' AND Tps_CycleType = 'L'))");
            
            #region Delete Alphalist Staging Records Before Insert
            string strDeleteQuery = string.Format(@"DELETE
												    FROM T_EmpAlphalistStaging
                                                    WHERE Tas_TaxYear = '{0}'
													{1} ", TaxYear
                                                    , EmployeeCondition);
            dalCentral.ExecuteNonQuery(strDeleteQuery);
            #endregion

            #region Template Insert Query
            string strQuery = @"INSERT INTO T_EmpAlphalistStaging (Tas_TaxYear,Tas_IDNo,Tas_PayCycle,Tas_ProfileCode,Tas_DataSource,Tas_SalaryRate,Tas_IsTaxExempted,Tas_Basic_PAY,Tas_Basic_INC,Tas_Basic_YTD,Tas_Basic_ADJ,Tas_Representation_INC,Tas_Representation_YTD,Tas_Transportation_INC,Tas_Transportation_YTD,Tas_CostLivingAllowance_INC,Tas_CostLivingAllowance_YTD,Tas_FixedHousingAllowance_INC,Tas_FixedHousingAllowance_YTD,Tas_RegularOtherTaxableIncome1_PAY,Tas_RegularOtherTaxableIncome1_INC,Tas_RegularOtherTaxableIncome1_YTD,Tas_RegularOtherTaxableIncome2_PAY,Tas_RegularOtherTaxableIncome2_INC,Tas_RegularOtherTaxableIncome2_YTD,Tas_Commision_INC,Tas_Commision_YTD,Tas_ProfitSharing_INC,Tas_ProfitSharing_YTD,Tas_Fees_INC,Tas_Fees_YTD,Tas_Hazard_INC,Tas_Hazard_YTD,Tas_Overtime_PAY,Tas_Overtime_INC,Tas_Overtime_YTD,Tas_Overtime_ADJ,Tas_SupplementaryOtherTaxableIncome1_PAY,Tas_SupplementaryOtherTaxableIncome1_INC,Tas_SupplementaryOtherTaxableIncome1_YTD,Tas_SupplementaryOtherTaxableIncome2_PAY,Tas_SupplementaryOtherTaxableIncome2_INC,Tas_SupplementaryOtherTaxableIncome2_YTD,Tas_MWEBasic_PAY,Tas_MWEBasic_INC,Tas_MWEBasic_YTD,Tas_MWEBasic_ADJ,Tas_MWEHoliday_PAY,Tas_MWEHoliday_INC,Tas_MWEHoliday_YTD,Tas_MWEHoliday_ADJ,Tas_MWEOvertime_PAY,Tas_MWEOvertime_INC,Tas_MWEOvertime_YTD,Tas_MWEOvertime_ADJ,Tas_MWENightShift_PAY,Tas_MWENightShift_INC,Tas_MWENightShift_YTD,Tas_MWENightShift_ADJ,Tas_MWEHazard_INC,Tas_MWEHazard_YTD,Tas_13thMonthBen_PAY,Tas_13thMonthBen_INC,Tas_13thMonthBen_YTD,Tas_DeMinimis_INC,Tas_DeMinimis_YTD,Tas_Premiums_PAY,Tas_Premiums_INC,Tas_Premiums_DED,Tas_Premiums_YTD,Tas_NontaxableSalaries_INC,Tas_NontaxableSalaries_YTD,Tas_Tax_PAY,Tas_Tax_INC,Tas_Tax_DED,Tas_Tax_YTD,Tas_PremiumPaidOnHealth,Usr_Login,Ludatetime)
                                SELECT Tas_TaxYear = '{0}'
	                                , Tas_IDNo = Tpy_IDNo
	                                , Tas_PayCycle = Tpy_Paycycle
	                                , Tas_ProfileCode = '{2}'
	                                , Tas_Datasource = 'A'
                                    , Tas_SalaryRate = Tpy_SalaryRate
	                                , Tas_IsTaxExempted = Tpy_IsTaxExempted
	                                , Tas_Basic_PAY = CASE WHEN Tpy_IsTaxExempted = 0 THEN 
							                                Tpy_REGAmt + 
							                                Tpy_PDLVAmt + 
							                                Tpy_PDLEGHOLAmt + 
							                                Tpy_PDSPLHOLAmt + 
							                                Tpy_PDCOMPHOLAmt + 
							                                Tpy_PDPSDAmt + 
							                                Tpy_PDOTHHOLAmt + 
							                                Tpy_PDRESTLEGHOLAmt 
						                                ELSE 0 END
	                                , Tas_Basic_INC = 0.00
	                                , Tas_Basic_YTD = 0.00
	                                , Tas_Basic_ADJ  = CASE WHEN Tpy_IsTaxExempted = 0 THEN 
							                                Tpy_SRGAdjAmt + 
							                                Tpy_SLVAdjAmt + 
							                                Tpy_MRGAdjAmt + 
							                                Tpy_SHOLAdjAmt + 
							                                Tpy_MHOLAdjAmt 
					                                ELSE 0 END
	                                , Tas_Representation_INC = 0.00
	                                , Tas_Representation_YTD = 0.00
	                                , Tas_Transportation_INC = 0.00
	                                , Tas_Transportation_YTD = 0.00
	                                , Tas_CostLivingAllowance_INC = 0.00
	                                , Tas_CostLivingAllowance_YTD = 0.00
	                                , Tas_FixedHousingAllowance_INC = 0.00
	                                , Tas_FixedHousingAllowance_YTD = 0.00
                                    , Tas_RegularOtherTaxableIncome1_PAY = 0.00
	                                , Tas_RegularOtherTaxableIncome1_INC = 0.00
	                                , Tas_RegularOtherTaxableIncome1_YTD = 0.00
                                    , Tas_RegularOtherTaxableIncome2_PAY = 0.00
	                                , Tas_RegularOtherTaxableIncome2_INC = 0.00
	                                , Tas_RegularOtherTaxableIncome2_YTD = 0.00
	                                , Tas_Commision_INC = 0.00
	                                , Tas_Commision_YTD = 0.00
	                                , Tas_ProfitSharing_INC = 0.00
	                                , Tas_ProfitSharing_YTD = 0.00
	                                , Tas_Fees_INC = 0.00
	                                , Tas_Fees_YTD = 0.00
	                                , Tas_Hazard_INC = 0.00
	                                , Tas_Hazard_YTD = 0.00
	                                , Tas_Overtime_PAY = CASE WHEN Tpy_IsTaxExempted = 0 THEN Tpy_TotalOTNDAmt ELSE 0 END
	                                , Tas_Overtime_INC = 0.00
	                                , Tas_Overtime_YTD = 0.00
	                                , Tas_Overtime_ADJ =  CASE WHEN Tpy_IsTaxExempted = 0 THEN 
									                                Tpy_SOTAdjAmt +
									                                Tpy_MOTAdjAmt +
									                                Tpy_SNDAdjAmt +
									                                Tpy_MNDAdjAmt ELSE 0 END
                                    , Tas_SupplementaryOtherTaxableIncome1_PAY = 0.00
	                                , Tas_SupplementaryOtherTaxableIncome1_INC = 0.00
	                                , Tas_SupplementaryOtherTaxableIncome1_YTD = 0.00
                                    , Tas_SupplementaryOtherTaxableIncome2_PAY = 0.00
	                                , Tas_SupplementaryOtherTaxableIncome2_INC = 0.00
	                                , Tas_SupplementaryOtherTaxableIncome2_YTD = 0.00
	                                , Tas_MWEBasic_PAY =  CASE WHEN Tpy_IsTaxExempted = 1 THEN 
								                                Tpy_REGAmt +
								                                Tpy_PDLVAmt  
						                                   ELSE 0 END
	                                , Tas_MWEBasic_INC = 0.00
	                                , Tas_MWEBasic_YTD = 0.00
	                                , Tas_MWEBasic_ADJ = CASE WHEN Tpy_IsTaxExempted = 1 THEN 
							                                Tpy_SRGAdjAmt +
							                                Tpy_SLVAdjAmt +
							                                Tpy_MRGAdjAmt
						                                    ELSE 0 END
	                                , Tas_MWEHoliday_PAY = CASE WHEN Tpy_IsTaxExempted = 1 THEN 
								                                Tpy_PDLEGHOLAmt +
								                                Tpy_PDSPLHOLAmt +
								                                Tpy_PDCOMPHOLAmt +
								                                Tpy_PDPSDAmt +
								                                Tpy_PDOTHHOLAmt +
								                                Tpy_PDRESTLEGHOLAmt
							                                ELSE 0 END
	                                , Tas_MWEHoliday_INC = 0.00
	                                , Tas_MWEHoliday_YTD = 0.00
	                                , Tas_MWEHoliday_ADJ = CASE WHEN Tpy_IsTaxExempted = 1 THEN 
								                                Tpy_SHOLAdjAmt +
								                                Tpy_MHOLAdjAmt
								                                ELSE 0 END
	                                , Tas_MWEOvertime_PAY =  CASE WHEN Tpy_IsTaxExempted = 1 THEN 
								                                Tpy_REGOTAmt +
								                                Tpy_RESTAmt +
								                                Tpy_RESTOTAmt +
								                                Tpy_LEGHOLAmt +
								                                Tpy_LEGHOLOTAmt +
								                                Tpy_SPLHOLAmt +
								                                Tpy_SPLHOLOTAmt +
								                                Tpy_PSDAmt +
								                                Tpy_PSDOTAmt +
								                                Tpy_COMPHOLAmt +
								                                Tpy_COMPHOLOTAmt +
								                                Tpy_RESTLEGHOLAmt +
								                                Tpy_RESTLEGHOLOTAmt +
								                                Tpy_RESTSPLHOLAmt +
								                                Tpy_RESTSPLHOLOTAmt +
								                                Tpy_RESTCOMPHOLAmt +
								                                Tpy_RESTCOMPHOLOTAmt +
								                                Tpy_RESTPSDAmt +
								                                Tpy_RESTPSDOTAmt +
								                                ISNULL(Tpm_Misc1Amt, 0) +
								                                ISNULL(Tpm_Misc1OTAmt, 0) +
								                                ISNULL(Tpm_Misc2Amt, 0) +
								                                ISNULL(Tpm_Misc2OTAmt, 0) +
								                                ISNULL(Tpm_Misc3Amt, 0) +
								                                ISNULL(Tpm_Misc3OTAmt, 0) +
								                                ISNULL(Tpm_Misc4Amt, 0) +
								                                ISNULL(Tpm_Misc4OTAmt, 0) +
								                                ISNULL(Tpm_Misc5Amt, 0) +
								                                ISNULL(Tpm_Misc5OTAmt, 0) +
								                                ISNULL(Tpm_Misc6Amt, 0) +
								                                ISNULL(Tpm_Misc6OTAmt, 0)
							                                ELSE 0 END
	                                , Tas_MWEOvertime_INC = 0.00
	                                , Tas_MWEOvertime_YTD = 0.00
	                                , Tas_MWEOvertime_ADJ = CASE WHEN Tpy_IsTaxExempted = 1 THEN 
								                                Tpy_SOTAdjAmt +
								                                Tpy_MOTAdjAmt
							                                ELSE 0 END
	                                , Tas_MWENightShift_PAY = CASE WHEN Tpy_IsTaxExempted = 1 THEN 
								                                Tpy_REGNDAmt + 
								                                Tpy_REGNDOTAmt + 
								                                Tpy_RESTNDAmt + 
								                                Tpy_RESTNDOTAmt + 
								                                Tpy_LEGHOLNDAmt + 
								                                Tpy_LEGHOLNDOTAmt + 
								                                Tpy_SPLHOLNDAmt + 
								                                Tpy_SPLHOLNDOTAmt + 
								                                Tpy_PSDNDAmt + 
								                                Tpy_PSDNDOTAmt + 
								                                Tpy_COMPHOLNDAmt + 
								                                Tpy_COMPHOLNDOTAmt + 
								                                Tpy_RESTLEGHOLNDAmt + 
								                                Tpy_RESTLEGHOLNDOTAmt + 
								                                Tpy_RESTSPLHOLNDAmt + 
								                                Tpy_RESTSPLHOLNDOTAmt + 
								                                Tpy_RESTCOMPHOLNDAmt + 
								                                Tpy_RESTCOMPHOLNDOTAmt + 
								                                Tpy_RESTPSDNDAmt + 
								                                Tpy_RESTPSDNDOTAmt + 
								                                ISNULL(Tpm_Misc1NDAmt, 0) + 
								                                ISNULL(Tpm_Misc1NDOTAmt, 0) + 
								                                ISNULL(Tpm_Misc2NDAmt, 0) + 
								                                ISNULL(Tpm_Misc2NDOTAmt, 0) + 
								                                ISNULL(Tpm_Misc3NDAmt, 0) + 
								                                ISNULL(Tpm_Misc3NDOTAmt, 0) + 
								                                ISNULL(Tpm_Misc4NDAmt, 0) + 
								                                ISNULL(Tpm_Misc4NDOTAmt, 0) + 
								                                ISNULL(Tpm_Misc5NDAmt, 0) + 
								                                ISNULL(Tpm_Misc5NDOTAmt, 0) + 
								                                ISNULL(Tpm_Misc6NDAmt, 0) + 
								                                ISNULL(Tpm_Misc6NDOTAmt, 0) 
							                                ELSE 0 END
	                                , Tas_MWENightShift_INC = 0.00
	                                , Tas_MWENightShift_YTD = 0.00
	                                , Tas_MWENightShift_ADJ = CASE WHEN Tpy_IsTaxExempted = 1 THEN 
									                                Tpy_SNDAdjAmt  +
									                                Tpy_MNDAdjAmt
								                                ELSE 0 END
	                                , Tas_MWEHazard_INC = 0.00
	                                , Tas_MWEHazard_YTD = 0.00
                                    , Tas_13thMonthBen_PAY = 0.00
	                                , Tas_13thMonthBen_INC = 0.00
	                                , Tas_13thMonthBen_YTD = 0.00
	                                , Tas_DeMinimis_INC = 0.00
	                                , Tas_DeMinimis_YTD = 0.00
	                                , Tas_Premiums_PAY = Tpy_SSSEE +
                                                        Tpy_MPFEE +
						                                Tpy_PhilhealthEE +
						                                Tpy_PagIbigEE +
						                                Tpy_UnionAmt
	                                , Tas_Premiums_INC = 0.00
	                                , Tas_Premiums_DED = 0.00
	                                , Tas_Premiums_YTD = 0.00
	                                , Tas_NontaxableSalaries_INC = 0.00
	                                , Tas_NontaxableSalaries_YTD = 0.00
	                                , Tas_Tax_PAY = Tpy_WtaxAmt
	                                , Tas_Tax_INC = 0.00
	                                , Tas_Tax_DED = 0.00
	                                , Tas_Tax_YTD = 0.00
                                    , Tas_PremiumPaidOnHealth = 0.00
	                                , Usr_Login = '{4}'
	                                , Ludatetime = GETDATE()
                                FROM (SELECT * FROM {3}..Udv_Payroll
				                      UNION ALL
				                      SELECT * FROM {3}..Udv_PayrollFinalPay ) Payroll
                                WHERE LEFT(Tpy_PayCycle, 4) = '{0}'
                                {1}
                                {5}
                                {6} ";
            #endregion

            #region Insert Alphalist Staging Records
            DataTable dtDatabases = GetActiveProfiles(CompanyCode, CentralProfile);
            for (int i = 0; i < dtDatabases.Rows.Count; i++)
            {
                string query = string.Format(strQuery
                                            , TaxYear
                                            , EmployeeCondition2
                                            , dtDatabases.Rows[i]["Mpf_DatabaseNo"]
                                            , dtDatabases.Rows[i]["Mpf_DatabaseName"]
                                            , UserLogin
                                            , HoldPayrollCondition
                                            , FinalPayCondition);
                dalCentral.ExecuteNonQuery(query);
            }
            #endregion
        }

        private void PopulateW2StagingYTD(bool ProcessAll, bool bFinalPay, string EmployeeId, string EmployeeList, string TaxYear, string UserLogin, string CompanyCode, string CentralProfile)
        {
            string EmployeeCondition = "";
            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition = " AND Tas_IDNo = '" + EmployeeId + "' ";
            else if (ProcessAll == true && EmployeeList != "")
                EmployeeCondition = " AND Tas_IDNo IN (" + EmployeeList + ") ";

            string EmployeeCondition2 = "";
            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition2 = " AND Tyt_IDNo = '" + EmployeeId + "' ";
            else if (ProcessAll == true && EmployeeList != "")
                EmployeeCondition2 = " AND Tyt_IDNo IN (" + EmployeeList + ") ";

            #region Delete Alphalist Staging Records Before Insert
            string strDeleteQuery = string.Format(@"DELETE
												    FROM T_EmpAlphalistStaging
                                                    WHERE Tas_TaxYear = '{0}'
													AND Tas_Datasource IN ('C','P','S')
                                                    {1} ", TaxYear
                                                        , EmployeeCondition);
            dalCentral.ExecuteNonQuery(strDeleteQuery);
            #endregion

            #region Template Insert Query
            string strQuery = @"INSERT INTO T_EmpAlphalistStaging (Tas_TaxYear,Tas_IDNo,Tas_PayCycle,Tas_ProfileCode,Tas_DataSource,Tas_SalaryRate,Tas_IsTaxExempted,Tas_Basic_PAY,Tas_Basic_INC,Tas_Basic_YTD,Tas_Basic_ADJ,Tas_Representation_INC,Tas_Representation_YTD,Tas_Transportation_INC,Tas_Transportation_YTD,Tas_CostLivingAllowance_INC,Tas_CostLivingAllowance_YTD,Tas_FixedHousingAllowance_INC,Tas_FixedHousingAllowance_YTD,Tas_RegularOtherTaxableIncome1_PAY,Tas_RegularOtherTaxableIncome1_INC,Tas_RegularOtherTaxableIncome1_YTD,Tas_RegularOtherTaxableIncome2_PAY,Tas_RegularOtherTaxableIncome2_INC,Tas_RegularOtherTaxableIncome2_YTD,Tas_Commision_INC,Tas_Commision_YTD,Tas_ProfitSharing_INC,Tas_ProfitSharing_YTD,Tas_Fees_INC,Tas_Fees_YTD,Tas_Hazard_INC,Tas_Hazard_YTD,Tas_Overtime_PAY,Tas_Overtime_INC,Tas_Overtime_YTD,Tas_Overtime_ADJ,Tas_SupplementaryOtherTaxableIncome1_PAY,Tas_SupplementaryOtherTaxableIncome1_INC,Tas_SupplementaryOtherTaxableIncome1_YTD,Tas_SupplementaryOtherTaxableIncome2_PAY,Tas_SupplementaryOtherTaxableIncome2_INC,Tas_SupplementaryOtherTaxableIncome2_YTD,Tas_MWEBasic_PAY,Tas_MWEBasic_INC,Tas_MWEBasic_YTD,Tas_MWEBasic_ADJ,Tas_MWEHoliday_PAY,Tas_MWEHoliday_INC,Tas_MWEHoliday_YTD,Tas_MWEHoliday_ADJ,Tas_MWEOvertime_PAY,Tas_MWEOvertime_INC,Tas_MWEOvertime_YTD,Tas_MWEOvertime_ADJ,Tas_MWENightShift_PAY,Tas_MWENightShift_INC,Tas_MWENightShift_YTD,Tas_MWENightShift_ADJ,Tas_MWEHazard_INC,Tas_MWEHazard_YTD,Tas_13thMonthBen_PAY,Tas_13thMonthBen_INC,Tas_13thMonthBen_YTD,Tas_DeMinimis_INC,Tas_DeMinimis_YTD,Tas_Premiums_PAY,Tas_Premiums_INC,Tas_Premiums_DED,Tas_Premiums_YTD,Tas_NontaxableSalaries_INC,Tas_NontaxableSalaries_YTD,Tas_Tax_PAY,Tas_Tax_INC,Tas_Tax_DED,Tas_Tax_YTD,Tas_PremiumPaidOnHealth,Usr_Login,Ludatetime)
                                SELECT Tas_TaxYear = '{0}'
	                            , Tas_IDNo              = Tyt_IDNo
	                            , Tas_PayCycle          = '{0}' + '000'
	                            , Tas_ProfileCode       = '000'
	                            , Tas_Datasource        = Tyt_EmployerType
                                , Tas_SalaryRate        = 0
	                            , Tas_IsTaxExempted     = 0
	                            , Tas_Basic_PAY         = 0.00
	                            , Tas_Basic_INC         = 0.00
	                            , Tas_Basic_YTD         = Tyt_TaxableBasic
	                            , Tas_Basic_ADJ         = 0.00
	                            , Tas_Representation_INC = 0.00
	                            , Tas_Representation_YTD = Tyt_Representation
	                            , Tas_Transportation_INC = 0.00
	                            , Tas_Transportation_YTD = Tyt_Transportation
	                            , Tas_CostLivingAllowance_INC = 0.00
	                            , Tas_CostLivingAllowance_YTD = Tyt_CostLivingAllowance
	                            , Tas_FixedHousingAllowance_INC = 0.00
	                            , Tas_FixedHousingAllowance_YTD = Tyt_FixedHousingAllowance
                                , Tas_RegularOtherTaxableIncome1_PAY = 0.00
	                            , Tas_RegularOtherTaxableIncome1_INC = 0.00
	                            , Tas_RegularOtherTaxableIncome1_YTD = Tyt_OtherTaxable1
                                , Tas_RegularOtherTaxableIncome2_PAY = 0.00 
	                            , Tas_RegularOtherTaxableIncome2_INC = 0.00
	                            , Tas_RegularOtherTaxableIncome2_YTD = Tyt_OtherTaxable2
	                            , Tas_Commision_INC     = 0.00
	                            , Tas_Commision_YTD     = Tyt_Commision
	                            , Tas_ProfitSharing_INC = 0.00
	                            , Tas_ProfitSharing_YTD = Tyt_ProfitSharing
	                            , Tas_Fees_INC          = 0.00
	                            , Tas_Fees_YTD          = Tyt_Fees
	                            , Tas_Hazard_INC        = 0.00
	                            , Tas_Hazard_YTD        = Tyt_Hazard
	                            , Tas_Overtime_PAY      = 0.00
	                            , Tas_Overtime_INC      = 0.00
	                            , Tas_Overtime_YTD      = Tyt_TaxableOvertime
	                            , Tas_Overtime_ADJ      = 0.00
                                , Tas_SupplementaryOtherTaxableIncome1_PAY = 0.00
	                            , Tas_SupplementaryOtherTaxableIncome1_INC = 0.00
	                            , Tas_SupplementaryOtherTaxableIncome1_YTD = Tyt_SupplementaryTaxable1
                                , Tas_SupplementaryOtherTaxableIncome2_PAY = 0.00
	                            , Tas_SupplementaryOtherTaxableIncome2_INC = 0.00
	                            , Tas_SupplementaryOtherTaxableIncome2_YTD = Tyt_SupplementaryTaxable2
	                            , Tas_MWEBasic_PAY      = 0.00
	                            , Tas_MWEBasic_INC      = 0.00
	                            , Tas_MWEBasic_YTD      = Tyt_MWEBasic
	                            , Tas_MWEBasic_ADJ      = 0.00
	                            , Tas_MWEHoliday_PAY    = 0.00
	                            , Tas_MWEHoliday_INC    = 0.00
	                            , Tas_MWEHoliday_YTD    = Tyt_MWEHoliday
	                            , Tas_MWEHoliday_ADJ    = 0.00
	                            , Tas_MWEOvertime_PAY   = 0.00
	                            , Tas_MWEOvertime_INC   = 0.00
	                            , Tas_MWEOvertime_YTD   = Tyt_MWEOvertime
	                            , Tas_MWEOvertime_ADJ   = 0.00
	                            , Tas_MWENightShift_PAY = 0.00
	                            , Tas_MWENightShift_INC = 0.00
	                            , Tas_MWENightShift_YTD = Tyt_MWENightShift
	                            , Tas_MWENightShift_ADJ = 0.00
	                            , Tas_MWEHazard_INC     = 0.00
	                            , Tas_MWEHazard_YTD     = Tyt_MWEHazard
                                , Tas_13thMonthBen_PAY  = 0.00
	                            , Tas_13thMonthBen_INC  = 0.00
	                            , Tas_13thMonthBen_YTD  = Tyt_Nontaxable13thMonth + Tyt_Taxable13thMonth
	                            , Tas_DeMinimis_INC     = 0.00
	                            , Tas_DeMinimis_YTD     = Tyt_DeMinimis
	                            , Tas_Premiums_PAY      = 0.00
	                            , Tas_Premiums_INC      = 0.00
	                            , Tas_Premiums_DED      = 0.00
	                            , Tas_Premiums_YTD      = Tyt_PremiumsUnionDues
	                            , Tas_NontaxableSalaries_INC = 0.00
	                            , Tas_NontaxableSalaries_YTD = Tyt_NontaxableSalariesCompensation
	                            , Tas_Tax_PAY           = 0.00
	                            , Tas_Tax_INC           = 0.00
	                            , Tas_Tax_DED           = 0.00
	                            , Tas_Tax_YTD           = Tyt_TaxWithheld
                                , Tas_PremiumPaidOnHealth = Tyt_PremiumPaidOnHealth
	                            , Usr_Login             = '{2}'
	                            , Ludatetime            = GETDATE()
                            FROM (  SELECT Tyt_IDNo
									,Tyt_TaxYear
									,Tyt_EmployerType
									,SUM(Tyt_MWEBasic) Tyt_MWEBasic
									,SUM(Tyt_MWEHoliday) Tyt_MWEHoliday
									,SUM(Tyt_MWEOvertime) Tyt_MWEOvertime
									,SUM(Tyt_MWENightShift) Tyt_MWENightShift
									,SUM(Tyt_MWEHazard) Tyt_MWEHazard
									,SUM(Tyt_Nontaxable13thMonth) Tyt_Nontaxable13thMonth
									,SUM(Tyt_DeMinimis) Tyt_DeMinimis
									,SUM(Tyt_PremiumsUnionDues) Tyt_PremiumsUnionDues
									,SUM(Tyt_NontaxableSalariesCompensation) Tyt_NontaxableSalariesCompensation
									,SUM(Tyt_TaxableBasic) Tyt_TaxableBasic
									,SUM(Tyt_Taxable13thMonth) Tyt_Taxable13thMonth
									,SUM(Tyt_TaxableOvertime) Tyt_TaxableOvertime
									,SUM(Tyt_Hazard) Tyt_Hazard
									,SUM(Tyt_PremiumPaidOnHealth) Tyt_PremiumPaidOnHealth
									,SUM(Tyt_TaxWithheld) Tyt_TaxWithheld
									,SUM(Tyt_Representation) Tyt_Representation
									,SUM(Tyt_Transportation) Tyt_Transportation
									,SUM(Tyt_CostLivingAllowance) Tyt_CostLivingAllowance
									,SUM(Tyt_FixedHousingAllowance) Tyt_FixedHousingAllowance
									,SUM(Tyt_OtherTaxable1) Tyt_OtherTaxable1
									,SUM(Tyt_OtherTaxable2) Tyt_OtherTaxable2
									,SUM(Tyt_Commision) Tyt_Commision
									,SUM(Tyt_ProfitSharing) Tyt_ProfitSharing
									,SUM(Tyt_Fees) Tyt_Fees
									,SUM(Tyt_SupplementaryTaxable1) Tyt_SupplementaryTaxable1
									,SUM(Tyt_SupplementaryTaxable2) Tyt_SupplementaryTaxable2
									FROM Udv_YearToDate
                                    WHERE Tyt_EmployerType <> 'M'
									GROUP BY Tyt_IDNo,Tyt_TaxYear,Tyt_EmployerType
								 ) TMP
                            WHERE Tyt_TaxYear = '{0}'
                            {1} ";
            #endregion

            #region Insert Alphalist Staging Records
            string query = string.Format(strQuery
                                        , TaxYear
                                        , EmployeeCondition2
                                        , UserLogin);
            dalCentral.ExecuteNonQuery(query);
            #endregion
        }

        private void UpdateW2StagingIncomeDeduction(bool ProcessAll, string EmployeeId, string EmployeeList, string TaxYear, string UserLogin, string CompanyCode, string CentralProfile)
        {
            string EmployeeCondition = "";
            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition = " AND Tas_IDNo = '" + EmployeeId + "' ";
            else if (ProcessAll == true && EmployeeList != "")
                EmployeeCondition = " AND Tas_IDNo IN (" + EmployeeList + ") ";

            #region Template Update Query
            string strQuery = @"UPDATE T_EmpAlphalistStaging
                    SET Tas_Basic_INC = CASE WHEN Tas_IsTaxExempted = 0 THEN  ISNULL(Income.[T-BASICSAL], 0) + ISNULL(Income.[T-HOLIDAYPY], 0) ELSE 0 END
	                    , Tas_Representation_INC = ISNULL(Income.[T-REPRESENTN], 0)
	                    , Tas_Transportation_INC = ISNULL(Income.[T-TRANSPO], 0)
	                    , Tas_CostLivingAllowance_INC = ISNULL(Income.[T-COLA], 0)
	                    , Tas_FixedHousingAllowance_INC = ISNULL(Income.[T-FHA], 0)
	                    , Tas_RegularOtherTaxableIncome1_INC = ISNULL(Income.[T-OTHERTAX1], 0)
	                    , Tas_RegularOtherTaxableIncome2_INC = ISNULL(Income.[T-OTHERTAX2], 0)
	                    , Tas_Commision_INC = ISNULL(Income.[T-COMMISSION], 0)
	                    , Tas_ProfitSharing_INC = ISNULL(Income.[T-PROFITSHR], 0)
	                    , Tas_Fees_INC = ISNULL(Income.[T-FEES], 0)
	                    , Tas_Hazard_INC = CASE WHEN Tas_IsTaxExempted = 0 THEN  ISNULL(Income.[T-HAZARDPY], 0) ELSE 0 END
	                    , Tas_Overtime_INC = CASE WHEN Tas_IsTaxExempted = 0 THEN ISNULL(Income.[T-OVERTIMEPY], 0) + ISNULL(Income.[T-NDPAY], 0) ELSE 0 END
	                    , Tas_SupplementaryOtherTaxableIncome1_INC = ISNULL(Income.[T-OTHERSUPL1], 0)
	                    , Tas_SupplementaryOtherTaxableIncome2_INC = ISNULL(Income.[T-OTHERSUPL2], 0)
	                    , Tas_MWEBasic_INC =  CASE WHEN Tas_IsTaxExempted = 1 THEN  ISNULL(Income.[T-BASICSAL], 0) ELSE 0 END
	                    , Tas_MWEHoliday_INC = CASE WHEN Tas_IsTaxExempted = 1 THEN ISNULL(Income.[T-HOLIDAYPY], 0)  ELSE 0 END
	                    , Tas_MWEOvertime_INC = CASE WHEN Tas_IsTaxExempted = 1 THEN ISNULL(Income.[T-OVERTIMEPY], 0) ELSE 0 END
	                    , Tas_MWENightShift_INC =  CASE WHEN Tas_IsTaxExempted = 1 THEN ISNULL(Income.[T-NDPAY], 0) ELSE 0 END
	                    , Tas_MWEHazard_INC = CASE WHEN Tas_IsTaxExempted = 1 THEN  ISNULL(Income.[T-HAZARDPY], 0) ELSE 0 END
	                    , Tas_13thMonthBen_INC = ISNULL(Income.[N-13THBEN], 0) + ISNULL(Income.[T-13THBEN], 0)
	                    , Tas_DeMinimis_INC = ISNULL(Income.[N-DEMINIMIS], 0)
	                   	, Tas_Premiums_INC = ISNULL(Income.[N-PREMIUMS] * -1, 0) ---Minus
	                    , Tas_NontaxableSalaries_INC = ISNULL(Income.[N-SALOTH], 0)
	                    , Tas_Tax_INC = ISNULL(Income.[N-WTAX] * -1, 0) ---Minus
	                    , Tas_Premiums_DED= ISNULL(Deduction.[N-PREMIUMS], 0)
	                    , Tas_Tax_DED= ISNULL(Deduction.[N-WTAX], 0)
                    FROM T_EmpAlphalistStaging
                    LEFT JOIN (	SELECT Tdd_IDNo
			                    , Tdd_ThisPayCycle
			                    , SUM(CASE WHEN Mdn_AlphalistCategory = 'N-PREMIUMS'
				                    THEN Tdd_Amount
				                    ELSE 0
				                    END) as [N-PREMIUMS]
			                    , SUM(CASE WHEN Mdn_AlphalistCategory = 'N-WTAX'
				                    THEN Tdd_Amount
				                    ELSE 0
				                    END) as [N-WTAX]
		                    FROM {3}..Udv_Deduction
		                    INNER JOIN M_Deduction 
		                        ON Tdd_DeductionCode = Mdn_DeductionCode
                                AND Mdn_CompanyCode = '{4}'
		                    WHERE Tdd_PaymentFlag = 1
		                    GROUP BY Tdd_IDNo, Tdd_ThisPayCycle) Deduction ON Tas_IDNo = Deduction.Tdd_IDNo
			                    AND Tas_PayCycle = Deduction.Tdd_ThisPayCycle
			                    AND Tas_DataSource = 'A'
			                    AND Tas_ProfileCode = '{2}'
                    LEFT JOIN (	SELECT Tin_IDNo
			                    , Tin_PayCycle
			                    , SUM(CASE WHEN Min_AlphalistCategory = 'T-BASICSAL'
				                    THEN Tin_IncomeAmt
				                    ELSE 0
				                    END) as [T-BASICSAL]
			                    , SUM(CASE WHEN Min_AlphalistCategory = 'T-COLA'
				                    THEN Tin_IncomeAmt
				                    ELSE 0
				                    END) as [T-COLA]
			                    , SUM(CASE WHEN Min_AlphalistCategory = 'T-COMMISSION'
				                    THEN Tin_IncomeAmt
				                    ELSE 0
				                    END) as [T-COMMISSION]
			                    , SUM(CASE WHEN Min_AlphalistCategory = 'T-FEES'
				                    THEN Tin_IncomeAmt
				                    ELSE 0
				                    END) as [T-FEES]
			                    , SUM(CASE WHEN Min_AlphalistCategory = 'T-FHA'
				                    THEN Tin_IncomeAmt
				                    ELSE 0
				                    END) as [T-FHA]
			                    , SUM(CASE WHEN Min_AlphalistCategory = 'T-HAZARDPY'
				                    THEN Tin_IncomeAmt
				                    ELSE 0
				                    END) as [T-HAZARDPY]
			                    , SUM(CASE WHEN Min_AlphalistCategory = 'T-HOLIDAYPY'
				                    THEN Tin_IncomeAmt
				                    ELSE 0
				                    END) as [T-HOLIDAYPY]
			                    , SUM(CASE WHEN Min_AlphalistCategory = 'T-NDPAY'
				                    THEN Tin_IncomeAmt
				                    ELSE 0
				                    END) as [T-NDPAY]
			                    , SUM(CASE WHEN Min_AlphalistCategory = 'T-OTHERSUPL1'
				                    THEN Tin_IncomeAmt
				                    ELSE 0
				                    END) as [T-OTHERSUPL1]
			                    , SUM(CASE WHEN Min_AlphalistCategory = 'T-OTHERSUPL2'
				                    THEN Tin_IncomeAmt
				                    ELSE 0
				                    END) as [T-OTHERSUPL2]
			                    , SUM(CASE WHEN Min_AlphalistCategory = 'T-OTHERTAX1'
				                    THEN Tin_IncomeAmt
				                    ELSE 0
				                    END) as [T-OTHERTAX1]
			                    , SUM(CASE WHEN Min_AlphalistCategory = 'T-OTHERTAX2'
				                    THEN Tin_IncomeAmt
				                    ELSE 0
				                    END) as [T-OTHERTAX2]
			                    , SUM(CASE WHEN Min_AlphalistCategory = 'T-OVERTIMEPY'
				                    THEN Tin_IncomeAmt
				                    ELSE 0
				                    END) as [T-OVERTIMEPY]
			                    , SUM(CASE WHEN Min_AlphalistCategory = 'T-PROFITSHR'
				                    THEN Tin_IncomeAmt
				                    ELSE 0
				                    END) as [T-PROFITSHR]
			                    , SUM(CASE WHEN Min_AlphalistCategory = 'T-REPRESENTN'
				                    THEN Tin_IncomeAmt
				                    ELSE 0
				                    END) as [T-REPRESENTN]
			                    , SUM(CASE WHEN Min_AlphalistCategory = 'T-13THBEN'
				                    THEN Tin_IncomeAmt
				                    ELSE 0
				                    END) as [T-13THBEN]
			                    , SUM(CASE WHEN Min_AlphalistCategory = 'T-TRANSPO'
				                    THEN Tin_IncomeAmt
				                    ELSE 0
				                    END) as [T-TRANSPO]
			                    , SUM(CASE WHEN Min_AlphalistCategory = 'N-13THBEN'
				                    THEN Tin_IncomeAmt
				                    ELSE 0
				                    END) as [N-13THBEN]
			                    , SUM(CASE WHEN Min_AlphalistCategory = 'N-DEMINIMIS'
				                    THEN Tin_IncomeAmt
				                    ELSE 0
				                    END) as [N-DEMINIMIS]
			                    , SUM(CASE WHEN Min_AlphalistCategory = 'N-PREMIUMS'
				                    THEN Tin_IncomeAmt
				                    ELSE 0
				                    END) as [N-PREMIUMS]
			                    , SUM(CASE WHEN Min_AlphalistCategory = 'N-SALOTH'
				                    THEN Tin_IncomeAmt
				                    ELSE 0
				                    END) as [N-SALOTH]
			                    , SUM(CASE WHEN Min_AlphalistCategory = 'N-WTAX'
				                    THEN Tin_IncomeAmt
				                    ELSE 0
				                    END) as [N-WTAX]
                            FROM {3}..Udv_Income
                            INNER JOIN M_Income 
                                ON Min_IncomeCode = Tin_IncomeCode
                                AND Min_CompanyCode = '{4}'
                            WHERE Tin_PostFlag = 1
		                    GROUP BY Tin_IDNo, Tin_PayCycle) Income
		                    ON Tas_IDNo = Income.Tin_IDNo
			                    AND Tas_PayCycle = Income.Tin_PayCycle
			                    AND Tas_DataSource = 'A'
			                    AND Tas_ProfileCode = '{2}'
                    WHERE LEFT(Tas_PayCycle, 4) = '{0}'
	                    AND Tas_DataSource = 'A'
	                    AND Tas_ProfileCode = '{2}'
                        {1} ";
            #endregion

            #region Update Alphalist Staging Records
            DataTable dtDatabases = GetActiveProfiles(CompanyCode, CentralProfile);
            for (int i = 0; i < dtDatabases.Rows.Count; i++)
            {
                string query = string.Format(strQuery
                                            , TaxYear
                                            , EmployeeCondition
                                            , dtDatabases.Rows[i]["Mpf_DatabaseNo"]
                                            , dtDatabases.Rows[i]["Mpf_DatabaseName"]
                                            , CompanyCode);
                dalCentral.ExecuteNonQuery(query);
            }
            #endregion
        }

        private void PopulateW2StagingEstimateValues(bool ProcessAll, bool bFinalPay, string EmployeeId, string EmployeeList, string TaxYear, string RCURALPHACATGY, string UserLogin, string CompanyCode, string CentralProfile)
        {
            string EmployeeCondition = "";
            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition = " AND Tas_IDNo = '" + EmployeeId + "' ";
            else if (ProcessAll == true && EmployeeList != "")
                EmployeeCondition = " AND Tas_IDNo IN (" + EmployeeList + ") ";

            string EmployeeCondition2 = "";
            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition2 = " AND Tah_IDNo = '" + EmployeeId + "' ";
            else if (ProcessAll == true && EmployeeList != "")
                EmployeeCondition2 = " AND Tah_IDNo IN (" + EmployeeList + ") AND Tah_AssumePayCycle > 0 ";


            #region Delete Alphalist Staging Records Before Insert
            string strDeleteQuery = string.Format(@"DELETE A
												    FROM T_EmpAlphalistStaging A
												    INNER JOIN T_EmpAlphalistHdr B
													ON Tah_IDNo = Tas_IDNo
	                                                AND Tah_TaxYear = Tas_TaxYear
                                                    WHERE Tas_TaxYear = '{0}'
													    AND Tas_Datasource = 'E' 
                                                        {1}
                                                        ", TaxYear
                                                        , EmployeeCondition);
            dalCentral.ExecuteNonQuery(strDeleteQuery);
            #endregion

            #region Template Insert Query
            string strQuery = @"INSERT INTO T_EmpAlphalistStaging (Tas_TaxYear,Tas_IDNo,Tas_PayCycle,Tas_ProfileCode,Tas_DataSource,Tas_SalaryRate,Tas_IsTaxExempted,Tas_Basic_PAY,Tas_Basic_INC,Tas_Basic_YTD,Tas_Basic_ADJ,Tas_Representation_INC,Tas_Representation_YTD,Tas_Transportation_INC,Tas_Transportation_YTD,Tas_CostLivingAllowance_INC,Tas_CostLivingAllowance_YTD,Tas_FixedHousingAllowance_INC,Tas_FixedHousingAllowance_YTD,Tas_RegularOtherTaxableIncome1_PAY,Tas_RegularOtherTaxableIncome1_INC,Tas_RegularOtherTaxableIncome1_YTD,Tas_RegularOtherTaxableIncome2_PAY,Tas_RegularOtherTaxableIncome2_INC,Tas_RegularOtherTaxableIncome2_YTD,Tas_Commision_INC,Tas_Commision_YTD,Tas_ProfitSharing_INC,Tas_ProfitSharing_YTD,Tas_Fees_INC,Tas_Fees_YTD,Tas_Hazard_INC,Tas_Hazard_YTD,Tas_Overtime_PAY,Tas_Overtime_INC,Tas_Overtime_YTD,Tas_Overtime_ADJ,Tas_SupplementaryOtherTaxableIncome1_PAY,Tas_SupplementaryOtherTaxableIncome1_INC,Tas_SupplementaryOtherTaxableIncome1_YTD,Tas_SupplementaryOtherTaxableIncome2_PAY,Tas_SupplementaryOtherTaxableIncome2_INC,Tas_SupplementaryOtherTaxableIncome2_YTD,Tas_MWEBasic_PAY,Tas_MWEBasic_INC,Tas_MWEBasic_YTD,Tas_MWEBasic_ADJ,Tas_MWEHoliday_PAY,Tas_MWEHoliday_INC,Tas_MWEHoliday_YTD,Tas_MWEHoliday_ADJ,Tas_MWEOvertime_PAY,Tas_MWEOvertime_INC,Tas_MWEOvertime_YTD,Tas_MWEOvertime_ADJ,Tas_MWENightShift_PAY,Tas_MWENightShift_INC,Tas_MWENightShift_YTD,Tas_MWENightShift_ADJ,Tas_MWEHazard_INC,Tas_MWEHazard_YTD,Tas_13thMonthBen_PAY,Tas_13thMonthBen_INC,Tas_13thMonthBen_YTD,Tas_DeMinimis_INC,Tas_DeMinimis_YTD,Tas_Premiums_PAY,Tas_Premiums_INC,Tas_Premiums_DED,Tas_Premiums_YTD,Tas_NontaxableSalaries_INC,Tas_NontaxableSalaries_YTD,Tas_Tax_PAY,Tas_Tax_INC,Tas_Tax_DED,Tas_Tax_YTD,Tas_PremiumPaidOnHealth,Usr_Login,Ludatetime)
                                SELECT  Tas_TaxYear = '{0}'
	                            , Tas_IDNo                          = Tah_IDNo
	                            , Tas_PayCycle                      = '{0}' + '000'
	                            , Tas_ProfileCode                   = '000'
	                            , Tas_Datasource                    = 'E'
                                , Tas_SalaryRate                    = 0.00
	                            , Tas_IsTaxExempted                 = Tah_IsTaxExempted
	                            , Tas_Basic_PAY                     = CASE WHEN Tah_IsTaxExempted = 0 THEN Tah_AssumeBasic ELSE 0 END
	                            , Tas_Basic_INC                     = 0.00
	                            , Tas_Basic_YTD                     = 0.00
	                            , Tas_Basic_ADJ                     = 0.00
	                            , Tas_Representation_INC            = 0.00
	                            , Tas_Representation_YTD            = 0.00
	                            , Tas_Transportation_INC            = 0.00
	                            , Tas_Transportation_YTD            = 0.00
	                            , Tas_CostLivingAllowance_INC       = 0.00
	                            , Tas_CostLivingAllowance_YTD       = 0.00
	                            , Tas_FixedHousingAllowance_INC     = 0.00
	                            , Tas_FixedHousingAllowance_YTD     = 0.00
                                , Tas_RegularOtherTaxableIncome1_PAY = CASE WHEN '{3}' = 'T-OTHERTAX1' THEN  Tah_AssumeSalariesCompensation ELSE 0 END
	                            , Tas_RegularOtherTaxableIncome1_INC = 0.00
	                            , Tas_RegularOtherTaxableIncome1_YTD = 0.00
                                , Tas_RegularOtherTaxableIncome2_PAY = CASE WHEN '{3}' = 'T-OTHERTAX2' THEN  Tah_AssumeSalariesCompensation ELSE 0 END
	                            , Tas_RegularOtherTaxableIncome2_INC = 0.00
	                            , Tas_RegularOtherTaxableIncome2_YTD = 0.00
	                            , Tas_Commision_INC                 = 0.00
	                            , Tas_Commision_YTD                 = 0.00
	                            , Tas_ProfitSharing_INC             = 0.00
	                            , Tas_ProfitSharing_YTD             = 0.00
	                            , Tas_Fees_INC                      = 0.00
	                            , Tas_Fees_YTD                      = 0.00
	                            , Tas_Hazard_INC                    = 0.00
	                            , Tas_Hazard_YTD                    = 0.00
	                            , Tas_Overtime_PAY                  = 0.00
	                            , Tas_Overtime_INC                  = 0.00
	                            , Tas_Overtime_YTD                  = 0.00
	                            , Tas_Overtime_ADJ                  =  0.00
                                , Tas_SupplementaryOtherTaxableIncome1_PAY = CASE WHEN '{3}' = 'T-OTHERSUPL1' THEN  Tah_AssumeSalariesCompensation ELSE 0 END
	                            , Tas_SupplementaryOtherTaxableIncome1_INC = 0.00
	                            , Tas_SupplementaryOtherTaxableIncome1_YTD = 0.00
                                , Tas_SupplementaryOtherTaxableIncome2_PAY = CASE WHEN '{3}' = 'T-OTHERSUPL2' THEN  Tah_AssumeSalariesCompensation ELSE 0 END
	                            , Tas_SupplementaryOtherTaxableIncome2_INC = 0.00
	                            , Tas_SupplementaryOtherTaxableIncome2_YTD = 0.00
	                            , Tas_MWEBasic_PAY                  = CASE WHEN Tah_IsTaxExempted = 1 THEN Tah_AssumeBasic ELSE 0 END
	                            , Tas_MWEBasic_INC                  = 0.00
	                            , Tas_MWEBasic_YTD                  = 0.00
	                            , Tas_MWEBasic_ADJ                  = 0.00
	                            , Tas_MWEHoliday_PAY                = 0.00
	                            , Tas_MWEHoliday_INC                = 0.00
	                            , Tas_MWEHoliday_YTD                = 0.00
	                            , Tas_MWEHoliday_ADJ                = 0.00
	                            , Tas_MWEOvertime_PAY               = 0.00
	                            , Tas_MWEOvertime_INC               = 0.00
	                            , Tas_MWEOvertime_YTD               = 0.00
	                            , Tas_MWEOvertime_ADJ               = 0.00
	                            , Tas_MWENightShift_PAY             = 0.00
	                            , Tas_MWENightShift_INC             = 0.00
	                            , Tas_MWENightShift_YTD             = 0.00
	                            , Tas_MWENightShift_ADJ             = 0.00
	                            , Tas_MWEHazard_INC                 = 0.00
	                            , Tas_MWEHazard_YTD                 = 0.00
                                , Tas_13thMonthBen_PAY              = Tah_Assume13th
	                            , Tas_13thMonthBen_INC              = 0.00
	                            , Tas_13thMonthBen_YTD              = 0.00
	                            , Tas_DeMinimis_INC                 = 0.00
	                            , Tas_DeMinimis_YTD                 = 0.00
	                            , Tas_Premiums_PAY                  = Tah_AssumePremiumsUnionDues
	                            , Tas_Premiums_INC                  = 0.00
	                            , Tas_Premiums_DED                  = 0.00
	                            , Tas_Premiums_YTD                  = 0.00
	                            , Tas_NontaxableSalaries_INC        = 0.00
	                            , Tas_NontaxableSalaries_YTD        = 0.00
	                            , Tas_Tax_PAY                       = 0.00
	                            , Tas_Tax_INC                       = 0.00
	                            , Tas_Tax_DED                       = 0.00
	                            , Tas_Tax_YTD                       = 0.00
                                , Tas_PremiumPaidOnHealth           = 0.00
	                            , Usr_Login                         = '{2}'
	                            , Ludatetime                        = GETDATE()
                            FROM T_EmpAlphalistHdr
                            WHERE Tah_TaxYear = '{0}'
                                {1} ";
            #endregion

            #region Insert Alphalist Staging Records
            string query = string.Format(strQuery
                                        , TaxYear
                                        , EmployeeCondition2
                                        , UserLogin
                                        , RCURALPHACATGY);
            dalCentral.ExecuteNonQuery(query);
            #endregion
        }

        private void DeleteW2Records(bool ProcessAll, bool bFinalPay, string EmployeeId, string EmployeeList, string TaxYear)
        {
            string EmployeeCondition = "";
            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition = " AND Tah_IDNo = '" + EmployeeId + "' ";
            else if (ProcessAll == true && EmployeeList != "")
                EmployeeCondition = " AND Tah_IDNo IN (" + EmployeeList + ") ";

            string strQuery = string.Format(@"DELETE A
                                                FROM T_EmpAlphalistDtl A
                                                INNER JOIN T_EmpAlphalistHdr B
                                                ON Tah_IDNo = Tad_IDNo
	                                                AND Tah_TaxYear = Tad_TaxYear
                                                WHERE Tah_TaxYear = '{0}'
                                                {1}

                                                DELETE FROM T_EmpAlphalistHdr
                                                WHERE Tah_TaxYear = '{0}'
                                                {1}
                                                ", TaxYear
                                                , EmployeeCondition);

            dalCentral.ExecuteNonQuery(strQuery);
        }

        private DataTable GetEmployeeUserGeneratedW2(bool ProcessAll, bool bFinalPay, string EmployeeId, string EmployeeList, string TaxYear)
        {
            string EmployeeCondition = "";
            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition = " AND Tah_IDNo = '" + EmployeeId + "' ";
            else if (ProcessAll == true && EmployeeList != "")
                EmployeeCondition = " AND Tah_IDNo IN (" + EmployeeList + ") ";

            string strQuery = string.Format(@"SELECT Tah_IDNo FROM T_EmpAlphalistHdr
                                                WHERE Tah_TaxYear = '{0}'
                                                {1}
                                                ", TaxYear
                                                , EmployeeCondition);

            DataTable dtResult = dalCentral.ExecuteDataSet(strQuery).Tables[0];
            return dtResult;
        }

        private DataTable GetEmployeeAnnualizedPayrollDec2ndQ(bool ProcessAll, string EmployeeId, string EmployeeList, string PayPeriod, string LoginDBName)
        {
            string EmployeeCondition = "";
            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition = " AND Tpy_IDNo = '" + EmployeeId + "' ";
            else if (ProcessAll == true && EmployeeList != "")
                EmployeeCondition = " AND Tpy_IDNo IN (" + EmployeeList + ") ";

            #region Query
            string strQuery = string.Format(@"SELECT Tpy_IDNo
                                                    , Tpy_TaxCode
                                                    , Tpy_TotalExemptions
                                                    , Tpy_YTDWtaxAmt
                                                    , Tpy_YTDWtaxAmtBefore
                                                    , Tpy_WtaxAmt
                                                    , Tpy_IsTaxExempted
                                                    , Tpy_CostcenterCode
													, Tpy_PayrollGroup
                                                    , Tpy_PayrollType
													, Tpy_EmploymentStatus
													, Tpy_PositionCode
                                                    , Tpy_PositionGrade
                                                    , Tpy_WorkStatus
                                                FROM (SELECT * FROM {2}..Udv_Payroll
				                                      UNION ALL
				                                      SELECT * FROM {2}..Udv_PayrollFinalPay ) Payroll
                                                WHERE Tpy_PayCycle = '{0}'
                                                {1}
                                                ", PayPeriod, EmployeeCondition, LoginDBName);
            #endregion

            DataTable dtResult = dalCentral.ExecuteDataSet(strQuery).Tables[0];
            return dtResult;
        }

        private DataTable GetYTDRecords(bool ProcessAll, string EmployeeId, string EmployeeList, string TaxYear, string EmployerType)
        {
            string EmployeeCondition = "";
            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition = " AND Tyt_IDNo = '" + EmployeeId + "' ";
            else if (ProcessAll == true && EmployeeList != "")
                EmployeeCondition = " AND Tyt_IDNo IN (" + EmployeeList + ") ";

            #region Query
            string strQuery = string.Format(@"SELECT Tyt_IDNo
	                                                , SUM ( Tyt_MWEBasic ) Tyt_MWEBasic
	                                                , SUM ( Tyt_MWEHoliday ) Tyt_MWEHoliday
	                                                , SUM ( Tyt_MWEOvertime	 ) Tyt_MWEOvertime
	                                                , SUM ( Tyt_MWENightShift ) Tyt_MWENightShift
	                                                , SUM ( Tyt_MWEHazard ) Tyt_MWEHazard 
	                                                , SUM ( Tyt_Nontaxable13thMonth	 ) Tyt_Nontaxable13thMonth
	                                                , SUM ( Tyt_DeMinimis ) Tyt_DeMinimis
	                                                , SUM ( Tyt_PremiumsUnionDues ) Tyt_PremiumsUnionDues
	                                                , SUM ( Tyt_NontaxableSalariesCompensation ) Tyt_NontaxableSalariesCompensation
	                                                , SUM ( Tyt_TaxableBasic ) Tyt_TaxableBasic
	                                                , SUM ( Tyt_Taxable13thMonth ) Tyt_Taxable13thMonth
	                                                , SUM ( Tyt_TaxableSalariesCompensation ) Tyt_TaxableSalariesCompensation
	                                                , SUM ( Tyt_TaxableOvertime ) Tyt_TaxableOvertime
	                                                , SUM ( Tyt_Hazard ) Tyt_Hazard
	                                                , SUM ( Tyt_PremiumPaidOnHealth ) Tyt_PremiumPaidOnHealth
	                                                , SUM ( Tyt_TaxWithheld ) Tyt_TaxWithheld
	                                                , SUM ( Tyt_Representation ) Tyt_Representation
	                                                , SUM ( Tyt_Transportation ) Tyt_Transportation
	                                                , SUM ( Tyt_CostLivingAllowance ) Tyt_CostLivingAllowance
	                                                , SUM ( Tyt_FixedHousingAllowance ) Tyt_FixedHousingAllowance
	                                                , SUM ( Tyt_OtherTaxable1 ) Tyt_OtherTaxable1
	                                                , SUM ( Tyt_OtherTaxable2 ) Tyt_OtherTaxable2
	                                                , SUM ( Tyt_Commision ) Tyt_Commision
	                                                , SUM ( Tyt_ProfitSharing ) Tyt_ProfitSharing
	                                                , SUM ( Tyt_Fees ) Tyt_Fees
	                                                , SUM ( Tyt_SupplementaryTaxable1 ) Tyt_SupplementaryTaxable1
	                                                , SUM ( Tyt_SupplementaryTaxable2 ) Tyt_SupplementaryTaxable2
                                                FROM T_EmpYTD
                                                WHERE Tyt_TaxYear = '{0}'
	                                                AND Tyt_EmployerType = '{1}'
                                                    {2}
                                                GROUP BY Tyt_IDNo", TaxYear, EmployerType, EmployeeCondition);
            #endregion

            DataTable dtResult = dalCentral.ExecuteDataSet(strQuery).Tables[0];
            return dtResult;
        }

        private DataTable GetW2StagingPayrollData(bool ProcessAll, string EmployeeId, string EmployeeList, string TaxYear, string DataSource)
        {
            string EmployeeCondition = "";
            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition = " AND Tas_IDNo = '" + EmployeeId + "' ";
            else if (ProcessAll == true && EmployeeList != "")
                EmployeeCondition = " AND Tas_IDNo IN (" + EmployeeList + ") ";

            string EmployeeCondition2 = "";
            if (DataSource.Equals(""))
                EmployeeCondition2 = " AND Tas_DataSource IN ('A','C','S')";  // A-Actual Payroll, C-Current Employer, System
            else
                EmployeeCondition2 = string.Format(" AND Tas_DataSource IN ('{0}')", DataSource);

            #region Query
            string strQuery = string.Format(@"
            SELECT [Tas_IDNo]
	            ,SUM([Tas_Basic_PAY]) as Tas_Basic_PAY
                ,SUM([Tas_Basic_INC]) as Tas_Basic_INC
                ,SUM([Tas_Basic_YTD]) as Tas_Basic_YTD
                ,SUM([Tas_Basic_ADJ]) as Tas_Basic_ADJ
                ,SUM([Tas_Representation_INC]) as Tas_Representation_INC
                ,SUM([Tas_Representation_YTD]) as Tas_Representation_YTD
                ,SUM([Tas_Transportation_INC]) as Tas_Transportation_INC
                ,SUM([Tas_Transportation_YTD]) as Tas_Transportation_YTD
                ,SUM([Tas_CostLivingAllowance_INC]) as Tas_CostLivingAllowance_INC
                ,SUM([Tas_CostLivingAllowance_YTD]) as Tas_CostLivingAllowance_YTD
                ,SUM([Tas_FixedHousingAllowance_INC]) as Tas_FixedHousingAllowance_INC
                ,SUM([Tas_FixedHousingAllowance_YTD]) as Tas_FixedHousingAllowance_YTD
                ,SUM([Tas_RegularOtherTaxableIncome1_PAY]) as Tas_RegularOtherTaxableIncome1_PAY
                ,SUM([Tas_RegularOtherTaxableIncome1_INC]) as Tas_RegularOtherTaxableIncome1_INC
                ,SUM([Tas_RegularOtherTaxableIncome1_YTD]) as Tas_RegularOtherTaxableIncome1_YTD
                ,SUM([Tas_RegularOtherTaxableIncome2_PAY]) as Tas_RegularOtherTaxableIncome2_PAY
                ,SUM([Tas_RegularOtherTaxableIncome2_INC]) as Tas_RegularOtherTaxableIncome2_INC
                ,SUM([Tas_RegularOtherTaxableIncome2_YTD]) as Tas_RegularOtherTaxableIncome2_YTD
                ,SUM([Tas_Commision_INC]) as Tas_Commision_INC
                ,SUM([Tas_Commision_YTD]) as Tas_Commision_YTD
                ,SUM([Tas_ProfitSharing_INC]) as Tas_ProfitSharing_INC
                ,SUM([Tas_ProfitSharing_YTD]) as Tas_ProfitSharing_YTD
                ,SUM([Tas_Fees_INC]) as Tas_Fees_INC
                ,SUM([Tas_Fees_YTD]) as Tas_Fees_YTD
                ,SUM([Tas_Hazard_INC]) as Tas_Hazard_INC
                ,SUM([Tas_Hazard_YTD]) as Tas_Hazard_YTD
                ,SUM([Tas_Overtime_PAY]) as Tas_Overtime_PAY
                ,SUM([Tas_Overtime_INC]) as Tas_Overtime_INC
                ,SUM([Tas_Overtime_YTD]) as Tas_Overtime_YTD
                ,SUM([Tas_Overtime_ADJ]) as Tas_Overtime_ADJ
                ,SUM([Tas_SupplementaryOtherTaxableIncome1_PAY]) as Tas_SupplementaryOtherTaxableIncome1_PAY
                ,SUM([Tas_SupplementaryOtherTaxableIncome1_INC]) as Tas_SupplementaryOtherTaxableIncome1_INC
                ,SUM([Tas_SupplementaryOtherTaxableIncome1_YTD]) as Tas_SupplementaryOtherTaxableIncome1_YTD
                ,SUM([Tas_SupplementaryOtherTaxableIncome2_PAY]) as Tas_SupplementaryOtherTaxableIncome2_PAY
                ,SUM([Tas_SupplementaryOtherTaxableIncome2_INC]) as Tas_SupplementaryOtherTaxableIncome2_INC
                ,SUM([Tas_SupplementaryOtherTaxableIncome2_YTD]) as Tas_SupplementaryOtherTaxableIncome2_YTD
                ,SUM([Tas_MWEBasic_PAY]) as Tas_MWEBasic_PAY
                ,SUM([Tas_MWEBasic_INC]) as Tas_MWEBasic_INC
                ,SUM([Tas_MWEBasic_YTD]) as Tas_MWEBasic_YTD
                ,SUM([Tas_MWEBasic_ADJ]) as Tas_MWEBasic_ADJ
                ,SUM([Tas_MWEHoliday_PAY]) as Tas_MWEHoliday_PAY
                ,SUM([Tas_MWEHoliday_INC]) as Tas_MWEHoliday_INC
                ,SUM([Tas_MWEHoliday_YTD]) as Tas_MWEHoliday_YTD
                ,SUM([Tas_MWEHoliday_ADJ]) as Tas_MWEHoliday_ADJ
                ,SUM([Tas_MWEOvertime_PAY]) as Tas_MWEOvertime_PAY
                ,SUM([Tas_MWEOvertime_INC]) as Tas_MWEOvertime_INC
                ,SUM([Tas_MWEOvertime_YTD]) as Tas_MWEOvertime_YTD
                ,SUM([Tas_MWEOvertime_ADJ]) as Tas_MWEOvertime_ADJ
                ,SUM([Tas_MWENightShift_PAY]) as Tas_MWENightShift_PAY
                ,SUM([Tas_MWENightShift_INC]) as Tas_MWENightShift_INC
                ,SUM([Tas_MWENightShift_YTD]) as Tas_MWENightShift_YTD
                ,SUM([Tas_MWENightShift_ADJ]) as Tas_MWENightShift_ADJ
                ,SUM([Tas_MWEHazard_INC]) as Tas_MWEHazard_INC
                ,SUM([Tas_MWEHazard_YTD]) as Tas_MWEHazard_YTD
                ,SUM([Tas_13thMonthBen_PAY]) as Tas_13thMonthBen_PAY
                ,SUM([Tas_13thMonthBen_INC]) as Tas_13thMonthBen_INC
                ,SUM([Tas_13thMonthBen_YTD]) as Tas_13thMonthBen_YTD
                ,SUM([Tas_DeMinimis_INC]) as Tas_DeMinimis_INC
                ,SUM([Tas_DeMinimis_YTD]) as Tas_DeMinimis_YTD
                ,SUM([Tas_Premiums_PAY]) as Tas_Premiums_PAY
                ,SUM([Tas_Premiums_INC]) as Tas_Premiums_INC
                ,SUM([Tas_Premiums_DED]) as Tas_Premiums_DED
                ,SUM([Tas_Premiums_YTD]) as Tas_Premiums_YTD
                ,SUM([Tas_NontaxableSalaries_INC]) as Tas_NontaxableSalaries_INC
                ,SUM([Tas_NontaxableSalaries_YTD]) as Tas_NontaxableSalaries_YTD
                ,SUM([Tas_Tax_PAY]) as Tas_Tax_PAY
                ,SUM([Tas_Tax_INC]) as Tas_Tax_INC
                ,SUM([Tas_Tax_DED]) as Tas_Tax_DED
                ,SUM([Tas_Tax_YTD]) as Tas_Tax_YTD
                ,SUM([Tas_PremiumPaidOnHealth]) as Tas_PremiumPaidOnHealth
            FROM T_EmpAlphalistStaging
            WHERE LEFT(Tas_PayCycle, 4) = '{0}'
            {1}
            {2}
            GROUP BY Tas_IDNo", TaxYear, EmployeeCondition, EmployeeCondition2);
            #endregion

            DataTable dtResult = dalCentral.ExecuteDataSet(strQuery).Tables[0];
            return dtResult;
        }

        private DataTable GetW2StagingWithholdingTaxData(bool ProcessAll, string EmployeeId, string EmployeeList, string TaxYear, string WTAXCUTOFF, bool GetAllRecords)
        {
            string EmployeeCondition = "";
            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition = " AND Tas_IDNo = '" + EmployeeId + "' ";
            else if (ProcessAll == true && EmployeeList != "")
                EmployeeCondition = " AND Tas_IDNo IN (" + EmployeeList + ") ";

            string PayPeriodCondition = "";
            if (GetAllRecords == false)
            {
                if (WTAXCUTOFF != "")
                    PayPeriodCondition = string.Format(" AND Tas_PayCycle <= '{0}{1}' ", TaxYear, WTAXCUTOFF);
                else PayPeriodCondition = "";
            }

            string LastPayWtaxCondition = "";
            if (GetAllRecords == true)
            {
                //LastPayWtaxCondition = @" + (CASE WHEN Tas_Tax_PAY = 0 AND Tas_WorkStatus = 'IN' THEN Tas_YTDWtaxAmt - Tas_YTDWtaxAmtBefore ELSE 0 END)";
            }

            #region Query
            string strQuery = string.Format(@"
                SELECT [Tas_IDNo]
                    , MAX([Tas_SalaryRate]) AS [Tas_SalaryRate]
                    , SUM(Tas_Basic_PAY + Tas_Basic_ADJ + Tas_Basic_INC + Tas_Basic_YTD) AS [Tas_TotalREGAmt]
	                , SUM(Tas_Tax_PAY {3}) AS [Tas_WtaxAmt]
	                , SUM(Tas_Tax_INC) AS [Tas_IncomeWTAX]
	                , SUM(Tas_Tax_DED) AS [Tas_DeductionWTAX]
                FROM T_EmpAlphalistStaging
                WHERE Tas_DataSource = 'A'
                AND LEFT(Tas_PayCycle, 4) = '{0}'
                {1}
                {2}
                GROUP BY Tas_IDNo", TaxYear, EmployeeCondition, PayPeriodCondition, LastPayWtaxCondition);
            #endregion

            DataTable dtResult = dalCentral.ExecuteDataSet(strQuery).Tables[0];
            return dtResult;
        }

        private DataTable GetW2StagingWithholdingTaxData(bool ProcessAll, string PayCycleCode, bool bYearEndAdjustment, string EmployeeId, string EmployeeList, string TaxYear, string WTAXCUTOFF, bool GetAllRecords)
        {
            string EmployeeCondition = "";
            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition = " AND Tas_IDNo = '" + EmployeeId + "' ";
            else if (ProcessAll == true && EmployeeList != "")
                EmployeeCondition = " AND Tas_IDNo IN (" + EmployeeList + ") ";

            string YearEndAdjustmentCondition = "";
            if (GetAllRecords)
            {
                if (bYearEndAdjustment && PayCycleCode.CompareTo(TaxYear + "122") >= 0)
                {
                    YearEndAdjustmentCondition = string.Format(" AND Tas_PayCycle <= '{0}121' ", TaxYear);
                }
                else
                    YearEndAdjustmentCondition = string.Format(" AND Tas_PayCycle <= '{0}122' ", TaxYear);
            }
            else
            {
                if (WTAXCUTOFF != "")
                    YearEndAdjustmentCondition = string.Format(" AND Tas_PayCycle <= '{0}{1}' ", TaxYear, WTAXCUTOFF);
            }

            #region Query
            string strQuery = string.Format(@"
                SELECT [Tas_IDNo]
                    , MAX([Tas_SalaryRate]) AS [Tas_SalaryRate]
                    , SUM(Tas_Basic_PAY + Tas_Basic_ADJ + Tas_Basic_INC + Tas_Basic_YTD) AS [Tas_TotalREGAmt]
	                , SUM(Tas_Tax_PAY) AS [Tas_Tax_PAY]
	                , SUM(Tas_Tax_INC) AS [Tas_Tax_INC]
	                , SUM(Tas_Tax_DED) AS [Tas_Tax_DED]
                    , SUM(Tas_Tax_YTD) AS [Tas_Tax_YTD]
                FROM T_EmpAlphalistStaging
                WHERE Tas_DataSource IN ('A','C','S')
                AND LEFT(Tas_PayCycle, 4) = '{0}'
                {1}
                {2}
                GROUP BY Tas_IDNo", TaxYear, EmployeeCondition, YearEndAdjustmentCondition);
            #endregion

            DataTable dtResult = dalCentral.ExecuteDataSet(strQuery).Tables[0];
            return dtResult;
        }

        private DataTable GetW2StagingWithholdingTaxDataFP(bool ProcessAll, string EmployeeId, string EmployeeList, string PayCycleSep, string LoginDBName, DALHelper daltemp)
        {
            string EmployeeCondition = "";
            if (!ProcessAll && EmployeeId != "")
                EmployeeCondition = " AND Tpy_IDNo = '" + EmployeeId + "' ";
            else if (ProcessAll == true && EmployeeList != "")
                EmployeeCondition = " AND Tpy_IDNo IN (" + EmployeeList + ") ";

            #region Query
            string strQuery = string.Format(@"
                 SELECT Tpy_IDNo [Tas_IDNo]
	                , Tpy_WtaxAmt AS [Tas_WtaxAmt]
	                , Tpy_YTDWtaxAmtBefore AS [Tas_YTDWtaxAmtBefore]
                FROM (SELECT * FROM {2}..Udv_Payroll
				      UNION ALL
				      SELECT * FROM {2}..Udv_PayrollFinalPay ) Payroll
                WHERE Tpy_PayCycle = '{0}'
                {1}
                ", PayCycleSep, EmployeeCondition, LoginDBName);
            #endregion

            DataTable dtResult = daltemp.ExecuteDataSet(strQuery).Tables[0];
            return dtResult;
        }

        private DataTable GetTaxCodeExemptionTable(string CompanyCode, string LoginDBName)
        {
            #region Query PayCycle
            string query = string.Format(@"SELECT TOP 1 Tps_PayCycle 
                                           FROM {0}..T_PaySchedule 
                                           WHERE Tps_CycleIndicator = 'C'"
                                        , LoginDBName);

            DataTable dtPayCycle = dalCentral.ExecuteDataSet(query).Tables[0];
            #endregion

            #region Query
            string strQuery = string.Format(@"  DECLARE @MaxTaxAnnualPayPeriod AS VARCHAR(7)
                                                DECLARE @DependentCount1 AS DECIMAL(8,2)
                                                DECLARE @DependentCount2 AS DECIMAL(8,2)
                                                DECLARE @DependentCount3 AS DECIMAL(8,2)
                                                DECLARE @DependentCount4 AS DECIMAL(8,2)

                                                SELECT @MaxTaxAnnualPayPeriod = Max(Myt_PayCycle)
                                                FROM M_YearlyTaxSchedule
                                                WHERE Myt_PayCycle <= ('{1}')
                                                    AND Myt_RecordStatus = 'A'
                                                    AND Myt_CompanyCode = '{0}'

                                                SELECT @DependentCount1 = Mte_Amount 
                                                FROM M_TaxExemption 
                                                WHERE Mte_TaxCode = 'D1' 
                                                    AND Mte_PayCycle = @MaxTaxAnnualPayPeriod
                                                    AND Mte_ExemptType = 'A' 
                                                    AND Mte_RecordStatus = 'A'
                                                    AND  Mte_CompanyCode = '{0}'

                                                SELECT @DependentCount2 = @DependentCount1 + Mte_Amount 
                                                FROM M_TaxExemption 
                                                WHERE Mte_TaxCode = 'D2' 
                                                    AND Mte_PayCycle = @MaxTaxAnnualPayPeriod
                                                    AND Mte_ExemptType = 'A' 
                                                    AND Mte_RecordStatus = 'A'
                                                    AND Mte_CompanyCode = '{0}'

                                                SELECT @DependentCount3 = @DependentCount2 + Mte_Amount 
                                                FROM M_TaxExemption 
                                                WHERE Mte_TaxCode = 'D3' 
                                                    AND Mte_PayCycle = @MaxTaxAnnualPayPeriod
                                                    AND Mte_ExemptType = 'A' 
                                                    AND Mte_RecordStatus = 'A'
                                                    AND Mte_CompanyCode = '{0}'

                                                SELECT @DependentCount4 = @DependentCount3 + Mte_Amount 
                                                FROM M_TaxExemption 
                                                WHERE Mte_TaxCode = 'D4' 
                                                    AND Mte_PayCycle = @MaxTaxAnnualPayPeriod
                                                    AND Mte_ExemptType = 'A' 
                                                    AND Mte_RecordStatus = 'A'
                                                    AND Mte_CompanyCode = '{0}'

                                                SELECT Mcd_Code AS [TaxCode]
	                                                , (SELECT M_TaxExemption.Mte_Amount + 
		                                                (SELECT CASE WHEN Len(RTRIM(RIGHT(Mcd_Code,1 ))) = 0 THEN 
				                                                0 
		                                                ELSE  
			                                                CASE 
				                                                WHEN Right(Mcd_Code, 1) = '1' THEN convert(decimal, @DependentCount1)
				                                                WHEN Right(Mcd_Code, 1) = '2' THEN convert(decimal, @DependentCount2)
				                                                WHEN Right(Mcd_Code, 1) = '3' THEN convert(decimal, @DependentCount3)
				                                                WHEN Right(Mcd_Code, 1) = '4' THEN convert(decimal, @DependentCount4)
				                                                ELSE 0 END  
			                                                END )
		                                                FROM M_TaxExemption
		                                                WHERE M_TaxExemption.Mte_RecordStatus = 'A'
			                                                AND M_TaxExemption.Mte_PayCycle = @MaxTaxAnnualPayPeriod
			                                                AND M_TaxExemption.Mte_TaxCode = RTRIM(ISNULL(LEFT(Mcd_Code,2),'S'))
                                                            AND M_TaxExemption.Mte_CompanyCode = '{0}'
                                                            ) AS [ExemptionAmount]
                                                            
                                                FROM M_CodeDtl
                                                WHERE Mcd_CodeType = 'TAXCODE'
                                                    AND Mcd_CompanyCode = '{0}'", CompanyCode, dtPayCycle.Rows[0][0].ToString());
            #endregion

            DataTable dtResult = dalCentral.ExecuteDataSet(strQuery).Tables[0];
            return dtResult;
        }

        public DataTable GetAnnualizedTaxAmount(decimal SalaryAmount, string TaxYear, string CompanyCode)
        {
            #region query
            string query = string.Format(@"DECLARE @MaxTaxAnnual AS varchar(20)
                                            SET @MaxTaxAnnual = (
                                                SELECT MAX(Myt_PayCycle) FROM M_YearlyTaxSchedule
                                                WHERE Myt_PayCycle <= '{1}122'
                                                    AND Myt_CompanyCode = '{2}'
                                            )
                                            
                                            DECLARE @TAXANNUALPERIOD AS VARCHAR(10)
                                            SET @TAXANNUALPERIOD = (SELECT Max(Myt_PayCycle) as [PayPeriod]
                                                                   FROM M_YearlyTaxSchedule
                                                                   WHERE Myt_PayCycle <= @MaxTaxAnnual
                                                                        AND Myt_RecordStatus = 'A'
                                                                        AND Myt_CompanyCode = '{2}')

                                            SELECT Myt_TaxOnCompensationLevel + (( {0:0.00} - Myt_ExcessOverCompensationLevel) * (Myt_TaxOnExcess/100))
                                            , Myt_BracketNo
                                            FROM M_YearlyTaxSchedule
                                            WHERE M_YearlyTaxSchedule.Myt_RecordStatus = 'A' 
                                                    AND M_YearlyTaxSchedule.Myt_PayCycle = @TAXANNUALPERIOD
                                                    AND {0:0.00} >= Myt_MinCompensationLevel and {0:0.00} <= Myt_MaxCompensationLevel
                                                    AND Myt_CompanyCode = '{2}'"
                                                , SalaryAmount, TaxYear, CompanyCode);
            #endregion
            DataTable dtResult = dalCentral.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public DataTable GetPremiumContributionTable(string CompanyCode)
        {
            string query = string.Format(
                            @"SELECT Mps_DeductionCode
                                  ,Mps_CompensationFrom
                                  ,Mps_MonthlySalaryCredit
                                  ,Mps_ERShare
                                  ,Mps_EEShare
                                  ,Mps_PayCycle
                                  ,Mps_IsPercentage
                                  ,Mps_MonthlySalaryCreditMPF
                                  ,Mps_MPFERShare
                                  ,Mps_MPFEEShare
                              FROM M_PremiumSchedule
                              WHERE Mps_RecordStatus = 'A'
                                AND Mps_CompanyCode = '{0}'", CompanyCode);

            DataTable dtResult;
            dtResult = dalCentral.ExecuteDataSet(query).Tables[0];
            return dtResult;
        }

        public int GetNextQuincenaCount(string PayPeriod, string TaxYr, string EmployeeID, string LoginDBName)
        {
            #region query
            string query = string.Format(@"DECLARE @EmployeeID VARCHAR(15) = '{1}'
                                            DECLARE @StartPayPeriod VARCHAR(7) = '{0}'
                                            DECLARE @TaxYr VARCHAR(4) = '{2}'
                                            DECLARE @EndPayPeriod VARCHAR(7)
                                            DECLARE @StartCurrentCycle DATETIME

                                            SELECT @StartCurrentCycle = Tps_StartCycle
                                            FROM {3}..T_PaySchedule
                                            WHERE Tps_PayCycle = @StartPayPeriod

                                            SELECT @StartPayPeriod = CASE WHEN Mem_SeparationDate IS NOT NULL AND Mem_SeparationDate < @StartCurrentCycle
						                                            THEN (SELECT TOP 1 Tps_PayCycle
								                                            FROM {3}..T_PaySchedule
								                                            WHERE Mem_SeparationDate BETWEEN Tps_StartCycle AND Tps_EndCycle
									                                            AND Tps_CycleIndicator = 'P'
									                                            AND Tps_RecordStatus = 'A')
						                                            ELSE @StartPayPeriod
						                                            END
                                            FROM {3}..M_Employee
                                            WHERE Mem_IDNo = @EmployeeID

                                            SELECT @EndPayPeriod = CASE WHEN LEFT(@StartPayPeriod, 4) < @TaxYr 
						                                            THEN LEFT(@StartPayPeriod, 4) 
						                                            ELSE @TaxYr 
						                                            END + '122'

                                            SELECT COUNT(Tps_PayCycle)
                                            FROM {3}..T_PaySchedule
                                            WHERE Tps_PayCycle <= @EndPayPeriod
                                            AND Tps_PayCycle > @StartPayPeriod 
                                            AND Tps_RecordStatus = 'A'
                                            AND Tps_CycleIndicator IN ('P','C','F')"
                                                , PayPeriod, EmployeeID, TaxYr, LoginDBName);
            #endregion
            DataSet ds = null;
            int iCnt = 0;
            ds = dalCentral.ExecuteDataSet(query);
            if (ds.Tables[0].Rows.Count > 0)
            {
                iCnt = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            }
            return iCnt;
        }

        public decimal GetMonthlyRecurringAllowance(string EmployeeId, string PayPeriod, string CompanyCode)
        {
            #region query
            string query = string.Format(@"SELECT SUM(CASE WHEN Min_ApplicablePayCycle = 0 
                                                    THEN Tfa_Amount * 2
                                                    ELSE Tfa_Amount END)
                                            FROM T_EmpFixAllowance
                                            INNER JOIN M_Income ON Min_IncomeCode = Tfa_AllowanceCode
                                                AND Min_TaxClass = 'T'
                                                AND Min_IsRecurring = 1
                                                AND Min_CompanyCode = '{2}'
                                            WHERE ISNULL(Tfa_EndPayCycle, '{1}') >= '{1}' 
                                                AND '{1}' >= Tfa_StartPayCycle
                                                AND Tfa_IDNo = '{0}'"
                                            , EmployeeId, PayPeriod, CompanyCode);
            #endregion
            DataSet ds = null;
            decimal dAllowanceAmount = 0;
            ds = dalCentral.ExecuteDataSet(query);
            if (ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows[0][0].ToString() != "")
            {
                dAllowanceAmount = Convert.ToDecimal(ds.Tables[0].Rows[0][0]);
            }
            return dAllowanceAmount;
        }

        public DataTable AlphalistExceptionList(string profileCode, string centralProfile, string TaxYear, string EmployeeID, string userlogin, string companyCode, string ErrorType, string menuCode)
        {
            #region Query
            string condition = string.Empty;
            if (ErrorType != string.Empty)
            {
                condition = string.Format(@"
                                            AND Tpc_Type LIKE '%{0}'", ErrorType);
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
                                    WHERE Tpc_ModuleCode = '{3}' 
                                        AND Tpc_PayCycle = '{0}'
                                        AND ('{1}' = '' OR Tpc_IDNo = '{1}')
                                        @CONDITIONS	
                                    "
                                    , TaxYear, EmployeeID, centralProfile, menuCode);
            #endregion
            query = query.Replace("@CONDITIONS", condition);
            query = query.Replace("@USERCOSTCENTERACCESSCONDITION", (new HRCReportsBL()).UserCostCenterAccessTmpQuery(profileCode, "PAYROLL", userlogin, "Mem_CostcenterCode", "Mem_PayrollGroup", "Mem_EmploymentStatusCode", "Mem_PayrollType", companyCode, centralProfile, false));
            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dtResult = dal.ExecuteDataSet(query).Tables[0];
            }
            return dtResult;
        }

        public void CheckAlphalistPreProcessingErrorExists(bool ProcessAll, string EmployeeId, string TaxYear, string UserLogin, string CompanyCode, string CentralProfile, string LoginDBName, string SCHEDCODE, string menuCode, DALHelper dlhelper)
        {
            string TaxYearStart = "01/01/" + TaxYear;
            string TaxYearEnd = "12/31/" + TaxYear;

            #region Query
            string query = string.Format(@"
                SET ARITHABORT ON

                DECLARE @STARTYEAR DATETIME = '{3}'
                DECLARE @ENDYEAR DATETIME   = '{4}'

                DELETE FROM {0}..T_EmpProcessCheck 
                WHERE Tpc_ModuleCode = '{7}' 
                      AND Tpc_SystemDefined = 1
                      AND Tpc_PayCycle = '{1}'
                      AND ('{2}' = '' OR Tpc_IDNo = '{2}')

                INSERT INTO {0}..T_EmpProcessCheck 

                SELECT '{7}'
                    , Tah_IDNo
	                , 'BW'
	                , '{1}'
	                , 0
	                , 0
	                , 'Active with Separation date - ' +  CONVERT(VARCHAR(10),Mem_SeparationDate,101)
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_WorkStatus = 'A'
	                AND Mem_SeparationDate IS NOT NULL


                ", LoginDBName
                , TaxYear
                , EmployeeId
                , TaxYearStart
                , TaxYearEnd
                , CompanyCode
                , UserLogin
                , menuCode);
            #endregion
            dlhelper.ExecuteNonQuery(query);
        }
        public void CheckAlphalistPostProcessingErrorExists(bool ProcessAll, string EmployeeId, string TaxYear, string UserLogin, string CompanyCode, string CentralProfile, string LoginDBName, string SCHEDCODE, string menuCode, DALHelper dlhelper)
        {
            string TaxYearStart = "01/01/" + TaxYear;
            string TaxYearEnd   = "12/31/" + TaxYear;

            #region Query
            string query = string.Format(@"
                SET ARITHABORT ON

                DECLARE @STARTYEAR DATE = '{3}'
                DECLARE @ENDYEAR DATE   = '{4}'

                INSERT INTO {0}..T_EmpProcessCheck  ---(Tpc_ModuleCode,Tpc_IDNo,Tpc_Type,Tpc_PayCycle,Tpc_NumValue1,Tpc_NumValue2,Tpc_Remarks,Tpc_SystemDefined,Usr_Login,Ludatetime)

                SELECT '{8}'
                    , Mem_IDNo
	                , 'BW'
	                , '{1}'
	                , 0
	                , 0
	                , 'No Registered Address Information'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM {0}..M_Employee
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
                        AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Mem_EmploymentStatusCode = Mpd_SubCode
                WHERE CONVERT(DATE,Mem_IntakeDate) <= @ENDYEAR
                    AND (Mem_SeparationDate IS NULL
                        OR CONVERT(DATE,Mem_SeparationDate) >= @STARTYEAR)
	                AND LEN(LTRIM(RTRIM(Mem_PresMailingAddress))) = 0
	                AND LEN(LTRIM(RTRIM(Mem_ProvMailingAddress))) = 0
                    AND ('{2}' = '' OR Mem_IDNo = '{2}')
                    AND Mem_WorkStatus = 'A'

                UNION ALL

                SELECT '{8}'
                    , Mem_IDNo
	                , 'BW'
	                , '{1}'
	                , 0
	                , 0
	                , 'No Zip Code Information'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM {0}..M_Employee
                INNER JOIN (
                            SELECT Mpd_SubCode 
                            FROM {0}..M_PolicyDtl 
                            WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                                AND Mpd_ParamValue = 1
                                AND Mpd_CompanyCode = '{5}'
                            ) EMPSTAT
                ON Mem_EmploymentStatusCode = Mpd_SubCode
                WHERE CONVERT(DATE,Mem_IntakeDate) <= @ENDYEAR
                    AND (Mem_SeparationDate IS NULL
                        OR CONVERT(DATE,Mem_SeparationDate) >= @STARTYEAR)
	                AND LEN(LTRIM(RTRIM(Mem_PresLocationCode))) = 0
	                AND LEN(LTRIM(RTRIM(Mem_PresLocationCode))) = 0
                    AND ('{2}' = '' OR Mem_IDNo = '{2}')
                    AND Mem_WorkStatus = 'A'

                UNION ALL

                SELECT '{8}'
                    , Tas_IDNo
	                , 'BW'
	                , '{1}'
	                , 0
	                , 0
	                , 'With Movement from MWE to Taxable (vice versa)'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM (
	                    SELECT Tas_IDNo, Tas_IsTaxExempted
		                    , ROW_NUMBER() OVER(PARTITION BY Tas_IDNo ORDER BY Tas_IDNo, Tas_IsTaxExempted) AS [RowCnt]
	                    FROM T_EmpAlphalistStaging
	                    WHERE LEFT(Tas_PayCycle, 4) = '{1}'
	                    GROUP BY Tas_IDNo, Tas_IsTaxExempted
                    ) W2STAGING
                INNER JOIN {0}..M_Employee
                ON Tas_IDNo = Mem_IDNo
	                AND [RowCnt] = 2
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Mem_EmploymentStatusCode = Mpd_SubCode
                WHERE CONVERT(DATE,Mem_IntakeDate) <= @ENDYEAR
                    AND (Mem_SeparationDate IS NULL
                        OR CONVERT(DATE,Mem_SeparationDate) >= @STARTYEAR)
	                AND [RowCnt] = 2
                    AND ('{2}' = '' OR Mem_IDNo = '{2}')
                    AND Mem_WorkStatus = 'A'

                UNION ALL

                SELECT '{8}'
                    , Tas_IDNo
	                , 'BW'
	                , '{1}'
	                , 0
	                , 0
	                , 'With Movement in Different Profiles'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM (
	                    SELECT Tas_IDNo, Tas_ProfileCode
		                    , ROW_NUMBER() OVER(PARTITION BY Tas_IDNo ORDER BY Tas_IDNo, Tas_ProfileCode) AS [RowCnt]
	                    FROM T_EmpAlphalistStaging
	                    WHERE LEFT(Tas_PayCycle, 4) = '{1}'
                            AND Tas_DataSource = 'A'
	                    GROUP BY Tas_IDNo, Tas_ProfileCode
                    ) W2STAGING
                INNER JOIN {0}..M_Employee
                ON Tas_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Mem_EmploymentStatusCode = Mpd_SubCode
                WHERE CONVERT(DATE,Mem_IntakeDate) <= @ENDYEAR
                    AND (Mem_SeparationDate IS NULL
                        OR CONVERT(DATE,Mem_SeparationDate) >= @STARTYEAR)
	                AND [RowCnt] > 1
                    AND ('{2}' = '' OR Mem_IDNo = '{2}')
                    AND Mem_WorkStatus = 'A'

               UNION ALL

               SELECT '{8}'
                        , Mem_IDNo 
	                    , 'BW'
	                    , '{1}'
	                    , 0
	                    , 0
	                    , LTRIM(RTRIM(Mem_TIN)) + '- Incorrect TIN length'
                        , 1
	                    , '{6}'
	                    , GETDATE()
                    FROM {0}..M_Employee
                    INNER JOIN (
                        SELECT Mpd_SubCode 
                        FROM {0}..M_PolicyDtl 
                        WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                            AND Mpd_ParamValue = 1
		                    AND Mpd_CompanyCode = '{5}'
                    ) EMPSTAT
                    ON Mem_EmploymentStatusCode = Mpd_SubCode
                    WHERE CONVERT(DATE,Mem_IntakeDate) <= @ENDYEAR
                        AND (Mem_SeparationDate IS NULL
                            OR CONVERT(DATE,Mem_SeparationDate) >= @STARTYEAR)
	                    AND Mem_TIN != 'APPLIED'
	                    AND LEN(REPLACE(Mem_TIN, '-', '') ) NOT IN (9, 12)
                        AND ('{2}' = '' OR Mem_IDNo = '{2}')
                        AND Mem_WorkStatus = 'A'

                    UNION ALL

                    SELECT '{8}'
                        , Tah_IDNo 
	                    , 'AE'
	                    , '{1}'
	                    , Tah_RepresentationCurER
						                    +Tah_TransportationCurER
						                    +Tah_CostLivingAllowanceCurER
						                    +Tah_FixedHousingAllowanceCurER
						                    +Tah_OtherTaxable1CurER
						                    +Tah_OtherTaxable2CurER
						                    +Tah_CommisionCurER
						                    +Tah_ProfitSharingCurER
						                    +Tah_FeesCurER
						                    +Tah_HazardCurER
						                    +Tah_SupplementaryTaxable1CurER
						                    +Tah_SupplementaryTaxable2CurER
	                    , 0
	                    , 'Salaries and Other Forms of Compensation Details Not Equal to TSALOTH Total (' 
		                    + CONVERT(VARCHAR, Tah_TaxableSalariesCompensationCurER) + ')'
                        , 1
	                    , '{6}'
	                    , GETDATE()
                    FROM T_EmpAlphalistHdr
                    INNER JOIN {0}..M_Employee
                    ON Tah_IDNo = Mem_IDNo
                    INNER JOIN (
                        SELECT Mpd_SubCode 
                        FROM {0}..M_PolicyDtl 
                        WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                            AND Mpd_ParamValue = 1
		                    AND Mpd_CompanyCode = '{5}'
                    ) EMPSTAT
                    ON Tah_EmploymentStatus = Mpd_SubCode
                    WHERE Tah_TaxYear = '{1}'
	                    AND (Tah_RepresentationCurER
			                    +Tah_TransportationCurER
			                    +Tah_CostLivingAllowanceCurER
			                    +Tah_FixedHousingAllowanceCurER
			                    +Tah_OtherTaxable1CurER
			                    +Tah_OtherTaxable2CurER
			                    +Tah_CommisionCurER
			                    +Tah_ProfitSharingCurER
			                    +Tah_FeesCurER
			                    +Tah_HazardCurER
			                    +Tah_SupplementaryTaxable1CurER
			                    +Tah_SupplementaryTaxable2CurER) 
		                    != Tah_TaxableSalariesCompensationCurER
                        AND ('{2}' = '' OR Tah_IDNo = '{2}')

                    UNION ALL

                    SELECT DISTINCT '{8}'
                        , Tin_IDNo 
	                    , 'BW'
	                    , '{1}'
	                    , Tin_IncomeAmt
	                    , 0
	                    , Tin_PayCycle + ' - ' + Min_IncomeCode + ' Income Code has no Alphalist Category'
                        , 1
	                    , '{6}'
	                    , GETDATE()
                    FROM {0}..Udv_Income
                    INNER JOIN {0}..M_Employee
                        ON Tin_IDNo = Mem_IDNo
                        AND Mem_WorkStatus = 'A'
                    INNER JOIN M_Income
                        ON Tin_IncomeCode = Min_IncomeCode
                        AND Min_CompanyCode = '{5}'
                    INNER JOIN (
                        SELECT Mpd_SubCode 
                        FROM {0}..M_PolicyDtl 
                        WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                            AND Mpd_ParamValue = 1
		                    AND Mpd_CompanyCode = '{5}'
                    ) EMPSTAT
                    ON Mem_EmploymentStatusCode = Mpd_SubCode
                    LEFT JOIN M_CodeDtl ON Mcd_CodeType = 'IALPHACATGY'
						AND Mcd_CompanyCode = '{5}'
						AND Mcd_Code = Min_AlphalistCategory
                    WHERE ISNULL(Mcd_Code, '') = ''
	                    AND LEFT(Tin_PayCycle, 4) = '{1}'
                        AND ('{2}' = '' OR Tin_IDNo = '{2}')
                        

                    UNION ALL

                    SELECT DISTINCT '{8}'
                        , Tdd_IDNo 
                    	, 'BW'
                    	, '{1}'
                        ,  Tdd_Amount
                        , 0
                        , Tdd_ThisPayCycle + ' - ' + Mdn_DeductionCode +' Deduction Code has no Alphalist Category'
                        , 1
                        , '{6}'
                        , GETDATE()
                    FROM {0}..Udv_Deduction
                    INNER JOIN {0}..M_Employee
                        ON Tdd_IDNo = Mem_IDNo
                        AND Mem_WorkStatus = 'A'
                    INNER JOIN M_Deduction
                        ON Tdd_DeductionCode = Mdn_DeductionCode
                        AND Mdn_CompanyCode = '{5}'
                    INNER JOIN (
                        SELECT Mpd_SubCode 
                        FROM {0}..M_PolicyDtl 
                        WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                            AND Mpd_ParamValue = 1
                            AND Mpd_CompanyCode = '{5}'
                        ) EMPSTAT
                       ON Mem_EmploymentStatusCode = Mpd_SubCode
                     LEFT JOIN M_CodeDtl ON Mcd_CodeType = 'DALPHACATGY'
						AND Mcd_CompanyCode = '{5}'
						AND Mcd_Code = Mdn_AlphalistCategory
                     WHERE ISNULL(Mcd_Code, '') = ''
                            AND LEFT(Tdd_ThisPayCycle, 4) = '{1}'
                            AND ('{2}' = '' OR Tdd_IDNo = '{2}')

                    UNION ALL

                    SELECT '{8}'
                        , Tah_IDNo
                        , 'AE'
                        , '{1}'
                        , 0
                        , 0
                        , Tah_Schedule + ' - Invalid Schedule in Alphalist Entry Record'
                        , 1
                        , '{6}'
                        , GETDATE()
                        FROM T_EmpAlphalistHdr
                        INNER JOIN {0}..M_Employee
                        ON Tah_IDNo = Mem_IDNo
                        LEFT JOIN M_CodeDtl
                        ON Tah_Schedule = Mcd_Code
	                        AND Mcd_CodeType = '{7}'
	                        AND Mcd_CompanyCode = '{5}'
                        INNER JOIN (
                            SELECT Mpd_SubCode 
                            FROM {0}..M_PolicyDtl 
                            WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                                AND Mpd_ParamValue = 1
		                        AND Mpd_CompanyCode = '{5}'
                        ) EMPSTAT
                        ON Tah_EmploymentStatus = Mpd_SubCode
                        WHERE Tah_TaxYear = '{1}'
	                        AND Mcd_Code IS NULL
                            AND ('{2}' = '' OR Tah_IDNo = '{2}')

                    UNION ALL

                    SELECT '{8}'
	                    , Tah_IDNo
	                    , 'AW'
	                    , '{1}'
	                    , Tah_MWEBasicCurER
	                    , 0
	                    , 'MWE Basic Current ER is negative' 
                        , 1
	                    , '{6}'
	                    , GETDATE()
                    FROM T_EmpAlphalistHdr
                    INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                    INNER JOIN (
                        SELECT Mpd_SubCode 
                        FROM {0}..M_PolicyDtl 
                        WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                            AND Mpd_ParamValue = 1
		                    AND Mpd_CompanyCode = '{5}'
                    ) EMPSTAT ON Tah_EmploymentStatus = Mpd_SubCode
                    WHERE Tah_TaxYear = '{1}'
                         AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                    AND Tah_MWEBasicCurER < 0           

                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo 
	                , 'AW'
	                , '{1}'
	                , Tah_MWEHolidayCurER 
	                , 0
	                , 'MWE Holiday Current ER is negative' 
	                , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_MWEHolidayCurER < 0

                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_MWEOvertimeCurER
	                , 0
	                , 'MWE Overtime Current ER is negative' 
	                , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_MWEOvertimeCurER < 0

                    UNION ALL

                    SELECT '{8}'
	                    , Tah_IDNo
	                    , 'AW'
	                    , '{1}'
	                    , Tah_MWENightShiftCurER
	                    , 0
	                    , 'MWE Night Shift Current ER is negative'
                        , 1
	                    , '{6}'
	                    , GETDATE()
                    FROM T_EmpAlphalistHdr
                    INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                    INNER JOIN (
                        SELECT Mpd_SubCode 
                        FROM {0}..M_PolicyDtl 
                        WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                            AND Mpd_ParamValue = 1
		                    AND Mpd_CompanyCode = '{5}'
                    ) EMPSTAT
                    ON Tah_EmploymentStatus = Mpd_SubCode
                    WHERE Tah_TaxYear = '{1}'
                        AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                    AND Tah_MWENightShiftCurER < 0

                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_MWEHazardCurER
	                , 0
	                , 'MWE Hazard Current ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_MWEHazardCurER < 0

                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_Nontaxable13thMonthCurER
	                , 0
	                , 'Nontaxable 13th Month Current ER is negative' 
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_Nontaxable13thMonthCurER < 0

                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_DeMinimisCurER
	                , 0
	                , 'De Minimis Current ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_DeMinimisCurER < 0

                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_PremiumsUnionDuesCurER 
	                , 0
	                , 'Premiums & Union Dues Current ER is negative' 
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_PremiumsUnionDuesCurER < 0

                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_NontaxableSalariesCompensationCurER
	                , 0
	                , 'Nontaxable Salaries & Compensation Current ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_NontaxableSalariesCompensationCurER < 0

                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_TaxableBasicCurER
	                , 0
	                , 'Taxable Basic Current ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_TaxableBasicCurER < 0

                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo 
	                , 'AW'
	                , '{1}'
	                , Tah_TaxableBasicNetPremiumsCurER
	                , 0
	                , 'Taxable Basic Net Premiums Current ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_TaxableBasicNetPremiumsCurER < 0

                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo 
	                , 'AW'
	                , '{1}'
	                , Tah_TaxableOvertimeCurER
	                , 0
	                , 'Taxable Overtime Current ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_TaxableOvertimeCurER < 0

                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_Taxable13thMonthCurER
	                , 0
	                , 'Taxable 13th Month Current ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_Taxable13thMonthCurER < 0

                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_TaxableSalariesCompensationCurER
	                , 0
	                , 'Taxable Salaries & Compensation Current ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_TaxableSalariesCompensationCurER < 0

                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_MWEBasicPrvER
	                , 0
	                , 'MWE Basic Previous ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_MWEBasicPrvER < 0

                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_MWEHolidayPrvER
	                , 0
	                , 'MWE Holiday Previous ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_MWEHolidayPrvER < 0

                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_MWEOvertimePrvER
	                , 0
	                , 'MWE Overtime Previous ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_MWEOvertimePrvER < 0

                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_MWENightShiftPrvER
	                , 0
	                , 'MWE Night Shift Previous ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_MWENightShiftPrvER < 0

                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_MWEHazardPrvER
	                , 0
	                , 'MWE Hazard Previous ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_MWEHazardPrvER < 0

                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_Nontaxable13thMonthPrvER
	                , 0
	                , 'Nontaxable 13th Month Previous ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_Nontaxable13thMonthPrvER < 0

                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_DeMinimisPrvER
	                , 0
	                , 'De Minimis Previous ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_DeMinimisPrvER < 0

                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_PremiumsUnionDuesPrvER
	                , 0
	                , 'Premiums & Union Dues Previous ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_PremiumsUnionDuesPrvER < 0

                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_NontaxableSalariesCompensationPrvER
	                , 0
	                , 'Nontaxable Salaries & Compensation Previous ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_NontaxableSalariesCompensationPrvER < 0

                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_TaxableBasicPrvER
	                , 0
	                , 'Taxable Basic Previous ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_TaxableBasicPrvER < 0

                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_TaxableBasicNetPremiumsPrvER
	                , 0
	                , 'Taxable Basic Net Premiums Previous ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_TaxableBasicNetPremiumsPrvER < 0

                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_TaxableOvertimePrvER
	                , 0
	                , 'Taxable Overtime Previous ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_TaxableOvertimePrvER < 0

                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_Taxable13thMonthPrvER
	                , 0
	                , 'Taxable 13th Month Previous ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_Taxable13thMonthPrvER < 0

                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_TaxableSalariesCompensationPrvER
	                , 0
	                , 'Taxable Salaries & Compensation Previous ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_TaxableSalariesCompensationPrvER < 0

                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_RepresentationCurER
	                , 0
	                , 'Representation Current ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_RepresentationCurER < 0

                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_TransportationCurER
	                , 0
	                , 'Transportation Current ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_TransportationCurER < 0

                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_CostLivingAllowanceCurER
	                , 0
	                , 'Cost of Living Allowance Current ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_CostLivingAllowanceCurER < 0

                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_FixedHousingAllowanceCurER
	                , 0
	                , 'Fixed Housing Allowance Current ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_FixedHousingAllowanceCurER < 0

                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_OtherTaxable1CurER
	                , 0
	                , 'Other Taxable1 Current ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_OtherTaxable1CurER < 0

                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_OtherTaxable2CurER
	                , 0
	                , 'Other Taxable2 Current ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_OtherTaxable2CurER < 0

                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_CommisionCurER
	                , 0
	                , 'Commision Current ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_CommisionCurER < 0


                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_ProfitSharingCurER
	                , 0
	                , 'Profit Sharing Current ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_ProfitSharingCurER < 0


                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_FeesCurER
	                , 0
	                , 'Fees Current ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_FeesCurER < 0


                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_HazardCurER
	                , 0
	                , 'Hazard Current ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_HazardCurER < 0


                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_SupplementaryTaxable1CurER
	                , 0
	                , 'Supplementary Taxable1 Current ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_SupplementaryTaxable1CurER < 0


                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_SupplementaryTaxable2CurER
	                , 0
	                , 'Supplementary Taxable2 Current ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_SupplementaryTaxable2CurER < 0


                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_RepresentationPrvER
	                , 0
	                , 'Representation Previous ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_RepresentationPrvER < 0


                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_TransportationPrvER
	                , 0
	                , 'Transportation Previous ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_TransportationPrvER < 0


                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_CostLivingAllowancePrvER
	                , 0
	                , 'Cost of Living Allowance Previous ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_CostLivingAllowancePrvER < 0


                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_FixedHousingAllowancePrvER
	                , 0
	                , 'Fixed Housing Allowance Previous ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_FixedHousingAllowancePrvER < 0


                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_OtherTaxable1PrvER
	                , 0
	                , 'Other Taxable1 Previous ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_OtherTaxable1PrvER < 0


                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_OtherTaxable2PrvER
	                , 0
	                , 'Other Taxable2 Previous ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_OtherTaxable2PrvER < 0


                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_OtherTaxable2PrvER
	                , 0
	                , 'Other Taxable2 Previous ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_OtherTaxable2PrvER < 0


                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_CommisionPrvER
	                , 0
	                , 'Commision Previous ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_CommisionPrvER < 0

                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_ProfitSharingPrvER
	                , 0
	                , 'Profit Sharing Previous ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_ProfitSharingPrvER < 0

                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_FeesPrvER
	                , 0
	                , 'Fees Previous ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_FeesPrvER < 0

                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_HazardPrvER
	                , 0
	                , 'Hazard Previous ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_HazardPrvER < 0

                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_SupplementaryTaxable1PrvER
	                , 0
	                , 'Supplementary Taxable1 Previous ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_SupplementaryTaxable1PrvER < 0

                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_SupplementaryTaxable2PrvER
	                , 0
	                , 'Supplementary Taxable2 Previous ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_SupplementaryTaxable2PrvER < 0

                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_GrossCompensationIncome
	                , 0
	                , 'Gross Compensation Income is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_GrossCompensationIncome < 0

                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_NontaxableIncomeCurER
	                , 0
	                , 'Nontaxable Income Current ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_NontaxableIncomeCurER < 0

                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_NontaxableIncomePrvER
	                , 0
	                , 'Nontaxable Income Previous ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_NontaxableIncomePrvER < 0

                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_TaxableIncomeCurER
	                , 0
	                , 'Taxable Income Current ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_TaxableIncomeCurER < 0

                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_TaxableIncomePrvER
	                , 0
	                , 'Taxable Income Previous ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_TaxableIncomePrvER < 0

                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_TaxableIncomePrvER
	                , 0
	                , 'Taxable Income Previous ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_TaxableIncomePrvER < 0

                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_NetTaxableIncome
	                , 0
	                , 'Net Taxable Income is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_NetTaxableIncome < 0

                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_TaxDue
	                , 0
	                , 'Tax Due is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_TaxDue < 0

                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_TaxWithheldPrvER
	                , 0
	                , 'Tax Withheld Previous ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_TaxWithheldPrvER < 0

                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_TaxWithheldCurER
	                , 0
	                , 'Tax Withheld Current ER is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_TaxWithheldCurER < 0

                UNION ALL

                SELECT '{8}'
	                , Tah_IDNo
	                , 'AW'
	                , '{1}'
	                , Tah_TotalTaxWithheld
	                , 0
	                , 'Total Tax Withheld is negative'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                WHERE Tah_TaxYear = '{1}'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')
	                AND Tah_TotalTaxWithheld < 0

                UNION ALL

                SELECT '{8}'
                    , Tah_IDNo 
	                , 'AE'
	                , '{1}'
	                , 0
	                , 0
	                , ISNULL(Tah_CurrentEmploymentStatus, '') + ' Invalid BIR Employment Status Code'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                LEFT JOIN M_CodeDtl on Mcd_Code = Tah_CurrentEmploymentStatus
	                AND Mcd_CodeType = 'BIR_EMPSTAT'
                    AND Mcd_CompanyCode = '{5}'
                WHERE Tah_TaxYear = '{1}'
                    AND (LEN(RTRIM(Tah_CurrentEmploymentStatus)) = 0 OR Mcd_Code is null)
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')

                UNION ALL

                SELECT '{8}'
                    , Tah_IDNo 
	                , 'AE'
	                , '{1}'
	                , 0
	                , 0
	                , ISNULL(Tah_SeparationReason,'') + ' Invalid BIR Separation Code'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                LEFT JOIN M_CodeDtl on Mcd_Code = Tah_SeparationReason
	                AND Mcd_CodeType = 'BIR_SEPCODE'
                    AND Mcd_CompanyCode = '{5}'
                WHERE Tah_TaxYear = '{1}'
                    AND (LEN(RTRIM(Tah_SeparationReason)) = 0 OR Mcd_Code IS NULL)
                    AND Tah_WorkStatus = 'IN'
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')

                UNION ALL

                SELECT '{8}'
                    , Tah_IDNo 
	                , 'AE'
	                , '{1}'
	                , 0
	                , 0
	                , ISNULL(Tah_Nationality,'') + ' Invalid Nationality'
                    , 1
	                , '{6}'
	                , GETDATE()
                FROM T_EmpAlphalistHdr
                INNER JOIN {0}..M_Employee ON Tah_IDNo = Mem_IDNo
                INNER JOIN (
                    SELECT Mpd_SubCode 
                    FROM {0}..M_PolicyDtl 
                    WHERE Mpd_PolicyCode = 'EMPSTATPAY'
                        AND Mpd_ParamValue = 1
		                AND Mpd_CompanyCode = '{5}'
                ) EMPSTAT
                ON Tah_EmploymentStatus = Mpd_SubCode
                LEFT JOIN M_CodeDtl on Mcd_Code = Tah_Nationality
	                AND Mcd_CodeType = 'CITIZEN'
                    AND Mcd_CompanyCode = '{5}'
                WHERE Tah_TaxYear = '{1}'
                    AND (LEN(RTRIM(Tah_Nationality)) = 0 OR Mcd_Code IS NULL)
                    AND ('{2}' = '' OR Tah_IDNo = '{2}')

                ", LoginDBName
                , TaxYear
                , EmployeeId
                , TaxYearStart
                , TaxYearEnd
                , CompanyCode
                , UserLogin
                , SCHEDCODE
                , menuCode);
            #endregion
            dlhelper.ExecuteNonQuery(query);
        }
        #endregion

        #region Error List functions
        struct structErrorReport
        {
            public string strEmployeeId;
            public string strLastName;
            public string strFirstName;
            public string strRemarks;

            public structErrorReport(string EmployeeId, string LastName, string FirstName, string Remarks)
            {
                strEmployeeId = EmployeeId;
                strLastName = LastName;
                strFirstName = FirstName;
                strRemarks = Remarks;
            }
        }
        List<structErrorReport> listErrorRept = new List<structErrorReport>();

        public void AddToErrorList(string strEmployeeId, string strLastName, string strFirstName, string strRemarks)
        {
            listErrorRept.Add(new structErrorReport(strEmployeeId, strLastName, strFirstName, strRemarks));
        }

        public void InitializeErrorList()
        {
            listErrorRept.Clear();
        }

        private DataTable SaveErrorReportList()
        {
            DataTable dtErrList = new DataTable();
            if (dtErrList.Columns.Count == 0)
            {
                dtErrList.Columns.Add("IDNumber");
                dtErrList.Columns.Add("Last Name");
                dtErrList.Columns.Add("First Name");
                dtErrList.Columns.Add("Remarks");
                dtErrList.Columns.Add("Value");
            }
            for (int i = 0; i < listErrorRept.Count; i++)
            {
                dtErrList.Rows.Add();
                dtErrList.Rows[dtErrList.Rows.Count - 1]["IDNumber"] = listErrorRept[i].strEmployeeId;
                dtErrList.Rows[dtErrList.Rows.Count - 1]["Last Name"] = listErrorRept[i].strLastName;
                dtErrList.Rows[dtErrList.Rows.Count - 1]["First Name"] = listErrorRept[i].strFirstName;
                dtErrList.Rows[dtErrList.Rows.Count - 1]["Remarks"] = listErrorRept[i].strRemarks;
                dtErrList.Rows[dtErrList.Rows.Count - 1]["Value"] = "";
            }

            dtErrList.DefaultView.Sort = "Remarks, [Last Name], [First Name]";
            dtErrList = dtErrList.DefaultView.ToTable();

            return dtErrList;
        }

        public void InsertToEmpPayrollCheckTable(DataTable dt, string DBName)
        {
            string qString = "";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                #region Insert to Process Check Table
                qString = string.Format(@"INSERT INTO "+ DBName + @"..T_EmpProcessCheck 
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
                                                               , TaxYear
                                                               , 0
                                                               , 0
                                                               , dt.Rows[i]["Remarks"].ToString().Replace("'", "")
                                                               , LoginUser);
                dalCentral.ExecuteNonQuery(qString);
                #endregion
            }
        }
        #endregion

        #region Get Query For Report and Textfile
        public static string GetW2Query(string schedule, bool isTextfileGeneration, string CentralProfile)
        {
            string query = string.Empty;
            if (schedule == "SCHED7.1")
            {
                #region Sched 7.1
                query += @"
                    SELECT 
                    'D7.1'                                                                      [ScheduleNumber]
                    ,'1604CF'                                                                   [FormType]
                    ,'{3}'                                                                      [TIN]
                    ,'{1}'                                                                      [BranchCode]
                    ,'12/31/{2}'                                                                [ReturnPeriod]
                    @EMPLOYEETIN
                    ,'{1}'                                                                      [EmployeeBranchCode]
                    @COMPLETENAME
                    ,CONVERT(VARCHAR(20), Tah_StartDate, 101)                                   [StartPeriod]
                    ,CONVERT(VARCHAR(20), Tah_EndDate, 101)                                     [EndPeriod]
                                                                    
		            ,Tah_GrossCompensationIncome									            [NUMVAL GrossCompensationIncome]
                    ,Tah_Nontaxable13thMonthCurER + Tah_Nontaxable13thMonthPrvER                [NUMVAL Tah_Nontaxable13thMonthCurER]
		            ,Tah_DeMinimisCurER + Tah_DeMinimisPrvER                                    [NUMVAL Tah_DeMinimisCurER]
		            ,Tah_PremiumsUnionDuesCurER + Tah_PremiumsUnionDuesPrvER                    [NUMVAL Tah_PremiumsUnionDuesCurER]
		            ,Tah_NontaxableSalariesCompensationCurER   + Tah_NontaxableSalariesCompensationPrvER   
                            + Tah_MWEBasicCurER         + Tah_MWEBasicPrvER    
                            + Tah_MWEHolidayCurER       + Tah_MWEHolidayPrvER 
                            + Tah_MWENightShiftCurER    + Tah_MWENightShiftPrvER 
                            + Tah_MWEOvertimeCurER      + Tah_MWEOvertimePrvER  
                            + Tah_MWEHazardCurER        + Tah_MWEHazardPrvER			        [NUMVAL Tah_NontaxableSalariesCompensationCurER]
		 
		            ,Tah_Nontaxable13thMonthCurER + Tah_Nontaxable13thMonthPrvER
                            + Tah_DeMinimisCurER + Tah_DeMinimisPrvER
                            + Tah_PremiumsUnionDuesCurER + Tah_PremiumsUnionDuesPrvER 
                            + Tah_NontaxableSalariesCompensationCurER   + Tah_NontaxableSalariesCompensationPrvER   
                            + Tah_MWEBasicCurER         + Tah_MWEBasicPrvER    
                            + Tah_MWEHolidayCurER       + Tah_MWEHolidayPrvER 
                            + Tah_MWENightShiftCurER    + Tah_MWENightShiftPrvER 
                            + Tah_MWEOvertimeCurER      + Tah_MWEOvertimePrvER  
                            + Tah_MWEHazardCurER        + Tah_MWEHazardPrvER                    [NUMVAL TotalNontax]
		
		            ,Tah_TaxableBasicNetPremiumsCurER + Tah_TaxableBasicNetPremiumsPrvER	    [NUMVAL Tah_TaxableBasicCurER]
		            ,Tah_Taxable13thMonthCurER + Tah_Taxable13thMonthPrvER                      [NUMVAL Tah_Taxable13thMonthCurER]
		            ,Tah_TaxableSalariesCompensationCurER  + Tah_TaxableSalariesCompensationPrvER   
                              + Tah_TaxableOvertimeCurER  + Tah_TaxableOvertimePrvER	        [NUMVAL Tah_TaxableSalariesCompensationCurER]
        
                    ,Tah_TaxableBasicNetPremiumsCurER + Tah_TaxableBasicNetPremiumsPrvER
			                + Tah_Taxable13thMonthCurER + Tah_Taxable13thMonthPrvER
                            + Tah_TaxableSalariesCompensationCurER  + Tah_TaxableSalariesCompensationPrvER
                            + Tah_TaxableOvertimeCurER  + Tah_TaxableOvertimePrvER              [NUMVAL TotalTax]
		
                    ,CASE WHEN Tah_TaxYear < '2018' THEN REPLACE(REPLACE(Tah_ExemptionCode, 'E', ''), ' ', '') ELSE '' END [Tah_ExemptionCode]
                    ,Tah_ExemptionAmount                                                        [NUMVAL Tah_ExemptionAmount]
                    ,Tah_PremiumPaidOnHealth                                                    [NUMVAL Tah_PremiumPaidOnHealth]
                    ,Tah_NetTaxableIncome                                                       [NUMVAL NetTaxableCompensation]
                    ,Tah_TaxDue                                                                 [NUMVAL Tah_TaxDue]
                    ,Tah_TaxWithheldCurER + Tah_TaxWithheldPrvER					            [NUMVAL Tah_TaxWithheldCurER]
                    ,CASE WHEN Tah_TaxDue 
				                - (Tah_TaxWithheldCurER + Tah_TaxWithheldPrvER)	 > 0
				                THEN Tah_TaxDue 
					                - (Tah_TaxWithheldCurER + Tah_TaxWithheldPrvER)	
				                ELSE 0 
			                END															        [NUMVAL TaxWithheldDec]
		           ,CASE WHEN Tah_TaxDue 
				                - (Tah_TaxWithheldCurER + Tah_TaxWithheldPrvER) > 0
				                THEN 0
				                ELSE (Tah_TaxDue 
				                - (Tah_TaxWithheldCurER + Tah_TaxWithheldPrvER)) * -1
			                END															        [NUMVAL OverWithheld]
			
			
                  ,Tah_TaxDue													                [NUMVAL ActualWithheld]
                  ,CASE WHEN Tah_IsSubstitutedFiling = 1
			                @SUBSTITUTEFORTEXTFILE
			                END															        [Tah_IsSubstitutedFiling]
                  FROM Udv_AlphalistHdr HEADER
                  INNER JOIN @PROFILES..M_Employee ON Mem_IDNo = Tah_IDNo
                      AND Mem_WorkStatus <> 'IM'
                  @USERCOSTCENTERACCESSCONDITION
                  @CONDITIONS
                  ";
                #endregion
            }
            else if (schedule == "SCHED7.2")
            {
                #region Sched 7.2
                query += @"
                SELECT 
                 'D7.2'                                                                             [ScheduleNumber]
                 ,'1604CF'                                                                          [FormType]
                 ,'{3}'                                                                             [TIN]
                 ,'{1}'                                                                             [BranchCode]
                 ,'12/31/{2}'                                                                       [ReturnPeriod]
                 ,CAST(ROW_NUMBER() OVER (ORDER BY Tah_LastName, Tah_FirstName, Tah_MiddleName) AS VARCHAR(10)) [Seq Num]
                 @EMPLOYEETIN
                 ,'{1}'                                                                             [EmployeeBranchCode]
                 @COMPLETENAME
                ,Tah_GrossCompensationIncome									                    [NUMVAL GrossCompensationIncome]
                ,Tah_Nontaxable13thMonthCurER + Tah_Nontaxable13thMonthPrvER                        [NUMVAL Tah_Nontaxable13thMonthCurER]
                ,Tah_DeMinimisCurER + Tah_DeMinimisPrvER                                            [NUMVAL Tah_DeMinimisCurER]
                ,Tah_PremiumsUnionDuesCurER + Tah_PremiumsUnionDuesPrvER                            [NUMVAL Tah_PremiumsUnionDuesCurER]
                ,Tah_NontaxableSalariesCompensationCurER   + Tah_NontaxableSalariesCompensationPrvER   
                        + Tah_MWEBasicCurER         + Tah_MWEBasicPrvER    
                        + Tah_MWEHolidayCurER       + Tah_MWEHolidayPrvER 
                        + Tah_MWENightShiftCurER    + Tah_MWENightShiftPrvER 
                        + Tah_MWEOvertimeCurER      + Tah_MWEOvertimePrvER  
                        + Tah_MWEHazardCurER        + Tah_MWEHazardPrvER			                [NUMVAL Tah_NontaxableSalariesCompensationCurER]
		        ,Tah_Nontaxable13thMonthCurER + Tah_Nontaxable13thMonthPrvER
                        + Tah_DeMinimisCurER + Tah_DeMinimisPrvER
                        + Tah_PremiumsUnionDuesCurER + Tah_PremiumsUnionDuesPrvER 
                        + Tah_NontaxableSalariesCompensationCurER   + Tah_NontaxableSalariesCompensationPrvER   
                        + Tah_MWEBasicCurER         + Tah_MWEBasicPrvER    
                        + Tah_MWEHolidayCurER       + Tah_MWEHolidayPrvER 
                        + Tah_MWENightShiftCurER    + Tah_MWENightShiftPrvER 
                        + Tah_MWEOvertimeCurER      + Tah_MWEOvertimePrvER  
                        + Tah_MWEHazardCurER        + Tah_MWEHazardPrvER                            [NUMVAL TotalNontax]
		
                ,Tah_TaxableBasicNetPremiumsCurER + Tah_TaxableBasicNetPremiumsPrvER	            [NUMVAL Tah_TaxableBasicCurER]
                ,Tah_Taxable13thMonthCurER + Tah_Taxable13thMonthPrvER  
                        + Tah_TaxableSalariesCompensationCurER  + Tah_TaxableSalariesCompensationPrvER 
                        + Tah_TaxableOvertimeCurER + Tah_TaxableOvertimePrvER	                    [NUMVAL Tah_TaxableSalariesCompensationCurER]
		
               ,CASE WHEN Tah_TaxYear < '2018' THEN REPLACE(REPLACE(Tah_ExemptionCode, 'E', ''), ' ', '') ELSE '' END [Tah_ExemptionCode]

               ,Tah_ExemptionAmount                                                                 [NUMVAL Tah_ExemptionAmount]
               ,Tah_PremiumPaidOnHealth                                                             [NUMVAL Tah_PremiumPaidOnHealth]
	           ,Tah_NetTaxableIncome                                                                [NUMVAL NetTaxableCompensation]
               ,Tah_TaxDue                                                                          [NUMVAL Tah_TaxDue]
               FROM Udv_AlphalistHdr HEADER
               INNER JOIN @PROFILES..M_Employee ON Mem_IDNo = Tah_IDNo
                    AND Mem_WorkStatus <> 'IM'
               @USERCOSTCENTERACCESSCONDITION
               @CONDITIONS

                    ";
                #endregion
            }
            else if (schedule == "SCHED7.3")
            {
                #region Sched 7.3
                query += @"
            SELECT 
                'D7.3'                                                                              [ScheduleNumber]
                ,'1604CF'                                                                           [FormType]
                ,'{3}'                                                                              [TIN]
                ,'{1}'                                                                              [BranchCode]
                ,'12/31/{2}'                                                                        [ReturnPeriod]
                ,CAST(ROW_NUMBER() OVER (ORDER BY Tah_LastName, Tah_FirstName, Tah_MiddleName) AS VARCHAR(10)) [Seq Num]
                @EMPLOYEETIN
                ,'{1}'                                                                              [EmployeeBranchCode]
                @COMPLETENAME
                ,Tah_GrossCompensationIncome                                                        [NUMVAL GrossCompensationIncome]
                ,Tah_Nontaxable13thMonthCurER                                                       [NUMVAL Tah_Nontaxable13thMonthCurER]
                ,Tah_DeMinimisCurER                                                                 [NUMVAL Tah_DeMinimisCurER]
                ,Tah_PremiumsUnionDuesCurER                                                         [NUMVAL Tah_PremiumsUnionDuesCurER]
                ,Tah_NontaxableSalariesCompensationCurER  
                        + Tah_MWEBasicCurER  
                        + Tah_MWEHolidayCurER  
                        + Tah_MWEOvertimeCurER  
                        + Tah_MWENightShiftCurER  
                        + Tah_MWEHazardCurER                                                        [NUMVAL Tah_NontaxableSalariesCompensationCurER]
                ,Tah_NontaxableIncomeCurER					                                        [NUMVAL TotalNontax]
                ,Tah_TaxableBasicNetPremiumsCurER								                    [NUMVAL Tah_TaxableBasicCurER]			
                ,Tah_Taxable13thMonthCurER                                                          [NUMVAL Tah_Taxable13thMonthCurER]
                ,Tah_TaxableSalariesCompensationCurER + Tah_TaxableOvertimeCurER                    [NUMVAL Tah_TaxableSalariesCompensationCurER]
                ,Tah_TaxableBasicNetPremiumsCurER
			            + Tah_Taxable13thMonthCurER 
			            + Tah_TaxableSalariesCompensationCurER 
                        + Tah_TaxableOvertimeCurER		                                            [NUMVAL TotalTax]

               ,CASE WHEN Tah_TaxYear < '2018' THEN REPLACE(REPLACE(Tah_ExemptionCode, 'E', ''), ' ', '') ELSE '' END [Tah_ExemptionCode]
               ,Tah_ExemptionAmount                                                                 [NUMVAL Tah_ExemptionAmount]
               ,Tah_PremiumPaidOnHealth                                                             [NUMVAL Tah_PremiumPaidOnHealth]
               ,Tah_NetTaxableIncome                                                                [NUMVAL NetTaxableCompensation]
               ,Tah_TaxDue                                                                          [NUMVAL Tah_TaxDue]
               ,Tah_TaxWithheldCurER                                                                [NUMVAL Tah_TaxWithheldCurER]
        
               ,CASE WHEN Tah_TaxDue 
				            - Tah_TaxWithheldCurER > 0
				            THEN Tah_TaxDue 
					            - Tah_TaxWithheldCurER
				            ELSE 0 
			            END															                [NUMVAL TaxWithheldDec]
                ,CASE WHEN Tah_TaxDue 
				            - Tah_TaxWithheldCurER > 0
				            THEN 0
				            ELSE (Tah_TaxDue 
				            - Tah_TaxWithheldCurER) * -1
			            END															                [NUMVAL OverWithheld]
                ,Tah_TaxDue														                    [NUMVAL ActualWithheld]
                ,CASE WHEN Tah_IsSubstitutedFiling = 1
			            @SUBSTITUTEFORTEXTFILE
			            END															                [Tah_IsSubstitutedFiling]
                FROM Udv_AlphalistHdr HEADER
                INNER JOIN @PROFILES..M_Employee ON Mem_IDNo = Tah_IDNo
                    AND Mem_WorkStatus <> 'IM'
                @USERCOSTCENTERACCESSCONDITION
                @CONDITIONS
                ";
                #endregion
            }
            else if (schedule == "SCHED7.4")
            {
                #region Sched 7.4
                query += @"
                     SELECT 
                        'D7.4'                                                                                          [ScheduleNumber]
                        ,'1604CF'                                                                                       [FormType]
                        ,'{3}'                                                                                          [TIN]
                        ,'{1}'                                                                                          [BranchCode]
                        ,'12/31/{2}'                                                                                    [ReturnPeriod]
                        ,CAST(ROW_NUMBER() OVER (ORDER BY Tah_LastName, Tah_FirstName, Tah_MiddleName) AS VARCHAR(10))  [Seq Num]
                        @EMPLOYEETIN
                        ,'{1}'                                                                                          [EmployeeBranchCode]
                        @COMPLETENAME
                        ,Tah_GrossCompensationIncome                                                                    [NUMVAL GrossCompensationIncome]
                        ,Tah_Nontaxable13thMonthPrvER									                                [NUMVAL Tah_Nontaxable13thMonthPrvER]
                        ,Tah_DeMinimisPrvER									                                            [NUMVAL Tah_DeMinimisPrvER]
                        ,Tah_PremiumsUnionDuesPrvER									                                    [NUMVAL Tah_PremiumsUnionDuesPrvER]
                        ,Tah_NontaxableSalariesCompensationPrvER  
                                + Tah_MWEBasicPrvER   
                                + Tah_MWEHolidayPrvER  
                                + Tah_MWEOvertimePrvER  
                                + Tah_MWENightShiftPrvER  
                                + Tah_MWEHazardPrvER 						                                            [NUMVAL Tah_NontaxableSalariesCompensationPrvER]
        
                        ,Tah_Nontaxable13thMonthPrvER
			                    + Tah_DeMinimisPrvER
			                    + Tah_PremiumsUnionDuesPrvER
			                    + Tah_NontaxableSalariesCompensationPrvER  
                                + Tah_MWEBasicPrvER   
                                + Tah_MWEHolidayPrvER  
                                + Tah_MWEOvertimePrvER  
                                + Tah_MWENightShiftPrvER  
                                + Tah_MWEHazardPrvER 					                                                [NUMVAL TotalNontax Prev]
        
                        ,Tah_TaxableBasicNetPremiumsPrvER										                        [NUMVAL Tah_TaxableBasicPrvER]
                        ,Tah_Taxable13thMonthPrvER									                                    [NUMVAL Tah_Taxable13thMonthPrvER]
                        ,Tah_TaxableOvertimePrvER + Tah_TaxableSalariesCompensationPrvER   		                        [NUMVAL Tah_TaxableSalariesCompensationPrvER]
        
                        ,Tah_TaxableBasicNetPremiumsPrvER
			                    + Tah_Taxable13thMonthPrvER 
			                    + Tah_TaxableOvertimePrvER 
                                + Tah_TaxableSalariesCompensationPrvER	                                                [NUMVAL TotalTaxable Prev]
        
                        ,Tah_Nontaxable13thMonthCurER                                                                   [NUMVAL Tah_Nontaxable13thMonthCurER]
                        ,Tah_DeMinimisCurER                                                                             [NUMVAL Tah_DeMinimisCurER]
                        ,Tah_PremiumsUnionDuesCurER                                                                     [NUMVAL Tah_PremiumsUnionDuesCurER]
                        ,Tah_NontaxableSalariesCompensationCurER  
                                + Tah_MWEBasicCurER  
                                + Tah_MWEHolidayCurER  
                                + Tah_MWEOvertimeCurER  
                                + Tah_MWENightShiftCurER  
                                + Tah_MWEHazardCurER                                                                    [NUMVAL Tah_NontaxableSalariesCompensationCurER]
		 
                        ,Tah_Nontaxable13thMonthCurER 
			                    + Tah_DeMinimisCurER
			                    + Tah_PremiumsUnionDuesCurER
			                    + Tah_NontaxableSalariesCompensationCurER  
                                + Tah_MWEBasicCurER  
                                + Tah_MWEHolidayCurER  
                                + Tah_MWEOvertimeCurER  
                                + Tah_MWENightShiftCurER  
                                + Tah_MWEHazardCurER					                                                [NUMVAL TotalNontax Pres]
		
                        ,Tah_TaxableBasicNetPremiumsCurER										                        [NUMVAL Tah_TaxableBasicCurER]
                        ,Tah_Taxable13thMonthCurER                                                                      [NUMVAL Tah_Taxable13thMonthCurER]
                        ,Tah_TaxableOvertimeCurER + Tah_TaxableSalariesCompensationCurER  					            [NUMVAL Tah_TaxableSalariesCompensationCurER]

                        ,Tah_TaxableBasicNetPremiumsCurER
			                + Tah_Taxable13thMonthCurER 
			                + Tah_TaxableOvertimeCurER + Tah_TaxableSalariesCompensationCurER					        [NUMVAL TotalCompensation Pres]
        
                        ,Tah_TaxableBasicNetPremiumsPrvER
			                + Tah_Taxable13thMonthPrvER 
			                + Tah_TaxableOvertimePrvER 
			                + Tah_TaxableSalariesCompensationPrvER 
			
			                + Tah_TaxableBasicNetPremiumsCurER
			                + Tah_Taxable13thMonthCurER 
			                + Tah_TaxableOvertimeCurER 
			                + Tah_TaxableSalariesCompensationCurER					                                    [NUMVAL TotalTaxable PresPrev]
                                		
                        ,CASE WHEN Tah_TaxYear < '2018' THEN REPLACE(REPLACE(Tah_ExemptionCode, 'E', ''), ' ', '') ELSE '' END  [Tah_ExemptionCode]
                        ,Tah_ExemptionAmount                                                                            [NUMVAL Tah_ExemptionAmount]
                        ,Tah_PremiumPaidOnHealth                                                                        [NUMVAL Tah_PremiumPaidOnHealth]
                        ,Tah_NetTaxableIncome                                                                           [NUMVAL NetTaxableCompensation]
		
                        ,Tah_TaxDue                                                                                     [NUMVAL Tah_TaxDue]
                        ,Tah_TaxWithheldPrvER											                                [NUMVAL Tah_TaxWithheldPrvER]
                        ,Tah_TaxWithheldCurER                                                                           [NUMVAL Tah_TaxWithheldCurER]
                        ,CASE WHEN Tah_TaxDue 
				                    - (Tah_TaxWithheldCurER + Tah_TaxWithheldPrvER) > 0
				                    THEN Tah_TaxDue 
					                    - (Tah_TaxWithheldCurER + Tah_TaxWithheldPrvER)
				                    ELSE 0 
			                    END															                            [NUMVAL TaxWithheldDec]
                        ,CASE WHEN Tah_TaxDue 
				                    - (Tah_TaxWithheldCurER + Tah_TaxWithheldPrvER) > 0
				                    THEN 0
				                    ELSE (Tah_TaxDue 
				                    - (Tah_TaxWithheldCurER + Tah_TaxWithheldPrvER)) * -1
			                    END                                                                                     [NUMVAL OverWithheld]
                         ,Tah_TaxDue														                            [NUMVAL ActualWithheld]
                        FROM Udv_AlphalistHdr HEADER
                        INNER JOIN @PROFILES..M_Employee ON Mem_IDNo = Tah_IDNo
                            AND Mem_WorkStatus <> 'IM'
                        @USERCOSTCENTERACCESSCONDITION
                        @CONDITIONS

                        ";
                #endregion
            }
            else if (schedule == "SCHED7.5")
            {
                #region Sched 7.5
                query += @"
                 SELECT 
                        'D7.5'                                                                  [ScheduleNumber]
                        ,'1604CF'                                                               [FormType]
                        ,'{3}'                                                                  [TIN]
                        ,'{1}'                                                                  [BranchCode]
                        ,'12/31/{2}'                                                            [ReturnPeriod]
                        ,CAST(ROW_NUMBER() OVER (ORDER BY Tah_LastName, Tah_FirstName, Tah_MiddleName) AS VARCHAR(10)) [Seq Num]
                        @EMPLOYEETIN
                        ,'{1}'                                                                  [EmployeeBranchCode]
                        @COMPLETENAME
	                    ,ISNULL(Tal_MWERegion,'')  										        [Region No Assigned]
                        ,(Tah_MWEBasicPrvER 
			                + Tah_MWEHolidayPrvER 
			                + Tah_MWEOvertimePrvER 
			                + Tah_MWENightShiftPrvER 
			                + Tah_MWEHazardPrvER 
			                + Tah_Nontaxable13thMonthPrvER 
			                + Tah_DeMinimisPrvER 
			                + Tah_PremiumsUnionDuesPrvER 
			                + Tah_NontaxableSalariesCompensationPrvER

                            + Tah_Taxable13thMonthPrvER 
			                + Tah_TaxableBasicNetPremiumsPrvER
			                + Tah_TaxableOvertimePrvER 
			                + Tah_TaxableSalariesCompensationPrvER)					            [NUMVAL GrossCompensationIncome Prev]
        
                        ,Tah_MWEBasicPrvER											            [NUMVAL Tah_MWEBasicPrvER]
                        ,Tah_MWEHolidayPrvER											        [NUMVAL Tah_MWEHolidayPrvER]
                        ,Tah_MWEOvertimePrvER										            [NUMVAL Tah_MWEOvertimePrvER]
                        ,Tah_MWENightShiftPrvER											        [NUMVAL Tah_MWENightShiftPrvER]
                        ,Tah_MWEHazardPrvER											            [NUMVAL Tah_MWEHazardPrvER]
                        ,Tah_Nontaxable13thMonthPrvER									        [NUMVAL Tah_Nontaxable13thMonthPrvER]
                        ,Tah_DeMinimisPrvER									                    [NUMVAL Tah_DeMinimisPrvER]
                        ,Tah_PremiumsUnionDuesPrvER									            [NUMVAL Tah_PremiumsUnionDuesPrvER]
                        ,Tah_NontaxableSalariesCompensationPrvER 						        [NUMVAL Tah_NontaxableSalariesCompensationPrvER]
        
                        , Tah_MWEBasicPrvER 
			                + Tah_MWEHolidayPrvER 
			                + Tah_MWEOvertimePrvER 
			                + Tah_MWENightShiftPrvER 
			                + Tah_MWEHazardPrvER 
			                + Tah_Nontaxable13thMonthPrvER 
			                + Tah_DeMinimisPrvER 
			                + Tah_PremiumsUnionDuesPrvER 
			                + Tah_NontaxableSalariesCompensationPrvER					        [NUMVAL TotalNontax Prev]
        
                        ,Tah_Taxable13thMonthPrvER									            [NUMVAL Tah_Taxable13thMonthPrvER]
                        ,Tah_TaxableSalariesCompensationPrvER   					            [NUMVAL Tah_TaxableSalariesCompensationPrvER]
        
                        ,Tah_Taxable13thMonthPrvER 
			                + Tah_TaxableSalariesCompensationPrvER					            [NUMVAL TotalTax Prev]
        
                        ,CONVERT(VARCHAR(20), HEADER.Tah_StartDate, 101)				        [StartPeriod]
                        ,CONVERT(VARCHAR(20), HEADER.Tah_EndDate, 101)				            [EndPeriod]
        
                        ,(Tah_MWEHolidayCurER											
                                + Tah_MWEOvertimeCurER
                                + Tah_MWENightShiftCurER
                                + Tah_MWEHazardCurER
                                + Tah_Nontaxable13thMonthCurER
                                + Tah_DeMinimisCurER	
                                + Tah_PremiumsUnionDuesCurER	
                                + Tah_NontaxableSalariesCompensationCurER
			                        + Tah_MWEBasicCurER 
			                        + Tah_TaxableBasicNetPremiumsCurER
			                        + Tah_TaxableOvertimeCurER)          					    [NUMVAL Nontax Gross Compensation Pres]
        
                        ,ISNULL(Tal_MWEBasicPerDay,0)											[NUMVAL Tah_MWEBasicPerDay]
                        ,ISNULL(Tal_MWEBasicPerMonth,0)											[NUMVAL Tah_MWEBasicPerMonth]
                        ,ISNULL(Tal_MWEBasicPerYear,0)										    [NUMVAL Tah_MWEBasicPerYear]
                        ,ISNULL(CAST(Tal_MWEDaysPerYear AS VARCHAR),'')							[Tah_MWEDaysPerYear]
                        ,Tah_MWEHolidayCurER											        [NUMVAL Tah_MWEHolidayCurER]
                        ,Tah_MWEOvertimeCurER										            [NUMVAL Tah_MWEOvertimeCurER]
                        ,Tah_MWENightShiftCurER											        [NUMVAL Tah_MWENightShiftCurER]
                        ,Tah_MWEHazardCurER											            [NUMVAL Tah_MWEHazardCurER]
                        ,Tah_Nontaxable13thMonthCurER									        [NUMVAL Tah_Nontaxable13thMonthCurER]
                        ,Tah_DeMinimisCurER									                    [NUMVAL Tah_DeMinimisCurER]
                        ,Tah_PremiumsUnionDuesCurER									            [NUMVAL Tah_PremiumsUnionDuesCurER]
                        ,Tah_MWEBasicCurER + Tah_NontaxableSalariesCompensationCurER            [NUMVAL Tah_NontaxableSalariesCompensationCurER]
        
                        ,Tah_Taxable13thMonthCurER									            [NUMVAL Tah_Taxable13thMonthCurER]
                        ,Tah_TaxableSalariesCompensationCurER						            [NUMVAL Tah_TaxableSalariesCompensationCurER]
        
                        ,Tah_Taxable13thMonthCurER
                                + Tah_TaxableSalariesCompensationCurER                          [NUMVAL Total Compensation Present]
        
                        ,( Tah_Taxable13thMonthPrvER 
		                    + Tah_TaxableSalariesCompensationPrvER) 
				
			            + (Tah_Taxable13thMonthCurER
                              + Tah_TaxableSalariesCompensationCurER)			                [NUMVAL Total Compensation Pres Prev]
        
            		
                        ,CASE WHEN Tah_TaxYear < '2018' THEN REPLACE(REPLACE(Tah_ExemptionCode, 'E', ''), ' ', '') ELSE '' END [Tah_ExemptionCode]

                        ,Tah_ExemptionAmount                                                    [NUMVAL Tah_ExemptionAmount]
                        ,Tah_PremiumPaidOnHealth                                                [NUMVAL Tah_PremiumPaidOnHealth]
        
                        ,CASE WHEN ((
                            (Tah_Taxable13thMonthPrvER 
			                    + Tah_TaxableBasicNetPremiumsPrvER 
			                    + Tah_TaxableOvertimePrvER 
			                    + Tah_TaxableSalariesCompensationPrvER) 
				
			                + (Tah_TaxableBasicNetPremiumsCurER
			                        + Tah_TaxableOvertimeCurER
                                + Tah_Taxable13thMonthCurER
                                + Tah_TaxableSalariesCompensationCurER)
                                    )
		                            - Tah_ExemptionAmount 
		                            - Tah_PremiumPaidOnHealth) > 0
                            THEN ((
                            (Tah_Taxable13thMonthPrvER 
			                    + Tah_TaxableBasicNetPremiumsPrvER
			                    + Tah_TaxableOvertimePrvER 
			                    + Tah_TaxableSalariesCompensationPrvER) 
				
			                + (Tah_TaxableBasicNetPremiumsCurER
			                        + Tah_TaxableOvertimeCurER
                                + Tah_Taxable13thMonthCurER
                                + Tah_TaxableSalariesCompensationCurER)
                                    )
		                            - Tah_ExemptionAmount 
		                            - Tah_PremiumPaidOnHealth)
                            ELSE 0 END                                                      [NUMVAL NetTaxableCompensation]
		
                        ,Tah_TaxDue                                                         [NUMVAL Tah_TaxDue]
                        ,Tah_TaxWithheldPrvER											    [NUMVAL Tah_TaxWithheldPrvER]
                        ,Tah_TaxWithheldCurER                                               [NUMVAL Tah_TaxWithheldCurER]
                        ,CASE WHEN Tah_TaxDue 
				                - (Tah_TaxWithheldCurER + Tah_TaxWithheldPrvER) > 0
				                THEN Tah_TaxDue 
					                - (Tah_TaxWithheldCurER + Tah_TaxWithheldPrvER) 
				                ELSE 0 
			                END															    [NUMVAL TaxWithheldDec]
                        ,CASE WHEN Tah_TaxDue 
				                - (Tah_TaxWithheldCurER + Tah_TaxWithheldPrvER)  > 0
				                THEN 0
				                ELSE (Tah_TaxDue 
				                - (Tah_TaxWithheldCurER + Tah_TaxWithheldPrvER) ) * -1
			                END															    [NUMVAL OverWithheld]
                        ,Tah_TaxDue														    [NUMVAL ActualWithheld]
                        FROM Udv_AlphalistHdr HEADER
                        INNER JOIN @PROFILES..M_Employee ON Mem_IDNo = Tah_IDNo
                            AND Mem_WorkStatus <> 'IM'
                        @USERCOSTCENTERACCESSCONDITION
                        LEFT JOIN T_Alphalist ON Tal_TaxYear = Tah_TaxYear 
	                        AND Tal_CompanyCode = '{0}'
                        @CONDITIONS

";
                #endregion
            }
            else if (schedule == "SCHED1")
            {
                #region Sched 1
                query += @"
                    SELECT 'D1'                                                                [ScheduleNumber]
                        ,'1604C'                                                                [FormType]
                        ,'{3}'                                                                  [TIN]
                        ,'{1}'                                                                  [BranchCode]
                        ,'12/31/{2}'                                                            [ReturnPeriod]
                    ,CAST(ROW_NUMBER() OVER (ORDER BY Tah_LastName, Tah_FirstName, Tah_MiddleName) AS VARCHAR(10)) [Seq Num]
                    ,'{1}'                                                                      [EmployeeBranchCode]
                    @EMPLOYEETIN
                    @COMPLETENAME
                    , T_Alphalist.Tal_MWERegion                                                 [Region No. Where Assigned]
                    , Mcd_Name                                                                  [Nationality]
	                , Tah_CurrentEmploymentStatus                                               [Current Employment Status]
                    , CONVERT(VARCHAR(20), Tah_StartDate, 101)                                  [StartPeriod]
                    , CONVERT(VARCHAR(20), Tah_EndDate, 101)                                    [EndPeriod]
                    , Tah_SeparationReason                                                      [Reason of Separation]
                    , Tah_TaxableIncomeCurER + Tah_NontaxableIncomeCurER                        [NUMVAL GrossCompensationIncomeCurER] 
                    , Tah_MWEBasicCurER                                                         [NUMVAL Tah_MWEBasicCurER]                                                									            
                    , Tah_Nontaxable13thMonthCurER                                              [NUMVAL Tah_Nontaxable13thMonthCurER]
		            , Tah_DeMinimisCurER                                                        [NUMVAL Tah_DeMinimisCurER]
		            , Tah_PremiumsUnionDuesCurER                                                [NUMVAL Tah_PremiumsUnionDuesCurER]
		            , CASE WHEN 'TRUE' = '" + (isTextfileGeneration ? "TRUE" : "FALSE") + @"' 
                        THEN Tah_NontaxableSalariesCompensationCurER 
                                + Tah_MWEHolidayCurER 
                                + Tah_MWEOvertimeCurER
                                + Tah_MWENightShiftCurER 
                                + Tah_MWEHazardCurER
                        ELSE Tah_NontaxableSalariesCompensationCurER 
                            + Tah_MWEBasicCurER
                            + Tah_MWEHolidayCurER 
                            + Tah_MWEOvertimeCurER
                            + Tah_MWENightShiftCurER 
                            + Tah_MWEHazardCurER    
                        END                                                                     [NUMVAL Tah_NontaxableSalariesCompensationCurER]
		            , Tah_NontaxableIncomeCurER                                                 [NUMVAL Tah_NontaxableIncomeCurER]
		            , Tah_TaxableBasicNetPremiumsCurER	                                        [NUMVAL Tah_TaxableBasicNetPremiumsCurER]
		            , Tah_Taxable13thMonthCurER                                                 [NUMVAL Tah_Taxable13thMonthCurER]
		            , Tah_TaxableSalariesCompensationCurER + Tah_TaxableOvertimeCurER 	        [NUMVAL Tah_TaxableSalariesCompensationCurER]
                    , Tah_TaxableIncomeCurER                                                    [NUMVAL Tah_TaxableIncomeCurER]
		
                    , CASE WHEN Tyt_TIN <> 'APPLIED' 
						THEN LEFT(REPLACE(Tyt_TIN, '-', ''), 3) + '-' 
							+ RIGHT(LEFT(REPLACE(Tyt_TIN, '-', ''), 6),3) + '-' 
							+ RIGHT(LEFT(REPLACE(Tyt_TIN, '-', ''), 9),3) + '-' + '0000' 
						ELSE '' 
						END		                                                                [YTD TIN]
                    , Tyt_EmploymentStatus                                                      [YTD Employment Status] 
	                , CONVERT(VARCHAR(20), Tyt_StartDate, 101)                                  [YTD StartPeriod]
	                , CONVERT(VARCHAR(20), Tyt_EndDate, 101)                                    [YTD EndPeriod]            
	                , Tyt_SeparationReason                                                      [YTD Reason of Separation] 

	                , Tah_TaxableIncomePrvER + Tah_NontaxableIncomePrvER                        [NUMVAL GrossCompensationIncomePrvER]
                    , Tah_MWEBasicPrvER                                                         [NUMVAL Tah_MWEBasicPrvER]
			        , Tah_Nontaxable13thMonthPrvER                                              [NUMVAL Tah_Nontaxable13thMonthPrvER]
	                , Tah_DeMinimisPrvER                                                        [NUMVAL Tah_DeMinimisPrvER]
	                , Tah_PremiumsUnionDuesPrvER                                                [NUMVAL Tah_PremiumsUnionDuesPrvER]
	                , CASE WHEN 'TRUE' = '" + (isTextfileGeneration ? "TRUE" : "FALSE") + @"' 
                        THEN Tah_NontaxableSalariesCompensationPrvER   
                            + Tah_MWEHolidayPrvER 
                            + Tah_MWEOvertimePrvER
                            + Tah_MWENightShiftPrvER 
                            + Tah_MWEHazardPrvER  
                        ELSE
                            Tah_NontaxableSalariesCompensationPrvER   
                                + Tah_MWEBasicPrvER
                                + Tah_MWEHolidayPrvER 
                                + Tah_MWEOvertimePrvER
                                + Tah_MWENightShiftPrvER 
                                + Tah_MWEHazardPrvER
                        END                                                                     [NUMVAL Tah_NontaxableSalariesCompensationPrvER]
	                , Tah_NontaxableIncomePrvER                                                 [NUMVAL Tah_NontaxableIncomePrvER]
	                , Tah_TaxableBasicNetPremiumsPrvER                                          [NUMVAL Tah_TaxableBasicNetPremiumsPrvER]
	                , Tah_Taxable13thMonthPrvER                                                 [NUMVAL Tah_Taxable13thMonthPrvER]
	                , Tah_TaxableSalariesCompensationPrvER + Tah_TaxableOvertimePrvER           [NUMVAL Tah_TaxableSalariesCompensationPrvER]
	                , Tah_TaxableIncomePrvER                                                    [NUMVAL Tah_TaxableIncomePrvER]
	                , Tah_TaxableIncomeCurER + Tah_TaxableIncomePrvER                           [NUMVAL TaxableIncomeCurPrvER]
                    , Tah_TaxDue													            [NUMVAL TaxDue]
                    , Tah_TaxWithheldPrvER                                                      [NUMVAL Tah_TaxWithheldPrvER]
                    , Tah_TaxWithheldCurER                                                      [NUMVAL Tah_TaxWithheldCurER]
                    , CASE WHEN Tah_TaxAmount > 0 THEN Tah_TaxAmount ELSE 0 END                 [NUMVAL Tax Payable]
	                , CASE WHEN Tah_TaxAmount < 0 THEN Tah_TaxAmount*-1 ELSE 0 END              [NUMVAL Tax Refund]
	                , Tah_TaxDue                                                                [NUMVAL AMOUNT OF TAX WITHHELD AS ADJUSTED]
                    , CASE WHEN Tah_IsSubstitutedFiling = 1
			                @SUBSTITUTEFORTEXTFILE
			                END															        [Tah_IsSubstitutedFiling]
                  FROM Udv_AlphalistHdr
                  INNER JOIN @PROFILES..M_Employee ON Mem_IDNo = Tah_IDNo
                        AND Mem_WorkStatus <> 'IM'
                  @USERCOSTCENTERACCESSCONDITION
                  LEFT JOIN Udv_YearToDate ON Tyt_IDNo = Tah_IDNo
	                    AND Tyt_TaxYear = Tah_TaxYear
	                    AND Tyt_EmployerType = 'P'
                  LEFT JOIN T_Alphalist ON Tal_CompanyCode = '{0}'
	                   AND Tal_TaxYear = Tah_TaxYear
                  LEFT JOIN M_CodeDtl ON Mcd_Code = Tah_Nationality
	                    AND Mcd_CompanyCode = '{0}'
	                    AND Mcd_CodeType ='CITIZEN'
                  @CONDITIONS
                        ";
                #endregion
            }
            else if (schedule == "SCHED2")
            {
                #region Sched 2
                query += @"
                    SELECT 'D2'                                                                 [ScheduleNumber]
                        ,'1604C'                                                                [FormType]
                        ,'{3}'                                                                  [TIN]
                        ,'{1}'                                                                  [BranchCode]
                        ,'12/31/{2}'                                                            [ReturnPeriod]
                    , CAST(ROW_NUMBER() OVER (ORDER BY Tah_LastName, Tah_FirstName, Tah_MiddleName) AS VARCHAR(10)) [Seq Num]
                    ,'{1}'                                                                      [EmployeeBranchCode]
                    @EMPLOYEETIN
                    @COMPLETENAME
                    , Mcd_Name                                                                  [Nationality]   
	                , Tah_CurrentEmploymentStatus                                               [Current Employment Status]
                    , T_Alphalist.Tal_MWERegion                                                 [Region No. Where Assigned]
                    , CONVERT(VARCHAR(20), Tah_StartDate, 101)                                  [StartPeriod]
                    , CONVERT(VARCHAR(20), Tah_EndDate, 101)                                    [EndPeriod]
                    , Tah_SeparationReason                                                      [Reason of Separation]
                    , Tah_TaxableIncomeCurER + Tah_NontaxableIncomeCurER                        [NUMVAL GrossCompensationIncomeCurER]  

                    , T_Alphalist.Tal_MWEBasicPerDay                                            [NUMVAL Tal_MWEBasicPerDay]                             
	                , T_Alphalist.Tal_MWEBasicPerMonth                                          [NUMVAL Tal_MWEBasicPerMonth] 
	                , T_Alphalist.Tal_MWEBasicPerYear                                           [NUMVAL Tal_MWEBasicPerYear] 
	                , T_Alphalist.Tal_MWEDaysPerYear                                            [Tal_MWEDaysPerYear]                                            									            
                    , Tah_MWEBasicCurER                                                         [NUMVAL Tah_MWEBasicCurER] 
	                , Tah_MWEHolidayCurER                                                       [NUMVAL Tah_MWEHolidayCurER] 
	                , Tah_MWEOvertimeCurER                                                      [NUMVAL Tah_MWEOvertimeCurER] 
	                , Tah_MWENightShiftCurER                                                    [NUMVAL Tah_MWENightShiftCurER] 
	                , Tah_MWEHazardCurER                                                        [NUMVAL Tah_MWEHazardCurER] 
                    , Tah_Nontaxable13thMonthCurER                                              [NUMVAL Tah_Nontaxable13thMonthCurER] 
	                , Tah_DeMinimisCurER                                                        [NUMVAL Tah_DeMinimisCurER] 
	                , Tah_PremiumsUnionDuesCurER                                                [NUMVAL Tah_PremiumsUnionDuesCurER] 
	                , CASE WHEN 'TRUE' = '" + (isTextfileGeneration ? "TRUE" : "FALSE") + @"' 
                       THEN Tah_NontaxableSalariesCompensationCurER + Tah_MWEBasicCurER
                       ELSE Tah_NontaxableSalariesCompensationCurER END                         [NUMVAL Tah_NontaxableSalariesCompensationCurER] 
	                , Tah_NontaxableIncomeCurER                                                 [NUMVAL Tah_NontaxableIncomeCurER]

	                , Tah_Taxable13thMonthCurER                                                 [NUMVAL Tah_Taxable13thMonthCurER]
	                , Tah_TaxableBasicNetPremiumsCurER 
                            + Tah_TaxableSalariesCompensationCurER 
                            + Tah_TaxableOvertimeCurER                                          [NUMVAL Tah_TaxableSalariesCompensationCurER]
	                , Tah_TaxableIncomeCurER                                                    [NUMVAL Tah_TaxableIncomeCurER]
		
                    , CASE WHEN Tyt_TIN <> 'APPLIED' 
						THEN LEFT(REPLACE(Tyt_TIN, '-', ''), 3) + '-' 
							+ RIGHT(LEFT(REPLACE(Tyt_TIN, '-', ''), 6),3) + '-' 
							+ RIGHT(LEFT(REPLACE(Tyt_TIN, '-', ''), 9),3) + '-' + '0000' 
						ELSE '' 
						END		                                                                [YTD TIN]
                    , Tyt_EmploymentStatus                                                      [YTD Employment Status] 
	                , CONVERT(VARCHAR(20), Tyt_StartDate, 101)                                  [YTD StartPeriod]
	                , CONVERT(VARCHAR(20), Tyt_EndDate, 101)                                    [YTD EndPeriod]            
	                , Tyt_SeparationReason                                                      [YTD Reason of Separation]

	                , Tah_TaxableIncomePrvER + Tah_NontaxableIncomePrvER                        [NUMVAL GrossCompensationIncomePrvER]
                    , Tah_MWEBasicPrvER                                                         [NUMVAL Tah_MWEBasicPrvER]
	                , Tah_MWEHolidayPrvER                                                       [NUMVAL Tah_MWEHolidayPrvER]
	                , Tah_MWEOvertimePrvER                                                      [NUMVAL Tah_MWEOvertimePrvER]
	                , Tah_MWENightShiftPrvER                                                    [NUMVAL Tah_MWENightShiftPrvER]
	                , Tah_MWEHazardPrvER                                                        [NUMVAL Tah_MWEHazardPrvER]
	                , Tah_Nontaxable13thMonthPrvER                                              [NUMVAL Tah_Nontaxable13thMonthPrvER]
	                , Tah_DeMinimisPrvER                                                        [NUMVAL Tah_DeMinimisPrvER] 
	                , Tah_PremiumsUnionDuesPrvER                                                [NUMVAL Tah_PremiumsUnionDuesPrvER]
	                , Tah_NontaxableSalariesCompensationPrvER                                   [NUMVAL Tah_NontaxableSalariesCompensationPrvER]
	                , Tah_NontaxableIncomePrvER                                                 [NUMVAL Tah_NontaxableIncomePrvER]

	                , Tah_Taxable13thMonthPrvER                                                 [NUMVAL Tah_Taxable13thMonthPrvER]
	                , Tah_TaxableBasicNetPremiumsPrvER 
                            + Tah_TaxableSalariesCompensationPrvER 
                            + Tah_TaxableOvertimePrvER                                          [NUMVAL Tah_TaxableSalariesCompensationPrvER]
	                , Tah_TaxableIncomePrvER                                                    [NUMVAL Tah_TaxableIncomePrvER]

	                , Tah_TaxableIncomeCurER + Tah_TaxableIncomePrvER                           [NUMVAL TaxableIncomeCurPrvER]
                    , Tah_TaxDue													            [NUMVAL TaxDue]
                    , Tah_TaxWithheldPrvER                                                      [NUMVAL Tah_TaxWithheldPrvER]
                    , Tah_TaxWithheldCurER                                                      [NUMVAL Tah_TaxWithheldCurER]
                    , CASE WHEN Tah_TaxAmount > 0 THEN Tah_TaxAmount ELSE 0 END                 [NUMVAL Tax Payable]
	                , CASE WHEN Tah_TaxAmount < 0 THEN Tah_TaxAmount*-1 ELSE 0 END              [NUMVAL Tax Refund]
	                , Tah_TaxDue                                                                [NUMVAL AMOUNT OF TAX WITHHELD AS ADJUSTED]
                    , CASE WHEN Tah_IsSubstitutedFiling = 1
			              @SUBSTITUTEFORTEXTFILE
			              END															        [Tah_IsSubstitutedFiling]
                  FROM Udv_AlphalistHdr
                  INNER JOIN @PROFILES..M_Employee ON Mem_IDNo = Tah_IDNo
                        AND Mem_WorkStatus <> 'IM'
                  @USERCOSTCENTERACCESSCONDITION
                  LEFT JOIN Udv_YearToDate ON Tyt_IDNo = Tah_IDNo
	                    AND Tyt_TaxYear = Tah_TaxYear
	                    AND Tyt_EmployerType = 'P'
                  LEFT JOIN T_Alphalist ON Tal_CompanyCode = '{0}'
	                    AND Tal_TaxYear = Tah_TaxYear
                  LEFT JOIN M_CodeDtl ON Mcd_Code = Tah_Nationality
	                    AND Mcd_CompanyCode = '{0}'
	                    AND Mcd_CodeType ='CITIZEN'
                  @CONDITIONS
                  ";
                #endregion
            }

            #region Employee TIN
            query = query.Replace("@EMPLOYEETIN", isTextfileGeneration ? @"
,CASE WHEN Tah_TIN <> 'APPLIED' AND LEN(Tah_TIN) IN (9,12) THEN LEFT(REPLACE(Tah_TIN, '-', ''), 3) ELSE '000' END 
    + CASE WHEN Tah_TIN <> 'APPLIED' AND LEN(Tah_TIN) = 12 THEN RIGHT(LEFT(REPLACE(Tah_TIN, '-', ''),6),3)
				WHEN Tah_TIN <> 'APPLIED' AND LEN(Tah_TIN) = 9 THEN LEFT(RIGHT(REPLACE(Tah_TIN, '-', ''),6),3) ELSE '000' END
    + CASE WHEN Tah_TIN <> 'APPLIED' AND LEN(Tah_TIN) = 12 THEN LEFT(RIGHT(REPLACE(Tah_TIN, '-', ''),6),3) 
				WHEN Tah_TIN <> 'APPLIED' AND LEN(Tah_TIN) = 9 THEN RIGHT(REPLACE(Tah_TIN, '-', ''), 3) ELSE '000' END  [EmployeeTin]															        
" : @"
,CASE WHEN Tah_TIN <> 'APPLIED' 
		THEN LEFT(REPLACE(Tah_TIN, '-', ''), 3) + '-' 
			+ RIGHT(LEFT(REPLACE(Tah_TIN, '-', ''), 6),3) + '-' 
			+ RIGHT(LEFT(REPLACE(Tah_TIN, '-', ''), 9),3) + '-' + '{1}' 
		ELSE '000-000-000-{1}' 
		END																        [EmployeeTin]
");
            #endregion
            #region Set Substitute Filing for Textfile
            query = query.Replace("@SUBSTITUTEFORTEXTFILE", isTextfileGeneration ? @"
            THEN 'Y'
			ELSE 'N'
            "
            : @"
            THEN 'Yes'
			ELSE 'No'
            ");
            #endregion
            #region Last Name, First Name, Middle Name
            query = query.Replace("@COMPLETENAME", isTextfileGeneration ? @"
," + "'\"'" + " + Tah_LastName + " + "'\"'" + @"                        [Last NAME]
," + "'\"'" + " + Tah_FirstName + " + "'\"'" + @"                       [First NAME]
," + "'\"'" + " + Tah_MiddleName + " + "'\"'" + @"                      [Middle NAME]"
                                                                        : @"
,Tah_LastName                                                           [Last NAME]
,Tah_FirstName                                                          [First NAME]
,Tah_MiddleName                                                         [Middle NAME]");
            #endregion
            return query;
        }
        public static string Get2316Query()
        {
            string query = string.Empty;
            query = @"
                   
        SELECT Tah_IDNo  AS [EMPID]
	        ,Tah_TaxYear AS [Year]
	        , RIGHT('00' + CONVERT(NVARCHAR(2), DATEPART(MONTH, Tah_StartDate)), 2) + '/' + RIGHT('00' + CONVERT(NVARCHAR(2), DATEPART(DAY, Tah_StartDate)), 2) [PeriodStart]
			, RIGHT('00' + CONVERT(NVARCHAR(2), DATEPART(MONTH, Tah_EndDate)), 2) + '/' + RIGHT('00' + CONVERT(NVARCHAR(2), DATEPART(DAY, Tah_EndDate)), 2) [PeriodEnd]
            ,CASE WHEN Tah_TIN <> 'APPLIED' AND LEN(Tah_TIN) IN (9,12) THEN LEFT(REPLACE(Tah_TIN, '-', ''), 3) ELSE '' END [TIN1]
            ,CASE WHEN Tah_TIN <> 'APPLIED' AND LEN(Tah_TIN) = 12 THEN RIGHT(LEFT(REPLACE(Tah_TIN, '-', ''),6),3)
				WHEN Tah_TIN <> 'APPLIED' AND LEN(Tah_TIN) = 9 THEN LEFT(RIGHT(REPLACE(Tah_TIN, '-', ''),6),3) ELSE '' END [TIN2]
            ,CASE WHEN Tah_TIN <> 'APPLIED' AND LEN(Tah_TIN) = 12 THEN LEFT(RIGHT(REPLACE(Tah_TIN, '-', ''),6),3) 
				WHEN Tah_TIN <> 'APPLIED' AND LEN(Tah_TIN) = 9 THEN RIGHT(REPLACE(Tah_TIN, '-', ''), 3) ELSE '' END  [TIN3]
            ,CASE WHEN Tah_TIN <> 'APPLIED' AND LEN(Tah_TIN) = 12 THEN RIGHT(REPLACE(Tah_TIN, '-', ''), 3) 
				WHEN Tah_TIN <> 'APPLIED' AND LEN(Tah_TIN) = 9 THEN '000' ELSE '' END  [TIN4]
            , ISNULL(M_Employer.Mcm_RDOCode, M_Company.Mcm_RDOCode) AS [RDOCode]
            ,Tah_Lastname +  CASE WHEN LEN(ISNULL(Tah_ExtensionName,'')) > 0 THEN  ' ' + Tah_ExtensionName ELSE '' END + ', ' + Tah_FirstName + ' ' + Tah_MiddleName AS [EmployeeName]
	        ,Tah_FirstName + ' ' + LEFT(Tah_MiddleName,1) + '. ' + Tah_LastName AS [SigEmployeeName]
            ,Tah_Lastname +  CASE WHEN LEN(ISNULL(Tah_ExtensionName,'')) > 0 THEN  ' ' + Tah_ExtensionName ELSE '' END  AS Tah_LastName
	        ,Tah_FirstName
	        ,Tah_MiddleName
            ,Tah_PresCompleteAddress AS [Address1]
	        ,Tah_PresAddressMunicipalityCity AS [Zipcode1]
	        ,Tah_ProvCompleteAddress AS [Address2]
	        ,Tah_ProvAddressMunicipalityCity AS [Zipcode2]
            ,''  AS [Address3]
            ,''  AS [Zipcode3]
            ,DATEPART(MONTH, Mem_BirthDate) [BirthMonth]
            ,DATEPART(DAY, Mem_BirthDate) [BirthDay]
            ,DATEPART(YEAR, Mem_BirthDate) [BirthYear]
            ,Tah_TelephoneNo AS [Contact]
            ,CASE WHEN Mem_CivilStatusCode <> 'M'
                THEN 'X'
                ELSE '' 
                END AS [isSingle]
            ,CASE WHEN Mem_CivilStatusCode = 'M'
                THEN 'X'
                ELSE '' 
                END AS [isMarried]
            ,CASE WHEN Tah_WifeClaim = 'Y' 
		        THEN 'X'
                ELSE ''
                END AS [isWifeClaim]
            ,CASE WHEN Tah_WifeClaim <> 'Y'
		        THEN 'X'
                ELSE ''
                END AS [isNotWifeClaim]
            ,D1.dependentName [Dependent1]
            ,DATEPART(MONTH, D1.dependentBirth) [Dependent1BMonth]
            ,DATEPART(DAY, D1.dependentBirth) [Dependent1BDay]
            ,DATEPART(YEAR, D1.dependentBirth) [Dependent1BYear]
            ,D2.dependentName [Dependent2]
            ,DATEPART(MONTH, D2.dependentBirth) [Dependent2BMonth]
            ,DATEPART(DAY, D2.dependentBirth) [Dependent2BDay]
            ,DATEPART(YEAR, D2.dependentBirth) [Dependent2BYear]
            ,D3.dependentName [Dependent3]
            ,DATEPART(MONTH, D3.dependentBirth) [Dependent3BMonth]
            ,DATEPART(DAY, D3.dependentBirth) [Dependent3BDay]
            ,DATEPART(YEAR, D3.dependentBirth) [Dependent3BYear]
            ,D4.dependentName [Dependent4]
            ,DATEPART(MONTH, D4.dependentBirth) [Dependent4BMonth]
            ,DATEPART(DAY, D4.dependentBirth) [Dependent4BDay]
            ,DATEPART(YEAR, D4.dependentBirth) [Dependent4BYear]
            ,CASE WHEN Tah_Schedule IN ('SCHED7.5','SCHED2') THEN ISNULL(Tal_MWEBasicPerDay, 0) 
                ELSE 0 END AS [MinimumWagePerDay]
            ,CASE WHEN Tah_Schedule IN ('SCHED7.5','SCHED2') THEN ISNULL(Tal_MWEBasicPerMonth, 0) 
                ELSE 0 END AS [MinimumWagePerMonth]
            ,CASE WHEN Tah_Schedule IN ('SCHED7.5','SCHED2') THEN 'X'
		        ELSE ''
		        END AS [MinimumWageExemptFromWithholdingTax]
            ,CASE WHEN ISNULL(M_Employer.Mcm_TIN, M_Company.Mcm_TIN) <> 'APPLIED' AND LEN(ISNULL(M_Employer.Mcm_TIN, M_Company.Mcm_TIN) ) IN (9,12) THEN LEFT(REPLACE(ISNULL(M_Employer.Mcm_TIN, M_Company.Mcm_TIN) , '-', ''), 3) ELSE '' END [CompanyTIN]
            ,CASE WHEN ISNULL(M_Employer.Mcm_TIN, M_Company.Mcm_TIN)  <> 'APPLIED' AND LEN(ISNULL(M_Employer.Mcm_TIN, M_Company.Mcm_TIN) ) = 12 THEN RIGHT(LEFT(REPLACE(ISNULL(M_Employer.Mcm_TIN, M_Company.Mcm_TIN) , '-', ''),6),3)
				WHEN ISNULL(M_Employer.Mcm_TIN, M_Company.Mcm_TIN)  <> 'APPLIED' AND LEN(ISNULL(M_Employer.Mcm_TIN, M_Company.Mcm_TIN) ) = 9 THEN LEFT(RIGHT(REPLACE(ISNULL(M_Employer.Mcm_TIN, M_Company.Mcm_TIN) , '-', ''),6),3) ELSE '' END [CompanyTIN2]
            ,CASE WHEN ISNULL(M_Employer.Mcm_TIN, M_Company.Mcm_TIN)  <> 'APPLIED' AND LEN(ISNULL(M_Employer.Mcm_TIN, M_Company.Mcm_TIN) ) = 12 THEN LEFT(RIGHT(REPLACE(ISNULL(M_Employer.Mcm_TIN, M_Company.Mcm_TIN) , '-', ''),6),3) 
				WHEN ISNULL(M_Employer.Mcm_TIN, M_Company.Mcm_TIN)  <> 'APPLIED' AND LEN(ISNULL(M_Employer.Mcm_TIN, M_Company.Mcm_TIN) ) = 9 THEN RIGHT(REPLACE(ISNULL(M_Employer.Mcm_TIN, M_Company.Mcm_TIN) , '-', ''), 3) ELSE '' END  [CompanyTIN3]
            ,CASE WHEN ISNULL(M_Employer.Mcm_TIN, M_Company.Mcm_TIN)  <> 'APPLIED' AND LEN(ISNULL(M_Employer.Mcm_TIN, M_Company.Mcm_TIN) ) = 12 THEN RIGHT(REPLACE(ISNULL(M_Employer.Mcm_TIN, M_Company.Mcm_TIN) , '-', ''), 3) 
				WHEN ISNULL(M_Employer.Mcm_TIN, M_Company.Mcm_TIN)  <> 'APPLIED' AND LEN(ISNULL(M_Employer.Mcm_TIN, M_Company.Mcm_TIN) ) = 9 THEN '000' ELSE '' END  [CompanyTIN4]
	        ,ISNULL(M_Employer.Mcm_EmployerName, M_Company.Mcm_CompanyName) AS [CurEmployerName]
            ,ISNULL(M_Employer.Mcm_BusinessAddress, M_Company.Mcm_BusinessAddress) AS [CurEmployerAddress]
            ,Mlh_ZipCode AS [CurEmployerZipcode]
            ,'X' AS [isMainEmployer]
            ,'' AS [isNotMainEmployer]
            ,Tyt_EmployerName AS [PrevEmployerName]
            ,Tyt_EmployerAddress AS [PrevEmployerAddress]
            ,Tyt_EmployerZipCode AS [PrevEmployerZipcode]
            ,CASE WHEN Tyt_TIN <> 'APPLIED' AND LEN(Tyt_TIN) IN (9,12) THEN LEFT(REPLACE(Tyt_TIN, '-', ''), 3) ELSE '' END [PrevTIN1]
            ,CASE WHEN Tyt_TIN <> 'APPLIED' AND LEN(Tyt_TIN) = 12 THEN RIGHT(LEFT(REPLACE(Tyt_TIN, '-', ''),6),3)
				WHEN Tyt_TIN <> 'APPLIED' AND LEN(Tyt_TIN) = 9 THEN LEFT(RIGHT(REPLACE(Tyt_TIN, '-', ''),6),3) ELSE '' END [PrevTIN2]
            ,CASE WHEN Tyt_TIN <> 'APPLIED' AND LEN(Tyt_TIN) = 12 THEN LEFT(RIGHT(REPLACE(Tyt_TIN, '-', ''),6),3) 
				WHEN Tyt_TIN <> 'APPLIED' AND LEN(Tyt_TIN) = 9 THEN RIGHT(REPLACE(Tyt_TIN, '-', ''), 3) ELSE '' END  [PrevTIN3]
            ,CASE WHEN Tyt_TIN <> 'APPLIED' AND LEN(Tyt_TIN) = 12 THEN RIGHT(REPLACE(Tyt_TIN, '-', ''), 3) 
				WHEN Tyt_TIN <> 'APPLIED' AND LEN(Tyt_TIN) = 9 THEN '000' ELSE '' END  [PrevTIN4]
            ,Tah_MWEBasicCurER AS [BasicSalaryMWE]
            ,Tah_MWEHolidayCurER AS [HolidayPayMWE]
            ,Tah_MWEOvertimeCurER AS [OvertimePayMWE]
            ,Tah_MWEHazardCurER AS [HazardPayMWE]
            ,Tah_MWENightShiftCurER AS [NightShiftMWE]
            ,Tah_Nontaxable13thMonthCurER AS [NT13Month]
            ,Tah_DeMinimisCurER AS[NTDeminimis]
            ,Tah_PremiumsUnionDuesCurER AS [NTSSSGSIS]
            ,Tah_NontaxableSalariesCompensationCurER AS [NTSalariesAndOther]
	        ,Tah_NontaxableIncomeCurER AS [NTTotal]

	        ,Tah_TaxableBasicNetPremiumsCurER AS [BasicSalary]

	        ,Tah_RepresentationCurER AS [Representation]
            ,Tah_TransportationCurER AS [Transportation]
            ,Tah_CostLivingAllowanceCurER AS [CostOfLiving]
            ,Tah_FixedHousingAllowanceCurER AS [FixedHousing]
            ,CASE WHEN Tah_OtherTaxable1CurER > 0
                THEN Tal_OtherTaxableSpecify1
                ELSE ''
                END AS [Others1Desc]
            ,Tah_OtherTaxable1CurER AS [Others1Amt]
            ,CASE WHEN Tah_OtherTaxable2CurER > 0
                THEN Tal_OtherTaxableSpecify2
                ELSE ''
                END AS [Others2Desc]	
            ,Tah_OtherTaxable2CurER AS [Others2Amt]		
            ,Tah_CommisionCurER AS [Commission]
            ,Tah_ProfitSharingCurER AS [ProfitSharing]
            ,Tah_FeesCurER AS [Fees]
            ,CASE WHEN Tah_SupplementaryTaxable1CurER > 0
                THEN Tal_SupplementaryTaxableSpecify1
                ELSE ''
                END AS [Supplement1Desc]
            ,Tah_SupplementaryTaxable1CurER [Supplement1Amt]
            ,CASE WHEN Tah_SupplementaryTaxable2CurER > 0
                THEN Tal_SupplementaryTaxableSpecify2
                ELSE ''
                END AS [Supplement2Desc]
            ,Tah_SupplementaryTaxable2CurER AS [Supplement2Amt]

            ,Tah_HazardCurER AS [THazardPay]
            ,Tah_TaxableOvertimeCurER AS [TOvertimePay]
	        ,Tah_Taxable13thMonthCurER AS [T13Month]
            ,Tah_TaxableIncomeCurER  AS [TTotal]
        
            ,Tah_TaxableIncomeCurER + Tah_NontaxableIncomeCurER AS [CrossCompensationIncome]
    		
            ,Tah_NontaxableIncomeCurER AS [LessTotalNontaxable]
            
            ,Tah_TaxableIncomeCurER AS [TaxableCompensationFromPresEmployer]
    		
	        ,Tah_TaxableIncomePrvER AS	[TaxableCompensationFromPrevEmployer]
            
            ,Tah_TaxableIncomeCurER + Tah_TaxableIncomePrvER AS [GrossTaxableCompensationIncome]
            
	        ,Tah_ExemptionAmount AS [LessTotalExemptions]
	        ,Tah_PremiumPaidOnHealth AS [PremiumPaidOnHealth]
    	
	        ,Tah_NetTaxableIncome AS [NetTaxableCompensationIncome]

	        ,Tah_TaxDue AS [TaxDue]
	        ,Tah_TaxWithheldCurER + Tah_TaxAmount AS [TaxWithheldPres]
	        ,Tah_TaxWithheldPrvER AS [TaxWithheldPrev]
	        ,Tah_TaxDue AS [TotalAmountTaxAdjusted]
	        , CASE WHEN (RTRIM(ISNULL(Tyt_EmployerName, '')) = '' 
			        OR RTRIM(ISNULL(Tyt_EmployerName, '')) = 'NONE')
			        AND Tah_WorkStatus NOT IN ('IN','IM')
		        THEN 'YES'
		        ELSE 'NO'
		        END AS [With58and59] 
            ,Tah_CostcenterCode
	        ,Tct_CommunityTaxNo  AS [CedulaNumber]
	        ,Tct_IssuedAt AS [CedulaIssuePlace]
	        ,CONVERT(VARCHAR,Tct_IssuedDate, 101) AS [CedulaIssueDate]
	        ,Tct_IssuedBy AS [CedulaIssuingOfficer]
	        ,Tct_PaidAmt AS [CedulaAmountPaid]
	        ,Mdv_DivShortName
	        ,Mdp_DptShortName
	        ,Msc_SecShortName
	        ,Mem_Signature
            ,CASE WHEN Tah_IsSubstitutedFiling = 1 THEN 'YES' ELSE 'NO' END AS Tah_IsSubstitutedFiling
        FROM Udv_AlphalistHdr
        INNER JOIN @PROFILES..M_Employee ON Mem_IDNo = Tah_IDNo
            AND Mem_WorkStatus <> 'IM'
        @USERCOSTCENTERACCESSCONDITION
        LEFT JOIN M_CostCenter ON Mem_CostcenterCode = Mcc_CostCenterCode
	        AND Mcc_CompanyCode = '{0}'
        LEFT JOIN M_Division ON Mcc_DivCode = Mdv_DivCode
	        AND Mdv_CompanyCode = Mcc_CompanyCode
        LEFT JOIN M_Department ON Mcc_DptCode = Mdp_DptCode
	        AND Mdp_CompanyCode = Mcc_CompanyCode
        LEFT JOIN M_Section ON Mcc_SecCode = Msc_SecCode
	        AND Msc_CompanyCode = Mcc_CompanyCode
        LEFT JOIN @Dependents D1
	        on D1.EmployeeID = Tah_IDNo
	        and D1.SeqNo = 1
        LEFT JOIN @Dependents D2
	        on D2.EmployeeID = Tah_IDNo
	        and D2.SeqNo = 2
        LEFT JOIN @Dependents D3
	        on D3.EmployeeID = Tah_IDNo
	        and D3.SeqNo = 3
        LEFT JOIN @Dependents D4
	        on D4.EmployeeID = Tah_IDNo
	        and D4.SeqNo = 4
        LEFT JOIN M_Company ON M_Company.Mcm_CompanyCode = '{0}'
        LEFT JOIN M_Employer ON M_Employer.Mcm_CompanyCode = M_Company.Mcm_CompanyCode 
                AND Mcm_EmployerCode = Tah_PayrollGroup
        LEFT JOIN M_LocationHdr ON Mlh_CompanyCode = ISNULL(M_Employer.Mcm_CompanyCode, M_Company.Mcm_CompanyCode)		
              AND Mlh_LocationCode = ISNULL(M_Employer.Mcm_LocationCode, M_Company.Mcm_LocationCode)		
        LEFT JOIN Udv_YearToDate ON Tyt_TaxYear = Tah_TaxYear
	        AND Tyt_IDNo = Tah_IDNo
	        AND Tyt_EmployerType = 'P'
        LEFT JOIN Udv_CommunityTax	ON Tah_TaxYear = Tct_TaxYear
	        AND Tah_IDNo = Tct_IDNo
        LEFT JOIN T_Alphalist ON Tal_TaxYear = Tah_TaxYear AND Tal_CompanyCode = '{0}'
        @CONDITIONS
        ";
        return query;
        }
        #endregion

        #region Check if Year is Hist
        public static string GetTableName(string TaxYear, string CentralProfile)
        {
            return CheckIfTaxYearIsInHistory(TaxYear, CentralProfile) ? " T_EmpAlphalistHdrHst " : " T_EmpAlphalistHdr ";
        }

        public static bool CheckIfTaxYearIsInHistory(string TaxYear, string CentralProfile)
        {
            bool ret = false;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                try
                {
                    dal.OpenDB();
                    DataSet ds = dal.ExecuteDataSet(string.Format(@"
                            SELECT
	                            TOP 1 *
                            FROM T_EmpAlphalistHdrHst
                            WHERE Tah_TaxYear = '{0}'
                    ", TaxYear));
                    if (ds != null
                        && ds.Tables[0].Rows.Count > 0)
                    {
                        ret = true;
                    }
                }
                catch
                {

                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return ret;
        }
        #endregion

    }
}
