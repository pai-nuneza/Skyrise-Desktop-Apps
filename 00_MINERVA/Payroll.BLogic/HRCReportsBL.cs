/* File revision no. D3.2.00004 
 * 
 *  Modules using this BL:
 *          -   DXrptHRCReports.cs
 *          -   frmHRCReports.cs
 *          -   frmHRCPayrollScheduleReport.cs
 * 
 */
using System;
using System.Data;
using Payroll.DAL;
using System.Drawing;
using CommonLibrary;
using System.Configuration;

namespace Payroll.BLogic
{
    public class HRCReportsBL : BaseBL
    {

        #region main overrides 
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

        //#reports
        #region reports
        private string[] reports = new string[13]
        {
            "Other Income and Benefits Report",
            "Deduction and Loan Report",
            "Deduction and Loan Ledger Report",
            "Adjustment Report",
            "Log Report",
            "Assume Present Report",
            "Overtime Report",
            "Leave Availment Report",
            "Leave Ledger Report",
            "DTR Report",
            "Work Location Report",
            "Restday Report",
            "Post-Payroll Report"
        };
        #endregion

        //#menucodes
        #region menucodes
        private string[] menucodes = new string[13]
        {
            "OTHERINCOMEREP",
            "DEDUCTIONREP",
            "DEDUCTNLEDGRREP",
            "ADJUSTMENTLIST",
            "LOGREP",
            "ASSUMEPRESREP",
            "OVERTIMEREP",
            "LEAVEAVAILREP",
            "LEAVELEDGERREP",
            "RAWDTRREP",
            "WORKLOCREP",
            "RESTDAYREP",
            "POSTPAYCALCREP"
        };
        #endregion

        //#systemID 
        #region systemID
        public string[] SystemIDs = new string[13]
        {
            "PAYROLL",
            "PAYROLL",
            "PAYROLL",
            "PAYROLL",
            "TIMEKEEP",
            "TIMEKEEP",
            "OVERTIME",
            "LEAVE",
            "LEAVE",
            "TIMEKEEP",
            "TIMEKEEP",
            "TIMEKEEP",
            "PAYROLL"
        };
        #endregion

        public string PremRemittanceType = string.Empty;

        #region Global Variables
        bool IsNotDefault = true;
        bool ThereSelected = false;
        #endregion

        public string[] ReturnMenuCodes()
        {
            return menucodes;
        }

        public string[] ReturnSystemIDs()
        {
            return SystemIDs;
        }

        public string[] ReturnReports()
        {
            return reports;
        }

