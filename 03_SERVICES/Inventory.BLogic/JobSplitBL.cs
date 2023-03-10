using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using CommonPostingLibrary;
using Posting.DAL;
using System.Configuration;
using System.Data.SqlClient;


namespace Posting.BLogic
{
    public class JobSplitBL : BaseBL
    {
        public JobSplitBL()
        {
        }
        #region override
        public override int Add(DataRow row)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override int Update(DataRow row)
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

        public void AddJobSplit(DataRow row, DALHelper dal)
        {
            #region query
            string qString = @"INSERT INTO T_JobSplitDetail
                                           (Jsd_ControlNo,
                                            Jsd_Seqno,
                                            Jsd_StartTime,
                                            Jsd_EndTime,
                                            Jsd_JobCode,
                                            Jsd_ClientJobNo,
                                            Jsd_PlanHours,
                                            Jsd_ActHours,
                                            Jsd_Status,                                            
                                            Usr_Login,
                                            Ludatetime)
                                     VALUES
                                           (@Jsd_ControlNo
                                            ,@Jsd_Seqno
                                            ,@Jsd_StartTime
                                            ,@Jsd_EndTime
                                            ,@Jsd_JobCode
                                            ,@Jsd_ClientJobNo
                                            ,@Jsd_PlanHours
                                            ,@Jsd_ActHours
                                            ,@Jsd_Status                                            
                                            ,@Usr_Login                                            
                                            ,Getdate())";

            #endregion
            try
            {
                ParameterInfo[] paramInfo = new ParameterInfo[10];
                paramInfo[0] = new ParameterInfo("@Jsd_ControlNo", row["Jsd_ControlNo"]);
                paramInfo[1] = new ParameterInfo("@Jsd_Seqno", row["Jsd_Seqno"]);
                paramInfo[2] = new ParameterInfo("@Jsd_StartTime", row["Jsd_StartTime"]);
                paramInfo[3] = new ParameterInfo("@Jsd_EndTime", row["Jsd_EndTime"]);
                paramInfo[4] = new ParameterInfo("@Jsd_JobCode", row["Jsd_JobCode"]);
                paramInfo[5] = new ParameterInfo("@Jsd_ClientJobNo", row["Jsd_ClientJobNo"]);
                paramInfo[6] = new ParameterInfo("@Jsd_PlanHours", row["Jsd_PlanHours"]);
                paramInfo[7] = new ParameterInfo("@Jsd_ActHours", row["Jsd_ActHours"]);
                paramInfo[8] = new ParameterInfo("@Jsd_Status", row["Jsd_Status"].ToString());
                paramInfo[9] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);
                dal.ExecuteNonQuery(qString, CommandType.Text, paramInfo);
            }
            catch (Exception e)
            {
                throw new PayrollException(e);
            }
        }
        public DataSet  getJobForPosting()
        {
            DataSet ds = new DataSet();
            string DBName1 = Encrypt.decryptText(ConfigurationManager.AppSettings["DBNameConfi"].ToString());
            string DBName2 = Encrypt.decryptText(ConfigurationManager.AppSettings["DBNameNonConfi"].ToString());
            string sql = @" declare @JobPlan as table
                            (
                            Employee varchar(15),
                            ProcessDate datetime,
                            JobCtrlNo varchar(12) 	
                            )

                            declare @Start datetime
                            declare @End datetime                        
                            DECLARE @CurrentBillingCycle as Char(6)                            

                            SET @CurrentBillingCycle = (SELECT DISTINCT bcn_billingyearmonth
							                            FROM T_BillingConfiguration
							                            WHERE Bcn_Indicator='C')
                            SET @Start = (SELECT Min(Bcn_StartCycle) as MinDate
						                            FROM T_BillingConfiguration
						                            WHERE Bcn_BillingYearMonth = @CurrentBillingCycle )

                            SET @End = (SELECT Max(Bcn_EndCycle) as MinDate
						                            FROM T_BillingConfiguration
						                            WHERE Bcn_BillingYearMonth = @CurrentBillingCycle )

                           
                            --get Overtime record

                            declare @OTTable as table
                            (   
                                OTStartTime char(4),
                                OTEndTime char(4),                                     
                                EmployeeId char(15),
                                OvertimeDate datetime	
                            )

                            declare @OTTableSub as table
                            (   
                                StartTimeSub char(4),
                                EndTimeSub char(4),                                     
                                EmployeeIdSub char(15),
                                OvertimeDateSub datetime	
                            )

                            --insert to subquery
                            insert @OTTableSub
                            select min(Eot_StartTime),'0000',Eot_EmployeeId,Eot_OvertimeDate
                                                                from t_employeeovertime
                                                                where eot_status in ('A','9')
										                              and (Eot_overtimedate between @Start and @End)
                            group by Eot_EmployeeId,Eot_OvertimeDate
                            union
                            select '0000',max(Eot_EndTime) ,Eot_EmployeeId,Eot_OvertimeDate
                                                                from t_employeeovertime
                                                                where eot_status in ('A','9')
										                              and (Eot_overtimedate between @Start and @End)
                            group by Eot_EmployeeId,Eot_OvertimeDate
                            union
                            select min(Eot_StartTime),'0000',Eot_EmployeeId,Eot_OvertimeDate
                                                                from t_employeeovertimehist
                                                                where eot_status in ('A','9')
										                              and (Eot_overtimedate between @Start and @End)
                            group by Eot_EmployeeId,Eot_OvertimeDate
                            union
                            select '0000',max(Eot_EndTime) ,Eot_EmployeeId,Eot_OvertimeDate
                                                                from t_employeeovertimehist
                                                                where eot_status in ('A','9')
										                              and (Eot_overtimedate between @Start and @End)
                            group by Eot_EmployeeId,Eot_OvertimeDate



                            --insert to Main OT
                            insert @OTTable
                            select  replicate('0',4-len(convert(varchar,sum(convert(int,StartTimeSub)))))+convert(varchar,sum(convert(int,StartTimeSub))),
                                    replicate('0',4-len(convert(varchar,sum(convert(int,EndTimeSub)))))+convert(varchar,sum(convert(int,EndTimeSub))),
                                    EmployeeIdSub,OvertimeDateSub 
                                    from @OTTableSub
                            group by EmployeeIdSub,OvertimeDateSub

                            --end

                            insert @JobPlan
                            select distinct Jsh_EmployeeId,Jsh_JobSplitDate,Jsh_ControlNo from T_jobsplitdetail 
                                      inner join T_jobsplitheader on jsh_controlno=jsd_controlno
                                   where jsd_status='9' 
                                          and jsh_jobsplitdate between @Start and @End
                            group by Jsh_EmployeeId,Jsh_JobSplitDate,Jsh_ControlNo
                            
                            --Select for the main Query
                            Select JobCtrlNo,Employee,Ell_DayCode,Ell_ActualTimeIn_1 as ActualTimeIn_1, Ell_ActualTimeOut_2 as ActualTimeOut_2,
                               (cast(substring(Ell_ActualTimeIn_1, 1, 2) as smallint) * 60) +
                               (cast(substring(Ell_ActualTimeIn_1, 3, 2) as smallint)) as Ell_ActualTimeIn_1,
                               (cast(substring(Ell_ActualTimeOut_1, 1, 2) as smallint) * 60) +
                               (cast(substring(Ell_ActualTimeOut_1, 3, 2) as smallint)) as Ell_ActualTimeOut_1,
                               (cast(substring(Ell_ActualTimeIn_2, 1, 2) as smallint) * 60) +
                               (cast(substring(Ell_ActualTimeIn_2, 3, 2) as smallint)) as Ell_ActualTimeIn_2,
                               (cast(substring(Ell_ActualTimeOut_2, 1, 2) as smallint) * 60) +
                               (cast(substring(Ell_ActualTimeOut_2, 3, 2) as smallint)) as Ell_ActualTimeOut_2,                               
                               case when OTStartTime <> Null then (cast(substring(OTStartTime, 1, 2) as smallint) * 60) +
									                               (cast(substring(OTStartTime, 3, 2) as smallint)) else 0 end as OTStart,
                               case when OTStartTime <> Null then (cast(substring(OTEndTime, 1, 2) as smallint) * 60) +
									                               (cast(substring(OTEndTime, 3, 2) as smallint)) else 0 end as OTEnd,Ell_flex                            	
                            from T_employeelogledger  
                            inner join @JobPlan on ell_employeeid=Employee and ell_processdate=ProcessDate
                            left join @OTTable on EmployeeId = Employee and overtimedate=Processdate 
                            where 
                            (Ell_ActualTimeIn_1<>'0000' and Ell_ActualTimeOut_2<>'0000')
                            or
                            (Ell_ActualTimeIn_1<>'0000' and Ell_ActualTimeOut_1<>'0000')
                            or
                            (Ell_ActualTimeIn_2<>'0000' and Ell_ActualTimeOut_2<>'0000')
                            union
                            Select JobCtrlNo,Employee,Ell_DayCode,Ell_ActualTimeIn_1 as ActualTimeIn_1, Ell_ActualTimeOut_2 as ActualTimeOut_2,
                               (cast(substring(Ell_ActualTimeIn_1, 1, 2) as smallint) * 60) +
                               (cast(substring(Ell_ActualTimeIn_1, 3, 2) as smallint)) as Ell_ActualTimeIn_1,
                               (cast(substring(Ell_ActualTimeOut_1, 1, 2) as smallint) * 60) +
                               (cast(substring(Ell_ActualTimeOut_1, 3, 2) as smallint)) as Ell_ActualTimeOut_1,
                               (cast(substring(Ell_ActualTimeIn_2, 1, 2) as smallint) * 60) +
                               (cast(substring(Ell_ActualTimeIn_2, 3, 2) as smallint)) as Ell_ActualTimeIn_2,
                               (cast(substring(Ell_ActualTimeOut_2, 1, 2) as smallint) * 60) +
                               (cast(substring(Ell_ActualTimeOut_2, 3, 2) as smallint)) as Ell_ActualTimeOut_2,                               
                               case when OTStartTime <> Null then (cast(substring(OTStartTime, 1, 2) as smallint) * 60) +
									                               (cast(substring(OTStartTime, 3, 2) as smallint)) else 0 end as OTStart,
                               case when OTStartTime <> Null then (cast(substring(OTEndTime, 1, 2) as smallint) * 60) +
									                               (cast(substring(OTEndTime, 3, 2) as smallint)) else 0 end as OTEnd,Ell_flex                            	
                            from T_employeelogledgerhist  
                            inner join @JobPlan on ell_employeeid=Employee and ell_processdate=ProcessDate
                            left join @OTTable on EmployeeId = Employee and overtimedate=Processdate 
                            where 
                            (Ell_ActualTimeIn_1<>'0000' and Ell_ActualTimeOut_2<>'0000')
                            or
                            (Ell_ActualTimeIn_1<>'0000' and Ell_ActualTimeOut_1<>'0000')
                            or
                            (Ell_ActualTimeIn_2<>'0000' and Ell_ActualTimeOut_2<>'0000')
                                
                                ";
            using (DALHelper dal = new DALHelper(false))
            {
                dal.OpenDB();
                try
                {
                    ds = dal.ExecuteDataSet(sql, CommandType.Text);
                }
                catch (Exception ex)
                {
                    throw new PayrollException("Error to fetch job splitting." + "\n" + ex.ToString());
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return ds;
        }
        public DataSet FetchJobDetail(string CtlrNo, DALHelper dal)
        {
            DataSet ds = new DataSet();
            string sql = @"                            
                            declare @shiftcode varchar(10)
                            set @shiftcode=
                            (
                            	
                            select case when EML.ell_shiftcode is null then (case when EMLH.ell_shiftcode is null then emt_shiftcode else EMLH.ell_shiftcode end) else EML.ell_shiftcode end from t_jobsplitheader 
		                                 left outer join t_employeelogledger as EML on EML.ell_employeeid=jsh_employeeid and EML.ell_processdate=jsh_Jobsplitdate
	                                     left outer join T_employeemaster on emt_employeeid=jsh_employeeid
										 left outer join t_employeelogledgerhist as EMLH on EMLH.ell_employeeid=jsh_employeeid and EMLH.ell_processdate=jsh_Jobsplitdate 		
                                   where jsh_controlno='{0}'
                            )

                            select *, 
                                     (cast(substring(Jsd_StartTime, 1, 2) as smallint) * 60) +
                                     (cast(substring(Jsd_StartTime, 3, 2) as smallint)) as TimeStart,
                                     (cast(substring(Jsd_EndTime, 1, 2) as smallint) * 60) +
                                     (cast(substring(Jsd_EndTime, 3, 2) as smallint)) as TimeEnd
	                        from T_jobsplitdetail 
                                    where jsd_controlno='{0}' and jsd_status='9' 
                            order by  Jsd_Seqno asc

                            --Get Maximum seqno and shift time end 
                            select max(Jsd_Seqno) as LastSeq,(cast(substring(scm_shifttimeout, 1, 2) as smallint) * 60) as shifttimeout,scm_shifttimeout from T_jobsplitdetail
                                      inner join t_shiftcodemaster on scm_shiftcode=@shiftcode	
                                    where jsd_controlno='{0}'
                            group by scm_shifttimeout

                            --Get Minimum seqno and shift time start
                            select min(Jsd_Seqno) as LastSeq,(cast(substring(scm_shifttimein, 1, 2) as smallint) * 60) + (cast(substring(scm_shifttimein, 3, 2) as smallint)) as shifttimein,
			                    (cast(substring(scm_shifttimeout, 1, 2) as smallint) * 60) + (cast(substring(scm_shifttimeOut, 3, 2) as smallint)) as shifttimeout,
                                scm_shifttimein,scm_shifttimeOut,
                               (cast(substring(Scm_ShiftBreakStart, 1, 2) as smallint) * 60) + 	(cast(substring(Scm_ShiftBreakStart, 3, 2) as smallint)) as BreakStart,
                               (cast(substring(Scm_ShiftBreakend, 1, 2) as smallint) * 60) + 	(cast(substring(Scm_ShiftBreakEnd, 3, 2) as smallint)) as BreakEnd,
                                Scm_ShiftBreakStart,Scm_ShiftBreakend			
                            from T_jobsplitdetail
                                  inner join t_shiftcodemaster on scm_shiftcode=@shiftcode
                                    where jsd_controlno='{0}'
                            group by scm_shifttimein,scm_shifttimeout,Scm_ShiftBreakStart,Scm_ShiftBreakend 
                            ";
            try
            {
                ds = dal.ExecuteDataSet(string.Format(sql, CtlrNo), CommandType.Text);
            }
            catch (Exception ex)
            {
                throw new PayrollException("Error upon fetch Job detail." + "\n" + ex.ToString());
            }
            return ds;
        }
        public int ResetLossTime(DALHelper dal)
        {
            int Val = 0;
            try
            {
                string sql = @"declare @startdate datetime
                                declare @enddate datetime
                                DECLARE @CurrentBillingCycle as Char(6)
                                DECLARE @CurrentMinDate as Datetime


                                SET @CurrentBillingCycle = (SELECT DISTINCT bcn_billingyearmonth
						                                    FROM T_BillingConfiguration
						                                    WHERE Bcn_Indicator='C')

                                SET @startdate = (SELECT Min(Bcn_StartCycle) as MinDate
					                                    FROM T_BillingConfiguration
					                                    WHERE Bcn_BillingYearMonth = @CurrentBillingCycle )
                                set @enddate =
                                (
                                  select convert(char(10),getdate(),101)
                                )

                                delete from T_JobSplitDetailLossTime
                                 where jsd_controlno in (select jsh_controlno from T_JobSplitDetailLossTime
                                 inner join t_jobsplitheaderLossTime on jsh_controlno=jsd_controlno
                                where jsh_jobsplitdate between @startdate and @enddate)

                                delete from t_jobsplitheaderLossTime where jsh_jobsplitdate between @startdate and @enddate
                                --resest all actual hrs in zero
                                update T_JobSplitDetail set jsd_acthours=0,Ludatetime=getdate() 
	                                from t_jobsplitheader inner join t_jobsplitdetail on jsd_controlno=jsh_controlno
	                                where jsh_jobsplitdate between @startdate and @enddate
                               ";
                Val = dal.ExecuteNonQuery(sql, CommandType.Text);
            }
            catch (Exception ex)
            {
                throw new PayrollException(ex);
            }

            return Val;
        }
        public void UpdateJobActualHr(string CtrlNo, string SeqNo, decimal ActualHr, DALHelper dal)
        {
            string sql = @"update T_jobsplitDetail set Jsd_ActHours={2},ludatetime=getdate() where Jsd_ControlNo='{0}' and Jsd_Seqno='{1}'";
            try
            {
                dal.ExecuteNonQuery(string.Format(sql, CtrlNo, SeqNo, ActualHr), CommandType.Text);
            }
            catch (Exception ex)
            {
                throw new PayrollException(ex);
            }
        }
        public DataSet LedgerRec()
        {
            DataSet ds = new DataSet();
            #region <SQL>
            string sql = @" DECLARE @NextBillingCycle as Char(6)
                            DECLARE @CurrentBillingCycle as Char(6)
                            DECLARE @CurrentMinDate as Datetime
                            DECLARE @CurrentMaxDate as Datetime                            

                            SET @CurrentBillingCycle = (SELECT DISTINCT bcn_billingyearmonth
							                            FROM T_BillingConfiguration
							                            WHERE Bcn_Indicator='C')
                            SET @CurrentMinDate = (SELECT Min(Bcn_StartCycle) as MinDate
						                          FROM T_BillingConfiguration
						                          WHERE Bcn_BillingYearMonth = @CurrentBillingCycle )

                            SET @CurrentMaxDate = convert(char(10),getdate(),101);                                                   

                            SET @NextBillingCycle = (SELECT DISTINCT convert(Char(6),dateadd(mm,1,convert(Datetime,  right(bcn_billingyearmonth,2) +'/01/'+ left(bcn_billingyearmonth,4))),112)
						                            FROM T_BillingConfiguration
						                            WHERE Bcn_Indicator='C')

                            declare @JobSplit as table
                            (     Jsd_ControlNo    Varchar(12)
	                            , Jsd_Seqno        Char(2)
	                            , Jsh_EmployeeID   Varchar(15)
	                            , Jsh_JobSplitDate Datetime
	                            , Jsd_JobCode      Varchar(10)
	                            , Jsd_ClientJobNo  Varchar(15)
	                            , Jsd_SubWorkCode  Varchar(10)
	                            , Jsd_PlanHours    Decimal(5,2)
	                            , Jsd_ActHours     Decimal(5,2)
	                            , Bcn_BillingCycle Varchar(10)
	                            , Bcn_StartCycle   Datetime
                                , Bcn_EndCycle     Datetime                                 
                            )

                            -- Job Split Transactions for posting of actual hours per billing cyle
                             INSERT INTO @JobSplit
                            SELECT Jsd_ControlNo 
                             , Jsd_Seqno
                             , Jsh_EmployeeID
                             , Jsh_JobSplitDate
                             , Jsd_JobCode 
                             , Jsd_ClientJobNo
                             , Jsd_SubWorkCode
                             , Jsd_PlanHours
                             , Jsd_ActHours
                         , Bcn_BillingCycle =null
                             , Bcn_StartCycle=null
                                , Bcn_EndCycle=null
                            FROM T_JobSplitDetail
                            INNER JOIN T_SalesMaster on Slm_DashJobCode = Jsd_JobCode
                                  and Slm_ClientJobNo = Jsd_ClientJobNo
                            INNER JOIN T_JobSplitHeader on jsh_controlno = jsd_controlno
                             and jsh_status = '9'
                             and jsh_jobsplitdate between @CurrentMinDate and @CurrentMaxDate
                            WHERE jsd_status = '9'


                            -- Determine the unsplitted hours on the specified date range
                            declare @Logs as table
                            (   Ell_EmployeeID Varchar(15),
	                            Ell_ProcessDate Datetime,
	                            Ell_daycode	Varchar(4),
	                            ell_ShiftCode Varchar(10),
                                Ell_EncodedOvertimeAdvHr decimal(5,2),
                                Ell_EncodedOvertimePostHr decimal(5,2),
	                            Ell_ActualTimeIn_1 Char(4),
	                            Ell_ActualTimeIn_2 Char(4),
	                            Ell_ActualTimeOuT_1 Char(4),
	                            Ell_ActualTimeOuT_2 Char(4),
								Ell_Flex bit,
                                Ell_Costcenter varchar(12),
                                Emt_jobstatus char(2)                                                                
                            )

                            INSERT INTO @Logs
                            SELECT Ell_EmployeeID,
	                            Ell_ProcessDate,
	                            Ell_DayCode,
	                            Ell_Shiftcode,
								Ell_EncodedOvertimeAdvHr,
								Ell_EncodedOvertimePostHr,
	                            case when isnull(Ell_ActualTimeIn_1,'0000')<>'0000' and Ell_ActualTimeIn_1>scm_shifttimeIn and ell_flex='True' and Ell_DayCode='REG' then scm_shifttimeIn else 
											(case when Ell_EncodedOvertimeAdvHr=0 and Ell_ActualTimeIn_1 < scm_shifttimeIn and isnull(Ell_ActualTimeIn_1,'0000')<>'0000' then scm_shifttimeIn else Ell_ActualTimeIn_1 end) end Ell_ActualTimeIn_1,
	                            case when isnull(Ell_ActualTimeIn_2,'0000')<>'0000' and Ell_ActualTimeIn_2>scm_shiftbreakend and ell_flex='True' and Ell_DayCode='REG' then substring(Ell_ActualTimeIn_2, 1, 2) + '00' else 
											 (case when Ell_EncodedOvertimeAdvHr=0 and Ell_ActualTimeIn_2 < scm_shifttimeIn and isnull(Ell_ActualTimeIn_2,'0000')<>'0000' then scm_shiftbreakend else Ell_ActualTimeIn_2 end) end Ell_ActualTimeIn_2,
	                            Ell_ActualTimeOuT_1,
	                            Ell_ActualTimeOuT_2,
								Ell_Flex,
                                Emt_CostCenterCode,
                                emt_jobstatus 
                            FROM T_EmployeeLogledger
                            inner join T_shiftcodemaster on Scm_ShiftCode = ell_shiftcode --and Scm_Status='A'
                            inner join T_EmployeeMaster on Emt_Employeeid = Ell_Employeeid
                            WHERE Ell_Processdate between @CurrentMinDate and @CurrentMaxDate 
		                            and ( ( Ell_ActualTimeIn_1 <> '0000' and Ell_ActualTimeOuT_1 <> '0000') or
			                              ( Ell_ActualTimeIn_2 <> '0000' and Ell_ActualTimeOuT_2 <> '0000') or
			                              ( Ell_ActualTimeIn_1 <> '0000' and Ell_ActualTimeOuT_2 <> '0000') or
			                              ( Ell_ActualTimeIn_1 <> '0000' and Ell_ActualTimeOuT_1 <> '0000'
				                            and Ell_ActualTimeIn_2 <> '0000' and Ell_ActualTimeOuT_2 <> '0000' ) ) 

                            UNION

                            SELECT Ell_EmployeeID,
	                            Ell_ProcessDate, 
	                            Ell_DayCode,
	                            Ell_Shiftcode,
								Ell_EncodedOvertimeAdvHr,
								Ell_EncodedOvertimePostHr,
	                            case when isnull(Ell_ActualTimeIn_1,'0000')<>'0000' and Ell_ActualTimeIn_1>scm_shifttimeIn and ell_flex='True' and Ell_DayCode='REG' then scm_shifttimeIn else 
											(case when Ell_EncodedOvertimeAdvHr=0 and Ell_ActualTimeIn_1 < scm_shifttimeIn and isnull(Ell_ActualTimeIn_1,'0000')<>'0000' then scm_shifttimeIn else Ell_ActualTimeIn_1 end) end Ell_ActualTimeIn_1,
	                            case when isnull(Ell_ActualTimeIn_2,'0000')<>'0000' and Ell_ActualTimeIn_2>scm_shiftbreakend and ell_flex='True' and Ell_DayCode='REG' then substring(Ell_ActualTimeIn_2, 1, 2) + '00' else 
											 (case when Ell_EncodedOvertimeAdvHr=0 and Ell_ActualTimeIn_2 < scm_shifttimeIn and isnull(Ell_ActualTimeIn_2,'0000')<>'0000' then scm_shiftbreakend else Ell_ActualTimeIn_2 end) end Ell_ActualTimeIn_2,
	                            Ell_ActualTimeOuT_1,
	                            Ell_ActualTimeOuT_2 ,
								Ell_Flex,
                                Emt_CostCenterCode,
                                emt_jobstatus  
                            FROM T_EmployeeLogledgerHist
                            inner join T_shiftcodemaster on Scm_ShiftCode = ell_shiftcode
                                inner join T_EmployeeMasterhist on Emt_Employeeid = Ell_Employeeid
                                and Ell_Payperiod = Emt_PayPeriod
                            WHERE Ell_Processdate between @CurrentMinDate and @CurrentMaxDate 
	                            and ( ( Ell_ActualTimeIn_1 <> '0000' and Ell_ActualTimeOuT_1 <> '0000') or
		                              ( Ell_ActualTimeIn_2 <> '0000' and Ell_ActualTimeOuT_2 <> '0000') or
		                              ( Ell_ActualTimeIn_1 <> '0000' and Ell_ActualTimeOuT_2 <> '0000') or
		                              ( Ell_ActualTimeIn_1 <> '0000' and Ell_ActualTimeOuT_1 <> '0000'
			                            and Ell_ActualTimeIn_2 <> '0000' and Ell_ActualTimeOuT_2 <> '0000') ) 



                            SELECT DISTINCT Jsd_ControlNo,
			                            Ell_Employeeid,
			                            convert(char(10),Ell_Processdate,101) as Ell_Processdate,
			                            Emt_JobStatus,
			                            ell_daycode,
			                            Scm_ShiftTimeIn,
			                            Scm_ShiftBreakStart,
			                            Scm_ShiftBreakEnd,
			                            Scm_ShiftTimeOut,
                                        scm_shifthours,	
                                        Ell_EncodedOvertimeAdvHr,
                                        Ell_EncodedOvertimePostHr,					
			                            Ell_ActualTimeIn_1,
			                            Ell_ActualTimeIn_2,
			                            Ell_ActualTimeOut_1,
			                            Ell_ActualTimeOut_2,
                                        case when ell_daycode<>'REG' and (cast(substring(Ell_ActualTimeIn_1, 3, 2) as smallint))<>0  then (cast(substring(Ell_ActualTimeIn_1, 1, 2) as smallint) * 60)+60 else
                                        (cast(substring(Ell_ActualTimeIn_1, 1, 2) as smallint) * 60) +
                                        (cast(substring(Ell_ActualTimeIn_1, 3, 2) as smallint)) end ConLogIn1,
                                         case when ell_daycode<>'REG' and (cast(substring(Ell_ActualTimeIn_2, 3, 2) as smallint))<>0  then  (cast(substring(Ell_ActualTimeIn_2, 1, 2) as smallint) * 60)+60 else  
										(cast(substring(Ell_ActualTimeIn_2, 1, 2) as smallint) * 60) +
                                        (cast(substring(Ell_ActualTimeIn_2, 3, 2) as smallint)) end  ConLogIn2,
                                         case when ell_flex='True' then 
                                                (cast(substring(Ell_ActualTimeOut_1, 1, 2) as smallint) * 60)
                                             else
										(cast(substring(Ell_ActualTimeOut_1, 1, 2) as smallint) * 60) +
                                        (cast(substring(Ell_ActualTimeOut_1, 3, 2) as smallint)) 
                                         end ConLogOut1,
										 case when ell_flex='True' then 
                                                (cast(substring(Ell_ActualTimeOut_2, 1, 2) as smallint) * 60)
                                             else
                                                (case when ell_daycode='REG' and 
                                                         (cast(substring(Ell_ActualTimeOut_2, 1, 2) as smallint) * 60)+
                                                         (cast(substring(Ell_ActualTimeOut_2, 3, 2) as smallint)) >       
                                                         (cast(substring(Scm_ShiftTimeOut, 1, 2) as smallint) * 60) +
                                                         (cast(substring(Scm_ShiftTimeOut, 3, 2) as smallint))                                                                    
                                                     then (cast(substring(Ell_ActualTimeOut_2, 1, 2) as smallint) * 60)
                                                     else (cast(substring(Ell_ActualTimeOut_2, 1, 2) as smallint) * 60)+
                                                         (cast(substring(Ell_ActualTimeOut_2, 3, 2) as smallint)) end)
                                         end  ConLogOut2,
                                        (cast(substring(Scm_ShiftTimeIn, 1, 2) as smallint) * 60) +
                                        (cast(substring(Scm_ShiftTimeIn, 3, 2) as smallint)) as ConShiftIn,
										(cast(substring(Scm_ShiftBreakStart, 1, 2) as smallint) * 60) +
                                        (cast(substring(Scm_ShiftBreakStart, 3, 2) as smallint)) as ConShiftBreakStart,
										(cast(substring(Scm_ShiftBreakEnd, 1, 2) as smallint) * 60) +
                                        (cast(substring(Scm_ShiftBreakEnd, 3, 2) as smallint)) as ConShiftBreakEnd,
										(cast(substring(Scm_ShiftTimeOut, 1, 2) as smallint) * 60) +
                                        (cast(substring(Scm_ShiftTimeOut, 3, 2) as smallint)) as ConShiftOut,Ell_Costcenter,ell_flex
                            			  
                            FROM @Logs
                            LEFT JOIN @JobSplit on Ell_Employeeid = jsh_employeeid 
	                            and Ell_Processdate = jsh_jobsplitdate
                            inner join T_shiftcodemaster on Scm_ShiftCode = ell_shiftcode                                                         
                            --where ell_employeeid ='000797'
                            order by Ell_Employeeid asc
                            
                            ";
            #endregion

            using (DALHelper dal = new DALHelper(false))
            {
                dal.OpenDB();
                try
                {
                    ds = dal.ExecuteDataSet(sql, CommandType.Text);
                }
                catch (Exception ex)
                {
                    throw new PayrollException("Error upon fetch Ledger record." + "\n" + ex.ToString());
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return ds;
        }
        public String GetLasSeries(DALHelper dal)
        {
            string series = string.Empty;
            string sql = @"declare @maxNum as int
                            set @maxNum=(select substring(max(jsh_controlno),4,9) from t_jobsplitheaderlosstime where substring(jsh_controlno,2,2)=right(year(getdate()),2))
                            select tcm_transactionprefix + right(year(getdate()),2)+ convert(varchar,(replicate('0',9-len(@maxNum + 1))+ convert(varchar,( @maxNum+ 1)))) from T_TransactionControlMaster where tcm_transactioncode='TMPJOBLOSS'
                           --update T_TransactionControlMaster set tcm_lastseries = tcm_lastseries + 1 where tcm_transactioncode='TMPJOBLOSS'
                           ";
            try
            {
                series = dal.ExecuteDataSet(sql, CommandType.Text).Tables[0].Rows[0][0].ToString().Trim();
            }
            catch (Exception ex)
            {
                throw new PayrollException("Error upon fetch Ledger record." + "\n" + ex.ToString());
            }
            return series;
        }
        public void InsertLossTime(string EmpId, string JobCtrl, string Jobdate, string ShiftIN, string ShiftOut, decimal ShiftHr, string seq, bool LossFlag, string costcenter, DALHelper dal)
        {
            string sql = @"                           
                            if(select count(jsh_controlno)from T_jobsplitheaderLossTime where jsh_controlno='{1}')=0
                            begin 
                                insert T_jobsplitheaderLossTime
                                select '{1}','{0}','{2}',getdate(),' ','{7}',NULL,NULL,NULL,NULL,NULL,NULL,NULL,' ',9,'sa',getdate()
                            end

                            if(select convert(int,case when max(Jsd_EndTime)is NULL then 0 else max(Jsd_EndTime) end) from T_jobsplitdetailLossTime where jsd_controlno='{1}')<convert(int,'{3}')
                            begin
                                insert T_jobsplitdetailLossTime
                                select '{1}','{6}','{3}','{4}','_DASH-9999','_DASH-9999','NOTSET',{5},{5},'9','sa',getdate()
                            end
                          ";
            try
            {
                dal.ExecuteNonQuery(string.Format(sql, EmpId, JobCtrl, Jobdate, ShiftIN, ShiftOut, ShiftHr, seq, costcenter), CommandType.Text);
            }
            catch (Exception ex)
            {
                throw new PayrollException("Error upon insert Jobsplit record." + "\n" + ex.ToString());
            }
        }
        public DataSet getJobPlan(string controlno, string shiftIN, string shiftOUT)
        {
            DataSet ds = new DataSet();
            string sql = @"select jsd_controlno
                            ,jsd_seqno
                            ,(cast(substring(jsd_starttime, 1, 2) as smallint) * 60) + (cast(substring(jsd_starttime, 3, 2) as smallint))
                            ,(cast(substring(jsd_endtime, 1, 2) as smallint) * 60) + (cast(substring(jsd_endtime, 3, 2) as smallint))
                              from t_jobsplitdetail where jsd_status='9' and jsd_controlno='{0}' and jsd_starttime <='{1}' and jsd_endtime <='{2}' 
                           order by jsd_seqno asc";
            using (DALHelper dal = new DALHelper(false))
            {
                dal.OpenDB();
                try
                {
                    dal.BeginTransactionSnapshot();
                    ds = dal.ExecuteDataSet(string.Format(sql, controlno, shiftIN, shiftOUT), CommandType.Text);
                    dal.CommitTransactionSnapshot();
                }
                catch (Exception ex)
                {
                    dal.RollBackTransactionSnapshot();
                    throw new PayrollException("Error upon Job plan record." + "\n" + ex.ToString());
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return ds;
        }
        public DataSet getOTJobPlan(string controlno, string shiftIN, string shiftOUT)
        {
            DataSet ds = new DataSet();
            string sql = @"select jsd_controlno
                            ,jsd_seqno
                            ,(cast(substring(jsd_starttime, 1, 2) as smallint) * 60) + (cast(substring(jsd_starttime, 3, 2) as smallint))
                            ,(cast(substring(jsd_endtime, 1, 2) as smallint) * 60) + (cast(substring(jsd_endtime, 3, 2) as smallint))
                              from t_jobsplitdetail where jsd_status='9' and jsd_controlno='{0}' and jsd_starttime <='{1}' and jsd_endtime <='{2}' 
                           order by jsd_seqno asc";
            using (DALHelper dal = new DALHelper(false))
            {
                dal.OpenDB();
                try
                {
                    dal.BeginTransactionSnapshot();
                    ds = dal.ExecuteDataSet(string.Format(sql, controlno, shiftIN, shiftOUT), CommandType.Text);
                    dal.CommitTransactionSnapshot();
                }
                catch (Exception ex)
                {
                    dal.RollBackTransactionSnapshot();
                    throw new PayrollException("Error upon fetch Job plan record." + "\n" + ex.ToString());
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return ds;
        }
        public DataSet GetOTlist(string EmpId, string OTDate)
        {
            DataSet ds = new DataSet();
            string sql = @"
                            declare @OTTable as table
                            (   
                                OTStartTime char(4),
                                OTEndTime char(4),                                     
                                EmployeeId char(15),
                                OvertimeDate datetime,
                            	overtimetype char(1)	
                            ) 

                            declare @OTTableSub as table
                            (   
                                StartTimeSub char(4),
                                EndTimeSub char(4),                                     
                                EmployeeIdSub char(15),
                                OvertimeDateSub datetime,
                                overtimetype char(1)	
                            )

                            --insert to subquery
                            insert @OTTableSub
                            select min(Eot_StartTime),'0000',Eot_EmployeeId,Eot_OvertimeDate,eot_overtimetype
                                                                from t_employeeovertime
                                                                where eot_employeeid='{0}' and eot_overtimedate='{1}' and eot_status in ('A','9')
                            group by Eot_EmployeeId,Eot_OvertimeDate,eot_overtimetype
                            union
                            select '0000',max(Eot_EndTime) ,Eot_EmployeeId,Eot_OvertimeDate,eot_overtimetype
                                                                from t_employeeovertime
                                                                where eot_employeeid='{0}' and eot_overtimedate='{1}' and eot_status in ('A','9')
                            group by Eot_EmployeeId,Eot_OvertimeDate,eot_overtimetype
                            union
                            select min(Eot_StartTime),'0000',Eot_EmployeeId,Eot_OvertimeDate,eot_overtimetype
                                                                from t_employeeovertimehist
                                                                where eot_employeeid='{0}' and eot_overtimedate='{1}' and eot_status in ('A','9')
                            group by Eot_EmployeeId,Eot_OvertimeDate,eot_overtimetype
                            union
                            select '0000',max(Eot_EndTime) ,Eot_EmployeeId,Eot_OvertimeDate,eot_overtimetype
                                                                from t_employeeovertimehist
                                                                where eot_employeeid='{0}' and eot_overtimedate='{1}' and eot_status in ('A','9')
                            group by Eot_EmployeeId,Eot_OvertimeDate,eot_overtimetype



                            --insert to Main OT
                            insert @OTTable
                            select  replicate('0',4-len(convert(varchar,sum(convert(int,StartTimeSub)))))+convert(varchar,sum(convert(int,StartTimeSub))),
                                    replicate('0',4-len(convert(varchar,sum(convert(int,EndTimeSub)))))+convert(varchar,sum(convert(int,EndTimeSub))),
                                    EmployeeIdSub,OvertimeDateSub,overtimetype 
                                    from @OTTableSub
                            group by EmployeeIdSub,OvertimeDateSub,overtimetype

                            Select EmployeeId,OvertimeDate,OTStartTime,OTEndTime,
	                            (cast(substring(OTStartTime, 1, 2) as smallint) * 60) +
                                (cast(substring(OTStartTime, 3, 2) as smallint)) as ConOTStart,
                                (cast(substring(OTEndTime, 1, 2) as smallint) * 60) +
                                (cast(substring(OTEndTime, 3, 2) as smallint)) as ConOTEnd,overtimetype
                            from @OTTable ";

            using (DALHelper dal = new DALHelper(false))
            {
                dal.OpenDB();
                try
                {
                    dal.BeginTransactionSnapshot();
                    ds = dal.ExecuteDataSet(string.Format(sql, EmpId, OTDate), CommandType.Text);
                    dal.CommitTransactionSnapshot();
                }
                catch (Exception ex)
                {
                    dal.RollBackTransactionSnapshot();
                    throw new PayrollException("Error upon fetch overtime record." + "\n" + ex.ToString());
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return ds;
        }
        public DataSet GetLeavelist(string EmpId, string OTDate)
        {
            DataSet ds = new DataSet();
            string sql = @"Select Elt_EmployeeId,Elt_LeaveDate,Elt_StartTime,Elt_EndTime,Elt_LeaveHour,Elt_DayUnit,elt_leavetype, 
                                (cast(substring(Elt_StartTime, 1, 2) as smallint) * 60) +
                                (cast(substring(Elt_StartTime, 3, 2) as smallint)) as ConLeaveStart,
                                (cast(substring(Elt_EndTime, 1, 2) as smallint) * 60) +
                                (cast(substring(Elt_EndTime, 3, 2) as smallint)) as ConLeaveEnd		
                            from t_employeeleaveavailment where Elt_EmployeeId='{0}' and Elt_LeaveDate='{1}' and elt_status in ('A','9')

                            union

                            Select Elt_EmployeeId,Elt_LeaveDate,Elt_StartTime,Elt_EndTime,Elt_LeaveHour,Elt_DayUnit,elt_leavetype, 
                                (cast(substring(Elt_StartTime, 1, 2) as smallint) * 60) +
                                (cast(substring(Elt_StartTime, 3, 2) as smallint)) as ConLeaveStart,
                                (cast(substring(Elt_EndTime, 1, 2) as smallint) * 60) +
                                (cast(substring(Elt_EndTime, 3, 2) as smallint)) as ConLeaveEnd		
                            from t_employeeleaveavailmentHist where Elt_EmployeeId='{0}' and Elt_LeaveDate='{1}' and elt_status in ('A','9')
                            order by Elt_EmployeeId,Elt_LeaveDate asc";

            using (DALHelper dal = new DALHelper(false))
            {
                dal.OpenDB();
                try
                {
                    dal.BeginTransactionSnapshot();
                    ds = dal.ExecuteDataSet(string.Format(sql, EmpId, OTDate), CommandType.Text);
                    dal.CommitTransactionSnapshot();
                }
                catch (Exception ex)
                {
                    dal.RollBackTransactionSnapshot();
                    throw new PayrollException("Error upon fetch overtime record." + "\n" + ex.ToString());
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return ds;
        }
        public void TransDetailsRec(DALHelper dal)
        {
            #region <SQL>
            string sql = @"DECLARE @NextBillingCycle as Char(6)
                            DECLARE @CurrentBillingCycle as Char(6)
                            DECLARE @CurrentMinDate as Datetime
                            DECLARE @CurrentMaxDate as Datetime                            

                            SET @CurrentBillingCycle = (SELECT DISTINCT bcn_billingyearmonth
                                                        FROM T_BillingConfiguration
                                                        WHERE Bcn_Indicator='C')
                            SET @CurrentMinDate = (SELECT Min(Bcn_StartCycle) as MinDate
                                                  FROM T_BillingConfiguration
                                                  WHERE Bcn_BillingYearMonth = @CurrentBillingCycle )

                            SET @CurrentMaxDate = convert(char(10),getdate(),101);

                            DECLARE @StartDate as datetime
                            DECLARE @EndDate as datetime
                            SET @StartDate	= @CurrentMinDate
                            SET @EndDate	= @CurrentMaxDate

                            --Create Temporary Tables
                            declare @Leave as table
                            ( EmployeeID varchar(15) NOT NULL
                            , LeaveDate  datetime NOT NULL 
                            , LeaveType  Char(2) NOT NULL 
                            , LeaveHour Decimal(5,2) NOT NULL
                            , Elt_Status Char(1) NOT NULL)

                            declare @Overtime as table
                            ( EmployeeID varchar(15) NOT NULL
                            , OvertimeDate  datetime NOT NULL 
                            , OTStartTime Char(4) NOT NULL
                            , OTEndTime Char(4) NOT NULL
                            , OTHours Decimal(9,2))

                            declare @Ledger as table
                            ( 
                              EmployeeID varchar(15) NOT NULL
                            , ProcessDate  datetime NOT NULL 
                            , Shiftcode Varchar(10) NOT NULL
                            , Daycode Varchar(4) NOT NULL
                            , ActualTimeIn_1 Char(4) NOT NULL
                            , ActualTimeOut_1 Char(4) NOT NULL
                            , ActualTimeIn_2 Char(4) NOT NULL
                            , ActualTimeOut_2 Char(4) NOT NULL
                            , Flex Char(1) NOT NULL
                            , TimeMod Char(1) NOT NULL
                            )

                            --------------------------
                            -- Insert Leave Data
                            --------------------------

                            INSERT INTO @Leave
                            SELECT Elt_EmployeeId
	                            , Elt_LeaveDate
	                            , Elt_LeaveType
	                            , Elt_LeaveHour
	                            , Elt_Status
                            FROM T_EmployeeLeaveAvailment
                            WHERE Elt_Status in ('9','0')
	                            and Elt_LeaveDate between @StartDate and @EndDate

                            UNION ALL

                            SELECT Elt_EmployeeId
	                            , Elt_LeaveDate
	                            , Elt_LeaveType
	                            , Elt_LeaveHour
	                            , Elt_Status
                            FROM T_EmployeeLeaveAvailmentHist
                            WHERE Elt_Status in ('9','0')
	                            and Elt_LeaveDate between @StartDate and @EndDate


                            DELETE FROM @Leave
                            FROM @Leave as LRec
                            INNER JOIN (SELECT EmployeeID
				                            , LeaveDate
				                            , LeaveType
			                             FROM @Leave
			                            GROUP BY EmployeeID
				                            , LeaveDate
				                            , LeaveType
			                            Having Count(*) = 2 ) xx on xx.EmployeeID =  LRec.EmployeeID
	                            and  xx.LeaveDate = LRec.LeaveDate
	                            and xx.LeaveType = LRec.LeaveType


                            --------------------------
                            -- Insert Overtime Data
                            --------------------------
                            INSERT INTO @Overtime
                            SELECT Eot_employeeid
	                            , Eot_overtimedate
	                            , Min(Eot_StartTime) 
	                            , Max(Eot_EndTime) 
	                            , Sum(Eot_OvertimeHour)
                            FROM (	SELECT Eot_employeeid
		                            , Eot_overtimedate
		                            , Eot_StartTime
		                            , Eot_EndTime
		                            , Eot_OvertimeHour
		                             FROM t_employeeovertime
		                            WHERE Eot_overtimedate between @StartDate and @EndDate
		                            and Eot_status ='9'
		                            UNION ALL
		                            SELECT Eot_employeeid
		                            , Eot_overtimedate
		                            , Eot_StartTime
		                            , Eot_EndTime
		                            , Eot_OvertimeHour
		                             FROM t_employeeovertimehist
		                            WHERE Eot_overtimedate between @StartDate and @EndDate
		                            and Eot_status ='9') xx
                            GROUP BY  Eot_employeeid
	                            , Eot_overtimedate

                            --------------------------
                            -- Insert Log Data
                            --------------------------
                            INSERT INTO @Ledger
                            SELECT  Ell_EmployeeId
                            , Ell_ProcessDate
                            , Ell_ShiftCode
                            , Ell_DayCode
                            , Ell_ActualTimeIn_1
                            , Ell_ActualTimeOut_1
                            , Ell_ActualTimeIn_2
                            , Ell_ActualTimeOut_2
                            , Ell_TagFlex
                            , Ell_TagTimeMod
                            FROM T_EmployeeLogledger
                            WHERE Ell_Processdate between  @StartDate and @EndDate

                            UNION

                            SELECT  Ell_EmployeeId
                            , Ell_ProcessDate
                            , Ell_ShiftCode
                            , Ell_DayCode
                            , Ell_ActualTimeIn_1
                            , Ell_ActualTimeOut_1
                            , Ell_ActualTimeIn_2
                            , Ell_ActualTimeOut_2
                            , Ell_TagFlex
                            , Ell_TagTimeMod
                            FROM T_EmployeeLogledgerHist
                            WHERE Ell_Processdate between  @StartDate and @EndDate


                            --------------------------
                            -- Insert Manhour Data
                            --------------------------
                            DELETE FROM T_ManHourDetails

                            INSERT INTO T_ManHourDetails
                            SELECT  Jsd_controlno
		                            , Jsd_Seqno
		                            , Shiftcode
		                            , Daycode
		                            , ActualTimeIn_1
		                            , ActualTimeOut_1
		                            , ActualTimeIn_2
		                            , ActualTimeOut_2
		                            , Flex
		                            , TimeMod
		                            , Isnull(OTRec.OTStartTime,'')
		                            , Isnull(OTRec.OTEndTime,'')
		                            , Isnull(OTRec.OTHours,0)
		                            , Isnull(Lrec.LeaveType,'')
		                            , Isnull(Lrec.LeaveHour,0)
	 	                            , Isnull(Jsd_ActHours,0)
		                            , 'sa'
		                            , Getdate()
                            FROM t_jobsplitdetail
                            INNER JOIN t_jobsplitheader on jsh_controlno = jsd_controlno
	                            and jsh_jobsplitdate between @StartDate and @EndDate
                            left join @Ledger  as LedgerRec on LedgerRec.EmployeeID = jsh_employeeid  COLLATE SQL_Latin1_General_CP1_CI_AS
	                            and Processdate = jsh_jobsplitdate
                            left Join @Overtime  as OTRec on OTRec.EmployeeID = jsh_employeeid  COLLATE SQL_Latin1_General_CP1_CI_AS
	                            and overtimedate = jsh_jobsplitdate
                            left Join @Leave as Lrec on Lrec.EmployeeID = jsh_employeeid COLLATE SQL_Latin1_General_CP1_CI_AS
	                            and Lrec.LeaveDate = jsh_jobsplitdate
                            WHERE jsd_status ='9'

                            ";
            #endregion
            try
            {
                dal.ExecuteNonQuery(sql, CommandType.Text);
            }
            catch (Exception ex)
            {
                throw new PayrollException(ex);
            }
        }
        public void DeleteAllLessThanOneLosstime(DALHelper dal)
        {
            #region <SQL>
            string sql = @"delete from T_jobsplitdetaillosstime where jsd_actHours<1";
            #endregion
            try
            {
                dal.ExecuteNonQuery(sql, CommandType.Text);
            }
            catch (Exception ex)
            {
                throw new PayrollException(ex);
            }
        }
    }
}
