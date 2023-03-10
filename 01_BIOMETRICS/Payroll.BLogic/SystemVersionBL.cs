using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Payroll.DAL;
using CommonLibrary;

namespace Payroll.BLogic
{
    public class SystemVersionBL : BaseMasterBase
    {
        public DataSet SetupSystemVersionGrid()
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
                        Svc_SystemID [System ID]
                        , Svc_SystemName [System Name]
                        , Svc_VersionNumber [Version Number]
                        , case when Svc_Status = 'A'
	                        then 'ACTIVE'
	                        else 'CANCELLED'
	                        end [Status]
                        from T_SystemVersionControl
                            ");

                }
                catch(Exception er)
                {
                    ds = null;
                    CommonProcedures.showMessageError(er.Message);
                }
                finally
                {
                    dal.CloseDB();
                }
            }

            return ds;
        }

        public void AddNewSystemVersion(ParameterInfo[] param)
        {
            using (DALHelper dal = new DALHelper("", true))
            {
                try
                {
                    dal.OpenDB();
                    dal.BeginTransaction();

                    dal.ExecuteNonQuery(@"
                        INSERT INTO [T_SystemVersionControl]
                                   ([Svc_SystemID]
                                   ,[Svc_SystemName]
                                   ,[Svc_VersionNumber]
                                   ,[Svc_Status]
                                   ,[Usr_Login]
                                   ,[Ludatetime])
                             VALUES
                                   (@Svc_SystemID 
                                   ,@Svc_SystemName 
                                   ,@Svc_VersionNumber 
                                   ,@Svc_Status 
                                   ,@Usr_Login 
                                   ,GETDATE() )
                        ", CommandType.Text, param);

                    dal.CommitTransaction();
                    CommonProcedures.showMessageInformation("Successfully inserted new System Version!");
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

        public void ModifySystemVersion(ParameterInfo[] param)
        {
            using (DALHelper dal = new DALHelper("", true))
            {
                try
                {
                    dal.OpenDB();
                    dal.BeginTransaction();

                    dal.ExecuteNonQuery(@"
                        UPDATE [T_SystemVersionControl]
                           SET [Svc_SystemName] = @Svc_SystemName
                              ,[Svc_VersionNumber] = @Svc_VersionNumber
                              ,[Svc_Status] = @Svc_Status
                              ,[Usr_Login] = @Usr_Login
                              ,[Ludatetime] = GETDATE()
                         WHERE [Svc_SystemID] = @Svc_SystemID
                        ", CommandType.Text, param);

                    dal.CommitTransaction();
                    CommonProcedures.showMessageInformation("Successfully updated System Version!");
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

