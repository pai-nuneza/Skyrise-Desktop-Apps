using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using CommonLibrary;
using Payroll.DAL;

namespace Payroll.BLogic
{
    public class ShiftCodeBL : BaseBL
    {
        public ShiftCodeBL()
        {
        }

        public override int Add(System.Data.DataRow row)
        {
            int retVal = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[18];
            paramInfo[0] = new ParameterInfo("@Msh_ShiftCode", row["Msh_ShiftCode"]);
            paramInfo[1] = new ParameterInfo("@Msh_ShiftName", row["Msh_ShiftName"]);
            paramInfo[2] = new ParameterInfo("@Msh_ShiftIn1", row["Msh_ShiftIn1"]);
            paramInfo[3] = new ParameterInfo("@Msh_ShiftOut1", row["Msh_ShiftOut1"]);
            paramInfo[4] = new ParameterInfo("@Msh_ShiftIn2", row["Msh_ShiftIn2"]);
            paramInfo[5] = new ParameterInfo("@Msh_ShiftOut2", row["Msh_ShiftOut2"]);
            paramInfo[6] = new ParameterInfo("@Msh_Schedule", row["Msh_Schedule"]);
            paramInfo[7] = new ParameterInfo("@Msh_ShiftHours", row["Msh_ShiftHours"]);
            paramInfo[8] = new ParameterInfo("@Msh_PaidBreak", row["Msh_PaidBreak"]);
            paramInfo[9] = new ParameterInfo("@Msh_8HourShiftCode", row["Msh_8HourShiftCode"]);
            paramInfo[10] = new ParameterInfo("@Msh_RecordStatus", row["Msh_RecordStatus"]);
            paramInfo[11] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);
            ////inserted 20090131: pad shift
            paramInfo[12] = new ParameterInfo("@Msh_PadIn1", row["Msh_PadIn1"]);
            paramInfo[13] = new ParameterInfo("@Msh_PadOut1", row["Msh_PadOut1"]);
            paramInfo[14] = new ParameterInfo("@Msh_PadIn2", row["Msh_PadIn2"]);
            paramInfo[15] = new ParameterInfo("@Msh_PadOut2", row["Msh_PadOut2"]);
            paramInfo[16] = new ParameterInfo("@Msh_IsDefaultShift", row["Msh_IsDefaultShift"]);
            paramInfo[17] = new ParameterInfo("@Msh_HourFractionCutoff", row["Msh_HourFractionCutoff"]);
            ////end 20090131

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();
                string sqlquery = @"INSERT INTO M_Shift 
                                                (Msh_ShiftCode,
                                                 Msh_ShiftName,
                                                 Msh_ShiftIn1,
                                                 Msh_ShiftOut1,
                                                 Msh_ShiftIn2,
                                                 Msh_ShiftOut2,
                                                 Msh_Schedule,
                                                 Msh_ShiftHours,
                                                 Msh_PaidBreak,
                                                 Msh_8HourShiftCode,
                                                 Msh_RecordStatus,  
                                                 Usr_Login,
                                                 Ludatetime,
                                                 Msh_PadIn1,   
                                                 Msh_PadOut1,
                                                 Msh_PadIn2,
                                                 Msh_PadOut2,
                                                 Msh_NightPrem,
                                                 Msh_IsDefaultShift,
                                                 Msh_HourFractionCutoff) 
                                               VALUES
                                                (@Msh_ShiftCode,
                                                 @Msh_ShiftName,
                                                 @Msh_ShiftIn1,
                                                 @Msh_ShiftOut1,
                                                 @Msh_ShiftIn2,
                                                 @Msh_ShiftOut2,
                                                 @Msh_Schedule,
                                                 @Msh_ShiftHours,
                                                 @Msh_PaidBreak,
                                                 @Msh_8HourShiftCode,
                                                 @Msh_RecordStatus,
                                                 @Usr_Login,
                                                 GetDate(),                                                
                                                 @Msh_PadIn1,   
                                                 @Msh_PadOut1,
                                                 @Msh_PadIn2,
                                                 @Msh_PadOut2,
                                                 'O',
                                                 @Msh_IsDefaultShift,
                                                 @Msh_HourFractionCutoff)";
                try
                {
                    retVal = dal.ExecuteNonQuery(sqlquery, CommandType.Text, paramInfo);

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
  
        public override int Update(System.Data.DataRow row)
        {
            int retVal = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[18];
            paramInfo[0] = new ParameterInfo("@Msh_ShiftCode", row["Msh_ShiftCode"]);
            paramInfo[1] = new ParameterInfo("@Msh_ShiftName", row["Msh_ShiftName"]);
            paramInfo[2] = new ParameterInfo("@Msh_ShiftIn1", row["Msh_ShiftIn1"]);
            paramInfo[3] = new ParameterInfo("@Msh_ShiftOut1", row["Msh_ShiftOut1"]);
            paramInfo[4] = new ParameterInfo("@Msh_ShiftIn2", row["Msh_ShiftIn2"]);
            paramInfo[5] = new ParameterInfo("@Msh_ShiftOut2", row["Msh_ShiftOut2"]);
            paramInfo[6] = new ParameterInfo("@Msh_Schedule", row["Msh_Schedule"]);
            paramInfo[7] = new ParameterInfo("@Msh_ShiftHours", row["Msh_ShiftHours"]);
            paramInfo[8] = new ParameterInfo("@Msh_PaidBreak", row["Msh_PaidBreak"]);
            paramInfo[9] = new ParameterInfo("@Msh_8HourShiftCode", row["Msh_8HourShiftCode"]);
            paramInfo[10] = new ParameterInfo("@Msh_RecordStatus", row["Msh_RecordStatus"]);
            paramInfo[11] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);
            ////inserted 20090131: pad shift
            paramInfo[12] = new ParameterInfo("@Msh_PadIn1", row["Msh_PadIn1"]);
            paramInfo[13] = new ParameterInfo("@Msh_PadOut1", row["Msh_PadOut1"]);
            paramInfo[14] = new ParameterInfo("@Msh_PadIn2", row["Msh_PadIn2"]);
            paramInfo[15] = new ParameterInfo("@Msh_PadOut2", row["Msh_PadOut2"]);
            paramInfo[16] = new ParameterInfo("@Msh_IsDefaultShift", row["Msh_IsDefaultShift"]);
            paramInfo[17] = new ParameterInfo("@Msh_HourFractionCutoff", row["Msh_HourFractionCutoff"]);
            ////end 20090131

            string sqlquery = @"UPDATE M_Shift 
                                              SET Msh_ShiftName = @Msh_ShiftName,     
                                                  Msh_ShiftIn1 = @Msh_ShiftIn1,
                                                  Msh_ShiftOut1 = @Msh_ShiftOut1,
                                                  Msh_ShiftIn2 = @Msh_ShiftIn2,
                                                  Msh_ShiftOut2 = @Msh_ShiftOut2,
                                                  Msh_Schedule = @Msh_Schedule,
                                                  Msh_ShiftHours = @Msh_ShiftHours,
                                                  Msh_PaidBreak = @Msh_PaidBreak,
                                                  Msh_8HourShiftCode = @Msh_8HourShiftCode,
                                                  Msh_PadIn1 = @Msh_PadIn1,
                                                  Msh_PadOut1 = @Msh_PadOut1,
                                                  Msh_PadIn2 = @Msh_PadIn2,
                                                  Msh_PadOut2 = @Msh_PadOut2,
                                                  Msh_RecordStatus = @Msh_RecordStatus,                               
                                                  Usr_Login=@Usr_Login,
                                                  Ludatetime = GetDate(),
                                                  Msh_IsDefaultShift = @Msh_IsDefaultShift,
                                                  Msh_HourFractionCutoff = @Msh_HourFractionCutoff
                                            WHERE Msh_ShiftCode = @Msh_ShiftCode";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    retVal = dal.ExecuteNonQuery(sqlquery, CommandType.Text, paramInfo);
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

        public override int Delete(string shiftcode, string userlogin)
        {
            int retVal = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Msh_ShiftCode", shiftcode.Trim());
            paramInfo[1] = new ParameterInfo("@Usr_Login", userlogin.Trim());

            string sqlquery = @"UPDATE M_Shift SET Msh_RecordStatus = 'C' 
                                                  ,Usr_Login=@Usr_Login                               
                                                  ,Ludatetime = GetDate()
                                              WHERE Msh_ShiftCode = @Msh_ShiftCode";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    retVal = dal.ExecuteNonQuery(sqlquery, CommandType.Text, paramInfo);
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

        public override System.Data.DataSet FetchAll()
        {
            DataSet ds = new DataSet();

            string sqlquery = @"SELECT Msh_ShiftCode
                                      ,Msh_ShiftName
                                      ,Mcd_Name as [Msh_Schedule]
                                      ,(LEFT(Msh_ShiftIn1, 2) + ':' + SUBSTRING(Msh_ShiftIn1, 3,2)) AS [Msh_ShiftIn1]
                                      ,Msh_PadIn1
                                      ,(LEFT(Msh_ShiftOut1, 2) + ':' + SUBSTRING(Msh_ShiftOut1, 3,2)) AS [Msh_ShiftOut1]
                                      ,Msh_PadOut1
                                      ,(LEFT(Msh_ShiftIn2, 2) + ':' + SUBSTRING(Msh_ShiftIn2, 3,2)) AS [Msh_ShiftIn2]
                                      ,Msh_PadIn2
                                      ,(LEFT(Msh_ShiftOut2, 2) + ':' + SUBSTRING(Msh_ShiftOut2, 3,2)) AS [Msh_ShiftOut2]
                                      ,Msh_PadOut2
                                      ,Msh_ShiftHours
                                      ,Msh_PaidBreak
                                      ,Msh_8HourShiftCode
                                      ,Msh_RecordStatus
                                      ,Msh_ShiftName AS [EquivalentShiftCodeDesc]
                                      ,SCM.Usr_Login
                                      ,SCM.Ludatetime
                                      ,Msh_Schedule as ScheduleTypeCode
                                      ,Msh_IsDefaultShift
                                      ,Msh_HourFractionCutoff
                                 FROM M_Shift AS SCM
                            LEFT JOIN M_CodeDtl on Msh_Schedule = Mcd_Code AND Mcd_CodeType = 'SCHEDTYPE'";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlquery, CommandType.Text);

                dal.CloseDB();
            }
            return ds;
        }

        public override System.Data.DataRow Fetch(string code)
        {
            DataSet ds = new DataSet();

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0];
            else
                return null;
        }

