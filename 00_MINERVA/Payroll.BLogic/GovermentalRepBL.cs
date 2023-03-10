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
using System.Collections;

namespace Payroll.BLogic
{
    public class GovernmentalRepBL : BaseBL
    {
        private SaveFileDialog saveDialog = new SaveFileDialog();
        public string Error = string.Empty;
        //private DateTime dateGenerate;
        private string MonthYear = string.Empty;
        public string Menucode = string.Empty;

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
     

        public string FomatQuery(string prof, string TextType, string TextCategory)
        {
            string query = string.Empty;

            int indx = 0;
            if (TextType.IndexOf("ATM DISK") != -1)
                indx = 1;
            else if (TextType.IndexOf("SSS LOAN") != -1)
                indx = 2;
            else if (TextType.IndexOf("SSS CONTRIBUTION") != -1)
                indx = 3;
            else if (TextType.IndexOf("PHILHEALTH EMPLOYER'S") != -1)
                indx = 4;
            else if (TextType.IndexOf("HDMF MEMBERSHIP") != -1)
                indx = 5;

            switch (indx)
            {
                case 1:
                    #region
                    query = @"
                        select 
                            Mem_LastName,
                            Mem_FirstName,
                            Mem_MiddleName,
                            Tpy_PaymentMode,
                            Tpy_BankCode,
                            Tpy_BankAcctNo,
                            Tpy_NetAmt
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
                            Tpy_NetAmt
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
                            Tpy_NetAmt
                        From @PROF..T_EmpPayrollHst
                        left join @PROF..M_Employee
                        on Tpy_IDNo = Mem_IDNo
                        @CONDITIONS
                        ";
                    #endregion
                    break;
                case 2:
                    #region
                    query = @"
                    --select 
--                        REPLACE(Mem_SSSNo, '-', '')[Mem_SSSNo],
--                        SSS.Tgr_IDNo,
--                        Mem_LastName,
--                        Mem_FirstName,
--                        Mem_MiddleName,
--                        SSS.Tgr_Filler,
--                        SSS.Tgr_EEShare,
--                        isnull(SUBSTRING(RTRIM(SSS.Tgr_Filler),11,8),'00000000') [Loan Date],
--                        isnull(REPLICATE('0', 6 - LEN(SUBSTRING(rtrim(SSS.Tgr_Filler),10,7)))
--                        +SUBSTRING(rtrim(SSS.Tgr_Filler),10,6),'000000') [Loan Amount],
--                        '0000000' [Penalty Paid],
--                        SSS.Tgr_EEShare [Amort Amount],
--                        '0' [Amort Paid Sign],
--                        CASE WHEN(ISNULL(LEFT(SSS.Tgr_Filler,2),'99999')='SP')
--			                                                            THEN '2'
--			                                                            ELSE 'H'
--			                                                            END  [Remarks]
--                    From @PROF..T_EmpGovRemittance SSS
--                    left join @PROF..M_Employee
--                    on SSS.Tgr_IDNo = Mem_IDNo
--                    

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
						,ISNULL(SUBSTRING(SSS.Tgr_Filler, 23,10), space(10)) as 'Tgr_LoanAmount'
						,ISNULL(SUBSTRING(SSS.Tgr_Filler, 33,10), space(10)) as 'Tgr_PrincipalAmount'
                        ,'0000000' [Penalty Paid]
                        ,'0' [Amort Paid Sign]
                        ,case when Mdn_DeductionCode = 'SLERP'
							then '1'
						    else 'S'
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
						,ISNULL(SUBSTRING(SSS.Tgr_Filler, 13,10), space(10)) as 'Tgr_CheckDate'
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
                        Mem_Salary,
                        Tgr_EEShare,
                        Tgr_ERShare,
                        Tgr_Filler
                        ,ISNULL(SUBSTRING(SSS.Tgr_Filler, 1,2), space(2)) as 'Tgr_StatusCode'
						,ISNULL(SUBSTRING(SSS.Tgr_Filler, 3,10), space(10)) as 'Tgr_StatusDate'
                    	
                    From @PROF..T_EmpGovRemittance as SSS
                    left join @PROF..M_Employee
                    on Mem_IDNo = Tgr_IDNo
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

        public DataSet checkSSSLoan(string query)
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
                           ,[Lastname] varchar(20)
                           ,[Firstname] varchar(20)
                           ,[Middle Name] varchar(20)
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
					{0}                   
                    
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

        public DataSet checkSSSPrem(string query)
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
                           ,[Lastname] varchar(20)
                           ,[Firstname] varchar(20)
                           ,[Middle Name] varchar(20)
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
					{0}                   
                    
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
						or [SSS Prem]< 0
						or [EC Fund] < 0
						--or [Philhealth] <= 0
						
						
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

        public DataSet checkPHILHEALTHPrem(string query)
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
                            ,[Lastname] varchar(30)
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
					{0}                   
                   

                     select
						[Employee ID],
						[Lastname],
						[Firstname],
						[Philhealth Number],
						CONVERT(varchar(20), [Employee Share]) [Employee Share],
						CONVERT(varchar(20), [Employer Share]) [Employer Share]
                        ,case when [Philhealth Number] = 'APPLIED'
							then 'PHILHEALTH NUMBER = APPLIED'
							when [Philhealth Number] = 'PENDING'
							then 'PHILHEALTH NUMBER = PENDING'
							when [Philhealth Number] = ''
							then 'NO PHILHEALTH NUMBER'
							else ''
						end 
						+ 
						case when [Employee Share] = 0
							then 'Employee Share = 0'
							when [Employee Share] < 0
							then 'Employee Share < 0'
							else ''
						end 
						+
						case when [Employer Share] = 0
							then 'Employer Share = 0'
							when [Employer Share] < 0
							then 'Employer Share < 0'
							else ''
						end  [Remarks]
						From @Table
						where 
							[Philhealth Number] in ('', 'APPLIED', 'PENDING')
							or [Employee Share] < 0
							or [Employer Share] < 0
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

        public DataSet checkPAGIBIGPrem(string query)
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
                            ,[Lastname] varchar(30)
                            ,[Firstname] varchar(30)
                            ,[Middlename] varchar(20)
                            ,[PAG-IBIG Number] varchar(30)
                            ,[TIN Number] varchar(30)
                            ,[Birthday] varchar(20)
                            ,[Employee Share] decimal(8,2)
                            ,[Employer Share] decimal(8,2)
                        )

					insert into @Table
					{0}   
                        
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

        #endregion

 
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
	                                ,Mps_CompensationFrom
                                from M_PremiumSchedule T
                                where Mps_DeductionCode = 'PHICPREM'
                                    and Mps_PayCycle = (SELECT MAX(Mps_PayCycle)
													    FROM M_PremiumSchedule
													    WHERE Mps_DeductionCode = T.Mps_DeductionCode)
                                order by Mps_CompensationFrom asc
                        ", CommandType.Text);
                        if (dsBracket != null && dsBracket.Tables[0].Rows.Count > 0)
                        {
                            for (int idx = 0; idx < ds.Tables[0].Rows.Count; idx++)
                            {
                                double d1 = Convert.ToDouble(ds.Tables[0].Rows[idx]["Mem_Salary"]);
                                string bracket = "1";
                                for (int i1 = 0; i1 < dsBracket.Tables[0].Rows.Count; i1++)
                                {
                                    double d2 = Convert.ToDouble(dsBracket.Tables[0].Rows[i1]["Mps_CompensationFrom"]);
                                    if (d1 >= d2)
                                        bracket = dsBracket.Tables[0].Rows[i1]["Bracket"].ToString();
                                }
                                ds.Tables[0].Rows[idx]["SalaryBracket"] = bracket;
                            }
                        }
                        else
                        {
                            throw new Exception("Salary Bracket not yet setup!");
                        }

                        //ds.WriteXmlSchema(@"c:\RF1.XSD");
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
            return ds;
        }

