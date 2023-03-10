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
    public class UserGroupBL : BaseBL
    {
        #region - Kevin added 02062009 c1 conversion
        //for group link report
        public DataSet GetReportDetail(string status, string employeeid, string sysID, string usergroup)
        {
            DataSet ds;

            string sql = @"
                           declare @processflag1 as bit
                           set @processflag1 = (select pcm_processflag from t_processcontrolmaster
                           where pcm_systemid = 'PERSONNEL' and pcm_processid = 'VWNICKNAME')

                           declare @processflag2 as bit
                           set @processflag2 = (select pcm_processflag from t_processcontrolmaster
                           where pcm_systemid = 'PERSONNEL' and pcm_processid = 'DSPIDCODE')

                           SELECT Ugd_SystemID
		                         ,Ugd_usercode
		                         ,Emt_LastName [LastName]
		                         ,Emt_FirstName [FirstName]
		                         ,LEFT(Emt_MiddleName,1) [MI]
		                         ,Emt_NickName
                                 ,case @processflag1 when 1 then 
                                            case @processflag2 when 1 then 'ID Code'
						                                       when 0 then 'Nick Name'
							                end
						                when 0 then 'MI'
                                       End as [MIHeader]
		                         ,Ugd_usergroupcode
		                         ,CASE
			                          WHEN Ugd_status = 'A' THEN 'ACTIVE'
			                          WHEN Ugd_status = 'C' THEN 'CANCELLED'
	                              END AS Ugd_status
	                         FROM T_UserGroupDetail
                        LEFT JOIN T_EmployeeMaster 
                               on Ugd_UserCode = Emt_EmployeeID
		                    WHERE (Ugd_status = @status OR @status = 'ALL')
                              and (Ugd_usercode = @employeeid or @employeeid = '')
                              and (Ugd_SystemID = @sysID or @sysID = '')
                              and (Ugd_usergroupcode = @usergroup or @usergroup = '')
                         Order By Ugd_usercode,Ugd_SystemID";

            ParameterInfo[] param = new ParameterInfo[4];
            param[0] = new ParameterInfo("@status", status);
            param[1] = new ParameterInfo("@employeeid", employeeid);
            param[2] = new ParameterInfo("@sysID", sysID);
            param[3] = new ParameterInfo("@usergroup", usergroup);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sql, CommandType.Text, param);
                dal.CloseDB();
            }

            return ds;
        }

        //for group report
        public DataSet GetReportGroupDetail(string status)
        {
            DataSet ds;

            string sql = @"SELECT Ugh_usergroupcode
	,Ugh_usergroupdesc
	,CASE
		WHEN Ugh_status = 'A' THEN 'ACTIVE'
		WHEN Ugh_status = 'C' THEN 'CANCELLED'
	 END AS Ugh_status
	FROM T_UserGroupHeader
	  WHERE Ugh_status = @status
		 OR @status = 'ALL'";

            ParameterInfo[] param = new ParameterInfo[1];
            param[0] = new ParameterInfo("@status", status);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sql, CommandType.Text, param);
                dal.CloseDB();
            }

            return ds;
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

                    
                    paramInfo[0] = new ParameterInfo("@Ugh_usergroupcode", code, SqlDbType.Char, 15);
                    paramInfo[1] = new ParameterInfo("@user_login", LoginInfo.getUser().UserCode, SqlDbType.Char, 15);
                    string sqlquery = "UPDATE T_UserGroupHeader SET Ugh_status = 'C', user_login = @user_login, ludatetime = GetDate() WHERE Ugh_usergroupcode = @Ugh_usergroupcode";
                    retVal = dal.ExecuteNonQuery(sqlquery, CommandType.Text, paramInfo);
                    sqlquery = "DELETE FROM T_UserGroupDetail WHERE Ugd_usergroupcode = @Ugh_usergroupcode";
                    retVal = dal.ExecuteNonQuery(sqlquery, CommandType.Text, paramInfo);

                    dal.CommitTransactionSnapshot();
                }
                catch (Exception e)
                {
                    dal.RollBackTransactionSnapshot();
                    //date:08262006 10:10AM
                    //Author:Eugene C. Biton
                    //start
                    throw new PayrollException(e);
                    //end
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
            throw new Exception("The method or operation is not implemented.");
        }

        public override DataSet FetchAll()
        {
            DataSet ds = new DataSet();

            string sqlquery = @"
                                            SELECT DISTINCT Ugh_usergroupcode, Ugh_usergroupdesc, Ugh_status, Count(Ugd_usergroupcode) [Count]
                                            FROM T_UserGroupHeader
                                            Left outer JOIN T_UserGroupDetail ON (Ugh_usergroupcode = Ugd_usergroupcode)
                                            GROUP BY Ugh_usergroupcode, Ugh_usergroupdesc, Ugh_status;

                                            SELECT Mur_UserCode, 
                                                    --dbo.constructCompleteName(LTRIM(RTRIM([Umt_userfname])),LTRIM(RTRIM([Umt_userlname])), LTRIM(RTRIM([Umt_usermi]))) [UserName],
		                                            LTRIM(RTRIM([Umt_userfname])) FName,
		                                            LTRIM(RTRIM([Umt_userlname])) LName, 
		                                            LTRIM(RTRIM([Umt_usermi])) MI,
                                                    Ugd_usergroupcode,
                                                    Ugd_status,
                                                    (Select 1) Checked
                                            FROM M_User
                                            LEFT OUTER JOIN T_UserGroupDetail ON (Mur_UserCode = Ugd_usercode);
                                            --LEFT OUTER JOIN T_UserGroupHeader ON (Ugd_usergroupcode = Ugd_usercode);

                                            SELECT Mur_UserCode, 
                                                    --dbo.constructCompleteName(LTRIM(RTRIM([Umt_userfname])),LTRIM(RTRIM([Umt_userlname])), LTRIM(RTRIM([Umt_usermi]))) [UserName],
		                                            LTRIM(RTRIM([Umt_userfname])) FName,
		                                            LTRIM(RTRIM([Umt_userlname])) LName, 
		                                            LTRIM(RTRIM([Umt_usermi])) MI,
                                                    Umt_Status Ugd_status,
                                                    (Select 0) Checked
                                            FROM M_User
                                            ";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(sqlquery, CommandType.Text);

                dal.CloseDB();
            }
            return ds;
        }

        public bool FetchIsUserGroupCodeExist(string GroupCode)
        {
            String ret;

            ParameterInfo[] paramInfo = new ParameterInfo[1];

            paramInfo[0] = new ParameterInfo("@Ugh_usergroupcode", GroupCode, SqlDbType.Char, 15);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ret = (string)dal.ExecuteScalar(CommonConstants.Queries.SelectIfUserGroupCodeExist, CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            if (ret != null && ret.Trim().Length > 0)
                return true;
            else
                return false;
        }

        public int Add(DataRow drGroup, DataTable dtMember)
        {
            int retVal = 0;

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();
                try
                {
                    ParameterInfo[] paramInfo = new ParameterInfo[4];

                    paramInfo[0] = new ParameterInfo("@Ugh_usergroupcode", drGroup["Ugh_usergroupcode"], SqlDbType.Char, 15);
                    paramInfo[1] = new ParameterInfo("@Ugh_usergroupdesc", drGroup["Ugh_usergroupdesc"], SqlDbType.Char, 30);
                    paramInfo[2] = new ParameterInfo("@Ugh_status", drGroup["Ugh_status"], SqlDbType.Char, 1);
                    paramInfo[3] = new ParameterInfo("@user_login", LoginInfo.getUser().UserCode, SqlDbType.Char, 15);
                    string sqlquery = @"INSERT INTO T_UserGroupHeader 
                                               (Ugh_usergroupcode, 
                                                Ugh_usergroupdesc,
                                                Ugh_status,
                                                user_login, 
                                                ludatetime)
                                               VALUES
                                               (@Ugh_usergroupcode, 
                                                @Ugh_usergroupdesc,
                                                @Ugh_status, 
                                                @user_login, 
                                                GetDate())";
                    retVal = dal.ExecuteNonQuery(sqlquery, CommandType.Text, paramInfo);

                    if (dtMember != null)
                    {
                        paramInfo = new ParameterInfo[4];
                        foreach (DataRow dr in dtMember.Rows)
                        {
                            if (dr["Checked"].ToString() == "1")
                            {
                                paramInfo[0] = new ParameterInfo("@Ugd_usergroupcode", drGroup["Ugh_usergroupcode"], SqlDbType.Char, 15);
                                paramInfo[1] = new ParameterInfo("@Ugd_usercode", dr["Mur_UserCode"], SqlDbType.Char, 15);
                                if (dr["Ugd_status"].ToString() == StringEnum.GetEnumDisplay(CommonEnum.Status.ACTIVE))
                                    paramInfo[2] = new ParameterInfo("@Ugd_status", StringEnum.GetStringValue(CommonEnum.Status.ACTIVE), SqlDbType.Char, 1);
                                else if (dr["Ugd_status"].ToString() == StringEnum.GetEnumDisplay(CommonEnum.Status.CANCELLED))
                                    paramInfo[2] = new ParameterInfo("@Ugd_status", StringEnum.GetStringValue(CommonEnum.Status.CANCELLED), SqlDbType.Char, 1);
                                else if (dr["Ugd_status"].ToString() == StringEnum.GetEnumDisplay(CommonEnum.Status.ONHOLD))
                                    paramInfo[2] = new ParameterInfo("@Ugd_status", StringEnum.GetStringValue(CommonEnum.Status.ONHOLD), SqlDbType.Char, 1);
                                paramInfo[3] = new ParameterInfo("@user_login", LoginInfo.getUser().UserCode, SqlDbType.Char, 15);
                                sqlquery = @"INSERT INTO T_UserGroupDetail
                                               (Ugd_usergroupcode, 
                                                Ugd_usercode,
                                                Ugd_status,
                                                user_login, 
                                                ludatetime)
                                               VALUES
                                               (@Ugd_usergroupcode, 
                                                @Ugd_usercode,
                                                @Ugd_status, 
                                                @user_login, 
                                                GetDate())";
                                retVal = dal.ExecuteNonQuery(sqlquery, CommandType.Text, paramInfo);
                            }
                        }
                    }

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

        public int Update(DataRow drGroup, DataTable dtMember)
        {
            int retVal = 0;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();
                try
                {
                    //update
                    ParameterInfo[] paramInfo = new ParameterInfo[4];

                    paramInfo[0] = new ParameterInfo("@Ugh_usergroupcode", drGroup["Ugh_usergroupcode"], SqlDbType.Char, 15);
                    paramInfo[1] = new ParameterInfo("@Ugh_usergroupdesc", drGroup["Ugh_usergroupdesc"], SqlDbType.Char, 30);
                    paramInfo[2] = new ParameterInfo("@Ugh_status", drGroup["Ugh_status"], SqlDbType.Char, 1);
                    paramInfo[3] = new ParameterInfo("@user_login", LoginInfo.getUser().UserCode, SqlDbType.Char, 15);

                    string sqlquery = @"UPDATE T_UserGroupHeader 
                                            SET 
                                                Ugh_usergroupdesc = @Ugh_usergroupdesc,
                                                Ugh_status = @Ugh_status, 
                                                user_login = @user_login, 
                                                ludatetime = GetDate() 
                                            WHERE Ugh_usergroupcode = @Ugh_usergroupcode";
                    retVal = dal.ExecuteNonQuery(sqlquery, CommandType.Text, paramInfo);

                    //delete all members
                    paramInfo = new ParameterInfo[1];
                    paramInfo[0] = new ParameterInfo("@Ugh_usergroupcode", drGroup["Ugh_usergroupcode"], SqlDbType.Char, 15);
                    sqlquery = "DELETE FROM T_UserGroupDetail WHERE Ugd_usergroupcode = @Ugh_usergroupcode";
                    retVal = dal.ExecuteNonQuery(sqlquery, CommandType.Text, paramInfo);

                    //add all members
                    if (dtMember != null)
                    {
                        paramInfo = new ParameterInfo[4];
                        foreach (DataRow dr in dtMember.Rows)
                        {
                            if (dr["Checked"].ToString() == "1")
                            {
                                paramInfo[0] = new ParameterInfo("@Ugd_usergroupcode", drGroup["Ugh_usergroupcode"], SqlDbType.Char, 15);
                                paramInfo[1] = new ParameterInfo("@Ugd_usercode", dr["Mur_UserCode"], SqlDbType.Char, 15);
                                if (dr["Ugd_status"].ToString() == StringEnum.GetEnumDisplay(CommonEnum.Status.ACTIVE))
                                    paramInfo[2] = new ParameterInfo("@Ugd_status", StringEnum.GetStringValue(CommonEnum.Status.ACTIVE), SqlDbType.Char, 1);
                                else if (dr["Ugd_status"].ToString() == StringEnum.GetEnumDisplay(CommonEnum.Status.CANCELLED))
                                    paramInfo[2] = new ParameterInfo("@Ugd_status", StringEnum.GetStringValue(CommonEnum.Status.CANCELLED), SqlDbType.Char, 1);
                                else if (dr["Ugd_status"].ToString() == StringEnum.GetEnumDisplay(CommonEnum.Status.ONHOLD))
                                    paramInfo[2] = new ParameterInfo("@Ugd_status", StringEnum.GetStringValue(CommonEnum.Status.ONHOLD), SqlDbType.Char, 1);
                                paramInfo[3] = new ParameterInfo("@user_login", LoginInfo.getUser().UserCode, SqlDbType.Char, 15);
                                sqlquery = @"INSERT INTO T_UserGroupDetail
                                               (Ugd_usergroupcode, 
                                                Ugd_usercode,
                                                Ugd_status,
                                                user_login, 
                                                ludatetime)
                                               VALUES
                                               (@Ugd_usergroupcode, 
                                                @Ugd_usercode,
                                                @Ugd_status, 
                                                @user_login, 
                                                GetDate())";

                                retVal = dal.ExecuteNonQuery(sqlquery, CommandType.Text, paramInfo);
                            }
                        }
                    }

                    dal.CommitTransactionSnapshot();
                }
                catch (Exception e)
                {
                    dal.RollBackTransactionSnapshot();
                    //date:08262006 10:10AM
                    //Author:Eugene C. Biton
                    //start
                    throw new PayrollException(e);
                    //end
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return retVal;
        }

        public int Delete(string groupCode)
        {
            int retVal = 0;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();
                try
                {

                    ParameterInfo[] paramInfo = new ParameterInfo[2];

                    paramInfo[0] = new ParameterInfo("@Ugh_usergroupcode", groupCode, SqlDbType.Char, 15);
                    paramInfo[1] = new ParameterInfo("@user_login", LoginInfo.getUser().UserCode, SqlDbType.Char, 15);
                    string sqlquery = "UPDATE T_UserGroupHeader SET Ugh_status = 'C', user_login = @user_login, ludatetime = GetDate() WHERE Ugh_usergroupcode = @Ugh_usergroupcode";
                    retVal = dal.ExecuteNonQuery(sqlquery, CommandType.Text, paramInfo);
                    sqlquery = "UPDATE T_UserGroupDetail SET Ugd_status = 'C', user_login = @user_login, ludatetime = GetDate() WHERE Ugd_usergroupcode = @Ugh_usergroupcode";
                    retVal = dal.ExecuteNonQuery( sqlquery, CommandType.Text, paramInfo);

                    dal.CommitTransactionSnapshot();
                }
                catch (Exception e)
                {
                    dal.RollBackTransactionSnapshot();
                    //date:08262006 10:10AM
                    //Author:Eugene C. Biton
                    //start
                    throw new PayrollException(e);
                    //end
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return retVal;
        }

        #region <Added for new Header Design>

        public DataSet FetchAllinHeader()
        {
            DataSet ds = new DataSet();

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet("SELECT Ugh_usergroupcode,Ugh_usergroupdesc,Ugh_status FROM T_UserGroupHeader", CommandType.Text);

                dal.CloseDB();
            }
            return ds;
        }

        public int DeleteHeader(string Ugh_usergroupcode)
        {
            int retVal = 0;

            #region <query>
            string qString = @"UPDATE T_UserGroupHeader
                                Set Ugh_status = 'C'
	                                ,user_login = @user_login
	                                ,ludatetime = Getdate()
                                Where Ugh_usergroupcode = @Ugh_usergroupcode";
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];

            paramInfo[0] = new ParameterInfo("@Ugh_usergroupcode", Ugh_usergroupcode);
            paramInfo[1] = new ParameterInfo("@user_login", LoginInfo.getUser().UserCode);
                    
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();
                try
                {
                    retVal = dal.ExecuteNonQuery(qString , CommandType.Text, paramInfo);
                    
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

        public bool CheckIfUserGroupExistsinDetail(string Ugh_usergroupcode)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Ugh_usergroupcode", Ugh_usergroupcode);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet("SELECT * FROM T_UserGroupDetail WHERE Ugd_usergroupcode = @Ugh_usergroupcode AND Ugd_status = 'A'", CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            if (ds.Tables[0].Rows.Count > 0)
                return true;
            else
                return false;
        }

        public bool CheckifUserGroupExists(string Ugh_usergroupcode)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Ugh_usergroupcode", Ugh_usergroupcode);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet("SELECT * FROM T_UserGroupHeader WHERE Ugh_usergroupcode = @Ugh_usergroupcode", CommandType.Text, paramInfo);

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
            string qString = @"INSERT INTO T_UserGroupHeader 
                                               (Ugh_usergroupcode, 
                                                Ugh_usergroupdesc,
                                                Ugh_status,
                                                user_login, 
                                                ludatetime)
                                               VALUES
                                               (@Ugh_usergroupcode, 
                                                @Ugh_usergroupdesc,
                                                @Ugh_status, 
                                                @user_login, 
                                                GetDate())";
            #endregion

            //insert
            ParameterInfo[] paramInfo = new ParameterInfo[4];

            paramInfo[0] = new ParameterInfo("@Ugh_usergroupcode", row["Ugh_usergroupcode"]);
            paramInfo[1] = new ParameterInfo("@Ugh_usergroupdesc", row["Ugh_usergroupdesc"]);
            paramInfo[2] = new ParameterInfo("@Ugh_status", row["Ugh_status"]);
            paramInfo[3] = new ParameterInfo("@user_login", row["user_login"]);

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
            string qString = @"UPDATE T_UserGroupHeader 
                                            SET 
                                                Ugh_usergroupdesc = @Ugh_usergroupdesc,
                                                Ugh_status = @Ugh_status, 
                                                user_login = @user_login, 
                                                ludatetime = GetDate() 
                                            WHERE Ugh_usergroupcode = @Ugh_usergroupcode";
            #endregion

            //update
            ParameterInfo[] paramInfo = new ParameterInfo[4];

            paramInfo[0] = new ParameterInfo("@Ugh_usergroupcode", row["Ugh_usergroupcode"]);
            paramInfo[1] = new ParameterInfo("@Ugh_usergroupdesc", row["Ugh_usergroupdesc"]);
            paramInfo[2] = new ParameterInfo("@Ugh_status", row["Ugh_status"]);
            paramInfo[3] = new ParameterInfo("@user_login", row["user_login"]);

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

        public DataSet GetLastUpdateInfoInHeader(string Ugh_usergroupcode)
        {
            DataSet ds = new DataSet();

            #region query
            string qString = @"Select user_login,ludatetime
                                    From T_UserGroupHeader
                                    Where Ugh_usergroupcode = @Ugh_usergroupcode";
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Ugh_usergroupcode", Ugh_usergroupcode);
            
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

        public DataSet FetchAllinDetail(string employeeid, string sysID, string usergroup)
        {
            DataSet ds = new DataSet();

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(string.Format(@"SELECT Ugd_SystemID,
                                                 Ugd_usercode,
                                                 Emt_LastName [LastName],
                                                 Emt_FirstName [FirstName],
                                                 LEFT(Emt_MiddleName,1) [MI],
                                                 Emt_NickName,
                                                 Ugd_usergroupcode,
                                                 Ugd_status 
                                            FROM T_UserGroupDetail
                                       LEFT JOIN T_EmployeeMaster 
                                              on Ugd_UserCode = Emt_EmployeeID
                                           WHERE (Ugd_usercode = '{0}' or '{0}' = '')
                                             and (Ugd_SystemID = '{1}' or '{1}' = '')
                                             and (Ugd_usergroupcode = '{2}' or '{2}' = '')
                                        Order By Ugd_usercode,Ugd_SystemID", employeeid, sysID, usergroup), CommandType.Text);

                dal.CloseDB();
            }
            return ds;
        }

        public int DeleteDetail(string Ugd_SystemID, string Ugd_usercode)
        {
            int retVal = 0;

            #region <query>
            string qString = @"UPDATE T_UserGroupDetail
                                    SET Ugd_status = 'C'
	                                    ,user_login = @user_login
	                                    ,ludatetime = Getdate()
                                    Where Ugd_SystemID = @Ugd_SystemID
                                    And Ugd_usercode = @Ugd_usercode";
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[3];

            paramInfo[0] = new ParameterInfo("@Ugd_SystemID", Ugd_SystemID);
            paramInfo[1] = new ParameterInfo("@Ugd_usercode", Ugd_usercode);
            paramInfo[2] = new ParameterInfo("@user_login", LoginInfo.getUser().UserCode);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();
                try
                {
                    retVal = dal.ExecuteNonQuery(qString, CommandType.Text, paramInfo);
                    this.UpdateCostCenterAccess(Ugd_SystemID, Ugd_usercode, dal);

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

        public void UpdateCostCenterAccess(string Ugd_SystemID, string Ugd_usercode, DALHelper DalUp)
        {
            int retVal = 0;

            #region <query>
            string qString = @"Update T_UserCostCenterAccess
                                    set uca_status = 'C'
                                        ,usr_login = @user_login
                                        ,ludatetime = getdate()
                                    where uca_usercode= @Ugd_usercode
                                    and Uca_SytemID = @Ugd_SystemID
                                    and uca_status = 'A'";
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[3];

            paramInfo[0] = new ParameterInfo("@Ugd_SystemID", Ugd_SystemID);
            paramInfo[1] = new ParameterInfo("@Ugd_usercode", Ugd_usercode);
            paramInfo[2] = new ParameterInfo("@user_login", LoginInfo.getUser().UserCode);

            retVal = DalUp.ExecuteNonQuery(qString, CommandType.Text, paramInfo);

        }

        public bool CheckifDetailRecExists(string Ugd_SystemID, string Ugd_usercode)
        {
            DataSet ds = new DataSet();

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Ugd_SystemID", Ugd_SystemID);
            paramInfo[1] = new ParameterInfo("@Ugd_usercode", Ugd_usercode);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet("SELECT * FROM T_UserGroupDetail WHERE Ugd_SystemID = @Ugd_SystemID And Ugd_usercode = @Ugd_usercode", CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            if (ds.Tables[0].Rows.Count > 0)
                return true;
            else
                return false;
        }

        public string GetSystemIDDescription(string SystemID)
        {
            DataSet ds = new DataSet();

            #region query
            string qString = @"Select Adt_AccountDesc
                                From T_AccountDetail
                                Where Adt_AccountType = 'SYSTEMID'
                                And Adt_AccountCode = @Adt_AccountCode
                                And Adt_Status = 'A'";
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Adt_AccountCode", SystemID);

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

        public string GetUserGroupDescription(string UserGroupCode)
        {
            DataSet ds = new DataSet();

            #region query
            string qString = @"Select Ugh_usergroupdesc
                                    From T_UserGroupHeader
                                    Where Ugh_status = 'A'
                                    And Ugh_usergroupcode = @Ugh_usergroupcode";
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Ugh_usergroupcode", UserGroupCode);

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

        public string GetUserCodeName(string UserCode)
        {
            DataSet ds = new DataSet();

            #region query
            string qString = @"Select RTRIM(Emt_LastName) + ', ' + 
                                      RTRIM(Emt_FirstName) + ' ' + 
                                      Case when Emt_MiddleName = '' then ''
                                           else Substring(Emt_MiddleName,1,1) + '.'
                                      End
                                    From T_EmployeeMaster
                                    Where Emt_EmployeeID = @Emt_EmployeeID
                                    --And Emt_JobStatus <> 'IN'";
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Emt_EmployeeID", UserCode);

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

        public int AddDetail(DataRow row)
        {
            int retVal = 0;

            #region query
            string qString = @"INSERT INTO T_UserGroupDetail
                                               (Ugd_SystemID
                                               ,Ugd_usercode
                                               ,Ugd_usergroupcode
                                               ,Ugd_status
                                               ,user_login
                                               ,ludatetime)
                                         VALUES
                                               (@Ugd_SystemID
                                               ,@Ugd_usercode
                                               ,@Ugd_usergroupcode
                                               ,@Ugd_status
                                               ,@user_login
                                               ,Getdate())";
            #endregion

            //insert
            ParameterInfo[] paramInfo = new ParameterInfo[5];

            paramInfo[0] = new ParameterInfo("@Ugd_SystemID", row["Ugd_SystemID"]);
            paramInfo[1] = new ParameterInfo("@Ugd_usercode", row["Ugd_usercode"]);
            paramInfo[2] = new ParameterInfo("@Ugd_usergroupcode", row["Ugd_usergroupcode"]);
            paramInfo[3] = new ParameterInfo("@Ugd_status", row["Ugd_status"]);
            paramInfo[4] = new ParameterInfo("@user_login", row["user_login"]);

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

        public int UpdateDetail(DataRow row)
        {
            int retVal = 0;

            #region query
            string qString = @"UPDATE T_UserGroupDetail
                                   SET Ugd_usergroupcode = @Ugd_usergroupcode
                                      ,Ugd_status = @Ugd_status
                                      ,user_login = @user_login
                                      ,ludatetime = Getdate()
                                 WHERE Ugd_SystemID = @Ugd_SystemID
                                   And Ugd_usercode = @Ugd_usercode";
            #endregion

            //update
            ParameterInfo[] paramInfo = new ParameterInfo[5];

            paramInfo[0] = new ParameterInfo("@Ugd_SystemID", row["Ugd_SystemID"]);
            paramInfo[1] = new ParameterInfo("@Ugd_usercode", row["Ugd_usercode"]);
            paramInfo[2] = new ParameterInfo("@Ugd_usergroupcode", row["Ugd_usergroupcode"]);
            paramInfo[3] = new ParameterInfo("@Ugd_status", row["Ugd_status"]);
            paramInfo[4] = new ParameterInfo("@user_login", row["user_login"]);

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

        public DataSet GetLastUpdateInfo(string Ugd_SystemID, string Ugd_usercode)
        {
            DataSet ds = new DataSet();

            #region query
            string qString = @"Select user_login,ludatetime
                                From T_UserGroupDetail
                                Where Ugd_SystemID = @Ugd_SystemID
                                And Ugd_usercode = @Ugd_usercode";
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@Ugd_SystemID", Ugd_SystemID);
            paramInfo[1] = new ParameterInfo("@Ugd_usercode", Ugd_usercode);
            
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);

                dal.CloseDB();
            }
            return ds;
        }

        public string GetUserCode(string UserCode)
        {
            DataSet ds = new DataSet();

            #region query
            string qString = @"Select RTRIM(Umt_userlname) + ', ' + Umt_userfname + ' ' + Umt_usermi + '.'
	                                    From M_User
                                    Where Umt_status = 'A'
	                                    And Mur_UserCode = @Mur_UserCode";
            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Mur_UserCode", UserCode);

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

        public DataSet FetchAllinDetails()
        {
            DataSet ds = new DataSet();

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(@"SELECT Ugd_SystemID,
                                                 Ugd_usercode,
                                                 Emt_LastName [LastName],
                                                 Emt_FirstName [FirstName],
                                                 LEFT(Emt_MiddleName,1) [MI],
                                                 Emt_NickName,
                                                 Ugd_usergroupcode,
                                                 Ugd_status 
                                            FROM T_UserGroupDetail
                                       LEFT JOIN T_EmployeeMaster 
                                              on Ugd_UserCode = Emt_EmployeeID", CommandType.Text);

                dal.CloseDB();
            }
            return ds;
        }

        #endregion

        //Added By Rendell Uy - 11/28/2010
        public DataSet CheckIfUserGroupExists(string UserCode, string SystemID)
        {
            string query = string.Format(@"SELECT Ugd_status FROM T_UserGroupDetail
                                            WHERE Ugd_SystemID = '{0}'
                                            AND Ugd_UserCode = '{1}'", SystemID, UserCode);
            DataSet ds;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(query);
                dal.CloseDB();
            }

            return ds;
        }

        public void DeleteAllUserGroup(string UserCode)
        {
            string query = string.Format(@"DELETE FROM T_UserGroupDetail
                                            WHERE Ugd_UserCode = '{0}'", UserCode);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                dal.BeginTransactionSnapshot();
                try
                {
                    dal.ExecuteNonQuery(query);
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
        }

        #region <Save to Excel>

        public DataSet FetchUserGroupLinkMasterToSaveinExcel(string status, bool viewNickName, string searchstring, string sortby, string empID, string sysID, string userGroup)
        {
            DataSet ds = new DataSet();

            #region query
            string qString = string.Format(@"DECLARE @processflag as bit
                                SET @processflag = (SELECT pcm_processflag from t_processcontrolmaster
                                WHERE pcm_systemid = 'PERSONNEL' and pcm_processid = 'VWNICKNAME')
                                DECLARE @processflag2 as bit
                                SET @processflag2 = (SELECT pcm_processflag from t_processcontrolmaster
                                WHERE pcm_systemid = 'PERSONNEL' and pcm_processid = 'DSPIDCODE')

					            Declare @NAMER as varchar(20)
					            set @NAMER = (select case @processflag when 1 then case @processflag2 when 1 then 'ID Code' when 0 then 'Nick Name' end when 0 then 'MI' end as MI)

					            declare @query as varchar(max)
					            select @query = '
					            DECLARE @processflag as bit
                                SET @processflag = (SELECT pcm_processflag from t_processcontrolmaster
                                WHERE pcm_systemid = ''PERSONNEL'' and pcm_processid = ''VWNICKNAME'') 
                  
								SELECT '''''''' + Ugd_usercode as [User Code]
                                      ,Emt_LastName as ''Last Name''
                                      ,Emt_FirstName as ''First Name''
                                      ,LEFT(Emt_MiddleName,1) as ''MI''");
            if (viewNickName == true)
            {
                qString += string.Format(@"
									  ,Case @processflag 
				                            When 1 then Emt_NickName
                                            When 0 then Left(Emt_Middlename, 1)								
				                       End as '''+@NAMER +'''");
            }

            qString += string.Format(@" 
                                      ,Ugd_SystemID [System]
                                      ,Ugd_usergroupcode [User group]
                                      ,Case Ugd_status when ''A'' then ''ACTIVE''
                                                       When ''C'' then ''CANCELLED''
									   End as ''Status''
                                  FROM T_UserGroupDetail
                             LEFT JOIN T_EmployeeMaster 
                                    on Ugd_UserCode = Emt_EmployeeID
                                 Where (Ugd_status = ''{0}'' or ''{0}'' = ''ALL'') 
                                   and (Ugd_usercode = ''{3}'' or ''{3}'' = '''')
                                   and (Ugd_SystemID = ''{4}'' or ''{4}'' = '''')
                                   and (Ugd_usergroupcode = ''{5}'' or ''{5}'' = '''')
                                   and (Ugd_usercode like ''{1}%'' or 
                                        Emt_LastName like ''{1}%'' or 
                                        Emt_FirstName like ''{1}%'' or ", status, searchstring, sortby, empID, sysID, userGroup);
            if (viewNickName == true)
            {
                qString += string.Format(" Emt_NickName like ''{1}%'' or ", status, searchstring, sortby, empID, sysID, userGroup);
            }

            qString += string.Format(@"                        
                                       Ugd_SystemID like ''{1}%'' or 
                                       Ugd_usergroupcode like ''{1}%'' or 
                                       Ugd_status like ''{1}%'' ) 
                                       {2}
                                    ' exec (@query)", status, searchstring, sortby, empID, sysID, userGroup);
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
