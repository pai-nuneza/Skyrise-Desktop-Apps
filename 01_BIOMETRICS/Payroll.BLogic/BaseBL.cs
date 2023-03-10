using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using CommonLibrary;
using Payroll.DAL;


namespace Payroll.BLogic
{
    public abstract class BaseBL
    {
        public BaseBL()
        {

        }

        #region public

        public abstract int Add(DataRow row);
        public abstract int Update(DataRow row);
        public abstract int Delete(string code, string userLogin);
        public abstract DataSet FetchAll();
        public abstract DataRow Fetch(string code);
        
        public static DataSet searchTable(string selectCommand)
        {
            DataSet ds = new DataSet();
            string sqlSelectCommand = selectCommand;

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sqlSelectCommand);
                dal.CloseDB();
            }
            return ds;
        }
        
        public static DataSet searchTable(string selectCommand, string status)
        {
            DataSet ds = new DataSet();
            string sqlSelectCommand = selectCommand;
            ParameterInfo[] param = new ParameterInfo[1];
            param[0] = new ParameterInfo("@status", status, SqlDbType.VarChar);

            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(sqlSelectCommand, CommandType.Text, param);
                ds = dal.ExecuteDataSet(selectCommand, CommandType.Text, param);
                
                dal.CloseDB();
            }
            return ds;
        }
        //end
        #endregion//public

        #region protected

        protected DateTime GetServerDate()
        {
            string sqlQuery = @"Select Convert(char(10),Getdate(),101) AS ServerDate";

            DateTime date;
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                date = Convert.ToDateTime(dal.ExecuteScalar(sqlQuery, CommandType.Text));
                dal.CloseDB();
            }

            return date;
        }
        #endregion//protected

        //added by louie 01/13/2007
        public static bool CompareLudatetime(DateTime orig, DateTime compare)
        {
            if (string.Compare(orig.ToString(),compare.ToString(),true)==0)
                return true;
            else
                return false;
        }
        //end

        protected string GetValue(object objVal)
        {
            if (objVal != null)
                return objVal.ToString().Trim();
            else
                return String.Empty;
        }

        protected Int32 getIntValue(object obj)
        {
            Int32 retval = 0;
            try
            {
                retval = Convert.ToInt32(obj.ToString().Replace(",","").Split('.')[0]);
            }
            catch (Exception)
            {
                retval = 0;
            }
            return retval;
        }

        protected decimal getDecimalValue(object objVal)
        {
            Decimal retval = Convert.ToDecimal(0.0);
            try
            {
                retval = Convert.ToDecimal(objVal);
            }
            catch (Exception)
            {
                retval = Convert.ToDecimal(0.0);
            }
            return retval;
        }

        public static DataRow getCompanyLogo()
        {
            DataSet ds = new DataSet();

            string query = @"Select Ccd_CompanyLogo From T_CompanyMaster";
            using (DALHelper dal = new DALHelper())
            {
                dal.OpenDB();
                ds = dal.ExecuteDataSet(query, CommandType.Text);
                dal.CloseDB();
            }
            return ds.Tables[0].Rows[0];

        }
    }
}
