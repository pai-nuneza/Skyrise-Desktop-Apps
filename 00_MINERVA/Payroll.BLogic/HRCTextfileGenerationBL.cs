using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Payroll.DAL;
using System.Drawing;
using System.Configuration;
using CommonLibrary;
using System.Windows.Forms;
using System.IO;


namespace Payroll.BLogic
{
    public class HRCTextfileGenerationBL : BaseBL
    {
        private SaveFileDialog saveDialog = new SaveFileDialog();
        public string Error = string.Empty;
        private DateTime dateGenerate;
        private string MonthYear = string.Empty;
        public DataSet DSEXEMPTIONS = null;
        public DataTable dtNewErrors = null;
        private int EXEMPTIONSCOUNT = 0;
        public string Menucode = string.Empty;
        private ParameterInfo[] param;
        public bool GenerationSuccess = false;
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

        private char[] DeleteChar = { '-', ' ' };

        public DataSet GetCompanyInfo()
        {
            DataSet ds = new DataSet();

            #region query

            string query = @"
                           select 
	                            Case when Mcm_CompanyAddress1 <> ''
	                            then Mcm_CompanyAddress1
	                            else 
	                            ''
	                            end [Address1],
	                            Case when Mcm_CompanyAddress2 <> ''
	                            then Mcm_CompanyAddress2
	                            else 
	                            ''
	                            end [Address2],
	                            Case when Mcd_Name <> '' or Mcd_Name <> null
	                            then Mcd_Name
	                            else 
	                            ''
	                            end [Address3],
	                            M_Company.* 
	                            ,Mcd_Name [Address3]
                            from M_Company
                            left join M_CodeDtl
                            on Mcd_Code = Mcm_CompanyAddress3
                            and Mcd_CodeType = 'ZIPCODE'
                            ";
                           
            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(query, CommandType.Text);

                dal.CloseDB();
            }
            return ds;
        }

        public bool checkCanGenerateATM()
        {
            bool ret = false;
            string query = @"
                select 
	                Tsc_SetFlag
                from T_SettingControl
                where Tsc_SystemCode = 'PAYROLL'
                and Tsc_SettingCode = 'ATMDISKGEN' 

                union

                select 
	                Tsc_SetFlag
                From T_SettingControl
                where Tsc_SystemCode = 'PAYROLL'
                and Tsc_SettingCode = 'PAYCALC'
                ";
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    DataSet ds = dal.ExecuteDataSet(query, CommandType.Text);
                    if (ds.Tables[0].Rows.Count == 1)
                        ret = (bool)ds.Tables[0].Rows[0][0];
                    else
                        ret = false;
                }
                catch
                {
                    ret = false;
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return ret;
        }

        public string FomatQuery(string prof, string TextType, string TextCategory)
        {
            string query = string.Empty;

            int indx = 0;
            if (TextType.IndexOf("ATM DISK") != -1)
                indx = 1;
            else if (TextType.IndexOf("SSS LOAN") != -1)
                indx = 2;
            else if (TextType.IndexOf("SSS PREMIUM") != -1)
                indx = 3;
            else if (TextType.IndexOf("PHILHEALTH") != -1)
                indx = 4;
            else if (TextType.IndexOf("PAG-IBIG") != -1)
                indx = 5;

            switch (indx)
            {
                case 1:
                    #region

                    if (isOverseas())
                    {
                        #region Overseas

                        query = @"
                       
                       select 
                            Mem_LastName,
                            Mem_FirstName,
                            Mem_MiddleName,
                            Mem_PaymentMode,
                            Tpy_BankCode,
                            Tpy_BankAcctNo,
                            Epc_DepositAmt [Tpy_NetAmt],
                            Mem_PaymentMode [Tpy_PaymentMode]
                        From @PROF..E_EmployeePayrollCalcDeposit
                        left join @PROF..M_Employee
                        on Tpy_IDNo = Mem_IDNo
                        @CONDITIONS               

                        UNION

                        select 
                            Mem_LastName,
                            Mem_FirstName,
                            Mem_MiddleName,
                            Mem_PaymentMode,
                            Tpy_BankCode,
                            Tpy_BankAcctNo,
                            Epc_DepositAmt [Tpy_NetAmt],
                            Mem_PaymentMode [Tpy_PaymentMode]
                        From @PROF..E_EmployeePayrollCalcAnnualDeposit
                        left join @PROF..M_Employee
                        on Tpy_IDNo = Mem_IDNo
                        @CONDITIONS

                        UNION

                        select 
                            Mem_LastName,
                            Mem_FirstName,
                            Mem_MiddleName,
                            Mem_PaymentMode,
                            Tpy_BankCode,
                            Tpy_BankAcctNo,
                            Epc_DepositAmt [Tpy_NetAmt],
                            Mem_PaymentMode [Tpy_PaymentMode]
                        From @PROF..E_EmployeePayrollCalcHistDeposit
                        left join @PROF..M_Employee
                        on Tpy_IDNo = Mem_IDNo
                        @CONDITIONS
                        ";

                        #endregion
                    }
                    else
                    {
                        #region Domestic

                        query = @"
                       
                       select 
                            Mem_LastName,
                            Mem_FirstName,
                            Mem_MiddleName,
                            Tpy_PaymentMode,
                            Tpy_BankCode,
                            Tpy_BankAcctNo,
                            Tpy_NetAmt,
                            Tpy_PaymentMode
                        From @PROF..T_EmpPayroll
                        left join @PROF..M_Employee
                        on Tpy_IDNo = Mem_IDNo
                        @CONDITIONS               

                        UNION

                        select 
                            Mem_LastName,
                            Mem_FirstName,
                            Mem_MiddleName,
                            Tpy_PaymentMode,
                            Tpy_BankCode,
                            Tpy_BankAcctNo,
                            Tpy_NetAmt,
                            Tpy_PaymentMode
                        From @PROF..T_EmpPayrollYearly
                        left join @PROF..M_Employee
                        on Tpy_IDNo = Mem_IDNo
                        @CONDITIONS

                        UNION

                        select 
                            Mem_LastName,
                            Mem_FirstName,
                            Mem_MiddleName,
                            Tpy_PaymentMode,
                            Tpy_BankCode,
                            Tpy_BankAcctNo,
                            Tpy_NetAmt,
                            Tpy_PaymentMode
                        From @PROF..T_EmpPayrollHst
                        left join @PROF..M_Employee
                        on Tpy_IDNo = Mem_IDNo
                        @CONDITIONS

                        UNION

                        select 
                            Mem_LastName,
                            Mem_FirstName,
                            Mem_MiddleName,
                            Tpy_PaymentMode,
                            Tpy_BankCode,
                            Tpy_BankAcctNo,
                            Tpy_NetAmt,
                            Tpy_PaymentMode
                        From @PROF..T_EmpPayrollFinalPay
                        left join @PROF..M_Employee
                        on Tpy_IDNo = Mem_IDNo
                        @CONDITIONS

                        UNION

                        select 
                            Mem_LastName,
                            Mem_FirstName,
                            Mem_MiddleName,
                            Tpy_PaymentMode,
                            Tpy_BankCode,
                            Tpy_BankAcctNo,
                            Tpy_NetAmt,
                            Tpy_PaymentMode
                        From @PROF..T_EmpPayrollFinalPayHst
                        left join @PROF..M_Employee
                        on Tpy_IDNo = Mem_IDNo
                        @CONDITIONS
                        ";

                        #endregion
                    }
                    #endregion
                    break;
                case 2:
                    #region
                    query = @"
                    select 
                        REPLACE(Mem_SSSNo, '-', '')[Mem_SSSNo],
                        SSS.Tgr_IDNo,
                        Mem_LastName,
                        Mem_FirstName,
                        Mem_MiddleName,
                        SSS.Tgr_Filler,
                        SSS.Tgr_EEShare                        
						,ISNULL(SSS.Tgr_SalaryBracket, 0) as Tgr_SalaryBracket
						,ISNULL(SUBSTRING(SSS.Tgr_Filler, 1,2), space(2)) as 'Tgr_StatusCode'
						,ISNULL(SUBSTRING(SSS.Tgr_Filler, 3,10), space(10)) as 'Tgr_StatusDate'
						,ISNULL(SUBSTRING(SSS.Tgr_Filler, 13,10), space(10)) as 'Tgr_CheckDate'
						,(select 
                            SUM(Tgr_EEShare)
                            From @PROF..T_EmpGovRemittance SSS2
                            where SSS2.Tgr_IDNo = SSS.Tgr_IDNo
                            And SSS2.Tgr_DeductionCode = SSS.Tgr_DeductionCode
                            And SSS2.Tgr_PayCycleMonth = SSS.Tgr_PayCycleMonth
                            and SUBSTRING(SSS2.Tgr_Filler, 13,10) = SUBSTRING(SSS.Tgr_Filler, 13,10)
                            ) as 'Tgr_LoanAmount'     

                        -- ISNULL(SUBSTRING(SSS.Tgr_Filler, 23,10), space(10)) as 'Tgr_LoanAmount'
						--, (select 
                        --    SUM(Tgr_EEShare)
                        --    From @PROF..T_EmpGovRemittance SSS2
                        --    where SSS2.Tgr_IDNo = SSS.Tgr_IDNo
                        --    And SSS2.Tgr_DeductionCode = SSS.Tgr_DeductionCode
                        --    And SSS2.Tgr_PayCycleMonth = SSS.Tgr_PayCycleMonth
                        --    and SUBSTRING(SSS2.Tgr_Filler, 13,10) = SUBSTRING(SSS.Tgr_Filler, 13,10)
                        --    ) as 'Tgr_PrincipalAmount' 

                        ,ISNULL(SUBSTRING(SSS.Tgr_Filler, 33,10), space(10)) as 'Tgr_PrincipalAmount'
                        ,'0000000' [Penalty Paid]
                        ,' ' [Amort Paid Sign]
                        ,case when Mdn_DeductionCode = 'SSSLOAN'
							then 'S'
                            when Mdn_DeductionCode like 'SLER%'
						    then '1'
                            else ' '
						end [LoanType] 
                    From @PROF..T_EmpGovRemittance SSS
                    left join @PROF..M_Employee
                    on SSS.Tgr_IDNo = Mem_IDNo
                    Left join @PROF..M_Deduction
                    on SSS.Tgr_DeductionCode = Mdn_DeductionCode
                    @CONDITIONS  

                ";
                    #endregion
                    break;
                case 3:
                    #region
                    query = @"
                    select 
                        REPLACE(Mem_SSSNo, '-', '')[Mem_SSSNo],
                        SSS.Tgr_IDNo,
                        Mem_LastName,
                        Mem_FirstName,
                        Mem_MiddleName,
                        CONVERT(varchar(20), Mem_BirthDate, 101) [Mem_BirthDate],

                                            
                        SSS.Tgr_EEShare + SSS.Tgr_ERShare [SSS PRem],
                        isnull(EC.Tgr_ERShare, 0.00) [EC Fund],
                        --isnull(PH.Tgr_EEShare + PH.Tgr_ERShare, 0.00) [PHILHEALTH]
                        0.00 [PHILHEALTH]
                        --, case when Mem_SeparationDate is null 
                        --    then REPLACE(convert(char(10),Mem_IntakeDate,101),'/', '') 
                        --    else REPLACE(convert(char(10),Mem_SeparationDate,101), '/' ,'') 
                        --    end [HIRE SEP DATE]
                        --, case when Mem_SeparationDate is null 
                        --    then (case when convert(char(6),Mem_IntakeDate,112)=SSS.Tgr_PayCycleMonth then '1' else 'N' end) 
                        --    else '2' 
                        --    end [Remarks]
                        ,ISNULL(SUBSTRING(SSS.Tgr_Filler, 1,2), space(2)) as 'Tgr_StatusCode'
						,ISNULL(SUBSTRING(SSS.Tgr_Filler, 3,10), space(10)) as 'Tgr_StatusDate'
						,case when Mem_SeparationDate is null 
                            then convert(char(10),Mem_IntakeDate,112) 
                            else convert(char(10),Mem_SeparationDate,112) 
                            end as 'Tgr_CheckDate'
						,ISNULL(SUBSTRING(SSS.Tgr_Filler, 23,10), space(10)) as 'Tgr_LoanAmount'
						,ISNULL(SUBSTRING(SSS.Tgr_Filler, 33,10), space(10)) as 'Tgr_PrincipalAmount'
                        
                    From  @PROF..T_EmpGovRemittance SSS
                    left join  @PROF..M_Employee
                    on SSS.Tgr_IDNo = Mem_IDNo
			
					left join @PROF..T_EmpGovRemittance EC
					on SSS.Tgr_IDNo = EC.Tgr_IDNo
					and SSS.Tgr_PayCycleMonth = EC.Tgr_PayCycleMonth
					and EC.Tgr_DeductionCode = 'ECFUND'
					
					left join @PROF..T_EmpGovRemittance PH
					on SSS.Tgr_IDNo = PH.Tgr_IDNo
					and SSS.Tgr_PayCycleMonth = PH.Tgr_PayCycleMonth
					and PH.Tgr_DeductionCode = 'PHICPREM'
                    @CONDITIONS  
                ";
                    #endregion
                    break;
                case 4:
                    #region
                    query = @"
                    select
                        Mem_IDNo,
                        Mem_LastName,
                        Mem_FirstName,
                        Mem_MiddleName,
                        Mem_PhilhealthNo,
                        CASE 
                            When Tgr_EEShare = 0.00
							then 0.00
							WHEN Mps_CompensationFrom = 1 
							THEN 4000.00 
							ELSE Mps_CompensationFrom 
							END [Mem_Salary],
                        Tgr_EEShare,
                        Tgr_ERShare,
                        Tgr_Filler
                        ,ISNULL(SUBSTRING(SSS.Tgr_Filler, 1,2), space(2)) as 'Tgr_StatusCode'
						,ISNULL(SUBSTRING(SSS.Tgr_Filler, 3,10), space(10)) as 'Tgr_StatusDate'
                    	
                    From @PROF..T_EmpGovRemittance as SSS
                    left join @PROF..M_Employee
                    on Mem_IDNo = Tgr_IDNo
					left join @PROF..M_PremiumSchedule
                    on Mps_EEShare = Tgr_EEShare
                    and Mps_DeductionCode = 'PHICPREM'
                    and Mps_PayCycle = 
                    (	
						Select MAX(Mps_PayCycle) 
							from @PROF..M_PremiumSchedule
						where Mps_DeductionCode = 'PHICPREM'
						and Mps_PayCycle <= Tgr_PayCycleMonth + '2'
                    )
                    @CONDITIONS 
                ";
                    #endregion
                    break;
                case 5:
                    #region
                    query = @"

                    select                         
                        Mem_IDNo,
                        Mem_LastName,
                        Mem_FirstName,
                        Mem_MiddleName,
                        Mem_PagIbigNo,
                        Mem_TIN,
                        convert(varchar(8),Mem_BirthDate,112) as [Mem_BirthDate],
                        Tgr_EEShare,
                        Tgr_ERShare                       
                    from @PROF..T_EmpGovRemittance as SSS
                    left join @PROF..M_Employee
                    on Mem_IDNo = Tgr_IDNo
                    @CONDITIONS 
                ";
                    #endregion
                    break;
            }
            query = query.Replace("@PROF", prof);


            return query;
        }

        #region Checkings