        public DataSet GetPhilheatlthReportNew(string query, string CentralProfile)
        {
            DataSet ds = null;

            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(query, CommandType.Text);
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
            return ds;
        }

        /*rowFiller: specify total rows that will fit in each page to autofill the blank space with 
         *           blank rows if using the PageFooter and not the GroupFooter band. 
         */
        public DataSet GetGenericReport(string query, int rowFiller)
        {
            DataSet ds = null;

            using (DALHelper dal = new DALHelper(LoginInfo.getUser().CentralProfileName, false))
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(query, CommandType.Text);
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            ds.Tables[0].Rows[i]["RowCount"] = i + 1;
                        }

                        if (rowFiller > 0 && (ds.Tables[0].Rows.Count % rowFiller) != 0)
                        {
                            int cnt = ds.Tables[0].Rows.Count;
                            int i = (rowFiller - (ds.Tables[0].Rows.Count % rowFiller));

                            for (int m = 1; m <= i; m++)
                            {
                                ds.Tables[0].Rows.Add(ds.Tables[0].NewRow());
                                ds.Tables[0].Rows[cnt]["RowCount"] = cnt + 1;
                                cnt++;
                            }
                        }

                        //ds.WriteXmlSchema(@"c:\" + this.Menucode + ".XSD");
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
            return ds;
        }

