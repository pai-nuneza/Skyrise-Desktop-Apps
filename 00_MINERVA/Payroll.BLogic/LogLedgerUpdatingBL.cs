using System;
using System.Configuration;
using System.Data;

using Payroll.DAL;
using CommonLibrary;

namespace Payroll.BLogic
{
    public class LogLedgerUpdatingBL : BaseBL
    {
        #region Overriden Functions
        public int Add(DataRow row, DALHelper dal)
        {
            int retVal = 0;
            int paramIndex = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[17];
            paramInfo[paramIndex++] = new ParameterInfo("@Ttl_IDNo", row["Ttl_IDNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttl_Date", row["Ttl_Date"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttl_ShiftCode", row["Ttl_ShiftCode"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttl_ActIn_1", row["Ttl_ActIn_1"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttl_ActOut_1", row["Ttl_ActOut_1"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttl_ActIn_2", row["Ttl_ActIn_2"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttl_ActOut_2", row["Ttl_ActOut_2"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ludatetime", row["Ludatetime"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttl_Remarks", row["Ttl_Remarks"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttl_DayCode", row["Ttl_DayCode"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttl_RestDayFlag", row["Ttl_RestDayFlag"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttl_HolidayFlag", row["Ttl_HolidayFlag"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttl_SkipService", row["Ttl_SkipService"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttl_AssumedFlag", row["Ttl_AssumedFlag"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttl_Amnesty", row["Ttl_Amnesty"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttl_DocumentBatchNo", row["Ttl_DocumentBatchNo"]);

            string sqlQuery = @"INSERT INTO T_EmpTimeRegisterLog    
                                                      (Ttl_IDNo
                                                        ,Ttl_Date
                                                        ,Ttl_DayCode
                                                        ,Ttl_RestDayFlag
                                                        ,Ttl_HolidayFlag
                                                        ,Ttl_ShiftCode
                                                        ,Ttl_ActIn_1
                                                        ,Ttl_ActOut_1
                                                        ,Ttl_ActIn_2
                                                        ,Ttl_ActOut_2
                                                        ,Ttl_SkipService
                                                        ,Ttl_AssumedFlag
                                                        ,Ttl_Amnesty
                                                        ,Ttl_Remarks
                                                        ,Ttl_DocumentBatchNo
                                                        ,Usr_Login
                                                        ,Ludatetime)
                                               VALUES
                                                     (@Ttl_IDNo
                                                     ,@Ttl_Date
                                                     ,@Ttl_DayCode
                                                     ,@Ttl_RestDayFlag
                                                     ,@Ttl_HolidayFlag
                                                     ,@Ttl_ShiftCode
                                                     ,@Ttl_ActIn_1
                                                     ,@Ttl_ActOut_1
                                                     ,@Ttl_ActIn_2
                                                     ,@Ttl_ActOut_2
                                                     ,@Ttl_SkipService
                                                     ,@Ttl_AssumedFlag
                                                     ,@Ttl_Amnesty
                                                     ,@Ttl_Remarks
                                                     ,@Ttl_DocumentBatchNo
                                                     ,@Usr_Login
                                                     ,@Ludatetime)";

            retVal = dal.ExecuteNonQuery(sqlQuery, CommandType.Text, paramInfo);
            return retVal;
        }
        public override int Add(DataRow row)
        {
            int retVal = 0;
            int paramIndex = 0;
            			                                               
            ParameterInfo[] paramInfo = new ParameterInfo[17];
            paramInfo[paramIndex++] = new ParameterInfo("@Ttl_IDNo", row["Ttl_IDNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttl_Date", row["Ttl_Date"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttl_DayCode", row["Ttl_DayCode"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttl_RestDayFlag", row["Ttl_RestDayFlag"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttl_HolidayFlag", row["Ttl_HolidayFlag"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttl_ShiftCode", row["Ttl_ShiftCode"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttl_ActIn_1", row["Ttl_ActIn_1"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttl_ActOut_1", row["Ttl_ActOut_1"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttl_ActIn_2", row["Ttl_ActIn_2"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttl_ActOut_2", row["Ttl_ActOut_2"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttl_SkipService", row["Ttl_SkipService"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttl_AssumedFlag", row["Ttl_AssumedFlag"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttl_Amnesty", row["Ttl_Amnesty"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttl_Remarks", row["Ttl_Remarks"], SqlDbType.VarChar, 200);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttl_DocumentBatchNo", row["Ttl_DocumentBatchNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ludatetime", row["Ludatetime"]);

            string sqlQuery = @"INSERT INTO T_EmpTimeRegisterLog
                                                      (Ttl_IDNo
                                                        ,Ttl_Date
                                                        ,Ttl_DayCode
                                                        ,Ttl_RestDayFlag
                                                        ,Ttl_HolidayFlag
                                                        ,Ttl_ShiftCode
                                                        ,Ttl_ActIn_1
                                                        ,Ttl_ActOut_1
                                                        ,Ttl_ActIn_2
                                                        ,Ttl_ActOut_2
                                                        ,Ttl_SkipService
                                                        ,Ttl_AssumedFlag
                                                        ,Ttl_Amnesty
                                                        ,Ttl_Remarks
                                                        ,Ttl_DocumentBatchNo
                                                        ,Usr_Login
                                                        ,Ludatetime)
                                               VALUES
                                                     (@Ttl_IDNo
                                                     ,@Ttl_Date
                                                     ,@Ttl_DayCode
                                                     ,@Ttl_RestDayFlag
                                                     ,@Ttl_HolidayFlag
                                                     ,@Ttl_ShiftCode
                                                     ,@Ttl_ActIn_1
                                                     ,@Ttl_ActOut_1
                                                     ,@Ttl_ActIn_2
                                                     ,@Ttl_ActOut_2
                                                     ,@Ttl_SkipService
                                                     ,@Ttl_AssumedFlag
                                                     ,@Ttl_Amnesty
                                                     ,@Ttl_Remarks
                                                     ,@Ttl_DocumentBatchNo
                                                     ,@Usr_Login
                                                     ,@Ludatetime)";

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

        public int AddLogTrailWithRemarks(DataRow row, DALHelper dal)
        {
            int retVal = 0;
            int paramIndex = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[17];
            paramInfo[paramIndex++] = new ParameterInfo("@Ttl_IDNo", row["Ttl_IDNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttl_Date", row["Ttl_Date"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttl_DayCode", row["Ttl_DayCode"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttl_RestDayFlag", row["Ttl_RestDayFlag"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttl_HolidayFlag", row["Ttl_HolidayFlag"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttl_ShiftCode", row["Ttl_ShiftCode"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttl_ActIn_1", row["Ttl_ActIn_1"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttl_ActOut_1", row["Ttl_ActOut_1"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttl_ActIn_2", row["Ttl_ActIn_2"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttl_ActOut_2", row["Ttl_ActOut_2"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttl_SkipService", row["Ttl_SkipService"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttl_AssumedFlag", row["Ttl_AssumedFlag"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttl_Amnesty", row["Ttl_Amnesty"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttl_Remarks", row["Ttl_Remarks"], SqlDbType.VarChar, 200);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttl_DocumentBatchNo", row["Ttl_DocumentBatchNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ludatetime", row["Ludatetime"]);

            string sqlQuery = @"INSERT INTO T_EmpTimeRegisterLog
                                                      (Ttl_IDNo
                                                        ,Ttl_Date
                                                        ,Ttl_DayCode
                                                        ,Ttl_RestDayFlag
                                                        ,Ttl_HolidayFlag
                                                        ,Ttl_ShiftCode
                                                        ,Ttl_ActIn_1
                                                        ,Ttl_ActOut_1
                                                        ,Ttl_ActIn_2
                                                        ,Ttl_ActOut_2
                                                        ,Ttl_SkipService
                                                        ,Ttl_AssumedFlag
                                                        ,Ttl_Amnesty
                                                        ,Ttl_Remarks
                                                        ,Ttl_DocumentBatchNo
	                                                    ,Usr_Login
	                                                    ,Ludatetime)
                                               VALUES
                                                     (@Ttl_IDNo
                                                     ,@Ttl_Date
                                                     ,@Ttl_DayCode
                                                     ,@Ttl_RestDayFlag
                                                     ,@Ttl_HolidayFlag
                                                     ,@Ttl_ShiftCode
                                                     ,@Ttl_ActIn_1
                                                     ,@Ttl_ActOut_1
                                                     ,@Ttl_ActIn_2
                                                     ,@Ttl_ActOut_2
                                                     ,@Ttl_SkipService
                                                     ,@Ttl_AssumedFlag
                                                     ,@Ttl_Amnesty
                                                     ,@Ttl_Remarks
                                                     ,@Ttl_DocumentBatchNo
                                                     ,@Usr_Login
                                                     ,@Ludatetime)";

            retVal = dal.ExecuteNonQuery(sqlQuery, CommandType.Text, paramInfo);
            return retVal;
        }

        public int AddDTROverride(DataRow row, DALHelper dal)
        {
            int retVal = 0;
            int paramIndex = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[7];
            paramInfo[paramIndex++] = new ParameterInfo("@Tdo_IDNo", row["Tdo_IDNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdo_Date", row["Tdo_Date"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdo_Type", row["Tdo_Type"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdo_Time", row["Tdo_Time"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tdo_Remarks", row["Tdo_Remarks"], SqlDbType.VarChar, 100);
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ludatetime", row["Ludatetime"]);

            string sqlQuery = @"INSERT INTO T_EmpDTROverride
                                                      (Tdo_IDNo,
                                                       Tdo_Date,
	                                                   Tdo_Type,
	                                                   Tdo_Time,
                                                       Tdo_Remarks,
	                                                   Usr_Login,
	                                                   Ludatetime)
                                               VALUES
                                                     (@Tdo_IDNo
                                                     ,@Tdo_Date
                                                     ,@Tdo_Type
                                                     ,@Tdo_Time
                                                     ,@Tdo_Remarks
                                                     ,@Usr_Login
                                                     ,@Ludatetime)";
            retVal = dal.ExecuteNonQuery(sqlQuery, CommandType.Text, paramInfo);

            return retVal;
        }


        public int UpdateLogledger(DataRow row, string TableName, DALHelper dal)
        {
            int retVal = 0;
            int paramIndex = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[14];
            paramInfo[paramIndex++] = new ParameterInfo("@Ttr_IDNo", row["Ttr_IDNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttr_Date", row["Ttr_Date"], SqlDbType.Date);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttr_DayCode", row["Ttr_DayCode"].ToString().Replace("*", ""));
            paramInfo[paramIndex++] = new ParameterInfo("@Ttr_ShiftCode", row["Ttr_ShiftCode"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttr_RestDayFlag", row["Ttr_RestDayFlag"].ToString());
            paramInfo[paramIndex++] = new ParameterInfo("@Ttr_HolidayFlag", row["Ttr_HolidayFlag"].ToString());
            paramInfo[paramIndex++] = new ParameterInfo("@Ttr_ActIn_1", row["Ttr_ActIn_1"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttr_ActOut_1", row["Ttr_ActOut_1"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttr_ActIn_2", row["Ttr_ActIn_2"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttr_ActOut_2", row["Ttr_ActOut_2"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttr_Amnesty", row["Ttr_Amnesty"].ToString());
            paramInfo[paramIndex++] = new ParameterInfo("@Ttr_SkipService", row["Ttr_SkipService"].ToString());
            paramInfo[paramIndex++] = new ParameterInfo("@Ttr_AssumedFlag", row["Ttr_AssumedFlag"].ToString());

            string sqlQuery = string.Format(@"UPDATE {0}
                                              SET Ttr_ShiftCode         = @Ttr_ShiftCode
                                                  ,Ttr_DayCode          = @Ttr_DayCode
                                                  ,Ttr_RestDayFlag      = @Ttr_RestDayFlag
                                                  ,Ttr_HolidayFlag      = @Ttr_HolidayFlag
                                                  ,Ttr_ActIn_1          = @Ttr_ActIn_1 
                                                  ,Ttr_ActOut_1         = @Ttr_ActOut_1
                                                  ,Ttr_ActIn_2          = @Ttr_ActIn_2
                                                  ,Ttr_ActOut_2         = @Ttr_ActOut_2
                                                  ,Ttr_Amnesty          = @Ttr_Amnesty
                                                  ,Ttr_SkipService      = @Ttr_SkipService
                                                  ,Ttr_AssumedFlag      = @Ttr_AssumedFlag
                                                  ,Usr_Login            = @Usr_Login
                                                  ,Ludatetime           = GETDATE()
                                              WHERE Ttr_IDNo = @Ttr_IDNo
                                                AND Ttr_Date = @Ttr_Date", TableName);



            retVal = dal.ExecuteNonQuery(sqlQuery, CommandType.Text, paramInfo);
            return retVal;
        }

        public override int Update(DataRow row)
        {
            int retVal = 0;
            int paramIndex = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[14];
            paramInfo[paramIndex++] = new ParameterInfo("@Ttr_ShiftCode", row["Ttr_ShiftCode"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttr_ActIn_1", row["Ttr_ActIn_1"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttr_ActOut_1", row["Ttr_ActOut_1"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttr_ActIn_2", row["Ttr_ActIn_2"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttr_ActOut_2", row["Ttr_ActOut_2"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttr_IDNo", row["Ttr_IDNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttr_Date", row["Ttr_Date"], SqlDbType.Date);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttr_CalendarType", row["Ttr_CalendarType"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttr_CalendarGroup", row["Ttr_CalendarGroup"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttr_DayCode", row["Ttr_DayCode"].ToString().Replace("*", ""));
            paramInfo[paramIndex++] = new ParameterInfo("@Ttr_RestDayFlag", row["Ttr_RestDayFlag"].ToString());
            paramInfo[paramIndex++] = new ParameterInfo("@Ttr_HolidayFlag", row["Ttr_HolidayFlag"].ToString());
            paramInfo[paramIndex++] = new ParameterInfo("@Ttr_Amnesty", row["Ttr_Amnesty"].ToString());

            string sqlQuery = @"UPDATE T_EmpTimeRegister
                                              SET Ttr_ShiftCode = @Ttr_ShiftCode,
                                                  Ttr_ActIn_1 = @Ttr_ActIn_1, 
                                                  Ttr_ActOut_1 = @Ttr_ActOut_1,
                                                  Ttr_ActIn_2 = @Ttr_ActIn_2,
                                                  Ttr_ActOut_2 = @Ttr_ActOut_2,
                                                  Ttr_CalendarType = @Ttr_CalendarType,
                                                  Ttr_CalendarGroup = @Ttr_CalendarGroup,
                                                  Ttr_DayCode = @Ttr_DayCode,
                                                  Ttr_RestDayFlag = @Ttr_RestDayFlag,
                                                  Ttr_HolidayFlag = @Ttr_HolidayFlag,
                                                  Ttr_Amnesty = @Ttr_Amnesty,
                                                  Usr_Login = @Usr_Login,
                                                  Ludatetime = GETDATE()
                                              WHERE Ttr_IDNo = @Ttr_IDNo
                                              AND Ttr_Date = @Ttr_Date";

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

        #region Functions for Log Ledger Extension
        public int AddtoLogExtension(string TableName, DataRow row, DALHelper dal)
        {
            int retVal = 0;
            int paramIndex = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[28];
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_IDNo", row["Ttm_IDNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_Date", row["Ttm_Date"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_PayCycle", row["Ttm_PayCycle"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActIn_01", row["Ttm_ActIn_01"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActOut_01", row["Ttm_ActOut_01"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActIn_02", row["Ttm_ActIn_02"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActOut_02", row["Ttm_ActOut_02"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActIn_03", row["Ttm_ActIn_03"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActOut_03", row["Ttm_ActOut_03"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActIn_04", row["Ttm_ActIn_04"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActOut_04", row["Ttm_ActOut_04"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActIn_05", row["Ttm_ActIn_05"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActOut_05", row["Ttm_ActOut_05"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActIn_06", row["Ttm_ActIn_06"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActOut_06", row["Ttm_ActOut_06"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActIn_07", row["Ttm_ActIn_07"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActOut_07", row["Ttm_ActOut_07"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActIn_08", row["Ttm_ActIn_08"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActOut_08", row["Ttm_ActOut_08"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActIn_09", row["Ttm_ActIn_09"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActOut_09", row["Ttm_ActOut_09"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActIn_10", row["Ttm_ActIn_10"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActOut_10", row["Ttm_ActOut_10"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActIn_11", row["Ttm_ActIn_11"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActOut_11", row["Ttm_ActOut_11"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActIn_12", row["Ttm_ActIn_12"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActOut_12", row["Ttm_ActOut_12"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);

            string sqlQuery = string.Format(@"INSERT INTO {0}
                                                      (Ttm_IDNo,
                                                       Ttm_Date,
                                                       Ttm_PayCycle,
                                                       Ttm_ActIn_01, 
                                                       Ttm_ActOut_01,
                                                       Ttm_ActIn_02,
                                                       Ttm_ActOut_02,
                                                       Ttm_ActIn_03, 
                                                       Ttm_ActOut_03,
                                                       Ttm_ActIn_04,
                                                       Ttm_ActOut_04,
                                                       Ttm_ActIn_05, 
                                                       Ttm_ActOut_05,
                                                       Ttm_ActIn_06,
                                                       Ttm_ActOut_06,
                                                       Ttm_ActIn_07, 
                                                       Ttm_ActOut_07,
                                                       Ttm_ActIn_08,
                                                       Ttm_ActOut_08,
                                                       Ttm_ActIn_09, 
                                                       Ttm_ActOut_09,
                                                       Ttm_ActIn_10,
                                                       Ttm_ActOut_10,
                                                       Ttm_ActIn_11,
                                                       Ttm_ActOut_11,
                                                       Ttm_ActIn_12,
                                                       Ttm_ActOut_12,
                                                       Usr_Login,
                                                       Ludatetime)
                                               VALUES
                                                     (@Ttm_IDNo
                                                      ,@Ttm_Date
                                                      ,@Ttm_PayCycle
                                                      ,@Ttm_ActIn_01 
                                                      ,@Ttm_ActOut_01
                                                      ,@Ttm_ActIn_02
                                                      ,@Ttm_ActOut_02
                                                      ,@Ttm_ActIn_03 
                                                      ,@Ttm_ActOut_03
                                                      ,@Ttm_ActIn_04
                                                      ,@Ttm_ActOut_04
                                                      ,@Ttm_ActIn_05 
                                                      ,@Ttm_ActOut_05
                                                      ,@Ttm_ActIn_06
                                                      ,@Ttm_ActOut_06
                                                      ,@Ttm_ActIn_07 
                                                      ,@Ttm_ActOut_07
                                                      ,@Ttm_ActIn_08
                                                      ,@Ttm_ActOut_08
                                                      ,@Ttm_ActIn_09 
                                                      ,@Ttm_ActOut_09
                                                      ,@Ttm_ActIn_10
                                                      ,@Ttm_ActOut_10
                                                      ,@Ttm_ActIn_11
                                                      ,@Ttm_ActOut_11
                                                      ,@Ttm_ActIn_12
                                                      ,@Ttm_ActOut_12
                                                      ,@Usr_Login
                                                      ,GETDATE())", TableName);

            //using (DALHelper dal = new DALHelper())
            //{
            //    dal.OpenDB();
            //    dal.BeginTransactionSnapshot();

            //    try
            //    {
            retVal = dal.ExecuteNonQuery(sqlQuery, CommandType.Text, paramInfo);
            //        dal.CommitTransactionSnapshot();
            //    }
            //    catch (Exception e)
            //    {
            //        dal.RollBackTransactionSnapshot();
            //        throw new PayrollException(e);
            //    }
            //    finally
            //    {
            //        dal.CloseDB();
            //    }
            //}

            return retVal;
        }

        //updating log ledger extension entries 
        public int UpdateLogExtension(string TableName, DataRow row, DALHelper dal)
        {
            int retVal = 0;
            int paramIndex = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[27];
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_IDNo", row["Ttm_IDNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_Date", row["Ttm_Date"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActIn_01", row["Ttm_ActIn_01"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActOut_01", row["Ttm_ActOut_01"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActIn_02", row["Ttm_ActIn_02"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActOut_02", row["Ttm_ActOut_02"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActIn_03", row["Ttm_ActIn_03"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActOut_03", row["Ttm_ActOut_03"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActIn_04", row["Ttm_ActIn_04"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActOut_04", row["Ttm_ActOut_04"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActIn_05", row["Ttm_ActIn_05"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActOut_05", row["Ttm_ActOut_05"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActIn_06", row["Ttm_ActIn_06"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActOut_06", row["Ttm_ActOut_06"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActIn_07", row["Ttm_ActIn_07"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActOut_07", row["Ttm_ActOut_07"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActIn_08", row["Ttm_ActIn_08"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActOut_08", row["Ttm_ActOut_08"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActIn_09", row["Ttm_ActIn_09"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActOut_09", row["Ttm_ActOut_09"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActIn_10", row["Ttm_ActIn_10"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActOut_10", row["Ttm_ActOut_10"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActIn_11", row["Ttm_ActIn_11"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActOut_11", row["Ttm_ActOut_11"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActIn_12", row["Ttm_ActIn_12"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActOut_12", row["Ttm_ActOut_12"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);


            string sqlQuery = string.Format(@"UPDATE {0}
                                              SET Ttm_ActIn_01 = @Ttm_ActIn_01, 
                                                  Ttm_ActOut_01 = @Ttm_ActOut_01,
                                                  Ttm_ActIn_02 = @Ttm_ActIn_02,
                                                  Ttm_ActOut_02 = @Ttm_ActOut_02,
                                                  Ttm_ActIn_03 = @Ttm_ActIn_03, 
                                                  Ttm_ActOut_03 = @Ttm_ActOut_03,
                                                  Ttm_ActIn_04 = @Ttm_ActIn_04,
                                                  Ttm_ActOut_04 = @Ttm_ActOut_04,
                                                  Ttm_ActIn_05 = @Ttm_ActIn_05, 
                                                  Ttm_ActOut_05 = @Ttm_ActOut_05,
                                                  Ttm_ActIn_06 = @Ttm_ActIn_06,
                                                  Ttm_ActOut_06 = @Ttm_ActOut_06,
                                                  Ttm_ActIn_07 = @Ttm_ActIn_07, 
                                                  Ttm_ActOut_07 = @Ttm_ActOut_07,
                                                  Ttm_ActIn_08 = @Ttm_ActIn_08,
                                                  Ttm_ActOut_08 = @Ttm_ActOut_08,
                                                  Ttm_ActIn_09 = @Ttm_ActIn_09, 
                                                  Ttm_ActOut_09 = @Ttm_ActOut_09,
                                                  Ttm_ActIn_10 = @Ttm_ActIn_10,
                                                  Ttm_ActOut_10 = @Ttm_ActOut_10,
                                                  Ttm_ActIn_11 = @Ttm_ActIn_11,
                                                  Ttm_ActOut_11 = @Ttm_ActOut_11,
                                                  Ttm_ActIn_12 = @Ttm_ActIn_12,
                                                  Ttm_ActOut_12 = @Ttm_ActOut_12,
                                                  Usr_Login = @Usr_Login,
                                                  Ludatetime = GETDATE()
                                              WHERE Ttm_IDNo = @Ttm_IDNo
                                              AND Ttm_Date = @Ttm_Date", TableName);

            retVal = dal.ExecuteNonQuery(sqlQuery, CommandType.Text, paramInfo);
            return retVal;
        }

        //updating log ledger extension entries 
        public int AddLogTrailExtension(DataRow row, DALHelper dal)
        {
            int retVal = 0;
            int paramIndex = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[37];
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_IDNo", row["Ttm_IDNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_Date", row["Ttm_Date"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_DayCode", row["Ttm_DayCode"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_RestDayFlag", row["Ttm_RestDayFlag"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_HolidayFlag", row["Ttm_HolidayFlag"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ShiftCode", row["Ttm_ShiftCode"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActIn_01", row["Ttm_ActIn_01"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActOut_01", row["Ttm_ActOut_01"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActIn_02", row["Ttm_ActIn_02"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActOut_02", row["Ttm_ActOut_02"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActIn_03", row["Ttm_ActIn_03"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActOut_03", row["Ttm_ActOut_03"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActIn_04", row["Ttm_ActIn_04"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActOut_04", row["Ttm_ActOut_04"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActIn_05", row["Ttm_ActIn_05"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActOut_05", row["Ttm_ActOut_05"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActIn_06", row["Ttm_ActIn_06"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActOut_06", row["Ttm_ActOut_06"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActIn_07", row["Ttm_ActIn_07"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActOut_07", row["Ttm_ActOut_07"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActIn_08", row["Ttm_ActIn_08"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActOut_08", row["Ttm_ActOut_08"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActIn_09", row["Ttm_ActIn_09"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActOut_09", row["Ttm_ActOut_09"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActIn_10", row["Ttm_ActIn_10"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActOut_10", row["Ttm_ActOut_10"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActIn_11", row["Ttm_ActIn_11"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActOut_11", row["Ttm_ActOut_11"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActIn_12", row["Ttm_ActIn_12"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_ActOut_12", row["Ttm_ActOut_12"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_SkipService", row["Ttm_SkipService"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_AssumedFlag", row["Ttm_AssumedFlag"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_Amnesty", row["Ttm_Amnesty"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_Remarks", row["Ttm_Remarks"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttm_DocumentBatchNo", row["Ttm_DocumentBatchNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ludatetime", row["Ludatetime"]);

            string sqlQuery = @"INSERT INTO T_EmpTimeRegisterLogMisc
                                                      (Ttm_IDNo
                                                        ,Ttm_Date
                                                        ,Ttm_DayCode
                                                        ,Ttm_RestDayFlag
                                                        ,Ttm_HolidayFlag
                                                        ,Ttm_ShiftCode
                                                        ,Ttm_ActIn_01
                                                        ,Ttm_ActOut_01
                                                        ,Ttm_ActIn_02
                                                        ,Ttm_ActOut_02
                                                        ,Ttm_ActIn_03
                                                        ,Ttm_ActOut_03
                                                        ,Ttm_ActIn_04
                                                        ,Ttm_ActOut_04
                                                        ,Ttm_ActIn_05
                                                        ,Ttm_ActOut_05
                                                        ,Ttm_ActIn_06
                                                        ,Ttm_ActOut_06
                                                        ,Ttm_ActIn_07
                                                        ,Ttm_ActOut_07
                                                        ,Ttm_ActIn_08
                                                        ,Ttm_ActOut_08
                                                        ,Ttm_ActIn_09
                                                        ,Ttm_ActOut_09
                                                        ,Ttm_ActIn_10
                                                        ,Ttm_ActOut_10
                                                        ,Ttm_ActIn_11
                                                        ,Ttm_ActOut_11
                                                        ,Ttm_ActIn_12
                                                        ,Ttm_ActOut_12
                                                        ,Ttm_SkipService
                                                        ,Ttm_AssumedFlag
                                                        ,Ttm_Amnesty
                                                        ,Ttm_Remarks
                                                        ,Ttm_DocumentBatchNo
                                                        ,Usr_Login
                                                        ,Ludatetime)
                                               VALUES
                                                     (@Ttm_IDNo
                                                     ,@Ttm_Date
                                                     ,@Ttm_DayCode
                                                     ,@Ttm_RestDayFlag
                                                     ,@Ttm_HolidayFlag
                                                     ,@Ttm_ShiftCode
                                                     ,@Ttm_ActIn_01
	                                                 ,@Ttm_ActOut_01
	                                                 ,@Ttm_ActIn_02
	                                                 ,@Ttm_ActOut_02
                                                     ,@Ttm_ActIn_03
	                                                 ,@Ttm_ActOut_03
	                                                 ,@Ttm_ActIn_04
	                                                 ,@Ttm_ActOut_04
                                                     ,@Ttm_ActIn_05
	                                                 ,@Ttm_ActOut_05
	                                                 ,@Ttm_ActIn_06
	                                                 ,@Ttm_ActOut_06
                                                     ,@Ttm_ActIn_07
	                                                 ,@Ttm_ActOut_07
	                                                 ,@Ttm_ActIn_08
	                                                 ,@Ttm_ActOut_08
                                                     ,@Ttm_ActIn_09
	                                                 ,@Ttm_ActOut_09
	                                                 ,@Ttm_ActIn_10
	                                                 ,@Ttm_ActOut_10
                                                     ,@Ttm_ActIn_11
	                                                 ,@Ttm_ActOut_11
                                                     ,@Ttm_ActIn_12
	                                                 ,@Ttm_ActOut_12
                                                     ,@Ttm_SkipService
                                                     ,@Ttm_AssumedFlag
                                                     ,@Ttm_Amnesty
                                                     ,@Ttm_Remarks
                                                     ,@Ttm_DocumentBatchNo
                                                     ,@Usr_Login
                                                     ,@Ludatetime)";

            retVal = dal.ExecuteNonQuery(sqlQuery, CommandType.Text, paramInfo);
            return retVal;
        }

        //checking if already in log ledger extension 
        public bool IsExistInLogLedgerExtension(string TableName, string IDNumber, string processDate, DALHelper dal)
        {
            bool exist = true;

            string sqlSelect = string.Format(@"SELECT COUNT(Ttm_IDNo)
                                                 FROM {0}
                                                 WHERE Ttm_IDNo = @Ttm_IDNo 
                                                 AND Ttm_Date = @Ttm_Date", TableName);

            ParameterInfo[] param = new ParameterInfo[2];
            param[0] = new ParameterInfo("@Ttm_IDNo", IDNumber);
            param[1] = new ParameterInfo("@Ttm_Date", processDate);

            DataSet ds = dal.ExecuteDataSet(sqlSelect, CommandType.Text, param);
            if (getIntValue(ds.Tables[0].Rows[0][0]) <= 0)
                exist = false;

            return exist;
        }
        #endregion

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

        #region Defined Functions

        public DataSet GetEmployeeTimeRegisterRecord(string TableName, string TableNameExt, string EmployeeID, string payPeriod, string CompanyCode, string CentralProfile, bool bExtension, string NAMEDSPLY)
        {
            DataSet ds = new DataSet();
            string sqlQuery = string.Empty;
            if (!bExtension)
                #region Two Pockets
                sqlQuery = string.Format(@"SELECT LEFT(CONVERT(CHAR(10), Ttr_Date, 107),6) as Ttr_Date
                                        ,Ttr_RestDayFlag
                                        ,Ttr_HolidayFlag
                                        ,SUBSTRING(DATENAME(weekday, Ttr_Date), 1, 3) as DOW
										, Ttr_DayCode + CASE WHEN (Ttr_RestDayFlag = 1) 
										    AND (Ttr_DayCode <> 'REST') THEN '*' ELSE '' end 
										    + ' ' + CASE when Ttr_AssumedPost <> 'N' THEN '>' ELSE '' END as Ttr_DayCode
                                        ,Ttr_ShiftCode + CASE WHEN ISNULL(Msh_RequiredLogsOnBreak,0) = 1 THEN  + '*' ELSE '' END AS Ttr_ShiftCode
                                        ,CASE Ttr_ActIn_1
                                            WHEN '0000' THEN ''
                                            WHEN '' THEN ''
                                            ELSE SUBSTRING(Ttr_ActIn_1,1,2) + ':' + SUBSTRING(Ttr_ActIn_1,3,2) 
                                         END as IN1
                                        ,CASE Ttr_ActOut_1
                                            WHEN '0000' THEN ''
                                            WHEN '' THEN ''
                                            ELSE SUBSTRING(Ttr_ActOut_1,1,2) + ':' + SUBSTRING(Ttr_ActOut_1,3,2) 
                                         END as OUT1
                                        ,CASE Ttr_ActIn_2
                                            WHEN '0000' THEN ''
                                            WHEN '' THEN ''
                                            ELSE SUBSTRING(Ttr_ActIn_2,1,2) + ':' + SUBSTRING(Ttr_ActIn_2,3,2) 
                                         END as IN2
                                        ,CASE Ttr_ActOut_2
                                            WHEN '0000' THEN ''
                                            WHEN '' THEN ''
                                            ELSE SUBSTRING(Ttr_ActOut_2,1,2) + ':' + SUBSTRING(Ttr_ActOut_2,3,2) 
                                         END as OUT2
                                        ,CASE WHEN Ttr_WFOTAdvHr > 0
								         THEN CONVERT(varchar,Ttr_WFOTAdvHr)
								         ELSE '' END as AdvOT ---Ttr_WFOTAdvHr
                                        ,CASE WHEN Ttr_WFOTPostHr > 0
								         THEN CONVERT(varchar,Ttr_WFOTPostHr) 
								         ELSE '' END as PostOT  ---Ttr_WFOTPostHr
										, CASE WHEN Ttr_WFPayLVHr > 0 
											  THEN Ttr_WFPayLVCode + ' ' + CONVERT(VARCHAR, Ttr_WFPayLVHr)
											  ELSE '' END as [PaidLV]
                                        , CASE WHEN Ttr_WFNoPayLVHr > 0 
											  THEN Ttr_WFNoPayLVCode + ' ' + CONVERT(VARCHAR, Ttr_WFNoPayLVHr)
											  ELSE '' END as [UnPaidLV]
                                        ,CASE WHEN Ttr_DayCode = 'SPL' AND Ttr_PayrollType = 'D' AND @HIDSPLDLY = 'TRUE'
									        THEN ''
									        ELSE
										       CASE WHEN CONVERT(VARCHAR,Ttr_ABSHour) = '0.00' THEN '' 
													ELSE  CONVERT(VARCHAR,Ttr_ABSHour)
										       END
                                         END as Ttr_ABSHour
                                        , CASE WHEN [Ttr_REGHour] > 0
                                            THEN CONVERT(VARCHAR, [Ttr_REGHour])
                                            ELSE ''
                                            END as Ttr_REGHour
                                        ,CASE WHEN [Ttr_OTHour] > 0
                                           THEN CONVERT(VARCHAR, [Ttr_OTHour])
                                           ELSE ''
                                           END as ActOT
                                        , CASE Cast(Ttr_LVHour as char)
											WHEN '0.00'
                                            THEN '' 
											ELSE Cast(Ttr_LVHour as char)
                                         END as Ttr_LVHour
                                        ,CASE Cast(Ttr_NDHour as char)
                                            WHEN '0.00'
                                            THEN ''
                                            ELSE Cast(Ttr_NDHour as char)
                                         END as Ttr_NDHour
                                        ,CASE Cast(Ttr_NDOTHour as char)
                                            WHEN '0.00'
                                            THEN ''
                                            ELSE Cast(Ttr_NDOTHour as char)
                                         END as Ttr_NDOTHour
                                         , CASE WHEN ([Ttr_PDHOLHour] <> 0 OR [Ttr_PDRESTLEGHOLDay] <> 0) 
                                        THEN  CONVERT(VARCHAR,Ttr_PDHOLHour 
                                        + (Ttr_PDRESTLEGHOLDay*8)) ELSE '' END
                                        as [PHOL]
                                        ,CASE WHEN Ttr_PrvDayHolRef IS NOT NULL
                                        THEN CONVERT(VARCHAR, Ttr_PrvDayHolRef, 101)
                                        ELSE ''
                                        END 
                                        as [PDAY]
                                        ,CASE WHEN Ttr_PrvDayHolRef IS NOT NULL
                                        THEN CONVERT(VARCHAR, CONVERT(DECIMAL(4,2), ISNULL(Ttr_PrvDayWorkMin, 0) / 60.0))
                                        ELSE ''
                                        END 
                                        as [PDHR]
										, CASE WHEN ([Ttr_PDHOLHour] > 0 OR [Ttr_PDRESTLEGHOLDay] > 0) 
											THEN  CONVERT(VARCHAR,Ttr_PDHOLHour 
											+ (Ttr_PDRESTLEGHOLDay*8)) ELSE '' END as Ttr_PDHOLHour
										,Ttr_WorkLocationCode
                                        ,ZIPCODE.Mcd_Name as [Ttr_WorkLocation]
                                        ,Ttr_WFFlexTime as Flex
                                        ,Ttr_WFFlexTime as orgFlex
                                        ,Ttr_AssumedFlag as AssumedPresent
                                        ,Ttr_AssumedFlag as AssumedPresentOrig
                                        ,Ttr_SkipService as ServiceOverride
                                        ,CONVERT(varchar(10), Ttr_Date, 101) as DBProcessDate
                                        ,Ttr_CalendarGroup 
                                        ,CALGRP.Mcd_Name as CalendarGroup
                                        ,'' as Remarks
										, CASE WHEN Ttr_AssumedPost ='T' THEN 'TAGGED'
											WHEN Ttr_AssumedPost ='A' THEN 'ACTUAL'
											ELSE '' END [Ttr_AssumedPost]
                                        ,Ttr_Amnesty as [Amnesty]
                                        ,Ttr_PayrollType
										,CASE (CASE CONVERT(varchar,Ttr_WFOTAdvHr) 
						                 WHEN '0.00'
						                 THEN '0.00' ELSE CONVERT(varchar,Ttr_WFOTAdvHr) END
                                         + '/' + CONVERT(varchar,Ttr_WFOTPostHr))    
                                         WHEN '0.00/0.00'
                                         THEN ''
                                         ELSE (CASE CONVERT(varchar,Ttr_WFOTAdvHr) 
						                 WHEN '0.00'
						                 THEN '0.00' ELSE CONVERT(varchar,Ttr_WFOTAdvHr) END
                                         + '/' + CONVERT(varchar,Ttr_WFOTPostHr))  
								        END as AdvPostOT
                                        ,CASE WHEN Ttr_WFPayLVCode = 'OB'
										 THEN (CASE WHEN Ttr_WFPayLVHr > 0 THEN Ttr_WFPayLVCode + ' ' + CONVERT(VARCHAR, Ttr_WFPayLVHr)
											   ELSE '' END)
									     ELSE (CASE WHEN CONVERT(DECIMAL(4,2), ISNULL([Ttr_PayLVMin], 0) / 60.0) > 0
											THEN  Ttr_WFPayLVCode + ' ' + CONVERT(VARCHAR, CONVERT(DECIMAL(4,2), ISNULL([Ttr_PayLVMin], 0) / 60.0))
											ELSE '' END) 
										 END as PayLeave 
								        ,CONVERT(VARCHAR(15), Ttr_Date, 101) as [ProcessDate101]
                                        ,{1}.Usr_Login AS Usr_Login
                                        ,ISNULL({0}.dbo.Udf_DisplayName({1}.Usr_Login,@NAMEDSPLY),{1}.Usr_Login) AS ModifiedBy
                                        ,{1}.Ludatetime AS Ludatetime
                                        ,ISNULL(Msh_RequiredLogsOnBreak,0) AS RequiredLogsOnBreak
                                        FROM {1} 
                                        LEFT JOIN {0}..M_CodeDtl ZIPCODE
	                                         ON ZIPCODE.Mcd_Code = Ttr_WorkLocationCode
                                             AND ZIPCODE.Mcd_CompanyCode = @CompanyCode
                                             AND ZIPCODE.Mcd_CodeType = 'ZIPCODE'
                                        LEFT JOIN {0}..M_CodeDtl CALGRP
	                                        ON CALGRP.Mcd_Code = Ttr_CalendarGroup
                                            AND CALGRP.Mcd_CompanyCode = @CompanyCode
                                            AND CALGRP.Mcd_CodeType = 'CALGRP'
                                        LEFT JOIN {0}..M_Shift ON Msh_ShiftCode = Ttr_ShiftCode
											AND Msh_CompanyCode = @CompanyCode
                                        WHERE Ttr_IDNo = @IDNo
                                            AND Ttr_PayCycle = @PayCycleCode
                                        ORDER BY {1}.Ttr_Date"
                                , CentralProfile
                                , TableName);
            #endregion
            else
            {
                #region Multiple Pockets
                sqlQuery = string.Format(@"SELECT LEFT(Convert(char(10), Ttr_Date, 107),6) as Ttr_Date
                                        ,Ttr_RestDayFlag
                                        ,Ttr_HolidayFlag
                                        ,SUBSTRING(DATENAME(weekday, Ttr_Date), 1, 3) as DOW
										, Ttr_DayCode + CASE WHEN (Ttr_RestDayFlag = 1) 
										    AND (Ttr_DayCode <> 'REST') THEN '*' ELSE '' END
										    + ' ' + CASE WHEN Ttr_AssumedPost <> 'N' THEN '>' ELSE '' END AS [Ttr_DayCode]
                                        ,Ttr_ShiftCode + CASE WHEN ISNULL(Msh_RequiredLogsOnBreak,0) = 1 THEN  + '*' ELSE '' END AS Ttr_ShiftCode
                                        
                                        ,CASE Ttr_ActIn_1
                                            WHEN '0000' THEN ''
                                            WHEN '' THEN ''
                                            ELSE SUBSTRING(Ttr_ActIn_1,1,2) + ':' + SUBSTRING(Ttr_ActIn_1,3,2) 
                                         END as IN1
                                        ,CASE Ttr_ActOut_1
                                            WHEN '0000' THEN ''
                                            WHEN '' THEN ''
                                            ELSE SUBSTRING(Ttr_ActOut_1,1,2) + ':' + SUBSTRING(Ttr_ActOut_1,3,2) 
                                         END as OUT1
                                        ,CASE Ttr_ActIn_2
                                            WHEN '0000' THEN ''
                                            WHEN '' THEN ''
                                            ELSE SUBSTRING(Ttr_ActIn_2,1,2) + ':' + SUBSTRING(Ttr_ActIn_2,3,2) 
                                         END as IN2
                                        ,CASE Ttr_ActOut_2
                                            WHEN '0000' THEN ''
                                            WHEN '' THEN ''
                                            ELSE SUBSTRING(Ttr_ActOut_2,1,2) + ':' + SUBSTRING(Ttr_ActOut_2,3,2) 
                                         END as OUT2
                                        ,CASE WHEN Ttr_WFOTAdvHr > 0
								         THEN CONVERT(varchar,Ttr_WFOTAdvHr)
								         ELSE '' END as AdvOT ---Ttr_WFOTAdvHr
                                        ,CASE WHEN Ttr_WFOTPostHr > 0
								         THEN CONVERT(varchar,Ttr_WFOTPostHr) 
								         ELSE '' END as PostOT  ---Ttr_WFOTPostHr
										, CASE WHEN Ttr_WFPayLVHr > 0 
											  THEN Ttr_WFPayLVCode + ' ' + CONVERT(VARCHAR, Ttr_WFPayLVHr)
											  ELSE '' END as [PaidLV]
                                        , CASE WHEN Ttr_WFNoPayLVHr > 0 
											  THEN Ttr_WFNoPayLVCode + ' ' + CONVERT(VARCHAR, Ttr_WFNoPayLVHr)
											  ELSE '' END as [UnPaidLV]
                                        ,CASE WHEN Ttr_DayCode = 'SPL' AND Ttr_PayrollType = 'D' AND @HIDSPLDLY = 'TRUE'
									        THEN ''
									        ELSE
										       CASE WHEN CONVERT(VARCHAR,Ttr_ABSHour) = '0.00' THEN '' 
													ELSE  CONVERT(VARCHAR,Ttr_ABSHour)
										       END
                                         END as Ttr_ABSHour
                                        ,CASE WHEN [Ttr_REGHour] > 0
                                            THEN CONVERT(VARCHAR, [Ttr_REGHour])
                                            ELSE ''
                                            END as Ttr_REGHour
                                        ,CASE WHEN [Ttr_OTHour] > 0
                                           THEN CONVERT(VARCHAR, [Ttr_OTHour])
                                           ELSE ''
                                           END as ActOT
                                        ,CASE Cast(Ttr_NDHour as char)
                                            WHEN '0.00'
                                            THEN ''
                                            ELSE Cast(Ttr_NDHour as char)
                                         END as Ttr_NDHour
                                        ,CASE Cast(Ttr_NDOTHour as char)
                                            WHEN '0.00'
                                            THEN ''
                                            ELSE Cast(Ttr_NDOTHour as char)
                                         END as Ttr_NDOTHour
										, CASE Cast(Ttr_LVHour as char)
											WHEN '0.00'
                                            THEN '' 
											ELSE Cast(Ttr_LVHour as char)
                                         END as Ttr_LVHour
										, CASE WHEN ([Ttr_PDHOLHour] > 0 OR [Ttr_PDRESTLEGHOLDay] > 0) 
											THEN  CONVERT(VARCHAR,Ttr_PDHOLHour 
											+ (Ttr_PDRESTLEGHOLDay*8)) ELSE '' END as Ttr_PDHOLHour
										,CASE WHEN Ttr_WFPayLVCode = 'OB'
										 THEN (CASE WHEN Ttr_WFPayLVHr > 0 THEN Ttr_WFPayLVCode + ' ' + CONVERT(VARCHAR, Ttr_WFPayLVHr)
											   ELSE '' END)
									     ELSE (CASE WHEN CONVERT(DECIMAL(4,2), ISNULL([Ttr_PayLVMin], 0) / 60.0) > 0
											THEN  Ttr_WFPayLVCode + ' ' + CONVERT(VARCHAR, CONVERT(DECIMAL(4,2), ISNULL([Ttr_PayLVMin], 0) / 60.0))
											ELSE '' END) 
										 END as PayLeave 
										,Ttr_WorkLocationCode
                                        ,ZIPCODE.Mcd_Name as [Ttr_WorkLocation]
                                        ,Ttr_WFFlexTime as Flex
                                        ,Ttr_WFFlexTime as orgFlex
                                        ,Ttr_AssumedFlag as AssumedPresent
                                        ,Ttr_AssumedFlag as AssumedPresentOrig
                                        ,CONVERT(varchar(10), Ttr_Date, 101) as DBProcessDate
                                        ,Ttr_CalendarGroup 
                                        ,CALGRP.Mcd_Name as CalendarGroup
                                        ,'' as Remarks
					                     ,CASE WHEN ([Ttr_PDHOLHour] <> 0 OR [Ttr_PDRESTLEGHOLDay] <> 0) 
                                        THEN  CONVERT(VARCHAR,Ttr_PDHOLHour 
                                        + (Ttr_PDRESTLEGHOLDay*8)) ELSE '' END
                                        as [PHOL]
                                        ,CASE WHEN Ttr_PrvDayHolRef IS NOT NULL
                                        THEN CONVERT(VARCHAR, Ttr_PrvDayHolRef, 101)
                                        ELSE ''
                                        END 
                                        as [PDAY]
                                        ,CASE WHEN Ttr_PrvDayHolRef IS NOT NULL
                                        THEN CONVERT(VARCHAR, CONVERT(DECIMAL(4,2), ISNULL(Ttr_PrvDayWorkMin, 0) / 60.0))
                                        ELSE ''
                                        END 
                                        AS [PDHR]
										,CASE WHEN Ttr_AssumedPost ='T' THEN 'TAGGED'
											WHEN Ttr_AssumedPost ='A' THEN 'ACTUAL'
											else '' end [Ttr_AssumedPost]
                                        ,Ttr_Amnesty as [Amnesty]
										,CASE (CASE CONVERT(varchar,Ttr_WFOTAdvHr) 
						                 WHEN '0.00'
						                 THEN '0.00' ELSE CONVERT(varchar,Ttr_WFOTAdvHr) END
                                         + '/' + CONVERT(varchar,Ttr_WFOTPostHr))    
                                         WHEN '0.00/0.00'
                                         THEN ''
                                         ELSE (CASE CONVERT(varchar,Ttr_WFOTAdvHr) 
						                 WHEN '0.00'
						                 THEN '0.00' ELSE CONVERT(varchar,Ttr_WFOTAdvHr) END
                                         + '/' + CONVERT(varchar,Ttr_WFOTPostHr))  
								        END as AdvPostOT
								        ,Convert(varchar(15), Ttr_Date, 101) as [ProcessDate101]
                                        ,Ttr_SkipService as ServiceOverride
                                        ,CASE Ttm_ActIn_01
                                            WHEN '0000' THEN ''
                                            WHEN '' THEN ''
                                            ELSE SUBSTRING(Ttm_ActIn_01,1,2) + ':' + SUBSTRING(Ttm_ActIn_01,3,2) 
                                            END as IN1_EXT
                                        ,CASE Ttm_ActOut_01
                                            WHEN '0000' THEN ''
                                            WHEN '' THEN ''
                                            ELSE SUBSTRING(Ttm_ActOut_01,1,2) + ':' + SUBSTRING(Ttm_ActOut_01,3,2) 
                                            END as OUT1_EXT
                                        ,CASE Ttm_ActIn_02
                                            WHEN '0000' THEN ''
                                            WHEN '' THEN ''
                                            ELSE SUBSTRING(Ttm_ActIn_02,1,2) + ':' + SUBSTRING(Ttm_ActIn_02,3,2) 
                                            END as IN2_EXT
                                        ,CASE Ttm_ActOut_02
                                            WHEN '0000' THEN ''
                                            WHEN '' THEN ''
                                            ELSE SUBSTRING(Ttm_ActOut_02,1,2) + ':' + SUBSTRING(Ttm_ActOut_02,3,2) 
                                            END as OUT2_EXT
                                        ,CASE Ttm_ActIn_03
                                            WHEN '0000' THEN ''
                                            WHEN '' THEN ''
                                            ELSE SUBSTRING(Ttm_ActIn_03,1,2) + ':' + SUBSTRING(Ttm_ActIn_03,3,2) 
                                            END as IN3_EXT
                                        ,CASE Ttm_ActOut_03
                                            WHEN '0000' THEN ''
                                            WHEN '' THEN ''
                                            ELSE SUBSTRING(Ttm_ActOut_03,1,2) + ':' + SUBSTRING(Ttm_ActOut_03,3,2) 
                                            END as OUT3_EXT
                                        ,CASE Ttm_ActIn_04
                                            WHEN '0000' THEN ''
                                            WHEN '' THEN ''
                                            ELSE SUBSTRING(Ttm_ActIn_04,1,2) + ':' + SUBSTRING(Ttm_ActIn_04,3,2) 
                                            END as IN4_EXT
                                        ,CASE Ttm_ActOut_04
                                            WHEN '0000' THEN ''
                                            WHEN '' THEN ''
                                            ELSE SUBSTRING(Ttm_ActOut_04,1,2) + ':' + SUBSTRING(Ttm_ActOut_04,3,2) 
                                            END as OUT4_EXT
                                        ,CASE Ttm_ActIn_05
                                            WHEN '0000' THEN ''
                                            WHEN '' THEN ''
                                            ELSE SUBSTRING(Ttm_ActIn_05,1,2) + ':' + SUBSTRING(Ttm_ActIn_05,3,2) 
                                            END as IN5_EXT
                                       ,CASE Ttm_ActOut_05
                                            WHEN '0000' THEN ''
                                            WHEN '' THEN ''
                                            ELSE SUBSTRING(Ttm_ActOut_05,1,2) + ':' + SUBSTRING(Ttm_ActOut_05,3,2) 
                                            END as OUT5_EXT
                                       ,CASE Ttm_ActIn_06
                                           WHEN '0000' THEN ''
                                           WHEN '' THEN ''
                                           ELSE SUBSTRING(Ttm_ActIn_06,1,2) + ':' + SUBSTRING(Ttm_ActIn_06,3,2) 
                                           END as IN6_EXT
                                       ,CASE Ttm_ActOut_06
                                           WHEN '0000' THEN ''
                                           WHEN '' THEN ''
                                           ELSE SUBSTRING(Ttm_ActOut_06,1,2) + ':' + SUBSTRING(Ttm_ActOut_06,3,2) 
                                           END as OUT6_EXT
                                      ,CASE Ttm_ActIn_07
                                           WHEN '0000' THEN ''
                                           WHEN '' THEN ''
                                           ELSE SUBSTRING(Ttm_ActIn_07,1,2) + ':' + SUBSTRING(Ttm_ActIn_07,3,2) 
                                           END as IN7_EXT
                                      ,CASE Ttm_ActOut_07
                                           WHEN '0000' THEN ''
                                           WHEN '' THEN ''
                                           ELSE SUBSTRING(Ttm_ActOut_07,1,2) + ':' + SUBSTRING(Ttm_ActOut_07,3,2) 
                                           END as OUT7_EXT
                                     ,CASE Ttm_ActIn_08
                                           WHEN '0000' THEN ''
                                           WHEN '' THEN ''
                                           ELSE SUBSTRING(Ttm_ActIn_08,1,2) + ':' + SUBSTRING(Ttm_ActIn_08,3,2) 
                                           END as IN8_EXT
                                     ,CASE Ttm_ActOut_08
                                           WHEN '0000' THEN ''
                                           WHEN '' THEN ''
                                           ELSE SUBSTRING(Ttm_ActOut_08,1,2) + ':' + SUBSTRING(Ttm_ActOut_08,3,2) 
                                           END as OUT8_EXT
                                     ,CASE Ttm_ActIn_09
                                           WHEN '0000' THEN ''
                                           WHEN '' THEN ''
                                           ELSE SUBSTRING(Ttm_ActIn_09,1,2) + ':' + SUBSTRING(Ttm_ActIn_09,3,2) 
                                           END as IN9_EXT
                                     ,CASE Ttm_ActOut_09
                                           WHEN '0000' THEN ''
                                           WHEN '' THEN ''
                                           ELSE SUBSTRING(Ttm_ActOut_09,1,2) + ':' + SUBSTRING(Ttm_ActOut_09,3,2) 
                                           END as OUT9_EXT
                                     ,CASE Ttm_ActIn_10
                                           WHEN '0000' THEN ''
                                           WHEN '' THEN ''
                                           ELSE SUBSTRING(Ttm_ActIn_10,1,2) + ':' + SUBSTRING(Ttm_ActIn_10,3,2) 
                                           END as IN10_EXT
                                     ,CASE Ttm_ActOut_10
                                           WHEN '0000' THEN ''
                                           WHEN '' THEN ''
                                           ELSE SUBSTRING(Ttm_ActOut_10,1,2) + ':' + SUBSTRING(Ttm_ActOut_10,3,2) 
                                           END as OUT10_EXT  
                                    ,CASE Ttm_ActIn_11
                                           WHEN '0000' THEN ''
                                           WHEN '' THEN ''
                                           ELSE SUBSTRING(Ttm_ActIn_11,1,2) + ':' + SUBSTRING(Ttm_ActIn_11,3,2) 
                                           END as IN11_EXT
                                     ,CASE Ttm_ActOut_11
                                           WHEN '0000' THEN ''
                                           WHEN '' THEN ''
                                           ELSE SUBSTRING(Ttm_ActOut_11,1,2) + ':' + SUBSTRING(Ttm_ActOut_11,3,2) 
                                           END as OUT11_EXT 
                                    ,CASE Ttm_ActIn_12
                                           WHEN '0000' THEN ''
                                           WHEN '' THEN ''
                                           ELSE SUBSTRING(Ttm_ActIn_12,1,2) + ':' + SUBSTRING(Ttm_ActIn_12,3,2) 
                                           END as IN12_EXT
                                     ,CASE Ttm_ActOut_12
                                           WHEN '0000' THEN ''
                                           WHEN '' THEN ''
                                           ELSE SUBSTRING(Ttm_ActOut_12,1,2) + ':' + SUBSTRING(Ttm_ActOut_12,3,2) 
                                           END as OUT12_EXT 
                                     ,{1}.Usr_Login AS Usr_Login
                                     ,ISNULL({0}.dbo.Udf_DisplayName({1}.Usr_Login,@NAMEDSPLY),{1}.Usr_Login) AS ModifiedBy
                                     ,{1}.Ludatetime AS Ludatetime  
                                     ,ISNULL(Msh_RequiredLogsOnBreak,0) AS RequiredLogsOnBreak    
                                     ,Msh_Schedule AS Schedule         
                                FROM {1} 
                                INNER JOIN {2} ON Ttr_IDNo = Ttm_IDNo
                                    AND Ttr_Date = Ttm_Date
                                LEFT JOIN {0}..M_CodeDtl ZIPCODE
	                                 ON ZIPCODE.Mcd_Code = Ttr_WorkLocationCode
                                     AND ZIPCODE.Mcd_CompanyCode = @CompanyCode
                                     AND ZIPCODE.Mcd_CodeType = 'ZIPCODE'
                                LEFT JOIN {0}..M_CodeDtl CALGRP
	                                 ON CALGRP.Mcd_Code = Ttr_CalendarGroup
                                     AND CALGRP.Mcd_CompanyCode = @CompanyCode
                                     AND CALGRP.Mcd_CodeType = 'CALGRP'
                                LEFT JOIN {0}..M_Shift ON Msh_ShiftCode = Ttr_ShiftCode
									 AND Msh_CompanyCode = @CompanyCode
                                WHERE Ttr_IDNo = @IDNo
                                AND Ttr_PayCycle = @PayCycleCode
                                ORDER BY {1}.Ttr_Date"
                                , CentralProfile
                                , TableName
                                , TableNameExt);
                #endregion
            }
            int idxx = 0;
            ParameterInfo[] paramInfo = new ParameterInfo[5];
            paramInfo[idxx++] = new ParameterInfo("@IDNo", EmployeeID);
            paramInfo[idxx++] = new ParameterInfo("@PayCycleCode", payPeriod);
            paramInfo[idxx++] = new ParameterInfo("@CompanyCode", CompanyCode);
            paramInfo[idxx++] = new ParameterInfo("@NAMEDSPLY", NAMEDSPLY);
            paramInfo[idxx++] = new ParameterInfo("@HIDSPLDLY", (new CommonBL()).GetParameterValueFromPayroll("HIDSPLDLY", CompanyCode));

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);
                dal.CloseDB();
            }
            return ds;
        }

        public DataSet GetEmployeeTimeRegisterRecord(string TableName, string TableNameExt, string EmployeeID, string payPeriod, string CompanyCode, string CentralProfile, bool bExtension, string NAMEDSPLY, string condition, DALHelper dal)
        {
            DataSet ds = new DataSet();
            string sqlQuery = string.Empty;
            if (!bExtension)
                #region Two Pockets
                sqlQuery = string.Format(@"SELECT LEFT(CONVERT(CHAR(10), Ttr_Date, 107),6) as Ttr_Date
                                        ,Ttr_RestDayFlag
                                        ,Ttr_HolidayFlag
                                        ,SUBSTRING(DATENAME(weekday, Ttr_Date), 1, 3) as DOW
										, Ttr_DayCode + CASE 
										    WHEN (Ttr_RestDayFlag = 1) 
										    AND (Ttr_DayCode <> 'REST') THEN '*' ELSE '' end 
										    + ' ' + CASE when Ttr_AssumedPost <> 'N' THEN '>' ELSE '' 
										    END as Ttr_DayCode
                                        ,Ttr_ShiftCode + CASE WHEN ISNULL(Msh_RequiredLogsOnBreak,0) = 1 THEN  + '*' ELSE '' END AS Ttr_ShiftCode
                                        ,CASE Ttr_ActIn_1
                                            WHEN '0000' THEN ''
                                            WHEN '' THEN ''
                                            ELSE SUBSTRING(Ttr_ActIn_1,1,2) + ':' + SUBSTRING(Ttr_ActIn_1,3,2) 
                                         END as IN1
                                        ,CASE Ttr_ActOut_1
                                            WHEN '0000' THEN ''
                                            WHEN '' THEN ''
                                            ELSE SUBSTRING(Ttr_ActOut_1,1,2) + ':' + SUBSTRING(Ttr_ActOut_1,3,2) 
                                         END as OUT1
                                        ,CASE Ttr_ActIn_2
                                            WHEN '0000' THEN ''
                                            WHEN '' THEN ''
                                            ELSE SUBSTRING(Ttr_ActIn_2,1,2) + ':' + SUBSTRING(Ttr_ActIn_2,3,2) 
                                         END as IN2
                                        ,CASE Ttr_ActOut_2
                                            WHEN '0000' THEN ''
                                            WHEN '' THEN ''
                                            ELSE SUBSTRING(Ttr_ActOut_2,1,2) + ':' + SUBSTRING(Ttr_ActOut_2,3,2) 
                                         END as OUT2
                                        ,CASE WHEN Ttr_WFOTAdvHr > 0
								         THEN CONVERT(varchar,Ttr_WFOTAdvHr)
								         ELSE '' END as AdvOT ---Ttr_WFOTAdvHr
                                        ,CASE WHEN Ttr_WFOTPostHr > 0
								         THEN CONVERT(varchar,Ttr_WFOTPostHr) 
								         ELSE '' END as PostOT  ---Ttr_WFOTPostHr
										, CASE WHEN Ttr_WFPayLVHr > 0 
											  THEN Ttr_WFPayLVCode + ' ' + CONVERT(VARCHAR, Ttr_WFPayLVHr)
											  ELSE '' END as [PaidLV]
                                        , CASE WHEN Ttr_WFNoPayLVHr > 0 
											  THEN Ttr_WFNoPayLVCode + ' ' + CONVERT(VARCHAR, Ttr_WFNoPayLVHr)
											  ELSE '' END as [UnPaidLV]
                                        ,CASE WHEN Ttr_DayCode = 'SPL' AND Ttr_PayrollType = 'D' AND @HIDSPLDLY = 'TRUE'
									        THEN ''
									        ELSE
										       CASE WHEN CONVERT(VARCHAR,Ttr_ABSHour) = '0.00' THEN '' 
													ELSE  CONVERT(VARCHAR,Ttr_ABSHour)
										       END
                                         END as Ttr_ABSHour
                                        ,CASE WHEN [Ttr_REGHour] > 0
                                            THEN CONVERT(VARCHAR, [Ttr_REGHour])
                                            ELSE ''
                                            END as Ttr_REGHour
                                        ,CASE WHEN [Ttr_OTHour] > 0
                                           THEN CONVERT(VARCHAR, [Ttr_OTHour])
                                           ELSE ''
                                           END as ActOT
                                        , CASE Cast(Ttr_LVHour as char)
											WHEN '0.00'
                                            THEN '' 
											ELSE Cast(Ttr_LVHour as char)
                                         END as Ttr_LVHour
                                        ,CASE Cast(Ttr_NDHour as char)
                                            WHEN '0.00'
                                            THEN ''
                                            ELSE Cast(Ttr_NDHour as char)
                                         END as Ttr_NDHour
                                        ,CASE Cast(Ttr_NDOTHour as char)
                                            WHEN '0.00'
                                            THEN ''
                                            ELSE Cast(Ttr_NDOTHour as char)
                                         END as Ttr_NDOTHour
                                       , CASE WHEN ([Ttr_PDHOLHour] <> 0 OR [Ttr_PDRESTLEGHOLDay] <> 0) 
                                        THEN  CONVERT(VARCHAR,Ttr_PDHOLHour 
                                        + (Ttr_PDRESTLEGHOLDay*8)) ELSE '' END
                                        as [PHOL]
                                        ,CASE WHEN Ttr_PrvDayHolRef IS NOT NULL
                                        THEN CONVERT(VARCHAR, Ttr_PrvDayHolRef, 101)
                                        ELSE ''
                                        END 
                                        as [PDAY]
                                        ,CASE WHEN Ttr_PrvDayHolRef IS NOT NULL
                                        THEN CONVERT(VARCHAR, CONVERT(DECIMAL(4,2), ISNULL(Ttr_PrvDayWorkMin, 0) / 60.0))
                                        ELSE ''
                                        END 
                                        as [PDHR]
                                        ,ZIPCODE.Mcd_Name as [Ttr_WorkLocation]
                                        ,Ttr_WFFlexTime as Flex
                                        ,Ttr_WFFlexTime as orgFlex
                                        ,Ttr_AssumedFlag as AssumedPresent
                                        ,Ttr_AssumedFlag as AssumedPresentOrig
                                        ,Ttr_SkipService as ServiceOverride
                                        ,CONVERT(varchar(10), Ttr_Date, 101) as DBProcessDate
                                        ,CALGRP.Mcd_Name as CalendarGroup
                                        ,'' as Remarks
										, CASE WHEN Ttr_AssumedPost ='T' THEN 'TAGGED'
											WHEN Ttr_AssumedPost ='A' THEN 'ACTUAL'
											ELSE '' END [Ttr_AssumedPost]
                                        ,Ttr_Amnesty as [Amnesty]
                                        ,Ttr_PayrollType
                                        ,{2}.Usr_Login AS Usr_Login
                                        ,ISNULL({0}.dbo.Udf_DisplayName({2}.Usr_Login,@NAMEDSPLY),{2}.Usr_Login) AS ModifiedBy
                                        ,{2}.Ludatetime AS Ludatetime
                                        FROM {2} 
                                        LEFT JOIN {0}..M_CodeDtl ZIPCODE
	                                         ON ZIPCODE.Mcd_Code = Ttr_WorkLocationCode
                                             AND ZIPCODE.Mcd_CompanyCode = @CompanyCode
                                             AND ZIPCODE.Mcd_CodeType = 'ZIPCODE'
                                        LEFT JOIN {0}..M_CodeDtl CALGRP
	                                        ON CALGRP.Mcd_Code = Ttr_CalendarGroup
                                            AND CALGRP.Mcd_CompanyCode = @CompanyCode
                                            AND CALGRP.Mcd_CodeType = 'CALGRP'
                                        LEFT JOIN {0}..M_Shift ON Msh_ShiftCode = Ttr_ShiftCode
											AND Msh_CompanyCode = @CompanyCode
                                        WHERE Ttr_IDNo = @IDNo
                                            {1}
                                        ORDER BY {2}.Ttr_Date"
                                    , CentralProfile
                                    , condition
                                    , TableName);
            #endregion
            else
            {
                #region Multiple Pockets
                sqlQuery = string.Format(@"SELECT LEFT(Convert(char(10), Ttr_Date, 107),6) as Ttr_Date
                                        ,Ttr_RestDayFlag
                                        ,Ttr_HolidayFlag
                                        ,SUBSTRING(DATENAME(weekday, Ttr_Date), 1, 3) as DOW
										, Ttr_DayCode + case 
										when (Ttr_RestDayFlag = 1) 
										and (Ttr_DayCode <> 'REST') then '*' else '' end 
										+ ' ' + case when Ttr_AssumedPost <> 'N' then '>' else '' 
										end as [Ttr_DayCode]
                                        ,Ttr_ShiftCode + CASE WHEN ISNULL(Msh_RequiredLogsOnBreak,0) = 1 THEN  + '*' ELSE '' END AS Ttr_ShiftCode
                                        
                                        ,CASE Ttr_ActIn_1
                                            WHEN '0000' THEN ''
                                            WHEN '' THEN ''
                                            ELSE SUBSTRING(Ttr_ActIn_1,1,2) + ':' + SUBSTRING(Ttr_ActIn_1,3,2) 
                                         END as IN1
                                        ,CASE Ttr_ActOut_1
                                            WHEN '0000' THEN ''
                                            WHEN '' THEN ''
                                            ELSE SUBSTRING(Ttr_ActOut_1,1,2) + ':' + SUBSTRING(Ttr_ActOut_1,3,2) 
                                         END as OUT1
                                        ,CASE Ttr_ActIn_2
                                            WHEN '0000' THEN ''
                                            WHEN '' THEN ''
                                            ELSE SUBSTRING(Ttr_ActIn_2,1,2) + ':' + SUBSTRING(Ttr_ActIn_2,3,2) 
                                         END as IN2
                                        ,CASE Ttr_ActOut_2
                                            WHEN '0000' THEN ''
                                            WHEN '' THEN ''
                                            ELSE SUBSTRING(Ttr_ActOut_2,1,2) + ':' + SUBSTRING(Ttr_ActOut_2,3,2) 
                                         END as OUT2
                                        ,CASE WHEN Ttr_WFOTAdvHr > 0
								         THEN CONVERT(varchar,Ttr_WFOTAdvHr)
								         ELSE '' END as AdvOT ---Ttr_WFOTAdvHr
                                        ,CASE WHEN Ttr_WFOTPostHr > 0
								         THEN CONVERT(varchar,Ttr_WFOTPostHr) 
								         ELSE '' END as PostOT  ---Ttr_WFOTPostHr
										, CASE WHEN Ttr_WFPayLVHr > 0 
											  THEN Ttr_WFPayLVCode + ' ' + CONVERT(VARCHAR, Ttr_WFPayLVHr)
											  ELSE '' END as [PaidLV]
                                        , CASE WHEN Ttr_WFNoPayLVHr > 0 
											  THEN Ttr_WFNoPayLVCode + ' ' + CONVERT(VARCHAR, Ttr_WFNoPayLVHr)
											  ELSE '' END as [UnPaidLV]
                                        ,CASE WHEN Ttr_DayCode = 'SPL' AND Ttr_PayrollType = 'D' AND @HIDSPLDLY = 'TRUE'
									        THEN ''
									        ELSE
										       CASE WHEN CONVERT(VARCHAR,Ttr_ABSHour) = '0.00' THEN '' 
													ELSE  CONVERT(VARCHAR,Ttr_ABSHour)
										       END
                                         END as Ttr_ABSHour
                                        ,CASE WHEN [Ttr_REGHour] > 0
                                            THEN CONVERT(VARCHAR, [Ttr_REGHour])
                                            ELSE ''
                                            END as Ttr_REGHour
                                        ,CASE WHEN [Ttr_OTHour] > 0
                                           THEN CONVERT(VARCHAR, [Ttr_OTHour])
                                           ELSE ''
                                           END as ActOT
                                        ,CASE Cast(Ttr_NDHour as char)
                                            WHEN '0.00'
                                            THEN ''
                                            ELSE Cast(Ttr_NDHour as char)
                                         END as Ttr_NDHour
                                        ,CASE Cast(Ttr_NDOTHour as char)
                                            WHEN '0.00'
                                            THEN ''
                                            ELSE Cast(Ttr_NDOTHour as char)
                                         END as Ttr_NDOTHour
										, CASE Cast(Ttr_LVHour as char)
											WHEN '0.00'
                                            THEN '' 
											ELSE Cast(Ttr_LVHour as char)
                                         END as Ttr_LVHour
					                    , CASE WHEN ([Ttr_PDHOLHour] <> 0 OR [Ttr_PDRESTLEGHOLDay] <> 0) 
                                        THEN  CONVERT(VARCHAR,Ttr_PDHOLHour 
                                        + (Ttr_PDRESTLEGHOLDay*8)) ELSE '' END
                                        as [PHOL]
                                        ,CASE WHEN Ttr_PrvDayHolRef IS NOT NULL
                                        THEN CONVERT(VARCHAR, Ttr_PrvDayHolRef, 101)
                                        ELSE ''
                                        END 
                                        as [PDAY]
                                        ,CASE WHEN Ttr_PrvDayHolRef IS NOT NULL
                                        THEN CONVERT(VARCHAR, CONVERT(DECIMAL(4,2), ISNULL(Ttr_PrvDayWorkMin, 0) / 60.0))
                                        ELSE ''
                                        END 
                                        AS [PDHR]
                                        ,CASE Ttm_ActIn_01
                                            WHEN '0000' THEN ''
                                            WHEN '' THEN ''
                                            ELSE SUBSTRING(Ttm_ActIn_01,1,2) + ':' + SUBSTRING(Ttm_ActIn_01,3,2) 
                                            END as IN1_EXT
                                        ,CASE Ttm_ActOut_01
                                            WHEN '0000' THEN ''
                                            WHEN '' THEN ''
                                            ELSE SUBSTRING(Ttm_ActOut_01,1,2) + ':' + SUBSTRING(Ttm_ActOut_01,3,2) 
                                            END as OUT1_EXT
                                        ,CASE Ttm_ActIn_02
                                            WHEN '0000' THEN ''
                                            WHEN '' THEN ''
                                            ELSE SUBSTRING(Ttm_ActIn_02,1,2) + ':' + SUBSTRING(Ttm_ActIn_02,3,2) 
                                            END as IN2_EXT
                                        ,CASE Ttm_ActOut_02
                                            WHEN '0000' THEN ''
                                            WHEN '' THEN ''
                                            ELSE SUBSTRING(Ttm_ActOut_02,1,2) + ':' + SUBSTRING(Ttm_ActOut_02,3,2) 
                                            END as OUT2_EXT
                                        ,CASE Ttm_ActIn_03
                                            WHEN '0000' THEN ''
                                            WHEN '' THEN ''
                                            ELSE SUBSTRING(Ttm_ActIn_03,1,2) + ':' + SUBSTRING(Ttm_ActIn_03,3,2) 
                                            END as IN3_EXT
                                        ,CASE Ttm_ActOut_03
                                            WHEN '0000' THEN ''
                                            WHEN '' THEN ''
                                            ELSE SUBSTRING(Ttm_ActOut_03,1,2) + ':' + SUBSTRING(Ttm_ActOut_03,3,2) 
                                            END as OUT3_EXT
                                        ,CASE Ttm_ActIn_04
                                            WHEN '0000' THEN ''
                                            WHEN '' THEN ''
                                            ELSE SUBSTRING(Ttm_ActIn_04,1,2) + ':' + SUBSTRING(Ttm_ActIn_04,3,2) 
                                            END as IN4_EXT
                                        ,CASE Ttm_ActOut_04
                                            WHEN '0000' THEN ''
                                            WHEN '' THEN ''
                                            ELSE SUBSTRING(Ttm_ActOut_04,1,2) + ':' + SUBSTRING(Ttm_ActOut_04,3,2) 
                                            END as OUT4_EXT
                                        ,CASE Ttm_ActIn_05
                                            WHEN '0000' THEN ''
                                            WHEN '' THEN ''
                                            ELSE SUBSTRING(Ttm_ActIn_05,1,2) + ':' + SUBSTRING(Ttm_ActIn_05,3,2) 
                                            END as IN5_EXT
                                       ,CASE Ttm_ActOut_05
                                            WHEN '0000' THEN ''
                                            WHEN '' THEN ''
                                            ELSE SUBSTRING(Ttm_ActOut_05,1,2) + ':' + SUBSTRING(Ttm_ActOut_05,3,2) 
                                            END as OUT5_EXT
                                       ,CASE Ttm_ActIn_06
                                           WHEN '0000' THEN ''
                                           WHEN '' THEN ''
                                           ELSE SUBSTRING(Ttm_ActIn_06,1,2) + ':' + SUBSTRING(Ttm_ActIn_06,3,2) 
                                           END as IN6_EXT
                                       ,CASE Ttm_ActOut_06
                                           WHEN '0000' THEN ''
                                           WHEN '' THEN ''
                                           ELSE SUBSTRING(Ttm_ActOut_06,1,2) + ':' + SUBSTRING(Ttm_ActOut_06,3,2) 
                                           END as OUT6_EXT
                                      ,CASE Ttm_ActIn_07
                                           WHEN '0000' THEN ''
                                           WHEN '' THEN ''
                                           ELSE SUBSTRING(Ttm_ActIn_07,1,2) + ':' + SUBSTRING(Ttm_ActIn_07,3,2) 
                                           END as IN7_EXT
                                      ,CASE Ttm_ActOut_07
                                           WHEN '0000' THEN ''
                                           WHEN '' THEN ''
                                           ELSE SUBSTRING(Ttm_ActOut_07,1,2) + ':' + SUBSTRING(Ttm_ActOut_07,3,2) 
                                           END as OUT7_EXT
                                     ,CASE Ttm_ActIn_08
                                           WHEN '0000' THEN ''
                                           WHEN '' THEN ''
                                           ELSE SUBSTRING(Ttm_ActIn_08,1,2) + ':' + SUBSTRING(Ttm_ActIn_08,3,2) 
                                           END as IN8_EXT
                                     ,CASE Ttm_ActOut_08
                                           WHEN '0000' THEN ''
                                           WHEN '' THEN ''
                                           ELSE SUBSTRING(Ttm_ActOut_08,1,2) + ':' + SUBSTRING(Ttm_ActOut_08,3,2) 
                                           END as OUT8_EXT
                                     ,CASE Ttm_ActIn_09
                                           WHEN '0000' THEN ''
                                           WHEN '' THEN ''
                                           ELSE SUBSTRING(Ttm_ActIn_09,1,2) + ':' + SUBSTRING(Ttm_ActIn_09,3,2) 
                                           END as IN9_EXT
                                     ,CASE Ttm_ActOut_09
                                           WHEN '0000' THEN ''
                                           WHEN '' THEN ''
                                           ELSE SUBSTRING(Ttm_ActOut_09,1,2) + ':' + SUBSTRING(Ttm_ActOut_09,3,2) 
                                           END as OUT9_EXT
                                     ,CASE Ttm_ActIn_10
                                           WHEN '0000' THEN ''
                                           WHEN '' THEN ''
                                           ELSE SUBSTRING(Ttm_ActIn_10,1,2) + ':' + SUBSTRING(Ttm_ActIn_10,3,2) 
                                           END as IN10_EXT
                                     ,CASE Ttm_ActOut_10
                                           WHEN '0000' THEN ''
                                           WHEN '' THEN ''
                                           ELSE SUBSTRING(Ttm_ActOut_10,1,2) + ':' + SUBSTRING(Ttm_ActOut_10,3,2) 
                                           END as OUT10_EXT  
                                    ,CASE Ttm_ActIn_11
                                           WHEN '0000' THEN ''
                                           WHEN '' THEN ''
                                           ELSE SUBSTRING(Ttm_ActIn_11,1,2) + ':' + SUBSTRING(Ttm_ActIn_11,3,2) 
                                           END as IN11_EXT
                                     ,CASE Ttm_ActOut_11
                                           WHEN '0000' THEN ''
                                           WHEN '' THEN ''
                                           ELSE SUBSTRING(Ttm_ActOut_11,1,2) + ':' + SUBSTRING(Ttm_ActOut_11,3,2) 
                                           END as OUT11_EXT 
                                    ,CASE Ttm_ActIn_12
                                           WHEN '0000' THEN ''
                                           WHEN '' THEN ''
                                           ELSE SUBSTRING(Ttm_ActIn_12,1,2) + ':' + SUBSTRING(Ttm_ActIn_12,3,2) 
                                           END as IN12_EXT
                                     ,CASE Ttm_ActOut_12
                                           WHEN '0000' THEN ''
                                           WHEN '' THEN ''
                                           ELSE SUBSTRING(Ttm_ActOut_12,1,2) + ':' + SUBSTRING(Ttm_ActOut_12,3,2) 
                                           END as OUT12_EXT 
                                     ,CASE WHEN Ttr_AssumedPost ='T' THEN 'TAGGED'
											WHEN Ttr_AssumedPost ='A' THEN 'ACTUAL'
											ELSE '' END [Ttr_AssumedPost]
                                      ,Ttr_Amnesty as [Amnesty]
                                      ,Ttr_SkipService as ServiceOverride
                                      ,ZIPCODE.Mcd_Name as [Ttr_WorkLocation]
                                      ,Ttr_WFFlexTime as Flex
                                      ,Ttr_WFFlexTime as orgFlex
                                      ,Ttr_AssumedFlag as AssumedPresent
                                      ,Ttr_AssumedFlag as AssumedPresentOrig
                                      ,CONVERT(varchar(10), Ttr_Date, 101) as DBProcessDate 
                                      ,CALGRP.Mcd_Name as CalendarGroup
                                        ,'' as Remarks
                                     ,{2}.Usr_Login AS Usr_Login
                                     ,ISNULL({0}.dbo.Udf_DisplayName({2}.Usr_Login,@NAMEDSPLY),{2}.Usr_Login) AS ModifiedBy
                                     ,{2}.Ludatetime AS Ludatetime             
                                FROM {2} 
                                INNER JOIN {3} ON Ttr_IDNo = Ttm_IDNo
                                    AND Ttr_Date = Ttm_Date
                                LEFT JOIN {0}..M_CodeDtl ZIPCODE
	                                 ON ZIPCODE.Mcd_Code = Ttr_WorkLocationCode
                                     AND ZIPCODE.Mcd_CompanyCode = @CompanyCode
                                     AND ZIPCODE.Mcd_CodeType = 'ZIPCODE'
                                LEFT JOIN {0}..M_CodeDtl CALGRP
	                                 ON CALGRP.Mcd_Code = Ttr_CalendarGroup
                                     AND CALGRP.Mcd_CompanyCode = @CompanyCode
                                     AND CALGRP.Mcd_CodeType = 'CALGRP'
                                LEFT JOIN {0}..M_Shift ON Msh_ShiftCode = Ttr_ShiftCode
									 AND Msh_CompanyCode = @CompanyCode
                                WHERE Ttr_IDNo = @IDNo
                                    {1}
                                ORDER BY {2}.Ttr_Date"
                                , CentralProfile
                                , condition
                                , TableName
                                , TableNameExt);
                #endregion
            }
            int idxx = 0;
            ParameterInfo[] paramInfo = new ParameterInfo[4];
            paramInfo[idxx++] = new ParameterInfo("@IDNo", EmployeeID);
            paramInfo[idxx++] = new ParameterInfo("@CompanyCode", CompanyCode);
            paramInfo[idxx++] = new ParameterInfo("@NAMEDSPLY", NAMEDSPLY);
            paramInfo[idxx++] = new ParameterInfo("@HIDSPLDLY", (new CommonBL()).GetParameterValueFromPayroll("HIDSPLDLY", CompanyCode, dal));
            ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);
            return ds;
        }

        public DataTable GetEmployeePreviousLogRecordPerDate(string TableName, string TableNameExt, string EmployeeID, string payPeriod, string Date, bool bTrail, string CompanyCode, string CentralProfile, string NAMEDSPLY, bool bExtension, DALHelper dal)
        {
            string condition = string.Empty;
            if (bTrail)
                condition = string.Format("And Ttr_AdjPayCycle = '{0}'", payPeriod);

            DataTable dt = new DataTable();

            string sqlQuery = "";
            if (!bExtension)
                #region Two Pockets
                sqlQuery = string.Format(@"
                                    SELECT LEFT(Convert(char(10), Ttr_Date, 107),6) as Ttr_Date
                                        ,Ttr_RestDayFlag
                                        ,Ttr_HolidayFlag
                                        ,SUBSTRING(DATENAME(weekday, Ttr_Date), 1, 3) as DOW
										,Ttr_DayCode + CASE WHEN (Ttr_RestDayFlag = 1) 
										    AND (Ttr_DayCode <> 'REST') THEN '*' ELSE '' END 
										  + ' ' + CASE WHEN Ttr_AssumedPost <> 'N' THEN '>' ELSE '' END AS [Ttr_DayCode]
                                        ,Ttr_ShiftCode + CASE WHEN ISNULL(Msh_RequiredLogsOnBreak,0) = 1 THEN  + '*' ELSE '' END AS Ttr_ShiftCode
                                        
                                        ,CASE Ttr_ActIn_1
                                            WHEN '0000' THEN ''
                                            WHEN '' THEN ''
                                            ELSE SUBSTRING(Ttr_ActIn_1,1,2) + ':' + SUBSTRING(Ttr_ActIn_1,3,2) 
                                         END as IN1
                                        ,CASE Ttr_ActOut_1
                                            WHEN '0000' THEN ''
                                            WHEN '' THEN ''
                                            ELSE SUBSTRING(Ttr_ActOut_1,1,2) + ':' + SUBSTRING(Ttr_ActOut_1,3,2) 
                                         END as OUT1
                                        ,CASE Ttr_ActIn_2
                                            WHEN '0000' THEN ''
                                            WHEN '' THEN ''
                                            ELSE SUBSTRING(Ttr_ActIn_2,1,2) + ':' + SUBSTRING(Ttr_ActIn_2,3,2) 
                                         END as IN2
                                        ,CASE Ttr_ActOut_2
                                            WHEN '0000' THEN ''
                                            WHEN '' THEN ''
                                            ELSE SUBSTRING(Ttr_ActOut_2,1,2) + ':' + SUBSTRING(Ttr_ActOut_2,3,2) 
                                         END as OUT2
                                        ,CASE WHEN Ttr_WFOTAdvHr > 0
								         THEN CONVERT(varchar,Ttr_WFOTAdvHr)
								         ELSE '' END as AdvOT ---Ttr_WFOTAdvHr
                                        ,CASE WHEN Ttr_WFOTPostHr > 0
								         THEN CONVERT(varchar,Ttr_WFOTPostHr) 
								         ELSE '' END as PostOT  ---Ttr_WFOTPostHr
										, CASE WHEN Ttr_WFPayLVHr > 0 
											  THEN Ttr_WFPayLVCode + ' ' + CONVERT(VARCHAR, Ttr_WFPayLVHr)
											  ELSE '' END as [PaidLV]
                                        , CASE WHEN Ttr_WFNoPayLVHr > 0 
											  THEN Ttr_WFNoPayLVCode + ' ' + CONVERT(VARCHAR, Ttr_WFNoPayLVHr)
											  ELSE '' END as [UnPaidLV]
                                        ,CASE WHEN Ttr_DayCode = 'SPL' AND @HIDSPLDLY = 'TRUE'
									        THEN ''
									        ELSE
										       CASE WHEN Ttr_PDHOLHour < 0 And Ttr_DayCode <> 'REG'
												THEN CONVERT(VARCHAR, ABS(Ttr_PDHOLHour))
												ELSE CASE WHEN CONVERT(VARCHAR,Ttr_ABSHour) = '0.00' THEN '' 
													ELSE  CONVERT(VARCHAR,Ttr_ABSHour) END
										       END
                                         END as Ttr_ABSHour
                                        ,CASE WHEN [Ttr_REGHour] > 0
                                            THEN CONVERT(VARCHAR, [Ttr_REGHour])
                                            ELSE ''
                                            END as Ttr_REGHour
                                        ,CASE WHEN [Ttr_OTHour] > 0
                                           THEN CONVERT(VARCHAR, [Ttr_OTHour])
                                           ELSE ''
                                           END as ActOT
                                        ,CASE Cast(Ttr_NDHour as char)
                                            WHEN '0.00'
                                            THEN ''
                                            ELSE Cast(Ttr_NDHour as char)
                                         END as Ttr_NDHour
                                        ,CASE Cast(Ttr_NDOTHour as char)
                                            WHEN '0.00'
                                            THEN ''
                                            ELSE Cast(Ttr_NDOTHour as char)
                                         END as Ttr_NDOTHour
										, CASE Cast(Ttr_LVHour as char)
											WHEN '0.00'
                                            THEN '' 
											ELSE Cast(Ttr_LVHour as char)
                                         END as Ttr_LVHour
										---,Ttr_WorkLocationCode
                                        ,ZIPCODE.Mcd_Name as [Ttr_WorkLocation]
                                        ,Ttr_WFFlexTime as Flex
                                        ,Ttr_WFFlexTime as orgFlex
                                        ,Ttr_AssumedFlag as AssumedPresent
                                        ,Ttr_AssumedFlag as AssumedPresentOrig
                                        ,CONVERT(varchar(10), Ttr_Date, 101) as DBProcessDate
                                        ,Ttr_CalendarGroup 
                                        ,CALGRP.Mcd_Name as CalendarGroup
                                        ,'' as Remarks
					                    , CASE WHEN ([Ttr_PDHOLHour] <> 0 OR [Ttr_PDRESTLEGHOLDay] <> 0) 
                                        THEN  CONVERT(VARCHAR,Ttr_PDHOLHour 
                                        + (Ttr_PDRESTLEGHOLDay*8)) ELSE '' END
                                        as [PHOL]
                                        ,CASE WHEN Ttr_PrvDayHolRef IS NOT NULL
                                        THEN CONVERT(VARCHAR, Ttr_PrvDayHolRef, 101)
                                        ELSE ''
                                        END 
                                        as [PDAY]
                                        ,CASE WHEN Ttr_PrvDayHolRef IS NOT NULL
                                        THEN CONVERT(VARCHAR, CONVERT(DECIMAL(4,2), ISNULL(Ttr_PrvDayWorkMin, 0) / 60.0))
                                        ELSE ''
                                        END 
                                        as [PDHR]
										,CASE WHEN Ttr_AssumedPost ='T' THEN 'TAGGED'
											WHEN Ttr_AssumedPost ='A' THEN 'ACTUAL'
											ELSE '' END [Ttr_AssumedPost]
                                        ,Ttr_Amnesty as [Amnesty]
                                        ,Ttr_SkipService as ServiceOverride
                                        ,{2}.Usr_Login AS Usr_Login
                                        ,ISNULL({0}.dbo.Udf_DisplayName({2}.Usr_Login,@NAMEDSPLY),{2}.Usr_Login) AS ModifiedBy
                                        ,{2}.Ludatetime AS Ludatetime 
                                FROM {2} 
                                LEFT JOIN {0}..M_CodeDtl ZIPCODE
	                                  ON ZIPCODE.Mcd_Code = Ttr_WorkLocationCode
                                      AND ZIPCODE.Mcd_CompanyCode = @CompanyCode
                                      AND ZIPCODE.Mcd_CodeType = 'ZIPCODE'
                                LEFT JOIN {0}..M_CodeDtl CALGRP
	                                 ON CALGRP.Mcd_Code = Ttr_CalendarGroup
                                     AND CALGRP.Mcd_CompanyCode = @CompanyCode
                                     AND CALGRP.Mcd_CodeType = 'CALGRP'
                                LEFT JOIN {0}..M_Shift ON Msh_ShiftCode = Ttr_ShiftCode
									 AND Msh_CompanyCode = @CompanyCode
                                WHERE Ttr_IDNo = @IDNo
                                    AND {2}.Ttr_Date = @Date
                                    {1}
                                ", CentralProfile 
                                , condition
                                , TableName);
            #endregion
            else
                #region Multiple Pockets
                sqlQuery = string.Format(@"
                                    SELECT LEFT(CONVERT(CHAR(10), Ttr_Date, 107),6) as Ttr_Date
                                        ,Ttr_RestDayFlag
                                        ,Ttr_HolidayFlag
                                        ,SUBSTRING(DATENAME(weekday, Ttr_Date), 1, 3) as DOW
										, Ttr_DayCode + CASE WHEN (Ttr_RestDayFlag = 1) 
										    AND (Ttr_DayCode <> 'REST') THEN '*' ELSE '' END 
										    + ' ' + CASE WHEN Ttr_AssumedPost <> 'N' THEN '>' ELSE '' END as [Ttr_DayCode]
                                        ,Ttr_ShiftCode + CASE WHEN ISNULL(Msh_RequiredLogsOnBreak,0) = 1 THEN  + '*' ELSE '' END AS Ttr_ShiftCode
                                        
                                        ,CASE Ttr_ActIn_1
                                            WHEN '0000' THEN ''
                                            WHEN '' THEN ''
                                            ELSE SUBSTRING(Ttr_ActIn_1,1,2) + ':' + SUBSTRING(Ttr_ActIn_1,3,2) 
                                         END as IN1
                                        ,CASE Ttr_ActOut_1
                                            WHEN '0000' THEN ''
                                            WHEN '' THEN ''
                                            ELSE SUBSTRING(Ttr_ActOut_1,1,2) + ':' + SUBSTRING(Ttr_ActOut_1,3,2) 
                                         END as OUT1
                                        ,CASE Ttr_ActIn_2
                                            WHEN '0000' THEN ''
                                            WHEN '' THEN ''
                                            ELSE SUBSTRING(Ttr_ActIn_2,1,2) + ':' + SUBSTRING(Ttr_ActIn_2,3,2) 
                                         END as IN2
                                        ,CASE Ttr_ActOut_2
                                            WHEN '0000' THEN ''
                                            WHEN '' THEN ''
                                            ELSE SUBSTRING(Ttr_ActOut_2,1,2) + ':' + SUBSTRING(Ttr_ActOut_2,3,2) 
                                         END as OUT2
                                        ,CASE WHEN Ttr_WFOTAdvHr > 0
								         THEN CONVERT(VARCHAR,Ttr_WFOTAdvHr)
								         ELSE '' END as AdvOT ---Ttr_WFOTAdvHr
                                        ,CASE WHEN Ttr_WFOTPostHr > 0
								         THEN CONVERT(varchar,Ttr_WFOTPostHr) 
								         ELSE '' END as PostOT  ---Ttr_WFOTPostHr
										, CASE WHEN Ttr_WFPayLVHr > 0 
											  THEN Ttr_WFPayLVCode + ' ' + CONVERT(VARCHAR, Ttr_WFPayLVHr)
											  ELSE '' END as [PaidLV]
                                        , CASE WHEN Ttr_WFNoPayLVHr > 0 
											  THEN Ttr_WFNoPayLVCode + ' ' + CONVERT(VARCHAR, Ttr_WFNoPayLVHr)
											  ELSE '' END as [UnPaidLV]
                                        ,CASE WHEN Ttr_DayCode = 'SPL' AND @HIDSPLDLY = 'TRUE'
									        THEN ''
									        ELSE
										       CASE WHEN Ttr_PDHOLHour < 0 And Ttr_DayCode <> 'REG'
												THEN CONVERT(VARCHAR, ABS(Ttr_PDHOLHour))
												ELSE CASE WHEN CONVERT(VARCHAR,Ttr_ABSHour) = '0.00' THEN '' 
													ELSE  CONVERT(VARCHAR,Ttr_ABSHour) END
										       END
                                         END as Ttr_ABSHour
                                        ,CASE WHEN [Ttr_REGHour] > 0
                                            THEN CONVERT(VARCHAR, [Ttr_REGHour])
                                            ELSE ''
                                            END as Ttr_REGHour
                                        ,CASE WHEN [Ttr_OTHour] > 0
                                           THEN CONVERT(VARCHAR, [Ttr_OTHour])
                                           ELSE ''
                                           END as ActOT
                                        ,CASE Cast(Ttr_NDHour as char)
                                            WHEN '0.00'
                                            THEN ''
                                            ELSE Cast(Ttr_NDHour as char)
                                         END as Ttr_NDHour
                                        ,CASE Cast(Ttr_NDOTHour as char)
                                            WHEN '0.00'
                                            THEN ''
                                            ELSE Cast(Ttr_NDOTHour as char)
                                         END as Ttr_NDOTHour
										, CASE Cast(Ttr_LVHour as char)
											WHEN '0.00'
                                            THEN '' 
											ELSE Cast(Ttr_LVHour as char)
                                         END as Ttr_LVHour
										---,Ttr_WorkLocationCode
                                        ,ZIPCODE.Mcd_Name as [Ttr_WorkLocation]
                                        ,Ttr_WFFlexTime as Flex
                                        ,Ttr_WFFlexTime as orgFlex
                                        ,Ttr_AssumedFlag as AssumedPresent
                                        ,Ttr_AssumedFlag as AssumedPresentOrig
                                        ,CONVERT(varchar(10), Ttr_Date, 101) as DBProcessDate
                                        ,Ttr_CalendarGroup 
                                        ,CALGRP.Mcd_Name as CalendarGroup
                                        ,'' as Remarks
					                    ,CASE WHEN ([Ttr_PDHOLHour] <> 0 OR [Ttr_PDRESTLEGHOLDay] <> 0) 
                                        THEN  CONVERT(VARCHAR,Ttr_PDHOLHour 
                                        + (Ttr_PDRESTLEGHOLDay*8)) ELSE '' END
                                        as [PHOL]
                                        ,CASE WHEN Ttr_PrvDayHolRef IS NOT NULL
                                        THEN CONVERT(VARCHAR, Ttr_PrvDayHolRef, 101)
                                        ELSE ''
                                        END 
                                        as [PDAY]
                                        ,CASE WHEN Ttr_PrvDayHolRef IS NOT NULL
                                        THEN CONVERT(VARCHAR, CONVERT(DECIMAL(4,2), ISNULL(Ttr_PrvDayWorkMin, 0) / 60.0))
                                        ELSE ''
                                        END 
                                        as [PDHR]
										,CASE WHEN Ttr_AssumedPost ='T' THEN 'TAGGED'
											WHEN Ttr_AssumedPost ='A' THEN 'ACTUAL'
											ELSE '' END [Ttr_AssumedPost]
                                        ,Ttr_Amnesty as [Amnesty]
                                        ,Ttr_SkipService as ServiceOverride
                                        ,{2}.Usr_Login AS Usr_Login
                                        ,ISNULL({0}.dbo.Udf_DisplayName({2}.Usr_Login,@NAMEDSPLY),{2}.Usr_Login) AS ModifiedBy
                                        ,{2}.Ludatetime AS Ludatetime 
                                FROM {2} 
                                LEFT JOIN {0}..M_CodeDtl ZIPCODE
	                                  ON ZIPCODE.Mcd_Code = Ttr_WorkLocationCode
                                      AND ZIPCODE.Mcd_CompanyCode = @CompanyCode
                                      AND ZIPCODE.Mcd_CodeType = 'ZIPCODE'
                                LEFT JOIN {0}..M_CodeDtl CALGRP
	                                 ON CALGRP.Mcd_Code = Ttr_CalendarGroup
                                     AND CALGRP.Mcd_CompanyCode = @CompanyCode
                                     AND CALGRP.Mcd_CodeType = 'CALGRP'
                                LEFT JOIN {0}..M_Shift ON Msh_ShiftCode = Ttr_ShiftCode
									 AND Msh_CompanyCode = @CompanyCode
                                WHERE Ttr_IDNo = @IDNo
                                    AND {2}.Ttr_Date = @Date
                                    {1}
                                ", CentralProfile
                                , condition
                                , TableName);
            #endregion


            ParameterInfo[] paramInfo = new ParameterInfo[5];
            paramInfo[0] = new ParameterInfo("@IDNo", EmployeeID);
            paramInfo[1] = new ParameterInfo("@Date", Date, SqlDbType.Date);
            paramInfo[2] = new ParameterInfo("@CompanyCode", CompanyCode);
            paramInfo[3] = new ParameterInfo("@NAMEDSPLY", NAMEDSPLY);
            paramInfo[4] = new ParameterInfo("@HIDSPLDLY", (new CommonBL()).GetParameterValueFromPayroll("HIDSPLDLY", CompanyCode, dal));

            dt = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo).Tables[0];
            return dt;
        }

        public DataTable GetLogTrail(string EmployeeID, string processDate, string CompanyCode, string CentralProfile, string NAMEDSPLY, DALHelper dal)
        {
            DataTable dt = new DataTable();
            #region query
            string sqlQuery = string.Format(@"SELECT Ttl_DayCode as [Day Code]
                                                     ,Ttl_ShiftCode + CASE WHEN ISNULL(Msh_RequiredLogsOnBreak,0) = 1 THEN  + '*' ELSE '' END as [Shift Code]
                                                     ,CASE Ttl_RestDayFlag
			                                            WHEN 'TRUE'
			                                            THEN 'YES'
			                                            ELSE 'NO'
		                                             END as [Is Restday]
													 , CASE Ttl_HolidayFlag
			                                            WHEN 'TRUE'
			                                            THEN 'YES'
			                                            ELSE 'NO'
		                                             END as [Is Holiday]
                                                     ,CASE Ttl_ActIn_1
                                                        WHEN '0000' THEN Null
                                                        ELSE SUBSTRING(Ttl_ActIn_1, 1, 2) + ':' + SUBSTRING(Ttl_ActIn_1, 3, 2)
                                                     END AS [In 1]
                                                     ,CASE Ttl_ActOut_1
                                                        WHEN '0000' THEN Null
                                                        ELSE SUBSTRING(Ttl_ActOut_1, 1, 2) + ':' + SUBSTRING(Ttl_ActOut_1, 3, 2)
                                                     END AS [Out 1]
                                                     ,CASE Ttl_ActIn_2
                                                        WHEN '0000' THEN Null
                                                        ELSE SUBSTRING(Ttl_ActIn_2, 1, 2) + ':' + SUBSTRING(Ttl_ActIn_2, 3, 2)
                                                     END AS [In 2]
                                                     ,CASE Ttl_ActOut_2
                                                        WHEN '0000' THEN Null
                                                        ELSE SUBSTRING(Ttl_ActOut_2, 1, 2) + ':' + SUBSTRING(Ttl_ActOut_2, 3, 2)
                                                     END AS [Out 2] 
                                                    ,CASE Ttl_AssumedFlag 
                                                        WHEN 'TRUE'
			                                            THEN 'YES'
			                                            ELSE 'NO'
                                                    END AS [Tag as Present]
                                                    ,SKIPSERVICE.Mcd_Name AS [Skip Service]
                                                    ,AMNESTY.Mcd_Name AS [Amnesty]
                                                    ,Ttl_DocumentBatchNo AS [Doc No Ref]
                                                    ,Ttl_Remarks AS [Remarks]
                                                    ,ISNULL({0}.dbo.Udf_DisplayName(EmpLogT.Usr_Login,@NAMEDSPLY),EmpLogT.Usr_Login) AS [Modified by]
                                                    ,EmpLogT.Ludatetime AS [Last modified date]
                                               FROM T_EmpTimeRegisterLog AS EmpLogT
                                               LEFT JOIN {0}..M_CodeDtl AMNESTY
	                                                ON AMNESTY.Mcd_Code = Ttl_Amnesty
                                                    AND AMNESTY.Mcd_CompanyCode = @CompanyCode
                                                    AND AMNESTY.Mcd_CodeType = 'AMNESTY'
                                               LEFT JOIN {0}..M_CodeDtl SKIPSERVICE
	                                                ON SKIPSERVICE.Mcd_Code = Ttl_SkipService
                                                    AND SKIPSERVICE.Mcd_CompanyCode = @CompanyCode
                                                    AND SKIPSERVICE.Mcd_CodeType = 'SKIPSERVICE'
                                               LEFT JOIN {0}..M_Shift ON Msh_ShiftCode = Ttl_ShiftCode
											        AND Msh_CompanyCode = @CompanyCode
                                               WHERE Ttl_IDNo = @IDNo
                                               AND Ttl_Date = @Date
                                               ORDER BY EmpLogT.Ludatetime DESC"
                                                , CentralProfile);
            #endregion

            int idxx = 0;
            ParameterInfo[] paramInfo = new ParameterInfo[4];
            paramInfo[idxx++] = new ParameterInfo("@IDNo", EmployeeID);
            paramInfo[idxx++] = new ParameterInfo("@Date", processDate, SqlDbType.Date);
            paramInfo[idxx++] = new ParameterInfo("@CompanyCode", CompanyCode);
            paramInfo[idxx++] = new ParameterInfo("@NAMEDSPLY", NAMEDSPLY);

            dt = dal.ExecuteDataSet(sqlQuery).Tables[0];
            return dt;
        }

        public DataTable GetLogTrailMisc(string EmployeeID, string processDate, string CompanyCode, string CentralProfile, string NAMEDSPLY, DALHelper dal)
        {
            DataTable dt = new DataTable();
            #region query
            string sqlQuery = string.Format(@"SELECT Ttm_DayCode as [Day Code]
                                                     ,Ttm_ShiftCode + CASE WHEN ISNULL(Msh_RequiredLogsOnBreak,0) = 1 THEN  + '*' ELSE '' END as [Shift Code]
                                                     ,CASE Ttm_RestDayFlag
			                                            WHEN 'TRUE'
			                                            THEN 'YES'
			                                            ELSE 'NO'
		                                             END as [Is Restday]
													 , CASE Ttm_HolidayFlag
			                                            WHEN 'TRUE'
			                                            THEN 'YES'
			                                            ELSE 'NO'
		                                             END as [Is Holiday]
                                                     ,CASE Ttm_ActIn_01
                                                        WHEN '0000' THEN Null
                                                        ELSE SUBSTRING(Ttm_ActIn_01, 1, 2) + ':' + SUBSTRING(Ttm_ActIn_01, 3, 2)
                                                     END AS [In 1]
                                                     ,CASE Ttm_ActOut_01
                                                        WHEN '0000' THEN Null
                                                        ELSE SUBSTRING(Ttm_ActOut_01, 1, 2) + ':' + SUBSTRING(Ttm_ActOut_01, 3, 2)
                                                     END AS [Out 1]
                                                     ,CASE Ttm_ActIn_02
                                                        WHEN '0000' THEN Null
                                                        ELSE SUBSTRING(Ttm_ActIn_02, 1, 2) + ':' + SUBSTRING(Ttm_ActIn_02, 3, 2)
                                                     END AS [In 2]
                                                     ,CASE Ttm_ActOut_02
                                                        WHEN '0000' THEN Null
                                                        ELSE SUBSTRING(Ttm_ActOut_02, 1, 2) + ':' + SUBSTRING(Ttm_ActOut_02, 3, 2)
                                                     END AS [Out 2] 
                                                    ,CASE Ttm_ActIn_03
                                                        WHEN '0000' THEN Null
                                                        ELSE SUBSTRING(Ttm_ActIn_03, 1, 2) + ':' + SUBSTRING(Ttm_ActIn_03, 3, 2)
                                                     END AS [In 3]
                                                     ,CASE Ttm_ActOut_03
                                                        WHEN '0000' THEN Null
                                                        ELSE SUBSTRING(Ttm_ActOut_03, 1, 2) + ':' + SUBSTRING(Ttm_ActOut_03, 3, 2)
                                                     END AS [Out 3] 
                                                    ,CASE Ttm_ActIn_04
                                                        WHEN '0000' THEN Null
                                                        ELSE SUBSTRING(Ttm_ActIn_04, 1, 2) + ':' + SUBSTRING(Ttm_ActIn_04, 3, 2)
                                                     END AS [In 4]
                                                     ,CASE Ttm_ActOut_04
                                                        WHEN '0000' THEN Null
                                                        ELSE SUBSTRING(Ttm_ActOut_04, 1, 2) + ':' + SUBSTRING(Ttm_ActOut_04, 3, 2)
                                                     END AS [Out 4] 
                                                    ,CASE Ttm_ActIn_05
                                                        WHEN '0000' THEN Null
                                                        ELSE SUBSTRING(Ttm_ActIn_05, 1, 2) + ':' + SUBSTRING(Ttm_ActIn_05, 3, 2)
                                                     END AS [In 5]
                                                     ,CASE Ttm_ActOut_05
                                                        WHEN '0000' THEN Null
                                                        ELSE SUBSTRING(Ttm_ActOut_05, 1, 2) + ':' + SUBSTRING(Ttm_ActOut_05, 3, 2)
                                                     END AS [Out 5] 
                                                    ,CASE Ttm_ActIn_06
                                                        WHEN '0000' THEN Null
                                                        ELSE SUBSTRING(Ttm_ActIn_06, 1, 2) + ':' + SUBSTRING(Ttm_ActIn_06, 3, 2)
                                                     END AS [In 6]
                                                     ,CASE Ttm_ActOut_06
                                                        WHEN '0000' THEN Null
                                                        ELSE SUBSTRING(Ttm_ActOut_06, 1, 2) + ':' + SUBSTRING(Ttm_ActOut_06, 3, 2)
                                                     END AS [Out 6] 
                                                    ,CASE Ttm_ActIn_07
                                                        WHEN '0000' THEN Null
                                                        ELSE SUBSTRING(Ttm_ActIn_07, 1, 2) + ':' + SUBSTRING(Ttm_ActIn_07, 3, 2)
                                                     END AS [In 7]
                                                     ,CASE Ttm_ActOut_07
                                                        WHEN '0000' THEN Null
                                                        ELSE SUBSTRING(Ttm_ActOut_07, 1, 2) + ':' + SUBSTRING(Ttm_ActOut_07, 3, 2)
                                                     END AS [Out 7] 
                                                    ,CASE Ttm_ActIn_08
                                                        WHEN '0000' THEN Null
                                                        ELSE SUBSTRING(Ttm_ActIn_08, 1, 2) + ':' + SUBSTRING(Ttm_ActIn_08, 3, 2)
                                                     END AS [In 8]
                                                     ,CASE Ttm_ActOut_08
                                                        WHEN '0000' THEN Null
                                                        ELSE SUBSTRING(Ttm_ActOut_08, 1, 2) + ':' + SUBSTRING(Ttm_ActOut_08, 3, 2)
                                                     END AS [Out 8] 
                                                    ,CASE Ttm_ActIn_09
                                                        WHEN '0000' THEN Null
                                                        ELSE SUBSTRING(Ttm_ActIn_09, 1, 2) + ':' + SUBSTRING(Ttm_ActIn_09, 3, 2)
                                                     END AS [In 9]
                                                     ,CASE Ttm_ActOut_09
                                                        WHEN '0000' THEN Null
                                                        ELSE SUBSTRING(Ttm_ActOut_09, 1, 2) + ':' + SUBSTRING(Ttm_ActOut_09, 3, 2)
                                                     END AS [Out 9] 
                                                    ,CASE Ttm_ActIn_10
                                                        WHEN '0000' THEN Null
                                                        ELSE SUBSTRING(Ttm_ActIn_10, 1, 2) + ':' + SUBSTRING(Ttm_ActIn_10, 3, 2)
                                                     END AS [In 10]
                                                     ,CASE Ttm_ActOut_10
                                                        WHEN '0000' THEN Null
                                                        ELSE SUBSTRING(Ttm_ActOut_10, 1, 2) + ':' + SUBSTRING(Ttm_ActOut_10, 3, 2)
                                                     END AS [Out 10] 
                                                    ,CASE Ttm_ActIn_11
                                                        WHEN '0000' THEN Null
                                                        ELSE SUBSTRING(Ttm_ActIn_11, 1, 2) + ':' + SUBSTRING(Ttm_ActIn_11, 3, 2)
                                                     END AS [In 11]
                                                     ,CASE Ttm_ActOut_11
                                                        WHEN '0000' THEN Null
                                                        ELSE SUBSTRING(Ttm_ActOut_11, 1, 2) + ':' + SUBSTRING(Ttm_ActOut_11, 3, 2)
                                                     END AS [Out 11] 
                                                    ,CASE Ttm_ActIn_12
                                                        WHEN '0000' THEN Null
                                                        ELSE SUBSTRING(Ttm_ActIn_12, 1, 2) + ':' + SUBSTRING(Ttm_ActIn_12, 3, 2)
                                                     END AS [In 12]
                                                     ,CASE Ttm_ActOut_12
                                                        WHEN '0000' THEN Null
                                                        ELSE SUBSTRING(Ttm_ActOut_12, 1, 2) + ':' + SUBSTRING(Ttm_ActOut_12, 3, 2)
                                                     END AS [Out 12] 
                                                    ,CASE Ttm_AssumedFlag 
                                                        WHEN 'TRUE'
			                                            THEN 'YES'
			                                            ELSE 'NO'
                                                    END AS [Tag as Present]
                                                    ,SKIPSERVICE.Mcd_Name AS [Skip Service]
                                                    ,AMNESTY.Mcd_Name AS [Amnesty]
                                                    ,Ttm_DocumentBatchNo AS [Doc No Ref]
                                                    ,Ttm_Remarks AS [Remarks]
                                                    ,ISNULL({0}.dbo.Udf_DisplayName(EmpLogM.Usr_Login,@NAMEDSPLY),EmpLogM.Usr_Login) AS [Modified by]
                                                    ,EmpLogM.Ludatetime AS [Last modified date]
                                               FROM T_EmpTimeRegisterLogMisc AS EmpLogM
                                               LEFT JOIN {0}..M_CodeDtl AMNESTY
	                                                ON AMNESTY.Mcd_Code = Ttm_Amnesty
                                                    AND AMNESTY.Mcd_CompanyCode = @CompanyCode
                                                    AND AMNESTY.Mcd_CodeType = 'AMNESTY'
                                               LEFT JOIN {0}..M_CodeDtl SKIPSERVICE
	                                                ON SKIPSERVICE.Mcd_Code = Ttm_SkipService
                                                    AND SKIPSERVICE.Mcd_CompanyCode = @CompanyCode
                                                    AND SKIPSERVICE.Mcd_CodeType = 'SKIPSERVICE'
                                               LEFT JOIN {0}..M_Shift ON Msh_ShiftCode = Ttm_ShiftCode
											        AND Msh_CompanyCode = @CompanyCode
                                               WHERE Ttm_IDNo = @IDNo
                                               AND Ttm_Date = @Date
                                               ORDER BY EmpLogM.Ludatetime DESC"
                                                , CentralProfile);
            #endregion

            int idxx = 0;
            ParameterInfo[] paramInfo = new ParameterInfo[4];
            paramInfo[idxx++] = new ParameterInfo("@IDNo", EmployeeID);
            paramInfo[idxx++] = new ParameterInfo("@Date", processDate, SqlDbType.Date);
            paramInfo[idxx++] = new ParameterInfo("@CompanyCode", CompanyCode);
            paramInfo[idxx++] = new ParameterInfo("@NAMEDSPLY", NAMEDSPLY);

            dt = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo).Tables[0];
            return dt;
        }

        public DataTable GetLogTrailMaxSeq(string EmployeeID, string processDate, string CentralProfile, string CompanyCode, string NAMEDSPLY)
        {
            DataTable dt = new DataTable();

            #region query
            string sqlQuery = string.Format(@"SELECT Ttl_LineNo as [Seq]
                                                     ,Ttl_DayCode as [Day Code]
                                                     ,Ttl_ShiftCode + CASE WHEN ISNULL(Msh_RequiredLogsOnBreak,0) = 1 THEN  + '*' ELSE '' END as [Shift Code]
                                                     ,CASE Ttl_RestDayFlag
			                                            WHEN 'TRUE'
			                                            THEN 'YES'
			                                            ELSE 'NO'
		                                             END as [Is Restday]
													 , CASE Ttl_HolidayFlag
			                                            WHEN 'TRUE'
			                                            THEN 'YES'
			                                            ELSE 'NO'
		                                             END as [Is Holiday]
                                                     ,CASE Ttl_ActIn_1
                                                        WHEN '0000' THEN Null
                                                        ELSE SUBSTRING(Ttl_ActIn_1, 1, 2) + ':' + SUBSTRING(Ttl_ActIn_1, 3, 2)
                                                     END AS [In 1]
                                                     ,CASE Ttl_ActOut_1
                                                        WHEN '0000' THEN Null
                                                        ELSE SUBSTRING(Ttl_ActOut_1, 1, 2) + ':' + SUBSTRING(Ttl_ActOut_1, 3, 2)
                                                     END AS [Out 1]
                                                     ,CASE Ttl_ActIn_2
                                                        WHEN '0000' THEN Null
                                                        ELSE SUBSTRING(Ttl_ActOut_1, 1, 2) + ':' + SUBSTRING(Ttl_ActOut_1, 3, 2)
                                                     END AS [In 2]
                                                     ,CASE Ttl_ActOut_2
                                                        WHEN '0000' THEN Null
                                                        ELSE SUBSTRING(Ttl_ActOut_2, 1, 2) + ':' + SUBSTRING(Ttl_ActOut_2, 3, 2)
                                                     END AS [Out 2] 
                                                    ,Ttl_Remarks AS [Remarks]
                                                    ,ISNULL({0}.dbo.Udf_DisplayName(EmpLogT.Usr_Login,@NAMEDSPLY),EmpLogT.Usr_Login) AS [Modified by]
                                                    ,EmpLogT.Ludatetime AS [Last modified date]

                                               FROM T_EmpTimeRegisterLog AS EmpLogT
                                               LEFT JOIN {0}..M_Shift ON Msh_ShiftCode = Ttl_ShiftCode
											        AND Msh_CompanyCode = @CompanyCode
                                               WHERE Ttl_IDNo = @IDNo
                                                    AND CONVERT(DATE,Ttl_Date) = @Date
                                                    AND Ttl_LineNo = (SELECT (CONVERT(INT,MAX(Ttl_LineNo)) - 1) 
                                                                      FROM T_EmpTimeRegisterLog 
																      WHERE Ttl_IDNo = @IDNo
                                                                        AND CONVERT(DATE,Ttl_Date) =  @Date)
                                               ORDER BY EmpLogT.Ludatetime DESC", CentralProfile);

            #endregion
            int idxx = 0;
            ParameterInfo[] paramInfo = new ParameterInfo[4];
            paramInfo[idxx++] = new ParameterInfo("@IDNo", EmployeeID);
            paramInfo[idxx++] = new ParameterInfo("@Date", processDate, SqlDbType.Date);
            paramInfo[idxx++] = new ParameterInfo("@CompanyCode", CompanyCode);
            paramInfo[idxx++] = new ParameterInfo("@NAMEDSPLY", NAMEDSPLY);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dt = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo).Tables[0];
                dal.CloseDB();
            }
            return dt;
        }

        public DataSet GetLastUpdateByAndDate(string userLogin, string companyCode, string CentralProfile)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@userLogin", userLogin);
            paramInfo[1] = new ParameterInfo("@NAMEDSPLY", (new CommonBL()).GetParameterValueFromCentral("NAMEDSPLY", companyCode, CentralProfile));

            string sqlQuery = @"SELECT ISNULL(dbo.Udf_DisplayName(Mem_UpdatedBy,@NAMEDSPLY),Mem_UpdatedBy) AS UserLogin
	                                   ,CONVERT(CHAR(10), Mem_UpdatedDate, 101) + ' ' + SUBSTRING(CONVERT(CHAR(10), Mem_UpdatedDate, 108), 1, 5) AS LastUpdate
                                FROM M_Employee
                                WHERE Mem_IDNo = @userLogin";

            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);
                dal.CloseDB();
            }
            return ds;
        }

        public DataSet GetCreatedByAndDate(string userLogin, string companyCode, string CentralProfile)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@userLogin", userLogin);
            paramInfo[1] = new ParameterInfo("@NAMEDSPLY", (new CommonBL()).GetParameterValueFromCentral("NAMEDSPLY", companyCode, CentralProfile));

            string sqlQuery = @"SELECT ISNULL(dbo.Udf_DisplayName(Mem_CreatedBy,@NAMEDSPLY),Mem_CreatedBy) AS UserLogin
	                                   ,CONVERT(CHAR(10), Mem_CreatedDate, 101) + ' ' + SUBSTRING(CONVERT(CHAR(10), Mem_CreatedDate, 108), 1, 5) AS CreatedDate
                                              FROM M_Employee
                                              WHERE Mem_IDNo = @userLogin";

            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            return ds;
        }

        public bool IfShiftCodeExists(string ShiftCode, string companyCode, string centralProfile)
        {
            string query = string.Format(@"SELECT Msh_ShiftCode FROM M_Shift 
                                                WHERE Msh_ShiftCode = @ShiftCode
                                                    AND Msh_CompanyCode = @CompanyCode");

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@ShiftCode", ShiftCode);
            paramInfo[1] = new ParameterInfo("@CompanyCode", companyCode);

            DataTable dtResult = new DataTable();
            using (DALHelper dal = new DALHelper(centralProfile, false))
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query, CommandType.Text, paramInfo).Tables[0];
                dal.CloseDB();
            }

            return (dtResult.Rows.Count > 0) ? true : false;
        }

        public DataTable GetEmployeeShift(bool IsNonRegularDay, string DayCode, string ShiftCode, string CompanyCode, string CentralProfile)
        {
            string query = string.Empty;
            string condition = "";
            if (IsNonRegularDay)
                condition = "AND Msh_ShiftHours = 8";

            #region query
            query = string.Format(@"SELECT Msh_ShiftCode As 'Shift Code'
                                           ,Msh_ShiftName As 'Description'
                                           ,Msh_ShiftIn1 As 'Time In'
                                           ,Msh_ShiftOut1 As 'Break Start'
                                           ,Msh_ShiftIn2 As 'Break End'
                                           ,Msh_ShiftOut2 As 'Time Out'
                                           ,Msh_ShiftHours As 'Shift Hours'
                                           ,Msh_PaidBreak As 'Paid Break'
                                     FROM M_Shift
                                     WHERE Msh_RecordStatus = 'A' 
                                            AND Msh_CompanyCode = @CompanyCode
                                     {0}
                                     and Msh_ShiftCode = @ShiftCode", condition);


            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@ShiftCode", ShiftCode);
            paramInfo[1] = new ParameterInfo("@CompanyCode", CompanyCode);

            DataTable dtResult;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query, CommandType.Text, paramInfo).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetEmployeeDetails(string EmployeeID, string PayCycle, string CompanyCode, string CentralProfile, string TableName, string CCTRDSPLY)
        {
            #region query

            string query = string.Format(@"SELECT Mem_IDNo as [ID Number]
                                , Mem_LastName + CASE WHEN LEN(ISNULL(Mem_ExtensionName,'')) > 0 
								     THEN  ' ' + Mem_ExtensionName ELSE '' END + ', ' + Mem_Firstname + ' ' + Mem_MiddleName as [Name]
                                ----, Mem_LastName + CASE WHEN LEN(ISNULL(Mem_ExtensionName,'')) > 0 
								----								THEN  ' ' + Mem_ExtensionName ELSE '' END as Mem_LastName
                                ----, Mem_FirstName as Mem_FirstName
								----, Mem_MiddleName as Mem_MiddleName
                                , PAYGRP.Mcd_Name as [Payroll Group]
                                , EmploymentStatus.Mcd_Name as [Employment Status]
								, PayrollType.Mcd_Name as [Payroll Type]
                                , Position.Mcd_Name as Position
								, Mem_CostcenterCode as [Cost Center Code]
                                , {0}.dbo.Udf_DisplayCostCenterName(@CompanyCode,Mem_CostcenterCode,@CCTRDSPLY) as [Cost Center Name]
                                , CONVERT(CHAR(10), Mem_IntakeDate, 101) as [Hire Date]
	                            , CONVERT(CHAR(10), Mem_RegularDate, 101) as [Regular Date]
	                            , CONVERT(CHAR(10), Mem_SeparationDate, 101) as [Separation Date]
				FROM {1}				
				INNER JOIN M_Employee on Ttr_IDNo = Mem_IDNo
				LEFT OUTER JOIN {0}..M_CodeDtl as EmploymentStatus
                     ON EmploymentStatus.Mcd_Code = Mem_EmploymentStatusCode AND
                     EmploymentStatus.Mcd_CodeType = 'EMPSTAT' AND 
                     EmploymentStatus.Mcd_CompanyCode = @CompanyCode AND
                     EmploymentStatus.Mcd_RecordStatus = 'A'
               LEFT OUTER JOIN {0}..M_CodeDtl AS Position ON Position.Mcd_Code = Mem_PayrollType AND
                     Position.Mcd_CodeType = 'POSITION' AND
                     Position.Mcd_CompanyCode = @CompanyCode AND
                     Position.Mcd_RecordStatus = 'A'
               LEFT OUTER JOIN {0}..M_CodeDtl AS PayrollType ON PayrollType.Mcd_Code = Mem_PayrollType AND
                     PayrollType.Mcd_CodeType = 'PAYTYPE' AND
                     PayrollType.Mcd_CompanyCode = @CompanyCode AND
                     PayrollType.Mcd_RecordStatus = 'A'
               LEFT OUTER JOIN {0}..M_CodeDtl AS PAYGRP ON PAYGRP.Mcd_Code = Mem_PayrollGroup AND
                     PAYGRP.Mcd_CodeType = 'PAYGRP' AND 
                     PAYGRP.Mcd_CompanyCode = @CompanyCode AND
                     PAYGRP.Mcd_RecordStatus = 'A'
			    WHERE Ttr_IDNo = @IDNo AND Ttr_PayCycle = @PayCycleCode"
                , CentralProfile
                , TableName);

            #endregion
            int idxx = 0;
            ParameterInfo[] paramInfo = new ParameterInfo[4];
            paramInfo[idxx++] = new ParameterInfo("@IDNo", EmployeeID);
            paramInfo[idxx++] = new ParameterInfo("@CompanyCode", CompanyCode);
            paramInfo[idxx++] = new ParameterInfo("@PayCycleCode", PayCycle);
            paramInfo[idxx++] = new ParameterInfo("@CCTRDSPLY", CCTRDSPLY);

            DataTable dt = new DataTable();
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dt = dal.ExecuteDataSet(query, CommandType.Text, paramInfo).Tables[0];
                dal.CloseDB();
            }
            return dt;
        }

        public DataSet SetDayTypeToolTip(string HolidayDate, string CompanyCode, string CentralProfile)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@HolidayDate", HolidayDate, SqlDbType.Date);
            paramInfo[1] = new ParameterInfo("@CompanyCode", CompanyCode);

            string query = @"SELECT Thl_HolidayName
                             FROM T_Holiday
                             WHERE Thl_HolidayDate = @HolidayDate
                             AND Thl_CompanyCode = @CompanyCode";

            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(query, CommandType.Text, paramInfo);
                dal.CloseDB();
            }
            return ds;
        }

        public DataSet CheckIfRowExistsInLogLedger(DataRow row, string tableName, DALHelper dal)
        {
            DataSet ds = new DataSet();
            string tmp = row["Ttr_ShiftCode"].ToString();
            string tmp1 = row["Ttr_ActIn_1"].ToString();
            string tmp2 = row["Ttr_ActOut_1"].ToString();
            string tmp3 = row["Ttr_ActIn_2"].ToString();
            string tmp4 = row["Ttr_ActOut_2"].ToString();
            string tmp5 = row["Ttr_IDNo"].ToString();
            string tmp6 = Convert.ToDateTime(row["Ttr_Date"]).ToString("MM/dd/yyyy").Substring(0, 5);

            string query = string.Format(@"  SELECT Ttr_IDNo
                                             FROM {7}
                                             WHERE Ttr_IDNo = '{0}'
                                                 AND SUBSTRING(CONVERT(CHAR(10),Ttr_Date,101),0,6) = '{1}'
                                                 AND Ttr_ShiftCode = '{2}' 
                                                 AND Ttr_ActIn_1 = '{3}'
                                                 AND Ttr_ActOut_1 = '{4}'
                                                 AND Ttr_ActIn_2 = '{5}'
                                                 AND Ttr_ActOut_2 = '{6}'"
                                            , tmp5
                                            , tmp6
                                            , tmp
                                            , tmp1
                                            , tmp2
                                            , tmp3
                                            , tmp4
                                            , tableName);

            ds = dal.ExecuteDataSet(query);
            return ds;
        }

        public DataSet GetRegularLogMiscRecord(string EmployeeID, string PayCycle, string CycleIndicator, string CentralProfile, string CompanyCode, bool bRegular)
        {
            string tableName = "";
            string tableNameMisc = "";

            if (CycleIndicator != "P")
            {
                tableName = CommonConstants.TableName.T_EmpTimeRegister;
                tableNameMisc = CommonConstants.TableName.T_EmpTimeRegisterMisc;
            }
            else
            {
                tableName = CommonConstants.TableName.T_EmpTimeRegisterHst;
                tableNameMisc = CommonConstants.TableName.T_EmpTimeRegisterMiscHst;
            }

            DataSet ds = new DataSet();
            string sql = string.Format(@"SELECT  Ttm_IDNo AS [ID Number], Ttm_Date AS [Date], temp.ID
	                                        , MAX(CASE WHEN Dtl.ID = 1 THEN Dtl.Data ELSE '' END) as [Start Time]
	                                        , MAX(CASE WHEN Dtl.ID = 2 THEN Dtl.Data ELSE '' END) as [End Time]
	                                        , CAST(MAX(CASE WHEN Dtl.ID = 3 THEN Dtl.Data ELSE '' END) AS DECIMAL(11,6)) as [No. of Hours]
	                                        , MAX(CASE WHEN Dtl.ID = 4 THEN TIMEREGTYPE.Mcd_Name ELSE '' END) as [Type]
	                                        , MAX(CASE WHEN Dtl.ID = 5 THEN TIMEREGCODE.Mcd_Name ELSE '' END)  
	                                        + MAX(CASE WHEN Dtl.ID = 6 THEN  CASE WHEN LEN(Dtl.Data) > 0 THEN ': ' +  Dtl.Data ELSE '' END
		                                        ELSE '' END) as [Remarks]
                                        FROM (SELECT Ttm_IDNo, Ttm_Date, cs.*
		                                        FROM {0}
		                                        CROSS APPLY {4}.dbo.Udf_Split(Ttm_Result, ',') cs
		                                        WHERE  Ttm_IDNo = '{2}'
			                                        AND LEN(Ttm_Result) > 0
			                                        AND Ttm_PayCycle = '{3}'
	                                         ) temp
                                        CROSS APPLY {4}.dbo.Udf_Split(Data, '|') dtl
                                        LEFT JOIN {4}..M_CodeDtl TIMEREGTYPE 
	                                        ON TIMEREGTYPE.Mcd_Code = dtl.Data
	                                        AND dtl.ID = 4
	                                        AND TIMEREGTYPE.Mcd_CodeType='TIMEREGTYPE'
	                                        AND TIMEREGTYPE.Mcd_CompanyCode = '{5}'
                                        LEFT JOIN {4}..M_CodeDtl TIMEREGCODE 
	                                        ON TIMEREGCODE.Mcd_Code = dtl.Data
	                                        AND dtl.ID = 5
	                                        AND TIMEREGCODE.Mcd_CodeType='TIMEREGCODE'
	                                        AND TIMEREGCODE.Mcd_CompanyCode = '{5}'
                                        GROUP BY Ttm_IDNo, Ttm_Date,  temp.ID
                                        HAVING CAST(MAX(CASE WHEN Dtl.ID = 3 THEN Dtl.Data ELSE '' END) AS DECIMAL(11,6)) > 0.00
                                            {6}
                                        ORDER BY Ttm_IDNo, Ttm_Date,  temp.ID
                                        ", tableNameMisc
                                            , tableName
                                            , EmployeeID
                                            , PayCycle
                                            , CentralProfile
                                            , CompanyCode
                                            , (bRegular ? "AND MAX(CASE WHEN Dtl.ID = 4 THEN TIMEREGTYPE.Mcd_Name ELSE '' END) <> 'NIGHT PREMIUM'" : "AND MAX(CASE WHEN Dtl.ID = 4 THEN TIMEREGTYPE.Mcd_Name ELSE '' END) = 'NIGHT PREMIUM'"));


            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sql, CommandType.Text);

                dal.CloseDB();
            }
            return ds;
        }

        #endregion

        #region for Other Information
        public DataTable GetOtherInformation(string ID, string Date, string PayPeriod, string CycleIndicator, string CompanyCode, string CentralProfile, DALHelper dal)
        {
            string table = "T_EmpTimeRegister";
            DataTable dt = new DataTable();

            if (CycleIndicator == "P")
                table = "T_EmpTimeRegisterHst";

            #region query
            string qString = string.Format(@"
                                            SELECT  
													CASE Ttr_RestDayFlag
															WHEN 1 THEN Ttr_DayCode	+ '*' + ' - ' + Mdy_DayName
													ELSE Ttr_DayCode + ' - ' + Mdy_DayName
													END  Ttr_DayCode
													, SUBSTRING(DATENAME(weekday, Ttr_Date), 1, 3) AS DOW
													, CONVERT(CHAR(10),Ttr_Date,101) as [ProcessDate]
													, Ttr_ShiftCode + CASE WHEN ISNULL(Msh_RequiredLogsOnBreak,0) = 1 THEN  + '*' ELSE '' END
                                                              + ' (' + ISNULL(LEFT(Msh_ShiftIn1,2) + ':' + Right(Msh_ShiftIn1,2) + ' - ' +
															  LEFT(Msh_ShiftOut1,2) + ':' + Right(Msh_ShiftOut1,2) + '  ' +
															  LEFT(Msh_ShiftIn2,2) + ':' + Right(Msh_ShiftIn2,2) + ' - ' + 
															  LEFT(Msh_ShiftOut2,2) + ':' + Right(Msh_ShiftOut2,2) + 
															  ', ' + CAST(Msh_ShiftHours AS varchar) + ' Hours' ,'')
                                                              + ')' AS Ttr_ShiftCode
                                                    , CASE WHEN Msh_ReducedHours != 0 THEN ISNULL(LEFT(Msh_ShiftIn1,2) + ':' + Right(Msh_ShiftIn1,2) + ' - ' +
															  LEFT(Msh_ReducedTimeOut2,2) + ':' + Right(Msh_ReducedTimeOut2,2) +
															  ', ' + CAST(Msh_ReducedHours AS varchar) + ' Hours' ,'')
                                                      ELSE '' END AS ReducedShiftCode

		                                            , CASE Ttr_RestDayFlag
			                                            WHEN 'TRUE'
			                                            THEN 'YES'
			                                            ELSE 'NO'
		                                             END AS Ttr_RestDayFlag
													 , CASE Ttr_HolidayFlag
			                                            WHEN 'TRUE'
			                                            THEN 'YES'
			                                            ELSE 'NO'
		                                             END AS Ttr_HolidayFlag
													, CASE WHEN Ttr_PDRESTLEGHOLDay > 0
			                                            THEN 'YES'
			                                            ELSE 'NO'
		                                             END AS Ttr_PDRESTLEGHOLDay
                                                    , CASE WHEN Ttr_WorkDay > 0
			                                            THEN 'YES'
			                                            ELSE 'NO'
		                                             END AS Ttr_WorkDay
													,CONVERT(CHAR(10), Ttr_PrvDayHolRef, 101) AS Ttr_PrvDayHolRef
													,Ttr_PrvDayWorkMin / 60.00 AS Ttr_PrvDayWorkMin
                                                    ,AMNESTY.Mcd_Name AS Amnesty
                                                    ,Ttr_CalendarGroup
                                                    ,Ttr_WorkLocationCode
                                                    ,ZIPCODE.Mcd_Name as [Ttr_WorkLocation]
                                                    ,Msh_Schedule
                                                    ,CASE Ttr_AssumedPost
                                                        WHEN '' THEN 'NO'
                                                        WHEN 'T' THEN 'YES'
                                                        WHEN 'A' THEN 'YES'
                                                        ELSE 'NO'
                                                    END AS AssumedDay
                                                    ,Msh_PaidBreak
                                                    ,CASE Msh_NDCount
			                                            WHEN 'TRUE'
			                                            THEN 'YES'
			                                            ELSE 'NO'
		                                             END AS Msh_NDCount
                                                   ,Msh_HourFractionCutoff
                                                   , Ttr_TBAmt01
                                                   , Ttr_TBAmt02
                                                   , Ttr_TBAmt03
                                                   , Ttr_TBAmt04
                                                   , Ttr_TBAmt05
                                                   , Ttr_TBAmt06
                                                   , Ttr_TBAmt07
                                                   , Ttr_TBAmt08
                                                   , Ttr_TBAmt09
                                                   , Ttr_TBAmt10
                                                   , Ttr_TBAmt11
                                                   , Ttr_TBAmt12
                                            FROM {3} 
                                            LEFT JOIN {4}..M_Shift ON Msh_ShiftCode = Ttr_ShiftCode
                                                AND Msh_CompanyCode = '{5}'
											LEFT JOIN {4}..M_Day ON Mdy_DayCode = Ttr_DayCode
                                                AND Mdy_CompanyCode = '{5}'
                                            LEFT JOIN {4}..M_CodeDtl AMNESTY ON AMNESTY.Mcd_Code = Ttr_Amnesty
                                                AND AMNESTY.Mcd_CodeType = 'AMNESTY'
                                                AND AMNESTY.Mcd_CompanyCode = '{5}'
                                            LEFT JOIN {4}..M_CodeDtl ZIPCODE ON ZIPCODE.Mcd_Code = Ttr_WorkLocationCode
                                                AND ZIPCODE.Mcd_CodeType = 'ZIPCODE'
                                                AND ZIPCODE.Mcd_CompanyCode = '{5}'
                                            WHERE Ttr_IDNo = '{0}'
                                                AND Ttr_Date = '{1}' 
                                                AND Ttr_PayCycle = '{2}'"
                                            , ID, Date, PayPeriod, table
                                            , CentralProfile
                                            , CompanyCode);

            #endregion

            dt = dal.ExecuteDataSet(qString).Tables[0];
            return dt;
        }

        public DataTable GetOvertime(string IDNumber, string Date, string PayPeriod, string CompanyCode, string CentralProfile, string NAMEDSPLY, DALHelper dal)
        { 
            #region query
            string qString = string.Format(@"
                                            SELECT 
                                                 LEFT(Tot_StartTime, 2) + ':' + RIGHT(Tot_StartTime, 2) as [Start Time]
                                                , LEFT(Tot_EndTime, 2) + ':' + RIGHT(Tot_EndTime, 2) as [End Time]
                                                , Tot_OvertimeHours as [Hour(s)]
                                                , Mpd_SubName as [Overtime Type]
                                                , Tot_ReasonForRequest as [Reason for Request]
                                                , DOCUGSTAT.Mcd_Name as [Status]
                                                , CYCLEINDIC.Mcd_Name as [Cycle Flag]
                                                , ISNULL({0}.dbo.Udf_DisplayName(Tot_SubmittedBy,@NAMEDSPLY),Tot_SubmittedBy) as [Submitted By]
                                                , Tot_SubmittedDate as [Submitted Date]
                                                , ISNULL({0}.dbo.Udf_DisplayName(Tot_Authority1,@NAMEDSPLY),Tot_Authority1) AS [Authority 1]
                                                , Tot_Authority1Date AS [Authority 1 Date]
                                                , ISNULL({0}.dbo.Udf_DisplayName(Tot_Authority2,@NAMEDSPLY),Tot_Authority2) AS [Authority 2]
                                                , Tot_Authority2Date AS [Authority 2 Date]
                                                , ISNULL({0}.dbo.Udf_DisplayName(Tot_Authority3,@NAMEDSPLY),Tot_Authority3) AS [Authority 3]
                                                , Tot_Authority3Date AS [Authority 3 Date]
                                                , ISNULL({0}.dbo.Udf_DisplayName(Tot_Authority4,@NAMEDSPLY),Tot_Authority4) AS [Authority 4]
                                                , Tot_Authority4Date AS [Authority 4 Date]
                                                , ISNULL({0}.dbo.Udf_DisplayName(Tot_Authority5,@NAMEDSPLY),Tot_Authority5) AS [Authority 5]
                                                , Tot_Authority5Date AS [Authority 5 Date]
                                                FROM (SELECT * FROM T_EmpOvertime 
											            UNION ALL
											            SELECT * FROM T_EmpOvertimeHst 
										             ) T_EmpOvertime  
                                                LEFT JOIN M_PolicyDtl ON Mpd_SubCode = Tot_OvertimeType
                                                    AND Mpd_CompanyCode = @CompanyCode
                                                    AND Mpd_PolicyCode = 'OTTYPE'
                                                LEFT JOIN {0}..M_CodeDtl DOCUGSTAT ON DOCUGSTAT.Mcd_Code = Tot_OvertimeStatus
                                                    AND DOCUGSTAT.Mcd_CompanyCode = @CompanyCode
                                                    AND DOCUGSTAT.Mcd_CodeType = 'DOCUGSTAT'
                                                LEFT JOIN {0}..M_CodeDtl CYCLEINDIC ON CYCLEINDIC.Mcd_Code = Tot_OvertimeFlag
                                                    AND CYCLEINDIC.Mcd_CompanyCode = @CompanyCode
                                                    AND CYCLEINDIC.Mcd_CodeType = 'CYCLEINDIC'
                                                WHERE Tot_IDNo = @IDNo
                                                    AND Tot_OvertimeStatus <> '03'
                                                    AND CONVERT(DATE,Tot_OvertimeDate) = @Date
                                                ORDER BY Tot_StartTime, Tot_EndTime, Tot_OvertimeStatus
                                                ", CentralProfile);

            #endregion
            int idxx = 0;
            ParameterInfo[] paramInfo = new ParameterInfo[4];
            paramInfo[idxx++] = new ParameterInfo("@IDNo", IDNumber);
            paramInfo[idxx++] = new ParameterInfo("@Date", Date, SqlDbType.Date);
            paramInfo[idxx++] = new ParameterInfo("@CompanyCode", CompanyCode);
            paramInfo[idxx++] = new ParameterInfo("@NAMEDSPLY", NAMEDSPLY);

            DataTable dt = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo).Tables[0];
            return dt;
        }

        public DataTable GetLeave(string IDNumber, string Date, string PayPeriod, string CompanyCode, string CentralProfile, string NAMEDSPLY, DALHelper dal)
        {
            #region query
            string qString = string.Format(@"
                                                SELECT 
                                                     LEFT(Tlv_StartTime, 2) + ':' + RIGHT(Tlv_StartTime, 2) as [Start Time]
                                                    , LEFT(Tlv_EndTime, 2) + ':' + RIGHT(Tlv_EndTime, 2) as [End Time]
                                                    , Tlv_LeaveHours as [Hour(s)]
                                                    , LVTYPE.Mlv_LeaveDescription  as [Leave Type]
                                                    , Tlv_ReasonForRequest as [Reason for Request]
                                                    , DOCUGSTAT.Mcd_Name as [Status]
                                                    , CYCLEINDIC.Mcd_Name as [Cycle Flag]
                                                    , ISNULL({0}.dbo.Udf_DisplayName(Tlv_SubmittedBy,@NAMEDSPLY),Tlv_SubmittedBy) as [Submitted By]
                                                    , Tlv_SubmittedDate as [Submitted Date]
                                                    , ISNULL({0}.dbo.Udf_DisplayName(Tlv_Authority1,@NAMEDSPLY),Tlv_Authority1) as [Authority 1]
                                                    , Tlv_Authority1Date as [Authority 1 Date]
                                                    , ISNULL({0}.dbo.Udf_DisplayName(Tlv_Authority2,@NAMEDSPLY),Tlv_Authority2) as [Authority 2]
                                                    , Tlv_Authority2Date as [Authority 2 Date]
                                                    , ISNULL({0}.dbo.Udf_DisplayName(Tlv_Authority3,@NAMEDSPLY),Tlv_Authority3) as [Authority 3]
                                                    , Tlv_Authority3Date as [Authority 3 Date]
                                                    , ISNULL({0}.dbo.Udf_DisplayName(Tlv_Authority4,@NAMEDSPLY),Tlv_Authority4) as [Authority 4]
                                                    , Tlv_Authority4Date as [Authority 4 Date]
                                                    , ISNULL({0}.dbo.Udf_DisplayName(Tlv_Authority5,@NAMEDSPLY),Tlv_Authority5) as [Authority 5]
                                                    , Tlv_Authority5Date as [Authority 5 Date]
                                                FROM (SELECT * FROM T_EmpLeave 
											            UNION ALL
											            SELECT * FROM T_EmpLeaveHst 
										             ) T_EmpLeave 
                                                LEFT JOIN {0}..M_Leave LVTYPE ON LVTYPE.Mlv_CompanyCode = @CompanyCode
                                                    AND LVTYPE.Mlv_LeaveCode = Tlv_LeaveCode
                                                LEFT JOIN {0}..M_CodeDtl DOCUGSTAT ON DOCUGSTAT.Mcd_Code = Tlv_LeaveStatus
                                                    AND DOCUGSTAT.Mcd_CompanyCode = @CompanyCode
                                                    AND DOCUGSTAT.Mcd_CodeType = 'DOCUGSTAT'
                                                LEFT JOIN {0}..M_CodeDtl CYCLEINDIC ON CYCLEINDIC.Mcd_Code = Tlv_LeaveFlag
                                                    AND CYCLEINDIC.Mcd_CompanyCode = @CompanyCode
                                                    AND CYCLEINDIC.Mcd_CodeType = 'CYCLEINDIC'
                                                WHERE Tlv_IDNo = @IDNo
                                                    AND Tlv_LeaveStatus <> '03'
                                                    AND CONVERT(DATE,Tlv_LeaveDate) = @Date
                                                ORDER BY Tlv_LeaveStatus
                                                ", CentralProfile);

            #endregion

            int idxx = 0;
            ParameterInfo[] paramInfo = new ParameterInfo[4];
            paramInfo[idxx++] = new ParameterInfo("@IDNo", IDNumber);
            paramInfo[idxx++] = new ParameterInfo("@Date", Date, SqlDbType.Date);
            paramInfo[idxx++] = new ParameterInfo("@CompanyCode", CompanyCode);
            paramInfo[idxx++] = new ParameterInfo("@NAMEDSPLY", NAMEDSPLY);

            DataTable dt = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo).Tables[0];
            return dt;
        }

        public DataTable GetTimeCorrection(string IDNumber, string Date, string PayPeriod, string CompanyCode, string CentralProfile, int POCKETSIZE, string NAMEDSPLY, DALHelper dal)
        {
            #region query
            string qString = string.Format(@"
                                            SELECT {1}
                                                            CASE WHEN @POCKETSIZE > 2 THEN
			                                                STUFF((SELECT '   ' + CASE WHEN LogCtrlIn IN ('1','2') THEN  
			                                                LEFT(TIn, 2) + ':' + RIGHT(TIn, 2) 
				                                                  ELSE
					                                                '[' + LEFT(TIn, 2) + ':' + RIGHT(TIn, 2) + '] '
				                                                  END + '-' + 
				                                                  CASE WHEN LogCtrlOut IN ('1','2') THEN  
					                                                LEFT(TOut, 2) + ':' + RIGHT(TOut, 2) 
				                                                  ELSE
					                                                '[' + LEFT(TOut, 2) + ':' + RIGHT(TOut, 2) + '] '
				                                                  END
			                                                FROM (SELECT * FROM T_EmpTimeCorrection 
											                      UNION ALL
											                      SELECT * FROM T_EmpTimeCorrectionHst ) TC
			                                                CROSS APPLY ( VALUES ('01', Ttm_TimeIn01,Ttm_TimeOut01, LEFT(Ttm_LogControl,1),SUBSTRING(Ttm_LogControl,2,1)),
								                                                ('02', Ttm_TimeIn02,Ttm_TimeOut02,SUBSTRING(Ttm_LogControl,3,1),SUBSTRING(Ttm_LogControl,4,1)),
								                                                ('03', Ttm_TimeIn03,Ttm_TimeOut03,SUBSTRING(Ttm_LogControl,5,1),SUBSTRING(Ttm_LogControl,6,1)),
								                                                ('04', Ttm_TimeIn04,Ttm_TimeOut04,SUBSTRING(Ttm_LogControl,7,1),SUBSTRING(Ttm_LogControl,8,1)),
								                                                ('05', Ttm_TimeIn05,Ttm_TimeOut05,SUBSTRING(Ttm_LogControl,9,1),SUBSTRING(Ttm_LogControl,10,1)),
								                                                ('06', Ttm_TimeIn06,Ttm_TimeOut06,SUBSTRING(Ttm_LogControl,11,1),SUBSTRING(Ttm_LogControl,12,1)),
								                                                ('07', Ttm_TimeIn07,Ttm_TimeOut07,SUBSTRING(Ttm_LogControl,13,1),SUBSTRING(Ttm_LogControl,14,1)),
								                                                ('08', Ttm_TimeIn08,Ttm_TimeOut08,SUBSTRING(Ttm_LogControl,15,1),SUBSTRING(Ttm_LogControl,16,1)),
								                                                ('09', Ttm_TimeIn09,Ttm_TimeOut09,SUBSTRING(Ttm_LogControl,17,1),SUBSTRING(Ttm_LogControl,18,1)),
								                                                ('10', Ttm_TimeIn10,Ttm_TimeOut10,SUBSTRING(Ttm_LogControl,19,1),SUBSTRING(Ttm_LogControl,20,1)),
								                                                ('11', Ttm_TimeIn10,Ttm_TimeOut10,SUBSTRING(Ttm_LogControl,21,1),SUBSTRING(Ttm_LogControl,22,1)),
								                                                ('12', Ttm_TimeIn10,Ttm_TimeOut10,SUBSTRING(Ttm_LogControl,23,1),SUBSTRING(Ttm_LogControl,24,1))

								                                                ) temp (Seq, TIn, TOut,LogCtrlIn,LogCtrlOut)
			                                                WHERE TC.Ttm_IDNo = Availment.Ttm_IDNo
				                                                AND TC.Ttm_TimeCorDate = Availment.Ttm_TimeCorDate
				                                                AND (LogCtrlIn <> 'X' OR LogCtrlOut <> 'X')
		                                                    ORDER BY Seq
			                                                FOR XML PATH('')),1,1,'') 
			
		                                                ELSE
			                                                CASE WHEN Availment.Ttm_TimeIn01 + Availment.Ttm_TimeOut01 = '00000000' THEN
					                                                CASE WHEN Availment.Ttm_TimeCorType like '%I2%' THEN '[' + LEFT(Availment.Ttm_TimeIn02, 2) + ':' + RIGHT(Availment.Ttm_TimeIn02, 2) + '] ' 
   							                                                ELSE  LEFT(Availment.Ttm_TimeIn02, 2) + ':' + RIGHT(Availment.Ttm_TimeIn02, 2) END +
					                                                CASE WHEN Availment.Ttm_TimeCorType like '%O2%' THEN '  - [' + LEFT(Availment.Ttm_TimeOut02, 2) + ':' + RIGHT(Availment.Ttm_TimeOut02, 2) + '] ' 
   						                                                ELSE  '  - ' + LEFT(Availment.Ttm_TimeOut02, 2) + ':' + RIGHT(Availment.Ttm_TimeOut02, 2) END 

				                                                WHEN Availment.Ttm_TimeIn02 + Availment.Ttm_TimeOut02 = '00000000' THEN
					                                                CASE WHEN Availment.Ttm_TimeCorType like '%I1%' THEN '[' + LEFT(Availment.Ttm_TimeIn01, 2) + ':' + RIGHT(Availment.Ttm_TimeIn01, 2) + '] ' 
						                                                ELSE LEFT(Availment.Ttm_TimeIn01, 2) + ':' + RIGHT(Availment.Ttm_TimeIn01, 2) END +
					                                                CASE WHEN Availment.Ttm_TimeCorType like '%O1%' THEN '  - [' + LEFT(Availment.Ttm_TimeOut01, 2) + ':' + RIGHT(Availment.Ttm_TimeOut01, 2) + '] ' 
						                                                ELSE  '  - ' + LEFT(Availment.Ttm_TimeOut01, 2) + ':' + RIGHT(Availment.Ttm_TimeOut01, 2) END 
				                                                WHEN Availment.Ttm_TimeOut01 + Availment.Ttm_TimeIn02 = '00000000' THEN
					                                                CASE WHEN Availment.Ttm_TimeCorType like '%I1%' THEN '[' + LEFT(Availment.Ttm_TimeIn01, 2) + ':' + RIGHT(Availment.Ttm_TimeIn01, 2) + '] ' 
					                                                ELSE LEFT(Availment.Ttm_TimeIn01, 2) + ':' + RIGHT(Availment.Ttm_TimeIn01, 2) END +
						                                                CASE WHEN Availment.Ttm_TimeCorType like '%O2%' THEN '  - [' + LEFT(Availment.Ttm_TimeOut02, 2) + ':' + RIGHT(Availment.Ttm_TimeOut02, 2) + '] ' 
					                                                ELSE  '  - ' + LEFT(Availment.Ttm_TimeOut02, 2) + ':' + RIGHT(Availment.Ttm_TimeOut02, 2) END
				                                                ELSE
					                                                CASE WHEN Availment.Ttm_TimeCorType like '%I1%' THEN '[' + LEFT(Availment.Ttm_TimeIn01, 2) + ':' + RIGHT(Availment.Ttm_TimeIn01, 2) + ']' 
						                                                ELSE LEFT(Availment.Ttm_TimeIn01, 2) + ':' + RIGHT(Availment.Ttm_TimeIn01, 2) END +
					                                                CASE WHEN Availment.Ttm_TimeCorType like '%O1%' THEN '  - [' + LEFT(Availment.Ttm_TimeOut01, 2) + ':' + RIGHT(Availment.Ttm_TimeOut01, 2) + '] ' 
						                                                ELSE  '  - ' + LEFT(Availment.Ttm_TimeOut01, 2) + ':' + RIGHT(Availment.Ttm_TimeOut01, 2) END +  '  ' +
					                                                CASE WHEN Availment.Ttm_TimeCorType like '%I2%' THEN ' [' + LEFT(Availment.Ttm_TimeIn02, 2) + ':' + RIGHT(Availment.Ttm_TimeIn02, 2) + '] ' 
   							                                                ELSE '  ' + LEFT(Availment.Ttm_TimeIn02, 2) + ':' + RIGHT(Availment.Ttm_TimeIn02, 2) END +
					                                                CASE WHEN Availment.Ttm_TimeCorType like '%O2%' THEN ' - [' + LEFT(Availment.Ttm_TimeOut02, 2) + ':' + RIGHT(Availment.Ttm_TimeOut02, 2) + ']' 
   						                                                ELSE  ' - ' + LEFT(Availment.Ttm_TimeOut02, 2) + ':' + RIGHT(Availment.Ttm_TimeOut02, 2) END 
				                                                END 
		
		                                                END	AS [Time]
                                                        , Ttm_ReasonForRequest as [Reason for Request]
                                                        , DOCUGSTAT.Mcd_Name as [Status]
                                                        , CYCLEINDIC.Mcd_Name as [Cycle Flag]
                                                        , ISNULL({0}.dbo.Udf_DisplayName(Ttm_SubmittedBy,@NAMEDSPLY),Ttm_SubmittedBy) as [Submitted By]
                                                        , Ttm_SubmittedDate as [Submitted Date]
                                                        , ISNULL({0}.dbo.Udf_DisplayName(Ttm_Authority1,@NAMEDSPLY),Ttm_Authority1) as [Authority 1]
                                                        , Ttm_Authority1Date as [Authority 1 Date]
                                                        , ISNULL({0}.dbo.Udf_DisplayName(Ttm_Authority2,@NAMEDSPLY),Ttm_Authority2) as [Authority 2]
                                                        , Ttm_Authority2Date as [Authority 2 Date]
                                                        , ISNULL({0}.dbo.Udf_DisplayName(Ttm_Authority3,@NAMEDSPLY),Ttm_Authority3) as [Authority 3]
                                                        , Ttm_Authority3Date as [Authority 3 Date]
                                                        , ISNULL({0}.dbo.Udf_DisplayName(Ttm_Authority4,@NAMEDSPLY),Ttm_Authority4) as [Authority 4]
                                                        , Ttm_Authority4Date as [Authority 4 Date]
                                                        , ISNULL({0}.dbo.Udf_DisplayName(Ttm_Authority5,@NAMEDSPLY),Ttm_Authority5) as [Authority 5]
                                                        , Ttm_Authority5Date as [Authority 5 Date]
                                                    FROM (SELECT * FROM T_EmpTimeCorrection 
											                UNION ALL
											              SELECT * FROM T_EmpTimeCorrectionHst ) Availment
                                                    LEFT JOIN {0}..M_CodeDtl REQTYPE ON REQTYPE.Mcd_Code = Ttm_TimeCorType
                                                        AND REQTYPE.Mcd_CodeType = 'TMERECTYPE'
                                                        AND REQTYPE.Mcd_CompanyCode = @CompanyCode
                                                    LEFT JOIN {0}..M_CodeDtl DOCUGSTAT ON DOCUGSTAT.Mcd_Code = Ttm_TimeCorStatus
                                                        AND DOCUGSTAT.Mcd_CompanyCode = @CompanyCode
                                                        AND DOCUGSTAT.Mcd_CodeType = 'DOCUGSTAT'
                                                    LEFT JOIN {0}..M_CodeDtl CYCLEINDIC ON CYCLEINDIC.Mcd_Code = Ttm_TimeCorFlag
                                                        AND CYCLEINDIC.Mcd_CompanyCode = @CompanyCode
                                                        AND CYCLEINDIC.Mcd_CodeType = 'CYCLEINDIC'
                                                    WHERE Ttm_IDNo = @IDNo
                                                        AND Ttm_TimeCorStatus <> '03'
                                                        AND CONVERT(DATE,Ttm_TimeCorDate) = @Date
                                                    ORDER BY Ttm_TimeCorStatus
                                                    ", CentralProfile
                                                    , (Convert.ToInt32(POCKETSIZE) == 2 ? "REQTYPE.Mcd_Name  as [Type], " : ""));

            #endregion
            int idxx = 0;
            ParameterInfo[] paramInfo = new ParameterInfo[5];
            paramInfo[idxx++] = new ParameterInfo("@IDNo", IDNumber);
            paramInfo[idxx++] = new ParameterInfo("@Date", Date, SqlDbType.Date);
            paramInfo[idxx++] = new ParameterInfo("@CompanyCode", CompanyCode);
            paramInfo[idxx++] = new ParameterInfo("@NAMEDSPLY", NAMEDSPLY);
            paramInfo[idxx++] = new ParameterInfo("@POCKETSIZE", POCKETSIZE, SqlDbType.TinyInt);

            DataTable dt = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo).Tables[0];
            return dt;
        }

        public DataTable GetAllowanceHeader(string CompanyCode, string CentralProfile, DALHelper dal)
        {
            DataTable dt = new DataTable();
            string sql = string.Format(@"
                                SELECT Mvh_TimeBaseID
                                    , Mvh_AllowanceCode 
                                FROM {0}..M_VarianceAllowanceHdr
                                WHERE Mvh_CompanyCode = '{1}'
                                ORDER BY Mvh_TimeBaseID", CentralProfile, CompanyCode);
            dt = dal.ExecuteDataSet(sql, CommandType.Text).Tables[0];
            return dt;
        }

        public DataTable GetWorkAuthorization(string IDNumber, string Date, string PayPeriod, string CompanyCode, string CentralProfile, string NAMEDSPLY, DALHelper dal)
        {
            #region query
            string qString = string.Format(@"
                                            SELECT 
                                                 LEFT(Twa_StartTime, 2) + ':' + RIGHT(Twa_StartTime, 2) as [Start Time]
                                                , LEFT(Twa_EndTime, 2) + ':' + RIGHT(Twa_EndTime, 2) as [End Time]
                                                , Twa_WorkHours as [Hour(s)]
                                                , Twa_ReasonForRequest as [Reason for Request]
                                                , DOCUGSTAT.Mcd_Name as [Status]
                                                , CYCLEINDIC.Mcd_Name as [Cycle Flag]
                                                , ISNULL({0}.dbo.Udf_DisplayName(Twa_SubmittedBy,@NAMEDSPLY),Twa_SubmittedBy) as [Submitted By]
                                                , Twa_SubmittedDate as [Submitted Date]
                                                , ISNULL({0}.dbo.Udf_DisplayName(Twa_Authority1,@NAMEDSPLY),Twa_Authority1) AS [Authority 1]
                                                , Twa_Authority1Date AS [Authority 1 Date]
                                                , ISNULL({0}.dbo.Udf_DisplayName(Twa_Authority2,@NAMEDSPLY),Twa_Authority2) AS [Authority 2]
                                                , Twa_Authority2Date AS [Authority 2 Date]
                                                , ISNULL({0}.dbo.Udf_DisplayName(Twa_Authority3,@NAMEDSPLY),Twa_Authority3) AS [Authority 3]
                                                , Twa_Authority3Date AS [Authority 3 Date]
                                                , ISNULL({0}.dbo.Udf_DisplayName(Twa_Authority4,@NAMEDSPLY),Twa_Authority4) AS [Authority 4]
                                                , Twa_Authority4Date AS [Authority 4 Date]
                                                , ISNULL({0}.dbo.Udf_DisplayName(Twa_Authority5,@NAMEDSPLY),Twa_Authority5) AS [Authority 5]
                                                , Twa_Authority5Date AS [Authority 5 Date]
                                                FROM (SELECT * FROM T_EmpWorkAuthorization 
											            UNION ALL
											            SELECT * FROM T_EmpWorkAuthorizationHst 
										             ) T_EmpWorkAuthorization  
                                                LEFT JOIN {0}..M_CodeDtl DOCUGSTAT ON DOCUGSTAT.Mcd_Code = Twa_WorkAuthStatus
                                                    AND DOCUGSTAT.Mcd_CompanyCode = @CompanyCode
                                                    AND DOCUGSTAT.Mcd_CodeType = 'DOCUGSTAT'
                                                LEFT JOIN {0}..M_CodeDtl CYCLEINDIC ON CYCLEINDIC.Mcd_Code = Twa_WorkAuthFlag
                                                    AND CYCLEINDIC.Mcd_CompanyCode = @CompanyCode
                                                    AND CYCLEINDIC.Mcd_CodeType = 'CYCLEINDIC'
                                                WHERE Twa_IDNo = @IDNo
                                                    AND Twa_WorkAuthStatus <> '03'
                                                    AND CONVERT(DATE,Twa_WorkDate) = @Date
                                                ORDER BY Twa_StartTime, Twa_EndTime, Twa_WorkAuthStatus
                                                ", CentralProfile);

            #endregion
            int idxx = 0;
            ParameterInfo[] paramInfo = new ParameterInfo[4];
            paramInfo[idxx++] = new ParameterInfo("@IDNo", IDNumber);
            paramInfo[idxx++] = new ParameterInfo("@Date", Date, SqlDbType.Date);
            paramInfo[idxx++] = new ParameterInfo("@CompanyCode", CompanyCode);
            paramInfo[idxx++] = new ParameterInfo("@NAMEDSPLY", NAMEDSPLY);

            DataTable dt = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo).Tables[0];
            return dt;
        }

        #endregion

        #region for Assumed Present Methods
        public int AssumePresent(DataRow dr, string TableName, DALHelper dal)
        {
            int retVal = 0;

            #region Query
            string qString1 = string.Format(@"UPDATE {3}
                                                SET Ttr_AssumedFlag = 1
                                                    ,Ttr_AssumedBy = '{2}'
                                                    ,Ttr_AssumedDate = GETDATE()
                                                FROM {3}
                                                WHERE Ttr_IDNo = '{0}'
                                                    AND Ttr_Date = '{1}'
                                                    AND Ttr_DayCode = 'REG'"
                                                , dr["Ttr_IDNo"]
                                                , dr["Ttr_Date"]
                                                , dr["Usr_Login"]
                                                , TableName);

            string qString2 = string.Format(@"UPDATE {2}
                                                SET Ttr_AssumedFlag = 0
                                                    ,Ttr_AssumedBy = NULL
                                                    ,Ttr_AssumedDate = NULL
                                                FROM {2}
                                                WHERE Ttr_IDNo = '{0}'
                                                    AND Ttr_Date = '{1}'"
                                                , dr["Ttr_IDNo"]
                                                , dr["Ttr_Date"]
                                                , TableName);
            #endregion

            if (dr["Ttr_AssumedFlag"].ToString().ToUpper().Equals("TRUE"))
                retVal = dal.ExecuteNonQuery(qString1);
            else
                retVal = dal.ExecuteNonQuery(qString2);

            return retVal;
        }

        public int SkipService(DataRow dr, string TableName, DALHelper dal)
        {
            int retVal = 0;
            int paramIndex = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[4];
            paramInfo[paramIndex++] = new ParameterInfo("@Ttr_IDNo", dr["Ttr_IDNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttr_Date", dr["Ttr_Date"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttr_SkipService", dr["Ttr_SkipService"].ToString());
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", dr["Usr_Login"]);


            #region Query
            string sqlQuery = string.Format(@"UPDATE {0}
                                               SET Ttr_SkipService      = @Ttr_SkipService
                                                  ,Ttr_SkipServiceBy    = @Usr_Login
                                                  ,Ttr_SkipServiceDate  = GETDATE()
                                                  ,Usr_Login            = @Usr_Login
                                                  ,Ludatetime           = GETDATE()
                                              WHERE Ttr_IDNo = @Ttr_IDNo
                                                AND Ttr_Date = @Ttr_Date", TableName);
            #endregion
            retVal = dal.ExecuteNonQuery(sqlQuery, CommandType.Text, paramInfo);
            return retVal;
        }

        public void RefreshDefaultOT(string PayPeriod, string OvertimeFlag, string EmployeeId, string ProcessDate, string ProfileCode, DALHelper dal)
        {
            SystemCycleProcessingBL SystemCycleProcessingBL = new SystemCycleProcessingBL(dal, PayPeriod);
            SystemCycleProcessingBL.MandatoryOverTimePostingCurPayPeriod(PayPeriod, EmployeeId, ProcessDate, ProfileCode, dal);
        }

        #endregion

        public bool IsExistInLogLedger(string IDNumber, string processDate, DALHelper dal)
        {
            bool exist = true;

            string sqlSelect = @"SELECT COUNT(Ttl_IDNo)
                                 FROM T_EmpTimeRegisterLog
                                 WHERE Ttl_IDNo = @Ttl_IDNo 
                                 AND Ttl_Date = @Ttl_Date";

            ParameterInfo[] param = new ParameterInfo[2];
            param[0] = new ParameterInfo("@Ttl_IDNo", IDNumber);
            param[1] = new ParameterInfo("@Ttl_Date", processDate);

            DataSet ds;

            ds = dal.ExecuteDataSet(sqlSelect, CommandType.Text, param);

            if (getIntValue(ds.Tables[0].Rows[0][0]) <= 0)
                exist = false;

            return exist;
        }

        public DataTable GetDayCodeColor(string CentralProfile, string CompanyCode)
        {
            string query = string.Format(@"SELECT  Mdy_DayCode [DayCode]
		                                   , Mcd_Name [Color]
                                           FROM M_Day
                                           INNER JOIN M_CodeDtl ON Mdy_ColorCode = Mcd_Code 
                                            AND Mcd_CodeType = 'DAYCOLOR'
                                            AND  Mdy_CompanyCode = Mcd_CompanyCode
                                           WHERE Mdy_CompanyCode = @CompanyCode");

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@CompanyCode", CompanyCode);

            DataTable dtResult;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query, CommandType.Text, paramInfo).Tables[0];
                dal.CloseDB();
            }

            return dtResult;
        }

        public bool CheckIfWithinMinPastPeriod(string PayCycle, string CompanyCode)
        {
            CommonBL CommonBL = new CommonBL();
            if (CommonBL.GetCycleIndicator(PayCycle) == "P")
            {
                string query = string.Format(@"
                                            SELECT *
                                            FROM (
	                                            SELECT ROW_NUMBER() OVER(ORDER BY Tps_PayCycle DESC) AS PayCycleCount
			                                            , Tps_PayCycle AS [Pay Cycle]
			                                            , Tps_PayCycle + ' (' + CONVERT(VARCHAR, Tps_StartCycle, 101) + ' - ' + CONVERT(VARCHAR, Tps_EndCycle, 101) + ')' AS [Pay Cycle Range]
	                                            FROM T_PaySchedule
	                                            WHERE Tps_CycleIndicator IN ('P')
                                            ) TEMP
                                            WHERE PayCycleCount <= {1}
	                                            AND [Pay Cycle] = '{0}'"
                                                , PayCycle
                                                , (new CommonBL()).GetParameterValueFromPayroll("TKADJCYCLE", CompanyCode));
                DataTable dtResult;
                using (DALHelper dal = new DALHelper())
                {
                    dal.OpenDB();
                    dtResult = dal.ExecuteDataSet(query).Tables[0];
                    dal.CloseDB();
                }
                if (dtResult.Rows.Count > 0)
                    return true;
                else
                    return false;
            }
            else
                return true;
        }

        public bool CheckIfRecordExixstsInTrail(string Ttr_IDNo, string Ttr_PayCycle, string Ttr_AdjPayCycle)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"Select * 
                                From T_EmpTimeRegisterTrl
                                Where Ttr_IDNo = @Ttr_IDNo
                                And Ttr_PayCycle = @Ttr_PayCycle
                                And Ttr_AdjPayCycle = @Ttr_AdjPayCycle";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[3];
            paramInfo[0] = new ParameterInfo("@Ttr_IDNo", Ttr_IDNo);
            paramInfo[1] = new ParameterInfo("@Ttr_PayCycle", Ttr_PayCycle);
            paramInfo[2] = new ParameterInfo("@Ttr_AdjPayCycle", Ttr_AdjPayCycle);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);
                dal.CloseDB();
            }
            if (ds.Tables[0].Rows.Count > 0)
                return true;
            else
                return false;
        }

        public void UpdateLogControlTable(DateTime dtProcessDate, string LogLedgerTable, string CentralProfile, string UserLogin, DALHelper dal)
        {
            #region Query to Force Repost Logs
            string strRepostLogs = "";
            //if (Globals.isOverwrite == false)
            //{
            //    strRepostLogs = string.Format(@"WHERE LEFT(Tlc_Day{1}, 1) != 'F'", dtProcessDate.ToString("dd"));
            //}
            //else
            //{
            //    strRepostLogs = @""; //No condition, so that it will update the log control status of all employees
            //}
            #endregion

            #region Query
            string strQuery = string.Format(@"
                UPDATE L
                SET Tlc_Day{1} = LogStatus
                    , Tlc_UpdatedBy = '{6}'
                    , Tlc_UpdatedDate = GETDATE()
                FROM {5}..T_EmpLogControl L
                INNER JOIN (
	                SELECT Ttr_IDNo
		                , Ttr_Date
		                , Ttr_ActIn_1
		                , Ttr_ActOut_1
		                , Ttr_ActIn_2
		                , Ttr_ActOut_2
		                , CASE WHEN ((Ttr_ActIn_1= '0000'	 and Ttr_ActOut_1= '0000'	 and Ttr_ActIn_2= '0000'	and Ttr_ActOut_2<> '0000') 
								OR (Ttr_ActIn_1= '0000'	 and Ttr_ActOut_1<> '0000'	 and Ttr_ActIn_2<> '0000'	and Ttr_ActOut_2<> '0000') 
								OR  (Ttr_ActIn_1= '0000'	 and Ttr_ActOut_1<> '0000'	 and Ttr_ActIn_2= '0000'	and Ttr_ActOut_2= '0000')  
								OR  (Ttr_ActIn_1= '0000'	 and Ttr_ActOut_1<> '0000'	 and Ttr_ActIn_2= '0000'	and Ttr_ActOut_2<> '0000') 
								OR  (Ttr_ActIn_1= '0000'	 and Ttr_ActOut_1<> '0000'	 and Ttr_ActIn_2<> '0000'	and Ttr_ActOut_2= '0000')  
								OR  (Ttr_ActIn_1<> '0000'	 and Ttr_ActOut_1<> '0000'	 and Ttr_ActIn_2= '0000'	and Ttr_ActOut_2<> '0000') 
								OR  (Ttr_ActIn_1<> '0000'	 and Ttr_ActOut_1= '0000'	 and Ttr_ActIn_2= '0000'	and Ttr_ActOut_2= '0000') 
								OR  (Ttr_ActIn_1<> '0000'	 and Ttr_ActOut_1= '0000'	 and Ttr_ActIn_2<> '0000'	and Ttr_ActOut_2<> '0000') 
								OR  (Ttr_ActIn_1<> '0000'	 and Ttr_ActOut_1= '0000'	 and Ttr_ActIn_2<> '0000'	and Ttr_ActOut_2= '0000')  
								OR  (Ttr_ActIn_1= '0000'	 and Ttr_ActOut_1= '0000'	 and Ttr_ActIn_2<> '0000'	and Ttr_ActOut_2= '0000') 
								OR  (Ttr_ActIn_1<> '0000'	 and Ttr_ActOut_1<> '0000'	 and Ttr_ActIn_2<> '0000'	and Ttr_ActOut_2= '0000')) 
								THEN 'P,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114)
								WHEN (Ttr_ActIn_1 != '0000' AND Ttr_ActOut_2 != '0000')
					                OR (Ttr_ActIn_1 != '0000' AND Ttr_ActOut_1 != '0000' 
						                AND Ttr_ActIn_2 != '0000' AND Ttr_ActOut_2 != '0000')
				                THEN 'F,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114)
							    ELSE 'W,' + CONVERT(VARCHAR(10),GETDATE(),101) + ' ' + CONVERT(VARCHAR(12),GETDATE(),114)
				                END AS LogStatus
	                FROM {4}
	                WHERE Ttr_Date = '{2}'
                ) TEMP
                ON Tlc_IDNo = Ttr_IDNo
	                AND Tlc_YearMonth = '{0}'
                {3}"
                , dtProcessDate.ToString("yyyyMM")
                , dtProcessDate.ToString("dd")
                , dtProcessDate.ToShortDateString()
                , strRepostLogs
                , LogLedgerTable
                , CentralProfile
                , UserLogin);
            #endregion

            dal.ExecuteNonQuery(strQuery);
        }


        public int GetPocketCount(string IDNumber, string PayCycleCode, string tableName, string Date)
        {
            string condition = "";
            if (!PayCycleCode.Equals(""))
                condition = string.Format("AND Ttm_PayCycle = @PayCycleCode");
            else if (!Date.Equals(""))
                condition = string.Format("AND Ttm_Date = @Date");

            #region Query
            string query = string.Format(@"SELECT MAX(Seq)
                                FROM {0}
                                CROSS APPLY ( VALUES ('01', Ttm_ActIn_01,Ttm_Actout_01),
					                                ('02', Ttm_ActIn_02,Ttm_Actout_02),
					                                ('03', Ttm_ActIn_03,Ttm_Actout_03),
					                                ('04', Ttm_ActIn_04,Ttm_Actout_04),
					                                ('05', Ttm_ActIn_05,Ttm_Actout_05),
					                                ('06', Ttm_ActIn_06,Ttm_Actout_06),
					                                ('07', Ttm_ActIn_07,Ttm_Actout_07),
					                                ('08', Ttm_ActIn_08,Ttm_Actout_08),
					                                ('09', Ttm_ActIn_09,Ttm_Actout_09),
					                                ('10', Ttm_ActIn_10,Ttm_Actout_10)) temp (Seq, TIn, TOut)
                                WHERE Ttm_IDNo = @IDNo
	                                {1}
	                                AND (TIn <> '0000' OR TOut <> '0000')"
                                , tableName
                                , condition);

            ParameterInfo[] paramInfo = new ParameterInfo[3];
            paramInfo[0] = new ParameterInfo("@IDNo", IDNumber);
            paramInfo[1] = new ParameterInfo("@Date", Date);
            paramInfo[2] = new ParameterInfo("@PayCycleCode", PayCycleCode);

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query, CommandType.Text, paramInfo).Tables[0];
                dal.CloseDB();
            }
            if (dtResult.Rows.Count > 0 && dtResult.Rows[0][0].ToString() != "")
                return Convert.ToInt32(dtResult.Rows[0][0]);
            else
                return 0;
            #endregion
        }

        public void InsertLogLedgerTrail(string IDNumber, string PayCycleCode, string CompanyCode, DALHelper dal)
        {
            #region <query>

            string qString = @"
DECLARE @EmployeeID VARCHAR(15) = @Ttr_IDNo
DECLARE @AffectedPayPeriod VARCHAR(7) = @Ttr_PayCycle
DECLARE @AdjustPayPeriod VARCHAR(7) = (Select Tps_PayCycle 
									   From T_PaySchedule 
									   Where Tps_CycleIndicator='C' 
										and Tps_RecordStatus = 'A')
DECLARE @POCKETSIZE AS INT = (SELECT Mph_NumValue FROM M_PolicyHdr WHERE Mph_PolicyCode = 'POCKETSIZE' AND Mph_CompanyCode = @CompanyCode)

--INSERT INTO LOG LEDGER TRAIL
IF NOT EXISTS (
	Select TOP 1 * 
	From T_EmpTimeRegisterTrl
	Where Ttr_IDNo = @EmployeeID
		And Ttr_PayCycle = @AffectedPayPeriod
		And Ttr_AdjPayCycle = @AdjustPayPeriod
)
INSERT INTO T_EmpTimeRegisterTrl
(
  Ttr_IDNo
, Ttr_Date
, Ttr_AdjPayCycle
, Ttr_PayCycle
, Ttr_DayCode
, Ttr_ShiftCode
, Ttr_HolidayFlag
, Ttr_RestDayFlag
, Ttr_ActIn_1
, Ttr_ActOut_1
, Ttr_ActIn_2
, Ttr_ActOut_2
, Ttr_WFPayLVCode
, Ttr_WFPayLVHr
, Ttr_PayLVMin
, Ttr_ExcLVMin
, Ttr_WFNoPayLVCode
, Ttr_WFNoPayLVHr
, Ttr_NoPayLVMin
, Ttr_WFOTAdvHr
, Ttr_WFOTPostHr
, Ttr_OTMin
, Ttr_CompOTMin
, Ttr_OffsetOTMin
, Ttr_WFTimeMod
, Ttr_WFFlexTime
, Ttr_Amnesty
, Ttr_SkipService
, Ttr_SkipServiceBy
, Ttr_SkipServiceDate
, Ttr_AssumedFlag
, Ttr_AssumedBy
, Ttr_AssumedDate
, Ttr_AssumedPost
, Ttr_ConvIn_1Min
, Ttr_ConvOut_1Min
, Ttr_ConvIn_2Min
, Ttr_ConvOut_2Min
, Ttr_CompIn_1Min
, Ttr_CompOut_1Min
, Ttr_CompIn_2Min
, Ttr_CompOut_2Min
, Ttr_CompAdvOTMin
, Ttr_ShiftIn_1Min
, Ttr_ShiftOut_1Min
, Ttr_ShiftIn_2Min
, Ttr_ShiftOut_2Min
, Ttr_ShiftMin
, Ttr_ScheduleType
, Ttr_ActLT1Min
, Ttr_ActLT2Min
, Ttr_CompLT1Min
, Ttr_CompLT2Min
, Ttr_ActUT1Min
, Ttr_ActUT2Min
, Ttr_CompUT1Min
, Ttr_CompUT2Min
, Ttr_InitialABSMin
, Ttr_CompABSMin
, Ttr_CompREGMin
, Ttr_CompWorkMin
, Ttr_CompNDMin
, Ttr_CompNDOTMin
, Ttr_PrvDayWorkMin
, Ttr_PrvDayHolRef
, Ttr_PDHOLHour
, Ttr_PDRESTLEGHOLDay
, Ttr_WorkDay
, Ttr_EXPHour
, Ttr_ABSHour
, Ttr_REGHour
, Ttr_OTHour
, Ttr_NDHour
, Ttr_NDOTHour
, Ttr_LVHour
, Ttr_PaidBreakHour
, Ttr_OBHour
, Ttr_RegPlusHour
, Ttr_TBAmt01
, Ttr_TBAmt02
, Ttr_TBAmt03
, Ttr_TBAmt04
, Ttr_TBAmt05
, Ttr_TBAmt06
, Ttr_TBAmt07
, Ttr_TBAmt08
, Ttr_TBAmt09
, Ttr_TBAmt10
, Ttr_TBAmt11
, Ttr_TBAmt12
, Ttr_WorkLocationCode
, Ttr_CalendarGroup
, Ttr_PremiumGrpCode
, Ttr_PayrollGroup
, Ttr_CostcenterCode
, Ttr_EmploymentStatusCode
, Ttr_PayrollType
, Ttr_Grade
, Usr_Login
, Ludatetime
)
Select Ttr_IDNo
	,Ttr_Date
	,@AdjustPayPeriod
    , Ttr_PayCycle
    , Ttr_DayCode
    , Ttr_ShiftCode
    , Ttr_HolidayFlag
    , Ttr_RestDayFlag
    , Ttr_ActIn_1
    , Ttr_ActOut_1
    , Ttr_ActIn_2
    , Ttr_ActOut_2
    , Ttr_WFPayLVCode
    , Ttr_WFPayLVHr
    , Ttr_PayLVMin
    , Ttr_ExcLVMin
    , Ttr_WFNoPayLVCode
    , Ttr_WFNoPayLVHr
    , Ttr_NoPayLVMin
    , Ttr_WFOTAdvHr
    , Ttr_WFOTPostHr
    , Ttr_OTMin
    , Ttr_CompOTMin
    , Ttr_OffsetOTMin
    , Ttr_WFTimeMod
    , Ttr_WFFlexTime
    , Ttr_Amnesty
    , Ttr_SkipService
    , Ttr_SkipServiceBy
    , Ttr_SkipServiceDate
    , Ttr_AssumedFlag
    , Ttr_AssumedBy
    , Ttr_AssumedDate
    , Ttr_AssumedPost
    , Ttr_ConvIn_1Min
    , Ttr_ConvOut_1Min
    , Ttr_ConvIn_2Min
    , Ttr_ConvOut_2Min
    , Ttr_CompIn_1Min
    , Ttr_CompOut_1Min
    , Ttr_CompIn_2Min
    , Ttr_CompOut_2Min
    , Ttr_CompAdvOTMin
    , Ttr_ShiftIn_1Min
    , Ttr_ShiftOut_1Min
    , Ttr_ShiftIn_2Min
    , Ttr_ShiftOut_2Min
    , Ttr_ShiftMin
    , Ttr_ScheduleType
    , Ttr_ActLT1Min
    , Ttr_ActLT2Min
    , Ttr_CompLT1Min
    , Ttr_CompLT2Min
    , Ttr_ActUT1Min
    , Ttr_ActUT2Min
    , Ttr_CompUT1Min
    , Ttr_CompUT2Min
    , Ttr_InitialABSMin
    , Ttr_CompABSMin
    , Ttr_CompREGMin
    , Ttr_CompWorkMin
    , Ttr_CompNDMin
    , Ttr_CompNDOTMin
    , Ttr_PrvDayWorkMin
    , Ttr_PrvDayHolRef
    , Ttr_PDHOLHour
    , Ttr_PDRESTLEGHOLDay
    , Ttr_WorkDay
    , Ttr_EXPHour
    , Ttr_ABSHour
    , Ttr_REGHour
    , Ttr_OTHour
    , Ttr_NDHour
    , Ttr_NDOTHour
    , Ttr_LVHour
    , Ttr_PaidBreakHour
    , Ttr_OBHour
    , Ttr_RegPlusHour
    , Ttr_TBAmt01
    , Ttr_TBAmt02
    , Ttr_TBAmt03
    , Ttr_TBAmt04
    , Ttr_TBAmt05
    , Ttr_TBAmt06
    , Ttr_TBAmt07
    , Ttr_TBAmt08
    , Ttr_TBAmt09
    , Ttr_TBAmt10
    , Ttr_TBAmt11
    , Ttr_TBAmt12
    , Ttr_WorkLocationCode
    , Ttr_CalendarGroup
    , Ttr_PremiumGrpCode
    , Ttr_PayrollGroup
    , Ttr_CostcenterCode
    , Ttr_EmploymentStatusCode
    , Ttr_PayrollType
    , Ttr_Grade
    , Usr_Login
    , Ludatetime
From T_EmpTimeRegisterHst
Where Ttr_IDNo = @EmployeeID
	And Ttr_PayCycle = @AffectedPayPeriod

IF (@POCKETSIZE > 2)
BEGIN
--INSERT INTO LOG LEDGER EXT. TRAIL
IF NOT EXISTS (
	Select TOP 1 * 
	From T_EmpTimeRegisterMiscTrl
	Where Ttm_IDNo = @EmployeeID
		And Ttm_PayCycle = @AffectedPayPeriod
		And Ttm_AdjPayCycle = @AdjustPayPeriod
)
INSERT INTO T_EmpTimeRegisterMiscTrl
(
    Ttm_IDNo
    , Ttm_Date
    , Ttm_AdjPayCycle
    , Ttm_PayCycle
    , Ttm_ActIn_01
    , Ttm_ActOut_01
    , Ttm_ActIn_02
    , Ttm_ActOut_02
    , Ttm_ActIn_03
    , Ttm_ActOut_03
    , Ttm_ActIn_04
    , Ttm_ActOut_04
    , Ttm_ActIn_05
    , Ttm_ActOut_05
    , Ttm_ActIn_06
    , Ttm_ActOut_06
    , Ttm_ActIn_07
    , Ttm_ActOut_07
    , Ttm_ActIn_08
    , Ttm_ActOut_08
    , Ttm_ActIn_09
    , Ttm_ActOut_09
    , Ttm_ActIn_10
    , Ttm_ActOut_10
    , Ttm_ActIn_11
    , Ttm_ActOut_11
    , Ttm_ActIn_12
    , Ttm_ActOut_12
    , Ttm_Result
    , Ttm_ActIn1
    , Ttm_ActOut1
    , Ttm_ActIn2
    , Ttm_ActOut2
    , Usr_Login
    , Ludatetime
)
SELECT Ttm_IDNo
    ,Ttm_Date
    ,@AdjustPayPeriod
    ,Ttm_PayCycle
    ,Ttm_ActIn_01
    ,Ttm_ActOut_01
    ,Ttm_ActIn_02
    ,Ttm_ActOut_02
    ,Ttm_ActIn_03
    ,Ttm_ActOut_03
    ,Ttm_ActIn_04
    ,Ttm_ActOut_04
    ,Ttm_ActIn_05
    ,Ttm_ActOut_05
    ,Ttm_ActIn_06
    ,Ttm_ActOut_06
    ,Ttm_ActIn_07
    ,Ttm_ActOut_07
    ,Ttm_ActIn_08
    ,Ttm_ActOut_08
    ,Ttm_ActIn_09
    ,Ttm_ActOut_09
    ,Ttm_ActIn_10
    ,Ttm_ActOut_10
    ,Ttm_ActIn_11
    ,Ttm_ActOut_11
    ,Ttm_ActIn_12
    ,Ttm_ActOut_12
    ,Ttm_Result
    ,Ttm_ActIn1
    ,Ttm_ActOut1
    ,Ttm_ActIn2
    ,Ttm_ActOut2
    ,Usr_Login
    ,Ludatetime
From T_EmpTimeRegisterMiscHst
Where Ttm_IDNo = @EmployeeID
	And Ttm_PayCycle = @AffectedPayPeriod
END

--INSERT INTO PAYROLL TRANSACTION TRAIL
IF NOT EXISTS (
	Select TOP 1 * 
	From T_EmpPayTranHdrTrl
	Where Tph_IDNo = @EmployeeID
		And Tph_PayCycle = @AffectedPayPeriod
		And Tph_AdjPayCycle = @AdjustPayPeriod
)
INSERT INTO T_EmpPayTranHdrTrl
(       Tph_IDNo
            ,Tph_AdjPayCycle
            ,Tph_PayCycle
            ,Tph_LTHr
            ,Tph_UTHr
            ,Tph_UPLVHr
            ,Tph_ABSLEGHOLHr
            ,Tph_ABSSPLHOLHr
            ,Tph_ABSCOMPHOLHr
            ,Tph_ABSPSDHr
            ,Tph_ABSOTHHOLHr
            ,Tph_WDABSHr
            ,Tph_ABSHr
            ,Tph_REGHr
            ,Tph_PDLVHr
            ,Tph_PDLEGHOLHr
            ,Tph_PDSPLHOLHr
            ,Tph_PDCOMPHOLHr
            ,Tph_PDPSDHr
            ,Tph_PDOTHHOLHr
            ,Tph_PDRESTLEGHOLHr
            ,Tph_REGOTHr
            ,Tph_REGNDHr
            ,Tph_REGNDOTHr
            ,Tph_RESTHr
            ,Tph_RESTOTHr
            ,Tph_RESTNDHr
            ,Tph_RESTNDOTHr
            ,Tph_LEGHOLHr
            ,Tph_LEGHOLOTHr
            ,Tph_LEGHOLNDHr
            ,Tph_LEGHOLNDOTHr
            ,Tph_SPLHOLHr
            ,Tph_SPLHOLOTHr
            ,Tph_SPLHOLNDHr
            ,Tph_SPLHOLNDOTHr
            ,Tph_PSDHr
            ,Tph_PSDOTHr
            ,Tph_PSDNDHr
            ,Tph_PSDNDOTHr
            ,Tph_COMPHOLHr
            ,Tph_COMPHOLOTHr
            ,Tph_COMPHOLNDHr
            ,Tph_COMPHOLNDOTHr
            ,Tph_RESTLEGHOLHr
            ,Tph_RESTLEGHOLOTHr
            ,Tph_RESTLEGHOLNDHr
            ,Tph_RESTLEGHOLNDOTHr
            ,Tph_RESTSPLHOLHr
            ,Tph_RESTSPLHOLOTHr
            ,Tph_RESTSPLHOLNDHr
            ,Tph_RESTSPLHOLNDOTHr
            ,Tph_RESTCOMPHOLHr
            ,Tph_RESTCOMPHOLOTHr
            ,Tph_RESTCOMPHOLNDHr
            ,Tph_RESTCOMPHOLNDOTHr
            ,Tph_RESTPSDHr
            ,Tph_RESTPSDOTHr
            ,Tph_RESTPSDNDHr
            ,Tph_RESTPSDNDOTHr
            ,Tph_SRGAdjHr
            ,Tph_SRGAdjAmt
            ,Tph_SOTAdjHr
            ,Tph_SOTAdjAmt
            ,Tph_SHOLAdjHr
            ,Tph_SHOLAdjAmt
            ,Tph_SNDAdjHr
            ,Tph_SNDAdjAmt
            ,Tph_SLVAdjHr
            ,Tph_SLVAdjAmt
            ,Tph_MRGAdjHr
            ,Tph_MRGAdjAmt
            ,Tph_MOTAdjHr
            ,Tph_MOTAdjAmt
            ,Tph_MHOLAdjHr
            ,Tph_MHOLAdjAmt
            ,Tph_MNDAdjHr
            ,Tph_MNDAdjAmt
            ,Tph_TotalAdjAmt
            ,Tph_TaxableIncomeAmt
            ,Tph_NontaxableIncomeAmt
            ,Tph_WorkDay
            ,Tph_PayrollType
            ,Tph_RetainUserEntry
            ,Usr_Login
            ,Ludatetime
)
Select Tph_IDNo
        ,@AdjustPayPeriod
        ,Tph_PayCycle
        ,Tph_LTHr
        ,Tph_UTHr
        ,Tph_UPLVHr
        ,Tph_ABSLEGHOLHr
        ,Tph_ABSSPLHOLHr
        ,Tph_ABSCOMPHOLHr
        ,Tph_ABSPSDHr
        ,Tph_ABSOTHHOLHr
        ,Tph_WDABSHr
        ,Tph_ABSHr
        ,Tph_REGHr
        ,Tph_PDLVHr
        ,Tph_PDLEGHOLHr
        ,Tph_PDSPLHOLHr
        ,Tph_PDCOMPHOLHr
        ,Tph_PDPSDHr
        ,Tph_PDOTHHOLHr
        ,Tph_PDRESTLEGHOLHr
        ,Tph_REGOTHr
        ,Tph_REGNDHr
        ,Tph_REGNDOTHr
        ,Tph_RESTHr
        ,Tph_RESTOTHr
        ,Tph_RESTNDHr
        ,Tph_RESTNDOTHr
        ,Tph_LEGHOLHr
        ,Tph_LEGHOLOTHr
        ,Tph_LEGHOLNDHr
        ,Tph_LEGHOLNDOTHr
        ,Tph_SPLHOLHr
        ,Tph_SPLHOLOTHr
        ,Tph_SPLHOLNDHr
        ,Tph_SPLHOLNDOTHr
        ,Tph_PSDHr
        ,Tph_PSDOTHr
        ,Tph_PSDNDHr
        ,Tph_PSDNDOTHr
        ,Tph_COMPHOLHr
        ,Tph_COMPHOLOTHr
        ,Tph_COMPHOLNDHr
        ,Tph_COMPHOLNDOTHr
        ,Tph_RESTLEGHOLHr
        ,Tph_RESTLEGHOLOTHr
        ,Tph_RESTLEGHOLNDHr
        ,Tph_RESTLEGHOLNDOTHr
        ,Tph_RESTSPLHOLHr
        ,Tph_RESTSPLHOLOTHr
        ,Tph_RESTSPLHOLNDHr
        ,Tph_RESTSPLHOLNDOTHr
        ,Tph_RESTCOMPHOLHr
        ,Tph_RESTCOMPHOLOTHr
        ,Tph_RESTCOMPHOLNDHr
        ,Tph_RESTCOMPHOLNDOTHr
        ,Tph_RESTPSDHr
        ,Tph_RESTPSDOTHr
        ,Tph_RESTPSDNDHr
        ,Tph_RESTPSDNDOTHr
        ,Tph_SRGAdjHr
        ,Tph_SRGAdjAmt
        ,Tph_SOTAdjHr
        ,Tph_SOTAdjAmt
        ,Tph_SHOLAdjHr
        ,Tph_SHOLAdjAmt
        ,Tph_SNDAdjHr
        ,Tph_SNDAdjAmt
        ,Tph_SLVAdjHr
        ,Tph_SLVAdjAmt
        ,Tph_MRGAdjHr
        ,Tph_MRGAdjAmt
        ,Tph_MOTAdjHr
        ,Tph_MOTAdjAmt
        ,Tph_MHOLAdjHr
        ,Tph_MHOLAdjAmt
        ,Tph_MNDAdjHr
        ,Tph_MNDAdjAmt
        ,Tph_TotalAdjAmt
        ,Tph_TaxableIncomeAmt
        ,Tph_NontaxableIncomeAmt
        ,Tph_WorkDay
        ,Tph_PayrollType
        ,Tph_RetainUserEntry
        ,Usr_Login
        ,Ludatetime
From T_EmpPayTranHdrHst
Where Tph_IDNo = @EmployeeID
	And Tph_PayCycle = @AffectedPayPeriod

--INSERT INTO PAYROLL TRANSACTION TRAIL DETAIL
IF NOT EXISTS (
	Select TOP 1 * 
	From T_EmpPayTranDtlTrl
	Where Tpd_IDNo = @EmployeeID
		And Tpd_PayCycle = @AffectedPayPeriod
		And Tpd_AdjPayCycle = @AdjustPayPeriod
)
INSERT INTO T_EmpPayTranDtlTrl
(       Tpd_IDNo
        ,Tpd_AdjPayCycle
        ,Tpd_PayCycle
        ,Tpd_Date
        ,Tpd_LTHr
        ,Tpd_UTHr
        ,Tpd_UPLVHr
        ,Tpd_ABSLEGHOLHr
        ,Tpd_ABSSPLHOLHr
        ,Tpd_ABSCOMPHOLHr
        ,Tpd_ABSPSDHr
        ,Tpd_ABSOTHHOLHr
        ,Tpd_WDABSHr
        ,Tpd_ABSHr
        ,Tpd_REGHr
        ,Tpd_PDLVHr
        ,Tpd_PDLEGHOLHr
        ,Tpd_PDSPLHOLHr
        ,Tpd_PDCOMPHOLHr
        ,Tpd_PDPSDHr
        ,Tpd_PDOTHHOLHr
        ,Tpd_PDRESTLEGHOLHr
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
        ,Tpd_WorkDay
        ,Tpd_PayrollType
        ,Tpd_PremiumGrpCode
        ,Usr_Login
        ,Ludatetime
)
Select Tpd_IDNo
        ,@AdjustPayPeriod
        ,Tpd_PayCycle
        ,Tpd_Date
        ,Tpd_LTHr
        ,Tpd_UTHr
        ,Tpd_UPLVHr
        ,Tpd_ABSLEGHOLHr
        ,Tpd_ABSSPLHOLHr
        ,Tpd_ABSCOMPHOLHr
        ,Tpd_ABSPSDHr
        ,Tpd_ABSOTHHOLHr
        ,Tpd_WDABSHr
        ,Tpd_ABSHr
        ,Tpd_REGHr
        ,Tpd_PDLVHr
        ,Tpd_PDLEGHOLHr
        ,Tpd_PDSPLHOLHr
        ,Tpd_PDCOMPHOLHr
        ,Tpd_PDPSDHr
        ,Tpd_PDOTHHOLHr
        ,Tpd_PDRESTLEGHOLHr
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
        ,Tpd_WorkDay
        ,Tpd_PayrollType
        ,Tpd_PremiumGrpCode
        ,Usr_Login
        ,Ludatetime
From T_EmpPayTranDtlHst
Where Tpd_IDNo = @EmployeeID
	And Tpd_PayCycle = @AffectedPayPeriod
	
--INSERT INTO PAYROLL TRANSACTION TRAIL EXT
IF NOT EXISTS (
	Select TOP 1 * 
	From T_EmpPayTranHdrMiscTrl
	Where Tph_IDNo = @EmployeeID
		And Tph_PayCycle = @AffectedPayPeriod
		And Tph_AdjPayCycle = @AdjustPayPeriod
)
INSERT INTO T_EmpPayTranHdrMiscTrl
(
	   Tph_IDNo
      ,Tph_AdjPayCycle
      ,Tph_PayCycle
      ,Tph_Misc1Hr
      ,Tph_Misc1OTHr
      ,Tph_Misc1NDHr
      ,Tph_Misc1NDOTHr
      ,Tph_Misc2Hr
      ,Tph_Misc2OTHr
      ,Tph_Misc2NDHr
      ,Tph_Misc2NDOTHr
      ,Tph_Misc3Hr
      ,Tph_Misc3OTHr
      ,Tph_Misc3NDHr
      ,Tph_Misc3NDOTHr
      ,Tph_Misc4Hr
      ,Tph_Misc4OTHr
      ,Tph_Misc4NDHr
      ,Tph_Misc4NDOTHr
      ,Tph_Misc5Hr
      ,Tph_Misc5OTHr
      ,Tph_Misc5NDHr
      ,Tph_Misc5NDOTHr
      ,Tph_Misc6Hr
      ,Tph_Misc6OTHr
      ,Tph_Misc6NDHr
      ,Tph_Misc6NDOTHr
      ,Usr_Login
      ,Ludatetime
)
Select Tph_IDNo
      ,@AdjustPayPeriod
      ,Tph_PayCycle
      ,Tph_Misc1Hr
      ,Tph_Misc1OTHr
      ,Tph_Misc1NDHr
      ,Tph_Misc1NDOTHr
      ,Tph_Misc2Hr
      ,Tph_Misc2OTHr
      ,Tph_Misc2NDHr
      ,Tph_Misc2NDOTHr
      ,Tph_Misc3Hr
      ,Tph_Misc3OTHr
      ,Tph_Misc3NDHr
      ,Tph_Misc3NDOTHr
      ,Tph_Misc4Hr
      ,Tph_Misc4OTHr
      ,Tph_Misc4NDHr
      ,Tph_Misc4NDOTHr
      ,Tph_Misc5Hr
      ,Tph_Misc5OTHr
      ,Tph_Misc5NDHr
      ,Tph_Misc5NDOTHr
      ,Tph_Misc6Hr
      ,Tph_Misc6OTHr
      ,Tph_Misc6NDHr
      ,Tph_Misc6NDOTHr
      ,Usr_Login
      ,Ludatetime
From T_EmpPayTranHdrMiscHst
Where Tph_IDNo = @EmployeeID
	And Tph_PayCycle = @AffectedPayPeriod
	
--INSERT INTO PAYROLL TRANSACTION TRAIL EXT DETAIL
IF NOT EXISTS (
	Select TOP 1 * 
	From T_EmpPayTranDtlMiscTrl
	Where Tpd_IDNo = @EmployeeID
		And Tpd_PayCycle = @AffectedPayPeriod
		And Tpd_AdjPayCycle = @AdjustPayPeriod
)
INSERT INTO T_EmpPayTranDtlMiscTrl
(
	   Tpd_IDNo
      ,Tpd_AdjPayCycle
      ,Tpd_PayCycle
      ,Tpd_Date
      ,Tpd_Misc1Hr
      ,Tpd_Misc1OTHr
      ,Tpd_Misc1NDHr
      ,Tpd_Misc1NDOTHr
      ,Tpd_Misc2Hr
      ,Tpd_Misc2OTHr
      ,Tpd_Misc2NDHr
      ,Tpd_Misc2NDOTHr
      ,Tpd_Misc3Hr
      ,Tpd_Misc3OTHr
      ,Tpd_Misc3NDHr
      ,Tpd_Misc3NDOTHr
      ,Tpd_Misc4Hr
      ,Tpd_Misc4OTHr
      ,Tpd_Misc4NDHr
      ,Tpd_Misc4NDOTHr
      ,Tpd_Misc5Hr
      ,Tpd_Misc5OTHr
      ,Tpd_Misc5NDHr
      ,Tpd_Misc5NDOTHr
      ,Tpd_Misc6Hr
      ,Tpd_Misc6OTHr
      ,Tpd_Misc6NDHr
      ,Tpd_Misc6NDOTHr
      ,Usr_Login
      ,Ludatetime
)
Select Tpd_IDNo
      ,@AdjustPayPeriod
      ,Tpd_PayCycle
      ,Tpd_Date
      ,Tpd_Misc1Hr
      ,Tpd_Misc1OTHr
      ,Tpd_Misc1NDHr
      ,Tpd_Misc1NDOTHr
      ,Tpd_Misc2Hr
      ,Tpd_Misc2OTHr
      ,Tpd_Misc2NDHr
      ,Tpd_Misc2NDOTHr
      ,Tpd_Misc3Hr
      ,Tpd_Misc3OTHr
      ,Tpd_Misc3NDHr
      ,Tpd_Misc3NDOTHr
      ,Tpd_Misc4Hr
      ,Tpd_Misc4OTHr
      ,Tpd_Misc4NDHr
      ,Tpd_Misc4NDOTHr
      ,Tpd_Misc5Hr
      ,Tpd_Misc5OTHr
      ,Tpd_Misc5NDHr
      ,Tpd_Misc5NDOTHr
      ,Tpd_Misc6Hr
      ,Tpd_Misc6OTHr
      ,Tpd_Misc6NDHr
      ,Tpd_Misc6NDOTHr
      ,Usr_Login
      ,Ludatetime
From T_EmpPayTranDtlMiscHst
Where Tpd_IDNo = @EmployeeID
	And Tpd_PayCycle = @AffectedPayPeriod";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[3];
            paramInfo[0] = new ParameterInfo("@Ttr_IDNo", IDNumber);
            paramInfo[1] = new ParameterInfo("@Ttr_PayCycle", PayCycleCode);
            paramInfo[2] = new ParameterInfo("@CompanyCode", CompanyCode);

            dal.ExecuteNonQuery(qString, CommandType.Text, paramInfo);
        }

        public DataSet CheckIfDayCodeExists(string DayCode, string CompanyCode, string CentralProfile)
        {
            DataSet ds = new DataSet();

            #region <query>

            string qString = @"SELECT Mdy_DayCode
                                FROM M_Day
                                WHERE Mdy_RecordStatus = 'A'
                                AND Mdy_DayCode = @Mdy_DayCode
                                AND Mdy_CompanyCode = @Mdy_CompanyCode";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Mdy_DayCode", DayCode);
            paramInfo[1] = new ParameterInfo("@Mdy_CompanyCode", CompanyCode);

            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            return ds;
        }

        public DataTable GetDayCodeFillerColor(string CompanyCode, string CentralProfile)
        {
            string query = @"SELECT [Mmd_MiscDayID]
                                  ,[Mmd_DayCode]
                                  ,Mcd_Name
                              FROM M_MiscellaneousDay
                              INNER JOIN M_Day
                              ON Mmd_DayCode = Mdy_DayCode
                              AND Mmd_CompanyCode = Mdy_CompanyCode
                              LEFT JOIN M_CodeDtl
                              ON Mcd_Code = Mdy_ColorCode
                              AND Mcd_CompanyCode = Mdy_CompanyCode
                              WHERE Mmd_CompanyCode = @CompanyCode";

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@CompanyCode", CompanyCode);

            DataTable dtResult = null;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dtResult = dal.ExecuteDataSet(query, CommandType.Text, paramInfo).Tables[0];
            }
            return dtResult;
        }


        public DataTable GetEmployeeTimeRegister()
        {
            #region query
            string query = @"SELECT Ttr_IDNo
                            , Ttr_Date
                            , Ttr_PayCycle
                            , Ttr_DayCode
                            , Ttr_ShiftCode
                            , Ttr_Amnesty
                            , Ttr_ActIn_1 [IN1]
                            , Ttr_ActOut_1 [OUT1]
                            , Ttr_ActIn_2 [IN2]
                            , Ttr_ActOut_2 [OUT2]
                            FROM T_EmpTimeRegister";
            #endregion

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }


        public DataTable GetEmployeeTimeRegisterMisc()
        {
            #region query
            string query = @"SELECT Ttm_IDNo
                            ,Ttm_Date
                            ,Ttm_PayCycle
                            ,Ttm_ActIn_01 [IN1_EXT]
                            ,Ttm_ActOut_01 [OUT1_EXT]
                            ,Ttm_ActIn_02 [IN2_EXT]
                            ,Ttm_ActOut_02 [OUT2_EXT]
                            ,Ttm_ActIn_03 [IN3_EXT]
                            ,Ttm_ActOut_03 [OUT3_EXT]
                            ,Ttm_ActIn_04 [IN4_EXT]
                            ,Ttm_ActOut_04 [OUT4_EXT]
                            ,Ttm_ActIn_05 [IN5_EXT]
                            ,Ttm_ActOut_05 [OUT5_EXT]
                            ,Ttm_ActIn_06 [IN6_EXT]
                            ,Ttm_ActOut_06 [OUT6_EXT]
                            ,Ttm_ActIn_07 [IN7_EXT]
                            ,Ttm_ActOut_07 [OUT7_EXT]
                            ,Ttm_ActIn_08 [IN8_EXT]
                            ,Ttm_ActOut_08 [OUT8_EXT]
                            ,Ttm_ActIn_09 [IN9_EXT]
                            ,Ttm_ActOut_09 [OUT9_EXT]
                            ,Ttm_ActIn_10 [IN10_EXT]
                            ,Ttm_ActOut_10 [OUT10_EXT]
                            ,Ttm_ActIn_11 [IN11_EXT]
                            ,Ttm_ActOut_11 [OUT11_EXT]
                            ,Ttm_ActIn_12 [IN12_EXT]
                            ,Ttm_ActOut_12 [OUT12_EXT]
                            FROM T_EmpTimeRegisterMisc";
            #endregion

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }


        public DataTable GetEmployeeMasterList()
        {
            #region query
            string query = string.Format(@"SELECT Mem_IDNo
                                            , Mem_IntakeDate
                                            , Mem_SeparationDate
                                            , Mem_WorkStatus
                                           FROM M_Employee");
            #endregion
            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetDTROverride()
        {
            #region query
            string query = string.Format(@"SELECT Tdo_IDNo [ID Number]
                                            , Tdo_Date [Date]
                                            , Tdo_Type [Type]
                                            , Tdo_Time [Time]
                                            , Tdo_Remarks [Remarks]
                                            FROM T_EmpDTROverride");
            #endregion
            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetWaived()
        {
            #region query
            string query = string.Format(@"SELECT Ttr_IDNo [ID Number]
                                            , Ttr_Date [Date]
                                            , Ttr_Amnesty [With Anmesty]
                                            FROM T_EmpTimeRegister
                                            WHERE Ttr_Amnesty IN ('L','U','B')");
            #endregion
            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public DataTable GetDTR(string IDNumber, string Date)
        {
            #region query
            string query = string.Format(@"SELECT Tel_IDNo
                                            , Tel_LogTime
                                            , Ludatetime 
                                            FROM T_EmpDTR
                                            WHERE Tel_IDNo = '{0}'
                                                AND Tel_LogDate = '{1}'
                                           ORDER BY Ludatetime"
                                        , IDNumber, Date);
            #endregion
            DataTable dtResult;
            using (DALHelper dal = new DALHelper(true))
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }

        public bool IsDayCodeExist(string DayCode, string CompanyCode, string CentralProfile)
        {
            string query = string.Format(@"SELECT Mdy_DayCode 
                                           FROM M_Day 
                                           WHERE Mdy_DayCode = @DayCode
                                                AND Mdy_CompanyCode = @CompanyCode");

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@CompanyCode", CompanyCode);
            paramInfo[1] = new ParameterInfo("@DayCode", DayCode);

            DataTable dtResult = new DataTable();
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();

                dtResult = dal.ExecuteDataSet(query, CommandType.Text, paramInfo).Tables[0];

                dal.CloseDB();
            }

            return (dtResult.Rows.Count > 0) ? true : false;
        }

        public void UpdateShiftCodesInLedger(string EmployeeID, string ProcessDate, string ShiftCode, DALHelper dal)
        {
            string query;

            query = string.Format(@"
                IF NOT EXISTS(SELECT Ttl_IDNo FROM T_EmpTimeRegisterLog 
                                WHERE Ttl_IDNo = '{1}'
                                    AND Ttl_Date = '{2}')  -- IF NO RECORD IN TRAIL YET
                BEGIN
                    -- 1. INSERT ORIGINAL RECORD
                    INSERT INTO T_EmpTimeRegisterLog
                    (
                        Ttl_IDNo
                        , Ttl_Date
                        , Ttl_DayCode
                        , Ttl_RestDayFlag
                        , Ttl_HolidayFlag
                        , Ttl_ShiftCode
                        , Ttl_ActIn_1
                        , Ttl_ActOut_1
                        , Ttl_ActIn_2
                        , Ttl_ActOut_2
                        , Ttl_SkipService
                        , Ttl_AssumedFlag
                        , Ttl_Amnesty
                        , Ttl_Remarks
                        , Ttl_DocumentBatchNo
                        , Usr_Login
                        , Ludatetime
                    )
                    SELECT
                        Ttr_IDNo
                        , Ttr_Date
                        , Ttr_DayCode
                        , Ttr_RestDayFlag
                        , Ttr_HolidayFlag
                        , Ttr_ShiftCode
                        , Ttr_ActIn_1
                        , Ttr_ActOut_1
                        , Ttr_ActIn_2
                        , Ttr_ActOut_2
                        , Ttr_SkipService
                        , Ttr_AssumedFlag
                        , Ttr_Amnesty
                        , ''
                        , ''
                        , Usr_Login
                        , Ludatetime
                    FROM T_EmpTimeRegister
                    WHERE Ttr_IDNo = '{1}'
                        AND Ttr_Date = '{2}'

                    -- 2. UPDATE LEDGER TO DESIRED VALUES
                    UPDATE T_EmpTimeRegister SET Ttr_ShiftCode = '{0}'
                        , Usr_Login = '{3}'
                        , Ludatetime = GETDATE()
                    WHERE Ttr_IDNo = '{1}'
                        AND Ttr_Date = '{2}'

                    -- 3. INSERT UPDATED RECORDS TO TRAIL
                    INSERT INTO T_EmpTimeRegisterLog
                    (
                        Ttl_IDNo
                        , Ttl_Date
                        , Ttl_DayCode
                        , Ttl_RestDayFlag
                        , Ttl_HolidayFlag
                        , Ttl_ShiftCode
                        , Ttl_ActIn_1
                        , Ttl_ActOut_1
                        , Ttl_ActIn_2
                        , Ttl_ActOut_2
                        , Ttl_SkipService
                        , Ttl_AssumedFlag
                        , Ttl_Amnesty
                        , Ttl_Remarks
                        , Ttl_DocumentBatchNo
                        , Usr_Login
                        , Ludatetime
                    )
                    SELECT
                        Ttr_IDNo
                        , Ttr_Date
                        , Ttr_DayCode
                        , Ttr_RestDayFlag
                        , Ttr_HolidayFlag
                        , Ttr_ShiftCode
                        , Ttr_ActIn_1
                        , Ttr_ActOut_1
                        , Ttr_ActIn_2
                        , Ttr_ActOut_2
                        , Ttr_SkipService
                        , Ttr_AssumedFlag
                        , Ttr_Amnesty
                        , ''
                        , ''
                        , Usr_Login
                        , Ludatetime
                    FROM T_EmpTimeRegister
                    WHERE Ttr_IDNo = '{1}'
                        AND Ttr_Date = '{2}'
                END
                ELSE
                BEGIN
                    -- 1. UPDATE LEDGER TO DESIRED VALUES
                    UPDATE T_EmpTimeRegister SET Ttr_ShiftCode = '{0}'
                        , Usr_Login = '{3}'
                        , Ludatetime = GETDATE()
                    WHERE Ttr_IDNo = '{1}'
                        AND Ttr_Date = '{2}'

                    -- 2. INSERT UPDATED RECORDS TO TRAIL
                    INSERT INTO T_EmpTimeRegisterLog
                    (
                        Ttl_IDNo
                        , Ttl_Date
                        , Ttl_DayCode
                        , Ttl_RestDayFlag
                        , Ttl_HolidayFlag
                        , Ttl_ShiftCode
                        , Ttl_ActIn_1
                        , Ttl_ActOut_1
                        , Ttl_ActIn_2
                        , Ttl_ActOut_2
                        , Ttl_SkipService
                        , Ttl_AssumedFlag
                        , Ttl_Amnesty
                        , Ttl_Remarks
                        , Ttl_DocumentBatchNo
                        , Usr_Login
                        , Ludatetime
                    )
                    SELECT
                        Ttr_IDNo
                        , Ttr_Date
                        , Ttr_DayCode
                        , Ttr_RestDayFlag
                        , Ttr_HolidayFlag
                        , Ttr_ShiftCode
                        , Ttr_ActIn_1
                        , Ttr_ActOut_1
                        , Ttr_ActIn_2
                        , Ttr_ActOut_2
                        , Ttr_SkipService
                        , Ttr_AssumedFlag
                        , Ttr_Amnesty
                        , ''
                        , ''
                        , Usr_Login
                        , Ludatetime
                    FROM T_EmpTimeRegister
                    WHERE Ttr_IDNo = '{1}'
                        AND Ttr_Date = '{2}'
                END"
                    , ShiftCode
                    , EmployeeID
                    , ProcessDate
                    , LoginInfo.getUser().UserCode);

            dal.ExecuteDataSet(query);

        }

        public void UpdateDayCodesInLedger(string EmployeeID, string ProcessDate, string DayCode, bool IsRestDay, bool IsHoliday, DALHelper dal)
        {
            string query;

            query = string.Format(@"
                IF NOT EXISTS(SELECT Ttl_IDNo FROM T_EmpTimeRegisterLog 
                                WHERE Ttl_IDNo = '{1}'
                                    AND Ttl_Date = '{2}')  -- IF NO RECORD IN TRAIL YET
                BEGIN
                    -- 1. INSERT ORIGINAL RECORD
                    INSERT INTO T_EmpTimeRegisterLog
                    (
                        Ttl_IDNo
                        , Ttl_Date
                        , Ttl_DayCode
                        , Ttl_RestDayFlag
                        , Ttl_HolidayFlag
                        , Ttl_ShiftCode
                        , Ttl_ActIn_1
                        , Ttl_ActOut_1
                        , Ttl_ActIn_2
                        , Ttl_ActOut_2
                        , Ttl_SkipService
                        , Ttl_AssumedFlag
                        , Ttl_Amnesty
                        , Ttl_Remarks
                        , Ttl_DocumentBatchNo
                        , Usr_Login
                        , Ludatetime
                    )
                    SELECT
                        Ttr_IDNo
                        , Ttr_Date
                        , Ttr_DayCode
                        , Ttr_RestDayFlag
                        , Ttr_HolidayFlag
                        , Ttr_ShiftCode
                        , Ttr_ActIn_1
                        , Ttr_ActOut_1
                        , Ttr_ActIn_2
                        , Ttr_ActOut_2
                        , Ttr_SkipService
                        , Ttr_AssumedFlag
                        , Ttr_Amnesty
                        , ''
                        , ''
                        , Usr_Login
                        , Ludatetime
                    FROM T_EmpTimeRegister
                    WHERE Ttr_IDNo = '{1}'
                        AND Ttr_Date = '{2}'
                    
                    -- 2. UPDATE LEDGER TO DESIRED VALUES
                    UPDATE T_EmpTimeRegister SET Ttr_DayCode = '{0}', Ttr_RestDayFlag = '{3}'
                        , Ttr_HolidayFlag = '{4}'
                        , Usr_Login = '{5}'
                        , Ludatetime = GETDATE()
                    WHERE Ttr_IDNo = '{1}'
                        AND Ttr_Date = '{2}'

                    -- 3. INSERT UPDATED RECORDS TO TRAIL
                    INSERT INTO T_EmpTimeRegisterLog
                    (
                        Ttl_IDNo
                        , Ttl_Date
                        , Ttl_DayCode
                        , Ttl_RestDayFlag
                        , Ttl_HolidayFlag
                        , Ttl_ShiftCode
                        , Ttl_ActIn_1
                        , Ttl_ActOut_1
                        , Ttl_ActIn_2
                        , Ttl_ActOut_2
                        , Ttl_SkipService
                        , Ttl_AssumedFlag
                        , Ttl_Amnesty
                        , Ttl_Remarks
                        , Ttl_DocumentBatchNo
                        , Usr_Login
                        , Ludatetime
                    )
                    SELECT
                        Ttr_IDNo
                        , Ttr_Date
                        , Ttr_DayCode
                        , Ttr_RestDayFlag
                        , Ttr_HolidayFlag
                        , Ttr_ShiftCode
                        , Ttr_ActIn_1
                        , Ttr_ActOut_1
                        , Ttr_ActIn_2
                        , Ttr_ActOut_2
                        , Ttr_SkipService
                        , Ttr_AssumedFlag
                        , Ttr_Amnesty
                        , ''
                        , ''
                        , Usr_Login
                        , Ludatetime
                    FROM T_EmpTimeRegister
                    WHERE Ttr_IDNo = '{1}'
                        AND Ttr_Date = '{2}'
                END
                ELSE
                BEGIN
                    -- 1. UPDATE LEDGER TO DESIRED VALUES
                    UPDATE T_EmpTimeRegister SET Ttr_DayCode = '{0}', Ttr_RestDayFlag = '{3}'
                        , Ttr_HolidayFlag = '{4}'
                        , Usr_Login = '{5}'
                        , Ludatetime = GETDATE()
                    WHERE Ttr_IDNo = '{1}'
                        AND Ttr_Date = '{2}'

                    -- 2. INSERT UPDATED RECORDS TO TRAIL
                    INSERT INTO T_EmpTimeRegisterLog
                    (
                        Ttl_IDNo
                        , Ttl_Date
                        , Ttl_DayCode
                        , Ttl_RestDayFlag
                        , Ttl_HolidayFlag
                        , Ttl_ShiftCode
                        , Ttl_ActIn_1
                        , Ttl_ActOut_1
                        , Ttl_ActIn_2
                        , Ttl_ActOut_2
                        , Ttl_SkipService
                        , Ttl_AssumedFlag
                        , Ttl_Amnesty
                        , Ttl_Remarks
                        , Ttl_DocumentBatchNo
                        , Usr_Login
                        , Ludatetime
                    )
                    SELECT
                        Ttr_IDNo
                        , Ttr_Date
                        , Ttr_DayCode
                        , Ttr_RestDayFlag
                        , Ttr_HolidayFlag
                        , Ttr_ShiftCode
                        , Ttr_ActIn_1
                        , Ttr_ActOut_1
                        , Ttr_ActIn_2
                        , Ttr_ActOut_2
                        , Ttr_SkipService
                        , Ttr_AssumedFlag
                        , Ttr_Amnesty
                        , ''
                        , ''
                        , Usr_Login
                        , Ludatetime
                    FROM T_EmpTimeRegister
                    WHERE Ttr_IDNo = '{1}'
                        AND Ttr_Date = '{2}'
                END"
                    , DayCode
                    , EmployeeID
                    , ProcessDate
                    , IsRestDay
                    , IsHoliday
                    , LoginInfo.getUser().UserCode);

            dal.ExecuteDataSet(query);

        }

        public bool IsHoliday(string DayCode, string CompanyCode, string CentralProfile)
        {
            string query = string.Format(@"SELECT Mdy_HolidayFlag 
                                            FROM M_Day 
                                            WHERE Mdy_DayCode = @DayCode
                                            AND Mdy_CompanyCode = @CompanyCode");

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@CompanyCode", CompanyCode);
            paramInfo[1] = new ParameterInfo("@DayCode", DayCode);

            DataTable dtResult = new DataTable();
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();

                dtResult = dal.ExecuteDataSet(query, CommandType.Text, paramInfo).Tables[0];

                dal.CloseDB();
            }

            return Convert.ToBoolean(dtResult.Rows[0][0]);
        }

        public DataTable GetShifts(bool isHeaderOnly, string PayCycleCode)
        {
            string Query = string.Format(@"EXEC CROSSTAB2 
                            'SELECT Ttr_IDNo AS [ID Number]
                             , CONVERT(DATE, Ttr_Date) AS [Date]
                             , Ttr_ShiftCode AS [Shift]
                            FROM T_EmpTimeRegister
                            INNER JOIN M_Employee
                            ON Ttr_IDNo = Mem_IDNo
                            AND Ttr_PayCycle = ''{0}'''
                            , '[Date]'
                            , 'MAX([Shift])[]'
                            , '[ID Number]'
                            , ''
                            , '2, 3'
                            , 0
                            , 0", PayCycleCode);

            DataTable dtResult = new DataTable();
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                dtResult = dal.ExecuteDataSet(Query).Tables[0];

                dal.CloseDB();
            }

            if (isHeaderOnly)
                dtResult.Rows.Clear();
            return dtResult;
        }

        public DataTable GetDays(bool isHeaderOnly, string PayCycleCode)
        {
            string Query = string.Format(@"EXEC CROSSTAB2 
                            'SELECT Ttr_IDNo AS [ID Number]
                             , CONVERT(DATE, Ttr_Date)  AS [Date]
                             , Ttr_DayCode AS [Day]
                            FROM T_EmpTimeRegister
                            INNER JOIN M_Employee
                            ON Ttr_IDNo = Mem_IDNo
                            AND Ttr_PayCycle = ''{0}'''
                            , '[Date]'
                            , 'MAX([Day])[]'
                            , '[ID Number]'
                            , ''
                            , '2, 3'
                            , 0
                            , 0", PayCycleCode);

            DataTable dtResult = new DataTable();
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                dtResult = dal.ExecuteDataSet(Query).Tables[0];

                dal.CloseDB();
            }

            if (isHeaderOnly)
                dtResult.Rows.Clear();
            return dtResult;
        }

        public DataTable GetTimeCorrection(string CompanyCode, string CentralProfile)
        {
            #region query
            string query = string.Format(@"SELECT Ttm_IDNo AS [ID Number]
                                            , CONVERT(CHAR(10),Ttm_TimeCorDate,101) AS [Date]
                                            , Ttm_Classification AS [Classification]
                                            FROM T_EmpTimeCorrection
                                            WHERE Ttm_RequestType = 'A' "
                                            );
            #endregion
            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }
        //public bool TimeCorrectionDocumentNumberExist(string DocumentNumber)
        //{
        //    string query = string.Format(@"SELECT Ttm_DocumentNo 
        //                                   FROM Udv_TimeCorrection 
        //                                   WHERE Ttm_DocumentNo = '{0}'", DocumentNumber);

        //    DataTable dtResult = new DataTable();
        //    using (DALHelper dal = new DALHelper())
        //    {
        //        dal.OpenDB();
        //        dtResult = dal.ExecuteDataSet(query).Tables[0];
        //        dal.CloseDB();
        //    }

        //    return (dtResult.Rows.Count > 0) ? true : false;
        //}
    }
}
