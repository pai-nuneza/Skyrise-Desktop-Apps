using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Payroll.DAL;
using CommonLibrary;

namespace Payroll.BLogic
{
    public class PasswordMasterBL : Payroll.BLogic.BaseMasterBase
    {
        public DataSet SetupPasswordGrid()
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
	                            Pcs_Code [Password Code]
	                            , Pcs_Description [Password Description]
	                            , Pcs_EnumeratedList [Enumerated List]
	                            , case when Pcs_Status = 'A'
		                            then 'ACTIVE'
		                            else 'CANCELLED'
		                            end [Status]
                            from T_PasswordCharacterSet
                        ");
                }
                catch(Exception er)
                {
                    CommonProcedures.showMessageError(er.Message);
                    ds = null;
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return ds;
        }

        public string AutoGeneratePasswordCode()
        {
            string ret = string.Empty;
            using (DALHelper dal = new DALHelper("", true))
            {
                try
                {
                    dal.OpenDB();
                    ret = dal.ExecuteScalar(
                        @"
                        select top 1 
	                        REPLICATE('0', 2 - LEN(convert(varchar(2), MAX(Pcs_Code) + 1))) 
                            + convert(varchar(2), MAX(Pcs_Code) + 1)
                        from T_PasswordCharacterSet").ToString();
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
            return ret;
        }

        public void AddnewPassword(ParameterInfo[] param)
        {
            using (DALHelper dal = new DALHelper("", true))
            {
                try
                {
                    dal.OpenDB();
                    dal.BeginTransaction();

                    dal.ExecuteNonQuery(
                        @"
                        INSERT INTO [T_PasswordCharacterSet]
                                   ([Pcs_Code]
                                   ,[Pcs_Description]
                                   ,[Pcs_EnumeratedList]
                                   ,[Pcs_Status]
                                   ,[Usr_login]
                                   ,[Ludatetime])
                             VALUES
                                   (@Pcs_Code 
                                   ,@Pcs_Description 
                                   ,@Pcs_EnumeratedList 
                                   ,@Pcs_Status 
                                   ,@Usr_login 
                                   ,GETDATE() )
                        ", CommandType.Text, param);

                    dal.CommitTransaction();
                    CommonProcedures.showMessageInformation("Successfully inserted new password setting!");
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

        public void ModifyPassword(ParameterInfo[] param)
        {
            using (DALHelper dal = new DALHelper("", true))
            {
                try
                {
                    dal.OpenDB();
                    dal.BeginTransaction();

                    dal.ExecuteNonQuery(
                        @"
                        UPDATE [T_PasswordCharacterSet]
                           SET [Pcs_Description] = @Pcs_Description
                              ,[Pcs_EnumeratedList] = @Pcs_EnumeratedList
                              ,[Pcs_Status] = @Pcs_Status
                              ,[Usr_login] = @Usr_login
                              ,[Ludatetime] = GETDATE()
                         WHERE [Pcs_Code] = @Pcs_Code
                        ", CommandType.Text, param);

                    dal.CommitTransaction();
                    CommonProcedures.showMessageInformation("Successfully updated password setting!");
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
