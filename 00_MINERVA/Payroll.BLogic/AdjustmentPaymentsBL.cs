using System;
using Payroll.DAL;
using CommonLibrary;
using System.Data;

namespace Payroll.BLogic
{
    public class AdjustmentPaymentsBL : BaseBL
    {
        public override int Add(System.Data.DataRow row)
        {
            int retVal = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[7];

            paramInfo[0] = new ParameterInfo("@Tdd_IDNo", row["Tdd_IDNo"], SqlDbType.VarChar, 15);
            paramInfo[1] = new ParameterInfo("@Tdd_DeductionCode", row["Tdd_DeductionCode"], SqlDbType.VarChar, 10);
            paramInfo[2] = new ParameterInfo("@Tdd_StartDate", row["Tdd_StartDate"], SqlDbType.Date);
            paramInfo[3] = new ParameterInfo("@Tdd_PayCycle", row["Tdd_PayCycle"], SqlDbType.Char, 7);
            paramInfo[4] = new ParameterInfo("@Tdd_LineNo", row["Tdd_LineNo"], SqlDbType.Char, 2);
            paramInfo[5] = new ParameterInfo("@Tdd_DeferredAmt", row["Tdd_DeferredAmt"]);
            paramInfo[6] = new ParameterInfo("@Usr_Login", row["Usr_Login"], SqlDbType.VarChar, 15);

            #region query

            string qString = @"INSERT INTO T_EmpDeductionDefer
                                               (Tdd_IDNo
                                               ,Tdd_DeductionCode
                                               ,Tdd_StartDate
                                               ,Tdd_PayCycle
                                               ,Tdd_LineNo
                                               ,Tdd_DeferredAmt
                                               ,Usr_Login
                                               ,Ludatetime)
                                         VALUES
                                               (@Tdd_IDNo
                                               ,@Tdd_DeductionCode
                                               ,@Tdd_StartDate
                                               ,@Tdd_PayCycle
                                               ,@Tdd_LineNo
                                               ,@Tdd_DeferredAmt
                                               ,@Usr_Login
                                               ,Getdate())";

            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    retVal = dal.ExecuteNonQuery(qString, CommandType.Text, paramInfo);
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

        public override int Update(System.Data.DataRow row)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override int Delete(string code, string userLogin)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override System.Data.DataSet FetchAll()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override System.Data.DataRow Fetch(string code)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public DataSet FetchData(string Tdd_IDNo, string Tdd_DeductionCode, object Tdd_StartDate, string CompanyCode, string CentralProfile)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = string.Format(@"
                            SELECT a.Tdd_ThisPayCycle, a.Tdd_PayCycle, MAX(a.Tdd_Amount) + SUM(ISNULL(b.Tdd_Amount,0)) AS [Amount]
                                ,a.Tdd_DeferredFlag, DEDPAYTYPE.Mcd_Name AS Tdd_PaymentType, '' AS Tdd_Remarks, a.Tdd_LineNo
                            FROM T_EmpDeductionDtlHst a
                            LEFT JOIN T_EmpDeductionDtlHst b ON b.Tdd_IDNo = a.Tdd_IDNo 
	                            AND b.Tdd_DeductionCode = a.Tdd_DeductionCode
	                            AND b.Tdd_StartDate = a.Tdd_StartDate
	                            AND b.Tdd_ThisPayCycle = a.Tdd_ThisPayCycle
	                            AND b.Tdd_PayCycle = a.Tdd_PayCycle
	                            AND b.Tdd_RefLineNo = a.Tdd_LineNo
	                            AND b.Tdd_PaymentFlag = 1
	                            AND b.Tdd_PaymentType = 'S'
                            LEFT JOIN {0}..M_CodeDtl DEDPAYTYPE ON DEDPAYTYPE.Mcd_Code = a.Tdd_PaymentType 
	                            AND DEDPAYTYPE.Mcd_CodeType = 'DEDPAYTYPE'
                                AND DEDPAYTYPE.Mcd_CompanyCode = @CompanyCode
                            WHERE a.Tdd_IDNo = @Tdd_IDNo
                              AND a.Tdd_DeductionCode= @Tdd_DeductionCode
                              AND a.Tdd_StartDate = @Tdd_StartDate
                              AND a.Tdd_PaymentType IN ('A','D','X')
                              AND a.Tdd_PaymentFlag = 1
                            GROUP BY a.Tdd_ThisPayCycle, a.Tdd_PayCycle, a.Tdd_LineNo, a.Tdd_DeferredFlag, DEDPAYTYPE.Mcd_Name, a.Tdd_LineNo
                            HAVING  MAX(a.Tdd_Amount) + SUM(ISNULL(b.Tdd_Amount,0)) <> 0"
                            , CentralProfile);

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[4];
            paramInfo[0] = new ParameterInfo("@Tdd_IDNo", Tdd_IDNo, SqlDbType.VarChar, 15);
            paramInfo[1] = new ParameterInfo("@Tdd_DeductionCode", Tdd_DeductionCode, SqlDbType.VarChar, 10);
            paramInfo[2] = new ParameterInfo("@Tdd_StartDate", Tdd_StartDate, SqlDbType.Date); 
            paramInfo[3] = new ParameterInfo("@CompanyCode", CompanyCode, SqlDbType.VarChar, 10);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            return ds;
        }