        public bool checkIfCurrentQuincena(string period)
        {
            bool ret = false;
            string sql = @"
                select 
                    Tps_CycleIndicator
                from T_PaySchedule
                where Tps_PayCycle = '" + period + @"'
                ";

            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();

                    string s = (string) dal.ExecuteScalar(sql, CommandType.Text);
                    if (s == "C")
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

        public DataSet checkBankAccounts(string query)
        {
            DataSet ds;
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    string sql = @"
                        declare @TABLE as table
                        (
	                        [Lastname] varchar(40),
	                        [Firstname] varchar(40),
	                        [Middlename] varchar(40),
	                        [Paymode] varchar(20),
	                        [BankCode] varchar(20),
	                        [BankAccount] varchar(20),
	                        [Netpay] decimal(15, 2),
                            [PaymentMode] varchar(1)
                        )

                        insert into @TABLE
                        {0}

                                        
                        select 
                        [Lastname],
                        [Firstname],
                        [BankCode],
                        [BankAccount],
                        CONVERT(varchar(20), [Netpay]) [Netpay],
                        case when [BankCode] = ''
							then 'NO BANK CODE, '
							else ''
						end
						+
						case when [BankAccount] = 'APPLIED'
							then 'APPLIED, '
							when RTRIM([BankAccount]) = ''
							then 'NO BANK ACCOUNT'
							else ''
						end
						+
						case when [Netpay] < 0
							then 'NETPAY < 0.00, '
							when [Netpay] = 0
							then 'NETPAY = 0.00, '
							else ''
						end [Remarks]
                        from @TABLE
                        where RTRIM([BankCode]) = ''
                        or [BankAccount] = 'APPLIED'
                        or RTRIM([BankAccount]) = ''
						or [Netpay] <=0 
                        or LEN(RTRIM([BankAccount])) < 10
						
                    ";
                    sql = string.Format(sql, query);
                    ds = dal.ExecuteDataSet(sql, CommandType.Text);

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

        public DataSet checkSSSLoan(ref string query)
        {
            DataSet ds;
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    string sql = @"
                         declare @Table as table(
                           [SSS Number]	varchar(20)
                           ,[Employee ID] varchar(20)
                           ,[Lastname] varchar(40)
                           ,[Firstname] varchar(40)
                           ,[Middle Name] varchar(40)
                           ,[Filler] varchar(50)
                           ,[Employee Share] decimal
                          , [Tgr_SalaryBracket] decimal
                            
						,[Tgr_StatusCode] varchar(20)
						,[Tgr_StatusDate] varchar(20)
						,[Tgr_CheckDate] varchar(20)
						,[Tgr_LoanAmount] varchar(20)
						,[Tgr_PrincipalAmount]  varchar(20)
                        ,[Penalty Paid]varchar(20)
                        ,[Amort Paid Sign] varchar(20)
                        ,[LoanType] varchar(5)
                        )

					insert into @Table
					{0}  ";                 
                    
                    string sqlChecking = @"
                    select
						[Employee ID],
						[Lastname],
						[Firstname],
						[SSS Number],
						[Employee Share]
                        ,case when LTRIM([SSS Number]) = ''
							then 'NO SSS NUMBER, ' 
							else ''
						end 
						+
						case when [Employee Share] = 0
							then 'EMPLOYEE SHARE = 0, '
							when [Employee Share] < 0
							then 'EMPLOYEE SHARE < 0, '
							else ''
						end [Remarks]
					From @Table
					where LTRIM([SSS Number]) = ''
					    or [Employee Share] < 0
                    ORDER BY [Lastname], [Firstname], [Middle Name]
                    ";

                    string sqlSum = @" 
                    SELECT  MAX([SSS Number]) AS Mem_SSSNo
                            ,[Employee ID] AS Tgr_IDNo
                            ,[Lastname] AS Mem_LastName
                            ,[Firstname] AS Mem_FirstName 
                            ,[Middle Name] AS Mem_MiddleName 
                            ,MAX([Filler]) AS Tgr_Filler
                            ,SUM([Employee Share]) AS Tgr_EEShare
                            ,MAX([Tgr_SalaryBracket]) AS Tgr_SalaryBracket
                            ,MAX([Tgr_StatusCode]) AS Tgr_StatusCode
                            ,MAX([Tgr_StatusDate]) AS Tgr_StatusDate
                            ,MAX([Tgr_CheckDate]) AS Tgr_CheckDate
                            ,MAX([Tgr_LoanAmount]) AS Tgr_LoanAmount
                            ,MAX([Tgr_PrincipalAmount]) AS Tgr_PrincipalAmount
                            ,MAX([Penalty Paid]) AS [Penalty Paid]
                            ,MAX([Amort Paid Sign]) AS [Amort Paid Sign]
                            ,MAX([LoanType]) AS [LoanType]
                    FROM @Table
                    GROUP BY [Employee ID], [Lastname], [Firstname], [Middle Name] ";

                    sqlChecking = string.Format(sql, query) + sqlChecking;
                    sqlSum = string.Format(sql, query) + sqlSum;
                    query = sqlSum;
                    ds = dal.ExecuteDataSet(sqlChecking, CommandType.Text);

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

        public DataSet checkSSSPrem(ref string query)
        {
            DataSet ds;
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    string sql = @"
                          declare @Table as table(
                            [SSS Number]	varchar(20)
                           ,[Employee ID] varchar(20)
                           ,[Lastname] varchar(40)
                           ,[Firstname] varchar(30)
                           ,[Middle Name] varchar(40)
                           ,[Birthdate] varchar(20)
                           ,[SSS Prem] decimal
                           ,[EC Fund] decimal 
                           ,[Philhealth] decimal
                           --,[Hire / Sep date] varchar(20)
                           --,[Stat] varchar(20)
                           
                        ,[Tgr_StatusCode]  varchar(20)
						,[Tgr_StatusDate] varchar(20)
						,[Tgr_CheckDate] varchar(20)
						,[Tgr_LoanAmount] varchar(20)
						,[Tgr_PrincipalAmount] varchar(20)
                        )

					insert into @Table
					{0} ";
                  
                    string sqlChecking = @" 
                    select
						[Employee ID],
						[Lastname],
						[Firstname],
						CONVERT (varchar(20), [SSS Prem]) [SSS Prem],
						CONVERT (varchar(20), [EC Fund]) [EC Fund],
						CONVERT (varchar(20), [Philhealth]) [Philhealth]
                        ,case when LTRIM([SSS Number]) = ''
							then 'NO SSS NUMBER, '
							else ''
						end 
						+
						case when [SSS Prem]= 0
							then 'SSS PREM = 0, '
							when [SSS Prem] < 0
							then 'SSS PREM < 0, '
							else ''
						end 
						+
						case when [EC Fund]= 0
							then 'EC FUND = 0, '
							when [EC Fund] < 0
							then 'EC FUND < 0, '
							else ''
						end 
						+
						case when [Philhealth]= 0
							then 'Philhealth PREM = 0, '
							when [Philhealth] < 0
							then 'Philhealth PREM < 0, '
							else ''
						end [Remarks]
						
					From @Table
					where LTRIM([SSS Number]) = ''
					or [SSS Prem]<= 0
					or [EC Fund] <= 0
					--or [Philhealth] <= 0
                    ORDER BY [Employee ID], [Lastname], [Firstname], [Middle Name]";

                    string sqlSum = @" 
                    SELECT  MAX([SSS Number]) AS Mem_SSSNo
                            ,[Employee ID] AS Tgr_IDNo
                            ,[Lastname] AS Mem_LastName
                            ,[Firstname] AS Mem_FirstName 
                            ,[Middle Name] AS Mem_MiddleName 
                            ,MAX([Birthdate]) AS Mem_BirthDate
                            ,SUM([SSS Prem]) AS [SSS Prem]
                            ,SUM([EC Fund]) AS [EC Fund] 
                            ,SUM([Philhealth]) AS [Philhealth]
                            ,MAX([Tgr_StatusCode]) AS [Tgr_StatusCode]
                            ,MAX([Tgr_StatusDate]) AS [Tgr_StatusDate]
                            ,MAX([Tgr_CheckDate]) AS [Tgr_CheckDate]
                            ,MAX([Tgr_LoanAmount]) AS [Tgr_LoanAmount]
                            ,MAX([Tgr_PrincipalAmount]) AS [Tgr_PrincipalAmount]
                    FROM @Table
                    GROUP BY [Employee ID], [Lastname], [Firstname], [Middle Name] ";

                    sqlChecking = string.Format(sql, query) + sqlChecking;
                    sqlSum = string.Format(sql, query) + sqlSum;
                    query = sqlSum;
                    ds = dal.ExecuteDataSet(sqlChecking, CommandType.Text);

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

        public DataSet checkPHILHEALTHPrem(ref string query)
        {
            DataSet ds;
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    string sql = @"
                        declare @Table as table(
							[Employee ID] varchar(20)
                            ,[Lastname] varchar(40)
                            ,[Firstname] varchar(30)
                            ,[Middlename] varchar(20)
                            ,[Philhealth Number] varchar(20)
                            ,[Salary Rate] varchar(20)
                            ,[Employee Share] decimal(8,2)
                            ,[Employer Share] decimal(8,2)
                            ,[Filler] varchar(50)
                            ,[Status] varchar(20)
                            ,[HIRESEP] varchar(20)
                        )

					insert into @Table
					{0} ";                  
                   
                    string sqlChecking = @"
                     select
						[Employee ID],
						[Lastname],
						[Firstname],
						[Philhealth Number],
						CONVERT(varchar(20), [Employee Share]) [Employee Share],
						CONVERT(varchar(20), [Employer Share]) [Employer Share]
                        ,case when [Philhealth Number] = 'APPLIED'
							then 'PHILHEALTH NUMBER = APPLIED, '
							when [Philhealth Number] = 'PENDING'
							then 'PHILHEALTH NUMBER = PENDING, '
							when [Philhealth Number] = ''
							then 'NO PHILHEALTH NUMBER, '
							else ''
						end 
						+ 
						case when [Employee Share] = 0
							then 'Employee Share = 0, '
							when [Employee Share] < 0
							then 'Employee Share < 0, '
							else ''
						end 
						+
						case when [Employer Share] = 0
							then 'Employer Share = 0, '
							when [Employer Share] < 0
							then 'Employer Share < 0, '
							else ''
						end  [Remarks]
					From @Table
					where 
						[Philhealth Number] in ('', 'APPLIED', 'PENDING')
						or [Employee Share] < 0
						or [Employer Share] < 0
                    ORDER BY [Lastname], [Firstname], [Middlename]";

                    string sqlSum = @" 
                    SELECT  [Employee ID] AS Mem_IDNo
                            ,[Lastname] AS Mem_LastName
                            ,[Firstname] AS Mem_FirstName 
                            ,[Middlename] AS Mem_MiddleName 
                            ,MAX([Philhealth Number]) AS Mem_PhilhealthNo
                            ,MAX([Salary Rate]) AS [Mem_Salary]
                            ,SUM([Employee Share]) AS Tgr_EEShare
                            ,SUM([Employer Share]) AS Tgr_ERShare
                            ,MAX([Filler]) AS Tgr_Filler
                            ,CASE WHEN MAX([Status]) = 'NE' AND SUM([Employee Share]) > 0
									THEN ''
									ELSE MAX([Status])
									END AS [Tgr_StatusCode]
                            ,MAX([HIRESEP]) AS [Tgr_StatusDate]
                    FROM @Table
                    GROUP BY [Employee ID], [Lastname], [Firstname], [Middlename] ";

                    sqlChecking = string.Format(sql, query) + sqlChecking;
                    sqlSum = string.Format(sql, query) + sqlSum;
                    query = sqlSum;
                    ds = dal.ExecuteDataSet(sqlChecking, CommandType.Text);

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

        public DataSet checkPAGIBIGPrem(ref string query)
        {
            DataSet ds;
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    string sql = @"
                         declare @Table as table(
							[Employee ID] varchar(20)
                            ,[Lastname] varchar(40)
                            ,[Firstname] varchar(30)
                            ,[Middlename] varchar(20)
                            ,[PAG-IBIG Number] varchar(30)
                            ,[TIN Number] varchar(30)
                            ,[Birthday] varchar(20)
                            ,[Employee Share] decimal(8,2)
                            ,[Employer Share] decimal(8,2)
                        )

					insert into @Table
					{0}  "; 
                        
                    string sqlChecking = @"  
                    select
						[Employee ID],
						[Lastname],
						[Firstname],
						[PAG-IBIG Number],
						[TIN Number],
						[Birthday],
						CONVERT(varchar(20), [Employee Share]) [Employee Share],
						CONVERT(varchar(20), [Employer Share]) [Employer Share]
                        ,case when [PAG-IBIG Number] = 'APPLIED'
							then 'PAG-IBIG NUMBER = APPLIED, '
							when [PAG-IBIG Number] = 'PENDING'
							then 'PAG-IBIG NUMBER = PENDING, '
							when [PAG-IBIG Number] = ''
							then 'NO PAG-IBIG NUMBER, '
							else ''
						end 
						+
						case when [TIN Number] = 'APPLIED'
							then 'TIN NUMBER = APPLIED, '
							when [TIN Number] = 'PENDING'
							then 'TIN NUMBER = PENDING, '
							when [TIN Number] = ''
							then 'NO TIN NUMBER, '
							else ''
						end 
						+
						case when [Birthday] = '' 
							then 'NO BIRTHDAY, '
							else ''	
						end 
						+
						case when [Employee Share] = 0
							then 'Employee Share = 0, '
							when [Employee Share] < 0
							then 'Employee Share < 0, '
							else ''
						end +
						case when [Employer Share] = 0
							then 'Employer Share = 0, '
							when [Employer Share] < 0
							then 'Employer Share < 0, '
							else ''
						end [Remarks]
					From @Table
					where 
						[PAG-IBIG Number] in ('APPLIED', 'PENDING')
						or [TIN Number] = ''
						or [Employee Share] <= 0
						or [Employer Share] <= 0
                    ORDER BY [Lastname], [Firstname], [Middlename]
                    ";

                    string sqlSum = @" 
                    SELECT  [Employee ID] AS Mem_IDNo
                            ,[Lastname] AS Mem_LastName
                            ,[Firstname] AS Mem_FirstName 
                            ,[Middlename] AS Mem_MiddleName 
                            ,MAX([PAG-IBIG Number]) AS Mem_PagIbigNo
                            ,MAX([TIN Number]) AS Mem_TIN
                            ,MAX([Birthday]) AS [Mem_BirthDate]
                            ,SUM([Employee Share]) AS Tgr_EEShare
                            ,SUM([Employer Share]) AS Tgr_ERShare
                    FROM @Table
                    GROUP BY [Employee ID], [Lastname], [Firstname], [Middlename] ";

                    sqlChecking = string.Format(sql, query) + sqlChecking;
                    sqlSum = string.Format(sql, query) + sqlSum;
                    query = sqlSum;
                    ds = dal.ExecuteDataSet(sqlChecking, CommandType.Text);

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

        #endregion

        public string getHighestNetPay(DataSet ds)
        { 
            string temp = "0";
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                int j = 0;
                if (EXEMPTIONSCOUNT > 0)
                {
                    for (; j < DSEXEMPTIONS.Tables[0].Rows.Count
                            && (
                            DSEXEMPTIONS.Tables[0].Rows[j]["Lastname"].ToString() != ds.Tables[0].Rows[i]["Mem_LastName"].ToString()
                            ||
                            DSEXEMPTIONS.Tables[0].Rows[j]["Firstname"].ToString() != ds.Tables[0].Rows[i]["Mem_FirstName"].ToString()
                            )
                        ; j++) ;
                }
                if (j == EXEMPTIONSCOUNT)
                {
                    if (Convert.ToDouble(temp) < Convert.ToDouble(ds.Tables[0].Rows[i]["Tpy_NetAmt"].ToString()))
                        temp = ds.Tables[0].Rows[i]["Tpy_NetAmt"].ToString();
                }
            }
            return temp.Replace(".", "");
        }

        public string getTotalNetpay(DataSet ds)
        {
            double total = 0;
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                int j = 0;
                if (EXEMPTIONSCOUNT > 0)
                {
                    for (; j < DSEXEMPTIONS.Tables[0].Rows.Count
                            && (
                            DSEXEMPTIONS.Tables[0].Rows[j]["Lastname"].ToString() != ds.Tables[0].Rows[i]["Mem_LastName"].ToString()
                            ||
                            DSEXEMPTIONS.Tables[0].Rows[j]["Firstname"].ToString() != ds.Tables[0].Rows[i]["Mem_FirstName"].ToString()
                            )
                        ; j++) ;
                }
                if (j == EXEMPTIONSCOUNT)
                {
                    total += Convert.ToDouble(ds.Tables[0].Rows[i]["Tpy_NetAmt"].ToString());
                }
            }
            return string.Format("{0:0.00}", total).Replace(".", "");
        
        }
        
        public void ProcessGeneration(string query, string monthyear, string TextType, string TextCategory, DateTime dt, ParameterInfo[] par, bool isBranchTransaction)
        {
            GenerationSuccess = false;
            dateGenerate = dt;
            MonthYear = monthyear;
            saveDialog.InitialDirectory = Environment.SpecialFolder.Desktop.ToString();
            DataSet dsComp = GetCompanyInfo();
            param = par;

            #region Dialogs
            switch (TextType)
            { 
                case "ATM DISK GENERATION":
                    #region
                    if (TextCategory.IndexOf("METROBANK") != -1)
                    {
                        saveDialog.Filter = "DAT Files (*.dat)|*.dat";
                        saveDialog.FileName = "PAYROLL";
                    }
                    else if (TextCategory.IndexOf("RCBC") != -1)
                    {
                        #region
                        string year = DateTime.Now.Year.ToString();
                        string sql = @"
                            select COUNT(*) from T_PaySchedule
                            where LEFT(Tps_PayCycle, 4) = '{0}'
                            and Tps_CycleIndicator in ('P', 'C')
                            ";
                        sql = string.Format(sql, year);
                        int i = 1;
                        string filename = string.Empty;
                        using (DALHelper dal = new DALHelper())
                        {
                            try
                            {
                                dal.OpenDB();
                                i = Convert.ToInt32 ( dal.ExecuteScalar(sql, CommandType.Text) );
                                DataSet dstemp = dal.ExecuteDataSet(
                                    @" select * from M_Company ", CommandType.Text);
                                filename = "S"
                                        + dstemp.Tables[0].Rows[0]["Mcm_BankBranchCode"].ToString().Trim().Replace("-", "").Replace(" ", "").Substring(0, 3)
                                        + dstemp.Tables[0].Rows[0]["Mcm_CompanyCode"].ToString().Trim().Replace("-", "").Replace(" ", "").Substring(0, 2)
                                        + DateTime.Now.ToString("dd").Trim()
                                        + ".TX" + i.ToString().Trim();
                                saveDialog.FileName = filename;
                                saveDialog.Filter = @"TX"+ i.ToString().Trim() + @" (*.TX " + i.ToString().Trim() + @")|*.TX" + i.ToString().Trim();
                            }
                            catch
                            {
                                i = 1;
                                saveDialog.Filter = @"TX" + i.ToString().Trim() + @" (*.TX " + i.ToString().Trim() + @")|*.TX" + i.ToString().Trim();
                            }
                            finally
                            {
                                dal.CloseDB();
                            }
                        }
                        #endregion
                    }
                    else if (TextCategory.IndexOf("BPI") != -1)
                    {
                        #region
                        string year = DateTime.Now.Year.ToString();
                        string sql = @"
                            select COUNT(*) from T_PaySchedule
                            where Tps_CycleIndicator in ('P', 'C')
                            and Tps_PayCycle <= '{0}'
                            ";
                        sql = string.Format(sql, monthyear);
                        int i = 1;
                        string filename = string.Empty;
                        using (DALHelper dal = new DALHelper())
                        {
                            try
                            {
                                dal.OpenDB();
                                i = Convert.ToInt32(dal.ExecuteScalar(sql, CommandType.Text));
                                DataSet dstemp = dal.ExecuteDataSet(
                                    @" select * from M_Company ", CommandType.Text);
                                filename = dstemp.Tables[0].Rows[0]["Mcm_ATMBankCode"].ToString().Trim().Replace("-", "").Replace(" ", "");
                                saveDialog.FileName = filename;
                                if (isBranchTransaction)
                                {
                                    saveDialog.Filter = "";
                                }
                                else
                                {
                                    if (i.ToString().Trim().Length < 2)
                                        saveDialog.Filter = @"0" + i.ToString().Trim() + @" (*.0" + i.ToString().Trim() + @")|*.0" + i.ToString().Trim();
                                    else
                                        saveDialog.Filter = i.ToString().Trim() + @" (*." + i.ToString().Trim() + @")|*." + i.ToString().Trim();
                                }
                            }
                            catch
                            {
                                i = 1;
                                saveDialog.Filter = @"0" + i.ToString().Trim() + @" (*.0" + i.ToString().Trim() + @")|*.0" + i.ToString().Trim();
                            }
                            finally
                            {
                                dal.CloseDB();
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        saveDialog.Filter = "Text Files (*.txt)|*.txt";
                    }
                    break;
                    #endregion
                case "SSS LOAN REMITTANCE":
                    #region
                    if (TextCategory == "GOVERNMENT AGENCY" || TextCategory == "UNIONBANK" || TextCategory == "METROBANK")
                    {
                        saveDialog.FileName = dsComp.Tables[0].Rows[0]["Mcm_CompanyCode"].ToString().Trim() + MonthYear.Substring(2, 4);
                        saveDialog.Filter = "Text Files (*.txt)|*.txt";
                    }                    
                    break;
                    #endregion
                case "SSS PREMIUM REMITTANCE":
                    #region
                    if (TextCategory == "GOVERNMENT AGENCY" || TextCategory == "UNIONBANK" || TextCategory == "METROBANK")
                    {
                        saveDialog.FileName = "NR3001DK";
                        saveDialog.Filter = "Text Files (*.txt)|*.txt";
                    }
                    else if (TextCategory == "BDO")
                    {
                        DateTime dttemp = Convert.ToDateTime(MonthYear.Substring(4, 2) + "/01/" + MonthYear.Substring(0, 4));
                        saveDialog.FileName = "MCL_" + dttemp.ToString("MMMM yyyy");
                        saveDialog.Filter = "Text Files (*.txt)|*.txt";
                    }
                    break;
                    #endregion
                case "PHILHEALTH PREMIUM REMITTANCE":
                    #region
                    if (TextCategory == "GOVERNMENT AGENCY" || TextCategory == "UNIONBANK")
                    {
                        int reportQuarter = 0;
                        if (Convert.ToInt32(MonthYear.Substring(4, 2)) <= 3)
                            reportQuarter = 1;
                        else if (Convert.ToInt32(MonthYear.Substring(4, 2)) <= 6)
                            reportQuarter = 2;
                        else if (Convert.ToInt32(MonthYear.Substring(4, 2)) <= 9)
                            reportQuarter = 3;
                        else if (Convert.ToInt32(MonthYear.Substring(4, 2)) <= 12)
                            reportQuarter = 4;

                        saveDialog.FileName = dsComp.Tables[0].Rows[0]["Mcm_CompanyName"].ToString() + "ph" + MonthYear.Substring(0, 4) + reportQuarter.ToString();
                        saveDialog.Filter = "Text Files (*.txt)|*.txt";
                    }
                    break;
                    #endregion
                case "PAG-IBIG PREMIUM AND LOAN REMITTANCE":
                    #region
                    if (TextCategory == "GOVERNMENT AGENCY" || TextCategory == "UNIONBANK")
                    {
                        using (DALHelper dal = new DALHelper())
                        {
                            string s = string.Empty;
                            try
                            {
                                dal.OpenDB();
                                s = dal.ExecuteScalar(string.Format(@"select DATENAME(M, '{0}')", MonthYear.Substring(0,4) + "-" + MonthYear.Substring(4, 2) + "-01")).ToString();
                            }
                            catch
                            {
                                s = string.Empty;
                            }
                            finally
                            {
                                dal.CloseDB();
                            }
                            saveDialog.FileName = "HDMF Remittance for " + s + " " + MonthYear.Substring(0, 4);
                            saveDialog.Filter = "Text Files (*.txt)|*.txt";
                        }
                    }                    
                    break;
                    #endregion
                default:
                    CommonProcedures.showMessageError(TextType + " textfile generation not yet available");
                    return;
                    break;
            }
            #endregion

            DialogResult result = saveDialog.ShowDialog();
            
            if (result == DialogResult.OK)
            {
                   
                string sqlQuery = query;

                DataSet dsExemption;
                if (TextType == "ATM DISK GENERATION")
                    dsExemption = checkBankAccounts(sqlQuery);
                else if (TextType.IndexOf("SSS LOAN") != -1)
                    dsExemption = checkSSSLoan(ref sqlQuery);
                else if (TextType.IndexOf("SSS PREMIUM") != -1)
                    dsExemption = checkSSSPrem(ref sqlQuery);
                else if (TextType.IndexOf("PHILHEALTH PREMIUM") != -1)
                    dsExemption = checkPHILHEALTHPrem(ref sqlQuery);
                else if (TextType.IndexOf("PAG-IBIG") != -1)
                    dsExemption = checkPAGIBIGPrem(ref sqlQuery);
                else
                    dsExemption = null;

                DataSet dsRecords;
                using (DALHelper dal = new DALHelper())
                {
                    try
                    {
                        dal.OpenDB();

                        if (sqlQuery != string.Empty)
                        {
                            if (TextType == "ATM DISK GENERATION")
                            {
                                sqlQuery += @" order by Mem_LastName, Mem_FirstName ";
                            }
                            else
                            {
                                sqlQuery += @" order by [Lastname], [Firstname] ";
                            }
                        }

                        dsRecords = dal.ExecuteDataSet(sqlQuery, CommandType.Text);
                        if (dsRecords.Tables[0].Rows.Count > 0)
                        {

                            ReadyTextfileWritables(dsRecords, dsExemption, TextType, TextCategory);
                        }
                        else
                        {
                            CommonProcedures.showMessageError("No Records Found");
                        }
                    }
                    catch
                    {
                        CommonProcedures.showMessageError("Unable to Generate Textfile, Query Error");
                    }
                    finally
                    {
                        dal.CloseDB();
                    }
                }

            }
        }

        public void WriteToText(string text)
        {
            try
            {
                if (text != string.Empty)
                {
                    TextWriter txtWriter = new StreamWriter(saveDialog.FileName);
                    txtWriter.WriteLine(text);
                    txtWriter.Close();
                    GenerationSuccess = true;
                    CommonProcedures.showMessageInformation("Textfile Generation Successful!");
                }
                else
                {
                    CommonProcedures.showMessageError("No Textfile Generated!");
                }
            }
            catch
            {
                CommonProcedures.showMessageError("Error in saving textfile");
            }
        }

        public void ReadyTextfileWritables(DataSet ds, DataSet dsExemp, string TextType, string TextCategory)
        {
            DSEXEMPTIONS = dsExemp;
            EXEMPTIONSCOUNT = dsExemp.Tables[0].Rows.Count;
            if (TextType.IndexOf("ATM DISK") != -1)
            {
                #region
                if (TextCategory.IndexOf("UBP") != -1)
                {
                    WriteToText( UBPTextFile(ds, dsExemp));
                }
                else if (TextCategory.IndexOf("METROBANK") != -1)
                {
                    WriteToText(METROBANKTextFile(ds, dsExemp));
                }
                else if (TextCategory.IndexOf("RCBC") != -1)
                {
                    WriteToText(RCBCTextFile(ds, dsExemp));
                }
                else if (TextCategory.IndexOf("BPI") != -1)
                {
                    WriteToText(BPITextFile(ds, dsExemp));
                }
                else if (TextCategory.IndexOf("BDO") != -1)
                {
                    WriteToText(BDOTextFile(ds, dsExemp));
                }
                else
                {
                    CommonProcedures.showMessageError("Textfile Generation for " + TextCategory + " is not available");
                    return;
                }
                #endregion
            }
            else if (TextType.IndexOf("SSS LOAN") != -1)
            {
                #region
                if (TextCategory == "GOVERNMENT AGENCY")
                {
                    WriteToText(SSSLoanGov(ds));
                }
                else if (TextCategory == "UNIONBANK")
                {
                    WriteToText(SSSLoanUnionBank(ds));
                }
                else if (TextCategory == "METROBANK")
                {
                    WriteToText(SSSLoanMetroBank(ds));
                }
                #endregion
            }
            else if (TextType.IndexOf("SSS PREMIUM") != -1)
            {
                #region
                if (TextCategory == "GOVERNMENT AGENCY")
                {
                    WriteToText(SSSPremGov(ds));
                }
                else if (TextCategory == "UNIONBANK")
                {
                    WriteToText(SSSPremUnionBank(ds));
                }
                else if (TextCategory == "METROBANK")
                {
                    WriteToText(SSSPremMetroBankMCL(ds));
                }
                else if (TextCategory == "BDO")
                {
                    WriteToText(SSSPremBDO(ds));
                }
                #endregion
            }
            else if (TextType.IndexOf("PHILHEALTH") != -1)
            {
                #region
                if (TextCategory == "GOVERNMENT AGENCY")
                {
                    WriteToText(PhilHealthGov(ds));
                }
                else if (TextCategory == "UNIONBANK")
                {
                    WriteToText(PhilHealthUnionBank(ds));
                }
                #endregion
            }
            else if (TextType.IndexOf("PAG-IBIG") != -1)
            {
                #region
                if (TextCategory == "GOVERNMENT AGENCY")
                {
                    WriteToText(PagIbigGov(ds));
                }
                else if (TextCategory == "UNIONBANK")
                {
                    WriteToText(PagIbigUnionBank(ds));
                }
                #endregion
            }

            if (dsExemp.Tables[0].Rows.Count > 0)
                DSEXEMPTIONS = dsExemp;
        }

        #region Textfiles 

        //OK
        #region Banks

        //OK
        private string UBPTextFile(DataSet ds, DataSet dsExemp)
        {
            string text = string.Empty;
            int DetailCounter = 0;
            Error = string.Empty;


            //DETAILS
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                string temp = string.Empty;
                int j = 0;
                for (; j < dsExemp.Tables[0].Rows.Count
                        && (
                        dsExemp.Tables[0].Rows[j]["Lastname"].ToString() != ds.Tables[0].Rows[i]["Mem_LastName"].ToString()
                        ||
                        dsExemp.Tables[0].Rows[j]["Firstname"].ToString() != ds.Tables[0].Rows[i]["Mem_FirstName"].ToString()
                        )
                    ; j++) ;
                if (j == dsExemp.Tables[0].Rows.Count)
                {
                    string errTemp = string.Empty;
                    try
                    {
                        ++DetailCounter;
                        errTemp = "Error in SequenceNumber";                        
                        temp = DetailCounter + @";";

                        errTemp = "Error in BankAccountnumber";
                        temp += ds.Tables[0].Rows[i]["Tpy_BankAcctNo"].ToString().Replace(" ", "").Replace("-", "") + @";";

                        errTemp = "Error in Name";
                        temp += ds.Tables[0].Rows[i]["Mem_LastName"].ToString().Trim() + " " + ds.Tables[0].Rows[i]["Mem_FirstName"].ToString().Trim();
                        if (ds.Tables[0].Rows[i]["Mem_LastName"].ToString().Trim() != string.Empty)
                            temp += " " +ds.Tables[0].Rows[i]["Mem_MiddleName"].ToString().Substring(0, 1);
                        temp += @";";

                        errTemp = "Error in Netpay";
                        temp += string.Format("{0:0.00}", Convert.ToDecimal(ds.Tables[0].Rows[i]["Tpy_NetAmt"]));

                        text += temp + "\r\n";
                    }
                    catch
                    {
                        DataRow dr = DSEXEMPTIONS.Tables[0].NewRow();
                        dr["Lastname"] = ds.Tables[0].Rows[i]["Mem_LastName"].ToString();
                        dr["Firstname"] = ds.Tables[0].Rows[i]["Mem_FirstName"].ToString();
                        dr["Remarks"] = errTemp;
                        DSEXEMPTIONS.Tables[0].Rows.Add(dr);
                    }

                }
            }

            return text;

        }

        //OK
        private string METROBANKTextFile(DataSet ds, DataSet dsExemp)
        {
            string text = string.Empty;
            int DetailCounter = 0;
            Error = string.Empty;

            DataSet dsComp = GetCompanyInfo();

            //DETAILS
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                string temp = string.Empty;
                int j = 0;
                for (; j < dsExemp.Tables[0].Rows.Count
                        && (
                        dsExemp.Tables[0].Rows[j]["Lastname"].ToString() != ds.Tables[0].Rows[i]["Mem_LastName"].ToString()
                        ||
                        dsExemp.Tables[0].Rows[j]["Firstname"].ToString() != ds.Tables[0].Rows[i]["Mem_FirstName"].ToString()
                        )
                    ; j++) ;
                if (j == dsExemp.Tables[0].Rows.Count)
                {
                    string errTemp = string.Empty;
                    try
                    {
						if(text.Trim() != string.Empty)
						{
							temp += "\r\n";
						}
                        errTemp = "Error in SequenceNumber";
                        temp += "2"; ;

                        errTemp = "Error in Company Branch Code";
                        temp += dsComp.Tables[0].Rows[0]["Mcm_BankBranchCode"].ToString().Trim().Replace("-", "").Replace(" ","").Substring(0,3);

                        errTemp = "Error in Institution Code";
                        temp += "26";

                        errTemp = "Error in CurrencyCode";
                        temp += "001";

                        errTemp = "Error in Branch code 2";
                        temp += dsComp.Tables[0].Rows[0]["Mcm_BankBranchCode"].ToString().Trim().Replace("-", "").Replace(" ", "").Substring(0, 3);

                        errTemp = "Error in Fixed Value";
                        temp += "0000000";

                        errTemp = "Error in Company Name";
                        if (dsComp.Tables[0].Rows[0]["Mcm_CompanyName"].ToString().Trim().Length < 40)
                        {
                            int len = dsComp.Tables[0].Rows[0]["Mcm_CompanyName"].ToString().Trim().Length;
                            temp += dsComp.Tables[0].Rows[0]["Mcm_CompanyName"].ToString().Trim();
                            for (int idx = 0; idx < 40 - len; idx++)
                                temp += " ";
                        }
                        else
                        {
                            temp += dsComp.Tables[0].Rows[0]["Mcm_CompanyName"].ToString().Trim().Substring(0, 40).Substring(0, 40);
                        }

                        errTemp = "Error in BankAccountnumber";
                        if (ds.Tables[0].Rows[i]["Tpy_BankAcctNo"].ToString().Trim().Replace(" ", "").Replace("-", "").Length < 10)
                        {
                            throw new Exception();
                        }
                        else
                        {
                            temp += ds.Tables[0].Rows[i]["Tpy_BankAcctNo"].ToString().Trim().Replace(" ", "").Replace("-", "").Substring(0, 10);
                        }

                        errTemp = "Error in Amount";
                        if (ds.Tables[0].Rows[i]["Tpy_NetAmt"].ToString().Trim().Replace(",", "").Replace(" ", "").Replace(".", "").Length < 15)
                        {
                            string temp2 = string.Empty;
                            temp2 = ds.Tables[0].Rows[i]["Tpy_NetAmt"].ToString().Trim().Replace(",", "").Replace(" ", "").Replace(".", "");
                            int len = ds.Tables[0].Rows[i]["Tpy_NetAmt"].ToString().Trim().Replace(",", "").Replace(" ", "").Replace(".", "").Length;
                            for (int idx = 0; idx < (15 - len); idx++)
                            {
                                temp2 = "0" + temp2;
                            }
                            temp += temp2;
                        }
                        else
                        {
                            temp += ds.Tables[0].Rows[i]["Tpy_NetAmt"].ToString().Trim().Replace(",", "").Replace(" ", "").Replace(".", "").Substring(0, 15);
                        }

                        errTemp = "Error in Fixed Value";
                        temp += "9";

                        errTemp = "Error in Company code";
                        if (dsComp.Tables[0].Rows[0]["Mcm_ATMBankCode"].ToString().Trim().Length == 5)
                        {
                            temp += dsComp.Tables[0].Rows[0]["Mcm_ATMBankCode"].ToString().Trim();
                        }
                        else
                        {
                            throw new Exception();
                        }
                        errTemp = "Error in Date Generated";
                        temp += dateGenerate.ToString("MMddyyyy");

                        text += temp;
                        ++DetailCounter;
                    }
                    catch
                    {
                        DataRow dr = DSEXEMPTIONS.Tables[0].NewRow();
                        dr["Lastname"] = ds.Tables[0].Rows[i]["Mem_LastName"].ToString();
                        dr["Firstname"] = ds.Tables[0].Rows[i]["Mem_FirstName"].ToString();
                        dr["Remarks"] = errTemp;
                        DSEXEMPTIONS.Tables[0].Rows.Add(dr);
                    }

                }
            }


            return text;

        }

        //OK
        private string RCBCTextFile(DataSet ds, DataSet dsExemp)
        {
            if (dateGenerate.DayOfWeek == DayOfWeek.Saturday || dateGenerate.DayOfWeek == DayOfWeek.Sunday)
            {
                CommonProcedures.showMessageError("Cannot Generate Textfile on a " + dateGenerate.DayOfWeek.ToString());
                return string.Empty;
            }

            string sql = @"
            select * from T_Holiday
            where Thl_HolidayDate = '" + dateGenerate.ToString("MM/dd/yyyy") + @"'
            ";

            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    if (dal.ExecuteDataSet(sql, CommandType.Text).Tables[0].Rows.Count > 0)
                    {
                        CommonProcedures.showMessageError("Cannot Generate Textfile on a Holiday");
                        return string.Empty;
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

            string text = string.Empty;
            int DetailCounter = 0;
            Error = string.Empty;
            Int64 dAccounts = 0;
            Int64 dNetpays = 0;
            Int64 dHash = 0;

            DataSet dsComp = GetCompanyInfo();

            #region Details
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                string temp = string.Empty;
                int j = 0;
                for (; j < dsExemp.Tables[0].Rows.Count
                        && (
                        dsExemp.Tables[0].Rows[j]["Lastname"].ToString() != ds.Tables[0].Rows[i]["Mem_LastName"].ToString()
                        ||
                        dsExemp.Tables[0].Rows[j]["Firstname"].ToString() != ds.Tables[0].Rows[i]["Mem_FirstName"].ToString()
                        )
                    ; j++) ;
                if (j == dsExemp.Tables[0].Rows.Count)
                {
                    string errTemp = string.Empty;
                    try
                    {
                        errTemp = "Error in Number Constant";
                        temp = "01001"; 

                        errTemp = "Error in BankAccountnumber";
                        string temp2 = string.Empty;
                        
                        if (ds.Tables[0].Rows[i]["Tpy_BankAcctNo"].ToString().Trim().Replace(" ", "").Replace("-", "").Length < 3)
                        {
                            throw new Exception();
                        }
                        else
                        {
                            temp2 = ds.Tables[0].Rows[i]["Tpy_BankAcctNo"].ToString().Trim().Replace(" ", "").Replace("-", "");
                            if (temp2.Length < 14)
                            {
                                temp2 = temp2.PadLeft(14, '0');
                            }
                            else
                            {
                                temp2 = temp2.Substring(0, 14);
                            }
                            temp2 = temp2.Substring(5, 3);
                            if (temp2 == "000")
                                temp2 = "001";
                        }
                        


                        temp += temp2;
                        temp2 = string.Empty;

                        errTemp = "Error in constant 2";
                        temp += "000";

                        errTemp = "Error in Bankaccount 2";
                        temp2 = ds.Tables[0].Rows[i]["Tpy_BankAcctNo"].ToString().Trim().Replace(" ", "").Replace("-", "");


                        string TempAccount = string.Empty;
                        if (temp2.Length < 14)
                        {
                            temp += temp2.PadLeft(14, '0');
                            dAccounts += Convert.ToInt64(temp2);
                            TempAccount = temp2.PadLeft(14, '0');
                        }
                        else
                        {
                            temp += temp2.Substring(0, 14);
                            dAccounts += Convert.ToInt64(temp2.Substring(0, 14));
                            TempAccount = temp2.Substring(0, 14);
                        }


                        errTemp = "Error in transaction code and user code";
                        temp += "801110";

                        errTemp = "Error in DRCR Amount";
                        if (ds.Tables[0].Rows[i]["Tpy_NetAmt"].ToString().Trim().Replace(",", "").Replace(" ", "").Replace(".", "").Length < 15)
                        {
                            temp2 = string.Empty;
                            temp2 = ds.Tables[0].Rows[i]["Tpy_NetAmt"].ToString().Trim().Replace(",", "").Replace(" ", "").Replace(".", "");
                            int len = ds.Tables[0].Rows[i]["Tpy_NetAmt"].ToString().Trim().Replace(",", "").Replace(" ", "").Replace(".", "").Length;
                            for (int idx = 0; idx < (15 - len); idx++)
                            {
                                temp2 = "0" + temp2;
                            }
                            temp += temp2;
                        }
                        else
                        {
                            temp += ds.Tables[0].Rows[i]["Tpy_NetAmt"].ToString().Trim().Replace(",", "").Replace(" ", "").Replace(".", "").Substring(0, 15);
                        }

                        string strsd = ds.Tables[0].Rows[i]["Tpy_NetAmt"].ToString().Trim();
                        strsd = strsd.Substring(0, strsd.Length - 3);
                        dNetpays += Convert.ToInt64(strsd);

                        Int64 netpay = Convert.ToInt64(strsd);

                        strsd = TempAccount.Substring(8, 6);
                        dHash += (Convert.ToInt64(strsd) * netpay);

                        errTemp = "Error in Transaction date";
                        temp += dateGenerate.ToString("MMdd") + dateGenerate.Year.ToString().Substring(2, 2);

                        errTemp = "Error in backdate";
                        temp += "0";

                        errTemp = "Error in Filler1";
                        temp += "          ";
                               //1234567890 

                        errTemp = "Error in Filler2";
                        temp += "            ";
                               //123456789012 

                        errTemp = "Error in Filler3";
                        temp += "  ";
                               //1234567890

                        errTemp = "Error in Filler4";
                        temp += " ";
                               //123457  

                        errTemp = "Error in batchnumber";
                        temp += "00001";

                        errTemp = "Error in Filler5";
                        temp += "          ";
                               //1234567890

                        errTemp = "Error in branch code";
                        if (dsComp.Tables[0].Rows[0]["Mcm_BankBranchCode"].ToString().Trim().Length == 3)
                            temp += dsComp.Tables[0].Rows[0]["Mcm_BankBranchCode"].ToString().Trim();
                        else
                            temp += "001";

                        errTemp = "Error in Filler6";
                        temp += "    ";
                               //1234567

                        text += temp + "\r\n";
                        ++DetailCounter;
                    }
                    catch
                    {
                        DataRow dr = DSEXEMPTIONS.Tables[0].NewRow();
                        dr["Lastname"] = ds.Tables[0].Rows[i]["Mem_LastName"].ToString();
                        dr["Firstname"] = ds.Tables[0].Rows[i]["Mem_FirstName"].ToString();
                        dr["Remarks"] = errTemp;
                        DSEXEMPTIONS.Tables[0].Rows.Add(dr);
                    }

                }
            }
            #endregion

            #region Trail

            string temp3 = string.Empty;
            temp3 += "H";

            temp3 += string.Format("{0:00000000000000000000}", dAccounts);

            temp3 += string.Format("{0:00000000000000000000}", dNetpays);

            temp3 += string.Format("{0:00000000000000000000}", dHash);

            text += temp3;

            #endregion

            return text;

        }

        //OK
        private string BPITextFile(DataSet ds, DataSet dsExemp)
        {
            string text = string.Empty;
            int DetailCounter = 0;
            Error = string.Empty;
            string batchNumber = string.Empty;
            Int64 totalbankAccount = 0;
            double totalHorizontalhash = 0;
            double totalNetPay = 0;

            DataSet dsComp = GetCompanyInfo();

            #region header

            string header = string.Empty;
            string errtemp1 = string.Empty;
            try
            {
                header = "H";

                errtemp1 = "Error in Company Code";
                header += dsComp.Tables[0].Rows[0]["Mcm_ATMBankCode"].ToString().Trim().Substring(0, 5);

                errtemp1 = "Error in payroll date";
                header += dateGenerate.ToString("MMdd") + dateGenerate.Year.ToString().Substring(2, 2);

                errtemp1 = "Error in batch number";
                using (DALHelper dal = new DALHelper())
                {
                    dal.OpenDB();
                    int batchNum = Convert.ToInt32(dal.ExecuteScalar(@"
                         select COUNT(*) from T_PaySchedule
                            where Tps_PayCycle <= '" + MonthYear + @"'
                            and Tps_CycleIndicator in ('P', 'C')       
                                        ", CommandType.Text));
                    if (batchNum.ToString().Length < 2)
                        batchNumber = "0" + batchNum.ToString();
                    else
                        batchNumber = batchNum.ToString();
                    header += batchNumber;
                    dal.CloseDB();
                }

                errtemp1 = "Error in record type";
                header += "1";

                errtemp1 = "Error in Company account number";
                header += dsComp.Tables[0].Rows[0]["Mcm_BankAccountNo"].ToString().Trim().Replace("-", "").Replace(" ", "").Substring(0, 10);

                errtemp1 = "Error in Branch code";
                header += dsComp.Tables[0].Rows[0]["Mcm_BankBranchCode"].ToString().Trim().Substring(0, 3);

                errtemp1 = "Error in getting highest netpay amount";
                string temp2 = getHighestNetPay(ds);
                if (temp2.Length < 12)
                {
                    int len = temp2.Length;
                    for (int idx = 0; idx < (12 - len); idx++)
                    {
                        temp2 = "0" + temp2;
                    }
                    header += temp2.Trim().Substring(0, 12);
                }
                else
                {
                    header += temp2.Trim().Substring(0, 12);
                }
                temp2 = string.Empty;

                errtemp1 = "Error in getting total netpay amount";
                temp2 = getTotalNetpay(ds);
                if (temp2.Length < 12)
                {
                    int len = temp2.Length;
                    for (int idx = 0; idx < (12 - len); idx++)
                    {
                        temp2 = "0" + temp2;
                    }
                    header += temp2.Trim().Substring(0, 12);
                }
                else
                {
                    header += temp2.Trim().Substring(0, 12);
                }

                errtemp1 = "Error in BPI Identifier";
                header += "1";

                errtemp1 = "Error in header filler";
                         //         1         2         3         4         5         6         7     
                         //12345678901234567890123456789012345678901234567890123456789012345678901234567890
                header += "                                                                           ";

                text += header;
                text += "\r\n";
            }
            catch
            {
                DataRow dr = DSEXEMPTIONS.Tables[0].NewRow();
                dr["Lastname"] = "BPI Header";
                dr["Firstname"] = "Header";
                dr["Remarks"] = errtemp1;
                DSEXEMPTIONS.Tables[0].Rows.Add(dr);
            }

            #endregion

            #region details
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                string temp = string.Empty;
                int j = 0;
                for (; j < dsExemp.Tables[0].Rows.Count
                        && (
                        dsExemp.Tables[0].Rows[j]["Lastname"].ToString() != ds.Tables[0].Rows[i]["Mem_LastName"].ToString()
                        ||
                        dsExemp.Tables[0].Rows[j]["Firstname"].ToString() != ds.Tables[0].Rows[i]["Mem_FirstName"].ToString()
                        )
                    ; j++) ;
                if (j == dsExemp.Tables[0].Rows.Count)
                {
                    string errTemp = string.Empty;
                    try
                    {
                        errTemp = "Error in Record ID";
                        temp = "D";

                        errTemp = "Error in Company Code detail";
                        temp += dsComp.Tables[0].Rows[0]["Mcm_ATMBankCode"].ToString().Trim().Substring(0, 5);

                        errTemp = "Error in detail payroll date";
                        temp += dateGenerate.ToString("MMdd") + dateGenerate.Year.ToString().Substring(2, 2);

                        errTemp = "Error in batch number detail";
                        temp += batchNumber;

                        errTemp = "Error in Fixed number detail";
                        temp += "3";

                        errTemp = "Error in BankAccountnumber";
                        string temp2 = string.Empty;
                        if (ds.Tables[0].Rows[i]["Tpy_BankAcctNo"].ToString().Trim().Replace(" ", "").Replace("-", "").Length < 10)
                        {
                            throw new Exception();
                        }
                        else
                        {
                            if (ds.Tables[0].Rows[i]["Tpy_PaymentMode"].ToString().Trim() == "B")
                            {
                                temp2 = ds.Tables[0].Rows[i]["Tpy_BankAcctNo"].ToString().Trim().Replace(" ", "").Replace("-", "").Substring(0, 10);
                                string temp3 = string.Empty;

                                temp += temp2.Trim().Substring(0, 3);
                                temp3 = temp2.Trim().Substring(0, 3);
                                if (temp2.Trim().Substring(3, 1) == "5")       //Still for clarrification
                                {
                                    temp += "6";
                                    temp3 += "6";
                                }
                                else
                                {
                                    temp += temp2.Trim().Substring(3, 1);
                                    temp3 += temp2.Trim().Substring(3, 1);
                                }
                                temp += temp2.Trim().Substring(4, 6);
                                temp3 += temp2.Trim().Substring(4, 6);

                                totalbankAccount += Convert.ToInt64(temp3);
                            }
                            else
                            {
                                temp2 = ds.Tables[0].Rows[i]["Tpy_BankAcctNo"].ToString().Trim().Replace(" ", "").Replace("-", "").Substring(0, 10);
                                temp += temp2;
                                totalbankAccount += Convert.ToInt64(temp2);
                            }
                        }
                        temp2 = string.Empty;

                        errTemp = "Error in Netpay detail";
                        temp2 = ds.Tables[0].Rows[i]["Tpy_NetAmt"].ToString().Trim().Replace(".", "").Replace(" ", "");
                        if (temp2.Length < 12)
                        {
                            int len = temp2.Length;
                            for (int idx = 0; idx < (12 - len); idx++)
                                temp += "0";
                            temp += temp2;
                        }
                        else
                        {
                            temp += ds.Tables[0].Rows[i]["Tpy_NetAmt"].ToString().Trim().Replace(".", "").Replace(" ", "").Substring(0, 12);
                        }
                        totalNetPay += Convert.ToDouble(ds.Tables[0].Rows[i]["Tpy_NetAmt"].ToString().Trim());

                        errTemp = "Error in Horizontal hash";
                        
                        double hash = 0;

                        string Baccount = ds.Tables[0].Rows[i]["Tpy_BankAcctNo"].ToString().Trim().Replace(" ", "").Replace("-", "");
                        string Netp = ds.Tables[0].Rows[i]["Tpy_NetAmt"].ToString();
                        double d1 = 0;
                        double d2 = 0;
                        d1 = Convert.ToDouble(Baccount.Substring(4, 2));
                        d2 = Convert.ToDouble(Netp);
                        hash = d1 * d2;

                        d1 =  Convert.ToDouble(Baccount.Substring(6, 2));
                        hash = hash + (d1 * d2);
                       
                        d1 = Convert.ToDouble(Baccount.Substring(8, 2));
                        hash = hash + (d1 * d2);

                        totalHorizontalhash += hash;

                        temp2 = string.Format("{0:0.00}", hash).Replace(".", "");
                        if (temp2.Length < 12)
                        {
                            int len = temp2.Length;
                            for (int idx = 0; idx < (12 - len); idx++)
                            {
                                temp2 = "0" + temp2;
                            }
                            temp += temp2.ToString().Trim().Substring(0, 12);
                        }
                        else
                        {
                            temp += temp2.ToString().Trim().Substring(0, 12);
                        }

                               //         1         2         3         4         5         6         7         8
                               //12345678901234567890123456789012345678901234567890123456789012345678901234567890
                        temp += "                                                                               ";


                        text += temp + "\r\n";
                        ++DetailCounter;
                    }
                    catch
                    {
                        DataRow dr = DSEXEMPTIONS.Tables[0].NewRow();
                        dr["Lastname"] = ds.Tables[0].Rows[i]["Mem_LastName"].ToString();
                        dr["Firstname"] = ds.Tables[0].Rows[i]["Mem_FirstName"].ToString();
                        dr["Remarks"] = errTemp;
                        DSEXEMPTIONS.Tables[0].Rows.Add(dr);
                    }

                }
            }

            #endregion

            #region trailer

            string Trail = string.Empty;
            string errTrail = string.Empty;

            try
            {
                errTrail = "Error in record id trail";
                Trail = "T";

                errTrail = "Error in Company code Trail";
                Trail += dsComp.Tables[0].Rows[0]["Mcm_ATMBankCode"].ToString().Trim().Substring(0, 5);

                errTrail = "Error in payroll date Trail";
                Trail += dateGenerate.ToString("MMdd") + dateGenerate.Year.ToString().Substring(2, 2);

                errTrail = "Error in batch number trail";
                Trail += batchNumber;

                errTrail = "Error in record type trail";
                Trail += "2";

                errTrail = "Error in company account number trail";
                Trail += dsComp.Tables[0].Rows[0]["Mcm_BankAccountNo"].ToString().Trim().Replace("-", "").Replace(" ", "").Substring(0, 10);

                errTrail = "Error in acount number hash trail";
                string tempTotalbankAccount = string.Format("{0:0}", totalbankAccount);
                tempTotalbankAccount = tempTotalbankAccount.Trim().Replace(" ", "").Replace(",", "").Replace(".", "");
                if (tempTotalbankAccount.Length < 15)
                {
                    int len = tempTotalbankAccount.Length;
                    for (int idx = 0; idx < (15 - len); idx++)
                        Trail += "0";
                    Trail += tempTotalbankAccount;
                }
                else
                {
                    Trail += tempTotalbankAccount.Substring(0, 15);
                }
                
                errTrail = "Error in transaction amount hash trail";
                string tempTotalnetpay = string.Format("{0:0.00}", totalNetPay);
                tempTotalnetpay = tempTotalnetpay.Trim().Replace(" ", "").Replace(",", "").Replace(".", "");
                if (tempTotalnetpay.Length < 15)
                {
                    int len = tempTotalnetpay.Length;
                    for (int idx = 0; idx < (15 - len); idx++)
                        Trail += "0";
                    Trail += tempTotalnetpay;
                }
                else
                {
                    Trail += tempTotalnetpay.Substring(0, 15);
                }

                errTrail = "Error in Horizontal hash total";
                string tempTotalHorizontalHash = string.Format("{0:0.00}", totalHorizontalhash);
                tempTotalHorizontalHash = tempTotalHorizontalHash.Trim().Replace(" ", "").Replace(",", "").Replace(".", "");
                if (tempTotalHorizontalHash.Length < 18)
                {
                    int len = tempTotalHorizontalHash.Length;
                    for (int idx = 0; idx < (18 - len); idx++)
                        Trail += "0";
                    Trail += tempTotalHorizontalHash;
                }
                else
                {
                    Trail += tempTotalHorizontalHash.Substring(0, 18);
                }

                errTrail = "Error in detail count trail";
                if (DetailCounter.ToString().Trim().Length < 5)
                {
                    int len = DetailCounter.ToString().Trim().Length;
                    for (int idx = 0; idx < (5 - len); idx++)
                        Trail += "0";
                    Trail += DetailCounter.ToString().Trim();
                }
                else
                {
                    Trail += DetailCounter.ToString().Trim().Substring(0, 5);
                }

                errTrail = "Error in Filler trail";
                        //         1         2         3         4         5         6         7     
                        //12345678901234567890123456789012345678901234567890123456789012345678901234567890
                Trail += "                                                  ";

                text += Trail;

            }
            catch
            {
                DataRow dr = DSEXEMPTIONS.Tables[0].NewRow();
                dr["Lastname"] = "BPI Trail";
                dr["Firstname"] = "Trail";
                dr["Remarks"] = errTrail;
                DSEXEMPTIONS.Tables[0].Rows.Add(dr);
                //Error += "\r\n";
                //Error += "Error in Trailer ";
                //Error += errTrail;
            }


            #endregion

            return text;

        }

        public DataSet GetBankDebit(string query, DataSet dsExemp)
        {
            DataSet dsret = null;
            DataTable dt = new DataTable();
            dt.Columns.Add("EmpAccountNo", typeof(double));
            dt.Columns.Add("TransactionAmount", typeof(double));
            dt.Columns.Add("HorizontalHash", typeof(double));
            dt.Columns.Add("Mem_LastName");
            dt.Columns.Add("Mem_FirstName");
            dt.Columns.Add("Tpy_NetAmt", typeof(double));
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    DataSet ds = dal.ExecuteDataSet(query, CommandType.Text);
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        string temp = string.Empty;
                        int j = 0;
                        for (; j < dsExemp.Tables[0].Rows.Count
                                && (
                                dsExemp.Tables[0].Rows[j]["Lastname"].ToString() != ds.Tables[0].Rows[i]["Mem_LastName"].ToString()
                                ||
                                dsExemp.Tables[0].Rows[j]["Firstname"].ToString() != ds.Tables[0].Rows[i]["Mem_FirstName"].ToString()
                                )
                            ; j++) ;
                        if (j == dsExemp.Tables[0].Rows.Count)
                        {
                            #region
                            string errTemp = string.Empty;
                            try
                            {
                                DataRow dr = dt.NewRow();
                                dr["Mem_LastName"] = ds.Tables[0].Rows[i]["Mem_LastName"].ToString();
                                dr["Mem_FirstName"] = ds.Tables[0].Rows[i]["Mem_FirstName"].ToString();
                                dr["Tpy_NetAmt"] = ds.Tables[0].Rows[i]["Tpy_NetAmt"];
                                errTemp = "Error in BankAccountnumber";
                                string temp2 = string.Empty;
                                if (ds.Tables[0].Rows[i]["Tpy_BankAcctNo"].ToString().Trim().Replace(" ", "").Replace("-", "").Length < 10)
                                {
                                    throw new Exception();
                                }
                                else
                                {
                                    string temp3 = string.Empty;
                                    if (ds.Tables[0].Rows[i]["Tpy_PaymentMode"].ToString().Trim() == "B")
                                    {
                                        temp2 = ds.Tables[0].Rows[i]["Tpy_BankAcctNo"].ToString().Trim().Replace(" ", "").Replace("-", "").Substring(0, 10);


                                        temp += temp2.Trim().Substring(0, 3);
                                        temp3 = temp2.Trim().Substring(0, 3);
                                        if (temp2.Trim().Substring(3, 1) == "5")
                                        {
                                            temp += "6";
                                            temp3 += "6";
                                        }
                                        else
                                        {
                                            temp += temp2.Trim().Substring(3, 1);
                                            temp3 += temp2.Trim().Substring(3, 1);
                                        }
                                        temp += temp2.Trim().Substring(4, 6);
                                        temp3 += temp2.Trim().Substring(4, 6);
                                    }
                                    else
                                    {
                                        temp2 = ds.Tables[0].Rows[i]["Tpy_BankAcctNo"].ToString().Trim().Replace(" ", "").Replace("-", "").Substring(0, 10);
                                        temp3 = temp2;
                                    }
                                    dr["EmpAccountNo"] = Convert.ToDouble(temp3);
                                }
                                temp2 = string.Empty;

                                errTemp = "Error in Netpay detail";
                                temp2 = ds.Tables[0].Rows[i]["Tpy_NetAmt"].ToString().Trim().Replace(".", "").Replace(" ", "");
                                dr["TransactionAmount"] = Convert.ToDouble(ds.Tables[0].Rows[i]["Tpy_NetAmt"].ToString().Trim());

                                errTemp = "Error in Horizontal hash";

                                double hash = 0;

                                string Baccount = ds.Tables[0].Rows[i]["Tpy_BankAcctNo"].ToString().Trim().Replace(" ", "").Replace("-", "");
                                string Netp = ds.Tables[0].Rows[i]["Tpy_NetAmt"].ToString();
                                double d1 = 0;
                                double d2 = 0;
                                d1 = Convert.ToDouble(Baccount.Substring(4, 2));
                                d2 = Convert.ToDouble(Netp);
                                hash = d1 * d2;

                                d1 = Convert.ToDouble(Baccount.Substring(6, 2));
                                hash = hash + (d1 * d2);

                                d1 = Convert.ToDouble(Baccount.Substring(8, 2));
                                hash = hash + (d1 * d2);

                                dr["HorizontalHash"] = hash;

                                dt.Rows.Add(dr);
                            }
                            catch
                            {
                                CommonProcedures.showMessageError("Error: "
                                            + "\n " + ds.Tables[0].Rows[i]["Mem_LastName"].ToString()
                                            + ", " + ds.Tables[0].Rows[i]["Mem_FirstName"].ToString()
                                            + "\n " + errTemp);
                                if (MessageBox.Show("Continue with process?", "", MessageBoxButtons.YesNo) == DialogResult.No)
                                    return null;
                            }
                            #endregion
                        }
                    }
                    dsret = new DataSet();
                    dsret.Tables.Add(dt);

                }
                catch (Exception er)
                {
                    CommonProcedures.showMessageError(er.Message);
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return dsret;
        }

        public DataSet GetBankDebit(string query, DataSet dsExemp, bool isBankDebitReport)
        {
            DataSet dsret = null;
            //DataSet dsComp = GetCompanyInfo();
            DataTable dt = new DataTable();
            dt.Columns.Add("Employee ID");
            dt.Columns.Add("Last Name");
            dt.Columns.Add("First Name");
            dt.Columns.Add("Middle Name");
            dt.Columns.Add("Original bank Acct");
            if (isBankDebitReport)
            {
                dt.Columns.Add("EmpAccountNo", typeof(double));
                dt.Columns.Add("TransactionAmount", typeof(double));
                dt.Columns.Add("HorizontalHash", typeof(double));
            }
            else
            {
                dt.Columns.Add("Hashed Acct No", typeof(double));
                dt.Columns.Add("Is Hashed");
                dt.Columns.Add("Transaction Amt", typeof(double));
                dt.Columns.Add("Horizontal Hash", typeof(double));
            }
            dt.Columns.Add("Net Pay Amount", typeof(double));
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    DataSet ds = dal.ExecuteDataSet(query, CommandType.Text);
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        string temp = string.Empty;
                        int j = 0;
                        for (; j < dsExemp.Tables[0].Rows.Count
                                && (
                                dsExemp.Tables[0].Rows[j]["Lastname"].ToString() != ds.Tables[0].Rows[i]["Mem_LastName"].ToString()
                                ||
                                dsExemp.Tables[0].Rows[j]["Firstname"].ToString() != ds.Tables[0].Rows[i]["Mem_FirstName"].ToString()
                                )
                            ; j++) ;
                        if (j == dsExemp.Tables[0].Rows.Count)
                        {
                            #region
                            string errTemp = string.Empty;
                            try
                            {
                                DataRow dr = dt.NewRow();

                                dr["Last Name"] = ds.Tables[0].Rows[i]["Mem_LastName"].ToString();
                                dr["First Name"] = ds.Tables[0].Rows[i]["Mem_FirstName"].ToString();
                                dr["Middle Name"] = ds.Tables[0].Rows[i]["Mem_FirstName"].ToString().Trim() == string.Empty
                                                    ? ""
                                                    : " " + ds.Tables[0].Rows[i]["Mem_FirstName"].ToString().Trim().Substring(0, 1) + ".";
                                dr["Net Pay Amount"] = ds.Tables[0].Rows[i]["Tpy_NetAmt"];
                                dr["Employee ID"] = GetEmployeeID(
                                    ds.Tables[0].Rows[i]["Mem_LastName"].ToString().Trim()
                                    , ds.Tables[0].Rows[i]["Mem_FirstName"].ToString().Trim()
                                    , dal);
                                dr["Original bank Acct"] = ds.Tables[0].Rows[i]["Tpy_BankAcctNo"].ToString().Trim().Replace(" ", "").Replace("-", "");

                                errTemp = "Error in BankAccountnumber";
                                string temp2 = string.Empty;
                                if (ds.Tables[0].Rows[i]["Tpy_BankAcctNo"].ToString().Trim().Replace(" ", "").Replace("-", "").Length < 10)
                                {
                                    if (isBankDebitReport)
                                    {
                                        throw new Exception();
                                    }
                                    else 
                                    {
                                        dr["Hashed Acct No"] = ds.Tables[0].Rows[i]["Tpy_BankAcctNo"].ToString().Trim();
                                    }
                                }
                                else
                                {
                                    string temp3 = string.Empty;
                                    if (ds.Tables[0].Rows[i]["Tpy_PaymentMode"].ToString().Trim() == "B")
                                    {
                                        temp2 = ds.Tables[0].Rows[i]["Tpy_BankAcctNo"].ToString().Trim().Replace(" ", "").Replace("-", "").Substring(0, 10);
                                        

                                        temp += temp2.Trim().Substring(0, 3);
                                        temp3 = temp2.Trim().Substring(0, 3);
                                        if (temp2.Trim().Substring(3, 1) == "5")
                                        {
                                            temp += "6";
                                            temp3 += "6";
                                        }
                                        else
                                        {
                                            temp += temp2.Trim().Substring(3, 1);
                                            temp3 += temp2.Trim().Substring(3, 1);
                                        }
                                        temp += temp2.Trim().Substring(4, 6);
                                        temp3 += temp2.Trim().Substring(4, 6);
                                    }
                                    else
                                    {
                                        temp2 = ds.Tables[0].Rows[i]["Tpy_BankAcctNo"].ToString().Trim().Replace(" ", "").Replace("-", "").Substring(0, 10);
                                        temp3 = temp2;
                                    }

                                    if (isBankDebitReport)
                                    {
                                        dr["EmpAccountNo"] = Convert.ToDouble(temp3);
                                    }
                                    else
                                    {
                                        dr["Hashed Acct No"] = Convert.ToDouble(temp3);
                                    }
                                }
                                temp2 = string.Empty;

                                errTemp = "Error in Netpay detail";
                                temp2 = ds.Tables[0].Rows[i]["Tpy_NetAmt"].ToString().Trim().Replace(".", "").Replace(" ", "");
                                //if (temp2.Length < 12)
                                //{
                                //    int len = temp2.Length;
                                //    for (int idx = 0; idx < (12 - len); idx++)
                                //        temp += "0";
                                //    temp += temp2;
                                //}
                                //else
                                //{
                                //    temp += ds.Tables[0].Rows[i]["Tpy_NetAmt"].ToString().Trim().Replace(".", "").Replace(" ", "").Substring(0, 12);
                                //}

                                if (isBankDebitReport)
                                {
                                    dr["TransactionAmount"] = Convert.ToDouble(ds.Tables[0].Rows[i]["Tpy_NetAmt"].ToString().Trim());
                                }
                                else
                                {
                                    dr["Transaction Amt"] = Convert.ToDouble(ds.Tables[0].Rows[i]["Tpy_NetAmt"].ToString().Trim());
                                }

                                errTemp = "Error in Horizontal hash";

                                double hash = 0;
                                try
                                {
                                    string Baccount = ds.Tables[0].Rows[i]["Tpy_BankAcctNo"].ToString().Trim().Replace(" ", "").Replace("-", "");
                                    string Netp = ds.Tables[0].Rows[i]["Tpy_NetAmt"].ToString();
                                    double d1 = 0;
                                    double d2 = 0;
                                    d1 = Convert.ToDouble(Baccount.Substring(4, 2));
                                    d2 = Convert.ToDouble(Netp);
                                    hash = d1 * d2;

                                    d1 = Convert.ToDouble(Baccount.Substring(6, 2));
                                    hash = hash + (d1 * d2);

                                    d1 = Convert.ToDouble(Baccount.Substring(8, 2));
                                    hash = hash + (d1 * d2);

                                    if (isBankDebitReport)
                                    {
                                        dr["HorizontalHash"] = hash;
                                    }
                                    else
                                    {
                                        dr["Horizontal Hash"] = hash;
                                    }
                                }
                                catch(Exception er)
                                {
                                    if (isBankDebitReport)
                                    {
                                        throw new Exception(er.Message);
                                    }
                                    else
                                    {
                                        dr["Horizontal Hash"] = 0;
                                    }
                                }

                                if (!isBankDebitReport)
                                {
                                    if (dr["Original bank Acct"].ToString().Trim() == dr["Hashed Acct No"].ToString().Trim())
                                    {
                                        dr["Is Hashed"] = "No";
                                    }
                                    else
                                    {
                                        dr["Is Hashed"] = "Yes";
                                    }
                                }

                                dt.Rows.Add(dr);
                            }
                            catch
                            {
                                CommonProcedures.showMessageError("Error: "
                                            + "\n " + ds.Tables[0].Rows[i]["Mem_LastName"].ToString() 
                                            + ", " + ds.Tables[0].Rows[i]["Mem_FirstName"].ToString()
                                            + "\n " + errTemp);
                                if (MessageBox.Show("Continue with process?", "", MessageBoxButtons.YesNo) == DialogResult.No)
                                    return null;
                            }
                            #endregion
                        }
                    }
                    dsret = new DataSet();
                    dsret.Tables.Add(dt);

                }
                catch(Exception er)
                {
                    CommonProcedures.showMessageError(er.Message);
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return dsret;
        }

        private string GetEmployeeID(string Lastname, string Firstname, DALHelper dal)
        {
            string str = string.Empty;
            DataSet ds = null;
            ds = dal.ExecuteDataSet(string.Format(@"
                select 
	                Mem_IDNo
                from M_Employee
                where Mem_LastName = '{0}'
                and Mem_FirstName = '{1}'
            ", Lastname, Firstname));

            if (ds != null
                && ds.Tables[0].Rows.Count > 0)
            {
                str = ds.Tables[0].Rows[0][0].ToString().Trim();
            }

            return str;
        }

        //OK
        private string BDOTextFile(DataSet ds, DataSet dsExemp)
        {
            string text = string.Empty;
            int DetailCounter = 0;
            Error = string.Empty;
            double totalNetPay = 0;

            DataSet dsComp = GetCompanyInfo();

            #region header

            string header = string.Empty;
            string errtemp1 = string.Empty;
            try
            {
                header = "H";

                errtemp1 = "Error in Time Format";
                header += "        01"; // + DateTime.Now.ToString("HHmmss");

                errtemp1 = "Error in Company Bank Account";
                header += dsComp.Tables[0].Rows[0]["Mcm_BankAccountNo"].ToString().Trim().Replace("-", "").Replace(" ", "").Substring(0, 10);

                errtemp1 = "Error in Company Institution code";
                string temp2 = dsComp.Tables[0].Rows[0]["Mcm_CompanyCode"].ToString().Trim().Replace("-", "").Replace(" ", "");
                if (temp2.Length < 10)
                {
                    for (int idx = 0; idx < (10 - temp2.Length); idx++)
                        header += " ";
                    header += temp2;
                }
                else
                {
                    header += temp2;
                }


                errtemp1 = "Error in Credit due date";
                header += dateGenerate.ToString("yyyyMMdd");

                text += header;
                text += "\r\n";
            }
            catch
            {
                DataRow dr = DSEXEMPTIONS.Tables[0].NewRow();
                dr["Lastname"] = "BDO Header";
                dr["Firstname"] = "Header";
                dr["Remarks"] = errtemp1;
                DSEXEMPTIONS.Tables[0].Rows.Add(dr);
            }

            #endregion

            #region details
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                string temp = string.Empty;
                int j = 0;
                for (; j < dsExemp.Tables[0].Rows.Count
                        && (
                        dsExemp.Tables[0].Rows[j]["Lastname"].ToString() != ds.Tables[0].Rows[i]["Mem_LastName"].ToString()
                        ||
                        dsExemp.Tables[0].Rows[j]["Firstname"].ToString() != ds.Tables[0].Rows[i]["Mem_FirstName"].ToString()
                        )
                    ; j++) ;
                if (j == dsExemp.Tables[0].Rows.Count)
                {
                    string errTemp = string.Empty;
                    try
                    {
                        
                        errTemp = "Error in BankAccountnumber";
                        string temp2 = string.Empty;
                        if (ds.Tables[0].Rows[i]["Tpy_BankAcctNo"].ToString().Trim().Replace(" ", "").Replace("-", "").Length < 10)
                        {
                            throw new Exception();
                        }
                        else
                        {
                            temp = ds.Tables[0].Rows[i]["Tpy_BankAcctNo"].ToString().Trim().Replace(" ", "").Replace("-", "").Substring(0, 10);
                            
                        }
                               //123456 
                        temp += "      ";

                        errTemp = "Error in Netpay detail";
                        temp  += ds.Tables[0].Rows[i]["Tpy_NetAmt"].ToString().Trim();

                        totalNetPay += Convert.ToDouble(ds.Tables[0].Rows[i]["Tpy_NetAmt"]);

                        text += temp + "\r\n";
                        ++DetailCounter;
                    }
                    catch
                    {
                        DataRow dr = DSEXEMPTIONS.Tables[0].NewRow();
                        dr["Lastname"] = ds.Tables[0].Rows[i]["Mem_LastName"].ToString();
                        dr["Firstname"] = ds.Tables[0].Rows[i]["Mem_FirstName"].ToString();
                        dr["Remarks"] = errTemp;
                        DSEXEMPTIONS.Tables[0].Rows.Add(dr);
                    }

                }
            }

            #endregion

            #region trailer

            string Trail = string.Empty;
            string errTrail = string.Empty;

            try
            {
                errTrail = "Error in record id trail";
                Trail = "T";

                errTrail = "Error in total records";
                string total = DetailCounter.ToString().Trim();

                for (int idx = 0; idx < (10 - total.Length); idx++)
                    Trail += " ";

                Trail += total;

                Trail += string.Format("{0:0.00}", totalNetPay);

                text += Trail;

            }
            catch
            {
                DataRow dr = DSEXEMPTIONS.Tables[0].NewRow();
                dr["Lastname"] = "BPI Trail";
                dr["Firstname"] = "Trail";
                dr["Remarks"] = errTrail;
                DSEXEMPTIONS.Tables[0].Rows.Add(dr);
                //Error += "\r\n";
                //Error += "Error in Trailer ";
                //Error += errTrail;
            }


            #endregion

            return text;

        }

        #endregion

        //OK
        #region SSS Loans
        
        //OK
        private string SSSLoanGov(DataSet ds)
        {
            string text = string.Empty;
            int DetailCounter = 0;
            Error = string.Empty;
            string batchNumber = string.Empty;
            double TotalPenalty = 0;
            double TotalAmort = 0;
            DataSet dsExemp = DSEXEMPTIONS;
            DataSet dsComp = GetCompanyInfo();

            #region header

            string header = string.Empty;
            string errtemp1 = string.Empty;
            try
            {                                                                     
                header = "00";

                errtemp1 = "Error in employer ID";
                string temp2 = dsComp.Tables[0].Rows[0]["Mcm_SSSNo"].ToString().Replace(" ", "").Replace("-", "");
                if (temp2.Length < 10)
                {
                    throw new Exception();
                }
                else
                {
                    header += temp2;
                }
                temp2 = string.Empty;

                errtemp1 = "Error in Employer name";
                temp2 = dsComp.Tables[0].Rows[0]["Mcm_CompanyName"].ToString().Trim().Replace("Ñ", "N").ToUpper().Replace(".", "").Replace(",", "");
                if (temp2.Length < 30)
                {
                    //int len = temp2.Length;
                    //header += temp2;
                    //for (int idx = 0; idx < (30 - len); idx++)
                    //    header += " ";
                    header += temp2.PadRight(30, ' ');
                    
                }
                else
                {
                    header += temp2.Substring(0, 30);
                }

                errtemp1 = "Error in applicable month";
                header += MonthYear.Substring(2,4);

                         //         1         2         3         4         5         6         7         8
                         //12345678901234567890123456789012345678901234567890123456789012345678901234567890
                header += "                           ";


                text += header;
                text += "\r\n";
            }
            catch
            {
                DataRow dr = DSEXEMPTIONS.Tables[0].NewRow();
                dr["Lastname"] = "SSS Loans Header";
                dr["Firstname"] = "Header";
                dr["Remarks"] = errtemp1;
                DSEXEMPTIONS.Tables[0].Rows.Add(dr);
                //Error += "\r\n";
                //Error += "Error in header ";
                //Error += errtemp1;
            }

            #endregion

            #region details
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                string temp = string.Empty;
                int j = 0;
                for (; j < dsExemp.Tables[0].Rows.Count
                        && (
                        dsExemp.Tables[0].Rows[j]["Lastname"].ToString() != ds.Tables[0].Rows[i]["Mem_LastName"].ToString()
                        ||
                        dsExemp.Tables[0].Rows[j]["Firstname"].ToString() != ds.Tables[0].Rows[i]["Mem_FirstName"].ToString()
                        )
                    ; j++) ;
                if (j == dsExemp.Tables[0].Rows.Count)
                {
                    string errTemp = string.Empty;
                    try
                    {
                        temp = "10";

                        errTemp = "Error in SSS Number";
                        if (ds.Tables[0].Rows[i]["Mem_SSSNo"].ToString().Trim().Replace(" ", "").Replace("-", "").Length < 10)
                        {
                            throw new Exception();
                        }
                        else
                        {
                            temp += ds.Tables[0].Rows[i]["Mem_SSSNo"].ToString().Trim().Replace(" ", "").Replace("-", "").Substring(0, 10);
                        }

                        string temp2 = string.Empty;
                        errTemp = "Error in Last name";
                        temp2 = ds.Tables[0].Rows[i]["Mem_LastName"].ToString().Trim().Replace("Ñ", "N").ToUpper().Replace(".","").Replace(",","");
                        if (temp2.Length < 15)
                        {
                            temp += temp2.PadRight(15, ' ');
                        }
                        else
                        {
                            temp += temp2.Substring(0, 15);
                        }
                        temp2 = string.Empty;

                        errTemp = "Error in First name";
                        temp2 = ds.Tables[0].Rows[i]["Mem_FirstName"].ToString().Trim().Replace("Ñ", "N").ToUpper().Replace(".", "").Replace(",", "");
                        if (temp2.Length < 15)
                        {
                            temp += temp2.PadRight(15, ' ');
                            //int len = temp2.Length;
                            //for (int idx = 0; idx < (15 - len); idx++)
                            //    temp += " ";
                        }
                        else
                        {
                            temp += temp2.Substring(0, 15);
                        }
                        temp2 = string.Empty;

                        errTemp = "Error in Middlename";
                        if (ds.Tables[0].Rows[i]["Mem_MiddleName"].ToString().Trim().Replace("Ñ", "N").Replace(".", "").Replace(",", "") != string.Empty)
                        {
                            temp += ds.Tables[0].Rows[i]["Mem_MiddleName"].ToString().Trim().ToUpper().Replace("Ñ", "N").Replace(".", "").Replace(",", "").Substring(0, 1) + " ";
                        }
                        else
                        {
                            temp += "  ";
                        }

                        errTemp = "Error in Loan Type";
                        temp += ds.Tables[0].Rows[i]["LoanType"].ToString().Trim();

                        errTemp = "Error in Loan Date";
                        if (ds.Tables[0].Rows[i]["Tgr_CheckDate"].ToString().Trim() != string.Empty)
                        {
                            temp2 = ds.Tables[0].Rows[i]["Tgr_CheckDate"].ToString().Trim().Replace("/", "");
                            temp += temp2.Substring(6, 2) + temp2.Substring(0, 2) + temp2.Substring(2, 2);
                        }
                        else
                        {
                            temp += "      ";
                        }

                        errTemp = "Error in Loan Amount";

                        if (ds.Tables[0].Rows[i]["Tgr_PrincipalAmount"].ToString().Trim() != string.Empty)
                        {
                            double d = Convert.ToDouble(ds.Tables[0].Rows[i]["Tgr_PrincipalAmount"]); //Convert.ToDouble(ds.Tables[0].Rows[i]["Tgr_LoanAmount"]);
                            temp2 = string.Format("{0:0}", d);
                            //for (int idx = 0; idx < (6 - temp2.Length); idx++)
                            //    temp += "0";
                            temp += temp2.PadLeft(6, '0');
                        }
                        else
                        {
                            temp += "000000";
                        }

                        errTemp = "Error in Penalty Amount";
                        temp += ds.Tables[0].Rows[i]["Penalty Paid"].ToString().Trim();

                        TotalPenalty += Convert.ToDouble(ds.Tables[0].Rows[i]["Penalty Paid"].ToString().Trim());

                        errTemp = "Error in Amort Amount";
                        temp2 = ds.Tables[0].Rows[i]["Tgr_LoanAmount"].ToString().Trim();
                        temp2 = temp2.Replace(".", "");
                        if (temp2.Length < 7)
                        {
                            //int len = temp2.Length;
                            //for (int idx = 0; idx < (7 - len); idx++)
                            //    temp += "0";
                            temp += temp2.PadLeft(7, '0');
                        }
                        else
                            temp += temp2.Substring(0,7);

                        if (temp2 != string.Empty)
                            TotalAmort += Convert.ToDouble(ds.Tables[0].Rows[i]["Tgr_LoanAmount"].ToString().Trim());


                        errTemp = "Error in Amort paid Sign";

                        if (ds.Tables[0].Rows[i]["Amort Paid Sign"].ToString().Trim() == string.Empty)
                            temp += " ";
                        else 
                            temp += ds.Tables[0].Rows[i]["Amort Paid Sign"].ToString();



                        errTemp = "Error in Remarks";
                        if (ds.Tables[0].Rows[i]["Tgr_StatusCode"].ToString().Trim() == string.Empty)
                        {
                            temp += " ";
                        }
                        else
                        {
                            temp += ds.Tables[0].Rows[i]["Tgr_StatusCode"].ToString().Trim();
                        }
                        text += temp + "\r\n";
                        ++DetailCounter;
                    }
                    catch
                    {
                        DataRow dr = DSEXEMPTIONS.Tables[0].NewRow();
                        dr["Lastname"] = ds.Tables[0].Rows[i]["Mem_LastName"].ToString();
                        dr["Firstname"] = ds.Tables[0].Rows[i]["Mem_FirstName"].ToString();
                        dr["Remarks"] = errTemp;
                        DSEXEMPTIONS.Tables[0].Rows.Add(dr);
                    }

                }
            }

            #endregion

            #region trailer

            string Trail = string.Empty;
            string errTrail = string.Empty;

            try
            {
                errTrail = "Error in record id trail";
                Trail = "99";

                errTrail = "Error in Total number of employees";
                string temp = DetailCounter.ToString();
                //for(int idx = 0; idx < (4 - temp.Length); idx++)
                //    Trail += "0";
                Trail += temp.PadLeft(4, '0');

                errTrail = "Error in Total Penalty paid";
                string temp2 = string.Format("{0:0.00}", TotalPenalty).Replace(".", "");
                if (temp2.Length < 9)
                {
                    //int len = temp2.Length;
                    //for (int idx = 0; idx < (9 - len); idx++)
                    //    Trail += "0";
                    Trail += temp2.PadLeft(9, '0');
                }
                else
                {
                    Trail += temp2.Substring(0, 9);
                }

                errTrail = "Error in Amort paid";
                temp2 = string.Format("{0:0.00}", TotalAmort).Replace(".", "");
                if (temp2.Length < 9)
                {
                    //int len = temp2.Length;
                    //for (int idx = 0; idx < (9 - len); idx++)
                    //    Trail += "0";
                    Trail += temp2.PadLeft(9, '0');
                }
                else
                {
                    Trail += temp2.Substring(0, 9);
                }


                errTrail = "Error in Filler trail";
                        //         1         2         3         4         5         6         7     
                        //12345678901234567890123456789012345678901234567890123456789012345678901234567890
                Trail += "                                                ";

                text += Trail;

            }
            catch
            {
                DataRow dr = DSEXEMPTIONS.Tables[0].NewRow();
                dr["Lastname"] = "SSS Loans Trail";
                dr["Firstname"] = "Trail";
                dr["Remarks"] = errTrail;
                DSEXEMPTIONS.Tables[0].Rows.Add(dr);
                //Error += "\r\n";
                //Error += "Error in Trailer ";
                //Error += errTrail;
            }


            #endregion
            
            return text;
        }

        //OK
        private string SSSLoanUnionBank(DataSet ds)
        {
            string text = string.Empty;
            int DetailCounter = 0;
            Error = string.Empty;
            string batchNumber = string.Empty;
            double TotalPenalty = 0;
            double TotalAmort = 0;
            DataSet dsExemp = DSEXEMPTIONS;
            DataSet dsComp = GetCompanyInfo();

            #region header

            string header = string.Empty;
            string errtemp1 = string.Empty;
            try
            {                                                                     
                header = "00";

                errtemp1 = "Error in employer ID";
                string temp2 = dsComp.Tables[0].Rows[0]["Mcm_SSSNo"].ToString().Replace(" ", "").Replace("-", "");
                if (temp2.Length < 10)
                {
                    throw new Exception();
                }
                else
                {
                    header += temp2;
                }
                temp2 = string.Empty;

                errtemp1 = "Error in Employer name";
                temp2 = dsComp.Tables[0].Rows[0]["Mcm_CompanyName"].ToString().Trim();
                if (temp2.Length < 30)
                {
                    //int len = temp2.Length;
                    //header += temp2;
                    //for (int idx = 0; idx < (30 - len); idx++)
                    //    header += " ";
                    header += temp2.PadRight(30, ' ');
                }
                else
                {
                    header += temp2.Substring(0, 30);
                }

                errtemp1 = "Error in applicable month";
                header += MonthYear.Substring(2,4);

                text += header;
                text += "\r\n";
            }
            catch
            {
                DataRow dr = DSEXEMPTIONS.Tables[0].NewRow();
                dr["Lastname"] = "SSS Loans Header";
                dr["Firstname"] = "Header";
                dr["Remarks"] = errtemp1;
                DSEXEMPTIONS.Tables[0].Rows.Add(dr);
                //Error += "\r\n";
                //Error += "Error in header ";
                //Error += errtemp1;
            }

            #endregion

            #region details
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                string temp = string.Empty;
                int j = 0;
                for (; j < dsExemp.Tables[0].Rows.Count
                        && (
                        dsExemp.Tables[0].Rows[j]["Lastname"].ToString() != ds.Tables[0].Rows[i]["Mem_LastName"].ToString()
                        ||
                        dsExemp.Tables[0].Rows[j]["Firstname"].ToString() != ds.Tables[0].Rows[i]["Mem_FirstName"].ToString()
                        )
                    ; j++) ;
                if (j == dsExemp.Tables[0].Rows.Count)
                {
                    string errTemp = string.Empty;
                    try
                    {
                        temp = "10";

                        errTemp = "Error in SSS Number";
                        if (ds.Tables[0].Rows[i]["Mem_SSSNo"].ToString().Trim().Replace(" ", "").Replace("-", "").Length < 10)
                        {
                            throw new Exception();
                        }
                        else
                        {
                            temp += ds.Tables[0].Rows[i]["Mem_SSSNo"].ToString().Trim().Replace(" ", "").Replace("-", "").Substring(0, 10);
                        }

                        string temp2 = string.Empty;
                        errTemp = "Error in Last name";
                        temp2 = ds.Tables[0].Rows[i]["Mem_LastName"].ToString().Replace("Ñ", "N").Trim().ToUpper();
                        if (temp2.Length < 15)
                        {
                            temp += temp2.PadRight(15, ' ');
                        }
                        else
                        {
                            temp += temp2.Substring(0, 15);
                        }
                        temp2 = string.Empty;

                        errTemp = "Error in First name";
                        temp2 = ds.Tables[0].Rows[i]["Mem_FirstName"].ToString().Replace("Ñ", "N").Trim().ToUpper();
                        if (temp2.Length < 15)
                        {
                            temp += temp2.PadRight(15, ' ');
                        }
                        else
                        {
                            temp += temp2.Substring(0, 15);
                        }
                        temp2 = string.Empty;

                        errTemp = "Error in Middlename";
                        if (ds.Tables[0].Rows[i]["Mem_MiddleName"].ToString().Trim() != string.Empty)
                        {
                            temp += ds.Tables[0].Rows[i]["Mem_MiddleName"].ToString().Trim().ToUpper().Substring(0, 1) + " ";
                        }
                        else
                        {
                            temp += "  ";
                        }

                        errTemp = "Error in Loan Type";
                        temp += ds.Tables[0].Rows[i]["LoanType"].ToString().Trim();


                        errTemp = "Error in Loan Date";
                        if (ds.Tables[0].Rows[i]["Tgr_CheckDate"].ToString().Trim() != string.Empty)
                        {
                            temp2 = ds.Tables[0].Rows[i]["Tgr_CheckDate"].ToString().Trim().Replace("/", "");
                            temp += temp2.Substring(6, 2) + temp2.Substring(0, 2) + temp2.Substring(2, 2);
                        }
                        else
                        {
                            temp += string.Empty;
                        }

                        errTemp = "Error in Loan Amount";
                        if (ds.Tables[0].Rows[i]["Tgr_LoanAmount"].ToString().Trim() != string.Empty)
                        {
                            double d = Convert.ToDouble(ds.Tables[0].Rows[i]["Tgr_LoanAmount"].ToString());
                            temp2 = string.Format("{0:0}", d);
                            if (temp2.Length < 6)
                            {
                                //for (int idx = 0; idx < (6 - temp2.Length); idx++)
                                //    temp += "0";
                                temp += temp2.PadLeft(6, '0');
                            }
                            else
                            {
                                temp += temp2;
                            }
                        }
                        else
                        {
                            temp += string.Empty;
                        }

                        errTemp = "Error in Penalty Amount";
                        temp2 = ds.Tables[0].Rows[i]["Penalty Paid"].ToString().Trim();
                        temp2 = temp2.Replace(".", "");
                        if (temp2.Length < 7)
                        {
                            //for (int idx = 0; idx < (7 - temp2.Length); idx++)
                            //    temp += "0";
                            temp += temp2.PadLeft(7, '0');
                        }
                        else
                        {
                            temp += temp2;
                        }

                        TotalPenalty += Convert.ToDouble(ds.Tables[0].Rows[i]["Penalty Paid"].ToString().Trim());

                        errTemp = "Error in Amort Amount";
                        temp2 = ds.Tables[0].Rows[i]["Prm_PrincipalAmount"].ToString().Trim();
                        if (temp2.Replace(".", "").Length < 7)
                        {
                            //int len = temp2.Replace(".", "").Length;
                            //for (int idx = 0; idx < (7 - len); idx++)
                            //    temp += "0";
                            temp += temp2.Replace(".", "").PadLeft(7, '0');
                        }
                        else
                            temp += temp2.Replace(".", "").Substring(0,7);

                        TotalAmort += Convert.ToDouble(temp2);


                        errTemp = "Error in Amort paid Sign";
                        temp += ds.Tables[0].Rows[i]["Amort Paid Sign"].ToString().Trim();

                        errTemp = "Error in Remarks";

                        if (ds.Tables[0].Rows[i]["Prm_StatusCode"].ToString().Trim() == string.Empty)
                        {
                            temp += " ";
                        }
                        else
                        {
                            temp += ds.Tables[0].Rows[i]["Prm_StatusCode"].ToString().Trim();
                        }
                        text += temp + "\r\n";
                        ++DetailCounter;
                    }
                    catch
                    {
                        DataRow dr = DSEXEMPTIONS.Tables[0].NewRow();
                        dr["Lastname"] = ds.Tables[0].Rows[i]["Mem_LastName"].ToString();
                        dr["Firstname"] = ds.Tables[0].Rows[i]["Mem_FirstName"].ToString();
                        dr["Remarks"] = errTemp;
                        DSEXEMPTIONS.Tables[0].Rows.Add(dr);
                    }

                }
            }

            #endregion

            #region trailer

            string Trail = string.Empty;
            string errTrail = string.Empty;

            try
            {
                errTrail = "Error in record id trail";
                Trail = "99";

                errTrail = "Error in Total number of employees";
                string temp = DetailCounter.ToString();
                for(int idx = 0; idx < (4 - temp.Length); idx++)
                    Trail += "0";
                Trail += temp;

                errTrail = "Error in Total Penalty paid";
                string temp2 = string.Format("{0:0.00}", TotalPenalty).Replace(".", "");
                if (temp2.Length < 9)
                {
                    //int len = temp2.Length;
                    //for (int idx = 0; idx < (9 - len); idx++)
                    //    Trail += "0";
                    Trail += temp2.PadLeft(9, '0');
                }
                else
                {
                    Trail += temp2.Substring(0, 9);
                }

                errTrail = "Error in Amort paid";
                temp2 = string.Format("{0:0.00}", TotalAmort).Replace(".", "");
                if (temp2.Length < 9)
                {
                    //int len = temp2.Length;
                    //for (int idx = 0; idx < (9 - len); idx++)
                    //    Trail += "0";
                    Trail += temp2.PadLeft(9, '0');
                }
                else
                {
                    Trail += temp2.Substring(0, 9);
                }

                text += Trail;

            }
            catch
            {
                DataRow dr = DSEXEMPTIONS.Tables[0].NewRow();
                dr["Lastname"] = "SSS Loans Trail";
                dr["Firstname"] = "Trail";
                dr["Remarks"] = errTrail;
                DSEXEMPTIONS.Tables[0].Rows.Add(dr);
                //Error += "\r\n";
                //Error += "Error in Trailer ";
                //Error += errTrail;
            }


            #endregion
            
            return text;
        }

        //OK
        private string SSSLoanMetroBank(DataSet ds)
        {
            string text = string.Empty;
            int DetailCounter = 0;
            Error = string.Empty;
            string batchNumber = string.Empty;
            double TotalPenalty = 0;
            double TotalAmort = 0;
            DataSet dsExemp = DSEXEMPTIONS;
            DataSet dsComp = GetCompanyInfo();

            #region header

            string header = string.Empty;
            string errtemp1 = string.Empty;
            try
            {
                header = "00";

                errtemp1 = "Error in employer ID";
                string temp2 = dsComp.Tables[0].Rows[0]["Mcm_SSSNo"].ToString().Replace(" ", "").Replace("-", "");
                if (temp2.Length < 10)
                {
                    throw new Exception();
                }
                else
                {
                    header += temp2;
                }
                temp2 = string.Empty;

                errtemp1 = "Error in Employer name";
                temp2 = dsComp.Tables[0].Rows[0]["Mcm_CompanyName"].ToString().Trim();
                if (temp2.Length < 30)
                {
                    int len = temp2.Length;
                    header += temp2;
                    for (int idx = 0; idx < (30 - len); idx++)
                        header += " ";

                }
                else
                {
                    header += temp2.Substring(0, 30);
                }

                errtemp1 = "Error in applicable month";
                header += MonthYear.Substring(2, 4);

                errtemp1 = "Error in Branch Code";
                header += dsComp.Tables[0].Rows[0]["Mcm_BankBranchCode"].ToString().Trim().Substring(0, 2);

                errtemp1 = "Error in SBR";
                string query = string.Format(
                    @"
                        Select * from T_MonthlySBR
                        Where Tms_DeductionCode = 'SSSLOAN'
                        And Tms_YearMonth = '{0}'
                    ", MonthYear);
                using (DALHelper dal = new DALHelper())
                {
                    try
                    {
                        dal.OpenDB();
                        DataSet dsSBR = dal.ExecuteDataSet(query, CommandType.Text);

                        temp2 = dsSBR.Tables[0].Rows[0]["Tms_ReceiptNo"].ToString().Trim().Replace(" ", "").Replace("-", "").Replace(".", "");
                        if (temp2.Length < 8)
                        {
                            int len = temp2.Length;
                            for (int idx = 0; idx < (8 - len); idx++)
                                header += "0";
                            header += temp2;
                        }
                        else
                        {
                            header += temp2.Substring(0, 8);
                        }

                        temp2 = dsSBR.Tables[0].Rows[0]["Tms_PaymentDate"].ToString().Trim();
                        header += temp2.Replace("/", "").Substring(4, 4) + temp2.Replace("/", "").Substring(0, 2) + temp2.Replace("/", "").Substring(2, 2);
                                               
                    }
                    catch
                    {
                        throw new Exception();
                    }
                    finally
                    {
                        dal.CloseDB();
                    }
                }

                text += header;
                text += "\r\n";
            }
            catch
            {
                DataRow dr = DSEXEMPTIONS.Tables[0].NewRow();
                dr["Lastname"] = "SSS Loan Header";
                dr["Firstname"] = "Header";
                dr["Remarks"] = errtemp1;
                DSEXEMPTIONS.Tables[0].Rows.Add(dr);
                //Error += "\r\n";
                //Error += "Error in header ";
                //Error += errtemp1;
            }

            #endregion

            #region details
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                string temp = string.Empty;
                int j = 0;
                for (; j < dsExemp.Tables[0].Rows.Count
                        && (
                        dsExemp.Tables[0].Rows[j]["Lastname"].ToString() != ds.Tables[0].Rows[i]["Mem_LastName"].ToString()
                        ||
                        dsExemp.Tables[0].Rows[j]["Firstname"].ToString() != ds.Tables[0].Rows[i]["Mem_FirstName"].ToString()
                        )
                    ; j++) ;
                if (j == dsExemp.Tables[0].Rows.Count)
                {
                    string errTemp = string.Empty;
                    try
                    {
                        temp = "10";

                        errTemp = "Error in SSS Number";
                        if (ds.Tables[0].Rows[i]["Mem_SSSNo"].ToString().Trim().Replace(" ", "").Replace("-", "").Length < 10)
                        {
                            throw new Exception();
                        }
                        else
                        {
                            temp += ds.Tables[0].Rows[i]["Mem_SSSNo"].ToString().Trim().Replace(" ", "").Replace("-", "").Substring(0, 10);
                        }

                        string temp2 = string.Empty;
                        errTemp = "Error in Last name";
                        temp2 = ds.Tables[0].Rows[i]["Mem_LastName"].ToString().Replace("Ñ", "N").Trim().ToUpper();
                        if (temp2.Length < 15)
                        {
                            temp += temp2;
                            int len = temp2.Length;
                            for (int idx = 0; idx < (15 - len); idx++)
                                temp += " ";
                        }
                        else
                        {
                            temp += temp2.Substring(0, 15);
                        }
                        temp2 = string.Empty;

                        errTemp = "Error in First name";
                        temp2 = ds.Tables[0].Rows[i]["Mem_FirstName"].ToString().Replace("Ñ", "N").Trim().ToUpper();
                        if (temp2.Length < 15)
                        {
                            temp += temp2;
                            int len = temp2.Length;
                            for (int idx = 0; idx < (15 - len); idx++)
                                temp += " ";
                        }
                        else
                        {
                            temp += temp2.Substring(0, 15);
                        }
                        temp2 = string.Empty;

                        errTemp = "Error in Middlename";
                        if (ds.Tables[0].Rows[i]["Mem_MiddleName"].ToString().Trim() != string.Empty)
                        {
                            temp += ds.Tables[0].Rows[i]["Mem_MiddleName"].ToString().Trim().ToUpper().Substring(0, 1) + " ";
                        }
                        else
                        {
                            temp += "  ";
                        }

                        errTemp = "Error in Loan Type";
                        temp += ds.Tables[0].Rows[i]["LoanType"].ToString().Trim();

                        errTemp = "Error in Loan Date";
                        if (ds.Tables[0].Rows[i]["Prm_CheckDate"].ToString().Trim() != string.Empty)
                        {
                            temp2 = ds.Tables[0].Rows[i]["Prm_CheckDate"].ToString().Trim().Replace("/", "");
                            temp += temp2.Substring(6, 2) + temp2.Substring(0, 2) + temp2.Substring(2, 2);
                        }
                        else
                        {
                            temp += "      ";
                        }

                        errTemp = "Error in Loan Amount";
                        if (ds.Tables[0].Rows[i]["Prm_LoanAmount"].ToString().Trim() != string.Empty)
                        {
                            double d = Convert.ToDouble(ds.Tables[0].Rows[i]["Prm_LoanAmount"].ToString());
                            temp2 = string.Format("{0:0}", d);
                            if (temp2.Length < 6)
                            {
                                for (int idx = 0; idx < (6 - temp2.Length); idx++)
                                    temp += "0";
                                temp += temp2;
                            }
                            else
                            {
                                temp += temp2;
                            }
                        }
                        else
                        {
                            temp += string.Empty;
                        }

                        errTemp = "Error in Penalty Amount";
                        temp2 = ds.Tables[0].Rows[i]["Penalty Paid"].ToString().Trim();
                        if (temp2.Replace(".", "").Length < 7)
                        {
                            for (int idx = 0; idx < (7 - temp2.Replace(".", "").Length); idx++)
                                temp += "0";
                            temp += temp2.Replace(".", "");
                        }
                        else
                        {
                            temp += temp2.Replace(".", "");
                        }

                        TotalPenalty += Convert.ToDouble(ds.Tables[0].Rows[i]["Penalty Paid"].ToString().Trim());

                        errTemp = "Error in Amort Amount";
                        temp2 = ds.Tables[0].Rows[i]["Tgr_PrincipalAmount"].ToString().Trim();
                        if (temp2.Replace(".", "").Length < 7)
                        {
                            int len = temp2.Replace(".", "").Length;
                            for (int idx = 0; idx < (7 - len); idx++)
                                temp += "0";
                            temp += temp2.Replace(".", "");
                        }
                        else
                            temp += temp2.Replace(".", "");

                        TotalAmort += Convert.ToDouble(temp2);

                        text += temp + "\r\n";
                        ++DetailCounter;
                    }
                    catch
                    {
                        DataRow dr = DSEXEMPTIONS.Tables[0].NewRow();
                        dr["Lastname"] = ds.Tables[0].Rows[i]["Mem_LastName"].ToString();
                        dr["Firstname"] = ds.Tables[0].Rows[i]["Mem_FirstName"].ToString();
                        dr["Remarks"] = errTemp;
                        DSEXEMPTIONS.Tables[0].Rows.Add(dr);
                    }

                }
            }

            #endregion

            #region trailer

            string Trail = string.Empty;
            string errTrail = string.Empty;

            try
            {
                errTrail = "Error in record id trail";
                Trail = "99";

                errTrail = "Error in Total number of employees";
                string temp = DetailCounter.ToString();
                for (int idx = 0; idx < (6 - temp.Length); idx++)
                    Trail += "0";
                Trail += temp;

                errTrail = "Error in Total Penalty paid";
                string temp2 = string.Format("{0:0.00}", TotalPenalty).Replace(".", "");
                if (temp2.Length < 10)
                {
                    int len = temp2.Length;
                    for (int idx = 0; idx < (10 - len); idx++)
                        Trail += "0";
                    Trail += temp2;
                }
                else
                {
                    Trail += temp2.Substring(0, 10);
                }

                errTrail = "Error in Amort paid";
                temp2 = string.Format("{0:0.00}", TotalAmort).Replace(".", "");
                if (temp2.Length < 10)
                {
                    int len = temp2.Length;
                    for (int idx = 0; idx < (10 - len); idx++)
                        Trail += "0";
                    Trail += temp2;
                }
                else
                {
                    Trail += temp2.Substring(0, 10);
                }

                text += Trail;

            }
            catch
            {
                DataRow dr = DSEXEMPTIONS.Tables[0].NewRow();
                dr["Lastname"] = "SSS Loans Trail";
                dr["Firstname"] = "Trail";
                dr["Remarks"] = errTrail;
                DSEXEMPTIONS.Tables[0].Rows.Add(dr);
                //Error += "\r\n";
                //Error += "Error in Trailer ";
                //Error += errTrail;
            }


            #endregion

            return text;
        }

        #endregion

        //OK
        #region SSS Prem

        //OK
        private string SSSPremGov(DataSet ds)
        {
            string text = string.Empty;
            int DetailCounter = 0;
            Error = string.Empty;
            string batchNumber = string.Empty;
            double TotalSSS = 0;
            double TotalEC = 0;
            double TotalPhil = 0;
            DataSet dsExemp = DSEXEMPTIONS;
            DataSet dsComp = GetCompanyInfo();
            string temp2 = string.Empty;
            #region header
            
            string header = string.Empty;
            string errtemp1 = string.Empty;
            try
            {
                header = "00";
                
                errtemp1 = "Error in Employer name";
                temp2 = dsComp.Tables[0].Rows[0]["Mcm_CompanyName"].ToString().Trim();
                if (temp2.Length < 30)
                {
                    int len = temp2.Length;
                    header += temp2;
                    for (int idx = 0; idx < (30 - len); idx++)
                        header += " ";
                }
                else
                {
                    header += temp2.Substring(0, 30);
                }

                errtemp1 = "Error in applicable month";
                header += MonthYear.Substring(4, 2) + MonthYear.Substring(0,4);


                errtemp1 = "Error in employer ID";
                temp2 = dsComp.Tables[0].Rows[0]["Mcm_SSSNo"].ToString().Replace(" ", "").Replace("-", "");
                if (temp2.Length < 10)
                {
                    throw new Exception();
                }
                else
                {
                    header += temp2;
                }
                temp2 = string.Empty;

                errtemp1 = "Error in SBR";
                string query = string.Format(
                    @"
                        Select * from T_MonthlySBR
                        Where Tms_DeductionCode = 'SSSPREM'
                        And Tms_YearMonth = '{0}'
                    ", MonthYear);
                using (DALHelper dal = new DALHelper())
                {
                    try
                    {
                        dal.OpenDB();
                        DataSet dsSBR = dal.ExecuteDataSet(query, CommandType.Text);

                        if (dsSBR != null && dsSBR.Tables[0].Rows.Count > 0)
                        {
                            temp2 = dsSBR.Tables[0].Rows[0]["Tms_ReceiptNo"].ToString().Trim().Replace(" ", "").Replace("-", "").Replace(".", "");
                            if (temp2.Length < 10)
                            {
                                int len = temp2.Length;
                                for (int idx = 0; idx < (10 - len); idx++)
                                    header += " ";
                                header += temp2;
                            }
                            else
                            {
                                header += temp2.Substring(0, 10);
                            }

                            temp2 = dsSBR.Tables[0].Rows[0]["Tms_PaymentDate"].ToString().Trim();
                            header += temp2.Replace("/", "");

                            temp2 = dsSBR.Tables[0].Rows[0]["Tms_PaidAmount"].ToString().Trim().Replace(" ", "").Replace("-", "");
                            if (temp2.Length < 12)
                            {
                                int len = temp2.Length;
                                for (int idx = 0; idx < (12 - len); idx++)
                                    header += " ";
                                header += temp2;
                            }
                            else
                            {
                                header += temp2;
                            }
                        }
                        else
                        {
                                     //         1         2         3
                                     //12345678901234567890123456789012345678901234567890
                            header += "                              ";
                        }
                        
                    }
                    catch
                    {
                        throw new Exception();
                    }
                    finally
                    {
                        dal.CloseDB();
                    }
                }
                
                text += header;
                text += "\r\n";
            }
            catch
            {
                DataRow dr = DSEXEMPTIONS.Tables[0].NewRow();
                dr["Lastname"] = "SSS Premium Header";
                dr["Firstname"] = "Header";
                dr["Remarks"] = errtemp1;
                DSEXEMPTIONS.Tables[0].Rows.Add(dr);
                //Error += "\r\n";
                //Error += "Error in header ";
                //Error += errtemp1;
            }

            #endregion

            #region details
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                string temp = string.Empty;
                int j = 0;
                for (; j < dsExemp.Tables[0].Rows.Count
                        && (
                        dsExemp.Tables[0].Rows[j]["Lastname"].ToString() != ds.Tables[0].Rows[i]["Mem_LastName"].ToString()
                        ||
                        dsExemp.Tables[0].Rows[j]["Firstname"].ToString() != ds.Tables[0].Rows[i]["Mem_FirstName"].ToString()
                        )
                    ; j++) ;
                if (j == dsExemp.Tables[0].Rows.Count)
                {
                    string errTemp = string.Empty;
                    try
                    {
                        temp = "20";

                        temp2 = string.Empty;
                        errTemp = "Error in Last name";
                        temp2 = ds.Tables[0].Rows[i]["Mem_LastName"].ToString().Replace("Ñ", "N").Trim().ToUpper();
                        if (temp2.Length < 15)
                        {
                            temp += temp2;
                            int len = temp2.Length;
                            for (int idx = 0; idx < (15 - len); idx++)
                                temp += " ";
                        }
                        else
                        {
                            temp += temp2.Substring(0, 15);
                        }
                        temp2 = string.Empty;

                        errTemp = "Error in First name";
                        temp2 = ds.Tables[0].Rows[i]["Mem_FirstName"].ToString().Replace("Ñ", "N").Trim().ToUpper();
                        if (temp2.Length < 15)
                        {
                            temp += temp2;
                            int len = temp2.Length;
                            for (int idx = 0; idx < (15 - len); idx++)
                                temp += " ";
                        }
                        else
                        {
                            temp += temp2.Substring(0, 15);
                        }
                        temp2 = string.Empty;

                        errTemp = "Error in Middlename";
                        if (ds.Tables[0].Rows[i]["Mem_MiddleName"].ToString().Trim() != string.Empty)
                        {
                            temp += ds.Tables[0].Rows[i]["Mem_MiddleName"].ToString().Trim().ToUpper().Substring(0, 1);
                        }
                        else
                        {
                            temp += " ";
                        }

                        errTemp = "Error in SSS Number";
                        if (ds.Tables[0].Rows[i]["Mem_SSSNo"].ToString().Trim().Replace(" ", "").Replace("-", "").Length < 10)
                        {
                            throw new Exception();
                        }
                        else
                        {
                            temp += ds.Tables[0].Rows[i]["Mem_SSSNo"].ToString().Trim().Replace(" ", "").Replace("-", "").Substring(0, 10);
                        }

                        temp += " ";

                        errTemp = "Error in SSS PREM, EC FUND, PHILHEALTH Values";
                        int monthIndicator = Convert.ToInt32( MonthYear.Substring(4, 2) );

                        string temp3 = string.Empty;
                        string temp4 = string.Empty;

                        temp2 = ds.Tables[0].Rows[i]["SSS Prem"].ToString().Trim();
                        temp3 = ds.Tables[0].Rows[i]["Philhealth"].ToString().Trim();
                        temp4 = ds.Tables[0].Rows[i]["EC Fund"].ToString().Trim();

                        TotalSSS += Convert.ToDouble(ds.Tables[0].Rows[i]["SSS Prem"].ToString());
                        TotalEC += Convert.ToDouble(ds.Tables[0].Rows[i]["EC Fund"].ToString());
                        TotalPhil += Convert.ToDouble(ds.Tables[0].Rows[i]["Philhealth"].ToString());

                        if (monthIndicator == 1
                           || monthIndicator == 4
                            || monthIndicator == 7
                            || monthIndicator == 10)
                        {                          
                            int len = temp2.Length;
                            for (int idx = 0; idx < (7 - len); idx++)
                                temp += " ";

                            temp += temp2;
                            temp += "    0.00    0.00 ";

                            len = temp3.Length;
                            for (int idx = 0; idx < (5 - len); idx++)
                                temp += " ";

                            temp += temp3;
                            temp += "  0.00  0.00 ";

                            len = temp4.Length;
                            for (int idx = 0; idx < (5 - len); idx++)
                                temp += " ";

                            temp += temp4;
                            temp += "  0.00  0.00      ";
                        }
                        else if (monthIndicator == 2
                           || monthIndicator == 5
                            || monthIndicator == 8
                            || monthIndicator == 11)
                        {
                            int len = temp2.Length;
                            temp += "   0.00 ";
                            for (int idx = 0; idx < (7 - len); idx++)
                                temp += " ";

                            temp += temp2;
                            temp += "    0.00 ";

                            len = temp3.Length;
                            temp += " 0.00 ";
                            for (int idx = 0; idx < (5 - len); idx++)
                                temp += " ";

                            temp += temp3;
                            temp += "  0.00 ";

                            len = temp4.Length;
                            temp += " 0.00 ";
                            for (int idx = 0; idx < (5 - len); idx++)
                                temp += " ";

                            temp += temp4;
                            temp += "  0.00      ";
                        }
                        else if (monthIndicator == 3
                      || monthIndicator == 6
                       || monthIndicator == 9
                       || monthIndicator == 12)
                        {
                            int len = temp2.Length;
                            temp += "   0.00    0.00 ";
                            for (int idx = 0; idx < (7 - len); idx++)
                                temp += " ";

                            temp += temp2;
                            temp += " ";

                            len = temp3.Length;
                            temp += " 0.00  0.00 ";
                            for (int idx = 0; idx < (5 - len); idx++)
                                temp += " ";

                            temp += temp3;
                            temp += " ";

                            len = temp4.Length;
                            temp += " 0.00  0.00 ";
                            for (int idx = 0; idx < (5 - len); idx++)
                                temp += " ";

                            temp += temp4;
                            temp += "      ";
                        }

                        errTemp = "Error in remarks";

                        if (ds.Tables[0].Rows[i]["Tgr_StatusCode"].ToString().Trim() == string.Empty)
                        {
                            temp += " ";
                        }
                        else
                        {
                            temp += ds.Tables[0].Rows[i]["Tgr_StatusCode"].ToString().Substring(0, 1);
                        }
                        errTemp = "Error in hire /sep date";
                        if (ds.Tables[0].Rows[i]["Tgr_StatusDate"].ToString().Trim() != string.Empty)
                        {
                            temp2 = ds.Tables[0].Rows[i]["Tgr_StatusDate"].ToString().Trim().Replace("/", "");
                            temp += temp2.Substring(4, 4) + temp2.Substring(0, 2) + temp2.Substring(2, 2);
                        }
                        else
                        {
                            temp += "0       ";
                        }

                        text += temp + "\r\n";
                        ++DetailCounter;
                    }
                    catch
                    {
                        DataRow dr = DSEXEMPTIONS.Tables[0].NewRow();
                        dr["Lastname"] = ds.Tables[0].Rows[i]["Mem_LastName"].ToString();
                        dr["Firstname"] = ds.Tables[0].Rows[i]["Mem_FirstName"].ToString();
                        dr["Remarks"] = errTemp;
                        DSEXEMPTIONS.Tables[0].Rows.Add(dr);
                    }

                }
            }

            #endregion

            #region trailer

            string Trail = string.Empty;
            string errTrail = string.Empty;

            try
            {
                errTrail = "Error in record id trail";
                Trail = "99";

                Trail += " ";

                errTrail = "Error in totals";
                int monthIndicator = Convert.ToInt32(MonthYear.Substring(4, 2));

                string temp3 = string.Empty;
                string temp4 = string.Empty;

                temp2 = string.Format("{0:0.00}", TotalSSS);
                temp3 = string.Format("{0:0.00}", TotalPhil);
                temp4 = string.Format("{0:0.00}", TotalEC);

                if (monthIndicator == 1
                           || monthIndicator == 4
                            || monthIndicator == 7
                            || monthIndicator == 10)
                {
                    int len = temp2.Length;
                    for (int idx = 0; idx < (11 - len); idx++)
                        Trail += " ";

                    Trail += temp2;
                    Trail += "        0.00        0.00 ";

                    len = temp3.Length;
                    for (int idx = 0; idx < (9 - len); idx++)
                        Trail += " ";

                    Trail += temp3;
                    Trail += "      0.00      0.00 ";

                    len = temp4.Length;
                    for (int idx = 0; idx < (9 - len); idx++)
                        Trail += " ";

                    Trail += temp4;
                    Trail += "      0.00      0.00";
                }
                else if (monthIndicator == 2
                           || monthIndicator == 5
                            || monthIndicator == 8
                            || monthIndicator == 11)
                {
                    int len = temp2.Length;
                    Trail += "       0.00 ";
                    for (int idx = 0; idx < (11 - len); idx++)
                        Trail += " ";

                    Trail += temp2;
                    Trail += "        0.00 ";

                    len = temp3.Length;
                    Trail += "     0.00 ";
                    for (int idx = 0; idx < (9 - len); idx++)
                        Trail += " ";

                    Trail += temp3;
                    Trail += "      0.00 ";

                    len = temp4.Length;
                    Trail += "     0.00 ";
                    for (int idx = 0; idx < (9 - len); idx++)
                        Trail += " ";

                    Trail += temp4;
                    Trail += "      0.00";
                }
                else if (monthIndicator == 3
              || monthIndicator == 6
               || monthIndicator == 9
               || monthIndicator == 12)
                {
                    int len = temp2.Length;
                    Trail += "       0.00        0.00 ";
                    for (int idx = 0; idx < (11 - len); idx++)
                        Trail += " ";

                    Trail += temp2;

                    len = temp3.Length;
                    Trail += "      0.00      0.00 ";
                    for (int idx = 0; idx < (9 - len); idx++)
                        Trail += " ";

                    Trail += temp3;

                    len = temp4.Length;
                    Trail += "      0.00      0.00 ";
                    for (int idx = 0; idx < (9 - len); idx++)
                        Trail += " ";

                    Trail += temp4;
                }
                        //         1         2         3
                        //123456789012345678901234567890
                Trail += "                    ";

                text += Trail;

            }
            catch
            {
                DataRow dr = DSEXEMPTIONS.Tables[0].NewRow();
                dr["Lastname"] = "SSS Premium Trail";
                dr["Firstname"] = "Trail";
                dr["Remarks"] = errTrail;
                DSEXEMPTIONS.Tables[0].Rows.Add(dr);
                //Error += "\r\n";
                //Error += "Error in Trailer ";
                //Error += errTrail;
            }


            #endregion

            return text;
        }

        //OK
        private string SSSPremUnionBank(DataSet ds)
        {
            string text = string.Empty;
            int DetailCounter = 0;
            Error = string.Empty;
            string batchNumber = string.Empty;
            double TotalSSS = 0;
            double TotalEC = 0;
            double TotalPhil = 0;
            DataSet dsExemp = DSEXEMPTIONS;
            DataSet dsComp = GetCompanyInfo();
            string temp2 = string.Empty;
            #region header
            
            string header = string.Empty;
            string errtemp1 = string.Empty;
            try
            {           //123456789012345678901234567890
                header = "00   1000001         " + dateGenerate.Year.ToString() 
                                + dateGenerate.ToString("MMddyyyy").Substring(0,2)
                                + dateGenerate.ToString("MMddyyyy").Substring(2,2)
                                + MonthYear.ToString();
                
                errtemp1 = "Error in Employer name";
                temp2 = dsComp.Tables[0].Rows[0]["Mcm_CompanyName"].ToString().Trim();
                if (temp2.Length < 40)
                {
                    int len = temp2.Length;
                    header += temp2;
                    for (int idx = 0; idx < (40 - len); idx++)
                        header += " ";
                }
                else
                {
                    header += temp2.Substring(0, 40);
                }

                errtemp1 = "Error in employer ID";
                temp2 = dsComp.Tables[0].Rows[0]["Mcm_SSSNo"].ToString().Replace(" ", "").Replace("-", "");
                if (temp2.Length < 10)
                {
                    throw new Exception();
                }
                else
                {
                    header += temp2;
                }
                temp2 = string.Empty;

                temp2 = dsComp.Tables[0].Rows[0]["Mcm_BankBranchCode"].ToString().Trim();
                if(temp2.Length < 3)
                {
                    for(int idx = 0; idx < temp2.Length; idx++)
                        header += " ";
                    header += temp2;
                }
                else
                {
                    header += temp2.Substring(0, 3);
                }

                errtemp1 = " Error in Locator Code";
                using (DALHelper dal = new DALHelper())
                {
                    try
                    {
                        dal.OpenDB();
                        DataSet dsloccode = dal.ExecuteDataSet(
                            @"
                           select Mpd_SubCode
                            from M_PolicyDtl
                            where Mpd_ParamValue = '1'
                            and Mpd_PolicyCode = 'SSSLOCCODE'
                            and Mpd_RecordStatus = 'A'
                            ");
                        if (dsloccode != null && dsloccode.Tables[0].Rows.Count > 0)
                            temp2 = dsloccode.Tables[0].Rows[0][0].ToString().Trim();
                        else
                            throw new Exception();
                                                        
                    }
                    catch
                    {
                        throw new Exception();
                    }
                    finally
                    {
                        dal.CloseDB();
                    }
                }
                if(temp2.Length < 3)
                {
                    header += temp2;
                    for(int idx = 0; idx < (3 - temp2.Length); idx++)
                    {
                        header += " ";
                    }
                }

                
                text += header;
                text += "\r\n";
            }
            catch
            {
                DataRow dr = DSEXEMPTIONS.Tables[0].NewRow();
                dr["Lastname"] = "SSS Premium Header";
                dr["Firstname"] = "Header";
                dr["Remarks"] = errtemp1;
                DSEXEMPTIONS.Tables[0].Rows.Add(dr);
                //Error += "\r\n";
                //Error += "Error in header ";
                //Error += errtemp1;
            }

            #endregion

            #region details
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                string temp = string.Empty;
                int j = 0;
                for (; j < dsExemp.Tables[0].Rows.Count
                        && (
                        dsExemp.Tables[0].Rows[j]["Lastname"].ToString() != ds.Tables[0].Rows[i]["Mem_LastName"].ToString()
                        ||
                        dsExemp.Tables[0].Rows[j]["Firstname"].ToString() != ds.Tables[0].Rows[i]["Mem_FirstName"].ToString()
                        )
                    ; j++) ;
                if (j == dsExemp.Tables[0].Rows.Count)
                {
                    string errTemp = string.Empty;
                    try
                    {
                        temp = "20   ";

                        temp2 = string.Empty;
                        errTemp = "Error in Last name";
                        temp2 = ds.Tables[0].Rows[i]["Mem_LastName"].ToString().Replace("Ñ", "N").Trim().ToUpper();
                        if (temp2.Length < 20)
                        {
                            temp += temp2;
                            int len = temp2.Length;
                            for (int idx = 0; idx < (20 - len); idx++)
                                temp += " ";
                        }
                        else
                        {
                            temp += temp2.Substring(0, 20);
                        }
                        temp2 = string.Empty;

                        errTemp = "Error in First name";
                        temp2 = ds.Tables[0].Rows[i]["Mem_FirstName"].ToString().Replace("Ñ", "N").Trim().ToUpper();
                        if (temp2.Length < 20)
                        {
                            temp += temp2;
                            int len = temp2.Length;
                            for (int idx = 0; idx < (20 - len); idx++)
                                temp += " ";
                        }
                        else
                        {
                            temp += temp2.Substring(0, 20);
                        }
                        temp2 = string.Empty;

                        errTemp = "Error in Middlename";
                        if (ds.Tables[0].Rows[i]["Mem_MiddleName"].ToString().Trim() != string.Empty)
                        {
                            temp += ds.Tables[0].Rows[i]["Mem_MiddleName"].ToString().Trim().ToUpper().Substring(0, 1);
                        }
                        else
                        {
                            temp += " ";
                        }

                        errTemp = "Error in SSS Number";
                        if (ds.Tables[0].Rows[i]["Mem_SSSNo"].ToString().Trim().Replace(" ", "").Replace("-", "").Length < 10)
                        {
                            throw new Exception();
                        }
                        else
                        {
                            temp += ds.Tables[0].Rows[i]["Mem_SSSNo"].ToString().Trim().Replace(" ", "").Replace("-", "").Substring(0, 10);
                        }

                        errTemp = "Error in SSS PREM, EC FUND, PHILHEALTH Values";
                        int monthIndicator = Convert.ToInt32( MonthYear.Substring(4, 2) );

                        string temp3 = string.Empty;
                        string temp4 = string.Empty;

                        temp2 = ds.Tables[0].Rows[i]["SSS Prem"].ToString().Trim();
                        temp3 = ds.Tables[0].Rows[i]["EC Fund"].ToString().Trim();
                        temp4 = ds.Tables[0].Rows[i]["EC Fund"].ToString().Trim();

                        TotalSSS += Convert.ToDouble(ds.Tables[0].Rows[i]["SSS Prem"].ToString());
                        TotalEC += Convert.ToDouble(ds.Tables[0].Rows[i]["EC Fund"].ToString());
                        TotalPhil += Convert.ToDouble(ds.Tables[0].Rows[i]["EC Fund"].ToString());

                        int len2 = temp2.Length;
                        for (int idx = 0; idx < (8 - len2); idx++)
                            temp += " ";
                        temp += temp2;

                        len2 = temp3.Length;
                        for (int idx = 0; idx < (8 - len2); idx++)
                            temp += " ";

                        temp += temp3;

                        errTemp = "Error in hire /sep date";
                        if (ds.Tables[0].Rows[i]["Tgr_CheckDate"].ToString().Trim().Replace("/", "") != string.Empty)
                        {
                            temp2 = ds.Tables[0].Rows[i]["Tgr_CheckDate"].ToString().Trim().Replace("/", "");
                            temp += temp2; //temp2.Substring(4, 4) + temp2.Substring(0, 2) + temp2.Substring(2, 2);
                        }
                        else
                        {
                            temp += "        ";
                        }
                        errTemp = "Error in remarks";
                        temp2 = ds.Tables[0].Rows[i]["Tgr_StatusCode"].ToString().Trim();
                        if (temp2.Trim() != string.Empty)
                        {
                            if (temp2.Trim().Substring(0, 1) == "N")
                                temp += "3";
                            else 
                                temp += temp2.Trim().Substring(0, 1);
                            
                        }
                        else
                        {
                            temp += "3";
                        }

                        text += temp + "\r\n";
                        ++DetailCounter;
                    }
                    catch
                    {
                        DataRow dr = DSEXEMPTIONS.Tables[0].NewRow();
                        dr["Lastname"] = ds.Tables[0].Rows[i]["Mem_LastName"].ToString();
                        dr["Firstname"] = ds.Tables[0].Rows[i]["Mem_FirstName"].ToString();
                        dr["Remarks"] = errTemp;
                        DSEXEMPTIONS.Tables[0].Rows.Add(dr);
                    }

                }
            }

            #endregion

            #region trailer

            string Trail = string.Empty;
            string errTrail = string.Empty;

            try
            {
                errTrail = "Error in record id trail";
                Trail = "99   ";

                errTrail = "Error in totals";
                int monthIndicator = Convert.ToInt32(MonthYear.Substring(4, 2));

                string temp3 = string.Empty;
                string temp4 = string.Empty;

                temp2 = string.Format("{0:0.00}", TotalSSS);
                temp3 = string.Format("{0:0.00}", TotalPhil);
                temp4 = string.Format("{0:0.00}", TotalEC);

                int len = temp2.Length;
                for (int idx = 0; idx < (12 - len); idx++)
                    Trail += " ";

                Trail += temp2;

                len = temp4.Length;
                for (int idx = 0; idx < (12 - len); idx++)
                    Trail += " ";

                Trail += temp4;

                Trail += "00000001";

                        //         1         2         3
                        //123456789012345678901234567890
                Trail += "       ";

                text += Trail;

            }
            catch
            {
                DataRow dr = DSEXEMPTIONS.Tables[0].NewRow();
                dr["Lastname"] = "SSS Premium Trail";
                dr["Firstname"] = "Trail";
                dr["Remarks"] = errTrail;
                DSEXEMPTIONS.Tables[0].Rows.Add(dr);
                //Error += "\r\n";
                //Error += "Error in Trailer ";
                //Error += errTrail;
            }


            #endregion

            return text;
        }

        //OK
        private string SSSPremMetroBankEPF(DataSet ds)
        {
            string text = string.Empty;
            int DetailCounter = 0;
            Error = string.Empty;
            string batchNumber = string.Empty;
            DataSet dsExemp = DSEXEMPTIONS;
            DataSet dsComp = GetCompanyInfo();
            string temp2 = string.Empty;
            #region header

            string header = string.Empty;
            string errtemp1 = string.Empty;
            try
            {           //123456789012345678901234567890
                header = "00EDIEELST";

                errtemp1 = "Error in employer ID";
                temp2 = dsComp.Tables[0].Rows[0]["Mcm_SSSNo"].ToString().Replace(" ", "").Replace("-", "");
                if (temp2.Length < 10)
                {
                    throw new Exception();
                }
                else
                {
                    header += temp2;
                }

                errtemp1 = "Error in Employer name";
                temp2 = dsComp.Tables[0].Rows[0]["Mcm_CompanyName"].ToString().Trim().Replace("Ñ", "N");
                if (temp2.Length < 30)
                {
                    int len = temp2.Length;
                    header += temp2;
                    for (int idx = 0; idx < (30 - len); idx++)
                        header += " ";
                }
                else
                {
                    header += temp2.Substring(0, 30);
                }

                text += header;
                text += "\r\n";
            }
            catch
            {
                DataRow dr = DSEXEMPTIONS.Tables[0].NewRow();
                dr["Lastname"] = "SSS Premium Header";
                dr["Firstname"] = "Header";
                dr["Remarks"] = errtemp1;
                DSEXEMPTIONS.Tables[0].Rows.Add(dr);
                //Error += "\r\n";
                //Error += "Error in header ";
                //Error += errtemp1;
            }

            #endregion

            #region details
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                string temp = string.Empty;
                int j = 0;
                for (; j < dsExemp.Tables[0].Rows.Count
                        && (
                        dsExemp.Tables[0].Rows[j]["Lastname"].ToString() != ds.Tables[0].Rows[i]["Mem_LastName"].ToString()
                        ||
                        dsExemp.Tables[0].Rows[j]["Firstname"].ToString() != ds.Tables[0].Rows[i]["Mem_FirstName"].ToString()
                        )
                    ; j++) ;
                if (j == dsExemp.Tables[0].Rows.Count)
                {
                    string errTemp = string.Empty;
                    try
                    {
                        temp = "20   ";

                        errTemp = "Error in SSS Number";
                        if (ds.Tables[0].Rows[i]["Mem_SSSNo"].ToString().Trim().Replace(" ", "").Replace("-", "").Length < 10)
                        {
                            throw new Exception();
                        }
                        else
                        {
                            temp += ds.Tables[0].Rows[i]["Mem_SSSNo"].ToString().Trim().Replace(" ", "").Replace("-", "").Substring(0, 10);
                        }

                        temp2 = string.Empty;
                        errTemp = "Error in Last name";
                        temp2 = ds.Tables[0].Rows[i]["Mem_LastName"].ToString().Trim().ToUpper().Replace("Ñ", "N");
                        if (temp2.Length < 15)
                        {
                            temp += temp2;
                            int len = temp2.Length;
                            for (int idx = 0; idx < (15 - len); idx++)
                                temp += " ";
                        }
                        else
                        {
                            temp += temp2.Substring(0, 15);
                        }
                        temp2 = string.Empty;

                        errTemp = "Error in First name";
                        temp2 = ds.Tables[0].Rows[i]["Mem_FirstName"].ToString().Trim().ToUpper().Replace("Ñ", "N");
                        if (temp2.Length < 15)
                        {
                            temp += temp2;
                            int len = temp2.Length;
                            for (int idx = 0; idx < (15 - len); idx++)
                                temp += " ";
                        }
                        else
                        {
                            temp += temp2.Substring(0, 15);
                        }
                        temp2 = string.Empty;

                        errTemp = "Error in Middlename";
                        if (ds.Tables[0].Rows[i]["Mem_MiddleName"].ToString().Trim() != string.Empty)
                        {
                            temp += ds.Tables[0].Rows[i]["Mem_MiddleName"].ToString().Trim().ToUpper().Replace("Ñ", "N").Substring(0, 1);
                        }
                        else
                        {
                            temp += " ";
                        }

                        errTemp = "Error in Birthdate";
                        temp += ds.Tables[0].Rows[i]["Mem_BirthDate"].ToString().Trim().Replace("/", "");

                        text += temp + "\r\n";
                        ++DetailCounter;
                    }
                    catch
                    {
                        DataRow dr = DSEXEMPTIONS.Tables[0].NewRow();
                        dr["Lastname"] = ds.Tables[0].Rows[i]["Mem_LastName"].ToString();
                        dr["Firstname"] = ds.Tables[0].Rows[i]["Mem_FirstName"].ToString();
                        dr["Remarks"] = errTemp;
                        DSEXEMPTIONS.Tables[0].Rows.Add(dr);
                    }

                }
            }

            #endregion

            #region trailer

            string Trail = string.Empty;
            string errTrail = string.Empty;

            try
            {
                errTrail = "Error in record id trail";
                Trail = "99   ";

                errTrail = "Error in totals";
                if (DetailCounter.ToString().Trim().Length < 6)
                {
                    for (int idx = 0; idx < (6 - DetailCounter.ToString().Trim().Length); idx++)
                        Trail += "0";
                    Trail += DetailCounter.ToString().Trim();
                }
                else
                {
                    Trail += DetailCounter.ToString().Trim().Substring(0, 9);
                }

                text += Trail;

            }
            catch
            {
                DataRow dr = DSEXEMPTIONS.Tables[0].NewRow();
                dr["Lastname"] = "SSS Premium Trail";
                dr["Firstname"] = "Trail";
                dr["Remarks"] = errTrail;
                DSEXEMPTIONS.Tables[0].Rows.Add(dr);
                //Error += "\r\n";
                //Error += "Error in Trailer ";
                //Error += errTrail;
            }


            #endregion

            return text;
        }

        //OK
        private string SSSPremMetroBankMCL(DataSet ds)
        {
            string text = string.Empty;
            int DetailCounter = 0;
            Error = string.Empty;
            string batchNumber = string.Empty;
            double TotalSSS = 0;
            double TotalEC = 0;
            double TotalPhil = 0;
            DataSet dsExemp = DSEXEMPTIONS;
            DataSet dsComp = GetCompanyInfo();
            string temp2 = string.Empty;
            #region header

            string header = string.Empty;
            string errtemp1 = string.Empty;
            try
            {           //12345       123456789
                header = "00   1000001         ";

                errtemp1 = "Error in Date";
                header += dateGenerate.ToString("MMddyyyy").Substring(4, 4)
                    + dateGenerate.ToString("MMddyyyy").Substring(0, 2)
                    + dateGenerate.ToString("MMddyyyy").Substring(2, 2);

                errtemp1 = "Error on applicatble month";
                header += MonthYear.ToString();

                errtemp1 = "Error in Employer name";
                temp2 = dsComp.Tables[0].Rows[0]["Mcm_CompanyName"].ToString().Replace("Ñ", "N").Trim();
                if (temp2.Length < 40)
                {
                    int len = temp2.Length;
                    header += temp2;
                    for (int idx = 0; idx < (40 - len); idx++)
                        header += " ";
                }
                else
                {
                    header += temp2.Substring(0, 40);
                }

                errtemp1 = "Error in employer ID";
                temp2 = dsComp.Tables[0].Rows[0]["Mcm_SSSNo"].ToString().Replace(" ", "").Replace("-", "");
                if (temp2.Length < 10)
                {
                    throw new Exception();
                }
                else
                {
                    header += temp2;
                }
                temp2 = string.Empty;

                temp2 = dsComp.Tables[0].Rows[0]["Mcm_BankBranchCode"].ToString().Trim();
                if (temp2.Length < 3)
                {
                    for (int idx = 0; idx < temp2.Length; idx++)
                        header += " ";
                    header += temp2;
                }
                else
                {
                    header += temp2.Substring(0, 3);
                }

                errtemp1 = "Error in Locator Code";
                using (DALHelper dal = new DALHelper())
                {
                    try
                    {
                        dal.OpenDB();
                        DataSet dsloccode = dal.ExecuteDataSet(
                            @"
                           select Mpd_SubCode
                            from M_PolicyDtl
                            where Mpd_ParamValue = '1'
                            and Mpd_PolicyCode = 'SSSLOCCODE'
                            and Mpd_RecordStatus = 'A'
                            ");
                        if (dsloccode != null && dsloccode.Tables[0].Rows.Count > 0)
                            temp2 = dsloccode.Tables[0].Rows[0][0].ToString().Trim();
                        else
                            throw new Exception();

                    }
                    catch
                    {
                        throw new Exception();
                    }
                    finally
                    {
                        dal.CloseDB();
                    }
                }
                if (temp2.Length < 3)
                {
                    header += temp2;
                    for (int idx = 0; idx < (3 - temp2.Length); idx++)
                    {
                        header += " ";
                    }
                }


                text += header;
                text += "\r\n";
            }
            catch
            {
                DataRow dr = DSEXEMPTIONS.Tables[0].NewRow();
                dr["Lastname"] = "SSS Premium Header";
                dr["Firstname"] = "Header";
                dr["Remarks"] = errtemp1;
                DSEXEMPTIONS.Tables[0].Rows.Add(dr);
                //Error += "\r\n";
                //Error += "Error in header ";
                //Error += errtemp1;
            }

            #endregion

            #region details
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                string temp = string.Empty;
                int j = 0;
                for (; j < dsExemp.Tables[0].Rows.Count
                        && (
                        dsExemp.Tables[0].Rows[j]["Lastname"].ToString() != ds.Tables[0].Rows[i]["Mem_LastName"].ToString()
                        ||
                        dsExemp.Tables[0].Rows[j]["Firstname"].ToString() != ds.Tables[0].Rows[i]["Mem_FirstName"].ToString()
                        )
                    ; j++) ;
                if (j == dsExemp.Tables[0].Rows.Count)
                {
                    string errTemp = string.Empty;
                    try
                    {
                        temp = "20   ";
                        
                        temp2 = string.Empty;
                        errTemp = "Error in Last name";
                        temp2 = ds.Tables[0].Rows[i]["Mem_LastName"].ToString().Trim().Replace("Ñ", "N").ToUpper();
                        if (temp2.Length < 20)
                        {
                            temp += temp2;
                            int len = temp2.Length;
                            for (int idx = 0; idx < (20 - len); idx++)
                                temp += " ";
                        }
                        else
                        {
                            temp += temp2.Substring(0, 20);
                        }
                        temp2 = string.Empty;

                        errTemp = "Error in First name";
                        temp2 = ds.Tables[0].Rows[i]["Mem_FirstName"].ToString().Trim().Replace("Ñ", "N").ToUpper();
                        if (temp2.Length < 20)
                        {
                            temp += temp2;
                            int len = temp2.Length;
                            for (int idx = 0; idx < (20 - len); idx++)
                                temp += " ";
                        }
                        else
                        {
                            temp += temp2.Substring(0, 20);
                        }
                        temp2 = string.Empty;

                        errTemp = "Error in Middlename";
                        if (ds.Tables[0].Rows[i]["Mem_MiddleName"].ToString().Trim() != string.Empty)
                        {
                            temp += ds.Tables[0].Rows[i]["Mem_MiddleName"].ToString().Trim().Replace("Ñ", "N").ToUpper().Substring(0, 1);
                        }
                        else
                        {
                            temp += " ";
                        }

                        errTemp = "Error in SSS Number";
                        if (ds.Tables[0].Rows[i]["Mem_SSSNo"].ToString().Trim().Replace(" ", "").Replace("-", "").Length < 10)
                        {
                            throw new Exception();
                        }
                        else
                        {
                            temp += ds.Tables[0].Rows[i]["Mem_SSSNo"].ToString().Trim().Replace(" ", "").Replace("-", "").Substring(0, 10);
                        }

                        errTemp = "Error in SSS PREM, EC FUND, PHILHEALTH Values";
                        int monthIndicator = Convert.ToInt32(MonthYear.Substring(4, 2));

                        string temp3 = string.Empty;
                        string temp4 = string.Empty;

                        temp2 = ds.Tables[0].Rows[i]["SSS Prem"].ToString().Trim();
                        temp3 = ds.Tables[0].Rows[i]["Philhealth"].ToString().Trim();
                        temp4 = ds.Tables[0].Rows[i]["EC Fund"].ToString().Trim();

                        TotalSSS += Convert.ToDouble(ds.Tables[0].Rows[i]["SSS Prem"].ToString());
                        TotalEC += Convert.ToDouble(ds.Tables[0].Rows[i]["EC Fund"].ToString());
                        TotalPhil += Convert.ToDouble(ds.Tables[0].Rows[i]["Philhealth"].ToString());

                        int len2 = temp2.Length;
                        for (int idx = 0; idx < (8 - len2); idx++)
                            temp += " ";

                        temp += temp2;

                        len2 = temp3.Length;
                        for (int idx = 0; idx < (8 - len2); idx++)
                            temp += " ";

                        temp += temp3;

                        len2 = temp4.Length;
                        for (int idx = 0; idx < (8 - len2); idx++)
                            temp += " ";

                        temp += temp4;


                        errTemp = "Error in hire /sep date";
                        if (ds.Tables[0].Rows[i]["Mem_BirthDate"].ToString().Trim().Replace("/", "") != string.Empty)
                        {
                            temp2 = ds.Tables[0].Rows[i]["Mem_BirthDate"].ToString().Trim().Replace("/", "");
                            temp += temp2.Substring(4, 4) + temp2.Substring(0, 2) + temp2.Substring(2, 2);
                        }
                        else
                        {
                            temp += "        ";
                        }
                        errTemp = "Error in remarks";
                        temp2 = ds.Tables[0].Rows[i]["Tgr_StatusCode"].ToString().Trim();
                        if (temp2.Trim() != string.Empty)
                        {
                            if (temp2.Trim().Substring(0, 1) == "N")
                                temp += "3";
                            else
                                temp += temp2.Trim().Substring(0, 1);

                        }
                        else
                        {
                            temp += "3";
                        }

                        text += temp + "\r\n";
                        ++DetailCounter;
                    }
                    catch
                    {
                        DataRow dr = DSEXEMPTIONS.Tables[0].NewRow();
                        dr["Lastname"] = ds.Tables[0].Rows[i]["Mem_LastName"].ToString();
                        dr["Firstname"] = ds.Tables[0].Rows[i]["Mem_FirstName"].ToString();
                        dr["Remarks"] = errTemp;
                        DSEXEMPTIONS.Tables[0].Rows.Add(dr);
                    }

                }
            }

            #endregion

            #region trailer

            string Trail = string.Empty;
            string errTrail = string.Empty;

            try
            {
                errTrail = "Error in record id trail";
                Trail = "99   ";

                errTrail = "Error in totals";
                int monthIndicator = Convert.ToInt32(MonthYear.Substring(4, 2));

                string temp3 = string.Empty;
                string temp4 = string.Empty;

                temp2 = string.Format("{0:0.00}", TotalSSS);
                temp3 = string.Format("{0:0.00}", TotalPhil);
                temp4 = string.Format("{0:0.00}", TotalEC);

                int len = temp2.Length;
                for (int idx = 0; idx < (12 - len); idx++)
                    Trail += " ";

                Trail += temp2;

                len = temp3.Length;
                for (int idx = 0; idx < (12 - len); idx++)
                    Trail += " ";

                Trail += temp3;

                len = temp4.Length;
                for (int idx = 0; idx < (12 - len); idx++)
                    Trail += " ";

                Trail += temp4;

                Trail += "00000001";

                        //         1         2         3
                        //123456789012345678901234567890
                Trail += "       ";

                text += Trail;

            }
            catch
            {
                DataRow dr = DSEXEMPTIONS.Tables[0].NewRow();
                dr["Lastname"] = "SSS Premium Trail";
                dr["Firstname"] = "Trail";
                dr["Remarks"] = errTrail;
                DSEXEMPTIONS.Tables[0].Rows.Add(dr);
                //Error += "\r\n";
                //Error += "Error in Trailer ";
                //Error += errTrail;
            }


            #endregion

            return text;
        }

        //OK
        private string SSSPremBDO(DataSet ds)
        {
            string text = string.Empty;
            int DetailCounter = 0;
            Error = string.Empty;
            string batchNumber = string.Empty;
            double TotalSSS = 0;
            double TotalEC = 0;
            double TotalPhil = 0;
            DataSet dsExemp = DSEXEMPTIONS;
            DataSet dsComp = GetCompanyInfo();
            string temp2 = string.Empty;
            #region header

            string header = string.Empty;
            string errtemp1 = string.Empty;
            try
            {
                header = "00   1";

                errtemp1 = "Error in applicable month";
                header += MonthYear.Substring(0, 4) + MonthYear.Substring(4, 2) + "01";

                         //1234567890
                header += "       ";

                errtemp1 = "Error in date transaction";
                header += dateGenerate.ToString("yyyyMMdd");

                errtemp1 = "Error in Month Year";
                header += MonthYear;

                errtemp1 = "Error in Employer name";
                temp2 = dsComp.Tables[0].Rows[0]["Mcm_CompanyName"].ToString().Trim();
                if (temp2.Length < 40)
                {
                    int len = temp2.Length;
                    header += temp2;
                    for (int idx = 0; idx < (40 - len); idx++)
                        header += " ";
                }
                else
                {
                    header += temp2.Substring(0, 40);
                }
                
                errtemp1 = "Error in employer ID";
                temp2 = dsComp.Tables[0].Rows[0]["Mcm_SSSNo"].ToString().Replace(" ", "").Replace("-", "");
                if (temp2.Length < 10)
                {
                    throw new Exception();
                }
                else
                {
                    header += temp2;
                }
                temp2 = string.Empty;
                            
                         //12345678
                header += "   ";

                errtemp1 = " Error in Locator Code";
                using (DALHelper dal = new DALHelper())
                {
                    try
                    {
                        dal.OpenDB();
                        DataSet dsloccode = dal.ExecuteDataSet(
                            @"
                           select Mpd_SubCode
                            from M_PolicyDtl
                            where Mpd_ParamValue = '1'
                            and Mpd_PolicyCode = 'SSSLOCCODE'
                            and Mpd_RecordStatus = 'A'
                            ");
                        if (dsloccode != null && dsloccode.Tables[0].Rows.Count > 0)
                            header += dsloccode.Tables[0].Rows[0][0].ToString().Trim() + "  ";
                        else
                            throw new Exception();

                    }
                    catch
                    {
                        throw new Exception();
                    }
                    finally
                    {
                        dal.CloseDB();
                    }
                }

                text += header;
                text += "\r\n";
            }
            catch
            {
                DataRow dr = DSEXEMPTIONS.Tables[0].NewRow();
                dr["Lastname"] = "SSS Premium Header";
                dr["Firstname"] = "Header";
                dr["Remarks"] = errtemp1;
                DSEXEMPTIONS.Tables[0].Rows.Add(dr);
                //Error += "\r\n";
                //Error += "Error in header ";
                //Error += errtemp1;
            }

            #endregion

            #region details
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                string temp = string.Empty;
                int j = 0;
                for (; j < dsExemp.Tables[0].Rows.Count
                        && (
                        dsExemp.Tables[0].Rows[j]["Lastname"].ToString() != ds.Tables[0].Rows[i]["Mem_LastName"].ToString()
                        ||
                        dsExemp.Tables[0].Rows[j]["Firstname"].ToString() != ds.Tables[0].Rows[i]["Mem_FirstName"].ToString()
                        )
                    ; j++) ;
                if (j == dsExemp.Tables[0].Rows.Count)
                {
                    string errTemp = string.Empty;
                    try
                    {
                        temp = "20   ";

                        temp2 = string.Empty;
                        errTemp = "Error in Last name";
                        temp2 = ds.Tables[0].Rows[i]["Mem_LastName"].ToString().Replace("Ñ", "N").Trim().ToUpper();
                        if (temp2.Length < 20)
                        {
                            temp += temp2;
                            int len = temp2.Length;
                            for (int idx = 0; idx < (20 - len); idx++)
                                temp += " ";
                        }
                        else
                        {
                            temp += temp2.Substring(0, 20);
                        }
                        temp2 = string.Empty;

                        errTemp = "Error in First name";
                        temp2 = ds.Tables[0].Rows[i]["Mem_FirstName"].ToString().Replace("Ñ", "N").Trim().ToUpper();
                        if (temp2.Length < 20)
                        {
                            temp += temp2;
                            int len = temp2.Length;
                            for (int idx = 0; idx < (20 - len); idx++)
                                temp += " ";
                        }
                        else
                        {
                            temp += temp2.Substring(0, 20);
                        }
                        temp2 = string.Empty;

                        errTemp = "Error in Middlename";
                        if (ds.Tables[0].Rows[i]["Mem_MiddleName"].ToString().Trim() != string.Empty)
                        {
                            temp += ds.Tables[0].Rows[i]["Mem_MiddleName"].ToString().Replace("Ñ", "N").Trim().ToUpper().Substring(0, 1);
                        }
                        else
                        {
                            temp += " ";
                        }

                        errTemp = "Error in SSS Number";
                        if (ds.Tables[0].Rows[i]["Mem_SSSNo"].ToString().Trim().Replace(" ", "").Replace("-", "").Length < 10)
                        {
                            throw new Exception();
                        }
                        else
                        {
                            temp += ds.Tables[0].Rows[i]["Mem_SSSNo"].ToString().Trim().Replace(" ", "").Replace("-", "").Substring(0, 10);
                        }

                        errTemp = "Error in SSS PREM, EC FUND, PHILHEALTH Values";
                        int monthIndicator = Convert.ToInt32(MonthYear.Substring(4, 2));

                        string temp3 = string.Empty;
                        string temp4 = string.Empty;

                        temp2 = ds.Tables[0].Rows[i]["SSS Prem"].ToString().Trim();
                        temp3 = ds.Tables[0].Rows[i]["EC Fund"].ToString().Trim();
                        temp4 = ds.Tables[0].Rows[i]["EC Fund"].ToString().Trim();

                        TotalSSS += Convert.ToDouble(ds.Tables[0].Rows[i]["SSS Prem"].ToString());
                        TotalEC += Convert.ToDouble(ds.Tables[0].Rows[i]["EC Fund"].ToString());
                        TotalPhil += 0;// Convert.ToDouble(ds.Tables[0].Rows[i]["EC Fund"].ToString());

                        int len2 = temp2.Length;
                        for (int idx = 0; idx < (8 - len2); idx++)
                            temp += " ";
                        temp += temp2;

                        temp += "    0.00";

                        len2 = temp3.Length;
                        for (int idx = 0; idx < (8 - len2); idx++)
                            temp += " ";

                        temp += temp3;

                        string strRemarks = string.Empty;
                        errTemp = "Error in remarks";
                        temp2 = ds.Tables[0].Rows[i]["Tgr_StatusCode"].ToString().Trim();
                        if (temp2.Trim() != string.Empty)
                        {
                            strRemarks = temp2.Trim().Substring(0, 1);

                        }
                        else
                        {
                            strRemarks = "N";
                        }

                        errTemp = "Error in hire /sep date";
                        if (ds.Tables[0].Rows[i]["Tgr_CheckDate"].ToString().Trim().Replace("/", "") != string.Empty
                            && strRemarks != "N")
                        {
                            temp2 = ds.Tables[0].Rows[i]["Tgr_CheckDate"].ToString().Trim().Replace("/", "");
                            temp += temp2.Substring(4, 4) + temp2.Substring(0, 2) + temp2.Substring(2, 2);
                        }
                        else
                        {
                            temp += "        ";
                        }
                        errTemp = "Error in remarks";
                        if (strRemarks.Trim() != string.Empty
                            && strRemarks.Trim() != "N")
                        {                      
                            temp += strRemarks;
                        }
                               //1234567890
                        temp += "        ";

                        text += temp + "\r\n";
                        ++DetailCounter;
                    }
                    catch
                    {
                        DataRow dr = DSEXEMPTIONS.Tables[0].NewRow();
                        dr["Lastname"] = ds.Tables[0].Rows[i]["Mem_LastName"].ToString();
                        dr["Firstname"] = ds.Tables[0].Rows[i]["Mem_FirstName"].ToString();
                        dr["Remarks"] = errTemp;
                        DSEXEMPTIONS.Tables[0].Rows.Add(dr);
                    }

                }
            }

            #endregion

            #region trailer

            string Trail = string.Empty;
            string errTrail = string.Empty;

            try
            {
                errTrail = "Error in record id trail";
                Trail = "99   ";

                errTrail = "Error in totals";
                int monthIndicator = Convert.ToInt32(MonthYear.Substring(4, 2));

                string temp3 = string.Empty;
                string temp4 = string.Empty;

                temp2 = string.Format("{0:0.00}", TotalSSS);
                temp3 = string.Format("{0:0.00}", TotalPhil);
                temp4 = string.Format("{0:0.00}", TotalEC);

                int len = temp2.Length;
                for (int idx = 0; idx < (12 - len); idx++)
                    Trail += " ";

                Trail += temp2;

                Trail += "        0.00";

                len = temp4.Length;
                for (int idx = 0; idx < (12 - len); idx++)
                    Trail += " ";

                Trail += temp4;
                 
                        //12345678901234567890
                Trail += "               ";

                text += Trail;

            }
            catch
            {
                DataRow dr = DSEXEMPTIONS.Tables[0].NewRow();
                dr["Lastname"] = "SSS Premium Trail";
                dr["Firstname"] = "Trail";
                dr["Remarks"] = errTrail;
                DSEXEMPTIONS.Tables[0].Rows.Add(dr);
                //Error += "\r\n";
                //Error += "Error in Trailer ";
                //Error += errTrail;
            }


            #endregion

            return text;
        }

        #endregion

        #region Philhealth

        //OK
        private string PhilHealthGov(DataSet ds)
        {
            string text = string.Empty;
            int DetailCounter = 0;
            Error = string.Empty;
            string batchNumber = string.Empty;
            double TotalContribEE = 0;
            double TotalContribER = 0;
            DataSet dsExemp = DSEXEMPTIONS;
            DataSet dsComp = GetCompanyInfo();
            string temp2 = string.Empty;
            #region header

            string header = string.Empty;
            string errtemp1 = string.Empty;
            try
            {
                header = "REMITTANCE REPORT";
                header += "\r\n";

                errtemp1 = "Error in Employer name";
                temp2 = dsComp.Tables[0].Rows[0]["Mcm_CompanyName"].ToString().Trim();
                if (temp2.Length < 60)
                {
                    int len = temp2.Length;
                    header += temp2;
                    //for (int idx = 0; idx < (60 - len); idx++)
                    //    header += " ";
                }
                else
                {
                    header += temp2.Substring(0, 60);
                }
                header += "\r\n";

                errtemp1 = "Error in Employer Address";
                if (dsComp.Tables[0].Rows[0]["Address1"].ToString() != string.Empty)
                    temp2 = dsComp.Tables[0].Rows[0]["Address1"].ToString().Trim();
                if (dsComp.Tables[0].Rows[0]["Address2"].ToString().Trim() != string.Empty)
                    temp2 += dsComp.Tables[0].Rows[0]["Address2"].ToString().Trim();
                if (dsComp.Tables[0].Rows[0]["Address3"].ToString().Trim() != string.Empty)
                    temp2 += dsComp.Tables[0].Rows[0]["Address3"].ToString().Trim();

                header += temp2;
                header += "\r\n";

                errtemp1 = "Error in Company Philhealth Number";
                header += dsComp.Tables[0].Rows[0]["Mcm_PhilhealthNo"].ToString().Trim().Replace("-", "").Replace(" ", "");
                temp2 = string.Empty;

                 errtemp1 = "Error in report quarter";
                int quarter = Convert.ToInt32( MonthYear.Substring(4, 2));
                if (quarter <= 3)
                    temp2 = "1";
                else if (quarter <= 6)
                    temp2 = "2";
                else if (quarter <= 9)
                    temp2 = "3";
                else if (quarter <= 12)
                    temp2 = "4";
                header += temp2 + MonthYear.Substring(0, 4) + "R";

                header += "\r\n" + "MEMBERS";

                text += header;
                text += "\r\n";

            }
            catch
            {
                DataRow dr = DSEXEMPTIONS.Tables[0].NewRow();
                dr["Lastname"] = "Phlhealth Header";
                dr["Firstname"] = "Header";
                dr["Remarks"] = errtemp1;
                DSEXEMPTIONS.Tables[0].Rows.Add(dr);
                //Error += "\r\n";
                //Error += "Error in header ";
                //Error += errtemp1;
            }

            #endregion

            #region details
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                string temp = string.Empty;
                int j = 0;
                for (; j < dsExemp.Tables[0].Rows.Count
                        && (
                        dsExemp.Tables[0].Rows[j]["Lastname"].ToString() != ds.Tables[0].Rows[i]["Mem_LastName"].ToString()
                        ||
                        dsExemp.Tables[0].Rows[j]["Firstname"].ToString() != ds.Tables[0].Rows[i]["Mem_FirstName"].ToString()
                        )
                    ; j++) ;
                if (j == dsExemp.Tables[0].Rows.Count)
                {
                    string errTemp = string.Empty;
                    try
                    {
                        temp2 = string.Empty;

                        errTemp = "Error in Philhealth Number";
                        temp2 = ds.Tables[0].Rows[i]["Mem_PhilhealthNo"].ToString().Trim().Replace("-", "").Replace(" ", "");
                        if (temp2.Length < 12)
                        {
                            int len = temp2.Length;
                            temp += temp2;
                            for (int idx = 0; idx < (12 - len); idx++)
                                temp += " ";
                        }
                        else
                        {
                            temp += temp2.Substring(0, 12);
                        }
                        
                        errTemp = "Error in Last name";
                        temp2 = ds.Tables[0].Rows[i]["Mem_LastName"].ToString().Replace("Ñ", "N").Replace(".", "").Trim().ToUpper();
                        if (temp2.Length < 30)
                        {
                            temp += temp2;
                            int len = temp2.Length;
                            for (int idx = 0; idx < (30 - len); idx++)
                                temp += " ";
                        }
                        else
                        {
                            temp += temp2.Substring(0, 30);
                        }
                        temp2 = string.Empty;

                        errTemp = "Error in First name";
                        temp2 = ds.Tables[0].Rows[i]["Mem_FirstName"].ToString().Replace("Ñ", "N").Replace(".", "").Trim().ToUpper();
                        if (temp2.Length < 30)
                        {
                            temp += temp2;
                            int len = temp2.Length;
                            for (int idx = 0; idx < (30 - len); idx++)
                                temp += " ";
                        }
                        else
                        {
                            temp += temp2.Substring(0, 30);
                        }
                        temp2 = string.Empty;

                        errTemp = "Error in Middlename";
                        if (ds.Tables[0].Rows[i]["Mem_MiddleName"].ToString().Trim() != string.Empty)
                        {
                            temp += ds.Tables[0].Rows[i]["Mem_MiddleName"].ToString().Trim().ToUpper().Substring(0, 1);
                        }
                        else
                        {
                            temp += " ";
                        }

                        errTemp = "Error in Employee Salary";
                        temp2 = ds.Tables[0].Rows[i]["Mem_Salary"].ToString().Trim().Replace(".", "");

                        if (temp2.Length < 8)
                        {
                            int len = temp2.Length;
                            for (int idx = 0; idx < (8 - len); idx++)
                                temp += "0";
                            temp += temp2;
                        }
                        else
                        {
                            temp += temp2.Substring(0, 8);
                        }


                        errTemp = "Error in PHILHEALTH PREM Values";
                        int monthIndicator = Convert.ToInt32(MonthYear.Substring(4, 2));
                        string temp3 = string.Empty;
                        temp2 = ds.Tables[0].Rows[i]["Tgr_EEShare"].ToString().Trim().Replace(".", "");
                        temp3 = ds.Tables[0].Rows[i]["Tgr_ERShare"].ToString().Trim().Replace(".", "");

                        TotalContribEE += Convert.ToDouble(ds.Tables[0].Rows[i]["Tgr_EEShare"].ToString().Trim());
                        TotalContribER += Convert.ToDouble(ds.Tables[0].Rows[i]["Tgr_ERShare"].ToString().Trim());

                        if (temp2.Length < 6)
                        {
                            int len = temp2.Length;
                            temp2 = string.Empty;
                            for (int idx = 0; idx < (6 - len); idx++)
                                temp2 += "0";
                            temp2 += ds.Tables[0].Rows[i]["Tgr_EEShare"].ToString().Trim().Replace(".", "");
                        }
                        if (temp3.Length < 6)
                        {
                            int len = temp3.Length;
                            temp3 = string.Empty;
                            for (int idx = 0; idx < (6 - len); idx++)
                                temp3 += "0";
                            temp3 += ds.Tables[0].Rows[i]["Tgr_ERShare"].ToString().Trim().Replace(".", "");
                        }


                        if (monthIndicator == 1 ||
                            monthIndicator == 4 ||
                            monthIndicator == 7 ||
                            monthIndicator == 10)
                        {
                            temp += temp2 + temp3 + "000000000000000000000000";
                        }
                        else if (monthIndicator == 2 ||
                            monthIndicator == 5 ||
                            monthIndicator == 8 ||
                            monthIndicator == 11)
                        {
                            temp += "000000000000" + temp2 + temp3 + "000000000000";
                        }
                        else if (monthIndicator == 3 ||
                           monthIndicator == 6 ||
                           monthIndicator == 9 ||
                           monthIndicator == 12)
                        {
                            temp += "000000000000000000000000" + temp2 + temp3;
                        }

                        errTemp = "Error in Status";
                        if (ds.Tables[0].Rows[i]["Tgr_StatusCode"].ToString().Trim() != string.Empty)
                        {
                            temp += ds.Tables[0].Rows[i]["Tgr_StatusCode"].ToString().Trim().Substring(0, 2);
                        }
                        else
                        {
                            temp += "  ";
                        }
                        errTemp = "Error in Hire / Sep Date";
                        if (ds.Tables[0].Rows[i]["Tgr_StatusDate"].ToString().Trim() != string.Empty)
                        {
                            temp += ds.Tables[0].Rows[i]["Tgr_StatusDate"].ToString().Trim().Replace("/", "");
                        }
                        else
                        {
                            temp += "        ";
                        }

                        text += temp + "\r\n";
                        ++DetailCounter;
                    }
                    catch
                    {
                        DataRow dr = DSEXEMPTIONS.Tables[0].NewRow();
                        dr["Lastname"] = ds.Tables[0].Rows[i]["Mem_LastName"].ToString();
                        dr["Firstname"] = ds.Tables[0].Rows[i]["Mem_FirstName"].ToString();
                        dr["Remarks"] = errTemp;
                        DSEXEMPTIONS.Tables[0].Rows.Add(dr);
                    }

                }
            }

            #endregion

            #region trailer

            string Trail = string.Empty;
            string errTrail = string.Empty;

            try
            {
                Trail = "M5-SUMMARY";
                Trail += "\r\n";

                int monthIndicator = Convert.ToInt32(MonthYear.Substring(4, 2));

                if (monthIndicator == 1 ||
                    monthIndicator == 4 ||
                    monthIndicator == 7 ||
                    monthIndicator == 10)
                {
                    Trail += "1";
                }
                else if (monthIndicator == 2 ||
                    monthIndicator == 5 ||
                    monthIndicator == 8 ||
                    monthIndicator == 11)
                {
                    Trail += "2";
                }
                else if (monthIndicator == 3 ||
                   monthIndicator == 6 ||
                   monthIndicator == 9 ||
                   monthIndicator == 12)
                {
                    Trail += "3";
                }
                string temp3 = string.Empty;
                string temp4 = string.Empty;

                errTrail = "Error in Total";
                temp3 = string.Format("{0:0.00}", TotalContribEE + TotalContribER);

                if (temp3.Trim().Replace(".", "").Length < 8)
                {
                    int len = temp3.Trim().Replace(".", "").Length;
                    for (int idx = 0; idx < (8 - len); idx++)
                        Trail += "0";
                    Trail += temp3.Trim().Replace(".", "");                    
                }
                else
                {
                    Trail += temp3.Trim().Replace(".", "").Substring(0, 8);
                }

                errTrail = "Error in PBR Number";
                using (DALHelper dal = new DALHelper())
                {
                    dal.OpenDB();
                    DataSet dsSBR = dal.ExecuteDataSet(
                        string.Format(@"
                        select * from T_MonthlySBR
                        where Tms_DeductionCode = 'PHICPREM'				
                        and Tms_YearMonth = '{0}'
                        ", MonthYear)
                        , CommandType.Text
                        );
                    dal.CloseDB();
                    if (dsSBR.Tables[0].Rows.Count > 0)
                    {
                        temp2 = dsSBR.Tables[0].Rows[0]["Tms_ReceiptNo"].ToString().Trim().Replace("/", "").Replace(" ", "");
                        if (temp2.Length < 15)
                        {
                            int len = temp2.Length;
                            Trail += temp2;
                            for (int idx = 0; idx < (15 - len); idx++)
                                Trail += " ";
                        }
                        else
                        {
                            Trail += temp2.Substring(0, 15);
                        }

                        Trail += dsSBR.Tables[0].Rows[0]["Tms_PaymentDate"].ToString().Trim().Replace("/", "").Replace(" ", "").Substring(0, 8);
                    }
                    else
                    {
                        //throw new Exception();
                               // 123456789012345678901234567890
                        Trail += "                       ";
                    }
                }
                Trail += DetailCounter;

                Trail += "\r\n";
                Trail += "GRAND TOTAL";

                temp3 = string.Format("{0:0.00}", TotalContribEE + TotalContribER);
                temp3 = temp3.Trim().Replace(".", "");
                for (int idx = 0; idx < (12 - temp3.Length); idx++)
                {
                    Trail += "0";
                }
                Trail += temp3 + "\r\n";
                //*BAJA start
                //CommonBL cmbl = new CommonBL();

                //errTrail = "Error in Signatory";
                //DataSet dsSig = cmbl.GetSignatory2(Menucode);
                //if (dsSig.Tables[0].Rows.Count > 0)
                //{
                //    temp2 = dsSig.Tables[0].Rows[0]["Last Name"].ToString() + ", " + dsSig.Tables[0].Rows[0]["First Name"].ToString();
                //    if (temp2.Length < 40)
                //    {
                //        int len = temp2.Length;
                //        Trail += temp2;
                //        for (int idx = 0; idx < (40 - len); idx++)
                //            Trail += " ";
                //    }
                //    else
                //    {
                //        Trail += temp2.Substring(0, 40);
                //    }
                //    temp4 = dsSig.Tables[0].Rows[0]["Position"].ToString().Trim().ToUpper();
                //    if (temp4.Length < 20)
                //    {
                //        Trail += temp4;
                //        for (int idx = 0; idx < (20 - temp4.Length); idx++)
                //            Trail += " ";
                //    }
                //    else
                //    {
                //        Trail += dsSig.Tables[0].Rows[0]["Position"].ToString().Trim().ToUpper().Substring(0, 20);
                //    }
                //}
                //else
                //{
                //    throw new Exception();
                //}
                //*BAJA end

                text += Trail;

            }
            catch
            {
                DataRow dr = DSEXEMPTIONS.Tables[0].NewRow();
                dr["Lastname"] = "Philhealth Trail";
                dr["Firstname"] = "Trail";
                dr["Remarks"] = errTrail;
                DSEXEMPTIONS.Tables[0].Rows.Add(dr);
                Error += "\r\n";
                Error += "Error in Trailer ";
                Error += errTrail;
            }


            #endregion

            return text;
        }

        //OK
        private string PhilHealthUnionBank(DataSet ds)
        {
            string text = string.Empty;
            int DetailCounter = 0;
            Error = string.Empty;
            string batchNumber = string.Empty;
            double TotalContribEE = 0;
            double TotalContribER = 0;
            DataSet dsExemp = DSEXEMPTIONS;
            DataSet dsComp = GetCompanyInfo();
            string temp2 = string.Empty;
            #region header

            string header = string.Empty;
            string errtemp1 = string.Empty;
            try
            {
                header = "REMITTANCE REPORT";
                header += "\r\n";

                errtemp1 = "Error in Employer name";
                temp2 = dsComp.Tables[0].Rows[0]["Mcm_CompanyName"].ToString().Trim();
                if (temp2.Length < 60)
                {
                    int len = temp2.Length;
                    header += temp2;
                    //for (int idx = 0; idx < (60 - len); idx++)
                    //    header += " ";
                }
                else
                {
                    header += temp2.Substring(0, 60);
                }
                header += "\r\n";

                errtemp1 = "Error in Employer Address";
                if (dsComp.Tables[0].Rows[0]["Address1"].ToString() != string.Empty)
                    temp2 = dsComp.Tables[0].Rows[0]["Address1"].ToString().Trim();
                if (dsComp.Tables[0].Rows[0]["Address2"].ToString().Trim() != string.Empty)
                    temp2 += dsComp.Tables[0].Rows[0]["Address2"].ToString().Trim();
                if (dsComp.Tables[0].Rows[0]["Address3"].ToString().Trim() != string.Empty)
                    temp2 += dsComp.Tables[0].Rows[0]["Address3"].ToString().Trim();
                if (temp2.Length < 100)
                {
                    header += temp2;
                    int len = temp2.Length;
                    for (int idx = 0; idx < (100 - len); idx++)
                        header += " ";
                }
                else
                {
                    header += temp2.Substring(0, 100);
                }
                header += "\r\n";

                errtemp1 = "Error in Company Philhealth Number";
                header += dsComp.Tables[0].Rows[0]["Mcm_PhilhealthNo"].ToString().Trim().Replace("-", "").Replace(" ", "");
                temp2 = string.Empty;

                errtemp1 = "Error in report quarter";
                int quarter = Convert.ToInt32(MonthYear.Substring(4, 2));
                if (quarter <= 3)
                    temp2 = "1";
                else if (quarter <= 6)
                    temp2 = "2";
                else if (quarter <= 9)
                    temp2 = "3";
                else if (quarter <= 12)
                    temp2 = "4";
                header += temp2 + MonthYear.Substring(0, 4) + "R";

                header += "\r\n" + "MEMBERS";

                text += header;
                text += "\r\n";

            }
            catch
            {
                DataRow dr = DSEXEMPTIONS.Tables[0].NewRow();
                dr["Lastname"] = "Philhealth Header";
                dr["Firstname"] = "Header";
                dr["Remarks"] = errtemp1;
                DSEXEMPTIONS.Tables[0].Rows.Add(dr);
                //Error += "\r\n";
                //Error += "Error in header ";
                //Error += errtemp1;
            }

            #endregion

            #region details
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                string temp = string.Empty;
                int j = 0;
                for (; j < dsExemp.Tables[0].Rows.Count
                        && (
                        dsExemp.Tables[0].Rows[j]["Lastname"].ToString() != ds.Tables[0].Rows[i]["Mem_LastName"].ToString()
                        ||
                        dsExemp.Tables[0].Rows[j]["Firstname"].ToString() != ds.Tables[0].Rows[i]["Mem_FirstName"].ToString()
                        )
                    ; j++) ;
                if (j == dsExemp.Tables[0].Rows.Count)
                {
                    string errTemp = string.Empty;
                    try
                    {
                        temp2 = string.Empty;

                        errTemp = "Error in Philhealth Number";
                        temp2 = ds.Tables[0].Rows[i]["Mem_PhilhealthNo"].ToString().Trim().Replace("-", "").Replace(" ", "");
                        if (temp2.Length < 12)
                        {
                            int len = temp2.Length;
                            temp += temp2;
                            for (int idx = 0; idx < (12 - len); idx++)
                                temp += " ";
                        }
                        else
                        {
                            temp += temp2.Substring(0, 12);
                        }

                        errTemp = "Error in Last name";
                        temp2 = ds.Tables[0].Rows[i]["Mem_LastName"].ToString().Replace("Ñ", "N").Trim().ToUpper();
                        if (temp2.Length < 30)
                        {
                            temp += temp2;
                            int len = temp2.Length;
                            for (int idx = 0; idx < (30 - len); idx++)
                                temp += " ";
                        }
                        else
                        {
                            temp += temp2.Substring(0, 30);
                        }
                        temp2 = string.Empty;

                        errTemp = "Error in First name";
                        temp2 = ds.Tables[0].Rows[i]["Mem_FirstName"].ToString().Replace("Ñ", "N").Trim().ToUpper();
                        if (temp2.Length < 30)
                        {
                            temp += temp2;
                            int len = temp2.Length;
                            for (int idx = 0; idx < (30 - len); idx++)
                                temp += " ";
                        }
                        else
                        {
                            temp += temp2.Substring(0, 30);
                        }
                        temp2 = string.Empty;

                        errTemp = "Error in Middlename";
                        if (ds.Tables[0].Rows[i]["Mem_MiddleName"].ToString().Trim() != string.Empty)
                        {
                            temp += ds.Tables[0].Rows[i]["Mem_MiddleName"].ToString().Trim().ToUpper().Substring(0, 1);
                        }
                        else
                        {
                            temp += " ";
                        }

                        errTemp = "Error in Employee Salary";
                        temp2 = ds.Tables[0].Rows[i]["Mem_Salary"].ToString().Trim().Replace(".", "");

                        if (temp2.Length < 8)
                        {
                            int len = temp2.Length;
                            for (int idx = 0; idx < (8 - len); idx++)
                                temp += "0";
                            temp += temp2;
                        }
                        else
                        {
                            temp += temp2.Substring(0, 8);
                        }


                        errTemp = "Error in PHILHEALTH PREM Values";
                        int monthIndicator = Convert.ToInt32(MonthYear.Substring(4, 2));
                        string temp3 = string.Empty;
                        temp2 = ds.Tables[0].Rows[i]["Tgr_EEShare"].ToString().Trim().Replace(".", "");
                        temp3 = ds.Tables[0].Rows[i]["Tgr_ERShare"].ToString().Trim().Replace(".", "");

                        TotalContribEE += Convert.ToDouble(ds.Tables[0].Rows[i]["Tgr_EEShare"].ToString().Trim());
                        TotalContribER += Convert.ToDouble(ds.Tables[0].Rows[i]["Tgr_ERShare"].ToString().Trim());

                        if (temp2.Length < 6)
                        {
                            int len = temp2.Length;
                            temp2 = string.Empty;
                            for (int idx = 0; idx < (6 - len); idx++)
                                temp2 += "0";
                            temp2 += ds.Tables[0].Rows[i]["Tgr_EEShare"].ToString().Trim().Replace(".", "");
                        }
                        if (temp3.Length < 6)
                        {
                            int len = temp3.Length;
                            temp3 = string.Empty;
                            for (int idx = 0; idx < (6 - len); idx++)
                                temp3 += "0";
                            temp3 += ds.Tables[0].Rows[i]["Tgr_ERShare"].ToString().Trim().Replace(".", "");
                        }


                        if (monthIndicator == 1 ||
                            monthIndicator == 4 ||
                            monthIndicator == 7 ||
                            monthIndicator == 10)
                        {
                            temp += temp2 + temp3 + "000000000000000000000000";
                        }
                        else if (monthIndicator == 2 ||
                            monthIndicator == 5 ||
                            monthIndicator == 8 ||
                            monthIndicator == 11)
                        {
                            temp += "000000000000" + temp2 + temp3 + "000000000000";
                        }
                        else if (monthIndicator == 3 ||
                           monthIndicator == 6 ||
                           monthIndicator == 9 ||
                           monthIndicator == 12)
                        {
                            temp += "000000000000000000000000" + temp2 + temp3;
                        }

                        errTemp = "Error in Status";
                        temp2 = string.Empty;
                        temp2 = ds.Tables[0].Rows[i]["Tgr_StatusCode"].ToString().Trim();
                        if (temp2 == string.Empty)
                        {
                            temp += "  ";
                        }
                        else
                        {
                            temp += ds.Tables[0].Rows[i]["Tgr_StatusCode"].ToString().Trim().Substring(0, 2);
                        }

                        errTemp = "Error in Hire / Sep Date";
                        temp += ds.Tables[0].Rows[i]["Tgr_StatusDate"].ToString().Trim().Replace("/", "");


                        text += temp + "\r\n";
                        ++DetailCounter;
                    }
                    catch
                    {
                        DataRow dr = DSEXEMPTIONS.Tables[0].NewRow();
                        dr["Lastname"] = ds.Tables[0].Rows[i]["Mem_LastName"].ToString();
                        dr["Firstname"] = ds.Tables[0].Rows[i]["Mem_FirstName"].ToString();
                        dr["Remarks"] = errTemp;
                        DSEXEMPTIONS.Tables[0].Rows.Add(dr);
                    }

                }
            }

            #endregion

            #region trailer

            string Trail = string.Empty;
            string errTrail = string.Empty;

            try
            {
                Trail = "M5-SUMMARY";
                Trail += "\r\n";

                int monthIndicator = Convert.ToInt32(MonthYear.Substring(4, 2));

                if (monthIndicator == 1 ||
                    monthIndicator == 4 ||
                    monthIndicator == 7 ||
                    monthIndicator == 10)
                {
                    Trail += "1";
                }
                else if (monthIndicator == 2 ||
                    monthIndicator == 5 ||
                    monthIndicator == 8 ||
                    monthIndicator == 11)
                {
                    Trail += "2";
                }
                else if (monthIndicator == 3 ||
                   monthIndicator == 6 ||
                   monthIndicator == 9 ||
                   monthIndicator == 12)
                {
                    Trail += "3";
                }
                string temp3 = string.Empty;
                string temp4 = string.Empty;

                errTrail = "Error in Total";
                temp3 = string.Format("{0:0.00}", TotalContribEE + TotalContribER);

                if (temp3.Trim().Replace(".", "").Length < 8)
                {
                    int len = temp3.Trim().Replace(".", "").Length;
                    for (int idx = 0; idx < (8 - len); idx++)
                        Trail += "0";
                    Trail += temp3.Trim().Replace(".", "");
                }
                else
                {
                    Trail += temp3.Trim().Replace(".", "").Substring(0, 8);
                }

                errTrail = "Error in PBR Number";
                        //12345678901234567890
                Trail += "               ";

                errTrail = "Error in PRB Date";
                Trail += dateGenerate.ToString("MMddyyyy");

                if (DetailCounter < 100)
                {
                    Trail += string.Format("{0:000}", DetailCounter);
                }
                else
                {
                    Trail += DetailCounter.ToString();
                }

                Trail += "\r\n";
                Trail += "GRAND TOTAL";

                temp3 = string.Format("{0:0.00}", TotalContribEE + TotalContribER);
                temp3 = temp3.Trim().Replace(".", "");
                for (int idx = 0; idx < (10 - temp3.Length); idx++)
                {
                    Trail += "0";
                }
                Trail += temp3 + "\r\n";
                Trail += "NONE                                    NONE                ";
                //CommonBL cmbl = new CommonBL();

                //errTrail = "Error in Signatory";
                //DataSet dsSig = cmbl.GetSignatory(Menucode);
                //if (dsSig.Tables[0].Rows.Count > 0)
                //{
                //    temp2 = dsSig.Tables[0].Rows[0]["Last Name"].ToString() + ", " + dsSig.Tables[0].Rows[0]["First Name"].ToString();
                //    if (temp2.Length < 40)
                //    {
                //        int len = temp2.Length;
                //        Trail += temp2.ToUpper();
                //        for (int idx = 0; idx < (40 - len); idx++)
                //            Trail += " ";
                //    }
                //    else
                //    {
                //        Trail += temp2.Substring(0, 40);
                //    }
                //    temp4 = dsSig.Tables[0].Rows[0]["Position"].ToString().Trim().ToUpper();
                //    if (temp4.Length < 20)
                //    {
                //        Trail += temp4;
                //        for (int idx = 0; idx < (20 - temp4.Length); idx++)
                //            Trail += " ";
                //    }
                //    else
                //    {
                //        Trail += dsSig.Tables[0].Rows[0]["Position"].ToString().Trim().ToUpper().Substring(0, 20);
                //    }
                //}
                //else
                //{
                //    throw new Exception();
                //}

                text += Trail;

            }
            catch
            {
                DataRow dr = DSEXEMPTIONS.Tables[0].NewRow();
                dr["Lastname"] = "Philhealth Trail";
                dr["Firstname"] = "Trail";
                dr["Remarks"] = errTrail;
                DSEXEMPTIONS.Tables[0].Rows.Add(dr);
                //Error += "\r\n";
               // Error += "Error in Trailer ";
                //Error += errTrail;
            }


            #endregion

            return text;
        }

