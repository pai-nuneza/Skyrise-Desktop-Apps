using System;
using System.Collections.Generic;
using System.Text;

using System.Data;
using CommonLibrary;
using Payroll.DAL;

namespace Payroll.BLogic
{
    public class IndividualLeaveEntryBL: BaseBL
    {
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
    
        //Charlie
        public DataSet GetAllRecords(string EmployeeID)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@EmployeeID", EmployeeID);

            string sqlQuery = @"Select Tlv_LeaveDate
                                                    ,Tlv_LeaveCode
                                                    ,Substring(Tlv_StartTime, 1, 2) + ':' + Substring(Tlv_StartTime, 3, 2) Tlv_StartTime
                                                    ,Substring(Tlv_EndTime, 1, 2) + ':' + Substring(Tlv_EndTime, 3, 2) Tlv_EndTime
                                                    ,Tlv_LeaveHours
                                                    ,case when convert(int,Tlv_LeaveHours)=5 or convert(int,Tlv_LeaveHours)=4 then 'Half Day' else 'Whole Day' end LeaveDay
                                                    ,RTrim(Tlv_ReasonForRequest) Tlv_ReasonForRequest
                                                    ,Tlv_LeaveFlag
                                                    ,Tlv_Authority1
                                                    ,Tlv_Authority1Date
                                                    ,Tlv_Authority3
                                                    ,Tlv_Authority3Date
                                                    ,Tlv_LeaveStatus
                                                    ,Usr_Login
                                                    ,Ludatetime
                                                    ,Elt_LeaveCategory
                                                    ,Tlv_LeaveUnit
                                              From T_EmpLeave
                                              Where Tlv_IDNo = @EmployeeID";
            
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            return ds;
        }

