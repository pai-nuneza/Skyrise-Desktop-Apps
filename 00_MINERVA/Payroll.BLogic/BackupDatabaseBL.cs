using System;
using System.Collections.Generic;
using System.Text;

using Payroll.DAL;
using CommonLibrary;
using System.Data;
using System.Configuration;

namespace Payroll.BLogic
{
    public class BackupDatabaseBL : BaseBL
    {
        public override int Add(System.Data.DataRow row)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override int Update(System.Data.DataRow row)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override int Delete(string code, string userLogin)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override System.Data.DataSet FetchAll()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override System.Data.DataRow Fetch(string code)
        {
            throw new Exception("The method or operation is not implemented.");
        }

      
        public string GetCurrentDBName()
        {
            string query = string.Format(@"SELECT Mpf_DatabaseName FROM M_Profile WHERE Mpf_DatabaseNo = '{0}'", LoginInfo.getUser().DBNumber);

            DataTable dtResult = new DataTable();
            using (DALHelper dal = new DALHelper(LoginInfo.getUser().CentralProfileName, true))
            {
                dal.OpenDB();

                dtResult = dal.ExecuteDataSet(query).Tables[0];

                dal.CloseDB();
            }

            return dtResult.Rows[0][0].ToString();
        }

        public string FetchServerName()
        {
            DataSet ds = new DataSet();

            #region query

            string qString = @"Select @@ServerName 'Server Name',
                                        'Date Stamp:' = Convert(VarChar(10), GetDate(),101)";

            #endregion

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text);

                dal.CloseDB();
            }
            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0][0].ToString();
            else
                return string.Empty;
        }

        public string FetchServerIPAddress()
        {
            DataSet ds = new DataSet();
            //object test;
            string qString = @"declare @ip varchar(40)
                                exec sp_get_ip_address @ip out
                                Select @ip ";

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();

                ds = dal.ExecuteDataSet(qString, CommandType.Text);

                dal.CloseDB();
            }
            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0][0].ToString();
            else
                return string.Empty;
        }

    }
}