        public DataSet GetPhilheatlthReport(string query)
        {
            DataSet ds = null;

            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(query, CommandType.Text);
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        DataSet dsBracket = dal.ExecuteDataSet(@"
                                select 
	                                ROW_NUMBER() over (order by Mps_CompensationFrom)  [Bracket]
	                                ,Mps_EEShare
                                    ,Mps_CompensationFrom
                                from M_PremiumSchedule
                                where Mps_DeductionCode = 'PHICPREM'
                                order by Mps_CompensationFrom asc
                        ", CommandType.Text);
                        if (dsBracket != null && dsBracket.Tables[0].Rows.Count > 0)
                        {
                            for (int idx = 0; idx < ds.Tables[0].Rows.Count; idx++)
                            {
                                double d1 = Convert.ToDouble(ds.Tables[0].Rows[idx]["Tgr_EEShare"]);
                                string bracket = "1";
                                string SalaryRate = "4000.00";
                                for (int i1 = 0; i1 < dsBracket.Tables[0].Rows.Count; i1++)
                                {
                                    double d2 = Convert.ToDouble(dsBracket.Tables[0].Rows[i1]["Mps_EEShare"]);
                                    if (d1 == d2)
                                    {
                                        bracket = dsBracket.Tables[0].Rows[i1]["Bracket"].ToString();
                                        SalaryRate = dsBracket.Tables[0].Rows[i1]["Mps_CompensationFrom"].ToString();
                                    }
                                }
                                ds.Tables[0].Rows[idx]["SalaryBracket"] = bracket;
                                ds.Tables[0].Rows[idx]["Mem_Salary"] = SalaryRate;
                            }
                        }
                        else
                        {
                            throw new Exception("Salary Bracket not yet setup!");
                        }
                    }
                }
                catch(Exception er)
                {
                    CommonProcedures.showMessageError(er.Message);
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            //ds.WriteXmlSchema(@"d:\RF1.XSD");
            return ds;
        }

