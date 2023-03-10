using System;
using System.Collections.Generic;
using System.Text;

using Payroll.DAL;
using CommonLibrary;
using System.Data;

namespace Payroll.BLogic
{
    public class EmployeeDeferredBL : BaseBL
    {

        #region <Override Functions>

        public override int Add(DataRow row)
        {
            int retVal = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[8];
            
            paramInfo[0] = new ParameterInfo("@Tdd_IDNo", row["Tdd_IDNo"], SqlDbType.VarChar, 15);
            paramInfo[1] = new ParameterInfo("@Tdd_DeductionCode", row["Tdd_DeductionCode"], SqlDbType.VarChar, 10);
            paramInfo[2] = new ParameterInfo("@Tdd_StartDate", row["Tdd_StartDate"], SqlDbType.Date);
            paramInfo[3] = new ParameterInfo("@Tdd_PayCycle", row["Tdd_PayCycle"], SqlDbType.Char, 7);
            paramInfo[4] = new ParameterInfo("@Tdd_LineNo", this.GetSeqNo(row["Tdd_IDNo"].ToString(),row["Tdd_DeductionCode"].ToString(),row["Tdd_StartDate"].ToString(),row["Tdd_PayCycle"].ToString()), SqlDbType.Char, 2);
            paramInfo[5] = new ParameterInfo("@Tdd_DeferredAmt", row["Tdd_DeferredAmt"]);
            paramInfo[6] = new ParameterInfo("@Tdd_Remarks", row["Tdd_Remarks"], SqlDbType.VarChar, 100);
            paramInfo[7] = new ParameterInfo("@Usr_Login", row["Usr_Login"], SqlDbType.VarChar, 15);
            
            #region query

            string qString = @"INSERT INTO T_EmpDeductionDefer
                                               (Tdd_IDNo
                                               ,Tdd_DeductionCode
                                               ,Tdd_StartDate
                                               ,Tdd_PayCycle
                                               ,Tdd_LineNo
                                               ,Tdd_DeferredAmt
                                               ,Tdd_Remarks
                                               ,Usr_Login
                                               ,Ludatetime)
                                         VALUES
                                               (@Tdd_IDNo
                                               ,@Tdd_DeductionCode
                                               ,@Tdd_StartDate
                                               ,@Tdd_PayCycle
                                               ,@Tdd_LineNo
                                               ,@Tdd_DeferredAmt
                                               ,@Tdd_Remarks
                                               ,@Usr_Login
                                               ,GETDATE())";

            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    retVal = dal.ExecuteNonQuery(qString, CommandType.Text, paramInfo);
                    // add Tdh_DeferredAmount  in T_EmpDeductionHdr
                    retVal = dal.ExecuteNonQuery(this.UpdateDeferredAmount(row), CommandType.Text);

                    dal.CommitTransactionSnapshot();
                }
                catch (Exception e)
                {
                    dal.RollBackTransactionSnapshot();
                    throw new PayrollException(e);
                }
                finally { dal.CloseDB(); }
            }

            return retVal;
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

        #region <Defined Functions>

