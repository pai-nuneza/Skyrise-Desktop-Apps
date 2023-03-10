using System;
using System.Data;
using Payroll.DAL;
using CommonLibrary;

namespace Payroll.BLogic
{
    public class UserGrant: BaseBL
    {

        #region - Kevin added 02062009 -  c1 report conversion

        public DataSet GetDetailReport(string sysid, string menutype, string usergroup, string status)
        {
            DataSet ds;

            string sql = @"SELECT T_UserGrant.Ugt_Usergroup 
	            , T_UserGrant.Ugt_SystemID 
	            , T_SystemMenu.Smc_MenuType 
	            , T_UserGrant.Ugt_sysmenucode 'Menu code'
	            , T_SystemMenu.Smc_MenuDesc 'Desciption' 
	            , T_UserGroupHeader.Ugh_UserGroupDesc 
	            , CASE WHEN T_UserGrant.Ugt_CanRetrieve = 1 THEN 'YES' ELSE 'NO' END AS 'Can Retrieve?'
	            , CASE WHEN T_UserGrant.Ugt_CanAdd = 1 THEN 'YES' ELSE 'NO' END AS 'Can Add?'
	            , CASE WHEN T_UserGrant.Ugt_CanEdit = 1 THEN 'YES' ELSE 'NO' END AS 'Can Edit?'
	            , CASE WHEN T_UserGrant.Ugt_CanDelete = 1 THEN 'YES' ELSE 'NO' END AS 'Can Delete?'
	            , CASE WHEN T_UserGrant.Ugt_CanGenerate = 1 THEN 'YES' ELSE 'NO' END AS 'Can Generate?'
	            , CASE WHEN T_UserGrant.Ugt_CanCheck = 1 THEN 'YES' ELSE 'NO' END AS 'Can Check?'
	            , CASE WHEN T_UserGrant.Ugt_CanApprove = 1 THEN 'YES' ELSE 'NO' END AS 'Can Approve?'
	            , CASE WHEN T_UserGrant.Ugt_CanPrintPreview = 1 THEN 'YES' ELSE 'NO' END AS 'Can Print Prev?'
	            , CASE WHEN T_UserGrant.Ugt_CanPrint = 1 THEN 'YES' ELSE 'NO' END AS 'Can Print?'
	            , CASE WHEN T_UserGrant.Ugt_CanReprint = 1 THEN 'YES' ELSE 'NO' END AS 'Can Reprint'
	            , CASE WHEN T_UserGrant.Ugt_Status = 'A' THEN 'ACTIVE' ELSE 'CANCELLED' END AS 'Status' 
            FROM T_UserGrant 
              INNER JOIN T_UserGroupHeader 
            ON (T_UserGrant.Ugt_Usergroup = T_UserGroupHeader.Ugh_UserGroupCode)
              INNER JOIN T_SystemMenu 
            ON (T_UserGrant.Ugt_sysmenucode = T_SystemMenu.Smc_MenuCode)
            WHERE T_UserGrant.Ugt_SystemID = @SystemID
            AND T_UserGrant.Ugt_Usergroup = @Usergroup
            AND (T_SystemMenu.Smc_MenuType = @MenuType OR 'ALL' = @MenuType)
            And (Ugt_Status =  @Status OR 'ALL' = @Status)";

            ParameterInfo[] param = new ParameterInfo[4];
            param[0] = new ParameterInfo("@SystemID",sysid);
            param[1] = new ParameterInfo("@Usergroup", usergroup);
            param[2] = new ParameterInfo("@MenuType", menutype);
            param[3] = new ParameterInfo("@Status", status);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sql, CommandType.Text, param);
                dal.CloseDB();
            }