        public string GetSBRPhilhealth(string payperiod)
        {
            string ret = string.Empty;
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    DataSet ds = dal.ExecuteDataSet(string.Format(@"
                        select Tms_ReceiptNo from T_MonthlySBR
                        where Tms_DeductionCode = 'PHICPREM'
                        and Tms_YearMonth = '{0}'
                    ", payperiod));
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        ret = ds.Tables[0].Rows[0][0].ToString().Trim();
                    }
                }
                catch (Exception er)
                {
                    CommonProcedures.showMessageError(er.Message);
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return ret;
        }

        #endregion

        #region Pag ibig

        private string PagIbigGov(DataSet ds)
        {
            string text = string.Empty;
            int DetailCounter = 0;
            Error = string.Empty;
            string batchNumber = string.Empty;
            double TotalContribEE = 0;
            double TotalContribER = 0;
            DataSet dsExemp = DSEXEMPTIONS;
            DataSet dsComp = GetCompanyInfo();
            string temp2 = string.Empty;
            #region header

            string header = string.Empty;
            string errtemp1 = string.Empty;
            try
            {
                header = "EH";

                errtemp1 = "Error in header branch code";
                header += dsComp.Tables[0].Rows[0]["Mcm_BankBranchCode"].ToString().Substring(0, 2);

                header += MonthYear;

                errtemp1 = "Error in header SSS";
                temp2 = dsComp.Tables[0].Rows[0]["Mcm_SSSNo"].ToString().Trim().Replace("-", "");
                if (temp2.Length < 15)
                {
                    int len = temp2.Length;
                    header += temp2;
                    for (int idx = 0; idx < (15 - len); idx++)
                        header += " ";
                }
                else
                {
                    header += temp2.Substring(0, 15);
                }

                errtemp1 = "Error in Employer Type, Paytype";
                header += param[1].Value.ToString() + param[0].Value.ToString();

                errtemp1 = "Error in Employer name";
                temp2 = dsComp.Tables[0].Rows[0]["Mcm_CompanyName"].ToString().Trim();
                if (temp2.Length < 100)
                {
                    int len = temp2.Length;
                    header += temp2;
                    for (int idx = 0; idx < (100 - len); idx++)
                       header += " ";
                }
                else
                {
                    header += temp2.Substring(0, 100);
                }

                errtemp1 = "Error in Employer Address";
                if (dsComp.Tables[0].Rows[0]["Address1"].ToString() != string.Empty)
                    temp2 = dsComp.Tables[0].Rows[0]["Address1"].ToString().Trim();
                if (dsComp.Tables[0].Rows[0]["Address2"].ToString().Trim() != string.Empty)
                    temp2 += dsComp.Tables[0].Rows[0]["Address2"].ToString().Trim();
                if (dsComp.Tables[0].Rows[0]["Address3"].ToString().Trim() != string.Empty)
                    temp2 += dsComp.Tables[0].Rows[0]["Address3"].ToString().Trim();

                
                if (temp2.Length < 107)
                {
                    header += temp2;
                    for (int idx = 0; idx < (100 - temp2.Length); idx++)
                        header += " ";
                }
                else
                {
                    header += temp2.Substring(0, 100);
                }

                temp2 = dsComp.Tables[0].Rows[0]["Mcm_CompanyAddress3"].ToString().Trim();
                if (temp2.Length < 7)
                {
                    header += temp2;
                    for (int idx = 0; idx < (7 - temp2.Length); idx++)
                        header += " ";
                }
                else
                {
                    header += temp2.Substring(0, 7);
                }

                if (dsComp.Tables[0].Rows[0]["Mcm_TelNo"].ToString().Trim().Replace("-", "").Length < 15)
                {
                    int len = dsComp.Tables[0].Rows[0]["Mcm_TelNo"].ToString().Trim().Replace("-", "").Length;
                    header += dsComp.Tables[0].Rows[0]["Mcm_TelNo"].ToString().Trim().Replace("-", "");
                    
                }
                else
                {
                    header += dsComp.Tables[0].Rows[0]["Mcm_TelNo"].ToString().Trim().Replace("-", "").Substring(0, 15);
                }

                text += header;
                text += "\r\n";

            }
            catch
            {
                DataRow dr = DSEXEMPTIONS.Tables[0].NewRow();
                dr["Lastname"] = "PagIbig Header";
                dr["Firstname"] = "Header";
                dr["Remarks"] = errtemp1;
                DSEXEMPTIONS.Tables[0].Rows.Add(dr);
                //Error += "\r\n";
                //Error += "Error in header ";
                //Error += errtemp1;
            }

            #endregion

            #region details
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                string temp = string.Empty;
                int j = 0;
                for (; j < dsExemp.Tables[0].Rows.Count
                        && (
                        dsExemp.Tables[0].Rows[j]["Lastname"].ToString() != ds.Tables[0].Rows[i]["Mem_LastName"].ToString()
                        ||
                        dsExemp.Tables[0].Rows[j]["Firstname"].ToString() != ds.Tables[0].Rows[i]["Mem_FirstName"].ToString()
                        )
                    ; j++) ;
                if (j == dsExemp.Tables[0].Rows.Count)
                {
                    string errTemp = string.Empty;
                    try
                    {
                        temp2 = string.Empty;

                        temp = "DT";

                        errTemp = "Error in Pagibig Number";
                        temp2 = ds.Tables[0].Rows[i]["Mem_PagIbigNo"].ToString();
                        if (temp2 == string.Empty)
                        {
                                   //123456789012345678901234567890
                            temp += "            ";
                        }
                        else if (temp2.Trim().Replace("-", "").Replace(" ", "").Length < 12)
                        {
                            int len = temp2.Trim().Replace("-", "").Replace(" ", "").Length;
                            temp += temp2;
                            for (int idx = 0; idx < (12 - len); idx++)
                                temp += " ";
                        }
                        else
                        {
                            temp += temp2.Trim().Replace("-", "").Replace(" ", "").Substring(0, 12);
                        }

                        errTemp = "Error in Employee Number";
                        temp2 = ds.Tables[0].Rows[i]["Mem_IDNo"].ToString().Trim().Replace("-", "").Replace(" ", "");
                        if (temp2.Length < 15)
                        {
                            int len = temp2.Length;
                            temp += temp2;
                            for (int idx = 0; idx < (15 - len); idx++)
                                temp += " ";
                        }
                        else
                        {
                            temp += temp2.Substring(0, 15);
                        }


                        errTemp = "Error in Last name";
                        temp2 = ds.Tables[0].Rows[i]["Mem_LastName"].ToString().Replace("Ñ", "N").Trim().ToUpper();
                        if (temp2.Length < 30)
                        {
                            temp += temp2;
                            int len = temp2.Length;
                            for (int idx = 0; idx < (30 - len); idx++)
                                temp += " ";
                        }
                        else
                        {
                            temp += temp2.Substring(0, 30);
                        }
                        temp2 = string.Empty;

                        errTemp = "Error in First name";
                        temp2 = ds.Tables[0].Rows[i]["Mem_FirstName"].ToString().Replace("Ñ", "N").Trim().ToUpper();
                        if (temp2.Length < 30)
                        {
                            temp += temp2;
                            int len = temp2.Length;
                            for (int idx = 0; idx < (30 - len); idx++)
                                temp += " ";
                        }
                        else
                        {
                            temp += temp2.Substring(0, 30);
                        }
                        temp2 = string.Empty;

                        errTemp = "Error in Middle name";
                        temp2 = ds.Tables[0].Rows[i]["Mem_MiddleName"].ToString().Trim().ToUpper();
                        if (temp2 == string.Empty)
                        { 
                                   //123456789012345678901234567890
                            temp += "                              ";
                        }
                        else if (temp2.Length < 30)
                        {
                            temp += temp2;
                            int len = temp2.Length;
                            for (int idx = 0; idx < (30 - len); idx++)
                                temp += " ";
                        }
                        else
                        {
                            temp += temp2.Substring(0, 30);
                        }
                        temp2 = string.Empty;

                        errTemp = "Error in PagIbig share Values";
                        int monthIndicator = Convert.ToInt32(MonthYear.Substring(4, 2));
                        string temp3 = string.Empty;
                        temp2 = ds.Tables[0].Rows[i]["Tgr_EEShare"].ToString().Trim();
                        temp3 = ds.Tables[0].Rows[i]["Tgr_ERShare"].ToString().Trim();

                        TotalContribEE += Convert.ToDouble(ds.Tables[0].Rows[i]["Tgr_EEShare"].ToString().Trim());
                        TotalContribER += Convert.ToDouble(ds.Tables[0].Rows[i]["Tgr_ERShare"].ToString().Trim());

                        if (temp2.Length < 13)
                        {
                            int len = temp2.Length;
                            for (int idx = 0; idx < (13 - len); idx++)
                                temp2 += " ";
                            temp += temp2;
                        }
                        if (temp3.Length < 13)
                        {
                            int len = temp3.Length;
                            for (int idx = 0; idx < (13 - len); idx++)
                                temp3 += " ";
                            temp += temp3;
                        }

                        errTemp = "Error in TIN number";
                        temp2 = ds.Tables[0].Rows[i]["Mem_TIN"].ToString().Trim().Replace("-", "");
                        if (temp2 == string.Empty)
                        {
                                   //123456789012345678901234567890
                            temp += "               ";
                        }
                        else if (temp2.Length < 15)
                        {
                            temp += temp2;
                            int len = temp2.Length;
                            for (int idx = 0; idx < (15 - len); idx++)
                                temp += " ";
                        }
                        else
                        {
                            temp += temp2.Substring(0, 15);
                        }
                        temp2 = string.Empty;

                        errTemp = "Error in Birth Date";
                        temp += ds.Tables[0].Rows[i]["Mem_BirthDate"].ToString().Substring(0, 8);


                        text += temp + "\r\n";
                        ++DetailCounter;
                    }
                    catch
                    {
                        DataRow dr = DSEXEMPTIONS.Tables[0].NewRow();
                        dr["Lastname"] = ds.Tables[0].Rows[i]["Mem_LastName"].ToString();
                        dr["Firstname"] = ds.Tables[0].Rows[i]["Mem_FirstName"].ToString();
                        dr["Remarks"] = errTemp;
                        DSEXEMPTIONS.Tables[0].Rows.Add(dr);
                    }

                }
            }

            #endregion

            return text;
        }

