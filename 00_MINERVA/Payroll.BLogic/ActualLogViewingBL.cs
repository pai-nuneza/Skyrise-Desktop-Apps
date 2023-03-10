using System;
using System.Data;
using Payroll.DAL;

namespace Payroll.BLogic
{
    public class ActualLogViewingBL : BaseBL
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

        public DataSet FetchRecord(string Dtr_EmployeeId, string startdate, string enddate)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = string.Format(@"Select	Dtr_EmployeeId
		                                ,Convert(char(10),Dtr_ProcessDate,101) as Dtr_ProcessDate
		                                ,SUBSTRING(DATENAME(weekday, Dtr_ProcessDate), 1, 3) as DOW
		                                ,CASE Dtr_In_1
			                                WHEN '0000'
			                                THEN ''
			                                ELSE SUBSTRING(Dtr_In_1,1,2) + ':' + SUBSTRING(Dtr_In_1,3,2) 
		                                 END as Dtr_In_1
		                                ,CASE Dtr_Out_1
			                                WHEN '0000'
			                                THEN ''
			                                ELSE SUBSTRING(Dtr_Out_1,1,2) + ':' + SUBSTRING(Dtr_Out_1,3,2) 
		                                 END as Dtr_Out_1
		                                ,CASE Dtr_In_2
			                                WHEN '0000'
			                                THEN ''
			                                ELSE SUBSTRING(Dtr_In_2,1,2) + ':' + SUBSTRING(Dtr_In_2,3,2) 
		                                 END as Dtr_In_2
		                                ,CASE Dtr_Out_2
			                                WHEN '0000'
			                                THEN ''
			                                ELSE SUBSTRING(Dtr_Out_2,1,2) + ':' + SUBSTRING(Dtr_Out_2,3,2) 
		                                 END as Dtr_Out_2
		                                ,CASE Dtr_In_3
			                                WHEN '0000'
			                                THEN ''
			                                ELSE SUBSTRING(Dtr_In_3,1,2) + ':' + SUBSTRING(Dtr_In_3,3,2) 
		                                 END as Dtr_In_3
		                                ,CASE Dtr_Out_3
			                                WHEN '0000'
			                                THEN ''
			                                ELSE SUBSTRING(Dtr_Out_3,1,2) + ':' + SUBSTRING(Dtr_Out_3,3,2) 
		                                 END as Dtr_Out_3
		                                ,CASE Dtr_In_4
			                                WHEN '0000'
			                                THEN ''
			                                ELSE SUBSTRING(Dtr_In_4,1,2) + ':' + SUBSTRING(Dtr_In_4,3,2) 
		                                 END as Dtr_In_4
		                                ,CASE Dtr_Out_4
			                                WHEN '0000'
			                                THEN ''
			                                ELSE SUBSTRING(Dtr_Out_4,1,2) + ':' + SUBSTRING(Dtr_Out_4,3,2) 
		                                 END as Dtr_Out_4
		                                ,CASE Dtr_In_5
			                                WHEN '0000'
			                                THEN ''
			                                ELSE SUBSTRING(Dtr_In_5,1,2) + ':' + SUBSTRING(Dtr_In_5,3,2) 
		                                 END as Dtr_In_5
		                                ,CASE Dtr_Out_5
			                                WHEN '0000'
			                                THEN ''
			                                ELSE SUBSTRING(Dtr_Out_5,1,2) + ':' + SUBSTRING(Dtr_Out_5,3,2) 
		                                 END as Dtr_Out_5
                                From T_DTRLedger
                                Where Dtr_EmployeeId = '{0}'
                                And Dtr_ProcessDate between '{1}' and '{2}'", Dtr_EmployeeId, startdate, enddate);

            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString);

