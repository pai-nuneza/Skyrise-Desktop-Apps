using System;
using System.Collections.Generic;
using System.Text;

using CommonLibrary;
using Payroll.DAL;
using System.Data;

namespace Payroll.BLogic
{
    public class RestDayUpdatingBL : BaseBL
    {
        public override int Add(System.Data.DataRow row)
        {
            int retVal = 0;
            int paramIndex = 0;

            #region query

            string qString = @"INSERT INTO T_EmpRest
                                           (Ter_IDNo
                                           ,Ter_StartDate
                                           ,Ter_RestDayIndic
                                           ,Usr_Login
                                           ,Ludatetime)
                                     VALUES
                                           (@Ter_IDNo
                                           ,@Ter_StartDate
                                           ,@Ter_RestDayIndic
                                           ,@Usr_Login
                                           ,GetDate())";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[4];
            paramInfo[paramIndex++] = new ParameterInfo("@Ter_IDNo", row["Ter_IDNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ter_StartDate", row["Ter_StartDate"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ter_RestDayIndic", row["Ter_RestDayIndic"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);
            
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    retVal = dal.ExecuteNonQuery(qString, CommandType.Text, paramInfo);

                    SystemCycleProcessingBL SystemCycleProcessingBL = new SystemCycleProcessingBL(dal, "");
                    DataTable dtEmployee = SystemCycleProcessingBL.GetActiveEmployeeList(row["Ter_IDNo"].ToString());
                    SystemCycleProcessingBL.CorrectLogLedgerRecord(false, true, true, dtEmployee, ""
                                                                    , new CommonBL().GetCurrentPayPeriod(), "", ""
                                                                    , false, false, true, true, false, false, true
                                                                    , false, false, false, false, false, false, row["Usr_Login"].ToString());
                    SystemCycleProcessingBL.CorrectLogLedgerRecord(false, true, true, dtEmployee, ""
                                                                    , new CommonBL().GetNextPayPeriod(), "", ""
                                                                    , false, false, true, true, false, false, true
                                                                    , false, false, false, false, false, false, row["Usr_Login"].ToString()); 

                    dal.CommitTransactionSnapshot();
                }
                catch (Exception e)
                {
                    dal.RollBackTransactionSnapshot();
                    int messageCode = System.Runtime.InteropServices.Marshal.GetHRForException(e);
                    CommonProcedures.ShowMessage(messageCode, "");
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return retVal;
        }

        public override int Update(System.Data.DataRow row)
        {
            CommonProcedures.ShowMessage(10113, ""); return -1;
        }

        public override int Delete(string Ter_IDNo, string Ter_StartDate)
        {
            int retVal = 0;

            #region query

            string qString = @"Delete From T_EmpRest
                                Where Ter_IDNo = @Ter_IDNo
                                And Ter_StartDate = @Ter_StartDate";
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Ter_IDNo", Ter_IDNo);
            paramInfo[1] = new ParameterInfo("@Ter_StartDate", Ter_StartDate);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    retVal = dal.ExecuteNonQuery(qString, CommandType.Text, paramInfo);
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

        public int Add(string Ter_IDNo, string Ter_StartDate, string RestDayIndic, string UserLogin)
        {
            int retVal = 0;
            int paramIndex = 0;

            #region query

            string qString = @"INSERT INTO T_EmpRest
                                           (Ter_IDNo
                                           ,Ter_StartDate
                                           ,Ter_RestDayIndic
                                           ,Usr_Login
                                           ,Ludatetime)
                                     VALUES
                                           (@Ter_IDNo
                                           ,@Ter_StartDate
                                           ,@Ter_RestDayIndic
                                           ,@Usr_Login
                                           ,GetDate())";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[4];
            paramInfo[paramIndex++] = new ParameterInfo("@Ter_IDNo", Ter_IDNo);
            paramInfo[paramIndex++] = new ParameterInfo("@Ter_StartDate", Ter_StartDate);
            paramInfo[paramIndex++] = new ParameterInfo("@Ter_RestDayIndic", RestDayIndic);
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", UserLogin);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    retVal = dal.ExecuteNonQuery(qString, CommandType.Text, paramInfo);

                    SystemCycleProcessingBL SystemCycleProcessingBL = new SystemCycleProcessingBL(dal, "");
                    DataTable dtEmployee = SystemCycleProcessingBL.GetActiveEmployeeList(Ter_IDNo);
                    SystemCycleProcessingBL.CorrectLogLedgerRecord(false, true, true, dtEmployee, ""
                                                                    , new CommonBL().GetCurrentPayPeriod(), "", ""
                                                                    , false, false, true, true, false, false, true
                                                                    , false, false, false, false, false, false, UserLogin);
                    SystemCycleProcessingBL.CorrectLogLedgerRecord(false, true, true, dtEmployee, ""
                                                                    , new CommonBL().GetNextPayPeriod(), "", ""
                                                                    , false, false, true, true, false, false, true
                                                                    , false, false, false, false, false, false, UserLogin);

                    dal.CommitTransactionSnapshot();
                }
                catch (Exception e)
                {
                    dal.RollBackTransactionSnapshot();
                    int messageCode = System.Runtime.InteropServices.Marshal.GetHRForException(e);
                    CommonProcedures.ShowMessage(messageCode, "");
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return retVal;
        }

        public int Delete(string Ter_IDNo, string Ter_StartDate, string UserLogin)
        {
            int retVal = 0;

            #region query

            string qString = @"Delete From T_EmpRest
                                Where Ter_IDNo = @Ter_IDNo
                                And Ter_StartDate = @Ter_StartDate";
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Ter_IDNo", Ter_IDNo);
            paramInfo[1] = new ParameterInfo("@Ter_StartDate", Ter_StartDate);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    retVal = dal.ExecuteNonQuery(qString, CommandType.Text, paramInfo);

                    SystemCycleProcessingBL SystemCycleProcessingBL = new SystemCycleProcessingBL(dal, "");
                    DataTable dtEmployee = SystemCycleProcessingBL.GetActiveEmployeeList(Ter_IDNo);
                    SystemCycleProcessingBL.CorrectLogLedgerRecord(false, true, true, dtEmployee, ""
                                                                    , new CommonBL().GetCurrentPayPeriod(), "", ""
                                                                    , false, false, true, true, false, false, true
                                                                    , false, false, false, false, false, false, UserLogin);
                    SystemCycleProcessingBL.CorrectLogLedgerRecord(false, true, true, dtEmployee, ""
                                                                    , new CommonBL().GetNextPayPeriod(), "", ""
                                                                    , false, false, true, true, false, false, true
                                                                    , false, false, false, false, false, false, UserLogin);

                    dal.CommitTransactionSnapshot();
                }
                catch (Exception e)
                {
                    dal.RollBackTransactionSnapshot();
                    int messageCode = System.Runtime.InteropServices.Marshal.GetHRForException(e);
                    CommonProcedures.ShowMessage(messageCode, "");
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return retVal;
        }

        public override System.Data.DataSet FetchAll()
        {
            CommonProcedures.ShowMessage(10113, ""); return null;
        }

        public override System.Data.DataRow Fetch(string code)
        {
            CommonProcedures.ShowMessage(10113, ""); return null;
        }

        public DataSet FetchRecord(string Ter_IDNo)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"Select    CONVERT(char(10), Ter_StartDate, 101) as Ter_StartDate
		                                ,Cast(Substring(Ter_RestDayIndic,1,1) as bit) as Mon
		                                ,Cast(Substring(Ter_RestDayIndic,2,1) as bit) as Tue
		                                ,Cast(Substring(Ter_RestDayIndic,3,1) as bit) as Wed
		                                ,Cast(Substring(Ter_RestDayIndic,4,1) as bit) as Thurs
		                                ,Cast(Substring(Ter_RestDayIndic,5,1) as bit) as Fri
		                                ,Cast(Substring(Ter_RestDayIndic,6,1) as bit) as Sat
		                                ,Cast(Substring(Ter_RestDayIndic,7,1) as bit) as Sun
                                From T_EmpRest
                                Where Ter_IDNo = @Ter_IDNo
                                ORDER BY Ter_StartDate DESC";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Ter_IDNo", Ter_IDNo);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            return ds;
        }

        public DataSet FetchRecord2(string Ter_IDNo)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"Select    CONVERT(char(10), Ter_StartDate, 101) as Ter_StartDate
		                                ,Cast(Substring(Ter_RestDayIndic,1,1) as bit) as Mon
		                                ,Cast(Substring(Ter_RestDayIndic,2,1) as bit) as Tue
		                                ,Cast(Substring(Ter_RestDayIndic,3,1) as bit) as Wed
		                                ,Cast(Substring(Ter_RestDayIndic,4,1) as bit) as Thurs
		                                ,Cast(Substring(Ter_RestDayIndic,5,1) as bit) as Fri
		                                ,Cast(Substring(Ter_RestDayIndic,6,1) as bit) as Sat
		                                ,Cast(Substring(Ter_RestDayIndic,7,1) as bit) as Sun
                                From T_EmpRest
                                Where Ter_IDNo = @Ter_IDNo
                                AND Ter_StartDate = (SELECT MAX(RestDay.Ter_StartDate)
							                                            FROM T_EmpRest RestDay
							                                            WHERE Ter_IDNo = @Ter_IDNo)
                                ORDER BY Ter_StartDate DESC";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Ter_IDNo", Ter_IDNo);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            return ds;
        }