        private string PagIbigUnionBank(DataSet ds)
        {
            string text = string.Empty;
            int DetailCounter = 0;
            Error = string.Empty;
            string batchNumber = string.Empty;
            double TotalContribEE = 0;
            double TotalContribER = 0;
            DataSet dsExemp = DSEXEMPTIONS;
            DataSet dsComp = GetCompanyInfo();
            string temp2 = string.Empty;
            #region header

            string header = string.Empty;
            string errtemp1 = string.Empty;
            try
            {
                header = "EH";

                errtemp1 = "Error in header branch code";
                header += dsComp.Tables[0].Rows[0]["Mcm_BankBranchCode"].ToString().Substring(0, 2);

                header += MonthYear;

                errtemp1 = "Error in header SSS";
                temp2 = dsComp.Tables[0].Rows[0]["Mcm_SSSNo"].ToString().Trim().Replace("-", "");
                if (temp2.Length < 15)
                {
                    int len = temp2.Length;
                    header += temp2;
                    for (int idx = 0; idx < (15 - len); idx++)
                        header += " ";
                }
                else
                {
                    header += temp2.Substring(0, 15);
                }

                errtemp1 = "Error in Employer Type, Paytype";
                header += param[1].Value.ToString() + param[0].Value.ToString();

                errtemp1 = "Error in Employer name";
                temp2 = dsComp.Tables[0].Rows[0]["Mcm_CompanyName"].ToString().Trim();
                if (temp2.Length < 100)
                {
                    int len = temp2.Length;
                    header += temp2;
                    for (int idx = 0; idx < (100 - len); idx++)
                        header += " ";
                }
                else
                {
                    header += temp2.Substring(0, 100);
                }

                errtemp1 = "Error in Employer Address";
                if (dsComp.Tables[0].Rows[0]["Address1"].ToString() != string.Empty)
                    temp2 = dsComp.Tables[0].Rows[0]["Address1"].ToString().Trim();
                if (dsComp.Tables[0].Rows[0]["Address2"].ToString().Trim() != string.Empty)
                    temp2 += dsComp.Tables[0].Rows[0]["Address2"].ToString().Trim();
                
                if (temp2.Length < 100)
                {
                    header += temp2;
                    for (int idx = 0; idx < (100 - temp2.Length); idx++)
                        header += " ";
                }
                else
                {
                    header += temp2.Substring(0, 100);
                }

                if (dsComp.Tables[0].Rows[0]["Mcm_CompanyAddress3"].ToString().Trim() != string.Empty)
                {
                    temp2 += dsComp.Tables[0].Rows[0]["Mcm_CompanyAddress3"].ToString().Trim();
                    header += temp2;
                    for (int idx = 0; idx < (7 - temp2.Length); idx++)
                        header += " ";
                }
                else
                {
                    header += "       ";
                }

                if (dsComp.Tables[0].Rows[0]["Mcm_TelNo"].ToString().Trim().Replace("-", "").Length < 15)
                {
                    header += dsComp.Tables[0].Rows[0]["Mcm_TelNo"].ToString().Trim().Replace("-", "");
                }
                else
                {
                    header += dsComp.Tables[0].Rows[0]["Mcm_TelNo"].ToString().Trim().Replace("-", "").Substring(0, 15);
                }

                text += header;
                text += "\r\n";

            }
            catch
            {
                DataRow dr = DSEXEMPTIONS.Tables[0].NewRow();
                dr["Lastname"] = "PagIbig Header";
                dr["Firstname"] = "Header";
                dr["Remarks"] = errtemp1;
                DSEXEMPTIONS.Tables[0].Rows.Add(dr);
                Error += "\r\n";
                Error += "Error in header ";
                Error += errtemp1;
            }

            #endregion

            #region details
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                string temp = string.Empty;
                int j = 0;
                for (; j < dsExemp.Tables[0].Rows.Count
                        && (
                        dsExemp.Tables[0].Rows[j]["Lastname"].ToString() != ds.Tables[0].Rows[i]["Mem_LastName"].ToString()
                        ||
                        dsExemp.Tables[0].Rows[j]["Firstname"].ToString() != ds.Tables[0].Rows[i]["Mem_FirstName"].ToString()
                        )
                    ; j++) ;
                if (j == dsExemp.Tables[0].Rows.Count)
                {
                    string errTemp = string.Empty;
                    try
                    {
                        temp2 = string.Empty;

                        temp = "DT";

                        errTemp = "Error in Pagibig Number";
                        temp2 = ds.Tables[0].Rows[i]["Mem_PagIbigNo"].ToString().Trim().Replace("-", "").Replace(" ", "");
                        if (temp2 == string.Empty)
                        {
                            //123456789012345678901234567890
                            temp += "            ";
                        }
                        else if (temp2.Length < 12)
                        {
                            int len = temp2.Length;
                            temp += temp2;
                            for (int idx = 0; idx < (12 - len); idx++)
                                temp += " ";
                        }
                        else
                        {
                            temp += temp2.Substring(0, 12);
                        }

                        errTemp = "Error in Employee Number";
                        temp2 = ds.Tables[0].Rows[i]["Mem_IDNo"].ToString().Trim().Replace("-", "").Replace(" ", "");
                        if (temp2.Length < 15)
                        {
                            int len = temp2.Length;
                            temp += temp2;
                            for (int idx = 0; idx < (15 - len); idx++)
                                temp += " ";
                        }
                        else
                        {
                            temp += temp2.Substring(0, 15);
                        }


                        errTemp = "Error in Last name";
                        temp2 = ds.Tables[0].Rows[i]["Mem_LastName"].ToString().Replace("Ñ", "N").Trim().ToUpper();
                        if (temp2.Length < 30)
                        {
                            temp += temp2;
                            int len = temp2.Length;
                            for (int idx = 0; idx < (30 - len); idx++)
                                temp += " ";
                        }
                        else
                        {
                            temp += temp2.Substring(0, 30);
                        }
                        temp2 = string.Empty;

                        errTemp = "Error in First name";
                        temp2 = ds.Tables[0].Rows[i]["Mem_FirstName"].ToString().Replace("Ñ", "N").Trim().ToUpper();
                        if (temp2.Length < 30)
                        {
                            temp += temp2;
                            int len = temp2.Length;
                            for (int idx = 0; idx < (30 - len); idx++)
                                temp += " ";
                        }
                        else
                        {
                            temp += temp2.Substring(0, 30);
                        }
                        temp2 = string.Empty;

                        errTemp = "Error in Middle name";
                        temp2 = ds.Tables[0].Rows[i]["Mem_MiddleName"].ToString().Trim().ToUpper();
                        if (temp2 == string.Empty)
                        {
                            //123456789012345678901234567890
                            temp += "                              ";
                        }
                        else if (temp2.Length < 30)
                        {
                            temp += temp2;
                            int len = temp2.Length;
                            for (int idx = 0; idx < (30 - len); idx++)
                                temp += " ";
                        }
                        else
                        {
                            temp += temp2.Substring(0, 30);
                        }
                        temp2 = string.Empty;

                        errTemp = "Error in PagIbig share Values";
                        int monthIndicator = Convert.ToInt32(MonthYear.Substring(4, 2));
                        string temp3 = string.Empty;
                        temp2 = ds.Tables[0].Rows[i]["Tgr_EEShare"].ToString().Trim();
                        temp3 = ds.Tables[0].Rows[i]["Tgr_ERShare"].ToString().Trim();

                        TotalContribEE += Convert.ToDouble(ds.Tables[0].Rows[i]["Tgr_EEShare"].ToString().Trim());
                        TotalContribER += Convert.ToDouble(ds.Tables[0].Rows[i]["Tgr_ERShare"].ToString().Trim());

                        if (temp2.Length < 13)
                        {
                            int len = temp2.Length;
                            for (int idx = 0; idx < (13 - len); idx++)
                                temp2 += " ";
                            temp += temp2;
                        }
                        if (temp3.Length < 13)
                        {
                            int len = temp3.Length;
                            for (int idx = 0; idx < (13 - len); idx++)
                                temp3 += " ";
                            temp += temp3;
                        }

                        errTemp = "Error in TIN number";
                        temp2 = ds.Tables[0].Rows[i]["Mem_TIN"].ToString().Trim().Replace("-", "");
                        if (temp2 == string.Empty)
                        {
                            //123456789012345678901234567890
                            temp += "               ";
                        }
                        else if (temp2.Length < 15)
                        {
                            temp += temp2;
                            int len = temp2.Length;
                            for (int idx = 0; idx < (15 - len); idx++)
                                temp += " ";
                        }
                        else
                        {
                            temp += temp2.Substring(0, 15);
                        }
                        temp2 = string.Empty;

                        errTemp = "Error in Birth Date";
                        temp += ds.Tables[0].Rows[i]["Mem_BirthDate"].ToString().Substring(0, 8);


                        text += temp + "\r\n";
                        ++DetailCounter;
                    }
                    catch
                    {
                        DataRow dr = DSEXEMPTIONS.Tables[0].NewRow();
                        dr["Lastname"] = ds.Tables[0].Rows[i]["Mem_LastName"].ToString();
                        dr["Firstname"] = ds.Tables[0].Rows[i]["Mem_FirstName"].ToString();
                        dr["Remarks"] = errTemp;
                        DSEXEMPTIONS.Tables[0].Rows.Add(dr);
                        
                    }

                }
            }

