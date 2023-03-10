using System;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Data;
using System.Data.SqlClient;

using Payroll.DAL;
using CommonLibrary;

namespace Payroll.BLogic
{
    public class UserMasterBL : BaseBL
    {

        #region  - Kevin added 02062009 - c1 conversion query
        public DataTable GetDetailReport(string status)
        {
            DataTable dt;
            string sql = @"DECLARE @processflag as bit
                    SET @processflag = (SELECT pcm_processflag from t_processcontrolmaster
                    WHERE pcm_systemid = 'PERSONNEL' and pcm_processid = 'VWNICKNAME')


                    DECLARE @processflag2 as bit
                    SET @processflag2 = (SELECT pcm_processflag from t_processcontrolmaster
                    WHERE pcm_systemid = 'PERSONNEL' and pcm_processid = 'DSPIDCODE')

                    SELECT Mur_UserCode      
                          ,Umt_userlname
                          ,Umt_userfname
                          ,Case @processflag 
                               When 1 then emt_nickname
                               When 0 then left(Umt_usermi, 1)
                           End as MI
						  ,CASE
	                    WHEN Umt_CanViewRate = 1 THEN 'YES'
	                    WHEN Umt_CanViewRate = 0 THEN 'NO'
                           END AS Umt_CanViewRate
						  ,CASE
	                    WHEN Umt_ConsolidateRep = 1 THEN 'YES'
	                    WHEN Umt_ConsolidateRep = 0 THEN 'NO'
                           END AS Umt_ConsolidateRep
                          ,CASE
	                    WHEN Umt_status = 'A' THEN 'ACTIVE'
	                    WHEN Umt_status = 'C' THEN 'CANCELLED'
                           END AS Umt_status
                        FROM M_User
                   LEFT JOIN T_EmployeeMaster on Emt_EmployeeID = Mur_UserCode
                               WHERE Umt_status = @status
	                     OR @status = 'ALL'";

            ParameterInfo[] param = new ParameterInfo[1];
            param[0] = new ParameterInfo("@status", status);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dt = dal.ExecuteDataSet(sql, CommandType.Text, param).Tables[0];
                dal.CloseDB();
            }

            return dt;
        }
        #endregion