        public bool CheckIfRecExists(string Ter_IDNo, string Ter_StartDate)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"Select  *
                                From T_EmpRest
                                Where Ter_IDNo = @Ter_IDNo
                                And Ter_StartDate = @Ter_StartDate";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Ter_IDNo", Ter_IDNo);
            paramInfo[1] = new ParameterInfo("@Ter_StartDate", Ter_StartDate);

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

        public int UpdateREGEmployeeLogLedgerRecord(DataRow row)
        {
            int retVal = 0;
            #region query
            string qString = @"UPDATE T_EmpTimeRegister
                               SET Ttr_DayCode = @Ttr_DayCode
                                  ,Ttr_RestDayFlag = @Ttr_RestDayFlag
                                  ,Usr_Login = @Usr_Login
                                  ,Ludatetime = Getdate()
                             WHERE Ttr_IDNo = @Ttr_IDNo
                              And Ttr_Date = @Ttr_Date";
            #endregion
            ParameterInfo[] paramInfo = new ParameterInfo[5];
            paramInfo[0] = new ParameterInfo("@Ttr_DayCode", row["Ttr_DayCode"]);
            paramInfo[1] = new ParameterInfo("@Ttr_RestDayFlag", row["Ttr_RestDayFlag"]);
            paramInfo[2] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);
            paramInfo[3] = new ParameterInfo("@Ttr_IDNo", row["Ttr_IDNo"]);
            paramInfo[4] = new ParameterInfo("@Ttr_Date", row["Ttr_Date"]);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    retVal = dal.ExecuteNonQuery(qString, CommandType.Text, paramInfo);
                    dal.CommitTransactionSnapshot();
                }
                catch (Exception e)
                {
                    dal.RollBackTransactionSnapshot();
                    int messageCode = System.Runtime.InteropServices.Marshal.GetHRForException(e);
                    CommonProcedures.ShowMessage(messageCode, "");
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return retVal;
        }

        public DataSet FetchHolidayRecord(string startDate, string endDate)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"Select CONVERT(char(10), Thl_HolidayDate, 101) as HolidayDate
                                        ,Thl_HolidayCode
                                From T_Holiday
                                Where Thl_HolidayDate between @startDate and @endDate";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@startDate", startDate);
            paramInfo[1] = new ParameterInfo("@endDate", endDate);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            return ds;
        }

        public int UpdateHOLEmployeeLogLedgerRecord(DataRow row)
        {
            int retVal = 0;
            #region query
            string qString = @"UPDATE T_EmpTimeRegister
                               SET Ttr_DayCode = @Ttr_DayCode
                                  ,Ttr_HolidayFlag = @Ttr_HolidayFlag
                                  ,Usr_Login = @Usr_Login
                                  ,Ludatetime = Getdate()
                             WHERE Ttr_IDNo = @Ttr_IDNo
                              And Ttr_Date = @Ttr_Date";
            #endregion
            ParameterInfo[] paramInfo = new ParameterInfo[5];
            paramInfo[0] = new ParameterInfo("@Ttr_DayCode", row["Ttr_DayCode"]);
            paramInfo[1] = new ParameterInfo("@Ttr_HolidayFlag", row["Ttr_HolidayFlag"]);
            paramInfo[2] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);
            paramInfo[3] = new ParameterInfo("@Ttr_IDNo", row["Ttr_IDNo"]);
            paramInfo[4] = new ParameterInfo("@Ttr_Date", row["Ttr_Date"]);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    retVal = dal.ExecuteNonQuery(qString, CommandType.Text, paramInfo);
                    dal.CommitTransactionSnapshot();
                }
                catch (Exception e)
                {
                    dal.RollBackTransactionSnapshot();
                    int messageCode = System.Runtime.InteropServices.Marshal.GetHRForException(e);
                    CommonProcedures.ShowMessage(messageCode, "");
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return retVal;
        }

        public string GetPrevEmployeeRestDayData(string Ter_IDNo, string Ter_StartDate)
        {
            DataSet ds = new DataSet();

            #region query
            string qString = @"Select Convert(char(10), Max(Ter_StartDate), 101) as Ter_StartDate
                                From T_EmpRest
                                Where Ter_StartDate < @Ter_StartDate
                                And Ter_IDNo = @Ter_IDNo";
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Ter_IDNo", Ter_IDNo);
            paramInfo[1] = new ParameterInfo("@Ter_StartDate", Ter_StartDate);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0]["Ter_StartDate"].ToString().Trim();
            else
                return string.Empty;
        }

        public DataTable GetPrevEmployee(string EmployeeId)
        {
            string EmployeeCondition = "";
            if (EmployeeId.Length > 0)
                EmployeeCondition = " and Mem_IDNo < '" + EmployeeId + "'";

            string query = string.Format(@" select top(1) Mem_IDNo, Mem_LastName, Mem_FirstName, left(Mem_MiddleName,1) as Mem_MiddleName
                                            ,Mem_CostcenterCode as [CostCenterCode]
                                            from M_Employee
                            LEFT JOIN M_CostCenter on   Mcc_CostCenterCode = Mem_CostcenterCode
                            LEFT JOIN M_Division on Mdv_DivCode= Mcc_DivCode 
                            LEFT JOIN M_Department on Mdp_DptCode = Mcc_DptCode
                            LEFT JOIN M_Section on  Msc_SecCode = Mcc_SecCode
                            LEFT JOIN M_SubSection  on Msb_SubSecCode = Mcc_SubsecCode 
                            LEFT JOIN M_Process on Mpr_PrcCode = Mcc_PrcCode
                            WHERE  Mem_CostcenterCode IN  ( SELECT Mcc_CostCenterCode 
                                                            FROM M_CostCenter
                                    WHERE Mcc_RecordStatus = 'A' and LEFT(Mem_WorkStatus,1) = 'A' {0})
                            order by Mem_IDNo desc", EmployeeCondition);

            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }

            return dtResult;
        }

        public DataTable GetNextEmployee(string EmployeeId)
        {
            string EmployeeCondition = "";
            if (EmployeeId.Length > 0)
                EmployeeCondition = " and Mem_IDNo > '" + EmployeeId + "'";

            string query = string.Format(@" select top(1) Mem_IDNo, 
                            Mem_LastName, 
                            Mem_FirstName, 
                            left(Mem_MiddleName,1) as Mem_MiddleName
                            ,Mem_CostcenterCode as [CostCenterCode]
                            from M_Employee
                            LEFT JOIN M_CostCenter on   Mcc_CostCenterCode = Mem_CostcenterCode
                            LEFT JOIN M_Division on Mdv_DivCode= Mcc_DivCode 
                            LEFT JOIN M_Department on Mdp_DptCode = Mcc_DptCode
                            LEFT JOIN M_Section on  Msc_SecCode = Mcc_SecCode
                            LEFT JOIN M_SubSection  on Msb_SubSecCode = Mcc_SubsecCode 
                            LEFT JOIN M_Process on Mpr_PrcCode = Mcc_PrcCode
                            WHERE  Mem_CostcenterCode IN  ( SELECT Mcc_CostCenterCode 
                                                        FROM M_CostCenter
                                                        WHERE Mcc_RecordStatus = 'A' and LEFT(Mem_WorkStatus,1) = 'A' {0})", EmployeeCondition);
            DataTable dtResult;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }

            return dtResult;
        }
    }
}