            #endregion

            return text;
        }

        public DataTable GetPagibigExcel(string query)
        {
            DataTable dtret = new DataTable();
            dtret.Columns.Add("EYENO", typeof(string));
            dtret.Columns.Add("HDMFID", typeof(string));
            dtret.Columns.Add("EYERID", typeof(string));
            dtret.Columns.Add("LASTNAME", typeof(string));
            dtret.Columns.Add("FIRSTNAME", typeof(string));
            dtret.Columns.Add("MIDNAME", typeof(string));
            dtret.Columns.Add("PERCOV", typeof(string));
            dtret.Columns.Add("PFRNO", typeof(string));
            dtret.Columns.Add("BIRTHDATE", typeof(string));
            dtret.Columns.Add("TIN", typeof(string));
            dtret.Columns.Add("SSSNO", typeof(string));
            dtret.Columns.Add("EE", typeof(decimal));
            dtret.Columns.Add("ER", typeof(decimal));
            dtret.Columns.Add("APPLNO", typeof(string));
            dtret.Columns.Add("AMOUNT", typeof(decimal));
            dtret.Columns.Add("TAG", typeof(string));
            dtret.Columns.Add("REM", typeof(string));
            dtret.Columns.Add("LTYPE", typeof(string));


            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    DataSet ds = dal.ExecuteDataSet(query, CommandType.Text);
                    double d1 = 0;
                    double d2 = 0;
                    double d3 = 0;
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        for(int idx = 0; idx < ds.Tables[0].Rows.Count; idx++)
                        {
                            DataRow dr = dtret.NewRow();
                            dr["EYENO"] = ds.Tables[0].Rows[idx]["EYENO"].ToString();
                            dr["HDMFID"] = ds.Tables[0].Rows[idx]["HDMFID"].ToString();
                            dr["EYERID"] = ds.Tables[0].Rows[idx]["EYERID"].ToString();
                            dr["LASTNAME"] = ds.Tables[0].Rows[idx]["LASTNAME"].ToString();
                            dr["FIRSTNAME"] = ds.Tables[0].Rows[idx]["FIRSTNAME"].ToString();
                            dr["MIDNAME"] = ds.Tables[0].Rows[idx]["MIDNAME"].ToString();
                            dr["PERCOV"] = ds.Tables[0].Rows[idx]["PERCOV"].ToString();
                            dr["PFRNO"] = ds.Tables[0].Rows[idx]["PFRNO"].ToString();
                            dr["BIRTHDATE"] = ds.Tables[0].Rows[idx]["BIRTHDATE"].ToString();
                            dr["TIN"] = ds.Tables[0].Rows[idx]["TIN"].ToString();
                            dr["SSSNO"] = ds.Tables[0].Rows[idx]["SSSNO"].ToString();
                            dr["EE"] = ds.Tables[0].Rows[idx]["EE"].ToString();
                            dr["ER"] = ds.Tables[0].Rows[idx]["ER"].ToString();
                            dr["APPLNO"] = ds.Tables[0].Rows[idx]["APPLNO"].ToString();
                            dr["AMOUNT"] = ds.Tables[0].Rows[idx]["AMOUNT"].ToString();
                            dr["TAG"] = ds.Tables[0].Rows[idx]["TAG"].ToString();
                            dr["REM"] = ds.Tables[0].Rows[idx]["REM"].ToString();
                            dr["LTYPE"] = ds.Tables[0].Rows[idx]["LTYPE"].ToString();

                            dtret.Rows.Add(dr);
                            d1 += Convert.ToDouble(ds.Tables[0].Rows[idx]["EE"]);
                            d2 += Convert.ToDouble(ds.Tables[0].Rows[idx]["ER"]);
                            d3 += Convert.ToDouble(ds.Tables[0].Rows[idx]["AMOUNT"]);
                        }
                    }
                    if (dtret.Rows.Count > 0)
                    {
                        DataRow dr = dtret.NewRow();
                        dr["EYENO"] = string.Empty;
                        dr["HDMFID"] = string.Empty;
                        dr["EYERID"] = string.Empty;
                        dr["LASTNAME"] = string.Empty;
                        dr["FIRSTNAME"] = string.Empty;
                        dr["MIDNAME"] = string.Empty;
                        dr["PERCOV"] = string.Empty;
                        dr["PFRNO"] = string.Empty;
                        dr["BIRTHDATE"] = string.Empty;
                        dr["TIN"] = string.Empty;
                        dr["SSSNO"] = string.Empty;
                        dr["EE"] = d1;
                        dr["ER"] = d2;
                        dr["APPLNO"] = string.Empty;
                        dr["AMOUNT"] = d3;
                        dr["TAG"] = string.Empty;
                        dr["REM"] = string.Empty;
                        dr["LTYPE"] = string.Empty;
                        dtret.Rows.Add(dr);
                    }
                }
                catch(Exception er)
                {
                    CommonProcedures.showMessageError(er.Message);
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return dtret;
        }

        #endregion

        #endregion

        
        public DataSet GetPreparedBy(string prepare)
        {
            DataSet ds = new DataSet();

            #region query

            string query = string.Format(@"
                                SELECT [Muh_FirstName] + ' ' +
                                       case [Muh_MiddleName]
                                             when '' then ''
                                             else  left([Muh_MiddleName],1) + '. '
                                             end + [Muh_LastName] as [USER NAME]
                                     ,ISNULL(Mcd_Name, [Muh_JobTitle]) as [POSITION]
                                  FROM [M_UserHdr]
                                  left join M_CodeDtl
                                  on Mcd_Code = Muh_JobTitle 
                                  and Mcd_CodeType = 'POSITION'
                                 where [Muh_UserCode] = '{0}'
                           ", prepare);

            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(query, CommandType.Text);

                dal.CloseDB();
            }
            return ds;
        }

        public bool isOverseas()
        {
            bool ret = false;

            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    DataSet ds = dal.ExecuteDataSet(@"
                        select 
	                        case when Tsc_SetFlag = 0
		                        then 'YES'
		                        else 'NO'
		                        END [isOverseas]
                        from T_SettingControl
                        where Tsc_SettingCode = 'DOMPAY'");

                    if (ds != null
                        && ds.Tables[0].Rows.Count > 0
                        && ds.Tables[0].Rows[0]["isOverseas"].ToString().Trim() == "YES")
                    {
                        ret = true;
                    }

                }
                catch
                {

                }
                finally
                { 
                
                }
            }

            return ret;
        }
    }
}