        public DataSet GetPaidAndDeferredAmount(string Tdd_IDNo, string Tdd_DeductionCode, object Tdd_StartDate, string CentralProfile)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"SELECT  Tdh_PaidAmount 
									, Tdh_DeferredAmount
	                            FROM  T_EmpDeductionHdr
                                WHERE Tdh_IDNo = @Tdd_IDNo
                                AND Tdh_DeductionCode = @Tdd_DeductionCode
                                AND Tdh_StartDate = @Tdd_StartDate";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[3];
            paramInfo[0] = new ParameterInfo("@Tdd_IDNo", Tdd_IDNo, SqlDbType.VarChar, 15);
            paramInfo[1] = new ParameterInfo("@Tdd_DeductionCode", Tdd_DeductionCode, SqlDbType.VarChar, 10);
            paramInfo[2] = new ParameterInfo("@Tdd_StartDate", Tdd_StartDate, SqlDbType.Date);

            using (DALHelper dal = new DALHelper(CentralProfile, false))
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            return ds;
        }

        public string GetSeqNo(string Tdd_IDNo, string Tdd_DeductionCode, object Tdd_StartDate, string Tdd_ThisPayCycle, string Tdd_PayCycle)
        {
            object t = new object();

            #region query

            string qString = @"SELECT ISNULL(max(Tdd_LineNo),00)
                                 FROM T_EmpDeductionDtlHst
                                WHERE Tdd_IDNo = @Tdd_IDNo
                                    AND Tdd_DeductionCode = @Tdd_DeductionCode
                                    AND Tdd_StartDate = @Tdd_StartDate
                                    AND Tdd_ThisPayCycle = @Tdd_ThisPayCycle
                                    AND Tdd_PayCycle = @Tdd_PayCycle";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[5];

            paramInfo[0] = new ParameterInfo("@Tdd_IDNo", Tdd_IDNo, SqlDbType.VarChar, 15);
            paramInfo[1] = new ParameterInfo("@Tdd_DeductionCode", Tdd_DeductionCode, SqlDbType.VarChar, 10);
            paramInfo[2] = new ParameterInfo("@Tdd_StartDate", Tdd_StartDate, SqlDbType.Date);
            paramInfo[3] = new ParameterInfo("@Tdd_ThisPayCycle", Tdd_ThisPayCycle, SqlDbType.Char, 7);
            paramInfo[4] = new ParameterInfo("@Tdd_PayCycle", Tdd_PayCycle, SqlDbType.Char, 7);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                t = dal.ExecuteScalar(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }

            return string.Format("{0:0#}", Convert.ToInt32(t.ToString()) + 1);
        }

