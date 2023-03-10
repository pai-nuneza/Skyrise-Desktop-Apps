using System;
using System.Collections.Generic;
using System.Text;
using Payroll.DAL;
using CommonLibrary;
using System.Data;

namespace Payroll.BLogic
{
    public class ProfileMasterBL : Payroll.BLogic.BaseMasterBase
    {

        public DataSet SetUpProfileGrid()
        {
            DataSet ds = null;   
            using (DALHelper dal = new DALHelper("", true))
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(
                            @"
                            select 
                                Mpf_DatabaseNo [Profile No]
                                , Mpf_ProfileName [Profile]
                                , Mpf_DatabaseName [Database]
                                , Mpf_ServerName [Server]
                                , Mpf_UserID [User ID]
                                , Mpf_Password [Password]
                                , isnull(Mpf_DatabasePrefix, '') [Database Prefix]
                                , case when Mpf_ProfileType = 'P'
	                                then 'PRIMARY'
	                                when Mpf_ProfileType = 'S'
	                                then 'SECONDARY'
	                                else ''
	                                end [Profile Type]	
                                , case when Mpf_RecordStatus = 'A'
	                                then 'ACTIVE'
	                                else 'CANCELLED'
	                                end [Status]
                            		
                            from M_Profile
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

            return ds;
        }

        public DataSet GetDataForPrint()
        {
            DataSet ds = null;

            using (DALHelper dal = new DALHelper("", true))
            {
                try
                {
                    dal.OpenDB();
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

        public void AddNewProfile(ParameterInfo[] param)
        {
            using (DALHelper dal = new DALHelper("", true))
            {
                try
                {
                    dal.OpenDB();
                    dal.BeginTransaction();

                    dal.ExecuteNonQuery(
                        @"
                        INSERT INTO [M_Profile]
                                   ([Mpf_DatabaseNo]
                                   ,[Mpf_ProfileName]
                                   ,[Mpf_ServerName]
                                   ,[Mpf_DatabaseName]
                                   ,[Mpf_UserID]
                                   ,[Mpf_Password]
                                   ,[Mpf_RecordStatus]
                                   ,[Usr_login]
                                   ,[Mpf_DatabasePrefix]
                                   ,[Mpf_ProfileType]
                                   ,[Ludatetime])
                             VALUES
                                   (@Mpf_DatabaseNo 
                                   ,@Mpf_ProfileName 
                                   ,@Mpf_ServerName 
                                   ,@Mpf_DatabaseName 
                                   ,@Mpf_UserID 
                                   ,@Mpf_Password 
                                   ,@Mpf_RecordStatus 
                                   ,@Usr_login 
                                   ,@Mpf_DatabasePrefix 
                                   ,@Mpf_ProfileType 
                                   ,GETDATE() )
                        ", CommandType.Text, param);

                    dal.CommitTransaction();
                    CommonProcedures.showMessageInformation("Successfully Inserted new Profile!");
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
        }

        public void ModifyProfile(ParameterInfo[] param)
        {
            using (DALHelper dal = new DALHelper("", true))
            {
                try
                {
                    dal.OpenDB();
                    dal.BeginTransaction();

                    dal.ExecuteNonQuery(
                        @"
                        UPDATE [M_Profile]
                           SET [Mpf_ProfileName] = @Mpf_ProfileName
                              ,[Mpf_ServerName] = @Mpf_ServerName
                              ,[Mpf_DatabaseName] = @Mpf_DatabaseName
                              ,[Mpf_UserID] = @Mpf_UserID
                              ,[Mpf_Password] = @Mpf_Password
                              ,[Mpf_RecordStatus] = @Mpf_RecordStatus
                              ,[Usr_login] = @Usr_login
                              ,[Mpf_DatabasePrefix] = @Mpf_DatabasePrefix
                              ,[Mpf_ProfileType] = @Mpf_ProfileType
                              ,[Ludatetime] = GETDATE()
                         WHERE [Mpf_DatabaseNo] = @Mpf_DatabaseNo
                              
                        ", CommandType.Text, param);

                    dal.CommitTransaction();
                    CommonProcedures.showMessageInformation("Successfully Updated Profile!");
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
        }

    }
}