        public DataSet GetGenericReport(string query)
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
                        //ds.WriteXmlSchema(@"c:\" + this.Menucode + ".XSD");
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
            return ds;
        }

        public DataSet GetHDMFReport(string query, bool isLoan)
        {
            DataSet ds = new DataSet();

            if (isLoan)
            {
                using (DALHelper dal = new DALHelper())
                {
                    try
                    {
                        dal.OpenDB();
                        ds = dal.ExecuteDataSet(query, CommandType.Text);
                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                        {
                            if ((ds.Tables[0].Rows.Count % 33) != 0)
                            {
                                int i = (33 - (ds.Tables[0].Rows.Count % 33));

                                for (int m = 1; m <= i; m++)
                                {
                                    ds.Tables[0].Rows.Add(ds.Tables[0].NewRow());
                                }
                            }

                            //ds.WriteXmlSchema(@"c:\" + this.Menucode + ".XSD");
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

            }
            else
            {
                using (DALHelper dal = new DALHelper())
                {
                    try
                    {
                        dal.OpenDB();
                        ds = dal.ExecuteDataSet(query, CommandType.Text);
                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                        {
                            if ((ds.Tables[0].Rows.Count % 33) != 0)
                            {
                                int i = (33 - (ds.Tables[0].Rows.Count % 33));

                                for (int m = 1; m <= i; m++)
                                {
                                    ds.Tables[0].Rows.Add(ds.Tables[0].NewRow());
                                }
                            }
                            //ds.WriteXmlSchema(@"c:\" + this.Menucode + ".XSD");
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

            }

            return ds;
        }

        public DataSet GetSSSReport(string query)
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
                        string Tms_ReceiptNo1 = string.Empty;
                        string Tms_PaymentDate1 = string.Empty;
                        decimal Tms_PaidAmount1 = 0;
                        //1st month select
                        if (ds.Tables[1].Rows.Count > 0)
                        {
                            Tms_ReceiptNo1 = ds.Tables[1].Rows[0]["Tms_ReceiptNo"].ToString().Trim();
                            Tms_PaymentDate1 = ds.Tables[1].Rows[0]["Tms_PaymentDate"].ToString().Trim();
                            Tms_PaidAmount1 = ds.Tables[1].Rows[0]["Tms_PaidAmount"].ToString().Trim() == "" ? 0 :
                                Convert.ToDecimal(ds.Tables[1].Rows[0]["Tms_PaidAmount"].ToString().Trim());
                        }

                        string Tms_ReceiptNo2 = string.Empty;
                        string Tms_PaymentDate2 = string.Empty;
                        decimal Tms_PaidAmount2 = 0;
                        //2nd month select
                        if (ds.Tables[2].Rows.Count > 0)
                        {
                            Tms_ReceiptNo2 = ds.Tables[2].Rows[0]["Tms_ReceiptNo"].ToString().Trim();
                            Tms_PaymentDate2 = ds.Tables[2].Rows[0]["Tms_PaymentDate"].ToString().Trim();
                            Tms_PaidAmount2 = ds.Tables[2].Rows[0]["Tms_PaidAmount"].ToString().Trim() == "" ? 0 :
                                Convert.ToDecimal(ds.Tables[2].Rows[0]["Tms_PaidAmount"].ToString().Trim());
                        }

                        string Tms_ReceiptNo3 = string.Empty;
                        string Tms_PaymentDate3 = string.Empty;
                        decimal Tms_PaidAmount3 = 0;
                        //3rd month select
                        if (ds.Tables[3].Rows.Count > 0)
                        {
                            Tms_ReceiptNo3 = ds.Tables[3].Rows[0]["Tms_ReceiptNo"].ToString().Trim();
                            Tms_PaymentDate3 = ds.Tables[3].Rows[0]["Tms_PaymentDate"].ToString().Trim();
                            Tms_PaidAmount3 = ds.Tables[3].Rows[0]["Tms_PaidAmount"].ToString().Trim() == "" ? 0
                                : Convert.ToDecimal(ds.Tables[3].Rows[0]["Tms_PaidAmount"].ToString().Trim());
                        }
                        string primarykey = string.Empty;
                        ArrayList indkeys = new ArrayList();
                        int x = 0;
                        int cnt = 1;
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            if (cnt <= 18)
                            {
                                cnt++;
                            }
                            else
                            {
                                x++;
                                indkeys.Add(primarykey + x);
                                cnt = 2;
                            }
                            if (i == ds.Tables[0].Rows.Count - 1)
                            {
                                x++;
                                indkeys.Add(primarykey + x);
                            }

                            ds.Tables[0].Rows[i]["rownum"] = i + 1;
                        }
                        //end set
                        cnt = 1;
                        int listcnt = 0;
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            if (cnt > 18)
                            {
                                listcnt++;
                                cnt = 1;
                            }
                            ds.Tables[0].Rows[i]["primarykey"] = indkeys[listcnt];
                            cnt++;
                            //place cheat here
                            ds.Tables[0].Rows[i]["Tms_ReceiptNo1"] = Tms_ReceiptNo1;
                            ds.Tables[0].Rows[i]["Tms_PaymentDate1"] = Tms_PaymentDate1;
                            ds.Tables[0].Rows[i]["Tms_PaidAmount1"] = Tms_PaidAmount1;

                            ds.Tables[0].Rows[i]["Tms_ReceiptNo2"] = Tms_ReceiptNo2;
                            ds.Tables[0].Rows[i]["Tms_PaymentDate2"] = Tms_PaymentDate2;
                            ds.Tables[0].Rows[i]["Tms_PaidAmount2"] = Tms_PaidAmount2;

                            ds.Tables[0].Rows[i]["Tms_ReceiptNo3"] = Tms_ReceiptNo3;
                            ds.Tables[0].Rows[i]["Tms_PaymentDate3"] = Tms_PaymentDate3;
                            ds.Tables[0].Rows[i]["Tms_PaidAmount3"] = Tms_PaidAmount3;

                            //end
                        }

                        cnt = ds.Tables[0].Rows.Count;

                        if ((ds.Tables[0].Rows.Count % 14) != 0)
                        {
                            int i = (14 - (ds.Tables[0].Rows.Count % 14));
                            for (int m = 1; m <= i; m++)
                            {
                                ds.Tables[0].Rows.Add(ds.Tables[0].NewRow());
                                ds.Tables[0].Rows[cnt]["rownum"] = cnt + 1;

                                cnt++;
                            }
                        }

                        //ds.WriteXmlSchema(@"c:\" + this.Menucode + ".XSD");
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

            return ds;
        }

        public string GetSBRPhilhealth(string PayCycle, string CompanyCode, string CentralProfile)
        {
            string ret = string.Empty;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                try
                {
                    dal.OpenDB();
                    DataSet ds = dal.ExecuteDataSet(string.Format(@"
                        SELECT Tms_ReceiptNo FROM T_MonthlySBR
                            WHERE Tms_DeductionCode = 'HDMFPREM'
                                AND Tms_YearMonth = '{0}'
                                AND Tms_CompanyCode = '{1}'
                    ", PayCycle, CompanyCode));
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        ret = ds.Tables[0].Rows[0][0].ToString().Trim();
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
            return ret;
        }

    }
}
