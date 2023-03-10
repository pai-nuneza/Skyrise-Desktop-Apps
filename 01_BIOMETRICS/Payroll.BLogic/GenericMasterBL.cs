using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using CommonLibrary;
using Payroll.DAL;

namespace Payroll.BLogic
{
    public class GenericMasterBL : BaseBL
    {
        public override int Add(DataRow row)
        {
            int retVal = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[5];
            paramInfo[0] = new ParameterInfo("@gmt_Genericcode", row["Gmt_Genericcode"], SqlDbType.NVarChar, 6);
            paramInfo[1] = new ParameterInfo("@gmt_GenericType", row["Gmt_GenericType"], SqlDbType.NVarChar, 1);
            paramInfo[2] = new ParameterInfo("@gmt_GenericDesc", row["Gmt_GenericDesc"], SqlDbType.NVarChar, 35);
            paramInfo[3] = new ParameterInfo("@gmt_status", row["Gmt_status"], SqlDbType.NVarChar, 1);
            paramInfo[4] = new ParameterInfo("@user_login", row["User_login"], SqlDbType.NVarChar, 15);

            string sqlQuery = "INSERT INTO T_GenericMaster VALUES(@gmt_Genericcode,@gmt_GenericType, @gmt_GenericDesc, @gmt_status, @user_login, GETDATE())";

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

        public override int Update(DataRow row)
        {
            int retVal = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[5];
            paramInfo[0] = new ParameterInfo("@gmt_Genericcode", row["Gmt_Genericcode"], SqlDbType.NVarChar, 6);
            paramInfo[1] = new ParameterInfo("@gmt_GenericType", row["Gmt_GenericType"], SqlDbType.NVarChar, 1);
            paramInfo[2] = new ParameterInfo("@gmt_GenericDesc", row["Gmt_GenericDesc"], SqlDbType.NVarChar, 35);
            paramInfo[3] = new ParameterInfo("@gmt_status", row["Gmt_status"], SqlDbType.NVarChar, 1);
            paramInfo[4] = new ParameterInfo("@user_login", row["User_login"], SqlDbType.NVarChar, 15);

            string sqlQuery = "INSERT INTO T_GenericMaster VALUES(@gmt_Genericcode,@gmt_GenericType, @gmt_GenericDesc, @gmt_status, @user_login, GETDATE())";

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

        public override int Delete(string code, string userLogin)
        {
            int retVal = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@gmt_Genericcode", code, SqlDbType.NVarChar, 6);
            paramInfo[1] = new ParameterInfo("@user_login", userLogin, SqlDbType.NVarChar, 7);

            string sqlQuery = "UPDATE T_GenericMaster SET Gmt_status = 'C', User_login = @user_login, ludatetime = GETDATE() WHERE Gmt_Genericcode = @gmt_Genericcode";

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

        public override DataRow Fetch(string code)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@gmt_Genericcode", code, SqlDbType.NVarChar, 6);

            string sqlQuery = "SELECT * FROM T_GenericMaster WHERE Gmt_Genericcode = @gmt_Genericcode";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0];
            else
                return null;
        }

        public DataRow FetchHumidity(string code)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@gmt_Genericcode", code, SqlDbType.NVarChar, 6);

            string sqlQuery = "SELECT * FROM T_GenericMaster WHERE Gmt_Genericcode = @gmt_Genericcode AND Gmt_GenericType = 'H' and Gmt_Status = 'A'";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0];
            else
                return null;
        }

        public DataRow FetchPiling(string code)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@gmt_Genericcode", code, SqlDbType.NVarChar, 6);

            string sqlQuery = "SELECT * FROM T_GenericMaster WHERE Gmt_Genericcode = @gmt_Genericcode AND Gmt_GenericType = 'P' and Gmt_Status = 'A'";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0];
            else
                return null;
        }

        public override DataSet FetchAll()
        {
            DataSet ds = new DataSet();

            string sqlQuery = @"SELECT 
                                [Gmt_Genericcode] Code,
                                [Gmt_GenericType] Type,
                                [Gmt_GenericDesc] Description,
                                [Gmt_status] Status,
                                [User_login] Userlogin
                                FROM T_GenericMaster
                                ORDER BY ludatetime DESC ";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text);

                dal.CloseDB();
            }
            return ds;
        }

        public int checkGeneric(string stat, string code)
        {
            DataSet ds = new DataSet();
            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@genCode", code, SqlDbType.NVarChar, 6);
            
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                if( stat.Equals("H"))
                    ds =  dal.ExecuteDataSet(CommonConstants.Queries.checkHumidity, CommandType.Text, paramInfo);
                else if (stat.Equals("P"))
                    ds =  dal.ExecuteDataSet(CommonConstants.Queries.checkPilingHeight, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            return Convert.ToInt32(ds.Tables[0].Rows[0][0].ToString());
        }

        public DataRow FetchCode(string code)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@gmt_Genericcode", code, SqlDbType.NVarChar, 6);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(CommonConstants.Queries.checkGenericeExist, CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0];
            else
                return null;
        }
    }
}
