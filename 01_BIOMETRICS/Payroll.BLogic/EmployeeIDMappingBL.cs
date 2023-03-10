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
    public class EmployeeIDMappingBL : Payroll.BLogic.BaseMasterBase
    {
        public DataSet SetUpLogMaster()
        {
            DataSet ds = null;
            using (DALHelper dal = new DALHelper(true))
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(
                            @"
                            SELECT Tel_IDNo AS 'EmployeeID'
                                  ,Tel_LastName AS 'Lastname'
                                  ,Tel_FirstName AS 'Firstname'
                                  ,Tel_MiddleName AS 'Middlename'
                                  ,Tel_CostcenterName AS 'CostCenter'
                                  ,Tel_PositionName AS 'Position'
                                  ,Tel_Gender AS 'Gender'
                              FROM T_EmpLog
                            WHERE Tel_RecordStatus = 'A'
                            ");
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

        public DataSet SetUpSelectedFingerPrintTable(String TableName)
        { 
            DataSet ds1 = new DataSet();
            using (DALHelper dal = new DALHelper(true))
            {
                try
                {
                    dal.OpenDB();
                    ds1 = dal.ExecuteDataSet(string.Format(Payroll.BLogic.CommonDeviceQueries.SelectMappingTableDisplay(TableName)), CommandType.Text);
                }
                catch (Exception er)
                {
                    CommonProcedures.showMessageError(er.Message);
                }
                finally
                {
                    dal.CloseDB();
                }

                return ds1;
            }
        }

        public bool  UpdateDeviceMapID(String table , ParameterInfo[] Param)
        {
            int affected = 0;
            using (DALHelper dal = new DALHelper(true))
            {
                try
                {
                    dal.OpenDB();
                    dal.BeginTransaction();

                    affected = dal.ExecuteNonQuery(
                        string.Format(@"
                        UPDATE [{0}]
                           SET [Trf_EmployeeIDMapping] = @Trf_EmployeeIDMapping
                              ,[Usr_Login] = @Usr_Login
                              ,[Ludatetime] = GETDATE()
                         WHERE [Trf_EmployeeID] = @Trf_EmployeeID
                              
                        ", table), CommandType.Text, Param);

                    dal.CommitTransaction();
                    CommonProcedures.showMessageInformation("Successfully Updated Terminal Device ID Mapping!");
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

            return affected==1?true:false;
        }

        public bool AutoMapping(String TableName,String User, String CompanyCode)
        {
            int affected = 0;

            using (DALHelper dal = new DALHelper(true))
            {
                try
                {
                    dal.OpenDB();
                    dal.BeginTransaction();

                    affected = dal.ExecuteNonQuery(
                        string.Format(@"
                        UPDATE [{0}]
                           SET [Trf_EmployeeIDMapping] = [Trf_EmployeeID]
                              ,[Usr_Login] = '{1}'
                              ,[Ludatetime] = GETDATE()
                         WHERE [Trf_EmployeeIDMapping] IS NULL OR [Trf_EmployeeIDMapping] = ''
                        ", TableName, User), CommandType.Text);
                    dal.CommitTransaction();
                    CommonProcedures.showMessageInformation("Successfully Updated Terminal Device ID Mapping!");
                }
                catch (Exception er)
                {
                    CommonProcedures.showMessageError(er.Message);
                }
                finally
                {
                    dal.CloseDB();
                }

                return affected == 1 ? true : false;
            }
        }

        public bool UpdateRFID(String table, ParameterInfo[] Param)
        {
            int affected = 0;
            using (DALHelper dal = new DALHelper(true))
            {
                try
                {
                    dal.OpenDB();
                    dal.BeginTransaction();

                    affected = dal.ExecuteNonQuery(
                        string.Format(@"
                        UPDATE [{0}]
                           SET [Trf_RFID] = @Trf_RFID
                              ,[Usr_Login] = @Usr_Login
                              ,[Ludatetime] = GETDATE()
                         WHERE [Trf_EmployeeID] = @Trf_EmployeeID
                           AND [Trf_FingerIndex] = '0'     
                        ", table), CommandType.Text, Param);

                    dal.CommitTransaction();
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

            return affected == 1 ? true : false;
        }
    }
}