        public DataRow FetchAcctCodeInAccountDetail(string Mcd_Name, string Mcd_CodeType)
        {
            DataSet ds = new DataSet();

            using (DALHelper dal = new DALHelper())
            {
                ParameterInfo[] paramCollection = new ParameterInfo[2];
                paramCollection[0] = new ParameterInfo("@Mcd_Name", Mcd_Name, SqlDbType.Char, 50);
                paramCollection[1] = new ParameterInfo("@Mcd_CodeType", Mcd_CodeType, SqlDbType.Char, 10);

                dal.OpenDB();
                ds = dal.ExecuteDataSet("SELECT Mcd_Code FROM M_CodeDtl WHERE Mcd_CodeType=@Mcd_CodeType AND Mcd_Name=@Mcd_Name", CommandType.Text, paramCollection);
                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0];
            else
                return null;
        }

        public DataRow FetchShiftCode(string Msh_ShiftName)
        {
            DataSet ds = new DataSet();

            using (DALHelper dal = new DALHelper())
            {
                ParameterInfo[] paramCollection = new ParameterInfo[1];
                paramCollection[0] = new ParameterInfo("@Msh_ShiftName", Msh_ShiftName, SqlDbType.Char, 50);

                dal.OpenDB();
                ds = dal.ExecuteDataSet("SELECT Msh_ShiftCode FROM M_Shift WHERE Msh_ShiftCode = @Msh_ShiftName", CommandType.Text, paramCollection);
                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0];
            else
                return null;
        }


