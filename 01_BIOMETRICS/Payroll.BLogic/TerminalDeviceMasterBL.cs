using System;
using Payroll.DAL;
using System.Data;
using CommonLibrary;

namespace Payroll.BLogic
{
    public class TerminalDeviceMasterBL : Payroll.BLogic.BaseMasterBase
    {
        private string[] tmpDeviceIP;
        public DataSet SetUpTerminalDeviceGrid(string CompanyCode)
        {           
            DataSet ds = null;
            using (DALHelper dal = new DALHelper(true))
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(
                            string.Format(@" 
                                    SELECT Mtd_DeviceIP [Device IP]
                                    , Mtd_DeviceName [Device Name]
                                    , Mtd_LastLoadedTable [Last Loaded Group]
                                    , Mtd_VersionNo [Version No]
                                    , case when Mtd_RecordStatus = 'A'
	                                       then 'ACTIVE'
	                                       else 'CANCELLED'
	                                        end [Status]
                                    , Mtd_DevicePort [Device Port]
                                    , Mtd_RS232Com [RS232 Com]
                                    , Mtd_RS232BaudRate [RS232 Baud Rate]
                                    , Mtd_TableName [Table Name]
                                    , CASE Mtd_DefaultLogType 
                                                        WHEN 'I'
                                                            THEN 'IN'
                                                        WHEN 'O'
                                                            THEN 'OUT'
                                                        ELSE
                                                            'MULTI'   
                                                        END AS [Log Type]
                                 FROM M_TerminalDevice2
                                 WHERE Mtd_CompanyCode = '{0}'
                                 ORDER BY Mtd_RecordStatus, Mtd_DeviceName", CompanyCode));
                    //Decrypt IP - Rendell Uy (9/13/2016)
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        tmpDeviceIP = Encrypt.decryptText(ds.Tables[0].Rows[i]["Device IP"].ToString()).Split('_');
                        ds.Tables[0].Rows[i]["Device IP"] = tmpDeviceIP[0];
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

        public void AddNewProfile(ParameterInfo[] param)
        {
            string deviceIP = string.Empty;
            using (DALHelper dal = new DALHelper(true))
            {
                try
                {
                    dal.OpenDB();
                    dal.BeginTransaction();

                    dal.ExecuteNonQuery(
                        @"
                        INSERT INTO [M_TerminalDevice2]
                                      ([Mtd_CompanyCode]
                                      ,[Mtd_DeviceIP]
                                      ,[Mtd_DeviceName]
                                      ,[Mtd_DevicePort]
                                      ,[Mtd_RS232Com]
                                      ,[Mtd_RS232BaudRate]
                                      ,[Mtd_LastLoadedTable]
                                      ,[Mtd_TableName]
                                      ,[Mtd_RecordStatus]
                                      ,[Usr_Login]
                                      ,[Ludatetime]
                                      ,[Mtd_VersionNo])
                             VALUES
                                      (@Mtd_CompanyCode
                                      ,@Mtd_DeviceIP
                                      ,@Mtd_DeviceName
                                      ,@Mtd_DevicePort
                                      ,@Mtd_RS232Com
                                      ,@Mtd_RS232BaudRate
                                      ,@Mtd_LastLoadedTable
                                      ,@Mtd_TableName
                                      ,@Mtd_RecordStatus
                                      ,@Usr_Login
                                      ,GETDATE()
                                      ,@Mtd_VersionNo )
                        ", CommandType.Text, param);
                    DataTable dt = dal.ExecuteDataSet(string.Format(@"SELECT Mtd_ID
                                                                        , Mtd_DeviceIP 
                                                                        , Mtd_DeviceName
                                                                        FROM M_TerminalDevice2 
                                                                        WHERE Mtd_DeviceIP = '{0}' 
                                                                        AND Mtd_DeviceName = '{1}'
                                                                        AND Mtd_CompanyCode = '{2}'", param[0].Value
                                                                                                   , param[9].Value
                                                                                                   , param[10].Value)).Tables[0];
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        deviceIP = Encrypt.encryptText(dt.Rows[i]["Mtd_DeviceIP"].ToString() + "_" + dt.Rows[i]["Mtd_ID"].ToString());
                        dal.ExecuteNonQuery(string.Format(@"UPDATE M_TerminalDevice2
                                                            SET Mtd_DeviceIP = '{0}'
                                                            WHERE Mtd_DeviceIP = '{1}' 
                                                            AND Mtd_DeviceName = '{2}'
                                                            AND Mtd_ID = '{3}'
                                                            AND Mtd_CompanyCode = '{4}'", deviceIP
                                                                               , dt.Rows[i]["Mtd_DeviceIP"].ToString()
                                                                               , dt.Rows[i]["Mtd_DeviceName"].ToString()
                                                                               , dt.Rows[i]["Mtd_ID"].ToString()
                                                                               , param[10].Value));
                    }
                    dal.CommitTransaction();
                    CommonProcedures.showMessageInformation("New Terminal Device successfully added.");
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

        public void ModifyProfile(ParameterInfo[] param, string deviceName)
        {
            DataRow[] arrDeviceIP;
            using (DALHelper dal = new DALHelper(true))
            {
                try
                {
                    dal.OpenDB();
                    dal.BeginTransaction();
                    dal.BeginTransaction();
                    DataTable dt = dal.ExecuteDataSet(@"SELECT Mtd_ID
                                                        , Mtd_DeviceIP 
                                                        , Mtd_DeviceName
                                                        FROM M_TerminalDevice2").Tables[0];
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        tmpDeviceIP = Encrypt.decryptText(dt.Rows[i]["Mtd_DeviceIP"].ToString()).Split('_');
                        dt.Rows[i]["Mtd_DeviceIP"] = tmpDeviceIP[0];
                    }

                    arrDeviceIP = dt.Select("Mtd_DeviceIP = '" + param[0].Value + "' AND Mtd_DeviceName = '" + deviceName + "'");
                    if (arrDeviceIP.Length > 0)
                    dal.ExecuteNonQuery(string.Format(
                        @"
                        UPDATE [M_TerminalDevice2]
                           SET [Mtd_DevicePort] = @Mtd_DevicePort
                              ,[Mtd_DeviceName] = @Mtd_DeviceName
                              ,[Mtd_RS232Com] = @Mtd_RS232Com
                              ,[Mtd_RS232BaudRate] = @Mtd_RS232BaudRate
                              ,[Mtd_LastLoadedTable] = @Mtd_LastLoadedTable
                              ,[Mtd_TableName] = @Mtd_TableName
                              ,[Mtd_RecordStatus] = @Mtd_RecordStatus
                              ,[Usr_Login] = @Usr_Login
                              ,[Ludatetime] = GETDATE()
                              ,[Mtd_VersionNo] = @Mtd_VersionNo
                         WHERE [Mtd_DeviceIP] = '{0}'
                            AND Mtd_ID = '{1}'  
                            AND Mtd_CompanyCode = @Mtd_CompanyCode
                        ", Encrypt.encryptText(arrDeviceIP[0].ItemArray[1].ToString() + "_" + arrDeviceIP[0].ItemArray[0].ToString())
                                         , arrDeviceIP[0].ItemArray[0].ToString()), CommandType.Text, param);

                    dal.CommitTransaction();
                    CommonProcedures.showMessageInformation("Terminal Device successfully updated.");
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

        public void DeleteProfile(ParameterInfo[] param)
        {
            DataRow[] arrDeviceIP;
            string[] tmpDeviceIP;
            using (DALHelper dal = new DALHelper(true))
            {
                try
                {
                    dal.OpenDB();
                    dal.BeginTransaction();
                    DataTable dt = dal.ExecuteDataSet(@"SELECT Mtd_ID
                                                        , Mtd_DeviceIP 
                                                        , Mtd_DeviceName
                                                        FROM M_TerminalDevice2").Tables[0];
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        tmpDeviceIP =  Encrypt.decryptText(dt.Rows[i]["Mtd_DeviceIP"].ToString()).Split('_');
                        dt.Rows[i]["Mtd_DeviceIP"] = tmpDeviceIP[0];
                    }

                    arrDeviceIP = dt.Select("Mtd_DeviceIP = '" + param[0].Value + "' AND Mtd_DeviceName = '" + param[9].Value + "'");
                    if(arrDeviceIP.Length > 0)
                        dal.ExecuteNonQuery(string.Format(
                                            @" DELETE FROM [M_TerminalDevice2]
                                                WHERE [Mtd_DeviceIP] = '{0}'
                                                AND [Mtd_DeviceName] = '{1}'
                                                AND Mtd_ID = '{2}'  
                                                AND Mtd_CompanyCode = '{3}'                                                
                                        ", Encrypt.encryptText(arrDeviceIP[0].ItemArray[1].ToString() + "_" + arrDeviceIP[0].ItemArray[0].ToString())
                                         , arrDeviceIP[0].ItemArray[2].ToString()
                                         , arrDeviceIP[0].ItemArray[0].ToString()
                                         , param[10].Value));
                    dal.CommitTransaction();
                    CommonProcedures.showMessageInformation("Terminal Device successfully deleted.");
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
