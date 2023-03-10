using System;
using System.Text;
using System.Data;
using CommonPostingLibrary;
using Posting.DAL;
using System.Configuration;

namespace Posting.BLogic
{
    public class LogUploadingBL : BaseBL
    {
        public LogUploadingBL()
        {
        }

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

        #region <Functions Defined>

        public DataSet CheckIfCycleOpen()
        {
            DataSet ds = new DataSet();

            string sqlQuery = @"Select Tsc_SetFlag 
                                            From T_SettingControl 
                                            Where Tsc_SystemCode = 'PAYROLL' AND Tsc_SettingCode = 'CYCLEOPEN'";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text);

                dal.CloseDB();
            }
            return ds;
        }

        public DataSet GetAllRecInLogControl(string Lct_PayPeriod)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Lct_PayPeriod", Lct_PayPeriod);

            string sqlQuery = @"SELECT Tlc_Day01
                                              ,Tlc_Day02
                                              ,Tlc_Day03
                                              ,Tlc_Day04
                                              ,Tlc_Day05
                                              ,Tlc_Day06
                                              ,Tlc_Day07
                                              ,Tlc_Day08
                                              ,Tlc_Day09
                                              ,Tlc_Day10
                                              ,Tlc_Day11
                                              ,Tlc_Day12
                                              ,Tlc_Day13
                                              ,Tlc_Day14
                                              ,Tlc_Day15
                                              ,Tlc_Day16
                                          FROM T_EmpLogControl
                                        WHERE Tlc_YearMonth = @Lct_PayPeriod";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            return ds;
        }

        public DataSet RetrievePunches(string From, string To, DALHelper dal, DALHelper dalDTR)
        {
            DataSet ds = new DataSet();
            string DBName = (ConfigurationManager.AppSettings["DBNameDTR"].ToString());
            string sqlQuery = @"SELECT Tel_IDNo
                                    ,Tel_LogDate
                                    ,Tel_LogTime
                                    ,Tel_LogType
                                    ,Tel_StationNo
                                    ,Tel_IsPosted
                                    FROM {0}..{3} 
                                    Left Join T_EmpTimeRegister on Ttr_IDNo = Tel_IDNo
                                    and Convert(char(10),Ttr_Date,101) = Tel_LogDate
                                    Left Join M_Employee on Mem_IDNo = Tel_IDNo
                                    Left Join M_Shift on Msh_ShiftCode = Ttr_ShiftCode
                                    WHERE Tel_LogDate between '{1}'and '{2}'  
                                    and ( Ttr_IDNo is not null and  Msh_ShiftCode is not null)
                                    ORDER BY Tel_LogDate,Tel_IDNo, T_EmpDTR.LudateTime ASC";

            try
            {
                ds = dal.ExecuteDataSet(string.Format(sqlQuery, DBName, From, To, Globals.T_EmpDTR));
            }
            catch (Exception e)
            {
                //  throw new PayrollException("Error in retrieve punches record." + "\n" + "\n" + e.ToString());
            }
            return ds;
        }

        public DataSet RetrieveEmployeeLogLedgerRec(string Ttr_Date, DALHelper dal, DALHelper dalDTR)
        {
            DataSet ds = new DataSet();
            string DBName = (ConfigurationManager.AppSettings["DBNameDTR"].ToString());
            string sqlQuery = @"SELECT Ttr_IDNo
		                                            ,Ttr_Date
		                                            ,Ttr_PayCycle
		                                            ,Ttr_DayCode
		                                            ,Ttr_ShiftCode
		                                            ,Ttr_HolidayFlag
		                                            ,Ttr_RestDayFlag
		                                            ,Ttr_ActIn_1
		                                            ,Ttr_ActOut_1
		                                            ,Ttr_ActIn_2
		                                            ,Ttr_ActOut_2
                                            FROM T_EmpTimeRegister
                                            WHERE Ttr_Date = '{0}'
                                                    
                                            ORDER BY Ttr_IDNo,Ttr_Date ASC";

            try
            {
                ds = dal.ExecuteDataSet(string.Format(sqlQuery, Ttr_Date), CommandType.Text);
            }
            catch (Exception e)
            {
                // throw new PayrollException("Error in Retrieve employee log ledger record." + "\n" + "\n" + e.ToString());
            }

            return ds;
        }

        public string GetNewShiftCode(string Msh_ShiftCode)
        {
            DataSet ds = new DataSet();
            string retval = string.Empty;

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Msh_ShiftCode", Msh_ShiftCode);

            string sqlQuery = @"SELECT Msh_8HourShiftCode
	                                              ,Msh_ShiftCode
	                                              ,Msh_ShiftName
                                                  ,Msh_Schedule
                                                  ,Msh_ShiftIn1
                                                  ,Msh_ShiftOut1
                                                  ,Msh_ShiftIn2
                                                  ,Msh_ShiftOut2
                                                  ,Msh_ShiftHours
                                            FROM M_Shift
                                            WHERE Msh_ShiftCode = @Msh_ShiftCode";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            if (ds.Tables[0].Rows.Count > 0)
            {
                retval = ds.Tables[0].Rows[0]["Msh_8HourShiftCode"].ToString();
            }
            return retval;
        }

        public int UpdateShiftCode(string Usr_Login, string Ttr_Date, DALHelper dal)
        {
            int retVal = 0;
            int paramIndex = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[2];

            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", Usr_Login);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttr_Date", Ttr_Date);

            string sqlQuery = @"UPDATE  T_EmpTimeRegister
                                                SET Ttr_ShiftCode = Msh_8HourShiftCode,
                                                    Usr_Login = @Usr_Login,
                                                    Ludatetime = GetDate()
                                                FROM T_EmpTimeRegister
                                                INNER JOIN M_Shift on Msh_ShiftCode = Ttr_ShiftCode
                                                 AND LEN(RTRIM(Msh_8HourShiftCode)) > 0
                                                 AND Msh_ShiftHours <> 8
                                                WHERE Ttr_Date = @Ttr_Date
                                                 AND Ttr_DayCode <> 'REG'";

            try
            {
                retVal = dal.ExecuteNonQuery(sqlQuery, CommandType.Text, paramInfo);
            }
            catch (Exception e)
            {
                //throw new PayrollException(e);
            }

            return retVal;
        }

        public int UpdateLogControl(string Usr_Login, string Lct_PayPeriod, string Cont, DALHelper DalUp)
        {
            int retVal = 0;
            int paramIndex = 0;

            string qString = @"UPDATE  T_EmpLogControl ";
            if (Cont != string.Empty)
            {
                qString = qString + Cont;

                ParameterInfo[] paramInfo = new ParameterInfo[2];

                paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", Usr_Login);
                paramInfo[paramIndex++] = new ParameterInfo("@Lct_PayPeriod", Lct_PayPeriod);

                retVal = DalUp.ExecuteNonQuery(qString, CommandType.Text, paramInfo);
            }
            return retVal;
        }

        public int UpdateEmployeeLogLedger(DataRow row, DALHelper DalUp)
        {
            int retVal = 0;
            int paramIndex = 0;

            #region query
            string qString = @"UPDATE T_EmpTimeRegister 
                                SET Ttr_ActIn_1 = @Ttr_ActIn_1
		                            ,Ttr_ActOut_1 = @Ttr_ActOut_1
		                            ,Ttr_ActIn_2 = @Ttr_ActIn_2
		                            ,Ttr_ActOut_2 = @Ttr_ActOut_2
                                    ,Usr_Login = @Usr_Login
                                    ,Ludatetime = GetDate() 
                                WHERE RTRIM(Ttr_IDNo) = RTRIM(@Ttr_IDNo)
		                            AND Ttr_Date = @Ttr_Date";
            string sql = @"select convert(char(10),Tps_StartCycle,101) as start,convert(char(10),Tps_EndCycle,101) as endDate  
                                from T_PaySchedule where Tps_CycleIndicator='C' and Tps_RecordStatus='A'";
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[7];

            paramInfo[paramIndex++] = new ParameterInfo("@Ttr_ActIn_1", row["Ttr_ActIn_1"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttr_ActOut_1", row["Ttr_ActOut_1"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttr_ActIn_2", row["Ttr_ActIn_2"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttr_ActOut_2", row["Ttr_ActOut_2"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttr_IDNo", row["Ttr_IDNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Ttr_Date", row["Ttr_Date"]);

            retVal = DalUp.ExecuteNonQuery(qString, CommandType.Text, paramInfo);

            return retVal;
        }

        public DataSet GetShiftCodeData(string Msh_ShiftCode)
        {
            DataSet ds = new DataSet();
            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Msh_ShiftCode", Msh_ShiftCode);

            string sqlQuery = @"SELECT Msh_8HourShiftCode
	                                              ,Msh_ShiftCode
	                                              ,Msh_ShiftName
                                                  ,Msh_Schedule
                                                  ,Msh_ShiftIn1
                                                  ,Msh_ShiftOut1
                                                  ,Msh_ShiftIn2
                                                  ,Msh_ShiftOut2
                                                  ,Msh_ShiftHours
                                            FROM M_Shift
                                            WHERE Msh_ShiftCode = @Msh_ShiftCode";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            return ds;
        }

        public string GetEmployeeShiftCodeData(string Mem_IDNo)
        {
            DataSet ds = new DataSet();

            #region query
            string qString = @"Select Mem_ShiftCode
                                From M_Employee
                                Where Mem_IDNo = @Mem_IDNo";
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Mem_IDNo", Mem_IDNo);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0]["Mem_ShiftCode"].ToString().Trim();
            else
                return string.Empty;
        }

        public DataSet CheckIfEmpLedgerRecExists(string startdate, string enddate)
        {
            DataSet ds = new DataSet();
            CommonBL CmnBL = new CommonBL();
            string[] param = new string[3];
            param[0] = CmnBL.GetDTRDatabaseName();
            param[1] = startdate;
            param[2] = enddate;

            #region query

            string qString = string.Format(@"Select Distinct({0}..{3}.Tel_IDNo)
                                From {0}..{3} Left Join T_EmpTimeRegister 
	                                on Ttr_IDNo = Tel_IDNo
	                                and Convert(char(10),Ttr_Date,101) = Tel_LogDate
                                Where Tel_LogDate between '{1}' and '{2}'
                                And Ttr_IDNo is null", param, Globals.T_EmpDTR);

            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text);

                dal.CloseDB();
            }

            return ds;
        }

        public bool GetLCTVaue(string Lct_PayPeriod, string ColumnName)
        {
            DataSet ds = new DataSet();

            if (ColumnName.Trim() != string.Empty)
            {
                #region query
                string qString = @"Select " + ColumnName +
                @" From T_EmpLogControl
                                Where Tlc_YearMonth = @Lct_PayPeriod";
                #endregion

                ParameterInfo[] paramInfo = new ParameterInfo[1];
                paramInfo[0] = new ParameterInfo("@Lct_PayPeriod", Lct_PayPeriod);
                using (DALHelper dal = new DALHelper())
                {
                    dal.OpenDB();

                    ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

                    dal.CloseDB();
                }
            }
            if (ds.Tables[0].Rows.Count > 0)
                return Convert.ToBoolean(ds.Tables[0].Rows[0][0].ToString());
            else
                return false;
        }

        #endregion

        public DataSet GetAllEmployeeIDEmpMaster()
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"Select Distinct(Ttr_IDNo) as Mem_IDNo
	                                           ,Mem_LastName
                                               ,Mem_FirstName
                                               ,Mem_MiddleName
                                        From T_EmpTimeRegister Inner Join M_Employee
                                        on Ttr_IDNo = Mem_IDNo
                                        Where LEFT(Mem_WorkStatus,1) = 'A'";

            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text);

                dal.CloseDB();
            }
            return ds;
        }

        private string SQLShiftCodeChecking(string startdate, string enddate)
        {
            CommonBL CmnBL = new CommonBL();
            string[] param = new string[3];
            param[0] = CmnBL.GetDTRDatabaseName();
            param[1] = startdate;
            param[2] = enddate;

            #region 

            string qString = string.Format(@"Select Distinct({0}..{3}.Tel_IDNo) 
                                            From {0}..{3} Left Join T_EmpTimeRegister 
		                                            On Ttr_IDNo = Tel_IDNo
		                                            and Convert(char(10),Ttr_Date,101) = Tel_LogDate
	                                            Left Join M_Shift 
		                                            On Ttr_ShiftCode = Msh_ShiftCode
                                            Where Tel_LogDate between '{1}' and '{2}'
	                                            and  (LEN(RTRIM(Ttr_ShiftCode)) = 0
                                                Or Msh_RecordStatus = 'C' )
                                            Union
                                            Select Distinct({0}..{3}.Tel_IDNo)
                                            From {0}..{3} Left Join T_EmpTimeRegister 
                                                on Ttr_IDNo = Tel_IDNo
                                                and Convert(char(10),Ttr_Date,101) = Tel_LogDate
                                            Where Tel_LogDate between '{1}' and '{2}' 
                                            And Ttr_IDNo is null", param, Globals.T_EmpDTR);

            #endregion

            return qString;
        }

        public bool CheckIfShiftCodeErrorExists(string startdate, string enddate)
        {
            DataSet ds = new DataSet();
            string qString = this.SQLShiftCodeChecking(startdate, enddate);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text);

                dal.CloseDB();
            }
            if (ds.Tables[0].Rows.Count > 0)
                return true;
            else
                return false;
        }

        public bool CheckIfNullShiftCodeExists(string startdate, string enddate)
        {
            DataSet ds = new DataSet();
            CommonBL CmnBL = new CommonBL();
            string[] param = new string[3];
            param[0] = CmnBL.GetDTRDatabaseName();
            param[1] = startdate;
            param[2] = enddate;

            #region query

            string qString = string.Format(@"Select Distinct({0}..{3}.Tel_IDNo)
                                                        ,Mem_ShiftCode
                                            From {0}..{3} Left Join T_EmpTimeRegister 
	                                                on Ttr_IDNo = Tel_IDNo
	                                                and Convert(char(10),Ttr_Date,101) = Tel_LogDate
                                                Left Join M_Employee
	                                                on Mem_IDNo = Tel_IDNo
                                            Where Tel_LogDate between '{1}' and '{2}'
                                            And Ttr_IDNo is null
                                            And Mem_ShiftCode is null", param, Globals.T_EmpDTR);

            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text);
                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return true;
            else
                return false;
        }

        public int CheckConnection(string datasource, string DBName)
        {
            DataSet ds = new DataSet();
            int val = 0;

            #region query

            string query = @"Select dbid
                             From Master..Sysdatabases
                             Where Name = 'DTR'";

            #endregion

            using (DALHelper dal = new DALHelper(datasource, DBName))
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(query, CommandType.Text);
                    if (ds.Tables[0].Rows.Count > 0)
                        val = 1;
                }
                catch
                {
                    val = 0;
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return val;
        }

        public DataSet GetLogControlData()
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"Select  Tlc_Day01,Tlc_Day02,Tlc_Day03,Tlc_Day04
	                                   ,Tlc_Day05,Tlc_Day06,Tlc_Day07,Tlc_Day08
	                                   ,Tlc_Day09,Tlc_Day10,Tlc_Day11,Tlc_Day12
	                                   ,Tlc_Day13,Tlc_Day14,Tlc_Day15,Tlc_Day16
                                From T_EmpLogControl";

            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text);

                dal.CloseDB();
            }
            return ds;
        }

        #region <Added Function for T_DTRLedger Posting>

        public DataSet GetDTRRecord()
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"Select Distinct(Tel_IDNo)
	                                   ,Tel_LogDate
                                From T_EmpDTR
                                Where Tel_IsPosted = 'False'";

            #endregion

            using (DALHelper dal = new DALHelper(true))
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text);

                dal.CloseDB();
            }
            return ds;
        }

        public DataSet GetDTRRecordForPosting(DALHelper dal)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"Select Tel_IDNo
	                                   ,Tel_LogDate
	                                   ,Tel_LogTime
	                                   ,Tel_LogType
                                From T_EmpDTR
                                Where Tel_IsPosted = 'False'";

            #endregion


            try
            {
                ds = dal.ExecuteDataSet(qString, CommandType.Text);
            }
            catch (Exception e)
            {
                throw new PayrollException("Error in get DTR record for posting" + "\n" + "\n" + e.ToString());
            }
            return ds;
        }

        public string GetMinDateInDTR()
        {
            DataSet ds = new DataSet();

            using (DALHelper dal = new DALHelper(true))
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet("Select min(Tel_LogDate) From T_EmpDTR Where Tel_IsPosted = 'False'", CommandType.Text);

                dal.CloseDB();
            }
            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0][0].ToString();
            else
                return string.Empty;
        }

        public string GetMaxDateInDTR()
        {
            DataSet ds = new DataSet();

            using (DALHelper dal = new DALHelper(true))
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet("Select max(Tel_LogDate) From T_EmpDTR Where Tel_IsPosted = 'False'", CommandType.Text);

                dal.CloseDB();
            }
            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0][0].ToString();
            else
                return string.Empty;
        }

        public DataSet GetDTRLedgerRecord(string mindate, string maxdate, DALHelper dal)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"Select Dtr_EmployeeId
                                    ,Convert(Char(10),Dtr_ProcessDate,101) as Dtr_ProcessDate
                                    ,Dtr_In_1
                                    ,Dtr_Out_1
                                    ,Dtr_In_2
                                    ,Dtr_Out_2
                                    ,Dtr_In_3
                                    ,Dtr_Out_3
                                    ,Dtr_In_4
                                    ,Dtr_Out_4
                                    ,Dtr_In_5
                                    ,Dtr_Out_5
                                    From T_DTRLedger
                                    Where Dtr_ProcessDate between @mindate and @maxdate";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@mindate", mindate);
            paramInfo[1] = new ParameterInfo("@maxdate", maxdate);
            try
            {
                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);
            }
            catch (Exception e)
            {
                // throw new PayrollException("Error in get DTR Ledger record" + "\n" + "\n" + e.ToString());
            }
            return ds;
        }

        public bool CheckIfRecordExistsInDTRLedger(string Dtr_EmployeeId, string Dtr_ProcessDate, DALHelper dal)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"Select Dtr_EmployeeId
                                From T_DTRLedger
                                WHERE Dtr_EmployeeId = @Dtr_EmployeeId
                                And Dtr_ProcessDate = @Dtr_ProcessDate";
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Dtr_EmployeeId", Dtr_EmployeeId);
            paramInfo[1] = new ParameterInfo("@Dtr_ProcessDate", Dtr_ProcessDate);
            try
            {
                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);
            }
            catch (Exception e)
            {
                // throw new PayrollException("Error in Check if record exists in DTR Ledger" + "\n"+ "\n" + e.ToString());
            }

            if (ds.Tables[0].Rows.Count > 0)
                return true;
            else
                return false;
        }

        public int InsertDTRLedgerRecord(string Dtr_EmployeeId, string Dtr_ProcessDate, DALHelper dal)
        {
            int retVal = 0;

            #region query

            string qString = @"Insert Into T_DTRLedger
		                                    (Dtr_EmployeeId
		                                    ,Dtr_ProcessDate
		                                    ,Dtr_In_1
		                                    ,Dtr_Out_1
		                                    ,Dtr_In_2
		                                    ,Dtr_Out_2
		                                    ,Dtr_In_3
		                                    ,Dtr_Out_3
		                                    ,Dtr_In_4
		                                    ,Dtr_Out_4
		                                    ,Dtr_In_5
		                                    ,Dtr_Out_5
		                                    ,Usr_Login
		                                    ,Ludatetime)
                                    VALUES (@Dtr_EmployeeId
		                                    ,@Dtr_ProcessDate
		                                    ,null
		                                    ,null
		                                    ,null
		                                    ,null
		                                    ,null
		                                    ,null
		                                    ,null
		                                    ,null
		                                    ,null
		                                    ,null
		                                    ,@Usr_Login
		                                    ,Getdate())";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[3];
            paramInfo[0] = new ParameterInfo("@Dtr_EmployeeId", Dtr_EmployeeId);
            paramInfo[1] = new ParameterInfo("@Dtr_ProcessDate", Dtr_ProcessDate);
            paramInfo[2] = new ParameterInfo("@Usr_Login", "sa");//LoginInfo.getUser().UserCode);

            try
            {
                retVal = dal.ExecuteNonQuery(qString, CommandType.Text, paramInfo);
            }
            catch (Exception e)
            {
                //  throw new PayrollException("Error in insert DTR Ledger record" + "\n" + "\n" + e.ToString());
            }

            return retVal;
        }

        public int UpdateDTRLedgerRecord(DataRow row, DALHelper dal)
        {
            int retVal = 0;

            #region query

            string qString = @"UPDATE T_DTRLedger 
                                SET  Dtr_In_1 = @Dtr_In_1
	                                ,Dtr_Out_1 = @Dtr_Out_1
	                                ,Dtr_In_2 = @Dtr_In_2
	                                ,Dtr_Out_2 = @Dtr_Out_2
	                                ,Dtr_In_3 = @Dtr_In_3
	                                ,Dtr_Out_3 = @Dtr_Out_3
	                                ,Dtr_In_4 = @Dtr_In_4
	                                ,Dtr_Out_4 = @Dtr_Out_4
	                                ,Dtr_In_5 = @Dtr_In_5
	                                ,Dtr_Out_5 = @Dtr_Out_5
	                                ,Usr_Login = @Usr_Login
	                                ,Ludatetime = Getdate()
                                WHERE Dtr_EmployeeId = @Dtr_EmployeeId
                                And Dtr_ProcessDate = @Dtr_ProcessDate";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[13];
            paramInfo[0] = new ParameterInfo("@Dtr_EmployeeId", row["Dtr_EmployeeId"]);
            paramInfo[1] = new ParameterInfo("@Dtr_ProcessDate", row["Dtr_ProcessDate"]);
            paramInfo[2] = new ParameterInfo("@Dtr_In_1", row["Dtr_In_1"]);
            paramInfo[3] = new ParameterInfo("@Dtr_Out_1", row["Dtr_Out_1"]);
            paramInfo[4] = new ParameterInfo("@Dtr_In_2", row["Dtr_In_2"]);
            paramInfo[5] = new ParameterInfo("@Dtr_Out_2", row["Dtr_Out_2"]);
            paramInfo[6] = new ParameterInfo("@Dtr_In_3", row["Dtr_In_3"]);
            paramInfo[7] = new ParameterInfo("@Dtr_Out_3", row["Dtr_Out_3"]);
            paramInfo[8] = new ParameterInfo("@Dtr_In_4", row["Dtr_In_4"]);
            paramInfo[9] = new ParameterInfo("@Dtr_Out_4", row["Dtr_Out_4"]);
            paramInfo[10] = new ParameterInfo("@Dtr_In_5", row["Dtr_In_5"]);
            paramInfo[11] = new ParameterInfo("@Dtr_Out_5", row["Dtr_Out_5"]);
            paramInfo[12] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);
            try
            {
                retVal = dal.ExecuteNonQuery(qString, CommandType.Text, paramInfo);
            }
            catch (Exception e)
            {
                //  throw new PayrollException("Error in update DTR Ledger record." + "\n" + "\n" + e.ToString());
            }

            return retVal;
        }

        public int UpdateDTRPostFlag(DataRow row, DALHelper dalDTR)
        {
            int retVal = 0;

            #region query

            string qString = @"Update T_EmpDTR
                                Set Tel_IsPosted = 'True'
	                                , Usr_Login = @Usr_Login
	                                --, LudateTime = Getdate()
                                Where Tel_IDNo = @Tel_IDNo
                                And Tel_LogDate = @Tel_LogDate
                                And Tel_LogTime = @Tel_LogTime
                                And Tel_LogType = @Tel_LogType";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[5];
            paramInfo[0] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);
            paramInfo[1] = new ParameterInfo("@Tel_IDNo", row["Tel_IDNo"]);
            paramInfo[2] = new ParameterInfo("@Tel_LogDate", row["Tel_LogDate"]);
            paramInfo[3] = new ParameterInfo("@Tel_LogTime", row["Tel_LogTime"]);
            paramInfo[4] = new ParameterInfo("@Tel_LogType", row["Tel_LogType"]);

            try
            {
                retVal = dalDTR.ExecuteNonQuery(qString, CommandType.Text, paramInfo);
            }
            catch (Exception e)
            {
                //throw new PayrollException(e);
            }

            return retVal;
        }

        #endregion

        #region <For EmployeeLogLedger Saving>

        public int SaveEmpLogLedger(DataSet LedgerRecds, string Is_fieldname, string Is_posttype, DALHelper dal)
        {
            int retVal = 0;

            try
            {
                if (Is_posttype == "Full")
                    this.UpdateLogControl(Is_fieldname, dal);

                DataRow dr = DbRecord.Generate(CommonConstants.TableName.T_EmpTimeRegister);
                for (int i = 0; i < LedgerRecds.Tables[0].Rows.Count; i++)
                {
                    dr["Ttr_ActIn_1"] = LedgerRecds.Tables[0].Rows[i]["Ttr_ActIn_1"].ToString();
                    dr["Ttr_ActOut_1"] = LedgerRecds.Tables[0].Rows[i]["Ttr_ActOut_1"].ToString();
                    dr["Ttr_ActIn_2"] = LedgerRecds.Tables[0].Rows[i]["Ttr_ActIn_2"].ToString();
                    dr["Ttr_ActOut_2"] = LedgerRecds.Tables[0].Rows[i]["Ttr_ActOut_2"].ToString();
                    dr["Usr_Login"] = "service";
                    dr["Ttr_IDNo"] = LedgerRecds.Tables[0].Rows[i]["Ttr_IDNo"].ToString();
                    dr["Ttr_Date"] = LedgerRecds.Tables[0].Rows[i]["Ttr_Date"].ToString();

                    this.UpdateEmployeeLogLedger(dr, dal);
                }
            }
            catch (Exception e)
            {
                throw new PayrollException(e);
            }

            return retVal;
        }

        private void UpdateLogControl(string Is_fieldname, DALHelper DalUp)
        {
            string cont = "SET " + Is_fieldname + @" = 'True'
                                                ,Usr_Login = @Usr_Login
                                                ,Ludatetime = GetDate() 
                                    WHERE Tlc_YearMonth = @Lct_PayPeriod";

            string CurPayPeriod = this.GetCurrentPayPeriod(DalUp);
            this.UpdateLogControl("service", CurPayPeriod, cont, DalUp);
        }

        public string GetCurrentPayPeriod(DALHelper DalUp)
        {
            DataSet ds = new DataSet();

            string sqlQuery = @"Select Tps_PayCycle From T_PaySchedule
                                                Where Tps_CycleIndicator = 'C'
                                                And Tps_RecordStatus = 'A'";

            ds = DalUp.ExecuteDataSet(sqlQuery, CommandType.Text);

            return ds.Tables[0].Rows[0][0].ToString();
        }

        #endregion

        #region <Added Function For Manual Logs Uploading>

        public bool CheckIfRecordAlreadyExists(DataRow row)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"Select * From T_EmpDTR 
                                Where Tel_IDNo = @Tel_IDNo
                                      And Tel_LogDate = @Tel_LogDate
                                      And Tel_LogTime = @Tel_LogTime
                                      And Tel_LogType = @Tel_LogType";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[4];
            paramInfo[0] = new ParameterInfo("@Tel_IDNo", row["Tel_IDNo"]);
            paramInfo[1] = new ParameterInfo("@Tel_LogDate", row["Tel_LogDate"]);
            paramInfo[2] = new ParameterInfo("@Tel_LogTime", row["Tel_LogTime"]);
            paramInfo[3] = new ParameterInfo("@Tel_LogType", row["Tel_LogType"]);
            using (DALHelper dal = new DALHelper(true))
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

        public DataSet GetDataFromLocalDB(string datasource, string DBName)
        {
            DataSet ds = new DataSet();

            using (DALHelper dal = new DALHelper(datasource, DBName))
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet("Select * From T_EmpDTR", CommandType.Text);
                dal.CloseDB();
            }

            return ds;
        }

        public int DeleteDataInLocalDB(string datasource, string DBName, DataRow row)
        {
            int retVal = 0;

            #region query

            string qString = @"Delete From T_EmpDTR 
                                Where Tel_IDNo = @Tel_IDNo
                                And Tel_LogDate = @Tel_LogDate
                                And Tel_LogTime = @Tel_LogTime
                                And Tel_LogType = @Tel_LogType";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[4];
            paramInfo[0] = new ParameterInfo("@Tel_IDNo", row["Tel_IDNo"]);
            paramInfo[1] = new ParameterInfo("@Tel_LogDate", row["Tel_LogDate"]);
            paramInfo[2] = new ParameterInfo("@Tel_LogTime", row["Tel_LogTime"]);
            paramInfo[3] = new ParameterInfo("@Tel_LogType", row["Tel_LogType"]);

            using (DALHelper dal = new DALHelper(datasource, DBName))
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
                    //   throw new PayrollException(e);
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return retVal;
        }

        public DataSet FetchAllActiveStations()
        {
            DataSet ds = new DataSet();

            using (DALHelper dal = new DALHelper(true))
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(@"Select Mst_StationName
                                            From M_Station
                                            Where Mst_RecordStatus = 'A'", CommandType.Text);
                dal.CloseDB();
            }

            return ds;
        }

        public int DBAddToServer(DataRow row)
        {
            int retVal = 0;
            int paramIndex = 0;

            #region query

            string qString = @"INSERT INTO T_EmpDTR
                                     (Tel_IDNo,
                                      Tel_LogDate,
                                      Tel_LogTime,
                                      Tel_LogType,
                                      Tel_StationNo,
                                      Tel_IsPosted,
                                      Usr_Login,
                                      LudateTime)
                                 VALUES
                                     (@Tel_IDNo,
                                      @Tel_LogDate,
                                      @Tel_LogTime,
                                      @Tel_LogType,
                                      @Tel_StationNo,
                                      @Tel_IsPosted,
                                      @Usr_Login,
                                      @LudateTime)";

            #endregion

            #region parameters
            ParameterInfo[] paramInfo = new ParameterInfo[8];
            paramInfo[paramIndex++] = new ParameterInfo("@Tel_IDNo", row["Tel_IDNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tel_LogDate", row["Tel_LogDate"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tel_LogTime", row["Tel_LogTime"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tel_LogType", row["Tel_LogType"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tel_StationNo", row["Tel_StationNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tel_IsPosted", row["Tel_IsPosted"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);
            paramInfo[paramIndex++] = new ParameterInfo("@LudateTime", row["LudateTime"]);
            #endregion

            using (DALHelper dal = new DALHelper(true))
            {
                dal.OpenDB();
                //dal.BeginTransaction();

                try
                {
                    retVal = dal.ExecuteNonQuery(qString, CommandType.Text, paramInfo);
                    //dal.CommitTransaction();
                }
                catch (Exception e)
                {
                    //dal.RollBackTransaction();
                    // throw new PayrollException(e);
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return retVal;
        }

        public bool CheckIfDataFileAlreadyExists(DataRow row, DALHelper dalDTR)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"Select * From T_EmpDTR 
                                Where Tel_IDNo = @Tel_IDNo
                                      And Tel_LogDate = @Tel_LogDate
                                      And Tel_LogTime = @Tel_LogTime
                                      And Tel_LogType = @Tel_LogType";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[4];
            paramInfo[0] = new ParameterInfo("@Tel_IDNo", row["Column_0"]);
            paramInfo[1] = new ParameterInfo("@Tel_LogDate", row["Column_1"]);
            paramInfo[2] = new ParameterInfo("@Tel_LogTime", row["Column_2"]);
            paramInfo[3] = new ParameterInfo("@Tel_LogType", row["Column_3"]);
            try
            {
                ds = dalDTR.ExecuteDataSet(qString, CommandType.Text, paramInfo);
            }
            catch (Exception e)
            {
                // throw new PayrollException("Error in DTR checking record to upload!" + "\n" + "\n" + e.ToString());
            }

            if (ds.Tables[0].Rows.Count > 0)
                return true;
            else
                return false;
        }

        public int DBAddTextFileToServer(DataRow row, DALHelper dalDTR)
        {
            int retVal = 0;
            int paramIndex = 0;

            #region query

            string qString = @"INSERT INTO T_EmpDTR
                                     (Tel_IDNo,
                                      Tel_LogDate,
                                      Tel_LogTime,
                                      Tel_LogType,
                                      Tel_StationNo,
                                      Tel_IsPosted,
                                      Usr_Login,
                                      LudateTime)
                                 VALUES
                                     (@Tel_IDNo,
                                      @Tel_LogDate,
                                      @Tel_LogTime,
                                      @Tel_LogType,
                                      ' ',
                                      'false',
                                      @Usr_Login,
                                      Getdate())";

            #endregion

            #region parameters

            ParameterInfo[] paramInfo = new ParameterInfo[5];

            paramInfo[paramIndex++] = new ParameterInfo("@Tel_IDNo", row["Tel_IDNo"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tel_LogDate", row["Tel_LogDate"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tel_LogTime", row["Tel_LogTime"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tel_LogType", row["Tel_LogType"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", LoginInfo.getUser().UserCode);

            #endregion

            try
            {
                retVal = dalDTR.ExecuteNonQuery(qString, CommandType.Text, paramInfo);
            }
            catch (Exception e)
            {
                //  throw new PayrollException("Error in Text file upon insert." + "\n" + "\n" + e);
            }

            return retVal;
        }

        #endregion

        public DataTable GetServiceCode(string columname)
        {
            string CentralProfile = Encrypt.decryptText(ConfigurationManager.AppSettings["CentralDBName"].ToString());
            StringBuilder sb = new StringBuilder();
            DataSet ds;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                try
                {
                    dal.OpenDB();
                    //dal.BeginTransaction();

                    sb.AppendFormat(@"SELECT DISTINCT Mss_ServiceCode 
                                    FROM M_ServiceSchedule 
                                    INNER JOIN M_Profile
                                    ON Mss_CompanyCode = Mpf_CompanyCode
                                    WHERE {0} = 1 
                                        AND Mss_RecordStatus = 'A' 
                                        AND Mpf_CentralProfile = '{1}'
                                        AND Mpf_ProfileType IN ('P','S')
                                        AND Mpf_ProfileCategory = '{2}'"
                                    , columname
                                    , CentralProfile
                                    , ConfigurationManager.AppSettings["ProfileCategory"].ToString());
                    ds = dal.ExecuteDataSet(sb.ToString());

                    //dal.CommitTransaction();
                }
                catch (Exception ex)
                {
                    //dal.RollBackTransaction();
                    throw ex;
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            if (ds.Tables[0].Rows != null)
                return ds.Tables[0];
            else
                return null;
        }
    }
}
