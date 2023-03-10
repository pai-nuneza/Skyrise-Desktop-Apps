using System;
using System.Collections.Generic;
using System.Text;
using Payroll.DAL;
using CommonLibrary;
using System.Data;


namespace Payroll.BLogic
{
    public class ParameterMasterBL : BaseMasterBase
    {
        public DataSet SetupParameterGrid()
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
	                        Mph_PolicyCode [Parameter ID]
	                        , Mph_PolicyName [Description]
	                        , Mph_NumValue [Numeric Value]
	                        , case when Mph_RecordStatus = 'A'
	                        then 'ACTIVE'
	                        else 'CANCELLED'
	                        end [Status]
                        	, case when Mph_DataType = 'D'
                        		then 'DECIMAL'
                        		when Mph_DataType = 'I'
                        		then 'INTEGER'
                        		when Mph_DataType = 'B'
                        		then 'BOOLEAN'
                        		when Mph_DataType = 'C'
                        		then 'CHARACTER'
                        		when Mph_DataType = 'T'
                        		then 'DATETIME'
                        		else ''
                        		end [Data Type]
                        	,case when Mph_SettingType = 'S'
                        		then 'SINGLE'
                        		when Mph_SettingType = 'M'
                        		then 'MULTIPLE'
                        		else ''
                        		end [Select Type]
                        	,Mph_CharValue [Char Value]
                        	
                        from M_PolicyHdr
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

        public void AddNewParameter(ParameterInfo[] param)
        {
            using (DALHelper dal = new DALHelper("", true))
            {
                try
                {
                    dal.OpenDB();
                    dal.BeginTransaction();

                    dal.ExecuteNonQuery(
                        @"
                        INSERT INTO [M_PolicyHdr]
                                   ([Mph_PolicyCode]
                                   ,[Mph_PolicyName]
                                   ,[Mph_NumValue]
                                   ,[Mph_RecordStatus]
                                   ,[Usr_Login]
                                    ,Mph_DataType
                                    ,Mph_SettingType
                                    ,Mph_CharValue
                                   ,[Ludatetime])
                             VALUES
                                   (@Mph_PolicyCode 
                                   ,@Mph_PolicyName 
                                   ,@Mph_NumValue 
                                   ,@Mph_RecordStatus 
                                   ,@Usr_Login
                                    ,@Mph_DataType
                                    ,@Mph_SettingType
                                    ,@Mph_CharValue 
                                   ,GETDATE() )
                        ", CommandType.Text, param);

                    dal.CommitTransaction();
                    CommonProcedures.showMessageInformation("Successfully inserted new parameter");
                }
                catch(Exception er)
                {
                    dal.RollBackTransaction();
                }
                finally
                {
                    dal.CloseDB();
                }
            }
        }

        public void ModifyParameter(ParameterInfo[] param)
        {
            using (DALHelper dal = new DALHelper("", true))
            {
                try
                {
                    dal.OpenDB();
                    dal.BeginTransaction();

                    dal.ExecuteNonQuery(
                        @"
                        UPDATE [M_PolicyHdr]
                           SET [Mph_PolicyName] = @Mph_PolicyName
                              ,[Mph_NumValue] = @Mph_NumValue
                              ,[Mph_RecordStatus] = @Mph_RecordStatus
                              ,[Usr_Login] = @Usr_Login
                              ,[Ludatetime] = GETDATE()
                                    ,Mph_DataType = @Mph_DataType
                                    ,Mph_SettingType = @Mph_SettingType
                                    ,Mph_CharValue = @Mph_CharValue
                         WHERE Mph_PolicyCode = @Mph_PolicyCode
                        ", CommandType.Text, param);

                    dal.CommitTransaction();
                    CommonProcedures.showMessageInformation("Successfully updated parameter");
                }
                catch (Exception er)
                {
                    dal.RollBackTransaction();
                }
                finally
                {
                    dal.CloseDB();
                }
            }
        }

    }
}
