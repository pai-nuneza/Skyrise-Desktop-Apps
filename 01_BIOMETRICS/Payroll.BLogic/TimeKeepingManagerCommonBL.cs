using System;
using System.Text;
using System.Data;
using Payroll.DAL;
using CommonLibrary;

namespace Payroll.BLogic
{
    public class TimeKeepingManagerCommonBL 
    {
        public DataSet GetTerminalDeviceList(string CompanyCode)
        {
            DataSet ds = null;
            using (DALHelper dal = new DALHelper(true))
            {
                try
                {
                    dal.OpenDB();
                    ds = dal.ExecuteDataSet(string.Format(@"SELECT [Mtd_DeviceIP] AS DEVICEIP
                                                         ,[Mtd_DeviceName] AS DEVICENAME
                                                      FROM [M_TerminalDevice2]
                                                      WHERE Mtd_RecordStatus='A'
                                                            AND Mtd_CompanyCode = '{0}'", CompanyCode));
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        ds.Tables[0].Rows[i]["DEVICEIP"] = Encrypt.decryptText(ds.Tables[0].Rows[i]["DEVICEIP"].ToString());
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