            return ds;
        }

        #endregion

        public override int Add(System.Data.DataRow row)
        {

            int retVal = 0;
            ParameterInfo[] paramInfo = new ParameterInfo[14];

            paramInfo[0] = new ParameterInfo("@Ugt_Usergroup", row["Ugt_Usergroup"]);
            paramInfo[1] = new ParameterInfo("@Ugt_SystemID", row["Ugt_SystemID"]);
            paramInfo[2] = new ParameterInfo("@Ugt_sysmenucode", row["Ugt_sysmenucode"]);

            paramInfo[3] = new ParameterInfo("@Ugt_CanRetrieve", row["Ugt_CanRetrieve"]);
            paramInfo[4] = new ParameterInfo("@Ugt_CanAdd", row["Ugt_CanAdd"]);
            paramInfo[5] = new ParameterInfo("@Ugt_CanEdit", row["Ugt_CanEdit"]);
            paramInfo[6] = new ParameterInfo("@Ugt_CanDelete", row["Ugt_CanDelete"]);
            paramInfo[7] = new ParameterInfo("@Ugt_CanGenerate", row["Ugt_CanGenerate"]);
            paramInfo[8] = new ParameterInfo("@Ugt_CanCheck", row["Ugt_CanCheck"]);
            paramInfo[9] = new ParameterInfo("@Ugt_CanApprove", row["Ugt_CanApprove"]);
            paramInfo[10] = new ParameterInfo("@Ugt_CanPrintPreview", row["Ugt_CanPrintPreview"]);
            paramInfo[11] = new ParameterInfo("@Ugt_CanPrint", row["Ugt_CanPrint"]);
            paramInfo[12] = new ParameterInfo("@Ugt_CanReprint", row["Ugt_CanReprint"]);

            paramInfo[13] = new ParameterInfo("@user_login", LoginInfo.getUser().UserCode);

            string sqlquery = @"INSERT INTO T_UserGrant
                                                       (Ugt_Usergroup
                                                       ,Ugt_SystemID
                                                       ,Ugt_sysmenucode
                                                       ,Ugt_CanRetrieve
                                                       ,Ugt_CanAdd
                                                       ,Ugt_CanEdit
                                                       ,Ugt_CanDelete
                                                       ,Ugt_CanGenerate
                                                       ,Ugt_CanCheck
                                                       ,Ugt_CanApprove
                                                       ,Ugt_CanPrintPreview
                                                       ,Ugt_CanPrint
                                                       ,Ugt_CanReprint
                                                       ,Ugt_Status
                                                       ,user_login
                                                       ,ludatetime)
                                                 VALUES
                                                       (@Ugt_Usergroup
                                                       ,@Ugt_SystemID
                                                       ,@Ugt_sysmenucode
                                                       ,@Ugt_CanRetrieve
                                                       ,@Ugt_CanAdd
                                                       ,@Ugt_CanEdit
                                                       ,@Ugt_CanDelete
                                                       ,@Ugt_CanGenerate
                                                       ,@Ugt_CanCheck
                                                       ,@Ugt_CanApprove
                                                       ,@Ugt_CanPrintPreview
                                                       ,@Ugt_CanPrint
                                                       ,@Ugt_CanReprint
                                                       ,'A'
                                                       ,@user_login
                                                       ,Getdate())";

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

        public override int Delete(string userGroup, string userLogin)
        {
            int retVal=0;
            //throw new Exception("The method or operation is not implemented.");
            return retVal;
        }

        public int delete(string Ugt_Usergroup, string Ugt_SystemID, string Ugt_sysmenucode)
        {
            int retVal = 0;

            ParameterInfo[] paramInfo = new ParameterInfo[4];
            paramInfo[0] = new ParameterInfo("@Ugt_Usergroup", Ugt_Usergroup);
            paramInfo[1] = new ParameterInfo("@Ugt_SystemID", Ugt_SystemID);
            paramInfo[2] = new ParameterInfo("@Ugt_sysmenucode", Ugt_sysmenucode);
            paramInfo[3] = new ParameterInfo("@user_Login", LoginInfo.getUser().UserCode);
            string sqlquery = @"UPDATE T_UserGrant SET Ugt_status = 'C', user_Login = @user_Login, ludatetime = Getdate()
                                                WHERE Ugt_Usergroup = @Ugt_Usergroup 
                                                AND Ugt_SystemID = @Ugt_SystemID
                                                AND Ugt_sysmenucode = @Ugt_sysmenucode";

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

        public override int Update(System.Data.DataRow row)
        {
            int retVal=0;
            ParameterInfo[] paramInfo = new ParameterInfo[15];

            paramInfo[0] = new ParameterInfo("@Ugt_Usergroup", row["Ugt_Usergroup"]);
            paramInfo[1] = new ParameterInfo("@Ugt_SystemID", row["Ugt_SystemID"]);
            paramInfo[2] = new ParameterInfo("@Ugt_sysmenucode", row["Ugt_sysmenucode"]);

            paramInfo[3] = new ParameterInfo("@Ugt_CanRetrieve", row["Ugt_CanRetrieve"]);
            paramInfo[4] = new ParameterInfo("@Ugt_CanAdd", row["Ugt_CanAdd"]);
            paramInfo[5] = new ParameterInfo("@Ugt_CanEdit", row["Ugt_CanEdit"]);
            paramInfo[6] = new ParameterInfo("@Ugt_CanDelete", row["Ugt_CanDelete"]);
            paramInfo[7] = new ParameterInfo("@Ugt_CanGenerate", row["Ugt_CanGenerate"]);
            paramInfo[8] = new ParameterInfo("@Ugt_CanCheck", row["Ugt_CanCheck"]);
            paramInfo[9] = new ParameterInfo("@Ugt_CanApprove", row["Ugt_CanApprove"]);
            paramInfo[10] = new ParameterInfo("@Ugt_CanPrintPreview", row["Ugt_CanPrintPreview"]);
            paramInfo[11] = new ParameterInfo("@Ugt_CanPrint", row["Ugt_CanPrint"]);
            paramInfo[12] = new ParameterInfo("@Ugt_CanReprint", row["Ugt_CanReprint"]);
            paramInfo[13] = new ParameterInfo("@Ugt_Status", row["Ugt_Status"]);

            paramInfo[14] = new ParameterInfo("@user_login", LoginInfo.getUser().UserCode);

            string sqlquery = @"UPDATE T_UserGrant
                                               SET 
                                                  Ugt_CanRetrieve = @Ugt_CanRetrieve
                                                  ,Ugt_CanAdd = @Ugt_CanAdd
                                                  ,Ugt_CanEdit = @Ugt_CanEdit
                                                  ,Ugt_CanDelete = @Ugt_CanDelete
                                                  ,Ugt_CanGenerate = @Ugt_CanGenerate
                                                  ,Ugt_CanCheck = @Ugt_CanCheck
                                                  ,Ugt_CanApprove = @Ugt_CanApprove
                                                  ,Ugt_CanPrintPreview = @Ugt_CanPrintPreview
                                                  ,Ugt_CanPrint = @Ugt_CanPrint
                                                  ,Ugt_CanReprint = @Ugt_CanReprint
                                                  ,Ugt_Status = @Ugt_Status
                                                  ,user_login = @user_login
                                                  ,ludatetime = GetDate()
                                             WHERE Ugt_Usergroup = @Ugt_Usergroup
                                                  AND Ugt_SystemID = @Ugt_SystemID
                                                  AND Ugt_sysmenucode = @Ugt_sysmenucode";
            
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

        public override DataSet FetchAll()
        {
 	        DataSet ds = new DataSet();

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(CommonConstants.Queries.fetchUserGrant, CommandType.Text);

                dal.CloseDB();
            }
            return ds;
    
        }
    
        public override DataRow Fetch(string code)
        {
           using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                dal.ExecuteNonQuery(CommonConstants.Queries.fetchUserGrant, CommandType.Text);

                dal.CloseDB();
            }
            return null;
          
        }

        public System.Data.DataSet searchDuplicate(string userGroup, string menuCode)
        {
            DataSet ds = new DataSet();
            ParameterInfo[] paramInfo = new ParameterInfo[2];
            paramInfo[0] = new ParameterInfo("@userGroup", userGroup, SqlDbType.Char, 15);
            paramInfo[1] = new ParameterInfo("@menuCode", menuCode, SqlDbType.Char, 15);


            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(CommonConstants.Queries.selectUserGrantDuplicate, CommandType.Text, paramInfo);
                dal.CloseDB();
            }
            return ds;
        }

        public System.Data.DataSet getData()
        {
            DataSet ds = new DataSet();

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(CommonConstants.Queries.selectUserGrantAll, CommandType.Text);
                dal.CloseDB();
            }
            return ds;
        }

        public bool CheckifUserGrantExist(string Ugt_Usergroup, string Ugt_SystemID, string Ugt_sysmenucode)
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"SELECT *
                                    FROM T_UserGrant
                                WHERE Ugt_Usergroup = @Ugt_Usergroup 
                                    AND Ugt_SystemID = @Ugt_SystemID
                                    AND Ugt_sysmenucode = @Ugt_sysmenucode";

            #endregion

            ParameterInfo[] paramInfo = new ParameterInfo[3];
            paramInfo[0] = new ParameterInfo("@Ugt_Usergroup", Ugt_Usergroup);
            paramInfo[1] = new ParameterInfo("@Ugt_SystemID", Ugt_SystemID);
            paramInfo[2] = new ParameterInfo("@Ugt_sysmenucode", Ugt_sysmenucode);

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

        public DataSet GetLastUpdatedInfo(string Ugt_Usergroup, string Ugt_SystemID, string Ugt_sysmenucode)
        {
            DataSet ds = new DataSet();

            string qString = @"SELECT user_login,
                                            Convert (char(10),ludatetime,101) as ludatetime
                                            FROM T_UserGrant 
                                                WHERE Ugt_Usergroup = @Ugt_Usergroup 
                                                AND Ugt_SystemID = @Ugt_SystemID
                                                AND Ugt_sysmenucode = @Ugt_sysmenucode";

            ParameterInfo[] paramCollection = new ParameterInfo[3];
            paramCollection[0] = new ParameterInfo("@Ugt_Usergroup", Ugt_Usergroup);
            paramCollection[1] = new ParameterInfo("@Ugt_SystemID", Ugt_SystemID);
            paramCollection[2] = new ParameterInfo("@Ugt_sysmenucode", Ugt_sysmenucode);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramCollection);

                dal.CloseDB();
            }
            return ds;
        }

        public string GetMenuCodeDescription(string Smc_MenuCode)
        {
            DataSet ds = new DataSet();

            #region query
            string qString = @"SELECT  Smc_MenuDesc 
                                       FROM T_SystemMenu
                                WHERE Smc_MenuCode = @Smc_MenuCode
                                AND Smc_Status = 'A'";
            #endregion 

            ParameterInfo[] paramInfo = new ParameterInfo[1];
            paramInfo[0] = new ParameterInfo("@Smc_MenuCode", Smc_MenuCode);
            

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString, CommandType.Text, paramInfo);
                dal.CloseDB();
            }

            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0][0].ToString();
            else
                return string.Empty;
        }

        //Added By Rendell Uy - 12/2/2010
        public DataSet GetSystemFlags()
        {
            DataSet ds = new DataSet();

            #region query
            string qString = @"SELECT 'False' AS 'GENERAL'
                                    , 'False' AS 'LEAVE'
                                    , 'False' AS 'OVERTIME'
                                    , 'False' AS 'PAYROLL'
                                    , 'False' AS 'PERSONNEL'
                                    , 'False' AS 'TIMEKEEP'";
            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString);
                dal.CloseDB();
            }

            return ds;
        }

        public DataSet GetDistinctAccessibleSystems(string UserGroupCode)
        {
            DataSet ds = new DataSet();

            #region query
            string qString = string.Format(@"SELECT DISTINCT Ugt_SystemId AS SystemID
                                             FROM T_UserGrant
                                             WHERE Ugt_UserGroup = '{0}'", UserGroupCode);
            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString);
                dal.CloseDB();
            }

            return ds;
        }

        public DataSet GetMenusBySystem(string SystemId)
        {
            DataSet ds = new DataSet();

            #region query
//            string qString = string.Format(@"SELECT 'False' AS 'MenuAccessCheck'
//                                                    , Smc_MenuCode as 'MenuCode'
//                                                    , Smc_MenuDesc as 'MenuDesc'
//                                                    , Smc_MenuType as 'MenuType'
//                                                    , 'False' AS 'CanRetrieve'
//                                                    , 'False' AS 'CanAdd'
//                                                    , 'False' AS 'CanEdit'
//                                                    , 'False' AS 'CanDelete'
//                                                    , 'False' AS 'CanGenerate'
//                                                    , 'False' AS 'CanCheck'
//                                                    , 'False' AS 'CanApprove'
//                                                    , 'False' AS 'CanPrintPrev'
//                                                    , 'False' AS 'CanPrint'
//                                                    , 'False' AS 'CanReprint' 
//                                            FROM T_SystemMenu
//                                            WHERE Smc_Status = 'A'
//                                            AND Smc_SystemId = '{0}'", SystemId);
            string qString = string.Format(@"SELECT 'False' AS 'MenuAccessCheck'
                                                    ,[Smc_MenuCode] as 'MenuCode'
                                                    ,[Smc_MenuDesc] as 'MenuDesc'
                                                    ,[Smc_MenuType] as 'MenuType'
                                                    ,[Smc_CanRetrieve] AS 'CanRetrieve'
                                                    ,[Smc_CanAdd] AS 'CanAdd'
                                                    ,[Smc_CanEdit] AS 'CanEdit'
                                                    ,[Smc_CanDelete] AS 'CanDelete'
                                                    ,[Smc_CanGenerate] AS 'CanGenerate'
                                                    ,[Smc_CanCheck] AS 'CanCheck'
                                                    ,[Smc_CanApprove] AS 'CanApprove'
                                                    ,[Smc_CanPrintPreview] AS 'CanPrintPrev'
                                                    ,[Smc_CanPrint]  AS 'CanPrint'
                                                    ,[Smc_CanReprint]  AS 'CanReprint' 
                                            FROM [T_SystemMenu]
                                            WHERE Smc_Status = 'A'
                                            AND Smc_SystemId = '{0}'", SystemId);
            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(qString);
                dal.CloseDB();
            }

            return ds;
        }

        public DataSet GetUsergroupMenuAccess(string sysid, string menutype, string usergroup, string status)
        {
            DataSet ds;

            string sql = @"SELECT T_UserGrant.Ugt_Usergroup 
	            , T_UserGrant.Ugt_SystemID 
	            , T_SystemMenu.Smc_MenuType 
	            , T_UserGrant.Ugt_sysmenucode 'Menu code'
	            , T_SystemMenu.Smc_MenuDesc 'Desciption' 
	            , T_UserGroupHeader.Ugh_UserGroupDesc 
	            , CASE WHEN T_UserGrant.Ugt_CanRetrieve = 1 THEN 'TRUE' ELSE 'FALSE' END AS 'Can Retrieve?'
	            , CASE WHEN T_UserGrant.Ugt_CanAdd = 1 THEN 'TRUE' ELSE 'FALSE' END AS 'Can Add?'
	            , CASE WHEN T_UserGrant.Ugt_CanEdit = 1 THEN 'TRUE' ELSE 'FALSE' END AS 'Can Edit?'
	            , CASE WHEN T_UserGrant.Ugt_CanDelete = 1 THEN 'TRUE' ELSE 'FALSE' END AS 'Can Delete?'
	            , CASE WHEN T_UserGrant.Ugt_CanGenerate = 1 THEN 'TRUE' ELSE 'FALSE' END AS 'Can Generate?'
	            , CASE WHEN T_UserGrant.Ugt_CanCheck = 1 THEN 'TRUE' ELSE 'FALSE' END AS 'Can Check?'
	            , CASE WHEN T_UserGrant.Ugt_CanApprove = 1 THEN 'TRUE' ELSE 'FALSE' END AS 'Can Approve?'
	            , CASE WHEN T_UserGrant.Ugt_CanPrintPreview = 1 THEN 'TRUE' ELSE 'FALSE' END AS 'Can Print Prev?'
	            , CASE WHEN T_UserGrant.Ugt_CanPrint = 1 THEN 'TRUE' ELSE 'FALSE' END AS 'Can Print?'
	            , CASE WHEN T_UserGrant.Ugt_CanReprint = 1 THEN 'TRUE' ELSE 'FALSE' END AS 'Can Reprint?' 
            FROM T_UserGrant 
              INNER JOIN T_UserGroupHeader 
            ON (T_UserGrant.Ugt_Usergroup = T_UserGroupHeader.Ugh_UserGroupCode)
              INNER JOIN T_SystemMenu 
            ON (T_UserGrant.Ugt_sysmenucode = T_SystemMenu.Smc_MenuCode)
            WHERE T_UserGrant.Ugt_SystemID = @SystemID
            AND T_UserGrant.Ugt_Usergroup = @Usergroup
            AND (T_SystemMenu.Smc_MenuType = @MenuType OR 'ALL' = @MenuType)
            And (Ugt_Status =  @Status OR 'ALL' = @Status)";

            ParameterInfo[] param = new ParameterInfo[4];
            param[0] = new ParameterInfo("@SystemID", sysid);
            param[1] = new ParameterInfo("@Usergroup", usergroup);
            param[2] = new ParameterInfo("@MenuType", menutype);
            param[3] = new ParameterInfo("@Status", status);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sql, CommandType.Text, param);
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

        public void DeleteAllUserGrant(string SystemId, string UserGroup)
        {
            string query = string.Format(@"DELETE FROM T_UserGrant
                                            WHERE Ugt_SystemID = '{0}' AND Ugt_UserGroup = '{1}'", SystemId, UserGroup);

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
    }
}
