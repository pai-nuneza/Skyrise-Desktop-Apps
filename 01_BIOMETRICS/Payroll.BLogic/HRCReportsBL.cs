/*
 *  Created by : 0951 - Perth Ownyl Te
 *               04/25/2011
 * 
 *  Modules using this BL:
 *          -   DXrptHRCReports.cs
 *          -   frmHRCReports.cs
 *          -   frmHRCPayrollScheduleReport.cs
 * 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Payroll.DAL;
using System.Drawing;
using System.Configuration;
using CommonLibrary;

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
            "ADJUSTMENTREP",
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

        public bool CheckIfMenucodeExists(string menu)
        {
            bool flag = false;
            for (int i = 0; i < menucodes.Length && !flag; i++)
            {
                if (menucodes[i] == menu)
                    flag = true;
            }
            return flag;

        }

        public DataSet getHeaderData()
        {
            DataSet ds = new DataSet();

            #region query

            string query = @"
                            declare @processflag1 as bit
                            set @processflag1 = (select pcm_processflag from t_processcontrolmaster
                            where pcm_systemid = 'PERSONNEL' and pcm_processid = 'VWNICKNAME')

                            declare @processflag2 as bit
                            set @processflag2 = (select pcm_processflag from t_processcontrolmaster
                            where pcm_systemid = 'PERSONNEL' and pcm_processid = 'DSPIDCODE')

                            Select Ccd_CompanyName
                                  ,Ccd_CompanyAddress1 + ' ' + Ccd_CompanyAddress2 + ', ' + Adt_AccountDesc as Address
                                  ,'TEL NO. ' + Ccd_TelephoneNo + ' FAX NO. ' + Ccd_FaxNo as Contacts
                                  ,Ccd_CompanyLogo  
                                  
                            From T_CompanyMaster
                            Inner Join T_AccountDetail on Ccd_CompanyAddress3 = Adt_AccountCode and Adt_AccountType='ZIPCODE'";

            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(query, CommandType.Text);

                dal.CloseDB();
            }
            return ds;
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
					from {0}..T_UserGroupDetail
					inner join {0}..T_UserGrant
					on Ugd_usergroupcode = Ugt_Usergroup
					and Ugd_SystemID = Ugt_SystemID
					where Ugd_usercode = '{1}'
					and Ugt_sysmenucode = '{2}'
					and Ugt_CanRetrieve = 1
					and Ugt_CanPrint = 1
                ";
            query = string.Format(query, database, user_Logged, systemID);
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    DataSet ds = dal.ExecuteDataSet(query, CommandType.Text);
                    if(ds.Tables[0].Rows.Count == 1)
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

        public bool GetDefaultuserCostCenterAccessRights( string user_Logged, string systemID)
        {
            bool ret = false;

            string query = @"
                                           
                        select 
	                        case when COUNT(Uca_CostCenterCode) > 1
	                        then 'YES'
	                        when COUNT(Uca_CostCenterCode) = 0
	                        then 'NO'
	                        when COUNT(Uca_CostCenterCode) = 1
	                        then 'ALL'
	                        end
	                        from T_UserCostCenterAccess
                        where UPPER( Uca_Usercode) = '{0}'
                        and Uca_SytemID = '{1}'

                    ";
            query = string.Format(query , user_Logged , systemID);
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    DataSet ds = dal.ExecuteDataSet(query, CommandType.Text);
                    if(ds.Tables[0].Rows[0][0].ToString() == "ALL")
                        ret = true;
                    else
                        ret = false;
                }
                catch(Exception er)
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

        private string SetUpItemsForEncodeFilterItems(DataTable dt)
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
            Font font = new Font("Times New Roman", 8f);
            return font;
        }

        public DataSet getCompanyData()
        {
            DataSet ds = null;
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(
                        @"
                            select
	                            Ccd_CompanyName
	                            ,Ccd_CompanyAddress1
	                            + ' ' + Adt_AccountDesc 
	                            ,Ccd_TelephoneNo
                            from T_CompanyMaster
                            left join T_AccountDetail
                            on Adt_AccountCode = Ccd_CompanyAddress3
                            and Adt_AccountType = 'ZIPCODE'
                        ", CommandType.Text
                        );
                }
                catch
                {
                    ds = null;
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return ds;
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
            }
            return table;

        }


        //#query
        public string GetQuery(string report, bool reportType, bool category)
        {
            
            string query = string.Empty;
            if (reportType) //Summary
            {
                #region summary
                switch (report)
                {
                    case "Other Income and Benefits Report":
                        #region query
                        query = @"
                                    SELECT  Eal_EmployeeId as IDNumber
		                                    , Emt_LastName  + '', '' + Emt_FirstName + '' '' +  Left(Emt_Middlename,1 ) + ''.'' as Fullname
				                            , {0}..T_EmployeeMaster.Emt_CostCenterCode + '' - '' + dbo.getCostCenterFullNameV2({0}..T_EmployeeMaster.Emt_CostCenterCode) as Costcenter		

		                                    , Eal_CurrentPayPeriod as Payperiod
                                            , Eal_AllowanceCode as OtherIncomeCode
		                                    , Eal_AllowanceAmt as Amount
		                                    , CASE WHEN Acm_Recursive = 1 THEN ''YES'' ELSE ''NO'' END	as Recursive
		                                    , CASE WHEN Eal_PayrollPost = 1 THEN ''YES'' ELSE ''NO'' END	as PosttoPayroll
		                                    , CASE WHEN Acm_TaxClass = ''T'' THEN ''TAX'' 
			                                    When Acm_TaxClass = ''N'' THEN ''NONTAX'' 
			                                    ELSE '' '' End as TaxClass
		                                    , Alpha.Adt_accountdesc as AlphalistCategory
		                                    , PayrollType.Adt_AccountDesc as PayrollType 
		                                    , EmploymentStatus.Adt_AccountDesc AS EmploymentStatus
		                                    , JobStatus.Adt_AccountDesc AS JobStatus
		                                    , Freq.Adt_AccountDesc AS Frequency
                                    FROM {0}..T_EmployeeAllowance   
                                    INNER JOIN {0}..T_EmployeeMaster on Emt_EmployeeID = Eal_EmployeeId
                                    INNER JOIN  {0}..T_AllowanceCodeMaster on Acm_AllowanceCode = Eal_AllowanceCode
                                    LEFT JOIN {0}..T_AccountDetail Alpha on Alpha.Adt_AccountCode = Acm_AlphalistCategory
	                                    AND Alpha.Adt_AccountType = ''ALPHACATGY''
                                    LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = {0}..T_EmployeeMaster.Emt_PayrollType 
	                                    AND  PayrollType.Adt_AccountType = ''PAYTYPE''
                                    LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = {0}..T_EmployeeMaster.Emt_EmploymentStatus 
	                                    AND EmploymentStatus.Adt_AccountType = ''EMPSTAT''
                                    LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = {0}..T_EmployeeMaster.Emt_JobStatus    
	                                    AND JobStatus.Adt_AccountType = ''JOBSTATUS''
                                    LEFT JOIN  {0}..T_AccountDetail  Freq ON Freq.Adt_AccountCode = Acm_ApplicablePayrollPeriod    
	                                    AND Freq.Adt_AccountType = ''DEDNFREQ''
                                    {1}
                                    	
                                    UNION ALL

                                    SELECT  Eal_EmployeeId as IDNumber
		                                    , Emt_LastName  + '', '' + Emt_FirstName + '' '' +  Left(Emt_Middlename,1 ) + ''.'' as Fullname
		                                    , {0}..T_EmployeeMasterHist.Emt_CostCenterCode + '' - '' + dbo.getCostCenterFullNameV2({0}..T_EmployeeMasterHist.Emt_CostCenterCode) as Costcenter		
		                                    , Eal_CurrentPayPeriod as Payperiod
                                            , Eal_AllowanceCode as OtherIncomeCode
		                                    , Eal_AllowanceAmt as Amount
		                                    , CASE WHEN Acm_Recursive = 1 THEN ''YES'' ELSE ''NO'' END	as Recursive
		                                    , CASE WHEN Eal_PayrollPost = 1 THEN ''YES'' ELSE ''NO'' END	as PosttoPayroll
		                                    , CASE WHEN Acm_TaxClass = ''T'' THEN ''TAX'' 
			                                    When Acm_TaxClass = ''N'' THEN ''NONTAX'' 
			                                    ELSE '' '' End as TaxClass
		                                    , Alpha.Adt_accountdesc as AlphalistCategory
		                                    , PayrollType.Adt_AccountDesc as PayrollType 
		                                    , EmploymentStatus.Adt_AccountDesc AS EmploymentStatus
		                                    , JobStatus.Adt_AccountDesc AS JobStatus
		                                    , Freq.Adt_AccountDesc AS Frequency
                                    FROM {0}..T_EmployeeAllowanceHist    
                                    INNER JOIN {0}..T_EmployeeMaster on T_EmployeeMaster.Emt_EmployeeID = Eal_EmployeeId
                                    INNER JOIN {0}..T_EmployeeMasterHist on T_EmployeeMasterHist.Emt_EmployeeID = Eal_EmployeeId
	                                    AND Emt_PayPeriod = Eal_CurrentPayPeriod
                                    INNER JOIN  {0}..T_AllowanceCodeMaster on Acm_AllowanceCode = Eal_AllowanceCode
                                    LEFT JOIN {0}..T_AccountDetail Alpha on Alpha.Adt_AccountCode = Acm_AlphalistCategory
	                                    AND Alpha.Adt_AccountType = ''ALPHACATGY''
                                    LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = {0}..T_EmployeeMaster.Emt_PayrollType 
	                                    AND  PayrollType.Adt_AccountType = ''PAYTYPE''
                                    LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = {0}..T_EmployeeMaster.Emt_EmploymentStatus 
	                                    AND EmploymentStatus.Adt_AccountType = ''EMPSTAT''
                                    LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = {0}..T_EmployeeMaster.Emt_JobStatus    
	                                    AND JobStatus.Adt_AccountType = ''JOBSTATUS''
                                    LEFT JOIN  {0}..T_AccountDetail  Freq ON Freq.Adt_AccountCode = Acm_ApplicablePayrollPeriod    
	                                    AND Freq.Adt_AccountType = ''DEDNFREQ''
                                        {1} 
                            ";
                        #endregion
                        break;
                    case "Deduction and Loan Report":
                        if (category)
                        {
                            #region query
                            query = @"
                            SELECT  Edd_EmployeeID as IDNumber
		                            , Emt_LastName  + '', '' + Emt_FirstName + '' '' +  Left(Emt_Middlename,1 ) + ''.'' as Fullname
		                            , Emt_CostCenterCode + '' - '' + dbo.getCostCenterFullNameV2(Emt_CostCenterCode) as Costcenter	
		                            , Edd_DeductionCode as DeductionCode
                                    , Edd_CurrentPayPeriod as Payperiod
		                            , Convert(Char(10),Edd_StartDeductionDate, 101) as StartofDeduction
		                            , Edd_Amount as Amount
		                            , CASE WHEN Edd_FromDeferred = 1 THEN ''YES'' ELSE ''NO'' END	as FromDeferred
		                            , CASE WHEN Edd_PaymentFlag = 1 THEN ''YES'' ELSE ''NO'' END	as PosttoPayroll
		                            , CASE WHEN Dcm_WithCheck = 1 THEN ''YES'' ELSE ''NO'' END	as WithCheck
		                            , Convert(Char(10),Edl_CheckDate, 101) as CheckDate
		                            , CASE WHEN Dcm_WithVoucher = 1 THEN ''YES'' ELSE ''NO'' END	as WithVoucher
		                            , Edl_VoucherNumber as VoucherNumber
		                            , PayrollType.Adt_AccountDesc as PayrollType 
		                            , EmploymentStatus.Adt_AccountDesc as EmploymentStatus
		                            , JobStatus.Adt_AccountDesc as JobStatus
		                            , DeductionType.Adt_AccountDesc as DeductionType
		                            , DeductionPayType.Adt_AccountDesc as PaymentType
                            FROM {0}..T_EmployeeDeductionDetail
                            INNER JOIN {0}..T_EmployeeDeductionLedger on Edl_EmployeeID =  Edd_EmployeeID
	                            AND Edl_DeductionCode = Edd_DeductionCode
	                            AND Edl_StartDeductionDate = Edd_StartDeductionDate
                            INNER JOIN {0}..T_EmployeeMaster on Emt_EmployeeID = Edd_EmployeeID
                            INNER JOIN  {0}..T_DeductionCodeMaster on Dcm_DeductionCode = Edd_DeductionCode
                            LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = Emt_PayrollType 
	                            AND  PayrollType.Adt_AccountType = ''PAYTYPE''
                            LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = Emt_EmploymentStatus 
	                            AND EmploymentStatus.Adt_AccountType = ''EMPSTAT''
                            LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = Emt_JobStatus    
	                            AND JobStatus.Adt_AccountType = ''JOBSTATUS''
                            LEFT JOIN  {0}..T_AccountDetail  DeductionType ON DeductionType.Adt_AccountCode = Dcm_DeductionType    
	                            AND DeductionType.Adt_AccountType = ''DEDNTYPE''
                            LEFT JOIN  {0}..T_AccountDetail  DeductionPayType ON DeductionPayType.Adt_AccountCode = Edd_PaymentType    
	                            AND DeductionPayType.Adt_AccountType = ''DEDPAYTYPE''
                            {1}

                            UNION ALL

                            SELECT  Edd_EmployeeID as IDNumber
		                            , Emt_LastName  + '', '' + Emt_FirstName + '' '' +  Left(Emt_Middlename,1 ) + ''.'' as Fullname
		                            , Emt_CostCenterCode + '' - '' + dbo.getCostCenterFullNameV2(Emt_CostCenterCode) as Costcenter	
		                            , Edd_DeductionCode as DeductionCode
                                    , Edd_CurrentPayPeriod as Payperiod
		                            , Convert(Char(10),Edd_StartDeductionDate, 101) as StartofDeduction
		                            , Edd_Amount as Amount
		                            , CASE WHEN Edd_FromDeferred = 1 THEN ''YES'' ELSE ''NO'' END	as FromDeferred
		                            , CASE WHEN Edd_PaymentFlag = 1 THEN ''YES'' ELSE ''NO'' END	as PosttoPayroll
		                            , CASE WHEN Dcm_WithCheck = 1 THEN ''YES'' ELSE ''NO'' END	as WithCheck
		                            , Convert(Char(10),Edl_CheckDate, 101) as CheckDate
		                            , CASE WHEN Dcm_WithVoucher = 1 THEN ''YES'' ELSE ''NO'' END	as WithVoucher
		                            , Edl_VoucherNumber as VoucherNumber
		                            , PayrollType.Adt_AccountDesc as PayrollType 
		                            , EmploymentStatus.Adt_AccountDesc as EmploymentStatus
		                            , JobStatus.Adt_AccountDesc as JobStatus
		                            , DeductionType.Adt_AccountDesc as DeductionType
		                            , DeductionPayType.Adt_AccountDesc as PaymentType
                            FROM {0}..T_EmployeeDeductionDetailHist
                            INNER JOIN {0}..T_EmployeeDeductionLedger on Edl_EmployeeID =  Edd_EmployeeID
	                            AND Edl_DeductionCode = Edd_DeductionCode
	                            AND Edl_StartDeductionDate = Edd_StartDeductionDate
                            INNER JOIN {0}..T_EmployeeMaster on {0}..T_EmployeeMaster.Emt_EmployeeID = Edd_EmployeeID
                            INNER JOIN  {0}..T_PayPeriodMaster on Ppm_PayPeriod =  Edd_PayPeriod
	                            AND Ppm_CycleIndicator = ''C''
                            INNER JOIN  {0}..T_DeductionCodeMaster on Dcm_DeductionCode = Edd_DeductionCode
                            LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = Emt_PayrollType 
	                            AND  PayrollType.Adt_AccountType = ''PAYTYPE''
                            LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = Emt_EmploymentStatus 
	                            AND EmploymentStatus.Adt_AccountType = ''EMPSTAT''
                            LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = Emt_JobStatus    
	                            AND JobStatus.Adt_AccountType = ''JOBSTATUS''
                            LEFT JOIN  {0}..T_AccountDetail  DeductionType ON DeductionType.Adt_AccountCode = Dcm_DeductionType    
	                            AND DeductionType.Adt_AccountType = ''DEDNTYPE''
                            LEFT JOIN  {0}..T_AccountDetail  DeductionPayType ON DeductionPayType.Adt_AccountCode = Edd_PaymentType    
	                            AND DeductionPayType.Adt_AccountType = ''DEDPAYTYPE''
                            {1}

                            UNION ALL

                            SELECT  Edd_EmployeeID as IDNumber
		                            , Emt_LastName  + '', '' + Emt_FirstName + '' '' +  Left(Emt_Middlename,1 ) + ''.'' as Fullname
		                            , {0}..T_EmployeeMasterHist.Emt_CostCenterCode + '' - '' + dbo.getCostCenterFullNameV2({0}..T_EmployeeMasterHist.Emt_CostCenterCode) as Costcenter	
		                            , Edd_DeductionCode as DeductionCode
                                    , Edd_CurrentPayPeriod as Payperiod
		                            , Convert(Char(10),Edd_StartDeductionDate, 101) as StartofDeduction
		                            , Edd_Amount as Amount
		                            , CASE WHEN Edd_FromDeferred = 1 THEN ''YES'' ELSE ''NO'' END	as FromDeferred
		                            , CASE WHEN Edd_PaymentFlag = 1 THEN ''YES'' ELSE ''NO'' END	as PosttoPayroll
		                            , CASE WHEN Dcm_WithCheck = 1 THEN ''YES'' ELSE ''NO'' END	as WithCheck
		                            , Convert(Char(10),Edl_CheckDate, 101) as CheckDate
		                            , CASE WHEN Dcm_WithVoucher = 1 THEN ''YES'' ELSE ''NO'' END	as WithVoucher
		                            , Edl_VoucherNumber as VoucherNumber
		                            , PayrollType.Adt_AccountDesc as PayrollType 
		                            , EmploymentStatus.Adt_AccountDesc as EmploymentStatus
		                            , JobStatus.Adt_AccountDesc as JobStatus
		                            , DeductionType.Adt_AccountDesc as DeductionType
		                            , DeductionPayType.Adt_AccountDesc as PaymentType
                            FROM {0}..T_EmployeeDeductionDetailHist
                            INNER JOIN {0}..T_EmployeeDeductionLedger on Edl_EmployeeID =  Edd_EmployeeID
	                            AND Edl_DeductionCode = Edd_DeductionCode
	                            AND Edl_StartDeductionDate = Edd_StartDeductionDate
                            INNER JOIN {0}..T_EmployeeMaster on {0}..T_EmployeeMaster.Emt_EmployeeID = Edd_EmployeeID
                            INNER JOIN  {0}..T_EmployeeMasterHist on {0}..T_EmployeeMasterHist.Emt_EmployeeID = Edd_EmployeeID
	                            AND Emt_PayPeriod = Edd_PayPeriod
                            INNER JOIN  {0}..T_DeductionCodeMaster on Dcm_DeductionCode = Edd_DeductionCode
                            LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = {0}..T_EmployeeMasterHist.Emt_PayrollType 
	                            AND  PayrollType.Adt_AccountType = ''PAYTYPE''
                            LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = {0}..T_EmployeeMasterHist.Emt_EmploymentStatus 
	                            AND EmploymentStatus.Adt_AccountType = ''EMPSTAT''
                            LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = {0}..T_EmployeeMasterHist.Emt_JobStatus    
	                            AND JobStatus.Adt_AccountType = ''JOBSTATUS''
                            LEFT JOIN  {0}..T_AccountDetail  DeductionType ON DeductionType.Adt_AccountCode = Dcm_DeductionType    
	                            AND DeductionType.Adt_AccountType = ''DEDNTYPE''
                            LEFT JOIN  {0}..T_AccountDetail  DeductionPayType ON DeductionPayType.Adt_AccountCode = Edd_PaymentType    
	                            AND DeductionPayType.Adt_AccountType = ''DEDPAYTYPE''
                            {1}

                            UNION ALL

                            SELECT  Edd_EmployeeID as IDNumber
		                            , Emt_LastName  + '', '' + Emt_FirstName + '' '' +  Left(Emt_Middlename,1 ) + ''.'' as Fullname
		                            , {0}..T_EmployeeMasterHist.Emt_CostCenterCode + '' - '' + dbo.getCostCenterFullNameV2({0}..T_EmployeeMasterHist.Emt_CostCenterCode) as Costcenter	
		                            , Edd_DeductionCode as DeductionCode
                                    , Edd_CurrentPayPeriod as Payperiod
		                            , Convert(Char(10),Edd_StartDeductionDate, 101) as StartofDeduction
		                            , Edd_Amount as Amount
		                            , CASE WHEN Edd_FromDeferred = 1 THEN ''YES'' ELSE ''NO'' END	as FromDeferred
		                            , CASE WHEN Edd_PaymentFlag = 1 THEN ''YES'' ELSE ''NO'' END	as PosttoPayroll
		                            , CASE WHEN Dcm_WithCheck = 1 THEN ''YES'' ELSE ''NO'' END	as WithCheck
		                            , Convert(Char(10),Edl_CheckDate, 101) as CheckDate
		                            , CASE WHEN Dcm_WithVoucher = 1 THEN ''YES'' ELSE ''NO'' END	as WithVoucher
		                            , Edl_VoucherNumber as VoucherNumber
		                            , PayrollType.Adt_AccountDesc as PayrollType 
		                            , EmploymentStatus.Adt_AccountDesc as EmploymentStatus
		                            , JobStatus.Adt_AccountDesc as JobStatus
		                            , DeductionType.Adt_AccountDesc as DeductionType
		                            , DeductionPayType.Adt_AccountDesc as PaymentType
                            FROM {0}..T_EmployeeDeductionDetailHistFP
                            INNER JOIN {0}..T_EmployeeDeductionLedgerHist on Edl_EmployeeID =  Edd_EmployeeID
	                            AND Edl_DeductionCode = Edd_DeductionCode
	                            AND Edl_StartDeductionDate = Edd_StartDeductionDate
                            INNER JOIN {0}..T_EmployeeMaster on {0}..T_EmployeeMaster.Emt_EmployeeID = Edd_EmployeeID
                            INNER JOIN  {0}..T_EmployeeMasterHist on {0}..T_EmployeeMasterHist.Emt_EmployeeID = Edd_EmployeeID
	                            AND Emt_PayPeriod = Edd_PayPeriod
                            INNER JOIN  {0}..T_DeductionCodeMaster on Dcm_DeductionCode = Edd_DeductionCode
                            LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = {0}..T_EmployeeMasterHist.Emt_PayrollType 
	                            AND  PayrollType.Adt_AccountType = ''PAYTYPE''
                            LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = {0}..T_EmployeeMasterHist.Emt_EmploymentStatus 
	                            AND EmploymentStatus.Adt_AccountType = ''EMPSTAT''
                            LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = {0}..T_EmployeeMasterHist.Emt_JobStatus    
	                            AND JobStatus.Adt_AccountType = ''JOBSTATUS''
                            LEFT JOIN  {0}..T_AccountDetail  DeductionType ON DeductionType.Adt_AccountCode = Dcm_DeductionType    
	                            AND DeductionType.Adt_AccountType = ''DEDNTYPE''
                            LEFT JOIN  {0}..T_AccountDetail  DeductionPayType ON DeductionPayType.Adt_AccountCode = Edd_PaymentType    
	                            AND DeductionPayType.Adt_AccountType = ''DEDPAYTYPE''
                            {1}
                            ";
                            #endregion
                        }
                        else
                        {
                            #region query
                            query = @"
                            SELECT  Edd_EmployeeID as IDNumber
		                            , Emt_LastName  + '', '' + Emt_FirstName + '' '' +  Left(Emt_Middlename,1 ) + ''.'' as Fullname
		                            , Emt_CostCenterCode + '' - '' + dbo.getCostCenterFullNameV2({0}..T_EmployeeMaster.Emt_CostCenterCode) as Costcenter	
		                            , Edd_DeductionCode as DeductionCode
		                            , Edd_PayPeriod as Payperiod
		                            , Convert(Char(10),Edd_StartDeductionDate, 101) as StartofDeduction
		                            , Edd_DeferredAmount as Amount
		                            , CasE WHEN Dcm_WithCheck = 1 THEN ''YES'' ELSE ''NO'' END	as WithCheck
		                            , Convert(Char(10),Edl_CheckDate, 101) as CheckDate
		                            , CasE WHEN Dcm_WithVoucher = 1 THEN ''YES'' ELSE ''NO'' END	as WithVoucher
		                            , Edl_VoucherNumber as VoucherNumber
		                            , PayrollType.Adt_AccountDesc as PayrollType 
		                            , EmploymentStatus.Adt_AccountDesc as EmploymentStatus
		                            , JobStatus.Adt_AccountDesc as JobStatus
		                            , DeductionType.Adt_AccountDesc as DeductionType
                            FROM {0}..T_EmployeeDeductionDeffered
                            INNER JOIN {0}..T_EmployeeDeductionLedger on Edl_EmployeeID =  Edd_EmployeeID
	                            and Edl_DeductionCode = Edd_DeductionCode
	                            and Edl_StartDeductionDate = Edd_StartDeductionDate
                            INNER JOIN {0}..T_EmployeeMaster on Emt_EmployeeID = Edd_EmployeeID
                            INNER JOIN  {0}..T_DeductionCodeMaster on Dcm_DeductionCode = Edd_DeductionCode
                            LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = Emt_PayrollType 
	                            AND  PayrollType.Adt_AccountType = ''PAYTYPE''
                            LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = Emt_EmploymentStatus 
	                            AND EmploymentStatus.Adt_AccountType = ''EMPSTAT''
                            LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = Emt_JobStatus    
	                            AND JobStatus.Adt_AccountType = ''JOBSTATUS''
                            LEFT JOIN  {0}..T_AccountDetail  DeductionType ON DeductionType.Adt_AccountCode = Dcm_DeductionType    
	                            AND DeductionType.Adt_AccountType = ''DEDNTYPE''
                            {1}
                            ";
                            #endregion
                        }
                        break;
                    case "Deduction and Loan Ledger Report":
                        if (category)
                        {
                            #region query
                            query = @"
                            SELECT  Edl_EmployeeID as IDNumber
		                            , Emt_LastName  + '', '' + Emt_FirstName + '' '' +  Left(Emt_Middlename,1 ) + ''.'' as Fullname
		                            , Emt_CostCenterCode + '' - '' + dbo.getCostCenterFullNameV2(Emt_CostCenterCode) as Costcenter	
		                            , Edl_DeductionCode as DeductionCode
		                            , Convert(Char(10),Edl_StartDeductionDate, 101) as StartofDeduction
		                            , Edl_DeductionAmount as DeductionAmount
		                            , Edl_PaidAmount as PaidAmount
		                            , Edl_DeductionAmount - Edl_PaidAmount as BalanceAmount
		                            , Edl_AmortizationAmount as AmortizationAmount
		                            , Edl_DeferredAmount as DeferredAmount
		                            , CASE WHEN Dcm_WithCheck = 1 THEN ''YES'' ELSE ''NO'' END	as WithCheck
		                            , Convert(Char(10),Edl_CheckDate, 101) as CheckDate
		                            , CASE WHEN Dcm_WithVoucher = 1 THEN ''YES'' ELSE ''NO'' END	as WithVoucher
		                            , Edl_VoucherNumber as VoucherNumber
		                            , CASE WHEN Edl_ExcludeFromPayroll = 1 THEN ''YES'' ELSE ''NO'' END	as ExcludeFromPayroll
		                            , Convert(Char(10),Edl_FullyPaidDate, 101) as FullyPaidDate		
		                            , PayrollType.Adt_AccountDesc as PayrollType  
		                            , EmploymentStatus.Adt_AccountDesc AS EmploymentStatus
		                            , JobStatus.Adt_AccountDesc AS JobStatus
		                            , DeductionType.Adt_AccountDesc AS DeductionType
                            FROM {0}..T_EmployeeDeductionLedger
                            INNER JOIN {0}..T_EmployeeMaster on Emt_EmployeeID = Edl_EmployeeID
                            INNER JOIN  {0}..T_DeductionCodeMaster on Dcm_DeductionCode = Edl_DeductionCode
                            LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = Emt_PayrollType 
	                            AND  PayrollType.Adt_AccountType = ''PAYTYPE''
                            LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = Emt_EmploymentStatus 
	                            AND EmploymentStatus.Adt_AccountType = ''EMPSTAT''
                            LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = Emt_JobStatus    
	                            AND JobStatus.Adt_AccountType = ''JOBSTATUS''
                            LEFT JOIN  {0}..T_AccountDetail  DeductionType ON DeductionType.Adt_AccountCode = Dcm_DeductionType    
	                            AND DeductionType.Adt_AccountType = ''DEDNTYPE''
                            {1}
                            ";
                            #endregion
                        }
                        else
                        {
                            #region query
                            query = @"
                            SELECT  Edl_EmployeeID as IDNumber
		                            , Emt_LastName  + '', '' + Emt_FirstName + '' '' +  Left(Emt_Middlename,1 ) + ''.'' as Fullname
		                            , Emt_CostCenterCode + '' - '' + dbo.getCostCenterFullNameV2(Emt_CostCenterCode) as Costcenter	
		                            , Edl_DeductionCode as DeductionCode
		                            , Convert(Char(10),Edl_StartDeductionDate, 101) as StartofDeduction
		                            , Edl_DeductionAmount as DeductionAmount
		                            , Edl_PaidAmount as PaidAmount
		                            , Edl_DeductionAmount - Edl_PaidAmount as BalanceAmount
		                            , Edl_AmortizationAmount as AmortizationAmount
		                            , Edl_DeferredAmount as DeferredAmount
		                            , CASE WHEN Dcm_WithCheck = 1 THEN ''YES'' ELSE ''NO'' END	as WithCheck
		                            , Convert(Char(10),Edl_CheckDate, 101) as CheckDate
		                            , CASE WHEN Dcm_WithVoucher = 1 THEN ''YES'' ELSE ''NO'' END	as WithVoucher
		                            , Edl_VoucherNumber as VoucherNumber
		                            , CASE WHEN Edl_ExcludeFromPayroll = 1 THEN ''YES'' ELSE ''NO'' END	as ExcludeFromPayroll
		                            , Convert(Char(10),Edl_FullyPaidDate, 101) as FullyPaidDate		
		                            , PayrollType.Adt_AccountDesc as PayrollType  
		                            , EmploymentStatus.Adt_AccountDesc AS EmploymentStatus
		                            , JobStatus.Adt_AccountDesc AS JobStatus
		                            , DeductionType.Adt_AccountDesc AS DeductionType
                            FROM {0}..T_EmployeeDeductionLedgerHist
                            INNER JOIN {0}..T_EmployeeMaster on Emt_EmployeeID = Edl_EmployeeID
                            INNER JOIN  {0}..T_DeductionCodeMaster on Dcm_DeductionCode = Edl_DeductionCode
                            LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = Emt_PayrollType 
	                            AND  PayrollType.Adt_AccountType = ''PAYTYPE''
                            LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = Emt_EmploymentStatus 
	                            AND EmploymentStatus.Adt_AccountType = ''EMPSTAT''
                            LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = Emt_JobStatus    
	                            AND JobStatus.Adt_AccountType = ''JOBSTATUS''
                            LEFT JOIN  {0}..T_AccountDetail  DeductionType ON DeductionType.Adt_AccountCode = Dcm_DeductionType    
	                            AND DeductionType.Adt_AccountType = ''DEDNTYPE''
                            {1}
                            ";
                            #endregion
                        }
                        break;
                    case "Adjustment Report":
                        #region query
                        query = @"
                        SELECT  {0}..T_EmployeeAdjustment.Ead_EmployeeId as IDNumber
		                        , Emt_LastName  + '', '' + Emt_FirstName + '' '' +  Left(Emt_Middlename,1 ) + ''.'' as Fullname
		                        , Emt_CostCenterCode + '' - '' + dbo.getCostCenterFullNameV2(Emt_CostCenterCode) as Costcenter		
		                        , {0}..T_EmployeeAdjustment.Ead_CurrentPayPeriod as ThisPayperiod
		                        , Ead_HrlyRate as HourlyRate
		                        , Ead_RegularHr as RegularHour
		                        , Ead_RegularOTHr as RegularOTHour
		                        , Ead_RegularNDHr as RegularNDHour
		                        , Ead_RegularOTNDHr as RegularOTNDHour
		                        , Ead_RestdayHr as RestdayHour
		                        , Ead_RestdayOTHr as RestdayOTHour
		                        , Ead_RestdayNDHr as RestdayNDHour
		                        , Ead_RestdayOTNDHr as RestdayOTNDHour
		                        , Ead_LegalHolidayHr as LegalHolidayHour
		                        , Ead_LegalHolidayOTHr as LegalHolidayOTHour
		                        , Ead_LegalHolidayNDHr as LegalHolidayNDHour
		                        , Ead_LegalHolidayOTNDHr as LegalHolidayOTNDHour
		                        , Ead_SpecialHolidayHr as SpecialHolidayHour
		                        , Ead_SpecialHolidayOTHr as SpecialHolidayOTHour
		                        , Ead_SpecialHolidayNDHr as SpecialHolidayNDHour
		                        , Ead_SpecialHolidayOTNDHr as SpecialHolidayOTNDHour
		                        , Ead_PlantShutdownHr as PlantShutdownHour		
		                        , Ead_PlantShutdownOTHr as PlantShutdownOTHour
		                        , Ead_PlantShutdownNDHr as PlantShutdownNDHour
		                        , Ead_PlantShutdownOTNDHr as PlantShutdownOTNDHour
		                        , Ead_CompanyHolidayHr as CompanyHolidayHour
		                        , Ead_CompanyHolidayOTHr as CompanyHolidayOTHour
		                        , Ead_CompanyHolidayNDHr as CompanyHolidayNDHour
		                        , Ead_CompanyHolidayOTNDHr as CompanyHolidayOTNDHour
		                        , Ead_RestdayLegalHolidayHr as RestdayLegalHolidayHour
		                        , Ead_RestdayLegalHolidayOTHr as RestdayLegalHolidayOTHour
		                        , Ead_RestdayLegalHolidayNDHr as RestdayLegalHolidayNDHour
		                        , Ead_RestdayLegalHolidayOTNDHr as RestdayLegalHolidayOTNDHour
		                        , Ead_RestdaySpecialHolidayHr as RestdaySpecialHolidayHour
		                        , Ead_RestdaySpecialHolidayOTHr as RestdaySpecialHolidayOTHour
		                        , Ead_RestdaySpecialHolidayNDHr as RestdaySpecialHolidayNDHour
		                        , Ead_RestdaySpecialHolidayOTNDHr as RestdaySpecialHolidayOTNDHour
		                        , Ead_RestdayCompanyHolidayHr as RestdayCompanyHolidayHour
		                        , Ead_RestdayCompanyHolidayOTHr as RestdayCompanyHolidayOTHour
		                        , Ead_RestdayCompanyHolidayNDHr as RestdayCompanyHolidayNDHour
		                        , Ead_RestdayCompanyHolidayOTNDHr as RestdayCompanyHolidayOTNDHour
		                        , Ead_RestdayPlantShutdownHr as RestdayPlantShutdownHour	
		                        , Ead_RestdayPlantShutdownOTHr as RestdayPlantShutdownOTHour
		                        , Ead_RestdayPlantShutdownNDHr as RestdayPlantShutdownNDHour
		                        , Ead_RestdayPlantShutdownOTNDHr as RestdayPlantShutdownOTNDHour
		                        , Ead_Filler01_Hr as Filler01Hour
		                        , Ead_Filler01_OTHr as Filler01OTHour
		                        , Ead_Filler01_NDHr as Filler01NDHour
		                        , Ead_Filler01_OTNDHr as Filler01OTNDHour
		                        , Ead_Filler02_Hr as Filler02Hour
		                        , Ead_Filler02_OTHr as Filler02OTHour
		                        , Ead_Filler02_NDHr as Filler02NDHour
		                        , Ead_Filler02_OTNDHr as Filler02OTNDHour
		                        , Ead_Filler03_Hr as Filler03Hour
		                        , Ead_Filler03_OTHr as Filler03OTHour
		                        , Ead_Filler03_NDHr as Filler03NDHour
		                        , Ead_Filler03_OTNDHr as Filler03OTNDHour
		                        , Ead_Filler04_Hr as Filler04Hour
		                        , Ead_Filler04_OTHr as Filler04OTHour
		                        , Ead_Filler04_NDHr as Filler04NDHour
		                        , Ead_Filler04_OTNDHr as Filler04OTNDHour
		                        , Ead_Filler05_Hr as Filler05Hour
		                        , Ead_Filler05_OTHr as Filler05OTHour
		                        , Ead_Filler05_NDHr as Filler05NDHour
		                        , Ead_Filler05_OTNDHr as Filler05OTNDHour
		                        , Ead_Filler06_Hr as Filler06Hour
		                        , Ead_Filler06_OTHr as Filler06OTHour
		                        , Ead_Filler06_NDHr as Filler06NDHour
		                        , Ead_Filler06_OTNDHr as Filler06OTNDHour
		                        , Ead_LaborHrsAdjustmentAmt as LaborHrsAdjustmentAmount
		                        , Ead_TaxAdjustmentAmt as TaxAdjustmentAmount
		                        , Ead_NonTaxAdjustmentAmt as NontaxAdjustmentAmount
		                        , CASE WHEN Ead_PayrollPost = 1 THEN ''YES'' ELSE ''NO'' END	as PosttoPayroll
		                        , PayrollType.Adt_AccountDesc as PayrollType 
		                        , EmploymentStatus.Adt_AccountDesc AS EmploymentStatus
		                        , JobStatus.Adt_AccountDesc AS JobStatus
                        FROM {0}..T_EmployeeAdjustment
                        LEFT JOIN {0}..T_EmployeeAdjustmentExt on {0}..T_EmployeeAdjustmentExt.Ead_EmployeeId = {0}..T_EmployeeAdjustment.Ead_EmployeeId
	                        AND {0}..T_EmployeeAdjustmentExt.Ead_CurrentPayPeriod = {0}..T_EmployeeAdjustment.Ead_CurrentPayPeriod
                        INNER JOIN {0}..T_EmployeeMaster on Emt_EmployeeID = {0}..T_EmployeeAdjustment.Ead_EmployeeId
                        LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = Emt_PayrollType 
	                        AND  PayrollType.Adt_AccountType = ''PAYTYPE''
                        LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = Emt_EmploymentStatus 
	                        AND EmploymentStatus.Adt_AccountType = ''EMPSTAT''
                        LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = Emt_JobStatus    
	                        AND JobStatus.Adt_AccountType = ''JOBSTATUS''
                        {1}

                        UNION ALL

                        SELECT  {0}..T_EmployeeAdjustmentHist.Ead_EmployeeId as IDNumber
		                        , Emt_LastName  + '', '' + Emt_FirstName + '' '' +  Left(Emt_Middlename,1 ) + ''.'' as Fullname
		                        , {0}..T_EmployeeMasterHist.Emt_CostCenterCode + '' - '' + dbo.getCostCenterFullNameV2({0}..T_EmployeeMasterHist.Emt_CostCenterCode) as Costcenter		
		                        , {0}..T_EmployeeAdjustmentHist.Ead_CurrentPayPeriod as ThisPayperiod
		                        , Ead_HrlyRate as HourlyRate
		                        , Ead_RegularHr as RegularHour
		                        , Ead_RegularOTHr as RegularOTHour
		                        , Ead_RegularNDHr as RegularNDHour
		                        , Ead_RegularOTNDHr as RegularOTNDHour
		                        , Ead_RestdayHr as RestdayHour
		                        , Ead_RestdayOTHr as RestdayOTHour
		                        , Ead_RestdayNDHr as RestdayNDHour
		                        , Ead_RestdayOTNDHr as RestdayOTNDHour
		                        , Ead_LegalHolidayHr as LegalHolidayHour
		                        , Ead_LegalHolidayOTHr as LegalHolidayOTHour
		                        , Ead_LegalHolidayNDHr as LegalHolidayNDHour
		                        , Ead_LegalHolidayOTNDHr as LegalHolidayOTNDHour
		                        , Ead_SpecialHolidayHr as SpecialHolidayHour
		                        , Ead_SpecialHolidayOTHr as SpecialHolidayOTHour
		                        , Ead_SpecialHolidayNDHr as SpecialHolidayNDHour
		                        , Ead_SpecialHolidayOTNDHr as SpecialHolidayOTNDHour
		                        , Ead_PlantShutdownHr as PlantShutdownHour
		                        , Ead_PlantShutdownOTHr as PlantShutdownOTHour
		                        , Ead_PlantShutdownNDHr as PlantShutdownNDHour
		                        , Ead_PlantShutdownOTNDHr as PlantShutdownOTNDHour
		                        , Ead_CompanyHolidayHr as CompanyHolidayHour
		                        , Ead_CompanyHolidayOTHr as CompanyHolidayOTHour
		                        , Ead_CompanyHolidayNDHr as CompanyHolidayNDHour
		                        , Ead_CompanyHolidayOTNDHr as CompanyHolidayOTNDHour
		                        , Ead_RestdayLegalHolidayHr as RestdayLegalHolidayHour
		                        , Ead_RestdayLegalHolidayOTHr as RestdayLegalHolidayOTHour
		                        , Ead_RestdayLegalHolidayNDHr as RestdayLegalHolidayNDHour
		                        , Ead_RestdayLegalHolidayOTNDHr as RestdayLegalHolidayOTNDHour
		                        , Ead_RestdaySpecialHolidayHr as RestdaySpecialHolidayHour
		                        , Ead_RestdaySpecialHolidayOTHr as RestdaySpecialHolidayOTHour
		                        , Ead_RestdaySpecialHolidayNDHr as RestdaySpecialHolidayNDHour
		                        , Ead_RestdaySpecialHolidayOTNDHr as RestdaySpecialHolidayOTNDHour
		                        , Ead_RestdayCompanyHolidayHr as RestdayCompanyHolidayHour
		                        , Ead_RestdayCompanyHolidayOTHr as RestdayCompanyHolidayOTHour
		                        , Ead_RestdayCompanyHolidayNDHr as RestdayCompanyHolidayNDHour
		                        , Ead_RestdayCompanyHolidayOTNDHr as RestdayCompanyHolidayOTNDHour
		                        , Ead_RestdayPlantShutdownHr as RestdayPlantShutdownHour
		                        , Ead_RestdayPlantShutdownOTHr as RestdayPlantShutdownOTHour
		                        , Ead_RestdayPlantShutdownNDHr as RestdayPlantShutdownNDHour
		                        , Ead_RestdayPlantShutdownOTNDHr as RestdayPlantShutdownOTNDHour
		                        , Ead_Filler01_Hr as Filler01Hour
		                        , Ead_Filler01_OTHr as Filler01OTHour
		                        , Ead_Filler01_NDHr as Filler01NDHour
		                        , Ead_Filler01_OTNDHr as Filler01OTNDHour
		                        , Ead_Filler02_Hr as Filler02Hour
		                        , Ead_Filler02_OTHr as Filler02OTHour
		                        , Ead_Filler02_NDHr as Filler02NDHour
		                        , Ead_Filler02_OTNDHr as Filler02OTNDHour
		                        , Ead_Filler03_Hr as Filler03Hour
		                        , Ead_Filler03_OTHr as Filler03OTHour
		                        , Ead_Filler03_NDHr as Filler03NDHour
		                        , Ead_Filler03_OTNDHr as Filler03OTNDHour
		                        , Ead_Filler04_Hr as Filler04Hour
		                        , Ead_Filler04_OTHr as Filler04OTHour
		                        , Ead_Filler04_NDHr as Filler04NDHour
		                        , Ead_Filler04_OTNDHr as Filler04OTNDHour
		                        , Ead_Filler05_Hr as Filler05Hour
		                        , Ead_Filler05_OTHr as Filler05OTHour
		                        , Ead_Filler05_NDHr as Filler05NDHour
		                        , Ead_Filler05_OTNDHr as Filler05OTNDHour
		                        , Ead_Filler06_Hr as Filler06Hour
		                        , Ead_Filler06_OTHr as Filler06OTHour
		                        , Ead_Filler06_NDHr as Filler06NDHour
		                        , Ead_Filler06_OTNDHr as Filler06OTNDHour
		                        , Ead_LaborHrsAdjustmentAmt as LaborHrsAdjustmentAmount
		                        , Ead_TaxAdjustmentAmt as TaxAdjustmentAmount
		                        , Ead_NonTaxAdjustmentAmt as NontaxAdjustmentAmount
		                        , CASE WHEN Ead_PayrollPost = 1 THEN ''YES'' ELSE ''NO'' END	as PosttoPayroll
		                        , PayrollType.Adt_AccountDesc as PayrollType 
		                        , EmploymentStatus.Adt_AccountDesc AS EmploymentStatus
		                        , JobStatus.Adt_AccountDesc AS JobStatus
                        FROM {0}..T_EmployeeAdjustmentHist
                        LEFT JOIN {0}..T_EmployeeAdjustmentHistExt on {0}..T_EmployeeAdjustmentHistExt.Ead_EmployeeId = {0}..T_EmployeeAdjustmentHist.Ead_EmployeeId
	                        AND {0}..T_EmployeeAdjustmentHistExt.Ead_CurrentPayPeriod = {0}..T_EmployeeAdjustmentHist.Ead_CurrentPayPeriod
                        INNER JOIN {0}..T_EmployeeMaster on {0}..T_EmployeeMaster.Emt_EmployeeID = {0}..T_EmployeeAdjustmentHist.Ead_EmployeeId
                        INNER JOIN {0}..T_EmployeeMasterHist on {0}..T_EmployeeMasterHist.Emt_EmployeeID = {0}..T_EmployeeAdjustmentHist.Ead_EmployeeId
	                        AND {0}..T_EmployeeMasterHist.Emt_PayPeriod =  {0}..T_EmployeeAdjustmentHist.Ead_CurrentPayPeriod
                        LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = {0}..T_EmployeeMasterHist.Emt_PayrollType 
	                        AND  PayrollType.Adt_AccountType = ''PAYTYPE''
                        LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = {0}..T_EmployeeMasterHist.Emt_EmploymentStatus 
	                        AND EmploymentStatus.Adt_AccountType = ''EMPSTAT''
                        LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = {0}..T_EmployeeMasterHist.Emt_JobStatus    
	                        AND JobStatus.Adt_AccountType = ''JOBSTATUS''
                        {2}	

                        ";
                        #endregion
                        break;
                    case "Log Report":
                        #region query
                        query = @"
                        SELECT  {0}..T_EmployeeLogledger.Ell_EmployeeId as IDNumber
		                        , Emt_LastName  + '', '' + Emt_FirstName + '' '' +  Left(Emt_Middlename,1 ) + ''.'' as Fullname
		                        , Emt_CostCenterCode + '' - '' + dbo.getCostCenterFullNameV2(Emt_CostCenterCode) as Costcenter
		                        , Ell_PayPeriod as ThisPeriod
		                        , CONVERT(char(10), Ell_ProcessDate,101) as LogDate
		                        , Ell_DayCode as DayCode
		                        , Ell_ShiftCode as ShiftCode
		                        , LEFT(Ell_ActualTimeIn_1,2) + '':'' + RIGHT(Ell_ActualTimeIn_1, 2) as ActualTimeIn1
		                        , LEFT(Ell_ActualTimeOut_1,2) +'':'' + RIGHT(Ell_ActualTimeOut_1, 2) as ActualTimeOut1
		                        , LEFT(Ell_ActualTimeIn_2, 2) + '':'' + RIGHT(Ell_ActualTimeIn_2, 2) as ActualTimeIn2
		                        , LEFT(Ell_ActualTimeOut_2, 2) + '':'' + RIGHT(Ell_ActualTimeOut_2, 2) as ActualTimeOut2
		                        , LEFT(Scm_ShiftTimeIn, 2) + '':'' + RIGHT(Scm_ShiftTimeIn, 2) as ShiftTimeIn
		                        , LEFT(Scm_ShiftBreakStart, 2) + '':'' + RIGHT(Scm_ShiftBreakStart, 2) as StartofBreak
		                        , LEFT(Scm_ShiftBreakEnd, 2) + '':'' + RIGHT(Scm_ShiftBreakEnd, 2) as EndofBreak
		                        , LEFT(Scm_ShiftTimeOut, 2) + '':'' + RIGHT(Scm_ShiftTimeOut, 2) as ShiftTimeOut
		                        , ((CONVERT(int,LEFT(Ell_ActualTimeIn_1,2)) * 60) + CONVERT(int,RIGHT(Ell_ActualTimeIn_1,2)))- ((CONVERT(int,LEFT(Scm_ShiftTimeIn,2)) * 60) + CONVERT(int,RIGHT(Scm_ShiftTimeIn,2))) as MinutesLogIN1beforeshift
		                        , ((CONVERT(int,LEFT(Ell_ActualTimeOut_1,2)) * 60) + CONVERT(int,RIGHT(Ell_ActualTimeOut_1,2))) - ((CONVERT(int,LEFT(Scm_ShiftBreakStart,2)) * 60) + CONVERT(int,RIGHT(Scm_ShiftBreakStart,2))) as MinutesLogOUT1aftershift
		                        , ((CONVERT(int,LEFT(Ell_ActualTimeIn_2,2)) * 60) + CONVERT(int,RIGHT(Ell_ActualTimeIn_2,2))) - ((CONVERT(int,LEFT(Scm_ShiftBreakEnd,2)) * 60) + CONVERT(int,RIGHT(Scm_ShiftBreakEnd,2))) as MinutesLogIN2beforeshift
		                        , ((CONVERT(int,LEFT(Ell_ActualTimeOut_2,2)) * 60) + CONVERT(int,RIGHT(Ell_ActualTimeOut_2,2))) - ((CONVERT(int,LEFT(Scm_ShiftTimeOut,2)) * 60) + CONVERT(int,RIGHT(Scm_ShiftTimeOut,2))) as MinutesLogOUT2aftershift
		                        , Ell_AbsentHour as AbsentHour
		                        , Ell_RegularHour as RegularHour
		                        , Ell_OvertimeHour as OvertimeHour
		                        , Ell_RegularNightPremHour as RegularNightPremHour
		                        , Ell_OvertimeNightPremHour as OvertimeNightPremHour
		                        , Ell_LeaveHour as LeaveHour
		                        , Ell_WorkType as WorkType
		                        , Ell_WorkGroup as WorkGroup
		                        , LocationCode.Adt_AccountDesc as Location
		                        , PayrollType.Adt_AccountDesc as PayrollType 
		                        , EmploymentStatus.Adt_AccountDesc AS EmploymentStatus
		                        , JobStatus.Adt_AccountDesc AS JobStatus
                        FROM {0}..T_EmployeeLogledger
                        INNER JOIN {0}..T_EmployeeMaster on Emt_EmployeeID = {0}..T_EmployeeLogledger.Ell_EmployeeId
                        INNER JOIN {0}..T_ShiftCodeMaster on Scm_ShiftCode = Ell_ShiftCode
                        LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = Emt_PayrollType 
	                        AND  PayrollType.Adt_AccountType = ''PAYTYPE''
                        LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = Emt_EmploymentStatus 
	                        AND EmploymentStatus.Adt_AccountType = ''EMPSTAT''
                        LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = Emt_JobStatus    
	                        AND JobStatus.Adt_AccountType = ''JOBSTATUS''
                        LEFT JOIN T_AccountDetail LocationCode on LocationCode.Adt_AccountCode = Ell_LocationCode
							and LocationCode.Adt_AccountType = ''ZIPCODE''
                        {1}

                        UNION ALL

                        SELECT  {0}..T_EmployeeLogLedgerHist.Ell_EmployeeId as IDNumber
		                        , Emt_LastName  + '', '' + Emt_FirstName + '' '' +  Left(Emt_Middlename,1 ) + ''.'' as Fullname
		                        , {0}..T_EmployeeMasterHist.Emt_CostCenterCode + '' - '' + dbo.getCostCenterFullNameV2({0}..T_EmployeeMasterHist.Emt_CostCenterCode) as Costcenter
		                        , Ell_PayPeriod as ThisPeriod
		                        , CONVERT(char(10), Ell_ProcessDate,101) as LogDate
		                        , Ell_DayCode as DayCode
		                        , Ell_ShiftCode as ShiftCode
		                        , LEFT(Ell_ActualTimeIn_1,2) + '':'' + RIGHT(Ell_ActualTimeIn_1, 2) as ActualTimeIn1
		                        , LEFT(Ell_ActualTimeOut_1,2) +'':'' + RIGHT(Ell_ActualTimeOut_1, 2) as ActualTimeOut1
		                        , LEFT(Ell_ActualTimeIn_2, 2) + '':'' + RIGHT(Ell_ActualTimeIn_2, 2) as ActualTimeIn2
		                        , LEFT(Ell_ActualTimeOut_2, 2) + '':'' + RIGHT(Ell_ActualTimeOut_2, 2) as ActualTimeOut2
		                        , LEFT(Scm_ShiftTimeIn, 2) + '':'' + RIGHT(Scm_ShiftTimeIn, 2) as ShiftTimeIn
		                        , LEFT(Scm_ShiftBreakStart, 2) + '':'' + RIGHT(Scm_ShiftBreakStart, 2) as StartofBreak
		                        , LEFT(Scm_ShiftBreakEnd, 2) + '':'' + RIGHT(Scm_ShiftBreakEnd, 2) as EndofBreak
		                        , LEFT(Scm_ShiftTimeOut, 2) + '':'' + RIGHT(Scm_ShiftTimeOut, 2) as ShiftTimeOut
		                        , ((CONVERT(int,LEFT(Ell_ActualTimeIn_1,2)) * 60) + CONVERT(int,RIGHT(Ell_ActualTimeIn_1,2)))- ((CONVERT(int,LEFT(Scm_ShiftTimeIn,2)) * 60) + CONVERT(int,RIGHT(Scm_ShiftTimeIn,2))) as MinutesLogIN1beforeshift
		                        , ((CONVERT(int,LEFT(Ell_ActualTimeOut_1,2)) * 60) + CONVERT(int,RIGHT(Ell_ActualTimeOut_1,2))) - ((CONVERT(int,LEFT(Scm_ShiftBreakStart,2)) * 60) + CONVERT(int,RIGHT(Scm_ShiftBreakStart,2))) as MinutesLogOUT1aftershift
		                        , ((CONVERT(int,LEFT(Ell_ActualTimeIn_2,2)) * 60) + CONVERT(int,RIGHT(Ell_ActualTimeIn_2,2))) - ((CONVERT(int,LEFT(Scm_ShiftBreakEnd,2)) * 60) + CONVERT(int,RIGHT(Scm_ShiftBreakEnd,2))) as MinutesLogIN2beforeshift
		                        , ((CONVERT(int,LEFT(Ell_ActualTimeOut_2,2)) * 60) + CONVERT(int,RIGHT(Ell_ActualTimeOut_2,2))) - ((CONVERT(int,LEFT(Scm_ShiftTimeOut,2)) * 60) + CONVERT(int,RIGHT(Scm_ShiftTimeOut,2))) as MinutesLogOUT2aftershift
		                        , Ell_AbsentHour as AbsentHour
		                        , Ell_RegularHour as RegularHour
		                        , Ell_OvertimeHour as OvertimeHour
		                        , Ell_RegularNightPremHour as RegularNightPremHour
		                        , Ell_OvertimeNightPremHour as OvertimeNightPremHour
		                        , Ell_LeaveHour as LeaveHour
		                        , Ell_WorkType as WorkType
		                        , Ell_WorkGroup as WorkGroup
		                        , LocationCode.Adt_AccountDesc as Location
		                        , PayrollType.Adt_AccountDesc as PayrollType 
		                        , EmploymentStatus.Adt_AccountDesc AS EmploymentStatus
		                        , JobStatus.Adt_AccountDesc AS JobStatus
                        FROM {0}..T_EmployeeLogLedgerHist
                        INNER JOIN {0}..T_EmployeeMaster on {0}..T_EmployeeMaster.Emt_EmployeeID = {0}..T_EmployeeLogLedgerHist.Ell_EmployeeId
                        INNER JOIN {0}..T_EmployeeMasterHist on {0}..T_EmployeeMasterHist.Emt_EmployeeID = {0}..T_EmployeeLogLedgerHist.Ell_EmployeeId
	                        AND {0}..T_EmployeeMasterHist.Emt_Payperiod = {0}..T_EmployeeLogLedgerHist.Ell_PayPeriod
                        INNER JOIN {0}..T_ShiftCodeMaster on Scm_ShiftCode = Ell_ShiftCode
                        LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = {0}..T_EmployeeMasterHist.Emt_PayrollType 
	                        AND  PayrollType.Adt_AccountType = ''PAYTYPE''
                        LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = {0}..T_EmployeeMasterHist.Emt_EmploymentStatus 
	                        AND EmploymentStatus.Adt_AccountType = ''EMPSTAT''
                        LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = {0}..T_EmployeeMasterHist.Emt_JobStatus    
	                        AND JobStatus.Adt_AccountType = ''JOBSTATUS''
                        LEFT JOIN T_AccountDetail LocationCode on LocationCode.Adt_AccountCode = Ell_LocationCode
							and LocationCode.Adt_AccountType = ''ZIPCODE''
	                        {1}
                        ";
                        #endregion
                        break;
                    case "Assume Present Report":
                        #region query
                        query = @"
                        SELECT  {0}..T_EmployeeLogledger.Ell_EmployeeId as [IDNumber]
		                        , Emt_LastName  + '', '' + Emt_FirstName + '' '' +  Left(Emt_Middlename,1 ) + ''.'' as Fullname
		                        , {0}..T_EmployeeMaster.Emt_CostCenterCode + '' - '' + dbo.getCostCenterFullNameV2({0}..T_EmployeeMaster.Emt_CostCenterCode) as Costcenter
		                        , Ell_PayPeriod as [ThisPeriod]
		                        , CONVERT(char(10), Ell_ProcessDate,101) as [LogDate]	
		                        , Ell_DayCode as [DayCode]
		                        , Ell_ShiftCode as [ShiftCode]
		                        , LEFT(Ell_ActualTimeIn_1,2) + '':'' + RIGHT(Ell_ActualTimeIn_1, 2) as [ActualTimeIn1]
		                        , LEFT(Ell_ActualTimeOut_1,2) +'':'' + RIGHT(Ell_ActualTimeOut_1, 2) as [ActualTimeOut1]
		                        , LEFT(Ell_ActualTimeIn_2, 2) + '':'' + RIGHT(Ell_ActualTimeIn_2, 2) as [ActualTimeIn2]
		                        , LEFT(Ell_ActualTimeOut_2, 2) + '':'' + RIGHT(Ell_ActualTimeOut_2, 2) as [ActualTimeOut2]
		                        , LEFT(Scm_ShiftTimeIn, 2) + '':'' + RIGHT(Scm_ShiftTimeIn, 2) as [Shift Time In]
		                        , LEFT(Scm_ShiftBreakStart, 2) + '':'' + RIGHT(Scm_ShiftBreakStart, 2) as [StartofBreak]
		                        , LEFT(Scm_ShiftBreakEnd, 2) + '':'' + RIGHT(Scm_ShiftBreakEnd, 2) as [EndofBreak]
		                        , LEFT(Scm_ShiftTimeOut, 2) + '':'' + RIGHT(Scm_ShiftTimeOut, 2) as [ShiftTimeOut]
		                        , Ell_WorkType as [WorkType]
		                        , Ell_WorkGroup as [WorkGroup]
		                        , LocationCode.Adt_AccountDesc as [Location]
		                        , PayrollType.Adt_AccountDesc as [PayrollType] 
		                        , EmploymentStatus.Adt_AccountDesc AS [EmploymentStatus]
		                        , JobStatus.Adt_AccountDesc AS [JobStatus]
		                        , Case when Ell_AssumedPostBack = ''T'' then ''TEMP LOGS''
			                         when Ell_AssumedPostBack = ''A'' and Ell_ActualTimeIn_1 + Ell_ActualTimeOut_1 + Ell_ActualTimeIn_2 + Ell_ActualTimeOut_2 = ''0000000000000000'' then ''ACTUAL NO LOGS''
			                        when Ell_AssumedPostBack = ''A'' and Ell_ActualTimeIn_1 + Ell_ActualTimeOut_1 + Ell_ActualTimeIn_2 + Ell_ActualTimeOut_2 <> ''0000000000000000'' then ''ACTUAL WITH LOGS''
			                        else '' '' end as [Remarks]
                        FROM {0}..T_EmployeeLogledger
                        INNER JOIN  {0}..T_EmployeeMaster on Emt_EmployeeID = {0}..T_EmployeeLogledger.Ell_EmployeeId
                        INNER JOIN  {0}..T_ShiftCodeMaster on Scm_ShiftCode = Ell_ShiftCode
                        LEFT  JOIN {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = Emt_PayrollType 
	                        AND  PayrollType.Adt_AccountType = ''PAYTYPE''
                        LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = Emt_EmploymentStatus 
	                        AND EmploymentStatus.Adt_AccountType = ''EMPSTAT''
                        LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = Emt_JobStatus    
	                        AND JobStatus.Adt_AccountType = ''JOBSTATUS''
                        LEFT JOIN {0}..T_AccountDetail LocationCode on LocationCode.Adt_AccountCode = Ell_LocationCode
						and LocationCode.Adt_AccountType = ''ZIPCODE''
                        {1}

                        UNION ALL

                        SELECT  {0}..T_EmployeeLogLedgerHist.Ell_EmployeeId as [IDNumber]
		                        , Emt_LastName  + '', '' + Emt_FirstName + '' '' +  Left(Emt_Middlename,1 ) + ''.'' as Fullname
		                        , {0}..T_EmployeeMasterHist.Emt_CostCenterCode + '' - '' + dbo.getCostCenterFullNameV2({0}..T_EmployeeMasterHist.Emt_CostCenterCode) as Costcenter
		                        , Ell_PayPeriod as [ThisPeriod]
		                        , CONVERT(char(10), Ell_ProcessDate,101) as [LogDate]
		                        , Ell_DayCode as [DayCode]
		                        , Ell_ShiftCode as [ShiftCode]
		                        , LEFT(Ell_ActualTimeIn_1,2) + '':'' + RIGHT(Ell_ActualTimeIn_1, 2) as [ActualTimeIn1]
		                        , LEFT(Ell_ActualTimeOut_1,2) +'':'' + RIGHT(Ell_ActualTimeOut_1, 2) as [ActualTimeOut1]
		                        , LEFT(Ell_ActualTimeIn_2, 2) + '':'' + RIGHT(Ell_ActualTimeIn_2, 2) as [ActualTimeIn2]
		                        , LEFT(Ell_ActualTimeOut_2, 2) + '':'' + RIGHT(Ell_ActualTimeOut_2, 2) as [ActualTimeOut2]
		                        , LEFT(Scm_ShiftTimeIn, 2) + '':'' + RIGHT(Scm_ShiftTimeIn, 2) as [Shift Time In]
		                        , LEFT(Scm_ShiftBreakStart, 2) + '':'' + RIGHT(Scm_ShiftBreakStart, 2) as [StartofBreak]
		                        , LEFT(Scm_ShiftBreakEnd, 2) + '':'' + RIGHT(Scm_ShiftBreakEnd, 2) as [EndofBreak]
		                        , LEFT(Scm_ShiftTimeOut, 2) + '':'' + RIGHT(Scm_ShiftTimeOut, 2) as [ShiftTimeOut]
		                        , Ell_WorkType as [WorkType]
		                        , Ell_WorkGroup as [WorkGroup]
		                        , LocationCode.Adt_AccountDesc as [Location]
		                        , PayrollType.Adt_AccountDesc as [PayrollType] 
		                        , EmploymentStatus.Adt_AccountDesc AS [EmploymentStatus]
		                        , JobStatus.Adt_AccountDesc AS [JobStatus]
		                        , Case when Ell_AssumedPostBack = ''T'' then ''TEMP LOGS''
			                         when Ell_AssumedPostBack = ''A'' and Ell_ActualTimeIn_1 + Ell_ActualTimeOut_1 + Ell_ActualTimeIn_2 + Ell_ActualTimeOut_2 = ''0000000000000000'' then ''ACTUAL NO LOGS''
			                        when Ell_AssumedPostBack = ''A'' and Ell_ActualTimeIn_1 + Ell_ActualTimeOut_1 + Ell_ActualTimeIn_2 + Ell_ActualTimeOut_2 <> ''0000000000000000'' then ''ACTUAL WITH LOGS''
			                        else '' '' end as [Remarks]
                        FROM {0}..T_EmployeeLogLedgerHist
                        INNER JOIN {0}..T_EmployeeMaster on {0}..T_EmployeeMaster.Emt_EmployeeID = {0}..T_EmployeeLogLedgerHist.Ell_EmployeeId
                        INNER JOIN {0}..T_EmployeeMasterHist on {0}..T_EmployeeMasterHist.Emt_EmployeeID = {0}..T_EmployeeLogLedgerHist.Ell_EmployeeId
	                        AND {0}..T_EmployeeMasterHist.Emt_Payperiod = {0}..T_EmployeeLogLedgerHist.Ell_PayPeriod
                        INNER JOIN  {0}..T_ShiftCodeMaster on Scm_ShiftCode = Ell_ShiftCode
                        LEFT  JOIN {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = {0}..T_EmployeeMasterHist.Emt_PayrollType 
	                        AND  PayrollType.Adt_AccountType = ''PAYTYPE''
                        LEFT JOIN  {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = {0}..T_EmployeeMasterHist.Emt_EmploymentStatus 
	                        AND EmploymentStatus.Adt_AccountType = ''EMPSTAT''
                        LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = {0}..T_EmployeeMasterHist.Emt_JobStatus    
	                        AND JobStatus.Adt_AccountType = ''JOBSTATUS''
                        LEFT JOIN {0}..T_AccountDetail LocationCode on LocationCode.Adt_AccountCode = Ell_LocationCode
						and LocationCode.Adt_AccountType = ''ZIPCODE''
                        {1}
                        ";
                    #endregion
                        break;
                    case "Overtime Report":
                        #region query
                        query = @"
                        SELECT Eot_EmployeeId as IDNumber
		                        , Emt_LastName  + '', '' + Emt_FirstName + '' '' +  Left(Emt_Middlename,1 ) + ''.'' as Fullname
		                        , Eot_CostCenter + '' - '' + dbo.getCostCenterFullNameV2(Eot_CostCenter) as Costcenter
		                        , Eot_CurrentPayPeriod as ThisPayperiod
		                        , Convert(Date,Eot_OvertimeDate) as OvertimeDate
		                        , Eot_AppliedDate as AppliedDate
		                        , CASE Eot_OvertimeType when ''A'' then ''ADVANCE''
			                        when ''M'' then ''MID''
			                        when ''P'' then ''POST''
			                        else '''' END as OvertimeType
		                        , LEFT(Eot_StartTime, 2) + '':'' + RIGHT(Eot_StartTime,2) as StartTime
		                        , LEFT(Eot_EndTime, 2) + '':'' + RIGHT(Eot_EndTime, 2) as EndTime
		                        , Eot_OvertimeHour as OvertimeHours
		                        , Eot_Reason as ReasonofOvertime
		                        , Eot_EndorsedDateToChecker as EndorseDatetoChecker
		                        , Eot_CheckedBy as Checker1
		                        , CASE WHEN UserMaster2.Mur_UserCode IS NULL THEN Eot_CheckedBy ELSE Rtrim(UserMaster2.Umt_userlname)  + '', '' + RTrim(UserMaster2.Umt_userfname) END AS Checker1Name
		                        , Eot_CheckedDate as CheckDate1
		                        , Eot_Checked2By as Checker2
		                        , CASE WHEN UserMaster3.Mur_UserCode IS NULL THEN Eot_Checked2By ELSE Rtrim(UserMaster3.Umt_userlname)  +  '', '' + RTrim(UserMaster3.Umt_userfname) END AS Checker2Name
		                        , Eot_Checked2Date as CheckDate2
		                        , Eot_ApprovedBy as Approver
		                        , CASE WHEN UserMaster4.Mur_UserCode IS NULL THEN Eot_ApprovedBy ELSE Rtrim(UserMaster4.Umt_userlname)  + '', '' + RTrim(UserMaster4.Umt_userfname) END AS ApproverName  
		                        , Eot_ApprovedDate as ApproveDate
		                        , WFStatus.Adt_AccountDesc as Status
		                        , Eot_ControlNo as TransactionControlNo
		                        , Eot_BatchNo as BatchControlNo
		                        , CycIndic.Adt_AccountDesc as OvertimeFlag
		                        , PayrollType.Adt_AccountDesc as PayrollType 
		                        , EmploymentStatus.Adt_AccountDesc AS EmploymentStatus
		                        , JobStatus.Adt_AccountDesc AS JobStatus
                        From {0}..T_EmployeeOvertime
                        INNER JOIN {0}..T_EmployeeMaster on Emt_EmployeeID = Eot_EmployeeID
                        LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = Emt_PayrollType 
	                        AND  PayrollType.Adt_AccountType = ''PAYTYPE''
                        LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = Emt_EmploymentStatus 
	                        AND EmploymentStatus.Adt_AccountType = ''EMPSTAT''
                        LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = Emt_JobStatus    
	                        AND JobStatus.Adt_AccountType = ''JOBSTATUS''
                        LEFT JOIN  {0}..T_AccountDetail  WFStatus ON WFStatus.Adt_AccountCode = Eot_Status    
	                        AND WFStatus.Adt_AccountType = ''WFSTATUS''
                        LEFT JOIN  {0}..T_AccountDetail  CycIndic ON CycIndic.Adt_AccountCode = Eot_OvertimeFlag
	                        AND CycIndic.Adt_AccountType = ''CYCLEINDIC''
                        LEFT JOIN  {0}..M_User AS UserMaster2 ON UserMaster2.Mur_UserCode = Eot_CheckedBy
                        LEFT JOIN  {0}..M_User AS UserMaster3 ON UserMaster3.Mur_UserCode = Eot_Checked2By
                        LEFT JOIN  {0}..M_User AS UserMaster4 ON UserMaster4.Mur_UserCode = Eot_ApprovedBy 
                        {1}

                        UNION ALL

                        SELECT 	 Eot_EmployeeId as IDNumber
		                        , Emt_LastName  + '', '' + Emt_FirstName + '' '' +  Left(Emt_Middlename,1 ) + ''.'' as Fullname
		                        , Eot_CostCenter + '' - '' + dbo.getCostCenterFullNameV2(Eot_CostCenter) as Costcenter
		                        , Eot_CurrentPayPeriod as ThisPayperiod
		                        , Convert(Date,Eot_OvertimeDate) as OvertimeDate
		                        , Eot_AppliedDate as AppliedDate
		                        , CASE Eot_OvertimeType when ''A'' then ''ADVANCE''
			                        when ''M'' then ''MID''
			                        when ''P'' then ''POST''
			                        else '''' END as OvertimeType
		                        , LEFT(Eot_StartTime, 2) + '':'' + RIGHT(Eot_StartTime,2) as StartTime
		                        , LEFT(Eot_EndTime, 2) + '':'' + RIGHT(Eot_EndTime, 2) as EndTime
		                        , Eot_OvertimeHour as OvertimeHours
		                        , Eot_Reason as ReasonofOvertime
		                        , Eot_EndorsedDateToChecker as EndorseDatetoChecker
		                        , Eot_CheckedBy as Checker1
		                        , CASE WHEN UserMaster2.Mur_UserCode IS NULL THEN Eot_CheckedBy ELSE Rtrim(UserMaster2.Umt_userlname)  + '', '' + RTrim(UserMaster2.Umt_userfname) END AS Checker1Name
		                        , Eot_CheckedDate as CheckDate1
		                        , Eot_Checked2By as Checker2
		                        , CASE WHEN UserMaster3.Mur_UserCode IS NULL THEN Eot_Checked2By ELSE Rtrim(UserMaster3.Umt_userlname)  +  '', '' + RTrim(UserMaster3.Umt_userfname) END AS Checker2Name
		                        , Eot_Checked2Date as CheckDate2
		                        , Eot_ApprovedBy as Approver
		                        , CASE WHEN UserMaster4.Mur_UserCode IS NULL THEN Eot_ApprovedBy ELSE Rtrim(UserMaster4.Umt_userlname)  + '', '' + RTrim(UserMaster4.Umt_userfname) END AS ApproverName  
		                        , Eot_ApprovedDate as ApproveDate
		                        , WFStatus.Adt_AccountDesc as Status
		                        , Eot_ControlNo as TransactionControlNo
		                        , Eot_BatchNo as BatchControlNo
		                        , CycIndic.Adt_AccountDesc as OvertimeFlag
		                        , PayrollType.Adt_AccountDesc as PayrollType 
		                        , EmploymentStatus.Adt_AccountDesc AS EmploymentStatus
		                        , JobStatus.Adt_AccountDesc AS JobStatus
                        From {0}..T_EmployeeOvertimeHist
                        INNER JOIN {0}..T_EmployeeMaster on Emt_EmployeeID = Eot_EmployeeID
                        LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = Emt_PayrollType 
	                        AND  PayrollType.Adt_AccountType = ''PAYTYPE''
                        LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = Emt_EmploymentStatus 
	                        AND EmploymentStatus.Adt_AccountType = ''EMPSTAT''
                        LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = Emt_JobStatus    
	                        AND JobStatus.Adt_AccountType = ''JOBSTATUS''
                        LEFT JOIN  {0}..T_AccountDetail  WFStatus ON WFStatus.Adt_AccountCode = Eot_Status    
	                        AND WFStatus.Adt_AccountType = ''WFSTATUS''
                        LEFT JOIN  {0}..T_AccountDetail  CycIndic ON CycIndic.Adt_AccountCode = Eot_OvertimeFlag
	                        AND CycIndic.Adt_AccountType = ''CYCLEINDIC''
                        LEFT JOIN  {0}..M_User AS UserMaster2 ON UserMaster2.Mur_UserCode = Eot_CheckedBy
                        LEFT JOIN  {0}..M_User AS UserMaster3 ON UserMaster3.Mur_UserCode = Eot_Checked2By
                        LEFT JOIN  {0}..M_User AS UserMaster4 ON UserMaster4.Mur_UserCode = Eot_ApprovedBy
                        {1}
                        ";
                        #endregion
                        break;
                    case "Leave Availment Report":
                        #region query
                        query = @"
                        SELECT Elt_EmployeeId as IDNumber
		                        , Emt_LastName  + '', '' + Emt_FirstName + '' '' +  Left(Emt_Middlename,1 ) + ''.'' as Fullname
		                        , Elt_CostCenter + '' - '' + dbo.getCostCenterFullNameV2(Elt_CostCenter) as Costcenter
		                        , Elt_CurrentPayPeriod as ThisPayperiod
		                        , Convert(Date,Elt_LeaveDate) as LeaveDate
		                        , Elt_InformDate as InformDate
		                        , Elt_AppliedDate as AppliedDate
		                        , Elt_LeaveType as LeaveType
		                        , Elt_LeaveCategory as LeaveCategory
		                        , Elt_Leavecode as LeaveCode
		                        , LEFT(Elt_StartTime, 2) + '':'' + RIGHT(Elt_StartTime,2) as StartTime
		                        , LEFT(Elt_EndTime, 2) + '':'' + RIGHT(Elt_EndTime, 2) as EndTime
		                        , Elt_LeaveHour as LeaveHours
		                        , Elt_DayUnit as DayUnit
		                        , CASE WHEN Elt_LeaveNotice = 1 THEN ''YES'' ELSE ''NO'' END	as WithLeaveNotice
		                        , Elt_Reason as ReasonofLeave
		                        , Elt_EndorsedDateToChecker as EndorseDatetoChecker
		                        , Elt_CheckedBy as Checker1
		                        , CASE WHEN UserMaster2.Mur_UserCode IS NULL THEN Elt_CheckedBy ELSE Rtrim(UserMaster2.Umt_userlname)  + '', '' + RTrim(UserMaster2.Umt_userfname) END AS Checker1Name
		                        , Elt_CheckedDate as CheckDate1
		                        , Elt_Checked2By as Checker2
		                        , CASE WHEN UserMaster3.Mur_UserCode IS NULL THEN Elt_Checked2By ELSE Rtrim(UserMaster3.Umt_userlname)  +  '', '' + RTrim(UserMaster3.Umt_userfname) END AS Checker2Name
		                        , Elt_Checked2Date as CheckDate2
		                        , Elt_ApprovedBy as Approver
		                        , CASE WHEN UserMaster4.Mur_UserCode IS NULL THEN Elt_ApprovedBy ELSE Rtrim(UserMaster4.Umt_userlname)  + '', '' + RTrim(UserMaster4.Umt_userfname) END AS ApproverName  
		                        , Elt_ApprovedDate as ApproveDate
		                        , WFStatus.Adt_AccountDesc as Status
		                        , Elt_ControlNo as TransactionControlNo
		                        , CycIndic.Adt_AccountDesc as LeaveFlag
		                        , PayrollType.Adt_AccountDesc as PayrollType 
		                        , EmploymentStatus.Adt_AccountDesc AS EmploymentStatus
		                        , JobStatus.Adt_AccountDesc AS JobStatus
                        From {0}..T_EmployeeLeaveAvailment
                        INNER JOIN {0}..T_EmployeeMaster on Emt_EmployeeID = Elt_EmployeeID
                        LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = Emt_PayrollType 
	                        AND  PayrollType.Adt_AccountType = ''PAYTYPE''
                        LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = Emt_EmploymentStatus 
	                        AND EmploymentStatus.Adt_AccountType = ''EMPSTAT''
                        LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = Emt_JobStatus    
	                        AND JobStatus.Adt_AccountType = ''JOBSTATUS''
                        LEFT JOIN  {0}..T_AccountDetail  WFStatus ON WFStatus.Adt_AccountCode = Elt_Status    
	                        AND WFStatus.Adt_AccountType = ''WFSTATUS''
                        LEFT JOIN  {0}..T_AccountDetail  CycIndic ON CycIndic.Adt_AccountCode = Elt_LeaveFlag
	                        AND CycIndic.Adt_AccountType = ''CYCLEINDIC''
                        LEFT JOIN  {0}..M_User AS UserMaster2 ON UserMaster2.Mur_UserCode = Elt_CheckedBy
                        LEFT JOIN  {0}..M_User AS UserMaster3 ON UserMaster3.Mur_UserCode = Elt_Checked2By
                        LEFT JOIN  {0}..M_User AS UserMaster4 ON UserMaster4.Mur_UserCode = Elt_ApprovedBy 
                        -- Add here the WHERE CLAUSE

                        UNION ALL

                        SELECT Elt_EmployeeId as IDNumber
		                        , Emt_LastName  + '', '' + Emt_FirstName + '' '' +  Left(Emt_Middlename,1 ) + ''.'' as Fullname
		                        , Elt_CostCenter + '' - '' + dbo.getCostCenterFullNameV2(Elt_CostCenter) as Costcenter
		                        , Elt_CurrentPayPeriod as ThisPayperiod
		                        , Convert(Date,Elt_LeaveDate) as LeaveDate
		                        , Elt_InformDate as InformDate
		                        , Elt_AppliedDate as AppliedDate
		                        , Elt_LeaveType as LeaveType
		                        , Elt_LeaveCategory as LeaveCategory
		                        , Elt_Leavecode as LeaveCode
		                        , LEFT(Elt_StartTime, 2) + '':'' + RIGHT(Elt_StartTime,2) as StartTime
		                        , LEFT(Elt_EndTime, 2) + '':'' + RIGHT(Elt_EndTime, 2) as EndTime
		                        , Elt_LeaveHour as LeaveHours
		                        , Elt_DayUnit as DayUnit
		                        , CASE WHEN Elt_LeaveNotice = 1 THEN ''YES'' ELSE ''NO'' END	as WithLeaveNotice
		                        , Elt_Reason as ReasonofLeave
		                        , Elt_EndorsedDateToChecker as EndorseDatetoChecker
		                        , Elt_CheckedBy as Checker1
		                        , CASE WHEN UserMaster2.Mur_UserCode IS NULL THEN Elt_CheckedBy ELSE Rtrim(UserMaster2.Umt_userlname)  + '', '' + RTrim(UserMaster2.Umt_userfname) END AS Checker1Name
		                        , Elt_CheckedDate as CheckDate1
		                        , Elt_Checked2By as Checker2
		                        , CASE WHEN UserMaster3.Mur_UserCode IS NULL THEN Elt_Checked2By ELSE Rtrim(UserMaster3.Umt_userlname)  +  '', '' + RTrim(UserMaster3.Umt_userfname) END AS Checker2Name
		                        , Elt_Checked2Date as CheckDate2
		                        , Elt_ApprovedBy as Approver
		                        , CASE WHEN UserMaster4.Mur_UserCode IS NULL THEN Elt_ApprovedBy ELSE Rtrim(UserMaster4.Umt_userlname)  + '', '' + RTrim(UserMaster4.Umt_userfname) END AS ApproverName  
		                        , Elt_ApprovedDate as ApproveDate
		                        , WFStatus.Adt_AccountDesc as Status
		                        , Elt_ControlNo as TransactionControlNo
		                        , CycIndic.Adt_AccountDesc as LeaveFlag
		                        , PayrollType.Adt_AccountDesc as PayrollType 
		                        , EmploymentStatus.Adt_AccountDesc AS EmploymentStatus
		                        , JobStatus.Adt_AccountDesc AS JobStatus
                        From {0}..T_EmployeeLeaveAvailmentHist
                        INNER JOIN {0}..T_EmployeeMaster on Emt_EmployeeID = Elt_EmployeeID
                        LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = Emt_PayrollType 
	                        AND  PayrollType.Adt_AccountType = ''PAYTYPE''
                        LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = Emt_EmploymentStatus 
	                        AND EmploymentStatus.Adt_AccountType = ''EMPSTAT''
                        LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = Emt_JobStatus    
	                        AND JobStatus.Adt_AccountType = ''JOBSTATUS''
                        LEFT JOIN  {0}..T_AccountDetail  WFStatus ON WFStatus.Adt_AccountCode = Elt_Status    
	                        AND WFStatus.Adt_AccountType = ''WFSTATUS''
                        LEFT JOIN  {0}..T_AccountDetail  CycIndic ON CycIndic.Adt_AccountCode = Elt_LeaveFlag
	                        AND CycIndic.Adt_AccountType = ''CYCLEINDIC''
                        LEFT JOIN  {0}..M_User AS UserMaster2 ON UserMaster2.Mur_UserCode = Elt_CheckedBy
                        LEFT JOIN  {0}..M_User AS UserMaster3 ON UserMaster3.Mur_UserCode = Elt_Checked2By
                        LEFT JOIN  {0}..M_User AS UserMaster4 ON UserMaster4.Mur_UserCode = Elt_ApprovedBy
                        ";
                        #endregion
                        break;
                    case "Leave Ledger Report":
                        #region query
                        if (category)
                        {
                            query = @"
                            SELECT 	 Elm_EmployeeId as IDNumber
				                            , Emt_LastName  + '', '' + Emt_FirstName + '' '' +  Left(Emt_Middlename,1 ) + ''.'' as Fullname
				                            , Emt_CostCenterCode + '' - '' + dbo.getCostCenterFullNameV2(Emt_CostCenterCode) as Costcenter
				                            , Elm_LeaveYear as LeaveYear
				                            , Elm_Leavetype as LeaveType
				                            , Elm_Entitled as Entitled
				                            , Elm_Used as Used
				                            , Elm_Reserved as Reserved
				                            , Elm_Entitled - Elm_Used as Balance
				                            , PayrollType.Adt_AccountDesc as PayrollType 
				                            , EmploymentStatus.Adt_AccountDesc AS EmploymentStatus
				                            , JobStatus.Adt_AccountDesc AS JobStatus
		                            From {0}..T_EmployeeLeave
		                            INNER JOIN {0}..T_EmployeeMaster on Emt_EmployeeID = Elm_EmployeeID
		                            LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = Emt_PayrollType 
			                            AND  PayrollType.Adt_AccountType = ''PAYTYPE''
		                            LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = Emt_EmploymentStatus 
			                            AND EmploymentStatus.Adt_AccountType = ''EMPSTAT''
		                            LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = Emt_JobStatus    
			                            AND JobStatus.Adt_AccountType = ''JOBSTATUS''
		                            {1}

		                            UNION ALL

		                            SELECT 	 Elm_EmployeeId as IDNumber
				                            , Emt_LastName  + '', '' + Emt_FirstName + '' '' +  Left(Emt_Middlename,1 ) + ''.'' as Fullname
				                            , Emt_CostCenterCode + '' - '' + dbo.getCostCenterFullNameV2(Emt_CostCenterCode) as Costcenter
				                            , Elm_LeaveYear as LeaveYear
				                            , Elm_Leavetype as LeaveType
				                            , Elm_Entitled as Entitled
				                            , Elm_Used as Used
				                            , Elm_Reserved as Reserved
				                            , Elm_Entitled - Elm_Used as Balance
				                            , PayrollType.Adt_AccountDesc as PayrollType 
				                            , EmploymentStatus.Adt_AccountDesc AS EmploymentStatus
				                            , JobStatus.Adt_AccountDesc AS JobStatus
		                            From {0}..T_EmployeeLeaveHist
		                            INNER JOIN {0}..T_EmployeeMaster on Emt_EmployeeID = Elm_EmployeeID
		                            LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = Emt_PayrollType 
			                            AND  PayrollType.Adt_AccountType = ''PAYTYPE''
		                            LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = Emt_EmploymentStatus 
			                            AND EmploymentStatus.Adt_AccountType = ''EMPSTAT''
		                            LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = Emt_JobStatus    
			                            AND JobStatus.Adt_AccountType = ''JOBSTATUS''
			                            {1}
                            ";
                        }
                        else
                        {
                            query = @"
                            SELECT 	 Elm_EmployeeId as IDNumber
	   			                            , Emt_LastName  + '', '' + Emt_FirstName + '' '' +  Left(Emt_Middlename,1 ) + ''.'' as Fullname
				                            , Emt_CostCenterCode + '' - '' + dbo.getCostCenterFullNameV2(Emt_CostCenterCode) as Costcenter
				                            , Elm_LeaveYear as LeaveYear
				                            , Elm_Leavetype as LeaveType
				                            , Elm_Entitled as Entitled
				                            , Elm_Used/8.000 as Used
				                            , Elm_Reserved/8.000 as Reserved
				                            , (Elm_Entitled - Elm_Used)/8.000 as Balance
				                            , PayrollType.Adt_AccountDesc as PayrollType 
				                            , EmploymentStatus.Adt_AccountDesc AS EmploymentStatus
				                            , JobStatus.Adt_AccountDesc AS JobStatus
		                            From {0}..T_EmployeeLeave
		                            INNER JOIN {0}..T_EmployeeMaster on Emt_EmployeeID = Elm_EmployeeID
		                            LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = Emt_PayrollType 
			                            AND  PayrollType.Adt_AccountType = ''PAYTYPE''
		                            LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = Emt_EmploymentStatus 
			                            AND EmploymentStatus.Adt_AccountType = ''EMPSTAT''
		                            LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = Emt_JobStatus    
			                            AND JobStatus.Adt_AccountType = ''JOBSTATUS''
		                            {1}
                            		
		                            UNION ALL
                            		
	                               SELECT 	 Elm_EmployeeId as IDNumber
				                            , Emt_LastName  + '', '' + Emt_FirstName + '' '' +  Left(Emt_Middlename,1 ) + ''.'' as Fullname
				                            , Emt_CostCenterCode + '' - '' + dbo.getCostCenterFullNameV2(Emt_CostCenterCode) as Costcenter	   
				                            , Elm_LeaveYear as LeaveYear
				                            , Elm_Leavetype as LeaveType
				                            , Elm_Entitled/8.000 as Entitled
				                            , Elm_Used/8.000 as Used
				                            , Elm_Reserved/8.000 as Reserved
				                            , (Elm_Entitled - Elm_Used)/8.000 as Balance
				                            , PayrollType.Adt_AccountDesc as PayrollType 
				                            , EmploymentStatus.Adt_AccountDesc AS EmploymentStatus
				                            , JobStatus.Adt_AccountDesc AS JobStatus
		                            From {0}..T_EmployeeLeaveHist
		                            INNER JOIN {0}..T_EmployeeMaster on Emt_EmployeeID = Elm_EmployeeID
		                            LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = Emt_PayrollType 
			                            AND  PayrollType.Adt_AccountType = ''PAYTYPE''
		                            LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = Emt_EmploymentStatus 
			                            AND EmploymentStatus.Adt_AccountType = ''EMPSTAT''
		                            LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = Emt_JobStatus    
			                            AND JobStatus.Adt_AccountType = ''JOBSTATUS''
			                            {1}
                            ";
                        }
                        #endregion
                        break;
                    case "DTR Report":
                        #region query
                        query = @"
                        SELECT Tel_IDNo AS IDNumber
	                        , Emt_LastName  + '', '' + Emt_FirstName + '' '' +  Left(Emt_Middlename,1 ) + ''.'' as Fullname
	                        , Emt_CostCenterCode + '' - '' + dbo.getCostCenterFullNameV2(Emt_CostCenterCode) as Costcenter
	                        , CONVERT(date, Tel_LogDate) AS LogDate
	                        , Tel_LogTime AS LogTime
	                        , Tel_LogType AS LogType
	                        , Tel_StationNo AS StationNumber  
	                        , PayrollType.Adt_AccountDesc as PayrollType 
	                        , EmploymentStatus.Adt_AccountDesc AS EmploymentStatus
	                        , JobStatus.Adt_AccountDesc AS JobStatus
                        FROM  {0}..T_EmpDTR DTR
                        INNER JOIN {1}..T_EmployeeMaster ON Emt_EmployeeID = Tel_IDNo  
                        LEFT  JOIN  {1}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = Emt_PayrollType 
	                        AND  PayrollType.Adt_AccountType = ''PAYTYPE''
                        LEFT JOIN {1}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = Emt_EmploymentStatus 
	                        AND EmploymentStatus.Adt_AccountType = ''EMPSTAT''
                        LEFT JOIN  {1}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = Emt_JobStatus    
	                        AND JobStatus.Adt_AccountType = ''JOBSTATUS''
	                        {2}
                        ";
                        #endregion
                        break;
                    case "Work Location Report":
                        #region query
                        query = @"
                        SELECT  Ewl_EmployeeId as IDNumber
		                        , Emt_LastName  + '', '' + Emt_FirstName + '' '' +  Left(Emt_Middlename,1 ) + ''.'' as Fullname
		                        , Emt_CostCenterCode + '' - '' + dbo.getCostCenterFullNameV2(Emt_CostCenterCode) as Costcenter
		                        , CONVERT(date, Ewl_EffectivityDate) as EffectivityDate
		                        , Location.Adt_AccountDesc AS WorkLocation
		                        , PayrollType.Adt_AccountDesc as PayrollType 
		                        , EmploymentStatus.Adt_AccountDesc AS EmploymentStatus
		                        , JobStatus.Adt_AccountDesc AS JobStatus
                        FROM {0}..T_Employeeworklocation
                        INNER JOIN {0}..T_EmployeeMaster on Emt_EmployeeID = Ewl_EmployeeId
                        LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = Emt_PayrollType 
	                        AND  PayrollType.Adt_AccountType = ''PAYTYPE''
                        LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = Emt_EmploymentStatus 
	                        AND EmploymentStatus.Adt_AccountType = ''EMPSTAT''
                        LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = Emt_JobStatus    
	                        AND JobStatus.Adt_AccountType = ''JOBSTATUS''
                        LEFT JOIN  {0}..T_AccountDetail  Location ON Location.Adt_AccountCode = Ewl_LocationCode    
	                        AND Location.Adt_AccountType = ''ZIPCODE''
	                        {1}
                        ";
                        #endregion
                        break;
                    case "Restday Report":
                        #region query
                        query = @"
                        SELECT  Erd_EmployeeId as IDNumber
		                        , Emt_LastName  + '', '' + Emt_FirstName + '' '' +  Left(Emt_Middlename,1 ) + ''.'' as Fullname
		                        , Emt_CostCenterCode + '' - '' + dbo.getCostCenterFullNameV2(Emt_CostCenterCode) as Costcenter
		                        , CONVERT(date, Erd_EffectivityDate) as EffectivityDate
		                        , Rtrim(CASE WHEN Left(Erd_RestDay,1) = ''1'' then ''- Mon'' ELSE '''' END +      
		                        CASE WHEN LEN(CASE WHEN Substring(Erd_RestDay,2,1) = ''1'' then ''Tue'' ELSE '''' END) = 0 then '''' ELSE ''- Tue'' END +      
		                        CASE WHEN LEN(CASE WHEN Substring(Erd_RestDay,3,1) = ''1'' then ''Wed'' ELSE '''' END) = 0 then '''' ELSE ''- Wed'' END +      
		                        CASE WHEN LEN(CASE WHEN Substring(Erd_RestDay,4,1) = ''1'' then ''Thu'' ELSE '''' END) = 0 then '''' ELSE ''- Thu'' END +      
		                        CASE WHEN LEN(CASE WHEN Substring(Erd_RestDay,5,1) = ''1'' then ''Fri'' ELSE '''' END) = 0 then '''' ELSE ''- Fri'' END +      
		                        CASE WHEN LEN(CASE WHEN Substring(Erd_RestDay,6,1) = ''1'' then ''Sat'' ELSE '''' END) = 0 then '''' ELSE ''- Sat'' END +      
		                        CASE WHEN LEN(CASE WHEN Substring(Erd_RestDay,7,1) = ''1'' then ''Sun'' ELSE '''' END) = 0 then '''' ELSE ''- Sun'' END) as RestDay
		                        , PayrollType.Adt_AccountDesc as PayrollType 
		                        , EmploymentStatus.Adt_AccountDesc AS EmploymentStatus
		                        , JobStatus.Adt_AccountDesc AS JobStatus
                        FROM {0}..T_EmployeeRestday
                        INNER JOIN {0}..T_EmployeeMaster on Emt_EmployeeID = Erd_EmployeeId
                        LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = Emt_PayrollType 
	                        AND  PayrollType.Adt_AccountType = ''PAYTYPE''
                        LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = Emt_EmploymentStatus 
	                        AND EmploymentStatus.Adt_AccountType = ''EMPSTAT''
                        LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = Emt_JobStatus    
	                        AND JobStatus.Adt_AccountType = ''JOBSTATUS''
                            {1}
                        ";
                        #endregion
                        break;
                    case "Post-Payroll Report":
                        #region query
                        query = @"
                        SELECT  Per_EmployeeId as IDNumber
		                        , Emt_LastName  + '', '' + Emt_FirstName + '' '' +  Left(Emt_Middlename,1 ) + ''.'' as Fullname
		                        , Emt_CostCenterCode + '' - '' + dbo.getCostCenterFullNameV2(Emt_CostCenterCode) as Costcenter
		                        , Per_CurrentPayPeriod as [ThisPeriod]
		                        , Per_Remarks as [Remarks]
		                        ,  CASE WHEN Per_Adjustment = 1 THEN ''YES'' ELSE ''NO'' END	as [For Adjustment] 
		                        , PayrollType.Adt_AccountDesc as PayrollType 
		                        , EmploymentStatus.Adt_AccountDesc AS EmploymentStatus
		                        , JobStatus.Adt_AccountDesc AS JobStatus
                        FROM {0}..T_PayrollError
                        INNER JOIN {0}..T_EmployeeMaster on Emt_EmployeeID = Per_EmployeeId
                        LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = Emt_PayrollType 
	                        AND  PayrollType.Adt_AccountType = ''PAYTYPE''
                        LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = Emt_EmploymentStatus 
	                        AND EmploymentStatus.Adt_AccountType = ''EMPSTAT''
                        LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = Emt_JobStatus    
	                        AND JobStatus.Adt_AccountType = ''JOBSTATUS''
	                        {1}
                        	
                        UNION ALL

                        SELECT  Per_EmployeeId as IDNumber
		                        , Emt_LastName  + '', '' + Emt_FirstName + '' '' +  Left(Emt_Middlename,1 ) + ''.'' as Fullname
		                        , {0}..T_EmployeeMasterHist.Emt_CostCenterCode + '' - '' + dbo.getCostCenterFullNameV2({0}..T_EmployeeMasterHist.Emt_CostCenterCode) as Costcenter
		                        , Per_CurrentPayPeriod as [ThisPeriod]
		                        , Per_Remarks as [Remarks]
		                        ,  CASE WHEN Per_Adjustment = 1 THEN ''YES'' ELSE ''NO'' END	as [For Adjustment] 
		                        , PayrollType.Adt_AccountDesc as PayrollType 
		                        , EmploymentStatus.Adt_AccountDesc AS EmploymentStatus
		                        , JobStatus.Adt_AccountDesc AS JobStatus
                        FROM {0}..T_PayrollErrorHist
                        INNER JOIN {0}..T_EmployeeMasterHist on {0}..T_EmployeeMasterHist.Emt_EmployeeID = Per_EmployeeId
                        and {0}..T_EmployeeMasterHist.Emt_Payperiod = Per_CurrentPayPeriod
                        INNER JOIN {0}..T_EmployeeMaster on {0}..T_EmployeeMaster.Emt_EmployeeID = Per_EmployeeId
                        LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = {0}..T_EmployeeMasterHist.Emt_PayrollType 
	                        AND  PayrollType.Adt_AccountType = ''PAYTYPE''
                        LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = {0}..T_EmployeeMasterHist.Emt_EmploymentStatus 
	                        AND EmploymentStatus.Adt_AccountType = ''EMPSTAT''
                        LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = {0}..T_EmployeeMasterHist.Emt_JobStatus    
	                        AND JobStatus.Adt_AccountType = ''JOBSTATUS''
	                        {1}
                        ";
                        #endregion
                        break;
                }
                #endregion
            }
            else
            {
                #region details
                switch (report)
                {
                    case "Other Income and Benefits Report":
                        #region query
                        query = @"
                        SELECT  Eal_EmployeeId as [IDNumber]
		                        , Emt_LastName as [Last Name]
		                        , Emt_FirstName as [First Name]
		                        , Emt_CostCenterCode as  Costcenter
		                        , dbo.getCostCenterFullNameV2(Emt_CostCenterCode) as Description
		                        , Eal_CurrentPayPeriod as [This Payperiod]
                                , Eal_AllowanceCode as [Other Income Code]
		                        , Eal_AllowanceAmt as [Amount]
		                        , CASE WHEN Acm_Recursive = 1 THEN 'YES' ELSE 'NO' END	as [Recursive]
		                        , CASE WHEN Eal_PayrollPost = 1 THEN 'YES' ELSE 'NO' END	as [Post to Payroll]
		                        , CASE WHEN Acm_TaxClass = 'T' THEN 'TAX' 
			                        When Acm_TaxClass = 'N' THEN 'NONTAX' 
			                        ELSE ' ' End as [Tax Class]
		                        , Alpha.Adt_accountdesc as [Alphalist Category]  
		                        , PayrollType.Adt_AccountDesc as [Payroll Type] 
		                        , EmploymentStatus.Adt_AccountDesc AS [Employment Status]
		                        , JobStatus.Adt_AccountDesc AS [Job Status]
		                        , Freq.Adt_AccountDesc AS [Frequency]
                        FROM {0}..T_EmployeeAllowance    
                        INNER JOIN {0}..T_EmployeeMaster on Emt_EmployeeID = Eal_EmployeeId
                        INNER JOIN  {0}..T_AllowanceCodeMaster on Acm_AllowanceCode = Eal_AllowanceCode
                        LEFT JOIN {0}..T_AccountDetail Alpha on Alpha.Adt_AccountCode = Acm_AlphalistCategory
	                        AND Alpha.Adt_AccountType = 'ALPHACATGY'
                        LEFT  JOIN {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = Emt_PayrollType 
	                        AND  PayrollType.Adt_AccountType = 'PAYTYPE'
                        LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = Emt_EmploymentStatus 
	                        AND EmploymentStatus.Adt_AccountType = 'EMPSTAT'
                        LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = Emt_JobStatus    
	                        AND JobStatus.Adt_AccountType = 'JOBSTATUS'
                        LEFT JOIN  {0}..T_AccountDetail  Freq ON Freq.Adt_AccountCode = Acm_ApplicablePayrollPeriod    
	                        AND Freq.Adt_AccountType = 'DEDNFREQ'
                        {1}

                        UNION ALL

                        SELECT  Eal_EmployeeId as [IDNumber]
		                        , Emt_LastName as [Last Name]
		                        , Emt_FirstName as [First Name]
		                        , T_EmployeeMasterHist.Emt_CostCenterCode as  Costcenter
		                        , dbo.getCostCenterFullNameV2({0}..T_EmployeeMasterHist.Emt_CostCenterCode) as Description
		                        , Eal_CurrentPayPeriod as [This Payperiod]
                                , Eal_AllowanceCode as [Other Income Code]
		                        , Eal_AllowanceAmt as [Amount]
		                        , CASE WHEN Acm_Recursive = 1 THEN 'YES' ELSE 'NO' END	as [Recursive]
		                        , CASE WHEN Eal_PayrollPost = 1 THEN 'YES' ELSE 'NO' END	as [Post to Payroll]
		                        , CASE WHEN Acm_TaxClass = 'T' THEN 'TAX' 
			                        When Acm_TaxClass = 'N' THEN 'NONTAX' 
			                        ELSE ' ' End as [Tax Class]
		                        , Alpha.Adt_accountdesc as [Alphalist Category]  
		                        , PayrollType.Adt_AccountDesc as [Payroll Type] 
		                        , EmploymentStatus.Adt_AccountDesc AS [Employment Status]
		                        , JobStatus.Adt_AccountDesc AS [Job Status]
		                        , Freq.Adt_AccountDesc AS [Frequency]
                        FROM {0}..T_EmployeeAllowanceHist    
                        INNER JOIN {0}..T_EmployeeMaster on {0}..T_EmployeeMaster.Emt_EmployeeID = Eal_EmployeeId
                        INNER JOIN {0}..T_EmployeeMasterHist on {0}..T_EmployeeMasterHist.Emt_EmployeeID = Eal_EmployeeId
	                        AND Emt_PayPeriod = Eal_CurrentPayPeriod
                        INNER JOIN  {0}..T_AllowanceCodeMaster on Acm_AllowanceCode = Eal_AllowanceCode
                        LEFT JOIN {0}..T_AccountDetail Alpha on Alpha.Adt_AccountCode = Acm_AlphalistCategory
	                        AND Alpha.Adt_AccountType = 'ALPHACATGY'
                        LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = {0}..T_EmployeeMaster.Emt_PayrollType 
	                        AND  PayrollType.Adt_AccountType = 'PAYTYPE'
                        LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = {0}..T_EmployeeMaster.Emt_EmploymentStatus 
	                        AND EmploymentStatus.Adt_AccountType = 'EMPSTAT'
                        LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = {0}..T_EmployeeMaster.Emt_JobStatus    
	                        AND JobStatus.Adt_AccountType = 'JOBSTATUS'
                        LEFT JOIN  {0}..T_AccountDetail  Freq ON Freq.Adt_AccountCode = Acm_ApplicablePayrollPeriod    
	                        AND Freq.Adt_AccountType = 'DEDNFREQ'
                       {1}
                            ";
                        #endregion
                        break;                    
                    case "Deduction and Loan Report":
                        if (category)
                        {
                            #region query
                            query = @"                           
                            SELECT  Edd_EmployeeID as [IDNumber]
		                            , Emt_LastName as [Last Name]
		                            , Emt_FirstName as [First Name]
		                            , Emt_CostCenterCode as  [Costcenter]
		                            , dbo.getCostCenterFullNameV2(Emt_CostCenterCode) as [Description]
		                            , Edd_DeductionCode as [Deduction Code]
		                            , Convert(Char(10),Edd_StartDeductionDate, 101) as [Start of Deduction]
		                            , Edd_Amount as [Amount]
		                            , CASE WHEN Edd_FromDeferred = 1 THEN 'YES' ELSE 'NO' END	as [From Deferred]
		                            , CASE WHEN Edd_PaymentFlag = 1 THEN 'YES' ELSE 'NO' END	as [Post to Payroll]
		                            , CASE WHEN Dcm_WithCheck = 1 THEN 'YES' ELSE 'NO' END	as [With Check]
		                            , ISNULL(Convert(Char(10),Edl_CheckDate, 101), '') as [Check Date]
		                            , CASE WHEN Dcm_WithVoucher = 1 THEN 'YES' ELSE 'NO' END	as [With Voucher]
		                            , Edl_VoucherNumber as [Voucher Number]
		                            , PayrollType.Adt_AccountDesc as [Payroll Type] 
		                            , EmploymentStatus.Adt_AccountDesc AS [Employment Status]
		                            , JobStatus.Adt_AccountDesc AS [Job Status]
		                            , DeductionType.Adt_AccountDesc AS [Deduction Type]
		                            , ISNULL(DeductionPayType.Adt_AccountDesc, '') AS [Payment Type]
                            FROM {0}..T_EmployeeDeductionDetail
                            INNER JOIN {0}..T_EmployeeDeductionLedger on Edl_EmployeeID =  Edd_EmployeeID
	                            AND Edl_DeductionCode = Edd_DeductionCode
	                            AND Edl_StartDeductionDate = Edd_StartDeductionDate
                            INNER JOIN {0}..T_EmployeeMaster on Emt_EmployeeID = Edd_EmployeeID
                            INNER JOIN  {0}..T_DeductionCodeMaster on Dcm_DeductionCode = Edd_DeductionCode
                            LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = Emt_PayrollType 
	                            AND  PayrollType.Adt_AccountType = 'PAYTYPE'
                            LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = Emt_EmploymentStatus 
	                            AND EmploymentStatus.Adt_AccountType = 'EMPSTAT'
                            LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = Emt_JobStatus    
	                            AND JobStatus.Adt_AccountType = 'JOBSTATUS'
                            LEFT JOIN  {0}..T_AccountDetail  DeductionType ON DeductionType.Adt_AccountCode = Dcm_DeductionType    
	                            AND DeductionType.Adt_AccountType = 'DEDNTYPE'
                            LEFT JOIN  {0}..T_AccountDetail  DeductionPayType ON DeductionPayType.Adt_AccountCode = Edd_PaymentType    
	                            AND DeductionPayType.Adt_AccountType = 'DEDPAYTYPE'
                            {1}

                            UNION ALL

                            SELECT  Edd_EmployeeID as [IDNumber]
		                            , Emt_LastName as [Last Name]
		                            , Emt_FirstName as [First Name]
		                            , Emt_CostCenterCode as  [Costcenter]
		                            , dbo.getCostCenterFullNameV2(Emt_CostCenterCode) as [Description]
		                            , Edd_DeductionCode as [Deduction Code]
		                            , Convert(Char(10),Edd_StartDeductionDate, 101) as [Start of Deduction]
		                            , Edd_Amount as [Amount]
		                            , CASE WHEN Edd_FromDeferred = 1 THEN 'YES' ELSE 'NO' END	as [From Deferred]
		                            , CASE WHEN Edd_PaymentFlag = 1 THEN 'YES' ELSE 'NO' END	as [Post to Payroll]
		                            , CASE WHEN Dcm_WithCheck = 1 THEN 'YES' ELSE 'NO' END	as [With Check]
		                            , ISNULL(Convert(Char(10),Edl_CheckDate, 101), '') as [Check Date]
		                            , CASE WHEN Dcm_WithVoucher = 1 THEN 'YES' ELSE 'NO' END	as [With Voucher]
		                            , Edl_VoucherNumber as [Voucher Number]
		                            , PayrollType.Adt_AccountDesc as [Payroll Type] 
		                            , EmploymentStatus.Adt_AccountDesc AS [Employment Status]
		                            , JobStatus.Adt_AccountDesc AS [Job Status]
		                            , DeductionType.Adt_AccountDesc AS [Deduction Type]
		                            , ISNULL(DeductionPayType.Adt_AccountDesc, '') AS [Payment Type]
                            FROM {0}..T_EmployeeDeductionDetailHist
                            INNER JOIN {0}..T_EmployeeDeductionLedger on Edl_EmployeeID =  Edd_EmployeeID
	                            AND Edl_DeductionCode = Edd_DeductionCode
	                            AND Edl_StartDeductionDate = Edd_StartDeductionDate
                            INNER JOIN {0}..T_EmployeeMaster on {0}..T_EmployeeMaster.Emt_EmployeeID = Edd_EmployeeID
                            INNER JOIN  {0}..T_PayPeriodMaster on Ppm_PayPeriod =  Edd_PayPeriod
	                            AND Ppm_CycleIndicator = 'C'
                            INNER JOIN  {0}..T_DeductionCodeMaster on Dcm_DeductionCode = Edd_DeductionCode
                            LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = Emt_PayrollType 
	                            AND  PayrollType.Adt_AccountType = 'PAYTYPE'
                            LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = Emt_EmploymentStatus 
	                            AND EmploymentStatus.Adt_AccountType = 'EMPSTAT'
                            LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = Emt_JobStatus    
	                            AND JobStatus.Adt_AccountType = 'JOBSTATUS'
                            LEFT JOIN  {0}..T_AccountDetail  DeductionType ON DeductionType.Adt_AccountCode = Dcm_DeductionType    
	                            AND DeductionType.Adt_AccountType = 'DEDNTYPE'
                            LEFT JOIN  {0}..T_AccountDetail  DeductionPayType ON DeductionPayType.Adt_AccountCode = Edd_PaymentType    
	                            AND DeductionPayType.Adt_AccountType = 'DEDPAYTYPE'
                            {1}

                            UNION ALL

                            SELECT  Edd_EmployeeID as [IDNumber]
		                            , Emt_LastName as [Last Name]
		                            , Emt_FirstName as [First Name]
		                            , {0}..T_EmployeeMasterHist.Emt_CostCenterCode as  [Costcenter]
		                            , dbo.getCostCenterFullNameV2({0}..T_EmployeeMasterHist.Emt_CostCenterCode) as [Description]
		                            , Edd_DeductionCode as [Deduction Code]
		                            , Convert(Char(10),Edd_StartDeductionDate, 101) as [Start of Deduction]
		                            , Edd_Amount as [Amount]
		                            , CASE WHEN Edd_FromDeferred = 1 THEN 'YES' ELSE 'NO' END	as [From Deferred]
		                            , CASE WHEN Edd_PaymentFlag = 1 THEN 'YES' ELSE 'NO' END	as [Post to Payroll]
		                            , CASE WHEN Dcm_WithCheck = 1 THEN 'YES' ELSE 'NO' END	as [With Check]
		                            , ISNULL(Convert(Char(10),Edl_CheckDate, 101), '') as [Check Date]
		                            , CASE WHEN Dcm_WithVoucher = 1 THEN 'YES' ELSE 'NO' END	as [With Voucher]
		                            , Edl_VoucherNumber as [Voucher Number]
		                            , PayrollType.Adt_AccountDesc as [Payroll Type] 
		                            , EmploymentStatus.Adt_AccountDesc AS [Employment Status]
		                            , JobStatus.Adt_AccountDesc AS [Job Status]
		                            , DeductionType.Adt_AccountDesc AS [Deduction Type]
		                            , ISNULL(DeductionPayType.Adt_AccountDesc, '') AS [Payment Type]
                            FROM {0}..T_EmployeeDeductionDetailHist
                            INNER JOIN {0}..T_EmployeeDeductionLedger on Edl_EmployeeID =  Edd_EmployeeID
	                            AND Edl_DeductionCode = Edd_DeductionCode
	                            AND Edl_StartDeductionDate = Edd_StartDeductionDate
                            INNER JOIN {0}..T_EmployeeMaster on {0}..T_EmployeeMaster.Emt_EmployeeID = Edd_EmployeeID
                            INNER JOIN  {0}..T_EmployeeMasterHist on {0}..T_EmployeeMasterHist.Emt_EmployeeID = Edd_EmployeeID
	                            AND Emt_PayPeriod = Edd_PayPeriod
                            INNER JOIN  {0}..T_DeductionCodeMaster on Dcm_DeductionCode = Edd_DeductionCode
                            LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = {0}..T_EmployeeMasterHist.Emt_PayrollType 
	                            AND  PayrollType.Adt_AccountType = 'PAYTYPE'
                            LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = {0}..T_EmployeeMasterHist.Emt_EmploymentStatus 
	                            AND EmploymentStatus.Adt_AccountType = 'EMPSTAT'
                            LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = {0}..T_EmployeeMasterHist.Emt_JobStatus    
	                            AND JobStatus.Adt_AccountType = 'JOBSTATUS'
                            LEFT JOIN  {0}..T_AccountDetail  DeductionType ON DeductionType.Adt_AccountCode = Dcm_DeductionType    
	                            AND DeductionType.Adt_AccountType = 'DEDNTYPE'
                            LEFT JOIN  {0}..T_AccountDetail  DeductionPayType ON DeductionPayType.Adt_AccountCode = Edd_PaymentType    
	                            AND DeductionPayType.Adt_AccountType = 'DEDPAYTYPE'
                            {1}

                            UNION ALL

                            SELECT  Edd_EmployeeID as [IDNumber]
		                            , Emt_LastName as [Last Name]
		                            , Emt_FirstName as [First Name]
		                            , {0}..T_EmployeeMasterHist.Emt_CostCenterCode as  [Costcenter]
		                            , dbo.getCostCenterFullNameV2({0}..T_EmployeeMasterHist.Emt_CostCenterCode) as [Description]
		                            , Edd_DeductionCode as [Deduction Code]
		                            , Convert(Char(10),Edd_StartDeductionDate, 101) as [Start of Deduction]
		                            , Edd_Amount as [Amount]
		                            , CASE WHEN Edd_FromDeferred = 1 THEN 'YES' ELSE 'NO' END	as [From Deferred]
		                            , CASE WHEN Edd_PaymentFlag = 1 THEN 'YES' ELSE 'NO' END	as [Post to Payroll]
		                            , CASE WHEN Dcm_WithCheck = 1 THEN 'YES' ELSE 'NO' END	as [With Check]
		                            , ISNULL(Convert(Char(10),Edl_CheckDate, 101), '') as [Check Date]
		                            , CASE WHEN Dcm_WithVoucher = 1 THEN 'YES' ELSE 'NO' END	as [With Voucher]
		                            , Edl_VoucherNumber as [Voucher Number]
		                            , PayrollType.Adt_AccountDesc as [Payroll Type] 
		                            , EmploymentStatus.Adt_AccountDesc AS [Employment Status]
		                            , JobStatus.Adt_AccountDesc AS [Job Status]
		                            , DeductionType.Adt_AccountDesc AS [Deduction Type]
		                            , ISNULL(DeductionPayType.Adt_AccountDesc, '') AS [Payment Type]
                            FROM {0}..T_EmployeeDeductionDetailHistFP
                            INNER JOIN {0}..T_EmployeeDeductionLedgerHist on Edl_EmployeeID =  Edd_EmployeeID
	                            AND Edl_DeductionCode = Edd_DeductionCode
	                            AND Edl_StartDeductionDate = Edd_StartDeductionDate
                            INNER JOIN {0}..T_EmployeeMaster on {0}..T_EmployeeMaster.Emt_EmployeeID = Edd_EmployeeID
                            INNER JOIN  {0}..T_EmployeeMasterHist on {0}..T_EmployeeMasterHist.Emt_EmployeeID = Edd_EmployeeID
	                            AND Emt_PayPeriod = Edd_PayPeriod
                            INNER JOIN  {0}..T_DeductionCodeMaster on Dcm_DeductionCode = Edd_DeductionCode
                            LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = {0}..T_EmployeeMasterHist.Emt_PayrollType 
	                            AND  PayrollType.Adt_AccountType = 'PAYTYPE'
                            LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = {0}..T_EmployeeMasterHist.Emt_EmploymentStatus 
	                            AND EmploymentStatus.Adt_AccountType = 'EMPSTAT'
                            LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = {0}..T_EmployeeMasterHist.Emt_JobStatus    
	                            AND JobStatus.Adt_AccountType = 'JOBSTATUS'
                            LEFT JOIN  {0}..T_AccountDetail  DeductionType ON DeductionType.Adt_AccountCode = Dcm_DeductionType    
	                            AND DeductionType.Adt_AccountType = 'DEDNTYPE'
                            LEFT JOIN  {0}..T_AccountDetail  DeductionPayType ON DeductionPayType.Adt_AccountCode = Edd_PaymentType    
	                            AND DeductionPayType.Adt_AccountType = 'DEDPAYTYPE'
                            {1}

                            ";
                            #endregion
                        }
                        else
                        {
                            #region query

                            query = @"
                            SELECT  Edd_EmployeeID as [IDNumber]
                                    , Emt_LastName as [Last Name]
                                    , Emt_FirstName as [First Name]
                                    , Emt_CostCenterCode as  [Costcenter]
                                    , dbo.getCostCenterFullNameV2(Emt_CostCenterCode) as [Description]
                                    , Edd_DeductionCode as [Deduction Code]
                                    , Edd_PayPeriod as [Deferred this Period]
                                    , Convert(Char(10),Edd_StartDeductionDate, 101) as [Start of Deduction]
                                    , Edd_DeferredAmount as [Amount]
                                    , CASE WHEN Dcm_WithCheck = 1 THEN 'YES' ELSE 'NO' END	as [With Check]
                                    , ISNULL(Convert(Char(10),Edl_CheckDate, 101), '') as [Check Date]
                                    , CASE WHEN Dcm_WithVoucher = 1 THEN 'YES' ELSE 'NO' END	as [With Voucher]
                                    , Edl_VoucherNumber as [Voucher Number]
                                    , PayrollType.Adt_AccountDesc as [Payroll Type] 
                                    , EmploymentStatus.Adt_AccountDesc AS [Employment Status]
                                    , JobStatus.Adt_AccountDesc AS [Job Status]
                                    , ISNULL(DeductionType.Adt_AccountDesc, '') AS [Deduction Type]
                            FROM {0}..T_EmployeeDeductionDeffered
                            INNER JOIN {0}..T_EmployeeDeductionLedger on Edl_EmployeeID =  Edd_EmployeeID
                                and Edl_DeductionCode = Edd_DeductionCode
                                and Edl_StartDeductionDate = Edd_StartDeductionDate
                            INNER JOIN {0}..T_EmployeeMaster on Emt_EmployeeID = Edd_EmployeeID
                            INNER JOIN  {0}..T_DeductionCodeMaster on Dcm_DeductionCode = Edd_DeductionCode
                            LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = Emt_PayrollType 
                                AND  PayrollType.Adt_AccountType = 'PAYTYPE'
                            LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = Emt_EmploymentStatus 
                                AND EmploymentStatus.Adt_AccountType = 'EMPSTAT'
                            LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = Emt_JobStatus    
                                AND JobStatus.Adt_AccountType = 'JOBSTATUS'
                            LEFT JOIN  {0}..T_AccountDetail  DeductionType ON DeductionType.Adt_AccountCode = Dcm_DeductionType    
                                AND DeductionType.Adt_AccountType = 'DEDNTYPE'
                            {1}
                            ";

                            #endregion
                        }
                        break;
                    case "Deduction and Loan Ledger Report":
                        if (category)
                        {
                            #region query
                            query = @"
                            SELECT  Edl_EmployeeID as [IDNumber]
		                            , Emt_LastName as [Last Name]
		                            , Emt_FirstName as [First Name]
		                            , Emt_CostCenterCode as  [Costcenter]
		                            , dbo.getCostCenterFullNameV2(Emt_CostCenterCode) as [Description]
		                            , Edl_DeductionCode as [Deduction Code]
		                            , Convert(Char(10),Edl_StartDeductionDate, 101) as [Start of Deduction]
		                            , Edl_DeductionAmount as [Deduction Amount]
		                            , Edl_PaidAmount as [Paid Amount]
		                            , Edl_DeductionAmount - Edl_PaidAmount as [Balance Amount]
		                            , Edl_AmortizationAmount as [Amortization Amount]
		                            , Edl_DeferredAmount as [Deferred Amount]
		                            , CASE WHEN Dcm_WithCheck = 1 THEN 'YES' ELSE 'NO' END	as [With Check]
		                            , Convert(Char(10),Edl_CheckDate, 101) as [Check Date]
		                            , CASE WHEN Dcm_WithVoucher = 1 THEN 'YES' ELSE 'NO' END	as [With Voucher]
		                            , Edl_VoucherNumber as [Voucher Number]
		                            , CASE WHEN Edl_ExcludeFromPayroll = 1 THEN 'YES' ELSE 'NO' END	as [Exclude From Payroll]
		                            , Convert(Char(10),Edl_FullyPaidDate, 101) as [FullyPaid Date]		
		                            , PayrollType.Adt_AccountDesc as [Payroll Type] 
		                            , EmploymentStatus.Adt_AccountDesc AS [Employment Status]
		                            , JobStatus.Adt_AccountDesc AS [Job Status]
		                            , DeductionType.Adt_AccountDesc AS [Deduction Type]
                            FROM {0}..T_EmployeeDeductionLedger
                            INNER JOIN {0}..T_EmployeeMaster on Emt_EmployeeID = Edl_EmployeeID
                            INNER JOIN  {0}..T_DeductionCodeMaster on Dcm_DeductionCode = Edl_DeductionCode
                            LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = Emt_PayrollType 
	                            AND  PayrollType.Adt_AccountType = 'PAYTYPE'
                            LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = Emt_EmploymentStatus 
	                            AND EmploymentStatus.Adt_AccountType = 'EMPSTAT'
                            LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = Emt_JobStatus    
	                            AND JobStatus.Adt_AccountType = 'JOBSTATUS'
                            LEFT JOIN  {0}..T_AccountDetail  DeductionType ON DeductionType.Adt_AccountCode = Dcm_DeductionType    
	                            AND DeductionType.Adt_AccountType = 'DEDNTYPE'
                            {1}
                            ";
                            #endregion
                        }
                        else
                        {
                            #region query
                            query = @"
                              SELECT  Edl_EmployeeID as [IDNumber]
		                            , Emt_LastName as [Last Name]
		                            , Emt_FirstName as [First Name]
		                            , Emt_CostCenterCode as  [Costcenter]
		                            , dbo.getCostCenterFullNameV2(Emt_CostCenterCode) as [Description]
		                            , Edl_DeductionCode as [Deduction Code]
		                            , Convert(Char(10),Edl_StartDeductionDate, 101) as [Start of Deduction]
		                            , Edl_DeductionAmount as [Deduction Amount]
		                            , Edl_PaidAmount as [Paid Amount]
		                            , Edl_DeductionAmount - Edl_PaidAmount as [Balance Amount]
		                            , Edl_AmortizationAmount as [Amortization Amount]
		                            , Edl_DeferredAmount as [Deferred Amount]
		                            , CASE WHEN Dcm_WithCheck = 1 THEN 'YES' ELSE 'NO' END	as [With Check]
		                            , Convert(Char(10),Edl_CheckDate, 101) as [Check Date]
		                            , CASE WHEN Dcm_WithVoucher = 1 THEN 'YES' ELSE 'NO' END	as [With Voucher]
		                            , Edl_VoucherNumber as [Voucher Number]
		                            , CASE WHEN Edl_ExcludeFromPayroll = 1 THEN 'YES' ELSE 'NO' END	as [Exclude From Payroll]
		                            , Convert(Char(10),Edl_FullyPaidDate, 101) as [FullyPaid Date]		
		                            , PayrollType.Adt_AccountDesc as [Payroll Type] 
		                            , EmploymentStatus.Adt_AccountDesc AS [Employment Status]
		                            , JobStatus.Adt_AccountDesc AS [Job Status]
		                            , DeductionType.Adt_AccountDesc AS [Deduction Type]
                            FROM {0}..T_EmployeeDeductionLedgerHist
                            INNER JOIN {0}..T_EmployeeMaster on Emt_EmployeeID = Edl_EmployeeID
                            INNER JOIN  {0}..T_DeductionCodeMaster on Dcm_DeductionCode = Edl_DeductionCode
                            LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = Emt_PayrollType 
	                            AND  PayrollType.Adt_AccountType = 'PAYTYPE'
                            LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = Emt_EmploymentStatus 
	                            AND EmploymentStatus.Adt_AccountType = 'EMPSTAT'
                            LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = Emt_JobStatus    
	                            AND JobStatus.Adt_AccountType = 'JOBSTATUS'
                            LEFT JOIN  {0}..T_AccountDetail  DeductionType ON DeductionType.Adt_AccountCode = Dcm_DeductionType    
	                            AND DeductionType.Adt_AccountType = 'DEDNTYPE'
                                {1}  
                            ";
                            #endregion
                        }
                        break;
                    case "Adjustment Report":
                        #region query
                        query = @"
                        SELECT  {0}..T_EmployeeAdjustment.Ead_EmployeeId as [IDNumber]
                                , Emt_LastName as [Last Name]
                                , Emt_FirstName as [First Name]
                                , Emt_CostCenterCode as  [Costcenter]
                                , dbo.getCostCenterFullNameV2(Emt_CostCenterCode) as [Description]
                                , {0}..T_EmployeeAdjustment.Ead_CurrentPayPeriod as [This Payperiod]
                                , Ead_HrlyRate as [Hourly Rate]
                                , Ead_RegularHr as [Regular Hour]
                                , Ead_RegularOTHr as [Regular OT Hour]
                                , Ead_RegularNDHr as [Regular ND Hour]
                                , Ead_RegularOTNDHr as [Regular OT ND Hour]
                                , Ead_RestdayHr as [Restday Hour]
                                , Ead_RestdayOTHr as [Restday OT Hour]
                                , Ead_RestdayNDHr as [Restday ND Hour]
                                , Ead_RestdayOTNDHr as [Restday OT ND Hour]
                                , Ead_LegalHolidayHr as [Legal Holiday Hour]
                                , Ead_LegalHolidayOTHr as [Legal Holiday OT Hour]
                                , Ead_LegalHolidayNDHr as [Legal Holiday ND Hour]
                                , Ead_LegalHolidayOTNDHr as [Legal Holiday OT ND Hour]
                                , Ead_SpecialHolidayHr as [Special Holiday Hour]
                                , Ead_SpecialHolidayOTHr as [Special Holiday OT Hour]
                                , Ead_SpecialHolidayNDHr as [Special Holiday ND Hour]
                                , Ead_SpecialHolidayOTNDHr as [Special Holiday OT ND Hour]
                                , Ead_PlantShutdownHr as [Plant Shutdown Hour]
                                , Ead_PlantShutdownOTHr as [Plant Shutdown OT Hour]
                                , Ead_PlantShutdownNDHr as [Plant Shutdown ND Hour]
                                , Ead_PlantShutdownOTNDHr as [Plant Shutdown OT ND Hour]
                                , Ead_CompanyHolidayHr as [Company Holiday Hour]
                                , Ead_CompanyHolidayOTHr as [Company Holiday OT Hour]
                                , Ead_CompanyHolidayNDHr as [Company Holiday ND Hour]
                                , Ead_CompanyHolidayOTNDHr as [Company Holiday OT ND Hour]
                                , Ead_RestdayLegalHolidayHr as [Restday Legal Holiday Hour]
                                , Ead_RestdayLegalHolidayOTHr as [Restday Legal Holiday OT Hour]
                                , Ead_RestdayLegalHolidayNDHr as [Restday Legal Holiday ND Hour]
                                , Ead_RestdayLegalHolidayOTNDHr as [Restday Legal Holiday OT ND Hour]
                                , Ead_RestdaySpecialHolidayHr as [Restday Special Holiday Hour]
                                , Ead_RestdaySpecialHolidayOTHr as [Restday Special Holiday OT Hour]
                                , Ead_RestdaySpecialHolidayNDHr as [Restday Special Holiday ND Hour]
                                , Ead_RestdaySpecialHolidayOTNDHr as [Restday SpecialHoliday OT ND  Hour]
                                , Ead_RestdayCompanyHolidayHr as [Restday Company Holiday Hour]
                                , Ead_RestdayCompanyHolidayOTHr as [Restday Company Holiday OT Hour]
                                , Ead_RestdayCompanyHolidayNDHr as [Restday Company Holiday ND Hour]
                                , Ead_RestdayCompanyHolidayOTNDHr as [Restday Company Holiday OT ND Hour]
                                , Ead_RestdayPlantShutdownHr as [Restday Plant Shutdown Hour]
                                , Ead_RestdayPlantShutdownOTHr as [Restday Plant Shutdown OT Hour]
                                , Ead_RestdayPlantShutdownNDHr as [Restday Plant Shutdown NDHour]
                                , Ead_RestdayPlantShutdownOTNDHr as [Restday Plant Shutdown OT ND  Hour]
                                , Ead_Filler01_Hr as [Filler01 Hour]
                                , Ead_Filler01_OTHr as [Filler01 OT Hour]
                                , Ead_Filler01_NDHr as [Filler01 ND Hour]
                                , Ead_Filler01_OTNDHr as [Filler01 OT ND Hour]
                                , Ead_Filler02_Hr as [Filler02 Hour]
                                , Ead_Filler02_OTHr as [Filler02 OT Hour]
                                , Ead_Filler02_NDHr as [Filler02 ND Hour]
                                , Ead_Filler02_OTNDHr as [Filler02 OT ND Hour]
                                , Ead_Filler03_Hr as [Filler03 Hour]
                                , Ead_Filler03_OTHr as [Filler03 OT Hour]
                                , Ead_Filler03_NDHr as [Filler03 ND Hour]
                                , Ead_Filler03_OTNDHr as [Filler03 OT ND Hour]
                                , Ead_Filler04_Hr as [Filler04 Hour]
                                , Ead_Filler04_OTHr as [Filler04 OT Hour]
                                , Ead_Filler04_NDHr as [Filler04 ND Hour]
                                , Ead_Filler04_OTNDHr as [Filler04 OT ND Hour]
                                , Ead_Filler05_Hr as [Filler05 Hour]
                                , Ead_Filler05_OTHr as [Filler05 OT Hour]
                                , Ead_Filler05_NDHr as [Filler05 ND Hour]
                                , Ead_Filler05_OTNDHr as [Filler05 OT ND Hour]
                                , Ead_Filler06_Hr as [Filler06 Hour]
                                , Ead_Filler06_OTHr as [Filler06 OT Hour]
                                , Ead_Filler06_NDHr as [Filler06 ND Hour]
                                , Ead_Filler06_OTNDHr as [Filler06 OT ND Hour]
                                , Ead_LaborHrsAdjustmentAmt as [Labor Hrs Adjustment Amount]
                                , Ead_TaxAdjustmentAmt as [Tax Adjustment Amount]
                                , Ead_NonTaxAdjustmentAmt as [Nontax Adjustment Amount]
                                , CASE WHEN Ead_PayrollPost = 1 THEN 'YES' ELSE 'NO' END	as [Post to Payroll]
                                , PayrollType.Adt_AccountDesc as [Payroll Type] 
                                , EmploymentStatus.Adt_AccountDesc AS [Employment Status]
                                , JobStatus.Adt_AccountDesc AS [Job Status]
                        FROM {0}..T_EmployeeAdjustment
                        LEFT JOIN {0}..T_EmployeeAdjustmentExt on {0}..T_EmployeeAdjustmentExt.Ead_EmployeeId = {0}..T_EmployeeAdjustment.Ead_EmployeeId
                            AND {0}..T_EmployeeAdjustmentExt.Ead_CurrentPayPeriod = {0}..T_EmployeeAdjustment.Ead_CurrentPayPeriod
                        INNER JOIN {0}..T_EmployeeMaster on Emt_EmployeeID = {0}..T_EmployeeAdjustment.Ead_EmployeeId
                        LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = Emt_PayrollType 
                            AND  PayrollType.Adt_AccountType = 'PAYTYPE'
                        LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = Emt_EmploymentStatus 
                            AND EmploymentStatus.Adt_AccountType = 'EMPSTAT'
                        LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = Emt_JobStatus    
                            AND JobStatus.Adt_AccountType = 'JOBSTATUS'
                        {1}

                        UNION ALL

                        SELECT  {0}..T_EmployeeAdjustmentHist.Ead_EmployeeId as [IDNumber]
                                , Emt_LastName as [Last Name]
                                , Emt_FirstName as [First Name]
                                , {0}..T_EmployeeMasterHist.Emt_CostCenterCode as  [Costcenter]
                                , dbo.getCostCenterFullNameV2({0}..T_EmployeeMasterHist.Emt_CostCenterCode) as [Description]
                                , {0}..T_EmployeeAdjustmentHist.Ead_CurrentPayPeriod as [This Payperiod]
                                , Ead_HrlyRate as [Hourly Rate]
                                , Ead_RegularHr as [Regular Hour]
                                , Ead_RegularOTHr as [Regular OT Hour]
                                , Ead_RegularNDHr as [Regular ND Hour]
                                , Ead_RegularOTNDHr as [Regular OT ND Hour]
                                , Ead_RestdayHr as [Restday Hour]
                                , Ead_RestdayOTHr as [Restday OT Hour]
                                , Ead_RestdayNDHr as [Restday ND Hour]
                                , Ead_RestdayOTNDHr as [Restday OT ND Hour]
                                , Ead_LegalHolidayHr as [Legal Holiday Hour]
                                , Ead_LegalHolidayOTHr as [Legal Holiday OT Hour]
                                , Ead_LegalHolidayNDHr as [Legal Holiday ND Hour]
                                , Ead_LegalHolidayOTNDHr as [Legal Holiday OT ND Hour]
                                , Ead_SpecialHolidayHr as [Special Holiday Hour]
                                , Ead_SpecialHolidayOTHr as [Special Holiday OT Hour]
                                , Ead_SpecialHolidayNDHr as [Special Holiday ND Hour]
                                , Ead_SpecialHolidayOTNDHr as [Special Holiday OT ND Hour]
                                , Ead_PlantShutdownHr as [Plant Shutdown Hour]
                                , Ead_PlantShutdownOTHr as [Plant Shutdown OT Hour]
                                , Ead_PlantShutdownNDHr as [Plant Shutdown ND Hour]
                                , Ead_PlantShutdownOTNDHr as [Plant Shutdown OT ND Hour]
                                , Ead_CompanyHolidayHr as [Company Holiday Hour]
                                , Ead_CompanyHolidayOTHr as [Company Holiday OT Hour]
                                , Ead_CompanyHolidayNDHr as [Company Holiday ND Hour]
                                , Ead_CompanyHolidayOTNDHr as [Company Holiday OT ND Hour]
                                , Ead_RestdayLegalHolidayHr as [Restday Legal Holiday Hour]
                                , Ead_RestdayLegalHolidayOTHr as [Restday Legal Holiday OT Hour]
                                , Ead_RestdayLegalHolidayNDHr as [Restday Legal Holiday ND Hour]
                                , Ead_RestdayLegalHolidayOTNDHr as [Restday Legal Holiday OT ND Hour]
                                , Ead_RestdaySpecialHolidayHr as [Restday Special Holiday Hour]
                                , Ead_RestdaySpecialHolidayOTHr as [Restday Special Holiday OT Hour]
                                , Ead_RestdaySpecialHolidayNDHr as [Restday Special Holiday ND Hour]
                                , Ead_RestdaySpecialHolidayOTNDHr as [Restday SpecialHoliday OT ND  Hour]
                                , Ead_RestdayCompanyHolidayHr as [Restday Company Holiday Hour]
                                , Ead_RestdayCompanyHolidayOTHr as [Restday Company Holiday OT Hour]
                                , Ead_RestdayCompanyHolidayNDHr as [Restday Company Holiday ND Hour]
                                , Ead_RestdayCompanyHolidayOTNDHr as [Restday Company Holiday OT ND Hour]
                                , Ead_RestdayPlantShutdownHr as [Restday Plant Shutdown Hour]
                                , Ead_RestdayPlantShutdownOTHr as [Restday Plant Shutdown OT Hour]
                                , Ead_RestdayPlantShutdownNDHr as [Restday Plant Shutdown NDHour]
                                , Ead_RestdayPlantShutdownOTNDHr as [Restday Plant Shutdown OT ND  Hour]
                                , Ead_Filler01_Hr as [Filler01 Hour]
                                , Ead_Filler01_OTHr as [Filler01 OT Hour]
                                , Ead_Filler01_NDHr as [Filler01 ND Hour]
                                , Ead_Filler01_OTNDHr as [Filler01 OT ND Hour]
                                , Ead_Filler02_Hr as [Filler02 Hour]
                                , Ead_Filler02_OTHr as [Filler02 OT Hour]
                                , Ead_Filler02_NDHr as [Filler02 ND Hour]
                                , Ead_Filler02_OTNDHr as [Filler02 OT ND Hour]
                                , Ead_Filler03_Hr as [Filler03 Hour]
                                , Ead_Filler03_OTHr as [Filler03 OT Hour]
                                , Ead_Filler03_NDHr as [Filler03 ND Hour]
                                , Ead_Filler03_OTNDHr as [Filler03 OT ND Hour]
                                , Ead_Filler04_Hr as [Filler04 Hour]
                                , Ead_Filler04_OTHr as [Filler04 OT Hour]
                                , Ead_Filler04_NDHr as [Filler04 ND Hour]
                                , Ead_Filler04_OTNDHr as [Filler04 OT ND Hour]
                                , Ead_Filler05_Hr as [Filler05 Hour]
                                , Ead_Filler05_OTHr as [Filler05 OT Hour]
                                , Ead_Filler05_NDHr as [Filler05 ND Hour]
                                , Ead_Filler05_OTNDHr as [Filler05 OT ND Hour]
                                , Ead_Filler06_Hr as [Filler06 Hour]
                                , Ead_Filler06_OTHr as [Filler06 OT Hour]
                                , Ead_Filler06_NDHr as [Filler06 ND Hour]
                                , Ead_Filler06_OTNDHr as [Filler06 OT ND Hour]
                                , Ead_LaborHrsAdjustmentAmt as [Labor Hrs Adjustment Amount]
                                , Ead_TaxAdjustmentAmt as [Tax Adjustment Amount]
                                , Ead_NonTaxAdjustmentAmt as [Nontax Adjustment Amount]
                                , CASE WHEN Ead_PayrollPost = 1 THEN 'YES' ELSE 'NO' END	as [Post to Payroll]
                                , PayrollType.Adt_AccountDesc as [Payroll Type] 
                                , EmploymentStatus.Adt_AccountDesc AS [Employment Status]
                                , JobStatus.Adt_AccountDesc AS [Job Status]
                        FROM {0}..T_EmployeeAdjustmentHist
                        LEFT JOIN {0}..T_EmployeeAdjustmentHistExt on {0}..T_EmployeeAdjustmentHistExt.Ead_EmployeeId = {0}..T_EmployeeAdjustmentHist.Ead_EmployeeId
                            AND {0}..T_EmployeeAdjustmentHistExt.Ead_CurrentPayPeriod = {0}..T_EmployeeAdjustmentHist.Ead_CurrentPayPeriod
                        INNER JOIN {0}..T_EmployeeMaster on {0}..T_EmployeeMaster.Emt_EmployeeID = {0}..T_EmployeeAdjustmentHist.Ead_EmployeeId
                        INNER JOIN {0}..T_EmployeeMasterHist on {0}..T_EmployeeMasterHist.Emt_EmployeeID = {0}..T_EmployeeAdjustmentHist.Ead_EmployeeId
                            AND {0}..T_EmployeeMasterHist.Emt_PayPeriod =  {0}..T_EmployeeAdjustmentHist.Ead_CurrentPayPeriod
                        LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = {0}..T_EmployeeMasterHist.Emt_PayrollType 
                            AND  PayrollType.Adt_AccountType = 'PAYTYPE'
                        LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = {0}..T_EmployeeMasterHist.Emt_EmploymentStatus 
                            AND EmploymentStatus.Adt_AccountType = 'EMPSTAT'
                        LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = {0}..T_EmployeeMasterHist.Emt_JobStatus    
                            AND JobStatus.Adt_AccountType = 'JOBSTATUS'
                        {2}
                        ";
                        #endregion
                        break;
                    case "Log Report":
                        #region query
                        query = @"
                            SELECT  {0}..T_EmployeeLogledger.Ell_EmployeeId as [IDNumber]
		                        , Emt_LastName as [Last Name]
		                        , Emt_FirstName as [First Name]
		                        , Emt_CostCenterCode as  [Costcenter]
		                        , dbo.getCostCenterFullNameV2(Emt_CostCenterCode) as [Description]
		                        , Ell_PayPeriod as [This Period]
		                        , CONVERT(char(10), Ell_ProcessDate,101) as [Log Date]
		                        , Ell_DayCode as [Day Code]
		                        , Ell_ShiftCode as [Shift Code]
		                        , LEFT(Ell_ActualTimeIn_1,2) + ':' + RIGHT(Ell_ActualTimeIn_1, 2) as [Actual Time In 1]
		                        , LEFT(Ell_ActualTimeOut_1,2) +':' + RIGHT(Ell_ActualTimeOut_1, 2) as [Actual Time Out 1]
		                        , LEFT(Ell_ActualTimeIn_2, 2) + ':' + RIGHT(Ell_ActualTimeIn_2, 2) as [Actual Time In 2]
		                        , LEFT(Ell_ActualTimeOut_2, 2) + ':' + RIGHT(Ell_ActualTimeOut_2, 2) as [Actual Time Out 2]
		                        , LEFT(Scm_ShiftTimeIn, 2) + ':' + RIGHT(Scm_ShiftTimeIn, 2) as [Shift Time In]
		                        , LEFT(Scm_ShiftBreakStart, 2) + ':' + RIGHT(Scm_ShiftBreakStart, 2) as [Start of Break]
		                        , LEFT(Scm_ShiftBreakEnd, 2) + ':' + RIGHT(Scm_ShiftBreakEnd, 2) as [End of Break]
		                        , LEFT(Scm_ShiftTimeOut, 2) + ':' + RIGHT(Scm_ShiftTimeOut, 2) as [Shift Time Out]
		                        , ((CONVERT(int,LEFT(Ell_ActualTimeIn_1,2)) * 60) + CONVERT(int,RIGHT(Ell_ActualTimeIn_1,2)))- ((CONVERT(int,LEFT(Scm_ShiftTimeIn,2)) * 60) + CONVERT(int,RIGHT(Scm_ShiftTimeIn,2))) as [Minutes Log IN 1 before shift]
		                        , ((CONVERT(int,LEFT(Ell_ActualTimeOut_1,2)) * 60) + CONVERT(int,RIGHT(Ell_ActualTimeOut_1,2))) - ((CONVERT(int,LEFT(Scm_ShiftBreakStart,2)) * 60) + CONVERT(int,RIGHT(Scm_ShiftBreakStart,2))) as [Minutes Log OUT 1 after shift]
		                        , ((CONVERT(int,LEFT(Ell_ActualTimeIn_2,2)) * 60) + CONVERT(int,RIGHT(Ell_ActualTimeIn_2,2))) - ((CONVERT(int,LEFT(Scm_ShiftBreakEnd,2)) * 60) + CONVERT(int,RIGHT(Scm_ShiftBreakEnd,2))) as [Minutes Log IN 2 before shift]
		                        , ((CONVERT(int,LEFT(Ell_ActualTimeOut_2,2)) * 60) + CONVERT(int,RIGHT(Ell_ActualTimeOut_2,2))) - ((CONVERT(int,LEFT(Scm_ShiftTimeOut,2)) * 60) + CONVERT(int,RIGHT(Scm_ShiftTimeOut,2))) as [Minutes Log OUT 2 after shift]
		                        , Ell_AbsentHour as [Absent Hour]
		                        , Ell_RegularHour as [Regular Hour]
		                        , Ell_OvertimeHour as [Overtime Hour]
		                        , Ell_RegularNightPremHour as [Regular Night Prem Hour]
		                        , Ell_OvertimeNightPremHour as [Overtime Night Prem Hour]
		                        , Ell_LeaveHour as [Leave Hour]
                                , Ell_WorkType as [Work Type]
		                        , Ell_WorkGroup as [Work Group]
		                        , LocationCode.Adt_AccountDesc as [Location]
		                        , PayrollType.Adt_AccountDesc as [Payroll Type] 
		                        , EmploymentStatus.Adt_AccountDesc AS [Employment Status]
		                        , JobStatus.Adt_AccountDesc AS [Job Status]
                        FROM {0}..T_EmployeeLogledger
                        INNER JOIN {0}..T_EmployeeMaster on Emt_EmployeeID = {0}..T_EmployeeLogledger.Ell_EmployeeId
                        INNER JOIN {0}..T_ShiftCodeMaster on Scm_ShiftCode = Ell_ShiftCode
                        LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = Emt_PayrollType 
	                        AND  PayrollType.Adt_AccountType = 'PAYTYPE'
                        LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = Emt_EmploymentStatus 
	                        AND EmploymentStatus.Adt_AccountType = 'EMPSTAT'
                        LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = Emt_JobStatus    
	                        AND JobStatus.Adt_AccountType = 'JOBSTATUS'
	                    LEFT JOIN T_AccountDetail LocationCode on LocationCode.Adt_AccountCode = Ell_LocationCode
							and LocationCode.Adt_AccountType = 'ZIPCODE'
                        {1}

                        UNION ALL

                        SELECT  {0}..T_EmployeeLogLedgerHist.Ell_EmployeeId as [IDNumber]
		                        , Emt_LastName as [Last Name]
		                        , Emt_FirstName as [First Name]
		                        , {0}..T_EmployeeMasterHist.Emt_CostCenterCode as  [Costcenter]
		                        , dbo.getCostCenterFullNameV2({0}..T_EmployeeMasterHist.Emt_CostCenterCode) as [Description]
		                        , Ell_PayPeriod as [This Period]
		                        , CONVERT(char(10), Ell_ProcessDate,101) as [Log Date]
		                        , Ell_DayCode as [Day Code]
		                        , Ell_ShiftCode as [Shift Code]
		                        , LEFT(Ell_ActualTimeIn_1,2) + ':' + RIGHT(Ell_ActualTimeIn_1, 2) as [Actual Time In 1]
		                        , LEFT(Ell_ActualTimeOut_1,2) +':' + RIGHT(Ell_ActualTimeOut_1, 2) as [Actual Time Out 1]
		                        , LEFT(Ell_ActualTimeIn_2, 2) + ':' + RIGHT(Ell_ActualTimeIn_2, 2) as [Actual Time In 2]
		                        , LEFT(Ell_ActualTimeOut_2, 2) + ':' + RIGHT(Ell_ActualTimeOut_2, 2) as [Actual Time Out 2]
		                        , LEFT(Scm_ShiftTimeIn, 2) + ':' + RIGHT(Scm_ShiftTimeIn, 2) as [Shift Time In]
		                        , LEFT(Scm_ShiftBreakStart, 2) + ':' + RIGHT(Scm_ShiftBreakStart, 2) as [Start of Break]
		                        , LEFT(Scm_ShiftBreakEnd, 2) + ':' + RIGHT(Scm_ShiftBreakEnd, 2) as [End of Break]
		                        , LEFT(Scm_ShiftTimeOut, 2) + ':' + RIGHT(Scm_ShiftTimeOut, 2) as [Shift Time Out]
		                        , ((CONVERT(int,LEFT(Ell_ActualTimeIn_1,2)) * 60) + CONVERT(int,RIGHT(Ell_ActualTimeIn_1,2)))- ((CONVERT(int,LEFT(Scm_ShiftTimeIn,2)) * 60) + CONVERT(int,RIGHT(Scm_ShiftTimeIn,2))) as [Minutes Log IN 1 before shift]
		                        , ((CONVERT(int,LEFT(Ell_ActualTimeOut_1,2)) * 60) + CONVERT(int,RIGHT(Ell_ActualTimeOut_1,2))) - ((CONVERT(int,LEFT(Scm_ShiftBreakStart,2)) * 60) + CONVERT(int,RIGHT(Scm_ShiftBreakStart,2))) as [Minutes Log OUT 1 after shift]
		                        , ((CONVERT(int,LEFT(Ell_ActualTimeIn_2,2)) * 60) + CONVERT(int,RIGHT(Ell_ActualTimeIn_2,2))) - ((CONVERT(int,LEFT(Scm_ShiftBreakEnd,2)) * 60) + CONVERT(int,RIGHT(Scm_ShiftBreakEnd,2))) as [Minutes Log IN 2 before shift]
		                        , ((CONVERT(int,LEFT(Ell_ActualTimeOut_2,2)) * 60) + CONVERT(int,RIGHT(Ell_ActualTimeOut_2,2))) - ((CONVERT(int,LEFT(Scm_ShiftTimeOut,2)) * 60) + CONVERT(int,RIGHT(Scm_ShiftTimeOut,2))) as [Minutes Log OUT 2 after shift]
		                        , Ell_AbsentHour as [Absent Hour]
		                        , Ell_RegularHour as [Regular Hour]
		                        , Ell_OvertimeHour as [Overtime Hour]
		                        , Ell_RegularNightPremHour as [Regular Night Prem Hour]
		                        , Ell_OvertimeNightPremHour as [Overtime Night Prem Hour]
		                        , Ell_LeaveHour as [Leave Hour]
                                , Ell_WorkType as [Work Type]
		                        , Ell_WorkGroup as [Work Group]
		                        , LocationCode.Adt_AccountDesc as [Location]
		                        , PayrollType.Adt_AccountDesc as [Payroll Type] 
		                        , EmploymentStatus.Adt_AccountDesc AS [Employment Status]
		                        , JobStatus.Adt_AccountDesc AS [Job Status]
                        FROM {0}..T_EmployeeLogLedgerHist
                        INNER JOIN {0}..T_EmployeeMaster on {0}..T_EmployeeMaster.Emt_EmployeeID = {0}..T_EmployeeLogLedgerHist.Ell_EmployeeId
                        INNER JOIN {0}..T_EmployeeMasterHist on {0}..T_EmployeeMasterHist.Emt_EmployeeID = {0}..T_EmployeeLogLedgerHist.Ell_EmployeeId
	                        AND {0}..T_EmployeeMasterHist.Emt_Payperiod = {0}..T_EmployeeLogLedgerHist.Ell_PayPeriod
                        INNER JOIN {0}..T_ShiftCodeMaster on Scm_ShiftCode = Ell_ShiftCode
                        LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = {0}..T_EmployeeMasterHist.Emt_PayrollType 
	                        AND  PayrollType.Adt_AccountType = 'PAYTYPE'
                        LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = {0}..T_EmployeeMasterHist.Emt_EmploymentStatus 
	                        AND EmploymentStatus.Adt_AccountType = 'EMPSTAT'
                        LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = {0}..T_EmployeeMasterHist.Emt_JobStatus    
	                        AND JobStatus.Adt_AccountType = 'JOBSTATUS'
	                    LEFT JOIN T_AccountDetail LocationCode on LocationCode.Adt_AccountCode = Ell_LocationCode
							and LocationCode.Adt_AccountType = 'ZIPCODE'
	                        {1}
                        ";
                        #endregion
                        break;
                    case "Assume Present Report":
                        #region query
                        query = @"
                        SELECT  {0}..T_EmployeeLogledger.Ell_EmployeeId as [IDNumber]
		                        , Emt_LastName as [Last Name]
		                        , Emt_FirstName as [First Name]
		                        , Emt_CostCenterCode as  [Costcenter]
		                        , dbo.getCostCenterFullNameV2(Emt_CostCenterCode) as [Description]
		                        , Ell_PayPeriod as [This Period]
		                        , CONVERT(char(10), Ell_ProcessDate,101) as [Log Date]
		                        , Ell_DayCode as [Day Code]
		                        , Ell_ShiftCode as [Shift Code]
		                        , LEFT(Ell_ActualTimeIn_1,2) + ':' + RIGHT(Ell_ActualTimeIn_1, 2) as [Actual Time In 1]
		                        , LEFT(Ell_ActualTimeOut_1,2) +':' + RIGHT(Ell_ActualTimeOut_1, 2) as [Actual Time Out 1]
		                        , LEFT(Ell_ActualTimeIn_2, 2) + ':' + RIGHT(Ell_ActualTimeIn_2, 2) as [Actual Time In 2]
		                        , LEFT(Ell_ActualTimeOut_2, 2) + ':' + RIGHT(Ell_ActualTimeOut_2, 2) as [Actual Time Out 2]
		                        , LEFT(Scm_ShiftTimeIn, 2) + ':' + RIGHT(Scm_ShiftTimeIn, 2) as [Shift Time In]
		                        , LEFT(Scm_ShiftBreakStart, 2) + ':' + RIGHT(Scm_ShiftBreakStart, 2) as [Start of Break]
		                        , LEFT(Scm_ShiftBreakEnd, 2) + ':' + RIGHT(Scm_ShiftBreakEnd, 2) as [End of Break]
		                        , LEFT(Scm_ShiftTimeOut, 2) + ':' + RIGHT(Scm_ShiftTimeOut, 2) as [Shift Time Out]
                                , Ell_WorkType as [Work Type]
		                        , Ell_WorkGroup as [Work Group]
		                        , LocationCode.Adt_AccountDesc as [Location]
		                        , PayrollType.Adt_AccountDesc as [Payroll Type] 
		                        , EmploymentStatus.Adt_AccountDesc AS [Employment Status]
		                        , JobStatus.Adt_AccountDesc AS [Job Status]
		                        , Case when Ell_AssumedPostBack = 'T' then 'TEMP LOGS'
			                         when Ell_AssumedPostBack = 'A' and Ell_ActualTimeIn_1 + Ell_ActualTimeOut_1 + Ell_ActualTimeIn_2 + Ell_ActualTimeOut_2 = '0000000000000000' then 'ACTUAL NO LOGS'
			                        when Ell_AssumedPostBack = 'A' and Ell_ActualTimeIn_1 + Ell_ActualTimeOut_1 + Ell_ActualTimeIn_2 + Ell_ActualTimeOut_2 <> '0000000000000000' then 'ACTUAL WITH LOGS'
			                        else ' ' end as [Remarks]
                        FROM {0}..T_EmployeeLogledger
                        INNER JOIN {0}..T_EmployeeMaster on Emt_EmployeeID = {0}..T_EmployeeLogledger.Ell_EmployeeId
                        INNER JOIN {0}..T_ShiftCodeMaster on Scm_ShiftCode = Ell_ShiftCode
                        LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = Emt_PayrollType 
	                        AND  PayrollType.Adt_AccountType = 'PAYTYPE'
                        LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = Emt_EmploymentStatus 
	                        AND EmploymentStatus.Adt_AccountType = 'EMPSTAT'
                        LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = Emt_JobStatus    
	                        AND JobStatus.Adt_AccountType = 'JOBSTATUS'
	                    LEFT JOIN {0}..T_AccountDetail LocationCode on LocationCode.Adt_AccountCode = Ell_LocationCode
							and LocationCode.Adt_AccountType = 'ZIPCODE'
                        {1}

                        UNION ALL

                        SELECT  {0}..T_EmployeeLogLedgerHist.Ell_EmployeeId as [IDNumber]
		                        , Emt_LastName as [Last Name]
		                        , Emt_FirstName as [First Name]
		                        , {0}..T_EmployeeMasterHist.Emt_CostCenterCode as  [Costcenter]
		                        , dbo.getCostCenterFullNameV2({0}..T_EmployeeMasterHist.Emt_CostCenterCode) as [Description]
		                        , Ell_PayPeriod as [This Period]
		                        , CONVERT(char(10), Ell_ProcessDate,101) as [Log Date]
		                        , Ell_DayCode as [Day Code]
		                        , Ell_ShiftCode as [Shift Code]
		                        , LEFT(Ell_ActualTimeIn_1,2) + ':' + RIGHT(Ell_ActualTimeIn_1, 2) as [Actual Time In 1]
		                        , LEFT(Ell_ActualTimeOut_1,2) +':' + RIGHT(Ell_ActualTimeOut_1, 2) as [Actual Time Out 1]
		                        , LEFT(Ell_ActualTimeIn_2, 2) + ':' + RIGHT(Ell_ActualTimeIn_2, 2) as [Actual Time In 2]
		                        , LEFT(Ell_ActualTimeOut_2, 2) + ':' + RIGHT(Ell_ActualTimeOut_2, 2) as [Actual Time Out 2]
		                        , LEFT(Scm_ShiftTimeIn, 2) + ':' + RIGHT(Scm_ShiftTimeIn, 2) as [Shift Time In]
		                        , LEFT(Scm_ShiftBreakStart, 2) + ':' + RIGHT(Scm_ShiftBreakStart, 2) as [Start of Break]
		                        , LEFT(Scm_ShiftBreakEnd, 2) + ':' + RIGHT(Scm_ShiftBreakEnd, 2) as [End of Break]
		                        , LEFT(Scm_ShiftTimeOut, 2) + ':' + RIGHT(Scm_ShiftTimeOut, 2) as [Shift Time Out]
                                , Ell_WorkType as [Work Type]
		                        , Ell_WorkGroup as [Work Group]
		                        , LocationCode.Adt_AccountDesc as [Location]
		                        , PayrollType.Adt_AccountDesc as [Payroll Type] 
		                        , EmploymentStatus.Adt_AccountDesc AS [Employment Status]
		                        , JobStatus.Adt_AccountDesc AS [Job Status]
		                        , Case when Ell_AssumedPostBack = 'T' then 'TEMP LOGS'
			                         when Ell_AssumedPostBack = 'A' and Ell_ActualTimeIn_1 + Ell_ActualTimeOut_1 + Ell_ActualTimeIn_2 + Ell_ActualTimeOut_2 = '0000000000000000' then 'ACTUAL NO LOGS'
			                        when Ell_AssumedPostBack = 'A' and Ell_ActualTimeIn_1 + Ell_ActualTimeOut_1 + Ell_ActualTimeIn_2 + Ell_ActualTimeOut_2 <> '0000000000000000' then 'ACTUAL WITH LOGS'
			                        else ' ' end as [Remarks]
                        FROM {0}..T_EmployeeLogLedgerHist
                        INNER JOIN {0}..T_EmployeeMaster on {0}..T_EmployeeMaster.Emt_EmployeeID = {0}..T_EmployeeLogLedgerHist.Ell_EmployeeId
                        INNER JOIN {0}..T_EmployeeMasterHist on {0}..T_EmployeeMasterHist.Emt_EmployeeID = {0}..T_EmployeeLogLedgerHist.Ell_EmployeeId
	                        AND {0}..T_EmployeeMasterHist.Emt_Payperiod = {0}..T_EmployeeLogLedgerHist.Ell_PayPeriod
                        INNER JOIN {0}..T_ShiftCodeMaster on Scm_ShiftCode = Ell_ShiftCode
                        LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = {0}..T_EmployeeMasterHist.Emt_PayrollType 
	                        AND  PayrollType.Adt_AccountType = 'PAYTYPE'
                        LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = {0}..T_EmployeeMasterHist.Emt_EmploymentStatus 
	                        AND EmploymentStatus.Adt_AccountType = 'EMPSTAT'
                        LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = {0}..T_EmployeeMasterHist.Emt_JobStatus    
	                        AND JobStatus.Adt_AccountType = 'JOBSTATUS'
	                    LEFT JOIN {0}..T_AccountDetail LocationCode on LocationCode.Adt_AccountCode = Ell_LocationCode
							and LocationCode.Adt_AccountType = 'ZIPCODE'
                        {1}
                        ";
                        #endregion
                        break;
                    case "Overtime Report":
                        #region query
                        query = @"
                        SELECT 	 Eot_EmployeeId as [ID Number]
		                        , Emt_LastName as [Last Name]
		                        , Emt_FirstName as [First Name]
		                        , Eot_CostCenter as  [Costcenter]
		                        , dbo.getCostCenterFullNameV2(Eot_CostCenter) as [Description]
		                        , Eot_CurrentPayPeriod as [This Payperiod]
                                                                                   , Convert(Date,Eot_OvertimeDate) as [Overtime Date]
		                        , Eot_AppliedDate as [Applied Date]
		                        , CASE Eot_OvertimeType when 'A' then 'ADVANCE'
			                        when 'M' then 'MID'
			                        when 'P' then 'POST'
			                        else '' END as [Overtime Type]
		                        , LEFT(Eot_StartTime, 2) + ':' + RIGHT(Eot_StartTime,2) as [Start Time]
		                        , LEFT(Eot_EndTime, 2) + ':' + RIGHT(Eot_EndTime, 2) as [End Time]
		                        , Eot_OvertimeHour as [Overtime Hours]
		                        , Eot_Reason as [Reason of Overtime]
		                        , Eot_EndorsedDateToChecker as [Endorse Date to Checker]
		                        , Eot_CheckedBy as [Checker 1]
		                        , CASE WHEN UserMaster2.Mur_UserCode IS NULL THEN Eot_CheckedBy ELSE Rtrim(UserMaster2.Umt_userlname)  + ', ' + RTrim(UserMaster2.Umt_userfname) END AS [Checker 1 Name]
		                        , Eot_CheckedDate as [Check Date 1]
		                        , Eot_Checked2By as [Checker 2]
		                        , CASE WHEN UserMaster3.Mur_UserCode IS NULL THEN Eot_Checked2By ELSE Rtrim(UserMaster3.Umt_userlname)  +  ', ' + RTrim(UserMaster3.Umt_userfname) END AS [Checker 2 Name]
		                        , Eot_Checked2Date as [Check Date 2]
		                        , Eot_ApprovedBy as [Approver]
		                        , CASE WHEN UserMaster4.Mur_UserCode IS NULL THEN Eot_ApprovedBy ELSE Rtrim(UserMaster4.Umt_userlname)  + ', ' + RTrim(UserMaster4.Umt_userfname) END AS [Approver Name]  
		                        , Eot_ApprovedDate as [Approve Date]
		                        , WFStatus.Adt_AccountDesc as [Status]
		                        , Eot_ControlNo as [Transaction Control No.]
		                        , Eot_BatchNo as [Batch Control No.]
		                        , CycIndic.Adt_AccountDesc as [Overtime Flag]
		                        , PayrollType.Adt_AccountDesc as [Payroll Type] 
		                        , EmploymentStatus.Adt_AccountDesc AS [Employment Status]
		                        , JobStatus.Adt_AccountDesc AS [Job Status]
                        From {0}..T_EmployeeOvertime
                        INNER JOIN {0}..T_EmployeeMaster on Emt_EmployeeID = Eot_EmployeeID
                        LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = Emt_PayrollType 
	                        AND  PayrollType.Adt_AccountType = 'PAYTYPE'
                        LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = Emt_EmploymentStatus 
	                        AND EmploymentStatus.Adt_AccountType = 'EMPSTAT'
                        LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = Emt_JobStatus    
	                        AND JobStatus.Adt_AccountType = 'JOBSTATUS'
                        LEFT JOIN  {0}..T_AccountDetail  WFStatus ON WFStatus.Adt_AccountCode = Eot_Status    
	                        AND WFStatus.Adt_AccountType = 'WFSTATUS'
                        LEFT JOIN  {0}..T_AccountDetail  CycIndic ON CycIndic.Adt_AccountCode = Eot_OvertimeFlag
	                        AND CycIndic.Adt_AccountType = 'CYCLEINDIC'
                        LEFT JOIN  {0}..M_User AS UserMaster2 ON UserMaster2.Mur_UserCode = Eot_CheckedBy
                        LEFT JOIN  {0}..M_User AS UserMaster3 ON UserMaster3.Mur_UserCode = Eot_Checked2By
                        LEFT JOIN  {0}..M_User AS UserMaster4 ON UserMaster4.Mur_UserCode = Eot_ApprovedBy 
                        {1}

                        UNION ALL

                        SELECT 	 Eot_EmployeeId as [ID Number]
		                        , Emt_LastName as [Last Name]
		                        , Emt_FirstName as [First Name]
		                        , Eot_CostCenter as  [Costcenter]
		                        , dbo.getCostCenterFullNameV2(Eot_CostCenter) as [Description]
		                        , Eot_CurrentPayPeriod as [This Payperiod]
		                        , Convert(Date,Eot_OvertimeDate) as [Overtime Date]
		                        , Eot_AppliedDate as [Applied Date]
		                        , CASE Eot_OvertimeType when 'A' then 'ADVANCE'
			                        when 'M' then 'MID'
			                        when 'P' then 'POST'
			                        else '' END as [Overtime Type]
		                        , LEFT(Eot_StartTime, 2) + ':' + RIGHT(Eot_StartTime,2) as [Start Time]
		                        , LEFT(Eot_EndTime, 2) + ':' + RIGHT(Eot_EndTime, 2) as [End Time]
		                        , Eot_OvertimeHour as [Overtime Hours]
		                        , Eot_Reason as [Reason of Overtime]
		                        , Eot_EndorsedDateToChecker as [Endorse Date to Checker]
		                        , Eot_CheckedBy as [Checker 1]
		                        , CASE WHEN UserMaster2.Mur_UserCode IS NULL THEN Eot_CheckedBy ELSE Rtrim(UserMaster2.Umt_userlname)  + ', ' + RTrim(UserMaster2.Umt_userfname) END AS [Checker 1 Name]
		                        , Eot_CheckedDate as [Check Date 1]
		                        , Eot_Checked2By as [Checker 2]
		                        , CASE WHEN UserMaster3.Mur_UserCode IS NULL THEN Eot_Checked2By ELSE Rtrim(UserMaster3.Umt_userlname)  +  ', ' + RTrim(UserMaster3.Umt_userfname) END AS [Checker 2 Name]
		                        , Eot_Checked2Date as [Check Date 2]
		                        , Eot_ApprovedBy as [Approver]
		                        , CASE WHEN UserMaster4.Mur_UserCode IS NULL THEN Eot_ApprovedBy ELSE Rtrim(UserMaster4.Umt_userlname)  + ', ' + RTrim(UserMaster4.Umt_userfname) END AS [Approver Name]  
		                        , Eot_ApprovedDate as [Approve Date]
		                        , WFStatus.Adt_AccountDesc as [Status]
		                        , Eot_ControlNo as [Transaction Control No.]
		                        , Eot_BatchNo as [Batch Control No.]
		                        , CycIndic.Adt_AccountDesc as [Overtime Flag]
		                        , PayrollType.Adt_AccountDesc as [Payroll Type] 
		                        , EmploymentStatus.Adt_AccountDesc AS [Employment Status]
		                        , JobStatus.Adt_AccountDesc AS [Job Status]
                        From {0}..T_EmployeeOvertimeHist
                        INNER JOIN {0}..T_EmployeeMaster on Emt_EmployeeID = Eot_EmployeeID
                        LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = Emt_PayrollType 
	                        AND  PayrollType.Adt_AccountType = 'PAYTYPE'
                        LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = Emt_EmploymentStatus 
	                        AND EmploymentStatus.Adt_AccountType = 'EMPSTAT'
                        LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = Emt_JobStatus    
	                        AND JobStatus.Adt_AccountType = 'JOBSTATUS'
                        LEFT JOIN  {0}..T_AccountDetail  WFStatus ON WFStatus.Adt_AccountCode = Eot_Status    
	                        AND WFStatus.Adt_AccountType = 'WFSTATUS'
                        LEFT JOIN  {0}..T_AccountDetail  CycIndic ON CycIndic.Adt_AccountCode = Eot_OvertimeFlag
	                        AND CycIndic.Adt_AccountType = 'CYCLEINDIC'
                        LEFT JOIN  {0}..M_User AS UserMaster2 ON UserMaster2.Mur_UserCode = Eot_CheckedBy
                        LEFT JOIN  {0}..M_User AS UserMaster3 ON UserMaster3.Mur_UserCode = Eot_Checked2By
                        LEFT JOIN  {0}..M_User AS UserMaster4 ON UserMaster4.Mur_UserCode = Eot_ApprovedBy 
                        {1}
                        ";
                        #endregion
                        break;
                    case "Leave Availment Report":
                        #region query
                        query = @"
                        SELECT 	 Elt_EmployeeId as [ID Number]
		                        , Emt_LastName as [Last Name]
		                        , Emt_FirstName as [First Name]
		                        , Elt_CostCenter as  [Costcenter]
		                        , dbo.getCostCenterFullNameV2(Elt_CostCenter) as [Description]
		                        , Elt_CurrentPayPeriod as [This Payperiod]
		                        , Convert(Date,Elt_LeaveDate) as [Leave Date]
		                        , Elt_InformDate as [Inform Date]
		                        , Elt_AppliedDate as [Applied Date]
		                        , Elt_LeaveType as [Leave Type]
		                        , Elt_LeaveCategory as [Leave Category]
		                        , Elt_Leavecode as [Leave Code]
		                        , LEFT(Elt_StartTime, 2) + ':' + RIGHT(Elt_StartTime,2) as [Start Time]
		                        , LEFT(Elt_EndTime, 2) + ':' + RIGHT(Elt_EndTime, 2) as [End Time]
		                        , Elt_LeaveHour as [Leave Hours]
		                        , Elt_DayUnit as [Day Unit]
		                        , CASE WHEN Elt_LeaveNotice = 1 THEN 'YES' ELSE 'NO' END	as [With Leave Notice]
		                        , Elt_Reason as [Reason of Leave]
		                        , Elt_EndorsedDateToChecker as [Endorse Date to Checker]
		                        , Elt_CheckedBy as [Checker 1]
		                        , CASE WHEN UserMaster2.Mur_UserCode IS NULL THEN Elt_CheckedBy ELSE Rtrim(UserMaster2.Umt_userlname)  + ', ' + RTrim(UserMaster2.Umt_userfname) END AS [Checker 1 Name]
		                        , Elt_CheckedDate as [Check Date 1]
		                        , Elt_Checked2By as [Checker 2]
		                        , CASE WHEN UserMaster3.Mur_UserCode IS NULL THEN Elt_Checked2By ELSE Rtrim(UserMaster3.Umt_userlname)  +  ', ' + RTrim(UserMaster3.Umt_userfname) END AS [Checker 2 Name]
		                        , Elt_Checked2Date as [Check Date 2]
		                        , Elt_ApprovedBy as [Approver]
		                        , CASE WHEN UserMaster4.Mur_UserCode IS NULL THEN Elt_ApprovedBy ELSE Rtrim(UserMaster4.Umt_userlname)  + ', ' + RTrim(UserMaster4.Umt_userfname) END AS [Approver Name]  
		                        , Elt_ApprovedDate as [Approve Date]
		                        , WFStatus.Adt_AccountDesc as [Status]
		                        , Elt_ControlNo as [Transaction Control No.]
		                        , CycIndic.Adt_AccountDesc as [Overtime Flag]
		                        , PayrollType.Adt_AccountDesc as [Payroll Type] 
		                        , EmploymentStatus.Adt_AccountDesc AS [Employment Status]
		                        , JobStatus.Adt_AccountDesc AS [Job Status]
                        From {0}..T_EmployeeLeaveAvailment
                        INNER JOIN {0}..T_EmployeeMaster on Emt_EmployeeID = Elt_EmployeeID
                        LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = Emt_PayrollType 
	                        AND  PayrollType.Adt_AccountType = 'PAYTYPE'
                        LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = Emt_EmploymentStatus 
	                        AND EmploymentStatus.Adt_AccountType = 'EMPSTAT'
                        LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = Emt_JobStatus    
	                        AND JobStatus.Adt_AccountType = 'JOBSTATUS'
                        LEFT JOIN  {0}..T_AccountDetail  WFStatus ON WFStatus.Adt_AccountCode = Elt_Status    
	                        AND WFStatus.Adt_AccountType = 'WFSTATUS'
                        LEFT JOIN  {0}..T_AccountDetail  CycIndic ON CycIndic.Adt_AccountCode = Elt_LeaveFlag
	                        AND CycIndic.Adt_AccountType = 'CYCLEINDIC'
                        LEFT JOIN  {0}..M_User AS UserMaster2 ON UserMaster2.Mur_UserCode = Elt_CheckedBy
                        LEFT JOIN  {0}..M_User AS UserMaster3 ON UserMaster3.Mur_UserCode = Elt_Checked2By
                        LEFT JOIN  {0}..M_User AS UserMaster4 ON UserMaster4.Mur_UserCode = Elt_ApprovedBy 
                        {1}

                        UNION ALL

                        SELECT 	 Elt_EmployeeId as [ID Number]
		                        , Emt_LastName as [Last Name]
		                        , Emt_FirstName as [First Name]
		                        , Elt_CostCenter as  [Costcenter]
		                        , dbo.getCostCenterFullNameV2(Elt_CostCenter) as [Description]
		                        , Elt_CurrentPayPeriod as [This Payperiod]
		                        , Convert(Date,Elt_LeaveDate) as [Leave Date]
		                        , Elt_InformDate as [Inform Date]
		                        , Elt_AppliedDate as [Applied Date]
		                        , Elt_LeaveType as [Leave Type]
		                        , Elt_LeaveCategory as [Leave Category]
		                        , Elt_Leavecode as [Leave Code]
		                        , LEFT(Elt_StartTime, 2) + ':' + RIGHT(Elt_StartTime,2) as [Start Time]
		                        , LEFT(Elt_EndTime, 2) + ':' + RIGHT(Elt_EndTime, 2) as [End Time]
		                        , Elt_LeaveHour as [Leave Hours]
		                        , Elt_DayUnit as [Day Unit]
		                        , CASE WHEN Elt_LeaveNotice = 1 THEN 'YES' ELSE 'NO' END	as [With Leave Notice]
		                        , Elt_Reason as [Reason of Leave]
		                        , Elt_EndorsedDateToChecker as [Endorse Date to Checker]
		                        , Elt_CheckedBy as [Checker 1]
		                        , CASE WHEN UserMaster2.Mur_UserCode IS NULL THEN Elt_CheckedBy ELSE Rtrim(UserMaster2.Umt_userlname)  + ', ' + RTrim(UserMaster2.Umt_userfname) END AS [Checker 1 Name]
		                        , Elt_CheckedDate as [Check Date 1]
		                        , Elt_Checked2By as [Checker 2]
		                        , CASE WHEN UserMaster3.Mur_UserCode IS NULL THEN Elt_Checked2By ELSE Rtrim(UserMaster3.Umt_userlname)  +  ', ' + RTrim(UserMaster3.Umt_userfname) END AS [Checker 2 Name]
		                        , Elt_Checked2Date as [Check Date 2]
		                        , Elt_ApprovedBy as [Approver]
		                        , CASE WHEN UserMaster4.Mur_UserCode IS NULL THEN Elt_ApprovedBy ELSE Rtrim(UserMaster4.Umt_userlname)  + ', ' + RTrim(UserMaster4.Umt_userfname) END AS [Approver Name]  
		                        , Elt_ApprovedDate as [Approve Date]
		                        , WFStatus.Adt_AccountDesc as [Status]
		                        , Elt_ControlNo as [Transaction Control No.]
		                        , CycIndic.Adt_AccountDesc as [Overtime Flag]
		                        , PayrollType.Adt_AccountDesc as [Payroll Type] 
		                        , EmploymentStatus.Adt_AccountDesc AS [Employment Status]
		                        , JobStatus.Adt_AccountDesc AS [Job Status]
                        From {0}..T_EmployeeLeaveAvailmentHist
                        INNER JOIN {0}..T_EmployeeMaster on Emt_EmployeeID = Elt_EmployeeID
                        LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = Emt_PayrollType 
	                        AND  PayrollType.Adt_AccountType = 'PAYTYPE'
                        LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = Emt_EmploymentStatus 
	                        AND EmploymentStatus.Adt_AccountType = 'EMPSTAT'
                        LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = Emt_JobStatus    
	                        AND JobStatus.Adt_AccountType = 'JOBSTATUS'
                        LEFT JOIN  {0}..T_AccountDetail  WFStatus ON WFStatus.Adt_AccountCode = Elt_Status    
	                        AND WFStatus.Adt_AccountType = 'WFSTATUS'
                        LEFT JOIN  {0}..T_AccountDetail  CycIndic ON CycIndic.Adt_AccountCode = Elt_LeaveFlag
	                        AND CycIndic.Adt_AccountType = 'CYCLEINDIC'
                        LEFT JOIN  {0}..M_User AS UserMaster2 ON UserMaster2.Mur_UserCode = Elt_CheckedBy
                        LEFT JOIN  {0}..M_User AS UserMaster3 ON UserMaster3.Mur_UserCode = Elt_Checked2By
                        LEFT JOIN  {0}..M_User AS UserMaster4 ON UserMaster4.Mur_UserCode = Elt_ApprovedBy 
                        {1}
                        ";
                        #endregion
                        break;
                    case "Leave Ledger Report":
                        #region query
                        if (category)
                        {
                            query = @"
                            SELECT 	 Elm_EmployeeId as [ID Number]
				                            , Emt_LastName as [Last Name]
				                            , Emt_FirstName as [First Name]
				                            , Emt_CostCenterCode as  [Costcenter]
				                            , dbo.getCostCenterFullNameV2(Emt_CostCenterCode) as [Description]
				                            , Elm_LeaveYear as [Leave Year]
				                            , Elm_Leavetype as [Leave Type]
				                            , Elm_Entitled as [Entitled]
				                            , Elm_Used as [Used]
				                            , Elm_Reserved as [Reserved]
				                            , Elm_Entitled - Elm_Used as [Balance]
				                            , PayrollType.Adt_AccountDesc as [Payroll Type] 
				                            , EmploymentStatus.Adt_AccountDesc AS [Employment Status]
				                            , JobStatus.Adt_AccountDesc AS [Job Status]
		                            From {0}..T_EmployeeLeave
		                            INNER JOIN {0}..T_EmployeeMaster on Emt_EmployeeID = Elm_EmployeeID
		                            LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = Emt_PayrollType 
			                            AND  PayrollType.Adt_AccountType = 'PAYTYPE'
		                            LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = Emt_EmploymentStatus 
			                            AND EmploymentStatus.Adt_AccountType = 'EMPSTAT'
		                            LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = Emt_JobStatus    
			                            AND JobStatus.Adt_AccountType = 'JOBSTATUS'
		                            {1}

		                            UNION ALL

		                            SELECT 	 Elm_EmployeeId as [ID Number]
				                            , Emt_LastName as [Last Name]
				                            , Emt_FirstName as [First Name]
				                            , Emt_CostCenterCode as  [Costcenter]
				                            , dbo.getCostCenterFullNameV2(Emt_CostCenterCode) as [Description]
				                            , Elm_LeaveYear as [Leave Year]
				                            , Elm_Leavetype as [Leave Type]
				                            , Elm_Entitled as [Entitled]
				                            , Elm_Used as [Used]
				                            , Elm_Reserved as [Reserved]
				                            , Elm_Entitled - Elm_Used as [Balance]
				                            , PayrollType.Adt_AccountDesc as [Payroll Type] 
				                            , EmploymentStatus.Adt_AccountDesc AS [Employment Status]
				                            , JobStatus.Adt_AccountDesc AS [Job Status]
		                            From {0}..T_EmployeeLeaveHist
		                            INNER JOIN {0}..T_EmployeeMaster on Emt_EmployeeID = Elm_EmployeeID
		                            LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = Emt_PayrollType 
			                            AND  PayrollType.Adt_AccountType = 'PAYTYPE'
		                            LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = Emt_EmploymentStatus 
			                            AND EmploymentStatus.Adt_AccountType = 'EMPSTAT'
		                            LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = Emt_JobStatus    
			                            AND JobStatus.Adt_AccountType = 'JOBSTATUS'
		                            {1}
                            ";
                        }
                        else
                        {
                            query = @"
                            SELECT 	 Elm_EmployeeId as [ID Number]
				                            , Emt_LastName as [Last Name]
				                            , Emt_FirstName as [First Name]
				                            , Emt_CostCenterCode as  [Costcenter]
				                            , dbo.getCostCenterFullNameV2(Emt_CostCenterCode) as [Description]
				                            , Elm_LeaveYear as [Leave Year]
				                            , Elm_Leavetype as [Leave Type]
				                            , Elm_Entitled as [Entitled]
				                            , Elm_Used/8.000 as [Used]
				                            , Elm_Reserved/8.000 as [Reserved]
				                            , (Elm_Entitled - Elm_Used)/8.000 as [Balance]
				                            , PayrollType.Adt_AccountDesc as [Payroll Type] 
				                            , EmploymentStatus.Adt_AccountDesc AS [Employment Status]
				                            , JobStatus.Adt_AccountDesc AS [Job Status]
		                            From {0}..T_EmployeeLeave
		                            INNER JOIN {0}..T_EmployeeMaster on Emt_EmployeeID = Elm_EmployeeID
		                            LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = Emt_PayrollType 
			                            AND  PayrollType.Adt_AccountType = 'PAYTYPE'
		                            LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = Emt_EmploymentStatus 
			                            AND EmploymentStatus.Adt_AccountType = 'EMPSTAT'
		                            LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = Emt_JobStatus    
			                            AND JobStatus.Adt_AccountType = 'JOBSTATUS'
		                            {1}
                            		
		                            UNION ALL
                            		
	                               SELECT 	 Elm_EmployeeId as [ID Number]
				                            , Emt_LastName as [Last Name]
				                            , Emt_FirstName as [First Name]
				                            , Emt_CostCenterCode as  [Costcenter]
				                            , dbo.getCostCenterFullNameV2(Emt_CostCenterCode) as [Description]
				                            , Elm_LeaveYear as [Leave Year]
				                            , Elm_Leavetype as [Leave Type]
				                            , Elm_Entitled/8.000 as [Entitled]
				                            , Elm_Used/8.000 as [Used]
				                            , Elm_Reserved/8.000 as [Reserved]
				                            , (Elm_Entitled - Elm_Used)/8.000 as [Balance]
				                            , PayrollType.Adt_AccountDesc as [Payroll Type] 
				                            , EmploymentStatus.Adt_AccountDesc AS [Employment Status]
				                            , JobStatus.Adt_AccountDesc AS [Job Status]
		                            From {0}..T_EmployeeLeaveHist
		                            INNER JOIN {0}..T_EmployeeMaster on Emt_EmployeeID = Elm_EmployeeID
		                            LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = Emt_PayrollType 
			                            AND  PayrollType.Adt_AccountType = 'PAYTYPE'
		                            LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = Emt_EmploymentStatus 
			                            AND EmploymentStatus.Adt_AccountType = 'EMPSTAT'
		                            LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = Emt_JobStatus    
			                            AND JobStatus.Adt_AccountType = 'JOBSTATUS'
		                            {1}
                            ";
                        }
                        #endregion
                        break;
                    case "DTR Report":
                        #region query
                        query = @"
                        SELECT Tel_IDNo AS  [ID Number]
	                        , Emt_LastName AS [Last Name]
	                        , Emt_FirstName AS [First Name]
	                        , CONVERT(date, Tel_LogDate) AS [Log Date]
	                        , Tel_LogTime AS [Log Time]
	                        , Tel_LogType AS [Log Type]
	                        , Tel_StationNo AS [Station Number]  
	                        , PayrollType.Adt_AccountDesc as [Payroll Type] 
	                        , EmploymentStatus.Adt_AccountDesc AS [Employment Status]
	                        , JobStatus.Adt_AccountDesc AS [Job Status]
                        FROM  {0}..T_EmpDTR DTR
                        INNER JOIN {1}..T_EmployeeMaster ON Emt_EmployeeID = Tel_IDNo  
                        LEFT  JOIN  {1}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = Emt_PayrollType 
	                        AND  PayrollType.Adt_AccountType = 'PAYTYPE'
                        LEFT JOIN {1}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = Emt_EmploymentStatus 
	                        AND EmploymentStatus.Adt_AccountType = 'EMPSTAT'
                        LEFT JOIN  {1}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = Emt_JobStatus    
	                        AND JobStatus.Adt_AccountType = 'JOBSTATUS'
	                        {2}
                        ";
                        #endregion
                        break;
                    case "Work Location Report":
                        #region query
                        query = @"
                        SELECT  Ewl_EmployeeId as [IDNumber]
		                        , Emt_LastName as [Last Name]
		                        , Emt_FirstName as [First Name]
		                        , Emt_CostCenterCode as  [Costcenter]
		                        , dbo.getCostCenterFullNameV2(Emt_CostCenterCode) as [Description]
		                        , CONVERT(date, Ewl_EffectivityDate) as [Effectivity Date]
		                        , Location.Adt_AccountDesc AS [Work Location]
		                        , PayrollType.Adt_AccountDesc as [Payroll Type] 
		                        , EmploymentStatus.Adt_AccountDesc AS [Employment Status]
		                        , JobStatus.Adt_AccountDesc AS [Job Status]
                        FROM {0}..T_Employeeworklocation
                        INNER JOIN {0}..T_EmployeeMaster on Emt_EmployeeID = Ewl_EmployeeId
                        LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = Emt_PayrollType 
	                        AND  PayrollType.Adt_AccountType = 'PAYTYPE'
                        LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = Emt_EmploymentStatus 
	                        AND EmploymentStatus.Adt_AccountType = 'EMPSTAT'
                        LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = Emt_JobStatus    
	                        AND JobStatus.Adt_AccountType = 'JOBSTATUS'
                        LEFT JOIN  {0}..T_AccountDetail  Location ON Location.Adt_AccountCode = Ewl_LocationCode    
	                        AND Location.Adt_AccountType = 'ZIPCODE'
	                        {1}
                        ";
                        #endregion
                        break;
                    case "Restday Report":
                        #region query 
                        query = @"
                        SELECT  Erd_EmployeeID as [IDNumber]
		                        , Emt_LastName as [Last Name]
		                        , Emt_FirstName as [First Name]
		                        , Emt_CostCenterCode as  [Costcenter]
		                        , dbo.getCostCenterFullNameV2(Emt_CostCenterCode) as [Description]
		                        , CONVERT(date, Erd_EffectivityDate) as [Effectivity Date]
		                        , Rtrim(CASE WHEN Left(Erd_RestDay,1) = '1' then '- Mon' ELSE '' END +      
		                        CASE WHEN LEN(CASE WHEN Substring(Erd_RestDay,2,1) = '1' then 'Tue' ELSE '' END) = 0 then '' ELSE '- Tue' END +      
		                        CASE WHEN LEN(CASE WHEN Substring(Erd_RestDay,3,1) = '1' then 'Wed' ELSE '' END) = 0 then '' ELSE '- Wed' END +      
		                        CASE WHEN LEN(CASE WHEN Substring(Erd_RestDay,4,1) = '1' then 'Thu' ELSE '' END) = 0 then '' ELSE '- Thu' END +      
		                        CASE WHEN LEN(CASE WHEN Substring(Erd_RestDay,5,1) = '1' then 'Fri' ELSE '' END) = 0 then '' ELSE '- Fri' END +      
		                        CASE WHEN LEN(CASE WHEN Substring(Erd_RestDay,6,1) = '1' then 'Sat' ELSE '' END) = 0 then '' ELSE '- Sat' END +      
		                        CASE WHEN LEN(CASE WHEN Substring(Erd_RestDay,7,1) = '1' then 'Sun' ELSE '' END) = 0 then '' ELSE '- Sun' END) as [Rest Day]
		                        , PayrollType.Adt_AccountDesc as [Payroll Type] 
		                        , EmploymentStatus.Adt_AccountDesc AS [Employment Status]
		                        , JobStatus.Adt_AccountDesc AS [Job Status]
                        FROM {0}..T_EmployeeRestday
                        INNER JOIN {0}..T_EmployeeMaster on Emt_EmployeeID = Erd_EmployeeId
                        LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = Emt_PayrollType 
	                        AND  PayrollType.Adt_AccountType = 'PAYTYPE'
                        LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = Emt_EmploymentStatus 
	                        AND EmploymentStatus.Adt_AccountType = 'EMPSTAT'
                        LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = Emt_JobStatus    
	                        AND JobStatus.Adt_AccountType = 'JOBSTATUS'
	                        {1}
                        ";
                        #endregion
                        break;
                    case "Post-Payroll Report":
                        #region query
                        query = @"
                        SELECT  Per_EmployeeId as [IDNumber]
		                        , Emt_LastName as [Last Name]
		                        , Emt_FirstName as [First Name]
		                        , Emt_CostCenterCode as  [Costcenter]
		                        , dbo.getCostCenterFullNameV2(Emt_CostCenterCode) as [Description]
		                        , Per_CurrentPayPeriod as [This Period]
		                        , Per_Remarks as [Remarks]
		                        ,  CASE WHEN Per_Adjustment = 1 THEN 'YES' ELSE 'NO' END	as [For Adjustment] 
		                        , PayrollType.Adt_AccountDesc as [Payroll Type] 
		                        , EmploymentStatus.Adt_AccountDesc AS [Employment Status]
		                        , JobStatus.Adt_AccountDesc AS [Job Status]
                        FROM {0}..T_PayrollError
                        INNER JOIN {0}..T_EmployeeMaster on Emt_EmployeeID = Per_EmployeeId
                        LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = Emt_PayrollType 
	                        AND  PayrollType.Adt_AccountType = 'PAYTYPE'
                        LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = Emt_EmploymentStatus 
	                        AND EmploymentStatus.Adt_AccountType = 'EMPSTAT'
                        LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = Emt_JobStatus    
	                        AND JobStatus.Adt_AccountType = 'JOBSTATUS'
                        {1}

                        UNION ALL

                        SELECT  Per_EmployeeId as [IDNumber]
		                        , Emt_LastName as [Last Name]
		                        , Emt_FirstName as [First Name]
		                        , {0}..T_EmployeeMasterHist.Emt_CostCenterCode as  [Costcenter]
		                        , dbo.getCostCenterFullNameV2({0}..T_EmployeeMasterHist.Emt_CostCenterCode) as [Description]
		                        , Per_CurrentPayPeriod as [This Period]
		                        , Per_Remarks as [Remarks]
		                        ,  CASE WHEN Per_Adjustment = 1 THEN 'YES' ELSE 'NO' END	as [For Adjustment] 
		                        , PayrollType.Adt_AccountDesc as [Payroll Type] 
		                        , EmploymentStatus.Adt_AccountDesc AS [Employment Status]
		                        , JobStatus.Adt_AccountDesc AS [Job Status]
                        FROM {0}..T_PayrollErrorHist
                        INNER JOIN {0}..T_EmployeeMasterHist on {0}..T_EmployeeMasterHist.Emt_EmployeeID = Per_EmployeeId
                        and {0}..T_EmployeeMasterHist.Emt_Payperiod = Per_CurrentPayPeriod
                        INNER JOIN {0}..T_EmployeeMaster on {0}..T_EmployeeMaster.Emt_EmployeeID = Per_EmployeeId
                        LEFT  JOIN  {0}..T_AccountDetail PayrollType ON PayrollType.Adt_AccountCode = {0}..T_EmployeeMasterHist.Emt_PayrollType 
	                        AND  PayrollType.Adt_AccountType = 'PAYTYPE'
                        LEFT JOIN {0}..T_AccountDetail  EmploymentStatus ON EmploymentStatus.Adt_AccountCode = {0}..T_EmployeeMasterHist.Emt_EmploymentStatus 
	                        AND EmploymentStatus.Adt_AccountType = 'EMPSTAT'
                        LEFT JOIN  {0}..T_AccountDetail  JobStatus ON JobStatus.Adt_AccountCode = {0}..T_EmployeeMasterHist.Emt_JobStatus    
	                        AND JobStatus.Adt_AccountType = 'JOBSTATUS'
                        {1}
                        	
                        	
                        ";
                        #endregion
                        break;

                }
                #endregion
            }

            return query;
        }

        public bool GetProcessControlForLeave()
        {
            string query = @"
                    SELECT Pcm_ProcessFlag FROM T_ProcessControlMaster WHERE Pcm_SystemID = 'LEAVE' and Pcm_ProcessID = 'LVHRENTRY'
                    ";
            bool ret = false;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ret = Convert.ToBoolean(dal.ExecuteScalar(query, CommandType.Text));
                dal.CloseDB();
            }
            return ret;
        }

        public DataTable GetAllowanceCode()
        {
            string query = @"
                Select 
	                Acm_AllowanceCode [Allowance Code],
	                Acm_AllowanceDesc [Allowance Desc]
                From T_AllowanceCodeMaster
                where Acm_Status = 'A' ";

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetAlphalistCat()
        {
            string query = @"
               		Select 
			            Adt_AccountCode [Code],
			            Adt_AccountDesc [Desc]
		            From T_AccountDetail
		            where Adt_AccountType = 'ALPHACATGY'
                    And Adt_Status = 'A'";

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetApplicablePeriod()
        { 
            string query = @"
               		 select 
				        Adt_AccountCode [Code],
				        Adt_AccountDesc [Desc]
			        from T_AccountDetail
			        where Adt_AccountType = 'DEDNFREQ'
			        and Adt_Status = 'A'
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

        public DataTable GetEmployeeList2()
        {
            string query = @"
               		 select 
				        Emt_EmployeeID [ID],
				        Emt_LastName + ', ' + Emt_FirstName + ' ' + 
				        Case Emt_MiddleName
					        when ''
					        then ''
					        else Emt_MiddleName
					        end [Employee Name]
			        from T_EmployeeMaster
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
        
        public DataTable GetJobStatus(bool accessRights, string userlogged, string costcenters, string SystemID)
        {

            string cond = string.Empty;

            string accessQuery = @"
                    select
                       distinct Uca_JobStatus
                    from T_UserCostCenterAccess
                    where Uca_Usercode = '{0}'
                    {1}
                    and Uca_SytemID = '{2}'
                    ";

            string query = @"
               		 SELECT [Adt_AccountCode] AS 'Status Code'
                                   ,[Adt_AccountDesc] AS 'Description'
                              FROM  [T_AccountDetail]
                             WHERE  [Adt_Status] = 'A'
                               AND  [Adt_AccountType] = 'JOBSTATUS'
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
                        cost = "and Uca_CostCenterCode in ( " + EncodeFilterItems(costcenters) + ")"; 
                    accessQuery = string.Format(accessQuery, userlogged, cost, SystemID);
                    DataTable dt = dal.ExecuteDataSet(accessQuery).Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        cond = "And Adt_AccountCode in (" + EncodeFilterItems(SetUpItemsForEncodeFilterItems(dt)) + ")";
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

        public DataTable GetEmploymentStatus(bool accessRights, string userlogged, string costcenters, string SystemID)
        {

            string cond = string.Empty;

            string accessQuery = @"
                    select
                       distinct Uca_EmploymentStatus
                    from T_UserCostCenterAccess
                    where Uca_Usercode = '{0}'
                    {1}
                    and Uca_SytemID = '{2}'
                    ";

            string query = @"
               		 SELECT [Adt_AccountCode] AS 'Status Code'
                                   ,[Adt_AccountDesc] AS 'Description'
                              FROM  [T_AccountDetail]
                             WHERE  [Adt_Status] = 'A'
                               AND  [Adt_AccountType] = 'EMPSTAT'
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
                        cost = "and Uca_CostCenterCode in ( " + EncodeFilterItems(costcenters) + ")";
                    accessQuery = string.Format(accessQuery, userlogged, cost, SystemID);
                    DataTable dt = dal.ExecuteDataSet(accessQuery).Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        cond = "And Adt_AccountCode in (" + EncodeFilterItems(SetUpItemsForEncodeFilterItems(dt)) + ")";
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

        public DataTable GetCostCenters2(string userlogged, string SystemID)
        {
            string accessQuery = @"
                    select
                       distinct Uca_CostCenterCode
                    from T_UserCostCenterAccess
                    where Uca_Usercode = '{0}'
                    and Uca_SytemID = '{1}'
                    ";

            accessQuery = string.Format(accessQuery, userlogged, SystemID);

            string cond = string.Empty;

            string query = @"
               		 select 
						Cct_CostCenterCode [Code],
						dbo.getCostCenterFullNameV2(Cct_CostCenterCode) [Description]
					from T_CostCenter
					where Cct_status = 'A'
                    {0}
                    ";

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                DataTable dt = dal.ExecuteDataSet(accessQuery).Tables[0];
                if (dt.Rows.Count > 0)
                {                    
                    if (dt.Rows[0][0].ToString() != "ALL")
                        cond = "And Cct_CostCenterCode in (" + EncodeFilterItems(SetUpItemsForEncodeFilterItems(dt)) + ")";
                    query = string.Format(query, cond);
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

        public DataTable GetPayPeriod2()
        { 
            string query = @"
               		 select 
						Ppm_PayPeriod [Payroll Period],
						CONVERT(varchar(15), Ppm_StartCycle, 101) + ' - ' + CONVERT(varchar(15), Ppm_EndCycle, 101) [Cycle]
					from T_PayPeriodMaster
					where Ppm_CycleIndicator in ('P', 'C')
					order by Ppm_CycleIndicator asc
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

        public DataTable GetDeductionCodes()
        {
            string query = @"
                    select 
	                    Dcm_DeductionCode [Deduction Code],
	                    Dcm_DeductionDesc [Description]
                    from T_DeductionCodeMaster
                    where Dcm_Status = 'A'
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

        public DataTable GetDeductionType()
        {
            string query = @"
                    select 
                        Adt_AccountCode [Deduction Type],
                        Adt_AccountDesc [Description]
                    from T_AccountDetail
                    where Adt_AccountType = 'DEDNTYPE'
                    and Adt_Status = 'A'
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

        public DataTable GetPaymentType()
        {
            string query = @"
                    select 
                         Adt_AccountCode [Payment Type],
                         Adt_AccountDesc [Description]
                     from T_AccountDetail
                    where Adt_AccountType = 'DEDPAYTYPE'
                    and Adt_Status = 'A'
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

        public string GetAllowedCostCeneterForUser(string userlogged, string systemID)
        {
            string ret = string.Empty;
            string query = @"
                        select 
                            distinct Uca_CostCenterCode
                        from T_UserCostCenterAccess
                        where Uca_SytemID = '{0}'
                        and Uca_Usercode = '{1}' 
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
                        from T_UserCostCenterAccess
                        where Uca_SytemID = '{0}'
                        and Uca_Usercode = '{1}'
                        and Uca_CostCenterCode in ({2})
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

        public DataTable GetDayCode()
        {
            string query = @"
                    select 
	                    Dcm_DayCode [Day Code],
	                    Dcm_DayDesc [Description]	
                    from T_DayCodeMaster
                    where Dcm_Status = 'A'

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

        public DataTable GetShiftCode()
        {            
             string query = @"
                    select
	                    Scm_ShiftCode [Shift Code],
	                    Scm_ShiftDesc [Description]
                    from T_ShiftCodeMaster
                    where Scm_Status = 'A'

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

        public DataTable GetWorkType()
        { 
             string query = @"
                    select 
                        Adt_AccountCode [Work Type Code],
                        Adt_AccountDesc [Description]	
                    from T_AccountDetail
                    where Adt_AccountType = 'WORKTYPE'
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

        public DataTable GetWorkGroup()
        {
            string query = @"
                    select 
                        Adt_AccountCode [Work Type Code],
                        Adt_AccountDesc [Description]	
                    from T_AccountDetail
                    where Adt_AccountType = 'WORKGROUP'
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

        public DataTable GetWorkLocation()
        { 
            string query = @"
                    select 
                        Adt_AccountCode [Location Code],
                        Adt_AccountDesc [Description]	 
                    from T_AccountDetail
                    where Adt_AccountType = 'ZIPCODE'
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

        public DataTable GetTransactionControlNo(string Transaction)
        {
            string query = string.Empty;

            if (Transaction == "OVERTIME")
            {
                query = @"
                select 
                    Eot_ControlNo [Control No.]
                from T_EmployeeOvertime
                union 
                select 
                    Eot_ControlNo [Control No.]
                from T_EmployeeOvertimeHist
                order by Eot_ControlNo asc                    
                ";
            }
            else if (Transaction == "LEAVE")
            {
                query = @"
                select 
	                Elt_ControlNo [Control No.] 
                from T_EmployeeLeaveAvailment
                union
                select 
	                Elt_ControlNo [Control No.] 
                from T_EmployeeLeaveAvailmentHist
                order by Elt_ControlNo asc
                ";
            }
            
            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetBatchControlNo()
        {
            string query  = @"
                select 
	                Eot_BatchNo [Batch No.] 
                from T_EmployeeOvertime
                union
                select 
	                Eot_BatchNo [Batch No.] 
                from T_EmployeeOvertimeHist
                order by Eot_BatchNo asc                 
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

        public DataTable GetCheckers(string Transaction, int routeNumber)
        {
            string query = string.Empty;

            if (Transaction == "OVERTIME")
            {
                #region
                if (routeNumber == 1)
                {
                    query = @"
                        select distinct
                            Eot_CheckedBy [Checker 1],
                            Umt_UserLName + ', ' + Umt_UserFName [Checker 1 Name]
                        from T_EmployeeOvertime
                        left outer join M_User
                        on Eot_CheckedBy = Mur_UserCode
                        where Eot_CheckedBy is not null
                        and  LTRIM( Eot_CheckedBy)  <> '' 
                        and UPPER(Eot_CheckedBy) <> 'SA' 

                        union

                        select distinct
                            Eot_CheckedBy [Checker 1],
                            Umt_UserLName + ', ' + Umt_UserFName [Checker 1 Name]
                        from T_EmployeeOvertimeHist
                        left outer join M_User
                        on Eot_CheckedBy = Mur_UserCode
                        where Eot_CheckedBy is not null
                        and  LTRIM( Eot_CheckedBy)  <> '' 
                        and UPPER(Eot_CheckedBy) <> 'SA' 

                         order by [Checker 1 Name] asc                 
                    ";
                }
                else if (routeNumber == 2)
                {
                    query = @"
                       select distinct
                            Eot_Checked2By [Checker 2],
                            Umt_UserLName + ', ' + Umt_UserFName [Checker 2 Name]
                        from T_EmployeeOvertime
                        left outer join M_User
                        on Eot_Checked2By = Mur_UserCode
                        where Eot_Checked2By is not null
                        and  LTRIM( Eot_Checked2By)  <> '' 
                        and UPPER(Eot_Checked2By) <> 'SA' 

                        union

                        select distinct
                            Eot_Checked2By [Checker 2],
                            Umt_UserLName + ', ' + Umt_UserFName [Checker 2 Name]
                        from T_EmployeeOvertimeHist
                        left outer join M_User
                        on Eot_Checked2By = Mur_UserCode
                        where Eot_Checked2By is not null
                        and  LTRIM( Eot_Checked2By)  <> '' 
                        and UPPER(Eot_Checked2By) <> 'SA' 

                        order by [Checker 2 Name] asc                  
                    ";
                }
                else if (routeNumber == 3)
                {
                    query = @"
                       select distinct
                            Eot_ApprovedBy [Approver],
                            Umt_UserLName + ', ' + Umt_UserFName [Approver Name]
                        from T_EmployeeOvertime
                        left outer join M_User
                        on Eot_ApprovedBy = Mur_UserCode
                        where Eot_ApprovedBy is not null
                        and  LTRIM( Eot_ApprovedBy)  <> '' 
                        and UPPER(Eot_ApprovedBy) <> 'SA' 

                        union

                        select distinct
                            Eot_ApprovedBy [Approver],
                            Umt_UserLName + ', ' + Umt_UserFName [Approver Name]
                        from T_EmployeeOvertimeHist
                        left outer join M_User
                        on Eot_ApprovedBy = Mur_UserCode
                        where Eot_ApprovedBy is not null
                        and  LTRIM( Eot_ApprovedBy)  <> '' 
                        and UPPER(Eot_ApprovedBy) <> 'SA' 

                        order by [Approver Name] asc                
                    ";
                }
                #endregion
            }
            else if (Transaction == "LEAVE")
            {
                #region
                if (routeNumber == 1)
                {
                    query = @"
                        select distinct
	                        Elt_CheckedBy [Checker 1],
	                        Umt_UserLName + ', ' + Umt_UserFName [Checker 1 Name]
                        from T_EmployeeLeaveAvailment
                        left outer join M_User
                        on Elt_CheckedBy = Mur_UserCode
                        where Elt_CheckedBy is not null
                        and  LTRIM( Elt_CheckedBy)  <> '' 
                        and UPPER(Elt_CheckedBy) <> 'SA' 

                        union

                        select distinct
	                        Elt_CheckedBy [Checker 1],
	                        Umt_UserLName + ', ' + Umt_UserFName [Checker 1 Name]
                        from T_EmployeeLeaveAvailmentHist
                        left outer join M_User
                        on Elt_CheckedBy = Mur_UserCode
                        where Elt_CheckedBy is not null
                        and  LTRIM( Elt_CheckedBy)  <> '' 
                        and UPPER(Elt_CheckedBy) <> 'SA' 


                        order by [Checker 1 Name] asc                  
                    ";
                }
                else if (routeNumber == 2)
                {
                    query = @"
                       select distinct
	                        Elt_Checked2By [Checker 2],
	                        Umt_UserLName + ', ' + Umt_UserFName [Checker 2 Name]
                        from T_EmployeeLeaveAvailment
                        left outer join M_User
                        on Elt_Checked2By = Mur_UserCode
                        where Elt_Checked2By is not null
                        and  LTRIM( Elt_Checked2By)  <> '' 
                        and UPPER(Elt_Checked2By) <> 'SA' 

                        union

                        select distinct
	                        Elt_Checked2By [Checker 2],
	                        Umt_UserLName + ', ' + Umt_UserFName [Checker 2 Name]
                        from T_EmployeeLeaveAvailmentHist
                        left outer join M_User
                        on Elt_Checked2By = Mur_UserCode
                        where Elt_Checked2By is not null
                        and  LTRIM( Elt_Checked2By)  <> '' 
                        and UPPER(Elt_Checked2By) <> 'SA' 


                        order by [Checker 2 Name] asc                  
                    ";
                }
                else if (routeNumber == 3)
                {
                    query = @"
                       select distinct
	                        Elt_ApprovedBy [Approver],
	                        Umt_UserLName + ', ' + Umt_UserFName [Approver Name]
                        from T_EmployeeLeaveAvailment
                        left outer join M_User
                        on Elt_ApprovedBy = Mur_UserCode
                        where Elt_ApprovedBy is not null
                        and  LTRIM( Elt_ApprovedBy)  <> '' 
                        and UPPER(Elt_ApprovedBy) <> 'SA' 

                        union

                        select distinct
	                        Elt_ApprovedBy [Approver],
	                        Umt_UserLName + ', ' + Umt_UserFName [Approver Name]
                        from T_EmployeeLeaveAvailmentHist
                        left outer join M_User
                        on Elt_ApprovedBy = Mur_UserCode
                        where Elt_ApprovedBy is not null
                        and  LTRIM( Elt_ApprovedBy)  <> '' 
                        and UPPER(Elt_ApprovedBy) <> 'SA' 


                        order by [Approver Name] asc                
                    ";
                }
                #endregion
            }

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetWorkflowStatuses()
        {
            string query = @"
                    select
	                    Adt_AccountCode [Status]
	                    ,Adt_AccountDesc [Description]
                     from T_AccountDetail
                    where Adt_AccountType = 'WFSTATUS'                
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

        public DataTable GetTransactionFlags()
        {
            string query = @"
                   select 
	                    Adt_AccountCode [Flag],
	                    Adt_AccountDesc [Description]
                    from T_AccountDetail
                    where Adt_AccountType = 'CYCLEINDIC'
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

        public DataTable GetLeavetype()
        {
            string query = @"
                   select 
	                    Ltm_LeaveType [Leave Type]
	                    ,Ltm_LeaveDesc [Description]
                    from T_LeaveTypeMaster             
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

        public DataTable GetLeaveCategory()
        {
            string query = @"
                   select
	                    Adt_AccountCode [Leave Category],
	                    Adt_AccountDesc [Description]
                    from T_AccountDetail
                    where Adt_AccountType = 'LVECATEGRY'            
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

        public DataTable GetLeaveCode()
        {
            string query = @"
                   select 
	                    Adt_AccountCode [Leave Code],
	                    Adt_AccountDesc [Description]
                    from T_AccountDetail
                    where Adt_AccountType like 'ABSEVALFAC'
                    or Adt_AccountType like 'LVEEVALFAC'     
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

        public DataTable GetStartDeductionDate()
        {
            string query = @"
                   select 
                    distinct convert(varchar(20), Edl_StartDeductionDate, 101) [Start Deduction Date]
                    from T_EmployeeDeductionLedger
                    union

                    select 
                    distinct convert(varchar(20), Edl_StartDeductionDate, 101) [Start Deduction Date]
                    from T_EmployeeDeductionLedgerHist
                    union

                    select 
                    distinct convert(varchar(20), Edd_StartDeductionDate, 101) [Start Deduction Date]
                    from T_EmployeeDeductionDetail
                    union

                    select 
                    distinct convert(varchar(20), Edd_StartDeductionDate, 101) [Start Deduction Date]
                    from T_EmployeeDeductionDetailHist

                    union
                    select 
                    distinct convert(varchar(20), Edd_StartDeductionDate, 101) [Start Deduction Date]
                    from T_EmployeeDeductionDetailHistFP    
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

        public DataTable GetVoucherNumber()
        {
            string query = @"
                    select 
                    distinct Edl_VoucherNumber [Voucher Number]
                    from T_EmployeeDeductionLedger
                    where LTRIM(Edl_VoucherNumber) <> ''
                    union

                    select 
                    distinct Edl_VoucherNumber [Voucher Number]
                    from T_EmployeeDeductionLedgerHist
                    where LTRIM(Edl_VoucherNumber) <> ''   
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

        public DataTable GetCheckDate()
        {
            string query = @"
                   select 
                       CONVERT(varchar(20), Edl_CheckDate, 101) [Check Date]
                    from T_EmployeeDeductionLedgerHist
                    where Edl_CheckDate is not null

                    union
                    select 
                       CONVERT(varchar(20), Edl_CheckDate, 101) [Check Date]
                    from T_EmployeeDeductionLedger
                    where Edl_CheckDate is not null 
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

        public DataTable GetLeaveYear()
        {
            string query = @"
                  select
	                    distinct
	                    Elm_LeaveYear [Leave Type]
                    from T_EmployeeLeave

                    union
                    select
	                    distinct
	                    Elm_LeaveYear [Leave Type]
                    from T_EmployeeLeaveHist
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

        public DataTable GetPostPayrollRemarks()
        {
            string query = @"
                 select 
	                distinct
	                Per_Remarks [Remarks]
                from T_PayrollError

                union

                select 
	                distinct
	                Per_Remarks [Remarks]
                from T_PayrollErrorHist
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

        public string GetPeriodPosition(string payperiod)
        {
            if (payperiod == string.Empty)
            {
                return "ALL";
            }
            string ret = string.Empty;
            string query = @"
                select 
                    * 
                from T_PayPeriodMaster
                Where Ppm_CycleIndicator = 'C'
                And Ppm_PayPeriod = '{0}'
                ";
            using (DALHelper dal = new DALHelper())
            {
                query = string.Format(query, payperiod);
                try
                {
                    dal.OpenDB();
                    DataSet ds = dal.ExecuteDataSet(query, CommandType.Text);
                    if (ds.Tables[0].Rows.Count > 0)
                        return "CURRENT PAYPERIOD";
                }
                catch
                {
                    return "";
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            string year = payperiod.Substring(0, 4);
            if (year == DateTime.Now.Year.ToString())
                ret = "CURRENT YEAR";
            else
                ret = "PREVIOUS YEAR";

            return ret;

        }

        public string[] getPayperiodBreakdown(string strDelimited)
        {
            string[] strArrFilterItems = strDelimited.Split(new char[] { ',' });

            return strArrFilterItems;
        }

        public string GetPayrollScheduleData( string reportType )
        {
            string queryColumns = string.Empty;

            switch (reportType)
            { 
                case "HOURS":
                    #region
                    queryColumns = @"
                    '@Prof' [Profiles],
                    Epc_Employeeid as [ID Number]
                    , Emt_Lastname + ', ' + Emt_Firstname as [Employee Name]
                    , Epc_Costcenter as [Costcenter]
                    , dbo.getCostCenterFullNameV2 (Epc_Costcenter) as [Description]
                    , Epc_CurrentPayPeriod [Payperiod]
                    , Epc_AbsentHr as [Hours Late/UT/ Absent]
                    , Epc_RegularHr as [Hrs. Regular]
                    , Epc_RegularOTHr as [OT Hrs. Regular]
                    , Epc_RestdayHr as [OT Hrs. Rest Day 1st 8]
                    , Epc_RestdayOTHr as [OT Hrs. Rest Day Excess 8]
                    , Epc_SpecialHolidayHr as [OT Hrs. Spec Hol 1st 8]
                    , Epc_SpecialHolidayOTHr as [OT Hrs. Spec Hol Excess 8]
                    , Epc_LegalHolidayHr as [OT Hrs. Legal Hol 1st 8]
                    , Epc_LegalHolidayOTHr as [OT Hrs. Legal Hol Excess 8]
                    , Epc_RestdaySpecialHolidayHr as [OT Hrs. SH Fall on RD 1st 8]
                    , Epc_RestdaySpecialHolidayOTHr as [OT Hrs. SH Fall on RD Excess 8]
                    , Epc_RestdayLegalHolidayHr as [OT Hrs. LH Fall on RD 1st 8]
                    , Epc_RestdayLegalHolidayOTHr as [OT Hrs. LH Fall on RD Excess 8]
                    , Epc_RegularNDHr + Epc_RegularOTNDHr as [NDOT Hrs Regular]
                    , Epc_RestdayNDHr as [NDOT Hrs. Rest Day 1st 8]
                    , Epc_RestdayOTNDHr as [NDOT Hrs. Rest Day Excess 8]
                    , Epc_SpecialHolidayNDHr as [NDOT Hrs. Spec Hol 1st 8]
                    , Epc_SpecialHolidayOTNDHr as [NDOT Hrs. Spec Hol Excess 8]
                    , Epc_LegalHolidayNDHr as [NDOT Hrs. Legal Hol 1st 8]
                    , Epc_LegalHolidayOTNDHr as [NDOT Hrs. Legal Hol Excess 8]
                    , Epc_RestdaySpecialHolidayNDHr as [NDOT Hrs. SH Fall on RD 1st 8]
                    , Epc_RestdaySpecialHolidayOTNDHr as [NDOT Hrs. SH Fall on RD Excess 8]
                    , Epc_RestdayLegalHolidayNDHr as [NDOT Hrs. LH Fall on RD 1st 8]
                    , Epc_RestdayLegalHolidayOTNDHr as [NDOT Hrs LH Fall on RD Excess 8]
                     , ( Epc_RegularOTHr+ Epc_RegularNDHr+ Epc_RegularOTNDHr+ Epc_RestdayHr
                    + Epc_RestdayOTHr+ Epc_RestdayNDHr+ Epc_RestdayOTNDHr+ Epc_LegalHolidayHr
                    + Epc_LegalHolidayOTHr+ Epc_LegalHolidayNDHr+ Epc_LegalHolidayOTNDHr
                    + Epc_SpecialHolidayHr+ Epc_SpecialHolidayOTHr+ Epc_SpecialHolidayNDHr
                    + Epc_SpecialHolidayOTNDHr+ Epc_RestdayLegalHolidayHr+ Epc_RestdayLegalHolidayOTHr
                    + Epc_RestdayLegalHolidayNDHr+ Epc_RestdayLegalHolidayOTNDHr+ Epc_RestdaySpecialHolidayHr
                    + Epc_RestdaySpecialHolidayOTHr+ Epc_RestdaySpecialHolidayNDHr+ Epc_RestdaySpecialHolidayOTNDHr) as [Total OT Hrs.]
                        ";
                    #endregion
                    break;
                case "AMOUNTS":
                    #region
                    queryColumns = @"
                    '@Prof' [Profiles],
                    Epc_Employeeid as [ID Number]
	                , Emt_Lastname + ', ' + Emt_Firstname as [Employee Name]
                    , Epc_Costcenter as [Costcenter]
                    , dbo.getCostCenterFullNameV2 (Epc_Costcenter) as [Description]
                    , Epc_CurrentPayPeriod [Payperiod]
	                , Epc_TaxCode as [Tax Code]
	                , Epc_RegularAmt as [Half Mo. Pay]
	                , Epc_LaborHrsAdjustmentAmt + Epc_TaxAdjustmentAmt as [Adjustment]
                    , Epc_AbsentAmt as [Amount Late/UT/ Absent]
                    , Epc_RegularAmt as [Amt. Regular]
                    , Epc_RegularOTAmt as [OT Amt. Regular]
                    , Epc_RestdayAmt as [OT Amt. Rest Day 1st 8]
                    , Epc_RestdayOTAmt as [OT Amt. Rest Day Excess 8]
                    , Epc_SpecialHolidayAmt as [OT Amt. Spec Hol 1st 8]
                    , Epc_SpecialHolidayOTAmt as [OT Amt. Spec Hol Excess 8]
                    , Epc_LegalHolidayAmt  as [OT Amt. Legal Hol 1st 8]
                    , Epc_LegalHolidayOTAmt as [OT Amt. Legal Hol Excess 8]
                    , Epc_RestdaySpecialHolidayAmt as [OT Amt. SH Fall on RD 1st 8]
                    , Epc_RestdaySpecialHolidayOTAmt as [OT Amt. SH Fall on RD Excess 8]
                    , Epc_RestdayLegalHolidayAmt as [OT Amt. LH Fall on RD 1st 8]
                    , Epc_RestdayLegalHolidayOTAmt as [OT Amt. LH Fall on RD Excess 8]
                    , Epc_RegularNDAmt + Epc_RegularOTNDAmt as [NDOT Amt. Regular]
                    , Epc_RestdayNDAmt as [NDOT Amt. Rest Day 1st 8]
                    , Epc_RestdayOTNDAmt as [NDOT Amt. Rest Day Excess 8]
                    , Epc_SpecialHolidayNDAmt as [NDOT Amt. Spec Hol 1st 8]
                    , Epc_SpecialHolidayOTNDAmt as [NDOT Amt. Spec Hol Excess 8]
                    , Epc_LegalHolidayNDAmt as [NDOT Amt. Legal Hol 1st 8]
                    , Epc_LegalHolidayOTNDAmt as [NDOT Amt. Legal Hol Excess 8]
                    , Epc_RestdaySpecialHolidayNDAmt as [NDOT Amt. SH Fall on RD 1st 8]
                    , Epc_RestdaySpecialHolidayOTNDAmt as [NDOT Amt. SH Fall on RD Excess 8]
                    , Epc_RestdayLegalHolidayOTNDAmt as [NDOT Amt LH Fall on RD Excess 8]
                    , Epc_RestdayLegalHolidayNDAmt as [NDOT Amt. LH Fall on RD 1st 8]
                    , ( Epc_RegularOTAmt+ Epc_RegularNDAmt+ Epc_RegularOTNDAmt+ Epc_RestdayAmt
                    + Epc_RestdayOTAmt+ Epc_RestdayNDAmt+ Epc_RestdayOTNDAmt+ Epc_LegalHolidayAmt
                    + Epc_LegalHolidayOTAmt+ Epc_LegalHolidayNDAmt+ Epc_LegalHolidayOTNDAmt
                    + Epc_SpecialHolidayAmt+ Epc_SpecialHolidayOTAmt+ Epc_SpecialHolidayNDAmt
                    + Epc_SpecialHolidayOTNDAmt+ Epc_RestdayLegalHolidayAmt+ Epc_RestdayLegalHolidayOTAmt
                    + Epc_RestdayLegalHolidayNDAmt+ Epc_RestdayLegalHolidayOTNDAmt+ Epc_RestdaySpecialHolidayAmt
                    + Epc_RestdaySpecialHolidayOTAmt+ Epc_RestdaySpecialHolidayNDAmt+ Epc_RestdaySpecialHolidayOTNDAmt ) as [Total OT Pay]
                    ";
                    #endregion
                    break;
                case "DEDUCTIONS":
                    #region
                    queryColumns = @"
                    '@Prof' [Profiles],
                    Epc_Employeeid as [ID Number]
                    , Emt_Lastname + ', ' + Emt_Firstname as [Employee Name]
                    , Epc_Costcenter as [Costcenter]
                    , dbo.getCostCenterFullNameV2 (Epc_Costcenter) as [Description]
                    , Epc_CurrentPayPeriod [Payperiod]
                    , Epc_TaxCode as [Tax Code]
                    , 0 as [Taxable 13th Month Pay]
                    , 0 as [Performance Bonus]
                    , Epc_GrossPayAmt as [Gross Pay before SSS/MED /PAG]
                    , Epc_SSSEmployeeShare as [SSS]
                    , Epc_PhilhealthEmployeeShare as [PHEALTH]
                    , Epc_HDMFEmployeeShare as [PAGIBIG]
                    , Epc_GrossPayAmt - Epc_SSSEmployeeShare - Epc_PhilhealthEmployeeShare - Epc_HDMFEmployeeShare  as [Gross Pay after SSS/PH/PAG]
                    , ISNULL(EDCI, 0) as [EDCI]
                    , ISNULL(PAGIBIGLOAN, 0) as [PAGIBIG LOAN]
                    , ISNULL(SSSLOAN, 0) as [SSS LOAN]
                    , ISNULL(CALAMITYLN, 0)  as [CALAMITY LOAN]
                    , ISNULL(COMPANYLN, 0)  as [COMPANY LOAN]
                    , ISNULL(HOMECREDIT, 0) as [HOME CREDIT]
                    , ISNULL(FUNDSWHELD, 0)  as [FUNDS W/HELD]
                    , ISNULL(UNIFORM, 0) as [UNIFORM]
                    , ISNULL(CARLOAN, 0)  as [CAR LOAN]
                    , ISNULL(HOUSNGLOAN, 0)  as [HOUSING LOAN]
                    , ISNULL(EDUCLOAN, 0)   as [EDUC. LOAN]
                    , ISNULL(LIFESTYLE, 0)  as [LIFE STYLE LOAN]
                    , Epc_wtaxamt as [W/ TAX]
                    , ISNULL(CASHADV, 0)   as [CASH ADVANCE]
                    , ISNULL(OTHER, 0)    as [Others]
                    , Epc_TotalDeductionAmt as [TOTAL Deduction]
                    , ISNULL(TRANSPO, 0) as [TRANSPO]
                    , ISNULL(MEAL, 0) as [MEAL]
                    , ISNULL(TAXI, 0) as [TAXI]
                    , Epc_Netpayamt as [NET PAY]

                    ";
                    #endregion
                    break;
                default:
                    #region
                    queryColumns = @"
                    '@Prof' [Profiles],
                    Epc_Employeeid as [ID Number]
                    , Emt_Lastname + ', ' + Emt_Firstname as [Employee Name]
                    , Epc_Costcenter as [Costcenter]
                    , dbo.getCostCenterFullNameV2 (Epc_Costcenter) as [Description]
                    , Epc_CurrentPayPeriod [Payperiod]
                    , Epc_TaxCode as [Tax Code]
                    , Epc_RegularAmt as [Half Mo. Pay]
                    , Epc_LaborHrsAdjustmentAmt + Epc_TaxAdjustmentAmt as [Adjustment]
                    , Epc_AbsentHr as [Hours Late/UT/ Absent]
                    , Epc_RegularHr as [Hrs. Regular]
                    , Epc_RegularOTHr as [OT Hrs. Regular]
                    , Epc_RestdayHr as [OT Hrs. Rest Day 1st 8]
                    , Epc_RestdayOTHr as [OT Hrs. Rest Day Excess 8]
                    , Epc_SpecialHolidayHr as [OT Hrs. Spec Hol 1st 8]
                    , Epc_SpecialHolidayOTHr as [OT Hrs. Spec Hol Excess 8]
                    , Epc_LegalHolidayHr as [OT Hrs. Legal Hol 1st 8]
                    , Epc_LegalHolidayOTHr as [OT Hrs. Legal Hol Excess 8]
                    , Epc_RestdaySpecialHolidayHr as [OT Hrs. SH Fall on RD 1st 8]
                    , Epc_RestdaySpecialHolidayOTHr as [OT Hrs. SH Fall on RD Excess 8]
                    , Epc_RestdayLegalHolidayHr as [OT Hrs. LH Fall on RD 1st 8]
                    , Epc_RestdayLegalHolidayOTHr as [OT Hrs. LH Fall on RD Excess 8]
                    , Epc_RegularNDHr + Epc_RegularOTNDHr as [NDOT Hrs Regular]
                    , Epc_RestdayNDHr as [NDOT Hrs. Rest Day 1st 8]
                    , Epc_RestdayOTNDHr as [NDOT Hrs. Rest Day Excess 8]
                    , Epc_SpecialHolidayNDHr as [NDOT Hrs. Spec Hol 1st 8]
                    , Epc_SpecialHolidayOTNDHr as [NDOT Hrs. Spec Hol Excess 8]
                    , Epc_LegalHolidayNDHr as [NDOT Hrs. Legal Hol 1st 8]
                    , Epc_LegalHolidayOTNDHr as [NDOT Hrs. Legal Hol Excess 8]
                    , Epc_RestdaySpecialHolidayNDHr as [NDOT Hrs. SH Fall on RD 1st 8]
                    , Epc_RestdaySpecialHolidayOTNDHr as [NDOT Hrs. SH Fall on RD Excess 8]
                    , Epc_RestdayLegalHolidayNDHr as [NDOT Hrs. LH Fall on RD 1st 8]
                    , Epc_RestdayLegalHolidayOTNDHr as [NDOT Hrs LH Fall on RD Excess 8]
                     , ( Epc_RegularOTHr+ Epc_RegularNDHr+ Epc_RegularOTNDHr+ Epc_RestdayHr
                    + Epc_RestdayOTHr+ Epc_RestdayNDHr+ Epc_RestdayOTNDHr+ Epc_LegalHolidayHr
                    + Epc_LegalHolidayOTHr+ Epc_LegalHolidayNDHr+ Epc_LegalHolidayOTNDHr
                    + Epc_SpecialHolidayHr+ Epc_SpecialHolidayOTHr+ Epc_SpecialHolidayNDHr
                    + Epc_SpecialHolidayOTNDHr+ Epc_RestdayLegalHolidayHr+ Epc_RestdayLegalHolidayOTHr
                    + Epc_RestdayLegalHolidayNDHr+ Epc_RestdayLegalHolidayOTNDHr+ Epc_RestdaySpecialHolidayHr
                    + Epc_RestdaySpecialHolidayOTHr+ Epc_RestdaySpecialHolidayNDHr+ Epc_RestdaySpecialHolidayOTNDHr) as [Total OT Hrs.]
                    , Epc_AbsentAmt as [Amount Late/UT/ Absent]
                    , Epc_RegularAmt as [Amt. Regular]
                    , Epc_RegularOTAmt as [OT Amt. Regular]
                    , Epc_RestdayAmt as [OT Amt. Rest Day 1st 8]
                    , Epc_RestdayOTAmt as [OT Amt. Rest Day Excess 8]
                    , Epc_SpecialHolidayAmt as [OT Amt. Spec Hol 1st 8]
                    , Epc_SpecialHolidayOTAmt as [OT Amt. Spec Hol Excess 8]
                    , Epc_LegalHolidayAmt  as [OT Amt. Legal Hol 1st 8]
                    , Epc_LegalHolidayOTAmt as [OT Amt. Legal Hol Excess 8]
                    , Epc_RestdaySpecialHolidayAmt as [OT Amt. SH Fall on RD 1st 8]
                    , Epc_RestdaySpecialHolidayOTAmt as [OT Amt. SH Fall on RD Excess 8]
                    , Epc_RestdayLegalHolidayAmt as [OT Amt. LH Fall on RD 1st 8]
                    , Epc_RestdayLegalHolidayOTAmt as [OT Amt. LH Fall on RD Excess 8]
                    , Epc_RegularNDAmt + Epc_RegularOTNDAmt as [NDOT Amt. Regular]
                    , Epc_RestdayNDAmt as [NDOT Amt. Rest Day 1st 8]
                    , Epc_RestdayOTNDAmt as [NDOT Amt. Rest Day Excess 8]
                    , Epc_SpecialHolidayNDAmt as [NDOT Amt. Spec Hol 1st 8]
                    , Epc_SpecialHolidayOTNDAmt as [NDOT Amt. Spec Hol Excess 8]
                    , Epc_LegalHolidayNDAmt as [NDOT Amt. Legal Hol 1st 8]
                    , Epc_LegalHolidayOTNDAmt as [NDOT Amt. Legal Hol Excess 8]
                    , Epc_RestdaySpecialHolidayNDAmt as [NDOT Amt. SH Fall on RD 1st 8]
                    , Epc_RestdaySpecialHolidayOTNDAmt as [NDOT Amt. SH Fall on RD Excess 8]
                    , Epc_RestdayLegalHolidayOTNDAmt as [NDOT Amt LH Fall on RD Excess 8]
                    , Epc_RestdayLegalHolidayNDAmt as [NDOT Amt. LH Fall on RD 1st 8]
                    , ( Epc_RegularOTAmt+ Epc_RegularNDAmt+ Epc_RegularOTNDAmt+ Epc_RestdayAmt
                    + Epc_RestdayOTAmt+ Epc_RestdayNDAmt+ Epc_RestdayOTNDAmt+ Epc_LegalHolidayAmt
                    + Epc_LegalHolidayOTAmt+ Epc_LegalHolidayNDAmt+ Epc_LegalHolidayOTNDAmt
                    + Epc_SpecialHolidayAmt+ Epc_SpecialHolidayOTAmt+ Epc_SpecialHolidayNDAmt
                    + Epc_SpecialHolidayOTNDAmt+ Epc_RestdayLegalHolidayAmt+ Epc_RestdayLegalHolidayOTAmt
                    + Epc_RestdayLegalHolidayNDAmt+ Epc_RestdayLegalHolidayOTNDAmt+ Epc_RestdaySpecialHolidayAmt
                    + Epc_RestdaySpecialHolidayOTAmt+ Epc_RestdaySpecialHolidayNDAmt+ Epc_RestdaySpecialHolidayOTNDAmt ) as [Total OT Pay]
                    , 0 as [Taxable 13th Month Pay]
                    , 0 as [Performance Bonus]
                    , Epc_GrossPayAmt as [Gross Pay before SSS/MED /PAG]
                    , Epc_SSSEmployeeShare as [SSS]
                    , Epc_PhilhealthEmployeeShare as [PHEALTH]
                    , Epc_HDMFEmployeeShare as [PAGIBIG]
                    , Epc_GrossPayAmt - Epc_SSSEmployeeShare - Epc_PhilhealthEmployeeShare - Epc_HDMFEmployeeShare  as [Gross Pay after SSS/PH/PAG]
                    , ISNULL(EDCI, 0) as [EDCI]
                    , ISNULL(PAGIBIGLOAN, 0) as [PAGIBIG LOAN]
                    , ISNULL(SSSLOAN, 0) as [SSS LOAN]
                    , ISNULL(CALAMITYLN, 0)  as [CALAMITY LOAN]
                    , ISNULL(COMPANYLN, 0)  as [COMPANY LOAN]
                    , ISNULL(HOMECREDIT, 0) as [HOME CREDIT]
                    , ISNULL(FUNDSWHELD, 0)  as [FUNDS W/HELD]
                    , ISNULL(UNIFORM, 0) as [UNIFORM]
                    , ISNULL(CARLOAN, 0)  as [CAR LOAN]
                    , ISNULL(HOUSNGLOAN, 0)  as [HOUSING LOAN]
                    , ISNULL(EDUCLOAN, 0)   as [EDUC. LOAN]
                    , ISNULL(LIFESTYLE, 0)  as [LIFE STYLE LOAN]
                    , Epc_wtaxamt as [W/ TAX]
                    , ISNULL(CASHADV, 0)   as [CASH ADVANCE]
                    , ISNULL(OTHER, 0)    as [Others]
                    , Epc_TotalDeductionAmt as [TOTAL Deduction]
                    , ISNULL(TRANSPO, 0) as [TRANSPO]
                    , ISNULL(MEAL, 0) as [MEAL]
                    , ISNULL(TAXI, 0) as [TAXI]
                    , Epc_Netpayamt as [NET PAY]
                    ";
                    #endregion
                    break;
            }

            return queryColumns;

        }


        public string GetPayrollScheduleDataSummary(string reportType)
        {
            string queryColumns = string.Empty;

            switch (reportType)
            {
                case "HOURS":
                    #region
                    queryColumns = @"
                    ,Sum( Epc_AbsentHr ) [Hours Late/UT/ Absent]
                    ,Sum( Epc_RegularHr ) [Hrs. Regular]
                    ,Sum( Epc_RegularOTHr ) [OT Hrs. Regular]
                    ,Sum( Epc_RestdayHr ) [OT Hrs. Rest Day 1st 8]
                    ,Sum( Epc_RestdayOTHr ) [OT Hrs. Rest Day Excess 8]
                    ,Sum( Epc_SpecialHolidayHr ) [OT Hrs. Spec Hol 1st 8]
                    ,Sum( Epc_SpecialHolidayOTHr ) [OT Hrs. Spec Hol Excess 8]
                    ,Sum( Epc_LegalHolidayHr ) [OT Hrs. Legal Hol 1st 8]
                    ,Sum( Epc_LegalHolidayOTHr ) [OT Hrs. Legal Hol Excess 8]
                    ,Sum( Epc_RestdaySpecialHolidayHr ) [OT Hrs. SH Fall on RD 1st 8]
                    ,Sum( Epc_RestdaySpecialHolidayOTHr ) [OT Hrs. SH Fall on RD Excess 8]
                    ,Sum( Epc_RestdayLegalHolidayHr ) [OT Hrs. LH Fall on RD 1st 8]
                    ,Sum( Epc_RestdayLegalHolidayOTHr ) [OT Hrs. LH Fall on RD Excess 8]
                    ,Sum( Epc_RegularNDHr + Epc_RegularOTNDHr ) [NDOT Hrs Regular]
                    ,Sum( Epc_RestdayNDHr ) [NDOT Hrs. Rest Day 1st 8]
                    ,Sum( Epc_RestdayOTNDHr ) [NDOT Hrs. Rest Day Excess 8]
                    ,Sum( Epc_SpecialHolidayNDHr ) [NDOT Hrs. Spec Hol 1st 8]
                    ,Sum( Epc_SpecialHolidayOTNDHr ) [NDOT Hrs. Spec Hol Excess 8]
                    ,Sum( Epc_LegalHolidayNDHr ) [NDOT Hrs. Legal Hol 1st 8]
                    ,Sum( Epc_LegalHolidayOTNDHr ) [NDOT Hrs. Legal Hol Excess 8]
                    ,Sum( Epc_RestdaySpecialHolidayNDHr ) [NDOT Hrs. SH Fall on RD 1st 8]
                    ,Sum( Epc_RestdaySpecialHolidayOTNDHr ) [NDOT Hrs. SH Fall on RD Excess 8]
                    ,Sum( Epc_RestdayLegalHolidayNDHr ) [NDOT Hrs. LH Fall on RD 1st 8]
                    ,Sum( Epc_RestdayLegalHolidayOTNDHr ) [NDOT Hrs LH Fall on RD Excess 8]
                     ,Sum( ( Epc_RegularOTHr+ Epc_RegularNDHr+ Epc_RegularOTNDHr+ Epc_RestdayHr
                    + Epc_RestdayOTHr+ Epc_RestdayNDHr+ Epc_RestdayOTNDHr+ Epc_LegalHolidayHr
                    + Epc_LegalHolidayOTHr+ Epc_LegalHolidayNDHr+ Epc_LegalHolidayOTNDHr
                    + Epc_SpecialHolidayHr+ Epc_SpecialHolidayOTHr+ Epc_SpecialHolidayNDHr
                    + Epc_SpecialHolidayOTNDHr+ Epc_RestdayLegalHolidayHr+ Epc_RestdayLegalHolidayOTHr
                    + Epc_RestdayLegalHolidayNDHr+ Epc_RestdayLegalHolidayOTNDHr+ Epc_RestdaySpecialHolidayHr
                    + Epc_RestdaySpecialHolidayOTHr+ Epc_RestdaySpecialHolidayNDHr+ Epc_RestdaySpecialHolidayOTNDHr) ) [Total OT Hrs.]
                    ";
                    #endregion
                    break;
                case "AMOUNTS":
                    #region
                    queryColumns = @"
                    ,Sum( Epc_RegularAmt ) as [Half Mo. Pay]
	                ,Sum( Epc_LaborHrsAdjustmentAmt + Epc_TaxAdjustmentAmt ) as [Adjustment]
                    ,Sum( Epc_AbsentAmt ) as [Amount Late/UT/ Absent]
                    ,Sum( Epc_RegularAmt ) [Amt. Regular]
                    ,Sum( Epc_RegularOTAmt ) as [OT Amt. Regular]
                    ,Sum( Epc_RestdayAmt ) as [OT Amt. Rest Day 1st 8]
                    ,Sum( Epc_RestdayOTAmt ) as [OT Amt. Rest Day Excess 8]
                    ,Sum( Epc_SpecialHolidayAmt ) as [OT Amt. Spec Hol 1st 8]
                    ,Sum( Epc_SpecialHolidayOTAmt ) as [OT Amt. Spec Hol Excess 8]
                    ,Sum( Epc_LegalHolidayAmt  ) as [OT Amt. Legal Hol 1st 8]
                    ,Sum( Epc_LegalHolidayOTAmt ) as [OT Amt. Legal Hol Excess 8]
                    ,Sum( Epc_RestdaySpecialHolidayAmt ) as [OT Amt. SH Fall on RD 1st 8]
                    ,Sum( Epc_RestdaySpecialHolidayOTAmt ) as [OT Amt. SH Fall on RD Excess 8]
                    ,Sum( Epc_RestdayLegalHolidayAmt ) as [OT Amt. LH Fall on RD 1st 8]
                    ,Sum( Epc_RestdayLegalHolidayOTAmt ) as [OT Amt. LH Fall on RD Excess 8]
                    ,Sum( Epc_RegularNDAmt + Epc_RegularOTNDAmt ) as [NDOT Amt. Regular]
                    ,Sum( Epc_RestdayNDAmt ) as [NDOT Amt. Rest Day 1st 8]
                    ,Sum( Epc_RestdayOTNDAmt ) as [NDOT Amt. Rest Day Excess 8]
                    ,Sum( Epc_SpecialHolidayNDAmt ) as [NDOT Amt. Spec Hol 1st 8]
                    ,Sum( Epc_SpecialHolidayOTNDAmt ) as [NDOT Amt. Spec Hol Excess 8]
                    ,Sum( Epc_LegalHolidayNDAmt ) as [NDOT Amt. Legal Hol 1st 8]
                    ,Sum( Epc_LegalHolidayOTNDAmt ) as [NDOT Amt. Legal Hol Excess 8]
                    ,Sum( Epc_RestdaySpecialHolidayNDAmt ) as [NDOT Amt. SH Fall on RD 1st 8]
                    ,Sum( Epc_RestdaySpecialHolidayOTNDAmt ) as [NDOT Amt. SH Fall on RD Excess 8]
                    ,Sum( Epc_RestdayLegalHolidayOTNDAmt ) as [NDOT Amt LH Fall on RD Excess 8]
                    ,Sum( Epc_RestdayLegalHolidayNDAmt ) as [NDOT Amt. LH Fall on RD 1st 8]
                    ,Sum(  Epc_RegularOTAmt+ Epc_RegularNDAmt+ Epc_RegularOTNDAmt+ Epc_RestdayAmt
                    + Epc_RestdayOTAmt+ Epc_RestdayNDAmt+ Epc_RestdayOTNDAmt+ Epc_LegalHolidayAmt
                    + Epc_LegalHolidayOTAmt+ Epc_LegalHolidayNDAmt+ Epc_LegalHolidayOTNDAmt
                    + Epc_SpecialHolidayAmt+ Epc_SpecialHolidayOTAmt+ Epc_SpecialHolidayNDAmt
                    + Epc_SpecialHolidayOTNDAmt+ Epc_RestdayLegalHolidayAmt+ Epc_RestdayLegalHolidayOTAmt
                    + Epc_RestdayLegalHolidayNDAmt+ Epc_RestdayLegalHolidayOTNDAmt+ Epc_RestdaySpecialHolidayAmt
                    + Epc_RestdaySpecialHolidayOTAmt+ Epc_RestdaySpecialHolidayNDAmt+ Epc_RestdaySpecialHolidayOTNDAmt ) as [Total OT Pay]
                     ";
                    #endregion
                    break;
                case "DEDUCTIONS":
                    #region
                    queryColumns = @"
                    , SUM( 0 ) as [Taxable 13th Month Pay]
                    , SUM( 0 ) as [Performance Bonus]
                    , SUM(  Epc_GrossPayAmt ) as [Gross Pay before SSS/MED /PAG]
                    , SUM( Epc_SSSEmployeeShare ) as [SSS]
                    , SUM( Epc_PhilhealthEmployeeShare ) as [PHEALTH]
                    , SUM( Epc_HDMFEmployeeShare ) as [PAGIBIG]
                    , SUM( Epc_GrossPayAmt - Epc_SSSEmployeeShare - Epc_PhilhealthEmployeeShare - Epc_HDMFEmployeeShare )  as [Gross Pay after SSS/PH/PAG]
                    , SUM( ISNULL(EDCI, 0) ) as [EDCI]
                    , SUM( ISNULL(PAGIBIGLOAN, 0) ) as [PAGIBIG LOAN]
                    , SUM( ISNULL(SSSLOAN, 0) ) as [SSS LOAN]
                    , SUM( ISNULL(CALAMITYLN, 0)  ) as [CALAMITY LOAN]
                    , SUM( ISNULL(COMPANYLN, 0)  ) as [COMPANY LOAN]
                    , SUM( ISNULL(HOMECREDIT, 0) ) as [HOME CREDIT]
                    , SUM( ISNULL(FUNDSWHELD, 0) )  as [FUNDS W/HELD]
                    , SUM( ISNULL(UNIFORM, 0) ) as [UNIFORM]
                    , SUM( ISNULL(CARLOAN, 0)  ) as [CAR LOAN]
                    , SUM( ISNULL(HOUSNGLOAN, 0)  ) as [HOUSING LOAN]
                    , SUM( ISNULL(EDUCLOAN, 0)   ) as [EDUC. LOAN]
                    , SUM( ISNULL(LIFESTYLE, 0)  ) as [LIFE STYLE LOAN]
                    , SUM( Epc_wtaxamt ) as [W/ TAX]
                    , SUM( ISNULL(CASHADV, 0) )   as [CASH ADVANCE]
                    , SUM( ISNULL(OTHER, 0)  )   as [Others]
                    , SUM( Epc_TotalDeductionAmt ) as [TOTAL Deduction]
                    , SUM( ISNULL(TRANSPO, 0) ) as [TRANSPO]
                    , SUM( ISNULL(MEAL, 0) ) as [MEAL]
                    , SUM( ISNULL(TAXI, 0))  as [TAXI]
                    , SUM( Epc_Netpayamt ) as [NET PAY]

                    ";
                    #endregion
                    break;
                default:
                    #region
                    queryColumns = @"
                    ,Sum( Epc_AbsentHr ) [Hours Late/UT/ Absent]
                    ,Sum( Epc_RegularHr ) [Hrs. Regular]
                    ,Sum( Epc_RegularOTHr ) [OT Hrs. Regular]
                    ,Sum( Epc_RestdayHr ) [OT Hrs. Rest Day 1st 8]
                    ,Sum( Epc_RestdayOTHr ) [OT Hrs. Rest Day Excess 8]
                    ,Sum( Epc_SpecialHolidayHr ) [OT Hrs. Spec Hol 1st 8]
                    ,Sum( Epc_SpecialHolidayOTHr ) [OT Hrs. Spec Hol Excess 8]
                    ,Sum( Epc_LegalHolidayHr ) [OT Hrs. Legal Hol 1st 8]
                    ,Sum( Epc_LegalHolidayOTHr ) [OT Hrs. Legal Hol Excess 8]
                    ,Sum( Epc_RestdaySpecialHolidayHr ) [OT Hrs. SH Fall on RD 1st 8]
                    ,Sum( Epc_RestdaySpecialHolidayOTHr ) [OT Hrs. SH Fall on RD Excess 8]
                    ,Sum( Epc_RestdayLegalHolidayHr ) [OT Hrs. LH Fall on RD 1st 8]
                    ,Sum( Epc_RestdayLegalHolidayOTHr ) [OT Hrs. LH Fall on RD Excess 8]
                    ,Sum( Epc_RegularNDHr + Epc_RegularOTNDHr ) [NDOT Hrs Regular]
                    ,Sum( Epc_RestdayNDHr ) [NDOT Hrs. Rest Day 1st 8]
                    ,Sum( Epc_RestdayOTNDHr ) [NDOT Hrs. Rest Day Excess 8]
                    ,Sum( Epc_SpecialHolidayNDHr ) [NDOT Hrs. Spec Hol 1st 8]
                    ,Sum( Epc_SpecialHolidayOTNDHr ) [NDOT Hrs. Spec Hol Excess 8]
                    ,Sum( Epc_LegalHolidayNDHr ) [NDOT Hrs. Legal Hol 1st 8]
                    ,Sum( Epc_LegalHolidayOTNDHr ) [NDOT Hrs. Legal Hol Excess 8]
                    ,Sum( Epc_RestdaySpecialHolidayNDHr ) [NDOT Hrs. SH Fall on RD 1st 8]
                    ,Sum( Epc_RestdaySpecialHolidayOTNDHr ) [NDOT Hrs. SH Fall on RD Excess 8]
                    ,Sum( Epc_RestdayLegalHolidayNDHr ) [NDOT Hrs. LH Fall on RD 1st 8]
                    ,Sum( Epc_RestdayLegalHolidayOTNDHr ) [NDOT Hrs LH Fall on RD Excess 8]
                     ,Sum( ( Epc_RegularOTHr+ Epc_RegularNDHr+ Epc_RegularOTNDHr+ Epc_RestdayHr
                    + Epc_RestdayOTHr+ Epc_RestdayNDHr+ Epc_RestdayOTNDHr+ Epc_LegalHolidayHr
                    + Epc_LegalHolidayOTHr+ Epc_LegalHolidayNDHr+ Epc_LegalHolidayOTNDHr
                    + Epc_SpecialHolidayHr+ Epc_SpecialHolidayOTHr+ Epc_SpecialHolidayNDHr
                    + Epc_SpecialHolidayOTNDHr+ Epc_RestdayLegalHolidayHr+ Epc_RestdayLegalHolidayOTHr
                    + Epc_RestdayLegalHolidayNDHr+ Epc_RestdayLegalHolidayOTNDHr+ Epc_RestdaySpecialHolidayHr
                    + Epc_RestdaySpecialHolidayOTHr+ Epc_RestdaySpecialHolidayNDHr+ Epc_RestdaySpecialHolidayOTNDHr) ) [Total OT Hrs.]
                    ,Sum( Epc_RegularAmt ) as [Half Mo. Pay]
	                ,Sum( Epc_LaborHrsAdjustmentAmt + Epc_TaxAdjustmentAmt ) as [Adjustment]
                    ,Sum( Epc_AbsentAmt ) as [Amount Late/UT/ Absent]
                    ,Sum( Epc_RegularAmt ) [Amt. Regular]
                    ,Sum( Epc_RegularOTAmt ) as [OT Amt. Regular]
                    ,Sum( Epc_RestdayAmt ) as [OT Amt. Rest Day 1st 8]
                    ,Sum( Epc_RestdayOTAmt ) as [OT Amt. Rest Day Excess 8]
                    ,Sum( Epc_SpecialHolidayAmt ) as [OT Amt. Spec Hol 1st 8]
                    ,Sum( Epc_SpecialHolidayOTAmt ) as [OT Amt. Spec Hol Excess 8]
                    ,Sum( Epc_LegalHolidayAmt  ) as [OT Amt. Legal Hol 1st 8]
                    ,Sum( Epc_LegalHolidayOTAmt ) as [OT Amt. Legal Hol Excess 8]
                    ,Sum( Epc_RestdaySpecialHolidayAmt ) as [OT Amt. SH Fall on RD 1st 8]
                    ,Sum( Epc_RestdaySpecialHolidayOTAmt ) as [OT Amt. SH Fall on RD Excess 8]
                    ,Sum( Epc_RestdayLegalHolidayAmt ) as [OT Amt. LH Fall on RD 1st 8]
                    ,Sum( Epc_RestdayLegalHolidayOTAmt ) as [OT Amt. LH Fall on RD Excess 8]
                    ,Sum( Epc_RegularNDAmt + Epc_RegularOTNDAmt ) as [NDOT Amt. Regular]
                    ,Sum( Epc_RestdayNDAmt ) as [NDOT Amt. Rest Day 1st 8]
                    ,Sum( Epc_RestdayOTNDAmt ) as [NDOT Amt. Rest Day Excess 8]
                    ,Sum( Epc_SpecialHolidayNDAmt ) as [NDOT Amt. Spec Hol 1st 8]
                    ,Sum( Epc_SpecialHolidayOTNDAmt ) as [NDOT Amt. Spec Hol Excess 8]
                    ,Sum( Epc_LegalHolidayNDAmt ) as [NDOT Amt. Legal Hol 1st 8]
                    ,Sum( Epc_LegalHolidayOTNDAmt ) as [NDOT Amt. Legal Hol Excess 8]
                    ,Sum( Epc_RestdaySpecialHolidayNDAmt ) as [NDOT Amt. SH Fall on RD 1st 8]
                    ,Sum( Epc_RestdaySpecialHolidayOTNDAmt ) as [NDOT Amt. SH Fall on RD Excess 8]
                    ,Sum( Epc_RestdayLegalHolidayOTNDAmt ) as [NDOT Amt LH Fall on RD Excess 8]
                    ,Sum( Epc_RestdayLegalHolidayNDAmt ) as [NDOT Amt. LH Fall on RD 1st 8]
                    ,Sum(  Epc_RegularOTAmt+ Epc_RegularNDAmt+ Epc_RegularOTNDAmt+ Epc_RestdayAmt
                    + Epc_RestdayOTAmt+ Epc_RestdayNDAmt+ Epc_RestdayOTNDAmt+ Epc_LegalHolidayAmt
                    + Epc_LegalHolidayOTAmt+ Epc_LegalHolidayNDAmt+ Epc_LegalHolidayOTNDAmt
                    + Epc_SpecialHolidayAmt+ Epc_SpecialHolidayOTAmt+ Epc_SpecialHolidayNDAmt
                    + Epc_SpecialHolidayOTNDAmt+ Epc_RestdayLegalHolidayAmt+ Epc_RestdayLegalHolidayOTAmt
                    + Epc_RestdayLegalHolidayNDAmt+ Epc_RestdayLegalHolidayOTNDAmt+ Epc_RestdaySpecialHolidayAmt
                    + Epc_RestdaySpecialHolidayOTAmt+ Epc_RestdaySpecialHolidayNDAmt+ Epc_RestdaySpecialHolidayOTNDAmt ) as [Total OT Pay]
                    , SUM( 0 ) as [Taxable 13th Month Pay]
                    , SUM( 0 ) as [Performance Bonus]
                    , SUM(  Epc_GrossPayAmt ) as [Gross Pay before SSS/MED /PAG]
                    , SUM( Epc_SSSEmployeeShare ) as [SSS]
                    , SUM( Epc_PhilhealthEmployeeShare ) as [PHEALTH]
                    , SUM( Epc_HDMFEmployeeShare ) as [PAGIBIG]
                    , SUM( Epc_GrossPayAmt - Epc_SSSEmployeeShare - Epc_PhilhealthEmployeeShare - Epc_HDMFEmployeeShare )  as [Gross Pay after SSS/PH/PAG]
                    , SUM( ISNULL(EDCI, 0) ) as [EDCI]
                    , SUM( ISNULL(PAGIBIGLOAN, 0) ) as [PAGIBIG LOAN]
                    , SUM( ISNULL(SSSLOAN, 0) ) as [SSS LOAN]
                    , SUM( ISNULL(CALAMITYLN, 0)  ) as [CALAMITY LOAN]
                    , SUM( ISNULL(COMPANYLN, 0)  ) as [COMPANY LOAN]
                    , SUM( ISNULL(HOMECREDIT, 0) ) as [HOME CREDIT]
                    , SUM( ISNULL(FUNDSWHELD, 0) )  as [FUNDS W/HELD]
                    , SUM( ISNULL(UNIFORM, 0) ) as [UNIFORM]
                    , SUM( ISNULL(CARLOAN, 0)  ) as [CAR LOAN]
                    , SUM( ISNULL(HOUSNGLOAN, 0)  ) as [HOUSING LOAN]
                    , SUM( ISNULL(EDUCLOAN, 0)   ) as [EDUC. LOAN]
                    , SUM( ISNULL(LIFESTYLE, 0)  ) as [LIFE STYLE LOAN]
                    , SUM( Epc_wtaxamt ) as [W/ TAX]
                    , SUM( ISNULL(CASHADV, 0) )   as [CASH ADVANCE]
                    , SUM( ISNULL(OTHER, 0)  )   as [Others]
                    , SUM( Epc_TotalDeductionAmt ) as [TOTAL Deduction]
                    , SUM( ISNULL(TRANSPO, 0) ) as [TRANSPO]
                    , SUM( ISNULL(MEAL, 0) ) as [MEAL]
                    , SUM( ISNULL(TAXI, 0))  as [TAXI]
                    , SUM( Epc_Netpayamt ) as [NET PAY]
                    ";
                    #endregion
                    break;
            }

            return queryColumns;

        }


        public string[] GetColumnsNotIncludedInSummary()
        {
            string[] cols = new string[57] { 
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
                "StartofDeduction"
            };
            return cols;
        }


    }
}