        public DataTable GetIncome(string CompanyCode, string CentralProfile)
        {
            string query = string.Format(
                    @"
                    SELECT 
                       Min_IncomeCode  [Code],
                       Min_IncomeName  [Description]
                    FROM M_Income
                    WHERE Min_RecordStatus = 'A'
                    AND Min_CompanyCode = '{0}'
                    ", CompanyCode);

            DataTable dtResult;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public int[] getMargins()
        {
            int[] margs = new int[4]
            {
                50, 50, 50, 50
            };
            return margs;
        }

        public bool GetAccessRights(string user_Logged, string systemID, string database)
        {
            bool ret = false;
            string query = @"
                select 
					*
					from {0}..M_UserDtl
					inner join {0}..M_UserRoleAccess
					on Mud_UserRoleCode = Mra_UserRoleCode
					and Mud_SystemCode = Mra_SystemCode
					where Mud_UserCode = '{1}'
					and Mra_ModuleCode = '{2}'
					and Mra_CanView = 1
					and Mra_CanPrint = 1
                ";
            query = string.Format(query, database, user_Logged, systemID);
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    DataSet ds = dal.ExecuteDataSet(query, CommandType.Text);
                    if (ds.Tables[0].Rows.Count == 1)
                        ret = true;
                    else
                        ret = false;
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

        public bool GetAccessRights2(string user_Logged, string systemID, string profileCode, string companyCode, string CentralProfile)
        {
            bool ret = false;
            string query = @"
                SELECT 
					*
					FROM M_UserDtl
					INNER JOIN M_UserRoleAccess
					ON Mud_UserRoleCode = Mra_UserRoleCode
						AND Mud_SystemCode = Mra_SystemCode
						AND Mud_CompanyCode = Mra_CompanyCode
                        AND Mud_ProfileCode = '{2}'
					WHERE Mud_UserCode = '{0}'
					    AND Mra_ModuleCode = '{1}'
						AND Mra_CompanyCode = '{3}'
					    AND Mra_CanView = 1
					    AND Mra_CanPrint = 1
                ";
            query = string.Format(query, user_Logged, systemID, profileCode, companyCode);
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                try
                {
                    dal.OpenDB();
                    DataSet ds = dal.ExecuteDataSet(query, CommandType.Text);
                    if (ds.Tables[0].Rows.Count > 0)
                        ret = true;
                    else
                        ret = false;
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

        public bool GetAccessRightsCanProcess(string UserLogin, string SystemCode, string CompanyCode, string ProfileCode, string CentralProfile)
        {
            bool ret = false;
            string query = @"
                SELECT 
					*
					FROM M_UserDtl
					INNER JOIN M_UserRoleAccess
					    ON Mud_UserRoleCode = Mra_UserRoleCode
					    AND Mud_SystemCode = Mra_SystemCode
                        AND Mud_CompanyCode = Mra_CompanyCode
                        AND Mud_ProfileCode = '{3}'
					WHERE Mud_UserCode = '{0}'
					    AND Mra_ModuleCode = '{1}'
                        AND Mra_CompanyCode = '{2}'
					    and Mra_CanProcess = 1
                ";
            query = string.Format(query, UserLogin, SystemCode, CompanyCode, ProfileCode);
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                try
                {
                    dal.OpenDB();
                    DataSet ds = dal.ExecuteDataSet(query, CommandType.Text);
                    if (ds.Tables[0].Rows.Count > 0)
                        ret = true;
                    else
                        ret = false;
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


        public bool GetDefaultuserCostCenterAccessRights(string user_Logged, string systemID, string CompanyCode, string ProfileCode, string CentralProfile)
        {
            bool ret = false;

            string query = @"                
                        SELECT 
	                        case when COUNT(Mue_CostCenterCode) > 1
	                        then 'YES'
	                        when COUNT(Mue_CostCenterCode) = 0
	                        then 'NO'
	                        when COUNT(Mue_CostCenterCode) = 1
	                        then 'ALL'
	                        end
	                        FROM M_UserExt
                        WHERE UPPER( Mue_UserCode) = '{0}'
                        AND Mue_SystemCode = '{1}'
                        AND Mue_CompanyCode = '{2}'
                        AND Mue_ProfileCode = '{3}'
                    ";
            query = string.Format(query, user_Logged, systemID, CompanyCode, ProfileCode);
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                try
                {
                    dal.OpenDB();
                    DataSet ds = dal.ExecuteDataSet(query, CommandType.Text);
                    if (ds.Tables[0].Rows[0][0].ToString() == "ALL")
                        ret = true;
                    else
                        ret = false;
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

        public string SetUpItemsForEncodeFilterItems(DataTable dt)
        {
            string s = string.Empty;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (i > 0)
                    s += ",";
                s += dt.Rows[i][0].ToString().Trim();
            }
            return s;
        }

        public Font getReportFont()
        {
            Font font = new Font("Tahoma", 6.75F);
            return font;
        }

        //#table
        public string getTable(string report, bool category)
        {
            string table = string.Empty;
            switch (report)
            {
                case "Other Income and Benefits Report":
                    #region table
                    table = @"
                            [IDNumber] varchar(20)
                            , [Last Name] varchar(30)
                            , [First Name]  varchar(30)
                            , [Costcenter]  varchar(30)
                            , [Description] varchar(90)
                            , [Payperiod] varchar(10)
                            , [Other Income Code]  varchar(30)
                            , [Amount] varchar(20)
                            , [Recursive] varchar(10)
                            , [Post to Payroll]  varchar(5)
                            , [Tax Class]  varchar(10)
                            , [Alphalist Category]   varchar(50)
                            , [Payroll Type]  varchar(50)
                            , [Employment Status] varchar(50)
                            , [Job Status] varchar(50)
                            , [Frequency] varchar(50)
                        ";
                    #endregion
                    break;
                case "Deduction and Loan Report":
                    #region table
                    if (category)
                    {
                        table = @"
                            [IDNumber] varchar(20)
                            , [Last Name] varchar(30)
                            , [First Name] varchar(30)
                            , [Costcenter] varchar(10)
                            , [Description] varchar(100)
                            , [Deduction Code] varchar(30)
                            , [Start of Deduction]  varchar(20)
                            , [Amount] varchar(15)
                            , [From Deferred] varchar(5)
                            , [Post to Payroll] varchar(5)
                            , [With Check] varchar(5)
                            , [Check Date] varchar(15)
                            , [With Voucher] varchar(5)
                            , [Voucher Number]  varchar(30)
                            , [Payroll Type]  varchar(30)
                            , [Employment Status] varchar(50)
                            , [Job Status] varchar(50)
                            , [Deduction Type] varchar(30)
                            , [Payment Type] varchar(30)
                        ";
                    }
                    else
                    {
                        table = @"
                            [IDNumber] varchar(20)
                            , [Last Name] varchar(30)
                            , [First Name] varchar(30)
                            , [Costcenter] varchar(10)
                            , [Description] varchar(100)
                            , [Deduction Code] varchar(30)
							, [Deferred this payperiod] varchar(30)
                            , [Start of Deduction]  varchar(20)
                            , [Amount] varchar(15)
                            , [With Check] varchar(5)
                            , [Check Date] varchar(15)
                            , [With Voucher] varchar(5)
                            , [Voucher Number]  varchar(30)
                            , [Payroll Type]  varchar(30)
                            , [Employment Status] varchar(50)
                            , [Job Status] varchar(50)
                            , [Deduction Type] varchar(30)
                        ";
                    }
                    #endregion
                    break;
                case "Deduction and Loan Ledger Report":
                    #region table
                    table = @"
                            [IDNumber] varchar(20)
                            , [Last Name]  varchar(30)
                            , [First Name] varchar(30)
                            ,[Costcenter] varchar(20)
                            , [Description] varchar(100)
                            , [Deduction Code] varchar(30)
                            , [Start of Deduction] varchar(15)
                            , [Deduction Amount] varchar(20)
                            , [Paid Amount] varchar(20)
                            , [Balance Amount] varchar(20)
                            , [Amortization Amount] varchar(20)
                            , [Deferred Amount] varchar(20)
                            , [With Check] varchar(5)
                            , [Check Date] varchar(15)
                            , [With Voucher] varchar(5)
                            , [Voucher Number] varchar(20)
                            , [Exclude From Payroll] varchar(5)
                            , [FullyPaid Date] varchar(15)
                            , [Payroll Type]  varchar(20)
                            , [Employment Status] varchar(30)
                            , [Job Status] varchar(30)
                            , [Deduction Type] varchar(30)
                        ";
                    #endregion 
                    break;
                case "Adjustment Report":
                    #region table
                    table = @"
                    [IDNumber] varchar(20) 
                    ,[Last Name] varchar(20)
                    , [First Name] varchar(20)
                    , [Costcenter] varchar(15)
                    , [Description] varchar(100)
                    , [This Payperiod] varchar(15)
                    , [Hourly Rate] varchar(15)
                    , [Regular Hour] varchar(15)
                    , [Regular OT Hour] varchar(15)
                    , [Regular ND Hour] varchar(15)
                    , [Regular OT ND Hour] varchar(15)
                    , [Restday Hour] varchar(15)
                    , [Restday OT Hour] varchar(15)
                    , [Restday ND Hour] varchar(15)
                    , [Restday OT ND Hour] varchar(15)
                    , [Legal Holiday Hour] varchar(15)
                    , [Legal Holiday OT Hour] varchar(15)
                    , [Legal Holiday ND Hour] varchar(15)
                    , [Legal Holiday OT ND Hour] varchar(15)
                    , [Special Holiday Hour] varchar(15)
                    , [Special Holiday OT Hour] varchar(15)
                    , [Special Holiday ND Hour] varchar(15)
                    , [Special Holiday OT ND Hour] varchar(15)
                    , [Plant Shutdown Hour] varchar(15)
                    , [Plant Shutdown OT Hour] varchar(15)
                    , [Plant Shutdown ND Hour] varchar(15)
                    , [Plant Shutdown OT ND Hour] varchar(15)
                    , [Company Holiday Hour] varchar(15)
                    , [Company Holiday OT Hour] varchar(15)
                    , [Company Holiday ND Hour] varchar(15)
                    , [Company Holiday OT ND Hour] varchar(15)
                    , [Restday Legal Holiday Hour] varchar(15)
                    , [Restday Legal Holiday OT Hour] varchar(15)
                    , [Restday Legal Holiday ND Hour] varchar(15)
                    , [Restday Legal Holiday OT ND Hour] varchar(15)
                    , [Restday Special Holiday Hour] varchar(15)
                    , [Restday Special Holiday OT Hour] varchar(15)
                    , [Restday Special Holiday ND Hour] varchar(15)
                    , [Restday SpecialHoliday OT ND  Hour] varchar(15)
                    , [Restday Company Holiday Hour] varchar(15)
                    , [Restday Company Holiday OT Hour] varchar(15)
                    , [Restday Company Holiday ND Hour] varchar(15)
                    , [Restday Company Holiday OT ND Hour] varchar(15)
                    , [Restday Plant Shutdown Hour] varchar(15)
                    , [Restday Plant Shutdown OT Hour] varchar(15)
                    , [Restday Plant Shutdown NDHour] varchar(15)
                    , [Restday Plant Shutdown OT ND  Hour] varchar(15)
                    , [Filler01 Hour] varchar(15)
                    , [Filler01 OT Hour] varchar(15)
                    , [Filler01 ND Hour] varchar(15)
                    , [Filler01 OT ND Hour] varchar(15)
                    , [Filler02 Hour] varchar(15)
                    , [Filler02 OT Hour] varchar(15)
                    , [Filler02 ND Hour] varchar(15)
                    , [Filler02 OT ND Hour] varchar(15)
                    , [Filler03 Hour] varchar(15)
                    , [Filler03 OT Hour] varchar(15)
                    , [Filler03 ND Hour] varchar(15)
                    , [Filler03 OT ND Hour] varchar(15)
                    , [Filler04 Hour] varchar(15)
                    , [Filler04 OT Hour] varchar(15)
                    , [Filler04 ND Hour] varchar(15)
                    , [Filler04 OT ND Hour] varchar(15)
                    , [Filler05 Hour] varchar(15)
                    , [Filler05 OT Hour] varchar(15)
                    , [Filler05 ND Hour] varchar(15)
                    , [Filler05 OT ND Hour] varchar(15)
                    , [Filler06 Hour] varchar(15)
                    , [Filler06 OT Hour] varchar(15)
                    , [Filler06 ND Hour] varchar(15)
                    , [Filler06 OT ND Hour] varchar(15)
                    , [Labor Hrs Adjustment Amount] varchar(15)
                    , [Tax Adjustment Amount] varchar(15)
                    , [Nontax Adjustment Amount] varchar(15)
                    , [Post to Payroll] varchar(15)
                    , [Payroll Type] varchar(30) 
                    , [Employment Status] varchar(30)
                    , [Job Status] varchar(30)
                    ";
                    #endregion
                    break;
                case "Log Report":
                    #region table
                    table = @"
                    [IDNumber] varchar(20)
                    , [Last Name] varchar(20)
                    , [First Name] varchar(20)
                    , [Costcenter] varchar(20)
                    , [Description] varchar(100)
                    , [This Period] varchar(20)
                    , [Log Date] varchar(20)
                    , [Day Code] varchar(20)
                    , [Shift Code] varchar(20)
                    , [Actual Time In 1] varchar(10)
                    , [Actual Time Out 1] varchar(10)
                    , [Actual Time In 2] varchar(10)
                    , [Actual Time Out 2] varchar(10)
                    , [Shift Time In] varchar(10)
                    , [Start of Break] varchar(10)
                    , [End of Break] varchar(10)
                    , [Shift Time Out] varchar(10)
                    , [Minutes Log IN 1 before shift] varchar(10)
                    , [Minutes Log OUT 1 after shift] varchar(10)
                    , [Minutes Log IN 2 before shift] varchar(10)
                    , [Minutes Log OUT 2 after shift] varchar(10)
                    , [Absent Hour] varchar(10)
                    , [Regular Hour] varchar(10)
                    , [Overtime Hour] varchar(10)
                    , [Regular Night Prem Hour] varchar(10)
                    , [Overtime Night Prem Hour] varchar(10)
                    , [Leave Hour] varchar(10)
                    , [Work Type] varchar(20)
		            , [Work Group] varchar(20)
		            , [Location] varchar(30)
                    , [Payroll Type]  varchar(10)
                    , [Employment Status] varchar(30)
                    , [Job Status] varchar(30)
                    ";
                    #endregion
                    break;
                case "Assume Present Report":
                    #region table
                    table = @"
                    [IDNumber] varchar(20)
		            , [Last Name] varchar(20)
		            , [First Name] varchar(20)
		            , [Costcenter] varchar(20)
		            , [Description] varchar(100)
		            , [This Period] varchar(20)
		            , [Log Date] varchar(20)
		            , [Day Code] varchar(20)
		            , [Shift Code] varchar(20)
		            , [Actual Time In 1] varchar(20)
		            , [Actual Time Out 1] varchar(20)
		            , [Actual Time In 2] varchar(20)
		            , [Actual Time Out 2] varchar(20)
		            , [Shift Time In] varchar(20)
		            , [Start of Break] varchar(20)
		            , [End of Break] varchar(20)
		            , [Shift Time Out] varchar(20)
                    , [Work Type] varchar(20)
		            , [Work Group] varchar(20)
		            , [Location] varchar(30)
		            , [Payroll Type]  varchar(20)
		            , [Employment Status] varchar(30)
		            , [Job Status] varchar(30)
		            , [Remarks] varchar(20)
                    ";
                    #endregion
                    break;
                case "Overtime Report":
                    #region table
                    table = @"
                    [ID Number] varchar(30)
		            , [Last Name] varchar(30)
		            , [First Name] varchar(30)
		            , [Costcenter] varchar(30)
		            , [Description] varchar(150)
		            , [This Payperiod] varchar(30)
                    , [Overtime Date] varchar(30)
		            , [Applied Date] varchar(30)
		            , [Overtime Type] varchar(30)
		            , [Start Time] varchar(30)
		            , [End Time] varchar(30)
		            , [Overtime Hours] varchar(30)
		            , [Reason of Overtime] varchar(100)
		            , [Endorse Date to Checker] varchar(30)
		            , [Checker 1] varchar(30)
		            , [Checker 1 Name] varchar(100)
		            , [Check Date 1] varchar(30)
		            , [Checker 2] varchar(30)
		            , [Checker 2 Name] varchar(100)
		            , [Check Date 2] varchar(30)
		            , [Approver] varchar(30)
		            , [Approver Name]   varchar(100)
		            , [Approve Date] varchar(30)
		            , [Status] varchar(30)
		            , [Transaction Control No.] varchar(30)
		            , [Batch Control No.] varchar(30)
		            , [Overtime Flag] varchar(30)
		            , [Payroll Type]  varchar(30)
		            , [Employment Status] varchar(50)
		            , [Job Status] varchar(50)
                    ";
                    #endregion
                    break;
                case "Leave Availment Report":
                    #region table
                    table = @"
               
                     [ID Number] varchar(30)
		            ,[Last Name] varchar(30)
		            ,[First Name] varchar(30)
		            ,[Costcenter] varchar(30)
		            ,[Description] varchar(100)
		            ,[This Payperiod] varchar(30)
		            , [Leave Date] varchar(30)
		            , [Inform Date] varchar(30)
		            , [Applied Date] varchar(30)
		            , [Leave Type] varchar(30)
		            , [Leave Category] varchar(30)
		            , [Leave Code] varchar(30)
		            , [Start Time] varchar(10)
		            , [End Time] varchar(10)
		            , [Leave Hours] varchar(10)
		            , [Day Unit] varchar(30)
		            , [With Leave Notice] varchar(10)
		            , [Reason of Leave] varchar(150)
		            , [Endorse Date to Checker] varchar(30)
		            , [Checker 1] varchar(30)
		            , [Checker 1 Name] varchar(50)
		            , [Check Date 1] varchar(50)
		            , [Checker 2] varchar(30)
		            , [Checker 2 Name] varchar(50)
		            , [Check Date 2] varchar(30)
		            , [Approver] varchar(30)
		            , [Approver Name]   varchar(50)
		            , [Approve Date] varchar(30)
		            , [Status] varchar(30)
		            , [Transaction Control No.] varchar(30)
		            , [Overtime Flag] varchar(30)
		            , [Payroll Type]  varchar(30)
		            , [Employment Status] varchar(50)
		            , [Job Status] varchar(50)
                    ";
                    #endregion
                    break;
                case "Leave Ledger Report":
                    #region table
                    table = @"
                    [ID Number] varchar(30)
				    , [Last Name] varchar(30)
				    , [First Name] varchar(30)
				    , [Costcenter] varchar(30)
				    , [Description] varchar(200)
				    , [Leave Year] varchar(30)
				    , [Leave Type] varchar(30)
				    , [Entitled] varchar(30)
				    , [Used] varchar(30)
				    , [Reserved] varchar(30)
				    , [Balance] varchar(30)
				    , [Payroll Type]  varchar(50)
				    , [Employment Status] varchar(50)
				    , [Job Status] varchar(50)
                    ";
                    #endregion
                    break;
                case "DTR Report":
                    #region table
                    table = @"
                    [ID Number] varchar(20)
                    , [Last Name] varchar(30)
                    , [First Name] varchar(30)
                    , [Log Date] varchar(30)
                    , [Log Time] varchar(30)
                    , [Log Type] varchar(5)
                    , [Station Number] varchar(30)
                    , [Payroll Type] varchar(50)
                    , [Employment Status] varchar(50)
                    , [Job Status] varchar(50)
                    ";
                    #endregion
                    break;
                case "Work Location Report":
                    #region table
                    table = @"
	                [IDNumber] varchar(20)
	                , [Last Name] varchar(20)
	                , [First Name] varchar(20)
	                , [Costcenter] varchar(20)
	                , [Description] varchar(150)
	                , [Effectivity Date] varchar(20)
	                , [Work Location] varchar(150)
	                , [Payroll Type]  varchar(50)
	                , [Employment Status] varchar(50)
	                , [Job Status] varchar(50)
                    ";
                    #endregion
                    break;
                case "Restday Report":
                    #region table
                    table = @"
                    [IDNumber] varchar(20)
                    , [Last Name] varchar(30)
                    , [First Name] varchar(30)
                    , [Costcenter] varchar(30)
                    , [Description] varchar(150)
                    , [Effectivity Date] varchar(30)
                    , [Rest Day] varchar(50)
                    , [Payroll Type]  varchar(30)
                    , [Employment Status] varchar(50)
                    , [Job Status] varchar(50)
                    ";
                    #endregion
                    break;
                case "Post-Payroll Report":
                    #region table 
                    table = @"
                    [IDNumber] varchar(20)
                    , [Last Name] varchar(30)
                    , [First Name] varchar(30)
                    , [Costcenter] varchar(20)
                    , [Description] varchar(150)
                    , [This Period] varchar(20)
                    , [Remarks] varchar(50)
                    , [For Adjustment]  varchar(20)
                    , [Payroll Type]  varchar(50)
                    , [Employment Status] varchar(50)
                    , [Job Status] varchar(50)
                    ";
                    #endregion
                    break;
                case "Tardiness/Undertime/Absences Report":
                    #region table
                    table = @"
                      [ID Number] varchar(50)
                    , [Last Name] varchar(50)
                    , [First Name] varchar(50)
                    , [Middle Name] varchar(50)
                    , [Date] varchar(50)
                    , [Absent] varchar(50)
                    , [Last User Modified] varchar(50)
                    , [Last Modified Date] varchar(50)
                    ";
                    #endregion
                    break;
                case "Loan Collection Report":
                    #region table
                    table = @"
                              [Year & Month] varchar(50)
                            , [Deduction Code] varchar(50)
                            , [ID Number] varchar(50)
                            , [Last Name] varchar(50)
                            , [First Name] varchar(50)
                            , [Middle Name] varchar(50)
                            , [Maiden Name] varchar(50)
                            , [Amount] varchar(50)
                            , [Check Date] varchar(50)
                            , [Loan Amount] varchar(50)
                            , [Principal Amount] varchar(50)
                            , [Status Code] varchar(50)
                            , [Status Date] varchar(50)
                            , [Last User Modified] varchar(50)
                            , [Last Modified Date] varchar(50)
                            , [SSS Number] varchar(50)
                            , [Philhealth Number] varchar(50)
                            , [Pag-ibig Number] varchar(50)
                            , [JobStatus] varchar(50)
                            , [Payroll Type] varchar(50)
                            , [Employment Status] varchar(50)
                            , [Gender] varchar(50)
                            , [Costcenter] varchar(50)
                            , [Description] varchar(150)
                    ";
                    #endregion
                    break;
                case "13th Month Report":
                    #region table
                    table = @"
                              [Employee ID]         varchar(50) 
                            , [Last Name]           varchar(50) 
                            , [First Name]          varchar(50) 
                            , [Middle Name]         varchar(50) 
                            , [Maiden Name]         varchar(50) 
                            , [Pay Period]          varchar(10) 
                            , [Salary Rate]         decimal(18,2)
                            , [Payroll Type]        varchar(20) 
                            , [Regular Pay]         decimal(18,2) 
                            , [Adjustment Amount]   decimal(18,2) 
                            , [Bonus Amount]        decimal(18,2) 
                            , [Used Nontax Amount]  decimal(18,2) 
                            , [Nontax Amount]       decimal(18,2) 
                            , [Tax Amount]          decimal(18,2) 
                            , [Post to Payroll]     varchar(10) 
                            , [Costcenter]          varchar(50) 
                            , [Description]         varchar(200) 
                            , [Actual Days]         varchar(50) 
                            , [Last User Modified]  varchar(50) 
                            , [Last Modified Date] varchar(50) 
                            , [Job Status]          varchar(50) 
                            , [Employment Status]   varchar(50) 
                            , [Gender]              varchar(10) 
                    ";
                    #endregion
                    break;
                case "Premium Remittance Report":
                    #region table
                    table = @"
                              [Year & Month] varchar(50)
                            , [Deduction Code] varchar(50)
                            , [ID Number] varchar(50)
                            , [Last Name] varchar(50)
                            , [First Name] varchar(50)
                            , [Middle Name] varchar(50)
                            , [Maiden Name] varchar(50)
                            , [Employer Share] varchar(50)
                            , [Employee Share] varchar(50)
                            , [Salary Credit] varchar(50)
                            , [Status Code] varchar(50)
                            , [Status Date] varchar(50)
                            , [Last User Modified] varchar(50)
                            , [Last Modified Date] varchar(50)
                            , [SSS Number] varchar(50)
                            , [Philhealth Number] varchar(50)
                            , [Pag-ibig Number] varchar(50)
                            , [TIN] varchar(50)
                            , [JobStatus] varchar(50)
                            , [Payroll Type] varchar(50)
                            , [Employment Status] varchar(50)
                            , [Gender] varchar(50)
                            , [Costcenter] varchar(50)
                            , [Description] varchar(150)
                    ";
                    #endregion
                    break;
                case "Withholding Tax Report":
                    #region table
                    table = @"
                              [Employee ID] varchar(150)
                            , [Last Name] varchar(150)
                            , [First Name] varchar(150)
                            , [Middle Name] varchar(150)
                            , [Costcenter] varchar(150)
                            , [Description] varchar(150)
                            , [TIN] varchar(150)
                            , [Payperiod] varchar(150)
                            , [Tax Code] varchar(150)
                            , [Witholding Tax Amount] varchar(150)
                    ";
                    #endregion
                    break;
            }
            return table;

        }

        public string[] getDefaultColumns(string report, bool category)
        {
            string[] table = new string[0];
            switch (report)
            {
                case "Other Income and Benefits Report":
                    #region table
                    table = new string[4] {
                        "IDNumber"
                        ,"Last Name"
                        ,"First Name"
                        ,"Payperiod"
                    };
                    #endregion
                    break;
                case "Premium Remittance Report":
                    #region table
                    if (category)
                    {
                        if (PremRemittanceType != string.Empty
                            && PremRemittanceType != "ALL")
                        {
                            if (PremRemittanceType.IndexOf("HDMF") != -1)
                            {
                                table = new string[9] {
                                "ID Number"
                                ,"Last Name"
                                ,"First Name"
                                ,"Middle Name"
                                ,"Pag-ibig Number"
                                ,"Employer Share"
                                ,"Employee Share"
                                ,"Costcenter"
                                ,"Description"

                            };
                            }
                            else if (PremRemittanceType.IndexOf("PAGIBIG") != -1)
                            {
                                table = new string[7] {
                                "ID Number"
                                ,"Last Name"
                                ,"First Name"
                                ,"Middle Name"
                                ,"Pag-ibig Number"
                                ,"Costcenter"
                                ,"Description"
                            };
                            }
                            else if (PremRemittanceType.IndexOf("SSS") != -1)
                            {
                                table = new string[9] {
                                "ID Number"
                                ,"Last Name"
                                ,"First Name"
                                ,"Middle Name"
                                ,"SSS Number"
                                ,"Employer Share"
                                ,"Employee Share"
                                ,"Costcenter"
                                ,"Description"
                            };
                            }
                            else if (PremRemittanceType.IndexOf("PHICPREM") != -1)
                            {
                                table = new string[9] {
                                "ID Number"
                                ,"Last Name"
                                ,"First Name"
                                ,"Middle Name"
                                ,"Philhealth Number"
                                ,"Employer Share"
                                ,"Employee Share"
                                ,"Costcenter"
                                ,"Description"
                            };
                            }
                            else
                            {
                                table = new string[12] {
                                "Year & Month"
                                ,"Deduction Code"
                                ,"ID Number"
                                ,"Last Name"
                                ,"SSS Number"
                                ,"Philhealth Number"
                                ,"Pag-ibig Number"
                                ,"TIN"
                                ,"Employer Share"
                                ,"Employee Share"
                                ,"Costcenter"
                                ,"Description"
                            };
                            }
                        }
                        else
                        {
                            table = new string[12] {
                            "Year & Month"
                            ,"Deduction Code"
                            ,"ID Number"
                            ,"Last Name"
                            ,"SSS Number"
                            ,"Philhealth Number"
                            ,"Pag-ibig Number"
                            ,"TIN"
                            ,"Employer Share"
                            ,"Employee Share"
                            ,"Costcenter"
                            ,"Description"
                            };
                        }
                    }
                    else
                    {
                        if (PremRemittanceType != string.Empty
                            && PremRemittanceType != "ALL")
                        {
                            if (PremRemittanceType.IndexOf("HDMF") != -1)
                            {
                                table = new string[7] {
                                "ID Number"
                                ,"Last Name"
                                ,"First Name"
                                ,"Middle Name"
                                ,"Pag-ibig Number"
                                ,"Employer Share"
                                ,"Employee Share"
                            };
                            }
                            else if (PremRemittanceType.IndexOf("PAGIBIG") != -1)
                            {
                                table = new string[5] {
                                "ID Number"
                                ,"Last Name"
                                ,"First Name"
                                ,"Middle Name"
                                ,"Pag-ibig Number"
                            };
                            }
                            else if (PremRemittanceType.IndexOf("SSS") != -1)
                            {
                                table = new string[7] {
                                "ID Number"
                                ,"Last Name"
                                ,"First Name"
                                ,"Middle Name"
                                ,"SSS Number"
                                ,"Employer Share"
                                ,"Employee Share"
                            };
                            }
                            else if (PremRemittanceType.IndexOf("PHICPREM") != -1)
                            {
                                table = new string[7] {
                                "ID Number"
                                ,"Last Name"
                                ,"First Name"
                                ,"Middle Name"
                                ,"Philhealth Number"
                                ,"Employer Share"
                                ,"Employee Share"
                            };
                            }
                            else
                            {
                                table = new string[10] {
                                "Year & Month"
                                ,"Deduction Code"
                                ,"ID Number"
                                ,"Last Name"
                                ,"SSS Number"
                                ,"Philhealth Number"
                                ,"Pag-ibig Number"
                                ,"TIN"
                                ,"Employer Share"
                                ,"Employee Share"
                            };
                            }
                        }
                        else
                        {
                            table = new string[10] {
                            "Year & Month"
                            ,"Deduction Code"
                            ,"ID Number"
                            ,"Last Name"
                            ,"SSS Number"
                            ,"Philhealth Number"
                            ,"Pag-ibig Number"
                            ,"TIN"
                            ,"Employer Share"
                            ,"Employee Share"
                        };
                        }
                    }
                    #endregion
                    break;
                case "Loan Collection Report":
                    #region table
                    if (category)
                    {
                        if (PremRemittanceType != string.Empty
                           && PremRemittanceType != "ALL")
                        {
                            if (PremRemittanceType.IndexOf("PAGIBIG") != -1)
                            {
                                table = new string[7] {
                                "ID Number"
                                ,"Last Name"
                                ,"First Name"
                                ,"Middle Name"
                                ,"Pag-ibig Number"
                                ,"Costcenter"
                                ,"Description"
                            };
                            }
                            else if (PremRemittanceType.IndexOf("SSSLOAN") != -1)
                            {
                                table = new string[8] {
                                "ID Number"
                                ,"Last Name"
                                ,"First Name"
                                ,"Middle Name"
                                ,"SSS Number"
                                ,"Loan Amount"
                                ,"Costcenter"
                                ,"Description"
                            };
                            }
                            else
                            {
                                table = new string[13] {
                                "Year & Month"
                                ,"Deduction Code"
                                ,"ID Number"
                                ,"Last Name"
                                ,"SSS Number"
                                ,"Philhealth Number"
                                ,"Pag-ibig Number"
                                ,"TIN"
                                ,"Check Date"
                                ,"Loan Amount"
                                ,"Principal Amount"
                                ,"Costcenter"
                                ,"Description"
                            };
                            }
                        }
                        else
                        {
                            table = new string[13] {
                            "Year & Month"
                            ,"Deduction Code"
                            ,"ID Number"
                            ,"Last Name"
                            ,"SSS Number"
                            ,"Philhealth Number"
                            ,"Pag-ibig Number"
                            ,"TIN"
                            ,"Check Date"
                            ,"Loan Amount"
                            ,"Principal Amount"
                            ,"Costcenter"
                            ,"Description"
                        };
                        }
                    }
                    else
                    {
                        if (PremRemittanceType != string.Empty
                            && PremRemittanceType != "ALL")
                        {
                            if (PremRemittanceType.IndexOf("PAGIBIG") != -1)
                            {
                                table = new string[5] {
                                "ID Number"
                                ,"Last Name"
                                ,"First Name"
                                ,"Middle Name"
                                ,"Pag-ibig Number"
                            };
                            }
                            else if (PremRemittanceType.IndexOf("SSSLOAN") != -1)
                            {
                                table = new string[6] {
                                "ID Number"
                                ,"Last Name"
                                ,"First Name"
                                ,"Middle Name"
                                ,"SSS Number"
                                ,"Loan Amount"
                            };
                            }
                            else
                            {
                                table = new string[11] {
                                "Year & Month"
                                ,"Deduction Code"
                                ,"ID Number"
                                ,"Last Name"
                                ,"SSS Number"
                                ,"Philhealth Number"
                                ,"Pag-ibig Number"
                                ,"TIN"
                                ,"Check Date"
                                ,"Loan Amount"
                                ,"Principal Amount"
                            };
                            }
                        }
                        else
                        {
                            table = new string[11] {
                            "Year & Month"
                            ,"Deduction Code"
                            ,"ID Number"
                            ,"Last Name"
                            ,"SSS Number"
                            ,"Philhealth Number"
                            ,"Pag-ibig Number"
                            ,"TIN"
                            ,"Check Date"
                            ,"Loan Amount"
                            ,"Principal Amount"
                        };
                        }
                    }
                    #endregion
                    break;
                case "Withholding Tax Report":
                    #region table
                    table = new string[9] {
                        "Employee ID"
                        ,"Last Name"
                        ,"First Name"
                        ,"Payperiod"
                        ,"TIN"
                        ,"Tax Code"
                        ,"Witholding Tax Amount"
                        ,"Costcenter"
                        ,"Description"
                    };
                    #endregion
                    break;
            }
            return table;
        }

        public string UserCostCenterAccessTmpQuery(string profile, string systemID, string userCode, string costCenterColumn, string payrollGroupColumn, string empStatusColumn, string payrollType, string companyCode, string CentralProfile, bool reportType)
        {
            string query = string.Format(@"
                    INNER JOIN 
                    {0}..M_UserExtTmp
                    ON Mue_SystemCode = '{1}'
                    AND Mue_UserCode = '{2}'
                    AND Mue_CompanyCode = '{3}'
                    AND Mue_ProfileCode = '{4}'
                    AND Mue_CostCenterCode = {5}
                    AND Mue_PayrollGroup = {6}
                    AND Mue_PayrollType = {7}
                    AND Mue_EmploymentStatus = {8}"
                , CentralProfile
                , systemID, userCode, companyCode, profile, costCenterColumn, payrollGroupColumn, payrollType, empStatusColumn);
            if (reportType)
            {
                query = query.Replace("'", "''");
            }

            return query;
        }

        public string GetPayrollCalcTable(bool isSepTable, string strPayPeriod)
        {
            string strPayCalcTable = "T_EmpPayroll";
            DALHelper dal = new DALHelper();
            DataSet ds = dal.ExecuteDataSet(
                            string.Format(@"
                            select Tps_CycleIndicator, Tps_CycleIndicatorSpecial from T_PaySchedule
                            where Tps_PayCycle = '{0}'", strPayPeriod));
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[0].Rows[0][0].ToString().Trim() == "C"
                    || ds.Tables[0].Rows[0][1].ToString().Trim() == "C")
                {
                    if (isSepTable == true)
                        strPayCalcTable = "T_EmpPayrollFinalPay";
                    else
                        strPayCalcTable = "T_EmpPayroll";
                }
                else
                {
                    if (isSepTable == true)
                        strPayCalcTable = "T_EmpPayrollFinalPayHst";
                    else
                    {
                        DataSet ds2 = dal.ExecuteDataSet(
                                    string.Format(@"
                                select Tps_PayCycle from T_PaySchedule
                                where Tps_CycleIndicator = 'C'"));
                        if (ds2 != null && ds2.Tables[0].Rows.Count > 0
                            && ds2.Tables[0].Rows[0][0].ToString().Remove(4) == strPayPeriod.Substring(0, 4))
                        {
                            strPayCalcTable = "T_EmpPayrollYearly"; //Within the same year as the current
                        }
                        else
                        {
                            strPayCalcTable = "T_EmpPayrollHst"; //Not the same year with the current
                        }
                    }
                }
            }

            return strPayCalcTable;
        }

        //#query


        public DataTable GetEmploymentStatus(bool accessRights, string userlogged, string costcenters, string SystemID)
        {

            string cond = string.Empty;

            string accessQuery = @"
                    select
                       distinct Mue_EmploymentStatus
                    from M_UserExt
                    where Mue_UserCode = '{0}'
                    {1}
                    and Mue_SystemCode = '{2}'
                    ";

            string query = @"
               		 SELECT [Mcd_Code] AS 'Status Code'
                                   ,[Mcd_Name] AS 'Description'
                              FROM  [M_CodeDtl]
                             WHERE  [Mcd_RecordStatus] = 'A'
                               AND  [Mcd_CodeType] = 'EMPSTAT'
                    {0}
                    ";

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                if (!accessRights)
                {
                    string cost = "";
                    if (costcenters != string.Empty)
                        cost = "and Mue_CostCenterCode in ( " + EncodeFilterItems(costcenters) + ")";
                    accessQuery = string.Format(accessQuery, userlogged, cost, SystemID);
                    DataTable dt = dal.ExecuteDataSet(accessQuery).Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        cond = "And Mcd_Code in (" + EncodeFilterItems(SetUpItemsForEncodeFilterItems(dt)) + ")";
                    }
                    else
                    {
                        cond = "And 1 = 0";
                    }
                }
                query = string.Format(query, cond);
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;

        }

        public DataTable GetCostCenters2(string userlogged, string SystemID, string CompanyCode, string ProfileCode, string CentralProfile)
        {
            string accessQuery = string.Format(@"
                    SELECT
                       DISTINCT Mue_CostCenterCode
                    FROM M_UserExt
                    WHERE Mue_UserCode = '{0}'
                        AND Mue_SystemCode = '{1}'
                        AND Mue_CompanyCode = '{2}'
                        AND Mue_ProfileCode = '{3}'"
                    , userlogged
                    , SystemID
                    , CompanyCode
                    , ProfileCode);

            string cond = string.Empty;

            string query = @"
               		 SELECT 
						Mcc_CostCenterCode [Code],
						dbo.Udf_DisplayCostCenterName('{1}', Mcc_CostCenterCode,'{2}') [Description]
					FROM M_CostCenter
					WHERE Mcc_RecordStatus = 'A'
                    AND Mcc_CompanyCode = '{1}'
                    {0}
                    ";

            DataTable dtResult;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                DataTable dt = dal.ExecuteDataSet(accessQuery).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0][0].ToString() != "ALL")
                        cond = "AND Mcc_CostCenterCode IN (" + EncodeFilterItems(SetUpItemsForEncodeFilterItems(dt)) + ")";
                    query = string.Format(query, cond, CompanyCode, (new CommonBL()).GetParameterValueFromCentral("CCTRDSPLY", CompanyCode, CentralProfile));
                    dtResult = dal.ExecuteDataSet(query).Tables[0];
                }
                else
                {
                    dtResult = new DataTable();
                }
                dal.CloseDB();
            }
            return dtResult;

        }

        public DataTable GetPayrollType()
        {
            string query = @"
                    select 
                        Mcd_Code [Code],
                        Mcd_Name [Description]	 
                    from M_CodeDtl
                    where Mcd_CodeType = 'PAYTYPE'
                                  AND Mcd_RecordStatus = 'A'
                    ";

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetRankLevel()
        {
            string query = @"
                    select 
                        Mcd_Code [Code],
                        Mcd_Name [Description]	 
                    from M_CodeDtl
                    where Mcd_CodeType = 'RANKLEVEL'
                                  AND Mcd_RecordStatus = 'A'
                    ";

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetPayPeriod2()
        {
            string query = @"
               		 select 
						Tps_PayCycle [Payroll Period],
						CONVERT(varchar(15), Tps_StartCycle, 101) + ' - ' + CONVERT(varchar(15), Tps_EndCycle, 101) [Cycle]
					from T_PaySchedule
					where Tps_CycleIndicator in ('P', 'C')
					order by Tps_CycleIndicator asc
                    ";

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;

        }

        public DataTable GetDeductionCodes(string CompanyCode, string CentralProfile)
        {
            string query = string.Format(
                    @"
                    SELECT 
	                    Mdn_DeductionCode [Deduction Code],
	                    Mdn_DeductionName [Description]
                    FROM M_Deduction
                    WHERE Mdn_RecordStatus = 'A'
                    AND Mdn_CompanyCode = '{0}'
                    ", CompanyCode);

            DataTable dtResult;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public string GetAllowedCostCeneterForUser(string userlogged, string systemID)
        {
            string ret = string.Empty;
            string query = @"
                        select 
                            distinct Mue_CostCenterCode
                        from M_UserExt
                        where Mue_SystemCode = '{0}'
                        and Mue_UserCode = '{1}' 
                    ";
            query = string.Format(query, systemID, userlogged);

            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    DataSet ds = dal.ExecuteDataSet(query, CommandType.Text);
                    ret = EncodeFilterItems(SetUpItemsForEncodeFilterItems(ds.Tables[0]));
                    if (ret == string.Empty)
                        ret = "''";
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

        public string GetAllowedQueryForUser(string userlogged, string costCenter, string column, string SystemID)
        {
            string ret = string.Empty;
            string query = @"
                        select 
                            distinct {3}
                        from M_UserExt
                        where Mue_SystemCode = '{0}'
                        and Mue_UserCode = '{1}'
                        and Mue_CostCenterCode in ({2})
                ";

            query = string.Format(query, SystemID, userlogged, costCenter, column);

            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    DataSet ds = dal.ExecuteDataSet(query, CommandType.Text);
                    ret = EncodeFilterItems(SetUpItemsForEncodeFilterItems(ds.Tables[0]));
                    if (ret == string.Empty)
                        ret = "''";
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

        public string[] GetColumnsNotIncludedInSummary()
        {
            string[] cols = new string[82] {
                "Profiles",
                "ID Number",
                "IDNumber",
                "Employee Name",
                "Costcenter",
                "Tax Code",
                "Description",
                "Payperiod",
                "PayPeriod",
                "ThisPayperiod",
                "This Payperiod",
                "ThisPeriod",
                "LeaveYear",
                "Location",
                "ActualTimeIn1",
                "ActualTimeIn2",
                "ActualTimeOut1",
                "ActualTimeOut2",
                "MinutesLogIN1beforeshift",
                "MinutesLogIN2beforeshift",
                "MinutesLogOUT1aftershift",
                "MinutesLogOUT2aftershift",
                "StartTime",
                "EndTime",
                "Checker1",
                "Checker2",
                "Approver",
                "OvertimeFlag",
                "LeaveFlag",
                "LeaveCategory",
                "LeaveCode",
                "WorkLocation",
                "RestDay",
                "Remarks",
                "Costcenter",
                "Description",
                "Fullname",
                "FullName",
                "Full Name",
                "PayrollType",
                "EmploymentStatus",
                "Daycode",
                "WorkType",

                "LogDate",
                "WorkGroup",
                "DayCode",
                "ShiftCode",

                "LogType",
                "JobStatus",

                "Division",
                "Department",
                "Section",
                "Profilename",

                "DeductionType",
                "VoucherNumber",
                "DeductionCode",

                "PaymentType",
                "StartofDeduction",

                "Year & Month",
                "Deduction Code",
                "Last Name",
                "First Name",
                "Middle Name",
                "Maiden Name",
                "Check Date",
                "Status Code",
                "Status Date",
                "Last User Modified",
                "Last Modified Date",
                "SSS Number",
                "Philhealth Number",
                "Pag-ibig Number",
                "Job Status",
                "Payroll Type",
                "Employment Status",
                "Gender",

                "Employee ID",
                "TIN",

                "Post to Payroll", //added by Christian Aloba - for Adjustment Report
                "Process Date",
                "User Generated",
                "Rank/Level"
            };
            return cols;
        }

        public DataTable GetEmploymentStatus(string userlogged, string SystemID, string CompanyCode, string ProfileCode, string CentralProfile)
        {
            string cond = string.Empty;

            string accessQuery = string.Format(@"
                    SELECT
	                    DISTINCT
	                    Mue_EmploymentStatus
                    FROM M_UserExt
                    WHERE Mue_UserCode = '{0}'
                        AND Mue_SystemCode = '{1}'
                        AND Mue_CompanyCode = '{2}'
                        AND Mue_ProfileCode = '{3}'
                    ", userlogged
                    , SystemID
                    , CompanyCode
                    , ProfileCode);

            string query = @"
               		 SELECT Mcd_Code AS 'Code'
                                   ,Mcd_Name AS 'Description'
                              FROM  M_CodeDtl
                             WHERE  Mcd_RecordStatus = 'A'
                               AND  Mcd_CodeType = 'EMPSTAT'
                               AND  Mcd_CompanyCode = '{1}'
                    {0}
                    ";

            DataTable dtResult;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();

                DataSet ds = dal.ExecuteDataSet(accessQuery);

                if (ds != null
                    && ds.Tables[0].Rows.Count > 0)
                {
                    string Cols = SetupJobStatEmpStatforQuery(ds.Tables[0]);

                    query = string.Format(query, string.Format(" AND Mcd_Code in ({0})", Cols), CompanyCode);
                    dtResult = dal.ExecuteDataSet(query, CommandType.Text).Tables[0];
                }
                else
                {
                    dtResult = new DataTable();
                }

                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetPayrollType(string userlogged, string SystemID, string CompanyCode, string ProfileCode, string CentralProfile)
        {
            string cond = string.Empty;

            string accessQuery = string.Format(@"
                    SELECT
	                    DISTINCT
	                    Mue_PayrollType
                    FROM M_UserExt
                    WHERE Mue_UserCode = '{0}'
                        AND Mue_SystemCode = '{1}'
                        AND Mue_CompanyCode = '{2}'
                        AND Mue_ProfileCode = '{3}'
                    ", userlogged, SystemID, CompanyCode, ProfileCode);

            string query = @"
               		 SELECT Mcd_Code AS 'Code'
                                   ,Mcd_Name AS 'Description'
                              FROM  M_CodeDtl
                             WHERE  Mcd_RecordStatus = 'A'
                               AND  Mcd_CodeType = 'PAYTYPE'
                               AND  Mcd_CompanyCode = '{1}'
                    {0}
                    ";

            DataTable dtResult;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();

                DataSet ds = dal.ExecuteDataSet(accessQuery);

                if (ds != null
                    && ds.Tables[0].Rows.Count > 0)
                {
                    string Cols = SetupJobStatEmpStatforQuery(ds.Tables[0]);

                    query = string.Format(query, string.Format(" AND Mcd_Code in ({0})", Cols), CompanyCode);
                    dtResult = dal.ExecuteDataSet(query, CommandType.Text).Tables[0];
                }
                else
                {
                    dtResult = new DataTable();
                }

                dal.CloseDB();
            }
            return dtResult;

        }

        public DataTable GetPayrollGroup(string userlogged, string SystemID, string CompanyCode, string ProfileCode, string CentralProfile)
        {
            string cond = string.Empty;

            string accessQuery = string.Format(@"
                    SELECT
	                    DISTINCT
	                    Mue_PayrollGroup
                    FROM M_UserExt
                    WHERE Mue_UserCode = '{0}'
                        AND Mue_SystemCode = '{1}'
                        AND Mue_CompanyCode = '{2}'
                        AND Mue_ProfileCode = '{3}'
                    ", userlogged, SystemID, CompanyCode, ProfileCode);

            string query = @"
               		 SELECT Mcd_Code AS 'Code'
                                   ,Mcd_Name AS 'Description'
                              FROM  M_CodeDtl
                             WHERE  Mcd_RecordStatus = 'A'
                               AND  Mcd_CodeType = 'PAYGRP'
                               AND  Mcd_CompanyCode = '{1}'
                    {0}
                    ";

            DataTable dtResult;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();

                DataSet ds = dal.ExecuteDataSet(accessQuery);

                if (ds != null
                    && ds.Tables[0].Rows.Count > 0)
                {
                    string Cols = SetupJobStatEmpStatforQuery(ds.Tables[0]);

                    query = string.Format(query, string.Format(" AND Mcd_Code in ({0})", Cols), CompanyCode);
                    dtResult = dal.ExecuteDataSet(query, CommandType.Text).Tables[0];
                }
                else
                {
                    dtResult = new DataTable();
                }

                dal.CloseDB();
            }
            return dtResult;

        }

        public string SetupJobStatEmpStatforQuery(DataTable dt)
        {
            string ret = string.Empty;

            for (int idx = 0; idx < dt.Rows.Count; idx++)
            {
                string str = dt.Rows[idx][0].ToString().Trim();
                if (str.IndexOf(",") != -1)
                {
                    string[] strArr = str.Split(new char[] { ',' });
                    for (int idx2 = 0; idx2 < strArr.Length; idx2++)
                    {
                        if (strArr[idx2].Trim() != string.Empty)
                        {
                            if (ret != string.Empty)
                            {
                                ret += ",";
                            }
                            ret += "'" + strArr[idx2].Trim() + "'";
                        }
                    }
                }
                else
                {
                    if (ret != string.Empty)
                    {
                        ret += ",";
                    }
                    ret += "'" + str + "'";
                }
            }

            if (ret == string.Empty)
            {
                ret = "''";
            }
            return ret;
        }

        public DataTable GetBank(string CompanyCode, string CentralProfile)
        {
            string query = string.Format(@"
                    SELECT Mbn_BankCode [Code]
                    , Mbn_BankName [Description] 
                    FROM  M_Bank
                    WHERE Mbn_CompanyCode = '{0}'
	                    AND Mbn_RecordStatus = 'A'
                    ", CompanyCode);

            DataTable dtResult;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetExpenseClass(string CompanyCode, string CentralProfile)
        {
            string query = string.Format(@"
                    SELECT Mex_ExpenseClass [Code]
					, Mex_ExpenseName [Description]
                    FROM M_Expense
					WHERE Mex_CompanyCode = '{0}'
						AND Mex_RecordStatus = 'A'
                    ", CompanyCode);

            DataTable dtResult;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public bool IsQueryNameUnique(string user, string queryDesc)
        {
            string query = string.Format(@"SELECT * FROM T_UserQuery 
                                            WHERE Tuq_Usercode = '{0}' AND Tuq_QueryName = '{1}'", user, queryDesc);
            DataTable dt;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dt = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }

            if (dt.Rows.Count > 0)
                return true;
            else
                return false;
        }

        public DataTable GetAllQueries(string user, char viewType)
        {
            string query = string.Format(@"SELECT Tuq_QueryCode AS 'Table', Tuq_QueryName AS 'Description', 'false' AS 'Custom' 
                                           FROM T_UserQuery 
                                           WHERE Tuq_Usercode = '{0}' AND Tuq_QueryType = '{1}'", user, viewType);

            DataTable dt = null;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dt = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }

            return dt;
        }

        public DataTable GetAllStandardQueries(string ViewCode, string UserCode)
        {
            string query;
            DataTable dt = null;

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                query = string.Format(@"SELECT Mqu_MasterQueryCode
                                            FROM M_Query 
                                            WHERE Mqu_QueryCode = '{0}' AND Mqu_MasterQueryCode != ''", ViewCode);
                dt = dal.ExecuteDataSet(query).Tables[0];

                if (dt.Rows.Count > 0)
                    ViewCode = dt.Rows[0]["Mqu_MasterQueryCode"].ToString();

                query = string.Format(@"SELECT Mqu_QueryCode AS 'Table', Mqu_QueryParticulars AS 'Description', 'false' AS 'Custom'
                                            FROM M_Query 
                                            WHERE Mqu_QueryCode = '{0}'

                                            UNION ALL

                                            SELECT Mqu_QueryCode AS 'Table', Mqu_QueryParticulars AS 'Description', 'false' AS 'Custom' 
                                            FROM M_Query 
                                            WHERE Mqu_MasterQueryCode = '{0}'", ViewCode, UserCode);

                dt = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }

            return dt;
        }

        public DataTable GetQueryStatement(string user, string viewName)
        {
            string query = string.Format(@"SELECT Tuq_QueryCode, Tuq_SelectStatement, Tuq_WhereStatement, Tuq_OrderStatement FROM T_UserQuery 
                                            WHERE Tuq_Usercode = '{0}' AND Tuq_QueryName = '{1}'", user, viewName);
            DataTable dt = null;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dt = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }

            return dt;
        }

        public DataTable GetStandardQueryStatement(string viewName)
        {
            string query = string.Format(@"SELECT Vst_ViewCode, Vst_SelectStatement, Vst_WhereStatement, Vst_OrderStatement FROM T_ViewStandard 
                                            WHERE Vst_StandardViewCode = '{0}'", viewName);
            DataTable dt = null;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dt = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }

            return dt;
        }
        public bool CheckIfColumnExistsInTable(string tablename, string columnname)
        {
            string query = string.Format(@"IF EXISTS ( SELECT * FROM INFORMATION_SCHEMA.COLUMNS
                                            WHERE  TABLE_NAME='{0}' AND COLUMN_NAME LIKE '{1}%')
                                            SELECT 1
                                            ELSE
                                            SELECT 0", tablename, columnname);
            DataTable dt = null;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dt = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }

            if (dt.Rows.Count > 0 && dt.Rows[0][0].ToString().Equals("1"))
                return true;
            else
                return false;
        }

    }
}