        public DataSet FetchData(string Tdd_IDNo, string Tdd_DeductionCode, object Tdd_StartDate, string CentralProfile)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = string.Format(
                        @"SELECT '[' + Tdd_PayCycle + ']  ' + Convert(Char(10),Tps_StartCycle, 101) + ' - ' + Convert(Char(10),Tps_EndCycle, 101) as PayPeriod
	                                , Tdd_LineNo
	                                , Tdd_DeferredAmt
	                                , Left(Mur_UserFirstName,1) + Left(Mur_UserMiddleName,1) + RTRIM(Mur_UserLastName) as 'Updated by'
	                                , T_EmpDeductionDefer.Ludatetime as 'Last Update'
                                    , Tdd_PayCycle
                                    , Tdd_Remarks
                                FROM T_EmpDeductionDefer
	                                LEFT JOIN T_PaySchedule on  Tps_PayCycle = Tdd_PayCycle
	                                LEFT JOIN {0}..M_User On T_EmpDeductionDefer.Usr_Login = Mur_UserCode
                                WHERE Tdd_IDNo = @Tdd_IDNo
	                                AND Tdd_DeductionCode = @Tdd_DeductionCode
	                                AND Tdd_StartDate = @Tdd_StartDate
                                ORDER BY Tdd_PayCycle, Tdd_LineNo", CentralProfile);

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[3];
            paramInfo[0] = new ParameterInfo("@Tdd_IDNo", Tdd_IDNo, SqlDbType.VarChar, 15);
            paramInfo[1] = new ParameterInfo("@Tdd_DeductionCode", Tdd_DeductionCode, SqlDbType.VarChar, 10);
            paramInfo[2] = new ParameterInfo("@Tdd_StartDate", Tdd_StartDate, SqlDbType.Date);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);
                dal.CloseDB();
            }
            return ds;
        }

        public string GetLastUpdatedBy(string UserCode, string NAMEDSPLY, string CentralProfile)
        {
            DataSet ds = new DataSet();
            #region query
            string qString = string.Format(@"SELECT ISNULL(dbo.Udf_DisplayName('{0}','{1}'),'{0}') AS Usr_Login
                                            , GETDATE() AS Ludatetime", UserCode, NAMEDSPLY);
            #endregion
            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString);
                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0][0].ToString().Trim();
            else
                return string.Empty;
        }

        public string GetFormattedPayPeriod(string Tps_PayCycle)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"SELECT ' [' + Tps_PayCycle + ']  ' + Convert(Char(10),Tps_StartCycle, 101) + ' - ' + Convert(Char(10),Tps_EndCycle, 101) as PayPeriod
                                FROM T_PaySchedule
                                WHERE Tps_PayCycle = @Tps_PayCycle
                                    AND Tps_CycleIndicator = 'C'
                                    AND Tps_RecordStatus = 'A'";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Tps_PayCycle", Tps_PayCycle);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0][0].ToString().Trim();
            else
                return string.Empty;
        }

        private string UpdateDeferredAmount(DataRow row)
        {
            string[] param = new string[6];

            param[0] = row["Tdd_DeferredAmt"].ToString();
            param[1] = LoginInfo.getUser().UserCode;
            param[2] = row["Tdd_IDNo"].ToString();
            param[3] = row["Tdd_DeductionCode"].ToString();
            param[4] = row["Tdd_StartDate"].ToString();
            param[5] = LoginInfo.getUser().CentralProfileName;

            return string.Format(@"UPDATE {5}..T_EmpDeductionHdr
                            SET Tdh_DeferredAmount = Tdh_DeferredAmount + '{0}'
                                ,Usr_Login = '{1}'
	                            ,Ludatetime = GETDATE()
                            WHERE Tdh_IDNo = '{2}'
                            AND Tdh_DeductionCode = '{3}'
                            AND Tdh_StartDate = '{4}'", param);
        }

        private string UpdateDeferredAmountForDeletion(DataRow row)
        {
            string[] param = new string[6];

            param[0] = row["Tdd_DeferredAmt"].ToString();
            param[1] = LoginInfo.getUser().UserCode;
            param[2] = row["Tdd_IDNo"].ToString();
            param[3] = row["Tdd_DeductionCode"].ToString();
            param[4] = row["Tdd_StartDate"].ToString();
            param[5] = LoginInfo.getUser().CentralProfileName;

            return string.Format(@"UPDATE {5}..T_EmpDeductionHdr
                            SET Tdh_DeferredAmount = Tdh_DeferredAmount - '{0}'
                                ,Usr_Login = '{1}'
	                            ,Ludatetime = Getdate()
                            WHERE Tdh_IDNo = '{2}'
                            AND Tdh_DeductionCode = '{3}'
                            AND Tdh_StartDate = '{4}'", param);
        }

        public int DeleteRecord(DataRow row)
        {
            int retVal = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[5];

            paramInfo[0] = new ParameterInfo("@Tdd_IDNo", row["Tdd_IDNo"], SqlDbType.VarChar, 15);
            paramInfo[1] = new ParameterInfo("@Tdd_DeductionCode", row["Tdd_DeductionCode"], SqlDbType.VarChar, 10);
            paramInfo[2] = new ParameterInfo("@Tdd_StartDate", row["Tdd_StartDate"], SqlDbType.Date);
            paramInfo[3] = new ParameterInfo("@Tdd_PayCycle", row["Tdd_PayCycle"], SqlDbType.Char, 7);
            paramInfo[4] = new ParameterInfo("@Tdd_LineNo", row["Tdd_LineNo"], SqlDbType.Char, 2);
            
            #region query

            string qString = @"DELETE FROM T_EmpDeductionDefer
                                WHERE Tdd_IDNo = @Tdd_IDNo
                                   AND Tdd_DeductionCode = @Tdd_DeductionCode
                                   AND Tdd_StartDate = @Tdd_StartDate
                                   AND Tdd_PayCycle = @Tdd_PayCycle
                                   AND Tdd_LineNo = @Tdd_LineNo";

            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    retVal = dal.ExecuteNonQuery(qString, CommandType.Text, paramInfo);
                    retVal = dal.ExecuteNonQuery(this.UpdateDeferredAmountForDeletion(row), CommandType.Text);

                    dal.CommitTransactionSnapshot();
                }
                catch (Exception e)
                {
                    dal.RollBackTransactionSnapshot();
                    throw new PayrollException(e);
                }
                finally { dal.CloseDB(); }
            }

            return retVal;
        }

        public string GetMaxAllowable(string Tdh_IDNo, string Tdh_DeductionCode, object Tdh_StartDate, string CentralProfile)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"SELECT (Tdh_DeductionAmount - Tdh_PaidAmount) - Tdh_DeferredAmount as 'MaxAllowable'
                                FROM T_EmpDeductionHdr
                                WHERE Tdh_IDNo = @Tdh_IDNo
                                AND Tdh_DeductionCode = @Tdh_DeductionCode
                                AND Tdh_StartDate = @Tdh_StartDate";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[3];
            paramInfo[0] = new ParameterInfo("@Tdh_IDNo", Tdh_IDNo, SqlDbType.VarChar, 15);
            paramInfo[1] = new ParameterInfo("@Tdh_DeductionCode", Tdh_DeductionCode, SqlDbType.VarChar, 10);
            paramInfo[2] = new ParameterInfo("@Tdh_StartDate", Tdh_StartDate, SqlDbType.Date);

            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0][0].ToString().Trim();
            else
                return string.Empty;
        }

        #endregion

        #region <Function Added>

        public string GetSeqNo(string Tdd_IDNo, string Tdd_DeductionCode, string Tdd_StartDate, string Tdd_PayCycle)
        {
            object t = new object();

            #region query

            string qString = @"Select ISNULL(MAX(Tdd_LineNo),00)
                                From T_EmpDeductionDefer
                                Where Tdd_IDNo = @Tdd_IDNo
                                And Tdd_DeductionCode = @Tdd_DeductionCode
                                And Tdd_StartDate = @Tdd_StartDate
                                And Tdd_PayCycle = @Tdd_PayCycle";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[4];

            paramInfo[0] = new ParameterInfo("@Tdd_IDNo", Tdd_IDNo, SqlDbType.VarChar, 15);
            paramInfo[1] = new ParameterInfo("@Tdd_DeductionCode", Tdd_DeductionCode, SqlDbType.VarChar, 10);
            paramInfo[2] = new ParameterInfo("@Tdd_StartDate", Tdd_StartDate, SqlDbType.Date);
            paramInfo[3] = new ParameterInfo("@Tdd_PayCycle", Tdd_PayCycle, SqlDbType.Char, 7);
            
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                t = dal.ExecuteScalar(qString, CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            return string.Format("{0:0#}", Convert.ToInt32(t.ToString()) + 1);
        }

        #endregion

    }
}
