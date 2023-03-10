using System;
using System.Collections.Generic;
using System.Text;
using Payroll.DAL;
using System.Data;
using CommonLibrary;

namespace Payroll.BLogic
{
    public class ProfileManagerBL : BaseMasterBase
    {
        public DataSet GetAllUsers()
        {
            DataSet dsret = null;

            using (DALHelper dal = new DALHelper("", true))
            {
                try
                {
                    dal.OpenDB();
                    dsret = dal.ExecuteDataSet(
                                @"
                        --select 
                        --    distinct
	                    --    Mup_UserCode [User Code] 
	                    --    ,Mur_UserLastName [Last Name]
                        --    ,Mur_UserFirstName [First Name]
                        --from M_UserProfile
                        --left join M_User
                        --on Mur_UserCode = Mup_UserCode

                        select 
                            distinct
                            Mur_UserCode [User Code] 
	                        ,Mur_UserLastName [Last Name]
                            ,Mur_UserFirstName [First Name]
                        from M_User
                        left join M_UserProfile
                        on Mur_UserCode = Mup_UserCode

                                ");

                }
                catch(Exception er)
                {
                    CommonProcedures.showMessageError(er.Message);
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return dsret;
        }

        public DataSet GetProfilesByUser(string user)
        {
            DataSet dsret = null;
            using (DALHelper dal = new DALHelper("", true))
            {
                try
                {
                    dal.OpenDB();
                    if (user != string.Empty)
                    {
                        dsret = dal.ExecuteDataSet(
                            string.Format(
                                @"
                            select  Mpf_DatabaseNo [Profile No]
                                    ,Mpf_ProfileName [Profile]
                                    ,Case when Mup_RecordStatus = 'A'
                                        then 'ACTIVE'
                                        else 'CANCELELD'
                                    end [Status] 
                            from M_Profile
                            left join M_UserProfile
                            on Upt_DatabaseNo = Mpf_DatabaseNo
                            where UPPER(Mup_UserCode) = UPPER('{0}')
                            ", user));
                    }
                    else
                    {
                        dsret = dal.ExecuteDataSet(
                            @"
                            select 
                                Mpf_DatabaseNo [Database No]
                                ,Mpf_ProfileName [Profile] from M_Profile
                            where Mpf_RecordStatus = 'A'
                            ");
                    }
                }
                catch(Exception er)
                {
                    CommonProcedures.showMessageError(er.Message);
                    dsret = null;
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return dsret;
        }

        public DataSet GetDataForPrint(string EmployeeID)
        {
            DataSet ds = null;
            using (DALHelper dal = new DALHelper("", true))
            {
                try
                {
                    dal.OpenDB();
                    string query = string.Empty;
                    if (EmployeeID.Trim() != string.Empty)
                    {
                        query = string.Format(@"
                            select 
	                            Mur_UserCode
	                            ,Mur_UserLastName
	                            ,Mur_UserFirstName
	                            ,Mur_UserMiddleName
	                            ,Mur_OfficeEmailAddress
	                            ,Mur_UserNickName
	                            ,Mpf_ProfileName
                            	
                            from M_Profile
                            left join M_UserProfile
                            on Mpf_DatabaseNo = Upt_DatabaseNo
                            left outer join M_User
                            on Mur_UserCode = Mup_UserCode
                            where Mpf_RecordStatus = 'A'
                            and Mur_UserCode = '{0}'
                            
                        ", EmployeeID);
                    }
                    else
                    {
                        query = @"
                            select 
	                            Mur_UserCode
	                            ,Mur_UserLastName
	                            ,Mur_UserFirstName
	                            ,Mur_UserMiddleName
	                            ,Mur_OfficeEmailAddress
	                            ,Mur_UserNickName
	                            ,Mpf_ProfileName
                            	
                            from M_Profile
                            left join M_UserProfile
                            on Mpf_DatabaseNo = Upt_DatabaseNo
                            left outer join M_User
                            on Mur_UserCode = Mup_UserCode
                            where Mpf_RecordStatus = 'A'
                            and Mur_UserCode is not null
                        ";
                    }
                    ds = dal.ExecuteDataSet(query, CommandType.Text);
                }
                catch(Exception er)
                {
                    CommonProcedures.showMessageError(er.Message);
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return ds;
        }

        public bool AddNewRecord(ParameterInfo[] param, DataTable dtProfiles)
        {
            bool ret = false;

            using (DALHelper dal = new DALHelper("", true))
            {
                try
                {
                    dal.OpenDB();
                    dal.BeginTransaction();

                    #region Insert into User master

                    dal.ExecuteNonQuery(
                        @"
                            INSERT INTO [M_User]
                                       ([Mur_UserCode]
                                       ,[Umt_EffectivityDate]
                                       ,[Umt_Password]
                                       ,[Mur_UserLastName]
                                       ,[Mur_UserFirstName]
                                       ,[Mur_UserMiddleName]
                                       ,[Mur_UserNickName]
                                       ,[Mur_OfficeEmailAddress]
                                       ,[Umt_Status]
                                       ,[Usr_Login]
                                       ,[Ludatetime])
                                 VALUES
                                       (@Mur_UserCode 
                                       ,GETDATE()
                                       ,@Umt_Password 
                                       ,@Mur_UserLastName 
                                       ,@Mur_UserFirstName 
                                       ,@Mur_UserMiddleName 
                                       ,@Mur_UserNickName 
                                       ,@Mur_OfficeEmailAddress 
                                       ,@Umt_Status 
                                       ,@Usr_Login 
                                       ,GETDATE() )
                        ", CommandType.Text, param);

                    #endregion

                    #region Insert into User Profiles

                    string query = string.Empty;
                    for (int idx = 0; idx < dtProfiles.Rows.Count; idx++)
                    {
                        if (idx != 0)
                        {
                            query += @"
                                UNION
                            ";
                        }
                        query += string.Format(
                            @"
                        select '{0}', '{1}', '{2}', '{3}', GETDATE()
                            ",param[0].Value.ToString().Trim()
                            , dtProfiles.Rows[idx][0].ToString().Trim()
                            , param[8].Value.ToString().Trim()
                            , param[9].Value.ToString().Trim());
                    }

                    if (query != string.Empty)
                    {
                        dal.ExecuteNonQuery("Insert into M_UserProfile " + query);
                    }
                    #endregion

                    #region Updated UserMasters of Selected DB

                    for (int idx = 0; idx < dtProfiles.Rows.Count; idx++)
                    {
                        string DBName = dal.ExecuteScalar(
                            string.Format(@"
                                select Mpf_DatabaseName from M_Profile
                                where Mpf_DatabaseNo = '{0}'
                            ", dtProfiles.Rows[idx][0].ToString().Trim())).ToString().Trim();
                        DataSet ds = dal.ExecuteDataSet(
                            string.Format(@"
                                select * from {0}..M_User
                                where Mur_UserCode = '{1}'
                            ", DBName, param[0].Value.ToString()));
                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                        {
                            #region Update if record Exists

                            dal.ExecuteNonQuery(
                                string.Format(@"
                                UPDATE {0}..[M_User]
                                   SET [Umt_Userpswd] = @Umt_Password
                                      ,[Umt_userlname] = @Mur_UserLastName
                                      ,[Umt_userfname] = @Mur_UserFirstName
                                      ,[Umt_usermi] = @Mur_UserMiddleName
                                      ,[Umt_usernickname] = @Mur_UserNickName
                                      ,[Umt_Position] = @Umt_Position
                                      ,[Mur_OfficeEmailAddress] = @Mur_OfficeEmailAddress
                                      ,[Umt_CanViewRate] = 1
                                      ,[Umt_status] = @Umt_Status
                                      ,[user_login] = @Usr_Login
                                      ,[ludatetime] = GETDATE()
                                      ,[Umt_ConsolidateRep] = 1
                                      ,[Umt_EffectivityDate] = GETDATE()
                                 WHERE [Mur_UserCode] = @Mur_UserCode
                                ", DBName), CommandType.Text, param);

                            #endregion
                        }
                        else
                        {
                            #region Insert if record doesnot Exist

                            dal.ExecuteNonQuery(
                                string.Format(@"
                                INSERT INTO {0}..[M_User]
                                           ([Mur_UserCode]
                                           ,[Umt_Userpswd]
                                           ,[Umt_userlname]
                                           ,[Umt_userfname]
                                           ,[Umt_usermi]
                                           ,[Umt_usernickname]
                                           ,[Umt_Position]
                                           ,[Mur_OfficeEmailAddress]
                                           ,[Umt_CanViewRate]
                                           ,[Umt_status]
                                           ,[user_login]
                                           ,[ludatetime]
                                           ,[Umt_ConsolidateRep]
                                           ,[Umt_EffectivityDate])
                                     VALUES
                                           (@Mur_UserCode
                                           ,@Umt_Password
                                           ,@Mur_UserLastName
                                           ,@Mur_UserFirstName
                                           ,@Mur_UserMiddleName
                                           ,@Mur_UserNickName
                                           ,@Umt_Position
                                           ,@Mur_OfficeEmailAddress
                                           ,1
                                           ,@Umt_Status
                                           ,@Usr_Login
                                           ,GETDATE()
                                           ,1
                                           ,GETDATE())
                                ", DBName), CommandType.Text, param);

                            #endregion
                        }
                    }

                    #endregion

                    dal.CommitTransaction();
                    ret = true;
                    CommonProcedures.showMessageInformation("Successfully Inserted Profile Manager Setting!");
                }
                catch(Exception er)
                {
                    dal.RollBackTransaction();
                    CommonProcedures.showMessageError(er.Message);
                }
                finally
                {
                    dal.CloseDB();                
                }
            }

            return ret;
        }

        public bool ModifyRecord(ParameterInfo[] param, DataTable dtProfiles, string CurrentPass, string PrevPass)
        {
            bool ret = false;

            using (DALHelper dal = new DALHelper("", true))
            {
                try
                {
                    dal.OpenDB();
                    dal.BeginTransaction();

                    #region Update User master

                    dal.ExecuteNonQuery(
                        @"
                           UPDATE [M_User]
                               SET [Umt_EffectivityDate] = GETDATE()
                                  ,[Umt_Password] = @Umt_Password
                                  ,[Mur_UserLastName] = @Mur_UserLastName
                                  ,[Mur_UserFirstName] = @Mur_UserFirstName
                                  ,[Mur_UserMiddleName] = @Mur_UserMiddleName
                                  ,[Mur_UserNickName] = @Mur_UserNickName
                                  ,[Mur_OfficeEmailAddress] = @Mur_OfficeEmailAddress
                                  ,[Umt_Status] = @Umt_Status
                                  ,[Usr_Login] = @Usr_Login
                                  ,[Ludatetime] = GETDATE()
                             WHERE [Mur_UserCode] = @Mur_UserCode 
                        ", CommandType.Text, param);

                    #endregion

                    #region Update User Profiles

                    string query = string.Empty;
                    for (int idx = 0; idx < dtProfiles.Rows.Count; idx++)
                    {
                        if (idx != 0)
                        {
                            query += @"
                                UNION
                            ";
                        }
                        query += string.Format(
                            @"
                        select '{0}', '{1}', '{2}', '{3}', GETDATE()
                            ", param[0].Value.ToString().Trim()
                            , dtProfiles.Rows[idx][0].ToString().Trim()
                            , param[8].Value.ToString().Trim()
                            , param[9].Value.ToString().Trim());
                    }

                    if (query != string.Empty)
                    {
                        dal.ExecuteNonQuery(
                            string.Format(
                                @"
                                    delete from M_UserProfile
                                    where Mup_UserCode = '{0}'
                                ", param[0].Value.ToString()));
                        dal.ExecuteNonQuery("Insert into M_UserProfile " + query);
                    }
                    #endregion

                    #region Updated UserMasters of Selected DB

                    for (int idx = 0; idx < dtProfiles.Rows.Count; idx++)
                    {
                        string DBName = dal.ExecuteScalar(
                            string.Format(@"
                                select Mpf_DatabaseName from M_Profile
                                where Mpf_DatabaseNo = '{0}'
                            ", dtProfiles.Rows[idx][0].ToString().Trim())).ToString().Trim();
                        DataSet ds = dal.ExecuteDataSet(
                            string.Format(@"
                                select * from {0}..M_User
                                where Mur_UserCode = '{1}'
                            ", DBName, param[0].Value.ToString()));
                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                        {
                            #region Update if record Exists

                            dal.ExecuteNonQuery(
                                string.Format(@"
                                UPDATE {0}..[M_User]
                                   SET [Umt_Userpswd] = @Umt_Password
                                      ,[Umt_userlname] = @Mur_UserLastName
                                      ,[Umt_userfname] = @Mur_UserFirstName
                                      ,[Umt_usermi] = @Mur_UserMiddleName
                                      ,[Umt_usernickname] = @Mur_UserNickName
                                      ,[Umt_Position] = @Umt_Position
                                      ,[Mur_OfficeEmailAddress] = @Mur_OfficeEmailAddress
                                      ,[Umt_CanViewRate] = 1
                                      ,[Umt_status] = @Umt_Status
                                      ,[user_login] = @Usr_Login
                                      ,[ludatetime] = GETDATE()
                                      ,[Umt_ConsolidateRep] = 1
                                      ,[Umt_EffectivityDate] = GETDATE()
                                 WHERE [Mur_UserCode] = @Mur_UserCode
                                ", DBName), CommandType.Text, param);

                            #endregion
                        }
                        else
                        {
                            #region Insert if record doesnot Exist

                            dal.ExecuteNonQuery(
                                string.Format(@"
                                INSERT INTO {0}..[M_User]
                                           ([Mur_UserCode]
                                           ,[Umt_Userpswd]
                                           ,[Umt_userlname]
                                           ,[Umt_userfname]
                                           ,[Umt_usermi]
                                           ,[Umt_usernickname]
                                           ,[Umt_Position]
                                           ,[Mur_OfficeEmailAddress]
                                           ,[Umt_CanViewRate]
                                           ,[Umt_status]
                                           ,[user_login]
                                           ,[ludatetime]
                                           ,[Umt_ConsolidateRep]
                                           ,[Umt_EffectivityDate])
                                     VALUES
                                           (@Mur_UserCode
                                           ,@Umt_Password
                                           ,@Mur_UserLastName
                                           ,@Mur_UserFirstName
                                           ,@Mur_UserMiddleName
                                           ,@Mur_UserNickName
                                           ,@Umt_Position
                                           ,@Mur_OfficeEmailAddress
                                           ,1
                                           ,@Umt_Status
                                           ,@Usr_Login
                                           ,GETDATE()
                                           ,1
                                           ,GETDATE())
                                ", DBName), CommandType.Text, param);

                            #endregion
                        }
                    }

                    #endregion

                    #region Insert into Password Trail

                    if (CurrentPass != PrevPass)
                    {
                        param[2] = new ParameterInfo("@Umt_Password", PrevPass.Trim());
                        dal.ExecuteNonQuery(
                            @"
                            INSERT INTO [T_PasswordTrail]
                                       ([Mur_UserCode]
                                       ,[Umt_EffectivityDate]
                                       ,[Umt_Password]
                                       ,[Usr_Login]
                                       ,[Ludatetime])
                                 VALUES
                                       (@Mur_UserCode 
                                       ,@Umt_EffectivityDate 
                                       ,@Umt_Password 
                                       ,@Usr_Login 
                                       ,GETDATE() )
                            ", CommandType.Text, param);
                    }

                    #endregion

                    dal.CommitTransaction();
                    ret = true;
                    CommonProcedures.showMessageInformation("Successfully updated Profile Manager setting!");
                }
                catch (Exception er)
                {
                    dal.RollBackTransaction();
                    CommonProcedures.showMessageError(er.Message);
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return ret;
        }

        public DataSet GetUserByProfile(string user)
        {
            DataSet ds = null;

            using (DALHelper dal = new DALHelper())
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(
                        string.Format(@"
                            select 
	                            Mup_UserCode [User Code]
	                            ,isnull(Mur_UserLastName, '') [Last Name]
	                            ,isnull(Mur_UserFirstName,'') [First Name]
                            from M_UserProfile
                            left join M_User
                            on Mur_UserCode = Mup_UserCode
                            left join M_Profile
                            on Mpf_DatabaseNo = Upt_DatabaseNo
                            and Mpf_RecordStatus = 'A'
                            where Mpf_DatabaseNo = '{0}'
                            and Mup_RecordStatus = 'A'
                        ", user));
                }
                catch(Exception er)
                {
                    CommonProcedures.showMessageError(er.Message);
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return ds;
        }

        public DataSet GetDataForPrint2(string db)
        {
            DataSet ds = null;

            using (DALHelper dal = new DALHelper("", true))
            {
                try
                {
                    dal.OpenDB();
                    if (db != string.Empty)
                    {
                        ds = dal.ExecuteDataSet(string.Format(@"
                        select
	                        Mpf_DatabaseNo
	                        ,Mpf_ProfileName
	                        ,Mpf_DatabaseName
	                        ,Mpf_ServerName
	                        ,Mpf_UserID
	                        ,Mpf_Password
	                        ,case when Mpf_RecordStatus = 'A'
	                        then 'ACTIVE DATABASE'
	                        else 'CANCELLED DATABASE'
	                        end [Mpf_RecordStatus]
	                        ,Mpf_DatabasePrefix
	                        ,Case when Mpf_ProfileType = 'P'
	                        then 'PRIMARY PROFILE'
	                        else 'SECONDARY PROFILE'
	                         end [Mpf_ProfileType]
	                         ,Mur_UserCode
	                         ,Mur_UserLastName
	                         ,Mur_UserFirstName
	                         ,Mur_UserMiddleName
	                         ,Mur_OfficeEmailAddress
	                         ,Mur_UserNickName
                        from M_Profile
                        left join M_UserProfile
                        on Mpf_DatabaseNo = Upt_DatabaseNo
                        and Mup_RecordStatus = 'A'
                        left join M_User
                        on Mup_UserCode = Mur_UserCode
                        where 
                        Mur_UserCode is not null
                        and Mpf_DatabaseNo = '{0}'
                        order by Mpf_DatabaseNo
                         ", db), CommandType.Text);
                    }
                    else
                    {
                        ds = dal.ExecuteDataSet(@"
                        select
	                        Mpf_DatabaseNo
	                        ,Mpf_ProfileName
	                        ,Mpf_DatabaseName
	                        ,Mpf_ServerName
	                        ,Mpf_UserID
	                        ,Mpf_Password
	                        ,case when Mpf_RecordStatus = 'A'
	                        then 'ACTIVE DATABASE'
	                        else 'CANCELLED DATABASE'
	                        end [Mpf_RecordStatus]
	                        ,Mpf_DatabasePrefix
	                        ,Case when Mpf_ProfileType = 'P'
	                        then 'PRIMARY PROFILE'
	                        else 'SECONDARY PROFILE'
	                         end [Mpf_ProfileType]
	                         ,Mur_UserCode
	                         ,Mur_UserLastName
	                         ,Mur_UserFirstName
	                         ,Mur_UserMiddleName
	                         ,Mur_OfficeEmailAddress
	                         ,Mur_UserNickName
                        from M_Profile
                        left join M_UserProfile
                        on Mpf_DatabaseNo = Upt_DatabaseNo
                        and Mup_RecordStatus = 'A'
                        left join M_User
                        on Mup_UserCode = Mur_UserCode
                        where 
                        Mur_UserCode is not null
                        order by Mpf_DatabaseNo
                         ", CommandType.Text);
                    }

                }
                catch (Exception er)
                {
                    CommonProcedures.showMessageError(er.Message);
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return ds;

        }

    }
}
