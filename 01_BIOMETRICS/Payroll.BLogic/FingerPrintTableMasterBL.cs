using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net.Mail;
using System.Data;
using System.Windows.Forms;
using System.Drawing;
using Payroll.DAL;
using System.Web;
using System.Text;
using System.Web.UI;
using CommonLibrary;

namespace Payroll.BLogic
{
    public class FingerPrintTableMasterBL : Payroll.BLogic.BaseMasterBase
    {
        public DataSet SetUpFingerTemplateMaster(string CompanyCode)
        {
            DataSet ds = null;
            using (DALHelper dal = new DALHelper(true))
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(
                            string.Format(@"
                            SELECT REPLACE([Mfp_TableName],LEFT([Mfp_TableName],7),'') AS [Table Name]
                                  ,[Mfp_Description] AS [Description]
                                  ,CASE WHEN [Mfp_RecordStatus] = 'A'
                                        THEN 'ACTIVE'
                                        ELSE 'CANCELLED' END [Status]
                                  ,[Ludatetime] AS [Last Updated]
                            FROM [dbo].[M_FingerPrint]
                            WHERE Mfp_CompanyCode = '{0}'
                            ", CompanyCode)
                            );
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

        public void AddNewFingTemplateTable(ParameterInfo[] param)
        {
            using (DALHelper dal = new DALHelper(true))
            {
                try
                {
                    dal.OpenDB();
                    dal.BeginTransaction();
                    dal.ExecuteNonQuery(
                        @"
                        INSERT INTO [M_FingerPrint]
                                   ([Mfp_CompanyCode]
                                   ,[Mfp_TableName]
                                   ,[Mfp_Description]
                                   ,[Mfp_RecordStatus]
                                   ,[Usr_Login]
                                   ,[Ludatetime])
                             VALUES
                                   (@Mfp_CompanyCode
                                   ,@Mfp_TableName 
                                   ,@Mfp_Description 
                                   ,@Mfp_RecordStatus 
                                   ,@Usr_login 
                                   ,GETDATE())
                        ", CommandType.Text, param);

                    dal.CommitTransaction();
                    CommonProcedures.showMessageInformation("New Group successfully added.");
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

        public bool CreateNewFingTemplateTable(String TableName)
        {
            bool proceed = false;

            using (DALHelper dal = new DALHelper(true))
            {
                try
                {
                    dal.OpenDB();
                    dal.BeginTransaction();
                    dal.ExecuteNonQuery(Payroll.BLogic.CommonDeviceQueries.CreateEmployeeFingerPrintTable(TableName));
                    dal.CommitTransaction();
                    proceed = true;
                }
                catch (Exception er)
                {
                    dal.RollBackTransaction();
                    proceed = false;
                    CommonProcedures.showMessageError(er.Message);
                }
                finally
                {
                    dal.CloseDB();
                }
            }
            return proceed;
        }

        public void ModifyFingerTemplateTable(ParameterInfo[] param)
        {
            using (DALHelper dal = new DALHelper(true))
            {
                try
                {
                    dal.OpenDB();

                    dal.BeginTransaction();
                    dal.ExecuteNonQuery(
                        @"
                        UPDATE [M_FingerPrint]
                           SET [Mfp_TableName] = @Mfp_TableName
                              ,[Mfp_Description] = @Mfp_Description
                              ,[Mfp_RecordStatus] = @Mfp_RecordStatus
                              ,[Usr_Login] = @Usr_login
                              ,[Ludatetime] = GETDATE()
                         WHERE [Mfp_TableName] = @Mfp_TableName
                            AND Mfp_CompanyCode = @Mfp_CompanyCode                              
                        ", CommandType.Text, param);
                    dal.CommitTransaction();

                    dal.BeginTransaction();
                    dal.ExecuteNonQuery(
                        @"
                        UPDATE [M_FingerPrint]
                           SET [Mfp_TableName] = @Mfp_TableName
                              ,[Mfp_Description] = @Mfp_Description
                              ,[Mfp_RecordStatus] = @Mfp_RecordStatus
                              ,[Usr_Login] = @Usr_login
                              ,[Ludatetime] = GETDATE()
                         WHERE [Mfp_TableName] = @Mfp_TableName
                              AND Mfp_CompanyCode = @Mfp_CompanyCode
                        ", CommandType.Text, param);
                    dal.CommitTransaction();
                    CommonProcedures.showMessageInformation("Group successfully updated.");
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


        public void DropFingerTemplateTable(ParameterInfo[] param, String TableName)
        {
            using (DALHelper dal = new DALHelper(true))
            {
                try
                {

                    dal.OpenDB();
                    dal.BeginTransaction();

                    dal.ExecuteNonQuery(@"
                        DELETE FROM [dbo].[M_FingerPrint]
                        WHERE Mfp_TableName=@Mfp_TableName
                            AND Mfp_CompanyCode = @Mfp_CompanyCode", CommandType.Text,param);

                    dal.ExecuteNonQuery(
                        String.Format(@"
                        IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{0}]') AND type in (N'U'))
                        DROP TABLE [dbo].[{0}]   
                        ",TableName));
                    dal.CommitTransaction();
                    CommonProcedures.showMessageInformation("Group successfully deleted.");
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