                dal.CloseDB();
            }
            return ds;
        }

        public DataSet FetchRecordEx(string Dtr_EmployeeId, object startdate, object enddate, string companyCode)
        {
            DataSet ds = new DataSet();
            string qString = string.Format(@"
                                    DECLARE @StartDate DATE = '{1}'
                                    DECLARE @EndDate DATE = '{2}'

                                    SELECT	Tel_IDNo
                                        ,Tel_LogDate as [LogDate]
                                        ,SUBSTRING(DATENAME(weekday, Tel_LogDate), 1, 3) as DOW
                                        ,CASE Tel_LogTime
                                            WHEN '0000'
                                            THEN '24:00'
                                            ELSE SUBSTRING(Tel_LogTime,1,2) + ':' + SUBSTRING(Tel_LogTime,3,2) 
                                         END as LogTime
                                        ,Tel_LogType as LogType
                                        ,CASE WHEN Tel_IsPosted = 1 THEN 'YES' ELSE 'NO' END as Posted
                                        ,Mst_StationName as StationName
                                    FROM T_EmpDTR
                                    LEFT JOIN M_Station on T_EmpDTR.Tel_StationNo = M_Station.Mst_StationNo
                                        AND M_Station.Mst_CompanyCode = '{3}'
                                    WHERE Tel_IDNo = '{0}'
	                                    AND CONVERT(date,Tel_LogDate) BETWEEN @StartDate AND @EndDate
                                    ORDER BY Convert(date, Tel_LogDate)
                                    , CASE Tel_LogTime
                                                WHEN '0000'
                                                THEN '24:00'
                                                ELSE SUBSTRING(Tel_LogTime,1,2) + ':' + SUBSTRING(Tel_LogTime,3,2) 
                                    END", Dtr_EmployeeId, Convert.ToDateTime(startdate), Convert.ToDateTime(enddate).AddDays(1), companyCode);

            using (DALHelper dal = new DALHelper(true))
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString);

                dal.CloseDB();
            }
            return ds;
        }

        public DataSet FetchRecordEx2(string Dtr_EmployeeId, object startdate, object enddate, string CentralProfile, string CompanyCode)
        {
            DataSet ds = new DataSet();
            CommonBL CommonBL = new CommonBL();
            string DTRDBName = CommonBL.GetDatabaseNameByProfileType(CentralProfile, CompanyCode, "D");
            string qString = string.Format(@"
                                    DECLARE @StartDate DATE = '{1}'
                                    DECLARE @EndDate DATE = '{2}'

                                    SELECT Tel_IDNo [IDNo]
                                       , Tel_LogDate [Date]
                                       , MAX([1]) [DTR1]
                                       , MAX([2]) [DTR2]
                                       , MAX([3]) [DTR3]
                                       , MAX([4]) [DTR4]
                                       , MAX([5]) [DTR5]
                                       , MAX([6]) [DTR6]
                                       , MAX([7]) [DTR7]
                                       , MAX([8]) [DTR8]
                                       , MAX([9]) [DTR9]
                                       , MAX([10]) [DTR10]
                                       , MAX([11]) [DTR11]
                                       , MAX([12]) [DTR12]
                                FROM (
                                       SELECT Tel_IDNo
                                              , CONVERT(DATETIME, Tel_LogDate) Tel_LogDate
                                              ,CASE Tel_LogTime
                                                                        WHEN '0000'
                                                                        THEN '24:00'
                                                                        ELSE SUBSTRING(Tel_LogTime,1,2) + ':' + SUBSTRING(Tel_LogTime,3,2) 
                                                                     END AS Tel_LogTime
                                              , ROW_NUMBER() OVER(PARTITION BY Tel_IDNo, Tel_LogDate ORDER BY Ludatetime) [RowCount]
                                       FROM {3}..T_EmpDTR
                                ) DTR
                                PIVOT (MAX([Tel_LogTime]) FOR [RowCount] IN ([1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12] )) P
                                WHERE Tel_IDNo = '{0}' AND CONVERT(date,Tel_LogDate) BETWEEN @StartDate AND @EndDate
                                GROUP BY Tel_IDNo, Tel_LogDate
                                ORDER BY Tel_IDNo, Tel_LogDate
                                ", Dtr_EmployeeId, Convert.ToDateTime(startdate), Convert.ToDateTime(enddate).AddDays(1), DTRDBName);

            
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString);
                dal.CloseDB();
            }
            return ds;
        }

        public DataSet GetSartAndEndCycleDate(string Tps_PayCycle)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"Select Convert(char(10),Tps_StartCycle,101) as Tps_StartCycle
                                     ,Convert(char(10),Tps_EndCycle,101) as Tps_EndCycle
                                        From T_PaySchedule
                                        Where Tps_RecordStatus = 'A'
                                        And Tps_CycleIndicator <> 'F'
                                        And Tps_PayCycle = @Tps_PayCycle";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Tps_PayCycle", Tps_PayCycle);
            
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            return ds;
        }

        public string GetEquivalentDay(string date)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@date", date);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet("Select SUBSTRING(DATENAME(weekday, @date), 1, 3) as DOW", CommandType.Text, paramInfo);

                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0][0].ToString();
            else
                return string.Empty;
        }

        public DataSet GetReportLogViewing(string employeeid, string startdate, string enddate)
        {
            DataSet ds;

            #region
            string sql = @"Select	Dtr_EmployeeId
                                ,RTRIM(Mem_LastName) as LastName
		,RTRIM(Mem_FirstName) as FirstName
		,Left(Mem_MiddleName,1) as MI
		                                ,Convert(char(10),Dtr_ProcessDate,101) as Dtr_ProcessDate
		                                ,SUBSTRING(DATENAME(weekday, Dtr_ProcessDate), 1, 3) as DOW
		                                ,CASE Dtr_In_1
			                                WHEN '0000'
			                                THEN ''
			                                ELSE SUBSTRING(Dtr_In_1,1,2) + ':' + SUBSTRING(Dtr_In_1,3,2) 
		                                 END as Dtr_In_1
		                                ,CASE Dtr_Out_1
			                                WHEN '0000'
			                                THEN ''
			                                ELSE SUBSTRING(Dtr_Out_1,1,2) + ':' + SUBSTRING(Dtr_Out_1,3,2) 
		                                 END as Dtr_Out_1
		                                ,CASE Dtr_In_2
			                                WHEN '0000'
			                                THEN ''
			                                ELSE SUBSTRING(Dtr_In_2,1,2) + ':' + SUBSTRING(Dtr_In_2,3,2) 
		                                 END as Dtr_In_2
		                                ,CASE Dtr_Out_2
			                                WHEN '0000'
			                                THEN ''
			                                ELSE SUBSTRING(Dtr_Out_2,1,2) + ':' + SUBSTRING(Dtr_Out_2,3,2) 
		                                 END as Dtr_Out_2
		                                ,CASE Dtr_In_3
			                                WHEN '0000'
			                                THEN ''
			                                ELSE SUBSTRING(Dtr_In_3,1,2) + ':' + SUBSTRING(Dtr_In_3,3,2) 
		                                 END as Dtr_In_3
		                                ,CASE Dtr_Out_3
			                                WHEN '0000'
			                                THEN ''
			                                ELSE SUBSTRING(Dtr_Out_3,1,2) + ':' + SUBSTRING(Dtr_Out_3,3,2) 
		                                 END as Dtr_Out_3
		                                ,CASE Dtr_In_4
			                                WHEN '0000'
			                                THEN ''
			                                ELSE SUBSTRING(Dtr_In_4,1,2) + ':' + SUBSTRING(Dtr_In_4,3,2) 
		                                 END as Dtr_In_4
		                                ,CASE Dtr_Out_4
			                                WHEN '0000'
			                                THEN ''
			                                ELSE SUBSTRING(Dtr_Out_4,1,2) + ':' + SUBSTRING(Dtr_Out_4,3,2) 
		                                 END as Dtr_Out_4
		                                ,CASE Dtr_In_5
			                                WHEN '0000'
			                                THEN ''
			                                ELSE SUBSTRING(Dtr_In_5,1,2) + ':' + SUBSTRING(Dtr_In_5,3,2) 
		                                 END as Dtr_In_5
		                                ,CASE Dtr_Out_5
			                                WHEN '0000'
			                                THEN ''
			                                ELSE SUBSTRING(Dtr_Out_5,1,2) + ':' + SUBSTRING(Dtr_Out_5,3,2) 
		                                 END as Dtr_Out_5
                                From T_DTRLedger Inner Join M_Employee
                                         On Dtr_EmployeeId = Mem_IDNo
                                Where Dtr_EmployeeId = @Dtr_EmployeeId
                                And Dtr_ProcessDate between @startdate and @enddate";
                    #endregion

            ParameterInfo[] param = new ParameterInfo[3];
            param[0] = new ParameterInfo("@Dtr_EmployeeId", employeeid);
            param[1] = new ParameterInfo("@startdate", startdate);
            param[2] = new ParameterInfo("@enddate", enddate);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sql, CommandType.Text, param);
                dal.CloseDB();
            }

            return ds;
        }

        public void PostLogs(string Dtr_EmployeeId, string startdate, string enddate)
        {
            bool firstIn = false;
            bool lastOut = false;
            string dbname = string.Empty;

            string sqlFetch = string.Format(@"
                            select 
                            Dtr_LogDate, Dtr_LogTime, Dtr_EmployeeID, Dtr_LogType
                            from T_DTR
                            where Dtr_EmployeeId = '{0}' and Dtr_LogDate >= '{1}' and Dtr_LogDate <= '{2}'", Dtr_EmployeeId, startdate, enddate);

            DALHelper dalDTR = new DALHelper(true);
            dalDTR.OpenDB();

            //load non posted records from DTR
            DataTable dtrTable = dalDTR.ExecuteDataSet(sqlFetch).Tables[0];

            sqlFetch = @"
                        SELECT Ttr_ActIn_1
                              ,Ttr_ActOut_1
                              ,Ttr_ActIn_2
                              ,Ttr_ActOut_2
                              ,Msh_ShiftIn1
                              ,Msh_ShiftOut1
                              ,Msh_ShiftIn2
                              ,Msh_ShiftOut2  
                          FROM T_EmpTimeRegister
                          JOIN M_Shift
                            ON Ttr_ShiftCode = Msh_ShiftCode
                         WHERE Ttr_IDNo = '{0}'
                           AND Ttr_Date = '{1}'";

            string sqlReplace = @"
                        UPDATE T_EmpTimeRegister
                           SET {2} = '{3}'
                         WHERE Ttr_IDNo = '{0}'
                           AND Ttr_Date = '{1}'";

            string sqlClearAll = @"
                        UPDATE T_EmpTimeRegister
                           SET Ttr_ActIn_1 = '0000'
                             , Ttr_ActIn_2 = '0000'
                             , Ttr_ActOut_1 = '0000'
                             , Ttr_ActOut_2 = '0000'
                         WHERE Ttr_IDNo = '{0}'
                           AND Ttr_Date >= '{1}'
                           AND Ttr_Date <= '{2}'";


            DataTable dtLogLedger;
            DataRow drLogLedger;
            DALHelper dalLogdger = new DALHelper();
            dalLogdger.OpenDB();

            string Field;
            bool bReplace;
            bool bError;
            bool bGraveyard;

            //initialize log ledger records
            dalLogdger.ExecuteNonQuery(string.Format(sqlClearAll, Dtr_EmployeeId, startdate, enddate));

            //loop through non posted records from DTR
            foreach (DataRow drDTR in dtrTable.Rows)
            {
                Field = string.Empty;
                bReplace = false;
                bError = false;
                bGraveyard = false;

                dtLogLedger = dalLogdger.ExecuteDataSet(string.Format(sqlFetch, drDTR["Dtr_EmployeeID"].ToString(), drDTR["Dtr_LogDate"].ToString())).Tables[0];
                if (dtLogLedger.Rows.Count == 1)
                {
                    //load LogLedger row for current date
                    drLogLedger = dtLogLedger.Rows[0];

                    //change dtr log if exactly equal to 0000, move by 1 minute
                    if (drDTR["Dtr_LogTime"].ToString() == "0000")
                    {
                        if (drDTR["Dtr_LogType"].ToString() == "I")
                        {
                            drDTR["Dtr_LogDate"] = Convert.ToDateTime(drDTR["Dtr_LogDate"].ToString()).AddDays(-1).ToString("MM/dd/yyyy");
                            drDTR["Dtr_LogTime"] = "2359";
                        }
                        else if (drDTR["Dtr_LogType"].ToString() == "O")
                            drDTR["Dtr_LogTime"] = "0001";
                    }

                    //IN mapping
                    if (drDTR["Dtr_LogType"].ToString() == "I")
                    {
                        //Check which time IN field the time belongs to
                        if (Convert.ToInt32(drDTR["Dtr_LogTime"].ToString()) <= Convert.ToInt32(drLogLedger["Msh_ShiftOut1"].ToString()))
                            Field = "Ttr_ActIn_1";
                        else
                            Field = "Ttr_ActIn_2";

                        //if 0000, means null value so always replace
                        if (drLogLedger[Field].ToString() == "0000")
                            bReplace = true;

                        //for IN2, check if OUT2 already exists and IN2 value is earlier than supposed OUT2
                        if (Field == "Ttr_ActIn_2" && Convert.ToInt32(drDTR["Dtr_LogTime"].ToString()) >= Convert.ToInt32(drLogLedger["Msh_ShiftOut2"].ToString()))
                        {
                            bError = true;
                        }

                        //check if First IN policy is implemented, and compare against current posted log time
                        if (firstIn)
                        {
                            //if new IN is earlier than previous IN
                            if (Convert.ToInt32(drLogLedger[Field].ToString()) > Convert.ToInt32(drDTR["Dtr_LogTime"].ToString()))
                                bReplace = true;
                        }
                        else
                        {
                            //if new IN is later than previous IN
                            if (Convert.ToInt32(drLogLedger[Field].ToString()) < Convert.ToInt32(drDTR["Dtr_LogTime"].ToString()))
                                bReplace = true;
                        }
                    }
                    else if (drDTR["Dtr_LogType"].ToString() == "O")
                    {
                        //check if previous day is graveyard
                        if (isGraveyardPreviousDay(drDTR["Dtr_EmployeeID"].ToString(), drDTR["Dtr_LogDate"].ToString(), dalLogdger))
                        {
                            //check if there is aleady time IN for current day
                            if (drLogLedger["Ttr_ActIn_1"].ToString() == "0000")
                            {
                                //if none, change DTR date to previous day
                                drDTR["Dtr_LogDate"] = Convert.ToDateTime(drDTR["Dtr_LogDate"].ToString()).AddDays(-1).ToString("MM/dd/yyyy");
                                //reload log ledger record to previous day
                                dtLogLedger = dalLogdger.ExecuteDataSet(string.Format(sqlFetch, drDTR["Dtr_EmployeeID"].ToString(), drDTR["Dtr_LogDate"].ToString())).Tables[0];
                                if (dtLogLedger.Rows.Count == 1)
                                    drLogLedger = dtLogLedger.Rows[0];
                                else
                                    bError = true; //no log for previous day to post on
                            }
                            bGraveyard = true;
                        }

                        //logTime will be used instead of Dtr_LogTime field so we can add 2400 if it is graveyard next day
                        //for the log checkings
                        int logTime = Convert.ToInt32(drDTR["Dtr_LogTime"].ToString());

                        //Check which time OUT field the time belongs to
                        //if graveyard previous day, add 24 hours to DTR
                        //not yet 100% sure on this part
                        if (bGraveyard)
                        {
                            int shiftBreakEnd;
                            logTime = logTime + 2400;
                            if (Convert.ToInt32(drLogLedger["Msh_ShiftIn2"].ToString()) < Convert.ToInt32(drLogLedger["Msh_ShiftIn1"].ToString()))
                            {
                                shiftBreakEnd = Convert.ToInt32(drLogLedger["Msh_ShiftIn2"].ToString()) + 2400;
                            }
                            else
                            {
                                shiftBreakEnd = Convert.ToInt32(drLogLedger["Msh_ShiftIn2"].ToString());
                            }

                            if (logTime <= shiftBreakEnd)
                                Field = "Ttr_ActOut_1";
                            else
                                Field = "Ttr_ActOut_2";
                        }
                        else
                        {
                            if (Convert.ToInt32(drDTR["Dtr_LogTime"].ToString()) <= Convert.ToInt32(drLogLedger["Msh_ShiftIn2"].ToString()))
                                Field = "Ttr_ActOut_1";
                            else
                                Field = "Ttr_ActOut_2";
                        }

                        //0000 means no value posted, so it will be replaced
                        if (drLogLedger[Field].ToString() == "0000")
                            bReplace = true;

                        //for OUT1, check if IN1 already exists and IN1 value is earlier than supposed OUT1
                        if (Field == "Ttr_ActOut_1" && (drLogLedger["Ttr_ActIn_1"].ToString() == "0000" || logTime <= Convert.ToInt32(drLogLedger["Ttr_ActIn_1"].ToString())))
                        {
                            bError = true;
                        }

                        //check if Last OUT policy is implemented, and compare against current posted log time
                        if (lastOut)
                        {
                            if (Convert.ToInt32(drLogLedger[Field].ToString()) < Convert.ToInt32(drDTR["Dtr_LogTime"].ToString()))
                                bReplace = true;
                        }
                        else
                        {
                            if (Convert.ToInt32(drLogLedger[Field].ToString()) > Convert.ToInt32(drDTR["Dtr_LogTime"].ToString()))
                                bReplace = true;
                        }
                    }
                    else
                    {
                        //error
                    }

                    if (bReplace && !bError)
                    {
                        dalLogdger.ExecuteNonQuery(string.Format(sqlReplace, drDTR["Dtr_EmployeeID"].ToString(), drDTR["Dtr_LogDate"].ToString(), Field, drDTR["Dtr_LogTime"].ToString()));
                    }

                    //dalDTR.ExecuteNonQuery(string.Format(sqlPost, drDTR["Dtr_EmployeeID"].ToString(), drDTR["Dtr_LogDate"].ToString(), drDTR["Dtr_LogTime"].ToString(), drDTR["Dtr_LogType"].ToString()));

                    //dalLogdger.ExecuteNonQuery(string.Format(sqlInsertDtrLedger, drDTR["Dtr_EmployeeID"].ToString(), drDTR["Dtr_LogDate"].ToString(), drDTR["Dtr_LogTime"].ToString()));
                    //if (drDTR["Dtr_LogType"].ToString() == "I")
                    //    dalLogdger.ExecuteNonQuery(string.Format(sqlDtrLedgerIN, drDTR["Dtr_EmployeeID"].ToString(), drDTR["Dtr_LogDate"].ToString(), drDTR["Dtr_LogTime"].ToString()));
                    //if (drDTR["Dtr_LogType"].ToString() == "O")
                    //    dalLogdger.ExecuteNonQuery(string.Format(sqlDtrLedgerOUT, drDTR["Dtr_EmployeeID"].ToString(), drDTR["Dtr_LogDate"].ToString(), drDTR["Dtr_LogTime"].ToString()));
                }
            }

            //add code to send e-mail here
        }

        public bool isGraveyardPreviousDay(string employeeID, string logDate, DALHelper dal)
        {
            string previousDay = Convert.ToDateTime(logDate).AddDays(-1).ToString("MM/dd/yyyy");

            string sqlFetch = @"
                        SELECT Msh_Schedule
                          FROM T_EmpTimeRegister
                          JOIN M_Shift
                            ON Ttr_ShiftCode = Msh_ShiftCode
                         WHERE Ttr_IDNo = '{0}'
                           AND Ttr_Date = '{1}'";

            DataTable dt = dal.ExecuteDataSet(string.Format(sqlFetch, employeeID, previousDay)).Tables[0];

            if (dt.Rows.Count == 1)
            {
                if (dt.Rows[0][0].ToString() == "G")
                    return true;
            }

            return false;
        }
    }
}