        public DataSet GetLeaveBalances(string EmployeeID)
        {
            DataSet ds = new DataSet();
            DataSet dsflag= new DataSet();
            //get leave balance in days or hrs
            string sql = @"Select Tsc_SetFlag
                                From T_SettingControl 
                                     Where Tsc_SystemCode = 'LEAVE'
                                           and Tsc_SettingCode = 'LVHRENTRY' 
                                           and  Tsc_SetFlag='False'";                      

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@EmployeeID", EmployeeID);

            string sqlQueryHr = @"Select Elm_VLBalance		
                                                    ,Elm_SLBalance
                                                    ,Elm_ELBalance
                                                    ,Elm_PLBalance
                                                    ,Elm_BLBalance
                                                    ,Elm_DLBalance
                                              From T_EmployeeLeaveMaster
                                              Where Tll_IDNo = @EmployeeID";

            string sqlQueryDay = @"Select Elm_VLBalance/8 as  Elm_VLBalance		
                                                    ,Elm_SLBalance/8 as Elm_SLBalance
                                                    ,Elm_ELBalance/8 as Elm_ELBalance
                                                    ,Elm_PLBalance/8 as Elm_PLBalance
                                                    ,Elm_BLBalance/8 as Elm_BLBalance 
                                                    ,Elm_DLBalance/8 as Elm_DLBalance
                                              From T_EmployeeLeaveMaster
                                                Where Tll_IDNo = @EmployeeID";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dsflag = dal.ExecuteDataSet(sql, CommandType.Text);

                if (dsflag.Tables[0].Rows.Count > 0)
                    ds = dal.ExecuteDataSet(sqlQueryDay, CommandType.Text, paramInfo);
                else
                    ds = dal.ExecuteDataSet(sqlQueryHr, CommandType.Text, paramInfo);
                                
                dal.CloseDB();
            }
            return ds;
        }

        public DataSet GetLeaveTypeDesc(string LeaveType)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@LeaveType", LeaveType);

            string sqlQuery = @"Select Mlv_LeaveDescription, Mlv_LeaveCode 
                                              From M_Leave
                                              Where Mlv_LeaveCode = @LeaveType
                                              And Mlv_IsCombineLeave = 0 and Mlv_RecordStatus='A'";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            return ds;
        }

        public bool CheckDuplicateEntry(string EmployeeID
                                          ,string LeaveType
                                          ,string LeaveDate)
        {
            DataSet ds = new DataSet();
            bool isDuplicate;

            ParameterInfo[] paramInfo = new ParameterInfo[3];
            paramInfo[0] = new ParameterInfo("@EmployeeID", EmployeeID);
            paramInfo[1] = new ParameterInfo("@LeaveType", LeaveType);
            paramInfo[2] = new ParameterInfo("@LeaveDate", LeaveDate);

            string sqlQuery = @"Select Tlv_IDNo
                                              From T_EmpLeave
                                              Where Tlv_IDNo = @EmployeeID
                                              And Tlv_LeaveCode = @LeaveType
                                              And Tlv_LeaveDate = @LeaveDate";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);

                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                isDuplicate = true;
            else
                isDuplicate = false;

            return isDuplicate;
        }

        public DataSet CheckLeaveBalances(string EmployeeID
                                         ,string LeaveType)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@EmployeeID", EmployeeID);
            paramInfo[1] = new ParameterInfo("@LeaveType", LeaveType);

            ////string sqlQuery = @"Select dbo.GetLeaveBalance(@LeaveType, @EmployeeID) As 'LeaveBalance'";
            string sql = @"select Tll_BegCredit-(Tll_OutCredit + Tll_PendingCredit) as Bal
                                        from T_EmpLeaveLdg 
                                            where Tll_IDNo=@EmployeeID and Tll_LeaveCode=@LeaveType";


            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sql, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            return ds;
        }

        public DataSet GetPayrollTypeAndBdate(string EmployeeID)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@EmployeeID", EmployeeID);

            string sqlQuery = @"Select Mem_BirthDate
                                                    ,Mem_PayrollType
                                                    ,Convert(char, Mem_BirthDate, 107) As BDate
                                              From M_Employee
                                              Where Mem_IDNo = @EmployeeID";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            return ds;
        }

        public DataSet CheckIfPaidWithCreditCombined(string LeaveType)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@LeaveType", LeaveType);

            string sqlQuery = @"Select Mlv_LeaveCode
                                                    ,Mlv_IsCombineLeave
                                                    ,Mlv_IsPaidLeave
                                                    ,Mlv_WithCredit
                                              From M_Leave
                                              Where Mlv_LeaveCode = @LeaveType";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            return ds;
        }

        public DataSet GetShiftCodeBreakOutInDayCode(string PayPeriod
                                                    ,string EmployeeID
                                                    ,string LeaveDate)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[3];
            paramInfo[0] = new ParameterInfo("@PayPeriod", PayPeriod);
            paramInfo[1] = new ParameterInfo("@EmployeeID", EmployeeID);
            paramInfo[2] = new ParameterInfo("@LeaveDate", LeaveDate);

            string sqlQuery = @"Declare @TableName char(30)
                                              Declare @ColumnName char(30)

                                              If @PayPeriod = 'C'
	                                              Begin
		                                              Set @TableName = 'T_EmpTimeRegister' 
		                                              Set @ColumnName = 'Ttr_ShiftCode'
	                                              End
                                              Else If @PayPeriod = 'P'
	                                              Begin
		                                              Set @TableName = 'T_EmpTimeRegisterHst' 
		                                              Set @ColumnName = 'Ttr_ShiftCode' 
	                                              End

                                              Execute('Select Ttr_DayCode, Ttr_ShiftCode, Msh_ShiftOut1, Msh_ShiftIn2
                                                      From ' + @TableName + ' 
                                                       Inner Join M_Shift On ' + @ColumnName + ' = Msh_ShiftCode 
                                                       Where Ttr_IDNo = ''' + @EmployeeID + '''   
		                                               And Ttr_Date = ''' + @LeaveDate + '''')";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            return ds;
        }

        public DataSet GetShiftCodeBreakOutInFuture(string EmployeeID)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@EmployeeID", EmployeeID);

            string sqlQuery = @"Select Mem_ShiftCode, Msh_ShiftOut1, Msh_ShiftIn2
                                              From M_Employee
                                              Inner Join M_Shift On Mem_ShiftCode = Msh_ShiftCode 
                                              Where Mem_IDNo = @EmployeeID";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            return ds;
        }

        public DataSet CheckIfHoliday(string LeaveDate)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@LeaveDate", LeaveDate);

            string sqlQuery = @"Select Thl_HolidayDate, Thl_LeaveCutOffDate 
                                              From T_Holiday
                                              Where Thl_HolidayDate = @LeaveDate";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);

                dal.CloseDB();
            }

            return ds;
        }

        public DataSet GetMaxHour(string PayPeriod, string EmployeeID, string LeaveDate)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[3];
            paramInfo[0] = new ParameterInfo("@PayPeriod", PayPeriod);
            paramInfo[1] = new ParameterInfo("@EmployeeID", EmployeeID);
            paramInfo[2] = new ParameterInfo("@LeaveDate", LeaveDate);

            string sqlQuery = @"Declare @TableName char(30)
                                              Declare @ColumnName char(30)

                                              If @PayPeriod = 'C'
	                                              Begin
		                                              Set @TableName = 'T_EmpTimeRegister' 
		                                              Set @ColumnName = 'Ttr_ShiftCode'
	                                              End
                                              Else If @PayPeriod = 'P'
	                                              Begin
		                                              Set @TableName = 'T_EmpTimeRegisterHst' 
		                                              Set @ColumnName = 'Ttr_ShiftCode' 
	                                              End
                                              
                                              Execute('Select Msh_ShiftHours MaxHr
                                                       From ' + @TableName + ' 
                                                       Inner Join M_Shift On ' + @ColumnName + ' = Msh_ShiftCode 
                                                       Where Ttr_IDNo = ''' + @EmployeeID + '''   
		                                               And Ttr_Date = ''' + @LeaveDate + '''')";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            return ds;
        }

        public DataSet GetMaxHourFuture(string EmployeeID)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@EmployeeID", EmployeeID);

            string sqlQuery = @"Select Msh_ShiftHours as MaxHr
                                              From M_Shift
                                              Inner Join M_Employee On Mem_ShiftCode = Msh_ShiftCode
                                              And Mem_IDNo = @EmployeeID";
            
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            return ds;
        }

        public bool CheckCut0ff()
        {
            DataSet ds = new DataSet();
            bool isCutOff;

            string sqlQuery = @"Select Tsc_SetFlag
                                              From T_SettingControl 
                                              Where Tsc_SystemCode = 'LEAVE'
                                              And Tsc_SettingCode = 'CUT-OFF'";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text);

                dal.CloseDB();
            }

            if (ds.Tables[0].Rows[0][0].ToString().ToUpper().Equals("TRUE"))
                isCutOff = true;
            else
                isCutOff = false;
               
            return isCutOff;
        }

        public DataSet GetMinLvHrAndLvFraction(string ParamID)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@ParamID", ParamID);

            string sqlQuery = @"Select * From NumericValue(@ParamID)";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            return ds;
        }

        public int InsertLeaveEntry(System.Data.DataRow row,decimal ReqVal,bool isUsed,bool withCredit, DALHelper dal)
        {
            int retVal = 0;
            string sql = string.Empty;

            ParameterInfo[] paramInfo = new ParameterInfo[19];
            paramInfo[0] = new ParameterInfo("@Tlv_PayCycle", row["Tlv_PayCycle"]);
            paramInfo[1] = new ParameterInfo("@Tlv_IDNo", row["Tlv_IDNo"]);
            paramInfo[2] = new ParameterInfo("@Tlv_LeaveDate", row["Tlv_LeaveDate"]);
            paramInfo[3] = new ParameterInfo("@Tlv_LeaveCode", row["Tlv_LeaveCode"]);
            paramInfo[4] = new ParameterInfo("@Tlv_StartTime", row["Tlv_StartTime"]);
            paramInfo[5] = new ParameterInfo("@Tlv_EndTime", row["Tlv_EndTime"]);
            paramInfo[6] = new ParameterInfo("@Tlv_LeaveHours", row["Tlv_LeaveHours"]);
            paramInfo[7] = new ParameterInfo("@Tlv_ReasonForRequest", row["Tlv_ReasonForRequest"]);
            paramInfo[8] = new ParameterInfo("@Tlv_LeaveFlag", row["Tlv_LeaveFlag"]);
            paramInfo[9] = new ParameterInfo("@Elt_InformDate", row["Elt_InformDate"]);
            paramInfo[10] = new ParameterInfo("@Tlv_Authority1", row["Tlv_Authority1"]);
            paramInfo[11] = new ParameterInfo("@Tlv_Authority1Date", row["Tlv_Authority1Date"]);
            paramInfo[12] = new ParameterInfo("@Tlv_Authority3", row["Tlv_Authority3"]);
            paramInfo[13] = new ParameterInfo("@Tlv_Authority3Date", row["Tlv_Authority3Date"]);
            paramInfo[14] = new ParameterInfo("@Tlv_LeaveStatus", row["Tlv_LeaveStatus"]);
            paramInfo[15] = new ParameterInfo("@Elt_LeaveCategory", row["Elt_LeaveCategory"]);
            paramInfo[16] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);
            paramInfo[17] = new ParameterInfo("@Tlv_LeaveUnit", row["Tlv_LeaveUnit"]);
            paramInfo[18] = new ParameterInfo("@Tlv_DocumentNo", row["Tlv_DocumentNo"]);

            string sqlQuery = @"
                                DECLARE @Costcenter as varchar(10)
                                set @Costcenter = (SELECT Mem_CostcenterCode FROM M_Employee
				                                   WHERE Mem_IDNo = @Tlv_IDNo)

                                Insert Into T_EmpLeave 
			                                                (Tlv_PayCycle
                                                            ,Tlv_IDNo
                                                            ,Tlv_LeaveDate
			                                                ,Tlv_LeaveCode
			                                                ,Tlv_StartTime
			                                                ,Tlv_EndTime
			                                                ,Tlv_LeaveHours
			                                                ,Tlv_ReasonForRequest
			                                                ,Tlv_LeaveFlag
			                                                ,Elt_InformDate
			                                                ,Tlv_RequestDate
			                                                ,Tlv_Authority1
			                                                ,Tlv_Authority1Date
			                                                ,Tlv_Authority3
			                                                ,Tlv_Authority3Date
			                                                ,Tlv_DocumentNo
			                                                ,Tlv_LeaveStatus
                                                            ,Elt_LeaveCategory
                                                            ,Tlv_LeaveUnit
			                                                ,Usr_Login
			                                                ,Ludatetime
			                                                ,Tlv_CostcenterCode)
	                                                  Values(@Tlv_PayCycle
                                                            ,@Tlv_IDNo
                                                            ,@Tlv_LeaveDate
			                                                ,@Tlv_LeaveCode
			                                                ,@Tlv_StartTime
			                                                ,@Tlv_EndTime
			                                                ,@Tlv_LeaveHours
			                                                ,@Tlv_ReasonForRequest
			                                                ,@Tlv_LeaveFlag
			                                                ,@Elt_InformDate
			                                                ,getdate()
			                                                ,@Tlv_Authority1
			                                                ,@Tlv_Authority1Date
			                                                ,@Tlv_Authority3
			                                                ,@Tlv_Authority3Date
			                                                ,@Tlv_DocumentNo
			                                                ,@Tlv_LeaveStatus
                                                            ,@Elt_LeaveCategory
                                                            ,@Tlv_LeaveUnit
			                                                ,@Usr_Login
			                                                ,GetDate()
			                                                ,@Costcenter)

                                ";
            if (isUsed)
            {
                sql = @"update T_EmpLeaveLdg set Tll_OutCredit = Tll_OutCredit + {2},Usr_Login='{3}' 
                                       where Tll_IDNo='{0}' and Tll_LeaveCode='{1}'
                        --to update the part of leave 
                        if(select Mlv_IsPartOfLeave from M_Leave where Mlv_LeaveCode='{1}' and Mlv_RecordStatus='A') !=''
                             begin 
                              update T_EmpLeaveLdg set Tll_OutCredit = Tll_OutCredit + {2},Usr_Login='{3}' 
                                                                   where Tll_IDNo='{0}' 
											                             and Tll_LeaveCode=(select Mlv_IsPartOfLeave from M_Leave where Mlv_LeaveCode='{1}' and Mlv_RecordStatus='A')
                             end    
                        ";
            }
            else
            {
                sql = @"update T_EmpLeaveLdg set Tll_PendingCredit = Tll_PendingCredit + {2},Usr_Login='{3}' 
                                       where Tll_IDNo='{0}' and Tll_LeaveCode='{1}'
                        --to update the part of leave
                        if(select Mlv_IsPartOfLeave from M_Leave where Mlv_LeaveCode='{1}' and Mlv_RecordStatus='A') !=''
                             begin 
                              update T_EmpLeaveLdg set Tll_PendingCredit = Tll_PendingCredit + {2},Usr_Login='{3}' 
                                                                   where Tll_IDNo='{0}' 
											                             and Tll_LeaveCode=(select Mlv_IsPartOfLeave from M_Leave where Mlv_LeaveCode='{1}' and Mlv_RecordStatus='A')
                             end 
                       ";
            }
             
                try
                {                    
                    retVal = dal.ExecuteNonQuery(sqlQuery, CommandType.Text, paramInfo);
                    if (withCredit)
                       dal.ExecuteNonQuery(string.Format(sql, row["Tlv_IDNo"].ToString().Trim(), row["Tlv_LeaveCode"].ToString().Trim(), ReqVal, row["Tlv_LeaveStatus"].ToString().Trim()), CommandType.Text);
                }
                catch (Exception e)
                {                    
                    throw new PayrollException(e);
                }
        

            return retVal;
        }

        public int InsertLeaveEntryHist(System.Data.DataRow row)
        {
            int retVal = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[15];
            paramInfo[0] = new ParameterInfo("@Tlv_IDNo", row["Tlv_IDNo"]);
            paramInfo[1] = new ParameterInfo("@Tlv_LeaveDate", row["Tlv_LeaveDate"]);
            paramInfo[2] = new ParameterInfo("@Tlv_LeaveCode", row["Tlv_LeaveCode"]);
            paramInfo[3] = new ParameterInfo("@Tlv_StartTime", row["Tlv_StartTime"]);
            paramInfo[4] = new ParameterInfo("@Tlv_EndTime", row["Tlv_EndTime"]);
            paramInfo[5] = new ParameterInfo("@Tlv_LeaveHours", row["Tlv_LeaveHours"]);
            paramInfo[6] = new ParameterInfo("@Tlv_ReasonForRequest", row["Tlv_ReasonForRequest"]);
            paramInfo[7] = new ParameterInfo("@Tlv_LeaveFlag", row["Tlv_LeaveFlag"]);
            paramInfo[8] = new ParameterInfo("@Tlv_Authority1", row["Tlv_Authority1"]);
            paramInfo[9] = new ParameterInfo("@Tlv_Authority1Date", row["Tlv_Authority1Date"]);
            paramInfo[10] = new ParameterInfo("@Tlv_Authority3", row["Tlv_Authority3"]);
            paramInfo[11] = new ParameterInfo("@Tlv_Authority3Date", row["Tlv_Authority3Date"]);
            paramInfo[12] = new ParameterInfo("@Tlv_LeaveStatus", row["Tlv_LeaveStatus"]);
            paramInfo[13] = new ParameterInfo("@Elt_LeaveCategory", row["Elt_LeaveCategory"]);
            paramInfo[14] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);

            string sqlQuery = @"Insert Into T_EmpLeaveHst 
			                                                (Tlv_IDNo
                                                            ,Tlv_LeaveDate
			                                                ,Tlv_LeaveCode
			                                                ,Tlv_StartTime
			                                                ,Tlv_EndTime
			                                                ,Tlv_LeaveHours
			                                                ,Tlv_ReasonForRequest
			                                                ,Tlv_LeaveFlag
			                                                ,Tlv_Authority1
			                                                ,Tlv_Authority1Date
			                                                ,Tlv_Authority3
			                                                ,Tlv_Authority3Date
			                                                ,Tlv_LeaveStatus
                                                            ,Elt_LeaveCategory
			                                                ,Usr_Login
			                                                ,Ludatetime)
	                                                  Values(@Tlv_IDNo
                                                            ,@Tlv_LeaveDate
			                                                ,@Tlv_LeaveCode
			                                                ,@Tlv_StartTime
			                                                ,@Tlv_EndTime
			                                                ,@Tlv_LeaveHours
			                                                ,@Tlv_ReasonForRequest
			                                                ,@Tlv_LeaveFlag
			                                                ,@Tlv_Authority1
			                                                ,@Tlv_Authority1Date
			                                                ,@Tlv_Authority3
			                                                ,@Tlv_Authority3Date
			                                                ,@Tlv_LeaveStatus
                                                            ,@Elt_LeaveCategory
			                                                ,@Usr_Login
			                                                ,GetDate())";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    retVal = dal.ExecuteNonQuery(sqlQuery, CommandType.Text, paramInfo);
                    dal.CommitTransactionSnapshot();
                }
                catch (Exception e)
                {
                    dal.RollBackTransactionSnapshot();
                    throw new PayrollException(e);
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return retVal;
        }

        public int UpdateLeaveEntry(System.Data.DataRow row, decimal ReqVal,bool isUsed,bool withCredit, DALHelper dal)
        {
            int retVal = 0;
            //int paramIndex = 0;
            string sql = string.Empty;
            ParameterInfo[] paramInfo = new ParameterInfo[9];
            paramInfo[0] = new ParameterInfo("@Tlv_IDNo", row["Tlv_IDNo"]);
            paramInfo[1] = new ParameterInfo("@Tlv_LeaveDate", row["Tlv_LeaveDate"]);
            paramInfo[2] = new ParameterInfo("@Tlv_LeaveCode", row["Tlv_LeaveCode"]);
            paramInfo[3] = new ParameterInfo("@Tlv_StartTime", row["Tlv_StartTime"]);
            paramInfo[4] = new ParameterInfo("@Tlv_EndTime", row["Tlv_EndTime"]);
            paramInfo[5] = new ParameterInfo("@Tlv_LeaveHours", row["Tlv_LeaveHours"]);
            paramInfo[6] = new ParameterInfo("@Tlv_ReasonForRequest", row["Tlv_ReasonForRequest"]);
            paramInfo[7] = new ParameterInfo("@Elt_LeaveCategory", row["Elt_LeaveCategory"]);
            paramInfo[8] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);

            string sqlQuery = @"Update T_EmpLeave
                                              Set Tlv_StartTime = @Tlv_StartTime
                                                 ,Tlv_EndTime = @Tlv_EndTime
                                                 ,Tlv_LeaveHours = @Tlv_LeaveHours
                                                 ,Tlv_ReasonForRequest = @Tlv_ReasonForRequest
                                                 ,Elt_LeaveCategory = @Elt_LeaveCategory
                                                 ,Usr_Login = @Usr_Login
			                                     ,Ludatetime = GetDate()
                                              Where Tlv_IDNo = @Tlv_IDNo
                                              And Tlv_LeaveDate = @Tlv_LeaveDate
                                              And Tlv_LeaveCode = @Tlv_LeaveCode";
            if (isUsed)
            {
                sql = @"update T_EmpLeaveLdg set Tll_OutCredit = Tll_OutCredit + {2},Usr_Login='{3}' 
                                       where Tll_IDNo='{0}' and Tll_LeaveCode='{1}'
                        --update the part of leave 
                        if(select Mlv_IsPartOfLeave from M_Leave where Mlv_LeaveCode='{1}' and Mlv_RecordStatus='A') !=''
                             begin 
                              update T_EmpLeaveLdg set Tll_OutCredit = Tll_OutCredit + {2},Usr_Login='{3}' 
                                                                   where Tll_IDNo='{0}' 
											                             and Tll_LeaveCode=(select Mlv_IsPartOfLeave from M_Leave where Mlv_LeaveCode='{1}' and Mlv_RecordStatus='A')
                             end    
                        ";
            }
            else
            {
                sql = @"update T_EmpLeaveLdg set Tll_PendingCredit = Tll_PendingCredit + {2},Usr_Login='{3}' 
                                       where Tll_IDNo='{0}' and Tll_LeaveCode='{1}'
                        --update the part of leave if they have
                        if(select Mlv_IsPartOfLeave from M_Leave where Mlv_LeaveCode='{1}' and Mlv_RecordStatus='A') !=''
                             begin 
                              update T_EmpLeaveLdg set Tll_PendingCredit = Tll_PendingCredit + {2},Usr_Login='{3}' 
                                                                   where Tll_IDNo='{0}' 
											                             and Tll_LeaveCode=(select Mlv_IsPartOfLeave from M_Leave where Mlv_LeaveCode='{1}' and Mlv_RecordStatus='A')
                             end 
                       ";
            }
            
            try
            {
                retVal = dal.ExecuteNonQuery(sqlQuery, CommandType.Text, paramInfo);
                if (withCredit)
                    dal.ExecuteNonQuery(string.Format(sql, row["Tlv_IDNo"].ToString().Trim(), row["Tlv_LeaveCode"].ToString().Trim(), ReqVal, row["Tlv_LeaveStatus"].ToString().Trim()), CommandType.Text);                
            }
            catch (Exception e)
            {               
                throw new PayrollException(e);
            }              
            return retVal;
        }

        public int DeleteLeaveEntry(string Tlv_IDNo,
                                    string Tlv_LeaveDate,
                                    string Tlv_LeaveCode,string Status,DALHelper dal)
        {
            int retVal = 0;
            int paramIndex = 0;
            string leave = string.Empty;
            ParameterInfo[] paramInfo = new ParameterInfo[4];

            paramInfo[paramIndex++] = new ParameterInfo("@Tlv_IDNo", Tlv_IDNo);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlv_LeaveDate", Tlv_LeaveDate);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlv_LeaveCode", Tlv_LeaveCode);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlv_LeaveStatus", Status);

            string sqlQuery = @"    declare @Value decimal(7,4)
                                    set @Value = (
                                    select Tlv_LeaveHours From T_EmpLeave
                                           Where Tlv_IDNo = @Tlv_IDNo
                                              And Tlv_LeaveDate = @Tlv_LeaveDate
                                              And Tlv_LeaveCode = @Tlv_LeaveCode
                                    )
                                    
                                    if(@Tlv_LeaveStatus)='A'
                                        begin
                                            update T_EmpLeaveLdg set Tll_OutCredit = Tll_OutCredit-@Value
                                                    where Tll_IDNo=@Tlv_IDNo and Tll_LeaveCode=@Tlv_LeaveCode
                                        end
                                    else
                                        begin
                                            update T_EmpLeaveLdg set Tll_PendingCredit = Tll_PendingCredit - @Value
                                                    where Tll_IDNo=@Tlv_IDNo and Tll_LeaveCode=@Tlv_LeaveCode
                                        end
                                            
                                    --Delete From T_EmpLeave
                                    --          Where Tlv_IDNo = @Tlv_IDNo
                                    --          And Tlv_LeaveDate = @Tlv_LeaveDate
                                    --          And Tlv_LeaveCode = @Tlv_LeaveCode

                                    Update T_EmpLeave
                                    Set Tlv_LeaveStatus = '2'
                                              Where Tlv_IDNo = @Tlv_IDNo
                                              And Tlv_LeaveDate = @Tlv_LeaveDate
                                              And Tlv_LeaveCode = @Tlv_LeaveCode";

            

                try
                {
                    retVal = dal.ExecuteNonQuery(string.Format(sqlQuery,leave), CommandType.Text, paramInfo);
                }
                catch (Exception e)
                {                    
                    throw new PayrollException(e);
                }                          

            return retVal;
        }
        public void UpdateEmployeeLeaveLedger(string EmployeeID, string LeaveType, decimal Value, string UserLogin)
        {
            string sqlQuery = @"update T_EmpLeaveLdg set Tll_OutCredit = Tll_OutCredit + {2},Usr_Login='{3}' 
                                       where Tll_IDNo='{0}' and Tll_LeaveCode='{1}'";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    dal.ExecuteNonQuery(string.Format(sqlQuery, EmployeeID.Trim(), LeaveType.Trim(), Value, UserLogin.Trim()), CommandType.Text);
                    dal.CommitTransactionSnapshot();
                }
                catch (Exception e)
                {
                    dal.RollBackTransactionSnapshot();
                    throw new PayrollException(e);
                }
                finally
                {
                    dal.CloseDB();
                }
            }
        }
        public int UpdateLeaveBalances(string LeaveType,
                                       string Value,
                                       string UserLogin,
                                       string EmployeeID)
        {
            int retVal = 0;
            int paramIndex = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[4];

            paramInfo[paramIndex++] = new ParameterInfo("@LeaveType", LeaveType);
            paramInfo[paramIndex++] = new ParameterInfo("@Value", Value);
            paramInfo[paramIndex++] = new ParameterInfo("@UserLogin", UserLogin);
            paramInfo[paramIndex++] = new ParameterInfo("@EmployeeID", EmployeeID);

            string sqlQuery = @"Declare @ColumnName char(30)

                                                If @LeaveType = 'VL'
                                                 Begin
                                                  Set @ColumnName = 'Elm_VLBalance'
                                                 End
                                                Else If @LeaveType = 'SL'
                                                 Begin
                                                  Set @ColumnName = 'Elm_SLBalance' 
                                                 End
                                                Else If @LeaveType = 'EL'
                                                 Begin
                                                  Set @ColumnName = 'Elm_ELBalance' 
                                                 End
                                                Else If @LeaveType = 'DL'
                                                 Begin
                                                  Set @ColumnName = 'Elm_DLBalance' 
                                                 End
                                                Else If @LeaveType = 'BL'
                                                 Begin
                                                  Set @ColumnName = 'Elm_BLBalance' 
                                                 End
                                                Else If @LeaveType = 'PL'
                                                 Begin
                                                  Set @ColumnName = 'Elm_PLBalance' 
                                                 End

                                                Execute('Update T_EmployeeLeaveMaster
                                                       Set ' + @ColumnName + ' = ''' + @Value + ''' 
                                                           ,Usr_Login = ''' + @UserLogin + '''
                                                           ,Ludatetime = Getdate()
                                                       Where Tll_IDNo = ''' + @EmployeeID + '''')";

        
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    retVal = dal.ExecuteNonQuery(sqlQuery, CommandType.Text, paramInfo);
                    dal.CommitTransactionSnapshot();
                }
                catch (Exception e)
                {
                    dal.RollBackTransactionSnapshot();
                    throw new PayrollException(e);
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return retVal;
        }

        public DataSet GetCurrentPayPeriodCycle()
        {
            DataSet ds = new DataSet();

            string sqlQuery = @"Select Tps_StartCycle
                                                    ,Tps_EndCycle
                                              From T_PaySchedule
                                              Where Tps_CycleIndicator = 'C'";
            
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text);

                dal.CloseDB();
            }

            return ds;
        }

        public int PostToLogLedger(string PayPeriod,
                                   string LeaveType,
                                   string LeaveHr,
                                   string UserLogin,
                                   string EmployeeID,
                                   string LeaveDate,
                                   DALHelper dal)
        {
            int retVal = 0;
            int paramIndex = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[6];

            paramInfo[paramIndex++] = new ParameterInfo("@PayPeriod", PayPeriod);
            paramInfo[paramIndex++] = new ParameterInfo("@LeaveType", LeaveType);
            paramInfo[paramIndex++] = new ParameterInfo("@LeaveHr", LeaveHr);
            paramInfo[paramIndex++] = new ParameterInfo("@UserLogin", UserLogin);
            paramInfo[paramIndex++] = new ParameterInfo("@EmployeeID", EmployeeID);
            paramInfo[paramIndex++] = new ParameterInfo("@LeaveDate", LeaveDate);

            string sqlQuery = @"Declare @TableName char(30)

                                                If @PayPeriod = 'C'
	                                                Begin
		                                                Set @TableName = 'T_EmpTimeRegister' 
	                                                End
                                                Else If @PayPeriod = 'P'
	                                                Begin
		                                                Set @TableName = 'T_EmpTimeRegisterHst' 
	                                                End

                                                Execute('Update '+ @TableName +'
                                                      Set Ttr_WFPayLVCode = '''+ @LeaveType +''' 
                                                         ,Ttr_WFPayLVHr = '''+ @LeaveHr +'''
                                                         ,Usr_Login = '''+ @UserLogin +'''
                                                         ,Ludatetime = GetDate()
	                                                  Where Ttr_IDNo = '''+ @EmployeeID +''' 
	                                                  And Ttr_Date = '''+ @LeaveDate +'''' )";
                   
                try
                {
                    retVal = dal.ExecuteNonQuery(sqlQuery, CommandType.Text, paramInfo);                   
                }
                catch (Exception e)
                {                    
                    throw new PayrollException(e);
                }
                           

            return retVal;
        }

        public int PostToLogLedgerWithNoPay(string PayPeriod,
                                            string LeaveType,
                                            string LeaveHr,
                                            string UserLogin,
                                            string EmployeeID,
                                            string LeaveDate,
                                            DALHelper dal)
        {
            int retVal = 0;
            int paramIndex = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[6];

            paramInfo[paramIndex++] = new ParameterInfo("@PayPeriod", PayPeriod);
            paramInfo[paramIndex++] = new ParameterInfo("@LeaveType", LeaveType);
            paramInfo[paramIndex++] = new ParameterInfo("@LeaveHr", LeaveHr);
            paramInfo[paramIndex++] = new ParameterInfo("@UserLogin", UserLogin);
            paramInfo[paramIndex++] = new ParameterInfo("@EmployeeID", EmployeeID);
            paramInfo[paramIndex++] = new ParameterInfo("@LeaveDate", LeaveDate);

            string sqlQuery = @"Declare @TableName char(30)

                                                If @PayPeriod = 'C'
	                                                Begin
		                                                Set @TableName = 'T_EmpTimeRegister' 
	                                                End
                                                Else If @PayPeriod = 'P'
	                                                Begin
		                                                Set @TableName = 'T_EmpTimeRegisterHst' 
	                                                End

                                                Execute('Update '+ @TableName +'
                                                      Set Ttr_WFNoPayLVCode = '''+ @LeaveType +''' 
                                                         ,Ttr_WFNoPayLVHr = '''+ @LeaveHr +'''
                                                         ,Usr_Login = '''+ @UserLogin +'''
                                                         ,Ludatetime = GetDate()
	                                                  Where Ttr_IDNo = '''+ @EmployeeID +''' 
	                                                  And Ttr_Date = '''+ @LeaveDate +'''' )";
           

                try
                {
                    retVal = dal.ExecuteNonQuery(sqlQuery, CommandType.Text, paramInfo);                    
                }
                catch (Exception e)
                {                    
                    throw new PayrollException(e);
                }
               
            return retVal;
        }

        public DataSet CheckIfLogLedgerHasEntryOnFiledDate(string EmployeeID, string LeaveDate)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@EmployeeID", EmployeeID);
            paramInfo[1] = new ParameterInfo("@LeaveDate", LeaveDate);

            #region query

            string query = @"Select Ttr_WFPayLVCode, Ttr_WFPayLVHr
                             From T_EmpTimeRegister
                             Where Ttr_IDNo = @EmployeeID
	                         And Ttr_Date = @LeaveDate";

            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(query, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            return ds;
        }

        public DataSet CheckIfLogLedgerHistHasEntryOnFiledDate(string EmployeeID, string LeaveDate)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@EmployeeID", EmployeeID);
            paramInfo[1] = new ParameterInfo("@LeaveDate", LeaveDate);

            #region query

            string query = @"Select Ttr_WFPayLVCode, Ttr_WFPayLVHr
                             From T_EmpTimeRegisterHst
                             Where Ttr_IDNo = @EmployeeID
	                         And Ttr_Date = @LeaveDate";

            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(query, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            return ds;
        }

        public DataSet CheckIfLogLedgerHasEntryOnFiledDateNoPay(string EmployeeID, string LeaveDate)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@EmployeeID", EmployeeID);
            paramInfo[1] = new ParameterInfo("@LeaveDate", LeaveDate);

            #region query

            string query = @"Select Ttr_WFNoPayLVCode, Ttr_WFNoPayLVHr
                             From T_EmpTimeRegister
                             Where Ttr_IDNo = @EmployeeID
	                         And Ttr_Date = @LeaveDate";

            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(query, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            return ds;
        }

        public DataSet CheckIfLogLedgerHistHasEntryOnFiledDateNoPay(string EmployeeID, string LeaveDate)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@EmployeeID", EmployeeID);
            paramInfo[1] = new ParameterInfo("@LeaveDate", LeaveDate);

            #region query

            string query = @"Select Ttr_WFNoPayLVCode, Ttr_WFNoPayLVHr
                             From T_EmpTimeRegisterHst
                             Where Ttr_IDNo = @EmployeeID
	                         And Ttr_Date = @LeaveDate";

            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(query, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            return ds;
        }

        public DataSet GetLeaveCombination(string firstLveType, 
                                           string secondLveType)
        {
            DataSet ds = new DataSet();

            #region query

            string sqlstr = @"SELECT Mlv_LeaveCode
                                    ,Substring(Mlv_LeaveDescription,1,2) as FirstCombination
                                    ,Substring(Mlv_LeaveDescription,6,7) as SecondCombination
                              FROM M_Leave
                              WHERE Mlv_LeaveDescription Like '%+%'
                              AND 
                                  (
                                     (Substring(Mlv_LeaveDescription,1,2) = '{0}'
                                      And Substring(Mlv_LeaveDescription,6,7) = '{1}')
                              OR
                                      (Substring(Mlv_LeaveDescription,1,2) = '{1}'
                                       And Substring(Mlv_LeaveDescription,6,7) = '{0}')
                                   )";

            #endregion


            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(string.Format(sqlstr, firstLveType, secondLveType), CommandType.Text);

                dal.CloseDB();
            }
            return ds;

        }

        public int UpdateVLBalance(string Value,
                                   string EmployeeID)
        {
            int retVal = 0;
            int paramIndex = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[2];

            paramInfo[paramIndex++] = new ParameterInfo("@Value", Value);
            paramInfo[paramIndex++] = new ParameterInfo("@EmployeeID", EmployeeID);

            #region query

            string statement = @"Update T_EmployeeLeaveMaster
                                 Set Elm_VLBalance = @Value
                                 Where Tll_IDNo = @EmployeeID";

            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    retVal = dal.ExecuteNonQuery(statement, CommandType.Text, paramInfo);
                    dal.CommitTransactionSnapshot();
                }
                catch (Exception e)
                {
                    dal.RollBackTransactionSnapshot();
                    throw new PayrollException(e);
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return retVal;
        }

        public bool CheckIfCombinedLeave(string LeaveType)
        {
            DataSet ds = new DataSet();
            int paramIndex = 0;
            bool isCombined;

            ParameterInfo[] paramInfo = new ParameterInfo[1];

            paramInfo[paramIndex++] = new ParameterInfo("@LeaveType", LeaveType);

            #region query

            string statement = @"Select Mlv_IsCombineLeave
                                 From M_Leave
                                 Where Mlv_LeaveCode = @LeaveType";

            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(statement, CommandType.Text, paramInfo);

                dal.CloseDB();
            }

            if (ds.Tables[0].Rows[0][0].ToString().Equals("True"))
                isCombined = true;
            else
                isCombined = false;

            return isCombined;
        }

        public DataSet CheckTotalHoursIfLeaveDateHasAnotherEntry(string EmployeeID,
                                                                 string LeaveDate,
                                                                 string LeaveType,
                                                                 string PaidLeave)
        {
            DataSet ds = new DataSet();
            int paramIndex = 0;
           
            ParameterInfo[] paramInfo = new ParameterInfo[4];

            paramInfo[paramIndex++] = new ParameterInfo("@EmployeeID", EmployeeID);
            paramInfo[paramIndex++] = new ParameterInfo("@LeaveDate", LeaveDate);
            paramInfo[paramIndex++] = new ParameterInfo("@LeaveType", LeaveType);
            paramInfo[paramIndex++] = new ParameterInfo("@PaidLeave", PaidLeave);

            #region query

            string statement = @"Select Tlv_LeaveCode, Tlv_LeaveHours
                                 From T_EmpLeave
                                 Inner Join M_Leave On Mlv_LeaveCode = Tlv_LeaveCode
                                 Where Tlv_IDNo = @EmployeeID
                                 And Tlv_LeaveDate = @LeaveDate
                                 And Tlv_LeaveCode <> @LeaveType
                                 And Mlv_IsPaidLeave = @PaidLeave";

            #endregion


            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(statement, CommandType.Text, paramInfo);

                dal.CloseDB();
            }

            return ds;

        }

        public DataSet CheckTotalHoursIfLeaveDateHasAnotherEntryAll(string EmployeeID,
                                                                 string LeaveDate,
                                                                 string LeaveType)
        {
            DataSet ds = new DataSet();
            int paramIndex = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[3];

            paramInfo[paramIndex++] = new ParameterInfo("@EmployeeID", EmployeeID);
            paramInfo[paramIndex++] = new ParameterInfo("@LeaveDate", LeaveDate);
            paramInfo[paramIndex++] = new ParameterInfo("@LeaveType", LeaveType);

            #region query

            string statement = @"Select Sum(Tlv_LeaveHours) As TotalHrs
                                 From T_EmpLeave
                                 Inner Join M_Leave On Mlv_LeaveCode = Tlv_LeaveCode
                                 Where Tlv_IDNo = @EmployeeID
                                 And Tlv_LeaveDate = @LeaveDate
                                 And Tlv_LeaveCode <> @LeaveType";

            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(statement, CommandType.Text, paramInfo);

                dal.CloseDB();
            }

            return ds;

        }

        public DataSet CheckIfTwoEntry(string EmployeeID,
                                       string LeaveDate,
                                       string PaidLeave)
        {
            DataSet ds = new DataSet();
            int paramIndex = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[3];

            paramInfo[paramIndex++] = new ParameterInfo("@EmployeeID", EmployeeID);
            paramInfo[paramIndex++] = new ParameterInfo("@LeaveDate", LeaveDate);
            paramInfo[paramIndex++] = new ParameterInfo("@PaidLeave", PaidLeave);

            #region query

            string statement = @"Select Tlv_LeaveCode, Tlv_LeaveHours
                                 From T_EmpLeave
                                 Inner Join M_Leave On Mlv_LeaveCode = Tlv_LeaveCode
                                 Where Tlv_IDNo = @EmployeeID
                                 And Tlv_LeaveDate = @LeaveDate
                                 And Mlv_IsPaidLeave = @PaidLeave";

            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(statement, CommandType.Text, paramInfo);

                dal.CloseDB();
            }

            return ds;
        }

        public DataSet FetchEmployeeShiftBreakChargeData(string idnumber, string date)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"Select 
	                                Msh_ShiftOut1,
	                                Msh_ShiftIn2,
	                                Msh_PaidBreak,
                                    ((cast(substring(Msh_ShiftIn2, 1, 2) as smallint) * 60) +
                                                 (cast(substring(Msh_ShiftIn2, 3, 2) as smallint))) - 
	                                ((cast(substring(Msh_ShiftOut1, 1, 2) as smallint) * 60) +
                                                 (cast(substring(Msh_ShiftOut1, 3, 2) as smallint)))
	                                as TotBreakInMin
                                From M_Shift
                                Where Msh_ShiftCode = 
	                                (Select Ttr_ShiftCode
		                                From T_EmpTimeRegister
		                                Where Ttr_IDNo = @Ttr_IDNo
			                                And Ttr_Date = @Ttr_Date)";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Ttr_IDNo", idnumber);
            paramInfo[1] = new ParameterInfo("@Ttr_Date", date);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            return ds;
        }

        public DataSet FetchEmployeeShiftBreakChargeDataHist(string idnumber, string date)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"Select 
	                                Msh_ShiftOut1,
	                                Msh_ShiftIn2,
	                                Msh_PaidBreak,
                                    ((cast(substring(Msh_ShiftIn2, 1, 2) as smallint) * 60) +
                                                 (cast(substring(Msh_ShiftIn2, 3, 2) as smallint))) - 
	                                ((cast(substring(Msh_ShiftOut1, 1, 2) as smallint) * 60) +
                                                 (cast(substring(Msh_ShiftOut1, 3, 2) as smallint)))
	                                as TotBreakInMin
                                From M_Shift
                                Where Msh_ShiftCode = 
	                                (Select Ttr_ShiftCode
		                                From T_EmpTimeRegisterHst
		                                Where Ttr_IDNo = @Ttr_IDNo
			                                And Ttr_Date = @Ttr_Date)";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Ttr_IDNo", idnumber);
            paramInfo[1] = new ParameterInfo("@Ttr_Date", date);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            return ds;
        }

        public DataSet FetchEmployeeShiftBreakChargeDataFuture(string idnumber)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"Select 
	                                Msh_ShiftOut1,
	                                Msh_ShiftIn2,
	                                Msh_PaidBreak,
                                    ((cast(substring(Msh_ShiftIn2, 1, 2) as smallint) * 60) +
                                                 (cast(substring(Msh_ShiftIn2, 3, 2) as smallint))) - 
	                                ((cast(substring(Msh_ShiftOut1, 1, 2) as smallint) * 60) +
                                                 (cast(substring(Msh_ShiftOut1, 3, 2) as smallint)))
	                                as TotBreakInMin
                                From M_Shift
                                Where Msh_ShiftCode = 
	                                (Select Mem_ShiftCode
                                     From M_Employee
                                     Where Mem_IDNo = @EmployeeID)";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@EmployeeID", idnumber);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            return ds;
        }

        public bool GetApproveUponEntry()
        {
            DataSet ds = new DataSet();
            bool isApprove;

            #region query

            string statement = @"Select Tsc_SetFlag
                                 From T_SettingControl
                                 Where Tsc_SystemCode = 'LEAVE'
                                 And Tsc_SettingCode = 'APRVENTRY'";

            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(statement, CommandType.Text);

                dal.CloseDB();
            }

            if (ds.Tables[0].Rows[0][0].ToString().Equals("True"))
                isApprove = true;
            else
                isApprove = false;

            return isApprove;
        }

        public DataSet GetHolidayApplicCity(string LeaveDate)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"Select Thl_LocationCode, Thl_LeaveCutOffDate
                               From T_Holiday
                               Where Thl_HolidayDate = @LeaveDate";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@LeaveDate", LeaveDate);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            return ds;
        }

        public DataSet GetEmployeeLocationCodeForCurFuture(string EmployeeID)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"Select Mem_WorkLocationCode
                               From M_Employee
                               Where Mem_IDNo = @EmployeeID";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@EmployeeID", EmployeeID);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            return ds;
        }

        public DataSet GetEmployeeLocationCodeForPast(string EmployeeID)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"Select Mem_WorkLocationCode
                               From M_EmployeeHst
                               Where Mem_IDNo = @EmployeeID";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@EmployeeID", EmployeeID);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            return ds;
        }

        public DataSet CheckIfWithAmendment(string EmployeeID)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"Select Convert(char(10), Tlv_LeaveDate, 101) As 'Leave Date'
                                     ,Substring(Tlv_StartTime, 1, 2) + ':' + Substring(Tlv_StartTime, 3, 2) As 'Start Time'
                                     ,Substring(Tlv_EndTime, 1, 2) + ':' + Substring(Tlv_EndTime, 3, 2) As 'End Time'
                                     ,Tlv_LeaveHours As 'Leave Hours'
                                     ,Tlv_ReasonForRequest As 'Reason'
                               From T_EmpLeaveHst
                               Where Tlv_IDNo = @EmployeeID
                               And Tlv_LeaveFlag <> 'A'";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@EmployeeID", EmployeeID);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            return ds;
        }
       
        public bool GetLeaveFlag()
        {
            bool flag = false;
            DataSet dsflag = new DataSet();            
            string sql = @"Select Tsc_SetFlag
                              From T_SettingControl 
                                Where Tsc_SystemCode = 'LEAVE'
                                      and Tsc_SettingCode = 'LVHRENTRY' 
                                      and  Tsc_SetFlag='False'";
            
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dsflag = dal.ExecuteDataSet(sql, CommandType.Text);

                if (dsflag.Tables[0].Rows.Count > 0)
                    flag = true;
                else
                    flag = false;

                dal.CloseDB();
            }
            return flag;
        }
        public string LeaveTypeDesc(string LeaveType)
        {
            DataSet ds = new DataSet();
            string desc = string.Empty;
            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@LeaveType", LeaveType);

            string sqlQuery = @"select Mcd_Name 
	                            from M_CodeDtl 
		                            where Mcd_CodeType='LVECATEGRY'
			                            and Mcd_RecordStatus='A' and Mcd_Code=@LeaveType";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);
                if (ds.Tables[0].Rows.Count > 0)
                    desc = ds.Tables[0].Rows[0][0].ToString().Trim();

                dal.CloseDB();
            }
            return desc;
        }
        public void UpdateLeaveBal(string Tlv_IDNo,
                                    string Tlv_LeaveDate,
                                    string Tlv_LeaveCode, string Status, DALHelper dal)
        {
            int retVal = 0;
            int paramIndex = 0;
            string leave = string.Empty;
            ParameterInfo[] paramInfo = new ParameterInfo[4];

            paramInfo[paramIndex++] = new ParameterInfo("@Tlv_IDNo", Tlv_IDNo);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlv_LeaveDate", Tlv_LeaveDate);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlv_LeaveCode", Tlv_LeaveCode);
            paramInfo[paramIndex++] = new ParameterInfo("@Tlv_LeaveStatus", Status);

            string sqlQuery = @"    declare @Value decimal(7,4)
                                    set @Value = (
                                    select Tlv_LeaveHours From T_EmpLeave
                                           Where Tlv_IDNo = @Tlv_IDNo
                                              And Tlv_LeaveDate = @Tlv_LeaveDate
                                              And Tlv_LeaveCode = @Tlv_LeaveCode
                                    )
                                    
                                    if(@Tlv_LeaveStatus)='A'
                                        begin
                                            update T_EmpLeaveLdg set Tll_OutCredit = Tll_OutCredit-@Value
                                                    where Tll_IDNo=@Tlv_IDNo and Tll_LeaveCode=@Tlv_LeaveCode
                                        end
                                    else
                                        begin
                                            update T_EmpLeaveLdg set Tll_PendingCredit = Tll_PendingCredit - @Value
                                                    where Tll_IDNo=@Tlv_IDNo and Tll_LeaveCode=@Tlv_LeaveCode
                                        end";



            try
            {
                retVal = dal.ExecuteNonQuery(string.Format(sqlQuery, leave), CommandType.Text, paramInfo);
            }
            catch (Exception e)
            {
                throw new PayrollException(e);
            }
           
        }

        public void DeleteEmployeeLeave(string EmployeeID, string LeaveType)
        {
            int retVal = 0;
            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@LeaveType", LeaveType);
            paramInfo[1] = new ParameterInfo("@EmployeeID", EmployeeID);

            string sqlQuery = @"DELETE FROM T_EmpLeaveLdg
                                WHERE Tll_IDNo = @EmployeeID
                                  AND Tll_LeaveCode = @LeaveType";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                retVal = dal.ExecuteNonQuery(sqlQuery, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
        }
    }
}