        public override int Add(DataRow row)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override int Update(DataRow row)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int Add(System.Data.DataRow row, bool samePass)
        {
            int retVal = 0;

            #region query
            string qString = @"INSERT INTO M_User
                                       (Mur_UserCode
                                       ,Umt_Userpswd
                                       ,Umt_userlname
                                       ,Umt_userfname
                                       ,Umt_usermi
                                       ,Umt_Position
                                       ,Mur_OfficeEmailAddress
                                       ,Umt_CanViewRate
                                       ,Umt_status
                                       ,user_login
                                       ,ludatetime,Umt_ConsolidateRep)
                                 VALUES
                                       (@Mur_UserCode
                                       ,@Umt_Userpswd
                                       ,@Umt_userlname
                                       ,@Umt_userfname
                                       ,@Umt_usermi
                                       ,@Umt_Position
                                       ,@Mur_OfficeEmailAddress
                                       ,@Umt_CanViewRate
                                       ,@Umt_status
                                       ,@user_login
                                       ,GetDate(),@Umt_ConsolidateRep)";

            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();
                try
                {
                    ParameterInfo[] paramInfo = new ParameterInfo[11];

                    paramInfo[0] = new ParameterInfo("@Mur_UserCode", row["Mur_UserCode"]);
                    if (!samePass)
                        paramInfo[1] = new ParameterInfo("@Umt_Userpswd", encryptPassword(row["Umt_Userpswd"].ToString()));
                    else
                        paramInfo[1] = new ParameterInfo("@Umt_Userpswd", row["Umt_Userpswd"]);
                    paramInfo[2] = new ParameterInfo("@Umt_userlname", row["Umt_userlname"]);
                    paramInfo[3] = new ParameterInfo("@Umt_userfname", row["Umt_userfname"]);
                    paramInfo[4] = new ParameterInfo("@Umt_usermi", row["Umt_usermi"]);
                    paramInfo[5] = new ParameterInfo("@Umt_Position", row["Umt_Position"]);
                    paramInfo[6] = new ParameterInfo("@Mur_OfficeEmailAddress", row["Mur_OfficeEmailAddress"]);
                    paramInfo[7] = new ParameterInfo("@Umt_CanViewRate", row["Umt_CanViewRate"]);
                    paramInfo[8] = new ParameterInfo("@Umt_status", row["Umt_status"]);
                    paramInfo[9] = new ParameterInfo("@user_login", row["user_login"]);
                    paramInfo[10] = new ParameterInfo("@Umt_ConsolidateRep", row["Umt_ConsolidateRep"]);
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

        public int Update(System.Data.DataRow row, string strOldPass, bool samePass)
        {
            int retVal = 0;

            #region query
            string qString = @"UPDATE M_User
                                   SET Umt_Userpswd = @Umt_Userpswd
                                      ,Umt_userlname = @Umt_userlname
                                      ,Umt_userfname = @Umt_userfname
                                      ,Umt_usermi = @Umt_usermi
                                      ,Umt_Position = @Umt_Position
                                      ,Mur_OfficeEmailAddress = @Mur_OfficeEmailAddress
                                      ,Umt_CanViewRate = @Umt_CanViewRate
                                      ,Umt_status = @Umt_status
                                      ,user_login = @user_login
                                      ,ludatetime = GetDate()
                                       ,Umt_Effectivitydate = GetDate()
                                      ,Umt_ConsolidateRep = @Umt_ConsolidateRep
                                 WHERE Mur_UserCode = @Mur_UserCode";

            #endregion

            #region T_PasswordTrail query

            string InsertString = @"INSERT INTO T_PasswordTrail
                                       (Mur_UserCode
                                       ,Umt_EffectivityDate
                                       ,Umt_Userpswd
                                       ,user_login
                                       ,ludatetime)
                                 VALUES
                                       (@Mur_UserCode
                                       ,GetDate()
                                       ,@Umt_Userpswd
                                       ,@user_login
                                       ,GetDate())";

            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();
                try
                {
                    ParameterInfo[] paramInfo = new ParameterInfo[11];

                    paramInfo[0] = new ParameterInfo("@Mur_UserCode", row["Mur_UserCode"]);
                    if (!samePass)
                        paramInfo[1] = new ParameterInfo("@Umt_Userpswd", encryptPassword(row["Umt_Userpswd"].ToString()));      
                    else
                        paramInfo[1] = new ParameterInfo("@Umt_Userpswd", row["Umt_Userpswd"].ToString());                    
                    paramInfo[2] = new ParameterInfo("@Umt_userlname", row["Umt_userlname"]);
                    paramInfo[3] = new ParameterInfo("@Umt_userfname", row["Umt_userfname"]);
                    paramInfo[4] = new ParameterInfo("@Umt_usermi", row["Umt_usermi"]);
                    paramInfo[5] = new ParameterInfo("@Umt_Position", row["Umt_Position"]);
                    paramInfo[6] = new ParameterInfo("@Mur_OfficeEmailAddress", row["Mur_OfficeEmailAddress"]);
                    paramInfo[7] = new ParameterInfo("@Umt_CanViewRate", row["Umt_CanViewRate"]);
                    paramInfo[8] = new ParameterInfo("@Umt_status", row["Umt_status"]);
                    paramInfo[9] = new ParameterInfo("@user_login", row["user_login"]);
                    paramInfo[10] = new ParameterInfo("@Umt_ConsolidateRep", row["Umt_ConsolidateRep"]);
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
            
            //Perth Added 1/18/11
            if (!samePass)
            {
                using (DALHelper dal2 = new DALHelper())
                {
                    dal2.OpenDB();
                    dal2.BeginTransactionSnapshot();
                    try
                    {
                        ParameterInfo[] paramInfo2 = new ParameterInfo[3];
                        paramInfo2[0] = new ParameterInfo("@Mur_UserCode", row["Mur_UserCode"]);
                        paramInfo2[1] = new ParameterInfo("@Umt_Userpswd", strOldPass);
                        paramInfo2[2] = new ParameterInfo("@user_login", row["user_login"]);

                        retVal = dal2.ExecuteNonQuery(InsertString, CommandType.Text, paramInfo2);
                        dal2.CommitTransactionSnapshot();
                    }
                    catch (Exception e)
                    {
                        dal2.RollBackTransactionSnapshot();
                        throw new PayrollException(e);
                    }
                    finally
                    {
                        dal2.CloseDB();
                    }
                }
            }
            //end
            return retVal;
        }

        public int Add(System.Data.DataRow row, string UserLogin)
        {
            int retVal = 0;

            throw new Exception("Code not in use.");//arthur added 20070121
           

            return retVal;
        }

        public int Update(System.Data.DataRow row, string UserLogin, bool isPasswordChanged, bool sameHandyPin)
        {
            int retVal = 0;

            throw new Exception("Code not in use.");//arthur added 20070121
            
            return retVal;
        }

        public override int Delete(string code, string userLogin)
        {
            int retVal = 0;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();

                try
                {
                    ParameterInfo[] paramInfo = new ParameterInfo[2];

                    paramInfo[0] = new ParameterInfo("@Mur_UserCode", code);
                    paramInfo[1] = new ParameterInfo("@user_login", LoginInfo.getUser().UserCode);

                    string query = @"UPDATE M_User 
                                     SET Umt_status = 'C', 
                                         user_login = @user_login, 
                                         ludatetime = GetDate() 
                                     WHERE Mur_UserCode = @Mur_UserCode";

                    retVal = dal.ExecuteNonQuery(query, CommandType.Text, paramInfo);
                    this.UpdateCostCenter(code, dal);
                    this.UpdateUserGroupDetail(code, dal);

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

        private void UpdateCostCenter(string code, DALHelper dalUp)
        {
            int retVal = 0;

            string qString = @"Update T_UserCostCenterAccess
                                    Set uca_status = 'C'
                                        ,Usr_Login = @Usr_Login
                                        ,Ludatetime = Getdate()
                                    where uca_usercode= @uca_usercode
                                    and uca_status = 'A'";

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@uca_usercode", code);
            paramInfo[1] = new ParameterInfo("@Usr_Login", LoginInfo.getUser().UserCode);
            
            retVal = dalUp.ExecuteNonQuery(qString, CommandType.Text, paramInfo);
        }

        private void UpdateUserGroupDetail(string code, DALHelper dalUp)
        {
            int retVal = 0;

            string qString = @"Update T_UserGroupDetail
                                    Set Ugd_status = 'C'
                                        ,user_login = @Usr_Login
                                        ,Ludatetime = Getdate()
                                    where Ugd_usercode = @Ugd_usercode
                                    and Ugd_status = 'A'";

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Ugd_usercode", code);
            paramInfo[1] = new ParameterInfo("@Usr_Login", LoginInfo.getUser().UserCode);

            retVal = dalUp.ExecuteNonQuery(qString, CommandType.Text, paramInfo);
        }

        public override System.Data.DataSet FetchAll()
        {
            DataSet ds = new DataSet();

            string query = @"SELECT Mur_UserCode
                                                    ,Umt_Userpswd
                                                    ,Umt_userlname
                                                    ,Umt_userfname
                                                    ,Umt_usermi
                                                    ,Umt_Position
                                                    ,Mur_OfficeEmailAddress
                                                    ,Umt_CanViewRate
                                                    ,Umt_status
                                                    ,user_login
                                                    ,ludatetime
                                                    ,dbo.constructCompleteName([Umt_userfname],[Umt_userlname], [Umt_usermi]) [Complete Name]
                                            FROM M_User 
                                            WHERE Umt_status <> 'C'";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(query, CommandType.Text);

                dal.CloseDB();
            }
            return ds;
        }

        public override System.Data.DataRow Fetch(string Mur_UserCode)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[1];

            //paramInfo[0] = new ParameterInfo("@userCode", code, SqlDbType.NVarChar, 7);
            paramInfo[0] = new ParameterInfo("@Mur_UserCode", Mur_UserCode, SqlDbType.Char, 15);

            string sqlquery = @"SELECT Mur_UserCode
                                                    ,Umt_Userpswd
                                                    ,Umt_userlname
                                                    ,Umt_userfname
                                                    ,Umt_usermi
                                                    ,Umt_Position
                                                    ,Mur_OfficeEmailAddress
                                                    ,Umt_CanViewRate
                                                    ,Umt_status
                                                    ,user_login
                                                    ,ludatetime
                                                  ,dbo.constructCompleteName(LTRIM(RTRIM([Umt_userfname])),LTRIM(RTRIM([Umt_userlname])), LTRIM(RTRIM([Umt_usermi]))) [Complete Name]
                                            FROM M_User 
                                            WHERE Mur_UserCode = @Mur_UserCode 
                                                AND Umt_status = 'A'";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                //ds = dal.ExecuteDataSet(CommonConstants.Queries.SelectUser, CommandType.Text, paramInfo);
                ds = dal.ExecuteDataSet(sqlquery, CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0];
            else
                return null;
        }

        public System.Data.DataRow FetchUserNumber(string UserNumber)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[1];

            paramInfo[0] = new ParameterInfo("@umt_Usernumber", UserNumber, SqlDbType.Char, 6);

            string sqlquery = @"SELECT [Mur_UserCode] Mur_UserCode
                                                  ,[Umt_Userpswd] Umt_Userpswd
                                                  ,[Umt_Usernumber] Umt_Usernumber
                                                  ,[Umt_UserHandypin] Umt_UserHandypin
                                                  ,[Umt_userlname] Umt_userlname
                                                  ,[Umt_userfname] Umt_userfname
                                                  ,[Umt_usermi] Umt_usermi
                                                  ,[Umt_Position] Umt_Position
                                                  ,[Umt_userview] Umt_userview
                                                  ,[Umt_userappend] Umt_userappend
                                                  ,[Umt_useredit] Umt_useredit
                                                  ,[Umt_userdelete] Umt_userdelete
                                                  ,[Umt_userprint] Umt_userprint
                                                  ,[Umt_usersupv] Umt_usersupv
                                                  ,[Umt_usermnt] Umt_usermnt
                                                  ,[Umt_status] Umt_status
                                                  ,[user_login] user_login
                                                  ,[ludatetime] ludatetime                                                  
                                            FROM M_User 
                                            WHERE Umt_Usernumber = @umt_Usernumber";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sqlquery, CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0];
            else
                return null;
        }

        public bool FetchIsUserCodeExist(string UserCode)
        {
            String ret;

            ParameterInfo[] paramInfo = new ParameterInfo[1];

            paramInfo[0] = new ParameterInfo("@Mur_UserCode", UserCode, SqlDbType.Char, 15);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ret = (string)dal.ExecuteScalar(CommonConstants.Queries.SelectIfUserCodeExist, CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            if (ret != null && ret.Trim().Length > 0)
                return true;
            else
                return false;
        }

        public bool FetchIsGroupCodeExist(string GroupCode)
        {
            String ret;

            ParameterInfo[] paramInfo = new ParameterInfo[1];

            paramInfo[0] = new ParameterInfo("@Ugt_groupcode", GroupCode, SqlDbType.Char, 15);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ret = (string)dal.ExecuteScalar(CommonConstants.Queries.SelectIfGroupCodeExist, CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            if (ret != null && ret.Trim().Length > 0)
                return true;
            else
                return false;
        }

        public bool FetchIsControlNumberExist(string ControlNumber)
        {
            String ret;

            ParameterInfo[] paramInfo = new ParameterInfo[1];

            paramInfo[0] = new ParameterInfo("@Umt_Usernumber", ControlNumber, SqlDbType.Char, 15);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ret = (string)dal.ExecuteScalar(CommonConstants.Queries.SelectIfControlNumberExist, CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            if (ret != null && ret.Trim().Length > 0)
                return true;
            else
                return false;
        }

        public bool FetchIsCostCenterExist(string CostCenter)
        {
            String ret;

            ParameterInfo[] paramInfo = new ParameterInfo[1];

            paramInfo[0] = new ParameterInfo("@Umt_UserCostCenter", CostCenter, SqlDbType.Char, 10);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ret = (string)dal.ExecuteScalar(CommonConstants.Queries.SelectIfCostCenterSectionExist, CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            if (ret != null && ret.Trim().Length > 0)
                return true;
            else
                return false;
        }

        public string FetchIsCostCenterDesc(string CostCenter)
        {
            String ret;

            ParameterInfo[] paramInfo = new ParameterInfo[1];

            paramInfo[0] = new ParameterInfo("@Umt_UserCostCenter", CostCenter, SqlDbType.Char, 10);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ret = (string)dal.ExecuteScalar(CommonConstants.Queries.SelectCostCenterDesc, CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            if (ret != null)
                return ret;
            else
                return String.Empty;
        }

        public string FetchIsCostCenterDesc(string CostCenter, DALHelper dal)
        {
            String ret;

            ParameterInfo[] paramInfo = new ParameterInfo[1];

            paramInfo[0] = new ParameterInfo("@Umt_UserCostCenter", CostCenter, SqlDbType.Char, 10);

            ret = (string)dal.ExecuteScalar(CommonConstants.Queries.SelectCostCenterDesc, CommandType.Text, paramInfo);

            if (ret != null)
                return ret;
            else
                return String.Empty;
        }

        public string FetchIsGroupDesc(string GroupCode)
        {
            String ret;

            ParameterInfo[] paramInfo = new ParameterInfo[1];

            paramInfo[0] = new ParameterInfo("@Ugt_groupcode", GroupCode, SqlDbType.Char, 15);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ret = (string)dal.ExecuteScalar(CommonConstants.Queries.SelectGroupDesc, CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            if (ret != null)
                return ret.Trim();
            else
                return String.Empty;
        }
    
        public System.Data.DataSet getData()
        {
            DataSet ds = new DataSet();

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(CommonConstants.Queries.SelectUserMaster, CommandType.Text);
                dal.CloseDB();
            }
            return ds;
        }
        public bool ExpiryPass(string userCode, int hasrights)
        {
            DataSet ds = new DataSet();
            string sql = @"select case when datediff(day,umt_effectivitydate,getdate())>(Select Mph_NumValue from M_PolicyHdr where Mph_PolicyCode= 'PASSWRDEXP' and Mph_RecordStatus ='A') 
                        then 1
                        else 0
                        end
                        from M_User where Mur_UserCode='@user'";
            sql = sql.Replace("@user",userCode);
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sql,CommandType.Text);
                dal.CloseDB();
            }
            return Convert.ToBoolean(ds.Tables[0].Rows[0][0]);
        }

        public string encryptPassword(string password)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider md5Pass = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] arrB;
            StringBuilder sb = new StringBuilder(String.Empty);

            arrB = md5Pass.ComputeHash(Encoding.ASCII.GetBytes(password));

            foreach (byte b in arrB)
            {
                sb.Append(b.ToString("x").PadLeft(2, '0'));
            }

            md5Pass.Clear();

            return sb.ToString().Substring(0, CommonConstants.Misc.PasswordLength);
        }

        public bool SupervisoryRights(string userCode)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[1];

            paramInfo[0] = new ParameterInfo("@userCode", userCode, SqlDbType.NVarChar, 5);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(@"SELECT [Umt_usersupv] FROM M_User WHERE (Mur_UserCode = @userCode)", CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            return (Convert.ToBoolean(ds.Tables[0].Rows[0]["Umt_usersupv"]));
        }

        private DataSet fetchUserMenu(string userCode)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@userCode", userCode, SqlDbType.Char, 15);

            using(DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(CommonConstants.Queries.SelectUserMenu, CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            return ds;
        }
        public bool VersionControl(string version)
        {
            DataSet ds = new DataSet();
            string sqlStatement = @"Select Ccd_SystemVer FROM T_CompanyMaster";

            //ParameterInfo[] paramCols = new ParameterInfo[1];


            using (DALHelper dal = new DALHelper())
            {

                try
                {
                    dal.OpenDB();
                    dal.BeginTransactionSnapshot();
                    ds = dal.ExecuteDataSet(sqlStatement, CommandType.Text);

                    dal.CommitTransactionSnapshot();
                }
                catch (Exception ex)
                {
                    dal.RollBackTransactionSnapshot();
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            if (ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[0].Rows[0][0].ToString().Trim() == version.Trim())
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public DataSet FetchAllUserMaster()
        {
            DataSet ds = new DataSet();

            #region query
            string qString = @"SELECT '''' + Mur_UserCode as UserCode -- for save to excel
                                      ,Umt_userlname
                                      ,Umt_userfname
                                      ,substring(Umt_usermi,1,1) as MI
                                      ,Emt_NickName as IDCode -- jhael modified 12/28/2009 to display ID Code 
                                      ,Umt_status
                                      ,Mur_UserCode
                                      ,user_login
                                      --,Convert(char(10), ludatetime, 101) as ludatetime
                                      ,usermaster.ludatetime   -- shane modified 07/11/2008
                                      ,Umt_CanViewRate
                                      ,Umt_ConsolidateRep
                                      ,Umt_Userpswd
                                      ,Umt_usermi
                                      ,Umt_Position
                                      ,Mur_OfficeEmailAddress
                                 FROM M_User usermaster
                            LEFT JOIN T_EmployeeMaster on Emt_EmployeeID = Mur_UserCode";
            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text);

                dal.CloseDB();
            }
            return ds;
        }

        public DataSet GetEmployeeMasterDetails(string Emt_EmployeeID)
        {
            DataSet ds = new DataSet();

            #region query
            string qString = @"Select Emt_EmployeeID
		                            ,Emt_LastName
		                            ,Emt_FirstName
		                            ,Substring(Emt_MiddleName,1,1) as MI
		                            ,Emt_MiddleName
		                            ,(Select Adt_AccountDesc From T_AccountDetail Where Adt_AccountType = 'POSITION' And Adt_AccountCode = Emt_PositionCode) as Position
		                            ,Emt_EmailAddress
                            From T_EmployeeMaster 
                            Where Emt_EmployeeID = @Emt_EmployeeID";
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Emt_EmployeeID", Emt_EmployeeID);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            return ds;
        }

        public bool CheckifUserCodeExist(string Mur_UserCode)
        {
            DataSet ds = new DataSet();
            #region query
            string qString = @"Select Mur_UserCode From M_User Where Mur_UserCode = @Mur_UserCode --And Umt_status = 'A'";
            #endregion
            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Mur_UserCode", Mur_UserCode);

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

        public bool CheckifUserCodeExistInConfi(string Mur_UserCode)
        {
            DataSet ds = new DataSet();
            #region query
            string qString = @"Select Mur_UserCode From M_User Where Mur_UserCode = @Mur_UserCode --And Umt_status = 'A'";
            #endregion
            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Mur_UserCode", Mur_UserCode);

            using (DALHelper dal = new DALHelper(false))
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

        public bool GetCanViewRate(string Mur_UserCode)
        {
            DataSet ds = new DataSet();
            #region query
            string qString = @"Select Umt_CanViewRate From M_User Where Mur_UserCode = @Mur_UserCode";
            #endregion
            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Mur_UserCode", Mur_UserCode);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
            {
                return Convert.ToBoolean(ds.Tables[0].Rows[0][0].ToString().Trim());
            }
            else
                return false;
        }
        
        public DataRow FetchConfi(string Mur_UserCode)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[1];

            paramInfo[0] = new ParameterInfo("@Mur_UserCode", Mur_UserCode, SqlDbType.Char, 15);

            string sqlquery = @"SELECT Mur_UserCode
                                                    ,Umt_Userpswd
                                                    ,Umt_userlname
                                                    ,Umt_userfname
                                                    ,Umt_usermi
                                                    ,Umt_Position
                                                    ,Mur_OfficeEmailAddress
                                                    ,Umt_CanViewRate
                                                    ,Umt_status
                                                    ,user_login
                                                    ,ludatetime
                                                  ,dbo.constructCompleteName(LTRIM(RTRIM([Umt_userfname])),LTRIM(RTRIM([Umt_userlname])), LTRIM(RTRIM([Umt_usermi]))) [Complete Name]
                                            FROM M_User 
                                            WHERE Mur_UserCode = @Mur_UserCode 
                                                AND Umt_status = 'A'";

            using (DALHelper dal = new DALHelper(false))
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sqlquery, CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0];
            else
                return null;
        }

        public DataRow FetchNonConfi(string Mur_UserCode)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[1];

            paramInfo[0] = new ParameterInfo("@Mur_UserCode", Mur_UserCode, SqlDbType.Char, 15);
            string sqlquery = @"SELECT Mur_UserCode
                                                    ,Umt_Userpswd
                                                    ,Umt_userlname
                                                    ,Umt_userfname
                                                    ,Umt_usermi
                                                    ,Umt_Position
                                                    ,Mur_OfficeEmailAddress
                                                    ,Umt_CanViewRate
                                                    ,Umt_status
                                                    ,user_login
                                                    ,ludatetime
                                                  ,dbo.constructCompleteName(LTRIM(RTRIM([Umt_userfname])),LTRIM(RTRIM([Umt_userlname])), LTRIM(RTRIM([Umt_usermi]))) [Complete Name]
                                            FROM M_User 
                                            WHERE Mur_UserCode = @Mur_UserCode 
                                                AND Umt_status = 'A'";


            using (DALHelper dal = new DALHelper("NONCONFI"))
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sqlquery, CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0];
            else
                return null;
        }

        public int UpdatePassword(DataRow row)
        {
            int retVal = 0;

            #region query

            string qString = @"UPDATE M_User
                                   SET Umt_Userpswd = @Umt_Userpswd
                                      ,user_login = @user_login
                                      ,ludatetime = GetDate()
                                 WHERE Mur_UserCode = @Mur_UserCode";

            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();
                try
                {
                    ParameterInfo[] paramInfo = new ParameterInfo[3];

                    paramInfo[0] = new ParameterInfo("@Mur_UserCode", row["Mur_UserCode"]);
                    paramInfo[1] = new ParameterInfo("@Umt_Userpswd", encryptPassword(row["Umt_Userpswd"].ToString()));
                    paramInfo[2] = new ParameterInfo("@user_login", row["user_login"]);

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

        //Added By Rendell Uy - 11/22/2010
        public DataSet GetUserDetails(string UserCode)
        {
            string query = string.Format(@"SELECT * FROM M_User WHERE Mur_UserCode = '{0}'", UserCode);
            DataSet ds;
            using(DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(query);
                dal.CloseDB();
            }
            return ds;
        }

        public DataSet GetUserGroupLink(string UserCode)
        {
            string query = string.Format(@"SELECT    Ugd_SystemID as 'SystemID',
                                                     Ugd_usergroupcode as 'UserGroupCode'
                                                FROM T_UserGroupDetail
                                               WHERE Ugd_usercode = '{0}'
                                                 AND Ugd_status = 'A'
                                            Order By Ugd_SystemID", UserCode);
            DataSet ds;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(query);
                dal.CloseDB();
            }
            return ds;
        }

        public DataSet GetAllSystemIDs()
        {
            string query = string.Format(@"SELECT Adt_AccountCode as 'SystemID'
	                                              , '' as 'UserGroup' 
                                            FROM T_AccountDetail
                                            WHERE Adt_AccountType = 'SYSTEMID'
                                            AND Adt_Status = 'A'");
            DataSet ds;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(query);
                dal.CloseDB();
            }
            return ds;
        }

        public DataSet GetAllCostCenters()
        {
            string query = string.Format(@"Select 'false' as CostCenterCheck
                                                  , Cct_CostCenterCode as CostCenterCode
                                                  , dbo.getCostCenterFullNameV2(Cct_CostCenterCode) as CostCenterDesc
                                                  , '' as EmploymentStatus
                                                  , '' as JobStatus
                                            FROM T_CostCenter
                                            Where  Cct_status = 'A'");
            DataSet ds;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(query);
                dal.CloseDB();
            }
            return ds;
        }

        public void DeleteUserProfile(string userCode)
        {
            string query = string.Format(@"Delete from M_UserProfile
                                            where Mup_UserCode = '{0}'", userCode);
            using (DALHelper dal = new DALHelper("", true))
            {
                dal.OpenDB();
                dal.ExecuteNonQuery(query);
                dal.CloseDB();
            }
        }

        public void CreateChiyodaUserProfile(string userCode, bool DB1, bool DB2, bool DB3, string LoginUser)
        {
            string query = @"INSERT INTO [M_UserProfile]
                                   ([Mup_UserCode]
                                   ,[Upt_DatabaseNo]
                                   ,[Mup_RecordStatus]
                                   ,[Usr_Login]
                                   ,[Ludatetime])
                             VALUES
                                   ('{0}'
                                   ,'{1}'
                                   ,'A'
                                   ,'{2}'
                                   ,GetDate())";
            using (DALHelper dal = new DALHelper("", true))
            {
                dal.OpenDB();

                if (DB1 == true)
                    dal.ExecuteNonQuery(string.Format(query, userCode, "001", LoginUser));
                if (DB2 == true)
                    dal.ExecuteNonQuery(string.Format(query, userCode, "002", LoginUser));
                if (DB3 == true)
                    dal.ExecuteNonQuery(string.Format(query, userCode, "003", LoginUser));

                dal.CloseDB();
            }
        }

        public int UpdateTrail(DataRow row)
        {
            int retVal = 0;

            #region query
            string sql = @"Select Umt_Userpswd
                            ,Umt_EffectivityDate 
                            from M_User where Mur_UserCode='@user'";
            sql = sql.Replace("@user", row["Mur_UserCode"].ToString());
            string qString = @"Insert into T_PasswordTrail
                                   (Umt_Userpswd 
                                        ,Mur_UserCode
                                      ,user_login 
                                      ,Umt_Effectivitydate
                                      ,ludatetime)
                                values
                                (@Umt_Userpswd 
                                        ,@Mur_UserCode
                                      ,@user_login 
                                      ,@Umt_Effectivitydate
                                      ,getdate())
                                 ";

            #endregion

            using (DALHelper dal = new DALHelper())
            {
                DataSet ds = new DataSet();
                dal.OpenDB();
                dal.BeginTransactionSnapshot();
                try
                {
                    ds = dal.ExecuteDataSet(sql, CommandType.Text);
                    ParameterInfo[] paramInfo = new ParameterInfo[4];

                    paramInfo[0] = new ParameterInfo("@Umt_Effectivitydate", ds.Tables[0].Rows[0]["Umt_Effectivitydate"]);
                    paramInfo[1] = new ParameterInfo("@Umt_Userpswd", ds.Tables[0].Rows[0]["Umt_Userpswd"]);
                    paramInfo[2] = new ParameterInfo("@user_login", row["user_login"]);
                    paramInfo[3] = new ParameterInfo("@Mur_UserCode", row["Mur_UserCode"]);

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
        public DataTable getCharacterSet()
        {
            DataSet ds = new DataSet();
            string sql = "select * from T_PasswordCharacterSet";
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sql, CommandType.Text);
                dal.CloseDB();
            }
           
            return ds.Tables[0];
        }
        public int getCombNum()
        {
            DataSet ds = new DataSet();
            string sql = "select Mph_NumValue  from M_PolicyHdr where Mph_PolicyCode = 'PASSWRDCMB'";
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sql, CommandType.Text);
                dal.CloseDB();
            }

            return Convert.ToInt16(ds.Tables[0].Rows[0][0]);
        }
        public void UpdatePasswordCombination(string desc, string status, string values)
        {
           
            string sql=@"update T_PasswordCharacterSet
                    set Pcs_Status = '@status'
                    @values
                    where Pcs_Description = '@desc'            
                    ";
            sql =sql.Replace("@status",status);
            sql = sql.Replace("@desc",desc);
            if (values != "")
            {
                sql = sql.Replace("@values",",Pcs_EnumeratedList = '"+values+"'");
            }
            else
                sql = sql.Replace("@values",values);
            using(DALHelper dal = new DALHelper())
            {
                try{
                dal.OpenDB();
                dal.ExecuteDataSet(sql,CommandType.Text);
                dal.CloseDB();}
                catch(Exception ex)
                {
                  
                }
            }
          
        }
        public void updateParameter(string num, string param)
        {
            string sql = @"update M_PolicyHdr
                    set Mph_NumValue = @num
                    
                    where Mph_PolicyCode = '@param'            
                    ";
            sql = sql.Replace("@num", num);
            sql = sql.Replace("@param", param);
            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    dal.ExecuteDataSet(sql, CommandType.Text);
                    dal.CloseDB();
                }
                catch (Exception ex)
                {
                }
            }

        }

        public DataSet GetUserCostCenterAccess(string SystemId, string UserCode)
        {
            string query = string.Format(@"SELECT Uca_CostCenterCode, Uca_Jobstatus, Uca_EmploymentStatus
                                            FROM T_UserCostCenterAccess
                                            WHERE Uca_SytemID = '{0}'
                                            AND Uca_UserCode = '{1}'
                                            AND Uca_Status = 'A'", SystemId, UserCode);
            DataSet ds;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(query);
                dal.CloseDB();
            }
            return ds;
        }
    }
}
