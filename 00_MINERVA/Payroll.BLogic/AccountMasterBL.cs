using System;
using System.Data;
using Payroll.DAL;
using CommonLibrary;

namespace Payroll.BLogic
{
    public class frmAccountMasterBL : BaseBL
    {
        #region Overrides
            public override int Add(System.Data.DataRow row)
            {
                int retVal = 0;

                
                return retVal;
            }
            public override int Update(System.Data.DataRow row)
            {
                int retVal = 0;
                return retVal;
            }
            public override int Delete(string deductcode, string Userlogin)
            {
                int retVal = 0;

                return retVal;
            }
            public override System.Data.DataSet FetchAll()
            {
                DataSet ds = new DataSet();

                using (DALHelper dal = new DALHelper())
                {
                    dal.OpenDB();

                    string sqlQuery = @"SELECT Mch_CodeType,
                                                     Mch_TypeName,
                                                     Mch_CodeLength,
                                                     Mch_RecordStatus
                                              from M_CodeHdr";

                    ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text);

                    dal.CloseDB();
                }
                return ds;
            }

            public override DataRow Fetch(string code)
            {
                DataSet ds = new DataSet();
                ParameterInfo[] paramInfo = new ParameterInfo[1];
                paramInfo[0] = new ParameterInfo("@accounttype", code, SqlDbType.Char, 10);

                string sqlQuery = @"Select 
                                         Mch_TypeName,
                                         Mch_CodeLength,
                                         Mch_RecordStatus
                                      from M_CodeHdr
                                      where Mch_CodeType=@accounttype";

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
    #endregion

        #region User-Defined
        #region Header
        public int HeaderUpdate(System.Data.DataRow row)
        {
            int retVal = 0;
            ParameterInfo[] paramInfo = new ParameterInfo[5];
            paramInfo[0] = new ParameterInfo("@accounttype", row["Mch_CodeType"], SqlDbType.Char, 10);
            paramInfo[1] = new ParameterInfo("@accountname", row["Mch_TypeName"], SqlDbType.Char, 50);
            paramInfo[2] = new ParameterInfo("@charlength",row["Mch_CodeLength"], SqlDbType.TinyInt);
            paramInfo[3] = new ParameterInfo("@status", row["Mch_RecordStatus"], SqlDbType.Char, 1);
            paramInfo[4] = new ParameterInfo("@Userlogin", row["Usr_login"], SqlDbType.Char, 15);

            string sqlQuery = @"UPDATE M_CodeHdr
                                                   SET 
                                                      Mch_TypeName=@accountname,
                                                      Mch_CodeLength=@charlength,
                                                      Mch_RecordStatus=@status,
                                                      Usr_Login = @Userlogin, 
                                                      ludatetime = GetDate()
                                                   WHERE Mch_CodeType=@accounttype";

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

        public int HeaderDelete(string accounttype, string Userlogin)
        {
            int retVal = 0;
            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@accounttype", accounttype, SqlDbType.Char, 10);
            paramInfo[1] = new ParameterInfo("@Userlogin", Userlogin, SqlDbType.Char, 15);

            string sqlQuery = @"UPDATE M_CodeHdr
                                                   SET 
                                                      Mch_RecordStatus='C',
                                                      Usr_Login = @Userlogin, 
                                                      ludatetime = GetDate()
                                                   WHERE Mch_CodeType=@accounttype";

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

        public int HeaderAdd(System.Data.DataRow row)
        {
            int retVal = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[5];
            paramInfo[0] = new ParameterInfo("@AccountType", row["Mch_CodeType"], SqlDbType.Char, 10);
            paramInfo[1] = new ParameterInfo("@AccountName", row["Mch_TypeName"], SqlDbType.Char, 50);
            paramInfo[2] = new ParameterInfo("@Status", row["Mch_RecordStatus"], SqlDbType.Char, 1);
            paramInfo[3] = new ParameterInfo("@Charlength", row["Mch_CodeLength"], SqlDbType.TinyInt);
            paramInfo[4] = new ParameterInfo("@Usr_Login", row["Usr_Login"], SqlDbType.Char, 15);

            string sqlQuery = @"INSERT INTO M_CodeHdr
                                                (Mch_CodeType,
                                                 Mch_TypeName,
                                                 Mch_RecordStatus,
                                                 Mch_CodeLength,
                                                 Usr_Login,
                                                  ludatetime) 
                                               VALUES
                                                (@AccountType,
                                                 @AccountName,
                                                 @Status,
                                                 @Charlength,
                                                 @Usr_Login, 
                                                 GetDate())";

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

        #endregion

        #region Detail

        public int DetailUpdate(System.Data.DataRow row)
        {
            int retVal = 0;
            ParameterInfo[] paramInfo = new ParameterInfo[4];
            paramInfo[0] = new ParameterInfo("@accountcode", row["Mcd_Code"], SqlDbType.Char, 10);
            paramInfo[1] = new ParameterInfo("@accountdesc", row["Mcd_Name"], SqlDbType.Char, 50);
            paramInfo[2] = new ParameterInfo("@Userlogin", row["Usr_login"], SqlDbType.Char, 15);
            paramInfo[3] = new ParameterInfo("@status", row["Mcd_RecordStatus"], SqlDbType.Char, 1);

            string sqlQuery = @"UPDATE M_CodeDtl
                                                   SET 
                                                      Mcd_Name=@accountdesc,
                                                      Mcd_RecordStatus=@status,
                                                      Usr_Login = @Userlogin, 
                                                      ludatetime = GetDate()
                                                   WHERE Mcd_Code=@accountcode";

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

        public int DetailDelete(string accountcode, string Userlogin)
        {
            int retVal = 0;
            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@accountcode", accountcode, SqlDbType.Char, 10);
            paramInfo[1] = new ParameterInfo("@Userlogin", Userlogin, SqlDbType.Char, 15);

            string sqlQuery = @"UPDATE M_CodeDtl
                                                   SET 
                                                      Mcd_RecordStatus='C',
                                                      Usr_Login = @Userlogin, 
                                                      ludatetime = GetDate()
                                                   WHERE Mcd_Code=@accountcode";

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

        public int DetailAdd(System.Data.DataRow row)
        {
            int retVal = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[5];
            paramInfo[0] = new ParameterInfo("@AccountType", row["Mcd_CodeType"], SqlDbType.Char, 10);
            paramInfo[1] = new ParameterInfo("@AccountCode", row["Mcd_Code"], SqlDbType.Char, 10);
            paramInfo[2] = new ParameterInfo("@AccountDesc", row["Mcd_Name"], SqlDbType.Char, 50);
            paramInfo[3] = new ParameterInfo("@Status", row["Mcd_RecordStatus"], SqlDbType.Char, 1);
            paramInfo[4] = new ParameterInfo("@Usr_Login", row["Usr_Login"], SqlDbType.Char, 15);

            string sqlQuery = @"INSERT INTO M_CodeDtl
                                                (Mcd_CodeType,
                                                 Mcd_Code,
                                                 Mcd_Name,
                                                 Mcd_RecordStatus,
                                                 Usr_Login,
                                                  ludatetime) 
                                               VALUES
                                                (@AccountType,
                                                 @AccountCode,
                                                 @AccountDesc,
                                                 @Status,
                                                 @Usr_Login, 
                                                 GetDate())";

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
        #endregion

        #region fetch
        public System.Data.DataSet Fetchallmember(string accounttype)
        {
            DataSet ds = new DataSet();
            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@accounttype", accounttype, SqlDbType.Char, 10);

            string sqlQuery = @"Select Mcd_Code,
                                                     Mcd_Name,
                                                     Mcd_RecordStatus
                                              from M_CodeDtl
                                              where Mcd_CodeType=@accounttype;";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            return ds;
        }

        public System.Data.DataRow ifAccountTypeExist(string accounttype)
        {
            DataSet ds = new DataSet();

            using (DALHelper dal = new DALHelper())
            {
                ParameterInfo[] paramCollection = new ParameterInfo[1];
                paramCollection[0] = new ParameterInfo("@accounttype", accounttype, SqlDbType.Char, 10);
                dal.OpenDB();
                ds = dal.ExecuteDataSet("SELECT Mch_CodeType FROM M_CodeHdr WHERE Mch_CodeType=@accounttype", CommandType.Text, paramCollection);
                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0];
            else
                return null;

        }

        public System.Data.DataRow ifAccountCodeExist(string accountcode)
        {
            DataSet ds = new DataSet();

            using (DALHelper dal = new DALHelper())
            {
                ParameterInfo[] paramCollection = new ParameterInfo[1];
                paramCollection[0] = new ParameterInfo("@accountcode", accountcode, SqlDbType.Char, 10);
                dal.OpenDB();
                ds = dal.ExecuteDataSet("SELECT Mcd_Code FROM M_CodeDtl WHERE Mcd_Code=@accountcode", CommandType.Text, paramCollection);
                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0];
            else
                return null;

        }

        public System.Data.DataRow ifAccountNameExist(string accountname, string code)
        {
            DataSet ds = new DataSet();

            using (DALHelper dal = new DALHelper())
            {
                ParameterInfo[] paramCollection = new ParameterInfo[2];
                paramCollection[0] = new ParameterInfo("@accountname", accountname, SqlDbType.Char, 10);
                paramCollection[1] = new ParameterInfo("@code", code, SqlDbType.Char, 10);
                dal.OpenDB();
                ds = dal.ExecuteDataSet("SELECT Mch_TypeName FROM M_CodeHdr WHERE Mch_TypeName=@accountname AND Mch_CodeType<>@code", CommandType.Text, paramCollection);
                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0];
            else
                return null;

        }    

        public System.Data.DataRow ifAccountDescExist(string accountdesc,string code, string type)
        {
            DataSet ds = new DataSet();

            using (DALHelper dal = new DALHelper())
            {
                ParameterInfo[] paramCollection = new ParameterInfo[3];
                paramCollection[0] = new ParameterInfo("@accountdesc", accountdesc, SqlDbType.Char, 50);
                paramCollection[1] = new ParameterInfo("@code", code, SqlDbType.Char, 10);
                paramCollection[2] = new ParameterInfo("@type", type, SqlDbType.Char, 10);
                dal.OpenDB();
                ds = dal.ExecuteDataSet("SELECT Mcd_Name FROM M_CodeDtl WHERE Mcd_Name=@accountdesc AND Mcd_Code<>@code AND Mcd_CodeType=@type", CommandType.Text, paramCollection);
                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0];
            else
                return null;

        }    
    
        public System.Data.DataRow fetchactivemember(string accounttype)
        {
            DataSet ds = new DataSet();

            using (DALHelper dal = new DALHelper())
            {
                ParameterInfo[] paramCollection = new ParameterInfo[1];
                paramCollection[0] = new ParameterInfo("@accounttype", accounttype, SqlDbType.Char, 10);
                dal.OpenDB();
                ds = dal.ExecuteDataSet("SELECT Mcd_Code FROM M_CodeDtl WHERE Mcd_CodeType=@accounttype AND Mcd_RecordStatus like 'A'", CommandType.Text, paramCollection);
                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0];
            else
                return null;
        }
        public int countallmembers(string accounttype)
        {
            DataSet ds = new DataSet();

            using (DALHelper dal = new DALHelper())
            {
                ParameterInfo[] paramCollection = new ParameterInfo[1];
                paramCollection[0] = new ParameterInfo("@accounttype", accounttype, SqlDbType.Char, 10);
                dal.OpenDB();
                ds = dal.ExecuteDataSet("SELECT Mcd_Code FROM M_CodeDtl WHERE Mcd_CodeType=@accounttype", CommandType.Text, paramCollection);
                dal.CloseDB();
            }

            return ds.Tables[0].Rows.Count;

        }
        #endregion
        #endregion

        #region <For Account Header New Design>

        public DataSet FetchAllinHeader()
        {
            DataSet ds = new DataSet();

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet("SELECT Mch_CodeType,Mch_TypeName,Mch_CodeLength, CASE WHEN Mch_RecordStatus = 'A' THEN 'ACTIVE'WHEN Mch_RecordStatus = 'C' THEN 'CANCELLED' END as 'Mch_RecordStatus'  FROM M_CodeHdr WHERE Mch_RecordStatus = 'A'", CommandType.Text);

                dal.CloseDB();
            }
            return ds;
        }

        public int DeleteHeader(string Mch_CodeType)
        {
            int retVal = 0;

            #region <query>
            string qString = @"UPDATE M_CodeHdr
                                    SET Mch_RecordStatus = 'C'
	                                    ,Usr_Login = @Usr_Login
	                                    ,Ludatetime = Getdate()
                                    Where Mch_CodeType = @Mch_CodeType";
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];

            paramInfo[0] = new ParameterInfo("@Mch_CodeType", Mch_CodeType);
            paramInfo[1] = new ParameterInfo("@Usr_Login", LoginInfo.getUser().UserCode);

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

        public bool CheckIfAccntTypeExistsinDetail(string Mcd_CodeType)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Mcd_CodeType", Mcd_CodeType);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet("SELECT * FROM M_CodeDtl WHERE Mcd_CodeType = @Mcd_CodeType AND Mcd_RecordStatus = 'A'", CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            if (ds.Tables[0].Rows.Count > 0)
                return true;
            else
                return false;
        }

        public bool CheckifAccountTypeExists(string Mch_CodeType)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Mch_CodeType", Mch_CodeType);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet("SELECT * FROM M_CodeHdr WHERE Mch_CodeType = @Mch_CodeType", CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            if (ds.Tables[0].Rows.Count > 0)
                return true;
            else
                return false;
        }

        public int AddHeader(DataRow row)
        {
            int retVal = 0;

            #region query
            string qString = @"INSERT INTO M_CodeHdr
                                           (Mch_CodeType
                                           ,Mch_TypeName
                                           ,Mch_CodeLength
                                           ,Mch_RecordStatus
                                           ,Usr_Login
                                           ,Ludatetime)
                                     VALUES
                                           (@Mch_CodeType
                                           ,@Mch_TypeName
                                           ,@Mch_CodeLength
                                           ,@Mch_RecordStatus
                                           ,@Usr_Login
                                           ,Getdate())";
            #endregion

            //insert
            ParameterInfo[] paramInfo = new ParameterInfo[5];

            paramInfo[0] = new ParameterInfo("@Mch_CodeType", row["Mch_CodeType"]);
            paramInfo[1] = new ParameterInfo("@Mch_TypeName", row["Mch_TypeName"]);
            paramInfo[2] = new ParameterInfo("@Mch_CodeLength", row["Mch_CodeLength"]);
            paramInfo[3] = new ParameterInfo("@Mch_RecordStatus", row["Mch_RecordStatus"]);
            paramInfo[4] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);

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

        public int UpdateHeader(DataRow row)
        {
            int retVal = 0;

            #region query
            string qString = @"UPDATE M_CodeHdr
                                       SET Mch_TypeName = @Mch_TypeName
                                          ,Mch_CodeLength = @Mch_CodeLength
                                          ,Mch_RecordStatus = @Mch_RecordStatus
                                          ,Usr_Login = @Usr_Login
                                          ,Ludatetime = Getdate()
                                     WHERE Mch_CodeType = @Mch_CodeType";
            #endregion

            //update
            ParameterInfo[] paramInfo = new ParameterInfo[5];

            paramInfo[0] = new ParameterInfo("@Mch_CodeType", row["Mch_CodeType"]);
            paramInfo[1] = new ParameterInfo("@Mch_TypeName", row["Mch_TypeName"]);
            paramInfo[2] = new ParameterInfo("@Mch_CodeLength", row["Mch_CodeLength"]);
            paramInfo[3] = new ParameterInfo("@Mch_RecordStatus", row["Mch_RecordStatus"]);
            paramInfo[4] = new ParameterInfo("@Usr_Login", row["Usr_Login"]);

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

        public DataSet GetLastUpdateInfoInHeader(string Mch_CodeType)
        {
            DataSet ds = new DataSet();

            #region query
            string qString = @"Select Usr_Login,Ludatetime From M_CodeHdr
                                    Where Mch_CodeType = @Mch_CodeType";
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Mch_CodeType", Mch_CodeType);
            
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            return ds;
        }

        #endregion

        #region <Added for new Detail Design>

        public DataSet FetchAllinDetail()
        {
            DataSet ds = new DataSet();

            #region <query>
            string qString = @"SELECT Mcd_Code
                                     ,Mcd_Name
                                     ,Mcd_RecordStatus 
                                     ,Mcd_CodeType
                               FROM M_CodeDtl
                               WHERE Mcd_RecordStatus = 'A'";
            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text);

                dal.CloseDB();
            }
            return ds;
        }

        public int DeleteDetail(string Mcd_CodeType, string Mcd_Code, string CentralProfile)
        {
            int retVal = 0;

            #region <query>
            string qString = @"UPDATE M_CodeDtl
                                    SET Mcd_RecordStatus = 'C'
	                                    ,Usr_Login = @Usr_Login
	                                    ,Ludatetime = Getdate()
                                    WHERE Mcd_CodeType = @Mcd_CodeType
	                                    And Mcd_Code = @Mcd_Code";
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[3];

            paramInfo[0] = new ParameterInfo("@Mcd_CodeType", Mcd_CodeType);
            paramInfo[1] = new ParameterInfo("@Mcd_Code", Mcd_Code);
            paramInfo[2] = new ParameterInfo("@Usr_Login", LoginInfo.getUser().UserCode);

            using (DALHelper dal = new DALHelper(CentralProfile, false))
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

        public bool CheckifDetailRecExists(string Mcd_CodeType, string Mcd_Code, string CompanyCode, string CentralProfile)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[3];
            paramInfo[0] = new ParameterInfo("@Mcd_CodeType", Mcd_CodeType);
            paramInfo[1] = new ParameterInfo("@Mcd_Code", Mcd_Code);
            paramInfo[2] = new ParameterInfo("@Mcd_CompanyCode", CompanyCode);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(string.Format(@"SELECT * FROM {0}..M_CodeDtl 
                                                      WHERE Mcd_CodeType = @Mcd_CodeType 
                                                        AND Mcd_Code = @Mcd_Code
                                                        AND Mcd_CompanyCode = @Mcd_CompanyCode", CentralProfile), CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            if (ds.Tables[0].Rows.Count > 0)
                return true;
            else
                return false;
        }

        public DataSet GetAccountTypeDescription(string AccountType, string CompanyCode, string CentralProfile)
        {
            DataSet ds = new DataSet();

            #region query
            string qString =string.Format(@"SELECT Mch_TypeName,
                                             Mch_CodeLength
                                           FROM {0}..M_CodeHdr
                                           WHERE Mch_RecordStatus = 'A'
                                            AND Mch_CodeType = @Mch_CodeType
                                            AND Mch_CompanyCode = @Mch_CompanyCode", CentralProfile);
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Mch_CodeType", AccountType);
            paramInfo[1] = new ParameterInfo("@Mch_CompanyCode", CompanyCode);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }

            return ds;
        }

        public int AddDetail(DataRow row, string CentralProfile)
        {
            int retVal = 0;

            #region query
            string qString = @"INSERT INTO M_CodeDtl
                                           (Mcd_CompanyCode
                                           ,Mcd_CodeType
                                           ,Mcd_Code
                                           ,Mcd_Name
                                           ,Mcd_RecordStatus
                                           ,Mcd_CreatedBy
                                           ,Mcd_CreatedDate)
                                     VALUES(@Mcd_CompanyCode
                                           ,@Mcd_CodeType
                                           ,@Mcd_Code
                                           ,@Mcd_Name
                                           ,@Mcd_RecordStatus
                                           ,@Mcd_CreatedBy
                                           ,GETDATE())";
            #endregion

            //insert
            ParameterInfo[] paramInfo = new ParameterInfo[6];
            paramInfo[0] = new ParameterInfo("@Mcd_CompanyCode", row["Mcd_CompanyCode"]);
            paramInfo[1] = new ParameterInfo("@Mcd_CodeType", row["Mcd_CodeType"]);
            paramInfo[2] = new ParameterInfo("@Mcd_Code", row["Mcd_Code"]);
            paramInfo[3] = new ParameterInfo("@Mcd_Name", row["Mcd_Name"]);
            paramInfo[4] = new ParameterInfo("@Mcd_RecordStatus", row["Mcd_RecordStatus"]);
            paramInfo[5] = new ParameterInfo("@Mcd_CreatedBy", row["Mcd_CreatedBy"]);

            using (DALHelper dal = new DALHelper(CentralProfile, false))
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

        public int UpdateDetail(DataRow row, string CentralProfile)
        {
            int retVal = 0;

            #region query
            string qString = @"UPDATE M_CodeDtl
                                   SET Mcd_Name = @Mcd_Name
                                      ,Mcd_RecordStatus = @Mcd_RecordStatus
                                      ,Mcd_UpdatedBy = @Mcd_UpdatedBy
                                      ,Mcd_UpdatedDate = GETDATE()
                                 WHERE Mcd_CodeType = @Mcd_CodeType
                                    And Mcd_Code = @Mcd_Code
                                    AND Mcd_CompanyCode = @Mcd_CompanyCode";
            #endregion

            //update
            ParameterInfo[] paramInfo = new ParameterInfo[6];
            paramInfo[0] = new ParameterInfo("@Mcd_CompanyCode", row["Mcd_CompanyCode"]);
            paramInfo[1] = new ParameterInfo("@Mcd_CodeType", row["Mcd_CodeType"]);
            paramInfo[2] = new ParameterInfo("@Mcd_Code", row["Mcd_Code"]);
            paramInfo[3] = new ParameterInfo("@Mcd_Name", row["Mcd_Name"]);
            paramInfo[4] = new ParameterInfo("@Mcd_RecordStatus", row["Mcd_RecordStatus"]);
            paramInfo[5] = new ParameterInfo("@Mcd_UpdatedBy", row["Mcd_UpdatedBy"]);

            using (DALHelper dal = new DALHelper(CentralProfile, false))
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

        #endregion

        public System.Data.DataSet FetchDetailData(string Mch_CodeType)
        {
            DataSet ds = new DataSet();
            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Mch_CodeType", Mch_CodeType);
            string sqlQuery = @"SELECT
                                      Mcd_Code
                                     ,Mcd_Name
                                     ,CASE 
                                        WHEN Mcd_RecordStatus = 'A' THEN 'ACTIVE'
                                        WHEN Mcd_RecordStatus = 'C' THEN 'CANCELLED' 
                                      END as 'Mcd_RecordStatus' 
                                     ,Mcd_CodeType
                                FROM M_CodeDtl
                                LEFT JOIN M_CodeHdr on Mcd_CodeType = Mch_CodeType
                                WHERE Mcd_CodeType = @Mch_CodeType
                                 and Mcd_RecordStatus = 'A'
                                    ";
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(sqlQuery, CommandType.Text, paramInfo);
                }
                catch (Exception Error)
                {
                    CommonProcedures.showMessageError(Error.Message);
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return ds;
        }

        #region Reports

        //Reports  
        public DataSet GetHeaderData(string Status)
        {
            DataSet ds = new DataSet();
            int paramIndex = 0;

            #region query

            string query = @"Select Mcm_CompanyName
                ,Mcm_CompanyAddress1 + ' ' + Mcm_CompanyAddress2 + ', ' + Mcd_Name as Address
                ,'TEL NO. ' + Mcm_TelNo + ' FAX NO. ' + Mcm_FaxNo as Contacts
                ,Mcm_CompanyLogo, @Status as Status
                From M_Company
                Inner Join M_CodeDtl on Mcm_CompanyAddress3 = Mcd_Code
                and Mcd_CodeType='ZIPCODE'";

            #endregion

            ParameterInfo[] param = new ParameterInfo[1];
            param[paramIndex++] = new ParameterInfo("@Status", Status);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(query, CommandType.Text, param);

                dal.CloseDB();
            }
            return ds;
        }

        public DataSet rptADMasterDetail(string Status, string AccountType)
        {
            DataSet ds = new DataSet();
            int paramIndex = 0;

            #region query

            string query = @"SELECT Mcd_CodeType
	                            , Mcd_Code
	                            , Mcd_Name
	                            , CASE WHEN Mcd_RecordStatus = 'A' THEN 'ACTIVE' 
		                            WHEN Mcd_RecordStatus = 'C' THEN 'CANCELLED' END as 'Mcd_RecordStatus' 
                            FROM M_CodeDtl
                            WHERE Mcd_CodeType = @AccountType
                            AND (Mcd_RecordStatus =  @Status OR @Status = 'ALL')";

            #endregion

            ParameterInfo[] param = new ParameterInfo[2];
            param[paramIndex++] = new ParameterInfo("@Status", Status);
            param[paramIndex++] = new ParameterInfo("@AccountType", AccountType);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(query, CommandType.Text, param);

                dal.CloseDB();
            }
            return ds;
        }
        //<end>

        //Reports     
        public DataSet GetDetailData(string Status)
        {
            DataSet ds = new DataSet();
            int paramIndex = 0;

            #region query

            string query = @"DECLARE @ALL VARCHAR(3)
                            SET @ALL = 'ALL'
                            SELECT Mch_CodeType
                                , Mch_TypeName
                                , Mch_CodeLength
                                , CASE WHEN Mch_RecordStatus = 'A' THEN 'ACTIVE' 
		                            WHEN Mch_RecordStatus = 'C' THEN 'CANCELLED' END as 'Mch_RecordStatus'
                            FROM M_CodeHdr
                                WHERE Mch_RecordStatus =  @Status OR @ALL = @Status
                                order by Mch_CodeType, Mch_TypeName";

            #endregion

            ParameterInfo[] param = new ParameterInfo[1];
            param[paramIndex++] = new ParameterInfo("@Status", Status);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(query, CommandType.Text, param);

                dal.CloseDB();
            }
            return ds;
        }

        #endregion

        #region <Save to Excel>
        public DataSet FetchAccountTypeMasteToSaveinExcel(string status, string searchstring, string sortby)
        {
            DataSet ds = new DataSet();

            #region query
            string qString = string.Format(@"SELECT Mch_CodeType as 'Account Type'
                                                  , Mch_TypeName as 'Description'
                                                  , Mch_CodeLength as 'Char Length'
                                                  , CASE WHEN Mch_RecordStatus = 'A' THEN 'ACTIVE' 
		                                                 WHEN Mch_RecordStatus = 'C' THEN 'CANCELLED' 
                                                    END as 'Status'
                                               FROM M_CodeHdr
                                              WHERE (Mch_RecordStatus = '{0}' or '{0}' = 'ALL') 
                                                AND (Mch_CodeType like '{1}%' or
									                 Mch_TypeName like '{1}%') 
                                                     {2}
                                                     ", status, searchstring, sortby);
            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text);

                dal.CloseDB();
            }
            return ds;
        }
        #endregion <Save to Excel>
    }
}