        private string UpdateDeferredAmount(DataRow row)
        {
            string[] param = new string[6];

            param[0] = Convert.ToString(Convert.ToDecimal(row["Tdd_DeferredAmt"].ToString()) * Convert.ToDecimal("-1.00"));
            param[1] = LoginInfo.getUser().UserCode;
            param[2] = row["Tdd_IDNo"].ToString();
            param[3] = row["Tdd_DeductionCode"].ToString();
            param[4] = row["Tdd_StartDate"].ToString();
            param[5] = LoginInfo.getUser().CentralProfileName;

            return string.Format(@"UPDATE {5}..T_EmpDeductionHdr
                            SET Tdh_DeferredAmount = Tdh_DeferredAmount - '{0}'
                                ,Usr_Login = '{1}'
	                            ,Ludatetime = GETDATE()
                            WHERE Tdh_IDNo = '{2}'
                            AND Tdh_DeductionCode = '{3}'
                            AND Tdh_StartDate = '{4}'", param);
        }

        private string UpdatePayedAmount(DataRow row)
        {
            string[] param = new string[6];

            param[0] = row["Tdd_Amount"].ToString();
            param[1] = LoginInfo.getUser().UserCode;
            param[2] = row["Tdd_IDNo"].ToString();
            param[3] = row["Tdd_DeductionCode"].ToString();
            param[4] = row["Tdd_StartDate"].ToString();
            param[5] = LoginInfo.getUser().CentralProfileName;

            return string.Format(@"UPDATE {5}..T_EmpDeductionHdr
                            SET Tdh_PaidAmount = Tdh_PaidAmount + '{0}'
                                ,Usr_Login = '{1}'
	                            ,Ludatetime = GETDATE()
                            WHERE Tdh_IDNo = '{2}'
                            AND Tdh_DeductionCode = '{3}'
                            AND Tdh_StartDate = '{4}'", param);
        }

        private string InsertDeductionDetailHist(DataRow row)
        {
            string[] param = new string[12];

            param[0] = row["Tdd_IDNo"].ToString();
            param[1] = row["Tdd_DeductionCode"].ToString();
            param[2] = row["Tdd_StartDate"].ToString();
            param[3] = row["Tdd_ThisPayCycle"].ToString();
            param[4] = row["Tdd_PayCycle"].ToString();
            param[5] = row["Tdd_LineNo"].ToString();
            param[6] = row["Tdd_Amount"].ToString();
            param[7] = row["Tdd_DeferredFlag"].ToString();
            param[8] = row["Tdd_PaymentFlag"].ToString();
            param[9] = row["Tdd_Remarks"].ToString();
            param[10] = row["Tdd_RefLineNo"].ToString();
            param[11] = row["Usr_Login"].ToString();
            

            return string.Format(@"INSERT INTO T_EmpDeductionDtlHst
                                   (Tdd_IDNo
                                   ,Tdd_DeductionCode
                                   ,Tdd_StartDate
                                   ,Tdd_ThisPayCycle
                                   ,Tdd_PayCycle
                                   ,Tdd_LineNo
                                   ,Tdd_PaymentType
                                   ,Tdd_Amount
                                   ,Tdd_DeferredFlag
                                   ,Tdd_PaymentFlag
                                   ,Tdd_Remarks
                                   ,Tdd_RefLineNo
                                   ,Usr_Login
                                   ,Ludatetime)
                             VALUES
                                   ('{0}'
                                   ,'{1}'
                                   ,'{2}'
                                   ,'{3}'
                                   ,'{4}'
                                   ,'{5}'
                                   ,'S'
                                   ,'{6}'
                                   ,'{7}'
                                   ,'{8}'
                                   ,'{9}'
                                   ,'{10}'
                                   ,'{11}'
                                   ,GETDATE())", param);
        }

        public int AddRecord(DataRow row, DataRow rowDetailHist, bool FromDeferred, string CentralProfile)
        {
            int retVal = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[8];

            paramInfo[0] = new ParameterInfo("@Tdd_IDNo", row["Tdd_IDNo"], SqlDbType.VarChar, 15);
            paramInfo[1] = new ParameterInfo("@Tdd_DeductionCode", row["Tdd_DeductionCode"], SqlDbType.VarChar, 10);
            paramInfo[2] = new ParameterInfo("@Tdd_StartDate", row["Tdd_StartDate"], SqlDbType.Date);
            paramInfo[3] = new ParameterInfo("@Tdd_PayCycle", row["Tdd_PayCycle"], SqlDbType.Char, 7);
            paramInfo[4] = new ParameterInfo("@Tdd_LineNo", row["Tdd_LineNo"], SqlDbType.Char, 2);
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
                                               ,Getdate())";

            #endregion

            DALHelper dal = new DALHelper();

            dal.OpenDB();
            dal.BeginTransactionSnapshot();

            try
            {
                if (FromDeferred)
                {
                    retVal = dal.ExecuteNonQuery(qString, CommandType.Text, paramInfo);
                    // subtract Tdh_DeferredAmount  in T_EmpDeductionHdr 
                    retVal = dal.ExecuteNonQuery(this.UpdateDeferredAmount(row), CommandType.Text);
                }

                retVal = dal.ExecuteNonQuery(this.InsertDeductionDetailHist(rowDetailHist), CommandType.Text);
                // add Tdh_PaidAmount  in T_EmpDeductionHdr
                retVal = dal.ExecuteNonQuery(this.UpdatePayedAmount(rowDetailHist), CommandType.Text);

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

            return retVal;
        }

        public string GetSeqNoDeferred(string Tdd_IDNo, string Tdd_DeductionCode, object Tdd_StartDate, string Tdd_PayCycle)
        {
            object t = new object();

            #region query

            string qString = @"SELECT ISNULL(max(Tdd_LineNo),00)
                                 FROM T_EmpDeductionDefer
                                WHERE Tdd_IDNo = @Tdd_IDNo
                                AND Tdd_DeductionCode = @Tdd_DeductionCode
                                AND Tdd_StartDate = @Tdd_StartDate
                                AND Tdd_PayCycle = @Tdd_PayCycle";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[4];

            paramInfo[0] = new ParameterInfo("@Tdd_IDNo", Tdd_IDNo, SqlDbType.VarChar, 15);
            paramInfo[1] = new ParameterInfo("@Tdd_DeductionCode", Tdd_DeductionCode, SqlDbType.VarChar, 10);
            paramInfo[2] = new ParameterInfo("@Tdd_StartDate", Tdd_StartDate, SqlDbType.Date);
            paramInfo[3] = new ParameterInfo("@Tdd_PayCycle", Tdd_PayCycle, SqlDbType.VarChar, 7);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                t = dal.ExecuteScalar(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }

            return string.Format("{0:0#}", Convert.ToInt32(t.ToString()) + 1);
        }

    }
}