        public DataTable GetEmployeeShift(bool IsNonRegularDay, string DayCode, string ShiftCode, string CompanyCode, string CentralProfile)
        {
            string query = string.Empty;
            string condition = "";
            if (IsNonRegularDay)
                condition = "AND Msh_ShiftHours = 8";

            #region query
            query = string.Format(@"Select Msh_ShiftCode As 'Shift Code'
                                           ,Msh_ShiftName As 'Description'
                                           ,Msh_ShiftIn1 As 'Time In'
                                           ,Msh_ShiftOut1 As 'Break Start'
                                           ,Msh_ShiftIn2 As 'Break End'
                                           ,Msh_ShiftOut2 As 'Time Out'
                                           ,Msh_ShiftHours As 'Shift Hours'
                                           ,Msh_PaidBreak As 'Paid Break'
                                     FROM M_Shift
                                     WHERE Msh_RecordStatus = 'A' 
                                            AND Msh_CompanyCode = '{2}'
                                     {1}
                                     and Msh_ShiftCode = '{0}'", ShiftCode, condition, CompanyCode);


            #endregion

            DataTable dtResult;
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                dtResult = dal.ExecuteDataSet(query).Tables[0];
                dal.CloseDB();
            }
            return dtResult;
        }


        public bool ShiftCodeExist(string ShiftCode, string CompanyCode, string CentralProfile)
        {
            string query = string.Format(@"SELECT Msh_ShiftCode FROM M_Shift 
                                                WHERE Msh_ShiftCode = '{0}'
                                                    AND Msh_CompanyCode = '{1}'", ShiftCode, CompanyCode);

            DataTable dtResult = new DataTable();
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();

                dtResult = dal.ExecuteDataSet(query).Tables[0];

                dal.CloseDB();
            }

            return (dtResult.Rows.Count > 0) ? true : false;
        }

        //It will check if the data is being used by other records.
        public bool CheckIfAllowDelete(string shiftcode)
        {
            DataSet ds = new DataSet();
            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@shiftcode", shiftcode, SqlDbType.Char, 25);

            string qstring = @"SELECT DISTINCT  Ttr_ShiftCode
								    FROM T_EmpTimeRegisterHst
                                        WHERE Ttr_ShiftCode = @shiftcode";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qstring, CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return false;
            else
                return true;
        }

    }
}

