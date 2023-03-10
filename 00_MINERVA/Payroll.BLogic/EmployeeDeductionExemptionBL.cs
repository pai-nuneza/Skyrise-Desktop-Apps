using System;
using System.Collections.Generic;
using System.Text;

using System.Data;
using CommonLibrary;
using Payroll.DAL;

namespace Payroll.BLogic
{
    public class EmployeeDeductionExemptionBL : BaseBL
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
            DataSet ds = new DataSet();

            string sqlQuery = @"
                            SELECT Tps_PayCycle
                            ,Convert(char(10), Tps_StartCycle, 101) as Tps_StartCycle
                            ,Convert(char(10), Tps_EndCycle, 101) as Tps_EndCycle
                            FROM T_PaySchedule
                            WHERE Tps_CycleIndicator = 'C'
                            OR Tps_CycleIndicator = 'F'";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text);

                dal.CloseDB();
            }
            return ds;
        }

        public override DataRow Fetch(string code)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int InsertRecord(DataRow row)
        {
            int retVal = 0;
            int paramIndex = 0;
            ParameterInfo[] paramInfo = new ParameterInfo[6];
            paramInfo[paramIndex++] = new ParameterInfo("@Tde_IDNo", row["Tde_IDNo"], SqlDbType.VarChar, 15);
            paramInfo[paramIndex++] = new ParameterInfo("@Tde_DeductionCode", row["Tde_DeductionCode"], SqlDbType.VarChar, 10);
            paramInfo[paramIndex++] = new ParameterInfo("@Tde_StartDate", row["Tde_StartDate"], SqlDbType.Date);
            paramInfo[paramIndex++] = new ParameterInfo("@Tde_PayCycle", row["Tde_PayCycle"]);
            paramInfo[paramIndex++] = new ParameterInfo("@Tde_Remarks", row["Tde_Remarks"], SqlDbType.VarChar, 30);
            paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", row["Usr_Login"], SqlDbType.VarChar, 15);

            string qString = @"SELECT * FROM T_EmpDeductionExempt
                               WHERE Tde_IDNo = @Tde_IDNo
                                   AND Tde_DeductionCode = @Tde_DeductionCode
                                   AND Tde_StartDate = @Tde_StartDate
                                   AND Tde_PayCycle = @Tde_PayCycle";

            string UpdateString = @"UPDATE T_EmpDeductionExempt
                                        SET Tde_Remarks = @Tde_Remarks
                                        ,Usr_Login = @Usr_Login
                                        ,Ludatetime = Getdate()
                                    WHERE Tde_IDNo = @Tde_IDNo
                                        AND Tde_DeductionCode = @Tde_DeductionCode
                                        AND Tde_StartDate = @Tde_StartDate
                                        AND Tde_PayCycle = @Tde_PayCycle";

            

            string sqlQuery = @"INSERT INTO T_EmpDeductionExempt
                                               (Tde_IDNo
                                               ,Tde_DeductionCode
                                               ,Tde_StartDate
                                               ,Tde_PayCycle
                                               ,Tde_Remarks
                                               ,Usr_Login
                                               ,Ludatetime)
                                         VALUES
                                               (@Tde_IDNo
			                                    ,@Tde_DeductionCode
			                                    ,@Tde_StartDate
			                                    ,@Tde_PayCycle
			                                    ,@Tde_Remarks
			                                    ,@Usr_Login
			                                    ,GETDATE())";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                try
                {
                    DataSet ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);
                    if (ds.Tables[0].Rows.Count > 0)
                        retVal = dal.ExecuteNonQuery(UpdateString, CommandType.Text, paramInfo);
                    else
                        retVal = dal.ExecuteNonQuery(sqlQuery, CommandType.Text, paramInfo);
                }
                catch (Exception e)
                {
                    throw new PayrollException(e);
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return retVal;
        }

        public DataSet FetchAllForEdit(string Tde_IDNo, string Tde_DeductionCode, object Tde_StartDate, string CentralProfile)
        {
            DataSet ds = new DataSet();

            string qString = string.Format(@"SELECT Tde_PayCycle AS [PayCycle]
		                                        ,CONVERT(CHAR(10), Tps_StartCycle, 101) as [StartCycle]
		                                        ,CONVERT(CHAR(10), Tps_EndCycle, 101) as [EndCycle]
                                                ,T_EmpDeductionHdr.Tdh_CycleAmount as [Amount]
                                                ,Tde_Remarks AS [Remarks]
                                                ,Tps_CycleIndicator AS [CycleIndicator]
                                                ,Tps_CycleIndicatorSpecial AS [CycleIndicatorSpecial] 
                                                ,T_EmpDeductionExempt.Usr_Login AS [Usr_Login]
                                                ,T_EmpDeductionExempt.Ludatetime AS [Ludatetime]
                                             FROM T_EmpDeductionExempt
	                                         INNER JOIN T_PaySchedule ON Tde_PayCycle = Tps_PayCycle
                                             LEFT JOIN {0}..T_EmpDeductionHdr ON Tdh_IDNo = Tde_IDNo
                                                AND Tdh_DeductionCode = Tde_DeductionCode
                                                AND Tdh_StartDate = Tde_StartDate
                                            WHERE Tde_IDNo = @Tde_IDNo
                                                AND Tde_DeductionCode = @Tde_DeductionCode
                                                AND Tde_StartDate = @Tde_StartDate"
                                            , CentralProfile);

            ParameterInfo[] paramInfo = new ParameterInfo[3];
            paramInfo[0] = new ParameterInfo("@Tde_IDNo", Tde_IDNo);
            paramInfo[1] = new ParameterInfo("@Tde_DeductionCode", Tde_DeductionCode);
            paramInfo[2] = new ParameterInfo("@Tde_StartDate", Tde_StartDate);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);
                dal.CloseDB();
            }
            return ds;
        }

        public DataSet FetchAllForEdit(string Tde_IDNo, string Tde_DeductionCode, object Tde_StartDate, string Tde_PayCycle, string CentralProfile)
        {
            DataSet ds = new DataSet();

            string qString = string.Format(@"SELECT CONVERT(CHAR(10), Tps_StartCycle, 101) as [StartCycle]
		                                        ,CONVERT(CHAR(10), Tps_EndCycle, 101) as [EndCycle]
                                                ,T_EmpDeductionHdr.Tdh_CycleAmount as [Amount]
                                                ,Tde_Remarks AS [Remarks]
                                                ,Tps_CycleIndicator AS [CycleIndicator]
                                                ,Tps_CycleIndicatorSpecial AS [CycleIndicatorSpecial] 
                                                ,T_EmpDeductionExempt.Usr_Login AS [Usr_Login]
                                                ,T_EmpDeductionExempt.Ludatetime AS [Ludatetime]
                                             FROM T_EmpDeductionExempt
	                                         INNER JOIN T_PaySchedule ON Tde_PayCycle = Tps_PayCycle
                                             LEFT JOIN {0}..T_EmpDeductionHdr ON Tdh_IDNo = Tde_IDNo
                                                AND Tdh_DeductionCode = Tde_DeductionCode
                                                AND Tdh_StartDate = Tde_StartDate
                                            WHERE Tde_IDNo = @Tde_IDNo
                                                AND Tde_DeductionCode = @Tde_DeductionCode
                                                AND Tde_StartDate = @Tde_StartDate 
                                                AND Tde_PayCycle = @Tde_PayCycle "
                                            , CentralProfile);

            ParameterInfo[] paramInfo = new ParameterInfo[4];
            paramInfo[0] = new ParameterInfo("@Tde_IDNo", Tde_IDNo);
            paramInfo[1] = new ParameterInfo("@Tde_DeductionCode", Tde_DeductionCode);
            paramInfo[2] = new ParameterInfo("@Tde_StartDate", Tde_StartDate, SqlDbType.Date);
            paramInfo[3] = new ParameterInfo("@Tde_PayCycle", Tde_PayCycle);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);
                dal.CloseDB();
            }
            return ds;
        }

        public int UpdateRecord(DataRow row)
        {
            try
            {
                int retVal = 0;
                int paramIndex = 0;
                string UpdateString = @"UPDATE T_EmpDeductionExempt
                                        SET Tde_Remarks = @Tde_Remarks
                                            ,Usr_Login = @Usr_Login
                                            ,Ludatetime = GETDATE()
                                        WHERE Tde_IDNo = @Tde_IDNo
                                            AND Tde_DeductionCode = @Tde_DeductionCode
                                            AND Tde_StartDate = @Tde_StartDate
                                            AND Tde_PayCycle = @Tde_PayCycle";

                ParameterInfo[] paramInfo = new ParameterInfo[6];
                paramInfo[paramIndex++] = new ParameterInfo("@Tde_IDNo", row["Tde_IDNo"], SqlDbType.VarChar, 15);
                paramInfo[paramIndex++] = new ParameterInfo("@Tde_DeductionCode", row["Tde_DeductionCode"], SqlDbType.VarChar, 10);
                paramInfo[paramIndex++] = new ParameterInfo("@Tde_StartDate", row["Tde_StartDate"], SqlDbType.Date);
                paramInfo[paramIndex++] = new ParameterInfo("@Tde_PayCycle", row["Tde_PayCycle"]);
                paramInfo[paramIndex++] = new ParameterInfo("@Tde_Remarks", row["Tde_Remarks"], SqlDbType.VarChar, 30);
                paramInfo[paramIndex++] = new ParameterInfo("@Usr_Login", row["Usr_Login"], SqlDbType.VarChar, 15);

                using (DALHelper dal = new DALHelper())
                {
                    dal.OpenDB();
                    retVal = dal.ExecuteNonQuery(UpdateString, CommandType.Text, paramInfo);
                    dal.CloseDB();

                }
                return retVal;
            }
            catch (Exception e)
            {
                //throw new PayrollException(e);
                return 0;
            }
        }

        public int DeleteRecord(string Tde_IDNo, string Tde_DeductionCode, object Tde_StartDate, string Tde_PayCycle)
        {
            int retVal = 0;
            int paramIndex = 0;

            string UpdateString = @"DELETE FROM T_EmpDeductionExempt
                                        WHERE Tde_IDNo = @Tde_IDNo
                                        AND Tde_DeductionCode = @Tde_DeductionCode
                                        AND Tde_StartDate = @Tde_StartDate
                                        AND Tde_PayCycle = @Tde_PayCycle";

            ParameterInfo[] paramInfo = new ParameterInfo[4];
            paramInfo[paramIndex++] = new ParameterInfo("@Tde_IDNo", Tde_IDNo);
            paramInfo[paramIndex++] = new ParameterInfo("@Tde_DeductionCode", Tde_DeductionCode);
            paramInfo[paramIndex++] = new ParameterInfo("@Tde_StartDate", Tde_StartDate);
            paramInfo[paramIndex++] = new ParameterInfo("@Tde_PayCycle", Tde_PayCycle);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    retVal = dal.ExecuteNonQuery(UpdateString, CommandType.Text, paramInfo);
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

        public DataSet FetchAllForEmpDeductExempt(string Tde_IDNo, string Tde_DeductionCode, object Tde_StartDate)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"SELECT Tps_PayCycle
                                    ,Convert(char(10), Tps_StartCycle, 101) as Tps_StartCycle
                                    ,Convert(char(10), Tps_EndCycle, 101) as Tps_EndCycle
                                    FROM T_PaySchedule
                                    WHERE (Tps_CycleIndicator = 'C'
                                    OR Tps_CycleIndicator = 'F')
                                    AND Tps_PayCycle not in 
                                    (SELECT Tde_PayCycle
                                     FROM T_EmpDeductionExempt
                                        INNER JOIN T_PaySchedule
                                            ON Tde_PayCycle = Tps_PayCycle
                                     WHERE Tde_IDNo = @Tde_IDNo
	                                    AND Tde_DeductionCode = @Tde_DeductionCode
	                                    AND Convert(char(10), Tde_StartDate, 101) = @Tde_StartDate
                                    )";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[3];
            paramInfo[0] = new ParameterInfo("@Tde_IDNo", Tde_IDNo);
            paramInfo[1] = new ParameterInfo("@Tde_DeductionCode", Tde_DeductionCode);
            paramInfo[2] = new ParameterInfo("@Tde_StartDate", Tde_StartDate);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);
                dal.CloseDB();
            }
            return ds;
        }

    }
}
